Imports Onit.Controls.OnitDataPanel.OnitDataPanel
Imports Onit.Database.DataAccessManager


Partial Class RicPazPS
    Inherits Common.PageBase

    Private _tipoAnags As Onit.OnAssistnet.OnVac.Enumerators.TipoAnags
    Protected WithEvents fmOnVacAlias As Onit.Controls.OnitFinestraModale

#Region " RicercaPazientePage "

    Private _makeSearch As Boolean = False

    Protected Overrides Sub OnPreRender(e As System.EventArgs)
        If _makeSearch Then
            Me.Ricerca()
        End If
        MyBase.OnPreRender(e)
    End Sub

#Region " Public properties "

    ' Proprietà utilizzate dal file di configurazione dell'onitdatapanel.

    Public ReadOnly Property ottieniDam() As IDAM
        Get
            If Not IsInDesignTime() Then
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

#Region " Private and protected properties "

    Private Property searchPerformed() As Boolean
        Get
            Return ViewState(Me.ID + "searchPerformed")
        End Get
        Set(ByVal Value As Boolean)
            ViewState(Me.ID + "searchPerformed") = Value
        End Set
    End Property

    Private Property blnSceltoLocale() As Boolean
        Get
            Return ViewState("blnSceltoLocale")
        End Get
        Set(ByVal Value As Boolean)
            ViewState("blnSceltoLocale") = Value
        End Set
    End Property

    Protected ReadOnly Property LoadLeftFramePS() As Boolean
        Get
            Dim loadLeftFrame As String = Me.Request.QueryString("LoadLeftFramePS")
            If loadLeftFrame = Nothing Then Return False
            Return Boolean.Parse(loadLeftFrame)
        End Get
    End Property

#End Region

#Region " Inizializzazione "

    Protected Overrides Sub OnLoad(e As System.EventArgs)

        MyBase.OnLoad(e)

        'imposta lo switch centrale locale attraverso il radio button
        If Not IsPostBack Then
            blnSceltoLocale = False
        End If

        Dim rblCentraleLocale As System.Web.UI.WebControls.RadioButtonList = Me.tabRicerca.FindControl("rblCentraleLocale")
        Dim lblCentraleLocale As System.Web.UI.WebControls.Label = Me.tabRicerca.FindControl("lblCentraleLocale")

        If Not rblCentraleLocale Is Nothing AndAlso Not lblCentraleLocale Is Nothing Then

            If _tipoAnags = Onit.OnAssistnet.OnVac.Enumerators.TipoAnags.CentraleLettLocaleAgg Then
                AddHandler rblCentraleLocale.SelectedIndexChanged, AddressOf rblCentraleLocale_SelectedIndexChanged
                rblCentraleLocale.Visible = True
                lblCentraleLocale.Visible = True
            Else
                rblCentraleLocale.Visible = False
                lblCentraleLocale.Visible = False
            End If

        End If

        'impostazione di alcune opzioni di ricerca
        ImpostaOpzioniRicerca()

        If Not IsPostBack() Then

            'impostazione del focus
            ImpostaFocus()
            searchPerformed = False
            If Not rblCentraleLocale Is Nothing Then
                rblCentraleLocale.Enabled = False
            End If

            If Me.UtenteLoggatoIsMedico Then

                Dim trMedico As System.Web.UI.HtmlControls.HtmlTableRow = tabRicerca.FindControl("trMedico")
                trMedico.Visible = True

                Dim dam As IDAM = OnVacUtility.OpenDam()

                Try
                    Dim fmMedico As Onit.Controls.OnitDataPanel.wzFinestraModale = tabRicerca.FindControl("fmMedico")
                    fmMedico.Filtro = String.Format(fmMedico.Filtro, dam.QB.FC.ToDate(DateTime.Now.Date), Me.CodiceMedicoUtenteLoggato)

                Finally
                    OnVacUtility.CloseDam(dam)
                End Try

                Me.SetFmMedicoToUtenteLoggato()

            End If

        End If

        Select Case Request.Form("__EVENTTARGET")
            Case "cerca"
                _makeSearch = True
        End Select

    End Sub

#End Region

#Region " Gestione del datapanel "

    Private Sub ImpostaOpzioniRicerca()

        If _tipoAnags = Onit.OnAssistnet.OnVac.Enumerators.TipoAnags.SoloLocale Then

        End If

    End Sub

    Private Sub ImpostaFocus()

        Dim ctlToFocus, ctls() As System.Web.UI.Control
        Dim strJs As String

        ctlToFocus = CType(tabRicerca, System.Web.UI.Control).FindControl(Settings.RICERCA_PAZ_FOCUS)

        If IsNothing(ctlToFocus) Then
            ctls = CType(tabRicerca, Onit.Controls.OnitDataPanel.wzFilter).getWzControlsByBinding("centrale", "t_paz_pazienti_centrale", Me.Settings.RICERCA_PAZ_FOCUS)
            If ctls.Length > 0 Then
                ctlToFocus = ctls(0)
            End If
        End If

        If Not IsNothing(ctlToFocus) Then
            strJs = "<script language=javascript> document.getElementById('" + ctlToFocus.ClientID + "').focus(); </script>"
            ClientScript.RegisterStartupScript(Me.GetType(), "scr001", strJs, False)
        End If

    End Sub

    Private Sub odpRicercaPaziente_beforeLoadData(sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles odpRicercaPaziente.beforeLoadData

        'imposto il pannello a seconda dei parametri dell'applicativo
        Select Case _tipoAnags

            Case Enumerators.TipoAnags.SoloLocale 'ok

                ImpostaCentrale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.none, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)
                ImpostaLocale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.read, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)
                'impostazione max numero di pazienti in locale
                odpRicercaPaziente.maxRecord = Settings.RICERCA_PAZ_MAX_RECORDS

            Case Enumerators.TipoAnags.CentraleLettura

                ImpostaCentrale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.read, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)
                ImpostaLocale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.read, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)

            Case Enumerators.TipoAnags.CentraleLettScritt

                ImpostaCentrale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.read, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)
                ImpostaLocale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.read, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)

            Case Enumerators.TipoAnags.CentraleScrittInsLettAgg

                ImpostaCentrale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.read, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)
                ImpostaLocale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.read, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)

            Case Enumerators.TipoAnags.CentraleLettLocaleAgg

                If blnSceltoLocale Then
                    ImpostaCentrale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.none, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)
                    ImpostaLocale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.read, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)

                    'impostazione max numero di pazienti in locale
                    odpRicercaPaziente.maxRecord = Settings.RICERCA_PAZ_MAX_RECORDS
                Else
                    'centrale
                    ImpostaCentrale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.read, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)
                    ImpostaLocale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.none, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)

                End If

        End Select

    End Sub

    Private Sub ImpostaCentrale(readAuth As Onit.Controls.OnitDataPanel.Connection.readAuthorizations, writeAuth As Onit.Controls.OnitDataPanel.Connection.writeAuthorizations)

        Me.odpRicercaPaziente.Connections("centrale").ReadAuth = readAuth
        Me.odpRicercaPaziente.Connections("centrale").WriteAuth = writeAuth

        'imposto il parametro per il recupero del messaggio inerente al superamento del limite massimo di elementi in fase di ricerca 
        If Me.odpRicercaPaziente.Connections("centrale").Parameters.Item("strErrMsg") Is Nothing Then

            Dim p As New Onit.Controls.OnitDataPanel.PanelParameter("strErrMsg")

            p.valueProvenience = Onit.Controls.OnitDataPanel.PanelParameter.Proveniences.valueProperty
            p.Enabled = True
            p.Type = Onit.Controls.OnitDataPanel.PanelParameter.Types.String
            p.value = ""
            p.Direction = Onit.Controls.OnitDataPanel.PanelParameter.Directions.InOut

            Me.odpRicercaPaziente.Connections("centrale").Parameters.Add(p)

        End If

        Me.odpRicercaPaziente.Connections("locale_cnas").ReadAuth = readAuth
        Me.odpRicercaPaziente.Connections("locale_cnas").WriteAuth = Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none

        Me.odpRicercaPaziente.Connections("locale_comre").ReadAuth = readAuth
        Me.odpRicercaPaziente.Connections("locale_comre").WriteAuth = Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none

    End Sub

    Private Sub ImpostaLocale(readAuth As Onit.Controls.OnitDataPanel.Connection.readAuthorizations, writeAuth As Onit.Controls.OnitDataPanel.Connection.writeAuthorizations)

        Me.odpRicercaPaziente.Connections("locale").ReadAuth = readAuth
        Me.odpRicercaPaziente.Connections("locale").WriteAuth = writeAuth

    End Sub

    Private Sub odpRicercaPaziente_afterBarOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpRicercaPaziente.afterBarOperation

        Dim strMsg As String
        If operation = Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.Find Then

            'fase 2  della ricerca:
            '   testo se non ho trovato niente in centrale, nel qual caso conduco una ricerca solo in locale
            If _tipoAnags = Onit.OnAssistnet.OnVac.Enumerators.TipoAnags.CentraleLettura Or
               _tipoAnags = Onit.OnAssistnet.OnVac.Enumerators.TipoAnags.CentraleScrittInsLettAgg Then

                If odpRicercaPaziente.getRecordCount <= 0 Then
                    _tipoAnags = Onit.OnAssistnet.OnVac.Enumerators.TipoAnags.SoloLocale
                    odpRicercaPaziente.Find()
                    _tipoAnags = Me.Settings.TIPOANAG
                    strMsg = "Non è stato trovato alcun paziente in centrale. La ricerca è stata condotta in locale"
                    AlertClientMsg(strMsg)
                End If

            End If

        End If

    End Sub

    Private Sub odpRicercaPaziente_controlLayoutChanging(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpRicercaPaziente.controlLayoutChanging

        Dim btnNew As Infragistics.WebUI.UltraWebToolbar.TBarButton = Me.ToolBar.Items.FromKeyButton("btnNew")
        Dim btnConfirm As Infragistics.WebUI.UltraWebToolbar.TBarButton = Me.ToolBar.Items.FromKeyButton("btnConfirm")
        Dim rblCentraleLocale As System.Web.UI.WebControls.RadioButtonList = Me.tabRicerca.FindControl("rblCentraleLocale")

        If Not IsNothing(btnNew) Then btnNew.Enabled = Me.searchPerformed
        If Not rblCentraleLocale Is Nothing Then rblCentraleLocale.Enabled = Me.searchPerformed
        If Not IsNothing(btnConfirm) Then btnConfirm.Enabled = (odpRicercaPaziente.getRecordCount() > 0)

    End Sub

    Private Sub odpRicercaPaziente_beforeOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpRicercaPaziente.beforeOperation

        If operation = Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord Then

            'quando si effettua un inserimento in modalità 4 ci si assicura di essere in locale
            If _tipoAnags = Onit.OnAssistnet.OnVac.Enumerators.TipoAnags.CentraleLettLocaleAgg Then
                blnSceltoLocale = True
                ImpostaCentrale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.none, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)
                ImpostaLocale(Onit.Controls.OnitDataPanel.Connection.readAuthorizations.read, Onit.Controls.OnitDataPanel.Connection.writeAuthorizations.none)
            End If

            'ElseIf operation = Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.Find Then

            ' TODO [profilo-MMG-PLS]: il giro funziona, da fare x bene! => FONDERE CON RICERCAPAZIENTEBIS

            ' N.B. : Così funziona perchè il pannello usa la classe Adapter!
            'If Me.UtenteLoggatoIsMedico Then

            '    If String.IsNullOrWhiteSpace(fmMedico.Codice) Then
            '        ' TODO [profilo-MMG-PLS]: caricare tutti i codici dei medici a cui quello loggato è abilitato a vedere i pazienti
            '        fmMedico.Codice = "003705,005952"
            '    End If

            'End If

        End If

    End Sub

    Private Sub odpRicercaPaziente_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpRicercaPaziente.afterOperation

        Dim intTrovati As Integer

        If operation = Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.Find Then

            Me.searchPerformed = True
            odpRicercaPaziente_controlLayoutChanging(sender, operation)

            If Not IsNothing(labelRisultati) Then

                intTrovati = odpRicercaPaziente.getRecordCount()
                Me.labelRisultati.Text = "Risultati della ricerca: " + intTrovati.ToString + IIf(intTrovati = 1, " paziente trovato", " pazienti trovati")

                'testo se è stato raggiunto il numero massimo di record consentito
                If Me.odpRicercaPaziente.Connections("centrale").ReadAuth = Onit.Controls.OnitDataPanel.Connection.readAuthorizations.read AndAlso
                   Not odpRicercaPaziente.Connections("centrale").Parameters("strErrMsg") Is Nothing Then

                    Dim str As String = Me.odpRicercaPaziente.Connections("centrale").Parameters("strErrMsg").value.ToString().Trim()
                    If str <> "" Then
                        AlertClientMsg(str)
                    End If
                End If

            End If


            ' TODO [profilo-MMG-PLS]: il giro funziona! => FONDERE CON RICERCAPAZIENTEBIS
            '' Sbianca il campo codice se è stato valorizzato dall'applicativo per filtrare su tutti i medici del gruppo
            'If Me.UtenteLoggatoIsMedico Then
            '    If fmMedico.Codice.Contains(",") Then
            '        fmMedico.Codice = String.Empty
            '    End If
            'End If


        End If

    End Sub

    'gestisce il rimbalzo al master in caso di ricerca non locale
    Private Sub odpRicercaPaziente_onSendDetailStruct(ByVal sender As Onit.Controls.OnitDataPanel.OnitDataPanel, ByVal detailStruct As Onit.Controls.OnitDataPanel.infoDetailStruct) Handles odpRicercaPaziente.onSendDetailStruct

        Dim drCurRow As DataRow
        Dim strMasterId, strPazTipo As String
        Dim codiceFilters As Onit.Controls.OnitDataPanel.FilterCollection

        Dim blnRimbalzaAlMaster As Boolean = _tipoAnags <> Onit.OnAssistnet.OnVac.Enumerators.TipoAnags.SoloLocale
        If _tipoAnags = Onit.OnAssistnet.OnVac.Enumerators.TipoAnags.CentraleLettLocaleAgg And blnSceltoLocale Then
            blnRimbalzaAlMaster = False
        End If

        If blnRimbalzaAlMaster Then

            'controllo se la riga corrente è un alias
            drCurRow = Me.odpRicercaPaziente.getCurrentDataRow()
            If Not IsNothing(drCurRow) Then

                strPazTipo = Me.odpRicercaPaziente.GetCurrentTableEncoder.getValOf(drCurRow, "centrale", "paz_tipo", "t_paz_pazienti_centrale", True).ToString()

                If String.Compare(strPazTipo, "A", True) = 0 Then

                    'ricavo il codice del master
                    strMasterId = Me.odpRicercaPaziente.GetCurrentTableEncoder.getValOf(drCurRow, "centrale", "paz_alias", "t_paz_pazienti_centrale", True)

                    'aggiorno il filtro con questo codice
                    codiceFilters = detailStruct.filters.search("centrale", "T_PAZ_PAZIENTI_CENTRALE", "PAZ_CODICE", True)

                    If codiceFilters.Count > 0 Then
                        codiceFilters(0).Value = strMasterId
                        detailStruct.alertMessage = "Il paziente scelto era un alias. Sono stati caricati i dati reali del paziente master"
                    End If

                End If

            End If

        End If

    End Sub

#End Region

#Region " Ordinamento ricerca "

    Private Sub odpRicercaPaziente_onCreateQuery(ByRef QB As Object) Handles odpRicercaPaziente.onCreateQuery

        Dim abstractQB As AbstractQB = DirectCast(QB, AbstractQB)

        Dim strSql As String = abstractQB.GetSelect()

        Dim strFields(), strField As String

        If Me._tipoAnags = Onit.OnAssistnet.OnVac.Enumerators.TipoAnags.SoloLocale Then

            If strSql.ToLower.IndexOf("t_paz_pazienti") >= 0 Then

                'si sta eseguendo la query sulla tabella pazienti locale
                If Not String.IsNullOrWhiteSpace(Settings.RICERCA_PAZ_ORDINAMENTO) Then

                    strFields = Settings.RICERCA_PAZ_ORDINAMENTO.Split(",")

                    For Each strField In strFields
                        If strField.Trim <> "" Then
                            abstractQB.AddOrderByFields(strField)
                        End If
                    Next

                End If

            End If

        End If

    End Sub

#End Region

#Region " Switch centrale locale - modo 4 "

    Private Sub rblCentraleLocale_SelectedIndexChanged(sender As Object, e As System.EventArgs)

        blnSceltoLocale = (DirectCast(sender, System.Web.UI.WebControls.RadioButtonList).SelectedValue = "locale")

    End Sub

#End Region

#End Region

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(sender As System.Object, e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
        _tipoAnags = Me.Settings.TIPOANAG
    End Sub

#End Region

#Region " Page Events "

    Private Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        Dim fmComNacita As Onit.Controls.OnitDataPanel.wzFinestraModale = Me.tabRicerca.FindControl("WzFinestraModale1")
        Me.ClientScript.RegisterClientScriptBlock(GetType(String), "finestraModaleNacita_id", "<script language='javascript'> var finestraModaleNacita_id='" + fmComNacita.ClientID + "';</script>")

    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub ToolBar_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case be.Button.Key

            Case "btnFind"

                ' TODO [profilo-MMG-PLS]: BUG!!! Questo è da togliere se no fa la ricerca due volte!!!
                Me.Ricerca()

            Case "btnConfirm"

                Dim drPazRow As DataRow = odpRicercaPaziente.getCurrentDataRow()
                Dim obj As Object = odpRicercaPaziente.GetCurrentTableEncoder().getValOf(drPazRow, "locale", "PAZ_CODICE", "T_PAZ_PAZIENTI", True)

                If obj Is Nothing OrElse obj Is System.DBNull.Value OrElse String.IsNullOrEmpty(obj.ToString()) Then
                    AlertClientMsg("Impossibile visualizzare il dettaglio del paziente. Paziente non presente in anagrafe locale.")
                Else
                    RedirectToVacEseguite(obj.ToString())
                End If

        End Select

    End Sub

#End Region

#Region " Private "

    Private Sub Ricerca()

        If Me.UtenteLoggatoIsMedico Then

            Dim fmMedico As Onit.Controls.OnitDataPanel.wzFinestraModale = tabRicerca.FindControl("fmMedico")

            If fmMedico.Codice = Nothing Then
                Me.SetFmMedicoToUtenteLoggato()
            End If

        End If

        odpRicercaPaziente.Find()

    End Sub

    Private Sub RedirectToVacEseguite(codicePaziente As String)

        OnVacUtility.Variabili.PazId = codicePaziente

        Response.Redirect(String.Format("./VacEseguitePS.aspx?LoadLeftFramePS={0}", HttpUtility.UrlEncode(Me.LoadLeftFramePS)))

    End Sub

    Private Sub SetFmMedicoToUtenteLoggato()

        Dim fmMedico As Onit.Controls.OnitDataPanel.wzFinestraModale = Me.tabRicerca.FindControl("fmMedico")

        fmMedico.Codice = Me.CodiceMedicoUtenteLoggato
        fmMedico.RefreshDataBind()

    End Sub

#End Region

#Region " Protected "

    Protected Function HideLeftFrameIfNeeded() As String

        If Not Page.IsPostBack Then
            Return Me.GetOpenLeftFrameScript(False)
        End If

        Return String.Empty

    End Function

#End Region

End Class
