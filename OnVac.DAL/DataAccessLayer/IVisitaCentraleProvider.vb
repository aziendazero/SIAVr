Imports System.Collections.Generic

Public Interface IVisitaCentraleProvider

    Function GetVisitaCentraleById(idVisitaCentrale As Long) As Entities.VisitaCentrale
    Function GetVisitaCentraleByUslInserimento(idVisita As Int64, codiceUslInserimento As String) As Entities.VisitaCentrale
    Function GetVisitaCentraleDistribuitaByUsl(idVisita As Int64, codiceUsl As String) As Entities.VisitaCentrale.VisitaDistribuita
    Function GetVisitaCentraleDistribuitaByIdCentrale(idVisitaCentrale As Long, codiceUsl As String) As Entities.VisitaCentrale.VisitaDistribuita

    Function GetVisitaCentraleByIdLocale(idVisitaLocale As Long, codiceUslLocale As String) As Entities.VisitaCentrale

    Function GetVisitaCentraleEnumerable(codicePazienteCentrale As String) As IEnumerable(Of Entities.VisitaCentrale)
    Function GetVisitaCentraleEnumerable(codicePazienteCentrale As String, flagVisibilita As String) As IEnumerable(Of Entities.VisitaCentrale)
    Function GetVisitaCentraleDistribuitaEnumerable(codicePazienteLocale As Int64, codiceUsl As String) As IEnumerable(Of Entities.VisitaCentrale.VisitaDistribuita)

    Function CountVisitePazienteCentrale(codicePazienteCentrale As String, listVisibilita As List(Of String), noEliminate As Boolean) As Integer

    Function GetUslInserimentoVisite(codicePazienteCentrale As String, noEliminate As Boolean) As List(Of String)

    Function GetUslDistribuiteVisite(idVisiteLocali As List(Of Long), codiceUslInserimentoVisite As String) As List(Of Entities.UslDistribuitaDatoVaccinaleInfo)

    Function CountVisitaCentraleInConflittoByIdConflitto(idConflitto As Int64) As Int64
    Function CountConflittiVisiteCentrale(filtriRicercaConflitti As Filters.FiltriRicercaConflittiDatiVaccinali) As Integer
    Function GetConflittiVisiteCentrale(filtriRicercaConflitti As Filters.FiltriRicercaConflittiDatiVaccinali, pagingOptions As OnAssistnet.Data.PagingOptions) As List(Of Entities.ConflittoVisite)

    Sub InsertVisitaCentrale(visitaCentrale As VisitaCentrale)
    Sub InsertVisitaCentraleDistribuita(visitaCentraleDistribuita As VisitaCentrale.VisitaDistribuita)

    Sub UpdateVisitaCentrale(visitaCentrale As VisitaCentrale)
    Sub UpdateVisitaCentraleDistribuita(visitaCentraleDistribuita As VisitaCentrale.VisitaDistribuita)

    Sub UpdateIdConflittoVisitaCentraleByIdConflitto(idConflittoCorrente As Int64, idConflittoAggiornato As Int64?)

    Sub DeleteVisitaCentraleDistribuita(visitaCentraleDistribuita As VisitaCentrale.VisitaDistribuita)

End Interface