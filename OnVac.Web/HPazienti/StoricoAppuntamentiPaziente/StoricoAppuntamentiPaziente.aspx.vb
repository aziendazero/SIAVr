Public Class StoricoAppuntamentiPaziente
    Inherits OnVac.Common.PageBase

#Region " Page Events "

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                OnVacUtility.ImpostaIntestazioniPagina(Me.OnitLayout31, Me.LayoutTitolo, genericProvider, Me.Settings, Me.IsGestioneCentrale)

                Me.ucStoricoAppuntamenti.LoadStoricoAppuntamenti(Convert.ToInt64(OnVacUtility.Variabili.PazId), Nothing)

            End Using

        End If

    End Sub

#End Region

End Class