namespace Onit.OnAssistnet.OnVac.Queries.VaccinazioniEscluse
{

	/// <summary>
	/// Queries vaccinazioni escluse
	/// </summary>
	public static class OracleQueries
	{

        /// <summary>
        /// Stringa con i campi delle query di selezione, la tabella e i join
        /// </summary>
        private static string selectFromVaccinazioniEscluse
        {
            get
            {
                return @"select t_vac_escluse.*, moe_descrizione, ope_nome from t_vac_escluse 
                            left join t_ana_motivi_esclusione on vex_moe_codice = moe_codice
                            left join t_ana_operatori on vex_ope_codice = ope_codice ";
            }
        }
        
        /// <summary>
        /// Restituisce il numero di vaccinazioni escluse relative al paziente specificato
        /// </summary>
        public static string countEsclusePaziente
        {
            get
            {
                return "select count(*) from t_vac_escluse where vex_paz_codice = :cod_paz";
            }
        }

        public static string selNextSeqEsclusa
        {
            get
            {
                return @"select seq_escluse.nextval from dual";
            }
        }
        
        /// <summary>
        /// Recupero vaccinazioni escluse per il paziente specificato
        /// </summary>
        public static string selVaccinazioniEsclusePaziente
        {
            get 
            {
                return selectFromVaccinazioniEscluse + @" where vex_paz_codice = :pazCodice";
            }
        }

        /// <summary>
        /// Restituisce la vaccinazione esclusa per il paziente specificato
        /// </summary>
        public static string selVaccinazioneEsclusaPaziente
        {
            get
            {
                return selectFromVaccinazioniEscluse + @" where vex_paz_codice = :pazCodice
and vex_vac_codice = :vacCodice
order by vex_data_visita, vex_vac_codice, vex_data_scadenza, moe_descrizione";
            }
        }

        /// <summary>
        /// Restituisce la vaccinazione esclusa corrispondente al vex_id specificato
        /// </summary>
        public static string selVaccinazioneEsclusaById
        {
            get
            {
                return selectFromVaccinazioniEscluse + @" where vex_id = :vex_id";
            }
        }
        
        /// <summary>
        /// Restituisce la vaccinazione esclusa eliminata corrispondente al vxe_id specificato
        /// </summary>
        public static string selVaccinazioneEsclusaEliminataById
        {
            get
            {
                return @"select t_vac_escluse_eliminate.* from t_vac_escluse_eliminate where vxe_id=:vxe_id";
            }
        }

        /// <summary>
        /// Restituisce le vaccinazioni escluse eliminate filtrate per paziente e per vaccinazione, in modo da creare uno storico dei rinnovi della vaccinazione esclusa
        /// </summary>
        public static string selVaccinazioneEsclusaEliminataByPazienteVaccinazione
        {
            get
            {
                return @"select t_vac_escluse_eliminate.*, vac_descrizione , ope_nome, ute_reg.ute_descrizione ute_registrazione, ute_eli.ute_descrizione ute_eliminazione,  ute_var.ute_descrizione ute_modifica from t_vac_escluse_eliminate 
                        join t_ana_vaccinazioni on vac_codice = vxe_vac_codice 
                        left join t_ana_operatori on ope_codice = vxe_ope_codice 
                        left join t_ana_utenti ute_reg on ute_reg.ute_id=vxe_ute_id_registrazione
                        left join t_ana_utenti ute_eli on ute_eli.ute_id=vxe_ute_id_eliminazione
                        left join t_ana_utenti ute_var on ute_var.ute_id=vxe_ute_id_variazione
                        where vxe_paz_codice = :vxe_paz_codice 
                          and vxe_vac_codice = nvl(:vxe_vac_codice,vxe_vac_codice) ";
            }
        }

        /// <summary>
        /// Inserimento esclusione
        /// </summary>
        public static string insEsclusione
		{
			get
			{
                return @"insert into t_vac_escluse(vex_id, vex_paz_codice, vex_dose, vex_data_visita, vex_vac_codice, vex_ope_codice, vex_moe_codice, vex_data_scadenza, vex_usl_inserimento, vex_data_registrazione, vex_ute_id_registrazione, vex_data_variazione, vex_ute_id_variazione, vex_flag_visibilita, vex_paz_codice_old, vex_note)
                            VALUES(:id, :cod_paz, :dose, :data_visita, :cod_vac, :cod_ope, :cod_motivo, :data_scadenza, :usl_inserimento, :data_registrazione, :ute_registrazione, :data_variazione, :ute_variazione, :flag_visibilita, :cod_paz_old, :note)";
			}
		}

        /// <summary>
        /// Inserimento esclusione eliminata
        /// </summary>
        public static string insEsclusioneEliminata
        {
            get
            {
                return @"insert into t_vac_escluse_eliminate(vxe_id, vxe_paz_codice, vxe_dose, vxe_data_visita, vxe_vac_codice, vxe_ope_codice, vxe_moe_codice, vxe_data_scadenza, vxe_usl_inserimento, vxe_flag_visibilita, vxe_data_eliminazione, vxe_ute_id_eliminazione,  vxe_data_registrazione, vxe_ute_id_registrazione, vxe_data_variazione, vxe_ute_id_variazione, vxe_paz_codice_old, vxe_stato_eliminazione, vxe_note)
                            VALUES(:id, :cod_paz, :dose, :data_visita, :cod_vac, :cod_ope, :cod_motivo, :data_scadenza, :usl_inserimento, :flag_visibilita, :data_eliminazione, :ute_eliminazione, :data_registrazione, :ute_registrazione, :data_variazione, :ute_variazione, :cod_paz_old, :stato_eliminazione, :note)";
            }
        }

        /// <summary>
        /// Aggiornamento esclusione
        /// </summary>
        public static string updEsclusione
        {
            get
            {
                return @"update t_vac_escluse 
                        set vex_data_visita = :data_visita, 
                        vex_ope_codice = :cod_ope, 
                        vex_dose = :dose,
                        vex_moe_codice = :cod_motivo, 
                        vex_data_scadenza = :data_scadenza,
                        vex_flag_visibilita = :flag_visibilita,
                        vex_usl_inserimento = :usl_inserimento,
                        vex_data_registrazione = :data_registrazione, 
                        vex_ute_id_registrazione = :ute_registrazione, 
                        vex_data_variazione = :data_variazione,
                        vex_ute_id_variazione = :ute_variazione,
                        vex_id = :id,
                        vex_note = :note
                        WHERE vex_paz_codice = :cod_paz 
                        AND vex_vac_codice = :cod_vac";
            }
        }

        /// <summary>
		/// Restituisce 1 se esiste una esclusione relativamente alla vaccinazione specificata
		/// </summary>
        public static string existsEsclusione
		{
			get
			{
                return @"select 1 from t_vac_escluse
                        where vex_paz_codice = :cod_paz
                        and vex_vac_codice = :cod_vac";
			}
		}

		/// <summary>
		/// Restituisce 1 se esiste una esclusione non scaduta rispetto alla data di convocazione
		/// </summary>
		public static string selExistsEsclusioneNonScaduta
		{  
			get
			{
				return @"select 1 from t_vac_escluse
where vex_paz_codice = :cod_paz
and vex_vac_codice = :cod_vac
and (vex_data_scadenza > :data_cnv or vex_data_scadenza is null)";
			}
		}

		/// <summary>
		/// Restituisce la data di scadenza dell'esclusione, se esiste un'esclusione non scaduta rispetto alla data di convocazione.
		/// </summary>
		public static string selDataScadenzaEsclusioneNonScaduta
		{   
			get
			{
				return @"select vex_data_scadenza from t_vac_escluse
where vex_paz_codice = :cod_paz
and vex_vac_codice = :cod_vac
and (vex_data_scadenza > :data_cnv or vex_data_scadenza is null)";
			}
		}

        /// <summary>
        /// Cancellazione esclusione
        /// </summary>
        public static string delEsclusione
        {
            get 
            {
                return @"delete from t_vac_escluse where vex_id=:id";
            }
        }

        /// <summary>
        /// Cancellazione esclusione
        /// </summary>
        public static string delAllEsclusione
        {
            get
            {
                return @"delete from t_vac_escluse where vex_paz_codice = :codicePaziente";
            }
        }

        /// <summary>
        /// Restituisce i dati della Vaccinazione Esclusa centrale in base all'id locale e al codice della usl
        /// </summary>
        public static string selVaccinazioneEsclusaCentraleByIdLocale
        {
            get
            {
                return @"select T_ESCLUSE_CENTRALE.* 
from T_ESCLUSE_DISTRIBUITE join T_ESCLUSE_CENTRALE on EXD_EXC_ID = EXC_ID 
where EXD_VEX_ID = :idVaccinazioneEsclusa 
and EXD_USL_CODICE = :codiceUsl";
            }
        }
        	
    }
}

