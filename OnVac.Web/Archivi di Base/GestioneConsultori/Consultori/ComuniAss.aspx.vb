Imports Onit.Database.DataAccessManager
Imports Onit.Controls


Partial Class OnVac_ComuniAss
    Inherits OnVac.Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Properties "

    Property nMod() As Int16
        Get
            Return Session("OnVac_nMod")
        End Get
        Set(ByVal Value As Int16)
            Session("OnVac_nMod") = Value
        End Set
    End Property

    'tabella riempita dal database 
    Property comuni() As DataTable
        Get
            Return Session("OnVac_comuni")
        End Get
        Set(ByVal Value As DataTable)
            Session("OnVac_comuni") = Value
        End Set
    End Property

    'variabile che contiene il codice del consultorio in esame
    Property codConsultorio() As String
        Get
            Return Session("OnVac_codConsultorio")
        End Get
        Set(ByVal Value As String)
            Session("OnVac_codConsultorio") = Value
        End Set
    End Property

#End Region

#Region " Public "

    ' la variabile 'strJS'  contiene qualsiasi script che deve essere eseguito dal browser al caricamento successivo della pag.
    '(lo script verrà renderizzato tramite il comando asp 'response.write' nella pag HTML)
    Public strJS As String

#End Region

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack() Then

            nMod = 0
            codConsultorio = Request.QueryString("CODICE").ToString.Split("|")(0) 'il valore è passato dalla pag. 'consultori.aspx'
            
            If Not comuni Is Nothing Then
                comuni.Dispose()
            End If
            comuni = New DataTable()

            Using DAM As IDAM = OnVacUtility.OpenDam()

                DAM.QB.NewQuery()
                DAM.QB.AddTables("t_ana_consultori")
                DAM.QB.AddSelectFields("cns_descrizione")
                DAM.QB.AddWhereCondition("cns_codice", Comparatori.Uguale, codConsultorio, DataTypes.Stringa)

                LayoutTitolo.Text = DAM.ExecScalar()

                'la sql recupera i valori codice e descrizione dei comuni associati tramite un join 
                DAM.QB.NewQuery()
                DAM.QB.AddTables("t_ana_link_comuni_consultori,t_ana_comuni")
                DAM.QB.AddSelectFields("cco_com_codice,com_descrizione")
                DAM.QB.AddWhereCondition("cco_cns_codice", Comparatori.Uguale, codConsultorio, DataTypes.Stringa)
                DAM.QB.AddWhereCondition("com_codice", Comparatori.Uguale, "cco_com_codice", DataTypes.Join)
                DAM.QB.AddOrderByFields("com_descrizione")

                DAM.BuildDataTable(comuni)

                Dim com_key(0) As DataColumn
                com_key(0) = comuni.Columns("cco_com_codice")
                comuni.PrimaryKey = com_key

                'riempie il datagrid con la tab comuni
                dg_comuni.DataSource = comuni.DefaultView
                dg_comuni.DataBind()

            End Using

        End If

        strJS = ""

    End Sub

    Private Sub Salva()

        If checkLock() Then
            Exit Sub
        End If

        Dim DAM As IDAM = OnVacUtility.OpenDam()

        Try

            DAM.BeginTrans()

            comuni.PrimaryKey = Nothing

            For Each row As DataRow In comuni.Rows

                If row.RowState = DataRowState.Deleted Then
                    DAM.QB.NewQuery()
                    DAM.QB.AddTables("t_ana_link_comuni_consultori")
                    DAM.QB.AddWhereCondition("cco_cns_codice", Comparatori.Uguale, codConsultorio, DataTypes.Stringa)
                    DAM.QB.AddWhereCondition("cco_com_codice", Comparatori.Uguale, row("cco_com_codice", DataRowVersion.Original), DataTypes.Stringa)
                    DAM.ExecNonQuery(ExecQueryType.Delete)
                End If

            Next

            For Each row As DataRow In comuni.Rows

                If row.RowState = DataRowState.Added Then
                    DAM.QB.NewQuery()
                    DAM.QB.AddTables("t_ana_link_comuni_consultori")
                    DAM.QB.AddInsertField("cco_cns_codice", codConsultorio, DataTypes.Stringa)
                    DAM.QB.AddInsertField("cco_com_codice", row("cco_com_codice") & "", DataTypes.Stringa)
                    DAM.ExecNonQuery(ExecQueryType.Insert)
                End If

            Next

            Dim com_key(0) As DataColumn
            com_key(0) = comuni.Columns("cco_com_codice")
            comuni.PrimaryKey = com_key
            comuni.AcceptChanges()

            DAM.Commit()

        Catch exc As Exception
            DAM.Rollback()
            exc.InternalPreserveStackTrace()
            Throw

        Finally
            OnVacUtility.CloseDam(DAM)
        End Try

    End Sub

    Private Sub Inserisci()

        'aggiungo nuova riga alla tabella comuni
        Dim newrow As DataRow
        newrow = comuni.NewRow
        newrow("cco_com_codice") = "X"
        comuni.Rows.InsertAt(newrow, 0)

        'ricarica la tabella nel datagrid
        comuni.DefaultView.Sort = "com_descrizione"
        comuni.DefaultView.Sort = ""
        dg_comuni.EditItemIndex = 0
        dg_comuni.DataSource = comuni.DefaultView
        dg_comuni.DataBind()

        Me.ToolBar.Enabled = False

        Dim dvCom As New DataView(comuni)
        dvCom.RowFilter = "cco_com_codice <> 'X'"

        Dim stbFiltro As New System.Text.StringBuilder()
        For i As Int16 = 0 To dvCom.Count - 1
            If dvCom(i).Row.RowState <> DataRowState.Deleted Then
                stbFiltro.AppendFormat(" com_codice <> '{0}' and", dvCom(i)("cco_com_codice"))
            End If
        Next
        If (stbFiltro.Length = 0) Then
            stbFiltro.Append("'true'='true'")
        Else
            stbFiltro.Remove(stbFiltro.Length - 3, 3)
        End If
        stbFiltro.Append(" order by com_descrizione")

        CType(dg_comuni.Items(dg_comuni.EditItemIndex).FindControl("fm_com"), Onit.Controls.OnitModalList).Filtro = stbFiltro.ToString
        CType(dg_comuni.Items(dg_comuni.EditItemIndex).FindControl("fm_com"), Onit.Controls.OnitModalList).Codice = ""

        'lo script da il fuoco al primo campo di testo della riga da edit
        strJS = "<script language='javascript'>document.getElementById(""dg_comuni"").rows[" & dg_comuni.EditItemIndex + 1 & "].cells[1].firstChild.firstChild.focus()</script>"
        Me.OnitLayout31.Busy = True

        nMod += 1

    End Sub

    Private Sub dg_comuni_UpdateCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_comuni.UpdateCommand

        If checkLock() Then
            Exit Sub
        End If

        'recupera i valori immessi nella riga del datagrid e li inserisce nella tabella 'comuni'
        Dim cod As String = CType(e.Item.Cells(1).FindControl("fm_com"), Onit.Controls.OnitModalList).Codice.ToString()
        Dim des As String = CType(e.Item.Cells(1).FindControl("fm_com"), Onit.Controls.OnitModalList).Descrizione.ToString()

        comuni.Rows.Item(0)(0) = cod
        comuni.Rows.Item(0)(1) = des
        comuni.DefaultView.Sort = "com_descrizione"
        comuni.DefaultView.Sort = ""

        'ricarica il datagrid senza la riga editabile
        dg_comuni.EditItemIndex = -1
        dg_comuni.DataSource = comuni.DefaultView
        dg_comuni.DataBind()

        ToolBar.Enabled = True

    End Sub

    Private Sub dg_comuni_CancelCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_comuni.CancelCommand
        If checkLock() Then
            Exit Sub
        End If
        cancellaRiga(comuni.Rows(0))
        nMod -= 1
        If (nMod = 0) Then OnitLayout31.Busy = False
    End Sub

    Private Sub cancellaRiga(ByRef row As DataRow)
        row.Delete()
        dg_comuni.EditItemIndex = -1
        'controlla se eliminando la riga si deve visualizzare la pag precedente in quanto 
        'in quella corrente non ne rimangono 
        If (dg_comuni.Items.Count = 1) Then
            If (dg_comuni.CurrentPageIndex <> 0) Then
                dg_comuni.CurrentPageIndex -= 1
            End If
        End If
        'ricarico mil datagrid
        comuni.DefaultView.Sort = "com_descrizione"
        comuni.DefaultView.Sort = ""
        dg_comuni.DataSource = comuni.DefaultView
        dg_comuni.DataBind()
        ToolBar.Enabled = True
    End Sub

    'N.B.l'evento non rende la riga editabile, anche se il nome dice così, ma la elimina!!
    Private Sub dg_comuni_EditCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_comuni.EditCommand

        If checkLock() Then
            Exit Sub
        End If

        'controlla se si è in fase 'editabile'
        If (dg_comuni.EditItemIndex = -1) Then
            'calcola il numero di riga da eliminare
            Dim row As DataRow
            row = comuni.Rows.Find(CType(dg_comuni.Items.Item(e.Item.ItemIndex).FindControl("tb_codCom"), Label).Text)
            cancellaRiga(row)
            OnitLayout31.Busy = True
            nMod += 1
        Else
            strJS = "<script language='javascript'>alert('Cliccare AGGIORNA O ANNULLA della riga che si sta modificando prima di eliminare questa riga')</script>"
        End If

    End Sub

    Private Sub OnitLayout31_BusyChanged(Sender As Object, E As System.EventArgs, State As Boolean) Handles OnitLayout31.BusyChanged
        If (State) Then
            OnitLayout31.lock.Lock(0, codConsultorio)
        Else
            OnitLayout31.lock.EndLock(0)
        End If
    End Sub

    Private Function checkLock() As Boolean
        lb_warning.Visible = False
        If (OnitLayout31.lock.IsLocked(0) AndAlso codConsultorio = OnitLayout31.lock.GetLockInfo(0).Info) Then
            If Not (lb_warning.Visible) Then
                lb_warning.Visible = True
                lb_warning.Text = "ATTENZIONE.. L'APPLICAZIONE E' BLOCCATA DA " & OnitLayout31.lock.GetLockInfo(0).UserCodice & ".<BR>RIPROVARE FRA QUALCHE MINUTO."
            End If
            Return True
        Else
            Return False
        End If
    End Function

    Private Sub OnitLayout31_TimeoutLock(ByVal Sender As Object, ByVal E As System.EventArgs, ByVal int As String) Handles OnitLayout31.TimeoutLock
        lb_warning.Visible = True
        lb_warning.Text = "ATTENZIONE..IL BLOCCO SULL'APPLICAZIONE E' SCADUTO..CHIUDERE<BR> LA PAGINA SENZA SALVARE E RIPROVARE FRA QUALCHE MINUTO."
    End Sub

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btn_Salva"
                Salva()
                OnitLayout31.Busy = False
                nMod = 0

            Case "btn_Inserisci"
                If checkLock() Then
                    Exit Sub
                End If
                Inserisci()

            Case "btn_Annulla"
                If checkLock() Then
                    Exit Sub
                End If
                comuni.PrimaryKey = Nothing
                comuni.RejectChanges()
                comuni.DefaultView.Sort = "com_descrizione"
                comuni.DefaultView.Sort = ""
                dg_comuni.DataSource = comuni.DefaultView
                dg_comuni.DataBind()
                Dim com_key(0) As DataColumn
                com_key(0) = comuni.Columns("cco_com_codice")
                comuni.PrimaryKey = com_key
                OnitLayout31.Busy = False
                nMod = 0

        End Select

    End Sub

End Class
