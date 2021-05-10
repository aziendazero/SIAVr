namespace Onit.OnAssistnet.OnVac.Queries.VaccinazioniEseguite
{
    /// <summary>
    /// Query Vaccinazioni Eseguite
    /// </summary>
    public static class OracleQueries
    {

        /// <summary>
        /// Inserimento vaccinazione eseguita
        /// </summary>
        public static string selNextSeqEseguita
        {
            get
            {
                return @"SELECT SEQ_ESEGUITE.NEXTVAL FROM DUAL";
            }
        }

        /// <summary>
        /// Inserimento reazione avversa
        /// </summary>
        public static string selNextSeqReazioneAvversa
        {
            get
            {
                return @"SELECT SEQ_REAZIONI_AVVERSE.NEXTVAL FROM DUAL";
            }
        }
      
        /// <summary>
        /// Vaccinazione eseguita corrispondente al codice paziente, vaccinazione  e dose specificati
       /// </summary>
        public static string selVaccinazioniEseguitePaziente
        {
            get
            {
                return @"select * from t_vac_eseguite 
                            where ves_paz_codice = :codicePaziente 
                            and ves_vac_codice = :codiceVaccinazione 
                            and ves_n_richiamo = :numeroRichiamo";
            }
        }

        /// <summary>
        /// Vaccinazione eseguita corrispondente al ves_paz_codice, ves_vac_codice, ves_n_richiamo e ves_data_effettuazione specificati 
        /// </summary>
        public static string selVaccinazioniEseguitePaziente2
        {
            get
            {
                return @"select * from t_vac_eseguite 
                            where ves_paz_codice = :ves_paz_codice and ves_vac_codice = :ves_vac_codice 
                            and ves_data_effettuazione = :ves_data_effettuazione and ves_n_richiamo = :ves_n_richiamo";
            }
        }

        /// <summary>
        /// Vaccinazione eseguita corrispondente al ves_id specificato
        /// </summary>
        public static string selVaccinazioneEseguitaById
        {
            get
            {
                return @"select * from t_vac_eseguite where ves_id = :ves_id";
            }
        }

        /// <summary>
        /// Vaccinazione eseguita Scaduta corrispondente al vsc_id specificato
        /// </summary>
        public static string selVaccinazioneEseguitaScadutaById
        {
            get
            {
                return @"select * from t_vac_scadute where vsc_id = :vsc_id";
            }
        }

        /// <summary>
        /// Vaccinazioni Eseguite, Scadute, Eseguite Eliminate e Scadute Eliminate relative agli id specificati (solo i dati delle 4 tabelle, in union, senza nessun join)
        /// </summary>
        public static string selVaccinazioniEseguiteScaduteEliminateByListaId
        {
            get
            {
                return @"select VES_PAZ_CODICE, VES_VAC_CODICE, VES_N_RICHIAMO, VES_DATA_EFFETTUAZIONE, 
VES_DATAORA_EFFETTUAZIONE, VES_CNS_CODICE, VES_STATO, VES_CIC_CODICE, VES_N_SEDUTA, 
VES_DATA_REGISTRAZIONE, VES_LOT_CODICE, VES_OPE_CODICE, VES_UTE_ID, VES_VII_CODICE, 
VES_SII_CODICE, VES_NOC_CODICE, VES_ASS_CODICE, VES_LUOGO, VES_PAZ_CODICE_OLD, 
VES_COMUNE_O_STATO, VES_MED_VACCINANTE, VES_CNV_DATA, VES_IN_CAMPAGNA, VES_ESITO, 
VES_OPE_IN_AMBULATORIO, VES_NOTE, VES_CNS_REGISTRAZIONE, VES_ACCESSO, VES_AMB_CODICE, VES_ID, 
VES_IMPORTO, VES_MAL_CODICE_MALATTIA, VES_CODICE_ESENZIONE, VES_CNV_DATA_PRIMO_APP,
VES_ASS_N_DOSE, VES_ASS_PROG, VES_FLAG_FITTIZIA, VES_DATA_ULTIMA_VARIAZIONE, 
VES_UTE_ID_ULTIMA_VARIAZIONE, VES_USL_INSERIMENTO, VES_USL_SCADENZA, VES_DATA_SCADENZA, 
VES_UTE_ID_SCADENZA, VES_DATA_RIPRISTINO, VES_UTE_ID_RIPRISTINO, VES_FLAG_VISIBILITA, 
VES_NOTE_ACQUISIZIONE, VES_ID_ACN, VES_PROVENIENZA, 
VES_RSC_CODICE, VES_MAL_CODICE_COND_SANITARIA, VES_TPA_GUID_TIPI_PAGAMENTO,
VES_LOT_DATA_SCADENZA, VES_PAGATO, VES_ANTIGENE, VES_TIPO_EROGATORE, 
VES_CODICE_STRUTTURA, VES_USL_COD_SOMMINISTRAZIONE, VES_TIPO_ASSOCIAZIONE_ACN 
from t_vac_eseguite where ves_id in ({0})
union
select VSC_PAZ_CODICE VES_PAZ_CODICE, VSC_VAC_CODICE VES_VAC_CODICE, VSC_N_RICHIAMO VES_N_RICHIAMO, VSC_DATA_EFFETTUAZIONE VES_DATA_EFFETTUAZIONE, 
VSC_DATAORA_EFFETTUAZIONE VES_DATAORA_EFFETTUAZIONE, VSC_CNS_CODICE VES_CNS_CODICE, VSC_STATO VES_STATO, VSC_CIC_CODICE VES_CIC_CODICE, VSC_N_SEDUTA VES_N_SEDUTA, 
VSC_DATA_REGISTRAZIONE VES_DATA_REGISTRAZIONE, VSC_LOT_CODICE VES_LOT_CODICE, VSC_OPE_CODICE VES_OPE_CODICE, VSC_UTE_ID VES_UTE_ID, VSC_VII_CODICE VES_VII_CODICE, 
VSC_SII_CODICE VES_SII_CODICE, VSC_NOC_CODICE VES_NOC_CODICE, VSC_ASS_CODICE VES_ASS_CODICE, VSC_LUOGO VES_LUOGO, VSC_PAZ_CODICE_OLD VES_PAZ_CODICE_OLD,
VSC_COMUNE_O_STATO VES_COMUNE_O_STATO, VSC_MED_VACCINANTE VES_MED_VACCINANTE, VSC_CNV_DATA VES_CNV_DATA, VSC_IN_CAMPAGNA VES_IN_CAMPAGNA, VSC_ESITO VES_ESITO,
VSC_OPE_IN_AMBULATORIO VES_OPE_IN_AMBULATORIO, VSC_NOTE VES_NOTE, VSC_CNS_REGISTRAZIONE VES_CNS_REGISTRAZIONE, VSC_ACCESSO VES_ACCESSO, VSC_AMB_CODICE VES_AMB_CODICE, VSC_ID VES_ID,
VSC_IMPORTO VES_IMPORTO, VSC_MAL_CODICE_MALATTIA VES_MAL_CODICE_MALATTIA, VSC_CODICE_ESENZIONE VES_CODICE_ESENZIONE, VSC_CNV_DATA_PRIMO_APP VES_CNV_DATA_PRIMO_APP, 
VSC_ASS_N_DOSE VES_ASS_N_DOSE, VSC_ASS_PROG VES_ASS_PROG, VSC_FLAG_FITTIZIA VES_FLAG_FITTIZIA, VSC_DATA_ULTIMA_VARIAZIONE VES_DATA_ULTIMA_VARIAZIONE, 
VSC_UTE_ID_ULTIMA_VARIAZIONE VES_UTE_ID_ULTIMA_VARIAZIONE, VSC_USL_INSERIMENTO VES_USL_INSERIMENTO, VSC_USL_SCADENZA VES_USL_SCADENZA, VSC_DATA_SCADENZA VES_DATA_SCADENZA, 
VSC_UTE_ID_SCADENZA VES_UTE_ID_SCADENZA, VSC_DATA_RIPRISTINO VES_DATA_RIPRISTINO, VSC_UTE_ID_RIPRISTINO VES_UTE_ID_RIPRISTINO, VSC_FLAG_VISIBILITA VES_FLAG_VISIBILITA, 
VSC_NOTE_ACQUISIZIONE VES_NOTE_ACQUISIZIONE, VSC_ID_ACN VES_ID_ACN, VSC_PROVENIENZA VES_PROVENIENZA,
VSC_RSC_CODICE VES_RSC_CODICE, VSC_MAL_CODICE_COND_SANITARIA VES_MAL_CODICE_COND_SANITARIA, VSC_TPA_GUID_TIPI_PAGAMENTO VES_TPA_GUID_TIPI_PAGAMENTO,
VSC_LOT_DATA_SCADENZA VES_LOT_DATA_SCADENZA, VSC_PAGATO VES_PAGATO, VSC_ANTIGENE VES_ANTIGENE, VSC_TIPO_EROGATORE VES_TIPO_EROGATORE, 
VSC_CODICE_STRUTTURA VES_CODICE_STRUTTURA, VSC_USL_COD_SOMMINISTRAZIONE VES_USL_COD_SOMMINISTRAZIONE, VSC_TIPO_ASSOCIAZIONE_ACN VES_TIPO_ASSOCIAZIONE_ACN 
from t_vac_scadute where vsc_id in ({0})
union
select VEE_PAZ_CODICE VES_PAZ_CODICE, VEE_VAC_CODICE VES_VAC_CODICE, VEE_N_RICHIAMO VES_N_RICHIAMO, VEE_DATA_EFFETTUAZIONE VES_DATA_EFFETTUAZIONE, 
VEE_DATAORA_EFFETTUAZIONE VES_DATAORA_EFFETTUAZIONE, VEE_CNS_CODICE VES_CNS_CODICE, VEE_STATO VES_STATO, VEE_CIC_CODICE VES_CIC_CODICE, VEE_N_SEDUTA VES_N_SEDUTA, 
VEE_DATA_REGISTRAZIONE VES_DATA_REGISTRAZIONE, VEE_LOT_CODICE VES_LOT_CODICE, VEE_OPE_CODICE VES_OPE_CODICE, VEE_UTE_ID VES_UTE_ID, VEE_VII_CODICE VES_VII_CODICE, 
VEE_SII_CODICE VES_SII_CODICE, VEE_NOC_CODICE VES_NOC_CODICE, VEE_ASS_CODICE VES_ASS_CODICE, VEE_LUOGO VES_LUOGO, VEE_PAZ_CODICE_OLD VES_PAZ_CODICE_OLD,
VEE_COMUNE_O_STATO VES_COMUNE_O_STATO, VEE_MED_VACCINANTE VES_MED_VACCINANTE, VEE_CNV_DATA VES_CNV_DATA, VEE_IN_CAMPAGNA VES_IN_CAMPAGNA, VEE_ESITO VES_ESITO,
VEE_OPE_IN_AMBULATORIO VES_OPE_IN_AMBULATORIO, VEE_NOTE VES_NOTE, VEE_CNS_REGISTRAZIONE VES_CNS_REGISTRAZIONE, VEE_ACCESSO VES_ACCESSO, VEE_AMB_CODICE VES_AMB_CODICE, VEE_ID VES_ID,
VEE_IMPORTO VES_IMPORTO, VEE_MAL_CODICE_MALATTIA VES_MAL_CODICE_MALATTIA, VEE_CODICE_ESENZIONE VES_CODICE_ESENZIONE, VEE_CNV_DATA_PRIMO_APP VES_CNV_DATA_PRIMO_APP, 
VEE_ASS_N_DOSE VES_ASS_N_DOSE, VEE_ASS_PROG VES_ASS_PROG, VEE_FLAG_FITTIZIA VES_FLAG_FITTIZIA, VEE_DATA_ULTIMA_VARIAZIONE VES_DATA_ULTIMA_VARIAZIONE, 
VEE_UTE_ID_ULTIMA_VARIAZIONE VES_UTE_ID_ULTIMA_VARIAZIONE, VEE_USL_INSERIMENTO VES_USL_INSERIMENTO, VEE_USL_SCADENZA VES_USL_SCADENZA, VEE_DATA_SCADENZA VES_DATA_SCADENZA, 
VEE_UTE_ID_SCADENZA VES_UTE_ID_SCADENZA, VEE_DATA_RIPRISTINO VES_DATA_RIPRISTINO, VEE_UTE_ID_RIPRISTINO VES_UTE_ID_RIPRISTINO, VEE_FLAG_VISIBILITA VES_FLAG_VISIBILITA, 
VEE_NOTE_ACQUISIZIONE VES_NOTE_ACQUISIZIONE, VEE_ID_ACN VES_ID_ACN, VEE_PROVENIENZA VES_PROVENIENZA,
VEE_RSC_CODICE VES_RSC_CODICE, VEE_MAL_CODICE_COND_SANITARIA VES_MAL_CODICE_COND_SANITARIA, VEE_TPA_GUID_TIPI_PAGAMENTO VES_TPA_GUID_TIPI_PAGAMENTO,
VEE_LOT_DATA_SCADENZA VES_LOT_DATA_SCADENZA, VEE_PAGATO VES_PAGATO, VEE_ANTIGENE VES_ANTIGENE, VEE_TIPO_EROGATORE VES_TIPO_EROGATORE, 
VEE_CODICE_STRUTTURA VES_CODICE_STRUTTURA, VEE_USL_COD_SOMMINISTRAZIONE VES_USL_COD_SOMMINISTRAZIONE, VEE_TIPO_ASSOCIAZIONE_ACN VES_TIPO_ASSOCIAZIONE_ACN 
from t_vac_eseguite_eliminate where vee_id in ({0})
union
select VSE_PAZ_CODICE VES_PAZ_CODICE, VSE_VAC_CODICE VES_VAC_CODICE, VSE_N_RICHIAMO VES_N_RICHIAMO, VSE_DATA_EFFETTUAZIONE VES_DATA_EFFETTUAZIONE, 
VSE_DATAORA_EFFETTUAZIONE VES_DATAORA_EFFETTUAZIONE, VSE_CNS_CODICE VES_CNS_CODICE, VSE_STATO VES_STATO, VSE_CIC_CODICE VES_CIC_CODICE, VSE_N_SEDUTA VES_N_SEDUTA, 
VSE_DATA_REGISTRAZIONE VES_DATA_REGISTRAZIONE, VSE_LOT_CODICE VES_LOT_CODICE, VSE_OPE_CODICE VES_OPE_CODICE, VSE_UTE_ID VES_UTE_ID, VSE_VII_CODICE VES_VII_CODICE, 
VSE_SII_CODICE VES_SII_CODICE, VSE_NOC_CODICE VES_NOC_CODICE, VSE_ASS_CODICE VES_ASS_CODICE, VSE_LUOGO VES_LUOGO, VSE_PAZ_CODICE_OLD VES_PAZ_CODICE_OLD,
VSE_COMUNE_O_STATO VES_COMUNE_O_STATO, VSE_MED_VACCINANTE VES_MED_VACCINANTE, VSE_CNV_DATA VES_CNV_DATA, VSE_IN_CAMPAGNA VES_IN_CAMPAGNA, VSE_ESITO VES_ESITO,
VSE_OPE_IN_AMBULATORIO VES_OPE_IN_AMBULATORIO, VSE_NOTE VES_NOTE, VSE_CNS_REGISTRAZIONE VES_CNS_REGISTRAZIONE, VSE_ACCESSO VES_ACCESSO, VSE_AMB_CODICE VES_AMB_CODICE, VSE_ID VES_ID,
VSE_IMPORTO VES_IMPORTO, VSE_MAL_CODICE_MALATTIA VES_MAL_CODICE_MALATTIA, VSE_CODICE_ESENZIONE VES_CODICE_ESENZIONE, VSE_CNV_DATA_PRIMO_APP VES_CNV_DATA_PRIMO_APP, 
VSE_ASS_N_DOSE VES_ASS_N_DOSE, VSE_ASS_PROG VES_ASS_PROG, VSE_FLAG_FITTIZIA VES_FLAG_FITTIZIA, VSE_DATA_ULTIMA_VARIAZIONE VES_DATA_ULTIMA_VARIAZIONE, 
VSE_UTE_ID_ULTIMA_VARIAZIONE VES_UTE_ID_ULTIMA_VARIAZIONE, VSE_USL_INSERIMENTO VES_USL_INSERIMENTO, VSE_USL_SCADENZA VES_USL_SCADENZA, VSE_DATA_SCADENZA VES_DATA_SCADENZA, 
VSE_UTE_ID_SCADENZA VES_UTE_ID_SCADENZA, VSE_DATA_RIPRISTINO VES_DATA_RIPRISTINO, VSE_UTE_ID_RIPRISTINO VES_UTE_ID_RIPRISTINO, VSE_FLAG_VISIBILITA VES_FLAG_VISIBILITA, 
VSE_NOTE_ACQUISIZIONE VES_NOTE_ACQUISIZIONE, VSE_ID_ACN VES_ID_ACN, VSE_PROVENIENZA VES_PROVENIENZA,
VSE_RSC_CODICE VES_RSC_CODICE, VSE_MAL_CODICE_COND_SANITARIA VES_MAL_CODICE_COND_SANITARIA, VSE_TPA_GUID_TIPI_PAGAMENTO VES_TPA_GUID_TIPI_PAGAMENTO,
VSE_LOT_DATA_SCADENZA VES_LOT_DATA_SCADENZA, VSE_PAGATO VES_PAGATO, VSE_ANTIGENE VES_ANTIGENE, VSE_TIPO_EROGATORE VES_TIPO_EROGATORE, 
VSE_CODICE_STRUTTURA VES_CODICE_STRUTTURA, VSE_USL_COD_SOMMINISTRAZIONE VES_USL_COD_SOMMINISTRAZIONE, VSE_TIPO_ASSOCIAZIONE_ACN VES_TIPO_ASSOCIAZIONE_ACN 
from t_vac_scadute_eliminate where vse_id in ({0})
";
            }
        }

        /// <summary>
        /// Reazione Avversa  corrispondente al VRA_ID specificato
        /// </summary>
        public static string selReazioneAvversaById
        {
            get
            {
                return @"select * from T_VAC_REAZIONI_AVVERSE where VRA_ID = :VRA_ID";
            }
        }

        /// <summary>
        /// Reazione Avversa Scaduta corrispondente al VRS_ID specificato
        /// </summary>
        public static string selReazioneAvversaScadutaById
        {
            get
            {
                return @"select * from T_VAC_REAZIONI_SCADUTE where VRS_ID = :VRS_ID";
            }
        }

        /// <summary>
        /// Reazione Avversa Scaduta Eliminata corrispondente al VRL_ID specificato
        /// </summary>
        public static string selReazioneAvversaScadutaEliminataById
        {
            get
            {
                return @"select * from T_VAC_REAZIONI_SCAD_ELIMINATE where VRL_ID = :VRL_ID";
            }
        }

        /// <summary>
        /// Reazione Avversa Eliminata corrispondente al VRE_ID specificato
        /// </summary>
        public static string selReazioneAvversaEliminataById
        {
            get
            {
                return @"select * from T_VAC_REAZIONI_AVV_ELIMINATE where VRE_ID = :VRE_ID";
            }
        }

        /// <summary>
        /// Vaccinazione eseguita scaduta corrispondente a vsc_paz_codice, vsc_vac_codice, vsc_n_richiamo, vsc_data_effettuazione specificati
        /// </summary>
        public static string selVaccinazioneEseguitaScadutaPaziente
        {
            get
            {
                return @"select * from t_vac_scadute where vsc_paz_codice = :vsc_paz_codice and vsc_vac_codice=:vsc_vac_codice and vsc_n_richiamo=:vsc_n_richiamo and vsc_data_effettuazione=:vsc_data_effettuazione";
            }
        }
        
        /// <summary>
        /// Vaccinazione eseguita eliminata corrispondente al vee_id specificato
        /// </summary>
        public static string selVaccinazioneEseguitaEliminataById
        {
            get
            {
                return @"select * from t_vac_eseguite_eliminate where vee_id = :vee_id";
            }
        }

        /// <summary>
        /// Vaccinazione scaduta eliminata corrispondente al vse_id specificato
        /// </summary>
        public static string selVaccinazioneEseguitaScadutaEliminataById
        {
            get
            {
                return @"select * from t_vac_scadute_eliminate where vse_id = :vse_id";
            }
        }

		/// <summary>
		/// Vaccinazioni eseguite del paziente (con anche reazioni avverse)
		/// </summary>
		public static string selVaccinazioniEseguite
		{
			get
			{
                return @"SELECT paz_codice, paz_data_nascita, paz_data_decesso, paz_stato_anagrafico,
         ves_paz_codice_old, ves_cns_registrazione, ves_accesso,
         lot_data_scadenza, ves_dataora_effettuazione, ves_vii_codice,
         ves_in_campagna, ves_cnv_data, ves_cic_codice, ves_stato,
         ves_noc_codice, ves_ass_codice, ves_ass_n_dose, ves_data_registrazione, vac_ordine,
         ves_sii_codice, vra_rea_codice, vra_re1_codice, vra_re2_codice,
         t_ana_reazioni_avverse.rea_descrizione, reazioni1.rea_descrizione rea_descrizione1,
         reazioni2.rea_descrizione rea_descrizione2, vra_data_reazione, vra_visita,
         vra_esi_codice, vra_terapia, ves_vac_codice, vac_descrizione,
         ves_n_richiamo, ves_data_effettuazione, com_descrizione,
         ves_comune_o_stato, cns_descrizione, ves_cns_codice, ves_n_seduta,
         ves_lot_codice, resp.ope_nome, vac.ope_nome ope_nome1, ves_ope_codice,
         ves_med_vaccinante, sii_descrizione, vii_descrizione,
         noc_descrizione noc_descrizione, esi_descrizione, vac_obbligatoria, ves_luogo,
         ves_ute_id, ute_descrizione, ass_descrizione, ass_codice, ass_anti_influenzale,
         'N' scaduta, ves_ope_in_ambulatorio, ves_esito, ves_flag_fittizia, ves_note,
         vra_rea_altro, vra_gravita_reazione, vra_grave, vra_esito,
         vra_data_esito, vra_motivo_decesso, vra_dosaggio, vra_sospeso,
         vra_migliorata, vra_ripreso, vra_ricomparsa, vra_indicazioni,
         vra_richiamo, vra_luogo, vra_altro_luogo, vra_farmaco_concomitante,
         vra_farmaco_descrizione, vra_uso_concomitante,
         vra_condizioni_concomitanti, vra_qualifica, vra_altra_qualifica,
         vra_cognome_segnalatore, vra_nome_segnalatore,
         vra_indirizzo_segnalatore, vra_tel_segnalatore, vra_fax_segnalatore,
         vra_mail_segnalatore, vra_data_compilazione, ves_amb_codice, amb_descrizione, 
         vra_ute_id_compilazione, vra_data_variazione, vra_ute_id_variazione,
         ves_importo, ves_mal_codice_malattia, mal_descrizione, ves_codice_esenzione,
         ves_cnv_data_primo_app, ves_id, ves_ass_prog, ves_data_ultima_variazione, ves_ute_id_ultima_variazione,
         ves_usl_inserimento, t_ana_usl_inserimento_ves.usl_descrizione usl_inserimento_ves_descr,
         ves_usl_scadenza, ves_data_scadenza, ves_ute_id_scadenza, 
         ves_data_ripristino, ves_ute_id_ripristino, 
         ves_flag_visibilita, ves_note_acquisizione,
         vra_id, 
         vra_usl_inserimento, t_ana_usl_inserimento_vra.usl_descrizione usl_inserimento_vra_descr,
         vra_altre_informazioni, vra_ambito_osservazione, 
         vra_ambito_studio_titolo, vra_ambito_studio_tipologia, vra_ambito_studio_numero,
         vra_peso, vra_altezza, vra_data_ultima_mestruazione, 
         vra_allattamento, vra_gravidanza, vra_causa_osservata,
         vra_farmconc1_noc_descrizione, vra_farmconc1_lot_codice, vra_farmconc1_dataora_eff,
         vra_farmconc1_dose, vra_farmconc1_sii_codice, vra_farmconc1_vii_codice, 
         vra_farmconc1_sospeso, vra_farmconc1_migliorata, vra_farmconc1_ripreso,
         vra_farmconc1_ricomparsa, vra_farmconc1_indicazioni, vra_farmconc1_dosaggio, 
         vra_farmconc1_richiamo,
         vra_farmconc2_noc_descrizione, vra_farmconc2_lot_codice, vra_farmconc2_dataora_eff,
         vra_farmconc2_dose, vra_farmconc2_sii_codice, vra_farmconc2_vii_codice, 
         vra_farmconc2_sospeso, vra_farmconc2_migliorata, vra_farmconc2_ripreso,
         vra_farmconc2_ricomparsa, vra_farmconc2_indicazioni, vra_farmconc2_dosaggio, 
         vra_farmconc2_richiamo, vra_firma_segnalatore, vra_oet_codice, vra_lot_data_scadenza, vra_farmconc1_lot_data_scad, vra_farmconc2_lot_data_scad, vra_noi_codice_indicazioni, vra_farmconc1_noi_cod_indic, vra_farmconc2_noi_cod_indic, vra_id_scheda,
         vra_farmconc1_noc_codice, vra_farmconc2_noc_codice,vra_segnalazione_id, ves_id_acn, ves_provenienza, ves_mal_codice_cond_sanitaria, ves_rsc_codice, ves_tpa_guid_tipi_pagamento,
		 ves_lot_data_scadenza, ves_antigene, ves_tipo_erogatore, ves_codice_struttura, ast_descrizione, ves_usl_cod_somministrazione, ves_tipo_associazione_acn, vra_ute_id_invio, vra_data_invio, vra_flag_inviato
	FROM t_paz_pazienti, t_vac_eseguite, t_ana_malattie,
         t_vac_reazioni_avverse, t_ana_lotti, t_ana_esiti_terapie,
         t_ana_reazioni_avverse, t_ana_reazioni_avverse reazioni1, t_ana_reazioni_avverse reazioni2,
         t_ana_vaccinazioni, t_ana_consultori, t_ana_nomi_commerciali ,
         t_ana_siti_inoculazione, t_ana_vie_somministrazione,
         t_ana_operatori resp, t_ana_operatori vac,
         t_ana_utenti, t_ana_associazioni,
         t_ana_comuni, t_ana_ambulatori,
         t_ana_usl t_ana_usl_inserimento_ves,
         t_ana_usl t_ana_usl_inserimento_vra,
         v_ana_strutture
   WHERE paz_codice = :paz_codice
     {0}
     AND paz_codice = ves_paz_codice
     AND ves_paz_codice = :paz_codice
     AND ves_mal_codice_malattia = mal_codice(+)
     AND ves_paz_codice = vra_paz_codice(+)
     AND ves_vac_codice = vra_vac_codice(+)
     AND ves_n_richiamo = vra_n_richiamo(+)
     AND ves_data_effettuazione = vra_res_data_effettuazione(+)
     AND ves_lot_codice = lot_codice(+)
     AND ute_id(+) = ves_ute_id
     AND ass_codice(+) = ves_ass_codice
     AND vac_codice = ves_vac_codice
     AND cns_codice(+) = ves_cns_codice
     AND resp.ope_codice(+) = ves_ope_codice
     AND vac.ope_codice(+) = ves_med_vaccinante
     AND sii_codice(+) = ves_sii_codice
     AND vii_codice(+) = ves_vii_codice
     AND noc_codice(+) = ves_noc_codice
     AND vra_rea_codice = t_ana_reazioni_avverse.rea_codice(+)
     AND ves_comune_o_stato = com_codice(+)
     AND vra_re1_codice = reazioni1.rea_codice(+)
     AND vra_re2_codice = reazioni2.rea_codice(+)
     AND vra_esi_codice = esi_codice(+)
     AND ves_amb_codice = amb_codice(+)
     AND ves_usl_inserimento = t_ana_usl_inserimento_ves.usl_codice(+)
     AND vra_usl_inserimento = t_ana_usl_inserimento_vra.usl_codice(+)
     and ves_codice_struttura = v_ana_strutture.ast_codice(+)
UNION
SELECT   paz_codice, paz_data_nascita, paz_data_decesso, paz_stato_anagrafico,
         vsc_paz_codice_old, vsc_cns_registrazione, vsc_accesso,
         lot_data_scadenza, vsc_dataora_effettuazione, vsc_vii_codice,
         vsc_in_campagna, vsc_cnv_data, vsc_cic_codice, vsc_stato,
         vsc_noc_codice, vsc_ass_codice, vsc_ass_n_dose, vsc_data_registrazione, vac_ordine,
         vsc_sii_codice, vrs_rea_codice, vrs_re1_codice, vrs_re2_codice,
         t_ana_reazioni_avverse.rea_descrizione, reazioni1.rea_descrizione rea_descrizione1,
         reazioni2.rea_descrizione rea_descrizione2, vrs_data_reazione, vrs_visita,
         vrs_esi_codice, vrs_terapia, vsc_vac_codice, vac_descrizione,
         vsc_n_richiamo, vsc_data_effettuazione, com_descrizione,
         vsc_comune_o_stato, cns_descrizione, vsc_cns_codice, vsc_n_seduta,
         vsc_lot_codice, resp.ope_nome, vac.ope_nome ope_nome1,
         vsc_ope_codice, vsc_med_vaccinante, sii_descrizione, vii_descrizione,
         noc_descrizione, esi_descrizione, vac_obbligatoria, vsc_luogo,
         vsc_ute_id, ute_descrizione, ass_descrizione, ass_codice, ass_anti_influenzale,
         'S' scaduta, vsc_ope_in_ambulatorio, vsc_esito, vsc_flag_fittizia, vsc_note,
         vrs_rea_altro, vrs_gravita_reazione, vrs_grave, vrs_esito,
         vrs_data_esito, vrs_motivo_decesso, vrs_dosaggio, vrs_sospeso,
         vrs_migliorata, vrs_ripreso, vrs_ricomparsa, vrs_indicazioni,
         vrs_richiamo, vrs_luogo, vrs_altro_luogo, vrs_farmaco_concomitante,
         vrs_farmaco_descrizione, vrs_uso_concomitante,
         vrs_condizioni_concomitanti, vrs_qualifica, vrs_altra_qualifica,
         vrs_cognome_segnalatore, vrs_nome_segnalatore,
         vrs_indirizzo_segnalatore, vrs_tel_segnalatore, vrs_fax_segnalatore,
         vrs_mail_segnalatore, vrs_data_compilazione, vsc_amb_codice, amb_descrizione,  
         vrs_ute_id_compilazione, vrs_data_variazione, vrs_ute_id_variazione,
         vsc_importo, vsc_mal_codice_malattia, mal_descrizione, vsc_codice_esenzione,
         vsc_cnv_data_primo_app, vsc_id, vsc_ass_prog, vsc_data_ultima_variazione, vsc_ute_id_ultima_variazione,
         vsc_usl_inserimento ves_usl_inserimento, 
         t_ana_usl_inserimento_ves.usl_descrizione usl_inserimento_ves_descr,
         vsc_usl_scadenza ves_usl_scadenza, vsc_data_scadenza ves_data_scadenza, vsc_ute_id_scadenza ves_ute_id_scadenza, 
         vsc_data_ripristino ves_data_ripristino, vsc_ute_id_ripristino ves_ute_id_ripristino, 
         vsc_flag_visibilita ves_flag_visibilita, vsc_note_acquisizione ves_note_acquisizione,
         vrs_id vra_id,
         vrs_usl_inserimento vra_usl_inserimento, t_ana_usl_inserimento_vra.usl_descrizione usl_inserimento_vra_descr,
         vrs_altre_informazioni vra_altre_informazioni, vrs_ambito_osservazione vra_ambito_osservazione, 
         vrs_ambito_studio_titolo vra_ambito_studio_titolo, vrs_ambito_studio_tipologia vra_ambito_studio_tipologia, vrs_ambito_studio_numero vra_ambito_studio_numero,
         vrs_peso vra_peso, vrs_altezza vra_altezza, vrs_data_ultima_mestruazione vra_data_ultima_mestruazione, 
         vrs_allattamento vra_allattamento, vrs_gravidanza vra_gravidanza, vrs_causa_osservata vra_causa_osservata,
         vrs_farmconc1_noc_descrizione vra_farmconc1_noc_descrizione, vrs_farmconc1_lot_codice vra_farmconc1_lot_codice, vrs_farmconc1_dataora_eff vra_farmconc1_dataora_eff,
         vrs_farmconc1_dose vra_farmconc1_dose, vrs_farmconc1_sii_codice vra_farmconc1_sii_codice, vrs_farmconc1_vii_codice vra_farmconc1_vii_codice, 
         vrs_farmconc1_sospeso vra_farmconc1_sospeso, vrs_farmconc1_migliorata vra_farmconc1_migliorata, vrs_farmconc1_ripreso vra_farmconc1_ripreso,
         vrs_farmconc1_ricomparsa vra_farmconc1_ricomparsa, vrs_farmconc1_indicazioni vra_farmconc1_indicazioni, vrs_farmconc1_dosaggio vra_farmconc1_dosaggio, 
         vrs_farmconc1_richiamo vra_farmconc1_richiamo,
         vrs_farmconc2_noc_descrizione vra_farmconc2_noc_descrizione, vrs_farmconc2_lot_codice vra_farmconc2_lot_codice, vrs_farmconc2_dataora_eff vra_farmconc2_dataora_eff,
         vrs_farmconc2_dose vra_farmconc2_dose, vrs_farmconc2_sii_codice vra_farmconc2_sii_codice, vrs_farmconc2_vii_codice vra_farmconc2_vii_codice, 
         vrs_farmconc2_sospeso vra_farmconc2_sospeso, vrs_farmconc2_migliorata vra_farmconc2_migliorata, vrs_farmconc2_ripreso vra_farmconc2_ripreso,
         vrs_farmconc2_ricomparsa vra_farmconc2_ricomparsa, vrs_farmconc2_indicazioni vra_farmconc2_indicazioni, vrs_farmconc2_dosaggio vra_farmconc2_dosaggio, 
         vrs_farmconc2_richiamo vra_farmconc2_richiamo, vrs_firma_segnalatore vra_firma_segnalatore, vrs_oet_codice vra_oet_codice, VRS_LOT_DATA_SCADENZA vra_lot_data_scadenza, VRS_FARMCONC1_LOT_DATA_SCAD vra_farmconc1_lot_data_scad, VRS_FARMCONC2_LOT_DATA_SCAD vra_farmconc2_lot_data_scad, vrs_noi_codice_indicazioni vra_noi_codice_indicazioni, vrs_farmconc1_noi_cod_indic vra_farmconc1_noi_cod_indic, vrs_farmconc2_noi_cod_indic vra_farmconc2_noi_cod_indic, vrs_id_scheda vra_id_scheda,
         vrs_farmconc1_noc_codice vra_farmconc1_noc_codice, vrs_farmconc2_noc_codice vra_farmconc2_noc_codice, vrs_segnalazione_id vra_segnalazione_id, vsc_id_acn ves_id_acn, vsc_provenienza ves_provenienza, vsc_mal_codice_cond_sanitaria, vsc_rsc_codice, vsc_tpa_guid_tipi_pagamento,
		 vsc_lot_data_scadenza, vsc_antigene, vsc_tipo_erogatore, vsc_codice_struttura, ast_descrizione, vsc_usl_cod_somministrazione,vsc_tipo_associazione_acn, vrs_ute_id_invio vra_ute_id_invio, vrs_data_invio vra_data_invio, vrs_flag_inviato vra_flag_inviato
    FROM t_paz_pazienti, t_vac_scadute, t_ana_malattie,
         t_vac_reazioni_scadute, t_ana_lotti, t_ana_esiti_terapie,
         t_ana_reazioni_avverse, t_ana_reazioni_avverse reazioni1, t_ana_reazioni_avverse reazioni2,
         t_ana_vaccinazioni, t_ana_consultori, t_ana_nomi_commerciali,
         t_ana_siti_inoculazione, t_ana_vie_somministrazione,
         t_ana_operatori resp, t_ana_operatori vac,
         t_ana_utenti, t_ana_associazioni,
         t_ana_comuni, t_ana_ambulatori,
         t_ana_usl t_ana_usl_inserimento_ves,
         t_ana_usl t_ana_usl_inserimento_vra,
         v_ana_strutture
   WHERE paz_codice = :paz_codice
     {1}
     AND paz_codice = vsc_paz_codice
     AND vsc_paz_codice = :paz_codice
     AND vsc_mal_codice_malattia = mal_codice(+)
     AND vsc_paz_codice = vrs_paz_codice(+)
     AND vsc_vac_codice = vrs_vac_codice(+)
     AND vsc_n_richiamo = vrs_n_richiamo(+)
     AND vsc_data_effettuazione = vrs_res_data_effettuazione(+)
     AND vsc_lot_codice = lot_codice(+)
     AND ute_id(+) = vsc_ute_id
     AND ass_codice(+) = vsc_ass_codice
     AND vac_codice = vsc_vac_codice
     AND cns_codice(+) = vsc_cns_codice
     AND resp.ope_codice(+) = vsc_ope_codice
     AND vac.ope_codice(+) = vsc_med_vaccinante
     AND sii_codice(+) = vsc_sii_codice
     AND vii_codice(+) = vsc_vii_codice
     AND noc_codice(+) = vsc_noc_codice
     AND vrs_rea_codice = t_ana_reazioni_avverse.rea_codice(+)
     AND vrs_re1_codice = reazioni1.rea_codice(+)
     AND vrs_re2_codice = reazioni2.rea_codice(+)
     AND vrs_esi_codice = esi_codice(+)
     AND vsc_comune_o_stato = com_codice(+)
     AND vsc_amb_codice = amb_codice(+)
     AND vsc_usl_inserimento = t_ana_usl_inserimento_ves.usl_codice(+)
     AND vrs_usl_inserimento = t_ana_usl_inserimento_vra.usl_codice(+)
     and vsc_codice_struttura = v_ana_strutture.ast_codice(+)
ORDER BY vac_obbligatoria, vac_ordine, ves_data_effettuazione, ves_ass_n_dose";
            }
        }


        /// <summary>
        /// Vaccinazioni eseguite della reazioni da integrare  (con anche reazioni avverse)
        /// </summary>
        public static string selVaccinazioniEseguiteIntegrazione
        {
            get
            {
                return @"SELECT paz_codice, paz_data_nascita, paz_data_decesso, paz_stato_anagrafico,
         ves_paz_codice_old, ves_cns_registrazione, ves_accesso,
         lot_data_scadenza, ves_dataora_effettuazione, ves_vii_codice,
         ves_in_campagna, ves_cnv_data, ves_cic_codice, ves_stato,
         ves_noc_codice, ves_ass_codice, ves_ass_n_dose, ves_data_registrazione, vac_ordine,
         ves_sii_codice, vra_rea_codice, vra_re1_codice, vra_re2_codice,
         t_ana_reazioni_avverse.rea_descrizione, reazioni1.rea_descrizione rea_descrizione1,
         reazioni2.rea_descrizione rea_descrizione2, vra_data_reazione, vra_visita,
         vra_esi_codice, vra_terapia, ves_vac_codice, vac_descrizione,
         ves_n_richiamo, ves_data_effettuazione, com_descrizione,
         ves_comune_o_stato, cns_descrizione, ves_cns_codice, ves_n_seduta,
         ves_lot_codice, resp.ope_nome, vac.ope_nome ope_nome1, ves_ope_codice,
         ves_med_vaccinante, sii_descrizione, somministrazioni.vii_descrizione vii_descrizione,
         commerciali.noc_descrizione noc_descrizione, esi_descrizione, vac_obbligatoria, ves_luogo,
         ves_ute_id, ute_descrizione, ass_descrizione, ass_codice,
         'N' scaduta, ves_ope_in_ambulatorio, ves_esito, ves_flag_fittizia, ves_note,
         vra_rea_altro, vra_gravita_reazione, vra_grave, vra_esito,
         vra_data_esito, vra_motivo_decesso, vra_dosaggio, vra_sospeso,
         vra_migliorata, vra_ripreso, vra_ricomparsa, vra_indicazioni,
         vra_richiamo, vra_luogo, vra_altro_luogo, vra_farmaco_concomitante,
         vra_farmaco_descrizione, vra_uso_concomitante,
         vra_condizioni_concomitanti, vra_qualifica, vra_altra_qualifica,
         vra_cognome_segnalatore, vra_nome_segnalatore,
         vra_indirizzo_segnalatore, vra_tel_segnalatore, vra_fax_segnalatore,
         vra_mail_segnalatore, vra_data_compilazione, ves_amb_codice, amb_descrizione, 
         vra_ute_id_compilazione, vra_data_variazione, vra_ute_id_variazione,
         ves_importo, ves_mal_codice_malattia, mal_descrizione, ves_codice_esenzione,
         ves_cnv_data_primo_app,ves_id, ves_ass_prog, ves_data_ultima_variazione, ves_ute_id_ultima_variazione,
         ves_usl_inserimento, t_ana_usl_inserimento_ves.usl_descrizione usl_inserimento_ves_descr,
         ves_usl_scadenza, ves_data_scadenza, ves_ute_id_scadenza, 
         ves_data_ripristino, ves_ute_id_ripristino, 
         ves_flag_visibilita, ves_note_acquisizione,
         vra_id, 
         vra_usl_inserimento, t_ana_usl_inserimento_vra.usl_descrizione usl_inserimento_vra_descr,
         vra_altre_informazioni, vra_ambito_osservazione, 
         vra_ambito_studio_titolo, vra_ambito_studio_tipologia, vra_ambito_studio_numero,
         vra_peso, vra_altezza, vra_data_ultima_mestruazione, 
         vra_allattamento, vra_gravidanza, vra_causa_osservata,
         vra_farmconc1_noc_descrizione, vra_farmconc1_lot_codice, vra_farmconc1_dataora_eff,
         vra_farmconc1_dose, vra_farmconc1_sii_codice, vra_farmconc1_vii_codice, 
         vra_farmconc1_sospeso, vra_farmconc1_migliorata, vra_farmconc1_ripreso,
         vra_farmconc1_ricomparsa, vra_farmconc1_indicazioni, vra_farmconc1_dosaggio, 
         vra_farmconc1_richiamo,
         vra_farmconc2_noc_descrizione, vra_farmconc2_lot_codice, vra_farmconc2_dataora_eff,
         vra_farmconc2_dose, vra_farmconc2_sii_codice, vra_farmconc2_vii_codice, 
         vra_farmconc2_sospeso, vra_farmconc2_migliorata, vra_farmconc2_ripreso,
         vra_farmconc2_ricomparsa, vra_farmconc2_indicazioni, vra_farmconc2_dosaggio, 
         vra_farmconc2_richiamo, vra_firma_segnalatore, vra_oet_codice, vra_lot_data_scadenza, vra_farmconc1_lot_data_scad, vra_farmconc2_lot_data_scad, vra_noi_codice_indicazioni, vra_farmconc1_noi_cod_indic, vra_farmconc2_noi_cod_indic, vra_id_scheda,
         vra_farmconc1_noc_codice, vra_farmconc2_noc_codice, commerciali.noc_codice_aic noc_codice_aic, commerciali1.noc_codice_aic farmconc1_noc_codice_aic, commerciali2.noc_codice_aic farmconc2_noc_codice_aic,
         commerciali.noc_codice_atc noc_codice_atc, commerciali1.noc_codice_atc farmconc1_noc_codice_atc, commerciali2.noc_codice_aic farmconc2_noc_codice_atc,
         commerciali.noc_forma_farmaceutica noc_forma_farmaceutica, commerciali1.noc_forma_farmaceutica farmconc1_noc_forma_farmac, commerciali2.noc_forma_farmaceutica farmconc2_noc_forma_farmac,
         somministrazioni.vii_codice_esterno vii_codice_esterno,somministrazioni1.vii_codice_esterno farmconc1_vii_codice_esterno,somministrazioni2.vii_codice_esterno farmconc2_vii_codice_esterno,
         indicazioni.noi_codice_esterno noi_codice_esterno,indicazioni1.noi_codice_esterno farmconc1_noi_codice_esterno,indicazioni2.noi_codice_esterno farmconc2_noi_codice_esterno,
         indicazioni.noi_descrizione noi_descrizione,indicazioni1.noi_descrizione farmconc1_noi_descr,indicazioni2.noi_descrizione farmconc2_noi_descr,
         mgr_codice_esterno,mer_codice_esterno,mmr_codice_esterno,mqr_codice_esterno,paz_nome,paz_cognome,paz_sesso,oet_codice_esterno,mcr_codice_esterno, vra_segnalazione_id, ves_id_acn, ves_provenienza, vra_ute_id_invio, vra_data_invio, vra_flag_inviato
    FROM t_paz_pazienti, t_vac_eseguite, t_ana_malattie,
         t_vac_reazioni_avverse, t_ana_lotti, t_ana_esiti_terapie,
         t_ana_reazioni_avverse, t_ana_reazioni_avverse reazioni1, t_ana_reazioni_avverse reazioni2,
         t_ana_vaccinazioni, t_ana_consultori, t_ana_nomi_commerciali commerciali,
         t_ana_siti_inoculazione, t_ana_vie_somministrazione somministrazioni,
         t_ana_operatori resp, t_ana_operatori vac,
         t_ana_utenti, t_ana_associazioni,
         t_ana_comuni, t_ana_ambulatori,
         t_ana_usl t_ana_usl_inserimento_ves,
         t_ana_usl t_ana_usl_inserimento_vra,
         t_ana_nomi_commerciali commerciali1, t_ana_nomi_commerciali commerciali2,
         t_ana_vie_somministrazione somministrazioni1,t_ana_vie_somministrazione somministrazioni2,
         t_ana_nomi_com_indicazioni indicazioni, t_ana_nomi_com_indicazioni indicazioni1, t_ana_nomi_com_indicazioni indicazioni2,
         t_map_gravita_rea,t_map_esito_rea,t_map_motivo_decesso_rea,t_map_qualifica_rea,t_ana_origine_etnica,t_map_causa_rea
   WHERE vra_id in ({0})
     AND paz_codice = ves_paz_codice
     AND ves_mal_codice_malattia = mal_codice(+)
     AND ves_paz_codice = vra_paz_codice(+)
     AND ves_vac_codice = vra_vac_codice(+)
     AND ves_n_richiamo = vra_n_richiamo(+)
     AND ves_data_effettuazione = vra_res_data_effettuazione(+)
     AND ves_lot_codice = lot_codice(+)
     AND ute_id(+) = ves_ute_id
     AND ass_codice(+) = ves_ass_codice
     AND vac_codice = ves_vac_codice
     AND cns_codice(+) = ves_cns_codice
     AND resp.ope_codice(+) = ves_ope_codice
     AND vac.ope_codice(+) = ves_med_vaccinante
     AND sii_codice(+) = ves_sii_codice
     AND somministrazioni.vii_codice(+) = ves_vii_codice
     AND commerciali.noc_codice(+) = ves_noc_codice
     AND vra_rea_codice = t_ana_reazioni_avverse.rea_codice(+)
     AND ves_comune_o_stato = com_codice(+)
     AND vra_re1_codice = reazioni1.rea_codice(+)
     AND vra_re2_codice = reazioni2.rea_codice(+)
     AND vra_esi_codice = esi_codice(+)
     AND ves_amb_codice = amb_codice(+)
     AND ves_usl_inserimento = t_ana_usl_inserimento_ves.usl_codice(+)
     AND vra_usl_inserimento = t_ana_usl_inserimento_vra.usl_codice(+)
     AND commerciali1.noc_codice(+) = vra_farmconc1_noc_codice
     AND commerciali2.noc_codice(+) = vra_farmconc2_noc_codice
     AND somministrazioni1.vii_codice(+) = vra_farmconc1_vii_codice
     AND somministrazioni2.vii_codice(+) = vra_farmconc2_vii_codice
     AND indicazioni.noi_codice(+) = vra_noi_codice_indicazioni
     AND indicazioni1.noi_codice(+) = vra_farmconc1_noi_cod_indic
     AND indicazioni2.noi_codice(+) = vra_farmconc2_noi_cod_indic
     AND mgr_codice(+) = vra_gravita_reazione
     AND mer_codice(+) = vra_esito
     AND mmr_codice(+) = vra_motivo_decesso
     AND mqr_codice(+) = vra_qualifica
     AND oet_codice(+) = vra_oet_codice
     AND mcr_codice(+) = vra_causa_osservata
UNION
SELECT   paz_codice,paz_data_nascita, paz_data_decesso, paz_stato_anagrafico,
         vsc_paz_codice_old, vsc_cns_registrazione, vsc_accesso,
         lot_data_scadenza, vsc_dataora_effettuazione, vsc_vii_codice,
         vsc_in_campagna, vsc_cnv_data, vsc_cic_codice, vsc_stato,
         vsc_noc_codice, vsc_ass_codice, vsc_ass_n_dose, vsc_data_registrazione, vac_ordine,
         vsc_sii_codice, vrs_rea_codice, vrs_re1_codice, vrs_re2_codice,
         t_ana_reazioni_avverse.rea_descrizione, reazioni1.rea_descrizione rea_descrizione1,
         reazioni2.rea_descrizione rea_descrizione2, vrs_data_reazione, vrs_visita,
         vrs_esi_codice, vrs_terapia, vsc_vac_codice, vac_descrizione,
         vsc_n_richiamo, vsc_data_effettuazione, com_descrizione,
         vsc_comune_o_stato, cns_descrizione, vsc_cns_codice, vsc_n_seduta,
         vsc_lot_codice, resp.ope_nome, vac.ope_nome ope_nome1,
         vsc_ope_codice, vsc_med_vaccinante, sii_descrizione, somministrazioni.vii_descrizione vii_descrizione,
         commerciali.noc_descrizione noc_descrizione, esi_descrizione, vac_obbligatoria, vsc_luogo,
         vsc_ute_id, ute_descrizione, ass_descrizione, ass_codice,
         'S' scaduta, vsc_ope_in_ambulatorio, vsc_esito, vsc_flag_fittizia, vsc_note,
         vrs_rea_altro, vrs_gravita_reazione, vrs_grave, vrs_esito,
         vrs_data_esito, vrs_motivo_decesso, vrs_dosaggio, vrs_sospeso,
         vrs_migliorata, vrs_ripreso, vrs_ricomparsa, vrs_indicazioni,
         vrs_richiamo, vrs_luogo, vrs_altro_luogo, vrs_farmaco_concomitante,
         vrs_farmaco_descrizione, vrs_uso_concomitante vra_uso_concomitante,
         vrs_condizioni_concomitanti, vrs_qualifica, vrs_altra_qualifica,
         vrs_cognome_segnalatore, vrs_nome_segnalatore,
         vrs_indirizzo_segnalatore, vrs_tel_segnalatore, vrs_fax_segnalatore,
         vrs_mail_segnalatore, vrs_data_compilazione, vsc_amb_codice, amb_descrizione,  
         vrs_ute_id_compilazione, vrs_data_variazione, vrs_ute_id_variazione,
         vsc_importo, vsc_mal_codice_malattia, mal_descrizione, vsc_codice_esenzione,
         vsc_cnv_data_primo_app,vsc_id, vsc_ass_prog, vsc_data_ultima_variazione, vsc_ute_id_ultima_variazione,
         vsc_usl_inserimento ves_usl_inserimento, 
         t_ana_usl_inserimento_ves.usl_descrizione usl_inserimento_ves_descr,
         vsc_usl_scadenza ves_usl_scadenza, vsc_data_scadenza ves_data_scadenza, vsc_ute_id_scadenza ves_ute_id_scadenza, 
         vsc_data_ripristino ves_data_ripristino, vsc_ute_id_ripristino ves_ute_id_ripristino, 
         vsc_flag_visibilita ves_flag_visibilita, vsc_note_acquisizione ves_note_acquisizione,
         vrs_id vra_id,
         vrs_usl_inserimento vra_usl_inserimento, t_ana_usl_inserimento_vra.usl_descrizione usl_inserimento_vra_descr,
         vrs_altre_informazioni vra_altre_informazioni, vrs_ambito_osservazione vra_ambito_osservazione, 
         vrs_ambito_studio_titolo vra_ambito_studio_titolo, vrs_ambito_studio_tipologia vra_ambito_studio_tipologia, vrs_ambito_studio_numero vra_ambito_studio_numero,
         vrs_peso vra_peso, vrs_altezza vra_altezza, vrs_data_ultima_mestruazione vra_data_ultima_mestruazione, 
         vrs_allattamento vra_allattamento, vrs_gravidanza vra_gravidanza, vrs_causa_osservata vra_causa_osservata,
         vrs_farmconc1_noc_descrizione vra_farmconc1_noc_descrizione, vrs_farmconc1_lot_codice vra_farmconc1_lot_codice, vrs_farmconc1_dataora_eff vra_farmconc1_dataora_eff,
         vrs_farmconc1_dose vra_farmconc1_dose, vrs_farmconc1_sii_codice vra_farmconc1_sii_codice, vrs_farmconc1_vii_codice vra_farmconc1_vii_codice, 
         vrs_farmconc1_sospeso vra_farmconc1_sospeso, vrs_farmconc1_migliorata vra_farmconc1_migliorata, vrs_farmconc1_ripreso vra_farmconc1_ripreso,
         vrs_farmconc1_ricomparsa vra_farmconc1_ricomparsa, vrs_farmconc1_indicazioni vra_farmconc1_indicazioni, vrs_farmconc1_dosaggio vra_farmconc1_dosaggio, 
         vrs_farmconc1_richiamo vra_farmconc1_richiamo,
         vrs_farmconc2_noc_descrizione vra_farmconc2_noc_descrizione, vrs_farmconc2_lot_codice vra_farmconc2_lot_codice, vrs_farmconc2_dataora_eff vra_farmconc2_dataora_eff,
         vrs_farmconc2_dose vra_farmconc2_dose, vrs_farmconc2_sii_codice vra_farmconc2_sii_codice, vrs_farmconc2_vii_codice vra_farmconc2_vii_codice, 
         vrs_farmconc2_sospeso vra_farmconc2_sospeso, vrs_farmconc2_migliorata vra_farmconc2_migliorata, vrs_farmconc2_ripreso vra_farmconc2_ripreso,
         vrs_farmconc2_ricomparsa vra_farmconc2_ricomparsa, vrs_farmconc2_indicazioni vra_farmconc2_indicazioni, vrs_farmconc2_dosaggio vra_farmconc2_dosaggio, 
         vrs_farmconc2_richiamo vra_farmconc2_richiamo, vrs_firma_segnalatore vra_firma_segnalatore, vrs_oet_codice vra_oet_codice, VRS_LOT_DATA_SCADENZA vra_lot_data_scadenza, VRS_FARMCONC1_LOT_DATA_SCAD vra_farmconc1_lot_data_scad, VRS_FARMCONC2_LOT_DATA_SCAD vra_farmconc2_lot_data_scad, vrs_noi_codice_indicazioni vra_noi_codice_indicazioni, vrs_farmconc1_noi_cod_indic vra_farmconc1_noi_cod_indic, vrs_farmconc2_noi_cod_indic vra_farmconc2_noi_cod_indic, vrs_id_scheda vra_id_scheda,
         vrs_farmconc1_noc_codice, vrs_farmconc2_noc_codice, commerciali.noc_codice_aic noc_codice_aic, commerciali1.noc_codice_aic farmconc1_noc_codice_aic, commerciali2.noc_codice_aic farmconc2_noc_codice_aic,
         commerciali.noc_codice_atc noc_codice_atc, commerciali1.noc_codice_atc farmconc1_noc_codice_atc, commerciali2.noc_codice_aic farmconc2_noc_codice_atc,
         commerciali.noc_forma_farmaceutica noc_forma_farmaceutica, commerciali1.noc_forma_farmaceutica farmconc1_noc_forma_farmac, commerciali2.noc_forma_farmaceutica farmconc2_noc_forma_farmac,
         somministrazioni.vii_codice_esterno vii_codice_esterno,somministrazioni1.vii_codice_esterno farmconc1_vii_codice_esterno,somministrazioni2.vii_codice_esterno farmconc2_vii_codice_esterno,
         indicazioni.noi_codice_esterno noi_codice_esterno,indicazioni1.noi_codice_esterno farmconc1_noi_codice_esterno,indicazioni2.noi_codice_esterno farmconc2_noi_codice_esterno,
         indicazioni.noi_descrizione noi_descrizione,indicazioni1.noi_descrizione farmconc1_noi_descr,indicazioni2.noi_descrizione farmconc2_noi_descr,
         mgr_codice_esterno,mer_codice_esterno,mmr_codice_esterno,mqr_codice_esterno,paz_nome,paz_cognome,paz_sesso,oet_codice_esterno,mcr_codice_esterno, vrs_segnalazione_id vra_segnalazione_id, vsc_id_acn ves_id_acn, vsc_provenienza ves_provenienza, vrs_ute_id_invio vra_ute_id_invio, vrs_data_invio vra_data_invio, vrs_flag_inviato vra_flag_inviato 
    FROM t_paz_pazienti, t_vac_scadute, t_ana_malattie,
         t_vac_reazioni_scadute, t_ana_lotti, t_ana_esiti_terapie,
         t_ana_reazioni_avverse, t_ana_reazioni_avverse reazioni1, t_ana_reazioni_avverse reazioni2,
         t_ana_vaccinazioni, t_ana_consultori, t_ana_nomi_commerciali commerciali,
         t_ana_siti_inoculazione, t_ana_vie_somministrazione somministrazioni,
         t_ana_operatori resp, t_ana_operatori vac,
         t_ana_utenti, t_ana_associazioni,
         t_ana_comuni, t_ana_ambulatori,
         t_ana_usl t_ana_usl_inserimento_ves,
         t_ana_usl t_ana_usl_inserimento_vra,
         t_ana_nomi_commerciali commerciali1, t_ana_nomi_commerciali commerciali2,
         t_ana_vie_somministrazione somministrazioni1,t_ana_vie_somministrazione somministrazioni2,
         t_ana_nomi_com_indicazioni indicazioni, t_ana_nomi_com_indicazioni indicazioni1, t_ana_nomi_com_indicazioni indicazioni2,
         t_map_gravita_rea,t_map_esito_rea,t_map_motivo_decesso_rea,t_map_qualifica_rea,t_ana_origine_etnica,t_map_causa_rea
   WHERE vrs_id in ({0}) 
     AND paz_codice = vsc_paz_codice
     AND vsc_mal_codice_malattia = mal_codice(+)
     AND vsc_paz_codice = vrs_paz_codice(+)
     AND vsc_vac_codice = vrs_vac_codice(+)
     AND vsc_n_richiamo = vrs_n_richiamo(+)
     AND vsc_data_effettuazione = vrs_res_data_effettuazione(+)
     AND vsc_lot_codice = lot_codice(+)
     AND ute_id(+) = vsc_ute_id
     AND ass_codice(+) = vsc_ass_codice
     AND vac_codice = vsc_vac_codice
     AND cns_codice(+) = vsc_cns_codice
     AND resp.ope_codice(+) = vsc_ope_codice
     AND vac.ope_codice(+) = vsc_med_vaccinante
     AND sii_codice(+) = vsc_sii_codice
     AND somministrazioni.vii_codice(+) = vsc_vii_codice
     AND commerciali.noc_codice(+) = vsc_noc_codice
     AND vrs_rea_codice = t_ana_reazioni_avverse.rea_codice(+)
     AND vrs_re1_codice = reazioni1.rea_codice(+)
     AND vrs_re2_codice = reazioni2.rea_codice(+)
     AND vrs_esi_codice = esi_codice(+)
     AND vsc_comune_o_stato = com_codice(+)
     AND vsc_amb_codice = amb_codice(+)
     AND vsc_usl_inserimento = t_ana_usl_inserimento_ves.usl_codice(+)
     AND vrs_usl_inserimento = t_ana_usl_inserimento_vra.usl_codice(+)
     AND commerciali1.noc_codice(+) = vrs_farmconc1_noc_codice
     AND commerciali2.noc_codice(+) = vrs_farmconc2_noc_codice
     AND somministrazioni1.vii_codice(+) = vrs_farmconc1_vii_codice
     AND somministrazioni2.vii_codice(+) = vrs_farmconc2_vii_codice
     AND indicazioni.noi_codice(+) = vrs_noi_codice_indicazioni
     AND indicazioni1.noi_codice(+) = vrs_farmconc1_noi_cod_indic
     AND indicazioni2.noi_codice(+) = vrs_farmconc2_noi_cod_indic
     AND mgr_codice(+) = vrs_gravita_reazione
     AND mer_codice(+) = vrs_esito
     AND mmr_codice(+) = vrs_motivo_decesso
     AND mqr_codice(+) = vrs_qualifica
     AND oet_codice(+) = vrs_oet_codice
     AND mcr_codice(+) = vrs_causa_osservata
ORDER BY vac_obbligatoria, vac_ordine, ves_data_effettuazione, ves_ass_n_dose";
            }
        }

        /// <summary>
        /// Max richiamo per il  paziente e la vaccinazione
        /// </summary>
        public static string selMaxRichiamo
        {
            get
            {
                return @"select nvl(max(ves_n_richiamo), 0)
from t_vac_eseguite
where ves_vac_codice = :cod_vac
and ves_paz_codice = :cod_paz";
            }
        }

        /// <summary>
        /// Query di selezione della dose successiva di una associazione
        /// </summary>
        public static string selMaxDoseAssociazione
        {
            get
            {
                return @"SELECT  nvl(max(ves_ass_n_dose),0) as ves_ass_n_dose
                          FROM   t_vac_eseguite
                         WHERE   ves_paz_codice = :paz_cod
                           AND   ves_ass_codice = :ass_codice";
            }
        }

        /// <summary>
        /// Query di selezione della dose successiva di una associazione
        /// </summary>
        public static string selProssimaSedutaDaRegistrare
        {
            get
            {
                return @"SELECT   DISTINCT vacSed2.sas_ass_codice, tsd_eta_seduta, tsd_intervallo, paz_data_nascita
    FROM   t_paz_pazienti,
           t_paz_cicli,
           t_ana_tempi_sedute,
           t_ana_associazioni_sedute vacSed2
   WHERE       paz_codice = :paz_codice
           AND pac_paz_codice = paz_codice
           AND pac_cic_codice = tsd_cic_codice
           AND tsd_cic_codice = vacsed2.sas_cic_codice
           AND tsd_n_seduta = vacsed2.sas_n_seduta
           AND NOT EXISTS
                 (SELECT   1
                    FROM   t_vac_eseguite
                   WHERE   ves_paz_codice = :paz_codice
                           AND EXISTS
                                 (SELECT   1
                                    FROM   v_ana_ass_vacc_sedute vacsed1
                                   WHERE   vacsed1.sas_cic_codice =
                                              vacsed2.sas_cic_codice
                                           AND vacsed1.sas_n_seduta =
                                                 vacsed2.sas_n_seduta
                                           AND vacsed1.sas_vac_codice =
                                                 ves_vac_codice
                                           AND vacsed1.sas_n_richiamo =
                                                 ves_n_richiamo))
           AND NOT EXISTS
                 (SELECT   1
                    FROM   t_vac_programmate
                   WHERE       vpr_paz_codice = :paz_codice
                           AND vpr_vac_codice = vacsed2.sas_vac_codice
                           AND vpr_n_richiamo = vacsed2.sas_n_richiamo)
ORDER BY   tsd_eta_seduta, tsd_intervallo";
            }
        }

        /// <summary>
        /// Query di selezione della reazioni avverse
        /// </summary>
        public static string selReazioniAvverseAssociazioneDose
        {
            get
            {
                return @"SELECT   DISTINCT vra_paz_codice ves_paz_codice,
                  vra_res_data_effettuazione ves_data_effettuazione,
                  ves_cns_codice,
                  ves_ass_n_dose,
                  ves_ass_codice,
                  ass_descrizione,
                  vra_data_reazione,
                  vra_rea_codice,
                  rea_descrizione
  FROM   t_sta_reazioni_avverse, t_ana_associazioni, t_ana_reazioni_avverse
 WHERE   vra_rea_codice = rea_codice AND ves_ass_codice = ass_codice(+)
UNION
SELECT   DISTINCT vra_paz_codice,
                  vra_res_data_effettuazione,
                  ves_cns_codice,
                  ves_ass_n_dose,
                  ves_ass_codice,
                  ass_descrizione,
                  vra_data_reazione,
                  vra_re1_codice,
                  rea_descrizione
  FROM   t_sta_reazioni_avverse, t_ana_associazioni, t_ana_reazioni_avverse
 WHERE   vra_re1_codice = rea_codice AND ves_ass_codice = ass_codice(+)
UNION
SELECT   DISTINCT vra_paz_codice,
                  vra_res_data_effettuazione,
                  ves_cns_codice,
                  ves_ass_n_dose,
                  ves_ass_codice,
                  ass_descrizione,
                  vra_data_reazione,
                  vra_re2_codice,
                  rea_descrizione
  FROM   t_sta_reazioni_avverse, t_ana_associazioni, t_ana_reazioni_avverse
 WHERE   vra_re2_codice = rea_codice AND ves_ass_codice = ass_codice(+)";
            }
        }

        /// <summary>
        /// Selezione vaccinazioni eseguite e scadute in base agli id delle vaccinazioni
        /// </summary>
        public static string selVaccinazioniById
        {
            get
            {
                return @"SELECT paz_codice, paz_data_nascita, paz_data_decesso, paz_stato_anagrafico,
         ves_paz_codice_old, ves_cns_registrazione, ves_accesso,
         lot_data_scadenza, ves_dataora_effettuazione, ves_vii_codice,
         ves_in_campagna, ves_cnv_data, ves_cic_codice, ves_stato,
         ves_noc_codice, ves_ass_codice, ves_ass_n_dose, ves_data_registrazione, vac_ordine,
         ves_sii_codice, ves_vac_codice, vac_descrizione, ves_n_richiamo, ves_data_effettuazione, com_descrizione,
         ves_comune_o_stato, cns_descrizione, ves_cns_codice, ves_n_seduta,
         ves_lot_codice, resp.ope_nome, vac.ope_nome ope_nome1, ves_ope_codice,
         ves_med_vaccinante, sii_descrizione, vii_descrizione,
         noc_descrizione, vac_obbligatoria, ves_luogo,
         ves_ute_id, ute_descrizione, ass_descrizione, ass_codice,
         'N' scaduta, ves_ope_in_ambulatorio, ves_esito, ves_flag_fittizia, ves_note,
         ves_amb_codice, amb_descrizione,
         ves_importo, ves_mal_codice_malattia, mal_descrizione, ves_codice_esenzione,
         ves_cnv_data_primo_app, ves_id, ves_ass_prog, ves_data_ultima_variazione, ves_ute_id_ultima_variazione,
         ves_usl_inserimento, t_ana_usl_inserimento_ves.usl_descrizione usl_inserimento_ves_descr,
         ves_usl_scadenza, ves_data_scadenza, ves_ute_id_scadenza, 
         ves_data_ripristino, ves_ute_id_ripristino, ves_flag_visibilita, ves_note_acquisizione, ves_id_acn, ves_provenienza
    FROM t_paz_pazienti,
         t_vac_eseguite,
         t_ana_malattie,
         t_ana_lotti,
         t_ana_vaccinazioni,
         t_ana_consultori,
         t_ana_nomi_commerciali,
         t_ana_siti_inoculazione,
         t_ana_vie_somministrazione,
         t_ana_operatori resp,
         t_ana_operatori vac,
         t_ana_utenti,
         t_ana_associazioni,
         t_ana_comuni,
         t_ana_ambulatori,
         t_ana_usl t_ana_usl_inserimento_ves,
         t_usl_gestite
   WHERE ves_id in ({0})
     AND ugs_abilitazione_vacc_centr = 'S'
     AND ves_paz_codice = paz_codice
     AND ves_mal_codice_malattia = mal_codice(+)
     AND ves_lot_codice = lot_codice(+)
     AND ves_ute_id = ute_id(+)
     AND ves_ass_codice = ass_codice(+)
     AND ves_vac_codice = vac_codice
     AND ves_cns_codice = cns_codice(+)
     AND ves_ope_codice = resp.ope_codice(+)
     AND ves_med_vaccinante = vac.ope_codice(+) 
     AND ves_sii_codice = sii_codice(+)
     AND ves_vii_codice = vii_codice(+)
     AND ves_noc_codice = noc_codice(+)
     AND ves_comune_o_stato = com_codice(+)
     AND ves_amb_codice = amb_codice(+)
     AND ves_usl_inserimento = t_ana_usl_inserimento_ves.usl_codice(+)
     AND ves_usl_inserimento = ugs_usl_codice
UNION
SELECT   paz_codice, paz_data_nascita, paz_data_decesso, paz_stato_anagrafico,
         vsc_paz_codice_old, vsc_cns_registrazione, vsc_accesso,
         lot_data_scadenza, vsc_dataora_effettuazione, vsc_vii_codice,
         vsc_in_campagna, vsc_cnv_data, vsc_cic_codice, vsc_stato,
         vsc_noc_codice, vsc_ass_codice, vsc_ass_n_dose, vsc_data_registrazione, vac_ordine,
         vsc_sii_codice, vsc_vac_codice, vac_descrizione, vsc_n_richiamo, vsc_data_effettuazione, com_descrizione,
         vsc_comune_o_stato, cns_descrizione, vsc_cns_codice, vsc_n_seduta,
         vsc_lot_codice, resp.ope_nome, vac.ope_nome ope_nome1, vsc_ope_codice, 
         vsc_med_vaccinante, sii_descrizione, vii_descrizione, 
         noc_descrizione, vac_obbligatoria, vsc_luogo,
         vsc_ute_id, ute_descrizione, ass_descrizione, ass_codice,
         'S' scaduta, vsc_ope_in_ambulatorio, vsc_esito, vsc_flag_fittizia, vsc_note,
         vsc_amb_codice, amb_descrizione,  
         vsc_importo, vsc_mal_codice_malattia, mal_descrizione, vsc_codice_esenzione,
         vsc_cnv_data_primo_app, vsc_id, vsc_ass_prog, vsc_data_ultima_variazione, vsc_ute_id_ultima_variazione,
         vsc_usl_inserimento ves_usl_inserimento, 
         t_ana_usl_inserimento_ves.usl_descrizione usl_inserimento_ves_descr,
         vsc_usl_scadenza ves_usl_scadenza, 
         vsc_data_scadenza ves_data_scadenza, vsc_ute_id_scadenza ves_ute_id_scadenza, 
         vsc_data_ripristino ves_data_ripristino, vsc_ute_id_ripristino ves_ute_id_ripristino, 
         vsc_flag_visibilita ves_flag_visibilita,
         vsc_note_acquisizione ves_note_acquisizione, vsc_id_acn ves_id_acn, vsc_provenienza ves_provenienza
    FROM t_paz_pazienti,
         t_vac_scadute,
         t_ana_malattie,
         t_ana_lotti,
         t_ana_vaccinazioni,
         t_ana_consultori,
         t_ana_nomi_commerciali,
         t_ana_siti_inoculazione,
         t_ana_vie_somministrazione,
         t_ana_operatori resp,
         t_ana_operatori vac,
         t_ana_utenti,
         t_ana_associazioni,
         t_ana_comuni,
         t_ana_ambulatori,
         t_ana_usl t_ana_usl_inserimento_ves,
         t_usl_gestite
   WHERE vsc_id in ({0}) 
     AND ugs_abilitazione_vacc_centr = 'S'
     AND vsc_paz_codice = paz_codice
     AND vsc_mal_codice_malattia = mal_codice(+)
     AND vsc_lot_codice = lot_codice(+)
     AND vsc_ute_id = ute_id(+)
     AND vsc_ass_codice = ass_codice(+)
     AND vsc_vac_codice = vac_codice
     AND vsc_cns_codice = cns_codice(+)
     AND vsc_ope_codice = resp.ope_codice(+)
     AND vsc_med_vaccinante = vac.ope_codice(+)
     AND vsc_sii_codice = sii_codice(+)
     AND vsc_vii_codice = vii_codice(+)
     AND vsc_noc_codice = noc_codice(+)
     AND vsc_comune_o_stato = com_codice(+)
     AND vsc_amb_codice = amb_codice(+)
     AND vsc_usl_inserimento = t_ana_usl_inserimento_ves.usl_codice(+)
     AND vsc_usl_inserimento = ugs_usl_codice
ORDER BY vac_obbligatoria, vac_ordine, ves_data_effettuazione, ves_ass_n_dose";
            }
        }

        /// <summary>
        /// Selezione reazioni avverse e reazioni scadute in base agli id delle reazioni
        /// </summary>
        public static string selReazioniAvverseById
        {
            get 
            {
                return @"SELECT vra_rea_codice, vra_re1_codice, vra_re2_codice,
         t_ana_reazioni_avverse.rea_descrizione, reazioni1.rea_descrizione rea_descrizione1,
         reazioni2.rea_descrizione rea_descrizione2, vra_data_reazione, vra_visita,
         vra_esi_codice, vra_terapia, esi_descrizione, 
         vra_rea_altro, vra_gravita_reazione, vra_grave, vra_esito,
         vra_data_esito, vra_motivo_decesso, vra_dosaggio, vra_sospeso,
         vra_migliorata, vra_ripreso, vra_ricomparsa, vra_indicazioni,
         vra_richiamo, vra_luogo, vra_altro_luogo, vra_farmaco_concomitante,
         vra_farmaco_descrizione, vra_uso_concomitante,
         vra_condizioni_concomitanti, vra_qualifica, vra_altra_qualifica,
         vra_cognome_segnalatore, vra_nome_segnalatore,
         vra_indirizzo_segnalatore, vra_tel_segnalatore, vra_fax_segnalatore,
         vra_mail_segnalatore, vra_data_compilazione,  
         vra_ute_id_compilazione, vra_data_variazione, vra_ute_id_variazione,
         vra_id, vra_usl_inserimento, t_ana_usl_inserimento_vra.usl_descrizione usl_inserimento_vra_descr,
         vra_altre_informazioni, vra_ambito_osservazione, 
         vra_ambito_studio_titolo, vra_ambito_studio_tipologia, vra_ambito_studio_numero,
         vra_peso, vra_altezza, vra_data_ultima_mestruazione, 
         vra_allattamento, vra_gravidanza, vra_causa_osservata,
         vra_farmconc1_noc_descrizione, vra_farmconc1_lot_codice, vra_farmconc1_dataora_eff,
         vra_farmconc1_dose, vra_farmconc1_sii_codice, vra_farmconc1_vii_codice, 
         vra_farmconc1_sospeso, vra_farmconc1_migliorata, vra_farmconc1_ripreso,
         vra_farmconc1_ricomparsa, vra_farmconc1_indicazioni, vra_farmconc1_dosaggio, 
         vra_farmconc1_richiamo,
         vra_farmconc2_noc_descrizione, vra_farmconc2_lot_codice, vra_farmconc2_dataora_eff,
         vra_farmconc2_dose, vra_farmconc2_sii_codice, vra_farmconc2_vii_codice, 
         vra_farmconc2_sospeso, vra_farmconc2_migliorata, vra_farmconc2_ripreso,
         vra_farmconc2_ricomparsa, vra_farmconc2_indicazioni, vra_farmconc2_dosaggio, 
         vra_farmconc2_richiamo, vra_firma_segnalatore, vra_oet_codice, vra_lot_data_scadenza, vra_farmconc1_lot_data_scad, vra_farmconc2_lot_data_scad, vra_noi_codice_indicazioni, vra_farmconc1_noi_cod_indic, vra_farmconc2_noi_cod_indic, vra_id_scheda,
         vra_farmconc1_noc_codice, vra_farmconc2_noc_codice, vra_segnalazione_id, vra_ute_id_invio, vra_data_invio, vra_flag_inviato
    FROM t_vac_reazioni_avverse,
         t_ana_esiti_terapie,
         t_ana_reazioni_avverse,
         t_ana_reazioni_avverse reazioni1,
         t_ana_reazioni_avverse reazioni2,
         t_ana_usl t_ana_usl_inserimento_vra,
         t_usl_gestite
   WHERE vra_id in ({0})
     AND ugs_abilitazione_vacc_centr = 'S'
     AND vra_rea_codice = t_ana_reazioni_avverse.rea_codice(+)
     AND vra_re1_codice = reazioni1.rea_codice(+)
     AND vra_re2_codice = reazioni2.rea_codice(+)
     AND vra_esi_codice = esi_codice(+)
     AND vra_usl_inserimento = t_ana_usl_inserimento_vra.usl_codice(+)
     AND vra_usl_inserimento = ugs_usl_codice
UNION
SELECT   vrs_rea_codice, vrs_re1_codice, vrs_re2_codice,
         t_ana_reazioni_avverse.rea_descrizione, reazioni1.rea_descrizione rea_descrizione1,
         reazioni2.rea_descrizione rea_descrizione2, vrs_data_reazione, vrs_visita,
         vrs_esi_codice, vrs_terapia, esi_descrizione, 
         vrs_rea_altro, vrs_gravita_reazione, vrs_grave, vrs_esito,
         vrs_data_esito, vrs_motivo_decesso, vrs_dosaggio, vrs_sospeso,
         vrs_migliorata, vrs_ripreso, vrs_ricomparsa, vrs_indicazioni,
         vrs_richiamo, vrs_luogo, vrs_altro_luogo, vrs_farmaco_concomitante,
         vrs_farmaco_descrizione, vrs_uso_concomitante,
         vrs_condizioni_concomitanti, vrs_qualifica, vrs_altra_qualifica,
         vrs_cognome_segnalatore, vrs_nome_segnalatore,
         vrs_indirizzo_segnalatore, vrs_tel_segnalatore, vrs_fax_segnalatore,
         vrs_mail_segnalatore, vrs_data_compilazione, 
         vrs_ute_id_compilazione, vrs_data_variazione, vrs_ute_id_variazione,
         vrs_id vra_id, vrs_usl_inserimento vra_usl_inserimento, t_ana_usl_inserimento_vra.usl_descrizione usl_inserimento_vra_descr,
         vrs_altre_informazioni vra_altre_informazioni, vrs_ambito_osservazione vra_ambito_osservazione, 
         vrs_ambito_studio_titolo vra_ambito_studio_titolo, vrs_ambito_studio_tipologia vra_ambito_studio_tipologia, vrs_ambito_studio_numero vra_ambito_studio_numero,
         vrs_peso vra_peso, vrs_altezza vra_altezza, vrs_data_ultima_mestruazione vra_data_ultima_mestruazione, 
         vrs_allattamento vra_allattamento, vrs_gravidanza vra_gravidanza, vrs_causa_osservata vra_causa_osservata,
         vrs_farmconc1_noc_descrizione vra_farmconc1_noc_descrizione, vrs_farmconc1_lot_codice vra_farmconc1_lot_codice, vrs_farmconc1_dataora_eff vra_farmconc1_dataora_eff,
         vrs_farmconc1_dose vra_farmconc1_dose, vrs_farmconc1_sii_codice vra_farmconc1_sii_codice, vrs_farmconc1_vii_codice vra_farmconc1_vii_codice, 
         vrs_farmconc1_sospeso vra_farmconc1_sospeso, vrs_farmconc1_migliorata vra_farmconc1_migliorata, vrs_farmconc1_ripreso vra_farmconc1_ripreso,
         vrs_farmconc1_ricomparsa vra_farmconc1_ricomparsa, vrs_farmconc1_indicazioni vra_farmconc1_indicazioni, vrs_farmconc1_dosaggio vra_farmconc1_dosaggio, 
         vrs_farmconc1_richiamo vra_farmconc1_richiamo,
         vrs_farmconc2_noc_descrizione vra_farmconc2_noc_descrizione, vrs_farmconc2_lot_codice vra_farmconc2_lot_codice, vrs_farmconc2_dataora_eff vra_farmconc2_dataora_eff,
         vrs_farmconc2_dose vra_farmconc2_dose, vrs_farmconc2_sii_codice vra_farmconc2_sii_codice, vrs_farmconc2_vii_codice vra_farmconc2_vii_codice, 
         vrs_farmconc2_sospeso vra_farmconc2_sospeso, vrs_farmconc2_migliorata vra_farmconc2_migliorata, vrs_farmconc2_ripreso vra_farmconc2_ripreso,
         vrs_farmconc2_ricomparsa vra_farmconc2_ricomparsa, vrs_farmconc2_indicazioni vra_farmconc2_indicazioni, vrs_farmconc2_dosaggio vra_farmconc2_dosaggio, 
         vrs_farmconc2_richiamo vra_farmconc2_richiamo, vrs_firma_segnalatore vra_firma_segnalatore, vrs_oet_codice vra_oet_codice, vrs_lot_data_scadenza vra_lot_data_scadenza, vrs_farmconc1_lot_data_scad vra_farmconc1_lot_data_scad,
         vrs_farmconc2_lot_data_scad vra_farmconc2_lot_data_scad, vrs_noi_codice_indicazioni vra_noi_codice_indicazioni, vrs_farmconc1_noi_cod_indic vra_farmconc1_noi_cod_indic, vrs_farmconc2_noi_cod_indic vra_farmconc2_noi_cod_indic, vrs_id_scheda vra_id_scheda,
         vrs_farmconc1_noc_codice vra_farmconc1_noc_codice, vrs_farmconc2_noc_codice vra_farmconc2_noc_codice, vrs_segnalazione_id vra_segnalazione_id, vrs_ute_id_invio vra_ute_id_invio, vrs_data_invio vra_data_invio, vrs_flag_inviato vra_flag_inviato
    FROM t_vac_reazioni_scadute,
         t_ana_esiti_terapie,
         t_ana_reazioni_avverse,
         t_ana_reazioni_avverse reazioni1,
         t_ana_reazioni_avverse reazioni2,
         t_ana_usl t_ana_usl_inserimento_vra,
         t_usl_gestite
   WHERE vrs_id in ({0}) 
     AND ugs_abilitazione_vacc_centr = 'S'
     AND vrs_rea_codice = t_ana_reazioni_avverse.rea_codice(+)
     AND vrs_re1_codice = reazioni1.rea_codice(+)
     AND vrs_re2_codice = reazioni2.rea_codice(+)
     AND vrs_esi_codice = esi_codice(+)
     AND vrs_usl_inserimento = t_ana_usl_inserimento_vra.usl_codice(+)
     AND vrs_usl_inserimento = ugs_usl_codice
ORDER BY vra_id";
            }
        }

        /// <summary>
        /// Restituisce i dati della Vaccinazione Eseguita centrale in base all'id locale e al codice della usl
        /// </summary>
        public static string selVaccinazioneEseguitaCentraleByIdLocale
        {
            get
            {
                return @"select T_VACCINAZIONI_CENTRALE.* 
from T_VACCINAZIONI_DISTRIBUITE join T_VACCINAZIONI_CENTRALE on VCD_VCC_ID = VCC_ID 
where VCD_VES_ID = :idVaccinazioneEseguita
and VCD_USL_CODICE = :codiceUsl";
            }
        }
        

    }
}

