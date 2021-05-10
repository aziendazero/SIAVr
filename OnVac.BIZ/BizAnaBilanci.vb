Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL

Public Class BizAnaBilanci
    Inherits BizClass

#Region " Constructors "

    Public Sub New(genericProvider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos)

        MyBase.New(genericProvider, settings, contextInfos, Nothing)

    End Sub

#End Region

#Region " Public "

#Region " Anagrafica Bilanci "

    ''' <summary>
    ''' Restituisce i dati relativi al bilancio specificato.
    ''' </summary>
    ''' <param name="numeroBilancio"></param>
    ''' <param name="codiceMalattia"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAnagraficaBilancio(numeroBilancio As Integer, codiceMalattia As String) As Entities.BilancioAnagrafica

        Return Me.GenericProvider.AnaBilancio.GetAnagraficaBilancio(numeroBilancio, codiceMalattia)

    End Function

    ''' <summary>
    ''' Restituisce le anagrafiche dei bilanci relativi alla malattia specificata
    ''' </summary>
    ''' <param name="codiceMalattia"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAnagraficaBilanciByMalattia(codiceMalattia As String) As List(Of Entities.BilancioAnagrafica)

        Return Me.GenericProvider.AnaBilancio.GetAnagraficaBilanciByMalattia(codiceMalattia)

    End Function

    ''' <summary>
    ''' Restituisce le anagrafiche dei bilanci relativi alla malattia specificata, non ancora compilati per il paziente.
    ''' </summary>
    ''' <param name="codiceMalattia"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAnagraficaBilanciNonCompilatiPazienteByMalattia(codicePaziente As Long, codiceMalattia As String) As List(Of Entities.BilancioAnagrafica)

        Return Me.GenericProvider.AnaBilancio.GetAnagraficaBilanciNonCompilatiPazienteByMalattia(codicePaziente, codiceMalattia)

    End Function

#End Region

#Region " Osservazioni "

    ''' <summary>
    ''' Restituisce la lista di oggetti BilancioOsservazioneAssociata contenente tutte le osservazioni associate al bilancio specificato.
    ''' </summary>
    ''' <param name="numeroBilancio"></param>
    ''' <param name="codiceMalattia"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetOsservazioniAssociate(numeroBilancio As Integer, codiceMalattia As String) As List(Of Entities.BilancioOsservazioneAssociata)

        Return Me.GenericProvider.AnaBilancio.GetOsservazioniAssociate(numeroBilancio, codiceMalattia)

    End Function

    ''' <summary>
    ''' Restituisce la lista di oggetti BilancioOsservazioneAssociata contenente tutte le osservazioni presenti in anagrafica escluse quelle già associate.
    ''' </summary>
    ''' <param name="listOsservazioniAssociate"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetOsservazioniAssociabili(listOsservazioniAssociate As List(Of Entities.BilancioOsservazioneAssociata)) As List(Of Entities.BilancioOsservazioneAnagrafica)

        Dim list As List(Of Entities.BilancioOsservazioneAnagrafica) = Me.GenericProvider.AnaBilancio.GetOsservazioni()

        If Not list.IsNullOrEmpty() AndAlso Not listOsservazioniAssociate.IsNullOrEmpty() Then

            list.RemoveAll(Function(p) listOsservazioniAssociate.Any(Function(q) p.CodiceOsservazione = q.CodiceOsservazione))

        End If

        Return list

    End Function

    ''' <summary>
    ''' Salvataggio osservazioni relative al bilancio specificato. 
    ''' Il metodo effettua anche il log nella tabella relativa alle modifiche dei bilanci.
    ''' </summary>
    ''' <param name="numeroBilancio"></param>
    ''' <param name="codiceMalattia"></param>
    ''' <param name="listAssociate"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SalvaOsservazioniAssociate(numeroBilancio As Integer, codiceMalattia As String, listAssociate As List(Of Entities.BilancioOsservazioneAssociata)) As BizGenericResult

        Dim result As New Biz.BizGenericResult()

        If listAssociate.Any(Function(p) String.IsNullOrWhiteSpace(p.CodiceSezione)) Then
            result.Success = False
            result.Message = "Salvataggio non effettuato. Impostare le sezioni per ogni osservazione."
            Return result
        End If

        Dim listAssociateOriginali As List(Of Entities.BilancioOsservazioneAssociata) = GetOsservazioniAssociate(numeroBilancio, codiceMalattia)

        ' Osservazioni da eliminare (x log)
        Dim listDaEliminare As List(Of Entities.BilancioOsservazioneAssociata) = listAssociateOriginali.
            Where(Function(originale) Not listAssociate.Any(Function(corrente) corrente.CodiceOsservazione = originale.CodiceOsservazione)).
            ToList()

        ' Osservazioni da inserire (x log)
        Dim listDaInserire As List(Of Entities.BilancioOsservazioneAssociata) = listAssociate.
            Where(Function(originale) Not listAssociateOriginali.Any(Function(corrente) corrente.CodiceOsservazione = originale.CodiceOsservazione)).
            ToList()

        Try
            Me.GenericProvider.BeginTransaction()

            ' Eliminazione osservazioni (tutte)
            Me.GenericProvider.AnaBilancio.DeleteOsservazioniAssociate(numeroBilancio, codiceMalattia)

            ' Inserimento osservazioni (tutte)
            If Not listAssociate.IsNullOrEmpty() Then

                For Each osservazione As Entities.BilancioOsservazioneAssociata In listAssociate

                    Me.GenericProvider.AnaBilancio.InsertOsservazioneAssociata(DirectCast(osservazione, Entities.BilancioOsservazione))

                Next

            End If

            Me.GenericProvider.Commit()

        Catch ex As Exception

            Me.GenericProvider.Rollback()

            ex.InternalPreserveStackTrace()
            Throw

        End Try

        ' Log
        Try
            Me.GenericProvider.BeginTransaction()

            LogModificheOsservazioni(listDaInserire, Constants.OperazioneLogOsservazioniBilancio.Inserimento, DateTime.Now)
            LogModificheOsservazioni(listDaEliminare, Constants.OperazioneLogOsservazioniBilancio.Eliminazione, DateTime.Now)

            Me.GenericProvider.Commit()

        Catch ex As Exception

            Me.GenericProvider.Rollback()
            Common.Utility.EventLogHelper.EventLogWrite(ex, String.Format("Errore scrittura log in BizAnaBilanci.SalvaOsservazioniAssociate [numeroBilancio: {0}; codiceMalattia:{1}]", numeroBilancio, codiceMalattia), Me.ContextInfos.IDApplicazione)

        End Try

        result.Success = True
        result.Message = "Salvataggio effettuato"

        Return result

    End Function

#End Region

#Region " Sezioni "

    ''' <summary>
    ''' Restituisce i dati delle sezioni associate al bilancio selezionato
    ''' </summary>
    ''' <param name="numeroBilancio"></param>
    ''' <param name="codiceMalattia"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSezioniBilancio(numeroBilancio As Integer, codiceMalattia As String, includeEmptyRow As Boolean) As List(Of Entities.BilancioSezione)

        Dim list As List(Of Entities.BilancioSezione) = Me.GenericProvider.AnaBilancio.GetAnagraficaSezioni(numeroBilancio, codiceMalattia)

        If includeEmptyRow Then
            list.Insert(0, New Entities.BilancioSezione())
        End If

        Return list

    End Function

    ''' <summary>
    ''' Salvataggio sezioni per il bilancio specificato
    ''' </summary>
    ''' <param name="numeroBilancio"></param>
    ''' <param name="codiceMalattia"></param>
    ''' <param name="listSezioniDaSalvare"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SalvaSezioni(numeroBilancio As Integer, codiceMalattia As String, listSezioniDaSalvare As List(Of Entities.BilancioSezione)) As BizGenericResult

        Dim result As New BizGenericResult()

        Try
            Dim listSezioniOriginali As List(Of Entities.BilancioSezione) = GetSezioniBilancio(numeroBilancio, codiceMalattia, False)

            ' Controllo eliminazione sezioni solo se vuote
            Dim listSezioniDaEliminare As List(Of Entities.BilancioSezione) = listSezioniOriginali.
                Where(Function(originale) Not listSezioniDaSalvare.Any(Function(corrente) corrente.CodiceSezione = originale.CodiceSezione)).
                ToList()

            If Not listSezioniDaEliminare.IsNullOrEmpty() Then

                Dim message As New System.Text.StringBuilder()

                For Each sezioneDaEliminare As Entities.BilancioSezione In listSezioniDaEliminare

                    If Me.GenericProvider.AnaBilancio.CountOsservazioniSezione(sezioneDaEliminare.CodiceSezione) > 0 Then

                        message.AppendFormat(" - {0}{1}", sezioneDaEliminare.DescrizioneSezione, Environment.NewLine)

                    End If

                Next

                If message.Length > 0 Then

                    message.Insert(0, "Salvataggio non effettuato. Le seguenti sezioni non possono essere eliminate perchè hanno osservazioni associate: " + Environment.NewLine)

                    result.Success = False
                    result.Message = message.ToString()

                    Return result

                End If

            End If

            Me.GenericProvider.BeginTransaction()

            ' Eliminazione sezioni
            If Not listSezioniDaEliminare.IsNullOrEmpty() Then

                For Each sezioneDaEliminare As Entities.BilancioSezione In listSezioniDaEliminare

                    Me.GenericProvider.AnaBilancio.DeleteSezione(sezioneDaEliminare.CodiceSezione)

                Next

            End If

            ' Update sezioni
            Dim listSezioniDaModificare As List(Of Entities.BilancioSezione) = listSezioniDaSalvare.
                Where(Function(originale) listSezioniOriginali.Any(Function(corrente) corrente.CodiceSezione = originale.CodiceSezione)).
                ToList()

            If Not listSezioniDaModificare.IsNullOrEmpty() Then

                For Each sezioneDaModificare As Entities.BilancioSezione In listSezioniDaModificare

                    Me.GenericProvider.AnaBilancio.UpdateSezione(sezioneDaModificare)

                Next

            End If

            ' Insert sezioni
            Dim listSezioniDaInserire As List(Of Entities.BilancioSezione) = listSezioniDaSalvare.
                Where(Function(p) String.IsNullOrWhiteSpace(p.CodiceSezione)).
                ToList()

            If Not listSezioniDaInserire.IsNullOrEmpty() Then

                For Each sezioneDaInserire As Entities.BilancioSezione In listSezioniDaInserire

                    Me.GenericProvider.AnaBilancio.InsertSezione(sezioneDaInserire)

                Next

            End If

            Me.GenericProvider.Commit()

        Catch ex As Exception

            Me.GenericProvider.Rollback()

            ex.InternalPreserveStackTrace()
            Throw

        End Try

        Return result

    End Function

#End Region

#End Region

#Region " Private "

    Private Sub LogModificheOsservazioni(list As List(Of Entities.BilancioOsservazioneAssociata), operazione As String, dataLog As DateTime)

        For Each osservazione As Entities.BilancioOsservazioneAssociata In list

            Me.GenericProvider.AnaBilancio.WriteLogOsservazione(osservazione, Convert.ToInt32(operazione), dataLog, Me.ContextInfos.IDUtente)

        Next

    End Sub

#End Region

End Class
