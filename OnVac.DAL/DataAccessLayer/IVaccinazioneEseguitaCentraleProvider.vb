Imports System.Collections.Generic

Public Interface IVaccinazioneEseguitaCentraleProvider

    Function GetVaccinazioneEseguitaCentraleById(idVaccinazioneEseguitaCentrale As Int64) As VaccinazioneEseguitaCentrale
    Function GetVaccinazioneEseguitaCentraleByUslInserimento(idVaccinazioneEseguita As Int64, codiceUslInserimento As String) As Entities.VaccinazioneEseguitaCentrale
    Function GetVaccinazioneEseguitaCentraleByUslInserimentoReazioneAvversa(idReazioneAvversa As Int64, codiceUslInserimentoReazioneAvversa As String) As Entities.VaccinazioneEseguitaCentrale

    Function GetVaccinazioneEseguitaCentraleDistribuitaByUsl(idVaccinazioneEseguita As Int64, codiceUsl As String) As Entities.VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita
    Function GetVaccinazioneEseguitaCentraleDistribuitaByIdCentrale(idVaccinazioneEseguitaCentrale As Int64, codiceUsl As String) As Entities.VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita
    Function GetVaccinazioneEseguitaCentraleByIdLocale(idVaccinazioneEseguitaLocale As Long, codiceUslLocale As String) As Entities.VaccinazioneEseguitaCentrale

    Function GetVaccinazioneEseguitaCentraleEnumerable(codicePazienteCentrale As String) As IEnumerable(Of Entities.VaccinazioneEseguitaCentrale)
    Function GetVaccinazioneEseguitaCentraleEnumerable(codicePazienteCentrale As String, flagVisibilita As String) As IEnumerable(Of Entities.VaccinazioneEseguitaCentrale)
    Function GetVaccinazioneEseguitaCentraleDistribuitaEnumerable(codicePazienteLocale As Int64, codiceUsl As String) As IEnumerable(Of Entities.VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita)
    Function GetVaccinazioneEseguitaCentraleDistribuitaEnumerable(listIdVaccinazioniEseguiteCentrali As List(Of Int64), codiceUsl As String) As IEnumerable(Of Entities.VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita)

    Function GetIdLocaleVaccinazioneUslReazioneByUslVaccinazione(idVaccinazioneEseguitaLocale As Long, codiceUslVaccinazione As String, codiceUslReazione As String) As Long?

    Function GetUslDistribuiteVaccinazioniEseguite(idVaccinazioniEseguiteLocali As List(Of Long), codiceUslInserimentoVaccinazioni As String) As List(Of Entities.UslDistribuitaDatoVaccinaleInfo)
    Function GetUslDistribuiteReazioniAvverse(idReazioniAvverseLocali As List(Of Long), codiceUslInserimentoReazioni As String) As List(Of Entities.UslDistribuitaDatoVaccinaleInfo)

    Function CountVaccinazioniEseguitePazienteCentrale(codicePazienteCentrale As String, listVisibilita As List(Of String), noEliminate As Boolean) As Integer
    Function CountReazioniAvversePazienteCentrale(codicePazienteCentrale As String, listVisibilita As List(Of String), noEliminate As Boolean) As Integer

    Function GetUslInserimentoVaccinazioniEseguite(codicePazienteCentrale As String, noEliminate As Boolean) As List(Of String)
    Function GetUslInserimentoReazioniAvverse(codicePazienteCentrale As String, noEliminate As Boolean) As List(Of String)

    Function CountVaccinazioneEseguitaCentraleInConflittoByIdConflitto(idConflitto As Int64) As Int64

    Function CountConflittiVaccinazioniEseguiteCentrale(filtriRicercaConflitti As Filters.FiltriRicercaConflittiDatiVaccinali) As Integer
    Function GetConflittiVaccinazioniEseguiteCentrale(filtriRicercaConflitti As Filters.FiltriRicercaConflittiDatiVaccinali, pagingOptions As OnAssistnet.Data.PagingOptions) As List(Of Entities.ConflittoVaccinazioniEseguite)
    Function GetConflittiVaccinazioniEseguiteCentrale(codiciCentraliPazienti As List(Of String), risolviConflittiGiaTentati As Boolean) As List(Of Entities.ConflittoVaccinazioniEseguite)

    Function UpdateConflittoEseguitaCentrale(idVaccinazioneCentrale As Long, idUtenteRisoluzione As Long, dataRisoluzione As DateTime) As Integer
    Function InsertConflittoEseguiteRisoluzione(conflittoRisoluzione As Entities.ConflittoEseguiteRisoluzione) As Integer
    Function UpdateConflittoEseguiteRisoluzione(conflittoRisoluzione As Entities.ConflittoEseguiteRisoluzione) As Integer
    Function CancellaIdConflittoVaccinazioneEseguitaCentrale(idVaccinazioneCentrale As Long) As Integer

    Sub InsertVaccinazioneEseguitaCentrale(vaccinazioneEseguitaCentrale As VaccinazioneEseguitaCentrale)
    Sub InsertVaccinazioneEseguitaCentraleDistribuita(vaccinazioneEseguitaCentraleDistribuita As VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita)

    Sub UpdateVaccinazioneEseguitaCentrale(vaccinazioneEseguitaCentrale As VaccinazioneEseguitaCentrale)
    Sub UpdateVaccinazioneEseguitaCentraleDistribuita(vaccinazioneEseguitaCentraleDistribuita As VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita)

    Sub UpdateIdConflittoVaccinazioneEseguitaCentraleByIdConflitto(idConflittoCorrente As Int64, idConflittoAggiornato As Int64?)

    Sub DeleteVaccinazioneEseguitaCentraleDistribuita(vaccinazioneEseguitaCentraleDistribuita As VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita)

End Interface

