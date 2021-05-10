Imports System.Collections.Generic
Imports Onit.OnAssistnet.Data
Imports Onit.OnAssistnet.OnVac.Entities


Public Interface IEsitoControlloScuoleProvider
    Function CountListaEsitoControlloScuole(filtro As FiltroEsitoControlloScuole) As Integer
    Function GetListaEsitoControlloScuole(filtro As FiltroEsitoControlloScuole, campoOrdinamento As String, versoOrdinamento As String, pagingOptions As PagingOptions) As List(Of EsitoControlloScuole)
End Interface