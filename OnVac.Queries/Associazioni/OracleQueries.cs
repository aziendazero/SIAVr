namespace Onit.OnAssistnet.OnVac.Queries.Associazioni
{
	/// <summary>
	/// Query oracle relative alle associazioni.
	/// </summary>
	public static class OracleQueries
	{

		/// <summary>
		/// Restituisce le vaccinazioni che compongono l'associazione.
		/// </summary>
		public static string selVaccinazioniAssociazione
        {
			get 
            {
                return @"select val_vac_codice
from t_ana_link_ass_vaccinazioni
where val_ass_codice = :cod_ass";
			}
		}

        /// <summary>
        /// Recupero sito e via somministrazione relativi all'associazione specificata
        /// </summary>
        public static string selSitoViaByAssociazione
        {
            get
            {
                return @"SELECT ASS_SII_CODICE, SII_DESCRIZIONE, ASS_VII_CODICE, VII_DESCRIZIONE
FROM T_ANA_ASSOCIAZIONI 
  LEFT JOIN T_ANA_SITI_INOCULAZIONE ON ASS_SII_CODICE = SII_CODICE
  LEFT JOIN T_ANA_VIE_SOMMINISTRAZIONE ON ASS_VII_CODICE = VII_CODICE
WHERE ASS_CODICE = :ass_codice";
            }
        }
	
	}
}

