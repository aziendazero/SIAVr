Imports System.Collections.Generic

Imports Onit.OnAssistnet.Data


Public Interface IUtentiProvider

    ''' <summary>
    '''  Restituisce la descrizione dell'utente in base all'id specificato
    ''' </summary>
    ''' <param name="idUtente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetDescrizioneUtente(idUtente As Integer) As String

    ''' <summary>
    ''' Restituisce il codice dell'utente in base all'id specificato
    ''' </summary>
    ''' <param name="idUtente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetCodiceUtente(idUtente As Integer) As String

    ''' <summary>
    ''' Restituisce il codice fiscale dell'utente in base all'id specificato
    ''' </summary>
    ''' <param name="idUtente"></param>
    ''' <returns></returns>
    Function GetCodiceFiscaleUtente(idUtente As Integer) As String

    ''' <summary>
    ''' Restituisce il codice del medico, se l'utente è associato a un medico tramite il codice esterno dell'operatore
    ''' </summary>
    Function GetMedicoDaUtente(idUtente As Integer) As String

    ''' <summary>
    ''' Restituisce un oggetto Utente contenente tutti i dati dell'utente letti da db, in base all'id specificato.
    ''' </summary>
    Function GetUtente(idUtente As Integer) As OnVac.Entities.Utente

    ''' <summary>
    ''' Restituisce il numero di utenti trovati, in base ai filtri.
    ''' <summary>
    Function CountUtenti(codiceDescrizioneLikeFilter As String, codiceConsultorio As String, appId As String, codiceAzienda As String) As Integer

    ''' <summary>
    ''' Restituisce una lista di utenti letti dalla v_ana_utenti, filtrati per id dell'applicazione e codice azienda (se specificati).
    ''' Se il parametro codiceDescrizioneLikeFilter è valorizzato, la ricerca sarà effettuata filtrando (in like) codice e descrizione in base al valore del parametro.
    ''' Se il parametro codiceConsultorio è valorizzato, restituisce solo gli utenti abilitati al consultorio specificato.
    ''' </summary>
    Function GetListUtenti(codiceDescrizioneLikeFilter As String, codiceConsultorio As String, appId As String, codiceAzienda As String, pagingOptions As PagingOptions) As List(Of Entities.Utente)

    ''' <summary>
    ''' Restituisce una lista contenente i dati relativi ai consultori abilitati per l'utente specificato
    ''' </summary>
    Function GetListConsultoriUtente(idUtente As Integer) As List(Of Entities.ConsultorioUtente)

    Function InsertConsultorioUtente(consultorioUtente As Entities.ConsultorioUtente) As Integer

    Function DeleteConsultoriUtente(idUtente As Integer) As Integer
    Function DeleteConsultorioUtente(codiceConsultorio As String, idUtente As Integer) As Integer

    Function IsUserInGroup(idUtente As Long, idGruppo As Integer) As Boolean

    Function InserimentoConsensi(idUtente As Long, codicePaziente As Long, codiceAusiliarioPaziente As String, approvazione As String) As ResultSetPost

End Interface
