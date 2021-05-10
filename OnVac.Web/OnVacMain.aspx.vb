Imports Onit.Database.DataAccessManager


Partial Class OnVacMain
    Inherits Common.PageBase

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

#Region " Properties "

    Protected Overrides ReadOnly Property SkipResizeIE10 As Boolean
        Get
            Return True
        End Get
    End Property

    Public Property DisabledCat() As String()
        Get
            Return Session("DisabledCat")
        End Get
        Set(Value As String())
            Session("DisabledCat") = Value
        End Set
    End Property

    Public Property AppTitle As String

    Public ReadOnly Property IsDebug() As String
        Get
#If DEBUG Then
            Return "true"
#Else
            Return "false"
#End If
        End Get
    End Property

#End Region

#Region " Page Events "

    Private Sub Page_PreInit(sender As Object, e As System.EventArgs) Handles Me.PreInit

        If Not IsPostBack Then
            '--
            OnVacContext.InitCurrentUserInformation()
            '--
        End If

    End Sub

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles MyBase.Load

#Region " Test transaction scope "

        'Dim transactionOptions As New System.Transactions.TransactionOptions()
        'transactionOptions.IsolationLevel = Transactions.IsolationLevel.ReadCommitted

        'Using transactionScope As New System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions)

        '    For i As Int16 = 0 To 20

        '        ClassLibrary1.Class1.DoQuery("797970", "OnVac_Veneto106", "050000")

        '        ClassLibrary1.Class1.DoQuery2("254672", "OnVac_Veneto107", "050000")

        '        ClassLibrary1.Class1.DoQuery("797970", "OnVac_Veneto106", "050000")

        '        ClassLibrary1.Class1.DoQuery2("254672", "OnVac_Veneto107", "050000")

        '    Next

        '    transactionScope.Complete()

        'End Using

#End Region

        If Not IsPostBack Then
            '--
            ' MenAppId: id applicativo per creazione menu
            '--
            Dim menAppId As String = Request.QueryString("MenAppId")
            If String.IsNullOrEmpty(menAppId) Then menAppId = OnVacContext.AppId
            '--
            OnVacContext.MenAppId = menAppId
            '--
            ' Codice ULSS corrente (solo nuove ulss unificate)
            '--
            OnVacContext.CodiceUslCorrente = Request.QueryString("ULSS")
            '--
            ' PageTracker
            '--
            HttpContext.Current.Session("PageTracker") = New OnVac.Common.PageTracker(4)
            '--
            'Variabile per la gestione degli archivi di base 1 = AnagrafeNew
            '--
            HttpContext.Current.Session("ModalitaAnagrafe") = 1
            '--
            'Imposta la pagina chiamante per l'eventuale back dell'anagrafe
            '--
            HttpContext.Current.Session("AnaPaginaChiamante") = HttpContext.Current.Request.Url.AbsoluteUri
            '--
            ' Test di scrittura sul registro eventi, se il parametro "AppTest" del web.config vale true
            '--
            Dim testResult As ManageTestConfigurationResultType = ManageTestConfiguration()
            If testResult = ManageTestConfigurationResultType.OnVacInTestUtenteNonValido Then
                Return
            ElseIf testResult = ManageTestConfigurationResultType.OnVacInTestUtenteValido Then
                TestScritturaEventLog()
            End If
            '--
            OnVacUtility.Variabili = New VariabiliOnVac()
            '--
            ' GetGlobalParams e' un metodo della libreria PortaleSharedLibrary che carica i parametri dalla t_ana_parametri
            ' sulla connessione ManagerConnectString impostata nel web.config.
            ' Il datatable restituito dalla query e' contenuto in rv.value
            '--
            Dim rv As Onit.Database.DataAccessManager.ReturnValue = OnVacUtility.GetGlobalParams("MAXPAZIE", "SEPAANAG", "CONDBCEN", "TIPOANAG", "NOSCON", "STORVARI", "CAMPFON1", "CAMPFON2", "OWTESSAN", "OWMEDBAS", "CHKUNICF")
            '--
            If (Not IsNothing(rv.ErrorMessage)) Then Throw New Exception(rv.ErrorMessage)
            '--
            'Caricamento dei parametri relativi al consultorio
            '--
            Dim caricamentoVar As Boolean
            '--
            'ipostazione automatica dei consultori per i nuovi utenti
            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Try
                    genericProvider.BeginTransaction()
                    ' i settings sono quelli di sistema perché il consultorio corrente non è ancora impostato
                    Using bizConsultori As New Biz.BizConsultori(genericProvider, New OnVac.Settings.Settings(genericProvider), OnVacContext.CreateBizContextInfos(), Nothing)
                        bizConsultori.VerificaAssociazioneAutomaticaConsultori(OnVacContext.UserId)
                    End Using
                    genericProvider.Commit()
                Catch ex As Exception
                    genericProvider.Rollback()
                    ex.InternalPreserveStackTrace()
                    Throw
                End Try
            End Using

            Try
                caricamentoVar = OnVacUtility.CaricaVariabiliConsultorio(String.Empty, True)
            Catch ex As UnhandledSettingException
                ' Parametro presente su db ma non gestito dalla libreria
                ex.InternalPreserveStackTrace()
                Throw
            Catch ex As CryptedSettingException
                ' Parametro criptato contenente un valore errato 
                Throw New Exception(String.Format("Impossibile accedere all'applicativo.{0}" + ex.Message, "<br>"), ex)
            Catch ex As Exception
                ' Altra eccezione non gestita
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            ' Se il caricamento non è riuscito: nessun consultorio associato
            If Not caricamentoVar Then
                Throw New Exception(String.Format("Impossibile accedere all'applicativo.{0}Nessun centro vaccinale associato a questa macchina nè all'utente corrente.{0}Nome macchina: {1}{0}Utente: {2} [{3}]",
                                                  "<br>", OnVacContext.CurrentMachine.ComputerName, OnVacContext.UserCode, OnVacContext.UserId))
            End If

            ' Caricamento parametri
            Dim settings As Onit.OnAssistnet.OnVac.Settings.Settings = Common.PageBase.LoadSettingsFromDb(OnVacUtility.Variabili.CNS.Codice)

            ' Controllo autorizzazioni
            ' Se il numero di postazioni presenti su db supera il massimo (impostato in MAXPOSTAZIONI), non si può accedere all'applicativo
            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizPostazioni As New Biz.BizPostazioni(genericProvider, settings, OnVacContext.CreateBizContextInfos())
                    '--
                    If Not bizPostazioni.CheckAutorizzazioni() Then
                        Throw New Exception("Impossibile accedere all'applicativo.<br>Autorizzazioni insufficienti. Il numero di postazioni supera il numero di licenze.")
                    End If
                    '--
                End Using
            End Using

            'costruisce il datatable delle vaccinazioni sostitute [modifica 08/08/2005]
            OnVacUtility.RecuperaVaccinazioniSostitute()

            ' Impostazione titolo
            Dim title As String = Request.QueryString("Title")

            If String.IsNullOrWhiteSpace(title) Then
                title = settings.APPTITLE
            End If

            AppTitle = String.Format("{0} {1} {2} {3} ",
                                     title,
                                     Reflection.Assembly.GetExecutingAssembly().GetName.Version(),
                                     OnVacContext.AppId,
                                     OnVacContext.CurrentMachine.ComputerName)

            ' patch per far funzionare la OnitModalList. Non utilizzarlo per le connessioni di onvac
            Session(OnVacContext.AppId + "_CNINFO") = New ConnForModallist With {.CRConnectionString = OnVacContext.GetEncryptedConnectionString(), .Provider = OnVacContext.Connection.Provider}

            '--
            ' TEST
            '--
            'Dim transactionOptions As New System.Transactions.TransactionOptions()
            'transactionOptions.IsolationLevel = Transactions.IsolationLevel.ReadCommitted
            '--
            'Using transactionScope As New System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions)
            '    doquery(1, cs)
            '    transactionScope.Complete()
            'End Using
            '--

        End If
        '--
    End Sub

    'Public Sub doquery(c As Integer, cs As String)

    '    Dim transactionOptions As New System.Transactions.TransactionOptions()
    '    transactionOptions.IsolationLevel = Transactions.IsolationLevel.ReadCommitted
    '    '--
    '    Using transactionScope As New System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions)

    '        If (c < 50) Then

    '            Using conn As New Oracle.DataAccess.Client.OracleConnection(cs + ";PROMOTABLE TRANSACTION=promotable;")

    '                conn.Open()
    '                Dim cmd As Oracle.DataAccess.Client.OracleCommand = conn.CreateCommand()
    '                cmd.CommandText = "SELECT 1 FROM T_VAC_PROGRAMMATE"

    '                cmd.ExecuteScalar()

    '                ' dam.QB.NewQuery()
    '                ' dam.QB.AddSelectFields("1")
    '                '  dam.QB.AddTables("T_VAC_PROGRAMMATE")
    '                ' dam.ExecScalar()

    '                c += 1

    '                doquery(c, cs)

    '            End Using

    '        End If

    '        transactionScope.Complete()

    '    End Using


    'End Sub

#End Region

#Region " Private "

    Private Sub TestScritturaEventLog()

        Dim sbTest As New System.Text.StringBuilder()
        sbTest.AppendFormat("--- Test scrittura EventLog ---{0}Appid: {1} - Azienda: {2}", Environment.NewLine, OnVacContext.AppId, OnVacContext.Azienda)

        Common.Utility.EventLogHelper.EventLogWrite(sbTest.ToString(), OnVacContext.AppId)

    End Sub

#End Region

#Region "workaround"

    <Serializable> Public Class ConnForModallist
        Public CRConnectionString As String
        Public Provider As String
    End Class

#End Region

End Class
