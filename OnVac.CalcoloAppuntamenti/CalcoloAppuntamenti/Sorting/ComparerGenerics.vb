'@ -----------------------------------------------------------------------------
'@ Project	 : CalcoloAppuntamenti
'@ Class	 : CalcoloAppuntamenti.ComparerGenerics
'@ 
'@ -----------------------------------------------------------------------------
'@ <summary>
'@ Classe generica per eseguire ordinamenti multipli su oggetti generici.
'@ </summary>
'@ <remarks>
'@ La classe si basa sullla reflection per eseguire degli ordinamenti multipli su oggetti generici. Si configura indicando su quali proprietà eseguire l'ordinamento e in quale verso (per ogni singola proprietà).
'@ Le proprietà devono eseere dei tipi che implementano IComparable (come i tipi di base).
'@ </remarks>
'@ <history>
'@ 	[ssabbattini]	13/02/2006	Created
'@ </history>
'@ <example>
'@ L'esempio illustra come utilizzare due proprietà per ordinare una collezione generica.
'@ <code>
'@ Public Class MyCollectionClass
'@     Inherits CollectionBase
'@ 
'@     Sub Sort(ByVal comparer As IComparer)
'@         InnerList.Sort(comparer)
'@     End Sub
'@ End Class
'@ 
'@ Sub SortMyClass(ByVal collection As MyCollectionClass)
'@     Dim comparer As New ComparerGenerics
'@     comparer.SortClasses.Add(New SortClass("Proprieta1", SortDirection.Ascending))
'@     comparer.SortClasses.Add(New SortClass("Proprieta2", SortDirection.Descending))
'@     collection.Sort(comparer)
'@ End Sub
'@ </code>
'@ </example>
'@ -----------------------------------------------------------------------------

Namespace Sorting
    Public Class ComparerGenerics
        Implements IComparer

        '@ -----------------------------------------------------------------------------
        '@ <summary>
        '@ Costruttore di default. Inizializza l'ArrayList delle classe.
        '@ </summary>
        '@ <remarks>
        '@ </remarks>
        '@ <history>
        '@ 	[ssabbattini]	13/02/2006	Created
        '@ </history>
        '@ -----------------------------------------------------------------------------
        Sub New()
            _SortClasses = New ArrayList
        End Sub

        '@ -----------------------------------------------------------------------------
        '@ <summary>
        '@ Inizializza la classe con un ArrayList di SortingClasses
        '@ </summary>
        '@ <param name="sortClasses">ArrayList che contiene l'elenco degli ordinamenti. Gli oggetti devono essere di tipo SortClass.</param>
        '@ <remarks>
        '@ </remarks>
        '@ <history>
        '@ 	[ssabbattini]	13/02/2006	Created
        '@ </history>
        '@ -----------------------------------------------------------------------------
        Sub New(ByVal sortClasses As ArrayList)
            _SortClasses = sortClasses
        End Sub

        '@ -----------------------------------------------------------------------------
        '@ <summary>
        '@ Aggiunge un elemento di per eseguire l'ordinamento.
        '@ </summary>
        '@ <param name="sortColumn">Proprietà su cui eseguire l'iordinamento.</param>
        '@ <param name="sortDirection">Direzione di ordinamento.</param>
        '@ <remarks>
        '@ L'ordine in cui si inseriscono gli oggetti verrà utilizzato come livello di ordinamento.
        '@ </remarks>
        '@ <history>
        '@ 	[ssabbattini]	13/02/2006	Created
        '@ </history>
        '@ -----------------------------------------------------------------------------
        Sub New(ByVal sortColumn As String, ByVal sortDirection As SortDirection)
            _SortClasses = New ArrayList
            _SortClasses.Add(New SortClass(sortColumn, sortDirection))
        End Sub

        Private _SortClasses As ArrayList

        '@ -----------------------------------------------------------------------------
        '@ <summary>
        '@ Collezione di classi di ordinamento.
        '@ </summary>
        '@ <value></value>
        '@ <remarks>
        '@ L'ordine delle classi inserito nell'ArrayList verrà utilizzato come livello di ordinamento delle classi.
        '@ </remarks>
        '@ <history>
        '@ 	[ssabbattini]	13/02/2006	Created
        '@ </history>
        '@ -----------------------------------------------------------------------------
        Public ReadOnly Property SortClasses() As ArrayList
            Get
                Return _SortClasses
            End Get
        End Property

        '@ -----------------------------------------------------------------------------
        '@ <summary>
        '@ Implementazione del metodo IComparer.Compare utilizzato per comparare due oggetti.
        '@ </summary>
        '@ <param name="x">Primo oggetto generico</param>
        '@ <param name="y">Secondo ooggetto generico</param>
        '@ <returns>-1 se x minore di y, 0 se x uguale a y o 1 x maggiore di y</returns>
        '@ <remarks>
        '@ La funzione richiama CheckSort che, ricorsivamente, esegue un controllo su ogni oggetto per stabilire l'ordine in base alle classi di SortClass.
        '@ </remarks>
        '@ <history>
        '@ 	[ssabbattini]	13/02/2006	Created
        '@ </history>
        '@ -----------------------------------------------------------------------------
        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
            If SortClasses.Count = 0 Then
                Return 0
            End If

            Return CheckSort(0, x, y)
        End Function

        '@ -----------------------------------------------------------------------------
        '@ <summary>
        '@ Funzione ricorsiva che esegue l'ordinamento.
        '@ </summary>
        '@ <param name="SortLevel">Livello di ordinamento. La prima chiamata deve essere 0, ad ogni livello di ordinamento successivo, viene incrementato.</param>
        '@ <param name="obj1">Primo oggetto</param>
        '@ <param name="obj2">Secondo oggetto</param>
        '@ <returns>-1 se x minore di y, 0 se x uguale a y o 1 x maggiore di y</returns>
        '@ <remarks>
        '@ Sfrutta la reflection per ricercare i campi su cui eseguire gli ordinamenti.
        '@ </remarks>
        '@ <history>
        '@ 	[ssabbattini]	13/02/2006	Created
        '@ </history>
        '@ -----------------------------------------------------------------------------
        Private Function CheckSort(ByVal SortLevel As Integer, ByVal obj1 As Object, ByVal obj2 As Object) As Integer
            Dim returnVal As Integer = 0

            If SortClasses.Count - 1 >= SortLevel Then
                Dim valueOf1 As Object = obj1.GetType.GetProperty(CType(SortClasses(SortLevel), SortClass).SortColumn).GetValue(obj1, Nothing)
                Dim valueOf2 As Object = obj2.GetType.GetProperty(CType(SortClasses(SortLevel), SortClass).SortColumn).GetValue(obj2, Nothing)

                If CType(SortClasses(SortLevel), SortClass).SortDirection = SortDirection.Ascending Then
                    returnVal = valueOf1.compareTo(valueOf2)
                Else
                    returnVal = valueOf2.compareTo(valueOf1)
                End If

                If returnVal = 0 Then
                    returnVal = CheckSort(SortLevel + 1, obj1, obj2)
                End If

            End If

            Return returnVal
        End Function

        '@ -----------------------------------------------------------------------------
        '@ Project	 : CalcoloAppuntamenti
        '@ Class	 : CalcoloAppuntamenti.ComparerGenerics.SortClass
        '@ 
        '@ -----------------------------------------------------------------------------
        '@ <summary>
        '@ Classe che identifica una entità di ordinamento. Viene utilizzata per immagazzinare le informazioni su come ordinare una singola proprietà. E' possibile iindire la direzione di ordinamento di ogni singola istanza.
        '@ </summary>
        '@ <remarks>
        '@ </remarks>
        '@ <history>
        '@ 	[ssabbattini]	14/02/2006	Created
        '@ </history>
        '@ -----------------------------------------------------------------------------
        Public Class SortClass

            '@ -----------------------------------------------------------------------------
            '@ <summary>
            '@ Inizializza una nuova istanza di ordinamento per una singola proprietà.
            '@ </summary>
            '@ <param name="sortColumn">Il nome della proprietà della classe da ordinare.</param>
            '@ <param name="sortDirection">Direzione di ordinamento.</param>
            '@ <remarks>
            '@ </remarks>
            '@ <history>
            '@ 	[ssabbattini]	14/02/2006	Created
            '@ </history>        
            '@ -----------------------------------------------------------------------------
            Sub New(ByVal sortColumn As String, ByVal sortDirection As SortDirection)
                Me.SortColumn = sortColumn
                Me.SortDirection = sortDirection
            End Sub

            Private _SortColumn As String
            '@ -----------------------------------------------------------------------------
            '@ <summary>
            '@ Nome della proprietà su cui eseguire l'ordinamento.
            '@ </summary>
            '@ <value></value>
            '@ <remarks>
            '@ </remarks>
            '@ <history>
            '@ 	[ssabbattini]	14/02/2006	Created
            '@ </history>
            '@ -----------------------------------------------------------------------------
            Public Property SortColumn() As String
                Get
                    Return _SortColumn
                End Get
                Set(ByVal Value As String)
                    _SortColumn = Value
                End Set
            End Property

            Private _SortDirection As SortDirection
            '@ -----------------------------------------------------------------------------
            '@ <summary>
            '@ Direzione di ordinamento. Ascendento o Discendente
            '@ </summary>
            '@ <value>Indica come verrà ordinato il membro specifico</value>
            '@ <remarks>
            '@ </remarks>
            '@ <history>
            '@ 	[ssabbattini]	15/02/2006	Created
            '@ </history>
            '@ -----------------------------------------------------------------------------
            Public Property SortDirection() As SortDirection
                Get
                    Return _SortDirection
                End Get
                Set(ByVal Value As SortDirection)
                    _SortDirection = Value
                End Set
            End Property
        End Class
    End Class
End Namespace