Imports Onit.OnAssistnet.OnVac.DAL
Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.Log

Public Class BizCancellazioneConvocazioni
    Inherits Biz.BizClass

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos)
        MyBase.New(genericprovider, settings, contextInfos, Nothing)
    End Sub

#End Region

#Region " Constants "

    Public Const ID_PROCESSO_CANCELLAZIONE_CONVOCAZIONI As Integer = 17
    Public Const DESCRIZIONE_PROCESSO_CANCELLAZIONE_CONVOCAZIONI As String = "Processo di cancellazione programmazione"

#End Region

#Region " Types "

    Public Class ParametriGetConvocazioniBiz

        Public Filtri As Entities.FiltriConvocazioneDaCancellare
        Public CampoOrdinamento As String
        Public VersoOrdinamento As String
        Public PageIndex As Int32
        Public PageSize As Int32

    End Class

    <Serializable>
    Public Structure GetConvocazioniResult
        Public Convocazioni As List(Of Entities.ConvocazioneDaCancellare)
        Public CountConvocazioni As Int32
    End Structure

    <Serializable>
    Public Class EliminaConvocazioniCmd
        Public IdJob As Long
        Public FiltriProgrammazione As List(Of Entities.FiltroProgrammazione)
        Public TipoFiltri As Entities.TipoFiltriProgrammazione?
        Public CancellaInteraConvocazione As Boolean
        Public CancellaConvocazioneConSolleciti As Boolean
        Public CancellaConvocazioneConAppuntamenti As Boolean
    End Class

    Public Class BatchCancellazioneConvocazioni

        Public Class ParameterName
            Public Const IdApplicazioneLocale As String = "IdApplicazioneLocale"
            Public Const IdUtenteEliminazione As String = "IdUtenteEliminazione"
            Public Const FiltriProgrammazione As String = "FiltriProgrammazione"
            Public Const TipoFiltri As String = "TipoFiltri"
            Public Const CancellaInteraConvocazione As String = "CancellaInteraConvocazione"
            Public Const CancellaConSolleciti As String = "CancellaConSolleciti"
            Public Const CancellaConAppuntamenti As String = "CancellaConAppuntamenti"
            Public Const CodiceUslCorrente As String = "CodiceUslCorrente"
        End Class

    End Class

    Public Class StartBatchCancellazioneConvocazioniCommand

        Public FiltriProgrammazione As List(Of Entities.FiltroProgrammazione)
        Public TipoFiltri As Entities.TipoFiltriProgrammazione?
        Public CancellaInteraConvocazione As Boolean
        Public CancellaConSolleciti As Boolean
        Public CancellaConAppuntamenti As Boolean
        Public PazientiConvocazioniDaElaborare As List(Of Entities.ConvocazionePK)
        Public DatiReport As DatiReportCancellazioneConvocazioni

        Public Class DatiReportCancellazioneConvocazioni
            Public CodiceCentroVaccinale As String
            Public DescrizioneCentroVaccinale As String
            Public DataNascitaDa As DateTime?
            Public DataNascitaA As DateTime?
            Public Sesso As String
            Public DescrizioneMalattia As String
            Public DescrizioneCategoriaRischio As String
            Public DescrizioneStatiAnagrafici As String
            Public DataConvocazioneDa As DateTime?
            Public DataConvocazioneA As DateTime?
        End Class

        Public Sub New()
            Me.DatiReport = New DatiReportCancellazioneConvocazioni()
        End Sub

    End Class

#End Region

#Region " Public "

#Region " Ricerca "

    Public Function GetConvocazioniPerUtilityCancellazione(param As ParametriGetConvocazioniBiz) As GetConvocazioniResult

        If param Is Nothing Then Throw New ArgumentNullException()

        Dim paramToPass As New ParametriGetCnvPerCancellazione()
        paramToPass.Filtri = param.Filtri
        paramToPass.PagingOpts = New Data.PagingOptions()
        paramToPass.PagingOpts.StartRecordIndex = param.PageIndex * param.PageSize
        paramToPass.PagingOpts.EndRecordIndex = paramToPass.PagingOpts.StartRecordIndex + param.PageSize

        paramToPass.OrderBy = Me.GetOrderByConvocazioni(param.CampoOrdinamento, param.VersoOrdinamento)

        'Recupero gli elementi filtrati
        Dim lstConvocazioni As List(Of Entities.ConvocazioneDaCancellare) =
            Me.GenericProvider.CancellazioneConvocazioni.GetConvocazioniPerUtilityCancellazione(paramToPass)

        'Recupero il count degli elementi
        Dim countDocumentiVisite As Int32 =
            Me.GenericProvider.CancellazioneConvocazioni.CountConvocazioniPerUtilityCancellazione(param.Filtri)

        Dim result As New GetConvocazioniResult()
        result.Convocazioni = lstConvocazioni
        result.CountConvocazioni = countDocumentiVisite

        Return result

    End Function

    ''' <summary>
    ''' Restituisce le pk delle convocazioni in base ai filtri specificati
    ''' </summary>
    Public Function GetIdConvocazioniPerUtilityCancellazione(filtri As Entities.FiltriConvocazioneDaCancellare) As List(Of Entities.ConvocazionePK)

        ' Filtri obbligatori non impostati => restituisco una lista vuota
        If filtri Is Nothing OrElse String.IsNullOrWhiteSpace(filtri.CodiceCentroVaccinale) Then
            Return New List(Of Entities.ConvocazionePK)()
        End If

        Return Me.GenericProvider.CancellazioneConvocazioni.GetIdConvocazioniPerUtilityCancellazione(filtri)

    End Function

#End Region

#Region " Eliminazione convocazioni "

    Public Event RefreshTotaleElementiDaElaborare(e As BizBatch.RefreshTotaleElementiDaElaborareEventArgs)
    Public Event RefreshParzialeElementiElaborati(e As BizBatch.RefreshParzialeElementiElaboratiEventArgs)

    ''' <summary>
    ''' Metodo di eliminazione massiva convocazioni
    ''' </summary>
    ''' <param name="command"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function EliminaConvocazioni(command As EliminaConvocazioniCmd) As BizGenericResult

        If command Is Nothing Then Throw New ArgumentNullException()

        Dim numeroElaborazioni As Integer = 0
        Dim numeroErrori As Integer = 0

        Try
            ' Recupero le primary key dalla T_PAZ_ELABORAZIONI
            Dim lstCnvToDel As List(Of Entities.ConvocazionePK) =
                Me.GenericProvider.CancellazioneConvocazioni.GetConvocazioniDaEliminareByJobId(command.IdJob)

            ' Totale degli elementi da elaborare
            RaiseEvent RefreshTotaleElementiDaElaborare(New Biz.BizBatch.RefreshTotaleElementiDaElaborareEventArgs(lstCnvToDel.Count))

            Using bizCnv As New Biz.BizConvocazione(Me.GenericProvider, Me.Settings, Me.ContextInfos, Me.LogOptions)

                '  Filtra ulteriormente le cnv in base ai filtri con appuntamenti/con solleciti 
                If Not command.CancellaConvocazioneConAppuntamenti OrElse Not command.CancellaConvocazioneConSolleciti Then

                    Dim paramFiltra As New ParametriFiltraCnvDaEliminare()
                    paramFiltra.CnvKeys = lstCnvToDel
                    paramFiltra.EscludiCnvConAppuntamenti = Not command.CancellaConvocazioneConAppuntamenti
                    paramFiltra.EscludiCnvConSolleciti = Not command.CancellaConvocazioneConSolleciti

                    Dim cnvFiltrate As List(Of Entities.ConvocazionePK) =
                        Me.GenericProvider.CancellazioneConvocazioni.FiltraConvocazioniDaEliminare(paramFiltra)

                    ' Convocazioni che non verranno eliminate perchè aventi appuntamento o sollecito
                    Dim cnvNonEliminate As List(Of Entities.ConvocazionePK) = Nothing

                    If cnvFiltrate Is Nothing OrElse cnvFiltrate.Count = 0 Then
                        ' Tutte le cnv hanno appuntamento/sollecito
                        cnvNonEliminate = lstCnvToDel
                    Else
                        cnvNonEliminate = lstCnvToDel.Where(Function(p) Not cnvFiltrate.Any(Function(q) q.IdPaziente = p.IdPaziente And q.Data = p.Data)).ToList()
                    End If

                    If Not cnvNonEliminate Is Nothing AndAlso cnvNonEliminate.Count > 0 Then

                        ' Le cnv non eliminate sono considerate elaborate con errore
                        numeroElaborazioni = cnvNonEliminate.Count
                        numeroErrori = cnvNonEliminate.Count

                        RaiseEvent RefreshParzialeElementiElaborati(New Biz.BizBatch.RefreshParzialeElementiElaboratiEventArgs(numeroElaborazioni, numeroErrori))

                        ' Recupero dati cnv non eliminate dalla t_paz_elaborazioni
                        Dim listElaborazioniNonEliminate As List(Of JobItem) =
                            Me.GenericProvider.CancellazioneConvocazioni.GetElaborazioniJobCorrente(command.IdJob, cnvNonEliminate)

                        Dim bizBatch As New BizBatch(Me.GenericProvider)
                        For Each cnv As JobItem In listElaborazioniNonEliminate

                            Dim updateCommand As New BizBatch.UpdatePazienteElaborazioneCommand()
                            updateCommand.CodiceErrore = 41
                            updateCommand.DescrizioneErrore = "Programmazione non eliminata: appuntamento-sollecito associato alla convocazione."
                            updateCommand.Progressivo = cnv.Progressivo
                            updateCommand.StatoElaborazione = Constants.StatoElaborazioneBatch.Errore

                            bizBatch.UpdatePazienteElaborazione(updateCommand)

                        Next

                        ' Convocazioni da eliminare: solo quelle filtrate
                        lstCnvToDel = cnvFiltrate

                    End If

                End If

                If Not lstCnvToDel Is Nothing AndAlso lstCnvToDel.Count > 0 Then

                    Dim listLogVaccProgEliminate As New List(Of Entities.VaccinazioneProgrammata)()
                    Dim listLogCicliEliminati As New List(Of Entities.CicloConvocazione)()

                    'Le cnv devono essere cancellate completamente nel caso in cui non vengano 
                    'passati i filtri programmazione o ci sia il filtro CancellaInteraConvocazione a true
                    If command.CancellaInteraConvocazione OrElse command.FiltriProgrammazione Is Nothing OrElse command.FiltriProgrammazione.Count = 0 Then

                        For Each cnv As Entities.ConvocazionePK In lstCnvToDel

                            ' Eliminazione delle vaccinazioni programmate della convocazione + inserimento dati cancellati in lista per log
                            Dim vaccLog As List(Of Entities.VaccinazioneProgrammata) =
                                Me.GenericProvider.VaccinazioneProg.GetVaccinazioniProgrammateByCnv(cnv.IdPaziente, cnv.Data)

                            If Not vaccLog Is Nothing AndAlso vaccLog.Count > 0 Then
                                Me.GenericProvider.CancellazioneConvocazioni.EliminaVaccinazioniProgrammate(cnv.IdPaziente, cnv.Data, Nothing)
                                listLogVaccProgEliminate.AddRange(vaccLog)
                            End If

                            'Eliminazione dei cicli della convocazione + inserimento dati cancellati in lista per log
                            Dim cicliLog As List(Of Entities.CicloConvocazione) = Me.GenericProvider.Cicli.GetCicliConvocazione(cnv.IdPaziente, cnv.Data)

                            If Not cicliLog Is Nothing AndAlso cicliLog.Count > 0 Then
                                Me.GenericProvider.Convocazione.EliminaCicliConvocazione(cnv.IdPaziente, cnv.Data)
                                listLogCicliEliminati.AddRange(cicliLog)
                            End If

                            'NOTA: Eliminazione convocazione sotto

                        Next

                    Else

                        'Recupero le vaccinazione programmate
                        Dim paramGetVac As New ParametriVacProgDaEliminare()
                        paramGetVac.CnvKeys = lstCnvToDel
                        paramGetVac.FiltriProgrammazione = command.FiltriProgrammazione
                        paramGetVac.TipoFiltri = command.TipoFiltri

                        Dim count As Integer = 0

                        Dim lstVac As List(Of Entities.VaccinazioneProgrammata) =
                            Me.GenericProvider.CancellazioneConvocazioni.GetVaccinazioniProgrammateDaEliminare(paramGetVac)

                        For Each vac As Entities.VaccinazioneProgrammata In lstVac

                            'Eliminazione delle vaccinazioni programmate + inserimento dati cancellati in lista per log
                            count = Me.GenericProvider.CancellazioneConvocazioni.EliminaVaccinazioniProgrammate(vac.CodicePaziente, vac.DataConvocazione, vac.CodiceVaccinazione)

                            If count > 0 Then
                                listLogVaccProgEliminate.Add(vac.Clone())
                            End If

                            ' Eliminazione ciclo => se ciclo-seduta valorizzati (in caso di cnv in campagna non succede!)
                            If Not String.IsNullOrWhiteSpace(vac.CodiceCiclo) AndAlso vac.NumeroSeduta.HasValue Then

                                'Eliminazione dei cicli + inserimento dati cancellati in lista per log
                                Dim ciclo As Entities.CicloConvocazione =
                                    Me.GenericProvider.Cicli.GetCicloConvocazione(vac.CodicePaziente, vac.DataConvocazione, vac.CodiceCiclo, vac.NumeroSeduta)

                                count = Me.GenericProvider.CancellazioneConvocazioni.EliminaCicliEmpty(vac.CodicePaziente, vac.DataConvocazione, vac.CodiceCiclo, vac.NumeroSeduta)

                                If count > 0 Then
                                    listLogCicliEliminati.Add(ciclo.Clone())
                                End If

                            End If

                            'NOTA: Eliminazione convocazione sotto perchè stiamo ciclando le vac programmate

                        Next

                    End If

                    ' Recupero dati dalla t_paz_elaborazioni (mi servono i progressivi)
                    Dim listElaborazioni As List(Of JobItem) =
                        Me.GenericProvider.CancellazioneConvocazioni.GetElaborazioniJobCorrente(command.IdJob, lstCnvToDel)

                    Dim refreshStatusAfter As Integer = (listElaborazioni.Count * 20 \ 100) + 1

                    Dim now As DateTime = DateTime.Now

                    For Each cnv As JobItem In listElaborazioni

                        Dim codicePazienteCorrente As Long = Convert.ToInt64(cnv.CodicePaziente)

                        ' Caricamento dati convocazione da eliminare per log
                        Dim cnvEliminata As Entities.Convocazione =
                            Me.GenericProvider.Convocazione.GetConvocazionePaziente(codicePazienteCorrente, cnv.DataConvocazione)

                        'Eliminazione delle convocazioni (se vuote) e update risultato
                        Dim updateCommand As New BizBatch.UpdatePazienteElaborazioneCommand()
                        updateCommand.Progressivo = cnv.Progressivo

                        Dim eliminazioneCommand As New BizConvocazione.EliminaConvocazioneEmptyCommand()
                        eliminazioneCommand.CodicePaziente = codicePazienteCorrente
                        eliminazioneCommand.DataConvocazione = cnv.DataConvocazione
                        eliminazioneCommand.DataEliminazione = now
                        eliminazioneCommand.NoteEliminazione = "Eliminazione convocazione da utility Cancellazione Programmazione"
                        eliminazioneCommand.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.EliminazioneConvocazione
                        eliminazioneCommand.WriteLog = False

                        Dim eliminazioneResult As BizConvocazione.EliminaConvocazioneEmptyResult = bizCnv.EliminaConvocazioneEmpty(eliminazioneCommand)

                        numeroElaborazioni += 1

                        Select Case eliminazioneResult.ResultType

                            Case BizConvocazione.EliminaConvocazioneEmptyResultType.Success
                                '--
                                ' Convocazione eliminata
                                '--
                                updateCommand.StatoElaborazione = Constants.StatoElaborazioneBatch.TerminataCorrettamente
                                updateCommand.CodiceErrore = -1
                                updateCommand.DescrizioneErrore = String.Empty

                            Case BizConvocazione.EliminaConvocazioneEmptyResultType.CnvNonEliminata
                                '--
                                ' Convocazione non eliminata perchè ci sono dati vaccinali collegati (programmate/cicli/eseguite/escluse/bilanci)
                                '--
                                If command.CancellaInteraConvocazione Then
                                    '--
                                    ' Errore: l'elaborazione prevede la cancellazione dell'intera convocazione, che non è avvenuta
                                    '--
                                    numeroErrori += 1

                                    updateCommand.StatoElaborazione = Constants.StatoElaborazioneBatch.Errore
                                    updateCommand.CodiceErrore = 40
                                    updateCommand.DescrizioneErrore = "Non è stato possibile eliminare l'intera convocazione perchè ha dati associati."

                                ElseIf Not command.FiltriProgrammazione Is Nothing AndAlso command.FiltriProgrammazione.Count > 0 Then
                                    '--
                                    ' Ok: l'elaborazione non prevede la cancellazione dell'intera convocazione 
                                    '     e sono impostati filtri che non hanno permesso di cancellare tutti i dati associati alla convocazione
                                    '--
                                    updateCommand.StatoElaborazione = Constants.StatoElaborazioneBatch.TerminataCorrettamente
                                    updateCommand.CodiceErrore = -1
                                    updateCommand.DescrizioneErrore = String.Empty

                                Else
                                    '--
                                    ' Errore: l'elaborazione non prevede la cancellazione dell'intera convocazione, 
                                    '         ma non sono impostati filtri sulla programmazione, quindi la convocazione avrebbe dovuto essere cancellata 
                                    '         (non è considerato il caso dei bilanci programmati perchè in Veneto non sono previsti)
                                    '--
                                    numeroErrori += 1

                                    updateCommand.StatoElaborazione = Constants.StatoElaborazioneBatch.Errore
                                    updateCommand.CodiceErrore = 40
                                    updateCommand.DescrizioneErrore = "Programmazione non completamente eliminata: la convocazione ha dati associati."

                                End If

                            Case BizConvocazione.EliminaConvocazioneEmptyResultType.Exception
                                '--
                                ' Convocazione non eliminata perchè è stata sollevata un'eccezione
                                '--
                                numeroErrori += 1

                                updateCommand.StatoElaborazione = Constants.StatoElaborazioneBatch.Errore
                                updateCommand.CodiceErrore = 0
                                updateCommand.DescrizioneErrore = eliminazioneResult.Message

                        End Select

                        ' --- Update elaborati (t_paz_elaborazioni) --- '
                        Dim bizBatch As New BizBatch(Me.GenericProvider)
                        bizBatch.UpdatePazienteElaborazione(updateCommand)

                        ' --- Update dati di riepilogo del processo (t_processi) --- '
                        If (listElaborazioni.IndexOf(cnv) + 1) Mod refreshStatusAfter = 0 Then
                            RaiseEvent RefreshParzialeElementiElaborati(New Biz.BizBatch.RefreshParzialeElementiElaboratiEventArgs(numeroElaborazioni, numeroErrori))
                        End If

                        ' --- Log --- '
                        ' 1 testata per ogni Cancellazione di CNV
                        '   + 1 record per ogni programmata
                        '   + 1 record per ogni ciclo
                        '   + 1 record per ogni cnv
                        Dim testataLog As New DataLogStructure.Testata(DataLogStructure.TipiArgomento.ELIMINA_PROG, DataLogStructure.Operazione.Eliminazione, codicePazienteCorrente, True,
                                                                       DESCRIZIONE_PROCESSO_CANCELLAZIONE_CONVOCAZIONI, Me.ContextInfos.IDUtente)

                        ' Log programmate eliminate per la cnv corrente
                        If Not listLogVaccProgEliminate Is Nothing AndAlso listLogVaccProgEliminate.Count > 0 Then

                            Dim vacc As IEnumerable(Of Entities.VaccinazioneProgrammata) =
                                listLogVaccProgEliminate.Where(Function(p) p.CodicePaziente = codicePazienteCorrente And p.DataConvocazione = cnv.DataConvocazione)

                            If Not vacc Is Nothing AndAlso vacc.Count > 0 Then

                                Dim list As List(Of DataLogStructure.Record) = Me.GetRecordLogEliminazioneVaccinazioniProgrammate(vacc.ToList())
                                If Not list Is Nothing AndAlso list.Count > 0 Then
                                    For Each record As DataLogStructure.Record In list
                                        testataLog.Records.Add(record)
                                    Next
                                End If

                            End If
                        End If

                        ' Log cicli eliminati per la cnv corrente
                        If Not listLogCicliEliminati Is Nothing AndAlso listLogCicliEliminati.Count > 0 Then

                            Dim cicli As IEnumerable(Of Entities.CicloConvocazione) =
                                listLogCicliEliminati.Where(Function(p) p.CodicePaziente = codicePazienteCorrente And p.DataConvocazione = cnv.DataConvocazione)

                            If Not cicli.IsNullOrEmpty() Then

                                Dim list As List(Of DataLogStructure.Record) = Me.GetRecordLogEliminazioneCicli(cicli.ToList())

                                If Not list.IsNullOrEmpty() Then
                                    For Each record As DataLogStructure.Record In list
                                        testataLog.Records.Add(record)
                                    Next
                                End If

                            End If
                        End If

                        ' Log cnv corrente eliminata
                        If eliminazioneResult.Success Then

                            If Not cnvEliminata Is Nothing Then
                                testataLog.Records.Add(Me.GetRecordLogEliminazioneConvocazione(cnvEliminata))
                            End If

                        End If

                        ' Scrittura log
                        If testataLog.Records.Count > 0 Then
                            Me.WriteLogCancellazione(testataLog)
                        End If
                        ' ----------- '
                    Next

                End If
            End Using

        Catch ex As Exception
            numeroErrori += 1
            ex.InternalPreserveStackTrace()
            Throw
        Finally
            RaiseEvent RefreshParzialeElementiElaborati(New Biz.BizBatch.RefreshParzialeElementiElaboratiEventArgs(numeroElaborazioni, numeroErrori))
        End Try

        Return New BizGenericResult()

    End Function

    ''' <summary>
    ''' Avvia il processo batch di cancellazione delle convocazioni
    ''' </summary>
    ''' <param name="cancellazioneCNVcommand"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function StartBatchCancellazioneConvocazioni(cancellazioneCNVcommand As StartBatchCancellazioneConvocazioniCommand) As BizGenericResult

        If cancellazioneCNVcommand.PazientiConvocazioniDaElaborare Is Nothing OrElse cancellazioneCNVcommand.PazientiConvocazioniDaElaborare.Count = 0 Then
            Return New BizGenericResult(False, String.Format("Nessuna convocazione selezionata: {0} NON avviato.", DESCRIZIONE_PROCESSO_CANCELLAZIONE_CONVOCAZIONI))
        End If

        Dim startBatchCommand As New BizBatch.StartBatchProcedureCommand()
        startBatchCommand.ProcedureId = ID_PROCESSO_CANCELLAZIONE_CONVOCAZIONI
        startBatchCommand.ProcedureDescription = DESCRIZIONE_PROCESSO_CANCELLAZIONE_CONVOCAZIONI
        startBatchCommand.StartingAppId = ContextInfos.IDApplicazione
        startBatchCommand.StartingCodiceAzienda = ContextInfos.CodiceAzienda
        startBatchCommand.StartingUserId = ContextInfos.IDUtente

        startBatchCommand.PazientiConvocazioniDaElaborare = cancellazioneCNVcommand.PazientiConvocazioniDaElaborare

        startBatchCommand.ListAppIdConnections.Add(ContextInfos.IDApplicazione)

        Dim ser As New System.Web.Script.Serialization.JavaScriptSerializer()

        Dim parameterFiltriProgrammazione As String = String.Empty
        If Not cancellazioneCNVcommand.FiltriProgrammazione Is Nothing Then
            parameterFiltriProgrammazione = ser.Serialize(cancellazioneCNVcommand.FiltriProgrammazione)
        End If

        Dim parameterTipoFiltri As String = String.Empty
        If Not cancellazioneCNVcommand.TipoFiltri Is Nothing Then
            parameterTipoFiltri = ser.Serialize(cancellazioneCNVcommand.TipoFiltri)
        End If

        ' Parametri del processo batch
        startBatchCommand.ListParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                             BatchCancellazioneConvocazioni.ParameterName.FiltriProgrammazione, parameterFiltriProgrammazione))

        startBatchCommand.ListParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                             BatchCancellazioneConvocazioni.ParameterName.TipoFiltri, parameterTipoFiltri))

        startBatchCommand.ListParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                             BatchCancellazioneConvocazioni.ParameterName.CancellaInteraConvocazione, cancellazioneCNVcommand.CancellaInteraConvocazione))

        startBatchCommand.ListParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                             BatchCancellazioneConvocazioni.ParameterName.CancellaConSolleciti, cancellazioneCNVcommand.CancellaConSolleciti))

        startBatchCommand.ListParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                             BatchCancellazioneConvocazioni.ParameterName.CancellaConAppuntamenti, cancellazioneCNVcommand.CancellaConAppuntamenti))

        startBatchCommand.ListParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                             BatchCancellazioneConvocazioni.ParameterName.CodiceUslCorrente, ContextInfos.CodiceUsl))

        ' Parametri del report
        startBatchCommand.ListReportParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                                   "Centro vaccinale", String.Format("{0} ({1})", cancellazioneCNVcommand.DatiReport.DescrizioneCentroVaccinale, cancellazioneCNVcommand.DatiReport.CodiceCentroVaccinale)))

        If cancellazioneCNVcommand.DatiReport.DataNascitaDa.HasValue OrElse cancellazioneCNVcommand.DatiReport.DataNascitaA.HasValue Then
            startBatchCommand.ListReportParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                                       "Nascita", Me.GetIntervalloDate(cancellazioneCNVcommand.DatiReport.DataNascitaDa, cancellazioneCNVcommand.DatiReport.DataNascitaA)))
        End If

        If cancellazioneCNVcommand.DatiReport.DataConvocazioneDa.HasValue OrElse cancellazioneCNVcommand.DatiReport.DataConvocazioneA.HasValue Then
            startBatchCommand.ListReportParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                                       "Convocazione", Me.GetIntervalloDate(cancellazioneCNVcommand.DatiReport.DataConvocazioneDa, cancellazioneCNVcommand.DatiReport.DataConvocazioneA)))
        End If

        If Not String.IsNullOrWhiteSpace(cancellazioneCNVcommand.DatiReport.Sesso) Then
            startBatchCommand.ListReportParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                                       "Sesso", cancellazioneCNVcommand.DatiReport.Sesso))
        End If

        If Not String.IsNullOrWhiteSpace(cancellazioneCNVcommand.DatiReport.DescrizioneStatiAnagrafici) Then
            startBatchCommand.ListReportParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                                       "Stati anagrafici", cancellazioneCNVcommand.DatiReport.DescrizioneStatiAnagrafici))
        End If

        If Not String.IsNullOrWhiteSpace(cancellazioneCNVcommand.DatiReport.DescrizioneMalattia) Then
            startBatchCommand.ListReportParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                                       "Malattia", cancellazioneCNVcommand.DatiReport.DescrizioneMalattia))
        End If

        If Not String.IsNullOrWhiteSpace(cancellazioneCNVcommand.DatiReport.DescrizioneCategoriaRischio) Then
            startBatchCommand.ListReportParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                                       "Categoria rischio", cancellazioneCNVcommand.DatiReport.DescrizioneCategoriaRischio))
        End If

        ' Filtro programmazione (cicli/associazioni/vaccinazioni)
        If Not cancellazioneCNVcommand.FiltriProgrammazione Is Nothing Then

            Dim parametro As KeyValuePair(Of String, String) =
                Me.GetNomeValoreFiltroProgrammazione(cancellazioneCNVcommand.FiltriProgrammazione, cancellazioneCNVcommand.TipoFiltri)

            startBatchCommand.ListReportParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                                       parametro.Key, parametro.Value))

        End If

        startBatchCommand.ListReportParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                                   "Cancella intera convocazione", Me.GetReportParameterBooleanValue(cancellazioneCNVcommand.CancellaInteraConvocazione)))

        startBatchCommand.ListReportParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                                   "Cancella anche convocazioni con sollecito", Me.GetReportParameterBooleanValue(cancellazioneCNVcommand.CancellaConSolleciti)))

        startBatchCommand.ListReportParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                                   "Cancella anche convocazioni con appuntamento", Me.GetReportParameterBooleanValue(cancellazioneCNVcommand.CancellaConAppuntamenti)))


        Dim bizBatch As New BizBatch(GenericProvider)

        Return bizBatch.StartBatchProcedure(startBatchCommand)

    End Function

#End Region

#End Region

#Region " Private "

    Private Function GetOrderByConvocazioni(campoOrdinamento As String, versoOrdinamento As String) As String

        'Ordinamento di default
        Dim orderBy As String = "paz_cognome ASC, paz_nome ASC, paz_data_nascita ASC "

        'Ordinamento manuale
        If Not String.IsNullOrWhiteSpace(campoOrdinamento) Then

            'Aggiornamento campoOrdinamento
            Select Case campoOrdinamento
                Case "PazienteCognome"
                    orderBy = String.Format("paz_cognome {0}", versoOrdinamento)
                Case "PazienteNome"
                    orderBy = String.Format("paz_nome {0}", versoOrdinamento)
                Case "PazienteDataNascita"
                    orderBy = String.Format("paz_data_nascita {0}", versoOrdinamento)
                Case "Data"
                    orderBy = String.Format("cnv_data {0}", versoOrdinamento)
                Case "Vaccinazioni"
                    orderBy = String.Format("vaccinazioni {0}", versoOrdinamento)
                Case "DataAppuntamento"
                    orderBy = String.Format("cnv_data_appuntamento {0}", versoOrdinamento)
                Case Else
            End Select

        End If

        Return orderBy

    End Function

    Private Function GetRecordLogEliminazioneVaccinazioniProgrammate(vaccinazioniProgrammate As List(Of Entities.VaccinazioneProgrammata)) As List(Of DataLogStructure.Record)

        Dim listRecordLog As New List(Of DataLogStructure.Record)()

        For Each vac As Entities.VaccinazioneProgrammata In vaccinazioniProgrammate
            If Not vac Is Nothing Then

                Dim recordLog As New DataLogStructure.Record()

                recordLog.Campi.Add(New DataLogStructure.Campo("VPR_CNV_DATA", vac.DataConvocazione.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)))
                recordLog.Campi.Add(New DataLogStructure.Campo("VPR_VAC_CODICE", vac.CodiceVaccinazione))
                recordLog.Campi.Add(New DataLogStructure.Campo("VPR_N_RICHIAMO", vac.NumeroRichiamo.ToString()))

                If String.IsNullOrWhiteSpace(vac.CodiceCiclo) Then
                    recordLog.Campi.Add(New DataLogStructure.Campo("VPR_CIC_CODICE", String.Empty))
                Else
                    recordLog.Campi.Add(New DataLogStructure.Campo("VPR_CIC_CODICE", vac.CodiceCiclo))
                End If

                If vac.NumeroSeduta.HasValue Then
                    recordLog.Campi.Add(New DataLogStructure.Campo("VPR_N_SEDUTA", vac.NumeroSeduta.Value.ToString()))
                Else
                    recordLog.Campi.Add(New DataLogStructure.Campo("VPR_N_SEDUTA", String.Empty))
                End If

                If String.IsNullOrWhiteSpace(vac.CodiceAssociazione) Then
                    recordLog.Campi.Add(New DataLogStructure.Campo("VPR_ASS_CODICE", String.Empty))
                Else
                    recordLog.Campi.Add(New DataLogStructure.Campo("VPR_ASS_CODICE", vac.CodiceAssociazione))
                End If

                listRecordLog.Add(recordLog)

            End If
        Next

        Return listRecordLog

    End Function

    Private Function GetRecordLogEliminazioneCicli(cicli As List(Of Entities.CicloConvocazione)) As List(Of DataLogStructure.Record)

        Dim listRecordLog As New List(Of DataLogStructure.Record)()

        For Each ciclo As Entities.CicloConvocazione In cicli
            If Not ciclo Is Nothing Then

                Dim recordLog As New DataLogStructure.Record()

                recordLog.Campi.Add(New DataLogStructure.Campo("CNC_CNV_DATA", ciclo.DataConvocazione.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)))
                recordLog.Campi.Add(New DataLogStructure.Campo("CNC_CIC_CODICE", ciclo.CodiceCiclo))
                recordLog.Campi.Add(New DataLogStructure.Campo("CNC_SED_N_SEDUTA", ciclo.NumeroSeduta.ToString()))

                If String.IsNullOrWhiteSpace(ciclo.FlagGiorniPosticipo) Then
                    recordLog.Campi.Add(New DataLogStructure.Campo("CNC_FLAG_GIORNI_POSTICIPO", String.Empty))
                Else
                    recordLog.Campi.Add(New DataLogStructure.Campo("CNC_FLAG_GIORNI_POSTICIPO", ciclo.FlagGiorniPosticipo))
                End If

                If String.IsNullOrWhiteSpace(ciclo.FlagPosticipoSeduta) Then
                    recordLog.Campi.Add(New DataLogStructure.Campo("CNC_FLAG_POSTICIPO_SEDUTA", String.Empty))
                Else
                    recordLog.Campi.Add(New DataLogStructure.Campo("CNC_FLAG_POSTICIPO_SEDUTA", ciclo.FlagPosticipoSeduta))
                End If

                If ciclo.NumeroSollecito.HasValue Then
                    recordLog.Campi.Add(New DataLogStructure.Campo("CNC_N_SOLLECITO", ciclo.NumeroSollecito.Value.ToString()))
                Else
                    recordLog.Campi.Add(New DataLogStructure.Campo("CNC_N_SOLLECITO", String.Empty))
                End If

                If ciclo.DataInvioSollecito.HasValue Then
                    recordLog.Campi.Add(New DataLogStructure.Campo("CNC_DATA_INVIO_SOLLECITO", ciclo.DataInvioSollecito.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)))
                Else
                    recordLog.Campi.Add(New DataLogStructure.Campo("CNC_DATA_INVIO_SOLLECITO", String.Empty))
                End If

                If ciclo.DataInserimento.HasValue Then
                    recordLog.Campi.Add(New DataLogStructure.Campo("CNC_DATA_INSERIMENTO", ciclo.DataInserimento.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)))
                Else
                    recordLog.Campi.Add(New DataLogStructure.Campo("CNC_DATA_INSERIMENTO", String.Empty))
                End If

                If ciclo.IdUtenteInserimento.HasValue Then
                    recordLog.Campi.Add(New DataLogStructure.Campo("CNC_UTE_ID_INSERIMENTO", ciclo.IdUtenteInserimento.Value.ToString()))
                Else
                    recordLog.Campi.Add(New DataLogStructure.Campo("CNC_UTE_ID_INSERIMENTO", String.Empty))
                End If

                listRecordLog.Add(recordLog)

            End If
        Next

        Return listRecordLog

    End Function

    Private Function GetRecordLogEliminazioneConvocazione(cnv As Entities.Convocazione) As DataLogStructure.Record

        Dim recordLog As New DataLogStructure.Record()

        recordLog.Campi.Add(New DataLogStructure.Campo("CNV_DATA", cnv.Data_CNV.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)))

        If cnv.DataInvio > DateTime.MinValue Then
            recordLog.Campi.Add(New DataLogStructure.Campo("CNV_DATA_INVIO", cnv.DataInvio.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)))
        Else
            recordLog.Campi.Add(New DataLogStructure.Campo("CNV_DATA_INVIO", String.Empty))
        End If

        If cnv.DataAppuntamento > DateTime.MinValue Then
            recordLog.Campi.Add(New DataLogStructure.Campo("CNV_DATA_APPUNTAMENTO", cnv.DataAppuntamento.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)))
        Else
            recordLog.Campi.Add(New DataLogStructure.Campo("CNV_DATA_APPUNTAMENTO", String.Empty))
        End If

        recordLog.Campi.Add(New DataLogStructure.Campo("CNV_DURATA_APPUNTAMENTO", cnv.Durata_Appuntamento.ToString()))
        recordLog.Campi.Add(New DataLogStructure.Campo("CNV_CNS_CODICE", cnv.Cns_Codice))
        recordLog.Campi.Add(New DataLogStructure.Campo("CNV_TIPO_APPUNTAMENTO", cnv.TipoAppuntamento))

        If cnv.DataPrimoAppuntamento > DateTime.MinValue Then
            recordLog.Campi.Add(New DataLogStructure.Campo("CNV_PRIMO_APPUNTAMENTO", cnv.DataPrimoAppuntamento.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)))
        Else
            recordLog.Campi.Add(New DataLogStructure.Campo("CNV_PRIMO_APPUNTAMENTO", String.Empty))
        End If

        recordLog.Campi.Add(New DataLogStructure.Campo("CNV_CAMPAGNA", cnv.CampagnaVaccinale))
        recordLog.Campi.Add(New DataLogStructure.Campo("CNV_AMB_CODICE", cnv.CodiceAmbulatorio))

        If cnv.DataInserimento.HasValue Then recordLog.Campi.Add(New DataLogStructure.Campo("CNV_DATA_INSERIMENTO", cnv.DataInserimento.Value))
        If cnv.IdUtenteInserimento.HasValue Then recordLog.Campi.Add(New DataLogStructure.Campo("CNV_UTE_ID_INSERIMENTO", cnv.IdUtenteInserimento.Value))

        Return recordLog

    End Function

    Private Sub WriteLogCancellazione(testataLog As DataLogStructure.Testata)

        ' N.B. : connection string della usl corrente - provider di default del log (impostato nel file config)
        If LogBox.IsEnabled AndAlso LogBox.Argomenti.Count = 0 Then
            '--
            ' Caricamento argomenti log. 
            ' N.B. : se OnBatch sta scrivendo questo log per la prima volta, non ha ancora caricato gli argomenti quindi non scriverebbe il log!
            '--
            Using bizLog As New Biz.BizLog(Me.GenericProvider, Me.ContextInfos)

                Dim listArgomentiLog As List(Of DataLogStructure.Argomento) = bizLog.GetListArgomentiAttivi()

                If Not listArgomentiLog Is Nothing Then
                    For Each argomentoLog As DataLogStructure.Argomento In listArgomentiLog
                        LogBox.Argomenti.Add(argomentoLog)
                    Next
                End If

            End Using
            '--

        End If

        Me.WriteLog(testataLog)

    End Sub

    Private Function GetReportParameterBooleanValue(value As Boolean) As String

        If value Then Return "Si"

        Return "No"

    End Function

    Private Function GetIntervalloDate(da As DateTime?, a As DateTime?) As String

        Dim intervallo As String = String.Empty

        If da.HasValue Then
            intervallo = String.Format("da {0} ", da.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture))
        End If

        If a.HasValue Then
            intervallo += String.Format("a {0} ", a.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture))
        End If

        Return intervallo

    End Function

    Private Function GetNomeValoreFiltroProgrammazione(filtriProgrammazione As List(Of Entities.FiltroProgrammazione), tipoFiltri As Entities.TipoFiltriProgrammazione) As KeyValuePair(Of String, String)

        Dim nomeParametro As String = String.Empty
        Select Case tipoFiltri
            Case Entities.TipoFiltriProgrammazione.AssociazioneDose
                nomeParametro = "Associazioni-dosi selezionate"
            Case Entities.TipoFiltriProgrammazione.CicloSeduta
                nomeParametro = "Cicli-sedute selezionati"
            Case Entities.TipoFiltriProgrammazione.VaccinazioneDose
                nomeParametro = "Vaccinazioni-dosi selezionate"
        End Select

        Dim valoreParametro As New Text.StringBuilder()

        For Each filtro As Entities.FiltroProgrammazione In filtriProgrammazione

            If Not String.IsNullOrWhiteSpace(filtro.Codice) Then
                valoreParametro.Append(filtro.Codice)
            End If

            If filtro.Valore.HasValue Then

                If String.IsNullOrWhiteSpace(filtro.Codice) Then
                    valoreParametro.Append(filtro.Valore.Value.ToString())
                Else
                    valoreParametro.AppendFormat("({0})", filtro.Valore.Value.ToString())
                End If

            End If

            valoreParametro.Append("; ")
        Next

        If valoreParametro.Length > 0 Then
            valoreParametro.Remove(valoreParametro.Length - 2, 2)
        End If

        Return New KeyValuePair(Of String, String)(nomeParametro, valoreParametro.ToString())

    End Function

#End Region

End Class
