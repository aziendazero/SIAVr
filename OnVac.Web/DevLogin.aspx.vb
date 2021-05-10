Imports System.Web.Security
Imports Onit.Shared.Manager.Entities
Imports Onit.Shared.Manager.OnitProfile
Imports Onit.Shared.Manager.Security

Public Class DevLogin
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Dim utente As String = ConfigurationManager.AppSettings.Get("DevLoginUtente")
        Dim dominio As String = ConfigurationManager.AppSettings.Get("DevLoginDominio")
        Dim azienda As String = ConfigurationManager.AppSettings.Get("DevLoginAzienda")

        Dim ute As T_ANA_UTENTI = UserDbManager.GetUser(utente, azienda)
        LoginInternal(dominio, ute)
    End Sub

    Protected Sub LoginInternal(dom As String, utente As T_ANA_UTENTI)
        Dim azienda As String = utente.T_ANA_AZIENDE.AZI_CODICE
        Dim currentProfile As WebProfile = WebProfile.Current

        currentProfile.SetPropertyValue("Azienda", azienda)
        currentProfile.SetPropertyValue("AziendaImp", azienda)
        currentProfile.SetPropertyValue("Dominio", dom)
        currentProfile.Save()

        FormsAuthentication.SetAuthCookie(utente.UTE_ID.ToString(), False)
        HttpContext.Current.Response.AppendCookie(New HttpCookie("OnPortalUserID", utente.UTE_ID.ToString()))

        Dim url As String = ConfigurationManager.AppSettings.Get("DevLoginUrl")
        Response.Redirect(url)
    End Sub

End Class