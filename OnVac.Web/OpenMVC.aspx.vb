Partial Class OpenMVC
    Inherits Common.PageBase

    Protected Function GetPaziente() As String
        Return OnVacUtility.Variabili.PazId()
    End Function

    Protected Function GetUrl() As String
        Return Request.QueryString.Get("url")
    End Function

    Protected Function GetAppId() As String
        Return Me.Settings.APP_ID_CENTRALE
    End Function

End Class