namespace Onit.OnAssistnet.OnVac.Queries.Copertura
{
    /// <summary>
    /// Descrizione di riepilogo per OracleQueries.
    /// </summary>
    public static class OracleQueries
    {

        /// <summary>
        /// 
        /// </summary>
        public static string selTotaleAnagrafico
        {
            get
            {
                return @"SELECT   count(*) 
                    FROM   t_paz_pazienti 
                   WHERE   paz_data_nascita <= :data_nascita_fine
                     AND   paz_data_nascita >= :data_nascita_inizio
                     {0}";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string selTotaleAnagraficoMedico
        {
            get
            {
                return @"SELECT   count(*) 
                    FROM   t_paz_pazienti, t_ana_medici 
                   WHERE   paz_med_codice_base = med_codice
                     AND   paz_data_nascita <= :data_nascita_fine
                     AND   paz_data_nascita >= :data_nascita_inizio
                     {0}";
            }
        }

        /// <summary>
        /// Query per report Copertura Vaccinale
        /// </summary>
        public static string selCopertura
        {
            get
            {
                return @"SELECT   t1.vac_codice,
           t1.vac_descrizione,
           t1.vac_ordine,
           (SELECT   COUNT (DISTINCT paz_codice)
              FROM   t_vac_eseguite, t_paz_pazienti
             WHERE       t1.vac_codice = ves_vac_codice
                     AND ves_paz_codice = paz_codice
                     AND paz_data_nascita <= :data_nascita_fine
                     AND paz_data_nascita >= :data_nascita_inizio
                     {0}
                     AND (ves_data_effettuazione - paz_data_nascita) <= :giorni_vita
                     AND ves_n_richiamo >= :dosi
                     {1}
            )
              AS num_vaccinati,
           (SELECT   COUNT (DISTINCT paz_codice)
              FROM   t_vac_escluse,
                     t_ana_motivi_esclusione,
                     t_paz_pazienti,
                     t_vac_eseguite
             WHERE       t1.vac_codice = vex_vac_codice
                     AND vex_moe_codice = moe_codice
                     AND vex_paz_codice = paz_codice
                     AND vex_paz_codice = ves_paz_codice(+)
                     AND vex_vac_codice = ves_vac_codice(+)
                     AND (vex_data_scadenza is null or vex_data_scadenza > sysdate)
                     AND moe_calcolo_copertura = 'I'
                     AND ves_data_effettuazione IS NULL
                     AND paz_data_nascita <= :data_nascita_fine
                     AND paz_data_nascita >= :data_nascita_inizio
                     {0}
            ) AS immuni_mai_vaccinati,
           (SELECT   COUNT (DISTINCT paz_codice)
              FROM   t_vac_escluse,
                     t_ana_motivi_esclusione,
                     t_paz_pazienti,
                     t_vac_eseguite ese1
             WHERE       t1.vac_codice = vex_vac_codice
                     AND vex_moe_codice = moe_codice
                     AND vex_paz_codice = paz_codice
                     AND vex_paz_codice = ves_paz_codice
                     AND vex_vac_codice = ves_vac_codice
                     AND (vex_data_scadenza is null or vex_data_scadenza > sysdate)
                     AND moe_calcolo_copertura = 'I'
                     AND paz_data_nascita <= :data_nascita_fine
                     AND paz_data_nascita >= :data_nascita_inizio
                     {0}
                     AND ves_n_richiamo < :dosi
                     AND not exists (
                             select   1
                               from   t_vac_eseguite ese2
                              where   ese2.ves_vac_codice = ese1.ves_vac_codice
                                and   ese2.ves_paz_codice = ese1.ves_paz_codice
                                and   ese2.ves_n_richiamo >= :dosi
                                and   (ese2.ves_data_effettuazione - paz_data_nascita) <= :giorni_vita
                        )
                     {1}
            ) AS immuni_vaccinati,
           (SELECT   COUNT (DISTINCT paz_codice)
              FROM   t_vac_escluse,
                     t_ana_motivi_esclusione,
                     t_paz_pazienti,
                     t_vac_eseguite
             WHERE       t1.vac_codice = vex_vac_codice
                     AND vex_moe_codice = moe_codice
                     AND vex_paz_codice = paz_codice
                     AND vex_paz_codice = ves_paz_codice(+)
                     AND vex_vac_codice = ves_vac_codice(+)
                     AND (vex_data_scadenza is null or vex_data_scadenza > sysdate)
                     AND moe_calcolo_copertura = 'V'
                     AND ves_data_effettuazione IS NULL
                     AND paz_data_nascita <= :data_nascita_fine
                     AND paz_data_nascita >= :data_nascita_inizio
                     {0}
            ) AS non_vaccinabili_mai_vaccinati,
           (SELECT   COUNT (DISTINCT paz_codice)
              FROM   t_vac_escluse,
                     t_ana_motivi_esclusione,
                     t_paz_pazienti,
                     t_vac_eseguite ese1
             WHERE       t1.vac_codice = vex_vac_codice
                     AND vex_moe_codice = moe_codice
                     AND vex_paz_codice = paz_codice
                     AND vex_paz_codice = ves_paz_codice
                     AND vex_vac_codice = ves_vac_codice
                     AND (vex_data_scadenza is null or vex_data_scadenza > sysdate)
                     AND moe_calcolo_copertura = 'V'
                     AND paz_data_nascita <= :data_nascita_fine
                     AND paz_data_nascita >= :data_nascita_inizio
                     {0}
                     AND ves_n_richiamo < :dosi
                     AND not exists (
                             select   1
                               from   t_vac_eseguite ese2
                              where   ese2.ves_vac_codice = ese1.ves_vac_codice
                                and   ese2.ves_paz_codice = ese1.ves_paz_codice
                                and   ese2.ves_n_richiamo >= :dosi
                               and   (ese2.ves_data_effettuazione - paz_data_nascita) <= :giorni_vita
                        )
                     {1}
            ) AS non_vaccinabili_vaccinati,
           (SELECT   COUNT (DISTINCT paz_codice)
              FROM   t_paz_pazienti
             WHERE   paz_data_nascita <= :data_nascita_fine
                     AND paz_data_nascita >= :data_nascita_inizio
                     {0}
            ) AS num_pazienti
    FROM   t_ana_vaccinazioni t1";
            }
        }

        /// <summary>
        /// Query per report Copertura Vaccinale
        /// </summary>
        public static string selCoperturaMedico
        {
            get
            {
                return @"SELECT   t1.vac_codice,
           t1.vac_descrizione,
           t1.vac_ordine,
           t2.med_codice,
           t2.med_descrizione,
           (SELECT   COUNT (DISTINCT paz_codice)
              FROM   t_vac_eseguite, t_paz_pazienti
             WHERE       t1.vac_codice = ves_vac_codice
                     AND ves_paz_codice = paz_codice
                     AND paz_data_nascita <= :data_nascita_fine
                     AND paz_data_nascita >= :data_nascita_inizio
                     AND t2.med_codice = paz_med_codice_base
                     {0}
                     AND (ves_data_effettuazione - paz_data_nascita) <= :giorni_vita
                     AND ves_n_richiamo >= :dosi
                     {1}
            )
              AS num_vaccinati,
           (SELECT   COUNT (DISTINCT paz_codice)
              FROM   t_vac_escluse,
                     t_ana_motivi_esclusione,
                     t_paz_pazienti,
                     t_vac_eseguite
             WHERE       t1.vac_codice = vex_vac_codice
                     AND vex_moe_codice = moe_codice
                     AND vex_paz_codice = paz_codice
                     AND vex_paz_codice = ves_paz_codice(+)
                     AND vex_vac_codice = ves_vac_codice(+)
                     AND moe_calcolo_copertura = 'I'
                     AND ves_data_effettuazione IS NULL
                     AND paz_data_nascita <= :data_nascita_fine
                     AND paz_data_nascita >= :data_nascita_inizio
                     AND t2.med_codice = paz_med_codice_base
                     {0}
            ) AS immuni_mai_vaccinati,
           (SELECT   COUNT (DISTINCT paz_codice)
              FROM   t_vac_escluse,
                     t_ana_motivi_esclusione,
                     t_paz_pazienti,
                     t_vac_eseguite ese1
             WHERE       t1.vac_codice = vex_vac_codice
                     AND vex_moe_codice = moe_codice
                     AND vex_paz_codice = paz_codice
                     AND vex_paz_codice = ves_paz_codice
                     AND vex_vac_codice = ves_vac_codice
                     AND moe_calcolo_copertura = 'I'
                     AND paz_data_nascita <= :data_nascita_fine
                     AND paz_data_nascita >= :data_nascita_inizio
                     AND t2.med_codice = paz_med_codice_base
                     {0}
                     AND ves_n_richiamo < :dosi
                     AND not exists (
                             select   1
                               from   t_vac_eseguite ese2
                              where   ese2.ves_vac_codice = ese1.ves_vac_codice
                                and   ese2.ves_paz_codice = ese1.ves_paz_codice
                                and   ese2.ves_n_richiamo >= :dosi
                                and   (ese2.ves_data_effettuazione - paz_data_nascita) <= :giorni_vita
                        )
                     {1}
            ) AS immuni_vaccinati,
           (SELECT   COUNT (DISTINCT paz_codice)
              FROM   t_vac_escluse,
                     t_ana_motivi_esclusione,
                     t_paz_pazienti,
                     t_vac_eseguite
             WHERE       t1.vac_codice = vex_vac_codice
                     AND vex_moe_codice = moe_codice
                     AND vex_paz_codice = paz_codice
                     AND vex_paz_codice = ves_paz_codice(+)
                     AND vex_vac_codice = ves_vac_codice(+)
                     AND moe_calcolo_copertura = 'V'
                     AND ves_data_effettuazione IS NULL
                     AND paz_data_nascita <= :data_nascita_fine
                     AND paz_data_nascita >= :data_nascita_inizio
                     AND t2.med_codice = paz_med_codice_base
                     {0}
            ) AS non_vaccinabili_mai_vaccinati,
           (SELECT   COUNT (DISTINCT paz_codice)
              FROM   t_vac_escluse,
                     t_ana_motivi_esclusione,
                     t_paz_pazienti,
                     t_vac_eseguite ese1
             WHERE       t1.vac_codice = vex_vac_codice
                     AND vex_moe_codice = moe_codice
                     AND vex_paz_codice = paz_codice
                     AND vex_paz_codice = ves_paz_codice
                     AND vex_vac_codice = ves_vac_codice
                     AND moe_calcolo_copertura = 'V'
                     AND paz_data_nascita <= :data_nascita_fine
                     AND paz_data_nascita >= :data_nascita_inizio
                     AND t2.med_codice = paz_med_codice_base
                     {0}
                     AND ves_n_richiamo < :dosi
                     AND not exists (
                             select   1
                               from   t_vac_eseguite ese2
                              where   ese2.ves_vac_codice = ese1.ves_vac_codice
                                and   ese2.ves_paz_codice = ese1.ves_paz_codice
                                and   ese2.ves_n_richiamo >= :dosi
                               and   (ese2.ves_data_effettuazione - paz_data_nascita) <= :giorni_vita
                        )
                     {1}
            ) AS non_vaccinabili_vaccinati,
           (SELECT   COUNT (DISTINCT paz_codice)
              FROM   t_paz_pazienti
             WHERE   paz_data_nascita <= :data_nascita_fine
                     AND paz_data_nascita >= :data_nascita_inizio
                     AND t2.med_codice = paz_med_codice_base
                     {0}
            ) AS num_pazienti
    FROM   t_ana_vaccinazioni t1,
           t_ana_medici t2";
            }
        }

        /// <summary>
        /// Query per report Copertura per Consultrio
        /// </summary>
        public static string selCoperturaCNS
        {
            get
            {
                return @"SELECT   t1.vac_codice,
           t1.vac_descrizione,
           t1.vac_ordine,
           (SELECT   COUNT (DISTINCT paz_codice)
              FROM   t_vac_eseguite, t_paz_pazienti
             WHERE       t1.vac_codice = ves_vac_codice
                     AND ves_paz_codice = paz_codice
                     AND paz_data_nascita <= :data_nascita_fine
                     AND paz_data_nascita >= :data_nascita_inizio
                     AND paz_cns_codice = cns_codice
                     {0}
                     AND (ves_data_effettuazione - paz_data_nascita) <= :giorni_vita
                     AND ves_n_richiamo >= :dosi
                     {1}
            )
              AS num_vaccinati,
           (SELECT   COUNT (DISTINCT paz_codice)
              FROM   t_vac_escluse,
                     t_ana_motivi_esclusione,
                     t_paz_pazienti,
                     t_vac_eseguite 
             WHERE       t1.vac_codice = vex_vac_codice
                     AND vex_moe_codice = moe_codice
                     AND vex_paz_codice = paz_codice
                     AND vex_paz_codice = ves_paz_codice(+)
                     AND vex_vac_codice = ves_vac_codice(+)
                     AND moe_calcolo_copertura = 'I'
                     AND ves_data_effettuazione IS NULL
                     AND paz_data_nascita <= :data_nascita_fine
                     AND paz_data_nascita >= :data_nascita_inizio
                     AND paz_cns_codice = cns_codice
                     {0}
            ) AS immuni_mai_vaccinati,
           (SELECT   COUNT (DISTINCT paz_codice)
              FROM   t_vac_escluse,
                     t_ana_motivi_esclusione,
                     t_paz_pazienti,
                     t_vac_eseguite ese1
             WHERE       t1.vac_codice = vex_vac_codice
                     AND vex_moe_codice = moe_codice
                     AND vex_paz_codice = paz_codice
                     AND vex_paz_codice = ves_paz_codice
                     AND vex_vac_codice = ves_vac_codice
                     AND moe_calcolo_copertura = 'I'
                     AND paz_data_nascita <= :data_nascita_fine
                     AND paz_data_nascita >= :data_nascita_inizio
                     AND paz_cns_codice = cns_codice
                     {0}
                     AND (ves_data_effettuazione - paz_data_nascita) <= :giorni_vita
                     AND ves_n_richiamo < :dosi
                     AND not exists (
                             select   1
                               from   t_vac_eseguite ese2
                              where   ese2.ves_vac_codice = ese1.ves_vac_codice
                                and   ese2.ves_paz_codice = ese1.ves_paz_codice
                                and   ese2.ves_n_richiamo >= :dosi
                        )
                     {1}
            ) AS immuni_vaccinati,
           (SELECT   COUNT (DISTINCT paz_codice)
              FROM   t_vac_escluse,
                     t_ana_motivi_esclusione,
                     t_paz_pazienti,
                     t_vac_eseguite
             WHERE       t1.vac_codice = vex_vac_codice
                     AND vex_moe_codice = moe_codice
                     AND vex_paz_codice = paz_codice
                     AND vex_paz_codice = ves_paz_codice(+)
                     AND vex_vac_codice = ves_vac_codice(+)
                     AND moe_calcolo_copertura = 'V'
                     AND ves_data_effettuazione IS NULL
                     AND paz_data_nascita <= :data_nascita_fine
                     AND paz_data_nascita >= :data_nascita_inizio
                     AND paz_cns_codice = cns_codice
                     {0}
            ) AS non_vaccinabili_mai_vaccinati,
           (SELECT   COUNT (DISTINCT paz_codice)
              FROM   t_vac_escluse,
                     t_ana_motivi_esclusione,
                     t_paz_pazienti,
                     t_vac_eseguite ese1
             WHERE       t1.vac_codice = vex_vac_codice
                     AND vex_moe_codice = moe_codice
                     AND vex_paz_codice = paz_codice
                     AND vex_paz_codice = ves_paz_codice
                     AND vex_vac_codice = ves_vac_codice
                     AND moe_calcolo_copertura = 'V'
                     AND paz_data_nascita <= :data_nascita_fine
                     AND paz_data_nascita >= :data_nascita_inizio
                     AND paz_cns_codice = cns_codice
                     {0}
                     AND (ves_data_effettuazione - paz_data_nascita) <= :giorni_vita
                     AND ves_n_richiamo < :dosi
                     AND not exists (
                             select   1
                               from   t_vac_eseguite ese2
                              where   ese2.ves_vac_codice = ese1.ves_vac_codice
                                and   ese2.ves_paz_codice = ese1.ves_paz_codice
                                and   ese2.ves_n_richiamo >= :dosi
                        )
                     {1}
            ) AS non_vaccinabili_vaccinati,
           (SELECT   COUNT (DISTINCT paz_codice)
              FROM   t_paz_pazienti
             WHERE   paz_data_nascita <= :data_nascita_fine
                     AND paz_data_nascita >= :data_nascita_inizio
                     AND paz_cns_codice = cns_codice
                     {0}
            ) AS num_pazienti,
            cns_codice,
            cns_descrizione
    FROM   t_ana_vaccinazioni t1, t_ana_consultori";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string selMotiviEsclusione
        {
            get
            {
                return @"select   moe_descrizione,
         vac_descrizione,
         paz_codice
  from   (select   distinct
                   moe_descrizione,
                   vac_codice,
                   vac_descrizione,
                   vac_obbligatoria,
                   paz_codice
            from   t_vac_escluse,
                   t_ana_motivi_esclusione,
                   t_ana_vaccinazioni,
                   t_paz_pazienti
           where       vex_moe_codice = moe_codice
                   and vex_vac_codice = vac_codice
                   and (vex_data_scadenza is null or vex_data_scadenza > sysdate)
                   and paz_codice = vex_paz_codice
                   and paz_data_nascita <= :data_nascita_fine
                   and paz_data_nascita >= :data_nascita_inizio
                   and not exists
                   (select   1
                        from   t_vac_eseguite
                       where       ves_paz_codice = paz_codice
                         and ves_vac_codice = vac_codice
                         and (ves_data_effettuazione - paz_data_nascita) <= :giorni_vita
                         and ves_n_richiamo >= :dosi
                         {1}
                   )
                   {0}
union
          select   distinct
                   'SENZA ESCLUSIONE' as moe_descrizione,
                   vac_codice,
                   vac_descrizione,
                   vac_obbligatoria,
                   paz_codice
            from   t_paz_pazienti, t_ana_vaccinazioni
           where   not exists
                         (select   1
                            from   t_vac_escluse
                           where   vex_paz_codice = paz_codice
                                   and vex_vac_codice = vac_codice)
                   and not exists
                   (select   1
                        from   t_vac_eseguite
                       where       ves_paz_codice = paz_codice
                         and ves_vac_codice = vac_codice
                         and (ves_data_effettuazione - paz_data_nascita) <= :giorni_vita
                         and ves_n_richiamo >= :dosi
                         {1}
                   )
                   and paz_data_nascita <= :data_nascita_fine
                   and paz_data_nascita >= :data_nascita_inizio
                   {0}
        )
where   1 = 1 ";
            }
        }

        /// <summary>
        /// Query per report Coperture -> Elenco Non Vaccinati
        /// </summary>
        public static string selElencoNonVaccinati
        {
            get
            {
                return @"SELECT   *
  FROM   (SELECT   paz_codice,
                   paz_cognome,
                   paz_nome,
                   paz_data_nascita,
                   paz_indirizzo_residenza,
                   com_descrizione,
                   paz_cns_codice,
                   paz_com_codice_residenza,
                   paz_cir_codice,
                   cir_descrizione,
                   (select pno_testo_note
                        from t_ana_tipo_note
                        join t_paz_note on tno_codice = pno_tno_codice
                        where pno_paz_codice = paz_codice
                        and tno_codice = :codice_note_annotazioni
                        and (
                            pno_azi_codice = :pno_azi_codice
                            or pno_azi_codice in (
                                select dis_codice 
                                from t_ana_distretti
                                where dis_usl_codice = :pno_azi_codice
                            )
                        )) paz_note,
                   paz_stato_anagrafico,
                   paz_sesso,
                   vac_codice,
                   vac_descrizione,
                   vac_obbligatoria,
                   moe_descrizione,
                   vex_data_visita,
                   vex_data_scadenza,
                   (select pno_testo_note
                        from t_ana_tipo_note
                        join t_paz_note on tno_codice = pno_tno_codice
                        where pno_paz_codice = paz_codice
                        and tno_codice = :codice_note_solleciti
                        and (
                            pno_azi_codice = :pno_azi_codice
                            or pno_azi_codice in (
                                select dis_codice 
                                from t_ana_distretti
                                where dis_usl_codice = :pno_azi_codice
                            )
                        )) paz_note_solleciti
            FROM   t_ana_comuni,
                   t_ana_circoscrizioni,
                   t_vac_escluse,
                   t_ana_motivi_esclusione,
                   t_ana_vaccinazioni,
                   t_paz_pazienti
           WHERE       paz_com_codice_residenza = com_codice(+)
                   AND paz_cir_codice = cir_codice(+)
                   AND vex_moe_codice = moe_codice
                   AND vex_vac_codice = vac_codice
                   AND paz_codice = vex_paz_codice
                   AND  paz_data_nascita <= :data_nascita_fine
                   AND paz_data_nascita >= :data_nascita_inizio
                   AND NOT EXISTS
                   (SELECT 1
                      FROM t_vac_eseguite
                       WHERE ves_paz_codice = paz_codice
                         AND ves_vac_codice = vac_codice
                         AND (ves_data_effettuazione - paz_data_nascita) <= :giorni_vita
                         AND ves_n_richiamo >= :dosi
                         {1}
                   )
                   {0}
          UNION
          SELECT   paz_codice,
                   paz_cognome,
                   paz_nome,
                   paz_data_nascita,
                   paz_indirizzo_residenza,
                   com_descrizione,
                   paz_cns_codice,
                   paz_com_codice_residenza,
                   paz_cir_codice,
                   cir_descrizione,
                   (select pno_testo_note
                        from t_ana_tipo_note
                        join t_paz_note on tno_codice = pno_tno_codice
                        where pno_paz_codice = paz_codice
                        and tno_codice = :codice_note_annotazioni
                        and (
                            pno_azi_codice = :pno_azi_codice
                            or pno_azi_codice in (
                                select dis_codice 
                                from t_ana_distretti
                                where dis_usl_codice = :pno_azi_codice
                            )
                        )) paz_note,
                   paz_stato_anagrafico,
                   paz_sesso,
                   vac_codice,
                   vac_descrizione,
                   vac_obbligatoria,
                   NULL AS moe_descrizione,
                   NULL AS vex_data_visita,
                   NULL AS vex_data_scadenza,
                   (select pno_testo_note
                        from t_ana_tipo_note
                        join t_paz_note on tno_codice = pno_tno_codice
                        where pno_paz_codice = paz_codice
                        and tno_codice = :codice_note_solleciti
                        and (
                            pno_azi_codice = :pno_azi_codice
                            or pno_azi_codice in (
                                select dis_codice 
                                from t_ana_distretti
                                where dis_usl_codice = :pno_azi_codice
                            )
                        )) paz_note_solleciti
            FROM   t_ana_comuni,
                   t_ana_circoscrizioni,
                   t_paz_pazienti,
                   t_ana_vaccinazioni
           WHERE paz_com_codice_residenza = com_codice(+)
                   AND paz_cir_codice = cir_codice(+)
                   AND NOT EXISTS
                         (SELECT   1
                            FROM   t_vac_escluse
                           WHERE   vex_paz_codice = paz_codice
                                   AND vex_vac_codice = vac_codice)
                   AND  paz_data_nascita <= :data_nascita_fine
                   AND paz_data_nascita >= :data_nascita_inizio
                   AND NOT EXISTS
                   (SELECT 1
                      FROM t_vac_eseguite
                       WHERE ves_paz_codice = paz_codice
                         AND ves_vac_codice = vac_codice
                         AND (ves_data_effettuazione - paz_data_nascita) <= :giorni_vita
                         AND ves_n_richiamo >= :dosi
                         {1}
                   )
                   {0}
        )
 WHERE   1 = 1";
            }
        }

        /// <summary>
        /// Query per report Coperture -> Elenco Vaccinati
        /// </summary>
        public static string selElencoVaccinati
        {
            get
            {
                return @"SELECT paz_codice,
                   paz_cognome,
                   paz_nome,
                   paz_data_nascita,
                   paz_indirizzo_residenza,
                   com_descrizione,
                   paz_cns_codice,
                   paz_com_codice_residenza,
                   paz_cir_codice,
                   cir_descrizione,
                   (select pno_testo_note
                        from t_ana_tipo_note
                        join t_paz_note on tno_codice = pno_tno_codice
                        where pno_paz_codice = paz_codice
                        and tno_codice = :codice_note_annotazioni
                        and (
                            pno_azi_codice = :pno_azi_codice
                            or pno_azi_codice in (
                                select dis_codice 
                                from t_ana_distretti
                                where dis_usl_codice = :pno_azi_codice
                            )
                        )) paz_note,
                   paz_stato_anagrafico,
                   paz_sesso,
                   ves_n_richiamo,
                   ves_data_effettuazione,
                   vac_codice,
                   vac_descrizione,
                   vac_obbligatoria
                   
            FROM t_paz_pazienti
                join t_vac_eseguite on paz_codice = ves_paz_codice
                join t_ana_vaccinazioni on ves_vac_codice = vac_codice
                left join t_ana_comuni on paz_com_codice_residenza = com_codice
                left join t_ana_circoscrizioni on paz_cir_codice = cir_codice
                   
           WHERE paz_data_nascita <= :data_nascita_fine
                AND paz_data_nascita >= :data_nascita_inizio
                             AND (ves_data_effettuazione - paz_data_nascita) <= :giorni_vita
                AND ves_n_richiamo = :dosi
                AND EXISTS 
                (
                    SELECT 1
                    FROM t_vac_eseguite v2
                    WHERE v2.ves_paz_codice = paz_codice
                    AND v2.ves_vac_codice = vac_codice 
                    AND (v2.ves_data_effettuazione - paz_data_nascita) <= :giorni_vita
                    AND v2.ves_n_richiamo >= :dosi
                             {1}
                   )
                {0}
";
            }
        }

        public static string selElencoMediciVaccinazioni
        {
            get
            {
                return @"SELECT  vac_codice,
                                 vac_descrizione,
                                 vac_ordine,
                                 med_codice,
                                 med_descrizione,
           (SELECT   COUNT (DISTINCT paz_codice)
              FROM   t_paz_pazienti
             WHERE   paz_data_nascita <= :data_nascita_fine
                     AND paz_data_nascita >= :data_nascita_inizio
                     and paz_med_codice_base = med_codice
                     {0}
            ) AS num_pazienti
    from   t_ana_medici, t_ana_vaccinazioni";
            }
        }

        public static string selElencoNonVaccinatiMedico
        {
            get
            {
                return @"select   paz_codice,
  paz_cognome,
  paz_nome,
  paz_data_nascita,
  paz_indirizzo_residenza,
  com_descrizione,
  paz_com_codice_residenza,
  (select pno_testo_note
        from t_ana_tipo_note
        join t_paz_note on tno_codice = pno_tno_codice
        where pno_paz_codice = paz_codice
        and tno_codice = :codice_note_annotazioni
        and (
            pno_azi_codice = :pno_azi_codice
            or pno_azi_codice in (
                select dis_codice 
                from t_ana_distretti
                where dis_usl_codice = :pno_azi_codice
            )
        )) paz_note,
  (select pno_testo_note
    from t_ana_tipo_note
    join t_paz_note on tno_codice = pno_tno_codice
    where pno_paz_codice = paz_codice
    and tno_codice = :codice_note_appuntamenti
    and (
        pno_azi_codice = :pno_azi_codice
        or pno_azi_codice in (
            select dis_codice 
            from t_ana_distretti
            where dis_usl_codice = :pno_azi_codice
        )
    )) paz_libero_1,
  vac_codice,
  vac_descrizione,
  vac_ordine,
  paz_med_codice_base,
  (SELECT '1'
     from   t_paz_inadempienze
    where  pin_paz_codice = paz_codice
      and    pin_vac_codice = vac_codice) as inadempienza,
  (select moe_descrizione
     FROM   t_vac_escluse, t_ana_motivi_esclusione
    WHERE  vex_paz_codice = paz_codice
      and    vex_vac_codice = vac_codice
      and vex_moe_codice = moe_codice) AS moe_descrizione
from   t_ana_vaccinazioni,
       t_paz_pazienti a,
       t_ana_medici,
       t_ana_comuni
where   paz_com_codice_residenza = com_codice(+)
  and paz_med_codice_base = med_codice
  AND paz_data_nascita <= :data_nascita_fine
  and paz_data_nascita >= :data_nascita_inizio
  {0} -- filtri anagrafici 
  AND not exists (
    select   1
      from   t_vac_eseguite
     where   ves_vac_codice = vac_codice
       and   ves_paz_codice = paz_codice
       and   ves_n_richiamo >= :dosi
       and   (ves_data_effettuazione - paz_data_nascita) <= :giorni_vita
       {1} -- filtri vaccinazioni 
  )";
            }
        }

        public static string selElencoVaccinatiMedico
        {
            get
            {
                return @"select   paz_codice,
  paz_cognome,
  paz_nome,
  paz_data_nascita,
  paz_indirizzo_residenza,
  com_descrizione,
  paz_com_codice_residenza,
  (select pno_testo_note
        from t_ana_tipo_note
        join t_paz_note on tno_codice = pno_tno_codice
        where pno_paz_codice = paz_codice
        and tno_codice = :codice_note_annotazioni
        and (
            pno_azi_codice = :pno_azi_codice
            or pno_azi_codice in (
                select dis_codice 
                from t_ana_distretti
                where dis_usl_codice = :pno_azi_codice
            )
        )) paz_note,
  (select pno_testo_note
    from t_ana_tipo_note
    join t_paz_note on tno_codice = pno_tno_codice
    where pno_paz_codice = paz_codice
    and tno_codice = :codice_note_appuntamenti
    and (
        pno_azi_codice = :pno_azi_codice
        or pno_azi_codice in (
            select dis_codice 
            from t_ana_distretti
            where dis_usl_codice = :pno_azi_codice
        )
    )) paz_libero_1,
  vac_codice,
  vac_descrizione,
  vac_ordine,
  paz_med_codice_base
from   t_ana_vaccinazioni,
       t_paz_pazienti a,
       t_ana_medici,
       t_ana_comuni
where   paz_com_codice_residenza = com_codice(+)
  and paz_med_codice_base = med_codice
  AND paz_data_nascita <= :data_nascita_fine
  and paz_data_nascita >= :data_nascita_inizio
  {0} -- filtri anagrafici 
  AND exists (
    select   1
      from   t_vac_eseguite
     where   ves_vac_codice = vac_codice
       and   ves_paz_codice = paz_codice
       and   ves_n_richiamo >= :dosi
       and   (ves_data_effettuazione - paz_data_nascita) <= :giorni_vita
       {1} -- filtri vaccinazioni 
  )";
            }
        }
    
    }
}
