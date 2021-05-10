Partial Class OpenConsensi
    Inherits Common.PageBase

    Protected Function GetCryptedUser() As String

        Dim crypto As New Onit.Shared.NTier.Security.Crypto(Onit.Shared.NTier.Security.Providers.Rijndael)

        Dim strRiCriptata As String = crypto.Encrypt(Request.QueryString.Get("user"), Me.Settings.CONSENSO_KEY)
        'strRiCriptata = Me.ApplyEscapeJS(strRiCriptata)
        strRiCriptata = HttpUtility.JavaScriptStringEncode(strRiCriptata)

        Return strRiCriptata

    End Function

    Protected Function GetPaziente() As String
        Return Request.QueryString.Get("paziente")
    End Function

    Protected Function GetUrlConsensi() As String
        Return Me.Settings.CONSENSO_URL
    End Function

    Protected Function GetAppIdConsenso() As String
        Return Me.Settings.CONSENSO_APP_ID
    End Function

    Protected Function GetFlagEditMode() As String

        Dim editMode As String = Request.QueryString.Get("edit")
        If (Not String.IsNullOrEmpty(editMode)) Then
            Return "&edit=" + editMode
        End If

        Return String.Empty

    End Function

    Protected Function GetCodiceAziendaRegistrazione() As String
        Return String.Format("&azi={0}", OnVacUtility.GetCodiceAziendaRegistrazione(Me.Settings))
    End Function

    Protected Function GetIdConsensiNonVisibili() As String

        If Not Me.Settings.CONSENSO_ID_NON_VISIBILI Is Nothing AndAlso Me.Settings.CONSENSO_ID_NON_VISIBILI.Count > 0 Then

            Return String.Format("&hide={0}", String.Join("|", Me.Settings.CONSENSO_ID_NON_VISIBILI.ToArray()))

        End If

        Return String.Empty

    End Function

End Class