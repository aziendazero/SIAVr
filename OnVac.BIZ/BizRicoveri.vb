Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities

Public Class BizRicoveri
    Inherits BizClass
    Public Sub New(genericProvider As DbGenericProvider, settings As Settings.Settings, contextInfo As BizContextInfos)
        MyBase.New(genericProvider, settings, contextInfo, Nothing)
    End Sub

    Public Function SalvaRicovero(command As SalvaRicovero) As RicoveroPaziente
        Dim risultato As RicoveroPaziente = Me.GenericProvider.Ricoveri.SalvaRicovero(command, Me.ContextInfos.IDUtente)
        Return risultato
    End Function

    Private Function CheckRicoveroInData(ricovero As RicoveroPaziente, inizio As DateTime, fine As DateTime?) As Boolean
        For Each evento As RicoveroPaziente.EpisodioRicovero In ricovero.Episodi
            If evento.DataIngresso < inizio Then
                Return False
            End If
            If fine.HasValue Then
                If evento.DataIngresso > fine.Value Then
                    Return False
                End If
            End If
            Return True
        Next
        Return False
    End Function


    Public Function GetRicoveriPaziente(codicePaziente As Long) As IEnumerable(Of RicoveroPaziente)
        Return Me.GenericProvider.Ricoveri.GetRicoveriPaziente(codicePaziente)
    End Function

    Public Sub EliminaRicovero(codiceGruppo As String)
        Me.GenericProvider.Ricoveri.EliminaRicovero(codiceGruppo, Me.ContextInfos.IDUtente)
    End Sub

    Public Function CercaRicoveri(filtri As FiltriRicoveri) As IEnumerable(Of TestataRicovero)
        Dim ricoveri As IEnumerable(Of TestataRicovero) = Me.GenericProvider.Ricoveri.CercaRicoveri(filtri)
        Dim codiciPazienti As IEnumerable(Of Long) = ricoveri.Select(Function(r)
                                                                         Return r.CodicePaziente
                                                                     End Function).Distinct()

        For Each p As Long In codiciPazienti
            Dim el As List(Of TamponeDatiRidotti) = Me.GenericProvider.Covid19Tamponi.GetAllTamponiById(p)
            Dim tamponi As IEnumerable(Of TamponeDatiRidotti) = el.Where(Function(t) t.DataReferto.HasValue)
            If tamponi.Any() Then

                Dim last As Date? = tamponi.OrderByDescending(Function(t) t.DataReferto).Select(Function(t) t.DataReferto).FirstOrDefault()
                Dim pos As Date? = tamponi.OrderByDescending(Function(t) t.DataReferto).Where(Function(t) String.Equals(t.Esito, "POSITIVO", StringComparison.InvariantCultureIgnoreCase)).Select(Function(t) t.DataReferto).FirstOrDefault()

                For Each ricovero As TestataRicovero In ricoveri.Where(Function(r) r.CodicePaziente = p)
                    ricovero.DataUltimoTampone = last
                    ricovero.DataUltimoTamponePositivo = pos
                Next

            End If
        Next
        Return ricoveri
    End Function

    Public Sub AggiornaStatoEpisodi(dataRicovero As Date, codiceTipoRicovero As Long, codiciEpisodi As IEnumerable(Of Long))
        If IsNothing(codiciEpisodi) OrElse Not codiciEpisodi.Any() Then
            Return
        End If

        GenericProvider.RilevazioniCovid19.AggiornaStatoSorveglianzaEpisodiDaRicovero(dataRicovero, codiceTipoRicovero, codiciEpisodi.ToArray())

    End Sub
End Class
