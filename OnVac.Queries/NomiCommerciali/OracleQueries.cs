namespace Onit.OnAssistnet.OnVac.Queries.NomiCommerciali
{
    /// <summary>
    /// Query oracle relative ai nomi commerciali.
    /// </summary>
    public static class OracleQueries
    {
        /// <summary>
        /// Recupero sito e via somministrazione relativi al nome commerciale specificato
        /// </summary>
        public static string selSitoViaByNomeCommerciale
        {
            get
            {
                return @"SELECT NOC_SII_CODICE, SII_DESCRIZIONE, NOC_VII_CODICE, VII_DESCRIZIONE
FROM T_ANA_NOMI_COMMERCIALI 
  LEFT JOIN T_ANA_SITI_INOCULAZIONE ON NOC_SII_CODICE = SII_CODICE
  LEFT JOIN T_ANA_VIE_SOMMINISTRAZIONE ON NOC_VII_CODICE = VII_CODICE
WHERE NOC_CODICE = :noc_codice";
            }
        }

    }
}