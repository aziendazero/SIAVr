Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Web.Services
Imports System.Web.Script.Services

Imports Onit.Database.DataAccessManager


Partial Class OperazioniGruppo
    Inherits OnVac.Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub
    '[adesimone 071123]


    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Private "

    Private dtaTestate As DataTable
    Private hashTestateFast As Hashtable

    Private espandiTestate As System.Text.StringBuilder
    Private espandiPazienti As System.Text.StringBuilder

    Const MAXROW = 1000

#End Region

#Region " Properties "

    Private Property ReportPar(rpt As String) As ArrayList
        Get
            Return Session(rpt & "_param")
        End Get
        Set(ByVal Value As ArrayList)
            Session(rpt & "_param") = Value
        End Set
    End Property

    Private Property ReportPopUp_Filtro(rpt As String) As String
        Get
            Return Session(rpt)
        End Get
        Set(ByVal Value As String)
            Session(rpt) = Value
        End Set
    End Property

#End Region

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            'valorizzazione argomenti
            Me.rblArgomenti.DataSource = RecuperaArgomenti()
            Me.rblArgomenti.DataTextField = "LOA_DESCRIZIONE"
            Me.rblArgomenti.DataValueField = "LOA_CODICE"
            Me.rblArgomenti.DataBind()

            If Me.rblArgomenti.Items.Count > 0 Then Me.rblArgomenti.Items(0).Selected = True

            Me.chkOperazioni.DataSource = [Enum].GetNames(GetType(DataLogStructure.Operazione))
            Me.chkOperazioni.DataBind()

        End If

    End Sub

    Private Sub tlbOperazioniGruppo_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles tlbOperazioniGruppo.ButtonClicked

        Select Case be.Button.Key
            Case "btnOpGrpCerca"
                CaricaDataGrid(False)
            Case "btnOpGrpPulisci"
                Pulisci()
            Case "btnOpGrpStampa"
                ' C'era una funzione Stampa() che dentro aveva solo codice commentato, ho tolto tutto.
        End Select

    End Sub

    Private Sub dgrArgomento_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrArgomento.ItemDataBound

        If e.Item.ItemType <> ListItemType.Header And e.Item.ItemType <> ListItemType.Footer Then

            Dim dgr As DataGrid = e.Item.FindControl("dgrTestate")
            Dim codice As Label = e.Item.FindControl("lblGruppo")
            Dim img As HtmlImage = e.Item.FindControl("imgEspandi1")

            Dim tbl As HtmlTable = e.Item.FindControl("TableTestate")
            Dim tblInner As HtmlTable = e.Item.FindControl("TableTestateInner")
            Dim dgrPadre As String = e.Item.ClientID

            img.Attributes.Add("onclick", String.Format("Espandi('{0}','{1}')", tblInner.ClientID, img.ClientID))

            tblInner.Style.Add("display", "none")
            img.Attributes.Add("stato", "False")
            img.Src = "../../images/piu.gif"

            If dtaTestate Is Nothing Then CaricaTestate()

            Dim el As ArrayList = hashTestateFast(codice.Text)

            dgr.DataSource = el
            dgr.DataBind()

            If el Is Nothing Then
                img.Style.Add("display", "none")
            Else
                espandiTestate.Append(String.Format("EspandiTutto(espandi,'{0}','{1}','{2}','{3}');", dgr.ClientID, img.ClientID, tbl.ClientID, 2) & Chr(13) & Chr(10))
            End If

        End If

    End Sub

    Private Sub chkEsteso_CheckedChanged(sender As Object, e As System.EventArgs) Handles chkEsteso.CheckedChanged

        If dgrArgomento.Items.Count > 0 Then
            CaricaDataGrid(False)
        End If

    End Sub

    Private Function RecuperaArgomenti() As DataTable

        Dim dtaArgomenti As New DataTable()

        Using dam As IDAM = OnVacUtility.OpenDam()

            With dam.QB
                .NewQuery()
                .AddSelectFields("loa_descrizione", "loa_criticita", "loa_codice")
                .IsDistinct = True
                .AddTables("t_log_testata", "t_log_argomenti")
                .AddWhereCondition("lot_argomento", Comparatori.Uguale, "loa_codice", DataTypes.Join)
                .AddWhereCondition("lot_argomento", Comparatori.Diverso, "VARIAZIONE_CNS", DataTypes.Stringa)
                .AddWhereCondition("lot_gruppo", Comparatori.[IsNot], "null", DataTypes.Replace)
                .AddOrderByFields("loa_codice")
            End With

            dam.BuildDataTable(dtaArgomenti)

        End Using

        Return dtaArgomenti

    End Function

    Private Sub CaricaDataGrid(pulisci As Boolean)

        espandiTestate = New System.Text.StringBuilder()
        espandiPazienti = New System.Text.StringBuilder()

        Dim intLogGenerico As Int32 = 0
        Dim intEliminazione As Int32 = 0
        Dim intInserimento As Int32 = 0
        Dim intModifica As Int32 = 0
        Dim intEccezione As Int32 = 0

        Dim dtaOperazioniGruppo As New DataTable()

        Using dam As IDAM = OnVacUtility.OpenDam()

            ' Riempimento datatable operazioni
            With dam.QB

                .NewQuery()
                .AddSelectFields(.FC.Tronca("lot_data_operazione") & " data_operazione", "ute_descrizione", "lot_gruppo", "lot_argomento", "loa_descrizione", "lot_maschera")
                .AddTables("t_log_testata", "t_ana_utenti", "t_log_argomenti")
                .AddWhereCondition("lot_utente", Comparatori.Uguale, "ute_id", DataTypes.OutJoinLeft)
                .AddWhereCondition("lot_argomento", Comparatori.Uguale, rblArgomenti.SelectedValue, DataTypes.Stringa)
                .AddWhereCondition(.FC.Tronca("lot_data_operazione"), Comparatori.MaggioreUguale, Me.odpOpGrpDaData.Text, DataTypes.Data)

                If Me.odpOpGrpAdata.Text <> "" Then
                    .AddWhereCondition(.FC.Tronca("lot_data_operazione"), Comparatori.Minore, Me.odpOpGrpAdata.Data.AddDays(1), DataTypes.Data)
                End If

                Me.AddOperazioniSelezionateWhereConditions(dam)

                .AddWhereCondition("lot_gruppo", Comparatori.IsNot, "null", DataTypes.Replace)
                .AddWhereCondition("loa_codice", Comparatori.Uguale, "lot_argomento", DataTypes.Join)
                .AddGroupByFields(.FC.Tronca("lot_data_operazione"), "ute_descrizione", "lot_gruppo", "lot_argomento", "loa_descrizione", "lot_maschera")
                .AddOrderByFields("lot_gruppo DESC")

            End With

            dam.BuildDataTable(dtaOperazioniGruppo)

            If pulisci Then

                dtaOperazioniGruppo.Clear()

            Else

                ' Conteggio record raggruppati per operazione
                With dam.QB
                    .NewQuery()
                    .AddSelectFields("count(lot_codice) as n", "lot_operazione as operazione")
                    .AddTables("t_log_testata")
                    .AddWhereCondition("lot_argomento", Comparatori.Uguale, rblArgomenti.SelectedValue, DataTypes.Stringa)
                    .AddWhereCondition(.FC.Tronca("lot_data_operazione"), Comparatori.MaggioreUguale, Me.odpOpGrpDaData.Text, DataTypes.Data)

                    If odpOpGrpAdata.Text <> "" Then
                        .AddWhereCondition(.FC.Tronca("lot_data_operazione"), Comparatori.Minore, odpOpGrpAdata.Data.AddDays(1), DataTypes.Data)
                    End If

                    Me.AddOperazioniSelezionateWhereConditions(dam)

                    .AddWhereCondition("lot_gruppo", Comparatori.[IsNot], "null", DataTypes.Replace)
                    .AddGroupByFields("lot_operazione")
                End With

                Using dr As IDataReader = dam.BuildDataReader()

                    If Not dr Is Nothing Then

                        Dim operazione As Integer = dr.GetOrdinal("operazione")
                        Dim n As Integer = dr.GetOrdinal("n")

                        While dr.Read()

                            If Not dr.IsDBNull(operazione) Then

                                Select Case dr(operazione)
                                    Case Log.DataLogStructure.Operazione.Inserimento
                                        intInserimento = dr(n)
                                    Case Log.DataLogStructure.Operazione.Eliminazione
                                        intEliminazione = dr(n)
                                    Case Log.DataLogStructure.Operazione.Modifica
                                        intModifica = dr(n)
                                    Case Log.DataLogStructure.Operazione.Eccezione
                                        intEccezione = dr(n)
                                    Case Log.DataLogStructure.Operazione.Generico
                                        intLogGenerico = dr(n)
                                End Select

                            End If

                        End While

                    End If

                End Using

            End If

        End Using

        ' Valorizzazione label delle operazioni, con i conteggi
        Me.lblLogGenerico.Text = Me.GetStringNumeroLog(intLogGenerico)
        Me.lblEliminazione.Text = Me.GetStringNumeroLog(intEliminazione)
        Me.lblInserimento.Text = Me.GetStringNumeroLog(intInserimento)
        Me.lblModifica.Text = Me.GetStringNumeroLog(intModifica)
        Me.lblEccezione.Text = Me.GetStringNumeroLog(intEccezione)

        ' Bind del datagrid
        Me.dgrArgomento.DataSource = dtaOperazioniGruppo
        Me.dgrArgomento.DataBind()

        ' Script di espansione/compressione
        Dim strEspandiComprimi As String = "function EspandiComprimi(espandi){" & Chr(13) & Chr(10)
        strEspandiComprimi &= espandiTestate.ToString()
        strEspandiComprimi &= espandiPazienti.ToString()
        strEspandiComprimi &= "}"

        Me.OnitLayout.InsertRoutineJS(strEspandiComprimi)

    End Sub

    Private Function GetStringNumeroLog(count As Int32) As String

        Return String.Format("({0})", count.ToString())

    End Function

    Private Sub CaricaTestate()

        Using dbGenericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizLog As New Biz.BizLog(dbGenericProvider, OnVacContext.CreateBizContextInfos())

                dtaTestate = bizLog.GetDataTableTestateOperazioniGruppo(Me.rblArgomenti.SelectedValue, Me.odpOpGrpDaData.Data, Me.odpOpGrpAdata.Data, Me.GetOperazioniSelezionate())

            End Using
        End Using

        If hashTestateFast Is Nothing Then hashTestateFast = New Hashtable()

        Dim nArr As ArrayList

        Dim key As String
        Dim addedRow As Integer = 0

        For Each row As DataRow In dtaTestate.Rows

            key = row("lot_gruppo")

            If hashTestateFast.ContainsKey(key) Then
                hashTestateFast(key).add(row)
            Else
                nArr = New ArrayList()
                nArr.Add(row)
                hashTestateFast.Add(key, nArr)
            End If

            addedRow = addedRow + 1

            If addedRow >= MAXROW Then
                Me.OnitLayout.InsertRoutineJS("alert('Raggiunto il massimo numero di occorrenze visualizzabili. Provare a filtrare i dati con date più vicine o filtrare su una specifica operazione');")
                Exit For
            End If

        Next

    End Sub

    Private Function GetOperazioniSelezionate() As List(Of DataLogStructure.Operazione)

        Dim listOperazioni As New List(Of DataLogStructure.Operazione)()

        For Each item As ListItem In Me.chkOperazioni.Items

            If item.Selected Then
                listOperazioni.Add(DirectCast([Enum].Parse(GetType(DataLogStructure.Operazione), item.Value), DataLogStructure.Operazione))
            End If

        Next

        Return listOperazioni

    End Function

    Private Sub AddOperazioniSelezionateWhereConditions(dam As IDAM)

        Dim listOperazioni As List(Of DataLogStructure.Operazione) = Me.GetOperazioniSelezionate()

        If listOperazioni.Count > 0 Then

            With dam.QB

                .OpenParanthesis()

                .AddWhereCondition("lot_operazione", Comparatori.Uguale, Convert.ToInt32(listOperazioni(0)), DataTypes.Numero)

                If listOperazioni.Count > 1 Then
                    For i As Int16 = 1 To listOperazioni.Count - 1
                        .AddWhereCondition("lot_operazione", Comparatori.Uguale, Convert.ToInt32(listOperazioni(i)), DataTypes.Numero, "OR")
                    Next
                End If

                .CloseParanthesis()

            End With

        End If

    End Sub

    Public Function RecuperaImmagineOperazione(op As Integer) As String

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

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function CaricaCampi(lotCodice As Integer, operazione As String, appID As String, aziCodice As String) As String

        Dim dam As IDAM = Nothing

        Try
 
            dam = OnVacUtility.OpenDam()

        Catch ex As Exception
            Return ex.ToString()
        End Try

        With dam.QB

            .NewQuery()

            .AddSelectFields("loc_campo as Campo, null as Commenti, loc_valore_vecchio as Valore_Vecchio, loc_valore_nuovo as Valore_Nuovo")
            .AddTables("t_log_campi, t_log_record, t_log_testata")
            .AddWhereCondition("lor_record", Comparatori.Uguale, "loc_record", DataTypes.Join)
            .AddWhereCondition("lor_testata", Comparatori.Uguale, "lot_codice", DataTypes.Join)
            .AddWhereCondition("lot_gruppo", Comparatori.IsNot, "null", DataTypes.Replace)
            .AddWhereCondition("lot_codice", Comparatori.Uguale, lotCodice, DataTypes.Numero)

            Select Case operazione

                Case Log.DataLogStructure.Operazione.Eccezione,
                     Log.DataLogStructure.Operazione.Generico

                    .AddOrderByFields("loc_progressivo")

                Case Log.DataLogStructure.Operazione.Eliminazione,
                     Log.DataLogStructure.Operazione.Inserimento,
                     Log.DataLogStructure.Operazione.Modifica

                    .AddOrderByFields("loc_campo DESC")

            End Select

        End With

        Dim result As String = String.Empty

        Try

            Using idr As System.Data.IDataReader = dam.BuildDataReader()
                Dim ajr As New Renderator()
                result = ajr.ExportToHtml(idr, Renderator.Type.table)
            End Using

        Finally
            OnVacUtility.CloseDam(dam)
        End Try

        Return result

    End Function

    Private Sub Pulisci()

        If Me.rblArgomenti.Items.Count > 0 Then Me.rblArgomenti.SelectedIndex = 0

        Me.chkOperazioni.ClearSelection()

        Me.odpOpGrpDaData.Text = String.Empty
        Me.odpOpGrpAdata.Text = String.Empty

        CaricaDataGrid(True)

        Me.chkEsteso.Checked = False
        Me.chkEspandi.Checked = False

    End Sub

End Class
