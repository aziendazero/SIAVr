Imports System.Collections.Generic

Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Log


Public Class BizConsultori
    Inherits BizClass

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)

        MyBase.New(genericprovider, settings, contextInfos, logOptions)

    End Sub

#End Region

#Region " Public "

    ''' <summary>
    ''' Restituisce true se l'età del paziente è compresa nell'intervallo di validità del consultorio specificato.
    ''' </summary>
    ''' <param name="codiceConsultorio"></param>
    ''' <param name="dataNascitaPaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckEtaPazienteInConsultorio(codiceConsultorio As String, dataNascitaPaziente As Date) As Boolean

        ' Lettura intervallo età valide per il consultorio corrente
        Dim intervalloEta As Entities.EstremiIntervallo =
            Me.GenericProvider.Consultori.GetEtaValiditaConsultorio(codiceConsultorio)

        ' Età del paziente (in giorni)
        Dim etaGiorni As Integer = Date.Now.Subtract(dataNascitaPaziente).Days

        ' Controllo che l'età del paziente rientri in quella specifica per il consultorio corrente
        Return (etaGiorni >= intervalloEta.EstremoInferiore AndAlso etaGiorni <= intervalloEta.EstremoSuperiore)

    End Function

    ''' <summary>
    ''' Restituisce il codice del comune associato al consultorio specificato
    ''' </summary>
    ''' <param name="codiceConsultorio"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetComuneConsultorio(codiceConsultorio As String) As String

        Return GenericProvider.Consultori.GetComuneConsultorio(codiceConsultorio)

    End Function

    ''' <summary>
    ''' Restituisce il tipo erogatore associato al consultorio specificato
    ''' </summary>
    ''' <param name="codiceConsultorio"></param>
    ''' <returns></returns>
    Public Function GetTipoErogatoreConsultorio(codiceConsultorio As String) As String

        Return GenericProvider.Consultori.GetTipoErogatoreConsultorio(codiceConsultorio)

    End Function

    ''' <summary>
    ''' Restituisce una lista di consultori in cui sono valorizzati solo i campi codice e descrizione.
    ''' Il parametro codiceDescrizioneLikeFilter rappresenta il filtro (in like) che viene applicato a codice e descrizione.
    ''' </summary>
    ''' <param name="soloCnsAperti"></param>
    ''' <param name="codiceDescrizioneLikeFilter"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetListCodiceDescrizioneConsultori(soloCnsAperti As Boolean, codiceDescrizioneLikeFilter As String) As List(Of Entities.Consultorio)

        Return GenericProvider.Consultori.GetListCodiceDescrizioneConsultori(soloCnsAperti, codiceDescrizioneLikeFilter)

    End Function

	''' <summary>
	''' Restituisce la descrizione del consultorio specificato
	''' </summary>
	''' <param name="codiceConsultorio"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function GetCnsDescrizione(codiceConsultorio As String) As String

        Return GenericProvider.Consultori.GetCnsDescrizione(codiceConsultorio)

    End Function

    ''' <summary>
    ''' Restituisce un datatable contenente tutte le circoscrizioni associate al consultorio specificato
    ''' </summary>
    ''' <param name="codiceConsultorio"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCircoscrizioniInConsultorio(codiceConsultorio As String) As DataTable

        Return GenericProvider.Consultori.GetCircoscrizioniInConsultorio(codiceConsultorio)

    End Function

	''' <summary>
	''' Restituisce un oggetto Cns con i dati del consultorio specificato. Se non lo trova restituisce Nothing.
	''' </summary>
	''' <param name="codiceConsultorio"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function GetConsultorio(codiceConsultorio As String) As Entities.Cns

        Return GenericProvider.Consultori.GetConsultorio(codiceConsultorio)

    End Function

	Public Function GetListCodiceConsultoriUtente(idUtente As Long) As List(Of String)
		Dim list As New List(Of String)
		list = GenericProvider.Consultori.GetConsultoriAbilitatiUtente(idUtente).Select(Function(x) x.Codice).ToList()
		Return list
	End Function

    ''' <summary>
    ''' Restituisce una lista di consultori in cui sono valorizzati solo i campi codice e descrizione.
    ''' Il parametro codiceDescrizioneLikeFilter rappresenta il filtro (in like) che viene applicato a codice e descrizione.
    ''' </summary>
    ''' <param name="soloCnsAperti"></param>
    ''' <param name="codiceDescrizioneLikeFilter"></param>
    ''' <param name="idUtente"></param>
    ''' <param name="filtroCodDistretto"></param>
    ''' <param name="filtroUsl"></param>
    ''' <returns></returns>
    Public Function GetListCodiceDescrizioneConsultori(soloCnsAperti As Boolean, codiceDescrizioneLikeFilter As String, idUtente As Long, filtroCodDistretto As String, filtroUsl As String) As List(Of Entities.ConsultorioAperti)

        Return GenericProvider.Consultori.GetListCodiceDescrizioneConsultori(soloCnsAperti, codiceDescrizioneLikeFilter, idUtente, filtroCodDistretto, filtroUsl)

    End Function

    ''' <summary>
    ''' Restituisce il codice del consultorio specificato
    ''' </summary>
    ''' <param name="codiceConsultorio"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetTipoConsultorio(codiceConsultorio As String) As String

        Return Me.GenericProvider.Consultori.GetTipoConsultorio(codiceConsultorio)

    End Function

    ''' <summary>
    ''' Restituisce una stringa con tutti i codici dei consultori appartenenti al distretto specificato, utilizzabile come filtro per query di IN.
    ''' </summary>
    ''' <param name="codiceDistretto"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFiltroCodiciConsultoriDistretto(codiceDistretto As String) As String

        Dim listCodiciConsultori As List(Of String) =
            Me.GenericProvider.Consultori.GetCodiciConsultoriDistretto(codiceDistretto)

        If listCodiciConsultori Is Nothing OrElse listCodiciConsultori.Count = 0 Then
            Return String.Empty
        End If

        Return String.Format("'{0}'", listCodiciConsultori.Aggregate(Function(p, g) p & "', '" & g))

    End Function

    ''' <summary>
    ''' Restituisce l'ambulatorio associato al consultorio specificato, se univoco.
    ''' Se il consultorio ha più ambulatori associati, restituisce un ambulatorio fittizio che rappresenta il valore "TUTTI".
    ''' Se non trova nessun ambulatorio associato, restituisce lo stesso ambulatorio fittizio se il parametro setTuttiIfNull è True, altrimenti restituisce Nothing
    ''' </summary>
    ''' <param name="codiceConsultorio"></param>
    ''' <param name="soloAmbulatoriAperti"></param>
    ''' <param name="setTuttiIfNull"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAmbulatorioDefault(codiceConsultorio As String, soloAmbulatoriAperti As Boolean, setTuttiIfNull As Boolean) As Entities.Ambulatorio
        '--
        Dim ambulatorioDefault As New Entities.Ambulatorio()
        '--
        Dim numAmbulatori As Integer =
            Me.GenericProvider.Consultori.GetNumeroAmbulatori(codiceConsultorio, soloAmbulatoriAperti)
        '--
        If numAmbulatori > 1 Then
            '--
            ambulatorioDefault.Codice = Constants.AmbulatorioTUTTI.Codice
            ambulatorioDefault.Descrizione = Constants.AmbulatorioTUTTI.Descrizione
            '--
        ElseIf numAmbulatori = 0 Then
            '--
            ambulatorioDefault = Nothing
            '--
        Else
            '--
            Dim listAmbulatori As List(Of Entities.Ambulatorio) =
                Me.GenericProvider.Consultori.GetAmbulatoriConsultorio(codiceConsultorio, soloAmbulatoriAperti)
            '--
            ambulatorioDefault = listAmbulatori(0)
            '--
        End If
        '--
        If ambulatorioDefault Is Nothing AndAlso setTuttiIfNull Then
            '--
            ambulatorioDefault = New Entities.Ambulatorio()
            ambulatorioDefault.Codice = Constants.AmbulatorioTUTTI.Codice
            ambulatorioDefault.Descrizione = Constants.AmbulatorioTUTTI.Descrizione
            '--
        End If
        '--
        Return ambulatorioDefault
        '--
    End Function

    ''' <summary>
    ''' Restituisce una lista di ambulatori per il consultorio specificato
    ''' </summary>
    ''' <param name="codiceConsultorio"></param>
    ''' <param name="soloAmbulatoriAperti"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAmbulatori(codiceConsultorio As String, soloAmbulatoriAperti As Boolean) As List(Of Entities.Ambulatorio)

        Return Me.GenericProvider.Consultori.GetAmbulatoriConsultorio(codiceConsultorio, soloAmbulatoriAperti)

    End Function

    ''' <summary>
    ''' Restituisce l'ambulatorio specificato
    ''' </summary>
    ''' <param name="codiceAmbulatorio"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAmbulatorio(codiceAmbulatorio As Integer) As Entities.Ambulatorio

        Return Me.GenericProvider.Consultori.GetAmbulatorio(codiceAmbulatorio)

    End Function

    ''' <summary>
    ''' Restituisce una lista contenente i codici degli ambulatori appartenenti al consultorio specificato
    ''' </summary>
    ''' <param name="codiceConsultorio"></param>
    ''' <param name="soloAmbulatoriAperti"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCodiciAmbulatori(codiceConsultorio As String, soloAmbulatoriAperti As Boolean) As List(Of Integer)

        Dim listAmbulatori As List(Of Entities.Ambulatorio) = Me.GetAmbulatori(codiceConsultorio, soloAmbulatoriAperti)

        If listAmbulatori Is Nothing OrElse listAmbulatori.Count = 0 Then
            Return Nothing
        End If

        Return listAmbulatori.Select(Function(p) p.Codice).ToList()

    End Function

    ''' <summary>
    ''' Restituisce il valore del campo cns_stampa1 per il consultorio specificato
    ''' </summary>
    ''' <param name="codiceConsultorio"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCampoStampa1(codiceConsultorio As String) As String

        Return Me.GenericProvider.Consultori.GetCampoStampa1(codiceConsultorio)

    End Function

    ''' <summary>
    ''' Effettua l'update delle date di inizio e fine periodo di ultima stampa dell'avviso maggiorenni, relative al consultorio specificato.
    ''' </summary>
    ''' <param name="codiceConsultorio"></param>
    ''' <param name="dataIniziale"></param>
    ''' <param name="dataFinale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateDateUltimaStampaAvvisoMaggiorenni(codiceConsultorio As String, dataIniziale As DateTime, dataFinale As DateTime) As Integer

        Return Me.GenericProvider.Consultori.UpdateDateUltimaStampaAvvisoMaggiorenni(codiceConsultorio, dataIniziale, dataFinale)

    End Function

    ''' <summary>
    ''' Classe ConsultorioLoginCommand 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ConsultorioLoginCommand

        Public CodiceConsultorio As String
        Public IdUtente As Integer
        Public ComputerName As String
        Public RemoteIP As String
        Public MachineID As Integer?
        Public MachineGroupID As Integer?

    End Class

    ''' <summary>
    ''' Se il codice del consultorio non è specificato, effettua i seguenti tentativi di ricerca:
    ''' 1) ricerca consultorio in base all'id della postazione
    ''' 2) ricerca consultorio in base all'id del gruppo di postazioni
    ''' 3) ricerca consultorio di default tra quelli abilitati all'utente
    ''' Il primo che trova, nell'ordine indicato, viene restituito come consultorio di lavoro.
    ''' Effettua il log delle varie operazioni eseguite per la ricerca del consultorio.
    ''' </summary>
    ''' <param name="consultorioLoginCommand"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetConsultorioLogin(consultorioLoginCommand As ConsultorioLoginCommand) As Entities.Consultorio

        Dim consultorioLogin As Entities.Consultorio = Nothing

        If String.IsNullOrEmpty(consultorioLoginCommand.CodiceConsultorio) Then

            Dim testataLog As DataLogStructure.Testata = New DataLogStructure.Testata(DataLogStructure.TipiArgomento.POSTAZIONI, DataLogStructure.Operazione.Generico)
            testataLog.Utente = consultorioLoginCommand.IdUtente
            testataLog.ComputerName = consultorioLoginCommand.ComputerName
            testataLog.Intestazione = True
            testataLog.DataOperazione = Date.Now

            Dim recordLog As DataLogStructure.Record = New DataLogStructure.Record()
            recordLog.Campi.Add(New DataLogStructure.Campo("IP Macchina", consultorioLoginCommand.RemoteIP))
            recordLog.Campi.Add(New DataLogStructure.Campo("ID Macchina", consultorioLoginCommand.MachineID.ToStringOrDefault()))

            If consultorioLoginCommand.MachineID.HasValue Then
                '--
                ' Recupero del consultorio in base all'id della postazione
                consultorioLogin = Me.GenericProvider.Consultori.GetConsultorioByMachineID(consultorioLoginCommand.MachineID.Value)
                '--
                If Not consultorioLogin Is Nothing Then
                    recordLog.Campi.Add(New DataLogStructure.Campo("Associazione POSTAZIONE - CNS", "Trovata"))
                Else
                    recordLog.Campi.Add(New DataLogStructure.Campo("Associazione POSTAZIONE - CNS", "Non trovata"))
                    '--
                    ' Recupero del consultorio in base all'id del gruppo di postazioni
                    If (consultorioLoginCommand.MachineGroupID.HasValue) Then
                        consultorioLogin = Me.GenericProvider.Consultori.GetConsultorioByMachineGroupID(consultorioLoginCommand.MachineGroupID.Value)
                    Else
                        consultorioLogin = Nothing
                    End If
                    '--
                    If Not consultorioLogin Is Nothing Then
                        recordLog.Campi.Add(New DataLogStructure.Campo("Associazione GRUPPO - CNS", "Trovata"))
                    Else
                        recordLog.Campi.Add(New DataLogStructure.Campo("Associazione GRUPPO - CNS", "Non trovata"))
                    End If
                    '--
                End If
                '--
            Else
                recordLog.Campi.Add(New DataLogStructure.Campo("Associazione POSTAZIONE - CNS", "MachineID mancante"))
            End If

            If consultorioLogin Is Nothing Then
                '--
                ' Recupero del consultorio di default tra quelli abilitati all'utente
                consultorioLogin = Me.GenericProvider.Consultori.GetConsultorioDefaultUtente(consultorioLoginCommand.IdUtente)
                '--
                If Not consultorioLogin Is Nothing Then
                    recordLog.Campi.Add(New DataLogStructure.Campo("CNS di default abilitato all'utente", "Trovato"))
                Else
                    recordLog.Campi.Add(New DataLogStructure.Campo("CNS di default abilitato all'utente", "Non trovato"))
                End If
                '--
            End If

            If Not consultorioLogin Is Nothing Then
                recordLog.Campi.Add(New DataLogStructure.Campo("Consultorio associato", String.Format("{0} - {1}", consultorioLogin.Codice, consultorioLogin.Descrizione)))
            Else
                recordLog.Campi.Add(New DataLogStructure.Campo("Consultorio associato", "Nessun Consultorio associato"))
            End If

            ' --- Log postazioni --- '
            testataLog.Records.Add(recordLog)
            LogBox.WriteData(testataLog)
            ' ---------------------- '

        Else
            consultorioLogin = New Entities.Consultorio()
            consultorioLogin.Codice = consultorioLoginCommand.CodiceConsultorio
            consultorioLogin.Descrizione = Me.GetCnsDescrizione(consultorioLoginCommand.CodiceConsultorio)
        End If

        Return consultorioLogin

    End Function

    Public Sub VerificaAssociazioneAutomaticaConsultori(idUtente As Long)

        If Me.Settings.ASSOCIAZIONE_AUTO_CV Then
            Me.GenericProvider.Consultori.VerificaAssociazioneAutomaticaConsultori(idUtente, Me.Settings.CNS_DEFAULT)
        End If

    End Sub

    ''' <summary>
    ''' Restituisce una struttura contenente alcune informazioni relative al consultorio specificato: data periodo stampa avviso, bilancio, 
    ''' avviso maggiorenni, creazione campagna e ricerca convocazioni per appuntamenti
    ''' </summary>
    ''' <param name="codiceConsultorio"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDateInfoConsultorio(codiceConsultorio As String) As Entities.ConsultorioDateInfo

        Return GenericProvider.Consultori.GetDateInfoConsultorio(codiceConsultorio)

    End Function

    Public Function GetStrutture(codiceStruttura As String, tipoErogatore As String) As List(Of Entities.Struttura)

        If String.IsNullOrWhiteSpace(codiceStruttura) OrElse String.IsNullOrWhiteSpace(tipoErogatore) Then
            Return New List(Of Entities.Struttura)()
        End If

        Return GenericProvider.Consultori.GetStrutture(codiceStruttura, tipoErogatore)

    End Function

    Public Function GetStrutture(codiceConsultorio As String) As List(Of Entities.Struttura)

        If String.IsNullOrWhiteSpace(codiceConsultorio) Then
            Return New List(Of Entities.Struttura)
        End If

        Return GenericProvider.Consultori.GetStrutture(codiceConsultorio, String.Empty)

    End Function

    ''' <summary>
    ''' Restituisce true se il centro vaccinale specificato è impostato per richiedere il consenso all'utente sul trattamento dei dati
    ''' </summary>
    ''' <param name="codiceConsultorio"></param>
    ''' <returns></returns>
    Public Function GetRichiestaConsensoTrattamentoDatiUtente(codiceConsultorio As String) As Boolean

        Dim valore As String = GenericProvider.Consultori.GetRichiestaConsensoTrattamentoDatiUtente(codiceConsultorio)

        Return valore = "S"

    End Function

    ''' <summary>
    ''' Restituisce il codice del centro vaccinale associato all'RSA specificata
    ''' </summary>
    ''' <param name="idRSA"></param>
    ''' <returns></returns>
    Public Function GetCodiceConsultorioRSA(idRSA As String) As String

        If String.IsNullOrWhiteSpace(idRSA) Then
            Return String.Empty
        End If

        Return GenericProvider.Consultori.GetCodiceConsultorioRSA(idRSA)

    End Function

#Region " OnVac API "

    ''' <summary>
    ''' Restituisce l'elenco di consultori e ambulatorio aperti, con gli orari di appuntamento
    ''' </summary>
    ''' <returns></returns>
    Public Function GetListInfoConsultori() As List(Of Entities.ConsultorioInfoAPP)

        Return Me.GetListInfoConsultori(Nothing)

    End Function

    ''' <summary>
    ''' Restituisce le info sul consultorio specificato (una list perchè contiene un elemento per ogni ambulatorio, anche se il consultorio è sempre lo stesso)
    ''' </summary>
    ''' <param name="codiceConsultorio"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetListInfoConsultorio(codiceConsultorio As String) As List(Of Entities.ConsultorioInfoAPP)

        Return Me.GetListInfoConsultori(codiceConsultorio)

    End Function

    Private Function GetListInfoConsultori(codiceConsultorio As String) As List(Of Entities.ConsultorioInfoAPP)

        Dim listInfoConsultori As List(Of Entities.ConsultorioInfoAPP) = Nothing

        If String.IsNullOrEmpty(codiceConsultorio) Then

            ' Nessun filtro => leggo tutti i consultori aperti (tranne lo smistamento)
            listInfoConsultori = Me.GenericProvider.Consultori.GetListInfoConsultori(True)

        Else

            ' Filtro x codice consultorio => leggo solo il consultorio specificato (ma possono esserci più ambulatori, quindi viene sempre restituita una lista)
            listInfoConsultori = Me.GenericProvider.Consultori.GetListInfoConsultorio(codiceConsultorio)

        End If

        If Not listInfoConsultori Is Nothing AndAlso listInfoConsultori.Count > 0 Then

            Dim listCodiciAmbulatoriOrariGiornalieri As New List(Of Integer)()

            For Each infoConsultorio As Entities.ConsultorioInfoAPP In listInfoConsultori

                If infoConsultorio.CodiceAmbulatorio > 0 AndAlso infoConsultorio.CountOrariGiornalieri > 0 Then
                    listCodiciAmbulatoriOrariGiornalieri.Add(infoConsultorio.CodiceAmbulatorio)
                End If

            Next

            ' Carico gli orari degli ambulatori specificati
            Dim listOrari As List(Of Entities.OrarioInfoAPP) = Nothing

            If listCodiciAmbulatoriOrariGiornalieri.Count = 0 Then
                listOrari = New List(Of Entities.OrarioInfoAPP)()
            Else
                listOrari = Me.GenericProvider.Consultori.GetOrariGiornalieriAmbulatori(listCodiciAmbulatoriOrariGiornalieri)
            End If

            ' Includo gli orari nella lista delle info
            For Each infoConsultorio As Entities.ConsultorioInfoAPP In listInfoConsultori

                If listOrari.Any(Function(p) p.CodiceAmbulatorio = infoConsultorio.CodiceAmbulatorio) Then
                    infoConsultorio.OrariAmbulatorio = listOrari.Where(Function(p) p.CodiceAmbulatorio = infoConsultorio.CodiceAmbulatorio)
                End If

                infoConsultorio.AppIdAziendaLocale = Me.ContextInfos.IDApplicazione

            Next

        End If

        Return listInfoConsultori

    End Function

    Public Function GetRSA(ULSS As String, Filtro As String, Take As Integer?, Skip As Integer?) As List(Of String)
    End Function

#End Region

#End Region

End Class
