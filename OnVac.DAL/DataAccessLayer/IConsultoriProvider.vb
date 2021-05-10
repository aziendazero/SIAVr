Imports System.Collections.Generic

Imports Onit.OnAssistnet.OnVac.Entities


Public Interface IConsultoriProvider

    ''' <summary>
    ''' Restituisce il numero di ambulatori di un dato consultorio
    ''' </summary>
    ''' <param name="codiceConsultorio">Codice consultorio</param>
    ''' <returns>Numero di ambulatori</returns>
    ''' <remarks>
    ''' </remarks>
    Function GetNumeroAmbulatori(codiceConsultorio As String) As Integer

    Function GetNumeroAmbulatori(codiceConsultorio As String, soloAperti As Boolean) As Integer

    ''' <summary>
    ''' Restituisce un datareader sulla t_ana_ambulatori con amb_codice e amb_descrizione
    ''' degli ambulatori di un determinato consultorio.
    ''' </summary>
    ''' <param name="codiceConsultorio"></param>
    ''' <param name="soloAperti"></param>
    ''' <returns></returns>
    ''' <remarks>Specificare se si desiderano solo gli ambulatori aperti</remarks>
    Function GetAmbulatoriConsultorio(codiceConsultorio As String, soloAperti As Boolean) As List(Of Entities.Ambulatorio)

    ''' <summary>
    ''' Restituisce l'ambulatorio specificato
    ''' </summary>
    ''' <param name="codiceAmbulatorio"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetAmbulatorio(codiceAmbulatorio As Integer) As Entities.Ambulatorio

    ''' <summary>
    ''' Restituisce un datatable sulla t_ana_ambulatori con amb_codice e amb_descrizione
    ''' degli ambulatori APERTI di un determinato consultorio.
    ''' </summary>
    ''' <param name="codiceConsultorio"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetAmbulatoriAperti(codiceConsultorio As String) As DataTable

    ''' <summary>
    ''' Restituisce amb_descrizione per il determinato ambulatorio
    ''' </summary>
    ''' <param name="codiceAmbulatorio"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetAmbDescrizione(codiceAmbulatorio As Integer) As String

    ''' <summary>
    ''' Restituisce il valore di amb_medinamb per il determinato ambulatorio
    ''' </summary>
    ''' <param name="codiceAmbulatorio"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetMedicoInAmb(codiceAmbulatorio As Integer) As Boolean

    '''' <summary>
    '''' Restituisce l'ambulatorio con il minor codice ambulatorio per il determinato consultorio
    '''' </summary>
    '''' <param name="codiceConsultorio"></param>
    '''' <returns></returns>
    '''' <remarks> Il minore!!! Non quello di default!</remarks>
    'Function GetDefaultAmb(codiceConsultorio As String) As Integer

    ''' <summary>
    ''' Restituisce un oggetto di tipo consultorio creato in base al codice consultorio
    ''' </summary>
    ''' <param name="codiceConsultorio">Codice Consultorio</param>
    ''' <returns>Ritorna un datareader con "COM_DESCRIZIONE", "CNS_DESCRIZIONE", "CNS_INDIRIZZO", "COM_CAP", "CNS_N_TELEFONO"</returns>
    ''' <remarks>
    ''' </remarks>
    Function GetConsultorio(codiceConsultorio As String) As Cns

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="codiceConsultorio"></param>
    ''' <param name="queryType"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetConsultoriAperti(codiceConsultorio As String, queryType As Enumerators.QueryType) As DataTable

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="codiceConsultorio"></param>
    ''' <param name="descrizioneConsultorio"></param>
    ''' <param name="queryType"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetConsultoriAperti(codiceConsultorio As String, descrizioneConsultorio As String, queryType As Enumerators.QueryType) As DataTable

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetConsultoriAperti() As DataTable

    ''' <summary>
    ''' Restituisce il cns_descrizione per il determinato consultorio
    ''' </summary>
    ''' <param name="codiceConsultorio"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetCnsDescrizione(codiceConsultorio As String) As String

    ''' <summary>
    ''' Restituisce i consultori che sono validi per l'eta espressa in numero di giorni dalla nascita
    ''' </summary>
    Function GetConsultoriPerGiorniNascita(giorniNascita As Integer) As DataTable

    ''' <summary>
    ''' Restituisce i consultori che associati al comune passato
    ''' </summary>
    Function GetConsultoriInComune(codiceComune As String) As DataTable

    ''' <summary>
    ''' Restituisce i consultori che sono associati alla circoscrizione passata
    ''' </summary>
    Function GetConsultoriInCircoscrizione(codiceCircoscrizione As String) As DataTable

    ''' <summary>
    ''' Restituisce le circoscrizioni nel comune del consultorio
    ''' </summary>
    Function GetCircoscrizioniInConsultorio(codiceConsultorio As String) As DataTable

    ''' <summary>
    ''' Restituisce la data dell'ultima campagna effettuata nel consultorio specificato.
    ''' </summary>
    Function SelectDataUltimaConvocazioneCampagna(codiceConsultorio As String) As Date

    ''' <summary>
    ''' Modifica la data dell'ultima campagna effettuata nel consultorio specificato.
    ''' </summary>
    Function UpdateDataUltimaConvocazioneCampagna(codiceConsultorio As String, dataConvocazione As Date) As Integer

    ''' <summary>
    ''' Restituisce un intervallo con l'età minima e massima di validità del consultorio specificato.
    ''' </summary>
    Function GetEtaValiditaConsultorio(codiceConsultorio As String) As Entities.EstremiIntervallo

    ''' <summary>
    ''' Restituisce il comune associato al consultorio.
    ''' </summary>
    Function GetComuneConsultorio(codiceConsultorio As String) As String

    ''' <summary>
    ''' Restituisce il consultorio (aperto) in base al pediatra vaccinatore e all'età specificati.
    ''' </summary>
    Function GetCnsByPediatra(cod_pediatra As String, eta_paziente As Integer) As String

    ''' <summary>
    ''' Restituisce il consultorio (aperto) in base al comune e all'età specificati.
    ''' </summary>
    Function GetCnsByComune(cod_comune As String, eta_paziente As Integer) As String

    ''' <summary>
    ''' Restituisce il consultorio (aperto) in base alla circoscrizione e all'età specificata.
    ''' </summary>
    Function GetCnsByCircoscrizione(cod_circoscrizione As String, eta_paziente As Integer) As String

    ''' <summary>
    ''' Restituisce il consultorio di smistamento (aperto) in base all'età specificata.
    ''' </summary>
    Function GetCnsSmistamento(eta_paziente As Integer) As String

    ''' <summary>
    ''' Restituisce il codice del consultorio di magazzino associato al consultorio specificato
    ''' </summary>
    Function GetConsultorioMagazzino(codiceConsultorio As String) As Consultorio

    Function GetConsultorioByMachineID(machineID As Integer) As Consultorio
    Function GetConsultorioByMachineGroupID(machineGroupID As Integer) As Consultorio

    Function GetConsultoriAbilitatiUtente(idUtente As Long) As List(Of Consultorio)
    Function GetConsultorioDefaultUtente(idUtente As Long) As Consultorio

    ''' <summary>
    ''' Restituisce true se il codice del consultorio specificato è presente in anagrafe consultori
    ''' </summary>
    Function ExistsConsultorio(codiceConsultorio As String) As Boolean

    ''' <summary>
    ''' Restituisce una lista di consultori. 
    ''' Se il parametro soloCnsAperti è true, nella lista saranno presenti solo quelli aperti in data odierna.
    ''' Se il parametro codiceDescrizioneLikeFilter è valorizzato, la ricerca sarà effettuata filtrando (in like) codice e descrizione in base al valore del parametro.
    ''' </summary>
    ''' <param name="soloCnsAperti"></param>
    ''' <param name="codiceDescrizioneLikeFilter"></param>
    ''' <param name="idUtente"></param>
    ''' <param name="filtroCodDistretto"></param>
    ''' <param name="filtroUsl"></param>
    ''' <returns></returns>
    Function GetListCodiceDescrizioneConsultori(soloCnsAperti As Boolean, codiceDescrizioneLikeFilter As String, idUtente As Long, filtroCodDistretto As String, filtroUsl As String) As List(Of ConsultorioAperti)

    ''' <summary>
    ''' Restituisce il tipo del consultorio specificato.
    ''' </summary>
    Function GetTipoConsultorio(codiceConsultorio As String) As String

    ''' <summary>
    ''' Restituisce una lista con i codici dei consultori associati al distretto specificato
    ''' </summary>
    Function GetCodiciConsultoriDistretto(codiceDistretto As String) As List(Of String)
    Function GetCodiciConsultoriDistretto(codiceDistretto As String, soloAperti As Boolean) As List(Of String)
    Function GetCodiciConsultoriUsl(codiceDistretto As String, soloAperti As Boolean) As List(Of String)

    ''' <summary>
    ''' Restituisce il valore del campo cns_stampa1 per il consultorio specificato
    ''' </summary>
    Function GetCampoStampa1(codiceConsultorio As String) As String

    Function UpdateDateUltimaStampaAvvisi(codiceConsultorio As String, dataIniziale As DateTime, dataFinale As DateTime) As Integer
    Function UpdateDateUltimaStampaAvvisi(dataIniziale As DateTime, dataFinale As DateTime) As Integer
    Function UpdateDateUltimaStampaAvvisiBilancio(dataIniziale As DateTime, dataFinale As DateTime) As Integer
    Function UpdateDateUltimaStampaAvvisiBilancio(codiceConsultorio As String, dataIniziale As DateTime, dataFinale As DateTime) As Integer
    Function UpdateDateUltimaStampaAvvisoMaggiorenni(codiceConsultorio As String, dataIniziale As DateTime, dataFinale As DateTime) As Integer

    Sub VerificaAssociazioneAutomaticaConsultori(idUtente As Long, cnsDefault As String)

    Function GetDateInfoConsultorio(codiceConsultorio As String) As ConsultorioDateInfo

    Function GetListInfoConsultorio(codiceConsultorio As String) As List(Of ConsultorioInfoAPP)
    Function GetListInfoConsultori(escludiSmistamento As Boolean) As List(Of ConsultorioInfoAPP)
    Function GetOrariAppuntamentiAmbulatori(codiciAmbulatori As List(Of Integer)) As List(Of OrarioInfoAPP)
    Function GetOrariGiornalieriAmbulatori(codiciAmbulatori As List(Of Integer)) As List(Of OrarioInfoAPP)
    Function GetListCodiceDescrizioneConsultori(soloCnsAperti As Boolean, codiceDescrizioneLikeFilter As String) As List(Of Consultorio)
    Function GetTipoErogatoreConsultorio(cnsCodice As String) As String
    Function GetStrutture(codiceStruttura As String, tipoErogatore As String) As List(Of Struttura)
    Function GetRichiestaConsensoTrattamentoDatiUtente(codiceConsultorio As String) As String
    Function GetCodiceConsultorioRSA(idRSA As String) As String

End Interface
