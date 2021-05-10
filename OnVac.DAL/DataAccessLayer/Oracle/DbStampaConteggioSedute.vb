Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager


Namespace DAL

	Public Class DbStampaConteggioSedute
		Inherits DbProvider
		Implements IStampaConteggioSedute

#Region " Constructors "

		Public Sub New(ByRef DAM As IDAM)

			MyBase.New(DAM)

		End Sub

#End Region

#Region " IStampaElencoEsclusione "


		Public Function GetDataSetConteggioSedute(filtriConteggio As IStampaConteggioSedute.FiltriConteggioSedute, codiceUsl As String) As DSConteggioSedute Implements IStampaConteggioSedute.GetDataSetConteggioSedute

			Dim dsConteggioSedute As New DSConteggioSedute


			Dim parOrarioPM As String = queryPar("AL.ves_cns_codice", "ORAPM")
			Dim parNumeroVacFasciaOraria As String = queryPar("AL.ves_cns_codice", "NUM_MIN_VAC_PER_FASCIA_ORARIA")
			Dim mattino As String = String.Format("CASE
			WHEN(
				(select COUNT(DISTINCT VES_PAZ_CODICE) from V_VAC_ESEGUITE_UNION_SCADUTE B2
				WHERE B2.VES_CNS_CODICE = AL.VES_CNS_CODICE
				and B2.ves_data_effettuazione = AL.ves_data_effettuazione
				and TO_CHAR(ves_dataora_effettuazione,'hh24:mi')<=
				{0}
				GROUP BY ves_data_effettuazione)
				>=
				{1}
				) then 1 else 0 end as mattino", parOrarioPM, parNumeroVacFasciaOraria)

			Dim pomeriggio As String = String.Format("CASE
			WHEN(
				(select COUNT(DISTINCT VES_PAZ_CODICE) from V_VAC_ESEGUITE_UNION_SCADUTE B2
				WHERE B2.VES_CNS_CODICE = AL.VES_CNS_CODICE
				and B2.ves_data_effettuazione = AL.ves_data_effettuazione
				and TO_CHAR(ves_dataora_effettuazione,'hh24:mi')>=
				{0}
				GROUP BY ves_data_effettuazione)
				>=
				{1}
				) then 1 else 0 end as pomeriggio", parOrarioPM, parNumeroVacFasciaOraria)

			With _DAM.QB
				.NewQuery()
				.IsDistinct = True
				.AddSelectFields("ves_cns_codice,cns_descrizione,ves_data_effettuazione ves_dataora_effettuazione ", mattino, pomeriggio)
				.AddTables("V_VAC_ESEGUITE_UNION_SCADUTE AL, t_paz_pazienti P1, t_ana_consultori CN1, t_report_gruppo G1, t_ana_circoscrizioni CR1")
				.AddWhereCondition("AL.ves_paz_codice", Comparatori.Uguale, "P1.paz_codice", DataTypes.Join)
				.AddWhereCondition("P1.paz_cir_codice", Comparatori.Uguale, "CR1.cir_codice", DataTypes.OutJoinLeft)
				.AddWhereCondition("AL.ves_cns_codice", Comparatori.Uguale, "CN1.cns_codice", DataTypes.OutJoinLeft)
				.AddWhereCondition("CN1.cns_codice", Comparatori.Uguale, "G1.gruppo", DataTypes.OutJoinLeft)

			End With

			Me.SetFiltri(filtriConteggio, _DAM.QB)

			_DAM.BuildDataTable(dsConteggioSedute.ConteggioSedute)

			Return dsConteggioSedute

		End Function

		Private Function queryPar(codiceCns As String, parametro As String) As String
			Dim query As String

			query = String.Format(
						"(select par_valore
                          from t_ana_parametri par1
                          where (par1.par_cns_codice = NVL({1},'{0}') or par1.par_cns_codice = '{0}')
                          and par1.par_codice = '{2}'
                          and not exists (
                                select * from t_ana_parametri par2
                                where par2.par_codice = par1.par_codice
                                and par1.par_cns_codice = '{0}'
                                and par2.par_cns_codice = {1}
                                and par2.par_codice = '{2}'
                            ))", Constants.CommonConstants.CodiceConsultorioSistema, codiceCns, parametro)

			Return query
		End Function

#End Region

#Region " Private "

		Private Sub SetFiltri(filtriConteggio As IStampaConteggioSedute.FiltriConteggioSedute, ByRef abstractQB As AbstractQB)

			With abstractQB

				.AddWhereCondition("AL.ves_data_effettuazione", Comparatori.MaggioreUguale, filtriConteggio.DataEffettuazioneInizio, DataTypes.Data)
				.AddWhereCondition("AL.ves_data_effettuazione", Comparatori.MinoreUguale, filtriConteggio.DataEffettuazioneFine, DataTypes.Data)

				' CONSULTORIO
				If filtriConteggio.CodiceConsultorio.Count > 0 Then
					Dim filtroIn As New System.Text.StringBuilder()
					For i As Integer = 0 To filtriConteggio.CodiceConsultorio.Count - 1
						filtroIn.AppendFormat("{0},", .AddCustomParam(filtriConteggio.CodiceConsultorio(i)))
					Next

					If filtroIn.Length > 0 Then
						filtroIn.Remove(filtroIn.Length - 1, 1)
						.AddWhereCondition("AL.ves_cns_codice", Comparatori.In, filtroIn.ToString(), DataTypes.Replace)
					End If

				End If



				' DISTRETTO
				If Not String.IsNullOrEmpty(filtriConteggio.CodiceDistretto) Then
					.AddWhereCondition("CN1.cns_dis_codice", Comparatori.Uguale, filtriConteggio.CodiceDistretto, DataTypes.Stringa)
				End If

				'filtri per tipo vaccinazione (VES_STATO: R - Registrata, altrimenti Eseguita)
				If (filtriConteggio.FlagVacciniEseguiti = True) And (filtriConteggio.FlagVacciniRegistrati = False) Then
					.OpenParanthesis()
					.AddWhereCondition("AL.ves_stato", Comparatori.Is, "NULL", DataTypes.Stringa)
					.AddWhereCondition("AL.ves_stato", Comparatori.Diverso, "R", DataTypes.Stringa, "OR")
					.CloseParanthesis()
				ElseIf (filtriConteggio.FlagVacciniEseguiti = False) And (filtriConteggio.FlagVacciniRegistrati = True) Then
					.AddWhereCondition("AL.ves_stato", Comparatori.Uguale, "R", DataTypes.Stringa)
				End If
				' filtri per tipo consultorio (A - adulti, P - pediatrico, V - vaccinatore)
				If (filtriConteggio.FlagTipoCentroAdulti = True) And (filtriConteggio.FlagTipoCentroPediatrico = True) And (filtriConteggio.FlagTipoCentroVaccinatore = False) Then

					.OpenParanthesis()
					.AddWhereCondition("CN1.cns_tipo", Comparatori.Uguale, "A", DataTypes.Stringa)
					.AddWhereCondition("CN1.cns_tipo", Comparatori.Uguale, "P", DataTypes.Stringa, "or")
					.CloseParanthesis()



				ElseIf (filtriConteggio.FlagTipoCentroAdulti = True) And (filtriConteggio.FlagTipoCentroPediatrico = False) And (filtriConteggio.FlagTipoCentroVaccinatore = True) Then

					.OpenParanthesis()
					.AddWhereCondition("CN1.cns_tipo", Comparatori.Uguale, "A", DataTypes.Stringa)
					.AddWhereCondition("CN1.cns_tipo", Comparatori.Uguale, "V", DataTypes.Stringa, "or")
					.CloseParanthesis()


				ElseIf (filtriConteggio.FlagTipoCentroAdulti = True) And (filtriConteggio.FlagTipoCentroPediatrico = False) And (filtriConteggio.FlagTipoCentroVaccinatore = False) Then

					.AddWhereCondition("CN1.cns_tipo", Comparatori.Uguale, "A", DataTypes.Stringa)


				ElseIf (filtriConteggio.FlagTipoCentroAdulti = False) And (filtriConteggio.FlagTipoCentroPediatrico = True) And (filtriConteggio.FlagTipoCentroVaccinatore = False) Then

					.AddWhereCondition("CN1.cns_tipo", Comparatori.Uguale, "P", DataTypes.Stringa)


				ElseIf (filtriConteggio.FlagTipoCentroAdulti = False) And (filtriConteggio.FlagTipoCentroPediatrico = False) And (filtriConteggio.FlagTipoCentroVaccinatore = True) Then

					.AddWhereCondition("CN1.cns_tipo", Comparatori.Uguale, "V", DataTypes.Stringa)


				ElseIf (filtriConteggio.FlagTipoCentroAdulti = False) And (filtriConteggio.FlagTipoCentroPediatrico = True) And (filtriConteggio.FlagTipoCentroVaccinatore = True) Then

					.OpenParanthesis()
					.AddWhereCondition("CN1.cns_tipo", Comparatori.Uguale, "P", DataTypes.Stringa)
					.AddWhereCondition("CN1.cns_tipo", Comparatori.Uguale, "V", DataTypes.Stringa, "or")
					.CloseParanthesis()

				End If


			End With

		End Sub

#End Region

	End Class

End Namespace