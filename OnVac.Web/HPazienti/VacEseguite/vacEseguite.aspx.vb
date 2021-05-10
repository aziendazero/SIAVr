Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.Controls

Imports Onit.OnAssistnet.OnVac.Collection
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Biz
Imports Onit.OnAssistnet.OnVac.Common.Controls
Imports Onit.Web.UI.WebControls.Validators


Partial Class OnVac_VacEseguite
    Inherits OnVac.Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    Protected WithEvents nascosto_mod As System.Web.UI.WebControls.TextBox
    Protected WithEvents LayoutTitolo_datipaz As System.Web.UI.WebControls.Panel

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(sender As System.Object, e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Consts "

    Private Const ScaleFactor = 3
    Private Const WARNING_REAZIONE As String = "WarningReazione"

    Private Const MAX_ESEGUITE_REAZIONE As Int16 = 3

#End Region

#Region " Classes "

    <Serializable()>
    Private Class DataGridFarm

        Public OriginalWidth As System.Collections.Specialized.StringCollection
        Public ExtendedWidth As System.Collections.Specialized.StringCollection

        Public Sub New()
            OriginalWidth = New System.Collections.Specialized.StringCollection
            ExtendedWidth = New System.Collections.Specialized.StringCollection
        End Sub

    End Class

#End Region

#Region " Variables "

    Public strJS As String

    Protected WithEvents ReazAvverseDetail As OnVac.ReazAvverseDetail

#End Region

#Region " Enums "

    Private Enum StatoPagina As Integer
        VacEseguiteListEdit = 0
        VacEseguiteListEditItem = 1
        VacEseguiteListView = 2
        ReazioniAvverseDetailView = 3
        DatiVaccinaliDaRecuperare = 4
    End Enum

    Protected Enum ModeViewDgr
        Dettaglio = 0
        NoDettaglio = 1
    End Enum

    Protected Enum OperazioniVaccinazioniEseguite
        ReazioneAvversa = 0
        ModificaDati = 1
        Cancellazione = 2
    End Enum

#End Region

#Region " Properties "

    Protected Property IndexDrgVacEsToDelete() As Int16
        Get
            Return ViewState("OnVac_IndexDrgVacEsToDelete")
        End Get
        Set(ByVal Value As Int16)
            ViewState("OnVac_IndexDrgVacEsToDelete") = Value
        End Set
    End Property

    Protected Property IndexDrgVacEsToDuplicate() As Int16
        Get
            Return ViewState("IndexDrgVacEsToDuplicate")
        End Get
        Set(ByVal Value As Int16)
            ViewState("IndexDrgVacEsToDuplicate") = Value
        End Set
    End Property

    Protected Property ModeViewDgrStatus() As ModeViewDgr
        Get
            Return ViewState("OnVac_ModeViewDgrStatus")
        End Get
        Set(ByVal Value As ModeViewDgr)
            ViewState("OnVac_ModeViewDgrStatus") = Value
        End Set
    End Property

    Protected Property ModeViewDgrPreviousStatus() As ModeViewDgr
        Get
            Return ViewState("OnVac_ModeViewDgrPreviousStatus")
        End Get
        Set(ByVal Value As ModeViewDgr)
            ViewState("OnVac_ModeViewDgrPreviousStatus") = Value
        End Set
    End Property

    Protected Property ord() As Hashtable
        Get
            Return ViewState("ord")
        End Get
        Set(ByVal Value As Hashtable)
            ViewState("ord") = Value
        End Set
    End Property

    Protected Property nMod() As Int16
        Get
            Return ViewState("OnVac_nMod")
        End Get
        Set(ByVal Value As Int16)
            ViewState("OnVac_nMod") = Value
        End Set
    End Property

    Private Property datiLottiEliminati() As List(Of Entities.DatiLottoEliminato)

        Get
            If ViewState("datiLottiEliminati") Is Nothing Then
                Return New List(Of Entities.DatiLottoEliminato)()
            End If

            Return DirectCast(ViewState("datiLottiEliminati"), Entities.DatiLottoEliminato()).ToList()
        End Get

        Set(ByVal Value As List(Of Entities.DatiLottoEliminato))
            ViewState("datiLottiEliminati") = Value.ToArray()
        End Set

    End Property

    Private Property dt_vacEseguite() As DataTable
        Get
            If Session("OnVac_dt_vacEseguite") Is Nothing Then Return Nothing
            Return DirectCast(Session("OnVac_dt_vacEseguite"), SerializableDataTableContainer).Data
        End Get
        Set(ByVal Value As DataTable)
            If Session("OnVac_dt_vacEseguite") Is Nothing Then
                Session("OnVac_dt_vacEseguite") = New SerializableDataTableContainer
            End If
            DirectCast(Session("OnVac_dt_vacEseguite"), SerializableDataTableContainer).Data = Value
        End Set
    End Property

    Private Property RowKey() As Object()
        Get
            Return Session("OnVac_rowKey")
        End Get
        Set(Value As Object())
            Session("OnVac_rowKey") = Value
        End Set
    End Property

    ''memorizza il filtro per la stampa del report nella pop-up (modifica 20/01/2005)
    'Public WriteOnly Property filtroReport() As String
    '    Set(Value As String)
    '        Session("ReazioniAvverse") = Value
    '    End Set
    'End Property

    'struttura in cui salvo i dati replicabili dopo che una riga è stata effettivamente modificata
    Protected Property objDatiDaReplicare() As Entities.VaccinazioneEseguita
        Get
            Return ViewState("OnVac_eseguite_datiDaReplicare")
        End Get
        Set(Value As Entities.VaccinazioneEseguita)
            ViewState("OnVac_eseguite_datiDaReplicare") = Value
        End Set
    End Property

    'struttura in cui salvo i dati replicabili prima che una riga venga effettivamente modificata
    Protected Property objDatiDaReplicarePrimaDiModifica() As Entities.VaccinazioneEseguita
        Get
            Return ViewState("OnVac_eseguite_datiDaReplicarePrimaDiModifica")
        End Get
        Set(Value As Entities.VaccinazioneEseguita)
            ViewState("OnVac_eseguite_datiDaReplicarePrimaDiModifica") = Value
        End Set
    End Property

    'contiene la data della reazione avversa modificata [modifica 14/02/2005]
    Protected Property dataVacEseguitaModificata() As Date
        Get
            Return Session("dataVacEseguitaModificata")
        End Get
        Set(Value As Date)
            Session("dataVacEseguitaModificata") = Value
        End Set
    End Property

    Public Property OrdineColonne() As String()
        Get
            Return ViewState("OrdineColonne")
        End Get
        Set(Value As String())
            ViewState("OrdineColonne") = Value
        End Set
    End Property

    Public Property StartSorting() As String
        Get
            Return ViewState("StartSorting")
        End Get
        Set(Value As String)
            ViewState("StartSorting") = Value
        End Set
    End Property

    ' Campo ordinamento nuovo datagrid 
    Private Property OrdVacEseguite_Campo() As String
        Get
            If Session("OnVac_Ord_VacEseguite_Campo") Is Nothing Then Session("OnVac_Ord_VacEseguite_Campo") = String.Empty
            Return Session("OnVac_Ord_VacEseguite_Campo").ToString()
        End Get
        Set(Value As String)
            Session("OnVac_Ord_VacEseguite_Campo") = Value
        End Set
    End Property

    ' Verso ordinamento nuovo datagrid 
    Private Property OrdVacEseguite_Verso() As String
        Get
            If Session("OnVac_Ord_VacEseguite_Verso") Is Nothing Then Session("OnVac_Ord_VacEseguite_Verso") = String.Empty
            Return Session("OnVac_Ord_VacEseguite_Verso").ToString()
        End Get
        Set(Value As String)
            Session("OnVac_Ord_VacEseguite_Verso") = Value
        End Set
    End Property

    ' Datatable liv.0
    Private Property dtAssociazioni() As DataTable
        Get
            Return Session("OnVac_dt_associazioni")
        End Get
        Set(Value As DataTable)
            Session("OnVac_dt_associazioni") = Value
        End Set
    End Property

    ' Datatable liv.1 (dettaglio)
    Private Property dgf() As DataGridFarm
        Get
            Return ViewState("OnVac_DataGridFarm")
        End Get
        Set(Value As DataGridFarm)
            ViewState("OnVac_DataGridFarm") = Value
        End Set
    End Property

    Private Property SelectedPageState() As StatoPagina
        Get
            Return ViewState("OnVac_VacEseguiteNew_SelectedPageState")
        End Get
        Set(Value As StatoPagina)
            ViewState("OnVac_VacEseguiteNew_SelectedPageState") = Value
        End Set
    End Property

    Private Property ShowMessaggioAggiornamentoAnagrafeCentrale() As Boolean
        Get
            If ViewState("OnVac_ShowMessaggioAggiornamentoAnagrafeCentrale") Is Nothing Then
                ViewState("OnVac_ShowMessaggioAggiornamentoAnagrafeCentrale") = True
            End If
            Return ViewState("OnVac_ShowMessaggioAggiornamentoAnagrafeCentrale")
        End Get
        Set(Value As Boolean)
            ViewState("OnVac_ShowMessaggioAggiornamentoAnagrafeCentrale") = Value
        End Set
    End Property

    Private Property IsEditScadenzaRipristino() As Boolean
        Get
            If ViewState("IsEditScadenzaRipristino") Is Nothing Then ViewState("IsEditScadenzaRipristino") = False
            Return ViewState("IsEditScadenzaRipristino")
        End Get
        Set(value As Boolean)
            ViewState("IsEditScadenzaRipristino") = value
        End Set
    End Property

    ''' <summary>
    ''' Questo flag vale True quando viene inserita/modificata una reazione avversa, vale False altrimenti.
    ''' Al salvataggio dei dati, il flag viene riportato a False.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property IsEditReazioneAvversa() As Boolean
        Get
            If ViewState("IsEditReazioneAvversa") Is Nothing Then ViewState("IsEditReazioneAvversa") = False
            Return ViewState("IsEditReazioneAvversa")
        End Get
        Set(value As Boolean)
            ViewState("IsEditReazioneAvversa") = value
        End Set
    End Property

    Private Property ListCondizioniSanitarie As List(Of KeyValuePair(Of String, String))
        Get
            If ViewState("CS") Is Nothing Then ViewState("CS") = New List(Of KeyValuePair(Of String, String))()
            Return DirectCast(ViewState("CS"), List(Of KeyValuePair(Of String, String)))
        End Get
        Set(Value As List(Of KeyValuePair(Of String, String)))
            ViewState("CS") = Value
        End Set
    End Property

    Private Property ListCondizioniRischio As List(Of KeyValuePair(Of String, String))
        Get
            If ViewState("CR") Is Nothing Then ViewState("CR") = New List(Of KeyValuePair(Of String, String))()
            Return DirectCast(ViewState("CR"), List(Of KeyValuePair(Of String, String)))
        End Get
        Set(Value As List(Of KeyValuePair(Of String, String)))
            ViewState("CR") = Value
        End Set
    End Property

    Private Property ListTipiPagamento As List(Of KeyValuePair(Of String, String))
        Get
            If ViewState("TipiPagamento") Is Nothing Then ViewState("TipiPagamento") = New List(Of KeyValuePair(Of String, String))()
            Return ViewState("TipiPagamento")
        End Get
        Set(Value As List(Of KeyValuePair(Of String, String)))
            ViewState("TipiPagamento") = Value
        End Set
    End Property

    Private Property ListLuoghi As List(Of Entities.LuoghiEsecuzioneVaccinazioni)
        Get
            If ViewState("Luoghi") Is Nothing Then ViewState("TipiPagamento") = New List(Of Entities.LuoghiEsecuzioneVaccinazioni)()
            Return ViewState("Luoghi")
        End Get
        Set(Value As List(Of Entities.LuoghiEsecuzioneVaccinazioni))
            ViewState("Luoghi") = Value
        End Set
    End Property

#Region " Images "

    Private ReadOnly Property UrlArrowUp As String
        Get
            Return Me.ResolveClientUrl("~/images/arrow_up_small.gif")
        End Get
    End Property

    Private ReadOnly Property UrlArrowDown As String
        Get
            Return Me.ResolveClientUrl("~/images/arrow_down_small.gif")
        End Get
    End Property

    Private ReadOnly Property UrlArrowClear As String
        Get
            Return Me.ResolveClientUrl("~/images/transparent16.gif")
        End Get
    End Property

    Private ReadOnly Property UrlReazioneDettaglio As String
        Get
            Return Me.ResolveClientUrl("~/images/reazioneDettaglio.png")
        End Get
    End Property

    Private ReadOnly Property UrlReazioneInsert As String
        Get
            Return Me.ResolveClientUrl("~/images/reazioneInsert.png")
        End Get
    End Property

#End Region

#End Region

#Region " From base "

    Protected Sub OnPageLoad(ByRef layoutTitolo As Label, ByRef onitLayout31 As Onit.Controls.PagesLayout.OnitLayout3, reloadPage As Boolean)

        If IsGestioneCentrale Then

            OpenLeftFrame(True)

            'If Not reloadPage Then LayoutTitolo_sezione.Text += " [CENTRALE]"

            ToolBar.FindItemByValue("btn_Salva").Visible = False
            ToolBar.FindItemByValue("btn_Annulla").Visible = False
            ToolBar.FindItemByText("Scadenza").Visible = False
            ToolBar.FindItemByValue("btnModificaVaccinazioni").Visible = False
            ToolBar.FindItemByValue("btnRecuperaStoricoVacc").Visible = False
            ToolBar.FindItemByValue("btnStampaCertificatoVacc").Visible = True
			ToolBar.FindItemByText("Consenso").Visible = False
			ToolBar.FindItemByValue("btnStampaCertificatoVaccGior").Visible = False

		Else

            ' Visibilità pulsanti Recupera Storico, Certificato Vaccinale e Consenso
            ToolBar.FindItemByValue("btnRecuperaStoricoVacc").Visible = False
            ToolBar.FindItemByValue("btnStampaCertificatoVacc").Visible = False
			ToolBar.FindItemByText("Consenso").Visible = True
			' gestire con parametro
			If Settings.REPORT_VAC_GIORN Then
				ToolBar.FindItemByValue("btnStampaCertificatoVaccGior").Visible = True
			Else
				ToolBar.FindItemByValue("btnStampaCertificatoVaccGior").Visible = False
			End If


		End If

        'Inizializzazione variabili
        objDatiDaReplicare = New VaccinazioneEseguita()
        objDatiDaReplicarePrimaDiModifica = New VaccinazioneEseguita()

        ord = New Hashtable()
        datiLottiEliminati = New List(Of DatiLottoEliminato)()

        nMod = 0
        IsEditScadenzaRipristino = False

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            If Not reloadPage Then OnVacUtility.ImpostaIntestazioniPagina(onitLayout31, layoutTitolo, genericProvider, Settings, IsGestioneCentrale)

            Using biz As New BizVaccinazioniEseguite(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                dt_vacEseguite = biz.GetDtVaccinazioniEseguite(OnVacUtility.Variabili.PazId)

            End Using

        End Using

        LoadCondizioniSanitarieERischio()

        LoadListTipiPagamento()
        LoadListLuoghi()

    End Sub

#End Region

#Region " Gestione Luoghi Esecuzione Vaccinazioni "

    ' Restituisce la descrizione del luogo in base al codice. E' tutto impostato nel parametro LUOGHI, la gestione è nelle OnVacUtility.
    Public Function RisolviLuogo(codiceLuogo As String) As String
        Dim result As LuoghiEsecuzioneVaccinazioni = ListLuoghi.Where(Function(f) f.Codice = codiceLuogo).FirstOrDefault()
        If Not result Is Nothing Then
            Return result.Descrizione
        End If
        Return String.Empty

    End Function

    ' Legge codice e descrizione dei luoghi dal parametro LUOGHI e li inserisce nel combo. Tutto gestito da OnVacUtility.
    Protected Sub CaricaLuoghiEsecuzioneVaccinazioni(ByRef cmb As DropDownList, codiceLuogo As String)

        'Precedente versione, usa un parametro con una lista di luoghi
        'Dim collectionLuoghi As Collection.LuoghiEsecuzioneVacCollection = OnVacUtility.GetLuoghiEsecuzioneVaccinazioni(Me.Settings)

        Dim collectionLuoghi As List(Of Entities.LuoghiEsecuzioneVaccinazioni) = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using biz As New Biz.BizLuoghiEsecuzioneVaccinazioni(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                collectionLuoghi = biz.GetLuoghiEsecuzioneVaccinazioni()

            End Using
        End Using

        'Valorizzo quelli non obsoleti e quello selezionato
        For i As Integer = 0 To collectionLuoghi.Count - 1
            If collectionLuoghi(i).Obsoleto = "N" Then
                Dim listItem As ListItem = New ListItem(collectionLuoghi(i).Descrizione, collectionLuoghi(i).Codice)
                If Not cmb.Items.Contains(listItem) Then cmb.Items.Add(listItem)
            ElseIf collectionLuoghi(i).Codice = codiceLuogo Then
                Dim listItem As ListItem = New ListItem(collectionLuoghi(i).Descrizione, collectionLuoghi(i).Codice)
                If Not cmb.Items.Contains(listItem) Then cmb.Items.Add(listItem)
            End If
        Next

        If Not String.IsNullOrWhiteSpace(codiceLuogo) Then
            cmb.SelectedValue = codiceLuogo
        End If

    End Sub

    Private Sub LoadListLuoghi()
        Dim result As New List(Of Entities.LuoghiEsecuzioneVaccinazioni)
        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using biz As New BizLuoghiEsecuzioneVaccinazioni(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                Dim luoghi As List(Of Entities.LuoghiEsecuzioneVaccinazioni) = biz.GetLuoghiEsecuzioneVaccinazioni()
                result.Add(New Entities.LuoghiEsecuzioneVaccinazioni())
                result.AddRange(luoghi)
                ListLuoghi = result
            End Using
        End Using
    End Sub

#End Region

#Region " Tipo Erogatore "
    Public Function GetDescrizioneTipoErogatore(codice As String) As String

        If Not String.IsNullOrWhiteSpace(codice) Then

            Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                Using biz As New Biz.BizErogatori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                    Dim result As TipoErogatoreVacc = biz.GetDettaglioTipoErogatoreFromCodice(codice)
                    If Not result Is Nothing Then
                        Return result.Descrizione
                    End If
                End Using
            End Using
        End If
        Return ""

    End Function

    Protected Sub CaricaTipoErogatoriVaccinazioni(ByRef cmb As DropDownList, codiceTipoErogatore As String)

        'Precedente versione, usa un parametro con una lista di luoghi
        'Dim collectionLuoghi As Collection.LuoghiEsecuzioneVacCollection = OnVacUtility.GetLuoghiEsecuzioneVaccinazioni(Me.Settings)

        Dim collectionTipiErogatore As List(Of Entities.TipoErogatoreVacc) = Nothing

        collectionTipiErogatore = LoadTipiErogatore()

        'Valorizzo quelli non obsoleti e quello selezionato
        For i As Integer = 0 To collectionTipiErogatore.Count - 1
            If collectionTipiErogatore(i).Obsoleto = "N" Then
                Dim listItem As ListItem = New ListItem(collectionTipiErogatore(i).Descrizione, collectionTipiErogatore(i).Codice)
                If Not cmb.Items.Contains(listItem) Then cmb.Items.Add(listItem)
            ElseIf collectionTipiErogatore(i).Codice = codiceTipoErogatore Then
                Dim listItem As ListItem = New ListItem(collectionTipiErogatore(i).Descrizione, collectionTipiErogatore(i).Codice)
                If Not cmb.Items.Contains(listItem) Then cmb.Items.Add(listItem)
            End If
        Next

        If Not String.IsNullOrWhiteSpace(codiceTipoErogatore) Then
            cmb.SelectedValue = codiceTipoErogatore
        End If

    End Sub

    Private Function LoadTipiErogatore() As List(Of Entities.TipoErogatoreVacc)

        Dim result As New List(Of Entities.TipoErogatoreVacc)

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using biz As New Biz.BizErogatori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                result = biz.GetTipiErogatori()

            End Using
        End Using

        Return result

    End Function

#End Region

#Region " Strutture "

    Public Function GetDescrizioneStruttura(codice As String) As String
        If Not String.IsNullOrWhiteSpace(codice) Then
            Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                Using biz As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                    Dim strutture As List(Of Struttura) = biz.GetStrutture(codice)
                    If Not strutture Is Nothing AndAlso strutture.Count > 0 Then
                        Return strutture.FirstOrDefault().Descrizione
                    End If
                End Using
            End Using
        End If
        Return String.Empty
    End Function
#End Region

#Region " AVN Tipo Pagamento "

    Public Function RisolviTipoPagamento(guidTipoPagamento As Byte()) As String

        Dim filter As String = New Guid(guidTipoPagamento).ToString()
        Dim result As String = String.Empty
        result = ListTipiPagamento.Where(Function(f) f.Key = filter).Select(Function(x) x.Value)(0)

        If Not String.IsNullOrWhiteSpace(result) Then
            Return result
        End If
        Return String.Empty
    End Function

    Private Sub LoadListTipiPagamento()
        Dim result As New List(Of KeyValuePair(Of String, String))
        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using biz As New BizNomiCommerciali(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                Dim tipiPagamento As List(Of Entities.TipiPagamento) = biz.GetListTipiPagamento()
                result.Add(New KeyValuePair(Of String, String)(New Guid().ToString(), ""))
                For Each value As Entities.TipiPagamento In tipiPagamento
                    result.Add(New KeyValuePair(Of String, String)(value.GuidPagamento.ToString(), value.Descrizione))
                Next
                ListTipiPagamento = result
            End Using
        End Using
    End Sub

    Private Sub CaricaTipiPagamento(cmb As OnitCombo)
        cmb.DataSource = ListTipiPagamento
        cmb.DataBind()
    End Sub

#End Region

#Region " Page events "

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If Not IsPostBack() Then

            OnPageLoad(LayoutTitolo1, OnitLayout31, False)
            LayoutTitolo2.Text = LayoutTitolo1.Text
            RegisterDataGridStructure()

            If Not IsGestioneCentrale Then

                ' Controllo se è gestito lo storico centralizzato
                ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
                If FlagConsensoVaccUslCorrente Then

                    Dim statoAcquisizioneDatiVaccinaliCentrale As Enumerators.StatoAcquisizioneDatiVaccinaliCentrale? =
                       Common.OnVacStoricoVaccinaleCentralizzato.GetStatoAcquisizioneDatiVaccinaliCentralePaziente(OnVacUtility.Variabili.PazId)

                    If Not statoAcquisizioneDatiVaccinaliCentrale.HasValue OrElse
                       statoAcquisizioneDatiVaccinaliCentrale = Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Parziale Then

                        strJS &= Common.OnVacStoricoVaccinaleCentralizzato.AlertMessageRecuperoStoricoVaccinale

                        SelectedPageState = StatoPagina.DatiVaccinaliDaRecuperare

                    End If

                End If

            End If

            ImpostaLayoutDatagridEseguite()

            SetPrimaryKeyDtEseguite()

            If SelectedPageState <> StatoPagina.DatiVaccinaliDaRecuperare Then
                SelectedPageState = StatoPagina.VacEseguiteListView
            End If

            ChangeModeViewDdg(ModeViewDgr.NoDettaglio)

            ModeViewDgrStatus = ModeViewDgr.NoDettaglio

        End If

        If Not IsGestioneCentrale Then

            If SelectedPageState <> StatoPagina.DatiVaccinaliDaRecuperare Then

                Select Case Request.Form("__EVENTTARGET")

                    Case "SistemaLotti"

                        If Settings.GESMAG Then

                            ' Inserimento movimenti di carico per ripristinare le dosi 
                            RipristinoLottiEliminati()

                        End If

                        datiLottiEliminati.Clear()

                        SelectedPageState = StatoPagina.VacEseguiteListView

                    Case "NoSistemaLotti"

                        If Settings.GESMAG Then

                            ' Log se l'utente decide di non effettuare il carico
                            LogAnnullamentoRipristinoLotti()

                        End If

                        datiLottiEliminati.Clear()

                        SelectedPageState = StatoPagina.VacEseguiteListView

                End Select

            End If

        End If

        ReloadSection()

    End Sub

    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        RegisterStartupScriptCustom("scr", strJS)

    End Sub

#End Region

#Region " Eventi toolbar "

    Protected Sub ToolBar_ButtonClick(sender As Object, e As Telerik.Web.UI.RadToolBarEventArgs) Handles ToolBar.ButtonClick

        Select Case e.Item.Value

            Case "btn_Salva"

                Salva()

            Case "btn_Annulla"

                Annulla()

            Case "btnChangeView"

                DataBindDatagrid(OrdVacEseguite_Campo + " " + OrdVacEseguite_Verso)

                Select Case ModeViewDgrStatus

                    Case ModeViewDgr.Dettaglio

                        ChangeModeViewDdg(ModeViewDgr.NoDettaglio)
                        ModeViewDgrStatus = ModeViewDgr.NoDettaglio

                    Case ModeViewDgr.NoDettaglio

                        ChangeModeViewDdg(ModeViewDgr.Dettaglio)
                        ModeViewDgrStatus = ModeViewDgr.Dettaglio

                End Select

            Case "btnScaduta"

                CheckScadenzaRipristinoVaccinazioni(False)

            Case "btnRipristinaScaduta"

                CheckScadenzaRipristinoVaccinazioni(True)

            Case "btnConcediConsenso"

                SetVisibilitaDatiVaccinali(Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente)

            Case "btnRevocaConsenso"

                SetVisibilitaDatiVaccinali(Constants.ValoriVisibilitaDatiVaccinali.NegatoDaPaziente)

            Case "btnModificaVaccinazioni"

                OpenModaleElencoVaccinazioniEseguite()

            Case "btnRecuperaStoricoVacc"

                RecuperaStoricoVaccinale()

            Case "btnStampaCertificatoVacc"

				StampaCertificatoVaccinale()

			Case "btnStampaCertificatoVaccGior"
				StampaCertificatoVaccinaleGiornaliero()

		End Select

    End Sub

    Private Sub ToolBarDetail_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBarDetail.ButtonClicked

        Select Case e.Button.Key

            Case "btn_Indietro"

                ToVac()
                nMod -= 1
                If nMod = 0 And Not Me.IsEditScadenzaRipristino Then SetToolbarStatus(False)

            Case "btn_Conferma"

                ConfermaReazione()

        End Select

    End Sub

#End Region

#Region " Scadenza / Ripristino associazioni "

    ' Restituisce true se almeno una delle eseguite selezionate è una vaccinazione fittizia
    Private Function EsisteSelezionataFittizia() As Boolean

        Dim chk As CheckBox = Nothing
        Dim rowAssociazioneSelezionata As DataRow = Nothing

        For i As Int16 = 0 To dgrAssEseguite.Items.Count - 1

            chk = DirectCast(dgrAssEseguite.Items(i).FindControl("cb"), CheckBox)

            If chk.Checked Then

                rowAssociazioneSelezionata = dtAssociazioni.Rows.Find(FindRowKey(i))

                If rowAssociazioneSelezionata("ves_flag_fittizia").ToString() = "S" Then
                    Return True
                End If

            End If

        Next

        Return False

    End Function

    ' ------------------------------------ '
    ' [Unificazione Ulss]: Eliminato controllo
    ' ------------------------------------ '
    ' Restituisce true se almeno una delle eseguite selezionate ha la usl che ha effettuato la scadenza diversa dalla usl corrente
    ' N.B. : Questo controllo deve essere effettuato solo se FlagAbilitazioneVaccUslCorrente vale True, altrimenti la funzione restituisce True 
    '        anche nel caso in cui non si gestisca lo storico vaccinale centralizzato (perchè, in questo caso, la usl di scadenza viene lasciata vuota)
    'Private Function EsisteScadutaAltraUsl() As Boolean

    '    Dim chk As CheckBox = Nothing
    '    Dim rowAssociazioneSelezionata As DataRow = Nothing

    '    For i As Int16 = 0 To Me.dgrAssEseguite.Items.Count - 1

    '        chk = DirectCast(Me.dgrAssEseguite.Items(i).FindControl("cb"), CheckBox)

    '        If chk.Checked Then

    '            rowAssociazioneSelezionata = dtAssociazioni.Rows.Find(FindRowKey(i))

    '            If Not rowAssociazioneSelezionata Is Nothing AndAlso Not rowAssociazioneSelezionata("ves_usl_scadenza") Is DBNull.Value Then

    '                If rowAssociazioneSelezionata("scaduta").ToString() = "S" AndAlso
    '                   OnVacContext.CodiceUslCorrente <> rowAssociazioneSelezionata("ves_usl_scadenza").ToString() Then

    '                    'verifico che rowAssociazioneSelezionata("ves_usl_scadenza") non sia una vecchia usl che fa parte della nuova ulss
    '                    If rowAssociazioneSelezionata("ves_usl_scadenza").ToString() <> "" Then
    '                        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
    '                            Using bizUsl As New Biz.BizUsl(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

    '                                If Not bizUsl.IsInUslUnificata(rowAssociazioneSelezionata("ves_usl_scadenza").ToString(), OnVacContext.CodiceUslCorrente) Then

    '                                    Return True

    '                                End If

    '                            End Using
    '                        End Using
    '                    End If

    '                End If

    '            End If

    '        End If

    '    Next

    '    Return False

    'End Function
    ' ------------------------------------ '

    ' Le eseguite selezionate vengono fatte scadere o ripristinate in base al parametro:
    Private Sub CheckScadenzaRipristinoVaccinazioni(eseguiRipristino As Boolean)

        ' Controllo vaccinazione fittizia tra le selezionate
        If EsisteSelezionataFittizia() Then

            strJS &= "alert('E\' stata selezionata una associazione fittizia, impossibile proseguire');"
            Return

        End If

        ' ------------------------------------ '
        ' [Unificazione Ulss]: Eliminato controllo usl inserimento
        ' ------------------------------------ '
        '' Controlli in caso di gestione storico vaccinale centralizzato
        'If FlagAbilitazioneVaccUslCorrente Then

        '    If eseguiRipristino Then

        '        ' Controllo che tutte le eseguite selezionate siano state fatte scadere dalla usl corrente
        '        If Me.EsisteScadutaAltraUsl() Then

        '            strJS &= "alert('Almeno una delle associazioni selezionate per il ripristino e\' stata fatta scadere da un\'azienda diversa da quella corrente.\nImpossibile proseguire con l\'operazione.');"
        '            Return

        '        End If

        '    End If

        '    ShowAlertAggiornamentoStoricoCentralizzato()

        'End If
        ' ------------------------------------ '

        ' Scadenza/Ripristino delle eseguite selezionate
        SetScadenzaRipristinoVaccinazioni(eseguiRipristino)

        IsEditScadenzaRipristino = True

        ' Aggiornamento dati nella maschera
        DataBindDatagrid(OrdVacEseguite_Campo + " " + OrdVacEseguite_Verso)

        ' Reimposto la visualizzazione delle colonne
        ChangeModeViewDdg(ModeViewDgrStatus)

        ' Reimposto i pulsanti della toolbar
        SetToolbarStatus(True)

    End Sub

    '''<summary>eseguiRipristino = false => flagStato = "N"</summary>
    Private Sub SetScadenzaRipristinoVaccinazioni(eseguiRipristino As Boolean)

        Dim flagStato As String = IIf(eseguiRipristino, "S", "N")

        '------------------------------------------------------------------------------------------
        '-- controllo che non ci siano vaccinazione con dosi inferiori non scadute o da far scadere
        '------------------------------------------------------------------------------------------
        Dim chk As CheckBox = Nothing
        Dim rowAssociazioneSelezionata As DataRow = Nothing

        For i As Int16 = 0 To Me.dgrAssEseguite.Items.Count - 1

            chk = DirectCast(Me.dgrAssEseguite.Items(i).FindControl("cb"), CheckBox)

            If chk.Checked Then

                rowAssociazioneSelezionata = dtAssociazioni.Rows.Find(FindRowKey(i))

                If Not rowAssociazioneSelezionata Is Nothing AndAlso rowAssociazioneSelezionata("scaduta").ToString() = flagStato Then

                    ' Per l'associazione selezionata prendo le vaccinazioni contenute
                    Dim dv1 As New DataView(Me.dt_vacEseguite)

                    If rowAssociazioneSelezionata("ves_ass_codice") = String.Empty Then

                        dv1.RowFilter = "ves_ass_codice is null and ves_ass_n_dose is null and ves_data_effettuazione=" +
                                        FormatForDataView(rowAssociazioneSelezionata("ves_data_effettuazione"))
                    Else

                        dv1.RowFilter = "ves_ass_codice = '" + rowAssociazioneSelezionata("ves_ass_codice").ToString() +
                                        "' and ves_ass_n_dose = '" + rowAssociazioneSelezionata("ves_ass_n_dose").ToString() +
                                        "' and ves_data_effettuazione=" + FormatForDataView(rowAssociazioneSelezionata("ves_data_effettuazione"))
                    End If

                    ' Per ogni vaccinazione verifico che non ci sia una associazione non scaduta contenente
                    ' una vaccinazione con dose inferiore
                    Dim iterator1 As IEnumerator = dv1.GetEnumerator()

                    While iterator1.MoveNext()

                        Dim drv1 As System.Data.DataRowView = CType(iterator1.Current, System.Data.DataRowView)

                        Dim vacInAssCodice As String = drv1("ves_vac_codice")
                        Dim vacInAssDescrizione As String = drv1("vac_descrizione")
                        Dim vacInAssRichiamo As String = drv1("ves_n_richiamo")

                        For j As Int16 = 0 To Me.dgrAssEseguite.Items.Count - 1

                            If j <> i Then

                                Dim chk2 As CheckBox = DirectCast(Me.dgrAssEseguite.Items(j).FindControl("cb"), CheckBox)

                                If Not chk2.Checked Then

                                    Dim assNotToScad As DataRow = dtAssociazioni.Rows.Find(FindRowKey(j))

                                    If Not assNotToScad Is Nothing AndAlso assNotToScad("scaduta").ToString() = "N" Then

                                        ' Per l'associazione selezionata prendo le vaccinazioni contenute
                                        Dim dv2 As New DataView(Me.dt_vacEseguite)

                                        If assNotToScad("ves_ass_codice") = String.Empty Then

                                            dv2.RowFilter = "ves_ass_codice is null and ves_ass_n_dose is null and ves_data_effettuazione=" +
                                                            FormatForDataView(assNotToScad("ves_data_effettuazione"))
                                        Else

                                            dv2.RowFilter = "ves_ass_codice = '" + assNotToScad("ves_ass_codice").ToString() +
                                                            "' and ves_ass_n_dose = '" + assNotToScad("ves_ass_n_dose").ToString() +
                                                            "' and ves_data_effettuazione=" + FormatForDataView(assNotToScad("ves_data_effettuazione"))
                                        End If

                                        ' Per ogni vaccinazione verifico che non ci sia una associazione non scaduta contenente
                                        ' una vaccinazione con dose inferiore
                                        Dim iterator2 As IEnumerator = dv2.GetEnumerator()

                                        While iterator2.MoveNext()

                                            Dim drv2 As System.Data.DataRowView = CType(iterator2.Current, System.Data.DataRowView)

                                            If flagStato = "N" Then

                                                ' Controlli in scadenza
                                                If drv2("ves_vac_codice") = vacInAssCodice AndAlso
                                                   drv2("ves_n_richiamo") > vacInAssRichiamo Then

                                                    Me.OnitLayout31.ShowMsgBox(
                                                        New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(
                                                            "Impossibile eseguire l'operazione.\nTutte le dosi successive alla " & vacInAssRichiamo &
                                                            " dose della vaccinazione " & vacInAssDescrizione &
                                                            " devono essere scadute !!", "NoScad", False, False))
                                                    Return

                                                End If

                                            Else

                                                ' Controlli in ripristino
                                                If drv2("ves_vac_codice") = vacInAssCodice AndAlso
                                                   drv2("ves_n_richiamo") = vacInAssRichiamo Then

                                                    Me.OnitLayout31.ShowMsgBox(
                                                        New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(
                                                            "Impossibile eseguire l'operazione.\nEsiste già una dose " & vacInAssRichiamo &
                                                            " della vaccinazione " & vacInAssDescrizione &
                                                            " non scaduta!!", "NoScad", False, False))
                                                    Return

                                                End If

                                                If drv2("ves_vac_codice") = vacInAssCodice AndAlso
                                                   drv2("ves_n_richiamo") = vacInAssRichiamo Then

                                                    Me.OnitLayout31.ShowMsgBox(
                                                        New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(
                                                            "Impossibile eseguire l'operazione.\nTutte le dosi precedenti alla " & vacInAssRichiamo &
                                                            " dose della vaccinazione " & vacInAssDescrizione &
                                                            " devono essere non scadute !!", "NoScad", False, False))
                                                    Return

                                                End If

                                            End If

                                        End While

                                    End If

                                End If

                            End If

                        Next

                    End While

                End If

            End If

        Next

        '--------------------------------------------
        '-- faccio scadere le dosi delle vaccinazioni
        '--------------------------------------------
        Dim now As DateTime = Date.Today

        For i As Int16 = 0 To Me.dgrAssEseguite.Items.Count - 1

            chk = DirectCast(Me.dgrAssEseguite.Items(i).FindControl("cb"), CheckBox)

            If chk.Checked Then

                rowAssociazioneSelezionata = dtAssociazioni.Rows.Find(FindRowKey(i))

                If Not rowAssociazioneSelezionata Is Nothing Then

                    If rowAssociazioneSelezionata("scaduta").ToString() = flagStato Then

                        rowAssociazioneSelezionata("scaduta") = IIf(eseguiRipristino, "N", "S")

                    End If

                    ' Replico i dati di scadenza/ripristino in tutte le vac dell'associazione
                    ImpostaScadenzaVacStessaAss(rowAssociazioneSelezionata, eseguiRipristino)

                    ' Devo trovare la riga nel dtAssociazioni e impostare scaduta a S
                    For idxAss As Integer = 0 To dtAssociazioni.Rows.Count - 1

                        If dtAssociazioni.Rows(idxAss)("ves_ass_codice").ToString() = rowAssociazioneSelezionata("ves_ass_codice").ToString() AndAlso
                           dtAssociazioni.Rows(idxAss)("ves_ass_n_dose").ToString() = rowAssociazioneSelezionata("ves_ass_n_dose").ToString() AndAlso
                           dtAssociazioni.Rows(idxAss)("ves_data_effettuazione").ToString() = rowAssociazioneSelezionata("ves_data_effettuazione").ToString() Then

                            If eseguiRipristino Then

                                ' Impostazione dati ripristino esecuzione vaccinazioni
                                dtAssociazioni.Rows(idxAss)("scaduta") = "N"

                                dtAssociazioni.Rows(idxAss)("ves_data_ripristino") = now
                                dtAssociazioni.Rows(idxAss)("ves_ute_id_ripristino") = OnVacContext.UserId

                            Else

                                ' Impostazione dati scadenza esecuzione vaccinazioni e cancellazione dati ripristino
                                dtAssociazioni.Rows(idxAss)("scaduta") = "S"

                                dtAssociazioni.Rows(idxAss)("ves_usl_scadenza") = OnVacContext.CodiceUslCorrente
                                dtAssociazioni.Rows(idxAss)("ves_data_scadenza") = now
                                dtAssociazioni.Rows(idxAss)("ves_ute_id_scadenza") = OnVacContext.UserId

                                dtAssociazioni.Rows(idxAss)("ves_data_ripristino") = DBNull.Value
                                dtAssociazioni.Rows(idxAss)("ves_ute_id_ripristino") = DBNull.Value

                            End If

                        End If

                    Next

                End If

            End If

        Next

    End Sub

    ' Replico i dati di scadenza o di ripristino a tutte le vaccinazioni della stessa associazione
    ' Il flag scadenzaEseguite indica il tipo di operazione:
    '   scadenzaEseguite = true => scadenza
    '   scadenzaEseguite = false => ripristino
    Private Sub ImpostaScadenzaVacStessaAss(drow As DataRow, eseguiRipristino As Boolean)

        Dim now As DateTime = Date.Today

        ' Modifica replicata su tutte le vaccinazioni relative all'associazione selezionata
        For idxVac As Integer = 0 To dt_vacEseguite.Rows.Count - 1

            If dt_vacEseguite.Rows(idxVac).RowState <> DataRowState.Deleted Then

                If dt_vacEseguite.Rows(idxVac)("ves_ass_codice").ToString() = drow("ves_ass_codice").ToString() AndAlso
                   dt_vacEseguite.Rows(idxVac)("ves_ass_n_dose").ToString() = drow("ves_ass_n_dose").ToString() AndAlso
                   dt_vacEseguite.Rows(idxVac)("ves_data_effettuazione").ToString() = drow("ves_data_effettuazione").ToString() Then

                    If eseguiRipristino Then

                        ' Flag scadenza impostato a "N"
                        dt_vacEseguite.Rows(idxVac)("scaduta") = "N"

                        ' Impostazione operatore e data ripristino (le info sulla scadenza vengono mantenute)
                        dt_vacEseguite.Rows(idxVac)("ves_ute_id_ripristino") = OnVacContext.UserId
                        dt_vacEseguite.Rows(idxVac)("ves_data_ripristino") = now

                    Else

                        dt_vacEseguite.Rows(idxVac)("scaduta") = "S"

                        dt_vacEseguite.Rows(idxVac)("ves_usl_scadenza") = OnVacContext.CodiceUslCorrente
                        dt_vacEseguite.Rows(idxVac)("ves_ute_id_scadenza") = OnVacContext.UserId
                        dt_vacEseguite.Rows(idxVac)("ves_data_scadenza") = now

                        ' Sbianco le informazioni relative ad un eventuale ripristino precedente
                        dt_vacEseguite.Rows(idxVac)("ves_ute_id_ripristino") = DBNull.Value
                        dt_vacEseguite.Rows(idxVac)("ves_data_ripristino") = DBNull.Value

                    End If

                End If

            End If

        Next

    End Sub

    ' Scrittura log per tenere traccia dell'annullamento da parte dell'utente dell'inserimento di un movimento di carico per ripristinare
    ' le dosi dei lotti delle vaccinazioni eseguite che sono state eliminate.
    Private Sub LogAnnullamentoRipristinoLotti()

        Dim listLotti As List(Of DatiLottoEliminato) = datiLottiEliminati.Where(Function(p) Not p.IsRegistrato).ToList()

        If Not listLotti Is Nothing AndAlso listLotti.Count > 0 Then

            For Each datiLotto As Entities.DatiLottoEliminato In listLotti

                BizLotti.WriteLogAnnullamentoRipristinoCaricoLotto(datiLotto.CodiceLotto,
                                                                   OnVacUtility.Variabili.CNSMagazzino.Codice,
                                                                   datiLotto.DataEsecuzione,
                                                                   datiLotto.NumeroDosiDaRipristinare,
                                                                   datiLotto.IdAssociazione)
            Next
        End If

    End Sub

    ' Caricamento lotti in magazzino, per ripristinare il numero di dosi in caso di eliminazione 
    ' di una vaccinazione eseguita e solo se l'utente ha confermato il ripristino delle giacenze.
    ' Viene caricata 1 dose per ogni terna lotto-data esecuzione-id associazione.
    Private Sub RipristinoLottiEliminati()


        ' Filtro solo i lotti effettivamente eseguiti e non inseriti da RegistrazioneVac
        Dim listLotti As List(Of DatiLottoEliminato) = datiLottiEliminati.Where(Function(p) Not p.IsRegistrato).ToList()
        If Not listLotti Is Nothing AndAlso listLotti.Count > 0 Then

            Using dam As IDAM = OnVacUtility.OpenDam()

                Try
                    dam.BeginTrans()

                    Dim dataRegistrazione As DateTime = Date.Now
                    Dim listTestateLog As New List(Of DataLogStructure.Testata)()

                    ' Inserimento di un movimento di carico per ripristinare ogni lotto + log
                    For Each datiLotto As Entities.DatiLottoEliminato In listLotti

                        ' Oggetto lock che verrà creato (uno per ogni lotto da ripristinare), e che verrà chiuso al termine di ogni salvataggio.
                        Dim lockLotto As Onit.Shared.Manager.Lock.Lock = Nothing

                        Try
                            ' Lock del lotto utilizzato
                            lockLotto = BizLotti.EnterLockLotto(datiLotto.CodiceLotto, OnVacContext.AppId, datiLotto.CnsCodiceMagazzino)

                            Using genericProvider As New DAL.DbGenericProvider(dam)
                                Using bizLotti As New Biz.BizLotti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                                    Dim movimentoLotto As New Entities.MovimentoLotto() With
                                    {
                                        .CodiceLotto = datiLotto.CodiceLotto,
                                        .CodiceConsultorio = datiLotto.CnsCodiceMagazzino,
                                        .NumeroDosi = datiLotto.NumeroDosiDaRipristinare,
                                        .TipoMovimento = Constants.TipoMovimentoMagazzino.Carico,
                                        .IdUtente = OnVacContext.UserId,
                                        .DataRegistrazione = dataRegistrazione,
                                        .IdEsecuzioneAssociazione = datiLotto.IdAssociazione,
                                        .Note = Biz.BizLotti.ImpostaNoteMovimentoVaccinazione(OnVacUtility.Variabili.PazId.ToString(),
                                                                                              datiLotto.DataEsecuzione,
                                                                                              datiLotto.Vaccinazioni,
                                                                                              Constants.TipoMovimentoMagazzino.Carico)
                                    }

                                    bizLotti.CaricaLottoVaccinazione(movimentoLotto,
                                                                     datiLotto.CnsCodiceEsecuzione,
                                                                     datiLotto.CnsCodiceMagazzino,
                                                                     False,
                                                                     listTestateLog)

                                End Using
                            End Using

                        Finally
                            Biz.BizLotti.ExitLockLotto(lockLotto)
                        End Try

                    Next

                    dam.Commit()

                    ' Scrittura log
                    For Each testata As DataLogStructure.Testata In listTestateLog
                        LogBox.WriteData(testata)
                    Next

                Catch ex As Exception

                    If dam.ExistTra Then dam.Rollback()

                    ex.InternalPreserveStackTrace()
                    Throw

                End Try

            End Using
        End If

    End Sub

#End Region

#Region " Consenti / Nega Visibilità dati vaccinali "

    Private Sub SetVisibilitaDatiVaccinali(flagVisibilita As String)

        Dim listRowsAssociazioniSelezionate As New List(Of DataRow)()
        Dim listRowsAssociazioniScaduteSelezionate As New List(Of DataRow)()

        Dim countAssociazioniScartate As Integer = 0

        Dim chk As CheckBox = Nothing
        Dim rowAssociazioneSelezionata As DataRow = Nothing

        For i As Int16 = 0 To dgrAssEseguite.Items.Count - 1

            chk = DirectCast(dgrAssEseguite.Items(i).FindControl("cb"), CheckBox)

            If chk.Checked Then

                rowAssociazioneSelezionata = dtAssociazioni.Rows.Find(FindRowKey(i))

                If Not rowAssociazioneSelezionata Is Nothing Then

                    ' ------------------------------------ '
                    ' [Unificazione Ulss]: Eliminato controllo usl inserimento
                    ' ------------------------------------ '
                    '' Controllo che le associazioni selezionate siano state eseguite dalla usl corrente (se è gestito lo storico centralizzato)
                    'If FlagAbilitazioneVaccUslCorrente AndAlso
                    '   Not rowAssociazioneSelezionata("ves_usl_inserimento") Is DBNull.Value AndAlso
                    '   OnVacContext.CodiceUslCorrente <> rowAssociazioneSelezionata("ves_usl_inserimento").ToString() Then

                    '    Me.strJS &= "alert('Almeno una delle associazioni selezionate e\' stata inserita da un\'azienda diversa da quella corrente.\nImpossibile proseguire con l\'operazione.');"
                    '    Return

                    'End If
                    ' ------------------------------------ '

                    ' Scarto le associazioni che hanno già la visibilità uguale a quella da impostare
                    If rowAssociazioneSelezionata("ves_flag_visibilita").ToString() = flagVisibilita Then
                        countAssociazioniScartate += 1
                    Else

                        If rowAssociazioneSelezionata("scaduta").ToString() = "S" Then
                            listRowsAssociazioniScaduteSelezionate.Add(rowAssociazioneSelezionata)
                        Else
                            listRowsAssociazioniSelezionate.Add(rowAssociazioneSelezionata)
                        End If

                    End If

                End If

            End If

        Next

        If listRowsAssociazioniSelezionate.Count + listRowsAssociazioniScaduteSelezionate.Count = 0 Then

            Dim messageVaccinazioniScartate As New System.Text.StringBuilder()

            If countAssociazioniScartate > 0 Then

                If countAssociazioniScartate = 1 Then
                    messageVaccinazioniScartate.Append(" (è stata scartata 1 associazione ")
                Else
                    messageVaccinazioniScartate.AppendFormat(" (sono state scartate {0} associazioni ", countAssociazioniScartate)
                End If

                messageVaccinazioniScartate.Append("perchè la visibilità relativa è già impostata correttamente)")

            End If

            strJS &= (String.Format("alert('Operazione non effettuata: nessuna associazione selezionata{0}.');", messageVaccinazioniScartate.ToString()))
            Return

        End If

        ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
        If FlagAbilitazioneVaccUslCorrente Then

            ShowAlertAggiornamentoStoricoCentralizzato()

        End If

        ' Creazione lista vaccinazioni a cui modificare il flag di visibilità, in base alle associazioni selezionate
        Dim listIdVaccinazioniEseguite As List(Of Int64) = GetListIdVaccinazioni(listRowsAssociazioniSelezionate)
        Dim listIdVaccinazioniScadute As List(Of Int64) = GetListIdVaccinazioni(listRowsAssociazioniScaduteSelezionate)

        ' ------------------------------------ '
        ' [Unificazione Ulss]: Sostituito aggiornamento visibilità nei db locali delle varie ULSS con update nel db unico
        ' ------------------------------------ '
        'Using bizPaziente As New BizPaziente(Settings, Nothing, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(TipiArgomento.PAZIENTI))

        '    bizPaziente.AggiornaVisibilitaDatiVaccinaliCentrali(OnVacUtility.Variabili.PazId,
        '                                                        listIdVaccinazioniEseguite.ToArray(),
        '                                                        listIdVaccinazioniScadute.ToArray(),
        '                                                        Nothing,
        '                                                        Nothing,
        '                                                        flagVisibilita)

        'End Using

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizVacEseguite As New BizVaccinazioniEseguite(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(TipiArgomento.PAZIENTI))

                bizVacEseguite.UpdateFlagVisibilita(OnVacUtility.Variabili.PazId, listIdVaccinazioniEseguite, listIdVaccinazioniScadute, flagVisibilita)

            End Using
        End Using

        SelectedPageState = StatoPagina.VacEseguiteListView

        ' Ricarica la pagina
        OnPageLoad(LayoutTitolo1, OnitLayout31, True)
        LayoutTitolo2.Text = LayoutTitolo1.Text

        RegisterDataGridStructure()

        ' Rebind del datagrid (ordinato di default)
        DataBindDatagrid(OrdVacEseguite_Campo + " " + OrdVacEseguite_Verso)

        SetPrimaryKeyDtEseguite()

        StrJSNascondiReaz()

        SetToolbarStatus(False)

    End Sub

    Private Function GetListIdVaccinazioni(listRowsAssociazioni) As List(Of Int64)

        Dim listIdVaccinazioni As New List(Of Int64)()

        For Each row As DataRow In listRowsAssociazioni

            For idxVac As Integer = 0 To Me.dt_vacEseguite.Rows.Count - 1

                If Me.dt_vacEseguite.Rows(idxVac).RowState <> DataRowState.Deleted Then

                    If Me.dt_vacEseguite.Rows(idxVac)("ves_ass_codice").ToString() = row("ves_ass_codice").ToString() AndAlso
                       Me.dt_vacEseguite.Rows(idxVac)("ves_ass_n_dose").ToString() = row("ves_ass_n_dose").ToString() AndAlso
                       Me.dt_vacEseguite.Rows(idxVac)("ves_data_effettuazione").ToString() = row("ves_data_effettuazione").ToString() Then

                        listIdVaccinazioni.Add(Me.dt_vacEseguite.Rows(idxVac)("ves_id"))

                    End If

                End If

            Next

        Next

        Return listIdVaccinazioni

    End Function

#End Region

#Region " Eventi OnitLayout "

    Private Sub OnitLayout31_ConfirmClick(sender As Object, eventArgs As Onit.Controls.PagesLayout.OnitLayout3.ConfirmEventArgs) Handles OnitLayout31.ConfirmClick

        If eventArgs.Key = WARNING_REAZIONE AndAlso eventArgs.Result Then

            Dim rowKey As Object() = FindRowKey(Me.IndexDrgVacEsToDelete)

            If rowKey Is Nothing Then Return

            DeleteAssEseguita(dtAssociazioni.Rows.Find(rowKey))

        End If

    End Sub

#End Region

#Region " Datagrid e intestazione "

    Protected Function GetImageHeader(commandArgument As String) As String

        If Me.OrdVacEseguite_Campo = commandArgument Then
            If Me.OrdVacEseguite_Verso.ToLower() = "asc" Then
                Return Me.UrlArrowUp
            Else
                Return Me.UrlArrowDown
            End If
        Else
            Return Me.UrlArrowClear
        End If

    End Function

    Private Sub ImpostaLayoutDatagridEseguite()

        ' --------- datagrid gerarchico --------- '
        ' Creo il datatable delle associazioni, che deve essere quello di livello 0 per il datagrid
        ' gerarchico. Il datatable di livello 1 è quello di dettaglio, con le vaccinazioni e le dosi
        ' relative all'associazione e alla data.

        ' Ordinamento iniziale in base al parametro impostato su db
        Dim paramSort As String = Me.Settings.LASTORDVACESEGUITE
        Dim sortField As String = String.Empty
        Dim orderField As String = String.Empty

        If paramSort.Replace(";", "").Trim() = String.Empty Then

            sortField = "ves_data_effettuazione"
            orderField = "DESC"
            paramSort = sortField + " " + orderField

        Else

            ' Formato di paramSort (da db): "campo1 verso1;campo2 verso2; ... "
            ' Il datagrid è ordinato per tutti i campi specificati. La freccia è impostata solo
            ' per quanto riguarda il primo.
            Dim s1 As String = paramSort.Split(";")(0) ' split della stringa per leggere tutti i campi

            If s1.Length > 0 Then

                ' Split del primo campo per avere il campo e il verso della freccia
                Dim s2() As String = s1.Split(" ")
                sortField = s2(0).Trim()

                If s2.Length > 1 Then
                    orderField = s2(1).Trim()
                Else
                    orderField = "ASC"
                End If

            Else

                ' Se non è stato specificato nessun campo, prendo per default la data
                sortField = "ves_data_effettuazione"
                orderField = "DESC"

            End If

            ' paramSort deve essere usata per ordinare il datagrid
            paramSort = paramSort.Replace(";", ",").Trim()
            If paramSort.EndsWith(",") Then paramSort = paramSort.Substring(0, paramSort.Length - 1)

        End If

        Me.DataBindDatagrid(paramSort)

        ' Imposto le property
        Me.OrdVacEseguite_Campo = sortField
        Me.OrdVacEseguite_Verso = orderField

    End Sub

    Private Sub ClearOrderDatagridHeader()

        ' Cancello il valore delle properties
        Me.OrdVacEseguite_Campo = String.Empty
        Me.OrdVacEseguite_Verso = String.Empty

    End Sub

    Private Sub ChangeModeViewDdg(mode As ModeViewDgr)

        ' Contiene la SortExpression di ogni colonna da visualizzare solo nella vista completa del datagrid.
        ' N.B. : usare tutte colonne in minuscolo.
        Dim colonneVistaCompleta As New System.Collections.Specialized.StringCollection()

        colonneVistaCompleta.Add("ass_codice")
        colonneVistaCompleta.Add("ves_lot_codice")
        colonneVistaCompleta.Add("ope_nome")
        colonneVistaCompleta.Add("ves_ope_in_ambulatorio")
        colonneVistaCompleta.Add("ves_luogo")
        colonneVistaCompleta.Add("vii_descrizione")
        colonneVistaCompleta.Add("sii_descrizione")
        colonneVistaCompleta.Add("ute_descrizione")
        colonneVistaCompleta.Add("com_descrizione")
        colonneVistaCompleta.Add("ves_esito")
        colonneVistaCompleta.Add("ves_note")
        colonneVistaCompleta.Add("ves_in_campagna")
        colonneVistaCompleta.Add("ves_cnv_data_primo_app")
        colonneVistaCompleta.Add("ves_cnv_data")
        colonneVistaCompleta.Add("mal_descrizione")
        colonneVistaCompleta.Add("ves_codice_esenzione")
        colonneVistaCompleta.Add("ves_importo")
        colonneVistaCompleta.Add("ves_lot_data_scadenza")
        colonneVistaCompleta.Add("ves_tipo_erogatore")
        colonneVistaCompleta.Add("ves_codice_struttura")
        colonneVistaCompleta.Add("ves_tpa_guid_tipi_pagamento")

        Select Case mode

            Case ModeViewDgr.Dettaglio

                dgrAssEseguite.Width = New Unit(ScaleFactor * 100, UnitType.Percentage)

                SetDetailColumnsVisibility(colonneVistaCompleta, True)
                ToolBar.FindItemByValue("btnChangeView").Text = "Vista Ridotta"

            Case ModeViewDgr.NoDettaglio

                dgrAssEseguite.Width = New Unit(100, UnitType.Percentage)

                SetDetailColumnsVisibility(colonneVistaCompleta, False)
                ToolBar.FindItemByValue("btnChangeView").Text = "Vista Completa"

        End Select

    End Sub

    Private Sub SetDetailColumnsVisibility(colonneVistaCompleta As System.Collections.Specialized.StringCollection, visible As Boolean)

        For i As Integer = 0 To dgrAssEseguite.Columns.Count - 1

            Dim column As DataGridColumn = dgrAssEseguite.Columns(i)

            If colonneVistaCompleta.Contains(column.SortExpression.ToLower()) Then

                column.Visible = visible

            End If

        Next

    End Sub

    Private Sub RegisterDataGridStructure()

        dgf = New DataGridFarm()

        For i As Integer = 0 To Me.dgrAssEseguite.Columns.Count - 1

            Dim column As DataGridColumn = Me.dgrAssEseguite.Columns(i)

            If column.ItemStyle.Width.Type = UnitType.Percentage Then
                dgf.ExtendedWidth.Add(column.ItemStyle.Width.Value * ScaleFactor & "%")
                dgf.OriginalWidth.Add(column.ItemStyle.Width.Value.ToString() & "%")
            Else
                dgf.ExtendedWidth.Add(column.ItemStyle.Width.Value.ToString())
                dgf.OriginalWidth.Add(column.ItemStyle.Width.Value.ToString())
            End If

        Next

    End Sub

    ' Databind dei datagrid della maschera
    Private Sub DataBindDatagrid(strSort As String)

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            Using biz As New Biz.BizVaccinazioniEseguite(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                dtAssociazioni = biz.CreaDtAssociazioni(Me.dt_vacEseguite)

            End Using

            If IsGestioneCentrale Then
                dtAssociazioni.PrimaryKey = New DataColumn() {dtAssociazioni.Columns("ves_id"), dtAssociazioni.Columns("ves_usl_inserimento")}
            Else
                dtAssociazioni.PrimaryKey = New DataColumn() {dtAssociazioni.Columns("ves_id")}
            End If

        End Using

        ' --------- Datagrid esterno --------- '

        Dim dv As New DataView(dtAssociazioni)
        dv.Sort = strSort.Trim()
        If Not chkFilterAntiInfluenzali.Checked Then dv.RowFilter = " ass_anti_influenzale = 'N' "
        dgrAssEseguite.DataSource = dv
        dgrAssEseguite.DataBind()

        SetColumnVisibility("usl_inserimento_ves_descr", True)
        SetColumnVisibility("ves_flag_visibilita", True)

        Dim showEditButtons As Boolean = True

        If IsGestioneCentrale Then

            ' Pulsanti di edit righe datagrid -> sempre nascosti
            showEditButtons = False

            ' Pulsante di stampa del modulo delle reazioni avverse -> sempre nascosto
            SetColumnVisibility("BtnStampaReazione", False)

        Else

            ' Pulsanti di edit righe datagrid -> nascosti se i dati vaccinali sono da recuperare
            showEditButtons = (SelectedPageState <> StatoPagina.DatiVaccinaliDaRecuperare)

        End If

        ' Nasconde i pulsanti del datagrid
        SetColumnVisibility("CheckboxSelection", showEditButtons)
        SetColumnVisibility("BtnElimina", showEditButtons)
        SetColumnVisibility("BtnEdit", showEditButtons)

        ' ----------- '
        ' N.B. : modificato RowFilter nelle viste di dettaglio per filtrare i dati in maniera più precisa
        BindDatagridDettaglio(dv)

        ' ----------- '
        '' Dettaglio
        'Dim dgDettaglioEseguite As DataGrid
        'Dim dvDettaglioEseguite As DataView

        ''   Per ogni riga del datagrid faccio il bind dei 2 datagrid interni
        'For i As Integer = 0 To dgrAssEseguite.Items.Count - 1

        '    ' Dettaglio
        '    dgDettaglioEseguite = DirectCast(Me.dgrAssEseguite.Items(i).FindControl("dgrDettaglio"), DataGrid)
        '    dvDettaglioEseguite = New DataView(Me.dt_vacEseguite)

        '    If dv(i)("ves_ass_codice") = String.Empty Then
        '        dvDettaglioEseguite.RowFilter = String.Format("ves_ass_codice Is null And ves_ass_n_dose Is null And ves_data_effettuazione = {0}",
        '                                                        FormatForDataView(dv(i)("ves_data_effettuazione")))
        '    Else
        '        dvDettaglioEseguite.RowFilter = String.Format("ves_ass_codice='{0}' and ves_ass_n_dose = '{1}' and ves_data_effettuazione = {2}",
        '                                                       dv(i)("ves_ass_codice").ToString(),
        '                                                       dv(i)("ves_ass_n_dose").ToString(),
        '                                                       FormatForDataView(dv(i)("ves_data_effettuazione")))
        '    End If

        '    dgDettaglioEseguite.DataSource = dvDettaglioEseguite
        '    dgDettaglioEseguite.DataBind()

        'Next
        ' ----------- '

        Dim stbJs As New System.Text.StringBuilder()
        stbJs.AppendFormat("<script language='javascript'>{0}", vbCrLf)
        stbJs.AppendFormat("showDgrDetail({0});{1}", IIf(hiddenStatusDetailVac.Value = "1", "true", "false"), vbCrLf)
        stbJs.AppendFormat("</script>{0}", vbCrLf)

        Page.ClientScript.RegisterStartupScript(Me.GetType(), "statusDetailVac", stbJs.ToString())

        ' Nasconde l'immagine di stampa delle reazioni avverse se il report non è incluso nell'installazione corrente
        Dim btnStampa As ImageButton

        If Not ExistsRptReazAvv() Then

            For i As Integer = 0 To Me.dgrAssEseguite.Items.Count - 1
                btnStampa = DirectCast(Me.dgrAssEseguite.Items(i).FindControl("imgStampa"), ImageButton)
                If Not btnStampa Is Nothing Then btnStampa.Visible = False
            Next

        End If

    End Sub

    Private Sub BindDatagridDettaglio(dv As DataView)

        ' Aggiungo le descrizioni di condizione sanitaria e condizione rischio al datatable, per effettuare il bind nel dettaglio
        Dim dtDettaglio As DataTable = dt_vacEseguite.Copy()
        dtDettaglio.Columns.Add("descrizione_condizione_sanitaria")
        dtDettaglio.Columns.Add("descrizione_condizione_rischio")

        For Each row As DataRow In dtDettaglio.Rows

            If row.RowState <> DataRowState.Deleted Then

                Dim codiceMalattia As String = row("ves_mal_codice_cond_sanitaria").ToString()
                row("descrizione_condizione_sanitaria") = ListCondizioniSanitarie.Where(Function(p) p.Key = codiceMalattia).Select(Function(q) q.Value).FirstOrDefault()

                Dim codiceRischio As String = row("ves_rsc_codice").ToString()
                row("descrizione_condizione_rischio") = ListCondizioniRischio.Where(Function(p) p.Key = codiceRischio).Select(Function(q) q.Value).FirstOrDefault()

            End If

        Next

        dtDettaglio.AcceptChanges()

        ' Per ogni riga del datagrid esterno eseguo il bind del datagrid interno
        For i As Integer = 0 To dgrAssEseguite.Items.Count - 1

            ' Datagrid di dettaglio della riga corrente
            Dim dgDettaglioEseguite As DataGrid = DirectCast(dgrAssEseguite.Items(i).FindControl("dgrDettaglio"), DataGrid)

            Dim dvDettaglioEseguite As New DataView(dtDettaglio)
            dvDettaglioEseguite.RowFilter = BizVaccinazioniEseguite.GetRowFilterAssociazione(dv(i))

            dgDettaglioEseguite.DataSource = dvDettaglioEseguite
            dgDettaglioEseguite.DataBind()

        Next

    End Sub

    Private Sub LoadCondizioniSanitarieERischio()

        ListCondizioniSanitarie.Clear()
        ListCondizioniRischio.Clear()

        Dim codiciMalattie As New List(Of String)()
        Dim codiciCategorieRischio As New List(Of String)()

        For Each item As DataRowView In dt_vacEseguite.DefaultView

            If Not item("ves_mal_codice_cond_sanitaria") Is Nothing AndAlso Not item("ves_mal_codice_cond_sanitaria") Is DBNull.Value Then

                Dim codiceMalattia As String = item("ves_mal_codice_cond_sanitaria").ToString()
                If Not String.IsNullOrWhiteSpace(codiceMalattia) Then

                    If Not codiciMalattie.Contains(codiceMalattia) Then
                        codiciMalattie.Add(codiceMalattia)
                    End If

                End If

            End If

            If Not item("ves_rsc_codice") Is Nothing AndAlso Not item("ves_rsc_codice") Is DBNull.Value Then

                Dim codiceRischio As String = item("ves_rsc_codice").ToString()
                If Not String.IsNullOrWhiteSpace(codiceRischio) Then

                    If Not codiciCategorieRischio.Contains(codiceRischio) Then
                        codiciCategorieRischio.Add(codiceRischio)
                    End If

                End If

            End If

        Next

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            If codiciMalattie.Count > 0 Then
                Using bizMalattie As New BizMalattie(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                    ListCondizioniSanitarie = bizMalattie.GetCodiceDescrizioneMalattie(codiciMalattie)
                End Using
            End If

            If codiciCategorieRischio.Count > 0 Then
                Using bizRischio As New BizCategorieRischio(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                    ListCondizioniRischio = bizRischio.GetCodiceDescrizioneCategorieRischio(codiciCategorieRischio)
                End Using
            End If

        End Using

    End Sub

#Region " Eventi intestazione "

    '''<summary> 
    ''' Click nell'header per ordinamento datagrid
    ''' </summary>
    Private Sub HeaderVacEseguite_Click(commandArgument As String)

        ' Imposto campo e verso dell'ordinamento
        If Me.OrdVacEseguite_Campo = commandArgument Then

            If Me.OrdVacEseguite_Verso = Constants.VersoOrdinamento.Crescente Then
                OrdVacEseguite_Verso = Constants.VersoOrdinamento.Decrescente
            Else
                Me.OrdVacEseguite_Verso = Constants.VersoOrdinamento.Crescente
            End If

        Else

            Me.OrdVacEseguite_Campo = commandArgument
            Me.OrdVacEseguite_Verso = Constants.VersoOrdinamento.Crescente
        End If

        ' Aggiorno il datagrid
        DataBindDatagrid(Me.OrdVacEseguite_Campo + " " + Me.OrdVacEseguite_Verso)

        ' Reimposto la visualizzazione delle colonne
        ChangeModeViewDdg(ModeViewDgrStatus)

        StrJSNascondiReaz()

        If Me.SelectedPageState <> StatoPagina.DatiVaccinaliDaRecuperare Then
            Me.SelectedPageState = StatoPagina.VacEseguiteListView
        End If

        SetToolbarStatus()

    End Sub

#End Region

#Region " Eventi datagrid "

    Private Sub dgrAssEseguite_ItemCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgrAssEseguite.ItemCommand

        Select Case e.CommandName

            Case "Stampa"

                ' Ricerca datarow in base alla riga selezionata del datagrid
                Me.RowKey = Me.FindRowKey(e.Item.ItemIndex)
                If Me.RowKey Is Nothing Then Return

                Common.ReazioniAvverseCommon.StampaModuloReazioniAvverse(Me.dtAssociazioni.Rows.Find(Me.RowKey), True, Me.Page, Me.Settings)

            Case "Sort"

                If Not String.IsNullOrWhiteSpace(e.CommandArgument) AndAlso e.CommandSource.GetType() Is GetType(LinkButton) Then
                    HeaderVacEseguite_Click(e.CommandArgument)
                End If

        End Select

    End Sub

    Private Sub dgrAssEseguite_EditCommand(source As Object, e As DataGridCommandEventArgs) Handles dgrAssEseguite.EditCommand

        ' Controlli preventivi
        If AltraRigaInEdit(OperazioniVaccinazioniEseguite.ModificaDati) Then
            Return
        End If

        ' Riga selezionata
        RowKey = FindRowKey(e.Item.ItemIndex)

        If RowKey Is Nothing Then
            dgrAssEseguite.SelectedIndex = -1
            Return
        End If

        Dim rowAssociazioneSelezionata As DataRow = dtAssociazioni.Rows.Find(RowKey)

        ' Controllo associazione fittizia
        If CheckAssociazioneFittizia(rowAssociazioneSelezionata) Then
            Return
        End If

        ' ------------------------------------ '
        ' [Unificazione Ulss]: Eliminato controllo usl inserimento
        ' ------------------------------------ '
        'Dim gestioneStoricoVacc As New Common.OnVacStoricoVaccinaleCentralizzato(Settings)

        '' Controllo che la vaccinazione selezionata sia stata eseguita dalla usl corrente (se è gestito lo storico centralizzato)
        'If FlagAbilitazioneVaccUslCorrente AndAlso Not gestioneStoricoVacc.CheckEseguitaStessaUsl(rowAssociazioneSelezionata) Then

        '    Me.strJS &= Common.OnVacStoricoVaccinaleCentralizzato.AlertMessageModificaEseguitaUslInserimentoNoUslCorrente
        '    Return

        'End If
        ' ------------------------------------ '

        ' Controllo numero giorni trascorsi dalla registrazione (indipendentemente dalla gestione dello storico centralizzato)
        If Not OnVacUtility.CheckGiorniTrascorsiVariazioneDatiVaccinali(rowAssociazioneSelezionata("ves_data_registrazione"), Settings) Then

            strJS &= String.Format("alert('Il numero di giorni trascorsi dalla data di registrazione è superiore al limite massimo impostato ({0}): impossibile effettuare la modifica.');",
                                   Settings.GIORNI_VARIAZIONE_VACCINAZIONE_ESEGUITA.ToString())
            Return

        End If

        ' ------------------------------------ '
        ' [Unificazione Ulss]: Eliminato controllo usl inserimento
        ' ------------------------------------ '
        '' Controllo se esiste già una reazione registrata da un'altra azienda (se è gestito lo storico centralizzato)
        'If FlagAbilitazioneVaccUslCorrente AndAlso Not gestioneStoricoVacc.CheckReazioneAvversaStessaUsl(rowAssociazioneSelezionata) Then

        '    Me.strJS &= Common.OnVacStoricoVaccinaleCentralizzato.AlertMessageModificaEseguitaUslReazioneNoUslCorrente
        '    Return

        'End If
        ' ------------------------------------ '

        ' In caso di gestione dello storico centralizzato, viene visualizzato un messaggio per avvertire l'utente che, 
        ' al momento del salvataggio, verrà aggiornato lo storico centralizzato. Il messaggio viene visualizzato solo se
        ' la usl ha dato l'ok alla visibilità dei dati vaccinali.
        ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
        If FlagAbilitazioneVaccUslCorrente Then

            ShowAlertAggiornamentoStoricoCentralizzato()

        End If

        dgrAssEseguite.EditItemIndex = e.Item.ItemIndex
        DataBindDatagrid(String.Format("{0} {1}", OrdVacEseguite_Campo, OrdVacEseguite_Verso))

        Me.nMod += 1

        StrJSNascondiReaz()

        SelectedPageState = StatoPagina.VacEseguiteListEditItem

        SetToolbarStatus(True)

        objDatiDaReplicarePrimaDiModifica.ves_amb_codice = DirectCast(dgrAssEseguite.Items(dgrAssEseguite.EditItemIndex).FindControl("uscScegliAmb"), SelezioneAmbulatorio).ambCodice
        objDatiDaReplicarePrimaDiModifica.ves_ope_codice = DirectCast(dgrAssEseguite.Items(dgrAssEseguite.EditItemIndex).FindControl("fm_medico_edit"), OnitModalList).Codice
        objDatiDaReplicarePrimaDiModifica.ves_med_vaccinante_codice = DirectCast(dgrAssEseguite.Items(dgrAssEseguite.EditItemIndex).FindControl("fm_medico_vaccinante_edit"), OnitModalList).Codice
        objDatiDaReplicarePrimaDiModifica.ves_luogo = DirectCast(dgrAssEseguite.Items(dgrAssEseguite.EditItemIndex).FindControl("cmbLuogo"), OnitCombo).SelectedValue
        objDatiDaReplicarePrimaDiModifica.ves_tipo_erogatore = DirectCast(dgrAssEseguite.Items(dgrAssEseguite.EditItemIndex).FindControl("cmbTipoErogatore"), OnitCombo).SelectedValue
        objDatiDaReplicarePrimaDiModifica.ves_dataora_effettuazione = GetDataVaccinazione(CType(dgrAssEseguite.Items(dgrAssEseguite.EditItemIndex).FindControl("tb_data_eff_edit"), OnitDatePick).Data,
                                                                                          CType(dgrAssEseguite.Items(dgrAssEseguite.EditItemIndex).FindControl("tb_ora_eff_edit"), TextBox).Text,
                                                                                          Nothing)
        ' Numero dose associazione
        Dim strDose As String = DirectCast(dgrAssEseguite.Items(dgrAssEseguite.EditItemIndex).FindControl("tb_ass_n_dose_edit"), TextBox).Text()
        If IsNumeric(strDose) Then
            objDatiDaReplicarePrimaDiModifica.ves_ass_n_dose = Convert.ToInt32(strDose)
        End If

        ChangeModeViewDdg(ModeViewDgr.Dettaglio)

    End Sub

    Private Sub dgrAssEseguite_UpdateCommand(source As Object, e As DataGridCommandEventArgs) Handles dgrAssEseguite.UpdateCommand

        'salvataggio dei dati replicabili (luogo,consultorio,medico) prima delle modifiche
        If Not Me.dgrAssEseguite Is Nothing AndAlso dgrAssEseguite.Items.Count > 0 Then

            Dim indiceRigaModificata As Int16 = e.Item.ItemIndex

            Dim errLst As New System.Collections.Generic.List(Of String)

            Dim row As DataRow = dtAssociazioni.Rows.Find(Me.RowKey)
            Dim dgrItem As DataGridItem = Me.dgrAssEseguite.Items(indiceRigaModificata)

            ' --- Unbind dei campi modificati sui quali vanno eseguiti i controlli di coerenza --- '
            Dim assCodice As String = DirectCast(dgrItem.FindControl("tb_ass_codice"), Label).Text
            Dim dose As Integer
            Dim strDose As String = DirectCast(dgrItem.FindControl("tb_ass_n_dose_edit"), TextBox).Text

            ' Messaggio errore controlli data effettuazione e dose
            Dim msgDataDose As New System.Text.StringBuilder()

            Dim erroreDataEffettuazione As String = String.Empty

            Dim dpkDataEffettuazione As OnitDatePick = DirectCast(dgrItem.FindControl("tb_data_eff_edit"), OnitDatePick)

            Dim dataEffettuazione As DateTime =
                Me.GetDataVaccinazione(dpkDataEffettuazione.Data, DirectCast(dgrItem.FindControl("tb_ora_eff_edit"), TextBox).Text, erroreDataEffettuazione)

            If Not String.IsNullOrEmpty(erroreDataEffettuazione) Then

                msgDataDose.Append(erroreDataEffettuazione)

            End If

            If dataEffettuazione > DateTime.MinValue Then

                Dim msgDataEffettuazione As String = Me.ControlloDataEffettuazione(dpkDataEffettuazione.Data, row)

                If Not String.IsNullOrEmpty(msgDataEffettuazione) Then

                    msgDataDose.AppendFormat("{0}\n", msgDataEffettuazione)

                End If

            End If

            'Scadenza Lotto
            Dim dpkDataScadenzaLotto As OnitDatePick = DirectCast(dgrItem.FindControl("tb_data_scadenza_lotto_edit"), OnitDatePick)
            Dim dataScadenzaLotto As DateTime = New DateTime(dpkDataScadenzaLotto.Data.Year, dpkDataScadenzaLotto.Data.Month, dpkDataScadenzaLotto.Data.Day)

            ' Controllo dose di associazione
            If Not String.IsNullOrWhiteSpace(assCodice) Then

                If String.IsNullOrWhiteSpace(strDose) Then
                    msgDataDose.Append("La dose di associazione non puo\' essere nulla\n")
                Else
                    If Integer.TryParse(strDose, dose) Then
                        If dose <= 0 Then
                            msgDataDose.Append("La dose di associazione deve essere maggiore di 0\n")
                        End If
                    Else
                        msgDataDose.Append("La dose di associazione deve essere un valore numerico superiore a 0\n")
                    End If
                End If

            End If

            If msgDataDose.Length > 0 Then
                Me.OnitLayout31.ShowMsgBox(New PagesLayout.OnitLayout3.OnitLayoutMsgBox(msgDataDose.ToString(), "DataDose", False, False))
                Return
            End If

            ' Controllo della progressione delle dosi associazione e delle dosi vaccinazione
            If Not String.IsNullOrWhiteSpace(assCodice) Then

                Dim msgCoerenzaAssociazione As String = Me.ControlloCoerenzaAssociazione(assCodice, strDose, dataEffettuazione)

                If Not String.IsNullOrEmpty(msgCoerenzaAssociazione) Then
                    Me.OnitLayout31.ShowMsgBox(New PagesLayout.OnitLayout3.OnitLayoutMsgBox(msgCoerenzaAssociazione, "NoScad", False, False))
                    Return
                End If

            End If

            ' Per l'associazione selezionata prendo le vaccinazioni contenute
            Dim dv1 As New DataView(Me.dt_vacEseguite)
            Dim dataEffettuazioneOld As Date = System.Convert.ToDateTime(DirectCast(dgrItem.FindControl("tb_dataora_eff_edit_label"), Label).Text)

            If assCodice = String.Empty Then
                dv1.RowFilter = "ves_ass_codice is null and ves_ass_n_dose is null and ves_data_effettuazione=" +
                                FormatForDataView(dataEffettuazioneOld)
            Else
                Dim strDoseOld As String = DirectCast(dgrItem.FindControl("hdfAssDose"), HiddenField).Value
                dv1.RowFilter = "ves_ass_codice = '" + assCodice +
                                "' and ves_ass_n_dose = '" + strDoseOld +
                                "' and ves_data_effettuazione=" + FormatForDataView(dataEffettuazioneOld)
            End If

            ' Per ogni vaccinazione contenuta verifico la coerenza con le altre vaccinazioni
            Dim iterator1 As IEnumerator = dv1.GetEnumerator()

            While iterator1.MoveNext()

                Dim drv1 As System.Data.DataRowView = DirectCast(iterator1.Current, System.Data.DataRowView)
                Dim vacInAssCodice As String = drv1("ves_vac_codice")
                Dim vacInAssDescrizione As String = drv1("vac_descrizione")
                Dim vacInAssRichiamo As String = drv1("ves_n_richiamo")
                Dim vacInAssID As String = drv1("ves_id")

                ' Controllo 1: Per ogni vaccinazione verifico che non ci sia una vaccinazione non scaduta con dose inferiore e data successiva
                Dim dv2 As New DataView(Me.dt_vacEseguite)
                dv2.RowFilter = String.Format("ves_vac_codice = '{0}' and ves_n_richiamo < '{1}' and ves_id <> {2} and ves_data_effettuazione > {3} and scaduta = 'N'",
                                              vacInAssCodice,
                                              vacInAssRichiamo,
                                              vacInAssID,
                                              Me.FormatForDataView(dataEffettuazione))

                If dv2.GetEnumerator.MoveNext() Then
                    Dim msg As String = String.Format("La {0} dose della vaccinazione {1} ha dose e data effettuazione incoerenti con le altre vaccinazioni eseguite", vacInAssRichiamo, vacInAssDescrizione)
                    errLst.Add(msg)
                End If

                ' Controllo 2: Per ogni vaccinazione verifico che non ci sia una vaccinazione non scaduta con dose inferiore e data successiva
                dv2 = New DataView(Me.dt_vacEseguite)
                dv2.RowFilter = String.Format("ves_vac_codice = '{0}' and ves_n_richiamo > '{1}' and ves_id <> {2} and ves_data_effettuazione < {3} and scaduta = 'N'",
                                              vacInAssCodice,
                                              vacInAssRichiamo,
                                              vacInAssID,
                                              Me.FormatForDataView(dataEffettuazione))

                If dv2.GetEnumerator.MoveNext() Then
                    Dim msg As String = String.Format("La {0} dose della vaccinazione {1} ha dose e data effettuazione incoerenti con le altre vaccinazioni eseguite", vacInAssRichiamo, vacInAssDescrizione)
                    errLst.Add(msg)
                End If

            End While

            ' Check presenza errore
            If errLst.Count > 0 Then

                Dim msg As String = String.Format("Impossibile eseguire l'operazione.\n{0}", String.Join("\n", errLst.ToArray()))

                Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(msg, "NoScad", False, False))
                Return

            End If

            For i As Integer = 0 To Me.dt_vacEseguite.Rows.Count - 1

                Dim thisrow As DataRow = Me.dt_vacEseguite.Rows(i)

                If thisrow.RowState <> DataRowState.Deleted Then

                    Dim aggancio As Boolean = False

                    If thisrow("ves_ass_codice").ToString() = row("ves_ass_codice", DataRowVersion.Original).ToString() AndAlso
                       thisrow("ves_ass_n_dose").ToString() = row("ves_ass_n_dose", DataRowVersion.Original).ToString() AndAlso
                       thisrow("ves_data_effettuazione").ToString() = row("ves_data_effettuazione", DataRowVersion.Original).ToString() Then

                        aggancio = True

                    End If

                    If aggancio Then

                        Dim uscScegliAmb As SelezioneAmbulatorio = DirectCast(dgrItem.FindControl("uscScegliAmb"), SelezioneAmbulatorio)
                        Dim fmMedicoEdit As OnitModalList = DirectCast(dgrItem.FindControl("fm_medico_edit"), OnitModalList)
                        Dim fmMedicoVaccinanteEdit As OnitModalList = DirectCast(dgrItem.FindControl("fm_medico_vaccinante_edit"), OnitModalList)
                        Dim fmSiiEdit As OnitModalList = DirectCast(dgrItem.FindControl("fm_sii_edit"), OnitModalList)
                        Dim fmViiEdit As OnitModalList = DirectCast(dgrItem.FindControl("fm_vii_edit"), OnitModalList)
                        Dim fmComuneOStato As OnitModalList = DirectCast(dgrItem.FindControl("fmComuneOStato"), OnitModalList)

                        thisrow("ves_ass_n_dose") = dose
                        thisrow("ves_data_effettuazione") = dataEffettuazione.Date
                        thisrow("ves_dataora_effettuazione") = dataEffettuazione
                        thisrow("ves_cns_codice") = uscScegliAmb.cnsCodice
                        thisrow("cns_descrizione") = uscScegliAmb.cnsDescrizione
                        thisrow("ves_amb_codice") = uscScegliAmb.ambCodice
                        thisrow("amb_descrizione") = uscScegliAmb.ambDescrizione
                        thisrow("ope_nome") = fmMedicoEdit.Descrizione
                        thisrow("ope_nome1") = fmMedicoVaccinanteEdit.Descrizione
                        thisrow("ves_ope_codice") = fmMedicoEdit.Codice
                        thisrow("ves_med_vaccinante") = fmMedicoVaccinanteEdit.Codice
                        thisrow("ves_sii_codice") = fmSiiEdit.Codice
                        thisrow("sii_descrizione") = fmSiiEdit.Descrizione
                        thisrow("ves_vii_codice") = fmViiEdit.Codice
                        thisrow("vii_descrizione") = fmViiEdit.Descrizione
                        thisrow("ves_luogo") = DirectCast(dgrItem.FindControl("cmbLuogo"), DropDownList).SelectedItem.Value
                        thisrow("com_descrizione") = fmComuneOStato.Descrizione
                        thisrow("ves_comune_o_stato") = fmComuneOStato.Codice
                        thisrow("ves_ope_in_ambulatorio") = DirectCast(dgrItem.FindControl("ocOpeInAmb"), OnitCombo).SelectedValue
                        If dataScadenzaLotto = Date.MinValue Then
                            thisrow("ves_lot_data_scadenza") = DBNull.Value
                        Else
                            thisrow("ves_lot_data_scadenza") = dataScadenzaLotto
                        End If

                        thisrow("ves_tipo_erogatore") = DirectCast(dgrItem.FindControl("cmbTipoErogatore"), DropDownList).SelectedItem.Value
                        Dim guidValue As Guid = New Guid(DirectCast(dgrItem.FindControl("cmbTipoPagamento"), DropDownList).SelectedItem.Value)
                        Dim byteArrayValue As Byte() = guidValue.ToByteArray()
                        thisrow("ves_tpa_guid_tipi_pagamento") = byteArrayValue

                        ' Note
                        Dim txtNoteVac As TextBox = DirectCast(dgrItem.FindControl("txtNoteVac"), TextBox)

                        If Not txtNoteVac Is Nothing Then

                            If txtNoteVac.Text.Length > txtNoteVac.MaxLength Then
                                txtNoteVac.Text = txtNoteVac.Text.Substring(0, txtNoteVac.MaxLength)
                            End If

                            thisrow("ves_note") = txtNoteVac.Text

                        End If

                    End If

                End If

            Next

        End If

        StrJSNascondiReaz()
        SelectedPageState = StatoPagina.VacEseguiteListView
        SetToolbarStatus()

        dgrAssEseguite.EditItemIndex = -1
        DataBindDatagrid(OrdVacEseguite_Campo + " " + OrdVacEseguite_Verso)
        ChangeModeViewDdg(ModeViewDgrStatus)

    End Sub

    Private Sub dgrAssEseguite_DeleteCommand(source As Object, e As DataGridCommandEventArgs) Handles dgrAssEseguite.DeleteCommand

        ' Controlli preventivi
        If AltraRigaInEdit(OperazioniVaccinazioniEseguite.Cancellazione) Then

            StrJSNascondiReaz()
            SelectedPageState = StatoPagina.VacEseguiteListView
            SetToolbarStatus()

            Return

        End If

        ' Riga selezionata
        RowKey = FindRowKey(e.Item.ItemIndex)
        If RowKey Is Nothing Then Return

        Dim rowAssociazioneSelezionata As DataRow = dtAssociazioni.Rows.Find(RowKey)
        If rowAssociazioneSelezionata Is Nothing Then Return

        ' ------------------------------------ '
        ' [Unificazione Ulss]: Eliminato controllo usl inserimento
        ' ------------------------------------ '
        ' Dim gestioneStoricoVacc As New Common.OnVacStoricoVaccinaleCentralizzato(Me.Settings)

        '' Controllo se la usl corrente è quella che ha eseguito la vaccinazione (solo se è attiva la gestione dello storico centralizzato)
        'If FlagAbilitazioneVaccUslCorrente AndAlso Not gestioneStoricoVacc.CheckEseguitaStessaUsl(rowAssociazioneSelezionata) Then

        '    Me.strJS &= Common.OnVacStoricoVaccinaleCentralizzato.AlertMessageCancellazioneEseguitaUslInserimentoNoUslCorrente
        '    Return

        'End If
        ' ------------------------------------ '

        ' Controllo numero giorni trascorsi dalla registrazione (indipendentemente dalla gestione dello storico centralizzato)
        If Not OnVacUtility.CheckGiorniTrascorsiVariazioneDatiVaccinali(rowAssociazioneSelezionata("ves_data_registrazione"), Settings) Then

            strJS &= String.Format("alert('Il numero di giorni trascorsi dalla data di registrazione è superiore al limite massimo impostato ({0}): impossibile effettuare l\'eliminazione.');",
                                   Settings.GIORNI_VARIAZIONE_VACCINAZIONE_ESEGUITA.ToString())
            Return

        End If

        ' Controllo presenza reazione avversa
        If rowAssociazioneSelezionata("vra_data_reazione") Is DBNull.Value Then

            DeleteAssEseguita(rowAssociazioneSelezionata)

        Else
            ' ------------------------------------ '
            ' [Unificazione Ulss]: Eliminato controllo usl inserimento
            ' ------------------------------------ '
            '' Controllo se reazione avversa registrata da altra usl (solo se è attiva la gestione dello storico centralizzato)
            'If FlagAbilitazioneVaccUslCorrente AndAlso Not gestioneStoricoVacc.CheckReazioneAvversaStessaUsl(rowAssociazioneSelezionata) Then

            '    Me.strJS &= Common.OnVacStoricoVaccinaleCentralizzato.AlertMessageCancellazioneEseguitaUslReazioneNoUslCorrente
            '    Return

            'End If
            ' ------------------------------------ '

            IndexDrgVacEsToDelete = e.Item.ItemIndex
            OnitLayout31.ShowMsgBox(New PagesLayout.OnitLayout3.OnitLayoutMsgBox("La reazione avversa associata verrà eliminata insieme alla associazione. Continuare?", WARNING_REAZIONE, True, True))

        End If

        StrJSNascondiReaz()
        SelectedPageState = StatoPagina.VacEseguiteListView
        SetToolbarStatus()

    End Sub

    Private Sub dgrAssEseguite_CancelCommand(source As Object, e As DataGridCommandEventArgs) Handles dgrAssEseguite.CancelCommand

        dgrAssEseguite.EditItemIndex = -1
        DataBindDatagrid(OrdVacEseguite_Campo + " " + OrdVacEseguite_Verso)

        StrJSNascondiReaz()
        SelectedPageState = StatoPagina.VacEseguiteListView

        Me.nMod -= 1

        If Me.nMod = 0 And Not IsEditScadenzaRipristino Then
            SetToolbarStatus(False)
        Else
            SetToolbarStatus(True)
        End If

        ChangeModeViewDdg(ModeViewDgrStatus)

    End Sub

    ' Inserimento/Modifica/Visualizzazione reazione avversa
    Private Sub dgrAssEseguite_SelectedIndexChanged(sender As Object, e As EventArgs) Handles dgrAssEseguite.SelectedIndexChanged

        ' Modifiche effettuate, serve un salvataggio
        If OnitLayout31.Busy Then

            ToVac()
            strJS &= "alert('I dati devono essere salvati prima di inserire la reazione avversa.');"
            dgrAssEseguite.SelectedIndex = -1

            Return

        End If

        ' Altra riga già in modifica
        If AltraRigaInEdit(OperazioniVaccinazioniEseguite.ReazioneAvversa) Then

            ToVac()
            dgrAssEseguite.SelectedIndex = -1

            Return

        End If

        ' Numero righe selezionate > del massimo previsto
        Dim countRigheSelezionate As Int16 = 0

        For i As Int16 = 0 To dgrAssEseguite.Items.Count - 1
            If DirectCast(dgrAssEseguite.Items(i).FindControl("cb"), CheckBox).Checked Then

                countRigheSelezionate += 1

            End If
        Next

        If countRigheSelezionate > MAX_ESEGUITE_REAZIONE Then

            ToVac()
            strJS &= String.Format("alert('Possono essere selezionate al massimo {0} vaccinazioni eseguite');", MAX_ESEGUITE_REAZIONE.ToString())
            dgrAssEseguite.SelectedIndex = -1

            Return

        End If

        ' Riga su cui l'utente ha cliccato
        RowKey = FindRowKey(dgrAssEseguite.SelectedIndex)

        If RowKey Is Nothing Then
            dgrAssEseguite.SelectedIndex = -1
            Return
        End If

        Dim rowAssociazioneSelezionata As DataRow = dtAssociazioni.Rows.Find(RowKey)

        ' Lettura righe selezionate
        Dim listEseguiteReazioniSelezionate As List(Of VaccinazioneEseguita) = CreateListEseguiteReazioniSelezionate(rowAssociazioneSelezionata)

        ReazAvverseDetail.StatoControlloCorrente = ReazAvverseDetail.StatoControllo.Modifica

        ' In centrale (read-only) si possono solo modificare reazioni, non inserirle.
        If Me.IsGestioneCentrale AndAlso listEseguiteReazioniSelezionate.Any(Function(item) String.IsNullOrWhiteSpace(item.ReazioneAvversa.CodiceReazione)) Then

            strJS &= "alert('Non tutte le eseguite selezionate hanno reazioni avverse associate.');"

            Me.dgrAssEseguite.SelectedIndex = -1
            Return

        End If

        ' Controllo fittizie
        If listEseguiteReazioniSelezionate.Any(Function(p) p.ves_flag_fittizia = "S") Then

            strJS &= "alert('Impossibile modificare una associazione fittizia!');"

            Me.dgrAssEseguite.SelectedIndex = -1
            Return

        End If

        If Me.SelectedPageState = StatoPagina.DatiVaccinaliDaRecuperare Then

            strJS &= "alert('Impossibile inserire o modificare la reazione avversa: non e\' stato effettuato il recupero dei dati vaccinali centralizzati.');"
            ReazAvverseDetail.StatoControlloCorrente = ReazAvverseDetail.StatoControllo.SolaLettura

        Else

            'Dim gestioneStoricoVacc As New Common.OnVacStoricoVaccinaleCentralizzato(Me.Settings)

            If IsGestioneCentrale Then

                ' Maschera centrale => Dettaglio reazione read-only
                ReazAvverseDetail.StatoControlloCorrente = ReazAvverseDetail.StatoControllo.SolaLettura

                ' ------------------------------------ '
                ' [Unificazione Ulss]: Eliminato controllo usl inserimento
                ' ------------------------------------ '

                'ElseIf FlagAbilitazioneVaccUslCorrente AndAlso Not listEseguiteReazioniSelezionate.Any(Function(vac) gestioneStoricoVacc.CheckReazioneAvversaStessaUsl(vac)) Then

                '    ' Gestione storico centralizzato e una o + reazioni selezionate sono state inserite da un'altra azienda => messaggio all'utente e dettaglio reazione read-only
                '    strJS &= "alert('Sono state selezionate reazioni avverse inserite da un\'azienda diversa da quella corrente. Non e\' possibile effettuare modifiche alle reazioni.');"
                '    Me.ReazAvverseDetail.StatoControlloCorrente = ReazAvverseDetail.StatoControllo.SolaLettura
                ' ------------------------------------ '

            Else

                Dim ok As Boolean = True

                ' Controllo date di effettuazione delle eseguite selezionate (tutte in stessa data)
                If listEseguiteReazioniSelezionate.Count > 1 AndAlso
                   listEseguiteReazioniSelezionate.Select(Function(item) item.ves_data_effettuazione).Distinct().Count > 1 Then

                    strJS &= "alert('Impossibile inserire o modificare reazioni avverse se la data di effettuazione non è la stessa per tutte le eseguite selezionate.\nIl modulo di dettaglio della reazione non verra\' visualizzato. ');"
                    ReazAvverseDetail.StatoControlloCorrente = ReazAvverseDetail.StatoControllo.NonAbilitato
                    ok = False

                End If

                ' Controllo via di somministrazione e sito di inoculazione
                If ok AndAlso listEseguiteReazioniSelezionate.Any(
                    Function(item) String.IsNullOrWhiteSpace(item.ves_vii_codice) OrElse String.IsNullOrWhiteSpace(item.ves_sii_codice)) Then

                    strJS &= "alert('Impossibile inserire o modificare reazioni avverse se la via di somministrazione o il sito di inoculazione non sono valorizzati per tutte le eseguite selezionate.\nIl modulo di dettaglio della reazione non verra\' visualizzato. ');"
                    Me.ReazAvverseDetail.StatoControlloCorrente = ReazAvverseDetail.StatoControllo.NonAbilitato
                    ok = False

                End If

                ' In caso di modifica, controllo numero giorni trascorsi dalla registrazione della reazione avversa
                If ok AndAlso listEseguiteReazioniSelezionate.Any(
                        Function(item) Not String.IsNullOrWhiteSpace(item.ReazioneAvversa.CodiceReazione) AndAlso
                        Not OnVacUtility.CheckGiorniTrascorsiVariazioneDatiVaccinali(item.ReazioneAvversa.DataCompilazione, Settings)) Then

                    strJS &= String.Format("alert('Impossibile inserire o modificare le reazioni avverse: il numero di giorni trascorsi dalla registrazione di una della reazioni selezionate è superiore al limite massimo impostato ({0}).');",
                                           Settings.GIORNI_VARIAZIONE_VACCINAZIONE_ESEGUITA.ToString())

                    ReazAvverseDetail.StatoControlloCorrente = ReazAvverseDetail.StatoControllo.SolaLettura

                End If

            End If
        End If

        Select Case ReazAvverseDetail.StatoControlloCorrente

            Case ReazAvverseDetail.StatoControllo.SolaLettura

                ToReazAvv(listEseguiteReazioniSelezionate)

            Case ReazAvverseDetail.StatoControllo.Modifica

                ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
                If FlagAbilitazioneVaccUslCorrente Then
                    ShowAlertAggiornamentoStoricoCentralizzato()
                End If

                ToReazAvv(listEseguiteReazioniSelezionate)

            Case ReazAvverseDetail.StatoControllo.NonAbilitato

                ' Il dettaglio della reazione avversa non viene mostrato

        End Select

        dgrAssEseguite.SelectedIndex = -1

    End Sub

    Private Sub dgrAssEseguite_ItemDataBound(sender As Object, e As DataGridItemEventArgs) Handles dgrAssEseguite.ItemDataBound

        Select Case e.Item.ItemType

            Case ListItemType.EditItem, ListItemType.Item, ListItemType.AlternatingItem, ListItemType.SelectedItem

                ' ImageButton Reazione Avversa
                Dim btnDettaglioReazione As ImageButton = DirectCast(e.Item.FindControl("btnDettaglioReazione"), ImageButton)

                If Not btnDettaglioReazione Is Nothing Then

                    If e.Item.DataItem("vra_rea_codice") Is DBNull.Value Then

                        If Me.IsGestioneCentrale OrElse (FlagAbilitazioneVaccUslCorrente AndAlso Me.SelectedPageState = StatoPagina.DatiVaccinaliDaRecuperare) Then
                            btnDettaglioReazione.Visible = False
                        Else
                            btnDettaglioReazione.ImageUrl = UrlReazioneInsert
                            btnDettaglioReazione.ToolTip = "Inserimento reazione avversa"
                        End If

                    Else

                        btnDettaglioReazione.ImageUrl = UrlReazioneDettaglio
                        btnDettaglioReazione.ToolTip = "Dettaglio reazione avversa"

                    End If

                End If

                ' Se la riga è quella in edit
                If e.Item.ItemType = ListItemType.EditItem Then

                    ' rowKey è già valorizzata
                    If Not RowKey Is Nothing Then

                        Dim row As DataRow = dtAssociazioni.Rows.Find(RowKey)

                        ' Controllo sulla presenza della reazione avversa
                        If Not row("vra_rea_codice") Is DBNull.Value Then

                            strJS &= "alert('Questa associazione ha una reazione avversa associata. Non è possibile modificare la data, il sito di inoculazione e la via di somministrazione!');"

                            SetControlVisibility(Of OnitDatePick)(e.Item, "tb_data_eff_edit", False)
                            SetControlVisibility(Of TextBox)(e.Item, "tb_ora_eff_edit", False)
                            SetControlVisibility(Of Label)(e.Item, "tb_dataora_eff_edit_label", True)

                            SetControlVisibility(Of OnitModalList)(e.Item, "fm_vii_edit", False)
                            SetControlVisibility(Of Label)(e.Item, "tb_vii_edit_label", True)

                            SetControlVisibility(Of OnitModalList)(e.Item, "fm_sii_edit", False)
                            SetControlVisibility(Of Label)(e.Item, "tb_sii_edit_label", True)

                        Else

                            SetControlVisibility(Of OnitDatePick)(e.Item, "tb_data_eff_edit", True)
                            SetControlVisibility(Of TextBox)(e.Item, "tb_ora_eff_edit", True)
                            SetControlVisibility(Of Label)(e.Item, "tb_dataora_eff_edit_label", False)

                            SetControlVisibility(Of OnitModalList)(e.Item, "fm_vii_edit", True)
                            SetControlVisibility(Of Label)(e.Item, "tb_vii_edit_label", False)

                            SetControlVisibility(Of OnitModalList)(e.Item, "fm_sii_edit", True)
                            SetControlVisibility(Of Label)(e.Item, "tb_sii_edit_label", False)

                        End If

                    End If

                    'LUOGO
                    Dim cmb As OnitCombo = e.Item.FindControl("cmbLuogo")
                    If Not cmb Is Nothing Then
                        'Carica gli elementi della combo per i luoghi di esecuzione delle vaccinazioni
                        CaricaLuoghiEsecuzioneVaccinazioni(cmb, e.Item.DataItem("ves_luogo").ToString())
                    End If

                    'TIPO EROGATORE

                    Dim cmbTipoErogatore As OnitCombo = e.Item.FindControl("cmbTipoErogatore")
                    If Not cmb Is Nothing Then
                        CaricaTipoErogatoriVaccinazioni(cmbTipoErogatore, e.Item.DataItem("ves_tipo_erogatore").ToString())
                    End If

                    'TIPO PAGAMENTO
                    Dim cmbTipoPagamento As OnitCombo = e.Item.FindControl("cmbTipoPagamento")
                    If Not cmbTipoPagamento Is Nothing Then
                        CaricaTipiPagamento(cmbTipoPagamento)
                    End If

                    If Not e.Item.DataItem("ves_tpa_guid_tipi_pagamento") Is Nothing AndAlso Not e.Item.DataItem("ves_tpa_guid_tipi_pagamento") Is DBNull.Value Then
                        Dim guidByteArray As Byte() = e.Item.DataItem("ves_tpa_guid_tipi_pagamento")
                        Dim guidValue As String = New Guid(guidByteArray).ToString()
                        'Dim filter As String = New Guid(guidTipoPagamento).ToString()
                        Dim guidString As String = String.Empty
                        guidString = ListTipiPagamento.Where(Function(f) f.Key = guidValue).Select(Function(x) x.Key)(0)
                        cmbTipoPagamento.SelectedValue = guidString
                    End If

                    Dim uscScegliAmb As SelezioneAmbulatorio = e.Item.FindControl("uscScegliAmb")

                    If Not uscScegliAmb Is Nothing Then

                        If Not e.Item.DataItem("ves_cns_codice") Is Nothing AndAlso Not e.Item.DataItem("ves_cns_codice") Is DBNull.Value Then
                            uscScegliAmb.cnsCodice = e.Item.DataItem("ves_cns_codice")
                            uscScegliAmb.cnsDescrizione = e.Item.DataItem("cns_descrizione")
                        End If

                        If Not e.Item.DataItem("ves_amb_codice") Is Nothing AndAlso Not e.Item.DataItem("ves_amb_codice") Is DBNull.Value AndAlso e.Item.DataItem("ves_amb_codice") > 0 Then
                            uscScegliAmb.ambCodice = e.Item.DataItem("ves_amb_codice")
                            uscScegliAmb.ambDescrizione = e.Item.DataItem("amb_descrizione")
                        End If

                        uscScegliAmb.databind()

                    End If

                    ' Abilitazione check visibilità
                    Dim chkFlagVisibilita As CheckBox = e.Item.FindControl("chkFlagVisibilita")

                    If Not chkFlagVisibilita Is Nothing Then chkFlagVisibilita.Enabled = True
                End If

            Case ListItemType.Header
                'disabilitazione header in caso di sort
                For Each cell As TableCell In e.Item.Cells
                    For Each ctl As Control In cell.Controls
                        If (ctl.GetType() Is GetType(LinkButton)) Then
                            DirectCast(ctl, LinkButton).Enabled = dgrAssEseguite.EditItemIndex < 0
                        End If
                    Next
                Next

        End Select

    End Sub

#End Region

#End Region

#Region " Modifica numero dose "

    Private Sub OpenModaleElencoVaccinazioniEseguite()

        ucElencoVaccinazioniEseguite.IsGestioneCentrale = IsGestioneCentrale
        ucElencoVaccinazioniEseguite.ModaleName = "modElencoVaccinazioniEseguite"
        ucElencoVaccinazioniEseguite.LoadModale()

        modElencoVaccinazioniEseguite.VisibileMD = True

    End Sub

    Private Sub ucElencoVaccinazioniEseguite_DosiModificate() Handles ucElencoVaccinazioniEseguite.DosiModificate

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using biz As New BizVaccinazioniEseguite(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                dt_vacEseguite = biz.GetDtVaccinazioniEseguite(OnVacUtility.Variabili.PazId)

            End Using
        End Using

        LoadCondizioniSanitarieERischio()

        DataBindDatagrid(OrdVacEseguite_Campo + " " + OrdVacEseguite_Verso)

    End Sub

#End Region

#Region " Protected methods "

    Protected Function RisolviInAmbulatorio(inAmbulatorio As String) As String

        If inAmbulatorio = "S" Then
            Return "SI"
        End If

        Return "NO"

    End Function

    Protected Function FormattaData(dataDaFormattare As String) As String

        If dataDaFormattare <> String.Empty Then
            Return String.Format("{0:dd/MM/yyyy}", CDate(dataDaFormattare))
        End If

        Return String.Empty

    End Function

    Protected Function FormattaImporto(drvVaccinazioneEseguita As DataRowView) As String

        Dim importo As Double = 0

        If Not drvVaccinazioneEseguita.Row.IsNull("ves_importo") Then
            Try
                importo = Convert.ToDouble(drvVaccinazioneEseguita("ves_importo"))
            Catch ex As Exception
                importo = 0
            End Try
        End If

        Return importo.ToString("C", OnVacUtility.NumberFormatInfo)

    End Function

#End Region

#Region " Private methods "

    ' N.B. : salvataggio possibile solo se la maschera è in modalità locale e non in centrale
    Protected Sub Salva()

        Dim vaccinazioniEseguiteResult As List(Of ReazioneAvversa)

        Using transactionScope As New System.Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, OnVacUtility.GetReadCommittedTransactionOptions())

            Using dbGenericProviderFactory As New DbGenericProviderFactory()

                ' Salvataggio eseguite e valorizzazione lotti eliminati
                Using biz As New BizVaccinazioniEseguite(dbGenericProviderFactory, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(TipiArgomento.VAC_ESEGUITE))

                    Dim command As New BizVaccinazioniEseguite.VaccinazioniEseguiteSalvaCommand()
                    command.CodicePaziente = Convert.ToInt64(OnVacUtility.Variabili.PazId)
                    command.DtEseguite = dt_vacEseguite
                    'se ci sono, vengono eliminate dalle programmate le vaccinazioni eseguite scadute che sono state ripristinate
                    command.DatiVaccinazioniProgrammateDaEliminare = GetDatiVaccinazioniProgrammateDaEliminare()

                    vaccinazioniEseguiteResult = biz.Salva(command, datiLottiEliminati)
                End Using

                ' Caricamento dati paziente
                Dim pazienteCorrente As Paziente = Nothing
                Using bizPaziente As New BizPaziente(dbGenericProviderFactory, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                    pazienteCorrente = bizPaziente.CercaPaziente(OnVacUtility.Variabili.PazId)
                End Using

                ' Aggiornamento dati vaccinali paziente nelle varie usl (se gestito)
                OnVacMidSendManager.ModificaPaziente(pazienteCorrente, dt_vacEseguite)

                ' Caricamento dati eseguite
                Using biz As New BizVaccinazioniEseguite(dbGenericProviderFactory, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(TipiArgomento.VAC_ESEGUITE))
                    dt_vacEseguite = biz.GetDtVaccinazioniEseguite(OnVacUtility.Variabili.PazId)
                End Using

            End Using

            transactionScope.Complete()

        End Using

        LoadCondizioniSanitarieERischio()

        ' Inserisco segnalazione nel sistema d'integrazione vigifarmaco
        Dim messaggio As String = String.Empty

        If Settings.REAZIONE_AVVERSA_INTEGRAZIONE Then

			Dim listaTotaleRea As List(Of ReazioneAvversa) = vaccinazioniEseguiteResult.Where(Function(item) item.FlagInviato = "N").ToList()
			If listaTotaleRea.Count > 0 Then
				messaggio = inserisciIntegrazioneReazione(OnVacUtility.Variabili.PazId, listaTotaleRea)
			End If

        End If

        ' Recupero dati dei lotti eliminati, di cui verrà rispristinato il numero di dosi inserendo un movimento di carico
        If Settings.GESMAG Then

            Dim listLotti As List(Of DatiLottoEliminato) = datiLottiEliminati.Where(Function(p) Not p.IsRegistrato).ToList()
            If Not listLotti.IsNullOrEmpty() Then
                If listLotti.Count > 0 Then
                    strJS &= "if(confirm(""Salvataggio effettuato. Saranno ripristinate le giacenze del magazzino. Continuare?"")){__doPostBack(""SistemaLotti"")}else {__doPostBack(""NoSistemaLotti"")}" & vbCrLf
                End If
            End If

        End If

            If Not messaggio.IsNullOrEmpty Then
            strJS &= String.Format("alert('{0}');", HttpUtility.JavaScriptStringEncode(messaggio)) & vbCrLf
        End If

        nMod = 0
        IsEditScadenzaRipristino = False
        IsEditReazioneAvversa = False

        SelectedPageState = StatoPagina.VacEseguiteListView

        ' Rebind del datagrid (ordinato di default)
        DataBindDatagrid(OrdVacEseguite_Campo + " " + OrdVacEseguite_Verso)

        SetPrimaryKeyDtEseguite()

        StrJSNascondiReaz()

        SetToolbarStatus(False)

    End Sub

    Public Function inserisciIntegrazioneReazione(codicePaziente As Integer, vaccinazioniEseguiteResult As List(Of ReazioneAvversa)) As String

        Dim stringa As String = String.Empty

        Using dbGenericProvider As New DbGenericProviderFactory
            Using biz As New BizIntegrazioneReazioniAvverse(dbGenericProvider, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(TipiArgomento.VAC_ESEGUITE))
				stringa = biz.InserisciReazioneAversaPu(codicePaziente, OnVacUtility.Variabili.CNS.Codice, vaccinazioniEseguiteResult)
			End Using
        End Using

        Return stringa

    End Function

    Private Sub Annulla()

        StrJSNascondiReaz()
        SelectedPageState = StatoPagina.VacEseguiteListView

        dt_vacEseguite.RejectChanges()
        dtAssociazioni.RejectChanges()

        DataBindDatagrid(OrdVacEseguite_Campo + " " + OrdVacEseguite_Verso)

        ChangeModeViewDdg(ModeViewDgrStatus)

        nMod = 0
        IsEditScadenzaRipristino = False
        IsEditReazioneAvversa = False

        SetToolbarStatus(False)

    End Sub

    Private Function ControlloDataEffettuazione(dataEffettuazione As DateTime, row As DataRow) As String

        Dim dataNascitaPaziente As DateTime = DateTime.MinValue

        If Not row("paz_data_nascita") Is Nothing AndAlso Not row("paz_data_nascita") Is DBNull.Value Then

            Try
                dataNascitaPaziente = Convert.ToDateTime(row("paz_data_nascita"))
            Catch ex As Exception
                dataNascitaPaziente = DateTime.MinValue
            End Try

        End If

        ' Caricamento data di nascita del paziente se nulla.
        If dataNascitaPaziente = DateTime.MinValue Then

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                dataNascitaPaziente = genericProvider.Paziente.GetDataNascita(OnVacUtility.Variabili.PazId)

            End Using

        End If

        If dataEffettuazione < dataNascitaPaziente Then

            Return "La data di effettuazione non può essere inferiore alla data di nascita del paziente."

        End If

        Return String.Empty

    End Function

    Private Function ControlloCoerenzaAssociazione(codiceAssociazione As String, doseAssociazione As String, dataOraEffettuazione As Date) As String

        ' Se associazione o dose sono vuoti, non può effettuare nessun controllo
        If String.IsNullOrEmpty(codiceAssociazione) OrElse String.IsNullOrEmpty(doseAssociazione) Then Return String.Empty

        ' Riga corrente per ottenere l'id della vaccinazione
        Dim currentRow As DataRow = Me.dtAssociazioni.Rows.Find(Me.RowKey)
        If currentRow Is Nothing Then Return String.Empty

        Dim idVaccinazione As String = currentRow("ves_id").ToString()

        Dim dv1 As DataView = New DataView(dtAssociazioni)

        ' [Controllo 1] - Dose già presente (per stessa associazione)
        dv1.RowFilter = String.Format("ves_ass_codice = '{0}' and ves_ass_n_dose = {1} and ves_id <> {2} and scaduta = 'N'",
                                      codiceAssociazione,
                                      doseAssociazione,
                                      idVaccinazione)

        If dv1.Count > 0 Then
            Return String.Format("Impossibile eseguire l'operazione.\nLa dose {0} relativa alla associazione {1} esiste già",
                                 doseAssociazione, codiceAssociazione)
        End If

        ' [Controllo 2] - Dose inferiore in data successiva (per stessa associazione, non scaduta)
        dv1.RowFilter = String.Format("ves_ass_codice = '{0}' and ves_ass_n_dose < {1} and ves_id <> {2} and scaduta = 'N'",
                                      codiceAssociazione,
                                      doseAssociazione,
                                      idVaccinazione)
        If dv1.Count > 0 Then

            Dim list As List(Of DateTime) = CreateListDataOraEsecuzioneFromDataView(dv1)

            If list.Count > 0 AndAlso list.Any(Function(p) p > dataOraEffettuazione) Then
                Return String.Format("Impossibile eseguire l'operazione.\nRelativamente all'associazione {0}, esistono dosi inferiori alla {1} con data di registrazione successiva.",
                                     codiceAssociazione, doseAssociazione)
            End If

        End If

        ' [Controllo 3] - Dose superiore in data precedente (per stessa associazione, non scaduta)
        dv1.RowFilter = String.Format("ves_ass_codice = '{0}' and ves_ass_n_dose > {1} and ves_id <> {2} and scaduta = 'N'",
                                      codiceAssociazione,
                                      doseAssociazione,
                                      idVaccinazione)
        If dv1.Count > 0 Then

            Dim list As List(Of DateTime) = CreateListDataOraEsecuzioneFromDataView(dv1)

            If list.Count > 0 AndAlso list.Any(Function(p) p < dataOraEffettuazione) Then
                Return String.Format("Impossibile eseguire l'operazione.\nRelativamente all'associazione {0}, esistono dosi superiori alla {1} con data di registrazione precedente.",
                                     codiceAssociazione, doseAssociazione)
            End If

        End If

        Return String.Empty

    End Function

    Private Function CreateListDataOraEsecuzioneFromDataView(dv As DataView) As List(Of DateTime)

        Dim list As New List(Of DateTime)()

        For Each item As DataRowView In dv
            If Not item("ves_dataora_effettuazione") Is Nothing AndAlso Not item("ves_dataora_effettuazione") Is DBNull.Value Then
                list.Add(DirectCast(item("ves_dataora_effettuazione"), DateTime))
            End If
        Next

        Return list

    End Function

    Private Function GetDataVaccinazione(dataEffettuazione As Date, oreMinuti As String, ByRef errorMessage As String) As Date

        Dim message As New System.Text.StringBuilder()

        ' Controllo data
        If dataEffettuazione = DateTime.MinValue Then
            message.Append("La data di effettuazione non è valorizzata.\n")
        End If

        ' Controllo ora
        Dim oraEffettuazione As DateTime = DateTime.MinValue

        If Not String.IsNullOrEmpty(oreMinuti) Then

            If Not DateTime.TryParse(oreMinuti, oraEffettuazione) Then
                message.Append("Il formato dell'ora di effettuazione non è valido.\n")
            End If

        End If

        errorMessage = message.ToString()

        If dataEffettuazione = DateTime.MinValue Then Return DateTime.MinValue

        Return New DateTime(dataEffettuazione.Year, dataEffettuazione.Month, dataEffettuazione.Day, oraEffettuazione.Hour, oraEffettuazione.Minute, 0)

    End Function

    Private Function GetDataVaccinazione(ByRef item As DataGridItem) As Date

        Dim data As Date = CDate(CType(item.FindControl("tb_ora_eff"), Label).Text)

        Return data

    End Function

    Private Function FindRowKey(index As Integer) As Object()

        If Me.dgrAssEseguite Is Nothing OrElse Me.dgrAssEseguite.Items.Count = 0 Then
            Return Nothing
        End If

        If Me.IsGestioneCentrale Then

            Dim key(1) As Object
            key(0) = Convert.ToInt32(DirectCast(Me.dgrAssEseguite.Items(index).FindControl("tb_ves_id"), HiddenField).Value)
            key(1) = DirectCast(Me.dgrAssEseguite.Items(index).FindControl("hdVesUslInserimento"), HiddenField).Value

            Return key

        End If

        Return New Object() {Convert.ToInt32(DirectCast(Me.dgrAssEseguite.Items(index).FindControl("tb_ves_id"), HiddenField).Value)}

    End Function

    Private Function FormatForDataView(dateValue As DateTime) As String

        Return String.Format(System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat, "#{0}#", dateValue)

    End Function

    Private Sub SetControlVisibility(Of T As Control)(dataGridItem As DataGridItem, controlName As String, visible As Boolean)

        DirectCast(dataGridItem.FindControl(controlName), T).Visible = visible

    End Sub

    ''' <summary>
    ''' Restituisce l'elenco delle vaccinazioni modificate che da SCADUTE sono state RIPRISTINATE (in modo così da restituire la lista delle programmate che andranno eliminate)
    ''' </summary>
    ''' <returns></returns>
    Private Function GetDatiVaccinazioniProgrammateDaEliminare() As List(Of Entities.VaccinazioneProgrammata)

        Dim datiVacProgDaEliminare As New List(Of Entities.VaccinazioneProgrammata)()

        Dim dvEseguiteModificate As New DataView(dt_vacEseguite, String.Empty, String.Empty, DataViewRowState.ModifiedCurrent)

        For Each dataRowView As DataRowView In dvEseguiteModificate

            Dim row As DataRow = dataRowView.Row

            'vaccinazione ripristinata
            If row("scaduta").ToString() = "N" Then

                If row("scaduta", DataRowVersion.Original).ToString().ToUpper().Trim() <> row("scaduta", DataRowVersion.Current).ToString().ToUpper().Trim() Then

                    Dim dataCnv As Date = Date.MinValue
                    If IsDate(row("ves_cnv_data").ToString()) Then dataCnv = Convert.ToDateTime(row("ves_cnv_data").ToString())
                    Dim nrSeduta As Int16?
                    If Not String.IsNullOrWhiteSpace(row("ves_n_seduta").ToString()) Then nrSeduta = Convert.ToInt16(row("ves_n_seduta").ToString())
                    Dim newVacProgDaEliminare As New Entities.VaccinazioneProgrammata With {
                        .CodiceCiclo = row("ves_cic_codice").ToString(),
                        .NumeroSeduta = nrSeduta,
                        .CodiceVaccinazione = row("ves_vac_codice").ToString(),
                        .CodiceAssociazione = row("ves_ass_codice").ToString(),
                        .NumeroRichiamo = row("ves_n_richiamo").ToString(),
                        .CodicePaziente = Convert.ToInt64(row("paz_codice").ToString()),
                        .DataConvocazione = dataCnv
                    }

                    datiVacProgDaEliminare.Add(newVacProgDaEliminare)

                End If

            End If

        Next

        Return datiVacProgDaEliminare

    End Function


#Region " Cancellazione dal datatable "

    Private Sub DeleteAssEseguita(ByRef row As DataRow)

        ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
        If FlagAbilitazioneVaccUslCorrente Then

            ShowAlertAggiornamentoStoricoCentralizzato()

        End If

        ' Devo trovare la riga nel dtAssociazioni e cancellarla
        For i As Integer = 0 To dtAssociazioni.Rows.Count - 1

            If dtAssociazioni.Rows(i).RowState <> DataRowState.Deleted Then

                If dtAssociazioni.Rows(i)("ves_ass_codice").ToString() = row("ves_ass_codice").ToString() AndAlso
                   dtAssociazioni.Rows(i)("ves_ass_n_dose").ToString() = row("ves_ass_n_dose").ToString() AndAlso
                   dtAssociazioni.Rows(i)("ves_data_effettuazione").ToString() = row("ves_data_effettuazione").ToString() Then

                    dtAssociazioni.Rows(i).Delete()
                    Exit For

                End If

            End If

        Next

        ' Devo cancellare tutte le vaccinazioni associate nel dt_vacEseguite
        For i As Integer = dt_vacEseguite.Rows.Count - 1 To 0 Step -1

            If dt_vacEseguite.Rows(i).RowState <> DataRowState.Deleted Then

                If dt_vacEseguite.Rows(i)("ves_ass_codice").ToString() = row("ves_ass_codice", DataRowVersion.Original).ToString() AndAlso
                   dt_vacEseguite.Rows(i)("ves_ass_n_dose").ToString() = row("ves_ass_n_dose", DataRowVersion.Original).ToString() AndAlso
                   dt_vacEseguite.Rows(i)("ves_data_effettuazione").ToString() = row("ves_data_effettuazione", DataRowVersion.Original).ToString() Then

                    dt_vacEseguite.Rows(i).Delete()

                End If

            End If

        Next

        If Not row("ves_lot_codice", DataRowVersion.Original) Is DBNull.Value Then

            CancellaVacStessoLotto(row)

        End If

        Me.nMod += 1

        SetToolbarStatus(True)
        DataBindDatagrid(OrdVacEseguite_Campo + " " + OrdVacEseguite_Verso)

        ' Reimposto la visualizzazione delle colonne
        ChangeModeViewDdg(ModeViewDgrStatus)

    End Sub

    Private Sub CancellaVacStessoLotto(ByRef row As DataRow)

        For i As Int16 = dt_vacEseguite.Rows.Count - 1 To 0 Step -1

            Dim thisRow As DataRow = dt_vacEseguite.Rows(i)

            If thisRow.RowState <> DataRowState.Deleted Then

                If (thisRow("ves_lot_codice").ToString() = row("ves_lot_codice", DataRowVersion.Original).ToString() And
                    thisRow("ves_ass_codice").ToString() = row("ves_ass_codice", DataRowVersion.Original).ToString() And
                    thisRow("ves_ass_n_dose").ToString() = row("ves_ass_n_dose", DataRowVersion.Original).ToString() And
                    thisRow("ves_data_effettuazione").ToString() = row("ves_data_effettuazione", DataRowVersion.Original).ToString()) Then

                    thisRow.Delete()

                End If

            End If
        Next

    End Sub

#End Region

#Region " Layout "

    Private Sub ReloadSection()

        Select Case SelectedPageState

            Case StatoPagina.ReazioniAvverseDetailView

                Me.StrJSNascondiVac()

            Case StatoPagina.VacEseguiteListEdit,
                 StatoPagina.VacEseguiteListEditItem,
                 StatoPagina.VacEseguiteListView,
                 StatoPagina.DatiVaccinaliDaRecuperare

                Me.StrJSNascondiReaz()

            Case Else
                Throw New NotImplementedException(String.Format("Stato pagina non gestito: {0}", SelectedPageState.ToString()))

        End Select

        Me.SetToolbarStatus()

    End Sub

    Private Sub ToVac()

        Me.StrJSNascondiReaz()

        If Me.SelectedPageState <> StatoPagina.DatiVaccinaliDaRecuperare Then
            Me.SelectedPageState = StatoPagina.VacEseguiteListView
        End If

        Me.SetToolbarStatus()

    End Sub

    Private Sub ToReazAvv(listEseguiteReazioni As List(Of Entities.VaccinazioneEseguita))

        If listEseguiteReazioni.IsNullOrEmpty() Then
            Exit Sub
        End If

        Me.StrJSNascondiVac()

        If Me.SelectedPageState <> StatoPagina.DatiVaccinaliDaRecuperare Then
            Me.SelectedPageState = StatoPagina.ReazioniAvverseDetailView
        End If

        Me.SetToolbarStatus(True)
        Me.SetToolbarDetailStatus()

        Me.ReazAvverseDetail.LoadDataIntoControl(listEseguiteReazioni)

        Me.dgrAssEseguite.SelectedIndex = -1

        Me.nMod += 1

    End Sub

    Private Sub StrJSNascondiVac()

        Me.mlvMaster.SetActiveView(Me.ReazioneAvversa)

    End Sub

    Private Sub StrJSNascondiReaz()

        Me.mlvMaster.SetActiveView(Me.Eseguite)

    End Sub

#Region " Toolbars "

    Private Sub SetToolbarStatus()

        Me.SetToolbarStatus(Me.OnitLayout31.Busy)

    End Sub

    Private Sub SetToolbarStatus(busy As Boolean)

        If Me.nMod > 0 Or Me.IsEditScadenzaRipristino Then
            busy = True
        End If

        Dim btnSalva As Telerik.Web.UI.RadToolBarItem = Me.ToolBar.FindItemByValue("btn_Salva")
        Dim btnAnnulla As Telerik.Web.UI.RadToolBarItem = Me.ToolBar.FindItemByValue("btn_Annulla")
        Dim btnChangeView As Telerik.Web.UI.RadToolBarItem = Me.ToolBar.FindItemByValue("btnChangeView")
        Dim btnScadenza As Telerik.Web.UI.RadToolBarDropDown = DirectCast(Me.ToolBar.FindItemByText("Scadenza"), Telerik.Web.UI.RadToolBarDropDown)
        Dim btnShowVacc As Telerik.Web.UI.RadToolBarItem = Me.ToolBar.FindItemByValue("btnShowVacc")
        Dim btnModificaVaccinazioni As Telerik.Web.UI.RadToolBarItem = Me.ToolBar.FindItemByValue("btnModificaVaccinazioni")
        Dim btnRecuperaStoricoVacc As Telerik.Web.UI.RadToolBarItem = Me.ToolBar.FindItemByValue("btnRecuperaStoricoVacc")
        Dim btnStampaCertificatoVacc As Telerik.Web.UI.RadToolBarItem = Me.ToolBar.FindItemByValue("btnStampaCertificatoVacc")
		Dim btnConsenso As Telerik.Web.UI.RadToolBarDropDown = DirectCast(Me.ToolBar.FindItemByText("Consenso"), Telerik.Web.UI.RadToolBarDropDown)
		Dim btnStampaCertificatoVaccGior As Telerik.Web.UI.RadToolBarItem = Me.ToolBar.FindItemByValue("btnStampaCertificatoVaccGior")

		If Me.IsGestioneCentrale Then

            btnSalva.Enabled = False
            btnAnnulla.Enabled = False
            btnChangeView.Enabled = True
            btnScadenza.Enabled = False
            btnShowVacc.Enabled = True
            btnModificaVaccinazioni.Enabled = False
            btnRecuperaStoricoVacc.Enabled = False
            btnStampaCertificatoVacc.Enabled = True
			btnConsenso.Enabled = False
			btnStampaCertificatoVaccGior.Enabled = False

			chkFilterAntiInfluenzali.Enabled = True

        Else

            Select Case SelectedPageState

                Case StatoPagina.VacEseguiteListView

                    btnSalva.Enabled = busy
                    btnAnnulla.Enabled = busy
                    btnChangeView.Enabled = True
                    btnScadenza.Enabled = Not Me.IsEditReazioneAvversa
                    btnShowVacc.Enabled = True
                    btnModificaVaccinazioni.Enabled = Not busy
                    btnRecuperaStoricoVacc.Enabled = False
                    btnStampaCertificatoVacc.Enabled = False
					btnConsenso.Enabled = Not busy
					btnStampaCertificatoVaccGior.Enabled = True

					chkFilterAntiInfluenzali.Enabled = Not busy

                Case StatoPagina.VacEseguiteListEdit, StatoPagina.VacEseguiteListEditItem, StatoPagina.ReazioniAvverseDetailView

                    btnSalva.Enabled = False
                    btnAnnulla.Enabled = False
                    btnChangeView.Enabled = False
                    btnScadenza.Enabled = False
                    btnShowVacc.Enabled = False
                    btnModificaVaccinazioni.Enabled = False
                    btnRecuperaStoricoVacc.Enabled = False
                    btnStampaCertificatoVacc.Enabled = False
					btnConsenso.Enabled = False
					btnStampaCertificatoVaccGior.Enabled = False

					chkFilterAntiInfluenzali.Enabled = False

                Case StatoPagina.DatiVaccinaliDaRecuperare

                    btnSalva.Enabled = False
                    btnAnnulla.Enabled = False
                    btnChangeView.Enabled = True
                    btnScadenza.Enabled = False
                    btnShowVacc.Enabled = True
                    btnModificaVaccinazioni.Enabled = False
                    btnRecuperaStoricoVacc.Enabled = True
                    btnStampaCertificatoVacc.Enabled = False
					btnConsenso.Enabled = False
					btnStampaCertificatoVaccGior.Enabled = False

					chkFilterAntiInfluenzali.Enabled = True

            End Select

        End If

        Me.OnitLayout31.Busy = busy

    End Sub

    Private Sub SetToolbarDetailStatus()

        Me.ToolBarDetail.Items.FromKeyButton("btn_Conferma").Enabled =
            (Me.ReazAvverseDetail.StatoControlloCorrente = ReazAvverseDetail.StatoControllo.Modifica)

    End Sub

#End Region

#End Region

#Region " Controlli pre-variazioni sulle vaccinazioni eseguite "

    Private Function AltraRigaInEdit(operazioneDaEffettuare As OperazioniVaccinazioniEseguite) As Boolean

        If Me.dgrAssEseguite.EditItemIndex <> -1 Then

            Dim operazione As String = String.Empty

            Select Case operazioneDaEffettuare

                Case OperazioniVaccinazioniEseguite.Cancellazione
                    operazione = "cancellare"

                Case OperazioniVaccinazioniEseguite.ModificaDati
                    operazione = "modificare"

                Case OperazioniVaccinazioniEseguite.ReazioneAvversa
                    operazione = "selezionare"

            End Select

            Me.strJS &= String.Format("alert('Cliccare AGGIORNA O ANNULLA della riga che si sta modificando prima di {0} questa riga');",
                                      operazione)

            Return True

        End If

        Return False

    End Function

    ' Restituisce true se l'associazione selezionata è marcata come fittizia
    Private Function CheckAssociazioneFittizia(rowAssociazioneSelezionata As DataRow) As Boolean

        If rowAssociazioneSelezionata("ves_flag_fittizia").ToString() = "S" Then

            strJS &= "alert('Impossibile modificare una associazione fittizia!');"
            Return True

        End If

        Return False

    End Function

#End Region

#Region " Reazioni Avverse "

    Private Function ExistsRptReazAvv() As Boolean

        Dim exists As Boolean = False

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                exists = bizReport.ExistsReport(Constants.ReportName.ReazioniAvverse)

            End Using
        End Using

        Return exists

    End Function

    Private Sub CheckPrecReaz()

        For Each row As DataRow In Me.dt_vacEseguite.Rows

            If Not row.RowState = DataRowState.Deleted Then

                If Not row("vra_rea_codice") Is DBNull.Value Then
                    row("PrecReaz") = True
                End If

            End If

        Next

        Me.dt_vacEseguite.AcceptChanges()

    End Sub

    Private Sub InserisciReazioniAvverseAltreVaccinazioniStessaAssociazione(sourceRow As DataRow)

        Dim copiaDatiReazione As Boolean = False

        For Each currentRow As DataRow In Me.dt_vacEseguite.Rows

            If Not currentRow Is sourceRow AndAlso currentRow.RowState <> DataRowState.Deleted Then

                If Not currentRow("ves_lot_codice") Is DBNull.Value AndAlso
                   currentRow("ves_lot_codice").ToString() = sourceRow("ves_lot_codice").ToString() AndAlso
                   currentRow("ves_ass_codice").ToString() = sourceRow("ves_ass_codice").ToString() AndAlso
                   currentRow("ves_ass_n_dose").ToString() = sourceRow("ves_ass_n_dose").ToString() AndAlso
                   currentRow("ves_data_effettuazione").ToString() = sourceRow("ves_data_effettuazione").ToString() Then

                    copiaDatiReazione = True

                ElseIf Not currentRow("ves_ass_codice") Is DBNull.Value AndAlso
                       currentRow("ves_ass_codice").ToString() = sourceRow("ves_ass_codice").ToString() AndAlso
                       currentRow("ves_ass_n_dose").ToString() = sourceRow("ves_ass_n_dose").ToString() AndAlso
                       currentRow("ves_data_effettuazione").ToString() = sourceRow("ves_data_effettuazione").ToString() Then

                    copiaDatiReazione = (currentRow("ves_lot_codice") Is DBNull.Value)

                Else

                    copiaDatiReazione = False

                End If

                If copiaDatiReazione Then

                    Common.ReazioniAvverseCommon.CopyRowsReazioniAvverse(sourceRow, currentRow)

                End If

            End If

        Next

    End Sub

    Private Sub ConfermaReazione()

        ' N.B. : Il metodo GetListEseguiteReazioniAvverse effettua anche i controlli sui campi. 
        '        Se i controlli non vanno a buon fine, solleva l'evento con il messaggio di errore.
        Dim listVaccinazioniEseguite As List(Of Entities.VaccinazioneEseguita) = Me.ReazAvverseDetail.GetListEseguiteReazioniAvverse(True)

        If listVaccinazioniEseguite.IsNullOrEmpty() Then Return

        Me.ConfermaReazione(listVaccinazioniEseguite)

    End Sub

    Private Sub ConfermaReazione(listVaccinazioniEseguite As List(Of VaccinazioneEseguita))

        For Each vaccinazioneEseguita As VaccinazioneEseguita In listVaccinazioniEseguite

            Dim rowKey As Object()

            If IsGestioneCentrale Then
                rowKey = New Object() {vaccinazioneEseguita.ves_id, vaccinazioneEseguita.ves_usl_inserimento}
            Else
                rowKey = New Object() {vaccinazioneEseguita.ves_id}
            End If

            Dim row As DataRow = dt_vacEseguite.Rows.Find(rowKey)

            ' Modifica dati reazione della riga relativa alla reazione
            Common.ReazioniAvverseCommon.UpdateDataRowByEntity(row, vaccinazioneEseguita.ReazioneAvversa)

            ' Modifica dei dati relativi alla reazione anche per le altre vaccinazioni della stessa associazione-dose
            InserisciReazioniAvverseAltreVaccinazioniStessaAssociazione(row)

        Next

        IsEditReazioneAvversa = True

        DataBindDatagrid(OrdVacEseguite_Campo + " " + OrdVacEseguite_Verso)
        ChangeModeViewDdg(ModeViewDgrStatus)
        ToVac()

    End Sub

    Private Function CreateListEseguiteReazioniSelezionate(rowAssociazioneSelezionata As DataRow) As List(Of Entities.VaccinazioneEseguita)

        Dim selectedRows As New List(Of DataRow)()

        ' Creazione entità relative alle eseguite selezionate
        For i As Int16 = 0 To Me.dgrAssEseguite.Items.Count - 1

            If DirectCast(Me.dgrAssEseguite.Items(i).FindControl("cb"), CheckBox).Checked Then

                Dim selectedRow As DataRow = Me.dtAssociazioni.Rows.Find(FindRowKey(i))

                Dim selectedRowCopy As DataRow = Me.dtAssociazioni.NewRow()
                selectedRowCopy.ItemArray = selectedRow.ItemArray

                selectedRows.Add(selectedRowCopy)

            End If

        Next

        Return Me.ReazAvverseDetail.CreateListEseguiteReazioniSelezionate(selectedRows, rowAssociazioneSelezionata, Me.IsGestioneCentrale)

    End Function

#End Region

    ' N.B. : Questo metodo aggiunge l'alert alla stringa che viene renderizzata, quindi attenzione all'eventuale 
    '        presenza di altri alert (verranno visualizzati tutti separatamente, controllare se si possono accorpare)
    Private Function ShowAlertAggiornamentoStoricoCentralizzato() As Boolean

        ' Controllo per visualizzare alert solo se la usl corrente ha dato visibilità
        ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
        If FlagConsensoVaccUslCorrente Then

            If Settings.ALERT_AGGIORNAMENTO_DATI_CENTRALIZZATI AndAlso ShowMessaggioAggiornamentoAnagrafeCentrale Then

                strJS &= Common.OnVacStoricoVaccinaleCentralizzato.AlertMessageAggiornamentoAnagrafeCentrale

                ShowMessaggioAggiornamentoAnagrafeCentrale = False

            End If

        End If

    End Function

    Private Sub RecuperaStoricoVaccinale()

        Dim command As New Common.OnVacStoricoVaccinaleCentralizzato.AcquisisciStoricoCommand()
        command.CodicePaziente = OnVacUtility.Variabili.PazId
        command.RichiediConfermaSovrascrittura = False
        command.Settings = Me.Settings
        command.OnitLayout3 = Me.OnitLayout31
        command.BizLogOptions = OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.VAC_ESEGUITE)
        command.Note = "Recupero Storico Vaccinale da maschera VacEseguite"

        Dim acquisisciDatiVaccinaliCentraliResult As Biz.BizPaziente.AcquisisciDatiVaccinaliCentraliResult =
            Common.OnVacStoricoVaccinaleCentralizzato.AcquisisciDatiVaccinaliCentraliPaziente(command)

        If acquisisciDatiVaccinaliCentraliResult.StatoAcquisizioneDatiVaccinaliCentrale <> Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Parziale Then

            ' Se il recupero è andato a buon fine
            Me.SelectedPageState = StatoPagina.VacEseguiteListView

            Me.SetToolbarStatus(False)

        End If

        ' Ricarica la pagina
        Me.OnPageLoad(Me.LayoutTitolo1, Me.OnitLayout31, True)
        Me.LayoutTitolo2.Text = Me.LayoutTitolo1.Text

        Me.RegisterDataGridStructure()

        ' Rebind del datagrid (ordinato di default)
        Me.ClearOrderDatagridHeader()
        Me.ImpostaLayoutDatagridEseguite()

        Me.SetPrimaryKeyDtEseguite()

        ' Vista ridotta
        Me.ChangeModeViewDdg(ModeViewDgr.NoDettaglio)
        Me.ModeViewDgrStatus = ModeViewDgr.NoDettaglio

    End Sub

    Private Function GetColumnBySortExpression(sortExpression As String) As DataGridColumn

        If dgrAssEseguite.Items.Count = 0 Then Return Nothing

        Dim column As DataGridColumn =
            (From item As DataGridColumn In Me.dgrAssEseguite.Columns
             Where item.SortExpression = sortExpression
             Select item).FirstOrDefault()

        Return column

    End Function

    Private Sub SetColumnVisibility(sortExpression As String, visible As Boolean)

        Dim column As DataGridColumn = GetColumnBySortExpression(sortExpression)

        If Not column Is Nothing Then column.Visible = visible

    End Sub

	Private Sub StampaCertificatoVaccinale()

		Dim codicePazienteLocale As String = String.Empty

		Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
			codicePazienteLocale = genericProvider.Paziente.GetCodicePazientiByCodiceAusiliario(OnVacUtility.Variabili.PazId).FirstOrDefault()
		End Using

		If String.IsNullOrEmpty(codicePazienteLocale) Then
			strJS += "alert('Impossibile stampare il certificato vaccinale per il paziente corrente: paziente non presente in anagrafe locale.');"
			Return
		End If

		' Url della pagina, per gestire correttamente il ritorno dopo la chiusura della pagina di stampa
		Dim pageUrl As String = Request.Path

		If Not Request.QueryString Is Nothing AndAlso Request.QueryString.Count > 0 Then
			pageUrl = pageUrl + "?" + Request.QueryString.ToString()
		End If

		OnVacUtility.StampaCertificatoVaccinale(Me.Page, pageUrl, Settings, "{T_PAZ_PAZIENTI.PAZ_CODICE}=" + codicePazienteLocale, True)

	End Sub

	Private Sub StampaCertificatoVaccinaleGiornaliero()




		Dim countRigheSelezionate As Integer = 0
		Dim data As String = String.Empty
		Dim primaData As String = String.Empty
		Dim dataDiff As Boolean = False
		For i As Int16 = 0 To dgrAssEseguite.Items.Count - 1
			If DirectCast(dgrAssEseguite.Items(i).FindControl("cb"), CheckBox).Checked Then

				countRigheSelezionate += 1
				data = DirectCast(dgrAssEseguite.Items(i).FindControl("tb_data_eff"), Label).Text
				If Not String.IsNullOrWhiteSpace(primaData) AndAlso data <> primaData Then
					dataDiff = True
				Else
					primaData = data
				End If
			End If
		Next

		If countRigheSelezionate = 0 Then

			strJS &= String.Format("alert('Selezionare almeno una riga');")
			Return
		ElseIf dataDiff Then
			strJS &= String.Format("alert('Selezionare vaccinazioni effettuati nella stessa data');")
			Return

		End If

		' Url della pagina, per gestire correttamente il ritorno dopo la chiusura della pagina di stampa
		Dim pageUrl As String = Request.Path

		If Not Request.QueryString Is Nothing AndAlso Request.QueryString.Count > 0 Then
			pageUrl = pageUrl + "?" + Request.QueryString.ToString()
		End If
		Dim sf = "{T_PAZ_PAZIENTI.PAZ_CODICE}=" + OnVacUtility.Variabili.PazId + " AND {T_VAC_ESEGUITE.VES_DATA_EFFETTUAZIONE}=DATE('" + primaData + "')"
        OnVacUtility.StampaCertificatoVaccinaleGiornaliero(Me.Page, pageUrl, Settings, sf, False, primaData)

    End Sub

	Private Sub SetPrimaryKeyDtEseguite()

        If IsGestioneCentrale Then
            OnVacUtility.addKey(dt_vacEseguite, "ves_id", "ves_usl_inserimento")
        Else
            OnVacUtility.addKey(dt_vacEseguite, "ves_id")
        End If

    End Sub

#End Region

#Region " Protected Methods "

    Protected Function BindFlagVisibilitaImageUrlValue(dataItem As Object) As String

        Return Common.OnVacStoricoVaccinaleCentralizzato.GetImageUrlFlagVisibilita(dataItem, "VES_FLAG_VISIBILITA", Me)

    End Function

    Protected Function BindFlagVisibilitaToolTipValue(dataItem As Object) As String

        Return Common.OnVacStoricoVaccinaleCentralizzato.GetToolTipFlagVisibilita(dataItem, "VES_FLAG_VISIBILITA")

    End Function

#End Region

#Region " Eventi User control Reazioni "

    Private Sub ReazAvverseDetail_RecuperaConcomitanti(sender As Object, ByRef farmaciRecuperabili As List(Of Common.ReazioniAvverseCommon.FarmacoRecupero)) Handles ReazAvverseDetail.RecuperaConcomitanti

        farmaciRecuperabili = Common.ReazioniAvverseCommon.DataTableToListFarmacoRecupero(Me.dtAssociazioni)

    End Sub

    Private Sub ReazAvverseDetail_ShowMessage(sender As Object, e As Common.ReazioniAvverseCommon.ShowMessageReazioniEventArgs) Handles ReazAvverseDetail.ShowMessage

        Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(HttpUtility.JavaScriptStringEncode(e.Message), "ALERT", False, False))

    End Sub

    Protected Sub chkFilterAntiInfluenzali_CheckedChanged(sender As Object, e As EventArgs)

        DataBindDatagrid(OrdVacEseguite_Campo + " " + OrdVacEseguite_Verso)

    End Sub

#End Region

End Class
