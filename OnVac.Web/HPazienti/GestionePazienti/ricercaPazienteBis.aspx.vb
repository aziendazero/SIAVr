Imports System.Collections.Generic
Imports Onit.Controls.OnitDataPanel.OnitDataPanel
Imports Onit.Database.DataAccessManager


Partial Class ricercaPazienteBis
    Inherits Common.PageBase

#Region " Public property "

    ' Proprietà utilizzate dal file di configurazione dell'onitdatapanel.

    Public ReadOnly Property ottieniDam() As IDAM
        Get
            If Not Me.IsInDesignTime() Then
                Return OnVacUtility.OpenDam()
            Else
                Return Nothing
            End If
        End Get
    End Property

    ' N.B. : Questa proprietà è utilizzata dal file xml del pannello, ma non serve valorizzarla perchè la ricerca in centrale 
    '        è gestita dalla classe PazienteHL7OdpAdapter e non più tramite il servizio esterno Paziente_HL7.
    Public ReadOnly Property ottieniServizio() As String
        Get
            Return String.Empty
        End Get
    End Property

    Private Function IsInDesignTime() As Boolean
        Return Not IsNothing(Me.Site) AndAlso Me.Site.DesignMode
    End Function

#End Region

#Region " Private property "

    Private Property SearchPerformed() As Boolean
        Get
            Return ViewState(Me.ID + "searchPerformed")
        End Get
        Set(Value As Boolean)
            ViewState(Me.ID + "searchPerformed") = Value
        End Set
    End Property

    Private Property BlnSceltoLocale() As Boolean
        Get
            Return ViewState("blnSceltoLocale")
        End Get
        Set(Value As Boolean)
            ViewState("blnSceltoLocale") = Value
        End Set
    End Property

    ''' <summary>
    ''' Indica se la modale di rilevazione del consenso si è aperta automaticamente a causa di un tentativo di redirect al dettaglio di un paziente con stato consenso bloccante
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property AperturaAutomaticaRilevazioneConsensoBloccante() As Boolean
        Get
            If ViewState("AARC") Is Nothing Then ViewState("AARC") = False
            Return ViewState("AARC")
        End Get
        Set(Value As Boolean)
            ViewState("AARC") = Value
        End Set
    End Property

#End Region

#Region " Private variables "

    ' Flag che indica se deve essere eseguita la ricerca con il pannello
    Private doDataPanelSearch As Boolean = False

    ' Tipologia di connessione anagrafica corrente
    Private tipoAnag As Enumerators.TipoAnags

    Dim iconeConsensi As GestioneConsenso.Wcf.Proxy.ConsensiService.IconeConsensi

#End Region

#Region " Consts "

    Private Const CONFIRM_MERGE As String = "CM"
    Private Const OPEN_RILEVAZIONE_CONSENSO As String = "OPEN_CONS"

#End Region

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
        tipoAnag = Settings.TIPOANAG
    End Sub

#End Region

#Region " Eventi pagina "

    Protected Overrides Sub OnLoad(e As EventArgs)

        MyBase.OnLoad(e)

        'imposta lo switch centrale locale attraverso il radio button
        If Not IsPostBack Then
            BlnSceltoLocale = False
        End If

        Dim rblCentraleLocale As RadioButtonList = tabRicerca.FindControl("rblCentraleLocale")
        Dim lblCentraleLocale As Label = tabRicerca.FindControl("lblCentraleLocale")

        If Not rblCentraleLocale Is Nothing AndAlso Not lblCentraleLocale Is Nothing Then
            If tipoAnag = Enumerators.TipoAnags.CentraleLettLocaleAgg Then
                AddHandler rblCentraleLocale.SelectedIndexChanged, AddressOf rblCentraleLocale_SelectedIndexChanged
                rblCentraleLocale.Visible = True
                lblCentraleLocale.Visible = True
            Else
                rblCentraleLocale.Visible = False
                lblCentraleLocale.Visible = False
            End If
        End If

        If Not IsPostBack() Then

            ImpostaFocus()

            SearchPerformed = False

            If Not rblCentraleLocale Is Nothing Then
                rblCentraleLocale.Enabled = False
            End If

        End If

        Select Case Request.Form("__EVENTTARGET")

            Case "cerca"
                doDataPanelSearch = True

            Case "RefreshFromPopup"
                '--
                ' Se ho appena chiuso la modale di rilevazione del consenso, che si era aperta automaticamente perchè lo stato del cons del paz era bloccante,
                ' allora ricontrollo lo stato del consenso e, se non è più bloccante, faccio subito il redirect al dettaglio del paziente selezionato.
                '--
                Dim redirect As Boolean = False
                '--
                If AperturaAutomaticaRilevazioneConsensoBloccante Then
                    '--
                    AperturaAutomaticaRilevazioneConsensoBloccante = False
                    '--
                    ' N.B. : al click del pulsante di dettaglio viene memorizzato il paziente selezionato, poi viene aperta la pop-up di rilevazione.
                    '        in questo punto, sfrutto il codice memorizzato nella struttura per fare il redirect, quindi deve essere stata valorizzata!   
                    '--
                    Dim consensoGlobalePaziente As Entities.Consenso.StatoConsensoPaziente =
                        OnVacUtility.GetConsensoUltimoPazienteSelezionato(UltimoPazienteSelezionato, Settings)
                    '--
                    If consensoGlobalePaziente Is Nothing Then
                        '--
                        redirect = False
                        '--
                    ElseIf consensoGlobalePaziente.Controllo = Enumerators.ControlloConsenso.Bloccante Then
                        '--
                        redirect = False
                        '--
                    ElseIf (IsProntoSoccorso Or IsGestioneCentrale) AndAlso consensoGlobalePaziente.BloccoAccessiEsterni Then
                        '--
                        redirect = False
                        AlertClientMsg(consensoGlobalePaziente.DescrizioneStatoConsenso + Environment.NewLine + "Impossibile accedere ai dati del paziente.")
                        '--
                    Else
                        '--
                        redirect = True
                        RedirectToDetail(UltimoPazienteSelezionato.CodicePazienteLocale, UltimoPazienteSelezionato.CodicePazienteCentrale)
                        '--
                    End If
                    '--
                End If
                '--
                ' Se non effettuo il redirect, eseguo la ricerca per aggiornare i dati
                If Not redirect Then
                    doDataPanelSearch = True
                End If
                '--
        End Select

    End Sub

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Pulisco il valore di OnVacUtility.Variabili.PazId
        OnVacUtility.ClearPazId()

        'Pulizia e log della session
        Dim sc As New SessionCleaner(Settings)
        sc.Start()

        'Pulizia dell'history
        DirectCast(Page, Onit.Shared.Web.UI.Page).HistoryClear()

        If (Not IsNothing(Request.QueryString.Item("menu_dis"))) Then
            OnVacContext.MenuDis = Request.QueryString.Item("menu_dis").ToString()
        End If

        OnVacAlias1 = fmOnVacAlias.FindControl("OnVacAlias1")

        Dim fmComNascita As Onit.Controls.OnitDataPanel.wzFinestraModale = tabRicerca.FindControl("WzFinestraModale1")
        Dim fmConsultorio As Onit.Controls.OnitDataPanel.wzFinestraModale = tabRicerca.FindControl("omlConsultorio")

        If Not IsPostBack Then

            ' Visualizzazione pulsanti toolbar
            Dim showButtonAlias As Boolean

            If Request.QueryString("alias") = "true" Then
                showButtonAlias = True
                OnitLayout31.Titolo = "Gestione Alias"
            Else
                showButtonAlias = False

                If IsGestioneCentrale Then
                    OnitLayout31.Titolo = "Ricerca Paziente Centrale"
                ElseIf IsProntoSoccorso Then
                    OnitLayout31.Titolo = "Pronto Soccorso"
                Else
                    OnitLayout31.Titolo = "Ricerca Paziente"
                End If
            End If

            Dim isRicercaStandard As Boolean = Not IsGestioneCentrale AndAlso Not IsProntoSoccorso

            ToolBar.Items.FromKeyButton("btnNew").Visible = isRicercaStandard
            ToolBar.Items.FromKeyButton("btnAlias").Visible = showButtonAlias AndAlso isRicercaStandard
            ToolBar.Items.FromKeyButton("btnUltimoPaz").Visible = isRicercaStandard
            ToolBar.Items.FromKeyButton("btnUltimaRicerca").Visible = isRicercaStandard
            If Not isRicercaStandard Then
                Dim separator As Infragistics.WebUI.UltraWebToolbar.TBSeparator = ToolBar.Items.FromKeySeparator("sepRicercheRapide")
                If Not separator Is Nothing Then ToolBar.Items.Remove(separator)
            End If

            ' Aggiunta del consultorio di lavoro al titolo
            OnVacUtility.ImpostaCnsLavoro(OnitLayout31)

            fmOnVacAlias.VisibileMD = False

            If Not showButtonAlias Then
                ToolBar.Items.Remove(ToolBar.Items.FromKeyButton("btnAlias"))
                WzMsDataGrid1.EditMode = DetailEditMode.None
            Else
                WzMsDataGrid1.EditMode = DetailEditMode.ShowDetail
            End If

            ' Filtro sul consultorio
            ' Da visualizzare solo se RICERCA_PAZ_SHOW_FILTRO_CNS = 'S'
            ' Da impostare solo se RICERCA_PAZ_FILTRO_CNS_SET_DEFAULT = 'S'
            If Settings.RICERCA_PAZ_SHOW_FILTRO_CNS Then
                fmConsultorio.Visible = True
                If Settings.RICERCA_PAZ_FILTRO_CNS_SET_DEFAULT Then
                    fmConsultorio.Codice = OnVacUtility.Variabili.CNS.Codice
                    fmConsultorio.RefreshDataBind()
                End If
            Else
                fmConsultorio.Visible = False
                Dim lblCns As Label = tabRicerca.FindControl("lblFiltroCns")
                If Not lblCns Is Nothing Then lblCns.Visible = False
            End If

            ' Filtro sul comune di residenza
            lblComuneResidenza.Visible = Settings.RICERCA_PAZ_SHOW_FILTRO_COMUNE_RESIDENZA
            fmComuneResidenza.Visible = Settings.RICERCA_PAZ_SHOW_FILTRO_COMUNE_RESIDENZA

            '-- MGR abilito il campo codice solo se l'operatore appartiene ad un certo gruppo
            trCodicePaziente.Visible = False
            lblPazCodice.Visible = False
            txtPazCodice.Visible = False

            ShowDgrColumn("Codice", False)
            ShowDgrColumn("Ausiliario", False)
            ShowDgrColumn("ULSSD", False)

            If Not String.IsNullOrEmpty(Settings.ID_GRUPPO_SUPERUSER) Then
                '--
                Dim isSuperUser As Boolean = OnVacUtility.IsCurrentUserInGroup(Settings.ID_GRUPPO_SUPERUSER)
                '--
                trCodicePaziente.Visible = isSuperUser
                lblPazCodice.Visible = isSuperUser
                txtPazCodice.Visible = isSuperUser
                '--
                ShowDgrColumn("Codice", isSuperUser)
                ShowDgrColumn("Ausiliario", isSuperUser)
                '--
            End If
            '-- fine MGR

            ' --- Gestione consenso --- '
            ' Visualizzazione pulsante "Consensi" e colonna del datagrid in base al parametro di gestione dei consensi
            ToolBar.Items.FromKeyButton("btnConsenso").Visible = Settings.CONSENSO_GES
            DirectCast(WzMsDataGrid1.getColumnByKey("StatoConsenso"), Onit.Controls.OnitDataPanel.wzTemplateColumn).Visible = Settings.CONSENSO_GES
            ' ------------------------- '

        End If


        Dim script As New System.Text.StringBuilder()
        script.AppendLine("<script type='text/javascript'>")
        script.AppendFormat("var idFmNascita='{0}';", WzFinestraModale1.ClientID).AppendLine()

        If Settings.RICERCA_PAZ_SHOW_FILTRO_COMUNE_RESIDENZA Then
            script.AppendFormat("var idFmResidenza='{0}';", fmComuneResidenza.ClientID).AppendLine()
        Else
            script.AppendLine("var idFmResidenza='';")
        End If

        script.AppendLine("</script>")

        ClientScript.RegisterClientScriptBlock(Me.GetType(), "checkValiditaFm", script.ToString())

    End Sub

    Protected Overrides Sub OnPreRender(e As System.EventArgs)

        If doDataPanelSearch Then

            SetDatiUltimaRicercaEffettuata()

            If UltimaRicercaPaziente.IsEmpty() Then
                EseguiRicercaUltimoPaziente()
            Else
                EseguiUltimaRicercaEffettuata()
            End If

        End If

        MyBase.OnPreRender(e)

    End Sub

#End Region

#Region " Gestione del datapanel "

    Private Sub odpRicercaPaziente_afterLoadData(sender As Controls.OnitDataPanel.OnitDataPanel, ByRef dtData As System.Data.DataTable) Handles odpRicercaPaziente.afterLoadData

        If Settings.CONSENSO_GES Then

            Dim listaCodiciPaziente As New List(Of String)

            Dim cFieldLocale As String = sender.GetCurrentTableEncoder.getCode("t_paz_pazienti", "paz_codice_ausiliario")
            Dim cFieldCentrale As String = sender.GetCurrentTableEncoder.getCode("t_paz_pazienti_centrale", "paz_codice")

            Dim vFieldLocale As Object = Nothing
            Dim vFieldCentrale As Object = Nothing

            For i As Integer = 0 To dtData.Rows.Count - 1

                Dim drow As DataRow = dtData.Rows(i)

                If dtData.Columns.Contains(cFieldCentrale) Then vFieldCentrale = drow(cFieldCentrale)
                If dtData.Columns.Contains(cFieldLocale) Then vFieldLocale = drow(cFieldLocale)

                If Not vFieldCentrale Is Nothing AndAlso Not IsDBNull(vFieldCentrale) AndAlso vFieldCentrale.ToString <> String.Empty Then
                    listaCodiciPaziente.Add(vFieldCentrale.ToString())
                Else
                    listaCodiciPaziente.Add(vFieldLocale.ToString())
                End If

            Next

            If listaCodiciPaziente.Count > 0 Then

                Dim codiceAziendaRegistrazione As String = OnVacUtility.GetCodiceAziendaRegistrazione(Settings)

                Using consenso As New Onit.OnAssistnet.GestioneConsenso.Wcf.Proxy.ConsensiService.ConsensiServiceClient()
                    iconeConsensi = consenso.GetUltimoConsensoIconaPazienti(listaCodiciPaziente.ToArray(), OnVacContext.Azienda, Settings.CONSENSO_APP_ID, codiceAziendaRegistrazione)
                End Using

            Else
                iconeConsensi = New Onit.OnAssistnet.GestioneConsenso.Wcf.Proxy.ConsensiService.IconeConsensi()
            End If

        End If

    End Sub

    Private Sub odpRicercaPaziente_beforeLoadData(sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles odpRicercaPaziente.beforeLoadData

        'imposto il pannello a seconda dei parametri dell'applicativo
        Select Case tipoAnag

            Case Enumerators.TipoAnags.SoloLocale 'ok
                ImpostaCentrale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.none, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)
                ImpostaLocale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.read, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)
                'impostazione max numero di pazienti in locale
                odpRicercaPaziente.maxRecord = Settings.RICERCA_PAZ_MAX_RECORDS
                odpRicercaPaziente.Optimizer.useAlternativePlan = False

            Case Enumerators.TipoAnags.CentraleLettura
                ImpostaCentrale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.read, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)
                ImpostaLocale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.read, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)
                odpRicercaPaziente.Optimizer.useAlternativePlan = True

            Case Enumerators.TipoAnags.CentraleLettScritt
                ImpostaCentrale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.read, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)
                ImpostaLocale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.read, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)
                odpRicercaPaziente.Optimizer.useAlternativePlan = True

            Case Enumerators.TipoAnags.CentraleScrittInsLettAgg
                ImpostaCentrale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.read, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)
                ImpostaLocale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.read, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)
                odpRicercaPaziente.Optimizer.useAlternativePlan = True

            Case Enumerators.TipoAnags.CentraleLettLocaleAgg
                If BlnSceltoLocale Then
                    ImpostaCentrale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.none, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)
                    ImpostaLocale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.read, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)
                    'impostazione max numero di pazienti in locale
                    odpRicercaPaziente.maxRecord = Settings.RICERCA_PAZ_MAX_RECORDS
                    odpRicercaPaziente.Optimizer.useAlternativePlan = False
                Else
                    'centrale
                    ImpostaCentrale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.read, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)
                    ImpostaLocale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.none, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)
                    odpRicercaPaziente.Optimizer.useAlternativePlan = True
                End If

        End Select

    End Sub

    Private Sub ImpostaCentrale(readAu As Onit.Controls.OnitDataPanel.Connection.readAuthorizations, writeAu As Onit.Controls.OnitDataPanel.Connection.writeAuthorizations)

        odpRicercaPaziente.Connections(Constants.ConnessioneAnagrafe.CENTRALE).ReadAuth = readAu
        odpRicercaPaziente.Connections(Constants.ConnessioneAnagrafe.CENTRALE).WriteAuth = writeAu

        ' Imposto il parametro per il recupero del messaggio inerente al superamento del limite massimo di elementi in fase di ricerca 
        If odpRicercaPaziente.Connections(Constants.ConnessioneAnagrafe.CENTRALE).Parameters.Item("strErrMsg") Is Nothing Then

            Dim p As New Onit.Controls.OnitDataPanel.PanelParameter("strErrMsg")

            p.valueProvenience = Onit.Controls.OnitDataPanel.PanelParameter.Proveniences.valueProperty
            p.Enabled = True
            p.Type = Onit.Controls.OnitDataPanel.PanelParameter.Types.String
            p.value = String.Empty
            p.Direction = Onit.Controls.OnitDataPanel.PanelParameter.Directions.InOut

            odpRicercaPaziente.Connections(Constants.ConnessioneAnagrafe.CENTRALE).Parameters.Add(p)

        End If

        odpRicercaPaziente.Connections(Constants.ConnessioneAnagrafe.LOCALE_CNAS).ReadAuth = readAu
        odpRicercaPaziente.Connections(Constants.ConnessioneAnagrafe.LOCALE_CNAS).WriteAuth = Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none

        odpRicercaPaziente.Connections(Constants.ConnessioneAnagrafe.LOCALE_COMRE).ReadAuth = readAu
        odpRicercaPaziente.Connections(Constants.ConnessioneAnagrafe.LOCALE_COMRE).WriteAuth = Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none

    End Sub

    Private Sub ImpostaLocale(readAu As Onit.Controls.OnitDataPanel.Connection.readAuthorizations, writeAu As Onit.Controls.OnitDataPanel.Connection.writeAuthorizations)
        odpRicercaPaziente.Connections(Constants.ConnessioneAnagrafe.LOCALE).ReadAuth = readAu
        odpRicercaPaziente.Connections(Constants.ConnessioneAnagrafe.LOCALE).WriteAuth = writeAu
    End Sub

    Private Sub odpRicercaPaziente_afterBarOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpRicercaPaziente.afterBarOperation

        If operation = Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.Find Then

            SetDatiUltimaRicercaEffettuata()

            'fase 2  della ricerca:
            '   testo se non ho trovato niente in centrale, nel qual caso conduco una ricerca solo in locale
            If tipoAnag = Enumerators.TipoAnags.CentraleLettura Or tipoAnag = Enumerators.TipoAnags.CentraleScrittInsLettAgg Then

                If odpRicercaPaziente.getRecordCount() <= 0 Then

                    tipoAnag = Enumerators.TipoAnags.SoloLocale
                    odpRicercaPaziente.Find()
                    tipoAnag = Settings.TIPOANAG

                    AlertClientMsg("Non è stato trovato alcun paziente in centrale. La ricerca è stata condotta in locale")

                End If

            End If

        End If

    End Sub

    Private Sub odpRicercaPaziente_beforeOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpRicercaPaziente.beforeOperation

        If operation = Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord Then

            'quando si effettua un inserimento in modalità 4 ci si assicura di essere in locale
            If tipoAnag = Enumerators.TipoAnags.CentraleLettLocaleAgg Then
                BlnSceltoLocale = True
                ImpostaCentrale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.none, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)
                ImpostaLocale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.read, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)
            End If

        End If

    End Sub

    Private Sub odpRicercaPaziente_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpRicercaPaziente.afterOperation

        Dim intTrovati As Integer

        If operation = Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.Find Then

            SearchPerformed = True

            odpRicercaPaziente_controlLayoutChanging(sender, operation)

            If Not labelRisultati Is Nothing Then

                intTrovati = odpRicercaPaziente.getRecordCount()
                labelRisultati.Text = "Risultati della ricerca: " + intTrovati.ToString() + If(intTrovati = 1, " paziente trovato", " pazienti trovati")

                'testo se è stato raggiunto il numero massimo di record consentito
                If Not odpRicercaPaziente.Connections(Constants.ConnessioneAnagrafe.CENTRALE) Is Nothing Then
                    If odpRicercaPaziente.Connections(Constants.ConnessioneAnagrafe.CENTRALE).ReadAuth = Onit.Controls.OnitDataPanel.Connection.readAuthorizations.read AndAlso
                       Not odpRicercaPaziente.Connections(Constants.ConnessioneAnagrafe.CENTRALE).Parameters("strErrMsg") Is Nothing Then

                        Dim str As String = odpRicercaPaziente.Connections(Constants.ConnessioneAnagrafe.CENTRALE).Parameters("strErrMsg").value.ToString().Trim()
                        If str <> "" Then
                            AlertClientMsg(str)
                        End If

                    End If
                End If
                'fine riscontro massimo numero di record raggiunto

            End If

        End If

    End Sub

    'gestisce il rimbalzo al master in caso di ricerca non locale.
    Private Sub odpRicercaPaziente_onSendDetailStruct(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, detailStruct As Onit.Controls.OnitDataPanel.infoDetailStruct) Handles odpRicercaPaziente.onSendDetailStruct

        Dim currentRow As DataRow = odpRicercaPaziente.getCurrentDataRow()

        If tipoAnag <> Enumerators.TipoAnags.SoloLocale AndAlso Not IsNothing(currentRow) Then

            Dim codicePaziente As String = Nothing

            If detailStruct.operation <> CurrentOperationTypes.NewRecord Then

                Dim blnRimbalzaAlMaster As Boolean = tipoAnag <> Enumerators.TipoAnags.SoloLocale

                If tipoAnag = Enumerators.TipoAnags.CentraleLettLocaleAgg And BlnSceltoLocale Then
                    blnRimbalzaAlMaster = False
                End If

                ' Se il paziente selezionato è un alias, prendo il codice del paziente master relativo
                If blnRimbalzaAlMaster Then

                    Dim tipoPaziente As String = odpRicercaPaziente.GetCurrentTableEncoder.getValOf(currentRow, Constants.ConnessioneAnagrafe.CENTRALE, "paz_tipo", "t_paz_pazienti_centrale", True).ToString()

                    If String.Compare(tipoPaziente, "A", True) = 0 Then

                        ' Ricavo il codice del master
                        codicePaziente = odpRicercaPaziente.GetCurrentTableEncoder.getValOf(currentRow, Constants.ConnessioneAnagrafe.CENTRALE, "paz_alias", "t_paz_pazienti_centrale", True).ToString()

                        detailStruct.alertMessage = "Il paziente scelto era un alias. Sono stati caricati i dati reali del paziente master"

                    End If

                End If

                ' Se il paziente selezionato non è un alias, prendo il codice centrale
                If String.IsNullOrEmpty(codicePaziente) Then
                    codicePaziente = odpRicercaPaziente.GetCurrentTableEncoder.getValOf(currentRow, "paz_codice", "t_paz_pazienti_centrale", True).ToString()
                End If

                ' Altrimenti prendo il codice locale
                If String.IsNullOrEmpty(codicePaziente) Then
                    codicePaziente = odpRicercaPaziente.GetCurrentTableEncoder.getValOf(currentRow, "paz_codice", "t_paz_pazienti", True).ToString()
                End If

                ' N.B.  Aggiunta di un solo filtro col codice del paziente selezionato per gestire il caso 
                '       che l 'utente abbia chekkato + pazienti e cliccato su conferma
                detailStruct.filters.Clear()

                detailStruct.filters.addCollection(odpRicercaPaziente.getSelectionFilters(True))

                detailStruct.filters(0).Value = codicePaziente

            End If

        End If

        'toppa Antimo 22-10-04
        'Altrimenti se si fa ua ricerca e poi Inserisci esegue una query che carica un numero di record
        'tale da bloccare il programma...quando verrà risolto il problema nell ricercaPazientiSharef
        'questo codice non servirà +
        If detailStruct.operation = CurrentOperationTypes.NewRecord Then
            Dim f As New Onit.Controls.OnitDataPanel.Filter
            f.connectionName = Constants.ConnessioneAnagrafe.LOCALE
            f.Comparator = Onit.Controls.OnitDataPanel.Filter.FilterComparators.AlwaysFalse
            f.[Operator] = Onit.Controls.OnitDataPanel.Filter.FilterOperators.And
            f.FieldType = Onit.Controls.OnitDataPanel.Filter.FieldTypes.Null
            detailStruct.filters.Add(f)
        End If

    End Sub

    'ordinamento ricerca
    Private Sub odpRicercaPaziente_onCreateQuery(ByRef QB As Object) Handles odpRicercaPaziente.onCreateQuery

        If tipoAnag = Enumerators.TipoAnags.SoloLocale Then
            '--
            Dim _qb As AbstractQB = DirectCast(QB, AbstractQB)

            Dim strSql As String = _qb.GetSelect()
            '--
            If strSql.ToLower().IndexOf("t_paz_pazienti") >= 0 Then
                '--
                'si sta eseguendo la query sulla tabella pazienti locale
                '--
                ' Filtro anno nascita
                If Not String.IsNullOrEmpty(txtAnnoNascita.Text) Then

                    Dim annoNascita As Integer = Convert.ToInt32(txtAnnoNascita.Text)

                    Dim abstractQB As AbstractQB = DirectCast(QB, AbstractQB)
                    abstractQB.AddWhereCondition("paz_data_nascita", Comparatori.MaggioreUguale, New DateTime(annoNascita, 1, 1), DataTypes.Data)
                    abstractQB.AddWhereCondition("paz_data_nascita", Comparatori.Minore, New DateTime(annoNascita + 1, 1, 1), DataTypes.Data)

                End If
                '--
                ' Ordinamento
                If Not String.IsNullOrWhiteSpace(Settings.RICERCA_PAZ_ORDINAMENTO) Then

                    Dim strFields As String() = Settings.RICERCA_PAZ_ORDINAMENTO.Split(",")

                    For Each strField As String In strFields
                        If strField.Trim() <> "" Then
                            _qb.AddOrderByFields(strField)
                        End If
                    Next

                End If
                '--
            End If
        End If

    End Sub

    Private Sub odpRicercaPaziente_controlLayoutChanging(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpRicercaPaziente.controlLayoutChanging

        Dim isRicercaStandard As Boolean = Not IsGestioneCentrale AndAlso Not IsProntoSoccorso

        ' Abilitazione pulsante inserimento paziente (solo se il parametro relativo è true e solo dopo la prima ricerca)
        Dim btnNew As Infragistics.WebUI.UltraWebToolbar.TBarButton = ToolBar.Items.FromKeyButton("btnNew")
        If Not btnNew Is Nothing Then
            btnNew.Enabled = Settings.INSERIMENTO_PAZIENTE_ABILITATO AndAlso SearchPerformed AndAlso isRicercaStandard
        End If

        Dim rblCentraleLocale As System.Web.UI.WebControls.RadioButtonList = tabRicerca.FindControl("rblCentraleLocale")
        If Not rblCentraleLocale Is Nothing Then
            rblCentraleLocale.Enabled = SearchPerformed
        End If

        Dim enable As Boolean = (odpRicercaPaziente.getRecordCount() > 0)

        ' --- Gestione consenso --- '
        ' Abilitazione/disabilitazione pulsante Consenso
        Dim btnConsenso As Infragistics.WebUI.UltraWebToolbar.TBarButton = ToolBar.Items.FromKeyButton("btnConsenso")
        If Not btnConsenso Is Nothing Then
            btnConsenso.Enabled = enable
        End If
        ' ------------------------- '

        ' Pulsante Conferma 
        Dim btnConferma As Infragistics.WebUI.UltraWebToolbar.TBarButton = ToolBar.Items.FromKey("btnEdit2")
        If Not btnConferma Is Nothing Then
            btnConferma.Enabled = enable
        End If

        ' Pulsante Alias
        Dim btnAlias As Infragistics.WebUI.UltraWebToolbar.TBarButton = ToolBar.Items.FromKey("btnAlias")
        If Not btnAlias Is Nothing Then
            btnAlias.Enabled = enable And isRicercaStandard
        End If

    End Sub

#End Region

#Region " Eventi Tab Infragistics "

    ' Gestito per effettuare il filtro sull'anno in caso di ricerca in centrale
    Private Sub tabRicerca_AfterCreateFilters(sender As Controls.OnitDataPanel.wzFilter, Filters As Controls.OnitDataPanel.FilterCollection) Handles tabRicerca.AfterCreateFilters

        If Filters Is Nothing OrElse Filters.Count = 0 Then Return

        If tipoAnag <> Enumerators.TipoAnags.SoloLocale Then

            ' CODICE PAZIENTE: se ricerca in centrale, anche il filtro relativo al codice deve andare in centrale (con il valore del codice centrale!)
            For Each filter As Onit.Controls.OnitDataPanel.Filter In Filters
                If filter.Field = "PAZ_CODICE" Then
                    filter.connectionName = "centrale"
                    filter.TableName = "t_paz_pazienti_centrale"
                End If
            Next

            ' ANNO DI NASCITA
            If Not String.IsNullOrEmpty(txtAnnoNascita.Text) Then

                Dim filterAnnoNascita As New Controls.OnitDataPanel.Filter()
                filterAnnoNascita.connectionName = "centrale"
                filterAnnoNascita.TableName = "t_paz_pazienti_centrale"
                filterAnnoNascita.Field = Constants.CommonConstants.ANNO_NASCITA
                filterAnnoNascita.Value = txtAnnoNascita.Text
                filterAnnoNascita.FieldType = Onit.Controls.OnitDataPanel.Filter.FieldTypes.Number
                filterAnnoNascita.valueProvenience = Onit.Controls.OnitDataPanel.Filter.ValueProveniences.valueProperty
                filterAnnoNascita.Operator = Onit.Controls.OnitDataPanel.Filter.FilterOperators.And

                Filters.Add(filterAnnoNascita)

            End If

        End If

    End Sub

#End Region

#Region " Switch centrale locale - modo 4 "

    Private Sub rblCentraleLocale_SelectedIndexChanged(sender As Object, e As System.EventArgs)

        BlnSceltoLocale = (DirectCast(sender, RadioButtonList).SelectedValue = Constants.ConnessioneAnagrafe.LOCALE)

    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub ToolBar_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case be.Button.Key

            Case "btnAlias"
                ' Apertura usercontrol per esecuzione alias
                GestioneAlias()

            Case "btnEdit2"
                ' Redirect a dettaglio paziente
                DettaglioPazienteClick()

            Case "btnConsenso"

                If Settings.CONSENSO_LOCALE Then
                    '--
                    ' Consenso gestito localmente e paziente non presente in locale => non rilevo il consenso
                    '--
                    Dim codicePazienteLocale As Object =
                        odpRicercaPaziente.GetCurrentTableEncoder().getValOf(odpRicercaPaziente.getCurrentDataRow(), "paz_codice", "t_paz_pazienti", True)
                    '--
                    If codicePazienteLocale Is Nothing OrElse codicePazienteLocale Is DBNull.Value OrElse String.IsNullOrEmpty(codicePazienteLocale.ToString()) Then
                        '--
                        OnitLayout31.ShowMsgBox(New Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Il paziente non è presente in locale: impossibile rilevare il consenso.", "no_consenso_paz", False, False))
                        Return
                        '--
                    End If
                    '--
                End If

                ' Apertura popup rilevazione consenso
                ApriRilevazioneConsenso(False)

            Case "btnUltimoPaz"
                ' Ricerca dell'ultimo paziente selezionato
                If Not EseguiRicercaUltimoPaziente() Then
                    OnitLayout31.ShowMsgBox(New Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Ricerca non effettuata: nessun paziente selezionato in precedenza", "no_ultimo_paz", False, False))
                End If

            Case "btnUltimaRicerca"

#If DEBUG Then
                'If False Then

#Region " [COVID-19]: Test Report "

                ' [COVID-19]: prova report ComunicazioneSorveglianzaCOVID19.rpt
                'RegisterStartupScriptCustom("openHandlerTestCertificatoCOV",
                '    String.Format("window.open('{0}?paz={1}&ute={2}&app={3}&azi={4}&cns={5}&ulss={6}&rpt=COV');", ResolveClientUrl("~/Common/Handlers/Handler1.ashx"),
                '                  String.Empty, OnVacContext.UserId.ToString(), OnVacContext.AppId,
                '                  OnVacContext.Azienda, OnVacUtility.Variabili.CNS.Codice, OnVacContext.CodiceUslCorrente))

                ' [COVID-19]: prova report CertNegativizzazioneCOVID19.rpt
                'RegisterStartupScriptCustom("openHandlerTestCertificatoCOV",
                '   String.Format("window.open('{0}?paz={1}&ute={2}&app={3}&azi={4}&cns={5}&ulss={6}&rpt=COVNEG&ep={7}');", ResolveClientUrl("~/Common/Handlers/Handler1.ashx"),
                '                 560040, OnVacContext.UserId.ToString(), OnVacContext.AppId,
                '                 OnVacContext.Azienda, OnVacUtility.Variabili.CNS.Codice, OnVacContext.CodiceUslCorrente,
                '                 85))

                ' [COVID-19]: prova report CertTestAntigeneRapidoNegativoCOVID19.rpt
                'RegisterStartupScriptCustom("openHandlerTestCertificatoCOV",
                '    String.Format("window.open('{0}?paz={1}&ute={2}&app={3}&azi={4}&cns={5}&ulss={6}&rpt=COVTESTNEG');", ResolveClientUrl("~/Common/Handlers/Handler1.ashx"),
                '                  560040, OnVacContext.UserId.ToString(), OnVacContext.AppId,
                '                  OnVacContext.Azienda, OnVacUtility.Variabili.CNS.Codice, OnVacContext.CodiceUslCorrente))

                ' [COVID-19]: prova report CertTestAntigeneRapidoNegativoCOVID19_TAR.rpt
                'RegisterStartupScriptCustom("openHandlerTestCertificatoCOV",
                '    String.Format("window.open('{0}?campione={1}&centro={2}&ute={3}&app={4}&azi={5}&cns={6}&ulss={7}&rpt=COVTESTNEG_TAR');", ResolveClientUrl("~/Common/Handlers/Handler1.ashx"),
                '                  1016233, "95", OnVacContext.UserId.ToString(), OnVacContext.AppId,
                '                  OnVacContext.Azienda, OnVacUtility.Variabili.CNS.Codice, OnVacContext.CodiceUslCorrente))

#End Region

#Region " [COVID-19]: Test invio OTP a SAR"

                ' [COVID-19]: prova metodo BizRilevazioniCovid19.ComunicaOTP
                'Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                '    Using biz As New Biz.BizRilevazioniCovid19(genericProvider, Settings, OnVacContext.CreateBizContextInfos(OnVacUtility.Variabili.CNS.Codice))

                '        Dim command As New Biz.BizRilevazioniCovid19.ComunicaOTPCommand()
                '        command.DataInizioSintomi = Date.Now.AddDays(-10)
                '        command.IdUtente = 8    '1
                '        command.OTP = "5T0C4770"

                '        Dim result As Biz.BizGenericResult = biz.ComunicaOTP(command)

                '        Dim a As String = result.Message

                '    End Using
                'End Using

#End Region

#Region " [COVID-19]: Test inserimento paziente in centrale e locale "

                ''[COVID-19]: test inserimento paziente in centrale e locale
                'Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                '    Using biz As New Biz.BizCovid19Tamponi(genericProvider, Settings, OnVacContext.CreateBizContextInfos(OnVacUtility.Variabili.CNS.Codice))

                'pazienteVac = GenericProvider.Paziente.GetPazienti(codicePaziente)[0];
                'Dim codicePaziente As Integer = 640733
                'Dim pazienteVac As New Entities.Paziente
                'Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                '    pazienteVac = genericProvider.Paziente.GetPazienti(codicePaziente).FindByCodPaziente(codicePaziente)
                '    pazienteVac.Email = "prova1"
                '    pazienteVac.Paz_Codice = 0
                '    'Using biz As New Biz.BizFactory.Instance.(genericProvider, Settings, OnVacContext.CreateBizContextInfos(OnVacUtility.Variabili.CNS.Codice), Nothing)
                '    Using bizPaziente As Biz.BizPaziente = Biz.BizFactory.Instance.CreateBizPaziente(genericProvider, Settings,
                '                                                                                     OnVacContext.CreateBizContextInfos(),
                '                                                                                     New Biz.BizLogOptions(Log.DataLogStructure.TipiArgomento.PAZIENTI, True))
                '        'Dim modificaResult As Biz.BizResult = bizPaziente.ModificaPaziente(
                '        '        New Biz.BizPaziente.ModificaPazienteCommand() With {
                '        '            .Paziente = pazienteVac,
                '        '            .FromVAC = False
                '        '        })
                '        Dim inserisciResult As Biz.BizResult = bizPaziente.InserisciPaziente(
                '              New Biz.BizPaziente.InserisciPazienteCommand() With {
                '                  .Paziente = pazienteVac,
                '                  .FromVAC = False
                '              })
                '    End Using
                'End Using
                'GenericProvider, Settings, ContextInfos, New BizLogOptions(Log.DataLogStructure.TipiArgomento.PAZIENTI, True))

                '    Dim modificaResult As BizResult = pazienteBiz.ModificaPaziente(
                '        New BizPaziente.ModificaPazienteCommand() With {
                '            .Paziente = pazienteVac,
                '            .FromVAC = False,
                '            .ForzaInserimento = True
                '        })

                'Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                '    Using biz As New Biz.BizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(OnVacUtility.Variabili.CNS.Codice), Nothing)
                '        Dim paz As New Entities.Paziente
                '        biz.ModificaPaziente(New OnVac.Biz.BizPaziente.ModificaPazienteCommand() With {
                '                              .Paziente = New Entities.Paziente() With {
                '                                .Paz_Codice = 640718,
                '                                .PAZ_NOME = "MARCO",
                '                                .PAZ_COGNOME = "FABBRI"
                '                              }})
                '        .Email = "marco.fabbri"

                '        biz.InserisciPaziente(New OnVac.Biz.BizPaziente.InserisciPazienteCommand() With {
                '                            .Paziente = New Entities.Paziente() With {
                '                              .Paz_Codice = 0,
                '                              .PAZ_NOME = "MARCO",
                '                              .PAZ_COGNOME = "FABBRI"
                '                              }, .FromVAC = False})
                '    End Using
                'End Using
                '                .citizenship = "100",
                '                .cs = "12341234",
                '                .domAddrBnr = "2",
                '                .domAddrCode = "040008",
                '                .domAddrStr = "VIA DOMICILIO",
                '                .domAddrZip = "01234",
                '                .fiscalCode = "PRVRVA19R01D704X",
                '                .genderCode = "M",
                '                .istatUlssAss = "050506",
                '                .mmgRegionalCode = "998877",
                '                .mmgNameFam = "DOTTPROVA",
                '                .mmgNameGiv = "NOME",
                '                .mpi = "6543210",
                '                .nameFam = "PROVA AURV",
                '                .nameGiv = "NOME UNO",
                '                .telecomHp = "0123456789",
                '                .telecomMc = "000111222333",
                '                .telecomMail = "mail@m.com"
                '            })

                '    End Using
                'End Using

#End Region

#Region " [FSE] - Invio ITI-61 "

                '' [FSE] - Invio ITI-61
                'Dim bizContextInfos As Biz.BizContextInfos = OnVacContext.CreateBizContextInfos(OnVacUtility.Variabili.CNS.Codice)

                'Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                '    Using bizFSE As New Biz.BizFSE(genericProvider, Settings, bizContextInfos)

                '        Dim codicePaziente As Integer = 267328
                '        Dim codiceUsl As String = "050501"
                '        Dim codiceOperatore As String = "04846"

                '        Dim inputData As New Biz.FSErIndicizzazione.datiRegistryPaziente()
                '        Dim uniqueID As String = bizFSE.GetDocumentUniqueId(codicePaziente, codiceUsl)

                '        inputData.documentUniqueId = uniqueID
                '        inputData.repositoryUniqueId = bizFSE.GetRepositoryUniqueId(codiceUsl)

                '        inputData.paziente = bizFSE.GetPazienteCDA(codicePaziente)

                '        inputData.medicoRichiedente = New Biz.FSErIndicizzazione.medico()

                '        If Not String.IsNullOrWhiteSpace(codiceOperatore) Then

                '            Using bizOperatori As New Biz.BizOperatori(genericProvider, Settings, bizContextInfos)

                '                Dim operatore As Entities.Operatore = bizOperatori.GetOperatoreById(codiceOperatore)

                '                If operatore IsNot Nothing Then

                '                    Dim split As String() = operatore.Nome.Split("*")

                '                    inputData.medicoRichiedente = New Biz.FSErIndicizzazione.medico() With
                '                    {
                '                        .cognome = split(0),
                '                        .nome = If(split.Length > 1, split(1), String.Empty),
                '                        .codiceFiscale = operatore.CodiceFiscale
                '                    }

                '                End If

                '            End Using

                '        End If

                '        Using bizInst As New Biz.BizInstallazioni(genericProvider, Settings, bizContextInfos)

                '            Dim installazione As Entities.Installazione = bizInst.GetInstallazioneCorrenteBycodiceUsl(codiceUsl)

                '            If installazione IsNot Nothing Then

                '                inputData.strutturaRichiedente = New Biz.FSErIndicizzazione.struttura() With
                '                {
                '                    .codice = installazione.CodiceInstallazione,
                '                    .descrizione = installazione.UslDescrizione,
                '                    .tipoStruttura = Biz.FSErIndicizzazione.tipoStruttura.FLS11,
                '                    .tipoStrutturaSpecified = True
                '                }

                '                inputData.paziente.uslCorrente = New Biz.FSErIndicizzazione.struttura()
                '                inputData.paziente.uslCorrente.codice = installazione.CodiceInstallazione
                '                inputData.paziente.uslCorrente.descrizione = installazione.UslDescrizione
                '                inputData.paziente.uslCorrente.tipoStruttura = Biz.FSErIndicizzazione.tipoStruttura.FLS11
                '                inputData.paziente.uslCorrente.tipoStrutturaSpecified = True

                '            End If

                '        End Using

                '        ' Invio
                '        Dim dataInvio As Date = Date.Now
                '        Dim success As Boolean? = True
                '        Dim message As New Text.StringBuilder()

                '        Dim response As Biz.FSErIndicizzazione.RegistryResponseType = Nothing
                '        Dim client As Biz.FSErIndicizzazione.DocumentRegistryCustomPortTypeClient = Nothing

                '        Try
                '            client = New Biz.FSErIndicizzazione.DocumentRegistryCustomPortTypeClient()
                '            client.Open()

                '            response = client.sendRegistryMessage(inputData)

                '        Finally
                '            If client IsNot Nothing AndAlso client.State <> System.ServiceModel.CommunicationState.Closed Then client.Close()
                '        End Try

                '    End Using

                'End Using
#End Region

                ' End If
#End If

                ' Rieffettuazione dell'ultima ricerca eseguita
                If Not EseguiUltimaRicercaEffettuata() Then
                    OnitLayout31.ShowMsgBox(New Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Ricerca non effettuata: nessuna ricerca eseguita in precedenza", "no_ultima_ricerca", False, False))
                End If

        End Select

    End Sub

#End Region

#Region " Eventi User Control Alias "

    Private Sub OnVacAlias1_AliasNonEseguibile(sender As Object, e As AliasNonEseguibileArgs) Handles OnVacAlias1.AliasNonEseguibile

        fmOnVacAlias.VisibileMD = Not e.ChiudiControllo
        OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(e.Messaggio, "msgAliasNonEseguibile", False, False))

    End Sub

    Private Sub OnVacAlias1_ConfermaAlias(sender As Object, e As OnVacAliasArgs) Handles OnVacAlias1.ConfermaAlias

        fmOnVacAlias.VisibileMD = False

        If e.Conferma Then

            Dim checkResult As Biz.BizPaziente.CheckCodiciRegionaliMasterAliasResult

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizPaziente As Biz.BizPaziente = Biz.BizFactory.Instance.CreateBizPaziente(genericProvider, Settings,
                                                                                                 OnVacContext.CreateBizContextInfos(),
                                                                                                 OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.MERGEPAZIENTI))

                    checkResult = bizPaziente.CheckCodiciRegionaliMasterAlias(e.PazMasterId, e.pazAliasIDs)

                End Using
            End Using

            Select Case checkResult.Result

                Case Biz.BizPaziente.CheckCodiciRegionaliMasterAliasResult.ResultType.Failure

                    ' Se il controllo dei codici regionali restituisce lo stato Failure => NO MERGE
                    OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(checkResult.Message, "msgNoMergeCodReg", False, False))

                Case Biz.BizPaziente.CheckCodiciRegionaliMasterAliasResult.ResultType.Warning

                    ' Se il controllo dei codici regionali restituisce lo stato Warning => CONFERMA ALL'UTENTE
                    Dim key As String = String.Format("{0}*{1}*{2}", CONFIRM_MERGE, e.PazMasterId.ToString(), String.Join(";", e.pazAliasIDs))

                    OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("ATTENZIONE: " + checkResult.Message + " Continuare?", key, True, True))

                Case Biz.BizPaziente.CheckCodiciRegionaliMasterAliasResult.ResultType.Success

                    ' Se il controllo dei codici regionali restituisce lo stato Success => MERGE
                    MergePazienti(e.PazMasterId, e.pazAliasIDs)

                Case Else

                    Throw New NotImplementedException("CheckCodiciRegionaliMasterAlias")

            End Select

        End If

    End Sub

#End Region

#Region " Eventi Datagrid "

    Private Sub WzMsDataGrid1_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles WzMsDataGrid1.ItemDataBound

        If Settings.CONSENSO_GES Then

            Select Case e.Item.ItemType

                Case ListItemType.Item, ListItemType.AlternatingItem, ListItemType.SelectedItem

                    Dim imgStatoConsenso As System.Web.UI.WebControls.Image =
                        DirectCast(e.Item.FindControl("imgStatoConsenso"), System.Web.UI.WebControls.Image)

                    Dim pazCodice As String =
                        HttpUtility.HtmlDecode(e.Item.Cells(WzMsDataGrid1.getColumnNumberByKey("Codice")).Text).Trim()

                    If Settings.CONSENSO_LOCALE AndAlso String.IsNullOrEmpty(pazCodice) Then

                        ' Rilevazione consenso in locale e paziente non in locale => no immagine stato consenso
                        imgStatoConsenso.Visible = False

                    Else

                        Dim pazCodiceAusiliario As String =
                            HttpUtility.HtmlDecode(e.Item.Cells(WzMsDataGrid1.getColumnNumberByKey("Ausiliario")).Text).Trim()

                        Dim statoConsenso As Onit.OnAssistnet.GestioneConsenso.Wcf.Proxy.ConsensiService.IconeConsensiItem = iconeConsensi.Items.Where(Function(p) p.CodicePaziente = pazCodiceAusiliario).FirstOrDefault()
                        If statoConsenso Is Nothing Then
                            imgStatoConsenso.ImageUrl = "~/Images/consensoAltro.png"
                            imgStatoConsenso.ToolTip = "Stato consenso: Sconosciuto"
                        Else
                            Dim icona As Onit.OnAssistnet.GestioneConsenso.Wcf.Proxy.ConsensiService.Icona = iconeConsensi.Icone.Where(Function(p) p.ID = statoConsenso.IconaID).FirstOrDefault()
                            imgStatoConsenso.ImageUrl = icona.Url
                            imgStatoConsenso.ToolTip = String.Format("Stato consenso: {0}", icona.Descrizione)
                        End If

                    End If
                    Dim UlssCodice As String =
                        HttpUtility.HtmlDecode(e.Item.Cells(WzMsDataGrid1.getColumnNumberByKey("ULSS")).Text).Trim()
                    Dim UlssDCodice As String =
                        HttpUtility.HtmlDecode(e.Item.Cells(WzMsDataGrid1.getColumnNumberByKey("ULSSD")).Text).Trim()
                    If String.IsNullOrWhiteSpace(UlssCodice) AndAlso Not String.IsNullOrWhiteSpace(UlssDCodice) Then
                        e.Item.Cells(WzMsDataGrid1.getColumnNumberByKey("ULSS")).Text = UlssDCodice
                    End If
            End Select

        End If

    End Sub

#End Region

#Region " Eventi OnitLayout "

    Private Sub OnitLayout31_AlertClick(sender As Object, e As Controls.PagesLayout.OnitLayout3.AlertEventArgs) Handles OnitLayout31.AlertClick

        If e.Key = OPEN_RILEVAZIONE_CONSENSO Then
            ApriRilevazioneConsenso(True)
        End If

    End Sub

    Private Sub OnitLayout31_ConfirmClick(sender As Object, e As Controls.PagesLayout.OnitLayout3.ConfirmEventArgs) Handles OnitLayout31.ConfirmClick

        If e.Key.StartsWith(CONFIRM_MERGE) Then

            Dim params As String() = e.Key.Split("*")

            Dim idPazienteMaster As Int64 = Convert.ToInt64(params(1))
            Dim arrayIdPazientiAlias As Int64() = params(2).Split(";").Select(Function(p) Convert.ToInt64(p)).ToArray()

            If e.Result Then
                MergePazienti(idPazienteMaster, arrayIdPazientiAlias)
            End If

        End If

    End Sub

#End Region

#Region " Private Methods "

    Private Sub ShowDgrColumn(key As String, show As Boolean)

        DirectCast(WzMsDataGrid1.getColumnByKey(key), System.Web.UI.WebControls.DataGridColumn).Visible = show

    End Sub

    Private Sub ImpostaFocus()

        Dim ctlToFocus As System.Web.UI.Control = tabRicerca.FindControl(Settings.RICERCA_PAZ_FOCUS)

        If ctlToFocus Is Nothing Then
            Dim ctls() As System.Web.UI.Control = tabRicerca.getWzControlsByBinding(Constants.ConnessioneAnagrafe.CENTRALE, "t_paz_pazienti_centrale", Settings.RICERCA_PAZ_FOCUS)
            If ctls.Length > 0 Then
                ctlToFocus = ctls(0)
            End If
        End If

        If Not ctlToFocus Is Nothing Then
            Dim strJs As String = "<script language=javascript> document.getElementById('" + ctlToFocus.ClientID + "').focus(); </script>"
            ClientScript.RegisterStartupScript(Me.GetType(), "scr001", strJs, False)
        End If

    End Sub

    'avvia la popup che permette di selezionare il paziente master dagli altri che sono alias
    Private Sub GestioneAlias()

        ' {anty 19/1/05
        If odpRicercaPaziente.Connections(Constants.ConnessioneAnagrafe.LOCALE).ReadAuth = Onit.Controls.OnitDataPanel.Connection.readAuthorizations.none Then
            AlertClientMsg("Errore! Per la gestione degli alias la ricerca deve essere impostata solo in locale.(il campo 'tipoanag' della tabella 't_ana_param_ricerca_paz' nell'onit_manager)")
            Return
        End If
        '}
        Dim fc As Onit.Controls.OnitDataPanel.FilterCollection = odpRicercaPaziente.getSelectionFilters(False)
        If fc.Count = 0 Then
            AlertClientMsg("Selezionare almeno un paziente")
            Return
        End If

        Dim enc As Onit.Controls.OnitDataPanel.FieldsEncoder = odpRicercaPaziente.GetCurrentTableEncoder()
        Dim rows() As DataRow = odpRicercaPaziente.getCurrentDataTable().Select(fc.getSqlForDtb(Me.Page, Nothing, enc))

        Dim arrColonne() As String = {
            "PAZ_CANCELLATO",
            "PAZ_CODICE",
            "PAZ_CODICE_AUSILIARIO",
            "PAZ_COGNOME",
            "PAZ_NOME",
            "PAZ_SESSO",
            "PAZ_DATA_NASCITA",
            "PAZ_CODICE_FISCALE",
            "PAZ_TESSERA",
            "PAZ_INDIRIZZO_RESIDENZA"
        }

        Dim strCol, strDecodedCol As String

        'copio solo le righe spuntate nella griglia
        If rows.Length > 0 Then

            Dim dtAlias As DataTable = rows(0).Table.Clone()

            Dim destRow As DataRow
            For Each srcRow As DataRow In rows
                destRow = dtAlias.NewRow()
                destRow.ItemArray = srcRow.ItemArray
                dtAlias.Rows.Add(destRow)
            Next
            dtAlias.AcceptChanges()

            'rinomino le colonne del datatable
            For Each strCol In arrColonne

                strDecodedCol = enc.getCode(Constants.ConnessioneAnagrafe.LOCALE, "T_PAZ_PAZIENTI", strCol)

                If String.Compare(strDecodedCol, strCol, True) <> 0 Then

                    'occorre rinominare la colonna con il nome del campo al posto del nome assegnato dal pannello
                    'per evitare la collisione di nomi con le colonne che non interessano le rimuovo
                    If dtAlias.Columns.Contains(strCol) Then
                        dtAlias.Columns.Remove(strCol)
                    End If
                    If dtAlias.Columns.Contains(strDecodedCol) Then
                        dtAlias.Columns(strDecodedCol).ColumnName = strCol
                    End If

                End If

            Next

            'trattamento del comune di nascita e di residenza
            strDecodedCol = enc.getCode(Constants.ConnessioneAnagrafe.LOCALE, "T_ANA_COMUNI", "COM_DESCRIZIONE")
            dtAlias.Columns(strDecodedCol).ColumnName = "COMUNE_DI_NASCITA"

            strDecodedCol = enc.getCode(Constants.ConnessioneAnagrafe.LOCALE, "T_ANA_COMUNI_RES", "COM_DESCRIZIONE")
            dtAlias.Columns(strDecodedCol).ColumnName = "COMUNE_DI_RESIDENZA"

            'assegno i dati all'usercontrol
            If Not IsNothing(OnVacAlias1) Then
                fmOnVacAlias.VisibileMD = True
                OnVacAlias1.ImpostaDati(dtAlias)
            End If

        Else
            AlertClientMsg("Impossibile eseguire l'alias: devono essere selezionati almeno due pazienti!")
        End If

    End Sub

    Private Sub MergePazienti(idPazienteMaster As Int64, arrayIdPazientiAlias As Int64())

        Dim javascriptMessage As New Text.StringBuilder()

        Dim success As Boolean = True

        ' Esecuzione Alias
        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, OnVacUtility.GetReadCommittedTransactionOptions())

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                Dim pazienteMaster As Entities.Paziente

                Dim pazientiAliasList As New List(Of Entities.Paziente)()

                Using bizPaziente As Biz.BizPaziente =
                    Biz.BizFactory.Instance.CreateBizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.MERGEPAZIENTI))

                    pazienteMaster = bizPaziente.CercaPaziente(idPazienteMaster)

                    Dim idPazientiAlias As List(Of String) = New List(Of String)()

                    idPazientiAlias.AddRange(arrayIdPazientiAlias.Select(Function(p) p.ToString()).ToList())

                    Dim pazientiAlias As Collection.PazienteCollection = bizPaziente.CercaPazienti(String.Join(",", idPazientiAlias.ToArray()))

                    For Each pazienteAlias As Entities.Paziente In pazientiAlias

                        Dim unisciPazientiCommand As New Biz.BizPaziente.UnisciPazientiCommand()
                        unisciPazientiCommand.PazienteMaster = pazienteMaster
                        unisciPazientiCommand.PazienteAlias = pazienteAlias
                        unisciPazientiCommand.FromVAC = True

                        Dim bizResult As Biz.BizResult = bizPaziente.UnisciPazienti(unisciPazientiCommand)

                        If javascriptMessage.Length > 0 Then javascriptMessage.AppendLine()

                        If bizResult.Success Then
                            javascriptMessage.Append("Merge effettuato con successo")
                        Else
                            javascriptMessage.Append("Merge non effettuato")
                        End If

                        If pazientiAlias.Count > 1 Then
                            javascriptMessage.AppendLine(String.Format(": ALIAS [{0}]", pazienteAlias.Paz_Codice))
                        End If

                        If bizResult.Success Then
                            pazientiAliasList.Add(pazienteAlias)
                        Else
                            Dim message As String = bizResult.GetResultMessageCollection(False).ToString()
                            If Not String.IsNullOrEmpty(message) Then
                                javascriptMessage.AppendLine(": ")
                                javascriptMessage.AppendLine(message)
                            End If
                        End If

                        ' --- Log --- '
                        If bizResult.Messages.Count > 0 Then

                            Dim testataLog As New DataLogStructure.Testata(DataLogStructure.TipiArgomento.MERGEPAZIENTI, Operazione.Generico, pazienteMaster.Paz_Codice, False)
                            Dim recordLog As New DataLogStructure.Record()

                            recordLog.Campi.Add(New DataLogStructure.Campo(String.Empty, bizResult.Messages.ToString()))
                            testataLog.Records.Add(recordLog)

                            Biz.BizClass.WriteLog(New List(Of DataLogStructure.Testata)({testataLog}), True, OnVacContext.Connection)

                        End If
                        ' ----------- '

                    Next

                End Using

                If pazientiAliasList.Count > 0 Then
                    OnVacMidSendManager.UnisciPazienti(pazienteMaster, Nothing, pazientiAliasList.ToArray())
                End If

            End Using

            transactionScope.Complete()

        End Using
        '--

        'ripete la ricerca per aggiornare il datagrid
        odpRicercaPaziente.Find()

        'gestione dell'operazione di accorpamento (modifica 05/08/2004)
        If javascriptMessage.Length = 0 Then

            ' N.B. : qui andrebbe gestito il Consenso Utente, ma non fa il redirect automatico alla gestione del paziente perchè il javascriptMessage è sempre valorizzato!

            'deve fare un redirect alla gestione pazienti
            Dim filters As New Onit.Controls.OnitDataPanel.FilterCollection()
            Dim filter As New Onit.Controls.OnitDataPanel.Filter(idPazienteMaster, New Onit.Controls.OnitDataPanel.BindingFieldValue(Constants.ConnessioneAnagrafe.CENTRALE, "t_paz_pazienti", "paz_codice", idPazienteMaster), Onit.Controls.OnitDataPanel.Filter.FilterComparators.Uguale, Onit.Controls.OnitDataPanel.Filter.FilterOperators.And, Onit.Controls.OnitDataPanel.Filter.FieldTypes.Number, Nothing)

            filters.Add(filter)

            odpRicercaPaziente.applySelectionFilters(filters)
            odpRicercaPaziente.redirectToDetailPage(CurrentOperationTypes.EditRecord, odpRicercaPaziente.getSelectionFilters(True))

        Else

            'messaggio
            AlertClientMsg(javascriptMessage.ToString())

        End If

    End Sub

    ' Se il paziente ha il codice ausiliario apre la popup e carica l'applicativo per la rilevazione del consenso
    Private Sub ApriRilevazioneConsenso(autoEdit As Boolean)

        Dim codiceAusiliarioPaziente As String = String.Empty

        Try
            Dim s As String =
                odpRicercaPaziente.GetCurrentTableEncoder.getValOf(odpRicercaPaziente.getCurrentDataRow(), "paz_codice_ausiliario", "t_paz_pazienti", True).ToString()

            If Not String.IsNullOrEmpty(s) Then
                codiceAusiliarioPaziente = s
            Else
                s = odpRicercaPaziente.GetCurrentTableEncoder.getValOf(odpRicercaPaziente.getCurrentDataRow(), "paz_codice", "t_paz_pazienti_centrale", True).ToString()
                If Not String.IsNullOrEmpty(s) Then
                    codiceAusiliarioPaziente = s
                End If
            End If

        Catch ex As Exception
            ' nothing
        End Try

        If String.IsNullOrWhiteSpace(codiceAusiliarioPaziente) Then

            AlertClientMsg(Settings.CONSENSO_MSG_NO_COD_CENTRALE)

        Else

            modConsenso.VisibileMD = True
            frameConsenso.Attributes.Add("src", GetUrlMascheraRilevazioneConsenso(codiceAusiliarioPaziente, autoEdit))

        End If

    End Sub

    ' Nel caso in cui venga gestito il consenso, il redirect al dettaglio del paziente 
    ' non deve avvenire se il livello di consenso ha il flag controllo impostato a "bloccante".
    Private Sub DettaglioPazienteClick()

        Dim pazCodice As String = String.Empty
        Dim pazCodiceAusiliario As String = String.Empty

        Dim traceMessage As New System.Text.StringBuilder("Richiesta di dettaglio paziente")

        Try
            Dim drRow As DataRow = odpRicercaPaziente.getCurrentDataRow()

            If Not drRow Is Nothing Then
                '--
                Dim s As Object = odpRicercaPaziente.GetCurrentTableEncoder.getValOf(drRow, "paz_codice", "t_paz_pazienti_centrale", True)
                '--
                If Not s Is Nothing AndAlso Not IsDBNull(s) AndAlso s.ToString() <> String.Empty Then
                    '--
                    ' Codice paziente centrale
                    '--
                    pazCodiceAusiliario = s.ToString()
                    traceMessage.AppendLine()
                    traceMessage.AppendFormat("Codice ausiliario paziente da centrale: {0}", pazCodiceAusiliario)
                    '--
                Else
                    s = odpRicercaPaziente.GetCurrentTableEncoder.getValOf(drRow, "paz_codice_ausiliario", "t_paz_pazienti", True)
                    If Not s Is Nothing AndAlso Not IsDBNull(s) AndAlso s.ToString() <> String.Empty Then
                        '--
                        ' Codice ausiliario locale
                        '--
                        pazCodiceAusiliario = s.ToString()
                        traceMessage.AppendLine()
                        traceMessage.AppendFormat("Codice ausiliario paziente da locale: {0}", pazCodiceAusiliario)
                        '--
                    End If
                End If
                '--
                s = odpRicercaPaziente.GetCurrentTableEncoder.getValOf(drRow, "paz_codice", "t_paz_pazienti", True)
                If Not s Is Nothing AndAlso Not IsDBNull(s) AndAlso s.ToString() <> String.Empty Then
                    '--
                    ' Codice paziente locale
                    '--
                    pazCodice = s.ToString()
                    traceMessage.AppendLine()
                    traceMessage.AppendFormat("Codice paziente: {0}", pazCodice)
                    '--
                End If
                '--
                UltimoPazienteSelezionato = New Entities.UltimoPazienteSelezionato(pazCodiceAusiliario, pazCodice)
                '--
            End If

        Catch ex As Exception
            Common.Utility.EventLogHelper.EventLogWrite(ex, OnVacContext.AppId)
            traceMessage.AppendLine()
            traceMessage.AppendFormat("Eccezione in valorizzazione codice paziente e codice ausiliario paziente {0}", ex.Message)
        End Try
        '--
        ' GESTIONE CONSENSO PAZIENTE
        '--
        Dim hasErrors As Boolean = False
        '--
        Try
            If Settings.CONSENSO_GES Then
                '--
                traceMessage.AppendLine("Gestione del consenso ATTIVA.")
                '--
                If IsProntoSoccorso AndAlso String.IsNullOrEmpty(pazCodice) Then
                    '--
                    ' Ricerca PS e paziente non presente in locale
                    '--
                    traceMessage.AppendLine("Ricerca Paziente PS e paziente NON PRESENTE in locale: redirect alle eseguite NON EFFETTUATO.")
                    '--
                ElseIf Settings.CONSENSO_LOCALE AndAlso String.IsNullOrEmpty(pazCodice) Then
                    '--
                    ' Consenso gestito in LOCALE e paziente non presente in locale
                    '--
                    traceMessage.AppendLine("Consenso gestito in LOCALE e paziente NON PRESENTE in locale: ricerca del consenso NON EFFETTUATA.")
                    '--
                Else
                    '--
                    ' Consenso gestito in LOCALE e paziente presente in locale, oppure consenso gestito in CENTRALE.
                    '--
                    Dim consensoGlobalePaz As Entities.Consenso.StatoConsensoPaziente = Nothing
                    '--
                    If String.IsNullOrEmpty(pazCodiceAusiliario) Then
                        traceMessage.AppendLine("Codice ausiliario del paziente non valorizzato.")
                    Else
                        consensoGlobalePaz = OnVacUtility.GetConsensoGlobalePaziente(pazCodiceAusiliario, Settings)
                        If consensoGlobalePaz Is Nothing Then
                            traceMessage.AppendLine("Ricerca del consenso per codice ausiliario FALLITA.")
                        End If
                    End If
                    '--
                    If consensoGlobalePaz Is Nothing Then
                        '--
                        traceMessage.AppendLine("Ricerca del consenso per valore di default FALLITA.")
                        AlertClientMsg("Impossibile aprire il dettaglio dei dati del paziente. Nessun consenso trovato.")
                        hasErrors = True
                        '--
                    ElseIf consensoGlobalePaz.Controllo = Enumerators.ControlloConsenso.Bloccante Then
                        '-- 
                        ' Valore del consenso BLOCCANTE => blocco l'accesso al dettaglio e, se il param auto_edit vale true, apro la modale di rilevazione del consenso.
                        '--
                        ' N.B.: la modale di rilevazione viene aperta sempre (se il parametro lo prevede), indipendentemente dalla gestione del consenso,
                        '        perchè siamo nel caso in cui il consenso è locale e il paziente in locale c'è, oppure il consenso è centrale.
                        '--
                        traceMessage.AppendLine("Consenso BLOCCANTE.")
                        '--
                        Dim consensoBloccanteMessage As String =
                            consensoGlobalePaz.DescrizioneStatoConsenso + Environment.NewLine + "Impossibile aprire il dettaglio dei dati del paziente."
                        '--
                        If Settings.CONSENSO_BLOCCANTE_AUTO_EDIT Then
                            '--
                            AperturaAutomaticaRilevazioneConsensoBloccante = True
                            '--
                            ' Messaggio all'utente e apertura pop-up rilevazione del consenso in edit (nell'evento AlertClick dell'OnitLayout)
                            consensoBloccanteMessage += Environment.NewLine + "Verrà aperta la maschera di rilevazione del consenso."
                            OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(
                                                       HttpUtility.JavaScriptStringEncode(consensoBloccanteMessage), OPEN_RILEVAZIONE_CONSENSO, False, True))
                            '--
                        Else
                            '--
                            ' Messaggio all'utente
                            AlertClientMsg(consensoBloccanteMessage)
                            '--
                        End If
                        '--
                        hasErrors = True
                        '--
                    ElseIf (IsProntoSoccorso Or IsGestioneCentrale) AndAlso consensoGlobalePaz.BloccoAccessiEsterni Then
                        '--
                        ' Accesso da maschera in configurazione PS o Centrale: 
                        ' se il livello è marcato come bloccante per accessi esterni, non permetto all'utente di fare il redirect al dettaglio.
                        '--
                        AlertClientMsg(consensoGlobalePaz.DescrizioneStatoConsenso + Environment.NewLine + "Impossibile accedere ai dati del paziente.")
                        hasErrors = True
                        '--
                    End If
                    '--
                End If
                '--
            End If
            '--
        Finally
            System.Diagnostics.Trace.TraceInformation(traceMessage.ToString())
        End Try
        '--
        If Not hasErrors Then
            RedirectToDetail(pazCodice, pazCodiceAusiliario)
        End If
        '--
    End Sub

    Private Sub RedirectToDetail(codicePaziente As String, codiceAusiliarioPaziente As String)

        If Not String.IsNullOrEmpty(codicePaziente) OrElse Not String.IsNullOrEmpty(codiceAusiliarioPaziente) Then

            Dim bloccoLocale As Boolean = False

            ' Casi in cui viene bloccato il redirect:
            ' 1- codice paziente locale nullo e maschera in configurazione Pronto Soccorso, oppure
            ' 2- codice paziente nullo e consenso gestito in locale
            If String.IsNullOrEmpty(codicePaziente) AndAlso (IsProntoSoccorso OrElse Settings.CONSENSO_LOCALE) Then
                bloccoLocale = True
            End If

            If IsProntoSoccorso Then
                '--
                ' PRONTO SOCCORSO
                '--
                If bloccoLocale Then

                    AlertClientMsg("Impossibile visualizzare il dettaglio del paziente. Paziente non presente in anagrafe locale.")

                Else

                    If GetRichiestaConsensoTrattamentoDatiUtente(OnVacUtility.Variabili.CNS.Codice) Then

                        If Not String.IsNullOrWhiteSpace(codicePaziente) Then
                            ucConsensoUtente.CodicePaziente = Convert.ToInt64(codicePaziente)
                        End If

                        ucConsensoUtente.CodiceAusiliarioPaziente = codiceAusiliarioPaziente
                        ucConsensoUtente.CodiceConsultorio = OnVacUtility.Variabili.CNS.Codice
                        ucConsensoUtente.Destinazione = ConsensoTrattamentoDatiUtente.DestinazioneRedirect.VacEseguitePS

                        fmConsensoUtente.VisibileMD = True

                    Else

                        RedirectToVacEseguitePS(codicePaziente)

                    End If

                End If

            ElseIf IsGestioneCentrale Then
                '--
                ' ANAGRAFE CENTRALE
                '--

                ' TODO [Consenso]: da decommentare se si vuole bloccare l'accesso ai dati da centrale, in caso di consenso bloccante
                'If bloccoLocale Then
                '    ' Caso consenso locale e paz non in locale => se la maschera è in configurazione centrale, devo bloccare l'accesso ai dati del paziente
                '    AlertClientMsg("Impossibile visualizzare il dettaglio del paziente. Paziente non presente in anagrafe locale.")
                'Else

                If GetRichiestaConsensoTrattamentoDatiUtente(OnVacUtility.Variabili.CNS.Codice) Then

                    If Not String.IsNullOrWhiteSpace(codicePaziente) Then
                        ucConsensoUtente.CodicePaziente = Convert.ToInt64(codicePaziente)
                    End If

                    ucConsensoUtente.CodiceAusiliarioPaziente = codiceAusiliarioPaziente
                    ucConsensoUtente.CodiceConsultorio = OnVacUtility.Variabili.CNS.Codice
                    ucConsensoUtente.Destinazione = ConsensoTrattamentoDatiUtente.DestinazioneRedirect.VacEseguiteCentrale

                    fmConsensoUtente.VisibileMD = True

                Else

                    RedirectToVacEseguiteCentrale(codiceAusiliarioPaziente)

                End If

                'End If

            Else
                '--
                ' ANAGRAFE LOCALE
                '--

                If GetRichiestaConsensoTrattamentoDatiUtente(OnVacUtility.Variabili.CNS.Codice) Then

                    If Not String.IsNullOrWhiteSpace(codicePaziente) Then
                        ucConsensoUtente.CodicePaziente = Convert.ToInt64(codicePaziente)
                    End If

                    ucConsensoUtente.CodiceAusiliarioPaziente = codiceAusiliarioPaziente
                    ucConsensoUtente.CodiceConsultorio = OnVacUtility.Variabili.CNS.Codice
                    ucConsensoUtente.Destinazione = ConsensoTrattamentoDatiUtente.DestinazioneRedirect.DettaglioPaziente

                    fmConsensoUtente.VisibileMD = True

                Else

                    odpRicercaPaziente.redirectToDetailPage(CurrentOperationTypes.EditRecord, odpRicercaPaziente.getSelectionFilters(True))

                End If

            End If

        End If

    End Sub

    Private Sub RedirectToVacEseguitePS(codicePaziente As Long)

        OnVacUtility.Variabili.PazId = codicePaziente

        Response.Redirect(ResolveClientUrl("~/Pronto Soccorso/VacEseguitePS.aspx?LoadLeftFramePS=false"))

    End Sub

    Private Sub RedirectToVacEseguiteCentrale(codiceAusiliarioPaziente As String)

        OnVacUtility.Variabili.PazId = codiceAusiliarioPaziente

        Response.Redirect(ResolveClientUrl("~/HPazienti/VacEseguite/VacEseguite.aspx?isCentrale=true"))

    End Sub

#Region " Ricerche rapide "

    Private Function EseguiRicercaUltimoPaziente() As Boolean

        If UltimoPazienteSelezionato Is Nothing Then Return False

        If String.IsNullOrEmpty(UltimoPazienteSelezionato.CodicePazienteCentrale) AndAlso
           String.IsNullOrEmpty(UltimoPazienteSelezionato.CodicePazienteLocale) Then
            Return False
        End If

        Dim codicePaziente As String = String.Empty
        Dim inCentrale As Boolean = False

        If tipoAnag = Enumerators.TipoAnags.SoloLocale Then
            codicePaziente = UltimoPazienteSelezionato.CodicePazienteLocale
            inCentrale = False
        Else
            ' Se il codice centrale non è stato valorizzato (per selezione da Calendario, Gestione Appuntamenti o Movimenti), lo cerco in base al codice locale.
            If String.IsNullOrEmpty(UltimoPazienteSelezionato.CodicePazienteCentrale) Then

                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                    Using bizPaziente As New Biz.BizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                        UltimoPazienteSelezionato.CodicePazienteCentrale = bizPaziente.GetCodiceAusiliario(Convert.ToInt32(UltimoPazienteSelezionato.CodicePazienteLocale))

                    End Using
                End Using

            End If

            codicePaziente = UltimoPazienteSelezionato.CodicePazienteCentrale
            inCentrale = True
        End If

        If String.IsNullOrEmpty(codicePaziente) Then Return False

        ' Impostazione del filtro sul codice del paziente
        odpRicercaPaziente.Filters.Clear()
        ClearFilterFields()

        txtPazCodice.Text = codicePaziente
        If tipoAnag <> Enumerators.TipoAnags.SoloLocale Then
            txtPazCodice.BindingField.Connection = "centrale"
            txtPazCodice.BindingField.SourceTable = "t_paz_pazienti_centrale"
        End If

        ' Ricerca
        odpRicercaPaziente.Find()

        ' Cancellazione filtro codice paziente
        txtPazCodice.Text = String.Empty
        txtPazCodice.BindingField.Connection = "locale"
        txtPazCodice.BindingField.SourceTable = "t_paz_pazienti"
        odpRicercaPaziente.Filters.Clear()

        Return True

    End Function

    Private Function EseguiUltimaRicercaEffettuata() As Boolean

        If UltimaRicercaPaziente Is Nothing Then Return False
        If UltimaRicercaPaziente.IsEmpty() Then Return False

        ' Impostazione dei filtri di ricerca utilizzati
        odpRicercaPaziente.Filters.Clear()
        ImpostaFiltriUltimaRicerca()

        ' Ricerca
        odpRicercaPaziente.Find()

        Return True

    End Function

    Private Sub ClearFilterFields()
        txtPazCodice.Text = String.Empty
        txtCognome.Text = String.Empty
        txtNome.Text = String.Empty
        ddlSesso.ClearSelection()
        odpDataNascita.Text = String.Empty
        txtAnnoNascita.Text = String.Empty
        WzFinestraModale1.Codice = String.Empty
        WzFinestraModale1.Descrizione = String.Empty
        WzFinestraModale1.ValAltriCampi = String.Empty
        WzFinestraModale1.RefreshDataBind()
        txtCodFiscale.Text = String.Empty
        txtTesseraSan.Text = String.Empty
        fmComuneResidenza.Codice = String.Empty
        fmComuneResidenza.Descrizione = String.Empty
        fmComuneResidenza.ValAltriCampi = String.Empty
        fmComuneResidenza.RefreshDataBind()
        omlConsultorio.Codice = String.Empty
        omlConsultorio.Descrizione = String.Empty
        omlConsultorio.ValAltriCampi = String.Empty
        omlConsultorio.RefreshDataBind()
    End Sub

    Private Sub ImpostaFiltriUltimaRicerca()

        txtPazCodice.Text = UltimaRicercaPaziente.CodicePaziente
        txtCognome.Text = UltimaRicercaPaziente.Cognome
        txtNome.Text = UltimaRicercaPaziente.Nome

        ddlSesso.ClearSelection()
        Dim item As ListItem = ddlSesso.Items.FindByValue(UltimaRicercaPaziente.Sesso)
        If Not item Is Nothing Then item.Selected = True

        If UltimaRicercaPaziente.DataNascita.HasValue Then
            odpDataNascita.Data = UltimaRicercaPaziente.DataNascita.Value
        Else
            odpDataNascita.Text = String.Empty
        End If

        txtAnnoNascita.Text = UltimaRicercaPaziente.AnnoNascita
        txtCodFiscale.Text = UltimaRicercaPaziente.CodiceFiscale
        txtTesseraSan.Text = UltimaRicercaPaziente.TesseraSanitaria

        omlConsultorio.Codice = UltimaRicercaPaziente.CodiceConsultorio
        omlConsultorio.Descrizione = UltimaRicercaPaziente.DescrizioneConsultorio
        omlConsultorio.RefreshDataBind()

        ' Comune di nascita
        WzFinestraModale1.Codice = UltimaRicercaPaziente.CodiceComuneNascita
        WzFinestraModale1.Descrizione = UltimaRicercaPaziente.DescrizioneComuneNascita
        WzFinestraModale1.RefreshDataBind()

        ' Comune di residenza
        fmComuneResidenza.Codice = UltimaRicercaPaziente.CodiceComuneResidenza
        fmComuneResidenza.Descrizione = UltimaRicercaPaziente.DescrizioneComuneResidenza
        fmComuneResidenza.RefreshDataBind()

    End Sub

    Private Sub SetDatiUltimaRicercaEffettuata()

        UltimaRicercaPaziente = New Entities.UltimaRicercaPazientiEffettuata()

        UltimaRicercaPaziente.CodicePaziente = txtPazCodice.Text
        UltimaRicercaPaziente.Cognome = txtCognome.Text
        UltimaRicercaPaziente.Nome = txtNome.Text

        If Not ddlSesso.SelectedItem Is Nothing Then
            UltimaRicercaPaziente.Sesso = ddlSesso.SelectedItem.Value
        End If

        If Not String.IsNullOrEmpty(odpDataNascita.Text) Then
            UltimaRicercaPaziente.DataNascita = odpDataNascita.Data
        End If

        UltimaRicercaPaziente.AnnoNascita = txtAnnoNascita.Text
        UltimaRicercaPaziente.CodiceFiscale = txtCodFiscale.Text
        UltimaRicercaPaziente.TesseraSanitaria = txtTesseraSan.Text

        If Not String.IsNullOrEmpty(omlConsultorio.Codice) AndAlso Not String.IsNullOrEmpty(omlConsultorio.Descrizione) Then
            UltimaRicercaPaziente.CodiceConsultorio = omlConsultorio.Codice
            UltimaRicercaPaziente.DescrizioneConsultorio = omlConsultorio.Descrizione
        End If

        If Not String.IsNullOrEmpty(WzFinestraModale1.Codice) AndAlso Not String.IsNullOrEmpty(WzFinestraModale1.Descrizione) Then
            UltimaRicercaPaziente.CodiceComuneNascita = WzFinestraModale1.Codice
            UltimaRicercaPaziente.DescrizioneComuneNascita = WzFinestraModale1.Descrizione
        End If

        If Not String.IsNullOrEmpty(fmComuneResidenza.Codice) AndAlso Not String.IsNullOrEmpty(fmComuneResidenza.Descrizione) Then
            UltimaRicercaPaziente.CodiceComuneResidenza = fmComuneResidenza.Codice
            UltimaRicercaPaziente.DescrizioneComuneResidenza = fmComuneResidenza.Descrizione
        End If

    End Sub

#End Region

#Region " Consenso Utente "

    Private Sub ucConsensoUtente_ConsensoTrattamentoDatiUtenteAccettato() Handles ucConsensoUtente.ConsensoTrattamentoDatiUtenteAccettato

        ' TODO [Consenso Utente]: dove impostare UltimoPazienteSelezionato?
        '' Memorizzazione codice paziente per ricerca rapida (l'impostazione del paziente corrente quando si effettua il redirect vero e proprio)
        'UltimoPazienteSelezionato = New Entities.UltimoPazienteSelezionato(String.Empty, ucConsensoUtente.CodicePaziente)

        '' Memorizzazione ambulatorio per impostarlo prima del redirect
        'CodiceAmbulatorioPazienteSelezionato = ucConsensoUtente.CodiceAmbulatorio

        Dim codicePaziente As Long = ucConsensoUtente.CodicePaziente
        Dim codiceAusiliarioPaziente As String = ucConsensoUtente.CodiceAusiliarioPaziente
        Dim dest As ConsensoTrattamentoDatiUtente.DestinazioneRedirect = ucConsensoUtente.Destinazione

        ucConsensoUtente.ClearParameters()
        fmConsensoUtente.VisibileMD = False

        Select Case dest

            Case ConsensoTrattamentoDatiUtente.DestinazioneRedirect.NessunaSelezione
                ' nessun redirect

            Case ConsensoTrattamentoDatiUtente.DestinazioneRedirect.DettaglioPaziente
                odpRicercaPaziente.redirectToDetailPage(CurrentOperationTypes.EditRecord, odpRicercaPaziente.getSelectionFilters(True))

            Case ConsensoTrattamentoDatiUtente.DestinazioneRedirect.VacEseguitePS
                RedirectToVacEseguitePS(codicePaziente)

            Case ConsensoTrattamentoDatiUtente.DestinazioneRedirect.VacEseguiteCentrale
                RedirectToVacEseguiteCentrale(codiceAusiliarioPaziente)

                'Case ConsensoTrattamentoDatiUtente.DestinazioneRedirect.ConvocazioniPaziente
                ' Non previsto da ricerca paziente

        End Select

    End Sub

    Private Sub ucConsensoUtente_ConsensoTrattamentoDatiUtenteRifiutato() Handles ucConsensoUtente.ConsensoTrattamentoDatiUtenteRifiutato

        ucConsensoUtente.ClearParameters()
        fmConsensoUtente.VisibileMD = False

    End Sub

#End Region

#End Region

#Region " Protected Methods "

    Protected Function HideLeftFrameIfNeeded() As String

        If Not Page.IsPostBack Then
            Return GetOpenLeftFrameScript(False)
        End If

        Return String.Empty

    End Function

#End Region

End Class
