Imports Onit.OnAssistnet.OnVac.Report.ReportViewer
Imports System.Collections.Generic

Public Class ReportViewerExportHandler
    Implements IHttpHandler

    Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest

        Dim idApplicazione As String = context.Request.QueryString.Get("appId")
        Dim codiceAzienda As String = context.Request.QueryString.Get("codAzienda")
        Dim rptName As String = context.Request.QueryString.Get("rpt")
        Dim tipoReport As TipoEsportazioneReport = [Enum].Parse(GetType(TipoEsportazioneReport), context.Request.QueryString.Get("format"))
        Dim idProcesso As Long = Convert.ToInt64(context.Request.QueryString.Get("id"))

        Dim rptBytes As Byte() = Nothing

        Using rptViewer As New ExportReport(idApplicazione, codiceAzienda)

            Select Case rptName

                Case Constants.ReportName.EstrazioneControlliCentri
                    rptBytes = rptViewer.ExportReportEstrazioneControlliCentri(idProcesso)

                Case Constants.ReportName.EstrazioneControlliScuole
                    rptBytes = rptViewer.ExportReportEstrazioneControlliScuole(idProcesso)

                Case Constants.ReportName.EstrazioneElencoConsulenze

                    ' Filtri da querystring
                    Dim filters As String = GetExportFilters(context)
                    Dim result As ReportExportResult = GetReportEstrazioneElencoConsulenze(tipoReport, idApplicazione, codiceAzienda, context, filters)
                    If Not result Is Nothing Then
                        rptBytes = result.ReportExport
                    End If


            End Select

        End Using

        Dim fileName As String = String.Format("{0}_{1:yyyyMMddHHmmss}", rptName, DateTime.Now)

        context.Response.OutputStream.Write(rptBytes, 0, rptBytes.Length)

        Dim contentType As String = Nothing
        Dim fileExtension As String = Nothing

        Select Case tipoReport

            Case TipoEsportazioneReport.PDF
                contentType = Constants.MIMEContentType.APPLICATION_PDF
                fileExtension = "pdf"

            Case TipoEsportazioneReport.XLS
                contentType = Constants.MIMEContentType.APPLICATION_EXCEL
                fileExtension = "xls"

        End Select

        context.Response.AddHeader("Content-disposition", String.Format("filename={0}.{1}", fileName, fileExtension))
        context.Response.ContentType = contentType
        context.Response.Flush()

    End Sub

    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            ' Return false in case your Managed Handler cannot be reused for another request.
            ' Usually this would be false in case you have some state information preserved per request.
            Return False
        End Get
    End Property

    Private Class ReportExportResult
        Public Property ReportExport As Byte()
        Public Property FileName As String
    End Class


#Region " Export Filters "

    Private Function GetExportFilters(context As HttpContext) As String

        Dim filtriQuery As New Text.StringBuilder()

        ' Lettura filtri da querystring:
        Dim dataNascDa As String = context.Request.QueryString.Get("f-dataNascDa")
        If Not String.IsNullOrEmpty(dataNascDa) Then
            filtriQuery.AppendFormat(" AND PAZ_DATA_NASCITA >= TO_DATE('{0}','dd/MM/yyyy') ", dataNascDa)
        End If

        Dim dataNascA As String = context.Request.QueryString.Get("f-dataNascA")
        If Not String.IsNullOrEmpty(dataNascA) Then
            filtriQuery.AppendFormat(" AND PAZ_DATA_NASCITA <= TO_DATE('{0}','dd/MM/yyyy') ", dataNascA)
        End If

        Dim dataEsecDa As String = context.Request.QueryString.Get("f-dataEsecDa")
        If Not String.IsNullOrEmpty(dataEsecDa) Then
            filtriQuery.AppendFormat(" AND PIT_DATA_INTERVENTO >= TO_DATE('{0}','dd/MM/yyyy') ", dataEsecDa)
        End If

        Dim dataEsecA As String = context.Request.QueryString.Get("f-dataEsecA")
        If Not String.IsNullOrEmpty(dataEsecA) Then
            filtriQuery.AppendFormat(" AND PIT_DATA_INTERVENTO <= TO_DATE('{0}','dd/MM/yyyy') ", dataEsecA)
        End If

        Dim codCNS As String = context.Request.QueryString.Get("f-cns")
        If Not String.IsNullOrEmpty(codCNS) Then
            filtriQuery.Append(" AND PAZ_CNS_CODICE IN (")
            Dim listCns As List(Of String) = codCNS.Split("|").ToList()
            Dim i As Integer = listCns.Count()
            For Each cns As String In listCns
                filtriQuery.AppendFormat("'{0}'{1}", cns, IIf(i > 1, ",", ""))
                i -= 1
            Next
            filtriQuery.Append(") ")
        End If

        Dim tipoConsulenza As String = context.Request.QueryString.Get("f-tipoConsulenza")
        If Not String.IsNullOrEmpty(tipoConsulenza) Then
            filtriQuery.AppendFormat(" AND PIT_INT_CODICE = '{0}' ", tipoConsulenza)
        End If

        Dim codOperatore As String = context.Request.QueryString.Get("f-codOperatore")
        If Not String.IsNullOrEmpty(codOperatore) Then
            filtriQuery.AppendFormat(" AND PIT_OPE_CODICE = '{0}' ", codOperatore)
        End If

        Return filtriQuery.ToString()

    End Function

#End Region


#Region " Private "

    Private Function GetReportEstrazioneElencoConsulenze(tipoReport As TipoEsportazioneReport, idApplicazione As String, codiceAzienda As String, context As HttpContext, filtroReport As String) As ReportExportResult

        ' Lettura parametri da querystring
        Dim idProcesso As Long = Convert.ToInt64(context.Request.QueryString.Get("id"))
        Dim rptName As String = context.Request.QueryString.Get("rpt")
        Dim rpt As String = context.Request.QueryString.Get("rpt")

        ' Export
        Dim rptBytes As Byte() = Nothing

        Using rptViewer As New ExportReport(idApplicazione, codiceAzienda)

            Select Case rpt
                Case Constants.ReportName.EstrazioneElencoConsulenze
                    If tipoReport = TipoEsportazioneReport.XLS Then
                        rptBytes = rptViewer.ExportReportEstrazioneElencoConsulenze(idProcesso, filtroReport)
                    End If

            End Select

        End Using

        Dim result As New ReportExportResult()
        result.ReportExport = rptBytes
        result.FileName = String.Format("{0}_{1:yyyyMMddHHmmss}", rptName, DateTime.Now)

        Return result

    End Function

#End Region


End Class
