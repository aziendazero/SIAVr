Imports System.Collections.Generic

Namespace Common

    Public Class ReazioniAvverseCommon

#Region " Types "

#Region " EventArgs "

        Public Class ReplicaEventArgs
            Inherits EventArgs

            Public Property FarmacoDaReplicare As FarmacoInfo

            Public Sub New(farmacoDaReplicare As FarmacoInfo)
                Me.FarmacoDaReplicare = farmacoDaReplicare
            End Sub

        End Class

        Public Class ShowMessageReazioniEventArgs
            Inherits EventArgs

            Public Property Message As String

            Public Sub New(message As String)
                Me.Message = message
            End Sub

        End Class

#End Region

        Public Class CssName
            Public Const TextBox_Stringa As String = "TextBox_Stringa"
            Public Const TextBox_Stringa_Obbligatorio As String = "TextBox_Stringa_Obbligatorio"
            Public Const TextBox_Stringa_Disabilitato As String = "TextBox_Stringa_Disabilitato"

            Public Const TextBox_Data As String = "TextBox_Data"
            Public Const TextBox_Data_Obbligatorio As String = "TextBox_Data_Obbligatorio"
            Public Const TextBox_Data_Disabilitato As String = "TextBox_Data_Disabilitato"

            Public Const TextBox_Numerico As String = "TextBox_Numerico"
            Public Const TextBox_Numerico_Obbligatorio As String = "TextBox_Numerico_Obbligatorio"
            Public Const TextBox_Numerico_Disabilitato As String = "TextBox_Numerico_Disabilitato"
        End Class

        <Serializable>
        Public Class FarmacoInfo

            Public Property Ordinal As Integer
            Public Property CodiceNomeCommerciale As String
            Public Property DescrizioneNomeCommerciale As String
            Public Property CodiceLotto As String
            Public Property DataScadLotto As Date?
            Public Property DataOraEsecuzioneVaccinazione As DateTime?
            Public Property CodiceSitoInoculazione As String
            Public Property DescrizioneSitoInoculazione As String
            Public Property CodiceViaSomministrazione As String
            Public Property DescrizioneViaSomministrazione As String
            Public Property Dose As Integer?
            Public Property Sospeso As String
            Public Property ReazioneMigliorata As String
            Public Property Ripreso As String
            Public Property RicomparsiSintomi As String
            Public Property Indicazioni As String
            Public Property CodiceIndicazioni As String
            Public Property DosaggioFiale As String
            Public Property Richiamo As Integer?
            Public Property VesId As Long?

        End Class

        <Serializable>
        Public Class FarmacoRecupero
            Public Property DataOraEsecuzioneVaccinazione As DateTime
            Public Property CodiceAssociazione As String
            Public Property DescrizioneAssociazione As String
            Public Property DoseAssociazione As Integer
            Public Property DescrizioneNomeCommerciale As String
            Public Property CodiceLotto As String
            Public Property CodiceSitoInoculazione As String
            Public Property DescrizioneSitoInoculazione As String
            Public Property CodiceViaSomministrazione As String
            Public Property DescrizioneViaSomministrazione As String
            Public Property IsFittizia As Boolean
            Public Property VesId As Long
            Public Property DataScadenzaLotto As Date?
            Public Property CodiceNomeCommerciale As String
        End Class

        Public Class CheckResult

            Public Success As Boolean
            Public Message As String

            Public Sub New(success As Boolean, message As String)
                Me.Success = success
                Me.Message = message
            End Sub

        End Class

#End Region

#Region " Public "

        Public Shared Function DataTableToListFarmacoRecupero(dt As DataTable) As List(Of FarmacoRecupero)

            Dim list As New List(Of FarmacoRecupero)()

            If Not dt Is Nothing Then

                For Each row As DataRow In dt.Rows

                    Dim item As New FarmacoRecupero()

                    item.DataOraEsecuzioneVaccinazione = Convert.ToDateTime(row("ves_dataora_effettuazione"))

                    item.CodiceAssociazione = GetStringFromRowCell(row, "ves_ass_codice")
                    item.DescrizioneAssociazione = GetStringFromRowCell(row, "ass_descrizione")
                    item.DescrizioneNomeCommerciale = GetStringFromRowCell(row, "noc_descrizione")
                    item.CodiceNomeCommerciale = GetStringFromRowCell(row, "ves_noc_codice")
                    item.CodiceLotto = GetStringFromRowCell(row, "ves_lot_codice")
                    If row.IsNull("lot_data_scadenza") Then
                        item.DataScadenzaLotto = Nothing
                    Else
                        item.DataScadenzaLotto = Convert.ToDateTime(row("lot_data_scadenza"))
                    End If

                    item.CodiceSitoInoculazione = GetStringFromRowCell(row, "ves_sii_codice")
                    item.DescrizioneSitoInoculazione = GetStringFromRowCell(row, "sii_descrizione")
                    item.CodiceViaSomministrazione = GetStringFromRowCell(row, "ves_vii_codice")
                    item.DescrizioneViaSomministrazione = GetStringFromRowCell(row, "vii_descrizione")

                    If row.IsNull("ves_ass_n_dose") Then
                        item.DoseAssociazione = 0
                    Else
                        item.DoseAssociazione = Convert.ToInt32(row("ves_ass_n_dose"))
                    End If

                    If row.IsNull("ves_flag_fittizia") Then
                        item.IsFittizia = False
                    Else
                        item.IsFittizia = (Convert.ToString(row("ves_flag_fittizia")) = "S")
                    End If

                    item.VesId = Convert.ToInt64(row("ves_id"))

                    list.Add(item)

                Next

            End If

            Return list

        End Function

        ''' <summary>
        ''' Aggiorna la row del datatable delle eseguite utilizzando la entity reazione specificata.
        ''' </summary>
        ''' <param name="row"></param>
        ''' <remarks></remarks>
        Public Shared Sub UpdateDataRowByEntity(row As DataRow, reazioneAvversa As Entities.ReazioneAvversa)

            row("vra_rea_codice") = reazioneAvversa.CodiceReazione
            row("rea_descrizione") = reazioneAvversa.DescrizioneReazione
            row("vra_re1_codice") = reazioneAvversa.CodiceReazione1
            row("rea_descrizione1") = reazioneAvversa.DescrizioneReazione1
            row("vra_re2_codice") = reazioneAvversa.CodiceReazione2
            row("rea_descrizione2") = reazioneAvversa.DescrizioneReazione2
            row("vra_visita") = reazioneAvversa.VisiteRicoveri
            row("vra_terapia") = reazioneAvversa.Terapie
            row("vra_rea_altro") = reazioneAvversa.AltraReazione
            row("vra_gravita_reazione") = reazioneAvversa.GravitaReazione
            row("vra_grave") = reazioneAvversa.Grave
            row("vra_esito") = reazioneAvversa.Esito

            If reazioneAvversa.DataEsito = Date.MinValue Then
                row("vra_data_esito") = DBNull.Value
            Else
                row("vra_data_esito") = reazioneAvversa.DataEsito
            End If

            row("vra_motivo_decesso") = reazioneAvversa.MotivoDecesso
            row("vra_data_reazione") = reazioneAvversa.DataReazione
            row("vra_luogo") = reazioneAvversa.LuogoDescrizione
            row("vra_altro_luogo") = reazioneAvversa.AltroLuogoDescrizione
            row("vra_farmaco_concomitante") = reazioneAvversa.FarmacoConcomitante
            row("vra_farmaco_descrizione") = reazioneAvversa.FarmacoDescrizione
            row("vra_uso_concomitante") = reazioneAvversa.UsoConcomitante
            row("vra_condizioni_concomitanti") = reazioneAvversa.CondizioniConcomitanti
            row("vra_qualifica") = reazioneAvversa.Qualifica
            row("vra_altra_qualifica") = reazioneAvversa.AltraQualifica
            row("vra_cognome_segnalatore") = reazioneAvversa.CognomeSegnalatore
            row("vra_nome_segnalatore") = reazioneAvversa.NomeSegnalatore
            row("vra_indirizzo_segnalatore") = reazioneAvversa.IndirizzoSegnalatore
            row("vra_tel_segnalatore") = reazioneAvversa.TelSegnalatore
            row("vra_fax_segnalatore") = reazioneAvversa.FaxSegnalatore
            row("vra_mail_segnalatore") = reazioneAvversa.MailSegnalatore
            row("vra_data_compilazione") = reazioneAvversa.DataCompilazione
            row("vra_ute_id_variazione") = reazioneAvversa.IdUtenteModifica
            row("vra_data_variazione") = reazioneAvversa.DataModifica
            row("vra_usl_inserimento") = reazioneAvversa.CodiceUslInserimento
            row("usl_inserimento_vra_descr") = reazioneAvversa.DescrizioneUslInserimento

            row("vra_altre_informazioni") = reazioneAvversa.AltreInformazioni
            row("vra_ambito_osservazione") = reazioneAvversa.AmbitoOsservazione
            row("vra_ambito_studio_titolo") = reazioneAvversa.AmbitoOsservazione_Studio_Titolo
            row("vra_ambito_studio_tipologia") = reazioneAvversa.AmbitoOsservazione_Studio_Tipologia
            row("vra_ambito_studio_numero") = reazioneAvversa.AmbitoOsservazione_Studio_Numero

            If reazioneAvversa.Peso.HasValue Then
                row("vra_peso") = reazioneAvversa.Peso.Value
            Else
                row("vra_peso") = DBNull.Value
            End If

            If reazioneAvversa.Altezza.HasValue Then
                row("vra_altezza") = reazioneAvversa.Altezza.Value
            Else
                row("vra_altezza") = DBNull.Value
            End If

            If reazioneAvversa.DataUltimaMestruazione.HasValue Then
                row("vra_data_ultima_mestruazione") = reazioneAvversa.DataUltimaMestruazione.Value
            Else
                row("vra_data_ultima_mestruazione") = DBNull.Value
            End If

            row("vra_allattamento") = reazioneAvversa.Allattamento
            row("vra_gravidanza") = reazioneAvversa.Gravidanza
            row("vra_causa_osservata") = reazioneAvversa.CausaReazioneOsservata

            row("vra_dosaggio") = reazioneAvversa.Dosaggio
            row("vra_indicazioni") = reazioneAvversa.Indicazioni
            row("vra_noi_codice_indicazioni") = reazioneAvversa.CodiceIndicazioni
            row("vra_migliorata") = reazioneAvversa.Migliorata
            If reazioneAvversa.Richiamo = 0 Then
                row("vra_richiamo") = DBNull.Value
            Else
                row("vra_richiamo") = reazioneAvversa.Richiamo
            End If
            row("vra_ricomparsa") = reazioneAvversa.Ricomparsa
            row("vra_ripreso") = reazioneAvversa.Ripreso
            row("vra_sospeso") = reazioneAvversa.Sospeso
            If reazioneAvversa.DataScadenzaLotto.HasValue Then
                row("vra_lot_data_scadenza") = reazioneAvversa.DataScadenzaLotto.Value
            Else
                row("vra_lot_data_scadenza") = DBNull.Value
            End If

            row("vra_farmconc1_noc_codice") = reazioneAvversa.FarmacoConcomitante1_CodiceNomeCommerciale
            row("vra_farmconc1_noc_descrizione") = reazioneAvversa.FarmacoConcomitante1_DescrizioneNomeCommerciale
            row("vra_farmconc1_lot_codice") = reazioneAvversa.FarmacoConcomitante1_CodiceLotto
            If reazioneAvversa.FarmacoConcomitante1_DataScadenzaLotto.HasValue Then
                row("vra_farmconc1_lot_data_scad") = reazioneAvversa.FarmacoConcomitante1_DataScadenzaLotto.Value
            Else
                row("vra_farmconc1_lot_data_scad") = DBNull.Value
            End If
            If reazioneAvversa.FarmacoConcomitante1_DataOraEffettuazione.HasValue Then
                row("vra_farmconc1_dataora_eff") = reazioneAvversa.FarmacoConcomitante1_DataOraEffettuazione.Value
            Else
                row("vra_farmconc1_dataora_eff") = DBNull.Value
            End If

            If reazioneAvversa.FarmacoConcomitante1_Dose.HasValue Then
                row("vra_farmconc1_dose") = reazioneAvversa.FarmacoConcomitante1_Dose.Value
            Else
                row("vra_farmconc1_dose") = DBNull.Value
            End If

            row("vra_farmconc1_sii_codice") = reazioneAvversa.FarmacoConcomitante1_CodiceSitoInoculazione
            row("vra_farmconc1_vii_codice") = reazioneAvversa.FarmacoConcomitante1_CodiceViaSomministrazione
            row("vra_farmconc1_sospeso") = reazioneAvversa.FarmacoConcomitante1_Sospeso
            row("vra_farmconc1_migliorata") = reazioneAvversa.FarmacoConcomitante1_Migliorata
            row("vra_farmconc1_ripreso") = reazioneAvversa.FarmacoConcomitante1_Ripreso
            row("vra_farmconc1_ricomparsa") = reazioneAvversa.FarmacoConcomitante1_Ricomparsa
            row("vra_farmconc1_indicazioni") = reazioneAvversa.FarmacoConcomitante1_Indicazioni
            row("vra_farmconc1_noi_cod_indic") = reazioneAvversa.FarmacoConcomitante1_CodiceIndicazioni
            row("vra_farmconc1_dosaggio") = reazioneAvversa.FarmacoConcomitante1_Dosaggio

            If reazioneAvversa.FarmacoConcomitante1_Richiamo.HasValue Then
                row("vra_farmconc1_richiamo") = reazioneAvversa.FarmacoConcomitante1_Richiamo.Value
            Else
                row("vra_farmconc1_richiamo") = DBNull.Value
            End If
            row("vra_farmconc2_noc_codice") = reazioneAvversa.FarmacoConcomitante2_CodiceNomeCommerciale
            row("vra_farmconc2_noc_descrizione") = reazioneAvversa.FarmacoConcomitante2_DescrizioneNomeCommerciale
            row("vra_farmconc2_lot_codice") = reazioneAvversa.FarmacoConcomitante2_CodiceLotto
            If reazioneAvversa.FarmacoConcomitante2_DataScadenzaLotto.HasValue Then
                row("vra_farmconc2_lot_data_scad") = reazioneAvversa.FarmacoConcomitante2_DataScadenzaLotto.Value
            Else
                row("vra_farmconc2_lot_data_scad") = DBNull.Value
            End If

            If reazioneAvversa.FarmacoConcomitante2_DataOraEffettuazione.HasValue Then
                row("vra_farmconc2_dataora_eff") = reazioneAvversa.FarmacoConcomitante2_DataOraEffettuazione.Value
            Else
                row("vra_farmconc2_dataora_eff") = DBNull.Value
            End If

            If reazioneAvversa.FarmacoConcomitante2_Dose.HasValue Then
                row("vra_farmconc2_dose") = reazioneAvversa.FarmacoConcomitante2_Dose.Value
            Else
                row("vra_farmconc2_dose") = DBNull.Value
            End If

            row("vra_farmconc2_sii_codice") = reazioneAvversa.FarmacoConcomitante2_CodiceSitoInoculazione
            row("vra_farmconc2_vii_codice") = reazioneAvversa.FarmacoConcomitante2_CodiceViaSomministrazione
            row("vra_farmconc2_sospeso") = reazioneAvversa.FarmacoConcomitante2_Sospeso
            row("vra_farmconc2_migliorata") = reazioneAvversa.FarmacoConcomitante2_Migliorata
            row("vra_farmconc2_ripreso") = reazioneAvversa.FarmacoConcomitante2_Ripreso
            row("vra_farmconc2_ricomparsa") = reazioneAvversa.FarmacoConcomitante2_Ricomparsa
            row("vra_farmconc2_indicazioni") = reazioneAvversa.FarmacoConcomitante2_Indicazioni
            row("vra_farmconc2_noi_cod_indic") = reazioneAvversa.FarmacoConcomitante2_CodiceIndicazioni

            row("vra_farmconc2_dosaggio") = reazioneAvversa.FarmacoConcomitante2_Dosaggio

            If reazioneAvversa.FarmacoConcomitante2_Richiamo.HasValue Then
                row("vra_farmconc2_richiamo") = reazioneAvversa.FarmacoConcomitante2_Richiamo.Value
            Else
                row("vra_farmconc2_richiamo") = DBNull.Value
            End If

            row("vra_firma_segnalatore") = reazioneAvversa.FirmaSegnalatore
            row("vra_oet_codice") = reazioneAvversa.CodiceOrigineEtnica
            row("vra_id_scheda") = reazioneAvversa.IdScheda
			row("vra_segnalazione_id") = reazioneAvversa.SegnalazioneId

			If reazioneAvversa.UtenteInvio.HasValue Then
				row("vra_ute_id_invio") = reazioneAvversa.UtenteInvio.Value
			Else
				row("vra_ute_id_invio") = DBNull.Value
			End If

			If reazioneAvversa.DataInvio.HasValue Then
				row("vra_data_invio") = reazioneAvversa.DataInvio
			Else
				row("vra_data_invio") = DBNull.Value
			End If

			row("vra_flag_inviato") = reazioneAvversa.FlagInviato





		End Sub

        ''' <summary>
        ''' Copia i valori dei campi relativi alla reazione avversa da sourceRow a destinationRow.
        ''' </summary>
        ''' <param name="sourceRow"></param>
        ''' <param name="destinationRow"></param>
        ''' <remarks></remarks>
        Public Shared Sub CopyRowsReazioniAvverse(sourceRow As DataRow, destinationRow As DataRow)
            '---
            destinationRow("vra_rea_codice") = sourceRow("vra_rea_codice")
            destinationRow("vra_re1_codice") = sourceRow("vra_re1_codice")
            destinationRow("vra_re2_codice") = sourceRow("vra_re2_codice")
            destinationRow("rea_descrizione") = sourceRow("rea_descrizione")
            destinationRow("vra_visita") = sourceRow("vra_visita")
            destinationRow("vra_terapia") = sourceRow("vra_terapia")
            destinationRow("vra_rea_altro") = sourceRow("vra_rea_altro")
            destinationRow("vra_gravita_reazione") = sourceRow("vra_gravita_reazione")
            destinationRow("vra_grave") = sourceRow("vra_grave")
            destinationRow("vra_esito") = sourceRow("vra_esito")
            destinationRow("vra_data_esito") = sourceRow("vra_data_esito")
            destinationRow("vra_motivo_decesso") = sourceRow("vra_motivo_decesso")
            destinationRow("vra_data_reazione") = sourceRow("vra_data_reazione")
            destinationRow("noc_descrizione") = sourceRow("noc_descrizione")
            destinationRow("ves_lot_codice") = sourceRow("ves_lot_codice")
            destinationRow("lot_data_scadenza") = sourceRow("lot_data_scadenza")
            destinationRow("vii_descrizione") = sourceRow("vii_descrizione")
            destinationRow("ves_dataora_effettuazione") = sourceRow("ves_dataora_effettuazione")
            destinationRow("ves_sii_codice") = sourceRow("ves_sii_codice")
            destinationRow("vra_luogo") = sourceRow("vra_luogo")
            destinationRow("vra_altro_luogo") = sourceRow("vra_altro_luogo")
            destinationRow("vra_farmaco_concomitante") = sourceRow("vra_farmaco_concomitante")
            destinationRow("vra_farmaco_descrizione") = sourceRow("vra_farmaco_descrizione")
            destinationRow("vra_uso_concomitante") = sourceRow("vra_uso_concomitante")
            destinationRow("vra_condizioni_concomitanti") = sourceRow("vra_condizioni_concomitanti")
            destinationRow("vra_qualifica") = sourceRow("vra_qualifica")
            destinationRow("vra_altra_qualifica") = sourceRow("vra_altra_qualifica")
            destinationRow("vra_cognome_segnalatore") = sourceRow("vra_cognome_segnalatore")
            destinationRow("vra_nome_segnalatore") = sourceRow("vra_nome_segnalatore")
            destinationRow("vra_indirizzo_segnalatore") = sourceRow("vra_indirizzo_segnalatore")
            destinationRow("vra_tel_segnalatore") = sourceRow("vra_tel_segnalatore")
            destinationRow("vra_fax_segnalatore") = sourceRow("vra_fax_segnalatore")
            destinationRow("vra_mail_segnalatore") = sourceRow("vra_mail_segnalatore")
            destinationRow("vra_data_compilazione") = sourceRow("vra_data_compilazione")
            destinationRow("vra_usl_inserimento") = sourceRow("vra_usl_inserimento")
            destinationRow("vra_noi_codice_indicazioni") = sourceRow("vra_noi_codice_indicazioni")
            '---
            destinationRow("vra_altre_informazioni") = sourceRow("vra_altre_informazioni")
            destinationRow("vra_ambito_osservazione") = sourceRow("vra_ambito_osservazione")
            destinationRow("vra_ambito_studio_titolo") = sourceRow("vra_ambito_studio_titolo")
            destinationRow("vra_ambito_studio_tipologia") = sourceRow("vra_ambito_studio_tipologia")
            destinationRow("vra_ambito_studio_numero") = sourceRow("vra_ambito_studio_numero")
            destinationRow("vra_peso") = sourceRow("vra_peso")
            destinationRow("vra_altezza") = sourceRow("vra_altezza")
            destinationRow("vra_data_ultima_mestruazione") = sourceRow("vra_data_ultima_mestruazione")
            destinationRow("vra_allattamento") = sourceRow("vra_allattamento")
            destinationRow("vra_gravidanza") = sourceRow("vra_gravidanza")
            destinationRow("vra_causa_osservata") = sourceRow("vra_causa_osservata")
            '---
            destinationRow("vra_dosaggio") = sourceRow("vra_dosaggio")
            destinationRow("vra_sospeso") = sourceRow("vra_sospeso")
            destinationRow("vra_migliorata") = sourceRow("vra_migliorata")
            destinationRow("vra_ripreso") = sourceRow("vra_ripreso")
            destinationRow("vra_ricomparsa") = sourceRow("vra_ricomparsa")
            destinationRow("vra_indicazioni") = sourceRow("vra_indicazioni")
            destinationRow("vra_richiamo") = sourceRow("vra_richiamo")
            '---
            destinationRow("vra_farmconc1_noc_codice") = sourceRow("vra_farmconc1_noc_codice")
            destinationRow("vra_farmconc1_noc_descrizione") = sourceRow("vra_farmconc1_noc_descrizione")
            destinationRow("vra_farmconc1_lot_codice") = sourceRow("vra_farmconc1_lot_codice")
            destinationRow("vra_farmconc1_lot_data_scad") = sourceRow("vra_farmconc1_lot_data_scad")
            destinationRow("vra_farmconc1_dataora_eff") = sourceRow("vra_farmconc1_dataora_eff")
            destinationRow("vra_farmconc1_dose") = sourceRow("vra_farmconc1_dose")
            destinationRow("vra_farmconc1_sii_codice") = sourceRow("vra_farmconc1_sii_codice")
            destinationRow("vra_farmconc1_vii_codice") = sourceRow("vra_farmconc1_vii_codice")
            destinationRow("vra_farmconc1_sospeso") = sourceRow("vra_farmconc1_sospeso")
            destinationRow("vra_farmconc1_migliorata") = sourceRow("vra_farmconc1_migliorata")
            destinationRow("vra_farmconc1_ripreso") = sourceRow("vra_farmconc1_ripreso")
            destinationRow("vra_farmconc1_ricomparsa") = sourceRow("vra_farmconc1_ricomparsa")
            destinationRow("vra_farmconc1_indicazioni") = sourceRow("vra_farmconc1_indicazioni")
            destinationRow("vra_farmconc1_noi_cod_indic") = sourceRow("vra_farmconc1_noi_cod_indic")
            destinationRow("vra_farmconc1_lot_data_scad") = sourceRow("vra_farmconc1_lot_data_scad")
            destinationRow("vra_farmconc1_dosaggio") = sourceRow("vra_farmconc1_dosaggio")
            destinationRow("vra_farmconc1_richiamo") = sourceRow("vra_farmconc1_richiamo")
            '---
            destinationRow("vra_farmconc2_noc_codice") = sourceRow("vra_farmconc2_noc_codice")
            destinationRow("vra_farmconc2_noc_descrizione") = sourceRow("vra_farmconc2_noc_descrizione")
            destinationRow("vra_farmconc2_lot_codice") = sourceRow("vra_farmconc2_lot_codice")
            destinationRow("vra_farmconc2_lot_data_scad") = sourceRow("vra_farmconc2_lot_data_scad")
            destinationRow("vra_farmconc2_dataora_eff") = sourceRow("vra_farmconc2_dataora_eff")
            destinationRow("vra_farmconc2_dose") = sourceRow("vra_farmconc2_dose")
            destinationRow("vra_farmconc2_sii_codice") = sourceRow("vra_farmconc2_sii_codice")
            destinationRow("vra_farmconc2_vii_codice") = sourceRow("vra_farmconc2_vii_codice")
            destinationRow("vra_farmconc2_sospeso") = sourceRow("vra_farmconc2_sospeso")
            destinationRow("vra_farmconc2_migliorata") = sourceRow("vra_farmconc2_migliorata")
            destinationRow("vra_farmconc2_ripreso") = sourceRow("vra_farmconc2_ripreso")
            destinationRow("vra_farmconc2_ricomparsa") = sourceRow("vra_farmconc2_ricomparsa")
            destinationRow("vra_farmconc2_indicazioni") = sourceRow("vra_farmconc2_indicazioni")
            destinationRow("vra_farmconc2_noi_cod_indic") = sourceRow("vra_farmconc2_noi_cod_indic")
            destinationRow("vra_farmconc2_lot_data_scad") = sourceRow("vra_farmconc2_lot_data_scad")
            destinationRow("vra_farmconc2_dosaggio") = sourceRow("vra_farmconc2_dosaggio")
            destinationRow("vra_farmconc2_richiamo") = sourceRow("vra_farmconc2_richiamo")
            destinationRow("vra_altre_informazioni") = sourceRow("vra_altre_informazioni")
            '---
            destinationRow("vra_firma_segnalatore") = sourceRow("vra_firma_segnalatore")
            destinationRow("vra_oet_codice") = sourceRow("vra_oet_codice")
            destinationRow("vra_id_scheda") = sourceRow("vra_id_scheda")
			destinationRow("vra_segnalazione_id") = sourceRow("vra_segnalazione_id")
			destinationRow("vra_ute_id_invio") = sourceRow("vra_ute_id_invio")
			destinationRow("vra_data_invio") = sourceRow("vra_data_invio")
			destinationRow("vra_flag_inviato") = sourceRow("vra_flag_inviato")
			'---
		End Sub

        ''' <summary>
        ''' Restituisce un'entity VaccinazioneEseguita con i valori contenuti nella datarow specificata.
        ''' </summary>
        ''' <param name="row"></param>
        ''' <param name="dataCompilazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateEseguitaReazioneByDataRow(row As DataRow, dataModifica As DateTime) As Entities.VaccinazioneEseguita

            Dim vaccinazioneEseguita As New Entities.VaccinazioneEseguita()

            vaccinazioneEseguita.ves_id = GetNullableInt64FromRowCell(row, "ves_id")
            vaccinazioneEseguita.ves_usl_inserimento = row("ves_usl_inserimento").ToString()

            vaccinazioneEseguita.paz_codice = row("paz_codice")
            vaccinazioneEseguita.ves_dataora_effettuazione = GetDateTimeFromRowCell(row, "ves_dataora_effettuazione")
            vaccinazioneEseguita.ves_n_richiamo = GetInt32FromRowCell(row, "ves_n_richiamo")

            vaccinazioneEseguita.ves_vac_codice = row("ves_vac_codice").ToString()
            vaccinazioneEseguita.vac_descrizione = row("vac_descrizione").ToString()

            vaccinazioneEseguita.ves_ass_codice = row("ves_ass_codice").ToString()
            vaccinazioneEseguita.ass_descrizione = row("ass_descrizione").ToString()

            vaccinazioneEseguita.ves_luogo = row("ves_luogo").ToString()
            vaccinazioneEseguita.ves_noc_codice = row("ves_noc_codice").ToString()
            vaccinazioneEseguita.noc_descrizione = row("noc_descrizione").ToString()
            vaccinazioneEseguita.ves_lot_codice = row("ves_lot_codice").ToString()
            vaccinazioneEseguita.ves_sii_codice = row("ves_sii_codice").ToString()
            vaccinazioneEseguita.sii_descrizione = row("sii_descrizione").ToString()
            vaccinazioneEseguita.ves_vii_codice = row("ves_vii_codice").ToString()
            vaccinazioneEseguita.vii_descrizione = row("vii_descrizione").ToString()
            vaccinazioneEseguita.ves_flag_fittizia = row("ves_flag_fittizia").ToString()
            vaccinazioneEseguita.ves_data_scadenza = GetNullableDateTimeFromRowCell(row, "lot_data_scadenza")

            vaccinazioneEseguita.ReazioneAvversa.DataReazione = GetDateTimeFromRowCell(row, "vra_data_reazione")

            vaccinazioneEseguita.ReazioneAvversa.CodiceReazione = row("vra_rea_codice").ToString()
            vaccinazioneEseguita.ReazioneAvversa.DescrizioneReazione = row("rea_descrizione").ToString()
            vaccinazioneEseguita.ReazioneAvversa.CodiceReazione1 = row("vra_re1_codice").ToString()
            vaccinazioneEseguita.ReazioneAvversa.DescrizioneReazione1 = row("rea_descrizione1").ToString()
            vaccinazioneEseguita.ReazioneAvversa.CodiceReazione2 = row("vra_re2_codice").ToString()
            vaccinazioneEseguita.ReazioneAvversa.DescrizioneReazione2 = row("rea_descrizione2").ToString()

            vaccinazioneEseguita.ReazioneAvversa.VisiteRicoveri = row("vra_visita").ToString()
            vaccinazioneEseguita.ReazioneAvversa.Terapie = row("vra_terapia").ToString()
            vaccinazioneEseguita.ReazioneAvversa.GravitaReazione = row("vra_gravita_reazione").ToString()
            vaccinazioneEseguita.ReazioneAvversa.AltraReazione = row("vra_rea_altro").ToString()
            vaccinazioneEseguita.ReazioneAvversa.Grave = row("vra_grave").ToString()
            vaccinazioneEseguita.ReazioneAvversa.Esito = row("vra_esito").ToString()
            vaccinazioneEseguita.ReazioneAvversa.DataEsito = GetDateTimeFromRowCell(row, "vra_data_esito")
            vaccinazioneEseguita.ReazioneAvversa.MotivoDecesso = row("vra_motivo_decesso").ToString()

            ' Farmaco relativo all'associazione selezionata
            vaccinazioneEseguita.ReazioneAvversa.DescrizioneNomeCommerciale = row("noc_descrizione").ToString()
            vaccinazioneEseguita.ReazioneAvversa.CodiceLotto = row("ves_lot_codice").ToString()
            If Not GetNullableDateTimeFromRowCell(row, "vra_lot_data_scadenza") Is Nothing Then
                vaccinazioneEseguita.ReazioneAvversa.DataScadenzaLotto = GetNullableDateTimeFromRowCell(row, "vra_lot_data_scadenza")
            Else
                vaccinazioneEseguita.ReazioneAvversa.DataScadenzaLotto = GetNullableDateTimeFromRowCell(row, "lot_data_scadenza")
            End If

            vaccinazioneEseguita.ReazioneAvversa.CodiceSitoInoculazione = row("ves_sii_codice").ToString()
            vaccinazioneEseguita.ReazioneAvversa.DescrizioneSitoInoculazione = row("sii_descrizione").ToString()
            vaccinazioneEseguita.ReazioneAvversa.CodiceViaSomministrazione = row("ves_vii_codice").ToString()
            vaccinazioneEseguita.ReazioneAvversa.DescrizioneViaSomministrazione = row("vii_descrizione").ToString()

            vaccinazioneEseguita.ReazioneAvversa.Dosaggio = row("vra_dosaggio").ToString()
            vaccinazioneEseguita.ReazioneAvversa.Indicazioni = row("vra_indicazioni").ToString()
            vaccinazioneEseguita.ReazioneAvversa.CodiceIndicazioni = row("vra_noi_codice_indicazioni").ToString()
            vaccinazioneEseguita.ReazioneAvversa.Migliorata = row("vra_migliorata").ToString()
            vaccinazioneEseguita.ReazioneAvversa.Richiamo = GetInt32FromRowCell(row, "vra_richiamo")
            vaccinazioneEseguita.ReazioneAvversa.Ricomparsa = row("vra_ricomparsa").ToString()
            vaccinazioneEseguita.ReazioneAvversa.Ripreso = row("vra_ripreso").ToString()
            vaccinazioneEseguita.ReazioneAvversa.Sospeso = row("vra_sospeso").ToString()

            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante = row("vra_farmaco_concomitante").ToString()
            vaccinazioneEseguita.ReazioneAvversa.FarmacoDescrizione = row("vra_farmaco_descrizione").ToString()
            vaccinazioneEseguita.ReazioneAvversa.UsoConcomitante = row("vra_uso_concomitante").ToString()
            vaccinazioneEseguita.ReazioneAvversa.CondizioniConcomitanti = row("vra_condizioni_concomitanti").ToString()

            vaccinazioneEseguita.ReazioneAvversa.Qualifica = row("vra_qualifica").ToString()
            vaccinazioneEseguita.ReazioneAvversa.AltraQualifica = row("vra_altra_qualifica").ToString()
            vaccinazioneEseguita.ReazioneAvversa.CognomeSegnalatore = row("vra_cognome_segnalatore").ToString()
            vaccinazioneEseguita.ReazioneAvversa.NomeSegnalatore = row("vra_nome_segnalatore").ToString()
            vaccinazioneEseguita.ReazioneAvversa.IndirizzoSegnalatore = row("vra_indirizzo_segnalatore").ToString()
            vaccinazioneEseguita.ReazioneAvversa.TelSegnalatore = row("vra_tel_segnalatore").ToString()
            vaccinazioneEseguita.ReazioneAvversa.FaxSegnalatore = row("vra_fax_segnalatore").ToString()
            vaccinazioneEseguita.ReazioneAvversa.MailSegnalatore = row("vra_mail_segnalatore").ToString()
            vaccinazioneEseguita.ReazioneAvversa.DataCompilazione = GetDateTimeFromRowCell(row, "vra_data_compilazione")
            vaccinazioneEseguita.ReazioneAvversa.DataModifica = dataModifica
            vaccinazioneEseguita.ReazioneAvversa.IdUtenteModifica = OnVacContext.UserId

            vaccinazioneEseguita.ReazioneAvversa.CodiceUslInserimento = row("vra_usl_inserimento").ToString()
            vaccinazioneEseguita.ReazioneAvversa.DescrizioneUslInserimento = row("usl_inserimento_vra_descr").ToString()

            vaccinazioneEseguita.ReazioneAvversa.AltreInformazioni = row("vra_altre_informazioni").ToString()
            vaccinazioneEseguita.ReazioneAvversa.AmbitoOsservazione = row("vra_ambito_osservazione").ToString()
            vaccinazioneEseguita.ReazioneAvversa.AmbitoOsservazione_Studio_Titolo = row("vra_ambito_studio_titolo").ToString()
            vaccinazioneEseguita.ReazioneAvversa.AmbitoOsservazione_Studio_Tipologia = row("vra_ambito_studio_tipologia").ToString()
            vaccinazioneEseguita.ReazioneAvversa.AmbitoOsservazione_Studio_Numero = row("vra_ambito_studio_numero").ToString()
            vaccinazioneEseguita.ReazioneAvversa.Peso = GetNullableDoubleFromRowCell(row, "vra_peso")
            vaccinazioneEseguita.ReazioneAvversa.Altezza = GetNullableInt32FromRowCell(row, "vra_altezza")
            vaccinazioneEseguita.ReazioneAvversa.DataUltimaMestruazione = GetNullableDateTimeFromRowCell(row, "vra_data_ultima_mestruazione")
            vaccinazioneEseguita.ReazioneAvversa.Allattamento = row("vra_allattamento").ToString()
            vaccinazioneEseguita.ReazioneAvversa.Gravidanza = row("vra_gravidanza").ToString()
            vaccinazioneEseguita.ReazioneAvversa.CausaReazioneOsservata = row("vra_causa_osservata").ToString()

            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_CodiceNomeCommerciale = row("vra_farmconc1_noc_codice").ToString()
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_DescrizioneNomeCommerciale = row("vra_farmconc1_noc_descrizione").ToString()
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_CodiceLotto = row("vra_farmconc1_lot_codice").ToString()
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_DataScadenzaLotto = GetNullableDateTimeFromRowCell(row, "vra_farmconc1_lot_data_scad")
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_DataOraEffettuazione = GetNullableDateTimeFromRowCell(row, "vra_farmconc1_dataora_eff")
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_Dose = GetNullableInt32FromRowCell(row, "vra_farmconc1_dose")
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_CodiceSitoInoculazione = row("vra_farmconc1_sii_codice").ToString()
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_CodiceViaSomministrazione = row("vra_farmconc1_vii_codice").ToString()
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_Sospeso = row("vra_farmconc1_sospeso").ToString()
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_Migliorata = row("vra_farmconc1_migliorata").ToString()
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_Ripreso = row("vra_farmconc1_ripreso").ToString()
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_Ricomparsa = row("vra_farmconc1_ricomparsa").ToString()
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_Indicazioni = row("vra_farmconc1_indicazioni").ToString()
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_CodiceIndicazioni = row("vra_farmconc1_noi_cod_indic").ToString()
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_Dosaggio = row("vra_farmconc1_dosaggio").ToString()
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_Richiamo = GetNullableInt32FromRowCell(row, "vra_farmconc1_richiamo")

            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_CodiceNomeCommerciale = row("vra_farmconc2_noc_codice").ToString()
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_DescrizioneNomeCommerciale = row("vra_farmconc2_noc_descrizione").ToString()
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_CodiceLotto = row("vra_farmconc2_lot_codice").ToString()
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_DataScadenzaLotto = GetNullableDateTimeFromRowCell(row, "vra_farmconc2_lot_data_scad")
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_DataOraEffettuazione = GetNullableDateTimeFromRowCell(row, "vra_farmconc2_dataora_eff")
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_Dose = GetNullableInt32FromRowCell(row, "vra_farmconc2_dose")
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_CodiceSitoInoculazione = row("vra_farmconc2_sii_codice").ToString()
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_CodiceViaSomministrazione = row("vra_farmconc2_vii_codice").ToString()
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_Sospeso = row("vra_farmconc2_sospeso").ToString()
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_Migliorata = row("vra_farmconc2_migliorata").ToString()
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_Ripreso = row("vra_farmconc2_ripreso").ToString()
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_Ricomparsa = row("vra_farmconc2_ricomparsa").ToString()
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_Indicazioni = row("vra_farmconc2_indicazioni").ToString()
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_CodiceIndicazioni = row("vra_farmconc2_noi_cod_indic").ToString()
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_Dosaggio = row("vra_farmconc2_dosaggio").ToString()
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_Richiamo = GetNullableInt32FromRowCell(row, "vra_farmconc2_richiamo")

            vaccinazioneEseguita.ReazioneAvversa.FirmaSegnalatore = row("vra_firma_segnalatore").ToString()
            vaccinazioneEseguita.ReazioneAvversa.CodiceOrigineEtnica = row("vra_oet_codice").ToString()
            vaccinazioneEseguita.CodiceOrigineEtnica = row("vra_oet_codice").ToString()
            vaccinazioneEseguita.ReazioneAvversa.IdScheda = row("vra_id_scheda").ToString()
			vaccinazioneEseguita.ReazioneAvversa.SegnalazioneId = row("vra_segnalazione_id").ToString()
			vaccinazioneEseguita.ReazioneAvversa.UtenteInvio = GetNullableInt32FromRowCell(row, "vra_ute_id_invio")
			vaccinazioneEseguita.ReazioneAvversa.DataInvio = GetNullableDateTimeFromRowCell(row, "vra_data_invio")
			vaccinazioneEseguita.ReazioneAvversa.FlagInviato = row("vra_flag_inviato").ToString()

			Return vaccinazioneEseguita

        End Function

        Public Shared Sub StampaModuloReazioniAvverse(selectedRow As DataRow, openPopUp As Boolean, currentPage As Page, settings As Settings.Settings)

            Dim vaccinazioneEseguita As Entities.VaccinazioneEseguita = Common.ReazioniAvverseCommon.CreateEseguitaReazioneByDataRow(selectedRow, DateTime.Now)

            Common.ReazioniAvverseCommon.StampaModuloReazioniAvverse(vaccinazioneEseguita, openPopUp, currentPage, settings)

        End Sub

        Public Shared Sub StampaModuloReazioniAvverse(vaccinazioneEseguita As Entities.VaccinazioneEseguita, openPopUp As Boolean, currentPage As Page, settings As Settings.Settings)

            If vaccinazioneEseguita Is Nothing Then Return

            Dim filtro As String = String.Empty
            Dim reportFolder As String = String.Empty

            Dim rpt As New ReportParameter()

            rpt.AddParameter("codice_asl", OnVacContext.CodiceUslCorrente)
            rpt.AddParameter("vac_codice", vaccinazioneEseguita.ves_vac_codice)
            rpt.AddParameter("dose", vaccinazioneEseguita.ves_n_richiamo.ToString())
            rpt.AddParameter("data_vaccinazione", vaccinazioneEseguita.ves_dataora_effettuazione.ToString("{0:dd/MM/yyyy}"))

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                Using bizVaccinazioniEseguite As New Biz.BizVaccinazioniEseguite(genericProvider, settings, OnVacContext.CreateBizContextInfos, Nothing)

                    ' Parametri per stampa di ulteriori farmaci sospetti (max 2)
                    Dim farmaco2 As String = String.Empty
                    Dim farmaco3 As String = String.Empty

                    Dim listCodiciLottiReazione As List(Of String) = bizVaccinazioniEseguite.GetCodiciLottiReazioneByDataEffettuazione(
                        OnVacUtility.Variabili.PazId, vaccinazioneEseguita.ves_dataora_effettuazione, vaccinazioneEseguita.ves_lot_codice)

                    If Not listCodiciLottiReazione Is Nothing Then
                        If listCodiciLottiReazione.Count > 0 Then farmaco2 = listCodiciLottiReazione(0)
                        If listCodiciLottiReazione.Count > 1 Then farmaco3 = listCodiciLottiReazione(1)
                    End If

                    rpt.AddParameter("farmacoSospetto2", farmaco2)
                    rpt.AddParameter("farmacoSospetto3", farmaco3)

                    filtro = bizVaccinazioniEseguite.GetFiltroReportReazioniAvverse(
                        OnVacUtility.Variabili.PazId, vaccinazioneEseguita.ves_vac_codice, vaccinazioneEseguita.ves_n_richiamo,
                        vaccinazioneEseguita.ves_dataora_effettuazione, vaccinazioneEseguita.ReazioneAvversa.DataReazione)

                End Using

                Using bizReport As New Biz.BizReport(genericProvider, settings, OnVacContext.CreateBizContextInfos())

                    reportFolder = bizReport.GetReportFolder(Constants.ReportName.ReazioniAvverse)

                End Using

            End Using

            If String.IsNullOrWhiteSpace(reportFolder) Then

                OnVacUtility.StampaNonPresente(currentPage, Constants.ReportName.ReazioniAvverse)

            Else

                If openPopUp Then

                    ' In caso di apertura anteprima report da pop-up, parametri e filtro vengono passati alla maschera StampaReportPopUp utilizzando la Session (schifezza)
                    Dim param As New ArrayList()

                    For Each item As KeyValuePair(Of String, Object) In rpt.ParameterList
                        param.Add(String.Format("{0}|{1}", item.Key, item.Value))
                    Next

                    HttpContext.Current.Session("ReazioniAvverse_param") = param
                    HttpContext.Current.Session("ReazioniAvverse") = filtro

                    Dim script As String = String.Format("window.open('{0}?report={1}','Reazioni_Avverse','top=0,left=0,width=500,height=500,resizable=1')",
                                                         currentPage.ResolveClientUrl("~/Stampe/StampaReportPopUp.aspx"),
                                                         Constants.ReportName.ReazioniAvverse.Replace(".rpt", String.Empty))

                    currentPage.ClientScript.RegisterClientScriptBlock(currentPage.GetType(), "print", script, True)

                Else

                    OnVacReport.StampaReport(currentPage.Request.Path, Constants.ReportName.ReazioniAvverse, filtro, rpt, Nothing, Nothing, reportFolder)

                End If

            End If

        End Sub

#End Region

#Region " Private "

        Private Shared Function GetStringFromRowCell(row As DataRow, columnName As String) As String

            If row.IsNull(columnName) Then
                Return String.Empty
            End If

            Return Convert.ToString(row(columnName))

        End Function

        Private Shared Function GetInt32FromRowCell(row As DataRow, columnName As String)

            If row.IsNull(columnName) Then
                Return 0
            End If

            Return Convert.ToInt32(row(columnName))

        End Function

        Private Shared Function GetNullableInt32FromRowCell(row As DataRow, columnName As String) As Int32?

            If row.IsNull(columnName) Then
                Return Nothing
            End If

            Return Convert.ToInt32(row(columnName))

        End Function

        Private Shared Function GetNullableInt64FromRowCell(row As DataRow, columnName As String) As Int64?

            If row.IsNull(columnName) Then
                Return Nothing
            End If

            Return Convert.ToInt64(row(columnName))

        End Function

        Private Shared Function GetNullableDoubleFromRowCell(row As DataRow, columnName As String) As Double?

            If row.IsNull(columnName) Then
                Return Nothing
            End If

            Return Convert.ToDouble(row(columnName))

        End Function

        Private Shared Function GetNullableDateTimeFromRowCell(row As DataRow, columnName As String) As DateTime?

            If row.IsNull(columnName) Then
                Return Nothing
            End If

            Return Convert.ToDateTime(row(columnName))

        End Function

        Private Shared Function GetDateTimeFromRowCell(row As DataRow, columnName As String)

            If row.IsNull(columnName) Then
                Return DateTime.MinValue
            End If

            Return Convert.ToDateTime(row(columnName))

        End Function

#End Region

    End Class

End Namespace