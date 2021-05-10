Imports Onit.Database.DataAccessManager

Namespace DAL.Oracle

    Public Class DbStampaLibrettoVaccinale
        Inherits DbProvider
        Implements IStampaLibrettoVaccinale

#Region " Costruttori "

        Public Sub New(ByRef DAM As IDAM)
            MyBase.New(DAM)
        End Sub

#End Region

#Region " IStampaLibrettoVaccinale "

        Public Function GetDtVaccinazioniPaziente(codicePaziente As Integer) As DataTable Implements IStampaLibrettoVaccinale.GetDtVaccinazioniPaziente

            Dim dtVacPaziente As New DataTable()

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("VES_PAZ_CODICE, VES_VAC_CODICE, VES_N_RICHIAMO, VES_DATA_EFFETTUAZIONE, ASS_STAMPA, VAC_DESCRIZIONE, VAC_COD_UNICO, VES_FLAG_FITTIZIA")
                .AddTables("T_VAC_ESEGUITE, T_ANA_ASSOCIAZIONI, T_ANA_VACCINAZIONI")
                .AddWhereCondition("VES_ASS_CODICE", Comparatori.Uguale, "ASS_CODICE", DataTypes.OutJoinLeft)
                .AddWhereCondition("VES_VAC_CODICE", Comparatori.Uguale, "VAC_CODICE", DataTypes.Join)
                .AddWhereCondition("VES_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
            End With

            _DAM.BuildDataTable(dtVacPaziente)

            Return dtVacPaziente


        End Function

        Public Function GetDataSetLibrettoVaccinale(codicePaziente As Integer, vaccinatore As String) As DSLibrettoVaccinale Implements IStampaLibrettoVaccinale.GetDataSetLibrettoVaccinale

            Dim dsLibrettoVaccinale As New DSLibrettoVaccinale()

            With _DAM.QB

                .NewQuery()

                .AddSelectFields("paz_codice, paz_cognome, paz_nome, paz_data_nascita",
                                 "(select com_descrizione from t_ana_comuni where com_codice=paz_com_codice_residenza)as paz_com_residenza",
                                 "paz_indirizzo_residenza, paz_cap_residenza",
                                 "(select com_descrizione from t_ana_comuni where com_codice=paz_com_codice_domicilio)as paz_com_domicilio",
                                 "paz_indirizzo_domicilio", "paz_cap_domicilio",
                                 "(select com_descrizione from t_ana_comuni where com_codice=paz_com_codice_nascita)as paz_com_nascita",
                                 "paz_codice_demografico, paz_cir_codice, paz_sesso, paz_codice_fiscale",
                                 "com_descrizione, vac_descrizione, ves_n_richiamo, ves_data_effettuazione, ves_lot_codice")
                .AddSelectFields(vaccinatore + " AS VES_MED_VACCINANTE")
                .AddSelectFields("ves_n_seduta, cic_codice, cic_descrizione, cic_data_introduzione, vac_obbligatoria, tsd_cic_codice, tsd_n_seduta",
                                 "tsd_eta_seduta, tsd_durata_seduta, tsd_intervallo, noc_descrizione, lot_data_scadenza, ves_flag_fittizia")

                .AddTables("t_paz_pazienti, t_ana_comuni, t_ana_vaccinazioni, t_vac_eseguite, t_ana_tempi_sedute",
                           "t_ana_cicli, t_paz_indirizzi, t_ana_nomi_commerciali, t_ana_lotti")

                .AddWhereCondition("paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("paz_codice", Comparatori.Uguale, "ves_paz_codice", DataTypes.Join)
                .AddWhereCondition("paz_com_codice_residenza", Comparatori.Uguale, "com_codice", DataTypes.OutJoinLeft)

                .AddWhereCondition("ves_noc_codice", Comparatori.Uguale, "noc_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("ves_lot_codice", Comparatori.Uguale, "lot_codice", DataTypes.OutJoinLeft)

                .AddWhereCondition("ves_vac_codice", Comparatori.Uguale, "vac_codice", DataTypes.OutJoinLeft)

                .AddWhereCondition("ves_cic_codice", Comparatori.Uguale, "tsd_cic_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("ves_n_seduta", Comparatori.Uguale, "tsd_n_seduta", DataTypes.OutJoinLeft)
                .AddWhereCondition("tsd_cic_codice", Comparatori.Uguale, "cic_codice", DataTypes.OutJoinLeft)

                .AddWhereCondition("paz_ind_codice_dom", Comparatori.Uguale, "ind_codice", DataTypes.OutJoinLeft)

            End With

            _DAM.BuildDataTable(dsLibrettoVaccinale.dtLibVac)

            Return dsLibrettoVaccinale

        End Function

#End Region

    End Class

End Namespace