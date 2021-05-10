Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL

Public Class BizAttivita
    Inherits BizClass

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos)

        MyBase.New(genericprovider, settings, contextInfos, Nothing)

    End Sub

#End Region

#Region " Comparers "

    ''' <summary>
    ''' Comparer per eseguire il distinct tra gli operatori associati ad un'attività
    ''' </summary>
    ''' <remarks></remarks>
    Private Class AttivitaRegistrazioneOperatoriComparer
        Implements IEqualityComparer(Of Entities.AttivitaRegistrazioneOperatore)

        Public Function Equals1(x As Entities.AttivitaRegistrazioneOperatore, y As Entities.AttivitaRegistrazioneOperatore) As Boolean Implements IEqualityComparer(Of Entities.AttivitaRegistrazioneOperatore).Equals
            x.IdAttivitaRegistrazione = y.IdAttivitaRegistrazione
            x.IdOperatore = y.IdOperatore
        End Function

        Public Function GetHashCode1(obj As Entities.AttivitaRegistrazioneOperatore) As Integer Implements IEqualityComparer(Of Entities.AttivitaRegistrazioneOperatore).GetHashCode
            Return obj.IdAttivitaRegistrazione.GetHashCode() + obj.IdOperatore.GetHashCode()
        End Function

    End Class

#End Region

#Region " Public "

#Region " Anagrafe Tipi Attività "

    ''' <summary>
    ''' Restituisce i tipi attività presenti in anagrafe, filtrati, ordinati e paginati (se specificato).
    ''' Se non specificato, restituisce tutti i tipi attività, senza filtri, senza paginazione e ordinati per codice.
    ''' </summary>
    ''' <param name="command"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetElencoAttivitaTipo(command As RicercaCommand(Of Entities.AttivitaTipo.Ordinamento)) As IEnumerable(Of Entities.AttivitaTipo)

        SetRicercaCommandIfNull(command)

        Return Me.GenericProvider.Attivita.GetElencoAttivitaTipo(
            command.FiltroGenerico, command.FiltroSoloValidi, command.CampoOrdinamento, GetVersoOrdinamento(command), CreatePagingOptions(command))

    End Function

    ''' <summary>
    ''' Restituisce il numero di tipi attività in base al filtro specificato
    ''' </summary>
    ''' <param name="filtroGenerico"></param>
    ''' <param name="filtroSoloValidi"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountElencoAttivitaTipo(filtroGenerico As String, filtroSoloValidi As Boolean) As Integer

        Return Me.GenericProvider.Attivita.CountElencoAttivitaTipo(filtroGenerico, filtroSoloValidi)

    End Function

    ''' <summary>
    ''' Restituisce il tipo attività specificato in base al codice
    ''' </summary>
    ''' <param name="codiceAttivitaTipo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAttivitaTipo(codiceAttivitaTipo As String) As Entities.AttivitaTipo

        Return Me.GenericProvider.Attivita.GetAttivitaTipo(codiceAttivitaTipo)

    End Function

    ''' <summary>
    ''' Salvataggio tipo attività. Se è un inserimento, controlla che il codice non sia già presente.
    ''' </summary>
    ''' <param name="attivitaTipo"></param>
    ''' <param name="isInsert"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SaveAttivitaTipo(attivitaTipo As Entities.AttivitaTipo, isInsert As Boolean) As BizGenericResult

        If isInsert Then
            attivitaTipo.Obsoleto = "N"
            Return InsertAttivitaTipo(attivitaTipo)
        End If

        Return UpdateAttivitaTipo(attivitaTipo)

    End Function

    ''' <summary>
    ''' Eliminazione logica del tipo attività specificata.
    ''' </summary>
    ''' <param name="codiceTipoAttivita"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function EliminaAttivitaTipo(codiceTipoAttivita As String) As BizGenericResult

        Dim item As Entities.AttivitaTipo = GetAttivitaTipo(codiceTipoAttivita)
        item.Obsoleto = "S"

        Me.GenericProvider.Attivita.UpdateAttivitaTipo(item)

        Return New BizGenericResult(True, String.Empty)

    End Function

    ''' <summary>
    ''' Inserimento tipo attività
    ''' </summary>
    ''' <param name="attivitaTipo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function InsertAttivitaTipo(attivitaTipo As Entities.AttivitaTipo) As BizGenericResult

        Dim result As New BizGenericResult()

        If attivitaTipo Is Nothing Then
            result.Success = False
            result.Message = "Nessun dato da inserire."
            Return result
        End If

        If String.IsNullOrWhiteSpace(attivitaTipo.Codice) Then
            result.Success = False
            result.Message = "Codice non presente"
            Return result
        End If

        If String.IsNullOrWhiteSpace(attivitaTipo.Descrizione) Then
            result.Success = False
            result.Message = "Descrizione non presente"
            Return result
        End If

        attivitaTipo.Codice = attivitaTipo.Codice.ToUpper()

        Dim attivitaDuplicata As Entities.AttivitaTipo = GetAttivitaTipo(attivitaTipo.Codice)
        If Not attivitaDuplicata Is Nothing Then
            result.Success = False
            result.Message = "Codice tipo attività già presente"
            Return result
        End If

        Me.GenericProvider.Attivita.InsertAttivitaTipo(attivitaTipo)

        result.Success = True
        result.Message = String.Empty

        Return result

    End Function

    ''' <summary>
    ''' Modifica tipo attività (solo descrizione)
    ''' </summary>
    ''' <param name="tipoAttivita"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdateAttivitaTipo(attivitaTipo As Entities.AttivitaTipo) As BizGenericResult

        Dim result As New BizGenericResult()

        If attivitaTipo Is Nothing Then
            result.Success = False
            result.Message = "Nessun dato da modificare."
            Return result
        End If

        If String.IsNullOrWhiteSpace(attivitaTipo.Codice) Then
            result.Success = False
            result.Message = "Codice non presente"
            Return result
        End If

        If String.IsNullOrWhiteSpace(attivitaTipo.Descrizione) Then
            result.Success = False
            result.Message = "Descrizione non presente"
            Return result
        End If

        Me.GenericProvider.Attivita.UpdateAttivitaTipo(attivitaTipo)

        result.Success = True
        result.Message = String.Empty

        Return result

    End Function

#End Region

#Region " Anagrafe Attività "

    ''' <summary>
    ''' Restituisce le attività presenti in anagrafe in base ai filtri specificati
    ''' </summary>
    ''' <param name="command"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetElencoAttivitaAnagrafe(command As RicercaCommand(Of Entities.AttivitaAnagrafe.Ordinamento), ricerca As Boolean) As IEnumerable(Of Entities.AttivitaAnagrafe)

        SetRicercaCommandIfNull(command)

        Dim idutente As Integer = 0
        Dim appId As String = String.Empty

        If ricerca Then
            idutente = ContextInfos.IDUtente
            appId = ContextInfos.IDApplicazione
        End If

        Return Me.GenericProvider.Attivita.GetElencoAttivitaAnagrafe(
            command.FiltroGenerico, command.FiltroSoloValidi, command.CampoOrdinamento, GetVersoOrdinamento(command), CreatePagingOptions(command), idutente, appId)

    End Function

    ''' <summary>
    ''' Restituisce il numero di attività in base al filtro specificato
    ''' </summary>
    ''' <param name="filtroGenerico"></param>
    ''' <param name="filtroSoloValidi"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountElencoAttivitaAnagrafe(filtroGenerico As String, filtroSoloValidi As Boolean) As Integer

        Return Me.GenericProvider.Attivita.CountElencoAttivitaAnagrafe(filtroGenerico, filtroSoloValidi, ContextInfos.IDUtente, ContextInfos.IDApplicazione)

    End Function

    ''' <summary>
    ''' Restituisce l'anagrafica dell'attività specificata
    ''' </summary>
    ''' <param name="id"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAttivitaAnagrafe(id As Integer) As Entities.AttivitaAnagrafe

        Return GenericProvider.Attivita.GetAttivitaAnagrafe(id)

    End Function

    ''' <summary>
    ''' Salvataggio tipo attività. Se è un inserimento, controlla che il codice non sia già presente.
    ''' </summary>
    ''' <param name="attivitaAnagrafe"></param>
    ''' <param name="isInsert"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SaveAttivitaAnagrafe(attivitaAnagrafe As Entities.AttivitaAnagrafe, isInsert As Boolean) As BizGenericResult

        If isInsert Then
            attivitaAnagrafe.Obsoleto = "N"
            Return InsertAttivitaAnagrafe(attivitaAnagrafe)
        End If

        Return UpdateAttivitaAnagrafe(attivitaAnagrafe)

    End Function

    ''' <summary>
    ''' Eliminazione logica dell'attività specificata.
    ''' </summary>
    ''' <param name="idAttivitaAnagrafe"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function EliminaAttivitaAnagrafe(idAttivitaAnagrafe As Integer) As BizGenericResult

        Dim item As Entities.AttivitaAnagrafe = GetAttivitaAnagrafe(idAttivitaAnagrafe)
        item.Obsoleto = "S"

        Me.GenericProvider.Attivita.UpdateAttivitaAnagrafe(item)

        Return New BizGenericResult(True, String.Empty)

    End Function

    ''' <summary>
    ''' Restituisce true se l'attività specificata è relativa alle scuole
    ''' </summary>
    ''' <param name="idAttivitaAnagrafe"></param>
    ''' <returns></returns>
    Public Function IsAttivitaAnagrafePerScuole(idAttivitaAnagrafe As Integer) As Boolean

        Dim attivitaAnagrafe As Entities.AttivitaAnagrafe = GetAttivitaAnagrafe(idAttivitaAnagrafe)

        If attivitaAnagrafe Is Nothing Then
            Return False
        End If

        Return (attivitaAnagrafe.Scuola = "S")

    End Function

    ''' <summary>
    ''' Inserisce in anagrafe l'attività specificata. Controlla che il codice non sia già presente.
    ''' </summary>
    ''' <param name="attivitaAnagrafe"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function InsertAttivitaAnagrafe(attivitaAnagrafe As Entities.AttivitaAnagrafe) As BizGenericResult

        Dim result As New BizGenericResult()

        If attivitaAnagrafe Is Nothing Then
            result.Success = False
            result.Message = "Nessun dato da inserire."
            Return result
        End If

        If String.IsNullOrWhiteSpace(attivitaAnagrafe.Codice) Then
            result.Success = False
            result.Message = "Codice non presente"
            Return result
        End If

        If String.IsNullOrWhiteSpace(attivitaAnagrafe.Descrizione) Then
            result.Success = False
            result.Message = "Descrizione non presente"
            Return result
        End If

        attivitaAnagrafe.Codice = attivitaAnagrafe.Codice.ToUpper()

        Dim attivitaDuplicata As Boolean = False

        Dim list As IEnumerable(Of Entities.AttivitaAnagrafe) =
            Me.GenericProvider.Attivita.GetAttivitaAnagrafeByCodice(attivitaAnagrafe.Codice)

        If Not list.IsNullOrEmpty() Then
            result.Success = False
            result.Message = "Codice attività già presente"
            Return result
        End If

        If attivitaAnagrafe.Id <= 0 Then
            attivitaAnagrafe.Id = Me.GenericProvider.Attivita.GetNextIdAttivitaAnagrafe()
        End If

        Me.GenericProvider.Attivita.InsertAttivitaAnagrafe(attivitaAnagrafe)

        result.Success = True
        result.Message = String.Empty

        Return result

    End Function

    ''' <summary>
    ''' Modifica l'anagrafica dell'attività specificata. Codice e Descrizione sono obbligatori.
    ''' </summary>
    ''' <param name="attivitaAnagrafe"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdateAttivitaAnagrafe(attivitaAnagrafe As Entities.AttivitaAnagrafe) As BizGenericResult

        Dim result As New BizGenericResult()

        If attivitaAnagrafe Is Nothing Then
            result.Success = False
            result.Message = "Nessun dato da modificare."
            Return result
        End If

        If attivitaAnagrafe.Id <= 0 Then
            result.Success = False
            result.Message = "Id non presente"
            Return result

        End If
        If String.IsNullOrWhiteSpace(attivitaAnagrafe.Codice) Then
            result.Success = False
            result.Message = "Codice non presente"
            Return result
        End If

        If String.IsNullOrWhiteSpace(attivitaAnagrafe.Descrizione) Then
            result.Success = False
            result.Message = "Descrizione non presente"
            Return result
        End If

        Me.GenericProvider.Attivita.UpdateAttivitaAnagrafe(attivitaAnagrafe)

        result.Success = True
        result.Message = String.Empty

        Return result

    End Function

#End Region

#Region " Anagrafe Variabili e Utenti "

    Public Class RicercaVariabiliCommand(Of T As Structure)
        Inherits RicercaCommand(Of T)

        Public Property IdAttivita As Integer

    End Class

    ''' <summary>
    ''' Restituisce tutte le variabili (osservazioni di tipo "ATTIVITA") presenti in anagrafe e non obsolete.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetElencoVariabiliDaAssociare(command As RicercaVariabiliCommand(Of Entities.AttivitaVariabileDaAssociare.Ordinamento)) As List(Of Entities.AttivitaVariabileDaAssociare)

        SetRicercaCommandIfNull(command)

        Return Me.GenericProvider.Attivita.GetElencoVariabiliDaAssociare(
            command.IdAttivita, command.FiltroGenerico, command.FiltroSoloValidi,
            command.CampoOrdinamento, GetVersoOrdinamento(command), CreatePagingOptions(command)).ToList()

    End Function

    Public Function GetElencoUtentiDaAssociare(command As RicercaVariabiliCommand(Of Entities.AttivitaUtentiDaAssociare.Ordinamento)) As List(Of Entities.AttivitaUtentiDaAssociare)

        SetRicercaCommandIfNull(command)

        Return Me.GenericProvider.Attivita.GetElencoUtentiDaAssociare(
            command.IdAttivita, command.FiltroGenerico, command.FiltroSoloValidi,
            command.CampoOrdinamento, GetVersoOrdinamento(command), CreatePagingOptions(command)).ToList()

    End Function

    ''' <summary>
    ''' Restituisce il numero di variabili in base al filtro specificato
    ''' </summary>
    ''' <param name="idAttivita"></param>
    ''' <param name="filtroGenerico"></param>
    ''' <param name="filtroSoloValidi"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountVariabiliDaAssociare(idAttivita As Integer?, filtroGenerico As String, filtroSoloValidi As Boolean) As Integer

        Return Me.GenericProvider.Attivita.CountElencoVariabiliDaAssociare(idAttivita, filtroGenerico, filtroSoloValidi)

    End Function

    ''' <summary>
    ''' Restituisce le variabili associate all'attività specificata
    ''' </summary>
    ''' <param name="idAttivita"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetElencoVariabiliAssociate(command As RicercaVariabiliCommand(Of Entities.AttivitaVariabileAssociata.Ordinamento)) As List(Of Entities.AttivitaVariabileAssociata)

        SetRicercaCommandIfNull(command)

        'Return Me.GenericProvider.Attivita.GetElencoVariabiliAssociate(
        '    command.IdAttivita, command.FiltroGenerico, command.CampoOrdinamento, GetVersoOrdinamento(command), CreatePagingOptions(command)).ToList()

        Return Me.GenericProvider.Attivita.GetElencoVariabiliAssociate(command.IdAttivita, Nothing, Nothing, Nothing, Nothing).ToList()

    End Function

    Public Function GetElencoUtentiAssociati(command As RicercaVariabiliCommand(Of Entities.AttivitaUtenteAssociato.Ordinamento)) As List(Of Entities.AttivitaUtenteAssociato)

        SetRicercaCommandIfNull(command)

        Return Me.GenericProvider.Attivita.GetElencoUtentiAssociati(command.IdAttivita, Nothing, Nothing, Nothing, CreatePagingOptions(command)).ToList()

    End Function

    ''' <summary>
    ''' Restituisce il numero di variabili associate all'attività, in base al filtro
    ''' </summary>
    ''' <param name="idAttivita"></param>
    ''' <param name="filtroGenerico"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountVariabiliAssociate(idAttivita As Integer, filtroGenerico As String) As Integer

        Return Me.GenericProvider.Attivita.CountElencoVariabiliAssociate(idAttivita, filtroGenerico)

    End Function

    ''' <summary>
    ''' Restituisce il numero d'ordine più alto per le variabili dell'attività specificata
    ''' </summary>
    ''' <param name="idAttivita"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMaxOrdineVariabiliAssociate(idAttivita As Integer) As Integer

        Return Me.GenericProvider.Attivita.GetMaxOrdineVariabiliAssociate(idAttivita)

    End Function

    Public Function InsertVariabileAssociata(idAttivita As Integer, codiceVariabile As String, ordine As Integer) As BizGenericResult

        Dim result As New BizGenericResult()

        Dim variabileAssociata As New Entities.AttivitaVariabileAssociata()

        variabileAssociata.IdAttivita = idAttivita
        variabileAssociata.CodiceVariabile = codiceVariabile
        variabileAssociata.Ordine = ordine

        variabileAssociata.IdVariabile = Me.GenericProvider.Attivita.GetNextIdVariabileAssociata()

        Me.GenericProvider.Attivita.InsertVariabileAssociata(variabileAssociata)

        result.Success = True
        result.Message = String.Empty

        Return result

    End Function

    ''' <summary>
    ''' Inserimento associazione attivita, utente e appId
    ''' </summary>
    ''' <param name="idAttivita"></param>
    ''' <param name="idUtente"></param>
    ''' <param name="appId"></param>
    ''' <returns></returns>
    Public Function InsertUtenteAssociato(idAttivita As Integer, idUtente As Integer, appId As String) As BizGenericResult

        Dim result As New BizGenericResult()

        Dim utenteAssociato As New Entities.AttivitaUtenteAssociato()

        utenteAssociato.IdAttivita = idAttivita
        utenteAssociato.IdUtenti = idUtente
        utenteAssociato.AppId = appId

        utenteAssociato.Id = Me.GenericProvider.Attivita.GetNextIdUtenteAssociato()

        Me.GenericProvider.Attivita.InsertUtenteAssociato(utenteAssociato)

        result.Success = True
        result.Message = String.Empty

        Return result

    End Function

    ''' <summary>
    ''' Riordina le variabili associate a idAttività, in base all'ordine specificato per la variabile indicata.
    ''' </summary>
    ''' <param name="idAttivita"></param>
    ''' <param name="idAttivitaVariabile"></param>
    ''' <param name="ordine">Nuovo valore dell'ordine per la variabile</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateOrdineVariabileAssociata(idAttivita As Integer, idAttivitaVariabile As Integer, ordine As Integer) As BizGenericResult

        Dim result As New BizGenericResult()

        ' N.B. : considerando che le var associate ad un'attività non sono tantissime, le carico tutte e gli riassegno l'ordine.
        '        La cosa migliore sarebbe modificare, ogni volta, solo le due var coinvolte nello spostamento.
        Dim list As List(Of Entities.AttivitaVariabileAssociata) =
            Me.GenericProvider.Attivita.GetElencoVariabiliAssociate(idAttivita, Nothing, Nothing, Nothing, Nothing).ToList()

        Dim varSelezionata As Entities.AttivitaVariabileAssociata = list.Single(Function(p) p.IdVariabile = idAttivitaVariabile)
        Dim varDaSpostare As Entities.AttivitaVariabileAssociata = list.Where(Function(p) p.Ordine = ordine).FirstOrDefault()

        Dim ordineVarDaSpostare As Integer = varSelezionata.Ordine

        varSelezionata.Ordine = ordine

        If Not varDaSpostare Is Nothing Then
            varDaSpostare.Ordine = ordineVarDaSpostare
        End If

        list = list.OrderBy(Function(p) p.Ordine).ToList()

        For i As Integer = 0 To list.Count - 1
            list(i).Ordine = i + 1
            Me.GenericProvider.Attivita.UpdateOrdineVariabileAssociata(list(i).IdVariabile, list(i).Ordine)
        Next

        result.Success = True
        result.Message = String.Empty

        Return result

    End Function
    Public Function UpdateObbligatorioVariabileAssociata(idAttivita As Integer, idAttivitaVariabile As Integer, obbligatorio As String) As BizGenericResult

        Dim result As New BizGenericResult()



        Me.GenericProvider.Attivita.UpdateObbligatorioVariabileAssociata(idAttivitaVariabile, obbligatorio)

        result.Success = True
        result.Message = String.Empty

        Return result

    End Function

    ''' <summary>
    ''' Rimuove la variabile dall'attività (in base all'id univoco di associazione tra le due) e ricalcola l'ordine delle variabili rimaste
    ''' </summary>
    ''' <param name="idAttivitaVariabile"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteVariabileAssociata(idAttivita As Integer, idAttivitaVariabile As Integer) As BizGenericResult

        Dim result As New BizGenericResult()

        ' Rimozione associazione
        Me.GenericProvider.Attivita.DeleteVariabileAssociata(idAttivitaVariabile)

        ' Caricamento variabili rimaste
        Dim list As List(Of Entities.AttivitaVariabileAssociata) =
            Me.GenericProvider.Attivita.GetElencoVariabiliAssociate(idAttivita, Nothing, Nothing, Nothing, Nothing).ToList()

        ' Ricalcolo ordine
        For i As Integer = 0 To list.Count - 1
            list(i).Ordine = i + 1
            Me.GenericProvider.Attivita.UpdateOrdineVariabileAssociata(list(i).IdVariabile, list(i).Ordine)
        Next

        result.Success = True
        result.Message = String.Empty

        Return result

    End Function

    Public Function DeleteUtenteAssociato(idAttivita As Integer, id As Integer) As BizGenericResult

        Dim result As New BizGenericResult()

        ' Rimozione associazione
        Me.GenericProvider.Attivita.DeleteUtenteAssociato(id)

        ' Caricamento variabili rimaste
        Dim list As List(Of Entities.AttivitaUtenteAssociato) =
            Me.GenericProvider.Attivita.GetElencoUtentiAssociati(idAttivita, Nothing, Nothing, Nothing, Nothing).ToList()

        result.Success = True
        result.Message = String.Empty

        Return result

    End Function

#End Region

#Region " Registrazione Attività "

    Public Class RicercaAttivitaRegistrazioneCommand
        Inherits RicercaCommand(Of Entities.AttivitaRegistrazione.Ordinamento)

        Public Property FiltroDataEsecuzioneInizio As DateTime?
        Public Property FiltroDataEsecuzioneFine As DateTime?
        Public Property FiltroDataRegistrazioneInizio As DateTime?
        Public Property FiltroDataRegistrazioneFine As DateTime?
        Public Property FiltroIdUtenteRegistrazione As Long?

    End Class

    ''' <summary>
    ''' Restituisce le attività registrate, in base ai filtri specificati
    ''' </summary>
    ''' <param name="command"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetElencoAttivitaRegistrazione(command As RicercaAttivitaRegistrazioneCommand) As IEnumerable(Of Entities.AttivitaRegistrazione)

        SetRicercaCommandIfNull(command)

        ' Restituisce solo le attività non eliminate
        command.FiltroSoloValidi = True

        Return GenericProvider.Attivita.GetElencoAttivitaRegistrazione(
            command.FiltroGenerico, command.FiltroSoloValidi, ContextInfos.IDApplicazione,
            command.FiltroDataEsecuzioneInizio, command.FiltroDataEsecuzioneFine,
            command.FiltroDataRegistrazioneInizio, command.FiltroDataRegistrazioneFine, command.FiltroIdUtenteRegistrazione,
            command.CampoOrdinamento, GetVersoOrdinamento(command), CreatePagingOptions(command), ContextInfos.IDUtente, ContextInfos.CodiceUsl)

    End Function

    ''' <summary>
    ''' Restituisce il numero di attività registrate, in base ai filtri specificati
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountElencoAttivitaRegistrazione(command As RicercaAttivitaRegistrazioneCommand) As Integer

        Return GenericProvider.Attivita.CountElencoAttivitaRegistrazione(
            command.FiltroGenerico, command.FiltroSoloValidi, command.FiltroDataEsecuzioneInizio, command.FiltroDataEsecuzioneFine,
            command.FiltroDataRegistrazioneInizio, command.FiltroDataRegistrazioneFine, command.FiltroIdUtenteRegistrazione, ContextInfos.CodiceUsl)

    End Function

    ''' <summary>
    ''' Restituisce l'attività specificata
    ''' </summary>
    ''' <param name="idAttivitaRegistrazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAttivitaRegistrazione(idAttivitaRegistrazione As Integer) As Entities.AttivitaRegistrazione

        Return GenericProvider.Attivita.GetAttivitaRegistrazione(idAttivitaRegistrazione, Me.ContextInfos.IDApplicazione, ContextInfos.IDUtente)

    End Function

    ''' <summary>
    ''' Eliminazione logica dell'attività specificata.
    ''' </summary>
    ''' <param name="idAttivitaAnagrafe"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function EliminaAttivitaRegistrazione(idAttivita As Integer) As BizGenericResult

        Dim item As Entities.AttivitaRegistrazione = GetAttivitaRegistrazione(idAttivita)

        item.DataEliminazione = DateTime.Now
        item.IdUtenteEliminazione = Me.ContextInfos.IDUtente

        Dim result As New BizGenericResult()

        GenericProvider.Attivita.UpdateAttivitaRegistrazione(item)

        result.Success = True
        result.Message = String.Empty

        Return result

    End Function
    Protected Function CheckCampoNumerico(valore As String) As Boolean

        If String.IsNullOrWhiteSpace(valore) Then Return False

        Dim regEx As New System.Text.RegularExpressions.Regex("^[-0-9]\d{0,9}(.\d{1,2})?%?$")
        Return regEx.IsMatch(valore)

    End Function

    ''' <summary>
    ''' Salva la rilevazione dell'attività e degli eventuali valori specificati.
    '''  Controlla la presenza dei campi obbligatori (per l'attività).
    ''' </summary>
    ''' <param name="attivitaRegistrazione"></param>
    ''' <param name="valori"></param>
    ''' <param name="operatori"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SaveAttivitaRegistrazione(attivitaRegistrazione As Entities.AttivitaRegistrazione, valori As List(Of Entities.AttivitaRegistrazioneValore), operatori As List(Of Entities.AttivitaRegistrazioneOperatore), duplica As Boolean) As BizGenericResult

        Dim result As New BizGenericResult()

        If attivitaRegistrazione Is Nothing Then
            result.Success = False
            result.Message = "Nessun dato da registrare."
            Return result
        End If

        If attivitaRegistrazione.IdAttivitaAnagrafe <= 0 Then
            result.Success = False
            result.Message = "Attività non specificata."
            Return result
        End If

        If attivitaRegistrazione.DataAttivita = DateTime.MinValue Then
            result.Success = False
            result.Message = "Data di esecuzione dell'attività non specificata."
            Return result
        End If

        ' Controlli di obbligatorieta delle attività
        If Not valori.IsNullOrEmpty() Then

            For Each att As Entities.AttivitaRegistrazioneValore In valori

                ' verifica per campi con codifica nel caso essi siano definiti obbligatori.
                If String.IsNullOrWhiteSpace(att.CodiceRisposta) AndAlso att.Obbligatorio AndAlso (att.TipoRisposta = Constants.TipoRispostaOsservazioneBilancio.CodificataMultipla Or att.TipoRisposta = Constants.TipoRispostaOsservazioneBilancio.CodificataSingola) Then
                    result.Success = False
                    result.Message = String.Format("La variabile {0} è obbligatoria.", att.DescrizioneVariabile)
                    Return result
                End If

                ' verifiche per i campi testuali
                If att.TipoRisposta = Constants.TipoRispostaOsservazioneBilancio.TestoLibero Then

                    ' Verifico che il testo sia valorizzato se è obbligatorio
                    If String.IsNullOrWhiteSpace(att.ValoreRisposta) AndAlso att.Obbligatorio Then
                        result.Success = False
                        result.Message = String.Format("La variabile {0} è obbligatoria.", att.DescrizioneVariabile)
                        Return result
                    End If

                    ' verifico se il testo deve essere di tipo numerico
                    If att.TipoDatiRisposta = Constants.TipoDatiRispostaOsservazioneBilancio.Numerica AndAlso Not String.IsNullOrWhiteSpace(att.ValoreRisposta) AndAlso Not CheckCampoNumerico(att.ValoreRisposta) Then
                        result.Success = False
                        result.Message = String.Format("La variabile {0} non è numerica.", att.DescrizioneVariabile)
                        Return result
                    End If

                End If

            Next

        End If

        Dim isInsertAttivita As Boolean = attivitaRegistrazione.Id <= 0

        ' Controlli da fare se non siamo in inserimento o se siamo in duplicazione
        If Not isInsertAttivita OrElse duplica Then

            If IsAttivitaAnagrafePerScuole(attivitaRegistrazione.IdAttivitaAnagrafe) AndAlso String.IsNullOrWhiteSpace(attivitaRegistrazione.CodiceScuola) Then
                result.Success = False
                result.Message = "Il nome della scuola è obbligatorio per questa attività."
                Return result
            End If

            If operatori.IsNullOrEmpty() Then
                result.Success = False
                result.Message = "Deve essere specificato almeno un operatore che ha effettuato l'attività."
                Return result
            End If

        End If

        Dim ownTransaction As Boolean = False

        Try
            If GenericProvider.Transaction Is Nothing Then
                GenericProvider.BeginTransaction()
                ownTransaction = True
            End If

#Region " Salvataggio Attività "

            ' --- Salvataggio attività --- '
            If isInsertAttivita Then
                '--
                ' INSERT
                '--
                attivitaRegistrazione.Id = GenericProvider.Attivita.GetNextIdAttivitaRegistrazione()
                attivitaRegistrazione.DataRegistrazione = DateTime.Now
                attivitaRegistrazione.IdUtenteRegistrazione = ContextInfos.IDUtente

                GenericProvider.Attivita.InsertAttivitaRegistrazione(attivitaRegistrazione, ContextInfos.CodiceUsl)

            Else
                '--
                ' UPDATE
                '--
                Dim old As Entities.AttivitaRegistrazione = GetAttivitaRegistrazione(attivitaRegistrazione.Id)

                ' In maschera, i dati modificabili dell'entity attivitaRegistrazione sono Data e Scuola
                If attivitaRegistrazione.DataAttivita <> old.DataAttivita OrElse attivitaRegistrazione.CodiceScuola <> old.CodiceScuola Then

                    attivitaRegistrazione.DataVariazione = DateTime.Now
                    attivitaRegistrazione.IdUtenteVariazione = ContextInfos.IDUtente

                    GenericProvider.Attivita.UpdateAttivitaRegistrazione(attivitaRegistrazione)

                End If
            End If
#End Region

#Region " Salvataggio Valori "

            ' --- Salvataggio valori --- '
            If Not valori.IsNullOrEmpty() Then

                Dim dataCorrente As DateTime = DateTime.Now

                If isInsertAttivita Then

                    For Each valore As Entities.AttivitaRegistrazioneValore In valori

                        valore.IdAttivitaRegistrazioneValore = Me.GenericProvider.Attivita.GetNextIdAttivitaRegistrazioneValore()
                        valore.DataRegistrazione = dataCorrente
                        valore.IdUtenteRegistrazione = Me.ContextInfos.IDUtente
                        valore.IdAttivitaRegistrazione = attivitaRegistrazione.Id

                        Me.GenericProvider.Attivita.InsertAttivitaRegistrazioneValore(valore)

                    Next

                Else

                    For Each valore As Entities.AttivitaRegistrazioneValore In valori

                        Dim oldValore As Entities.AttivitaRegistrazioneValore = Nothing

                        If valore.IdAttivitaRegistrazioneValore.HasValue Then

                            oldValore = Me.GenericProvider.Attivita.GetAttivitaRegistrazioneValore(valore.IdAttivitaRegistrazioneValore.Value)

                        End If

                        If oldValore Is Nothing Then

                            valore.IdAttivitaRegistrazioneValore = Me.GenericProvider.Attivita.GetNextIdAttivitaRegistrazioneValore()
                            valore.DataRegistrazione = dataCorrente
                            valore.IdUtenteRegistrazione = Me.ContextInfos.IDUtente

                            Me.GenericProvider.Attivita.InsertAttivitaRegistrazioneValore(valore)

                        Else

                            If valore.CodiceRisposta <> oldValore.CodiceRisposta OrElse
                               (String.IsNullOrEmpty(valore.CodiceRisposta) AndAlso String.IsNullOrEmpty(oldValore.CodiceRisposta) AndAlso valore.ValoreRisposta <> oldValore.ValoreRisposta) Then

                                valore.DataVariazione = dataCorrente
                                valore.IdUtenteVariazione = Me.ContextInfos.IDUtente

                                Me.GenericProvider.Attivita.UpdateAttivitaRegistrazioneValore(valore)

                            End If

                        End If

                    Next

                End If

            End If
#End Region

#Region " Salvataggio Operatori "

            ' --- Salvataggio operatori --- '
            Dim messageOperatori As New Text.StringBuilder()

            ' Lista operatori associati all'attività, prima della modifica
            Dim operatoriOriginal As List(Of Entities.AttivitaRegistrazioneOperatore) =
                Me.GenericProvider.Attivita.GetOperatoriAttivitaRegistrazione(attivitaRegistrazione.Id)

            If operatori.IsNullOrEmpty() Then

                ' Gli operatori correnti sono nulli => Eliminazione di tutti gli operatori eventualmente presenti (viene eliminata l'associazione tra l'operatore e l'attività corrente)
                If Not operatoriOriginal.IsNullOrEmpty() Then

                    For Each operatore As Entities.AttivitaRegistrazioneOperatore In operatoriOriginal
                        Me.GenericProvider.Attivita.DeleteOperatoreAttivitaRegistrazione(attivitaRegistrazione.Id, operatore.IdOperatore)
                    Next

                End If

            Else

                ' Per ogni operatore da inserire (avente id == -1), controllo se è già presente in anagrafe. Se è già presente, non lo devo inserire di nuovo.
                For i As Integer = operatori.Count - 1 To 0 Step -1

                    Dim operatoreDaNonInserire As Entities.AttivitaRegistrazioneOperatore = operatori(i)

                    If operatoreDaNonInserire.IdOperatore = -1 Then

                        ' Controllo che l'operatore che si vuole inserire manualmente non esista già, in base ai dati inseriti
                        Dim operatoriGiaPresenti As IEnumerable(Of Entities.AttivitaOperatorePPA) =
                            GenericProvider.Attivita.GetOperatoriPPA(operatoreDaNonInserire.Cognome, operatoreDaNonInserire.Nome, operatoreDaNonInserire.IdUnitaOperativa, operatoreDaNonInserire.IdQualifica, True)

                        If Not operatoriGiaPresenti.IsNullOrEmpty() Then

                            ' Operatore da non inserire => lo cancello dalla lista e creo un messaggio da visualizzare all'utente.
                            Dim info As New System.Text.StringBuilder()

                            If operatoreDaNonInserire.IdQualifica.HasValue OrElse operatoreDaNonInserire.IdUnitaOperativa.HasValue Then

                                info.Append("(")

                                If Not String.IsNullOrWhiteSpace(operatoreDaNonInserire.DescrizioneQualifica) Then
                                    info.AppendFormat("{0} - ", operatoreDaNonInserire.DescrizioneQualifica)
                                End If

                                If Not String.IsNullOrWhiteSpace(operatoreDaNonInserire.DescrizioneUnitaOperativa) Then
                                    info.AppendFormat("{0} - ", operatoreDaNonInserire.DescrizioneUnitaOperativa)
                                End If

                                info.RemoveLast(3)

                                info.Append(")")

                            End If

                            messageOperatori.AppendFormat("{0} {1} {2},", operatoreDaNonInserire.Cognome, operatoreDaNonInserire.Nome, info)

                            operatori.RemoveAt(i)

                        End If
                    End If
                Next

                For Each operatore As Entities.AttivitaRegistrazioneOperatore In operatori

                    If operatore.IdOperatore = -1 Then

                        operatore.IdOperatore = Me.GenericProvider.Attivita.GetNextIdAttivitaOperatorePPA()

                        Dim operatorePPA As New Entities.AttivitaOperatorePPA()

                        operatorePPA.IdOperatore = operatore.IdOperatore
                        operatorePPA.Cognome = operatore.Cognome
                        operatorePPA.Nome = operatore.Nome
                        operatorePPA.CodiceFiscale = operatore.CodiceFiscale
                        operatorePPA.IdQualifica = operatore.IdQualifica
                        operatorePPA.IdUnitaOperativa = operatore.IdUnitaOperativa
                        operatorePPA.Obsoleto = False

                        Me.GenericProvider.Attivita.InsertOperatorePPA(operatorePPA)

                    End If

                Next

                If messageOperatori.Length > 0 Then
                    messageOperatori.RemoveLast(1)
                    messageOperatori.Insert(0, "I seguenti operatori non sono stati aggiunti perchè già presenti in anagrafica: ")
                End If

                ' Eliminazione duplicati (in base all'IdOperatore)
                operatori = operatori.Distinct(New AttivitaRegistrazioneOperatoriComparer()).ToList()

                Dim operatoriDaEliminare As List(Of Entities.AttivitaRegistrazioneOperatore) = Nothing

                If Not operatoriOriginal.IsNullOrEmpty() Then

                    ' Gli operatori da eliminare sono quelli che erano nella lista originale e non sono in quella corrente
                    operatoriDaEliminare =
                        operatoriOriginal.Where(Function(old) Not operatori.Any(Function(p) p.IdOperatore = old.IdOperatore)).ToList()

                    ' Gli operatori da inserire sono quelli che sono nella lista corrente e non sono in quella originale
                    operatori.RemoveAll(Function(p) operatoriOriginal.Any(Function(old) old.IdOperatore = p.IdOperatore))

                End If

                ' Eliminazione operatori (viene eliminata l'associazione tra l'operatore e l'attività corrente)
                If Not operatoriDaEliminare.IsNullOrEmpty() Then

                    For Each operatore As Entities.AttivitaRegistrazioneOperatore In operatoriDaEliminare
                        Me.GenericProvider.Attivita.DeleteOperatoreAttivitaRegistrazione(attivitaRegistrazione.Id, operatore.IdOperatore)
                    Next

                End If

                ' Inserimento operatori (viene inserita l'associazione tra l'operatore e l'attività corrente)
                If Not operatori.IsNullOrEmpty() Then

                    For Each operatore As Entities.AttivitaRegistrazioneOperatore In operatori
                        Me.GenericProvider.Attivita.InsertOperatoreAttivitaRegistrazione(attivitaRegistrazione.Id, operatore.IdOperatore)
                    Next

                End If

            End If
#End Region

            If ownTransaction Then
                GenericProvider.Commit()
            End If

            result.Success = True
            result.Message = messageOperatori.ToString()

        Catch ex As Exception

            If ownTransaction Then
                GenericProvider.Rollback()
            End If

            ex.InternalPreserveStackTrace()
            Throw

        End Try

        Return result

    End Function

    ''' <summary>
    ''' Restituisce il dettaglio della rilevazione specificata.
    ''' </summary>
    ''' <param name="idAttivitaRegistrazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAttivitaRegistrazioneDettaglio(idAttivitaRegistrazione As Integer) As Entities.AttivitaRegistrazioneDettaglio

        Dim dettaglio As New Entities.AttivitaRegistrazioneDettaglio()

        dettaglio.AttivitaRegistrazioneCorrente = GetAttivitaRegistrazione(idAttivitaRegistrazione)

        dettaglio.RispostePossibili = GenericProvider.Attivita.GetRispostePossibiliAttivita(dettaglio.AttivitaRegistrazioneCorrente.IdAttivitaAnagrafe).ToList()

        dettaglio.Valori = GenericProvider.Attivita.GetValoriAttivitaRegistrazione(idAttivitaRegistrazione).ToList()

        Return dettaglio

    End Function

#Region " Operatori "

    Public Class RicercaOperatoriCommand(Of T As Structure)
        Inherits RicercaCommand(Of T)

        Public Property IdAttivitaRegistrazione As Integer
        Public Property IdOperatoriDaEscludere As IEnumerable(Of Integer)

    End Class

    ''' <summary>
    ''' Restituisce tutti gli operatori PPA presenti in anagrafe e non obsoleti, esclusi quelli associati all'attività registrata specificata.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetOperatoriPPA(command As RicercaOperatoriCommand(Of Entities.AttivitaOperatorePPA.Ordinamento)) As List(Of Entities.AttivitaOperatorePPA)

        SetRicercaCommandIfNull(command)

        Return Me.GenericProvider.Attivita.GetOperatoriPPA(
            command.IdAttivitaRegistrazione, command.IdOperatoriDaEscludere, command.FiltroGenerico, command.FiltroSoloValidi,
            command.CampoOrdinamento, GetVersoOrdinamento(command), CreatePagingOptions(command)).ToList()

    End Function

    ''' <summary>
    ''' Restituisce il numero di operatori PPA presenti in anagrafe, esclusi quelli associati all'attività registrata specificata ed esclusi quelli con gli id indicati.
    ''' </summary>
    ''' <param name="idAttivita"></param>
    ''' <param name="idOperatoriDaEscludere"></param>
    ''' <param name="filtroGenerico"></param>
    ''' <param name="filtroSoloValidi"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountOperatoriPPA(idAttivita As Integer?, idOperatoriDaEscludere As IEnumerable(Of Integer), filtroGenerico As String, filtroSoloValidi As Boolean) As Integer

        Return Me.GenericProvider.Attivita.CountOperatoriPPA(idAttivita, idOperatoriDaEscludere, filtroGenerico, filtroSoloValidi)

    End Function

    ''' <summary>
    ''' Restituisce l'operatore PPA specificato
    ''' </summary>
    ''' <param name="idOperatore"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetOperatorePPA(idOperatore As Integer) As Entities.AttivitaOperatorePPA

        Return Me.GenericProvider.Attivita.GetOperatorePPA(idOperatore)

    End Function

    ''' <summary>
    ''' Restituisce gli operatori PPA associati all'attività specificata
    ''' </summary>
    ''' <param name="idAttivitaRegistrazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetOperatoriAttivitaRegistrazione(idAttivitaRegistrazione As Integer) As List(Of Entities.AttivitaRegistrazioneOperatore)

        Return Me.GenericProvider.Attivita.GetOperatoriAttivitaRegistrazione(idAttivitaRegistrazione).ToList()

    End Function

    ''' <summary>
    ''' Eliminazione associazione tra operatore e attività
    ''' </summary>
    ''' <param name="idAttivitaRegistrazione"></param>
    ''' <param name="idOperatore"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteOperatoreAttivitaRegistrazione(idAttivitaRegistrazione As Integer, idOperatore As Integer) As Integer

        Return Me.GenericProvider.Attivita.DeleteOperatoreAttivitaRegistrazione(idAttivitaRegistrazione, idOperatore)

    End Function

    ''' <summary>
    ''' Restituisce tutte le qualifiche presenti in anagrafe, in base ai filtri
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetQualifiche(command As RicercaCommand(Of Entities.Qualifica.Ordinamento)) As List(Of Entities.Qualifica)

        SetRicercaCommandIfNull(command)

        Return Me.GenericProvider.Attivita.GetQualifiche(
            command.FiltroGenerico, command.FiltroSoloValidi, command.CampoOrdinamento, GetVersoOrdinamento(command), CreatePagingOptions(command)).ToList()

    End Function

    ''' <summary>
    ''' Restituisce il numero di qualifiche presenti in anagrafe, in base ai filtri
    ''' </summary>
    ''' <param name="filtroGenerico"></param>
    ''' <param name="filtroSoloValidi"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountQualifiche(filtroGenerico As String, filtroSoloValidi As Boolean) As Integer

        Return Me.GenericProvider.Attivita.CountQualifiche(filtroGenerico, filtroSoloValidi)

    End Function

    ''' <summary>
    ''' Restituisce tutte le qualifiche presenti in anagrafe, in base ai filtri
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUnitaOperative(command As RicercaCommand(Of Entities.UnitaOperativa.Ordinamento)) As List(Of Entities.UnitaOperativa)

        SetRicercaCommandIfNull(command)

        Return Me.GenericProvider.Attivita.GetUnitaOperative(
            command.FiltroGenerico, command.FiltroSoloValidi, command.CampoOrdinamento, GetVersoOrdinamento(command), CreatePagingOptions(command)).ToList()

    End Function

    ''' <summary>
    ''' Restituisce il numero di unita operative presenti in anagrafe, in base ai filtri
    ''' </summary>
    ''' <param name="filtroGenerico"></param>
    ''' <param name="filtroSoloValidi"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountUnitaOperative(filtroGenerico As String, filtroSoloValidi As Boolean) As Integer

        Return Me.GenericProvider.Attivita.CountUnitaOperative(filtroGenerico, filtroSoloValidi)

    End Function

#End Region

#End Region

#Region " Scuole "

    ''' <summary>
    ''' Restituisce le scuole presenti in anagrafe in base ai filtri specificati
    ''' </summary>
    ''' <param name="command"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetElencoScuole(command As RicercaCommand(Of Entities.Scuola.Ordinamento), ricerca As Boolean) As IEnumerable(Of Entities.Scuola)

        SetRicercaCommandIfNull(command)

        Return GenericProvider.Attivita.GetElencoScuole(
            command.FiltroGenerico, command.FiltroSoloValidi, command.CampoOrdinamento, GetVersoOrdinamento(command), CreatePagingOptions(command))

    End Function

    ''' <summary>
    ''' Restituisce il numero di scuole in base al filtro specificato
    ''' </summary>
    ''' <param name="filtroGenerico"></param>
    ''' <param name="filtroSoloValidi"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountElencoScuole(filtroGenerico As String, filtroSoloValidi As Boolean) As Integer

        Return GenericProvider.Attivita.CountElencoScuole(filtroGenerico, filtroSoloValidi)

    End Function

    ''' <summary>
    ''' Restituisce la scuola selezionata
    ''' </summary>
    ''' <param name="codiceScuola"></param>
    ''' <returns></returns>
    Public Function GetScuola(codiceScuola As String) As Entities.Scuola

        Return GenericProvider.Attivita.GetScuola(codiceScuola)

    End Function

#End Region

#End Region

#Region " Private "

    Private Sub SetRicercaCommandIfNull(Of T As Structure)(command As RicercaCommand(Of T))

        If command Is Nothing Then
            command = New RicercaCommand(Of T)()
            command.IsDescending = False
            command.Size = 0
        End If

    End Sub

    Private Function GetVersoOrdinamento(Of T As Structure)(command As RicercaCommand(Of T)) As String

        Dim verso As String = Constants.VersoOrdinamento.Crescente

        If command.IsDescending Then verso = Constants.VersoOrdinamento.Decrescente

        Return verso

    End Function

    Private Function CreatePagingOptions(Of T As Structure)(command As RicercaCommand(Of T)) As Onit.OnAssistnet.Data.PagingOptions

        Dim pagingOptions As Onit.OnAssistnet.Data.PagingOptions = Nothing

        If command.Size > 0 Then
            pagingOptions = New Onit.OnAssistnet.Data.PagingOptions()
            pagingOptions.StartRecordIndex = command.Offset
            pagingOptions.EndRecordIndex = command.Offset + command.Size
        End If

        Return pagingOptions

    End Function

#End Region

End Class
