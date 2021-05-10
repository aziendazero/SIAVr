
namespace Onit.OnAssistnet.OnVac.Queries.MotiviEsclusione
{
	/// <summary>
	/// Query oracle relative alle convocazioni.
	/// </summary>
	public static class OracleQueries
	{

        /// <summary>
        /// 
        /// </summary>
        public static string selCalcoloScadenzaMotivoEsclusione
        {
            get
            {
                return @" select moe_calcolo_scadenza
                            from t_ana_motivi_esclusione
                           where moe_codice = :moe_codice";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string selScadenzeMotiviEsclusione
        {
			get {
                return @" select mos_id, mos_mesi, mos_anni, mos_lst_vac_codice
                            from t_ana_motivi_esclusione_scad
                           where mos_moe_codice = :mos_moe_codice
                        order by mos_anni, mos_mesi";
			}
		}

        /// <summary>
        /// 
        /// </summary>
        public static string insScadenzaMotiviEsclusione
        {
            get
            {
                return @" insert into t_ana_motivi_esclusione_scad 
                                 (mos_moe_codice, mos_mesi, mos_anni, mos_lst_vac_codice)
                          values (:mos_moe_codice, :mos_mesi, :mos_anni, :mos_lst_vac_codice)";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string updScadenzaMotiviEsclusione
        {
            get
            {
                return @" update t_ana_motivi_esclusione_scad 
                             set mos_mesi = :mos_mesi,
                                 mos_anni = :mos_anni,
                       mos_lst_vac_codice = :mos_lst_vac_codice
                           where mos_id = :mos_id";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string delScadenzaMotiviEsclusione
        {
            get
            {
                return @" delete t_ana_motivi_esclusione_scad 
                           where mos_id = :mos_id";
            }
        }

    }
}

