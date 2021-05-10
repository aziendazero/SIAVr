Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities


Public Class BizReport
    Inherits BizClass

#Region " Costruttori "

    Public Sub New(dbGenericProvider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos)
        MyBase.New(dbGenericProvider, settings, contextInfos, Nothing)
    End Sub

#End Region

#Region " Public "

    ''' <summary>
    ''' Restituisce true se il report specificato esiste, per l'installazione corrente. 
    ''' Altrimenti restituisce false.
    ''' </summary>
    ''' <param name="nomeReport"></param>
    ''' <returns></returns>
    Public Function ExistsReport(nomeReport As String) As Boolean

        Return (Not GenericProvider.Report.GetReport(nomeReport) Is Nothing)

    End Function

    ''' <summary>
    ''' Restituisce true se tutti i report specificati esistono, per l'installazione corrente. 
    ''' Altrimenti restituisce false.
    ''' </summary>
    ''' <param name="nomiReport"></param>
    ''' <returns></returns>
    Public Function ExistsReportList(nomiReport As List(Of String)) As Boolean

        Dim rptList As List(Of Report) = GenericProvider.Report.GetReports(nomiReport)

        Return (Not rptList.IsNullOrEmpty())

    End Function

    ''' <summary>
    ''' Restituisce un oggetto della classe report, contenente tutte le informazioni sul report, lette dalla t_ana_report. 
    ''' Restituisce nothing se il report non è presente
    ''' </summary>
    ''' <param name="nomeReport"></param>
    ''' <returns></returns>
    Public Function GetReport(nomeReport As String) As Report

        Return GenericProvider.Report.GetReport(nomeReport)

    End Function

    ''' <summary>
    ''' Restituisce una lista di oggetti di tipo Report, in base ai nomi specificati
    ''' </summary>
    ''' <param name="nomiReport"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetReports(nomiReport As List(Of String)) As List(Of Report)

        Return GenericProvider.Report.GetReports(nomiReport)

    End Function

    ''' <summary>
    ''' Restituisce la cartella in cui è presente il report, in base al nome
    ''' </summary>
    ''' <param name="nomeReport"></param>
    ''' <returns></returns>
    Public Function GetReportFolder(nomeReport As String) As String

        Return GenericProvider.Report.GetReportFolder(nomeReport)

    End Function

    ''' <summary>
    ''' Restituisce un oggetto della classe DatiIntestazioneReport, contenente tutti i dati 
    ''' che compongono l'intestazione di ogni report per l'installazione corrente.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetDatiIntestazione() As DatiIntestazioneReport

        Return GenericProvider.Report.GetDatiIntestazione(ContextInfos.CodiceUsl)

    End Function

    ''' <summary>
    ''' Restituisce il filtro utilizzato da crystal report per il between tra due date
    ''' </summary>
    ''' <param name="dataInizio"></param>
    ''' <param name="dataFine"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetBetweenDateReportFilter(dataInizio As DateTime, dataFine As DateTime) As String

        Return String.Format("in DateTime ({0}, {1}, {2}, 00, 00, 00) to DateTime ({3}, {4}, {5}, 00, 00, 00)",
                             dataInizio.Year.ToString(), dataInizio.Month.ToString(), dataInizio.Day.ToString(),
                             dataFine.Year.ToString(), dataFine.Month.ToString(), dataFine.Day.ToString())

    End Function

#End Region

End Class
