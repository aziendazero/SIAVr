Imports Onit.OnAssistnet.OnVac.CalcoloAppuntamenti.Sorting

Namespace Timing
    <Serializable()> _
    Public Class DayCollection
        Inherits CollectionBase
        Implements ISortable
        Implements ICloneable

        Sub New()
            Me.InnerList.Capacity = 30
        End Sub

        Default Public ReadOnly Property Item(ByVal index As Integer) As Day
            Get
                If Not _IsSorted Then Sort()

                Return CType(List(index), Day)
            End Get
        End Property

        Default Public ReadOnly Property Item(ByVal day As Date) As Day
            Get
                If Not _IsSorted Then Sort()

                For Each d As day In innerlist
                    If d.Giorno = day.Date Then
                        Return d
                    End If
                Next
                Return Nothing
            End Get
        End Property

        Public Function Add(ByVal value As Day) As Integer
            AddHandler value.DayChanged, AddressOf DayChanged
            _IsSorted = False

            Return List.Add(value)
        End Function

        Public Sub Remove(ByVal value As Day)
            List.Remove(value)
        End Sub

        Private _IsSorted As Boolean
        Public Property IsSorted() As Boolean Implements Sorting.ISortable.IsSorted
            Get
                Return _IsSorted
            End Get
            Set(ByVal Value As Boolean)
                _IsSorted = Value
            End Set
        End Property

        Public Sub Sort() Implements Sorting.ISortable.Sort
            Dim comparer As New ComparerGenerics
            comparer.SortClasses.Add(New ComparerGenerics.SortClass("Giorno", SortDirection.Ascending))
            innerlist.Sort(comparer)

            _IsSorted = True
        End Sub

        Public Function GetAppuntamentoLibero(ByVal minuti As Integer) As TimeBlock
            If Not _IsSorted Then Sort()

            For Each d As Day In InnerList
                If Not d.Full Then
                    Dim t As TimeBlock = d.TimeBlocks.GetFree(minuti)
                    If Not t Is Nothing Then Return t
                End If
            Next

            Return Nothing
        End Function

        Public Function GetAppuntamentoLibero(ByVal minuti As Integer, ByVal filtriVariabili As Filtri.FiltroCollection) As TimeBlock
            If Not _IsSorted Then Sort()

            For Each d As Day In InnerList
                If Not d.Full Then
                    Dim t As TimeBlock = d.TimeBlocks.GetFree(minuti, filtriVariabili)
                    If Not t Is Nothing Then Return t
                End If
            Next

            Return Nothing
        End Function

        '@ -----------------------------------------------------------------------------
        '@ <summary>
        '@ Rimuove un marcher specifico da tutti i TimeBlock dei Day (solo se non è obbligatorio).
        '@ </summary>
        '@ <param name="priority"></param>
        '@ <param name="tipo"></param>
        '@ <history>
        '@ 	[ssabbattini]	27/02/2006	Created
        '@ </history>
        '@ -----------------------------------------------------------------------------
        Public Sub RemoveMarcherFilter(ByVal priority As Integer, ByVal tipo As MarcherTipo)
            For Each d As Day In InnerList
                For Each t As TimeBlock In d.TimeBlocks
                    t.UnsetMarcher(priority, tipo)
                Next
            Next
        End Sub

        Public Sub DayChanged(ByVal d As Day, ByVal oldDate As Date, ByVal newDate As Date)
            If oldDate <> newDate Then _IsSorted = False
        End Sub

        Public Function Clone() As Object Implements System.ICloneable.Clone
            Return Utility.CloneObject(Me)
        End Function
    End Class
End Namespace