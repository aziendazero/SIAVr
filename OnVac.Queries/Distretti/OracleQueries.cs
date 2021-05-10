namespace Onit.OnAssistnet.OnVac.Queries.Distretti
{
	/// <summary>
	/// Descrizione di riepilogo per OracleQueries.
	/// </summary>
	public static class OracleQueries
	{
        /// <summary>
        /// 
        /// </summary>
        public static string selCodiceByCodiceEsterno
        {
            get
            {
                return @"select dis_codice from t_ana_distretti where dis_codice_esterno = :dis_codice_esterno";
            }
        }

	}
}
