Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities

Partial Class GestioneBilancio
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

#Region " Constants "

    Private Const CONFIRM_ERROR_EMPTY_FIELDS As String = "NR"
    Private Const CONFIRM_ERROR_EMPTY_FIELDS_FIRMA_DIGITALE As String = "FDC"
    Private Const CONFIRM_ERROR_EMPTY_FIELDS_STAMPA_BILANCIO As String = "SB"
    Private Const FIRMA_DIGITALE_COMPLETED As String = "FC"

#End Region

#Region " Properties "

    Private Property DatiPaziente() As Entities.PazienteDatiAnagrafici
        Get
            Return Session("Paziente_GestioneBilancio")
        End Get
        Set(Value As Entities.PazienteDatiAnagrafici)
            Session("Paziente_GestioneBilancio") = Value
        End Set
    End Property

    Private _CodiceMedicoUtenteLoggato As String
    Private ReadOnly Property CodiceMedicoUtenteLoggato()
        Get
            If _CodiceMedicoUtenteLoggato Is Nothing Then

                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                    _CodiceMedicoUtenteLoggato = genericProvider.Utenti.GetMedicoDaUtente(OnVacContext.UserId)

                End Using

            End If

            Return _CodiceMedicoUtenteLoggato
        End Get
    End Property

    Private ReadOnly Property UtenteLoggatoIsMedico() As Boolean
        Get
            Return Me.CodiceMedicoUtenteLoggato <> Nothing
        End Get
    End Property

    Public Property IsVacProgRequest() As Boolean
        Get
            If ViewState("IsVacProgRequest") Is Nothing Then ViewState("IsVacProgRequest") = False
            Return Convert.ToBoolean(ViewState("IsVacProgRequest"))
        End Get
        Set(value As Boolean)
            ViewState("IsVacProgRequest") = value
        End Set
    End Property

    Public Property IsCnvRequest() As Boolean
        Get
            If ViewState("IsCnvRequest") Is Nothing Then ViewState("IsCnvRequest") = False
            Return Convert.ToBoolean(ViewState("IsCnvRequest"))
        End Get
        Set(value As Boolean)
            ViewState("IsCnvRequest") = value
        End Set
    End Property

    Public ReadOnly Property EditCnv() As String
        Get
            If Not Me.Request.QueryString("EditCnv") Is Nothing Then
                Return HttpUtility.UrlDecode(Me.Request.QueryString("EditCnv"))
            End If
            Return String.Empty
        End Get
    End Property

    Public ReadOnly Property DataCnv() As String
        Get
            If Not Me.Request.QueryString("DataCnv") Is Nothing Then
                Return HttpUtility.UrlDecode(Me.Request.QueryString("DataCnv"))
            End If
            Return String.Empty
        End Get
    End Property

    Private Property StoricoVaccinaleCentralizzatoDaRecuperare() As String
        Get
            If ViewState("StoricoVaccinaleCentralizzatoDaRecuperare") Is Nothing Then ViewState("StoricoVaccinaleCentralizzatoDaRecuperare") = False
            Return ViewState("StoricoVaccinaleCentralizzatoDaRecuperare")
        End Get
        Set(value As String)
            ViewState("StoricoVaccinaleCentralizzatoDaRecuperare") = value
        End Set
    End Property

    Private ReadOnly Property IsVisibilitaCentraleDatiVaccinaliPaziente() As Boolean
        Get
            If ViewState("IsVisibilitaCentraleDatiVaccinaliPaziente") Is Nothing Then
                ViewState("IsVisibilitaCentraleDatiVaccinaliPaziente") =
                    Common.OnVacStoricoVaccinaleCentralizzato.IsVisibilitaCentraleDatiVaccinaliPaziente(OnVacUtility.Variabili.PazId, Settings)
            End If
            Return Convert.ToBoolean(ViewState("IsVisibilitaCentraleDatiVaccinaliPaziente"))
        End Get
    End Property

    Private Property FirmaDigitaleDopoSalvataggio() As Boolean
        Get
            If ViewState("FIRMA") Is Nothing Then ViewState("FIRMA") = False
            Return Convert.ToBoolean(ViewState("FIRMA"))
        End Get
        Set(value As Boolean)
            ViewState("FIRMA") = value
        End Set
    End Property

    Private Property StampaAnamnesiDopoSalvataggio() As Boolean
        Get
            If ViewState("STAMPA") Is Nothing Then ViewState("STAMPA") = False
            Return Convert.ToBoolean(ViewState("STAMPA"))
        End Get
        Set(value As Boolean)
            ViewState("STAMPA") = value
        End Set
    End Property

    Private ReadOnly Property NumeroBilancio() As Integer?
        Get
            If Me.Request.QueryString("bil_numero") Is Nothing Then Return Nothing
            Return Convert.ToInt32(Me.Request.QueryString("bil_numero"))
        End Get
    End Property

    Private ReadOnly Property CodiceMalattia() As String
        Get
            Return Me.Request.QueryString("mal_codice")
        End Get
    End Property
#Region "Bilancio Padre o Follow Up"
    Private ReadOnly Property VesIdPadre() As Integer?
        Get
            If Me.Request.QueryString("vesIdPadre") Is Nothing Then Return Nothing
            Return Convert.ToInt32(Me.Request.QueryString("vesIdPadre"))
        End Get
    End Property
    Public Property IsFollowUpRequest() As Boolean
        Get
            If ViewState("IsFollowUpRequest") Is Nothing Then ViewState("IsFollowUpRequest") = False
            Return Convert.ToBoolean(ViewState("IsFollowUpRequest"))
        End Get
        Set(value As Boolean)
            ViewState("IsFollowUpRequest") = value
        End Set
    End Property
#End Region

#End Region

#Region " Enums "

    Private Enum PageStatus
        Load = 0
        Edit = 1
        StoricoDaRecuperare = 2
    End Enum

    Private Enum OperazioniSalvataggio
        Salva
        Salva_Firma
        Salva_Stampa
    End Enum

#End Region

#Region " Eventi Page "

    Public Sub New()

        AddHandler Me.ServerHistoryBack, AddressOf GestioneBilancio_ServerHistoryBack

    End Sub

    Private Sub GestioneBilancio_ServerHistoryBack(sender As Object, e As EventArgs)

        ' Scatta quando si torna a questa pagina attraverso un HistoryNavigateBack.
        ' In questo caso, succede quando viene chiusa l'anteprima di stampa.
        ' Poichè la History conserva il viewstate, controllo le variabili nel viewstate 
        ' per decidere se rimanere qui o tornare alla pagina chiamante (convocazioni o programmate).

        If Me.IsVacProgRequest Then
            RedirectToConvocazioni()
        ElseIf Me.IsCnvRequest Then
            RedirectToListaConvocazioni()
        ElseIf IsFollowUpRequest Then
            RedirectToVisioneBilanci()
        End If

    End Sub

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '--
        If Not IsPostBack Then
            '--
            ' Gestione visibilità pulsante Recupera Storico
            ToolBar.Items.FromKeyButton("btnRecuperaStoricoVacc").Visible = False
            '--
            ' Gestione visibilità Label e Check Visibilità
            lblFlagVisibilita.Visible = True
            chkFlagVisibilita.Visible = True
            '--
            divPercentili.Visible = False
            '--
            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                '--
                ' Intestazione pagina
                OnVacUtility.ImpostaIntestazioniPagina(OnitLayout31, LayoutTitolo, genericProvider, Settings, IsGestioneCentrale)
                '--
                ' Caricamento dati paziente
                Using bizPaziente As New Biz.BizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                    DatiPaziente = bizPaziente.GetDatiAnagraficiPaziente(OnVacUtility.Variabili.PazId, IsGestioneCentrale)
                End Using
                '--
            End Using
            '--
            InizializzaPagina()
            '--
            ' Stato della pagina: servirà per impostare la toolbar
            Dim currentPageStatus As PageStatus = PageStatus.Load
            '--
            ' Controllo se la richiesta di compilazione arriva dalle vaccinazioni programmate
            IsVacProgRequest = GetBooleanValueFromQueryString("isVacProgRequest")
            '--
            ' Controllo se la richiesta di compilazione arriva dalle convocazioni
            IsCnvRequest = GetBooleanValueFromQueryString("isCnvRequest")
            IsFollowUpRequest = GetBooleanValueFromQueryString("isFollowUpRequest")
            '--
            If IsVacProgRequest Then
                '--
                If NumeroBilancio.HasValue AndAlso Not String.IsNullOrWhiteSpace(CodiceMalattia) Then
                    '--
                    Dim bilancioSelezionato As BilancioAnagrafica = CommonBilancio.GetBilancioAnagrafica(NumeroBilancio.Value, CodiceMalattia, Settings)
                    '--
                    If Not bilancioSelezionato Is Nothing Then
                        ImpostaBilancioSelezionato(bilancioSelezionato)
                    End If
                    '--
                End If
                '--
                currentPageStatus = PageStatus.Edit
                '--
            ElseIf IsCnvRequest Then
                '--
                ShowModaleSceltaBilancio()
                '--
                currentPageStatus = PageStatus.Edit
                '--
            ElseIf IsFollowUpRequest Then
                If NumeroBilancio.HasValue AndAlso Not String.IsNullOrWhiteSpace(CodiceMalattia) Then
                    Dim bilancioSelezionato As BilancioAnagrafica = CommonBilancio.GetBilancioAnagrafica(NumeroBilancio.Value, CodiceMalattia, Settings)
                    If Not bilancioSelezionato Is Nothing Then
                        ImpostaBilancioSelezionato(bilancioSelezionato)
                    End If
                End If
                currentPageStatus = PageStatus.Edit
            Else
                '--
                ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
                If FlagConsensoVaccUslCorrente Then
                    '--
                    Dim statoAcquisizioneDatiVaccinaliCentrale As Enumerators.StatoAcquisizioneDatiVaccinaliCentrale? =
                        Common.OnVacStoricoVaccinaleCentralizzato.GetStatoAcquisizioneDatiVaccinaliCentralePaziente(OnVacUtility.Variabili.PazId)
                    '--
                    If Not statoAcquisizioneDatiVaccinaliCentrale.HasValue OrElse
                        statoAcquisizioneDatiVaccinaliCentrale = Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Parziale Then
                        '--
                        StoricoVaccinaleCentralizzatoDaRecuperare = True
                        '--
                        OnitLayout31.InsertRoutineJS(Common.OnVacStoricoVaccinaleCentralizzato.AlertMessageRecuperoStoricoVaccinale)
                        '--
                        currentPageStatus = PageStatus.StoricoDaRecuperare
                        '--
                    End If
                    '--
                End If
                '--
            End If
            '--
            SetToolBarStatus(currentPageStatus)
            '--
        End If
        '--
    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btnBilancio"

                ShowModaleSceltaBilancio()

            Case "btnSalva"

                If CheckQuestionario(OperazioniSalvataggio.Salva) Then
                    Me.CheckAndSave(OperazioniSalvataggio.Salva)
                End If

            Case "btnSalvaFirma"

                If CheckQuestionario(OperazioniSalvataggio.Salva_Firma) Then
                    Me.CheckAndSave(OperazioniSalvataggio.Salva_Firma)
                End If

            Case "btnSalvaStampa"

                If CheckQuestionario(OperazioniSalvataggio.Salva_Stampa) Then
                    Me.CheckAndSave(OperazioniSalvataggio.Salva_Stampa)
                End If

            Case "btnAnnulla"

                If Me.IsVacProgRequest Then
                    RedirectToConvocazioni()
                ElseIf Me.IsCnvRequest Then
                    RedirectToListaConvocazioni()

                ElseIf IsFollowUpRequest Then
                    RedirectToVisioneBilanci()
                Else
                    Me.Pulisci()
                    Me.AbilitaCampi(False)
                    Me.uscRicBil_AnnullaBilancio()
                    Me.OnitLayout31.Busy = False
                End If

            Case "btnCompilaBilancio"

                GeneraQuestionario(Enumerators.TipoCompilazioneBilancio.Predefinita)

            Case "btnRecuperaStoricoVacc"

                Me.RecuperaStoricoVaccinale()

        End Select

    End Sub

#End Region

#Region " Eventi UserControl Ricerca Bilancio "

    Private Sub uscRicBil_ReturnBilancio(bilancioSelezionato As Entities.BilancioAnagrafica) Handles uscRicBil.ReturnBilancio

        ImpostaBilancioSelezionato(bilancioSelezionato)

        SetToolBarStatus(PageStatus.Edit)

    End Sub

    Private Sub ImpostaBilancioSelezionato(bilancioSelezionato As Entities.BilancioAnagrafica)

        If bilancioSelezionato Is Nothing Then

            Pulisci()

            Me.divMisure.Visible = False

            Me.lblCranio.Visible = False
            Me.txtCranio.Visible = False

            Me.lblAltezza.Visible = False
            Me.txtAltezza.Visible = False

            Me.lblPeso.Visible = False
            Me.txtPeso.Visible = False

            Me.lblPercentileCranio.Visible = False
            Me.txtPercentileCranio.Visible = False

            Me.lblPercentileAltezza.Visible = False
            Me.txtPercentileAltezza.Visible = False

            Me.lblPercentilePeso.Visible = False
            Me.txtPercentilePeso.Visible = False

            Me.ucDatiOpzionaliBilancio.Visible = False

        Else

            Me.txtData.Data = DateTime.Today
            Me.txtMalattia.Codice = bilancioSelezionato.CodiceMalattia
            Me.txtMalattia.Descrizione = bilancioSelezionato.DescrizioneMalattia
            Me.txtNumero.Text = bilancioSelezionato.NumeroBilancio

            Dim tipoCompilazione As Enumerators.TipoCompilazioneBilancio = Enumerators.TipoCompilazioneBilancio.Normale

            If Me.IsVacProgRequest Then

                If Not bilancioSelezionato.GestioneViaggi Then
                    tipoCompilazione = Enumerators.TipoCompilazioneBilancio.RispostaPrecedente
                End If

            End If

            GeneraQuestionario(tipoCompilazione)

            Me.OnitLayout31.Busy = True

            'caricamento della combo vaccinabile [modifica 01/04/2005]
            Me.CaricaVaccinabile()

            'riabilita la schermata per l'inserimento dei dati [modifica 05/04/2005]
            AbilitaCampi(True)

            Me.divMisure.Visible = bilancioSelezionato.GestioneCranio OrElse bilancioSelezionato.GestioneAltezza OrElse bilancioSelezionato.GestionePeso

            Me.lblCranio.Visible = bilancioSelezionato.GestioneCranio
            Me.txtCranio.Visible = bilancioSelezionato.GestioneCranio

            Me.lblAltezza.Visible = bilancioSelezionato.GestioneAltezza
            Me.txtAltezza.Visible = bilancioSelezionato.GestioneAltezza

            Me.lblPeso.Visible = bilancioSelezionato.GestionePeso
            Me.txtPeso.Visible = bilancioSelezionato.GestionePeso

            Me.lblPercentileCranio.Visible = bilancioSelezionato.GestioneCranio
            Me.txtPercentileCranio.Visible = bilancioSelezionato.GestioneCranio

            Me.lblPercentileAltezza.Visible = bilancioSelezionato.GestioneAltezza
            Me.txtPercentileAltezza.Visible = bilancioSelezionato.GestioneAltezza

            Me.lblPercentilePeso.Visible = bilancioSelezionato.GestionePeso
            Me.txtPercentilePeso.Visible = bilancioSelezionato.GestionePeso

            Me.ddlVaccinabile.SelectedValue = "S"
            Me.ddlVaccinabile.Enabled = Not Me.UtenteLoggatoIsMedico
            Me.ddlVaccinabile.CssClass = IIf(Me.UtenteLoggatoIsMedico, "TextBox_Stringa_Disabilitato", "TextBox_Stringa")

            Me.odpFineSospensione.Enabled = Not Me.UtenteLoggatoIsMedico
            Me.odpFineSospensione.CssClass = IIf(Me.UtenteLoggatoIsMedico, "TextBox_Data_Disabilitato", "TextBox_Data")

            If Me.Settings.BILANCI_PREVALORIZZA_OPERATORI Then

                Me.txtMedico.Codice = OnVacUtility.Variabili.MedicoResponsabile.Codice
                Me.txtMedico.Descrizione = OnVacUtility.Variabili.MedicoResponsabile.Nome

                ' Impostazione automatica rilevatore: se è già stato memorizzato un valore, imposto il rilevatore delle OnVacUtility.
                ' Altrimenti, imposto come rilevatore il vaccinatore selezionato nelle vac prog.
                If Not String.IsNullOrWhiteSpace(OnVacUtility.Variabili.MedicoRilevatore.Codice) Then
                    Me.fmRilevatore.Codice = OnVacUtility.Variabili.MedicoRilevatore.Codice
                    Me.fmRilevatore.Descrizione = OnVacUtility.Variabili.MedicoRilevatore.Nome
                Else
                    Me.fmRilevatore.Codice = OnVacUtility.Variabili.MedicoVaccinante.Codice
                    Me.fmRilevatore.Descrizione = OnVacUtility.Variabili.MedicoVaccinante.Nome
                End If

            End If

            ' Impostazione filtri modali operatori
            txtMedico.Filtro = OnVacUtility.GetModalListFilterOperatori(True, True)
            fmRilevatore.Filtro = OnVacUtility.GetModalListFilterOperatori(False, True)


            ' Dati opzionali (vaccinazioni e viaggio)
            Me.ucDatiOpzionaliBilancio.Enabled = True

            Dim datiOpzionali As New DatiOpzionaliBilancio.DatiOpzionaliSettings()
            datiOpzionali.ShowDatiVaccinazioni = bilancioSelezionato.GestioneVaccinazioni
            datiOpzionali.ShowDatiViaggio = bilancioSelezionato.GestioneViaggi


            If Me.IsVacProgRequest Then
                If Not String.IsNullOrWhiteSpace(Me.DataCnv) Then

                    Dim dataConvocazione As DateTime = DateTime.MinValue

                    If DateTime.TryParse(Me.DataCnv, dataConvocazione) Then

                        datiOpzionali.DataConvocazione = dataConvocazione

                    End If

                End If
            End If
            If VesIdPadre.HasValue AndAlso IsFollowUpRequest Then
                Dim viaggio As New List(Of ViaggioVisita)
                Dim visitaPadre As New Visita
                Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                    Using bizviaggi As New Biz.BizVisite(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                        viaggio = bizviaggi.GetListaViaggiVisita(VesIdPadre.Value)
                        visitaPadre = bizviaggi.GetVisitaById(VesIdPadre.Value)
                    End Using
                End Using
                For Each v As ViaggioVisita In viaggio
                    v.Id = Nothing
                    v.IdVisita = Nothing
                    v.Operazione = OperazioneViaggio.Insert
                Next
                datiOpzionali.DatiViaggi = viaggio
                Dim dataMax As Date = Nothing
                If visitaPadre.DataFollowUpPrevisto.HasValue Or visitaPadre.DataFollowUpPrevisto > DateTime.MinValue Then
                    dataMax = visitaPadre.DataFollowUpPrevisto
                Else
                    dataMax = viaggio.Where(Function(p) p.Operazione <> OperazioneViaggio.Delete).Max(Function(p) p.DataFineViaggio).Date
                    dataMax = dataMax.AddDays(Settings.NUM_GIORNI_FOLLOWUP)
                End If

                datiOpzionali.DataFollowUpPrevista = dataMax
                datiOpzionali.DataFollowUpEffettiva = Today
                'Ricalcolo stringa delle vaccinazioni eseguite nel periodo
                If bilancioSelezionato.GestioneVaccinazioni Then
                    Dim nuovaListaVaccini As String = ElencoVaccinazioni(OnVacContext.UserId, visitaPadre.DataRegistrazione, Today)
                    datiOpzionali.DatiVaccinazioniBilancio = nuovaListaVaccini
                End If
                datiOpzionali.VisibleDataEffettiva = True
            End If

            Me.ucDatiOpzionaliBilancio.SetDatiOpzionali(datiOpzionali)

        End If

    End Sub

    Private Sub uscRicBil_AnnullaBilancio() Handles uscRicBil.AnnullaBilancio

        If Me.IsCnvRequest Then
            RedirectToListaConvocazioni()
        Else
            ImpostaBilancioSelezionato(Nothing)
            SetToolBarStatus(PageStatus.Load)
        End If

    End Sub

#End Region

#Region " Eventi pulsanti spostamento cnv "

    'sposta le convocazioni precedenti la data di fine sospensione
    Private Sub btnSpostaOK_Click(sender As Object, e As System.EventArgs) Handles btnSpostaOK.Click

        Me.fmUnisciCnv.VisibileMD = False

        Dim saveResult As SalvaVisitaResult = Me.Salva()

        If saveResult.Success Then

            If Me.FirmaDigitaleDopoSalvataggio Then

                Me.FirmaDigitaleDopoSalvataggio = False
                Me.SwitchViewFirmaDigitale(saveResult.IdVisita)

            ElseIf Me.StampaAnamnesiDopoSalvataggio Then

                Me.StampaAnamnesiDopoSalvataggio = False

                Dim command As New CommonBilancio.StampaBilancioCommand()
                command.CodicePaziente = OnVacUtility.Variabili.PazId
                command.IdVisita = saveResult.IdVisita
                command.IsCentrale = Me.IsGestioneCentrale
                command.NumeroBilancio = Me.Questionario.NumeroBilancio
                command.CodiceMalattia = Me.Questionario.CodiceMalattia
                command.Page = Me.Page
                command.Settings = Me.Settings

                Dim result As CommonBilancio.StampaBilancioResult = CommonBilancio.StampaBilancio(command)

                If Not result.Success Then
                    Me.OnitLayout31.InsertRoutineJS(String.Format("alert('{0}')", HttpUtility.JavaScriptStringEncode(result.Message)))
                End If

            Else

                ' N.B. : se la cnv è stata spostata, il redirect riporta alla lista delle convocazioni anche provenendo dalle programmate
                'TODO[VisioneAnamnesi] anche qui devo fare controllo per follow up????
                If Me.IsVacProgRequest Or Me.IsCnvRequest Then Me.RedirectToListaConvocazioni()

            End If

        Else
            Me.FirmaDigitaleDopoSalvataggio = False
            Me.StampaAnamnesiDopoSalvataggio = False
        End If

    End Sub

    'annulla lo spostamento delle convocazioni ed il salvataggio [modifica 04/04/2005]
    Private Sub btnSpostaAnnulla_Click(sender As Object, e As System.EventArgs) Handles btnSpostaAnnulla.Click

        Me.FirmaDigitaleDopoSalvataggio = False
        Me.StampaAnamnesiDopoSalvataggio = False

        Me.fmUnisciCnv.VisibileMD = False

    End Sub

#End Region

#Region " Eventi OnitLayout "

    Private Sub OnitLayout31_AlertClick(sender As Object, e As Controls.PagesLayout.OnitLayout3.AlertEventArgs) Handles OnitLayout31.AlertClick

        Select Case e.Key

            Case FIRMA_DIGITALE_COMPLETED

                If Me.IsVacProgRequest Then
                    RedirectToConvocazioni()
                ElseIf Me.IsCnvRequest Then
                    RedirectToListaConvocazioni()
                    'TODO[VisioneAnamnesi] anche qui devo fare controllo per follow up????
                Else
                    SwitchViewDati()
                End If

        End Select

    End Sub

    Private Sub OnitLayout31_ConfirmClick(sender As Object, e As Onit.Controls.PagesLayout.OnitLayout3.ConfirmEventArgs) Handles OnitLayout31.ConfirmClick

        Select Case e.Key

            Case CONFIRM_ERROR_EMPTY_FIELDS

                If e.Result Then
                    Me.CheckAndSave(OperazioniSalvataggio.Salva)
                End If

            Case CONFIRM_ERROR_EMPTY_FIELDS_FIRMA_DIGITALE

                If e.Result Then
                    Me.CheckAndSave(OperazioniSalvataggio.Salva_Firma)
                End If

            Case CONFIRM_ERROR_EMPTY_FIELDS_STAMPA_BILANCIO

                If e.Result Then
                    Me.CheckAndSave(OperazioniSalvataggio.Salva_Stampa)
                End If

        End Select

    End Sub

#End Region

#Region " Firma Digitale "

    Private Enum ViewIndex
        ViewDati = 0
        ViewFirmaDigitale = 1
    End Enum

    Private Sub SwitchViewDati()

        Me.multiViewMain.SetActiveView(Me.multiViewMain.Views(ViewIndex.ViewDati))

    End Sub

    Private Sub SwitchViewFirmaDigitale(idVisita As Integer)

        Me.LayoutTitoloViewFirma.Text = Me.LayoutTitolo.Text

        Me.multiViewMain.SetActiveView(Me.multiViewMain.Views(ViewIndex.ViewFirmaDigitale))

        Me.ucFirma.AnteprimaAnamnesi(idVisita)

    End Sub

    Private Sub ucFirma_FirmaDigitaleCompleted(success As Boolean, message As String) Handles ucFirma.FirmaDigitaleCompleted

        If success And (Me.IsVacProgRequest Or Me.IsCnvRequest) Then
            message += Environment.NewLine + "Verrà visualizzata la maschera delle convocazioni."
        End If

        Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(HttpUtility.JavaScriptStringEncode(message), FIRMA_DIGITALE_COMPLETED, False, success))

    End Sub

    Private Sub ucFirma_ShowMessage(message As String) Handles ucFirma.ShowMessage

        ClientScript.RegisterClientScriptBlock(GetType(Page), "js",
            String.Format("<script type='text/javascript'>alert('{0}');</script>", HttpUtility.JavaScriptStringEncode(message)))

    End Sub

#End Region

#Region " Private "

#Region " Caricamento dati "

    'caricamento della combo vaccinabile [modifica 01/04/2005]
    Private Sub CaricaVaccinabile()

        Dim collCodifiche As Collection.CodificheCollection

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizCodifiche As New Biz.BizCodifiche(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                collCodifiche = bizCodifiche.GetCodifiche("VIS_VACCINABILE")

            End Using
        End Using

        Me.ddlVaccinabile.DataSource = collCodifiche
        Me.ddlVaccinabile.DataBind()
        Me.ddlVaccinabile.Items.Insert(0, New ListItem(String.Empty, String.Empty))
        Me.ddlVaccinabile.Items(0).Selected = True

    End Sub

#End Region

#Region " Controlli e salvataggio "

    ' Controllo data visita antecedente nascita, sovrapposizioni, salvataggio e firma digitale
    Private Sub CheckAndSave(operazione As OperazioniSalvataggio)

        Dim saveResult As New SalvaVisitaResult(False, 0)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            ' Controllo data visita antecedente nascita
            Dim dataNascita As DateTime = genericProvider.Paziente.GetDataNascita(OnVacUtility.Variabili.PazId)
            If txtData.Data < dataNascita Then

                OnitLayout31.InsertRoutineJS("alert('Salvataggio non effettuato.\nLa data della visita non puo\' essere antecedente alla data di nascita del paziente.')")
                Return

            End If

            ' Calcolo percentili
            Using bizBilancio As New Biz.BizBilancioProgrammato(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                If Not String.IsNullOrEmpty(txtPeso.Text) Then
                    txtPercentilePeso.Text =
                        bizBilancio.CalcolaPercentile(Enumerators.TipoPercentile.Peso, txtPeso.Text, DatiPaziente.DataNascita.Value, DatiPaziente.Sesso)
                End If

                If Not String.IsNullOrEmpty(txtAltezza.Text) Then
                    txtPercentileAltezza.Text =
                        bizBilancio.CalcolaPercentile(Enumerators.TipoPercentile.Altezza, txtAltezza.Text, DatiPaziente.DataNascita.Value, DatiPaziente.Sesso)
                End If

                If Not String.IsNullOrEmpty(txtCranio.Text) Then
                    txtPercentileCranio.Text =
                        bizBilancio.CalcolaPercentile(Enumerators.TipoPercentile.Cranio, txtCranio.Text, DatiPaziente.DataNascita.Value, DatiPaziente.Sesso)
                End If

            End Using

            ' Controllo date viaggio 
            Dim datiViaggioCorrente As DatiOpzionaliBilancio.DatiViaggio = ucDatiOpzionaliBilancio.GetDatiViaggio()
            If datiViaggioCorrente.DataInizioViaggio.HasValue AndAlso datiViaggioCorrente.DataInizioViaggio > datiViaggioCorrente.DataFineViaggio Then

                OnitLayout31.InsertRoutineJS("alert('Salvataggio non effettuato.\nLa data di inizio del viaggio non puo\' essere successiva alla data di fine.')")
                Return

            End If

            Dim result As Biz.BizGenericResult = ucDatiOpzionaliBilancio.CheckDosiVaccinazioniSelezionate()
            If Not result.Success Then

                OnitLayout31.InsertRoutineJS(String.Format("alert('Salvataggio non effettuato.\n{0}')", result.Message))
                Return

            End If

            ' Se si apre la modale di spostamento, al salvataggio bisogna sapere se l'utente ha scelto di firmare il documento o di visualizzare l'anteprima di stampa.
            FirmaDigitaleDopoSalvataggio = (operazione = OperazioniSalvataggio.Salva_Firma)
            StampaAnamnesiDopoSalvataggio = (operazione = OperazioniSalvataggio.Salva_Stampa)

            'controllo spostamento convocazione con fine sospensione valorizzata
            If EsistonoCnvPrecedentiFineSospensione(odpFineSospensione.Data, genericProvider) Then

                ' Apertura modale di richiesta di continuare con lo spostamento/unione delle convocazioni.
                ' In questo caso, lo spostamento della cnv e il salvataggio avvengono al click del btnSpostaOK.
                fmUnisciCnv.VisibileMD = True
                Return

            Else

                ' Salvataggio senza spostamento cnv
                saveResult = Salva()

            End If

        End Using

        ' Dopo il salvataggio, azzero i flag
        FirmaDigitaleDopoSalvataggio = False
        StampaAnamnesiDopoSalvataggio = False

        If saveResult.Success Then

            Dim numeroBilancioDaStampare As Integer = Questionario.NumeroBilancio
            Dim codiceMalattiaDaStampare As String = Questionario.CodiceMalattia

            ImpostaBilancioSelezionato(Nothing)

            Select Case operazione

                Case OperazioniSalvataggio.Salva

                    If IsVacProgRequest Then
                        RedirectToConvocazioni()
                    ElseIf IsCnvRequest Then
                        RedirectToListaConvocazioni()
                    ElseIf IsFollowUpRequest Then
                        ' Aggiungere anche salvataggio del nuovo id nel padre.
                        RedirectToVisioneBilanci()
                    End If

                Case OperazioniSalvataggio.Salva_Firma

                    SwitchViewFirmaDigitale(saveResult.IdVisita)

                Case OperazioniSalvataggio.Salva_Stampa

                    Dim command As New CommonBilancio.StampaBilancioCommand()
                    command.CodicePaziente = OnVacUtility.Variabili.PazId
                    command.IdVisita = saveResult.IdVisita
                    command.IsCentrale = IsGestioneCentrale
                    command.NumeroBilancio = numeroBilancioDaStampare
                    command.CodiceMalattia = codiceMalattiaDaStampare
                    command.Page = Me.Page
                    command.Settings = Settings

                    Dim result As CommonBilancio.StampaBilancioResult = CommonBilancio.StampaBilancio(command)

                    If Not result.Success Then
                        OnitLayout31.InsertRoutineJS(String.Format("alert('{0}')", HttpUtility.JavaScriptStringEncode(result.Message)))
                    End If

            End Select

        End If

        Return

    End Sub

    Private Class SalvaVisitaResult

        Public Success As Boolean
        Public IdVisita As Integer

        Public Sub New(success As Boolean, idVisita As Integer)
            Me.Success = success
            Me.IdVisita = idVisita
        End Sub

    End Class

    Private Function Salva() As SalvaVisitaResult

        Dim errorMessage As String = String.Empty

        Dim idVisitaInserita As Integer = 0

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, OnVacUtility.GetReadCommittedTransactionOptions())

            Dim visitaAggiunta As Visita = GetVisita()
            Dim osservazioneAggiuntaEnumerable As IEnumerable(Of Osservazione) = Nothing
            ' prima dell'insert verifico se i dati del viaggio sono compilati
            Dim resultViaggi As New Biz.BizGenericResult
            resultViaggi.Success = True
            resultViaggi.Message = String.Empty
            If ucDatiOpzionaliBilancio.Visible Then
                resultViaggi = ucDatiOpzionaliBilancio.CheckViaggi()
            End If



            If resultViaggi.Success Then
                Using dbGenericProviderFactory As New Biz.DbGenericProviderFactory()

                    Using bizVisite As New Biz.BizVisite(dbGenericProviderFactory, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.GEST_BIL))

                        osservazioneAggiuntaEnumerable = Questionario.GetOsservazioni(bizVisite.GenericProvider).AsEnumerable()


                        Dim listaViaggi As New List(Of ViaggioVisita)
                        listaViaggi = ucDatiOpzionaliBilancio.GetListaDatiViaggio()
                        Dim dataPrev As Date = ucDatiOpzionaliBilancio.GetDataFollowUpPrevista()
                        If dataPrev = DateTime.MinValue Then
                            ' se ci sono dei viaggi aggiorno data prevista
                            If Not listaViaggi Is Nothing AndAlso listaViaggi.Count > 0 Then
                                Dim dataMax As Date = listaViaggi.Where(Function(p) p.Operazione <> OperazioneViaggio.Delete).Max(Function(p) p.DataFineViaggio).Date
                                visitaAggiunta.DataFollowUpPrevisto = dataMax.AddDays(Settings.NUM_GIORNI_FOLLOWUP)
                            End If
                        Else
                            visitaAggiunta.DataFollowUpPrevisto = dataPrev.Date
                        End If

                        If VesIdPadre.HasValue Then
                            Dim dataeff As Date = ucDatiOpzionaliBilancio.GetDataFollowUpEffetiva
                            If dataeff > DateTime.MinValue Then
                                visitaAggiunta.DataFollowUpEffettivo = dataeff
                            Else
                                visitaAggiunta.DataFollowUpEffettivo = Today
                            End If

                        End If

                        Dim insertVisitaCommand As New Biz.BizVisite.InserisciVisitaCommand()
                        insertVisitaCommand.Visita = visitaAggiunta
                        insertVisitaCommand.Osservazioni = osservazioneAggiuntaEnumerable.ToArray()
                        insertVisitaCommand.listaViaggi = listaViaggi

                        ' Se il bilancio non è stato compilato in seguito ad una programmazione, ma provenendo dalle vac prog o dalle convocazioni,
                        ' allora non deve essere inserito nella T_BIL_PROGRAMMATI
                        insertVisitaCommand.ProgrammaBilancio = (Not Me.IsVacProgRequest) And (Not Me.IsCnvRequest)
                        insertVisitaCommand.Note = "Inserimento visita da maschera GestioneBilancio"

                        Dim insertVisitaResult As Biz.BizVisite.InserisciVisitaResult = bizVisite.InsertVisita(insertVisitaCommand)

                        If insertVisitaResult.Success Then

                            ' In caso di risultato positivo, memorizzo l'id della visita appena inserita
                            idVisitaInserita = insertVisitaCommand.Visita.IdVisita

                            ' Impostazione del rilevatore
                            If Settings.BILANCI_PREVALORIZZA_OPERATORI AndAlso fmRilevatore.IsValid Then

                                If Not String.IsNullOrWhiteSpace(fmRilevatore.Codice) AndAlso fmRilevatore.Codice <> OnVacUtility.Variabili.MedicoRilevatore.Codice Then
                                    OnVacUtility.Variabili.MedicoRilevatore.Codice = fmRilevatore.Codice
                                    OnVacUtility.Variabili.MedicoRilevatore.Nome = fmRilevatore.Descrizione
                                Else
                                    ' N.B. : se il rilevatore viene lasciato vuoto, non lo sbianco nelle OnVacUtility perchè in gestione bilanci è obbligatorio.
                                End If

                            End If
                            ' devo aggiornare i dati del padre
                            If VesIdPadre.HasValue AndAlso IsFollowUpRequest Then
                                bizVisite.UpdateFollowUp(VesIdPadre, insertVisitaCommand.Visita.IdVisita, insertVisitaCommand.Visita.DataFollowUpEffettivo)
                            End If

                        Else

                            ' In caso di risultato negativo, creo il messaggio di errore
                            errorMessage = insertVisitaResult.Messages.ToJavascriptString()

                        End If

                    End Using

                    If String.IsNullOrEmpty(errorMessage) Then

                        ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
                        If FlagAbilitazioneVaccUslCorrente Then

                            Using bizPaziente As Biz.BizPaziente = New Biz.BizPaziente(dbGenericProviderFactory, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.GEST_BIL, True))

                                Dim aggiornaDatiVaccinaliCentraliCommand As New Biz.BizPaziente.AggiornaDatiVaccinaliCentraliCommand()
                                aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale = bizPaziente.GenericProvider.Paziente.GetCodiceAusiliario(OnVacUtility.Variabili.PazId)
                                aggiornaDatiVaccinaliCentraliCommand.VisitaEnumerable = {visitaAggiunta}
                                aggiornaDatiVaccinaliCentraliCommand.OsservazioneEnumerable = osservazioneAggiuntaEnumerable

                                bizPaziente.AggiornaDatiVaccinaliCentrali(aggiornaDatiVaccinaliCentraliCommand)

                            End Using

                        End If

                    End If

                End Using
            Else
                errorMessage = resultViaggi.Message
            End If
            transactionScope.Complete()

        End Using

        If Not String.IsNullOrEmpty(errorMessage) Then
            Me.OnitLayout31.InsertRoutineJS(String.Format("alert(""Errore!\n{0}"")", errorMessage))
            Return New SalvaVisitaResult(False, idVisitaInserita)
        End If

        ' --- Ripristina la schermata iniziale --- '
        Me.Pulisci()
        Me.AbilitaCampi(False)
        Me.SetToolBarStatus(PageStatus.Load)
        ' ---------------------------------------- '

        Me.OnitLayout31.Busy = False

        Return New SalvaVisitaResult(True, idVisitaInserita)

    End Function

    ' Restituisce false se non esistono convocazioni precedenti la data di fine sospensione impostata (oppure se non è stata impostata nessuna data).
    ' Se ci sono convocazioni con data inferiore alla data di fine sospensione, restituisce true.
    Private Function EsistonoCnvPrecedentiFineSospensione(dataFineSospensione As DateTime, ByRef genericProvider As DAL.DbGenericProvider) As Boolean

        Dim cnvPrecedentiFineSospensione As Boolean = False

        Using bizCnv As New Biz.BizConvocazione(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)
            cnvPrecedentiFineSospensione = bizCnv.EsistonoCnvPrecedentiFineSospensione(OnVacUtility.Variabili.PazId, dataFineSospensione)
        End Using

        Return cnvPrecedentiFineSospensione

    End Function

#End Region

#Region " Layout "

    ' Abilita/disabilita i campi dell'intera schermata
    Private Sub AbilitaCampi(abilita As Boolean)

        ' Abilitazione/disabilitazione del flag di visibilità in base al consenso
        ImpostaFlagVisibilita(abilita)

        ' Testata
        txtMedico.Enabled = abilita
        fmRilevatore.Enabled = abilita

        txtData.CssClass = IIf(abilita, "TextBox_Data_Obbligatorio", "TextBox_Data_Disabilitato")
        txtData.Enabled = abilita

        ddlVaccinabile.Enabled = abilita AndAlso Not UtenteLoggatoIsMedico
        ddlVaccinabile.CssClass = IIf(ddlVaccinabile.Enabled, "TextBox_Stringa", "TextBox_Stringa_Disabilitato")

        odpFineSospensione.Enabled = abilita AndAlso Not UtenteLoggatoIsMedico
        odpFineSospensione.CssClass = IIf(odpFineSospensione.Enabled, "TextBox_Data", "TextBox_Data_Disabilitato")

        txtFirmaBil.ReadOnly = Not abilita
        txtFirmaBil.CssClass = IIf(abilita, "TextBox_Stringa", "TextBox_Stringa_Disabilitato")

        txtNote.Enabled = abilita

        ' Sezione misure
        lblPeso.Visible = abilita
        txtPeso.Visible = abilita

        lblAltezza.Visible = abilita
        txtAltezza.Visible = abilita

        lblCranio.Visible = abilita
        txtCranio.Visible = abilita

        ' Sezione percentili
        lblPercentilePeso.Visible = abilita
        txtPercentilePeso.Visible = abilita

        lblPercentileAltezza.Visible = abilita
        txtPercentileAltezza.Visible = abilita

        lblPercentileCranio.Visible = abilita
        txtPercentileCranio.Visible = abilita

        ' Dati opzionali (vaccinazioni e viaggio)
        ucDatiOpzionaliBilancio.Enabled = abilita

    End Sub

    Private Sub Pulisci()

        txtData.Text = String.Empty

        txtMalattia.Descrizione = String.Empty
        txtMalattia.Codice = String.Empty

        If Not UtenteLoggatoIsMedico Then
            txtMedico.Descrizione = String.Empty
            txtMedico.Codice = String.Empty
        End If

        fmRilevatore.Codice = String.Empty
        fmRilevatore.Descrizione = String.Empty

        txtNote.Text = String.Empty

        txtNumero.Text = String.Empty
        txtFirmaBil.Text = String.Empty

        ddlVaccinabile.Items.Clear()

        odpFineSospensione.Text = String.Empty

        txtPeso.Text = String.Empty
        txtAltezza.Text = String.Empty
        txtCranio.Text = String.Empty

        txtPercentilePeso.Text = String.Empty
        txtPercentileAltezza.Text = String.Empty
        txtPercentileCranio.Text = String.Empty

        ucDatiOpzionaliBilancio.Clear()

        Questionario.DtSezioni = Nothing
        Questionario.DataBind()

    End Sub

#End Region

#Region " Compilazione bilancio "

    Private Sub GeneraQuestionario(tipoCompilazione As Enumerators.TipoCompilazioneBilancio)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Dim numeroBilancio As Integer = Convert.ToInt32(Me.txtNumero.Text)

            Me.Questionario.CodicePaziente = OnVacUtility.Variabili.PazId
            Me.Questionario.CodiceMalattia = Me.txtMalattia.Codice
            Me.Questionario.DataVisita = Me.txtData.Data

            Me.Questionario.IdVisita = -1

            Me.Questionario.NumeroBilancio = numeroBilancio
            Me.Questionario.Compilazione = tipoCompilazione

            Using bizBilancioSalute As New Biz.BizBilancioProgrammato(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Dim datiBilancioPazienteResult As Biz.BizBilancioProgrammato.DatiBilancioPazienteResult =
                    bizBilancioSalute.GetDatiBilancioPaziente(OnVacUtility.Variabili.PazId, Me.txtNumero.Text, Me.txtMalattia.Codice, Me.DatiPaziente.Sesso, tipoCompilazione, Date.Now)

                Me.Questionario.DtSezioni = datiBilancioPazienteResult.Sezioni
                Me.Questionario.DtCondizioniBilancio = datiBilancioPazienteResult.CondizioniBilancio
                Me.Questionario.DtRispostePossibili = datiBilancioPazienteResult.RispostePossibili
                Me.Questionario.DtDomande = datiBilancioPazienteResult.Domande
                Me.Questionario.DtRisposte = datiBilancioPazienteResult.Risposte

            End Using

            Me.Questionario.DataBind()

        End Using

    End Sub

#End Region

#Region " Redirect "

    Private Sub RedirectToConvocazioni()

        Dim lstBilanci As List(Of BilancioProgrammato)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizBilanci As New Biz.BizBilancioProgrammato(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                lstBilanci = bizBilanci.GetBilanciPaziente(OnVacUtility.Variabili.PazId)
            End Using

        End Using

        If lstBilanci.Count > 0 Then
            Me.RedirectToListaConvocazioni()
        Else
            Me.RedirectToVaccinazioniProgrammate()
        End If

    End Sub

    Private Sub RedirectToListaConvocazioni()

        Dim edit As Boolean? = Nothing

        If Not String.IsNullOrWhiteSpace(Me.EditCnv) Then

            Dim value As Boolean = False
            If Boolean.TryParse(Me.EditCnv, value) Then edit = value

        End If

        Me.RedirectToConvocazioniPaziente(OnVacUtility.Variabili.PazId, edit, True, True, Nothing)

    End Sub

    Private Sub RedirectToVaccinazioniProgrammate()

        Me.Response.Redirect(String.Format("../../HPazienti/VacProg/VacProg.aspx?DataCnv={0}&EditCnv={1}",
                                           HttpUtility.UrlEncode(Me.DataCnv),
                                           HttpUtility.UrlEncode(Me.EditCnv)))

    End Sub
    Private Sub RedirectToVisioneBilanci()

        Me.Response.Redirect(String.Format("../../HBilanci/VisioneBilanci/VisioneBilanci.aspx"))

    End Sub

#End Region

#Region "Ricalcolo vaccinazioni dose per FollowUp"

    Private Function ElencoVaccinazioni(idPaz As Long, dataInizio As Date, dataFine As Date) As String
        Dim ret As String = String.Empty
        Dim listaVacDose As New List(Of VaccinazioneDose)
        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizVacEsec As New Biz.BizVaccinazioniEseguite(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                listaVacDose = bizVacEsec.GetVaccinazioniDosePaziente(OnVacUtility.Variabili.PazId, dataInizio, dataFine)
            End Using
        End Using
        Dim list As New List(Of String)()
        Dim codiceAp As String = String.Empty
        For Each ve As VaccinazioneDose In listaVacDose
            'Se ho una vaccinazione ripetuta prendo solo la prima
            ' che in questo caso è quella con dose massima in quanto la lista è ordinata per dose desc
            If codiceAp <> ve.Codice Then
                list.Add(String.Format("{0}|{1}", ve.Codice, ve.Dose))
                codiceAp = ve.Codice
            End If
        Next
        If Not list.IsNullOrEmpty() Then
            ret = String.Join(";", list.ToArray())

        End If
        Return ret
    End Function
#End Region
    Private Function CheckQuestionario(operazione As OperazioniSalvataggio) As Boolean

        Dim errorMsg As New System.Text.StringBuilder()

        ' Controllo campi obbligatori della visita
        If Me.txtData.Data = DateTime.MinValue Then
            errorMsg.Append("La Data della visita e\' obbligatoria.\n")
        End If

        If String.IsNullOrWhiteSpace(Me.txtMedico.Codice) OrElse String.IsNullOrWhiteSpace(Me.txtMedico.Descrizione) Then
            errorMsg.Append("Il Medico e\' obbligatorio.\n")
        End If

        If String.IsNullOrWhiteSpace(Me.fmRilevatore.Codice) OrElse String.IsNullOrWhiteSpace(Me.fmRilevatore.Descrizione) Then
            errorMsg.Append("L\'Operatore e\' obbligatorio.\n")
        End If

        ' Controllo osservazioni del questionario
        Dim checkResult As QuestionarioBilancio.CheckCampiQuestionarioResult = Me.Questionario.CheckCampiQuestionario()

        If checkResult.ErrorType = QuestionarioBilancio.CheckCampiQuestionarioType.ErroreBloccante Then

            errorMsg.AppendFormat("{0}\n", checkResult.Message)

        End If

        If errorMsg.Length > 0 Then

            Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(String.Format("Impossibile effettuare il salvataggio.\n{0}", errorMsg.ToString()), CONFIRM_ERROR_EMPTY_FIELDS, False, False))
            Return False

        End If

        ' Controllo campi singoli della visita e composizione messaggio per warning
        If Me.txtFirmaBil.Visible AndAlso String.IsNullOrWhiteSpace(Me.txtFirmaBil.Text) Then
            errorMsg.Append("Firma bilancio assente.\n")
        End If

        If Me.ddlVaccinabile.Visible AndAlso Me.ddlVaccinabile.SelectedValue = "N" AndAlso String.IsNullOrWhiteSpace(Me.odpFineSospensione.Text) Then
            errorMsg.Append("Fine sospensione assente.\n")
        End If

        If Me.txtPeso.Visible AndAlso String.IsNullOrWhiteSpace(Me.txtPeso.Text) Then
            errorMsg.Append("Peso assente.\n")
        End If

        If Me.txtAltezza.Visible AndAlso String.IsNullOrWhiteSpace(Me.txtAltezza.Text) Then
            errorMsg.Append("Altezza assente.\n")
        End If

        If Me.txtCranio.Visible AndAlso String.IsNullOrWhiteSpace(Me.txtCranio.Text) Then
            errorMsg.Append("Circonferenza cranica assente.\n")
        End If

        If checkResult.ErrorType = QuestionarioBilancio.CheckCampiQuestionarioType.Warning Then

            errorMsg.AppendFormat("{0}\n", checkResult.Message)

        End If

        If errorMsg.Length > 0 Then

            If Me.UtenteLoggatoIsMedico Then

                ' Bloccante (l'OnitLayoutMsgBox non chiede conferma ma mostra il messaggio e blocca)
                Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(String.Format("Impossibile effettuare il salvataggio.\n{0}", errorMsg.ToString()), CONFIRM_ERROR_EMPTY_FIELDS, False, False))
                Return False

                'Else

                '' Non bloccante (l'OnitLayoutMsgBox chiede conferma all'utente)
                'Dim msgBoxKey As String = String.Empty
                'Dim warningMessage As String = String.Empty

                'Select Case operazione

                '    Case OperazioniSalvataggio.Salva
                '        warningMessage = String.Format("ATTENZIONE!\n{0}\n\nContinuare con il salvataggio?", errorMsg.ToString())
                '        msgBoxKey = CONFIRM_ERROR_EMPTY_FIELDS

                '    Case OperazioniSalvataggio.Salva_Firma
                '        warningMessage = String.Format("ATTENZIONE!\n{0}\n\nContinuare con il salvataggio e la firma digitale del documento?", errorMsg.ToString())
                '        msgBoxKey = CONFIRM_ERROR_EMPTY_FIELDS_FIRMA_DIGITALE

                '    Case OperazioniSalvataggio.Salva_Stampa
                '        warningMessage = String.Format("ATTENZIONE!\n{0}\n\nContinuare con il salvataggio e la stampa dell\'anamnesi?", errorMsg.ToString())
                '        msgBoxKey = CONFIRM_ERROR_EMPTY_FIELDS_STAMPA_BILANCIO

                'End Select

                'Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(warningMessage, msgBoxKey, True, True))

            End If

            'Return False

        End If

        Return True

    End Function

    Private Function GetVisita() As Visita

        Dim visita As New Visita()

        visita.CodicePaziente = OnVacUtility.Variabili.PazId
        visita.DataVisita = txtData.Data
        visita.BilancioNumero = txtNumero.Text
        visita.MalattiaCodice = txtMalattia.Codice
        visita.CodiceConsultorio = OnVacUtility.Variabili.CNS.Codice
        visita.MedicoCodice = txtMedico.Codice
        visita.Firma = txtFirmaBil.Text
        visita.DataFineSospensione = odpFineSospensione.Data

        If Not ddlVaccinabile.SelectedItem Is Nothing Then
            visita.Vaccinabile = ddlVaccinabile.SelectedValue
        Else
            visita.Vaccinabile = String.Empty
        End If

        visita.Altezza = GetDoubleValue(txtAltezza.Text.Replace(".", ","))
        visita.Cranio = GetDoubleValue(txtCranio.Text.Replace(".", ","))
        visita.Peso = GetDoubleValue(txtPeso.Text.Replace(".", ","))

        visita.PercentileAltezza = txtPercentileAltezza.Text
        visita.PercentileCranio = txtPercentileCranio.Text
        visita.PercentilePeso = txtPercentilePeso.Text

        ' Valorizzazione flag visibilità in base a consenso (solo se gestito)
        visita.FlagVisibilitaDatiVaccinaliCentrale = Common.OnVacStoricoVaccinaleCentralizzato.GetValoreVisibilitaDatiVaccinali(chkFlagVisibilita)

        ' Salvataggio rilevatore
        visita.RilevatoreCodice = fmRilevatore.Codice

        If txtNote.Text.Length > txtNote.MaxLength Then
            txtNote.Text = txtNote.Text.Trim().Substring(0, txtNote.MaxLength)
        End If
        visita.Note = txtNote.Text

        Dim datiViaggioCorrente As DatiOpzionaliBilancio.DatiViaggio = ucDatiOpzionaliBilancio.GetDatiViaggio()
        visita.DataInizioViaggio = datiViaggioCorrente.DataInizioViaggio
        visita.DataFineViaggio = datiViaggioCorrente.DataFineViaggio
        visita.PaeseViaggioCodice = datiViaggioCorrente.PaeseViaggioCodice
        visita.PaeseViaggioDescrizione = datiViaggioCorrente.PaeseViaggioDescrizione

        visita.VaccinazioniBilancio = ucDatiOpzionaliBilancio.GetDatiVaccinazioni()

        Return visita

    End Function

    Private Function GetDoubleValue(value As String)

        Dim doubleValue As Double = 0

        If Not Double.TryParse(value, doubleValue) Then
            doubleValue = 0
        End If

        Return doubleValue

    End Function

    Private Sub SetToolBarStatus(pageStatus As PageStatus)

        Dim btnBilancio As Infragistics.WebUI.UltraWebToolbar.TBarButton = Me.ToolBar.Items.FromKeyButton("btnBilancio")
        Dim btnSalva As Infragistics.WebUI.UltraWebToolbar.TBarButton = Me.ToolBar.Items.FromKeyButton("btnSalva")
        Dim btnAnnulla As Infragistics.WebUI.UltraWebToolbar.TBarButton = Me.ToolBar.Items.FromKeyButton("btnAnnulla")
        Dim btnCompilaBilancio As Infragistics.WebUI.UltraWebToolbar.TBarButton = Me.ToolBar.Items.FromKeyButton("btnCompilaBilancio")
        Dim btnRecuperaStoricoVacc As Infragistics.WebUI.UltraWebToolbar.TBarButton = Me.ToolBar.Items.FromKeyButton("btnRecuperaStoricoVacc")
        Dim btnSalvaFirma As Infragistics.WebUI.UltraWebToolbar.TBarButton = Me.ToolBar.Items.FromKeyButton("btnSalvaFirma")
        Dim btnSalvaStampa As Infragistics.WebUI.UltraWebToolbar.TBarButton = Me.ToolBar.Items.FromKeyButton("btnSalvaStampa")

        btnSalvaFirma.Visible = Me.Settings.FIRMADIGITALE_ANAMNESI_ON

        Select Case pageStatus

            Case GestioneBilancio.PageStatus.Load

                btnBilancio.Enabled = True
                btnSalva.Enabled = False
                btnAnnulla.Enabled = False
                btnCompilaBilancio.Enabled = False
                btnRecuperaStoricoVacc.Enabled = False
                btnSalvaFirma.Enabled = False
                btnSalvaStampa.Enabled = False

            Case GestioneBilancio.PageStatus.Edit

                btnBilancio.Enabled = False
                btnSalva.Enabled = True
                btnAnnulla.Enabled = True

                If Not Questionario.DtSezioni Is Nothing AndAlso Questionario.DtSezioni.Rows.Count > 0 Then
                    btnCompilaBilancio.Enabled = True
                Else
                    btnCompilaBilancio.Enabled = False
                End If

                btnRecuperaStoricoVacc.Enabled = False
                btnSalvaFirma.Enabled = True
                btnSalvaStampa.Enabled = True

            Case GestioneBilancio.PageStatus.StoricoDaRecuperare

                btnBilancio.Enabled = False
                btnSalva.Enabled = False
                btnAnnulla.Enabled = False
                btnCompilaBilancio.Enabled = False
                btnRecuperaStoricoVacc.Enabled = True
                btnSalvaFirma.Enabled = False
                btnSalvaStampa.Enabled = False

        End Select

    End Sub

    Private Sub InizializzaPagina()

        'disabilita tutti i campi visualizzati
        AbilitaCampi(False)

        ' Nasconde i campi di dettaglio e opzionali
        ImpostaBilancioSelezionato(Nothing)

        StoricoVaccinaleCentralizzatoDaRecuperare = False

    End Sub

    Private Sub ImpostaFlagVisibilita(abilita As Boolean)

        If abilita Then
            chkFlagVisibilita.Checked = IsVisibilitaCentraleDatiVaccinaliPaziente
            chkFlagVisibilita.Enabled = True
        Else
            chkFlagVisibilita.Enabled = False
        End If

    End Sub

    Private Sub RecuperaStoricoVaccinale()

        Dim command As New Common.OnVacStoricoVaccinaleCentralizzato.AcquisisciStoricoCommand()
        command.CodicePaziente = OnVacUtility.Variabili.PazId
        command.RichiediConfermaSovrascrittura = False
        command.Settings = Me.Settings
        command.OnitLayout3 = Me.OnitLayout31
        command.BizLogOptions = OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.GEST_BIL)
        command.Note = "Recupero Storico Vaccinale da maschera GestioneBilancio"

        Dim acquisisciDatiVaccinaliCentraliResult As Biz.BizPaziente.AcquisisciDatiVaccinaliCentraliResult =
            Common.OnVacStoricoVaccinaleCentralizzato.AcquisisciDatiVaccinaliCentraliPaziente(command)

        If acquisisciDatiVaccinaliCentraliResult.StatoAcquisizioneDatiVaccinaliCentrale <> Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Parziale Then

            ' Se il recupero è andato a buon fine
            Me.StoricoVaccinaleCentralizzatoDaRecuperare = False

            Me.SetToolBarStatus(PageStatus.Load)

        Else

            Me.SetToolBarStatus(PageStatus.StoricoDaRecuperare)

        End If

        InizializzaPagina()

    End Sub

    Private Function GetBooleanValueFromQueryString(parameterName As String) As Boolean

        Dim value As Boolean = False

        If Not String.IsNullOrWhiteSpace(Me.Request.QueryString.Get(parameterName)) Then

            If Not Boolean.TryParse(Me.Request.QueryString(parameterName), value) Then
                value = False
            End If

        End If

        Return value

    End Function

    Private Sub ShowModaleSceltaBilancio()

        Me.uscRicBil.ModaleName = "modRicBil"
        Me.uscRicBil.LoadModale()
        Me.modRicBil.VisibileMD = True

    End Sub

#End Region

End Class
