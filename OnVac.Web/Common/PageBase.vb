Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager
Imports Onit.Shared.Web.Static

Namespace Common

    Public Class PageBase
        Inherits Onit.OnAssistnet.Web.UI.Page

#Region " Properties "

#Region " Gestione Parametri "

        Private _settings As Settings.Settings = Nothing

        Public ReadOnly Property Settings() As Settings.Settings
            Get
                If _settings Is Nothing Then
                    _settings = LoadSettingsFromDb(OnVacUtility.Variabili.CNS.Codice)
                End If
                Return _settings
            End Get
        End Property

        Friend Shared Function LoadSettingsFromDb(cnsCodice As String) As Settings.Settings

            Dim _sets As Settings.Settings

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                _sets = New Settings.Settings(cnsCodice, genericProvider)

            End Using

            Return _sets

        End Function

#End Region

#Region " Flag Dati Vaccinali Usl Gestite "

        Private Enum TipoFlagUslGestita
            FlagAbilitazioneVacc = 0
            FlagConsensoVacc = 1
        End Enum

        Private _FlagAbilitazioneVaccUslCorrente As Boolean? = Nothing

        ''' <summary>
        ''' Flag che indica se la usl corrente è abilitata alla gestione centralizzata dello storico vaccinale
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property FlagAbilitazioneVaccUslCorrente() As Boolean
            Get
                If Not _FlagAbilitazioneVaccUslCorrente.HasValue Then
                    _FlagAbilitazioneVaccUslCorrente = GetFlagVaccUslCorrente(TipoFlagUslGestita.FlagAbilitazioneVacc)
                End If
                Return _FlagAbilitazioneVaccUslCorrente.Value
            End Get
        End Property

        Private _FlagConsensoVaccUslCorrente As Boolean? = Nothing

        ''' <summary>
        ''' Flag che indica se la usl corrente ha dato il consenso alla distribuzione dei propri dati vaccinali
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property FlagConsensoVaccUslCorrente() As Boolean
            Get
                If Not _FlagConsensoVaccUslCorrente.HasValue Then
                    _FlagConsensoVaccUslCorrente = GetFlagVaccUslCorrente(TipoFlagUslGestita.FlagConsensoVacc)
                End If
                Return _FlagConsensoVaccUslCorrente.Value
            End Get
        End Property

        Private Function GetFlagVaccUslCorrente(tipoFlag As TipoFlagUslGestita) As Boolean

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                Using bizUsl As New Biz.BizUsl(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    If Not bizUsl.UslGestitaCorrente Is Nothing Then

                        If tipoFlag = TipoFlagUslGestita.FlagAbilitazioneVacc Then

                            Return bizUsl.UslGestitaCorrente.FlagAbilitazioneDatiVaccinaliCentralizzati

                        ElseIf tipoFlag = TipoFlagUslGestita.FlagConsensoVacc Then

                            Return bizUsl.UslGestitaCorrente.FlagConsensoDatiVaccinaliCentralizzati

                        Else

                            Throw New NotSupportedException()

                        End If

                    End If

                End Using

            End Using

            Return False

        End Function

        Protected Friend Function GetValoreVisibilitaDatiVaccinaliDefault(codicePaziente As String) As String

            Return Common.OnVacStoricoVaccinaleCentralizzato.GetValoreVisibilitaDatiVaccinaliDefault(Settings, codicePaziente)

        End Function

#End Region

#Region " Medico "

        Private _CodiceMedicoUtenteLoggato As String
        Protected ReadOnly Property CodiceMedicoUtenteLoggato() As String
            Get
                If _CodiceMedicoUtenteLoggato Is Nothing Then

                    Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                        _CodiceMedicoUtenteLoggato = genericProvider.Utenti.GetMedicoDaUtente(OnVacContext.UserId)
                    End Using

                End If

                Return _CodiceMedicoUtenteLoggato
            End Get
        End Property

        Protected ReadOnly Property UtenteLoggatoIsMedico() As Boolean
            Get
                Return Not String.IsNullOrEmpty(Me.CodiceMedicoUtenteLoggato)
            End Get
        End Property

#End Region

        Public ReadOnly Property IsGestioneCentrale() As Boolean
            Get
                Dim isCentrale As String = Request.QueryString("isCentrale")

                If String.IsNullOrEmpty(isCentrale) Then Return False

                Return Boolean.Parse(isCentrale)
            End Get
        End Property

        Public ReadOnly Property IsProntoSoccorso() As Boolean
            Get
                Dim isPS As String = Request.QueryString("isPS")

                If String.IsNullOrEmpty(isPS) Then Return False

                Return Boolean.Parse(isPS)
            End Get
        End Property

#Region " Ricerca pazienti "

        Public Property UltimaRicercaPaziente() As Entities.UltimaRicercaPazientiEffettuata
            Get
                If Session("LastSearch") Is Nothing Then Session("LastSearch") = New Entities.UltimaRicercaPazientiEffettuata()
                Return DirectCast(Session("LastSearch"), Entities.UltimaRicercaPazientiEffettuata)
            End Get
            Set(value As Entities.UltimaRicercaPazientiEffettuata)
                Session("LastSearch") = value
            End Set
        End Property

        Public Property UltimoPazienteSelezionato() As Entities.UltimoPazienteSelezionato
            Get
                If Session("LastPaz") Is Nothing Then Session("LastPaz") = New Entities.UltimoPazienteSelezionato()
                Return DirectCast(Session("LastPaz"), Entities.UltimoPazienteSelezionato)
            End Get
            Set(value As Entities.UltimoPazienteSelezionato)
                Session("LastPaz") = value
            End Set
        End Property

#End Region

#End Region

#Region " Page events "

        Protected Overrides Sub OnLoad(e As EventArgs)

            MyBase.OnLoad(e)

            Me.MyHistoryClear()

            'logger.Info(Me.GetType().ToString())

            '' Telerik css injection
            'Dim sm As System.Web.UI.ScriptManager = System.Web.UI.ScriptManager.GetCurrent(Me)
            'If (sm Is Nothing) Then
            '    sm = New System.Web.UI.ScriptManager()
            '    Form.Controls.Add(sm)
            'End If

            'Dim decorator As New Telerik.Web.UI.RadFormDecorator()
            'decorator.DecoratedControls = Telerik.Web.UI.FormDecoratorDecoratedControls.All
            'Form.Controls.Add(decorator)

        End Sub

        Protected Overridable Sub MyHistoryClear()

            DirectCast(Me.Page, Onit.Shared.Web.UI.Page).HistoryClear()

        End Sub

        Private Sub PageBase_Init(sender As Object, e As System.EventArgs) Handles MyBase.Init

            InitLog()

            Dim testResult As ManageTestConfigurationResultType = ManageTestConfiguration()

            If testResult = ManageTestConfigurationResultType.OnVacInTestUtenteNonValido Then
                '---
                ' Redirect alla pagina di blocco per lavori in corso
                Server.Transfer(ResolveClientUrl("~/WorkInProgress.html"))
                '---
            Else
                InitPage(Me)
            End If

        End Sub

        Friend Shared Sub InitPage(ByRef _page As System.Web.UI.Page)

            Dim rm As New System.Resources.ResourceManager("Onit.OnAssistnet.OnVac.Messages", System.Reflection.Assembly.GetExecutingAssembly())

            Dim checkDime As Boolean

            Try
                checkDime = Boolean.Parse(ConfigurationManager.AppSettings("CheckDime"))
            Catch ex As Exception
                Throw New ParamNotPresentException("CheckDime")
            End Try

            Dim checkSubmit As Boolean

            Try
                checkSubmit = Boolean.Parse(ConfigurationManager.AppSettings("CheckSubmit"))
            Catch ex As Exception
                Throw New ParamNotPresentException("CheckSubmit")
            End Try

            Dim checkNavigation As Boolean

            Try
                checkNavigation = Boolean.Parse(ConfigurationManager.AppSettings("CheckNavigation"))
            Catch ex As Exception
                Throw New ParamNotPresentException("CheckNavigation")
            End Try

            If checkDime Then
                '--------------------------------------------------------------------
                ' Controllo del dime di ritorno dalla pagina per i doppi click
                Dim _dimeValue As String

                Dim _dimeRep As DimeRepository
                If _page.Session("__EVENTDIME") Is Nothing Then
                    _dimeRep = New DimeRepository
                Else
                    _dimeRep = DirectCast(_page.Session("__EVENTDIME"), DimeRepository)
                End If

                If _page.IsPostBack Then
                    _dimeValue = _page.Request.Params("__EVENTDIME").ToString
                    If Not _dimeRep.Contains(_dimeValue) Then
                        Throw New IncorrectDimeException
                    End If
                End If

                '--------------------------------------------------------------------
                ' Aggiunta del nuovo dime
                _dimeValue = (New System.Random).Next.ToString()
                _dimeRep.Add(_dimeValue)
                _page.Session("__EVENTDIME") = _dimeRep
                _page.ClientScript.RegisterHiddenField("__EVENTDIME", _dimeValue)
                '--------------------------------------------------------------------
            End If

            'Aggiunta degli script globali
            _page.ClientScript.RegisterStartupScript(_page.GetType(), "Utility", "<script src='" + _page.ClientScript.GetWebResourceUrl(GetType(WebResources), "Onit.Shared.Web.Static.Scripts.Utility.js") + "' type='text/javascript' ></script>")

            If checkNavigation Then
                _page.ClientScript.RegisterStartupScript(_page.GetType(), "NavigationUtility", "<script src='" + _page.ResolveUrl("~/Scripts/NavigationUtility.js") + "' type='text/javascript' ></script>")
            End If

            If checkSubmit Then
                _page.ClientScript.RegisterStartupScript(_page.GetType(), "doCheckSubmitAndPostBack", rm.GetString("doPostBack"))
            End If

        End Sub

        Protected Enum ManageTestConfigurationResultType
            OnVacInRelease = 0
            OnVacInTestUtenteValido = 1
            OnVacInTestUtenteNonValido = 2
        End Enum

        Protected Function ManageTestConfiguration() As ManageTestConfigurationResultType

            Dim isTest As Boolean = False

            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("AppTest")) Then

                If Not Boolean.TryParse(ConfigurationManager.AppSettings.Get("AppTest"), isTest) Then
                    isTest = False
                End If

            End If

            If Not isTest Then
                Return ManageTestConfigurationResultType.OnVacInRelease
            End If

            ' Controllo utente per impedire l'accesso agli operatori
            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("AppTestUsers")) Then

                Dim users() As String = ConfigurationManager.AppSettings.Get("AppTestUsers").Split({";"}, StringSplitOptions.RemoveEmptyEntries)
                If Not users.Any(Function(user) user = OnVacContext.UserCode) Then
                    Return ManageTestConfigurationResultType.OnVacInTestUtenteNonValido
                End If

            End If

            Return ManageTestConfigurationResultType.OnVacInTestUtenteValido

        End Function

#End Region

#Region " Controlli "

        Protected Class CheckResult
            Public Success As Boolean
            Public Message As String
            Public Sub New()
                Me.Success = True
                Me.Message = String.Empty
            End Sub
        End Class

        ''' <summary>
        ''' Controlla che il valore specificato sia composto solo da numeri, lettere e dai caratteri ".", "_" e "-".
        ''' </summary>
        ''' <param name="codice"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Function CheckCampoCodice(codice As String) As CheckResult

            Dim result As New CheckResult()

            Dim regEx As New System.Text.RegularExpressions.Regex("^[\.\-\w]*$")

            If Not regEx.IsMatch(codice) Then
                result.Success = False
                result.Message = "Il codice specificato non è valido. Caratteri ammessi: numeri, lettere e i caratteri \'.\', \'-\' e \'_\'"
            End If

            Return result

        End Function

        ''' <summary>
        ''' Restituisce true se il valore specificato è composto esclusivamente da numeri e di lunghezza non superiore a maxCifre.
        ''' </summary>
        ''' <param name="valore"></param>
        ''' <param name="maxCifre"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Function CheckCampoNumericoIntero(valore As String, maxCifre As Integer) As Boolean

            If valore.Length > maxCifre Then Return False

            Dim regEx As New System.Text.RegularExpressions.Regex("^[\d]*$")
            Return regEx.IsMatch(valore)

        End Function

#End Region

#Region " Gestione visibilità pulsanti di stampa "

        <Serializable()>
        Public Class PrintButton

            Private rptName As String
            Public Property NomeReport() As String
                Get
                    Return rptName
                End Get
                Set(value As String)
                    rptName = value
                End Set
            End Property

            Private btnName As String
            Public Property NomePulsante() As String
                Get
                    Return btnName
                End Get
                Set(value As String)
                    btnName = value
                End Set
            End Property

            Public Sub New()
            End Sub

            Public Sub New(nomeReport As String, nomePulsante As String)
                rptName = nomeReport
                btnName = nomePulsante
            End Sub

        End Class

        '''<summary>
        ''' Gestione della visualizzazione dei pulsanti di stampa in base all'installazione
        '''</summary>
        Public Sub ShowPrintButton(nomeReport As String, ByRef button As Button)

            If String.IsNullOrEmpty(nomeReport) Then Return

            If button Is Nothing Then Return

            ' Caricamento report
            Dim rpt As Entities.Report = Nothing

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                    rpt = bizReport.GetReport(nomeReport)

                End Using
            End Using

            button.Visible = (Not rpt Is Nothing)

        End Sub

        '''<summary>
        ''' Gestione della visualizzazione dei pulsanti di stampa presenti nella toolbar, in base all'installazione
        '''</summary>
        Public Sub ShowToolbarPrintButtons(listPrintButtons As List(Of PrintButton), toolbar As Infragistics.WebUI.UltraWebToolbar.UltraWebToolbar)

            If listPrintButtons Is Nothing OrElse listPrintButtons.Count = 0 Then Return

            ' Caricamento report
            Dim rptList As List(Of Entities.Report) = Me.LoadRptList(listPrintButtons)

            If rptList Is Nothing OrElse rptList.Count = 0 Then
                ' Se non c'è nessun report da cercare nella lista, nascondo tutti i pulsanti
                For i As Integer = 0 To listPrintButtons.Count - 1
                    toolbar.Items.FromKeyButton(listPrintButtons(i).NomePulsante).Visible = False
                Next
            Else
                For i As Integer = 0 To listPrintButtons.Count - 1
                    SetToolbarButtonVisibility(listPrintButtons(i), rptList, toolbar)
                Next
            End If

        End Sub

        '''<summary>
        ''' Gestione della visualizzazione dei pulsanti di stampa presenti nella toolbar, in base all'installazione
        '''</summary>
        Public Sub ShowToolbarPrintButtons(listPrintButtons As List(Of PrintButton), toolbar As Telerik.Web.UI.RadToolBar)

            If listPrintButtons Is Nothing OrElse listPrintButtons.Count = 0 Then Return

            ' Caricamento report
            Dim rptList As List(Of Entities.Report) = Me.LoadRptList(listPrintButtons)

            If rptList Is Nothing OrElse rptList.Count = 0 Then
                ' Se non c'è nessun report da cercare nella lista, nascondo tutti i pulsanti
                For i As Integer = 0 To listPrintButtons.Count - 1
                    toolbar.FindItemByValue(listPrintButtons(i).NomePulsante).Visible = False
                Next
            Else
                For i As Integer = 0 To listPrintButtons.Count - 1
                    SetToolbarButtonVisibility(listPrintButtons(i), rptList, toolbar)
                Next
            End If

            ' Imposta la visibilità del Telerik.Web.UI.RadToolBarDropDown
            Me.SetDropDownButtonVisibility(toolbar)

        End Sub

        Private Function LoadRptList(listPrintButtons As List(Of PrintButton)) As List(Of Entities.Report)

            Dim rptList As List(Of Entities.Report) = Nothing

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                    Dim listRptNames As List(Of String) = (From element As PrintButton In listPrintButtons
                                                           Select element.NomeReport).ToList()

                    rptList = bizReport.GetReports(listRptNames)

                End Using
            End Using

            Return rptList

        End Function

        ' Se il report non è presente nella lista, nasconde il pulsante specificato della toolbar.
        Private Sub SetToolbarButtonVisibility(printBtn As PrintButton, rptList As List(Of Entities.Report), toolbar As Infragistics.WebUI.UltraWebToolbar.UltraWebToolbar)

            ' Cerco il report nella lista per sapere se devo nascondere o visualizzare il pulsante che serve per stamparlo
            Dim rpt As Entities.Report = (From r As Entities.Report In rptList
                                          Where r.Nome = printBtn.NomeReport
                                          Select r).FirstOrDefault()

            toolbar.Items.FromKeyButton(printBtn.NomePulsante).Visible = (Not rpt Is Nothing)

        End Sub

        ' Se il report non è presente nella lista, nasconde il pulsante specificato della toolbar.
        Private Sub SetToolbarButtonVisibility(printBtn As PrintButton, rptList As List(Of Entities.Report), toolbar As Telerik.Web.UI.RadToolBar)

            ' Cerco il report nella lista per sapere se devo nascondere o visualizzare il pulsante che serve per stamparlo
            Dim rpt As Entities.Report = (From r As Entities.Report In rptList
                                          Where r.Nome = printBtn.NomeReport
                                          Select r).FirstOrDefault()

            toolbar.FindItemByValue(printBtn.NomePulsante).Visible = (Not rpt Is Nothing)

        End Sub

        Private Sub SetDropDownButtonVisibility(toolbar As Telerik.Web.UI.RadToolBar)

            For i As Integer = 0 To toolbar.Items.Count - 1

                Dim item As Telerik.Web.UI.RadToolBarItem = toolbar.Items(i)
                If item.GetType() Is GetType(Telerik.Web.UI.RadToolBarDropDown) Then

                    Dim a As Telerik.Web.UI.RadToolBarDropDown = TryCast(item, Telerik.Web.UI.RadToolBarDropDown)
                    If Not a Is Nothing Then

                        Dim containItems As Boolean = False

                        For j As Integer = 0 To a.Buttons.Count - 1
                            If a.Buttons(j).Visible Then
                                containItems = True
                                Exit For
                            End If
                        Next

                        a.Visible = containItems

                    End If

                End If

            Next

        End Sub

#End Region

#Region " DimeRepository "

        <Serializable()>
        Public Class DimeRepository

            Private dime1 As String = String.Empty
            Private dime2 As String = String.Empty
            Private dime3 As String = String.Empty

            Public Sub Add(dime As String)

                dime3 = dime2
                dime2 = dime1
                dime1 = dime

            End Sub

            Public Function Contains(dime As String) As Boolean

                If dime = dime1 OrElse dime = dime2 OrElse dime = dime3 Then
                    Return True
                Else
                    Return False
                End If

            End Function

        End Class

#End Region

#Region " Redirect "

        ''' <summary>
        ''' Effettua il redirect alla maschera di gestione paziente, impostando il paziente corrente e il menu relativo ad HPazienti (locale)
        ''' </summary>
        Public Overridable Sub RedirectToGestionePaziente(codicePaziente As String)

            OnVacUtility.Variabili.PazId = codicePaziente

            OnVacContext.MenuDis = Me.GetMenuDis()

            Response.Redirect(Me.ResolveClientUrl("~/HPazienti/GestionePazienti/GestionePazienti.aspx?fromRedirect=true"), False)

        End Sub

        ''' <summary>
        ''' Effettua il redirect alla maschera delle convocazioni del paziente, impostando il paziente corrente e il menu relativo ad HPazienti (locale)
        ''' </summary>
        Public Sub RedirectToConvocazioniPaziente(codicePaziente As String)

            Me.RedirectToConvocazioniPaziente(codicePaziente, Nothing, Nothing, Nothing, True)

        End Sub

        Public Sub RedirectToConvocazioniPaziente(codicePaziente As String, editCnv As Boolean?, askAutoCnv As Boolean?, ignoreAutoCnv As Boolean?, fromRedirect As Boolean?)

            OnVacUtility.Variabili.PazId = codicePaziente

            OnVacContext.MenuDis = Me.GetMenuDis()

            Dim url As New System.Text.StringBuilder("~/HPazienti/Convocazioni/Convocazioni.aspx")

            Dim queryString As New System.Text.StringBuilder()

            If editCnv.HasValue Then
                queryString.AppendFormat("&EditCnv={0}", HttpUtility.UrlEncode(editCnv.Value))
            End If

            If askAutoCnv.HasValue Then
                queryString.AppendFormat("&AskAutoCnv={0}", HttpUtility.UrlEncode(askAutoCnv.Value))
            End If

            If ignoreAutoCnv.HasValue Then
                queryString.AppendFormat("&IgnoreAutoCnv={0}", HttpUtility.UrlEncode(ignoreAutoCnv.Value))
            End If

            If fromRedirect.HasValue Then
                queryString.AppendFormat("&fromRedirect={0}", HttpUtility.UrlEncode(fromRedirect.Value))
            End If

            If queryString.Length > 0 Then
                url.AppendFormat("?{0}", queryString.Remove(0, 1))
            End If

            Me.Response.Redirect(Me.ResolveClientUrl(url.ToString()))

        End Sub

        ' Imposta nella property MenuDis del Context il valore dei menu disabilitati per l'appid corrente, separati da *
        Private Function GetMenuDis() As String

            Dim menuDis As New System.Text.StringBuilder()

            For Each menWeight As String In Me.Settings.MENUDIS
                menuDis.AppendFormat("{0}-{1}*", OnVacContext.AppId, menWeight)
            Next

            If menuDis.Length > 0 Then
                menuDis.Remove(menuDis.Length - 1, 1)
            End If

            Return menuDis.ToString()

        End Function

        Public Function GetMenuDis_Appuntamenti() As String

            Return GetMenuDis(Settings.MENUDIS_APPUNTAMENTI)

        End Function

        Public Function GetMenuDis_HPazienti() As String

            Return GetMenuDis(Settings.MENUDIS_HPAZIENTI)

        End Function

        Private Function GetMenuDis(valore_menu_dis As String) As String

            Dim menuDis As New System.Text.StringBuilder()

            If Not String.IsNullOrWhiteSpace(valore_menu_dis) Then
                menuDis.AppendFormat("{0}-{1}", OnVacContext.AppId, valore_menu_dis)
            End If

            Return menuDis.ToString()

        End Function

#End Region

#Region " Log "

        Private Shared logInitialized As Boolean = False
        Private Shared logSynLock As Object = New Object()

        Private Sub InitLog()

            If Not logInitialized Then

                SyncLock (logSynLock)

                    If Not logInitialized Then

                        Try

                            LogBox.InitLog()

                            If LogBox.IsEnabled Then

                                AddHandler LogBox.PreWriteLog, AddressOf LogBox_PreWriteLog
                                AddHandler LogBox.PreWriteLogGroup, AddressOf LogBox_PreWriteLogGroup

                                Me.CaricaArgomentiLog()

                                logInitialized = True

                            End If

                        Catch ex As OnVacConfigurationException

                            ex.InternalPreserveStackTrace()
                            Throw

                        Catch ex As LogBoxWriteDataException

                            ex.InternalPreserveStackTrace()
                            Throw

                        Catch ex As Exception

                            ' Se è stata sollevata un'eccezione generica, ne faccio risalire una tipizzata (con la generica come innerexception)
                            Throw New LogBoxWriteDataException(ex)

                        End Try

                    End If

                End SyncLock

            End If

        End Sub

        ' Lettura dei vari argomenti che servono al Log per l'inizializzazione. Tutti gli argomenti possibili sono elencati nella t_log_argomenti.
        ' In caso di errore, viene sollevata una eccezione di tipo LogBoxWriteData, contenente come innerException quella avvenuta realmente.
        ' In più viene impostato il messaggio della innerexception anche nel messaggio dell'eccezione del LogBox.
        ' Questo perchè, a video, si può vedere subito se è una oracle exception (ed è più comodo per chi sta facendo l'installazione).
        Private Sub CaricaArgomentiLog()

            Using dam As IDAM = OnVacUtility.OpenDam(OnVacContext.AppId, OnVacContext.Azienda)

                Try

                    Using genericProvider As New DAL.DbGenericProvider(dam)
                        Using bizLog As New Biz.BizLog(genericProvider, OnVacContext.CreateBizContextInfos())

                            Dim listArgomentiLog As List(Of DataLogStructure.Argomento) = bizLog.GetListArgomentiAttivi()

                            If Not listArgomentiLog Is Nothing Then
                                For Each argomentoLog As DataLogStructure.Argomento In listArgomentiLog
                                    LogBox.Argomenti.Add(argomentoLog)
                                Next
                            End If

                        End Using
                    End Using

                Catch ex As Exception

                    Throw New LogBoxWriteDataException(ex, ex.Message)

                End Try

            End Using

        End Sub

        Private Sub LogBox_PreWriteLog(log As LogWriterProvider.ILogWriterProvider, t As Log.DataLogStructure.Testata, ByRef connString As String)

            Dim maschera As String = String.Empty
            Dim stack As String = String.Empty
            Dim computerName As String = String.Empty
            Dim cns As String = String.Empty
            Dim pazId, uteId As Integer

            Dim connInfo As ConnectionInfo = OnVacContext.Connection
            connString = connInfo.ConnectionString

            Try
                If (t.Maschera = String.Empty) Then
                    Me.CaricaMascheraDefault(maschera, stack)
                End If

                Me.CaricaCampiSessionDefault(pazId, uteId, computerName, cns)
                Me.CaricaCampiTestate(maschera, stack, OnVacUtility.Variabili.PazId, uteId, computerName, cns, t)

            Catch ex As Exception
                ' Nothing to do
            End Try

        End Sub

        Private Sub LogBox_PreWriteLogGroup(log As LogWriterProvider.ILogWriterProvider, t As Log.DataLogStructure.Testata(), ByRef connString As String)

            Dim maschera As String = String.Empty
            Dim stack As String = String.Empty
            Dim computerName As String = String.Empty
            Dim cns As String = String.Empty
            Dim pazId, uteId As Integer

            Dim connInfo As ConnectionInfo = OnVacContext.Connection
            connString = connInfo.ConnectionString

            Try

                If Not t Is Nothing AndAlso t.Length > 0 AndAlso Not t(0).Maschera = String.Empty Then
                    maschera = t(0).Maschera
                Else
                    Me.CaricaMascheraDefault(maschera, stack)
                End If

                Me.CaricaCampiSessionDefault(pazId, uteId, computerName, cns)

                For Each test As Log.DataLogStructure.Testata In t
                    Me.CaricaCampiTestate(maschera, stack, OnVacUtility.Variabili.PazId, uteId, computerName, cns, test)
                Next

            Catch ex As Exception
                ' Nothing to do
            End Try

        End Sub

        Private Sub CaricaCampiSessionDefault(ByRef pazId As Integer, ByRef uteId As Integer, ByRef computerName As String, ByRef cns As String)

            Try
                pazId = OnVacUtility.Variabili.PazId
                uteId = OnVacContext.UserId

                computerName = OnVacContext.CurrentMachine.ComputerName

                If OnVacUtility.Variabili.CNS Is Nothing Then
                    cns = "non disponibile"
                Else
                    cns = String.Format("{0} ({1})", OnVacUtility.Variabili.CNS.Descrizione, OnVacUtility.Variabili.CNS.Codice)
                End If

            Catch ex As Exception

                pazId = 0
                uteId = 0
                computerName = "non disponibile"
                cns = "non disponibile"

            End Try

        End Sub

        Private Sub CaricaMascheraDefault(ByRef maschera As String, ByRef stack As String)

            Try
                Dim i As Integer

                Dim stackTrace As System.Diagnostics.StackTrace = New System.Diagnostics.StackTrace(0, True)

                For i = stackTrace.FrameCount - 1 To 0 Step -1

                    If Not stackTrace.GetFrame(i).GetFileName() Is Nothing Then

                        If IsFileToLog(stackTrace.GetFrame(i).GetFileName()) Then

                            Exit For

                        End If

                    End If

                Next

                Dim stackFrame As System.Diagnostics.StackFrame = stackTrace.GetFrame(i)

                If stackFrame Is Nothing Then

                    maschera = "non disponibile"
                    stack = "non disponibile"

                Else

                    Dim str() As String = stackFrame.GetFileName().Split("\"c)

                    maschera = str(str.Length - 1)
                    stack = String.Format("{0} (riga: {1})", stackFrame.GetMethod().Name, stackFrame.GetFileLineNumber())

                End If

            Catch ex As Exception

                maschera = "non disponibile"
                stack = "non disponibile"

            End Try

        End Sub

        ' Se nello stack è presente un file .aspx, oppure è presente una delle classi elencate, 
        ' la imposta come nome della maschera
        Private Function IsFileToLog(fileName As String) As Boolean

            If fileName.ToUpper().IndexOf(".ASPX") > -1 Then Return True

            Dim nomeClasse As String = GetType(OnVac.Common.OnVacMovimentiPageBase).Name.ToUpper()

            If fileName.ToUpper().IndexOf(nomeClasse + ".VB") > -1 Then Return True

            ' N.B. : se vanno previste altre classi per cui loggare il nome della classe nel campo "Maschera" del log, 
            '        aggiungerle qui! 

            Return False

        End Function

        Private Sub CaricaCampiTestate(maschera As String, stack As String, pazId As Integer, uteId As Integer, computerName As String, cns As String, t As Log.DataLogStructure.Testata)

            If t.Maschera = String.Empty Then t.Maschera = maschera
            If t.Stack = String.Empty Then t.Stack = stack
            If t.Paziente = 0 Then t.Paziente = pazId
            If t.Utente = 0 Then t.Utente = uteId
            If t.ComputerName = String.Empty Then t.ComputerName = computerName
            If t.Consultorio = String.Empty Then t.Consultorio = cns

        End Sub

#End Region

#Region " Custom Script "

        Protected Friend Sub RegisterStartupScriptCustom(key As String, scriptContent As String)

            ' Define the name and type of the client scripts on the page. 
            Dim cstype As Type = Me.[GetType]()

            ' Get a ClientScriptManager reference from the Page class. 
            Dim cs As ClientScriptManager = Page.ClientScript

            ' Check to see if the startup script is already registered. 
            If Not cs.IsStartupScriptRegistered(cstype, key) Then

                Dim s As New System.Text.StringBuilder()

                s.Append("<script type=text/javascript>")
                s.Append(scriptContent)
                s.Append("</script>")

                cs.RegisterStartupScript(cstype, key, s.ToString())

            End If

        End Sub

        ''' <summary>
        ''' Restituisce il codice javascript per visualizzare/nascondere il left frame
        ''' </summary>
        ''' <param name="showLeftFrame"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Function GetOpenLeftFrameScript(showLeftFrame As Boolean) As String

            Dim scriptLeftFrame As New System.Text.StringBuilder()

            If showLeftFrame Then
                scriptLeftFrame.AppendFormat("top.frames[1].location='{0}/Layout/LeftFrame.aspx?MenAppId={1}&AppId={2}&FromMain=1&MenuDis={3}&enable=true';",
                                             Request.ApplicationPath, OnVacContext.MenAppId, OnVacContext.AppId, OnVacContext.MenuDis)
            Else
                scriptLeftFrame.AppendFormat("top.frames[1].location='{0}/Layout/LeftFrame.aspx?MenAppId=&AppId=&men_weight=';",
                                             Request.ApplicationPath)
            End If

            Return scriptLeftFrame.ToString()

        End Function

        ''' <summary>
        ''' Effettua la renderizzazione dello script per visualizzare/nascondere il left frame
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend Sub OpenLeftFrame(showLeftFrame As Boolean)

            Dim js As String = String.Format("<script type='text/javascript'>{0}</script>", Me.GetOpenLeftFrameScript(showLeftFrame))

            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "openLeftFrame", js)

        End Sub

#End Region

#Region " Report "

        Protected Function GetReportInstance(genericProvider As DAL.DbGenericProvider, reportName As String) As Telerik.Reporting.Report

            Dim reportFolder As String

            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                reportFolder = bizReport.GetReportFolder(reportName)
            End Using

            Dim reportTypeString As String = String.Format("Onit.OnAssistnet.OnVac.Report.{0}.{1}, Onit.OnAssistnet.OnVac.Report.{0}", reportFolder, reportName)
            Dim reportType As Type = Type.GetType(reportTypeString, True)

            Dim report As Telerik.Reporting.Report = Activator.CreateInstance(reportType)

            Return report

        End Function

#End Region

#Region " Consenso "

        ''' <summary>
        ''' Restituisce l'url per la pop-up di rilevazione del consenso
        ''' </summary>
        ''' <param name="codiceCentralePaziente"></param>
        ''' <param name="autoEdit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Function GetUrlMascheraRilevazioneConsenso(codiceCentralePaziente As String, autoEdit As Boolean) As String

            Dim urlConsenso As New System.Text.StringBuilder(ResolveClientUrl("~/OpenConsensi.aspx"))

            urlConsenso.AppendFormat("?user={0}&paziente={1}", OnVacContext.UserCode, codiceCentralePaziente)
            If autoEdit Then urlConsenso.Append("&edit=true")

            Return urlConsenso.ToString()

        End Function

#End Region

#Region " Consultori "

        Protected Function GetRichiestaConsensoTrattamentoDatiUtente(codiceConsultorio As String) As Boolean

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizCns As New Biz.BizConsultori(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    Return bizCns.GetRichiestaConsensoTrattamentoDatiUtente(codiceConsultorio)

                End Using
            End Using

        End Function

#End Region

#Region " StringResources "

        ''' <summary>
        ''' Restituisce il messaggio in base alla stringa di risorse specificata
        ''' </summary>
        ''' <param name="resourceKey"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Function GetOnVacResourceValue(resourceKey As String) As String

            Return GetGlobalResourceObject("Onit.OnAssistnet.OnVac.Web", resourceKey).ToString()

        End Function

#End Region

    End Class

End Namespace
