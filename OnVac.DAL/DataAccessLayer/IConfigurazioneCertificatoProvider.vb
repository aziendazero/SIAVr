Imports System.Collections.Generic

Public Interface IConfigurazioneCertificatoProvider

    Function GetConfigurazioneCertificato(id As Integer) As IEnumerable(Of Entities.ConfigurazioneCertificazione)
    Function GetListVacciniConfigurazione(idConfigurazione As Integer) As IEnumerable(Of Entities.ConfigurazioneCertificazioneVaccinazioni)
    Function InsertConfigurazioneCertificato(configurazione As Entities.ConfigurazioneCertificazione) As Integer
    Sub UpdateConfigurazioneCertificato(configurazione As Entities.ConfigurazioneCertificazione)
    Function DeleteConfigurazioneCertificato(idConfigurazione As Integer) As Integer
    Function DeleteConfigurazioneCertVaccini(idConfigurazione As Integer) As Integer
    Function InsertConfigurazioneCertificatoVaccini(configurazioneVaccini As ConfigurazioneCertificazioneVaccinazioni) As Boolean
    Function GetScrittaCertificato(idPaz As String) As String
End Interface

