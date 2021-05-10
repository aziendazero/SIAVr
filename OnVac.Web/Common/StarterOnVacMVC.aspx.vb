Public Class StarterOnVacMVC
    Inherits Common.PageBase

    Protected Function GetCodiceLocalePaziente() As Int64
        Dim codice As Long
        If Long.TryParse(Request.QueryString.Get("PazId"), codice) Then
            OnVacUtility.Variabili.PazId = codice.ToString()
            Return codice
        End If
        If OnVacUtility.IsPazIdEmpty() Then Return -1
        Return Convert.ToInt64(OnVacUtility.Variabili.PazId)
    End Function

    Protected Function GetUrl() As String
        Return HttpUtility.UrlEncode(Request.QueryString.Get("url"))
    End Function

    Protected Function GetAppIdCentrale() As String
        Return Me.Settings.APP_ID_CENTRALE
    End Function

    Protected Function GetAppIdLocale() As String
        Return OnVacContext.AppId
    End Function

    Protected Function GetCodiceAzienda() As String
        Return OnVacContext.Azienda
    End Function

    Protected Function GetCodiceConsultorio() As String
        Return OnVacUtility.Variabili.CNS.Codice
    End Function

    Protected Function GetUserId() As String
        Return OnVacContext.UserId.ToString()
    End Function

    Protected Function GetCodiceUslCorrente() As String
        Return OnVacContext.CodiceUslCorrente
    End Function

    Protected Function GetMainWebFolder() As String
        Return ConfigurationManager.AppSettings.Get("MainWebFolder").ToString()
    End Function

    Protected Function GetAppIdMenu() As String
        Return OnVacContext.MenAppId
    End Function

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub

End Class