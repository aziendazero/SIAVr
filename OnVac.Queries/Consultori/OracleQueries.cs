namespace Onit.OnAssistnet.OnVac.Queries.Consultori
{
	/// <summary>
	/// Query oracle relative alle convocazioni.
	/// </summary>
	public static class OracleQueries
	{
        
        /// <summary>
        /// 
        /// </summary>
		public static string selCnsByPediatra
		{
			get
			{
				return @"select cns_codice from t_ana_consultori
where cns_pediatra_vaccinatore = :cod_pediatra
and (:eta_paziente between (cns_da_eta/360) and (cns_a_eta/360))
and ( (sysdate between cns_data_apertura and cns_data_chiusura) or cns_data_chiusura is null )";
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public static string selCnsByComune
		{
			get
			{
				return @"select cns_codice
from t_ana_consultori join t_ana_link_comuni_consultori on cns_codice = cco_cns_codice
where :eta_paziente between (cns_da_eta/360) and (cns_a_eta/360)
and ( (sysdate between cns_data_apertura and cns_data_chiusura) or cns_data_chiusura is null )
and :cod_comune = cco_com_codice";
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public static string selCnsByCircoscrizione
		{
			get
			{
				return @"select cns_codice
from t_ana_consultori join t_ana_link_circoscrizioni_cns on cns_codice = rco_cns_codice
where :eta_paziente between (cns_da_eta/360) and (cns_a_eta/360)
and ( (sysdate between cns_data_apertura and cns_data_chiusura) or cns_data_chiusura is null)
and :cod_circoscrizione = rco_cir_codice";
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public static string selCnsSmistamento
		{
			get
			{
				return @"select cns_codice from t_ana_consultori
where :eta_paziente between (cns_da_eta/360) and (cns_a_eta/360)
and ( (sysdate between cns_data_apertura and cns_data_chiusura) or cns_data_chiusura is null)
and cns_smistamento = 'S'";
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public static string selComuneConsultorio
		{
			get
			{
				return @"select cns_com_codice from t_ana_consultori where cns_codice = :cod_cns";
			}
		}

        /// <summary>
        /// 
        /// </summary>
        public static string selTipoErogatoreConsultorio
        {
            get
            {
                return @"select cns_tipo_erogatore from t_ana_consultori where cns_codice = :cod_cns";
            }
        }

        /// <summary>
        /// 
        /// </summary>
		public static string selConsultoriInComune
		{
			get {
				return @"SELECT cco_cns_codice, cco_com_codice, cns_descrizione, cns_da_eta, cns_a_eta
  FROM t_ana_link_comuni_consultori, t_ana_consultori
 WHERE cco_cns_codice = cns_codice
   AND cco_com_codice = :comcodice";
			}
		}

        /// <summary>
        /// 
        /// </summary>			
		public static string selConsultoriInCircoscrizione
		{
			get 
			{
				return @"SELECT rco_cns_codice, rco_cir_codice, cns_descrizione, cns_da_eta, cns_a_eta
  FROM t_ana_link_circoscrizioni_cns, t_ana_consultori
 WHERE rco_cns_codice = cns_codice AND rco_cir_codice = :circodice";
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public static string selCircoscrizioniInConsultorio
		{
			get 
			{
				return @"SELECT   rco_cir_codice, cir_descrizione, cns_com_codice
			FROM t_ana_consultori, t_ana_link_circoscrizioni_cns, t_ana_circoscrizioni
				WHERE cns_codice = :cnscodice
				  AND cns_codice = rco_cns_codice
				  AND cir_codice = rco_cir_codice
			 ORDER BY cir_descrizione";
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public static string selConsultoriPerGiorniNascita
		{
			get 
			{
				return @"SELECT cns_codice, cns_descrizione, cns_a_eta, cns_da_eta
  FROM t_ana_consultori
 WHERE cns_da_eta <= :giorninascita AND cns_a_eta >= :giorninascita";
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public static string selDataConvocazioneCampagna
		{
			get
			{
				return @"select cns_crea_dcnv
from t_ana_consultori
where cns_codice = :cod_cns";
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public static string updDataConvocazioneCampagna
		{
			get 
			{
				return @"update t_ana_consultori
set cns_crea_dcnv = :data_cnv
where cns_codice = :cod_cns";
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public static string selEtaValidita
		{
			get
			{
				return @"select cns_da_eta, cns_a_eta
from t_ana_consultori
where cns_codice = :cod_cns";
			}
		}

        /// <summary>
        /// 
        /// </summary>
        public static string existsConsultorio
        {
            get
            {
                return @"select 1 from t_ana_consultori where cns_codice = :cns_codice";
            }
        }

	}
}

