Imports System.Collections.Generic

Imports Onit.OnAssistnet.Data

Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Log
Imports Onit.OnAssistnet.OnVac.Common.Utility
Imports Onit.OnAssistnet.OnVac.Entities


Public Class BizLotti
    Inherits Biz.BizClass

#Region " Constructors "

    ''' <summary>
    ''' Costruttore classe BizLotti. 
    ''' </summary>
    ''' <param name="genericprovider"></param>
    ''' <param name="settings"></param>
    ''' <param name="contextInfos"></param>
    ''' <remarks></remarks>
    Public Sub New(genericProvider As DbGenericProvider, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos)

        Me.New(genericProvider, settings, contextInfos, New BizLogOptions(DataLogStructure.TipiArgomento.MAGAZZINO))

    End Sub

    Public Sub New(genericProvider As DbGenericProvider, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos, bizLogOptions As BizLogOptions)

        MyBase.New(genericProvider, settings, contextInfos, bizLogOptions)

    End Sub

    Public Sub New(dbGenericProviderFactory As DbGenericProviderFactory, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)

        MyBase.New(dbGenericProviderFactory, settings, Nothing, contextInfos, logOptions)

    End Sub

#End Region

#Region " Constants "

    Const LUNGHEZZA_CAMPO_NUMERO_DOSI As Integer = 8
    Const LUNGHEZZA_CAMPO_QUANTITA_MINIMA As Integer = 8

#End Region

#Region " Types "

#Region " Result class "

    Public Class BizLottiResult

        Public Enum ResultType
            Success = 0
            GenericWarning = 1
            IsActiveLottoWarning = 2
            LottoDisattivatoScortaNullaWarning = 3
            GenericError = 4
        End Enum

        Private _resultType As ResultType
        Public Property Result() As ResultType
            Get
                Return _resultType
            End Get
            Set(value As ResultType)
                _resultType = value
            End Set
        End Property

        Private _resultMessage As String
        Public Property Message() As String
            Get
                Return _resultMessage
            End Get
            Private Set(value As String)
                _resultMessage = value
            End Set
        End Property

        Public Sub New()
        End Sub

        Public Sub New(resultType As ResultType, message As String)
            Me.Result = resultType
            Me.Message = message
        End Sub

    End Class

    Public Class EtaAttivazioneLottoResult
        Public EtaMinima As Integer?
        Public EtaMassima As Integer?
    End Class

#End Region

#Region " Comparers "

    ''' <summary>
    ''' Comparer per eseguire il distinct delle righe in base a codice lotto e data esecuzione
    ''' </summary>
    ''' <remarks></remarks>
    Private Class DeletedCodiceLottoDataEsecuzioneLottiComparer
        Implements IEqualityComparer(Of DataRow)

        Public Function Equals1(x As System.Data.DataRow, y As System.Data.DataRow) As Boolean Implements System.Collections.Generic.IEqualityComparer(Of System.Data.DataRow).Equals

            If (x("ves_lot_codice", DataRowVersion.Original) Is DBNull.Value AndAlso y("ves_lot_codice", DataRowVersion.Original) Is DBNull.Value) OrElse
               (x("ves_data_effettuazione", DataRowVersion.Original) Is DBNull.Value AndAlso y("ves_data_effettuazione", DataRowVersion.Original) Is DBNull.Value) Then

                Return False

            End If

            Return (x("ves_lot_codice", DataRowVersion.Original) = y("ves_lot_codice", DataRowVersion.Original) And
                    x("ves_data_effettuazione", DataRowVersion.Original) = y("ves_data_effettuazione", DataRowVersion.Original))

        End Function

        Public Function GetHashCode1(obj As System.Data.DataRow) As Integer Implements System.Collections.Generic.IEqualityComparer(Of System.Data.DataRow).GetHashCode

            Return obj("ves_lot_codice", DataRowVersion.Original).GetHashCode() + obj("ves_data_effettuazione", DataRowVersion.Original).GetHashCode()

        End Function

    End Class

    ''' <summary>
    ''' Comparer per eseguire il distinct dei dati dei lotti eliminati in base al campo CodiceLotto
    ''' </summary>
    ''' <remarks></remarks>
    Private Class DeletedCodiciLottiComparer
        Implements IEqualityComparer(Of Entities.DatiLottoEliminato)

        Public Function Equals1(x As Entities.DatiLottoEliminato, y As Entities.DatiLottoEliminato) As Boolean Implements System.Collections.Generic.IEqualityComparer(Of Entities.DatiLottoEliminato).Equals

            If String.IsNullOrEmpty(x.CodiceLotto) AndAlso String.IsNullOrEmpty(y.CodiceLotto) Then

                Return False

            End If

            Return (x.CodiceLotto = y.CodiceLotto)

        End Function

        Public Function GetHashCode1(obj As Entities.DatiLottoEliminato) As Integer Implements System.Collections.Generic.IEqualityComparer(Of Entities.DatiLottoEliminato).GetHashCode

            Return obj.CodiceLotto.GetHashCode()

        End Function

    End Class

#End Region

#End Region

#Region " Shared Methods "

    ''' <summary>
    ''' Scrittura del log per segnalare l'annullamento da parte dell'utente dell'inserimento di un movimento di carico per 
    ''' ripristinare le dosi in seguito all'eliminazione di una vaccinazione eseguita.
    ''' </summary>
    ''' <param name="codiceLotto"></param>
    ''' <param name="codiceConsultorioMagazzino"></param>
    ''' <param name="dataEsecuzione"></param>
    ''' <param name="numeroDosi"></param>
    ''' <param name="idAssociazione"></param>
    ''' <remarks></remarks>
    Public Shared Sub WriteLogAnnullamentoRipristinoCaricoLotto(codiceLotto As String, codiceConsultorioMagazzino As String, dataEsecuzione As DateTime, numeroDosi As Integer, idAssociazione As String)

        Dim testataLog As New DataLogStructure.Testata(DataLogStructure.TipiArgomento.MAGAZZINO, DataLogStructure.Operazione.Generico, 0, True)

        Dim recordLog As New DataLogStructure.Record()

        recordLog.Campi.Add(New DataLogStructure.Campo("CodiceLotto", codiceLotto))
        recordLog.Campi.Add(New DataLogStructure.Campo("CodiceCentroVaccinale", codiceConsultorioMagazzino))
        recordLog.Campi.Add(NewCampoDateTimeToLog("DataEsecuzioneVaccinazione", dataEsecuzione))
        recordLog.Campi.Add(New DataLogStructure.Campo("NumeroDosiNonRipristinate", numeroDosi.ToString()))
        recordLog.Campi.Add(New DataLogStructure.Campo("IdAssociazione", idAssociazione))
        recordLog.Campi.Add(New DataLogStructure.Campo("Note", "Carico per ripristino dose in seguito a canc vacc eseguita non effettuato: annullamento dell'utente"))

        testataLog.Records.Add(recordLog)

        LogBox.WriteData(testataLog)

    End Sub

    ''' <summary>
    ''' Lista di codici (distinti) dei lotti utilizzati nelle vaccinazioni eseguite che sono state eliminate
    ''' </summary>
    ''' <param name="datiLottiEliminati"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetListDistinctCodiciLottiEliminati(datiLottiEliminati As List(Of Entities.DatiLottoEliminato)) As List(Of String)

        Return datiLottiEliminati.Distinct(New DeletedCodiciLottiComparer()).Select(Function(l) l.CodiceLotto).ToList()

    End Function

    ''' <summary>
    ''' Restituisce le note che verranno inserite nel movimento relativo 
    ''' all'esecuzione o alla cancellazione di una vaccinazione
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataEsecuzione"></param>
    ''' <param name="vaccinazioni"></param>
    ''' <param name="tipoMovimento"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ImpostaNoteMovimentoVaccinazione(codicePaziente As Integer, dataEsecuzione As DateTime, vaccinazioni As String, tipoMovimento As String) As String

        Dim noteMovimento As New System.Text.StringBuilder()

        If tipoMovimento = Constants.TipoMovimentoMagazzino.Carico Then

            noteMovimento.Append("Canc. vacc.")

        ElseIf tipoMovimento = Constants.TipoMovimentoMagazzino.Scarico Then

            noteMovimento.Append("Esec. vacc.")

        End If

        noteMovimento.AppendFormat(" - Paz: {0}", codicePaziente.ToString())

        If dataEsecuzione > DateTime.MinValue Then
            noteMovimento.AppendFormat(" - {0:dd/MM/yyyy}", dataEsecuzione)
        End If

        If Not String.IsNullOrEmpty(vaccinazioni) Then
            noteMovimento.AppendFormat(" - Vacc: {0}", vaccinazioni)
        End If

        Return noteMovimento.ToString()

    End Function

    ' ''' <summary>
    ' ''' Restituisce un oggetto di tipo LottoMagazzino partendo da uno di tipo Lotto
    ' ''' </summary>
    ' ''' <param name="lotto"></param>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Shared Function CreateLottoMagazzino(lotto As Entities.Lotto) As Entities.LottoMagazzino

    '    Dim lottoMagazzino As New Entities.LottoMagazzino()

    '    lottoMagazzino.Ditta = lotto.Fornitore
    '    lottoMagazzino.Obsoleto = lotto.Annullato
    '    lottoMagazzino.Attivo = lotto.Attivo
    '    lottoMagazzino.CodiceLotto = lotto.Codice
    '    lottoMagazzino.DescrizioneLotto = lotto.Descrizione
    '    lottoMagazzino.CodiceNomeCommerciale = lotto.CodNC
    '    lottoMagazzino.DescrizioneNomeCommerciale = lotto.DescNC
    '    lottoMagazzino.DosiRimaste = lotto.DosiRimaste
    '    lottoMagazzino.DosiScatola = lotto.DosiScatola
    '    lottoMagazzino.DataPreparazione = GetDateTimeFromObject(lotto.Preparazione)
    '    lottoMagazzino.QuantitaMinima = lotto.QtaMinima
    '    lottoMagazzino.DataScadenza = GetDateTimeFromObject(lotto.Scadenza)
    '    lottoMagazzino.Note = lotto.Note

    '    Return lottoMagazzino


    '    'Public QtaIniziale As Int16
    '    'Public UnitaMisura As Enumerators.UnitaMisuraLotto


    'End Function

    ''' <summary>
    ''' Restituisce un oggetto di tipo DatiOrdinamento, con l'ordinamento inverso rispetto a quello specificato 
    ''' se l'elemento appartiene alla lista dei campi a cui invertire l'ordine.
    ''' </summary>
    ''' <param name="campo"></param>
    ''' <param name="verso"></param>
    ''' <param name="campiOrdineInverso"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateDatiOrdinamento(campo As String, verso As String, campiOrdineInverso As String()) As Entities.DatiOrdinamento

        Dim datiOrdinamento As New Entities.DatiOrdinamento(campo, verso)

        If CheckInversioneOrdinamentoCampo(campo, campiOrdineInverso) Then

            datiOrdinamento.InvertiOrdinamento()

        End If

        Return datiOrdinamento

    End Function

#Region " Età attivazione lotto "

    ''' <summary>
    ''' Restituisce un oggetto di tipo Eta in base ai valori specificati
    ''' </summary>
    ''' <param name="valoreCampoAnni"></param>
    ''' <param name="valoreCampoMesi"></param>
    ''' <param name="valoreCampoGiorni"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetEtaFromValoreCampi(valoreCampoAnni As String, valoreCampoMesi As String, valoreCampoGiorni As String) As Entities.Eta
        '--
        Dim etaAttivazione As Entities.Eta = Nothing
        '--
        Dim anni As Integer = GetInt32FromString(valoreCampoAnni)
        Dim mesi As Integer = GetInt32FromString(valoreCampoMesi)
        Dim giorni As Integer = GetInt32FromString(valoreCampoGiorni)
        '--
        If anni >= 0 Or mesi >= 0 Or giorni >= 0 Then
            '--
            If anni < 0 Then anni = 0
            If mesi < 0 Then mesi = 0
            If giorni < 0 Then giorni = 0
            '--
            etaAttivazione = New Entities.Eta(giorni, mesi, anni)
            '--
        End If
        '--
        Return etaAttivazione
        '--
    End Function

    Private Shared Function GetInt32FromString(value As String) As Integer?

        Dim intValue As Integer = -1

        If Not String.IsNullOrEmpty(value) Then

            If Not Integer.TryParse(value, intValue) Then
                intValue = -1
            End If

        End If

        Return intValue

    End Function

#End Region

#Region " Stampa Movimentazione quantitativi "

    ''' <summary>
    ''' Controllo valorizzazione filtri
    ''' </summary>
    ''' <param name="filtro"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CheckFiltriStampaQuantitativiMovimentati(filtro As Filters.FiltriStampaQuantitaLottiMovimentati) As BizLottiResult

        ' Controllo valorizzazione almeno un filtro
        If filtro Is Nothing Then
            Return New BizLottiResult(BizLottiResult.ResultType.GenericError, "Nessun filtro valorizzato.")
        End If

        If filtro.DataInizioRegistrazione = Date.MinValue AndAlso
           filtro.DataFineRegistrazione = Date.MinValue AndAlso
           String.IsNullOrEmpty(filtro.IdUtente) AndAlso
           String.IsNullOrEmpty(filtro.CodiceLotto) AndAlso
           String.IsNullOrEmpty(filtro.CodiceConsultorio) AndAlso
           String.IsNullOrEmpty(filtro.Quantita) AndAlso
           String.IsNullOrEmpty(filtro.OperatoreConfrontoQuantita) AndAlso
           String.IsNullOrEmpty(filtro.TipoMovimento) Then

            Return New BizLottiResult(BizLottiResult.ResultType.GenericError, "Nessun filtro valorizzato.")

        End If

        ' Controllo id utente
        If Not String.IsNullOrEmpty(filtro.IdUtente) Then

            If Not Integer.TryParse(filtro.IdUtente, Nothing) Then
                Return New BizLottiResult(BizLottiResult.ResultType.GenericError, "Id utente errato.")
            End If

        End If

        ' Controllo date di registrazione
        If filtro.DataInizioRegistrazione > Date.MinValue AndAlso
           filtro.DataFineRegistrazione > Date.MinValue AndAlso
           filtro.DataInizioRegistrazione > filtro.DataFineRegistrazione Then

            Return New BizLottiResult(BizLottiResult.ResultType.GenericError, "La data di inizio non puo\' essere superiore alla data di fine periodo.")

        End If

        If Not String.IsNullOrEmpty(filtro.Quantita) Then

            ' Controllo quantità
            Dim qta As Integer = 0
            If (Not Integer.TryParse(filtro.Quantita, qta)) Then
                Return New BizLottiResult(BizLottiResult.ResultType.GenericError, "La quantità deve essere un numero intero positivo.")
            Else
                If qta < 0 Then Return New BizLottiResult(BizLottiResult.ResultType.GenericError, "La quantità deve essere un numero intero positivo.")
            End If

            ' Controllo operatore confronto
            Select Case filtro.OperatoreConfrontoQuantita
                Case Constants.OperatoreConfronto.Maggiore, Constants.OperatoreConfronto.Minore, Constants.OperatoreConfronto.Uguale
                    ' OK
                Case Else
                    Return New BizLottiResult(BizLottiResult.ResultType.GenericError, "Operatore di confronto non riconosciuto.")
            End Select

        End If

        ' Controllo tipo movimento
        If Not String.IsNullOrEmpty(filtro.TipoMovimento) Then

            Select Case filtro.TipoMovimento
                Case Constants.TipoMovimentoMagazzino.Carico, Constants.TipoMovimentoMagazzino.Scarico,
                     Constants.TipoMovimentoMagazzino.TrasferimentoDa, Constants.TipoMovimentoMagazzino.TrasferimentoA
                    'OK
                Case Else
                    Return New BizLottiResult(BizLottiResult.ResultType.GenericError, "Tipo di movimento non previsto.")
            End Select

        End If

        Return New BizLottiResult(BizLottiResult.ResultType.Success, String.Empty)

    End Function

    ''' <summary>
    ''' Restituisce la stringa con i filtri da passare al report
    ''' </summary>
    ''' <param name="filtro"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetReportFilterStampaQuantitativiMovimentati(filtro As Filters.FiltriStampaQuantitaLottiMovimentati) As String

        Dim stbFiltro As New System.Text.StringBuilder()

        ' Aggiungo il filtro dei cns per l'operatore connesso 
        stbFiltro.AppendFormat("{{T_ANA_LINK_UTENTI_CONSULTORI.LUC_UTE_ID}} = {0} AND ", filtro.IdUtenteConnesso)

        ' Filtro fisso
        stbFiltro.Append("(isnull({T_ANA_CONSULTORI.CNS_CNS_MAGAZZINO}) OR {T_ANA_CONSULTORI.CNS_CNS_MAGAZZINO} = {T_ANA_CONSULTORI.CNS_CODICE}) AND ")

        ' Filtri impostati dall'utente

        ' Lotto
        If Not String.IsNullOrEmpty(filtro.CodiceLotto) Then
            stbFiltro.AppendFormat("{{T_ANA_LOTTI.LOT_CODICE}} = '{0}' AND ",
                                   filtro.CodiceLotto)
        End If

        ' Consultorio
        If Not String.IsNullOrEmpty(filtro.CodiceConsultorio) Then
            stbFiltro.AppendFormat("{{T_ANA_CONSULTORI.CNS_CODICE}} = '{0}' AND ",
                                   filtro.CodiceConsultorio)
        End If

        ' Date di registrazione
        If filtro.DataInizioRegistrazione > Date.MinValue Then
            stbFiltro.AppendFormat("{{T_LOT_MOVIMENTI.MMA_DATA_REGISTRAZIONE}} >=  DateTime ({0},{1},{2}, 00, 00, 00) AND ",
                                   filtro.DataInizioRegistrazione.Year,
                                   filtro.DataInizioRegistrazione.Month,
                                   filtro.DataInizioRegistrazione.Day)
        End If

        If filtro.DataFineRegistrazione > Date.MinValue Then
            stbFiltro.AppendFormat("{{T_LOT_MOVIMENTI.MMA_DATA_REGISTRAZIONE}} <=  DateTime ({0},{1},{2}, 23, 59, 59) AND ",
                                   filtro.DataFineRegistrazione.Year,
                                   filtro.DataFineRegistrazione.Month,
                                   filtro.DataFineRegistrazione.Day)
        End If

        ' Utente
        If Not String.IsNullOrEmpty(filtro.IdUtente) Then
            stbFiltro.AppendFormat("{{T_LOT_MOVIMENTI.MMA_UTE_ID}} = {0} AND ", filtro.IdUtente)
        End If

        ' Quantita
        If Not String.IsNullOrEmpty(filtro.Quantita) Then
            stbFiltro.AppendFormat("{{T_LOT_MOVIMENTI.MMA_N_DOSI}} {0} {1} AND ", filtro.OperatoreConfrontoQuantita, filtro.Quantita)
        End If

        ' Tipo movimento
        If Not String.IsNullOrEmpty(filtro.TipoMovimento) Then
            stbFiltro.AppendFormat("{{T_LOT_MOVIMENTI.MMA_TIPO}} = '{0}' AND ", filtro.TipoMovimento)
        End If

        stbFiltro.Remove(stbFiltro.Length - 4, 4)

        Return stbFiltro.ToString()

    End Function

    Public Shared Function GetDescrizioneMovimentoMagazzinoByTipo(tipoMovimentoMagazzino As String) As String

        Select Case tipoMovimentoMagazzino
            Case Constants.TipoMovimentoMagazzino.Carico
                Return "Carico"
            Case Constants.TipoMovimentoMagazzino.Scarico
                Return "Scarico"
            Case Constants.TipoMovimentoMagazzino.TrasferimentoA
                Return "Trasferimento A"
            Case Constants.TipoMovimentoMagazzino.TrasferimentoDa
                Return "Trasferimento Da"
        End Select

        Return String.Empty

    End Function

#End Region

#Region " Gestione Lock "

    ''' <summary>
    ''' Questo metodo apre un lock per ogni lotto nella lista degli eseguiti e restituisce la lista dei lock aperti.
    ''' </summary>
    ''' <param name="codiceLottoEseguito"></param>
    ''' <param name="idApplicazione"></param>
    ''' <param name="codiceConsultorioMagazzino"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function EnterLockLotto(codiceLottoEseguito As String, idApplicazione As String, codiceConsultorioMagazzino As String) As Onit.Shared.Manager.Lock.Lock

        Dim codiceAziendaToLock As String = BizLotti.GetCodiceAziendaToLockLotti(idApplicazione)

        ' Lock di ogni lotto utilizzato presente in codiciLottiEseguiti
        Dim lockLotto As New Onit.Shared.Manager.Lock.Lock()

        lockLotto.EnterLock(codiceAziendaToLock, BizLotti.GetCodiceRisorsaToLockLotti(codiceLottoEseguito, codiceConsultorioMagazzino))

        Return lockLotto

    End Function

    ''' <summary>
    ''' Metodo per la chiusura di un lock sui lotti
    ''' </summary>
    ''' <param name="lockLotto"></param>
    ''' <remarks></remarks>
    Public Shared Sub ExitLockLotto(lockLotto As Onit.Shared.Manager.Lock.Lock)

        If Not lockLotto Is Nothing Then lockLotto.ExitLock()

    End Sub

    ''' <summary>
    ''' Metodo per la chiusura di una lista di lock sui lotti
    ''' </summary>
    ''' <param name="listLockLotti"></param>
    ''' <remarks></remarks>
    Public Shared Sub ExitLockLotti(listLockLotti As List(Of Onit.Shared.Manager.Lock.Lock))

        If Not listLockLotti Is Nothing AndAlso listLockLotti.Count > 0 Then

            For Each lockLotto As Onit.Shared.Manager.Lock.Lock In listLockLotti

                lockLotto.ExitLock()

            Next

        End If

    End Sub

    ' TODO [Magazzino]: AppId al posto di CodiceAzienda per gestire il caso multiAsl (Veneto)
    '                   Se OnVac supporterà la multiazienda, questo dovrebbe diventare CodiceAzienda
    '                   In più, l'AppId deve essere limitato a 20 caratteri per non dare problemi. 
    '                   => refactor (AppId + CodiceAzienda) ?!?!
    Private Shared Function GetCodiceAziendaToLockLotti(appId As String) As String

        If appId.Length > 20 Then

            appId = appId.Substring(0, 20)

        End If

        Return appId

    End Function

    Private Shared Function GetCodiceRisorsaToLockLotti(codiceLotto As String, codiceConsultorio As String) As String

        Dim codiceRisorsa As String = codiceLotto

        If Not String.IsNullOrEmpty(codiceConsultorio) Then
            codiceRisorsa = String.Format("{0}|{1}", codiceLotto, codiceConsultorio)
        End If

        Return String.Format("MAG|{0}", codiceRisorsa)

    End Function

#End Region

#Region " Private Shared "

    Private Shared Function GetDateTimeFromObject(value As Object) As DateTime

        Dim dataScadenza As DateTime

        If value Is Nothing Then
            dataScadenza = Date.MinValue
        Else
            Try
                dataScadenza = Convert.ToDateTime(value)
            Catch ex As Exception
                dataScadenza = Date.MinValue
            End Try
        End If

        Return dataScadenza

    End Function

#End Region

#End Region

#Region " Magazzino "

    ''' <summary>
    ''' Restituisce true se il consultorio di provenienza è magazzino per il consultorio di destinazione.
    ''' </summary>
    ''' <param name="codiceConsultorioProvenienza"></param>
    ''' <param name="codiceConsultorioDestinazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsConsultorioMagazzino(codiceConsultorioProvenienza As String, codiceConsultorioDestinazione As String) As Boolean

        Dim magazzinoDestinazione As Entities.Consultorio =
            Me.GenericProvider.Consultori.GetConsultorioMagazzino(codiceConsultorioDestinazione)

        Return (magazzinoDestinazione.Codice = codiceConsultorioProvenienza)

    End Function

#End Region

#Region " Lotti "

    ''' <summary>
    ''' Caricamento lotti
    ''' </summary>
    ''' <param name="codiceConsultorioCorrente"></param>
    ''' <param name="codiceConsultorioMagazzino"></param>
    ''' <param name="filtriRicerca"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LoadLottiMagazzino(codiceConsultorioCorrente As String, codiceConsultorioMagazzino As String, filtriRicerca As Filters.FiltriRicercaLottiMagazzino) As List(Of Entities.LottoMagazzino)

        If filtriRicerca Is Nothing Then
            filtriRicerca = New Filters.FiltriRicercaLottiMagazzino()
        End If

        Return GenericProvider.Lotti.LoadLottiMagazzino(codiceConsultorioCorrente, codiceConsultorioMagazzino, String.Empty, filtriRicerca)

    End Function

    Public Function GetLotti(idRSA As String, escludiLottiGiacenzaZero As Boolean) As List(Of Lotto)

        Return GenericProvider.Lotti.GetLotti(idRSA, escludiLottiGiacenzaZero)

    End Function

    ''' <summary>
    ''' Caricamento lotto
    ''' </summary>
    ''' <param name="codiceConsultorioCorrente"></param>
    ''' <param name="codiceConsultorioMagazzino"></param>
    ''' <param name="codiceLotto"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LoadLottoMagazzino(codiceConsultorioCorrente As String, codiceConsultorioMagazzino As String, codiceLotto As String) As Entities.LottoMagazzino

        Dim listLottiMagazzino As List(Of Entities.LottoMagazzino) =
            GenericProvider.Lotti.LoadLottiMagazzino(codiceConsultorioCorrente, codiceConsultorioMagazzino, codiceLotto, Nothing)

        If Not listLottiMagazzino Is Nothing AndAlso listLottiMagazzino.Count > 0 Then

            Return listLottiMagazzino(0)

        End If

        Return Nothing

    End Function

    ''' <summary>
    ''' Caricamento lotti di tutti i magazzini, con somma delle dosi rimaste
    ''' </summary>
    ''' <param name="filtriRicerca"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LoadLottiMagazzinoCentrale(filtriRicerca As Filters.FiltriRicercaLottiMagazzino, listDatiOrdinamento As List(Of Entities.DatiOrdinamento), pagingOptions As PagingOptions, userId As Integer) As List(Of Entities.LottoMagazzino)

        Return GenericProvider.Lotti.LoadLottiMagazzinoCentrale(filtriRicerca, listDatiOrdinamento, pagingOptions, userId)

    End Function

    ''' <summary>
    ''' Restituisce il numero di lotti in base ai filtri impostati, per il magazzino centrale
    ''' </summary>
    ''' <param name="filtriRicerca"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountLottiMagazzinoCentrale(filtriRicerca As Filters.FiltriRicercaLottiMagazzino, userId As Integer) As Integer

        Return GenericProvider.Lotti.CountLottiMagazzinoCentrale(filtriRicerca, userId)

    End Function

    ''' <summary>
    ''' Caricamento lotto
    ''' </summary>
    ''' <param name="codiceLotto"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LoadLottoMagazzinoCentrale(codiceLotto As String, idUtente As Integer, codDistretto As String) As Entities.LottoMagazzino

        Return GenericProvider.Lotti.LoadLottoMagazzinoCentrale(codiceLotto, idUtente, codDistretto)

    End Function

    ''' <summary>
    ''' Caricamento dettagli del lotto specificato relativi ad ogni magazzino, ordinato in base a campo e verso specificati.
    ''' </summary>
    ''' <param name="codiceLotto"></param>
    ''' <param name="noLottiScortaNulla"></param>
    ''' <param name="campoOrdinamento1"></param>
    ''' <param name="versoOrdinamento1"></param>
    ''' <param name="campoOrdinamento2"></param>
    ''' <param name="versoOrdinamento2"></param>
    ''' <param name="campoOrdinamento3"></param>
    ''' <param name="versoOrdinamento3"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LoadDettaglioDosiLotto(codiceLotto As String, noLottiScortaNulla As Boolean, codDistr As String, campoOrdinamento1 As String, versoOrdinamento1 As String, campoOrdinamento2 As String, versoOrdinamento2 As String, campoOrdinamento3 As String, versoOrdinamento3 As String) As List(Of Entities.LottoDettaglioMagazzino)

        Dim listDettaglioDosiLotto As List(Of Entities.LottoDettaglioMagazzino) = Me.GenericProvider.Lotti.LoadDettaglioDosiLotto(codiceLotto, noLottiScortaNulla, ContextInfos.IDUtente, codDistr)

        Dim listDatiOrdinamento As New List(Of Entities.DatiOrdinamento)()

        listDatiOrdinamento.Add(CreateDatiOrdinamento(campoOrdinamento1, versoOrdinamento1, Nothing))
        listDatiOrdinamento.Add(CreateDatiOrdinamento(campoOrdinamento2, versoOrdinamento2, Nothing))
        listDatiOrdinamento.Add(CreateDatiOrdinamento(campoOrdinamento3, versoOrdinamento3, Nothing))

        Return CreateOrderedList(Of Entities.LottoDettaglioMagazzino)(listDettaglioDosiLotto, listDatiOrdinamento)

    End Function

    ''' <summary>
    ''' Restituisce true se il lotto può essere attivato.
    ''' </summary>
    ''' <param name="isNewLotto"></param>
    ''' <param name="codiceLotto"></param>
    ''' <param name="codiceNomeCommerciale"></param>
    ''' <param name="codiceConsultorioLotto"></param>
    ''' <param name="codiceConsultorioCorrente"></param>
    ''' <param name="codiceConsultorioMagazzino"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsLottoAttivabile(isNewLotto As Boolean, codiceLotto As String, codiceNomeCommerciale As String, codiceConsultorioLotto As String,
                                      codiceConsultorioCorrente As String, codiceConsultorioMagazzino As String) As BizLottiResult

        ' In inserimento, isNewLotto = true. 
        ' In questo caso, controlla (nel consultorio corrente): 
        '   - se il lotto è scaduto => non attivabile
        '   - se c'è già un altro lotto attivo per lo stesso nome commerciale => non attivabile
        ' Se il lotto specificato non è scaduto e non ci sono altri lotti attivi per lo stesso nome commerciale, allora il lotto viene attivato (relativamente al consultorio corrente).

        ' In modifica, isNewLotto = false. 
        ' In questo caso, controlla prima se il lotto è in consultorio: 
        '    - se il codice del consultorio del lotto è diverso dal codice del consultorio di magazzino, il lotto non è in consultorio => non attivabile.
        '    - se il lotto è in consultorio, effettua gli stessi controlli del caso precedente.

        If Not isNewLotto Then

            If codiceConsultorioLotto <> codiceConsultorioMagazzino Then

                ' Lotto non in consultorio => non attivabile
                Return New BizLottiResult(BizLottiResult.ResultType.GenericError, "Il lotto non è presente nel centro vaccinale corrente.")

            End If

        End If

        If Me.GenericProvider.Lotti.IsLottoScaduto(codiceLotto) Then

            ' Lotto scaduto => non attivabile
            Return New BizLottiResult(BizLottiResult.ResultType.GenericError, "Il lotto non può essere attivato perchè è scaduto.")

        End If

        ' Lotto da inserire o già in consultorio => può essere attivato solo se non esiste un altro lotto attivo, tra quelli del consultorio,
        '                                           a cui è già associato lo stesso nome commerciale.
        If Me.GenericProvider.Lotti.IsActiveAltroLottoStessoNomeCommerciale(codiceLotto, codiceNomeCommerciale, codiceConsultorioCorrente) Then

            Return New BizLottiResult(BizLottiResult.ResultType.GenericError, "Esiste un lotto con stesso nome commerciale già attivo nel centro vaccinale corrente.")

        End If

        Return New BizLottiResult(BizLottiResult.ResultType.Success, String.Empty)

    End Function

#Region " Ordinamento lotti "

    ''' <summary>
    ''' Restituisce una nuova lista con gli stessi elementi della lista specificata, nell'ordine indicato.
    ''' </summary>
    ''' <param name="listLottiMagazzino"></param>
    ''' <param name="campoOrdinamento1"></param>
    ''' <param name="versoOrdinamento1"></param>
    ''' <param name="campoOrdinamento2"></param>
    ''' <param name="versoOrdinamento2"></param>
    ''' <param name="campoOrdinamento3"></param>
    ''' <param name="versoOrdinamento3"></param>
    ''' <param name="campiOrdineInverso">Vanno specificati i campi per i quali il verso deve essere invertito</param>
    Public Function OrdinaListaLottiMagazzino(listLottiMagazzino As List(Of Entities.LottoMagazzino), campoOrdinamento1 As String, versoOrdinamento1 As String,
                                              campoOrdinamento2 As String, versoOrdinamento2 As String, campoOrdinamento3 As String, versoOrdinamento3 As String,
                                              campiOrdineInverso As String()) As List(Of Entities.LottoMagazzino)

        Dim invertiOrdinamento As Boolean = False

        ' Controllo se le colonne per cui si ordina fanno parte di quelle per cui invertire l'ordinamento (asc/desc)
        ' Allo stato attuale, serve per i flag "attivo" e "presenza nel consultorio" (che hanno una label nel datagrid)

        Dim datiOrdinamento1 As Entities.DatiOrdinamento = CreateDatiOrdinamento(campoOrdinamento1, versoOrdinamento1, campiOrdineInverso)

        If String.IsNullOrEmpty(campoOrdinamento2) Then
            campoOrdinamento2 = campoOrdinamento1
            versoOrdinamento2 = versoOrdinamento1
        End If

        Dim datiOrdinamento2 As Entities.DatiOrdinamento = CreateDatiOrdinamento(campoOrdinamento2, versoOrdinamento2, campiOrdineInverso)

        If String.IsNullOrEmpty(campoOrdinamento3) Then
            campoOrdinamento3 = campoOrdinamento2
            versoOrdinamento3 = versoOrdinamento2
        End If

        Dim datiOrdinamento3 As Entities.DatiOrdinamento = CreateDatiOrdinamento(campoOrdinamento3, versoOrdinamento3, campiOrdineInverso)

        Dim listDatiOrdinamento As New List(Of Entities.DatiOrdinamento)
        listDatiOrdinamento.Add(datiOrdinamento1)
        listDatiOrdinamento.Add(datiOrdinamento2)
        listDatiOrdinamento.Add(datiOrdinamento3)

        Return Me.CreateOrderedList(Of Entities.LottoMagazzino)(listLottiMagazzino, listDatiOrdinamento)

    End Function

    ''' <summary>
    ''' Controllo se la colonna per cui si ordina fa parte di quelle per cui invertire l'ordinamento (asc/desc)
    ''' </summary>
    ''' <param name="campoOrdinamento"></param>
    ''' <param name="campiOrdineInverso"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function CheckInversioneOrdinamentoCampo(campoOrdinamento As String, campiOrdineInverso As String()) As Boolean

        If Not campiOrdineInverso Is Nothing AndAlso campiOrdineInverso.Count > 0 Then

            Return campiOrdineInverso.Contains(campoOrdinamento)

        End If

        Return False

    End Function

    ''' <summary>
    ''' Restituisce una lista di elementi ordinati in base ai campi specificati.
    ''' Nella lista dei dati per cui ordinare, il primo campo è quello che verrà applicato come criterio 
    ''' di ordinamento principale, il secondo campo verrà applicato come ordinamento a parità del primo, e così via.
    ''' Se non sono specificati campi per cui ordinare, la lista di elementi viene restituita inalterata.
    ''' </summary>
    ''' <param name="listLotti"></param>
    ''' <param name="listDatiOrdinamento"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreateOrderedList(Of T)(listLotti As List(Of T), listDatiOrdinamento As List(Of Entities.DatiOrdinamento)) As List(Of T)

        ' Se i campi per cui ordinare non sono stati specificati, resituisce la lista così come è stata passata
        If listDatiOrdinamento Is Nothing OrElse listDatiOrdinamento.Count = 0 Then
            Return listLotti
        End If

        Dim tempList As IOrderedEnumerable(Of T)

        ' Primo campo e verso ordinamento
        If listDatiOrdinamento(0).Verso = Enumerators.VersoOrdinamento.ASC Then
            tempList = listLotti.OrderBy(Function(f) GetPropertyValue(f, listDatiOrdinamento(0).Campo))
        Else
            tempList = listLotti.OrderByDescending(Function(f) GetPropertyValue(f, listDatiOrdinamento(0).Campo))
        End If

        ' Campi ordinamento successivi al primo (se presenti)
        If listDatiOrdinamento.Count > 1 Then

            For i As Integer = 1 To listDatiOrdinamento.Count - 1

                tempList = Me.AddOrderFieldToList(Of T)(tempList, listDatiOrdinamento(i))

            Next

        End If

        Return tempList.ToList()

    End Function

    ''' <summary>
    ''' Aggiunge un criterio di ordinamento (successivo al primo) alla lista specificata
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="orderedList"></param>
    ''' <param name="datiOrdinamento"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function AddOrderFieldToList(Of T)(orderedList As IOrderedEnumerable(Of T), datiOrdinamento As Entities.DatiOrdinamento) As IOrderedEnumerable(Of T)

        If datiOrdinamento Is Nothing OrElse String.IsNullOrEmpty(datiOrdinamento.Campo) Then Return orderedList

        If datiOrdinamento.Verso = Enumerators.VersoOrdinamento.DESC Then

            Return orderedList.ThenByDescending(Function(f) Me.GetPropertyValue(f, datiOrdinamento.Campo))

        End If

        Return orderedList.ThenBy(Function(f) Me.GetPropertyValue(f, datiOrdinamento.Campo))

    End Function

    ''' <summary>
    ''' Restituisce l'oggetto che rappresenta la proprietà specificata attraverso la stringa contenente il nome.
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <param name="proprieta"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetPropertyValue(obj As Object, proprieta As String) As Object

        Dim propertyInfo As System.Reflection.PropertyInfo = obj.GetType().GetProperty(proprieta)

        Return propertyInfo.GetValue(obj, Nothing)

    End Function

#End Region

    ''' <summary>
    ''' Restituisce un messaggio in base al numero di dosi e alla quantita minima del lotto per il consultorio specificato.
    ''' </summary>
    ''' <param name="codiceLotto"></param>
    ''' <param name="codiceConsultorioMagazzino"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ControlloDosiLottoDaEseguire(codiceLotto As String, codiceConsultorioMagazzino As String) As BizLottiResult

        If String.IsNullOrEmpty(codiceLotto) Then Return New BizLottiResult(BizLottiResult.ResultType.Success, String.Empty)

        Dim resultType As BizLottiResult.ResultType = BizLottiResult.ResultType.Success

        Dim warningLotto As String = String.Empty

        ' Lettura valore dosiRimaste prima dello scarico (che deve essere ancora eseguito), quindi sottraggo 1 dose al valore letto da db
        Dim dosiRimaste As Integer = Me.GenericProvider.Lotti.GetDosiRimaste(codiceLotto, codiceConsultorioMagazzino)

        dosiRimaste -= 1

        If dosiRimaste < 0 Then

            resultType = BizLottiResult.ResultType.GenericError
            warningLotto = String.Format("Lotto {0}: dosi insufficienti. Il salvataggio non verrà effettuato.", codiceLotto)

        ElseIf dosiRimaste = 0 Then

            resultType = BizLottiResult.ResultType.LottoDisattivatoScortaNullaWarning
            warningLotto = String.Format("Lotto {0}: dosi esaurite. Al salvataggio, il lotto verrà disattivato.", codiceLotto)

        Else

            resultType = BizLottiResult.ResultType.Success

            Dim qtaMinima As Integer = Me.GenericProvider.Lotti.GetQuantitaMinima(codiceLotto, codiceConsultorioMagazzino)

            If dosiRimaste < qtaMinima Then

                If dosiRimaste = 1 Then
                    warningLotto = String.Format("Lotto {0}: 1 dose rimasta.", codiceLotto)
                Else
                    warningLotto = String.Format("Lotto {0}: {1} dosi rimaste.", codiceLotto, dosiRimaste.ToString())
                End If

            End If

        End If

        Return New BizLottiResult(resultType, warningLotto)

    End Function

    ''' <summary>
    ''' Restituisce un messaggio di errore se almeno un lotto tra quelli specificati ha esaurito le dosi. Altrimenti restituisce Success, senza nessun messaggio.
    ''' </summary>
    ''' <param name="codiciLotti"></param>
    ''' <param name="codiceConsultorioMagazzino"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckPresenzaDosi(codiciLotti As List(Of String), codiceConsultorioMagazzino As String) As BizLottiResult

        Dim resultType As BizLottiResult.ResultType = BizLottiResult.ResultType.Success
        Dim resultMessage As String = String.Empty

        If Not codiciLotti Is Nothing AndAlso codiciLotti.Count > 0 Then

            Dim listDosi As List(Of KeyValuePair(Of String, Integer)) = Me.GenericProvider.Lotti.GetDosiRimaste(codiciLotti, codiceConsultorioMagazzino)

            If Not listDosi Is Nothing AndAlso listDosi.Count > 0 Then

                Dim codici As List(Of String) = listDosi.Where(Function(p) p.Value = 0).Select(Function(p) p.Key).ToList()

                If Not codici Is Nothing AndAlso codici.Count > 0 Then

                    resultType = BizLottiResult.ResultType.GenericError

                    If codici.Count = 1 Then
                        resultMessage = String.Format("Lotto {0}: dosi insufficienti. Il salvataggio non verrà effettuato.", codici.First())
                    Else
                        resultMessage = String.Format("Il salvataggio non verrà effettuato. Dosi insufficienti per i lotti: {0}.", String.Join(", ", codici.ToArray()))
                    End If

                End If

            End If

        End If

        Return New BizLottiResult(resultType, resultMessage)

    End Function

    ''' <summary>
    ''' Restituisce una stringa contenente codice del lotto, codice dell'associazione e descrizione dell'associazione, separati da "|", 
    ''' per ogni codice del lotto specificato nella lista.
    ''' </summary>
    ''' <param name="listCodiciLotti"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetStringLottiAssociazioni(listCodiciLotti As List(Of String)) As String

        Dim listLottiAssociazioni As List(Of ILottiProvider.LottiAssociazioniResult) =
            Me.GenericProvider.Lotti.GetLottiAssociazioni(listCodiciLotti)

        If listCodiciLotti Is Nothing OrElse listCodiciLotti.Count = 0 Then Return String.Empty

        Dim stbLottiAssociazioni As New System.Text.StringBuilder()

        For Each lottoAssociazione As ILottiProvider.LottiAssociazioniResult In listLottiAssociazioni

            stbLottiAssociazioni.AppendFormat("{0}|{1}|{2}|",
                                              lottoAssociazione.CodiceLotto,
                                              lottoAssociazione.CodiceAssociazione,
                                              lottoAssociazione.DescrizioneAssociazione)

        Next

        Return stbLottiAssociazioni.ToString()

    End Function

    Public Function GetEtaAttivazioneLotto(codiceLotto As String, codiceConsultorioLotto As String) As BizLotti.EtaAttivazioneLottoResult

        Dim etaAttivazione As BizLotti.EtaAttivazioneLottoResult = Nothing

        Dim result As ILottiProvider.EtaAttivazioneLottoResult =
            Me.GenericProvider.Lotti.GetEtaAttivazioneLotto(codiceLotto, codiceConsultorioLotto)

        If result Is Nothing Then Return Nothing

        etaAttivazione = New EtaAttivazioneLottoResult()
        etaAttivazione.EtaMinima = result.EtaMinima
        etaAttivazione.EtaMassima = result.EtaMassima

        Return etaAttivazione

    End Function

    ''' <summary>
    ''' Restituisce l'anagrafica del lotto specificato
    ''' </summary>
    ''' <param name="codiceLotto"></param>
    ''' <returns></returns>
    Public Function GetLottoAnagrafe(codiceLotto As String) As LottoAnagrafe

        If String.IsNullOrWhiteSpace(codiceLotto) Then Return New LottoAnagrafe()

        Return GenericProvider.Lotti.GetLottoAnagrafe(codiceLotto)

    End Function

    ''' <summary>
    ''' Restituisce il codice del ciclo associato al nome commerciale relativo al lotto specificato, se presente e se il ciclo non è obsoleto
    ''' </summary>
    ''' <param name="codiceLotto"></param>
    ''' <returns></returns>
    Public Function GetCicloNomeCommercialeByLotto(codiceLotto As String) As String

        If String.IsNullOrWhiteSpace(codiceLotto) Then Return String.Empty

        Return GenericProvider.Lotti.GetCicloNomeCommercialeByLotto(codiceLotto)

    End Function

#End Region

#Region " Movimenti "

    ''' <summary>
    ''' Caricamento paginato dei movimenti del lotto nel consultorio specificato
    ''' </summary>
    ''' <param name="codiceLotto"></param>
    ''' <param name="codiceConsultorioMagazzino"></param>
    ''' <param name="pagingOptions"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LoadMovimentiLotto(codiceLotto As String, codiceConsultorioMagazzino As String, pagingOptions As Onit.OnAssistnet.Data.PagingOptions) As List(Of Entities.MovimentoLotto)

        Return Me.GenericProvider.Lotti.LoadMovimentiLotto(codiceLotto, codiceConsultorioMagazzino, pagingOptions.StartRecordIndex, pagingOptions.EndRecordIndex)

    End Function

    ''' <summary>
    ''' Conteggio movimenti del lotto nel consultorio specificato
    ''' </summary>
    ''' <param name="codiceLotto"></param>
    ''' <param name="codiceConsultorioMagazzino"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountMovimentiLotto(codiceLotto As String, codiceConsultorioMagazzino As String) As Integer

        Return Me.GenericProvider.Lotti.CountMovimentiLotto(codiceLotto, codiceConsultorioMagazzino)

    End Function

#End Region

#Region " Salvataggi su db "

#Region " Attivazione/Disattivazione Lotto "

    ''' <summary>
    ''' Disattiva il lotto nel consultorio specificato.
    ''' </summary>
    ''' <param name="codiceLotto"></param>
    ''' <param name="codiceConsultorio"></param>
    ''' <param name="listTestateLog"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DisattivaLotto(codiceLotto As String, codiceConsultorio As String, listTestateLog As List(Of DataLogStructure.Testata)) As Boolean

        Dim ownTransaction As Boolean = False

        Try

            If Me.GenericProvider.Transaction Is Nothing Then
                Me.GenericProvider.BeginTransaction()
                ownTransaction = True
            End If

            Dim now As DateTime = DateTime.Now

            ' Disattivazione lotto nel consultorio
            Me.GenericProvider.Lotti.UpdateLottoAttivo(codiceLotto, codiceConsultorio, False, Me.ContextInfos.IDUtente, now, Nothing, Nothing, False)

            ' Log disattivazione lotto
            listTestateLog.Add(Me.CreaTestataLogUpdateLottoAttivo(codiceLotto, codiceConsultorio, False, False, Nothing, Nothing, Nothing, False))

            If ownTransaction Then
                Me.GenericProvider.Commit()
            End If

        Catch ex As Exception

            If ownTransaction Then
                Me.GenericProvider.Rollback()
            End If

            ex.InternalPreserveStackTrace()
            Throw

        End Try

        Return True

    End Function

    ' TODO: per adesso lo lascio => controllare se in RicercaLotti può essere utilizzato il metodo successivo
    Public Function AttivaLotto(codiceLotto As String, codiceConsultorio As String, quantitaMinima As Integer, listTestateLog As List(Of DataLogStructure.Testata)) As Boolean
        Return Me.AttivaLotto(codiceLotto, codiceConsultorio, quantitaMinima, Nothing, Nothing, listTestateLog)
    End Function

    ''' <summary>
    ''' Attiva il lotto nel consultorio specificato.
    ''' Se il lotto non è presente nel consultorio, viene inserito a scorta nulla.
    ''' Vengono salvate anche le età (minima e massima) di attivazione per il lotto in consultorio, se specificate e se il parametro ASSOCIA_LOTTI_ETA è true.
    ''' Valorizza la lista di testate da scrivere.
    ''' </summary>
    ''' <param name="codiceLotto"></param>
    ''' <param name="codiceConsultorio"></param>
    ''' <param name="quantitaMinima"></param>
    ''' <param name="etaMinima"></param>
    ''' <param name="etaMassima"></param>
    ''' <param name="listTestateLog"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AttivaLotto(codiceLotto As String, codiceConsultorio As String, quantitaMinima As Integer, etaMinima As Integer?, etaMassima As Integer?, listTestateLog As List(Of DataLogStructure.Testata)) As Boolean
        '--
        Dim ownTransaction As Boolean = False
        '--
        Try
            '--
            If Me.GenericProvider.Transaction Is Nothing Then
                Me.GenericProvider.BeginTransaction()
                ownTransaction = True
            End If
            '--
            If Me.Settings.ASSOCIA_LOTTI_ETA AndAlso (etaMinima.HasValue Or etaMassima.HasValue) Then
                '--
                ' Modifica del flag di attivazione e delle età min e/o max nella lotti-consultori, per il consultorio corrente.
                ' Aggiunta testate da scrivere alla lista.
                Me.UpdateLottoAttivo(codiceLotto, codiceConsultorio, quantitaMinima, True, listTestateLog, Nothing, Me.ContextInfos.IDUtente, Date.Now, etaMinima, etaMassima, True)
                '--
            Else
                '--
                ' Modifica del flag di attivazione nella lotti-consultori, per il consultorio corrente.
                ' Aggiunta testate da scrivere alla lista.
                Me.UpdateLottoAttivo(codiceLotto, codiceConsultorio, quantitaMinima, True, listTestateLog, Nothing, Me.ContextInfos.IDUtente, Date.Now, Nothing, Nothing, False)
                '--
            End If
            '--
            If ownTransaction Then
                Me.GenericProvider.Commit()
            End If
            '--
        Catch ex As Exception
            '--
            If ownTransaction Then
                Me.GenericProvider.Rollback()
            End If
            '--
            ex.InternalPreserveStackTrace()
            Throw
            '--
        End Try
        '--
        Return True
        '--
    End Function


    ' TODO: per adesso lo lascio => controllare se, dove viene usato, può essere sostituito sempre con quello sotto
    'Private Function UpdateLottoAttivo(codiceLotto As String, codiceConsultorio As String, quantitaMinima As String, attiva As Boolean,
    '                                   listTestateLog As List(Of DataLogStructure.Testata), lottoMagazzinoOriginale As Entities.LottoMagazzino,
    '                                   idUtenteModificaFlagAttivo As Integer?, dataModificaFlagAttivo As DateTime?) As Boolean
    '    '--
    '    Return Me.UpdateLottoAttivo(codiceLotto, codiceConsultorio, quantitaMinima, attiva, listTestateLog,
    '                                lottoMagazzinoOriginale, idUtenteModificaFlagAttivo, dataModificaFlagAttivo, Nothing, Nothing, False)
    '    '--
    'End Function


    ''' <summary>
    ''' Update del flag di attivazione del lotto in consultorio. Eventuale inserimento del lotto in consultorio se non presente.
    ''' Update delle età di attivazione minima e massima se specificate.
    ''' Log delle operazioni effettuate.
    ''' La transazione viene gestita dal chiamante!
    ''' </summary>
    ''' <param name="codiceLotto"></param>
    ''' <param name="codiceConsultorio"></param>
    ''' <param name="quantitaMinima"></param>
    ''' <param name="attiva"></param>
    ''' <param name="listTestateLog"></param>
    ''' <param name="lottoMagazzinoOriginale"></param>
    ''' <param name="idUtenteModificaFlagAttivo"></param>
    ''' <param name="dataModificaFlagAttivo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdateLottoAttivo(codiceLotto As String, codiceConsultorio As String, quantitaMinima As String, attiva As Boolean,
                                       listTestateLog As List(Of DataLogStructure.Testata), lottoMagazzinoOriginale As Entities.LottoMagazzino,
                                       idUtenteModificaFlagAttivo As Integer?, dataModificaFlagAttivo As DateTime?,
                                       etaMinimaAttivazione As Integer?, etaMassimaAttivazione As Integer?, updateEta As Boolean) As Boolean
        '--
        ' Update dati di attivazione (flag attivazione e, se specificato, le date min e max di attivazione)
        Dim countUpdate As Integer = Me.GenericProvider.Lotti.UpdateLottoAttivo(codiceLotto, codiceConsultorio, attiva,
                                                                                idUtenteModificaFlagAttivo, dataModificaFlagAttivo,
                                                                                etaMinimaAttivazione, etaMassimaAttivazione, updateEta)
        If countUpdate > 0 Then
            '--
            ' Se è stato effettuato l'update nella lotti-consultori, 
            ' devo effettuare il log dell'attivazione/disattivazione del lotto in consultorio
            '--
            If lottoMagazzinoOriginale Is Nothing Then
                listTestateLog.Add(Me.CreaTestataLogUpdateLottoAttivo(codiceLotto, codiceConsultorio, attiva, False, Nothing, etaMinimaAttivazione, etaMassimaAttivazione, updateEta))
            Else
                ' Se ho i dati originali del lotto, li salvo nelle righe di log
                listTestateLog.Add(Me.CreaTestataLogUpdateLottoAttivo(lottoMagazzinoOriginale, codiceLotto, codiceConsultorio, attiva, Nothing, etaMinimaAttivazione, etaMassimaAttivazione))
            End If
            '--
        Else
            '--
            ' Se non è stato effettuato nessun update nella lotti-consultori,
            ' deve essere inserito un record relativo al consultorio, con scorta nulla e quantità minima specificata.
            '--
            Dim lottoConsultorio As New Entities.LottoMagazzino()
            lottoConsultorio.CodiceLotto = codiceLotto
            lottoConsultorio.CodiceConsultorio = codiceConsultorio
            lottoConsultorio.Attivo = attiva
            lottoConsultorio.DosiRimaste = 0
            lottoConsultorio.QuantitaMinima = quantitaMinima
            lottoConsultorio.IdUtenteModificaFlagAttivo = idUtenteModificaFlagAttivo
            lottoConsultorio.DataModificaFlagAttivo = dataModificaFlagAttivo

            If updateEta Then
                lottoConsultorio.EtaMinimaAttivazione = etaMinimaAttivazione
                lottoConsultorio.EtaMassimaAttivazione = etaMassimaAttivazione
            End If

            Me.GenericProvider.Lotti.InsertLottoConsultorio(lottoConsultorio)

            ' Log inserimento lotto-consultorio
            listTestateLog.Add(Me.CreaTestataLogInserimentoLottoConsultorio(lottoConsultorio, True,
                               "Inserito in automatico il lotto nel centro vaccinale, in seguito a tentativo di attivazione/disattivazione non riuscito."))
        End If

        Return True

    End Function

#End Region

#Region " Inserimento Lotto "

    ''' <summary>
    ''' Inserimento lotto per il magazzino centrale, con scrittura del log.
    ''' </summary>
    ''' <param name="lottoMagazzino"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function InsertLottoMagazzinoCentrale(lottoMagazzino As Entities.LottoMagazzino) As BizLottiResult

        Dim listTestateLog As New List(Of DataLogStructure.Testata)()

        ' Inserimento
        Dim bizLottiResult As BizLottiResult = Me.InsertLottoMagazzinoCentrale(lottoMagazzino, listTestateLog)

        ' Scrittura log
        If bizLottiResult.Result <> BizLotti.BizLottiResult.ResultType.GenericError Then

            For Each testata As DataLogStructure.Testata In listTestateLog
                LogBox.WriteData(testata)
            Next

        End If

        Return bizLottiResult

    End Function

    ''' <summary>
    ''' Inserimento lotto per il magazzino centrale.
    ''' </summary>
    ''' <param name="lottoMagazzino"></param>
    ''' <param name="listTestateLog"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function InsertLottoMagazzinoCentrale(lottoMagazzino As Entities.LottoMagazzino, listTestateLog As List(Of DataLogStructure.Testata)) As BizLottiResult

        Dim resultType As BizLottiResult.ResultType = BizLottiResult.ResultType.Success

        Dim warningMessage As New System.Text.StringBuilder()

        Dim now As DateTime = DateTime.Now

        ' Codice lotto senza spazi
        lottoMagazzino.CodiceLotto = lottoMagazzino.CodiceLotto.Trim()

        ' Se descrizione lotto vuota, inserisco descrizione nome commerciale
        If String.IsNullOrEmpty(lottoMagazzino.DescrizioneLotto) Then
            lottoMagazzino.DescrizioneLotto = lottoMagazzino.DescrizioneNomeCommerciale
        End If

        ' Valorizzazione dosi scatola
        If lottoMagazzino.DosiScatola <= 0 Then lottoMagazzino.DosiScatola = 1

        ' Dati attivazione lotto (non gestiti)
        lottoMagazzino.IdUtenteModificaFlagAttivo = Nothing
        lottoMagazzino.DataModificaFlagAttivo = Nothing

        ' Lotto attivo in inserimento sempre a false perchè il magazzino centrale non lo gestisce
        lottoMagazzino.Attivo = False

        ' Se il lotto viene reso Obsoleto, loggo l'utente e la data di obsolescenza
        If lottoMagazzino.Obsoleto Then

            lottoMagazzino.IdUtenteModificaFlagObsoleto = Me.ContextInfos.IDUtente
            lottoMagazzino.DataModificaFlagObsoleto = now

        Else

            lottoMagazzino.IdUtenteModificaFlagObsoleto = Nothing
            lottoMagazzino.DataModificaFlagObsoleto = Nothing

        End If

        Using lock As New Onit.Shared.Manager.Lock.Lock(GetCodiceAziendaToLockLotti(Me.ContextInfos.IDApplicazione),
                                                        GetCodiceRisorsaToLockLotti(lottoMagazzino.CodiceLotto, lottoMagazzino.CodiceConsultorio))

            ' Controllo codice lotto: se esiste già, non va effettuato l'insert
            If Me.GenericProvider.Lotti.ExistsCodiceLotto(lottoMagazzino.CodiceLotto) Then

                Return New BizLottiResult(BizLottiResult.ResultType.GenericError, "Inserimento non effettuato: il codice specificato per il lotto è già utilizzato.")

            End If

            Dim ownTransaction As Boolean = False

            Try
                If Me.GenericProvider.Transaction Is Nothing Then
                    Me.GenericProvider.BeginTransaction()
                    ownTransaction = True
                End If

                ' Inserimento in anagrafica lotti
                Me.GenericProvider.Lotti.InsertLottoMagazzinoCentrale(lottoMagazzino)

                ' Log inserimento in anagrafica
                listTestateLog.Add(Me.CreaTestataLogInserimentoLotto(lottoMagazzino, True))

                If ownTransaction Then
                    Me.GenericProvider.Commit()
                End If

            Catch ex As Exception

                If ownTransaction Then
                    Me.GenericProvider.Rollback()
                End If

                ex.InternalPreserveStackTrace()
                Throw

            End Try

        End Using

        Return New BizLottiResult(resultType, warningMessage.ToString())

    End Function

    ''' <summary>
    ''' Inserimento lotto in anagrafica (se isNewLotto = true), creazione associazione lotto-consultorio, 
    ''' creazione movimento di carico (se quantità iniziale positiva).
    ''' Se il lotto deve essere attivato, viene controllata la possibilità di attivazione: 
    ''' se è già attivo un lotto relativo allo stesso nome commerciale, l'inserimento avviene 
    ''' ma con flag di attivazione a false. In questo caso restituisce un risultato di tipo Warning.
    ''' Se insertEtaAttivazione è true, vengono inseriti anche i valori di età min e max di attivazione del lotto nel consultorio.
    ''' Scrive il log delle operazioni effettuate (tranne nel caso in cui il risultato sia GenericError).
    ''' </summary>
    ''' <param name="lottoMagazzino"></param>
    ''' <param name="isNewLotto"></param>
    ''' <param name="codiceConsultorioCorrente"></param>
    ''' <param name="codiceConsultorioMagazzino"></param>
    ''' <param name="quantitaIniziale"></param>
    ''' <param name="usaDosiScatola"></param>
    ''' <param name="disattivaLotto"></param>
    ''' <param name="insertEtaAttivazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function InsertLottoMagazzino(lottoMagazzino As Entities.LottoMagazzino, isNewLotto As Boolean, codiceConsultorioCorrente As String, codiceConsultorioMagazzino As String, quantitaIniziale As Integer, usaDosiScatola As Boolean, disattivaLotto As Boolean, insertEtaAttivazione As Boolean) As BizLottiResult

        Dim listTestateLog As New List(Of DataLogStructure.Testata)()

        ' Inserimento
        Dim bizLottiResult As BizLottiResult = Me.InsertLottoMagazzino(lottoMagazzino, isNewLotto, codiceConsultorioCorrente, codiceConsultorioMagazzino,
                                                                       quantitaIniziale, usaDosiScatola, disattivaLotto, insertEtaAttivazione, listTestateLog)

        ' Scrittura log
        If bizLottiResult.Result <> BizLotti.BizLottiResult.ResultType.GenericError Then

            For Each testata As DataLogStructure.Testata In listTestateLog
                LogBox.WriteData(testata)
            Next

        End If

        Return bizLottiResult

    End Function

    ''' <summary>
    ''' Inserimento lotto in anagrafica (se isNewLotto = true), creazione associazione lotto-consultorio, 
    ''' creazione movimento di carico (se quantità iniziale positiva).
    ''' Se il lotto deve essere attivato, viene controllata la possibilità di attivazione: 
    ''' se è già attivo un lotto relativo allo stesso nome commerciale, l'inserimento avviene 
    ''' ma con flag di attivazione a false. In questo caso restituisce un risultato di tipo Warning.
    ''' Se insertEtaAttivazione è true, vengono inseriti anche i valori di età min e max di attivazione del lotto nel consultorio.
    ''' Aggiunge alla listTestateLog le testate da scrivere.
    ''' </summary>
    ''' <param name="lottoMagazzino"></param>
    ''' <param name="isNewLotto"></param>
    ''' <param name="codiceConsultorioMagazzino"></param>
    ''' <param name="codiceConsultorioCorrente"></param>
    ''' <param name="quantitaIniziale"></param>
    ''' <param name="usaDosiScatola"></param>
    ''' <param name="disattivaLotto"></param>
    ''' <param name="insertEtaAttivazione"></param>
    ''' <param name="listTestateLog"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function InsertLottoMagazzino(lottoMagazzino As Entities.LottoMagazzino, isNewLotto As Boolean, codiceConsultorioCorrente As String, codiceConsultorioMagazzino As String, quantitaIniziale As Integer, usaDosiScatola As Boolean, disattivaLotto As Boolean, insertEtaAttivazione As Boolean, listTestateLog As List(Of DataLogStructure.Testata)) As BizLottiResult

        Dim resultType As BizLottiResult.ResultType = BizLottiResult.ResultType.Success

        Dim warningMessage As New System.Text.StringBuilder()

        Dim now As DateTime = DateTime.Now

        ' Codice lotto: maiuscolo e senza spazi
        lottoMagazzino.CodiceLotto = lottoMagazzino.CodiceLotto.Trim().ToUpper()

        If String.IsNullOrWhiteSpace(lottoMagazzino.CodiceLotto) Then

            Return New BizLottiResult(BizLottiResult.ResultType.GenericError,
                                      "Inserimento non effettuato: codice lotto non specificato.")

        End If

        ' Codice consultorio = codice magazzino
        lottoMagazzino.CodiceConsultorio = codiceConsultorioMagazzino

        ' Se descrizione lotto vuota, inserisco descrizione nome commerciale
        If String.IsNullOrEmpty(lottoMagazzino.DescrizioneLotto) Then
            lottoMagazzino.DescrizioneLotto = lottoMagazzino.DescrizioneNomeCommerciale
        End If

        ' Valorizzazione dosi scatola
        If lottoMagazzino.DosiScatola <= 0 Then lottoMagazzino.DosiScatola = 1

        ' Calcolo dosi rimaste: in inserimento è pari alla quantità iniziale specificata.
        If usaDosiScatola Then
            lottoMagazzino.DosiRimaste = quantitaIniziale * lottoMagazzino.DosiScatola
        Else
            lottoMagazzino.DosiRimaste = quantitaIniziale
        End If

        ' Se il valore delle dosi iniziali è maggiore di zero, deve essere inserito un carico
        Dim movimentoLotto As Entities.MovimentoLotto = Nothing

        If lottoMagazzino.DosiRimaste > 0 Then

            movimentoLotto = New Entities.MovimentoLotto()
            movimentoLotto.Progressivo = Me.GetProgressivo()
            movimentoLotto.DataRegistrazione = now
            movimentoLotto.CodiceLotto = lottoMagazzino.CodiceLotto
            movimentoLotto.CodiceConsultorio = codiceConsultorioMagazzino
            movimentoLotto.IdUtente = Me.ContextInfos.IDUtente
            movimentoLotto.NumeroDosi = lottoMagazzino.DosiRimaste
            movimentoLotto.TipoMovimento = Constants.TipoMovimentoMagazzino.Carico
            movimentoLotto.CodiceConsultorioTrasferimento = String.Empty
            movimentoLotto.Note = String.Empty
            movimentoLotto.IdEsecuzioneAssociazione = String.Empty

        End If

        ' Controllo codice lotto: se esiste già, non va effettuato l'insert in anagrafica
        If isNewLotto Then

            If Me.GenericProvider.Lotti.ExistsCodiceLotto(lottoMagazzino.CodiceLotto) Then

                Return New BizLottiResult(BizLottiResult.ResultType.GenericError,
                                          "Inserimento non effettuato: il codice specificato per il lotto è già utilizzato.")

            End If

        End If

        ' Controllo attivazione lotto
        lottoMagazzino.IdUtenteModificaFlagAttivo = Nothing
        lottoMagazzino.DataModificaFlagAttivo = Nothing

        If lottoMagazzino.Attivo Then

            Dim resultLottoAttivo As BizLottiResult =
                Me.IsLottoAttivabile(True, lottoMagazzino.CodiceLotto, lottoMagazzino.CodiceNomeCommerciale, String.Empty, codiceConsultorioCorrente, codiceConsultorioMagazzino)

            If resultLottoAttivo.Result <> BizLottiResult.ResultType.Success Then

                lottoMagazzino.Attivo = False

                ' Concateno il messaggio valorizzato dal metodo di controllo.
                warningMessage.AppendFormat(resultLottoAttivo.Message)

                resultType = BizLottiResult.ResultType.IsActiveLottoWarning

            Else

                ' Se viene attivato il lotto, loggo l'utente e la data di attivazione
                lottoMagazzino.IdUtenteModificaFlagAttivo = Me.ContextInfos.IDUtente
                lottoMagazzino.DataModificaFlagAttivo = now

            End If

        End If

        ' Se il lotto viene reso Obsoleto, loggo l'utente e la data di obsolescenza
        If lottoMagazzino.Obsoleto Then
            lottoMagazzino.IdUtenteModificaFlagObsoleto = Me.ContextInfos.IDUtente
            lottoMagazzino.DataModificaFlagObsoleto = now
        Else
            lottoMagazzino.IdUtenteModificaFlagObsoleto = Nothing
            lottoMagazzino.DataModificaFlagObsoleto = Nothing
        End If

        Dim ownTransaction As Boolean = False

        Try
            If Me.GenericProvider.Transaction Is Nothing Then
                Me.GenericProvider.BeginTransaction()
                ownTransaction = True
            End If

            If isNewLotto Then

                ' Lotto nuovo => inserimento in anagrafica lotti
                Me.GenericProvider.Lotti.InsertLotto(lottoMagazzino)

                ' Log inserimento in anagrafica
                listTestateLog.Add(Me.CreaTestataLogInserimentoLotto(lottoMagazzino, False))

            End If

            ' Controllo se consultorio ha se stesso come magazzino o un altro consultorio.
            If codiceConsultorioCorrente <> codiceConsultorioMagazzino Then

                ' Nel caso di cns <> mag, vanno effettuati due inserimenti:
                ' 1 - inserire un record lotto-consultorio relativo al cns di magazzino, 
                '     con la quantità minima e le dosi specificate, con il flag attivo a false e le date di attivazione nulle
                ' 2 - inserire un record lotto-consultorio relativo al cns corrente, 
                '     con le quantità a 0 e con il flag attivo e le età di attivazione uguali a quelli del lotto specificato
                Me.InserimentoLottoConsultorioMagazzino(lottoMagazzino, codiceConsultorioCorrente, codiceConsultorioMagazzino, insertEtaAttivazione, listTestateLog)

            Else

                If Not isNewLotto Then

                    ' Se il lotto non è appena stato inserito in anagrafica, controllo se è già associato
                    ' al consultorio prima di aggiungere l'associazione lotto-consultorio.
                    ' Se è già associato termino con un messaggio di errore.
                    If Me.GenericProvider.Lotti.IsLottoInConsultorio(lottoMagazzino.CodiceLotto, lottoMagazzino.CodiceConsultorio) Then

                        Return New BizLottiResult(BizLottiResult.ResultType.GenericError,
                                                  "Inserimento non effettuato: il lotto è già presente nel consultorio specificato.")

                    End If

                End If

                ' Clono l'oggetto per svuotare i campi età (se serve) senza incidere sull'oggetto passato dall'esterno.
                Dim lottoMagazzinoCopy As Entities.LottoMagazzino = lottoMagazzino.Clone()

                If Not insertEtaAttivazione Then
                    lottoMagazzinoCopy.EtaMinimaAttivazione = Nothing
                    lottoMagazzinoCopy.EtaMassimaAttivazione = Nothing
                End If

                ' Inserimento associazione lotto-consultorio
                Me.GenericProvider.Lotti.InsertLottoConsultorio(lottoMagazzinoCopy)

                ' Log inserimento lotto-consultorio
                listTestateLog.Add(Me.CreaTestataLogInserimentoLottoConsultorio(lottoMagazzinoCopy, True, Nothing))

            End If

            ' Inserimento carico se quantità iniziale positiva
            If Not movimentoLotto Is Nothing Then

                ' Inserimento movimento di carico
                Me.GenericProvider.Lotti.InsertMovimento(movimentoLotto)

                ' Log inserimento movimento
                listTestateLog.Add(Me.CreaTestataLogInserimentoMovimento(movimentoLotto, True, Nothing))

            End If

            ' Disattivazione del lotto su tutti i consultori
            If disattivaLotto Then

                ' Disattivazione su tutti i consultori
                Me.GenericProvider.Lotti.DisattivaLotto(lottoMagazzino.CodiceLotto, Me.ContextInfos.IDUtente, now)

                ' Log disattivazione
                listTestateLog.Add(Me.CreaTestataLogDisattivazioneLotto(lottoMagazzino, "Disattivazione del lotto su tutti i centri vaccinali, in seguito ad inserimento"))

            End If

            If ownTransaction Then
                Me.GenericProvider.Commit()
            End If

        Catch ex As Exception

            If ownTransaction Then
                Me.GenericProvider.Rollback()
            End If

            ex.InternalPreserveStackTrace()
            Throw

        End Try

        Return New BizLottiResult(resultType, warningMessage.ToString())

    End Function

    Private Sub InserimentoLottoConsultorioMagazzino(lottoMagazzino As Entities.LottoMagazzino, codiceConsultorioCorrente As String, codiceConsultorioMagazzino As String, insertEtaAttivazioneConsultorioCorrente As Boolean, listTestateLog As List(Of Log.DataLogStructure.Testata))

        '-----
        ' Inserimento nel consultorio di magazzino 
        '-----
        ' Flag Attivo e Date di Attivazione sempre a false
        Dim lottoConsultorioMagazzino As Entities.LottoMagazzino = lottoMagazzino.Clone()

        lottoConsultorioMagazzino.CodiceConsultorio = codiceConsultorioMagazzino
        lottoConsultorioMagazzino.Attivo = False
        lottoConsultorioMagazzino.EtaMinimaAttivazione = Nothing
        lottoConsultorioMagazzino.EtaMassimaAttivazione = Nothing

        ' Inserimento lotto-consultorio magazzino
        Me.GenericProvider.Lotti.InsertLottoConsultorio(lottoConsultorioMagazzino)

        ' Log inserimento lotto-consultorio magazzino
        listTestateLog.Add(Me.CreaTestataLogInserimentoLottoConsultorio(lottoConsultorioMagazzino, True, String.Format("Inserimento automatico per inserimento lotto dal centro {0}", codiceConsultorioCorrente)))

        '-----
        ' Inserimento nel consultorio corrente 
        '-----
        ' Flag Attivo uguale a quello del lotto da inserire, 
        ' Date Attivazione nulle se flag insertEtaAttivazioneConsultorioCorrente = false, altrimenti i valori delle date sono quelli del lotto da inserire
        Dim lottoConsultorioCorrente As Entities.LottoMagazzino = lottoMagazzino.Clone()

        lottoConsultorioCorrente.CodiceConsultorio = codiceConsultorioCorrente
        lottoConsultorioCorrente.DosiRimaste = 0
        lottoConsultorioCorrente.QuantitaMinima = 0

        If Not insertEtaAttivazioneConsultorioCorrente Then
            lottoConsultorioCorrente.EtaMinimaAttivazione = Nothing
            lottoConsultorioCorrente.EtaMassimaAttivazione = Nothing
        End If

        ' Inserimento lotto-consultorio corrente
        Me.GenericProvider.Lotti.InsertLottoConsultorio(lottoConsultorioCorrente)

        ' Log inserimento lotto-consultorio corrente
        listTestateLog.Add(Me.CreaTestataLogInserimentoLottoConsultorio(lottoConsultorioCorrente, True, Nothing))

    End Sub

    ''' <summary>
    ''' Controllo dosi e quantità minima. 
    ''' Se il controllo restituisce esito positivo, inserisce l'associazione lotto-consultorio 
    ''' e il primo carico se la quantità iniziale > 0. In questo caso, restituisce un risultato positivo.
    ''' Se il controllo restituisce esito negativo, viene restituito un risultato negativo con il messaggio di errore generato.
    ''' </summary>
    ''' <param name="codiceLotto"></param>
    ''' <param name="descrizioneLotto"></param>
    ''' <param name="nomeCampoConsultorio"></param>
    ''' <param name="codiceConsultorio"></param>
    ''' <param name="nomeCampoNumeroDosi"></param>
    ''' <param name="valoreCampoNumeroDosi"></param>
    ''' <param name="nomeCampoQuantitaMinima"></param>
    ''' <param name="valoreCampoQuantitaMinima"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function InsertLottoConsultorioMagazzinoCentrale(codiceLotto As String, descrizioneLotto As String,
                                                            nomeCampoConsultorio As String, codiceConsultorio As String,
                                                            nomeCampoNumeroDosi As String, valoreCampoNumeroDosi As String,
                                                            nomeCampoQuantitaMinima As String, valoreCampoQuantitaMinima As String) As BizLottiResult

        Dim errorMessage As New System.Text.StringBuilder()

        ' Numero dosi
        valoreCampoNumeroDosi = valoreCampoNumeroDosi.Trim()

        If String.IsNullOrEmpty(valoreCampoNumeroDosi) Then

            errorMessage.AppendFormat("\n - il campo '{0}' deve essere specificato", nomeCampoNumeroDosi)

        Else

            ' Controllo campo dosi
            Dim controlloDosiResult As BizLottiResult = Me.ControlliCampo(nomeCampoNumeroDosi, valoreCampoNumeroDosi, LUNGHEZZA_CAMPO_NUMERO_DOSI)

            If controlloDosiResult.Result <> BizLottiResult.ResultType.Success Then
                errorMessage.Append(controlloDosiResult.Message)
            End If

        End If

        ' Quantita minima
        valoreCampoQuantitaMinima = valoreCampoQuantitaMinima.Trim()

        If Not String.IsNullOrEmpty(valoreCampoQuantitaMinima) Then

            ' Controllo quantità minima
            Dim controlloQtaMinResult As BizLottiResult = Me.ControlliCampo(nomeCampoQuantitaMinima, valoreCampoQuantitaMinima, LUNGHEZZA_CAMPO_QUANTITA_MINIMA)

            If controlloQtaMinResult.Result <> BizLottiResult.ResultType.Success Then
                errorMessage.Append(controlloQtaMinResult.Message)
            End If

        End If

        ' Consultorio
        If String.IsNullOrEmpty(codiceConsultorio) Then
            errorMessage.AppendFormat("\n - il campo '{0}' deve essere specificato", nomeCampoConsultorio)
        End If

        ' In caso di errori, termina e restituisce il messaggio
        If errorMessage.Length > 0 Then

            Return New Biz.BizLotti.BizLottiResult(Biz.BizLotti.BizLottiResult.ResultType.GenericError,
                                                   String.Format("Salvataggio non effettuato: {0}", errorMessage.ToString()))
        End If

        If String.IsNullOrEmpty(valoreCampoQuantitaMinima) Then

            valoreCampoQuantitaMinima = "0"

        End If

        ' Inserimento dati lotto in consultorio
        Dim lottoMagazzino As New Entities.LottoMagazzino()
        lottoMagazzino.CodiceLotto = codiceLotto
        lottoMagazzino.DescrizioneLotto = descrizioneLotto
        lottoMagazzino.DosiRimaste = Convert.ToInt32(valoreCampoNumeroDosi)
        lottoMagazzino.CodiceConsultorio = codiceConsultorio
        lottoMagazzino.QuantitaMinima = Convert.ToInt32(valoreCampoQuantitaMinima)
        lottoMagazzino.Attivo = False
        lottoMagazzino.Obsoleto = False

        ' Inserimento associazione lotto-consultorio e primo carico se quantità iniziale > 0, 
        ' scrittura del log (tranne che in caso di errore)
        ' Il lotto non viene inserito in anagrafica perchè c'è già.
        Return Me.InsertLottoMagazzino(lottoMagazzino, False, lottoMagazzino.CodiceConsultorio, lottoMagazzino.CodiceConsultorio,
                                       lottoMagazzino.DosiRimaste, False, False, False)

    End Function

#End Region

#Region " Modifica Lotto "

    ''' <summary>
    ''' Modifica Lotto per il magazzino centrale, con scrittura del log
    ''' </summary>
    ''' <param name="lottoMagazzino"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateLottoMagazzinoCentrale(lottoMagazzino As Entities.LottoMagazzino) As BizLottiResult

        Dim listTestateLog As New List(Of DataLogStructure.Testata)()

        ' Modifica
        Dim bizLottiResult As BizLottiResult = Me.UpdateLottoMagazzinoCentrale(lottoMagazzino, listTestateLog)

        ' Scrittura log
        If bizLottiResult.Result <> BizLotti.BizLottiResult.ResultType.GenericError Then

            For Each testata As DataLogStructure.Testata In listTestateLog
                LogBox.WriteData(testata)
            Next

        End If

        Return bizLottiResult

    End Function

    ''' <summary>
    ''' Modifica Lotto per il magazzino centrale
    ''' </summary>
    ''' <param name="lottoMagazzino"></param>
    ''' <param name="listTestateLog"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateLottoMagazzinoCentrale(lottoMagazzino As Entities.LottoMagazzino, listTestateLog As List(Of DataLogStructure.Testata)) As BizLottiResult

        Dim now As DateTime = DateTime.Now

        Using lock As New Onit.Shared.Manager.Lock.Lock(GetCodiceAziendaToLockLotti(Me.ContextInfos.IDApplicazione),
                                                        GetCodiceRisorsaToLockLotti(lottoMagazzino.CodiceLotto, lottoMagazzino.CodiceConsultorio))

            ' Caricamento dati originali del lotto
            Dim lottoMagazzinoOriginale As Entities.LottoMagazzino = Me.LoadLottoMagazzinoCentrale(lottoMagazzino.CodiceLotto, ContextInfos.IDUtente, String.Empty)

            ' Controllo modifica flag Obsoleto
            If lottoMagazzinoOriginale.Obsoleto <> lottoMagazzino.Obsoleto Then

                lottoMagazzino.IdUtenteModificaFlagObsoleto = Me.ContextInfos.IDUtente
                lottoMagazzino.DataModificaFlagObsoleto = now

            Else

                lottoMagazzino.IdUtenteModificaFlagObsoleto = Nothing
                lottoMagazzino.DataModificaFlagObsoleto = Nothing

            End If

            Dim ownTransaction As Boolean = False

            Try
                If Me.GenericProvider.Transaction Is Nothing Then
                    Me.GenericProvider.BeginTransaction()
                    ownTransaction = True
                End If

                ' Modifica dei dati del lotto in anagrafica lotti
                Me.GenericProvider.Lotti.UpdateLottoMagazzinoCentrale(lottoMagazzino)

                ' Log update lotto
                listTestateLog.Add(Me.CreaTestataLogUpdateLotto(lottoMagazzinoOriginale, lottoMagazzino, True))

                If ownTransaction Then

                    Me.GenericProvider.Commit()

                End If

            Catch ex As Exception

                If ownTransaction Then
                    Me.GenericProvider.Rollback()
                End If

                ex.InternalPreserveStackTrace()
                Throw

            End Try

        End Using

        Return New BizLottiResult(BizLottiResult.ResultType.Success, String.Empty)

    End Function

    ''' <summary>
    ''' Modifica del lotto in anagrafica.
    ''' Il salvataggio non viene effettuato se il lotto deve essere attivato ma non è possibile farlo.
    ''' Effettua la scrittura del log.
    ''' </summary>
    ''' <param name="lottoMagazzino"></param>
    ''' <param name="codiceConsultorioCorrente"></param>
    ''' <param name="codiceConsultorioMagazzino"></param>
    ''' <param name="disattivaLotto"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateLottoMagazzino(lottoMagazzino As Entities.LottoMagazzino, codiceConsultorioCorrente As String, codiceConsultorioMagazzino As String, disattivaLotto As Boolean, updateEtaAttivazione As Boolean) As BizLottiResult

        Dim listTestateLog As New List(Of DataLogStructure.Testata)()

        ' Modifica
        Dim bizLottiResult As BizLottiResult = Me.UpdateLottoMagazzino(lottoMagazzino, codiceConsultorioCorrente, codiceConsultorioMagazzino, disattivaLotto, updateEtaAttivazione, listTestateLog)

        ' Scrittura log
        If bizLottiResult.Result <> BizLotti.BizLottiResult.ResultType.GenericError Then

            For Each testata As DataLogStructure.Testata In listTestateLog
                LogBox.WriteData(testata)
            Next

        End If

        Return bizLottiResult

    End Function

    ''' <summary>
    ''' Modifica del lotto in anagrafica.
    ''' Il salvataggio non viene effettuato se il lotto deve essere attivato ma non è possibile farlo.
    ''' Aggiunge alla listTestateLog le testate da scrivere.
    ''' </summary>
    ''' <param name="lottoMagazzino"></param>
    ''' <param name="codiceConsultorioCorrente"></param>
    ''' <param name="codiceConsultorioMagazzino"></param>
    ''' <param name="disattivaLotto"></param>
    ''' <param name="listTestateLog"></param>
    ''' <param name="updateEtaAttivazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateLottoMagazzino(lottoMagazzino As Entities.LottoMagazzino, codiceConsultorioCorrente As String, codiceConsultorioMagazzino As String, disattivaLotto As Boolean, updateEtaAttivazione As Boolean, listTestateLog As List(Of DataLogStructure.Testata)) As BizLottiResult

        Dim now As DateTime = DateTime.Now

        Using lock As New Onit.Shared.Manager.Lock.Lock(GetCodiceAziendaToLockLotti(Me.ContextInfos.IDApplicazione),
                                                        GetCodiceRisorsaToLockLotti(lottoMagazzino.CodiceLotto, codiceConsultorioMagazzino))

            ' Controllo lotto attivabile:
            ' se il lotto originale non è attivo e il lotto modificato è attivo, 
            ' controlla se è possibile effettuare l'attivazione: non deve esistere un altro lotto già attivo
            ' associato allo stesso nome commerciale del lotto corrente.

            ' Caricamento dati originali del lotto
            Dim lottoMagazzinoOriginale As Entities.LottoMagazzino =
                Me.LoadLottoMagazzino(codiceConsultorioCorrente, codiceConsultorioMagazzino, lottoMagazzino.CodiceLotto)

            If Not lottoMagazzinoOriginale.Attivo And lottoMagazzino.Attivo Then

                If Me.GenericProvider.Lotti.IsActiveAltroLottoStessoNomeCommerciale(lottoMagazzino.CodiceLotto, lottoMagazzino.CodiceNomeCommerciale, codiceConsultorioCorrente) Then
                    '--
                    Return New BizLottiResult(BizLottiResult.ResultType.GenericError, "Salvataggio non effettuato: esiste un lotto con stesso Nome Commerciale già attivo.")
                    '--
                End If

            End If

            ' Controllo modifica flag Attivo
            If lottoMagazzinoOriginale.Attivo <> lottoMagazzino.Attivo Then
                lottoMagazzino.IdUtenteModificaFlagAttivo = Me.ContextInfos.IDUtente
                lottoMagazzino.DataModificaFlagAttivo = now
            Else
                lottoMagazzino.IdUtenteModificaFlagAttivo = Nothing
                lottoMagazzino.DataModificaFlagAttivo = Nothing
            End If

            ' Controllo modifica flag Obsoleto
            If lottoMagazzinoOriginale.Obsoleto <> lottoMagazzino.Obsoleto Then
                lottoMagazzino.IdUtenteModificaFlagObsoleto = Me.ContextInfos.IDUtente
                lottoMagazzino.DataModificaFlagObsoleto = now
            Else
                lottoMagazzino.IdUtenteModificaFlagObsoleto = Nothing
                lottoMagazzino.DataModificaFlagObsoleto = Nothing
            End If

            Dim ownTransaction As Boolean = False

            Try
                If Me.GenericProvider.Transaction Is Nothing Then
                    Me.GenericProvider.BeginTransaction()
                    ownTransaction = True
                End If

                ' Modifica dei dati del lotto in anagrafica lotti
                Me.GenericProvider.Lotti.UpdateLotto(lottoMagazzino)

                ' Log update lotto
                listTestateLog.Add(Me.CreaTestataLogUpdateLotto(lottoMagazzinoOriginale, lottoMagazzino, False))

                ' Modifica della quantità minima del lotto in consultorio per quanto riguarda il consultorio di magazzino
                Dim countUpdate As Integer =
                    Me.GenericProvider.Lotti.UpdateQuantitaMinimaLottoConsultorio(lottoMagazzino.CodiceLotto,
                                                                                  codiceConsultorioMagazzino,
                                                                                  lottoMagazzino.QuantitaMinima)

                If countUpdate > 0 Then

                    ' Log modifica quantità minima lotto-consultorio
                    listTestateLog.Add(Me.CreaTestataLogUpdateQuantitaMinimaLotto(lottoMagazzinoOriginale, lottoMagazzino))

                Else

                    ' Se non è stato effettuato nessun update nella lotti-consultori significa che non c'è un'associazione 
                    ' tra lotto e consultorio, che deve essere inserito relativamente al consultorio di magazzino
                    ' Il flag di attivazione viene messo a false perchè viene gestito nel seguito.
                    Dim lottoConsultorio As New Entities.LottoMagazzino()
                    lottoConsultorio.CodiceLotto = lottoMagazzino.CodiceLotto
                    lottoConsultorio.CodiceConsultorio = codiceConsultorioMagazzino
                    lottoConsultorio.DosiRimaste = lottoMagazzino.DosiRimaste
                    lottoConsultorio.QuantitaMinima = lottoMagazzino.QuantitaMinima
                    lottoConsultorio.Attivo = False
                    lottoConsultorio.IdUtenteModificaFlagAttivo = Nothing
                    lottoConsultorio.DataModificaFlagAttivo = Nothing

                    ' N.B: eta min e max non vengono inserite perchè sono gestite dall'update subito dopo l'endif.
                    Me.GenericProvider.Lotti.InsertLottoConsultorio(lottoConsultorio)

                    Me.CreaTestataLogInserimentoLottoConsultorio(lottoConsultorio, True,
                       "Inserito in automatico il lotto nel centro vaccinale, in seguito a tentativo di modifica quantità minima non riuscito.")

                End If

                ' Modifica del flag di attivazione nella lotti-consultori, per il consultorio corrente + log.
                Me.UpdateLottoAttivo(lottoMagazzino.CodiceLotto, codiceConsultorioCorrente, lottoMagazzino.QuantitaMinima, lottoMagazzino.Attivo,
                                     listTestateLog, lottoMagazzinoOriginale, lottoMagazzino.IdUtenteModificaFlagAttivo, lottoMagazzino.DataModificaFlagAttivo,
                                     lottoMagazzino.EtaMinimaAttivazione, lottoMagazzino.EtaMassimaAttivazione, updateEtaAttivazione)

                ' Disattivazione del lotto su tutti i consultori
                If disattivaLotto Then

                    Me.GenericProvider.Lotti.DisattivaLotto(lottoMagazzino.CodiceLotto, Me.ContextInfos.IDUtente, now)

                    listTestateLog.Add(Me.CreaTestataLogDisattivazioneLotto(lottoMagazzinoOriginale, lottoMagazzino, "Disattivazione del lotto su tutti i centri vaccinali, in seguito ad update"))

                End If

                If ownTransaction Then
                    Me.GenericProvider.Commit()
                End If

            Catch ex As Exception

                If ownTransaction Then
                    Me.GenericProvider.Rollback()
                End If

                ex.InternalPreserveStackTrace()
                Throw

            End Try

        End Using

        Return New BizLottiResult(BizLottiResult.ResultType.Success, String.Empty)

    End Function

#End Region

#Region " Inserimento Movimenti "

    ''' <summary>
    ''' Caricamento di un lotto in magazzino, in seguito all'eliminazione di una vaccinazione eseguita.
    ''' Inserimento di un movimento di scarico in magazzino e aggiornamento dosi rimaste del lotto in consultorio.
    ''' </summary>
    ''' <param name="movimentoLotto"></param>
    ''' <param name="codiceConsultorioCorrente"></param>
    ''' <param name="codiceConsultorioMagazzino"></param>
    ''' <param name="usaDosiScatola"></param>
    ''' <param name="listTestateLog"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CaricaLottoVaccinazione(movimentoLotto As Entities.MovimentoLotto, codiceConsultorioCorrente As String, codiceConsultorioMagazzino As String, usaDosiScatola As Boolean, listTestateLog As List(Of DataLogStructure.Testata)) As BizLottiResult

        Return Me.MovimentaMagazzino(movimentoLotto, codiceConsultorioCorrente, codiceConsultorioMagazzino, usaDosiScatola, False, listTestateLog)

    End Function

    ''' <summary>
    ''' Scaricamento di un lotto in magazzino, in seguito all'esecuzione di una vaccinazione.
    ''' Inserimento di un movimento di scarico in magazzino e aggiornamento dosi rimaste del lotto in consultorio.
    ''' </summary>
    ''' <param name="movimentoLotto"></param>
    ''' <param name="codiceConsultorioCorrente"></param>
    ''' <param name="codiceConsultorioMagazzino"></param>
    ''' <param name="usaDosiScatola"></param>
    ''' <param name="listTestateLog"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ScaricaLottoVaccinazione(movimentoLotto As MovimentoLotto, codiceConsultorioCorrente As String, codiceConsultorioMagazzino As String, usaDosiScatola As Boolean, listTestateLog As List(Of DataLogStructure.Testata)) As BizLottiResult

        Dim result As BizLottiResult = MovimentaMagazzino(movimentoLotto, codiceConsultorioCorrente, codiceConsultorioMagazzino, usaDosiScatola, True, listTestateLog)

        Return result

    End Function

    Public Function ScaricaLottoVaccinazioneNoTransactionScope(movimentoLotto As MovimentoLotto, codiceConsultorioCorrente As String, codiceConsultorioMagazzino As String, usaDosiScatola As Boolean, listTestateLog As List(Of DataLogStructure.Testata)) As BizLottiResult

        Return MovimentaMagazzinoNoTransactionScope(movimentoLotto, codiceConsultorioCorrente, codiceConsultorioMagazzino, usaDosiScatola, True, listTestateLog)

    End Function

    ''' <summary>
    ''' Inserimento di un movimento in magazzino. Se il progressivo non è valorizzato, viene calcolato dalla funzione.
    ''' Vengono aggiornate la dosi rimaste del lotto nel consultorio.
    ''' Se il movimento è un trasferimento, viene inserito anche il relativo movimento 
    ''' nel consultorio di destinazione (e aggiornate le dosi rimaste nel consultorio di destinazione).
    ''' Se vengono esaurite le dosi, viene anche disattivato il lotto (nel consultorio corrente).
    ''' </summary>
    ''' <param name="movimentoLotto"></param>
    ''' <param name="codiceConsultorioCorrente"></param>
    ''' <param name="codiceConsultorioMagazzino"></param>
    ''' <param name="usaDosiScatola"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function InserisciMovimentoMagazzino(movimentoLotto As Entities.MovimentoLotto, codiceConsultorioCorrente As String, codiceConsultorioMagazzino As String, usaDosiScatola As Boolean) As BizLottiResult

        Dim bizLottiResult As BizLottiResult = Nothing

        Dim listTestateLog As New List(Of DataLogStructure.Testata)()

        ' TODO [Magazzino]: AppId al posto di CodiceAzienda per gestire il caso multiAsl (Veneto)
        '                   Se OnVac supporterà la multiazienda, questo dovrebbe essere CodiceAzienda
        '                   Per non dare problemi, l'AppId deve essere lungo max 20 caratteri.
        '                   => refactor (AppId + CodiceAzienda) ?!?!
        Using lock As New Onit.Shared.Manager.Lock.Lock(GetCodiceAziendaToLockLotti(Me.ContextInfos.IDApplicazione),
                                                        GetCodiceRisorsaToLockLotti(movimentoLotto.CodiceLotto, codiceConsultorioMagazzino))

            bizLottiResult = Me.MovimentaMagazzino(movimentoLotto, codiceConsultorioCorrente, codiceConsultorioMagazzino,
                                                   usaDosiScatola, True, listTestateLog)

        End Using

        ' Scrittura log
        If bizLottiResult.Result <> BizLotti.BizLottiResult.ResultType.GenericError Then

            For Each testata As DataLogStructure.Testata In listTestateLog
                LogBox.WriteData(testata)
            Next

        End If

        Return bizLottiResult

    End Function

    ''' <summary>
    ''' Inserimento di un movimento relativo al magazzino specificato, dal magazzino centrale. 
    ''' Vengono aggiornate la dosi rimaste del lotto nel megazzino specificato.
    ''' Se il movimento è un trasferimento, viene inserito anche il relativo movimento 
    ''' nel magazzino di destinazione (e aggiornate le dosi rimaste nel magazzino di destinazione).
    ''' </summary>
    ''' <param name="codiceLotto"></param>
    ''' <param name="nomeCampoNumeroDosi"></param>
    ''' <param name="valoreCampoNumeroDosi"></param>
    ''' <param name="codiceTipoMovimento"></param>
    ''' <param name="codiceMagazzinoCorrente"></param>
    ''' <param name="codiceMagazzinoTrasferimento"></param>
    ''' <param name="descrizioneMagazzinoTrasferimento"></param>
    ''' <param name="noteMovimento"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function InserisciMovimentoMagazzinoCentrale(codiceLotto As String,
                                                        nomeCampoNumeroDosi As String, valoreCampoNumeroDosi As String,
                                                        codiceTipoMovimento As String,
                                                        codiceMagazzinoCorrente As String,
                                                        codiceMagazzinoTrasferimento As String, descrizioneMagazzinoTrasferimento As String,
                                                        noteMovimento As String) As BizLottiResult

        Dim errorMessage As New System.Text.StringBuilder()

        ' Numero dosi
        valoreCampoNumeroDosi = valoreCampoNumeroDosi.Trim()

        If String.IsNullOrEmpty(valoreCampoNumeroDosi) Then

            errorMessage.AppendFormat("\n - il campo '{0}' deve essere specificato", nomeCampoNumeroDosi)

        Else

            ' Controllo campo dosi
            Dim controlloDosiResult As BizLottiResult = Me.ControlliCampo(nomeCampoNumeroDosi, valoreCampoNumeroDosi, LUNGHEZZA_CAMPO_NUMERO_DOSI)

            If controlloDosiResult.Result <> BizLottiResult.ResultType.Success Then
                errorMessage.Append(controlloDosiResult.Message)
            End If

        End If

        ' Tipo movimento
        ' Il controllo sul tipo di movimento viene effettuato anche dal metodo InserisciMovimentoMagazzino(), ma lo eseguo qui 
        ' per effettuare tutti i controlli in una volta e restituire un messaggio che contiene il risultato di tutti i controlli eseguiti
        Dim controlloTipoMovimentoResult As BizLottiResult = Me.ControlloTipoMovimento(codiceTipoMovimento, codiceMagazzinoCorrente, codiceMagazzinoTrasferimento)

        If controlloTipoMovimentoResult.Result <> BizLottiResult.ResultType.Success Then

            errorMessage.Append(controlloTipoMovimentoResult.Message)

        End If

        ' In caso di errori, termina e restituisce il messaggio
        If errorMessage.Length > 0 Then

            Return New Biz.BizLotti.BizLottiResult(Biz.BizLotti.BizLottiResult.ResultType.GenericError,
                                                   String.Format("Salvataggio non effettuato: {0}", errorMessage.ToString()))
        End If

        ' Movimento che verrà inserito
        Dim movimentoLotto As New Entities.MovimentoLotto()

        ' Tipo movimento e luogo di trasferimento
        movimentoLotto.TipoMovimento = codiceTipoMovimento

        Select Case movimentoLotto.TipoMovimento

            Case Constants.TipoMovimentoMagazzino.Carico, Constants.TipoMovimentoMagazzino.Scarico

                movimentoLotto.CodiceConsultorioTrasferimento = String.Empty
                movimentoLotto.DescrizioneConsultorio = String.Empty

            Case Constants.TipoMovimentoMagazzino.TrasferimentoA

                movimentoLotto.CodiceConsultorioTrasferimento = codiceMagazzinoTrasferimento
                movimentoLotto.DescrizioneConsultorio = descrizioneMagazzinoTrasferimento

        End Select

        ' Impostazione dati del movimento
        movimentoLotto.NumeroDosi = Convert.ToInt32(valoreCampoNumeroDosi)
        movimentoLotto.DataRegistrazione = Date.Now
        movimentoLotto.Note = noteMovimento
        movimentoLotto.CodiceLotto = codiceLotto
        movimentoLotto.CodiceConsultorio = codiceMagazzinoCorrente
        movimentoLotto.IdUtente = Me.ContextInfos.IDUtente

        ' Inserimento movimento
        Return Me.InserisciMovimentoMagazzino(movimentoLotto, codiceMagazzinoCorrente, codiceMagazzinoCorrente, False)

    End Function

    ''' <summary>
    ''' Metodo che gestisce carichi/scarichi/trasferimenti dei lotti in magazzino 
    ''' e aggiorna le dosi rimaste del lotto nel consultorio 
    ''' </summary>
    ''' <param name="movimentoLotto"></param>
    ''' <param name="codiceConsultorioCorrente"></param>
    ''' <param name="codiceConsultorioMagazzino"></param>
    ''' <param name="usaDosiScatola"></param>
    ''' <param name="disattivaLottoSeScortaNulla">Se vale True e, dopo il movimento, le dosi rimaste sono 0, disattiva il lotto nel consultorio</param>
    ''' <param name="listTestateLog"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function MovimentaMagazzino(movimentoLotto As Entities.MovimentoLotto, codiceConsultorioCorrente As String, codiceConsultorioMagazzino As String,
                                        usaDosiScatola As Boolean, disattivaLottoSeScortaNulla As Boolean, listTestateLog As List(Of DataLogStructure.Testata)) As BizLottiResult

        Dim result As BizLottiResult

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, GetReadCommittedTransactionOptions())

            result = MovimentaMagazzinoNoTransactionScope(
                movimentoLotto, codiceConsultorioCorrente, codiceConsultorioMagazzino, usaDosiScatola, disattivaLottoSeScortaNulla, listTestateLog)

            transactionScope.Complete()

        End Using

        Return result

    End Function

    Private Function MovimentaMagazzinoNoTransactionScope(movimentoLotto As MovimentoLotto, codiceConsultorioCorrente As String, codiceConsultorioMagazzino As String,
                                                          usaDosiScatola As Boolean, disattivaLottoSeScortaNulla As Boolean, listTestateLog As List(Of DataLogStructure.Testata)) As BizLottiResult

        Dim success As Boolean = True

        Dim bizLottiErrorResult As BizLottiResult = Nothing

        Dim resultType As BizLottiResult.ResultType = BizLottiResult.ResultType.Success
        Dim resultMessage As String = String.Empty

        Dim controlMessage As New System.Text.StringBuilder()

        ' Controllo numero dosi movimento
        If movimentoLotto.NumeroDosi < 1 Then
            controlMessage.Append("\n - il numero di dosi specificato deve essere maggiore di zero")
        End If

        ' Controllo tipo movimento
        Dim controlloTipoMovimentoResult As BizLottiResult =
            ControlloTipoMovimento(movimentoLotto.TipoMovimento, codiceConsultorioMagazzino, movimentoLotto.CodiceConsultorioTrasferimento)

        If controlloTipoMovimentoResult.Result <> BizLottiResult.ResultType.Success Then
            controlMessage.Append(controlloTipoMovimentoResult.Message)
        End If

        If controlMessage.Length > 0 Then

            success = False
            bizLottiErrorResult = New BizLottiResult(BizLottiResult.ResultType.GenericError,
                                                     String.Format("Impossibile registrare il movimento di magazzino. Sono stati rilevati i seguenti errori nei dati inseriti:\n{0}", controlMessage.ToString()))
        Else

            ' Progressivo movimento
            If String.IsNullOrEmpty(movimentoLotto.Progressivo) Then
                movimentoLotto.Progressivo = Me.GetProgressivo()
            End If

            ' Lettura dati aggiornati del lotto corrente
            Dim lottoMagazzino As LottoMagazzino = LoadLottoMagazzino(movimentoLotto.CodiceConsultorio, codiceConsultorioMagazzino, movimentoLotto.CodiceLotto)

            ' Se è stato specificato un numero di scatole, lo moltiplico per il numero di dosi per scatola
            ' per ottenere il numero totale di dosi (se il numero di dosi per scatola è specificato)
            If usaDosiScatola AndAlso lottoMagazzino.DosiScatola > 0 Then
                movimentoLotto.NumeroDosi = movimentoLotto.NumeroDosi * lottoMagazzino.DosiScatola
            End If

            Dim dosiNegative As Boolean = False

            ' Se il movimento è uno scarico o un trasferimento, controllo che la quantità sia sufficiente
            If movimentoLotto.TipoMovimento = Constants.TipoMovimentoMagazzino.Scarico OrElse
               movimentoLotto.TipoMovimento = Constants.TipoMovimentoMagazzino.TrasferimentoA Then

                dosiNegative = True

                If lottoMagazzino.DosiRimaste < movimentoLotto.NumeroDosi Then

                    success = False
                    bizLottiErrorResult = New BizLottiResult(BizLottiResult.ResultType.GenericError,
                                                             "Impossibile registrare il movimento di magazzino. Le dosi rimaste sono inferiori a quelle da scaricare.")
                End If

            End If

            If success Then

                ' La data di registrazione è obbligatoria: se vuota viene valorizzata con la data di oggi
                If movimentoLotto.DataRegistrazione = Date.MinValue Then
                    movimentoLotto.DataRegistrazione = Date.Now
                End If

                ' Controllo lunghezza campo note
                If Not movimentoLotto.Note Is Nothing AndAlso movimentoLotto.Note.Length > 250 Then
                    movimentoLotto.Note = movimentoLotto.Note.Substring(0, 250)
                End If

                ' Inserimento del movimento
                GenericProvider.Lotti.InsertMovimento(movimentoLotto)

                ' Log inserimento movimento
                listTestateLog.Add(CreaTestataLogInserimentoMovimento(movimentoLotto, False, Nothing))

                ' Aggiornamento dosi rimaste per il lotto nel consultorio
                Dim countUpdatedRows As Integer = 0

                ' Se il movimento è uno scarico, inverto il segno del numero di dosi scaricate (che è sempre positivo).
                If dosiNegative Then
                    movimentoLotto.NumeroDosi = (-1) * movimentoLotto.NumeroDosi
                End If

                countUpdatedRows = GenericProvider.Lotti.UpdateDosiRimasteLottoConsultorio(movimentoLotto.CodiceLotto, codiceConsultorioMagazzino, movimentoLotto.NumeroDosi)
                If countUpdatedRows > 0 Then

                    ' Log modifica dosi rimaste
                    listTestateLog.Add(CreaTestataLogUpdateDosiRimasteLottoConsultorio(movimentoLotto.CodiceLotto, codiceConsultorioMagazzino, movimentoLotto.NumeroDosi))

                    If disattivaLottoSeScortaNulla Then

                        ' Disattivazione lotto se numero dosi rimaste = 0
                        ' Il controllo del numero di dosi avviene sempre sul consultorio di magazzino.
                        ' Se non viene effettuata all'interno di un lock, questa operazione può dare risultati errati!
                        If Me.GenericProvider.Lotti.GetDosiRimaste(movimentoLotto.CodiceLotto, codiceConsultorioMagazzino) = 0 Then

                            Dim now As DateTime = DateTime.Now

                            ' Disattivazione lotto in consultorio 
                            ' La disattivazione è sempre relativa al lotto associato al consultorio corrente (non a quello di magazzino, se diversi)
                            Me.GenericProvider.Lotti.UpdateLottoAttivo(movimentoLotto.CodiceLotto, codiceConsultorioCorrente, False, Me.ContextInfos.IDUtente, now,
                                                                           Nothing, Nothing, False)

                            ' Risultato di tipo warning
                            resultType = BizLottiResult.ResultType.LottoDisattivatoScortaNullaWarning
                            resultMessage = "Operazione effettuata con successo.\nIl lotto è stato DISATTIVATO poichè il numero di dosi è esaurito."

                            ' Log disattivazione
                            listTestateLog.Add(Me.CreaTestataLogUpdateLottoAttivo(movimentoLotto.CodiceLotto, codiceConsultorioCorrente, False, True,
                                                                                      "Disattivazione del lotto in seguito a raggiungimento scorta nulla.",
                                                                                      Nothing, Nothing, False))
                        End If

                    End If

                Else

                    ' Se non c'è l'associazione tra lotto e consultorio, viene creata.
                    Dim lottoMagazzinoToInsert As New LottoMagazzino()
                    lottoMagazzinoToInsert.CodiceLotto = lottoMagazzino.CodiceLotto
                    lottoMagazzinoToInsert.CodiceConsultorio = codiceConsultorioMagazzino
                    lottoMagazzinoToInsert.DosiRimaste = lottoMagazzino.DosiRimaste + movimentoLotto.NumeroDosi
                    lottoMagazzinoToInsert.QuantitaMinima = lottoMagazzino.QuantitaMinima
                    lottoMagazzinoToInsert.Attivo = False
                    lottoMagazzinoToInsert.IdUtenteModificaFlagAttivo = Nothing
                    lottoMagazzinoToInsert.DataModificaFlagAttivo = Nothing

                    ' N.B. : età min e max di attivazione non impostate nel caso di insert/update effettuate in seguito a movimenti
                    lottoMagazzinoToInsert.EtaMinimaAttivazione = Nothing
                    lottoMagazzinoToInsert.EtaMassimaAttivazione = Nothing

                    ' Inserimento associazione lotto-consultorio
                    GenericProvider.Lotti.InsertLottoConsultorio(lottoMagazzinoToInsert)

                    ' Log inserimento lotto-consultorio
                    listTestateLog.Add(Me.CreaTestataLogInserimentoLottoConsultorio(lottoMagazzinoToInsert, True,
                                           "Inserito in automatico il lotto nel centro vaccinale, in seguito a movimento."))

                End If

                ' Se il movimento è un trasferimento, bisogna inserire il movimento opposto nel consultorio di destinazione
                If movimentoLotto.TipoMovimento = Constants.TipoMovimentoMagazzino.TrasferimentoA Then

                    Dim movimentoLottoTrasferimento As New Entities.MovimentoLotto()
                    movimentoLottoTrasferimento.Progressivo = Me.GetProgressivo()
                    movimentoLottoTrasferimento.CodiceLotto = movimentoLotto.CodiceLotto
                    movimentoLottoTrasferimento.TipoMovimento = Constants.TipoMovimentoMagazzino.TrasferimentoDa
                    movimentoLottoTrasferimento.CodiceConsultorio = movimentoLotto.CodiceConsultorioTrasferimento
                    movimentoLottoTrasferimento.CodiceConsultorioTrasferimento = movimentoLotto.CodiceConsultorio
                    movimentoLottoTrasferimento.IdUtente = movimentoLotto.IdUtente
                    movimentoLottoTrasferimento.DataRegistrazione = movimentoLotto.DataRegistrazione
                    movimentoLottoTrasferimento.Note = movimentoLotto.Note
                    movimentoLottoTrasferimento.IdEsecuzioneAssociazione = String.Empty

                    ' In questo caso il mov è un trasferimento, quindi il numero di dosi è negativo (perchè l'ho invertito prima)
                    ' Nel movimento il numero di dosi deve sempre essere positivo, quindi lo inverto di nuovo.
                    movimentoLottoTrasferimento.NumeroDosi = (-1) * movimentoLotto.NumeroDosi

                    ' Inserimento movimento nel consultorio di trasferimento
                    GenericProvider.Lotti.InsertMovimento(movimentoLottoTrasferimento)

                    ' Log inserimento movimento
                    listTestateLog.Add(Me.CreaTestataLogInserimentoMovimento(movimentoLottoTrasferimento, True,
                                                                                 "Inserito in automatico il movimento, in seguito a trasferimento da altro centro vaccinale"))

                    ' Controllo presenza associazione lotto-consultorio
                    If Me.GenericProvider.Lotti.ExistsLottoConsultorio(movimentoLottoTrasferimento.CodiceLotto,
                                                                           movimentoLottoTrasferimento.CodiceConsultorio) Then

                        ' Aggiornamento dosi rimaste per il lotto nel consultorio di trasferimento
                        Me.GenericProvider.Lotti.UpdateDosiRimasteLottoConsultorio(movimentoLottoTrasferimento.CodiceLotto,
                                                                                       movimentoLottoTrasferimento.CodiceConsultorio,
                                                                                       movimentoLottoTrasferimento.NumeroDosi)

                        ' Log modifica dosi rimaste
                        listTestateLog.Add(Me.CreaTestataLogUpdateDosiRimasteLottoConsultorio(movimentoLottoTrasferimento.CodiceLotto,
                                                                                                  movimentoLottoTrasferimento.CodiceConsultorio,
                                                                                                  movimentoLottoTrasferimento.NumeroDosi))
                    Else

                        ' Inserimento associazione lotto-consultorio
                        Dim lottoConsultorioTrasferimento As New Entities.LottoMagazzino()
                        lottoConsultorioTrasferimento.CodiceLotto = movimentoLottoTrasferimento.CodiceLotto
                        lottoConsultorioTrasferimento.CodiceConsultorio = movimentoLottoTrasferimento.CodiceConsultorio
                        lottoConsultorioTrasferimento.DosiRimaste = movimentoLottoTrasferimento.NumeroDosi
                        lottoConsultorioTrasferimento.QuantitaMinima = 0
                        lottoConsultorioTrasferimento.Attivo = False
                        lottoConsultorioTrasferimento.IdUtenteModificaFlagAttivo = Nothing
                        lottoConsultorioTrasferimento.DataModificaFlagAttivo = Nothing

                        ' N.B. : età min e max di attivazione non impostate nel caso di insert/update effettuate in seguito a movimenti
                        lottoConsultorioTrasferimento.EtaMinimaAttivazione = Nothing
                        lottoConsultorioTrasferimento.EtaMassimaAttivazione = Nothing

                        Me.GenericProvider.Lotti.InsertLottoConsultorio(lottoConsultorioTrasferimento)

                        ' Log inserimento lotto-consultorio
                        listTestateLog.Add(Me.CreaTestataLogInserimentoLottoConsultorio(lottoConsultorioTrasferimento, True,
                                               "Inserito in automatico il lotto nel centro vaccinale, in seguito a trasferimento."))

                    End If

                End If

            End If

        End If

        If Not success Then Return bizLottiErrorResult

        Return New BizLottiResult(resultType, resultMessage)

    End Function

#End Region

#End Region

#Region " Private Methods "

#Region " Controlli sui campi "

    Private Function ControlliCampo(nomeCampo As String, valoreCampo As String, maxLenghtCampo As Integer) As BizLottiResult

        Dim errorMessage As New System.Text.StringBuilder()

        Dim valore As Integer = 0

        ' Controllo lunghezza campo
        If valoreCampo.Length > maxLenghtCampo Then

            errorMessage.AppendFormat("\n - il campo '{0}' non può contenere più di {1} caratteri.", nomeCampo, maxLenghtCampo.ToString())

        Else

            If Integer.TryParse(valoreCampo, valore) Then

                If valore < 0 Then
                    errorMessage.AppendFormat("\n - il valore del campo '{0}' non può essere negativo!", nomeCampo)
                End If

            Else
                errorMessage.AppendFormat("\n - il valore del campo '{0}' deve essere un numero intero!", nomeCampo)
            End If

        End If

        If errorMessage.Length > 0 Then

            Return New BizLottiResult(BizLottiResult.ResultType.GenericError, errorMessage.ToString())

        End If

        Return New BizLottiResult(BizLottiResult.ResultType.Success, String.Empty)

    End Function

    Private Function ControlloTipoMovimento(tipoMovimento As String, codiceMagazzinoCorrente As String, codiceMagazzinoTrasferimento As String) As BizLottiResult

        Dim controlMessage As New System.Text.StringBuilder()

        If String.IsNullOrEmpty(tipoMovimento) Then

            controlMessage.Append("\n - non è stato impostato nessun tipo di movimento")

        ElseIf tipoMovimento = Constants.TipoMovimentoMagazzino.TrasferimentoA Then

            If String.IsNullOrEmpty(codiceMagazzinoTrasferimento) Then

                controlMessage.Append("\n - non è stato specificato nessun magazzino per il tipo di movimento 'TRASFERIMENTO'")

            Else

                ' Controllo che il codice del magazzino verso cui si trasferisce sia in anagrafe consultori
                If Not Me.GenericProvider.Consultori.ExistsConsultorio(codiceMagazzinoTrasferimento) Then

                    controlMessage.Append("\n - il magazzino specificato per il tipo di movimento 'TRASFERIMENTO' non esiste.")

                End If

            End If

            ' Se il cns di provenienza fa da magazzino per il cns di destinazione, non posso effettuare il trasferimento
            If Me.IsConsultorioMagazzino(codiceMagazzinoCorrente, codiceMagazzinoTrasferimento) Then

                controlMessage.Append("\n - il magazzino di provenienza e quello di destinazione del TRASFERIMENTO coincidono.")

            End If

        End If

        If controlMessage.Length > 0 Then

            Return New BizLottiResult(BizLottiResult.ResultType.GenericError, controlMessage.ToString())

        End If

        Return New BizLottiResult(BizLottiResult.ResultType.Success, String.Empty)

    End Function

#End Region

#Region " Gestione Progressivo "

    ''' <summary>
    ''' Restituisce il progressivo di magazzino per l'anno corrente
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetProgressivo() As String

        Dim progressivoMagazzino As String = String.Empty

        Using bizProgressivi As New Biz.BizProgressivi(Me.ContextInfos, Me.LogOptions)

            progressivoMagazzino = bizProgressivi.CalcolaProgressivo(Constants.TipoProgressivo.Magazzino, True)

        End Using

        Return progressivoMagazzino

    End Function

#End Region

#Region " Gestione Log "

    ''' <summary>
    ''' Restituisce la testata per il log dell'attivazione/disattivazione del lotto
    ''' </summary>
    ''' <param name="codiceLotto"></param>
    ''' <param name="codiceConsultorio"></param>
    ''' <param name="attivo"></param>
    ''' <param name="automatico"></param>
    ''' <param name="note"></param>
    ''' <param name="etaMinimaAttivazione"></param>
    ''' <param name="etaMassimaAttivazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreaTestataLogUpdateLottoAttivo(codiceLotto As String, codiceConsultorio As String, attivo As Boolean, automatico As Boolean, note As String, etaMinimaAttivazione As Integer?, etaMassimaAttivazione As Integer?, updateEta As Boolean) As DataLogStructure.Testata

        Dim testata As New DataLogStructure.Testata(DataLogStructure.TipiArgomento.MAGAZZINO, DataLogStructure.Operazione.Modifica, 0, automatico)

        Dim record As New DataLogStructure.Record()

        record.Campi.Add(New DataLogStructure.Campo("lcn_lot_codice", codiceLotto))
        record.Campi.Add(New DataLogStructure.Campo("lcn_cns_codice", codiceConsultorio))
        record.Campi.Add(New DataLogStructure.Campo("lcn_attivo", attivo.ToString()))

        If Not String.IsNullOrEmpty(note) Then
            record.Campi.Add(New DataLogStructure.Campo("Note", note))
        End If

        If updateEta Then

            If etaMinimaAttivazione.HasValue Then
                record.Campi.Add(New DataLogStructure.Campo("lcn_eta_min_attivazione", etaMinimaAttivazione.Value.ToString()))
            End If

            If etaMassimaAttivazione.HasValue Then
                record.Campi.Add(New DataLogStructure.Campo("lcn_eta_max_attivazione", etaMassimaAttivazione.Value.ToString()))
            End If

        End If

        testata.Records.Add(record)

        Return testata

    End Function

    ''' <summary>
    ''' Restituisce la testata per il log dell'attivazione/disattivazione del lotto
    ''' </summary>
    ''' <param name="lottoMagazzinoOriginale"></param>
    ''' <param name="codiceLotto"></param>
    ''' <param name="codiceConsultorio"></param>
    ''' <param name="attivo"></param>
    ''' <param name="note"></param>
    ''' <param name="etaMinimaAttivazione"></param>
    ''' <param name="etaMassimaAttivazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreaTestataLogUpdateLottoAttivo(lottoMagazzinoOriginale As Entities.LottoMagazzino, codiceLotto As String, codiceConsultorio As String, attivo As Boolean, note As String, etaMinimaAttivazione As Integer?, etaMassimaAttivazione As Integer?) As DataLogStructure.Testata

        Dim testata As New DataLogStructure.Testata(DataLogStructure.TipiArgomento.MAGAZZINO, DataLogStructure.Operazione.Modifica, 0, False)

        Dim record As New DataLogStructure.Record()

        record.Campi.Add(New DataLogStructure.Campo("lcn_lot_codice", lottoMagazzinoOriginale.CodiceLotto, codiceLotto))
        record.Campi.Add(New DataLogStructure.Campo("lcn_cns_codice", lottoMagazzinoOriginale.CodiceConsultorio, codiceConsultorio))
        record.Campi.Add(New DataLogStructure.Campo("lcn_attivo", Me.GetStringValueFromFlag(lottoMagazzinoOriginale.Attivo), Me.GetStringValueFromFlag(attivo)))

        If Not String.IsNullOrEmpty(note) Then
            record.Campi.Add(New DataLogStructure.Campo("Note", note))
        End If

        If etaMinimaAttivazione.HasValue Then
            record.Campi.Add(New DataLogStructure.Campo("lcn_eta_min_attivazione", etaMinimaAttivazione.Value.ToString()))
        End If

        If etaMassimaAttivazione.HasValue Then
            record.Campi.Add(New DataLogStructure.Campo("lcn_eta_max_attivazione", etaMassimaAttivazione.Value.ToString()))
        End If

        testata.Records.Add(record)

        Return testata

    End Function

    ''' <summary>
    ''' Restituisce una testata per il log dell'inserimento di un lotto nell'anagrafica dei lotti, 
    ''' con segnalazione se l'operazione è avvenuta da magazzino centrale o meno.
    ''' </summary>
    ''' <param name="lottoMagazzino"></param>
    ''' <param name="isMagazzinoCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreaTestataLogInserimentoLotto(lottoMagazzino As Entities.LottoMagazzino, isMagazzinoCentrale As Boolean) As DataLogStructure.Testata

        Dim testata As New DataLogStructure.Testata(DataLogStructure.TipiArgomento.MAGAZZINO, DataLogStructure.Operazione.Inserimento, 0, False)

        Dim record As New DataLogStructure.Record()

        record.Campi.Add(New DataLogStructure.Campo("lot_codice", lottoMagazzino.CodiceLotto))
        record.Campi.Add(New DataLogStructure.Campo("lot_descrizione", lottoMagazzino.DescrizioneLotto))
        record.Campi.Add(NewCampoDateTimeToLog("lot_data_preparazione", lottoMagazzino.DataPreparazione))
        record.Campi.Add(NewCampoDateTimeToLog("lot_data_scadenza", lottoMagazzino.DataScadenza))
        record.Campi.Add(New DataLogStructure.Campo("lot_ditta", lottoMagazzino.Ditta))
        record.Campi.Add(New DataLogStructure.Campo("lot_dosi_scatola", lottoMagazzino.DosiScatola.ToString()))
        record.Campi.Add(New DataLogStructure.Campo("lot_note", lottoMagazzino.Note))
        record.Campi.Add(New DataLogStructure.Campo("lot_obsoleto", Me.GetStringValueFromFlag(lottoMagazzino.Obsoleto)))
        record.Campi.Add(New DataLogStructure.Campo("lot_noc_codice", lottoMagazzino.CodiceNomeCommerciale))

        If isMagazzinoCentrale Then
            record.Campi.Add(New DataLogStructure.Campo("lot_qta_minima_complessiva", lottoMagazzino.QuantitaMinima))
            record.Campi.Add(New DataLogStructure.Campo("Note", "Inserimento lotto da magazzino centrale"))
        Else
            record.Campi.Add(New DataLogStructure.Campo("Note", "Inserimento lotto da magazzino locale"))
        End If

        testata.Records.Add(record)

        Return testata

    End Function

    ''' <summary>
    ''' Restituisce una testata per il log dell'inserimento di un lotto in consultorio
    ''' </summary>
    ''' <param name="lottoConsultorio"></param>
    ''' <param name="note"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreaTestataLogInserimentoLottoConsultorio(lottoConsultorio As Entities.LottoMagazzino, automatico As Boolean, note As String) As DataLogStructure.Testata

        Dim testata As New DataLogStructure.Testata(DataLogStructure.TipiArgomento.MAGAZZINO, DataLogStructure.Operazione.Inserimento, 0, automatico)

        Dim record As New DataLogStructure.Record()

        record.Campi.Add(New DataLogStructure.Campo("lcn_lot_codice", lottoConsultorio.CodiceLotto))
        record.Campi.Add(New DataLogStructure.Campo("lcn_cns_codice", lottoConsultorio.CodiceConsultorio))
        record.Campi.Add(New DataLogStructure.Campo("lcn_attivo", lottoConsultorio.Attivo.ToString()))
        record.Campi.Add(New DataLogStructure.Campo("lcn_dosi_rimaste", lottoConsultorio.DosiRimaste.ToString()))
        record.Campi.Add(New DataLogStructure.Campo("lcn_qta_minima", lottoConsultorio.QuantitaMinima.ToString()))

        If Not String.IsNullOrEmpty(note) Then
            record.Campi.Add(New DataLogStructure.Campo("Note", note))
        End If

        testata.Records.Add(record)

        Return testata

    End Function

    ''' <summary>
    ''' Restituisce una testata per il log dell'inserimento di un movimento
    ''' </summary>
    ''' <param name="movimentoLotto"></param>
    ''' <param name="automatico"></param>
    ''' <param name="note"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreaTestataLogInserimentoMovimento(movimentoLotto As Entities.MovimentoLotto, automatico As Boolean, note As String) As DataLogStructure.Testata

        Dim testata As New DataLogStructure.Testata(DataLogStructure.TipiArgomento.MAGAZZINO, DataLogStructure.Operazione.Inserimento, 0, automatico)

        Dim record As New DataLogStructure.Record()

        record.Campi.Add(New DataLogStructure.Campo("mma_progressivo", movimentoLotto.Progressivo))
        record.Campi.Add(New DataLogStructure.Campo("mma_lot_codice", movimentoLotto.CodiceLotto))
        record.Campi.Add(New DataLogStructure.Campo("mma_n_dosi", movimentoLotto.NumeroDosi.ToString()))
        record.Campi.Add(New DataLogStructure.Campo("mma_tipo", movimentoLotto.TipoMovimento))
        record.Campi.Add(New DataLogStructure.Campo("mma_cns_codice", movimentoLotto.CodiceConsultorio))
        record.Campi.Add(NewCampoDateTimeToLog("mma_data_registrazione", movimentoLotto.DataRegistrazione))
        record.Campi.Add(New DataLogStructure.Campo("mma_tra_cns_codice", movimentoLotto.CodiceConsultorioTrasferimento))
        record.Campi.Add(New DataLogStructure.Campo("mma_note", movimentoLotto.Note))
        record.Campi.Add(New DataLogStructure.Campo("mma_ves_ass_prog", movimentoLotto.IdEsecuzioneAssociazione))

        If movimentoLotto.IdUtente <= 0 Then
            record.Campi.Add(New DataLogStructure.Campo("mma_ute_id", String.Empty))
        Else
            record.Campi.Add(New DataLogStructure.Campo("mma_ute_id", movimentoLotto.IdUtente.ToString()))
        End If

        If Not String.IsNullOrEmpty(note) Then
            record.Campi.Add(New DataLogStructure.Campo("Note", note))
        End If

        testata.Records.Add(record)

        Return testata

    End Function

    ''' <summary>
    ''' Restituisce una testata per il log della modifica del lotto in anagrafica
    ''' </summary>
    ''' <param name="lottoMagazzinoOriginale"></param>
    ''' <param name="lottoMagazzino"></param>
    ''' <param name="isMagazzinoCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreaTestataLogUpdateLotto(lottoMagazzinoOriginale As Entities.LottoMagazzino, lottoMagazzino As Entities.LottoMagazzino, isMagazzinoCentrale As Boolean) As DataLogStructure.Testata

        Dim testata As New DataLogStructure.Testata(DataLogStructure.TipiArgomento.MAGAZZINO, DataLogStructure.Operazione.Modifica, 0, False)

        Dim record As New DataLogStructure.Record()

        record.Campi.Add(New DataLogStructure.Campo("lot_obsoleto", Me.GetStringValueFromFlag(lottoMagazzinoOriginale.Obsoleto), Me.GetStringValueFromFlag(lottoMagazzino.Obsoleto)))
        record.Campi.Add(New DataLogStructure.Campo("lot_descrizione", lottoMagazzinoOriginale.DescrizioneLotto, lottoMagazzino.DescrizioneLotto))
        record.Campi.Add(NewCampoDateTimeToLog("lot_data_preparazione", lottoMagazzinoOriginale.DataPreparazione, lottoMagazzino.DataPreparazione))
        record.Campi.Add(NewCampoDateTimeToLog("lot_data_scadenza", lottoMagazzinoOriginale.DataScadenza, lottoMagazzino.DataScadenza))
        record.Campi.Add(New DataLogStructure.Campo("lot_ditta", lottoMagazzinoOriginale.Ditta, lottoMagazzino.Ditta))
        record.Campi.Add(New DataLogStructure.Campo("lot_dosi_scatola", lottoMagazzinoOriginale.DosiScatola.ToString(), lottoMagazzino.DosiScatola.ToString()))
        record.Campi.Add(New DataLogStructure.Campo("lot_note", lottoMagazzinoOriginale.Note, lottoMagazzino.Note))
        record.Campi.Add(New DataLogStructure.Campo("lot_noc_codice", lottoMagazzinoOriginale.CodiceNomeCommerciale, lottoMagazzino.CodiceNomeCommerciale))

        If isMagazzinoCentrale Then
            record.Campi.Add(New DataLogStructure.Campo("lot_qta_minima_complessiva", lottoMagazzinoOriginale.QuantitaMinima, lottoMagazzino.QuantitaMinima))
            record.Campi.Add(New DataLogStructure.Campo("Note", "Modifica lotto da magazzino centrale"))
        Else
            record.Campi.Add(New DataLogStructure.Campo("Note", "Modifica lotto da magazzino locale"))
        End If

        testata.Records.Add(record)

        Return testata

    End Function

    ''' <summary>
    ''' Restituisce una testata per il log della modifica della quantità minima del lotto
    ''' </summary>
    ''' <param name="lottoMagazzinoOriginale"></param>
    ''' <param name="lottoMagazzino"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreaTestataLogUpdateQuantitaMinimaLotto(lottoMagazzinoOriginale As Entities.LottoMagazzino, lottoMagazzino As Entities.LottoMagazzino) As DataLogStructure.Testata

        Dim testata As New DataLogStructure.Testata(DataLogStructure.TipiArgomento.MAGAZZINO, DataLogStructure.Operazione.Modifica, 0, False)

        Dim record As New DataLogStructure.Record()

        record.Campi.Add(New DataLogStructure.Campo("lcn_lot_codice", lottoMagazzinoOriginale.CodiceLotto, lottoMagazzino.CodiceLotto))
        record.Campi.Add(New DataLogStructure.Campo("lcn_cns_codice", lottoMagazzinoOriginale.CodiceConsultorio, lottoMagazzino.CodiceConsultorio))
        record.Campi.Add(New DataLogStructure.Campo("lcn_qta_minima", lottoMagazzinoOriginale.QuantitaMinima.ToString(), lottoMagazzino.QuantitaMinima.ToString()))

        testata.Records.Add(record)

        Return testata

    End Function

    ''' <summary>
    ''' Restituisce una testata per il log della disattivazione del lotto su tutti i consultori
    ''' </summary>
    ''' <param name="lottoMagazzino"></param>
    ''' <param name="note"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreaTestataLogDisattivazioneLotto(lottoMagazzino As Entities.LottoMagazzino, note As String) As DataLogStructure.Testata

        Dim testata As New DataLogStructure.Testata(DataLogStructure.TipiArgomento.MAGAZZINO, DataLogStructure.Operazione.Modifica, 0, False)

        Dim record As New DataLogStructure.Record()

        record.Campi.Add(New DataLogStructure.Campo("lcn_lot_codice", lottoMagazzino.CodiceLotto))
        record.Campi.Add(New DataLogStructure.Campo("lcn_attivo", "N"))

        If Not String.IsNullOrEmpty(note) Then
            record.Campi.Add(New DataLogStructure.Campo("Note", note))
        End If

        testata.Records.Add(record)

        Return testata

    End Function

    ''' <summary>
    ''' Restituisce una testata per il log della disattivazione del lotto su tutti i consultori
    ''' </summary>
    ''' <param name="lottoMagazzinoOriginale"></param>
    ''' <param name="lottoMagazzino"></param>
    ''' <param name="note"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreaTestataLogDisattivazioneLotto(lottoMagazzinoOriginale As Entities.LottoMagazzino, lottoMagazzino As Entities.LottoMagazzino, note As String) As DataLogStructure.Testata

        Dim testata As New DataLogStructure.Testata(DataLogStructure.TipiArgomento.MAGAZZINO, DataLogStructure.Operazione.Modifica, 0, False)

        Dim record As New DataLogStructure.Record()

        record.Campi.Add(New DataLogStructure.Campo("lcn_lot_codice", lottoMagazzinoOriginale.CodiceLotto, lottoMagazzino.CodiceLotto))
        record.Campi.Add(New DataLogStructure.Campo("lcn_attivo", Me.GetStringValueFromFlag(lottoMagazzinoOriginale.Attivo), "N"))

        If Not String.IsNullOrEmpty(note) Then
            record.Campi.Add(New DataLogStructure.Campo("Note", note))
        End If

        testata.Records.Add(record)

        Return testata

    End Function

    ''' <summary>
    ''' Restituisce una testata per il log della modifica del numero di dosi rimaste per il lotto nel consultorio specificato
    ''' </summary>
    ''' <param name="codiceLotto"></param>
    ''' <param name="codiceConsultorioMagazzino"></param>
    ''' <param name="numeroDosi"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreaTestataLogUpdateDosiRimasteLottoConsultorio(codiceLotto As String, codiceConsultorioMagazzino As String, numeroDosi As Integer) As DataLogStructure.Testata

        Dim testata As New DataLogStructure.Testata(DataLogStructure.TipiArgomento.MAGAZZINO, DataLogStructure.Operazione.Modifica, 0, False)

        Dim record As New DataLogStructure.Record()

        record.Campi.Add(New DataLogStructure.Campo("lcn_lot_codice", codiceLotto))
        record.Campi.Add(New DataLogStructure.Campo("lcn_cns_codice", codiceConsultorioMagazzino))
        record.Campi.Add(New DataLogStructure.Campo("NumeroDosiSommateAlleDosiRimaste", numeroDosi.ToString()))

        testata.Records.Add(record)

        Return testata

    End Function

    ''' <summary>
    ''' Restituisce un oggetto di tipo DataLogStructure.Campo con il valore della data in formato stringa
    ''' </summary>
    ''' <param name="nomeCampo"></param>
    ''' <param name="dateValue"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function NewCampoDateTimeToLog(nomeCampo As String, dateValue As DateTime) As DataLogStructure.Campo

        If dateValue = DateTime.MinValue Then
            Return New DataLogStructure.Campo(nomeCampo, String.Empty)
        End If

        Return New DataLogStructure.Campo(nomeCampo, dateValue.ToString("dd/MM/yyyy"))

    End Function

    ''' <summary>
    ''' Restituisce un oggetto di tipo DataLogStructure.Campo con i valori (vecchio e nuovo) delle date in formato stringa
    ''' </summary>
    ''' <param name="nomeCampo"></param>
    ''' <param name="oldDateValue"></param>
    ''' <param name="newDateValue"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function NewCampoDateTimeToLog(nomeCampo As String, oldDateValue As DateTime, newDateValue As DateTime) As DataLogStructure.Campo

        Dim oldStringValue As String = String.Empty

        If oldDateValue > DateTime.MinValue Then
            oldStringValue = oldDateValue.ToString("dd/MM/yyyy")
        End If

        Dim newStringValue As String = String.Empty

        If newDateValue > DateTime.MinValue Then
            newStringValue = newDateValue.ToString("dd/MM/yyyy")
        End If

        Return New DataLogStructure.Campo(nomeCampo, oldStringValue, newStringValue)

    End Function

    ''' <summary>
    ''' Restituisce "S" se il flag è true, "N" altrimenti.
    ''' </summary>
    ''' <param name="flagValue"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetStringValueFromFlag(flagValue As Boolean) As String

        Return IIf(flagValue, "S", "N")

    End Function

#End Region

#End Region

#Region " Lotti WebService "

    Public Function GetLottiByAicOrVacc(aic As String, princVacc As String, codiceUlss As String) As List(Of LottoRicercaWebService)

        Dim lotti As New List(Of LottoRicercaWebService)()

        If Not String.IsNullOrWhiteSpace(aic) Then

            lotti = GenericProvider.Lotti.GetLottiByAIC(aic, codiceUlss)

            For Each item As LottoRicercaWebService In lotti
                item.AIC = String.Empty
            Next

        ElseIf Not String.IsNullOrWhiteSpace(princVacc) Then

            lotti = GenericProvider.Lotti.GetLottiByCodVacc(princVacc, codiceUlss)

            For Each item As LottoRicercaWebService In lotti
                item.AvnVacc = String.Empty
            Next
        End If

        Return lotti

    End Function

#End Region


End Class
