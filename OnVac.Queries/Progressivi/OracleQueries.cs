namespace Onit.OnAssistnet.OnVac.Queries.Progressivi
{
    /// <summary>
    /// Query progressivi.
    /// </summary>
    public static class OracleQueries
    {

        /// <summary>
        /// Inserimento progressivo
        /// </summary>
        public static string insProgressivo
        {
            get
            {
                return @"insert into t_ana_progressivi 
(prg_codice,prg_numero,prg_anno, prg_prefisso, prg_lunghezza, prg_max, prg_azi_codice)
values
(:codice, :progressivo, :anno, :prefisso, :lunghezza, :max, :azienda)";
            }
        }

        /// <summary>
        /// Update progressivo
        /// </summary>
        public static string updProgressivo
        {
            get
            {
                return @"update t_ana_progressivi
set prg_numero = :progressivo
where prg_codice = :codice and prg_anno = :anno";
            }
        }

        /// <summary>
        /// Update progressivo per lock record
        /// </summary>
        public static string lockProgressivo
        {
            get
            {
                return @"update t_ana_progressivi
                            set prg_numero = prg_numero
                            where prg_codice = :codice and prg_anno = :anno";
            }
        }

        /// <summary>
        /// Selezione progressivo in base a codice e anno (indice univoco)
        /// </summary>
        public static string selProgressivo
        {
            get
            {
                return @"select * from t_ana_progressivi where prg_codice = :codice and prg_anno = :anno";
            }
        }

        /// <summary>
        /// Selezione progressivi per codice, ordinati per anno descrescente
        /// </summary>
        public static string selProgressivi
        {
            get
            {
                return @"select * from t_ana_progressivi where prg_codice = :codice order by prg_anno desc";
            }
        }

    }
}
