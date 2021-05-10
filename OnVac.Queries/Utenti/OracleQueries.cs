namespace Onit.OnAssistnet.OnVac.Queries.Utenti
{
	/// <summary>
	/// Descrizione di riepilogo per OracleQueries.
	/// </summary>
	public static class OracleQueries
	{

        /// <summary>
        /// Seleziona tutti i dati dell'utente specificato
        /// </summary>
		public static string selUtente
		{
			get
			{
				return @"select * from t_ana_utenti where ute_id = :uteId";
			}
		}

		/// <summary>
		/// Lettura del codice del medico,
		/// se l'utente è associato a un medico tramite il codice esterno dell'operatore
		/// </summary>
		public static string sel_medico_da_codice_esterno
		{
			get 
			{
				return @"SELECT med_codice
  FROM t_ana_medici, t_ana_operatori, t_ana_utenti
 WHERE ute_id = :id
   AND UPPER (ute_codice) = UPPER (ope_codice_esterno)
   AND ope_codice = med_codice";
			}
		}

        /// <summary>
        /// Restituisce i consultori abilitati all'utente specificato
        /// </summary>
        public static string sel_consultori_utente 
        {
            get
            {
                return @"SELECT LUC_UTE_ID, LUC_CNS_CODICE, LUC_DEFAULT, CNS_DESCRIZIONE
FROM T_ANA_LINK_UTENTI_CONSULTORI 
JOIN T_ANA_CONSULTORI on LUC_CNS_CODICE = CNS_CODICE
WHERE LUC_UTE_ID = :LUC_UTE_ID
ORDER BY CNS_DESCRIZIONE, CNS_CODICE";
            }
        }

	}
}
