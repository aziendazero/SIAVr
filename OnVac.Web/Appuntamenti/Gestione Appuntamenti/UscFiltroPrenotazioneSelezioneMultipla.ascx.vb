Imports Onit.Database.DataAccessManager

Partial Class UscFiltroPrenotazioneSelezioneMultipla
    Inherits Common.UserControlPageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(sender As System.Object, e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Types "

    Public Enum tipo_valori_filtro As Integer
        Associazioni_Dosi = 0
        Cicli_Sedute = 1
        Vaccinazioni_Dosi = 2
    End Enum

    <Serializable()>
    Public Class Filtro1

        Public Property Codice As String
        Public Property Descrizione As String
        Public Property Valore As String

    End Class

    <Serializable()>
    Public Class Filtro2

        Public Property Codice As String
        Public Property Descrizione As String

    End Class

#End Region

#Region " Properties "

    Public Property Tipo() As tipo_valori_filtro
        Get
            Return Session("OnVacFiltroApp_" + Me.ClientID + "_Tipo")
        End Get
        Set(Value As tipo_valori_filtro)
            Session("OnVacFiltroApp_" + Me.ClientID + "_Tipo") = Value
        End Set
    End Property

    Public Property EscludiObsoleti() As Boolean
        Get
            If ViewState("obsoleti") Is Nothing Then ViewState("obsoleti") = False
            Return Convert.ToBoolean(ViewState("obsoleti"))
        End Get
        Set(value As Boolean)
            ViewState("obsoleti") = value
        End Set
    End Property
    Public Property ControlloDosi() As Integer
        Get
            If ViewState("ControlloDosi") Is Nothing Then ViewState("ControlloDosi") = 1
            Return ViewState("ControlloDosi")
        End Get
        Set(value As Integer)
            ViewState("ControlloDosi") = value
        End Set
    End Property

    Public Property TipoVisualizzazione() As String
        Get
            If ViewState("TipoVisualizzazione") Is Nothing Then ViewState("TipoVisualizzazione") = "1"
            Return ViewState("TipoVisualizzazione")
        End Get
        Set(value As String)
            ViewState("TipoVisualizzazione") = value
        End Set
    End Property

#End Region

    Protected Overrides Sub OnInit(e As EventArgs)

        If Not IsPostBack Then

            Dim tabella As String = String.Empty
            Dim campoCodice As String = String.Empty
            Dim campoDescrizione As String = String.Empty
            Dim campoObsoleto As String = String.Empty

            ' Layout
            Select Case Tipo
                Case tipo_valori_filtro.Associazioni_Dosi
                    rdbFiltro1.Text = "Associazioni"
                    rdbFiltro2.Text = "Dosi"
                    tabella = "t_ana_associazioni"
                    campoCodice = "ass_codice"
                    campoDescrizione = "ass_descrizione"
                    If Me.EscludiObsoleti Then
                        campoObsoleto = "ass_obsoleto"
                    End If

                Case tipo_valori_filtro.Cicli_Sedute
                    rdbFiltro1.Text = "Cicli"
                    rdbFiltro2.Text = "Sedute"
                    tabella = "t_ana_cicli"
                    campoCodice = "cic_codice"
                    campoDescrizione = "cic_descrizione"
                    If Me.EscludiObsoleti Then
                        campoObsoleto = "cic_obsoleto"
                    End If

                Case tipo_valori_filtro.Vaccinazioni_Dosi
                    rdbFiltro1.Text = "Vaccinazioni"
                    rdbFiltro2.Text = "Dosi"
                    tabella = "t_ana_vaccinazioni"
                    campoCodice = "vac_codice"
                    campoDescrizione = "vac_descrizione"
                    'If Me.EscludiObsoleti Then
                    ' campoObsoleto => Non gestito per le vaccinazioni
                    'End If

            End Select

            If rdbFiltro1.Checked Then
                dgrValori.Enabled = True
                chkList.Enabled = False
            Else
                dgrValori.Enabled = False
                chkList.Enabled = True
            End If

            ' Query
            Dim dt As New DataTable()
            Using dam As IDAM = OnVacUtility.OpenDam()
                dam.QB.AddTables(tabella)
                dam.QB.AddSelectFields(campoCodice + " Codice", campoDescrizione + " Descrizione")
                If Not String.IsNullOrWhiteSpace(campoObsoleto) Then
                    dam.QB.OpenParanthesis()
                    dam.QB.AddWhereCondition(campoObsoleto, Comparatori.Uguale, "N", DataTypes.Stringa)
                    dam.QB.AddWhereCondition(campoObsoleto, Comparatori.Is, "NULL", DataTypes.Replace, "OR")
                    dam.QB.CloseParanthesis()
                End If
                dam.QB.AddOrderByFields(campoDescrizione)
                dam.BuildDataTable(dt)
            End Using

            ' Riempimento datagrid
            dgrValori.DataSource = dt
            dgrValori.DataBind()

        End If

    End Sub

    ' Restituisce un datatable che rappresenta le impostazioni del filtro1.
    ' Colonne: Codice Descrizione, Valore
    Public Function getValoriSelezionatiFiltro1() As DataTable

        Dim dt As New DataTable()
        dt.Columns.Add("Codice")
        dt.Columns.Add("Descrizione")
        dt.Columns.Add("Valore")

        If rdbFiltro1.Checked Then
            For i As Integer = 0 To dgrValori.Items.Count - 1
                If DirectCast(dgrValori.Items(i).FindControl("chk"), CheckBox).Checked Then
                    dt.Rows.Add(New String() {dgrValori.Items(i).Cells(dgrValori.getColumnNumberByKey("Codice")).Text, dgrValori.Items(i).Cells(dgrValori.getColumnNumberByKey("Descrizione")).Text, DirectCast(dgrValori.Items(i).FindControl("txtValori"), TextBox).Text})
                End If
            Next
        End If
        dt.AcceptChanges()

        Return dt

    End Function

    Public Function getValoriSelezionatiFiltro2() As DataTable
        Dim dt As New DataTable
        dt.Columns.Add("Codice")
        dt.Columns.Add("Descrizione")

        If rdbFiltro2.Checked Then
            For i As Integer = 0 To chkList.Items.Count - 1
                If chkList.Items(i).Selected Then
                    dt.Rows.Add(New String() {chkList.Items(i).Value, chkList.Items(i).Text})
                End If
            Next
        End If
        dt.AcceptChanges()

        Return dt
    End Function

    ' Seleziona, nel datagrid, gli elementi specificati nella colonna codice del datatable 
    Public Sub setValoriSelezionatiFiltro1(dt As DataTable)

        Dim i As Integer

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            For i = 0 To dgrValori.Items.Count - 1
                DirectCast(dgrValori.Items(i).FindControl("chk"), CheckBox).Checked = False
                DirectCast(dgrValori.Items(i).FindControl("txtValori"), TextBox).Text = ""
            Next
            Return
        End If

        rdbFiltro1.Checked = True
        rdbFiltro2.Checked = False

        dgrValori.Enabled = True
        chkList.Enabled = False

        Dim hashCodiciValori As Hashtable = CreaHashCodiciValori(dt)
        Dim cod As String = String.Empty

        For i = 0 To dgrValori.Items.Count - 1
            cod = dgrValori.Items(i).Cells(dgrValori.getColumnNumberByKey("Codice")).Text.ToLower
            If hashCodiciValori.Contains(cod) Then
                DirectCast(dgrValori.Items(i).FindControl("chk"), CheckBox).Checked = True
                DirectCast(dgrValori.Items(i).FindControl("txtValori"), TextBox).Text = hashCodiciValori(cod).ToString
            End If
        Next

    End Sub


    ' Seleziona, nella cheklist, gli elementi specificati nella colonna codice del datatable 
    Public Sub setValoriSelezionatiFiltro2(dt As DataTable)
        Dim i As Integer
        If IsNothing(dt) OrElse dt.Rows.Count = 0 Then
            For i = 0 To chkList.Items.Count - 1
                chkList.Items(i).Selected = False
            Next
            Return
        End If
        '
        rdbFiltro1.Checked = False
        rdbFiltro2.Checked = True
        '
        dgrValori.Enabled = False
        chkList.Enabled = True
        '
        Dim hashCodiciValori As Hashtable = CreaHashCodiciValori(dt)
        For i = 0 To chkList.Items.Count - 1
            If hashCodiciValori.Contains(chkList.Items(i).Value.ToLower) Then
                chkList.Items(i).Selected = True
            End If
        Next

    End Sub


    ' Restituisce una struttura hashtable a partire dal datatable passato come parametro. 
    ' Se il datatable ha la colonna valore la utilizza come valore da memorizzare nella hash
    ' altrimenti imposta la stringa vuota
    Private Function CreaHashCodiciValori(dt As DataTable) As Hashtable
        Dim hashCodiciValori As New Hashtable
        Dim i As Integer

        If Not IsNothing(dt.Columns("Valore")) Then
            For i = 0 To dt.Rows.Count - 1
                hashCodiciValori.Add(dt.Rows(i)("Codice").ToString.ToLower, dt.Rows(i)("Valore").ToString)
            Next
        Else
            For i = 0 To dt.Rows.Count - 1
                hashCodiciValori.Add(dt.Rows(i)("Codice").ToString.ToLower, "")
            Next
        End If

        Return hashCodiciValori
    End Function


    ' Restituisce una stringa con i codici degli elementi selezionati separati dal separatore specificato.
    ' Ogni elemento compare tante volte quanti sono i valori specificati nel suo textbox (1 se textbox vuoto)
    Public Function getStringaFiltro1(separatore As String) As String
        Dim stbRet As New System.Text.StringBuilder
        Dim stbCod As New System.Text.StringBuilder
        Dim i, j As Integer
        Dim s As String()
        Dim txt As String = ""
        Dim cod As String = ""
        '
        If rdbFiltro2.Checked Then
            Return ""
        End If
        '
        For i = 0 To dgrValori.Items.Count - 1
            If DirectCast(dgrValori.Items(i).FindControl("chk"), CheckBox).Checked Then
                txt = DirectCast(dgrValori.Items(i).FindControl("txtValori"), TextBox).Text
                cod = dgrValori.Items(i).Cells(dgrValori.getColumnNumberByKey("Codice")).Text
                If txt.Replace(",", "").Trim = "" Then
                    stbRet.AppendFormat("{0}{1}", cod, separatore)
                Else
                    ' Creo una stringa con tanti codici ripetuti quanti sono i valori del textbox
                    s = txt.Split(",")
                    For j = 0 To s.Length - 1
                        stbRet.AppendFormat("{0}{1}", cod, separatore)
                    Next
                End If
            End If
        Next
        If stbRet.Length > 0 Then
            stbRet.Remove(stbRet.Length - 1, 1)
        End If

        Return stbRet.ToString
    End Function


    Public Function getStringaFiltro2(separatore As String) As String
        Dim stbRet As New System.Text.StringBuilder
        Dim i As Integer

        If rdbFiltro1.Checked Then
            ' L'elenco dei valori di filtro2 deve essere creato dai textbox del datagrid
            Dim txt As String = ""
            '
            For i = 0 To dgrValori.Items.Count - 1
                If DirectCast(dgrValori.Items(i).FindControl("chk"), CheckBox).Checked Then
                    txt = DirectCast(dgrValori.Items(i).FindControl("txtValori"), TextBox).Text
                    If txt.Replace(",", "").Trim = "" Then
                        ' se non sono specificati valori, metto un separatore per tenerli allineati con i codici
                        stbRet.Append(separatore)
                    Else
                        ' se sono specificati, li concateno
                        stbRet.AppendFormat("{0}{1}", txt.Replace(",", separatore), separatore)
                    End If
                End If
            Next
            If stbRet.Length > 0 Then
                stbRet.Remove(stbRet.Length - 1, 1)
            End If
        Else
            ' L'elenco dei valori di filtro2 deve essere creato dalla cheklist
            For i = 0 To chkList.Items.Count - 1
                If chkList.Items(i).Selected Then
                    ' Concateno la stringa con i codici per formare la stringa finale da restituire
                    stbRet.AppendFormat("{0}{1}", chkList.Items(i).Value, separatore)
                End If
            Next
            If stbRet.Length > 0 Then
                stbRet.Remove(stbRet.Length - 1, 1)
            End If
        End If

        Return stbRet.ToString
    End Function


    Private Sub rdbFiltro1_CheckedChanged(sender As Object, e As System.EventArgs) Handles rdbFiltro1.CheckedChanged
        ImpostaLayout(1)
    End Sub


    Private Sub rdbFiltro2_CheckedChanged(sender As Object, e As System.EventArgs) Handles rdbFiltro2.CheckedChanged
        ImpostaLayout(2)
    End Sub

    Private Sub ImpostaLayout(filtro_selezionato As Integer)
        Dim i As Integer
        If filtro_selezionato = 1 Then
            dgrValori.Enabled = True
            chkList.Enabled = False
            '
            ' Cancello eventuali check della datalist
            For i = 0 To chkList.Items.Count - 1
                chkList.Items(i).Selected = False
            Next
        Else
            dgrValori.Enabled = False
            chkList.Enabled = True
            '
            ' Cancello eventuali check del datagrid
            For i = 0 To dgrValori.Items.Count - 1
                DirectCast(dgrValori.Items(i).FindControl("chk"), CheckBox).Checked = False
                DirectCast(dgrValori.Items(i).FindControl("txtValori"), TextBox).Text = ""
            Next
        End If
    End Sub


    ' Restituisce la stringa con i valori separati da punti e virgole e, tra parentesi, ciò che è stato specificato 
    ' nel textbox relativo.
    Public Function getStringaFormattata() As String
        Dim stbRet As New System.Text.StringBuilder
        Dim i As Integer
        Dim txt As String = ""
        '
        If rdbFiltro1.Checked Then
            '
            ' Se è impostato il filtro 1, la stringa risultante deve essere del tipo: v1(c1,c2), v2(c3),...
            ' I valori v1, v2 rapprepresentano le descrizioni presenti nel datagrid, e i valori c1,c2,... sono 
            ' le stringhe inserite nei rispettivi checkbox.
            For i = 0 To dgrValori.Items.Count - 1
                If DirectCast(dgrValori.Items(i).FindControl("chk"), CheckBox).Checked Then
                    ' Contenuto del textbox associato
                    txt = DirectCast(dgrValori.Items(i).FindControl("txtValori"), TextBox).Text
                    If txt.Replace(",", "").Trim = "" Then
                        txt = ""
                    Else
                        txt = "(" + txt + ")"
                    End If
                    stbRet.AppendFormat("{0}{1}; ", dgrValori.Items(i).Cells(dgrValori.getColumnNumberByKey("Descrizione")).Text, txt)
                End If
            Next
            If stbRet.Length > 0 Then
                stbRet.Remove(stbRet.Length - 2, 2)
            End If
        Else
            '
            ' L'elenco dei valori di filtro2 deve essere creato dalla checklist
            Dim stbTemp As New System.Text.StringBuilder
            For i = 0 To chkList.Items.Count - 1
                If chkList.Items(i).Selected Then
                    ' Concateno la stringa con i codici per formare la stringa finale da restituire
                    stbTemp.AppendFormat("{0},", chkList.Items(i).Value)
                End If
            Next
            If stbTemp.Length > 0 Then
                stbTemp.Remove(stbTemp.Length - 1, 1)
                stbRet.AppendFormat("{0}: {1}", rdbFiltro2.Text, stbTemp)
            End If
        End If

        Return stbRet.ToString
    End Function

    Public Function Filtro1Selected() As Boolean
        Return rdbFiltro1.Checked
    End Function

    Public Function Filtro2Selected() As Boolean
        Return rdbFiltro2.Checked
    End Function

    Private Sub UscFiltroPrenotazioneSelezioneMultipla_Load(sender As Object, e As EventArgs) Handles Me.Load
        If TipoVisualizzazione = "1" Then
            trRdbFilter.Visible = True
            tdCheckList.Visible = True
        Else
            trRdbFilter.Visible = False
            tdCheckList.Visible = False
        End If
    End Sub

End Class
