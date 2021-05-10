namespace Onit.OnAssistnet.OnVac.Queries.Parametri
{
	/// <summary>
	/// Query Parametri
	/// </summary>
	public static class OracleQueries
	{

		/// <summary>
		/// Lettura di tutti i parametri relativi al consultorio specificato
		/// </summary>
		public static string selParametriCns
		{
			get 
			{
                return string.Format(
                        @"select par_codice, par_cns_codice, par_valore, par_descrizione, par_centrale
                          from t_ana_parametri par1
                          where (par1.par_cns_codice = :cod_cns or par1.par_cns_codice = '{0}')
                          and not exists (
                                select * from t_ana_parametri par2
                                where par2.par_codice = par1.par_codice
                                and par1.par_cns_codice = '{0}'
                                and par2.par_cns_codice = :cod_cns
                            )", Constants.CommonConstants.CodiceConsultorioSistema);
			}
		}

        /// <summary>
        /// Lettura di tutti i parametri di sistema
        /// </summary>
        public static string selParametriSistema
        {
            get
            {
                return string.Format(
                        @"select par_codice, par_cns_codice, par_valore, par_descrizione, par_centrale
                          from t_ana_parametri 
                          where  par_cns_codice = '{0}'", Constants.CommonConstants.CodiceConsultorioSistema);
            }
        }

	}
}
