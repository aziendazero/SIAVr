Imports System.Collections.Generic

Public Interface IDistrettoProvider

    Function GetCodiceByCodiceEsterno(codiceEsterno As String) As String
	Function GetListaDistretto(codice As String, codiceComune As String, codiceUls As String) As List(Of Distretto)
    Function GetListaDistrettoCNSUtente(idutente As Long) As List(Of DistrettoDDL)
End Interface
