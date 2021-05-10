Imports System.Collections.Generic

Public Interface IMalattieProvider

    Function LoadMalattiePaziente(codicePaziente As Integer) As dsMalattie.MalattieDataTable
    Sub SalvaMalattiePaziente(codicePaziente As Integer, dtaMalattie As dsMalattie.MalattieDataTable)

    Function LoadDescrizioneMalattia(codiceMalattia As String) As String
    Function LoadMalattieEsenzione() As List(Of Malattia)
    Function LoadMalattia(codiceMalattia As String) As Malattia

    Function GetTipologiaMalattie() As List(Of Malattia.TipologiaMalattia)
    Function DeleteTipologiaMalattia(codiceMalattia As String) As Integer
    Function InsertTipologiaMalattia(codiceMalattia As String, listaTipologie As List(Of String)) As Integer

    Function GetFlagVisita(codiceMalattia As String) As String

    Function GetMalattie(codiceEsclusioneNessunaMalattia As String) As DataTable
    Function GetCodiceDescrizioneMalattie(filtraObsoleti As Boolean, codiciTipologieMalattia As List(Of String)) As List(Of KeyValuePair(Of String, String))
    Function GetMalattieByCodiciACN(listaCodiciACN As List(Of String)) As List(Of Malattia)

    Function GetCodiceTipologieByMalattia(codiceMalattia As String) As List(Of String)
    Function GetEsenzioniMalattiaPaziente(codicePaziente As Integer) As List(Of EsenzioneMalattia)
    Function GetEsenzioneMalattiaPazienteByCodiceMalattia(codicePaziente As Integer, codiceMalattia As String) As EsenzioneMalattia
    Function GetCodiceMalattiaByCodiceEsenzione(codiceEsenzione As String) As String
    Function MaxNumeroMalattiaPaziente(codicePaziente As Integer) As Integer

    Function InsertEsenzioneMalattiaPaziente(codicePaziente As Integer, esenzioneMalattia As Entities.EsenzioneMalattia) As Integer
    Function DeleteMalattiePaziente(codicePaziente As Integer, codiciMalattie As List(Of String)) As Integer

    Function GetCondizioniSanitariePaziente(codicePaziente As Integer, codiciTipologiaMalattia As List(Of String)) As List(Of CondizioneSanitaria)
    Function GetCondizioniSanitarie(codicePaziente As Long, codiceVaccinazione As String) As List(Of PazienteCondizioneSanitaria)

    Function GetCodiceDescrizioneMalattie(codiciMalattie As List(Of String)) As List(Of KeyValuePair(Of String, String))
    Function GetCodiceDescrizioneMalattie(codiciMalattie As List(Of String), codiciTipologieMalattia As List(Of String)) As List(Of KeyValuePair(Of String, String))
    Function GetCondizioniSanitarieByAssociazione(codicePaziente As Long, codiceAssociazione As String) As List(Of PazienteCondizioneSanitaria)
End Interface
