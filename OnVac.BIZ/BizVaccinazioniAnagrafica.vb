Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL

''' <summary>
''' Classe di business relativa all'ANAGRAFICA delle vaccinazioni
''' </summary>
''' <remarks></remarks>
Public Class BizVaccinazioniAnagrafica
    Inherits BizClass

#Region " Costruttori "

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos)

        MyBase.New(genericprovider, settings, contextInfos, Nothing)

    End Sub

#End Region

#Region " Public "

    ''' <summary>
    ''' Restituisce la descrizione della vaccinazione in base al codice
    ''' </summary>
    ''' <param name="codiceVaccinazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDescrizioneVaccinazione(codiceVaccinazione As String) As String

        Return GenericProvider.AnaVaccinazioni.GetDescrizioneVaccinazione(codiceVaccinazione)

    End Function

    ''' <summary>
    ''' Restituisce true se la vaccinazione è marcata come anti-influenzale
    ''' </summary>
    ''' <param name="codiceVaccinazione"></param>
    ''' <returns></returns>
    Public Function ExistsVaccinazioneAntiInfluenzale(codiceVaccinazione As String) As Boolean

        Return GenericProvider.AnaVaccinazioni.ExistsVaccinazioneAntiInfluenzale(codiceVaccinazione)

    End Function

#Region " Codifiche Vaccinazione-Associazione "

    ''' <summary>
    ''' Restituisce la lista delle codifiche esterne per associazione relative alla vaccinazione specificata
    ''' </summary>
    ''' <param name="codiceVaccinazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetListCodiciEsterniVaccinazioneAssociazione(codiceVaccinazione As String) As List(Of Entities.CodificaEsternaVaccinazioneAssociazione)

        Return GenericProvider.AnaVaccinazioni.GetListCodiciEsterniVaccinazioneAssociazione(codiceVaccinazione)

    End Function

    ''' <summary>
    ''' Salvataggio codifiche per associazione 
    ''' </summary>
    ''' <param name="codiceVaccinazione"></param>
    ''' <param name="list"></param>
    ''' <param name="maxLengthCodiceEsterno"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SaveCodificaEsternaVaccinazioneAssociazione(codiceVaccinazione As String, list As List(Of Entities.CodificaEsternaVaccinazioneAssociazione), maxLengthCodiceEsterno As Integer) As BizGenericResult

        If Not list Is Nothing AndAlso list.Count > 0 Then

            If list.Any(Function(p) String.IsNullOrWhiteSpace(p.CodiceVaccinazione) OrElse String.IsNullOrWhiteSpace(p.CodiceAssociazione)) Then
                Return New BizGenericResult(False, "Salvataggio non effettuato: dati non completi.")
            End If

            If maxLengthCodiceEsterno > 0 AndAlso list.Any(Function(p) p.CodiceEsterno.Count > maxLengthCodiceEsterno) Then
                Return New BizGenericResult(False, "Attenzione, alcuni valori inseriti superano la lunghezza massima!")
            End If

        End If

        ' Cancellazione di tutte le codifiche per la vaccinazione specificata
        Me.GenericProvider.AnaVaccinazioni.DeleteCodificheAssociazione(codiceVaccinazione)

        ' Inserimento codifiche 
        If Not list Is Nothing AndAlso list.Count > 0 Then

            list.RemoveAll(Function(p) String.IsNullOrWhiteSpace(p.CodiceEsterno))

            If list.Count > 0 Then

                Dim ownTransaction As Boolean = False

                Try
                    If Me.GenericProvider.Transaction Is Nothing Then
                        Me.GenericProvider.BeginTransaction()
                        ownTransaction = True
                    End If

                    For Each codifica As Entities.CodificaEsternaVaccinazioneAssociazione In list

                        Me.GenericProvider.AnaVaccinazioni.InsertCodificaVaccinazioneAssociazione(codifica)

                    Next

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

            End If
        End If

        Return New BizGenericResult()

    End Function

#End Region

#Region " Antigene "

    ''' <summary>
    ''' Ottengo il codice antigene partendo dal codice vaccino o associazione
    ''' </summary>
    ''' <param name="codAssociazione"></param>
    ''' <param name="codVaccinazione"></param>
    ''' <returns></returns>
    Public Function GetAntigene(codiceAss As String, codiceVac As String) As String

        Dim codificaEsternaVac As String = String.Empty
        Dim vacAvn As Entities.VaccinazioneAVN = GenericProvider.AnaVaccinazioni.GetCodiceAvnVaccinazione(codiceVac)

        If Not vacAvn Is Nothing Then
            If vacAvn.TipoCodiceAvn = "V" Then
                Return vacAvn.CodiceAvn
            ElseIf vacAvn.TipoCodiceAvn = "A" Then
                codificaEsternaVac = GenericProvider.AnaVaccinazioni.GetCodificaEsterna(codiceVac, codiceAss)
                If Not codificaEsternaVac Is Nothing Then
                    Return codificaEsternaVac.Trim()
                End If
            End If
        End If

        Return codificaEsternaVac

    End Function

#End Region

#End Region

#Region " Private "

    '' Scarta la codifica se l'intervallo di età si accavalla con gli altri già esistenti
    'Private Function CheckIntervalliEta(codifica As Entities.CodificaEsternaVaccinazioneEta, listCodificheEsistenti As List(Of Entities.CodificaEsternaVaccinazioneEta)) As Boolean

    '    For Each condizioneEsistente As Entities.CodificaEsternaVaccinazioneEta In listCodificheEsistenti

    '        If Not condizioneEsistente.EtaFine.HasValue OrElse codifica.EtaInizio.Value < condizioneEsistente.EtaFine.Value Then
    '            '--
    '            If Not codifica.EtaFine.HasValue Then Return False
    '            If codifica.EtaFine.Value > condizioneEsistente.EtaInizio.Value Then Return False
    '            '--
    '        End If

    '    Next

    '    Return True

    'End Function

#End Region

End Class
