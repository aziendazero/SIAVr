Imports System.Collections.Generic

Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Biz
Imports Onit.OnAssistnet.OnVac.Log
Imports Onit.OnAssistnet.OnVac.Enumerators
Imports Onit.OnAssistnet.OnVac.CalcoloConvocazioni.ObjectModel


Public Class GestioneConvocazioni
    Implements IDisposable

#Region " Fields "

    Private Settings As Onit.OnAssistnet.OnVac.Settings.Settings
    Private ContextInfos As BizContextInfos

    Private DalCnv As DAL.IDALConvocazioni

    ' Indica se la connessione è gestita internamente o passata dall'esterno
    Private IsConnessioneInterna As Boolean = False

    ' Codice del consultorio di lavoro (su cui verranno effettuate le convocazioni)
    Private CodiceConsultorioCorrente As String

#End Region

#Region " Disposing "

    ' Dispose del dal solo se la connessione è stata creata internamente (in modo tale da poterla chiudere).
    Public Sub Dispose() Implements System.IDisposable.Dispose

        If Not Me.DalCnv Is Nothing Then Me.DalCnv.Dispose()

    End Sub

#End Region

#Region " Costruttori "

    Public Sub New(codiceConsultorio As String, contextInfos As BizContextInfos, provider As String, connectionString As String)

        Me.ContextInfos = contextInfos

        ' Istanza del dal creato in base al provider
        DalCnv = GetDALInstance(provider, connectionString, Nothing, Nothing)

        IsConnessioneInterna = True

        InizializzaGestioneConvocazioni(codiceConsultorio, String.Empty, DalCnv.Provider, DalCnv.Connection, DalCnv.Transaction)

    End Sub

    Public Sub New(codiceConsultorio As String, contextInfos As BizContextInfos, provider As String, ByRef conn As IDbConnection, ByRef tx As IDbTransaction)

        Me.ContextInfos = contextInfos

        ' Istanza del dal creato in base al provider
        DalCnv = GetDALInstance(provider, String.Empty, conn, tx)

        IsConnessioneInterna = False

        InizializzaGestioneConvocazioni(codiceConsultorio, String.Empty, provider, conn, tx)

    End Sub

    Public Sub New(codiceConsultorioLavoro As String, codiceConsultorioSettings As String, contextInfos As BizContextInfos, provider As String, ByRef conn As IDbConnection, ByRef tx As IDbTransaction)

        Me.ContextInfos = contextInfos

        ' Istanza del dal creato in base al provider
        DalCnv = GetDALInstance(provider, String.Empty, conn, tx)

        IsConnessioneInterna = False

        InizializzaGestioneConvocazioni(codiceConsultorioLavoro, codiceConsultorioSettings, provider, conn, tx)

    End Sub

    Private Sub InizializzaGestioneConvocazioni(codiceConsultorioLavoro As String, codiceConsultorioSettings As String, provider As String, ByRef conn As IDbConnection, ByRef tx As IDbTransaction)

        CodiceConsultorioCorrente = codiceConsultorioLavoro

        ' Caricamento parametri del consultorio specificato => codiceConsultorioSettings se specificato, altrimenti codiceConsultorioLavoro
        Dim cnsSettings As String = If(String.IsNullOrWhiteSpace(codiceConsultorioSettings), codiceConsultorioLavoro, codiceConsultorioSettings)

        Using genericProvider As New DbGenericProvider(provider, conn, tx)

            If cnsSettings = Constants.CommonConstants.CodiceConsultorioSistema Then
                Settings = New Settings.Settings(genericProvider)
            Else
                Settings = New Settings.Settings(cnsSettings, genericProvider)
            End If

        End Using

    End Sub

    Private Function GetDALInstance(provider As String, connectionString As String, ByRef conn As IDbConnection, ByRef tx As IDbTransaction) As DAL.IDALConvocazioni

        Dim asm As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly()

        ' Per caricamento .dll esterna
        'If strAssembly Is Nothing OrElse strAssembly = String.Empty Then
        'Else
        '    If Not strPath Is Nothing AndAlso strPath <> String.Empty Then
        '        asm = System.Reflection.Assembly.LoadFile(strPath)
        '    Else
        '        asm = System.Reflection.Assembly.Load(strAssembly)
        '    End If
        'End If

        Dim strClass As String = String.Empty

        Select Case provider.ToUpper()
            Case "ORACLE"
                strClass = "Onit.OnAssistnet.OnVac.CalcoloConvocazioni.DAL.Oracle.OracleDALConvocazioni"
            Case "SQLSERVER"
                strClass = "Onit.OnAssistnet.OnVac.CalcoloConvocazioni.DAL.Sql.SqlDALConvocazioni"
        End Select

        ' Richiama il costruttore del dal in base ai parametri passati
        Dim dalConvocazioni As DAL.IDALConvocazioni = Nothing

        If conn Is Nothing Then
            dalConvocazioni = DirectCast(asm.CreateInstance(strClass, True, Nothing, Nothing, New Object() {provider, connectionString}, Nothing, Nothing), DAL.IDALConvocazioni)
        Else
            dalConvocazioni = DirectCast(asm.CreateInstance(strClass, True, Nothing, Nothing, New Object() {provider, conn, tx}, Nothing, Nothing), DAL.IDALConvocazioni)
        End If

        Return dalConvocazioni

    End Function

#End Region

#Region " Properties "

    Private _ConvocazioniScartate As List(Of BizConvocazione.DatiConvocazioneScartata)

    ''' <summary>
    ''' Insieme di tutte le convocazioni scartate dai metodi di gestione delle convocazioni
    ''' </summary>
    Public ReadOnly Property ConvocazioniScartate() As List(Of BizConvocazione.DatiConvocazioneScartata)
        Get
            If _ConvocazioniScartate Is Nothing Then
                _ConvocazioniScartate = New List(Of BizConvocazione.DatiConvocazioneScartata)
            End If
            Return _ConvocazioniScartate
        End Get
    End Property

#End Region

#Region " Metodi pubblici per la gestione della transazione "

    Public Sub BeginTransaction()

        ' Richiama la beginTransaction del dal, che effettua l'operazione di apertura vera e propria della transazione
        Me.DalCnv.BeginTransaction()

    End Sub

    Public Sub CommitTransaction()

        Me.DalCnv.Transaction.Commit()

    End Sub

    Public Sub RollbackTransaction()

        Me.DalCnv.Transaction.Rollback()

    End Sub

#End Region

#Region " Ricerca pazienti da convocare "

    ''' <summary>
    ''' Cerca i pazienti da convocare per il consultorio, in base ai filtri specificati.
    ''' Restituisce un arraylist con i codici dei pazienti trovati.
    ''' </summary>
    ''' <param name="statiAnagrafici"></param>
    ''' <param name="dataNascitaDa"></param>
    ''' <param name="dataNascitaA"></param>
    ''' <param name="sesso"></param>
    ''' <param name="codiceMalattia"></param>
    ''' <param name="codiceCategoriaRischio"></param>
    Public Function CercaPazientiDaConvocare(statiAnagrafici As String(), dataNascitaDa As Date, dataNascitaA As Date, sesso As String, codiceMalattia As String, codiceCategoriaRischio As String) As ArrayList

        Dim listCodiciPazienti As ArrayList

        ' Se questo parametro è true la query per determinare i pazienti da convocare
        ' deve filtrare per il periodo di nascita (se le date sono valorizzate).
        ' Altrimenti, anche se le date sono valorizzate, non deve fare nessun filtro.
        If Not Me.Settings.CNVAUTOFILTRAETA Then
            dataNascitaDa = Date.MinValue
            dataNascitaA = Date.MinValue
        End If

        Try
            listCodiciPazienti = Me.DalCnv.GetCodiciPazientiDaConvocare(Me.CodiceConsultorioCorrente, statiAnagrafici,
                                                                dataNascitaDa, dataNascitaA, Me.Settings.TUTTECNV,
                                                                sesso, codiceMalattia, codiceCategoriaRischio)
        Catch ex As Exception
            Throw New ConvocazioniException("Errore Caricamento Pazienti Convocazioni Automatiche.", ex)
        End Try

        Return listCodiciPazienti

    End Function

#End Region

#Region " Convocazioni Scartate "

    ' Aggiunta alla collection delle convocazioni scartate dalla procedura
    Private Sub AddToConvocazioniScartate(codicePaziente As Integer, oldDataConvocazione As DateTime, newDataConvocazione As DateTime, codiceMotivoScarto As BizConvocazione.CodiceMotivoConvocazioneScartata)
        Me.ConvocazioniScartate.Add(BizConvocazione.CreateConvocazioneScartata(codicePaziente, oldDataConvocazione, newDataConvocazione, codiceMotivoScarto))
    End Sub

    ' Aggiunta alla collection delle convocazioni scartate dalla procedura
    Private Sub AddToConvocazioniScartate(codicePaziente As Integer, oldDataConvocazione As DateTime, newDataConvocazione As DateTime, codiceCiclo As String, numeroSeduta As Integer, intervallo As Integer, codiceMotivoScarto As BizConvocazione.CodiceMotivoConvocazioneScartata, codiceVaccinazioniList As List(Of String))
        Me.ConvocazioniScartate.Add(BizConvocazione.CreateConvocazioneScartata(codicePaziente, oldDataConvocazione, newDataConvocazione, codiceCiclo, numeroSeduta, intervallo, codiceMotivoScarto, codiceVaccinazioniList))
    End Sub

    ' Aggiunta alla collection delle convocazioni scartate dalla procedura
    Private Sub AddToConvocazioniScartate(codicePaziente As Integer, dataConvocazione As DateTime, codiceCiclo As String, numeroSeduta As Integer, intervallo As Integer, codiceMotivoScarto As BizConvocazione.CodiceMotivoConvocazioneScartata, codiceVaccinazioniList As List(Of String))
        Me.AddToConvocazioniScartate(codicePaziente, dataConvocazione, Nothing, codiceCiclo, numeroSeduta, intervallo, codiceMotivoScarto, codiceVaccinazioniList)
    End Sub

#End Region

#Region " Algoritmo di calcolo delle convocazioni "

    ''' <summary>
    ''' Restituisce il datatable che conterrà tutte le convocazioni calcolate, ancora da programmare.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="userId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function BuildDtConvocazioni(codicePaziente As Integer, userId As Integer) As DataTable

        ' Datatable che conterrà tutte le convocazioni calcolate, ancora da programmare
        Dim dta As New DataTable()

        ' Pulizia della struttura contenente le convocazioni scartate dal calcolo
        Me.ConvocazioniScartate.Clear()

        ' Provider per utilizzare i metodi dell'OnVac.DAL
        Using genericProvider As New Onit.OnAssistnet.OnVac.DAL.DbGenericProvider(Me.DalCnv.Provider, Me.DalCnv.Connection, Me.DalCnv.Transaction)


            ' --------- Ricerca dati paziente --------- '
            ' Ricerca i dati del paziente utilizzati nell'algoritmo, impostandoli in un oggetto DatiPazienteClass
            Dim datiPazienteCorrente As ObjectModel.DatiPazienteClass = Me.DalCnv.GetDatiPaziente(codicePaziente)
            ' ----------------------------------------- '


            ' --------- Ricerca convocazioni --------- '
            ' Effettua la query per la ricerca delle convocazioni da programmare
            Dim dtaProg As New DataTable()
            Try
                dta = Me.DalCnv.GetConvocazioniDaProgrammare(codicePaziente)
                dtaProg = Me.DalCnv.GetConvocazioniProgrammate(codicePaziente)
            Catch ex As Exception
                Throw New ConvocazioniException("Errore recupero dati delle convocazioni da programmare o programmate.", codicePaziente, ex)
            End Try

            ' DMI Creo una chiave su CICLO-SEDUTA-VACCINAZIONE
            dta.PrimaryKey = New DataColumn() {dta.Columns("CIC_CODICE"), dta.Columns("SED_N_SEDUTA"), dta.Columns("SED_VAC_CODICE")}
            ' ---------------------------------------- '


            ' --------- Controllo Vaccinazioni Sostitute --------- '
            'se la vaccinazione considerata ha il campo VAC_COD_SOSTITUTA
            'valorizzato, significa che deve continuare con le sedute della
            'vaccinazione specificata nel suddetto campo, ovvero:
            '--> se per la VAC_COD_SOSTITUTA è presente l'inadempienza oppu-
            '    re è stata esclusa, deve eliminare anche la vaccinazione
            '    calcolata nella convocazione
            '--> se esistono vaccinazioni eseguite per la VAC_COD_SOSTITUTA
            '    deve proporre la vaccinazione della convocazione con la se-
            '    duta successiva a quella della sostituta
            '
            ControllaVaccinazioniSostitute(codicePaziente, dta, dtaProg)
            ' ---------------------------------------------------- '


            ' --------- Controllo presenza vaccinazioni obbligatorie --------- '
            If Me.Settings.STOPCNV_NONOBBL Then

                ' Se non ci sono vaccinazioni obbligatorie, elimino tutte le convocazioni da programmare.
                Dim _rows As DataRow() = dta.Select("VAC_OBBLIGATORIA = 'A'")

                If _rows.Length = 0 Then
                    dta.Rows.Clear()
                    dta.AcceptChanges()
                End If

            End If
            ' ---------------------------------------------------------------- '


            ' ****************** Operazioni effettuate SOLO se sono rimaste vaccinazioni obbligatorie ****************** '
            '
            If dta.Rows.Count > 0 Then

                ' --------- Ciclo per Spostamento Età Seduta e Calcolo Data Convocazione --------- '
                Dim dtaNuovaSeduta As New DataTable()
                dtaNuovaSeduta = dta.Clone()

                Dim oldCic As String = String.Empty
                Dim oldSed As Int16 = -1

                Dim row As DataRow

                Dim dataScadenzaEsclusione As Date
                Dim giorniEtaSeduta As Integer

                For iSed As Int16 = 0 To dta.Rows.Count - 1

                    ' --- Spostamento età seduta --- '
                    If Not dta.Rows(iSed)("MYDATA") Is DBNull.Value Then

                        dataScadenzaEsclusione = dta.Rows(iSed)("MYDATA")

                        giorniEtaSeduta = 0
                        If Not dta.Rows(iSed)("TSD_ETA_SEDUTA") Is DBNull.Value Then
                            giorniEtaSeduta = CInt(dta.Rows(iSed)("TSD_ETA_SEDUTA"))
                        End If
                        '
                        If giorniEtaSeduta <= dataScadenzaEsclusione.Subtract(datiPazienteCorrente.DataNascita).Days Then
                            '
                            ' ???
                            ' Fuori dall'if, per ogni riga, il valore di Eta_Seduta è già moltiplicato per il parametro AGGGIORNI,
                            ' quindi non serve moltiplicarlo anche qui. Però, quando ricalcola l'età seduta in base alla data 
                            ' "MyData" (che sarebbe quella di scadenza esclusione), riporta i giorni in base 360 (anno da 360 giorni). 
                            ' Penso che faccia così perchè le età sono tutte espresse in questo modo, e con il parametro AGGGIORNI le 
                            ' aggiusta, ma non ne sono sicuro. Comunque, se fa così, non è preciso perchè se fuori le moltiplica,
                            ' qui le deve dividere per AGGGIORNI.
                            dta.Rows(iSed)("TSD_ETA_SEDUTA") = Math.Round(dataScadenzaEsclusione.Subtract(datiPazienteCorrente.DataNascita).Days / 365 * 360)
                            ' ???
                            '
                        End If

                    End If
                    ' ------------------------------ '


                    ' --- Calcolo Data Convocazione --- '
                    dta.Rows(iSed)("TSD_ETA_SEDUTA") = Math.Round(dta.Rows(iSed)("TSD_ETA_SEDUTA") * Me.Settings.AGGGIORNI)
                    row = dtaNuovaSeduta.NewRow()

                    If dta.Rows(iSed)("CIC_CODICE") = oldCic Then

                        If dta.Rows(iSed)("SED_N_SEDUTA") <> oldSed Then

                            If EsisteSedutaPrecedenteStessoCiclo(dta, dta.Rows(iSed)("CIC_CODICE"), dta.Rows(iSed)("SED_N_SEDUTA"), dta.Rows(iSed)("SED_VAC_CODICE")) Then
                                'Elimino le sedute che hanno già una precedente stesso ciclo calcolata
                                dta.Rows(iSed).Delete()
                            Else
                                ' ??? Cosa vuol dire questo commento ???
                                ' CONTROLLO POSSO ACCORPARE LA SEDUTA CORRENTE ALLA SEDUTA PRECEDENTE (EVIDENTEMENTE INCOMPLETA) DELLO STESSO CICLO
                                CalcolaDataConvocazione(dta.Rows(iSed), codicePaziente, datiPazienteCorrente.DataNascita)
                            End If

                        Else
                            dta.Rows(iSed)("CONVOCAZIONE") = dta.Rows(iSed - 1)("CONVOCAZIONE")
                            dta.Rows(iSed)("TSD_ETA_SEDUTA") = dta.Rows(iSed - 1)("TSD_ETA_SEDUTA")
                            dta.Rows(iSed)("TSD_INTERVALLO") = dta.Rows(iSed - 1)("TSD_INTERVALLO")
                        End If

                    Else
                        oldCic = dta.Rows(iSed)("CIC_CODICE")
                        oldSed = dta.Rows(iSed)("SED_N_SEDUTA")
                        CalcolaDataConvocazione(dta.Rows(iSed), codicePaziente, datiPazienteCorrente.DataNascita)
                    End If
                    ' --------------------------------- '

                Next
                dta.AcceptChanges()
                ' -------------------------------------------------------------------------------- '


                ' --------- Creazione collection di convocazioni con i dati originali (pre-accorpamenti e calcoli vari) --------- '

                ' Oggetto utilizzato per effettuare i controlli di associabilità.
                ' L'oggetto sarà istanziato e utilizzato solo se il parametro che abilita il controllo di associabilità è true.
                Dim ctrlAssociabilita As Associabilita.ControlloAssociabilita = Nothing

                ' Questa collection conterrà i dati delle convocazioni che sono state calcolate, con le date di convocazione
                ' originali, prima che vengano modificate dagli accorpamenti. Verrà utilizzata dal controllo di associabilità
                ' per ripristinare la data originale, se il controllo fallisce.
                Dim cnvDaProgrammareOriginali As Associabilita.ObjectModel.ConvocazioniCollection = Nothing

                If Me.Settings.CTRL_ASSOCIABILITA_VAC Then

                    ctrlAssociabilita = New Associabilita.ControlloAssociabilita(Me.DalCnv.Provider, Me.DalCnv.Connection, Me.DalCnv.Transaction)
                    cnvDaProgrammareOriginali = ctrlAssociabilita.CreaCollectionConvocazioni(dta)

                End If
                ' --------------------------------------------------------------------------------------------------------------- '


                ' --------- Accorpamento con convocazioni già esistenti --------- '
                ' L'accorpamento avviene in base al valore del campo Intervallo.
                ' Se la data di convocazione della vaccinazione da programmare è all'interno dell'intervallo,
                ' accorpo la vaccinazione da programmare.

                ' Arraylist contenente tutte le date di convocazione per vacc obbligatorie presenti su db, ordinate in modo crescente.
                Dim listDataConvocazione As ArrayList = Nothing

                Try
                    listDataConvocazione = Me.DalCnv.GetDateConvocazioneVaccinazioniObbligatorie(codicePaziente)
                Catch ex As Exception
                    Throw New ConvocazioniException("Errore recupero date di convocazione per le vaccinazioni obbligatorie.", codicePaziente, ex)
                End Try

                ' Controllo sull'intervallo di validità
                If Not listDataConvocazione Is Nothing AndAlso listDataConvocazione.Count > 0 Then

                    Dim d, cnv As Date
                    Dim rowDTA As DataRow = Nothing

                    For idxRow As Integer = 0 To dta.Rows.Count - 1

                        rowDTA = dta.Rows(idxRow)

                        For i As Integer = 0 To listDataConvocazione.Count - 1

                            d = Convert.ToDateTime(listDataConvocazione(i)).Date
                            cnv = Convert.ToDateTime(rowDTA("CONVOCAZIONE")).Date

                            If cnv.AddDays(rowDTA("TSD_INTERVALLO")) >= d And cnv <= d Then
                                rowDTA("CONVOCAZIONE") = d
                                Exit For
                            End If

                        Next

                    Next

                End If

                dta.AcceptChanges()
                ' --------------------------------------------------------------- '


                ' --- Ordinamento DefaultView --- '
                dta.DefaultView.Sort = "CONVOCAZIONE, CIC_CODICE"
                For iSed As Int16 = 0 To dta.DefaultView.Count - 1
                    dta.DefaultView(iSed)("ORDINE") = iSed
                Next
                dta.DefaultView.Sort = "ORDINE"

                ' Da ora in poi si usa il defaultview (che è ordinato)

                Dim etaSeduta, intervallo As Integer
                Dim dataConvocazione As Date

                If dta.DefaultView.Count > 0 Then
                    oldCic = String.Empty
                    etaSeduta = dta.DefaultView(0)("TSD_ETA_SEDUTA")
                    intervallo = dta.DefaultView(0)("TSD_INTERVALLO")
                    dataConvocazione = dta.DefaultView(0)("CONVOCAZIONE")
                    oldSed = -1
                End If


                ' --------- Calcolo valori per accorpamento --------- '
                '
                Dim vacObbligatoria As Boolean = False

                ' Recupero la data di sospensione (se c'è, altrimenti vale Date.MinValue)
                Dim dataSospensione As Date = Date.MinValue
                Try
                    dataSospensione = Me.DalCnv.GetMaxDataFineSospensionePaziente(codicePaziente)
                Catch ex As Exception
                    dataSospensione = Date.MinValue
                End Try

                ' Considero solo il componente data e non l'ora
                dataSospensione = dataSospensione.Date

                For iSed As Int16 = 0 To dta.DefaultView.Count - 1

                    If oldCic <> dta.DefaultView(iSed)("CIC_CODICE") Then

                        ' Se incontro un ciclo diverso dal precedente, confronto i valori per l'accorpamento. 
                        ' Il dataview è ordinato per data e ciclo, quindi i cicli sono in ordine.

                        oldCic = dta.DefaultView(iSed)("CIC_CODICE").ToString

                        If etaSeduta + intervallo >= dta.DefaultView(iSed)("TSD_ETA_SEDUTA") Then

                            intervallo = intervallo + etaSeduta - dta.DefaultView(iSed)("TSD_ETA_SEDUTA")

                            If dta.DefaultView(iSed)("TSD_INTERVALLO") < intervallo Then
                                intervallo = dta.DefaultView(iSed)("TSD_INTERVALLO")
                            End If

                            etaSeduta = dta.DefaultView(iSed)("TSD_ETA_SEDUTA")

                            If datiPazienteCorrente.DataNascita.AddDays(dta.DefaultView(iSed)("TSD_ETA_SEDUTA")) >= dataConvocazione Then
                                dataConvocazione = datiPazienteCorrente.DataNascita.AddDays(dta.DefaultView(iSed)("TSD_ETA_SEDUTA"))
                            End If

                            ' Sposto la data di convocazione dopo la fine della sospensione (se cnv precedente sosp).
                            If dataConvocazione <= dataSospensione Then dataConvocazione = dataSospensione.AddDays(1)
                            ' Se, comunque, la data di convocazione è per una data inferiore ad oggi, la imposto a oggi.
                            If dataConvocazione < Date.Now.Date Then dataConvocazione = Date.Now.Date

                        Else

                            ' Se è stata calcolata una convocazione da programmare contenente una vaccinazione obbligatoria,
                            ' questo flag vale true. Serve per eliminare tutte le altre convocazioni da programmare.
                            ' Cioè: dopo aver programmato la prima vaccinazione obbligatoria, non ne calcola altre.
                            If vacObbligatoria Then
                                ' Per non creare convocazioni in più quando ha già creato una convocazione con vaccinazione obbligatoria
                                For jSed As Int16 = dta.DefaultView.Count - 1 To iSed Step -1
                                    dta.DefaultView(jSed).Delete()
                                Next

                                Exit For

                            Else

                                ' Se l'algoritmo non ha ancora programmato vaccinazioni obbligatorie, continua con il calcolo.
                                ' Determina i nuovi valori per l'accorpamento. L'accorpamento avverrà solo da oldSed in poi, 
                                ' escludendo la convocazione precedente, che se ne sta per conto suo.

                                intervallo = dta.DefaultView(iSed)("TSD_INTERVALLO")
                                dataConvocazione = dta.DefaultView(iSed)("CONVOCAZIONE")
                                etaSeduta = dta.DefaultView(iSed)("TSD_ETA_SEDUTA")
                                oldSed = iSed - 1

                            End If

                        End If

                    End If

                    ' Se il parametro vale true, alla prima vaccinazione obbligatoria si ferma. Altrimenti calcola tutte le convocazioni.
                    If Settings.CALCOLOCNV_STOP_PRIMA_OBBLIGATORIA AndAlso Not vacObbligatoria AndAlso
                       dta.DefaultView(iSed)("VAC_OBBLIGATORIA").ToString() = Constants.ObbligatorietaVaccinazione.Obbligatoria Then

                        vacObbligatoria = True

                    End If

                Next

                dta.AcceptChanges()
                ' --------------------------------------------------- '


                ' --------- Accorpamento delle convocazioni calcolate --------- '
                ' Se l'età della seduta lo permette, le convocazioni vengono accorpate.
                ' N.B. : fino alla versione 4.0 il parametro GES_CICLI_COMPLESSI indicava se basarsi sull'età 
                '        oppure accorpare in ogni caso. Dalla versione 4.1 questo parametro è stato eliminato 
                '        e le sedute si accorpano solo se l'età lo consente.
                For jSed As Int16 = oldSed + 1 To dta.DefaultView.Count - 1

                    If datiPazienteCorrente.DataNascita.AddDays(dta.DefaultView(jSed)("TSD_ETA_SEDUTA")) <= dataConvocazione Then

                        AccorpaDatiSeduta(dta.DefaultView(jSed), intervallo, etaSeduta, dataConvocazione)

                    End If

                Next

                dta.AcceptChanges()
                ' ------------------------------------------------------------- '


                ' --- Controllo fine sospensione --- '
                ' Controllo data cnv: se sono precedenti rispetto alla data di sospensione, devono essere spostate
                ' C'erano casi in cui alcune vac non venivano accorpate e restavano in una data precedente la fine sospensione.
                For jSed As Int16 = 0 To dta.DefaultView.Count - 1

                    If (dta.DefaultView(jSed)("CONVOCAZIONE") <= dataSospensione) Then
                        dta.DefaultView(jSed)("CONVOCAZIONE") = dataSospensione.AddDays(1)
                    End If

                    If (dta.DefaultView(jSed)("CONVOCAZIONE") < Date.Now.Date) Then
                        dta.DefaultView(jSed)("CONVOCAZIONE") = Date.Now.Date
                    End If

                Next
                ' ---------------------------------- '


                ' --------- Calcolo durata seduta maggiore --------- '
                ' Assegna a tutte le sedute appartenenti alla stessa convocazione, la durata maggiore tra esse.
                Dim oldDataConv As Object = Nothing
                Dim durataSeduta As Integer

                For iSed As Int16 = 0 To dta.DefaultView.Count - 1

                    If oldDataConv = dta.DefaultView(iSed)("CONVOCAZIONE") Then

                        ' Se sono nella stessa data, controllo la durata della seduta
                        If durataSeduta < dta.DefaultView(iSed)("TSD_DURATA_SEDUTA") Then

                            ' Se la durata della seduta corrente è superiore alla durata calcolata fino a questo punto (per la data corrente)
                            ' imposto la nuova durata per tutte le sedute della data corrente, dalla prima fino al punto in cui sono arrivato
                            durataSeduta = dta.DefaultView(iSed)("TSD_DURATA_SEDUTA")
                            For jSed As Int16 = oldSed + 1 To iSed - 1
                                dta.DefaultView.Item(jSed)("TSD_DURATA_SEDUTA") = durataSeduta
                            Next

                        Else

                            ' Se la durata della seduta corrente è inferiore, la imposto uguale a quella maggiore.
                            dta.DefaultView(iSed)("TSD_DURATA_SEDUTA") = durataSeduta

                        End If

                    Else

                        ' Nel momento in cui si incontra una seduta di una nuova data, imposto la nuova durata, 
                        ' il nuovo valore di oldSed (che indica l'inizio delle sedute appartenenti alla data corrente) 
                        ' e la nuova data di convocazione su cui controllare la durata.
                        oldSed = iSed - 1
                        oldDataConv = dta.DefaultView(iSed)("CONVOCAZIONE")
                        durataSeduta = dta.DefaultView(iSed)("TSD_DURATA_SEDUTA")

                    End If

                Next
                ' -------------------------------------------------- '


                ' --------- Controllo Accorpabilità Vaccini --------- '
                ' Il controllo di accorpabilità dovrebbe essere effettuato ad ogni tentativo di associare convocazioni. 
                ' Se fallisce, non accorpo. Per come è fatto l'algoritmo, è più difficile e più pesante aggiungerlo 
                ' ad ogni passaggio, è meglio metterlo qui, alla fine di tutti gli spostamenti di convocazioni.
                ' Per questo, all'inizio, è stata salvata la data originale di ogni convocazione calcolata: per poter ripristinare
                ' la situazione iniziale se la convocazione non può essere accorpata ad un'altra nella stessa data.
                ' ??? DA FARE: se si riscriverà da capo l'algoritmo, utilizzando strutture ad hoc anzichè un datatable, allora
                ' si potrà effettuare il controllo di accorpabilità in maniera differente.

                If Me.Settings.CTRL_ASSOCIABILITA_VAC Then

                    ' Collection di convocazioni da programmare, con i dati attuali (data di convocazione modificata dagli accorpamenti)
                    Dim cnvDaProgrammareCorrenti As Associabilita.ObjectModel.ConvocazioniCollection =
                        ctrlAssociabilita.CreaCollectionConvocazioni(dta)

                    ' Lettura delle convocazioni già programmate. La lettura restituisce un datatable, che viene convertito in una collection.
                    Dim dtTemp As DataTable = Me.DalCnv.GetConvocazioniProgrammate(codicePaziente)

                    Dim cnvProgrammate As Associabilita.ObjectModel.ConvocazioniCollection =
                        ctrlAssociabilita.CreaCollectionConvocazioni(dtTemp)

                    dtTemp.Dispose()

                    ' Collection delle convocazioni che si possono accorpare. 
                    ' Inizialmente conterrà le convocazioni già programmate (se ci sono), lette da db.
                    Dim cnvAccorpate As New Associabilita.ObjectModel.ConvocazioniAccorpateCollection
                    For i As Integer = 0 To cnvProgrammate.Count - 1
                        AccorpaCnv(cnvProgrammate(i), cnvAccorpate)
                    Next

                    ' Ciclo di controllo accorpabilità
                    ' Faccio partire il ciclo dall'ultima convocazione e vado all'indietro perchè, essendo ordinate per data,
                    ' parto a valutare le cnv dalla più lontana. Se due cnv vengono accorpate, la data di accorpamento 
                    ' è sempre quella della più lontana. Perciò, in caso di non accorpabilità lascio la convocazione con data più lontana 
                    ' nella sua data, mentre riporto la più recente alla sua data originale e calcola la data di spostamento a partire da questa. 
                    ' Questo comporta che, nel caso in cui esca dall'intervallo di validità, viene scartata la più recente.

                    Dim cnvDaAccorpare As Associabilita.ObjectModel.ConvocazioniClass       ' convocazione che deve essere accorpata
                    Dim dataAccorpamento As Date                                            ' data in cui avverrà l'accorpamento
                    Dim cnvAcc As Associabilita.ObjectModel.ConvocazioniAccorpateClass      ' collection di convocazioni già accorpate nella stessa data

                    For idxDaProg As Integer = cnvDaProgrammareCorrenti.Count - 1 To 0 Step -1

                        cnvDaAccorpare = cnvDaProgrammareCorrenti(idxDaProg)
                        dataAccorpamento = cnvDaAccorpare.DataConvocazione

                        ' Cerco se ci sono già convocazioni nella data specificata
                        cnvAcc = cnvAccorpate.FindByDataConvocazione(dataAccorpamento)

                        If cnvAcc Is Nothing Then

                            ' Se non ci sono convocazioni in questa data, aggiungo un elemento alle cnvAccorpate e passo alla cnv successiva
                            ' Non devo aggiornare dta perchè la data di convocazione non è stata modificata.
                            cnvAcc = New Associabilita.ObjectModel.ConvocazioniAccorpateClass()
                            cnvAcc.DataConvocazione = dataAccorpamento
                            cnvAcc.ElencoConvocazioni.Add(cnvDaAccorpare)
                            cnvAccorpate.Add(cnvAcc)

                        Else

                            ' Se ci sono altre convocazioni in questa data, devo controllare l'accorpabilità della convocazione da programmare
                            ' con tutte quelle già accorpate in questa data.
                            If IsCnvAccorpabile(cnvDaAccorpare, cnvAcc.ElencoConvocazioni, ctrlAssociabilita) Then

                                ' Se è accorpabile, la aggiungo all'elenco. Anche in questo caso dta non deve essere aggiornato.
                                cnvAcc.ElencoConvocazioni.Add(cnvDaAccorpare)

                            Else

                                ' Tento di spostare la convocazione in una data successiva.
                                dataAccorpamento = GetDataSpostamento(cnvDaProgrammareOriginali, cnvDaAccorpare, cnvAccorpate, dataSospensione, ctrlAssociabilita)

                                If dataAccorpamento = Date.MinValue Then
                                    ' Non ho trovato nessuna data in cui la convocazione è possibile: non posso programmare la convocazione.

                                    ' 1 - LOG NELLA STRUTTURA CHE MANTIENE I DATI DELLE CNV NON PROGRAMMATE
                                    Dim cnvDaProgOriginale As Associabilita.ObjectModel.ConvocazioniClass =
                                        cnvDaProgrammareOriginali.FindByCodCnvSeduta(cnvDaAccorpare.CodiceConvocazione, cnvDaAccorpare.NumeroSeduta)

                                    AddToConvocazioniScartate(codicePaziente,
                                                              cnvDaProgOriginale.DataConvocazione,
                                                              cnvDaProgOriginale.CodiceConvocazione,
                                                              cnvDaProgOriginale.NumeroSeduta,
                                                              cnvDaProgOriginale.Intervallo,
                                                              BizConvocazione.CodiceMotivoConvocazioneScartata.NonAssociabilitaIntervallo,
                                                              New List(Of String)(cnvDaProgOriginale.ElencoVaccinazioni.ToArray(GetType(String))))

                                    ' 2 - ELIMINAZIONE CNV
                                    EliminaCnv(dta, cnvDaAccorpare)
                                Else
                                    ' Accorpamento convocazione
                                    cnvDaAccorpare.DataConvocazione = dataAccorpamento
                                    AccorpaCnv(cnvDaAccorpare, cnvAccorpate)

                                    ' Aggiornamento dta
                                    AccorpaDataCnv(dta, dataAccorpamento, cnvDaAccorpare)

                                End If

                            End If

                        End If

                    Next

                    dta.AcceptChanges() ' eventuali righe eliminate generano errore se rimangono in stato Deleted

                End If
                ' --------------------------------------------------- '

            End If  ' dta.Rows.Count > 0

        End Using

        Return dta

    End Function

    ''' <summary>
    ''' Esegue il calcolo della convocazione automatico, basandosi sui cicli assegnati al paziente ed escludendo
    ''' le vaccinazioni eseguite, escluse, inadempiute
    ''' Associa anche il bilancio alla convocazione creata ed eventualmente crea una convocaziona apposita
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="userId"></param>
    Public Sub CalcolaConvocazioni(codicePaziente As Integer, userId As Integer)

        ' Pulizia della struttura contenente le convocazioni scartate dal calcolo
        Me.ConvocazioniScartate.Clear()

        If Me.IsConnessioneInterna Then Me.DalCnv.BeginTransaction()

        ' Provider per utilizzare i metodi dell'OnVac.DAL
        Using genericProvider As New Onit.OnAssistnet.OnVac.DAL.DbGenericProvider(Me.DalCnv.Provider, Me.DalCnv.Connection, Me.DalCnv.Transaction)

            Try
                ' --------- Ricerca dati paziente --------- '
                ' Ricerca i dati del paziente utilizzati nell'algoritmo, impostandoli in un oggetto DatiPazienteClass
                Dim datiPazienteCorrente As ObjectModel.DatiPazienteClass = Me.DalCnv.GetDatiPaziente(codicePaziente)

                Dim dta As DataTable = Me.BuildDtConvocazioni(codicePaziente, userId)

                Dim now As DateTime = DateTime.Now

                Using bizConvocazioni As New BizConvocazione(genericProvider, Me.Settings, Me.ContextInfos, Nothing)

                    ' --------- Gestione dei Bilanci di Salute --------- '
                    If Me.Settings.GESBIL Then

                        Using bizBilancioProgrammato As New BizBilancioProgrammato(genericProvider, Me.Settings, Me.ContextInfos, Nothing)

                            Dim bilancioProgrammatoCollection As Collection.BilancioProgrammatoCollection = Nothing

                            ' TODO [storicoAppunt]: Creazione CNV + bilanci => DA TESTARE!

                            ' --- Controllo bilanci scaduti --- '
                            ' Se ci sono bilanci scaduti, li imposta a US. Le cnv che contengono solo bilanci impostati a US vengono eliminate 
                            bizBilancioProgrammato.ControlloBilanciNonEseguitiScaduti(codicePaziente)

                            ' --- Controllo bilanci da sollecitare --- '
                            If Me.Settings.GESSOLLECITIBILANCI Then
                                Using bizSollecitiBilanci As New Biz.BizSollecitiBilanci(genericProvider, Me.Settings, Me.ContextInfos)

                                    bizSollecitiBilanci.ControlloBilanciDaSollecitare(codicePaziente)

                                End Using
                            End If

                            ' --- Calcolo dei bilanci da programmare --- '
                            bilancioProgrammatoCollection = bizBilancioProgrammato.BilanciDaProgrammare(dta, codicePaziente, datiPazienteCorrente.Eta, datiPazienteCorrente.DataNascita)

                            ' --- Creazione delle convocazioni relative ai bilanci --- '
                            bizConvocazioni.CreaConvocazioni(bilancioProgrammatoCollection, codicePaziente, now, userId)

                            ' --- Inserimento bilanci programmati --- '
                            bizBilancioProgrammato.ProgrammaBilanci(bilancioProgrammatoCollection, codicePaziente)

                        End Using

                    End If

                    ' Se ci sono sedute, inserisco le cnv
                    If dta.Rows.Count > 0 Then

                        ' --------- Inserimento Convocazioni --------- '
                        ' Inserisco/aggiorno le nuove convocazioni relative a bilanci e a vaccinazioni
                        bizConvocazioni.CreaConvocazioni(dta, codicePaziente, now, userId)

                    End If

                    ' Se ci sono sedute, inserisco i cicli e le vac prog
                    If dta.Rows.Count > 0 Then

                        ' --------- Inserimento Cicli Vaccinali e Inserimento Vaccinazioni Programmate --------- '
                        Dim hCicliSedute As New Dictionary(Of String, Integer())
                        Dim cnvHash As New Hashtable()

                        For iSed As Int16 = 0 To dta.Rows.Count - 1

                            Dim insertCicloConvocazione As Boolean = False

                            Dim dataConvocazione As Date = dta.DefaultView(iSed)("CONVOCAZIONE")
                            Dim codiceCiclo As String = dta.DefaultView(iSed)("CIC_CODICE")
                            Dim numeroSeduta As Integer = dta.DefaultView(iSed)("SED_N_SEDUTA")

                            If Not hCicliSedute.ContainsKey(codiceCiclo) Then
                                '--
                                hCicliSedute.Add(codiceCiclo, New Integer() {numeroSeduta})
                                '--
                                insertCicloConvocazione = True
                                '--
                            Else
                                '--
                                Dim seduteTemp As List(Of Integer) = New List(Of Integer)(hCicliSedute(codiceCiclo))
                                '--
                                If Not seduteTemp.Contains(numeroSeduta) Then
                                    '--
                                    seduteTemp.Add(numeroSeduta)
                                    '--
                                    hCicliSedute(codiceCiclo) = seduteTemp.ToArray()
                                    '--
                                    insertCicloConvocazione = True
                                    '--
                                End If
                                '--
                            End If

                            ' --- Inserimento Cicli Vaccinali (t_cnv_cicli) --- '
                            If insertCicloConvocazione Then

                                ' Inserisce il ciclo, se non è già presente nella t_cnv_cicli.
                                Try
                                    Me.DalCnv.InsertCicloPaziente(codicePaziente, dataConvocazione, codiceCiclo, numeroSeduta, now, userId)

                                Catch oraEx As System.Data.OracleClient.OracleException

                                    ' Se l'errore è "violazione di unicità" (ORA-00001, codice dell'eccezione = 1) 
                                    ' l'inserimento non è avvenuto e il ciclo deve continuare.
                                    If oraEx.Code <> 1 Then
                                        Throw New CicliException("Errore inserimento ciclo.", codicePaziente, dataConvocazione, codiceCiclo, numeroSeduta, oraEx)
                                    End If

                                Catch ex As Exception

                                    Throw New CicliException("Errore inserimento ciclo.", codicePaziente, dataConvocazione, codiceCiclo, numeroSeduta, ex)

                                End Try

                            End If

                            ' --- Inserimento Vaccinazioni Programmate (t_vac_programmate) --- '
                            Dim numIns As Integer

                            Try
                                numIns = Me.DalCnv.InsertVaccinazioneProgrammata(codicePaziente, dataConvocazione,
                                                                                 dta.DefaultView(iSed)("SED_VAC_CODICE").ToString(),
                                                                                 codiceCiclo, numeroSeduta,
                                                                                 dta.DefaultView(iSed)("SED_N_RICHIAMO"),
                                                                                 dta.DefaultView(iSed)("SAS_ASS_CODICE").ToString(),
                                                                                 now, userId)

                            Catch oraEx As System.Data.OracleClient.OracleException

                                ' Se l'errore è "violazione di unicità" (ORA-00001, codice dell'eccezione = 1) 
                                ' l'inserimento non è avvenuto e il ciclo deve continuare.
                                If oraEx.Code <> 1 Then
                                    Throw New VaccinazioniProgrammateException("Errore inserimento vaccinazione programmata.",
                                                                               codicePaziente, dataConvocazione, codiceCiclo,
                                                                               dta.DefaultView(iSed)("SED_VAC_CODICE").ToString(),
                                                                               dta.DefaultView(iSed)("SAS_ASS_CODICE").ToString(),
                                                                               numeroSeduta,
                                                                               dta.DefaultView(iSed)("SED_N_RICHIAMO"),
                                                                               oraEx)
                                End If

                            Catch ex As Exception

                                Throw New VaccinazioniProgrammateException("Errore inserimento vaccinazione programmata.",
                                                                           codicePaziente, dataConvocazione, codiceCiclo,
                                                                           dta.DefaultView(iSed)("SED_VAC_CODICE").ToString(),
                                                                           dta.DefaultView(iSed)("SAS_ASS_CODICE").ToString(),
                                                                           numeroSeduta,
                                                                           dta.DefaultView(iSed)("SED_N_RICHIAMO"),
                                                                           ex)
                            End Try

                            ' Se è stato effettuato l'inserimento, scrive il log
                            If numIns > 0 Then

                                ' --- LOG --- '
                                Dim testataLog As DataLogStructure.Testata

                                If cnvHash.ContainsKey(dataConvocazione) Then
                                    testataLog = cnvHash(dataConvocazione)
                                Else
                                    testataLog = New DataLogStructure.Testata(DataLogStructure.TipiArgomento.CNV_AUTOMATICHE, DataLogStructure.Operazione.Inserimento, True)
                                    cnvHash.Add(dataConvocazione, testataLog)
                                End If

                                Dim recordLog As New DataLogStructure.Record()

                                recordLog.Campi.Add(New DataLogStructure.Campo("VPR_PAZ_CODICE", String.Empty, codicePaziente))
                                recordLog.Campi.Add(New DataLogStructure.Campo("VPR_CNV_DATA", String.Empty, dataConvocazione))
                                recordLog.Campi.Add(New DataLogStructure.Campo("VPR_VAC_CODICE", String.Empty, dta.DefaultView(iSed)("SED_VAC_CODICE")))
                                recordLog.Campi.Add(New DataLogStructure.Campo("VPR_N_RICHIAMO", String.Empty, dta.DefaultView(iSed)("SED_N_RICHIAMO")))
                                recordLog.Campi.Add(New DataLogStructure.Campo("VPR_CIC_CODICE", String.Empty, dta.DefaultView(iSed)("CIC_CODICE")))
                                recordLog.Campi.Add(New DataLogStructure.Campo("VPR_N_SEDUTA", String.Empty, dta.DefaultView(iSed)("SED_N_SEDUTA")))
                                If Not IsDBNull(dta.DefaultView(iSed)("SAS_ASS_CODICE")) Then
                                    recordLog.Campi.Add(New DataLogStructure.Campo("VPR_ASS_CODICE", String.Empty, dta.DefaultView(iSed)("SAS_ASS_CODICE")))
                                End If

                                testataLog.Records.Add(recordLog)
                                ' ----------- '

                            End If

                        Next

                        ' --- LOG --- '
                        For Each testataLog As DataLogStructure.Testata In cnvHash.Values
                            LogBox.WriteData(testataLog)
                        Next
                        ' ----------- '

                    End If  ' if dta.Rows.Count > 0

                    ' --------- Pulizia convocazioni vuote --------- '
                    Try
                        Dim command As New Biz.BizConvocazione.EliminaConvocazioneEmptyCommand()
                        command.CodicePaziente = codicePaziente
                        command.DataConvocazione = Nothing
                        command.DataEliminazione = now
                        command.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.EliminazioneConvocazione
                        command.NoteEliminazione = "Eliminazione convocazione da calcolo convocazioni"
                        command.WriteLog = True

                        bizConvocazioni.EliminaConvocazioneEmpty(command)

                    Catch ex As Exception
                        Throw New ConvocazioniException("Errore cancellazione convocazioni vuote", codicePaziente, ex)
                    End Try

                End Using

                ' --------- Impostazione dello stato vaccinale --------- '
                '
                ' Numero di vaccinazioni programmate
                Dim numProgrammate As Integer = 0
                Try
                    numProgrammate = Me.DalCnv.CountVaccinazioniProgrammatePaz(codicePaziente)
                Catch ex As Exception
                    Throw New VaccinazioniProgrammateException("Errore conteggio programmate.", codicePaziente, ex)
                End Try

                ' Aggiornamento stato vaccinale (+ LOG)
                Dim statoVaccinaleDaImpostare As Enumerators.StatiVaccinali = Enumerators.StatiVaccinali.InCorso

                If numProgrammate = 0 And dta.Rows.Count = 0 Then
                    statoVaccinaleDaImpostare = Enumerators.StatiVaccinali.Terminato
                End If

                ' Se il paziente è in uno stato vaccinale diverso da quello da impostare, lo aggiorno. Altrimenti non faccio niente
                If datiPazienteCorrente.StatoVaccinale <> Convert.ToString(statoVaccinaleDaImpostare) Then

                    Using bizPaziente As New BizPaziente(genericProvider, Me.Settings, Me.ContextInfos, Nothing)

                        bizPaziente.UpdateStatoVaccinalePaziente(codicePaziente, statoVaccinaleDaImpostare)

                    End Using

                End If

                If Me.IsConnessioneInterna Then Me.DalCnv.Transaction.Commit()

            Catch ex As Exception

                If Me.IsConnessioneInterna Then Me.DalCnv.Transaction.Rollback()

                ex.InternalPreserveStackTrace()
                Throw

            Finally
                If Me.IsConnessioneInterna And Not Me.DalCnv Is Nothing Then Me.DalCnv.Dispose()
            End Try

        End Using

    End Sub

    ' Calcolo data convocazione
    Private Sub CalcolaDataConvocazione(ByRef row As DataRow, codicePaziente As Long, dataNascita As Date)

        Dim datiIntervalloSeduta As ObjectModel.DatiIntervalliSeduteClass =
            Me.DalCnv.GetIntervalloUltimaEffettuata(codicePaziente, row.Item("CIC_CODICE").ToString(), row.Item("SED_N_SEDUTA"))

        If Not datiIntervalloSeduta Is Nothing Then

            datiIntervalloSeduta.IntervalloProssima =
                Math.Round(datiIntervalloSeduta.IntervalloProssima * Me.Settings.AGGGIORNI)

            ' Se la data di cnv calcolata in base all'età seduta è inferiore alla data calcolata sommando
            ' alla data di effettuazione della vac più recente l'intervallo relativo, reimposto l'età seduta
            If dataNascita.AddDays(row("TSD_ETA_SEDUTA")) < datiIntervalloSeduta.DataEffettuazione.AddDays(datiIntervalloSeduta.IntervalloProssima) Then

                row("TSD_ETA_SEDUTA") = datiIntervalloSeduta.DataEffettuazione.Subtract(dataNascita).Days + datiIntervalloSeduta.IntervalloProssima

            End If

        End If

        ' Se la data possibile in base all'età seduta è inferiore alla data odierna, reimposto i valori per ottenere la data di oggi
        Dim dataEtaSeduta As Date = dataNascita.AddDays(row("TSD_ETA_SEDUTA"))

        If dataEtaSeduta < Date.Now Then

            ' Convocazione ad oggi
            row("CONVOCAZIONE") = Date.Now.ToShortDateString()

            ' Sommo all'intervallo la differenza tra le due età seduta
            Dim etaSedutaAdOggi As Integer = Date.Now.Subtract(dataNascita).Days

            row("TSD_INTERVALLO") = row("TSD_INTERVALLO") + row("TSD_ETA_SEDUTA") - etaSedutaAdOggi
            If row("TSD_INTERVALLO") < 0 Then row("TSD_INTERVALLO") = 0

            ' L'età seduta diventa l'età in giorni calcolata ad oggi
            row("TSD_ETA_SEDUTA") = etaSedutaAdOggi

        Else

            ' Se la data possibile per l'età seduta è futura, la imposto come data di convocazione
            row("CONVOCAZIONE") = dataEtaSeduta.ToShortDateString()

        End If

    End Sub

    ' Controlla se, per ciclo, seduta e vaccinazione specificati, esiste una seduta precedente fra quelle da programmare
    Private Function EsisteSedutaPrecedenteStessoCiclo(dt As DataTable, ciclo As String, seduta As Integer, vaccinazione As String) As Boolean

        Dim exists As Boolean = False

        Dim filter As New System.Text.StringBuilder()

        ' Azzeramento filtri del dataview
        dt.DefaultView.RowStateFilter = DataViewRowState.CurrentRows    ' solo le righe correnti, non cancellate.
        dt.DefaultView.RowFilter = String.Empty

        ' Filtro tutto il dataview per stesso ciclo e stessa vaccinazione, con sedute precedenti
        filter.AppendFormat("CIC_CODICE = '{0}' and SED_N_SEDUTA < {1} and SED_VAC_CODICE = '{2}'", ciclo, seduta, vaccinazione)
        dt.DefaultView.RowFilter = filter.ToString()

        ' Se ci sono sedute precedenti, imposto il valore da restituire a true
        If dt.DefaultView.Count > 0 Then exists = True

        ' Azzeramento filtri del dataview
        dt.DefaultView.RowStateFilter = DataViewRowState.CurrentRows
        dt.DefaultView.RowFilter = String.Empty

        Return exists

    End Function

    ' Imposta i valori intervallo, etaSeduta e dataConvocazione nei campi corrispondenti.
    Private Sub AccorpaDatiSeduta(ByRef row As DataRowView, intervallo As Integer, etaSeduta As Integer, dataConvocazione As Date)

        row("TSD_INTERVALLO") = intervallo
        row("TSD_ETA_SEDUTA") = etaSeduta
        row("CONVOCAZIONE") = dataConvocazione

    End Sub

    ' --------- Metodo per la Gestione delle Vaccinazioni Sostitute --------- '
    'controlla le righe tenendo conto di ciò che è presente nelle vaccinazioni sostitute
    Private Sub ControllaVaccinazioniSostitute(codicePaziente As Integer, ByRef dta As DataTable, ByRef dt_vacProg As DataTable)

        Dim vacSostituta As String = String.Empty
        Dim vacDaEliminare As Boolean

        Dim maxRichiamo, sedNumRichiamo As Integer

        Dim listVacc As ArrayList

        For Each row As DataRow In dta.Rows

            Try
                vacSostituta = Me.DalCnv.GetVaccinazioneSostituta(row("sed_vac_codice").ToString())
            Catch ex As Exception
                Throw New VaccinazioniSostituteException(String.Format("Errore recupero vaccinazione sostituta.{0}Vaccinazione di cui recuperare la sostituta: {1}", 
                                                                       Environment.NewLine, vacSostituta), ex)
            End Try

            If vacSostituta <> String.Empty Then

                ' => significa che la vaccinazione considerata nella convocazione è la "continuazione" della vaccinazione impostata nel campo 

                ' Controllo esistenza inadempienze o esclusioni per la vaccinazione
                Try
                    vacDaEliminare = Me.DalCnv.ControllaInadempienzaEsclusioneVacPaz(vacSostituta, codicePaziente)
                Catch ex As Exception
                    Throw New VaccinazioniSostituteException("Errore controllo inadempienza-esclusione vaccinazione.", codicePaziente, vacSostituta, ex)
                End Try


                If vacDaEliminare Then
                    ' In questo caso, devo eliminare la riga
                    row.Delete()
                Else
                    ' Controllo se la vaccinazione sostituta è presente nelle eseguite e in quale dose
                    ' Se la dose della cnv da programmare è minore di quella della vac eseguita, cancello la cnv 
                    Try
                        maxRichiamo = Me.DalCnv.GetMaxRichiamo(codicePaziente, vacSostituta)
                    Catch ex As Exception
                        Throw New VaccinazioniSostituteException("Errore recupero massimo richiamo vaccinazione.", codicePaziente, vacSostituta, ex)
                    End Try

                    If maxRichiamo > 0 Then
                        ' Per evitare che vada in errore se sed_n_richiamo dovesse contenere dbnull
                        Try
                            sedNumRichiamo = CInt(row("SED_N_RICHIAMO"))
                        Catch ex As Exception
                            sedNumRichiamo = 0
                        End Try

                        ' Elimino la convocazione da quelle da programmare
                        If sedNumRichiamo <= maxRichiamo Then row.Delete()

                    End If

                End If

            End If

            '-------------------------------------------------------------------
            ' Deve controllare se tra le vaccinazioni calcolabili è presente una
            ' vaccinazione considerata sostituta di una presente tra quelle programmate [modifica 30/06/2006]
            If row.RowState <> DataRowState.Deleted Then

                ' Ciclo per modificare le vaccinazioni programmate.
                If Not dt_vacProg Is Nothing AndAlso dt_vacProg.Rows.Count > 0 Then

                    Try
                        listVacc = Me.DalCnv.GetVaccinazioniBySostituta(row("SED_VAC_CODICE").ToString())
                    Catch ex As Exception
                        Dim _msg As String = String.Format("Errore recupero codici vaccinazioni associate alla vaccinazione sostituta.{0}Codice vaccinazione sostituta: {1}", Environment.NewLine, row("SED_VAC_CODICE").ToString)
                        Throw New VaccinazioniSostituteException(_msg, ex)
                    End Try

                    For i As Integer = 0 To listVacc.Count - 1

                        ' Se la vaccinazione è presente tra le programmate, elimino la convocazione da quelle da programmare
                        If dt_vacProg.Select(String.Format("SED_VAC_CODICE = '{0}'", listVacc(i).ToString())).Length > 0 Then
                            row.Delete()
                            Exit For
                        End If

                    Next

                End If

            End If
            '-------------------------------------------------------------------

        Next

        'rende effettive le eventuali modifiche sul datatable
        dta.AcceptChanges()

    End Sub

    ' Effettua l'accorpamento, in data dataCnv, delle vaccinazioni di dta che compongono la convocaione indicata da cnvDaAccorpare
    Private Sub AccorpaDataCnv(ByRef dta As DataTable, dataCnv As Date, ByRef cnvDaAccorpare As Associabilita.ObjectModel.ConvocazioniClass)

        Dim key As Object()
        Dim row As DataRow

        ' Ciclo per ogni vaccinazione da accorpare
        For idxVac As Integer = 0 To cnvDaAccorpare.ElencoVaccinazioni.Count - 1

            ' Chiave di dta (ciclo-seduta-vaccinazione) con cui accedere direttamente alla riga
            key = New Object() {cnvDaAccorpare.CodiceConvocazione,
                                cnvDaAccorpare.NumeroSeduta,
                                cnvDaAccorpare.ElencoVaccinazioni(idxVac).ToString()}

            ' Modifico la data di convocazione del datatable contenente le vaccinazioni da programmare
            row = dta.Rows.Find(key)
            If Not row Is Nothing Then row("CONVOCAZIONE") = dataCnv

        Next

    End Sub

    ' Cancellazione delle vaccinazioni di dta che compongono la convocazione indicata da cnvDaAccorpare
    Private Sub EliminaCnv(ByRef dta As DataTable, ByRef cnv As Associabilita.ObjectModel.ConvocazioniClass)

        Dim key As Object()
        Dim row As DataRow

        ' Ciclo per ogni vaccinazione della cnv da cancellare
        For idxVac As Integer = 0 To cnv.ElencoVaccinazioni.Count - 1

            ' Chiave di dta (ciclo-seduta-vaccinazione) con cui accedere direttamente alla riga
            key = New Object() {cnv.CodiceConvocazione,
                                cnv.NumeroSeduta,
                                cnv.ElencoVaccinazioni(idxVac).ToString()}

            ' Cancellazione riga del datatable
            row = dta.Rows.Find(key)
            If Not row Is Nothing Then row.Delete()

        Next

    End Sub

    ' Restituisce true se la convocazione può essere accorpata a tutte quelle della già accorpate specificate nella collection
    Private Function IsCnvAccorpabile(cnvDaAccorpare As Associabilita.ObjectModel.ConvocazioniClass, cnvAccorpateData As Associabilita.ObjectModel.ConvocazioniCollection, ByRef ctrlAssociabilita As Associabilita.ControlloAssociabilita) As Boolean

        For idxAcc As Integer = 0 To cnvAccorpateData.Count - 1

            If Not ctrlAssociabilita.VaccinazioniAssociabili(cnvDaAccorpare.ElencoVaccinazioni, cnvAccorpateData(idxAcc).ElencoVaccinazioni) Then
                Return False
            End If

        Next

        Return True

    End Function

    ' Restituisce la prima data utile in cui spostare la convocazione, non accorpabile nella data di cnv calcolata,
    ' partendo dalla data originale o da quella di fine sospensione.
    ' Se non trova nessuna data, restituisce date.minvalue
    Private Function GetDataSpostamento(cnvDaProgrammareOriginali As Associabilita.ObjectModel.ConvocazioniCollection,
                                        cnvDaAccorpare As Associabilita.ObjectModel.ConvocazioniClass,
                                        cnvAccorpate As Associabilita.ObjectModel.ConvocazioniAccorpateCollection,
                                        dataFineSospensione As Date,
                                        ByRef ctrlAssociabilita As Associabilita.ControlloAssociabilita) As Date

        ' Recupero la data di convocazione originale
        Dim cnvDaProgOriginale As Associabilita.ObjectModel.ConvocazioniClass =
            cnvDaProgrammareOriginali.FindByCodCnvSeduta(cnvDaAccorpare.CodiceConvocazione, cnvDaAccorpare.NumeroSeduta)

        Dim dataCnvOriginale As Date = cnvDaProgOriginale.DataConvocazione

        ' Ultima data possibile per lo spostamento della data
        Dim dataFineIntervallo As Date = dataCnvOriginale.AddDays(cnvDaProgOriginale.Intervallo)

        ' Data di partenza per il calcolo della data di spostamento.
        Dim dataSpostamento As Date = dataCnvOriginale

        ' Se la data di convocazione è inferiore alla data di fine sospensione, parto dal giorno successivo alla fine della sospensione
        If dataSpostamento < dataFineSospensione Then dataSpostamento = dataFineSospensione.AddDays(1)

        ' Se la data calcolata è inferiore ad oggi, parto da oggi.
        If dataSpostamento < Date.Now.Date Then dataSpostamento = Date.Now.Date

        ' Elenco di convocazioni già programmate in una data
        Dim cnvAccorpateData As Associabilita.ObjectModel.ConvocazioniAccorpateClass = Nothing

        ' Se la data in cui spostare la convocazione è ancora contenuta nell'intervallo 
        ' rispetto alla data originale, posso spostare la data di cnv di un giorno.
        While dataSpostamento <= dataFineIntervallo

            ' Sposto la data
            cnvDaAccorpare.DataConvocazione = dataSpostamento

            ' Controllo se ci sono convocazioni già accorpate in tale data
            cnvAccorpateData = cnvAccorpate.FindByDataConvocazione(dataSpostamento)

            If cnvAccorpateData Is Nothing Then

                ' Se non ci sono convocazioni nella data, tutto ok, ho trovato la data giusta.
                Return dataSpostamento

            Else

                ' Se ci sono convocazioni, devo controllare l'accorpabilità nella data calcolata.
                If IsCnvAccorpabile(cnvDaAccorpare, cnvAccorpateData.ElencoConvocazioni, ctrlAssociabilita) Then

                    ' Se l'associabilità è possibile, restituisco la data calcolata.
                    Return dataSpostamento

                End If

            End If

            ' Se sono arrivato qui, non è possibile accorpare nella data di spostamento calcolata.
            ' Incremento la data di 1 e riprovo.
            dataSpostamento = dataSpostamento.AddDays(1)

        End While

        Return Date.MinValue

    End Function

    ' Accorpa la cnv specificata alla collection delle convocazioni accorpate
    Private Function AccorpaCnv(convocazioneDaAccorpare As Associabilita.ObjectModel.ConvocazioniClass, ByRef convocazioniAccorpate As Associabilita.ObjectModel.ConvocazioniAccorpateCollection) As Boolean

        Dim toAdd As Boolean

        Dim cnvAccorpate As Associabilita.ObjectModel.ConvocazioniAccorpateClass =
            convocazioniAccorpate.FindByDataConvocazione(convocazioneDaAccorpare.DataConvocazione)

        If cnvAccorpate Is Nothing Then
            ' Se non ci sono cnv accorpate nella data specificata, crea un nuovo elemento nella collection
            cnvAccorpate = New Associabilita.ObjectModel.ConvocazioniAccorpateClass()
            cnvAccorpate.DataConvocazione = convocazioneDaAccorpare.DataConvocazione
            toAdd = True
        Else
            ' Se la data è già presente, aggiunge la convocazione all'elenco delle convocazioni della collection.
            toAdd = False
        End If

        cnvAccorpate.ElencoConvocazioni.Add(convocazioneDaAccorpare)

        If toAdd Then convocazioniAccorpate.Add(cnvAccorpate)

        Return toAdd

    End Function

#End Region

#Region " Creazione di una convocazione singola "

    ''' <summary>Controlla se esiste una convocazione per il paziente nella data specificata. 
    '''  Se non c'è, ne inserisce una. Se c'è già, la modifica in base ai dati passati alla funzione. 
    '''  Restituisce true se è stata inserita una nuova convocazione, false se è stata modificata o se non è stato fatto niente.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataConvocazione"></param>
    ''' <param name="durataAppuntamento"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreaSingolaConvocazione(codicePaziente As Integer, dataConvocazione As Date, durataAppuntamento As Int16) As Boolean

        Dim insCnv As Boolean = False

        ' Pulizia della struttura contenente le convocazioni scartate dal calcolo
        Me.ConvocazioniScartate.Clear()

        Try
            Using genericProvider As New DbGenericProvider(Me.DalCnv.Provider, Me.DalCnv.Connection, Me.DalCnv.Transaction)

                ' Controllo l'esistenza di una cnv per il paz nella data specificata
                Dim convocazioneEsistente As Entities.Convocazione = Nothing

                Try
                    convocazioneEsistente = genericProvider.Convocazione.GetConvocazionePaziente(codicePaziente, dataConvocazione)
                Catch ex As Exception
                    Throw New ConvocazioniException("Errore recupero dati convocazione.", codicePaziente, dataConvocazione, ex)
                End Try

                If Not convocazioneEsistente Is Nothing Then

                    ' --- La convocazione esiste già --- '
                    ' In caso di update, vengono modificati soltanto Durata, Bilancio e Malattia.

                    ' Se la durata specificata è inferiore a quella già presente, non la sovrascrive
                    ' Se durata e bilancio non sono da modificare, non effettuo nessuna modifica e restituisco False
                    If convocazioneEsistente.Durata_Appuntamento >= durataAppuntamento Then Return False

                    ' Modifica della convocazione esistente
                    If Me.IsConnessioneInterna Then Me.DalCnv.BeginTransaction()

                    Try
                        Me.DalCnv.UpdateConvocazione(codicePaziente, dataConvocazione, durataAppuntamento)

                        If Me.IsConnessioneInterna Then Me.DalCnv.Transaction.Commit()

                    Catch ex As Exception

                        If Me.IsConnessioneInterna Then Me.DalCnv.Transaction.Rollback()

                        Throw New ConvocazioniException("Errore modifica convocazione per il paziente.", codicePaziente, dataConvocazione,
                                                        String.Empty, durataAppuntamento, ex)
                    End Try

                Else
                    ' --- La convocazione non c'è --- '
                    ' Viene inserita una nuova convocazione per il paziente nella data e con durata, bilancio e malattia specificati.

                    If Me.IsConnessioneInterna Then Me.DalCnv.BeginTransaction()

                    Try
                        Dim convocazione As New Entities.Convocazione()
                        convocazione.Paz_codice = codicePaziente
                        convocazione.Data_CNV = dataConvocazione
                        convocazione.Cns_Codice = Me.CodiceConsultorioCorrente
                        convocazione.Durata_Appuntamento = durataAppuntamento

                        convocazione.DataInserimento = DateTime.Now
                        convocazione.IdUtenteInserimento = Me.ContextInfos.IDUtente

                        ' Inserimento convocazione
                        insCnv = genericProvider.Convocazione.InsertConvocazione(convocazione)

                        ' Modifica stato vaccinale paziente: se è in stato Terminato, lo imposta a In Corso (e scrive il log)
                        Using bizPaziente As New BizPaziente(genericProvider, Me.Settings, Me.ContextInfos, Nothing)

                            bizPaziente.UpdateStatoVaccinalePaziente(codicePaziente, Onit.OnAssistnet.OnVac.Enumerators.StatiVaccinali.Terminato, Onit.OnAssistnet.OnVac.Enumerators.StatiVaccinali.InCorso)

                        End Using

                        If Me.IsConnessioneInterna Then Me.DalCnv.Transaction.Commit()

                    Catch ex As Exception

                        If Me.IsConnessioneInterna Then Me.DalCnv.Transaction.Rollback()

                        Throw New ConvocazioniException("Errore inserimento convocazione per il paziente.", codicePaziente, dataConvocazione,
                                                        Me.CodiceConsultorioCorrente, durataAppuntamento, ex)
                    End Try

                End If

            End Using

        Finally
            If Me.IsConnessioneInterna And Not Me.DalCnv Is Nothing Then Me.DalCnv.Dispose()
        End Try

        Return insCnv

    End Function

#End Region

#Region " Creazione convocazione per campagna vaccinale "

    ''' <summary>Creazione di una convocazione in base ad una campagna vaccinale.
    ''' La convocazione viene creata, oppure viene modificata una convocazione già presente per associare le vaccinazioni 
    ''' dell'associazione specificata.
    ''' Se una vaccinazione non può essere programmata, la convocazione viene aggiunta alla struttura delle scartate.
    ''' Restituisce true se tutto ok, false altrimenti.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataConvocazione"></param>
    ''' <param name="flagForzaturaSpostamentoConvocazioneEsistente">Se esiste già una convocazione per una o più delle vaccinazioni in campagna, 
    ''' e la convocazione esistente è successiva, forza l'anticipo della convocazione: cancella la convocazione esistente e crea quella in campagna</param>
    ''' <param name="includiRitardari"></param>
    ''' <param name="codiceAssociazione"></param>
    ''' <param name="idUtente"></param>
    Public Function CreaConvocazionePerCampagna(codicePaziente As Integer, dataConvocazione As DateTime, flagForzaturaSpostamentoConvocazioneEsistente As Boolean, includiRitardari As Boolean, codiceAssociazione As String, idUtente As Integer) As Boolean

        Dim ownTransaction As Boolean = False

        Try
            If Me.DalCnv.Transaction Is Nothing Then
                Me.BeginTransaction()
                ownTransaction = True
            End If

            Using genericprovider As New Onit.OnAssistnet.OnVac.DAL.DbGenericProvider(Me.DalCnv.Provider, Me.DalCnv.Connection, Me.DalCnv.Transaction)

                ' --- Controllo convocazione già esistente in stessa data (in altro centro vaccinale) --- '
                If genericprovider.Convocazione.ExistsConvocazioneAltroConsultorio(codicePaziente, dataConvocazione, Me.CodiceConsultorioCorrente) Then

                    AddToConvocazioniScartate(codicePaziente, dataConvocazione, String.Empty, -1, -1,
                                              Biz.BizConvocazione.CodiceMotivoConvocazioneScartata.CampagnaConvocazioneEsistenteInStessaDataAltroCns,
                                              Nothing)
                    Return False

                End If

                ' --- Vaccinazioni dell'associazione da programmare --- '
                Dim codiciVaccinazioniAssociazione As ArrayList = genericprovider.Associazioni.GetVaccinazioniAssociazione(codiceAssociazione)

                Using bizConvocazione As New BizConvocazione(genericprovider, Me.Settings, Me.ContextInfos, Nothing)

                    Dim vaccinazioniDaProgrammare As New Hashtable()

                    ' --- Controllo vaccinazioni già programmate --- '
                    Dim vaccinazioniProgrammateDaScorporare As New Dictionary(Of Date, List(Of String))

                    For Each codiceVaccinazioneAssociazione As String In codiciVaccinazioniAssociazione

                        Dim scorporaVaccinazioneProgrammata As Boolean = False

                        Dim numeroRichiamoDaEffettuare As Integer =
                            genericprovider.VaccinazioniEseguite.GetMaxRichiamo(codicePaziente, codiceVaccinazioneAssociazione) + 1

                        Dim dataCnvEsistente As Object =
                            genericprovider.VaccinazioneProg.GetDataByVaccinazione(codicePaziente, codiceVaccinazioneAssociazione)

                        If dataCnvEsistente <> Nothing Then

                            Dim scartaCnv As Boolean

                            If dataCnvEsistente > dataConvocazione Then

                                If flagForzaturaSpostamentoConvocazioneEsistente Then
                                    scorporaVaccinazioneProgrammata = True
                                Else
                                    scartaCnv = True
                                End If

                            ElseIf dataCnvEsistente < dataConvocazione Then

                                If includiRitardari Then
                                    If bizConvocazione.IsRitardatario(codicePaziente, dataCnvEsistente) Then
                                        scorporaVaccinazioneProgrammata = True
                                    Else
                                        scartaCnv = True
                                    End If
                                Else
                                    scartaCnv = True
                                End If

                            End If

                            If scartaCnv Then

                                AddToConvocazioniScartate(codicePaziente, dataCnvEsistente, String.Empty, numeroRichiamoDaEffettuare, 0,
                                                          Biz.BizConvocazione.CodiceMotivoConvocazioneScartata.CampagnaVaccinazioneProgrammata,
                                                          New List(Of String)(New String() {codiceVaccinazioneAssociazione}))
                                Return False

                            ElseIf scorporaVaccinazioneProgrammata Then

                                If Not vaccinazioniProgrammateDaScorporare.ContainsKey(dataCnvEsistente) Then
                                    vaccinazioniProgrammateDaScorporare.Add(dataCnvEsistente, New List(Of String)())
                                End If
                                vaccinazioniProgrammateDaScorporare(dataCnvEsistente).Add(codiceVaccinazioneAssociazione)

                            End If

                        End If

                        If dataCnvEsistente <> dataConvocazione Then
                            vaccinazioniDaProgrammare.Add(codiceVaccinazioneAssociazione, numeroRichiamoDaEffettuare)
                        End If

                    Next

                    ' --- Controllo associabilità --- '
                    If Me.Settings.CTRL_ASSOCIABILITA_VAC Then

                        If vaccinazioniDaProgrammare.Count > 0 Then

                            ' Elenco vaccinazioni nella nuova data
                            Dim collVaccProgrammate As ArrayList =
                                genericprovider.VaccinazioneProg.GetCodiceVacProgrammatePazienteByData(codicePaziente, dataConvocazione)

                            ' Se ci sono vaccinazioni, controllo che siano associabili con le nuove
                            If collVaccProgrammate.Count > 0 Then

                                ' Creazione oggetto utilizzato per effettuare i controlli di associabilità
                                Dim ctrlAssociabilita As New OnVac.Associabilita.ControlloAssociabilita(Me.DalCnv.Provider, Me.DalCnv.Connection, Me.DalCnv.Transaction)

                                ' Se le vac non sono somministrabili lo stesso giorno, non crea la convocazione (che viene aggiunta alle scartate)
                                If Not ctrlAssociabilita.VaccinazioniAssociabili(New ArrayList(vaccinazioniDaProgrammare.Keys), collVaccProgrammate) Then

                                    AddToConvocazioniScartate(codicePaziente, dataConvocazione, String.Empty, -1, -1,
                                                              Biz.BizConvocazione.CodiceMotivoConvocazioneScartata.NonAssociabilitaDataCnv,
                                                              vaccinazioniDaProgrammare.Keys)
                                    Return False

                                End If

                            End If

                        End If

                    End If

                    ' elimina vaccinazione programmata da altra convocazione 
                    For Each vaccinazioneProgrammataDaScorporare As KeyValuePair(Of Date, List(Of String)) In vaccinazioniProgrammateDaScorporare

                        For Each codiceVaccinazioneProgrammataDaScorporare As String In vaccinazioneProgrammataDaScorporare.Value
                            '--
                            'N.B. a differenza dell'unione convocazioni, i solleciti del ciclo della vaccinazione da scorporare vengono persi
                            '--
                            genericprovider.VaccinazioneProg.EliminaVaccinazioneProgrammata(codicePaziente, codiceVaccinazioneProgrammataDaScorporare, vaccinazioneProgrammataDaScorporare.Key)
                            '--
                            genericprovider.Convocazione.EliminaCicliSenzaVaccinazioniProgrammate(codicePaziente, vaccinazioneProgrammataDaScorporare.Key)
                            '--
                        Next

                    Next

                    ' --- Insert/Update convocazione --- '
                    Dim now As DateTime = DateTime.Now

                    If genericprovider.Convocazione.Exists(codicePaziente, dataConvocazione) Then

                        ' La cnv esiste già: modifico il campo campagna
                        genericprovider.Convocazione.UpdateCnvCampagna(codicePaziente, dataConvocazione, True)

                    Else

                        ' La cnv non è presente nella data specificata: la creo
                        Dim datiCnv As New Entities.Convocazione()

                        datiCnv.Paz_codice = codicePaziente
                        datiCnv.Data_CNV = dataConvocazione
                        datiCnv.EtaPomeriggio = String.Empty
                        datiCnv.Rinvio = String.Empty
                        datiCnv.DataAppuntamento = Date.MinValue
                        datiCnv.TipoAppuntamento = String.Empty
                        datiCnv.Durata_Appuntamento = Me.Settings.TEMPOSED
                        datiCnv.DataInvio = Date.MinValue
                        datiCnv.Cns_Codice = Me.CodiceConsultorioCorrente
                        datiCnv.IdUtente = idUtente
                        datiCnv.DataPrimoAppuntamento = Date.MinValue
                        datiCnv.CodiceAmbulatorio = -1
                        datiCnv.CampagnaVaccinale = "S"
                        datiCnv.DataInserimento = now
                        datiCnv.IdUtenteInserimento = idUtente

                        Me.DalCnv.InsertConvocazione(datiCnv)

                    End If

                    ' --- Controllo esclusione vaccinazioni --- '
                    ' Se la vaccinazione è esclusa, la convocazione per la campagna viene creata comunque,
                    ' ma viene comunque loggata nella struttura delle convocazioni scartate.
                    For Each vaccinazioneDaProgrammare As String In vaccinazioniDaProgrammare.Keys

                        If genericprovider.VaccinazioniEscluse.ExistsVaccinazioneEsclusaNonScaduta(codicePaziente, vaccinazioneDaProgrammare, dataConvocazione) Then

                            Dim dataScadenzaEscl As Date =
                                genericprovider.VaccinazioniEscluse.GetDataVaccinazioneEsclusaNonScaduta(codicePaziente, vaccinazioneDaProgrammare, dataConvocazione)

                            AddToConvocazioniScartate(codicePaziente, dataScadenzaEscl, String.Empty, -1, 0,
                                                      Biz.BizConvocazione.CodiceMotivoConvocazioneScartata.CampagnaVaccinazioneEsclusa,
                                                      New List(Of String)(New String() {vaccinazioneDaProgrammare}))
                            Exit For

                        End If

                    Next

                    ' --- Inserimento programmate --- '
                    Dim vaccinazioniDaProgrammareEnumerator As IDictionaryEnumerator = vaccinazioniDaProgrammare.GetEnumerator()

                    While vaccinazioniDaProgrammareEnumerator.MoveNext()

                        Me.DalCnv.InsertVaccinazioneProgrammata(codicePaziente, dataConvocazione, vaccinazioniDaProgrammareEnumerator.Key,
                                                                String.Empty, -1, vaccinazioniDaProgrammareEnumerator.Value, codiceAssociazione,
                                                                now, idUtente)

                    End While

                    ' --- Eliminazione convocazioni vuote --- '
                    For Each vaccinazioneProgrammataDaScorporare As KeyValuePair(Of Date, List(Of String)) In vaccinazioniProgrammateDaScorporare

                        Dim command As New Biz.BizConvocazione.EliminaConvocazioneEmptyCommand()
                        command.CodicePaziente = codicePaziente
                        command.DataConvocazione = vaccinazioneProgrammataDaScorporare.Key
                        command.DataEliminazione = now
                        command.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.SpostamentoConvocazione
                        command.NoteEliminazione = "Eliminazione convocazione da creazione campagna vaccinale"
                        command.WriteLog = True

                        bizConvocazione.EliminaConvocazioneEmpty(command)

                    Next

                End Using

                ' --- Stato Vaccinale --- '
                ' Se lo stato vaccinale del paziente è "Terminato" deve diventare "In Corso" (+ LOG)
                Using bizPaziente As New BizPaziente(genericprovider, Me.Settings, Me.ContextInfos, Nothing)
                    bizPaziente.UpdateStatoVaccinalePaziente(codicePaziente, Enumerators.StatiVaccinali.Terminato, Enumerators.StatiVaccinali.InCorso)
                End Using

                ' --- Regolarizzazione --- '
                ' Se il paziente non è regolarizzato, deve essere regolarizzato automaticamente (+ LOG)
                genericprovider.Paziente.RegolarizzaPaziente(codicePaziente, True)

            End Using

            If ownTransaction Then
                Me.CommitTransaction()
            End If

        Catch ex As Exception

            If ownTransaction Then
                Me.RollbackTransaction()
            End If

            ex.InternalPreserveStackTrace()
            Throw

        End Try

        Return True

    End Function

#End Region

#Region " Azzeramento campi convocazione "

    ''' <summary> 
    ''' Imposta a null la data di appuntamento relativa alla convocazione. 
    ''' Se il flag azzera_data_invio è true, imposta a null anche la data di invio. 
    ''' Restituisce true se ha eseguito la modifica, false altrimenti.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataConvocazione"></param>
    ''' <param name="azzeraDataInvio"></param>
    Public Function AzzeramentoDateAppuntamentoInvio(codicePaziente As Integer, dataConvocazione As Date, azzeraDataInvio As Boolean) As Boolean

        Dim count As Integer

        Try
            count = Me.DalCnv.UpdateDateAppuntamentoInvio(codicePaziente, dataConvocazione, Date.MinValue, azzeraDataInvio)
        Catch ex As Exception
            Throw New ConvocazioniException("Errore Azzeramento Date Appuntamento e Invio Convocazione.",
                                            codicePaziente, dataConvocazione, ex)
        End Try

        Return (count > 1)

    End Function

#End Region

End Class
