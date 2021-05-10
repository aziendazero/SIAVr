Imports System.Reflection

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.DAL.Oracle


Namespace DAL

    '''<summary>
    ''' Questa classe replica le varie classi di DAL come proprietà. Al momento viene
    ''' restituito sempre e solo il provider per Oracle, pur lavorando con le interfacce
    '''</summary>
    Public Class DbGenericProvider
        Implements IDisposable

#Region " Private "

        Private _ContattiScolasticiCovid As IContattiScolasticiCovidProvider
        Private _BilancioProgrammato As IBilancioProgrammatoProvider
        Private _AnaBilancio As IAnaBilancioProvider
        Private _Anagrafiche As IAnagraficheProvider
        Private _AnaVaccinazioni As IAnaVaccinazioniProvider
        Private _Convocazione As IConvocazioneProvider
        Private _Copertura As ICoperturaProvider
        Private _Paziente As IPazienteProvider
        Private _tagDal As ITagDal
        Private _RilevazioniCovid19 As IRilevazioniCovid19Provider
        Private _Ricoveri As IDbRicoveriProvider
        Private _Covid19Tamponi As ICovid19Tamponi
        Private _Covid19TestSierologici As ICovid19TestSierologici
        Private _Covid19Viaggiatori As ICovid19Viaggiatori
        Private _Medico As IMediciProvider
        Private _VaccinazioneProg As IVaccinazioneProgrammataProvider
        Private _sollecitiBilanciProvider As ISollecitiBilanciProvider
        Private _Report As IReportProvider
        Private _AppuntamentiGiorno As IAppuntamentiGiornoProvider
        Private _ElencoBilanci As IElencoBilanciProvider
        Private _Prenotazioni As IPrenotazioniProvider
        Private _Codifiche As ICodificheProvider
        Private _Comuni As IComuniProvider
        Private _AliasPazienti As IAliasProvider
        Private _Postazioni As IPostazioniProvider
        Private _Consultori As IConsultoriProvider
        Private _StatiAnagrafici As IStatiAnagraficiProvider
        Private _Procedure As IProcedureProvider
        Private _Usl As IUslProvider
        Private _PersonaleScolastico As IPersonaleScolastico
        Private _Utenti As IUtentiProvider
        Private _Parametri As IParametriProvider
        Private _Eseguite As IVaccinazioniEseguiteProvider
        Private _PazientiVaccinazioni As IPazientiVaccinazioniProvider
        Private _Escluse As IVaccinazioniEscluseProvider
        Private _AssociabilitaVaccinazioni As IAssociabilitaVaccinazioniProvider
        Private _Associazioni As IAssociazioniProvider
        Private _Cicli As ICicliProvider
        Private _Progressivi As IProgressiviProvider
        Private _MovimentiEsterniCNS As IMovimentiEsterniCNSProvider
        Private _MovimentiInterniCNS As IMovimentiInterniCNSProvider
        'Private _Consenso As IConsensoProvider
        Private _Visite As IVisiteProvider
        Private _Log As ILogProvider
        Private _Lotti As ILottiProvider
        Private _ElaborazioneAcn As IElaborazioneACNProvider
        Private _ElaborazionePaziente As IElaborazionePazienteProvider
        Private _ElaborazioneControllo As IElaborazioneControlliProvider
        Private _EsitoControlloScuole As IEsitoControlloScuoleProvider
        Private _Via As IViaProvider

        Private _ConfigurazioneCertificato As IConfigurazioneCertificatoProvider
        Private _Distretto As IDistrettoProvider
        Private _Cittadinanze As ICittadinanzeProvider
        Private _CategorieRischio As ICategorieRischioProvider
        Private _Malattie As IMalattieProvider
        Private _MotiviEsclusione As IMotiviEsclusioneProvider
        Private _NomiCommerciali As INomiCommercialiProvider
        Private _Operatori As IOperatoriProvider
        Private _StampaLibrettoVaccinale As IStampaLibrettoVaccinale
        Private _StampaElencoEsclusione As IStampaElencoEsclusione
        Private _StampaConteggioSedute As IStampaConteggioSedute
        Private _Installazioni As IInstallazioniProvider
        Private _CancellazioneConvocazioni As ICancellazioneConvocazioniProvider
        Private _Attivita As IAttivitaProvider

        Private _AliasCentralePazienti As IAliasCentraleProvider
        Private _VaccinazioneEseguitaCentrale As IVaccinazioneEseguitaCentraleProvider
        Private _VaccinazioneEsclusaCentrale As IVaccinazioneEsclusaCentraleProvider
        Private _VisitaCentrale As IVisitaCentraleProvider
        Private _FirmaDigitale As IFirmaDigitaleProvider
        Private _Interventi As IInterventiProvider
        Private _SitiInoculazione As ISitiInoculazioneProvider
        Private _VieSomministrazione As IVieSomministrazioneProvider
        Private _VaccinazioniAssociate As IVaccinazioniAssociateProvider
        Private _CodiciStruttura As ICodiciStrutturaProvider
        Private _LuoghiEsecuzioneVaccinazioni As ILuoghiEsecuzioneVaccinazioniProvider
        Private _EventiNotifiche As IEventiNotificaProvider
        Private _TipiErogatori As ITipiErogatoriProvider
        Private _Documenti As IDocumentiProvider

        Private _hasErr As Boolean = False

        Private _DAM As IDAM

        ' Utilizzata nel metodo Dispose per sapere se effettuare la dispose del dam e della connection
        Private _disposeObjects As Boolean = True
        Private _lastCaller As String
        Private _LogNotifiche As ILogNotificheProvider

#End Region

        Private Sub SetLastCaller()
#If DEBUG Then
            Dim frame As StackFrame = New StackFrame(2)
            Dim method As MethodBase = frame.GetMethod()
            _lastCaller = String.Format("{0}.{1}", method.DeclaringType.FullName, method.Name)
#End If
        End Sub

#Region " Properties "

        Public ReadOnly Property DAM As IDAM
            Get
                Return Me._DAM
            End Get
        End Property

        Private _provider As String
        Public ReadOnly Property Provider() As String
            Get
                Return Me._provider
            End Get
        End Property

        Private _connection As IDbConnection
        Public ReadOnly Property Connection() As System.Data.IDbConnection
            Get
                Return Me._connection
            End Get
        End Property

        Private _transaction As IDbTransaction
        Public ReadOnly Property Transaction() As System.Data.IDbTransaction
            Get
                Return Me._transaction
            End Get
        End Property

        ''' <summary>
        '''  Messaggio di errore da restituire all'utente.
        ''' </summary>
        Private _dbError As New DbError()
        Public ReadOnly Property DbError() As DbError
            Get
                Return Me._dbError
            End Get
        End Property

#End Region

#Region " Costruttori "

        ''' <summary>Costruttore di un oggetto di tipo provider di connessione al database</summary>
        ''' <param name="damEsterno">DAM proveniente dall'esterno per continuare le transazioni 
        ''' già iniziate. Se viene passato Nothing il dam è interno </param>
        ''' <remarks> E' necessario chiamare il metodo dispose di questo oggetto se il DAM è interno </remarks>
        Public Sub New(ByRef damEsterno As IDAM)

            If damEsterno Is Nothing Then Throw New Exception("DAL: IDAM is Nothing")

            Me._DAM = damEsterno

            ' dam esterno, no dispose
            Me._disposeObjects = False

            ' Connessione e transazione prese dal dam
            Me._provider = _DAM.Provider
            Me._connection = _DAM.Connection
            Me._transaction = _DAM.Transaction

        End Sub

        ''' <summary>Costruttore di un oggetto di tipo provider di connessione al database</summary>
        ''' <param name="c">Oggetto ConnectionInfo contenente provider e stringa di connessione</param>
        ''' <remarks>La connessione viene creata e aperta. E' necessario chiamare esplicitamente il metodo dispose di questo oggetto</remarks>
        Public Sub New(connectionInfo As ConnectionInfo)

            If connectionInfo Is Nothing Then Throw New Exception("DAL: ConnectionInfo is Nothing")

            ' DAM creato in base alla connessione del ConnectionInfo
            Me._DAM = DAMBuilder.CreateDAM(connectionInfo.Provider, connectionInfo.ConnectionString)

            ' dam e connessione interni (verrà effettuata la dispose)
            Me._disposeObjects = True

            ' Connessione e transazione prese dal dam
            Me._provider = _DAM.Provider
            Me._connection = _DAM.Connection
            Me._transaction = _DAM.Transaction

        End Sub

        ''' <summary>Costruttore di un oggetto di tipo provider di connessione al database</summary>
        ''' <param name="conn">Connessione al database</param>
        ''' <param name="tx">Transazione</param>
        ''' <remarks> E' necessario chiamare esplicitamente il metodo dispose di questo oggetto</remarks>
        Public Sub New(provider As String, ByRef connection As IDbConnection, ByRef transaction As IDbTransaction)

            Me._provider = provider
            Me._connection = connection
            Me._transaction = transaction

            ' Creo il dam in base alla connection e alla transaction passate dall'esterno
            Me._DAM = DAMBuilder.CreateDAM(Me._connection, Me._transaction)

            ' dam interno, ma creato con connection e transaction esterne (no dispose)
            Me._disposeObjects = False

        End Sub


#End Region

#Region " Transactions "

        Public Sub BeginTransaction()

            Me._DAM.BeginTrans()
            Me._transaction = Me._DAM.Transaction

        End Sub

        Public Sub BeginTransaction(isolationLevel As IsolationLevel)

            Me._DAM.BeginTrans(isolationLevel)
            Me._transaction = Me._DAM.Transaction

        End Sub

        Public Sub Commit()

            Me._DAM.Commit()

        End Sub

        Public Sub Rollback()

            Me._DAM.Rollback()

        End Sub

#End Region

#Region " Providers Pubblici "

        Public ReadOnly Property BilancioProgrammato() As DbBilancioProgrammatoProvider
            Get
                SetLastCaller()
                If _BilancioProgrammato Is Nothing Then _BilancioProgrammato = New DbBilancioProgrammatoProvider(_DAM)
                Return _BilancioProgrammato
            End Get
        End Property

        Public ReadOnly Property AnaBilancio() As DbAnaBilancioProvider
            Get
                SetLastCaller()
                If _AnaBilancio Is Nothing Then _AnaBilancio = New DbAnaBilancioProvider(_DAM)
                Return _AnaBilancio
            End Get
        End Property
        Public ReadOnly Property Anagrafiche() As DbAnagraficheProvider
            Get
                SetLastCaller()
                If _Anagrafiche Is Nothing Then _Anagrafiche = New DbAnagraficheProvider(_DAM)
                Return _Anagrafiche
            End Get
        End Property

        Public ReadOnly Property AnaVaccinazioni() As DbAnaVaccinazioniProvider
            Get
                SetLastCaller()
                If _AnaVaccinazioni Is Nothing Then _AnaVaccinazioni = New DbAnaVaccinazioniProvider(_DAM)
                Return _AnaVaccinazioni
            End Get
        End Property

        Public ReadOnly Property SollecitiBilanci() As DbSollecitiBilanciProvider
            Get
                SetLastCaller()
                If _sollecitiBilanciProvider Is Nothing Then _sollecitiBilanciProvider = New DbSollecitiBilanciProvider(_DAM)
                Return _sollecitiBilanciProvider
            End Get
        End Property

        Public ReadOnly Property Convocazione() As DbConvocazioneProvider
            Get
                SetLastCaller()
                If _Convocazione Is Nothing Then _Convocazione = New DbConvocazioneProvider(_DAM)
                Return _Convocazione
            End Get
        End Property

        Public ReadOnly Property Copertura() As DbCoperturaProvider
            Get
                SetLastCaller()
                If _Copertura Is Nothing Then _Copertura = New DbCoperturaProvider(_DAM)
                Return _Copertura
            End Get
        End Property

        Public ReadOnly Property Paziente() As DbPazienteProvider
            Get
                SetLastCaller()
                If _Paziente Is Nothing Then
                    _Paziente = New DbPazienteProvider(_DAM)
                End If
                Return _Paziente
            End Get
        End Property

        Public ReadOnly Property Tag() As ITagDal
            Get
                SetLastCaller()
                If _tagDal Is Nothing Then
                    _tagDal = New DbTag(_DAM)
                End If
                Return _tagDal
            End Get
        End Property

        Public ReadOnly Property Ricoveri() As IDbRicoveriProvider
            Get
                SetLastCaller()
                If _Ricoveri Is Nothing Then
                    _Ricoveri = New DbRicoveriProvider(_DAM)
                End If
                Return _Ricoveri
            End Get
        End Property

        Public ReadOnly Property RilevazioniCovid19() As IRilevazioniCovid19Provider
            Get
                SetLastCaller()
                If _RilevazioniCovid19 Is Nothing Then
                    _RilevazioniCovid19 = New DbRilevazioniCovid19Provider(_DAM)
                End If
                Return _RilevazioniCovid19
            End Get
        End Property
        Public ReadOnly Property Covid19Tamponi() As ICovid19Tamponi
            Get
                SetLastCaller()
                If _Covid19Tamponi Is Nothing Then
                    _Covid19Tamponi = New DbCovid19Tamponi(_DAM)
                End If
                Return _Covid19Tamponi
            End Get
        End Property
        Public ReadOnly Property Covid19TestSierologici() As DbCovid19TestSierologici
            Get
                SetLastCaller()
                If _Covid19TestSierologici Is Nothing Then
                    _Covid19TestSierologici = New DbCovid19TestSierologici(_DAM)
                End If
                Return _Covid19TestSierologici
            End Get
        End Property
        Public ReadOnly Property Covid19Viaggiatori() As DbCovid19Viaggiatori
            Get
                SetLastCaller()
                If _Covid19Viaggiatori Is Nothing Then
                    _Covid19Viaggiatori = New DbCovid19Viaggiatori(_DAM)
                End If
                Return _Covid19Viaggiatori
            End Get
        End Property

        Public ReadOnly Property Medico() As DbMediciProvider
            Get
                SetLastCaller()
                If _Medico Is Nothing Then _Medico = New DbMediciProvider(_DAM)
                Return _Medico
            End Get
        End Property

        Public ReadOnly Property VaccinazioneProg() As DbVaccinazioneProgrammataProvider
            Get
                SetLastCaller()
                If _VaccinazioneProg Is Nothing Then
                    _VaccinazioneProg = New DbVaccinazioneProgrammataProvider(_DAM)
                End If
                Return _VaccinazioneProg
            End Get
        End Property

        Public ReadOnly Property Report() As DbReportProvider
            Get
                SetLastCaller()
                If _Report Is Nothing Then _Report = New DbReportProvider(_DAM)
                Return _Report
            End Get
        End Property

        Public ReadOnly Property AppuntamentiGiorno() As DbAppuntamentiGiornoProvider
            Get
                SetLastCaller()
                If _AppuntamentiGiorno Is Nothing Then _AppuntamentiGiorno = New DbAppuntamentiGiornoProvider(_DAM)
                Return _AppuntamentiGiorno
            End Get
        End Property

        Public ReadOnly Property ElencoBilanci() As DbElencoBilanciProvider
            Get
                SetLastCaller()
                If _ElencoBilanci Is Nothing Then _ElencoBilanci = New DbElencoBilanciProvider(_DAM)
                Return _ElencoBilanci
            End Get
        End Property

        Public ReadOnly Property Prenotazioni() As DbPrenotazioniProvider
            Get
                SetLastCaller()
                If _Prenotazioni Is Nothing Then _Prenotazioni = New DbPrenotazioniProvider(_DAM)
                Return _Prenotazioni
            End Get
        End Property

        Public ReadOnly Property Codifiche() As DbCodificheProvider
            Get
                SetLastCaller()
                If _Codifiche Is Nothing Then _Codifiche = New DbCodificheProvider(_DAM)
                Return _Codifiche
            End Get
        End Property

        Public ReadOnly Property Comuni() As DbComuniProvider
            Get
                SetLastCaller()
                If _Comuni Is Nothing Then _Comuni = New DbComuniProvider(_DAM)
                Return _Comuni
            End Get
        End Property

        Public ReadOnly Property AliasPazienti() As DbAliasProvider
            Get
                SetLastCaller()
                If _AliasPazienti Is Nothing Then _AliasPazienti = New DbAliasProvider(_DAM)
                Return _AliasPazienti
            End Get
        End Property

        Public ReadOnly Property Postazioni() As DbPostazioniProvider
            Get
                SetLastCaller()
                If _Postazioni Is Nothing Then _Postazioni = New DbPostazioniProvider(_DAM)
                Return _Postazioni
            End Get
        End Property

        Public ReadOnly Property Consultori() As DbConsultoriProvider
            Get
                SetLastCaller()
                If _Consultori Is Nothing Then _Consultori = New DbConsultoriProvider(_DAM)
                Return _Consultori
            End Get
        End Property

        Public ReadOnly Property StatiAnagrafici() As DbStatiAnagraficiProvider
            Get
                SetLastCaller()
                If _StatiAnagrafici Is Nothing Then _StatiAnagrafici = New DbStatiAnagraficiProvider(_DAM)
                Return _StatiAnagrafici
            End Get
        End Property

        Public ReadOnly Property Procedure() As DbProcedureProvider
            Get
                SetLastCaller()
                If _Procedure Is Nothing Then _Procedure = New DbProcedureProvider(_DAM)
                Return _Procedure
            End Get
        End Property

        Public ReadOnly Property Usl() As DbUslProvider
            Get
                SetLastCaller()
                If _Usl Is Nothing Then _Usl = New DbUslProvider(_DAM)
                Return _Usl
            End Get
        End Property
        Public ReadOnly Property PersonaleScolastico() As DBPersonaleScolasticoProvider
            Get
                SetLastCaller()
                If _PersonaleScolastico Is Nothing Then _PersonaleScolastico = New DBPersonaleScolasticoProvider(_DAM)
                Return _PersonaleScolastico
            End Get
        End Property

        Public ReadOnly Property Utenti() As DbUtentiProvider
            Get
                SetLastCaller()
                If _Utenti Is Nothing Then _Utenti = New DbUtentiProvider(_DAM)
                Return _Utenti
            End Get
        End Property

        Public ReadOnly Property Parametri() As DbParametriProvider
            Get
                SetLastCaller()
                If _Parametri Is Nothing Then _Parametri = New DbParametriProvider(_DAM)
                Return _Parametri
            End Get
        End Property

        Public ReadOnly Property VaccinazioniEseguite() As DBVaccinazioniEseguiteProvider
            Get
                SetLastCaller()
                If _Eseguite Is Nothing Then _Eseguite = New DBVaccinazioniEseguiteProvider(_DAM)
                Return _Eseguite
            End Get
        End Property

        Public ReadOnly Property PazientiVaccinazioni() As DbPazientiVaccinazioniProvider
            Get
                SetLastCaller()
                If _PazientiVaccinazioni Is Nothing Then _PazientiVaccinazioni = New DbPazientiVaccinazioniProvider(_DAM)
                Return _PazientiVaccinazioni
            End Get
        End Property

        Public ReadOnly Property VaccinazioniEscluse() As DBVaccinazioniEscluseProvider
            Get
                SetLastCaller()
                If _Escluse Is Nothing Then
                    _Escluse = New DBVaccinazioniEscluseProvider(_DAM)
                End If
                Return _Escluse
            End Get
        End Property

        Public ReadOnly Property AssociabilitaVaccinazioni() As DbAssociabilitaVaccinazioniProvider
            Get
                SetLastCaller()
                If _AssociabilitaVaccinazioni Is Nothing Then
                    _AssociabilitaVaccinazioni = New DbAssociabilitaVaccinazioniProvider(_DAM)
                End If
                Return _AssociabilitaVaccinazioni
            End Get
        End Property

        Public ReadOnly Property Associazioni() As DbAssociazioniProvider
            Get
                SetLastCaller()
                If _Associazioni Is Nothing Then
                    _Associazioni = New DbAssociazioniProvider(_DAM)
                End If
                Return _Associazioni
            End Get
        End Property

        Public ReadOnly Property Cicli() As DbCicliProvider
            Get
                SetLastCaller()
                If _Cicli Is Nothing Then
                    _Cicli = New DbCicliProvider(_DAM)
                End If
                Return _Cicli
            End Get
        End Property

        Public ReadOnly Property Progressivi() As DbProgressiviProvider
            Get
                SetLastCaller()
                If _Progressivi Is Nothing Then
                    _Progressivi = New DbProgressiviProvider(_DAM)
                End If
                Return _Progressivi
            End Get
        End Property

        Public ReadOnly Property MovimentiEsterniCNS() As DbMovimentiEsterniCNSProvider
            Get
                SetLastCaller()
                If _MovimentiEsterniCNS Is Nothing Then
                    _MovimentiEsterniCNS = New DbMovimentiEsterniCNSProvider(_DAM)
                End If
                Return _MovimentiEsterniCNS
            End Get
        End Property

        Public ReadOnly Property MovimentiInterniCNS() As DbMovimentiInterniCNSProvider
            Get
                SetLastCaller()
                If _MovimentiInterniCNS Is Nothing Then
                    _MovimentiInterniCNS = New DbMovimentiInterniCNSProvider(_DAM)
                End If
                Return _MovimentiInterniCNS
            End Get
        End Property

        'Public ReadOnly Property Consenso() As DbConsensoProvider
        '    Get
        '        SetLastCaller()
        '        If _Consenso Is Nothing Then
        '            _Consenso = New DbConsensoProvider(_DAM)
        '        End If
        '        Return _Consenso
        '    End Get
        'End Property

        Public ReadOnly Property Visite() As DbVisiteProvider
            Get
                SetLastCaller()
                If _Visite Is Nothing Then
                    _Visite = New DbVisiteProvider(_DAM)
                End If
                Return _Visite
            End Get
        End Property

        Public ReadOnly Property Log() As DbLogProvider
            Get
                SetLastCaller()
                If _Log Is Nothing Then
                    _Log = New DbLogProvider(_DAM)
                End If
                Return _Log
            End Get
        End Property

        Public ReadOnly Property Lotti() As DbLottiProvider
            Get
                SetLastCaller()
                If _Lotti Is Nothing Then
                    _Lotti = New DbLottiProvider(_DAM)
                End If
                Return _Lotti
            End Get
        End Property

        Public ReadOnly Property ElaborazionePaziente() As DBElaborazioneProvider
            Get
                SetLastCaller()
                If _ElaborazionePaziente Is Nothing Then
                    _ElaborazionePaziente = New DBElaborazioneProvider(_DAM)
                End If
                Return _ElaborazionePaziente
            End Get
        End Property
        Public ReadOnly Property ElaborazioneAcn() As DbElaborazioneACNProvider
            Get
                SetLastCaller()
                If _ElaborazioneAcn Is Nothing Then
                    _ElaborazioneAcn = New DbElaborazioneACNProvider(_DAM)
                End If
                Return _ElaborazioneAcn
            End Get
        End Property

        Public ReadOnly Property Via() As DbViaProvider
            Get
                SetLastCaller()
                If _Via Is Nothing Then
                    _Via = New DbViaProvider(_DAM)
                End If
                Return _Via
            End Get
        End Property

        Public ReadOnly Property Distretto() As DbDistrettoProvider
            Get
                SetLastCaller()
                If _Distretto Is Nothing Then
                    _Distretto = New DbDistrettoProvider(_DAM)
                End If
                Return _Distretto
            End Get
        End Property

        Public ReadOnly Property Cittadinanze() As DbCittadinanzeProvider
            Get
                SetLastCaller()
                If _Cittadinanze Is Nothing Then
                    _Cittadinanze = New DbCittadinanzeProvider(_DAM)
                End If
                Return _Cittadinanze
            End Get
        End Property

        Public ReadOnly Property CategorieRischio() As DbCategorieRischioProvider
            Get
                SetLastCaller()
                If _CategorieRischio Is Nothing Then
                    _CategorieRischio = New DbCategorieRischioProvider(_DAM)
                End If
                Return _CategorieRischio
            End Get
        End Property

        Public ReadOnly Property Malattie() As DbMalattieProvider
            Get
                SetLastCaller()
                If _Malattie Is Nothing Then
                    _Malattie = New DbMalattieProvider(_DAM)
                End If
                Return _Malattie
            End Get
        End Property

        Public ReadOnly Property MotiviEsclusione() As DbMotiviEsclusioneProvider
            Get
                SetLastCaller()
                If _MotiviEsclusione Is Nothing Then
                    _MotiviEsclusione = New DbMotiviEsclusioneProvider(_DAM)
                End If
                Return _MotiviEsclusione
            End Get
        End Property

        Public ReadOnly Property NomiCommerciali() As DbNomiCommercialiProvider
            Get
                SetLastCaller()
                If _NomiCommerciali Is Nothing Then
                    _NomiCommerciali = New DbNomiCommercialiProvider(_DAM)
                End If
                Return _NomiCommerciali
            End Get
        End Property

        Public ReadOnly Property Operatori() As DbOperatoriProvider
            Get
                SetLastCaller()
                If _Operatori Is Nothing Then
                    _Operatori = New DbOperatoriProvider(_DAM)
                End If
                Return _Operatori
            End Get
        End Property

        Public ReadOnly Property StampaLibrettoVaccinale() As DbStampaLibrettoVaccinale
            Get
                SetLastCaller()
                If _StampaLibrettoVaccinale Is Nothing Then
                    _StampaLibrettoVaccinale = New DbStampaLibrettoVaccinale(_DAM)
                End If
                Return _StampaLibrettoVaccinale
            End Get
        End Property

        Public ReadOnly Property StampaElencoEsclusione() As DbStampaElencoEsclusione
            Get
                SetLastCaller()
                If _StampaElencoEsclusione Is Nothing Then
                    _StampaElencoEsclusione = New DbStampaElencoEsclusione(_DAM)
                End If
                Return _StampaElencoEsclusione
            End Get
        End Property
        Public ReadOnly Property StampaConteggioSedute() As DbStampaConteggioSedute
            Get
                SetLastCaller()
                If _StampaConteggioSedute Is Nothing Then
                    _StampaConteggioSedute = New DbStampaConteggioSedute(_DAM)
                End If
                Return _StampaConteggioSedute
            End Get
        End Property

        Public ReadOnly Property Installazioni() As DbInstallazioniProvider
            Get
                SetLastCaller()
                If _Installazioni Is Nothing Then
                    _Installazioni = New DbInstallazioniProvider(_DAM)
                End If
                Return _Installazioni
            End Get
        End Property

        Public ReadOnly Property CancellazioneConvocazioni() As DbCancellazioneConvocazioniProvider
            Get
                SetLastCaller()
                If _CancellazioneConvocazioni Is Nothing Then
                    _CancellazioneConvocazioni = New DbCancellazioneConvocazioniProvider(_DAM)
                End If
                Return _CancellazioneConvocazioni
            End Get
        End Property

        ''' <summary>
        ''' Provider per le operazioni relative alle attività
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>SOLO DB CENTRALE</remarks>
        Public ReadOnly Property Attivita() As DbAttivitaProvider
            Get
                SetLastCaller()
                If _Attivita Is Nothing Then
                    _Attivita = New DbAttivitaProvider(_DAM)
                End If
                Return _Attivita
            End Get
        End Property
        ''' <summary>
        ''' Provider per le operazioni di log
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property LogNotifiche() As DbLogNotificheProvider
            Get
                SetLastCaller()
                If _LogNotifiche Is Nothing Then
                    _LogNotifiche = New DbLogNotificheProvider(_DAM)
                End If
                Return _LogNotifiche
            End Get
        End Property
        Public ReadOnly Property ElaborazioneControlli() As DbElaborazioneControlliProvider
            Get
                SetLastCaller()
                If _ElaborazioneControllo Is Nothing Then
                    _ElaborazioneControllo = New DbElaborazioneControlliProvider(_DAM)
                End If
                Return _ElaborazioneControllo
            End Get
        End Property
        Public ReadOnly Property EsitoControlloScuole() As DbEsitoControlloScuoleProvider
            Get
                SetLastCaller()
                If _EsitoControlloScuole Is Nothing Then
                    _EsitoControlloScuole = New DbEsitoControlloScuoleProvider(_DAM)
                End If
                Return _EsitoControlloScuole
            End Get
        End Property
        Public ReadOnly Property ConfigurazioneCertificato() As DbConfigurazioneCertificatoProvider
            Get
                SetLastCaller()
                If _ConfigurazioneCertificato Is Nothing Then
                    _ConfigurazioneCertificato = New DbConfigurazioneCertificatoProvider(_DAM)
                End If
                Return _ConfigurazioneCertificato
            End Get
        End Property

        Public ReadOnly Property CodiciStruttura() As DbCodiciStrutturaProvider
            Get
                SetLastCaller()
                If _CodiciStruttura Is Nothing Then
                    _CodiciStruttura = New DbCodiciStrutturaProvider(_DAM)
                End If
                Return _CodiciStruttura
            End Get
        End Property

        Public ReadOnly Property EventiNotifiche() As DbEventiNotificheProvider
            Get
                SetLastCaller()
                If _EventiNotifiche Is Nothing Then
                    _EventiNotifiche = New DbEventiNotificheProvider(_DAM)
                End If
                Return _EventiNotifiche
            End Get
        End Property

        Public ReadOnly Property TipiErogatori() As DbTipiErogatoriProvider
            Get
                SetLastCaller()
                If _TipiErogatori Is Nothing Then
                    _TipiErogatori = New DbTipiErogatoriProvider(_DAM)
                End If
                Return _TipiErogatori
            End Get
        End Property
        Public ReadOnly Property Documenti() As DbDocumentiProvider
            Get
                SetLastCaller()
                If _Documenti Is Nothing Then
                    _Documenti = New DbDocumentiProvider(_DAM)
                End If
                Return _Documenti
            End Get
        End Property

        Public ReadOnly Property ContattiScolasticiCovid As IContattiScolasticiCovidProvider
            Get
                SetLastCaller()
                If _ContattiScolasticiCovid Is Nothing Then
                    _ContattiScolasticiCovid = New DbContattiScolasticiCovidProvider(_DAM)
                End If
                Return _ContattiScolasticiCovid
            End Get
        End Property

#Region " Provider Pubblici SOLO CENTRALI"

        Public ReadOnly Property AliasPazientiCentrale() As DbAliasCentraleProvider
            Get
                SetLastCaller()
                If _AliasCentralePazienti Is Nothing Then _AliasCentralePazienti = New DbAliasCentraleProvider(_DAM)
                Return _AliasCentralePazienti
            End Get
        End Property

        Public ReadOnly Property VaccinazioneEseguitaCentrale() As DBVaccinazioneEseguitaCentraleProvider
            Get
                SetLastCaller()
                If _VaccinazioneEseguitaCentrale Is Nothing Then
                    _VaccinazioneEseguitaCentrale = New DBVaccinazioneEseguitaCentraleProvider(_DAM)
                End If
                Return _VaccinazioneEseguitaCentrale
            End Get
        End Property

        Public ReadOnly Property VaccinazioneEsclusaCentrale() As DBVaccinazioneEsclusaCentraleProvider
            Get
                SetLastCaller()
                If _VaccinazioneEsclusaCentrale Is Nothing Then
                    _VaccinazioneEsclusaCentrale = New DBVaccinazioneEsclusaCentraleProvider(_DAM)
                End If
                Return _VaccinazioneEsclusaCentrale
            End Get
        End Property

        Public ReadOnly Property VisitaCentrale() As DBVisitaCentraleProvider
            Get
                SetLastCaller()
                If _VisitaCentrale Is Nothing Then
                    _VisitaCentrale = New DBVisitaCentraleProvider(_DAM)
                End If
                Return _VisitaCentrale
            End Get
        End Property

        ''' <summary>
        ''' Provider per le operazioni relative alla firma digitale: SOLO DB CENTRALE
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>SOLO DB CENTRALE</remarks>
        Public ReadOnly Property FirmaDigitale() As DbFirmaDigitaleProvider
            Get
                SetLastCaller()
                If _FirmaDigitale Is Nothing Then
                    _FirmaDigitale = New DbFirmaDigitaleProvider(_DAM)
                End If
                Return _FirmaDigitale
            End Get
        End Property

        ''' <summary>
        ''' Provider per le operazioni relative ad interventi
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>SOLO DB CENTRALE</remarks>
        Public ReadOnly Property Interventi() As DbInterventiProvider
            Get
                SetLastCaller()
                If _Interventi Is Nothing Then
                    _Interventi = New DbInterventiProvider(_DAM)
                End If
                Return _Interventi
            End Get
        End Property

        Public ReadOnly Property SitiInoculazione() As DbSitiInoculazioneProvider
            Get
                SetLastCaller()
                If _SitiInoculazione Is Nothing Then
                    _SitiInoculazione = New DbSitiInoculazioneProvider(_DAM)
                End If
                Return _SitiInoculazione
            End Get
        End Property

        Public ReadOnly Property VieSomministrazione() As DbVieSomministrazioneProvider
            Get
                SetLastCaller()
                If _VieSomministrazione Is Nothing Then
                    _VieSomministrazione = New DbVieSomministrazioneProvider(_DAM)
                End If
                Return _VieSomministrazione
            End Get
        End Property

        Public ReadOnly Property VaccinazioniAssociate() As DbVaccinazioniAssociateProvider
            Get
                SetLastCaller()
                If _VaccinazioniAssociate Is Nothing Then
                    _VaccinazioniAssociate = New DbVaccinazioniAssociateProvider(_DAM)
                End If
                Return _VaccinazioniAssociate
            End Get
        End Property

        Public ReadOnly Property LuoghiEsecuzioneVaccinazioni() As DbLuoghiEsecuzioneVaccinazioniProvider
            Get
                SetLastCaller()
                If _LuoghiEsecuzioneVaccinazioni Is Nothing Then _LuoghiEsecuzioneVaccinazioni = New DbLuoghiEsecuzioneVaccinazioniProvider(_DAM)
                Return _LuoghiEsecuzioneVaccinazioni
            End Get
        End Property

#End Region

#End Region

#Region "IDisposable"

        Private disposedValue As Boolean = False

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)

            If Not Me.disposedValue Then

                If disposing Then

                    If _disposeObjects Then
                        _DAM.Dispose(True)
                        If Not Me._transaction Is Nothing Then Me._transaction.Dispose()
                        If Not Me._connection Is Nothing Then Me._connection.Close()
                    End If

                Else

#If DEBUG Then
                    If Not Me._DAM Is Nothing AndAlso Me._DAM.Connection.State <> ConnectionState.Closed Then
                        Dim message As String = String.Format("Dispose del genericprovider mancante: {0}{1}{2}", _lastCaller, Environment.NewLine, _DAM.Command.CommandText)
                        Throw New ApplicationException(message)
                    End If
#End If

                End If

                'liberare le risorse non gestite condivise

            End If

            Me.disposedValue = True

        End Sub

        ' Questo codice è aggiunto da Visual Basic per implementare in modo corretto il modello Disposable.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Non modificare questo codice. Inserire il codice di pulitura in Dispose(ByVal disposing As Boolean).
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        Protected Overrides Sub Finalize()
            ' Simply call Dispose(False).
            Dispose(False)
        End Sub



#End Region

    End Class

#Region " Types "

    Public Class DbError

        Private _errorMessage As String

        Public Property ErrorMessage() As String
            Get
                Return _errorMessage
            End Get
            Set(Value As String)
                _errorMessage = Value
            End Set
        End Property

    End Class

#End Region

End Namespace
