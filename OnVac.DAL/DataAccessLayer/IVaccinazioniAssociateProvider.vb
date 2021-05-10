Imports System.Collections.Generic

Public Interface IVaccinazioniAssociateProvider

    Function GetVaccinazioniAssociabili() As List(Of VaccinazioneAssociabile)

    Function GetCodiciVaccinazioniAssociateAMalattia(codiceMalattia As String) As List(Of String)
    Function GetCodiciVaccinazioniAssociateACategoriaRischio(codiceCatRis As String) As List(Of String)

    Function DeleteVaccinazioniAssociateAMalattia(codiceMalattia As String) As Integer
    Function InsertVaccinazioniAssociateAMalattia(codiceMalattia As String, codiciVac As List(Of String)) As Integer

    Function DeleteVaccinazioniAssociateACategoriaRischio(codiceCatRis As String) As Integer
    Function InsertVaccinazioniAssociateACategoriaRischio(codiceCatRis As String, codiciVac As List(Of String)) As Integer

End Interface
