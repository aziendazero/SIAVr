Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.Entities.Consenso
Imports Onit.OnAssistnet.OnVac.Filters

Public Interface IPazienteProvider

    Sub InserisciPaziente(paziente As Paziente)
    Sub ModificaPaziente(paziente As Paziente)

    Function EliminaRitardiPaziente(codicePaziente As Integer) As Integer
    Function EliminaCicliPaziente(codicePaziente As Integer) As Integer
    Function DeleteInadempienza(codicePaziente As Integer, codiceVaccinazione As String) As Integer

    Function GetCodicePazientiByCodiceAusiliario(codiceAusiliario As String) As ICollection(Of String)
    Function GetCodicePazientiByCodiceRegionale(codiceRegionale As String) As ICollection(Of String)
    Function GetCodicePazientiByTessera(tessera As String) As ICollection(Of String)
    Function GetCodicePazientiByCodiceFiscale(codiceFiscale As String) As ICollection(Of String)
    Function GetCodicePazientiByComponentiCodiceFiscale(nome As String, cognome As String, sesso As String, dataNascita As DateTime?, codiceComuneNascita As String) As ICollection(Of String)

    Function Exists(codicePaziente As Integer) As Boolean
    Function GetFromKey(codicePaziente As Integer) As DataTable
    Function PazienteChiamabile(codicePaziente As Integer) As String
    Function PazienteDeceduto(codicePaziente As Integer) As Boolean
    Function IsSenzaPediatra(codicePaziente As Integer) As Boolean
    Function IsPazienteInRSA(codicePaziente As Integer) As Boolean
    Function StatoAnag(codicePaziente As Integer) As String
    Function HasRitardi(codicePaziente As String) As Boolean

    Function GetPazienti(filtri As FiltriRicercaPaziente, joinComuni As Boolean) As DataTable
    Function GetPazienti(elencoCodiciPazienti As String) As Collection.PazienteCollection
    Function GetPazienti(elencoCodiciPazienti As String, codiceUsl As String) As Collection.PazienteCollection
    Function GetPazientiRiconduzione(filtriRiconduzione As Filters.FiltriRiconduzionePazienti, codiceUsl As String) As Collection.PazienteCollection

    Function GetDatiAnagraficiPaziente(codicePaziente As Integer) As Entities.PazienteDatiAnagrafici
    Function GetDatiAnagraficiPazienteCentrale(codicePazienteCentrale As String) As Entities.PazienteDatiAnagrafici

    Function GetCodiceStatoAnag(codicePaziente As Integer) As String
    'Function GetNoteAcquisizione(codicePaziente As Integer) As String
    Function GetCodiceStatoVaccinale(codicePaziente As Integer) As Enumerators.StatiVaccinali
    Function GetCodiceConsultorio(codicePaziente As Integer) As String
    Function GetCodiceAusiliario(codicePaziente As Integer) As String
    Function GetCodiceRegionale(codicePaziente As Integer) As String
    Function GetCampiAnagraficiObbligatori(codicePaziente As Integer, bloccanti As Boolean, cittadinanza As String) As DataTable
    Function GetMaxDataFineSospensione(codicePaziente As Integer) As Date
    Function GetCircoscrizioneResidenza(codicePaziente As Integer) As String
    Function GetCircoscrizioneDomicilio(codicePaziente As Integer) As String
    Function GetAltroPazienteStessoCodFiscale(codicePaziente As Integer, codiceFiscale As String) As Integer
    Function GetCodiceComuneResidenza(codicePaziente As Integer) As String
    Function GetCodiceComuneDomicilio(codicePaziente As Integer) As String
    Function GetCodiceComuneEmigrazione(codicePaziente As Integer) As String
    Function GetCodiceComuneImmigrazione(codicePaziente As Integer) As String
    Function GetDataImmigrazione(codicePaziente As Integer) As Date
    Function GetDataNascita(codicePaziente As Integer) As Date
    Function GetDataNascitaCentrale(codicePazienteCentrale As String) As Date
    Function GetDataDecesso(codicePaziente As Integer) As DateTime
    Function GetDataDecessoCentrale(codicePazienteCentrale As String) As DateTime
    Function GetSessoPazienteCentrale(codicePazienteCentrale As String) As String
    Function GetSessoPaziente(codicePaziente As Integer) As String

    Function GetRitardi(codicePaziente As Integer, cnvData As Date) As DataTable
    Function GetDateAppuntamentiRitardi(codicePaziente As Integer, dataConvocazione As Date, codiceCiclo As String, numeroSeduta As Integer) As Entities.DateAppuntamentiRitardi

    Function CercaPazientiFuoriEtaCns(codiceConsultorio As String) As Integer()
    Function CercaPazientiFuoriEtaCns(filtriPaziente As Filters.FiltriRicercaPaziente, aggiornaAnchePazientiConAppuntamenti As Boolean) As Integer()
    Function CercaNuovoCns(codicePaziente As Integer) As Hashtable
    Function CercaPazientiAdultiDaAggiornare(codiceConsultorio As String, consideraCirc As Boolean, consideraCom As Boolean) As DataTable
    Function CercaPazientiDeceduti(codiceConsultorio As String) As Integer()
    Function CercaPazientiDecedutiConConvocazioni(codiceConsultorio As String) As Integer()

    Function UpdateStatoAnagrafico(codicePaziente As Integer, statoAnagrafico As Enumerators.StatoAnagrafico) As Integer
    Function UpdateStatoVaccinalePaziente(codicePaziente As Integer, newStatoVacc As String) As Integer
    Function UpdateStatoVaccinalePaziente(codicePaziente As Integer, oldStatoVacc As String, newStatoVacc As String) As Integer
    Function UpdateConsultori(codicePaziente As Integer, consultorioOld As String, consultorioNew As String, dataAssegnazione As DateTime, updateCnsTerritoriale As Boolean) As Integer
    Function UpdateCircoscrizioneResidenza(codicePaziente As Integer, codiceCircoscrizione As String) As Integer
    Function UpdateCircoscrizioneDomicilio(codicePaziente As Integer, codiceCircoscrizione As String) As Integer
    Function UpdateCodiceAusiliario(codicePaziente As Integer, codiceAusiliario As String) As Integer
    Function UpdateNotePazienteSollecito(codicePaziente As Integer, notePaziente As String, codiceUsl As String, idUtente As Integer) As Integer
    Function UpdateFlagStampaAvvisoMaggiorenni(codiceConsultorio As String, codiceComuneResidenza As String, dataNascitaIniziale As DateTime, dataNascitaFinale As DateTime, flagAvvisoMaggiorenni As String) As Integer
    Function UpdateCodiceRegionale(codicePaziente As Integer, codiceRegionale As String) As Integer

    Function InsertInadempienza(command As InsertInadempienzaCommand) As Integer
    Function UpdateInadempienze(codiceConsultorio As String, listStatiAnagrafici As List(Of String), idUtente As Integer, setCasoConcluso As Boolean) As Integer

    Function RegolarizzaPaziente(codicePaziente As Integer, regolarizza As Boolean) As Boolean
    Function SetFlagRegolarizzato(codicePaziente As Integer, flagRegolarizzato As Boolean) As Integer
    Function SetFlagRichiestaCertificato(codicePaziente As Integer, flagRichiestaCertificato As Boolean) As Integer
    Function SetFlagCancellato(codicePaziente As Integer, flagCancellato As Boolean) As Integer

    Function CountVaccinazioniEseguite(codicePaziente As Integer) As Integer
    Function CountVaccinazioniScadute(codicePaziente As Integer) As Integer
    Function CountVaccinazioniEscluse(codicePaziente As Integer) As Integer
    Function CountSospensioni(codicePaziente As Integer) As Integer
    Function CountOsservazioni(codicePaziente As Integer) As Integer

    Function CountDocumenti(codicePaziente As String) As Integer
    Function CountRifiuti(codicePaziente As String) As Integer
    Function CountVisite(codicePaziente As String) As Integer
    Function CountInadempienze(codicePaziente As Integer) As Integer
    Function CountInadempienze(codicePaziente As Integer, codiceVaccinazione As String) As Integer

    Function GetControlliCampiPaziente(funzione As Constants.FunzioniControlloCampiPaziente) As List(Of Entities.ControlloCampiPaziente)

    Function GetValoreCampiPaziente(codicePaziente As String, listNomiCampi As List(Of String)) As Hashtable

    Function GetStatoAcquisizioneDatiVaccinaliCentrale(codicePaziente As Int64) As Enumerators.StatoAcquisizioneDatiVaccinaliCentrale?

    Function GetPazienteDistribuito(codiceAusiliarioPaziente As String, codiceUsl As String) As PazienteDistribuito

    Function UpdateCodiceEIndirizzoResidenzaPaziente(codicePaziente As Integer, codiceIndirizzoResidenza As String, descrizioneIndirizzoResidenza As String) As Integer
    Function UpdateCodiceEIndirizzoDomicilioPaziente(codicePaziente As Integer, codiceIndirizzoDomicilio As String, descrizioneIndirizzoDomicilio As String) As Integer
    Function UpdateIndirizzoResidenzaPaziente(codicePaziente As Integer, descrizioneIndirizzoResidenza As String) As Integer
    Function UpdateIndirizzoDomicilioPaziente(codicePaziente As Integer, descrizioneIndirizzoDomicilio As String) As Integer

    Function GetCodiceIndirizzoResidenzaPaziente(codicePaziente As Integer) As Integer
    Function GetCodiceIndirizzoDomicilioPaziente(codicePaziente As Integer) As Integer

    Function GetConsensiRegistratiPazienteAzienda(codiceCentralePaziente As String, idConsensi As List(Of Integer), codiceAziendaRegistrazione As String) As List(Of Entities.Consenso.ConsensoRegistrato)

    Function InsertConsenso(consenso As Entities.Consenso.ConsensoPaziente) As Integer
    Function GetIdEnteDefaultConsenso(idConsenso As Integer) As Integer
    Function GetDescrizioneConsensoByIdConsenso(idConsenso As Integer) As String
    Function GetDescrizioneLivelloConsensoByIdLivello(idLivelloConsenso As Integer) As String
    Function GetTipoEventoByIdLivelloConsenso(idLivelloConsenso As Integer) As String

    Function GetUltimoConsensoRegistratoPaziente(codiceCentralePaziente As String, idConsenso As Integer, codiceAziendaRegistrazione As String) As Entities.Consenso.ConsensoRegistrato
    Function GetUltimoConsensoRegistratoPazientiCollegati(codiceCentralePazienteReferente As String, idConsenso As Integer, codiceAzienda As String) As List(Of Entities.Consenso.ConsensoRegistrato)
    Function GetPazientiStatoAnagraficoAttivo(listCodiciLocaliPazienti As List(Of Int64)) As List(Of Entities.DatiPazienteAPP)
    Function GetDatiPazientiAPP(listCodiciLocaliPazienti As List(Of Long)) As List(Of Entities.DatiPazienteAPP)
    Function GetUltimiConsensiRegistrati(listCodiciPazienti As List(Of String), filtroIdConsenso As Integer?, filtroFlagCalcoloStatoGlobale As Boolean?, codiceAziendaRegistrazione As String) As List(Of Entities.Consenso.ConsensoRegistrato)
    Function GetConsensiLivelliDefaultNonRilevato(codiceAziendaCentrale As String) As List(Of Entities.Consenso.ConsensoRegistrato)

    Function GetListInfoDistribuzionePazienti(listCodiciCentraliPazienti As List(Of String)) As List(Of Entities.PazienteInfoDistribuzione)
    Function GetInfoDistribuzioneCompletaPazienti(listCodiciCentraliPazienti As List(Of String)) As List(Of Entities.PazienteInfoDistribuzione)

    Function GetDtMantoux(codicePaziente As Long) As DataTable
    Function CountMantoux(codicePaziente As Long) As Integer

    Function GetDatiAnagraficiPazienteIntestazioneBilancio(codicePaziente As Long) As Entities.PazienteDatiAnagraficiIntestazioneBilancio
    Function GetDatiAnagraficiPazienteCentraleIntestazioneBilancio(codicePazienteCentrale As String) As Entities.PazienteDatiAnagraficiIntestazioneBilancio

    Function IsTotalmenteInadempiente(codicePaziente As Long) As Boolean

    Function GetEsitoControlloSituazioneVaccinale(codicePaziente As Long) As String
    Function GetNotePaziente(codicePaziente As Long, codiceUsl As String) As List(Of PazienteNote)
    Function UpdateNotePaziente(idNote As Long, codicePaziente As Long, codiceUsl As String, codiceTipoNote As String, testoNote As String, idUtenteInserimento As Integer, dataInserimento As Date) As Integer
    Function InsertNotePaziente(codicePaziente As Long, codiceUsl As String, codiceTipoNote As String, testoNote As String, idUtenteInserimento As Integer, dataInserimento As Date) As Integer
    Function CountNote(codicePaziente As String, codiceUsl As String, soloCampiShowCnv As Boolean) As Integer
    Function ExistsNotaPaziente(codicePaziente As Long, codiceUsl As String, codiceTipoNota As String) As Integer?
    Function GetDescrizioneNotePaziente(codiceTipoNota As String) As String

    Function GetInfoAssistito(CF As String) As InfoAssistito
    Function GetContattiAssistito(CF As String) As ContattiAssistito
    Function GetPazientiUlssByCodiceFiscale(codiceFiscale As String) As IEnumerable(Of PazienteUlss)
    Function GetCodicePazientiByIdACN(IdACN As String) As List(Of String)
    Function UpdateIdACNPaziente(codicePaziente As Long, idACN As String) As Integer
    Function GetPazientiUlssByCellulare(cellulare As String, idConsenso As Integer, codiceAziendaRegistrazione As String) As ConsensoRegistrato
    Function GetCodicePazienteByDocumentUniqueId(documentUniqueId As String, tipoDocumento As String) As Integer?
    Function GetDatiPazienteFSE(codicePaziente As Integer) As PazienteFSE
    Function GetDocumentUniqueIdByCodicePaziente(codicePaziente As Long, tipoDocumento As String) As String
    Function GetDocumentiAssistitoByCodice(codice As String) As List(Of DtoDocumento)
    Function InsertTrasferimentoAVN(codiceLocalePaziente As Integer, codiceRegionale As String, codiceComuneEmigrazione As String, dataEmigrazione As Date) As Integer
    Function GetPazientiByRSA(idRSA As String, Nome As String, Cognome As String, CF As String) As List(Of InfoAssistito)

    Function CountEpisodiCovid(codicePaziente As Long) As Integer
    Function CountTamponiCovid(codicePaziente As Long) As Integer
    Function CountTestSierologiciCovid(codicePaziente As Long) As Integer
    Function CountRicoveriCovid(codicePaziente As Long) As Integer

    Function InsertDocumentUniqueId(documentUniqueId As String, codicePaziente As Long, tipoDocumento As String, dataInserimento As Date) As Integer

    Function RicercaPazientiCentrale(filtro As FiltroRicercaPaziente, ordinamento As String) As List(Of PazienteTrovato)
    Function RicercaPazientiLocale(filtro As FiltroRicercaPaziente, ordinamento As String) As List(Of PazienteTrovato)
End Interface

Public Class InsertInadempienzaCommand
    Public CodicePaziente As Long
    Public CodiceVaccinazione As String
    Public FlagStampato As String
    Public StatoInadempienza As Enumerators.StatoInadempienza?
    Public IdUtenteInserimento As Long
    Public DataInserimento As DateTime
    Public DataInvioSollecito As DateTime?
    Public DataAppuntamento As DateTime?
    Public DateAppuntamentiRitardiPaziente As Entities.DateAppuntamentiRitardi
End Class