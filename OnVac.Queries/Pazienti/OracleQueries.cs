namespace Onit.OnAssistnet.OnVac.Queries.Pazienti
{
	/// <summary>
	/// Query oracle relative ai pazienti.
	/// </summary>
	public static class OracleQueries
	{

        #region Sequence
        
        /// <summary>
        /// 
        /// </summary>
        public static string nextValSeqPazienti
        {
            get
            {
                return "select seq_pazienti.nextval from dual";
            }
        }

        #endregion

        #region Select

        #region Select Codice Paziente

        /// <summary>
        /// Restituisce i codici dei pazienti corrispondenti al CodiceAusiliario specificato
        /// </summary>
        public static string selCodicePazientiByCodiceAusiliario
        {
            get
            {
                return @"select paz_codice from t_paz_pazienti 
                            where  paz_codice_ausiliario = :paz_codice_ausiliario";
            }
        }

        /// <summary>
        /// Restituisce i codici dei pazienti corrispondenti al CodiceRegionale specificato
        /// </summary>
        public static string selCodicePazientiByCodiceRegionale
        {
            get
            {
                return @"select paz_codice from t_paz_pazienti 
                            where paz_codice_regionale = :paz_codice_regionale";
            }
        }

        /// <summary>
        /// Restituisce i codici dei pazienti corrispondenti alla Tessera specificata
        /// </summary>
        public static string selCodicePazientiByTessera
        {
            get
            {
                return @"select paz_codice from t_paz_pazienti 
                            where paz_tessera = :paz_tessera";
            }
        }

        /// <summary>
        /// Restituisce i codici dei pazienti corrispondenti al CodiceFiscale specificato
        /// </summary>
        public static string selCodicePazientiByCodiceFiscale
        {
            get
            {
                return @"select paz_codice from t_paz_pazienti 
                            where paz_codice_fiscale = :paz_codice_fiscale";
            }
        }

        /// <summary>
        /// Restituisce i codici dei pazienti corrispondenti ai Componenti del CodiceFiscale specificati
        /// </summary>
        public static string selCodicePazientiByComponentiCodiceFiscale
        {
            get
            {
                return @"select paz_codice from t_paz_pazienti 
                            where paz_nome = :paz_nome
                            and paz_cognome = :paz_cognome
                            and paz_data_nascita = :paz_data_nascita
                            and paz_sesso = :paz_sesso
                            and paz_com_codice_nascita = :paz_com_codice_nascita";
            }
        }

        #endregion
        
        #region Select Codice Comune
        
        /// <summary>
        /// 
        /// </summary>
		public static string selCodiceComuneResidenza
		{
			get
			{
				return @"select paz_com_codice_residenza from t_paz_pazienti where paz_codice = :cod_paz";
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public static string selCodiceComuneDomicilio
		{
			get
			{
				return @"select paz_com_codice_domicilio from t_paz_pazienti where paz_codice = :cod_paz";
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public static string selCodiceComuneEmigrazione
		{
			get
			{
				return @"select paz_com_comune_emigrazione from t_paz_pazienti where paz_codice = :cod_paz";
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public static string selCodiceComuneImmigrazione
		{
			get
			{
				return @"select paz_com_comune_provenienza from t_paz_pazienti where paz_codice = :cod_paz";
			}
		}

        #endregion

        /// <summary>
        /// Data di nascita del paziente
        /// </summary>
        public static string selDataNascita
        {
            get
            {
                return @"select paz_data_nascita from t_paz_pazienti where paz_codice = :cod_paz";
            }
        }

        /// <summary>
        /// Data di decesso del paziente
        /// </summary>
        public static string selDataDecesso
        {
            get
            {
                return @"select paz_data_decesso from t_paz_pazienti where paz_codice = :cod_paz";
            }
        }

        /// <summary>
        /// 
        /// </summary>
		public static string selDataImmigrazione
		{
			get
			{
				return @"select paz_data_immigrazione from t_paz_pazienti where paz_codice = :cod_paz";
			}
		}

        /// <summary>
        /// Codice ausiliario del paziente in base al codice
        /// </summary>
        public static string selCodiceAusiliario
        {
            get
            {
                return @"select paz_codice_ausiliario from t_paz_pazienti where paz_codice = :cod_paz";
            }
        }

        /// <summary>
        /// Codice regionale del paziente in base al codice
        /// </summary>
        public static string selCodiceRegionale
        {
            get
            {
                return @"select paz_codice_regionale from t_paz_pazienti where paz_codice = :cod_paz";
            }
        }

        /// <summary>
        /// 
        /// </summary>
		public static string selCodFiscaleDuplicato
		{
			get
			{
				return @"select paz_codice
from t_paz_pazienti
where paz_codice_fiscale = :cod_fisc
and paz_codice <> :cod_paz";
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public static string selCircoscrizioneResidenza
		{
			get
			{
				return @"select paz_cir_codice from t_paz_pazienti where paz_codice = :cod_paz";
			}
		}

        /// <summary>
        /// 
        /// </summary>
        public static string selCircoscrizioneDomicilio
        {
            get
            {
                return @"select paz_cir_codice_2 from t_paz_pazienti where paz_codice = :cod_paz";
            }
        }
        
        /// <summary>
        /// Restituisce il codice dell'indirizzo di residenza del paziente
        /// </summary>
        public static string selCodiceIndirizzoResidenza
        {
            get
            {
                return @"SELECT PAZ_IND_CODICE_RES FROM T_PAZ_PAZIENTI WHERE PAZ_CODICE = :paz_codice";
            }
        }
        
        /// <summary>
        /// Restituisce il codice dell'indirizzo di domicilio del paziente
        /// </summary>
        public static string selCodiceIndirizzoDomicilio
        {
            get
            {
                return @"SELECT PAZ_IND_CODICE_DOM FROM T_PAZ_PAZIENTI WHERE PAZ_CODICE = :paz_codice";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string selExists
        {
            get
            {
                return @"select 1
						   from t_paz_pazienti
						  where paz_codice = :cod_paz";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string selStatoVaccPaz
        {
            get
            {
                return @"select paz_stato from t_paz_pazienti where paz_codice = :cod_paz";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string selCodiceCns
        {
            get
            {
                return @"select paz_cns_codice from t_paz_pazienti where paz_codice = :cod_paz";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string selNuovoConsultorio
        {
            get
            {
                return @"SELECT rco_cns_codice cns_new, 'C' tipo
							FROM t_paz_pazienti,
								t_ana_consultori c1,
								t_ana_link_circoscrizioni_cns cir,
								t_ana_consultori c2
							WHERE paz_codice = :pazcodice
							AND cir.rco_cns_codice IS NOT NULL
							AND paz_cns_codice <> cir.rco_cns_codice
							AND (   (c1.cns_tipo = 'V' AND c2.cns_tipo = 'P')
									OR (c1.cns_tipo = 'P' AND c2.cns_tipo = 'A')
								)
							AND paz_cir_codice = cir.rco_cir_codice(+)
							AND paz_cns_codice = c1.cns_codice
							AND cir.rco_cns_codice = c2.cns_codice(+)
							UNION
							SELECT res.cco_cns_codice cns_new, 'R' tipo
							FROM t_paz_pazienti,
								t_ana_consultori c1,
								t_ana_link_comuni_consultori res,
								t_ana_consultori c3
							WHERE paz_codice = :pazcodice
							AND res.cco_cns_codice IS NOT NULL
							AND paz_cns_codice <> res.cco_cns_codice
							AND (   (c1.cns_tipo = 'V' AND c3.cns_tipo = 'P')
									OR (c1.cns_tipo = 'P' AND c3.cns_tipo = 'A')
								)
							AND paz_com_codice_residenza = res.cco_com_codice(+)
							AND paz_cns_codice = c1.cns_codice
							AND res.cco_cns_codice = c3.cns_codice(+)
							UNION
							SELECT dom.cco_cns_codice cns_new, 'D' tipo
							FROM t_paz_pazienti,
								t_ana_consultori c1,
								t_ana_link_comuni_consultori dom,
								t_ana_consultori c4
							WHERE paz_codice = :pazcodice
							AND dom.cco_cns_codice IS NOT NULL
							AND paz_cns_codice <> dom.cco_cns_codice
							AND (   (c1.cns_tipo = 'V' AND c4.cns_tipo = 'P')
									OR (c1.cns_tipo = 'P' AND c4.cns_tipo = 'A')
								)
							AND paz_com_codice_domicilio = dom.cco_com_codice(+)
							AND paz_cns_codice = c1.cns_codice
							AND dom.cco_cns_codice = c4.cns_codice(+)";
            }
        }

        #region Pazienti Adulti

        /// <summary>
        /// 
        /// </summary>
        public static string selPazientiAdultiCambioConsultorio
        {
            get
            {
                return @"SELECT paz_codice, paz_data_nascita, paz_cns_codice, paz_cir_codice, paz_com_codice_residenza
						   FROM t_paz_pazienti
						  WHERE (paz_cns_codice = :cnscodice OR paz_cns_codice IS NULL)";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string selPazientiAdultiCambioConsultorio_Condition1
        {
            get
            {
                return @" AND NOT EXISTS (
							SELECT 1
								FROM t_ana_link_circoscrizioni_cns, t_ana_consultori
							WHERE rco_cns_codice = cns_codice
								AND paz_data_nascita <= (SYSDATE - cns_da_eta)
								AND paz_data_nascita >= (SYSDATE - cns_a_eta)
								AND paz_cns_codice = rco_cns_codice)";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string selPazientiAdultiCambioConsultorio_Condition2
        {
            get
            {
                return @" AND NOT EXISTS (
							SELECT 1
								FROM t_ana_link_comuni_consultori, t_ana_consultori
							WHERE cco_cns_codice = cns_codice
								AND paz_data_nascita <= (SYSDATE - cns_da_eta)
								AND paz_data_nascita >= (SYSDATE - cns_a_eta)
								AND paz_cns_codice = cco_cns_codice)";
            }
        }

        #endregion
        
        #region Pazienti Fuori Età

        /// <summary>
        /// Seleziona tutti i pazienti non deceduti fuori età rispetto al consultorio di appartenenza. Query principale.
        /// </summary>
        public static string selPazientiFuoriEtaCns
        {
            get
            {
                return @"SELECT paz_codice FROM t_paz_pazienti, t_ana_consultori
						WHERE paz_cns_codice = cns_codice
							AND paz_data_nascita < (SYSDATE - (cns_a_eta * 365 / 360))
							AND paz_data_decesso IS NULL 
							AND paz_stato_anagrafico <> '9'
							AND paz_cns_codice = :cnscodice
							AND cns_codice = :cnscodice";
            }
        }

        /// <summary>
        /// Filtro della query di selezione pazienti fuori età per escludere pazienti con appuntamenti
        /// </summary>
        public static string selPazientiFuoriEtaCns_Condition1
        {
            get
            {
                return @" AND NOT EXISTS (
							SELECT 1
								FROM t_cnv_convocazioni
							WHERE paz_codice = cnv_paz_codice
								AND cnv_data_appuntamento > SYSDATE) ";
            }
        }

        /// <summary>
        /// Filtro della query di selezione pazienti fuori età per controllo dello stato anagrafico
        /// </summary>
        public static string selPazientiFuoriEtaCns_Condition2
        {
            get
            {
                return @" AND paz_stato_anagrafico IN (:statoanagrafico) ";
            }
        }

        /// <summary>
        /// Filtro della query di selezione pazienti fuori età per controllo del sesso
        /// </summary>
        public static string selPazientiFuoriEtaCns_Condition3
        {
            get
            {
                return @" AND paz_sesso = :sesso ";
            }
        }

        /// <summary>
        /// Filtro della query di selezione pazienti fuori età per controllo data di nascita minima
        /// </summary>
        public static string selPazientiFuoriEtaCns_Condition4
        {
            get
            {
                return @" AND paz_data_nascita >= :datanascitada ";
            }
        }

        /// <summary>
        /// Filtro della query di selezione pazienti fuori età per controllo data di nascita massima
        /// </summary>
        public static string selPazientiFuoriEtaCns_Condition5
        {
            get
            {
                return @" AND paz_data_nascita <= :datanascitaa ";
            }
        }

        #endregion

        #region Controllo campi paziente

        /// <summary>
        /// Recupero controlli sui campi del paziente
        /// </summary>
        public static string selControlliCampiPaziente
        {
            get
            {
                return @"select * from T_ANA_CONTROLLO_CAMPI_PAZIENTE where CDP_FUNZIONE = :funzione AND CDP_TIPO <> :tipo";
            }
        }

        #endregion
                
        #endregion

        #region Count Dati Vaccinali

        /// <summary>
        /// Conteggio vaccinazioni eseguite del paziente specificato
        /// </summary>
        public static string cntVaccinazioniEseguitePaziente
        {
            get
            {
                return @"select count(*) from t_vac_eseguite where ves_paz_codice = :cod_paz";
            }
        }

        /// <summary>
        /// Conteggio vaccinazioni scadute del paziente specificato
        /// </summary>
        public static string cntVaccinazioniScadutePaziente
        {
            get
            {
                return @"select count(*) from t_vac_scadute where vsc_paz_codice = :cod_paz";
            }
        }

        /// <summary>
        /// Conteggio vaccinazioni escluse del paziente specificato
        /// </summary>
        public static string cntVaccinazioniEsclusePaziente
        {
            get
            {
                return @"select count(*) from t_vac_escluse where vex_paz_codice = :cod_paz";
            }
        }

        /// <summary>
        /// Conteggio visite con sospensione del paziente specificato
        /// </summary>
        public static string cntVisiteSospensionePaziente
        {
            get
            {
                return @"select count(*) from t_vis_visite where vis_paz_codice = :cod_paz and vis_fine_sospensione is not null";
            }
        }

        #endregion

        #region Insert
        
        /// <summary>
        /// Query di insert del paziente
        /// </summary>
        public static string insertPaziente
        {
            get
            {
                return @"INSERT INTO t_paz_pazienti (paz_codice,
                            paz_cognome,
                            paz_nome,
                            paz_data_nascita,
                            paz_codice_fiscale,
                            paz_indirizzo_residenza,
                            paz_indirizzo_domicilio,
                            paz_sesso,
                            paz_med_codice_base,
                            paz_cns_codice,
                            paz_com_codice_residenza,
                            paz_cap_residenza,
                            paz_data_inizio_residenza,
                            paz_data_fine_residenza,
                            paz_com_codice_domicilio,
                            paz_cap_domicilio,
                            paz_data_inizio_domicilio,
                            paz_data_fine_domicilio,
                            paz_flag_cessato,
                            paz_aire,
                            paz_cir_codice,
                            paz_cir_codice_2,
                            paz_cit_codice,
                            paz_cns_codice_old,
                            paz_cns_data_assegnazione,
                            paz_cns_terr_codice,
                            paz_codice_ausiliario,
                            paz_codice_regionale,
                            paz_com_codice_nascita,
                            paz_com_comune_emigrazione,
                            paz_com_comune_provenienza,
                            paz_data_agg_da_anag,
                            paz_data_aire,
                            paz_data_decesso,
                            paz_data_decorrenza_med,
                            paz_data_emigrazione,
                            paz_data_immigrazione,
                            paz_data_inserimento,
                            paz_data_irreperibilita,
                            paz_data_revoca_med,
                            paz_data_scadenza_med,
                            paz_dis_codice,
                            paz_irreperibile,
                            paz_locale,
                            paz_regolarizzato,
                            paz_stato,
                            paz_stato_anagrafico,
                            paz_telefono_1,
                            paz_telefono_2,
                            paz_telefono_3,
                            paz_tessera,
                            paz_usl_codice_assistenza,
                            paz_data_inizio_ass,
                            paz_data_cessazione_ass,
                            paz_usl_codice_residenza,
                            paz_usl_provenienza,
                            paz_anonimo,
                            paz_cat_codice,
                            paz_cag_codice,
                            paz_cancellato,
                            paz_codice_demografico,
                            paz_completare,
                            paz_data_aggiornamento,
                            paz_data_agg_da_comune,
                            paz_data_cancellazione,
                            paz_flag_stampa_maggiorenni,
                            paz_giorno,
                            paz_ind_codice_res,
                            paz_ind_codice_dom,
                            paz_padre,
                            paz_madre,
                            paz_occasionale,
                            paz_plb_id,
                            paz_posizione_vaccinale_ok,
                            paz_reg_assistenza,
                            paz_richiesta_cartella,
                            paz_richiesta_certificato,
                            paz_rsc_codice,
                            paz_stato_acquisizione_imi,
                            paz_stato_anagrafico_dett,
                            paz_stato_notifica_emi,
                            paz_sta_certificato_emi,
                            paz_tipo,
                            paz_tipo_occasionalita,
                            paz_livello_certificazione,
                            paz_codice_esterno,
                            paz_data_scadenza_ssn,
                            paz_stato_acquisizione,
                            paz_id_acn,
                            paz_categoria_cittadino,
                            paz_motivo_cessazione_assist,
                            paz_email)
  VALUES   (:paz_codice,
            :paz_cognome,
            :paz_nome,
            :paz_data_nascita,
            :paz_codice_fiscale,
            :paz_indirizzo_residenza,
            :paz_indirizzo_domicilio,
            :paz_sesso,
            :paz_med_codice_base,
            :paz_cns_codice,
            :paz_com_codice_residenza,
            :paz_cap_residenza,
            :paz_data_inizio_residenza,
            :paz_data_fine_residenza,
            :paz_com_codice_domicilio,
            :paz_cap_domicilio,
            :paz_data_inizio_domicilio,
            :paz_data_fine_domicilio,
            :paz_flag_cessato,
            :paz_aire,
            :paz_cir_codice,
            :paz_cir_codice_2,
            :paz_cit_codice,
            :paz_cns_codice_old,
            :paz_cns_data_assegnazione,
            :paz_cns_terr_codice,
            :paz_codice_ausiliario,
            :paz_codice_regionale,
            :paz_com_codice_nascita,
            :paz_com_comune_emigrazione,
            :paz_com_comune_provenienza,
            :paz_data_agg_da_anag,
            :paz_data_aire,
            :paz_data_decesso,
            :paz_data_decorrenza_med,
            :paz_data_emigrazione,
            :paz_data_immigrazione,
            :paz_data_inserimento,
            :paz_data_irreperibilita,
            :paz_data_revoca_med,
            :paz_data_scadenza_med,
            :paz_dis_codice,
            :paz_irreperibile,
            :paz_locale,
            :paz_regolarizzato,
            :paz_stato,
            :paz_stato_anagrafico,
            :paz_telefono_1,
            :paz_telefono_2,
            :paz_telefono_3,
            :paz_tessera,
            :paz_usl_codice_assistenza,
            :paz_data_inizio_ass,
            :paz_data_cessazione_ass,
            :paz_usl_codice_residenza,
            :paz_usl_provenienza,
            :paz_anonimo,
            :paz_cat_codice,
            :paz_cag_codice,
            :paz_cancellato,
            :paz_codice_demografico,
            :paz_completare,
            :paz_data_aggiornamento,
            :paz_data_agg_da_comune,
            :paz_data_cancellazione,
            :paz_flag_stampa_maggiorenni,
            :paz_giorno,
            :paz_ind_codice_res,
            :paz_ind_codice_dom,
            :paz_padre,
            :paz_madre,
            :paz_occasionale,
            :paz_plb_id,
            :paz_posizione_vaccinale_ok,
            :paz_reg_assistenza,
            :paz_richiesta_cartella,
            :paz_richiesta_certificato,
            :paz_rsc_codice,
            :paz_stato_acquisizione_imi,
            :paz_stato_anagrafico_dett,
            :paz_stato_notifica_emi,
            :paz_sta_certificato_emi,
            :paz_tipo,
            :paz_tipo_occasionalita,
            :paz_livello_certificazione,
            :paz_codice_esterno,
            :paz_data_scadenza_ssn,
            :paz_stato_acquisizione,
            :paz_id_acn,
            :paz_categoria_cittadino,
            :paz_motivo_cessazione_assist,
            :paz_email)";
            }
        }

        #endregion

        #region Update

        /// <summary>
        /// Query di update del paziente
        /// </summary>
        public static string updatePaziente
        {
            get
            {
                return @"update t_paz_pazienti set            
                            paz_cognome = :paz_cognome, 
                            paz_nome = :paz_nome, 
                            paz_data_nascita = :paz_data_nascita, 
                            paz_codice_fiscale = :paz_codice_fiscale,
                            paz_indirizzo_residenza = :paz_indirizzo_residenza,
                            paz_indirizzo_domicilio = :paz_indirizzo_domicilio, 
                            paz_sesso = :paz_sesso,
                            paz_med_codice_base = :paz_med_codice_base, 
                            paz_cns_codice = :paz_cns_codice,
                            paz_com_codice_residenza = :paz_com_codice_residenza, 
                            paz_cap_residenza = :paz_cap_residenza,
                            paz_data_inizio_residenza = :paz_data_inizio_residenza,
                            paz_data_fine_residenza = :paz_data_fine_residenza,
                            paz_com_codice_domicilio = :paz_com_codice_domicilio,
                            paz_cap_domicilio = :paz_cap_domicilio,
                            paz_data_inizio_domicilio = :paz_data_inizio_domicilio,
                            paz_data_fine_domicilio = :paz_data_fine_domicilio,
                            paz_flag_cessato = :paz_flag_cessato,
                            paz_aire = :paz_aire, 
                            paz_cir_codice = :paz_cir_codice, 
                            paz_cir_codice_2 = :paz_cir_codice_2, 
                            paz_cit_codice = :paz_cit_codice, 
                            paz_cns_codice_old = :paz_cns_codice_old, 
                            paz_cns_data_assegnazione = :paz_cns_data_assegnazione,
                            paz_cns_terr_codice = :paz_cns_terr_codice, 
                            paz_codice_ausiliario = :paz_codice_ausiliario,
                            paz_codice_regionale = :paz_codice_regionale,
                            paz_com_codice_nascita = :paz_com_codice_nascita,
                            paz_com_comune_emigrazione = :paz_com_comune_emigrazione,
                            paz_com_comune_provenienza = :paz_com_comune_provenienza,
                            paz_data_agg_da_anag = :paz_data_agg_da_anag,
                            paz_data_aire = :paz_data_aire,
                            paz_data_decesso = :paz_data_decesso,
                            paz_data_decorrenza_med = :paz_data_decorrenza_med,
                            paz_data_emigrazione = :paz_data_emigrazione,
                            paz_data_immigrazione = :paz_data_immigrazione,
                            paz_data_inserimento = :paz_data_inserimento, 
                            paz_data_irreperibilita = :paz_data_irreperibilita, 
                            paz_data_revoca_med = :paz_data_revoca_med,
                            paz_data_scadenza_med = :paz_data_scadenza_med,
                            paz_dis_codice = :paz_dis_codice,
                            paz_irreperibile = :paz_irreperibile,
                            paz_locale = :paz_locale,
                            paz_regolarizzato = :paz_regolarizzato,
                            paz_stato = :paz_stato,
                            paz_stato_anagrafico = :paz_stato_anagrafico,
                            paz_telefono_1 = :paz_telefono_1,
                            paz_telefono_2 = :paz_telefono_2, 
                            paz_telefono_3 = :paz_telefono_3,
                            paz_tessera = :paz_tessera, 
                            paz_usl_codice_assistenza = :paz_usl_codice_assistenza,
                            paz_data_inizio_ass = :paz_data_inizio_ass,
                            paz_data_cessazione_ass = :paz_data_cessazione_ass,
                            paz_usl_codice_residenza = :paz_usl_codice_residenza,
                            paz_usl_provenienza = :paz_usl_provenienza,
                            paz_anonimo = :paz_anonimo, 
                            paz_cat_codice = :paz_cat_codice, 
                            paz_cag_codice = :paz_cag_codice, 
                            paz_cancellato = :paz_cancellato,
                            paz_codice_demografico = :paz_codice_demografico,
                            paz_completare = :paz_completare, 
                            paz_data_aggiornamento = :paz_data_aggiornamento, 
                            paz_data_agg_da_comune = :paz_data_agg_da_comune,
                            paz_data_cancellazione = :paz_data_cancellazione, 
                            paz_flag_stampa_maggiorenni = :paz_flag_stampa_maggiorenni, 
                            paz_giorno = :paz_giorno, 
                            paz_ind_codice_res = :paz_ind_codice_res,
                            paz_ind_codice_dom = :paz_ind_codice_dom, 
                            paz_padre = :paz_padre, 
                            paz_madre = :paz_madre, 
                            paz_occasionale = :paz_occasionale, 
                            paz_plb_id = :paz_plb_id, 
                            paz_posizione_vaccinale_ok = :paz_posizione_vaccinale_ok,
                            paz_reg_assistenza = :paz_reg_assistenza,
                            paz_richiesta_cartella = :paz_richiesta_cartella, 
                            paz_richiesta_certificato = :paz_richiesta_certificato, 
                            paz_rsc_codice = :paz_rsc_codice,
                            paz_stato_acquisizione_imi = :paz_stato_acquisizione_imi, 
                            paz_stato_anagrafico_dett = :paz_stato_anagrafico_dett, 
                            paz_stato_notifica_emi = :paz_stato_notifica_emi,
                            paz_sta_certificato_emi = :paz_sta_certificato_emi, 
                            paz_tipo = :paz_tipo, 
                            paz_tipo_occasionalita = :paz_tipo_occasionalita,
                            paz_livello_certificazione = :paz_livello_certificazione,
                            paz_codice_esterno = :paz_codice_esterno, 
                            paz_data_scadenza_ssn = :paz_data_scadenza_ssn,
                            PAZ_STATO_ACQUISIZIONE = :PAZ_STATO_ACQUISIZIONE,
                            paz_id_acn = :paz_id_acn,
                            paz_categoria_cittadino = :paz_categoria_cittadino,
                            paz_motivo_cessazione_assist = :paz_motivo_cessazione_assist,
                            paz_email = :paz_email
                            where paz_codice = :paz_codice";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string updCircoscrizioneResidenza
        {
            get
            {
                return @"update t_paz_pazienti set paz_cir_codice = :cod_circ where paz_codice = :cod_paz";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string updCircoscrizioneDomicilio
        {
            get
            {
                return @"update t_paz_pazienti set paz_cir_codice_2 = :cod_circ where paz_codice = :cod_paz";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string updCodiceAusiliario
        {
            get
            {
                return @"update t_paz_pazienti set paz_codice_ausiliario = :cod_aux where paz_codice = :cod_paz";
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
		public static string updStatoVaccPaz
		{
			get
			{
				return @"update t_paz_pazienti
						 set paz_stato = :new_stato_vacc
						 where paz_codice = :cod_paz";
			}
		}
        
        /// <summary>
        /// 
        /// </summary>
		public static string updStatoVaccPazFromOldToNew
		{
			get
			{
				return @"update t_paz_pazienti
						 set paz_stato = :new_stato_vacc
						 where paz_codice = :cod_paz
						 and paz_stato = :old_stato_vacc";
			}
		}
        
        /// <summary>
        /// 
        /// </summary>
		public static string updRegolarizzaPaz
		{
			get
			{
				return @"update t_paz_pazienti
						 set paz_regolarizzato = :new_reg
						 where paz_codice = :cod_paz
						 and paz_regolarizzato = :old_reg";
			}
		}

        /// <summary>
        /// 
        /// </summary>
        public static string updFlagCancellato
        {
            get
            {
                return @"update t_paz_pazienti
						 set paz_cancellato = :paz_cancellato
						 where paz_codice = :paz_codice";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string updCodiceRegionale
        {
            get
            {
                return @"update t_paz_pazienti set paz_codice_regionale = :cod_reg where paz_codice = :cod_paz";
            }
        }

        /// <summary>
        /// Update codice residenza e indirizzo residenza del paziente
        /// </summary>
        public static string updCodiceEIndirizzoResidenzaPaziente
        {
            get
            {
                return @"UPDATE T_PAZ_PAZIENTI 
                         SET PAZ_IND_CODICE_RES = :codiceIndirizzo, 
                         PAZ_INDIRIZZO_RESIDENZA = :descrizioneIndirizzo
                         WHERE PAZ_CODICE = :codicePaziente";
            }
        }

        /// <summary>
        /// Update codice domicilio e indirizzo domicilio del paziente
        /// </summary>
        public static string updCodiceEIndirizzoDomicilioPaziente
        {
            get
            {
                return @"UPDATE T_PAZ_PAZIENTI 
                         SET PAZ_IND_CODICE_DOM = :codiceIndirizzo, 
                         PAZ_INDIRIZZO_DOMICILIO = :descrizioneIndirizzo
                         WHERE PAZ_CODICE = :codicePaziente";
            }
        }

        /// <summary>
        /// Update indirizzo residenza del paziente
        /// </summary>
        public static string updIndirizzoResidenzaPaziente
        {
            get
            {
                return @"UPDATE T_PAZ_PAZIENTI 
                         SET PAZ_INDIRIZZO_RESIDENZA = :descrizioneIndirizzo
                         WHERE PAZ_CODICE = :codicePaziente";
            }
        }

        /// <summary>
        /// Update indirizzo domicilio del paziente
        /// </summary>
        public static string updIndirizzoDomicilioPaziente
        {
            get
            {
                return @"UPDATE T_PAZ_PAZIENTI 
                         SET PAZ_INDIRIZZO_DOMICILIO = :descrizioneIndirizzo
                         WHERE PAZ_CODICE = :codicePaziente";
            }
        }

        /// <summary>
        /// Update ID ACN del paziente
        /// </summary>
        public static string updIdAcnPaziente
        {
            get
            {
                return @"UPDATE T_PAZ_PAZIENTI 
                         SET PAZ_ID_ACN = :idACN
                         WHERE PAZ_CODICE = :codicePaziente";
            }
        }

        #endregion

        #region Delete

        /// <summary>
        /// Eliminazione ritardi del paziente specificato
        /// </summary>
        public static string delRitardiPaziente
        {
            get
            {
                return @"delete from t_paz_ritardi where pri_paz_codice = :cod_paz";
            }
        }

        /// <summary>
        /// Eliminazione cicli del paziente specificato
        /// </summary>
        public static string delCicliPaziente
        {
            get
            {
                return @"delete from t_paz_cicli where pac_paz_codice = :cod_paz";
            }
        }

        #endregion

    }
}

