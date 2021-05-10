Imports System.Collections.Generic

Public Interface IRilevazioniCovid19Provider

    Function GetCodiceGuarito() As Integer?
    Function GetCodiceGuaritoClinicamente() As Integer?
    Function GetSituazioneDiaria(codiceDiaria As Long) As SituazioneDiaria
    Sub AggiornaDiaria(codiceDiaria As Long, note As String, nuoviSintomi As IEnumerable(Of Integer), linkSintomiDaEliminare As List(Of Long), idUtente As Long)

    Function GetUltimaDiaria(codicePaziente As Long) As Diaria
    Function GetDatiPazienti(codicePaziente As List(Of Long)) As List(Of DatiPaziente)
    Function GetDatiPaziente(codicePaziente As Long) As DatiPaziente
    Function SalvaElaborazioniContatti(dati As IEnumerable(Of ImportazioneContatti)) As String
    Function GetPossibiliContatti(codiciFiscali As IEnumerable(Of String)) As IEnumerable(Of InfoPossibileContatto)

    Function GetEpisodiByPaziente(codicePaziente As Long) As List(Of EpisodioPaziente)

    Function RicercaEpisodi(ricerca As RicercaEpisodi) As List(Of EpisodioPaziente)

    Function RicercaEpisodiWorkList(ricerca As RicercaEpisodi) As List(Of InfoEpisodioCovid)

    Function GetEpisodio(idEpisodio As Long) As EpisodioPaziente

    Function InsertEpisodio(episodio As EpisodioPaziente) As Long

    Sub UpdateEpisodio(episodio As EpisodioPaziente)

    Sub DeleteRicoveri(ricoveriDelete As List(Of Long))

    Sub DeleteTamponi(tamponiDelete As List(Of Long))

    Sub DeleteDiaria(diariaDelete As List(Of Long))

    Sub DeleteContatti(contattiDelete As List(Of Long))

    'Restituisce l'elenco dei pazienti che hanno evidenziato il paziente dell'episodio indice (passato nel parametro) come contatto
    Function RecuperaPazientiCreatoriDelContatto(codicePazienteEpisodio As Long) As List(Of DatiPazienteCreatoreContatto)

    Sub DeleteTags(idEpisodio As Long)

    Function RicercaTags(ricerca As RicercaTags) As Dictionary(Of String, String)

    'elimina i contatti che hanno come riferimento questo episodio.
    Sub DeleteContattiRiferimento(idEpisodio As Long)

    Sub DeleteEpisodio(episodioToDelete As EpisodioPaziente)

    'marca il paziente che ha ricevuto le credenziali per accedere all'app mobile del Veneto
    Sub MarcaPazienteInvioCredenzialiApp(pazCodice As Long)

    Function UpdateFlagLetturaInformativaCovid(codicePaziente As Long, informativaLetta As Boolean) As Integer

    Function GetFlagLetturaInformativaCovid(codicePaziente As Long) As String

    'Permette di modificare l'utente d'inserimento del tampone.
    Sub UpdateUtenteInserimento(idTampone As Long, utenteInserimento As Long)

    'Metodo utilizzato per le diarie "auto-inserite" dal paziente tramite app di OnVac.
    Sub InsertDiariaApp(codiceEpisodio As Long, codicePaziente As Long, sintomi As List(Of Integer), note As String, idUtente As Long)

    Function GetTamponiCertificatoNeg(idEpisodio As Long) As List(Of TamponeCertificatoNeg)
    Function GetTipoScolastico() As List(Of TipoScolastico)
    Function GetSettingScolastisco() As List(Of SettingScolastico)
    Function GetTipiEventiRicovero() As List(Of TipoEventoRicovero)
    Function GetTestRapidoTarById(idTest As String, ulssRichiedente As String) As TestRapido
    Function GetTestRapidoById(idTest As String, ulssRichiedente As String) As TestRapido
    Function GetTestRapidoByCodiceFiscale(codiceFiscale As String, UlssRichiedente As String) As List(Of TestRapido)
    Sub AggiornaStatoSorveglianzaEpisodiDaRicovero(dataRicovero As Date, codiceTipoRicovero As Long, codiciEpisodi() As Long)
    Function GetTestRapidoTarByCodiceFiscale(codiceFiscale As String, UlssRichiedente As String) As List(Of TestRapido)
    Function GetTestRapidoByCFeId(idTest As String, codiceFiscale As String, ulssRichiedente As String) As TestRapido
    Function GetTestRapidoTarByCFeId(idTest As String, codiceFiscale As String, ulssRichiedente As String) As TestRapido
    Function GetCentriByGruppo(gruppo As String) As List(Of String)
    Function GetTestRapidoByCFeIdeGr(idTest As String, codiceFiscale As String, gruppo As String) As TestRapido
    Function GetTestRapidoTarByCFeIdeGr(idTest As String, codiceFiscale As String, gruppo As String) As TestRapido
    Function GetTestRapidoByIdGr(idTest As String, Gruppo As String) As TestRapido
    Function GetTestRapidoTarByIdGr(idTest As String, Gruppo As String) As TestRapido
    Function GetTestRapidoByCodiceFiscaleGr(codiceFiscale As String, Gruppo As String) As List(Of TestRapido)
    Function GetTestRapidoTarByCodiceFiscaleGr(codiceFiscale As String, Gruppo As String) As List(Of TestRapido)
    Function SalvaDettaglio(dettaglio As DettaglioTestRapido) As ResultSetPost
    Function SalvaDettaglioTar(dettaglio As DettaglioAntigeneTestRapido) As ResultSetPost
    Function CercaCasiIndice(idEpisodi As List(Of Long)) As List(Of CasoIndice)
    Function GetResultEsiti() As List(Of ResultEsiti)
    Function GetStatiEpisodio() As List(Of StatiEpisodio)
    Function EpisodiApertiPerPaziente(codicePaziente As Long) As List(Of Long)
    Function PossibiliFocolaiEpisodio(codiceEpisodio As Long) As IEnumerable(Of InfoFocolaio)
    Function FocolaiEpisodio(codiceEpisodio As Long) As IEnumerable(Of InfoFocolaio)
    Function GetPazientiDaEpisodiConSorveglianzaAttiva() As List(Of AppZeroDatiEpisodio)
    Function SintomiApp() As List(Of SintomiApp)
    Function GetInformazioniFocolaio(codiceFocolaio As Integer) As DettagliFocolaio
    Function GetContattiPositiviLiberi(codiceEpisodio As Long) As IEnumerable(Of ContattoLiberoFocolaio)
    Sub AbbinaFocolaioEpisodio(codiceEpisodio As Long, codiceFocolaio As Integer)
    Function CreaFocolaio(model As CreaFocolaio) As Long
    Function GetCodiciLaboratorio(q As String) As IEnumerable(Of CodiceLaboratorioTampone)
    Function GetTestataEpisodi(codiceEpisodio As IEnumerable(Of Long)) As IEnumerable(Of TestataEpisodioCovid)
    Sub SalvaTestateEpisodi(dati As List(Of SalvaTestataEpisodioCovid))
    Function GetIdEpisodiByPaziente(codicePaziente As Long) As List(Of Long)
End Interface
