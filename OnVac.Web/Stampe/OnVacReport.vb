Public Class OnVacReport

    Public Shared Function StampaReport(rptName As String, rptFilter As String, Optional rptParameter As ReportParameter = Nothing, Optional otherRpt As String = Nothing, Optional ordinamento As ReportParameter.StrutturaOrdinamento = Nothing, Optional cartella As String = "", Optional formato As CrystalReportWrapper.docFormat = Web.Report.CrystalReportUtility.docFormat.PortableDocFormat) As Boolean

        If cartella <> "" Then

            Dim rpt As New ReportParameter()

            If Not rptParameter Is Nothing Then
                rpt = rptParameter
            End If

            rpt.BackToPage = String.Empty
            rpt.ExportType = formato
            rpt.ConnectionCripted = OnVacContext.GetEncryptedConnectionString()
            rpt.ReportName = rptName
            rpt.Filter = rptFilter
            rpt.ShowReport(ReportParameter.GoBackTypeEnum.HistoryBack, ordinamento, cartella)

            Return True

        Else

            Return False

        End If

    End Function

    Public Shared Function StampaReport(backToPage As String, rptName As String, rptFilter As String, Optional rptParameter As ReportParameter = Nothing, Optional otherRpt As String = Nothing, Optional ordinamento As ReportParameter.StrutturaOrdinamento = Nothing, Optional cartella As String = "", Optional formato As CrystalReportWrapper.docFormat = Web.Report.CrystalReportUtility.docFormat.PortableDocFormat) As Boolean

        If cartella <> "" Then

            Dim rpt As New ReportParameter()

            If Not rptParameter Is Nothing Then
                rpt = rptParameter
            End If

            rpt.BackToPage = backToPage
            rpt.ExportType = formato
            rpt.ConnectionCripted = OnVacContext.GetEncryptedConnectionString()
            rpt.ReportName = rptName
            rpt.Filter = rptFilter
            rpt.ShowReport(ReportParameter.GoBackTypeEnum.ResponseRedirect, ordinamento, cartella)

            Return True

        Else

            Return False

        End If

    End Function

End Class


