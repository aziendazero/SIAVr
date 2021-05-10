Imports System.Configuration
Imports System.Collections.Generic
Imports System.Collections.ObjectModel

Imports Onit.Database.DataAccessManager

Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.Filters
Imports Onit.OnAssistnet.OnVac.Collection
Imports Onit.OnAssistnet.OnVac.Log
Imports Onit.OnAssistnet.OnVac.Log.DataLogManager
Imports Onit.OnAssistnet.OnVac.Common.Utility.Extensions

''' -----------------------------------------------------------------------------
''' Project	 : Paziente_HL7
''' Class	 : QueryPaziente
''' 
''' -----------------------------------------------------------------------------
''' <summary> Questa classe racchiude tutte le procedure per la ricerca e la
''' modifica dei pazienti sul database </summary>
''' <remarks>
''' </remarks>
''' <history>
''' 	[adesimone]	10/06/2008	Created
''' </history>
''' -----------------------------------------------------------------------------
Public Class BizPazienteCentrale
    Inherits BizClass

#Region " Costanti "

    Private Const ERRORE_PROCEDURA As String = "#Errore"
    Private Const PAZIENTI_MAX As Integer = 200

    'tabelle dei pazienti
    Public Const T_PAZ_PAZIENTI As String = "T_PAZ_PAZIENTI"
    Public Const T_PAZ_STORICO_VARIAZIONI As String = "T_PAZ_VARIAZIONI_CENTRALE"

    'anagrafiche
    Public Const T_ANA_MEDICI As String = "T_MED_MEDICI"
    Public Const T_ANA_USL As String = "T_ANA_USL"
    Public Const T_ANA_COMUNI As String = "T_ANA_COMUNI"
    Public Const T_ANA_CITTADINANZE As String = "T_ANA_CITTADINANZE"
    Public Const T_ANA_DISTRETTI As String = "T_ANA_DISTRETTI"
    Public Const T_ANA_STATI_CIVILI As String = "T_ANA_STATI_CIVILI"
    Public Const T_ANA_MOTIVI_CESSAZ_ASSIST As String = "T_ANA_MOTIVI_CESSAZ_ASSIST"
    Public Const T_ANA_POSIZIONI_PROFESSIONALI As String = "T_ANA_POSIZIONI_PROFESSIONALI"
    Public Const T_ANA_LOCALITA As String = "T_ANA_LOCALITA"

#End Region

#Region " Properties "

    Protected ReadOnly Property LOG_PATH() As String
        Get
            Return ConfigurationManager.AppSettings.Get("LogPath")
        End Get
    End Property

#End Region

#Region " Costruttori "

    Public Sub New(contextInfos As BizContextInfos, logOptions As BizLogOptions)

        MyBase.New(contextInfos, logOptions)

    End Sub

    Public Sub New(settings As Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)

        MyBase.New(settings, contextInfos, logOptions)

    End Sub
#End Region

#Region " Types "

    Public Class SalvaAnagraficaResult

        Public Message As String
        Public HasError As Boolean
        Public PazCodice As String

        Public Sub generateError(message As String)

            Me.Message = message
            Me.HasError = True

        End Sub

    End Class

#End Region

    ''' <summary>
    ''' Salvataggio in centrale dei dati del paziente.
    ''' In base alla configurazione, può scrivere direttamente in centrale 
    ''' oppure utilizzare un servizio esterno (se il parametro CENTRALE_WS_XMPI è "S") specificato nel parametro UrlXmpiService del web.config
    ''' </summary>
    ''' <param name="paz"></param>
    ''' <param name="strEnte"></param>
    ''' <param name="strAddetto"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SalvaAnagrafica(ByRef paz As PazienteCentrale, strEnte As String, strAddetto As String) As SalvaAnagraficaResult

        Dim result As New SalvaAnagraficaResult()

        Dim allOk As Boolean = True
        Dim intNumValorized As Integer = 0 'controlla il numero di attributi del paziente modificati

        Dim strErrMessage As String = String.Empty

        ' Per non salvarla in centrale, metto a nothing la categoria di rischio 
        If Me.Settings.TIPOANAG_CATEGORIA_RISCHIO <> Enumerators.TipoAnags.CentraleLettScritt Then
            paz.CategoriaRischioCodice = Nothing
        End If

        If Me.Settings.CENTRALE_CHECK_UNICF AndAlso Not paz.codiceFiscale Is Nothing AndAlso paz.codiceFiscale <> "" Then
            ' il codice fiscale del paziente in centrale deve essere univoco
            If Not Me.UniCF(paz.codiceFiscale, strErrMessage, paz.codice) Then
                result.generateError(strErrMessage)
                allOk = False
            End If
        End If

        If allOk AndAlso Me.Settings.CENTRALE_CHECK_UNITESSERA Then
            ' il codice fiscale del paziente in centrale deve essere univoco
            If Not Me.UniTessera(paz.Tessera, strErrMessage, paz.codice) Then
                result.generateError(strErrMessage)
                allOk = False
            End If
        End If

        If allOk Then

            Select Case paz.TipoVariazione

                Case PazienteCentrale.TipoVariazioneEnum.Added

                    'effettuo l'inserimento in centrale
                    Dim strNewCode As String = ""

                    If Me.Settings.CENTRALE_WS_XMPI Then
                        '---
                        ' Inserimento utilizzando il servizio di scrittura in centrale specificato nel parametro UrlXmpiService del web.config
                        '---
                        strErrMessage = BizXmpi.update(paz, strEnte, strEnte)
                        strNewCode = paz.codice
                        allOk = String.IsNullOrEmpty(strErrMessage)
                        '---
                    Else
                        '---
                        ' Inserimento in centrale tramite query diretta
                        '---
                        allOk = Me.InserimentoInCentrale(paz, strAddetto, strEnte, strErrMessage, strNewCode)
                        '---
                    End If

                    result.PazCodice = strNewCode

                    If Not allOk Then
                        result.generateError(strErrMessage)
                    End If

                Case PazienteCentrale.TipoVariazioneEnum.Modified

                    If paz.NumeroVariazioni > 0 Then

                        If Me.Settings.CENTRALE_WS_XMPI Then
                            '---
                            ' Update utilizzando il servizio di scrittura in centrale specificato nel parametro UrlXmpiService del web.config
                            '---
                            strErrMessage = BizXmpi.update(paz, strEnte, strEnte)
                            allOk = String.IsNullOrEmpty(strErrMessage)
                            '---
                        Else
                            '---
                            ' Update in centrale tramite query diretta
                            '---
                            allOk = Me.ModificaInCentrale(paz, strAddetto, strEnte, strErrMessage)
                            '---
                        End If

                    Else
                        allOk = True
                        result.Message = "Nessun dato in centrale è stato modificato. Salvataggio non effettuato."
                    End If

                    If Not allOk Then
                        result.generateError(strErrMessage)
                    End If

                Case PazienteCentrale.TipoVariazioneEnum.Deleted
                    result.generateError("L'operazione di eliminazione in centrale non è consentita.")

                Case Else

            End Select

        End If

        'output dell'errore sul parametro
        Return result

    End Function

    Public Sub SalvaMalattie(pazCodice As String, dtaMalattie As dsMalattie.MalattieDataTable)

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions)

            Using dam As IDAM = GetDAMCentrale()

                dam.QB.NewQuery()
                dam.QB.AddTables("T_PAZ_MALATTIE")
                dam.QB.AddWhereCondition("PMA_PAZ_CODICE", Comparatori.Uguale, pazCodice, DataTypes.Stringa)
                dam.ExecNonQuery(ExecQueryType.Delete)

                For i As Integer = 0 To dtaMalattie.Rows.Count - 1

                    dam.QB.NewQuery()
                    dam.QB.AddTables("T_PAZ_MALATTIE")
                    dam.QB.AddInsertField("PMA_PAZ_CODICE", pazCodice, DataTypes.Stringa)
                    dam.QB.AddInsertField("PMA_MAL_CODICE", " ", DataTypes.Stringa)
                    dam.QB.AddInsertField("PMA_N_MALATTIA", "0", DataTypes.Numero)
                    dam.QB.AddInsertField("PMA_FOLLOW_UP", "S", DataTypes.Stringa)
                    dam.QB.AddInsertField("PMA_N_BILANCIO_PARTENZA", "0", DataTypes.Numero)
                    dam.QB.AddInsertField("PMA_NUOVA_DIAGNOSI", "S", DataTypes.Stringa)
                    dam.QB.AddInsertField("PMA_DATA_DIAGNOSI", New Date(1900, 1, 1), DataTypes.Data)
                    dam.QB.AddInsertField("PMA_DATA_ULTIMA_VISITA", New Date(1900, 1, 1), DataTypes.Data)

                    dam.QB.ChangeValue(1, ExecQueryType.Insert, dtaMalattie.Rows(i).Item("PMA_MAL_CODICE"))
                    dam.QB.ChangeValue(2, ExecQueryType.Insert, Val(dtaMalattie.Rows(i).Item("PMA_N_MALATTIA")))
                    dam.QB.ChangeValue(3, ExecQueryType.Insert, dtaMalattie.Rows(i).Item("PMA_FOLLOW_UP").ToString())
                    dam.QB.ChangeValue(4, ExecQueryType.Insert, Val(IIf(dtaMalattie.Rows(i).Item("PMA_N_BILANCIO_PARTENZA") Is DBNull.Value, "0", dtaMalattie.Rows(i).Item("PMA_N_BILANCIO_PARTENZA"))))
                    dam.QB.ChangeValue(5, ExecQueryType.Insert, dtaMalattie.Rows(i).Item("PMA_NUOVA_DIAGNOSI").ToString())
                    dam.QB.ChangeValue(6, ExecQueryType.Insert, IIf(dtaMalattie.Rows(i).Item("PMA_DATA_DIAGNOSI") Is DBNull.Value, "", dtaMalattie.Rows(i).Item("PMA_DATA_DIAGNOSI")))
                    dam.QB.ChangeValue(7, ExecQueryType.Insert, IIf(dtaMalattie.Rows(i).Item("PMA_DATA_ULTIMA_VISITA") Is DBNull.Value, "", dtaMalattie.Rows(i).Item("PMA_DATA_ULTIMA_VISITA")))
                    dam.ExecNonQuery(ExecQueryType.Insert)

                Next

            End Using

            transactionScope.Complete()

        End Using

    End Sub

    ''' <summary>
    ''' Restituisce true se il codice fiscale passato come argomento è univoco, altrimenti restituisce false.
    ''' </summary>
    ''' <param name="codiceFiscalePaziente">Codice Fiscale del paziente da modificare o da inserire</param>
    ''' <param name="errorMessage">Stringa di errore riempita nel caso in cui il metodo ritorni false</param>
    ''' <param name="codicePaziente"></param>
    Public Function UniCF(codiceFiscalePaziente As String, ByRef errorMessage As String, codicePaziente As String) As Boolean

        Dim dam As IDAM = Nothing

        Try
            dam = GetDAMCentrale()

            dam.QB.NewQuery()
            dam.QB.AddSelectFields("count(paz_codice)")
            dam.QB.AddTables("T_PAZ_PAZIENTI")
            dam.QB.AddWhereCondition("paz_codice_fiscale", Comparatori.Uguale, codiceFiscalePaziente, DataTypes.Stringa)
            If Not String.IsNullOrEmpty(codicePaziente) Then
                dam.QB.AddWhereCondition("paz_codice", Comparatori.Diverso, codicePaziente, DataTypes.Stringa)
            End If

            Dim ris As Object = dam.ExecScalar()

            If ris > 0 Then
                'il codice fiscale modificato appartiene già ad un altro paziente
                errorMessage = "**Codice non univoco**"
                Return False
            End If

        Catch exc As Exception

            errorMessage = exc.Message
            Return False

        Finally

            If Not dam Is Nothing Then dam.Dispose()

        End Try

        errorMessage = String.Empty
        Return True

    End Function

    ''' <summary>
    ''' Controlla l'univocità della tessera sanitaria.
    ''' </summary>
    ''' <param name="tessera">tessera sanitaria da verificare</param>
    ''' <param name="errorMessage">eventuale messaggio di errore ritornato</param>
    ''' <param name="codicePaziente">codice del record paziente da escludere dalla ricerca della tessera</param>
    ''' <returns></returns>
    ''' <remarks>In caso di errore viene restituito False.</remarks>
    Public Function UniTessera(tessera As String, ByRef errorMessage As String, codicePaziente As String) As Boolean

        ' Se tessera non valorizzata -> nessun controllo
        If String.IsNullOrEmpty(tessera) Then
            errorMessage = String.Empty
            Return True
        End If

        Dim dam As IDAM = Nothing

        Try
            dam = GetDAMCentrale()

            dam.QB.NewQuery()
            dam.QB.AddSelectFields("count(paz_codice)")
            dam.QB.AddTables("T_PAZ_PAZIENTI")
            dam.QB.AddWhereCondition("paz_tessera", Comparatori.Uguale, tessera, DataTypes.Stringa)
            If Not String.IsNullOrEmpty(codicePaziente) Then
                dam.QB.AddWhereCondition("paz_codice", Comparatori.Diverso, codicePaziente, DataTypes.Stringa)
            End If

            Dim ris As Object = dam.ExecScalar()

            If ris > 0 Then

                ' La tessera sanitaria specificata appartiene ad un altro paziente
                errorMessage = "Tessera sanitaria già esistente in anagrafe."
                Return False

            End If

        Catch exc As Exception

            errorMessage = exc.Message
            Return False

        Finally

            If Not dam Is Nothing Then dam.Dispose()

        End Try

        ' Tessera univoca
        errorMessage = String.Empty
        Return True

    End Function

    ''' <summary>
    ''' Funzione copiata da GestionePazientiDatiSanitari
    ''' </summary>
    ''' <param name="pazCodice"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CaricaMalattie(pazCodice As String) As dsMalattie.MalattieDataTable

        Dim dtaMalattie As New dsMalattie.MalattieDataTable()

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions)

            Using dam As IDAM = GetDAMCentrale()

                ' Caricamento malattie del paziente
                With dam.QB

                    .NewQuery()

                    .AddSelectFields("PMA_MAL_CODICE, PMA_N_MALATTIA, MAL_DESCRIZIONE, PMA_FOLLOW_UP, PMA_NUOVA_DIAGNOSI")
                    .AddSelectFields("PMA_DATA_DIAGNOSI, PMA_DATA_ULTIMA_VISITA, PMA_N_BILANCIO_PARTENZA")
                    .AddTables("T_PAZ_MALATTIE, T_ANA_MALATTIE")
                    .AddWhereCondition("MAL_CODICE", Comparatori.Uguale, "PMA_MAL_CODICE", DataTypes.Join)
                    .AddWhereCondition("PMA_PAZ_CODICE", Comparatori.Uguale, pazCodice, DataTypes.Stringa)
                    .AddOrderByFields("PMA_N_MALATTIA")

                End With

                dam.BuildDataTable(dtaMalattie)

                ' CONTROLLO PRESENZA ED IMPOSTAZIONE MALATTIE STANDARD
                ' se non è presente alcuna malattia, occorre impostare il valore "NESSUNA"
                ' [TODO] E' qui che bisognerebbe implementare l'eventuale aggiunta di malattie standard
                If dtaMalattie.Rows.Count = 0 Then

                    Dim r As DataRow

                    ' Lettura descrizione malattia
                    Dim descrizioneNessunaMalattia As String = String.Empty

                    With dam.QB
                        .NewQuery()
                        .AddSelectFields("MAL_DESCRIZIONE")
                        .AddTables("T_ANA_MALATTIE")
                        .AddWhereCondition("MAL_CODICE", Comparatori.Uguale, Me.Settings.CODNOMAL, DataTypes.Stringa)
                    End With

                    Using idr As IDataReader = dam.BuildDataReader()

                        If Not idr Is Nothing Then

                            If idr.Read() Then descrizioneNessunaMalattia = idr(0).ToString()

                        End If

                    End Using

                    ' Aggiunta della malattia al datatable delle malattie del paziente
                    r = dtaMalattie.NewRow()

                    r.Item("PMA_MAL_CODICE") = Me.Settings.CODNOMAL
                    r.Item("PMA_N_MALATTIA") = "1"
                    r.Item("MAL_DESCRIZIONE") = descrizioneNessunaMalattia
                    r.Item("PMA_FOLLOW_UP") = "S"

                    dtaMalattie.Rows.Add(r)

                    ' Inserimento su db: l'inserimento deve essere eseguito solamente se il codice paziente è già impostato (modifica 13/07/2004)
                    If Not String.IsNullOrEmpty(pazCodice) Then

                        ' Memorizzazione della malattia nel database
                        With dam.QB
                            .NewQuery()
                            .AddTables("T_PAZ_MALATTIE")
                            .AddInsertField("PMA_MAL_CODICE", Me.Settings.CODNOMAL, DataTypes.Stringa)
                            .AddInsertField("PMA_N_MALATTIA", "1", DataTypes.Numero)
                            .AddInsertField("PMA_PAZ_CODICE", pazCodice, DataTypes.Stringa)
                            .AddInsertField("PMA_FOLLOW_UP", "S", DataTypes.Stringa)
                        End With

                        dam.ExecNonQuery(ExecQueryType.Insert)

                    End If

                End If

            End Using

            transactionScope.Complete()

        End Using

        Return dtaMalattie

    End Function

#Region " Ricerca "

    Public Function RicercaPerCodiceInCentrale(strCodice As String, ByRef ds As System.Data.DataSet, ByRef strErrMsg As String) As Boolean

        Return RicercaInCentrale(strCodice, "", "", "",
                                 "", "", "", "",
                                 "", "", "", "",
                                 "", "", "", "",
                                 "", "", "", "",
                                 "", "", "", "",
                                 "", Nothing, strErrMsg, ds)

    End Function

    Public Function RicercaInCentrale(strCodice As String, strCognome As String, strNome As String, strCodFis As String,
                                      strTessera As String, strDatNas As String, strComCodNas As String, strSesso As String,
                                      strComCodRes As String, strIndRes As String, strCapRes As String, strComCodDom As String,
                                      strIndDom As String, strCapDom As String, strUslCodRes As String, strCitCod As String,
                                      strTel1 As String, strTel2 As String, strTel3 As String, strMedCodBas As String,
                                      strNote As String, strAnonimo As String, strDatIns As String, strDatDec As String,
                                      strDatCes As String, anno As Integer?, ByRef strErrMsg As String, ByRef dstPazienti As System.Data.DataSet) As Boolean

        ' Inizializza variabili di ritorno
        ' blnRitorno = True  --> comunicazione andata a buon fine
        ' blnRitorno = False  --> comunicazione fallita per qualche motivo
        Dim blnRitorno As Boolean = False

        ' Inizializza/resetta i parametri di output
        dstPazienti.Clear()
        strErrMsg = ""

        ' Crea il DAM per la connessione al database
        Using dam As IDAM = GetDAMCentrale()

            ' SELEZIONE (la modifica con IsNull serve per evitare l'errore oracle ora-01405)
            dam.QB.NewQuery()

            dam.QB.AddSelectFields("a.*")
            dam.QB.AddSelectFields(dam.QB.FC.IsNull("MED_NOMINATIVO", "null", DataTypes.Replace) + " as des_med_base")

            'CAMPI RISERVATI ALLA DECRIZIONE

            ' usl
            dam.QB.AddSelectFields("t_ana_usl_ass.usl_descrizione as paz_usl_assistenza_des")
            dam.QB.AddSelectFields("t_ana_usl_res.usl_descrizione as paz_usl_codice_residenza_des")
            dam.QB.AddSelectFields("t_ana_usl_imm.usl_descrizione as paz_usl_imm_des")

            Dim strFnStato As String = String.Empty
            Dim strFnCIstat As String = String.Empty

            ' residenza
            strFnStato = fc_decode(dam.QB.FC, "t_ana_comuni_res.com_tipo", "'S'", "substr(t_ana_comuni_res.com_istat,4)", "'100'")
            dam.QB.AddSelectFields("t_ana_comuni_res.com_descrizione as paz_com_des_residenza",
                                   "t_ana_comuni_res.com_provincia as paz_provincia_residenza",
                                   strFnStato + " as paz_stato_residenza")
            dam.QB.AddSelectFields("t_ana_comuni_res.com_istat as paz_com_istat_residenza")

            ' nascita
            strFnStato = fc_decode(dam.QB.FC, "t_ana_comuni_nas.com_tipo", "'S'", "substr(t_ana_comuni_nas.com_istat,4)", "'100'")
            strFnCIstat = fc_decode(dam.QB.FC, "t_ana_comuni_nas.com_codice_regionale", "'000000'", "t_ana_comuni_nas.com_istat", "t_ana_comuni_nas.com_codice_regionale")
            dam.QB.AddSelectFields("t_ana_comuni_nas.com_descrizione as paz_com_des_nascita",
                                   strFnStato + " as paz_stato_nascita",
                                   strFnCIstat + " as paz_com_istat_nascita")
            ' domicilio
            strFnStato = dam.QB.FC.Switch("t_ana_comuni_dom.com_tipo", "'S'", "substr(t_ana_comuni_dom.com_istat,4)", "'C'", "'100'", "NULL")
            strFnCIstat = "t_ana_comuni_dom.com_istat"
            dam.QB.AddSelectFields("t_ana_comuni_dom.com_descrizione as paz_com_des_domicilio",
                                   "t_ana_comuni_dom.com_provincia as paz_provincia_domicilio",
                                   strFnStato + " as paz_stato_domicilio")
            dam.QB.AddSelectFields(strFnCIstat + " as paz_com_istat_domicilio")

            ' domicilio sanitario
            dam.QB.AddSelectFields("t_ana_comuni_dom_san.com_descrizione as paz_com_des_domicilio_san")

            ' cittadinanza
            dam.QB.AddSelectFields("T_ANA_CITTADINANZE" + ".cit_stato as paz_cit_des")
            dam.QB.AddSelectFields("T_ANA_CITTADINANZE" + ".cit_istat as paz_cit_codice_istat")

            ' medico
            dam.QB.AddSelectFields("MED_CODICE_FISCALE as paz_med_cf_base", "MED_COD_REGIONALE" + "  as paz_med_reg_base")

            dam.QB.AddTables("T_PAZ_PAZIENTI a")

            'FILTRI DI RICERCA

            'ricerca per codice
            If Not strCodice Is Nothing AndAlso strCodice <> "" Then
                dam.QB.AddWhereCondition("paz_codice", Comparatori.Uguale, strCodice, DataTypes.Stringa)
            End If

            ' Se l'anagrafe è gestito con cognome e nome paziente separati    
            If Me.Settings.CENTRALE_SEPATIPO = Enumerators.CentraleTipoAnagrafe.COGNOME_NOME_SEPARATI Then
                If Not strCognome Is Nothing And strCognome <> "" Then
                    dam.QB.AddWhereCondition("paz_cognome", Comparatori.Like, strCognome & "%", DataTypes.Stringa)
                End If
                If Not strNome Is Nothing And strNome <> "" Then
                    dam.QB.AddWhereCondition("paz_nome", Comparatori.Like, strNome & "%", DataTypes.Stringa)
                End If
            ElseIf Me.Settings.CENTRALE_SEPATIPO = Enumerators.CentraleTipoAnagrafe.COGNOME_NOME_UNITI Then
                If (Not strCognome Is Nothing AndAlso strCognome <> String.Empty) Or (Not strNome Is Nothing AndAlso strNome <> String.Empty) Then
                    dam.QB.AddWhereCondition("paz_cognome_nome", Comparatori.Like, strCognome + "%" + Me.Settings.CENTRALE_SEPAANAG + strNome + "%", DataTypes.Stringa)
                End If
            End If
            If Not strCodFis Is Nothing AndAlso strCodFis <> "" Then
                dam.QB.AddWhereCondition("paz_codice_fiscale", Comparatori.Like, strCodFis + "%", DataTypes.Stringa)
            End If
            If Not strTessera Is Nothing AndAlso strTessera <> "" Then
                dam.QB.AddWhereCondition("paz_tessera", Comparatori.Like, strTessera + "%", DataTypes.Stringa)
            End If
            If Not strDatNas Is Nothing AndAlso strDatNas <> "" Then
                dam.QB.AddWhereCondition("paz_data_nascita", Comparatori.Uguale, strDatNas, DataTypes.Data)
            End If
            If Not strSesso Is Nothing AndAlso strSesso <> "" Then
                dam.QB.AddWhereCondition("paz_sesso", Comparatori.Uguale, strSesso, DataTypes.Stringa)
            End If
            If Not strIndRes Is Nothing AndAlso strIndRes <> "" Then
                dam.QB.AddWhereCondition("paz_indirizzo_residenza", Comparatori.Uguale, strIndRes, DataTypes.Stringa)
            End If
            If Not strComCodDom Is Nothing AndAlso strComCodDom <> "" Then
                dam.QB.AddWhereCondition("paz_com_codice_domicilio", Comparatori.Uguale, strComCodDom, DataTypes.Stringa)
            End If
            If Not strIndDom Is Nothing AndAlso strIndDom <> "" Then
                dam.QB.AddWhereCondition("paz_indirizzo_domicilio", Comparatori.Uguale, strIndDom, DataTypes.Stringa)
            End If
            If Not strCapDom Is Nothing AndAlso strCapDom <> "" Then
                dam.QB.AddWhereCondition("paz_cap_domicilio", Comparatori.Uguale, strCapDom, DataTypes.Stringa)
            End If
            If Not strCitCod Is Nothing AndAlso strCitCod <> "" Then
                dam.QB.AddWhereCondition("paz_cit_codice", Comparatori.Uguale, strCitCod, DataTypes.Stringa)
            End If
            If Not strUslCodRes Is Nothing AndAlso strUslCodRes <> "" Then
                dam.QB.AddWhereCondition("paz_usl_codice_residenza", Comparatori.Uguale, strUslCodRes, DataTypes.Stringa)
            End If
            If Not strTel1 Is Nothing AndAlso strTel1 <> "" Then
                dam.QB.AddWhereCondition("paz_telefono_1", Comparatori.Uguale, strTel1, DataTypes.Stringa)
            End If
            If Not strTel2 Is Nothing AndAlso strTel2 <> "" Then
                dam.QB.AddWhereCondition("paz_telefono_2", Comparatori.Uguale, strTel2, DataTypes.Stringa)
            End If
            If Not strTel3 Is Nothing AndAlso strTel3 <> "" Then
                dam.QB.AddWhereCondition("paz_telefono_3", Comparatori.Uguale, strTel3, DataTypes.Stringa)
            End If
            If Not strMedCodBas Is Nothing AndAlso strMedCodBas <> "" Then
                ' TODO [profilo-MMG-PLS]: gestire filtro IN per codici medici gruppo
                dam.QB.AddWhereCondition("paz_med_codice_base", Comparatori.Uguale, strMedCodBas, DataTypes.Stringa)
            End If
            If Not strNote Is Nothing AndAlso strNote <> "" Then
                dam.QB.AddWhereCondition("paz_note", Comparatori.Uguale, strNote, DataTypes.Stringa)
            End If
            If Not strAnonimo Is Nothing AndAlso strAnonimo <> "" Then
                dam.QB.AddWhereCondition("paz_anonimo", Comparatori.Uguale, strAnonimo, DataTypes.Stringa)
            End If
            If Not strDatIns Is Nothing AndAlso strDatIns <> "" Then
                dam.QB.AddWhereCondition("paz_data_inserimento", Comparatori.Uguale, strDatIns, DataTypes.Data)
            End If
            If Not strDatDec Is Nothing AndAlso strDatDec <> "" Then
                dam.QB.AddWhereCondition("paz_data_decesso", Comparatori.Uguale, strDatDec, DataTypes.Data)
            End If
            If Not strDatCes Is Nothing AndAlso strDatCes <> "" Then
                dam.QB.AddWhereCondition("paz_data_cessazione_ass", Comparatori.Uguale, strDatCes, DataTypes.Data)
            End If
            If anno.HasValue Then
                dam.QB.AddWhereCondition("paz_data_nascita", Comparatori.MaggioreUguale, New DateTime(anno, 1, 1), DataTypes.Data)
                dam.QB.AddWhereCondition("paz_data_nascita", Comparatori.Minore, New DateTime(anno + 1, 1, 1), DataTypes.Data)
            End If

            'RELAZIONI

            ' join descrizione medico di base 
            dam.QB.AddTables("T_MED_MEDICI")
            dam.QB.AddWhereCondition("paz_med_codice_base", Comparatori.Uguale, "MED_CODICE", DataTypes.OutJoinLeft)

            ' join con la usl assistenza
            dam.QB.AddTables("T_ANA_USL" + " t_ana_usl_ass")
            dam.QB.AddWhereCondition("PAZ_USL_CODICE_ASSISTENZA", Comparatori.Uguale, "t_ana_usl_ass.usl_codice", DataTypes.OutJoinLeft)

            ' join con la usl immigrazione
            dam.QB.AddTables("T_ANA_USL" + " t_ana_usl_imm")
            dam.QB.AddWhereCondition("PAZ_USL_PROVENIENZA", Comparatori.Uguale, "t_ana_usl_imm.usl_codice", DataTypes.OutJoinLeft)

            ' join con la usl residenza
            dam.QB.AddTables("T_ANA_USL" + " t_ana_usl_res")
            dam.QB.AddWhereCondition("paz_usl_codice_residenza", Comparatori.Uguale, "t_ana_usl_res.usl_codice", DataTypes.OutJoinLeft)

            ' join con il comune residenza
            dam.QB.AddTables("T_ANA_COMUNI" + " t_ana_comuni_res")
            dam.QB.AddWhereCondition("paz_com_codice_residenza", Comparatori.Uguale, "t_ana_comuni_res.com_codice", DataTypes.OutJoinLeft)

            ' comune di nascita
            dam.QB.AddTables("T_ANA_COMUNI" + " t_ana_comuni_nas")
            dam.QB.AddWhereCondition("paz_com_codice_nascita", Comparatori.Uguale, "t_ana_comuni_nas.com_codice", DataTypes.OutJoinLeft)

            ' join con il comune DI domicilio
            dam.QB.AddTables("T_ANA_COMUNI" + " t_ana_comuni_dom")
            dam.QB.AddWhereCondition("paz_com_codice_domicilio", Comparatori.Uguale, "t_ana_comuni_dom.com_codice", DataTypes.OutJoinLeft)

            ' join con il comune DI domicilio sanitario
            dam.QB.AddTables("T_ANA_COMUNI" + " t_ana_comuni_dom_san")
            dam.QB.AddWhereCondition("paz_com_codice_domicilio_san", Comparatori.Uguale, "t_ana_comuni_dom_san.com_codice", DataTypes.OutJoinLeft)

            ' cittadinanza
            dam.QB.AddTables("T_ANA_CITTADINANZE")
            dam.QB.AddWhereCondition("paz_cit_codice", Comparatori.Uguale, "T_ANA_CITTADINANZE" + ".cit_codice", DataTypes.OutJoinLeft)

            If Not strComCodNas Is Nothing AndAlso strComCodNas <> "" Then
                dam.QB.AddWhereCondition("paz_com_codice_nascita", Comparatori.Uguale, strComCodNas, DataTypes.Stringa)
            End If
            If Not strComCodRes Is Nothing AndAlso strComCodRes <> "" Then
                dam.QB.AddWhereCondition("paz_com_codice_residenza", Comparatori.Uguale, strComCodRes, DataTypes.Stringa)
            End If

            ' ORDINAMENTO
            dam.QB.AddOrderByFields("paz_cognome", "paz_nome", "paz_data_nascita")

            ' Esegue la query costruita con OnitDAM e inserisce il risultato nella prima Datatable del Dataset 
            ' dstPazienti copiando però solo i primi Maxpazie record
            ' dstPazienti.Tables("t_paz_pazienti_centrale") dovrebbe coincidere con dstPazienti.Tables(0)
            Dim dt As New DataTable()

            If PAZIENTI_MAX < 0 Then
                dam.BuildDataTable(dt)
            Else
                dt.TableName = "t_paz_pazienti_centrale"
                dam.Command.CommandText = dam.QB.GetSelect()
                dt = dam.BuildDataTable(dt, PAZIENTI_MAX)
                If (dt.Rows.Count = PAZIENTI_MAX) Then
                    strErrMsg = "Il numero di pazienti restituiti dall'anagrafe centrale è stato limitato a " + PAZIENTI_MAX.ToString() + ". " + vbCrLf + "Raffinare la ricerca."
                End If
            End If

            Dim dt1 As DataTable = dt.Copy()
            dt = Nothing

            'validazione e normalizzazione dei codici istat
            ValidaCodiciIstat(dt1)
            dstPazienti.Tables.Add(dt1)

            ' OK, nessun problema, aggiorna variabile di ritorno
            blnRitorno = True

        End Using

        Return blnRitorno

    End Function

#End Region

#Region " Inserimento "

    Public Function InserimentoInCentrale(paz As PazienteCentrale, addetto As String, ente As String, ByRef errorMessage As String, ByRef newPazCodice As String) As Boolean

        Dim result As Boolean = False

        ' Inizializzazioni    
        Dim msg As String = ""
        Dim ucf As String = ""

        ' Variabili per poter posticipare la query (in DAM non si può annidarne la creazione)
        Dim strPazComNas As String
        Dim strPazComRes As String
        Dim strPazComDom As String
        Dim strPazUslRes As String
        Dim strPazCodCit As String
        Dim strPazMedBas As String

        Dim strDisCodice As String
        Dim strComCodiceEmigrazione As String
        Dim strComCodiceImmigrazione As String
        Dim strStatoCivile As String
        Dim strMotivoCessazioneAss As String
        Dim strComCodiceDecesso As String
        Dim strCodiceProfessione As String
        Dim strUslCodiceAssistenza As String = Nothing
        Dim strLocalitaDomicilio As String
        Dim strLocalitaResidenza As String
        Dim strNote As String
        Dim strUslProvenienza As String
        Dim strCfisValidato As String
        Dim strCfisCertificatore As String
        Dim strComCodiceDemografico As String
        Dim strCategoriaRschioCodice As String
        Dim blnUCode As Boolean

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions)

            Try
                Using dam As IDAM = GetDAMCentrale()

                    'se il codice viene passato, non viene calcolato
                    If Not paz.codice Is Nothing AndAlso paz.codice <> String.Empty Then
                        blnUCode = Me.CodicePazUnivoco(paz.codice, dam, msg)
                        ucf = paz.codice
                    Else
                        blnUCode = (GenUniCode(paz.Cognome, paz.Nome, paz.DataNascita, paz.ComCodiceNascita, paz.Sesso, dam, msg, ucf))
                    End If

                    ' Se la generazione del codice paziente univoco va a buon fine ...
                    If blnUCode Then

                        ' Se non si deve effettuare nessun controllo di integrità referenziale
                        If Settings.CENTRALE_CHECK_INTEGRITY Then

                            ' Se si effettua il controllo di integrità referenziale
                            strPazComNas = CheckIntegrity(T_ANA_COMUNI, "com_codice", paz.ComCodiceNascita, dam)
                            strPazComRes = CheckIntegrity(T_ANA_COMUNI, "com_codice", paz.ResidenzaComCodice, dam)
                            strPazComDom = CheckIntegrity(T_ANA_COMUNI, "com_codice", paz.DomicilioComCodice, dam)
                            strPazUslRes = CheckIntegrity(T_ANA_USL, "usl_codice", paz.ResidenzaUslCodice, dam)
                            strPazCodCit = CheckIntegrity(T_ANA_CITTADINANZE, "cit_codice", paz.CitCodice, dam)
                            strPazMedBas = CheckIntegrity(T_ANA_MEDICI, "MED_CODICE", paz.MedCodiceBase, dam)

                            'nuovi campi da controllare
                            strDisCodice = CheckIntegrity(T_ANA_DISTRETTI, "dis_codice", paz.DisCodice, dam)
                            strComCodiceEmigrazione = CheckIntegrity(T_ANA_COMUNI, "com_codice", paz.EmigrazioneComCodice, dam)
                            strComCodiceImmigrazione = CheckIntegrity(T_ANA_COMUNI, "com_codice", paz.ImmigrazioneComCodice, dam)
                            strStatoCivile = CheckIntegrity(T_ANA_STATI_CIVILI, "stc_codice", paz.StatoCivile, dam)
                            strMotivoCessazioneAss = CheckIntegrity(T_ANA_MOTIVI_CESSAZ_ASSIST, "MCE_CODICE", paz.UslAssistenzaMotivoCessazione, dam)
                            strComCodiceDecesso = CheckIntegrity(T_ANA_COMUNI, "COM_CODICE", paz.ComCodiceDecesso, dam)
                            strCodiceProfessione = CheckIntegrity(T_ANA_POSIZIONI_PROFESSIONALI, "PFE_CODICE", paz.CodiceProfessione, dam)
                            strLocalitaDomicilio = CheckIntegrity(T_ANA_LOCALITA, "QUA_CODICE", paz.DomicilioLocalita, dam)
                            strLocalitaResidenza = CheckIntegrity(T_ANA_LOCALITA, "QUA_CODICE", paz.ResidenzaLocalita, dam)
                            strNote = paz.Note
                            strUslProvenienza = CheckIntegrity(T_ANA_USL, "usl_codice", paz.UslProvenienza, dam)
                            strCfisValidato = paz.CfisValidato
                            strCfisCertificatore = paz.CfisCertificatore
                            strComCodiceDemografico = CheckIntegrity(T_ANA_COMUNI, "com_codice", paz.ComCodiceDemografico, dam)
                            strCategoriaRschioCodice = CheckIntegrity("T_ANA_RISCHIO", "RSC_CODICE", paz.CategoriaRischioCodice, dam)

                        Else

                            strPazComNas = paz.ComCodiceNascita
                            strPazComRes = paz.ResidenzaComCodice
                            strPazComDom = paz.DomicilioComCodice
                            strPazUslRes = paz.ResidenzaUslCodice
                            strPazCodCit = paz.CitCodice
                            strPazMedBas = paz.MedCodiceBase

                            strDisCodice = paz.DisCodice
                            strComCodiceEmigrazione = paz.EmigrazioneComCodice
                            strComCodiceImmigrazione = paz.ImmigrazioneComCodice
                            strStatoCivile = paz.StatoCivile
                            strMotivoCessazioneAss = paz.UslAssistenzaMotivoCessazione
                            strComCodiceDecesso = paz.ComCodiceDecesso
                            strCodiceProfessione = paz.CodiceProfessione
                            strUslCodiceAssistenza = paz.UslAssistenzaCodice
                            strLocalitaDomicilio = paz.DomicilioLocalita
                            strLocalitaResidenza = paz.ResidenzaLocalita
                            strNote = paz.Note
                            strUslProvenienza = paz.UslProvenienza
                            strCfisValidato = paz.CfisValidato
                            strCfisCertificatore = paz.CfisCertificatore
                            strComCodiceDemografico = paz.ComCodiceDemografico
                            strCategoriaRschioCodice = paz.CategoriaRischioCodice

                        End If

                        ' Crea una nuova query OnitDam
                        dam.QB.NewQuery()
                        dam.QB.AddTables(T_PAZ_PAZIENTI)
                        If Not ucf Is Nothing Then
                            dam.QB.AddInsertField("paz_codice", ucf, DataTypes.Stringa)
                        End If
                        If Not paz.CodiceRegione Is Nothing Then
                            dam.QB.AddInsertField("paz_codice_regionale", paz.CodiceRegione, DataTypes.Stringa)
                        End If
                        dam.QB.AddInsertField("paz_data_inserimento", Strings.Mid(Now.ToString(), 1, 10), DataTypes.Data)

                        ' Se l'anagrafe è gestito con cognome e nome paziente separati
                        If Me.Settings.CENTRALE_SEPATIPO = Enumerators.CentraleTipoAnagrafe.COGNOME_NOME_SEPARATI Then

                            If Not paz.Cognome Is Nothing Then
                                dam.QB.AddInsertField("paz_cognome", paz.Cognome, DataTypes.Stringa)
                            End If
                            If Not paz.Nome Is Nothing Then
                                dam.QB.AddInsertField("paz_nome", paz.Nome, DataTypes.Stringa)
                            End If

                        ElseIf Me.Settings.CENTRALE_SEPATIPO = Enumerators.CentraleTipoAnagrafe.COGNOME_NOME_UNITI Then

                            ' Se l'anagrafe è gestito con cognome e nome paziente uniti
                            ' Solo se almeno uno dei campi Cognome e Nome è non nullo
                            If Not paz.Cognome Is Nothing And Not paz.Nome Is Nothing Then
                                If paz.Cognome <> "" Or paz.Nome <> "" Then
                                    dam.QB.AddInsertField("paz_cognome_nome", paz.Cognome & Me.Settings.CENTRALE_SEPAANAG & paz.Nome, DataTypes.Stringa)
                                Else
                                    dam.QB.AddInsertField("paz_cognome_nome", "", DataTypes.Stringa)
                                End If
                            End If

                        End If

                        If Not paz.codiceFiscale Is Nothing Then
                            dam.QB.AddInsertField("paz_codice_fiscale", paz.codiceFiscale, DataTypes.Stringa)
                        End If
                        If Not paz.Tessera Is Nothing Then
                            dam.QB.AddInsertField("paz_tessera", paz.Tessera, DataTypes.Stringa)
                        End If
                        If Not paz.DataNascita Is Nothing Then
                            dam.QB.AddInsertField("paz_data_nascita", paz.DataNascita, DataTypes.Data)
                        End If
                        If Not strPazComNas Is Nothing Then
                            dam.QB.AddInsertField("paz_com_codice_nascita", strPazComNas, DataTypes.Stringa)
                        End If
                        If Not paz.Sesso Is Nothing Then
                            dam.QB.AddInsertField("paz_sesso", paz.Sesso, DataTypes.Stringa)
                        End If
                        If Not strPazComRes Is Nothing Then
                            dam.QB.AddInsertField("paz_com_codice_residenza", strPazComRes, DataTypes.Stringa)
                        End If
                        If Not paz.ResidenzaIndirizzo Is Nothing Then
                            dam.QB.AddInsertField("paz_indirizzo_residenza", paz.ResidenzaIndirizzo, DataTypes.Stringa)
                        End If
                        If Not paz.ResidenzaCap Is Nothing Then
                            dam.QB.AddInsertField("paz_cap_residenza", paz.ResidenzaCap, DataTypes.Stringa)
                        End If
                        If Not paz.ResidenzaDataInizio Is Nothing Then
                            dam.QB.AddInsertField("paz_data_inizio_residenza", paz.ResidenzaDataInizio, DataTypes.Data)
                        End If
                        If Not paz.ResidenzaDataFine Is Nothing Then
                            dam.QB.AddInsertField("paz_data_fine_residenza", paz.ResidenzaDataFine, DataTypes.Data)
                        End If
                        If Not strPazComDom Is Nothing Then
                            dam.QB.AddInsertField("paz_com_codice_domicilio", strPazComDom, DataTypes.Stringa)
                        End If
                        If Not paz.DomicilioIndirizzo Is Nothing Then
                            dam.QB.AddInsertField("paz_indirizzo_domicilio", paz.DomicilioIndirizzo, DataTypes.Stringa)
                        End If
                        If Not paz.DomicilioCap Is Nothing Then
                            dam.QB.AddInsertField("paz_cap_domicilio", paz.DomicilioCap, DataTypes.Stringa)
                        End If
                        If Not paz.DomicilioDataInizio Is Nothing Then
                            dam.QB.AddInsertField("paz_data_inizio_domicilio", paz.DomicilioDataInizio, DataTypes.Data)
                        End If
                        If Not paz.DomicilioDataFine Is Nothing Then
                            dam.QB.AddInsertField("paz_data_fine_domicilio", paz.DomicilioDataFine, DataTypes.Data)
                        End If
                        If Not strPazUslRes Is Nothing Then
                            dam.QB.AddInsertField("paz_usl_codice_residenza", strPazUslRes, DataTypes.Stringa)
                        End If
                        If Not strPazCodCit Is Nothing Then
                            dam.QB.AddInsertField("paz_cit_codice", strPazCodCit, DataTypes.Stringa)
                        End If
                        If Not paz.Telefono1 Is Nothing Then
                            dam.QB.AddInsertField("paz_telefono_1", paz.Telefono1, DataTypes.Stringa)
                        End If
                        If Not paz.Telefono2 Is Nothing Then
                            dam.QB.AddInsertField("paz_telefono_2", paz.Telefono2, DataTypes.Stringa)
                        End If
                        If Not paz.Telefono3 Is Nothing Then
                            dam.QB.AddInsertField("paz_telefono_3", paz.Telefono3, DataTypes.Stringa)
                        End If
                        If Not strPazMedBas Is Nothing Then
                            dam.QB.AddInsertField("paz_med_codice_base", strPazMedBas, DataTypes.Stringa)
                        End If
                        If Not paz.DataDecesso Is Nothing Then
                            dam.QB.AddInsertField("paz_data_decesso", paz.DataDecesso, DataTypes.Data)
                        End If
                        If Not strUslCodiceAssistenza Is Nothing Then
                            dam.QB.AddInsertField("PAZ_USL_CODICE_ASSISTENZA", strUslCodiceAssistenza, DataTypes.Stringa)
                        End If
                        If Not paz.UslAssistenzaDataInizio Is Nothing Then
                            dam.QB.AddInsertField("paz_data_inizio_ass", paz.UslAssistenzaDataInizio, DataTypes.Data)
                        End If
                        If Not paz.UslAssistenzaDataCessazione Is Nothing Then
                            dam.QB.AddInsertField("paz_data_cessazione_ass", paz.UslAssistenzaDataCessazione, DataTypes.Data)
                        End If
                        If Not strMotivoCessazioneAss Is Nothing Then
                            dam.QB.AddInsertField("PAZ_MOTIVO_CESSAZIONE_ASS", strMotivoCessazioneAss, DataTypes.Stringa)
                        End If
                        If Not strDisCodice Is Nothing Then
                            dam.QB.AddInsertField("PAZ_DIS_CODICE", strDisCodice, DataTypes.Stringa)
                        End If
                        If Not paz.EmigrazioneData Is Nothing Then
                            dam.QB.AddInsertField("PAZ_DATA_EMIGRAZIONE", paz.EmigrazioneData, DataTypes.Data)
                        End If
                        If Not strComCodiceEmigrazione Is Nothing Then
                            dam.QB.AddInsertField("PAZ_COM_CODICE_EMIGRAZIONE", strComCodiceEmigrazione, DataTypes.Stringa)
                        End If
                        If Not paz.ImmigrazioneData Is Nothing Then
                            dam.QB.AddInsertField("PAZ_DATA_IMMIGRAZIONE", paz.ImmigrazioneData, DataTypes.Data)
                        End If
                        If Not strComCodiceImmigrazione Is Nothing Then
                            dam.QB.AddInsertField("PAZ_COM_CODICE_IMMIGRAZIONE", strComCodiceImmigrazione, DataTypes.Stringa)
                        End If
                        If Not strStatoCivile Is Nothing Then
                            dam.QB.AddInsertField("PAZ_STATO_CIVILE", strStatoCivile, DataTypes.Stringa)
                        End If
                        If Not strComCodiceDecesso Is Nothing Then
                            dam.QB.AddInsertField("PAZ_COM_CODICE_DECESSO", strComCodiceDecesso, DataTypes.Stringa)
                        End If
                        If Not strCodiceProfessione Is Nothing Then
                            dam.QB.AddInsertField("PAZ_CODICE_PROFESSIONE", strCodiceProfessione, DataTypes.Stringa)
                        End If
                        If Not paz.CodiceDemografico Is Nothing Then
                            dam.QB.AddInsertField("PAZ_CODICE_DEMOGRAFICO", paz.CodiceDemografico, DataTypes.Stringa)
                        End If
                        If Not paz.CEE Is Nothing Then
                            dam.QB.AddInsertField("PAZ_CEE", paz.CEE, DataTypes.Stringa)
                        End If
                        If Not strLocalitaDomicilio Is Nothing Then
                            dam.QB.AddInsertField("PAZ_LOCALITA_DOMICILIO", strLocalitaDomicilio, DataTypes.Stringa)
                        End If
                        If Not strLocalitaResidenza Is Nothing Then
                            dam.QB.AddInsertField("PAZ_LOCALITA_RESIDENZA", strLocalitaResidenza, DataTypes.Stringa)
                        End If
                        If Not strUslProvenienza Is Nothing Then
                            dam.QB.AddInsertField("PAZ_USL_PROVENIENZA", strUslProvenienza, DataTypes.Stringa)
                        End If
                        If Not strNote Is Nothing Then
                            dam.QB.AddInsertField("PAZ_NOTE", strNote, DataTypes.Stringa)
                        End If
                        If Not strCfisValidato Is Nothing Then
                            dam.QB.AddInsertField("paz_cfis_validato", strCfisValidato, DataTypes.Stringa)
                        End If
                        If Not strCfisCertificatore Is Nothing Then
                            dam.QB.AddInsertField("paz_cfis_certificatore", strCfisCertificatore, DataTypes.Stringa)
                        End If
                        If Not strComCodiceDemografico Is Nothing Then
                            dam.QB.AddInsertField("paz_com_codice_demografico", strComCodiceDemografico, DataTypes.Stringa)
                        End If
                        If paz.DataVariazioneComunale <> Nothing Then
                            dam.QB.AddInsertField("paz_data_variazione_comune", paz.DataVariazioneComunale, DataTypes.Data)
                        End If
                        If Not paz.TeamEmissione Is Nothing Then
                            dam.QB.AddInsertField("paz_team_emissione", paz.TeamEmissione, DataTypes.Stringa)
                        End If
                        If Not paz.Tipo Is Nothing Then
                            dam.QB.AddInsertField("paz_tipo", paz.Tipo, DataTypes.Stringa)
                        Else
                            dam.QB.AddInsertField("paz_tipo", "P", DataTypes.Stringa)
                        End If
                        If Not strCategoriaRschioCodice Is Nothing Then
                            dam.QB.AddInsertField("paz_rsc_codice", strCategoriaRschioCodice, DataTypes.Stringa)
                        End If

                        ' Esegue il comando SQL di inserimento
                        dam.ExecNonQuery(ExecQueryType.Insert)

                        ' Inserimento avvenuto correttamente; altrimenti il controllo sarebbe stato intercettato dalla try. 
                        ' Prima di eseguire la commit occorre inserire un record delle variazioni nella tabella dello storico
                        If Settings.CENTRALE_STORVAR Then
                            InserisciRecordInsInStoricoVariazioni(ucf, addetto, ente)
                        End If

                        ' Imposta il codice paziente che il metodo deve ritornare
                        newPazCodice = ucf

                        result = True

                    Else
                        ' Se si è verificato un errore nel generare un codice paziente univoco...
                        errorMessage = "Impossibile generare il codice univoco per il paziente" & vbNewLine & msg
                        result = False
                    End If

                End Using

            Catch ex As Exception
                Common.Utility.EventLogHelper.EventLogWrite(ex, "Eccezione BizPazienteCentrale.InserimentoInCentrale", EventLogEntryType.Information, ContextInfos.IDApplicazione)
                errorMessage = String.Format("CF PAZ: {0} - Eccezione: {1}", If(paz.codiceFiscale, String.Empty), ex.Message)
                result = False
            End Try

            transactionScope.Complete()

        End Using

        Return result

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Questa funzione si occupa di inserire un record inserimento nella tabella che memorizza lo storico delle variazioni.
    ''' Restituisce True se tutto è andato a buon fine, False altrimenti.
    ''' </summary>
    ''' <param name="codicePaziente">codice dell'anagrafe centrale associato al paziente di cui si vuole mantenere lo storico</param>
    ''' <param name="addetto">identificativo dell'addetto che ha effettuato l'inserimento del paziente nell'anagrafe centrale</param>
    ''' <param name="ente">ente che ha effettuato l'inserimento</param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adesimone]	10/06/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Sub InserisciRecordInsInStoricoVariazioni(codicePaziente As String, addetto As String, ente As String)

        ' Crea il DAM per la connessione al database
        Using dam As IDAM = GetDAMStoricoVar()
            dam.QB.NewQuery()
            dam.QB.AddTables(T_PAZ_STORICO_VARIAZIONI)
            dam.QB.AddInsertField("var_addetto", Strings.Mid(addetto, 1, 50), DataTypes.Stringa)
            dam.QB.AddInsertField("var_data_registrazione", Format(Now(), "yyyyMMdd HH:mm:ss").Replace(".", ":"), DataTypes.Stringa)
            dam.QB.AddInsertField("var_ente", Strings.Mid(ente, 1, 30), DataTypes.Stringa)
            dam.QB.AddInsertField("var_tipo", "I", DataTypes.Stringa)
            dam.QB.AddInsertField("paz_codice", codicePaziente, DataTypes.Stringa)
            dam.ExecNonQuery(ExecQueryType.Insert)
        End Using

    End Sub

#End Region

#Region " Modifica "

    Private Function FillPatientArray(pazCodice As String, ByRef fieldName As Array, ByRef oldRecord As Array, ByRef newRecord As Array, ByRef errorMessage As String) As Boolean

        Dim result As Boolean = False

        Using dam As IDAM = GetDAMCentrale()

            ' Carica in OldRecord il record che si andrà a modificare *******************
            dam.QB.NewQuery()
            dam.QB.AddSelectFields("*")
            dam.QB.AddTables(T_PAZ_PAZIENTI)
            dam.QB.AddWhereCondition("paz_codice", Comparatori.Uguale, pazCodice, DataTypes.Stringa)

            Using odr As IDataReader = dam.BuildDataReader()

                If odr.Read() Then

                    oldRecord = Array.CreateInstance(GetType(String), odr.FieldCount)
                    newRecord = Array.CreateInstance(GetType(String), odr.FieldCount)
                    fieldName = Array.CreateInstance(GetType(String), odr.FieldCount)

                    For i As Integer = fieldName.GetLowerBound(0) To fieldName.GetUpperBound(0)
                        fieldName.SetValue(odr.GetName(i), i)
                        oldRecord.SetValue(odr(i).ToString(), i)
                    Next

                    result = True

                Else

                    errorMessage = "Il paziente con codice " + pazCodice + " non esiste in anagrafe."
                    result = False

                End If

            End Using

        End Using

        Return result

    End Function

    Private Sub CheckFlags(ByRef paz As PazienteCentrale, ByRef flagModifica As Boolean, ByRef flagPazTipo As Boolean)

        Using dam As IDAM = GetDAMCentrale()

            Dim strCampFon As String = Me.Settings.CENTRALE_CAMPIFOND

            ' Cognome e nome separati
            If Me.Settings.CENTRALE_SEPATIPO = Enumerators.CentraleTipoAnagrafe.COGNOME_NOME_SEPARATI Then
                ' Verifica se il campo cognome è settato a nothing: in questo caso nessuna modifica
                If Not paz.Cognome Is Nothing Then
                    ' Ricorda che vanno effettuate modifiche
                    flagModifica = True
                    ' Controlla se il campo paz_cognome appartiene alla lista dei campi fondamentali
                    If Not flagPazTipo Then
                        flagPazTipo = RiempiPazTipo(strCampFon, "paz_cognome")
                    End If
                End If

                ' Verifica se il campo nome è settato a nothing: in questo caso nessuna modifica
                If Not paz.Nome Is Nothing Then
                    ' Ricorda che vanno effettuate modifiche
                    flagModifica = True
                    ' Controlla se il campo paz_cognome appartiene alla lista dei campi fondamentali
                    If Not flagPazTipo Then
                        flagPazTipo = RiempiPazTipo(strCampFon, "paz_nome")
                    End If
                End If
            ElseIf Me.Settings.CENTRALE_SEPATIPO = Enumerators.CentraleTipoAnagrafe.COGNOME_NOME_UNITI Then
                ' Cognome e nome uniti separati dal Sepaanag            
                ' Se il campo cognome o il campo nome non è settato a nothing, allora occorre effettuare una modifica
                If Not paz.Cognome Is Nothing Or Not paz.Nome Is Nothing Then
                    ' Ricorda che vanno effettuate modifiche
                    flagModifica = True
                    ' Controlla se il campo paz_cognome_nome appartiene alla lista dei campi fondamentali
                    If Not flagPazTipo Then
                        flagPazTipo = RiempiPazTipo(strCampFon, "paz_cognome_nome")
                    End If
                End If
            End If

            ' Controlla se il campo CodFis è nothing; in questo caso la modifica non va effettuata
            If Not paz.codiceFiscale Is Nothing Then
                ' Ricorda che vanno effettuate modifiche
                flagModifica = True
                ' Controlla se il campo codice fiscale appartiene alla lista dei campi fondamentali
                If Not flagPazTipo Then
                    flagPazTipo = RiempiPazTipo(strCampFon, "paz_codice_fiscale")
                End If
            End If

            ' La tessera va trattata come un campo normale, ed in particolare va effettuata
            ' la modifica in centrale
            ' Controlla se il campo Tess è nothing; in questo caso la modifica non va effettuata
            If Not paz.Tessera Is Nothing Then
                ' Ricorda che vanno effettuate modifiche
                flagModifica = True
                ' Controlla se il campo tessera sanitaria appartiene alla lista dei campi fondamentali
                If Not flagPazTipo Then
                    flagPazTipo = RiempiPazTipo(strCampFon, "paz_tessera")
                End If
            End If

            If Not paz.CodiceRegione Is Nothing Then
                ' Ricorda che vanno effettuate modifiche
                flagModifica = True
                ' Controlla se il campo tessera sanitaria appartiene alla lista dei campi fondamentali
                If Not flagPazTipo Then
                    flagPazTipo = RiempiPazTipo(strCampFon, "paz_codice_regionale")
                End If
            End If

            If Not paz.CodiceAzienda Is Nothing Then
                ' Ricorda che vanno effettuate modifiche
                flagModifica = True
                ' Controlla se il campo tessera sanitaria appartiene alla lista dei campi fondamentali
                If Not flagPazTipo Then
                    flagPazTipo = RiempiPazTipo(strCampFon, "paz_codice_ausiliario")
                End If
            End If

            ' Controlla se il campo DataNascita è nothing; in questo caso la modifica non va effettuata
            If Not paz.DataNascita Is Nothing Then
                ' Ricorda che vanno effettuate modifiche
                flagModifica = True
                ' Controlla se il campo data di nascita appartiene alla lista dei campi fondamentali
                If Not flagPazTipo Then
                    flagPazTipo = RiempiPazTipo(strCampFon, "paz_data_nascita")
                End If
            End If

            ' Controlla se il campo ComCodNas è nothing; in questo caso la modifica non va effettuata
            If Not paz.ComCodiceNascita Is Nothing Then
                ' Ricorda che vanno effettuate modifiche
                flagModifica = True
                ' Controlla se il campo codice del comune di nascita appartiene alla lista dei campi fondamentali
                If Not flagPazTipo Then
                    flagPazTipo = RiempiPazTipo(strCampFon, "paz_com_codice_nascita")
                End If
            End If

            ' Controlla se il campo Sesso è nothing; in questo caso la modifica non va effettuata
            If Not paz.Sesso Is Nothing Then
                ' Ricorda che vanno effettuate modifiche
                flagModifica = True
                ' Controlla se il campo sesso appartiene alla lista dei campi fondamentali
                If Not flagPazTipo Then
                    flagPazTipo = RiempiPazTipo(strCampFon, "paz_sesso")
                End If
            End If

            ' Controlla se il campo ComCodiceResidenza è nothing; in questo caso la modifica non va effettuata
            If Not paz.ResidenzaComCodice Is Nothing Then
                ' Ricorda che vanno effettuate modifiche
                flagModifica = True
                ' Controlla se il campo codice del comune di residenza appartiene alla lista dei campi fondamentali
                If Not flagPazTipo Then
                    flagPazTipo = RiempiPazTipo(strCampFon, "paz_com_codice_residenza")
                End If
            End If

            ' Controlla se il campo IndRes è nothing; in questo caso la modifica non va effettuata
            If Not paz.ResidenzaIndirizzo Is Nothing Then
                ' Ricorda che vanno effettuate modifiche
                flagModifica = True
                ' Controlla se il campo indirizzo di residenza appartiene alla lista dei campi fondamentali
                If Not flagPazTipo Then
                    flagPazTipo = RiempiPazTipo(strCampFon, "paz_indirizzo_residenza")
                End If
            End If

            ' Controlla se il campo CapRes è nothing; in questo caso la modifica non va effettuata
            If Not paz.ResidenzaCap Is Nothing Then
                ' Ricorda che vanno effettuate modifiche
                flagModifica = True
                ' Controlla se il campo CAP di residenza appartiene alla lista dei campi fondamentali
                If Not flagPazTipo Then
                    flagPazTipo = RiempiPazTipo(strCampFon, "paz_cap_residenza")
                End If
            End If

            If Not paz.ResidenzaDataInizio Is Nothing Then
                flagModifica = True
                If Not flagPazTipo Then flagPazTipo = RiempiPazTipo(strCampFon, "paz_data_inizio_residenza")
            End If

            If Not paz.ResidenzaDataFine Is Nothing Then
                flagModifica = True
                If Not flagPazTipo Then flagPazTipo = RiempiPazTipo(strCampFon, "paz_data_fine_residenza")
            End If

            ' Controlla se il campo CodComDom è nothing; in questo caso la modifica non va effettuata
            If Not paz.DomicilioComCodice Is Nothing Then
                ' Ricorda che vanno effettuate modifiche
                flagModifica = True
                ' Controlla se il campo codice del comune di domicilio appartiene alla lista dei campi fondamentali
                If Not flagPazTipo Then
                    flagPazTipo = RiempiPazTipo(strCampFon, "paz_com_codice_domicilio")
                End If
            End If

            ' Controlla se il campo IndDom è nothing; in questo caso la modifica non va effettuata
            If Not paz.DomicilioIndirizzo Is Nothing Then
                ' Ricorda che vanno effettuate modifiche
                flagModifica = True
                ' Controlla se il campo indirizzo di domicilio appartiene alla lista dei campi fondamentali
                If Not flagPazTipo Then
                    flagPazTipo = RiempiPazTipo(strCampFon, "paz_indirizzo_domicilio")
                End If
            End If

            ' Controlla se il campo CapDom è nothing; in questo caso la modifica non va effettuata
            If Not paz.DomicilioCap Is Nothing Then
                ' Ricorda che vanno effettuate modifiche
                flagModifica = True
                ' Controlla se il campo CAP domicilio appartiene alla lista dei campi fondamentali
                If (Not flagPazTipo) Then
                    flagPazTipo = RiempiPazTipo(strCampFon, "paz_cap_domicilio")
                End If
            End If

            If Not paz.DomicilioDataInizio Is Nothing Then
                flagModifica = True
                If Not flagPazTipo Then flagPazTipo = RiempiPazTipo(strCampFon, "paz_data_inizio_domicilio")
            End If

            If Not paz.DomicilioDataFine Is Nothing Then
                flagModifica = True
                If Not flagPazTipo Then flagPazTipo = RiempiPazTipo(strCampFon, "paz_data_fine_domicilio")
            End If

            ' Controlla se il campo UslCodRes è nothing; in questo caso la modifica non va effettuata
            If Not paz.ResidenzaUslCodice Is Nothing Then
                ' Ricorda che vanno effettuate modifiche
                flagModifica = True
                ' Controlla se il campo codice della USL di residenza appartiene alla lista dei campi fondamentali
                If (Not flagPazTipo) Then
                    flagPazTipo = RiempiPazTipo(strCampFon, "paz_usl_codice_residenza")
                End If
            End If

            ' Controlla se il campo CodCit è nothing; in questo caso la modifica non va effettuata
            If Not paz.CitCodice Is Nothing Then
                ' Ricorda che vanno effettuate modifiche
                flagModifica = True
                ' Controlla se il campo codice cittadinanza appartiene alla lista dei campi fondamentali
                If (Not flagPazTipo) Then
                    flagPazTipo = RiempiPazTipo(strCampFon, "paz_cit_codice")
                End If
            End If

            ' Controlla se il campo telefono_1 è nothing; in questo caso la modifica non va effettuata
            If Not paz.Telefono1 Is Nothing Then
                ' Ricorda che vanno effettuate modifiche
                flagModifica = True
                ' Controlla se il campo telefono_1 appartiene alla lista dei campi fondamentali
                If (Not flagPazTipo) Then
                    flagPazTipo = RiempiPazTipo(strCampFon, "paz_telefono_1")
                End If
            End If

            ' Controlla se il campo telefono_2 è nothing; in questo caso la modifica non va effettuata
            If Not paz.Telefono2 Is Nothing Then
                ' Ricorda che vanno effettuate modifiche
                flagModifica = True
                ' Controlla se il campo telefono_2 appartiene alla lista dei campi fondamentali
                If (Not flagPazTipo) Then
                    flagPazTipo = RiempiPazTipo(strCampFon, "paz_telefono_2")
                End If
            End If

            ' Controlla se il campo telefono_3 è nothing; in questo caso la modifica non va effettuata
            If Not paz.Telefono3 Is Nothing Then
                ' Ricorda che vanno effettuate modifiche
                flagModifica = True
                ' Controlla se il campo telefono_3 appartiene alla lista dei campi fondamentali
                If (Not flagPazTipo) Then
                    flagPazTipo = RiempiPazTipo(strCampFon, "paz_telefono_3")
                End If
            End If

            ' Ricorda che vanno effettuate modifiche
            'If Not flagModifica Then
            '    flagModifica = True
            'End If
            ' Controlla se il campo codice medico di base appartiene alla lista dei campi fondamentali
            If Not flagPazTipo Then
                flagPazTipo = RiempiPazTipo(strCampFon, "paz_med_codice_base")
            End If

            ' Controlla se il campo CodMedBase è nothing; in questo caso la modifica non va effettuata
            If Not paz.MedCodiceBase Is Nothing Then
                ' Ricorda che vanno effettuate modifiche
                flagModifica = True
            End If

            ' Controlla se il campo data_decesso è nothing; in questo caso la modifica non va effettuata
            If Not paz.DataDecesso Is Nothing Then
                ' Ricorda che vanno effettuate modifiche
                flagModifica = True
                ' Controlla se il campo data_decesso appartiene alla lista dei campi fondamentali
                If Not flagPazTipo Then
                    flagPazTipo = RiempiPazTipo(strCampFon, "paz_data_decesso")
                End If
            End If

            ' Controlla se il campo data_cessazione_ass è nothing; in questo caso la modifica non va effettuata
            If Not paz.UslAssistenzaDataInizio Is Nothing Then
                flagModifica = True
                If Not flagPazTipo Then flagPazTipo = RiempiPazTipo(strCampFon, "paz_data_inizio_ass")
            End If

            ' Controlla se il campo data_cessazione_ass è nothing; in questo caso la modifica non va effettuata
            If Not paz.UslAssistenzaDataCessazione Is Nothing Then
                ' Ricorda che vanno effettuate modifiche
                flagModifica = True
                ' Controlla se il campo data_cessazione_ass appartiene alla lista dei campi fondamentali
                If (Not flagPazTipo) Then
                    flagPazTipo = RiempiPazTipo(strCampFon, "paz_data_cessazione_ass")
                End If
            End If

            ' Altri campi nuovi

            'Codice del distretto 
            If Not paz.DisCodice Is Nothing Then
                flagModifica = True
            End If

            ' Data emigrazione
            If Not paz.EmigrazioneData Is Nothing Then
                flagModifica = True
                If (Not flagPazTipo) Then
                    flagPazTipo = RiempiPazTipo(strCampFon, "paz_data_emigrazione")
                End If
            End If

            ' comune di emigrazione
            If Not paz.EmigrazioneComCodice Is Nothing Then
                flagModifica = True
            End If

            ' data immigrazione
            If Not paz.ImmigrazioneData Is Nothing Then
                flagModifica = True
                If Not flagPazTipo Then
                    flagPazTipo = RiempiPazTipo(strCampFon, "paz_data_immigrazione")
                End If
            End If

            ' comune di immigrazione
            If Not paz.ImmigrazioneComCodice Is Nothing Then
                flagModifica = True
            End If

            ' stato civile
            If Not paz.StatoCivile Is Nothing Then
                flagModifica = True
            End If

            ' motivo cessazione assistenza
            If Not paz.UslAssistenzaMotivoCessazione Is Nothing Then
                flagModifica = True
            End If

            ' comune di decesso
            If Not paz.ComCodiceDecesso Is Nothing Then
                flagModifica = True
            End If

            ' professione
            If Not paz.CodiceProfessione Is Nothing Then
                flagModifica = True
            End If

            ' codice demografico
            If Not paz.CodiceDemografico Is Nothing Then
                flagModifica = True
                If (Not flagPazTipo) Then
                    flagPazTipo = RiempiPazTipo(strCampFon, "paz_codice_demografico")
                End If
            End If

            '  cee
            If Not paz.CEE Is Nothing Then
                flagModifica = True
                If Not flagPazTipo Then
                    flagPazTipo = RiempiPazTipo(strCampFon, "paz_cee")
                End If
            End If

            ' usl di assistenza
            If Not paz.UslAssistenzaCodice Is Nothing Then
                flagModifica = True
            End If

            ' localita di domicilio
            If Not paz.DomicilioLocalita Is Nothing Then
                flagModifica = True
            End If

            ' localita residenza
            If Not paz.ResidenzaLocalita Is Nothing Then
                flagModifica = True
            End If

            ' USL IMMIGRAZIONE
            If Not paz.UslProvenienza Is Nothing Then
                flagModifica = True
            End If

            ' NOTE
            If Not paz.Note Is Nothing Then
                flagModifica = True
                If Not flagPazTipo Then
                    flagPazTipo = RiempiPazTipo(strCampFon, "paz_note")
                End If
            End If

            If Not paz.Tipo Is Nothing Then
                flagModifica = True
                If Not flagPazTipo Then
                    flagPazTipo = True
                End If
            End If

            'CFIS VALIDATO
            If Not paz.CfisValidato Is Nothing Then
                flagModifica = True
            End If

            'CFIS CERTIFICATORE
            If Not paz.CfisCertificatore Is Nothing Then
                flagModifica = True
            End If

            If Not paz.TeamEmissione Is Nothing Then
                flagModifica = True
            End If

            If Not paz.CategoriaRischioCodice Is Nothing Then
                flagModifica = True
            End If

        End Using

    End Sub

    Private Sub CheckValueIntegrity(ByRef paz As PazienteCentrale, ByRef strPazTipo As String, ByRef strPazComNas As String,
                                    ByRef strPazComRes As String, ByRef strPazComDom As String, ByRef strPazUslRes As String, ByRef strPazCodCit As String,
                                    ByRef strPazMedBas As String, ByRef strDisCodice As String, ByRef strComCodiceEmigrazione As String,
                                    ByRef strComCodiceImmigrazione As String, ByRef strStatoCivile As String, ByRef strMotivoCessazioneAss As String,
                                    ByRef strComCodiceDecesso As String, ByRef strCodiceProfessione As String, ByRef strUslCodiceAssistenza As String,
                                    ByRef strLocalitaDomicilio As String, ByRef strLocalitaResidenza As String, ByRef strNote As String,
                                    ByRef strUslProvenienza As String, ByRef strCategoriaRschioCodice As String)

        Using dam As IDAM = GetDAMCentrale()

            Dim check As Boolean = False
            If Me.Settings.CENTRALE_CHECK_INTEGRITY Then
                check = True
            End If

            If Not paz.ComCodiceNascita Is Nothing Then
                If check Then
                    strPazComNas = CheckIntegrity("t_ana_comuni", "com_codice", paz.ComCodiceNascita, dam)
                Else
                    strPazComNas = paz.ComCodiceNascita
                End If
            End If

            If Not paz.ResidenzaComCodice Is Nothing Then
                If check Then
                    strPazComRes = CheckIntegrity("t_ana_comuni", "com_codice", paz.ResidenzaComCodice, dam)
                Else
                    strPazComRes = paz.ResidenzaComCodice
                End If
            End If

            If Not paz.DomicilioComCodice Is Nothing Then
                If check Then
                    strPazComDom = CheckIntegrity("t_ana_comuni", "com_codice", paz.DomicilioComCodice, dam)
                Else
                    strPazComDom = paz.DomicilioComCodice
                End If
            End If

            If Not paz.ResidenzaUslCodice Is Nothing Then
                If check Then
                    strPazUslRes = CheckIntegrity("t_ana_usl", "usl_codice", paz.ResidenzaUslCodice, dam)
                Else
                    strPazUslRes = paz.ResidenzaUslCodice
                End If
            End If

            If Not paz.CitCodice Is Nothing Then
                If check Then
                    strPazCodCit = CheckIntegrity("t_ana_cittadinanze", "cit_codice", paz.CitCodice, dam)
                Else
                    strPazCodCit = paz.CitCodice
                End If
            End If

            If Not paz.MedCodiceBase Is Nothing Then
                If check Then
                    strPazMedBas = CheckIntegrity(T_ANA_MEDICI, "MED_CODICE", paz.MedCodiceBase, dam)
                Else
                    strPazMedBas = paz.MedCodiceBase
                End If
            End If

            If Not paz.DisCodice Is Nothing Then
                If check Then
                    strDisCodice = CheckIntegrity(T_ANA_DISTRETTI, "DIS_CODICE", paz.DisCodice, dam)
                Else
                    strDisCodice = paz.DisCodice
                End If
            End If

            If Not paz.EmigrazioneComCodice Is Nothing Then
                If check Then
                    strComCodiceEmigrazione = CheckIntegrity(T_ANA_COMUNI, "COM_CODICE", paz.EmigrazioneComCodice, dam)
                Else
                    strComCodiceEmigrazione = paz.EmigrazioneComCodice
                End If
            End If

            If Not paz.ImmigrazioneComCodice Is Nothing Then
                If check Then
                    strComCodiceImmigrazione = CheckIntegrity(T_ANA_COMUNI, "COM_CODICE", paz.ImmigrazioneComCodice, dam)
                Else
                    strComCodiceImmigrazione = paz.ImmigrazioneComCodice
                End If
            End If

            If Not paz.StatoCivile Is Nothing Then
                If check Then
                    strStatoCivile = CheckIntegrity(T_ANA_STATI_CIVILI, "STC_CODICE", paz.StatoCivile, dam)
                Else
                    strStatoCivile = paz.StatoCivile
                End If
            End If

            If Not paz.UslAssistenzaMotivoCessazione Is Nothing Then
                If check Then
                    strMotivoCessazioneAss = CheckIntegrity(T_ANA_MOTIVI_CESSAZ_ASSIST, "MCE_CODICE", paz.UslAssistenzaMotivoCessazione, dam)
                Else
                    strMotivoCessazioneAss = paz.UslAssistenzaMotivoCessazione
                End If
            End If

            If Not paz.ComCodiceDecesso Is Nothing Then
                If check Then
                    strComCodiceDecesso = CheckIntegrity(T_ANA_COMUNI, "COM_CODICE", paz.ComCodiceDecesso, dam)
                Else
                    strComCodiceDecesso = paz.ComCodiceDecesso
                End If
            End If

            If Not paz.CodiceProfessione Is Nothing Then
                If check Then
                    strCodiceProfessione = CheckIntegrity(T_ANA_POSIZIONI_PROFESSIONALI, "PFE_CODICE", paz.CodiceProfessione, dam)
                Else
                    strCodiceProfessione = paz.CodiceProfessione
                End If
            End If

            If Not paz.UslAssistenzaCodice Is Nothing Then
                If check Then
                    strUslCodiceAssistenza = CheckIntegrity(T_ANA_USL, "USL_CODICE", paz.UslAssistenzaCodice, dam)
                Else
                    strUslCodiceAssistenza = paz.UslAssistenzaCodice
                End If
            End If

            If Not paz.DomicilioLocalita Is Nothing Then
                If check Then
                    strLocalitaDomicilio = CheckIntegrity(T_ANA_LOCALITA, "QUA_CODICE", paz.DomicilioLocalita, dam)
                Else
                    strLocalitaDomicilio = paz.DomicilioLocalita
                End If
            End If

            If Not paz.ResidenzaLocalita Is Nothing Then
                If check Then
                    strLocalitaResidenza = CheckIntegrity(T_ANA_LOCALITA, "QUA_CODICE", paz.ResidenzaLocalita, dam)
                Else
                    strLocalitaResidenza = paz.ResidenzaLocalita
                End If
            End If

            If Not paz.UslProvenienza Is Nothing Then
                If check Then
                    strUslProvenienza = CheckIntegrity(T_ANA_USL, "USL_CODICE", paz.UslProvenienza, dam)
                Else
                    strUslProvenienza = paz.UslProvenienza
                End If
            End If

            If Not paz.CategoriaRischioCodice Is Nothing Then
                If check Then
                    strCategoriaRschioCodice = CheckIntegrity("T_ANA_RISCHIO", "RSC_CODICE", paz.CategoriaRischioCodice, dam)
                Else
                    strCategoriaRschioCodice = paz.CategoriaRischioCodice
                End If
            End If

        End Using

    End Sub

    Protected Friend Function BuildUpdateQuery(ByRef paz As PazienteCentrale, ByRef pazTipo As String, ByRef pazComNas As String,
                                               ByRef pazComRes As String, ByRef pazComDom As String, ByRef pazUslRes As String, ByRef pazCodCit As String,
                                               ByRef pazMedBase As String, ByRef disCodice As String, ByRef codiceComuneEmigrazione As String,
                                               ByRef codiceComuneImmigrazione As String, ByRef statoCivile As String, ByRef motivoCessazioneAss As String,
                                               ByRef codiceComuneDecesso As String, ByRef codiceProfessione As String, ByRef codiceUslAssistenza As String,
                                               ByRef localitaDomicilio As String, ByRef localitaResidenza As String, ByRef note As String,
                                               ByRef codiceUslProvenienza As String, ByRef codiceCategoriaRischio As String,
                                               ByRef oldRecordPaz As Array, ByRef fieldNameArray As Array, ByRef dam As IDAM) As Boolean

        dam.QB.NewQuery()
        dam.QB.AddTables(T_PAZ_PAZIENTI)

        ' Cognome e nome divisi
        If Settings.CENTRALE_SEPATIPO = Enumerators.CentraleTipoAnagrafe.COGNOME_NOME_SEPARATI Then

            If Not paz.Cognome Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_cognome", paz.Cognome) Then
                dam.QB.AddUpdateField("paz_cognome", paz.Cognome, DataTypes.Stringa)
            End If
            If Not paz.Nome Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_nome", paz.Nome) Then
                dam.QB.AddUpdateField("paz_nome", paz.Nome, DataTypes.Stringa)
            End If

        ElseIf Settings.CENTRALE_SEPATIPO = Enumerators.CentraleTipoAnagrafe.COGNOME_NOME_UNITI Then

            ' Cognome e nome uniti
            Dim sepa As String = Settings.CENTRALE_SEPAANAG
            Dim par As String

            If Not paz.Cognome Is Nothing And Not paz.Nome Is Nothing Then

                'cognome e nome valorizzati entrambi
                If (paz.Cognome <> "" Or paz.Nome <> "") Then
                    dam.QB.AddUpdateField("paz_cognome_nome", paz.Cognome & sepa & paz.Nome, DataTypes.Stringa)
                Else
                    dam.QB.AddUpdateField("paz_cognome_nome", "", DataTypes.Stringa)
                End If

            ElseIf Not paz.Cognome Is Nothing And paz.Nome Is Nothing Then

                ' solo cognome valorizzato
                par = dam.QB.AddCustomParam(paz.Cognome & sepa)
                dam.QB.AddUpdateField("paz_cognome_nome", dam.QB.FC.Concat(par, "paz_nome"), DataTypes.Replace)

            ElseIf paz.Cognome Is Nothing And Not paz.Nome Is Nothing Then

                ' solo nome valorizzato
                par = dam.QB.AddCustomParam(sepa & paz.Nome)
                dam.QB.AddUpdateField("paz_cognome_nome", dam.QB.FC.Concat("paz_cognome", par), DataTypes.Replace)

            End If

        End If

        If Not paz.codiceFiscale Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_codice_fiscale", paz.codiceFiscale) Then
            dam.QB.AddUpdateField("paz_codice_fiscale", paz.codiceFiscale, DataTypes.Stringa)
        End If
        If Not paz.Tessera Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_tessera", paz.Tessera) Then
            dam.QB.AddUpdateField("paz_tessera", paz.Tessera, DataTypes.Stringa)
        End If
        If Not paz.CodiceRegione Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_codice_regionale", paz.CodiceRegione) Then
            dam.QB.AddUpdateField("paz_codice_regionale", paz.CodiceRegione, DataTypes.Stringa)
        End If
        If Not paz.CodiceAzienda Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_codice_ausiliario", paz.CodiceAzienda) Then
            dam.QB.AddUpdateField("paz_codice_ausiliario", paz.CodiceAzienda, DataTypes.Stringa)
        End If
        If Not paz.DataNascita Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_data_nascita", paz.DataNascita) Then
            dam.QB.AddUpdateField("paz_data_nascita", paz.DataNascita, DataTypes.Data)
        End If
        If Not pazComNas Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_com_codice_nascita", pazComNas) Then
            dam.QB.AddUpdateField("paz_com_codice_nascita", pazComNas, DataTypes.Stringa)
        End If
        If Not paz.Sesso Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_sesso", paz.Sesso) Then
            dam.QB.AddUpdateField("paz_sesso", paz.Sesso, DataTypes.Stringa)
        End If
        If Not pazComRes Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_com_codice_residenza", pazComRes) Then
            dam.QB.AddUpdateField("paz_com_codice_residenza", pazComRes, DataTypes.Stringa)
        End If
        If Not paz.ResidenzaIndirizzo Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_indirizzo_residenza", paz.ResidenzaIndirizzo) Then
            dam.QB.AddUpdateField("paz_indirizzo_residenza", paz.ResidenzaIndirizzo, DataTypes.Stringa)
        End If
        If Not paz.ResidenzaCap Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_cap_residenza", paz.ResidenzaCap) Then
            dam.QB.AddUpdateField("paz_cap_residenza", paz.ResidenzaCap, DataTypes.Stringa)
        End If
        If Not paz.ResidenzaDataInizio Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_data_inizio_residenza", paz.ResidenzaDataInizio) Then
            dam.QB.AddUpdateField("paz_data_inizio_residenza", paz.ResidenzaDataInizio, DataTypes.Data)
        End If
        If Not paz.ResidenzaDataFine Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_data_fine_residenza", paz.ResidenzaDataFine) Then
            dam.QB.AddUpdateField("paz_data_fine_residenza", paz.ResidenzaDataFine, DataTypes.Data)
        End If
        If Not pazComDom Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_com_codice_domicilio", pazComDom) Then
            dam.QB.AddUpdateField("paz_com_codice_domicilio", pazComDom, DataTypes.Stringa)
        End If
        If Not paz.DomicilioIndirizzo Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_indirizzo_domicilio", paz.DomicilioIndirizzo) Then
            dam.QB.AddUpdateField("paz_indirizzo_domicilio", paz.DomicilioIndirizzo, DataTypes.Stringa)
        End If
        If Not paz.DomicilioCap Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_cap_domicilio", paz.DomicilioCap) Then
            dam.QB.AddUpdateField("paz_cap_domicilio", paz.DomicilioCap, DataTypes.Stringa)
        End If
        If Not paz.DomicilioDataInizio Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_data_inizio_domicilio", paz.DomicilioDataInizio) Then
            dam.QB.AddUpdateField("paz_data_inizio_domicilio", paz.DomicilioDataInizio, DataTypes.Data)
        End If
        If Not paz.DomicilioDataFine Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_data_fine_domicilio", paz.DomicilioDataFine) Then
            dam.QB.AddUpdateField("paz_data_fine_domicilio", paz.DomicilioDataFine, DataTypes.Data)
        End If
        If Not pazUslRes Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_usl_codice_residenza", pazUslRes) Then
            dam.QB.AddUpdateField("paz_usl_codice_residenza", pazUslRes, DataTypes.Stringa)
        End If
        If Not pazCodCit Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_cit_codice", pazCodCit) Then
            dam.QB.AddUpdateField("paz_cit_codice", pazCodCit, DataTypes.Stringa)
        End If
        If Not paz.Telefono1 Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_telefono_1", paz.Telefono1) Then
            dam.QB.AddUpdateField("paz_telefono_1", paz.Telefono1, DataTypes.Stringa)
        End If
        If Not paz.Telefono2 Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_telefono_2", paz.Telefono2) Then
            dam.QB.AddUpdateField("paz_telefono_2", paz.Telefono2, DataTypes.Stringa)
        End If
        If Not paz.Telefono3 Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_telefono_3", paz.Telefono3) Then
            dam.QB.AddUpdateField("paz_telefono_3", paz.Telefono3, DataTypes.Stringa)
        End If
        If Not pazMedBase Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_med_codice_base", pazMedBase) Then
            dam.QB.AddUpdateField("paz_med_codice_base", pazMedBase, DataTypes.Stringa)
        End If
        If Not paz.DataDecesso Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_data_decesso", paz.DataDecesso) Then
            dam.QB.AddUpdateField("paz_data_decesso", paz.DataDecesso, DataTypes.Data)
        End If
        If Not paz.UslAssistenzaDataInizio Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_data_inizio_ass", paz.UslAssistenzaDataInizio) Then
            dam.QB.AddUpdateField("paz_data_inizio_ass", paz.UslAssistenzaDataInizio, DataTypes.Data)
        End If
        If Not paz.UslAssistenzaDataCessazione Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_data_cessazione_ass", paz.UslAssistenzaDataCessazione) Then
            dam.QB.AddUpdateField("paz_data_cessazione_ass", paz.UslAssistenzaDataCessazione, DataTypes.Data)
        End If
        If Not disCodice Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "PAZ_DIS_CODICE", disCodice) Then
            dam.QB.AddUpdateField("PAZ_DIS_CODICE", disCodice, DataTypes.Stringa)
        End If
        If Not paz.EmigrazioneData Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "PAZ_DATA_EMIGRAZIONE", paz.EmigrazioneData) Then
            dam.QB.AddUpdateField("PAZ_DATA_EMIGRAZIONE", paz.EmigrazioneData, DataTypes.Data)
        End If
        If Not codiceComuneEmigrazione Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "PAZ_COM_CODICE_EMIGRAZIONE", codiceComuneEmigrazione) Then
            dam.QB.AddUpdateField("PAZ_COM_CODICE_EMIGRAZIONE", codiceComuneEmigrazione, DataTypes.Stringa)
        End If
        If Not paz.ImmigrazioneData Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "PAZ_DATA_IMMIGRAZIONE", paz.ImmigrazioneData) Then
            dam.QB.AddUpdateField("PAZ_DATA_IMMIGRAZIONE", paz.ImmigrazioneData, DataTypes.Data)
        End If
        If Not codiceComuneImmigrazione Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "PAZ_COM_CODICE_IMMIGRAZIONE", codiceComuneImmigrazione) Then
            dam.QB.AddUpdateField("PAZ_COM_CODICE_IMMIGRAZIONE", codiceComuneImmigrazione, DataTypes.Stringa)
        End If
        If Not statoCivile Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "PAZ_STATO_CIVILE", statoCivile) Then
            dam.QB.AddUpdateField("PAZ_STATO_CIVILE", statoCivile, DataTypes.Stringa)
        End If
        If Not motivoCessazioneAss Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "PAZ_MOTIVO_CESSAZIONE_ASS", motivoCessazioneAss) Then
            dam.QB.AddUpdateField("PAZ_MOTIVO_CESSAZIONE_ASS", motivoCessazioneAss, DataTypes.Stringa)
        End If
        If Not codiceComuneDecesso Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "PAZ_COM_CODICE_DECESSO", codiceComuneDecesso) Then
            dam.QB.AddUpdateField("PAZ_COM_CODICE_DECESSO", codiceComuneDecesso, DataTypes.Stringa)
        End If
        If Not codiceProfessione Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "PAZ_CODICE_PROFESSIONE", codiceProfessione) Then
            dam.QB.AddUpdateField("PAZ_CODICE_PROFESSIONE", codiceProfessione, DataTypes.Stringa)
        End If
        If Not paz.CodiceDemografico Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "PAZ_CODICE_DEMOGRAFICO", paz.CodiceDemografico) Then
            dam.QB.AddUpdateField("PAZ_CODICE_DEMOGRAFICO", paz.CodiceDemografico, DataTypes.Stringa)
        End If
        If Not paz.CEE Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "PAZ_CEE", paz.CEE) Then
            dam.QB.AddUpdateField("PAZ_CEE", paz.CEE, DataTypes.Stringa)
        End If
        If Not codiceUslAssistenza Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "PAZ_USL_CODICE_ASSISTENZA", codiceUslAssistenza) Then
            dam.QB.AddUpdateField("PAZ_USL_CODICE_ASSISTENZA", codiceUslAssistenza, DataTypes.Stringa)
        End If
        If Not localitaDomicilio Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "PAZ_LOCALITA_DOMICILIO", localitaDomicilio) Then
            dam.QB.AddUpdateField("PAZ_LOCALITA_DOMICILIO", localitaDomicilio, DataTypes.Stringa)
        End If
        If Not localitaResidenza Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "PAZ_LOCALITA_RESIDENZA", localitaResidenza) Then
            dam.QB.AddUpdateField("PAZ_LOCALITA_RESIDENZA", localitaResidenza, DataTypes.Stringa)
        End If
        If Not paz.UslProvenienza Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "PAZ_USL_PROVENIENZA", paz.UslProvenienza) Then
            dam.QB.AddUpdateField("PAZ_USL_PROVENIENZA", paz.UslProvenienza, DataTypes.Stringa)
        End If
        If Not paz.Note Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "PAZ_NOTE", paz.Note) Then
            dam.QB.AddUpdateField("PAZ_NOTE", paz.Note, DataTypes.Stringa)
        End If
        If Not paz.CfisValidato Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_cfis_validato", paz.CfisValidato) Then
            dam.QB.AddUpdateField("paz_cfis_validato", paz.CfisValidato, DataTypes.Stringa)
        End If
        If Not paz.CfisCertificatore Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_cfis_certificatore", paz.CfisCertificatore) Then
            dam.QB.AddUpdateField("paz_cfis_certificatore", paz.CfisCertificatore, DataTypes.Stringa)
        End If
        If Not pazTipo Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_tipo", pazTipo) Then
            dam.QB.AddUpdateField("paz_tipo", pazTipo, DataTypes.Stringa)
        End If
        If Not paz.AliasCodice Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_nome", paz.AliasCodice) Then
            dam.QB.AddUpdateField("paz_alias", paz.AliasCodice, DataTypes.Stringa)
        End If
        If Not paz.AliasData Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_data_alias", paz.AliasData) Then
            dam.QB.AddUpdateField("paz_data_alias", paz.AliasData, DataTypes.DataOra)
        End If
        If Not paz.ComCodiceDemografico Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_com_codice_demografico", paz.ComCodiceDemografico) Then
            dam.QB.AddUpdateField("paz_com_codice_demografico", paz.ComCodiceDemografico, DataTypes.Stringa)
        End If
        If paz.DataVariazioneComunale <> Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_data_variazione_comune", paz.DataVariazioneComunale) Then
            dam.QB.AddUpdateField("paz_data_variazione_comune", paz.DataVariazioneComunale, DataTypes.Data)
        End If
        If Not paz.TeamEmissione Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_team_emissione", paz.TeamEmissione) Then
            dam.QB.AddUpdateField("paz_team_emissione", paz.TeamEmissione, DataTypes.Stringa)
        End If
        If Not codiceCategoriaRischio Is Nothing AndAlso IsModified(oldRecordPaz, fieldNameArray, "paz_rsc_codice", codiceCategoriaRischio) Then
            dam.QB.AddUpdateField("paz_rsc_codice", codiceCategoriaRischio, DataTypes.Stringa)
        End If

        Dim modified As Boolean = False

        If dam.QB.Param.Count > 0 Then
            'almeno un campo modificato
            modified = True
        End If

        dam.QB.AddWhereCondition("paz_codice", Comparatori.Uguale, paz.codice, DataTypes.Stringa)

        Return modified

    End Function

    Public Function ModificaInCentrale(paz As PazienteCentrale, addetto As String, ente As String, ByRef errorMessage As String) As Boolean

        Dim result As Boolean = False

        Dim newRecordPaziente As Array = Nothing
        Dim oldRecordPaziente As Array = Nothing
        Dim fieldName As Array = Nothing

        Dim flagPazTipo As Boolean = False
        Dim flagModifica As Boolean = False

        ' Variabili per poter posticipare la query (in DAM non si può annidarne la creazione)
        Dim pazTipo As String = Nothing
        Dim pazComNas As String = Nothing
        Dim pazComRes As String = Nothing
        Dim pazComDom As String = Nothing
        Dim pazUslRes As String = Nothing
        Dim pazCodCit As String = Nothing
        Dim pazMedBase As String = Nothing
        Dim disCodice As String = Nothing
        Dim codiceComuneEmigrazione As String = Nothing
        Dim codiceComuneImmigrazione As String = Nothing
        Dim statoCivile As String = Nothing
        Dim motivoCessazioneAss As String = Nothing
        Dim codiceComuneDecesso As String = Nothing
        Dim codiceProfessione As String = Nothing
        Dim codiceUslAssistenza As String = Nothing
        Dim localitaDomicilio As String = Nothing
        Dim localitaResidenza As String = Nothing
        Dim note As String = Nothing
        Dim codiceUslProvenienza As String = Nothing
        Dim codiceCategoriaRischio As String = Nothing

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions)

            result = FillPatientArray(paz.codice.ToString(), fieldName, oldRecordPaziente, newRecordPaziente, errorMessage)

            If result Then

                Try
                    ' Indicazione delle modifiche effettuate:
                    ' - flagModifica: è stato modificato un campo per cui e' richiesto il salvataggio
                    ' - flagPazTipo: è stato modificato un campo fondamentale
                    CheckFlags(paz, flagModifica, flagPazTipo)

                    ' Controlla che ci sia qualcosa da modificare: infatti se tutti i campi valgono nothing rimarrebbe un istruzione di set a penzoloni
                    If flagModifica Then

                        '[TODO] Come mai non viene riflesso il tipo passato da DatiPaziente ma viene sempre ricalcolato in base ai campi fondamentali?
                        ' Xmpi ci comunica se il paziente e' un master o un alias, occorre prevedere l'aggiornamento del tipo.
                        If flagPazTipo Then
                            If paz.Tipo IsNot Nothing Then
                                pazTipo = paz.Tipo
                            Else
                                ' Imposta il valore appropriato per il campo paz_tipo a seguito di una modifica di almeno un campo fondamentale, secondo il seguente schema:
                                ' vuoto -> P 
                                ' M -> m
                                ' m -> m
                                ' P -> P
                                Select Case oldRecordPaziente.GetValue(Array.IndexOf(fieldName, "PAZ_TIPO")).ToString()
                                    Case "M", "m"
                                        pazTipo = "m"
                                    Case "P", ""
                                        pazTipo = "P"
                                End Select
                            End If
                        End If

                        ' Controllo dei vincoli di integrita', vengono rienpiti i campi con in base al parametro 'CheckIntegrity'
                        ' e se il campo non viene trovato si lascia stringa vuota
                        CheckValueIntegrity(paz, pazTipo, pazComNas, pazComRes, pazComDom, pazUslRes, pazCodCit,
                                            pazMedBase, disCodice, codiceComuneEmigrazione, codiceComuneImmigrazione, statoCivile,
                                            motivoCessazioneAss, codiceComuneDecesso, codiceProfessione, codiceUslAssistenza,
                                            localitaDomicilio, localitaResidenza, note, codiceUslProvenienza, codiceCategoriaRischio)

                        ' *********************************************************************
                        ' **     Query di aggiornamento dei pazienti in anagrafe centrale    **
                        ' *********************************************************************
                        Using dam As IDAM = GetDAMCentrale()

                            Dim modified As Boolean = BuildUpdateQuery(paz, pazTipo, pazComNas, pazComRes, pazComDom, pazUslRes, pazCodCit,
                                                                       pazMedBase, disCodice, codiceComuneEmigrazione, codiceComuneImmigrazione, statoCivile,
                                                                       motivoCessazioneAss, codiceComuneDecesso, codiceProfessione, codiceUslAssistenza,
                                                                       localitaDomicilio, localitaResidenza, note, codiceUslProvenienza, codiceCategoriaRischio,
                                                                       oldRecordPaziente, fieldName, dam)

                            If modified Then

                                Dim controlloSovrascrittura As Boolean = False

                                If controlloSovrascrittura Then

                                    ' TODO [paz sovrascritti]: controllo + stop update

                                    result = False
                                    errorMessage = "TEST SOVRASCRITTURA"

                                Else

                                    ' Esegue l'aggiornamento dell'anagrafe centrale
                                    dam.ExecNonQuery(ExecQueryType.Update)

                                    ' Carica da DB in NewRecord il nuovo record inserito
                                    dam.QB.NewQuery()
                                    dam.QB.AddSelectFields("*")
                                    dam.QB.AddTables(T_PAZ_PAZIENTI)
                                    dam.QB.AddWhereCondition("paz_codice", Comparatori.Uguale, paz.codice.ToString(), DataTypes.Stringa)

                                    Using odr As IDataReader = dam.BuildDataReader()
                                        odr.Read()
                                        For i As Integer = newRecordPaziente.GetLowerBound(0) To newRecordPaziente.GetUpperBound(0)
                                            newRecordPaziente.SetValue(odr(i).ToString(), i)
                                        Next
                                    End Using

                                    ' Inserimento nello storico variazioni
                                    If Settings.CENTRALE_STORVAR Then
                                        InserisciRecordVarInStoricoVariazioni(addetto, ente, paz.codice, oldRecordPaziente, newRecordPaziente, fieldName)
                                    End If

                                    result = True
                                End If

                            Else
                                errorMessage = "Nessun salvataggio effettuato, perchè i dati passati sono coincidenti con quelli attuali!"
                                result = True
                            End If

                        End Using

                    Else
                        errorMessage = "Nessun salvataggio effettuato, perchè nulla era da modificare!"
                        result = True
                    End If

                Finally
                    If Not oldRecordPaziente Is Nothing Then Array.Clear(oldRecordPaziente, oldRecordPaziente.GetLowerBound(0), oldRecordPaziente.GetUpperBound(0))
                    If Not newRecordPaziente Is Nothing Then Array.Clear(newRecordPaziente, newRecordPaziente.GetLowerBound(0), newRecordPaziente.GetUpperBound(0))
                    If Not fieldName Is Nothing Then Array.Clear(fieldName, fieldName.GetLowerBound(0), fieldName.GetUpperBound(0))
                End Try

            End If

            transactionScope.Complete()

        End Using

        Return result

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Questa funzione valuta se il campo fname è un campo fondamentale  
    '''   in questo caso aggiorna la stringa sql per l'impostazione del campo paz_tipo
    ''' </summary>
    ''' <param name="CampFon">riporta il nome dei campi fondamentali separati da un ;</param>
    ''' <param name="fName">è il nome del campo che deve essere valutato: occorre verificare
    '''                   se appartiene alla stringa CampFon</param>
    ''' <returns>True se il campo fName è fondamentale, False altrimenti</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adesimone]	10/06/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function RiempiPazTipo(CampFon As String, fName As String) As Boolean

        If InStr(UCase(CampFon), UCase(fName), CompareMethod.Text) <> 0 Then
            RiempiPazTipo = True
        Else
            RiempiPazTipo = False
        End If

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Questa funzione si occupa di inserire un record variazione nella tabella che memorizza
    '''   lo storico delle variazioni
    ''' </summary>
    ''' <param name="Addetto">addetto che effettua la modifica ricavato dalla classe
    '''                           ModInCenPar, che viene istanziata prima della chiamata 
    '''                          alla ModificaInCentrale che a sua volta chiama la ModInCentrale
    '''                           che a sua volta chiama questa procedura</param>
    ''' <param name="Ente">ente che effettua la modifica</param>
    ''' <param name="pazID">identificativo del paziente soggetto a amodifica</param>
    ''' <param name="OldR">vettore di stringhe contenente il record prima dell'aggiornamento</param>
    ''' <param name="NewR">vettore di stringhe contenente il record dopo l'aggiornamento</param>
    ''' <param name="FName">vettore di stringhe contenente il nome dei campi dei record</param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adesimone]	10/06/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Sub InserisciRecordVarInStoricoVariazioni(Addetto As String, Ente As String, pazID As String, ByRef OldR As Array, ByRef NewR As Array, ByRef FName As Array)

        ' Crea il DAM per la connessione al database
        Using dam As IDAM = GetDAMStoricoVar()

            dam.QB.NewQuery()
            dam.QB.AddTables(T_PAZ_STORICO_VARIAZIONI)
            ' Inserisce l'addetto che ha effettuato la modifica
            dam.QB.AddInsertField("var_addetto", Strings.Mid(Addetto, 1, 50), DataTypes.Stringa)
            ' Inserisce la data di registrazione della modifica
            dam.QB.AddInsertField("var_data_registrazione", Format(Now(), "yyyyMMdd HH:mm:ss").Replace(".", ":"), DataTypes.Stringa)
            ' Inserisce l'ente che ha effettuato la modifica
            dam.QB.AddInsertField("var_ente", Strings.Mid(Ente, 1, 30), DataTypes.Stringa)
            ' Inserisce il tipo di modifica: V=modifica
            dam.QB.AddInsertField("var_tipo", "V", DataTypes.Stringa)
            ' Inserisce il paz_codice del paziente soggetto a modifica
            dam.QB.AddInsertField("paz_codice", pazID, DataTypes.Stringa)


            ' Valutazione della modifica dei campi anagrafici per 
            ' la registrazione in storico

            If (Me.Settings.CENTRALE_SEPATIPO = Enumerators.CentraleTipoAnagrafe.COGNOME_NOME_UNITI) Then
                ' Valuta se è stato modificato il campo paz_cognome_nome e lo inserisce nei parametri
                CheckModified(OldR, NewR, FName, "paz_cognome_nome", dam)
            ElseIf (Me.Settings.CENTRALE_SEPATIPO = Enumerators.CentraleTipoAnagrafe.COGNOME_NOME_SEPARATI) Then
                ' Valuta se è stato modificato il campo paz_nome, paz_cognome e lo inserisce nei parametri
                CheckModified(OldR, NewR, FName, "paz_cognome", dam)
                CheckModified(OldR, NewR, FName, "paz_nome", dam)
            End If

            CheckModified(OldR, NewR, FName, "PAZ_CODICE_REGIONALE", dam)
            CheckModified(OldR, NewR, FName, "PAZ_CODICE_AUSILIARIO", dam)
            CheckModified(OldR, NewR, FName, "PAZ_CODICE_FISCALE", dam)
            CheckModified(OldR, NewR, FName, "PAZ_TESSERA", dam)
            CheckModified(OldR, NewR, FName, "PAZ_DATA_NASCITA", dam, True)
            CheckModified(OldR, NewR, FName, "PAZ_COM_CODICE_NASCITA", dam)
            CheckModified(OldR, NewR, FName, "PAZ_SESSO", dam)
            CheckModified(OldR, NewR, FName, "PAZ_COM_CODICE_RESIDENZA", dam)
            CheckModified(OldR, NewR, FName, "PAZ_INDIRIZZO_RESIDENZA", dam)
            CheckModified(OldR, NewR, FName, "PAZ_CAP_RESIDENZA", dam)
            CheckModified(OldR, NewR, FName, "PAZ_COM_CODICE_DOMICILIO", dam)
            CheckModified(OldR, NewR, FName, "PAZ_INDIRIZZO_DOMICILIO", dam)
            CheckModified(OldR, NewR, FName, "PAZ_CAP_DOMICILIO", dam)
            CheckModified(OldR, NewR, FName, "PAZ_USL_CODICE_RESIDENZA", dam)
            CheckModified(OldR, NewR, FName, "PAZ_CIT_CODICE", dam)
            CheckModified(OldR, NewR, FName, "PAZ_TELEFONO_1", dam)
            CheckModified(OldR, NewR, FName, "PAZ_TELEFONO_2", dam)
            CheckModified(OldR, NewR, FName, "PAZ_TELEFONO_3", dam)
            CheckModified(OldR, NewR, FName, "PAZ_MED_CODICE_BASE", dam)
            CheckModified(OldR, NewR, FName, "PAZ_TIPO", dam)
            CheckModified(OldR, NewR, FName, "PAZ_ALIAS", dam)
            CheckModified(OldR, NewR, FName, "PAZ_EXALIAS", dam)
            CheckModified(OldR, NewR, FName, "PAZ_DATA_ALIAS", dam, True)
            CheckModified(OldR, NewR, FName, "PAZ_DATA_CESSAZIONE_ASS", dam, True)
            CheckModified(OldR, NewR, FName, "PAZ_DATA_DECESSO", dam, True)
            CheckModified(OldR, NewR, FName, "PAZ_DIS_CODICE", dam)
            CheckModified(OldR, NewR, FName, "PAZ_DATA_EMIGRAZIONE", dam, True)
            CheckModified(OldR, NewR, FName, "PAZ_COM_CODICE_EMIGRAZIONE", dam)
            CheckModified(OldR, NewR, FName, "PAZ_DATA_IMMIGRAZIONE", dam, True)
            CheckModified(OldR, NewR, FName, "PAZ_COM_CODICE_IMMIGRAZIONE", dam)
            CheckModified(OldR, NewR, FName, "PAZ_STATO_CIVILE", dam)
            CheckModified(OldR, NewR, FName, "PAZ_MOTIVO_CESSAZIONE_ASS", dam)
            CheckModified(OldR, NewR, FName, "PAZ_COM_CODICE_DECESSO", dam)
            CheckModified(OldR, NewR, FName, "PAZ_CODICE_PROFESSIONE", dam)
            CheckModified(OldR, NewR, FName, "PAZ_CODICE_DEMOGRAFICO", dam)
            CheckModified(OldR, NewR, FName, "PAZ_CEE", dam)
            CheckModified(OldR, NewR, FName, "PAZ_USL_CODICE_ASSISTENZA", dam)
            CheckModified(OldR, NewR, FName, "PAZ_LOCALITA_DOMICILIO", dam)
            CheckModified(OldR, NewR, FName, "PAZ_LOCALITA_RESIDENZA", dam)
            CheckModified(OldR, NewR, FName, "PAZ_USL_PROVENIENZA", dam)
            CheckModified(OldR, NewR, FName, "PAZ_NOTE", dam)
            CheckModified(OldR, NewR, FName, "PAZ_CFIS_VALIDATO", dam)
            CheckModified(OldR, NewR, FName, "PAZ_CFIS_CERTIFICATORE", dam)
            CheckModified(OldR, NewR, FName, "PAZ_RSC_CODICE", dam)


            ' Ora il Dam dispone della stringa SQL per la creazione del record nella tabella dello
            ' storico delle variazioni
            ' Esegue l'inserimento del record nella tabella dello storico
            dam.ExecNonQuery(ExecQueryType.Insert)

        End Using

    End Sub

    ''' <summary>
    ''' Questa sub verifica se il campo di nome fieldname è stato modificato oppure no in
    '''   fase di modifica di un record paziente e genera la stringa sql per l'inserimento di
    '''   un record nella tabella dello storico delle variazioni
    ''' </summary>
    ''' <param name="OldR">è un vettore di stringhe contenente il record prima dell'aggiornamento</param>
    ''' <param name="NewR"> è un vettore di stringhe contenente il record dopo l'aggiornamento</param>
    ''' <param name="FName">è un vettore di stringhe contenente il nome dei campi dei record</param>
    ''' <param name="fieldname">è il nome del campo del record che viene considerato.</param>
    ''' <param name="Dam"></param>
    ''' <param name="dt">un booleano che indica se il campo in fase di modifica è una data (true)
    '''               oppure no (false)</param>
    ''' <remarks>Non posso realizzare un ciclo all'interno della CheckModified per valutare
    '''           con un'unica chiamata a questa procedura tutti i campi eventualmente modificati; 
    '''          infatti la stringa SQL generata è di tipo insert e dunque è fondamentale la cor_
    '''           rispondenza tra l'ordine dei nomi dei campi e i valori; poichè non so in che ordine
    '''           vengono tirati su i campi dal database, è necessario effettuare una chiamata alla 
    '''           CheckModified per ogni campo presente nell'anagrafe centrale.
    ''' </remarks>
    ''' <history>
    ''' 	[adesimone]	10/06/2008	Created
    ''' </history>
    Private Sub CheckModified(ByRef OldR As Array, ByRef NewR As Array, ByRef FName As Array, fieldname As String, ByRef dam As IDAM, Optional dt As Boolean = False)

        Dim found As Boolean = False

        ' Il campo fieldname esiste sicuramente
        Dim pos As Integer = 0
        While Not found
            If (FName.GetValue(pos) = UCase(fieldname)) Then
                found = True
            Else
                pos += 1
            End If
        End While

        If (OldR.GetValue(pos) <> NewR.GetValue(pos)) Then

            ' Il campo FName(pos) è stato modificato
            If OldR.GetValue(pos) = "" Then
                ' Il campo è stato riempito => nel record dello storico va messo un #                
                If dt Then
                    ' Il campo in questione è una data
                    dam.QB.AddInsertField(fieldname, "1/1/1000", DataTypes.Data)
                Else
                    ' Il campo in questione è una stringa generica
                    dam.QB.AddInsertField(fieldname, "#", DataTypes.Stringa)
                End If
            Else
                ' Il campo è stato modificato => nel record dello storico va messo il 
                ' vecchio contenuto    
                If (dt) Then
                    dam.QB.AddInsertField(fieldname, Format(CDate(OldR.GetValue(pos)), "dd/MM/yyyy"), DataTypes.Data)
                Else
                    dam.QB.AddInsertField(fieldname, OldR.GetValue(pos), DataTypes.Stringa)
                End If
            End If

        Else
            ' Come si traduce?
            'SQLUtil.CreaStringaInsert(Nothing, sqlstr)
        End If

    End Sub

    Private Function IsModified(recordPaziente As Array, fieldNameArray As Array, currentFieldName As String, newValue As Object) As Boolean

        Dim found As Boolean = False
        Dim modified As Boolean = False

        If newValue IsNot Nothing Then

            ' Il campo fieldname esiste sicuramente
            Dim pos As Integer = 0

            While Not found
                If fieldNameArray.GetValue(pos) = UCase(currentFieldName) Then
                    found = True
                Else
                    pos += 1
                End If
            End While

            Dim oldValue As Object = recordPaziente.GetValue(pos)

            If oldValue Is DBNull.Value Then

                If newValue.ToString() <> String.Empty Then
                    modified = True
                End If

            Else

                If newValue.ToString() <> oldValue Then
                    modified = True
                End If

            End If

        End If

        Return modified

    End Function

#End Region

#Region " Eliminazione "

    Public Sub EliminaPaziente(codicePaziente As String)

        Using damCentrale As IDAM = Me.GetDAMCentrale()

            damCentrale.QB.NewQuery()
            damCentrale.QB.AddTables("T_PAZ_PAZIENTI_VAC")
            damCentrale.QB.AddWhereCondition("PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Stringa)

            damCentrale.ExecNonQuery(ExecQueryType.Delete)

        End Using

    End Sub

    Public Function CloneAlias(codicePazienteMaster As String, codicePazienteAlias As String, dataAlias As DateTime, idUtente As Integer, aziendaProvenienza As String) As Integer

        GenericProviderCentrale.AliasPazientiCentrale.CloneAlias(codicePazienteMaster, codicePazienteAlias, dataAlias, idUtente, aziendaProvenienza)

    End Function

#End Region

#Region " Private "

    ''' <summary>
    ''' Crea la connessione al database indicato dal parametro APP_ID_CENTRALE.
    ''' Dal manager, recupera la stringa di connessione dell'applicativo selezionandolo in base all'app_id specificato dal parametro APP_ID_CENTRALE e all'azienda corrente.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetDAMCentrale() As IDAM

        Return DAMBuilder.CreateDAM(ApplicazioneCentrale.DbmsProvider, ApplicazioneCentrale.getConnectionInfo().ConnectionString)

    End Function

    Private Function GetDAMStoricoVar() As IDAM

        Return DAMBuilder.CreateDAM(ApplicazioneCentrale.DbmsProvider, ApplicazioneCentrale.getConnectionInfo().ConnectionString)

    End Function

    Private Function fc_decode(fc As AbstractFC, strCampo As String, strCond As String, strValoreVero As String, strValoreFalso As String) As String

        Return fc.Switch(strCampo, strCond, strValoreVero, strValoreFalso)

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>Questa funzione ha il compito di calcolare un codice univoco per un paziente; per
    '''   prima cosa richiama la funzione di generazione del codice fiscale; se questo è già
    '''  presente nell'archivio centrale dei pazienti, allora provvede alla sostituzione dei
    '''   caratteri "-" presenti nel codice con lettere progressive dell'alfabeto (prima "A",
    '''   poi "B" ecc. fino alla generazione di un codice univoco (per maggiori dettagli
    '''   consultare il documento Server.doc par.3.7) </summary>
    ''' <param name="cognome"> cognome del paziente di cui si vuole calcolare il codice fiscale</param>
    ''' <param name="nome">nome del paziente</param>
    ''' <param name="dataNascita">data di nascita del paziente; E' necessario che la data sia 
    '''                      presentata come una stringa nel formato gg/mm/aaaa</param>
    ''' <param name="comune">comune di nascita del paziente</param>
    ''' <param name="sesso">sesso del paziente</param>
    ''' <param name="dam">oggetto Dam per la connessione all'anagrafe centrale  </param>
    ''' <param name="errMessage">IO: stringa contenente il messaggio di errore qualora la funzione ritornasse false</param>
    ''' <param name="ucf">IO: codice identificativo del paziente univoco</param>
    ''' <returns>true se tutto è andato a buon fine, false altrimenti</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adesimone]	10/06/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function GenUniCode(cognome As String, nome As String, dataNascita As String, comune As String, sesso As String, dam As IDAM, ByRef errMessage As String, ByRef ucf As String) As Boolean

        ' Variabile che vale true finchè il codice provvisorio cf non è univoco
        Dim found As Boolean = True
        ' Codice fiscale calcolato nel modo consueto
        Dim cf As String = ""

        ' Calcola il codice identificativo con i dati a disposizione
        If (Not CFGen(cognome, nome, dataNascita, comune, sesso, dam, errMessage, cf)) Then
            errMessage = "Impossibile calcolare il codice fiscale del paziente"
            Return False
        Else
            ' Il calcolo del cf originario è andato a buon fine
            ' Comando SQL da eseguire sul DB
            dam.QB.NewQuery()
            dam.QB.AddSelectFields("paz_codice")
            dam.QB.AddTables(T_PAZ_PAZIENTI)
            dam.QB.AddWhereCondition("paz_codice", Comparatori.Uguale, cf, DataTypes.Stringa)
            Try

                Using odr As IDataReader = dam.BuildDataReader()

                    If (odr.Read()) Then
                        ' Il cf non è univoco
                        odr.Close()
                        ' Crea la stringa per recuperare dal database tutti i codici
                        ' che potrebbero collidere con il codice che sto generando
                        '***************************************************************************
                        ' DA MODIFICARE: PROBABILMENTE IN UNA CASISTICA MEDIA RISULTA PIU' 
                        ' EFFICIENTE CONTROLLARE UN CODICE ALLA VOLTA ESEGUENDO UNA SELECT
                        ' PER OGNI CODICE GENERATO !
                        '***************************************************************************
                        Dim cft As String
                        cft = Replace(cf, "-", "%", , , CompareMethod.Text)

                        dam.QB.NewQuery()
                        dam.QB.AddSelectFields("paz_codice")
                        dam.QB.AddTables(T_PAZ_PAZIENTI)
                        dam.QB.AddWhereCondition("paz_codice", Comparatori.Like, cft, DataTypes.Stringa)

                        Dim dt As New DataTable("Codici")
                        dam.BuildDataTable(dt)

                        ' Imposta la chiave primaria della tabella per effettuare le ricerche
                        ' del codice paziente in seguito
                        Dim PrimaryKeyColumns(0) As DataColumn
                        PrimaryKeyColumns(0) = dt.Columns(0)
                        dt.PrimaryKey = PrimaryKeyColumns

                        Dim dr As DataRow
                        Dim i As Integer = 1
                        Dim j As Integer = 1

                        i = InStr(i, cf, "-", CompareMethod.Text)

                        If i > 0 Then

                            ' Esiste almeno un carattere "-" nella stringa del codice fiscale
                            While (found And i <> 0)
                                ' Trovato un carattere "-" nella stringa del codice fiscale.
                                ' Come specificato nel documento Server.doc, il codice univoco
                                ' viene generato sostituendo i caratteri "-" con lettere dell'
                                ' alfabeto in modo sequenziale (prima A poi B ecc.)
                                j = 65
                                While (found And j <= 90)
                                    ' Genera un codice univoco di prova.....
                                    cft = Replace(cf, "-", Chr(j), i, 1, CompareMethod.Text)
                                    cft = Strings.Mid(cf, 1, i - 1) & cft
                                    ' ....e ne verifica l'unicita confrontandolo con quelli
                                    ' restituiti dalla precedente ricerca sul DB (tutto
                                    ' deve avvenire sotto la medesima transazione)
                                    dr = dt.Rows.Find(cft)
                                    If dr Is Nothing Then
                                        ' Il codice cft generato non è presente in anagrafe 
                                        ' centrale quindi è il codice univoco desiderato
                                        found = False
                                    Else
                                        j += 1
                                    End If
                                End While
                                i = InStr(i + 1, cf, "-", CompareMethod.Text)
                            End While
                            ' Quando esco dal while ho trovato un codice univoco (cft)
                            cf = cft
                        Else
                            ' Il codice generato è il codice fiscale corretto. Potrebbe già esistere
                            ' un codice uguale in anagrafe centrale, quindi occorre assicurarsi che sia univoco
                            i = cf.Length
                            While (found And i > 0)
                                j = 65
                                While (found And j < 91)

                                    cft = cf.Substring(0, i - 1) & Chr(j) & cf.Substring(i, cf.Length - i)
                                    dam.QB.NewQuery()
                                    dam.QB.AddSelectFields("paz_codice")
                                    dam.QB.AddTables(T_PAZ_PAZIENTI)
                                    dam.QB.AddWhereCondition("paz_codice", Comparatori.Uguale, cft, DataTypes.Stringa)
                                    Using odr2 As IDataReader = dam.BuildDataReader
                                        If (Not odr2.Read) Then
                                            ' Il codice cft generato non è presente in anagrafe 
                                            ' centrale quindi è il codice univoco desiderato
                                            found = False
                                        Else
                                            j += 1
                                        End If
                                    End Using

                                End While
                                i -= 1
                            End While

                            ' Quando esco ho trovato il codice fiscale univoco
                            cf = cft
                        End If
                    End If
                    ' A questo punto cf è il codice univoco
                    ucf = cf
                    GenUniCode = True

                End Using

            Catch exc As Exception
                errMessage = exc.Message
                GenUniCode = False
            End Try

        End If

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Questa funzione permette di calcolare il codice fiscale di un paziente; non tiene conto
    ''' di eventuali doppioni, per i quali il solo Ministero delle finanze emette il codice corretto,
    ''' ne' tiene conto di eventuali codici fiscali sbagliati rilasciati dal ministero.
    ''' </summary>
    ''' <param name="Cognome">cognome del paziente di cui si vuole calcolare il codice fiscale</param>
    ''' <param name="Nome">nome del paziente</param>
    ''' <param name="DataNascita">data di nascita del paziente; E' necessario che la data sia 
    '''                      presentata come una stringa nel formato gg/mm/aaaa</param>
    ''' <param name="ComCod">codice del comune di nascita del paziente</param>
    ''' <param name="sesso">sesso del paziente</param>
    ''' <param name="dam">oggetto Dam per la connessione all'anagrafe centrale</param>
    ''' <param name="strErrMessage">stringa contenente il messaggio di errore nel caso in cui venga
    '''                      sollevata un'eccezione</param>
    ''' <param name="cf">codice fiscale del paziente che include il carattere "-" per
    '''                      quei campi che non è stato possibile calcolare.</param>
    ''' <returns>True se tutto si è svolto regolarmente, altrimenti false</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adesimone]	10/06/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function CFGen(Cognome As String, Nome As String, DataNascita As String, ComCod As String,
                           sesso As String, dam As IDAM, ByRef strErrMessage As String, ByRef cf As String) As Boolean

        ' Stringa di vocali
        Dim vocali As String = "AEIOU"
        ' Stringa che permette di associare un mese ad una lettera
        Dim mesi As String = "ABCDEHLMPRST"
        ' Conta i caratteri delle stringhe da scorrere
        Dim count As Integer = 1
        ' Conta le consonanti di una stringa
        Dim cc As Integer = 0
        ' Conta le vocali di una stringa
        Dim cv As Integer = 0
        ' Vettore di caratteri contenente le vocali di una stringa
        Dim voc(Math.Max(Cognome.Length, Nome.Length)) As Char
        'Vettore di caratteri contenente le consonanti di una stringa
        Dim cons(Math.Max(Cognome.Length, Nome.Length)) As Char
        ' Contatore
        Dim i As Integer = 0

        ' Processa la stringa del cognome
        If Cognome = "" Then

            cf &= "---"

        Else

            ' Prepara la stringa rappresentante il cognome per la processazione
            Cognome = Replace(Cognome, " ", "", , , CompareMethod.Text).Replace("'", "")

            While count <= Cognome.Length

                If InStr(vocali, Strings.Mid(Cognome, count, 1), CompareMethod.Text) = 0 Then
                    ' Il carattere count del cognome non è una vocale => è una consonante
                    cons(cc) = Strings.Mid(Cognome, count, 1)
                    cc += 1
                Else
                    ' Il carattere count del cognome è una vocale
                    voc(cv) = Strings.Mid(Cognome, count, 1)
                    cv += 1
                End If

                count += 1

            End While

            ' Crea il codice fiscale parziale costituito dalle consonanti presenti nel cognome
            For i = 0 To Math.Min(cc - 1, 2)
                cf &= UCase(cons(i))
            Next

            If cc < 3 Then
                ' Nel cognome non ci sono tre consonanti
                ' inserisce le vocali del cognome
                For i = 0 To Math.Min(cv - 1, 3 - cc - 1)
                    cf &= UCase(voc(i))
                Next
                If cv < (3 - cc) Then
                    ' Non si ha un numero sufficiente di vocali per completare la parte di codice 
                    ' riservata al cognome
                    For i = 1 To (3 - cc - cv)
                        ' Riempie gli spazi mancanti con delle X
                        cf &= "X"
                    Next
                End If
            End If

        End If

        If Nome = "" Then

            cf &= "---"

        Else

            cc = 0
            cv = 0
            count = 1
            ' Prepara la stringa rappresentante il cognome per la processazione
            Nome = Replace(Nome, " ", "", , , CompareMethod.Text).Replace("'", "")

            ' Processa la stringa del nome
            While count <= Nome.Length

                If InStr(vocali, Strings.Mid(Nome, count, 1), CompareMethod.Text) = 0 Then
                    ' Il carattere count del cognome non è una vocale => è una consonante
                    cons(cc) = Strings.Mid(Nome, count, 1)
                    cc += 1
                Else
                    ' Il carattere count del nome è una vocale
                    voc(cv) = Strings.Mid(Nome, count, 1)
                    cv += 1
                End If

                count += 1

            End While

            ' Se il nome ha almeno quattro consonanti, la seconda viene scartata
            If cc >= 4 Then
                For i = 1 To cc - 1
                    cons(i) = cons(i + 1)
                Next
                cc -= 1
            End If

            ' Crea il codice fiscale parziale costituito dalle consonanti presenti nel nome
            For i = 0 To Math.Min(cc - 1, 2)
                cf &= UCase(cons(i))
            Next

            If cc < 3 Then
                ' Nel cognome non ci sono tre consonanti
                ' inserisco le vocali del cognome
                For i = 0 To Math.Min(cv - 1, 3 - cc - 1)
                    cf &= UCase(voc(i))
                Next
                If cv < (3 - cc) Then
                    ' Non si ha un numero sufficiente di vocali per completare la parte di codice 
                    ' riservata al cognome
                    For i = 1 To (3 - cc - cv)
                        ' Riempie gli spazi mancanti con delle X
                        cf &= "X"
                    Next
                End If
            End If

        End If

        ' Calcola la parte del codice fiscale proveniente dalla data di nascita
        If DataNascita = "" Then

            cf &= "-----"

        Else

            ' Imposta la porzione del codice fiscale relativa all'anno di nascita
            cf &= Strings.Mid(DataNascita, InStrRev(DataNascita, "/", , CompareMethod.Text) + 3, 2)
            ' Imposta la porzione di codice fiscale relativa al mese di nascita
            cf &= Strings.Mid(mesi, (CType(Strings.Mid(DataNascita, InStr(DataNascita, "/", CompareMethod.Text) + 1, 2), Integer)), 1)
            ' Imposta la porzione del codice fiscale relativa al giorno di nascita
            If UCase(sesso) = "F" Then
                ' Nel caso delle donne al giorno di nascita occorre aggiungere il valore 40
                cf &= (CType(Strings.Mid(DataNascita, 1, 2), Integer) + 40).ToString()
            Else
                ' Nel caso degli uomini va preso il giorno di nascita così com'è
                cf &= Strings.Mid(DataNascita, 1, 2)
            End If

        End If

        ' Calcola la parte del codice fiscale derivante dal comune di nascita; per farlo
        ' viene utilizzato il codice catastale del comune così come riportato nella tabella
        ' T_ANA_COMUNI del database onit_anagrafi_centrali (ricavato tramite accesso alle viste
        ' sulla tabella locali al'applicativo)
        If ComCod = "" Then

            cf &= "----"

        Else

            ' Lavora sempre in anagrafe centrale per reperire il codice catastale del comune di nascita
            dam.QB.NewQuery()
            dam.QB.AddSelectFields("com_catastale")
            dam.QB.AddTables("t_ana_comuni")
            dam.QB.AddWhereCondition("com_codice", Comparatori.Uguale, ComCod, DataTypes.Stringa)

            Try

                Using odr As IDataReader = dam.BuildDataReader()
                    i = 0
                    Dim tmp As String = ""
                    While odr.Read()
                        tmp = odr(0).ToString()
                        i += 1
                    End While
                    If (i > 1 Or i = 0) Then
                        ' Trovati più comuni o nessun comune che matchano nel database
                        cf &= "XXXX"
                    Else
                        If (i = 1) Then
                            cf &= tmp
                        End If
                    End If
                End Using

            Catch exc As Exception
                strErrMessage = exc.Message
                CFGen = False
            End Try

        End If

        ' Aggiunge il carattere finale al codice fiscale
        cf &= LastChar(cf)
        CFGen = True

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Calcola il carattere finale del codice fiscale. Per tale calcolo si utilizzano 3 tabelle di conversione (vedi algoritmo).
    ''' </summary>
    ''' <param name="cf">cf [in] - stringa contenente i primi 15 caratteri del codice fiscale</param>
    ''' <returns>il carattere finale del codice fiscale</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adesimone]	10/06/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Function LastChar(cf As String) As String

        Dim lettere As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        Dim s_car As String
        Dim i_pos, value, i_cnt, i_totale As Integer
        Dim tabA As Integer() = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25}
        Dim tabB As Integer() = {1, 0, 5, 7, 9, 13, 15, 17, 19, 21, 1, 0, 5, 7, 9, 13, 15, 17, 19, 21, 2, 4, 18, 20, 11, 3, 6, 8, 12, 14, 16, 10, 22, 25, 24, 23}
        Dim tabC As String() = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"}

        ' Somma i primi 15 caratteri del codice in base al loro peso dipendente dalla 
        ' posizione nella sequenza (si distingue tra posizioni pari e dispari).
        ' Divide la somma per 26 e considera il resto
        ' Rimappa tale resto nel carattere corrispondente (0 => A, 1 => B, ...)
        i_totale = 0
        For i_cnt = 1 To Len(cf)

            s_car = Strings.Mid(cf, i_cnt, 1)

            If s_car = "-" Then
                value = 0
            Else
                If IsNumeric(s_car) Then
                    i_pos = CInt(s_car)
                Else
                    i_pos = InStr(lettere, s_car, CompareMethod.Text) + 9
                End If

                If i_cnt Mod 2 = 0 Then
                    value = tabA(i_pos)
                Else
                    value = tabB(i_pos)
                End If

                i_totale = i_totale + value
            End If
        Next

        'Ottiene l'indice del carattere di controllo	    
        Return tabC(i_totale Mod 26)

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Questa funzione verifica che venga rispettato il vincolo di integrità tra la
    '''        tabella t_paz_pazienti e le tabelle t_ana_comuni,t_ana_cittadinanza,t_ana_regioni,
    '''        t_ana_usl,t_ana_medici; l'integrità è verificata sulle viste locali dei rispettivi
    '''        applicativi sul db onit_anagrafi_centrali
    ''' </summary>
    ''' <param name="TableName">Nome della tabella su cui verificare l'integrità</param>
    ''' <param name="FieldName">Nome del campo su cui verificare l'integrità</param>
    ''' <param name="value">Valore del campo da verificare</param>
    ''' <param name="Dam">Connessione al db Onit_anagrafi_centrali</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adesimone]	10/06/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function CheckIntegrity(tableName As String, fieldName As String, value As String, dam As IDAM) As String

        Dim retstr As String

        'se il valore è omesso, ritorna nothing
        If value Is Nothing Then
            Return Nothing
        End If

        dam.QB.NewQuery()
        dam.QB.AddSelectFields(fieldName)
        dam.QB.AddTables(tableName)
        dam.QB.AddWhereCondition(fieldName, Comparatori.Uguale, value, DataTypes.Stringa)

        Using odr As IDataReader = dam.BuildDataReader()

            If odr.Read() Then
                retstr = odr(0).ToString()
            Else
                retstr = ""
            End If

        End Using

        Return retstr

    End Function

    Private Function CodicePazUnivoco(codicePaziente As String, ByRef errorMessage As String) As Boolean

        If String.IsNullOrEmpty(codicePaziente) Then Return True

        Dim bln As Boolean

        Using dam As IDAM = GetDAMCentrale()

            bln = CodicePazUnivoco(codicePaziente, dam, errorMessage)

        End Using

        Return bln

    End Function

    Private Function CodicePazUnivoco(strPazCodice As String, dam As IDAM, ByRef strErrMessage As String) As Boolean

        Dim blnRet As Boolean = False

        dam.QB.NewQuery()
        dam.QB.AddSelectFields(dam.QB.FC.Count("*"))
        dam.QB.AddTables(T_PAZ_PAZIENTI)
        dam.QB.AddWhereCondition("paz_codice", Comparatori.Uguale, strPazCodice, DataTypes.Stringa)

        Try
            Dim objVal As Object = dam.ExecScalar()

            blnRet = (objVal Is Nothing OrElse CInt(objVal) = 0)

            If Not blnRet Then
                strErrMessage = "Il codice paziente passate esiste già nella " + T_PAZ_PAZIENTI
            End If

        Catch ex As Exception
            blnRet = False
            strErrMessage = ex.Message + vbCrLf + ex.StackTrace
        End Try

        Return blnRet

    End Function

    ''' <summary>
    ''' nel caso di codici istat dei comuni stranieri fuori standard (con 3 cifre) vengono modificati apponendo il prefisso 999
    ''' </summary>
    ''' <param name="dtPazienti"></param>
    ''' <remarks></remarks>
    Private Sub ValidaCodiciIstat(dtPazienti As DataTable)

        If dtPazienti.Columns.Contains("paz_com_istat_residenza") AndAlso
           dtPazienti.Columns.Contains("paz_com_istat_nascita") AndAlso
           dtPazienti.Columns.Contains("paz_com_istat_domicilio") Then

            Dim row As DataRow
            Dim strPrefix As String = "999"

            For i As Integer = 0 To dtPazienti.Rows.Count - 1
                row = dtPazienti.Rows(i)
                If row("paz_com_istat_residenza").ToString().Length = 3 Then
                    row("paz_com_istat_residenza") = strPrefix + row("paz_com_istat_residenza")
                End If
                If row("paz_com_istat_nascita").ToString().Length = 3 Then
                    row("paz_com_istat_nascita") = strPrefix + row("paz_com_istat_nascita")
                End If
                If row("paz_com_istat_domicilio").ToString().Length = 3 Then
                    row("paz_com_istat_domicilio") = strPrefix + row("paz_com_istat_domicilio")
                End If
                row.AcceptChanges()
            Next

        End If
    End Sub

#End Region

End Class
