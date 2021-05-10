Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.Collection
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.DAL


Public Class BizSollecitiBilanci
    Inherits BizClass

#Region " Costruttori"

    Public Sub New(genericProvider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos)

        MyBase.New(genericProvider, settings, contextInfos, Nothing)

    End Sub

#End Region

#Region " Public "

    ''' <summary>
    ''' Controlla tutti i bilanci da sollecitare del paziente
    ''' </summary>
    ''' <param name="codicePaziente">Codice del paziente</param>
    Public Sub ControlloBilanciDaSollecitare(codicePaziente As Integer)

        Dim list As List(Of Entities.BilancioDaSollecitare) = Me.GenericProvider.BilancioProgrammato.GetBilanciDaSollecitare(codicePaziente)

        If list.IsNullOrEmpty() Then Return

        Dim now As DateTime = DateTime.Now

        For Each item As Entities.BilancioDaSollecitare In list

            If item.SollecitiCreati = 0 OrElse item.UltimaDataInvio.HasValue Then

                ' Per questo bilancio, non sono stati creati altri solleciti prima d'ora oppure l'ultima data di invio non è nulla

                If item.NumeroSollecitiBilancio <> 0 Then

                    ' IL BILANCIO PREVEDE SOLLECITI

                    ' Se i solleciti creati sono in numero minore rispetto al massimo previsto, ne crea ancora.
                    If item.SollecitiCreati < item.NumeroSollecitiBilancio Then

                        ' Creazione nuovo sollecito con data invio nulla
                        Me.GenericProvider.SollecitiBilanci.NewRecord(item.IdBilancio, DateTime.MinValue)

                        ' Aggiornamento convocazione
                        If item.FlagSoloBilancio Then

                            ' Nella convocazione c'è solo questo bilancio: cancellazione dati di appuntamento e invio.
                            Using bizConvocazione As New BizConvocazione(Me.GenericProvider, Me.Settings, Me.ContextInfos, Nothing)

                                Dim convocazioneAppuntamento As Entities.ConvocazioneAppuntamento = bizConvocazione.CreateConvocazioneAppuntamentoToDelete(
                                    codicePaziente, item.DataConvocazione, Nothing, now, Constants.MotiviEliminazioneAppuntamento.EliminazioneAppuntamento,
                                    "Eliminazione appuntamento in convocazione solo bilancio con sollecito")

                                bizConvocazione.UpdateConvocazione_DatiAppuntamento(convocazioneAppuntamento)

                            End Using

                        Else

                            ' Nella convocazione ci sono anche vaccinazioni o altri bilanci: il bilancio viene spostato ad oggi.
                            If Not Me.GenericProvider.Convocazione.Exists(codicePaziente, Date.Today) Then

                                ' Se non esiste, creo una nuova convocazione in data odierna (no appuntamento => no storico appuntamenti)
                                Me.GenericProvider.Convocazione.Copy(codicePaziente, item.DataConvocazione, Date.Today, True, True)

                            End If

                            ' Associo il bilancio alla convocazione in data odierna
                            Dim bil As BilancioProgrammato = CreateBilancio(codicePaziente, item.IdBilancio, item.CodiceMalattia, item.NumeroBilancio, Date.Today, item.StatoBilancio)

                            Me.GenericProvider.BilancioProgrammato.Update(bil)

                        End If

                    End If

                Else

                    ' IL BILANCIO NON PREVEDE I SOLLECITI

                    ' Update del bilancio per metterlo in stato unsolved ("US")
                    Dim bil As BilancioProgrammato = CreateBilancio(codicePaziente, item.IdBilancio, item.CodiceMalattia, item.NumeroBilancio, item.DataConvocazione, Constants.StatiBilancio.UNSOLVED)
                    Me.GenericProvider.BilancioProgrammato.Update(bil)

                    ' Eliminazione convocazione se solo bilancio
                    If item.FlagSoloBilancio Then

                        Using bizConvocazione As New BizConvocazione(Me.GenericProvider, Me.Settings, Me.ContextInfos, New BizLogOptions(Log.DataLogStructure.TipiArgomento.VAC_PROGRAMMATE, False))

                            Dim command As New BizConvocazione.EliminaConvocazioniSollecitiBilanciCommand()
                            command.CodicePaziente = codicePaziente
                            command.DataConvocazione = item.DataConvocazione
                            command.CancellaBilanciAssociati = False
                            command.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.EliminazioneConvocazione
                            command.DataEliminazione = now
                            command.NoteEliminazione = "Eliminazione convocazione solo bilancio"
                            command.WriteLog = False

                            bizConvocazione.EliminaConvocazioniSollecitiBilanci(command)

                        End Using

                    End If
                End If
            End If

        Next

    End Sub

#End Region

#Region " Private "

    Private Function CreateBilancio(codicePaziente As Integer, idBilancio As Integer, codiceMalattia As String, numeroBilancio As Integer, dataConvocazione As Date, statoBilancio As String) As BilancioProgrammato

        Dim bil As New BilancioProgrammato(codiceMalattia, numeroBilancio, dataConvocazione, statoBilancio, False, codicePaziente)

        bil.Bil_id = idBilancio

        Return bil

    End Function

#End Region

End Class
