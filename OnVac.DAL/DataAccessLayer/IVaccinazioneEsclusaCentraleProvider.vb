Imports System.Collections.Generic

Public Interface IVaccinazioneEsclusaCentraleProvider

    Function GetVaccinazioneEsclusaCentraleById(idVaccinazioneEsclusaCentrale As Int64) As Entities.VaccinazioneEsclusaCentrale
    Function GetVaccinazioneEsclusaCentraleByUslInserimento(idVaccinazioneEsclusa As Int64, codiceUslInserimento As String) As Entities.VaccinazioneEsclusaCentrale
    Function GetVaccinazioneEsclusaCentraleDistribuitaByUsl(idVaccinazioneEsclusa As Int64, codiceUsl As String) As Entities.VaccinazioneEsclusaCentrale.VaccinazioneEsclusaDistribuita
    Function GetVaccinazioneEsclusaCentraleDistribuitaByIdCentrale(idVaccinazioneEsclusaCentrale As Long, codiceUsl As String) As Entities.VaccinazioneEsclusaCentrale.VaccinazioneEsclusaDistribuita

    Function GetVaccinazioneEsclusaCentraleByIdLocale(idVaccinazioneEsclusaLocale As Long, codiceUslLocale As String) As Entities.VaccinazioneEsclusaCentrale

    Function GetVaccinazioneEsclusaCentraleEnumerable(codicePazienteCentrale As String) As IEnumerable(Of Entities.VaccinazioneEsclusaCentrale)
    Function GetVaccinazioneEsclusaCentraleEnumerable(codicePazienteCentrale As String, flagVisibilita As String) As IEnumerable(Of Entities.VaccinazioneEsclusaCentrale)
    Function GetVaccinazioneEsclusaCentraleDistribuitaEnumerable(codicePazienteLocale As Int64, codiceUsl As String) As IEnumerable(Of Entities.VaccinazioneEsclusaCentrale.VaccinazioneEsclusaDistribuita)

    Function CountVaccinazioniEsclusePazienteCentrale(codicePazienteCentrale As String, listVisibilita As List(Of String), noEliminate As Boolean) As Integer

    Function GetUslInserimentoEscluse(codicePazienteCentrale As String, noEliminate As Boolean) As List(Of String)

    Function GetUslDistribuiteVaccinazioniEscluse(idVaccinazioniEscluseLocali As List(Of Long), codiceUslInserimentoEscluse As String) As List(Of Entities.UslDistribuitaDatoVaccinaleInfo)

    Function CountVaccinazioneEsclusaCentraleInConflittoByIdConflitto(idConflitto As Int64) As Int64
    Function CountConflittiVaccinazioniEscluseCentrale(filtriRicercaConflitti As Filters.FiltriRicercaConflittiDatiVaccinali) As Integer
    Function GetConflittiVaccinazioniEscluseCentrale(filtriRicercaConflitti As Filters.FiltriRicercaConflittiDatiVaccinali, pagingOptions As OnAssistnet.Data.PagingOptions) As List(Of Entities.ConflittoVaccinazioniEscluse)

    Sub InsertVaccinazioneEsclusaCentrale(vaccinazioneEsclusaCentrale As VaccinazioneEsclusaCentrale)
    Sub InsertVaccinazioneEsclusaCentraleDistribuita(vaccinazioneEsclusaCentraleDistribuita As VaccinazioneEsclusaCentrale.VaccinazioneEsclusaDistribuita)

    Sub UpdateVaccinazioneEsclusaCentrale(vaccinazioneEsclusaCentrale As VaccinazioneEsclusaCentrale)
    Sub UpdateVaccinazioneEsclusaCentraleDistribuita(vaccinazioneEsclusaCentraleDistribuita As VaccinazioneEsclusaCentrale.VaccinazioneEsclusaDistribuita)

    Sub UpdateIdConflittoVaccinazioneEsclusaCentraleByIdConflitto(idConflittoCorrente As Int64, idConflittoAggiornato As Int64?)

    Sub DeleteVaccinazioneEsclusaCentraleDistribuita(vaccinazioneEseguitaCentraleDistribuita As VaccinazioneEsclusaCentrale.VaccinazioneEsclusaDistribuita)

End Interface

