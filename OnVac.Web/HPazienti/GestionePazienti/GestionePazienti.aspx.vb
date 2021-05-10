Imports System.Text
Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager
Imports Onit.Controls

Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Biz

Partial Class GestionePazienti
    Inherits Common.PageBase

#Region " Constants "

    Private Const CONFIRM_SALVATAGGIO_PAZIENTE_DUPLICATO As String = "CSPD"
    Private Const OPEN_RILEVAZIONE_CONSENSO As String = "OPEN_CONS"
    Private Const RILEVAZIONE_AUTOMATICA_CONSENSI As String = "RAC"

    Private Const CSSCLASS_TEXTBOX_STRINGA = "textbox_stringa"
    Private Const CSSCLASS_TEXTBOX_STRINGA_OBBLIGATORIO = "textbox_stringa_obbligatorio"

#End Region

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Enums "

    Public Enum StatoPagina
        VIEW = 0
        EDIT = 1
        LOCK = 2
        BLOCCO_CONSENSO = 3
    End Enum

#End Region

#Region " Variables "

    Public GestioneManuale As String
    Public CategoriaRischio As String
    Public strScript As String

#End Region

#Region " Proprietà "

#Region " OnitDataPanel "

    ' Proprietà utilizzate dal file di configurazione dell'onitdatapanel.

    Public ReadOnly Property ottieniDam() As Onit.Database.DataAccessManager.IDAM
        Get
            Return OnVacUtility.OpenDam()
        End Get
    End Property

    ' N.B. : Questa proprietà è utilizzata dal file xml del pannello, ma non serve valorizzarla perchè la ricerca in centrale 
    '        è gestita dalla classe PazienteHL7OdpAdapter e non più tramite il servizio esterno Paziente_HL7.
    Public ReadOnly Property ottieniServizio() As String
        Get
            Return String.Empty
        End Get
    End Property

#End Region

    ' N.B. : non può essere una variabile privata perchè non viene utilizzata solo in un unico flusso di elaborazione
    '        ma anche dopo il postback della richiesta di password per la modifica di campi fondamentali. In quel caso è nothing.
    ' Array contenente i dati originali del paziente (precedenti alle variazioni, se l'operazione è una modifica)
    ' Utilizzato per l'invio messaggi e per il log. Viene valorizzato prima del salvataggio e prima dell'allineamento centrale-locale.
    Private Property itemArrayDatiPazienteOriginal() As Object()
        Get
            Return ViewState("itemArrayDatiPazienteOriginal")
        End Get
        Set(value As Object())
            ViewState("itemArrayDatiPazienteOriginal") = value
        End Set
    End Property

    Private Property StatoCorrentePagina() As StatoPagina
        Get
            Return ViewState("StatoPagina")
        End Get
        Set(Value As StatoPagina)
            ViewState("StatoPagina") = Value
        End Set
    End Property

    Private Property blnSceltoLocale() As Boolean
        Get
            Return ViewState("blnSceltoLocale")
        End Get
        Set(Value As Boolean)
            ViewState("blnSceltoLocale") = Value
        End Set
    End Property

    Private Property ControlloSedeVaccinale() As Boolean
        Get
            Return Session("GestionePazienti_ControlloSedeVaccinale")
        End Get
        Set(Value As Boolean)
            Session("GestionePazienti_ControlloSedeVaccinale") = Value
        End Set
    End Property

    Public Property LoadLeftFrame() As Boolean
        Get
            Return ViewState("OnVac_LoadLeftFrame")
        End Get
        Set(Value As Boolean)
            ViewState("OnVac_LoadLeftFrame") = Value
        End Set
    End Property

    Public Property RefreshLeftFrame() As Boolean
        Get
            If Session("RefreshLeftFrame") Is Nothing Then Session("RefreshLeftFrame") = False
            Return Convert.ToBoolean(Session("RefreshLeftFrame"))
        End Get
        Set(Value As Boolean)
            Session("RefreshLeftFrame") = Value
        End Set
    End Property

    Public Property ModificaCircoscrizioneResidenza() As Boolean
        Get
            Return Session("ModificaCircoscrizioneResidenza")
        End Get
        Set(Value As Boolean)
            Session("ModificaCircoscrizioneResidenza") = Value
        End Set
    End Property

    Public Property ModificaCircoscrizioneDomicilio() As Boolean
        Get
            Return Session("ModificaCircoscrizioneDomicilio")
        End Get
        Set(Value As Boolean)
            Session("ModificaCircoscrizioneDomicilio") = Value
        End Set
    End Property

    Public Property msgWarning() As String
        Get
            Return Session("OnVac_warningMessage")
        End Get
        Set(Value As String)
            Session("OnVac_warningMessage") = Value
        End Set
    End Property

    'contiene i valori della sede vaccinale impostata automaticamente (modifica 10/01/2004)
    Public Property SedeVaccAuto() As String()
        Get
            Return Session("OnVac_SedeVaccAuto")
        End Get
        Set(Value As String())
            Session("OnVac_SedeVaccAuto") = Value
        End Set
    End Property

    Public Property NuovoStatoAnagrafico() As String
        Get
            Return Session("OnVac_NSA")
        End Get
        Set(Value As String)
            Session("OnVac_NSA") = Value
        End Set
    End Property

    ' Determina se può essere calcolato il codice fiscale, in base allo stato del campo (se abilitato oppure no) e al parametro AUTO_CF
    Protected ReadOnly Property CanCalculateCodiceFiscale() As Boolean
        Get
            Return Me.txtCodiceFiscale.BindingField.Editable <> OnitDataPanel.BindingFieldValue.editPositions.never AndAlso Me.Settings.AUTO_CF
        End Get
    End Property

    'per contraddistinguere l'indirizzo di domicilio da quello di residenza [modifica 31/08/2005]
    Private Property SalvaTipoIndirizzo() As Enumerators.TipoIndirizzo
        Get
            Return Session("salvaTipoIndirizzo")
        End Get
        Set(Value As Enumerators.TipoIndirizzo)
            Session("salvaTipoIndirizzo") = Value
        End Set
    End Property

#Region " Proprietà Consultorio "

    ' Codice del consultorio che è stato caricato inizialmente 
    ' per vedere se c'è stato un movimento da registrare nella t_cns_movimenti
    Public Property CodConsVaccinale() As String
        Get
            Return IIf(Session("CodConsVaccinale") Is Nothing, "", Session("CodConsVaccinale"))
        End Get
        Set(Value As String)
            Session("CodConsVaccinale") = Value
        End Set
    End Property

    ' Data assegnazione del consultorio che è stato caricato inizialmente, per ripristinare quella corretta
    Private Property DataAssegnazioneConsVaccinale() As String
        Get
            Return IIf(Session("DataAssegnazioneConsVaccinale") Is Nothing, "", Session("DataAssegnazioneConsVaccinale"))
        End Get
        Set(Value As String)
            Session("DataAssegnazioneConsVaccinale") = Value
        End Set
    End Property

    ' Descrizione del consultorio vaccinale corrente, prima della modifica.
    ' Se la modifica non va a buon fine viene ripristinato
    Private Property DescrConsVaccinale() As String
        Get
            Return IIf(Session("DescrConsVaccinale") Is Nothing, "", Session("DescrConsVaccinale"))
        End Get
        Set(Value As String)
            Session("DescrConsVaccinale") = Value
        End Set
    End Property

    ' Indirizzo del consultorio vaccinale corrente, prima della modifica.
    ' Se la modifica non va a buon fine viene ripristinato
    Private Property IndirConsVaccinale() As String
        Get
            Return IIf(Session("IndirConsVaccinale") Is Nothing, "", Session("IndirConsVaccinale"))
        End Get
        Set(Value As String)
            Session("IndirConsVaccinale") = Value
        End Set
    End Property

    ' Descrizione del consultorio vaccinale precedente, prima della modifica.
    ' Se la modifica non va a buon fine viene ripristinato.
    Private Property DescrConsVaccinalePrec() As String
        Get
            Return IIf(Session("DescrConsVaccinalePrec") Is Nothing, "", Session("DescrConsVaccinalePrec"))
        End Get
        Set(Value As String)
            Session("DescrConsVaccinalePrec") = Value
        End Set
    End Property

    ' Codice del consultorio vaccinale precedente, prima della modifica.
    ' Se la modifica non va a buon fine viene ripristinato.
    Private Property CodConsVaccinalePrec() As String
        Get
            Return IIf(Session("CodConsVaccinalePrec") Is Nothing, "", Session("CodConsVaccinalePrec"))
        End Get
        Set(Value As String)
            Session("CodConsVaccinalePrec") = Value
        End Set
    End Property

#End Region

#Region " Proprietà per gestione messaggi esterni "

    ' Proprietà utilizzate per mantenere i valori originali e correnti dei dati del pannello, utilizzati
    ' per la gestione dei messaggi esterni. Queste proprietà, compreso il flag isPazienteNew, sono valorizzate 
    ' prima del salvataggio perchè altrimenti i valori originali sarebbero persi. Devono essere mantenute in 
    ' sessione o nel viewstate perchè le operazioni in cui sono utilizzate possono essere richiamate o direttamente
    ' al salvataggio o ad un postback successivo, se viene richiesta la password. In questo caso non sarebbero valorizzate.

    ' Flag che indica se il paziente è stato appena inserito oppure si sta effettuando una modifica.
    Private Property IsPazienteNew() As Boolean
        Get
            Return ViewState("OnVac_isPazNew")
        End Get
        Set(Value As Boolean)
            ViewState("OnVac_isPazNew") = Value
        End Set
    End Property

    ' Valore originale dell'indirizzo di residenza (precedente alle variazioni, se l'operazione è una modifica)
    Private Property IndResidenzaOriginal() As String
        Get
            Return ViewState("OnVac_ResidenzaOriginal")
        End Get
        Set(Value As String)
            ViewState("OnVac_ResidenzaOriginal") = Value
        End Set
    End Property

    ' Valore originale dell'indirizzo di domicilio (precedente alle variazioni, se l'operazione è una modifica)
    Private Property IndDomicilioOriginal() As String
        Get
            Return ViewState("OnVac_DomicilioOriginal")
        End Get
        Set(Value As String)
            ViewState("OnVac_DomicilioOriginal") = Value
        End Set
    End Property

#End Region

    ''' <summary>
    ''' Impostato a true se sono stati riscontrati errori bloccanti. La maschera verrà disabilitata.
    ''' </summary>
    Private Property DisabilitaLayout() As Boolean
        Get
            Return IIf(ViewState("OnVac_DisabilitaLayout") Is Nothing, False, ViewState("OnVac_DisabilitaLayout"))
        End Get
        Set(Value As Boolean)
            ViewState("OnVac_DisabilitaLayout") = Value
        End Set
    End Property

    ' Restituisce una stringa contenente i codici degli stati anagrafici per cui è prevista 
    ' la cancellazione della programmazione vaccinale, separati da |
    Protected Property StatiAnagraficiCancellazioneProgrammazione() As String
        Get
            If ViewState("StatiAnagCancProg") Is Nothing Then Return String.Empty
            Return ViewState("StatiAnagCancProg")
        End Get
        Private Set(value As String)
            ViewState("StatiAnagCancProg") = value
        End Set
    End Property

#End Region

#Region " Formattazione lunghezza testo "

    Private Sub SetControls()

        SetControlLength(txtCognome, Settings.LEN_COGNOME)
        SetControlLength(viaDomicilio.txtIndirizzo, Settings.LEN_INDDOM)
        SetControlLength(viaResidenza.txtIndirizzo, Settings.LEN_INDRES)
        'SetControlLength(txtAnnotazioni, Settings.LEN_NOTE)
        SetControlLength(txtNome, Settings.LEN_NOME)
        SetControlLength(txtTelefono1, Settings.LEN_TEL1)
        SetControlLength(txtTelefono2, Settings.LEN_TEL2)
        SetControlLength(txtTelefono3, Settings.LEN_TEL3)
        SetControlLength(txtTesseraSanitaria, Settings.LEN_TESSERA)

        RicPazUtility.impostaRangeDate(txtDataNascita, 120)
        RicPazUtility.impostaRangeDate(txtDataDecesso, 300)

    End Sub

    Private Sub SetControlLength(ctl As System.Web.UI.Control, length As Integer)

        If length <> -1 Then

            If Not ctl.GetType().GetProperty("MaxLength") Is Nothing Then
                CType(ctl, Object).MaxLength = length
            End If

        End If

    End Sub

    Private Function CheckControlsLength() As List(Of String)

        Dim lst As New List(Of String)()

        CheckControlLength(txtCognome, Settings.LEN_COGNOME, lst)
        CheckControlLength(viaDomicilio.txtIndirizzo, Settings.LEN_INDDOM, lst)
        CheckControlLength(viaResidenza.txtIndirizzo, Settings.LEN_INDRES, lst)
        'CheckControlLength(txtAnnotazioni, Settings.LEN_NOTE, lst)
        CheckControlLength(txtNome, Settings.LEN_NOME, lst)
        CheckControlLength(txtTelefono1, Settings.LEN_TEL1, lst)
        CheckControlLength(txtTelefono2, Settings.LEN_TEL2, lst)
        CheckControlLength(txtTelefono3, Settings.LEN_TEL3, lst)
        CheckControlLength(txtTesseraSanitaria, Settings.LEN_TESSERA, lst)

        Return lst

    End Function

    Private Sub CheckControlLength(ctl As System.Web.UI.Control, length As Integer, lst As List(Of String))

        If length <> -1 Then

            If Not ctl.GetType().GetProperty("Text") Is Nothing Then

                Dim text As String = CType(ctl, Object).Text

                If Not String.IsNullOrEmpty(text) Then

                    If text.Length > length Then
                        lst.Add(String.Format("Lunghezza del campo {0} non supportata.", RecuperaLabelAssociata(ctl)))
                    End If

                End If

            End If

        End If

    End Sub

    Private Sub txtCategorieRischio_SetUpFiletr(sender As Object) Handles txtCategorieRischio.SetUpFiletr

        txtCategorieRischio.Filtro = "1=1"

        If Not Settings.GESTPAZ_CAT_RISCHIO_OBSOLETE Then
            txtCategorieRischio.Filtro &= " AND (RSC_OBSOLETO is null OR RSC_OBSOLETO <> 'S') "
        End If

        'se filtro macrocategorie valorizzato, aggiungere filtro a categorie rischio
        If Not String.IsNullOrWhiteSpace(ddlMacrocategorieRischio.SelectedValue) Then
            txtCategorieRischio.Filtro &= String.Format(" AND RSC_MCR_CODICE = '{0}' ", ddlMacrocategorieRischio.SelectedValue)
        End If

        If txtCategorieRischio.Filtro.Contains("AND") Then
            txtCategorieRischio.Filtro &= " ORDER BY Descrizione "
        End If

    End Sub

#End Region

#Region " Gestione Datapanel "

    Private Sub ImpostaCentrale(readAu As OnitDataPanel.Connection.readAuthorizations, writeAu As OnitDataPanel.Connection.writeAuthorizations)

        ' --- Impostazione diritti lettura/scrittura anagrafe centrale --- '
        Me.odpDettaglioPaziente.Connections(Constants.ConnessioneAnagrafe.CENTRALE).ReadAuth = readAu
        Me.odpDettaglioPaziente.Connections(Constants.ConnessioneAnagrafe.CENTRALE).WriteAuth = writeAu

        ' --- Impostazione diritti lettura alle connessioni utilizzate per le descrizioni --- '
        ' Connessioni sempre presenti (non è necessario controllarne la presenza)
        SetReadNoWriteAuthorizations(Constants.ConnessioneAnagrafe.LOCALE_CNAS, readAu, False)
        SetReadNoWriteAuthorizations(Constants.ConnessioneAnagrafe.LOCALE_CITT, readAu, False)
        SetReadNoWriteAuthorizations(Constants.ConnessioneAnagrafe.LOCALE_COMRE, readAu, False)
        SetReadNoWriteAuthorizations(Constants.ConnessioneAnagrafe.LOCALE_COMDO, readAu, False)
        SetReadNoWriteAuthorizations(Constants.ConnessioneAnagrafe.LOCALE_AUSL, readAu, False)
        SetReadNoWriteAuthorizations(Constants.ConnessioneAnagrafe.LOCALE_COMDOSAN, readAu, False)

        ' Connessioni opzionali (necessario controllo esistenza)
        SetReadNoWriteAuthorizations(Constants.ConnessioneAnagrafe.LOCALE_MED, readAu, True)
        SetReadNoWriteAuthorizations(Constants.ConnessioneAnagrafe.LOCALE_AUSL_ASS, readAu, True)
        SetReadNoWriteAuthorizations(Constants.ConnessioneAnagrafe.LOCALE_AUSL_ASS_PRECEDENTE, readAu, True)
        SetReadNoWriteAuthorizations(Constants.ConnessioneAnagrafe.LOCALE_CIR, readAu, True)
        SetReadNoWriteAuthorizations(Constants.ConnessioneAnagrafe.LOCALE_CIR_2, readAu, True)
        SetReadNoWriteAuthorizations(Constants.ConnessioneAnagrafe.LOCALE_DIS, readAu, True)
        SetReadNoWriteAuthorizations(Constants.ConnessioneAnagrafe.LOCALE_RIS, readAu, True)

    End Sub

    ''' <summary>
    ''' Imposta le autorizzazioni di lettura/scrittura per la connessione specificata.
    ''' La lettura è in base al parametro, la scrittura è sempre inibita.
    ''' Se il parametro checkConnection è true, prima di impostare le autorizzazioni controlla che la connessione esista.
    ''' </summary>
    Private Sub SetReadNoWriteAuthorizations(connectionName As String, readAuth As OnitDataPanel.Connection.readAuthorizations, checkConnection As Boolean)

        If checkConnection Then
            ' Termina se la connessione non esiste
            If Me.odpDettaglioPaziente.Connections(connectionName) Is Nothing Then Exit Sub
        End If

        ' Imposta le autorizzazioni di lettura/scrittura
        Me.odpDettaglioPaziente.Connections(connectionName).ReadAuth = readAuth
        Me.odpDettaglioPaziente.Connections(connectionName).WriteAuth = OnitDataPanel.Connection.writeAuthorizations.none

    End Sub

#End Region

#Region " Al salvataggio "

    Private Function GetPazientePrecedente() As Paziente

        Dim pazientePrecedente As Paziente = Nothing

        If Not IsPazienteNew Then

            Using dbGenericProvider As New DbGenericProvider(OnVacContext.Connection)
                Using bizPaziente As New BizPaziente(dbGenericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    pazientePrecedente = bizPaziente.CercaPaziente(OnVacUtility.Variabili.PazId)

                End Using
            End Using
        End If

        Return pazientePrecedente

    End Function

    Private Function ControllaComuneNascita() As Boolean

        Dim encoder As OnitDataPanel.FieldsEncoder = odpDettaglioPaziente.GetCurrentTableEncoder()
        Dim currentRow As DataRow = odpDettaglioPaziente.getCurrentDataRow()

        Dim dataNascita As Object = encoder.getValOf(currentRow, Constants.ConnessioneAnagrafe.CENTRALE, "PAZ_DATA_NASCITA", "t_paz_pazienti_centrale", True)

        If dataNascita Is Nothing OrElse dataNascita Is DBNull.Value Then
            Return True
        End If

        Dim codComune As String = encoder.getValOf(currentRow, Constants.ConnessioneAnagrafe.CENTRALE, "PAZ_COM_CODICE_NASCITA", "t_paz_pazienti_centrale", True).ToString()

        Return codComune = String.Empty

    End Function

#End Region

#Region " Eventi Page "

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If Not IsPostBack() Then
            RefreshLeftFrame = False

            Dim infodet As OnitDataPanel.infoDetailStruct = odpDettaglioPaziente.getReceivedDetailInfo()

            SetStatiAnagraficiCancellazioneProgrammazione()

            blnSceltoLocale = False

            If Not infodet Is Nothing Then
                If infodet.filters.Count > 0 AndAlso (infodet.filters(0).TableName Is Nothing OrElse infodet.filters(0).TableName.ToLower() <> "t_paz_pazienti_centrale") Then
                    blnSceltoLocale = True
                End If
            End If

            SetControls()

            txtControlloConfermaSalva.Text = "True"
            ControlloSedeVaccinale = False

            ' Flag modifica circoscrizione in caso di gestione vie codificate
            ModificaCircoscrizioneResidenza = False
            ModificaCircoscrizioneDomicilio = False

            ' Ricarico in console il messaggio precedente
            GestionePazientiMessage.Console.StatusMessage(msgWarning)
            msgWarning = String.Empty

            Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

                'caricamento della combo con lo stato anagrafico [modifica 31/03/2005]
                CaricaStatoAnagrafico(genericProvider)
                CaricaStatoAnagraficoDettagliato(genericProvider)
                CaricaTipoOccasionalita(genericProvider)
                LoadMacrocategorieRischio(genericProvider)

            End Using

            '--- INIZIALIZZAZIONE ---
            '- caricamento dei dati nel pannello
            If Not infodet Is Nothing AndAlso infodet.isValid AndAlso (Request.QueryString("fromRedirect") Is Nothing) Then

                odpDettaglioPaziente.LoadData()
                odpDettaglioPaziente.execReceivedDetailOperation(True)

                ' Controllo dati paziente e visualizzazione messaggi di errore/warning
                CheckDatiPazienteCorrente()

                ' Inizializzazioni in caso di inserimento di un paziente in anagrafe locale
                If odpDettaglioPaziente.CurrentOperation = OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord AndAlso
                   Settings.TIPOANAG = Onit.OnAssistnet.OnVac.Enumerators.TipoAnags.SoloLocale Then

                    ' Spunto il checkbox Paziente Locale
                    chkLocale.Checked = True

                    Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

                        ' Ricerca del default nella t_ana_stati_anagrafici
                        Dim statoAnagLocale As String = String.Empty

                        Using bizStatiAnagrafici As New BizStatiAnagrafici(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                            statoAnagLocale = bizStatiAnagrafici.GetStatoAnagraficoDefault()
                        End Using

                        ' Impostazione stato anagrafico di default nella maschera
                        If statoAnagLocale <> String.Empty Then cmbStatoAnagrafico.SelectedValue = statoAnagLocale

                        ' Impostazione automatica del consultorio vaccinale (= cns corrente)
                        If Settings.AUTOSETCNS_INSLOCALE Then

                            If Settings.CHK_ETA_CONSULT Then

                                ' Controllo età paziente per associare al consultorio vaccinale il cns corrente
                                Using bizCns As New BizConsultori(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                                    If bizCns.CheckEtaPazienteInConsultorio(OnVacUtility.Variabili.CNS.Codice, txtDataNascita.Data) Then
                                        SetCnsVaccinale(OnVacUtility.Variabili.CNS.Codice, OnVacUtility.Variabili.CNS.Descrizione, True)
                                    End If

                                End Using

                            Else
                                ' Impostazione del cns vaccinale uguale al cns corrente senza controllo sull'età del paziente
                                SetCnsVaccinale(OnVacUtility.Variabili.CNS.Codice, OnVacUtility.Variabili.CNS.Descrizione, True)
                            End If

                        End If

                    End Using

                End If  ' end if (CurrentOperation = Newrecord and TipoAnag = SoloLocale)

            Else

                If OnVacUtility.IsPazIdEmpty() Then

                    'nel caso in cui si tenti di filtrare per un paziente con codice negativo si rimanda alla ricerca
                    If Not infodet Is Nothing AndAlso
                       infodet.operation = OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord AndAlso
                       infodet.senderPage = "RicercaPaziente.aspx" Then

                        Server.Transfer("RicercaPaziente.aspx")

                    Else

                        Server.Transfer("ricercaPazienteBis.aspx")

                    End If

                End If

                LoadDataPanelPazCodice(OnVacUtility.Variabili.PazId)

                ' Controllo dati paziente e visualizzazione messaggi di errore/warning
                CheckDatiPazienteCorrente()

            End If

            txtSedeVaccinale.Obbligatorio = True

            CodConsVaccinale = String.Empty
            DescrConsVaccinale = String.Empty
            CodConsVaccinalePrec = String.Empty
            DescrConsVaccinalePrec = String.Empty
            IndirConsVaccinale = String.Empty

            '- recupero il codice del paziente, la data di nascita e il codice del consultorio dal pannello
            Dim giorniEtaPaz As Integer = 0

            Dim drRow As DataRow = odpDettaglioPaziente.getCurrentDataRow()
            Dim enc As OnitDataPanel.FieldsEncoder = odpDettaglioPaziente.GetCurrentTableEncoder()

            If Not drRow Is Nothing Then

                Dim objPazCodice As Object = enc.getValOf(drRow, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_CODICE", "T_PAZ_PAZIENTI", True)
                If objPazCodice Is DBNull.Value Then
                    OnVacUtility.ClearPazId()
                Else
                    OnVacUtility.Variabili.PazId = objPazCodice
                    UltimoPazienteSelezionato = New UltimoPazienteSelezionato(String.Empty, OnVacUtility.Variabili.PazId) ' TODO [GestionePazienti]: controllare se paz_codice_ausiliario
                End If

                ' Leggo il valore iniziale del consultorio vaccinale
                Dim objCons As Object = enc.getValOf(drRow, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_CNS_CODICE", "T_PAZ_PAZIENTI", True)
                If objCons IsNot DBNull.Value Then
                    CodConsVaccinale = objCons.ToString()
                End If

                ' Leggo i valori iniziali anche per data assegnazione e sede vaccinale precedente e corrente
                Dim objTemp As Object = enc.getValOf(drRow, Constants.ConnessioneAnagrafe.LOCALE, "CNS_DESCRIZIONE", "T_ANA_CONSULTORI", True)
                If objTemp IsNot DBNull.Value Then
                    DescrConsVaccinale = objTemp.ToString()
                End If

                objTemp = enc.getValOf(drRow, Constants.ConnessioneAnagrafe.LOCALE, "CNS_INDIRIZZO", "T_ANA_CONSULTORI", True)
                If objTemp IsNot DBNull.Value Then
                    IndirConsVaccinale = objTemp.ToString()
                End If

                objTemp = enc.getValOf(drRow, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_CNS_DATA_ASSEGNAZIONE", "T_PAZ_PAZIENTI", True)
                If objTemp IsNot DBNull.Value Then
                    DataAssegnazioneConsVaccinale = String.Format("{0:dd/MM/yyyy}", objTemp)
                End If

                objTemp = enc.getValOf(drRow, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_CNS_CODICE_OLD", "T_PAZ_PAZIENTI", True)
                If Not IsDBNull(objTemp) Then
                    CodConsVaccinalePrec = objTemp.ToString()
                End If

                objTemp = enc.getValOf(drRow, Constants.ConnessioneAnagrafe.LOCALE, "CNS_DESCRIZIONE", "T_ANA_CONS_OLD", True)
                If objTemp IsNot DBNull.Value Then
                    DescrConsVaccinalePrec = objTemp.ToString()
                End If

                ' Leggo la data di nascita per filtrare i consultori per età del paziente
                If txtDataNascita.Text <> "" Then
                    giorniEtaPaz = Common.Utility.PazienteHelper.CalcoloEta(txtDataNascita.Data).GiorniTotali
                End If

                Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                    SetFlagPazLocale(genericProvider, drRow)
                End Using

            End If

            ' Filtro nella modale della sede vaccinale corrente in base all'età 
            txtSedeVaccinale.Filtro = "cns_data_apertura <= SYSDATE"
            txtSedeVaccinale.Filtro += " AND (cns_data_chiusura > SYSDATE OR cns_data_chiusura IS NULL)"
            txtSedeVaccinale.Filtro += " AND (cns_da_eta <= " + giorniEtaPaz.ToString()
            txtSedeVaccinale.Filtro += " AND cns_a_eta >= " + giorniEtaPaz.ToString() + ") "
            txtSedeVaccinale.Filtro += "ORDER BY Descrizione"

            ' --- Caricamento altri dati --- '
            ' Caricamento dati non caricati dal pannello e valorizzazione controlli
            Me.BindControlsFromPanel()

            Using DAM As IDAM = OnVacUtility.OpenDam()

                ReloadAll(DAM)

                ' Carico le malattie dal centrale
                Dim letturaCentrale As Boolean =
                    (odpDettaglioPaziente.Connections(Constants.ConnessioneAnagrafe.CENTRALE).ReadAuth = OnitDataPanel.Connection.readAuthorizations.read)

                If letturaCentrale AndAlso Me.Settings.TIPOANAG_MALATTIE = Enumerators.TipoAnags.CentraleLettScritt Then

                    If OnVacUtility.IsPazIdEmpty() Then

                        Dim codiceAusiliarioPaziente As String = Me.GetCodiceAusiliario(drRow)

                        If Not String.IsNullOrEmpty(codiceAusiliarioPaziente) Then

                            Using bizPazienteCentrale As New BizPazienteCentrale(OnVacContext.CreateBizContextInfos(), Nothing)
                                Dim dtaMalattie As dsMalattie.MalattieDataTable = bizPazienteCentrale.CaricaMalattie(codiceAusiliarioPaziente)
                                gestionePazientiDatiSanitari.AddMalattie(dtaMalattie)
                                gestionePazientiDatiSanitari.ReloadMalattie()
                            End Using

                        End If

                    End If

                End If

                ' Aggiungo al controllo message le stringhe costanti
                GestionePazientiMessage.DescrizionePaziente = PageTitle(False)
                GestionePazientiMessage.CentroVaccinaleCorrente = OnVacUtility.Variabili.CNS.Descrizione
                GestionePazientiMessage.CentroVaccinalePaziente = txtSedeVaccinale.Descrizione

                '-aggiunta del consultorio di lavoro (modifica 16/08/2004)
                OnVacUtility.ImpostaCnsLavoro(OnitLayout21)

                ' --- Gestione indirizzi --- '
                ' Caricamento dati per quanto riguarda la gestione codificata degli indirizzi (se impostata).
                GestioneIndirizzi(DAM, drRow)

            End Using

            ' --- Allineamento del db locale con il db centrale --- '
            ' (solo se l'anagrafe non è locale e il parametro AUTOALLINEA è impostato a true)
            If Not infodet Is Nothing AndAlso
               Not OnVacUtility.IsPazIdEmpty() AndAlso
               Settings.TIPOANAG <> Enumerators.TipoAnags.SoloLocale AndAlso
               Settings.AUTOALLINEA Then

                AllineamentoCentraleLocale()

            End If

            ' --- Controllo campi (obbligatori, warning e sede vaccinale) --- '
            ' Se ci sono campi obbligatori non impostati, la left viene nascosta.
            ' Se la sede vaccinale non è valorizzata, prova ad impostarla.
            ControlloCampi(True)

            If Not Request.QueryString("annullato") Is Nothing AndAlso Request.QueryString("annullato").ToString() = "true" Then
                ' Redirect dovuto ad un annulla
                GestionePazientiMessage.Console.StatusMessage("Salvataggio annullato dall'utente.", WarningMessage.MessageType.ErrorMessage)
                ControllaDecesso()
            End If

            If Me.odpDettaglioPaziente.CurrentOperation = OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord Then

                RegistraDatiConsultorio()
                StatoCorrentePagina = StatoPagina.EDIT

                ' Gestione Consenso
                ' N. B. : per un nuovo inserimento, non controlla lo stato del consenso quindi non devo impostare left e stato pagina in base al consenso
                ManageLayoutConsenso(True)

            Else

                odpDettaglioPaziente.CancelData()

                Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                    SetFlagPazLocale(genericProvider, drRow)
                End Using

                StatoCorrentePagina = StatoPagina.VIEW

                ' Gestione Consenso
                ' Se il consenso è BLOCCANTE => imposta left e stato pagina come calcolati dal metodo
                ' Se il consenso è NON BLOCCANTE => lascia left e stato pagina come precedentemente impostati
                Dim result As ManageConsensoResult = ManageLayoutConsenso(False)
                If result.IsConsensoBloccante.HasValue AndAlso result.IsConsensoBloccante.Value Then
                    LoadLeftFrame = result.LoadLeft
                    StatoCorrentePagina = result.StatoPagina
                End If

            End If

            ' Mostra/nasconde i pulsanti di stampa della toolbar
            ShowPrintButtons()

        End If 'fine not ispostback

        If IsPostBack Then
            BindControls()
        End If

        'recupero (se presenti) dei cicli eliminati
        gestionePazientiDatiSanitari.RefreshMessaggiCicliEliminati()

        ' Il pezzo di seguito esegue SEMPRE il controllo
        ' sui campi obbligatori, che viene effettuato anche sopra, dentro
        ' il not IsPostback. Sotto bisognerebbe aggiungere l'andalso.
        ' Non ho il coraggio di fare questa modifica perchè ho paura che
        ' crolli tutto il castello di carte.

        '- aggiunta di eventi lato client per l'uppercase
        txtCognome.Attributes.Add("onblur", "toUpper(this)")
        txtNome.Attributes.Add("onblur", "toUpper(this)")
        txtTesseraSanitaria.Attributes.Add("onblur", "toUpper(this)")
        txtCodiceFiscale.Attributes.Add("onblur", "toUpper(this)")
        txtPadre.Attributes.Add("onblur", "toUpper(this)")
        txtMadre.Attributes.Add("onblur", "toUpper(this)")

        Select Case Request.Form("__EVENTTARGET")

            Case "RefreshFromPopup"
                '--
                ' Refresh dopo la chiusura della popup di rilevazione del consenso => devo sempre impostare left e stato pagina in base al nuovo stato del consenso
                '--
                Dim result As ManageConsensoResult = ManageLayoutConsenso(False)
                If result.IsConsensoBloccante.HasValue AndAlso result.IsConsensoBloccante.Value Then
                    LoadLeftFrame = result.LoadLeft
                    StatoCorrentePagina = result.StatoPagina
                Else
                    ControlloCampi(True)
                    StatoCorrentePagina = StatoPagina.VIEW
                End If

            Case "ChiudiCalVac"

                MainToolbar.Enabled = True

            Case "EliminazioneProgrammazione"

                ' N.B. : in questo caso non vengono eliminate le cnv con appuntamento, quindi i dati di eliminazione non servono
                EliminaProgrammazione(False, False, String.Empty, String.Empty)

                'salvataggio necessario in quanto il client esegue un needPostBack=false per passare l'EVENTTARGET (modifica 03/06/2004)
                SalvaDati(True)

                'controllo stato Gestione Manuale (modifica 03/06/2004)
                ControllaGestioneManuale()

            Case "EliminazioneProgrammazioneRischio"

                gestionePazientiDatiSanitari.gestisciCategoriaARischio = True
                SalvaDati(True)

            Case "SedeAutomatica"

                'impostazione automatica della sede vaccinale (modifica 10/01/2004)
                Select Case Request.Form("__EVENTARGUMENT")

                    Case "true"

                        'aggiorno la modale contenente la sede vaccinale
                        RegistraDatiConsultorio()
                        StatoCorrentePagina = StatoPagina.EDIT

                        SetCnsVaccinale(SedeVaccAuto(0), SedeVaccAuto(1), False)

                        OnitLayout21.InsertRoutineJS("alert(""La sede vaccinale è stata impostata automaticamente. Premere il pulsante 'Salva' per confermare"");")

                    Case "false"

                        Dim campiObbligatori As New ArrayList()
                        Dim campiWarning As New ArrayList()

                        If Not CheckCampiObbligatori(campiObbligatori, True) Or Not CheckCampiObbligatori(campiWarning, False) Then
                            ShowErrorNoFillRequiredField(campiObbligatori, campiWarning)
                        End If

                End Select

            Case "EliminaProg_cambioStatus"

                ' Scatta se lo stato anagrafico è stato cambiato ed ha assunto valori diversi da RESIDENTE o DOMICILIATO (letti da db)
                ' e l'operatore ha richiesto la cancellazione della programmazione
                EliminaProgrammazione(True, True, Constants.MotiviEliminazioneAppuntamento.VariazioneStatoAnagrafico, "Eliminazione convocazioni paziente per variazione stato anagrafico da maschera GestionePazienti")

            Case "VariaStatoAnagrafico"

                Select Case Request.Form("__EVENTARGUMENT")

                    Case "true"

                        ' ---------------------
                        ' serve ?!?!
                        ' ---------------------
                        RegistraDatiConsultorio()
                        odpDettaglioPaziente.EditRecord()
                        StatoCorrentePagina = StatoPagina.EDIT
                        ' ---------------------
                        cmbStatoAnagrafico.SelectedValue = Val(NuovoStatoAnagrafico)
                        SalvaDati(False)

                End Select

        End Select

        'controllo sulla gestione manuale (modifica 03/06/2004)
        ControllaGestioneManuale()

        'controllo sulla categoria a rischio (modifica 31/08/2004)
        ControllaCategoriaRischio()

        gestionePazientiDatiSanitari.ImpostaHandlerFrecciaSuGiu()

    End Sub

    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles MyBase.PreRender

        NascondiCampi()

        DataObbligatoria("canc")
        DataObbligatoria("irrep")
        TipoOccasionale()

        txtSedeVaccinale.Obbligatorio = Settings.CONSULTORIO_OBBL
        txtCircoscrizione.Obbligatorio = Settings.CIRCOSCRIZIONE_OBBL

        If DisabilitaLayout Or (StatoCorrentePagina = StatoPagina.BLOCCO_CONSENSO) Then

            viaDomicilio.Enabled = False
            viaResidenza.Enabled = False
            OnitLayout21.Busy = False
            MaintainScrollPositionOnPostBack = False

            gestionePazientiDatiSanitari.AbilitaLayout(False)

            DisableCheckPreferenza()

            If Me.StatoCorrentePagina = StatoPagina.BLOCCO_CONSENSO Then
                ' Toolbar disabilitata
                DisableToolbar(True)
                btnConsenso.Enabled = True
                btnAutoRilevazioneConsensi.Enabled = True
            Else
                ' Toolbar disabilitata tranne le stampe
                DisableToolbar(False)
                btnConsenso.Enabled = False
                btnAutoRilevazioneConsensi.Enabled = False
            End If

        Else

            Select Case StatoCorrentePagina

                Case StatoPagina.VIEW

                    viaDomicilio.Enabled = False
                    viaResidenza.Enabled = False
                    btnConsenso.Enabled = Not OnVacUtility.IsPazIdEmpty
                    btnAutoRilevazioneConsensi.Enabled = Not OnVacUtility.IsPazIdEmpty
                    OnitLayout21.Busy = False
                    MaintainScrollPositionOnPostBack = False

                Case StatoPagina.EDIT, StatoPagina.LOCK

                    viaDomicilio.Enabled = True
                    viaResidenza.Enabled = True
                    btnConsenso.Enabled = False
                    btnAutoRilevazioneConsensi.Enabled = False
                    OnitLayout21.Busy = True
                    odpDettaglioPaziente.EditRecord()
                    MaintainScrollPositionOnPostBack = True

                    DisabilitaCampi()
                    CaricaCampiObbligatori(OnVacUtility.OpenDam())
                    odpDettaglioPaziente.BindControls()

            End Select

            SetCheckPreferenza()
            SetToolBar()

        End If

    End Sub

    Protected Function HideLeftFrameIfNeeded() As String

        If Not Page.IsPostBack OrElse RefreshLeftFrame Then
            RefreshLeftFrame = False
            Return GetOpenLeftFrameScript(LoadLeftFrame)
        End If

        Return String.Empty

    End Function

    Private Sub LoadDataPanelPazCodice(pazCodice As String)

        'caso in cui il codice sia già istanziato nelle onvacutility
        odpDettaglioPaziente.Optimizer.useAlternativePlan = True

        Dim f As New OnitDataPanel.Filter()
        f.Operator = OnitDataPanel.Filter.FilterOperators.And
        f.connectionName = Constants.ConnessioneAnagrafe.LOCALE
        f.TableName = "T_PAZ_PAZIENTI"
        f.Field = "PAZ_CODICE"
        f.FieldType = OnitDataPanel.Filter.FieldTypes.Number
        f.Comparator = OnitDataPanel.Filter.FilterComparators.Uguale
        f.Value = pazCodice
        f.valueProvenience = OnitDataPanel.Filter.ValueProveniences.valueProperty

        odpDettaglioPaziente.Filters.Clear()
        odpDettaglioPaziente.Filters.Add(f)
        odpDettaglioPaziente.LoadData()

    End Sub

    Private Sub SetFlagPazLocale(genericProvider As DbGenericProvider, currentRow As DataRow)

        Using bizPaziente As BizPaziente = BizFactory.Instance.CreateBizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), New BizLogOptions(DataLogStructure.TipiArgomento.PAZIENTI, False))

            Dim codiceRegionale As String = String.Empty

            If Not currentRow.IsNull("paz_codice_regionale") Then
                codiceRegionale = currentRow("paz_codice_regionale").ToString()
            End If

            Dim locale As Boolean = bizPaziente.CalcolaFlagLocale(codiceRegionale)

            ' Impostazione check
            odpDettaglioPaziente.GetCurrentTableEncoder().setValOf(currentRow, IIf(locale, "S", "N"), Constants.ConnessioneAnagrafe.LOCALE, "paz_locale", "t_paz_pazienti")

            chkLocale.Checked = locale

        End Using

    End Sub

#End Region

#Region " Eventi pulsanti modale conferma password "

    Private Sub confermaDatiPaziente_btnOk_Click(sender As Object, e As EventArgs) Handles confermaDatiPaziente_btnOk.Click

        confermaDatiPaziente.VisibileMD = False

        Dim userAuthenticationInfo As OnVacContext.UserAuthenticationInfo = OnVacContext.GetCurrentUserAuthenticationInfo(confermaDatiPaziente_txt.Text)

        If userAuthenticationInfo.IsAuthenticated Then

            If Not SalvaDatiPaziente(False) Then

                'resto in modalità di modifica
                RegistraDatiConsultorio()
                StatoCorrentePagina = StatoPagina.EDIT

            End If

        Else

            GestionePazientiMessage.Console.StatusMessage(String.Format("{0}. Salvataggio non riuscito.", userAuthenticationInfo.AuthenticationError), WarningMessage.MessageType.ErrorMessage)

            'resto in modalità di modifica
            RegistraDatiConsultorio()
            StatoCorrentePagina = StatoPagina.EDIT

        End If

    End Sub

    Private Sub confermaDatiPaziente_btnCancel_Click(sender As Object, e As EventArgs) Handles confermaDatiPaziente_btnCancel.Click

        confermaDatiPaziente.VisibileMD = False
        GestionePazientiMessage.Console.StatusMessage("Salvataggio annullato dall'utente.", WarningMessage.MessageType.ErrorMessage)

        'resto in modalità di modifica
        RegistraDatiConsultorio()
        StatoCorrentePagina = StatoPagina.EDIT

    End Sub

#End Region

#Region " Inizializzazione e caricamento dati "

#Region " Caricamento dati e bind combo "

    ' Carica lo stato anagrafico dalla t_ana_stati_anagrafici
    Private Sub CaricaStatoAnagrafico(genericProvider As DbGenericProvider)

        Dim dt As DataTable = Nothing

        Using bizStatiAnagrafici As New BizStatiAnagrafici(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
            dt = bizStatiAnagrafici.LeggiStatiAnagrafici()
        End Using

        ' Bind dei dati passati per parametro
        cmbStatoAnagrafico.DataValueField = "san_codice"
        cmbStatoAnagrafico.DataTextField = "san_descrizione"
        cmbStatoAnagrafico.DataSource = dt
        cmbStatoAnagrafico.DataBind()

        ' Aggiunta elemento nullo
        cmbStatoAnagrafico.Items.Insert(0, New ListItem(String.Empty, String.Empty))
        cmbStatoAnagrafico.Items(0).Selected = True
        cmbStatoAnagrafico.DataSource = Nothing

    End Sub

    ' Carica lo stato anagrafico dettagliato dalla t_ana_codifiche
    Private Sub CaricaStatoAnagraficoDettagliato(genericProvider As DbGenericProvider)

        Dim collCodifiche As Collection.CodificheCollection = GetCodifiche("PAZ_STATO_ANAGRAFICO_DETT", genericProvider)

        BindCombo(cmbStatoAnagraficoDettagliato, collCodifiche)

    End Sub

    ' Carica il tipo di occasionalità dalla t_ana_codifiche
    Private Sub CaricaTipoOccasionalita(genericProvider As DbGenericProvider)

        Dim collCodifiche As Collection.CodificheCollection = GetCodifiche("PAZ_TIPO_OCCASIONALITA", genericProvider)

        BindCombo(cmbTipoOccasionalita, collCodifiche)

    End Sub

    Private Sub LoadMacrocategorieRischio(genericProvider As DbGenericProvider)
        Using bizRischio As New Biz.BizCategorieRischio(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
            ddlMacrocategorieRischio.DataSource = bizRischio.LoadDataTableMacroCategorieRischio(False, True)
            ddlMacrocategorieRischio.DataBind()
            ddlMacrocategorieRischio.SelectedValue = String.Empty
        End Using
    End Sub


    Private Sub BindCombo(ByRef cmb As OnitDataPanel.wzDropDownList, collCodifiche As Collection.CodificheCollection)

        ' Bind dei dati passati per parametro
        cmb.DataValueField = "Codice"
        cmb.DataTextField = "Descrizione"
        cmb.DataSource = collCodifiche
        cmb.DataBind()

        ' Aggiunta elemento nullo
        cmb.Items.Insert(0, New ListItem(String.Empty, String.Empty))
        cmb.Items(0).Selected = True
        cmb.DataSource = Nothing

    End Sub

    ' Restituisce un datatable contenente le codifiche relative al campo specificato.
    Private Function GetCodifiche(campo As String, genericProvider As DAL.DbGenericProvider) As Collection.CodificheCollection

        Dim collCodifiche As Collection.CodificheCollection = Nothing

        Using bizCodifiche As New BizCodifiche(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
            collCodifiche = bizCodifiche.GetCodifiche(campo)
        End Using

        Return collCodifiche

    End Function

#End Region

    ' Allineamento dati paziente in locale con quelli del db centrale, tramite il pannello
    Private Sub AllineamentoCentraleLocale()

        odpDettaglioPaziente.EditRecord()

        If odpDettaglioPaziente.CurrentRecord = 0 Then
            odpDettaglioPaziente.getCurrentDataRow.BeginEdit()
            odpDettaglioPaziente.getCurrentDataRow.EndEdit()
        End If

        itemArrayDatiPazienteOriginal = odpDettaglioPaziente.getCurrentDataRow().ItemArray
        SalvaDatiPaziente(True)

    End Sub

    Private Sub ImpostaParametriServizio()

        If odpDettaglioPaziente.Connections(Constants.ConnessioneAnagrafe.CENTRALE).Parameters.Item("strEnte") Is Nothing Then

            Dim parEnte As New OnitDataPanel.PanelParameter("strEnte")

            parEnte.Enabled = True
            parEnte.Type = OnitDataPanel.PanelParameter.Types.String
            parEnte.value = OnVacContext.AppId
            parEnte.valueProvenience = OnitDataPanel.PanelParameter.Proveniences.valueProperty

            odpDettaglioPaziente.Connections(Constants.ConnessioneAnagrafe.CENTRALE).Parameters.Add(parEnte)

        End If

        If odpDettaglioPaziente.Connections(Constants.ConnessioneAnagrafe.CENTRALE).Parameters.Item("strAddetto") Is Nothing Then

            Dim parAddetto As New OnitDataPanel.PanelParameter("strAddetto")

            parAddetto.Enabled = True
            parAddetto.Type = OnitDataPanel.PanelParameter.Types.String
            parAddetto.value = OnVacContext.UserCode
            parAddetto.valueProvenience = OnitDataPanel.PanelParameter.Proveniences.valueProperty

            odpDettaglioPaziente.Connections(Constants.ConnessioneAnagrafe.CENTRALE).Parameters.Add(parAddetto)

        End If

    End Sub

    ' Impostazione della sede vaccinale
    Private Sub SetCnsVaccinale(codiceConsultorio As String, descrizioneConsultorio As String, setIndirizzoCns As Boolean)

        txtSedeVaccinale.Codice = codiceConsultorio
        txtSedeVaccinale.Descrizione = descrizioneConsultorio
        txtSedeVaccinale.RefreshDataBind()

        If setIndirizzoCns Then txtIndirizzoSedeVaccinale.Text = txtSedeVaccinale.GetAltriCampi(2)

    End Sub

    ' Controllo che la data di decesso e lo stato anagrafico siano congruenti. 
    ' Se è valorizzata la data di decesso ma lo stato anagrafico non è DECEDUTO, deve essere richiesta conferma all'utente 
    ' per modificare lo stato anagrafico. Viene registrato lo script e, nel pageLoad, viene raccolto l'eventtarget 
    ' "VariaStatoAnagrafico" che, in caso di risposta positiva dell'utente, avvia il salvataggio. 
    Private Sub ControllaDecesso()

        ' DMI 14/03/05 ---------------------------------------------------------------------------------
        If Not odpDettaglioPaziente.getCurrentDataRow() Is Nothing Then

            Dim dr As DataRow = odpDettaglioPaziente.getCurrentDataRow()

            Dim sAnagrafico As String = String.Empty
            Dim statoAnag As Int16

            ' Controllo se è stata valorizzata la data di decesso.
            If (odpDettaglioPaziente.GetCurrentTableEncoder().getValOf(dr, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_DATA_DECESSO", "T_PAZ_PAZIENTI", True).ToString) <> String.Empty Then
                statoAnag = Enumerators.StatoAnagrafico.DECEDUTO
                sAnagrafico = statoAnag.ToString()
            End If

            ' Controllo se lo stato anagrafico calcolato è valorizzato e diverso da quello impostato.
            If (sAnagrafico <> String.Empty) AndAlso (odpDettaglioPaziente.GetCurrentTableEncoder().getValOf(dr, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_STATO_ANAGRAFICO", "T_PAZ_PAZIENTI", True).ToString) <> sAnagrafico Then

                NuovoStatoAnagrafico = sAnagrafico

                ' Script di conferma per l'utente.
                strScript = "if (confirm(""Attenzione: è stata valorizzata la data di decesso. Modificare lo stato anagrafico del paziente in DECEDUTO? Premendo OK, il paziente non verrà più chiamato a vaccinarsi!""))"
                strScript &= "__doPostBack('VariaStatoAnagrafico','true');"
                strScript &= "else "
                strScript &= "__doPostBack('VariaStatoAnagrafico','false');"

                OnitLayout21.InsertRoutineJS(strScript)

            End If

        End If

    End Sub

    ' Imposta il layout della maschera (la toolbar e qualche altro elemento, i campi sono invece gestiti dal pannello).
    ' Il parametro statoModifica indica se la maschera deve essere in stato di modifica oppure no.
    Private Sub RegistraDatiConsultorio()

        Dim drRow As DataRow = odpDettaglioPaziente.getCurrentDataRow()

        If Not drRow Is Nothing Then

            Dim objCons As Object = odpDettaglioPaziente.GetCurrentTableEncoder().getValOf(drRow, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_CNS_CODICE", "T_PAZ_PAZIENTI", True)
            If Not IsDBNull(objCons) Then
                CodConsVaccinale = objCons.ToString()
            End If

            Dim objTemp As Object = odpDettaglioPaziente.GetCurrentTableEncoder().getValOf(drRow, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_CNS_DATA_ASSEGNAZIONE", "T_PAZ_PAZIENTI", True)
            If Not IsDBNull(objTemp) Then
                DataAssegnazioneConsVaccinale = String.Format("{0:dd/MM/yyyy}", objTemp)
            End If

            objTemp = odpDettaglioPaziente.GetCurrentTableEncoder().getValOf(drRow, Constants.ConnessioneAnagrafe.LOCALE, "CNS_DESCRIZIONE", "T_ANA_CONSULTORI", True)
            If Not IsDBNull(objTemp) Then
                DescrConsVaccinale = objTemp.ToString()
            End If

            objTemp = odpDettaglioPaziente.GetCurrentTableEncoder().getValOf(drRow, Constants.ConnessioneAnagrafe.LOCALE, "CNS_INDIRIZZO", "T_ANA_CONSULTORI", True)
            If Not IsDBNull(objTemp) Then
                IndirConsVaccinale = objTemp.ToString()
            End If

            objTemp = odpDettaglioPaziente.GetCurrentTableEncoder().getValOf(drRow, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_CNS_CODICE_OLD", "T_PAZ_PAZIENTI", True)
            If Not IsDBNull(objTemp) Then
                CodConsVaccinalePrec = objTemp.ToString()
            End If

            objTemp = odpDettaglioPaziente.GetCurrentTableEncoder().getValOf(drRow, Constants.ConnessioneAnagrafe.LOCALE, "CNS_DESCRIZIONE", "T_ANA_CONS_OLD", True)
            If Not IsDBNull(objTemp) Then
                DescrConsVaccinalePrec = objTemp.ToString()
            End If

        End If

    End Sub

    'controllo del campo "Categoria a Rischio" (modifica 31/08/2004)
    Private Sub ControllaCategoriaRischio()

        Dim drPaz As DataRow = odpDettaglioPaziente.getCurrentDataRow()
        Dim enc As OnitDataPanel.FieldsEncoder = odpDettaglioPaziente.GetCurrentTableEncoder()
        Dim val As Object = enc.getValOf(drPaz, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_RSC_CODICE", "T_PAZ_PAZIENTI", False)

        If Not val Is Nothing AndAlso Not val Is DBNull.Value Then
            CategoriaRischio = val.ToString
        Else
            CategoriaRischio = String.Empty
        End If

    End Sub

    'controllo stato del flag "Gestione Manuale" (modifica 03/06/2004)
    Private Sub ControllaGestioneManuale()

        Dim drPaz As DataRow = odpDettaglioPaziente.getCurrentDataRow()
        Dim enc As OnitDataPanel.FieldsEncoder = odpDettaglioPaziente.GetCurrentTableEncoder()
        Dim val As Object = enc.getValOf(drPaz, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_COMPLETARE", "T_PAZ_PAZIENTI", False)

        If val Is Nothing OrElse val Is DBNull.Value Then
            GestioneManuale = "N"
        Else
            GestioneManuale = val.ToString()
        End If

    End Sub

    Sub ReloadAll(DAM As IDAM)

        gestionePazientiDatiSanitari.ReloadAll(DAM)

        'Aggiorna le finestre modali
        If txtSedeVaccinale.Codice <> "" Then
            txtSedeVaccinale.RefreshDataBind()
            txtIndirizzoSedeVaccinale.Text = txtSedeVaccinale.GetAltriCampi(2)
        End If

        '- titolo della pagina
        LayoutTitolo.InnerHtml = PageTitle(True)

    End Sub

    Protected Function PageTitle(includiEta As Boolean)

        Dim pazRow As DataRow = odpDettaglioPaziente.getCurrentDataRow()
        Dim panelEncoder As OnitDataPanel.FieldsEncoder = odpDettaglioPaziente.GetCurrentTableEncoder()

        Dim cognome As String = String.Empty

        Dim obj As Object = panelEncoder.getValOf(pazRow, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_COGNOME", "T_PAZ_PAZIENTI", True)

        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
            cognome = obj.ToString()
        End If

        Dim nome As String = String.Empty

        obj = panelEncoder.getValOf(pazRow, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_NOME", "T_PAZ_PAZIENTI", True)

        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
            nome = obj.ToString()
        End If

        Dim dataNascita As Date? = Nothing

        obj = panelEncoder.getValOf(pazRow, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_DATA_NASCITA", "T_PAZ_PAZIENTI", True)

        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
            dataNascita = Convert.ToDateTime(obj)
        End If

        Dim eta As String = String.Empty

        If includiEta Then
            eta = " - età " + OnVacUtility.ImpostaEtaPazienteConv(Date.Now, IsGestioneCentrale, Settings)
        End If

        Dim titolo As String = String.Format("{0} {1}", cognome, nome)

        If dataNascita.HasValue Then
            titolo += String.Format(" [nato il {0:dd/MM/yyyy}{1}]", dataNascita.Value, eta)
        End If

        Return titolo

    End Function

#End Region

#Region " Eventi toolbars "

    Protected Sub MainToolbar_ButtonClick(sender As Object, e As Telerik.Web.UI.RadToolBarEventArgs) Handles MainToolbar.ButtonClick

        If gestionePazientiDatiSanitari.ControlState = GestionePazientiDatiSanitari.ControlStateEnum.LOCK Then
            Exit Sub
        End If

        Select Case e.Item.Value

            Case "btnSalva"

                Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

                    ' Se il valore del campo nascosto è false deve ripristinare i valori precedenti di data
                    ' assegnazione, sede vaccinale corrente e precedente e non effettuare il salvataggio.
                    ' Altrimenti va avanti con i controlli e il salvataggio.
                    If txtControlloConfermaSalva.Text.ToLower() = "false" Then

                        ' Ripristino i valori precedenti
                        txtDataAssegnazione.Text = DataAssegnazioneConsVaccinale
                        txtSedeVaccinale.Descrizione = DescrConsVaccinale
                        txtSedeVaccinale.Codice = CodConsVaccinale
                        txtIndirizzoSedeVaccinale.Text = IndirConsVaccinale
                        txtSedeVaccinalePrec.Descrizione = DescrConsVaccinalePrec
                        txtSedeVaccinalePrec.Codice = CodConsVaccinalePrec

                        ' Messaggio all'utente
                        GestionePazientiMessage.Console.StatusMessage("Salvataggio non effettuato: sono stati ripristinati i valori precedenti delle sedi vaccinali.", WarningMessage.MessageType.AlertMessage)

                        Exit Sub
                    End If

                    'controllo validità dei codici comuni 
                    If Settings.CHECKVALCOMUNI Then

                        Dim checkComuniResult As BizGenericResult = Nothing

                        Using bizPaz As New BizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                            Dim command As New BizPaziente.CheckComuniPazienteCommand With {
                                .CodiceComuneNascita = txtComuneDiNascita.Codice,
                                .DataNascita = txtDataNascita.Data,
                                .CodiceComuneResidenza = txtComuneResidenza.Codice,
                                .CodiceComuneDomicilio = txtComuneDomicilio.Codice,
                                .CodiceComuneEmigrazione = txtLuogoEmigrazione.Codice,
                                .DataEmigrazione = txtDataEmigrazione.Data,
                                .CodiceComuneImmigrazione = txtLuogoImmigrazione.Codice,
                                .DataImmigrazione = txtDataImmigrazione.Data
                            }

                            checkComuniResult = bizPaz.CheckComuniPaziente(command)

                        End Using

                        If Not checkComuniResult.Success Then
                            AlertClientMsg(String.Format("Errore nel salvataggio dei dati del paziente!{0}{0}{1}", Environment.NewLine, checkComuniResult.Message))
                            Return
                        End If

                    End If

                    ' Controllo sui cicli e impostazione dello stato del paziente di conseguenza
                    Dim controllo As Boolean = gestionePazientiDatiSanitari.ImpostaCicliEStatoVaccinale(genericProvider)

                    ' Se stiamo inserendo un paz nell'anagrafe locale, va controllato che il cod. fisc. 
                    ' non sia già assegnato ad un altro paziente.
                    Dim cod_fisc As String = Me.txtCodiceFiscale.Text.Trim()

                    If odpDettaglioPaziente.CurrentOperation = OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord And
                       Settings.TIPOANAG = Onit.OnAssistnet.OnVac.Enumerators.TipoAnags.SoloLocale And
                       cod_fisc <> String.Empty Then

                        Using bizPaz As New BizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                            If bizPaz.IsCodiceFiscaleDuplicato(OnVacUtility.Variabili.PazId, cod_fisc) Then
                                ' Messaggio all'utente
                                GestionePazientiMessage.Console.StatusMessage("ATTENZIONE: esiste già un paziente con il codice fiscale specificato. Salvataggio non effettuato.", WarningMessage.MessageType.AlertMessage)
                                Exit Sub
                            End If

                        End Using

                    End If

                End Using

                SalvaDati(True)

                'controllo stato Gestione Manuale (modifica 03/06/2004)
                ControllaGestioneManuale()

                'se non è stato effettuato in precedenza, viene fatto 
                'il controllo sulla sede vaccinale [modifica 01/06/2006]
                If Not ControlloSedeVaccinale Then ControllaSedeVaccinale()

            Case "btnCalVac"

                uscCalVac.LoadData()
                modCalendarioVaccinale.VisibileMD = True

            Case "btnAnnulla"

                StatoCorrentePagina = StatoPagina.VIEW
                gestionePazientiDatiSanitari.Annulla()
                Response.Redirect("GestionePazienti.aspx?annullato=true", False)

            Case "btnModifica"

                RegistraDatiConsultorio()

                StatoCorrentePagina = StatoPagina.EDIT

            Case "btnCertificato"

                OnVacUtility.StampaCertificatoVaccinale(Page, Settings, True, False)

            Case "btnCertificatoVaccinaleValido"

                OnVacUtility.StampaCertificatoVaccinale(Page, Settings, False, False)

            Case "btnCertificatoFrequenza"

#Region " Test Certificato APP"

#If DEBUG Then
                ' Test per visualizzazione stampa certificato APP
                If False Then

                    RegisterStartupScriptCustom("openHandlerTestCertificatoAPP",
                        String.Format("window.open('{0}?paz={1}&ute={2}&app={3}&azi={4}&cns={5}&ulss={6}');", ResolveClientUrl("~/Common/Handlers/Handler1.ashx"),
                                      OnVacUtility.Variabili.PazId, OnVacContext.UserId.ToString(), OnVacContext.AppId,
                                      OnVacContext.Azienda, OnVacUtility.Variabili.CNS.Codice, OnVacContext.CodiceUslCorrente))
                    Return

                End If
#End If

#End Region

                OnVacUtility.StampaCertificatoVaccinale(Page, Settings, False, True)

            Case "btnCertificatoVaccinaleLotti"

                OnVacUtility.StampaCertificatoVaccinaleLotti(Page, Settings)

            Case "btnCertificatoEseguiteScadute", "btnCertificatoDiscrezionale", "btnCertificatoMantoux", "btnCertificatoSoloMantoux"

                StampaCertificato(e.Item.Value)

            Case "btnCertificatoVaccinaleCovid"

                OnVacUtility.StampaCertificatoVaccinalePazienteSoloVaccinazioniSpecificate(Page, Settings, False, False, Settings.CODICI_VACCINAZIONI_COVID)

        End Select

    End Sub

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles SelectStampaLibrettoVaccinaleToolBar.ButtonClicked

        If gestionePazientiDatiSanitari.ControlState = GestionePazientiDatiSanitari.ControlStateEnum.LOCK Then
            Exit Sub
        End If

        Select Case e.Button.Key

            Case "btnSelectedLibrettoVaccinale"
                Me.StampaLibrettoVaccinaleSingolaPagina()

            Case "btnSelectedLibrettoVaccinale2"
                Me.StampaLibrettoVaccinale(Constants.ReportName.LibrettoVaccinale2)

            Case "btnSelectedLibrettoVaccinale3"
                Me.StampaLibrettoVaccinale(Constants.ReportName.LibrettoVaccinale3)

        End Select

    End Sub

#End Region

#Region " Salvataggio "

    '- esegue il salvataggio dei dati non gestiti dal datapanel
    Private Sub SalvaDati(eseguiControlliDuplicazione As Boolean)

        If chkCancellato.Checked And String.IsNullOrWhiteSpace(txtDataCancellazione.Text) Then
            OnitLayout21.InsertRoutineJS("alert(""Il campo Data Cancellazione è obbligatorio!"");")

        ElseIf chkIrreperibile.Checked And String.IsNullOrWhiteSpace(txtDataIrreperibilta.Text) Then
            OnitLayout21.InsertRoutineJS("alert(""Il campo Data Irreperibilità è obbligatorio!"");")

        ElseIf chkOccasionale.Checked And cmbTipoOccasionalita.SelectedIndex = 0 Then
            OnitLayout21.InsertRoutineJS("alert(""Il campo Tipo Occasionalità è obbligatorio!"");")

        End If

        Dim campiObbligatori As New ArrayList()
        Dim campiWarning As New ArrayList()

        ' Flag che vale true se l'operazione è un'inserimento, false altrimenti
        IsPazienteNew = False

        ' Valori originali degli indirizzi
        IndResidenzaOriginal = String.Empty
        IndDomicilioOriginal = String.Empty

        ' Controllo campi obbligatori
        If Not CheckCampiObbligatori(campiObbligatori, True) Then

            ' Campi obbligatori non compilati
            CheckCampiObbligatori(campiWarning, False)
            ShowErrorNoFillRequiredField(campiObbligatori, campiWarning)
            GestionePazientiMessage.Console.StatusMessage("Errore nel salvataggio, alcuni campi fondamentali non sono stati impostati", WarningMessage.MessageType.AlertMessage)

        Else

            ' Campi obbligatori ok --> Controllo compatibilità cicli
            If Not gestionePazientiDatiSanitari.ControllaCompatibilitaCicli() Then

                ' Cicli non compatibili
                GestionePazientiMessage.Console.StatusMessage("I cicli del paziente non sono compatibili tra loro. Dati non salvati!", WarningMessage.MessageType.AlertMessage)

            Else
                ' Campi obbligatori ok e cicli compatibili --> Salvataggio

                ' Memorizzazione dei check dei giorni scelti
                CodificaGiorno()

                ' Allinea il codice della usl di residenza in base al comune di residenza
                Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                    AllineaUslResidenza(genericProvider)
                End Using

                Dim drRow As DataRow = odpDettaglioPaziente.getCurrentDataRow()

                ' --- Current Operation --- '
                ' Leggo la current operation prima del salva altrimenti diventa None 
                IsPazienteNew = (odpDettaglioPaziente.CurrentOperation = OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord)

                ' --- Calcolo codice ausiliario --- '
                If Settings.CALCOLA_COD_AUSILIARIO Then

                    ' Se l'operazione è un inserimento e il codice ausiliario è vuoto, lo calcolo e lo inserisco
                    If IsPazienteNew And drRow("PAZ_CODICE_AUSILIARIO").ToString() = String.Empty Then

                        '' ??? Settings.TIPOANAG <> Onit.OnAssistnet.OnVac.Enumerators.TipoAnags.SoloLocale 
                        ''     --> Il codice ausiliario viene sovrascritto.

                        Dim codiceAusiliario As String = String.Empty

                        ' Calcolo dell'ausiliario in base al progressivo associato.
                        Using bizProgressivi As New BizProgressivi(OnVacContext.CreateBizContextInfos, Nothing)

                            codiceAusiliario = bizProgressivi.CalcolaProgressivo(Constants.TipoProgressivo.Ausiliario, False)

                        End Using

                        ' Imposto il codice ausiliario nel campo della maschera e nel pannello
                        txtCodiceAusiliario.Text = codiceAusiliario
                        drRow("PAZ_CODICE_AUSILIARIO") = codiceAusiliario

                    End If

                End If

                ' --- Dati correnti e originali del paziente per invio messaggi esterni --- '
                ' Leggo la datarow prima del salvataggio per poter avere i dati originali.

                ' Utilizzo gli user control di gestione degli indirizzi (residenza e domicilio) per avere i dati correnti e originali. 
                IndResidenzaOriginal = viaResidenza.ValoreVecchioResidenza
                IndDomicilioOriginal = viaDomicilio.ValoreVecchioDomicilio

                ' Dati originali
                ' Per come è fatto il pannello, in questo caso nella riga corrente ci sono ancora i valori non modificati,
                ' per cui assegno direttamente l'itemarray anzichè fare un ciclo campo per campo assegnando la versione original.
                itemArrayDatiPazienteOriginal = drRow.ItemArray

                If Settings.TIPOANAG <> Enumerators.TipoAnags.SoloLocale Then
                    odpDettaglioPaziente.GetCurrentTableEncoder().setValOf(drRow, viaDomicilio.txtIndirizzo.Text, Constants.ConnessioneAnagrafe.CENTRALE, "PAZ_INDIRIZZO_DOMICILIO", "T_PAZ_PAZIENTI_CENTRALE")
                    odpDettaglioPaziente.GetCurrentTableEncoder().setValOf(drRow, viaResidenza.txtIndirizzo.Text, Constants.ConnessioneAnagrafe.CENTRALE, "PAZ_INDIRIZZO_RESIDENZA", "T_PAZ_PAZIENTI_CENTRALE")
                End If

                ' Controllo dati duplicati e salvataggio
                ControlloDuplicazioneESalvataggioDati(eseguiControlliDuplicazione)

            End If

        End If

        txtSedeVaccinale.Obbligatorio = True

    End Sub

    'ritraduce i giorni disponibili per essere salvati nel database
    Private Sub CodificaGiorno()

        Dim valp As Int16

        If chkDomenica.Checked Then valp = valp Or 1
        If chkLunedi.Checked Then valp = valp Or 2
        If chkMartedi.Checked Then valp = valp Or 4
        If chkMercoledi.Checked Then valp = valp Or 8
        If chkGiovedi.Checked Then valp = valp Or 16
        If chkVenerdi.Checked Then valp = valp Or 32
        If chkSabato.Checked Then valp = valp Or 64

        Dim row As DataRow = odpDettaglioPaziente.getCurrentDataRow()

        odpDettaglioPaziente.GetCurrentTableEncoder().setValOf(row, valp, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_GIORNO", "T_PAZ_PAZIENTI")

    End Sub

    Private Sub ControlloDuplicazioneESalvataggioDati(eseguiControlliDuplicazione As Boolean)

        ' Controlli paziente già presente in base ai campi configurati su db
        Dim risultatoControlloDuplicazione As Boolean = True

        If eseguiControlliDuplicazione Then

            ' TODO [GestionePazienti]: controlli paziente duplicato da fare in base ad un setting
            'If Me.Settings.ANAGPAZ_CONTROLLO_DUPLICATI Then

            ' TODO [GestionePazienti]: gestione controlli custom paziente duplicato
            ' risultatoControlloDuplicazione = controlli ...

            ' esito negativo -> 2 casi: blocco o warning a seconda del controllo fallito

            ' Controllo bloccante: stop salvataggio e alert all'utente.
            'risultatoControlloDuplicazione = False
            'Me.OnitLayout21.ShowMsgBox(New PagesLayout.OnitLayout3.OnitLayoutMsgBox("Salvataggio non effettuato. I dati inseriti corrispondono a quelli di un altro paziente già presente in anagrafe.", "checkPazienteDuplicato", False, False))
            'GestionePazientiMessage.Console.StatusMessage("Salvataggio non effettuato. Esiste un altro paziente con gli stessi dati!", WarningMessage.MessageType.ErrorMessage)

            ' Controllo non bloccante: richiesta di conferma all'utente, salvataggio rimandato nella gestione dell'evento opportuno 
            'If risultatoControlloDuplicazione Then

            '    risultatoControlloDuplicazione = False
            '    Me.OnitLayout21.ShowMsgBox(New PagesLayout.OnitLayout3.OnitLayoutMsgBox("ATTENZIONE! I dati inseriti corrispondono a quelli di un altro paziente già presente in anagrafe. Continuare con il salvataggio?", CONFIRM_SALVATAGGIO_PAZIENTE_DUPLICATO, True, True))

            'End If
            ' TODO [GestionePazienti]: utilizzare i messaggi configurati da db in base al controllo
            'End If

        End If

        If risultatoControlloDuplicazione Then

            Dim blnReturn As Boolean = False
            Dim lst As List(Of String) = CheckControlsLength()

            If lst.Count > 0 Then

                GestionePazientiMessage.Console.StatusMessage(lst.Aggregate(Function(p, g) p & Environment.NewLine & g), WarningMessage.MessageType.ErrorMessage)

                'resto in modalità di modifica
                RegistraDatiConsultorio()
                StatoCorrentePagina = StatoPagina.EDIT

                Return

            End If

            'controllo la lunghezza composita del cognome e del nome se corrisponde a quella parametrizzata
            If Settings.LEN_COGNOME_NOME > -1 AndAlso Not txtCognome Is Nothing AndAlso Not txtNome Is Nothing Then

                If txtCognome.Text.Length + txtNome.Text.Length + 1 > Settings.LEN_COGNOME_NOME Then

                    GestionePazientiMessage.Console.StatusMessage("Lunghezza del cognome e del nome non supportata.", WarningMessage.MessageType.ErrorMessage)

                    'resto in modalità di modifica
                    RegistraDatiConsultorio()
                    StatoCorrentePagina = StatoPagina.EDIT

                    Return

                End If

            End If

            If odpDettaglioPaziente.Connections(Constants.ConnessioneAnagrafe.CENTRALE).WriteAuth <> OnitDataPanel.Connection.writeAuthorizations.none Then

                'controlla i campi fondamentali
                Dim campiFondamentali() As String = Settings.CAMPFON.Split(";")

                'aggiorno i dati solo se sono in edit
                If odpDettaglioPaziente.CurrentOperation = OnitDataPanel.OnitDataPanel.CurrentOperationTypes.EditRecord Then
                    odpDettaglioPaziente.MoveRecord(OnitDataPanel.OnitDataPanel.RecordMoveTo.NextRecord, odpDettaglioPaziente.CurrentRecord)
                End If

                Dim strCampiModificati As String = String.Empty

                'devo controllare se il pannello è in stato di inserimento...
                If odpDettaglioPaziente.CurrentOperation <> OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord Then

                    Dim strVirgola As String = String.Empty
                    Dim oldVal, newVal As Object

                    Dim currentRow As DataRow = odpDettaglioPaziente.getCurrentDataRow()

                    Dim encoder As OnitDataPanel.FieldsEncoder = odpDettaglioPaziente.GetCurrentTableEncoder()

                    For Each nomeCampo As String In campiFondamentali

                        oldVal = encoder.getValOf(currentRow, Constants.ConnessioneAnagrafe.CENTRALE, nomeCampo, "t_paz_pazienti_centrale", False)
                        newVal = encoder.getValOf(currentRow, Constants.ConnessioneAnagrafe.CENTRALE, nomeCampo, "t_paz_pazienti_centrale", True)

                        If Not oldVal Is Nothing AndAlso Not newVal Is Nothing AndAlso oldVal.ToString() <> newVal.ToString() Then

                            If oldVal.ToString() <> "" Then
                                strCampiModificati += strVirgola + nomeCampo
                                strVirgola = ", "
                            Else
                                'si tratta di un campo che era sbiancato ed è stato aggiunto, questo non fa fede alla richiesta password
                            End If

                        End If

                    Next

                End If

                If strCampiModificati = "" Then

                    blnReturn = SalvaDatiPaziente(False)

                Else

                    'richiedo la password
                    confermaDatiPaziente.VisibileMD = True

                    'imposto il focus direttamente sul campo password
                    ClientScript.RegisterStartupScript(Me.GetType(), "confermaDatiPaziente_txt", OnVacUtility.GetJSFocus(confermaDatiPaziente_txt.ClientID, True), True)

                End If

            Else

                blnReturn = SalvaDatiPaziente(False)

            End If

            ' Dopo il salvataggio devo controllare che i valori di stato anagrafico e data di decesso siano congruenti.
            If blnReturn Then ControllaDecesso()

        End If

    End Sub

#End Region

#Region " Alert campi obbligatori/raccomandati/consenso "

    ' Visualizza un warning se mancano campi obbligatori/raccomandati o se il controllo del consenso è impostato a warning
    Private Sub ShowErrorNoFillRequiredField(campiObbligatori As ArrayList, campiWarning As ArrayList)

        ' Warning campi obbligatori
        If campiObbligatori.Count <> 0 Then
            GestionePazientiMessage.WarnCampiObbligatori.Text = GetCampiObbligatoriNotSet(campiObbligatori)
            GestionePazientiMessage.WarnCampiObbligatori.Visible = True
        End If

        ' Warning campi raccomandati
        If campiWarning.Count <> 0 Then
            GestionePazientiMessage.WarnCampiWarning.Text = GetCampiObbligatoriNotSet(campiWarning)
            GestionePazientiMessage.WarnCampiWarning.Visible = True
        End If

    End Sub

    'ritorna i campi obbligatori non impostati
    Private Function GetCampiObbligatoriNotSet(campi_non_valorizzati As ArrayList) As String

        Dim stbCampi As New System.Text.StringBuilder()

        For i As Integer = 0 To campi_non_valorizzati.Count - 1
            stbCampi.AppendFormat("{0}: {1}{2}", i + 1, campi_non_valorizzati(i), "<BR>")
        Next

        Return stbCampi.ToString()

    End Function

#End Region

#Region " Controllo campi obbligatori "

    ''' <summary>
    ''' Funzione di controllo obbligatorietà dei campi specificati nella tabella t_ana_campi_anagrafici_obbl
    ''' Controlla se tutti i campi obbligatori sono impostati e in base al campo can_bloccante blocca o lancia un warning all'utente
    ''' </summary>
    ''' <param name="elenco_non_valorizzati"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    Function CheckCampiObbligatori(ByRef elencoNonValorizzati As ArrayList, Optional obbligatori As Boolean = True) As Boolean

        Dim cittadinanza As String = txtCittadinanza.Codice

        Dim campiRichiesti As ArrayList

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using biz As New BizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                campiRichiesti = biz.GetCampiAnagraficiObbligatori(OnVacUtility.Variabili.PazId, obbligatori, cittadinanza)

            End Using
        End Using

        ' Campi non gestiti dal pannello: vanno resi obbligatori caso per caso

        ' --- Check della sezione "Preferenza" --- '
        If campiRichiesti.Contains("PAZ_GIORNO") Then
            If Not (chkLunedi.Checked And chkMartedi.Checked And chkMercoledi.Checked And chkGiovedi.Checked And chkVenerdi.Checked And chkSabato.Checked And chkDomenica.Checked) Then
                elencoNonValorizzati.Add("Preferenza")
                Return False
            End If
            campiRichiesti.Remove("PAZ_GIORNO")
        End If

        ' --- User control viaResidenza --- '
        If campiRichiesti.Contains("PAZ_INDIRIZZO_RESIDENZA") Then
            If viaResidenza.IndirizzoCorrente Is Nothing Then
                elencoNonValorizzati.Add("Indirizzo residenza")
                Return False
            End If
            campiRichiesti.Remove("PAZ_INDIRIZZO_RESIDENZA")
        End If

        ' --- User control viaDomicilio --- '
        If campiRichiesti.Contains("PAZ_INDIRIZZO_DOMICILIO") Then
            If viaDomicilio.IndirizzoCorrente Is Nothing Then
                elencoNonValorizzati.Add("Indirizzo domicilio")
                Return False
            End If
            campiRichiesti.Remove("PAZ_INDIRIZZO_DOMICILIO")
        End If

        If obbligatori Then

            Dim statoAnagDeceduto As Int16 = Enumerators.StatoAnagrafico.DECEDUTO

            If Settings.GESTPAZ_DATA_OBBLIGATORIA_SE_DECEDUTO Then
                If txtDataDecesso.Text = String.Empty And cmbStatoAnagrafico.SelectedValue = statoAnagDeceduto.ToString() Then
                    elencoNonValorizzati.Add("Data di decesso obbligatoria!")
                    Return False
                End If
            End If

            If txtDataDecesso.Text <> String.Empty And cmbStatoAnagrafico.SelectedValue <> statoAnagDeceduto.ToString() Then
                elencoNonValorizzati.Add("Lo Stato anagrafico deve essere deceduto, in quanto la data di decesso è valorizzata!")
                Return False
            End If

        End If

        ' Ricerca ricorsiva per comprendere tutti i campi del pannello (anche eventuali aggiunte successive)
        Dim compilaElenco As Boolean = True

        If elencoNonValorizzati Is Nothing Then
            elencoNonValorizzati = New ArrayList()
            compilaElenco = False
        End If

        Return ControlloObbligatoriPannello(odpDettaglioPaziente, campiRichiesti, compilaElenco, elencoNonValorizzati)

    End Function

    ''' <summary>
    ''' Cerca in tutto l'insieme dei controlli se, tra quelli obbligatori, qualcuno non è valorizzato.
    ''' </summary>
    ''' <param name="ctrl"></param>
    ''' <param name="obbligatori"></param>
    ''' <param name="blnCompilaNonValorizzati"></param>
    ''' <param name="elenco_non_valorizzati"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    Public Function ControlloObbligatoriPannello(ctrl As System.Web.UI.Control, ByRef elencoObbligatori As ArrayList, compilaNonValorizzati As Boolean, ByRef elencoNonValorizzati As ArrayList) As Boolean

        ' Se l'arraylist non contiene più campi, la ricerca è finita
        If elencoObbligatori.Count = 0 Then
            ' Se l'elenco dei non valorizzati non contiene nessun campo, tutti i campi obbligatori sono compilati
            Return (elencoNonValorizzati.Count = 0)
        End If

        ' --------- Campi gestiti dal pannello --------- '

        ' Devo testare se il campo è obbligatorio oppure è un contenitore di altri controlli
        If ctrl.Controls Is Nothing OrElse ctrl.Controls.Count = 0 Then
            ' Non è un contenitore di altri controlli, è un campo da testare

            Dim field As String = ""
            Dim field2 As String = ""

            Dim chk As OnitDataPanel.wzCheckBox = Nothing
            Dim ddl As OnitDataPanel.wzDropDownList = Nothing
            Dim dpk As OnitDataPanel.wzOnitDatePick = Nothing
            Dim val As OnitDataPanel.wzOnitJsValidator = Nothing
            Dim rdb As OnitDataPanel.wzRadioButtonList = Nothing
            Dim txt As OnitDataPanel.wzTextBox = Nothing
            Dim fm As OnitDataPanel.wzFinestraModale = Nothing

            Dim lbl As String = ""

            Dim controlloNonPrevisto As Boolean = False

            Select Case ctrl.GetType.Name
                Case "wzCheckBox"
                    chk = DirectCast(ctrl, OnitDataPanel.wzCheckBox)
                    field = chk.BindingField.SourceField
                Case "wzDropDownList"
                    ddl = DirectCast(ctrl, OnitDataPanel.wzDropDownList)
                    field = ddl.BindingField.SourceField
                Case "wzOnitDatePick"
                    dpk = DirectCast(ctrl, OnitDataPanel.wzOnitDatePick)
                    field = dpk.BindingField.SourceField
                Case "wzOnitJsValidator"
                    val = DirectCast(ctrl, OnitDataPanel.wzOnitJsValidator)
                    field = val.BindingField.SourceField
                Case "wzRadioButtonList"
                    rdb = DirectCast(ctrl, OnitDataPanel.wzRadioButtonList)
                    field = rdb.BindingField.SourceField
                Case "wzTextBox"
                    txt = DirectCast(ctrl, OnitDataPanel.wzTextBox)
                    field = txt.BindingField.SourceField
                Case "wzFinestraModale"
                    ' Per la modale considero sia il campo codice che il campo descrizione
                    fm = DirectCast(ctrl, OnitDataPanel.wzFinestraModale)
                    field = fm.BindingCode.SourceField
                    field2 = fm.BindingDescription.SourceField
                Case Else
                    controlloNonPrevisto = True
            End Select

            ' Se il campo non è gestito dal pannello, non continuo con il controllo
            If field = "" And field2 = "" Then
                Return True
            End If

            ' L'arraylist obbligatori contiene valori maiuscoli.
            ' Per sicurezza (dovrebbero esserlo già), metto maiuscoli anche i valori letti nei binding-fields.
            field = field.ToUpper()
            field2 = field2.ToUpper()

            ' --- Ricerca tra i campi obbligatori --- '
            If elencoObbligatori.Contains(field) Then
                ' Nascondo il campo. 

                ' Per nascondere anche la label associata, vado a leggere l'attributo LabelAssociata del controllo. 
                ' Se c'è, la cerco nello stesso namingContainer del campo e la nascondo
                If Not chk Is Nothing Then
                    If Not chk.Checked Then lbl = RecuperaLabelAssociata(ctrl)
                ElseIf Not ddl Is Nothing Then
                    If ddl.SelectedValue = "" Then lbl = RecuperaLabelAssociata(ctrl)
                ElseIf Not dpk Is Nothing Then
                    If dpk.Text = "" Then lbl = RecuperaLabelAssociata(ctrl)
                ElseIf Not val Is Nothing Then
                    If val.Text = "" Then lbl = RecuperaLabelAssociata(ctrl)
                ElseIf Not rdb Is Nothing Then
                    If rdb.SelectedIndex = -1 Then lbl = RecuperaLabelAssociata(ctrl)
                ElseIf Not txt Is Nothing Then
                    If txt.Text = "" Then lbl = RecuperaLabelAssociata(ctrl)
                ElseIf Not fm Is Nothing Then
                    If fm.Codice = "" Then lbl = RecuperaLabelAssociata(ctrl)
                End If

                ' Elimino il campo dall'array
                elencoObbligatori.Remove(field)

                If lbl <> "" Then

                    ' Aggiungo il nome della label nell'arraylist
                    elencoNonValorizzati.Add(lbl)

                    ' Se non devo compilare l'elenco dei campi obbligatori non valorizzati, termino subito la ricerca
                    If Not compilaNonValorizzati Then
                        Return False
                    End If

                End If

            Else

                If field2 <> "" And elencoObbligatori.Contains(field2) Then

                    If fm.Descrizione = "" Then lbl = RecuperaLabelAssociata(ctrl)

                    ' Elimino il campo dall'array
                    elencoObbligatori.Remove(field2)

                    If lbl <> "" Then

                        ' Aggiungo il nome della label nell'arraylist
                        elencoNonValorizzati.Add(lbl)

                        ' Se non devo compilare l'elenco dei campi obbligatori non valorizzati, termino subito la ricerca
                        If Not compilaNonValorizzati Then
                            Return False
                        End If

                    End If

                End If

            End If

            ' Devo finire perchè non ho altri controlli di livello inferiore a quello corrente. 
            ' Restituisco true perchè la ricerca deve continuare finchè non ne trovo uno obbligatorio vuoto 
            ' o fino alla fine, a seconda del parametro CompilaNonValorizzati
            Return True

        End If ' <-- il controllo non è un contenitore

        ' Se sono qui, il controllo è un contenitore quindi devo andare avanti con la ricerca
        Dim check_obbl As Boolean = True

        For i As Integer = 0 To ctrl.Controls.Count - 1

            check_obbl = ControlloObbligatoriPannello(ctrl.Controls(i), elencoObbligatori, compilaNonValorizzati, elencoNonValorizzati)

            If Not compilaNonValorizzati And Not check_obbl Then
                Return False
            End If

        Next

        Return (elencoNonValorizzati.Count = 0)

    End Function

    ''' <summary>
    ''' Per nascondere anche la label associata, vado a leggere l'attributo 
    ''' LabelAssociata del controllo. Se c'è, la cerco nello stesso namingContainer del campo
    ''' </summary>
    ''' <param name="ctrl"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    Public Function RecuperaLabelAssociata(ctrl As System.Web.UI.WebControls.WebControl) As String

        Dim labelName As String = ctrl.Attributes("LabelAssociata")
        Dim labelObj As Object
        Dim lbl As String = ""

        If Not labelName Is Nothing AndAlso labelName <> "" Then

            labelObj = ctrl.NamingContainer.FindControl(labelName)

            If Not labelObj Is Nothing Then
                ' Scrivo il nome della label associata nell'elenco dei campi non valorizzati
                If labelObj.GetType.Name = "Label" Then
                    lbl = CType(labelObj, System.Web.UI.WebControls.Label).Text
                Else
                    lbl = CType(labelObj, System.Web.UI.HtmlControls.HtmlGenericControl).InnerHtml
                End If
            End If

        Else
            ' In mancanza di label, scrivo l'id del controllo
            lbl = ctrl.ID
        End If

        Return lbl

    End Function

#End Region

#Region " Gestione Pannello "

    'scatta dopo il salvataggio dei dati nel pannello
    Private Function SalvaDatiPaziente(isAllineaLocaleCentrale As Boolean) As Boolean

        Dim success As Boolean = True

        Dim pazientePrecedente As Paziente = Nothing

        Dim currentTableEncoder As OnitDataPanel.FieldsEncoder = Nothing
        Dim currentRow As DataRow = Nothing

        Dim isLocaleNew As Boolean = False

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, OnVacUtility.GetReadCommittedTransactionOptions)

            Dim scritturaCentrale As Boolean = Me.ScritturaCentrale()
            Dim isCentraleNew As Boolean = False

            pazientePrecedente = GetPazientePrecedente()

            currentTableEncoder = odpDettaglioPaziente.GetCurrentTableEncoder()

            Dim s As Object = currentTableEncoder.getValOf(odpDettaglioPaziente.getCurrentDataRow(), "paz_codice", "t_paz_pazienti_centrale", False)
            If s Is DBNull.Value OrElse String.IsNullOrEmpty(s) Then
                isCentraleNew = True
            End If

            Dim s1 As Object = currentTableEncoder.getValOf(odpDettaglioPaziente.getCurrentDataRow(), "paz_codice", "t_paz_pazienti", False)
            If s1 Is DBNull.Value OrElse String.IsNullOrEmpty(s1) Then
                isLocaleNew = True
            End If

            'Dim codiceUslProgrammazioneVaccinale As String
            'Se è necessario copiare le convocazioni dall'usl di programmazione vaccinale ( se cambiata !!! )
            'Using bizPaziente As BizPaziente = Biz.BizFactory.Istance.CreateBizPaziente(OnVacContext.CreateBizContextInfos(), Nothing)
            '    codiceUslProgrammazioneVaccinale = bizPaziente.CalcolaCodiceUslProgrammazioneVaccinale(
            '        pazientePrecedente.UslAssistenza_Codice,
            '        pazientePrecedente.ComuneDomicilio_Codice,
            '        pazientePrecedente.UslResidenza_Codice)
            'End Using

            Using dam As IDAM = OnVacUtility.OpenDam()
                Using genericProvider As New DbGenericProvider(dam)

                    ' -----------------------------------
                    ' Step 1: salvataggio del dato anagrafico in locale 
                    '         => qui scatena l'evento beforeSaveRecord, che viene gestito in questa maschera e che effettua il salvataggio in centrale

                    Dim saveResult As OnitDataPanel.OnitDataPanelError = odpDettaglioPaziente.SaveData(True, True, dam)

                    If saveResult.ok Then

                        currentRow = odpDettaglioPaziente.getCurrentDataRow()
                        SetFlagPazLocale(genericProvider, currentRow)

                    Else
                        ' --------- '
                        ' TODO [PazScambiati]: gestire blocco del salvataggio (DA FINIRE)

                        'success = False

                        '' N.B. : il pannello nasconde l'eccezione, qui la trovo ma non è saltato in aria niente
                        'If saveResult.exc IsNot Nothing AndAlso saveResult.exc.GetType().Name() = GetType(PazientiScambiatiException).Name Then

                        '    ' PAZ SCAMBIATI: gestione eccezione + msg a video
                        '    CreaLogPazientiScambiati(saveResult.exc)

                        '    OnitLayout21.ShowMsgBox(New PagesLayout.OnitLayout3.OnitLayoutMsgBox(HttpUtility.JavaScriptStringEncode("Si e' verificato un problema durante il salvataggio. Ricaricare la maschera e riprovare."), "msgScambioPaz", False, False))
                        'Else

                        '    GestionePazientiMessage.Console.StatusMessage("Errore nel salvataggio dei dati.", WarningMessage.MessageType.ErrorMessage)
                        '    success = False

                        'End If
                        ' --------- '

                        GestionePazientiMessage.Console.StatusMessage("Errore nel salvataggio dei dati.", WarningMessage.MessageType.ErrorMessage)
                        success = False

                    End If

                    ' -----------------------------------
                    ' Step 2: Operazioni afteroperation del save
                    If success Then

                        Dim operationToLog As New Operazione()

                        ' Determino l'operazione che si sta eseguendo (inserimento/modifica)
                        If OnVacUtility.IsPazIdEmpty() Then

                            operationToLog = Operazione.Inserimento

                            ' In caso di inserimento, aggiorno l'id del paziente
                            If Not currentRow Is Nothing Then
                                OnVacUtility.Variabili.PazId = currentTableEncoder.getValOf(currentRow, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_CODICE", "T_PAZ_PAZIENTI", True)

                                ' TODO [GestionePazienti]: controllare se paz_codice_ausiliario
                                UltimoPazienteSelezionato = New UltimoPazienteSelezionato(String.Empty, OnVacUtility.Variabili.PazId)
                            End If

                        Else
                            operationToLog = Operazione.Modifica
                        End If

                        ' --- Log ---
#Region " OnVac.Log "

                        Dim testataLog As New Testata(DataLogStructure.TipiArgomento.PAZIENTI, operationToLog, False)
                        Dim recordLog As New Record()

                        ' itemArrayDatiPazienteOriginal non dovrebbe mai essere nothing perchè deve essere sempre valorizzato 
                        ' con la current datarow del pannello contenente i valori precedenti al salvataggio.
                        If Not itemArrayDatiPazienteOriginal Is Nothing Then

                            Dim tipoConn As String
                            If Settings.TIPOANAG <> Enumerators.TipoAnags.SoloLocale Then
                                tipoConn = Constants.ConnessioneAnagrafe.CENTRALE
                            Else
                                tipoConn = Constants.ConnessioneAnagrafe.LOCALE
                            End If

                            Dim encoder As OnitDataPanel.CodeTableField

                            For i As Integer = 0 To currentRow.Table.Columns.Count - 1

                                encoder = currentTableEncoder.decode(currentRow.Table.Columns(i).ColumnName.ToString())

                                If encoder.strConnection.ToUpper() = tipoConn.ToUpper() Then
                                    If itemArrayDatiPazienteOriginal(i).ToString() <> currentRow.Item(i).ToString() Then
                                        recordLog.Campi.Add(New Campo(encoder.strField, itemArrayDatiPazienteOriginal(i).ToString(), currentRow.Item(i).ToString()))
                                    End If
                                End If

                            Next

                        End If

#End Region
                        ' --- Indirizzo Domicilio --- '
#Region " Indirizzo domicilio "

                        If viaDomicilio.IndirizzoCorrente.IsNew Then
                            viaDomicilio.IndirizzoCorrente.Paziente = OnVacUtility.Variabili.PazId
                            viaDomicilio.PazCodice = OnVacUtility.Variabili.PazId
                        End If

                        viaDomicilio.SaveVia(dam)

                        If Settings.TIPOANAG <> Enumerators.TipoAnags.SoloLocale Then

                            viaDomicilio.ViaCentrale = currentTableEncoder.getValOf(currentRow, Constants.ConnessioneAnagrafe.CENTRALE, "PAZ_INDIRIZZO_DOMICILIO", "T_PAZ_PAZIENTI_CENTRALE", True).ToString()
                            viaDomicilio.InCentrale = True

                        End If

                        If Not viaDomicilio.ValoreVecchioDomicilio Is Nothing Then

                            If viaDomicilio.IndirizzoCorrente.Libero Is Nothing Then viaDomicilio.IndirizzoCorrente.Libero = String.Empty

                            If Settings.GESVIE AndAlso viaDomicilio.ValoreVecchioDomicilio <> Me.viaDomicilio.IndirizzoCorrente.Libero Then

                                If Not RecordLogContainsCampo(recordLog, "PAZ_INDIRIZZO_DOMICILIO") Then
                                    recordLog.Campi.Add(New Campo("PAZ_INDIRIZZO_DOMICILIO", Me.viaDomicilio.ValoreVecchioDomicilio, Me.viaDomicilio.IndirizzoCorrente.Libero))
                                End If

                            End If

                            'se non è codificato, deve riproporre il valore corrente
                            viaDomicilio.ValoreVecchioDomicilio = Me.viaDomicilio.IndirizzoCorrente.Libero

                        Else

                            If Settings.GESVIE AndAlso Not String.IsNullOrEmpty(viaDomicilio.IndirizzoCorrente.Libero) Then

                                If Not RecordLogContainsCampo(recordLog, "PAZ_INDIRIZZO_DOMICILIO") Then
                                    recordLog.Campi.Add(New Campo("PAZ_INDIRIZZO_DOMICILIO", Me.viaDomicilio.IndirizzoCorrente.Libero))
                                End If

                            End If

                        End If

                        ' Imposto il campo paz_cir_codice_2 in base alla circoscrizione di domicilio scelta, la loggo e la salvo su db
                        If ModificaCircoscrizioneDomicilio Then

                            ModificaCircoscrizioneDomicilio = False
                            UpdateCircoscrizione(viaDomicilio.PazCircoscrizioneCodice, False, recordLog, dam)

                        End If

                        viaDomicilio.LoadViaDB(dam)
                        viaDomicilio.Enabled = False
#End Region
                        ' --- Indirizzo Residenza --- '
#Region " Indirizzo residenza "

                        If viaResidenza.IndirizzoCorrente.IsNew Then
                            viaResidenza.IndirizzoCorrente.Paziente = OnVacUtility.Variabili.PazId
                            viaResidenza.PazCodice = OnVacUtility.Variabili.PazId
                        End If

                        viaResidenza.SaveVia(dam)

                        If Settings.TIPOANAG <> Enumerators.TipoAnags.SoloLocale Then

                            viaResidenza.ViaCentrale = currentTableEncoder.getValOf(currentRow, Constants.ConnessioneAnagrafe.CENTRALE, "PAZ_INDIRIZZO_RESIDENZA", "T_PAZ_PAZIENTI_CENTRALE", True).ToString()
                            viaResidenza.InCentrale = True

                        End If

                        If Not viaResidenza.ValoreVecchioResidenza Is Nothing Then

                            If viaResidenza.IndirizzoCorrente.Libero Is Nothing Then viaResidenza.IndirizzoCorrente.Libero = String.Empty

                            If Settings.GESVIE AndAlso viaResidenza.ValoreVecchioResidenza <> viaResidenza.IndirizzoCorrente.Libero Then

                                If Not RecordLogContainsCampo(recordLog, "PAZ_INDIRIZZO_RESIDENZA") Then
                                    recordLog.Campi.Add(New Campo("PAZ_INDIRIZZO_RESIDENZA", viaResidenza.ValoreVecchioResidenza, viaResidenza.IndirizzoCorrente.Libero))
                                End If

                            End If

                            'se non è codificato, deve riproporre il valore corrente
                            viaResidenza.ValoreVecchioResidenza = viaResidenza.IndirizzoCorrente.Libero

                        Else

                            If Settings.GESVIE AndAlso Not String.IsNullOrEmpty(viaResidenza.IndirizzoCorrente.Libero) Then

                                If Not RecordLogContainsCampo(recordLog, "PAZ_INDIRIZZO_RESIDENZA") Then
                                    recordLog.Campi.Add(New Campo("PAZ_INDIRIZZO_RESIDENZA", viaResidenza.IndirizzoCorrente.Libero))
                                End If

                            End If

                        End If

                        ' Imposto il campo paz_cir_codice circoscrizione in base alla circoscrizione di residenza scelta, la loggo e la salvo su db
                        If ModificaCircoscrizioneResidenza Then

                            ModificaCircoscrizioneResidenza = False
                            UpdateCircoscrizione(viaResidenza.PazCircoscrizioneCodice, True, recordLog, dam)

                        End If

                        If recordLog.Campi.Count > 0 Then

                            If isAllineaLocaleCentrale Then
                                recordLog.Campi.Add(New Campo("Modifica da Allineamento Centrale-Locale", "True"))
                            End If

                            testataLog.Records.Add(recordLog)

                        End If

                        viaResidenza.LoadViaDB(dam)
                        viaResidenza.Enabled = False

                        If testataLog.Records.Count > 0 Then LogBox.WriteData(testataLog)
#End Region

                    End If

                    ' -----------------------------------
                    ' Step 3: Allineamento delle malattie dal centrale
                    Dim letturaCentrale As Boolean = odpDettaglioPaziente.Connections(Constants.ConnessioneAnagrafe.CENTRALE).ReadAuth = OnitDataPanel.Connection.readAuthorizations.read
                    If success AndAlso letturaCentrale AndAlso Me.Settings.TIPOANAG_MALATTIE = Enumerators.TipoAnags.CentraleLettScritt Then
                        If isLocaleNew OrElse isAllineaLocaleCentrale Then

                            Dim pazCodiceAusiliario As String = GetCodiceAusiliario(currentRow)

                            Using bizPazienteCentrale As New BizPazienteCentrale(OnVacContext.CreateBizContextInfos(), Nothing)
                                Dim dtaMalattie As dsMalattie.MalattieDataTable = bizPazienteCentrale.CaricaMalattie(pazCodiceAusiliario)
                                gestionePazientiDatiSanitari.AddMalattie(dtaMalattie)
                            End Using

                        End If
                    End If

                    ' -----------------------------------
                    ' Step 4: salvataggio del dato sanitario in locale
                    If success Then

                        ' Prima del salvataggio rieseguo il bind con i dati della maschera
                        BindControls()
                        gestionePazientiDatiSanitari.ModificaPaziente(dam)

                        ' --- Inserimento Movimento e Modifica Consultorio delle Cnv --- '
                        ' Se c'è stata una variazione del consultorio vaccinale, devo salvare un record nella t_cns_movimenti
                        ' e modificare le convocazioni relative al paziente (variazione del cns e sbiancamento di data app, data invio e ambulatorio)
                        ' Queste operazioni devono essere effettuate solo se il salvataggio è andato a buon fine e non stiamo inserendo un nuovo paz.
                        If Not IsPazienteNew Then

#Region " Inserimento Movimento e Modifica Consultorio "
                            If Not currentRow Is Nothing Then

                                Dim codiceConsultorioPaziente As String = String.Empty

                                Dim objCons As Object = currentTableEncoder.getValOf(currentRow, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_CNS_CODICE", "T_PAZ_PAZIENTI", True)
                                If Not IsDBNull(objCons) Then
                                    codiceConsultorioPaziente = objCons.ToString()
                                End If

                                If CodConsVaccinale <> codiceConsultorioPaziente Then

                                    Using bizPaziente As BizPaziente = BizFactory.Instance.CreateBizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), New BizLogOptions(DataLogStructure.TipiArgomento.PAZIENTI, False))

                                        Dim dataAssegnazione As DateTime = DateTime.Now
                                        If txtDataAssegnazione.Data > DateTime.MinValue Then
                                            dataAssegnazione = Convert.ToDateTime(txtDataAssegnazione.Data)
                                        End If

                                        Dim command As New BizPaziente.ModificaConsultorioPazienteCommand()
                                        command.CodicePaziente = Convert.ToInt64(OnVacUtility.Variabili.PazId)
                                        command.CodiceConsultorioNew = codiceConsultorioPaziente
                                        command.CodiceConsultorioOld = CodConsVaccinale
                                        command.DataAssegnazioneConsultorio = dataAssegnazione
                                        command.FlagInvioCartella = False
                                        command.FlagMovimentoAutomaticoPassaggioAdulti = False
                                        command.DataEliminazione = DateTime.Now
                                        command.NoteEliminazione = "Eliminazione appuntamento per variazione centro vaccinale da maschera Gestione Pazienti"
                                        command.UpdateConsultoriAnagraficaPaziente = Biz.BizPaziente.UpdateConsultoriAnagraficaPazienteType.NoUpdateConsultori
                                        command.UpdateConvocazioniSoloConsultorioOld = True

                                        bizPaziente.ModificaConsultorioPaziente(command)

                                    End Using

                                End If
                            End If

#End Region

                        End If

                    End If

                    ' -----------------------------------
                    ' Step 5: Salvataggio delle malattie in centrale
                    If success AndAlso scritturaCentrale AndAlso Me.Settings.TIPOANAG_MALATTIE = Enumerators.TipoAnags.CentraleLettScritt AndAlso Not isAllineaLocaleCentrale Then

                        Using bizPazienteCentrale As New BizPazienteCentrale(OnVacContext.CreateBizContextInfos(), Nothing)

                            Dim pazCodiceAusiliario As String = GetCodiceAusiliario(currentRow)
                            bizPazienteCentrale.SalvaMalattie(pazCodiceAusiliario, gestionePazientiDatiSanitari.dtaMalattie)

                        End Using

                    End If

                    Dim pazienteCorrente As Paziente = Nothing

                    If success Then
                        pazienteCorrente = genericProvider.Paziente.GetPazienti(OnVacUtility.Variabili.PazId, OnVacContext.CodiceUslCorrente)(0)
                    End If

                    ' -----------------------------------
                    ' [Unificazione Ulss]: NON PIU' NECESSARIO => FlagConsensoVaccUslCorrente è sempre false
#Region " DA CANC "
                    ' Step 6: Update stato acquisizione se necessario
                    'If success AndAlso Me.Settings.TIPOANAG = Enumerators.TipoAnags.CentraleLettScritt AndAlso Not isAllineaLocaleCentrale AndAlso isCentraleNew Then

                    '    ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
                    '    If FlagConsensoVaccUslCorrente Then

                    '        pazienteCorrente.StatoAcquisizioneDatiVaccinaliCentrale = Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Vuota

                    '        genericProvider.Paziente.ModificaPaziente(pazienteCorrente)

                    '    End If

                    'End If
#End Region

                    ' -----------------------------------
                    ' Step 7: Gestione delle integrazioni
                    If success AndAlso Not isAllineaLocaleCentrale Then

                        ' Aggiungo le malattie per la propagazione 
                        pazienteCorrente.Malattie = gestionePazientiDatiSanitari.dtaMalattie

#Region " DA CANC "
                        ' [Unificazione Ulss]: DA VERIFICARE CON SOLO 050500 => ci sono solo un locale e il centrale, gestiti entrambi dal pannello

                        ' N.B. necessario in caso di gestione multi-usl (ex: veneto)
                        ' If Settings.TIPOANAG = Enumerators.TipoAnags.CentraleLettScritt Then

                        '   Using bizPaziente As BizPaziente = BizFactory.Instance.CreateBizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), New BizLogOptions(Log.DataLogStructure.TipiArgomento.PAZIENTI))

                        '       N.B. L'inserimento avviene in ogni db specificato nella tabella t_usl_gestite.
                        '            Per l'inserimento utilizza i metodi del biz custom specificato nel parametro BIZ_PAZIENTE_TYPE.
                        '            Se il parametro è null, utilizza la biz di default, che non gestisce l'inserimento/modifica
                        '            su db diversi ma sempre su quello corrente. Questo genera un errore nel caso in cui si tenti 
                        '            di inserire un paziente con il parametro BIZ_PAZIENTE_TYPE valorizzato a null e uno o più
                        '            record presenti nella t_usl_gestite (configurazione non prevista).

                        '       N.B. [Unificazione Ulss]: finchè c'erano anche le vecchie ulss non unificate, questo andava bene. 
                        '                                 Adesso che non ce ne sono più, e c'è solo la 050, questo è dannoso:
                        '                                 ad esempio, ricalcola lo stato anagrafico, sovrascrivendo quello eventualmente modificato da maschera
                        '       If bizPaziente.UslGestite.Count > 0 Then

                        '          Dim bizPazienteResult As BizResult

                        '          If pazientePrecedente Is Nothing Then

                        '              Dim inserisciPazienteCommand As New BizPaziente.InserisciPazienteCommand()
                        '              inserisciPazienteCommand.Paziente = pazienteCorrente
                        '              inserisciPazienteCommand.FromVAC = True

                        '              bizPazienteResult = bizPaziente.InserisciPaziente(inserisciPazienteCommand)

                        '          Else

                        '              Dim modificaPazienteCommand As New BizPaziente.ModificaPazienteCommand()
                        '              modificaPazienteCommand.Paziente = pazienteCorrente
                        '              modificaPazienteCommand.FromVAC = True

                        '              bizPazienteResult = bizPaziente.ModificaPaziente(modificaPazienteCommand)

                        '          End If

                        '          success = bizPazienteResult.Success

                        '          If Not success Then
                        '              Dim errorMessagePattern As String = String.Format("Errore {0} Paziente [1]:{2}{{0}}", IIf(pazientePrecedente Is Nothing, "Inserimento", "Modifica"), pazienteCorrente.Paz_Codice, Environment.NewLine)
                        '              Me.GestionePazientiMessage.Console.StatusMessage(String.Format(errorMessagePattern, bizPazienteResult.Messages.ToHtmlString(), WarningMessage.MessageType.ErrorMessage))
                        '          End If

                        '       End If

                        '   End Using

                        ' End If
#End Region
                        If success Then

                            ' ***  Invio messaggi esterni *** '
                            If IsPazienteNew Then
                                OnVacMidSendManager.InserisciPaziente(pazienteCorrente)
                            Else
                                OnVacMidSendManager.ModificaPaziente(pazienteCorrente, pazientePrecedente)
                            End If
                            ' ********************************* '

                        End If

                    End If

                End Using
            End Using

            transactionScope.Complete()

        End Using

        ' -----------------------------------
        ' Step 8: Fuori dallo scope della transaction
        If success Then

            'riabilitazione LeftBar in seguito a salvataggio
            LoadLeftFrame = Not OnVacUtility.IsPazIdEmpty()

            ' Se tutto è andato a buon fine, la maschera non deve più essere in modalità di modifica
            StatoCorrentePagina = StatoPagina.VIEW

            ' Se il consenso è gestito, controlla se è bloccante oppure no.
            ' In caso sia bloccante, disabilita la left e i pulsanti della maschera
            Dim result As ManageConsensoResult = ManageLayoutConsenso(False)
            If result.IsConsensoBloccante.HasValue AndAlso result.IsConsensoBloccante.Value Then
                LoadLeftFrame = result.LoadLeft
                StatoCorrentePagina = result.StatoPagina
            End If

            If LoadLeftFrame Then RefreshLeftFrame = True

            ' Solo in caso di inserimento, replico la gestione dei messaggi relativi al consenso (come al page_load)
            If isLocaleNew Then
                GestionePazientiMessage.ClearWarning()
                ShowAlertConsenso(result.StatoConsenso)
            End If

            ' --- Salvataggio OK --- '
            If isAllineaLocaleCentrale Then
                ' GestionePazientiMessage.Console.StatusMessage("Allineamento con centrale eseguito con successo.", WarningMessage.MessageType.InfoMessage)
            Else
                GestionePazientiMessage.Console.StatusMessage("Salvataggio eseguito con successo.", WarningMessage.MessageType.InfoMessage)
            End If

        Else

            If pazientePrecedente Is Nothing Then
                OnVacUtility.ClearPazId()
                ' TODO [GestionePazienti]: (chiedere a DESI !!) ripristino del datapanel altrimenti rimangono i valori di join con il centrale
                currentTableEncoder.setValOf(currentRow, DBNull.Value, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_CODICE", "T_PAZ_PAZIENTI")
            Else
                OnVacUtility.Variabili.PazId = pazientePrecedente.Paz_Codice

                ' TODO [GestionePazienti]: controllare se paz_codice_ausiliario
                UltimoPazienteSelezionato = New UltimoPazienteSelezionato(String.Empty, OnVacUtility.Variabili.PazId)

            End If

        End If

        Using dam As IDAM = OnVacUtility.OpenDam()
            ReloadAll(dam)
        End Using

        Return success

    End Function

    Private Sub CreaLogPazientiScambiati(ex As PazientiScambiatiException)

        Dim msg As New StringBuilder("Pazienti scambiati da maschera GestionePazienti")

        msg.AppendLine(ex.MessaggioPersonalizzato)

        msg.AppendLine()
        msg.AppendLine("Paziente Originale pre-salvataggio")

        If ex.PazienteOriginale Is Nothing Then
            msg.AppendLine("Nessuno")
        Else
            CreaMessaggioDatiPaziente(ex.PazienteOriginale, msg)
        End If

        msg.AppendLine()
        msg.AppendLine("Paziente Modificato (non salvato)")

        If ex.PazienteModificato Is Nothing Then
            msg.AppendLine("Nessuno")
        Else
            CreaMessaggioDatiPaziente(ex.PazienteModificato, msg)
        End If

        msg.AppendLine()
        msg.AppendFormat("Utente: {0} [{1}]", OnVacContext.UserCode, OnVacContext.UserId.ToString())

        Common.Utility.EventLogHelper.EventLogWrite(ex, msg.ToString(), Diagnostics.EventLogEntryType.Information, OnVacContext.AppId)

    End Sub

    Private Sub CreaMessaggioDatiPaziente(paziente As Paziente, msg As StringBuilder)

        msg.AppendFormat("Codice locale: {0}", paziente.Paz_Codice.ToString()).AppendLine()
        msg.AppendFormat("Codice ausiliario: {0}", paziente.CodiceAusiliario).AppendLine()
        msg.AppendFormat("Cognome: {0}", paziente.PAZ_COGNOME).AppendLine()
        msg.AppendFormat("Nome: {0}", paziente.PAZ_NOME).AppendLine()
        msg.AppendFormat("Sesso: {0}", paziente.Sesso).AppendLine()
        msg.AppendFormat("Comune nascita: {0}", paziente.ComuneNascita_Codice).AppendLine()
        msg.AppendFormat("Data di nascita: {0:dd/MM/yyyy}", paziente.Data_Nascita).AppendLine()
        msg.AppendFormat("Codice fiscale: {0}", paziente.PAZ_CODICE_FISCALE).AppendLine()

    End Sub

    Private Function GetPanelRowStringValue(panelRow As DataRow, panelEncoder As OnitDataPanel.FieldsEncoder, nomeCampo As String) As String

        Dim obj As Object = panelEncoder.getValOf(panelRow, Constants.ConnessioneAnagrafe.LOCALE, nomeCampo, "t_paz_pazienti", False)

        If obj Is Nothing OrElse obj Is DBNull.Value Then
            Return String.Empty
        End If

        Return obj.ToString()

    End Function

    Private Function GetPanelRowDateTimeValue(panelRow As DataRow, panelEncoder As OnitDataPanel.FieldsEncoder, nomeCampo As String) As DateTime

        Dim dateValue As DateTime

        Dim obj As Object = panelEncoder.getValOf(panelRow, Constants.ConnessioneAnagrafe.LOCALE, nomeCampo, "t_paz_pazienti", False)

        If obj Is Nothing OrElse obj Is DBNull.Value Then
            dateValue = DateTime.MinValue
        Else
            dateValue = Convert.ToDateTime(obj)
        End If

        Return dateValue.Date

    End Function

    ''' gestione del paz locale
    Private Sub odpDettaglioPaziente_beforeSaveRecord(sender As OnitDataPanel.OnitDataPanel, row As DataRow, encoder As OnitDataPanel.FieldsEncoder, dam As Object) Handles odpDettaglioPaziente.beforeSaveRecord

        ' Controllo della presenza di una transazione
        If Transactions.Transaction.Current Is Nothing Then Throw New ApplicationException("TransactionScope non presente")

        'testo se si tratta di un inserimento o meno in locale.
        'per farlo guardo se il valore del campo chiave della tabella locale è nullo   
        Dim isInsert As Boolean = encoder.getValOf(row, Constants.ConnessioneAnagrafe.LOCALE, "paz_codice", "t_paz_pazienti", False) Is DBNull.Value

        If isInsert Then

            encoder.setValOf(row, Date.Now, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_DATA_INSERIMENTO", "T_PAZ_PAZIENTI")
            encoder.setValOf(row, 0, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_LIVELLO_CERTIFICAZIONE", "T_PAZ_PAZIENTI")

        Else
            ' --------- '
            ' TODO [PazScambiati]: gestire blocco del salvataggio (DA FINIRE)

            '' TODO [PazScambiati]: tutto sotto parametro?

            'Dim pazienteOriginale As Paziente = Nothing

            'Dim codiceLocalePaziente As Long? = Nothing

            'Dim obj As Object = encoder.getValOf(row, "paz_codice", "t_paz_pazienti", False)

            'If obj IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(obj) Then

            '    codiceLocalePaziente = Convert.ToInt64(obj)

            '    Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            '        Using bizPaziente As New BizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

            '            pazienteOriginale = bizPaziente.CercaPaziente(codiceLocalePaziente)

            '        End Using
            '    End Using

            'End If

            '' Controllo sovrascrittura => solo se non siamo in inserimento!
            'If Not OnVacUtility.IsPazIdEmpty() AndAlso codiceLocalePaziente.HasValue AndAlso pazienteOriginale IsNot Nothing Then

            '    ' TODO [PazScambiati]: controllo dati originali e dati da salvare. 
            '    '                      Sollevo eccezione se i dati fondamentali sono tutti diversi ma il codice paziente è lo stesso

            '    Dim throwException As Boolean = False
            '    Dim msg As String = String.Empty

            '    If codiceLocalePaziente.HasValue AndAlso codiceLocalePaziente.Value.ToString() <> OnVacUtility.Variabili.PazId Then

            '        ' Il codice letto dal pannello e quello memorizzato in maschera non coincidono => BLOCCO IL SALVATAGGIO!
            '        throwException = True
            '        msg = "OnVacUtility.Variabili.PazId <> paz_codice di getCurrentDataRow "

            '    End If

            '    Dim pazienteModificato As New Paziente()

            '    pazienteModificato.Paz_Codice = GetPanelRowStringValue(row, encoder, "PAZ_CODICE")
            '    pazienteModificato.PAZ_COGNOME = GetPanelRowStringValue(row, encoder, "PAZ_COGNOME")
            '    pazienteModificato.PAZ_NOME = GetPanelRowStringValue(row, encoder, "PAZ_NOME")
            '    pazienteModificato.Sesso = GetPanelRowStringValue(row, encoder, "PAZ_SESSO")
            '    pazienteModificato.Data_Nascita = GetPanelRowDateTimeValue(row, encoder, "PAZ_DATA_NASCITA")
            '    pazienteModificato.ComuneNascita_Codice = GetPanelRowStringValue(row, encoder, "PAZ_COM_CODICE_NASCITA")
            '    pazienteModificato.PAZ_CODICE_FISCALE = GetPanelRowStringValue(row, encoder, "PAZ_CODICE_FISCALE")
            '    pazienteModificato.CodiceAusiliario = GetPanelRowStringValue(row, encoder, "PAZ_CODICE_AUSILIARIO")

            '    If pazienteOriginale.Paz_Codice = pazienteModificato.Paz_Codice AndAlso
            '            pazienteOriginale.PAZ_COGNOME <> pazienteModificato.PAZ_COGNOME AndAlso
            '            pazienteOriginale.PAZ_NOME <> pazienteModificato.PAZ_NOME AndAlso
            '            pazienteOriginale.Sesso <> pazienteModificato.Sesso AndAlso
            '            pazienteOriginale.Data_Nascita.Date <> pazienteModificato.Data_Nascita.Date AndAlso
            '            pazienteOriginale.ComuneNascita_Codice <> pazienteModificato.ComuneNascita_Codice Then

            '        throwException = True
            '        msg = "Il Paziente modificato ha stesso codice ma dati fondamentali diversi da quello originale"

            '    End If

            '    If throwException Then
            '        Throw New PazientiScambiatiException(pazienteOriginale, pazienteModificato, msg)
            '    End If

            'End If

        End If

        'esegue la valorizzazione del paz_tipo in modalità provvisoria
        If Settings.LOCALE_PROVVISORIO Then

            If sender.Connections(Constants.ConnessioneAnagrafe.LOCALE).WriteAuth <> OnitDataPanel.Connection.writeAuthorizations.none AndAlso
               sender.Connections(Constants.ConnessioneAnagrafe.CENTRALE).WriteAuth = OnitDataPanel.Connection.writeAuthorizations.none Then

                ' Versione precedente di paz_locale
                Dim oldPazLocale As Object = encoder.getValOf(row, Constants.ConnessioneAnagrafe.LOCALE, "paz_locale", "t_paz_pazienti", False)
                If oldPazLocale Is Nothing Then Return

                Dim strOldPazLocale As String = oldPazLocale.ToString().Trim()
                If strOldPazLocale = String.Empty Then   'variazione
                    'aggiorno il valore 
                    encoder.setValOf(row, IIf(isInsert, "I", "V"), Constants.ConnessioneAnagrafe.LOCALE, "paz_locale", "t_paz_pazienti")
                Else
                    ' Valore corrente di paz_locale
                    Dim newPazLocale As Object = encoder.getValOf(row, Constants.ConnessioneAnagrafe.LOCALE, "paz_locale", "t_paz_pazienti", True)

                    Dim strNewPazLocale As String
                    If newPazLocale Is Nothing Then
                        strNewPazLocale = String.Empty
                    Else
                        strNewPazLocale = newPazLocale.ToString().Trim()
                    End If

                    ' Controllo se il valore corrente è diverso dal precedente.
                    ' Siccome il valore precedente era impostato (<>""), non deve essere sovrascritto.
                    If strNewPazLocale <> strOldPazLocale Then
                        encoder.setValOf(row, strOldPazLocale, Constants.ConnessioneAnagrafe.LOCALE, "paz_locale", "t_paz_pazienti")
                    End If

                End If

            End If

        End If

        If ScritturaCentrale(row) Then

            Dim ente As String = OnVacContext.AppId
            Dim addetto As String = OnVacContext.UserCode
            Dim errorMessage As String = Nothing

            ' Salvataggio in centrale
            ' Se il parametro CENTRALE_WS_XMPI = "S", utilizza un web service specificato nel parametro UrlXmpiService del web.config.
            ' Altrimenti, esegue una query direttamente sul database centrale (recuperato in base alla stringa di connessione dell'applicativo APP_ID_CENTRALE)
            Dim centrale As New PazienteCentraleWrapper()

            Dim odpError As OnitDataPanel.OnitDataPanelError =
                centrale.SalvaAnagrafica(row, encoder, sender.MainTables(Constants.ConnessioneAnagrafe.CENTRALE),
                                         Constants.ConnessioneAnagrafe.CENTRALE, ente, addetto, errorMessage)

            If Not odpError.ok Then
                Throw New ApplicationException(errorMessage)
            End If

        End If

    End Sub

    Private Sub odpDettaglioPaziente_beforeLoadData(sender As OnitDataPanel.OnitDataPanel) Handles odpDettaglioPaziente.beforeLoadData

        'imposto il pannello a seconda dei parametri dell'applicativo
        ' --> impostazione dei diritti di lettura e scrittura delel connessioni
        Select Case Settings.TIPOANAG

            Case Enumerators.TipoAnags.SoloLocale 'ok

                ImpostaCentrale(OnitDataPanel.Connection.readAuthorizations.none, OnitDataPanel.Connection.writeAuthorizations.none)

            Case Enumerators.TipoAnags.CentraleLettura

                ImpostaCentrale(OnitDataPanel.Connection.readAuthorizations.read, OnitDataPanel.Connection.writeAuthorizations.none)

            Case Enumerators.TipoAnags.CentraleLettScritt '...

                ImpostaCentrale(OnitDataPanel.Connection.readAuthorizations.read, OnitDataPanel.Connection.writeAuthorizations.none)

            Case Enumerators.TipoAnags.CentraleScrittInsLettAgg

                ImpostaCentrale(OnitDataPanel.Connection.readAuthorizations.read, OnitDataPanel.Connection.writeAuthorizations.none)

            Case Enumerators.TipoAnags.CentraleLettLocaleAgg 'previsto per rimini

                If blnSceltoLocale Then
                    ImpostaCentrale(OnitDataPanel.Connection.readAuthorizations.none, OnitDataPanel.Connection.writeAuthorizations.none)
                Else
                    ImpostaCentrale(OnitDataPanel.Connection.readAuthorizations.read, OnitDataPanel.Connection.writeAuthorizations.none)
                End If

            Case Else

                Throw New NotImplementedException("odpDettaglioPaziente_beforeLoadData: TIPOANAG non supportato")

        End Select

        ' --> impostazione dei parametri delle connessioni di tipo servizio
        ImpostaParametriServizio()

    End Sub

    Private Sub odpDettaglioPaziente_OnExecuteDetailOperation(sender As OnitDataPanel.OnitDataPanel, op As OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpDettaglioPaziente.OnExecuteDetailOperation

        Dim strError As String = String.Empty
        Dim numErrori As Integer = 0

        If op = OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord Then

            'inserimento da ricerca paziente

            If Not txtComuneDiNascita Is Nothing AndAlso txtComuneDiNascita.Codice.Trim() <> "" AndAlso txtComuneDiNascita.Descrizione.Trim() = "" Then

                'riallinea la descrizione col codice
                txtComuneDiNascita.RefreshDataBind()

            End If

            'controllo del cognome e del nome appena inserito

            If Not txtCognome Is Nothing Then

                Dim txt As TextBox = CType(txtCognome, TextBox)
                Dim strTempCog As String = RicPazUtility.fullTrim(txt.Text)

                If Not RicPazUtility.CognomeNomeValido(strTempCog) Then
                    strError = "Il cognome '" + txt.Text + "' "
                    numErrori += 1
                End If

                txt.Text = RicPazUtility.validaCognomeNome(strTempCog)

            End If

            If Not txtNome Is Nothing Then

                Dim txt As System.Web.UI.WebControls.TextBox = CType(txtNome, System.Web.UI.WebControls.TextBox)
                Dim strTempCog As String = RicPazUtility.fullTrim(txt.Text)

                If Not RicPazUtility.CognomeNomeValido(strTempCog) Then
                    strError += IIf(numErrori = 0, "Il", " e il") + " cognome '" + txt.Text + "' "
                    numErrori += 1
                End If

                txt.Text = RicPazUtility.validaCognomeNome(strTempCog)

            End If

            ' risposta 
            If numErrori > 0 Then
                strError += IIf(numErrori = 1, " contiene ", " contengono ") + "caratteri non validi che sono stati eliminati"
                AlertClientMsg(strError)
            End If

        End If

    End Sub

    ''' impostazione del controllo per la tessera sanitaria 
    Private Sub odpDettaglioPaziente_controlLayoutChanging(sender As OnitDataPanel.OnitDataPanel, operation As OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpDettaglioPaziente.controlLayoutChanging

        'indica se la tessera è modificabile o meno
        If Not txtTesseraSanitaria Is Nothing Then

            If Settings.TESSCEN Then
                CType(txtTesseraSanitaria, TextBox).Enabled = False
            Else
                CType(txtTesseraSanitaria, TextBox).Enabled = True
            End If

        End If

    End Sub

    Private Sub odpDettaglioPaziente_afterOperation(sender As OnitDataPanel.OnitDataPanel, operation As OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpDettaglioPaziente.afterOperation

        Select Case operation

            Case OnitDataPanel.OnitDataPanel.CurrentOperationTypes.EditRecord

                PazRegolarizzato()

        End Select

    End Sub

    Private Sub odpDettaglioPaziente_afterUpdateWzControls(sender As OnitDataPanel.OnitDataPanel) Handles odpDettaglioPaziente.afterUpdateWzControls

        Dim pazRow As DataRow = sender.getCurrentDataRow()

        If Not pazRow Is Nothing Then

            'Scrive i giorni disponibili (sono considerati come sequenze di bit!)
            Dim strGiorno As String = ""
            Dim objGiorno As Object = sender.GetCurrentTableEncoder().getValOf(pazRow, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_GIORNO", "T_PAZ_PAZIENTI", True)
            If Not objGiorno Is Nothing Then
                strGiorno = objGiorno.ToString()
            End If

            'per rimanere compatibile con la soluzione precedente ho dovuto continuare a cifrare/decifrare
            'il valore nella strGiorno: si tratta di un sequenza binaria di 8 cifre che mi dicono quali
            'check sono cekkati (0=nonCekkato, 1 = cekkato)
            Dim state As Boolean

            For i As Int16 = 0 To 6

                If (Val(strGiorno) And CInt(2 ^ i)) = CInt(2 ^ i) Then
                    state = True
                Else
                    state = False
                End If

                Select Case i
                    Case 1
                        chkLunedi.Checked = state
                    Case 2
                        chkMartedi.Checked = state
                    Case 3
                        chkMercoledi.Checked = state
                    Case 4
                        chkGiovedi.Checked = state
                    Case 5
                        chkVenerdi.Checked = state
                    Case 6
                        chkSabato.Checked = state
                    Case 0
                        chkDomenica.Checked = state
                End Select

            Next

            ' Leggo la categoria di rischio dal locale
            If Settings.TIPOANAG_CATEGORIA_RISCHIO <> Enumerators.TipoAnags.CentraleLettScritt Then

                txtCategorieRischio.Codice = String.Empty
                txtCategorieRischio.Descrizione = String.Empty
                txtCategorieRischio.RefreshDataBind()

                Dim objCategoriaRischioLocale As Object = sender.GetCurrentTableEncoder().getValOf(pazRow, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_RSC_CODICE", "T_PAZ_PAZIENTI", True)
                If Not objCategoriaRischioLocale Is Nothing AndAlso Not objCategoriaRischioLocale Is DBNull.Value Then
                    txtCategorieRischio.Codice = objCategoriaRischioLocale.ToString()
                    txtCategorieRischio.RefreshDataBind()
                End If

            End If

        End If

    End Sub

    Private Sub odpDettaglioPaziente_onCreateNewRow(sender As OnitDataPanel.OnitDataPanel, row As DataRow, encoder As OnitDataPanel.FieldsEncoder, duplicated As Integer) Handles odpDettaglioPaziente.onCreateNewRow

        ' Impostazione campo regolarizzato
        encoder.setValOf(row, "S", Constants.ConnessioneAnagrafe.LOCALE, "paz_regolarizzato", "t_paz_pazienti")

        ' Impostazione stato vaccinale
        Dim statoVaccInCorso As Int16 = Enumerators.StatiVaccinali.InCorso
        row.Item("PAZ_STATO") = statoVaccInCorso.ToString()

    End Sub

#End Region

#Region " Indirizzo Codificato "

    ' Impostazione degli indirizzi negli user control
    Private Sub GestioneIndirizzi(dam As IDAM, ByRef currentRow As DataRow)

        SalvaTipoIndirizzo = Enumerators.TipoIndirizzo.Nessuno

        Me.viaDomicilio.Tipo = Enumerators.TipoIndirizzo.Domicilio
        Me.viaDomicilio.PazCodice = OnVacUtility.Variabili.PazId
        Me.viaDomicilio.PazCircoscrizioneCodice = Me.txtCircoscrizione2.Codice
        Me.viaDomicilio.PazCircoscrizioneDescrizione = Me.txtCircoscrizione2.Descrizione

        If Me.Settings.TIPOANAG <> Onit.OnAssistnet.OnVac.Enumerators.TipoAnags.SoloLocale Then
            Me.viaDomicilio.ViaCentrale = odpDettaglioPaziente.GetCurrentTableEncoder().getValOf(currentRow, Constants.ConnessioneAnagrafe.CENTRALE, "PAZ_INDIRIZZO_DOMICILIO", "T_PAZ_PAZIENTI_CENTRALE", True).ToString()
            Me.viaDomicilio.InCentrale = True
        End If

        Me.viaDomicilio.LoadViaDB(dam)

        'se non è impostata la gestione delle vie codificate, deve recuperare il valore vecchio degli indirizzi 
        'nel caso in cui si lavori in centrale, la gestione delle vie non è contemplata [modifica 13/09/2006]
        If (Me.Settings.TIPOANAG <> Onit.OnAssistnet.OnVac.Enumerators.TipoAnags.SoloLocale) Or (Not Me.Settings.GESVIE) Then
            Me.viaDomicilio.ValoreVecchioDomicilio = viaDomicilio.IndirizzoCorrente.Libero
        End If

        Me.viaResidenza.Tipo = Enumerators.TipoIndirizzo.Residenza
        Me.viaResidenza.PazCodice = OnVacUtility.Variabili.PazId
        Me.viaResidenza.PazCircoscrizioneCodice = txtCircoscrizione.Codice
        Me.viaResidenza.PazCircoscrizioneDescrizione = txtCircoscrizione.Descrizione

        If Me.Settings.TIPOANAG <> Onit.OnAssistnet.OnVac.Enumerators.TipoAnags.SoloLocale Then
            Me.viaResidenza.ViaCentrale = odpDettaglioPaziente.GetCurrentTableEncoder().getValOf(currentRow, Constants.ConnessioneAnagrafe.CENTRALE, "PAZ_INDIRIZZO_RESIDENZA", "T_PAZ_PAZIENTI_CENTRALE", True).ToString()
            Me.viaResidenza.InCentrale = True
        End If

        Me.viaResidenza.LoadViaDB(dam)

        'se non è impostata la gestione delle vie codificate, deve recuperare il valore vecchio degli indirizzi [modifica 13/09/2006]
        If (Me.Settings.TIPOANAG <> Onit.OnAssistnet.OnVac.Enumerators.TipoAnags.SoloLocale) Or (Not Me.Settings.GESVIE) Then
            Me.viaResidenza.ValoreVecchioResidenza = viaResidenza.IndirizzoCorrente.Libero
        End If

    End Sub

    Private Function RecordLogContainsCampo(recordLog As DataLogStructure.Record, nomeCampo As String) As Boolean

        For i As Integer = 0 To recordLog.Campi.Count - 1

            If recordLog.Campi(i).CampoDB = nomeCampo Then Return True

        Next

        Return False

    End Function

#Region " Eventi usercontrol GestioneVie "

    Private Sub viaResidenza_ConfermaModificaCircoscrizione(sender As Object, e As System.EventArgs) Handles viaResidenza.ConfermaModificaCircoscrizione

        Me.txtCircoscrizione.Codice = Me.viaResidenza.PazCircoscrizioneCodice
        Me.txtCircoscrizione.Descrizione = Me.viaResidenza.PazCircoscrizioneDescrizione
        Me.txtCircoscrizione.RefreshDataBind()

        Me.ModificaCircoscrizioneResidenza = True

    End Sub

    Private Sub viaDomicilio_ConfermaModificaCircoscrizione(sender As Object, e As System.EventArgs) Handles viaDomicilio.ConfermaModificaCircoscrizione

        Me.txtCircoscrizione2.Codice = Me.viaDomicilio.PazCircoscrizioneCodice
        Me.txtCircoscrizione2.Descrizione = Me.viaDomicilio.PazCircoscrizioneDescrizione
        Me.txtCircoscrizione2.RefreshDataBind()

        Me.ModificaCircoscrizioneDomicilio = True

    End Sub

    Private Sub viaResidenza_ShowDatiIndirizzo(sender As Object, e As GestioneVia.ShowDatiIndirizzoEventArgs) Handles viaResidenza.ShowDatiIndirizzo

        Me.viaResidenza.PazCircoscrizioneCodice = Me.txtCircoscrizione.Codice
        Me.viaResidenza.PazCircoscrizioneDescrizione = Me.txtCircoscrizione.Descrizione

    End Sub

    Private Sub viaDomicilio_ShowDatiIndirizzo(sender As Object, e As GestioneVia.ShowDatiIndirizzoEventArgs) Handles viaDomicilio.ShowDatiIndirizzo

        Me.viaDomicilio.PazCircoscrizioneCodice = Me.txtCircoscrizione2.Codice
        Me.viaDomicilio.PazCircoscrizioneDescrizione = Me.txtCircoscrizione2.Descrizione

    End Sub

#End Region

#End Region

#Region " Eventi CheckBox "

    Private Sub chkCancellato_CheckedChanged(sender As Object, e As System.EventArgs) Handles chkCancellato.CheckedChanged

        Me.DataObbligatoria("canc")

    End Sub

    Private Sub chkIrreperibile_CheckedChanged(sender As Object, e As System.EventArgs) Handles chkIrreperibile.CheckedChanged

        Me.DataObbligatoria("irrep")

    End Sub

    Private Sub chkOccasionale_CheckedChanged(sender As Object, e As System.EventArgs) Handles chkOccasionale.CheckedChanged

        ' Impostazione layout checkbox Occasionale
        Me.TipoOccasionale()

        ' Valorizzazione automatica campo stato anagrafico
        If Me.Settings.GES_AUTO_STATO_ANAGRAFICO Then

            If cmbStatoAnagrafico.SelectedItem.Text = Enumerators.StatoAnagrafico.OCCASIONALE.ToString() OrElse
               cmbStatoAnagrafico.SelectedItem.Text = Enumerators.StatoAnagrafico.RESIDENTE.ToString() OrElse
               cmbStatoAnagrafico.SelectedItem.Text = Enumerators.StatoAnagrafico.DOMICILIATO.ToString() OrElse
               cmbStatoAnagrafico.SelectedItem.Text = Enumerators.StatoAnagrafico.NON_RESIDENTE_NON_DOMICILIATO.ToString().Replace("_", " ") Then

                'Controllo il check Occasionale solo se sono in uno dei tre stati: Residente, Domiciliato, Non Residente Non Domiciliato
                If chkOccasionale.Checked Then

                    SelectStatoAnagraficoItem(cmbStatoAnagrafico.Items.FindByText(Enumerators.StatoAnagrafico.OCCASIONALE.ToString()))

                Else

                    Dim s As Enumerators.StatoAnagrafico? = CalcoloStatoAnagrafico(False, True)
                    If s.HasValue Then SelectStatoAnagraficoItem(cmbStatoAnagrafico.Items.FindByValue([Enum].Parse(GetType(Enumerators.StatoAnagrafico), s.ToString()))) 'SelectStatoAnagraficoItem(cmbStatoAnagrafico.Items.FindByText(s.Value.ToString().Replace("_", " ")))

                End If

            ElseIf chkOccasionale.Checked Then

                'Se sono in uno stato di priorità superiore e cerco di abilitare Occasionale devo mostrare un avviso
                OnitLayout21.InsertRoutineJS("alert(""Il campo occasionale non può essere abilitato, il paziente è " & cmbStatoAnagrafico.SelectedItem.Text.ToLower() & "."");")

                chkOccasionale.Checked = False

            End If

        End If

    End Sub

#End Region

#Region " Eventi wzFinestraModale "

    Private Sub txtSedeVaccinale_Change(sender As Object, e As OnitModalList.ModalListaEventArgument) Handles txtSedeVaccinale.Change

        Me.txtIndirizzoSedeVaccinale.Text = Me.txtSedeVaccinale.ValoriAltriCampi("INDIRIZZO")
        Me.txtSedeVaccinale.Descrizione = Me.txtSedeVaccinale.ValoriAltriCampi("DESCRIZIONE")
        Me.txtSedeVaccinale.Codice = Me.txtSedeVaccinale.ValoriAltriCampi("CODICE")

        ' Modifica automatica della sede vaccinale precedente
        If Me.txtSedeVaccinale.Codice <> String.Empty Then

            Dim codiceOld As String = String.Empty
            Dim descrizioneOld As String = String.Empty

            Dim drPazRow As DataRow = odpDettaglioPaziente.getCurrentDataRow()

            If Not drPazRow Is Nothing Then
                codiceOld = odpDettaglioPaziente.GetCurrentTableEncoder().getValOf(drPazRow, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_CNS_CODICE", "T_PAZ_PAZIENTI", True).ToString()
                descrizioneOld = odpDettaglioPaziente.GetCurrentTableEncoder().getValOf(drPazRow, Constants.ConnessioneAnagrafe.LOCALE, "CNS_DESCRIZIONE", "T_ANA_CONSULTORI", True).ToString()
            End If

            Me.txtSedeVaccinalePrec.Codice = codiceOld
            Me.txtSedeVaccinalePrec.Descrizione = descrizioneOld

        End If

        ' Modifica della data di assegnazione della sede vaccinale
        Me.txtDataAssegnazione.Data = Date.Now.Date

    End Sub

    Private Sub txtComuneResidenza_Change(sender As Object, e As OnitModalList.ModalListaEventArgument) Handles txtComuneResidenza.Change

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizComuni As New BizComuni(genericProvider, Settings, OnVacContext.CreateBizContextInfos, Nothing)

                ' Valorizzazione campo cap in base al comune selezionato.
                txtCapResidenza.Text = bizComuni.GetCapByCodiceComune(DirectCast(sender, OnitDataPanel.wzFinestraModale).Codice)

                ' Valorizzazione automatica campo stato anagrafico
                If Settings.GES_AUTO_STATO_ANAGRAFICO Then

                    Dim s As Enumerators.StatoAnagrafico? = CalcoloStatoAnagrafico(True, False)
                    If s.HasValue Then SelectStatoAnagraficoItem(cmbStatoAnagrafico.Items.FindByValue([Enum].Parse(GetType(Enumerators.StatoAnagrafico), s.ToString())))

                End If

                ' Controllo ed eventuale valorizzazione dell'usl di residenza in base al comune di residenza selezionato
                AllineaUslResidenza(genericProvider)

                Dim encoder As OnitDataPanel.FieldsEncoder = odpDettaglioPaziente.GetCurrentTableEncoder()
                Dim currentRow As DataRow = odpDettaglioPaziente.getCurrentDataRow()
                Dim encoderValue As Object = encoder.getValOf(currentRow, Constants.ConnessioneAnagrafe.CENTRALE, "PAZ_COM_CODICE_RESIDENZA", "t_paz_pazienti_centrale", False)

                Dim oldCodiceDb As String = String.Empty
                Dim oldDescrizioneDb As String = String.Empty

                If Not encoderValue Is Nothing Then oldCodiceDb = encoderValue.ToString()

                If oldCodiceDb <> txtComuneResidenza.Codice Then

                    Dim dataCorrente As DateTime = Date.Now.Date

                    ' Valorizzo la data di inizio residenza
                    If String.IsNullOrEmpty(txtComuneResidenza.Codice) Then
                        odpDataInizioResidenza.Data = Date.MinValue
                    Else
                        odpDataInizioResidenza.Data = dataCorrente
                    End If

                    odpDataFineResidenza.Data = Date.MinValue

                    ' [AVN - AURV]: immigrazione/emigrazione gestiti da AURV. Lasciamo la gestione automatica solo per i pazienti non regionali.
                    Dim codiceRegionale As Object = encoder.getValOf(currentRow, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_CODICE_REGIONALE", "T_PAZ_PAZIENTI", False)

                    If codiceRegionale Is Nothing OrElse codiceRegionale Is DBNull.Value OrElse String.IsNullOrWhiteSpace(codiceRegionale.ToString()) Then

                        ' Esegue la gestione automatica dei campi di immigrazione e emigrazione => solo per i pazienti che non hanno il codice MPI (regionale) valorizzato
                        Using pazienteBiz As BizPaziente = BizFactory.Instance.CreateBizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), New BizLogOptions(DataLogStructure.TipiArgomento.PAZIENTI, False))

                            Dim comune As Comune = bizComuni.GetComuneByCodice(oldCodiceDb)

                            If Not comune Is Nothing Then oldDescrizioneDb = comune.Descrizione

                            Dim uslEmigrazioneResult As BizPaziente.CalcolaUslEmigrazioneImmigrazioneResult =
                                pazienteBiz.CalcolaUslEmigrazioneImmigrazione(oldCodiceDb, oldDescrizioneDb, txtComuneResidenza.Codice, txtComuneResidenza.Descrizione)

                            If uslEmigrazioneResult.Modified Then

                                ' emigrazione
                                txtLuogoEmigrazione.Codice = uslEmigrazioneResult.LuogoEmigrazioneCodice
                                txtLuogoEmigrazione.Descrizione = uslEmigrazioneResult.LuogoEmigrazioneDescrizione
                                txtDataEmigrazione.Data = uslEmigrazioneResult.LuogoEmigrazioneData

                                ' immigrazione
                                txtLuogoImmigrazione.Codice = uslEmigrazioneResult.LuogoImmigrazioneCodice
                                txtLuogoImmigrazione.Descrizione = uslEmigrazioneResult.LuogoImmigrazioneDescrizione
                                txtDataImmigrazione.Data = uslEmigrazioneResult.LuogoImmigrazioneData

                            End If

                        End Using

                    End If
                End If

            End Using
        End Using

    End Sub

    Private Sub txtComuneDomicilio_Change(sender As Object, e As OnitModalList.ModalListaEventArgument) Handles txtComuneDomicilio.Change

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizComuni As New Biz.BizComuni(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)

                ' Valorizzazione campo cap in base al comune selezionato.
                txtCapDomicilio.Text = bizComuni.GetCapByCodiceComune(DirectCast(sender, OnitDataPanel.wzFinestraModale).Codice)

                ' Valorizzazione automatica campo stato anagrafico
                If Me.Settings.GES_AUTO_STATO_ANAGRAFICO Then
                    Dim s As Enumerators.StatoAnagrafico? = CalcoloStatoAnagrafico(False, False)
                    If s.HasValue Then SelectStatoAnagraficoItem(cmbStatoAnagrafico.Items.FindByValue([Enum].Parse(GetType(Enumerators.StatoAnagrafico), s.ToString()))) 'SelectStatoAnagraficoItem(cmbStatoAnagrafico.Items.FindByText(s.ToString().Replace("_", " ")))
                End If

                ' Gestione delle date di inizio e fine
                If e.OldCodice <> txtComuneDomicilio.Codice Then

                    Dim dataCorrente As DateTime = Date.Now.Date

                    ' Valorizzo la data di inizio domicilio
                    If String.IsNullOrEmpty(txtComuneDomicilio.Codice) Then
                        odpDataInizioDomicilio.Data = Date.MinValue
                    Else
                        odpDataInizioDomicilio.Data = dataCorrente
                    End If
                    odpDataFineDomicilio.Data = Date.MinValue

                End If

            End Using
        End Using

    End Sub

    Private Function CalcoloStatoAnagrafico(modificaResidenza As Boolean, modificaOccasionale As Boolean) As Enumerators.StatoAnagrafico?

        Dim s As Enumerators.StatoAnagrafico? = Nothing

        If (modificaOccasionale And cmbStatoAnagrafico.SelectedItem.Text = Enumerators.StatoAnagrafico.OCCASIONALE.ToString()) OrElse
           (modificaResidenza And cmbStatoAnagrafico.SelectedItem.Text = Enumerators.StatoAnagrafico.RESIDENTE.ToString()) OrElse
           cmbStatoAnagrafico.SelectedItem.Text = Enumerators.StatoAnagrafico.DOMICILIATO.ToString() OrElse
           cmbStatoAnagrafico.SelectedItem.Text = Enumerators.StatoAnagrafico.NON_RESIDENTE_NON_DOMICILIATO.ToString().Replace("_", " ") Then

            Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                Using bizPaz As New BizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos, Nothing)

                    Dim paziente As New Paziente() With {
                        .ComuneDomicilio_Codice = txtComuneDomicilio.Codice,
                        .ComuneResidenza_Codice = txtComuneResidenza.Codice,
                        .UslAssistenza_Codice = txtUSLDiAssistenza.Codice
                    }
                    s = bizPaz.CalcolaStatoAnagrafico(paziente)

                End Using
            End Using

        End If

        Return s

    End Function

    ' Ogni volta che viene richiamata la modale, imposto il filtro 
    ' per tener conto di eventuali cambiamenti della data di nascita
    Private Sub txtSedeVaccinale_SetUpFiletr(sender As Object) Handles txtSedeVaccinale.SetUpFiletr

        Dim giorniEtaPaz As Integer = 0

        If Me.txtDataNascita.Text <> "" Then
            giorniEtaPaz = Common.Utility.PazienteHelper.CalcoloEta(Me.txtDataNascita.Data).GiorniTotali
        End If

        Dim filtro As New System.Text.StringBuilder()

        filtro.Append("cns_data_apertura <= SYSDATE ")
        filtro.Append("AND (cns_data_chiusura > SYSDATE OR cns_data_chiusura IS NULL) ")
        filtro.AppendFormat("AND (cns_da_eta <= {0} AND cns_a_eta >= {0}) ", giorniEtaPaz.ToString())
        filtro.Append("ORDER BY Descrizione")

        Me.txtSedeVaccinale.Filtro = filtro.ToString()

    End Sub

    Private Sub txtComuneDiNascita_Change(sender As Object, e As Controls.OnitModalList.ModalListaEventArgument) Handles txtComuneDiNascita.Change

        Me.txtCatastaleNas.Text = String.Empty

        Dim valAltriCampi As String() = Me.txtComuneDiNascita.ValAltriCampi.Split("|")

        If Not valAltriCampi Is Nothing AndAlso valAltriCampi.Length >= 2 Then
            Me.txtCatastaleNas.Text = valAltriCampi(2)
        End If

    End Sub

#Region " Impostazione filtri sulle modali dei comuni "

    ''' <summary>
    ''' Il filtro delle varie ModalList dei comuni è impostato in due modi, a seconda del parametro CHECKVALCOMUNI:
    ''' se vale True ("S" su db), si filtra in base alle date di validità e all'obsolescenza (com_obsoleto = N o null)
    ''' se vale False ("N"), si filtra in base al campo com_scadenza (che deve essere null o N)
    ''' Filtro delle ModalList dei comuni (escluso quello di nascita) nel caso non debba essere controllata la validità.
    ''' </summary>
    Private Const FiltroComuneNoCheckValidita As String = "(COM_SCADENZA IS NULL OR COM_SCADENZA = 'N') ORDER BY COM_DESCRIZIONE"

    Private Sub txtComuneDiNascita_SetUpFiletr(sender As Object) Handles txtComuneDiNascita.SetUpFiletr

        If Me.Settings.CHECKVALCOMUNI Then
            Me.txtComuneDiNascita.Filtro = FiltroModalListComune(txtDataNascita.Data)
        Else
            Me.txtComuneDiNascita.Filtro = "(COM_SCADENZA IS NULL OR COM_SCADENZA = 'N' OR COM_MOTIVO_SCADENZA='E') ORDER BY COM_DESCRIZIONE"
        End If

    End Sub

    Private Sub txtComuneDomicilio_SetUpFiletr(sender As Object) Handles txtComuneDomicilio.SetUpFiletr

        If Me.Settings.CHECKVALCOMUNI Then
            Me.txtComuneDomicilio.Filtro = FiltroModalListComune(Date.MinValue)
        Else
            Me.txtComuneDomicilio.Filtro = FiltroComuneNoCheckValidita
        End If

    End Sub

    Private Sub txtComuneResidenza_SetUpFiletr(sender As Object) Handles txtComuneResidenza.SetUpFiletr

        Dim stbFiltro As New System.Text.StringBuilder()

        ' Filtro sui comuni bloccati (se ci sono)
        If (Me.Settings.COM_RES_BLOCCATI.Trim() <> String.Empty AndAlso Me.Settings.COM_RES_BLOCCATI.Trim() <> ";") Then

            Dim filtro_comuni() As String = Me.Settings.COM_RES_BLOCCATI.Trim().TrimStart(";").TrimEnd(";").Split(";")

            For idx_filtro As Integer = 0 To filtro_comuni.Length - 1
                If filtro_comuni(idx_filtro).Trim <> "" Then
                    stbFiltro.AppendFormat(" com_codice not like '{0}%'  and ", filtro_comuni(idx_filtro))
                End If
            Next

        End If

        If Me.Settings.CHECKVALCOMUNI Then
            stbFiltro.AppendFormat(FiltroModalListComune(Date.MinValue))
        Else
            stbFiltro.AppendFormat(" (COM_SCADENZA IS NULL OR COM_SCADENZA = 'N') order by Descrizione")
        End If

        Me.txtComuneResidenza.Filtro = stbFiltro.ToString()

    End Sub

    Private Sub txtLuogoImmigrazione_SetUpFiletr(sender As Object) Handles txtLuogoImmigrazione.SetUpFiletr

        If Me.Settings.CHECKVALCOMUNI Then
            Me.txtLuogoImmigrazione.Filtro = FiltroModalListComune(Me.txtDataImmigrazione.Data)
        Else
            Me.txtLuogoImmigrazione.Filtro = FiltroComuneNoCheckValidita
        End If

    End Sub

    Private Sub txtLuogoEmigrazione_SetUpFiletr(sender As Object) Handles txtLuogoEmigrazione.SetUpFiletr

        If Me.Settings.CHECKVALCOMUNI Then
            Me.txtLuogoEmigrazione.Filtro = FiltroModalListComune(Me.txtDataEmigrazione.Data)
        Else
            Me.txtLuogoEmigrazione.Filtro = FiltroComuneNoCheckValidita
        End If

    End Sub

    ' Filtro utilizzato nelle ModalList per filtrare i comuni obsoleti e non validi.
    Private Function FiltroModalListComune(dataValidita As Date) As String

        Dim sqlDataValidita As String = String.Empty

        ' ---------------------- '
        ' TODO [GestionePazienti]: che schifezza... '
        ' ---------------------- '
        ' Apertura del dam per utilizzare le funzioni sulle date (è necessario aprire il dam altrimenti dà errore!) 
        Dim dam As IDAM = OnVacUtility.OpenDam()
        Try
            If (dataValidita = Date.MinValue) Then
                sqlDataValidita = dam.QB.FC.Now
            Else
                sqlDataValidita = dam.QB.FC.ToDate(dataValidita)
            End If
        Finally
            OnVacUtility.CloseDam(dam)
        End Try
        ' ---------------------- '

        Dim stbFiltro As New System.Text.StringBuilder()

        stbFiltro.AppendFormat("(COM_DATA_INIZIO_VALIDITA IS NULL OR COM_DATA_INIZIO_VALIDITA <= {0})", sqlDataValidita)
        stbFiltro.Append(" AND ")
        stbFiltro.AppendFormat("(COM_DATA_FINE_VALIDITA >= {0} OR COM_DATA_FINE_VALIDITA IS NULL)", sqlDataValidita)
        stbFiltro.Append(" AND ")
        stbFiltro.Append("(COM_OBSOLETO IS NULL OR COM_OBSOLETO = 'N')")
        stbFiltro.Append(" ORDER BY COM_DESCRIZIONE")

        Return stbFiltro.ToString()

    End Function

#End Region

#End Region

#Region " Gestione Consenso "

    Private Class ManageConsensoResult

        Public StatoConsenso As Consenso.StatoConsensoPaziente
        Public IsConsensoBloccante As Boolean?
        Public LoadLeft As Boolean?
        Public StatoPagina As StatoPagina?

        Public Sub New()
            StatoConsenso = Nothing
            IsConsensoBloccante = Nothing
            LoadLeft = Nothing
            StatoPagina = Nothing
        End Sub

    End Class

    ''' <summary>
    ''' Se il consenso non è gestito, nasconde la visualizzazione della relativa sezione.
    ''' Se è gestito, imposta i controlli in base al consenso globale. 
    ''' In più, controlla lo stato del consenso (bloccante, warning o non bloccante) e restituisce una struttra composta da:
    ''' un valore che indica se il consenso è bloccante, un flag che contiene il valore da impostare per la 
    ''' </summary>
    ''' <param name="isNewRecord"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ManageLayoutConsenso(isNewRecord As Boolean) As ManageConsensoResult

        Dim result As New ManageConsensoResult()

        ' La sezione del consenso è visibile solo se il consenso viene gestito.
        secConsenso.Visible = Settings.CONSENSO_GES

        ' Se la gestione del consenso non è attivata, evito di effettuare le query per impostare tutto il resto.
        If Settings.CONSENSO_GES AndAlso Not isNewRecord Then

            Dim consenso As Consenso.StatoConsensoPaziente =
                OnVacUtility.GetConsensoGlobalePaziente(GetCodiceAusiliarioPazienteFromCurrentRow(True), Settings)

            If Not consenso Is Nothing Then

                If Not String.IsNullOrEmpty(consenso.UrlIconaStatoConsenso) Then
                    imgStatoConsensoPaz.ImageUrl = consenso.UrlIconaStatoConsenso
                    imgStatoConsensoPaz.ToolTip = consenso.DescrizioneStatoConsenso
                End If

                If Not String.IsNullOrEmpty(consenso.DescrizioneStatoConsenso) Then
                    lblDescrStatoConsensoPaz.Text = consenso.DescrizioneStatoConsenso
                End If

                If consenso.Controllo = Enumerators.ControlloConsenso.Bloccante Then
                    result.IsConsensoBloccante = True
                    result.LoadLeft = False
                    result.StatoPagina = IIf(OnVacUtility.IsPazIdEmpty(), StatoPagina.VIEW, StatoPagina.BLOCCO_CONSENSO)
                Else
                    result.IsConsensoBloccante = False
                    result.LoadLeft = True
                    result.StatoPagina = StatoPagina.VIEW
                End If

                result.StatoConsenso = consenso

            End If
        End If

        Return result

    End Function

    Private Sub ShowAlertConsenso(consenso As Consenso.StatoConsensoPaziente)

        ' Se la gestione del consenso non è attivata, evito di effettuare le query per impostare tutto il resto.
        If Settings.CONSENSO_GES Then

            Dim codiceAusiliarioPaziente As String = GetCodiceAusiliarioPazienteFromCurrentRow(True)

            ' Se il valore del consenso non è stato specificato, lo calcolo
            If consenso Is Nothing Then consenso = OnVacUtility.GetConsensoGlobalePaziente(codiceAusiliarioPaziente, Settings)

            ' Se lo stato del consenso è warning  => devo visualizzare un messaggio
            ' Se è bloccante => devo visualizzare un messaggio e impedire l'accesso ai dati del paziente
            '                   in più, se il parametro CONSENSO_BLOCCANTE_AUTO_EDIT è true, devo aprire in edit la pop-up di rilevazione
            If Not consenso Is Nothing AndAlso Not String.IsNullOrEmpty(consenso.UrlIconaStatoConsenso) AndAlso consenso.Controllo <> Enumerators.ControlloConsenso.NonBloccante Then

                If OnVacUtility.IsPazIdEmpty() Then
                    '--
                    ' Messaggio per consenso
                    '--
                    SetMessageConsenso(consenso)
                Else
                    If consenso.Controllo = Enumerators.ControlloConsenso.Bloccante And Settings.CONSENSO_BLOCCANTE_AUTO_EDIT Then
                        '--
                        ' Messaggio all'utente e apertura pop-up rilevazione del consenso in edit (nell'evento AlertClick dell'OnitLayout)
                        '--
                        Dim consensoBloccanteMessage As String =
                            String.Format("{0}{1}Impossibile visualizzare i dati del paziente.{1}Verrà aperta la maschera di rilevazione del consenso.",
                                          consenso.DescrizioneStatoConsenso, Environment.NewLine)

                        OnitLayout21.ShowMsgBox(New PagesLayout.OnitLayout3.OnitLayoutMsgBox(
                                                HttpUtility.JavaScriptStringEncode(consensoBloccanteMessage), OPEN_RILEVAZIONE_CONSENSO + "*" + codiceAusiliarioPaziente, False, True))
                    Else
                        '--
                        ' Messaggio per consenso
                        '--
                        SetMessageConsenso(consenso)
                    End If

                End If

            End If
        End If

    End Sub

    Private Sub SetMessageConsenso(consenso As Consenso.StatoConsensoPaziente)
        GestionePazientiMessage.StatoConsenso.ImageUrl = consenso.UrlIconaStatoConsenso
        GestionePazientiMessage.StatoConsenso.ToolTip = consenso.DescrizioneStatoConsenso
        GestionePazientiMessage.StatoConsenso.Text = consenso.DescrizioneStatoConsenso
        GestionePazientiMessage.StatoConsenso.Visible = True
    End Sub

    ''' <summary>
    ''' Impostazione automatica dei consensi specificati (se non presenti o diversi dal livello specificato).
    ''' I consensi da impostare (e il livello da rilevare per ogni consenso) sono specificati nel parametro CONSENSO_ID_AUTORILEVAZIONE.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnAutoRilevazioneConsensi_Click(sender As Object, e As EventArgs) Handles btnAutoRilevazioneConsensi.Click

        If Settings.CONSENSO_ID_AUTORILEVAZIONE Is Nothing OrElse Settings.CONSENSO_ID_AUTORILEVAZIONE.Count = 0 Then

            ' Nessun consenso configurato per la rilevazione automatica
            OnitLayout21.InsertRoutineJS("alert(""Non verranno rilevati consensi relativi al paziente, poichè nessun consenso è stato configurato per la rilevazione automatica."");")

        Else

            ' Consensi configurati: richiesta all'utente di confermare la rilevazione automatica
            Dim message As String = String.Empty

            Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                Using bizPaziente As New BizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    message = bizPaziente.GetDescrizioneConsensiRilevazioneAutomatica()

                End Using
            End Using

            OnitLayout21.ShowMsgBox(New PagesLayout.OnitLayout3.OnitLayoutMsgBox(
                                    HttpUtility.JavaScriptStringEncode(message), RILEVAZIONE_AUTOMATICA_CONSENSI, True, True))
        End If

    End Sub

    Private Sub btnConsenso_Click(sender As Object, e As EventArgs) Handles btnConsenso.Click

        Dim codiceAusiliarioPaziente As String = GetCodiceAusiliarioPazienteFromCurrentRow(False)

        If codiceAusiliarioPaziente = String.Empty Then
            AlertClientMsg(Settings.CONSENSO_MSG_NO_COD_CENTRALE)
        Else
            ApriRilevazioneConsenso(codiceAusiliarioPaziente, False)
        End If

    End Sub

    Private Sub ApriRilevazioneConsenso(codiceCentralePaziente As String, autoEdit As Boolean)
        modConsenso.VisibileMD = True
        frameConsenso.Attributes.Add("src", GetUrlMascheraRilevazioneConsenso(codiceCentralePaziente, autoEdit))
    End Sub

    Private Sub RilevazioneAutomaticaConsensi()

        '--
        ' Rilevazione automatica del consenso e aggiornamento visibilità dati (se modificato consenso alla comunicazione)
        '--
        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizPaziente As New BizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                bizPaziente.RilevazioneAutomaticaConsensi(GetCodiceAusiliarioPazienteFromCurrentRow(True), OnVacUtility.Variabili.PazId, OnVacContext.CodiceUslCorrente)

            End Using
        End Using

        '--
        ' Dopo la rilevazione automatica del consenso => devo sempre impostare left e stato pagina in base al nuovo stato del consenso
        '--
        Dim result As ManageConsensoResult = ManageLayoutConsenso(False)
        If result.IsConsensoBloccante.HasValue AndAlso result.IsConsensoBloccante.Value Then
            LoadLeftFrame = result.LoadLeft
            StatoCorrentePagina = result.StatoPagina
        Else
            ControlloCampi(True)
            StatoCorrentePagina = StatoPagina.VIEW
        End If

        If LoadLeftFrame Then RefreshLeftFrame = True

    End Sub

#End Region

#Region " Eventi gestionePazientiDatiSanitari "

    Private Sub gestionePazientiDatiSanitari_OnAlert(sender As Object, e As Common.UserControlEventArgs) Handles gestionePazientiDatiSanitari.OnAlert

        GestionePazientiMessage.Console.StatusMessage(e.Text, WarningMessage.MessageType.AlertMessage)

    End Sub

    Private Sub gestionePazientiDatiSanitari_OnInsertRoutineJS(sender As Object, e As GestionePazientiDatiSanitari.InsertRoutineJSEventArgs) Handles gestionePazientiDatiSanitari.OnInsertRoutineJS

        Me.OnitLayout21.InsertRoutineJS(e.JS)

    End Sub

    Private Sub gestionePazientiDatiSanitari_OnStateChanged(sender As Object, e As GestionePazientiDatiSanitari.StateChangedEventArgs) Handles gestionePazientiDatiSanitari.OnStateChanged

        Select Case e.ControlState

            Case OnVac.GestionePazientiDatiSanitari.ControlStateEnum.EDIT

                Me.StatoCorrentePagina = StatoPagina.EDIT
                RegistraDatiConsultorio()

            Case OnVac.GestionePazientiDatiSanitari.ControlStateEnum.LOCK

                Me.StatoCorrentePagina = StatoPagina.LOCK
                RegistraDatiConsultorio()

            Case OnVac.GestionePazientiDatiSanitari.ControlStateEnum.VIEW
                ' il controllo NON deve annullare l'edit della maschera

        End Select

    End Sub

    Private Sub BindControlsFromPanel()

        ' gestionePazientiDatiSanitari
        Dim pazRow As DataRow = odpDettaglioPaziente.getCurrentDataRow()
        Dim panelEncoder As OnitDataPanel.FieldsEncoder = odpDettaglioPaziente.GetCurrentTableEncoder()

        Dim obj As Object = panelEncoder.getValOf(pazRow, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_CODICE", "T_PAZ_PAZIENTI", True)
        If Not obj Is System.DBNull.Value Then
            gestionePazientiDatiSanitari.Paziente.Codice = obj.ToString()
        Else
            gestionePazientiDatiSanitari.Paziente.Codice = -1
            ' Throw New Exception("panelEncoder.getValOf(pazRow, Constants.ConnessioneAnagrafe.LOCALE, ""PAZ_CODICE"", ""T_PAZ_PAZIENTI"", True) is System.DBNull")
        End If
        obj = panelEncoder.getValOf(pazRow, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_DATA_NASCITA", "T_PAZ_PAZIENTI", True)
        If Not obj Is System.DBNull.Value Then gestionePazientiDatiSanitari.Paziente.DataNascita = obj.ToString()

        obj = panelEncoder.getValOf(pazRow, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_SESSO", "T_PAZ_PAZIENTI", True)
        If Not obj Is System.DBNull.Value Then gestionePazientiDatiSanitari.Paziente.Sesso = obj.ToString()

        obj = panelEncoder.getValOf(pazRow, Constants.ConnessioneAnagrafe.LOCALE, "PAZ_RSC_CODICE", "T_PAZ_PAZIENTI", True)
        If Not obj Is System.DBNull.Value Then gestionePazientiDatiSanitari.Paziente.CategorieRischio = obj.ToString()

        gestionePazientiDatiSanitari.BindControls()
        WwDataBinder1.DataBind()

    End Sub

    Private Sub BindControls()

        ' gestionePazientiDatiSanitari
        gestionePazientiDatiSanitari.Paziente.Codice = OnVacUtility.Variabili.PazId
        gestionePazientiDatiSanitari.Paziente.DataNascita = txtDataNascita.Data
        gestionePazientiDatiSanitari.Paziente.Sesso = cmbSesso.SelectedValue
        gestionePazientiDatiSanitari.Paziente.CategorieRischio = txtCategorieRischio.Codice
        gestionePazientiDatiSanitari.BindControls()

        WwDataBinder1.DataBind()

    End Sub

#End Region

#Region " ImpostaSedeVaccinale "

#Region " Types "

    Public Class ImpostazioneSedeVac
        Private _tipooperazione As Enumerators.TipoOperazioneSedeVac
        Private _consultorio As String

        Public Sub New(tipoOp As Enumerators.TipoOperazioneSedeVac, cns As String)
            _tipooperazione = tipoOp
            _consultorio = cns
        End Sub

        Public Function TipoOperazione() As Enumerators.TipoOperazioneSedeVac
            Return _tipooperazione
        End Function

        Public Function Consultorio() As String
            Return _consultorio
        End Function

    End Class

#End Region

    Public Function ImpostaSedeVaccinale(consideraCirc As Boolean, consideraCom As Boolean, tipoSedeVac As Enumerators.TipoImpostazioneSedeVaccinale,
                                         cnsCorrente As String, dtCnsCirc As DataTable, dtCnsCom As DataTable, giorniEtaPaz As String, dtCnsEta As DataTable,
                                         ByRef strScript As String, ByRef SedeVaccAuto As String()) As ImpostazioneSedeVac
        strScript = ""

        Dim risImpostazione As ImpostazioneSedeVac
        Dim stbCns As New System.Text.StringBuilder()

        'recupero tramite circoscrizione
        If consideraCirc Then

            If dtCnsCirc.Rows.Count > 0 Then

                If dtCnsCirc.Rows.Count = 1 Then

                    Select Case tipoSedeVac

                        Case Enumerators.TipoImpostazioneSedeVaccinale.CnsNo

                            SedeVaccAuto = New String() {dtCnsCirc.Rows(0).Item("RCO_CNS_CODICE"), dtCnsCirc.Rows(0).Item("CNS_DESCRIZIONE")}

                            strScript &= "if (confirm(""Attenzione: il campo 'Sede Vaccinale' non è impostato. Si desidera impostare automaticamente il valore\r'" & dtCnsCirc.Rows(0)("RCO_CNS_CODICE") & " " & dtCnsCirc.Rows(0)("CNS_DESCRIZIONE") & "'\rrecuperato tramite la circoscrizione di residenza?"
                            strScript &= ControllaEtaPazienteCns(dtCnsCirc.Rows(0).Item("CNS_A_ETA"), dtCnsCirc.Rows(0).Item("CNS_DA_ETA"), giorniEtaPaz, dtCnsEta)
                            strScript &= """))" & Chr(13) & Chr(10)
                            strScript &= "__doPostBack('SedeAutomatica', 'true');" & Chr(13) & Chr(10)
                            strScript &= "else " & Chr(13) & Chr(10)
                            strScript &= "__doPostBack('SedeAutomatica','false');" & Chr(13) & Chr(10)

                            risImpostazione = New ImpostazioneSedeVac(Enumerators.TipoOperazioneSedeVac.Ok, "")

                            Return risImpostazione

                        Case Enumerators.TipoImpostazioneSedeVaccinale.CnsSi

                            If dtCnsCirc.Rows(0)("RCO_CNS_CODICE") <> cnsCorrente Then
                                SedeVaccAuto = New String() {dtCnsCirc.Rows(0).Item("RCO_CNS_CODICE"), dtCnsCirc.Rows(0).Item("CNS_DESCRIZIONE")}

                                strScript &= "if (confirm(""Attenzione: il campo 'Sede Vaccinale' è impostato ad un valore non corretto. Si desidera impostare automaticamente il valore\r'" & dtCnsCirc.Rows(0)("RCO_CNS_CODICE") & " " & dtCnsCirc.Rows(0)("CNS_DESCRIZIONE") & "'\rrecuperato tramite la circoscrizione di residenza?"
                                strScript &= ControllaEtaPazienteCns(dtCnsCirc.Rows(0).Item("CNS_A_ETA"), dtCnsCirc.Rows(0).Item("CNS_DA_ETA"), giorniEtaPaz, dtCnsEta)
                                strScript &= """))" & Chr(13) & Chr(10)
                                strScript &= "__doPostBack('SedeAutomatica', 'true');" & Chr(13) & Chr(10)
                                strScript &= "else " & Chr(13) & Chr(10)
                                strScript &= "__doPostBack('SedeAutomatica','false');" & Chr(13) & Chr(10)

                                risImpostazione = New ImpostazioneSedeVac(Enumerators.TipoOperazioneSedeVac.Ok, "")

                                Return risImpostazione

                            End If

                    End Select

                Else

                    For count As Integer = 0 To dtCnsCirc.Rows.Count - 1
                        If dtCnsCirc.Rows(count)("RCO_CNS_CODICE") = cnsCorrente And (ControllaEtaPazienteCns(dtCnsCirc.Rows(count)("CNS_A_ETA"), dtCnsCirc.Rows(count)("CNS_DA_ETA"), giorniEtaPaz, dtCnsEta, False, True) = "SI") Then
                            risImpostazione = New ImpostazioneSedeVac(Enumerators.TipoOperazioneSedeVac.Nessuna, "")
                            Return risImpostazione
                        End If
                        stbCns.AppendFormat("- {0} {1}", dtCnsCirc.Rows(count)("RCO_CNS_CODICE"), dtCnsCirc.Rows(count)("CNS_DESCRIZIONE"))
                        stbCns.Append(ControllaEtaPazienteCns(dtCnsCirc.Rows(count)("CNS_A_ETA"), dtCnsCirc.Rows(count)("CNS_DA_ETA"), giorniEtaPaz, dtCnsEta, False))
                        stbCns.Append("\r")
                    Next

                    Select Case tipoSedeVac

                        Case Enumerators.TipoImpostazioneSedeVaccinale.CnsNo

                            strScript &= "alert(""Attenzione: il campo 'Sede Vaccinale' non è impostato.\rÈ cosigliabile selezionare una delle seguenti sedi vaccinali associate alla circoscrizione di residenza:\r"
                            strScript &= stbCns.ToString()
                            strScript &= """);" & Chr(13) & Chr(10)
                            strScript &= "__doPostBack('SedeAutomatica','false');" & Chr(13) & Chr(10)

                        Case Enumerators.TipoImpostazioneSedeVaccinale.CnsSi

                            strScript &= "alert(""Attenzione: il campo 'Sede Vaccinale' è impostato ad un valore non corretto.\rÈ cosigliabile selezionare una delle seguenti sedi vaccinali associate alla circoscrizione di residenza:\r"
                            strScript &= stbCns.ToString()
                            strScript &= """);"

                    End Select

                    risImpostazione = New ImpostazioneSedeVac(Enumerators.TipoOperazioneSedeVac.Ok, "")

                    Return risImpostazione

                End If

            End If

        End If

        'recupero tramite comune
        If consideraCom Then

            If dtCnsCom.Rows.Count > 0 Then
                If dtCnsCom.Rows.Count = 1 Then

                    Select Case tipoSedeVac

                        Case Enumerators.TipoImpostazioneSedeVaccinale.CnsNo

                            SedeVaccAuto = New String() {dtCnsCom.Rows(0).Item("CCO_CNS_CODICE"), dtCnsCom.Rows(0).Item("CNS_DESCRIZIONE")}

                            strScript &= "if (confirm(""Attenzione: il campo 'Sede Vaccinale' non è impostato. Si desidera impostare automaticamente il valore\r'" & dtCnsCom.Rows(0)("CCO_CNS_CODICE") & " " & dtCnsCom.Rows(0)("CNS_DESCRIZIONE") & "'\rrecuperato tramite il comune di residenza?"
                            strScript &= ControllaEtaPazienteCns(dtCnsCom.Rows(0).Item("CNS_A_ETA"), dtCnsCom.Rows(0).Item("CNS_DA_ETA"), giorniEtaPaz, dtCnsEta)
                            strScript &= """))" & Chr(13) & Chr(10)
                            strScript &= "__doPostBack('SedeAutomatica', 'true');" & Chr(13) & Chr(10)
                            strScript &= "else " & Chr(13) & Chr(10)
                            strScript &= "__doPostBack('SedeAutomatica','false');" & Chr(13) & Chr(10)

                            risImpostazione = New ImpostazioneSedeVac(Enumerators.TipoOperazioneSedeVac.Ok, "")

                            Return risImpostazione

                        Case Enumerators.TipoImpostazioneSedeVaccinale.CnsSi

                            If dtCnsCom.Rows(0)("CCO_CNS_CODICE") <> cnsCorrente Then

                                SedeVaccAuto = New String() {dtCnsCom.Rows(0).Item("CCO_CNS_CODICE"), dtCnsCom.Rows(0).Item("CNS_DESCRIZIONE")}

                                strScript &= "if (confirm(""Attenzione: il campo 'Sede Vaccinale' è impostato ad un valore non corretto. Si desidera impostare automaticamente il valore\r'" & dtCnsCom.Rows(0)("CCO_CNS_CODICE") & " " & dtCnsCom.Rows(0)("CNS_DESCRIZIONE") & "'\rrecuperato tramite il comune di residenza?"
                                strScript &= ControllaEtaPazienteCns(dtCnsCom.Rows(0).Item("CNS_A_ETA"), dtCnsCom.Rows(0).Item("CNS_DA_ETA"), giorniEtaPaz, dtCnsEta)
                                strScript &= """))" & Chr(13) & Chr(10)
                                strScript &= "__doPostBack('SedeAutomatica', 'true');" & Chr(13) & Chr(10)
                                strScript &= "else " & Chr(13) & Chr(10)
                                strScript &= "__doPostBack('SedeAutomatica','false');" & Chr(13) & Chr(10)

                                risImpostazione = New ImpostazioneSedeVac(Enumerators.TipoOperazioneSedeVac.Ok, "")

                                Return risImpostazione

                            End If

                    End Select

                Else

                    For count As Integer = 0 To dtCnsCom.Rows.Count - 1
                        If ((dtCnsCom.Rows(count)("CCO_CNS_CODICE") = cnsCorrente) And (ControllaEtaPazienteCns(dtCnsCom.Rows(count)("CNS_A_ETA"), dtCnsCom.Rows(count)("CNS_DA_ETA"), giorniEtaPaz, dtCnsEta, False, True)) = "SI") Then
                            risImpostazione = New ImpostazioneSedeVac(Enumerators.TipoOperazioneSedeVac.Nessuna, "")
                            Return risImpostazione
                        End If
                        stbCns.AppendFormat("- {0} {1}", dtCnsCom.Rows(count)("CCO_CNS_CODICE"), dtCnsCom.Rows(count)("CNS_DESCRIZIONE"))
                        stbCns.Append(ControllaEtaPazienteCns(dtCnsCom.Rows(count)("CNS_A_ETA"), dtCnsCom.Rows(count)("CNS_DA_ETA"), giorniEtaPaz, dtCnsEta, False))
                        stbCns.Append("\r")
                    Next

                    Select Case tipoSedeVac

                        Case Enumerators.TipoImpostazioneSedeVaccinale.CnsNo

                            strScript &= "alert(""Attenzione: il campo 'Sede Vaccinale' non è impostato.\rÈ cosigliabile selezionare una delle seguenti sedi vaccinali associate al comune di residenza:\r"
                            strScript &= stbCns.ToString()
                            strScript &= """);" & Chr(13) & Chr(10)
                            strScript &= "__doPostBack('SedeAutomatica','false');" & Chr(13) & Chr(10)

                        Case Enumerators.TipoImpostazioneSedeVaccinale.CnsSi

                            strScript &= "alert(""Attenzione: il campo 'Sede Vaccinale' è impostato ad un valore non corretto.\rÈ cosigliabile selezionare una delle seguenti sedi vaccinali associate al comune di residenza:\r"
                            strScript &= stbCns.ToString()
                            strScript &= """);"

                    End Select

                    risImpostazione = New ImpostazioneSedeVac(Enumerators.TipoOperazioneSedeVac.Ok, "")

                    Return risImpostazione

                End If

            End If

        End If

        risImpostazione = New ImpostazioneSedeVac(Enumerators.TipoOperazioneSedeVac.Nessuna, "")

        Return risImpostazione

    End Function

    Private Function ControllaEtaPazienteCns(cnsAEta As Integer, cnsDaEta As Integer, giorniEtaPaz As Integer, dtCnsEta As DataTable,
                                             Optional cnsAlternativi As Boolean = True, Optional soloControllo As Boolean = False) As String

        Dim stbEta As New System.Text.StringBuilder()

        If soloControllo Then

            If ControllaEtaPazienteCns(cnsAEta, cnsDaEta, giorniEtaPaz) Then
                Return "SI"
            Else
                Return "NO"
            End If

        Else

            If ControllaEtaPazienteCns(cnsAEta, cnsDaEta, giorniEtaPaz) Then

                If Not cnsAlternativi Then stbEta.Append(" --> (Associabile per data di nascita del paziente)")

            Else

                If cnsAlternativi Then

                    stbEta.Append("\r\r-- ! -- L'età del paziente non corrisponde a quella del centro vaccinale da impostare!")

                    For count As Integer = 0 To dtCnsEta.Rows.Count - 1
                        If count = 0 Then stbEta.Append("\rSecondo la data del paziente, infatti, i centri vaccinali associabili sono i seguenti:\r")
                        stbEta.AppendFormat("- {0} {1}\r", dtCnsEta.Rows(count)("CNS_CODICE"), dtCnsEta.Rows(count)("CNS_DESCRIZIONE"))
                        If count = dtCnsEta.Rows.Count - 1 Then stbEta.Append("-- ! -- È consigliabile annullare l'operazione e selezionare uno dei centri vaccinali che rientrano nella data.\r")
                    Next

                End If

            End If

        End If

        Return stbEta.ToString()

    End Function

    Private Function ControllaEtaPazienteCns(cnsAEta As Integer, cnsDaEta As Integer, giorniEtaPaz As Integer) As Boolean

        If Not (cnsDaEta <= giorniEtaPaz AndAlso cnsAEta >= giorniEtaPaz) Then
            Return False
        End If

        Return True

    End Function

#End Region

#Region " Eventi OnitLayout "

    Private Sub OnitLayout21_AlertClick(sender As Object, e As PagesLayout.OnitLayout3.AlertEventArgs) Handles OnitLayout21.AlertClick

        If e.Key.StartsWith(OPEN_RILEVAZIONE_CONSENSO) Then

            Dim codiceAusiliarioPaziente As String = String.Empty

            Dim key As String() = e.Key.Split("*")
            If key.Length > 1 Then
                codiceAusiliarioPaziente = key(1)
            End If

            ApriRilevazioneConsenso(codiceAusiliarioPaziente, True)

        End If

    End Sub

    Private Sub OnitLayout21_ConfirmClick(sender As Object, e As Controls.PagesLayout.OnitLayout3.ConfirmEventArgs) Handles OnitLayout21.ConfirmClick

        Select Case e.Key

            Case CONFIRM_SALVATAGGIO_PAZIENTE_DUPLICATO

                ' Gestione della richiesta di conferma all'utente in seguito a fallimento dei controlli non bloccanti su dati duplicati.

                If e.Result Then
                    Me.ControlloDuplicazioneESalvataggioDati(False)
                Else
                    GestionePazientiMessage.Console.StatusMessage("Salvataggio annullato dall'utente.", WarningMessage.MessageType.AlertMessage)
                End If

            Case RILEVAZIONE_AUTOMATICA_CONSENSI

                If e.Result Then Me.RilevazioneAutomaticaConsensi()

        End Select

    End Sub

#End Region

#Region " Gestione Layout Maschera "

#Region " Mostra/nasconde i pulsanti di stampa "

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()

        ' Imposta la visibilità dei pulsanti di stampa
        listPrintButtons.Add(New PrintButton(Constants.ReportName.CertificatoVaccinale, "btnCertificato"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.CertificatoVaccinale, "btnCertificatoVaccinaleValido"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.CertificatoVaccinale, "btnCertificatoVaccinaleLotti"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.CertificatoFrequenza, "btnCertificatoFrequenza"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.CertificatoEseguiteScadute, "btnCertificatoEseguiteScadute"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.CertificatoDiscrezionale, "btnCertificatoDiscrezionale"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.CertificatoMantoux, "btnCertificatoMantoux"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.CertificatoMantoux, "btnCertificatoSoloMantoux"))

        Me.ShowToolbarPrintButtons(listPrintButtons, MainToolbar)

        ' Imposta la visibilità del pulsante Libretto Vaccinale in base alla presenza di uno dei 3 possibili formati
        Me.ShowButtonLibrettoVaccinale()

        ' Imposta la toolbar con i pulsanti di stampa dei libretti vaccinali
        listPrintButtons.Clear()

        listPrintButtons.Add(New PrintButton(Constants.ReportName.LibrettoVaccinale, "btnSelectedLibrettoVaccinale"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.LibrettoVaccinale2, "btnSelectedLibrettoVaccinale2"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.LibrettoVaccinale3, "btnSelectedLibrettoVaccinale3"))

        Me.ShowToolbarPrintButtons(listPrintButtons, SelectStampaLibrettoVaccinaleToolBar)

    End Sub

    Private Sub ShowButtonLibrettoVaccinale()

        Dim showBtn As Boolean = True

        Dim rptList As List(Of Entities.Report) = Me.LoadReportsLibrettiVaccinali()

        If rptList Is Nothing OrElse rptList.Count = 0 Then
            showBtn = False
        End If

        Me.MainToolbar.FindItemByValue("btnLibrettoVaccinale").Visible = showBtn

    End Sub

    Private Function LoadReportsLibrettiVaccinali() As List(Of Entities.Report)

        Dim rptList As List(Of Entities.Report) = Nothing

        Dim listRptNames As New List(Of String)
        listRptNames.Add(Constants.ReportName.LibrettoVaccinale)
        listRptNames.Add(Constants.ReportName.LibrettoVaccinale2)
        listRptNames.Add(Constants.ReportName.LibrettoVaccinale3)

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                rptList = bizReport.GetReports(listRptNames)

            End Using
        End Using

        Return rptList

    End Function

#End Region

#Region " Controlli e visualizzazione messaggi "

    ' Controllo dati paziente e visualizzazione messaggi di errore/warning
    Private Sub CheckDatiPazienteCorrente()

        Me.DisabilitaLayout = False

        ' Il controllo avviene solo se non sono in inserimento
        If Me.odpDettaglioPaziente.CurrentOperation <> OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord Then

            ' Controllo se il paziente è cancellato
            Me.CheckPazienteCancellato()

            ' Controllo se il paziente è deceduto
            Dim statoAnagDeceduto As Int16 = Enumerators.StatoAnagrafico.DECEDUTO
            If Me.txtDataDecesso.Text <> String.Empty Or Me.cmbStatoAnagrafico.SelectedValue = statoAnagDeceduto.ToString() Then

                GestionePazientiMessage.WarnDeceduto.Visible = True

            End If

            ' Controllo se il consultorio corrente è diverso dal consultorio vaccinale del paziente
            If OnVacUtility.Variabili.CNS.Codice <> Me.txtSedeVaccinale.Codice Then

                GestionePazientiMessage.WarnConsultorio.Visible = True

            End If

            ' Controllo se assistenza sanitaria scaduta
            Me.CheckAssistenzaSanitariaScaduta()

            ' Gestione Consenso: controllo se deve essere visualizzato un warning relativo allo stato del consenso
            Me.ShowAlertConsenso(Nothing)

        End If

    End Sub

    Private Sub CheckPazienteCancellato()

        If Me.Settings.FLAG_CANCELLATO_CHECK Then

            If Me.odpDettaglioPaziente.getCurrentDataRow("paz_cancellato").ToString() = "S" Then

                ' Disabilitazione maschera 
                Me.DisabilitaLayout = True

                ' Visualizzazione alert
                GestionePazientiMessage.WarnCancellato.Visible = True

            End If

        End If

    End Sub

    Private Sub CheckAssistenzaSanitariaScaduta()

        Dim dataScadenzaAssistenza As DateTime = DateTime.MinValue

        Dim objDataScadenzaAssistenza As Object = odpDettaglioPaziente.getCurrentDataRow("paz_data_scadenza_ssn")

        If Not objDataScadenzaAssistenza Is Nothing AndAlso Not objDataScadenzaAssistenza Is DBNull.Value Then

            Try
                dataScadenzaAssistenza = Convert.ToDateTime(objDataScadenzaAssistenza)
            Catch ex As Exception
                dataScadenzaAssistenza = DateTime.MinValue
            End Try

        End If

        If dataScadenzaAssistenza > DateTime.MinValue AndAlso dataScadenzaAssistenza < Date.Today Then

            GestionePazientiMessage.WarnAssistenzaScaduta.Visible = True

        End If

    End Sub

#End Region

#Region " Layout Check Preferenza "

    ''' <summary>
    ''' Abilitazione/disabilitazione check preferenza in base allo stato della pagina
    ''' </summary>
    Private Sub SetCheckPreferenza()

        Select Case Me.StatoCorrentePagina

            Case StatoPagina.VIEW

                Me.DisableCheckPreferenza()

            Case StatoPagina.EDIT, StatoPagina.LOCK

                Me.chkLunedi.Enabled = True
                Me.chkMartedi.Enabled = True
                Me.chkMercoledi.Enabled = True
                Me.chkGiovedi.Enabled = True
                Me.chkVenerdi.Enabled = True
                Me.chkSabato.Enabled = True
                Me.chkDomenica.Enabled = True

        End Select

    End Sub

    Private Sub DisableCheckPreferenza()

        Me.chkLunedi.Enabled = False
        Me.chkMartedi.Enabled = False
        Me.chkMercoledi.Enabled = False
        Me.chkGiovedi.Enabled = False
        Me.chkVenerdi.Enabled = False
        Me.chkSabato.Enabled = False
        Me.chkDomenica.Enabled = False

    End Sub

#End Region

#Region " Layout Toolbar "

    Private Sub SetToolBar()

        Select Case Me.StatoCorrentePagina

            Case StatoPagina.VIEW

                GetToolBarButton("btnSalva").Enabled = False
                GetToolBarButton("btnAnnulla").Enabled = False
                GetToolBarButton("btnModifica").Enabled = True
                GetToolBarButton("btnCalVac").Enabled = True
                GetToolBarButton("btnCertificato").Enabled = True
                GetToolBarButton("btnCertificatoFrequenza").Enabled = True
                GetToolBarButton("btnCertificatoVaccinaleValido").Enabled = True
                GetToolBarButton("btnCertificatoVaccinaleLotti").Enabled = True
                GetToolBarButton("btnCertificatoEseguiteScadute").Enabled = True
                GetToolBarButton("btnCertificatoDiscrezionale").Enabled = True
                GetToolBarButton("btnCertificatoMantoux").Enabled = True
                GetToolBarButton("btnCertificatoSoloMantoux").Enabled = True
                GetToolBarButton("btnLibrettoVaccinale").Enabled = True

            Case StatoPagina.EDIT

                GetToolBarButton("btnSalva").Enabled = True
                GetToolBarButton("btnAnnulla").Enabled = True
                GetToolBarButton("btnModifica").Enabled = False
                GetToolBarButton("btnCalVac").Enabled = False
                GetToolBarButton("btnCertificato").Enabled = False
                GetToolBarButton("btnCertificatoFrequenza").Enabled = False
                GetToolBarButton("btnCertificatoVaccinaleValido").Enabled = False
                GetToolBarButton("btnCertificatoVaccinaleLotti").Enabled = False
                GetToolBarButton("btnCertificatoEseguiteScadute").Enabled = False
                GetToolBarButton("btnCertificatoDiscrezionale").Enabled = False
                GetToolBarButton("btnCertificatoMantoux").Enabled = False
                GetToolBarButton("btnCertificatoSoloMantoux").Enabled = False
                GetToolBarButton("btnLibrettoVaccinale").Enabled = False

            Case StatoPagina.LOCK, StatoPagina.BLOCCO_CONSENSO

                DisableToolbar(True)

        End Select

    End Sub

    Private Sub DisableToolbar(disabilitaStampe As Boolean)

        GetToolBarButton("btnSalva").Enabled = False
        GetToolBarButton("btnAnnulla").Enabled = False
        GetToolBarButton("btnModifica").Enabled = False
        GetToolBarButton("btnCalVac").Enabled = Not disabilitaStampe
        GetToolBarButton("btnCertificato").Enabled = Not disabilitaStampe
        GetToolBarButton("btnCertificatoFrequenza").Enabled = Not disabilitaStampe
        GetToolBarButton("btnCertificatoVaccinaleValido").Enabled = Not disabilitaStampe
        GetToolBarButton("btnCertificatoVaccinaleLotti").Enabled = Not disabilitaStampe
        GetToolBarButton("btnCertificatoEseguiteScadute").Enabled = Not disabilitaStampe
        GetToolBarButton("btnCertificatoDiscrezionale").Enabled = Not disabilitaStampe
        GetToolBarButton("btnCertificatoMantoux").Enabled = Not disabilitaStampe
        GetToolBarButton("btnCertificatoSoloMantoux").Enabled = Not disabilitaStampe
        GetToolBarButton("btnLibrettoVaccinale").Enabled = Not disabilitaStampe

    End Sub

    Private Function GetToolBarButton(buttonName As String) As Telerik.Web.UI.RadToolBarItem

        Return MainToolbar.FindItemByValue(buttonName)

    End Function

#End Region

    ' Campi da nascondere e disabilitare a seconda dei parametri
    Private Sub DisabilitaCampi()

        Me.gestionePazientiDatiSanitari.DisabilitaCampi()

        ' --- Campi da disabilitare/nascondere/rendere obbligatori --- '
        Dim campiDaDisabilitare As List(Of String)

        ' Gestione Flag Paziente Locale:
        ' se il paziente corrente è gestito in locale (paz_locale = "S")  i campi da disabilitare devono essere letti 
        ' dal parametro ANAGPAZ_CAMPI_DISABILITATI_LOC, altrimenti da quello centrale. Per i campi nascosti c'è solo un parametro.
        If Me.chkLocale.Checked Then
            campiDaDisabilitare = Me.Settings.ANAGPAZ_CAMPI_DISABILITATI_LOC
        Else
            campiDaDisabilitare = Me.Settings.ANAGPAZ_CAMPI_DISABILITATI_CEN
        End If

        ' Elenco dei campi da disabilitare in base al livello di certificazione del paziente
        If Not odpDettaglioPaziente.getCurrentDataRow("paz_livello_certificazione") Is DBNull.Value _
           AndAlso odpDettaglioPaziente.getCurrentDataRow("paz_livello_certificazione") <> 0 Then

            campiDaDisabilitare = campiDaDisabilitare.Union(Me.Settings.ANAGPAZ_CAMPI_DISABILITATI_LIVELLO_CERTIFICAZIONE).ToList()

        End If

        ' --- Campi non gestiti dal pannello --- '
        ' I campi non gestiti dal pannello vanno disabilitati esplicitamente, caso per caso 
        Dim idx As Integer = campiDaDisabilitare.IndexOf("PAZ_GIORNO")

        ' Check della sezione "Preferenza"
        If idx >= 0 Then
            Me.DisableCheckPreferenza()
            campiDaDisabilitare.RemoveAt(idx)
        End If

        ' User control viaResidenza
        idx = campiDaDisabilitare.IndexOf("PAZ_INDIRIZZO_RESIDENZA")

        If idx >= 0 Then
            Me.viaResidenza.Disabilita(True)
            campiDaDisabilitare.RemoveAt(idx)
        End If

        ' User control viaDomicilio
        idx = campiDaDisabilitare.IndexOf("PAZ_INDIRIZZO_DOMICILIO")

        If idx >= 0 Then
            Me.viaDomicilio.Disabilita(True)
            campiDaDisabilitare.RemoveAt(idx)
        End If

        ' --- Campi gestiti dal pannello --- '
        ' Ricerca ricorsiva per comprendere tutti i campi del pannello (anche eventuali aggiunte successive)
        Me.ImpostaCampiDisabilitati(campiDaDisabilitare, False)

    End Sub

    Private Sub NascondiCampi()

        Dim daNascondere As List(Of String) = Me.Settings.ANAGPAZ_CAMPI_NASCOSTI

        ' --- Campi non gestiti --- '
        ' I campi non gestiti dal pannello vanno disabilitati esplicitamente, caso per caso 
        Dim idx As Integer = -1

        ' Check della sezione "Preferenza"
        idx = daNascondere.IndexOf("PAZ_GIORNO")
        If idx >= 0 Then
            Me.chkLunedi.Visible = False
            Me.chkMartedi.Visible = False
            Me.chkMercoledi.Visible = False
            Me.chkGiovedi.Visible = False
            Me.chkVenerdi.Visible = False
            Me.chkSabato.Visible = False
            Me.chkDomenica.Visible = False

            Me.lblPreferenza.Visible = False

            daNascondere.RemoveAt(idx)
        End If

        ' User control viaResidenza
        idx = daNascondere.IndexOf("PAZ_INDIRIZZO_RESIDENZA")
        If idx >= 0 Then
            viaResidenza.Visible = False
            lblViaResidenza.Visible = False
            daNascondere.RemoveAt(idx)
        End If

        ' User control viaDomicilio
        idx = daNascondere.IndexOf("PAZ_INDIRIZZO_DOMICILIO")
        If idx >= 0 Then
            viaDomicilio.Visible = False
            lblViaDomicilio.Visible = False
            daNascondere.RemoveAt(idx)
        End If

        ' Ricerca ricorsiva per comprendere tutti i campi del pannello (anche eventuali aggiunte successive)
        Me.ImpostaCampiVisibili(daNascondere, False)

    End Sub

    Private Sub ImpostaCampiObbligatori(campiBloccantiRichiesti As ArrayList)

        Dim strObbligatorio As String
        Dim notNullField As OnitDataPanel.NotNullFieldInfo

        For j As Integer = 0 To odpDettaglioPaziente.MainTables.Count - 1

            For i As Integer = 0 To campiBloccantiRichiesti.Count - 1
                strObbligatorio = campiBloccantiRichiesti(i).ToString().Trim
                If strObbligatorio <> String.Empty Then

                    Dim exists As Boolean = False
                    For k As Integer = 0 To odpDettaglioPaziente.MainTables(j).NotNullFields.Count - 1
                        If odpDettaglioPaziente.MainTables(j).NotNullFields(k).FieldName = strObbligatorio Then
                            exists = True
                            Exit For
                        End If
                    Next

                    If Not exists Then
                        notNullField = New OnitDataPanel.NotNullFieldInfo(strObbligatorio)
                        odpDettaglioPaziente.MainTables(j).NotNullFields.Add(notNullField)
                    End If

                End If
            Next

        Next

    End Sub

    Private Sub ImpostaCampiDisabilitati(strElencoNoEdit As System.Collections.Generic.List(Of String), enabled As Boolean)

        Dim ctrArray As System.Web.UI.Control()
        Dim ctrl As System.Web.UI.WebControls.WebControl

        If Not strElencoNoEdit Is Nothing AndAlso strElencoNoEdit.Count > 0 Then

            For i As Integer = 0 To strElencoNoEdit.Count - 1

                ctrArray = odpDettaglioPaziente.getWzControlsByBinding(strElencoNoEdit(i))

                For j As Integer = 0 To ctrArray.Length - 1

                    ' Per impostare correttamente la visualizzazione del controllo nella pagina
                    ' è necessario capire che tipo di controllo è:

                    Dim chk As OnitDataPanel.wzCheckBox = Nothing
                    Dim ddl As OnitDataPanel.wzDropDownList = Nothing
                    Dim dpk As OnitDataPanel.wzOnitDatePick = Nothing
                    Dim val As OnitDataPanel.wzOnitJsValidator = Nothing
                    Dim rdb As OnitDataPanel.wzRadioButtonList = Nothing
                    Dim txt As OnitDataPanel.wzTextBox = Nothing
                    Dim fm As OnitDataPanel.wzFinestraModale = Nothing

                    Dim lbl As String = ""
                    Dim ControlloNonPrevisto As Boolean = False

                    ctrl = DirectCast(ctrArray(j), System.Web.UI.WebControls.WebControl)

                    Select Case ctrl.GetType().Name

                        Case "wzCheckBox"
                            chk = DirectCast(ctrl, OnitDataPanel.wzCheckBox)
                            If enabled Then
                                chk.BindingField.Editable = OnitDataPanel.BindingFieldValue.editPositions.always
                            Else
                                chk.BindingField.Editable = OnitDataPanel.BindingFieldValue.editPositions.never
                            End If

                        Case "wzDropDownList"
                            ddl = DirectCast(ctrl, OnitDataPanel.wzDropDownList)
                            If enabled Then
                                ddl.BindingField.Editable = OnitDataPanel.BindingFieldValue.editPositions.always
                            Else
                                ddl.BindingField.Editable = OnitDataPanel.BindingFieldValue.editPositions.never
                            End If

                        Case "wzOnitDatePick"
                            dpk = DirectCast(ctrl, OnitDataPanel.wzOnitDatePick)
                            If enabled Then
                                dpk.BindingField.Editable = OnitDataPanel.BindingFieldValue.editPositions.always
                            Else
                                dpk.BindingField.Editable = OnitDataPanel.BindingFieldValue.editPositions.never
                            End If

                        Case "wzOnitJsValidator"
                            val = DirectCast(ctrl, OnitDataPanel.wzOnitJsValidator)
                            If enabled Then
                                val.BindingField.Editable = OnitDataPanel.BindingFieldValue.editPositions.always
                            Else
                                val.BindingField.Editable = OnitDataPanel.BindingFieldValue.editPositions.never
                            End If

                        Case "wzRadioButtonList"
                            rdb = DirectCast(ctrl, OnitDataPanel.wzRadioButtonList)
                            If enabled Then
                                rdb.BindingField.Editable = OnitDataPanel.BindingFieldValue.editPositions.always
                            Else
                                rdb.BindingField.Editable = OnitDataPanel.BindingFieldValue.editPositions.never
                            End If

                        Case "wzTextBox"
                            txt = DirectCast(ctrl, OnitDataPanel.wzTextBox)
                            If enabled Then
                                txt.BindingField.Editable = OnitDataPanel.BindingFieldValue.editPositions.always
                            Else
                                txt.BindingField.Editable = OnitDataPanel.BindingFieldValue.editPositions.never
                            End If

                        Case "wzFinestraModale"
                            ' Per la modale considero sia il campo codice che il campo descrizione
                            fm = DirectCast(ctrl, OnitDataPanel.wzFinestraModale)
                            If enabled Then
                                fm.BindingCode.Editable = OnitDataPanel.BindingFieldValue.editPositions.always
                                fm.BindingDescription.Editable = OnitDataPanel.BindingFieldValue.editPositions.always
                            Else
                                fm.BindingCode.Editable = OnitDataPanel.BindingFieldValue.editPositions.never
                                fm.BindingDescription.Editable = OnitDataPanel.BindingFieldValue.editPositions.never
                            End If

                        Case Else
                            ctrl.Enabled = enabled

                    End Select

                Next
            Next

        End If

    End Sub

    Private Sub ImpostaCampiVisibili(listIdControlliNonVisibili As List(Of String), visible As Boolean)

        Dim ctrArray As System.Web.UI.Control()
        Dim ctrl As System.Web.UI.WebControls.WebControl

        If Not listIdControlliNonVisibili.IsNullOrEmpty() Then

            For i As Integer = 0 To listIdControlliNonVisibili.Count - 1

                ctrArray = odpDettaglioPaziente.getWzControlsByBinding(listIdControlliNonVisibili(i))

                For j As Integer = 0 To ctrArray.Length - 1

                    ctrl = DirectCast(ctrArray(j), System.Web.UI.WebControls.WebControl)
                    ctrl.Visible = visible

                    If Not ctrl.Attributes("LabelAssociata") Is Nothing AndAlso Not ctrl.NamingContainer.FindControl(ctrl.Attributes("LabelAssociata")) Is Nothing Then
                        ctrl.NamingContainer.FindControl(ctrl.Attributes("LabelAssociata")).Visible = False
                    End If

                Next

            Next

        End If

    End Sub

    Private Sub ControlloCampi(checkSedeVaccinale As Boolean)

        Dim campiObbligatori As New ArrayList()
        Dim campiWarning As New ArrayList()

        Dim controlloEffettuato As Boolean = False

        If Not CheckCampiObbligatori(campiObbligatori, True) Or Not CheckCampiObbligatori(campiWarning, False) Then

            If checkSedeVaccinale AndAlso Me.odpDettaglioPaziente.CurrentOperation <> OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord Then

                'controllo se è possibile inserire automaticamente la sede vaccinale (modifica 10/01/2005)
                controlloEffettuato = True
                If Not ControllaSedeVaccinale() Then ShowErrorNoFillRequiredField(campiObbligatori, campiWarning)

            End If

            If campiObbligatori.Count > 0 Then
                Me.LoadLeftFrame = False
            Else
                Me.LoadLeftFrame = (Not OnVacUtility.IsPazIdEmpty())
            End If

        Else
            Me.LoadLeftFrame = (Not OnVacUtility.IsPazIdEmpty())
        End If

        'la sede vaccinale deve essere controllata anche se è valorizzata [modifica 11/04/2006]
        If checkSedeVaccinale AndAlso Not controlloEffettuato Then ControllaSedeVaccinale()

    End Sub

#End Region

#Region " Private Methods "

#Region " Stampe "

    Private Sub StampaLibrettoVaccinaleSingolaPagina()

        Dim datiLibrettoVaccinalePazienteResult As Biz.BizStampaLibrettoVaccinale.DatiLibrettoVaccinalePazienteResult

        Dim reportFolder As String = String.Empty

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            Using bizStampaLibrettoVaccinale As New BizStampaLibrettoVaccinale(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                datiLibrettoVaccinalePazienteResult = bizStampaLibrettoVaccinale.GetDatiLibrettoVaccinalePazienteSingolaPagina(OnVacUtility.Variabili.PazId)

            End Using

            Using bizReport As New BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                reportFolder = bizReport.GetReportFolder(Constants.ReportName.LibrettoVaccinale)

            End Using

        End Using

        Dim rpt As New ReportParameter()

        ' Parametri per i nomi delle associazioni
        For i As Integer = 0 To datiLibrettoVaccinalePazienteResult.AssociazioniLibretto.Count - 1
            rpt.AddParameter(String.Format("associazione{0}", i + 1), datiLibrettoVaccinalePazienteResult.AssociazioniLibretto(i).DescrizioneAssociazione)
        Next

        rpt.set_dataset(datiLibrettoVaccinalePazienteResult.DstLibrettoVaccinale)

        'se è un sottoreport esterno deve essere passata anche l'estensione del file
        rpt.set_dataset_subreport(datiLibrettoVaccinalePazienteResult.DstLibrettoVaccinale, "SottoReportVaccinazioniColonne2.rpt")
        rpt.set_dataset_subreport(datiLibrettoVaccinalePazienteResult.DstLibrettoVaccinale, "SottoReportVaccinazioniColonne3.rpt")

        '---CMR 31/05/2007--- nel passaggio a Crystal Report XI bisogna passare il datasource anche a tutte le istanze dei sottoreport
        rpt.set_dataset_subreport(datiLibrettoVaccinalePazienteResult.DstLibrettoVaccinale, "SottoReportVaccinazioniColonne2.rpt - 01")
        rpt.set_dataset_subreport(datiLibrettoVaccinalePazienteResult.DstLibrettoVaccinale, "SottoReportVaccinazioniColonne2.rpt - 02")
        rpt.set_dataset_subreport(datiLibrettoVaccinalePazienteResult.DstLibrettoVaccinale, "SottoReportVaccinazioniColonne2.rpt - 04")
        rpt.set_dataset_subreport(datiLibrettoVaccinalePazienteResult.DstLibrettoVaccinale, "SottoReportVaccinazioniColonne2.rpt - 05")
        rpt.set_dataset_subreport(datiLibrettoVaccinalePazienteResult.DstLibrettoVaccinale, "SottoReportVaccinazioniColonne2.rpt - 06")
        rpt.set_dataset_subreport(datiLibrettoVaccinalePazienteResult.DstLibrettoVaccinale, "SottoReportVaccinazioniColonne2.rpt - 07")
        rpt.set_dataset_subreport(datiLibrettoVaccinalePazienteResult.DstLibrettoVaccinale, "SottoReportVaccinazioniColonne2.rpt - 08")
        rpt.set_dataset_subreport(datiLibrettoVaccinalePazienteResult.DstLibrettoVaccinale, "SottoReportVaccinazioniColonne2.rpt - 09")
        rpt.set_dataset_subreport(datiLibrettoVaccinalePazienteResult.DstLibrettoVaccinale, "SottoReportVaccinazioniColonne3.rpt - 01")
        rpt.set_dataset_subreport(datiLibrettoVaccinalePazienteResult.DstLibrettoVaccinale, "SottoReportVaccinazioniColonne3.rpt - 02")

        If Not OnVacReport.StampaReport(Page.Request.Path, Constants.ReportName.LibrettoVaccinale, String.Empty, rpt, Nothing, Nothing, reportFolder) Then
            OnVacUtility.StampaNonPresente(Page, Constants.ReportName.LibrettoVaccinale)
        End If

    End Sub

    ' Per libretto multipagina o etichette
    Private Sub StampaLibrettoVaccinale(nomeReport As String)

        Dim dsLibrettoVaccinale As DSLibrettoVaccinale = Nothing

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizStampaLibrettoVaccinale As New BizStampaLibrettoVaccinale(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                dsLibrettoVaccinale = bizStampaLibrettoVaccinale.GetDataSetLibrettoVaccinale(OnVacUtility.Variabili.PazId)

            End Using
        End Using

        Dim rpt As New ReportParameter()

        rpt.set_dataset(dsLibrettoVaccinale)

        Dim reportFolder As String = String.Empty

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                reportFolder = bizReport.GetReportFolder(nomeReport)

            End Using
        End Using

        If Not OnVacReport.StampaReport(Page.Request.Path, nomeReport, String.Empty, rpt, Nothing, Nothing, reportFolder) Then
            OnVacUtility.StampaNonPresente(Page, nomeReport)
        End If

        dsLibrettoVaccinale.Dispose()

    End Sub

    Private Sub StampaCertificato(buttonKey As String)

        Dim rpt As New ReportParameter()
        Dim nomeRpt As String = String.Empty

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Select Case buttonKey

                Case "btnCertificatoEseguiteScadute", "btnCertificatoDiscrezionale"

                    ' Lettura dati consultorio corrente
                    Dim consultorio As Entities.Cns = Nothing

                    Using bizCns As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)
                        consultorio = bizCns.GetConsultorio(OnVacUtility.Variabili.CNS.Codice)
                    End Using

                    If buttonKey = "btnCertificatoDiscrezionale" Then

                        nomeRpt = Constants.ReportName.CertificatoDiscrezionale

                    Else

                        nomeRpt = Constants.ReportName.CertificatoEseguiteScadute

                        If consultorio Is Nothing Then
                            rpt.AddParameter("cnsStampaIndirizzo", String.Empty)
                            rpt.AddParameter("cnsStampaCap", String.Empty)
                            rpt.AddParameter("cnsStampaComune", String.Empty)
                            rpt.AddParameter("cnsStampaTelefono", String.Empty)
                        Else
                            rpt.AddParameter("cnsStampaIndirizzo", consultorio.Indirizzo)
                            rpt.AddParameter("cnsStampaCap", consultorio.Cap)
                            rpt.AddParameter("cnsStampaComune", consultorio.Comune)
                            rpt.AddParameter("cnsStampaTelefono", consultorio.Telefono)
                        End If

                    End If

                    If consultorio Is Nothing Then
                        rpt.AddParameter("DescrizioneComune", String.Empty)
                    Else
                        rpt.AddParameter("DescrizioneComune", consultorio.Comune)
                    End If

                    rpt.AddParameter("notaValidita", Me.Settings.CERTIFICATO_VACCINALE_NOTA_VALIDITA)

                Case "btnCertificatoMantoux", "btnCertificatoSoloMantoux"

                    nomeRpt = Constants.ReportName.CertificatoMantoux
                    rpt.AddParameter("soloMantoux", IIf(buttonKey = "btnCertificatoSoloMantoux", "S", "N"))
                    rpt.AddParameter("headerDataInvio", Me.GetOnVacResourceValue(Constants.StringResourcesKey.DatiPaziente_MantouxDataInvio))
                    rpt.AddParameter("notaValidita", Me.Settings.CERTIFICATO_VACCINALE_NOTA_VALIDITA)

            End Select

            rpt.AddParameter("cnsStampaCodice", OnVacUtility.Variabili.CNS.Codice)

            Dim strFiltro As String = "{T_PAZ_PAZIENTI.PAZ_CODICE}=" + OnVacUtility.Variabili.PazId

            ' Stampa
            Using bizReport As New BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If Not OnVacReport.StampaReport(Page.Request.Path, nomeRpt, strFiltro, rpt, Nothing, Nothing, bizReport.GetReportFolder(nomeRpt)) Then
                    OnVacUtility.StampaNonPresente(Page, nomeRpt)
                End If

            End Using
        End Using

    End Sub

#End Region

#Region " Usl di Residenza "

    ' Controllo Usl Residenza in base al comune: 
    ' se la usl di residenza non è una di quelle associate al comune di residenza, la valorizzo correttamente.
    Private Sub AllineaUslResidenza(genericProvider As DbGenericProvider)

        Dim codiciUslResidenza As String() = genericProvider.Usl.GetCodiciUslByComune(Me.txtComuneResidenza.Codice)

        If Not codiciUslResidenza Is Nothing AndAlso codiciUslResidenza.Count > 0 Then

            If Not String.IsNullOrEmpty(Me.txtUSLDiResidenza.Codice) Then

                ' Se la usl di residenza selezionata nella modale non è tra quelle associate al comune di residenza scelto, 
                ' imposto il primo tra i codici dell'array.
                If Not codiciUslResidenza.Contains(Me.txtUSLDiResidenza.Codice) Then

                    Me.SetUSLResidenza(codiciUslResidenza.FirstOrDefault())

                End If

            Else

                Me.SetUSLResidenza(codiciUslResidenza.FirstOrDefault())

            End If

        Else

            ' Se non ci sono usl associate al comune selezionato, sbianco la usl di residenza
            Me.SetUSLResidenza(String.Empty)

        End If

    End Sub

    Private Sub SetUSLResidenza(codiceUslResidenza As String)

        txtUSLDiResidenza.Codice = codiceUslResidenza
        txtUSLDiResidenza.Descrizione = String.Empty
        txtUSLDiResidenza.RefreshDataBind()

    End Sub

#End Region

#Region " Codice Ausiliario "

    ''' <summary>
    ''' Restituisce il codice ausiliario del paziente in base relativo alla riga selezionata.
    ''' Se non c'è, solleva un'eccezione.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetCodiceAusiliarioPazienteFromCurrentRow(throwExceptionIfNotFound As Boolean) As String

        Dim codiceAusiliarioPaziente As String = String.Empty

        Try
            Dim currentRow As DataRow = Me.odpDettaglioPaziente.getCurrentDataRow()

            If Not currentRow Is Nothing Then
                codiceAusiliarioPaziente = Me.GetCodiceAusiliario(currentRow)
            End If

        Catch ex As Exception
            Common.Utility.EventLogHelper.EventLogWrite(ex, OnVacContext.AppId)
        End Try

        If throwExceptionIfNotFound AndAlso String.IsNullOrEmpty(codiceAusiliarioPaziente) Then
            Throw New Exception("Codice ausiliario paziente non presente")
        End If

        Return codiceAusiliarioPaziente

    End Function

    ''' <summary>
    ''' Restituisce il codice ausiliario presente nella riga specificata
    ''' </summary>
    ''' <param name="currentRow"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetCodiceAusiliario(currentRow As DataRow) As String

        If Not currentRow Is Nothing Then

            Dim s As Object = odpDettaglioPaziente.GetCurrentTableEncoder.getValOf(currentRow, "paz_codice", "t_paz_pazienti_centrale", True)

            If Not s Is Nothing AndAlso Not IsDBNull(s) AndAlso s.ToString <> String.Empty Then
                Return s.ToString()
            Else
                s = odpDettaglioPaziente.GetCurrentTableEncoder.getValOf(currentRow, "paz_codice_ausiliario", "t_paz_pazienti", True)
                If Not s Is Nothing AndAlso Not IsDBNull(s) AndAlso s.ToString <> String.Empty Then
                    Return s.ToString()
                End If
            End If

        End If

        Return String.Empty

    End Function

#End Region

    Private Function ControllaSedeVaccinale() As Boolean

        Dim control As Boolean = False

        If Settings.AGGIORNACNSBYCIR OrElse Settings.AGGIORNACNSBYCOM Then

            Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

                'il controllo deve essere fatto solo se il comune di residenza è valorizzato e la sede vaccinale non lo è
                If Not Me.odpDettaglioPaziente.getCurrentDataRow() Is Nothing Then

                    Dim r As DataRow = Me.odpDettaglioPaziente.getCurrentDataRow()

                    Dim dtCnsCirc As New DataTable()
                    Dim dtCnsCom As New DataTable()
                    Dim dtCnsEta As New DataTable()

                    Dim giorniEtaPaz As Integer = 0
                    Dim pazCodice As Integer = 0
                    Dim tipoCns As Enumerators.TipoImpostazioneSedeVaccinale

                    If Not r("PAZ_CODICE") Is Nothing AndAlso Not r("PAZ_CODICE") Is System.DBNull.Value Then
                        pazCodice = CInt(r("PAZ_CODICE"))
                    End If

                    If Not r("PAZ_CNS_CODICE") Is Nothing AndAlso Not r("PAZ_CNS_CODICE") Is System.DBNull.Value AndAlso r("PAZ_CNS_CODICE").ToString <> String.Empty Then
                        tipoCns = Enumerators.TipoImpostazioneSedeVaccinale.CnsSi
                    Else
                        tipoCns = Enumerators.TipoImpostazioneSedeVaccinale.CnsNo
                    End If

                    If Not r("PAZ_CIR_CODICE") Is Nothing AndAlso Not r("PAZ_CIR_CODICE") Is System.DBNull.Value AndAlso r("PAZ_CIR_CODICE").ToString <> String.Empty Then
                        dtCnsCirc = genericProvider.Consultori.GetConsultoriInCircoscrizione(r("PAZ_CIR_CODICE"))
                    Else
                        dtCnsCirc = New DataTable()
                    End If

                    If Not r("PAZ_COM_CODICE_RESIDENZA") Is Nothing AndAlso Not r("PAZ_COM_CODICE_RESIDENZA") Is System.DBNull.Value AndAlso r("PAZ_COM_CODICE_RESIDENZA").ToString <> String.Empty Then
                        dtCnsCom = genericProvider.Consultori.GetConsultoriInComune(r("PAZ_COM_CODICE_RESIDENZA"))
                    Else
                        dtCnsCom = New DataTable()
                    End If

                    If Not r("PAZ_DATA_NASCITA") Is Nothing AndAlso Not r("PAZ_DATA_NASCITA") Is System.DBNull.Value Then
                        giorniEtaPaz = Common.Utility.PazienteHelper.CalcoloEta(CDate(r("PAZ_DATA_NASCITA"))).GiorniTotali
                    End If

                    dtCnsEta = genericProvider.Consultori.GetConsultoriPerGiorniNascita(giorniEtaPaz)

                    Dim sv As ImpostazioneSedeVac =
                        Me.ImpostaSedeVaccinale(Me.Settings.AGGIORNACNSBYCIR, Me.Settings.AGGIORNACNSBYCOM, tipoCns,
                                                odpDettaglioPaziente.getCurrentDataRow.Item("PAZ_CNS_CODICE") & "",
                                                dtCnsCirc, dtCnsCom, giorniEtaPaz, dtCnsEta, strScript, SedeVaccAuto)

                    If sv.TipoOperazione = Enumerators.TipoOperazioneSedeVac.Ok Then
                        control = True
                        Me.OnitLayout21.InsertRoutineJS(strScript)
                    Else
                        control = False
                    End If

                End If

            End Using

        End If

        'imposta la variabile a True per indicare che ha effettuato il controllo [modifica 01/06/2006]
        ControlloSedeVaccinale = control

        Return control

    End Function

    Private Sub DataObbligatoria(tipo As String)

        If tipo = "canc" Then

            If Settings.GES_DATA_CANC_OBBLIG And txtDataCancellazione.Enabled Then

                If chkCancellato.Checked Then
                    txtDataCancellazione.CssClass = CSSCLASS_TEXTBOX_STRINGA_OBBLIGATORIO
                Else
                    txtDataCancellazione.CssClass = CSSCLASS_TEXTBOX_STRINGA
                End If

            End If

        Else

            If Settings.GES_DATA_IRREP_OBBLIG And txtDataIrreperibilta.Enabled Then

                If chkIrreperibile.Checked Then
                    txtDataIrreperibilta.CssClass = CSSCLASS_TEXTBOX_STRINGA_OBBLIGATORIO
                Else
                    txtDataIrreperibilta.CssClass = CSSCLASS_TEXTBOX_STRINGA
                End If

            End If

        End If

    End Sub

    Private Sub TipoOccasionale()

        If Settings.GES_DATA_CANC_OBBLIG And cmbTipoOccasionalita.Enabled Then

            If chkOccasionale.Checked Then
                cmbTipoOccasionalita.CssClass = CSSCLASS_TEXTBOX_STRINGA_OBBLIGATORIO
            Else
                cmbTipoOccasionalita.CssClass = CSSCLASS_TEXTBOX_STRINGA
            End If

        End If

    End Sub

    Private Sub SelectStatoAnagraficoItem(ByRef item As ListItem)

        If Not item Is Nothing Then

            cmbStatoAnagrafico.SelectedIndex = -1

            item.Selected = True

        End If

    End Sub

    Private Sub PazRegolarizzato()

        Dim enc As OnitDataPanel.FieldsEncoder = odpDettaglioPaziente.GetCurrentTableEncoder()
        Dim dr As DataRow = odpDettaglioPaziente.getCurrentDataRow()

        If Not dr Is Nothing Then

            Dim val As Object = enc.getValOf(dr, Constants.ConnessioneAnagrafe.LOCALE, "paz_codice", "t_paz_pazienti", True)

            'se il paziente non è presente nell'anagrafe locale devo settare il campo "paz_regolarizzato" a S
            If val Is DBNull.Value Then

                chkRegolarizzato.Checked = True

            End If

        End If

    End Sub

    Private Sub UpdateCircoscrizione(codiceCircoscrizioneNew As String, isCircoscrizioneResidenza As Boolean, recordLog As DataLogStructure.Record, DAM As IDAM)

        Using genericProvider As New DbGenericProvider(DAM)
            Using bizPaz As New BizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                bizPaz.UpdateCircoscrizione(OnVacUtility.Variabili.PazId, codiceCircoscrizioneNew, isCircoscrizioneResidenza, recordLog)

            End Using
        End Using

    End Sub

    Private Sub CaricaCampiObbligatori(dam As IDAM)

        Using genericProvider As New DbGenericProvider(dam)
            Using bizPaz As New BizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Dim campiRichiesti As ArrayList = bizPaz.GetCampiAnagraficiObbligatori(OnVacUtility.Variabili.PazId, True, txtCittadinanza.Codice)

                ' Coloro di giallo i campi obbligatori del pannello
                ImpostaCampiObbligatori(campiRichiesti)

            End Using
        End Using

    End Sub

    Private Sub SetStatiAnagraficiCancellazioneProgrammazione()

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizStatiAnagrafici As New BizStatiAnagrafici(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                StatiAnagraficiCancellazioneProgrammazione = bizStatiAnagrafici.GetStatiAnagraficiCancellazioneProgrammazione()

            End Using
        End Using

    End Sub

    Private Function ScritturaCentrale() As Boolean

        Select Case Settings.TIPOANAG

            Case Enumerators.TipoAnags.CentraleLettScritt '...
                Return True

            Case Enumerators.TipoAnags.CentraleScrittInsLettAgg
                Return True

        End Select

        Return False

    End Function

    Private Function ScritturaCentrale(row As DataRow) As Boolean

        Select Case Me.Settings.TIPOANAG

            Case Enumerators.TipoAnags.CentraleLettScritt '...
                Return True

            Case Enumerators.TipoAnags.CentraleScrittInsLettAgg

                Select Case row.RowState
                    Case DataRowState.Added
                        Return True

                    Case DataRowState.Modified

                End Select

        End Select

        Return False

    End Function

    Private Sub EliminaProgrammazione(eliminaAppuntamenti As Boolean, eliminaSollecitiBilancio As Boolean, idMotivoEliminazione As String, noteEliminazione As String)

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizVaccinazioneProg As New BizVaccinazioneProg(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Dim command As New BizVaccinazioneProg.EliminaProgrammazioneCommand()
                command.CodicePaziente = Convert.ToInt64(OnVacUtility.Variabili.PazId)
                command.DataConvocazione = Nothing
                command.EliminaAppuntamenti = eliminaAppuntamenti
                command.EliminaBilanci = False
                command.EliminaSollecitiBilancio = eliminaSollecitiBilancio
                command.TipoArgomentoLog = DataLogStructure.TipiArgomento.ELIMINA_PROG
                command.OperazioneAutomatica = False
                command.IdMotivoEliminazione = idMotivoEliminazione
                command.NoteEliminazione = noteEliminazione

                bizVaccinazioneProg.EliminaProgrammazione(command)

            End Using
        End Using

    End Sub


    ' --------- '
    ' TODO [PazScambiati]: gestire blocco del salvataggio => da finire

    'Private Sub odpDettaglioPaziente_onError(sender As OnitDataPanel.OnitDataPanel, err As OnitDataPanel.OnitDataPanelError) Handles odpDettaglioPaziente.onError

    '    If err.exc IsNot Nothing AndAlso err.exc.GetType().Name = GetType(PazientiScambiatiException).Name Then

    '        err.mute = True

    '    End If

    'End Sub
    ' --------- '


#End Region

End Class
