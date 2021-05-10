namespace Onit.OnAssistnet.OnVac.Queries.NotePazienti
{
    /// <summary>
    /// Query oracle relative alle note pazienti.
    /// </summary>
    public static class OracleQueries
    {

        /// <summary>
        /// Select delle note paziente
        /// </summary>
        public static string selNotePaziente
        {
            get
            {
                return @"select tno_codice, tno_descrizione, tno_ordine, tno_modificabile, tno_show_convocazioni, pno_id, pno_paz_codice, pno_testo_note, 
                            pno_azi_codice, pno_ute_id_ultima_modifica, pno_data_ultima_modifica
                            from t_ana_tipo_note
                            left join t_paz_note on tno_codice = pno_tno_codice
                            where pno_paz_codice = :pno_paz_codice
                            and (
                                pno_azi_codice = :pno_azi_codice
                                or pno_azi_codice in (
                                    select dis_codice 
                                    from t_ana_distretti
                                    where dis_usl_codice = :pno_azi_codice
                                )
                            )
                            union
                            select tno_codice, tno_descrizione, tno_ordine, tno_modificabile, tno_show_convocazioni, null pno_id, :pno_paz_codice pno_paz_codice, null pno_testo_note,
                            :pno_azi_codice pno_azi_codice, null pno_ute_id_ultima_modifica, null pno_data_ultima_modifica
                            from t_ana_tipo_note t1
                            where not exists (
                                select 1 
                                from t_paz_note 
                                where pno_tno_codice = t1.tno_codice
                                and pno_paz_codice = :pno_paz_codice
                                and (
                                    pno_azi_codice = :pno_azi_codice
                                    or pno_azi_codice in (
                                        select dis_codice 
                                        from t_ana_distretti
                                        where dis_usl_codice = :pno_azi_codice
                                    )
                                )
                            )
                            order by tno_ordine, tno_descrizione, tno_codice";
            }
        }

        /// <summary>
        /// Update delle note paziente
        /// </summary>
        public static string updNotePaziente
        {
            get
            {
                return @"update t_paz_note set            
                            PNO_TESTO_NOTE = :PNO_TESTO_NOTE, 
                            PNO_AZI_CODICE = :PNO_AZI_CODICE, 
                            PNO_UTE_ID_ULTIMA_MODIFICA = :PNO_UTE_ID_ULTIMA_MODIFICA, 
                            PNO_DATA_ULTIMA_MODIFICA = :PNO_DATA_ULTIMA_MODIFICA
                            where PNO_PAZ_CODICE = :PNO_PAZ_CODICE
                            and PNO_TNO_CODICE = :PNO_TNO_CODICE
                            and PNO_ID = :PNO_ID";
            }
        }

        /// <summary>
        /// Insert delle note paziente
        /// </summary>
        public static string insNotePaziente
        {
            get
            {
                return @"insert into t_paz_note
                            (PNO_PAZ_CODICE, PNO_TNO_CODICE, PNO_TESTO_NOTE, PNO_PAZ_CODICE_OLD, PNO_AZI_CODICE, PNO_UTE_ID_ULTIMA_MODIFICA, PNO_DATA_ULTIMA_MODIFICA)
                            values (:PNO_PAZ_CODICE, :PNO_TNO_CODICE, :PNO_TESTO_NOTE, :PNO_PAZ_CODICE_OLD, :PNO_AZI_CODICE, :PNO_UTE_ID_ULTIMA_MODIFICA, :PNO_DATA_ULTIMA_MODIFICA)";
            }
        }



        /// <summary>
        /// 
        /// </summary>
        public static string selExistsNota
        {
            get
            {
                return @"select 1
                            from t_ana_tipo_note
                            inner join t_paz_note on tno_codice = pno_tno_codice
                            where pno_paz_codice = :pno_paz_codice
                            and (
                                pno_azi_codice = :pno_azi_codice
                                or pno_azi_codice in (
                                    select dis_codice 
                                    from t_ana_distretti
                                    where dis_usl_codice = :pno_azi_codice
                                )
                            ) 
                            and pno_tno_codice = :pno_tno_codice";
            }
        }

    }
}