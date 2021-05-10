Imports System.Collections.Generic
Imports Onit.OnAssistnet.Web.Report


<Serializable()>
Public Class ReportParameter

#Region " Public "

    Public Filter As String
    Public ExportType As CrystalReportWrapper.docFormat
    Public CompletePath As String
    Public ReportName As String
    Public BackToPage As String
    Public ConnectionCripted As String
    Public ReportFileName As String
    Public arraySortFields As ArrayList

    Public ReadOnly Property ParameterList As List(Of KeyValuePair(Of String, Object))
        Get
            Return Me.reportParameterList
        End Get
    End Property

#End Region

#Region " Private "

    Private ds_dataset As System.Data.DataSet
    Private ds_datasetSubreport As New ArrayList()
    Private ds_subreport As New ArrayList()

    Private reportParameterList As List(Of KeyValuePair(Of String, Object))

    Private controlOrder As Boolean = False

#End Region

#Region " Types "

    Public Enum GoBackTypeEnum
        HistoryBack = 0
        ResponseRedirect = 1
    End Enum

    Public Enum Ordinamento
        ASC = 0
        DESC = 1
        Nessuno = 2
    End Enum

    Private Enum Ricerca
        Tipo = 0
        CampoReport = 1
    End Enum

    Public Class StrutturaOrdinamento

        Public Structure Order
            Public NomeFormula As String
            Public NomeCampo As String
            Public TipoOrdine As Ordinamento
        End Structure

        Private ordine As ArrayList

        Public Sub New()
            ordine = New ArrayList()
        End Sub

        Public Sub AggiungiOrdinamento(nomeFormula As String, nomeCampo As String, tipoOrdine As Ordinamento)

            Dim ord As New Order()

            ord.NomeFormula = nomeFormula
            ord.NomeCampo = nomeCampo
            ord.TipoOrdine = tipoOrdine

            ordine.Add(ord)

        End Sub

        Public Function NomeFormula(index As Integer) As String
            Return DirectCast(ordine(index), Order).NomeFormula
        End Function

        Public Function NomeCampo(index As Integer) As String
            Return DirectCast(ordine(index), Order).NomeCampo
        End Function

        Public Function TipoOrdine(index As Integer) As Ordinamento
            Return DirectCast(ordine(index), Order).TipoOrdine
        End Function

        Public Function ContaOrdinamento() As Integer
            Return ordine.Count
        End Function

    End Class

#End Region

#Region " Constructors "

    Public Sub New()

        reportParameterList = New List(Of KeyValuePair(Of String, Object))()

    End Sub

#End Region

#Region " Public Methods "

    Public Sub set_dataset(ds As System.Data.DataSet)

        Me.ds_dataset = ds

    End Sub

    'passaggio del dataset al sottoreport specificato [modifica 10/03/2006]
    Public Sub set_dataset_subreport(ds As System.Data.DataSet, subreport As String)

        Me.ds_datasetSubreport.Add(ds)
        Me.ds_subreport.Add(subreport)

    End Sub

    Public Sub AddParameter(parameterName As String, parameterValue As Object)

        Me.reportParameterList.Add(New KeyValuePair(Of String, Object)(parameterName, parameterValue))

    End Sub

    Public Sub ShowReport(goBackType As GoBackTypeEnum, Optional ordinamento As StrutturaOrdinamento = Nothing, Optional cartella As String = "")

        Dim useInternalReportWrapper As String = ConfigurationManager.AppSettings.Get("useInternalReportWrapper")
        If String.IsNullOrEmpty(useInternalReportWrapper) Then
            Me.ShowReportWithShared(goBackType, ordinamento, cartella)
        Else
            Me.ShowReportWithWrapper(goBackType, ordinamento, cartella)
        End If

    End Sub

    Public Sub ShowReportWithShared(goBackType As GoBackTypeEnum, Optional ordinamento As StrutturaOrdinamento = Nothing, Optional cartella As String = "")

        Dim rpt As CrystalReportUtility =
            New CrystalReportUtility(HttpContext.Current.Request.PhysicalApplicationPath + "Report\" + cartella + "\" + Me.ReportName, Me.ConnectionCripted)

        If Not arraySortFields Is Nothing AndAlso arraySortFields.Count > 0 Then
            rpt.SetDirectionFormulaSortFields(arraySortFields)
        End If

        If rpt.SelectionFormula <> "" And Me.Filter <> "" Then
            rpt.SelectionFormula = rpt.SelectionFormula & " AND " & Me.Filter
        Else
            rpt.SelectionFormula = Me.Filter
        End If

        For Each parametroReport As KeyValuePair(Of String, Object) In Me.reportParameterList
            rpt.SetParameter(parametroReport.Key, parametroReport.Value)
        Next

        If Not IsNothing(Me.ds_dataset) Then
            rpt.DataSource = Me.ds_dataset
        End If

        If Me.ds_datasetSubreport.Count > 0 AndAlso Me.ds_subreport.Count > 0 Then
            For count As Integer = 0 To Me.ds_datasetSubreport.Count - 1
                rpt.OpenSubReport(Me.ds_subreport(count))
                rpt.SetSubReportDataSource(Me.ds_subreport(count), Me.ds_datasetSubreport(count))
            Next
        End If

        rpt.SetParameter("Installazione", OnVacContext.CodiceUslCorrente)

        Dim strPath As String = ConfigurationManager.AppSettings.Get("staPath")

        Dim reportFileName As String = rpt.Export(Me.ExportType, "OnVac." & OnVacContext.UserCode, strPath)

        Select Case goBackType

            Case GoBackTypeEnum.HistoryBack

                If HttpContext.Current.Handler.GetType().IsSubclassOf(GetType(Onit.Shared.Web.UI.Page)) Then

                    Dim p As Onit.Shared.Web.UI.Page = DirectCast(HttpContext.Current.Handler, Onit.Shared.Web.UI.Page)
                    p.HistoryClear()
                    p.HistoryRedirect(String.Format("{0}/stampe/AnteprimaReport.aspx?RPT={1}&BTP={2}&hib={3}",
                                                    HttpContext.Current.Request.ApplicationPath,
                                                    HttpUtility.UrlEncode(reportFileName),
                                                    HttpUtility.UrlEncode(Me.BackToPage),
                                                    True))

                Else
                    Throw New UnauthorizedAccessException("La classe ReportParameter e la pagina AnteprimaReport sono accessibili solo se il chiamante eredita da Onit.Shared.Web.UI.Page")
                End If

            Case GoBackTypeEnum.ResponseRedirect

                HttpContext.Current.Response.Redirect(String.Format("{0}/stampe/AnteprimaReport.aspx?RPT={1}&BTP={2}&hib={3}",
                                                                    HttpContext.Current.Request.ApplicationPath,
                                                                    HttpUtility.UrlEncode(reportFileName),
                                                                    HttpUtility.UrlEncode(Me.BackToPage),
                                                                    False), False)

            Case Else
                Throw New NotImplementedException("GoBackTypeEnum value non riconosciuto")

        End Select

    End Sub

    Public Sub ShowReportWithWrapper(goBackType As GoBackTypeEnum, Optional ordinamento As StrutturaOrdinamento = Nothing, Optional cartella As String = "")

        Dim rpt As CrystalReportWrapper = New CrystalReportWrapper(HttpContext.Current.Request.PhysicalApplicationPath + "Report\" + cartella + "\" + Me.ReportName, Me.ConnectionCripted, True)

        If Not arraySortFields Is Nothing AndAlso arraySortFields.Count > 0 Then
            rpt.SetDirectionFormulaSortFields(arraySortFields)
        End If

        If rpt.SelectionFormula <> "" And Me.Filter <> "" Then
            rpt.SelectionFormula = rpt.SelectionFormula & " AND " & Me.Filter
        Else
            rpt.SelectionFormula = Me.Filter
        End If

        For Each parametroReport As KeyValuePair(Of String, Object) In Me.reportParameterList
            rpt.SetParameter(parametroReport.Key, parametroReport.Value)
        Next

        If Not IsNothing(Me.ds_dataset) Then
            rpt.DataSource = Me.ds_dataset
        End If

        If Me.ds_datasetSubreport.Count > 0 AndAlso Me.ds_subreport.Count > 0 Then
            For count As Integer = 0 To Me.ds_datasetSubreport.Count - 1
                rpt.OpenSubReport(Me.ds_subreport(count))
                rpt.SetSubReportDataSource(Me.ds_subreport(count), Me.ds_datasetSubreport(count))
            Next
        End If

        rpt.SetParameter("Installazione", OnVacContext.CodiceUslCorrente)

        Dim strPath As String = ConfigurationManager.AppSettings.Get("staPath")

        Dim reportFileName As String = rpt.Export(Me.ExportType, "OnVac." & OnVacContext.UserCode, strPath)

        Select Case goBackType

            Case GoBackTypeEnum.HistoryBack

                If HttpContext.Current.Handler.GetType().IsSubclassOf(GetType(Onit.Shared.Web.UI.Page)) Then

                    Dim p As Onit.Shared.Web.UI.Page = DirectCast(HttpContext.Current.Handler, Onit.Shared.Web.UI.Page)
                    p.HistoryClear()
                    p.HistoryRedirect(String.Format("{0}/stampe/AnteprimaReport.aspx?RPT={1}&BTP={2}&hib={3}",
                                                    HttpContext.Current.Request.ApplicationPath,
                                                    HttpUtility.UrlEncode(reportFileName),
                                                    HttpUtility.UrlEncode(Me.BackToPage),
                                                    True))

                Else
                    Throw New UnauthorizedAccessException("La classe ReportParameter e la pagina AnteprimaReport sono accessibili solo se il chiamante eredita da Onit.Shared.Web.UI.Page")
                End If

            Case GoBackTypeEnum.ResponseRedirect

                HttpContext.Current.Response.Redirect(String.Format("{0}/stampe/AnteprimaReport.aspx?RPT={1}&BTP={2}&hib={3}",
                                                                    HttpContext.Current.Request.ApplicationPath,
                                                                    HttpUtility.UrlEncode(reportFileName),
                                                                    HttpUtility.UrlEncode(Me.BackToPage),
                                                                    False), False)

            Case Else
                Throw New NotImplementedException("GoBackTypeEnum value non riconosciuto")

        End Select

    End Sub

#End Region

End Class
