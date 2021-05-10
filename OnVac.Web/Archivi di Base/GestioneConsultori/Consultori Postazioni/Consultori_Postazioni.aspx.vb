Imports Infragistics.WebUI
Imports Infragistics.WebUI.UltraWebNavigator

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.Web.UI


Partial Class OnVac_Consultori_Postazioni
    Inherits OnVac.Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(sender As System.Object, e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Proprietà "

    Public Property dtaGruppi() As DataTable
        Get
            Return Session("OnVac_dtaGruppi")
        End Get
        Set(Value As DataTable)
            Session("OnVac_dtaGruppi") = Value
        End Set
    End Property

    Public Property dtaPostazioni() As DataTable
        Get
            Return Session("OnVac_dtaPostazioni")
        End Get
        Set(Value As DataTable)
            Session("OnVac_dtaPostazioni") = Value
        End Set
    End Property

    Public Property dtaAssociazioni() As DataTable
        Get
            Return Session("OnVac_dtaAssociazioni")
        End Get
        Set(Value As DataTable)
            Session("OnVac_dtaAssociazioni") = Value
        End Set
    End Property

    Public Property IsLoaded() As Boolean
        Get
            Return Session("OnVac_IsLoaded")
        End Get
        Set(Value As Boolean)
            Session("OnVac_IsLoaded") = Value
        End Set
    End Property

#End Region

#Region " Eventi Page "

    '***********************************************************
    'Nella tabella T_ANA_LINK_CONSULTORI_GP la colonna tipo
    'identifica il tipo di oggetto associato (computer, gruppo)
    'in cui la colonna COG_TIPO è:
    '1. Gruppo
    '2. Postazione
    '***********************************************************
    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then
            SessionInit()
            CaricaAssociazioni(Me.tvwGP)
            If Not tvwGP.SelectedNode Is Nothing Then
                AggiornaTxtAssociazione(Me.tvwGP.SelectedNode)
            End If ' tvwGP.GetNodeFromIndex(tvwGP.SelectedNodeIndex))
            'slokkiamo il paziente lokkato...
            Me.OnitLayout21.lock.EndLock(OnVacUtility.Variabili.PazId)
        End If

        Me.lblWarning.Text = String.Empty
        LoadInfo()

    End Sub

#End Region

#Region " Eventi Controlli "

    Private Sub ToolBar_ButtonClicked(sender As System.Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        If CheckLocked() Then Exit Sub

        Select Case be.Button.Key

            Case "btnSalva"
                Salva()
                CaricaAssociazioni(tvwGP)

            Case "btnAnnulla"
                dtaAssociazioni.RejectChanges()
                CaricaAssociazioni(tvwGP)
                Me.OnitLayout21.Busy = False

        End Select

    End Sub

    Private Sub btnRiassocia_Click(sender As System.Object, e As System.EventArgs) Handles btnRiassocia.Click

        If CheckLocked() Then Exit Sub

        Dim ret As String = Associa(Me.tvwGP.SelectedNode, Me.txtConsultorio.Codice)
        If ret <> String.Empty Then
            Me.lblWarning.Text = "ATTENZIONE! Questa associazione nasconde l'associazione del gruppo! Associazione nascosta: " & ret
        End If

        CaricaAssociazioni(Me.tvwGP)

    End Sub

    Private Sub tvwGP_NodeSelectionChanged(sender As Object, e As Infragistics.WebUI.UltraWebNavigator.WebTreeNodeEventArgs) Handles tvwGP.NodeSelectionChanged

        If CheckLocked() Then Exit Sub

        AggiornaTxtAssociazione(e.Node)

        If Me.txtConsultorio.Codice = String.Empty Then
            Me.btnRiassocia.Text = "Associa"
        Else
            Me.btnRiassocia.Text = "Riassocia"
        End If

    End Sub

    Private Sub OnitLayout21_BusyChanged(sender As System.Object, e As System.EventArgs, State As Boolean)

        If State Then
            Me.OnitLayout21.lock.Lock("0", Nothing, 3)
        Else
            Me.OnitLayout21.lock.EndLock(0)
        End If

    End Sub

    Private Sub OnitLayout21_TimeoutLock(sender As System.Object, e As System.EventArgs, int As Short)

        Me.lblWarning.Text = "Il periodo di inattività è stato eccessivo. Il blocco dell'applicazione è scaduto. E' possibile che altri utenti abbiano preso il controllo dell'applicazione e abbiano modificato i dati. Chiudere senza salvare!"

    End Sub

#End Region

#Region " Private "

    Private Sub SessionInit()

        If Not Me.dtaGruppi Is Nothing Then Me.dtaGruppi.Dispose()
        If Not Me.dtaPostazioni Is Nothing Then Me.dtaPostazioni.Dispose()
        If Not Me.dtaAssociazioni Is Nothing Then Me.dtaAssociazioni.Dispose()

        Me.dtaGruppi = New DataTable()
        Me.dtaPostazioni = New DataTable()
        Me.dtaAssociazioni = New DataTable()

        Me.IsLoaded = False

    End Sub

    'Carica il TreeView con i gruppi e le postazioni presenti nel database
    Private Sub CaricaAssociazioni(tvwTreeView As UltraWebTree)

        Dim rFind As DataRow

        'Cancella tutto
        tvwTreeView.Nodes.Clear()

        Dim DAM As IDAM = OnVacUtility.OpenDam()

        Try

            If Not Me.IsLoaded Then
                DAM.QB.NewQuery()
                DAM.QB.AddTables("T_ANA_LINK_CONSULTORI_GP")
                DAM.QB.AddSelectFields("COG_GP_CODICE, COG_CNS_CODICE, COG_TIPO")
                DAM.BuildDataTable(Me.dtaAssociazioni)
                Me.dtaAssociazioni.PrimaryKey = New DataColumn() {Me.dtaAssociazioni.Columns("COG_GP_CODICE"), Me.dtaAssociazioni.Columns("COG_TIPO")}
            End If

            'Ricerca i gruppi e li insericse nel treeview
            If Not Me.IsLoaded Then
                DAM.QB.NewQuery()
                DAM.QB.AddTables("T_ANA_GRUPPI_POSTAZIONI")
                DAM.QB.AddSelectFields("GRP_ID, GRP_DESCRIZIONE")
                DAM.BuildDataTable(Me.dtaGruppi)
            End If

            For i As Int16 = 0 To Me.dtaGruppi.Rows.Count - 1

                Dim node As New Node()
                node.Text = Me.dtaGruppi.Rows(i)("GRP_DESCRIZIONE").ToString()
                node.Tag = Me.dtaGruppi.Rows(i)("GRP_ID")
                node.ImageUrl = "../../../images/computer_group.gif"
                node.Expanded = True
                node.DataKey = "1"

                'Controlla che esista un'associaizone con un consultorio
                rFind = Me.dtaAssociazioni.Rows.Find(New Object() {Me.dtaGruppi.Rows(i)("GRP_ID"), "1"})
                If Not rFind Is Nothing Then
                    If rFind.RowState = DataRowState.Added Then
                        node.Style.ForeColor = Color.DarkBlue
                    Else
                        node.Style.ForeColor = Color.Blue
                    End If
                End If

                tvwTreeView.Nodes.Add(node)
                node = Nothing

            Next

            If Not Me.IsLoaded Then
                DAM.QB.NewQuery()
                DAM.QB.AddTables("T_ANA_POSTAZIONI")
                DAM.QB.AddSelectFields("POS_ID, POS_IP, POS_DESCRIZIONE, POS_GRUP_ID")
                DAM.BuildDataTable(Me.dtaPostazioni)
            End If

        Finally
            OnVacUtility.CloseDam(DAM)
        End Try

        For i As Int16 = 0 To Me.dtaPostazioni.Rows.Count - 1

            Dim node As New Node()
            node.Text = Me.dtaPostazioni.Rows(i)("POS_DESCRIZIONE").ToString()
            node.Tag = Me.dtaPostazioni.Rows(i)("POS_ID")
            node.ImageUrl = "../../../images/computer.gif"
            node.DataKey = "2|" & Me.dtaPostazioni.Rows(i)("POS_IP") & "|" & Me.dtaPostazioni.Rows(i)("POS_GRUP_ID") & ""

            'Controlla che esista un'associazione con un consultorio
            rFind = Me.dtaAssociazioni.Rows.Find(New Object() {Me.dtaPostazioni.Rows(i)("POS_GRUP_ID"), "1"})
            If Not rFind Is Nothing Then
                If rFind.RowState = DataRowState.Added Then
                    node.Style.ForeColor = Color.DarkBlue
                Else
                    node.Style.ForeColor = Color.Blue
                End If
            End If

            rFind = Me.dtaAssociazioni.Rows.Find(New Object() {Me.dtaPostazioni.Rows(i)("POS_ID"), "2"})
            If Not rFind Is Nothing Then
                If rFind.RowState = DataRowState.Added Then
                    node.Style.ForeColor = Color.Brown
                Else
                    node.Style.ForeColor = Color.Red
                End If
            End If

            'Lo inserisce in un gruppo se lo possiede (<> NULL)
            If Not Me.dtaPostazioni.Rows(i).Item("POS_GRUP_ID") Is DBNull.Value Then
                Dim n As Node = GetNodeFromID(tvwTreeView.Nodes, Me.dtaPostazioni.Rows(i)("POS_GRUP_ID"))
                If Not n Is Nothing Then
                    n.Nodes.Add(node)
                End If
            Else
                tvwTreeView.Nodes.Add(node)
            End If

            node = Nothing

        Next

        Me.IsLoaded = True

    End Sub

    '*********************************************
    'Ricerca un nodo in base alla proprietà ID
    '---IN:
    'nodes:     Collezione di nodi (tvw.Nodes)
    'Value:     Proprietà ID
    '---OUT:
    'TreeNode trovato oppure Nothing
    '*********************************************
    Private Function GetNodeFromID(nodes As Nodes, Value As String) As Node

        For Each n As Node In nodes
            If n.Tag = Value Then Return n
        Next

        Return Nothing

    End Function

    '*******************************************
    'Associa una postazioni o un gruppo a un consultorio
    '----IN:
    'nodo:          Nodo contenente il gruppo o la postazione nel TreeView
    'condiceCNS:    Codice del consultorio
    '----OUT:
    'Se si vuole associare un consultorio a un computer il cui gruppo a già associato
    'un consultorio si va incontro a una collisione nella quale verrà scelto
    'il consultorio della postazione. Se questo avviene la funzione avvisa
    'con il codice del consultorio in collisione. Altrimenti ""
    '*******************************************
    Private Function Associa(nodo As Node, codiceCNS As String) As String

        Dim tipo As Int16 = Integer.Parse(nodo.DataKey.Split("|")(0))     'Esiste sempre l'elemento 0
        Dim ret As String = String.Empty

        'Se il tipo=2 allora è una postazione quindi viene cercato (se esiste) il codice del suo gruppo
        If tipo = 2 Then
            Dim codGruppo As String = nodo.DataKey.Split("|")(2)     'Se è un gruppo esiste sempre 2

            'Controlla se il codice del gruppo è presente fra le associazioni
            Dim cns As DataRow = dtaAssociazioni.Rows.Find(New Object() {codGruppo, "1"})
            If Not cns Is Nothing Then
                If codiceCNS <> cns("COG_CNS_CODICE").ToString() And codiceCNS <> String.Empty Then
                    ret = cns("COG_CNS_CODICE").ToString()
                End If
            End If
        End If

        'Aggiunge (o modifica) l'associazione
        Dim findRow As DataRow = dtaAssociazioni.Rows.Find(New Object() {nodo.Tag, tipo})
        If findRow Is Nothing Then
            findRow = dtaAssociazioni.NewRow()
            findRow("COG_GP_CODICE") = nodo.Tag
            findRow("COG_CNS_CODICE") = codiceCNS
            findRow("COG_TIPO") = tipo
            dtaAssociazioni.Rows.Add(findRow)
        Else
            'Se il codice = "" allora cancello il record
            If codiceCNS <> String.Empty Then
                findRow("COG_CNS_CODICE") = codiceCNS
            Else
                findRow.Delete()
            End If
        End If

        Me.OnitLayout21.Busy = True

        Return ret

    End Function

    '**********************************************
    'Salva i dati sul database. Esegue un ciclo su tutti i record del datatable dtaAssociazioni
    'e controlla lo stato della riga per eseguire le varie query
    'Richiama le funzione AddRowDB, DelRowDB, ModRowDB
    '**********************************************
    Private Sub Salva()

        'Controlla lo stato di ogni riga
        Dim r As DataRow

        For Each r In Me.dtaAssociazioni.Rows
            Select Case r.RowState
                Case DataRowState.Added
                    AddRowDB(r)
                Case DataRowState.Deleted
                    DelRowDB(r)
                Case DataRowState.Modified
                    ModRowDB(r)
            End Select
        Next

        Me.dtaAssociazioni.AcceptChanges()

        OnVacUtility.CaricaVariabiliConsultorio(String.Empty, False)

        Me.OnitLayout21.Busy = False

    End Sub

    Private Sub AddRowDB(row As DataRow)

        Dim DAM As IDAM = OnVacUtility.OpenDam()

        'Inserisce il valore nella tabella
        With DAM.QB
            .NewQuery()
            .AddTables("T_ANA_LINK_CONSULTORI_GP")
            .AddInsertField("COG_GP_CODICE", row("COG_GP_CODICE"), DataTypes.Numero)
            .AddInsertField("COG_CNS_CODICE", row("COG_CNS_CODICE"), DataTypes.Stringa)
            .AddInsertField("COG_TIPO", row("COG_TIPO"), DataTypes.Stringa)
        End With

        Try
            DAM.ExecNonQuery(ExecQueryType.Insert)
        Finally
            OnVacUtility.CloseDam(DAM)
        End Try

    End Sub

    Private Sub DelRowDB(row As DataRow)

        Dim DAM As IDAM = OnVacUtility.OpenDam()

        row.RejectChanges()

        With DAM.QB
            .NewQuery()
            .AddTables("T_ANA_LINK_CONSULTORI_GP")
            .AddWhereCondition("COG_GP_CODICE", Comparatori.Uguale, row("COG_GP_CODICE"), DataTypes.Numero)
            .AddWhereCondition("COG_TIPO", Comparatori.Uguale, row("COG_TIPO"), DataTypes.Stringa)
        End With

        Try
            row.Delete()
            DAM.ExecNonQuery(ExecQueryType.Delete)
        Finally
            OnVacUtility.CloseDam(DAM)
        End Try

    End Sub

    Private Sub ModRowDB(row As DataRow)

        Dim DAM As IDAM = OnVacUtility.OpenDam()

        With DAM.QB
            .NewQuery()
            .AddTables("T_ANA_LINK_CONSULTORI_GP")
            .AddUpdateField("COG_CNS_CODICE", row("COG_CNS_CODICE"), DataTypes.Stringa)
            .AddUpdateField("COG_GP_CODICE", row("COG_GP_CODICE"), DataTypes.Numero)
            .AddUpdateField("COG_TIPO", row("COG_TIPO"), DataTypes.Stringa)
            .AddWhereCondition("COG_GP_CODICE", Comparatori.Uguale, row("COG_GP_CODICE"), DataTypes.Numero)
            .AddWhereCondition("COG_TIPO", Comparatori.Uguale, row("COG_TIPO"), DataTypes.Stringa)
        End With

        Try
            DAM.ExecNonQuery(ExecQueryType.Update)
        Finally
            OnVacUtility.CloseDam(DAM)
        End Try

    End Sub

    '*************************************************
    'Aggiorna il txtConsultorio con il consultorio
    'associato a nodo
    '----IN:
    'nodo:      Nodo da cui ottenere il consultorio
    '*************************************************
    Private Sub AggiornaTxtAssociazione(nodo As UltraWebNavigator.Node)

        'Cerca (se c'è l'associazione con un consultorio)
        Dim findRow As DataRow = dtaAssociazioni.Rows.Find(New Object() {nodo.Tag, nodo.DataKey.Split("|")(0)})

        'Aggiorna la finestra modale
        If Not findRow Is Nothing Then
            Me.txtConsultorio.Codice = findRow("COG_CNS_CODICE").ToString()
            Me.txtConsultorio.RefreshDataBind()
        Else
            Me.txtConsultorio.Codice = String.Empty
            Me.txtConsultorio.Descrizione = String.Empty
        End If

    End Sub

    Private Function CheckLocked() As Boolean

        If Me.OnitLayout21.lock.IsLocked(0) Then

            Me.lblWarning.Text = "La risorsa è momentaneamente bloccata da " &
                                 Me.OnitLayout21.lock.GetLockInfo(0).UserCodice &
                                 ". Attendere qualche minuto!"
            Return True

        End If

        Return False

    End Function

    '****************************************
    'Carica le informazioni della postazione attiva
    '****************************************
    Private Sub LoadInfo()

        Dim rowFind As DataRow

        Dim pc As CurrentMachineInfo = OnVacContext.CurrentMachine

        rowFind = dtaAssociazioni.Rows.Find(New Object() {pc.MachineID.ToStringOrDefault(), "2"})
        If rowFind Is Nothing Then
            rowFind = dtaAssociazioni.Rows.Find(New Object() {pc.MachineGroupID.ToStringOrDefault(), "1"})
        End If

        Dim str As String = "La postazione correntemente loggata è <b>" & pc.ComputerName &
                            "</b> su <b>" & pc.MachineGroupName & "</b><br>"

        If rowFind Is Nothing Then
            str &= "La postazione non è associata a nessun centro vaccinale<BR>"
        Else
            str &= "Il centro vaccinale associato è <b>" & rowFind.Item("COG_CNS_CODICE") & "</b><br>"
        End If

        str &= "<br>"

        Me.lblInfo.Text = str

    End Sub

#End Region

End Class
