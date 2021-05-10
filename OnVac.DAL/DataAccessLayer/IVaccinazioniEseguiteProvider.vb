Imports System.Collections.Generic

Public Interface IVaccinazioniEseguiteProvider

    Function GetVaccinazioniEseguite(codicePaziente As Integer) As DataTable
    Function GetVaccinazioniEseguite(codicePaziente As Long, listaIdVaccinazione As List(Of Long)) As DataTable
    Function GetVaccinazioniEseguiteIntegrazione(listaReazioni As List(Of Integer)) As DataTable
    Function GetVaccinazioniEseguitePaziente(codicePaziente As Integer) As List(Of VaccinazioneEseguita)

    Function GetVaccinazioniEseguiteById(listIdVaccinazioniEseguite As List(Of Int64)) As List(Of Entities.VaccinazioneEseguita)
    Function GetDtVaccinazioniEseguiteById(listIdVaccinazioniEseguite As List(Of Int64)) As DataTable

    Function GetVaccinazioneEseguitaPaziente(codicePaziente As Int64, codiceVaccinazione As String, numeroRichiamo As Int16) As Entities.VaccinazioneEseguita
    Function GetVaccinazioneEseguitaPaziente(codicePaziente As Int64, codiceVaccinazione As String, numeroRichiamo As Int16, dataEffettuazione As DateTime) As Entities.VaccinazioneEseguita
    Function GetVaccinazioniDosePaziente(idPaziente As Long, dataInizio As Date?, dataFine As Date?) As List(Of VaccinazioneDose)
    Function GetVaccinazioneEseguitaById(idVaccinazioneEseguita As Integer) As Entities.VaccinazioneEseguita

    Function GetVaccinazioniEseguiteScadutePaziente(codicePaziente As Integer) As VaccinazioneEseguita()

    Function GetVaccinazioneEseguitaEliminataById(idVaccinazioneEseguitaEliminata As Integer) As Entities.VaccinazioneEseguita
    Function GetVaccinazioneEseguitaScadutaEliminataById(idVaccinazioneEseguitaScadutaEliminata As Integer) As VaccinazioneEseguita
    Function GetVaccinazioneEseguitaScadutaPaziente(codicePaziente As Int64, codiceVaccinazione As String, numeroRichiamo As Int16, dataEffettuazione As DateTime) As Entities.VaccinazioneEseguita
    Function GetVaccinazioneEseguitaScadutaById(idVaccinazioneEseguitaScaduta As Integer) As Entities.VaccinazioneEseguita

    Function GetDtReazioniAvverseById(listIdReazioniAvverse As List(Of Int64?)) As DataTable
    Function GetReazioneAvversaById(idReazioneAvversa As Integer) As Entities.ReazioneAvversa

    Function GetReazioniAvversePaziente(codicePaziente As Int64) As List(Of ReazioneAvversa)
    Function GetReazioniAvverseScadutePaziente(codicePaziente As Int64) As List(Of ReazioneAvversa)

    Function GetReazioneAvversaScadutaById(idReazioneAvversaScaduta As Integer) As Entities.ReazioneAvversa
    Function GetReazioneAvversaScadutaEliminataById(idReazioneAvversaScadutaEliminata As Integer) As Entities.ReazioneAvversa
    Function GetReazioneAvversaEliminataById(idReazioneAvversaEliminata As Integer) As Entities.ReazioneAvversa

    Function GetReazioneAvversaByVaccinazioneEseguita(idVaccinazioneEseguita As Int64) As Entities.ReazioneAvversa
    Function GetReazioneAvversaScadutaByVaccinazioneEseguitaScaduta(idVaccinazioneEseguitaScaduta As Int64) As Entities.ReazioneAvversa

    Function GetCodiciLottiReazioneByDataEffettuazione(codicePaziente As Integer, dataEffettuazione As DateTime) As List(Of String)
    Function GetCodiciLottiReazioneByDataReazione(codicePaziente As Integer, dataEffettuazione As DateTime) As List(Of String)

    Function GetProssimaSedutaDaRegistrare(codicePaziente As Integer) As DataTable
    Function GetMaxRichiamo(codicePaziente As Integer, codiceVaccinazione As String) As Integer
    Function GetMaxDoseAssociazione(codicePaziente As Integer, codiceAssociazione As String) As Integer

    Function GetDtRichiamiVaccinazionePaziente(codicePaziente As String, codiceVaccinazione As String) As DataTable
    Function GetDtNumeroDosiAssociazionePaziente(codicePaziente As String, codiceAssociazione As String) As DataTable

    Function EsisteVaccinazioneEseguita(codicePaziente As Integer, dataEsecuzione As Date, codiceVaccinazione As String) As Boolean
    Function EsisteVaccinazioneScaduta(codicePaziente As Integer, dataEsecuzione As Date, codiceVaccinazione As String) As Boolean
    Function EsisteAssociazioneEseguita(codicePaziente As Long, dataEsecuzione As Date, codiceAssociazione As String) As Boolean

    Function InsertVaccinazioneEseguita(vaccinazioneEseguita As VaccinazioneEseguita) As Boolean
    Function InsertVaccinazioneEseguitaScaduta(vaccinazioneEseguitaScaduta As VaccinazioneEseguita) As Boolean
    Function InsertVaccinazioneEseguitaEliminata(vaccinazioneEseguitaEliminata As VaccinazioneEseguita) As Boolean
    Function InsertVaccinazioneEseguitaScadutaEliminata(vaccinazioneEseguitaScadutaEliminata As VaccinazioneEseguita) As Boolean

    Function InsertReazioneAvversa(reazioneAvversa As ReazioneAvversa) As Boolean
    Function InsertReazioneAvversaScaduta(reazioneAvversaScaduta As ReazioneAvversa) As Boolean
    Function InsertReazioneAvversaEliminata(reazioneAvversaEliminata As ReazioneAvversa) As Boolean
    Function InsertReazioneAvversaScadutaEliminata(reazioneAvversaScadutaEliminata As ReazioneAvversa) As Boolean

    Function UpdateVaccinazioneEseguita(vaccinazioneEseguita As VaccinazioneEseguita) As Boolean
    Function UpdateVaccinazioneEseguitaScaduta(vaccinazioneEseguitaScaduta As VaccinazioneEseguita) As Boolean

    Function UpdateReazioneAvversa(reazioneAvversa As ReazioneAvversa) As Boolean
    Function UpdateReazioneAvversaScaduta(reazioneAvversaScaduta As ReazioneAvversa) As Boolean
    Function UpdateReazioneAvversaScadutaIdScheda(reazioneAvversaScaduta As ReazioneAvversa) As Boolean
    Function UpdateReazioneAvversaIdScheda(reazioneAvversaScaduta As ReazioneAvversa) As Boolean

    Function DeleteVaccinazioneEseguitaById(idVaccinazioneEseguita As Int64) As Boolean
    Function DeleteVaccinazioneEseguitaScadutaById(idVaccinazioneEseguitaScaduta As Int64) As Boolean

    Function DeleteReazioneAvversaById(idReazioneAvversa As Int64) As Boolean
    Function DeleteReazioneAvversaScadutaById(idReazioneAvversaScaduta As Int64) As Boolean

    Function CountVaccinazioniEseguite(codicePaziente As Integer) As Integer
    Function CountVaccinazioniScadute(codicePaziente As Integer) As Integer
    Function CountReazioniAvverse(codicePaziente As Integer) As Integer

    Function GetReazioneAvversaScadutaByIdIfExists(idReazioneAvversaScaduta As Integer) As Entities.ReazioneAvversa
    Function GetVaccinazioneEseguitaByIdIfExists(idVaccinazioneEseguita As Integer) As Entities.VaccinazioneEseguita
    Function GetVaccinazioneEseguitaScadutaByIdIfExists(idVaccinazioneEseguitaScaduta As Integer) As Entities.VaccinazioneEseguita
    Function GetReazioneAvversaByIdIfExists(idReazioneAvversa As Integer) As Entities.ReazioneAvversa

    Function GetListVaccinazioniEseguitePazientiAPP(listCodiciPazienti As List(Of Long)) As List(Of Entities.VaccinazioneEseguitaAPP)
    Function GetListVaccinazioniEseguitePazienteDataAPP(codicePaziente As Long, dataEffettuazione As DateTime, codiceAssociazione As String) As List(Of Entities.VaccinazioneEseguitaAPP)
    Function GetListMantoux(codiciPazienti As List(Of Long)) As List(Of Entities.PazienteMantoux)
    Function UpdateFlagVisibilitaEseguite(idVaccinazioneEseguita As Long, flagVisibilita As String, idUtenteVariazione As Long, dataVariazione As Date) As Integer
    Function UpdateFlagVisibilitaScadute(idVaccinazioneScaduta As Long, flagVisibilita As String, idUtenteVariazione As Long, dataVariazione As Date) As Integer
	Function InsertLinkReazLogInvio(idReazioneAvversa As Long, idLogInvio As Long) As Integer
	Function GetVaccinazioniEseguiteUnionScadute(filtroCodicePaziente As Long?, filtroIdACN As String, filtroCodAssociazione As String, filtroDataEffettuazione As Date?, filtroProvenienza As String) As List(Of VaccinazioneIntegrazioneDB)
    Function UpdateNumeroVacVacEseguita(ves_id As Integer, numeroVac As Integer) As Integer
    Function UpdateNumeroAssociazioneVacEseguita(ves_id As Integer, numeroAssociazione As Integer) As Integer
    Function UpdateNumeroVacVacScadute(vsc_id As Integer, numeroVac As Integer) As Integer
    Function UpdateNumeroAssociazioneVacScadute(vsc_id As Integer, numeroAssociazione As Integer) As Integer
    Function GetListVaccinazioniEseguitePazienteFSE(codicePaziente As Long) As List(Of VaccinazioneFSE)
End Interface



