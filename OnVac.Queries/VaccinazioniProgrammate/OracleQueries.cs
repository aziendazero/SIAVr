namespace Onit.OnAssistnet.OnVac.Queries.VaccinazioniProgrammate
{
	/// <summary>
	/// Query Vaccinazioni Programmate
	/// </summary>
	public static class OracleQueries
	{

        /// <summary>
        /// Conteggio programmate del paziente nella data specificata
        /// </summary>
		public static string selCountVaccProgPaziente
		{
			get
			{
				return @"select nvl(count(*), 0) 
from T_VAC_PROGRAMMATE 
where vpr_cnv_data = :data_cnv 
and vpr_paz_codice = :cod_paz";
			}
		}

		/// <summary>
		/// Query di selezione, per il paziente, delle vaccinazioni programmate per le quali non esistano eseguite con stessa dose
		/// ed escluse non scadute.
		/// </summary>
		public static string selVaccinazioniProgrammatePazienteNotEscluseAndNotEseguite
        {
			get
			{
                return @"select VPR_VAC_CODICE, VAC_DESCRIZIONE, VPR_N_RICHIAMO, VAC_ORDINE 
from T_VAC_PROGRAMMATE, T_ANA_VACCINAZIONI 
where VPR_PAZ_CODICE = :cod_paz 
and VPR_CNV_DATA = :data_cnv 
and VPR_VAC_CODICE = VAC_CODICE 
and not exists ( 
	select 1 from T_VAC_ESEGUITE 
	where VPR_PAZ_CODICE = VES_PAZ_CODICE 
	and VPR_VAC_CODICE = VES_VAC_CODICE 
	and VPR_N_RICHIAMO = VES_N_RICHIAMO 
) 
and not exists ( 
	select 1 from T_VAC_ESCLUSE 
	where VPR_PAZ_CODICE = VEX_PAZ_CODICE 
	and VPR_VAC_CODICE = VEX_VAC_CODICE 
	and (VEX_DATA_SCADENZA is null or VEX_DATA_SCADENZA > sysdate) 
) 
order by VPR_ASS_CODICE";
			}
		}

		/// <summary>
		/// Query principale di selezione della programmazione che deve essere eliminata
		/// </summary>
		public static string selProgrammazioneDaEliminare
		{
			get
			{
				return @"select cnv_paz_codice, cnv_data
                        from t_cnv_convocazioni
                        where cnv_paz_codice = :cod_paz";
			}
		}

        /// <summary>
        /// Query principale di conteggio della programmazione che deve essere eliminata
        /// </summary>
        public static string countProgrammazioneDaEliminare
        {
            get
            {
                return @"select count(*)
                        from t_cnv_convocazioni
                        where cnv_paz_codice = :cod_paz";
            }
        }

		/// <summary>
		/// Sottoquery di filtro sui solleciti (per la selezione della programmazione da eliminare)
		/// </summary>
		public static string selProgrElim_subQuerySolleciti
		{
			get
			{
				return @"select distinct 1
                        from T_CNV_CICLI
                        where cnc_cnv_paz_codice = cnv_paz_codice
                        and cnc_cnv_data = cnv_data
                        and nvl(cnc_n_sollecito, 0) > 0";
			}
		}

        /// <summary>
        /// Sottoquery di filtro sui cicli (per la selezione della programmazione da eliminare)
        /// </summary>
        public static string selProgElim_subQueryCicli
        {
            get
            {
                return @"select 1 from t_vac_programmate 
                         where vpr_paz_codice = cnv_paz_codice 
                         and vpr_cnv_data = cnv_data
                         and vpr_cic_codice {0}";
            }
        }

		/// <summary>
		/// Sottoquery di filtro sui bilanci (per la selezione della programmazione da eliminare)
		/// </summary>
		public static string selProgrElim_subQueryBilanci
		{
			get
			{
				return @"select 1
from T_BIL_PROGRAMMATI
where bip_paz_codice = cnv_paz_codice
and bip_cnv_data = cnv_data
and bip_stato = 'UX'  ";
			}
		}

		/// <summary>
		/// Query di cancellazione dei bilanci associati alla programmazione
		/// </summary>
		public static string delBilanciAssociati
		{
			get
			{
				return @"delete from t_bil_programmati
where bip_cnv_data = :data_cnv
and bip_paz_codice = :paz_cod
and bip_stato = 'UX'";
			}
		}
		
		/// <summary>
		/// Query di cancellazione della convocazione associata alla programmazione
		/// </summary>
		public static string delConvocazioneAssociata
		{
			get
			{
				return @"delete from t_cnv_convocazioni
where cnv_data = :data_cnv
and cnv_paz_codice = :paz_cod";
			}
		}

		/// <summary>
		/// Query di selezione di tutte le programmate in una data specifica
		/// </summary>
		public static string selVaccinazioniProgrammatePazienteByData
		{
			get
			{
				return @"select vpr_cic_codice, vpr_n_seduta, vpr_ass_codice, vpr_vac_codice, vpr_cnv_data
from t_vac_programmate
where vpr_paz_codice = :paz_cod
and vpr_cnv_data = :data_cnv
order by vpr_cic_codice, vpr_n_seduta, vpr_ass_codice, vpr_vac_codice";
			}
		}

        public static string selVaccinazione
        {
            get
            {
                return @"select * from T_VAC_PROGRAMMATE 
                          where VPR_PAZ_CODICE = :VPR_PAZ_CODICE 
                            and VPR_VAC_CODICE = :VPR_VAC_CODICE";
            }
        }

        public static string selVaccinazioneConvocazione
        {
            get
            {
                return @"select * from T_VAC_PROGRAMMATE 
                          where VPR_PAZ_CODICE = :VPR_PAZ_CODICE 
                            and VPR_VAC_CODICE = :VPR_VAC_CODICE
                            and VPR_CNV_DATA = :VPR_CNV_DATA";
            }
        }

        /// <summary>
        /// Query di selezione di tutte le programmate in una data specifica
        /// </summary>
        public static string selVaccinazioniProgrammatePazienteByData2
        {
            get
            {
                return @"SELECT    vpr_vac_codice,
                                   vac_descrizione,
                                   vac_cod_sostituta,
                                   cic_descrizione,
                                   vpr_cic_codice,
                                   vpr_n_seduta,
                                   ope_ex,
                                   ope_vac,
                                   vpr_n_richiamo,
                                   ves_n_richiamo,
                                   vac_obbligatoria,
                                   ves_data_effettuazione,
                                   ves_dataora_effettuazione,
                                   ves_lot_codice,
                                   ves_lot_data_scadenza,
                                   sii_descrizione,
                                   ves_sii_codice,
                                   ves_noc_codice,
                                   noc_descrizione,
                                   ves_vii_codice,
                                   vii_descrizione,                                 
                                   vex_data_visita,
                                   vex_data_scadenza,
                                   vex_ope_codice,
                                   vex_moe_codice,
                                   ves_med_vaccinante,
                                   vpr_ass_codice,
                                   ass_descrizione,
                                   ves_ass_n_dose,
                                   MAX (vra_data_reazione) vra_data_reazione,
                                   vac_ordine,
                                   ves_note,
                                   ves_codice_esenzione,
                                   ves_importo,
                                   ves_mal_codice_malattia,
                                   noc_tpa_guid_tipi_pagamento,
                                   null flag_esenzione,
                                   null flag_importo,
                                   ves_flag_visibilita,
                                   ves_id,
                                   vex_id,
                                   ves_mal_codice_cond_sanitaria,
                                   ves_rsc_codice,
                                   vex_dose,
                                   vex_note
                            FROM   (SELECT   vpr_vac_codice,
                                             vac_descrizione,
                                             vac_cod_sostituta,
                                             cic_descrizione,
                                             vpr_cic_codice,
                                             vpr_n_seduta,
                                             opEx.ope_nome ope_ex,
                                             opVac.ope_nome ope_vac,
                                             vpr_n_richiamo,
                                             vac_obbligatoria,
                                             vpr_ass_codice,
                                             ass_descrizione,
                                             DECODE (
                                                vpr_ass_codice,
                                                NULL,
                                                NULL,
                                                NVL (
                                                   (SELECT   MAX (ves_ass_n_dose)
                                                      FROM   t_vac_eseguite
                                                     WHERE   vpr_ass_codice = ves_ass_codice
                                                             AND vpr_paz_codice = ves_paz_codice),
                                                   0
                                                )
                                                + 1
                                             )
                                                ves_ass_n_dose,
                                             NVL (
                                                vex_data_visita,
                                                (SELECT   vex_data_visita
                                                   FROM   t_vac_escluse
                                                  WHERE   vex_vac_codice = vac_cod_sostituta
                                                          AND vex_paz_codice = vpr_paz_codice)
                                             )
                                                vex_data_visita,
                                             NVL (
                                                vex_data_scadenza,
                                                (SELECT   vex_data_scadenza
                                                   FROM   t_vac_escluse
                                                  WHERE   vex_vac_codice = vac_cod_sostituta
                                                          AND vex_paz_codice = vpr_paz_codice)
                                             )
                                                vex_data_scadenza,
                                             NVL (
                                                vex_ope_codice,
                                                (SELECT   vex_ope_codice
                                                   FROM   t_vac_escluse
                                                  WHERE   vex_vac_codice = vac_cod_sostituta
                                                          AND vex_paz_codice = vpr_paz_codice)
                                             )
                                                vex_ope_codice,
                                             NVL (
                                                vex_moe_codice,
                                                (SELECT   vex_moe_codice
                                                   FROM   t_vac_escluse
                                                  WHERE   vex_vac_codice = vac_cod_sostituta
                                                          AND vex_paz_codice = vpr_paz_codice)
                                             )
                                                vex_moe_codice,
                                             NVL (
                                                ves_n_richiamo,
                                                (SELECT   ves_n_richiamo
                                                   FROM   t_vac_eseguite
                                                  WHERE       ves_vac_codice = vac_cod_sostituta
                                                          AND ves_paz_codice = vpr_paz_codice
                                                          AND ves_n_richiamo = vpr_n_richiamo)
                                             )
                                                ves_n_richiamo,
                                             NVL (
                                                ves_data_effettuazione,
                                                (SELECT   ves_data_effettuazione
                                                   FROM   t_vac_eseguite
                                                  WHERE       ves_vac_codice = vac_cod_sostituta
                                                          AND ves_paz_codice = vpr_paz_codice
                                                          AND ves_n_richiamo = vpr_n_richiamo)
                                             )
                                                ves_data_effettuazione,
                                             NVL (
                                                ves_dataora_effettuazione,
                                                (SELECT   ves_dataora_effettuazione
                                                   FROM   t_vac_eseguite
                                                  WHERE       ves_vac_codice = vac_cod_sostituta
                                                          AND ves_paz_codice = vpr_paz_codice
                                                          AND ves_n_richiamo = vpr_n_richiamo)
                                             )
                                                ves_dataora_effettuazione,
                                             NVL (
                                                ves_lot_codice,
                                                (SELECT   ves_lot_codice
                                                   FROM   t_vac_eseguite
                                                  WHERE       ves_vac_codice = vac_cod_sostituta
                                                          AND ves_paz_codice = vpr_paz_codice
                                                          AND ves_n_richiamo = vpr_n_richiamo)
                                             )
                                                ves_lot_codice,
                                             NVL (
                                                ves_lot_data_scadenza,
                                                (SELECT   ves_lot_data_scadenza
                                                   FROM   t_vac_eseguite
                                                  WHERE       ves_vac_codice = vac_cod_sostituta
                                                          AND ves_paz_codice = vpr_paz_codice
                                                          AND ves_n_richiamo = vpr_n_richiamo)
                                             )
                                                ves_lot_data_scadenza,
                                             NVL (
                                                ves_noc_codice,
                                                (SELECT   ves_noc_codice
                                                   FROM   t_vac_eseguite
                                                  WHERE       ves_vac_codice = vac_cod_sostituta
                                                          AND ves_paz_codice = vpr_paz_codice
                                                          AND ves_n_richiamo = vpr_n_richiamo)
                                             )
                                                ves_noc_codice,
                                             NVL (
                                                ves_sii_codice,
                                                (SELECT   ves_sii_codice
                                                   FROM   t_vac_eseguite
                                                  WHERE       ves_vac_codice = vac_cod_sostituta
                                                          AND ves_paz_codice = vpr_paz_codice
                                                          AND ves_n_richiamo = vpr_n_richiamo)
                                             )
                                                ves_sii_codice,
                                             NVL (
                                                sii_descrizione,
                                                (SELECT   sii_descrizione
                                                   FROM   t_vac_eseguite, t_ana_siti_inoculazione
                                                  WHERE       ves_vac_codice = vac_cod_sostituta
                                                          AND ves_paz_codice = vpr_paz_codice
                                                          AND ves_n_richiamo = vpr_n_richiamo
                                                          AND ves_sii_codice = sii_codice)
                                             )
                                                sii_descrizione,
                                             NVL (
                                                noc_descrizione,
                                                (SELECT   noc_descrizione
                                                   FROM   t_vac_eseguite, t_ana_nomi_commerciali
                                                  WHERE       ves_vac_codice = vac_cod_sostituta
                                                          AND ves_paz_codice = vpr_paz_codice
                                                          AND ves_n_richiamo = vpr_n_richiamo
                                                          AND ves_noc_codice = noc_codice)
                                             )
                                                noc_descrizione,
                                             NVL (
                                                ves_vii_codice,
                                                (SELECT   ves_vii_codice
                                                   FROM   t_vac_eseguite
                                                  WHERE       ves_vac_codice = vac_cod_sostituta
                                                          AND ves_paz_codice = vpr_paz_codice
                                                          AND ves_n_richiamo = vpr_n_richiamo)
                                             )
                                                ves_vii_codice,
                                             NVL (
                                                vii_descrizione,
                                                (SELECT   vii_descrizione
                                                   FROM   t_vac_eseguite, t_ana_vie_somministrazione
                                                  WHERE       ves_vac_codice = vac_cod_sostituta
                                                          AND ves_paz_codice = vpr_paz_codice
                                                          AND ves_n_richiamo = vpr_n_richiamo
                                                          AND ves_vii_codice = vii_codice)
                                             )
                                                vii_descrizione,
                                             ves_med_vaccinante,
                                             vra_data_reazione,
                                             vac_ordine,
                                             vpr_cnv_data,
                                             '' ves_note,
                                             ves_codice_esenzione,
                                             ves_importo,
                                             ves_mal_codice_malattia,
                                             noc_tpa_guid_tipi_pagamento,
                                             ves_flag_visibilita,
                                             ves_id,
                                             vex_id,
                                             ves_mal_codice_cond_sanitaria,
                                             ves_rsc_codice,
                                             vex_dose,
                                             vex_note
                                      FROM   t_vac_programmate,
                                             t_ana_operatori opEx,
                                             t_ana_operatori opVac,
                                             t_ana_vaccinazioni,
                                             t_ana_cicli,
                                             t_vac_eseguite,
                                             t_vac_escluse,
                                             t_ana_siti_inoculazione,
                                             t_ana_nomi_commerciali,
                                             t_vac_reazioni_avverse,
                                             t_ana_associazioni,
                                             t_ana_vie_somministrazione
                                     WHERE       vpr_paz_codice = :paz_cod
                                             AND vpr_cnv_data = :data_cnv
                                             AND vpr_vac_codice = vac_codice
                                             AND vpr_ass_codice = ass_codice(+)
                                             AND vpr_cic_codice = cic_codice(+)
                                             AND vpr_vac_codice = vex_vac_codice(+)
                                             AND vpr_paz_codice = vex_paz_codice(+)
                                             AND vpr_paz_codice = vra_paz_codice(+)
                                             AND vpr_vac_codice = vra_vac_codice(+)
                                             AND vpr_vac_codice = ves_vac_codice(+)
                                             AND vpr_paz_codice = ves_paz_codice(+)
                                             AND vpr_n_richiamo = ves_n_richiamo(+)
                                             AND vex_ope_codice = opex.ope_codice(+)
                                             AND ves_sii_codice = sii_codice(+)
                                             AND ves_vii_codice = vii_codice(+)
                                             AND ves_med_vaccinante = opvac.ope_codice(+)
                                             AND ves_noc_codice = noc_codice(+)
                                             )
                        GROUP BY   ves_n_richiamo,
                                   vpr_vac_codice,
                                   vac_descrizione,
                                   vac_cod_sostituta,
                                   cic_descrizione,
                                   vpr_cic_codice,
                                   vpr_n_seduta,
                                   ope_ex,
                                   ope_vac,
                                   vpr_n_richiamo,
                                   vac_obbligatoria,
                                   ves_lot_codice,
                                   ves_lot_data_scadenza,
                                   sii_descrizione,
                                   ves_sii_codice,
                                   ves_noc_codice,
                                   noc_descrizione,
                                   ves_vii_codice,
                                   vii_descrizione,
                                   vex_data_visita,
                                   vpr_cnv_data,
                                   vex_data_scadenza,
                                   vex_ope_codice,
                                   vex_moe_codice,
                                   ves_med_vaccinante,
                                   ves_data_effettuazione,
                                   ves_dataora_effettuazione,
                                   vac_ordine,
                                   vpr_ass_codice,
                                   ass_descrizione,
                                   ves_ass_n_dose,
                                   ves_note,
                                   ves_codice_esenzione,
                                   ves_importo,
                                   ves_mal_codice_malattia,
                                   noc_tpa_guid_tipi_pagamento,
                                   ves_flag_visibilita,
                                   ves_id,
                                   vex_id,
                                   ves_mal_codice_cond_sanitaria,
                                   ves_rsc_codice,
                                   vex_dose,
                                   vex_note";
            }
        } 

		/// <summary>
		/// Conteggio programmate, del tipo indicato, per il paziente, aventi richiamo superiore 
		/// a quello specificato o presenti nella data specificata
		/// </summary>
		public static string selCountVaccinazioneProgrammataDataRichiamo
		{
			get
			{
				return @"select count(*) from t_vac_programmate
where vpr_paz_codice = :cod_paz
and vpr_vac_codice = :cod_vac
and (vpr_n_richiamo >= :num_sed or vpr_cnv_data = :data_cnv)";
			}
		}

		/// <summary>
		/// Restituisce la prima data di convocazione della vaccinazione per il paziente, in base al richiamo specificato 
		/// </summary>
		public static string selDataVaccinazioneProgrammataRichiamo
		{
			get
			{
				return @"select min(vpr_cnv_data) from t_vac_programmate
where vpr_vac_codice = :cod_vac
and vpr_paz_codice = :cod_paz
and vpr_n_richiamo >= :num_sed";
			}
		}

		/// <summary>
		/// Restituisce la data di convocazione della vaccinazione per il paziente
		/// </summary>
        public static string selDataVaccinazioneProgrammata
		{
			get
			{
				return @"select vpr_cnv_data from t_vac_programmate
					where vpr_vac_codice = :cod_vac
					and vpr_paz_codice = :cod_paz";
			}
		}

        /// <summary>
        /// Controlla se esiste in programmazione la vaccinazione con il richiamo specificato 
        /// </summary>
        public static string selExistsVaccinazioneRichiamo
        {
            get
            {
                return @"select 1 
                          from t_vac_programmate
				         where vpr_vac_codice = :cod_vac
					       and vpr_paz_codice = :cod_paz
                           and vpr_n_richiamo = :vpr_n_richiamo";
            }
        }

		/// <summary>
        /// Elimina la vaccinazione programmata per il paziente
		/// </summary>
		public static string delVaccinazione
		{
			get
			{
				return @"delete from t_vac_programmate
					where vpr_vac_codice = :cod_vac
					and vpr_paz_codice = :cod_paz";
			}
		}

        /// <summary>
        /// Elimina la vaccinazione programmata per il paziente dalla convocazione specificata
        /// </summary>
        public static string delVaccinazioneConvocazione
        {
            get
            {
                return @"delete from t_vac_programmate
					where vpr_vac_codice = :cod_vac
					and vpr_paz_codice = :cod_paz
                    and vpr_cnv_data = :dat_cnv";
            }
        }

        /// <summary>
        /// Elimina la vaccinazione programmata
        /// </summary>
        public static string delVaccinazioneByRichiamo
        {
            get
            {
                return @"delete from t_vac_programmate
					where vpr_vac_codice = :codiceVaccinazione
					and vpr_paz_codice = :codicePaziente
                    and vpr_n_richiamo = :numeroRichiamo";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string selCountProgrammateAmbulatorio
        {
            get
            {
                return @"SELECT   t1.cnv_data_appuntamento cnv_data_appuntamento,
           sum (t1.n_normali) n_normali,
           sum (t1.n_ritardi) n_ritardi,
           sum (t1.n_bilanci) n_bilanci
    from   (select   trunc (cnv_data_appuntamento) cnv_data_appuntamento,
                     (select   count ( * )
                        from   t_cnv_convocazioni c1
                       where   c.cnv_paz_codice = c1.cnv_paz_codice
                               and c.cnv_data = c1.cnv_data
                               and exists
                                     (select   1
                                        from   t_vac_programmate
                                       where   vpr_paz_codice =
                                                  c1.cnv_paz_codice
                                               and vpr_cnv_data = c1.cnv_data)
                               and not exists
                                     (select   1
                                        from   t_cnv_cicli
                                       where   cnc_cnv_paz_codice =
                                                  c1.cnv_paz_codice
                                               and cnc_cnv_data = c1.cnv_data
                                               and cnc_n_sollecito > 0))
                        n_normali,
                     (select   count ( * )
                        from   t_cnv_convocazioni c1
                       where   c.cnv_paz_codice = c1.cnv_paz_codice
                               and c.cnv_data = c1.cnv_data
                               and exists
                                     (select   1
                                        from   t_vac_programmate
                                       where   vpr_paz_codice =
                                                  c1.cnv_paz_codice
                                               and vpr_cnv_data = c1.cnv_data)
                               and exists
                                     (select   1
                                        from   t_cnv_cicli
                                       where   cnc_cnv_paz_codice =
                                                  c1.cnv_paz_codice
                                               and cnc_cnv_data = c1.cnv_data
                                               and cnc_n_sollecito > 0))
                        n_ritardi,
                     (select   count ( * )
                        from   t_cnv_convocazioni c1
                       where   c.cnv_paz_codice = c1.cnv_paz_codice
                               and c.cnv_data = c1.cnv_data
                               and not exists
                                     (select   1
                                        from   t_vac_programmate
                                       where   vpr_paz_codice =
                                                  c1.cnv_paz_codice
                                               and vpr_cnv_data = c1.cnv_data))
                        n_bilanci
              from   t_cnv_convocazioni c
             where   c.cnv_amb_codice = :amb_codice
                     and c.cnv_data_appuntamento is not null) t1
group by   t1.cnv_data_appuntamento";
            }
        }

        /// <summary>
        /// Elimina la vaccinazione programmata per il paziente
        /// </summary>
        public static string selAppuntamentiMattinoPomeriggio
        {  
            get
            {
                return @"SELECT   paz_codice,
           paz_nome,
           paz_cognome,
           paz_data_nascita,
           cnv_data_invio,
           cnv_amb_codice,
           cnv_data,
           cnv_data_appuntamento,
           cnv_durata_appuntamento,
           med_descrizione,
           cnv_tipo_appuntamento,
           vaccinazioni,
           NVL (MAX (cnc_n_sollecito), 0) sollecito,
           tempo_bil,
           trova_termine_perentorio (paz_codice,
                                     cnv_data,
                                     :max_num_soll_default)
              tp,
           eseguita,
           esclusa,
           bilanci
    FROM   (SELECT   paz_codice,
                     paz_nome,
                     paz_cognome,
                     paz_data_nascita,
                     cnv_data_invio,
                     cnv_amb_codice,
                     cnv_data,
                     cnv_data_appuntamento,
                     cnv_durata_appuntamento,
                     med_descrizione,
                     cnv_tipo_appuntamento,
                     trova_vaccinazioni (paz_codice, cnv_data) AS vaccinazioni,
                     cnc_n_sollecito,
                     'N' tempo_bil,
                     (SELECT   COUNT (1)
                        FROM   t_vac_programmate, t_vac_eseguite
                       WHERE       vpr_paz_codice = paz_codice
                               AND vpr_cnv_data = cnv_data
                               AND vpr_paz_codice = ves_paz_codice
                               AND vpr_vac_codice = ves_vac_codice
                               AND vpr_n_richiamo = ves_n_richiamo)
                        AS eseguita,
                     (SELECT   COUNT (1)
                        FROM   t_vac_programmate, t_vac_escluse
                       WHERE       vpr_paz_codice = paz_codice
                               AND vpr_cnv_data = cnv_data
                               AND vpr_paz_codice = vex_paz_codice
                               AND vpr_vac_codice = vex_vac_codice
                               AND (vex_data_scadenza is null 
                                    or vex_data_scadenza > SYSDATE))
                        AS esclusa,
                     (SELECT   COUNT (1)
                        FROM   t_bil_programmati
                       WHERE   bip_paz_codice = paz_codice
                               AND bip_cnv_data = cnv_data)
                        AS bilanci
              FROM   t_paz_pazienti,
                     t_cnv_convocazioni,
                     t_cnv_cicli,
                     t_ana_medici
             WHERE       cnv_data_appuntamento >= :data_inizio
                     AND cnv_data_appuntamento < :data_fine
                     AND cnv_amb_codice = :amb_codice
                     AND cnv_paz_codice = paz_codice
                     AND paz_med_codice_base = med_codice(+)
                     AND cnv_data = cnc_cnv_data(+)
                     AND cnv_paz_codice = cnc_cnv_paz_codice(+))
GROUP BY   paz_codice,
           paz_nome,
           paz_cognome,
           paz_data_nascita,
           cnv_data_invio,
           cnv_data,
           cnv_data_appuntamento,
           cnv_durata_appuntamento,
           med_descrizione,
           cnv_tipo_appuntamento,
           vaccinazioni,
           cnv_amb_codice,
           tempo_bil,
           eseguita,
           esclusa,
           bilanci
ORDER BY   cnv_data_appuntamento, cnv_data, paz_codice";
            }
        }

	}
}

