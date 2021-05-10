Imports System.Collections.Generic
Imports System.IO
Imports System.Xml.Serialization
Imports Onit.OnAssistnet.Data
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities

Public Class BizIntegrazioneReazioniAvverse
    Inherits BizClass

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos)

        MyBase.New(genericprovider, settings, contextInfos, Nothing)

    End Sub

    Public Sub New(dbGenericProviderFactory As DbGenericProviderFactory, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)

        MyBase.New(dbGenericProviderFactory, settings, Nothing, contextInfos, logOptions)

    End Sub

#End Region

#Region " Public "

#Region " Integrazione"
	''' <summary>
	''' Funzione che gestisce uno o più invii di reazioni avverse.
	''' </summary>
	''' <param name="codicePaziente"></param>
	''' <param name="consultorio"></param>
	''' <param name="vaccinazioniEseguiteResult"></param>
	''' <returns></returns>
	Public Function InserisciReazioneAversaPu(codicePaziente As Integer, consultorio As String, vaccinazioniEseguiteResult As List(Of ReazioneAvversa)) As String
		Dim ret As String
		If vaccinazioniEseguiteResult.Count = 1 Then
			ret = InserisciReazioneAversa(codicePaziente, consultorio, vaccinazioniEseguiteResult)
		Else
			Dim strRet As New System.Text.StringBuilder
			Dim ass As String = String.Empty
			Dim ndos As Integer
			Dim lis As List(Of String) = vaccinazioniEseguiteResult.Select(Function(p) p.VesAssCodice).Distinct().ToList()
			Dim iret As String
			For Each j As String In lis
				Dim iList As New List(Of ReazioneAvversa)
				For Each i As ReazioneAvversa In vaccinazioniEseguiteResult
					If j = i.VesAssCodice Then
						iList.Add(i)
					End If
				Next
				iret = InserisciReazioneAversa(codicePaziente, consultorio, IList)
				strRet.AppendFormat(iret).AppendLine()
			Next

			'For Each i As ReazioneAvversa In vaccinazioniEseguiteResult
			'	Dim iret As String
			'	Dim iList As New List(Of ReazioneAvversa)
			'	iList.Add(i)
			'	iret = InserisciReazioneAversa(codicePaziente, consultorio, iList)
			'	strRet.AppendFormat(iret).AppendLine()
			'Next
			ret = strRet.ToString()
		End If

		Return ret

	End Function
    ''' <summary>
    ''' Funzione di integrazione per creare segnalazione in VigiFaramaco
    ''' </summary>
    ''' <param name="idReazioneAvversa"></param>
    ''' <param name="operatoreInserimento"></param>
    ''' <returns></returns>
    Private Function InserisciReazioneAversa(codicePaziente As Integer, consultorio As String, vaccinazioniEseguiteResult As List(Of ReazioneAvversa)) As String
        Dim ret As String
        ret = ""
        Dim result As New Biz.BizGenericResult
        Dim parInSeg As New IntegrazioneReazioniAvverse.ParInSegnalazione
        Dim resp As New IntegrazioneReazioniAvverse.response
        parInSeg.ApiToken = ""
        parInSeg.UrlServizio = ""
        parInSeg.UrlServizioGetIdScheda = ""
        Dim parInXml As New IntegrazioneReazioniAvverse.ParInCreaXml
        parInXml.ApiToken = ""
        parInXml.UrlServizio = ""
        Dim resXml As New ResultXml
        Dim dtIntegrazione As New DataTable
        ' mi ricavo lista delle reazioni avverse nuove
        ' fatta modifica per permettere di inviare anche le reazioni che hanno restituito errore, ossia hanno flag inviato a N
        Dim listaMinRea As List(Of Integer) = vaccinazioniEseguiteResult.Where(Function(item) item.FlagInviato = "N").OrderByDescending(Function(item) item.IdReazioneAvversa).GroupBy(Function(p) New With {Key p.VesAssCodice, Key p.VesAssNDose}).Select(Function(grp) Convert.ToInt32(grp.First().IdReazioneAvversa)).ToList()
        'vaccinazioniEseguiteResult.Where(Function(item) item.FlgInsert = True).OrderByDescending(Function(item) item.IdReazioneAvversa).GroupBy(Function(p) New With {Key p.VesAssCodice, Key p.VesAssNDose}).Select(Function(grp) Convert.ToInt32(grp.First().IdReazioneAvversa)).ToList()
        'vaccinazioniEseguiteResult.Where(Function(item) item.FlgInsert = True).OrderByDescending(Function(item) item.IdReazioneAvversa).GroupBy(Function(p) p.VesAssCodice And p.VesAssNDose).Select(Function(grp) Convert.ToInt32(grp.First().IdReazioneAvversa)).ToList()
        Dim listaTotaleRea As List(Of ReazioneAvversa) = vaccinazioniEseguiteResult.Where(Function(item) item.FlagInviato = "N").ToList()
        If listaMinRea.Count > 0 Then
            Using biz As New Biz.BizVaccinazioniEseguite(GenericProvider, Me.Settings, ContextInfos)
                dtIntegrazione = biz.GetVaccinazioniEseguiteIntegrazione(listaMinRea)
            End Using
        End If

        'TODO[ReazioneAvversa] aggiungere try catch per time oput
        resXml = CreaXmlda(parInXml, dtIntegrazione, consultorio)
        parInSeg.XmlPost = resXml.Xml
        Dim logNotificaInviata As New Entities.LogNotificaInviata

        Dim codiceAusiliario As String
        Using bizpaz As New BizPaziente(GenericProvider, Settings, ContextInfos, LogOptions)
            codiceAusiliario = bizpaz.GetCodiceAusiliario(codicePaziente)
        End Using

        Using log As New BizLogNotifiche(GenericProviderCentrale, ContextInfos)
            Try
                logNotificaInviata = log.InsertLogNotificaInviata(codicePaziente, codiceAusiliario, parInSeg.XmlPost, Date.Now, String.Empty, Enumerators.OperazioneLogNotifica.InserimentoSegnalazioneReazioneAvversa, Nothing, String.Empty)
            Catch ex As Exception
                Common.Utility.EventLogHelper.EventLogWrite(ex, "Errore invio notifica inserimento reazione", ContextInfos.IDApplicazione)
            End Try
        End Using

        ' devo associare l'identificativo del log con id della reazione avversa.
        Using LinkReazioneLog As New BizVaccinazioniEseguite(GenericProvider, Settings, ContextInfos)
            If listaTotaleRea(0).IdReazioneAvversa.HasValue Then
                LinkReazioneLog.InsertLinkReazLogInvio(listaTotaleRea(0).IdReazioneAvversa, logNotificaInviata.IdNotifica)
            End If

        End Using

        Try
            Using clientServizio As New IntegrazioneReazioniAvverse.ReazioniAvverseClient
                resp = clientServizio.CreaSegnalazione(parInSeg)
                ret = resp.status
                If resp Is Nothing Then
                    result.Success = False
                    result.Message = "Nessuna risposta dal servizio."
                ElseIf resp.status = "failure" Then
                    result.Success = False
                    Dim listaerrori As String = String.Empty
                    For Each errore As String In resp.errors
                        listaerrori = listaerrori + "- " + errore
                    Next
                    result.Message = listaerrori
                End If

            End Using

        Catch ex As Exception
            result.Success = False
            result.Message = "Errore nel servizio di integrazione Reazione Avversa:" + ex.Message
        End Try

        Using bizLogNotifiche As New BizLogNotifiche(GenericProviderCentrale, ContextInfos)

            Dim command As New BizLogNotifiche.UpdateLogNotificaInviataCommand()

            command.LogNotificaDaModificare = logNotificaInviata

            ' Già valorizzati, non vengono sovrascritti
            command.CodiceCentralePaziente = String.Empty
            command.RichiestaServizio = String.Empty
            command.DataInvio = Nothing
            command.StatoInvio = Enumerators.StatoLogNotificaInviata.Inviata
            ' Valeva già "Inviata" dopo l'insert precedente.

            command.SuccessoRisposta = result.Success
            command.MessaggioRisposta = result.Message.ToString()

            If resp Is Nothing Then
                command.RispostaServizio = String.Empty
                command.DataRicezioneRisposta = Nothing
            Else
                command.RispostaServizio = resp.xmlreturn
                command.DataRicezioneRisposta = DateTime.Now
            End If

            If Not result.Success Then
                command.StatoInvio = Enumerators.StatoLogNotificaInviata.Errore
            End If

            ' Invio sincrono, numero invii già impostato a 1 dall'insert
            command.IncrementaInvii = False

            Try
                bizLogNotifiche.UpdateLogNotificaInviata(command)
            Catch ex As Exception
                Common.Utility.EventLogHelper.EventLogWrite(ex, "Errore invio notifica inserimento reazione - ricezione risposta", ContextInfos.IDApplicazione)
            End Try

        End Using

        If result.Success Then
            'Aggiornamento delle reazioni avverse con id scheda e SegnalazioneId
            For Each i As ReazioneAvversa In listaTotaleRea
                Dim reazione As New ReazioneAvversa
                reazione.IdScheda = resXml.idScheda
                reazione.SegnalazioneId = resp.vigifarmacoId
                reazione.IdReazioneAvversa = i.IdReazioneAvversa
                reazione.FlagInviato = "S"
                reazione.UtenteInvio = ContextInfos.IDUtente
                reazione.DataInvio = Date.Now
                If i.FlgScaduta Then
                    UpdateReazioneAvversaScadutoIdScheda(reazione)
                Else
                    UpdateReazioneAvversaIdScheda(reazione)
                End If

            Next
            ret = "Inserimento della segnalazione, in Vigifarmaco, avvenuto con successo!"
        Else
            'Aggiornamento delle reazioni avverse con id scheda e SegnalazioneId
            For Each i As ReazioneAvversa In listaTotaleRea
                Dim reazione As New ReazioneAvversa
                reazione.IdScheda = Nothing
                reazione.SegnalazioneId = Nothing
                reazione.IdReazioneAvversa = i.IdReazioneAvversa
                reazione.FlagInviato = "N"
                reazione.UtenteInvio = ContextInfos.IDUtente
                reazione.DataInvio = Date.Now
                If i.FlgScaduta Then
                    UpdateReazioneAvversaScadutoIdScheda(reazione)
                Else
                    UpdateReazioneAvversaIdScheda(reazione)
                End If

            Next
            Dim messaggioErrore As String
            messaggioErrore = String.Format("Inserimento della segnalazione, in Vigifarmaco, è fallito!{0} {1}", Environment.NewLine, result.Message.Replace("-", Environment.NewLine + "-"))
            ret = messaggioErrore
        End If

        Return ret

    End Function

    Public Function UpdateReazioneAvversaIdScheda(reazioneAvversa As ReazioneAvversa) As Boolean

        Dim updated As Boolean = GenericProvider.VaccinazioniEseguite.UpdateReazioneAvversaIdscheda(reazioneAvversa)

        Return updated

    End Function

    Public Function UpdateReazioneAvversaScadutoIdScheda(reazioneAvversa As ReazioneAvversa) As Boolean

        Dim updated As Boolean = GenericProvider.VaccinazioniEseguite.UpdateReazioneAvversaScadutaIdscheda(reazioneAvversa)

        Return updated

    End Function

	Private Function CreaXmlda(parametri As IntegrazioneReazioniAvverse.ParInCreaXml, dateReazTable As DataTable, consultorio As String) As ResultXml
		Dim ret As New ResultXml

		Using rac As New IntegrazioneReazioniAvverse.ReazioniAvverseClient
			Dim seg As New Segnalazioni
			Dim scheda_adr As New Scheda_adr()
			Dim listSmPa As New List(Of SM_PA)()

			Dim segn As New segnalatore


			Dim segnalatore As New segnalatore
			Dim datisegn As New dati_segnalatore

			' recupero id scheda vigefarmaco facendo chiamata a servizio
			Dim api As New IntegrazioneReazioniAvverse.ApiKey
			api.ApikeyValue = parametri.ApiToken
			api.UrlServizio = parametri.UrlServizio
			Dim id As String = rac.GetIdScheda(api)
			' inizio a comporre xml da inviare per creazione segnalazione
			' Dati del farmaco sospetto
			Dim numeroCicli As Integer = 1
			For Each dateReaz As DataRow In dateReazTable.Rows
				If numeroCicli >= 1 Then


					If Not dateReaz("ves_noc_codice") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("ves_noc_codice")) Then
						'tag farmaco
						Dim sm_pa As New SM_PA()
						Dim prodotto_medic As New prodotto_medicinale
						prodotto_medic.nome_farmaco = dateReaz("noc_descrizione") '"GARDASIL*1SIR IM 0,5ML+DISP+2"
						If Not dateReaz("noc_codice_aic") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("noc_codice_aic")) Then
							prodotto_medic.codice_farmaco = Left(dateReaz("noc_codice_aic"), 6) '"037311"
						End If
						If Not dateReaz("noc_codice_aic") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("noc_codice_aic")) Then
							prodotto_medic.codice_confezione = Right(dateReaz("noc_codice_aic"), 3) '"154"
						End If
						Dim farmaco As New farmaco
						farmaco.Item = prodotto_medic

						sm_pa.farmaco = farmaco
						' Dati somministrazione
						sm_pa.tipo_somministrazione = tipo_somministrazione.S
						If Not dateReaz("ves_n_richiamo") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("ves_n_richiamo")) Then
							sm_pa.numero_dose_richiamo = dateReaz("ves_n_richiamo")
						End If
						If Not dateReaz("ves_dataora_effettuazione") Is Nothing AndAlso Not dateReaz("ves_dataora_effettuazione") Is DBNull.Value Then
							Dim oraSom As DateTime = dateReaz("ves_dataora_effettuazione")
							Dim ore As String = oraSom.Hour
							Dim minuti As String = oraSom.Minute
							sm_pa.ora_somministrazione = String.Format("{0}:{1}", ore.PadLeft(2, "0"), minuti.PadLeft(2, "0"))
						End If
						If (Not dateReaz("ves_lot_codice") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("ves_lot_codice"))) Then
							sm_pa.lotto = dateReaz("ves_lot_codice")
							If Not dateReaz("vra_lot_data_scadenza") Is Nothing AndAlso Not dateReaz("vra_lot_data_scadenza") Is DBNull.Value Then
								sm_pa.data_scadenza = dateReaz("vra_lot_data_scadenza")
								sm_pa.data_scadenzaSpecified = True
							End If
						End If
						If Not dateReaz("vra_sospeso") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_sospeso")) Then
							If dateReaz("vra_sospeso") = "S" Then
								sm_pa.azioni_intraprese = azioni_intraprese.Item01
								sm_pa.azioni_intrapreseSpecified = True
							End If
						End If
						If Not dateReaz("vra_migliorata") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_migliorata")) Then
							If dateReaz("vra_sospeso") = "S" Then
								sm_pa.reazione_migliorata = dateReaz("vra_migliorata")
							End If

						End If
						If Not dateReaz("mcr_codice_esterno") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("mcr_codice_esterno")) Then
							sm_pa.causa_reazione = CType([Enum].Parse(GetType(causa_reazione), dateReaz("mcr_codice_esterno")), causa_reazione) 'dateReaz("t_map_causa_rea")
							sm_pa.causa_reazioneSpecified = True
						End If
						If Not dateReaz("noc_forma_farmaceutica") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("noc_forma_farmaceutica")) Then
							sm_pa.forma_farmaceutica = dateReaz("noc_forma_farmaceutica")
						End If
						Dim dosaggio As Decimal
						If Not dateReaz("vra_dosaggio") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_dosaggio")) AndAlso Decimal.TryParse(dateReaz("vra_dosaggio"), dosaggio) Then
							sm_pa.dosaggio = dosaggio
							sm_pa.dosaggioSpecified = True
							sm_pa.unita_misura = unita_misura.DF
							sm_pa.unita_misuraSpecified = True
							'TODO: come gestiamo la somministrazione. Si è deciso di passare sempre A
							sm_pa.frequenza_somministrazione = "A"
						End If
						If Not dateReaz("vii_codice_esterno") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vii_codice_esterno")) Then
							sm_pa.via_somministrazione = CType([Enum].Parse(GetType(via_somministrazione), dateReaz("vii_codice_esterno")), via_somministrazione) ' dateReaz("vii_codice_esterno")
							sm_pa.via_somministrazioneSpecified = True
						End If
						If Not dateReaz("ves_data_effettuazione") Is Nothing AndAlso Not dateReaz("ves_data_effettuazione") Is DBNull.Value Then
							sm_pa.data_inizio_terapia = dateReaz("ves_data_effettuazione")
							sm_pa.data_inizio_terapiaSpecified = True
							sm_pa.data_fine_terapia = dateReaz("ves_data_effettuazione")
							sm_pa.data_fine_terapiaSpecified = True
						End If
                        If Not dateReaz("vra_ripreso") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_ripreso")) Then
                            sm_pa.ripresa_somministrazione = dateReaz("vra_ripreso")
                            If dateReaz("vra_ripreso") = "S" Then
                                If Not dateReaz("vra_ricomparsa") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_ricomparsa")) Then
                                    sm_pa.ricomparsa_sintomi_dopo_risomministrazione = dateReaz("vra_ricomparsa")
                                End If
                            End If
                        End If
                        If Not dateReaz("noi_descrizione") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("noi_descrizione")) Then
							Dim indicTer As New indicazione_terapeutica
							indicTer.descrizione = dateReaz("noi_descrizione")
							If Not dateReaz("noi_codice_esterno") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("noi_codice_esterno")) Then
								indicTer.codice_llt = dateReaz("noi_codice_esterno")
							End If
							sm_pa.indicazione_terapeutica = indicTer
						End If

						listSmPa.Add(sm_pa)
					End If
				End If
				If numeroCicli = 1 Then
					'-----------------------------------------------------------------------------------------------------
					' FARMACO COMCOMITANTE 1
					'-----------------------------------------------------------------------------------------------------
					If Not dateReaz("vra_farmconc1_noc_codice") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_farmconc1_noc_codice")) Then
						'tag farmaco
						Dim prodotto_medic_farmc1 As New prodotto_medicinale
						Dim sm_pa_farm1 As New SM_PA()
						Dim farmaco1 As New farmaco
						prodotto_medic_farmc1.nome_farmaco = dateReaz("vra_farmconc1_noc_descrizione") '"GARDASIL*1SIR IM 0,5ML+DISP+2"
						If Not dateReaz("farmconc1_noc_codice_aic") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("farmconc1_noc_codice_aic")) Then
							prodotto_medic_farmc1.codice_farmaco = Left(dateReaz("farmconc1_noc_codice_aic"), 6) '"037311"
							prodotto_medic_farmc1.codice_confezione = Right(dateReaz("farmconc1_noc_codice_aic"), 3) '"154"
						End If

						farmaco1.Item = prodotto_medic_farmc1
						sm_pa_farm1.farmaco = farmaco1
						' Dati somministrzione
						sm_pa_farm1.tipo_somministrazione = tipo_somministrazione.C
						'If Not dateReaz("vra_farmconc1_richiamo") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_farmconc1_richiamo")) Then
						'	sm_pa_farm1.numero_dose_richiamo = dateReaz("vra_farmconc1_richiamo")
						'End If
						If Not dateReaz("vra_farmconc1_dose") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_farmconc1_dose")) Then
							sm_pa_farm1.numero_dose_richiamo = dateReaz("vra_farmconc1_dose")
						End If
						If Not dateReaz("vra_farmconc1_dataora_eff") Is Nothing AndAlso Not dateReaz("vra_farmconc1_dataora_eff") Is DBNull.Value Then
							Dim oraSom As DateTime = dateReaz("vra_farmconc1_dataora_eff")
							Dim ore As String = oraSom.Hour
							Dim minuti As String = oraSom.Minute
							sm_pa_farm1.ora_somministrazione = String.Format("{0}:{1}", ore.PadLeft(2, "0"), minuti.PadLeft(2, "0"))
						End If
						If (Not dateReaz("vra_farmconc1_lot_codice") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_farmconc1_lot_codice"))) Then
							sm_pa_farm1.lotto = dateReaz("vra_farmconc1_lot_codice")
							If Not dateReaz("vra_farmconc1_lot_data_scad") Is Nothing AndAlso Not dateReaz("vra_farmconc1_lot_data_scad") Is DBNull.Value Then
								sm_pa_farm1.data_scadenza = dateReaz("vra_farmconc1_lot_data_scad")
								sm_pa_farm1.data_scadenzaSpecified = True
							End If
						End If
						' Il farmaco sospeso è stato tolto in quanto se messo a S l'integrazione da errore in quanto non gestito dalle Api. Solo  per farmaco concomitante.
						' Azione intrapresa non può essere valorizzata per farmaci concomitante
						'If Not dateReaz("vra_farmconc1_sospeso") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_farmconc1_sospeso")) Then
						'    'sm_pa_farm1.farmaco_sospeso = dateReaz("vra_farmconc1_sospeso")
						'    sm_pa_farm1.azioni_intraprese = azioni_intraprese.Item01
						'    sm_pa_farm1.azioni_intrapreseSpecified = True
						'End If
						'If Not dateReaz("vra_farmconc1_migliorata") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_farmconc1_migliorata")) Then
						'    sm_pa_farm1.reazione_migliorata = dateReaz("vra_farmconc1_migliorata")
						'End If
						If Not dateReaz("farmconc1_noc_forma_farmac") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("farmconc1_noc_forma_farmac")) Then
							sm_pa_farm1.forma_farmaceutica = dateReaz("farmconc1_noc_forma_farmac")
						End If
						Dim dosaggio1 As Decimal
						If Not dateReaz("vra_farmconc1_dosaggio") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_farmconc1_dosaggio")) AndAlso Decimal.TryParse(dateReaz("vra_farmconc1_dosaggio"), dosaggio1) Then
							sm_pa_farm1.dosaggio = dosaggio1
							sm_pa_farm1.dosaggioSpecified = True
							sm_pa_farm1.unita_misura = unita_misura.DF
							sm_pa_farm1.unita_misuraSpecified = True
							sm_pa_farm1.frequenza_somministrazione = "A"
						End If
						If Not dateReaz("farmconc1_vii_codice_esterno") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("farmconc1_vii_codice_esterno")) Then
							sm_pa_farm1.via_somministrazione = CType([Enum].Parse(GetType(via_somministrazione), dateReaz("farmconc1_vii_codice_esterno")), via_somministrazione) 'dateReaz("vra_farmconc1_vii_codice_esterno")
							sm_pa_farm1.via_somministrazioneSpecified = True
						End If
						If Not dateReaz("vra_farmconc1_dataora_eff") Is Nothing AndAlso Not dateReaz("vra_farmconc1_dataora_eff") Is DBNull.Value Then
							sm_pa_farm1.data_inizio_terapia = dateReaz("vra_farmconc1_dataora_eff")
							sm_pa_farm1.data_inizio_terapiaSpecified = True
							sm_pa_farm1.data_fine_terapia = dateReaz("vra_farmconc1_dataora_eff")
							sm_pa_farm1.data_fine_terapiaSpecified = True
						End If
						'If Not dateReaz("vra_farmconc1_ripreso") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_farmconc1_ripreso")) Then
						'    sm_pa_farm1.ripresa_somministrazione = dateReaz("vra_farmconc1_ripreso")
						'End If
						'If Not dateReaz("vra_farmconc1_ricomparsa") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_farmconc1_ricomparsa")) Then
						'    sm_pa_farm1.ricomparsa_sintomi_dopo_risomministrazione = dateReaz("vra_farmconc1_ricomparsa")
						'End If

						If Not dateReaz("farmconc1_noi_descr") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("farmconc1_noi_descr")) Then
							Dim indicTer1 As New indicazione_terapeutica
							indicTer1.descrizione = dateReaz("farmconc1_noi_descr")
							If Not dateReaz("farmconc1_noi_codice_esterno") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("farmconc1_noi_codice_esterno")) Then
								indicTer1.codice_llt = dateReaz("farmconc1_noi_codice_esterno")
							End If
							sm_pa_farm1.indicazione_terapeutica = indicTer1
						End If
						listSmPa.Add(sm_pa_farm1)
					End If
					'-----------------------------------------------------------------------------------------------------
					' FARMACO COMCOMITANTE 2
					'-----------------------------------------------------------------------------------------------------
					If Not dateReaz("vra_farmconc2_noc_codice") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_farmconc2_noc_codice")) Then
						'tag farmaco
						Dim prodotto_medic_farmc2 As New prodotto_medicinale
						Dim sm_pa_farm2 As New SM_PA()
						Dim farmaco2 As New farmaco
						prodotto_medic_farmc2.nome_farmaco = dateReaz("vra_farmconc2_noc_descrizione") '"GARDASIL*1SIR IM 0,5ML+DISP+2"
						If Not dateReaz("farmconc2_noc_codice_aic") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("farmconc2_noc_codice_aic")) Then
							prodotto_medic_farmc2.codice_farmaco = Left(dateReaz("farmconc2_noc_codice_aic"), 6) '"037311"
							prodotto_medic_farmc2.codice_confezione = Right(dateReaz("farmconc2_noc_codice_aic"), 3) '"154"
						End If

						farmaco2.Item = prodotto_medic_farmc2
						sm_pa_farm2.farmaco = farmaco2
						' Dati somministrzione
						sm_pa_farm2.tipo_somministrazione = tipo_somministrazione.C
						'If Not dateReaz("vra_farmconc2_richiamo") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_farmconc2_richiamo")) Then
						'	sm_pa_farm2.numero_dose_richiamo = dateReaz("vra_farmconc2_richiamo")
						'End If
						If Not dateReaz("vra_farmconc2_dose") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_farmconc2_dose")) Then
							sm_pa_farm2.numero_dose_richiamo = dateReaz("vra_farmconc2_dose")
						End If
						If Not dateReaz("vra_farmconc2_dataora_eff") Is Nothing AndAlso Not dateReaz("vra_farmconc2_dataora_eff") Is DBNull.Value Then
							Dim oraSom As DateTime = dateReaz("vra_farmconc2_dataora_eff")
							Dim ore As String = oraSom.Hour
							Dim minuti As String = oraSom.Minute
							sm_pa_farm2.ora_somministrazione = String.Format("{0}:{1}", ore.PadLeft(2, "0"), minuti.PadLeft(2, "0"))
						End If
						If (Not dateReaz("vra_farmconc2_lot_codice") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_farmconc2_lot_codice"))) Then
							sm_pa_farm2.lotto = dateReaz("vra_farmconc2_lot_codice")
							If Not dateReaz("vra_farmconc2_lot_data_scad") Is Nothing AndAlso Not dateReaz("vra_farmconc2_lot_data_scad") Is DBNull.Value Then
								sm_pa_farm2.data_scadenza = dateReaz("vra_farmconc2_lot_data_scad")
								sm_pa_farm2.data_scadenzaSpecified = True
							End If
						End If
						' Azione intrapresa non può essere valorizzata per farmaci concomitante, altrimenti da errore l'api
						'If Not dateReaz("vra_farmconc2_sospeso") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_farmconc2_sospeso")) Then
						'    'sm_pa_farm2.farmaco_sospeso = dateReaz("vra_farmconc2_sospeso")
						'    sm_pa_farm2.azioni_intraprese = azioni_intraprese.Item01
						'    sm_pa_farm2.azioni_intrapreseSpecified = True
						'End If
						'If Not dateReaz("vra_farmconc2_migliorata") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_farmconc2_migliorata")) Then
						'    sm_pa_farm2.reazione_migliorata = dateReaz("vra_farmconc2_migliorata")
						'End If
						If Not dateReaz("farmconc2_noc_forma_farmac") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("farmconc2_noc_forma_farmac")) Then
							sm_pa_farm2.forma_farmaceutica = dateReaz("farmconc2_noc_forma_farmac")
						End If
						Dim dosaggio2 As Decimal
						If Not dateReaz("vra_farmconc2_dosaggio") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_farmconc2_dosaggio")) AndAlso Decimal.TryParse(dateReaz("vra_farmconc2_dosaggio"), dosaggio2) Then
							sm_pa_farm2.dosaggio = dosaggio2
							sm_pa_farm2.dosaggioSpecified = True
							sm_pa_farm2.unita_misura = unita_misura.DF
							sm_pa_farm2.unita_misuraSpecified = True
							sm_pa_farm2.frequenza_somministrazione = "A"
						End If
						If Not dateReaz("farmconc2_vii_codice_esterno") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("farmconc2_vii_codice_esterno")) Then
							sm_pa_farm2.via_somministrazione = CType([Enum].Parse(GetType(via_somministrazione), dateReaz("farmconc2_vii_codice_esterno")), via_somministrazione) 'dateReaz("vra_farmconc2_vii_codice_esterno")
							sm_pa_farm2.via_somministrazioneSpecified = True
						End If
						If Not dateReaz("vra_farmconc2_dataora_eff") Is Nothing AndAlso Not dateReaz("vra_farmconc2_dataora_eff") Is DBNull.Value Then
							sm_pa_farm2.data_inizio_terapia = dateReaz("vra_farmconc2_dataora_eff")
							sm_pa_farm2.data_inizio_terapiaSpecified = True
							sm_pa_farm2.data_fine_terapia = dateReaz("vra_farmconc2_dataora_eff")
							sm_pa_farm2.data_fine_terapiaSpecified = True
						End If
						'If Not dateReaz("vra_farmconc2_ripreso") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_farmconc2_ripreso")) Then
						'    sm_pa_farm2.ripresa_somministrazione = dateReaz("vra_farmconc2_ripreso")
						'End If
						'If Not dateReaz("vra_farmconc2_ricomparsa") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_farmconc2_ricomparsa")) Then
						'    sm_pa_farm2.ricomparsa_sintomi_dopo_risomministrazione = dateReaz("vra_farmconc2_ricomparsa")
						'End If

						If Not dateReaz("farmconc2_noi_descr") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("farmconc2_noi_descr")) Then
							Dim indicTer2 As New indicazione_terapeutica
							indicTer2.descrizione = dateReaz("farmconc2_noi_descr")
							If Not dateReaz("farmconc2_noi_codice_esterno") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("farmconc2_noi_codice_esterno")) Then
								indicTer2.codice_llt = dateReaz("farmconc2_noi_codice_esterno")
							End If
							sm_pa_farm2.indicazione_terapeutica = indicTer2
						End If
						listSmPa.Add(sm_pa_farm2)
					End If
				End If
				If numeroCicli = dateReazTable.Rows.Count Then
					' dati della scheda
					scheda_adr.Prodotti = listSmPa.ToArray()
				End If
				If numeroCicli = 1 Then


					scheda_adr.id_scheda = id
					scheda_adr.iniziali_nome = Left(dateReaz("PAZ_NOME"), 1)
					scheda_adr.iniziali_cognome = Left(dateReaz("PAZ_COGNOME"), 1)
					If Not dateReaz("PAZ_DATA_NASCITA") Is Nothing And Not dateReaz("PAZ_DATA_NASCITA") Is DBNull.Value Then
						scheda_adr.data_nascita = dateReaz("PAZ_DATA_NASCITA")
						scheda_adr.data_nascitaSpecified = True
					End If

                    scheda_adr.sesso = IIf(dateReaz("PAZ_SESSO") = "M", sesso.M, sesso.F)
                    scheda_adr.sessoSpecified = True
                    If Not dateReaz("VRA_PESO") Is Nothing AndAlso Not dateReaz("VRA_PESO") Is DBNull.Value Then
						'TODO[Reazione avversa] mettere conversione in parte intera
						scheda_adr.peso = Convert.ToInt32(dateReaz("VRA_PESO"))
					End If
					If Not dateReaz("VRA_ALTEZZA") Is Nothing AndAlso Not dateReaz("VRA_ALTEZZA") Is DBNull.Value Then
						scheda_adr.altezza = dateReaz("VRA_ALTEZZA")
					End If
					If Not dateReaz("VRA_DATA_REAZIONE") Is Nothing AndAlso Not dateReaz("VRA_DATA_REAZIONE") Is DBNull.Value Then
						scheda_adr.data_reazione = dateReaz("VRA_DATA_REAZIONE")
						scheda_adr.data_reazioneSpecified = True
					End If

					' fare verifiche dipende da VRA_GRAVIDANZA 
					If dateReaz("PAZ_SESSO") = "F" Then
						If Not dateReaz("VRA_GRAVIDANZA") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("VRA_DATA_REAZIONE")) AndAlso (dateReaz("VRA_GRAVIDANZA") = "1" OrElse dateReaz("VRA_GRAVIDANZA") = "2" OrElse dateReaz("VRA_GRAVIDANZA") = "3") Then
							scheda_adr.periodo_gestazionale = periodo_gestazionale.T
							scheda_adr.periodo_gestazionaleSpecified = True
							scheda_adr.eta_gestazionale = dateReaz("VRA_GRAVIDANZA")
						End If

					End If
					If Not dateReaz("OET_CODICE_ESTERNO") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("OET_CODICE_ESTERNO")) Then
						scheda_adr.origine_etnica = dateReaz("OET_CODICE_ESTERNO")
					End If
					'Calcolo descrizione della reazione avversa max 255 
					Dim reaDescr As String = ""
					If Not dateReaz("rea_descrizione") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("rea_descrizione")) Then
						reaDescr = dateReaz("rea_descrizione")
					End If
					If Not dateReaz("rea_descrizione1") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("rea_descrizione1")) Then
						reaDescr = reaDescr + ", " + dateReaz("rea_descrizione1")
					End If
					If Not dateReaz("rea_descrizione2") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("rea_descrizione2")) Then
						reaDescr = reaDescr + ", " + dateReaz("rea_descrizione2")
					End If
					If Not dateReaz("vra_rea_altro") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_rea_altro")) Then
						reaDescr = reaDescr + ", " + dateReaz("vra_rea_altro")
					End If
					If reaDescr <> "" Then
						scheda_adr.descrizione_reazione = Left(reaDescr, 255) 'funzione che restituisce descrizioni delle rea VRA_REA_CODICE,VRA_RE1_CODICE,VRA_RE2_CODICE,VRA_REA_ALTRO
					End If
					'dipende da VRA_GRAVE e VRA_GRAVITA_REAZIONE
					If dateReaz("vra_grave") = "N" Then
						scheda_adr.gravita_tutte_voci = "N"
					ElseIf (Not dateReaz("mgr_codice_esterno") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("mgr_codice_esterno"))) Then
						scheda_adr.gravita_tutte_voci = dateReaz("mgr_codice_esterno")
					Else
						scheda_adr.gravita_tutte_voci = "N"
					End If
					' altre sostanze
					If Not dateReaz("vra_uso_concomitante") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_uso_concomitante")) Then
						scheda_adr.altre_sostanze = dateReaz("vra_uso_concomitante")
					End If
					' mappare dati di VRA_ESITO con lista valori
					If Not dateReaz("mer_codice_esterno") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("mer_codice_esterno")) Then
						scheda_adr.esito = dateReaz("mer_codice_esterno")
						If Not dateReaz("VRA_DATA_ESITO") Is Nothing AndAlso Not dateReaz("VRA_DATA_ESITO") Is DBNull.Value Then
							scheda_adr.data_esito = dateReaz("VRA_DATA_ESITO")
							scheda_adr.data_esitoSpecified = True ' presente solo se c'è esito VRA_DATA_ESITO
						End If
					End If
					If Not dateReaz("mmr_codice_esterno") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("mmr_codice_esterno")) Then
						scheda_adr.relazione_adr_decesso = dateReaz("mmr_codice_esterno") 'VRA_MOTIVO_DECESSO se no
					End If
					If Not dateReaz("vra_terapia") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_terapia")) Then
						scheda_adr.azioni = dateReaz("vra_terapia")
					End If
					If Not dateReaz("mqr_codice_esterno") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("mqr_codice_esterno")) Then
						scheda_adr.fonte = CType([Enum].Parse(GetType(fonte), String.Format("Item{0}", dateReaz("mqr_codice_esterno"))), fonte)
						'valore calcolato rispetto al campo VRA_QUALIFICA
						'If Not dateReaz("vra_altra_qualifica") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_altra_qualifica")) AndAlso dateReaz("vra_qualifica") = "ALTRO" Then
						'scheda_adr.altra_fonte = Left(dateReaz("vra_altra_qualifica"), 50) 'VRA_ALTRA_QUALIFICA max 50
						'End If
					Else
						scheda_adr.fonte = fonte.Item3
					End If

					Dim idUtente As Long = ContextInfos.IDUtente
					Dim datiUte As Entities.Utente = Nothing
					Dim consult As Entities.Cns = Nothing
					consult = GenericProvider.Consultori.GetConsultorio(consultorio)
					datiUte = GenericProvider.Utenti.GetUtente(idUtente)
					Dim cognome As String = Nothing

					' dati del segnalatore
					If Not dateReaz("vra_nome_segnalatore") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_nome_segnalatore")) Then
						datisegn.nome = dateReaz("vra_nome_segnalatore") 'non obbligatorio
					End If
					' essendo il cognome obbligatorio faccio una sequenza di assegnazioni
					' se segnalatore è vuoto, provo cognome utente altrimenti metto descrizione consultorio
					If Not dateReaz("vra_cognome_segnalatore") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_cognome_segnalatore")) Then
						cognome = dateReaz("vra_cognome_segnalatore")
					ElseIf Not String.IsNullOrWhiteSpace(datiUte.Cognome) Then
						cognome = datiUte.Cognome
					Else
						cognome = consult.Descrizione
					End If
					datisegn.cognome = cognome
					If Not dateReaz("vra_indirizzo_segnalatore") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_indirizzo_segnalatore")) Then
						datisegn.indirizzo = dateReaz("vra_indirizzo_segnalatore") 'non obbligatorio
					End If
					If (Not dateReaz("vra_tel_segnalatore") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_tel_segnalatore"))) OrElse (Not dateReaz("vra_fax_segnalatore") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_fax_segnalatore"))) Then
						datisegn.telefono_fax = dateReaz("vra_tel_segnalatore") + " " + dateReaz("vra_fax_segnalatore") 'non obbligatorio
					End If
					Dim emailSegnalatore As String = ""
					'Essendo la mail obbligatoria cerco di ricavare una mail con la seguente logica: prima segnalarore poi utente ed in fine quella del consultorio
					If Not dateReaz("vra_mail_segnalatore") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_mail_segnalatore")) Then
						emailSegnalatore = dateReaz("vra_mail_segnalatore")
					ElseIf Not String.IsNullOrWhiteSpace(datiUte.Email) Then
						emailSegnalatore = datiUte.Email
					Else
						emailSegnalatore = consult.Email
					End If
					datisegn.e_mail = emailSegnalatore
					'TODO: COME CODIFICARE REGIONE E AZIENDA
					Dim regione As String = ""
					Dim codiceAusl As String = ContextInfos.CodiceUsl
					Dim codAuslAifa As String = String.Empty
					'Recupero regione con codice ausl
					Using ausl As New BizUsl(GenericProvider, ContextInfos)
						regione = ausl.GetCodiceRegione(codiceAusl)
						codAuslAifa = ausl.GetCodiceAifa(codiceAusl)
					End Using

					datisegn.codice_regione = CType([Enum].Parse(GetType(codice_regione), String.Format("Item{0}", regione)), codice_regione) 'regione codice ulss prime 3 cifre
					datisegn.az_sanitaria = Right(codAuslAifa, 3) 'codice ulss ultime 3 cifre
					segnalatore.Item = datisegn
					scheda_adr.segnalatore = segnalatore
					If Not dateReaz("vra_data_compilazione") Is Nothing AndAlso Not dateReaz("vra_data_compilazione") Is DBNull.Value Then
						scheda_adr.data_compilazione = dateReaz("vra_data_compilazione")
					End If
					'TODO[VIGIFARMACO NUOVA VERSIONE 1.5]
					Dim studioVar As New Classificazione
					studioVar.tipo_segnalazione = tipo_segnalazione.Item3
					If Not dateReaz("vra_ambito_osservazione") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_ambito_osservazione")) Then
						If dateReaz("vra_ambito_osservazione") = 1 OrElse dateReaz("vra_ambito_osservazione") = 3 Then
							If dateReaz("vra_ambito_osservazione") = 1 Then
								studioVar.tipo_segnalazione = tipo_segnalazione.Item1
								'studioVar.tipo_studio = tipo_studio.Item1
								'studioVar.nome_studio = "Farmacovigilanza Attiva"
							End If
							If dateReaz("vra_ambito_osservazione") = 3 Then
								studioVar.tipo_segnalazione = tipo_segnalazione.Item2
								studioVar.tipo_studio = tipo_studio.Item2
								studioVar.tipo_studioSpecified = True
								Dim titoloStudio As String = "NON SPECIFICATO"
								If Not dateReaz("vra_ambito_studio_titolo") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_ambito_studio_titolo")) Then
									titoloStudio = dateReaz("vra_ambito_studio_titolo").ToString()
								End If
								studioVar.nome_studio = titoloStudio
							End If

						End If

					End If
					scheda_adr.Classificazione = studioVar
					' aggiunto descrizione della visita che  ne
					If Not dateReaz("vra_visita") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_visita")) Then

						Dim esami As New List(Of esame)
						Dim esame As New esame
						esame.descrizione = dateReaz("vra_visita")
						esami.Add(esame)

						scheda_adr.esami_laboratorio = esami.ToArray()
					End If
					'' Aggiungo commento. Se sono valorizzatti il sito inoculazione e altre informazioni 
					Dim commento As String = String.Empty
					If Not dateReaz("sii_descrizione") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("sii_descrizione")) Then
						commento = String.Format("{0} {1}", "Sito inoculazione:", dateReaz("sii_descrizione"))
					End If
					If Not dateReaz("vra_altre_informazioni") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dateReaz("vra_altre_informazioni")) Then
						commento = String.Format("{0} {1}", commento, "Altre informazioni: " + dateReaz("vra_altre_informazioni"))
					End If
					' se commento è valorizzato allora aggiungo a dati da inviare
					If Not String.IsNullOrEmpty(commento) Then
						scheda_adr.commento_segnalatore = commento
					End If


				End If

				numeroCicli = numeroCicli + 1
			Next
			' segnalazione o root
			seg.Scheda_adr = scheda_adr
			' serializzo dati per creare xml
			Dim ser As New XmlSerializer(GetType(Segnalazioni), "Segnalazioni")

			Dim Write As New StringWriter()
			ser.Serialize(Write, seg)
			Dim serString As String = String.Empty
			serString = Write.ToString()
			' modifico utf in quanto la stringwriter genera utf-16
			ret.Xml = serString.Replace("utf-16", "utf-8")
			ret.idScheda = id
		End Using

		Return ret

	End Function
	Private Function MappingCodAuslOldCodiceAifa(codiUsl As String) As String
		Dim retUsl As String = String.Empty
		' Dovranno diventare Azienda ULSS n. 1 Dolomitica (050501)
		If codiUsl = "050101" Then
			retUsl = "050101"
		End If
		If codiUsl = "050102" Then
			retUsl = "050102"
		End If
		'Azienda ULSS n. 7 Pedemontana
		If codiUsl = "050103" Then
			retUsl = "050507"
		End If
		If codiUsl = "050104" Then
			retUsl = "050507"
		End If
		' Azienda ULSS n. 8 Berica
		If codiUsl = "050105" Then
			retUsl = "050508"
		End If
		If codiUsl = "050106" Then
			retUsl = "050508"
		End If
		' Azienda ULSS n. 2 Marca Trevigiana
		If codiUsl = "050107" Then
			retUsl = "050502"
		End If
		If codiUsl = "050108" Then
			retUsl = "050502"
		End If
		If codiUsl = "050109" Then
			retUsl = "050502"
		End If
		' Azienda ULSS n. 4 Veneto Orientale
		If codiUsl = "050110" Then
			retUsl = "050504"
		End If
		' Azienda ULSS n. 3 Serenissima
		If codiUsl = "050112" Then
			retUsl = "050503"
		End If
		If codiUsl = "050113" Then
			retUsl = "050503"
		End If
		If codiUsl = "050114" Then
			retUsl = "050503"
		End If
		' Azienda ULSS n. 6 Euganea 
		If codiUsl = "050115" Then
			retUsl = "050506"
		End If
		If codiUsl = "050116" Then
			retUsl = "050506"
		End If
		If codiUsl = "050117" Then
			retUsl = "050506"
		End If
		' Azienda ULSS n. 5 Polesana
		If codiUsl = "050118" Then
			retUsl = "050505"
		End If
		If codiUsl = "050119" Then
			retUsl = "050505"
		End If
		' Dovranno diventare Azienda ULSS n. 9 Scaligera 050509
		If codiUsl = "050120" Then
			retUsl = "050120"
		End If
		If codiUsl = "050121" Then
			retUsl = "050121"
		End If
		If codiUsl = "050122" Then
			retUsl = "050122"
		End If

		Return retUsl
	End Function
#End Region
	Public Class ResultXml
        Public Xml As String
        Public idScheda As String
    End Class




#End Region

End Class