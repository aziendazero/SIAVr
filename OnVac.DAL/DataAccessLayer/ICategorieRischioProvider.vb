Imports System.Collections.Generic

Public Interface ICategorieRischioProvider

    Function LoadDataTableCategorieRischio() As DataTable
	Function GetDescrizioneCategoriaRischio(codiceCategoriaRischio As String) As String
    Function GetCondizioniRischioPaziente(codicePaziente As Integer) As List(Of CondizioneRischio)
    Function GetCondizioniRischio(codicePaziente As Long, codiceVaccinazione As String) As List(Of PazienteCondizioneRischio)
    Function GetCondizioniRischioByAssociazione(codicePaziente As Long, codiceAssociazione As String) As List(Of PazienteCondizioneRischio)
    Function GetCodiceDescrizioneCategorieRischio(codiciCategorieRischio As List(Of String)) As List(Of KeyValuePair(Of String, String))
    Function GetCodCategoriaRischioCodiceACN(codiceEsterno As String) As String
    Function GetCategoriaRischioPaziente(codicePaziente As Long) As CondizioneRischioCodiceDescrizione

End Interface