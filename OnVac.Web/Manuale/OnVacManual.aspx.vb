Partial Class OnVac_OnVacManual
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

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then
            Me.OpenLeftFrame(False)
            Me.IFManuale.Attributes("src") = String.Format("./{0}", Me.Request.QueryString("ManualFileName"))
        End If

    End Sub

    Private Sub RenderScript(strAbilita As String)
        Dim str As String = String.Empty
        str &= "<script language=javascript> "
        str &= "top.frames[1].location=""" & Request.ApplicationPath & "/Layout/LeftFrame.aspx?AppId=" + OnVacContext.AppId + "&men_id=OnVacManual&enable=" & strAbilita & """;"
        str &= "</script>"
        Response.Write(str)
    End Sub

End Class
