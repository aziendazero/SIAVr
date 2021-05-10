Partial Class RedirRicercaPaziente_PS
    Inherits Common.PageBase

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


    Private _CodiceMedicoUtenteLoggato As String
    Private ReadOnly Property CodiceMedicoUtenteLoggato()
        Get
            If _CodiceMedicoUtenteLoggato Is Nothing Then

                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                    _CodiceMedicoUtenteLoggato = genericProvider.Utenti.GetMedicoDaUtente(OnVacContext.UserId)
                End Using

            End If

            Return _CodiceMedicoUtenteLoggato
        End Get
    End Property

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsNothing(Request.QueryString.Item("menu_dis")) Then
            OnVacContext.MenuDis = Request.QueryString.Item("menu_dis").ToString()
        End If

        If Me.CodiceMedicoUtenteLoggato = String.Empty Then
            Throw New Exception("Impossibile risalire al medico associato all'utente!")
        Else
            Server.Transfer(String.Format("../Pronto Soccorso/RicPazPS.aspx?LoadLeftFramePS=True"))
        End If

    End Sub

End Class
