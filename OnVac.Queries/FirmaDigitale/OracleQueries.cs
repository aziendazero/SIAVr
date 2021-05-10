namespace Onit.OnAssistnet.OnVac.Queries.FirmaDigitale
{
    /// <summary>
    /// Queries interventi.
    /// </summary>
    public static class OracleQueries
    {
        /// <summary>
        /// Ricerca delle visite nella tabella t_vis_visite
        /// </summary>
        public static string selectDocumentiVisite
        {
            get
            {
                return @"select 
                    vis_id, paz_cognome, paz_nome, paz_data_nascita, vis_data_visita, 
                    vis_data_registrazione, vis_usl_inserimento, uv.ute_codice as ute_visita, 
                    vis_ute_id_firma, uf.ute_codice as ute_firma, vis_data_firma,
                    vis_ute_id_archiviazione, ua.ute_codice as ute_arc, vis_data_archiviazione,
                    vis_doc_id_documento, uo.ope_nome as ute_rilevatore
                    from t_vis_visite 
                        inner join t_paz_pazienti on vis_paz_codice = paz_codice
                        left outer join t_ana_utenti uv on vis_ute_id = uv.ute_id
                        left outer join t_ana_utenti uf on vis_ute_id_firma = uf.ute_id
                        left outer join t_ana_utenti ua on vis_ute_id_archiviazione = ua.ute_id
                        left outer join t_ana_operatori uo on vis_ope_codice_rilevatore = uo.ope_codice
                    {0} order by {1}";
            }
        }

        /// <summary>
        /// Conteggio risultati della ricerca delle visite nella tabella t_vis_visite
        /// </summary>
        public static string countDocumentiVisite
        {
            get
            {
                return @"select count(*) from t_vis_visite {0}";
            }
        }

        /// <summary>
        /// Ricerca dei soli id visite in base ai filtri
        /// </summary>
        public static string selectIdVisiteDocumenti
        { 
            get
            {
                return @"select vis_id from t_vis_visite {0} ";
            }
        }

    }
}
