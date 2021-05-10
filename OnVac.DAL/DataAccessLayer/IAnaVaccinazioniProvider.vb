Imports System.Collections.Generic

Imports Onit.OnAssistnet.OnVac.Entities


Public Interface IAnaVaccinazioniProvider

    Function GetVaccinazioni(obbligatorietaVaccinazioni As List(Of String)) As DataTable
    Function GetDataTableCodiceDescrizioneVaccinazioni(campoOrdinamento As String) As DataTable
    Function GetDataTableVaccinazioniSostitute() As DataTable

    Function GetDescrizioneVaccinazione(codiceVaccinazione As String) As String

    Function GetCodiceDescrizioneVaccinazioni() As List(Of KeyValuePair(Of String, String))
    Function GetCodiceDescrizioneVaccinazioni(codiciVaccinazioni As List(Of String)) As List(Of KeyValuePair(Of String, String))
    Function GetCodiceDescrizioneVaccinazioni(includiObsolete As Boolean, includiSostituite As Boolean) As List(Of KeyValuePair(Of String, String))

    Function IsVaccinazioneObbligatoria(codiceVaccinazione As String) As Boolean

    Function GetVaccinazioneInfo(codiceVaccinazione As String) As Entities.VaccinazioneInfo
    Function InsertVaccinazioneInfo(info As Entities.VaccinazioneInfo) As Integer
    Function UpdateVaccinazioneInfo(info As Entities.VaccinazioneInfo) As Integer
    Function DeleteVaccinazioneInfo(idInfo As Integer) As Integer

    Function GetListCodiciEsterniVaccinazioneAssociazione(codiceVaccinazione As String) As List(Of Entities.CodificaEsternaVaccinazioneAssociazione)

    Function InsertCodificaVaccinazioneAssociazione(codifica As Entities.CodificaEsternaVaccinazioneAssociazione) As Integer
    Function DeleteCodificheAssociazione(codiceVaccinazione As String) As Integer

    Function GetListVaccinazioniAPP() As List(Of Entities.VaccinazioneAPP)
    Function GetListVaccinazioniAPP(codiceAssociazione As String) As List(Of Entities.VaccinazioneAPP)

    Function GetListBilancioVaccinazioneFromAnagrafica() As List(Of Entities.BilancioVaccinazione)
    Function GetListAssociazioneByListVacc(listaVac As List(Of String), countList As Integer) As List(Of StatVacciniAssociatiControllo)
    Function ExistsVaccinazioneAntiInfluenzale(codiceVac As String) As Boolean
    Function GetCodiceAvnVaccinazione(codiceVac As String) As VaccinazioneAVN
    Function GetCodificaEsterna(codiceVaccinazione As String, codiceAssociazione As String) As String

End Interface