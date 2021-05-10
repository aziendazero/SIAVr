namespace Onit.OnAssistnet.OnVac.Queries.Postazioni
{
	/// <summary>
	/// Descrizione di riepilogo per OracleQueries.
	/// </summary>
	public static class OracleQueries
	{

        /// <summary>
        /// 
        /// </summary>
		public static string cntPostazioni
		{
			get
			{
				return @"select count(pos_id) from t_ana_postazioni";
			}
		}

	}
}
