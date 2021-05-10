Imports System.Collections.Generic
Imports Onit.Shared.NTier.Dal.DAAB
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.DAL


Public MustInherit Class BizClass
    Implements IDisposable

#Region " Events "

    'Public Event PreWriteLog(ByVal log As Log.LogWriterProvider.ILogWriterProvider, ByVal t As Log.DataLogStructure.Testata)
    'Public Event PreWriteLogGroup(ByVal log As Log.LogWriterProvider.ILogWriterProvider, ByVal t As Log.DataLogStructure.Testata())

#End Region

#Region " Fields "

    Private ReadOnly externalGenericProvider As Boolean

    Public ReadOnly ContextInfos As BizContextInfos
    Public ReadOnly LogOptions As BizLogOptions

#End Region

#Region " Properties "

    Private _Applicazione As Onit.Shared.Manager.Apps.App
    Public ReadOnly Property Applicazione() As Onit.Shared.Manager.Apps.App
        Get
            If _Applicazione Is Nothing AndAlso Not String.IsNullOrEmpty(ContextInfos.IDApplicazione) Then

                Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Suppress)

                    _Applicazione = Onit.Shared.Manager.Apps.App.getInstance(ContextInfos.IDApplicazione, ContextInfos.CodiceAzienda)

                    transactionScope.Complete()

                End Using

            End If
            Return _Applicazione
        End Get
    End Property

    ''' <summary>
    ''' Restituisce la connection info in base all'applicativo.
    ''' IdApplicativo e CodiceAzienda devono essere impostati.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ConnectionInfo() As ConnectionInfo
        Get
            Dim managerConnectionInfo As DbConnectionInfo = Applicazione.getConnectionInfo()

            Return New ConnectionInfo(Applicazione.DbmsProvider, managerConnectionInfo.ConnectionString)
        End Get
    End Property

    Public ReadOnly Property GenericProvider() As DbGenericProvider
        Get
            Return GenericProviderFactory.GetDbGenericProvider(Applicazione.Id, Applicazione.AziCodice)
        End Get
    End Property

    ' Classe contenitore dei parametri, passata dall'esterno
    Private _Settings As Settings.Settings
    Public Property Settings() As Settings.Settings
        Get
            If _Settings Is Nothing Then
                If Not GenericProvider Is Nothing Then
                    _Settings = New Settings.Settings(ContextInfos.CodiceCentroVaccinale, GenericProvider)
                End If
            End If
            Return _Settings
        End Get
        Private Set(Value As Settings.Settings)
            _Settings = Value
        End Set
    End Property

#Region " UslGestite "

    '[Unificazione Ulss]: UslGestitaAllineaSettingsProvider => OK
    Private _UslGestitaAllineaSettingsProvider As BizUslGestitaAllineaSettingsProvider
    Public Property UslGestitaAllineaSettingsProvider() As BizUslGestitaAllineaSettingsProvider
        Get
            If _UslGestitaAllineaSettingsProvider Is Nothing Then
                _UslGestitaAllineaSettingsProvider = New BizUslGestitaAllineaSettingsProvider()
            End If
            Return _UslGestitaAllineaSettingsProvider
        End Get
        Private Set(value As BizUslGestitaAllineaSettingsProvider)
            _UslGestitaAllineaSettingsProvider = value
        End Set
    End Property

    ' TODO [Unificazione Ulss]: UslGestite => quelli rimasti si riescono a sostituire?
    Private _UslGestite As ICollection(Of Usl)
    Public ReadOnly Property UslGestite() As ICollection(Of Usl)
        Get
            If _UslGestite Is Nothing Then
                _UslGestite = GenericProvider.Usl.GetUslGestite()
            End If

            Return _UslGestite
        End Get
    End Property

    ' TODO [Unificazione Ulss]: UslGestitaCorrente => quelli rimasti si riescono a sostituire?
    Private _UslGestitaCorrente As Usl
    Public ReadOnly Property UslGestitaCorrente() As Usl
        Get
            If _UslGestitaCorrente Is Nothing Then
                _UslGestitaCorrente = GetUslGestitaByIDApplicazione(ContextInfos.IDApplicazione)
            End If
            Return _UslGestitaCorrente
        End Get
    End Property

    Private _UslUnificataCorrente As UslUnificata

    ''' <summary>
    ''' Restituisce l'entity relativa alla ulss unificata con codice usl specificato nel context info.
    ''' Se non è tra le ulss unificate, e non è nemmeno la ulss generica presente nella t_usl_gestite con flag unificata uguale a true, allora restituisce null.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property UslUnificataCorrente() As UslUnificata
        Get
            If _UslUnificataCorrente Is Nothing AndAlso Not String.IsNullOrWhiteSpace(ContextInfos.CodiceUsl) Then
                _UslUnificataCorrente = GenericProvider.Usl.GetUslUnificataByCodiceUsl(ContextInfos.CodiceUsl)
            End If

            If _UslUnificataCorrente Is Nothing Then

                Dim uslGestita As Usl = UslGestite.Where(Function(p) p.Codice = ContextInfos.CodiceUsl AndAlso p.IsApplicazioneUnificata).FirstOrDefault()

                If Not uslGestita Is Nothing Then

                    _UslUnificataCorrente = New UslUnificata()
                    _UslUnificataCorrente.CodiceUsl = uslGestita.Codice
                    _UslUnificataCorrente.IdApplicazione = uslGestita.IDApplicazione

                End If

            End If

            Return _UslUnificataCorrente

        End Get
    End Property

    Private _GenericProviderFactory As DbGenericProviderFactory
    Protected ReadOnly Property GenericProviderFactory() As DbGenericProviderFactory
        Get
            If _GenericProviderFactory Is Nothing Then
                _GenericProviderFactory = New DbGenericProviderFactory()
            End If
            Return _GenericProviderFactory
        End Get
    End Property

#End Region

#Region " Centrale "

    Public ReadOnly Property IsApplicazioneCentrale() As Boolean
        Get
            Return (Not String.IsNullOrEmpty(Settings.APP_ID_CENTRALE) AndAlso ContextInfos.IDApplicazione = Settings.APP_ID_CENTRALE)
        End Get
    End Property

    Private _ApplicazioneCentrale As Onit.Shared.Manager.Apps.App
    Public ReadOnly Property ApplicazioneCentrale() As Onit.Shared.Manager.Apps.App
        Get
            If _ApplicazioneCentrale Is Nothing Then

                Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Suppress)

                    _ApplicazioneCentrale = Onit.Shared.Manager.Apps.App.getInstance(Settings.APP_ID_CENTRALE, ContextInfos.CodiceAzienda)

                    transactionScope.Complete()

                End Using

            End If
            Return _ApplicazioneCentrale
        End Get
    End Property

    Private _GenericProviderCentrale As DbGenericProvider
    Public ReadOnly Property GenericProviderCentrale() As DbGenericProvider
        Get
            If _GenericProviderCentrale Is Nothing Then
                _GenericProviderCentrale = GenericProviderFactory.GetDbGenericProvider(ApplicazioneCentrale.Id, ApplicazioneCentrale.AziCodice)
            End If
            Return _GenericProviderCentrale
        End Get
    End Property

    Private _ConnectionStringCentrale As String
    Public ReadOnly Property ConnectionStringCentrale() As String
        Get
            If String.IsNullOrEmpty(_ConnectionStringCentrale) Then
                _ConnectionStringCentrale = ApplicazioneCentrale.getConnectionInfo().ConnectionString
            End If
            Return _ConnectionStringCentrale
        End Get
    End Property

    Private _ProviderCentrale As [Shared].NTier.Dal.DAAB.Provider?
    Public ReadOnly Property ProviderCentrale() As [Shared].NTier.Dal.DAAB.Provider
        Get
            If _ProviderCentrale Is Nothing Then
                _ProviderCentrale = Me.ApplicazioneCentrale.getConnectionInfo().Provider
            End If
            Return _ProviderCentrale
        End Get
    End Property

#End Region

#Region " Error "

    Private errMsg As String
    ''' <summary>
    '''  Messaggio di errore da restituire all'utente.
    ''' </summary>
    Public Property ERRORMESSAGE() As String
        Get
            Return errMsg
        End Get
        Set(ByVal Value As String)
            errMsg = Value
        End Set
    End Property

    Private hasErr As Boolean = False
    ''' <summary>
    '''  Restituisce un valore in base all'ultimo metodo invocato nella classe
    ''' </summary>
    Public Property HASERROR() As Boolean
        Get
            Return hasErr
        End Get
        Set(ByVal Value As Boolean)
            hasErr = Value
        End Set
    End Property

#End Region

#End Region

#Region " Constructors "

    Public Sub New(contextInfos As BizContextInfos, logOptions As BizLogOptions)

        Me.LogOptions = logOptions
        Me.ContextInfos = contextInfos

    End Sub

    Public Sub New(settings As Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)

        Me.Settings = settings
        Me.LogOptions = logOptions
        Me.ContextInfos = contextInfos

    End Sub

    Public Sub New(ByRef settings As Settings.Settings, uslGestitaAllineaSettingsProvider As BizUslGestitaAllineaSettingsProvider, contextInfos As BizContextInfos, logOptions As BizLogOptions)

        Me.New(contextInfos, logOptions)
        Me.externalGenericProvider = False
        Me.Settings = settings
        Me.UslGestitaAllineaSettingsProvider = uslGestitaAllineaSettingsProvider

    End Sub

    Public Sub New(dbGenericProviderFactory As DbGenericProviderFactory, ByRef settings As Onit.OnAssistnet.OnVac.Settings.Settings, uslGestitaAllineaSettingsProvider As BizUslGestitaAllineaSettingsProvider, contextInfos As BizContextInfos, logOptions As BizLogOptions)

        Me.New(contextInfos, logOptions)

        If dbGenericProviderFactory Is Nothing Then
            Throw New ApplicationException("dbGenericProviderFactory is nothing")
        Else
            Me.externalGenericProvider = (Not dbGenericProviderFactory Is Nothing)
            Me._GenericProviderFactory = dbGenericProviderFactory
            Me.Settings = settings
            Me.UslGestitaAllineaSettingsProvider = uslGestitaAllineaSettingsProvider
        End If

    End Sub

    Public Sub New(genericProvider As DbGenericProvider, ByRef settings As Settings.Settings, uslGestitaAllineaSettingsProvider As BizUslGestitaAllineaSettingsProvider, contextInfos As BizContextInfos, logOptions As BizLogOptions)

        Me.New(contextInfos, logOptions)

        If genericProvider Is Nothing Then
            Throw New ApplicationException("genericprovider is nothing")
        Else
            Me.GenericProviderFactory.AddDbGenericProvider(contextInfos.IDApplicazione, contextInfos.CodiceAzienda, genericProvider)
            Me.Settings = settings
            Me.UslGestitaAllineaSettingsProvider = uslGestitaAllineaSettingsProvider
        End If

    End Sub

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)

        Me.New(genericprovider, settings, Nothing, contextInfos, logOptions)

    End Sub

#End Region

#Region " Methods "

#Region " UslGestite "

    ' [Unificazione Ulss]: CreateBizContextInfosByCodiceUslGestita => OK
    Protected Function CreateBizContextInfosByCodiceUslGestita(codiceUslGestita As String) As BizContextInfos

        Return New BizContextInfos(ContextInfos.IDUtente, ContextInfos.CodiceAzienda, GetIDApplicazioneByCodiceUslGestita(codiceUslGestita), String.Empty, codiceUslGestita, Nothing)

    End Function

    ' [Unificazione Ulss]: GetUslGestitaByIDApplicazione => OK
    Protected Function GetUslGestitaByIDApplicazione(idApplicazione As String) As Usl

        Return UslGestite.FirstOrDefault(Function(u) u.IDApplicazione.Equals(idApplicazione))

    End Function

    ' [Unificazione Ulss]: GetUslGestitaByCodiceUsl => OK
    Protected Function GetUslGestitaByCodiceUsl(codiceUslGestita As String) As Usl

        Return UslGestite.FirstOrDefault(Function(u) (u.Codice = codiceUslGestita))

    End Function

    ' [Unificazione Ulss]: GetDBGenericProviderByCodiceUslGestita => OK
    Protected Function GetDBGenericProviderByCodiceUslGestita(codiceUsl As String) As DbGenericProvider

        Dim idApplicazione As String = GetIDApplicazioneByCodiceUslGestita(codiceUsl)

        If Not String.IsNullOrEmpty(idApplicazione) Then
            Return GetDBGenericProviderByIDApplicazione(idApplicazione)
        End If

        Return Nothing

    End Function

    ' [Unificazione Ulss]: GetDBGenericProviderByIDApplicazione => OK
    Protected Function GetDBGenericProviderByIDApplicazione(idApplicazione As String) As DbGenericProvider

        Dim dbGenericProvider As DbGenericProvider = Nothing

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Suppress)

            Dim app As Onit.Shared.Manager.Apps.App = Onit.Shared.Manager.Apps.App.getInstance(idApplicazione, ContextInfos.CodiceAzienda)

            dbGenericProvider = New DbGenericProvider(New ConnectionInfo(app.DbmsProvider, app.getConnectionInfo.ConnectionString))

            transactionScope.Complete()

        End Using

        Return dbGenericProvider

    End Function

    ' [Unificazione Ulss]: GetIDApplicazioneByCodiceUslGestita => OK
    Public Function GetIDApplicazioneByCodiceUslGestita(codiceUsl As String) As String

        Dim uslGestita As Usl = GetUslGestitaByCodiceUsl(codiceUsl)

        If uslGestita Is Nothing Then Return Nothing

        Return uslGestita.IDApplicazione

    End Function

    Public Function GetCodiceUslGestitaByIDApplicazione(idApplicazione As String) As String

        Dim uslGestita As Usl = GetUslGestitaByIDApplicazione(idApplicazione)

        If uslGestita Is Nothing Then Return String.Empty

        Return uslGestita.Codice

    End Function

    ''' <summary>
    ''' Restituisce il codice della usl in base all'app id specificato. 
    ''' Prima cerca tra le usl unificate, se non lo trova lo prende dalle usl gestite.
    ''' </summary>
    ''' <param name="idApplicazione"></param>
    ''' <returns></returns>
    Public Function GetCodiceUslByIdApplicazione(idApplicazione As String) As String

        Dim codiceUsl As String

        Dim uslUnificata As UslUnificata = GenericProvider.Usl.GetUslUnificataByIdApplicazione(idApplicazione)

        If Not uslUnificata Is Nothing Then
            codiceUsl = uslUnificata.CodiceUsl
        Else
            codiceUsl = GetCodiceUslGestitaByIDApplicazione(idApplicazione)
        End If

        Return codiceUsl

    End Function

    ''' <summary>
    ''' Restituisce l'app id in base al codice usl specificato.
    ''' Prima cerca tra le usl unificate, se non lo trova lo prende dalle usl gestite.
    ''' </summary>
    ''' <param name="codiceUsl"></param>
    ''' <returns></returns>
    Public Function GetIdApplicazioneByCodiceUsl(codiceUsl As String) As String

        Dim idApplicazione As String

        Dim uslUnificata As UslUnificata = GenericProvider.Usl.GetUslUnificataByCodiceUsl(codiceUsl)

        If Not uslUnificata Is Nothing Then
            idApplicazione = uslUnificata.IdApplicazione
        Else
            idApplicazione = GetIDApplicazioneByCodiceUslGestita(codiceUsl)
        End If

        Return idApplicazione

    End Function

    Protected Function CreateBizContextInfosByCodiceUsl(codiceUsl As String) As BizContextInfos

        Return New BizContextInfos(ContextInfos.IDUtente, ContextInfos.CodiceAzienda, GetIdApplicazioneByCodiceUsl(codiceUsl), String.Empty, codiceUsl, Nothing)

    End Function

#End Region

#Region " Log "

    Protected Sub WriteLog(testata As Log.DataLogStructure.Testata)

        Me.WriteLog(New Log.DataLogStructure.Testata() {testata})

    End Sub

    Protected Sub WriteLog(testate As IEnumerable(Of Log.DataLogStructure.Testata))

        Dim connectionInfo As DbConnectionInfo = Me.Applicazione.getConnectionInfo()

        BizClass.WriteLog(testate, False, connectionInfo.ConnectionString, connectionInfo.Provider)

    End Sub

    Public Shared Sub WriteLog(testate As IEnumerable(Of Log.DataLogStructure.Testata), forzaGruppo As Boolean, connectionInfo As Onit.OnAssistnet.OnVac.ConnectionInfo)

        Dim provider As String = String.Empty

        Select Case connectionInfo.Provider.ToUpper()

            Case "ORACLE"
                provider = BizClass.GetLogDBDataProvider()

            Case Else
                Throw New NotSupportedException(String.Format("LogProvider non supportato: {0}", connectionInfo.Provider))

        End Select

        Log.LogBox.WriteData(testate.ToArray(), forzaGruppo, connectionInfo.ConnectionString, provider)

    End Sub

    Public Shared Sub WriteLog(testate As IEnumerable(Of Log.DataLogStructure.Testata), forzaGruppo As Boolean, logConnectionString As String, logProvider As Onit.Shared.NTier.Dal.DAAB.Provider)

        Dim logProviderString As String

        Select Case logProvider

            Case [Shared].NTier.Dal.DAAB.Provider.ORACLE
                logProviderString = BizClass.GetLogDBDataProvider()

            Case Else
                Throw New NotSupportedException(String.Format("LogProvider non supportato: {0}", logProvider))

        End Select

        Log.LogBox.WriteData(testate.ToArray(), forzaGruppo, logConnectionString, logProviderString)

    End Sub

    Private Shared Function GetLogDBDataProvider() As String

        Dim logWriter As Onit.OnAssistnet.OnVac.Log.LogWriterProvider.ILogWriterProvider = Log.LogBox.LogWritersProvider("LogDB")
        If logWriter Is Nothing Then
            Throw New ArgumentNullException("Configurazione OnVac.Log errata: provider 'LogDB' mancante.")
        End If

        Dim dbLogWriter As Onit.OnAssistnet.OnVac.Log.LogWriterProvider.DbLogWriterProvider =
            DirectCast(logWriter, Onit.OnAssistnet.OnVac.Log.LogWriterProvider.DbLogWriterProvider)

        If String.IsNullOrWhiteSpace(dbLogWriter.DataProvider) Then
            Throw New ArgumentNullException("Configurazione OnVac.Log errata: parametro 'DataProvider' del provider 'LogDB' mancante o non valorizzato.")
        End If

        Return dbLogWriter.DataProvider

    End Function

#End Region

#Region " Transaction Scope "

    Public Function GetReadCommittedTransactionOptions() As Transactions.TransactionOptions

        Dim transactionOptions As New Transactions.TransactionOptions()
        transactionOptions.IsolationLevel = Transactions.IsolationLevel.ReadCommitted

        Return transactionOptions

    End Function

#End Region

#Region " Factory "

    'Friend Function CreateBizVisiteByUslGestita(codiceUsl As String) As BizVisite
    '    Return Me.CreateBizVisiteByUslGestita(codiceUsl, Me.GetDBGenericProviderByCodiceUslGestita(codiceUsl))
    'End Function

    'Friend Function CreateBizVisiteByUslGestita(codiceUsl As String, dbGenericProvider As DbGenericProvider, setting As Settings.Settings) As BizVisite

    '    'Dim settings As Settings.Settings = Me.UslGestitaAllineaSettingsProvider.GetSettings(dbGenericProvider)

    '    Dim bizUslGestitaContextInfos As BizContextInfos = Me.CreateBizContextInfosByCodiceUslGestita(codiceUsl)

    '    Return New BizVisite(dbGenericProvider, settings, bizUslGestitaContextInfos, Me.LogOptions)

    'End Function

    ''Friend Function CreateBizVaccinazioniEscluseByUslGestita(codiceUsl As String, setting As Settings.Settings) As BizVaccinazioniEscluse
    ''    Return Me.CreateBizVaccinazioniEscluseByUslGestita(codiceUsl, Me.GetDBGenericProviderByCodiceUslGestita(codiceUsl))
    ''End Function

    'Friend Function CreateBizVaccinazioniEscluseByUslGestita(codiceUsl As String, dbGenericProvider As DbGenericProvider, setting As Settings.Settings) As BizVaccinazioniEscluse

    '    'Dim settings As Settings.Settings = Me.UslGestitaAllineaSettingsProvider.GetSettings(dbGenericProvider)
    '    Dim bizUslGestitaContextInfos As BizContextInfos = Me.CreateBizContextInfosByCodiceUslGestita(codiceUsl)

    '    Return New BizVaccinazioniEscluse(dbGenericProvider, settings, bizUslGestitaContextInfos, Me.LogOptions)

    'End Function

    ''Friend Function CreateBizVaccinazioniEseguiteByUslGestita(codiceUsl As String, setting As Settings.Settings) As BizVaccinazioniEseguite
    ''    Return Me.CreateBizVaccinazioniEseguiteByUslGestita(codiceUsl, Me.GetDBGenericProviderByCodiceUslGestita(codiceUsl))
    ''End Function


    ''Friend Function CreateBizVaccinazioniEseguiteByUslGestita(codiceUsl As String, setting As Settings.Settings) As BizVaccinazioniEseguite
    ''    Return Me.CreateBizVaccinazioniEseguiteByUslGestita(codiceUsl, Me.GetDBGenericProviderByCodiceUslGestita(codiceUsl))
    ''End Function

    'Friend Function CreateBizVaccinazioniEseguiteByUslGestita(codiceUsl As String, dbGenericProvider As DbGenericProvider, setting As Settings.Settings) As BizVaccinazioniEseguite

    '    ' Dim settings As Settings.Settings = Me.UslGestitaAllineaSettingsProvider.GetSettings(dbGenericProvider)
    '    Dim bizUslGestitaContextInfos As BizContextInfos = Me.CreateBizContextInfosByCodiceUslGestita(codiceUsl)

    '    Return New BizVaccinazioniEseguite(dbGenericProvider, settings, bizUslGestitaContextInfos, Me.LogOptions)

    'End Function

#End Region

#Region " Controlli "

    Protected Function IsCodicePazienteEmpty(codicePaziente As String) As Boolean

        If String.IsNullOrEmpty(codicePaziente) Then Return True

        If codicePaziente < 0 Then Return True

        Return False

    End Function

#Region " Classe e metodi per controllo campi "

    Public Class CheckResult(Of T)

        Public Success As Boolean
        Public ListaCampiObbligatoriNonCompilati As List(Of Entities.FieldToCheck(Of T))
        Public ListaCampiLunghezzaMassimaSuperata As List(Of Entities.FieldToCheck(Of T))

        Public Sub New()
        End Sub

        Public Sub New(success As Boolean, listaObbligatori As List(Of Entities.FieldToCheck(Of T)), listaMaxLength As List(Of Entities.FieldToCheck(Of T)))
            Me.Success = success
            Me.ListaCampiObbligatoriNonCompilati = listaObbligatori
            Me.ListaCampiLunghezzaMassimaSuperata = listaMaxLength
        End Sub

    End Class

    Public Shared Function CheckFields(Of T)(fieldList As List(Of Entities.FieldToCheck(Of T))) As CheckResult(Of T)

        If fieldList.IsNullOrEmpty() Then Return New CheckResult(Of T)(True, Nothing, Nothing)

        Dim result As New CheckResult(Of T)()
        result.Success = True

        ' Obbligatorietà
        Dim listaCampiObbligatoriMancanti As List(Of Entities.FieldToCheck(Of T)) =
            fieldList.Where(Function(p) p.Required AndAlso Not CheckRequiredValue(p.Value)).ToList()

        If Not listaCampiObbligatoriMancanti Is Nothing AndAlso listaCampiObbligatoriMancanti.Count > 0 Then result.Success = False

        ' Lunghezza
        Dim listaCampiLunghezzaMassimaSuperata As List(Of Entities.FieldToCheck(Of T)) =
            fieldList.Where(Function(p) p.MaxLength > 0 AndAlso p.Value.ToString().Length > p.MaxLength).ToList()

        If Not listaCampiLunghezzaMassimaSuperata Is Nothing AndAlso listaCampiLunghezzaMassimaSuperata.Count > 0 Then result.Success = False

        result.ListaCampiObbligatoriNonCompilati = listaCampiObbligatoriMancanti
        result.ListaCampiLunghezzaMassimaSuperata = listaCampiLunghezzaMassimaSuperata

        Return result

    End Function

    Private Shared Function CheckRequiredValue(value As Object) As Boolean

        If value Is Nothing Then Return False

        Dim result As Boolean = True

        Select Case value.GetType()
            Case GetType(String)
                result = Not String.IsNullOrEmpty(value.ToString())
            Case GetType(Integer)
                result = True
            Case GetType(Integer?)
                result = Not DirectCast(value, Integer?).HasValue
            Case GetType(Double)
                result = True
            Case GetType(Double?)
                result = Not DirectCast(value, Double?).HasValue
            Case GetType(DateTime)
                result = Convert.ToDateTime(value) > DateTime.MinValue
            Case GetType(DateTime?)
                Dim nullableDateTimeValue As DateTime? = DirectCast(value, DateTime?)
                If Not nullableDateTimeValue.HasValue Then
                    result = False
                Else
                    result = nullableDateTimeValue > DateTime.MinValue
                End If
            Case Else
                ' ...
        End Select

        Return result

    End Function

#End Region

#End Region

    Public Sub AddParameter(rptDocument As CrystalDecisions.CrystalReports.Engine.ReportDocument, nome As String, valore As String)

        Dim paramField As CrystalDecisions.CrystalReports.Engine.ParameterFieldDefinition = rptDocument.DataDefinition.ParameterFields(nome)

        Dim discreteParam As New CrystalDecisions.Shared.ParameterDiscreteValue()

        discreteParam.Value = valore

        Dim defaultvalues As CrystalDecisions.Shared.ParameterValues = paramField.DefaultValues
        Dim currentValues As CrystalDecisions.Shared.ParameterValues = paramField.CurrentValues

        defaultvalues.Add(discreteParam)
        currentValues.Add(discreteParam)

        paramField.ApplyDefaultValues(defaultvalues)
        paramField.ApplyCurrentValues(defaultvalues)

    End Sub

    Public Function SerializeEntity(Of T)(entity As T) As String

        If entity Is Nothing Then Return String.Empty

        Dim text As String = String.Empty

        Try
            Using writer As New System.IO.StringWriter()

                Dim xmlSerializer As New System.Xml.Serialization.XmlSerializer(GetType(T))
                xmlSerializer.Serialize(New System.Xml.XmlTextWriter(writer), entity)

                text = writer.ToString()

            End Using
        Catch ex As Exception
            text = "Serializzazione non effettuata"
        End Try

        Return text

    End Function

#End Region

#Region " IDisposable "

    Private disposedValue As Boolean = False        ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then

                If Not externalGenericProvider Then
                    If Not Me.GenericProviderFactory Is Nothing Then
                        Me.GenericProviderFactory.Dispose()
                    End If

                End If

            End If

            ' free your own state (unmanaged objects).

            ' set large fields to null.

            Me.disposedValue = True
        End If
    End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Protected Overrides Sub Finalize()
        ' Simply call Dispose(False).
        Dispose(False)
    End Sub

#End Region

#Region " Types "

    Public Class BizUslGestitaAllineaSettingsProvider

        Public Overridable Function GetSettings(dbGenericProvider As DbGenericProvider) As Settings.Settings

            Return OnVac.Settings.Settings.GetSettingsAllineaInterno(dbGenericProvider)

        End Function

    End Class

    Public MustInherit Class SalvaCommandBase

        Public Property Operation As SalvaCommandOperation

    End Class

    Public Enum SalvaCommandOperation
        Insert
        Delete
        Update
    End Enum

    Public Class RicercaCommand(Of T As Structure)

        Public FiltroGenerico As String
        Public FiltroSoloValidi As Boolean
        Public CampoOrdinamento As T?
        Public IsDescending As Boolean
        Public Offset As Int32
        Public Size As Int32

    End Class

#End Region

End Class