Imports Onit.OnAssistnet.OnVac.CalcoloAppuntamenti.Sorting
Imports Onit.OnAssistnet.OnVac.CalcoloAppuntamenti.Sorting.ComparerGenerics

'@ -----------------------------------------------------------------------------
'@ Project	 : CalcoloAppuntamenti
'@ Class	 : CalcoloAppuntamenti.CnvPazienteCollection
'@ 
'@ -----------------------------------------------------------------------------
'@ <summary>
'@ Rappresenta la collezione dei pazienti di cui cercare la data di appuntamento. La classe contiene una Hastable per la ricerca dei pazienti tramite codice e data (i due dati sono stati uniti per creare un'unica chiave).
'@ Le collezioni presenti sono: <i>List</i>: IList generica utilizzata per l'inserimento, l'eliminazione. <i>InnerList</i>: Utilizzata per ordinare la collezione. <i>_RicercaVeloce</i>: utilizzato per la ricerca delle proprietà via codice e data convocazione.
'@ Un paziente può essere inserito più volte con una data di convocazione diversa.
'@ Se si vuole eseguire l'enumeraqzione di tuttii gli elementi utilizzare la sintassi For Each e GetEnumerator.
'@ </summary>
'@ <history>
'@ 	[ssabbattini]	21/02/2006	Created
'@ </history>
'@ -----------------------------------------------------------------------------
Public Class CnvPazienteCollection
    Inherits CollectionBase
    Implements ISortable

    Public Property UsaOrdineAlfabetico As Boolean


    Public Event AddPaziente(ByVal paziente As CnvPaziente, ByRef cancel As Boolean)
    Public Event RemovePaziente(ByVal paziente As CnvPaziente, ByRef cancel As Boolean)

    '@ -----------------------------------------------------------------------------
    '@ <summary>
    '@ Hashtable per la ricerca via codice. Rende questo tipo di ricerca molto veloce.
    '@ </summary>
    '@ <history>
    '@ 	[ssabbattini]	21/02/2006	Created
    '@ </history>
    '@ -----------------------------------------------------------------------------
    Private _RicercaVeloce As New Hashtable

    Default Public ReadOnly Property Item(ByVal codice As Integer, ByVal dataCnv As Date) As CnvPaziente
        Get
            Return CType(_RicercaVeloce(String.Format("{0}-{1}", codice, dataCnv.ToShortDateString)), CnvPaziente)
        End Get
    End Property

    '@ -----------------------------------------------------------------------------
    '@ <summary>
    '@ Inserisce un paziente nella collezione della classe e nella hashtable per la ricerca. La funzione registra un gestore di eventi per PrioritaChanged.
    '@ </summary>
    '@ <param name="value">Nuovo paziente</param>
    '@ <returns></returns>
    '@ <exception cref="OnVac.Exceptions.CnvPazienteDuplicatedException">Scatenato quando si inserisce un paziente con una data e un codice già esistenti.</exception>
    '@ <event cref="AddPaziente">Utilizzare il parametro ByRef cancel per annullare l'inserimento del paziente.</event>
    '@ <history>
    '@ 	[ssabbattini]	21/02/2006	Created
    '@ </history>
    '@ -----------------------------------------------------------------------------
    Public Function Add(ByVal value As CnvPaziente) As Integer
        Dim cancel As Boolean = False
        RaiseEvent AddPaziente(value, cancel)

        If Not cancel Then
            Dim hash As String = HashCode(value)
            If _RicercaVeloce.ContainsKey(hash) Then
                Throw New Exceptions.CnvPazienteDuplicatedException(value)
            End If
            _RicercaVeloce.Add(hash, value)
            IsSorted = False

            AddHandler value.PrioritaChanged, AddressOf PrioritaChanged

            Return List.Add(value)
        End If
    End Function

    '@ -----------------------------------------------------------------------------
    '@ <summary>
    '@ La rimozione di un elemento dalla collezione non ne altera lo stato di sorting. L'elemento viene anche rimosso dalla hashtable.
    '@ </summary>
    '@ <param name="value">Paziente da eliminare.</param>
    '@ <event cref="RemovePaziente">Utilizzare il parametro ByRef cancel per annullare l'eliminazione del paziente.</event>
    '@ <history>
    '@ 	[ssabbattini]	21/02/2006	Created
    '@ </history>
    '@ -----------------------------------------------------------------------------
    Public Sub Remove(ByVal value As CnvPaziente)
        Dim cancel As Boolean = False
        RaiseEvent RemovePaziente(value, cancel)

        If Not cancel Then
            _RicercaVeloce.Remove(HashCode(value))
            List.Remove(value)
        End If
    End Sub

    Public Function Contains(ByVal value As CnvPaziente) As Boolean
        Return List.Contains(value)
    End Function

    '@ -----------------------------------------------------------------------------
    '@ <summary>
    '@ Restituisce un codice da utilizzare per le ricerche nell'hashtable
    '@ </summary>
    '@ <param name="value">CnvPaziente di cui generare il codice univoco.</param>
    '@ <returns>Codice Hash</returns>
    '@ <history>
    '@ 	[ssabbattini]	21/02/2006	Created
    '@ </history>
    '@ -----------------------------------------------------------------------------
    Private Function HashCode(ByVal value As CnvPaziente) As String
        Return String.Format("{0}-{1}", value.Codice, value.DataConvocazione.ToShortDateString)
    End Function

    '@ -----------------------------------------------------------------------------
    '@ <summary>
    '@ Restituisce un array di CnvPazienti ordinato per Priorita. L'elenco contiene anche i ritardatari e i soloBialncio in base ai due parametri.
    '@ </summary>
    '@ <param name="ritardatari">Indica se includere nell'elenco i ritardatari.</param>
    '@ <param name="soloBilancio">Indica se includere nell'elenco i solo bilancio.</param>
    '@ <returns>Array ordinato su Prioprita.</returns>
    '@ <remarks>
    '@ Nel caso nell'elenco sia stato inserito un paziente dall'ultima chiamata, verrà richiamata la procedura Sort, quindi la chiamata può essere lenta. L'algoruitmo utilizzato è il QuickSort con complessità computazionale
    '@ di O(n^2)
    '@ </remarks>
    '@ <history>
    '@ 	[ssabbattini]	21/02/2006	Created
    '@ </history>
    '@ -----------------------------------------------------------------------------
    Public Function GetPazienti(ByVal ritardatari As Boolean, ByVal soloBilancio As Boolean) As CnvPaziente()
        If Not _IsSorted Then
            Sort()
        End If

        Dim returnList As New ArrayList
        For i As Integer = 0 To InnerList.Count - 1
            Dim paz As CnvPaziente = InnerList(i)

            If paz.Ritardatario = ritardatari And paz.SoloBilancio = soloBilancio Then
                returnList.Add(paz)
            End If
        Next

        Return returnList.ToArray(GetType(CnvPaziente))
    End Function

    Private _IsSorted As Boolean
    '@ -----------------------------------------------------------------------------
    '@ <summary>
    '@ Indica se la collezione è ordinata oppure no. Quando viene richiamato il metodo sort la proprietà viene settata a true. Se si aggiunge un elemento alla collezione viene settata a false.
    '@ </summary>
    '@ <value>True se la collezione è ordinata, False altrimenti.</value>
    '@ <history>
    '@ 	[ssabbattini]	23/02/2006	Created
    '@ </history>
    '@ -----------------------------------------------------------------------------
    Public Property IsSorted() As Boolean Implements ISortable.IsSorted
        Get
            Return _IsSorted
        End Get
        Set(ByVal Value As Boolean)
            _IsSorted = Value
        End Set
    End Property

    '@ -----------------------------------------------------------------------------
    '@ <summary>
    '@ Ordina la classe in base a Priorità, intesa come DataConvocazione + Min(Validità).
    '@ </summary>
    '@ <history>
    '@ 	[ssabbattini]	21/02/2006	Created
    '@ </history>
    '@ -----------------------------------------------------------------------------
    Public Sub Sort() Implements ISortable.Sort

        Dim comparer As New ComparerGenerics()

        comparer.SortClasses.Add(New SortClass("Priorita", SortDirection.Ascending))

        If UsaOrdineAlfabetico Then
            comparer.SortClasses.Add(New SortClass("Cognome", SortDirection.Ascending))
            comparer.SortClasses.Add(New SortClass("Nome", SortDirection.Ascending))
        End If

        comparer.SortClasses.Add(New SortClass("DataNascita", SortDirection.Ascending))

        InnerList.Sort(comparer)

        IsSorted = True

    End Sub

    '@ -----------------------------------------------------------------------------
    '@ <summary>
    '@ Scatenata quando si reimposta una priorità diversa per un paziente. Dato che la chiave su cui si ordina la collezione è la priorità, si suppone che
    '@ una volta cambiato questo valore la collezione non sia più ordinata.
    '@ </summary>
    '@ <param name="paz">Paziente che ha scatenato l'evento.</param>
    '@ <param name="oldPri">Vecchia priorità.</param>
    '@ <param name="newPri">Nuova priorità.</param>
    '@ <remarks>
    '@ La priorità non cambia se non cambia la proprità.
    '@ </remarks>
    '@ <history>
    '@ 	[ssabbattini]	23/02/2006	Created
    '@ </history>
    '@ -----------------------------------------------------------------------------
    Private Sub PrioritaChanged(ByVal paz As CnvPaziente, ByVal oldPri As Date, ByVal newPri As Date)
        If oldPri <> newPri Then IsSorted = False
    End Sub
End Class
