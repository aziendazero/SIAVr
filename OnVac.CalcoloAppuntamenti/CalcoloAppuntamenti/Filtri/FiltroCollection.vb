Imports Onit.OnAssistnet.OnVac.CalcoloAppuntamenti.Filtri
Imports Onit.OnAssistnet.OnVac.CalcoloAppuntamenti.Sorting

Namespace Filtri
    <Serializable()> _
        Public Class FiltroCollection
        Inherits CollectionBase
        Implements ISortable
        Implements ICloneable

        Default Public ReadOnly Property Item(ByVal index As Integer) As IFiltro
            Get
                If Not IsSorted Then Sort()

                Return CType(List(index), IFiltro)
            End Get
        End Property

        Public Function Add(ByVal value As IFiltro) As Integer
            IsSorted = False

            value.FiltriParent = Me
            Return List.Add(value)
        End Function

        Public Sub Remove(ByVal value As IFiltro)
            List.Remove(value)
        End Sub

        Public Function ApplyFilter(ByVal priorityMax As Integer, ByVal d As Timing.DayCollection) As Timing.DayCollection
            For Each f As IFiltro In InnerList
                If Not f.Obbligatorio Then
                    If f.Priorita <= priorityMax Then
                        d = f.ApplicaMarcher(d, pars)
                    End If
                Else
                    d = f.ApplicaMarcher(d, pars)
                End If
            Next

            Return d
        End Function

        Public Function ApplyFilter(ByVal priorityMax As Integer, ByVal d As Timing.Day) As Timing.Day
            For Each f As IFiltro In InnerList
                If Not f.Obbligatorio Then
                    If f.Priorita <= priorityMax Then
                        d = f.ApplicaMarcher(d, pars)
                    End If
                Else
                    d = f.ApplicaMarcher(d, pars)
                End If
            Next

            Return d
        End Function

        Private pars As New Hashtable
        '@ -----------------------------------------------------------------------------
        '@ <summary>
        '@ Imposta i parametri relativi a tutta la famiglia di fiiltri. Ogni oparametro contiene al suo interno variabili shared che identificano i nomi dei parametri che accettano.
        '@ </summary>
        '@ <param name="parName">Nome del parametro.</param>
        '@ <param name="value">Valore</param>
        '@ <history>
        '@ 	[ssabbattini]	02/03/2006	Created
        '@ </history>
        '@ -----------------------------------------------------------------------------
        Public Sub SetParam(ByVal parName As String, ByVal value As Object)
            pars.Add(parName, value)
        End Sub

        Public Function GetParam(ByVal parName As String) As Object
            Return pars(parName)
        End Function

        Public Function GetParams() As Hashtable
            Return pars
        End Function

        Public Function GetFromPriority(ByVal priority As Integer) As FiltroCollection
            Dim filters As New FiltroCollection

            For Each f As IFiltro In Me.InnerList
                If f.Priorita = priority Then
                    filters.Add(f)
                End If
            Next

            Return filters
        End Function

        Public Function GetFromPrioritySingle(ByVal priority As Integer) As IFiltro
            For Each f As IFiltro In Me.InnerList
                If f.Priorita = priority Then
                    Return f
                End If
            Next
            Return Nothing
        End Function

        Private _IsSorted As Boolean
        Public Property IsSorted() As Boolean Implements ISortable.IsSorted
            Get
                Return _IsSorted
            End Get
            Set(ByVal Value As Boolean)
                _IsSorted = Value
            End Set
        End Property

        Public Sub Sort() Implements ISortable.Sort
            Dim comparer As New ComparerGenerics
            comparer.SortClasses.Add(New ComparerGenerics.SortClass("Priorita", SortDirection.Ascending))
            innerlist.Sort(comparer)

            IsSorted = True
        End Sub

        Public Function Clone() As Object Implements System.ICloneable.Clone
            Return Utility.CloneObject(Me)
        End Function

        Private _NumeroMassimoBlocchiPersonalizzatiGiornalieri As Integer
        Public Property NumeroMassimoBlocchiPersonalizzatiGiornalieri() As Integer
            Get
                Return _NumeroMassimoBlocchiPersonalizzatiGiornalieri
            End Get
            Set(ByVal value As Integer)
                _NumeroMassimoBlocchiPersonalizzatiGiornalieri = value
            End Set
        End Property
    End Class
End Namespace