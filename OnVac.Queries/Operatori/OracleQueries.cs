namespace Onit.OnAssistnet.OnVac.Queries.Operatori
{
	/// <summary>
	/// Descrizione di riepilogo per OracleQueries.
	/// </summary>
	public static class OracleQueries
	{

		/// <summary>
		/// Seleziona tutti i dati dell'operatore specificato
		/// </summary>
		public static string selOperatore
		{
			get
			{
				return @"select * from t_ana_operati where ope_codice = :ope_codice";
			}
		}


		/// <summary>
		/// Restituisce i consultori abilitati all'operatore specificato
		/// </summary>
		public static string sel_consultori_operatore
		{
			get
			{
				return @"SELECT LOC_OPE_CODICE, CNS_CODICE, CNS_DESCRIZIONE, DIS_CODICE, DIS_DESCRIZIONE, DIS_USL_CODICE, USL_DESCRIZIONE
FROM T_ANA_CONSULTORI 
LEFT JOIN T_ANA_LINK_OPER_CONSULTORI on LOC_CNS_CODICE = CNS_CODICE
LEFT JOIN T_ANA_DISTRETTI on DIS_CODICE = CNS_DIS_CODICE
LEFT JOIN T_ANA_USL on USL_CODICE = DIS_USL_CODICE";
			}
		}

	}
}
