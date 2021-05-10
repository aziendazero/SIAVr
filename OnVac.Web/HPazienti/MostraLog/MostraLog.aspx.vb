Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager


Partial Class MostraLog
    Inherits OnVac.Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Private Members "

    Private _filtroTestate As String
    Private dtaTestate As DataTable
    Private hashTestateFast As Hashtable
    Private _dtaArgomenti As DataTable
    Private dtaRecordLink As DataTable
    Private hashRecordLinkFast As Hashtable
    Private dtaRecord As DataTable
    Private hashRecordFast As Hashtable

#End Region

#Region " Properties "

    Private ReadOnly Property dtaArgomenti() As DataTable
        Get
            If _dtaArgomenti Is Nothing Then _dtaArgomenti = GetArgomenti()
            Return _dtaArgomenti
        End Get
    End Property

#End Region

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        _filtroTestate = String.Format(" AND LOT_PAZIENTE = {0} ", OnVacUtility.Variabili.PazId)

        If Not IsPostBack Then

            OnVacUtility.ImpostaIntestazioniPagina(Me.OnitLayout, Me.LayoutTitolo, Nothing, Me.Settings, Me.IsGestioneCentrale)

            Me.txtDaData.Text = System.DateTime.Today.Subtract(New System.TimeSpan(30, 0, 0, 0)).ToString
            Me.txtAdata.Text = System.DateTime.Today.ToString

            Me.chkOperazioni.DataSource = [Enum].GetNames(GetType(OnVac.Log.DataLogStructure.Operazione))
            Me.chkOperazioni.DataBind()

            Me.chkArgomenti.DataSource = dtaArgomenti()
            Me.chkArgomenti.DataTextField = "LOA_DESCRIZIONE"
            Me.chkArgomenti.DataValueField = "LOA_CODICE"
            Me.chkArgomenti.DataBind()

            CaricaDataGrid()

        End If

    End Sub

    Private Function GetArgomenti() As DataTable

        Using dam As IDAM = OnVacUtility.OpenDam()

            Dim SQL As String = "   SELECT LOA_DESCRIZIONE, LOA_CRITICITA, LOA_CODICE  FROM T_LOG_ARGOMENTI WHERE 1=1 "

            Dim stati As New List(Of String)()
            For Each c As ListItem In chkArgomenti.Items
                If c.Selected Then
                    stati.Add("'" & c.Value & "'")
                End If
            Next

            Dim statiStr As String = String.Join(",", stati.ToArray())
            If statiStr.Length > 0 Then
                SQL &= String.Format(" AND LOA_CODICE IN ({0})", statiStr)
            End If

            SQL = SQL & "  ORDER BY LOA_CODICE"

            Return dam.BuildDataTable(SQL, "Argomenti")

        End Using

        Return Nothing

    End Function

    Private Sub CaricaDataGrid()

        dgrArgomento.DataSource = dtaArgomenti
        dgrArgomento.DataBind()

    End Sub

    Private Sub dgrArgomento_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrArgomento.ItemDataBound

        If e.Item.ItemType <> ListItemType.Header And e.Item.ItemType <> ListItemType.Footer Then

            Dim dgr As DataGrid = e.Item.FindControl("dgrTestate")
            Dim codice As Label = e.Item.FindControl("lblArgomento")

            AddHandler dgr.ItemDataBound, AddressOf dgrTestateRecords_ItemDataBound

            If dtaTestate Is Nothing Then LoadTestate()

            Dim el As ArrayList = hashTestateFast(codice.Text)
            dgr.DataSource = el
            dgr.DataBind()

            Dim img As HtmlImage = e.Item.FindControl("imgEspandi1")
            img.Attributes.Add("onclick", String.Format("Espandi('{0}','{1}')", dgr.ClientID, img.ClientID))
            img.Attributes.Add("stato", False)

            If el Is Nothing Then img.Style.Add("display", "none")

        End If

    End Sub

    Private Sub LoadTestate()

        Dim stati As New List(Of String)()

        For Each c As ListItem In Me.chkOperazioni.Items
            If c.Selected Then
                stati.Add(CType([Enum].Parse(GetType(OnVac.Log.DataLogStructure.Operazione), c.Value), OnVac.Log.DataLogStructure.Operazione).ToString("D"))
            End If
        Next

        Dim statiStr As String = String.Join(",", stati.ToArray())

        Using dam As IDAM = OnVacUtility.OpenDam

            Dim SQL As String = "  SELECT DISTINCT LOT_CODICE, LOT_DATA_OPERAZIONE, NVL(UTE_DESCRIZIONE,'-') UTE_DESCRIZIONE, LOT_OPERAZIONE, LOT_MASCHERA, PAZ_COGNOME, PAZ_NOME, LOT_AUTOMATICO, LOT_STACK, LOT_ARGOMENTO"
            SQL += "  FROM T_LOG_TESTATA, T_ANA_UTENTI, T_PAZ_PAZIENTI"
            SQL += "  WHERE LOT_UTENTE=UTE_ID(+) "
            SQL += "  AND LOT_PAZIENTE=PAZ_CODICE "

            If Not String.IsNullOrEmpty(_filtroTestate) Then
                SQL += _filtroTestate
            End If

            If statiStr.Length > 0 Then
                SQL += String.Format(" AND LOT_OPERAZIONE IN ({0})", statiStr)
            End If

            If txtDaData.Text <> "" Then
                SQL += " AND LOT_DATA_OPERAZIONE>=TO_DATE('" & txtDaData.Text & "','dd/mm/yyyy')"
            End If

            If txtAdata.Text <> "" Then
                Dim dataFine As Date = Date.Parse(txtAdata.Text).AddDays(1)
                SQL += " AND LOT_DATA_OPERAZIONE<=TO_DATE('" & dataFine.ToShortDateString() & "','dd/mm/yyyy')"
            End If

            SQL += "  ORDER BY LOT_ARGOMENTO ASC, LOT_DATA_OPERAZIONE DESC"

            If dtaTestate Is Nothing Then dtaTestate = New DataTable
            If hashTestateFast Is Nothing Then hashTestateFast = New Hashtable

            dtaTestate = dam.BuildDataTable(SQL, "Testate")

        End Using

        For Each r As DataRow In dtaTestate.Rows

            Dim key As String = r("LOT_ARGOMENTO")

            If hashTestateFast.ContainsKey(key) Then
                hashTestateFast(key).add(r)
            Else
                Dim nArr As New ArrayList()
                nArr.Add(r)
                hashTestateFast.Add(key, nArr)
            End If

        Next

    End Sub

    Private Sub dgrTestateRecords_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs)

        If e.Item.ItemType <> ListItemType.Header And e.Item.ItemType <> ListItemType.Footer Then

            Dim dgr As DataList = e.Item.FindControl("dgrRecords")
            Dim codice As Label = e.Item.FindControl("lblCodice")

            AddHandler dgr.ItemDataBound, AddressOf dgrRecord_ItemDataBound

            If dtaRecord Is Nothing Then LoadRecord()

            Dim el As ArrayList = hashRecordLinkFast(codice.Text)
            dgr.DataSource = el
            dgr.DataBind()

            Dim img As HtmlImage = e.Item.FindControl("imgEspandi2")
            img.Attributes.Add("onclick", String.Format("Espandi('{0}','{1}')", dgr.ClientID, img.ClientID))
            img.Attributes.Add("stato", False)

            If el Is Nothing OrElse el.Count = 0 Then img.Style.Add("display", "none")

        End If

    End Sub

    Private Sub LoadRecord()

        Dim SQL As String

        Using dam As IDAM = OnVacUtility.OpenDam()

            SQL = " SELECT DISTINCT LOR_RECORD, LOT_OPERAZIONE, LOR_TESTATA"
            SQL += " FROM T_LOG_RECORD, T_LOG_TESTATA"
            SQL += " WHERE LOR_TESTATA=LOT_CODICE"

            If _filtroTestate <> "" Then
                SQL = SQL & _filtroTestate
            End If

            If dtaRecordLink Is Nothing Then dtaRecordLink = New DataTable()
            If hashRecordLinkFast Is Nothing Then hashRecordLinkFast = New Hashtable()

            dtaRecordLink = dam.BuildDataTable(SQL, "Records")

        End Using

        For Each r As DataRow In dtaRecordLink.Rows

            Dim key As String = r("LOR_TESTATA")

            If hashRecordLinkFast.ContainsKey(key) Then
                hashRecordLinkFast(key).add(r)
            Else
                Dim nArr As New ArrayList()
                nArr.Add(r)
                hashRecordLinkFast.Add(key, nArr)
            End If

        Next

    End Sub

    Private Sub dgrRecord_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataListItemEventArgs)

        If e.Item.ItemType <> ListItemType.Header And e.Item.ItemType <> ListItemType.Footer Then

            Dim dgr As DataGrid = e.Item.FindControl("dgrRecord")
            Dim codice As Label = e.Item.FindControl("lblCodice")
            Dim tipo As Label = e.Item.FindControl("lblTipo")

            If dtaRecord Is Nothing Then LoadCampiDatabase()

            Dim el As ArrayList = hashRecordFast(codice.Text)

            dgr.DataSource = el
            dgr.DataBind()

            Dim img As HtmlImage = e.Item.Parent.Parent.FindControl("imgEspandi2")
            If img.Style("Count") Is Nothing Then
                img.Style("Count") = 0
            Else
                If Not el Is Nothing Then
                    img.Style("Count") = Convert.ToInt32(img.Style("Count")) + el.Count
                End If
            End If

            If el Is Nothing AndAlso img.Style("Count") = 0 Then
                img.Style.Add("display", "none")
            Else
                img.Style.Add("display", "block")
            End If

            Dim op As Operazione = CType(tipo.Text, OnVac.Log.DataLogStructure.Operazione)

            If op = Operazione.Eccezione Then
                dgr.Columns(2).Visible = False
            ElseIf op = Operazione.Inserimento Then
                dgr.Columns(1).Visible = False
            ElseIf op = Operazione.Generico Then
                dgr.Columns(2).Visible = False
            ElseIf op = Operazione.Eliminazione Then
                dgr.Columns(2).Visible = False
            End If

        End If

    End Sub

    Private Sub LoadCampiDatabase()

        Dim stati As New List(Of String)()

        For Each c As ListItem In chkOperazioni.Items
            If c.Selected Then
                stati.Add(CType([Enum].Parse(GetType(OnVac.Log.DataLogStructure.Operazione), c.Value), OnVac.Log.DataLogStructure.Operazione).ToString("D"))
            End If
        Next

        Dim statiStr As String = String.Join(",", stati.ToArray())

        Using dam As IDAM = OnVacUtility.OpenDam()

            Dim SQL As String = " SELECT DISTINCT LOR_RECORD,LOC_CAMPO,LOC_CAMPO Campo, NVL(LOC_VALORE_VECCHIO,'NULL') ""Valore Vecchio"", NVL(LOC_VALORE_NUOVO,'NULL') ""Valore Nuovo"""
            SQL += " FROM T_LOG_CAMPI, T_LOG_RECORD,T_LOG_TESTATA"
            SQL += " WHERE LOR_RECORD=LOC_RECORD"
            SQL += " AND LOR_TESTATA=LOT_CODICE "

            If _filtroTestate <> "" Then
                SQL = SQL & _filtroTestate
            End If

            If statiStr.Length > 0 Then
                SQL = SQL & String.Format(" AND LOT_OPERAZIONE IN ({0})", statiStr)
            End If

            If txtDaData.Text <> "" Then
                SQL = SQL & " AND LOT_DATA_OPERAZIONE>=TO_DATE('" & txtDaData.Text & "','dd/mm/yyyy')"
            End If

            If txtAdata.Text <> "" Then
                Dim datasuccessiva As Date = Date.Parse(txtAdata.Text).AddDays(1)
                SQL = SQL & " AND LOT_DATA_OPERAZIONE<=TO_DATE('" & datasuccessiva.ToShortDateString & "','dd/mm/yyyy')"
            End If

            'SQL = SQL & " AND UPPER(COMS.COLUMN_NAME(+)) = UPPER(LOC_CAMPO)"
            'SQL = SQL & " AND COMS.TABLE_NAME = COLS.TABLE_NAME(+)"
            'SQL = SQL & " AND COLS.COLUMN_NAME(+) = COMS.COLUMN_NAME"
            'SQL = SQL & " AND OBJECT_NAME(+) = COLS.TABLE_NAME"
            'SQL = SQL & " AND (OBJECT_TYPE ='TABLE' OR COMS.TABLE_NAME IS NULL)"
            'SQL = SQL & " AND (SUBSTR(COLS.TABLE_NAME,0,5)<>'T_TMP' OR COLS.TABLE_NAME IS NULL) "
            SQL = SQL & " ORDER BY LOR_RECORD"

            If dtaRecord Is Nothing Then dtaRecord = New DataTable()
            If hashRecordFast Is Nothing Then hashRecordFast = New Hashtable()

            dtaRecord = dam.BuildDataTable(SQL, "Record")

        End Using

        For Each r As DataRow In dtaRecord.Rows

            Dim key As String = r("LOR_RECORD")

            If hashRecordFast.ContainsKey(key) Then
                hashRecordFast(key).add(r)
            Else
                Dim nArr As New ArrayList
                nArr.Add(r)
                hashRecordFast.Add(key, nArr)
            End If

        Next

        dtaRecord.Select()

    End Sub

    Public Function GetImageOperazion(op As Integer) As String

        Dim strFile As String = String.Empty

        Select Case op
            Case Operazione.Inserimento
                strFile = "op_inserimento.gif"
            Case Operazione.Eliminazione
                strFile = "op_eliminazione.gif"
            Case Operazione.Generico
                strFile = "op_log.gif"
            Case Operazione.Modifica
                strFile = "op_modifica.gif"
            Case Operazione.Eccezione
                strFile = "op_eccezione.gif"
        End Select

        Return strFile

    End Function

    Private Sub chkEsteso_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkEsteso.CheckedChanged

        Me.CaricaDataGrid()

    End Sub

    Private Sub ToolBar_ButtonClicked(sender As System.Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case be.Button.Key
            Case "Filtra"
                Me.CaricaDataGrid()
            Case "Pulisci"
                Me.PulisciFiltri()
        End Select

    End Sub

    Private Sub PulisciFiltri()

        For Each el As ListItem In chkArgomenti.Items
            el.Selected = False
        Next

        For Each el As ListItem In chkOperazioni.Items
            el.Selected = False
        Next

        txtDaData.Text = ""
        txtAdata.Text = ""

        Me.CaricaDataGrid()

    End Sub

End Class
