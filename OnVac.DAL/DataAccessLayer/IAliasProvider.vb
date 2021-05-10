Imports System.Collections.Generic
Imports Onit.OnAssistnet.Data


Public Interface IAliasProvider

    Function LoadColonneAlias() As List(Of ColonneAlias)

    Function CloneAlias(codicePazienteMaster As Integer, codicePazienteAlias As Integer, dataAlias As DateTime, idUtente As Integer) As Integer

    Function DeleteAliasFromTemp(codicePazienteAlias As Integer) As Integer
    Function DeleteAliasFromPazienti(codicePazienteAlias As Integer) As Integer

    Function LoadAlias(filtriRicerca As Filters.FiltriRicercaPaziente, pagingOptions As PagingOptions) As List(Of PazienteAlias)
    Function LoadAliasFromPazienti(filtriRicerca As Filters.FiltriRicercaPaziente, pagingOptions As PagingOptions) As List(Of PazienteAlias)

    Function CountAliasToLoad(filtriRicerca As Filters.FiltriRicercaPaziente) As Integer
    Function CountAliasToLoadFromPazienti(filtriRicerca As Filters.FiltriRicercaPaziente) As Integer

    Function InsertDatiMergeAlias(datiColonneAlias As ColonneAlias, codicePazienteMaster As Integer, codicePazienteAlias As Integer) As Integer
    Function DeleteDatiMergeAlias(nomeTabella As String, nomeCampoCodicePaziente As String, codicePazienteAlias As Integer) As Integer

    Function InsertDatiUnmergeAlias(datiColonneAlias As ColonneAlias, codicePazienteMaster As Integer, codicePazienteAlias As Integer) As Integer
    Function DeleteDatiUnmergeAlias(nomeTabella As String, nomeCampoCodicePaziente As String, nomeCampoCodicePazienteOld As String, codicePazienteMaster As Integer, codicePazienteAlias As Integer) As Integer

    Function LoadMergeInfo() As List(Of MergeInfo)
    Function LoadMergeUpdateInfo() As List(Of MergeUpdateInfo)
    Function UpdateTabellaMerge(mergeUpdateInfo As MergeUpdateInfo, codicePazienteMaster As Long, codicePazienteAlias As Long) As Integer

    Function GetDtValoriAlias(mergeInfo As MergeInfo, codicePazienteMaster As Integer, codicePazienteAlias As Integer, unmergeOperation As Boolean) As DataTable

    Function GetNextSequenceValue(sequenceName As String) As Integer

    Function InsertValoreAliasPadre(mergeInfo As MergeInfo, codicePazienteMaster As Integer, codicePazienteAlias As Integer, sequenceId As Integer, rowDatiAlias As DataRow, unmergeOperation As Boolean) As Integer
    Function InsertValoriAliasFiglia(mergeInfo As MergeInfo, codicePazienteMaster As Integer, codicePazienteAlias As Integer, oldSequenceId As Integer, newSequenceId As Integer, unmergeOperation As Boolean) As Integer

    Function GetCodiciPazientiDaUnificare(dataUltimaElaborazione As DateTime?) As List(Of KeyValuePair(Of Integer, Integer))

End Interface


