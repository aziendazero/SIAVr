Imports Onit.Shared.Xmpi


Public Class BizXmpi

    Public Shared Function mapOnProperty(pazienteCentrale As PazienteCentrale) As proprietaCollection

        Dim campi As New proprietaCollection()

        If Not pazienteCentrale.codiceFiscale Is Nothing AndAlso pazienteCentrale.codiceFiscale = "" Then

            ' se sbianco il codice fiscale, sbianco anche l'stp
            campi.Add("codiceSTP", "")
            campi.Add("codiceFiscale", "")

        Else

            'altrimenti scelgo se aggiornare per stp o codice fiscale
            If Not pazienteCentrale.codiceFiscale Is Nothing Then
                'il codice fiscale è valorizzato

                If IsStp(pazienteCentrale.codiceFiscale) Then
                    campi.Add("codiceSTP", pazienteCentrale.codiceFiscale)
                    campi.Add("codiceFiscale", "")
                Else
                    campi.Add("codiceFiscale", pazienteCentrale.codiceFiscale)
                    campi.Add("codiceSTP", "")
                End If
            End If

        End If

        campi.AddIfNotNull("numeroTesseraSanitaria", pazienteCentrale.Tessera)
        campi.Add("idPaziente", pazienteCentrale.codice)
        campi.AddIfNotNull("cognomePaziente", pazienteCentrale.Cognome)
        campi.AddIfNotNull("nomePaziente", pazienteCentrale.Nome)
        campi.AddIfNotNull("dataNascita", fmtStrDate(pazienteCentrale.DataNascita))
        campi.AddIfNotNull("codiceComuneNascita", pazienteCentrale.ComCodiceNascita)
        campi.AddIfNotNull("sesso", pazienteCentrale.Sesso)
        campi.AddIfNotNull("codiceComuneResidenza", pazienteCentrale.ResidenzaComCodice)
        campi.AddIfNotNull("indirizzoResidenza", pazienteCentrale.ResidenzaIndirizzo)
        campi.AddIfNotNull("capResidenza", pazienteCentrale.ResidenzaCap)
        campi.AddIfNotNull("codiceComuneDomicilio", pazienteCentrale.DomicilioComCodice)
        campi.AddIfNotNull("indirizzoDomicilio", pazienteCentrale.DomicilioIndirizzo)
        campi.AddIfNotNull("capDomicilio", pazienteCentrale.DomicilioCap)
        campi.AddIfNotNull("aslAppartenenza", pazienteCentrale.ResidenzaUslCodice)
        campi.AddIfNotNull("codiceCittadinanza", pazienteCentrale.CitCodice)
        campi.AddIfNotNull("numeroTelefono1", pazienteCentrale.Telefono1)
        campi.AddIfNotNull("numeroTelefono2", pazienteCentrale.Telefono2)
        campi.AddIfNotNull("numeroTelefono3", pazienteCentrale.Telefono3)
        campi.AddIfNotNull("codiceMedico", pazienteCentrale.MedCodiceBase)
        campi.AddIfNotNull("dataDecesso", fmtStrDate(pazienteCentrale.DataDecesso))
        campi.AddIfNotNull("scadenzaTessera", fmtStrDate(pazienteCentrale.UslAssistenzaDataCessazione)) '*
        campi.AddIfNotNull("distrettoResidenza", pazienteCentrale.DisCodice)
        campi.AddIfNotNull("dataEmigrazione", fmtStrDate(pazienteCentrale.EmigrazioneData))
        campi.AddIfNotNull("codiceComuneEmigrazione", pazienteCentrale.EmigrazioneComCodice)
        campi.AddIfNotNull("dataImmigrazione", fmtStrDate(pazienteCentrale.ImmigrazioneData))
        campi.AddIfNotNull("codiceComuneImmigrazione", pazienteCentrale.ImmigrazioneComCodice)
        campi.AddIfNotNull("codiceStatoCivile", pazienteCentrale.StatoCivile)
        campi.AddIfNotNull("motivoCessazioneAssistenza", pazienteCentrale.UslAssistenzaMotivoCessazione)
        campi.AddIfNotNull("codiceComuneDecesso", pazienteCentrale.ComCodiceDecesso)
        campi.AddIfNotNull("codicePosizioneProfessionaleSacs", pazienteCentrale.CodiceProfessione) ' la professione è intesa come posizione professionale
        campi.AddIfNotNull("identificativoAnagrafeComunale", pazienteCentrale.CodiceDemografico) '*
        campi.AddIfNotNull("flagCEE", pazienteCentrale.CEE)
        campi.AddIfNotNull("aslAssistenza", pazienteCentrale.UslAssistenzaCodice)
        campi.AddIfNotNull("zonaSubcomunale", pazienteCentrale.DomicilioLocalita) '*
        campi.AddIfNotNull("quartiere", pazienteCentrale.ResidenzaLocalita) '*
        campi.AddIfNotNull("aslProvenienza", pazienteCentrale.UslProvenienza)
        campi.AddIfNotNull("cognomeConiuge", pazienteCentrale.Note) '*
        'campi.Add("", paz.CfisValidato)'non trattato
        'campi.Add("", paz.CfisCertificatore)'non rimappabile

        campi.AddIfNotNull("regioneAppartenenza", pazienteCentrale.CodiceRegione)
        campi.AddIfNotNull("codiceSituazioneProfessionaleSacs", pazienteCentrale.SituazioneProfessionale)  'ignorato per il momento
        campi.AddIfNotNull("nomeDatoreLavoro", pazienteCentrale.DatoreLavoro)
        campi.AddIfNotNull("codiceTipoAzienda", pazienteCentrale.TipoAzienda)
        campi.AddIfNotNull("codiceAttivitaProfessionaleSacs", pazienteCentrale.RamoAttivita)
        campi.AddIfNotNull("codiceTipoAziendaSacs", pazienteCentrale.CodiceAzienda)
        campi.AddIfNotNull("codiceComuneDatoreLavoro", pazienteCentrale.ComCodiceAzienda)


        Return campi

    End Function

    'controlla se il codice fiscale è un stp
    Private Shared Function IsStp(codiceFiscale As String) As Boolean

        If codiceFiscale Is Nothing Then
            Return False
        End If

        Return System.Text.RegularExpressions.Regex.IsMatch(codiceFiscale, "STP[0-9].+", System.Text.RegularExpressions.RegexOptions.IgnoreCase)

    End Function

    Public Shared Function update(paz As PazienteCentrale, strUtente As String, strApplicativo As String) As String

        Dim strEntita As String = Nothing
        Dim strUser As String = Nothing
        Dim strPassword As String = Nothing
        Dim strErr As String = Nothing

        Dim man As New JBFManager()

        Select Case strApplicativo.ToUpper().Trim()

            Case "ONAMB", "ONAMBV2"
                strEntita = "ON-AMB"
                strUser = "USR_ON-AMB"
                strPassword = ""

            Case "ONVAC"
                strEntita = "ON-VAC"
                strUser = "USR_ON-VAC"
                strPassword = ""

            Case "ONMED", "ONASLGACNEW"
                strEntita = "MDB"
                strUser = "USR_MDB"
                strPassword = ""

            Case "ONREP"
                strEntita = "ONREP"
                strUser = "USR_ONREP"
                strPassword = ""

            Case "ONSAS"
                strEntita = "ON-SAS"
                strUser = "USR_ON-SAS"
                strPassword = ""

            Case "VANASL"
                strEntita = "VANASL"
                strUser = "USR_VANASL"
                strPassword = ""

            Case "ALLINEA"
                strEntita = "ALLINEA"
                strUser = "USR_ALLIN"
                strPassword = ""

            Case "ONASLGACNEWCEDAP"
                strEntita = "ONGAC2"
                strUser = "OnAslGacNewCEDAP"
                strPassword = ""

            Case Else
                strErr = "Errore: " + strApplicativo + " applicativo sconosciuto"

        End Select

        If Not strErr Is Nothing Then
            Return strErr
        End If

        'apertura della sessione
        Try
            If Not man.Open(strUser, strPassword, strEntita) Then
                strErr = "Errore nell'apertura della sessione xmpi"
            End If
        Catch ex As Exception
            strErr = ex.Message + vbCrLf + ex.StackTrace
        End Try

        If Not strErr Is Nothing Then
            Return strErr
        End If

        'esecuzione dell'update
        Dim parametri As New paramCollection()
        parametri.Add("findAliases", "false") 'non invia la lista degli alias

        Dim incampi As Onit.Shared.Xmpi.proprietaCollection = mapOnProperty(paz)
        Dim outcampi As Onit.Shared.Xmpi.proprietaCollection = Nothing

        strErr = man.update(incampi, parametri, "sianc.paziente.PazienteBean", outcampi)

        If strErr Is Nothing OrElse strErr = String.Empty Then
            If paz.codice Is Nothing OrElse paz.codice = String.Empty Then
                If Not outcampi Is Nothing Then

                    Dim propTempResp As proprieta = outcampi.getByName("idPaziente")

                    If Not propTempResp Is Nothing Then
                        paz.codice = propTempResp.valore
                    End If

                End If
            End If
        End If

        'chiusura della sessione
        man.Close()

        Return strErr

    End Function

    Private Shared Function fmtStrDate(objDate As Object) As String

        If objDate Is Nothing Then
            Return Nothing
        ElseIf objDate.GetType Is GetType(String) AndAlso objDate.ToString().Trim() = String.Empty Then
            Return ""
        Else
            Return CDate(objDate).ToString("yyyyMMdd")
        End If

    End Function

End Class
