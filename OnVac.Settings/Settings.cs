using Onit.OnAssistnet.OnVac.DAL;
using Onit.Shared.NTier.Security;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace Onit.OnAssistnet.OnVac.Settings
{
    /// <summary>
    /// Parametri dell'applicativo. Nel file di configurazione presente nella directory di esecuzione,
    /// deve essere impostato il parametro "SoloParametriAggiornati". Valori possibili:
    /// - true: Tutti i parametri su db devono essere gestiti dalla libreria.
    ///			Eventuali parametri presenti su db ma non gestiti generano un'eccezione.
    /// - false: Possono essere presenti parametri su db non gestiti dalla libreria.
    ///			 In questo caso l'eccezione non viene lasciata risalire.
    ///	Se il parametro non è impostato, lo considera true.
    ///	Serve per poter utilizzare la libreria in una versione precedente rispetto al db.
    /// </summary>
    [Serializable]
    public class Settings
    {
        // Per aggiungere un parametro: 
        //		1 - Utilizzare lo stesso nome del parametro su db
        //		2 - Aggiungere il nome nell'enumerazione "SettingName" della region "Enumerazione Parametri"
        //		3 - Definire il parametro (variabile privata + proprietà pubblica read-only) nella region "Parametri"
        //		4 - Aggiungere il codice per l'estrazione tipizzata da db, nella funzione "SetParameters" della region "Metodi"


        // Per cancellare un parametro: 
        //		1 - Cancellare il nome dall'enumerazione "SettingName" della region "Enumerazione Parametri"
        //		2 - Cancellare la definizione dalla region "Parametri" (variabile privata e membro pubblico)
        //		3 - Cancellare il codice relativo alla valorizzazione, nella funzione "SetParameters" della region "Metodi"

        #region Const

        private const string CODICE_CONSULTORIO_ALLINEA = "ALLINEA";
        private const string CODICE_CONSULTORIO_ALLINEA_INTERNO = "ALLINEA_INTERNO";

        #endregion

        #region Private

        private bool SoloParametriAggiornati;

        #endregion

        #region Enumerazione Stati

        public enum FunctionalityStatus : short
        {
            DISABLED = 0,
            ENABLED = 1,
            ENABLED_SELECTED = 2,
        }

        #endregion

        #region Enumerazione Parametri

        public enum SettingName
        {
            _UNSPECIFIED,
            AGGGIORNI,
            AGGIORNACIRBYVIA,
            AGGIORNACNSBYCIR,
            AGGIORNACNSBYCOM,
            ALERT_AGGIORNAMENTO_DATI_CENTRALIZZATI,
            ALIAS_CONTROLLO_CODICI_REGIONALI,
            ALIAS_UPDATE_MASTER_NULL,
            ALIAS_UPDATE_MASTER_NULL_CAMPI_ESCLUSI,
            ALIAS_USA_CNS_MASTER_ANAGRAFICO,
            ALLINEA_ASSOCIAZIONI_ONICS_APP_ID,
            ALLINEA_ASSOCIAZIONI_RFC_ACTOR,
            ALLINEA_ASSOCIAZIONI_RFC_ENTE,
            ALLINEA_PAZIENTE_MODEL_TYPE,
            ALLINEA_PAZIENTE_SEND_OPERATIONS,
            ANAGPAZ_CAMPI_DISABILITATI_CEN,
            ANAGPAZ_CAMPI_DISABILITATI_LIVELLO_CERTIFICAZIONE,
            ANAGPAZ_CAMPI_DISABILITATI_LOC,
            ANAGPAZ_CAMPI_NASCOSTI,
            APP_ID_CENTRALE,
            APPETAPM,
            APPLIBERO,
            APPTITLE,
            ASSOCIA_LOTTI_ETA,
            ASSOCIAZIONE_AUTO_CV,
            ASSOCIAZIONI_TIPO_CNS,
            AUTOAGGIORNACIRBYVIA,
            AUTOAGGIORNACNSDECEDUTI,
            AUTOAGGIORNACNSADU,
            AUTOAGGIORNACNSADU_CRITERIOSELEZIONE,
            AUTOAGGIORNACNSBYCIR,
            AUTOAGGIORNACNSBYCOM,
            AUTOALLINEA,
            AUTO_CF,
            AUTO_CALC_CICLI,
            AUTOCNVAPP,
            AUTOCONV,
            AUTOSETCNS_INSLOCALE,
            AUTO_STATO_ANAG_CHECK_LOCALE,
            AUTO_STATO_ANAG_DA_IMMIGRATO_A_RESIDENTE,
            AUTO_STATO_ANAG_SOSTITUZIONE_IMMIGRATO,
            AVVISI_STAMPA_AMBULATORIO,
            BILANCI_PREVALORIZZA_OPERATORI,
            BIZ_PAZIENTE_TYPE,
            CALCOLA_COD_AUSILIARIO,
            CALCOLABILOBBPRECEDENTI,
            CALCOLOCNV_STOP_PRIMA_OBBLIGATORIA,
            CAMPFON,
            CAMPVACCINALE,
            CATEGORIE_CITTADINO_CESSATO,
            CENTRALE_CAMPIFOND,
            CENTRALE_CHECK_INTEGRITY,
            CENTRALE_CHECK_UNICF,
            CENTRALE_CHECK_UNITESSERA,
            CENTRALE_LOG_FILE,
            CENTRALE_SEPAANAG,
            CENTRALE_SEPATIPO,
            CENTRALE_STORVAR,
            CENTRALE_WS_XMPI,
            CERTIFICATO_VACCINALE_NOTA_VALIDITA,
            CHECKVALCOMUNI,
            CHECK_CICLI,
            CHECK_CICLI_ERRORE,
            CHECK_DATI_ALIAS_PER_MERGE,
            CHECK_SITO_INOCULO,
            CHECK_VIA_SOMMINISTRAZIONE,
            CHK_ETA_CONSULT,
            CIRCOSCRIZIONE_OBBL,
            CITTADINANZA_DEFAULT,
            CNSCNV,
            CNS_DEFAULT,
            CNVAUTOFILTRAETA,
            CODESCL,
            CODESCLNOCICLO,
            CODESCLNONOBBL,
            CODESCLNONOBBLSETI,
            CODICE_ASL,
            CODICE_REGIONE,
            CODICE_UTENTE_PRENOTAZIONE_WEB,
            CODICI_MOTIVI_CESSAZIONE_ASSISTENZA,
            CODICI_VACCINAZIONI_COVID,
            CODNOMAL,
            COMDEFAULT,
            COMUNE_SCONOSCIUTO,
            COM_RES_BLOCCATI,
            CONDIZIONE_RISCHIO_DEFAULT,
            CONDIZIONE_RISCHIO_OBBLIGATORIA,
            CONDIZIONE_SANITARIA_DEFAULT,
            CONDIZIONE_SANITARIA_OBBLIGATORIA,
            CONDIZIONE_SANITARIA_TIPOLOGIE_MALATTIA,
            CONFLITTI_AUTORISOLUZIONE,
            CONSENSO_APP_ID,
            CONSENSO_BLOCCANTE_AUTO_EDIT,
            CONSENSO_GES,
            CONSENSO_ID_AUTORILEVAZIONE,
            CONSENSO_ID_COMUNICAZIONE,
            CONSENSO_ID_NON_VISIBILI,
            CONSENSO_KEY,
            CONSENSO_LOCALE,
            CONSENSO_GLOBALE_VISIBILITA_CONCESSA,
            CONSENSO_MSG_NO_COD_CENTRALE,
            CONSENSO_SEMAFORI_VISIBILI,
            CONSENSO_TIPOANAG,
            CONSENSO_URL,
            CONSENSO_VALORI_VISIBILITA_CONCESSA,
            CONSULTORIO_OBBL,
            CONVOCAZIONI_ALTRI_CONSULTORI,
            COV_CODICE_STATO_EPISODIO_GUARITO,
            COV_ESITO_POSITIVO_TAMPONE,
            COV_ESITO_NEGATIVO_TAMPONE,
            COV_LOG_INVIO_COMUNICAZIONE_OTP,
            COV_ORARIO_LIMITE_DIARIA,
            CHECK_ACQUISIZIONE_REGOLARIZZAZIONE,
            CTRL_ASSOCIABILITA_VAC,
            DATA_INIZIO_SOMMINISTRAZIONE_COVID,
            DEFTAPPL,
            DELCNVSTATO,
            DESCAT1,
            DESCAT2,
            DESLIB1,
            DESLIB2,
            DESLIB3,
            ESCLUDINONOBBLSETI,
            ESCLUDISENOCICLO,
            EXPORT_POSTEL_ARGOMENTO,
            EXPORT_POSTEL_TIPO_AVVISO_VISIBILE,
            FIRMADIGITALE_ANAMNESI_ON,
            FLAG_CANCELLATO_CHECK,
            FLAG_REGOLARIZZATO_DEFAULT,
            FSE_GESTIONE,
            FSE_OID_DOCUMENTI,
            FSE_OID_REGIONE_VENETO,
            FSE_TENTATIVI_STOP,
            FSE_TIPO_DOC_CERTIFICATO_VACCINALE,
            GES_APP_RICORDA_FILTRI,
            GES_APP_OPZIONI_VISUALIZZAZIONE,
            GES_AUTO_STATO_ANAGRAFICO,
            GES_CALCOLO_COPERTURA,
            GES_CALCOLO_SCADENZA_ESCLUSIONE,
            GES_DATA_CANC_OBBLIG,
            GES_DATA_IRREP_OBBLIG,
            GES_NOTE_AVVISI,
            GESBALOT,
            GESBIL,
            GESCICLISEDUTE,
            GESDATAVALIDITA,
            GESDOSISCATOLA,
            GESINSLOTTO,
            GESMAG,
            GESMODALITAACCESSO,
            GESSESSOCICLI,
            GESSOLLECITI,
            GESSOLLECITIBILANCI,
            GESTPAZ_CAMPO_ORDINAMENTO_MALATTIA,
            GESTPAZ_CAT_RISCHIO_OBSOLETE,
            GESTPAZ_DATA_OBBLIGATORIA_SE_DECEDUTO,
            GESTPAZ_MALATTIE_NON_MODIFICABILI,
            GESTPAZ_TIPOLOGIA_MALATTIA,
            GESVACCAMPAGNA,
            GESVACFITTIZIA,
            GESVIE,
            GIORNI_MODIFICA_BILANCIO_MEDICO,
            GIORNI_VARIAZIONE_VACCINAZIONE_ESEGUITA,
            ID_GRUPPO_ADMIN_DATI_VACCINALI,
            ID_GRUPPO_SUPERUSER,
            INFO_ASS_TEMPLATE_DESCRIZIONE,
            INFO_VAC_TEMPLATE_DESCRIZIONE,
            INSERIMENTO_PAZIENTE_ABILITATO,
            INSVIA,
            ISTAT_PROVINCIA,
            LASTORDVACESEGUITE,
            LEN_COGNOME,
            LEN_COGNOME_NOME,
            LEN_INDDOM,
            LEN_INDRES,
            LEN_LIBERO1,
            LEN_LIBERO2,
            LEN_LIBERO3,
            LEN_NOME,
            LEN_NOTE,
            LEN_NOTE_SOLLECITI,
            LEN_NOTE_CERTIFICATO,
            LEN_TEL1,
            LEN_TEL2,
            LEN_TEL3,
            LEN_TESSERA,
            LIBRETTO_VAC_MEDICO,
            LOCALE_PROVVISORIO,
            LOG_CAMBIOPAZIENTE,
            LOG_DATAACCESSLAYER,
            LOG_MAGAZZINO,
            LOG_POSTAZIONI,
            LOG_SESSIONCLEANER,
            LUOGHI,
            MANTOUX_CODICE_ASSOCIAZIONE,
            MANTOUX_CODICE_VACCINAZIONE,
            MAXPOSTAZIONI,
            MEDINAMB,
            MEDLOGIN,
            MEDVACLOGIN,
            MENUDIS,
            MENUDIS_APPUNTAMENTI,
            MENUDIS_HPAZIENTI,
            MOVCV_EDIT_STATO_ANAGRAFICO,
            MOVCV_STATI_ANAGRAFICI_DA_ESCLUDERE,
            N_RITARDATARI,
            CALCOLOAPP_MINUTI_LOCK_DISPONIBILITA,
            NUM_GIORNI_REGOLARIZZAZIONE,
            NUM_GIORNI_FOLLOWUP,
            NUM_MAX_GIORNI_REGOLARIZZAZIONE,
            NUM_MIN_VAC_PER_FASCIA_ORARIA,
            NUMAMB,
            NUMSOL,
            ORAPM,
            ORDCOLVACESEGUITE,
            PAGAMENTO,
            PAGAMENTO_AUTO_CHECK_TIPO,
            POLO_ONVAC,
            POSTAZIONI,
            PPA_VARIABILI_NO_DUPLICA,
            REAZIONE_AVVERSA_INTEGRAZIONE,
            REGPROC_INTERVALLO_GIORNI_FILTRO_ESECUZIONE,
            REGVAC_ASSCODICEDESCRIZIONE,
            REPORT,
            REPORT_VAC_GIORN,
            RICERCA_APP_SET_AMB_CONVOCAZIONE,
            RICERCA_PAZ_FILTRO_CNS_SET_DEFAULT,
            RICERCA_PAZ_FOCUS,
            RICERCA_PAZ_MAX_RECORDS,
            RICERCA_PAZ_ORDINAMENTO,
            RICERCA_PAZ_QPV2,
            RICERCA_PAZ_QPV2_API_ENDPOINT,
            RICERCA_PAZ_SHOW_CODICE_AUSILIARIO,
            RICERCA_PAZ_SHOW_CODICE_PAZIENTE,
            RICERCA_PAZ_SHOW_CODICE_REGIONALE,
            RICERCA_PAZ_SHOW_FILTRO_CNS,
            RICERCA_PAZ_SHOW_FILTRO_COMUNE_RESIDENZA,
            RICERCA_PAZ_SHOW_FLAG_CANCELLATO,
            RICONDUZIONE_INS_PAZ,
            RICONDUZIONE_INS_PAZ_CAMPI_RICERCA,
            SCARTO_MASSIMO,
            SED_AUTO,
            SED_MANU,
            SESSIONCLEANER,
            SET_AMB_CALENDARIO,
            SET_AMB_ELENCO_CALENDARIO,
            SITO_INOCULAZIONE_SET_DEFAULT,
            SOSPOBBLIGATORIA,
            SPOSTAMENTO_ASSISTITI_MOV_CNS,
            STATIANAG_CANCAPP,
            STATIANAG_CANCCNV,
            STATIANAG_INSERT_PAZIENTE,
            STATIANAG_MOVCV_PAZ_INTERNI,
            STATIANAG_RICALCOLACNS,
            STOPCNV_NONOBBL,
            TEMPOBIL,
            TEMPOESCLUSIONE,
            TEMPOINADEMPIENZA,
            TEMPORIT,
            TEMPOSED,
            TESSCEN,
            TIPOANAG,
            TIPOANAG_CATEGORIA_RISCHIO,
            TIPOANAG_MALATTIE,
            TIPOCNV,
            TIPOFILTROSTAMPATP,
            TIPOPAGAMENTO_DEFAULT,
            TUTTECNV,
            UNIFICAZIONE_IN_CORSO,
            UPDCNV_DELAPP,
            UPDCNV_UPDAPP,
            UPDCNV_UPDCNS,
            USER_SOLO_CNS_ABILITATI,
            USESQLEXPRESSION,
            VACPROG_ATTIVAZIONE_LOTTO,
            VACPROG_BIL_CONSEGNATO_A,
            VACPROG_BLOCCO_DECEDUTI,
            VACPROG_CERTIFICATOVACCINALE,
            VACPROG_ELIMINARIGHE,
            VACPROG_MODVACEFFETTUATE,
            VACPROG_NOMECOMMERCIALE,
            VACPROG_SETVACCINATORE,
            VACPROG_TIPOLOGIA_MALATTIA,
            VAC_STATI_NON_ESEGUITE,
            VALIDITA_SB,
            VALORI_VISIBILITA_VACC_CENTRALE,
            VIA_SOMMINISTRAZIONE_SET_DEFAULT,
            VISITE_STESSA_DATA,
            VISNOTE
        }

        #endregion

        #region Parametri

        /// <summary>
        /// Valore di aggiustamento giorni (su db vale 1,014583)
        /// </summary>
        public decimal AGGGIORNI { get; private set; }

        /// <summary>
        /// Aggiorna circoscrizione paziente tramite la via di residenza
        /// </summary>
        public bool AGGIORNACIRBYVIA { get; private set; }

        /// <summary>
        /// Aggiorna cns pazienti in base all'eta' ed alla circoscrizione
        /// </summary>
        public bool AGGIORNACNSBYCIR { get; private set; }

        /// <summary>
        /// Aggiorna cns pazienti in base all'eta' e al comune di residenza
        /// </summary>
        public bool AGGIORNACNSBYCOM { get; private set; }

        /// <summary>
        /// Visualizzazione del messaggio di aggiornamento dei dati centralizzati
        /// </summary>
        public bool ALERT_AGGIORNAMENTO_DATI_CENTRALIZZATI { get; private set; }

        /// <summary>
        /// Controlla se master e alias hanno entrambi codice regionale. In questo caso, non effettua il merge.
        /// </summary>
        public bool ALIAS_CONTROLLO_CODICI_REGIONALI { get; private set; }

        /// <summary>
        /// Aggiorna i campi nulli del master con quelli dell'alias (nella procedura di accorpamento degli alias)
        /// </summary>
        public bool ALIAS_UPDATE_MASTER_NULL { get; private set; }

        /// <summary>
        /// Campi che, durante il merge, non verranno copiati dall'alias al master se nulli (la copia avviene solo se ALIAS_UPDATE_MASTER_NULL==true). 
        /// NB: i nomi dei campi devono essere quelli della entity Paziente.
        /// </summary>
        public List<string> ALIAS_UPDATE_MASTER_NULL_CAMPI_ESCLUSI { get; private set; }

        /// <summary>
        /// Se vale true: forza il mantenimento del cns del master anagrafico; se vale false: utilizza il cns del master vaccinale.
        /// </summary>
        public bool ALIAS_USA_CNS_MASTER_ANAGRAFICO { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string ALLINEA_ASSOCIAZIONI_ONICS_APP_ID { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string ALLINEA_ASSOCIAZIONI_RFC_ACTOR { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string ALLINEA_ASSOCIAZIONI_RFC_ENTE { get; private set; }

        /// <summary>
        /// Libreria custom utilizzata nell'allineamento di un paziente
        /// </summary>
        public string ALLINEA_PAZIENTE_MODEL_TYPE { get; private set; }

        /// <summary>
        /// Operazioni per cui vengono generati messaggi di allineamento relativi al paziente
        /// </summary>
        public ReadOnlyCollection<OnAssistnet.MID.OperazionePaziente> ALLINEA_PAZIENTE_SEND_OPERATIONS { get; private set; }

        /// <summary>
        /// Campi dell'anagrafe paziente disabilitati se il paziente e' in centrale
        /// </summary>
        public List<string> ANAGPAZ_CAMPI_DISABILITATI_CEN { get; private set; }

        /// <summary>
        /// Campi dell'anagrafe paziente disabilitati se il livello di certificazione del paziente e' diverso da 0
        /// </summary>
        public List<string> ANAGPAZ_CAMPI_DISABILITATI_LIVELLO_CERTIFICAZIONE { get; private set; }

        /// <summary>
        /// Campi dell'anagrafe paziente disabilitati se il paziente e' in locale
        /// </summary>
        public List<string> ANAGPAZ_CAMPI_DISABILITATI_LOC { get; private set; }

        /// <summary>
        /// Campi nascosti dell'anagrafe paziente
        /// </summary>
        public List<string> ANAGPAZ_CAMPI_NASCOSTI { get; private set; }

        /// <summary>
        /// usata in modalità multi-ulss
        /// </summary>
        public string APP_ID_CENTRALE { get; private set; }

        /// <summary>
        /// Eta per calcolo appuntamenti pomeridiani
        /// </summary>
        public int APPETAPM { get; private set; }

        /// <summary>
        /// Assegnamento manuale degli orari di appuntamento
        /// </summary>
        public bool APPLIBERO { get; private set; }

        /// <summary>
        /// Titolo dell'applicativo
        /// </summary>
        public string APPTITLE { get; private set; }

        /// <summary>
        /// true/false: se i lotti attivi vengono associa in automatico, controlla eta min e max specificate per il lotto nel cns corrente
        /// </summary>
        public bool ASSOCIA_LOTTI_ETA { get; private set; }

        /// <summary>
        /// true/false: abilita l'associazione automatica dei consultori per i nuovi utenti
        /// </summary>
        public bool ASSOCIAZIONE_AUTO_CV { get; private set; }

        /// <summary>
        /// true/false: imposta se utilizzare solo le associazioni legate al tipo di consultorio corrente (vacprog/campagna)
        /// </summary>
        public bool ASSOCIAZIONI_TIPO_CNS { get; private set; }

        /// <summary>
        /// Aggiorna automaticamente circoscrizione pazienti in base alla via
        /// </summary>
        public bool AUTOAGGIORNACIRBYVIA { get; private set; }

        /// <summary>
        /// Passaggio automatico al consultorio adulti in base al comune di residenza o domicilio
        /// </summary>
        public FunctionalityStatus AUTOAGGIORNACNSDECEDUTI { get; private set; }

        /// <summary>
        /// Passaggio automatico al consultorio adulti in base al comune di residenza o domicilio
        /// </summary>
        public FunctionalityStatus AUTOAGGIORNACNSADU { get; private set; }

        /// <summary>
        /// Passaggio automatico al consultorio adulti in base al comune di residenza o domicilio
        /// </summary>
        public string AUTOAGGIORNACNSADU_CRITERIOSELEZIONE { get; private set; }

        /// <summary>
        /// Aggiorna automaticamente cns pazienti in base all'eta' ed alla circoscrizione
        /// </summary>
        public bool AUTOAGGIORNACNSBYCIR { get; private set; }

        /// <summary>
        /// Aggiorna automaticamente cns pazienti in base all'eta' e al comune di residenza
        /// </summary>
        public bool AUTOAGGIORNACNSBYCOM { get; private set; }

        /// <summary>
        /// Allineamento automatico dall'anagrafe centrale (sempre, non solo sul modifica)
        /// </summary>
        public bool AUTOALLINEA { get; private set; }

        /// <summary>
        /// Calcolo automatico del codice fiscale (true/false)
        /// </summary>
        public bool AUTO_CF { get; private set; }

        /// <summary>
        /// Calcolo automatico cicli
        /// </summary>
        public bool AUTO_CALC_CICLI { get; private set; }

        /// <summary>
        /// Calcolo automatico delle convocazioni mancanti in gestione appuntamenti
        /// </summary>
        public bool AUTOCNVAPP { get; private set; }

        /// <summary>
        /// Calcolo automatico della convocazione quando non presente
        /// </summary>
        public bool AUTOCONV { get; private set; }

        /// <summary>
        /// Calcolo automatico del consultorio del paziente inserito in locale
        /// </summary>
        public bool AUTOSETCNS_INSLOCALE { get; private set; }

        /// <summary>
        /// Indica se controllare il flag locale in caso di ricalcolo dello stato anagrafico
        /// </summary>
        public bool AUTO_STATO_ANAG_CHECK_LOCALE { get; private set; }

        /// <summary>
        /// Se il nuovo stato anagrafico calcolato e' IMMIGRATO e quello originale fa parte di questa lista, lo stato viene impostato a RESIDENTE 
        /// </summary>
        public List<Enumerators.StatoAnagrafico> AUTO_STATO_ANAG_DA_IMMIGRATO_A_RESIDENTE { get; private set; }

        /// <summary>
        /// Se valorizzato, riporta il valore dello stato anagrafico con cui sostituire lo stato "IMMIGRATO".
        /// </summary>
        public Enumerators.StatoAnagrafico? AUTO_STATO_ANAG_SOSTITUZIONE_IMMIGRATO { get; private set; }

        /// <summary>
        /// Indica se deve essere stampata la descrizione dell'ambulatorio nell'avviso appuntamento.
        /// N.B. : questo parametro non è utilizzato da codice ma solo nella query della v_avvisi.
        /// </summary>
        public bool AVVISI_STAMPA_AMBULATORIO { get; private set; }

        /// <summary>
        /// Indica se pre-valorizzare i campi relativi agli operatori (medico e rilevatore) nella creazione di un bilancio o di una visita.
        /// </summary>
        public bool BILANCI_PREVALORIZZA_OPERATORI { get; private set; }

        /// <summary>
        /// Classe utilizzata via reflection come biz del paziente
        /// </summary>
        public string BIZ_PAZIENTE_TYPE { get; private set; }

        /// <summary>
        /// Indica se, all'inserimento di un paziente in locale, deve essere calcolato il codice ausiliario
        /// </summary>
        public bool CALCOLA_COD_AUSILIARIO { get; private set; }

        /// <summary>
        /// Calcola anche i bilanci obbligatori precedenti all'eta' del paziente
        /// </summary>
        public bool CALCOLABILOBBPRECEDENTI { get; private set; }

        /// <summary>
        /// Se vale true, calcola le convocazioni e si ferma alla prima che contiene un'obbligatoria. Altrimenti, le calcola tutte
        /// </summary>
        public bool CALCOLOCNV_STOP_PRIMA_OBBLIGATORIA { get; private set; }

        /// <summary>
        /// Elenco dei campi fondamentali per cui richiedere la password in modifica in centrale
        /// </summary>
        public string CAMPFON { get; private set; }

        /// <summary>
        /// Indica se e' attiva la campagna vaccinale
        /// </summary>
        public bool CAMPVACCINALE { get; private set; }

        /// <summary>
        /// Lista di codici categoria cittadino che rappresentano lo stato "CESSATO" 
        /// </summary>
        public List<string> CATEGORIE_CITTADINO_CESSATO { get; private set; }

        /// <summary>
        /// Controlla la validita dei comuni
        /// </summary>
        public bool CHECKVALCOMUNI { get; private set; }

        /// <summary>
        /// Stringa contenente l'elenco dei nomi dei campi fondamentali, per i quali un' eventuale modifica comporta la modifica del campo Paz_Tipo. I nomi dei campi devono essere separati da un ';'
        /// </summary>
        public string CENTRALE_CAMPIFOND { get; private set; }

        /// <summary>
        /// Controllo di integrità referenziale sulle anagrafiche centrali: 0 = non viene effettuato il controllo, 1 = viene effettuato il controllo
        /// </summary>
        public bool CENTRALE_CHECK_INTEGRITY { get; private set; }

        /// <summary>
        /// Controllo dell'unicità del codice fiscale inserito o modificato: 0 = non viene effettuato alcun controllo, 1 = viene effettuato il controllo e nel caso di non unicità si interrompe l'operazione che si stava eseguendo
        /// </summary>
        public bool CENTRALE_CHECK_UNICF { get; private set; }

        /// <summary>
        /// Controllo dell'unicità della tessera inserita o modificata:  0 = non viene effettuato alcun controllo, 1 = viene effettuato il controllo e nel caso di non unicità si interrompe l'operazione che si stava eseguendo
        /// </summary>
        public bool CENTRALE_CHECK_UNITESSERA { get; private set; }

        /// <summary>
        /// Salvataggio log su disco: 
        /// 0 = non viene salvato alcun Log, 
        /// 1 = viene salvato solo il Log di Errore, 
        /// 2 = viene salvato un Log in ogni caso
        /// </summary>
        public Enumerators.CentraleLogFile CENTRALE_LOG_FILE { get; private set; }

        /// <summary>
        /// Separatore utilizzato per separare cognome e nome paziente
        /// </summary>
        public string CENTRALE_SEPAANAG { get; private set; }

        /// <summary>
        /// Formato cognome e nome in anagrafe centrale
        /// </summary>
        public Enumerators.CentraleTipoAnagrafe CENTRALE_SEPATIPO { get; private set; }

        /// <summary>
        /// Salvataggio nella tabella dello storico variazioni in caso di inserimento o modifica:  
        /// 0 = non viene salvato record, 
        /// 1 = viene salvato un record adeguato nello storico variazioni
        /// </summary>
        public bool CENTRALE_STORVAR { get; private set; }

        /// <summary>
        /// Utilizza il web service di xmpi per la scrittura in centrale
        /// </summary>
        public bool CENTRALE_WS_XMPI { get; private set; }

        /// <summary>
        /// Normativa per la stampa cartacea del certificato agli organi della PA
        /// </summary>
        public string CERTIFICATO_VACCINALE_NOTA_VALIDITA { get; private set; }

        /// <summary>
        /// Al termine del calcolo del flag di regolarizzazione, se il paziente è NON REGOLARIZZATO, 
        /// viene controllato lo stato acquisizione per impostare il paziente a REGOLARIZZATO se l'acquisizione è TOTALE.
        /// </summary>
        public bool CHECK_ACQUISIZIONE_REGOLARIZZAZIONE { get; private set; }

        /// <summary>
        /// Indica se effettuare il controllo di congruenza dei cicli
        /// </summary>
        public bool CHECK_CICLI { get; private set; }

        /// <summary>
        /// Indica se il controllo di congruenza dei cicli viene considerato un errore
        /// </summary>
        public bool CHECK_CICLI_ERRORE { get; private set; }

        /// <summary>
        /// Controllo che alias sia con (paz_codice_ausiliario is null or paz_cancellato = 'S') per effettuare il merge
        /// </summary>
        public bool CHECK_DATI_ALIAS_PER_MERGE { get; private set; }

        /// <summary>
        /// Controlla l'obbligatorieta' del campo in  inserimento e update delle vaccinazioni eseguite
        /// </summary>
        public bool CHECK_SITO_INOCULO { get; private set; }

        /// <summary>
        /// Controlla l'obbligatorieta' del campo in  inserimento e update delle vaccinazioni eseguite
        /// </summary>
        public bool CHECK_VIA_SOMMINISTRAZIONE { get; private set; }

        /// <summary>
        /// Controllo sull'eta' prima di assegnare il consultorio corrente
        /// </summary>
        public bool CHK_ETA_CONSULT { get; private set; }

        /// <summary>
        /// Indica se il campo circoscrizione del paziente e' obbligatorio
        /// </summary>
        public bool CIRCOSCRIZIONE_OBBL { get; private set; }

        /// <summary>
        /// Codice della cittadinanza di default
        /// </summary>
        public string CITTADINANZA_DEFAULT { get; private set; }

        /// <summary>
        /// Codice della cittadinanza sconosciuta
        /// </summary>
        public string CITTADINANZA_SCONOSCIUTA { get; private set; }

        /// <summary>
        /// P/L: indica se selezionare in automatico il consultorio del paziente o quello di lavoro
        /// </summary>
        public string CNSCNV { get; private set; }

        /// <summary>
        /// Centro vaccinale di default associato all'utente 
        /// </summary>
        public string CNS_DEFAULT { get; private set; }

        /// <summary>
        /// Indica se utilizz. il filtro di eta nella generazione delle cnv o prendere tutti i paz del cns
        /// </summary>
        public bool CNVAUTOFILTRAETA { get; private set; }

        /// <summary>
        /// Codice del motivo di esclusione per le vaccinazioni spostate
        /// </summary>
        public string CODESCL { get; private set; }

        /// <summary>
        /// Codice del motivo di esclusione per le vaccinazioni senza ciclo
        /// </summary>
        public string CODESCLNOCICLO { get; private set; }

        /// <summary>
        /// Stringa con codice del motivo esclusione (per le vacc facoltative spostate)
        /// </summary>
        public string CODESCLNONOBBL { get; private set; }

        /// <summary>
        /// Codice del motivo di esclusione per le vaccinazioni facoltative se il paziente e' totalmente inademp
        /// </summary>
        public string CODESCLNONOBBLSETI { get; private set; }

        /// <summary>
        /// Codice asl corrente
        /// </summary>
        public string CODICE_ASL { get; private set; }

        /// <summary>
        /// Codice regione corrente
        /// </summary>
        public string CODICE_REGIONE { get; private set; }

        /// <summary>
        /// Id dell'utente di prenotazione_web
        /// </summary>
        public int CODICE_UTENTE_PRENOTAZIONE_WEB { get; private set; }

        /// <summary>
        /// Lista di codici dei motivi di cessazione assistenza
        /// </summary>
        public List<string> CODICI_MOTIVI_CESSAZIONE_ASSISTENZA { get; private set; }

        /// <summary>
        /// Lista dei codici relativi alle vaccinazioni anti covid
        /// </summary>
        public List<string> CODICI_VACCINAZIONI_COVID { get; private set; }

        /// <summary>
        /// Codice della malattia 'nessuna malattia'
        /// </summary>
        public string CODNOMAL { get; private set; }

        /// <summary>
        /// Comune di residenza e domicilio di default nel caricamento del file di aggiornamento
        /// </summary>
        public string COMDEFAULT { get; private set; }

        /// <summary>
        /// Codice relativo al comune sconosciuto.
        /// </summary>
        public string COMUNE_SCONOSCIUTO { get; private set; }

        /// <summary>
        /// Codici dei comuni da non mostrare nella modale del comune di residenza del paziente
        /// </summary>
        public string COM_RES_BLOCCATI { get; private set; }

        /// <summary>
        /// Codice della categoria di rischio da associare di default al paziente all'esecuzione di una vaccinazione
        /// </summary>
        public string CONDIZIONE_RISCHIO_DEFAULT { get; private set; }

        /// <summary>
        /// Controlla l'obbligatorieta' del campo in inserimento e update delle vaccinazioni eseguite
        /// </summary>
        public bool CONDIZIONE_RISCHIO_OBBLIGATORIA { get; private set; }

        /// <summary>
        /// Codice della malattia da associare di default al paziente all'esecuzione di una vaccinazione
        /// </summary>
        public string CONDIZIONE_SANITARIA_DEFAULT { get; private set; }

        /// <summary>
        /// Controlla l'obbligatorieta' del campo in inserimento e update delle vaccinazioni eseguite
        /// </summary>
        public bool CONDIZIONE_SANITARIA_OBBLIGATORIA { get; private set; }

        /// <summary>
        /// Elenco di codici di tipologie di malattia, separati da ";", da utilizzare per filtrare le malattie da usare come condizioni sanitarie
        /// </summary>
        public List<string> CONDIZIONE_SANITARIA_TIPOLOGIE_MALATTIA { get; private set; }

        /// <summary>
        /// Applica la logica di risoluzione automatica dei conflitti ad ogni funzionalita' relativa ai dati centralizzati
        /// </summary>
        public bool CONFLITTI_AUTORISOLUZIONE { get; private set; }

        /// <summary>
        /// Id dell'applicazione per la gestione del consenso
        /// </summary>
        public string CONSENSO_APP_ID { get; private set; }

        /// <summary>
        /// Nel caso in cui il paziente abbia un consenso bloccante che impedisce l'accesso ai dati, 
        /// se vale true viene aperta in edit la maschera di rilevazione del consenso
        /// </summary>
        public bool CONSENSO_BLOCCANTE_AUTO_EDIT { get; private set; }

        /// <summary>
        /// true: abilita la gestione del consenso (indicatori e pulsanti apertura popup programma di rilevazione consenso)
        /// false: nessuna gestione del consenso
        /// </summary>
        public bool CONSENSO_GES { get; private set; }

        /// <summary>
        /// Elenco delle accoppiate id consenso e id livello che verranno impostati dalla funzione automatica di rilevazione del consenso
        /// </summary>
        public List<Entities.Consenso.ConsensoAutoRilevazione> CONSENSO_ID_AUTORILEVAZIONE { get; private set; }

        /// <summary>
        /// Id del consenso alle comunicazioni specificato nella t_ana_consensi
        /// </summary>
        public int? CONSENSO_ID_COMUNICAZIONE { get; private set; }

        /// <summary>
        /// Lista degli id dei consensi non visibili dalla maschera di gestione del consenso
        /// </summary>
        public List<int> CONSENSO_ID_NON_VISIBILI { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string CONSENSO_KEY { get; private set; }

        /// <summary>
        /// Indica se l'icona di stato del consenso viene visualizzata solo per i pazienti presenti in locale o per tutti i pazienti
        /// </summary>
        public bool CONSENSO_LOCALE { get; private set; }

        /// <summary>
        /// Valori del consenso globale da considerare come visibilità concessa
        /// </summary>
        public List<string> CONSENSO_GLOBALE_VISIBILITA_CONCESSA { get; private set; }

        /// <summary>
        /// Messaggio di alert x la gestione del consenso, nel caso in cui il codice ausiliario del paziente non sia valorizzato
        /// </summary>
        public string CONSENSO_MSG_NO_COD_CENTRALE { get; private set; }

        /// <summary>
        /// Visualizza/nasconde l'icona semaforo indicatore del consenso
        /// </summary>
        public bool CONSENSO_SEMAFORI_VISIBILI { get; private set; }

        /// <summary>
        /// Stessi valori del parametro TIPOANAG. 
        /// Sono gestiti: 0 locale, 2 centrale lettura/scrittura.
        /// </summary>
        public Enumerators.TipoAnags CONSENSO_TIPOANAG { get; private set; }

        /// <summary>
        /// Url dell'applicazione per la rilevazione del consenso
        /// </summary>
        public string CONSENSO_URL { get; private set; }

        /// <summary>
        /// Valori del consenso alla comunicazione da considerare come visibilità concessa
        /// </summary>
        public List<string> CONSENSO_VALORI_VISIBILITA_CONCESSA { get; private set; }

        /// <summary>
        /// Indica se il campo consultorio del paziente e' obbligatorio
        /// </summary>
        public bool CONSULTORIO_OBBL { get; private set; }

        /// <summary>
        /// Indica il tipo di gestione relativa alle convocazioni su consultori diversi da quello corrente.
        /// 0: non visibili; 1: read-only; 2: read & write
        /// </summary>
        public Enumerators.TipoGestioneConvocazioniAltriConsultori CONVOCAZIONI_ALTRI_CONSULTORI { get; private set; }

        /// <summary>
        /// Valore di codifica stato episodio "guarito"
        /// </summary>
        public string COV_CODICE_STATO_EPISODIO_GUARITO { get; private set; }

        /// <summary>
        /// Valore di codifica in caso di esito positivo del tampone
        /// </summary>
        public string COV_ESITO_POSITIVO_TAMPONE { get; private set; }

        /// <summary>
        /// Valore di codifica in caso di esito negativo del tampone
        /// </summary>
        public string COV_ESITO_NEGATIVO_TAMPONE { get; private set; }

        /// <summary>
        /// Abilita/disabilita il log di invio della comunicazione OTP a SAR
        /// </summary>
        public bool COV_LOG_INVIO_COMUNICAZIONE_OTP { get; private set; }

        /// <summary>
        /// Orario limite per l'inserimento di una diaria da parte dell'utente dell'app, nel formato HH:mm
        /// </summary>
        public string COV_ORARIO_LIMITE_DIARIA { get; private set; }

        /// <summary>
        /// Abilita/Disabilita il controllo sulla non associabilità dei vaccini da somministrare in una stessa data
        /// </summary>
        public bool CTRL_ASSOCIABILITA_VAC { get; private set; }

        /// <summary>
        /// Data di inizio di somministrazione del vaccino anti-covid
        /// </summary>
        public DateTime? DATA_INIZIO_SOMMINISTRAZIONE_COVID { get; private set; }

        /// <summary>
        /// Nome di default dell'applicativo, utilizzato dal modulo di gestione del consenso. 
        /// Non è propriamente un parametro di OnVac, però è stato incluso per evitare l'eccezione dovuta alla mancanza di un parametro.
        /// </summary>
        public string DEFTAPPL { get; private set; }

        /// <summary>
        /// Elimina le convocazioni se lo stato anagrafico viene modificato in "STATIANAG_CANCCNV"
        /// </summary>
        public bool DELCNVSTATO { get; private set; }

        /// <summary>
        /// Descrizione categoria jolly 1
        /// </summary>
        public string DESCAT1 { get; private set; }

        /// <summary>
        /// Descrizione categoria jolly 2
        /// </summary>
        public string DESCAT2 { get; private set; }

        /// <summary>
        /// Campo libero n. 1
        /// </summary>
        public string DESLIB1 { get; private set; }

        /// <summary>
        /// Campo libero n. 2
        /// </summary>
        public string DESLIB2 { get; private set; }

        /// <summary>
        /// Campo libero n. 3
        /// </summary>
        public string DESLIB3 { get; private set; }

        /// <summary>
        /// Indica se escludere le vaccinazioni facoltative per i paz totalm inadempienti
        /// </summary>
        public bool ESCLUDINONOBBLSETI { get; private set; }

        /// <summary>
        /// Indica se nella gestione dei solleciti le vaccinazioni senza ciclo vengono escluse o se viene sbiancato l'appuntamento
        /// </summary>
        public bool ESCLUDISENOCICLO { get; private set; }

        /// <summary>
        /// Codice dell'argomento dell'export dati avvisi per tracciato postel
        /// </summary>
        public string EXPORT_POSTEL_ARGOMENTO { get; private set; }

        /// <summary>
        /// AV|SL|TP: Imposta la visibilita sui tipi di avviso postel
        /// </summary>
        public List<string> EXPORT_POSTEL_TIPO_AVVISO_VISIBILE { get; private set; }

        /// <summary>
        /// S|N: attiva o disattiva la funzionalità di firma digitale dei documenti di anamnesi.
        /// </summary>
        public bool FIRMADIGITALE_ANAMNESI_ON { get; private set; }

        /// <summary>
        /// Se il parametro vale true, viene controllato il campo paz_cancellato.
        /// Se il paziente risulta cancellato, alcune funzionalità verranno bloccate.
        /// </summary>
        public bool FLAG_CANCELLATO_CHECK { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? FLAG_REGOLARIZZATO_DEFAULT { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool FSE_GESTIONE { get; private set; }

        /// <summary>
        /// OID per gli identificativi dei documenti della regione. Sostituire il segnaposto con il codice della ulss senza lo 0 iniziale
        /// </summary>
        public string FSE_OID_DOCUMENTI { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string FSE_OID_REGIONE_VENETO { get; private set; }

        /// <summary>
        /// Numero massimo di tentativi di invio di un messaggio ad FSE. Se 0, il parametro non viene considerato (vengono ritentati infiniti invii)
        /// </summary>
        public int FSE_TENTATIVI_STOP { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string FSE_TIPO_DOC_CERTIFICATO_VACCINALE { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? GES_APP_RICORDA_FILTRI { get; private set; }

        /// <summary>
        /// C|V|M|B: iniziali dei checkbox selezionati di default
        /// </summary>
        public List<string> GES_APP_OPZIONI_VISUALIZZAZIONE { get; private set; }

        /// <summary>
        /// Gestione dello stato anagrafico in automatico
        /// </summary>
        public bool GES_AUTO_STATO_ANAGRAFICO { get; private set; }

        /// <summary>
        /// S/N: Visualizza il calcolo della copertura avanzato
        /// </summary>
        public bool GES_CALCOLO_COPERTURA { get; private set; }

        /// <summary>
        /// S/N: Abilita negli archivi la selezione del tipo di calcolo della scadenza per una esclusione
        /// </summary>
        public bool GES_CALCOLO_SCADENZA_ESCLUSIONE { get; private set; }

        /// <summary>
        /// Indica se il campo data cancellazione del paziente e' obbligatorio
        /// </summary>
        public bool GES_DATA_CANC_OBBLIG { get; private set; }

        /// <summary>
        /// Indica se il campo data irreperibilita' del paziente e' obbligatorio
        /// </summary>
        public bool GES_DATA_IRREP_OBBLIG { get; private set; }

        /// <summary>
        /// True: aggiunge il campo Note al report "Avviso appuntamento"
        /// </summary>
        public bool GES_NOTE_AVVISI { get; private set; }

        /// <summary>
        /// Gestione bilanci
        /// </summary>
        public bool GESBIL { get; private set; }

        /// <summary>
        /// Gestione associazione lotti con codice a barre
        /// </summary>
        public bool GESBALOT { get; private set; }

        /// <summary>
        /// Rende visibile i lpulsante cicli-sedute in gestione appuntamenti
        /// </summary>
        public bool GESCICLISEDUTE { get; private set; }

        /// <summary>
        /// Gestione del filtro data validita' nella prenotazione degli appuntamenti
        /// </summary>
        public bool GESDATAVALIDITA { get; private set; }

        /// <summary>
        /// Abilita/disabilita gestione dosi nell'inserimento di lotti
        /// </summary>
        public bool GESDOSISCATOLA { get; private set; }

        /// <summary>
        /// Gestisce abilita/disabilita il bottone ins.lotto nella maschera vacprog
        /// </summary>
        public bool GESINSLOTTO { get; private set; }

        /// <summary>
        /// Gestione lotti e magazzino
        /// </summary>
        public bool GESMAG { get; private set; }

        /// <summary>
        /// Gestione della scelta della modalita' di accesso
        /// </summary>
        public bool GESMODALITAACCESSO { get; private set; }

        /// <summary>
        /// Possibilità di specificare il sesso nei cicli
        /// </summary>
        public bool GESSESSOCICLI { get; private set; }

        /// <summary>
        /// Gestione dei solleciti
        /// </summary>
        public bool GESSOLLECITI { get; private set; }

        /// <summary>
        /// Gestione dei solleciti di bilancio
        /// </summary>
        public bool GESSOLLECITIBILANCI { get; private set; }

        /// <summary>
        /// Campo ordinamento per griglia delle malattie della maschera della gestione pazienti
        /// </summary>
        public string GESTPAZ_CAMPO_ORDINAMENTO_MALATTIA { get; private set; }

        /// <summary>
        /// Se vale S, mostra anche le categorie di rischio obsolete in gestione paziente
        /// </summary>
        public bool GESTPAZ_CAT_RISCHIO_OBSOLETE { get; private set; }

        /// <summary>
        /// Campo per verificare se data decesso del paziente è obbligatoria
        /// </summary>
        public bool GESTPAZ_DATA_OBBLIGATORIA_SE_DECEDUTO { get; private set; }

        /// <summary>
        /// Codici delle malattie che non possono essere modificate ne' eliminate in gestione paziente
        /// </summary>
        public List<string> GESTPAZ_MALATTIE_NON_MODIFICABILI { get; private set; }

        // Tipologie di malattia visibili dalla maschera della gestione pazienti
        public List<string> GESTPAZ_TIPOLOGIA_MALATTIA { get; private set; }

        /// <summary>
        /// Gestione della visualizzazione del campo "vaccinazione in campagna" nelle programmate
        /// </summary>
        public bool GESVACCAMPAGNA { get; private set; }

        /// <summary>
        /// Gestione della registrazione da reg storico vaccinale del campo VES_FLAG_FITTIZIA = 'S'
        /// </summary>
        public bool GESVACFITTIZIA { get; private set; }

        /// <summary>
        /// Gestione delle vie codificate
        /// </summary>
        public bool GESVIE { get; private set; }

        /// <summary>
        /// Massimo numero di giorni entro cui un medico può modificare un bilancio
        /// </summary>
        public int GIORNI_MODIFICA_BILANCIO_MEDICO { get; private set; }

        /// <summary>
        /// Numero massimo di giorni trascorsi i quali il dato vaccinale non è più modificabile
        /// </summary>
        public int GIORNI_VARIAZIONE_VACCINAZIONE_ESEGUITA { get; private set; }

        /// <summary>
        /// Id del gruppo di appartenenza degli utenti amministratori dei dati vaccinali
        /// </summary>
        public string ID_GRUPPO_ADMIN_DATI_VACCINALI { get; private set; }

        /// <summary>
        /// Id del gruppo superuser
        /// </summary>
        public string ID_GRUPPO_SUPERUSER { get; private set; }

        /// <summary>
        /// Template di default per le informazioni sulle associazioni, utilizzato per una nuova associazione
        /// </summary>
        public string INFO_ASS_TEMPLATE_DESCRIZIONE { get; private set; }

        /// <summary>
        /// Template di default per le informazioni sulle vaccinazioni, utilizzato per una nuova vaccinazione
        /// </summary>
        public string INFO_VAC_TEMPLATE_DESCRIZIONE { get; private set; }

        /// <summary>
        /// Abilita/disabilita il pulsante di inserimento paziente nella maschera di ricerca
        /// </summary>
        public bool INSERIMENTO_PAZIENTE_ABILITATO { get; private set; }

        /// <summary>
        /// Via inserita se non presente durante aggiornamento pazienti
        /// </summary>
        public bool INSVIA { get; private set; }

        /// <summary>
        /// Inizio del codice istat per identificare tutti i comuni della stessa provincia (prime 3 cifre)
        /// </summary>
        public string ISTAT_PROVINCIA { get; private set; }

        /// <summary>
        /// Ordine vac_eseguite applicato alla fine
        /// </summary>
        public string LASTORDVACESEGUITE { get; private set; }

        /// <summary>
        /// Lunghezza campo cognome
        /// </summary>
        public int LEN_COGNOME { get; private set; }

        /// <summary>
        /// Lunghezza massima cognome e nome incluso il carattere separatore
        /// </summary>
        public int LEN_COGNOME_NOME { get; private set; }

        /// <summary>
        /// Lunghezza indirizzo domicilio
        /// </summary>
        public int LEN_INDDOM { get; private set; }

        /// <summary>
        /// Lunghezza indirizzo residenza
        /// </summary>
        public int LEN_INDRES { get; private set; }

        /// <summary>
        /// Lunghezza massima campo libero1
        /// </summary>
        public int LEN_LIBERO1 { get; private set; }

        /// <summary>
        /// Lunghezza massima campo libero2
        /// </summary>
        public int LEN_LIBERO2 { get; private set; }

        /// <summary>
        /// Lunghezza massima campo libero3
        /// </summary>
        public int LEN_LIBERO3 { get; private set; }

        /// <summary>
        /// Lunghezza campo nome
        /// </summary>
        public int LEN_NOME { get; private set; }

        /// <summary>
        /// Lunghezza massima campo note 
        /// </summary>
        public int LEN_NOTE { get; private set; }

        /// <summary>
        /// Lunghezza massima campo note 
        /// </summary>
        public int LEN_NOTE_SOLLECITI { get; private set; }

        /// <summary>
        /// Lunghezza massima campo note certificato
        /// </summary>
        public int LEN_NOTE_CERTIFICATO { get; private set; }

        /// <summary>
        /// Lunghezza campo telefono 1
        /// </summary>
        public int LEN_TEL1 { get; private set; }

        /// <summary>
        /// Lunghezza campo telefono 2
        /// </summary>
        public int LEN_TEL2 { get; private set; }

        /// <summary>
        /// Lunghezza campo telefono 3
        /// </summary>
        public int LEN_TEL3 { get; private set; }

        /// <summary>
        /// Lunghezza campo tessera
        /// </summary>
        public int LEN_TESSERA { get; private set; }

        /// <summary>
        /// Indica il campo del medico da riportare sul libretto vaccinale; VES_OPE_CODICE (medico responsabile) o VES_MED_VACCINANTE (vaccinatore)
        /// </summary>
        public string LIBRETTO_VAC_MEDICO { get; private set; }

        /// <summary>
        /// Indica se il locale si strova in uno stato provvisorio per cui si valorizza il paz_tipo
        /// </summary>
        public bool LOCALE_PROVVISORIO { get; private set; }

        /// <summary>
        /// SS	NO = No Log / SS = Log Sempre / SD = log
        /// </summary>
        public bool LOG_CAMBIOPAZIENTE { get; private set; }

        /// <summary>
        /// Gestione log del layer di accesso ai dati
        /// </summary>
        public bool LOG_DATAACCESSLAYER { get; private set; }

        /// <summary>
        /// Gestione log dei movimenti di magazzino
        /// </summary>
        public bool LOG_MAGAZZINO { get; private set; }

        /// <summary>
        /// Gestione log delle postazioni che si collegano all'applicativo
        /// </summary>
        public bool LOG_POSTAZIONI { get; private set; }

        /// <summary>
        /// Gestione log della pulizia della session
        /// </summary>
        public bool LOG_SESSIONCLEANER { get; private set; }

        /// <summary>
        /// Luoghi da includere altre a quelli standard nell'esecuzione delle vaccinazioni
        /// </summary>
        public string LUOGHI { get; private set; }

        /// <summary>
        /// Codice relativo alla mantoux in anagrafe associazioni
        /// </summary>
        public string MANTOUX_CODICE_ASSOCIAZIONE { get; private set; }

        /// <summary>
        /// Codice relativo alla mantoux in anagrafe vaccinazioni
        /// </summary>
        public string MANTOUX_CODICE_VACCINAZIONE { get; private set; }

        /// <summary>
        /// Numero massimo di postazioni associabili all'applicativo
        /// </summary>
        public int MAXPOSTAZIONI { get; private set; }

        /// <summary>
        /// Check medico in ambulatorio default
        /// </summary>
        public bool MEDINAMB { get; private set; }

        /// <summary>
        /// Medico responsabile della seduta al login
        /// </summary>
        public bool MEDLOGIN { get; private set; }

        /// <summary>
        /// Richiede il medico che esegue la vaccinazione ad ogni seduta vaccinale
        /// </summary>
        public bool MEDVACLOGIN { get; private set; }

        /// <summary>
        /// Elenco dei men_weight (separati da pipe) dei menu disabilitati impostati dal manager relativi alla installazione corrente, presi dalla t_ana_menu
        /// </summary>
        public List<string> MENUDIS { get; private set; }

        /// <summary>
        /// MEN_WEIGHT del menu Appuntamenti (impostato dal manager per l'installazione corrente, preso dalla t_ana_menu)
        /// </summary>
        public string MENUDIS_APPUNTAMENTI { get; private set; }

        /// <summary>
        /// MEN_WEIGHT del menu HPazienti (impostato dal manager per l'installazione corrente, preso dalla t_ana_menu)
        /// </summary>
        public string MENUDIS_HPAZIENTI { get; private set; }

        /// <summary>
        /// Indica se il campo "STATO ANAGRAFICO" è modificabile nelle maschere dei movimenti di centro vaccinale
        /// </summary>
        public bool MOVCV_EDIT_STATO_ANAGRAFICO { get; private set; }

        /// <summary>
        /// Indica se nel campo "STATO ANAGRAFICO" è da escludere il vaolore deceduto
        /// </summary>
        public List<string> MOVCV_STATI_ANAGRAFICI_DA_ESCLUDERE { get; private set; }
        public int CALCOLOAPP_MINUTI_LOCK_DISPONIBILITA { get; private set; }
        /// <summary>
        /// Esprime il numero di ritardatari da inserire in ogni giorno per il calcolo degli appuntamenti
        /// </summary>
        public int N_RITARDATARI { get; private set; }

        /// <summary>
        /// Numero di giorni dalla nascita entro i quali il paziente è considerato regolarizzato
        /// </summary>
        public int NUM_GIORNI_FOLLOWUP { get; private set; }

        /// <summary>
        /// Numero di giorni dalla nascita entro i quali il paziente è considerato regolarizzato
        /// </summary>
        public int NUM_GIORNI_REGOLARIZZAZIONE { get; private set; }

        /// <summary>
        /// Numero di giorni dalla nascita dopo i quali il paziente è considerato regolarizzato
        /// </summary>
        public int? NUM_MAX_GIORNI_REGOLARIZZAZIONE { get; private set; }

        /// <summary>
        /// Numero di giorni dalla nascita dopo i quali il paziente è considerato regolarizzato
        /// </summary>
        public int NUM_MIN_VAC_PER_FASCIA_ORARIA { get; private set; }

        /// <summary>
        /// Numero di ambulatori che e' possibile inserire
        /// </summary>
        public int NUMAMB { get; private set; }

        /// <summary>
        /// Massimo numero solleciti per notifica
        /// </summary>
        public int NUMSOL { get; private set; }

        /// <summary>
        /// Orario per distinzione mattina/pomeriggio
        /// </summary>
        public string ORAPM { get; private set; }

        /// <summary>
        /// Ordine colonne vac_eseguite
        /// </summary>
        public string ORDCOLVACESEGUITE { get; private set; }

        /// <summary>
        /// Abilita la gestione delle vaccinazioni a pagamento
        /// </summary>
        public bool PAGAMENTO { get; private set; }

        /// <summary>
        /// Guid da impostare automaticamente come tipo pagamento se viene selezionato il check della vaccinazione a pagamento (VacProg). Vale null se non deve essere gestito automaticamente.
        /// </summary>
        public string PAGAMENTO_AUTO_CHECK_TIPO { get; private set; }

        /// <summary>
        /// Codice del polo per calendario cup wssgp
        /// </summary>
        public string POLO_ONVAC { get; private set; }

        /// <summary>
        /// S = bloccante / N = non bloccante
        /// </summary>
        public bool POSTAZIONI { get; private set; }

        /// <summary>
        /// Elenco delle osservazioni per cui i valori non sono da duplicare 
        /// </summary>
        public ICollection<int> PPA_VARIABILI_NO_DUPLICA { get; private set; }

        /// <summary>
        /// Flag per attivare integrazione della reazione avversa
        /// </summary>
        public bool REAZIONE_AVVERSA_INTEGRAZIONE { get; private set; }

        /// <summary>
        /// Intervallo di giorni del filtro delle date di esecuzione nel registro processi
        /// </summary>
        public int? REGPROC_INTERVALLO_GIORNI_FILTRO_ESECUZIONE { get; private set; }

        /// <summary>
        /// Indica se in registrazione vaccinazioni viene visualizzato codice (0) o desc (1) per l'associaz
        /// </summary>
        public string REGVAC_ASSCODICEDESCRIZIONE { get; private set; }

        /// <summary>
        /// Cartella con report di stampa
        /// </summary>
        public string REPORT { get; private set; }
        /// <summary>
        ///  E VALORIZZATO A S IL BOTTONE DI STAMPA btnStampaCertificatoVaccGior E'' VISIBILE NELLA MASCHERA DELLE VACCINAZIONI ESEGUITE
        /// </summary>
        public bool REPORT_VAC_GIORN { get; private set; }

        /// <summary>
        /// Indica se, in Ricerca Appuntamenti, deve essere impostato come ambulatorio di prenotazione l'ambulatorio selezionato nelle programmate
        /// </summary>
        public bool RICERCA_APP_SET_AMB_CONVOCAZIONE { get; private set; }

        /// <summary>
        /// In ricerca pazienti, indica se valorizzare in automatico il filtro sul centro vaccinale con quello corrente
        /// </summary>
        public bool RICERCA_PAZ_FILTRO_CNS_SET_DEFAULT { get; private set; }

        /// <summary>
        /// Campo che riceve il focus nel dettaglio dei pazienti
        /// </summary>
        public string RICERCA_PAZ_FOCUS { get; private set; }

        /// <summary>
        /// Numero massimo di record restituiti dalla ricerca paziente
        /// </summary>
        public int? RICERCA_PAZ_MAX_RECORDS { get; private set; }

        /// <summary>
        /// Elenco campi di ordinamento per la ricerca pazienti
        /// </summary>
        public string RICERCA_PAZ_ORDINAMENTO { get; private set; }

        /// <summary>
        /// Se vale true, ricerca in anagrafe nazionale abilitata
        /// </summary>
        public bool RICERCA_PAZ_QPV2 { get; private set; }

        /// <summary>
        /// Url dell'API che effettua la ricerca in anagrafe nazionale
        /// </summary>
        public string RICERCA_PAZ_QPV2_API_ENDPOINT { get; private set; }

        /// <summary>
        /// Indica se, in Ricerca Pazienti, deve essere visualizzato il codice ausiliario nella griglia dei risultati della ricerca
        /// </summary>
        public bool RICERCA_PAZ_SHOW_CODICE_AUSILIARIO { get; private set; }

        /// <summary>
        /// Indica se, in Ricerca Pazienti, deve essere visualizzato il codice paziente nei filtri di ricerca e nella griglia dei risultati della ricerca
        /// 0 nessuno 1 soloSuperutenti 2 tutti
        /// </summary>
        public Enumerators.TipoAutorizzazione RICERCA_PAZ_SHOW_CODICE_PAZIENTE { get; private set; }

        /// <summary>
        /// Indica se, in Ricerca Pazienti, deve essere visualizzato il codice regionale nella griglia dei risultati della ricerca
        /// </summary>
        public bool RICERCA_PAZ_SHOW_CODICE_REGIONALE { get; private set; }

        /// <summary>
        /// Indica se nella ricerca pazienti il filtro per cns deve essere visualizzato
        /// </summary>
        public bool RICERCA_PAZ_SHOW_FILTRO_CNS { get; private set; }

        /// <summary>
        /// Indica se, in Ricerca Pazienti, deve essere visualizzato il filtro di ricerca per comune di residenza
        /// </summary>
        public bool RICERCA_PAZ_SHOW_FILTRO_COMUNE_RESIDENZA { get; private set; }

        /// <summary>
        /// Indica se, in Ricerca Pazienti, deve essere visualizzato il cancellato nella griglia dei risultati della ricerca
        /// </summary>
        public bool RICERCA_PAZ_SHOW_FLAG_CANCELLATO { get; private set; }

        /// <summary>
        /// Indica se prima di inserire un paziente deve essere tentata la riconduzione dello stesso ad un paziente già presente
        /// </summary>
        public bool RICONDUZIONE_INS_PAZ { get; private set; }

        /// <summary>
        /// Lista dei campi con cui tentare la riconduzione del paziente (elenco separato da ;)
        /// </summary>
        public List<string> RICONDUZIONE_INS_PAZ_CAMPI_RICERCA { get; private set; }

        /// <summary>
        /// Giorni di scarto massimo per inserire un bilancio in una convocazione (se flag visita = "s")
        /// </summary>
        public int SCARTO_MASSIMO { get; private set; }

        /// <summary>
        /// Durata fissa per appuntamenti automatici
        /// </summary>
        public int SED_AUTO { get; private set; }

        /// <summary>
        /// Durata fissa per appuntamenti manuali
        /// </summary>
        public int SED_MANU { get; private set; }

        /// <summary>
        /// Abilita/disabilita la pulizia della sessione
        /// </summary>
        public bool SESSIONCLEANER { get; private set; }

        /// <summary>
        /// Memorizza codice ambulatorio scelto nel calendario dopo la prima scelta
        /// </summary>
        public bool SET_AMB_CALENDARIO { get; private set; }

        /// <summary>
        /// Valorizza il codice ambulatorio nelle programmate in base a quello presente nella convocazione
        /// </summary>
        public bool SET_AMB_ELENCO_CALENDARIO { get; private set; }

        /// <summary>
        /// S/N: GESTIONE DEL SITO DI INOCULAZIONE DI DEFAULT PER ASSOCIAZIONE E NOME COMMERCIALE
        /// </summary>
        public bool SITO_INOCULAZIONE_SET_DEFAULT { get; private set; }

        /// <summary>
        /// Rende obbligatoria l'immissione del motivo di sopensione della visita e la data di fine sosp
        /// </summary>
        public bool SOSPOBBLIGATORIA { get; private set; }

        /// <summary>
        /// Abilitazione funzionalità di spostamento degli assistiti nei MOVIMENTI CNS
        /// </summary>
        public bool SPOSTAMENTO_ASSISTITI_MOV_CNS { get; private set; }

        /// <summary>
        /// Per gli stati anagrafici elencati nel parametro STATIANAG_CANCCNV, indica se cancellare anche le cnv con appuntamento
        /// </summary>
        public bool STATIANAG_CANCAPP { get; private set; }

        /// <summary>
        /// Elenco degli stati anagrafici per cui cancellare le convocazioni del paziente
        /// </summary>
        public ICollection<string> STATIANAG_CANCCNV { get; private set; }

        /// <summary>
        /// Elenco degli stati anagrafici ammessi per l'inserimento del paziente.
        /// Se il parametro è null, non viene effettuato nessun controllo.
        /// </summary>
        public List<Enumerators.StatoAnagrafico> STATIANAG_INSERT_PAZIENTE { get; private set; }

        /// <summary>
        /// Elenco stati anagrafici che verranno selezionati automaticamente nella maschera dei pazienti interni nei MovCV.
        /// Se null, utilizza gli stati anagrafici attivi (dall'anagrafica).
        /// </summary>
        public List<Enumerators.StatoAnagrafico> STATIANAG_MOVCV_PAZ_INTERNI { get; private set; }

        /// <summary>
        /// Stringa contenente i codici degli stati anagrafici, separati da ; per i quali in veneto viene ricalcolato il centro vaccinale
        /// </summary>
        public List<Enumerators.StatoAnagrafico> STATIANAG_RICALCOLACNS { get; private set; }

        /// <summary>
        /// true: non calcola cnv se non ci sono vac obbligatorie; false: calcola comunque le cnv
        /// </summary>
        public bool STOPCNV_NONOBBL { get; private set; }

        /// <summary>
        /// Durata di default per seduta bilancio di salute
        /// </summary>
        public int TEMPOBIL { get; private set; }

        /// <summary>
        /// Massimo ritardo per esclusione
        /// </summary>
        public int TEMPOESCLUSIONE { get; private set; }

        /// <summary>
        /// Massimo ritardo per inadempienza
        /// </summary>
        public int TEMPOINADEMPIENZA { get; private set; }

        /// <summary>
        /// Massimo ritardo per invio 1° e 2° sollecito
        /// </summary>
        public int TEMPORIT { get; private set; }

        /// <summary>
        /// Durata di default per seduta vaccinazione
        /// </summary>
        public int TEMPOSED { get; private set; }

        /// <summary>
        /// Tessera in centrale non modificabile
        /// </summary>
        public bool TESSCEN { get; private set; }

        /// <summary>
        /// 0 locale 1 centrale lettura 2 centrale lettura/scrittura 3 centrale speciale
        /// </summary>
        public Enumerators.TipoAnags TIPOANAG { get; private set; }

        /// <summary>
        /// 0 locale 2 centrale lettura/scrittura
        /// </summary>
        public Enumerators.TipoAnags TIPOANAG_CATEGORIA_RISCHIO { get; private set; }

        /// <summary>
        /// 0 locale 2 centrale lettura/scrittura
        /// </summary>
        public Enumerators.TipoAnags TIPOANAG_MALATTIE { get; private set; }

        /// <summary>
        /// A/O/F: A=AUTOMATICA / O=ODIERNA / F=FUTURA
        /// </summary>
        public string TIPOCNV { get; private set; }

        /// <summary>
        /// Tipo stampa tp in inadempienze 0 nessuno, 1 filtro su t_paz_pazienti, 2 filtro su v_avvisi
        /// </summary>
        public string TIPOFILTROSTAMPATP { get; private set; }

        /// <summary>
        /// Tipo pagamento selezionato di default in Registrazione Vaccinazioni
        /// </summary>
        public string TIPOPAGAMENTO_DEFAULT { get; private set; }

        /// <summary>
        /// Per manipolare il calcolo convocazioni: calcola a tutti una cnv indipendentemente se esiste gia' o no una cnv
        /// </summary>
        public bool TUTTECNV { get; private set; }

        /// <summary>
        /// S/N: indica se è in corso l'unificazione delle ulss
        /// </summary>
        public bool UNIFICAZIONE_IN_CORSO { get; private set; }

        /// <summary>
        /// In caso di aggiornamento cns del paziente, aggiorna il consultorio anche nelle convocazioni con appuntamento, 
        /// cancella data appuntamento, data invio, tipo appuntamento e imposta data primo appuntamento = data appuntamento
        /// </summary>

        public bool UPDCNV_DELAPP { get; private set; }

        /// <summary>
        /// In caso di aggiornamento cns del paz, aggiorna il cns anche nelle convocazioni con app
        /// </summary>
        public bool UPDCNV_UPDAPP { get; private set; }

        /// <summary>
        /// In caso di aggiornamento cns del paz, aggiorna il cns anche nelle convocazioni (senza app)
        /// </summary>
        public bool UPDCNV_UPDCNS { get; private set; }

        /// <summary>
        /// Indica se l'utente puo' accedere solo su uno dei consultori a lui abilitati oppure a tutti i consultori.
        /// </summary>
        public bool USER_SOLO_CNS_ABILITATI { get; private set; }

        /// <summary>
        /// Utilizza sqlexpression nella stampa degli avvisi
        /// </summary>
        public bool USESQLEXPRESSION { get; private set; }

        /// <summary>
        /// Possibilità di attivare un lotto dalle programmate
        /// </summary>
        public bool VACPROG_ATTIVAZIONE_LOTTO { get; private set; }

        /// <summary>
        /// Valori di BIL_CONSEGNATO_A associato alla malattia per cui e' gestita la compilazione dei bilanci dalla maschera delle programmate
        /// </summary>
        public List<string> VACPROG_BIL_CONSEGNATO_A { get; private set; }

        /// <summary>
        /// Imposta la maschera in sola lettura se data decesso paziente valorizzata
        /// </summary>
        public bool VACPROG_BLOCCO_DECEDUTI { get; private set; }

        /// <summary>
        /// Visualizzazione pulsante di stampa del certificato vaccinale dalla maschera delle programmate
        /// </summary>
        public bool VACPROG_CERTIFICATOVACCINALE { get; private set; }

        /// <summary>
        /// Elimina dalle vac_prog tutte le vaccinazioni eseguite o escluse
        /// </summary>
        public bool VACPROG_ELIMINARIGHE { get; private set; }

        /// <summary>
        /// Indica se nella  maschera delle vaccinazioni programmate le vaccinazioni effettuate sono editabili
        /// </summary>
        public bool VACPROG_MODVACEFFETTUATE { get; private set; }

        /// <summary>
        /// Visualizzazione della colonna del nome commerciale nelle vaccinazioni programmate
        /// </summary>

        public bool VACPROG_NOMECOMMERCIALE { get; private set; }

        /// <summary>
        /// Richiesta di impostazione medico vaccinatore una volta per sessione (True) o ad ogni paziente (False)
        /// </summary>
        public bool VACPROG_SETVACCINATORE { get; private set; }

        /// <summary>
        ///  Tipologie di malattia per cui e' gestita la compilazione dei bilanci dalla maschera delle programmate
        /// </summary>
        public List<string> VACPROG_TIPOLOGIA_MALATTIA { get; private set; }

        /// <summary>
        /// Elenco dei valori di ves_stato che indicano vaccinazioni non eseguite direttamente dalla usl ma registrate, importate o altro
        /// </summary>
        public List<string> VAC_STATI_NON_ESEGUITE { get; private set; }

        /// <summary>
        /// Giorni di validità per i pazienti solo bilancio
        /// </summary>
        public int VALIDITA_SB { get; private set; }

        /// <summary>
        /// Giorni di validità per i pazienti solo bilancio
        /// </summary>
        public List<string> VALORI_VISIBILITA_VACC_CENTRALE { get; private set; }

        /// <summary>
        /// S/N: GESTIONE DELLA VIA DI SOMMINISTRAZIONE DI DEFAULT PER ASSOCIAZIONE E NOME COMMERCIALE
        /// </summary>
        public bool VIA_SOMMINISTRAZIONE_SET_DEFAULT { get; private set; }

        /// <summary>
        /// Permette la compilazione di piu' visite/bilanci nella stessa data
        /// </summary>
        public bool VISITE_STESSA_DATA { get; private set; }

        /// <summary>
        /// Visualizzazione note all'apertura dell'anagrafica del paziente
        /// </summary>
        public bool VISNOTE { get; private set; }

        #endregion

        #region Costruttori

        /// <summary>
        /// Costruttore con valorizzazione dei parametri, letti da db.
        /// </summary>
        /// <param name="genericProvider"></param>
        /// <remarks>Legge il parametro SoloParametriAggiornati dal file di configurazione.</remarks>
        public Settings(DbGenericProvider genericProvider)
            : this(null, genericProvider)
        {
        }

        /// <summary>
        /// Costruttore con valorizzazione dei parametri, letti da db.
        /// </summary>
        /// <param name="cnsCodice"></param>
        /// <param name="genericProvider"></param>
        /// <remarks>Legge il parametro SoloParametriAggiornati dal file di configurazione.</remarks>
        public Settings(string cnsCodice, DbGenericProvider genericProvider)
        {
            // Lettura parametro "SoloParametriAggiornati" dal file di configurazione presente nella directory di esecuzione.
            try
            {
                AppSettingsReader appReader = new AppSettingsReader();

                string valoreSoloAggiornati = (string)appReader.GetValue("SoloParametriAggiornati", typeof(string));

                if (string.IsNullOrEmpty(valoreSoloAggiornati))
                {
                    SoloParametriAggiornati = false;
                }
                else
                {
                    SoloParametriAggiornati = Convert.ToBoolean(valoreSoloAggiornati);
                }
            }
            catch
            {
                SoloParametriAggiornati = true;
            }

            ReadParamsFromDb(cnsCodice, genericProvider);
        }

        #endregion

        #region Factories

        public static Settings GetSettingsAllinea(DbGenericProvider dbGenericProvider)
        {
            return new Settings(CODICE_CONSULTORIO_ALLINEA, dbGenericProvider);
        }

        public static Settings GetSettingsAllineaInterno(DbGenericProvider dbGenericProvider)
        {
            return new Settings(CODICE_CONSULTORIO_ALLINEA_INTERNO, dbGenericProvider);
        }

        #endregion

        #region Metodi

        public TProperty GetSettingValue<TProperty>(SettingName settingName)
        {
            return (TProperty)this.GetType().InvokeMember(settingName.ToString(), System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, this, new object[] { });
        }

        private void ReadParamsFromDb(string cnsCodice, DbGenericProvider genericProvider)
        {
            List<string> enumSettings = new List<string>(Enum.GetNames(typeof(SettingName)));

            enumSettings.Remove(SettingName._UNSPECIFIED.ToString());

            List<KeyValuePair<string, object>> listParametri;

            if (string.IsNullOrEmpty(cnsCodice))
            {
                listParametri = genericProvider.Parametri.GetParametriSistema();
            }
            else
            {
                listParametri = genericProvider.Parametri.GetParametriCns(cnsCodice);
            }

            if (listParametri != null && listParametri.Count > 0)
            {
                foreach (KeyValuePair<string, object> item in listParametri)
                {
                    // Valorizzazione della proprietà corrispondente.
                    this.SetParameters(item.Key, item.Value);

                    // Rimuovo il parametro dalla lista di controllo
                    enumSettings.Remove(item.Key);
                }

                // Controllo i codici paramtro rimasti nella lista, cioè non letti da db
                if (SoloParametriAggiornati && enumSettings.Count > 0)
                {
                    string listaCodici = enumSettings.Aggregate((p, g) => p + ", " + g);
                    throw new UnhandledSettingException(string.Format("Parametri non presenti su db: {0}", listaCodici));
                }
            }
        }

        private void SetParameters(string codice, object valore)
        {
            SettingName settingName = GetSettingName(codice);

            switch (settingName)
            {
                case SettingName.AGGGIORNI:
                    AGGGIORNI = GetDecimalParam(valore);
                    break;
                case SettingName.AGGIORNACIRBYVIA:
                    AGGIORNACIRBYVIA = GetBooleanParam(valore);
                    break;
                case SettingName.AGGIORNACNSBYCIR:
                    AGGIORNACNSBYCIR = GetBooleanParam(valore);
                    break;
                case SettingName.AGGIORNACNSBYCOM:
                    AGGIORNACNSBYCOM = GetBooleanParam(valore);
                    break;
                case SettingName.ALERT_AGGIORNAMENTO_DATI_CENTRALIZZATI:
                    ALERT_AGGIORNAMENTO_DATI_CENTRALIZZATI = GetBooleanParam(valore);
                    break;
                case SettingName.ALIAS_CONTROLLO_CODICI_REGIONALI:
                    ALIAS_CONTROLLO_CODICI_REGIONALI = GetBooleanParam(valore);
                    break;
                case SettingName.ALIAS_UPDATE_MASTER_NULL:
                    ALIAS_UPDATE_MASTER_NULL = GetBooleanParam(valore);
                    break;
                case SettingName.ALIAS_UPDATE_MASTER_NULL_CAMPI_ESCLUSI:
                    ALIAS_UPDATE_MASTER_NULL_CAMPI_ESCLUSI = GetListStringParam(valore, ";");
                    break;
                case SettingName.ALIAS_USA_CNS_MASTER_ANAGRAFICO:
                    ALIAS_USA_CNS_MASTER_ANAGRAFICO = GetBooleanParam(valore);
                    break;
                case SettingName.ALLINEA_ASSOCIAZIONI_ONICS_APP_ID:
                    ALLINEA_ASSOCIAZIONI_ONICS_APP_ID = GetStringParam(valore);
                    break;
                case SettingName.ALLINEA_ASSOCIAZIONI_RFC_ACTOR:
                    ALLINEA_ASSOCIAZIONI_RFC_ACTOR = GetStringParam(valore);
                    break;
                case SettingName.ALLINEA_ASSOCIAZIONI_RFC_ENTE:
                    ALLINEA_ASSOCIAZIONI_RFC_ENTE = GetStringParam(valore);
                    break;
                case SettingName.ALLINEA_PAZIENTE_MODEL_TYPE:
                    ALLINEA_PAZIENTE_MODEL_TYPE = GetStringParam(valore);
                    break;
                case SettingName.ALLINEA_PAZIENTE_SEND_OPERATIONS:
                    List<OnAssistnet.MID.OperazionePaziente> sendOperationsList = new List<MID.OperazionePaziente>();
                    foreach (string operazione in GetListStringParam(valore, "|"))
                    {
                        sendOperationsList.Add((OnAssistnet.MID.OperazionePaziente)Enum.Parse(typeof(OnAssistnet.MID.OperazionePaziente), operazione));
                    }
                    ALLINEA_PAZIENTE_SEND_OPERATIONS = new ReadOnlyCollection<MID.OperazionePaziente>(sendOperationsList);
                    break;
                case SettingName.ANAGPAZ_CAMPI_DISABILITATI_CEN:
                    ANAGPAZ_CAMPI_DISABILITATI_CEN = GetListStringParam(valore, ";");
                    break;
                case SettingName.ANAGPAZ_CAMPI_DISABILITATI_LIVELLO_CERTIFICAZIONE:
                    ANAGPAZ_CAMPI_DISABILITATI_LIVELLO_CERTIFICAZIONE = GetListStringParam(valore, ";");
                    break;
                case SettingName.ANAGPAZ_CAMPI_DISABILITATI_LOC:
                    ANAGPAZ_CAMPI_DISABILITATI_LOC = GetListStringParam(valore, ";");
                    break;
                case SettingName.ANAGPAZ_CAMPI_NASCOSTI:
                    ANAGPAZ_CAMPI_NASCOSTI = GetListStringParam(valore, ";");
                    break;
                case SettingName.APP_ID_CENTRALE:
                    APP_ID_CENTRALE = GetStringParam(valore);
                    break;
                case SettingName.APPETAPM:
                    APPETAPM = GetIntParam(valore);
                    break;
                case SettingName.APPLIBERO:
                    APPLIBERO = GetBooleanParam(valore);
                    break;
                case SettingName.APPTITLE:
                    APPTITLE = GetStringParam(valore);
                    break;
                case SettingName.ASSOCIA_LOTTI_ETA:
                    ASSOCIA_LOTTI_ETA = GetBooleanParam(valore);
                    break;
                case SettingName.ASSOCIAZIONE_AUTO_CV:
                    ASSOCIAZIONE_AUTO_CV = GetBooleanParam(valore);
                    break;
                case SettingName.ASSOCIAZIONI_TIPO_CNS:
                    ASSOCIAZIONI_TIPO_CNS = GetBooleanParam(valore);
                    break;
                case SettingName.AUTO_CF:
                    AUTO_CF = GetBooleanParam(valore);
                    break;
                case SettingName.AUTO_CALC_CICLI:
                    AUTO_CALC_CICLI = GetBooleanParam(valore);
                    break;
                case SettingName.AUTOAGGIORNACIRBYVIA:
                    AUTOAGGIORNACIRBYVIA = GetBooleanParam(valore);
                    break;
                case SettingName.AUTOAGGIORNACNSDECEDUTI:
                    // Parametro criptato (nel formato "codice:valore")
                    AUTOAGGIORNACNSDECEDUTI = GetFunctionalityStatus(GetIntCryptedParamWithCodePrefix(codice, valore));
                    break;
                case SettingName.AUTOAGGIORNACNSADU:
                    // Parametro criptato (nel formato "codice:valore")
                    AUTOAGGIORNACNSADU = GetFunctionalityStatus(GetIntCryptedParamWithCodePrefix(codice, valore));
                    break;
                case SettingName.AUTOAGGIORNACNSADU_CRITERIOSELEZIONE:
                    AUTOAGGIORNACNSADU_CRITERIOSELEZIONE = GetStringParam(valore);
                    break;
                case SettingName.AUTOAGGIORNACNSBYCIR:
                    AUTOAGGIORNACNSBYCIR = GetBooleanParam(valore);
                    break;
                case SettingName.AUTOAGGIORNACNSBYCOM:
                    AUTOAGGIORNACNSBYCOM = GetBooleanParam(valore);
                    break;
                case SettingName.AUTOALLINEA:
                    AUTOALLINEA = GetBooleanParam(valore);
                    break;
                case SettingName.AUTOCNVAPP:
                    AUTOCNVAPP = GetBooleanParam(valore);
                    break;
                case SettingName.AUTOCONV:
                    AUTOCONV = GetBooleanParam(valore);
                    break;
                case SettingName.AUTOSETCNS_INSLOCALE:
                    AUTOSETCNS_INSLOCALE = GetBooleanParam(valore);
                    break;
                case SettingName.AUTO_STATO_ANAG_CHECK_LOCALE:
                    AUTO_STATO_ANAG_CHECK_LOCALE = GetBooleanParam(valore);
                    break;
                case SettingName.AUTO_STATO_ANAG_DA_IMMIGRATO_A_RESIDENTE:
                    AUTO_STATO_ANAG_DA_IMMIGRATO_A_RESIDENTE = GetListEnumeratorValues<Enumerators.StatoAnagrafico>(valore, ";");
                    break;
                case SettingName.AUTO_STATO_ANAG_SOSTITUZIONE_IMMIGRATO:
                    AUTO_STATO_ANAG_SOSTITUZIONE_IMMIGRATO = GetNullableEnumeratorValue<Enumerators.StatoAnagrafico>(valore);
                    break;
                case SettingName.AVVISI_STAMPA_AMBULATORIO:
                    AVVISI_STAMPA_AMBULATORIO = GetBooleanParam(valore);
                    break;
                case SettingName.BILANCI_PREVALORIZZA_OPERATORI:
                    BILANCI_PREVALORIZZA_OPERATORI = GetBooleanParam(valore);
                    break;
                case SettingName.BIZ_PAZIENTE_TYPE:
                    BIZ_PAZIENTE_TYPE = GetStringParam(valore);
                    break;
                case SettingName.CALCOLA_COD_AUSILIARIO:
                    CALCOLA_COD_AUSILIARIO = GetBooleanParam(valore);
                    break;
                case SettingName.CALCOLABILOBBPRECEDENTI:
                    CALCOLABILOBBPRECEDENTI = GetBooleanParam(valore);
                    break;
                case SettingName.CALCOLOCNV_STOP_PRIMA_OBBLIGATORIA:
                    CALCOLOCNV_STOP_PRIMA_OBBLIGATORIA = GetBooleanParam(valore);
                    break;
                case SettingName.CAMPFON:
                    CAMPFON = GetStringParam(valore);
                    break;
                case SettingName.CAMPVACCINALE:
                    CAMPVACCINALE = GetBooleanParam(valore);
                    break;
                case SettingName.CATEGORIE_CITTADINO_CESSATO:
                    CATEGORIE_CITTADINO_CESSATO = GetListStringParam(valore, ";");
                    break;
                case SettingName.CENTRALE_CAMPIFOND:
                    CENTRALE_CAMPIFOND = GetStringParam(valore);
                    break;
                case SettingName.CENTRALE_CHECK_INTEGRITY:
                    CENTRALE_CHECK_INTEGRITY = GetBooleanParam(valore);
                    break;
                case SettingName.CENTRALE_CHECK_UNICF:
                    CENTRALE_CHECK_UNICF = GetBooleanParam(valore);
                    break;
                case SettingName.CENTRALE_CHECK_UNITESSERA:
                    CENTRALE_CHECK_UNITESSERA = GetBooleanParam(valore);
                    break;
                case SettingName.CENTRALE_LOG_FILE:
                    CENTRALE_LOG_FILE = GetCentraleLogFileParam(valore);
                    break;
                case SettingName.CENTRALE_SEPAANAG:
                    CENTRALE_SEPAANAG = GetStringParam(valore);
                    break;
                case SettingName.CENTRALE_SEPATIPO:
                    CENTRALE_SEPATIPO = GetCentraleTipoAnagrafeParam(valore);
                    break;
                case SettingName.CENTRALE_STORVAR:
                    CENTRALE_STORVAR = GetBooleanParam(valore);
                    break;
                case SettingName.CENTRALE_WS_XMPI:
                    CENTRALE_WS_XMPI = GetBooleanParam(valore);
                    break;
                case SettingName.CERTIFICATO_VACCINALE_NOTA_VALIDITA:
                    CERTIFICATO_VACCINALE_NOTA_VALIDITA = GetStringParam(valore);
                    break;
                case SettingName.CHECKVALCOMUNI:
                    CHECKVALCOMUNI = GetBooleanParam(valore);
                    break;
                case SettingName.CHECK_ACQUISIZIONE_REGOLARIZZAZIONE:
                    CHECK_ACQUISIZIONE_REGOLARIZZAZIONE = GetBooleanParam(valore);
                    break;
                case SettingName.CHECK_CICLI:
                    CHECK_CICLI = GetBooleanParam(valore);
                    break;
                case SettingName.CHECK_CICLI_ERRORE:
                    CHECK_CICLI_ERRORE = GetBooleanParam(valore);
                    break;
                case SettingName.CHECK_DATI_ALIAS_PER_MERGE:
                    CHECK_DATI_ALIAS_PER_MERGE = GetBooleanParam(valore);
                    break;
                case SettingName.CHECK_SITO_INOCULO:
                    CHECK_SITO_INOCULO = GetBooleanParam(valore);
                    break;
                case SettingName.CHECK_VIA_SOMMINISTRAZIONE:
                    CHECK_VIA_SOMMINISTRAZIONE = GetBooleanParam(valore);
                    break;
                case SettingName.CHK_ETA_CONSULT:
                    CHK_ETA_CONSULT = GetBooleanParam(valore);
                    break;
                case SettingName.CIRCOSCRIZIONE_OBBL:
                    CIRCOSCRIZIONE_OBBL = GetBooleanParam(valore);
                    break;
                case SettingName.CITTADINANZA_DEFAULT:
                    CITTADINANZA_DEFAULT = GetStringParam(valore);
                    break;
                case SettingName.CNS_DEFAULT:
                    CNS_DEFAULT = GetStringParam(valore);
                    break;
                case SettingName.CNSCNV:
                    CNSCNV = GetStringParam(valore);
                    break;
                case SettingName.CNVAUTOFILTRAETA:
                    CNVAUTOFILTRAETA = GetBooleanParam(valore);
                    break;
                case SettingName.CODESCL:
                    CODESCL = GetStringParam(valore);
                    break;
                case SettingName.CODESCLNOCICLO:
                    CODESCLNOCICLO = GetStringParam(valore);
                    break;
                case SettingName.CODESCLNONOBBL:
                    CODESCLNONOBBL = GetStringParam(valore);
                    break;
                case SettingName.CODESCLNONOBBLSETI:
                    CODESCLNONOBBLSETI = GetStringParam(valore);
                    break;
                case SettingName.CODICE_ASL:
                    CODICE_ASL = GetStringParam(valore);
                    break;
                case SettingName.CODICE_REGIONE:
                    CODICE_REGIONE = GetStringParam(valore);
                    break;
                case SettingName.CODICE_UTENTE_PRENOTAZIONE_WEB:
                    CODICE_UTENTE_PRENOTAZIONE_WEB = GetIntParam(valore);
                    break;
                case SettingName.CODICI_MOTIVI_CESSAZIONE_ASSISTENZA:
                    CODICI_MOTIVI_CESSAZIONE_ASSISTENZA = GetListStringParam(valore, ";");
                    break;
                case SettingName.CODICI_VACCINAZIONI_COVID:
                    CODICI_VACCINAZIONI_COVID = GetListStringParam(valore, ",");
                    break;
                case SettingName.CODNOMAL:
                    CODNOMAL = GetStringParam(valore);
                    break;
                case SettingName.COM_RES_BLOCCATI:
                    COM_RES_BLOCCATI = GetStringParam(valore);
                    break;
                case SettingName.COMDEFAULT:
                    COMDEFAULT = GetStringParam(valore);
                    break;
                case SettingName.COMUNE_SCONOSCIUTO:
                    COMUNE_SCONOSCIUTO = GetStringParam(valore);
                    break;
                case SettingName.CONDIZIONE_RISCHIO_DEFAULT:
                    CONDIZIONE_RISCHIO_DEFAULT = GetStringParam(valore);
                    break;
                case SettingName.CONDIZIONE_SANITARIA_DEFAULT:
                    CONDIZIONE_SANITARIA_DEFAULT = GetStringParam(valore);
                    break;
                case SettingName.CONDIZIONE_RISCHIO_OBBLIGATORIA:
                    CONDIZIONE_RISCHIO_OBBLIGATORIA = GetBooleanParam(valore);
                    break;
                case SettingName.CONDIZIONE_SANITARIA_OBBLIGATORIA:
                    CONDIZIONE_SANITARIA_OBBLIGATORIA = GetBooleanParam(valore);
                    break;
                case SettingName.CONDIZIONE_SANITARIA_TIPOLOGIE_MALATTIA:
                    CONDIZIONE_SANITARIA_TIPOLOGIE_MALATTIA = GetListStringParam(valore, ";");
                    break;
                case SettingName.CONFLITTI_AUTORISOLUZIONE:
                    CONFLITTI_AUTORISOLUZIONE = GetBooleanParam(valore);
                    break;
                case SettingName.CONSENSO_APP_ID:
                    CONSENSO_APP_ID = GetStringParam(valore);
                    break;
                case SettingName.CONSENSO_BLOCCANTE_AUTO_EDIT:
                    CONSENSO_BLOCCANTE_AUTO_EDIT = GetBooleanParam(valore);
                    break;
                case SettingName.CONSENSO_GES:
                    // Parametro criptato (nel formato "codice:valore")
                    CONSENSO_GES = GetBooleanCryptedParamWithCodePrefix(codice, valore);
                    break;
                case SettingName.CONSENSO_ID_AUTORILEVAZIONE:
                    CONSENSO_ID_AUTORILEVAZIONE = GetListConsensoAutoRilevazione(valore);
                    break;
                case SettingName.CONSENSO_ID_COMUNICAZIONE:
                    CONSENSO_ID_COMUNICAZIONE = GetNullableIntParam(valore);
                    break;
                case SettingName.CONSENSO_ID_NON_VISIBILI:
                    CONSENSO_ID_NON_VISIBILI = GetListIntParam(valore, "|");
                    break;
                case SettingName.CONSENSO_KEY:
                    // Parametro criptato (nel formato "codice:valore")
                    CONSENSO_KEY = GetStringCryptedParamWithCodePrefix(codice, valore);
                    break;
                case SettingName.CONSENSO_MSG_NO_COD_CENTRALE:
                    CONSENSO_MSG_NO_COD_CENTRALE = GetStringParam(valore);
                    break;
                case SettingName.CONSENSO_LOCALE:
                    CONSENSO_LOCALE = GetBooleanParam(valore);
                    break;
                case SettingName.CONSENSO_GLOBALE_VISIBILITA_CONCESSA:
                    CONSENSO_GLOBALE_VISIBILITA_CONCESSA = GetListStringParam(valore, "|");
                    break;
                case SettingName.CONSENSO_SEMAFORI_VISIBILI:
                    CONSENSO_SEMAFORI_VISIBILI = GetBooleanParam(valore);
                    break;
                case SettingName.CONSENSO_TIPOANAG:
                    CONSENSO_TIPOANAG = GetTipoAnagsParam(valore);
                    break;
                case SettingName.CONSENSO_URL:
                    CONSENSO_URL = GetStringParam(valore);
                    break;
                case SettingName.CONSENSO_VALORI_VISIBILITA_CONCESSA:
                    CONSENSO_VALORI_VISIBILITA_CONCESSA = GetListStringParam(valore, "|");
                    break;
                case SettingName.CONSULTORIO_OBBL:
                    CONSULTORIO_OBBL = GetBooleanParam(valore);
                    break;
                case SettingName.CONVOCAZIONI_ALTRI_CONSULTORI:
                    CONVOCAZIONI_ALTRI_CONSULTORI = GetConvocazioniAltroConsultorioParam(valore);
                    break;
                case SettingName.COV_CODICE_STATO_EPISODIO_GUARITO:
                    COV_CODICE_STATO_EPISODIO_GUARITO = GetStringParam(valore);
                    break;
                case SettingName.COV_ESITO_POSITIVO_TAMPONE:
                    COV_ESITO_POSITIVO_TAMPONE = GetStringParam(valore);
                    break;
                case SettingName.COV_ESITO_NEGATIVO_TAMPONE:
                    COV_ESITO_NEGATIVO_TAMPONE = GetStringParam(valore);
                    break;
                case SettingName.COV_LOG_INVIO_COMUNICAZIONE_OTP:
                    COV_LOG_INVIO_COMUNICAZIONE_OTP = GetBooleanParam(valore);
                    break;
                case SettingName.COV_ORARIO_LIMITE_DIARIA:
                    COV_ORARIO_LIMITE_DIARIA = GetStringParam(valore);
                    break;
                case SettingName.CTRL_ASSOCIABILITA_VAC:
                    // Parametro criptato (nel formato "codice:valore")
                    CTRL_ASSOCIABILITA_VAC = GetBooleanCryptedParamWithCodePrefix(codice, valore);
                    break;
                case SettingName.DATA_INIZIO_SOMMINISTRAZIONE_COVID:
                    DATA_INIZIO_SOMMINISTRAZIONE_COVID = GetNullableDateTimeParam(valore);
                    break;
                case SettingName.DEFTAPPL:
                    DEFTAPPL = GetStringParam(valore);
                    break;
                case SettingName.DELCNVSTATO:
                    DELCNVSTATO = GetBooleanParam(valore);
                    break;
                case SettingName.DESCAT1:
                    DESCAT1 = GetStringParam(valore);
                    break;
                case SettingName.DESCAT2:
                    DESCAT2 = GetStringParam(valore);
                    break;
                case SettingName.DESLIB1:
                    DESLIB1 = GetStringParam(valore);
                    break;
                case SettingName.DESLIB2:
                    DESLIB2 = GetStringParam(valore);
                    break;
                case SettingName.DESLIB3:
                    DESLIB3 = GetStringParam(valore);
                    break;
                case SettingName.ESCLUDINONOBBLSETI:
                    ESCLUDINONOBBLSETI = GetBooleanParam(valore);
                    break;
                case SettingName.ESCLUDISENOCICLO:
                    ESCLUDISENOCICLO = GetBooleanParam(valore);
                    break;
                case SettingName.EXPORT_POSTEL_ARGOMENTO:
                    EXPORT_POSTEL_ARGOMENTO = GetStringParam(valore);
                    break;
                case SettingName.EXPORT_POSTEL_TIPO_AVVISO_VISIBILE:
                    EXPORT_POSTEL_TIPO_AVVISO_VISIBILE = GetListStringParam(valore, "|");
                    break;
                case SettingName.FIRMADIGITALE_ANAMNESI_ON:
                    FIRMADIGITALE_ANAMNESI_ON = GetBooleanParam(valore);
                    break;
                case SettingName.FLAG_CANCELLATO_CHECK:
                    FLAG_CANCELLATO_CHECK = GetBooleanParam(valore);
                    break;
                case SettingName.FLAG_REGOLARIZZATO_DEFAULT:
                    FLAG_REGOLARIZZATO_DEFAULT = GetNullableBooleanParam(valore);
                    break;
                case SettingName.FSE_GESTIONE:
                    FSE_GESTIONE = GetBooleanParam(valore);
                    break;
                case SettingName.FSE_OID_DOCUMENTI:
                    FSE_OID_DOCUMENTI = GetStringParam(valore);
                    break;
                case SettingName.FSE_OID_REGIONE_VENETO:
                    FSE_OID_REGIONE_VENETO = GetStringParam(valore);
                    break;
                case SettingName.FSE_TENTATIVI_STOP:
                    FSE_TENTATIVI_STOP = GetIntParam(valore);
                    break;
                case SettingName.FSE_TIPO_DOC_CERTIFICATO_VACCINALE:
                    FSE_TIPO_DOC_CERTIFICATO_VACCINALE = GetStringParam(valore);
                    break;
                case SettingName.GES_APP_RICORDA_FILTRI:
                    GES_APP_RICORDA_FILTRI = GetBooleanParam(valore);
                    break;
                case SettingName.GES_APP_OPZIONI_VISUALIZZAZIONE:
                    GES_APP_OPZIONI_VISUALIZZAZIONE = GetListStringParam(valore, "|");
                    break;
                case SettingName.GES_AUTO_STATO_ANAGRAFICO:
                    GES_AUTO_STATO_ANAGRAFICO = GetBooleanParam(valore);
                    break;
                case SettingName.GES_DATA_CANC_OBBLIG:
                    GES_DATA_CANC_OBBLIG = GetBooleanParam(valore);
                    break;
                case SettingName.GES_DATA_IRREP_OBBLIG:
                    GES_DATA_IRREP_OBBLIG = GetBooleanParam(valore);
                    break;
                case SettingName.GES_NOTE_AVVISI:
                    GES_NOTE_AVVISI = GetBooleanParam(valore);
                    break;
                case SettingName.GESBALOT:
                    GESBALOT = GetBooleanParam(valore);
                    break;
                case SettingName.GESBIL:
                    GESBIL = GetBooleanParam(valore);
                    break;
                case SettingName.GESCICLISEDUTE:
                    GESCICLISEDUTE = GetBooleanParam(valore);
                    break;
                case SettingName.GES_CALCOLO_COPERTURA:
                    GES_CALCOLO_COPERTURA = GetBooleanParam(valore);
                    break;
                case SettingName.GES_CALCOLO_SCADENZA_ESCLUSIONE:
                    GES_CALCOLO_SCADENZA_ESCLUSIONE = GetBooleanParam(valore);
                    break;
                case SettingName.GESDATAVALIDITA:
                    GESDATAVALIDITA = GetBooleanParam(valore);
                    break;
                case SettingName.GESDOSISCATOLA:
                    GESDOSISCATOLA = GetBooleanParam(valore);
                    break;
                case SettingName.GESINSLOTTO:
                    GESINSLOTTO = GetBooleanParam(valore);
                    break;
                case SettingName.GESMAG:
                    GESMAG = GetBooleanParam(valore);
                    break;
                case SettingName.GESMODALITAACCESSO:
                    GESMODALITAACCESSO = GetBooleanParam(valore);
                    break;
                case SettingName.GESSESSOCICLI:
                    GESSESSOCICLI = GetBooleanParam(valore);
                    break;
                case SettingName.GESSOLLECITI:
                    GESSOLLECITI = GetBooleanParam(valore);
                    break;
                case SettingName.GESSOLLECITIBILANCI:
                    GESSOLLECITIBILANCI = GetBooleanParam(valore);
                    break;
                case SettingName.GESTPAZ_CAMPO_ORDINAMENTO_MALATTIA:
                    GESTPAZ_CAMPO_ORDINAMENTO_MALATTIA = GetStringParam(valore);
                    break;
                case SettingName.GESTPAZ_CAT_RISCHIO_OBSOLETE:
                    GESTPAZ_CAT_RISCHIO_OBSOLETE = GetBooleanParam(valore);
                    break;
                case SettingName.GESTPAZ_DATA_OBBLIGATORIA_SE_DECEDUTO:
                    GESTPAZ_DATA_OBBLIGATORIA_SE_DECEDUTO = GetBooleanParam(valore);
                    break;
                case SettingName.GESTPAZ_MALATTIE_NON_MODIFICABILI:
                    GESTPAZ_MALATTIE_NON_MODIFICABILI = GetListStringParam(valore, "|");
                    break;
                case SettingName.GESTPAZ_TIPOLOGIA_MALATTIA:
                    GESTPAZ_TIPOLOGIA_MALATTIA = GetListStringParam(valore, "|");
                    break;
                case SettingName.GESVACCAMPAGNA:
                    GESVACCAMPAGNA = GetBooleanParam(valore);
                    break;
                case SettingName.GESVACFITTIZIA:
                    GESVACFITTIZIA = GetBooleanParam(valore);
                    break;
                case SettingName.GESVIE:
                    GESVIE = GetBooleanParam(valore);
                    break;
                case SettingName.GIORNI_MODIFICA_BILANCIO_MEDICO:
                    GIORNI_MODIFICA_BILANCIO_MEDICO = GetIntParam(valore);
                    break;
                case SettingName.GIORNI_VARIAZIONE_VACCINAZIONE_ESEGUITA:
                    GIORNI_VARIAZIONE_VACCINAZIONE_ESEGUITA = GetIntParam(valore);
                    break;
                case SettingName.ID_GRUPPO_ADMIN_DATI_VACCINALI:
                    ID_GRUPPO_ADMIN_DATI_VACCINALI = GetStringParam(valore);
                    break;
                case SettingName.ID_GRUPPO_SUPERUSER:
                    ID_GRUPPO_SUPERUSER = GetStringParam(valore);
                    break;
                case SettingName.INFO_ASS_TEMPLATE_DESCRIZIONE:
                    INFO_ASS_TEMPLATE_DESCRIZIONE = GetStringParam(valore);
                    break;
                case SettingName.INFO_VAC_TEMPLATE_DESCRIZIONE:
                    INFO_VAC_TEMPLATE_DESCRIZIONE = GetStringParam(valore);
                    break;
                case SettingName.INSERIMENTO_PAZIENTE_ABILITATO:
                    INSERIMENTO_PAZIENTE_ABILITATO = GetBooleanParam(valore);
                    break;
                case SettingName.INSVIA:
                    INSVIA = GetBooleanParam(valore);
                    break;
                case SettingName.ISTAT_PROVINCIA:
                    ISTAT_PROVINCIA = GetStringParam(valore);
                    break;
                case SettingName.LASTORDVACESEGUITE:
                    LASTORDVACESEGUITE = GetStringParam(valore);
                    break;
                case SettingName.LEN_COGNOME:
                    LEN_COGNOME = GetIntParam(valore);
                    break;
                case SettingName.LEN_COGNOME_NOME:
                    LEN_COGNOME_NOME = GetIntParam(valore);
                    break;
                case SettingName.LEN_INDDOM:
                    LEN_INDDOM = GetIntParam(valore);
                    break;
                case SettingName.LEN_INDRES:
                    LEN_INDRES = GetIntParam(valore);
                    break;
                case SettingName.LEN_LIBERO1:
                    LEN_LIBERO1 = GetIntParam(valore);
                    break;
                case SettingName.LEN_LIBERO2:
                    LEN_LIBERO2 = GetIntParam(valore);
                    break;
                case SettingName.LEN_LIBERO3:
                    LEN_LIBERO3 = GetIntParam(valore);
                    break;
                case SettingName.LEN_NOME:
                    LEN_NOME = GetIntParam(valore);
                    break;
                case SettingName.LEN_NOTE:
                    LEN_NOTE = GetIntParam(valore);
                    break;
                case SettingName.LEN_NOTE_SOLLECITI:
                    LEN_NOTE_SOLLECITI = GetIntParam(valore);
                    break;
                case SettingName.LEN_NOTE_CERTIFICATO:
                    LEN_NOTE_CERTIFICATO = GetIntParam(valore);
                    break;
                case SettingName.LEN_TEL1:
                    LEN_TEL1 = GetIntParam(valore);
                    break;
                case SettingName.LEN_TEL2:
                    LEN_TEL2 = GetIntParam(valore);
                    break;
                case SettingName.LEN_TEL3:
                    LEN_TEL3 = GetIntParam(valore);
                    break;
                case SettingName.LEN_TESSERA:
                    LEN_TESSERA = GetIntParam(valore);
                    break;
                case SettingName.LIBRETTO_VAC_MEDICO:
                    LIBRETTO_VAC_MEDICO = GetStringParam(valore);
                    break;
                case SettingName.LOCALE_PROVVISORIO:
                    LOCALE_PROVVISORIO = GetBooleanParam(valore);
                    break;
                case SettingName.LOG_CAMBIOPAZIENTE:
                    LOG_CAMBIOPAZIENTE = GetBooleanParam(valore);
                    break;
                case SettingName.LOG_DATAACCESSLAYER:
                    LOG_DATAACCESSLAYER = GetBooleanParam(valore);
                    break;
                case SettingName.LOG_MAGAZZINO:
                    LOG_MAGAZZINO = GetBooleanParam(valore);
                    break;
                case SettingName.LOG_POSTAZIONI:
                    LOG_POSTAZIONI = GetBooleanParam(valore);
                    break;
                case SettingName.LOG_SESSIONCLEANER:
                    LOG_SESSIONCLEANER = GetBooleanParam(valore);
                    break;
                case SettingName.LUOGHI:
                    LUOGHI = GetStringParam(valore);
                    break;
                case SettingName.MANTOUX_CODICE_ASSOCIAZIONE:
                    MANTOUX_CODICE_ASSOCIAZIONE = GetStringParam(valore);
                    break;
                case SettingName.MANTOUX_CODICE_VACCINAZIONE:
                    MANTOUX_CODICE_VACCINAZIONE = GetStringParam(valore);
                    break;
                case SettingName.MAXPOSTAZIONI:
                    // Parametro criptato (nel formato "codice:valore")
                    MAXPOSTAZIONI = GetIntCryptedParamWithCodePrefix(codice, valore);
                    break;
                case SettingName.MEDINAMB:
                    MEDINAMB = GetBooleanParam(valore);
                    break;
                case SettingName.MEDLOGIN:
                    MEDLOGIN = GetBooleanParam(valore);
                    break;
                case SettingName.MEDVACLOGIN:
                    MEDVACLOGIN = GetBooleanParam(valore);
                    break;
                case SettingName.MENUDIS:
                    MENUDIS = GetListStringParam(valore, "|");
                    break;
                case SettingName.MENUDIS_APPUNTAMENTI:
                    MENUDIS_APPUNTAMENTI = GetStringParam(valore);
                    break;
                case SettingName.MENUDIS_HPAZIENTI:
                    MENUDIS_HPAZIENTI = GetStringParam(valore);
                    break;
                case SettingName.MOVCV_EDIT_STATO_ANAGRAFICO:
                    MOVCV_EDIT_STATO_ANAGRAFICO = GetBooleanParam(valore);
                    break;
                case SettingName.MOVCV_STATI_ANAGRAFICI_DA_ESCLUDERE:
                    MOVCV_STATI_ANAGRAFICI_DA_ESCLUDERE = GetListStringParam(valore, ",");
                    break;
                    case SettingName.CALCOLOAPP_MINUTI_LOCK_DISPONIBILITA:
                        CALCOLOAPP_MINUTI_LOCK_DISPONIBILITA = GetIntParam(valore);
                    break;
                case SettingName.N_RITARDATARI:
                    N_RITARDATARI = GetIntParam(valore);
                    break;
                case SettingName.NUM_GIORNI_FOLLOWUP:
                    NUM_GIORNI_FOLLOWUP = GetIntParam(valore);
                    break;
                case SettingName.NUM_GIORNI_REGOLARIZZAZIONE:
                    NUM_GIORNI_REGOLARIZZAZIONE = GetIntParam(valore);
                    break;
                case SettingName.NUM_MAX_GIORNI_REGOLARIZZAZIONE:
                    NUM_MAX_GIORNI_REGOLARIZZAZIONE = GetNullableIntParam(valore);
                    break;
                case SettingName.NUM_MIN_VAC_PER_FASCIA_ORARIA:
                    NUM_MIN_VAC_PER_FASCIA_ORARIA = GetIntParam(valore);
                    break;
                case SettingName.NUMAMB:
                    NUMAMB = GetIntParam(valore);
                    break;
                case SettingName.NUMSOL:
                    NUMSOL = GetIntParam(valore);
                    break;
                case SettingName.ORAPM:
                    ORAPM = GetStringParam(valore);
                    break;
                case SettingName.ORDCOLVACESEGUITE:
                    ORDCOLVACESEGUITE = GetStringParam(valore);
                    break;
                case SettingName.PAGAMENTO:
                    PAGAMENTO = GetBooleanParam(valore);
                    break;
                case SettingName.PAGAMENTO_AUTO_CHECK_TIPO:
                    PAGAMENTO_AUTO_CHECK_TIPO = GetStringParam(valore);
                    break;
                case SettingName.POLO_ONVAC:
                    POLO_ONVAC = GetStringParam(valore);
                    break;
                case SettingName.POSTAZIONI:
                    // Parametro criptato (nel formato "codice:valore")
                    POSTAZIONI = GetBooleanCryptedParamWithCodePrefix(codice, valore);
                    break;
                case SettingName.PPA_VARIABILI_NO_DUPLICA:
                    PPA_VARIABILI_NO_DUPLICA = GetListIntParam(valore, ";");
                    break;
                case SettingName.REAZIONE_AVVERSA_INTEGRAZIONE:
                    REAZIONE_AVVERSA_INTEGRAZIONE = GetBooleanParam(valore);
                    break;
                case SettingName.REGPROC_INTERVALLO_GIORNI_FILTRO_ESECUZIONE:
                    REGPROC_INTERVALLO_GIORNI_FILTRO_ESECUZIONE = GetNullableIntParam(valore);
                    break;
                case SettingName.REGVAC_ASSCODICEDESCRIZIONE:
                    REGVAC_ASSCODICEDESCRIZIONE = GetStringParam(valore);
                    break;
                case SettingName.REPORT:
                    REPORT = GetStringParam(valore);
                    break;
                case SettingName.REPORT_VAC_GIORN:
                    REPORT_VAC_GIORN = GetBooleanParam(valore);
                    break;
                case SettingName.RICERCA_APP_SET_AMB_CONVOCAZIONE:
                    RICERCA_APP_SET_AMB_CONVOCAZIONE = GetBooleanParam(valore);
                    break;
                case SettingName.RICERCA_PAZ_FILTRO_CNS_SET_DEFAULT:
                    RICERCA_PAZ_FILTRO_CNS_SET_DEFAULT = GetBooleanParam(valore);
                    break;
                case SettingName.RICERCA_PAZ_FOCUS:
                    RICERCA_PAZ_FOCUS = GetStringParam(valore);
                    break;
                case SettingName.RICERCA_PAZ_MAX_RECORDS:
                    RICERCA_PAZ_MAX_RECORDS = GetNullableIntParam(valore);
                    break;
                case SettingName.RICERCA_PAZ_ORDINAMENTO:
                    RICERCA_PAZ_ORDINAMENTO = GetStringParam(valore);
                    break;
                case SettingName.RICERCA_PAZ_QPV2_API_ENDPOINT:
                    RICERCA_PAZ_QPV2_API_ENDPOINT = GetStringParam(valore);
                    break;
                case SettingName.RICERCA_PAZ_QPV2:
                    RICERCA_PAZ_QPV2 = GetBooleanParam(valore);
                    break;
                case SettingName.RICERCA_PAZ_SHOW_CODICE_AUSILIARIO:
                    // Parametro criptato (nel formato "codice:valore")
                    RICERCA_PAZ_SHOW_CODICE_AUSILIARIO = GetBooleanCryptedParamWithCodePrefix(codice, valore);
                    break;
                case SettingName.RICERCA_PAZ_SHOW_CODICE_PAZIENTE:
                    // Parametro criptato (nel formato "codice:valore")
                    switch (GetStringCryptedParamWithCodePrefix(codice, valore))
                    {
                        case Constants.TipoAutorizzazione.ALL_USERS:
                            RICERCA_PAZ_SHOW_CODICE_PAZIENTE = Enumerators.TipoAutorizzazione.TuttiUtenti;
                            break;
                        case Constants.TipoAutorizzazione.NONE:
                            RICERCA_PAZ_SHOW_CODICE_PAZIENTE = Enumerators.TipoAutorizzazione.Nessuno;
                            break;
                        case Constants.TipoAutorizzazione.ONLY_SUPERUSERS:
                            RICERCA_PAZ_SHOW_CODICE_PAZIENTE = Enumerators.TipoAutorizzazione.SoloSuperUtenti;
                            break;
                    }
                    break;
                case SettingName.RICERCA_PAZ_SHOW_CODICE_REGIONALE:
                    // Parametro criptato (nel formato "codice:valore")
                    RICERCA_PAZ_SHOW_CODICE_REGIONALE = GetBooleanCryptedParamWithCodePrefix(codice, valore);
                    break;
                case SettingName.RICERCA_PAZ_SHOW_FILTRO_CNS:
                    RICERCA_PAZ_SHOW_FILTRO_CNS = GetBooleanParam(valore);
                    break;
                case SettingName.RICERCA_PAZ_SHOW_FILTRO_COMUNE_RESIDENZA:
                    RICERCA_PAZ_SHOW_FILTRO_COMUNE_RESIDENZA = GetBooleanParam(valore);
                    break;
                case SettingName.RICERCA_PAZ_SHOW_FLAG_CANCELLATO:
                    RICERCA_PAZ_SHOW_FLAG_CANCELLATO = GetBooleanParam(valore);
                    break;
                case SettingName.RICONDUZIONE_INS_PAZ:
                    RICONDUZIONE_INS_PAZ = GetBooleanParam(valore);
                    break;
                case SettingName.RICONDUZIONE_INS_PAZ_CAMPI_RICERCA:
                    RICONDUZIONE_INS_PAZ_CAMPI_RICERCA = GetListStringParam(valore, ";");
                    break;
                case SettingName.SCARTO_MASSIMO:
                    SCARTO_MASSIMO = GetIntParam(valore);
                    break;
                case SettingName.SED_AUTO:
                    SED_AUTO = GetIntParam(valore);
                    break;
                case SettingName.SED_MANU:
                    SED_MANU = GetIntParam(valore);
                    break;
                case SettingName.SESSIONCLEANER:
                    SESSIONCLEANER = GetBooleanParam(valore);
                    break;
                case SettingName.SET_AMB_CALENDARIO:
                    SET_AMB_CALENDARIO = GetBooleanParam(valore);
                    break;
                case SettingName.SET_AMB_ELENCO_CALENDARIO:
                    SET_AMB_ELENCO_CALENDARIO = GetBooleanParam(valore);
                    break;
                case SettingName.SITO_INOCULAZIONE_SET_DEFAULT:
                    SITO_INOCULAZIONE_SET_DEFAULT = GetBooleanCryptedParamWithCodePrefix(codice, valore);
                    break;
                case SettingName.SOSPOBBLIGATORIA:
                    SOSPOBBLIGATORIA = GetBooleanParam(valore);
                    break;
                case SettingName.SPOSTAMENTO_ASSISTITI_MOV_CNS:
                    SPOSTAMENTO_ASSISTITI_MOV_CNS = GetBooleanParam(valore);
                    break;
                case SettingName.STATIANAG_CANCAPP:
                    STATIANAG_CANCAPP = GetBooleanParam(valore);
                    break;
                case SettingName.STATIANAG_CANCCNV:
                    STATIANAG_CANCCNV = GetListStringParam(valore, ";");
                    break;
                case SettingName.STATIANAG_INSERT_PAZIENTE:
                    STATIANAG_INSERT_PAZIENTE = GetListEnumeratorValues<Enumerators.StatoAnagrafico>(valore, ";");
                    break;
                case SettingName.STATIANAG_MOVCV_PAZ_INTERNI:
                    STATIANAG_MOVCV_PAZ_INTERNI = GetListEnumeratorValues<Enumerators.StatoAnagrafico>(valore, ";");
                    break;
                case SettingName.STATIANAG_RICALCOLACNS:
                    STATIANAG_RICALCOLACNS = GetListEnumeratorValues<Enumerators.StatoAnagrafico>(valore, ";");
                    break;
                case SettingName.STOPCNV_NONOBBL:
                    STOPCNV_NONOBBL = GetBooleanParam(valore);
                    break;
                case SettingName.TEMPOBIL:
                    TEMPOBIL = GetIntParam(valore);
                    break;
                case SettingName.TEMPOESCLUSIONE:
                    TEMPOESCLUSIONE = GetIntParam(valore);
                    break;
                case SettingName.TEMPOINADEMPIENZA:
                    TEMPOINADEMPIENZA = GetIntParam(valore);
                    break;
                case SettingName.TEMPORIT:
                    TEMPORIT = GetIntParam(valore);
                    break;
                case SettingName.TEMPOSED:
                    TEMPOSED = GetIntParam(valore);
                    break;
                case SettingName.TESSCEN:
                    TESSCEN = GetBooleanParam(valore);
                    break;
                case SettingName.TIPOANAG:
                    TIPOANAG = GetTipoAnagsParam(valore);
                    break;
                case SettingName.TIPOANAG_CATEGORIA_RISCHIO:
                    TIPOANAG_CATEGORIA_RISCHIO = GetTipoAnagsParam(valore);
                    if (TIPOANAG_CATEGORIA_RISCHIO != Enumerators.TipoAnags.SoloLocale && TIPOANAG_CATEGORIA_RISCHIO != Enumerators.TipoAnags.CentraleLettScritt)
                    {
                        throw new NotSupportedException("Valore parametro TIPOANAG_CATEGORIA_RISCHIO deve essere 0 o 2");
                    }

                    break;
                case SettingName.TIPOANAG_MALATTIE:
                    TIPOANAG_MALATTIE = GetTipoAnagsParam(valore);
                    if (TIPOANAG_MALATTIE != Enumerators.TipoAnags.SoloLocale && TIPOANAG_MALATTIE != Enumerators.TipoAnags.CentraleLettScritt)
                    {
                        throw new NotSupportedException("Valore parametro TIPOANAG_MALATTIE deve essere 0 o 2");
                    }

                    break;
                case SettingName.TIPOCNV:
                    TIPOCNV = GetStringParam(valore);
                    break;
                case SettingName.TIPOFILTROSTAMPATP:
                    TIPOFILTROSTAMPATP = GetStringParam(valore);
                    break;
                case SettingName.TIPOPAGAMENTO_DEFAULT:
                    TIPOPAGAMENTO_DEFAULT = GetStringParam(valore);
                    break;
                case SettingName.TUTTECNV:
                    TUTTECNV = GetBooleanParam(valore);
                    break;
                case SettingName.UNIFICAZIONE_IN_CORSO:
                    UNIFICAZIONE_IN_CORSO = GetBooleanParam(valore);
                    break;
                case SettingName.UPDCNV_DELAPP:
                    UPDCNV_DELAPP = GetBooleanParam(valore);
                    break;
                case SettingName.UPDCNV_UPDAPP:
                    UPDCNV_UPDAPP = GetBooleanParam(valore);
                    break;
                case SettingName.UPDCNV_UPDCNS:
                    UPDCNV_UPDCNS = GetBooleanParam(valore);
                    break;
                case SettingName.USESQLEXPRESSION:
                    USESQLEXPRESSION = GetBooleanParam(valore);
                    break;
                case SettingName.USER_SOLO_CNS_ABILITATI:
                    USER_SOLO_CNS_ABILITATI = GetBooleanParam(valore);
                    break;
                case SettingName.VACPROG_ATTIVAZIONE_LOTTO:
                    // Parametro criptato (nel formato "codice:valore")
                    VACPROG_ATTIVAZIONE_LOTTO = GetBooleanCryptedParamWithCodePrefix(codice, valore);
                    break;
                case SettingName.VACPROG_BIL_CONSEGNATO_A:
                    VACPROG_BIL_CONSEGNATO_A = GetListStringParam(valore, "|");
                    break;
                case SettingName.VACPROG_BLOCCO_DECEDUTI:
                    VACPROG_BLOCCO_DECEDUTI = GetBooleanParam(valore);
                    break;
                case SettingName.VACPROG_CERTIFICATOVACCINALE:
                    VACPROG_CERTIFICATOVACCINALE = GetBooleanParam(valore);
                    break;
                case SettingName.VACPROG_ELIMINARIGHE:
                    VACPROG_ELIMINARIGHE = GetBooleanParam(valore);
                    break;
                case SettingName.VACPROG_MODVACEFFETTUATE:
                    VACPROG_MODVACEFFETTUATE = GetBooleanParam(valore);
                    break;
                case SettingName.VACPROG_NOMECOMMERCIALE:
                    VACPROG_NOMECOMMERCIALE = GetBooleanParam(valore);
                    break;
                case SettingName.VACPROG_SETVACCINATORE:
                    VACPROG_SETVACCINATORE = GetBooleanParam(valore);
                    break;
                case SettingName.VACPROG_TIPOLOGIA_MALATTIA:
                    VACPROG_TIPOLOGIA_MALATTIA = GetListStringParam(valore, "|");
                    break;
                case SettingName.VAC_STATI_NON_ESEGUITE:
                    VAC_STATI_NON_ESEGUITE = GetListStringParam(valore, ";");
                    break;
                case SettingName.VALIDITA_SB:
                    VALIDITA_SB = GetIntParam(valore);
                    break;
                case SettingName.VALORI_VISIBILITA_VACC_CENTRALE:
                    VALORI_VISIBILITA_VACC_CENTRALE = GetListStringParam(valore, "|");
                    break;
                case SettingName.VIA_SOMMINISTRAZIONE_SET_DEFAULT:
                    VIA_SOMMINISTRAZIONE_SET_DEFAULT = GetBooleanCryptedParamWithCodePrefix(codice, valore);
                    break;
                case SettingName.VISITE_STESSA_DATA:
                    VISITE_STESSA_DATA = GetBooleanParam(valore);
                    break;
                case SettingName.VISNOTE:
                    VISNOTE = GetBooleanParam(valore);
                    break;
            }
        }

        [System.Diagnostics.DebuggerStepThrough()]
        private SettingName GetSettingName(string codice)
        {
            SettingName settingName = SettingName._UNSPECIFIED;
            try
            {
                settingName = (SettingName)Enum.Parse(typeof(SettingName), codice, true);
            }
            catch (Exception ex)
            {
                // Se il parametro su db non è elencato nell'enumerazione SettingName viene sollevata un'eccezione.
                // Se "SoloParametriAggiornati" del file di configurazione vale false, l'eccezione non viene lasciata risalire.
                if (SoloParametriAggiornati)
                {
                    throw new UnhandledSettingException(string.Format("Parametro non gestito: {0}", codice), ex);
                }
            }
            return settingName;
        }

        #region Metodi per tipizzazione parametri

        #region Type

        private Type GetTypeParam(object valore)
        {
            if (valore == null || valore == DBNull.Value)
            {
                return null;
            }

            Type type = Type.GetType(valore.ToString());

            if (type == null)
            {
                throw new TypeLoadException(string.Format("Type not found: {0}", valore));
            }

            return type;
        }

        #endregion

        #region Decimal

        private decimal GetDecimalParam(object valore)
        {
            if (valore == null || valore == DBNull.Value)
            {
                return -1;
            }

            decimal parametro = -1;

            try
            {
                parametro = Convert.ToDecimal(valore);
            }
            catch
            {
                parametro = -1;
            }

            return parametro;
        }

        #endregion

        #region String

        private string GetStringParam(object valore)
        {
            if (valore == null || valore == DBNull.Value)
            {
                return string.Empty;
            }

            return valore.ToString();
        }

        private List<string> GetListStringParam(object valore, string separatore)
        {
            List<string> lst = new List<string>();

            if (valore != null)
            {
                string[] str = valore.ToString().Trim().Split(separatore.ToCharArray());

                for (int i = 0; i < str.Length; i++)
                {
                    if (!string.IsNullOrEmpty(str[i]))
                    {
                        lst.Add(str[i].Trim().ToUpper());
                    }
                }
            }

            return lst;
        }

        private List<string> GetListStringParamToUpper(object valore, string separatore)
        {
            List<string> lst = new List<string>();

            if (valore != null)
            {
                string[] str = valore.ToString().Trim().Split(separatore.ToCharArray());

                for (int i = 0; i < str.Length; i++)
                {
                    if (!string.IsNullOrEmpty(str[i]))
                    {
                        lst.Add(str[i].Trim().ToUpper());
                    }
                }
            }

            return lst;
        }

        /// <summary>
        /// Decript del parametro. Il campo par_valore su db deve essere nel formato codice:valore.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="cryptedValue"></param>
        /// <returns></returns>
        private string GetStringCryptedParamWithCodePrefix(string code, object cryptedValue)
        {
            if (cryptedValue == null || cryptedValue == DBNull.Value)
            {
                return string.Empty;
            }

            // Decript del valore del parametro
            object decryptedValue = DecryptParamWithCodePrefix(code, cryptedValue);

            return GetStringParam(decryptedValue);
        }

        #endregion

        #region Boolean

        private bool GetBooleanParam(object valore)
        {
            if (valore == null || valore == DBNull.Value)
            {
                return false;
            }

            return (valore.ToString() == "S");
        }

        private bool? GetNullableBooleanParam(object valore)
        {
            if (valore == null || valore == DBNull.Value)
            {
                return null;
            }

            return GetBooleanParam(valore);
        }

        /// <summary>
        /// Decrittazione del parametro. Il campo par_valore su db deve contenere solo il valore del parametro.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="cryptedValue"></param>
        /// <returns></returns>
        private bool GetBooleanCryptedParam(string code, object cryptedValue)
        {
            if (cryptedValue == null || cryptedValue == DBNull.Value)
            {
                return false;
            }

            // Decript del valore del parametro
            object decryptedValue = DecryptParam(code, cryptedValue);

            return GetBooleanParam(decryptedValue);
        }

        /// <summary>
        /// Decrittazione del parametro. Il campo par_valore su db deve essere nel formato codice:valore.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="cryptedValue"></param>
        /// <returns></returns>
        private bool GetBooleanCryptedParamWithCodePrefix(string code, object cryptedValue)
        {
            if (cryptedValue == null || cryptedValue == DBNull.Value)
            {
                return false;
            }

            // Decript del valore del parametro
            object decryptedValue = DecryptParamWithCodePrefix(code, cryptedValue);

            return GetBooleanParam(decryptedValue);
        }

        #endregion

        #region int

        private int GetIntParam(object valore)
        {
            if (valore == null || valore == DBNull.Value)
            {
                return -1;
            }

            int parametro = -1;

            try
            {
                parametro = Convert.ToInt32(valore);
            }
            catch
            {
                parametro = -1;
            }

            return parametro;
        }

        private List<int> GetListIntParam(object valore, string separatore)
        {
            List<int> lst = new List<int>();

            if (valore != null)
            {
                string[] str = valore.ToString().Trim().Split(separatore.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < str.Length; i++)
                {
                    lst.Add(Convert.ToInt32(str[i].Trim()));
                }
            }

            return lst;
        }

        private int? GetNullableIntParam(object valore)
        {
            if (valore == null || valore == DBNull.Value)
            {
                return null;
            }

            return GetIntParam(valore);
        }

        private int GetIntCryptedParam(string code, object cryptedValue)
        {
            if (cryptedValue == null || cryptedValue == DBNull.Value)
            {
                return -1;
            }

            // Decript del valore del parametro
            object decryptedValue = DecryptParam(code, cryptedValue);

            return GetIntParam(decryptedValue);
        }

        private int GetIntCryptedParamWithCodePrefix(string code, object cryptedValue)
        {
            if (cryptedValue == null || cryptedValue == DBNull.Value)
            {
                return -1;
            }

            object decryptedValue = DecryptParamWithCodePrefix(code, cryptedValue);

            return GetIntParam(decryptedValue);
        }

        #endregion

        #region Enum

        private Enumerators.TipoAnags GetTipoAnagsParam(object valore)
        {
            Enumerators.TipoAnags parametro = Enumerators.TipoAnags.SoloLocale;

            try
            {
                parametro = (Enumerators.TipoAnags)Enum.Parse(typeof(Enumerators.TipoAnags), valore.ToString(), true);
            }
            catch (Exception e)
            {
                throw new UnhandledSettingException("TIPOANAG", e);
            }

            return parametro;
        }

        private TEntity? GetNullableEnumeratorValue<TEntity>(object valore)
            where TEntity : struct
        {
            if (valore == null || valore == DBNull.Value)
            {
                return null;
            }

            return (TEntity)Enum.Parse(typeof(TEntity), valore.ToString(), true);
        }

        private List<TEntity> GetListEnumeratorValues<TEntity>(object valore, string separatore)
            where TEntity : struct
        {
            List<TEntity> list = new List<TEntity>();

            if (valore == null || valore == DBNull.Value)
            {
                return list;
            }

            string[] arrayValori = valore.ToString().Trim().Split(separatore.ToCharArray());

            for (int i = 0; i < arrayValori.Length; i++)
            {
                if (!string.IsNullOrEmpty(arrayValori[i]))
                {
                    list.Add((TEntity)Enum.Parse(typeof(TEntity), arrayValori[i], true));
                }
            }

            if (list.Count == 0)
            {
                return null;
            }

            return list;
        }

        private Enumerators.TipoGestioneConvocazioniAltriConsultori GetConvocazioniAltroConsultorioParam(object valore)
        {
            Enumerators.TipoGestioneConvocazioniAltriConsultori? enumValue = GetNullableEnumeratorValue<Enumerators.TipoGestioneConvocazioniAltriConsultori>(valore);

            if (!enumValue.HasValue)
            {
                return Enumerators.TipoGestioneConvocazioniAltriConsultori.NonVisibili;
            }

            return enumValue.Value;
        }

        private Enumerators.CentraleLogFile GetCentraleLogFileParam(object valore)
        {
            Enumerators.CentraleLogFile? enumValue = GetNullableEnumeratorValue<Enumerators.CentraleLogFile>(valore);

            if (!enumValue.HasValue)
            {
                return Enumerators.CentraleLogFile.MAI;
            }

            return enumValue.Value;
        }

        private Enumerators.CentraleTipoAnagrafe GetCentraleTipoAnagrafeParam(object valore)
        {
            Enumerators.CentraleTipoAnagrafe parametro;

            try
            {
                parametro = (Enumerators.CentraleTipoAnagrafe)Enum.Parse(typeof(Enumerators.CentraleTipoAnagrafe), valore.ToString(), true);
            }
            catch (Exception e)
            {
                throw new UnhandledSettingException("CENTRALE_SEPATIPO", e);
            }

            return parametro;
        }

        #endregion

        #region Decrypt

        /// <summary>
        /// Restituisce il valore del parametro criptato nel formato "codiceParametro:valore".
        /// Devono essere specificati sia il codice che il valore criptato.
        /// Solleva un'eccezione se la decrittazione non riesce, oppure se il valore decrittato non è nel formato indicato.
        /// </summary>
        /// <param name="parameterCode"></param>
        /// <param name="cryptedValue"></param>
        /// <returns></returns>
        private object DecryptParamWithCodePrefix(string parameterCode, object cryptedValue)
        {
            // Decript del valore del parametro
            object decryptedValue = DecryptParam(parameterCode, cryptedValue);

            // Controllo presenza del nome del parametro nel valore (formato "codice:valore")
            object objValore = null;
            object[] arrVal = decryptedValue.ToString().Split(':');

            if (arrVal.Length == 2 && arrVal[0].ToString() == parameterCode)
            {
                objValore = arrVal[1];
            }
            else
            {
                throw new CryptedSettingException("Il parametro non è nel formato corretto.", parameterCode, null);
            }

            return objValore;
        }

        /// <summary>
        /// Restituisce il valore decrittato del parametro specificato.
        /// Solleva un'eccezione se la decrittazione non riesce.
        /// </summary>
        /// <param name="parameterCode"></param>
        /// <param name="cryptedValue"></param>
        /// <returns></returns>
        private object DecryptParam(string parameterCode, object cryptedValue)
        {
            if (cryptedValue == null || cryptedValue == DBNull.Value)
            {
                return null;
            }

            Crypto crypto = new Crypto(Providers.Rijndael);

            string decryptedValue = string.Empty;

            try
            {
                decryptedValue = crypto.Decrypt(cryptedValue.ToString());
            }
            catch (Exception ex)
            {
                // Se il decrypt non riesce, solleva un'eccezione.
                throw new CryptedSettingException("Il parametro contiene un valore non valido.", parameterCode, ex);
            }

            return decryptedValue;
        }

        #endregion

        #region DateTime

        private DateTime GetDateTimeParam(object valore)
        {
            if (valore == null || valore == DBNull.Value)
            {
                return DateTime.MinValue;
            }

            return Convert.ToDateTime(valore.ToString());
        }

        private DateTime? GetNullableDateTimeParam(object valore)
        {
            if (valore == null || valore == DBNull.Value)
            {
                return null;
            }

            return GetDateTimeParam(valore);
         }

        #endregion

        private FunctionalityStatus GetFunctionalityStatus(int v)
        {
            switch (v)
            {
                case 1:
                    return FunctionalityStatus.ENABLED;
                case 2:
                    return FunctionalityStatus.ENABLED_SELECTED;
                default:
                    return FunctionalityStatus.DISABLED;
            }
        }

        /// <summary>
        /// Restituisce la lista di elementi di tipo ConsensoAutoRilevazione valorizzata in base al valore specificato
        /// </summary>
        /// <param name="valore"></param>
        /// <returns></returns>
        private List<Entities.Consenso.ConsensoAutoRilevazione> GetListConsensoAutoRilevazione(object valore)
        {
            List<Entities.Consenso.ConsensoAutoRilevazione> lst = new List<Entities.Consenso.ConsensoAutoRilevazione>();

            if (valore != null)
            {
                string[] str = valore.ToString().Trim().Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (str != null && str.Length > 0)
                {
                    foreach (string item in str)
                    {
                        string[] id = item.Trim().Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                        // Devono esserci esattamente 2 elementi
                        if (id != null && id.Length == 2)
                        {
                            lst.Add(new Entities.Consenso.ConsensoAutoRilevazione()
                            {
                                IdConsenso = Convert.ToInt32(id[0]),
                                IdLivelloConsenso = Convert.ToInt32(id[1]),
                            });
                        }
                    }
                }
            }

            return lst;
        }

        #endregion

        #endregion
    }
}

