Namespace Constants

    ''' <summary>
    ''' Costanti generiche
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CommonConstants

        ''' <summary>
        ''' Versione dell'applicativo e delle sue librerie
        ''' </summary>
        ''' <remarks></remarks>
        Public Const Version As String = "4.24.0.0"

        ''' <summary>
        ''' Stato vaccinale di default nell'inserimento di un paziente
        ''' </summary>
        ''' <remarks></remarks>
        Public Const StatoVaccinaleDefault As Enumerators.StatiVaccinali = OnVac.Enumerators.StatiVaccinali.InCorso

        ''' <summary>
        ''' Anno considerato come indicatore di ricorsività della data
        ''' </summary>
        ''' <remarks></remarks>
        Public Const RECURSIVE_YEAR As String = "1904"

        ''' <summary>
        ''' Nome del campo Anno di Nascita utilizzato per la ricerca paziente
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ANNO_NASCITA As String = "ANNO_NASCITA"

        ''' <summary>
        ''' Codice del consultorio generico "VAC"
        ''' </summary>
        ''' <remarks></remarks>
        Public Const CodiceConsultorioSistema As String = "VAC"

#Region " Costanti per gestione menu (utilizzati da TopFrame e LeftFrame)"

        ''' <summary>
        ''' Numero massimo di livelli annidati supportati dal menu * MENU_VOICE_LENGHT
        ''' </summary>
        ''' <remarks></remarks>
        Public Const MENU_LEVELS As Byte = 8

        ''' <summary>
        ''' Numero di caratteri utilizzato per esprimere il peso del livello del menu
        ''' </summary>
        ''' <remarks></remarks>
        Public Const MENU_VOICE_LENGHT As Byte = 2

        ''' <summary>
        ''' Carattere di completamento del peso dei menu
        ''' </summary>
        ''' <remarks></remarks>
        Public Const MENU_PADDING_CHARACTER As String = "X"

#End Region

    End Class

    ''' <summary>
    ''' http://download.oracle.com/docs/cd/B28359_01/server.111/b28278/toc.htm
    ''' </summary>
    ''' <remarks></remarks>
    Public Class OracleErrors

        ''' <summary>
        ''' ORA-00001: unique constraint (string.string) violated
        ''' Cause: An UPDATE or INSERT statement attempted to insert a duplicate key. For Trusted Oracle configured in DBMS MAC mode, you may see this message if a duplicate entry exists at a different level.
        ''' Action: Either remove the unique restriction or do not insert the key.
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ORA_00001 As String = "ORA-00001"

        ''' <summary>
        ''' ORA-02291: integrity constraint (string.string) violated - parent key not found
        ''' Cause: A foreign key value has no matching primary key value.
        ''' Action: Delete the foreign key or add a matching primary key.
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ORA_02291 As String = "ORA-02291"

        ''' <summary>
        ''' ORA-02292: integrity constraint (string.string) violated - child record found
        ''' Cause: attempted to delete a parent key value that had a foreign key dependency.
        ''' Action: delete dependencies first then parent or disable constraint. 
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ORA_02292 As String = "ORA-02292"

    End Class

    ''' <summary>
    ''' Stati possibili del bilancio di un paziente
    ''' </summary>
    ''' <remarks></remarks>
    Public Class StatiBilancio

        ''' <summary>
        ''' Bilancio eseguito
        ''' </summary>
        Public Const EXECUTED As String = "EX"

        ''' <summary>
        ''' Bilancio non ancora eseguito e non scaduto
        ''' </summary>
        Public Const UNEXECUTED As String = "UX"

        ''' <summary>
        ''' Bilancio scaduto
        ''' </summary>
        Public Const UNSOLVED As String = "US"

    End Class

    ''' <summary>
    ''' Tipologia del comune (campo com_tipo dell'anagrafica t_ana_comuni)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TipologiaComune

        Public Const ComuneItaliano As String = "C"

        Public Const Stato As String = "S"

    End Class

    ''' <summary>
    ''' Codici dei luoghi di vaccinazione utilizzati nel programma. Tutti i luoghi sono specificati nel parametro LUOGHI.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CodiceLuogoVaccinazione

        Public Const CentroVaccinale As String = "CO"

        Public Const AltraAusl As String = "FA"

        Public Const Estero As String = "ES"

        Public Const InRegione As String = "RE"

    End Class

    ''' <summary>
    ''' Nomi dei report utilizzati nell'applicativo.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ReportName

        Public Const AppuntamentiDelGiorno As String = "AppuntamentiDelGiorno.rpt"
        Public Const AppuntamentiDelGiornoCup As String = "AppuntamentiDelGiornoCup.rpt"
        Public Const AvvisoAppuntamento As String = "AvvisoAppuntamento.rpt"
        Public Const AvvisoBianco As String = "AvvisoBianco.rpt"
        Public Const AvvisoCampagnaAdulti As String = "AvvisoCampagnaAdulti.rpt"
        Public Const AvvisoMaggiorenni As String = "AvvisoMaggiorenni.rpt"
        Public Const AvvisoMantoux As String = "AvvisoMantoux.rpt"
        Public Const AvvisoSoloBilancio As String = "AvvisoSoloBilancio.rpt"
        Public Const BilanciMalattia As String = "BilanciMalattia.rpt"
        Public Const BilancioInBianco As String = "BilancioInBianco.rpt"
        Public Const CalendarioVaccinale As String = "CalendarioVaccinale.rpt"
        Public Const CertificatoDiscrezionale As String = "CertificatoDiscrezionale.rpt"
        Public Const CertificatoEseguiteScadute As String = "CertificatoEseguiteScadute.rpt"
        Public Const CertificatoFrequenza As String = "CertificatoFrequenza.rpt"
        Public Const CertificatoMantoux As String = "CertificatoMantoux.rpt"
        Public Const CertificatoVaccinale As String = "CertificatoVaccinale.rpt"
        Public Const CertificatoVaccinaleGiornaliero As String = "CertificatoVaccinaleGiornaliero.rpt"
        Public Const CertificatoVaccinaleMassivo As String = "CertificatoVaccinaleMassivo.rpt"
        Public Const Cicli As String = "Cicli.rpt"
        Public Const Consulenze As String = "Consulenze.rpt"
        Public Const ConteggioSedute As String = "ConteggioSedute.rpt"
        Public Const CoperturaNonVaccinati As String = "CoperturaNonVaccinati.rpt"
        Public Const CoperturaVaccinale As String = "CoperturaVaccinale.rpt"
        Public Const CoperturaVaccinaleAssociazione As String = "CoperturaVaccinaleAssociazione.rpt"
        Public Const CoperturaVaccinaleCNS As String = "CoperturaVaccinaleCNS.rpt"
        Public Const CoperturaVaccinati As String = "CoperturaVaccinati.rpt"
        Public Const DocumentazioneAssistiti As String = "DocumentazioneAssistiti.rpt"
        Public Const ElencoAssistiti As String = "ElencoAssistiti.rpt"
        Public Const ElencoBilanciMalattia As String = "ElencoBilanciMalattia.rpt"
        Public Const ElencoConvocati As String = "ElencoConvocati.rpt"
        Public Const ElencoConvocatiSelezionati As String = "ElencoConvocatiSelezionati.rpt"
        Public Const ElencoEmigrati As String = "ElencoEmigrati.rpt"
        Public Const ElencoEmigratiComune As String = "ElencoEmigratiComune.rpt"
        Public Const ElencoEsclusione As String = "ElencoEsclusione.rpt"
        Public Const ElencoEsclusioneVaccinazioni As String = "ElencoEsclusioneVaccinazioni.rpt"
        Public Const ElencoImmigrati As String = "ElencoImmigrati.rpt"
        Public Const ElencoImmigratiComune As String = "ElencoImmigratiComune.rpt"
        Public Const ElencoLogDatiVaccinali As String = "ElencoLogDatiVaccinali.rpt"
        Public Const ElencoMantoux As String = "ElencoMantoux.rpt"
        Public Const ElencoMovimenti As String = "ElencoMovimenti.rpt"
        Public Const ElencoNonVaccinati As String = "ElencoNonVaccinati.rpt"
        Public Const ElencoNonVaccinatiPediatra As String = "ElencoNonVaccinatiPediatra.rpt"
        Public Const ElencoNotifiche As String = "ElencoNotifiche.rpt"
        Public Const ElencoRitardatari As String = "ElencoRitardatari.rpt"
        Public Const ElencoSoloBilanci As String = "ElencoSoloBilanci.rpt"
        Public Const ElencoSospesi As String = "ElencoSospesi.rpt"
        Public Const EsitoCampagna As String = "EsitoCampagna.rpt"
        Public Const EtichetteAssistiti As String = "EtichetteAssistiti.rpt"
        Public Const EtichetteAssistitiAvvisi As String = "EtichetteAssistitiAvvisi.rpt"
        Public Const EtichetteAvvisoAppuntamento As String = "EtichetteAvvisoAppuntamento.rpt"
        Public Const EtichetteImmigrati As String = "EtichetteImmigrati.rpt"                                                        ' Utilizzato sia per immigrati che per emigrati
        Public Const EtichetteNuoviNati As String = "EtichetteNuoviNati.rpt"
        Public Const EtichetteSpedizioneAssistiti As String = "EtichetteSpedizioneAssistiti.rpt"
        Public Const LibrettoVaccinale As String = "LibrettoVaccinale.rpt"
        Public Const LibrettoVaccinale2 As String = "LibrettoVaccinale2.rpt"
        Public Const LibrettoVaccinale3 As String = "LibrettoVaccinale3.rpt"
        Public Const LogGestioneAppuntamenti As String = "LogGestioneAppuntamenti.rpt"
        Public Const GestioneCampagneConvocazNonRiuscite As String = "GestioneCampagneConvocazNonRiuscite.rpt"                      ' Non usato nel codice
        Public Const MagazzinoElencoMovimenti As String = "MagazzinoElencoMovimenti.rpt"
        Public Const MagazzinoLotti As String = "MagazzinoLotti.rpt"
        Public Const MagazzinoQuantitativiMovimentatiConsultorio As String = "MagazzinoQuantitativiMovimentatiConsultorio.rpt"
        Public Const MagazzinoQuantitativiMovimentatiLotto As String = "MagazzinoQuantitativiMovimentatiLotto.rpt"
        Public Const MotiviInadempienti As String = "MotiviInadempienti.rpt"
        Public Const Operatori As String = "Operatori.rpt"
        Public Const OperazioniGruppo As String = "OperazioniGruppo.rpt"                                                            ' Non usato nel codice
        Public Const OrariAmbulatorio As String = "OrariAmbulatorio.rpt"
        Public Const OrariConsultorio As String = "OrariConsultorio.rpt"                                                            ' Non usato nel codice
        Public Const PianoDiLavoroCup As String = "PianoDiLavoroCup.rpt"
        Public Const ReazioniAvverse As String = "ReazioniAvverse.rpt"
        Public Const RistampaTerminePerentorio As String = "RistampaTerminePerentorio.rpt"
        Public Const RisultatiProcessi As String = "RisultatiProcessi.rpt"
        Public Const SollecitiPazienti As String = "SollecitiPazienti.rpt"
        Public Const StampaBilanciMalattia As String = "StampaBilanciMalattia.rpt"
        Public Const StatisticaReazioniAvverse As String = "StatisticaReazioniAvverse.rpt"
        Public Const UtilizzoLotto As String = "UtilizzoLotto.rpt"
        Public Const VaccinazioniEseguite As String = "VaccinazioniEseguite.rpt"
        Public Const VaccinazioniEseguiteCampagna As String = "VaccinazioniEseguiteCampagna.rpt"
        Public Const VaccinazioniEseguiteTotCNS As String = "VaccinazioniEseguiteTotCNS.rpt"
        Public Const VaccinazioniEseguiteXCircosc As String = "VaccinazioniEseguiteXCircosc.rpt"
        Public Const VaccinazioniEseguiteXComune As String = "VaccinazioniEseguiteXComune.rpt"
        Public Const VaccinazioniEseguiteXEsenzioneMalattia As String = "VaccinazioniEseguiteXEsenzioneMalattia.rpt"
        Public Const VaccinazioniGiornaliere As String = "VaccinazioniGiornaliere.rpt"
        Public Const VaccinazioniProgrammate As String = "VaccinazioniProgrammate.rpt"
        Public Const VaccinazioniProgrammateAssociazioni As String = "VaccinazioniProgrammateAssociazioni.rpt"
        Public Const VaccinazioniRifiutate As String = "VaccinazioniRifiutate.rpt"
        Public Const VaccinazioniRifiutatePaziente As String = "VaccinazioniRifiutatePaziente.rpt"
        Public Const VacciniSomministrati As String = "VacciniSomministrati.rpt"
        Public Const VacciniSomministratiAmbulatorio As String = "VacciniSomministratiAmbulatorio.rpt"
        Public Const VacciniSomministratiDose As String = "VacciniSomministratiDose.rpt"
        Public Const VacciniSomministratiEsenzioneMalattia As String = "VacciniSomministratiEsenzioneMalattia.rpt"
        Public Const VacciniSomministratiNomeCommerciale As String = "VacciniSomministratiNomeCommerciale.rpt"
        Public Const VaccSommCatRischio As String = "VaccSommCatRischio.rpt"
        Public Const VariazioniCns As String = "VariazioniCns.rpt"
        Public Const AnamnesiDefault As String = "AnamnesiDefault.rpt"
        Public Const AnamnesiViaggiatori As String = "AnamnesiViaggiatori.rpt"
        Public Const ElencoMantouxCompleto As String = "ElencoMantouxCompleto.rpt"

        ' TODO Report telerik 
        Public Const CoperturaVaccinaleMedico As String = "CoperturaVaccinaleMedico"
        Public Const ElencoVaccinatiMedico As String = "ElencoVaccinatiMedico"
        Public Const ElencoNonVaccinatiMedico As String = "ElencoNonVaccinatiMedico"
        Public Const ElencoNonVaccinatiPazienteMedico As String = "ElencoNonVaccinatiPazienteMedico"

        ' Estrazioni Excel con ReportViewer
        Public Const EstrazioneControlliCentri As String = "EstrazioneControlliCentri"
        Public Const EstrazioneControlliScuole As String = "EstrazioneControlliScuole"
        Public Const EstrazioneElencoConsulenze As String = "EstrazioneElencoConsulenze"

    End Class

    ''' <summary>
    ''' Tipologie di movimenti di magazzino
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TipoMovimentoMagazzino

        Public Const Carico As String = "C"

        Public Const Scarico As String = "S"

        Public Const TrasferimentoA As String = "A"

        Public Const TrasferimentoDa As String = "D"

        Public Const NonDefinito As String = ""

    End Class

    ''' <summary>
    ''' Tipologie di progressivi utilizzati
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TipoProgressivo

        ''' <summary>
        ''' Progressivo di associazione = "ASSOCIAZIONEPROG"
        ''' </summary>
        ''' <remarks></remarks>
        Public Const AssociazioneProg As String = "ASSOCIAZIONEPROG"

        ''' <summary>
        ''' Progressivo per il calcolo del codice ausiliario del paziente = "AUSILIARIO"
        ''' </summary>
        ''' <remarks></remarks>
        Public Const Ausiliario As String = "AUSILIARIO"

        ''' <summary>
        ''' Progressivo del movimento di magazzino = "MAGAZZINO"
        ''' </summary>
        ''' <remarks></remarks>
        Public Const Magazzino As String = "MAGAZZINO"

        ''' <summary>
        ''' Progressivo del consenso = "CON_PROG"
        ''' </summary>
        ''' <remarks></remarks>
        Public Const Consenso As String = "CON_PROG"

    End Class

    Public Class TipoLuogoEsecuzioneVaccinazione
        Public Const NonDefinito As String = ""
        Public Const InAzienda As String = "0"
        Public Const FuoriAzienda As String = "1"
        Public Const FuoriRegione As String = "2"
        Public Const Estero As String = "3"
    End Class

    ''' <summary>
    ''' Modalità di accesso 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ModalitaAccesso

        Public Const PrenotatiCup As String = "PC"

        Public Const ProntoSoccorso As String = "PS"

        Public Const AccessoVolontario As String = "AV"

        Public Const AppuntamentoOnVac As String = "AO"

    End Class

    ''' <summary>
    ''' Tipologia di numero civico
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TipoNumeroCivicoVia

        Public Const Tutti As String = "T"

        Public Const Pari As String = "P"

        Public Const Dispari As String = "D"

    End Class

    ''' <summary>
    ''' Tipo di controllo da effettuare
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TipoControlloCampiPaziente

        ''' <summary>
        ''' Nessun controllo
        ''' </summary>
        ''' <remarks></remarks>
        Public Const NessunControllo As String = ""

        ''' <summary>
        ''' Errore bloccante
        ''' </summary>
        ''' <remarks></remarks>
        Public Const Bloccante As String = "E"

        ''' <summary>
        ''' Warning non bloccante
        ''' </summary>
        ''' <remarks></remarks>
        Public Const NonBloccante As String = "W"

    End Class

    ''' <summary>
    ''' Funzionalità per i controlli sui campi del paziente
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FunzioniControlloCampiPaziente

        Public Const InserimentoAnagraficaPaziente As String = "ANAG_INS"
        Public Const ModificaAnagraficaPaziente As String = "ANAG_MOD"

    End Class

    ''' <summary>
    ''' Tipi di connessioni gestite dall'OnitDataPanel
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ConnessioneAnagrafe

        Public Const CENTRALE As String = "centrale"
        Public Const LOCALE As String = "locale"
        Public Const LOCALE_CNAS As String = "locale_cnas"
        Public Const LOCALE_COMRE As String = "locale_comre"
        Public Const LOCALE_COMDO As String = "locale_comdo"
        Public Const LOCALE_COMDOSAN As String = "locale_comdosan"
        Public Const LOCALE_CITT As String = "locale_citt"
        Public Const LOCALE_AUSL As String = "locale_ausl"
        Public Const LOCALE_AUSL_ASS As String = "locale_ausl_ass"
        Public Const LOCALE_AUSL_ASS_PRECEDENTE As String = "locale_ausl_ass_precedente"
        Public Const LOCALE_MED As String = "locale_med"
        Public Const LOCALE_CIR As String = "locale_cir"
        Public Const LOCALE_CIR_2 As String = "locale_cir_2"
        Public Const LOCALE_DIS As String = "locale_dis"
        Public Const LOCALE_RIS As String = "locale_ris"

    End Class

    ''' <summary>
    ''' Tipologia del consultorio
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TipoConsultorio

        Public Const Adulti As String = "A"
        Public Const Pediatrico As String = "P"
        Public Const Vaccinale As String = "V"
        Public Const Nessuno As String = ""

    End Class

    ''' <summary>
    ''' Possibili valori relativi all'obbligatorietà della vaccinazione.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ObbligatorietaVaccinazione
        Public Const Obbligatoria As String = "A"
        Public Const Raccomandata As String = "B"
        Public Const Facoltativa As String = "C"
    End Class

    ''' <summary>
    ''' Possibili valori per il tipo di vaccinazione eseguita, presente in centrale
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TipoVaccinazioneEseguitaCentrale
        Public Const Programmata As String = "P"
        Public Const Recuperata As String = "E"
        Public Const Registrata As String = "R"
        Public Const Scaduta As String = "S"
        Public Const ScadutaEliminata As String = "C"
        Public Const Ripristinata As String = "V"
        Public Const Eliminata As String = "D"
    End Class

    ''' <summary>
    ''' Possibili valori per il tipo di reazione avversa, presente in centrale
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TipoReazioneAvversaCentrale
        Public Const Nessuno As String = ""
        Public Const Eliminata As String = "D"
    End Class

    ''' <summary>
    ''' Possibili valori per il tipo di vaccinazione esclusa, presente in centrale
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TipoVaccinazioneEsclusaCentrale
        Public Const Nessuno As String = ""
        Public Const Eliminata As String = "D"
    End Class

    ''' <summary>
    ''' Possibili valori per il tipo di visita, presente in centrale
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TipoVisitaCentrale
        Public Const Nessuno As String = ""
        Public Const Eliminata As String = "D"
    End Class

    ''' <summary>
    ''' Possibili valori relativi alla visibilità dei dati presenti nello storico vaccinale centralizzato
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ValoriVisibilitaDatiVaccinali

        ''' <summary>
        ''' Visibilità dei dati vaccinali NEGATA DALLA USL = "L"
        ''' </summary>
        ''' <remarks></remarks>
        Public Const NegatoDaUsl As String = "L"

        ''' <summary>
        ''' Visibilità dei dati vaccinali NEGATA DAL PAZIENTE = "N"
        ''' </summary>
        ''' <remarks></remarks>
        Public Const NegatoDaPaziente As String = "N"

        ''' <summary>
        ''' Visibilità dei dati vaccinali CONCESSA DAL PAZIENTE = "V"
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ConcessoDaPaziente As String = "V"

    End Class

    ''' <summary>
    ''' Possibili valori che può assumere il consenso del paziente alla distribuzione dei dati vaccinali tra le usl
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ValoriConsensoPazienteDatiVaccinali

        ''' <summary>
        ''' Consenso alla distribuzione dei dati vaccinali ASSENTE = "A"
        ''' </summary>
        ''' <remarks></remarks>
        Public Const Assente As String = "A"

        ''' <summary>
        ''' Consenso alla distribuzione dei dati vaccinali NEGATO = "N"
        ''' </summary>
        ''' <remarks></remarks>
        Public Const Negato As String = "N"

        ''' <summary>
        ''' Consenso alla distribuzione dei dati vaccinali CONCESSO = "V"
        ''' </summary>
        ''' <remarks></remarks>
        Public Const Concesso As String = "V"

        ''' <summary>
        ''' Consenso alla distribuzione dei dati vaccinali SCADUTO = "S"
        ''' </summary>
        ''' <remarks></remarks>
        Public Const Scaduto As String = "S"

        ''' <summary>
        ''' Consenso alla distribuzione dei dati vaccinali REVOCATO = "R"
        ''' </summary>
        ''' <remarks></remarks>
        Public Const Revocato As String = "R"

        ''' <summary>
        ''' Consenso alla distribuzione dei dati vaccinali CANCELLATO = "C"
        ''' </summary>
        ''' <remarks></remarks>
        Public Const Cancellato As String = "C"

        ''' <summary>
        ''' Consenso alla distribuzione dei dati vaccinali RICONCESSO = "P"
        ''' </summary>
        ''' <remarks></remarks>
        Public Const Riconcesso As String = "P"

    End Class

    ''' <summary>
    ''' Valori per il MIME Content Type della pagina
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MIMEContentType

        Public Const APPLICATION_OCTET_STREAM As String = "application/octet-stream"
        Public Const APPLICATION_PDF As String = "application/pdf"
        Public Const APPLICATION_PKCS7 As String = "application/pkcs7"
        Public Const APPLICATION_EXCEL As String = "application/excel"

        Public Const TEXT_CSS As String = "text/css"
        Public Const TEXT_HTML As String = "text/html"
        Public Const TEXT_PLAIN As String = "text/plain"
        Public Const TEXT_XML As String = "text/xml"

        ' ...

    End Class

    Public Class TransferEncoding
        Public Const SevenBit As String = "7bit"
        Public Const Binary As String = "binary"
    End Class

    Public Class Charset
        Public Const US_ASCII As String = "us-ascii"
    End Class

    Public Class OperatoreConfronto

        Public Const Maggiore As String = ">"

        Public Const Minore As String = "<"

        Public Const Uguale As String = "="

    End Class

    ''' <summary>
    ''' Codice e descrizione dell'ambulatorio fittizio che rappresenta tutti gli ambulatori.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class AmbulatorioTUTTI

        ''' <summary>
        ''' Codice dell'ambulatorio fittizio che rappresenta tutti gli ambulatori di un consultorio
        ''' </summary>
        ''' <remarks>Deve sempre essere pari a 0, perchè la logica dell'applicativo se lo aspetta così</remarks>
        Public Const Codice As Integer = 0

        ''' <summary>
        ''' Descrizione dell'ambulatorio fittizio che rappresenta tutti gli ambulatori di un consultorio
        ''' </summary>
        ''' <remarks></remarks>
        Public Const Descrizione As String = "TUTTI"

    End Class

    ''' <summary>
    ''' Indica la provenienza del valore impostato nel sito di inoculazione o nella via di somministrazione
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TipoValorizzazioneSitoVia

        Public Const DaCiclo As String = "C"

        Public Const DaAssociazione As String = "A"

        Public Const DaNomeCommerciale As String = "N"

        Public Const Manuale As String = "M"

    End Class

    ''' <summary>
    ''' Possibili stampe relative agli appuntamenti
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TipoStampaAppuntamento
        Public Const Avvisi As String = "A"
        Public Const ElencoAvvisi As String = "EA"
        Public Const EtichetteAvvisi As String = "ETA"
        Public Const Bilanci As String = "B"
        Public Const ElencoBilanci As String = "EB"
        Public Const BilanciMalattia As String = "BM"
        Public Const CampagnaAdulti As String = "CA"
        Public Const ElencoBilanciMalattia As String = "EBM"
        Public Const AvvisoBilancio As String = "AB"            ' utilizzata in un caso nella RicercaAppuntamenti
        Public Const EtichetteAssistitiAvvisi As String = "EAA"
    End Class

    ''' <summary>
    ''' Tipologie di avviso previste per l'export del tracciato Postel
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TipoAvvisoPostel

        Public Const Avvisi As String = "AV"

        Public Const Sollecito As String = "SL"

        Public Const TerminePerentorio As String = "TP"

    End Class

    Public Class TipoSesso
        Public Const Maschio As String = "M"
        Public Const Femmina As String = "F"
        Public Const Entrambi As String = "E"
    End Class

    Public Class TipoDocumentoFirmaDigitale
        Public Const Visita As String = "VIS"
    End Class

    Public Class TipologiaInterventi
        Public Const Assistenziale As String = "ASS"
        Public Const Amministrativo As String = "AMM"
        Public Const Altro As String = "ALT"
    End Class

    ''' <summary>
    ''' Verso dell'ordinamento di query e/o griglie
    ''' </summary>
    ''' <remarks></remarks>
    Public Class VersoOrdinamento

        Public Const Crescente As String = "ASC"
        Public Const Decrescente As String = "DESC"

    End Class

    ''' <summary>
    ''' Stato dell'elaborazione di un paziente nella t_paz_elaborazioni
    ''' </summary>
    ''' <remarks></remarks>
    Public Class StatoElaborazioneBatch

        ''' <summary>
        ''' Elaborazione ancora da effettuare o in corso
        ''' </summary>
        ''' <remarks></remarks>
        Public Const DaElaborare As String = ""

        ''' <summary>
        ''' Elaborazione terminata correttamente = "C"
        ''' </summary>
        ''' <remarks></remarks>
        Public Const TerminataCorrettamente As String = "C"

        ''' <summary>
        ''' Elaborazione terminata con errore ="E"
        ''' </summary>
        ''' <remarks></remarks>
        Public Const Errore As String = "E"

    End Class

    Public Class TipoPrenotazioneAppuntamento

        ''' <summary>
        ''' Prenotazione automatica dell'appuntamento
        ''' </summary>
        ''' <remarks></remarks>
        Public Const Automatica As String = "A"

        ''' <summary>
        ''' Prenotazione manuale dalla maschera Gestione Appuntamenti
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ManualeDaGestioneAppuntamenti As String = "M"

        ''' <summary>
        ''' Prenotazione manuale dalla maschera Ricerca Appuntamenti
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ManualeDaRicercaAppuntamenti As String = "R"

    End Class

    Public Class MotiviEliminazioneAppuntamento

        Public Const Sollecito As String = "01"
        Public Const Sospensione As String = "02"
        Public Const Esecuzione As String = "03"
        Public Const Esclusione As String = "04"
        Public Const SpostamentoConvocazione As String = "05"
        Public Const EliminazioneConvocazione As String = "06"
        Public Const VariazioneConsultorio As String = "07"
        Public Const VariazioneStatoAnagrafico As String = "08"
        Public Const VariazioneCiclo As String = "09"
        Public Const EliminazioneAppuntamento As String = "10"
        Public Const SpostamentoAppuntamento As String = "11"

    End Class

    Public Class TipoRispostaOsservazioneBilancio
        Public Const CodificataSingola As String = "C"
        Public Const CodificataMultipla As String = "M"
        Public Const TestoLibero As String = "V"
        Public Const NonPrevista As String = "N"
    End Class

    Public Class TipoDatiRispostaOsservazioneBilancio
        Public Const Testuale As String = "T"
        Public Const Numerica As String = "N"
    End Class

    Public Class OperazioneLogOsservazioniBilancio
        Public Const Eliminazione As String = "0"
        Public Const Inserimento As String = "1"
    End Class

    Public Class AmbitoOsservazioneReazioneAvversa
        Public Const NonOsservata As String = "0"
        Public Const FarmacovigilanzaAttiva As String = "1"
        Public Const RegistroFarmaci As String = "2"
        Public Const StudioOsservazionale As String = "3"
    End Class

    ''' <summary>
    ''' Stringhe di risorse definite nel manager
    ''' </summary>
    ''' <remarks></remarks>
    Public Class StringResourcesKey

        Public Const IntestazioneGrid_DataUslAssistenza As String = "IntestazioneGrid.DataUslAssistenza"
        Public Const IntestazioneGrid_UslProvenienza As String = "IntestazioneGrid.UslProvenienza"
        Public Const DatiPaziente_MantouxDataInvio As String = "DatiPaziente.MantouxDataInvio"
        Public Const StatConsulenze_CentroVaccinale As String = "Label.CentroVaccinale"
        Public Const StatConsulenze_TipoConsulenza As String = "StatConsulenze.TipoConsulenza"
        Public Const StatConsulenze_PazienteDataNascita As String = "Paziente.DataNascita"
        Public Const StatConsulenze_Operatore As String = "Label.Operatore"
        Public Const StatConsulenze_DataEsecuzione As String = "StatConsulenze.DataEsecuzione"
        Public Const Ricerca_AlertMaxNrRisultati As String = "Ricerca.AlertMaxNrRisultati"

    End Class
    ''' <summary>
    ''' Stringa per codificare i valori di ricerca dello stato di estrazione controllo 
    ''' </summary>
    Public Class DecodeStatoEstrazioneControllo
        Public Const InRegola As String = "S"
        Public Const ElaboratoConErrore As String = "E"
        Public Const NonInRegola As String = "N"
        Public Const ParzialmenteInRegola As String = "P"
    End Class

    ''' <summary>
    ''' Codici delle tipologie di note pazienti.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CodiceTipoNotaPaziente

        Public Const Solleciti As String = "SOLL"
        Public Const Appuntamenti As String = "APP"
        Public Const MalattiePregresse As String = "MAL"
        Public Const Esclusioni As String = "ESCL"
        Public Const Certificato As String = "CERT"
        Public Const Annotazioni As String = "NOTE"

    End Class

    Public Class StatoVaccinazioniEscluseEliminate
        Public Const Eliminata As String = "E" 'Caso generico, precedente al caso Rinnovata
        Public Const Rinnovata As String = "R" '20/12/2018 Nuovo, rinnovata da maschera paziente
    End Class

    ''' <summary>
    ''' Tipologie di documenti inviati all'FSE
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TipoDocumentoFSE
        Public Const CertificatoVaccinale As String = "CERTIFICATO_VACCINALE_FSE"
    End Class

    ''' <summary>
    ''' Tipologie di vaccinazioni in base alle logiche dell'FSE
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TipoVaccinazioneFSE
        Public Const Ciclo As String = "C"
        Public Const Richiamo As String = "R"
        Public Const VaccinazioneNormale As String = "N"
    End Class

    ''' <summary>
    ''' Funzionalità per la gestione delle notifiche dell'FSE
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FunzionalitaNotificaFSE
        Public Const VacProg_Salva As String = "VacProg.Salva"
        Public Const RegistrazioneVac_Salva As String = "RegistrazioneVac.Salva"
        Public Const VacEscluse_Salva As String = "VacEscluse.Salva"
        Public Const SalvaVaccinazioniACN As String = "SalvaVaccinazioniACN"
        Public Const CreazioneCDAFSE As String = "CreazioneCDAFSE"
        Public Const RichiestaCertificatoVaccinaleFSE As String = "RichiestaCertificatoVaccinaleFSE"
        Public Const Solleciti_InserisciVaccinazioniEscluse As String = "Solleciti.InserisciVaccinazioniEscluse"
        Public Const BatchElaborazioneVaccinazionePaziente As String = "BatchElaborazioneVaccinazionePaziente"
    End Class

    ''' <summary>
    ''' Eventi per la gestione delle notifiche dell'FSE
    ''' </summary>
    ''' <remarks></remarks>
    Public Class EventoNotificaFSE
        Public Const InserimentoVaccinazione As String = "InserimentoVaccinazione"
        Public Const RichiestaCertificatoVaccinale As String = "RichiestaCertificatoVaccinale"
    End Class

    Public Class CategorieCittadino
        Public Const AggiornamentoDaSOGEI As String = "90"
        Public Const Cessato As String = "91"
        Public Const Deceduto As String = "99"
    End Class

    Public Class MotiviCessazioneAssistenza
        Public Const Trasferimento As String = "01"
        Public Const Aire As String = "02"
        Public Const IrreperibilitaAnagrafica As String = "03"
        Public Const IrreperibilitaAlCensimento As String = "04"
        Public Const CancellazioneDUfficio As String = "05"
        Public Const OmessaDichiarazioneDiDimoraAbituale As String = "06"
        Public Const Decesso As String = "07"
        Public Const ResidenzaRespinta As String = "08"
    End Class

    Public Class TipoAutorizzazione

        ''' <summary>
        ''' tutti gli utenti = "S"
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ALL_USERS As String = "S"

        ''' <summary>
        ''' nessun utente = "N"
        ''' </summary>
        ''' <remarks></remarks>
        Public Const NONE As String = "N"

        ''' <summary>
        ''' solo super utenti = "A"
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ONLY_SUPERUSERS As String = "A"

    End Class

    Public Class StatoVaccinazioneEseguita

        ''' <summary>
        ''' Registrata = "R"
        ''' </summary>
        ''' <remarks></remarks>
        Public Const Registrata As String = "R"

        ''' <summary>
        ''' Importata = "I"
        ''' </summary>
        Public Const Importata As String = "I"

    End Class

End Namespace
