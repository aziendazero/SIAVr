Namespace Enumerators

    Public Enum TipoAnags As Integer
        SoloLocale = 0
        CentraleLettura = 1
        CentraleLettScritt = 2
        CentraleScrittInsLettAgg = 3
        CentraleLettLocaleAgg = 4
        LocaleLettCentraleScritt = 5
    End Enum

    ''' <summary>
    ''' Valori possibili per il filtro sui pazienti avvisati
    ''' </summary>
    Public Enum FiltroAvvisati
        SoloAvvisati
        SoloNonAvvisati
        Tutti
    End Enum

    ''' <summary>
    ''' Tipo di query. 
    ''' PostLike indica che, nelle query di like, verrà inserito il carattere jolly % al termine della stringa specificata.
    ''' PrePostLike indica l'inserimento del carattere jolly % sia all'inizio che alla fine della stringa specificata.
    ''' </summary>
    Public Enum QueryType
        PostLike
        PrePostLike
        Equals
    End Enum

    ''' <summary>
    ''' Stati anagrafici possibili. 
    ''' Il valore "OCCASIONALE" non è presente su db, ma è gestito nella GestionePazienti per cui è stato previsto.
    ''' </summary>
    Public Enum StatoAnagrafico As Short
        OCCASIONALE = -1
        RESIDENTE = 1
        DOMICILIATO = 2
        RESIDENTE_DOMICILIATO_FUORI_USL = 3
        NON_RESIDENTE_NON_DOMICILIATO = 4
        AIRE = 5
        IMMIGRATO = 6
        IRREPERIBILE = 7
        EMIGRATO = 8
        DECEDUTO = 9
        ANAGRAFICA_NON_RICONDOTTA = 10                      ' SOLO ASTI
        NON_RESIDENTE_NON_DOMICILIATO_ASSISTITO = 11        ' SOLO VENETO, in sostituzione di DOMICILIATO_NON_ASSISTITO
        CESSATO = 12                                        ' VENETO e AREZZO
        DOMICILIATO_NON_IN_CHIAMATA = 12                    ' BOLOGNA
        PAZIENTE_NON_CERTIFICATO_AURA = 13                  ' SOLO ASTI
        NUOVO_DOMICILIATO = 14                              ' SOLO AREZZO
        CESSATO_BOLOGNA = 15                                ' BOLOGNA
    End Enum

    ''' <summary>
    ''' Stati vaccinali possibili per il paziente
    ''' </summary>
    Public Enum StatiVaccinali
        InCorso = 3
        Terminato = 4
        InadempienteTotale = 9
    End Enum

    Public Enum UnitaMisuraLotto
        Dose
        Scatola
    End Enum

    Public Enum DipendenzaSostituta
        Sinistra
        Destra
    End Enum

    ''' <summary>
    ''' Tipologia di indirizzo utilizzato nella gestione del paziente.
    ''' </summary>
    Public Enum TipoIndirizzo
        Residenza = 0
        Domicilio = 1
        Nessuno = 2
    End Enum

    ''' <summary>
    ''' Possibili stati di inadempienza.
    ''' </summary>
    Public Enum StatoInadempienza
        TerminePerentorio = 1
        ComunicazioneAlSindaco = 2
        CasoConcluso = 3
    End Enum

    Public Class MovimentiCNS

        Public Enum StatoNotificaEmigrazione
            Nessuno = 0
            Notificato = 1
            Avvertimento = 2
            Errore = 3
        End Enum

        Public Enum StatoAcquisizioneImmigrazione
            Nessuno = 0
            Acquisito = 1
            Avvertimento = 2
            Errore = 3
        End Enum

    End Class

    ''' <summary>
    ''' Valori del flag di controllo del consenso
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum ControlloConsenso As Integer

        ''' <summary>
        ''' Non definito
        ''' </summary>
        ''' <remarks></remarks>
        Undefined = 0

        ''' <summary>
        ''' Errore Bloccante = E
        ''' </summary>
        ''' <remarks></remarks>
        Bloccante = 1

        ''' <summary>
        ''' Warning = W
        ''' </summary>
        ''' <remarks></remarks>
        Warning = 2

        ''' <summary>
        ''' Non Bloccante = N
        ''' </summary>
        ''' <remarks></remarks>
        NonBloccante = 3

    End Enum

	''' <summary>
	''' 
	''' </summary>
	''' <remarks></remarks>
	Public Enum StatoAcquisizioneElaborazioneVaccinazionePaziente
		DaAcquisire = 0
		AcquisizioneInCorso = 1
		AcquisitaCorrettamente = 2
		NoCorrispondenzaPaziente = 3
		CorrispondenzaPazienteMultipla = 4
		VaccinazioneEsistente = 5
		VaccinazioneEsistenteInDataSuccessiva = 6
		ConsensoPazienteBloccante = 7
		Eccezione = 99
	End Enum
	Public Enum StatoAcquisizioneElaborazioneVaccinazioneAcn
		DaAcquisire = 0
		AcquisizioneInCorso = 1
		AcquisitaCorrettamente = 2
		NoCorrispondenzaPaziente = 3
		CorrispondenzaPazienteMultipla = 4
		VaccinazioneEsistente = 5
		VaccinazioneEsistenteInDataSuccessiva = 6
		ConsensoPazienteBloccante = 7
		Eccezione = 99
	End Enum

	''' <summary>
	''' 
	''' </summary>
	''' <remarks></remarks>
	Public Enum TipoOperazioneSedeVac
        Nessuna = 0
        Ok = 1
        ImpostazioneConsultorio = 2
        CancellazioneConsultorio = 3
        ImpostazioneManualeConsultorio = 4
    End Enum

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum TipoImpostazioneSedeVaccinale
        CnsNo = 0
        CnsSi = 1
    End Enum

    ''' <summary>
    ''' Verso ordinamento ASC/DESC
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum VersoOrdinamento
        ASC = 0
        DESC = 1
    End Enum

    ''' <summary>
    ''' Livello dell'utente relativamente alla gestione delle convocazioni e delle prenotazioni 
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum LivelloUtenteConvocazione As Integer
        [Default] = 0
        Standard = 1
        Administrator = 2
        Undefined = 99
    End Enum

    ''' <summary>
    ''' Gestione delle convocazioni relative a consultori diversi da quello di lavoro
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum TipoGestioneConvocazioniAltriConsultori As Integer
        NonVisibili = 0
        SolaVisualizzazione = 1
        Modificabili = 2
    End Enum

    ''' <summary>
    ''' Gestione del calcolo della scadenza per il motivo esclusione
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum MotiviEsclusioneCalcoloScadenza As Integer
        NessunCalcolo = 0
        Nascita = 1
        Registrazione = 2
        Visita = 3
    End Enum

    ''' <summary>
    ''' Tipologie di compilazione del bilancio di salute
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum TipoCompilazioneBilancio
        Normale = 0
        Predefinita = 1
        Compilata = 2
        RispostaPrecedente = 3
    End Enum

    ''' <summary>
    ''' Tipologia di percentile per i bilanci
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum TipoPercentile As Integer
        ' N.B. : Il valore del tipo corrisponde al valore usato su db. Il tipo "0" non esiste!
        Peso = 1
        Altezza = 2
        Cranio = 3
    End Enum

    ''' <summary>
    ''' Indicazione dello stato di un campo
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum StatoAbilitazioneCampo
        Disabilitato = 0
        Abilitato = 1
        Obbligatorio = 2
    End Enum
    ''' <summary>
    ''' Stato delle notifiche inviate/da inviare
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum StatoLogNotificaInviata
        DaInviare = 0
        Inviata = 1
		Errore = 2
		DaNonInviare = 3
	End Enum
    ''' <summary>
    ''' Operazioni log notifiche
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum OperazioneLogNotifica
        Inserimento
        Modifica
        Merge
        InserimentoVaccinazioniEseguite
        ModificaVaccinazioniEseguite
        EliminazioneVaccinazioniEseguite
        InserimentoSegnalazioneReazioneAvversa
        IndicizzazioneCertVaccFSE
		StoricizzazioneCertVaccFSE
        RecuperoCertVaccFSE
        ComunicazioneOTPCovid
        RicercaQPv2
    End Enum

    ''' <summary>
    ''' Simboli contenuti nella legenda delle vaccinazioni
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum LegendaVaccinazioniItemType
        ReazioneAvversa
        EseguitaScaduta
        EseguitaFittizia
        EsclusioneScaduta
        Eseguita
        Esclusa
        Obbligatoria
    End Enum

    Public Enum ProvenienzaVaccinazioni
        ACN
        RSA
    End Enum

    Public Enum TipoPaziente
        ITALIANO_STRANIERO_RES = 0
        STP = 1
        ENI = 2
        STRANIERO = 3
        RICHIEDENTE_ASILO = 4
    End Enum

    Public Enum TipoStruttura
        FLS11
        HSP11
        STS11
        RIA11
        AMB_MMG_PLS
    End Enum

    Public Enum TipoAutorizzazione
        Nessuno = 0
        SoloSuperUtenti = 1
        TuttiUtenti = 2
    End Enum

    ''' <summary>
    ''' Indica la provenienza dei dati anagrafici
    ''' </summary>
    Public Enum FonteAnagrafica
        AnagrafeLocale = 0
        AnagrafeCentrale = 1
        Mista = 2
    End Enum

#Region " Centrale "

    Public Enum CentraleLogFile As Integer
        MAI = 0
        ERRORE = 1
        SEMPRE = 2
    End Enum

    Public Enum CentraleTipoAnagrafe As Integer
        COGNOME_NOME_SEPARATI = 1
        COGNOME_NOME_UNITI = 2
    End Enum

    Public Enum StatoAcquisizioneDatiVaccinaliCentrale
        Vuota = 0
        Parziale = 1
        Totale = 2
    End Enum

    Public Enum StatoLogDatiVaccinaliCentrali
        Success = 0
        Warning = 1
        [Error] = 2
    End Enum

#End Region
#Region " Integrazione Call Center "
    Public Enum OperazioneCallCenter
        RicercaPaziente
        SelezionePaziente
        PrenotazioneAppuntamento
        SpostamentoAppuntamento
        EliminazioneAppuntamento
        CreazioneConvocazione
    End Enum
#End Region
End Namespace
