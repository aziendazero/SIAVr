namespace Onit.OnAssistnet.OnVac.Queries.Medici
{
	/// <summary>
	/// Queries relative ai medici.
	/// </summary>
	public static class OracleQueries
	{

        /// <summary>
        /// 
        /// </summary>
		public static string selMedicoByCodice
		{
			get
			{
				return @"select * from t_ana_medici left join t_ana_tipi_medico on med_tipo = tme_codice where med_codice = :cod_medico";
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public static string selCodiciMediciAbilitatiByCodiceAndDate
		{
			get
			{
				return @"select map_med_codice_abilitato from t_ana_medici, t_med_abilitazioni_vis_paz where med_codice = map_med_codice_medico and map_data_inizio <= :data and map_data_fine >= :data and med_codice = :cod_medico";
			}
		}

        /// <summary>
        /// 
        /// </summary>
        public static string insMedico
		{
			get
			{
                return @"insert into t_ana_medici(med_codice, med_codice_regionale, med_codice_fiscale, med_nome, med_cognome, med_descrizione) values(:med_codice, :med_codice_regionale, :med_codice_fiscale, :med_nome, :med_cognome, :med_descrizione)";
			}
		}


        /// <summary>
        /// 
        /// </summary>
		public static string selMedicoByCodiceFiscale
        {
            get
            {
                return @"select * from t_ana_medici where med_codice_fiscale = :med_codice_fiscale";
            }
        }

    }
}
