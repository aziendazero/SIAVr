namespace Onit.OnAssistnet.OnVac.Queries.Cicli
{
	/// <summary>
	/// Queries cicli.
	/// </summary>
	public static class OracleQueries
	{
        /// <summary>
        /// Caricamento cicli
        /// </summary>
        public static string selCicli
        {
            get
            {
                return @"select cic_codice, cic_descrizione, cic_data_introduzione, cic_standard, cic_n_sedute, cic_eta, cic_data_fine, cic_alert, cic_sesso from t_ana_cicli order by cic_descrizione";
            }
        }

        /// <summary>
        /// Caricamento cicli del paziente specificato
        /// </summary>
        public static string selCicliPaziente
        {
            get
            {
                return @"select pac_paz_codice, pac_paz_codice_old, pac_cic_codice, cic_descrizione, cic_standard
from t_paz_cicli join t_ana_cicli on pac_cic_codice = cic_codice
where pac_paz_codice = :pazCodice
order by pac_cic_codice";
            }
        }
             
		/// <summary>
		/// Inserimento ciclo per il paziente
		/// </summary>
		public static string insCicloPaziente
		{
			get
			{
				return @"insert into t_paz_cicli 
(pac_paz_codice, pac_cic_codice) 
values 
(:cod_paz, :cod_cic)";
			}
		}

        /// <summary>
        /// Inserimento dei cicli standard, in base a data di nascita e sesso.
        /// </summary>
        public static string selCicliStandard
        {
            get
            {
                return @"select cic_codice from t_ana_cicli
where cic_standard = 'T'
and cic_data_introduzione <= :data_nascita
and (cic_data_fine > :data_nascita or cic_data_fine is null) 
and (cic_sesso = :sesso or cic_sesso = 'E')";
            }
        }

        /// <summary>
        /// Conteggio cicli incompatibili per data di nascita e sesso
        /// </summary>
        public static string countCicliIncompatibili
        {
            get
            {
                return @"SELECT COUNT(*)       
                        FROM T_ANA_CICLI,T_PAZ_CICLI
                        WHERE CIC_CODICE = PAC_CIC_CODICE
                        AND PAC_PAZ_CODICE = :paz_codice
                        AND (:data_nascita < CIC_DATA_INTRODUZIONE
                             OR (:data_nascita > cic_data_fine AND cic_data_fine IS NOT NULL)
                             OR (CIC_SESSO <> :sesso AND CIC_SESSO <> 'E'))";
            }
        }

        /// <summary>
        /// Conteggio cicli incompatibili per data di nascita
        /// </summary>
        public static string countCicliIncompatibiliSenzaSesso
        {
            get
            {
                return @"SELECT COUNT(*)       
                        FROM T_ANA_CICLI,T_PAZ_CICLI
                        WHERE CIC_CODICE = PAC_CIC_CODICE
                        AND PAC_PAZ_CODICE = :paz_codice
                        AND (:data_nascita < CIC_DATA_INTRODUZIONE
                             OR (:data_nascita > cic_data_fine AND cic_data_fine IS NOT NULL))";
            }
        }

		/// <summary>
		/// Cancellazione ciclo per il paziente
		/// </summary>
		public static string delCicloPaziente
		{
			get
			{
				return @"delete from t_paz_cicli
where pac_paz_codice = :cod_paz
and pac_cic_codice = :cod_cic";
			}
		}

		/// <summary>
		/// Controllo se ci sono vaccinazioni in comune tra il ciclo specificato e quelli associati al paziente (in base alla t_ana_vaccinazioni_sedute)
		/// </summary>
		public static string selExistsVaccinazioniComuni
		{
			get
			{
				return @"select distinct 1 
from t_ana_vaccinazioni_sedute 
where sed_cic_codice in ( 
    select pac_cic_codice 
    from t_paz_cicli 
    where pac_paz_codice = :cod_paz) 
and sed_vac_codice in ( 
    select distinct sed_vac_codice 
    from t_ana_vaccinazioni_sedute 
    where sed_cic_codice = :cod_cic)";
			}
		}

		/// <summary>
		/// Controllo se ci sono vaccinazioni in comune tra il ciclo specificato e quelli associati al paziente (in base alla t_ana_associazioni_sedute)
		/// </summary>
		public static string selExistsVaccinazioniComuniAssociazioni
		{
			get
			{
				return @"select distinct 1 
from t_ana_associazioni_sedute 
where sas_cic_codice in ( 
    select pac_cic_codice 
    from t_paz_cicli 
    where pac_paz_codice = :cod_paz) 
and sas_vac_codice in ( 
    select distinct sas_vac_codice 
    from t_ana_associazioni_sedute 
    where sas_cic_codice = :cod_cic)";
			}
		}

        /// <summary>
        /// Controlla se esiste in programmazione la vaccinazione con il richiamo specificato 
        /// </summary>
        public static string selExistsCicloConVaccinazione
        {
            get
            {
                return @"  SELECT   pac_cic_codice
    FROM   t_paz_pazienti,
           t_paz_cicli,
           t_ana_tempi_sedute,
           t_ana_vaccinazioni_sedute vacSed2
   WHERE       paz_codice = :paz_codice
           AND pac_paz_codice = paz_codice
           AND pac_cic_codice = tsd_cic_codice
           AND tsd_cic_codice = vacsed2.sed_cic_codice
           AND tsd_n_seduta = vacsed2.sed_n_seduta
           AND NOT EXISTS
                 (SELECT   1
                    FROM   t_vac_eseguite
                   WHERE   ves_paz_codice = :paz_codice
                           AND EXISTS
                                 (SELECT   1
                                    FROM   v_ana_ass_vacc_sedute vacsed1
                                   WHERE   vacsed1.sas_cic_codice =
                                              vacsed2.sed_cic_codice
                                           AND vacsed1.sas_n_seduta =
                                                 vacsed2.sed_n_seduta
                                           AND vacsed1.sas_vac_codice =
                                                 ves_vac_codice
                                           AND vacsed1.sas_n_richiamo =
                                                 ves_n_richiamo))
           AND NOT EXISTS
                 (SELECT   1
                    FROM   t_vac_programmate
                   WHERE       vpr_paz_codice = :paz_codice
                           AND vpr_vac_codice = vacsed2.sed_vac_codice
                           AND vpr_n_richiamo = vacsed2.sed_n_richiamo)";
            }
        }

        /// <summary>
        /// Recupero sito e via somministrazione relativi a ciclo-seduta-vaccinazione specificati (utilizza la v_ana_ass_vacc_sedute)
        /// </summary>
        public static string selSitoViaByCicloSedutaVaccinazione
        {
            get 
            {
                return @"SELECT SAS_SII_CODICE, SII_DESCRIZIONE, SAS_VII_CODICE, VII_DESCRIZIONE
FROM V_ANA_ASS_VACC_SEDUTE 
  LEFT JOIN T_ANA_SITI_INOCULAZIONE ON SAS_SII_CODICE = SII_CODICE
  LEFT JOIN T_ANA_VIE_SOMMINISTRAZIONE ON SAS_VII_CODICE = VII_CODICE
WHERE SAS_CIC_CODICE = :sas_cic_codice
AND SAS_N_SEDUTA = :sas_n_seduta
AND SAS_VAC_CODICE = :sas_vac_codice";
            }
        }

        /// <summary>
        /// Restituisce le informazioni sui cicli relativi alla categoria di rischio specificata
        /// </summary>
        public static string selCicliByCategoriaRischio
        {
            get
            {
                return @"SELECT CIC_CODICE, CIC_DESCRIZIONE, CIC_DATA_INTRODUZIONE, CIC_DATA_FINE, COD_DESCRIZIONE
FROM T_ANA_CICLI 
  LEFT JOIN T_ANA_LINK_RISCHIO_CICLI ON CIC_CODICE = RCI_CIC_CODICE
  LEFT JOIN T_ANA_CODIFICHE ON CIC_STANDARD = COD_CODICE
WHERE COD_CAMPO = 'CIC_STANDARD'
AND RCI_RSC_CODICE = :rci_rsc_codice";
            }
        }

        /// <summary>
        /// Restituisce i codici dei cicli del paziente non compatibili con il ciclo specificato
        /// </summary>
        public static string selCicliIncompatibiliPazByCiclo
        {
            get
            {
                return @"select distinct sas_cic_codice
from t_ana_associazioni_sedute 
where sas_cic_codice in ( 
    select pac_cic_codice 
    from t_paz_cicli 
    where pac_paz_codice = :cod_paz) 
and sas_vac_codice in ( 
    select distinct sas_vac_codice 
    from t_ana_associazioni_sedute 
    where sas_cic_codice = :cod_cic)
union
select distinct sed_cic_codice 
from t_ana_vaccinazioni_sedute 
where sed_cic_codice in ( 
    select pac_cic_codice 
    from t_paz_cicli 
    where pac_paz_codice = :cod_paz) 
and sed_vac_codice in ( 
    select distinct sed_vac_codice 
    from t_ana_vaccinazioni_sedute 
    where sed_cic_codice = :cod_cic)";
            }
        }
    }
}
