Imports System.Collections.Generic


Public Interface INomiCommercialiProvider

    Function GetDescrizioneNomeCommerciale(codiceNomeCommerciale As String) As String
    Function GetInfoSomministrazioneDefaultNomeCommerciale(codiceNomeCommerciale As String) As InfoSomministrazione
    Function GetCostoUnitarioNomeCommerciale(codiceNomeCommerciale As String) As Double

    Function GetCodiciVaccinazioniByNomeCommerciale(codiceNomeCommerciale As String) As List(Of String)

    Function GetListCondizioniPagamento(codiceNomeCommerciale As String) As List(Of CondizioniPagamento)
    Function GetListCondizioniPagamento(codiceNomeCommerciale As String, codicePaziente As Long) As List(Of CondizioniPagamento)
    Function GetCondizionePagamento(idCondizionePagamento As Integer) As CondizioniPagamento

    Function GetListTipiPagamento() As List(Of TipiPagamento)
    Function GetTipoPagamento(guidPagamento As Byte()) As List(Of TipiPagamento)
    Function GetGestioneCondizioniPagamento(guidTipoPagamento As Byte()) As String
    Function GetNomeCommerciale(codiceNomeCommerciale As String, filtro As String) As DataTable

    Function NextValSequenceCondizionePagamento() As Integer
    Function InsertCondizionePagamento(condizionePagamento As CondizioniPagamento) As Integer
    Function UpdateCondizionePagamento(condizionePagamento As CondizioniPagamento) As Integer
    Function DeleteCondizionePagamento(idCondizionePagamento As Integer) As Integer
    Function GetCodiceNomeCommercialeByCodiceAic(codiceAic9 As String) As String
    Function GetGuidTipoPagamentoByCodiceACN(codiceACN As String) As Byte()

    Function GetTipiPagamento() As List(Of TipiPagamento)
    Sub DeleteTipoPagamento(guidPagamento As Byte())
    Sub AddTipoPagamento(pagamento As TipiPagamento)
    Sub UpdateTipoPagamento(pagamento As TipiPagamento)

End Interface
