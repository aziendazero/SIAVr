Imports Onit.Shared.Utility.OnitReport
Imports Onit.OnAssistnet.Web.Report
Imports Onit.OnAssistnet.Web.Report.CrystalReportUtility


Partial Class StampaReportPopUp
    Inherits OnVac.Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub
    Protected WithEvents lblPresentazione As System.Web.UI.WebControls.Label
    Protected WithEvents lblErrore As System.Web.UI.WebControls.Label
    Protected WithEvents tableErrore As System.Web.UI.HtmlControls.HtmlTable
    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(sender As System.Object, e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Properties "

    Private Property ReportParName() As String
        Get
            Return ViewState("ReportParName")
        End Get
        Set(Value As String)
            ViewState("ReportParName") = Value
        End Set
    End Property

    Private Property ReportFileName() As String
        Get
            Return Session("ReportFileName_PopUp")
        End Get
        Set(Value As String)
            Session("ReportFileName_PopUp") = Value
        End Set
    End Property

    Private Property ReportPopUp_DataSource() As System.Data.DataSet
        Get
            Return Session("ReportFileName_PopUp_dataset")
        End Get
        Set(Value As System.Data.DataSet)
            Session("ReportFileName_PopUp_dataset") = Value
        End Set
    End Property

    'per capire se la pagina ha già effettuato il secondo caricamento
    Private Property PostBack() As Boolean
        Get
            Return Convert.ToBoolean(ViewState("PostBack"))
        End Get
        Set(Value As Boolean)
            ViewState("PostBack") = Value
        End Set
    End Property

#End Region

#Region " Page Events "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        'il codice viene richiamato al secondo caricamento della pagina
        If IsPostBack Then

            If Not Me.PostBack Then

                Dim sessioneFiltro As String = Request.QueryString.Item("report")

                'questa finestra serve anche per la generazione delle ultime stampe [modifica 11/04/2005]
                If Request.QueryString.Item("stampaDirettaPdf") = "" Then
                    'recupero dei filtri
                    Dim filtroReport As String = Session(sessioneFiltro)

                    'passaggio dei parametri [modifica 03/05/2005]
                    Dim paramReport As New ArrayList()
                    paramReport = Session(sessioneFiltro & "_param")

                    'generazione del report
                    Me.GeneraReport(sessioneFiltro & ".rpt", filtroReport, paramReport)
                Else

                    'visualizzazione del pdf già creato [modifica 11/04/2005]
                    Me.ReportViewer.Attributes.Add("src", ConfigurationManager.AppSettings.Get("staWebPath").ToString & sessioneFiltro & ".pdf")
                End If

                Me.UltraWebToolbar1.Visible = True
                Me.PostBack = True

            End If
        Else
            'primo caricamento della pagina
            Me.PostBack = False
        End If

    End Sub

#End Region

#Region " Private "

    Private Sub GeneraReport(nomeReport As String, filtro As String, parametri As ArrayList)

        ' Cartella in cui è presente il report
        Dim reportFolder As String = String.Empty

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                reportFolder = bizReport.GetReportFolder(nomeReport)

            End Using
        End Using

        Dim rpt As New CrystalReportUtility(HttpContext.Current.Request.PhysicalApplicationPath + "Report\" + reportFolder + "\" + nomeReport,
                                            OnVacContext.GetEncryptedConnectionString())

        rpt.SelectionFormula = filtro
        If Not IsNothing(Me.ReportPopUp_DataSource) Then
            rpt.DataSource = Me.ReportPopUp_DataSource
        End If

        'parametri [modifica 03/05/2005]
        If Not parametri Is Nothing Then
            For count As Integer = 0 To parametri.Count - 1
                rpt.SetParameter(CStr(parametri(count)).Split("|")(0), CStr(parametri(count)).Split("|")(1))
            Next
        End If
        rpt.SetParameter("Installazione", OnVacContext.CodiceUslCorrente)

        Dim ExportType As docFormat = docFormat.PortableDocFormat

        Dim path As String = ConfigurationManager.AppSettings.Get("staPath").ToString()

        ReportFileName = rpt.Export(ExportType, "OnVac." & OnVacContext.UserCode, path)

        'brigoz 04-02-05
        'Se non lo svuoto le altre stampe che usano la popup si trovano il dataset istanziato e la stampa salta
        Me.ReportPopUp_DataSource = Nothing
        ReportViewer.Attributes.Add("src", ConfigurationManager.AppSettings.Get("staWebPath").ToString() & Me.ReportFileName)

    End Sub

#End Region

End Class
