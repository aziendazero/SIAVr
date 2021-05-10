Imports System.Collections.Generic

Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Log.DataLogStructure


Public Class BizMovimentiInterniCNS
    Inherits Biz.BizClass

#Region " Types "

    Public Class BizRisultatoMovimentiInterniCNS

        Public Successo As Boolean

        Public Messaggio As String

        Public Sub New()
        End Sub

        Public Sub New(successo As Boolean, messaggio As String)
            Me.Successo = successo
            Me.Messaggio = messaggio
        End Sub

    End Class

#End Region

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)

        MyBase.New(genericprovider, settings, contextInfos, logOptions)

    End Sub

#End Region

#Region " Public "

    ''' <summary>
    ''' Imposta a S il campo "Presa visione"
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="progressivoMovimento"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ImpostaPresaVisionePaziente(codicePaziente As Integer, progressivoMovimento As Integer) As BizRisultatoMovimentiInterniCNS

        Try
            Me.GenericProvider.BeginTransaction()

            ' Presa visione = "S"
            Me.GenericProvider.MovimentiInterniCNS.UpdatePresaVisione(progressivoMovimento, "S")

            ' Regolarizzazione se paziente domiciliato
            If Me.GenericProvider.Paziente.GetCodiceStatoAnag(codicePaziente) = Enumerators.StatoAnagrafico.DOMICILIATO Then

                Me.GenericProvider.Paziente.RegolarizzaPaziente(codicePaziente, True)

            End If

            Me.GenericProvider.Commit()

        Catch ex As Exception

            If Not Me.GenericProvider.Transaction Is Nothing Then Me.GenericProvider.Rollback()

            Return New BizRisultatoMovimentiInterniCNS(False, ex.Message.Replace(vbCrLf, "\n").Replace(Environment.NewLine, "\n"))

        End Try

        Return New BizRisultatoMovimentiInterniCNS(True, String.Empty)

    End Function

    ''' <summary>
    ''' Imposta a S il campo "Invio cartella"
    ''' </summary>
    ''' <param name="progressivoMovimento"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ImpostaInvioCartella(progressivoMovimento As Integer) As BizRisultatoMovimentiInterniCNS

        Try
            Me.GenericProvider.BeginTransaction()

            Me.GenericProvider.MovimentiInterniCNS.UpdateInvioCartella(progressivoMovimento, True)

            Me.GenericProvider.Commit()

        Catch ex As Exception

            If Not Me.GenericProvider.Transaction Is Nothing Then Me.GenericProvider.Rollback()

            Return New BizRisultatoMovimentiInterniCNS(False, ex.Message.Replace(vbCrLf, "\n").Replace(Environment.NewLine, "\n"))

        End Try

        Return New BizRisultatoMovimentiInterniCNS(True, String.Empty)

    End Function

    ''' <summary>
    ''' Movimentazione dei pazienti presenti dal consultorio di smistamento a quello specificato.
    ''' Per ogni paziente in lista: 
    '''   modifica consultorio assegnato, 
    '''   modifica consultorio nelle convocazioni, 
    '''   inserimento movimento di consultorio, 
    '''   update dello stato anagrafico.
    ''' </summary>
    ''' <param name="listPazientiSmistamento"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SalvataggioDatiPazientiSmistamento(listPazientiSmistamento As List(Of Entities.MovimentoCNS.PazienteSmistamento)) As BizRisultatoMovimentiInterniCNS

        Dim dataAssegnazione As DateTime = Date.Today

        Try
            Me.GenericProvider.BeginTransaction()

            Using bizPaziente As New Biz.BizPaziente(Me.GenericProvider, Me.Settings, Me.ContextInfos, Me.LogOptions)

                For Each pazienteSmistamento As Entities.MovimentoCNS.PazienteSmistamento In listPazientiSmistamento

                    Dim testataLog As New Testata(TipiArgomento.PAZIENTI, Operazione.Modifica, pazienteSmistamento.CodicePaziente, False)

                    ' Controllo se il consultorio è stato assegnato (diverso dal precedente)
                    If Not String.IsNullOrEmpty(pazienteSmistamento.CodiceConsultorioCorrente) AndAlso
                       pazienteSmistamento.CodiceConsultorioPrecedente <> pazienteSmistamento.CodiceConsultorioCorrente Then

                        Dim command As New Biz.BizPaziente.ModificaConsultorioPazienteCommand()
                        command.CodicePaziente = pazienteSmistamento.CodicePaziente
                        command.CodiceConsultorioNew = pazienteSmistamento.CodiceConsultorioCorrente
                        command.CodiceConsultorioOld = pazienteSmistamento.CodiceConsultorioPrecedente
                        command.DataAssegnazioneConsultorio = dataAssegnazione
                        command.FlagInvioCartella = True
                        command.FlagMovimentoAutomaticoPassaggioAdulti = False
                        command.DataEliminazione = dataAssegnazione
                        command.NoteEliminazione = "Eliminazione appuntamento per variazione centro vaccinale da Movimenti"
                        command.UpdateConsultoriAnagraficaPaziente = Biz.BizPaziente.UpdateConsultoriAnagraficaPazienteType.UpdateConsultorioVaccinaleOnly
                        command.UpdateConvocazioniSoloConsultorioOld = True

                        Dim result As Biz.BizPaziente.ModificaConsultorioPazienteResult = bizPaziente.ModificaConsultorioPaziente(command)

                        ' Log
                        If result.CountConsultoriModificati > 0 Then

                            Dim recordLog As New Record()
                            recordLog.Campi.Add(New Campo("PAZ_CNS_CODICE_OLD", String.Empty, pazienteSmistamento.CodiceConsultorioPrecedente))
                            recordLog.Campi.Add(New Campo("PAZ_CNS_CODICE", pazienteSmistamento.CodiceConsultorioPrecedente, pazienteSmistamento.CodiceConsultorioCorrente))
                            recordLog.Campi.Add(New Campo("PAZ_CNS_DATA_ASSEGNAZIONE", String.Empty, dataAssegnazione.ToString("dd/MM/yyyy")))

                            testataLog.Records.Add(recordLog)

                        End If

                        If result.CountConvocazioniModificate > 0 Then

                            Dim recordLog As New Record()

                            recordLog.Campi.Add(New Campo("Numero cnv del paziente in cui è stato modificato il cns (per variazione cns da smistamento)", result.CountConvocazioniModificate.ToString()))
                            recordLog.Campi.Add(New Campo("CNV_CNS_CODICE", String.Empty, pazienteSmistamento.CodiceConsultorioCorrente))
                            recordLog.Campi.Add(New Campo("CNV_DATA_APPUNTAMENTO", "NULL"))
                            recordLog.Campi.Add(New Campo("CNV_DATA_INVIO", "NULL"))
                            recordLog.Campi.Add(New Campo("CNV_AMB_CODICE", "NULL"))

                            testataLog.Records.Add(recordLog)

                        End If

                        If result.CountMovimentiInseriti > 0 Then

                            Dim recordLog As New Record()

                            recordLog.Campi.Add(New Campo("Numero movimenti inseriti per variazione cns paziente da smistamento", result.CountMovimentiInseriti.ToString()))
                            recordLog.Campi.Add(New Campo("CNM_CNS_CODICE_OLD", String.Empty, pazienteSmistamento.CodiceConsultorioPrecedente))
                            recordLog.Campi.Add(New Campo("CNM_CNS_CODICE_NEW", String.Empty, pazienteSmistamento.CodiceConsultorioCorrente))
                            recordLog.Campi.Add(New Campo("CNM_DATA", String.Empty, dataAssegnazione.ToString("dd/MM/yyyy")))
                            recordLog.Campi.Add(New Campo("CNM_INVIO_CARTELLA", "S"))
                            recordLog.Campi.Add(New Campo("CNM_PRESA_VISIONE", "N"))

                            testataLog.Records.Add(recordLog)

                        End If

                    End If

                    ' Salvataggio stato anagrafico del paziente (se diverso dal precedente) + log
                    If Not String.IsNullOrEmpty(pazienteSmistamento.StatoAnagraficoCorrente) AndAlso
                       (pazienteSmistamento.StatoAnagraficoPrecedente Is Nothing OrElse
                        pazienteSmistamento.StatoAnagraficoCorrente <> pazienteSmistamento.StatoAnagraficoPrecedente) Then

                        If Not bizPaziente.UpdateStatoAnagrafico(pazienteSmistamento.CodicePaziente,
                                                                 pazienteSmistamento.StatoAnagraficoCorrente,
                                                                 pazienteSmistamento.StatoAnagraficoPrecedente,
                                                                 testataLog) Then

                            Me.GenericProvider.Rollback()

                            Return New BizRisultatoMovimentiInterniCNS(False, "Salvataggio non effettuato: stato anagrafico non previsto")

                        End If


                    End If

                    If Not testataLog Is Nothing AndAlso testataLog.Records.Count > 0 Then

                        Log.LogBox.WriteData(testataLog)

                    End If

                Next
            End Using

            Me.GenericProvider.Commit()

        Catch exc As Exception

            Me.GenericProvider.Rollback()

            exc.InternalPreserveStackTrace()
            Throw

        End Try

        Return New BizRisultatoMovimentiInterniCNS(True, String.Empty)

    End Function


    Public Function GetDtSmistamenti(smistamentiFilter As AppoggiatiRASMIFiltriRicerca, pagingOptions As MovimentiInterniCNSPagingOptions?) As DataTable

        Return GenericProvider.MovimentiInterniCNS.GetDtSmistamenti(smistamentiFilter, pagingOptions, ContextInfos.CodiceUsl, Settings.CNS_DEFAULT)

    End Function

    Public Function CountSmistamenti(smistamentiFilter As AppoggiatiRASMIFiltriRicerca) As Integer

        Return GenericProvider.MovimentiInterniCNS.CountSmistamenti(smistamentiFilter, ContextInfos.CodiceUsl, Settings.CNS_DEFAULT)

    End Function

#End Region

End Class
