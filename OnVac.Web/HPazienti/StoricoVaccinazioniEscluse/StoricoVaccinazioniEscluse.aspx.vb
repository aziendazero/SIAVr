Public Class StoricoVaccinazioniEscluse
    Inherits OnVac.Common.PageBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        OnVacUtility.ImpostaIntestazioniPagina(OnitLayout31, LayoutTitolo, Nothing, Settings, IsGestioneCentrale)

        LoadStoricoRinnovi()

    End Sub

    Private Sub LoadStoricoRinnovi()
        'eseguo la ricerca e fisso la ricerca sulla prima pagina 
        StoricoRinnovi.Inizializza(String.Empty)
    End Sub

End Class