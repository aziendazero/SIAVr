Partial Class GestioneAlias
    Inherits OnVac.Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As Object

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If Not Request.QueryString.Item("menu_dis") Is Nothing Then
            OnVacContext.MenuDis = Request.QueryString.Item("menu_dis").ToString()
        End If

        If Request.QueryString("versione") = "2017" Then
            Response.Redirect(ResolveClientUrl(String.Format("~/hpazienti/gestionepazienti/ricercapaziente.aspx?alias=true&menu_dis={0}", OnVacContext.MenuDis)))
        Else
            Server.Transfer(ResolveClientUrl(String.Format("~/hpazienti/gestionepazienti/ricercapazientebis.aspx?alias=true&menu_dis={0}", OnVacContext.MenuDis)))
        End If

    End Sub

End Class
