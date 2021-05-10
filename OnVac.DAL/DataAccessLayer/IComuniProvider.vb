Imports System.Collections.Generic

Public Interface IComuniProvider

    Function GetIstatByCodice(codiceComune As String) As String

    Function GetCapByCodiceComune(codiceComune As String) As String

    Function CheckValiditaComune(codiceComune As String, dataValidita As Date) As Boolean

    Function GetComuneByCodice(codiceComune As String) As Entities.Comune

    Function GetCodiceComuneByCodiceIstat(codiceIstatComune As String) As String

    Function GetCodiceComuneByCodiceIstat(codiceIstatComune As String, dataValidita As Date) As String

    Function GetCodiceRegione(codiceComune As String) As String
    Function GetListComuni(ricerca As String) As List(Of Entities.Comune)
    Function GetComuni() As List(Of Comune)
    Function GetListComuniEsterniRegione(ricerca As String, codiceRegione As String) As List(Of Comune)
End Interface
