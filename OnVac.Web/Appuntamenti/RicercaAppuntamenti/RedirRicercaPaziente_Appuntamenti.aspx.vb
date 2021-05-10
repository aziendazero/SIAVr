Partial Class RedirRicercaPaziente_Appuntamenti1
    Inherits OnVac.Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsNothing(Request.QueryString.Item("menu_dis")) Then
            OnVacContext.MenuDis = Request.QueryString.Item("menu_dis").ToString()
        End If

        Dim strScript As String = "document.location='RicercaAppuntamenti.aspx';"

        CustomEventUtility.HandleEventsToMessage(Page, "CambiaLeftBar", strScript)

    End Sub

End Class
