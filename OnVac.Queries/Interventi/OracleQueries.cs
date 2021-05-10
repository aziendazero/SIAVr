namespace Onit.OnAssistnet.OnVac.Queries.Interventi
{
    /// <summary>
    /// Queries interventi.
    /// </summary>
    public static class OracleQueries
    {
        /// <summary>
        /// Caricamento interventi
        /// </summary>
        public static string selInterventi
        {
            get
            {
                return @"select int_codice, int_descrizione, int_tipologia, int_durata
                    from t_ana_interventi 
                    {0}   
                    order by int_descrizione";
            }
        }

        /// <summary>
        /// Caricamento intervento
        /// </summary>
        public static string selIntervento
        {
            get
            {
                return @"select int_codice, int_descrizione, int_tipologia, int_durata
                    from t_ana_interventi where int_codice = :cod_int";
            }
        }

        /// <summary>
        /// Inserimento intervento
        /// NOTA: il RETURNING TO è valido per Oracle >= 10
        /// </summary>
        public static string insIntervento
        {
            get
            {
                return @"insert into t_ana_interventi 
                    (int_descrizione, int_tipologia, int_durata) 
                    values 
                    (:descrizione, :tipologia, :durata)
                    returning int_codice into :id";
            }
        }

        /// <summary>
        /// Cancellazione intervento
        /// </summary>
        public static string delIntervento
        {
            get
            {
                return @"delete from t_ana_interventi
                    where int_codice = :cod_int";
            }
        }

        /// <summary>
        /// Aggiornamento intervento
        /// </summary>
        public static string updIntervento
        {
            get
            {
                return @"update t_ana_interventi
                    set int_descrizione = :descrizione, int_tipologia = :tipologia, int_durata = :durata
                    where int_codice = :cod_int";
            }
        }

        /// <summary>
        /// Caricamento interventi paziente
        /// </summary>
        public static string selInterventiPaziente
        {
            get
            {
                return @"select pit_codice, pit_data_intervento, pit_durata, pit_note, pit_ope_codice, int_codice, int_descrizione, int_tipologia, ope_nome
                        from t_paz_interventi
                            inner join t_ana_interventi on pit_int_codice = int_codice
                            inner join t_ana_operatori on pit_ope_codice = ope_codice
                        where pit_paz_codice = :cod_paziente
                            and pit_ute_id_eliminazione is null and pit_data_eliminazione is null
                        order by {0}";
            }
        }

        /// <summary>
        /// Inserimento intervento paziente
        /// NOTA: il RETURNING TO è valido per Oracle >= 10
        /// </summary>
        public static string insInterventoPaziente
        {
            get
            {
                return @"insert into t_paz_interventi 
                        (pit_paz_codice, pit_int_codice, pit_data_intervento, pit_durata, pit_ope_codice, pit_note, pit_ute_id_registrazione, pit_data_registrazione) 
                        values 
                        (:cod_paz, :cod_int, :data_int, :durata, :operatore, :note, :uteReg, :dataReg)
                        returning pit_codice into :id";
            }
        }

        /// <summary>
        /// Aggiornamento intervento paziente
        /// </summary>
        public static string updInterventoPaziente
        {
            get
            {
                return @"update t_paz_interventi
                    set pit_int_codice = :cod_int, pit_data_intervento = :data_int, pit_durata = :durata, pit_ope_codice = :operatore, pit_note = :note
                    where pit_codice = :pit_codice";
            }
        }

        /// <summary>
        /// Cancellazione logica intervento
        /// </summary>
        public static string delInterventoPaziente
        {
            get
            {
                // return @"delete from t_paz_interventi
                //      where pit_codice = :pit_codice";
                return @"update t_paz_interventi
                    set pit_ute_id_eliminazione = :uteId, pit_data_eliminazione = :data
                    where pit_codice = :pit_codice";
            }
        }

        /// <summary>
        /// Conteggio interventi (non cancellati) del paziente
        /// </summary>
        public static string countInterventiPaziente
        {
            get
            {
                return @"select count(*)
                        from t_paz_interventi
                        where pit_paz_codice = :cod_paziente
                        and pit_ute_id_eliminazione is null and pit_data_eliminazione is null";
            }
        }
    }
}
