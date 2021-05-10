Public Class LandingPage
    Inherits Common.PageBase

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        Dim cp As String = Request.QueryString.Get("cp")
        Dim w As String = Request.QueryString.Get("w")

        If String.IsNullOrWhiteSpace(w) Then
            Throw New Exception("Punto di arrivo non specificato")
        End If

        If Not String.IsNullOrWhiteSpace(cp) Then
            UltimoPazienteSelezionato = New Entities.UltimoPazienteSelezionato(String.Empty, cp)
        End If

        Select Case w

            Case "A"

                If String.IsNullOrWhiteSpace(cp) Then
                    Throw New Exception("Paziente non specificato")
                End If

                RedirectToGestionePaziente(cp)

            Case "B"

                OnVacContext.MenuDis = GetMenuDis_Appuntamenti()

                Dim qs As String = String.Empty
                Dim from As String = Request.QueryString.Get("fromCruscotto")

                If Not String.IsNullOrWhiteSpace(from) Then
                    qs = "?fromCruscotto=S"
                End If

                Response.Redirect(ResolveClientUrl(String.Format("~/Appuntamenti/Calendario/Calendario.aspx{0}", qs)), False)

            Case "C"

                OnVacUtility.Variabili.PazId = cp

                OnVacContext.MenuDis = GetMenuDis_HPazienti()
                Dim qs As String = String.Empty
                Dim from As String = Request.QueryString.Get("fromCruscotto")

                If Not String.IsNullOrWhiteSpace(from) Then
                    qs = "?fromCruscotto=S"
                End If

                Response.Redirect(ResolveClientUrl(String.Format("~/HPazienti/Inadempienze/Inadempienze.aspx{0}", qs)), False)

            Case "D"

                OnVacUtility.Variabili.PazId = cp

                OnVacContext.MenuDis = GetMenuDis_HPazienti()
                Dim qs As String = String.Empty
                Dim from As String = Request.QueryString.Get("fromCruscotto")

                If Not String.IsNullOrWhiteSpace(from) Then
                    qs = "?fromCruscotto=S"
                End If

                Response.Redirect(ResolveClientUrl(String.Format("~/HPazienti/ReazAvverse/ReazAvverse.aspx{0}", qs)), False)

        End Select

    End Sub

End Class