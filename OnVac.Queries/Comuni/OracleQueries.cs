namespace Onit.OnAssistnet.OnVac.Queries.Comuni
{
	/// <summary>
	/// Queries relative ai comuni.
	/// </summary>
	public static class OracleQueries
	{

        /// <summary>
        /// Restituisce il codice del comune in base al codice istat e al periodo di validità. 
        /// Il comune dovrebbe essere univoco, se l'anagrafica non contiene periodi sovrapposti.
        /// </summary>
        public static string selCodComuneByIstatStorico
        {
            get
            {
                return @"select com_codice from t_ana_comuni where com_istat = :codiceIstat
and (COM_DATA_INIZIO_VALIDITA is null or COM_DATA_INIZIO_VALIDITA <= :dataValidita) 
and (COM_DATA_FINE_VALIDITA is null or COM_DATA_FINE_VALIDITA >= :dataValidita)";
            }
        }

        /// <summary>
        /// Seleziona tutti i comuni aventi il codice Istat specificato, senza considerare il periodo di validità
        /// </summary>
        public static string selCodComuniByIstat
        {
            get
            {
                return @"select com_codice from t_ana_comuni where com_istat = :codiceIstat";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string selIstatByCodice
        {
            get
            {
                return @"select com_istat from t_ana_comuni where com_codice = :com_codice";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string selCapByCodice
        {
            get
            {
                return @"select com_cap from t_ana_comuni where com_codice = :com_codice";
            }
        }

        /// <summary>
        /// 
        /// </summary>
		public static string chkValiditaComune
		{
			get
			{
				return @"select 1
from t_ana_comuni
where com_codice = :cod_com
and (com_data_inizio_validita is null or com_data_inizio_validita <= :data_val)
and (com_data_fine_validita is null or com_data_fine_validita >= :data_val)
and (com_obsoleto is null or com_obsoleto = 'N')";
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public static string selComuneByCodiceInterno
		{
			get 
			{
                return @"select com_codice, com_codice_esterno,com_descrizione, com_provincia, com_istat, com_catastale, com_cap,
com_data_inizio_validita, com_data_fine_validita, com_obsoleto, com_scadenza, com_motivo_scadenza
from t_ana_comuni
where com_codice = :comCodice"; 
			}
		}

    }
}
