namespace Onit.OnAssistnet.OnVac.Queries.Cittadinanze
{
    /// <summary>
    /// Queries relative alle cittadinanze.
    /// </summary>
    public static class OracleQueries
    {

        /// <summary>
        /// Restituisce il codice della cittadinanza in base al codice istat e al periodo di validità. 
        /// La cittadinanza dovrebbe essere univoca, se l'anagrafica non contiene periodi sovrapposti.
        /// </summary>
        public static string selCodCittadinanzaByIstatStorico
        {
            get
            {
                return @"select CIT_CODICE from T_ANA_CITTADINANZE where CIT_ISTAT = :codiceIstat
and (CIT_DATA_INIZIO_VALIDITA is null or CIT_DATA_INIZIO_VALIDITA <= :dataValidita) 
and (CIT_SCADENZA is null or CIT_SCADENZA >= :dataValidita)";
            }
        }

        /// <summary>
        /// Seleziona tutte le cittadinanze aventi il codice Istat specificato, senza considerare il periodo di validità
        /// </summary>
        public static string selCodCittadinanzeByIstat
        {
            get
            {
                return @"select CIT_CODICE from T_ANA_CITTADINANZE where COM_ISTAT = :codiceIstat";
            }
        }

    }
}
