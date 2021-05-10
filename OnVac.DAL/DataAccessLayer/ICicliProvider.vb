Imports System.ComponentModel
Imports System.Collections.Generic

Public Interface ICicliProvider

    Function ExistsVaccinazioneCicliPaziente(codicePaziente As Integer, codiceCiclo As String) As Boolean
    Function ExistsCicliConVaccinazione(codicePaziente As Integer) As Boolean

    Function CountCicliIncompatibili(paziente As Paziente) As Integer

    Function LoadCicli() As BindingList(Of Ciclo)
    Function LoadCicliPaziente(codicePaziente As Integer) As List(Of CicloPaziente)

    Function GetCodiciCicliStandard(paziente As Paziente) As List(Of String)
    Function GetInfoSomministrazioneDefaultCiclo(codiceCiclo As String, numeroSeduta As Integer, codiceVaccinazione As String) As Entities.InfoSomministrazione
    Function GetDtCicliByCategoriaRischio(codiceCategoriaRischio As String) As DataTable

    Function InsertCicloPaziente(codicePaziente As Integer, codiceCiclo As String) As Integer

    Function DeleteCicloPaziente(codicePaziente As Integer, codiceCiclo As String) As Integer
    Function DeleteCicliPaziente(codicePaziente As Integer, codiciCicli As List(Of String)) As Integer
    Function DeleteCicliPaziente(codicePaziente As Integer) As Integer

    Function GetCalendarioVaccinaleStandard(dataNascita As DateTime, sesso As String) As List(Of Entities.CalendarioVaccinaleGenerico)
    Function GetCalendarioVaccinalePazienti(listCodiciPazienti As List(Of Long)) As List(Of Entities.CalendarioVaccinalePaziente)

    Function GetCicliConvocazione(codicePaziente As Long, dataConvocazione As DateTime) As List(Of Entities.CicloConvocazione)
    Function GetCicloConvocazione(codicePaziente As Long, dataConvocazione As DateTime, codiceCiclo As String, numeroSeduta As Integer) As Entities.CicloConvocazione

    Function GetDataInvioSollecito(codicePaziente As Long, dataConvocazione As DateTime, codiceCiclo As String, numeroSeduta As Integer, numeroSollecito As Integer) As DateTime?
    Function GetCicliIncompatibili(codicePaziente As Long, codiceCiclo As String) As List(Of String)
    Function ExistsCicloPaziente(codicePaziente As Long, codiceCiclo As String) As Boolean

End Interface