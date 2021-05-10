namespace Onit.OnAssistnet.OnVac.Queries.StatiAnagrafici
{
	/// <summary>
	/// Descrizione di riepilogo per OracleQueries.
	/// </summary>
	public static class OracleQueries
	{

        /// <summary>
        /// Restituisce i codici degli stati anagrafici impostati come default
        /// </summary>
		public static string selStatoAnagDefault
		{
			get
			{
				return @"select san_codice from t_ana_stati_anagrafici where san_default_locale = 'S'";
			}
		}

        public static string selStatoAnagFromCategoriaCittadino
        {
            get
            {
                return @"select sac_san_codice from t_ana_stati_anag_categorie where sac_categoria_cittadino = :sac_categoria_cittadino order by sac_san_codice ";
            }
        }

    }
}
