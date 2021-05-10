Imports System.Collections.Generic

Public Interface IInterventiProvider

    Function GetInterventi(filtro As String) As List(Of Intervento)
    Function GetIntervento(codiceIntervento As Integer) As Entities.Intervento
    Function InsertIntervento(intervento As Entities.Intervento) As Integer
    Function UpdateIntervento(intervento As Entities.Intervento) As Integer
    Function DeleteIntervento(codiceIntervento As Integer) As Integer

    Function GetInterventiPaziente(parametri As ParametriGetInterventiPaziente) As List(Of InterventoPaziente)
    Function InsertInterventoPaziente(intervento As Entities.InterventoPaziente, codicePaziente As Integer, uteId As Long) As Integer
    Function UpdateInterventoPaziente(intervento As Entities.InterventoPaziente) As Integer
    Function DeleteInterventoPaziente(codiceIntervento As Integer, uteId As Long) As Integer
    Function CountInterventiPaziente(codicePaziente As Long) As Integer
    Function GetDataSetConteggioConsulenze(dataNascitaInizio As Date?, dataNascitaFine As Date?, dataEsecuzioneInizio As Date?, dataEsecuzioneFine As Date?, codiciConsultori As List(Of String), codiceTipoConsulenza As String, codiceOperatore As String) As DSConteggioConsulenze
End Interface

Public Class ParametriGetInterventiPaziente

    Public CodicePaziente As Integer
    Public OrderBy As String

End Class
