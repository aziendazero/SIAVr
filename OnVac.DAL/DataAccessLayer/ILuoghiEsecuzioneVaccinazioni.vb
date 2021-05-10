Imports System.Collections.Generic

Public Interface ILuoghiEsecuzioneVaccinazioniProvider

    Function GetLuoghiEsecuzioneVaccinazioni() As List(Of LuoghiEsecuzioneVaccinazioni)
    Function GetLuogoEsecuzioneVaccinazione(codice As String) As LuoghiEsecuzioneVaccinazioni
    Function InsertLuoghiEsecuzioneVaccinazione(item As LuoghiEsecuzioneVaccCommand) As Integer
    Function UpdateLuoghiEsecuzioneVaccinazione(item As LuoghiEsecuzioneVaccCommand) As Integer
    Function DeleteLuoghiEsecuzioneVaccinazione(codice As String) As Integer

    Function IsCampoInLuogoCampiObbligatori(codLuogo As String, codCampo As String) As Boolean
    Function GetDettagliCampoObbligatorioLuogo(codLuogo As String, codCampo As String) As Entities.CampoObbligLuogoVacc
    Function GetCampiObbligatori() As List(Of CampoObbligLuogoVacc)
    Function GetCampiObbligatoriByLuogo(codLuogo As String) As List(Of CampoObbligLuogoVacc)
    Sub InsertCampoObbligatorioLuogo(item As CampoObbligLuogoVacc)
    Function DeleteCampiObbligatoriLuogo(codLuogo As String) As Integer

End Interface
