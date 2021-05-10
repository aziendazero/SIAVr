Imports System
Imports System.Web


Partial Class AnteprimaReport
    Inherits OnVac.Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(sender As System.Object, e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Properties "

    Private ReadOnly Property ReportFileName() As String
        Get
            Return Me.Request.QueryString("RPT")
        End Get
    End Property

    Private ReadOnly Property BackToPage() As String
        Get
            Return Me.Request.QueryString("BTP")
        End Get
    End Property

    Protected ReadOnly Property CompletePath() As String
        Get
            Return ConfigurationManager.AppSettings.Get("staWebPath").ToString & Me.ReportFileName
        End Get
    End Property

    Private ReadOnly Property HistoryBack() As Boolean
        Get
            Return Me.Request.QueryString("hib").toLower = "true"
        End Get
    End Property

#End Region

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            Me.ReportViewer.Attributes.Add("src", Me.CompletePath)

        End If

    End Sub

    Protected Overrides Sub MyHistoryClear()

        ''' Non deve eseguire il clear

    End Sub

    Private Sub UltraWebToolbar1_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles UltraWebToolbar1.ButtonClicked

        Select Case e.Button.Key

            Case "btnChiudi"

                If HistoryBack Then
                    DirectCast(Me.Page, Onit.Shared.Web.UI.Page).HistoryNavigateBack()
                Else
                    Response.Redirect(Me.BackToPage, False)
                End If

        End Select

    End Sub

End Class
