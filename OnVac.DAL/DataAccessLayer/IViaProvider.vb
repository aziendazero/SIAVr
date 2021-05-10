Imports System.Collections.Generic

Public Interface IViaProvider

    Function GetVieByComuneAndCodice(codiceComune As String, codiceVia As String) As IEnumerable(Of Entities.Via)
    Function GetVieByDistretto(codiceDistretto As String) As IEnumerable(Of Entities.Via)
    Function GetVieByCap(cap As String) As IEnumerable(Of Entities.Via)

    Function ExistsDescrizioneDiversa(codiceVia As String, codiceComune As String, descrizioneVia As String) As Boolean
    Function ExistsDescrizioneDiversa(codiceVia As String, codiceComune As String, descrizioneVia As String, progressivo As Integer) As Boolean

    Function ExistsDefault(codiceVia As String, codiceComune As String) As Boolean
    Function ExistsDefault(codiceVia As String, codiceComune As String, progressivo As Integer) As Boolean

    Function GetDescrizioniViaByCodice(codiceVia As String) As List(Of String)
    Function InsertIndirizzoCodificato(indirizzoPaziente As Entities.IndirizzoPaziente) As Integer
    Sub UpdateIndirizzoCodificato(indirizzoPaziente As Entities.IndirizzoPaziente)
    Function DeleteIndirizzoCodificato(codiceIndirizzo As Integer) As Integer

End Interface
