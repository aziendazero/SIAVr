Imports System.Collections.Generic

Public Interface IVisiteProvider

    Function GetIdVisitaPazienteByDataAndMalattia(codicePaziente As Int64, dataVisita As DateTime, codiceMalattia As String) As Int64?
    Function GetIdVisitaPazienteSenzaMalattiaByData(codicePaziente As Int64, dataVisita As DateTime) As Int64?
    Function GetIdVisitaPazienteByBilancioAndMalattia(codicePaziente As Int64, numeroBilancio As Int64, codiceMalattia As String) As Int64?

    Function GetIdVisitaPazienteByData(codicePaziente As Integer, dataVisita As DateTime) As Int64?

    Function GetVisitaById(idVisita As Int64) As Visita
    Function GetVisiteById(listIdVisite As List(Of Int64)) As List(Of Visita)
    Function GetVisitaEliminataById(idVisita As Int64) As Visita

    Function GetVisitePaziente(codicePaziente As Integer) As List(Of Visita)

    Function GetOsservazioneById(idOsservazione As Int64) As Osservazione
    Function GetOsservazioneEliminataById(idOsservazioneEliminata As Int64) As Osservazione
    Function GetOsservazioniEliminateByIdVisitaEliminata(idVisitaEliminata As Int64) As Osservazione()

    Function GetOsservazione(codicePaziente As Integer, codiceOsservazione As String, numeroBilancio As Integer, codiceMalattia As String) As Osservazione

    Function GetOsservazioniPaziente(codicePaziente As Integer) As List(Of Osservazione)
    Function GetOsservazioniByVisita(idVisita As Int64) As Osservazione()

    Function EliminaOsservazioni(idVisita As Integer) As Integer
    Function EliminaOsservazione(idOsservazione As Int64) As Boolean
    Function EliminaVisita(idVisita As Integer) As Boolean

    Function ExistsVisita(codicePaziente As Integer, dataVisita As DateTime) As Boolean
    Function ExistsVisita(codicePaziente As Integer, dataVisita As DateTime, codiceMalattia As String) As Boolean

    Function ExistsVisitaById(idVisita As Int64) As Boolean

    Function InsertVisita(visita As Visita) As Boolean
    Function InsertVisitaEliminata(visitaEliminata As Visita) As Boolean

    Function InsertOsservazione(osservazione As Osservazione) As Boolean
    Function InsertOsservazioneEliminata(osservazione As Osservazione) As Boolean

    Function UpdateOsservazione(osservazione As Osservazione) As Boolean

    Function UpdateVisita(visita As Visita, fromUpdateBilanci As Boolean) As Boolean

    Function UpdateFlagVisibilita(idVisita As Long, flagVisibilita As String, idUtenteVariazione As Long, dataVariazione As Date) As Integer

    Function GetVisitaByIdIfExists(idVisita As Int64) As Visita
    Function GetOsservazioneByIdIfExists(idOsservazione As Int64) As Osservazione

    Function GetInfoOsservazioniSezioniRisposteByVisita(idVisita As Int64) As List(Of Osservazione)
    Function GetInfoFirmaArchiviazioneVisita(idVisita As Long, idApplicazione As String) As ArchiviazioneDIRV.InfoFirmaArchiviazioneVisita

    Function UpdateInfoFirmaDigitaleVisita(updateCommand As ArchiviazioneDIRV.FirmaDigitaleInfo) As Integer
    Function UpdateInfoArchiviazioneSostitutivaVisita(updateCommand As ArchiviazioneDIRV.FirmaDigitaleInfo) As Integer

    Function GetMaxDataFineSospensione(codicePaziente As Integer) As DateTime
    Function GetMaxDataFineSospensione(codicePaziente As Integer, dataEsecuzione As DateTime) As DateTime?

    Function GetListOsservazioniByVisita(idVisita As Long) As List(Of Osservazione)
    Function GetListViaggiVisitaById(idVisita As Long) As List(Of ViaggioVisita)
    Function EliminaViaggio(idViaggio As Integer) As Boolean
    Function GetPadreFollowup(idFollowUp As Integer) As Integer
    Function UpdateIdFollowUp(idVisita As Long, idFollowUp As Long, dataFollowUpEffettiva As Date?) As Integer
    Function UpdateCancellaLegameFollowUp(idFollowUp As Long) As Integer
End Interface
