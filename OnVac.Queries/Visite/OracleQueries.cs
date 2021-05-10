namespace Onit.OnAssistnet.OnVac.Queries.Visite
{
   
    
    // <summary>
    /// Queries visite
    /// </summary>
    public static class OracleQueries
    {
        /// <summary>
        /// Conteggia se una visita ha un padre followup
        /// </summary>
        public static string cntVisitePadreFollowuUp
        {
            get
            {
                return @"select count(*) from t_vis_visite where vis_vis_id_follow_up = :vis_vis_id_follow_up";
            }
        }
        /// <summary>
        /// Ricerca visita in base all'id
        /// </summary>
        public static string selVisitaById
        {
            get
            {
                return @"select t_vis_visite.*, mal_descrizione, mos_descrizione, bil_descrizione, 
    ril.ope_nome descrizione_rilevatore, med.ope_nome descrizione_medico, cns_descrizione, cit_stato 
    from t_vis_visite 
    left join t_ana_motivi_sospensione on vis_mos_codice = mos_codice 
    left join t_ana_malattie on vis_mal_codice = mal_codice 
    left join t_ana_bilanci on vis_n_bilancio = bil_numero and vis_mal_codice = bil_mal_codice 
    left join t_ana_operatori ril on vis_ope_codice_rilevatore = ril.ope_codice
    left join t_ana_operatori med on vis_ope_codice = med.ope_codice
    left join t_ana_consultori on vis_cns_codice = cns_codice
    left join t_ana_cittadinanze on vis_cit_codice_paese_viaggio = cit_codice
    where vis_id = :vis_id";
            }
        }
        /// <summary>
        /// Recupera viaggidi una visita
        /// </summary>
        public static string selViaggiVisita
        {
            get
            {
                return @"SELECT * FROM T_VIS_VIAGGI JOIN T_ANA_CITTADINANZE ON VVG_CIT_CODICE_PAESE_VIAGGIO = CIT_CODICE where VVG_VIS_ID = {0} ";
            }
        }
        /// <summary>
        /// Ricerca visite in base agli id specificati
        /// </summary>
        public static string selVisiteById
        {
            get
            {
                return @"select t_vis_visite.*, mal_descrizione, mos_descrizione, bil_descrizione, 
    ope_nome descrizione_rilevatore, cit_stato, '' descrizione_medico, '' cns_descrizione 
    from t_vis_visite 
    left join t_ana_motivi_sospensione on vis_mos_codice = mos_codice
    left join t_ana_malattie on vis_mal_codice = mal_codice
    left join t_ana_bilanci on vis_n_bilancio = bil_numero and vis_mal_codice = bil_mal_codice
    left join t_ana_operatori on vis_ope_codice_rilevatore = ope_codice
    left join t_ana_cittadinanze on vis_cit_codice_paese_viaggio = cit_codice
    where vis_id in ({0})";
            }
        }

        public static string selVisitaEliminataById
        {
            get
            {
                return @"select t_vis_visite_eliminate.*,  '' mal_descrizione, '' mos_descrizione, 
    '' descrizione_medico, ope_nome descrizione_rilevatore, cit_stato, '' cns_descrizione
    from t_vis_visite_eliminate 
    left join t_ana_operatori on vie_ope_codice_rilevatore = ope_codice
    left join t_ana_cittadinanze on vie_cit_codice_paese_viaggio = cit_codice
    where vie_id = :vie_id";
            }
        }

        /// <summary>
        /// Ricerca visite del paziente specificato
        /// </summary>
        public static string selVisitePaziente
        {
            get
            {
                return @"select t_vis_visite.*, mal_descrizione, mos_descrizione, '' vie_data_eliminazione, '' vie_ute_id_eliminazione, '' bil_descrizione, 
    ope_nome descrizione_rilevatore, cit_stato, '' descrizione_medico, '' cns_descrizione 
    from t_vis_visite 
    left join t_ana_motivi_sospensione on vis_mos_codice = mos_codice
    left join t_ana_malattie on vis_mal_codice = mal_codice
    left join t_ana_operatori on vis_ope_codice_rilevatore = ope_codice
    left join t_ana_cittadinanze on vis_cit_codice_paese_viaggio = cit_codice
    where vis_paz_codice = :codPaziente";
            }
        }

        /// <summary>
        /// Restituisce il valore della sequence relativa alle visite
        /// </summary>
        public static string selNextSeqVisite
        {
            get
            {
                return @"select seq_visite.nextval from dual";
            }
        }

        public static string selNextSeqViaggioVisite
        {
            get
            {
                return @"select seq_viaggi.nextval from dual";
            }
        }

        /// <summary>
        /// Restituisce il valore della sequence relativa alle osservazioni
        /// </summary>
        public static string selNextSeqOsservazioni
        {
            get
            {
                return @"select seq_vis_osservazioni.nextval from dual";
            }
        }

        /// <summary>
        /// Restituisce i dati della visita centrale in base all'id della visita e alla usl di inserimento
        /// </summary>
        public static string selVisitaCentraleByUslInserimento
        {
            get
            {
                return "SELECT * FROM T_VISITE_CENTRALE WHERE VIC_VIS_ID = :idVisita AND VIC_USL_INSERIMENTO = :codiceUslInserimento";
            }
        }

        /// <summary>
        /// Restituisce i dati della visita centrale in base all'id locale e al codice della usl
        /// </summary>
        public static string selVisitaCentraleByIdLocale
        {
            get
            {
                return @"select T_VISITE_CENTRALE.* 
from T_VISITE_DISTRIBUITE join T_VISITE_CENTRALE on VID_VIC_ID = VIC_ID 
where VID_VIS_ID = :idVisita 
and VID_USL_CODICE = :codiceUsl";
            }
        }

        /// <summary>
        /// Restituisce i dati della visita distribuita in base all'id della visita e al codice della usl
        /// </summary>
        public static string selVisitaCentraleDistribuitaByUsl
        {
            get
            {
                return "SELECT * FROM T_VISITE_DISTRIBUITE WHERE VID_VIS_ID = :idVisita AND VID_USL_CODICE = :codiceUsl";
            }
        }

    }
}
