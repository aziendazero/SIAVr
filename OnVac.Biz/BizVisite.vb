Imports System.Collections.Generic
Imports System.Linq

Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Log
Imports Onit.OnAssistnet.OnVac.Log.DataLogStructure
Imports Onit.OnAssistnet.OnVac.Entities


Public Class BizVisite
    Inherits BizClass

#Region " Constructors "

    Public Sub New(dbGenericProviderFactory As DbGenericProviderFactory, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)
        MyBase.New(dbGenericProviderFactory, settings, Nothing, contextInfos, logOptions)
    End Sub

    Public Sub New(genericprovider As DbGenericProvider, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)
        MyBase.New(genericprovider, settings, contextInfos, logOptions)
    End Sub

#End Region

#Region " IEqualityComparers "

    Private Class UslInserimentoConflittoVisiteComparer
        Implements IEqualityComparer(Of Entities.ConflittoVisite.DatiVisitaInConflitto)

        Public Function Equals1(x As Entities.ConflittoVisite.DatiVisitaInConflitto, y As Entities.ConflittoVisite.DatiVisitaInConflitto) As Boolean Implements IEqualityComparer(Of Entities.ConflittoVisite.DatiVisitaInConflitto).Equals

            If String.IsNullOrEmpty(x.CodiceUslVisita) AndAlso String.IsNullOrEmpty(y.CodiceUslVisita) Then
                Return False
            End If

            Return (x.CodiceUslVisita = y.CodiceUslVisita)

        End Function

        Public Function GetHashCode1(obj As Entities.ConflittoVisite.DatiVisitaInConflitto) As Integer Implements IEqualityComparer(Of Entities.ConflittoVisite.DatiVisitaInConflitto).GetHashCode

            Return obj.CodiceUslVisita.GetHashCode()

        End Function

    End Class

#End Region

#Region " Public "

#Region " Centrale "

#Region " Conflitti Visite "

    ''' <summary>
    ''' Restituisce il numero di visite "master" con conflitti
    ''' </summary>
    ''' <param name="filtriRicercaConflitti"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountConflittiVisite(filtriRicercaConflitti As Filters.FiltriRicercaConflittiDatiVaccinali) As Integer

        Return Me.GenericProviderCentrale.VisitaCentrale.CountConflittiVisiteCentrale(filtriRicercaConflitti)

    End Function

    Public Function GetConflittiVisite(filtriRicercaConflitti As Filters.FiltriRicercaConflittiDatiVaccinali, pageIndex As Integer?, pageSize As Integer?) As List(Of Entities.ConflittoVisite)

        Dim pagingOptions As OnAssistnet.Data.PagingOptions = Nothing

        If Not pageIndex Is Nothing AndAlso Not pageSize Is Nothing Then

            pagingOptions = New OnAssistnet.Data.PagingOptions()

            pagingOptions.PageIndex = pageIndex
            pagingOptions.PageSize = pageSize

        End If

        ' Restituisce le visite "padre" che hanno conflitti. 
        ' Ogni elemento contiene i dati centrali del paziente e una lista di tutte le visite in conflitto (padre compreso). 
        ' Nella lista dei conflitti sono presenti solo i dati centrali di ogni visita.
        ' I dati locali (data visita, malattia, bilancio, ...) devono essere recuperati in base alla usl a cui appartengono.
        Dim listConflittoVisite As List(Of Entities.ConflittoVisite) =
            Me.GenericProviderCentrale.VisitaCentrale.GetConflittiVisiteCentrale(filtriRicercaConflitti, pagingOptions)

        If listConflittoVisite Is Nothing OrElse listConflittoVisite.Count = 0 Then Return Nothing

        ' Insieme delle accoppiate Usl-IdVisite
        Dim visiteUsl As New Dictionary(Of String, List(Of Int64))()

        ' Valorizzazione delle coppie usl-visite
        For Each conflitto As Entities.ConflittoVisite In listConflittoVisite

            Dim listCodiciUsl As List(Of String) =
                conflitto.VisiteInConflitto.
                Distinct(New UslInserimentoConflittoVisiteComparer()).
                Select(Function(v) v.CodiceUslVisita).ToList()

            For Each codiceUsl As String In listCodiciUsl

                ' Ricerca id visite nell'elenco corrente dei conflitti 
                Dim codiceUslCorrente As String = codiceUsl     ' --> utilizzata come filtro nella Where

                Dim listIdVisite As List(Of Int64) =
                    conflitto.VisiteInConflitto.
                    Where(Function(v) v.CodiceUslVisita = codiceUslCorrente).
                    Select(Function(v) v.IdVisita).ToList()

                If Not listIdVisite Is Nothing AndAlso listIdVisite.Count > 0 Then

                    If Not visiteUsl.ContainsKey(codiceUsl) Then
                        ' Aggiunta dell'accoppiata usl-visite al dictionary
                        visiteUsl.Add(codiceUsl, listIdVisite)
                    Else
                        ' Aggiunta delle visite alla lista relativa alla usl
                        visiteUsl(codiceUsl) = visiteUsl(codiceUsl).Union(listIdVisite).ToList()
                    End If

                End If

            Next
        Next

        Dim enumeratorVisiteUsl As IDictionaryEnumerator = visiteUsl.GetEnumerator()

        While enumeratorVisiteUsl.MoveNext()

            Dim codiceUslCorrente As String = enumeratorVisiteUsl.Key.ToString()
            Dim listaVisiteInConflittoCorrente As List(Of Int64) = DirectCast(enumeratorVisiteUsl.Value, List(Of Int64))

            ' Ricerca dati visite per ogni usl
            Dim visiteLocali As List(Of Visita) = Nothing

            ' [Unificazione Ulss]: conflitti => solo vecchia versione
            Using genericProviderUsl As DbGenericProvider = GetDBGenericProviderByCodiceUslGestita(codiceUslCorrente)
                visiteLocali = genericProviderUsl.Visite.GetVisiteById(listaVisiteInConflittoCorrente)
            End Using

            ' Assegnazione valori locali ai dati delle vaccinazioni in conflitto
            If Not visiteLocali Is Nothing Then

                For Each visitaLocale As Visita In visiteLocali

                    Dim idVisitaCorrente As Int64 = visitaLocale.IdVisita

                    For Each conflitto As ConflittoVisite In listConflittoVisite

                        Dim datiVisiteInConflitto As ConflittoVisite.DatiVisitaInConflitto =
                            (From item As ConflittoVisite.DatiVisitaInConflitto In conflitto.VisiteInConflitto
                             Where item.IdVisita = idVisitaCorrente _
                             And item.CodiceUslVisita = codiceUslCorrente
                             Select item).FirstOrDefault()

                        If Not datiVisiteInConflitto Is Nothing Then

                            datiVisiteInConflitto.DataVisita = visitaLocale.DataVisita
                            datiVisiteInConflitto.NumeroBilancio = visitaLocale.BilancioNumero
                            datiVisiteInConflitto.DescrizioneBilancio = visitaLocale.BilancioDescrizione
                            datiVisiteInConflitto.CodiceMalattia = visitaLocale.MalattiaCodice
                            datiVisiteInConflitto.DescrizioneMalattia = visitaLocale.MalattiaDescrizione
                            datiVisiteInConflitto.DataFineSospensione = visitaLocale.DataFineSospensione

                            Exit For

                        End If

                    Next

                Next

            End If

        End While

        Return listConflittoVisite

    End Function

#End Region

#End Region

    ''' <summary>
    ''' Inserimento visita e osservazioni associate (se presenti). 
    ''' I dati inseriti sono quelli dell'entity visitaLocale, che comprende i dati non centralizzati (se visitaLocale è valorizzata). 
    ''' Prima di effettuare l'inserimento controlla se la visita è duplicata (controllando data visita oppure data visita e malattia in base al parametro VISITE_STESSA_DATA)
    ''' </summary>
    ''' <param name="insertVisitaCommand"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function InsertVisita(insertVisitaCommand As InserisciVisitaCommand) As InserisciVisitaResult

        Dim success As Boolean = True
        Dim resultMessageList As New List(Of BizResult.ResultMessage)()

        Dim idVisitaEsistente As Int64? = Nothing

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions)

            Dim canSaveVisitaResult As CanSaveVisitaResult = Me.CanSaveVisita(insertVisitaCommand.Visita, Nothing)

            resultMessageList.AddRange(canSaveVisitaResult.Messages)
            success = canSaveVisitaResult.Success

            idVisitaEsistente = canSaveVisitaResult.IdVisitaEsistente

            If success Then

                If insertVisitaCommand.Visita.DataFineSospensione > DateTime.MinValue Then

                    Dim note As String = insertVisitaCommand.Note
                    If String.IsNullOrWhiteSpace(note) Then note = "InsertVisita"

                    Dim bizResultSpostaConvocazione As BizResult =
                        Me.SpostaConvocazioniVaccinazioniDopoFineSospensioneIfNeeded(insertVisitaCommand.Visita.CodicePaziente, insertVisitaCommand.Visita.DataFineSospensione, note)

                    resultMessageList.AddRange(bizResultSpostaConvocazione.Messages)
                    success = bizResultSpostaConvocazione.Success

                End If

                If success Then

                    Me.SetVisitaAddedContextInfosIfNeeded(insertVisitaCommand.Visita)

                    ' --- Salvataggio intestazione in t_vis_visite --- '
                    Dim isVisitaInserita As Boolean = False

                    Try
                        ' Inserimento visita
                        isVisitaInserita = Me.GenericProvider.Visite.InsertVisita(insertVisitaCommand.Visita)

                        If isVisitaInserita Then
                            LogBox.WriteData(GetTestataLogInserimentoVisita(insertVisitaCommand.Visita))
                        End If

                    Catch ex As Exception
                        Throw New Exception("Errore nell'inserimento della visita.", ex)
                    End Try

                    ' --- Salvataggio righe di bilancio in t_vis_osservazioni --- '
                    If isVisitaInserita AndAlso Not insertVisitaCommand.Osservazioni Is Nothing Then

                        Dim countOsservazioniInserite As Integer = 0
                        Dim osservazione As Entities.Osservazione = Nothing

                        Try
                            ' Inserimento osservazioni
                            ' Se i campi flag visibilità e usl inserimento non sono valorizzati, li prende dalla visita.
                            For Each osservazione In insertVisitaCommand.Osservazioni

                                osservazione.IdVisita = insertVisitaCommand.Visita.IdVisita
                                osservazione.DataVisita = insertVisitaCommand.Visita.DataVisita

                                If InsertOsservazione(osservazione) Then countOsservazioniInserite += 1

                            Next

                            ' Log osservazioni inserite
                            If countOsservazioniInserite > 0 Then

                                Dim recordLog As New Record()
                                recordLog.Campi.Add(New Campo("VOS_VIS_ID", "", insertVisitaCommand.Visita.IdVisita))
                                recordLog.Campi.Add(New Campo("Totale osservazioni inserite:", String.Empty, countOsservazioniInserite.ToString()))

                                Dim testataLog As New Testata(TipiArgomento.GEST_BIL, Operazione.Inserimento)
                                testataLog.Records.Add(recordLog)

                                LogBox.WriteData(testataLog)

                            End If

                        Catch ex As Exception

                            Dim msg As New System.Text.StringBuilder()
                            msg.AppendFormat("Errore nell'inserimento di una osservazione.{0}", Environment.NewLine)
                            msg.AppendFormat("Id visita: {0}{1}", insertVisitaCommand.Visita.IdVisita.ToString(), Environment.NewLine)
                            msg.AppendFormat("Codice osservazione: {0}{1}", osservazione.OsservazioneCodice, Environment.NewLine)
                            msg.AppendFormat("Codice risposta: {0}{1}", osservazione.RispostaCodice, Environment.NewLine)
                            msg.AppendFormat("Numero bilancio: {0}{1}", osservazione.NumeroBilancio, Environment.NewLine)
                            msg.AppendFormat("Malattia: {0}{1}", osservazione.CodiceMalattia, Environment.NewLine)
                            msg.AppendFormat("Data Visita: {0}", insertVisitaCommand.Visita.DataVisita.ToShortDateString())

                            Throw New Exception(msg.ToString(), ex)

                        End Try

                        If insertVisitaCommand.ProgrammaBilancio Then

                            Using bizBilancioProgrammato As New Biz.BizBilancioProgrammato(Me.GenericProvider, Me.Settings, Me.ContextInfos, Me.LogOptions)

                                bizBilancioProgrammato.ProgrammaBilancioByVisita(insertVisitaCommand.Visita)

                            End Using

                        End If
                        ' Aggiungere insert dei viaggi 
                        If isVisitaInserita AndAlso Not insertVisitaCommand.listaViaggi Is Nothing Then
                            Dim v As ViaggioVisita = Nothing
                            Try
                                For Each v In insertVisitaCommand.listaViaggi
                                    v.IdVisita = insertVisitaCommand.Visita.IdVisita
                                    If v.Operazione = OperazioneViaggio.Insert Then
                                        InsertViaggio(v)
                                    End If
                                Next
                            Catch ex As Exception
                                Dim msg As New System.Text.StringBuilder()
                                msg.AppendFormat("Errore nell'inserimento di un nuovo viaggio.{0}", Environment.NewLine)
                                msg.AppendFormat("Id visita: {0}{1}", insertVisitaCommand.Visita.IdVisita.ToString(), Environment.NewLine)
                                msg.AppendFormat("Data inizio viaggio: {0}{1}", v.DataInizioViaggio.Date.ToShortDateString(), Environment.NewLine)
                                msg.AppendFormat("Data fine viaggio: {0}{1}", v.DataFineViaggio.Date.ToShortDateString(), Environment.NewLine)
                                msg.AppendFormat("Codice paese: {0}{1}", v.CodicePaese, Environment.NewLine)
                                msg.AppendFormat("Descrizione paese: {0}{1}", v.DescPaese, Environment.NewLine)

                                Throw New Exception(msg.ToString(), ex)
                            End Try

                        End If
                    End If
                End If
            End If

            If idVisitaEsistente.HasValue Then

                If insertVisitaCommand.UpdateVisitaCentraleInConflittoIfNeeded Then
                    Me.UpdateConflittoVisitaCentraleIfNeeded(idVisitaEsistente.Value, insertVisitaCommand.VisitaCentrale)
                End If

            End If

            transactionScope.Complete()

        End Using

        Return New InserisciVisitaResult(success, resultMessageList.AsEnumerable())

    End Function

    Public Function InsertOsservazione(osservazione As Entities.Osservazione) As Boolean
        '--
        Me.SetOsservazioneAddedContextInfosIfNeeded(osservazione)
        '--
        Return Me.GenericProvider.Visite.InsertOsservazione(osservazione)
        '--
    End Function

    ''' <summary>
    ''' Inserimento osservazione. Se ha inserito l'osservazione e recordLog è valorizzato, aggiunge le informazioni da loggare.
    ''' Restituisce il numero di osservazioni inserite.
    ''' </summary>
    ''' <param name="osservazione"></param>
    ''' <param name="recordLog"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function InsertOsservazione(osservazione As Entities.Osservazione, ByRef recordLog As Record) As Boolean

        Dim osservazioneInserted As Boolean = Me.InsertOsservazione(osservazione)

        If osservazioneInserted Then

            If Not recordLog Is Nothing Then
                recordLog.Campi.Add(New Campo("Inserimento osservazione per visita (VOS_VIS_ID):", String.Empty, osservazione.IdVisita))
                recordLog.Campi.Add(New Campo("VOS_OSS_CODICE", String.Empty, osservazione.OsservazioneCodice))
                recordLog.Campi.Add(New Campo("VOS_RIS_CODICE", String.Empty, osservazione.RispostaCodice))
                recordLog.Campi.Add(New Campo("VOS_RISPOSTA", String.Empty, osservazione.RispostaTesto))
            End If

        End If

        Return osservazioneInserted

    End Function

    ''' <summary>
    ''' Cancellazione osservazioni e visite del paziente nella data specificata e log delle operazioni effettuate.
    ''' </summary>
    ''' <param name="visita"></param>
    ''' <remarks></remarks>
    Public Function DeleteVisitaAndOsservazioni(visita As Entities.Visita) As EliminaVisitaAndOsservazioniResult

        Dim deleteVisitaAndOsservazioniResult As New EliminaVisitaAndOsservazioniResult

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions)

            Dim testataLog As New DataLogStructure.Testata(TipiArgomento.GEST_BIL, Operazione.Eliminazione, visita.CodicePaziente, False)
            Dim recordLog As New DataLogStructure.Record()

            Dim numOsservazioniEliminate As Integer = 0

            deleteVisitaAndOsservazioniResult.OsservazioniEliminate = Me.GenericProvider.Visite.GetOsservazioniByVisita(visita.IdVisita)

            For Each osservazioneEliminata As Entities.Osservazione In deleteVisitaAndOsservazioniResult.OsservazioniEliminate

                If Me.DeleteOsservazione(osservazioneEliminata) Then
                    numOsservazioniEliminate += 1
                End If

            Next

            ' Log osservazioni
            If numOsservazioniEliminate > 0 Then

                recordLog.Campi.Add(New DataLogStructure.Campo("VOS_VIS_ID", visita.IdVisita.ToString()))
                recordLog.Campi.Add(New DataLogStructure.Campo("VOS_DATA_VISITA", visita.DataVisita.ToShortDateString()))
                recordLog.Campi.Add(New DataLogStructure.Campo("Osservazioni eliminate", numOsservazioniEliminate))

                testataLog.Records.Add(recordLog)

            End If

            ' devo verificare se id visita è figlia di una altra
            ' in questo caso devo fare update su padre per cancellare collegamento
            If ExisistIdPadreFollowUp(visita.IdVisita) Then
                UpdateCancellaLegameFollowUp(visita.IdVisita)
            End If
            ' cancellazione dei viaggi collegati alla visita
            Dim deleteViaggi As Integer = EliminaViaggiVisita(visita.IdVisita)
            If deleteViaggi > 0 Then
                recordLog.Campi.Add(New DataLogStructure.Campo("VVG_VIS_ID", visita.IdVisita.ToString()))
                recordLog.Campi.Add(New DataLogStructure.Campo("Viaggi eliminati", deleteViaggi))

                testataLog.Records.Add(recordLog)
            End If

            ' Cancellazione visite
            Dim isVisitaEliminata As Boolean = Me.DeleteVisita(visita)

            ' Log osservazioni
            If isVisitaEliminata Then

                recordLog = New DataLogStructure.Record()
                recordLog.Campi.Add(New DataLogStructure.Campo("VIS_ID", visita.IdVisita.ToString()))
                recordLog.Campi.Add(New DataLogStructure.Campo("VIS_DATA_VISITA", visita.DataVisita.ToShortDateString()))

                testataLog.Records.Add(recordLog)

            End If

            If (numOsservazioniEliminate > 0 OrElse isVisitaEliminata) Then LogBox.WriteData(testataLog)

            transactionScope.Complete()

        End Using

        Return deleteVisitaAndOsservazioniResult

    End Function

    Public Sub InsertVisitaEliminata(visitaEliminata As Entities.Visita)

        Me.SetVisitaDeletedContextInfosIfNeeded(visitaEliminata)

        Me.GenericProvider.Visite.InsertVisitaEliminata(visitaEliminata)

    End Sub

    Public Function DeleteVisita(visitaEliminata As Entities.Visita) As Boolean

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions())

            Me.GenericProvider.Visite.EliminaVisita(visitaEliminata.IdVisita)

            Me.InsertVisitaEliminata(visitaEliminata)

            Me.DeleteBilanciVisita(visitaEliminata.CodicePaziente, visitaEliminata.BilancioNumero, visitaEliminata.MalattiaCodice)

            transactionScope.Complete()

        End Using

    End Function

    Public Sub InsertOsservazioneEliminata(osservazioneEliminata As Entities.Osservazione)

        Me.SetOsservazioneDeletedContextInfosIfNeeded(osservazioneEliminata)

        Me.GenericProvider.Visite.InsertOsservazioneEliminata(osservazioneEliminata)

    End Sub

    Public Function DeleteOsservazione(osservazioneEliminata As Entities.Osservazione) As Boolean

        Dim deleted As Boolean

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions())

            deleted = Me.GenericProvider.Visite.EliminaOsservazione(osservazioneEliminata.Id)

            Me.InsertOsservazioneEliminata(osservazioneEliminata)

            transactionScope.Complete()

        End Using

        Return deleted

    End Function

    ''' <summary>
    ''' Modifica risposta dell'osservazione specificata (solo se codice e/o testo della risposta e/o flag visibilità sono variati). 
    ''' Aggiunta di un campo al recordLog, se valorizzato.
    ''' </summary>
    ''' <param name="osservazioneNew"></param>
    ''' <param name="osservazioneOld"></param>
    ''' <param name="visita"></param>
    ''' <param name="recordLog"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateOsservazione(osservazioneNew As Entities.Osservazione, osservazioneOld As Entities.Osservazione, visita As Entities.Visita, ByRef recordLog As Record) As Boolean

        Dim osservazioniModificata As Boolean = Me.UpdateOsservazione(osservazioneNew)

        ' Log se è stato passato un record e se ci sono modifiche da loggare
        If Not recordLog Is Nothing AndAlso osservazioniModificata Then

            If osservazioneNew.RispostaCodice <> osservazioneOld.RispostaCodice Then
                recordLog.Campi.Add(New Campo(String.Format("VOS_RIS_CODICE (Osservazione: {0})", osservazioneNew.OsservazioneCodice),
                                                osservazioneOld.RispostaCodice,
                                                osservazioneNew.RispostaCodice))
            End If

            If osservazioneNew.RispostaTesto <> osservazioneOld.RispostaTesto Then
                recordLog.Campi.Add(New Campo(String.Format("VOS_RISPOSTA (Osservazione: {0})", osservazioneNew.OsservazioneCodice),
                                                osservazioneOld.RispostaTesto,
                                                osservazioneNew.RispostaTesto))
            End If

        End If

        Return osservazioniModificata

    End Function

    Public Function UpdateOsservazione(osservazione As Entities.Osservazione) As Boolean

        Me.SetOsservazioneModifiedContextInfosIfNeeded(osservazione)

        If Not osservazione.DataVariazione.HasValue Then
            osservazione.DataVariazione = DateTime.Now
        End If

        If Not osservazione.IdUtenteVariazione.HasValue Then
            osservazione.IdUtenteVariazione = Me.ContextInfos.IDUtente
        End If

        Return Me.GenericProvider.Visite.UpdateOsservazione(osservazione)

    End Function

    Public Function UpdateFollowUp(idVisita As Long, idFollowup As Long, dataFollowUpEff As Date?) As Integer
        Return GenericProvider.Visite.UpdateIdFollowUp(idVisita, idFollowup, dataFollowUpEff)
    End Function
    ''' <summary>
    ''' Update di 
    ''' </summary>
    ''' <param name="idVisita"></param>
    ''' <param name="idFollowup"></param>
    ''' <param name="dataFollowUpEff"></param>
    ''' <returns></returns>
    Public Function UpdateCancellaLegameFollowUp(idVisita As Long) As Integer
        Return GenericProvider.Visite.UpdateCancellaLegameFollowUp(idVisita)
    End Function


    Public Function UpdateVisita(updateVisitaCommand As ModificaVisitaCommand, fromUpdateBilanci As Boolean) As ModificaVisitaBizResult

        Dim success As Boolean = True
        Dim resultMessageList As New List(Of BizResult.ResultMessage)()
        Dim idVisitaEsistente As Int64? = Nothing
        Dim logRecordList As New List(Of DataLogStructure.Record)()
        Dim convocazioniDeleted As Boolean

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions())

            Dim canSaveVisitaResult As CanSaveVisitaResult = Me.CanSaveVisita(updateVisitaCommand.Visita, updateVisitaCommand.VisitaOriginale)

            resultMessageList.AddRange(canSaveVisitaResult.Messages)
            success = canSaveVisitaResult.Success

            idVisitaEsistente = canSaveVisitaResult.IdVisitaEsistente

            If success Then

                If updateVisitaCommand.Visita.DataFineSospensione > updateVisitaCommand.VisitaOriginale.DataFineSospensione Then

                    Dim note As String = updateVisitaCommand.Note
                    If String.IsNullOrWhiteSpace(note) Then note = "UpdateVisita"

                    Dim spostaConvocazioniBizResult As BizResult =
                        Me.SpostaConvocazioniVaccinazioniDopoFineSospensioneIfNeeded(updateVisitaCommand.Visita.CodicePaziente, updateVisitaCommand.Visita.DataFineSospensione, note)

                    resultMessageList.AddRange(spostaConvocazioniBizResult.Messages)
                    success = spostaConvocazioniBizResult.Success

                End If

            End If

            If success Then

                Me.SetVisitaModifiedContextInfosIfNeeded(updateVisitaCommand.Visita)

                If updateVisitaCommand.SovrascriviInfoModifica Then

                    updateVisitaCommand.Visita.DataModifica = DateTime.Now
                    updateVisitaCommand.Visita.IdUtenteModifica = Me.ContextInfos.IDUtente

                Else

                    If Not updateVisitaCommand.Visita.DataModifica.HasValue Then
                        updateVisitaCommand.Visita.DataModifica = DateTime.Now
                    End If

                    If Not updateVisitaCommand.Visita.IdUtenteModifica.HasValue Then
                        updateVisitaCommand.Visita.IdUtenteModifica = Me.ContextInfos.IDUtente
                    End If

                End If

                ' controllo che se c'è almeno un viaggio prendo la data di fine massima da valorizzare come prevista per followup
                If Not updateVisitaCommand.ListaViaggi Is Nothing AndAlso updateVisitaCommand.ListaViaggi.Count > 0 Then
                    If Not updateVisitaCommand.Visita.DataFollowUpPrevisto.HasValue Or updateVisitaCommand.Visita.DataFollowUpPrevisto.Value = DateTime.MinValue Then
                        Dim dataMaxViaggio As Date = updateVisitaCommand.ListaViaggi.Where(Function(p) p.Operazione <> OperazioneViaggio.Delete).Max(Function(p) p.DataFineViaggio).Date
                        updateVisitaCommand.Visita.DataFollowUpPrevisto = dataMaxViaggio.AddDays(Settings.NUM_GIORNI_FOLLOWUP)
                    End If
                End If


                GenericProvider.Visite.UpdateVisita(updateVisitaCommand.Visita, fromUpdateBilanci)
                ' gestione osservazioni
                If Not updateVisitaCommand.Osservazioni Is Nothing Then

                    Dim osservazioneInserita As Boolean

                    Dim recordLog As New Record()

                    For Each osservazione As Entities.Osservazione In updateVisitaCommand.Osservazioni

                        osservazione.DataVisita = updateVisitaCommand.Visita.DataVisita

                        If osservazione.Id > 0 Then

                            Dim osservazioneOld As Entities.Osservazione = Me.GenericProvider.Visite.GetOsservazioneById(osservazione.Id)

                            ' Modifica osservazione
                            Me.UpdateOsservazione(osservazione, osservazioneOld, updateVisitaCommand.Visita, recordLog)

                        Else

                            osservazione.IdVisita = updateVisitaCommand.Visita.IdVisita

                            ' Inserimento dell'osservazione
                            Me.InsertOsservazione(osservazione, recordLog)

                            osservazioneInserita = True

                        End If

                    Next

                    logRecordList.Add(recordLog)

                    If updateVisitaCommand.Visita.BilancioNumero.HasValue Then

                        If osservazioneInserita Then

                            Using bizBilancioProgrammato As New Biz.BizBilancioProgrammato(Me.GenericProvider, Me.Settings, Me.ContextInfos, Me.LogOptions)
                                convocazioniDeleted = bizBilancioProgrammato.CancellaBilanciByVisita(updateVisitaCommand.Visita)
                            End Using

                        End If

                    End If

                End If
                ' gestione viaggi
                If Not updateVisitaCommand.listaViaggi Is Nothing Then
                    For Each v As ViaggioVisita In updateVisitaCommand.ListaViaggi
                        If v.Operazione = OperazioneViaggio.Update Then
                            UpdateViaggio(v)
                        End If
                        If v.Operazione = OperazioneViaggio.Insert Then
                            v.IdVisita = updateVisitaCommand.Visita.IdVisita
                            InsertViaggio(v)
                        End If
                        If v.Operazione = OperazioneViaggio.Delete Then
                            EliminaViaggio(v.Id)
                        End If
                    Next
                End If

            End If

                If idVisitaEsistente.HasValue Then

                If updateVisitaCommand.UpdateVisitaCentraleInConflittoIfNeeded Then
                    Me.UpdateConflittoVisitaCentraleIfNeeded(idVisitaEsistente.Value, updateVisitaCommand.VisitaCentrale)
                End If

            End If

            transactionScope.Complete()

        End Using

        Dim updateVisitaResult As New ModificaVisitaBizResult(success, resultMessageList.AsEnumerable())
        updateVisitaResult.ConvocazioniDeleted = convocazioniDeleted
        updateVisitaResult.OsservazioniLogRecords = logRecordList.ToArray()

        Return updateVisitaResult

    End Function

    ''' <summary>
    ''' Update flag di visibilità. Vengono modificati anche utente e data di variazione.
    ''' </summary>
    ''' <param name="idVisita"></param>
    ''' <param name="flagVisibilita"></param>
    ''' <returns></returns>
    Public Function UpdateFlagVisibilita(idVisita As Long, flagVisibilita As String) As Integer

        Dim visitaOriginale As Visita = GetVisitaById(idVisita)

        Dim dataModifica As Date = Date.Now

        Dim count As Integer = GenericProvider.Visite.UpdateFlagVisibilita(idVisita, flagVisibilita, ContextInfos.IDUtente, dataModifica)

        If count > 0 Then

            Dim bizLogOptions As BizLogOptions

            If LogOptions Is Nothing Then
                bizLogOptions = New BizLogOptions(TipiArgomento.VISITE, False)
            Else
                bizLogOptions = New BizLogOptions(LogOptions.CodiceArgomento, LogOptions.Automatico)
            End If

            Dim testataLog As New Testata(bizLogOptions.CodiceArgomento, Operazione.Modifica, visitaOriginale.CodicePaziente, bizLogOptions.Automatico)

            Dim recordLog As New DataLogStructure.Record()

            recordLog.Campi.Add(New Campo("Modificato flag visibilita visita (VIS_ID):", idVisita.ToString()))

            If String.IsNullOrWhiteSpace(visitaOriginale.FlagVisibilitaDatiVaccinaliCentrale) Then
                recordLog.Campi.Add(New Campo("VIS_FLAG_VISIBILITA", String.Empty, flagVisibilita))
            Else
                recordLog.Campi.Add(New Campo("VIS_FLAG_VISIBILITA", visitaOriginale.FlagVisibilitaDatiVaccinaliCentrale, flagVisibilita))
            End If

            If visitaOriginale.IdUtenteModifica.HasValue Then
                recordLog.Campi.Add(New Campo("VIS_UTE_ID_VARIAZIONE", visitaOriginale.IdUtenteModifica.Value.ToString(), ContextInfos.IDUtente.ToString()))
            Else
                recordLog.Campi.Add(New Campo("VIS_UTE_ID_VARIAZIONE", String.Empty, ContextInfos.IDUtente.ToString()))
            End If

            If visitaOriginale.DataModifica.HasValue Then
                recordLog.Campi.Add(New Campo("VIS_DATA_VARIAZIONE", visitaOriginale.DataModifica.Value, dataModifica))
            Else
                recordLog.Campi.Add(New Campo("VIS_DATA_VARIAZIONE", String.Empty, dataModifica))
            End If

            testataLog.Records.Add(recordLog)

            If testataLog.Records.Count > 0 Then
                LogBox.WriteData(testataLog)
            End If

        End If

        Return count

    End Function

    ''' <summary>
    ''' Restituisce i dati del bilancio relativo alla visita specificata
    ''' </summary>
    ''' <param name="idVisita"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAnamnesiByVisita(idVisita As Long) As Anamnesi.Anamnesi

        Dim visita As Visita = GenericProvider.BilancioProgrammato.GetVisitaBilancio(idVisita)

        Dim datiPaziente As PazienteDatiAnagrafici = GenericProvider.Paziente.GetDatiAnagraficiPaziente(visita.CodicePaziente)

        Dim listOsservazioni As List(Of Osservazione) = GenericProvider.Visite.GetInfoOsservazioniSezioniRisposteByVisita(idVisita)

        If listOsservazioni Is Nothing OrElse listOsservazioni.Count = 0 Then Return Nothing

        listOsservazioni =
            listOsservazioni.OrderBy(Function(p) p.SezioneNumero).ThenBy(Function(q) q.OsservazioneNumero).ToList()

        Dim list As List(Of Anamnesi.Anamnesi) =
            listOsservazioni.GroupBy(Function(p) p.IdVisita).Select(Function(item) New Anamnesi.Anamnesi() With
            {
                .IdVisita = visita.IdVisita,
                .DataVisita = visita.DataVisita,
                .BilancioDescrizione = visita.BilancioDescrizione,
                .BilancioNumero = visita.BilancioNumero,
                .BilancioEtaMinima = GetEtaDescrizione(visita.EtaGiorniEsecuzione),
                .Firma = visita.Firma,
                .MalattiaCodice = visita.MalattiaCodice,
                .MalattiaDescrizione = visita.MalattiaDescrizione,
                .Altezza = visita.Altezza,
                .PercentileAltezza = visita.PercentileAltezza,
                .RegistraAltezza = visita.RegistraAltezza,
                .Peso = visita.Peso,
                .PercentilePeso = visita.PercentilePeso,
                .RegistraPeso = visita.RegistraPeso,
                .Cranio = visita.Cranio,
                .PercentileCranio = visita.PercentileCranio,
                .RegistraCranio = visita.RegistraCranio,
                .PazienteCodice = Convert.ToInt64(datiPaziente.CodicePaziente),
                .PazienteCognome = datiPaziente.Cognome,
                .PazienteNome = datiPaziente.Nome,
                .PazienteSesso = datiPaziente.Sesso,
                .PazienteDataNascita = datiPaziente.DataNascita.Value,
                .PazienteIndirizzo = IIf(Not String.IsNullOrEmpty(datiPaziente.IndirizzoDomicilio), datiPaziente.IndirizzoDomicilio, datiPaziente.IndirizzoResidenza),
                .MedicoCodice = visita.MedicoCodice,
                .MedicoDescrizione = visita.MedicoDescrizione,
                .IdUtente = visita.IdUtente,
                .Sezioni = item.GroupBy(Function(q) q.SezioneNumero).Select(Function(sez) New Anamnesi.Sezione() With
                {
                    .SezioneNumero = sez.Key,
                    .SezioneCodice = sez.First().SezioneCodice,
                    .SezioneDescrizione = sez.First().SezioneDescrizione,
                    .Osservazioni = sez.GroupBy(Function(p) p.OsservazioneCodice).Select(Function(oss) New Anamnesi.Osservazione() With
                    {
                        .OsservazioneCodice = oss.Key,
                        .OsservazioneDescrizione = oss.First().OsservazioneDescrizione,
                        .OsservazioneId = oss.First().Id,
                        .OsservazioneNumero = oss.First().OsservazioneNumero,
                        .RispostaCodice = oss.First().RispostaCodice,
                        .RispostaDescrizione = oss.First().RispostaDescrizione,
                        .RispostaTesto = oss.First().RispostaTesto
                    }).OrderBy(Function(ord) ord.OsservazioneNumero).ToList()
                }).ToList()
            }).ToList()

        Return list.First()

    End Function

    ''' <summary>
    ''' Restituisce il dataset con i dati per la stampa del bilancio relativo alla visita specificata
    ''' </summary>
    ''' <param name="idVisita"></param>
    ''' <param name="codicePaziente"></param>
    ''' <param name="isCentrale"></param>
    ''' <param name="gestioneVaccinazioni"></param>
    ''' <param name="gestioneViaggi"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDataSetAnamnesiDefault(idVisita As Long, codicePaziente As String, isCentrale As Boolean, gestioneVaccinazioni As Boolean, gestioneViaggi As Boolean) As DataSet.AnamnesiDefault

        Dim dstAnamnesiDefault As New DataSet.AnamnesiDefault()

        ' --- Osservazioni --- '
        Dim listOsservazioni As List(Of Osservazione) = Me.GenericProvider.Visite.GetListOsservazioniByVisita(idVisita)


        For Each osservazione As Osservazione In listOsservazioni

            Dim rowOsservazione As DataSet.AnamnesiDefault.OsservazioniRow = dstAnamnesiDefault.Osservazioni.NewOsservazioniRow()

            rowOsservazione.vos_vis_id = osservazione.IdVisita
            rowOsservazione.sez_codice = osservazione.SezioneCodice
            rowOsservazione.sez_descrizione = osservazione.SezioneDescrizione
            rowOsservazione.sez_n_sezione = osservazione.SezioneNumero
            rowOsservazione.oss_codice = osservazione.OsservazioneCodice
            rowOsservazione.oss_descrizione = osservazione.OsservazioneDescrizione
            rowOsservazione.osb_n_osservazione = osservazione.OsservazioneNumero
            rowOsservazione.OsservazioneDisabilitata = osservazione.OsservazioneDisabilitata
            rowOsservazione.vos_ris_codice = osservazione.RispostaCodice
            rowOsservazione.ris_descrizione = osservazione.RispostaDescrizione
            rowOsservazione.vos_risposta = osservazione.RispostaTesto
            rowOsservazione.visibile = "F"

            dstAnamnesiDefault.Osservazioni.AddOsservazioniRow(rowOsservazione)

        Next


        Dim instal As Installazione = GenericProvider.Installazioni.GetInstallazione(ContextInfos.CodiceUsl)
        If Not instal Is Nothing Then
            Dim rowInstallazione As DataSet.AnamnesiDefault.InstallazioneRow = dstAnamnesiDefault.Installazione.NewInstallazioneRow()
            rowInstallazione.Piede_Report = instal.PiedeReport
            rowInstallazione.Usl_Cap = instal.UslCap
            rowInstallazione.Usl_Citta = instal.UslCitta
            rowInstallazione.Usl_indirizzo = instal.UslIndirizzo
            rowInstallazione.Usl_Partita_Iva = instal.UslPartitaIva
            rowInstallazione.Intestazione_report = instal.IntestazioneReport
            rowInstallazione.Installazione = instal.CodiceInstallazione
            dstAnamnesiDefault.Installazione.AddInstallazioneRow(rowInstallazione)
        End If

        ' --- Dati visita, bilancio, paziente e viaggio --- '
        Dim visita As Entities.Visita = GetVisitaById(idVisita)

        Dim datiPazienteIntestazione As Entities.PazienteDatiAnagraficiIntestazioneBilancio = Nothing
        Using bizPaziente As New Biz.BizPaziente(Me.GenericProvider, Me.Settings, Me.ContextInfos, Nothing)

            datiPazienteIntestazione = bizPaziente.GetDatiAnagraficiPazienteIntestazioneBilancio(codicePaziente, isCentrale)

        End Using

        Dim rowVisita As DataSet.AnamnesiDefault.VisitaRow = dstAnamnesiDefault.Visita.NewVisitaRow()

        rowVisita.vis_id = visita.IdVisita
        rowVisita.bil_n_bilancio = visita.BilancioNumero.Value
        rowVisita.mal_codice = visita.MalattiaCodice
        rowVisita.mal_descrizione = visita.MalattiaDescrizione
        rowVisita.ute_descrizione = visita.MedicoDescrizione
        rowVisita.vis_data_visita = visita.DataVisita.Date

        If Not String.IsNullOrWhiteSpace(visita.Note) Then rowVisita.vis_note = visita.Note

        If visita.Peso.HasValue Then
            rowVisita.vis_peso = visita.Peso.Value.ToString()
        Else
            rowVisita.vis_peso = String.Empty
        End If

        If visita.Altezza.HasValue Then
            rowVisita.vis_altezza = visita.Altezza.Value.ToString()
        Else
            rowVisita.vis_altezza = String.Empty
        End If

        If visita.Cranio.HasValue Then
            rowVisita.vis_cranio = visita.Cranio.Value.ToString()
        Else
            rowVisita.vis_cranio = String.Empty
        End If

        rowVisita.vis_percentile_peso = visita.PercentilePeso
        rowVisita.vis_percentile_altezza = visita.PercentileAltezza
        rowVisita.vis_percentile_cranio = visita.PercentileCranio

        rowVisita.bil_eta_minima = GetEtaDescrizione(visita.EtaGiorniEsecuzione)
        rowVisita.bil_descrizione = visita.BilancioDescrizione
        rowVisita.bil_cranio = IIf(visita.RegistraCranio, "S", "N")
        rowVisita.bil_altezza = IIf(visita.RegistraAltezza, "S", "N")
        rowVisita.bil_peso = IIf(visita.RegistraPeso, "S", "N")

        rowVisita.paz_cognome = datiPazienteIntestazione.Cognome
        rowVisita.paz_nome = datiPazienteIntestazione.Nome
        If Not String.IsNullOrWhiteSpace(datiPazienteIntestazione.Sesso) Then rowVisita.paz_sesso = datiPazienteIntestazione.Sesso
        If Not String.IsNullOrWhiteSpace(datiPazienteIntestazione.CodiceFiscale) Then rowVisita.paz_codice_fiscale = datiPazienteIntestazione.CodiceFiscale
        If Not String.IsNullOrWhiteSpace(datiPazienteIntestazione.TesseraSanitaria) Then rowVisita.paz_tessera = datiPazienteIntestazione.TesseraSanitaria

        If Not String.IsNullOrWhiteSpace(datiPazienteIntestazione.IndirizzoDomicilio) Then
            rowVisita.paz_indirizzo = datiPazienteIntestazione.IndirizzoDomicilio
            rowVisita.paz_comune_indirizzo = datiPazienteIntestazione.DescrizioneComuneDomicilio
            rowVisita.paz_provincia_indirizzo = datiPazienteIntestazione.ProvinciaDomicilio
            rowVisita.paz_cap_indirizzo = datiPazienteIntestazione.CapDomicilio
        ElseIf Not String.IsNullOrWhiteSpace(datiPazienteIntestazione.IndirizzoResidenza) Then
            rowVisita.paz_indirizzo = datiPazienteIntestazione.IndirizzoResidenza
            rowVisita.paz_comune_indirizzo = datiPazienteIntestazione.DescrizioneComuneResidenza
            rowVisita.paz_provincia_indirizzo = datiPazienteIntestazione.ProvinciaResidenza
            rowVisita.paz_cap_indirizzo = datiPazienteIntestazione.CapResidenza
        End If

        rowVisita.paz_data_nascita = datiPazienteIntestazione.DataNascita
        rowVisita.paz_comune_nascita = datiPazienteIntestazione.DescrizioneComuneNascita
        rowVisita.paz_provincia_nascita = datiPazienteIntestazione.ProvinciaNascita
        rowVisita.paz_cap_nascita = datiPazienteIntestazione.CapNascita

        If Not String.IsNullOrWhiteSpace(datiPazienteIntestazione.CodiceCittadinanza) Then
            rowVisita.paz_cit_codice = datiPazienteIntestazione.CodiceCittadinanza
            rowVisita.paz_cittadinanza_stato = datiPazienteIntestazione.StatoCittadinanza
        End If

        If Not String.IsNullOrWhiteSpace(datiPazienteIntestazione.CodiceMedico) Then
            rowVisita.paz_med_codice_base = datiPazienteIntestazione.CodiceMedico
            If Not String.IsNullOrWhiteSpace(datiPazienteIntestazione.CognomeMedico) Then
                rowVisita.paz_med_cognome_nome = String.Format("{0} {1}", datiPazienteIntestazione.CognomeMedico, datiPazienteIntestazione.NomeMedico)
            End If
        End If

        If Not String.IsNullOrWhiteSpace(datiPazienteIntestazione.Padre) Then rowVisita.paz_padre = datiPazienteIntestazione.Padre
        If Not String.IsNullOrWhiteSpace(datiPazienteIntestazione.Madre) Then rowVisita.paz_madre = datiPazienteIntestazione.Madre

        If gestioneViaggi Then
            If visita.DataInizioViaggio.HasValue Then rowVisita.vis_data_inizio_viaggio = visita.DataInizioViaggio.Value
            If visita.DataFineViaggio.HasValue Then rowVisita.vis_data_fine_viaggio = visita.DataFineViaggio.Value
            If Not String.IsNullOrEmpty(visita.PaeseViaggioDescrizione) Then rowVisita.vis_paese_viaggio = visita.PaeseViaggioDescrizione
            Dim listaViaggi As List(Of ViaggioVisita) = GenericProvider.Visite.GetListViaggiVisitaById(idVisita)
            For Each v As ViaggioVisita In listaViaggi
                Dim rowViaggi As DataSet.AnamnesiDefault.ViaggiRow = dstAnamnesiDefault.Viaggi.NewViaggiRow()

                rowViaggi.id = v.Id
                rowViaggi.vvg_vis_id = v.IdVisita
                rowViaggi.data_inizio = v.DataInizioViaggio
                rowViaggi.data_fine = v.DataFineViaggio
                rowViaggi.codice_paese = v.CodicePaese
                rowViaggi.descrizione_paese = v.DescPaese
                rowViaggi.giorni = Biz.BizVisite.CalcolaTotaleGiorniViaggio(v.DataInizioViaggio, v.DataFineViaggio)
                dstAnamnesiDefault.Viaggi.AddViaggiRow(rowViaggi)
            Next
        End If

        rowVisita.vis_medico_codice = visita.MedicoCodice
        rowVisita.vis_medico_descrizione = visita.MedicoDescrizione
        rowVisita.vis_rilevatore_codice = visita.RilevatoreCodice
        rowVisita.vis_rilevatore_descrizione = visita.RilevatoreDescrizione
        rowVisita.vis_vaccinabile = visita.Vaccinabile
        rowVisita.vis_firma = visita.Firma
        rowVisita.vis_cns_codice = visita.CodiceConsultorio
        rowVisita.vis_cns_descrizione = visita.DescrizioneConsultorio
        If Not String.IsNullOrWhiteSpace(visita.CodiceConsultorio) Then
            Dim consultorio As Cns = GenericProvider.Consultori.GetConsultorio(visita.CodiceConsultorio)
            Dim rowConsultorio As DataSet.AnamnesiDefault.CentroVaccinaleRow = dstAnamnesiDefault.CentroVaccinale.NewRow()
            rowConsultorio.Codice = consultorio.Codice
            rowConsultorio.Descrizione = consultorio.Descrizione
            rowConsultorio.Indirizzo = consultorio.Indirizzo
            rowConsultorio.Telefono = consultorio.Telefono
            rowConsultorio.Email = consultorio.Email
            dstAnamnesiDefault.CentroVaccinale.AddCentroVaccinaleRow(rowConsultorio)
        End If

        ' gestione followup
        If visita.FollowUpId.HasValue Then
            rowVisita.vis_id_followup = visita.FollowUpId
        End If
        If visita.DataFollowUpPrevisto.HasValue Then
            rowVisita.vis_data_followup_prevista = visita.DataFollowUpPrevisto
        End If
        If visita.DataFollowUpEffettivo.HasValue Then
            rowVisita.vis_data_followup_effettivo = visita.DataFollowUpEffettivo
        End If

        dstAnamnesiDefault.Visita.AddVisitaRow(rowVisita)

        ' --- Vaccinazioni --- '
        If gestioneVaccinazioni Then

            Dim listVaccinazioniDosi As List(Of Entities.BilancioVaccinazione) = GetListVaccinazioniBilancioSplitted(visita.VaccinazioniBilancio)

            If Not listVaccinazioniDosi.IsNullOrEmpty() Then

                Dim listCodiciDescrizioni As List(Of KeyValuePair(Of String, String)) =
                    Me.GenericProvider.AnaVaccinazioni.GetCodiceDescrizioneVaccinazioni(listVaccinazioniDosi.Select(Function(p) p.Codice).ToList())

                For Each vac As Entities.BilancioVaccinazione In listVaccinazioniDosi

                    Dim rowVaccinazione As DataSet.AnamnesiDefault.VaccinazioniRow = dstAnamnesiDefault.Vaccinazioni.NewVaccinazioniRow()

                    rowVaccinazione.IdVisita = idVisita
                    rowVaccinazione.Codice = vac.Codice
                    rowVaccinazione.Dose = vac.Dose.Value
                    rowVaccinazione.Descrizione = listCodiciDescrizioni.FirstOrDefault(Function(p) p.Key = vac.Codice).Value

                    dstAnamnesiDefault.Vaccinazioni.AddVaccinazioniRow(rowVaccinazione)

                Next

            End If

        End If

        Return dstAnamnesiDefault

    End Function

    ''' <summary>
    ''' Restituisce la visita in base all'id specificato
    ''' </summary>
    ''' <param name="idVisita"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetVisitaById(idVisita As Long) As Visita

        Return GenericProvider.Visite.GetVisitaById(idVisita)

    End Function

    ''' <summary>
    ''' Restituisce la lista delle visite del paziente specificato.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetVisitePaziente(codicePaziente As String) As List(Of Visita)

        Return GenericProvider.Visite.GetVisitePaziente(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce la data di fine sospensione massima per il paziente specificato, minvalue altrimenti.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMaxDataFineSospensione(codicePaziente As Integer) As DateTime

        Return GenericProvider.Visite.GetMaxDataFineSospensione(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce i giorni che intercorrono tra le due date specificate, estremi inclusi
    ''' </summary>
    ''' <param name="dataInizio"></param>
    ''' <param name="dataFine"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CalcolaTotaleGiorniViaggio(dataInizio As DateTime, dataFine As DateTime) As Integer?

        If dataInizio = DateTime.MinValue Then Return Nothing
        If dataFine = DateTime.MinValue Then Return Nothing
        If dataFine < dataInizio Then Return Nothing

        Return Convert.ToInt32(Math.Round((dataFine.Date - dataInizio.Date).TotalDays + 1))

    End Function

    ''' <summary>
    ''' Restituisce una lista di oggetti BilancioVaccinazione, composta da tutte le vaccinazioni presenti in anagrafica.
    ''' Per le vaccinazioni selezionate, specificate nella stringa vaccinazioniBilancio (nel formato: "codVac1|dose1;codVac2|dose2;..."), viene valorizzata anche la dose.
    ''' Per le altre, la dose rimane nulla. Se non è specificata la stringa da splittare ma è valorizzata la data di convocazione, vengono caricate le vaccinazioni presenti in quella data.
    ''' La lista contiene, per prime, le vaccinazioni selezionate ordinate in base al campo Ordine, poi le altre vaccinazioni ordinate sempre in base allo stesso campo.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="vaccinazioniBilancio"></param>
    ''' <param name="dataConvocazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetVaccinazioniBilancio(codicePaziente As String, vaccinazioniBilancio As String, dataConvocazione As DateTime?) As List(Of Entities.BilancioVaccinazione)

        ' N.B. : codicePaziente è string perchè questo metodo viene richiamato anche dalla Visione Bilanci in configurazione Centrale.
        '        Il codice del paziente non viene mai usato da questo metodo se siamo in configurazione centrale, ma verrebbe sollevata un'eccezione runtime se fosse long.

        Dim list As New List(Of Entities.BilancioVaccinazione)()

        ' Se la stringa con vaccinazioni e dosi è valorizzata, la splitto e restituisco la lista ottenuta, che avrà solo i campi Codice e Dose valorizzati.
        Dim listSelezionate As List(Of Entities.BilancioVaccinazione) = GetListVaccinazioniBilancioSplitted(vaccinazioniBilancio)

        ' Altrimenti, controllo se c'è la data di convocazione per caricarne le vaccinazioni
        If listSelezionate.IsNullOrEmpty() Then

            If dataConvocazione.HasValue Then

                Dim listVaccinazioniConvocazione As List(Of Entities.BilancioVaccinazione) =
                    Me.GenericProvider.VaccinazioneProg.GetVacProgNotEseguiteAndNotEscluseListBilancioVaccinazioni(codicePaziente, dataConvocazione.Value)

                If Not listVaccinazioniConvocazione.IsNullOrEmpty() Then

                    listVaccinazioniConvocazione = listVaccinazioniConvocazione.OrderBy(Function(p) p.Ordine).ToList()

                    listSelezionate = New List(Of Entities.BilancioVaccinazione)()
                    listSelezionate.AddRange(listVaccinazioniConvocazione)

                End If
            End If

        End If

        ' Leggo dall'anagrafica la lista delle vaccinazioni, che avrà i campi Codice, Descrizione e Ordine valorizzati (ed è ordinata per il campo Ordine).
        Dim listAnagrafica As List(Of Entities.BilancioVaccinazione) = Me.GenericProvider.AnaVaccinazioni.GetListBilancioVaccinazioneFromAnagrafica()

        ' Se ho delle vaccinazioni selezionate, riempio i campi mancanti con i valori dell'anagrafica
        If Not listSelezionate.IsNullOrEmpty() Then

            For Each vaccinazioneSelezionata As Entities.BilancioVaccinazione In listSelezionate

                Dim vaccinazioneAnagrafica As Entities.BilancioVaccinazione = listAnagrafica.FirstOrDefault(Function(p) p.Codice = vaccinazioneSelezionata.Codice)
                If vaccinazioneAnagrafica IsNot Nothing Then

                    vaccinazioneSelezionata.Descrizione = vaccinazioneAnagrafica.Descrizione
                    vaccinazioneSelezionata.Ordine = vaccinazioneAnagrafica.Ordine

                End If

            Next

            ' Ordino le selezionate in base al campo Ordine
            listSelezionate = listSelezionate.OrderBy(Function(p) p.Ordine).ToList()

            ' Rimuovo le selezionate dalla lista di quelle in anagrafica
            listAnagrafica.RemoveAll(Function(anagrafica) listSelezionate.Any(Function(selezionata) anagrafica.Codice = selezionata.Codice))

            ' Aggiungo le selezionate alla lista da restituire
            list.AddRange(listSelezionate)

        End If

        ' Aggiungo le anagrafiche alla lista da restituire
        If Not listAnagrafica.IsNullOrEmpty() Then

            list.AddRange(listAnagrafica)

        End If

        Return list

    End Function

    ''' <summary>
    ''' Restituisce una lista di oggetti BilancioVaccinazione, ognuno dei quali contiene il codice e la dose (la descrizione è vuota).
    ''' La valorizzazione avviene splittando la stringa passata per parametro, nel formato: "codVac1|dose1;codVac2|dose2;..."
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetListVaccinazioniBilancioSplitted(value As String) As List(Of Entities.BilancioVaccinazione)

        If String.IsNullOrWhiteSpace(value) Then
            Return Nothing
        End If

        Dim listVac As New List(Of Entities.BilancioVaccinazione)()

        Dim list As String() = value.Split({";"}, StringSplitOptions.RemoveEmptyEntries)
        If Not list.IsNullOrEmpty() Then

            For Each entry As String In list

                Dim item As String() = entry.Split({"|"}, StringSplitOptions.RemoveEmptyEntries)

                Dim itemVac As New Entities.BilancioVaccinazione()
                itemVac.Codice = item(0)
                itemVac.Dose = item(1)

                listVac.Add(itemVac)

            Next
        End If

        Return listVac

    End Function

    ''' <summary>
    ''' Controlla se il paziente ha una sospensione alle vaccinazioni
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataEsecuzione"></param>
    ''' <returns></returns>
    Public Function CheckSospensioneVaccinazioniPaziente(codicePaziente As Long, dataEsecuzione As DateTime) As BizGenericResult

        Dim result As New BizGenericResult()
        result.Success = True
        result.Message = String.Empty

        Dim dataFineSospensione As DateTime? = GenericProvider.Visite.GetMaxDataFineSospensione(codicePaziente, dataEsecuzione)

        If dataFineSospensione.HasValue Then
            result.Success = False
            result.Message = String.Format("L'assistito è sospeso fino al {0}. Non è possibile procedere con la vaccinazione.", dataFineSospensione.Value.ToString("dd/MM/yyyy", Globalization.CultureInfo.InvariantCulture))
        End If

        Return result

    End Function

    Public Function GetListaViaggiVisita(idVisita As Long) As List(Of ViaggioVisita)
        Return GenericProvider.Visite.GetListViaggiVisitaById(idVisita).OrderByDescending(Function(p) p.DataInizioViaggio).ToList()
    End Function

    Public Function UpdateViaggio(viaggio As ViaggioVisita) As Boolean
        Return GenericProvider.Visite.UpdateViaggio(viaggio)
    End Function

    Public Function InsertViaggio(viaggio As ViaggioVisita) As Boolean
        Return GenericProvider.Visite.InsertViaggio(viaggio)
    End Function

    Public Function EliminaViaggio(idViaggio As Integer) As Boolean
        Return GenericProvider.Visite.EliminaViaggio(idViaggio)
    End Function

    Public Function EliminaViaggiVisita(idVisita As Integer) As Integer
        Dim viaggi As List(Of ViaggioVisita) = GenericProviderCentrale.Visite.GetListViaggiVisitaById(idVisita)
        Dim ret As Integer = 0
        For Each v As ViaggioVisita In viaggi
                GenericProvider.Visite.EliminaViaggio(v.Id)
                ret = ret + 1
            Next

        Return ret
    End Function

#End Region

#Region " Friend "

    Friend Function CanSaveVisita(codicePaziente As Int64, dataVisita As DateTime, codiceMalattia As String, numeroBilancio As Int64?, idVisita As Int64?) As CanSaveVisitaResult

        Dim idVisitaEsistente As Int64?

        If codicePaziente <> -1 Then
            If numeroBilancio.HasValue And Not String.IsNullOrWhiteSpace(codiceMalattia) Then
                '--
                ' Controllo che non sia già presente una visita (diversa) per stesso bilancio e stessa malattia 
                '--
                idVisitaEsistente = Me.GenericProvider.Visite.GetIdVisitaPazienteByBilancioAndMalattia(codicePaziente, numeroBilancio.Value, codiceMalattia)

                If Me.ExistsVisita(idVisitaEsistente, idVisita) Then
                    Return New CanSaveVisitaResult(False, idVisitaEsistente.Value, New String() {"Esiste già una visita per il paziente per lo stesso bilancio, relativa alla stessa malattia."})
                End If
                '--
            End If

            If Not Me.Settings.VISITE_STESSA_DATA Then

                ' Se è già presente una visita (diversa) in stessa data (senza considerare nè la malattia nè il bilancio) => il controllo FALLISCE
                idVisitaEsistente = Me.GenericProvider.Visite.GetIdVisitaPazienteByData(codicePaziente, dataVisita)

                If Me.ExistsVisita(idVisitaEsistente, idVisita) Then
                    Return New CanSaveVisitaResult(False, idVisitaEsistente.Value, New String() {"Esiste già una visita per il paziente nella stessa data."})
                End If

            Else

                If Not String.IsNullOrEmpty(codiceMalattia) Then
                    '--
                    ' Codice malattia specificato => controllo che non sia già presente una visita (diversa) in stessa data per la stessa malattia 
                    '--
                    idVisitaEsistente = Me.GenericProvider.Visite.GetIdVisitaPazienteByDataAndMalattia(codicePaziente, dataVisita, codiceMalattia)

                    If Me.ExistsVisita(idVisitaEsistente, idVisita) Then
                        Return New CanSaveVisitaResult(False, idVisitaEsistente.Value, New String() {"Esiste già una visita per il paziente nella stessa data, relativa alla stessa malattia."})
                    End If
                    '--
                Else
                    '--
                    ' Codice malattia nullo => controllo che non ci sia già una visita in stessa data senza malattia specificata
                    '--
                    idVisitaEsistente = Me.GenericProvider.Visite.GetIdVisitaPazienteSenzaMalattiaByData(codicePaziente, dataVisita)

                    If Me.ExistsVisita(idVisitaEsistente, idVisita) Then
                        Return New CanSaveVisitaResult(False, idVisitaEsistente.Value, New String() {"Esiste già una visita per il paziente nella stessa data."})
                    End If
                    '--
                End If

            End If
        Else
            Return New CanSaveVisitaResult(False, idVisitaEsistente, New String() {"Il codice del paziente non è corretto!"})
        End If

        Return New CanSaveVisitaResult()

    End Function

#Region " Centrale "

    Friend Function AggiornaVisitaCentrale(codicePazienteCentrale As String, visita As Visita, tipoVisitaCentrale As String,
                                           isMerge As Boolean, isRisoluzioneConflitto As Boolean) As VisitaCentrale

        ' TODO [Unificazione Ulss]: se arriva qui con il codice usl nuovo, scoppia

        Dim visitaCentrale As VisitaCentrale = GenericProviderCentrale.VisitaCentrale.GetVisitaCentraleByIdLocale(visita.IdVisita, UslGestitaCorrente.Codice)

        Dim existsVisitaCentrale As Boolean = Not visitaCentrale Is Nothing

        If Not existsVisitaCentrale Then

            visitaCentrale = New VisitaCentrale()
            visitaCentrale.IdVisita = visita.IdVisita
            visitaCentrale.CodicePaziente = visita.CodicePaziente
            visitaCentrale.CodicePazienteCentrale = codicePazienteCentrale
            visitaCentrale.CodiceUslVisita = visita.CodiceUslInserimento
            visitaCentrale.DataInserimentoVisita = visita.DataRegistrazione
            visitaCentrale.IdUtenteInserimentoVisita = visita.IdUtente

        Else

            'VISIBILITA (REVOCA)
            Dim isRevocaVisibilitaCentrale As Boolean = False
            Dim isVisibilitaCentrale As Boolean = Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(visita.FlagVisibilitaDatiVaccinaliCentrale)

            If Not String.IsNullOrEmpty(visitaCentrale.FlagVisibilitaCentrale) Then

                Dim wasVisibilitaCentrale As Boolean = Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(visitaCentrale.FlagVisibilitaCentrale)

                If isVisibilitaCentrale AndAlso Not wasVisibilitaCentrale Then

                    visitaCentrale.DataRevocaVisibilita = Nothing

                ElseIf Not isVisibilitaCentrale AndAlso wasVisibilitaCentrale Then

                    visitaCentrale.DataRevocaVisibilita = DateTime.Now

                    isRevocaVisibilitaCentrale = True

                End If

            End If

            'CONFLITTI
            Dim idConflittoRimanente As Int64? = Nothing

            If isRisoluzioneConflitto Then

                visitaCentrale.IdUtenteRisoluzioneConflitto = Me.ContextInfos.IDUtente
                visitaCentrale.DataRisoluzioneConflitto = DateTime.Now

            ElseIf (isRevocaVisibilitaCentrale OrElse tipoVisitaCentrale = Constants.TipoVisitaCentrale.Eliminata) AndAlso
                   visitaCentrale.IdConflitto.HasValue AndAlso Not visitaCentrale.DataRisoluzioneConflitto.HasValue Then

                If isRevocaVisibilitaCentrale OrElse
                   (GenericProviderCentrale.VisitaCentrale.CountVisitaCentraleInConflittoByIdConflitto(visitaCentrale.IdConflitto) = 2) Then

                    idConflittoRimanente = visitaCentrale.IdConflitto

                End If

                visitaCentrale.IdConflitto = Nothing

            End If

            If idConflittoRimanente.HasValue Then
                GenericProviderCentrale.VisitaCentrale.UpdateIdConflittoVisitaCentraleByIdConflitto(idConflittoRimanente.Value, Nothing)
            End If

        End If

        Select Case tipoVisitaCentrale

            Case Constants.TipoVisitaCentrale.Nessuno

                visitaCentrale.IdUtenteModificaVisita = visita.IdUtenteModifica
                visitaCentrale.DataModificaVisita = visita.DataModifica

                If isMerge Then
                    visitaCentrale.MergeInfoCentrale.CodicePazienteCentraleAlias = visitaCentrale.CodicePazienteCentrale
                End If

            Case Constants.TipoVisitaCentrale.Eliminata

                visitaCentrale.IdUtenteEliminazioneVisita = visita.IdUtenteEliminazione
                visitaCentrale.DataEliminazioneVisita = visita.DataEliminazione

                If isMerge Then
                    'TODO [ DATI VAC CENTRALE ]: IdUtenteAlias = IdUtenteUltima
                    visitaCentrale.MergeInfoCentrale.IdUtenteAlias = ContextInfos.IDUtente
                    visitaCentrale.MergeInfoCentrale.DataAlias = DateTime.Now
                End If

            Case Else
                Throw New NotImplementedException(String.Format("Tipo Visita Centrale non implementata: {0}", tipoVisitaCentrale))

        End Select

        visitaCentrale.FlagVisibilitaCentrale = visita.FlagVisibilitaDatiVaccinaliCentrale

        visitaCentrale.TipoVisitaCentrale = tipoVisitaCentrale

        visitaCentrale.IdUtenteUltimaOperazione = ContextInfos.IDUtente

        If existsVisitaCentrale Then
            GenericProviderCentrale.VisitaCentrale.UpdateVisitaCentrale(visitaCentrale)
        Else
            GenericProviderCentrale.VisitaCentrale.InsertVisitaCentrale(visitaCentrale)
        End If


        Dim visitaCentraleDistributa As VisitaCentrale.VisitaDistribuita =
            GenericProviderCentrale.VisitaCentrale.GetVisitaCentraleDistribuitaByIdCentrale(visitaCentrale.Id, UslGestitaCorrente.Codice)

        AggiornaVisitaCentraleDistributa(visita, visitaCentrale, visitaCentraleDistributa)

        Return visitaCentrale

    End Function

    Private Function AggiornaVisitaCentraleDistributa(visita As Visita, visitaCentrale As VisitaCentrale, visitaCentraleDistributa As VisitaCentrale.VisitaDistribuita) As VisitaCentrale.VisitaDistribuita

        ' TODO [Unificazione Ulss]: se arriva qui con il codice usl nuovo, scoppia

        If visitaCentraleDistributa Is Nothing Then

            visitaCentraleDistributa = New VisitaCentrale.VisitaDistribuita()
            visitaCentraleDistributa.CodiceUslVisita = UslGestitaCorrente.Codice

            visitaCentraleDistributa.IdVisita = visita.IdVisita
            visitaCentraleDistributa.CodicePaziente = visita.CodicePaziente

            visitaCentraleDistributa.DataInserimentoVisita = DateTime.Now
            visitaCentraleDistributa.IdUtenteInserimentoVisita = visitaCentrale.IdUtenteInserimentoVisita
            visitaCentraleDistributa.IdVisitaCentrale = visitaCentrale.Id

            GenericProviderCentrale.VisitaCentrale.InsertVisitaCentraleDistribuita(visitaCentraleDistributa)

        Else

            visitaCentraleDistributa.DataAggiornamentoVisita = DateTime.Now

            GenericProviderCentrale.VisitaCentrale.UpdateVisitaCentraleDistribuita(visitaCentraleDistributa)

        End If

        Return visitaCentraleDistributa

    End Function

    Friend Function AcquisisciVisitaCentrale(codicePazienteDestinazione As Int64, visitaAcquisizioneCentraleInfos As VisitaCentraleInfo(),
                                             tipoVisitaCentrale As String, isVisibilitaModificata As Boolean) As AcquisisciVisitaCentraleResult

        Return AcquisisciVisitaCentrale(codicePazienteDestinazione, visitaAcquisizioneCentraleInfos, tipoVisitaCentrale, isVisibilitaModificata, String.Empty)

    End Function

    Friend Function AcquisisciVisitaCentrale(codicePazienteDestinazione As Int64, visitaAcquisizioneCentraleInfos As VisitaCentraleInfo(),
                                             tipoVisitaCentrale As String, isVisibilitaModificata As Boolean, note As String) As AcquisisciVisitaCentraleResult

        ' TODO [Unificazione Ulss]: se arriva qui con il codice usl nuovo, scoppia

        Dim visiteInseriteList As New List(Of Visita)()
        Dim visiteModificateList As New List(Of Visita)()

        Dim visiteNonInseriteList As New List(Of Visita)()
        Dim visiteNonModificateList As New List(Of Visita)()

        For Each visitaAcquisizioneCentraleInfo As VisitaCentraleInfo In visitaAcquisizioneCentraleInfos

            Dim operazioneLogDatiVaccinaliCentrali As Log.DataLogStructure.Operazione? = Nothing
            Dim statoLogDatiVaccinaliCentrali As Enumerators.StatoLogDatiVaccinaliCentrali? = Nothing

            Dim visitaCentraleDistributaDestinazione As VisitaCentrale.VisitaDistribuita =
                GenericProviderCentrale.VisitaCentrale.GetVisitaCentraleDistribuitaByIdCentrale(visitaAcquisizioneCentraleInfo.VisitaCentrale.Id, UslGestitaCorrente.Codice)

            Dim visitaDestinazione As Visita = Nothing

            Dim visitaSuccess As Boolean = True

            Dim isVisibilitaCentrale As Boolean = Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(visitaAcquisizioneCentraleInfo.Visita.FlagVisibilitaDatiVaccinaliCentrale)

            If tipoVisitaCentrale <> Constants.TipoVisitaCentrale.Eliminata Then

                If isVisibilitaCentrale OrElse Not visitaCentraleDistributaDestinazione Is Nothing Then

                    ' VISITA AGGIUNTA / MODIFICATA / ELIMINATA
                    Select Case tipoVisitaCentrale

                        Case Constants.TipoVisitaCentrale.Nessuno

                            visitaDestinazione = CreateVisitaDestinazioneByOrigine(visitaAcquisizioneCentraleInfo.Visita, visitaAcquisizioneCentraleInfo.VisitaCentrale,
                                                                                   codicePazienteDestinazione)

                            If visitaCentraleDistributaDestinazione Is Nothing Then

                                visitaDestinazione.IdVisita = -1
                                visitaDestinazione.DataRegistrazione = Date.MinValue

                                Dim insertVisitaCommand As New Biz.BizVisite.InserisciVisitaCommand()
                                insertVisitaCommand.Visita = visitaDestinazione
                                insertVisitaCommand.UpdateVisitaCentraleInConflittoIfNeeded = True
                                insertVisitaCommand.VisitaCentrale = visitaAcquisizioneCentraleInfo.VisitaCentrale

                                insertVisitaCommand.Note = note
                                If String.IsNullOrWhiteSpace(insertVisitaCommand.Note) Then
                                    insertVisitaCommand.Note = "BizVisite.AcquisisciVisitaCentrale"
                                End If
                                insertVisitaCommand.Note += " - Inserimento visita per acquisizione da centrale"

                                Dim insertVisitaBizResult As BizVisite.InserisciVisitaResult = Me.InsertVisita(insertVisitaCommand)

                                If insertVisitaBizResult.Success Then
                                    visiteInseriteList.Add(insertVisitaCommand.Visita)
                                Else
                                    visiteNonInseriteList.Add(insertVisitaCommand.Visita)
                                    visitaSuccess = False
                                End If

                                operazioneLogDatiVaccinaliCentrali = DataLogStructure.Operazione.Inserimento

                            Else

                                visitaDestinazione.IdVisita = visitaCentraleDistributaDestinazione.IdVisita
                                visitaDestinazione.DataModifica = Date.MinValue

                                Dim updateVisitaCommand As New ModificaVisitaCommand()
                                updateVisitaCommand.Visita = visitaDestinazione
                                updateVisitaCommand.VisitaOriginale = GenericProvider.Visite.GetVisitaById(visitaCentraleDistributaDestinazione.IdVisita)
                                updateVisitaCommand.UpdateVisitaCentraleInConflittoIfNeeded = True
                                updateVisitaCommand.VisitaCentrale = visitaAcquisizioneCentraleInfo.VisitaCentrale
                                updateVisitaCommand.SovrascriviInfoModifica = False

                                updateVisitaCommand.Note = note
                                If String.IsNullOrWhiteSpace(updateVisitaCommand.Note) Then
                                    updateVisitaCommand.Note = "BizVisite.AcquisisciVisitaCentrale: "
                                End If
                                updateVisitaCommand.Note += "Update visita per acquisizione da centrale"

                                Dim updateVisitaBizResult As ModificaVisitaBizResult = UpdateVisita(updateVisitaCommand, False)

                                If updateVisitaBizResult.Success Then
                                    visiteModificateList.Add(updateVisitaCommand.Visita)
                                Else
                                    visiteNonModificateList.Add(updateVisitaCommand.Visita)
                                    visitaSuccess = False
                                End If

                                operazioneLogDatiVaccinaliCentrali = DataLogStructure.Operazione.Modifica

                            End If

                        Case Else
                            Throw New NotImplementedException()

                    End Select

                    If visitaSuccess Then

                        If isVisibilitaCentrale Then

                            ' OSSERVAZIONI AGGIUNTE / MODIFICATE
                            If Not visitaAcquisizioneCentraleInfo.Osservazioni Is Nothing Then
                                AcquisisciOsservazioniVisitaCentrale(visitaAcquisizioneCentraleInfo, visitaDestinazione)
                            End If

                        End If

                    End If

                End If

            Else

                If Not visitaCentraleDistributaDestinazione Is Nothing Then

                    visitaDestinazione = GenericProvider.Visite.GetVisitaByIdIfExists(visitaCentraleDistributaDestinazione.IdVisita)

                    If Not visitaDestinazione Is Nothing Then

                        ' OSSERVAZIONI ELIMINATE
                        If Not visitaAcquisizioneCentraleInfo.Osservazioni Is Nothing Then
                            AcquisisciOsservazioniVisitaCentrale(visitaAcquisizioneCentraleInfo, visitaDestinazione)
                        End If

                        DeleteVisita(visitaDestinazione)

                        operazioneLogDatiVaccinaliCentrali = DataLogStructure.Operazione.Eliminazione

                    End If
                End If

            End If

            If visitaSuccess Then
                statoLogDatiVaccinaliCentrali = Enumerators.StatoLogDatiVaccinaliCentrali.Success
            Else
                statoLogDatiVaccinaliCentrali = Enumerators.StatoLogDatiVaccinaliCentrali.Error
            End If

            If operazioneLogDatiVaccinaliCentrali.HasValue Then

                If statoLogDatiVaccinaliCentrali <> Enumerators.StatoLogDatiVaccinaliCentrali.Error Then
                    AggiornaVisitaCentraleDistributa(visitaDestinazione, visitaAcquisizioneCentraleInfo.VisitaCentrale, visitaCentraleDistributaDestinazione)
                End If

                InsertLogAcquisizioneVisitaCentraleDistribuita(visitaDestinazione, visitaAcquisizioneCentraleInfo.VisitaCentrale, operazioneLogDatiVaccinaliCentrali.Value,
                                                               statoLogDatiVaccinaliCentrali.Value, isVisibilitaModificata)
            End If

        Next

        Dim aggiornaVisitaCentraleDistribuitaResult As New AcquisisciVisitaCentraleResult()

        aggiornaVisitaCentraleDistribuitaResult.VisiteInserite = visiteInseriteList.ToArray()
        aggiornaVisitaCentraleDistribuitaResult.VisiteModificate = visiteModificateList.ToArray()
        aggiornaVisitaCentraleDistribuitaResult.VisiteNonInserite = visiteNonInseriteList.ToArray()
        aggiornaVisitaCentraleDistribuitaResult.VisiteNonModificate = visiteNonModificateList.ToArray()

        Return aggiornaVisitaCentraleDistribuitaResult

    End Function

    Friend Function AggiornaVisibilitaVisita(flagVisibilitaDatiVaccinaliCentrale As String, visita As Visita, visitaCentrale As VisitaCentrale, note As String) As ModificaVisitaBizResult

        Dim modificaVisitaBizResult As BizVisite.ModificaVisitaBizResult = Nothing

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions)

            Dim visitaOriginale As Entities.Visita = visita.Clone()

            visita.FlagVisibilitaDatiVaccinaliCentrale = flagVisibilitaDatiVaccinaliCentrale

            Dim modificaVisitaCommand As New BizVisite.ModificaVisitaCommand()
            modificaVisitaCommand.VisitaOriginale = visitaOriginale
            modificaVisitaCommand.Visita = visita
            modificaVisitaCommand.SovrascriviInfoModifica = False

            modificaVisitaCommand.Note = note
            If String.IsNullOrWhiteSpace(note) Then
                modificaVisitaCommand.Note = "Aggiornamento visibilita visita"
            End If

            modificaVisitaBizResult = Me.UpdateVisita(modificaVisitaCommand, False)

            '-- LOG UPDATE FLAG VISIBILITA VISITA
            Me.InsertLogAcquisizioneVisitaCentraleDistribuita(visita, visitaCentrale, DataLogStructure.Operazione.Modifica,
                                                              IIf(modificaVisitaBizResult.Success, Enumerators.StatoLogDatiVaccinaliCentrali.Success, Enumerators.StatoLogDatiVaccinaliCentrali.Error),
                                                              True)

            transactionScope.Complete()

        End Using

        Return modificaVisitaBizResult

    End Function

    Friend Sub InsertLogAcquisizioneVisitaCentraleDistribuita(visita As Visita, visitaCentrale As VisitaCentrale, operazione As DataLogStructure.Operazione, statoLogDatiVaccinaliCentrali As Enumerators.StatoLogDatiVaccinaliCentrali, isVisibilitaModificata As Boolean)

        Dim codiceArgomentoLog As String

        Dim noteStringBuilder As New Text.StringBuilder()

        noteStringBuilder.Append(visita.DataVisita.ToString("dd/MM/yyyy"))

        If Not String.IsNullOrEmpty(visita.MalattiaCodice) Then
            noteStringBuilder.AppendFormat(" - {0}", visita.MalattiaCodice)
        End If

        If visita.BilancioNumero > 0 Then

            codiceArgomentoLog = DataLogStructure.TipiArgomento.GEST_BIL

            noteStringBuilder.AppendFormat(" - {0}", visita.BilancioNumero)

        Else

            codiceArgomentoLog = DataLogStructure.TipiArgomento.VISITE

        End If

        If isVisibilitaModificata Then

            noteStringBuilder.AppendFormat(" [Nuovo valore consenso: {0}]",
                                           IIf(visita.FlagVisibilitaDatiVaccinaliCentrale = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente, "CONCESSO", "NEGATO"))

        End If

        Dim logDatiVaccinali As New LogDatiVaccinali()

        logDatiVaccinali.Paziente.Paz_Codice = visita.CodicePaziente
        logDatiVaccinali.Operazione = operazione
        logDatiVaccinali.Argomento.Codice = codiceArgomentoLog
        logDatiVaccinali.Stato = statoLogDatiVaccinaliCentrali
        logDatiVaccinali.Usl.Codice = visitaCentrale.CodiceUslVisita
        logDatiVaccinali.Utente.Id = visitaCentrale.IdUtenteUltimaOperazione
        logDatiVaccinali.DataOperazione = DateTime.Now
        logDatiVaccinali.Note = noteStringBuilder.ToString()

        Me.GenericProvider.Log.InsertLogDatiVaccinali(logDatiVaccinali)

    End Sub

    Friend Function GetCodiciUslDistribuiteVisite(idVisiteLocali As List(Of Int64), codiceUslInserimento As String) As Dictionary(Of KeyValuePair(Of Int64, String), String())

        Dim datiUslDistribuite As New Dictionary(Of KeyValuePair(Of Int64, String), String())()

        If idVisiteLocali Is Nothing OrElse idVisiteLocali.Count = 0 Then Return datiUslDistribuite

        Dim uslDistribuitaDatoVaccinaleInfoList As List(Of Entities.UslDistribuitaDatoVaccinaleInfo) =
            Me.GenericProviderCentrale.VisitaCentrale.GetUslDistribuiteVisite(idVisiteLocali, codiceUslInserimento)

        If Not uslDistribuitaDatoVaccinaleInfoList Is Nothing AndAlso uslDistribuitaDatoVaccinaleInfoList.Count > 0 Then

            For Each idLocale As Int64 In idVisiteLocali

                Dim id As Int64 = idLocale

                datiUslDistribuite.Add(New KeyValuePair(Of Int64, String)(idLocale, codiceUslInserimento),
                                       uslDistribuitaDatoVaccinaleInfoList.Where(Function(p) p.IdDatoVaccinale = id AndAlso p.CodiceUslInserimento = codiceUslInserimento).
                                                                           Select(Function(p) p.CodiceUslDistribuita).ToArray())

            Next

        End If

        Return datiUslDistribuite

    End Function

#End Region

#End Region

#Region " Private "

    Private Function CanSaveVisita(visita As Entities.Visita, visitaOriginale As Entities.Visita) As CanSaveVisitaResult
        'Private Function CanSaveVisita(visita As Entities.Visita, visitaOriginale As Entities.Visita, checkConflittoVisibilitaCentraleIfNeeded As Boolean) As CanSaveVisitaResult

        Dim canSaveVisitaResult As CanSaveVisitaResult = Me.CanSaveVisita(visita.CodicePaziente, visita.DataVisita, visita.MalattiaCodice, visita.BilancioNumero, visita.IdVisita)

        'If canSaveVisitaResult.Success Then

        '    If checkConflittoVisibilitaCentraleIfNeeded AndAlso
        '   Me.UslGestitaCorrente.FlagConsensoDatiVaccinaliCentralizzati Then

        '        Dim flagVisibilitaCorrente As String = visita.FlagVisibilitaDatiVaccinaliCentrale
        '        Dim flagVisibilitaOriginale As String = String.Empty

        '        If Not visitaOriginale Is Nothing Then
        '            flagVisibilitaOriginale = visitaOriginale.FlagVisibilitaDatiVaccinaliCentrale
        '        End If

        '        If Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(flagVisibilitaCorrente) AndAlso
        '            Not Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(flagVisibilitaOriginale) Then

        '            Dim codicePazienteAusiliario As String = Me.GenericProvider.Paziente.GetCodiceAusiliario(visita.CodicePaziente)

        '            For Each uslGestita As Entities.Usl In Me.UslGestite
        '                If uslGestita.Codice <> Me.UslGestitaCorrente.Codice Then

        '                    Using dbGenericProvider As DbGenericProvider = Me.GetDBGenericProviderByCodiceUslGestita(uslGestita.Codice)

        '                        Using bizVisite As Biz.BizVisite = Me.CreateBizVisiteByUslGestita(uslGestita.Codice, dbGenericProvider, Me.UslGestitaAllineaSettingsProvider.GetSettings(dbGenericProvider))

        '                            Dim codicePaziente As String = bizVisite.GenericProvider.Paziente.GetCodicePazientiByCodiceAusiliario(codicePazienteAusiliario).FirstOrDefault()

        '                            If Not String.IsNullOrEmpty(codicePaziente) Then

        '                                canSaveVisitaResult = bizVisite.CanSaveVisita(visita.CodicePaziente, visita.DataVisita, visita.MalattiaCodice, visita.BilancioNumero, Nothing)

        '                                If Not canSaveVisitaResult.Success Then Exit For

        '                            End If

        '                        End Using

        '                    End Using

        '                End If

        '            Next

        '        End If

        '    End If

        'End If

        Return canSaveVisitaResult

    End Function

    ''' <summary>
    ''' Restituisce true se è presente una visita già esistente (idVisitaEsistente) diversa da quella corrente (idVisita)
    ''' </summary>
    ''' <param name="idVisitaEsistente"></param>
    ''' <param name="idVisita"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ExistsVisita(idVisitaEsistente As Int64?, idVisita As Int64?) As Boolean

        If idVisitaEsistente.HasValue AndAlso (Not idVisita.HasValue OrElse idVisitaEsistente.Value <> idVisita.Value) Then
            Return True
        End If

        Return False

    End Function
    Public Function ExisistIdPadreFollowUp(idFollowUp As Integer) As Boolean

        Return GenericProvider.Visite.GetPadreFollowup(idFollowUp) > 0

    End Function

    Private Function SpostaConvocazioniVaccinazioniDopoFineSospensioneIfNeeded(codicePaziente As Int64, dataFineSospensione As DateTime, noteSpostamento As String) As BizConvocazione.SpostaConvocazioneBizResult

        If String.IsNullOrWhiteSpace(noteSpostamento) Then noteSpostamento = "BizVisite"
        noteSpostamento += ": Spostamento convocazione per sospensione"

        Dim spostaConvocazioneBizResult As BizConvocazione.SpostaConvocazioneBizResult

        Dim dateConvocazioni As IEnumerable(Of Date) = Me.GenericProvider.Convocazione.GetDateConvocazioniPaziente(codicePaziente, dataFineSospensione)
        If Not dateConvocazioni Is Nothing Then

            Using bizConvocazione As New BizConvocazione(Me.GenericProvider, Me.Settings, Me.ContextInfos, Me.LogOptions)

                Dim command As New BizConvocazione.SpostaConvocazioneCommand()
                command.CodicePaziente = codicePaziente
                command.DataConvocazioneNew = dataFineSospensione.AddDays(1)
                command.DurataAppuntamentoNew = Nothing
                command.DateConvocazioneOld = dateConvocazioni.ToArray()
                command.ControlloAssociabilita = True
                command.MantieniAppuntamento = False
                command.OperazioneAutomatica = False
                command.NoteSpostamentoAppuntamento = noteSpostamento

                spostaConvocazioneBizResult = bizConvocazione.SpostaConvocazione(command)

            End Using

        Else
            spostaConvocazioneBizResult = New BizConvocazione.SpostaConvocazioneBizResult()
        End If

        Return spostaConvocazioneBizResult

    End Function

    Private Sub SetVisitaAddedContextInfosIfNeeded(visita As Visita)

        If String.IsNullOrEmpty(visita.CodiceUslInserimento) Then
            visita.CodiceUslInserimento = GetCodiceUslByIdApplicazione(ContextInfos.IDApplicazione)
        End If

        If visita.DataRegistrazione = DateTime.MinValue Then
            visita.DataRegistrazione = DateTime.Now
        End If

        If visita.IdUtente <= 0 Then
            visita.IdUtente = ContextInfos.IDUtente
        End If

    End Sub

    Private Sub SetVisitaModifiedContextInfosIfNeeded(visita As Visita)

        If Not visita.DataModifica.HasValue Then
            visita.DataModifica = DateTime.Now
        End If

        If Not visita.IdUtenteModifica.HasValue Then
            visita.IdUtenteModifica = ContextInfos.IDUtente
        End If

    End Sub

    Private Sub SetVisitaDeletedContextInfosIfNeeded(visitaEliminata As Visita)

        If Not visitaEliminata.DataEliminazione.HasValue Then
            visitaEliminata.DataEliminazione = DateTime.Now
        End If

        If Not visitaEliminata.IdUtenteEliminazione.HasValue Then
            visitaEliminata.IdUtenteEliminazione = ContextInfos.IDUtente
        End If

    End Sub

    Private Sub SetOsservazioneAddedContextInfosIfNeeded(osservazione As Osservazione)

        If osservazione.DataRegistrazione = Date.MinValue Then
            osservazione.DataRegistrazione = DateTime.Now
        End If

        If osservazione.IdUtenteRegistrazione <= 0 Then
            osservazione.IdUtenteRegistrazione = Me.ContextInfos.IDUtente
        End If

    End Sub

    Private Sub SetOsservazioneModifiedContextInfosIfNeeded(osservazione As Osservazione)

        If Not osservazione.DataVariazione.HasValue Then
            osservazione.DataVariazione = DateTime.Now
        End If

        If Not osservazione.IdUtenteVariazione.HasValue Then
            osservazione.IdUtenteVariazione = Me.ContextInfos.IDUtente
        End If

    End Sub

    Private Sub SetOsservazioneDeletedContextInfosIfNeeded(osservazioneEliminata As Osservazione)

        If Not osservazioneEliminata.DataEliminazione.HasValue Then
            osservazioneEliminata.DataEliminazione = DateTime.Now
        End If

        If Not osservazioneEliminata.IdUtenteEliminazione.HasValue Then
            osservazioneEliminata.IdUtenteEliminazione = Me.ContextInfos.IDUtente
        End If

    End Sub

    Private Function GetTestataLogInserimentoVisita(visita As Visita) As Testata

        Dim bilNum As String = String.Empty

        If visita.BilancioNumero.HasValue Then
            bilNum = visita.BilancioNumero.Value.ToString()
        End If

        Dim recordLog As New Record()
        recordLog.Campi.Add(New Campo("VIS_ID", String.Empty, visita.IdVisita))
        recordLog.Campi.Add(New Campo("VIS_N_BILANCIO", String.Empty, bilNum))
        recordLog.Campi.Add(New Campo("VIS_CNS_CODICE", String.Empty, visita.CodiceConsultorio))
        recordLog.Campi.Add(New Campo("VIS_MAL_CODICE", String.Empty, visita.MalattiaCodice))
        recordLog.Campi.Add(New Campo("VIS_DATA_VISITA", String.Empty, visita.DataVisita.ToShortDateString()))
        recordLog.Campi.Add(New Campo("VIS_DATA_REGISTRAZIONE", String.Empty, visita.DataRegistrazione.ToShortDateString()))
        recordLog.Campi.Add(New Campo("VIS_OPE_CODICE", String.Empty, visita.MedicoCodice))

        If Not String.IsNullOrEmpty(visita.Vaccinabile) Then recordLog.Campi.Add(New Campo("VIS_VACCINABILE", String.Empty, visita.Vaccinabile))
        If Not String.IsNullOrEmpty(visita.Firma) Then recordLog.Campi.Add(New Campo("VIS_FIRMA", String.Empty, visita.Firma))

        If visita.DataFineSospensione > DateTime.MinValue Then recordLog.Campi.Add(New Campo("VIS_FINE_SOSPENSIONE", String.Empty, visita.DataFineSospensione))

        If visita.Peso > 0 Then recordLog.Campi.Add(New Campo("VIS_PESO", String.Empty, visita.Peso))
        If visita.Altezza > 0 Then recordLog.Campi.Add(New Campo("VIS_ALTEZZA", String.Empty, visita.Altezza))
        If visita.Cranio > 0 Then recordLog.Campi.Add(New Campo("VIS_CRANIO", String.Empty, visita.Cranio))

        If Not String.IsNullOrEmpty(visita.PercentilePeso) Then recordLog.Campi.Add(New Campo("VIS_PERCENTILE_PESO", String.Empty, visita.PercentilePeso))
        If Not String.IsNullOrEmpty(visita.PercentileAltezza) Then recordLog.Campi.Add(New Campo("VIS_PERCENTILE_ALTEZZA", String.Empty, visita.PercentileAltezza))
        If Not String.IsNullOrEmpty(visita.PercentileCranio) Then recordLog.Campi.Add(New Campo("VIS_PERCENTILE_CRANIO", String.Empty, visita.PercentileCranio))

        If Not String.IsNullOrEmpty(visita.FlagPatologia) Then recordLog.Campi.Add(New Campo("VIS_PATOLOGIA", String.Empty, visita.FlagPatologia))
        If Not String.IsNullOrEmpty(visita.MotivoSospensioneCodice) Then recordLog.Campi.Add(New Campo("VIS_MAL_CODICE", String.Empty, visita.MotivoSospensioneCodice))
        If Not String.IsNullOrEmpty(visita.Note) Then recordLog.Campi.Add(New Campo("VIS_NOTE", String.Empty, visita.Note))

        If Not String.IsNullOrEmpty(visita.CodiceUslInserimento) Then recordLog.Campi.Add(New Campo("VIS_USL_INSERIMENTO", String.Empty, visita.CodiceUslInserimento))
        If Not String.IsNullOrEmpty(visita.FlagVisibilitaDatiVaccinaliCentrale) Then recordLog.Campi.Add(New Campo("VIS_FLAG_VISIBILITA", String.Empty, visita.FlagVisibilitaDatiVaccinaliCentrale))

        If visita.IdDocumento.HasValue Then recordLog.Campi.Add(New Campo("VIS_DOC_ID_DOCUMENTO", String.Empty, visita.IdDocumento.Value.ToString()))
        If visita.DataFirmaDigitale.HasValue Then recordLog.Campi.Add(New Campo("VIS_DATA_FIRMA", String.Empty, visita.DataFirmaDigitale.Value))
        If visita.IdUtenteFirmaDigitale.HasValue Then recordLog.Campi.Add(New Campo("VIS_UTE_ID_FIRMA", String.Empty, visita.IdUtenteFirmaDigitale.Value.ToString()))
        If visita.DataArchiviazione.HasValue Then recordLog.Campi.Add(New Campo("VIS_DATA_ARCHIVIAZIONE", String.Empty, visita.DataArchiviazione.Value))
        If visita.IdUtenteArchiviazione.HasValue Then recordLog.Campi.Add(New Campo("VIS_UTE_ID_ARCHIVIAZIONE", String.Empty, visita.IdUtenteArchiviazione.Value.ToString()))

        If visita.DataInizioViaggio.HasValue Then recordLog.Campi.Add(New Campo("VIS_DATA_INIZIO_VIAGGIO", String.Empty, visita.DataInizioViaggio.Value.ToShortDateString()))
        If visita.DataFineViaggio.HasValue Then recordLog.Campi.Add(New Campo("VIS_DATA_FINE_VIAGGIO", String.Empty, visita.DataFineViaggio.Value.ToShortDateString()))
        If Not String.IsNullOrWhiteSpace(visita.PaeseViaggioCodice) Then recordLog.Campi.Add(New Campo("VIS_CIT_CODICE_PAESE_VIAGGIO", String.Empty, visita.PaeseViaggioCodice))

        If Not String.IsNullOrWhiteSpace(visita.RilevatoreCodice) Then recordLog.Campi.Add(New Campo("VIS_OPE_CODICE_RILEVATORE", String.Empty, visita.RilevatoreCodice))

        If Not String.IsNullOrWhiteSpace(visita.VaccinazioniBilancio) Then recordLog.Campi.Add(New Campo("VIS_VACCINAZIONI_BILANCIO", String.Empty, visita.VaccinazioniBilancio))

        Dim testataLog As New Testata(TipiArgomento.GEST_BIL, Operazione.Inserimento)
        testataLog.Records.Add(recordLog)

        Return testataLog

    End Function

    Private Function GetIndirizzoPaziente(domicilio As String, residenza As String) As String

        If Not String.IsNullOrEmpty(domicilio) Then
            Return domicilio
        End If

        Return residenza

    End Function

    Private Function GetEtaDescrizione(etaPazienteInGiorni As Integer) As String

        Dim eta As New Entities.Eta(etaPazienteInGiorni)

        Return String.Format("{0} aa {1} mm {2} gg", eta.Anni.ToString(), eta.Mesi.ToString(), eta.Giorni.ToString())

    End Function

    Private Sub DeleteBilanciVisita(codicePaziente As Integer, numeroBilancio As Long?, codiceMalattia As String)

        If numeroBilancio.HasValue Then

            ' Cancellazione bilanci eseguiti
            Using bilanciobiz As New Biz.BizBilancioProgrammato(Me.GenericProvider, Me.Settings, Me.ContextInfos, Me.LogOptions)

                ' Ricerca bilanci da cancellare
                Dim bilanci As Collection.BilancioProgrammatoCollection =
                    bilanciobiz.CercaBilancio(codicePaziente, numeroBilancio.Value, codiceMalattia)

                If Not bilanci Is Nothing Then
                    For Each bilancio As Entities.BilancioProgrammato In bilanci

                        If bilancio.Bil_stato = Constants.StatiBilancio.EXECUTED Then
                            bilanciobiz.CancellaBilancio(bilancio)
                        End If

                    Next
                End If

            End Using

        End If

    End Sub

#Region " Centrale "

    Private Function AcquisisciOsservazioniVisitaCentrale(visitaAcquisizioneCentraleInfo As VisitaCentraleInfo, visitaDestinazione As Visita) As Boolean

        Dim osservazioneAggiornata As Boolean = False

        Dim osservazioniDestinazione As Osservazione() = Me.GenericProvider.Visite.GetOsservazioniByVisita(visitaDestinazione.IdVisita)

        For Each osservazione As Osservazione In visitaAcquisizioneCentraleInfo.Osservazioni

            Dim codiceOsservazione As String = osservazione.OsservazioneCodice

            Dim osservazioneDestinazioneEsistente As Osservazione =
                osservazioniDestinazione.FirstOrDefault(Function(oss) oss.OsservazioneCodice = codiceOsservazione)

            Dim isVisibilitaCentrale As Boolean =
                Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(visitaAcquisizioneCentraleInfo.Visita.FlagVisibilitaDatiVaccinaliCentrale)

            If visitaAcquisizioneCentraleInfo.VisitaCentrale.TipoVisitaCentrale <> Constants.TipoVisitaCentrale.Eliminata Then

                Dim osservazioneDestinazione As Osservazione = osservazione.Clone()
                osservazioneDestinazione.CodicePaziente = visitaDestinazione.CodicePaziente
                osservazioneDestinazione.CodicePazienteOld = New Int64?(Me.GenericProvider.Paziente.GetCodicePazientiByCodiceAusiliario(visitaAcquisizioneCentraleInfo.VisitaCentrale.CodicePazienteCentrale).FirstOrDefault())
                osservazioneDestinazione.IdVisita = visitaDestinazione.IdVisita

                If osservazioneDestinazioneEsistente Is Nothing Then

                    osservazioneDestinazione.Id = -1
                    osservazioneDestinazione.DataRegistrazione = Nothing

                    Me.InsertOsservazione(osservazioneDestinazione)

                Else
                    osservazioneDestinazione.Id = osservazioneDestinazioneEsistente.Id
                    osservazioneDestinazione.DataVariazione = Nothing

                    Me.UpdateOsservazione(osservazioneDestinazione)

                End If

            Else

                osservazioneDestinazioneEsistente.IdUtenteEliminazione = osservazione.IdUtenteEliminazione

                Me.DeleteOsservazione(osservazioneDestinazioneEsistente)

            End If

            osservazioneAggiornata = True

        Next

    End Function

    Private Function CreateVisitaDestinazioneByOrigine(visitaOrigine As Visita, visitaCentrale As VisitaCentrale, codicePazienteDestinazione As Int64) As Visita

        Dim visita As Visita = visitaOrigine.Clone()
        visita.CodicePaziente = codicePazienteDestinazione

        If visitaCentrale.MergeInfoCentrale.CodicePazienteCentraleAlias Is Nothing Then

            visita.CodicePazienteAlias = Nothing

        Else

            Dim codicePaziente As String =
                Me.GenericProvider.Paziente.GetCodicePazientiByCodiceAusiliario(visitaCentrale.MergeInfoCentrale.CodicePazienteCentraleAlias).FirstOrDefault()

            If Not String.IsNullOrEmpty(codicePaziente) Then
                visita.CodicePazienteAlias = New Int64?(Convert.ToInt64(codicePaziente))
            End If

        End If

        Dim noteAcquisizioneStringBuilder As New System.Text.StringBuilder()

        If Not String.IsNullOrEmpty(visita.CodiceConsultorio) Then
            noteAcquisizioneStringBuilder.AppendFormat("Centro Vaccinale: {0}", visita.CodiceConsultorio)
        End If

        visita.NoteAcquisizioneDatiVaccinaliCentrale = noteAcquisizioneStringBuilder.ToString()

        visita.MedicoCodice = Nothing
        visita.CodiceConsultorio = Nothing

        Return visita

    End Function

    Private Sub UpdateConflittoVisitaCentraleIfNeeded(idVisitaEsistente As Int64, visitaCentraleInConflitto As VisitaCentrale)

        Dim visitaCentraleEsistente As VisitaCentrale = GenericProviderCentrale.VisitaCentrale.GetVisitaCentraleByIdLocale(idVisitaEsistente, UslGestitaCorrente.Codice)

        Dim conflittoAccettato As Boolean = False

        If visitaCentraleEsistente Is Nothing Then Return

        If visitaCentraleEsistente.TipoVisitaCentrale <> Constants.TipoVisitaCentrale.Eliminata Then
            If visitaCentraleEsistente.IdConflitto.HasValue Then
                If (visitaCentraleEsistente.DataRisoluzioneConflitto.HasValue) Then
                    conflittoAccettato = True
                End If
            Else
                conflittoAccettato = True
            End If
        End If

        If conflittoAccettato Then

            visitaCentraleEsistente.IdConflitto = visitaCentraleInConflitto.Id
            visitaCentraleEsistente.DataRisoluzioneConflitto = Nothing
            visitaCentraleEsistente.IdUtenteRisoluzioneConflitto = Nothing

            GenericProviderCentrale.VisitaCentrale.UpdateVisitaCentrale(visitaCentraleEsistente)

            visitaCentraleInConflitto.IdConflitto = visitaCentraleInConflitto.Id
            visitaCentraleInConflitto.DataRisoluzioneConflitto = Nothing
            visitaCentraleInConflitto.IdUtenteRisoluzioneConflitto = Nothing

            GenericProviderCentrale.VisitaCentrale.UpdateVisitaCentrale(visitaCentraleInConflitto)

        End If

    End Sub

#End Region

#End Region

#Region " Types "

#Region " Commands "

    Public Class InserisciVisitaCommand
        Inherits SalvaVisitaBaseCommand

        Public Property Osservazioni As Entities.Osservazione()
        Public Property ProgrammaBilancio As Boolean
        Public Property Note As String
        Public Property listaViaggi As List(Of ViaggioVisita)

    End Class

    Public Class ModificaVisitaCommand
        Inherits SalvaVisitaBaseCommand

        Public Property VisitaOriginale As Entities.Visita
        Public Property Osservazioni As Entities.Osservazione()
        Public Property Note As String

        ''' <summary>
        ''' Indica se sovrascrivere utente e data di modifica della visita
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SovrascriviInfoModifica As Boolean
        Public Property ListaViaggi As List(Of ViaggioVisita)

    End Class

    Public MustInherit Class SalvaVisitaBaseCommand

#Region " Public "

        Public Property Visita As Entities.Visita

#End Region

#Region " Friend "

        Friend Property UpdateVisitaCentraleInConflittoIfNeeded As Boolean
        Friend Property VisitaCentrale As Entities.VisitaCentrale

#End Region

    End Class

#End Region

#Region " Results "

#Region " Public "

    Public Class InserisciVisitaResult
        Inherits BizResult

        Public Sub New(success As Boolean, resultMessageEnumerable As IEnumerable(Of BizResult.ResultMessage))
            MyBase.New(success, resultMessageEnumerable)
        End Sub

    End Class

    Public Class ModificaVisitaBizResult
        Inherits BizResult

        Public Sub New(success As Boolean, resultMessageEnumerable As IEnumerable(Of BizResult.ResultMessage))
            MyBase.New(success, resultMessageEnumerable)
        End Sub

        Public Property ConvocazioniDeleted As Boolean
        Public Property OsservazioniLogRecords As DataLogStructure.Record()

    End Class

    Public Class EliminaVisitaAndOsservazioniResult
        Inherits BizResult

        Public Property OsservazioniEliminate As Entities.Osservazione()

    End Class

#Region " Centrale "

    Public Class AcquisisciVisitaCentraleResult

        Public Property VisiteInserite As Visita()
        Public Property VisiteModificate As Visita()

        Public Property VisiteNonInserite As Visita()
        Public Property VisiteNonModificate As Visita()

        Public ReadOnly Property VisiteAcquisite As Boolean
            Get
                Return (Me.VisiteNonInserite.Count = 0 AndAlso Me.VisiteNonModificate.Count = 0)
            End Get
        End Property

    End Class

    Friend Class VisitaCentraleInfo

        Public Property Visita As Visita
        Public Property Osservazioni As Osservazione()
        Public Property VisitaCentrale As VisitaCentrale

    End Class

#End Region

#End Region

#Region " Friend "

    Friend Class CanSaveVisitaResult
        Inherits BizResult

        Public ReadOnly IdVisitaEsistente As Int64?

        Public Sub New()
        End Sub

        Public Sub New(success As Boolean, idVisitaEsistente As Int64?, messageDescriptionEnumerable As IEnumerable(Of String))

            MyBase.New(success, messageDescriptionEnumerable, Nothing)

            Me.IdVisitaEsistente = idVisitaEsistente

        End Sub

    End Class

#End Region

#End Region

    Public Class Messages
        Public Const NO_MALATTIA_FOLLOW_UP As String = "Impossibile creare FollowUP: l''anamnesi non e'' di tipo FollowUp !"
        Public Const NO_ID_FOLLOW_UP_VALORIZZATO As String = "Anamnesi Follow up già creata!"
    End Class

#End Region

End Class
