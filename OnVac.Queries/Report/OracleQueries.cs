namespace Onit.OnAssistnet.OnVac.Queries.Report
{
	/// <summary>
	/// Descrizione di riepilogo per OracleQueries.
	/// </summary>
	public static class OracleQueries
	{

        /// <summary>
        /// 
        /// </summary>
		public static string selReport
		{
			get 
			{
				return @"select rpt_nome, rpt_installazione, rpt_cartella, rpt_dataset 
from t_ana_report
where rpt_nome = :nome_rpt";
			}
		}

        /// <summary>
        /// 
        /// </summary>
        public static string selReportList
        {
            get
            {
                return @"select rpt_nome, rpt_installazione, rpt_cartella, rpt_dataset 
from t_ana_report
where rpt_nome in ({0})";
            }
        }

        /// <summary>
        /// 
        /// </summary>
		public static string selDatiInstallazione
		{
			get 
			{
				return @"select * from t_ana_installazioni where ins_usl_codice = :ins_usl_codice";
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public static string selReportFolder
		{
			get 
			{
				return @"select rpt_cartella from t_ana_report where rpt_nome = :nome_rpt";
			}
		}

	}
}