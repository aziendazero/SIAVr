Imports System.Collections.ObjectModel
Imports System.Collections.Generic

Imports Onit.Shared.Manager.Apps

Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.Common.Utility


Public Class BizMovimentiEsterniCNS
    Inherits Biz.BizClass

#Region " Constructors "

    Public Sub New(ByRef genericprovider As DbGenericProvider, ByRef settings As Settings.Settings, contextInfos As BizContextInfos)

        MyBase.New(genericprovider, settings, contextInfos, Nothing)

    End Sub

#End Region

#Region " Public "

    <MovimentiCV()>
    Public Function AcquisisciMovimentoEsterno(paziente As Paziente, sovrascrivi As Boolean) As AcquisizioneMovimentoEsternoResult

        ' [Unificazione Ulss]: codice vecchio per ulss non unificate

        Dim codicePaziente As Integer = paziente.Paz_Codice
        Dim acquisizioneMovimentoEsternoResult As New AcquisizioneMovimentoEsternoResult()

        acquisizioneMovimentoEsternoResult.Paziente = paziente
        acquisizioneMovimentoEsternoResult.Paziente.StatoAcquisizioneImmigrazione = Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Nessuno

        ' Controllo del database di import
        Dim idApplicazioneNotifica As String
        Dim usl As Usl = Me.GenericProvider.Usl.GetUslGestitaByCodiceComune(acquisizioneMovimentoEsternoResult.Paziente.ComuneProvenienza_Codice)
        If usl Is Nothing Then
            acquisizioneMovimentoEsternoResult.DatabaseNotificaIrraggiungibile = True
            Return acquisizioneMovimentoEsternoResult
        Else
            If String.IsNullOrEmpty(usl.IDApplicazione) Then
                acquisizioneMovimentoEsternoResult.DatabaseNotificaIrraggiungibile = True
                Return acquisizioneMovimentoEsternoResult
            End If
            idApplicazioneNotifica = usl.IDApplicazione
        End If

        ' Il database di import è lo stesso del database di destinazione
        If idApplicazioneNotifica = Me.ContextInfos.IDApplicazione Then
            Return acquisizioneMovimentoEsternoResult
        End If

        Dim vaccinazioniEseguite As List(Of VaccinazioneEseguita) = Me.GenericProvider.VaccinazioniEseguite.GetVaccinazioniEseguitePaziente(codicePaziente)
        Dim vaccinazioniEseguiteScadute As New List(Of VaccinazioneEseguita)(Me.GenericProvider.VaccinazioniEseguite.GetVaccinazioniEseguiteScadutePaziente(codicePaziente))
        Dim reazioniAvverse As List(Of ReazioneAvversa) = Me.GenericProvider.VaccinazioniEseguite.GetReazioniAvversePaziente(codicePaziente)
        Dim reazioniAvverseScadute As List(Of ReazioneAvversa) = Me.GenericProvider.VaccinazioniEseguite.GetReazioniAvverseScadutePaziente(codicePaziente)
        Dim vaccinazioniEscluseCount As Int32 = Me.GenericProvider.VaccinazioniEscluse.CountVaccinazioniEsclusePaziente(codicePaziente)
        Dim visite As List(Of Visita) = Me.GenericProvider.Visite.GetVisitePaziente(codicePaziente)


        If Not sovrascrivi Then

            If visite.Count > 0 Then
                acquisizioneMovimentoEsternoResult.VisitePresenti = True
                acquisizioneMovimentoEsternoResult.DatiPresenti = True
            End If

            If vaccinazioniEscluseCount > 0 Then
                acquisizioneMovimentoEsternoResult.EsclusioniPresenti = True
                acquisizioneMovimentoEsternoResult.DatiPresenti = True
            End If

            If vaccinazioniEseguite.Count > 0 Then
                acquisizioneMovimentoEsternoResult.VaccinazioneEseguitePresenti = vaccinazioniEseguite.Count > 0
                acquisizioneMovimentoEsternoResult.DatiPresenti = True
            End If

            If vaccinazioniEseguiteScadute.Count > 0 Then
                acquisizioneMovimentoEsternoResult.VaccinazioneScadutePresenti = vaccinazioniEseguiteScadute.Count > 0
                acquisizioneMovimentoEsternoResult.DatiPresenti = True
            End If

            If reazioniAvverse.Count > 0 Then
                acquisizioneMovimentoEsternoResult.ReazioneAvversePresenti = reazioniAvverse.Count > 0
                acquisizioneMovimentoEsternoResult.DatiPresenti = True
            End If

            If reazioniAvverseScadute.Count > 0 Then
                acquisizioneMovimentoEsternoResult.ReazioneScadutePresenti = reazioniAvverseScadute.Count > 0
                acquisizioneMovimentoEsternoResult.DatiPresenti = True
            End If

        End If

        If Not acquisizioneMovimentoEsternoResult.DatiPresenti Then

            Dim codicePazienteNotifica As Integer?
            Using genericProviderNotifica As DbGenericProvider = Me.GetDBGenericProviderByIDApplicazione(idApplicazioneNotifica)

                Dim visiteNotifica As New List(Of Visita)()
                Dim osservazioniNotifica As New List(Of Osservazione)()
                Dim vaccinazioniEscluseNotifica As New List(Of VaccinazioneEsclusa)()
                Dim vaccinazioniEseguiteNotifica As New List(Of VaccinazioneEseguita)()
                Dim vaccinazioniEseguiteScaduteNotifica As New List(Of VaccinazioneEseguita)()
                Dim reazioniAvverseNotifica As New List(Of ReazioneAvversa)()
                Dim reazioniAvverseScaduteNotifica As New List(Of ReazioneAvversa)()

                Try
                    codicePazienteNotifica = genericProviderNotifica.Paziente.GetCodicePazientiByCodiceAusiliario(acquisizioneMovimentoEsternoResult.Paziente.CodiceAusiliario).FirstOrDefault()

                    If Not codicePazienteNotifica Is Nothing Then
                        visiteNotifica = genericProviderNotifica.Visite.GetVisitePaziente(codicePazienteNotifica)
                        osservazioniNotifica = genericProviderNotifica.Visite.GetOsservazioniPaziente(codicePazienteNotifica)
                        vaccinazioniEscluseNotifica = genericProviderNotifica.VaccinazioniEscluse.GetVaccinazioniEsclusePaziente(codicePazienteNotifica)
                        vaccinazioniEseguiteNotifica = genericProviderNotifica.VaccinazioniEseguite.GetVaccinazioniEseguitePaziente(codicePazienteNotifica)
                        vaccinazioniEseguiteScaduteNotifica = New List(Of VaccinazioneEseguita)(genericProviderNotifica.VaccinazioniEseguite.GetVaccinazioniEseguiteScadutePaziente(codicePazienteNotifica))
                        reazioniAvverseNotifica = genericProviderNotifica.VaccinazioniEseguite.GetReazioniAvversePaziente(codicePazienteNotifica)
                        reazioniAvverseScaduteNotifica = genericProviderNotifica.VaccinazioniEseguite.GetReazioniAvverseScadutePaziente(codicePazienteNotifica)
                    Else
                        acquisizioneMovimentoEsternoResult.Paziente.StatoAcquisizioneImmigrazione = Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Errore
                        acquisizioneMovimentoEsternoResult.PazienteInesistente = True
                    End If

                Catch dbNotificaException As System.Data.Common.DbException
                    ' db notifica non raggiungibile
                    acquisizioneMovimentoEsternoResult.Paziente.StatoAcquisizioneImmigrazione = Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Errore
                    acquisizioneMovimentoEsternoResult.DatabaseNotificaIrraggiungibile = True

                    EventLogHelper.EventLogWrite(dbNotificaException, Me.ContextInfos.IDApplicazione)

                End Try

                If acquisizioneMovimentoEsternoResult.Paziente.StatoAcquisizioneImmigrazione <> Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Errore Then
                    If vaccinazioniEseguiteNotifica.Count > 0 Then
                        Dim vaccinazioniEseguiteCorrispondenti As IEnumerable(Of VaccinazioneEseguita) = Me.VaccinazioneEseguita(vaccinazioniEseguiteScadute, vaccinazioniEseguiteNotifica)
                        If vaccinazioniEseguiteCorrispondenti.Count > 0 Then
                            acquisizioneMovimentoEsternoResult.Paziente.StatoAcquisizioneImmigrazione = Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Errore
                            acquisizioneMovimentoEsternoResult.VaccinazioneEseguiteScaduteSovrapposte = True
                        End If
                    End If
                End If

                If acquisizioneMovimentoEsternoResult.Paziente.StatoAcquisizioneImmigrazione <> Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Errore Then
                    If vaccinazioniEseguiteScaduteNotifica.Count > 0 Then
                        Dim vaccinazioniEseguiteCorrispondenti As IEnumerable(Of VaccinazioneEseguita) = Me.VaccinazioneEseguita(vaccinazioniEseguite, vaccinazioniEseguiteScaduteNotifica)
                        If vaccinazioniEseguiteCorrispondenti.Count > 0 Then
                            acquisizioneMovimentoEsternoResult.Paziente.StatoAcquisizioneImmigrazione = Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Errore
                            acquisizioneMovimentoEsternoResult.VaccinazioneEseguiteScaduteSovrapposte = True
                        End If
                    End If
                End If

                If acquisizioneMovimentoEsternoResult.Paziente.StatoAcquisizioneImmigrazione <> Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Errore Then

                    Using transactionScope As New System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions())

                        Try

                            ' visite
                            If visiteNotifica.Count > 0 Then

                                Using bizVisite As New BizVisite(Me.GenericProvider, Me.Settings, Me.ContextInfos, Me.LogOptions)

                                    For Each visita As Visita In visite
                                        If visita.FollowUpId.HasValue Then
                                            Dim visitaDaEliminareFollowUp As New Visita
                                            visitaDaEliminareFollowUp = bizVisite.GetVisitaById(visita.FollowUpId)
                                            bizVisite.DeleteVisitaAndOsservazioni(visitaDaEliminareFollowUp)
                                        End If
                                        bizVisite.DeleteVisitaAndOsservazioni(visita)
                                    Next

                                    For Each visitaNotifica As Visita In visiteNotifica

                                        Dim listOsservazioniVisita As List(Of Osservazione) = osservazioniNotifica.Where(Function(p) p.IdVisita = visitaNotifica.IdVisita).ToList()
                                        For Each oss As Osservazione In listOsservazioniVisita
                                            oss.Id = -1
                                            oss.CodicePazienteOld = Nothing
                                            oss.CodicePaziente = codicePaziente
                                        Next

                                        visitaNotifica.IdVisita = -1
                                        visitaNotifica.CodicePaziente = codicePaziente
                                        visitaNotifica.MedicoCodice = Nothing
                                        visitaNotifica.CodiceConsultorio = Nothing
                                        visitaNotifica.CodicePazienteAlias = Nothing

                                        Dim inserisciVisitaCommand As New BizVisite.InserisciVisitaCommand()
                                        inserisciVisitaCommand.Visita = visitaNotifica
                                        inserisciVisitaCommand.Osservazioni = listOsservazioniVisita.ToArray()
                                        inserisciVisitaCommand.Note = "Acquisizione visita da Movimenti"

                                        bizVisite.InsertVisita(inserisciVisitaCommand)

                                    Next

                                End Using

                            End If

                            ' vaccinazioniEscluse
                            If vaccinazioniEscluseNotifica.Count > 0 Then
                                Me.GenericProvider.VaccinazioniEscluse.DeleteVaccinazioniEscluse(codicePaziente)
                                For Each vaccinazioneEsclusaPazienteNotifica As VaccinazioneEsclusa In vaccinazioniEscluseNotifica
                                    vaccinazioneEsclusaPazienteNotifica.Id = -1
                                    vaccinazioneEsclusaPazienteNotifica.DataModifica = Nothing
                                    vaccinazioneEsclusaPazienteNotifica.DataRegistrazione = DateTime.Now
                                    vaccinazioneEsclusaPazienteNotifica.CodiceOperatore = Nothing
                                    vaccinazioneEsclusaPazienteNotifica.CodicePazientePrecedente = Nothing
                                    vaccinazioneEsclusaPazienteNotifica.IdUtenteRegistrazione = Me.ContextInfos.IDUtente
                                    vaccinazioneEsclusaPazienteNotifica.CodicePaziente = codicePaziente
                                    Me.GenericProvider.VaccinazioniEscluse.InserisciVaccinazioneEsclusa(vaccinazioneEsclusaPazienteNotifica)
                                Next
                            End If

                            ' vaccinazioniEseguite
                            If vaccinazioniEseguiteNotifica.Count > 0 Then

                                Dim vaccinazioniEseguiteCorrispondenti As IEnumerable(Of VaccinazioneEseguita) = Me.VaccinazioneEseguita(vaccinazioniEseguite, vaccinazioniEseguiteNotifica)
                                If (vaccinazioniEseguite.Count = vaccinazioniEseguiteCorrispondenti.Count) Then
                                    ' ogni vaccinazione già presente in locale corrisponde a una vaccinazione dell'azienda di notifica,
                                    ' tengo solo quelle che sono in più nell'azienda di notifica per aggiungerle in locale
                                    vaccinazioniEseguiteNotifica = Me.GetVaccinazioniEseguiteNonCorrispondenti(vaccinazioniEseguiteCorrispondenti, vaccinazioniEseguiteNotifica).ToList
                                    acquisizioneMovimentoEsternoResult.VaccinazioniReazioniEseguiteAcquisite = True

                                Else
                                    acquisizioneMovimentoEsternoResult.Paziente.StatoAcquisizioneImmigrazione = Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Avvertimento
                                End If

                            Else
                                acquisizioneMovimentoEsternoResult.VaccinazioniReazioniEseguiteAcquisite = True
                            End If

                            If acquisizioneMovimentoEsternoResult.VaccinazioniReazioniEseguiteAcquisite Then

                                For Each vaccinazioneNotifica As VaccinazioneEseguita In vaccinazioniEseguiteNotifica
                                    Me.CreateVaccinazioneEseguitaDestinazioneByOrigine(codicePaziente, vaccinazioneNotifica)
                                    Me.GenericProvider.VaccinazioniEseguite.InsertVaccinazioneEseguita(vaccinazioneNotifica)
                                Next

                                ' reazioniAvverse
                                reazioniAvverseNotifica = Me.GetReazioniAvverseNonCorrispondenti(reazioniAvverse, reazioniAvverseNotifica).ToList()

                                For Each rea As ReazioneAvversa In reazioniAvverseNotifica

                                    Dim vac As VaccinazioneEseguita = vaccinazioniEseguiteNotifica.Where(Function(p) _
                                        p.ves_data_effettuazione = rea.DataEffettuazione AndAlso
                                        p.ves_vac_codice = rea.CodiceVaccinazione AndAlso
                                        p.ves_n_richiamo = rea.NumeroRichiamo).FirstOrDefault()

                                    rea.CodiceLotto = Nothing
                                    rea.CodicePazientePrecedente = Nothing
                                    rea.IdVaccinazioneEseguita = vac.ves_id
                                    rea.IdReazioneAvversa = Nothing
                                    rea.CodicePaziente = vac.paz_codice
                                    Me.GenericProvider.VaccinazioniEseguite.InsertReazioneAvversa(rea)

                                Next

                            End If

                            ' vaccinazioniEseguiteScadute
                            If vaccinazioniEseguiteScaduteNotifica.Count > 0 Then

                                Dim vaccinazioniEseguiteScaduteCorrispondenti As IEnumerable(Of VaccinazioneEseguita) = Me.VaccinazioneEseguita(vaccinazioniEseguiteScadute, vaccinazioniEseguiteScaduteNotifica)

                                If (vaccinazioniEseguiteScadute.Count = vaccinazioniEseguiteScaduteCorrispondenti.Count) Then

                                    vaccinazioniEseguiteScaduteNotifica = Me.GetVaccinazioniEseguiteNonCorrispondenti(vaccinazioniEseguiteScaduteCorrispondenti, vaccinazioniEseguiteScaduteNotifica).ToList()
                                    acquisizioneMovimentoEsternoResult.VaccinazioniReazioniScaduteAcquisite = True

                                Else
                                    acquisizioneMovimentoEsternoResult.Paziente.StatoAcquisizioneImmigrazione = Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Avvertimento
                                End If

                            Else
                                acquisizioneMovimentoEsternoResult.VaccinazioniReazioniScaduteAcquisite = True
                            End If

                            If acquisizioneMovimentoEsternoResult.VaccinazioniReazioniScaduteAcquisite Then

                                For Each vaccinazioneNotifica As VaccinazioneEseguita In vaccinazioniEseguiteScaduteNotifica
                                    Me.CreateVaccinazioneEseguitaDestinazioneByOrigine(codicePaziente, vaccinazioneNotifica)
                                    Me.GenericProvider.VaccinazioniEseguite.InsertVaccinazioneEseguitaScaduta(vaccinazioneNotifica)
                                Next

                                ' reazioniAvverseScadute
                                reazioniAvverseScaduteNotifica = Me.GetReazioniAvverseNonCorrispondenti(reazioniAvverseScadute, reazioniAvverseScaduteNotifica).ToList()

                                For Each rea As ReazioneAvversa In reazioniAvverseScaduteNotifica

                                    Dim vac As VaccinazioneEseguita = vaccinazioniEseguiteScaduteNotifica.Where(Function(p) _
                                        p.ves_data_effettuazione = rea.DataEffettuazione AndAlso
                                        p.ves_vac_codice = rea.CodiceVaccinazione AndAlso
                                        p.ves_n_richiamo = rea.NumeroRichiamo).FirstOrDefault()

                                    rea.CodiceLotto = Nothing
                                    rea.CodicePazientePrecedente = Nothing
                                    rea.IdVaccinazioneEseguita = vac.ves_id
                                    rea.IdReazioneAvversa = Nothing
                                    rea.CodicePaziente = vac.paz_codice
                                    Me.GenericProvider.VaccinazioniEseguite.InsertReazioneAvversaScaduta(rea)

                                Next

                            End If

                            ' cancellazione convocazioni/bilanciProgrammati
                            If acquisizioneMovimentoEsternoResult.VaccinazioniReazioniEseguiteAcquisite Then

                                Using bizConvocazione As New BizConvocazione(Me.GenericProvider, Me.Settings, Me.ContextInfos, Nothing)

                                    Dim command As New BizConvocazione.EliminaConvocazioniSollecitiBilanciCommand()
                                    command.CodicePaziente = codicePaziente
                                    command.DataConvocazione = Nothing
                                    command.CancellaBilanciAssociati = True
                                    command.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.EliminazioneConvocazione
                                    command.DataEliminazione = DateTime.Now
                                    command.NoteEliminazione = "Eliminata convocazione per acquisizione da Movimenti"
                                    command.WriteLog = False

                                    bizConvocazione.EliminaConvocazioniSollecitiBilanci(command)

                                End Using

                                acquisizioneMovimentoEsternoResult.ProgrammazioneEliminata = True
                            End If

                            Dim pazienteMovimentoCNSNotifica As Paziente = genericProviderNotifica.Paziente.GetPazienti(codicePazienteNotifica, ContextInfos.CodiceUsl)(0)
                            If acquisizioneMovimentoEsternoResult.Paziente.StatoAcquisizioneImmigrazione = Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Nessuno Then
                                acquisizioneMovimentoEsternoResult.Paziente.StatoAcquisizioneImmigrazione = Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Acquisito
                            End If

                            Me.GenericProvider.Paziente.ModificaPaziente(acquisizioneMovimentoEsternoResult.Paziente)

                            transactionScope.Complete()

                        Catch exc As Exception

                            acquisizioneMovimentoEsternoResult.Paziente.StatoAcquisizioneImmigrazione = Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Errore
                            EventLogHelper.EventLogWrite(exc, Me.ContextInfos.IDApplicazione)

                        End Try

                    End Using

                End If

            End Using

        End If

        Return acquisizioneMovimentoEsternoResult

    End Function

    ''' <summary>
    ''' Restituisce una struttura contenente il conteggio dei risultati totali e il datatable con i risultati della ricerca 
    ''' in base ai filtri impostati, all'ordinamento specificato e ai dati relativi alla paginazione.
    ''' </summary>
    ''' <param name="filtriRicercaPazientiInIngresso"></param>
    ''' <param name="campoOrdinamento"></param>
    ''' <param name="versoOrdinamento"></param>
    ''' <param name="pageIndex"></param>
    ''' <param name="pageSize"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LoadPazientiInIngresso(filtriRicercaPazientiInIngresso As MovimentiCNSPazientiInIngressoFilter, campoOrdinamento As String, versoOrdinamento As String, pageIndex As Int32, pageSize As Int32) As LoadPazientiInIngressoResult

        Dim pagingOptions As New MovimentiCNSPagingOptions()
        pagingOptions.StartRecordIndex = pageIndex * pageSize
        pagingOptions.EndRecordIndex = pagingOptions.StartRecordIndex + pageSize

        Dim orderBy As String = Me.GetOrderByPazientiInIngresso(campoOrdinamento, versoOrdinamento)

        Dim dstMovimentiEsterni As DstMovimentiEsterni =
            Me.GenericProvider.MovimentiEsterniCNS.LoadPazientiInIngresso(filtriRicercaPazientiInIngresso, orderBy, pagingOptions)

        Dim countPazientiInIngresso As Int32 =
            Me.GenericProvider.MovimentiEsterniCNS.CountPazientiInIngresso(filtriRicercaPazientiInIngresso)

        Dim loadPazientiInIngressoResult As New LoadPazientiInIngressoResult()
        loadPazientiInIngressoResult.PazientiInIngressoDataTable = dstMovimentiEsterni.MovimentiEsterni
        loadPazientiInIngressoResult.CountPazientiInIngresso = countPazientiInIngresso

        Return loadPazientiInIngressoResult

    End Function

    ''' <summary>
    ''' Restituisce il dataset contenente i pazienti in ingresso, in base ai filtri impostati, ordinati in base al campo e al verso specificati.
    ''' </summary>
    ''' <param name="filtriRicercaPazientiInIngresso"></param>
    ''' <param name="campoOrdinamento"></param>
    ''' <param name="versoOrdinamento"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LoadPazientiInIngresso(filtriRicercaPazientiInIngresso As MovimentiCNSPazientiInIngressoFilter, campoOrdinamento As String, versoOrdinamento As String) As DstMovimentiEsterni

        Dim orderBy As String = Me.GetOrderByPazientiInIngresso(campoOrdinamento, versoOrdinamento)

        Return Me.GenericProvider.MovimentiEsterniCNS.LoadPazientiInIngresso(filtriRicercaPazientiInIngresso, orderBy)

    End Function

#Region " Types "

    Public Structure LoadPazientiInIngressoResult
        Public PazientiInIngressoDataTable As DstMovimentiEsterni.MovimentiEsterniDataTable
        Public CountPazientiInIngresso As Int32
    End Structure

    <MovimentiCV()>
    Public Structure AcquisizioneMovimentoEsternoResult
        Public Paziente As Entities.Paziente
        Public PazienteInesistente As Boolean
        Public DatabaseNotificaIrraggiungibile As Boolean
        Public ProgrammazioneEliminata As Boolean
        Public VaccinazioniReazioniEseguiteAcquisite As Boolean
        Public VaccinazioniReazioniScaduteAcquisite As Boolean
        Public NoteAnagrafichePresenti As Boolean
        Public MalattiePresenti As Boolean
        Public CategorieRischioPresenti As Boolean
        Public CicliVaccinaliPresenti As Boolean
        Public VaccinazioneEseguitePresenti As Boolean
        Public VaccinazioneScadutePresenti As Boolean
        Public ReazioneAvversePresenti As Boolean
        Public ReazioneScadutePresenti As Boolean
        Public VisitePresenti As Boolean
        Public EsclusioniPresenti As Boolean
        Public RifiutiPresenti As Boolean
        Public InadempienzePresenti As Boolean
        Public ProgrammazionePresente As Boolean
        Public DatiPresenti As Boolean
        Public VaccinazioneEseguiteScaduteSovrapposte As Boolean
    End Structure

#End Region

#End Region

#Region " Private "

    <MovimentiCV()>
    Private Function GetReazioniAvverseCorrispondenti(reazioniAvversePazienteMin As IEnumerable(Of ReazioneAvversa),
                                                      reazioniAvversePazienteMax As IEnumerable(Of ReazioneAvversa)) As IEnumerable(Of ReazioneAvversa)

        Return reazioniAvversePazienteMax.Intersect(reazioniAvversePazienteMin, New ReazioneAvversaEqualityComparer())

    End Function

    <MovimentiCV()>
    Private Function GetReazioniAvverseNonCorrispondenti(reazioniAvversePazienteMin As IEnumerable(Of ReazioneAvversa), reazioniAvversePazienteMax As IEnumerable(Of ReazioneAvversa)) As IEnumerable(Of ReazioneAvversa)

        Return reazioniAvversePazienteMax.Except(reazioniAvversePazienteMin, New ReazioneAvversaEqualityComparer())

    End Function

    <MovimentiCV()>
    Private Function VaccinazioneEseguita(vaccinazioniEseguitePazienteMin As IEnumerable(Of VaccinazioneEseguita), vaccinazioniEseguitePazienteMax As IEnumerable(Of VaccinazioneEseguita)) As IEnumerable(Of VaccinazioneEseguita)

        Return vaccinazioniEseguitePazienteMin.Intersect(vaccinazioniEseguitePazienteMax, New VaccinazioneEseguitaEqualityComparer())

    End Function

    <MovimentiCV()>
    Private Function GetVaccinazioniEseguiteNonCorrispondenti(vaccinazioniEseguitePazienteMin As IEnumerable(Of VaccinazioneEseguita), vaccinazioniEseguitePazienteMax As IEnumerable(Of VaccinazioneEseguita)) As IEnumerable(Of VaccinazioneEseguita)

        Return vaccinazioniEseguitePazienteMax.Except(vaccinazioniEseguitePazienteMin, New VaccinazioneEseguitaEqualityComparer())

    End Function

    Private Sub CreateVaccinazioneEseguitaDestinazioneByOrigine(codicePaziente As Integer, vaccinazione As VaccinazioneEseguita)

        Dim infoConsultorioLotto As New System.Text.StringBuilder()
        If Not String.IsNullOrEmpty(vaccinazione.ves_cns_codice) Then
            infoConsultorioLotto.AppendFormat("Centro Vaccinale: {0} / ", vaccinazione.ves_cns_codice)
            vaccinazione.ves_cns_codice = Nothing
        End If

        If Not String.IsNullOrEmpty(vaccinazione.ves_lot_codice) Then
            infoConsultorioLotto.AppendFormat("Lotto: {0} / ", vaccinazione.ves_lot_codice)
            vaccinazione.ves_lot_codice = Nothing
        End If

        ' Le proprietà ves_med_vaccinante_nome, amb_descrizione non vengono valorizzate dalla query (mancano i join)
        'If Not String.IsNullOrEmpty(vaccinazione.ope_nome) OrElse Not String.IsNullOrEmpty(vaccinazione.ves_ope_codice) Then
        '    infoConsultorioLotto.AppendFormat("Medico: {0} / ", vaccinazione.ope_nome)
        '    vaccinazione.ope_nome = Nothing
        'End If

        'If Not String.IsNullOrEmpty(vaccinazione.ves_med_vaccinante_nome) OrElse Not String.IsNullOrEmpty(vaccinazione.ves_med_vaccinante_codice) Then
        '    infoConsultorioLotto.AppendFormat("Vaccinatore: {0} / ", vaccinazione.ves_med_vaccinante_nome)
        '    vaccinazione.ves_med_vaccinante_nome = Nothing
        'End If

        If infoConsultorioLotto.Length > 0 Then
            infoConsultorioLotto.Remove(infoConsultorioLotto.Length - 3, 3)
            infoConsultorioLotto.Insert(0, "=> ")
        End If

        vaccinazione.ves_id = Nothing
        vaccinazione.paz_codice = codicePaziente
        vaccinazione.ves_stato = "R"
        vaccinazione.ves_amb_codice = Nothing
        vaccinazione.ves_luogo = Nothing
        vaccinazione.ves_flag_visibilita_vac_centrale = Nothing
        vaccinazione.ves_note_acquisizione_vac_centrale = Nothing
        vaccinazione.ves_ute_id = Me.ContextInfos.IDUtente
        vaccinazione.ves_data_registrazione = DateTime.Now

        ' TODO [Unificazione Ulss]: usl inserimento - qui è da valorizzare?
        vaccinazione.ves_usl_inserimento = Nothing

        vaccinazione.ves_ass_prog = Nothing
        vaccinazione.ves_ope_codice = Nothing
        vaccinazione.ves_med_vaccinante_codice = Nothing

        ' altri campi non sbiancati, vengono riportati nella nuova usl anche se non hanno significato. 
        ' non serve sbiancarli perchè la vaccinazione è con ves_stato = "R"
        'vaccinazione.ves_sii_codice = Nothing
        'vaccinazione.ves_vii_codice = Nothing
        'vaccinazione.ves_noc_codice = Nothing
        'vaccinazione.ves_cnv_data_primo_app = Nothing
        'vaccinazione.cnv_data = Nothing
        'vaccinazione.ves_in_campagna = Nothing
        'vaccinazione.ves_ope_in_ambulatorio = Nothing
        'vaccinazione.ves_sii_codice = Nothing

        Dim msg As String = String.Format("{0} [Notifica{1}]", vaccinazione.ves_note, infoConsultorioLotto.ToString())
        If msg.Length > 200 Then msg = msg.Substring(0, 199)
        vaccinazione.ves_note = msg

    End Sub

    Private Function LoadImmigrati(immigratiFilter As MovimentiCNSImmigratiFilter, pagingOptions As MovimentiCNSPagingOptions?) As DstMovimentiEsterni

        Return Me.GenericProvider.MovimentiEsterniCNS.LoadImmigrati(immigratiFilter, pagingOptions)

    End Function

    Private Function GetOrderByPazientiInIngresso(campoOrdinamento As String, versoOrdinamento As String) As String

        Dim orderBy As New System.Text.StringBuilder()
        Dim ordinamentoDefault As New System.Text.StringBuilder("paz_cognome, paz_nome, paz_data_nascita, paz_codice, ")

        If Not String.IsNullOrWhiteSpace(campoOrdinamento) Then

            If String.IsNullOrWhiteSpace(versoOrdinamento) Then versoOrdinamento = "ASC"

            If campoOrdinamento.Contains(",") Then

                Dim campi As String() = campoOrdinamento.Split(",")

                For i As Integer = 0 To campi.Length - 1
                    ordinamentoDefault.Replace(campi(i) + ", ", String.Empty)
                Next

                campi(0) = campi(0) + " " + versoOrdinamento

                orderBy.AppendFormat("{0}, ", String.Join(",", campi))

            Else

                ordinamentoDefault.Replace(campoOrdinamento + ", ", String.Empty)

                orderBy.AppendFormat("{0} {1}, ", campoOrdinamento, versoOrdinamento)

            End If

        End If

        ordinamentoDefault.Remove(ordinamentoDefault.Length - 2, 2)

        orderBy.Append(ordinamentoDefault.ToString())

        Return orderBy.ToString()

    End Function

    '<MovimentiCV()>
    'Private Function MapPaziente(pazienteMovimentoCNS As MovimentoCNS.Paziente) As Entities.Paziente

    '    Dim paziente As New Entities.Paziente()

    '    If pazienteMovimentoCNS.DataNascita.HasValue Then paziente.Data_Nascita = pazienteMovimentoCNS.DataNascita.Value
    '    If Not pazienteMovimentoCNS.MedicoBase Is Nothing Then paziente.MedicoBase_Codice = pazienteMovimentoCNS.MedicoBase.Codice
    '    If Not pazienteMovimentoCNS.Circoscrizione Is Nothing Then paziente.Circoscrizione_Codice = pazienteMovimentoCNS.Circoscrizione.Codice
    '    If Not pazienteMovimentoCNS.Circoscrizione2 Is Nothing Then paziente.Circoscrizione2_Codice = pazienteMovimentoCNS.Circoscrizione2.Codice
    '    If Not pazienteMovimentoCNS.ComuneResidenza Is Nothing Then paziente.ComuneResidenza_Codice = pazienteMovimentoCNS.ComuneResidenza.Codice
    '    If Not pazienteMovimentoCNS.ComuneDomicilio Is Nothing Then paziente.ComuneDomicilio_Codice = pazienteMovimentoCNS.ComuneDomicilio.Codice

    '    Return paziente

    'End Function

#Region " Types "

    <MovimentiCV()>
    Private Class VaccinazioneEseguitaEqualityComparer
        Implements IEqualityComparer(Of VaccinazioneEseguita)

        Public Overloads Function Equals(x As Entities.VaccinazioneEseguita, y As Entities.VaccinazioneEseguita) As Boolean Implements System.Collections.Generic.IEqualityComparer(Of Entities.VaccinazioneEseguita).Equals

            Dim result As Boolean
            Dim vaccinazioneEseguitaPaziente1 As VaccinazioneEseguita = DirectCast(x, VaccinazioneEseguita)
            Dim vaccinazioneEseguitaPaziente2 As VaccinazioneEseguita = DirectCast(y, VaccinazioneEseguita)

            If vaccinazioneEseguitaPaziente1.ves_vac_codice = vaccinazioneEseguitaPaziente2.ves_vac_codice AndAlso
               vaccinazioneEseguitaPaziente1.ves_data_effettuazione = vaccinazioneEseguitaPaziente2.ves_data_effettuazione AndAlso
               vaccinazioneEseguitaPaziente1.ves_n_richiamo = vaccinazioneEseguitaPaziente2.ves_n_richiamo Then

                If vaccinazioneEseguitaPaziente1.ves_ass_codice = vaccinazioneEseguitaPaziente2.ves_ass_codice Then
                    result = True
                Else
                    result = False
                End If

            End If

            Return result

        End Function

        <MovimentiCV()>
        Public Overloads Function GetHashCode(obj As Entities.VaccinazioneEseguita) As Integer Implements System.Collections.Generic.IEqualityComparer(Of Entities.VaccinazioneEseguita).GetHashCode

            Return obj.ToString().ToLower().GetHashCode()

        End Function

    End Class

    <MovimentiCV()>
    Private Class ReazioneAvversaEqualityComparer
        Implements IEqualityComparer(Of ReazioneAvversa)

        Public Overloads Function Equals(x As Entities.ReazioneAvversa, y As Entities.ReazioneAvversa) As Boolean Implements System.Collections.Generic.IEqualityComparer(Of Entities.ReazioneAvversa).Equals

            Dim reazioneAvversaPaziente1 As ReazioneAvversa = DirectCast(x, ReazioneAvversa)
            Dim reazioneAvversaPaziente2 As ReazioneAvversa = DirectCast(y, ReazioneAvversa)

            Return reazioneAvversaPaziente1.CodiceVaccinazione = reazioneAvversaPaziente2.CodiceVaccinazione _
                AndAlso reazioneAvversaPaziente1.DataReazione = reazioneAvversaPaziente2.DataReazione _
                AndAlso reazioneAvversaPaziente1.DataEffettuazione = reazioneAvversaPaziente2.DataEffettuazione _
                AndAlso reazioneAvversaPaziente1.NumeroRichiamo = reazioneAvversaPaziente2.NumeroRichiamo


        End Function

        Public Overloads Function GetHashCode(obj As Entities.ReazioneAvversa) As Integer Implements System.Collections.Generic.IEqualityComparer(Of Entities.ReazioneAvversa).GetHashCode

            Return obj.ToString().ToLower().GetHashCode()

        End Function

    End Class

#End Region

#End Region

End Class

