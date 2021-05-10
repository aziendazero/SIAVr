Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Enumerators
Imports Onit.OnAssistnet.OnVac.Entities
Imports System.Text

Public Class BizACN
	Inherits BizClass

#Region " Constructors "

	Public Sub New(genericProvider As DbGenericProvider, settings As Settings.Settings, contextInfo As BizContextInfos)

		MyBase.New(genericProvider, settings, contextInfo, Nothing)

	End Sub

#End Region

#Region " Public "

#Region " Decodifica dei campi "
	Public Function GetAssociazioneByCodiceACN(codiceACN As String, codiceAic As String) As AssociazioneACN
		Dim retGet As AssociazioneAnag = New AssociazioneAnag
		Dim ret As AssociazioneACN = New AssociazioneACN
		Dim tipoAss As String = String.Empty
		If String.IsNullOrWhiteSpace(codiceAic) Then
			retGet = GenericProvider.Associazioni.GetAssociazioneByCodiceACN(codiceACN)
			'ret.Codice = retGet.Codice
			tipoAss = "Associazione solo con codice ACN"
		Else
			retGet = GenericProvider.Associazioni.GetAssociazioneByCodiceACNAic(codiceACN, codiceAic)
			' Se non viene restituito nulla provo a trovare associazione per codice aic
			If retGet Is Nothing Then
				retGet = GenericProvider.Associazioni.GetAssociazioneByCodiceACNAic(String.Empty, codiceAic)
				tipoAss = "Associazione con codice AIC"
			Else
				tipoAss = "Associazione con codice ACN e codice AIC"
			End If
		End If
		If Not retGet Is Nothing Then
			ret.Codice = retGet.Codice
			ret.TipoAssociazione = tipoAss
		End If

		Return ret

	End Function

	Public Function GetCodiceSitoInoculazioneByCodiceACN(codiceEsterno As String) As String

		Return GenericProvider.SitiInoculazione.GetCodiceSitoInoculazioneByCodiceACN(codiceEsterno)

	End Function

	Public Function GetCodiceViaSomministrazioneByCodiceACN(codiceEsterno As String) As String

		Return GenericProvider.VieSomministrazione.GetCodiceViaSomministrazioneByCodiceACN(codiceEsterno)

	End Function

	Public Function GetCodiceNomeCommercialeByCodiceAic(codiceAIC As String) As String

		Return GenericProvider.NomiCommerciali.GetCodiceNomeCommercialeByCodiceAic(codiceAIC)

	End Function

	Public Function GetMalattieByCodiciACN(listaCodiciEsterni As List(Of String)) As List(Of Malattia)

		Return GenericProvider.Malattie.GetMalattieByCodiciACN(listaCodiciEsterni)

	End Function

	Public Function GetGuidTipoPagamentoByCodiceACN(codiceEsterno As String) As Byte()

		Return GenericProvider.NomiCommerciali.GetGuidTipoPagamentoByCodiceACN(codiceEsterno)

	End Function
	Public Function GetCondizioneDiRischioACN(codiceEsternoACN As String) As String
		Return GenericProvider.CategorieRischio.GetCodCategoriaRischioCodiceACN(codiceEsternoACN)
	End Function


	Public Function GetListVaccinazioniByListCodiciACN(codiciEsterni As List(Of String), codAssociazione As String) As List(Of VaccinazioneAssociazione)

		Dim listVaccinazioni As New List(Of VaccinazioneAssociazione)()

		If Not codiciEsterni Is Nothing AndAlso codiciEsterni.Count > 0 Then

			For Each codEsterno As String In codiciEsterni

				Dim vaccAss As VaccinazioneAssociazione = GenericProvider.Associazioni.GetVaccByCodiceACN(codEsterno, codAssociazione)
				If Not vaccAss Is Nothing Then
					listVaccinazioni.Add(vaccAss)
				End If
			Next

		End If

		Return listVaccinazioni

	End Function

	Public Function GetUlssByAcn(codiceACN As String) As String
		Dim ulss As UslUnificata = GenericProvider.Usl.GetUslUnificataByCodiceAcn(codiceACN)
		Dim ret As String = String.Empty
		If Not ulss Is Nothing Then
			ret = ulss.CodiceUsl
		End If

		Return ret
	End Function


#End Region




#Region " Ricerca Paziente ACN "

	Public Class CercaPazienteACNResult
		Inherits BizGenericResult

		Public IsBloccante As Boolean 'usato solo per la ricerca ByIdACN
		Public PazienteACN As Paziente

	End Class


	Public Function CercaPazienteByIdACN(idACN As String) As CercaPazienteACNResult

		Dim result As New CercaPazienteACNResult()
		result.IsBloccante = False

		If String.IsNullOrWhiteSpace(idACN) Then

			result.Success = False
			result.Message = "Il valore di 'CodicePaziente' restituito dalla chiamata è null"
			result.PazienteACN = Nothing
			result.IsBloccante = True

		Else

			Dim codici As List(Of String) = GenericProvider.Paziente.GetCodicePazientiByIdACN(idACN)

			If codici Is Nothing OrElse codici.Count = 0 Then
				result.Success = False
				result.Message = "Paziente con ID ACN " + idACN + " non trovato."
				result.PazienteACN = Nothing
				result.IsBloccante = False
			Else
				If codici.Count > 1 Then
					result.Success = False
					result.Message = "Trovati più pazienti con ID ACN " + idACN + "."
					result.IsBloccante = True
				Else
					Dim paz As Paziente = Nothing
					Using bizPaz As New BizPaziente(GenericProvider, Settings, ContextInfos, Nothing)
						paz = bizPaz.CercaPaziente(codici(0))
					End Using
					If Not paz Is Nothing Then
						result.Success = True
						result.PazienteACN = paz
						result.IsBloccante = False
					Else
						result.Success = False
						result.Message = "Paziente con ID ACN " + idACN + " non trovato."
						result.PazienteACN = Nothing
						result.IsBloccante = False
					End If
				End If
			End If

		End If


		Return result

	End Function


	Public Function CercaPazienteByMPI(codiceRegionale As String) As CercaPazienteACNResult

		Dim result As New CercaPazienteACNResult()
		result.IsBloccante = False

		If String.IsNullOrWhiteSpace(codiceRegionale) Then

			result.Success = False
			result.Message = "Il Codice Regionale è null"
			result.PazienteACN = Nothing
			result.IsBloccante = True

		Else

			Dim codici As List(Of String) = GenericProvider.Paziente.GetCodicePazientiByCodiceRegionale(codiceRegionale).ToList()

			If codici Is Nothing OrElse codici.Count = 0 Then
				result.Success = False
				result.Message = "Paziente con Codice Regionale " + codiceRegionale + " non trovato."
				result.PazienteACN = Nothing
				result.IsBloccante = False
			Else
				If codici.Count > 1 Then
					result.Success = False
					result.Message = "Trovati più pazienti con Codice Regionale " + codiceRegionale + "."
					result.IsBloccante = True
				Else
					Dim paz As Paziente = Nothing
					Using bizPaz As New BizPaziente(GenericProvider, Settings, ContextInfos, Nothing)
						paz = bizPaz.CercaPaziente(codici(0))
					End Using
					If Not paz Is Nothing Then
						result.Success = True
						result.PazienteACN = paz
						result.IsBloccante = False
					Else
						result.Success = False
						result.Message = "Paziente con Codice Regionale " + codiceRegionale + " non trovato."
						result.PazienteACN = Nothing
						result.IsBloccante = False
					End If
				End If
			End If

		End If


		Return result

	End Function


	Public Function UpdateIdACNPaziente(codicePaziente As Long, idACN As String) As Integer

		Using bizPaz As New BizPaziente(GenericProvider, Settings, ContextInfos, LogOptions)
			Return bizPaz.UpdateIdACNPaziente(codicePaziente, idACN)
		End Using

	End Function

#End Region


#Region " Ricerca Operatore/Medico ACN "

	Public Class CercaOperatoreACNResult
		Inherits BizGenericResult

		Public OperatoreACN As Operatore

	End Class

	Public Function CercaOperatoreByIdACN(idACN As String) As CercaOperatoreACNResult

		Dim result As New CercaOperatoreACNResult()

		If String.IsNullOrWhiteSpace(idACN) Then

			result.Success = False
			result.Message = "Il valore di 'EsecutoreVaccinazione' restituito dalla chiamata è null"
			result.OperatoreACN = Nothing

		Else

			Dim operatore As Operatore = GenericProvider.Operatori.GetOperatoreByIdACN(idACN)

			If operatore Is Nothing Then
				result.Success = False
				result.Message = "Operatore con ID ACN " + idACN + " non trovato."
				result.OperatoreACN = Nothing
			Else
				result.Success = True
				result.OperatoreACN = operatore
			End If

		End If


		Return result

	End Function

	Public Function CercaOperatoreByCodiceFiscale(codiceFiscale As String) As Operatore

		Dim operatore As Operatore = Nothing

		If Not String.IsNullOrWhiteSpace(codiceFiscale) Then

			operatore = GenericProvider.Operatori.GetOperatoreByCodiceFiscale(codiceFiscale)

		End If

		Return operatore

	End Function

	Public Function InsertOperatore(codiceRegionale As String, codiceFiscale As String, nome As String, cognome As String, codiceACN As String) As Operatore

		Dim operatore As Operatore = Nothing

		If Not String.IsNullOrWhiteSpace(codiceFiscale) And Not String.IsNullOrWhiteSpace(codiceACN) And (Not String.IsNullOrWhiteSpace(nome) Or Not String.IsNullOrWhiteSpace(cognome)) Then

			Dim nomeCognome As String = cognome
			If Not String.IsNullOrWhiteSpace(nome) Then nomeCognome += "*" + nome

			operatore = New Operatore() With {
				.Codice = codiceRegionale,
				.CodiceFiscale = codiceFiscale,
				.Nome = nomeCognome,
				.CodiceEsterno = String.Empty,
				.Qualifica = String.Empty,
				.CodiceACN = codiceACN}

			GenericProvider.Operatori.InsertOperatore(operatore)

		End If

		Return operatore

	End Function

	Public Function UpdateIdACNOperatore(codiceOperatore As String, idACN As String) As Integer

		Using bizOpe As New BizOperatori(GenericProvider, Settings, ContextInfos)
			Return bizOpe.UpdateIdACNOperatore(codiceOperatore, idACN)
		End Using

	End Function

	Public Function ExistsMedico(codiceRegionale As String) As Boolean

		Dim result As Boolean = False

		If Not String.IsNullOrWhiteSpace(codiceRegionale) Then

			result = GenericProvider.Medico.ExistsMedico(codiceRegionale)

		End If

		Return result

	End Function

	Public Function InsertMedico(codiceRegionale As String, codiceFiscale As String, nome As String, cognome As String) As Medico

		Dim medico As Medico = Nothing

		If Not String.IsNullOrWhiteSpace(codiceRegionale) And Not String.IsNullOrWhiteSpace(codiceFiscale) And Not String.IsNullOrWhiteSpace(nome) And Not String.IsNullOrWhiteSpace(cognome) Then

			medico = New Medico() With {
				.Codice = codiceRegionale,
				.CodiceFiscale = codiceFiscale,
				.CodiceRegionale = codiceRegionale,
				.Nome = nome,
				.Cognome = cognome,
				.Descrizione = cognome + "*" + nome}

			GenericProvider.Medico.InsertMedico(medico)

		End If

		Return medico

	End Function


#End Region



	Public Class SalvaVaccinazioneEseguitaACNCommand

		Public Property CodiceLocalePaziente As Long
		Public Property DtVaccinazioniEseguite As DataTable

	End Class


	Public Class SalvaVaccinazioneEseguitaACNResult
		Public Property Success As Boolean
		Public Property Messaggio As String
	End Class


	Public Function SalvaVaccinazioneEseguita(command As SalvaVaccinazioneEseguitaACNCommand) As SalvaVaccinazioneEseguitaACNResult

		Dim result As New SalvaVaccinazioneEseguitaACNResult()
		result.Success = True

		Try
			Using bizVaccEseguite As New BizVaccinazioniEseguite(GenericProvider, Settings, ContextInfos, LogOptions)
				bizVaccEseguite.Salva(command.CodiceLocalePaziente, command.DtVaccinazioniEseguite)
			End Using
		Catch ex As Exception
			result.Success = False
			result.Messaggio = ex.ToString()
		End Try

		Return result

	End Function


	Public Function GetVaccinazioniAssociazioni(listCodiciAssociazioni As List(Of String)) As List(Of VaccinazioneAssociazione)

		Dim listVaccAss As List(Of VaccinazioneAssociazione) = GenericProvider.Associazioni.GetVaccinazioniAssociazioni(listCodiciAssociazioni)

		Return listVaccAss

	End Function


	Public Class GetVaccinazioniByIdACNResult
		Inherits BizGenericResult
		Public ListaVaccinazioni As List(Of VaccinazioneIntegrazioneDB)
	End Class

	Public Function GetVaccinazioniByIdACN(idACN As String, codicePaziente As Long, codiceAssociazione As String, tipoOperazione As OperazioneLogNotifica) As GetVaccinazioniByIdACNResult

		Dim result As New GetVaccinazioniByIdACNResult()
		result.Success = True
		result.ListaVaccinazioni = New List(Of VaccinazioneIntegrazioneDB)()

		Dim listaVaccByIdAcn As List(Of VaccinazioneIntegrazioneDB) = GenericProvider.VaccinazioniEseguite.GetVaccinazioniEseguiteUnionScadute(Nothing, idACN, String.Empty, Nothing, String.Empty)


		If Not listaVaccByIdAcn Is Nothing AndAlso listaVaccByIdAcn.Count > 0 Then

			'escludo quelle scadute
			'listaVaccByIdAcn.RemoveAll(Function(x) x.Scaduta = True)
			result.ListaVaccinazioni = listaVaccByIdAcn

			If listaVaccByIdAcn.Any(Function(x) x.Provenienza <> ProvenienzaVaccinazioni.ACN) Then
				result.Success = False
				result.Message = "In OnVac sono presenti vaccinazioni con lo stesso idAcn ma con provenienza non impostata correttamente."
			Else
				If listaVaccByIdAcn.Any(Function(x) x.CodicePaziente <> codicePaziente) Then
					If tipoOperazione = OperazioneLogNotifica.EliminazioneVaccinazioniEseguite Then
						result.Success = False
						result.Message = "In OnVac sono presenti vaccinazioni con lo stesso idAcn ma con codice paziente diverso."
					End If
				Else
					If listaVaccByIdAcn.Any(Function(x) x.AssociazioneCod <> codiceAssociazione) Then
						If tipoOperazione = OperazioneLogNotifica.EliminazioneVaccinazioniEseguite Then
							result.Success = False
							result.Message = "In OnVac sono presenti vaccinazioni con lo stesso idAcn ma con codice associazione diverso."
						End If
					End If
				End If
			End If
		End If

		Return result

	End Function

	''' <summary>
	''' Funzione che verifica se esiste la stessa vaccinazione in stessa data e stessa associazione
	''' </summary>
	''' <param name="codicePaziente"></param>
	''' <param name="idACN"></param>
	''' <param name="codiceAssociazione"></param>
	''' <param name="dataEffettuazione"></param>
	''' <returns></returns>
	Public Function GetVaccinazioniSovrapposte(codicePaziente As Long, codiceAssociazione As String, dataEffettuazione As Date) As List(Of VaccinazioneIntegrazioneDB)

		Dim listaVaccSovrapposte As List(Of VaccinazioneIntegrazioneDB) = GenericProvider.VaccinazioniEseguite.GetVaccinazioniEseguiteUnionScadute(codicePaziente, String.Empty, codiceAssociazione, dataEffettuazione, String.Empty)

		If Not listaVaccSovrapposte Is Nothing AndAlso listaVaccSovrapposte.Count > 0 Then
			'escludo quelle scadute
			listaVaccSovrapposte.RemoveAll(Function(x) x.Scaduta = True)
		End If

		Return listaVaccSovrapposte

	End Function



	Public Class DtVaccinazioniEseguiteCommand

		Public Property CodiceLocalePaziente As Long
		Public Property ListVacDaInserire As List(Of VaccinazioneIntegrazioneDB)
		Public Property ListVacDaModificare As List(Of VaccinazioneIntegrazioneDB)
		Public Property ListVacDaEliminare As List(Of VaccinazioneIntegrazioneDB)

	End Class


	Public Function CreaDtVaccinazioniEseguite(command As DtVaccinazioniEseguiteCommand) As DataTable

		Dim now As Date = Date.Now
		Dim dt_vacEseguite As DataTable = Nothing

		'-------------------- AGGIUNGO AL DATATABLE LE RIGHE CON LE EVENTUALI VACCINAZIONI ESISTENTI: ------------------------
		Using bizVaccEseguite As New BizVaccinazioniEseguite(GenericProvider, Settings, ContextInfos, LogOptions)
			dt_vacEseguite = bizVaccEseguite.GetVaccinazioniEseguite(command.CodiceLocalePaziente, False)
		End Using

		'questo non serve, tanto viene creata la struttura nel punto precedente)
		''-------------------- CREO IL DATATABLE VUOTO (SE NON GIA' CREATO AL PASSO PRECEDENTE): ------------------------
		'If dt_vacEseguite Is Nothing Then
		'    dt_vacEseguite = CreaNuovoDtVaccinazioniEseguite(command.CodiceLocalePaziente)
		'End If

		'-------------------- CREO LA PRIMARY KEY DEL DATATABLE: ------------------------
		'Dim key(4) As DataColumn
		'key(0) = dt_vacEseguite.Columns("paz_codice")
		'key(1) = dt_vacEseguite.Columns("ves_vac_codice")
		'key(2) = dt_vacEseguite.Columns("ves_n_richiamo")
		'key(3) = dt_vacEseguite.Columns("ves_data_effettuazione")
		'key(4) = dt_vacEseguite.Columns("ves_ass_codice")
		'dt_vacEseguite.PrimaryKey = key
		'Dim keyMod(3) As DataColumn
		'keyMod(1) = dt_vacEseguite.Columns("paz_codice")
		'keyMod(1) = dt_vacEseguite.Columns("ves_vac_codice")
		'keyMod(2) = dt_vacEseguite.Columns("ves_n_richiamo")
		'keyMod(3) = dt_vacEseguite.Columns("ves_ass_codice")
		'dt_vacEseguite.PrimaryKey = keyMod

		'-------------------- AGGIUNGO AL DATATABLE LE VACCINAZIONI DA INSERIRE: ------------------------
		If Not command.ListVacDaInserire Is Nothing AndAlso command.ListVacDaInserire.Count > 0 Then
			For Each vacDaInserire As VaccinazioneIntegrazioneDB In command.ListVacDaInserire
				Dim controllo As Boolean = True
				For index As Integer = 0 To dt_vacEseguite.Rows.Count - 1
					Dim row As DataRow = dt_vacEseguite.Rows.Item(index)
					If Not row Is Nothing Then
						If Not row.IsNull("ves_id_acn") AndAlso Convert.ToInt64(row("ves_id_acn")) = vacDaInserire.IdACN Then
							controllo = False
						End If
					End If
				Next
				If controllo Then
					Dim drow As DataRow = dt_vacEseguite.NewRow()
					drow("paz_codice") = command.CodiceLocalePaziente
					drow("ves_data_effettuazione") = vacDaInserire.DataEffettuazione
					drow("ves_dataora_effettuazione") = vacDaInserire.DataOraEffettuazione
					drow("ves_ass_codice") = vacDaInserire.AssociazioneCod
					drow("ass_descrizione") = vacDaInserire.AssociazioneDescr
					drow("ves_ass_n_dose") = vacDaInserire.NrDoseAssociazione
					drow("ves_vac_codice") = vacDaInserire.CodiceVaccinazione
					drow("vac_descrizione") = vacDaInserire.DescrVaccinazione
					drow("ves_n_richiamo") = vacDaInserire.NrDoseVaccinazione
					drow("ves_vii_codice") = vacDaInserire.CodiceSomministrazione
					drow("ves_sii_codice") = vacDaInserire.CodiceInoculazione
					drow("ves_in_campagna") = vacDaInserire.InCampagna
					drow("ves_mal_codice_malattia") = vacDaInserire.CodiceMalattia
					drow("ves_codice_esenzione") = vacDaInserire.CodiceEsenzione
					drow("ves_noc_codice") = vacDaInserire.CodiceNomeCommerciale
					drow("ves_luogo") = vacDaInserire.Luogo
					drow("ves_cns_codice") = vacDaInserire.CodiceConsultorio
					drow("ves_ope_codice") = vacDaInserire.CodiceMedico
					drow("ves_med_vaccinante") = vacDaInserire.CodiceVaccinatore
					drow("ves_lot_codice") = vacDaInserire.CodiceLotto

					If vacDaInserire.IsMedicoInAmbulatorio.HasValue Then
						drow("ves_ope_in_ambulatorio") = IIf(vacDaInserire.IsMedicoInAmbulatorio, "S", "N")
					Else
						drow("ves_ope_in_ambulatorio") = Nothing
					End If
					drow("ves_note") = vacDaInserire.Note
					drow("scaduta") = IIf(vacDaInserire.Scaduta, "S", "N")
					drow("ves_stato") = vacDaInserire.Stato
					drow("ves_id_acn") = vacDaInserire.IdACN
					If vacDaInserire.Provenienza.HasValue Then
						drow("ves_provenienza") = [Enum].GetName(GetType(Enumerators.ProvenienzaVaccinazioni), vacDaInserire.Provenienza.Value).ToUpper()
					Else
						drow("ves_provenienza") = Nothing
					End If
					drow("ves_rsc_codice") = vacDaInserire.CodiceCondizioneRischio
					drow("ves_mal_codice_cond_sanitaria") = vacDaInserire.CodiceMalCondizioneSanitaria
					drow("ves_tpa_guid_tipi_pagamento") = vacDaInserire.TipoPagamento.ToByteArray()
					drow("ves_lot_data_scadenza") = vacDaInserire.DataScadenzaLotto
					drow("ves_usl_inserimento") = vacDaInserire.Ulss
					drow("ves_tipo_associazione_acn") = vacDaInserire.TipoAssociazione

					dt_vacEseguite.Rows.Add(drow)
				End If


			Next
		End If


		'-------------------- SETTO NEL DATATABLE LE VACCINAZIONI DA MODIFICARE: ------------------------
		If Not command.ListVacDaModificare Is Nothing AndAlso command.ListVacDaModificare.Count > 0 Then
			For Each vacDaModificare As VaccinazioneIntegrazioneDB In command.ListVacDaModificare

				'Dim rowKey(4) As Object
				'rowKey(0) = vacDaModificare.CodicePaziente
				'rowKey(1) = vacDaModificare.CodiceVaccinazione
				'rowKey(2) = vacDaModificare.NrDoseVaccinazione
				'rowKey(3) = vacDaModificare.DataEffettuazione
				'rowKey(4) = vacDaModificare.AssociazioneCod
				'Dim rowKey(3) As Object
				'rowKey(0) = vacDaModificare.CodicePaziente
				'rowKey(1) = vacDaModificare.CodiceVaccinazione
				'rowKey(2) = vacDaModificare.NrDoseVaccinazione
				'rowKey(3) = vacDaModificare.AssociazioneCod
				'Dim row As DataRow = dt_vacEseguite.Rows.Find(rowKey)
				For index As Integer = 0 To dt_vacEseguite.Rows.Count - 1
					Dim row As DataRow = dt_vacEseguite.Rows.Item(index)

					If Not row Is Nothing Then
						If Not row.IsNull("ves_id_acn") AndAlso Convert.ToInt64(row("ves_id_acn")) = vacDaModificare.IdACN Then
							row.SetModified()
							row("paz_codice") = command.CodiceLocalePaziente
							row("ves_data_effettuazione") = vacDaModificare.DataEffettuazione
							row("ves_dataora_effettuazione") = vacDaModificare.DataOraEffettuazione
							row("ves_ass_codice") = vacDaModificare.AssociazioneCod
							row("ass_descrizione") = vacDaModificare.AssociazioneDescr
							row("ves_ass_n_dose") = vacDaModificare.NrDoseAssociazione
							row("ves_vac_codice") = vacDaModificare.CodiceVaccinazione
							row("vac_descrizione") = vacDaModificare.DescrVaccinazione
							row("ves_n_richiamo") = vacDaModificare.NrDoseVaccinazione
							row("ves_vii_codice") = vacDaModificare.CodiceSomministrazione
							row("ves_sii_codice") = vacDaModificare.CodiceInoculazione
							row("ves_in_campagna") = vacDaModificare.InCampagna
							row("ves_mal_codice_malattia") = vacDaModificare.CodiceMalattia
							row("ves_codice_esenzione") = vacDaModificare.CodiceEsenzione
							row("ves_noc_codice") = vacDaModificare.CodiceNomeCommerciale
							row("ves_luogo") = vacDaModificare.Luogo
							row("ves_cns_codice") = vacDaModificare.CodiceConsultorio
							row("ves_ope_codice") = vacDaModificare.CodiceMedico

							row("ves_lot_codice") = vacDaModificare.CodiceLotto
							row("ves_lot_data_scadenza") = vacDaModificare.DataScadenzaLotto
							If vacDaModificare.IsMedicoInAmbulatorio.HasValue Then
								row("ves_ope_in_ambulatorio") = IIf(vacDaModificare.IsMedicoInAmbulatorio, "S", "N")
							Else
								row("ves_ope_in_ambulatorio") = Nothing
							End If
							row("ves_note") = vacDaModificare.Note
							'row("scaduta") = IIf(vacDaModificare.Scaduta, "S", "N")    lo lascio come era!
							'row("ves_stato") = vacDaModificare.Stato                   lo lascio come era!
							row("ves_id_acn") = vacDaModificare.IdACN
							'row("ves_provenienza") = [Enum].GetName(GetType(Enumerators.ProvenienzaVaccinazioni), vacDaModificare.Provenienza.Value).ToUpper()     lo lascio come era!
							row("ves_rsc_codice") = vacDaModificare.CodiceCondizioneRischio
							row("ves_mal_codice_cond_sanitaria") = vacDaModificare.CodiceMalCondizioneSanitaria
							row("ves_tpa_guid_tipi_pagamento") = vacDaModificare.TipoPagamento.ToByteArray()
							row("ves_med_vaccinante") = vacDaModificare.CodiceVaccinatore
							row("ves_usl_inserimento") = vacDaModificare.Ulss
							row("ves_tipo_associazione_acn") = vacDaModificare.TipoAssociazione

						End If
					End If
				Next
			Next
		End If

		'-------------------- SETTO NEL DATATABLE LE VACCINAZIONI DA ELIMINARE: ------------------------
		If Not command.ListVacDaEliminare Is Nothing AndAlso command.ListVacDaEliminare.Count > 0 Then
			For Each vacDaEliminare As VaccinazioneIntegrazioneDB In command.ListVacDaEliminare

				'            Dim rowKey(4) As Object
				'            rowKey(0) = vacDaEliminare.CodicePaziente
				'            rowKey(1) = vacDaEliminare.CodiceVaccinazione
				'            rowKey(2) = vacDaEliminare.NrDoseVaccinazione
				'            rowKey(3) = vacDaEliminare.DataEffettuazione
				'rowKey(4) = vacDaEliminare.AssociazioneCod
				'Dim rowKey(3) As Object
				'rowKey(0) = vacDaEliminare.CodicePaziente
				'rowKey(1) = vacDaEliminare.CodiceVaccinazione
				'rowKey(2) = vacDaEliminare.NrDoseVaccinazione
				'rowKey(3) = vacDaEliminare.AssociazioneCod
				'Dim row As DataRow = dt_vacEseguite.Rows.Find(rowKey)
				For index As Integer = 0 To dt_vacEseguite.Rows.Count - 1
					Dim row As DataRow = dt_vacEseguite.Rows.Item(index)
					If Not row Is Nothing Then
						If Not row.IsNull("ves_id") AndAlso Convert.ToInt64(row("ves_id")) = vacDaEliminare.IdVaccinazione Then
							row.Delete()
						End If
					End If
				Next



			Next
		End If

		Return dt_vacEseguite

	End Function


	Public Function HasReazioniAvverse(listIdVaccinazioni As List(Of Long)) As Boolean

		Dim result As Boolean = False

		Using bizVaccEseguite As New BizVaccinazioniEseguite(GenericProvider, Settings, ContextInfos, LogOptions)
			For Each idVaccinazione As Long In listIdVaccinazioni
				Dim reazioneAvversa As ReazioneAvversa = Nothing
				reazioneAvversa = GenericProvider.VaccinazioniEseguite.GetReazioneAvversaByVaccinazioneEseguita(idVaccinazione)
				If Not reazioneAvversa Is Nothing Then
					result = True
					Exit For
				End If
			Next
		End Using

		Return result

	End Function

	Public Function Reinvia(idMessaggio As String) As BizGenericResult
		Dim retServizio As New FlussoACN.RicezioneFlussoACNResult
		Dim ret As New BizGenericResult

		Try
			Using clientServizio As New FlussoACN.RicezioneFlussoACNClient
				retServizio = clientServizio.ReinviaAssociazione(idMessaggio)
				ret.Message = retServizio.Message
				ret.Success = retServizio.Success
			End Using
		Catch ex As Exception
			Dim err As String = String.Format("Errore nella chiamata al servizio integrazione Vaccini (ACN): {0}", ex.Message)
			ret.Message = err
			ret.Success = False
			Onit.OnAssistnet.OnVac.Common.Utility.EventLogHelper.EventLogWrite(ex, err, ContextInfos.IDApplicazione)
		End Try


		Return ret

	End Function

#End Region

#Region " Private "

	Private Function CreaNuovoDtVaccinazioniEseguite(codicePaziente As Long) As DataTable

		'creo il dt delle eseguite del paziente, in modo da clonare la struttura nel nuovo dt vuoto
		Dim dt_original As DataTable = Nothing
		Using bizVaccEseguite As New BizVaccinazioniEseguite(GenericProvider, Settings, ContextInfos, LogOptions)
			dt_original = bizVaccEseguite.GetVaccinazioniEseguite(codicePaziente, False)
		End Using

		Dim dt_vacEseguite As New DataTable
		dt_vacEseguite = dt_original.Clone()

		' Primary Key Datatable 
		Dim key(1) As DataColumn
		key(0) = dt_vacEseguite.Columns("ves_vac_codice")
		key(1) = dt_vacEseguite.Columns("ves_n_richiamo")
		dt_vacEseguite.PrimaryKey = key

		Return dt_vacEseguite

	End Function


#End Region

#Region " Batch Importa Vaccinazioni ACN "

	<Serializable>
	Public Class ImportaVaccinazioniACNCommand
		Public IdJob As Long
		Public Elaborazioni As Integer
	End Class

	<Serializable>
	Public Class BatchImportaVaccinazioniACN

		Public Class ParameterName
			Public Const IdApplicazioneLocale As String = "IdApplicazioneLocale"
			Public Const IdUtenteImport As String = "IdUtenteImport"
			Public Const CodiceUslCorrente As String = "CodiceUslCorrente"
		End Class

	End Class

	Public Event RefreshTotaleElementiDaElaborare(e As BizBatch.RefreshTotaleElementiDaElaborareEventArgs)
	Public Event RefreshParzialeElementiElaborati(e As BizBatch.RefreshParzialeElementiElaboratiEventArgs)

	Public Structure RisultatoAcquisizioneElaborazioneACN
		Public Property Successo As Integer
		Public Property Fallite As Integer
	End Structure

    Public Function ImportaVaccinazioniACN(command As ImportaVaccinazioniACNCommand) As RisultatoAcquisizioneElaborazioneACN

        Dim risultatoElab As New RisultatoAcquisizioneElaborazioneACN
        Dim risultatoAcquisizioneElaborazioni As New RisultatoAcquisizioneElaborazione()
        Dim numElabSuccesso As Integer = 0
        Dim numElabFallite As Integer = 0
        ' TODO [Batch Import ACN]: Lettura tabella 
        ' L'import da tracciato a tabella deve essere già stato fatto
        ' Forse può convenire fare 9 tabelle? T_IMPORT_ACN_050501, T_IMPORT_ACN_050502, ...

        ' Qui deve partire la logica di import
        ' 1 Leggo il dato
        Dim elaborazioniVaccinazionePaziente As New List(Of ElaborazioneACN)
        elaborazioniVaccinazionePaziente = GenericProvider.ElaborazioneAcn.GetElaborazioniVaccinazioneACNByIDProcesso(command.IdJob, command.Elaborazioni)

        Dim filterCount As New FiltroElaborazioneACN
        filterCount.IdProcessoAcquisizione = command.IdJob
        filterCount.StatoAcquisizione = StatoAcquisizioneElaborazioneVaccinazioneAcn.DaAcquisire
        Dim countParziale As Integer = 1

        Dim countDaelab As Long = GenericProvider.ElaborazioneAcn.CountElaborazioniVaccinazioneACN(filterCount)
        Dim totaleDaElaborare As Long = countDaelab
        RaiseEvent RefreshTotaleElementiDaElaborare(New Biz.BizBatch.RefreshTotaleElementiDaElaborareEventArgs(totaleDaElaborare))
        ' 2. Controllo esistenza del paziente tramite codice MPI.
        Try
            While Not elaborazioniVaccinazionePaziente Is Nothing AndAlso elaborazioniVaccinazionePaziente.Count > 0 AndAlso countDaelab > 0
                Dim vaccinazionePaziente As ElaborazioneACN = elaborazioniVaccinazionePaziente.FirstOrDefault()
                vaccinazionePaziente.DataAcquisizione = DateTime.Now
                vaccinazionePaziente.MessaggioAcquisizione = Nothing
                Try
                    Dim Paziente As CercaPazienteACNResult = CercaPazienteByMPI(vaccinazionePaziente.CodieMpiPaziente)
                    ' 3. Se esiste ed è unico proseguo, altrimenti stop
                    If Not Paziente.Success Then
                        ' Mi devo remare
                        ' e fare update
                        vaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazioneAcn.CorrispondenzaPazienteMultipla
                        vaccinazionePaziente.MessaggioAcquisizione = "Il paziente non è univoco o non è stato trovato BLOCCANTE."
                    Else
                        ' 4. Mappo i dati del record
                        Dim datoDecodificato As DatoDecodificato = MappingDati(vaccinazionePaziente)
                        datoDecodificato.PazienteVac = Paziente.PazienteACN
                        Dim listVacEsistentiSovrapposte As New List(Of Entities.VaccinazioneIntegrazioneDB)
                        Dim listVacEsistentiByIdACN As New List(Of Entities.VaccinazioneIntegrazioneDB)
                        Dim listVacAssociabili As New List(Of VaccinazioneAssociazione)
                        Dim listVacAssDaInserire As New List(Of VaccinazioneAssociazione)
                        Dim listVacScartate As New List(Of VaccinazioneIntegrazioneDB)
                        Dim listaCodPazDaAllineare As New List(Of Long)
                        If datoDecodificato.EsitoControlli Then
                            ' 5. Effettuo controlli di validità

                            Try
                                listVacEsistentiSovrapposte = GetVaccinazioniSovrapposte(Convert.ToInt64(datoDecodificato.PazienteVac.Paz_Codice), datoDecodificato.CodiceAssociazione, datoDecodificato.DataEffettuazione.Date)
                                If listVacEsistentiSovrapposte.Count > 0 Then
                                    If listVacEsistentiSovrapposte.Any(Function(x) x.Provenienza.HasValue AndAlso x.Provenienza.Value = ProvenienzaVaccinazioni.ACN) Then
                                        listVacEsistentiByIdACN = listVacEsistentiSovrapposte.Where(Function(x) x.Provenienza.HasValue AndAlso x.Provenienza.Value = ProvenienzaVaccinazioni.ACN).ToList()
                                    End If
                                End If
                                listVacAssociabili = GetVaccinazioniAssociazioni(datoDecodificato.listCodiciAss)
                                CreaListeDaInserire(listVacAssDaInserire, listVacScartate, datoDecodificato.ListaVaccinazioni, listVacAssociabili, listVacEsistentiSovrapposte, datoDecodificato.MessaggioControlli)
                                If Not datoDecodificato Is Nothing AndAlso datoDecodificato.EsitoControlli AndAlso listVacScartate.Count <> 0 AndAlso listVacAssDaInserire.Count() = 0 Then
                                    datoDecodificato.EsitoControlli = False
                                    datoDecodificato.MessaggioControlli.Append("Vaccinazione Esistente").AppendLine()
                                    vaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazioneAcn.VaccinazioneEsistente
                                    vaccinazionePaziente.MessaggioAcquisizione = datoDecodificato.MessaggioControlli.ToString()
                                End If
                                If Not datoDecodificato Is Nothing AndAlso datoDecodificato.EsitoControlli AndAlso Not datoDecodificato.ListaVaccinazioni Is Nothing AndAlso (listVacAssDaInserire.Count + listVacScartate.Count) < datoDecodificato.ListaVaccinazioni.Count Then
                                    datoDecodificato.MessaggioControlli.AppendFormat("Non tutte le vaccinazioni ricevute sono state gestite durante la fase 'Verifiche di congruità e creazione liste vaccinazioni da inserire/modificare/eliminare/scartare' (IdACN {0}). Verificare il problema.", datoDecodificato.IdACN).AppendLine()
                                    datoDecodificato.EsitoControlli = False
                                    vaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazioneAcn.VaccinazioneEsistente
                                    vaccinazionePaziente.MessaggioAcquisizione = datoDecodificato.MessaggioControlli.ToString()
                                End If

                            Catch ex As Exception
                                datoDecodificato.EsitoControlli = False
                                Dim msg As String = String.Format("Eccezione non gestita durante la fase 'Verifiche di congruità e creazione liste vaccinazioni da inserire/modificare/eliminare/scartare' (IdACN {0}): {1}", datoDecodificato.IdACN, ex.ToString())
                                datoDecodificato.MessaggioControlli.Append(msg)
                                vaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazioneAcn.Eccezione
                                vaccinazionePaziente.MessaggioAcquisizione = datoDecodificato.MessaggioControlli.ToString()
                                Onit.OnAssistnet.OnVac.Common.Utility.EventLogHelper.EventLogWrite(ex, msg, ContextInfos.IDApplicazione)
                            End Try
                            ' 6. Se è ok effettuo insert
                            If datoDecodificato.EsitoControlli Then
                                Try
#Region "1) creo il Dt da salvare con i record da inserire"
                                    Dim listVacDaInserire As New List(Of VaccinazioneIntegrazioneDB)
                                    listVacDaInserire = SetValoriCampiVacDaInserire(listVacAssDaInserire, datoDecodificato)
                                    Dim commandDt As New DtVaccinazioniEseguiteCommand
                                    commandDt.CodiceLocalePaziente = Convert.ToInt64(datoDecodificato.PazienteVac.Paz_Codice)
                                    commandDt.ListVacDaInserire = listVacDaInserire
                                    Dim dtEseguite As DataTable = CreaDtVaccinazioniEseguite(commandDt)
                                    '' Manca costruisco la lista dei pazienti per cui dovranno essere riallineate le dosi associazioni e vaccinazioni
                                    listaCodPazDaAllineare.Add(Convert.ToInt64(datoDecodificato.PazienteVac.Paz_Codice))
                                    Dim dvModified As New DataView(dtEseguite, String.Empty, String.Empty, DataViewRowState.ModifiedCurrent)
                                    For Each rowModified As DataRowView In dvModified
                                        Dim row As DataRow = rowModified.Row
                                        If row("paz_codice", DataRowVersion.Original).ToString() <> row("paz_codice", DataRowVersion.Current).ToString() Then
                                            listaCodPazDaAllineare.Add(Convert.ToInt64(row("paz_codice", DataRowVersion.Current).ToString()))
                                        End If

                                    Next
#End Region
#Region "2) salvo le vaccinazioni inserite/modificate/eliminate"
                                    Dim commandSalva As New SalvaVaccinazioneEseguitaACNCommand()
                                    commandSalva.CodiceLocalePaziente = Convert.ToInt64(datoDecodificato.PazienteVac.Paz_Codice)
                                    commandSalva.DtVaccinazioniEseguite = dtEseguite
                                    Dim salvaVaccResult As New SalvaVaccinazioneEseguitaACNResult()
                                    salvaVaccResult.Success = False
                                    salvaVaccResult.Messaggio = String.Empty

                                    salvaVaccResult = SalvaVaccinazioneEseguita(commandSalva)

#End Region
#Region "3) allineo i valori delle dosi associazioni e vaccinazioni"
                                    If salvaVaccResult.Success Then
                                        Using biz As New BizVaccinazioniEseguite(GenericProvider, Settings, ContextInfos)
                                            If Not listaCodPazDaAllineare Is Nothing AndAlso listaCodPazDaAllineare.Count() > 0 Then
                                                listaCodPazDaAllineare.Distinct
                                                For Each codPazDaAllineare As Long In listaCodPazDaAllineare
                                                    Dim allineaResult As BizGenericResult = biz.AllineamentoNumeroDosiEAssociazioni(Convert.ToString(codPazDaAllineare))
                                                    If Not allineaResult.Success Then
                                                        Dim msg As String = String.Format("Eccezione non gestita durante l'allineamento delle dosi del paziente {0}: {1}", codPazDaAllineare.ToString(), allineaResult.Message)
                                                        datoDecodificato.EsitoControlli = False
                                                        datoDecodificato.MessaggioControlli.Append(msg).AppendLine()
                                                        vaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazioneAcn.Eccezione
                                                        vaccinazionePaziente.MessaggioAcquisizione = datoDecodificato.MessaggioControlli.ToString()
                                                        Onit.OnAssistnet.OnVac.Common.Utility.EventLogHelper.EventLogWrite(Nothing, msg, ContextInfos.IDApplicazione)
                                                    End If
                                                Next
                                            End If

                                        End Using
                                        If vaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazioneAcn.DaAcquisire Then
                                            vaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazioneAcn.AcquisitaCorrettamente
                                        End If
                                    Else
                                        Dim msg As String = String.Format("Eccezione non gestita durante il salvataggio delle vaccinazioni: {0}", salvaVaccResult.Messaggio)
                                        datoDecodificato.EsitoControlli = False
                                        datoDecodificato.MessaggioControlli.Append(msg).AppendLine()
                                        vaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazioneAcn.Eccezione
                                        vaccinazionePaziente.MessaggioAcquisizione = datoDecodificato.MessaggioControlli.ToString()
                                        Onit.OnAssistnet.OnVac.Common.Utility.EventLogHelper.EventLogWrite(Nothing, msg, ContextInfos.IDApplicazione)
                                    End If
#End Region
                                Catch ex As Exception
                                    Dim msg As String = String.Format("Eccezione non gestita durante il salvataggio delle vaccinazioni: {0}", ex.ToString())
                                    datoDecodificato.EsitoControlli = False
                                    datoDecodificato.MessaggioControlli.Append(msg).AppendLine()
                                    vaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazioneAcn.Eccezione
                                    vaccinazionePaziente.MessaggioAcquisizione = datoDecodificato.MessaggioControlli.ToString()
                                    Onit.OnAssistnet.OnVac.Common.Utility.EventLogHelper.EventLogWrite(Nothing, msg, ContextInfos.IDApplicazione)
                                End Try

                            End If
                        Else
                            vaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazioneAcn.Eccezione
                            vaccinazionePaziente.MessaggioAcquisizione = datoDecodificato.MessaggioControlli.ToString()
                        End If

                    End If

                Catch ex As Exception
                    Dim messaggioProcessoUpdate As New System.Text.StringBuilder
                    AddExceptionMessaggioProcessazione(ex, messaggioProcessoUpdate)
                    vaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazioneAcn.Eccezione
                    vaccinazionePaziente.MessaggioAcquisizione = messaggioProcessoUpdate.ToString()
                Finally
                    If vaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazioneAcn.AcquisitaCorrettamente Then

                        numElabSuccesso = numElabSuccesso + 1
                    Else
                        If vaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazioneAcn.DaAcquisire Then
                            vaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazioneAcn.Eccezione
                        End If
                        numElabFallite = numElabFallite + 1
                    End If
                    If countParziale = 200 Then
                        RaiseEvent RefreshParzialeElementiElaborati(New BizBatch.RefreshParzialeElementiElaboratiEventArgs(numElabSuccesso + numElabFallite, numElabFallite))
                        countParziale = 1
                    Else
                        countParziale = countParziale + 1
                    End If

                End Try
                ' 7. Aggiorno record con esito

                GenericProvider.ElaborazioneAcn.UpdateElaborazioneVaccinazioneAcn(vaccinazionePaziente)
                'countDaelab = GenericProvider.ElaborazioneAcn.CountElaborazioniVaccinazioneACN(filterCount)
                countDaelab = countDaelab - 1
                elaborazioniVaccinazionePaziente = GenericProvider.ElaborazioneAcn.GetElaborazioniVaccinazioneACNByIDProcesso(command.IdJob, command.Elaborazioni)
            End While

        Catch ex As Exception

        Finally
            RaiseEvent RefreshParzialeElementiElaborati(New BizBatch.RefreshParzialeElementiElaboratiEventArgs(numElabSuccesso + numElabFallite, numElabFallite))
        End Try

        risultatoElab.Fallite = numElabFallite
        risultatoElab.Successo = numElabSuccesso

        Return risultatoElab

    End Function

    Private Function SetValoriCampiVacDaInserire(listVacAssDaInserire As List(Of VaccinazioneAssociazione), datiDecodificati As DatoDecodificato) As List(Of VaccinazioneIntegrazioneDB)

        Dim listVacDaInserire As New List(Of VaccinazioneIntegrazioneDB)()

        If Not listVacAssDaInserire Is Nothing AndAlso listVacAssDaInserire.Count() > 0 Then

            For Each vacAss As VaccinazioneAssociazione In listVacAssDaInserire

                Dim vac As New VaccinazioneIntegrazioneDB()

                vac.CodicePaziente = Convert.ToInt64(datiDecodificati.PazienteVac.Paz_Codice)
                vac.DataEffettuazione = datiDecodificati.DataEffettuazione.Date
                vac.DataOraEffettuazione = datiDecodificati.DataEffettuazione
                vac.AssociazioneCod = vacAss.CodiceAssociazione
                vac.AssociazioneDescr = GenericProvider.Associazioni.GetDescrizioneAssociazione(vacAss.CodiceAssociazione)
                vac.NrDoseAssociazione = 1
                vac.CodiceVaccinazione = vacAss.CodiceVaccinazione

                Using bizAnaVac As New BizAnaVaccinazioni(GenericProvider, Settings, ContextInfos)
                    Dim vacInfo As VaccinazioneInfo = bizAnaVac.GetVaccinazioneInfo(vacAss.CodiceVaccinazione)
                    If Not vacInfo Is Nothing Then
                        vac.DescrVaccinazione = vacInfo.DescrizioneVaccinazione
                    End If
                End Using

                vac.NrDoseVaccinazione = 1
                vac.CodiceSomministrazione = datiDecodificati.CodiceViaSomministrazione
                vac.CodiceInoculazione = datiDecodificati.CodiceSitoInoculazione
                vac.InCampagna = datiDecodificati.IsCampagnaVaccinale
                vac.CodiceNomeCommerciale = datiDecodificati.CodiceNomeCommerciale
                vac.CodiceMedico = datiDecodificati.CodiceVaccinatore 'If(String.IsNullOrWhiteSpace(datiDecodificati.CodiceVaccinatore), datiDecodificati.CodiceMedico, datiDecodificati.CodiceVaccinatore)
                vac.CodiceVaccinatore = datiDecodificati.CodiceVaccinatore
                vac.CodiceLotto = datiDecodificati.NrLotto
                vac.DataScadenzaLotto = datiDecodificati.DataScadezaLotto
                vac.IsMedicoInAmbulatorio = datiDecodificati.PresenzaMedico
                vac.Note = datiDecodificati.Note
                vac.Scaduta = False
                vac.Stato = Constants.StatoVaccinazioneEseguita.Registrata
                vac.IdACN = datiDecodificati.IdACN
                vac.Provenienza = ProvenienzaVaccinazioni.ACN
                vac.CodiceCondizioneRischio = datiDecodificati.CondizioneDiRischio
                vac.CodiceMalCondizioneSanitaria = datiDecodificati.MalCondizioneSanitaria
                vac.TipoPagamento = datiDecodificati.TipoPagamento
                vac.CodiceMalattia = datiDecodificati.MalCodiceMalattia
                vac.CodiceEsenzione = datiDecodificati.CodiceEsenzione
                vac.Ulss = datiDecodificati.Ulss
                vac.TipoAssociazione = datiDecodificati.TipoAssociazione
                listVacDaInserire.Add(vac)

            Next

        End If

        Return listVacDaInserire

    End Function

    Private Sub CreaListeDaInserire(ByRef listVacAssDaInserire As List(Of VaccinazioneAssociazione), ByRef listVacScartate As List(Of VaccinazioneIntegrazioneDB), listVacAssRicevute As List(Of VaccinazioneAssociazione), listVacAssociabili As List(Of VaccinazioneAssociazione), listVacEsistentiSovrapposte As List(Of VaccinazioneIntegrazioneDB), ByRef msgControlli As StringBuilder)

        If Not listVacAssRicevute Is Nothing AndAlso listVacAssRicevute.Count > 0 Then

            Dim scartoTutto As Boolean = False

            '1) controllo che le vaccinazioni da inserire corrispondano come numero a quelle previste per l'associazione in questione, altrimenti scarto tutto
            If listVacAssRicevute.Count <> listVacAssociabili.Count Then
                msgControlli.Append("Le vaccinazioni sono state scartate in quanto non corrispondono con quelle associabili in base all'anagrafica delle vaccinazioni.").AppendLine()
                scartoTutto = True
            End If
            If Not scartoTutto Then
                For Each vaccRicevuta As VaccinazioneAssociazione In listVacAssRicevute
                    If Not listVacAssociabili.Any(Function(p) p.CodiceAssociazione = vaccRicevuta.CodiceAssociazione AndAlso p.CodiceVaccinazione = vaccRicevuta.CodiceVaccinazione) Then
                        scartoTutto = True
                        msgControlli.Append("Le vaccinazioni sono state scartate in quanto non corrispondono con quelle associabili in base all'anagrafica delle vaccinazioni.").AppendLine()
                        Exit For
                    End If

                Next
            End If
            If Not scartoTutto AndAlso Not listVacEsistentiSovrapposte Is Nothing AndAlso listVacEsistentiSovrapposte.Count > 0 Then
                scartoTutto = True
                msgControlli.Append("Le vaccinazioni sono state scartate in quanto già presenti in OnVac tra le eseguite con stesso codAssociazione e stessa data.").AppendLine()
            End If
            If (scartoTutto) Then
                ScartaVaccinazioni(listVacScartate, listVacAssRicevute, String.Empty)
            Else
                listVacAssDaInserire = listVacAssRicevute
            End If
        End If
    End Sub
	Private Sub ScartaVaccinazioni(ByRef listVacScartate As List(Of VaccinazioneIntegrazioneDB), listVacAssDaScartare As List(Of VaccinazioneAssociazione), idAcn As String)
		If Not listVacAssDaScartare Is Nothing AndAlso listVacAssDaScartare.Count() > 0 Then
			For Each vaccAss As VaccinazioneAssociazione In listVacAssDaScartare
				If (Not listVacScartate Is Nothing AndAlso listVacScartate.Count() > 0 AndAlso Not listVacScartate.Any(Function(p) p.AssociazioneCod = vaccAss.CodiceAssociazione AndAlso p.CodiceVaccinazione = vaccAss.CodiceVaccinazione)) OrElse (listVacScartate Is Nothing OrElse listVacScartate.Count = 0) Then
					Dim aa As New VaccinazioneIntegrazioneDB
					aa.CodiceVaccinazione = vaccAss.CodiceVaccinazione
					aa.AssociazioneCod = vaccAss.CodiceAssociazione
					aa.IdACN = idAcn
					listVacScartate.Add(aa)
				End If
			Next
		End If
	End Sub
	Private Function MappingDati(dati As ElaborazioneACN) As DatoDecodificato
		Dim datoCod As New DatoDecodificato
		Dim esitoControlli As Boolean = True
		Dim messaggioControlli As New StringBuilder
		Try
			If String.IsNullOrWhiteSpace(dati.IdVaccinoAcn) Then
				messaggioControlli.AppendLine("Il valore di 'IdVaccinoAcn' è null")
				esitoControlli = False
			Else
				datoCod.IdACN = dati.IdVaccinoAcn
			End If
			If Not dati.DataOraAttivita.HasValue Then
				messaggioControlli.AppendLine("Il valore di 'DataOraAttivita' è null")
				esitoControlli = False
			Else
				datoCod.DataEffettuazione = dati.DataOraAttivita.Value
			End If
			datoCod.IsCampagnaVaccinale = String.Empty
			If Not String.IsNullOrWhiteSpace(dati.CodCampagnaVaccinale) Then
				If dati.CodCampagnaVaccinale = "C" Then
					datoCod.IsCampagnaVaccinale = "S"
				Else
					datoCod.IsCampagnaVaccinale = "N"
				End If

			End If

			datoCod.NrLotto = dati.Lotto
			If dati.DataScadenzaLotto.HasValue Then
				datoCod.DataScadezaLotto = dati.DataScadenzaLotto.Value
			End If
			If Not String.IsNullOrWhiteSpace(dati.TipoAttivita) OrElse Not String.IsNullOrWhiteSpace(dati.CodiceDiagnosi) Then
				Dim tipoAttivita As String = String.Empty
				If Not String.IsNullOrWhiteSpace(dati.TipoAttivita) Then
					tipoAttivita = String.Format("Tipo attivita: {0}", dati.TipoAttivita)
				End If
				Dim codiceDiagnosi As String = String.Empty
				If Not String.IsNullOrWhiteSpace(dati.CodiceDiagnosi) Then
					codiceDiagnosi = String.Format("Codice diagnosi: {0}", dati.CodiceDiagnosi)
				End If

				datoCod.Note = String.Format("{0} {1}", tipoAttivita, codiceDiagnosi)
			End If
			' Condizioni di rischio
			If Not String.IsNullOrWhiteSpace(dati.CodiceCategoriaRischio) AndAlso dati.CodiceCategoriaRischio.Length > 2 Then
				If dati.CodiceCategoriaRischio.Contains("CR") Then
					datoCod.CondizioneDiRischio = GetCondizioneDiRischioACN(dati.CodiceCategoriaRischio)
				End If
				If dati.CodiceCategoriaRischio.Contains("CS") Then
					Dim condSanL As New List(Of String)
					condSanL.Add(dati.CodiceCategoriaRischio)
					Dim malCodndL As List(Of Malattia) = GetMalattieByCodiciACN(condSanL)
					If malCodndL.Count > 0 Then
						datoCod.MalCondizioneSanitaria = malCodndL.FirstOrDefault().CodiceMalattia
					End If


				End If

			End If
			datoCod.TipoPagamento = New Guid(GetGuidTipoPagamentoByCodiceACN(dati.CodiceModalitaPagamento))

			Dim associazione As AssociazioneACN = GetAssociazioneByCodiceACN(dati.CodicePrincipioAttivo, dati.CodiceAic)
			If Not associazione Is Nothing AndAlso Not String.IsNullOrWhiteSpace(associazione.Codice) Then
				Dim listCodiciAss As New List(Of String)
				listCodiciAss.Add(associazione.Codice)
				Dim primaAssociazione As String = listCodiciAss.First()
				datoCod.listCodiciAss = listCodiciAss
				datoCod.CodiceAssociazione = associazione.Codice
				datoCod.TipoAssociazione = associazione.TipoAssociazione
				Dim listaCodiciEsterniVacc As New List(Of VaccinazioneAssociazione)
				listaCodiciEsterniVacc = GetVaccinazioniAssociazioni(listCodiciAss)
				Dim listaCodiceVac As New List(Of String)
				For Each vac As VaccinazioneAssociazione In listaCodiciEsterniVacc.Where(Function(p) p.CodiceAssociazione = primaAssociazione)
					If Not String.IsNullOrWhiteSpace(vac.CodiceAssociazione) Then
						listaCodiceVac.Add(vac.CodiceVaccinazione)
					End If
				Next
				If Not listaCodiciEsterniVacc Is Nothing AndAlso listaCodiciEsterniVacc.Count > 0 Then
					datoCod.ListaVaccinazioni = listaCodiciEsterniVacc
				End If
				If Not datoCod.ListaVaccinazioni Is Nothing AndAlso datoCod.ListaVaccinazioni.Count = 0 Then
					Dim listaVacStr As String = String.Empty
					If listaCodiceVac.Count > 0 Then
						listaVacStr = String.Join("|", listaCodiceVac.ToArray())
					End If
					messaggioControlli.AppendFormat("Lista di vaccinazioni non trovata con i codici esterni '{0}'", listaVacStr).AppendLine()
					esitoControlli = False
				End If
			Else
				messaggioControlli.AppendFormat("Associazione non trovata con il codice esterno '{0}' e codice Aic {1}", dati.CodicePrincipioAttivo.ToString(), dati.CodiceAic.ToString()).AppendLine()
				esitoControlli = False
			End If
			'' Malatie da verificre
			If Not String.IsNullOrWhiteSpace(dati.CodiceDiagnosi) Then
				Dim listaMal As New List(Of String)
				listaMal.Add(dati.CodiceDiagnosi)
				Dim listMal As List(Of Malattia) = GetMalattieByCodiciACN(listaMal)
				If Not listMal Is Nothing AndAlso listMal.Count() > 0 Then
					datoCod.MalCodiceMalattia = listMal.FirstOrDefault().CodiceMalattia
				End If

			End If
			If Not String.IsNullOrWhiteSpace(dati.CodiceSitoInoculazione) Then
				datoCod.CodiceSitoInoculazione = GetCodiceSitoInoculazioneByCodiceACN(dati.CodiceSitoInoculazione)
			Else
				datoCod.CodiceSitoInoculazione = String.Empty
			End If
			If Not String.IsNullOrWhiteSpace(dati.CodiceViaSomministrazione) Then
				datoCod.CodiceViaSomministrazione = GetCodiceViaSomministrazioneByCodiceACN(dati.CodiceViaSomministrazione)
			Else
				datoCod.CodiceViaSomministrazione = String.Empty
			End If

			' esenzioni

			If Not String.IsNullOrWhiteSpace(dati.CodiceDiagnosi) Then
				Dim listaCodiciEsterniEsenzioni As New List(Of String)
				listaCodiciEsterniEsenzioni.Add(dati.CodiceDiagnosi)
				datoCod.ListaMalattie = GetMalattieByCodiciACN(listaCodiciEsterniEsenzioni)
				If Not datoCod.ListaMalattie Is Nothing AndAlso datoCod.ListaMalattie.Count > 0 Then
					datoCod.CodiceEsenzione = datoCod.ListaMalattie.FirstOrDefault().CodiceMalattia.ToString()
				End If
			End If
			datoCod.CodiceNomeCommerciale = GetCodiceNomeCommercialeByCodiceAic(dati.CodiceAic)

			datoCod.Ulss = dati.CodiceUlss
			If Not String.IsNullOrWhiteSpace(dati.CfMedico) Then
				Dim operatore As Operatore = CercaOperatoreByCodiceFiscale(dati.CfMedico)
				If Not operatore Is Nothing Then
					datoCod.CodiceVaccinatore = operatore.Codice
				Else
					'' Devo inserire l'operatore ?
					datoCod.CodiceVaccinatore = String.Empty
					messaggioControlli.AppendFormat("Operatore non trovato per CF {0}", dati.CfMedico).AppendLine()

				End If
				Dim medico As Medico = GenericProvider.Medico.GetMedicoByCodiceFiscale(dati.CfMedico)
				If Not medico Is Nothing Then
					datoCod.CodiceMedico = medico.Codice
				Else
					datoCod.CodiceMedico = String.Empty
					messaggioControlli.AppendFormat("Medico non trovato per CF {0}", dati.CfMedico).AppendLine()
					'esitoControlli = False
				End If
			Else
				messaggioControlli.AppendLine("Codice fiscale Medico non valorizato")
				datoCod.CodiceVaccinatore = String.Empty
				'esitoControlli = False
			End If




		Catch ex As Exception
			Dim msg As String = String.Format("Eccezione non gestita durante la fase di 'Mapping dei campi': {0}", ex.Message)
			esitoControlli = False
			messaggioControlli.AppendLine(msg)
		End Try
		datoCod.EsitoControlli = esitoControlli
		datoCod.MessaggioControlli = messaggioControlli
		Return datoCod

	End Function
	Public Structure RisultatoAcquisizioneElaborazione
		Public Property Successo As IEnumerable(Of ElaborazioneVaccinazionePaziente)
		Public Property Fallite As IEnumerable(Of ElaborazioneVaccinazionePaziente)
	End Structure
#Region " Message "

	Private Sub AddExceptionMessaggioProcessazione(exc As Exception, messaggioProcessazione As StringBuilder)

		Dim messaggioException As New StringBuilder()

		Dim excTemp As Exception = exc

		While Not excTemp Is Nothing

			If messaggioException.Length > 0 Then messaggioException.AppendLine()

			messaggioException.AppendLine(excTemp.Message)
			messaggioException.AppendFormat("[{0}]", excTemp.StackTrace)

			excTemp = excTemp.InnerException

		End While

		Me.AddMessaggiooProcessazione("EXCEPTION", messaggioException.ToString(), messaggioProcessazione)

	End Sub
	Private Sub AddMessaggiooProcessazione(title As String, content As String, messaggioProcessazione As StringBuilder)

		If messaggioProcessazione.Length > 0 Then
			messaggioProcessazione.AppendLine()
		End If

		messaggioProcessazione.AppendFormat("{0}:", title)
		messaggioProcessazione.AppendLine()
		messaggioProcessazione.AppendLine(content)

	End Sub
#End Region


#End Region

End Class
