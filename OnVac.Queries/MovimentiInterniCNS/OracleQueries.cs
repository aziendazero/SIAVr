namespace Onit.OnAssistnet.OnVac.Queries.MovimentiInterniCNS
{
    /// <summary>
    /// Queries movimenti interni
    /// </summary>
    public static class OracleQueries
    {

        /// <summary>
        /// Inserimento movimento
        /// </summary>
        public static string insMovimentoPaziente
        {
            get
            {
                return @"insert into t_cns_movimenti 
(cnm_paz_codice, cnm_cns_codice_old, cnm_cns_codice_new, cnm_data, cnm_invio_cartella, cnm_presa_visione, cnm_auto_adulti) 
values 
(:cnm_paz_codice, :cnm_cns_codice_old, :cnm_cns_codice_new, :cnm_data, :cnm_invio_cartella, :cnm_presa_visione, :cnm_auto_adulti)";
            }
        }

        /// <summary>
        /// Update campo presa visione
        /// </summary>
        public static string updPresaVisionePaziente
        {
            get
            {
                return @"update t_cns_movimenti 
set cnm_presa_visione = :cnm_presa_visione
where cnm_progressivo = :cnm_progressivo";
            }
        }

        /// <summary>
        /// Update campo invio cartella
        /// </summary>
        public static string updInviaCartellaPaziente
        {
            get 
            {
                return @"update t_cns_movimenti
set cnm_invio_cartella = :cnm_invio_cartella 
where cnm_progressivo = :cnm_progressivo";
            }
        }

    }
}
