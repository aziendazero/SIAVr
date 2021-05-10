using System;

namespace Onit.OnAssistnet.OnVac.Queries.ConfigurazioneCertificato
{
    /// <summary>
    /// Descrizione di riepilogo per OracleQueries.
    /// </summary>
    public static class OracleQueries
    {

        /// <summary>
        /// Sequence Configurazione Certificato
        /// </summary>
        public static string selNextSeqConfigurazioneCertificato
        {
            get
            {
                return @"SELECT SEQ_CONFIGURAZIONI.NEXTVAL FROM DUAL";
            }
        }

        /// <summary>
        /// Restituisce i dati di una singola configurazione 
        /// </summary>
        public static string selConfigurazioneControllo
        {
            get
            {
                return @"select * from t_configurazioni_controlli where coc_id = :coc_id order by coc_data_nascita_da, coc_data_nascita_a, coc_eta_anno_da, coc_eta_mese_da, coc_eta_giorno_da";
            }
        }
        /// <summary>
        /// Restituisce tutti i record della tabella di configurazione certificati
        /// </summary>
        public static string selConfigurazioneControlli
        {
            get
            {
                return @"select * from t_configurazioni_controlli order by coc_data_nascita_da, coc_data_nascita_a, coc_eta_anno_da, coc_eta_mese_da, coc_eta_giorno_da, coc_eta_anno_a, coc_eta_mese_a, coc_eta_giorno_a";
            }
        }
        /// <summary>
        /// Recupera scritta da mettere nel report dei certificati vaccinali 
        /// </summary>
        public static string selScrittaCetrificato
        {
            get
            {
                return @"select psc_scritta_certificato from v_paz_scritta_certificato where psc_paz_codice = :psc_paz_codice";
            }
        }
        /// <summary>
        /// Recupera i dati della link configurazione vaccini
        /// </summary>
        public static string selLinkVaccConfig
        {
            get
            {
                return @"select cov_coc_id, cov_vac_codice, vac_descrizione, cov_n_dose from t_configurazioni_vaccinazioni, t_ana_vaccinazioni where cov_vac_codice = vac_codice  and cov_coc_id = :cov_coc_id ";
            }
        }
        /// <summary>
        /// Inserimento configurazione certificato
        /// </summary>
        public static string insConfigurazione
        {
            get
            {
                return @"INSERT INTO T_CONFIGURAZIONI_CONTROLLI (COC_ID, COC_DATA_NASCITA_DA, COC_DATA_NASCITA_A, COC_ETA_ANNO_DA, COC_ETA_MESE_DA, COC_ETA_GIORNO_DA,COC_ETA_ANNO_A, COC_ETA_MESE_A, COC_ETA_GIORNO_A, COC_SESSO, COC_DATA_CONTROLLO, COC_MOT_CODICE_IMMUNITA, COC_MOT_CODICE_ESONERO, COC_TESTO_POSITIVO, COC_TESTO_NEGATIVO,  COC_TESTO_PARZIALE, COC_FLAG_CHECK_APPUNTAMENTI, COC_TIPO_CHECK_APPUNTAMENTI)
                         VALUES (:coc_id, :coc_data_nascita_da, :coc_data_nascita_a, :coc_eta_anno_da, :coc_eta_mese_da, :coc_eta_giorno_da, :coc_eta_anno_a, :coc_eta_mese_a, :coc_eta_giorno_a, :coc_sesso, :coc_data_controllo, :coc_mot_codice_immunita, :coc_mot_codice_esonero, :coc_testo_positivo, :coc_testo_negativo, :coc_testo_parziale, :coc_flag_check_appuntamenti, :coc_tipo_check_appuntamenti )";
            }
        }

        /// <summary>
        /// Update configurazione certificato
        /// </summary>
        public static string updConfigurazione
        {
            get
            {
                return @"UPDATE T_CONFIGURAZIONI_CONTROLLI
                         SET COC_ID = :coc_id,
                         COC_DATA_NASCITA_DA= :coc_data_nascita_da,
                         COC_DATA_NASCITA_A= :coc_data_nascita_a, 
                         COC_ETA_ANNO_DA = :coc_eta_anno_da, 
                         COC_ETA_MESE_DA = :coc_eta_mese_da,
                         COC_ETA_GIORNO_DA = :coc_eta_giorno_da,
                         COC_ETA_ANNO_A = :coc_eta_anno_a,
                         COC_ETA_MESE_A = :coc_eta_mese_a,
                         COC_ETA_GIORNO_A = :coc_eta_giorno_a,
                         COC_SESSO = :coc_sesso,
                         COC_DATA_CONTROLLO = :coc_data_controllo,
                         COC_MOT_CODICE_IMMUNITA = :coc_mot_codice_immunita,
                         COC_MOT_CODICE_ESONERO = :coc_mot_codice_esonero,
                         COC_TESTO_POSITIVO = :coc_testo_positivo,
                         COC_TESTO_NEGATIVO = :coc_testo_negativo,
                         COC_TESTO_PARZIALE = :coc_testo_parziale,
                         COC_FLAG_CHECK_APPUNTAMENTI = :coc_flag_check_appuntamenti,
                         COC_TIPO_CHECK_APPUNTAMENTI = :coc_tipo_check_appuntamenti
                         WHERE COC_ID = :coc_id";
            }
        }

        /// <summary>
        /// Cancellazione configurazione certificato
        /// </summary>
        public static string delConfigurazione
        {
            get
            {
                return @"DELETE FROM T_CONFIGURAZIONI_CONTROLLI WHERE COC_ID = :coc_id";
            }
        }
        /// <summary>
        /// Cancellazione configurazione certificato vaccini
        /// </summary>
        public static string delConfigurazioneVacciniDosi
        {
            get
            {
                return @"DELETE FROM T_CONFIGURAZIONI_VACCINAZIONI WHERE COV_COC_ID = :cov_coc_id";
            }
        }
        /// <summary>
        /// Inserimento configurazione certificato vaccini
        /// </summary>
        public static string insConfigurazioneVacciniDosi
        {
            get
            {
                return @"INSERT INTO T_CONFIGURAZIONI_VACCINAZIONI (COV_COC_ID, COV_VAC_CODICE, COV_N_DOSE)
                         VALUES (:cov_coc_id, :cov_vac_codice, :cov_n_dose)";
            }
        }

    }
}

