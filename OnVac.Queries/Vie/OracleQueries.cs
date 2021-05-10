using System;

namespace Onit.OnAssistnet.OnVac.Queries.Vie
{
	/// <summary>
	/// Descrizione di riepilogo per OracleQueries.
	/// </summary>
	public static class OracleQueries
	{

        /// <summary>
        /// Sequence indirizzi
        /// </summary>
        public static string selNextSeqIndirizzi
        {
            get
            {
                return @"SELECT SEQ_PAZ_INDIRIZZI.NEXTVAL FROM DUAL";
            }
        }

        /// <summary>
        /// Restituisce le descrizioni delle vie associate al codice specificato
        /// </summary>
        public static string selDescrizioniVieByCodice
        {
            get
            {
                return @"SELECT DISTINCT VIA_DESCRIZIONE FROM T_ANA_VIE WHERE VIA_CODICE = :via_codice";
            }
        }

        /// <summary>
        /// Inserimento indirizzo codificato
        /// </summary>
        public static string insIndirizzoPaziente
        {
            get
            {
                return @"INSERT INTO T_PAZ_INDIRIZZI (IND_CODICE, IND_VIA_CODICE, IND_N_CIVICO, IND_INTERNO, IND_LOTTO, IND_PALAZZINA, IND_SCALA, IND_PIANO, IND_CIVICO_LETTERA)
                         VALUES (:ind_codice, :ind_via_codice, :ind_n_civico, :ind_interno, :ind_lotto, :ind_palazzina, :ind_scala, :ind_piano, :ind_civico_lettera)";
            }
        }

        /// <summary>
        /// Update indirizzo codificato
        /// </summary>
        public static string updIndirizzoPaziente
        {
            get
            {
                return @"UPDATE T_PAZ_INDIRIZZI 
                         SET IND_VIA_CODICE = :ind_via_codice, 
                         IND_N_CIVICO = :ind_n_civico, 
                         IND_INTERNO = :ind_interno,
                         IND_LOTTO = :ind_lotto,
                         IND_PALAZZINA = :ind_palazzina,
                         IND_SCALA = :ind_scala,
                         IND_PIANO = :ind_piano,
                         IND_CIVICO_LETTERA = :ind_civico_lettera
                         WHERE IND_CODICE = :ind_codice";
            }
        }

        /// <summary>
        /// Cancellazione indirizzo codificato
        /// </summary>
        public static string delIndirizzoPaziente
        {
            get
            {
                return @"DELETE FROM T_PAZ_INDIRIZZI WHERE IND_CODICE = :ind_codice";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string selVieByComuneAndCodice
        {
            get
            {
                return @"select * from t_ana_vie where via_com_codice = :via_com_codice and via_codice = :via_codice";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string selVieByCap
        {
            get
            {
                return @"select * from t_ana_vie where via_cap = :via_cap";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string selVieByDistretto
		{
			get
			{
                return @"select * from t_ana_vie where via_dis_codice = :via_dis_codice";
			}
		}

        /// <summary>
        /// 
        /// </summary>
        public static string existsDescrizioneDiversa
        {
            get
            {
                return @"select 1 from t_ana_vie where via_codice = :via_codice and via_com_codice = :via_com_codice and via_descrizione <> :via_descrizione";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string existsDescrizioneMultipla
        {
            get
            {
                return @"select 1 from t_ana_vie where via_codice = :via_codice and via_com_codice = :via_com_codice and via_descrizione <> :via_descrizione and via_progressivo <> :via_progressivo";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string existsDefault
        {
            get
            {
                return @"select 1 from t_ana_vie where via_codice = :via_codice and via_com_codice = :via_com_codice and via_default = 'S'";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string existsDefaultMultiplo
        {
            get
            {
                return @"select 1 from t_ana_vie where via_codice = :via_codice and via_com_codice = :via_com_codice and via_progressivo <> :via_progressivo and via_default = 'S'";
            }
        }

	}
}
