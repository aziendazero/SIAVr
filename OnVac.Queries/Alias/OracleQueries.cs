namespace Onit.OnAssistnet.OnVac.Queries.Alias
{
    /// <summary>
    /// Query oracle relative agli alias.
    /// </summary>
    public static class OracleQueries
    {

        /// <summary>
        /// Restituisce i campi impostati nella tabella t_ana_colonne_alias
        /// </summary>
        public static string selColonneAlias
        {
            get
            {
                return @"select col_tabella, col_colonna, col_colonna_old, col_indice, col_campi, col_ordine, col_campo_ordinamento from t_ana_colonne_alias order by col_ordine";
            }
        }

        /// <summary>
        /// Eliminazione alias dalla t_tmp_pazienti_alias
        /// </summary>
        public static string delAliasFromTemp
        {
            get
            {
                return @"delete from t_tmp_pazienti_alias where paz_codice = :cod_paz";
            }
        }

        /// <summary>
        /// Eliminazione alias dalla t_paz_pazienti
        /// </summary>
        public static string delAliasFromPazienti
        {
            get
            {
                return @"delete from t_paz_pazienti where paz_codice = :cod_paz";
            }
        }

        /// <summary>
        /// Lettura colonne dal catalogo di oracle, escluse le colonne riguardanti le informazioni di 
        /// esecuzione dell'alias (paz_codice_master, paz_data_alias, paz_ute_id)
        /// </summary>
        public static string selColumnsDatiAliasFromCatalog
        {
            get
            {
                return @"select column_name from user_tab_columns where upper(table_name) = 'T_TMP_PAZIENTI_ALIAS'
and upper(column_name) not in ('PAZ_CODICE_MASTER', 'PAZ_DATA_ALIAS', 'PAZ_UTE_ID')
order by column_id";
            }
        }

        public static string selColumnsDatiAliasFromCatalogCentrale
        {
            get
            {
                return @"select column_name from user_tab_columns where upper(table_name) = 'T_TMP_PAZIENTI_ALIAS_VAC'
and upper(column_name) not in ('PAZ_CODICE_MASTER', 'PAZ_DATA_ALIAS', 'PAZ_UTE_ID','PAZ_PAC_AZIENDA')
order by column_id";
            }
        }
        
        /// <summary>
        /// Lettura di tutte le colonne dal catalogo di oracle, comprese quelle riguardanti le informazioni di 
        /// esecuzione dell'alias (paz_codice_master, paz_data_alias, paz_ute_id)
        /// </summary>
        public static string selAllColumnsFromCatalog
        {
            get
            {
                return @"select column_name from user_tab_columns where upper(table_name) = 'T_TMP_PAZIENTI_ALIAS'
order by column_id";
            }
        }

        public static string selAllColumnsFromCatalogCentrale
        {
            get
            {
                return @"select column_name from user_tab_columns where upper(table_name) = 'T_TMP_PAZIENTI_ALIAS_VAC'
order by column_id";
            }
        }

        /// <summary>
        /// Inserimento alias nella t_tmp_pazienti_alias
        /// </summary>
        public static string insAliasIntoTemp
        {
            get
            {
                return @"insert into t_tmp_pazienti_alias 
({0}, paz_codice_master, paz_data_alias, paz_ute_id)
(select {0}, :codMaster, :dataAlias, :idUtente
from t_paz_pazienti where paz_codice = :codAlias)";
            }
        }

        public static string insAliasIntoTempCentrale
        {
            get
            {
                return @"insert into t_tmp_pazienti_alias_vac 
({0}, paz_codice_master, paz_data_alias, paz_ute_id, paz_pac_azienda)
(select {0}, :codMaster, :dataAlias, :idUtente, :paz_pac_azienda
from t_paz_pazienti_vac where paz_codice = :codAlias)";
            }
        }

        /// <summary>
        /// Ricerca degli alias nella tabella t_tmp_pazienti_alias
        /// </summary>
        public static string selPazientiAlias
        {
            get
            {
                return @"select {0}, 
res.com_descrizione res_descrizione, res.com_provincia res_prv, res.com_cap res_cap,
dom.com_descrizione dom_descrizione, dom.com_provincia dom_prv, dom.com_cap dom_cap,
nas.com_descrizione nas_descrizione, med_descrizione, cit_stato,
master.paz_cognome master_cognome, master.paz_nome master_nome, 
master.paz_data_nascita master_data_nascita, master.paz_codice_fiscale master_codice_fiscale
from t_tmp_pazienti_alias paz 
left join t_ana_medici on paz.paz_med_codice_base = med_codice
left join t_ana_comuni res on paz.paz_com_codice_residenza = res.com_codice
left join t_ana_comuni dom on paz.paz_com_codice_domicilio = dom.com_codice
left join t_ana_comuni nas on paz.paz_com_codice_nascita = nas.com_codice
left join t_ana_cittadinanze on paz.paz_cit_codice = cit_codice
join t_paz_pazienti master on paz.paz_codice_master = master.paz_codice
{1} order by paz.paz_cognome, paz.paz_nome, paz.paz_data_nascita";
            }
        }

        /// <summary>
        /// Ricerca degli alias nella tabella t_tmp_pazienti_alias
        /// filtrando in base ai dati del master (nella t_paz_pazienti)
        /// </summary>
        public static string selPazientiAliasFromPazienti
        {
            get
            {
                return @"select {0}, 
res.com_descrizione res_descrizione, res.com_provincia res_prv, res.com_cap res_cap,
dom.com_descrizione dom_descrizione, dom.com_provincia dom_prv, dom.com_cap dom_cap,
nas.com_descrizione nas_descrizione, med_descrizione, cit_stato, 
paz.paz_cognome master_cognome, paz.paz_nome master_nome, 
paz.paz_data_nascita master_data_nascita, paz.paz_codice_fiscale master_codice_fiscale
from t_paz_pazienti paz 
join t_tmp_pazienti_alias alias on paz.paz_codice = alias.paz_codice_master
left join t_ana_medici on alias.paz_med_codice_base = med_codice
left join t_ana_comuni res on alias.paz_com_codice_residenza = res.com_codice
left join t_ana_comuni dom on alias.paz_com_codice_domicilio = dom.com_codice
left join t_ana_comuni nas on alias.paz_com_codice_nascita = nas.com_codice
left join t_ana_cittadinanze on alias.paz_cit_codice = cit_codice
{1} order by alias.paz_cognome, alias.paz_nome, alias.paz_data_nascita";
            }
        }

        /// <summary>
        /// Conteggio risultati della ricerca degli alias nella tabella t_tmp_pazienti_alias
        /// </summary>
        public static string cntPazientiAlias
        {
            get
            {
                return @"select count(*) from t_tmp_pazienti_alias paz {0}";
            }
        }

        /// <summary>
        /// Conteggio risultati della ricerca degli alias nella tabella t_tmp_pazienti_alias, 
        /// filtrando in base ai dati del master (nella t_paz_pazienti)
        /// </summary>
        public static string cntPazientiAliasFromPazienti
        {
            get
            {
                return @"select count(*) from t_paz_pazienti paz 
join t_tmp_pazienti_alias alias on paz.paz_codice = alias.paz_codice_master
{0}";
            }
        }
        
        #region Queries unmerge

        /// <summary>
        /// Inserimento dati vaccinali dell'alias per l'operazione di unmerge
        /// </summary>
        public static string insDatiUnmergeAlias
        {
            get
            {
                return @"insert into {0} ({1},{2},{3})
                            select :codAlias, {2}, null 
                            from {0} Alias 
                            where Alias.{1} = :codMaster 
                            and Alias.{3} = :codAlias 
                            and not exists 
                            (
                                select 1 from {0} Master 
                                where {1} = :codAlias
                                {4}
                            ) {5}";
            }
        }

        /// <summary>
        /// Cancellazione dati vaccinali del master relativi all'alias per l'operazione di unmerge
        /// </summary>
        public static string delDatiUnmergeAlias
        {
            get
            {
                return @"delete from {0} where {1} = :codMaster and {2} = :codAlias";
            }
        }
        
        #endregion

        #region Queries merge info

        /// <summary>
        /// Recupero informazioni per la gestione del merge nel caso di tabelle padre-figlia con chiave esterna autogenerata.
        /// </summary>
        public static string selMergeInfo
        {
            get
            {
                return @"select tlm_nome_tabella_padre nome_tabella_padre, tlm_nome_tabella_figlia nome_tabella_figlia,
         tlm_campi_indice_padre campi_indice_padre, tlm_campi_indice_figlia campi_indice_figlia,
         tlm_campo_fk_padre campo_fk_padre, tlm_campo_fk_figlia campo_fk_figlia, tlm_nome_sequence nome_sequence,
         padre.tbm_campi_select campi_select_padre, padre.tbm_campo_cod_paziente campo_cod_paziente_padre, 
         padre.tbm_campo_cod_paziente_old campo_cod_paziente_old_padre, padre.tbm_ordine ordine_padre,
         figlia.tbm_campi_select campi_select_figlia, figlia.tbm_campo_cod_paziente campo_cod_paziente_figlia,
         figlia.tbm_campo_cod_paziente_old campo_cod_paziente_old_figlia, figlia.tbm_ordine ordine_figlia
from t_ana_link_tabelle_merge
join t_ana_tabelle_merge padre on tlm_nome_tabella_padre = padre.tbm_nome_tabella
join t_ana_tabelle_merge figlia on tlm_nome_tabella_figlia = figlia.tbm_nome_tabella
order by ordine_padre, ordine_figlia";
            }
        }

        public static string selNextSequenceValue
        {
            get
            {
                return @"select {0}.nextval from dual";
            }
        }

        /// <summary>
        /// Recupero informazioni per la gestione del merge nel caso di update diretto del codice paziente.
        /// </summary>
        public static string selMergeUpdateInfo
        {
            get
            {
                return @"select TMU_NOME_TABELLA, TMU_CAMPO_COD_PAZIENTE, TMU_CAMPO_COD_PAZIENTE_OLD, TMU_ORDINE from T_ANA_TABELLE_MERGE_UPDATE order by TMU_ORDINE";
            }
        }

        #endregion        
    }
}
