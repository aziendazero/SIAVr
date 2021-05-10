Imports System.Collections.Generic

Imports Onit.OnAssistnet.Data
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities

Public Class BizUtenti
    Inherits BizClass

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos)

        MyBase.New(genericprovider, settings, contextInfos, Nothing)

    End Sub

#End Region

#Region " Public "

    Public Class GetListUtentiCommand
        Public Filtro As String
        Public AppId As String
        Public CodiceAzienda As String
        Public CodiceConsultorio As String
        Public PageIndex As Integer
        Public PageSize As Integer
    End Class

    Public Class GetListUtentiResult
        Public CurrentPageIndex As Integer
        Public TotalCountUtenti As Integer
        Public ListUtenti As List(Of Entities.Utente)
    End Class

    Public Function GetListUtenti(command As GetListUtentiCommand) As GetListUtentiResult

        Dim currentPageIndex As Integer = command.PageIndex

        If Not String.IsNullOrEmpty(command.Filtro) Then
            command.Filtro = command.Filtro.Trim()
        End If

        Dim countUtenti As Integer = Me.GenericProvider.Utenti.CountUtenti(command.Filtro, command.CodiceConsultorio, command.AppId, command.CodiceAzienda)

        Dim startIndex As Integer = currentPageIndex * command.PageSize

        If startIndex > countUtenti - 1 Then
            startIndex = 0
            currentPageIndex = 0
        End If

        Dim pagingOptions As New PagingOptions()
        pagingOptions.StartRecordIndex = startIndex
        pagingOptions.EndRecordIndex = startIndex + command.PageSize

        Dim listUtenti As List(Of Entities.Utente) =
            Me.GenericProvider.Utenti.GetListUtenti(command.Filtro, command.CodiceConsultorio, command.AppId, command.CodiceAzienda, pagingOptions)

        Dim result As New GetListUtentiResult()
        result.TotalCountUtenti = countUtenti
        result.CurrentPageIndex = currentPageIndex
        result.ListUtenti = listUtenti

        Return result

    End Function

    ''' <summary>
    ''' Restituisce la lista dei consultori a cui l'utente è abilitato, controllando che ci sia un solo consultorio di default, ed eventualmente impostandolo.
    ''' </summary>
    ''' <param name="idUtente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetListConsultoriUtente(idUtente As Integer) As List(Of Entities.ConsultorioUtente)

        Return Me.GenericProvider.Utenti.GetListConsultoriUtente(idUtente)

    End Function

    ''' <summary>
    ''' Salvataggio abilitazioni consultori per l'utente specificato. Se, nella lista dei consultori, non è specificato quello di default, viene impostato
    ''' quello indicato nei settings (CNS_DEFAULT). Se tale consultorio non è tra quelli abilitati all'utente, viene preso come default il primo delle lista.
    ''' </summary>
    ''' <param name="idUtente"></param>
    ''' <param name="listConsultoriUtente"></param>
    ''' <remarks></remarks>
    Public Sub SaveConsultoriUtente(idUtente As Integer, listConsultoriUtente As List(Of Entities.ConsultorioUtente))

        Try
            Me.GenericProvider.BeginTransaction()

            ' Eliminazione abilitazioni precedenti
            Me.GenericProvider.Utenti.DeleteConsultoriUtente(idUtente)

            If Not listConsultoriUtente Is Nothing AndAlso listConsultoriUtente.Count > 0 Then

                ' Se non è presente nessun consultorio di default, lo imposta:
                ' Al consultorio di default indicato nei settings, se abilitato all'utente, oppure al primo delle lista.
                If Not listConsultoriUtente.Any(Function(p) p.ConsultorioDefault) Then

                    Dim consultorioDefault As Entities.ConsultorioUtente =
                        listConsultoriUtente.Where(Function(p) p.CodiceConsultorio = Me.Settings.CNS_DEFAULT).FirstOrDefault()

                    If consultorioDefault Is Nothing Then
                        consultorioDefault = listConsultoriUtente(0)
                    End If

                    consultorioDefault.ConsultorioDefault = True

                End If

                ' Inserimento abilitazioni correnti
                For Each consultorioUtente As Entities.ConsultorioUtente In listConsultoriUtente
                    Me.GenericProvider.Utenti.InsertConsultorioUtente(consultorioUtente)
                Next

            End If

            Me.GenericProvider.Commit()

        Catch ex As Exception

            Me.GenericProvider.Rollback()
            ex.InternalPreserveStackTrace()
            Throw

        End Try

    End Sub

    ''' <summary>
    ''' Questo metodo si collega direttamente al DB del manager per controllare se l'utente fa parte del gruppo specificato.
    ''' </summary>
    ''' <param name="idGruppo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsCurrentUserInGroup(idGruppo As String) As Boolean
        '--
        ' N.B. : bisognerebbe usare una funzione ad hoc di Onit.Shared.Manager, ma non c'è.
        '--
        Dim groupId As Int32
        '--
        If String.IsNullOrEmpty(idGruppo) OrElse Not Int32.TryParse(idGruppo, groupId) Then Return False
        '--
        Return Me.GenericProvider.Utenti.IsUserInGroup(Me.ContextInfos.IDUtente, groupId)
        '--
    End Function

    ''' <summary>
    ''' Inserimento di un'associazione utente-consultorio
    ''' </summary>
    ''' <param name="idUtente"></param>
    ''' <param name="codiceConsultorio"></param>
    ''' <remarks></remarks>
    Public Function InserConsultoriotUtente(idUtente As Integer, codiceConsultorio As String) As BizGenericResult

        Dim result As New BizGenericResult()
        result.Success = True
        result.Message = String.Empty

        ' Se l'utente non ha consultori associati oppure, tra quelli associati, nessuno è il consultorio di default, imposto come default quello che sto inserendo.
        Dim isDefault As Boolean = True

        Dim listConsultoriUtente As List(Of Entities.ConsultorioUtente) = GetListConsultoriUtente(idUtente)

        If Not listConsultoriUtente.IsNullOrEmpty() Then

            If listConsultoriUtente.Any(Function(c) c.CodiceConsultorio = codiceConsultorio) Then
                result.Success = False
                result.Message = "Utente già associato al centro vaccinale"
                Return result
            End If

            isDefault = Not listConsultoriUtente.Any(Function(c) c.ConsultorioDefault)

        End If

        Dim consultorioUtente As New Entities.ConsultorioUtente()
        consultorioUtente.CodiceConsultorio = codiceConsultorio
        consultorioUtente.IdUtente = idUtente
        consultorioUtente.ConsultorioDefault = isDefault

        Me.GenericProvider.Utenti.InsertConsultorioUtente(consultorioUtente)

        Return result

    End Function

    ''' <summary>
    ''' Elimina l'associazione tra il consultorio e l'utente specificati
    ''' </summary>
    ''' <param name="idUtente"></param>
    ''' <param name="codiceConsultorio"></param>
    ''' <remarks></remarks>
    Public Sub DeleteConsultorioUtente(idUtente As Integer, codiceConsultorio As String)

        Me.GenericProvider.Utenti.DeleteConsultorioUtente(codiceConsultorio, idUtente)

    End Sub

    Public Function Getutente(idutente As Integer) As Entities.Utente
        Return GenericProvider.Utenti.GetUtente(idutente)
    End Function

    ''' <summary>
    ''' Restituisce il codice fiscale dell'utente specificato
    ''' </summary>
    ''' <param name="idUtente"></param>
    ''' <returns></returns>
    Public Function GetCodiceFiscaleUtente(idUtente As Long) As String

        Return GenericProvider.Utenti.GetCodiceFiscaleUtente(idUtente)

    End Function

    Public Function InserimentoConsensi(idUtente As Long, codicePaziente As Long, approvazione As String) As ResultSetPost

        Return GenericProvider.Utenti.InserimentoConsensi(idUtente, codicePaziente, String.Empty, approvazione)

    End Function

    ''' <summary>
    ''' Inserimento consenso trattamento dati per utente
    ''' </summary>
    ''' <param name="idUtente"></param>
    ''' <param name="codicePaziente"></param>
    ''' <param name="approvazione"></param>
    ''' <returns></returns>
    Public Function InserimentoConsensoByCodiceLocale(codicePaziente As Long, accettato As Boolean) As ResultSetPost

        Return GenericProvider.Utenti.InserimentoConsensi(ContextInfos.IDUtente, codicePaziente, String.Empty, If(accettato, "S", "N"))

    End Function

    ''' <summary>
    ''' Inserimento consenso trattamento dati per utente
    ''' </summary>
    ''' <param name="codiceAusiliarioPaziente"></param>
    ''' <param name="accettato"></param>
    ''' <returns></returns>
    Public Function InserimentoConsensoByCodiceAusiliario(codiceAusiliarioPaziente As String, accettato As Boolean) As ResultSetPost

        Return GenericProvider.Utenti.InserimentoConsensi(ContextInfos.IDUtente, 0, codiceAusiliarioPaziente, If(accettato, "S", "N"))

    End Function

#End Region

End Class