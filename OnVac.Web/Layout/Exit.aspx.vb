Partial Class _Exit
    Inherits System.Web.UI.Page
    Protected WithEvents txtVai As System.Web.UI.WebControls.TextBox

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

    <Global.System.Diagnostics.DebuggerNonUserCodeAttribute()>
    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        '--
        Try
            '--
            ' N.B. : non sostituire con OnVacContext.UserId => se è Nothing tenterebbe di valorizzarla!
            '--
            Dim usr As Onit.Shared.Manager.Entities.T_ANA_UTENTI = Onit.Shared.Manager.Security.UserDbManager.GetCurrentUser()
            If Not usr Is Nothing Then
                '--
                Dim lck As New Onit.Controls.OnitShared.PageLock(Context, Nothing)
                lck.DeleteLockForUser(usr.UTE_ID)
                '--
            End If
            '--
        Catch ex As Exception
        Finally
            '--
            Try
                System.Web.Profile.ProfileManager.DeleteProfile(Me.User.Identity.Name)
            Catch ex As Exception
            Finally
                '--
                Session.Clear()
                Session.Abandon()
                '--
            End Try
            '--
        End Try
        '--
    End Sub

End Class
