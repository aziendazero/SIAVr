Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.Entities


Public Interface IElaborazioneControlliProvider
    Function GetListaElaborazioneControlli(filtro As FiltroElaborazioneControlli, campoOrdinamento As String, versoOrdinamento As String, pagingOptions As Data.PagingOptions) As List(Of ElaborazioneControlli)
    Function CountListaElaborazioneControlli(filtro As FiltroElaborazioneControlli) As Integer
    Function InsertTestataControlli(testata As InsertControlliTesta) As Integer
    Function EseguiStoredProcedureControlli(idProceduraControlli As Integer) As Integer
    Function GetElaborazioneControlloXCodiceProc(filtro As FiltroSingolaElaborazioneControlli) As ElaborazioneControlli
End Interface