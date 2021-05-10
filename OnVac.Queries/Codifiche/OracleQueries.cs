namespace Onit.OnAssistnet.OnVac.Queries.Codifiche
{
	/// <summary>
	/// Queries relative alle codifiche.
	/// </summary>
	public static class OracleQueries
	{
        /// <summary>
        /// 
        /// </summary>
		public static string selCodifiche
		{
			get
			{
				return @"select * from t_ana_codifiche where cod_campo = :cod_campo and (cod_obsoleto is null or lower(cod_obsoleto) = 'n')";
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public static string selCodifica
		{
			get
			{
				return @"select * from t_ana_codifiche where cod_campo = :cod_campo and cod_codice = :cod_codice";
			}
		}

	}
}
