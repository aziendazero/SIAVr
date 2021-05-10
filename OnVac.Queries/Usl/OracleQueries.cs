namespace Onit.OnAssistnet.OnVac.Queries.Usl
{
	/// <summary>
	/// Descrizione di riepilogo per OracleQueries.
	/// </summary>
	public static class OracleQueries
	{

        /// <summary>
        /// Seleziona la descrizione della usl in base al codice
        /// </summary>
		public static string selDescrizione
		{
			get
			{
				return @"select usl_descrizione from t_ana_usl where usl_codice = :codUsl";
			}
		}

        public static string selCodiceRegione
        {
            get
            {
                return @"select USL_REG_CODICE from t_ana_usl where usl_codice = :codUsl";
            }
        }

        /// <summary>
        /// Restituisce i codici (ordinati) delle usl associate al comune specificato
        /// </summary>
        public static string selUslByComune
        {
            get
            {
                return @"select lcu_usl_codice from t_ana_link_comuni_usl where lcu_com_codice = :codiceComune order by lcu_usl_codice";
            }
        }
		/// <summary>
		/// 
		/// </summary>
		public static string selCodiceAifa
		{
			get
			{
				return @"select usu_codice_aifa from t_usl_unificate where usu_codice = :codUsl";
			}
		}

	}
}
