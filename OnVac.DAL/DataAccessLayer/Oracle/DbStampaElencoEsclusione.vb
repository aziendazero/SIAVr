Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager


Namespace DAL

    Public Class DbStampaElencoEsclusione
        Inherits DbProvider
        Implements IStampaElencoEsclusione

#Region " Constructors "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " IStampaElencoEsclusione "


        Public Function GetDataSetElencoEsclusione(filtriElencoEsclusione As IStampaElencoEsclusione.FiltriElencoEsclusione, codiceUsl As String) As DataSet.DSElencoEsclusione Implements IStampaElencoEsclusione.GetDataSetElencoEsclusione

            Dim dsElencoEsclusione As New DataSet.DSElencoEsclusione()

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("pno_testo_note")
                .AddTables("t_ana_tipo_note, t_paz_note")
                .AddWhereCondition("tno_codice", Comparatori.Uguale, "pno_tno_codice", DataTypes.Join)
                .AddWhereCondition("pno_paz_codice", Comparatori.Uguale, "paz_codice", DataTypes.Join)
                .OpenParanthesis()
                .AddWhereCondition("pno_azi_codice", Comparatori.Uguale, codiceUsl, DataTypes.Stringa)
                .AddWhereCondition("pno_azi_codice", Comparatori.In, String.Format("select dis_codice from t_ana_distretti where dis_usl_codice = '{0}'", codiceUsl), DataTypes.Replace, "OR")
                .CloseParanthesis()
                .AddWhereCondition("pno_tno_codice", Comparatori.Uguale, Constants.CodiceTipoNotaPaziente.Solleciti, DataTypes.Stringa)
            End With
            Dim queryNotaSolleciti As String = _DAM.QB.GetSelect()

            With _DAM.QB
                .NewQuery(False, False)
                .AddSelectFields("pno_testo_note")
                .AddTables("t_ana_tipo_note, t_paz_note")
                .AddWhereCondition("tno_codice", Comparatori.Uguale, "pno_tno_codice", DataTypes.Join)
                .AddWhereCondition("pno_paz_codice", Comparatori.Uguale, "paz_codice", DataTypes.Join)
                .OpenParanthesis()
                .AddWhereCondition("pno_azi_codice", Comparatori.Uguale, codiceUsl, DataTypes.Stringa)
                .AddWhereCondition("pno_azi_codice", Comparatori.In, String.Format("select dis_codice from t_ana_distretti where dis_usl_codice = '{0}'", codiceUsl), DataTypes.Replace, "OR")
                .CloseParanthesis()
                .AddWhereCondition("pno_tno_codice", Comparatori.Uguale, Constants.CodiceTipoNotaPaziente.Annotazioni, DataTypes.Stringa)
            End With
            Dim queryNotaAnnotazioni As String = _DAM.QB.GetSelect()


            _DAM.QB.NewQuery(False, False)

            With _DAM.QB

                .AddSelectFields("DISTINCT PAZ_CODICE,PAZ_NOME,PAZ_COGNOME,PAZ_DATA_NASCITA,PAZ_INDIRIZZO_RESIDENZA")
                .AddSelectFields("(" + queryNotaAnnotazioni + ") PAZ_NOTE")
                .AddSelectFields("(" + queryNotaSolleciti + ") PAZ_NOTE_SOLLECITI")
                .AddSelectFields("VEX_DATA_VISITA,VEX_DATA_SCADENZA,VAC_DESCRIZIONE,MOE_DESCRIZIONE,VAC_OBBLIGATORIA")
                .AddSelectFields("CNS_DESCRIZIONE,COM_DESCRIZIONE,OPE_NOME,PAZ_STATO_ANAGRAFICO,SAN_DESCRIZIONE")

                .AddTables("T_VAC_ESCLUSE, T_PAZ_PAZIENTI, T_ANA_VACCINAZIONI, T_ANA_MOTIVI_ESCLUSIONE")
                .AddTables("T_ANA_CONSULTORI, T_ANA_COMUNI,T_ANA_OPERATORI, T_ANA_STATI_ANAGRAFICI")

                .AddWhereCondition("VEX_PAZ_CODICE", Comparatori.Uguale, "PAZ_CODICE", DataTypes.Join)
                .AddWhereCondition("VEX_VAC_CODICE", Comparatori.Uguale, "VAC_CODICE", DataTypes.Join)
                .AddWhereCondition("VEX_MOE_CODICE", Comparatori.Uguale, "MOE_CODICE", DataTypes.OutJoinLeft)
                .AddWhereCondition("PAZ_COM_CODICE_RESIDENZA", Comparatori.Uguale, "COM_CODICE", DataTypes.OutJoinLeft)
                .AddWhereCondition("PAZ_CNS_CODICE", Comparatori.Uguale, "CNS_CODICE", DataTypes.OutJoinLeft)
                .AddWhereCondition("VEX_OPE_CODICE", Comparatori.Uguale, "OPE_CODICE", DataTypes.OutJoinLeft)
                .AddWhereCondition("PAZ_STATO_ANAGRAFICO", Comparatori.Uguale, "SAN_CODICE", DataTypes.OutJoinLeft)

                .AddWhereCondition("PAZ_DATA_NASCITA", Comparatori.MaggioreUguale, filtriElencoEsclusione.DataNascitaInizio, DataTypes.Data)
                .AddWhereCondition("PAZ_DATA_NASCITA", Comparatori.Minore, filtriElencoEsclusione.DataNascitaFine.AddDays(1), DataTypes.Data)

            End With

            Me.SetFiltriEsclusione(filtriElencoEsclusione, _DAM.QB)

            _DAM.BuildDataTable(dsElencoEsclusione.ElencoEsclusione)

            Return dsElencoEsclusione

        End Function

        Public Function GetDataSetElencoEsclusioneVaccinazioni(filtriElencoEsclusione As IStampaElencoEsclusione.FiltriElencoEsclusione) As DataSet.DSElencoEsclusione Implements IStampaElencoEsclusione.GetDataSetElencoEsclusioneVaccinazioni

            Dim dsElencoEsclusione As New DataSet.DSElencoEsclusione()

            _DAM.QB.NewQuery()

            With _DAM.QB

                .AddSelectFields("VAC_CODICE,VAC_DESCRIZIONE,VAC_OBBLIGATORIA,COUNT(*) as TOT_ESONERI")

                .AddTables("T_VAC_ESCLUSE, T_PAZ_PAZIENTI, T_ANA_MOTIVI_ESCLUSIONE, T_ANA_VACCINAZIONI, T_ANA_CONSULTORI,T_ANA_COMUNI")

                .AddWhereCondition("VEX_PAZ_CODICE", Comparatori.Uguale, "PAZ_CODICE", DataTypes.Join)
                .AddWhereCondition("VEX_VAC_CODICE", Comparatori.Uguale, "VAC_CODICE", DataTypes.Join)
                .AddWhereCondition("VEX_MOE_CODICE", Comparatori.Uguale, "MOE_CODICE", DataTypes.OutJoinLeft)
                .AddWhereCondition("PAZ_COM_CODICE_RESIDENZA", Comparatori.Uguale, "COM_CODICE", DataTypes.OutJoinLeft)
                .AddWhereCondition("PAZ_CNS_CODICE", Comparatori.Uguale, "CNS_CODICE", DataTypes.OutJoinLeft)

                .AddWhereCondition("PAZ_DATA_NASCITA", Comparatori.MaggioreUguale, filtriElencoEsclusione.DataNascitaInizio, DataTypes.Data)
                .AddWhereCondition("PAZ_DATA_NASCITA", Comparatori.Minore, filtriElencoEsclusione.DataNascitaFine.AddDays(1), DataTypes.Data)
                .OpenParanthesis()
                .AddWhereCondition("VEX_DATA_SCADENZA", Comparatori.Is, "NULL", DataTypes.Replace)
                .AddWhereCondition("VEX_DATA_SCADENZA", Comparatori.Maggiore, DateTime.Today, DataTypes.Data, "OR")
                .CloseParanthesis()

                .AddGroupByFields("VAC_CODICE,VAC_DESCRIZIONE,VAC_OBBLIGATORIA")

            End With

            Me.SetFiltriEsclusione(filtriElencoEsclusione, _DAM.QB)

            _DAM.BuildDataTable(dsElencoEsclusione.RiassuntoEsclusione)

            Return dsElencoEsclusione

        End Function

#End Region

#Region " Private "

        Private Sub SetFiltriEsclusione(filtriElencoEsclusione As IStampaElencoEsclusione.FiltriElencoEsclusione, ByRef abstractQB As AbstractQB)

            With abstractQB

				' CONSULTORIO
				If filtriElencoEsclusione.CodiceConsultorio.Count > 0 Then
					Dim filtroIn As New System.Text.StringBuilder()
					For i As Integer = 0 To filtriElencoEsclusione.CodiceConsultorio.Count - 1
						filtroIn.AppendFormat("{0},", .AddCustomParam(filtriElencoEsclusione.CodiceConsultorio(i)))
					Next

					If filtroIn.Length > 0 Then
						filtroIn.Remove(filtroIn.Length - 1, 1)
						.AddWhereCondition("T_PAZ_PAZIENTI.PAZ_CNS_CODICE", Comparatori.In, filtroIn.ToString(), DataTypes.Replace)
					End If

				End If

				' COMUNE DI RESIDENZA
				If Not String.IsNullOrEmpty(filtriElencoEsclusione.CodiceComuneResidenza) And Not String.IsNullOrEmpty(filtriElencoEsclusione.DescrizioneComuneResidenza) Then
                    .AddWhereCondition("T_PAZ_PAZIENTI.PAZ_COM_CODICE_RESIDENZA", Comparatori.Uguale, filtriElencoEsclusione.CodiceComuneResidenza, DataTypes.Stringa)
                End If

                ' CIRCOSCRIZIONE
                If Not String.IsNullOrEmpty(filtriElencoEsclusione.CodiceCircoscrizione) And Not String.IsNullOrEmpty(filtriElencoEsclusione.DescrizioneCircoscrizione) Then
                    .AddWhereCondition("T_PAZ_PAZIENTI.PAZ_CIR_CODICE", Comparatori.Uguale, filtriElencoEsclusione.CodiceCircoscrizione, DataTypes.Stringa)
                End If

                ' DISTRETTO
                If Not String.IsNullOrEmpty(filtriElencoEsclusione.CodiceDistretto) And Not String.IsNullOrEmpty(filtriElencoEsclusione.DescrizioneDistretto) Then
                    .AddWhereCondition("T_PAZ_PAZIENTI.PAZ_DIS_CODICE", Comparatori.Uguale, filtriElencoEsclusione.CodiceDistretto, DataTypes.Stringa)
                End If

                ' VACCINAZIONI OBBLIGATORIE
                If filtriElencoEsclusione.SoloVaccinazioniObbligatorie Then
                    .AddWhereCondition("VAC_OBBLIGATORIA", Comparatori.Uguale, Constants.ObbligatorietaVaccinazione.Obbligatoria, DataTypes.Stringa)
                End If

                ' MOTIVO ESCLUSIONE
                If Not String.IsNullOrEmpty(filtriElencoEsclusione.CodiceMotivoEsclusione) Then
                    .AddWhereCondition("MOE_CODICE", Comparatori.Uguale, filtriElencoEsclusione.CodiceMotivoEsclusione, DataTypes.Stringa)
                End If

                ' VACCINAZIONI
                If Not String.IsNullOrEmpty(filtriElencoEsclusione.CodiceVaccinazione) Then
                    .AddWhereCondition("VAC_CODICE", Comparatori.Uguale, filtriElencoEsclusione.CodiceVaccinazione, DataTypes.Stringa)
                End If

                ' STATO ANAGRAFICO
                If Not String.IsNullOrEmpty(filtriElencoEsclusione.CodiciStatiAnagrafici) Then
                    .AddWhereCondition("PAZ_STATO_ANAGRAFICO", Comparatori.In, filtriElencoEsclusione.CodiciStatiAnagrafici, DataTypes.Stringa)
                End If

            End With

        End Sub

#End Region

    End Class

End Namespace