Imports System.Collections.Generic
Imports Onit.OnAssistnet.Data


Public Interface IMotiviEsclusioneProvider

    Function GetMotivoEsclusione(codiceEsclusione As String) As MotivoEsclusione
    Function GetCodiciMotiviCentralizzati() As IEnumerable(Of String)
    Function GetScadenzeEsclusioneDt(codiceEsclusione As String) As DataTable
    Function GetDataTableMotiviEsclusione() As DataTable

    Function InsertScadenzaEsclusione(codiceEsclusione As String, mesi As Integer, anni As Integer, codiciVaccinazioni As String) As Integer
    Function DeleteScadenzaEsclusione(id As Integer) As Integer
    Function UpdateScadenzaEsclusione(id As Integer, mesi As Integer, anni As Integer, codiciVaccinazioni As String) As Integer

    Function MotivoEsclusioneGeneraInadempienza(codiceMotivoEsclusione As String) As Boolean

    Function GetCodiceMotivoEsclusioneDefaultInadempienza() As String

End Interface
