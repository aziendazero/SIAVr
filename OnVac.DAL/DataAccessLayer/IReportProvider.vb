Imports Onit.OnAssistnet.OnVac.Entities
Imports System.Collections.Generic


Public Interface IReportProvider

    Function GetReport(reportName As String) As Report

    Function GetReports(reportName As List(Of String)) As List(Of Report)

    Function GetDatiIntestazione(codiceUsl As String) As DatiIntestazioneReport

    Function GetReportFolder(reportName As String) As String

End Interface
