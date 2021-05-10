namespace Onit.OnAssistnet.OnVac.Queries.Convocazioni
{
    /// <summary>
    /// Query oracle relative alle convocazioni.
    /// </summary>
    public static class OracleQueries
    {

        /// <summary>
        /// 
        /// </summary>
        public static string selDatiPaziente
        {
            get
            {
                return @"select paz_codice, paz_cognome, paz_nome, paz_data_nascita, paz_cns_codice, paz_stato
from t_paz_pazienti
where paz_codice = :cod";
            }
        }

        /// <summary>
        /// 
        /// </summary>	
        public static string selVacSostByVacCod
        {
            get
            {
                return @"select vac_cod_sostituta
from t_ana_vaccinazioni
where vac_codice = :cod_vac";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string chkInadEsclVacByPaz
        {
            get
            {
                return @"select 1 from T_ANA_VACCINAZIONI
where VAC_CODICE = :cod_vac
and (
	exists (
		select 1 from T_PAZ_INADEMPIENZE
		where PIN_PAZ_CODICE = :cod_paz
		and PIN_VAC_CODICE = VAC_CODICE
	)
	or exists (
		select 1 from T_VAC_ESCLUSE
		where VEX_PAZ_CODICE = :cod_paz
		and VEX_VAC_CODICE = VAC_CODICE  
        AND (vex_data_scadenza is null  
	        or vex_data_scadenza > sysdate)
        )
)";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string selMaxRichiamo
        {
            get
            {
                return @"select max(ves_n_richiamo) max_richiamo
from t_vac_eseguite
where ves_paz_codice = :cod_paz
and ves_vac_codice = :cod_vac";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string selVacBySost
        {
            get
            {
                return @"select vac_codice
from t_ana_vaccinazioni
where vac_cod_sostituta = :cod_sost";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string selIntervalliSedute
        {
            get
            {
                return @"SELECT MAX (ves_data_effettuazione) ves_data_effettuazione, tsd_intervallo_prossima, tsd_n_seduta
    FROM t_vac_eseguite 
			join t_ana_vaccinazioni_sedute on (ves_vac_codice = sed_vac_codice AND ves_n_richiamo = sed_n_richiamo)
			join t_ana_tempi_sedute on (sed_cic_codice = tsd_cic_codice AND sed_n_seduta = tsd_n_seduta)
   WHERE ves_paz_codice = :cod_paz
     AND sed_n_seduta < :n_sed
     AND sed_cic_codice = :cod_ciclo
GROUP BY tsd_intervallo_prossima, tsd_n_seduta 
UNION 
SELECT MAX (ves_data_effettuazione) ves_data_effettuazione, tsd_intervallo_prossima, tsd_n_seduta
    FROM t_vac_eseguite 
        join t_ana_associazioni_sedute on (ves_vac_codice = sas_vac_codice AND ves_n_richiamo = sas_n_richiamo)
        join t_ana_tempi_sedute on (sas_cic_codice = tsd_cic_codice AND sas_n_seduta = tsd_n_seduta)
   WHERE ves_paz_codice = :cod_paz
     AND sas_n_seduta < :n_sed
     AND sas_cic_codice = :cod_ciclo
GROUP BY tsd_intervallo_prossima, tsd_n_seduta ORDER BY ves_data_effettuazione DESC, tsd_n_seduta desc";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string selDateCnvPaz
        {
            get
            {
                return @"select distinct vpr_cnv_data 
from t_vac_programmate 
where vpr_paz_codice = :cod_paz 
order by vpr_cnv_data";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string selDateCnvVacObbl
        {
            get
            {
                return @"select distinct vpr_cnv_data
from t_vac_programmate join t_ana_vaccinazioni on vpr_vac_codice = vac_codice
where vpr_paz_codice = :cod_paz
and vac_obbligatoria = 'A'
order by vpr_cnv_data";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string insVacProg
        {
            get
            {
                return @"insert into t_vac_programmate
(vpr_paz_codice, vpr_cnv_data, vpr_vac_codice, vpr_n_richiamo, vpr_cic_codice, vpr_n_seduta, vpr_ass_codice, vpr_data_inserimento, vpr_ute_id_inserimento)
values (:cod_paz, :data_cnv, :cod_vac, :n_richiamo, :cod_cic, :n_seduta, :cod_ass, :data_ins, :ute_ins)";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string countVacProg
        {
            get
            {
                return @"select count (*)
from t_vac_programmate
where vpr_paz_codice = :cod_paz";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string insCicloPaz
        {
            get
            {
                return @"insert into t_cnv_cicli
(cnc_cnv_paz_codice, cnc_cnv_data, cnc_cic_codice, cnc_sed_n_seduta, cnc_data_inserimento, cnc_ute_id_inserimento) 
values (:cod_paz, :data_cnv, :cod_cic, :n_seduta, :data_ins, :ute_ins)";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string selMaxFineSospPaz
        {
            get
            {
                return @"select max(vis_fine_sospensione)
from t_vis_visite
where vis_paz_codice = :cod_paz
and vis_fine_sospensione is not null";
            }
        }

        /// <summary>
        /// Restituisce i dati delle convocazioni da programmare, comprese le vaccinazioni associate
        /// </summary>
        public static string selCnvDaProgrammare
        {
            get
            {
                return @"select a.*, 
       (select vex_data_scadenza from t_vac_escluse
        where vex_paz_codice = :cod_paz 
        and vex_vac_codice = sed_vac_codice) mydata
from (
        select cic_codice, sed_vac_codice, sed_n_richiamo, nvl(tsd_eta_seduta, 0) tsd_eta_seduta,
               tsd_intervallo, sed_n_seduta, vac_obbligatoria, 1 ordine, sysdate convocazione, 
               tsd_durata_seduta, null sas_ass_codice, tsd_num_solleciti_rac
        from t_paz_cicli  
            join t_ana_cicli on (pac_cic_codice = cic_codice)
            join t_ana_tempi_sedute on (cic_codice = tsd_cic_codice)
            join t_ana_vaccinazioni_sedute on (tsd_cic_codice = sed_cic_codice and tsd_n_seduta = sed_n_seduta)
            join t_ana_vaccinazioni on (sed_vac_codice = vac_codice)
        where pac_paz_codice = :cod_paz
      union
        select cic_codice, sas_vac_codice sed_vac_codice, sas_n_richiamo sed_n_richiamo, nvl(tsd_eta_seduta, 0) tsd_eta_seduta,
               tsd_intervallo, sas_n_seduta sed_n_seduta, vac_obbligatoria, 1 ordine, sysdate convocazione, 
               tsd_durata_seduta, sas_ass_codice, tsd_num_solleciti_rac
        from t_paz_cicli
            join t_ana_cicli on (pac_cic_codice = cic_codice)
            join t_ana_tempi_sedute on (cic_codice = tsd_cic_codice)
            join t_ana_associazioni_sedute on (tsd_cic_codice = sas_cic_codice and tsd_n_seduta = sas_n_seduta)
            join t_ana_vaccinazioni on (sas_vac_codice = vac_codice)
        where pac_paz_codice = :cod_paz
) a
where sed_vac_codice not in
(
    select ves_vac_codice from t_vac_eseguite
    where ves_paz_codice = :cod_paz
    and ves_n_richiamo >= sed_n_richiamo
  union
    select pin_vac_codice from t_paz_inadempienze
    where pin_paz_codice = :cod_paz
  union
    select vex_vac_codice from t_vac_escluse
    where vex_paz_codice = :cod_paz
    and vex_data_scadenza is null 
  union
    select vpr_vac_codice from t_vac_programmate
    where vpr_paz_codice = :cod_paz
    and vpr_vac_codice = sed_vac_codice
    and not exists (
        select 1 from t_vac_eseguite
        where ves_paz_codice = :cod_paz
        and ves_vac_codice = vpr_vac_codice
        and ves_n_richiamo >= vpr_n_richiamo    
    ) and not exists (    
        select 1 from t_vac_escluse
        where vex_paz_codice = :cod_paz
        and vex_vac_codice = vpr_vac_codice 
        and vex_data_scadenza is null 
    )
)
order by cic_codice, sed_n_seduta";
            }
        }

        /// <summary>
        /// Restituisce i dati delle convocazioni programmate, comprese le vaccinazioni associate
        /// </summary>
        public static string selCnvProgrammate
        {
            get
            {
                return @"select vpr_cnv_data convocazione, vpr_cic_codice cic_codice, vpr_ass_codice sas_ass_codice,
vpr_vac_codice sed_vac_codice, vpr_n_seduta sed_n_seduta, vpr_n_richiamo sed_n_richiamo, 0 tsd_intervallo
from t_cnv_convocazioni 
join t_vac_programmate on cnv_paz_codice = vpr_paz_codice and cnv_data = vpr_cnv_data 
where cnv_paz_codice = :cod_paz 
order by vpr_cnv_data, vpr_cic_codice, vpr_ass_codice";
            }
        }

        /// <summary>
        /// Select dei dati della convocazione specificata (solo la convocazione, senza vaccinazioni)
        /// </summary>
        public static string selDatiCnv
        {
            get
            {
                return @"select * from t_cnv_convocazioni where cnv_paz_codice = :cnv_paz_codice and cnv_data = :cnv_data";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string updConvocazione
        {
            get
            {
                return @"update t_cnv_convocazioni set {0}
where cnv_paz_codice = :cod_paz
and cnv_data = :data_cnv";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string delConvocazione
        {
            get
            {
                return @"delete from t_cnv_convocazioni
where cnv_paz_codice = :cod_paz
and cnv_data = :data_cnv";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string updDataAppuntamentoCnv
        {
            get
            {
                return @"update t_cnv_convocazioni
set cnv_data_appuntamento = :data_app
where cnv_paz_codice = :cod_paz
and cnv_data = :data_cnv";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string updDataAppuntamentoInvioCnv
        {
            get
            {
                return @"update t_cnv_convocazioni
set cnv_data_appuntamento = :data_app, cnv_data_invio = :data_invio
where cnv_paz_codice = :cod_paz
and cnv_data = :data_cnv";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string selCountCnvPazByData
        {
            get
            {
                return @"select count(*) from t_cnv_convocazioni
where cnv_data = :data_cnv
and cnv_paz_codice = :cod_paz";
            }
        }

        #region queries utilizzate nel metodo di unione delle convocazioni

        /// <summary>
        /// Conteggio programmate in comune nelle due date di convocazione specificate.
        /// Restituisce un risultato solo se la stessa vaccinazione è presente in entrambe le date di convocazione.
        /// </summary>
        public static string selCountVaccProgComuni
        {
            get
            {
                return @"select count(*), vpr_vac_codice
from t_vac_programmate
where vpr_paz_codice = :cod_paz
and (vpr_cnv_data = :data_cnv_1 or vpr_cnv_data = :data_cnv_2)
having count(*) > 1
group by vpr_vac_codice";
            }
        }

        /// <summary>
        /// Inserimento convocazione spostata
        /// </summary>
        public static string insConvocazioneSpostata
        {
            get
            {
                return @"insert into t_cnv_convocazioni (
cnv_paz_codice, cnv_data, cnv_eta_pomeriggio, cnv_rinvio, cnv_data_appuntamento, 
cnv_tipo_appuntamento, cnv_durata_appuntamento, cnv_data_invio, cnv_cns_codice, 
cnv_ute_id, cnv_primo_appuntamento, cnv_amb_codice, cnv_campagna, 
cnv_data_inserimento, cnv_ute_id_inserimento
) values (
:cod_paz, :cnv_data, :eta_pom, :rinvio, :data_app, 
:tipo_app, :durata, :data_invio, :cod_cns, 
:id_ute, :data_primo_app, :cod_amb, :campagna, 
:data_ins, :ute_ins)";
            }
        }

        /// <summary>
        /// Modifica convocazione spostata
        /// </summary>
        public static string updConvocazioneSpostata
        {
            get
            {
                return @"update t_cnv_convocazioni set 
cnv_eta_pomeriggio = :eta_pom, cnv_rinvio = :rinvio, cnv_data_appuntamento = :data_app, 
cnv_tipo_appuntamento = :tipo_app, cnv_durata_appuntamento = :durata, cnv_data_invio = :data_invio, 
cnv_cns_codice = :cod_cns, cnv_ute_id = :id_ute, cnv_primo_appuntamento = :data_primo_app, 
cnv_amb_codice = :cod_amb, cnv_campagna = :campagna
where cnv_paz_codice = :cod_paz
and cnv_data = :data_cnv";
            }
        }

        /// <summary>
        /// Update data convocazione delle vaccinazioni programmate associate alla convocazione che è stata unita.
        /// </summary>
        public static string updCnvUnita_VaccProg
        {
            get
            {
                return @"update t_vac_programmate
set vpr_cnv_data = :new_data_cnv
where vpr_paz_codice = :cod_paz
and vpr_cnv_data = :old_data_cnv";
            }
        }

        /// <summary>
        /// Inserimento cicli associati alla convocazione che è stata unita, nella nuova data di convocazione.
        /// </summary>
        public static string insCnvUnita_Cicli
        {
            get
            {
                return @"INSERT INTO t_cnv_cicli (cnc_cnv_paz_codice,
                         cnc_cnv_data,
                         cnc_cic_codice,
                         cnc_sed_n_seduta,
                         cnc_cnv_paz_codice_old,
                         cnc_flag_giorni_posticipo,
                         cnc_flag_posticipo_seduta,
                         cnc_n_sollecito,
                         cnc_data_invio_sollecito,
                         cnc_data_inserimento,
                         cnc_ute_id_inserimento)
   SELECT   cnc_cnv_paz_codice,
            :new_data_cnv cnc_cnv_data,
            cnc_cic_codice,
            cnc_sed_n_seduta,
            cnc_cnv_paz_codice_old,
            cnc_flag_giorni_posticipo,
            cnc_flag_posticipo_seduta,
            cnc_n_sollecito,
            cnc_data_invio_sollecito,
            cnc_data_inserimento,
            cnc_ute_id_inserimento
     FROM   t_cnv_cicli c1
    WHERE   cnc_cnv_paz_codice = :cod_paz AND cnc_cnv_data = :old_data_cnv
    AND NOT EXISTS 
    (
        select 1 
        from t_cnv_cicli c2
        where c1.cnc_cnv_paz_codice = c2.cnc_cnv_paz_codice
        and c1.cnc_cic_codice = c2.cnc_cic_codice
        and c1.cnc_sed_n_seduta = c2.cnc_sed_n_seduta
        and c2.cnc_cnv_data = :new_data_cnv
    )";
            }
        }

        /// <summary>
        /// Inserimento dei cicli associati alla convocazione specificata
        /// </summary>
        public static string updCnvUnita_Cicli
        {
            get
            {
                return @"update t_cnv_cicli c1
        set c1.cnc_cnv_data = :new_data_cnv
        where c1.cnc_cnv_paz_codice = :cod_paz
        and c1.cnc_cnv_data = :old_data_cnv";
            }
        }

        /// <summary>
        /// Delete cicli del paziente nella data di convocazione specificata.
        /// </summary>
        public static string delCnv_Cicli
        {
            get
            {
                return @"delete from t_cnv_cicli
where cnc_cnv_paz_codice = :cod_paz
and cnc_cnv_data = :cnv_data";
            }
        }

        /// <summary>
        /// Update data convocazione dei ritardi associati alla convocazione che è stata unita.
        /// </summary>
        public static string updCnvUnita_Ritardi
        {
            get
            {
                return @"update t_paz_ritardi
set pri_cnv_data = :new_data_cnv
where pri_paz_codice = :cod_paz
and pri_cnv_data = :old_data_cnv";
            }
        }

        /// <summary>
        /// Update dei campi durata appuntamento e campagna per la convocazione specificata.
        /// </summary>
        public static string updDatiConvocazioneSoloBilancio
        {
            get
            {
                return @"update t_cnv_convocazioni 
set cnv_durata_appuntamento = :durata, cnv_campagna = :campagna 
where cnv_paz_codice = :paz_codice 
and cnv_data = :cnv_data";
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public static string selCodiciPazientiDaConvocare
        {
            get
            {
                return @"select distinct paz_codice, paz_data_nascita 
from t_paz_pazienti left join t_paz_malattie on paz_codice = pma_paz_codice 
where paz_cns_codice = :cod_cns 
and paz_cancellato <> 'S' 
and paz_stato <> :inadempiente_tot 
and paz_stato_anagrafico_dett is null 
{0} 
and exists ( 
    select 1 from t_paz_cicli 
    where pac_paz_codice = paz_codice) 
{1} 
{2} 
order by paz_data_nascita desc";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string selConvocazioni
        {
            get
            {
                return @"select * from t_cnv_convocazioni
                            where cnv_paz_codice = :cnv_paz_codice
                            and (:flag_appuntamento is null or (cnv_data_appuntamento is null and :flag_appuntamento = 'N') or  (cnv_data_appuntamento is not null and :flag_appuntamento = 'S'))
                            order by cnv_data desc";
            }
        }

        /// <summary>
        /// Restituisce le convocazioni del paziente, con i dati delle vaccinazioni componenti
        /// </summary>
        public static string selConvocazioniVaccinazioniPaziente
        {
            get
            {
                return @"select cnv_paz_codice, cnv_data, cnv_cns_codice, cns_descrizione, cnv_data_appuntamento, 
cnv_amb_codice, amb_descrizione, vpr_vac_codice, vac_descrizione, vpr_n_richiamo, vpr_paz_codice_old
from t_cnv_convocazioni
    left join t_ana_consultori on cnv_cns_codice = cns_codice
    left join t_ana_ambulatori on cnv_amb_codice = amb_codice
    join t_vac_programmate on cnv_paz_codice = vpr_paz_codice and cnv_data = vpr_cnv_data
    join t_ana_vaccinazioni on vpr_vac_codice = vac_codice
where cnv_paz_codice = :codPaziente
order by cnv_data, vpr_vac_codice";
            }
        }

        /// <summary>
        /// Restituisce i cicli delle convocazioni del paziente scecificato
        /// </summary>
        public static string selCicliConvocazioniPaziente
        {
            get
            {
                return @"select * from t_cnv_cicli where cnc_cnv_paz_codice = :cnc_cnv_paz_codice";
            }
        }

        /// <summary>
        /// Restituisce i ritardi dei cicli delle convocazioni del paziente scecificato
        /// </summary>
        public static string selRitardiCicliConvocazioniPaziente
        {
            get
            {
                return @"select * from t_paz_ritardi where pri_paz_codice = :pri_paz_codice";
            }
        }
        
    }
}

