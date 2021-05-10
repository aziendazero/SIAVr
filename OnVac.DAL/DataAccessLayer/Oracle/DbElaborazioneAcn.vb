Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.Data.OracleClient
Imports System.Text


Namespace DAL.Oracle

	Public Class DbElaborazioneACNProvider
		Inherits DbProvider
		Implements IElaborazioneACNProvider

#Region " Costruttore "

		Public Sub New(ByRef DAM As IDAM)

			MyBase.New(DAM)

		End Sub

#End Region
#Region " Public "

		Public Function GetElaborazioniVaccinazioneACNByIDProcesso(idProcessoElaborazione As Long, numElaborazioni As Integer) As List(Of Entities.ElaborazioneACN) Implements IElaborazioneACNProvider.GetElaborazioniVaccinazioneACNByIDProcesso

			Dim filter As New FiltroElaborazioneACN()
			filter.IdProcessoAcquisizione = idProcessoElaborazione
			filter.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazioneAcn.DaAcquisire


			Return Me.GetElaborazioniVaccinazionePaziente(filter, Nothing, String.Empty, numElaborazioni)

		End Function

		Public Function GetElaborazioniVaccinazioneACN(filter As FiltroElaborazioneACN, pagingOptions As Onit.OnAssistnet.Data.PagingOptions) As List(Of Entities.ElaborazioneACN) Implements IElaborazioneACNProvider.GetElaborazioniVaccinazioneACN

			Return Me.GetElaborazioniVaccinazionePaziente(filter, pagingOptions, "PEA_PAZ_CODICE_FISCALE, PEA_PAZ_COGNOME, PEA_PAZ_NOME, PEA_ATTIVITA_DATA, PEA_PRINCIPIO_ATTIVO_COD, PEA_AIC_CODICE", 0)

		End Function

		Public Function CountElaborazioniVaccinazioneACN(filter As FiltroElaborazioneACN) As Long Implements IElaborazioneACNProvider.CountElaborazioniVaccinazioneACN

			Dim cmd As OracleClient.OracleCommand = Nothing
			Dim ownConnection As Boolean = False

			Try
				cmd = New OracleClient.OracleCommand()

				cmd.Connection = Connection

				Dim commandText As New System.Text.StringBuilder()

				AddFilterWhereConditions(filter, commandText, cmd.Parameters)

				cmd.CommandText = String.Format("SELECT COUNT(*) FROM T_PAZ_ELAB_VACCINAZIONI_ACN{0}{1}", IIf(commandText.Length = 0, String.Empty, " WHERE "), commandText.ToString())

				ownConnection = ConditionalOpenConnection(cmd)

				Return Convert.ToInt64(cmd.ExecuteScalar())

			Finally

				ConditionalCloseConnection(ownConnection)

				If Not cmd Is Nothing Then cmd.Dispose()

			End Try

		End Function

		Public Sub UpdateElaborazioneVaccinazioneAcn(ByVal elaborazioneVaccinazionePaziente As Entities.ElaborazioneACN) Implements IElaborazioneACNProvider.UpdateElaborazioneVaccinazioneAcn

			Dim dataElaborazioneTemp As Object = Nothing
			Dim codicePazienteAcquisizioneTemp As Object = Nothing

			If elaborazioneVaccinazionePaziente.DataAcquisizione.HasValue Then
				dataElaborazioneTemp = elaborazioneVaccinazionePaziente.DataAcquisizione.Value
			Else
				dataElaborazioneTemp = DBNull.Value
			End If

			If elaborazioneVaccinazionePaziente.CodicePazienteAcquisizione.HasValue Then
				codicePazienteAcquisizioneTemp = elaborazioneVaccinazionePaziente.CodicePazienteAcquisizione.Value
			Else
				codicePazienteAcquisizioneTemp = DBNull.Value
			End If

			Dim cmd As OracleClient.OracleCommand = Nothing
			Dim ownConnection As Boolean = False

			Try
				cmd = New OracleClient.OracleCommand("UPDATE T_PAZ_ELAB_VACCINAZIONI_ACN SET PEA_ACQUISIZIONE_STATO = :PEA_ACQUISIZIONE_STATO, PEA_ACQUISIZIONE_MESSAGGIO = :PEA_ACQUISIZIONE_MESSAGGIO, PEA_ACQUISIZIONE_DATA = :PEA_ACQUISIZIONE_DATA, PEA_ACQUISIZIONE_PAZ_CODICE = :PEA_ACQUISIZIONE_PAZ_CODICE WHERE PEA_ID = :PEA_ID", Me.Connection)

				cmd.Parameters.AddWithValue("PEA_ACQUISIZIONE_STATO", Convert.ToInt32(elaborazioneVaccinazionePaziente.StatoAcquisizione))
				cmd.Parameters.AddWithValue("PEA_ACQUISIZIONE_MESSAGGIO", IIf(String.IsNullOrEmpty(elaborazioneVaccinazionePaziente.MessaggioAcquisizione), DBNull.Value, elaborazioneVaccinazionePaziente.MessaggioAcquisizione))
				cmd.Parameters.AddWithValue("PEA_ACQUISIZIONE_DATA", dataElaborazioneTemp)
				cmd.Parameters.AddWithValue("PEA_ACQUISIZIONE_PAZ_CODICE", codicePazienteAcquisizioneTemp)
				cmd.Parameters.AddWithValue("PEA_ID", elaborazioneVaccinazionePaziente.Id)

				ownConnection = ConditionalOpenConnection(cmd)

				cmd.ExecuteNonQuery()

			Finally

				ConditionalCloseConnection(ownConnection)

				If Not cmd Is Nothing Then cmd.Dispose()

			End Try

		End Sub

		Public Function UpdateElaborazioniVaccinazioneAcn(ByVal idProcessoElaborazione As Long, ByVal statoElaborazione As Enumerators.StatoAcquisizioneElaborazioneVaccinazioneAcn, ByVal statoElaborazionePrecedente As Enumerators.StatoAcquisizioneElaborazioneVaccinazioneAcn) As Int64 Implements IElaborazioneACNProvider.UpdateElaborazioniVaccinazioneAcn

			Dim cmd As OracleClient.OracleCommand = Nothing
			Dim ownConnection As Boolean = False

			Try
				cmd = New OracleClient.OracleCommand("UPDATE T_PAZ_ELAB_VACCINAZIONI SET  PEA_ACQUISIZIONE_PRC_ID = :PEA_ACQUISIZIONE_PRC_ID,  PEV_STATO_ACQUISIZIONE = :PEV_STATO_ACQUISIZIONE WHERE PEA_ACQUISIZIONE_STATO = :PEA_STATO_ELAB_PREC", Me.Connection)

				cmd.Parameters.AddWithValue("PEA_ACQUISIZIONE_PRC_ID", idProcessoElaborazione)
				cmd.Parameters.AddWithValue("PEA_ACQUISIZIONE_STATO", Convert.ToInt32(statoElaborazione))
				cmd.Parameters.AddWithValue("PEA_STATO_ELAB_PREC", Convert.ToInt32(statoElaborazionePrecedente))

				ownConnection = Me.ConditionalOpenConnection(cmd)

				Return cmd.ExecuteNonQuery()

			Finally

				ConditionalCloseConnection(ownConnection)

				If Not cmd Is Nothing Then cmd.Dispose()

			End Try

		End Function

#End Region

#Region " Private "

		Private Function GetElaborazioniVaccinazionePaziente(filter As FiltroElaborazioneACN, pagingOptions As Onit.OnAssistnet.Data.PagingOptions, orderByConditions As String,numeroElaborazioni As Integer) As List(Of Entities.ElaborazioneACN)

			Dim elaborazioniVaccinazioneACN As New List(Of ElaborazioneACN)

			Dim cmd As OracleClient.OracleCommand = Nothing

			Dim ownConnection As Boolean = False

			Try
				cmd = New OracleClient.OracleCommand()
				cmd.Connection = Connection

				Dim commandText As New System.Text.StringBuilder()

				AddFilterWhereConditions(filter, commandText, cmd.Parameters)
				Dim rownum As String = If(numeroElaborazioni = 0, String.Empty, String.Format(" and rownum <= {0}", numeroElaborazioni.ToString()))
				cmd.CommandText = String.Format("SELECT * FROM T_PAZ_ELAB_VACCINAZIONI_ACN where 1=1 {0}{1}{2}{3}{4}", IIf(commandText.Length = 0, String.Empty, " and "), commandText.ToString(), rownum, IIf(String.IsNullOrEmpty(orderByConditions), String.Empty, " ORDER BY "), orderByConditions)

				If Not pagingOptions Is Nothing Then
					cmd.AddPaginatedQuery(pagingOptions)
				End If

				ownConnection = Me.ConditionalOpenConnection(cmd)

				Using dataReader As IDataReader = cmd.ExecuteReader()

					Dim pea_id As Int16 = dataReader.GetOrdinal("PEA_ID")
					Dim pea_id_acn As Int16 = dataReader.GetOrdinal("PEA_ID_ACN")
					Dim pea_data_inizio As Int16 = dataReader.GetOrdinal("PEA_DATA_INIZIO")
					Dim pea_data_fine As Int16 = dataReader.GetOrdinal("PEA_DATA_FINE")

					Dim pea_inviante_cf As Int16 = dataReader.GetOrdinal("PEA_INVIANTE_CF")
					Dim pea_inviante_nome As Int16 = dataReader.GetOrdinal("PEA_INVIANTE_NOME")
					Dim pea_inviante_cognome As Int16 = dataReader.GetOrdinal("PEA_INVIANTE_COGNOME")

					Dim pea_data_ultima_oper As Int16 = dataReader.GetOrdinal("PEA_DATA_ULTIMA_OPER")

					Dim pea_medico_cf As Int16 = dataReader.GetOrdinal("PEA_MEDICO_CF")
					Dim pea_medico_nome As Int16 = dataReader.GetOrdinal("PEA_MEDICO_NOME")
					Dim pea_medico_cognome As Int16 = dataReader.GetOrdinal("PEA_MEDICO_COGNOME")
					Dim pea_medico_codice_reg As Int16 = dataReader.GetOrdinal("PEA_MEDICO_CODICE_REG")
					Dim pea_medico_tipo As Int16 = dataReader.GetOrdinal("PEA_MEDICO_TIPO")

					Dim pea_mgi_codice As Int16 = dataReader.GetOrdinal("PEA_MGI_CODICE")
					Dim pea_codice_ulss As Int16 = dataReader.GetOrdinal("PEA_CODICE_ULSS")

					Dim pea_attivita_tipo As Int16 = dataReader.GetOrdinal("PEA_ATTIVITA_TIPO")
					Dim pea_attivita_data As Int16 = dataReader.GetOrdinal("PEA_ATTIVITA_DATA")
					Dim pea_attivita_cf_autore As Int16 = dataReader.GetOrdinal("PEA_ATTIVITA_CF_AUTORE")
					Dim pea_attivita_nome_autore As Int16 = dataReader.GetOrdinal("PEA_ATTIVITA_NOME_AUTORE")
					Dim pea_attivita_cognome_autore As Int16 = dataReader.GetOrdinal("PEA_ATTIVITA_COGNOME_AUTORE")

					Dim pea_principio_attivo_cod As Int16 = dataReader.GetOrdinal("PEA_PRINCIPIO_ATTIVO_COD")
					Dim pea_aic_codice As Int16 = dataReader.GetOrdinal("PEA_AIC_CODICE")
					Dim pea_aic_descrizione As Int16 = dataReader.GetOrdinal("PEA_AIC_DESCRIZIONE")

					Dim pea_sito_inoculazione_codice As Int16 = dataReader.GetOrdinal("PEA_SITO_INOCULO_CODICE")
					Dim pea_sito_inoculazione_desc As Int16 = dataReader.GetOrdinal("PEA_SITO_INOCULO_DESC")

					Dim pea_via_sommin_codice As Int16 = dataReader.GetOrdinal("PEA_VIA_SOMMIN_CODICE")
					Dim pea_via_sommin_desc As Int16 = dataReader.GetOrdinal("PEA_VIA_SOMMIN_DESC")

					Dim pea_lotto As Int16 = dataReader.GetOrdinal("PEA_LOTTO")
					Dim pea_lotto_data_scad As Int16 = dataReader.GetOrdinal("PEA_LOTTO_DATA_SCAD")

					Dim pea_cat_rischio_codice As Int16 = dataReader.GetOrdinal("PEA_CAT_RISCHIO_CODICE")
					Dim peacat_rischio_descrizione As Int16 = dataReader.GetOrdinal("PEA_CAT_RISCHIO_DESCRIZIONE")

					Dim pea_mod_pagamento_codice As Int16 = dataReader.GetOrdinal("PEA_MOD_PAGAMENTO_CODICE")
					Dim pea_mod_pagamento_descrizione As Int16 = dataReader.GetOrdinal("PEA_MOD_PAGAMENTO_DESCRIZIONE")

					Dim pea_campagna_vac_codice As Int16 = dataReader.GetOrdinal("PEA_CAMPAGNA_VAC_CODICE")
					Dim pea_campagna_vac_descrizione As Int16 = dataReader.GetOrdinal("PEA_CAMPAGNA_VAC_DESCRIZIONE")

					Dim pea_diagnosi_codice As Int16 = dataReader.GetOrdinal("PEA_DIAGNOSI_CODICE")
					Dim pea_diagnosi_descrizione As Int16 = dataReader.GetOrdinal("PEA_DIAGNOSI_DESCRIZIONE")

					Dim pea_paz_codice_regionale As Int16 = dataReader.GetOrdinal("PEA_PAZ_CODICE_REGIONALE")
					Dim pea_paz_codice_fiscale As Int16 = dataReader.GetOrdinal("PEA_PAZ_CODICE_FISCALE")
					Dim pea_paz_cognome As Int16 = dataReader.GetOrdinal("PEA_PAZ_COGNOME")
					Dim pea_paz_nome As Int16 = dataReader.GetOrdinal("PEA_PAZ_NOME")

					Dim pea_acquisizione_prc_id As Int16 = dataReader.GetOrdinal("PEA_ACQUISIZIONE_PRC_ID")
					Dim pea_acquisizione_stato As Int16 = dataReader.GetOrdinal("PEA_ACQUISIZIONE_STATO")
					Dim pea_acquisizione_data As Int16 = dataReader.GetOrdinal("PEA_ACQUISIZIONE_DATA")
					Dim pea_acquisizione_messaggio As Int16 = dataReader.GetOrdinal("PEA_ACQUISIZIONE_MESSAGGIO")
					Dim pea_acquisizione_paz_codice As Int16 = dataReader.GetOrdinal("PEA_ACQUISIZIONE_PAZ_CODICE")


					While dataReader.Read()

						Dim elaborazioneAcn As New ElaborazioneACN()

						elaborazioneAcn.Id = dataReader.GetInt64(pea_id)
						If Not dataReader.IsDBNull(pea_id_acn) Then elaborazioneAcn.IdVaccinoAcn = dataReader.GetString(pea_id_acn)
						If Not dataReader.IsDBNull(pea_data_inizio) Then elaborazioneAcn.DataInizio = dataReader.GetDateTime(pea_data_inizio)
						If Not dataReader.IsDBNull(pea_data_fine) Then elaborazioneAcn.DataFine = dataReader.GetDateTime(pea_data_fine)

						If Not dataReader.IsDBNull(pea_inviante_cf) Then elaborazioneAcn.CfInviante = dataReader.GetString(pea_inviante_cf)
						If Not dataReader.IsDBNull(pea_medico_cf) Then elaborazioneAcn.CfMedico = dataReader.GetString(pea_medico_cf)
						If Not dataReader.IsDBNull(pea_medico_nome) Then elaborazioneAcn.NomeMedico = dataReader.GetString(pea_medico_nome)
						If Not dataReader.IsDBNull(pea_medico_cognome) Then elaborazioneAcn.CognomeMedico = dataReader.GetString(pea_medico_cognome)
						If Not dataReader.IsDBNull(pea_medico_codice_reg) Then elaborazioneAcn.CodiceRegionaleMedico = dataReader.GetString(pea_medico_codice_reg)
						If Not dataReader.IsDBNull(pea_medico_tipo) Then elaborazioneAcn.TipoMedico = dataReader.GetString(pea_medico_tipo)
						If Not dataReader.IsDBNull(pea_codice_ulss) Then elaborazioneAcn.CodiceUlss = dataReader.GetString(pea_codice_ulss)

						If Not dataReader.IsDBNull(pea_attivita_tipo) Then elaborazioneAcn.TipoAttivita = dataReader.GetString(pea_attivita_tipo)
						If Not dataReader.IsDBNull(pea_attivita_data) Then elaborazioneAcn.DataOraAttivita = dataReader.GetDateTime(pea_attivita_data)
						If Not dataReader.IsDBNull(pea_attivita_cf_autore) Then elaborazioneAcn.CfAttivita = dataReader.GetString(pea_attivita_cf_autore)

						If Not dataReader.IsDBNull(pea_principio_attivo_cod) Then elaborazioneAcn.CodicePrincipioAttivo = dataReader.GetString(pea_principio_attivo_cod)
						If Not dataReader.IsDBNull(pea_aic_codice) Then elaborazioneAcn.CodiceAic = dataReader.GetString(pea_aic_codice)

						If Not dataReader.IsDBNull(pea_sito_inoculazione_codice) Then elaborazioneAcn.CodiceSitoInoculazione = dataReader.GetString(pea_sito_inoculazione_codice)
						If Not dataReader.IsDBNull(pea_via_sommin_codice) Then elaborazioneAcn.CodiceViaSomministrazione = dataReader.GetString(pea_via_sommin_codice)

						If Not dataReader.IsDBNull(pea_lotto) Then elaborazioneAcn.Lotto = dataReader.GetString(pea_lotto)
						If Not dataReader.IsDBNull(pea_lotto_data_scad) Then elaborazioneAcn.DataScadenzaLotto = dataReader.GetDateTime(pea_lotto_data_scad)

						If Not dataReader.IsDBNull(pea_cat_rischio_codice) Then elaborazioneAcn.CodiceCategoriaRischio = dataReader.GetString(pea_cat_rischio_codice)
						If Not dataReader.IsDBNull(pea_mod_pagamento_codice) Then elaborazioneAcn.CodiceModalitaPagamento = dataReader.GetString(pea_mod_pagamento_codice)

						If Not dataReader.IsDBNull(pea_campagna_vac_codice) Then elaborazioneAcn.CodCampagnaVaccinale = dataReader.GetString(pea_campagna_vac_codice)
						If Not dataReader.IsDBNull(pea_diagnosi_codice) Then elaborazioneAcn.CodiceDiagnosi = dataReader.GetString(pea_diagnosi_codice)

						If Not dataReader.IsDBNull(pea_paz_codice_regionale) Then elaborazioneAcn.CodieMpiPaziente = dataReader.GetString(pea_paz_codice_regionale)
						If Not dataReader.IsDBNull(pea_paz_codice_fiscale) Then elaborazioneAcn.CfPaziente = dataReader.GetString(pea_paz_codice_fiscale)
						If Not dataReader.IsDBNull(pea_paz_cognome) Then elaborazioneAcn.CognomePaziente = dataReader.GetString(pea_paz_cognome)
						If Not dataReader.IsDBNull(pea_paz_nome) Then elaborazioneAcn.NomePaziente = dataReader.GetString(pea_paz_nome)

						If Not dataReader.IsDBNull(pea_acquisizione_prc_id) Then elaborazioneAcn.IdPrcAcquisizione = dataReader.GetInt64(pea_acquisizione_prc_id)
						If Not dataReader.IsDBNull(pea_acquisizione_stato) Then elaborazioneAcn.StatoAcquisizione = DirectCast([Enum].Parse(GetType(Enumerators.StatoAcquisizioneElaborazioneVaccinazioneAcn), dataReader.GetInt32(pea_acquisizione_stato).ToString()), Enumerators.StatoAcquisizioneElaborazioneVaccinazioneAcn)
						If Not dataReader.IsDBNull(pea_acquisizione_data) Then elaborazioneAcn.DataAcquisizione = dataReader.GetDateTime(pea_acquisizione_data)
						If Not dataReader.IsDBNull(pea_acquisizione_messaggio) Then elaborazioneAcn.MessaggioAcquisizione = dataReader.GetString(pea_acquisizione_messaggio)
						If Not dataReader.IsDBNull(pea_acquisizione_paz_codice) Then elaborazioneAcn.CodicePazienteAcquisizione = dataReader.GetInt64(pea_acquisizione_paz_codice)

						elaborazioniVaccinazioneACN.Add(elaborazioneAcn)

					End While

				End Using

			Finally

				ConditionalCloseConnection(ownConnection)

				If Not cmd Is Nothing Then cmd.Dispose()

			End Try

			Return elaborazioniVaccinazioneACN

		End Function

		Private Sub AddFilterWhereConditions(filter As FiltroElaborazioneACN, commandText As StringBuilder, parameters As OracleClient.OracleParameterCollection)

			If Not filter Is Nothing Then

				If filter.Id.HasValue Then

					commandText.Append(" PEA_ID = :PEA_ID")

					parameters.AddWithValue("PEA_ID", filter.Id.Value)

				End If

				If filter.IdProcessoAcquisizione.HasValue Then

					If commandText.Length > 0 Then commandText.Append(" AND")

					commandText.Append(" PEA_ACQUISIZIONE_PRC_ID = :PEA_ACQUISIZIONE_PRC_ID")

					parameters.AddWithValue("PEA_ACQUISIZIONE_PRC_ID", filter.IdProcessoAcquisizione.Value)

				End If

				If filter.StatoAcquisizione.HasValue Then

					If commandText.Length > 0 Then commandText.Append(" AND")

					commandText.Append(" PEA_ACQUISIZIONE_STATO = :PEA_ACQUISIZIONE_STATO")

					parameters.AddWithValue("PEA_ACQUISIZIONE_STATO", Convert.ToInt16(filter.StatoAcquisizione.Value))

				End If

				If filter.DataAcquisizione.HasValue Then

					If commandText.Length > 0 Then commandText.Append(" AND")

					commandText.Append(" PEA_ACQUISIZIONE_DATA >= :PEA_DATA_ACQ_DA")

					parameters.AddWithValue("PEA_DATA_ACQ_DA", filter.DataAcquisizione)

				End If

				If filter.DataAcquisizioneA.HasValue Then

					If commandText.Length > 0 Then commandText.Append(" AND")

					commandText.Append(" PEA_ACQUISIZIONE_DATA <= :PEA_DATA_ACQ_A")

					parameters.AddWithValue("PEA_DATA_ACQ_A", filter.DataAcquisizioneA)

				End If

			End If

		End Sub

#End Region

	End Class

End Namespace
