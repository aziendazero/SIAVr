Imports System.Collections.Generic

Public Interface IVaccinazioniEscluseProvider

    Sub InserisciVaccinazioneEsclusa(vaccinazioneEsclusa As VaccinazioneEsclusa)

    Sub InserisciVaccinazioneEsclusaEliminata(vaccinazioneEsclusaEliminata As VaccinazioneEsclusa)
    Sub InserisciVaccinazioneEsclusaEliminata(vaccinazioneEsclusaEliminata As VaccinazioneEsclusa, statoEliminazione As String)

    Sub ModificaVaccinazioneEsclusa(vaccinazioneEsclusa As VaccinazioneEsclusa)

    Function ExistsVaccinazioneEsclusa(codicePaziente As Integer, codiceVaccinazione As String) As Boolean

    Function ExistsVaccinazioneEsclusaNonScaduta(codicePaziente As Integer, codiceVaccinazione As String, dataConvocazione As Date) As Boolean

    Function CountVaccinazioniEsclusePaziente(codicePaziente As Integer) As Integer

    Function GetDataVaccinazioneEsclusaNonScaduta(codicePaziente As Integer, codiceVaccinazione As String, dataConvocazione As Date) As Date

    Function GetDtVaccinazioniEsclusePaziente(codicePaziente As Integer) As DataTable

    Function GetVaccinazioniEsclusePaziente(codicePaziente As Integer) As List(Of Entities.VaccinazioneEsclusa)

    Function GetVaccinazioneEsclusaPaziente(codicePaziente As Integer, codiceVaccinazione As String) As Entities.VaccinazioneEsclusa

    Function GetVaccinazioneEsclusaById(idVaccinazioneEsclusa As Long) As VaccinazioneEsclusa

    Function GetVaccinazioneEsclusaEliminataById(idVaccinazioneEsclusaEliminata As Int64) As Entities.VaccinazioneEsclusa
    Function GetVaccinazioneEsclusaEliminataByPazienteVaccinazione(idPaziente As Integer, codiceVaccinazione As String, sortColumn As String, filtraRinnovate As Boolean) As List(Of Entities.VaccinazioneEsclusaDettaglio)

    Function GetDtVaccinazioniEscluseById(listIdVaccinazioniEscluse As List(Of Int64)) As DataTable

    Sub DeleteVaccinazioneEsclusa(idVaccinazioneEsclusa As Int64)

    Sub DeleteVaccinazioniEscluse(codicePaziente As Integer)

    Function GetVaccinazioneEsclusaByIdIfExists(idVaccinazioneEsclusa As Int64) As Entities.VaccinazioneEsclusa

    Function GetVaccinazioniEscluseById(listIdVaccinazioniEseguite As List(Of Int64)) As List(Of Entities.VaccinazioneEsclusa)

    Function GetListVaccinazioniEsclusePazientiAPP(listCodiciPazienti As List(Of Int64)) As List(Of Entities.VaccinazioneEsclusaAPP)
    Function GetListVaccinazioniEsclusePazienteAPP(codicePaziente As Long, codiceVaccinazioneCovid As String) As List(Of VaccinazioneEsclusaAPP)
    Function GetVaccinazioneEsclusaPazienteAPP(idEsclusa As Int64) As Entities.VaccinazioneEsclusaAPP
    Function UpdateFlagVisibilita(idVaccinazioneEsclusa As Long, flagVisibilita As String, idUtenteVariazione As Long, dataVariazione As Date) As Integer
    Function GetListVaccinazioniEsclusePazienteFSE(codicePaziente As Long) As List(Of VaccinazioneFSE)

    Function GetMaxDoseEseguitaVaccinazionaEsclusa(codicePaziente As String, codiceVaccinazione As String) As Integer
    Function CreaInadempienza() As ResultSetPost
End Interface

