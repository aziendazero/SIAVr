Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.Security
Imports System.Web.Profile
Imports System.Configuration.Provider
Imports System.Text

Imports Onit.Shared.NTier
Imports Onit.Shared.Manager.Entities
Imports Onit.Shared.Manager.OnitProfile
Imports Onit.Shared.Manager.Security
Imports Onit.Shared.Manager.Exceptions


Public Class Login
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim azienda As String = Onit.Shared.NTier.OnitNTierAppRuntime.Settings.OnitManager.Azienda

        Membership.ApplicationName = azienda
        Roles.ApplicationName = azienda

        Dim domain As String = Me.txtDominio.Text
        Dim username As String = (domain & If(String.IsNullOrEmpty(domain), "", "\") & Me.txtUserName.Text)

        Membership.ValidateUser(username, Me.txtPassword.Text)
        Dim utente As T_ANA_UTENTI = If(String.IsNullOrEmpty(domain),
                                        UserDbManager.GetUser(Me.txtUserName.Text, azienda),
                                        UserDbManager.GetUser(Me.txtUserName.Text, azienda, domain))
        Dim propertyValue As String = utente.T_ANA_AZIENDE.AZI_CODICE
        Dim aziCodice As String = utente.T_ANA_AZIENDE.AZI_CODICE
        Dim authCookie As HttpCookie = FormsAuthentication.GetAuthCookie(utente.UTE_CODICE, False)

        authCookie.Name = ".PORTALE20"
        HttpContext.Current.Response.Cookies.Add(authCookie)
        Dim cookie3 As New HttpCookie("AUTHUSER", utente.UTE_CODICE)
        HttpContext.Current.Response.Cookies.Add(cookie3)

        UserDbManager.InitOnitSession(utente.UTE_ID, aziCodice)
        If (Not WebProfile.Current Is Nothing) Then
            Dim current As ProfileBase = ProfileBase.Create(utente.UTE_ID.ToString, True)
            current.SetPropertyValue("Azienda", aziCodice)
            current.SetPropertyValue("AziendaImp", aziCodice)
            current.SetPropertyValue("Dominio", domain)
            current.Save()
        End If

        FormsAuthentication.SetAuthCookie(utente.UTE_ID.ToString, False)

        HttpContext.Current.Response.Redirect(HttpContext.Current.Request.QueryString("RedirectUrl"))

    End Sub

End Class