Imports System.Collections.Generic
Imports Onit.OnAssistnet.Data

Public Interface ICodiciStrutturaProvider

#Region " HSP "

    Function GetCodiciHsp(pagingOptions As PagingOptions) As List(Of CodiceHSP)

    Function GetCountCodiciHsp() As Integer

    Function GetCodiciHspByFiltro(filtro As String, pagingOptions As Data.PagingOptions) As List(Of CodiceHSP)
    Function GetCodiciHspValidiByFiltro(filtro As String, pagingOptions As Data.PagingOptions) As List(Of CodiceHSP)

    Function GetCountCodiciHspByFiltro(filtro As String) As Integer
    Function GetCountCodiciHspValidiByFiltro(filtro As String) As Integer

    Sub UpdateCodiceHsp(command As CodiceHSPCommand)

    'Sub EliminaCodiceHsp(codiceMalattia As String, idUtente As String)

    Sub AggiungiCodiceHsp(command As CodiceHSPCommand)

    Function IsCodiceInCodiciHsp(codice As String) As Boolean

    Function GetDettaglioCodiceHspById(codice As String) As List(Of CodiceHSP)

#End Region

#Region " STS "

    Function GetCodiciSTS(pagingOptions As PagingOptions) As List(Of CodiceSTS)

    Function GetCountCodiciSTS() As Integer

    Function GetCodiciSTSByFiltro(filtro As String, pagingOptions As Data.PagingOptions) As List(Of CodiceSTS)

    Function GetCountCodiciSTSByFiltro(filtro As String) As Integer

    Sub UpdateCodiceSTS(command As CodiceSTSCommand)

    'Sub EliminaCodiceSTS(codiceMalattia As String, idUtente As String)

    Sub AggiungiCodiceSTS(command As CodiceSTSCommand)

    Function IsCodiceInCodiciSTS(codice As String) As Boolean

    Function GetDettaglioCodiceSTSById(codice As String) As List(Of CodiceSTS)

#End Region

End Interface
