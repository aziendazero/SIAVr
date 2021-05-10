Partial Class RedirRicercaPaziente_Pazienti
    Inherits Common.PageBase

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

        If Not Request.QueryString("menu_dis") Is Nothing Then
            OnVacContext.MenuDis = Request.QueryString("menu_dis").ToString()
        End If

        ' Check della sessione
        OnVacContext.AssertSession()

        Dim queryString As New Text.StringBuilder()

        If IsGestioneCentrale Then
            queryString.AppendFormat("isCentrale={0}", IsGestioneCentrale.ToString())
        End If

        If queryString.Length > 0 Then queryString.Insert(0, "?")

        If Request.QueryString("versione") = "2017" Then
            Response.Redirect(String.Format("./hpazienti/gestionepazienti/ricercapaziente.aspx{0}", queryString.ToString()))
        Else
            Server.Transfer(String.Format("./hpazienti/gestionepazienti/ricercapazientebis.aspx{0}", queryString.ToString()))
        End If

    End Sub

End Class