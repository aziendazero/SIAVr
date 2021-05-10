Imports Onit.OnAssistnet.OnVac.CalcoloAppuntamenti.Sorting
Imports Onit.OnAssistnet.OnVac.CalcoloAppuntamenti.Filtri

Namespace Timing
    <Serializable()> _
    Public Class TimeBlockCollection
        Inherits CollectionBase
        Implements ISortable

        Private _dayParent As Day

        Sub New()
            Me.InnerList.Capacity = 20
        End Sub

        Friend Sub New(ByVal parentDay As Day)
            Me.New()
            _dayParent = parentDay
        End Sub

        Default Public ReadOnly Property Item(ByVal index As Integer) As TimeBlock
            Get
                If Not _IsSorted Then Sort()

                Return CType(List(index), TimeBlock)
            End Get
        End Property

        Public Function GetItemByStart(ByVal start As Date, ByRef position As Integer) As TimeBlock
            If Not _IsSorted Then Sort()

            Dim index As Integer = 0
            For Each t As TimeBlock In InnerList
                If t.Inizio <= start And t.Fine > start Then
                    position = index
                    Return t
                End If
                index += 1
            Next

            Return Nothing
        End Function

        Public Function Add(ByVal value As TimeBlock) As Integer
            AddHandler value.InizioChanged, AddressOf InizioChanged

            If list.Count >= 1 Then _IsSorted = False

            value._ParentDay = _dayParent

            Return List.Add(value)
        End Function

        Public Sub Remove(ByVal value As TimeBlock)
            List.Remove(value)
        End Sub

        Function GetFree(ByVal minuti As Integer) As TimeBlock
            If Not _IsSorted Then Sort()

            For Each el As TimeBlock In InnerList
                If el.IsFree(minuti) Then
                    Return el
                End If
            Next
            Return Nothing
        End Function

        Function GetFree(ByVal minuti As Integer, ByVal filtriVariabili As Filtri.FiltroCollection) As TimeBlock
            If Not _IsSorted Then Sort()

            For Each el As TimeBlock In InnerList
                Dim found As Boolean = False
                For i As Integer = 0 To filtriVariabili.NumeroMassimoBlocchiPersonalizzatiGiornalieri
                    If Not filtriVariabili.GetParam(Filtri.FiltroNumeroMassimoAppuntamentiBlocco.GetNewParameterID(_dayParent.Giorno, i)) Is Nothing Then
                        Dim durataOverride As ParameterBlocco = filtriVariabili.GetParam(Filtri.FiltroNumeroMassimoAppuntamentiBlocco.GetNewParameterID(_dayParent.Giorno, i))
                        If durataOverride.OverrideDurata AndAlso durataOverride.Numero <> 0 Then
                            If durataOverride.DaOra <= el.Inizio AndAlso durataOverride.AOra >= el.Fine Then
                                found = True

                                If el.IsFree(durataOverride.Durata) Then
                                    el._InOverrideDurata = True
                                    el._OverrideDurata = durataOverride.Durata
                                    Return el
                                End If
                            End If
                        End If
                    End If
                Next
                If Not found Then
                    If el.IsFree(minuti) Then
                        Return el
                    End If
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
            comparer.SortClasses.Add(New ComparerGenerics.SortClass("Inizio", SortDirection.Ascending))
            innerlist.Sort(comparer)

            IsSorted = True
        End Sub

        Public Sub InizioChanged(ByVal timeBlock As TimeBlock, ByVal oldInizio As Date, ByVal newInizio As Date, ByRef cancel As Boolean)
            If oldInizio <> newInizio Then _IsSorted = False
        End Sub

        '@ -----------------------------------------------------------------------------
        '@ <summary>
        '@ Divide un TimeBlock. Il time block può essere diviso in 4 modi differenti: <img src="img\TimeBlockDivision.gif">. La procedura mantiene ordinati i TiumeBlock.
        '@ </summary>
        '@ <param name="inizio"></param>
        '@ <param name="fine"></param>
        '@ <param name="marcher"></param>
        '@ <param name="tipo"></param>
        '@ <history>
        '@ 	[ssabbattini]	27/02/2006	Created
        '@ </history>
        '@ -----------------------------------------------------------------------------
        Public Sub Split(ByVal inizio As Date, ByVal fine As Date, ByVal nuovoMarcher As Integer, ByVal tipo As MarcherTipo)
            Dim tb As TimeBlock() = Nothing
            Dim posizione As Integer

            Dim t As TimeBlock = GetItemByStart(inizio, posizione)      'Cerca quello che contiene la data di inizio

            If fine > t.Fine Then
                Split(inizio, t.Fine, nuovoMarcher, tipo)
                Split(t.Fine, fine, nuovoMarcher, tipo)
                Return
                'Throw New Exceptions.TimeBlockInvalidException(inizio, fine)
            Else
                If inizio > t.Inizio AndAlso fine < t.Fine Then
                    tb = Array.CreateInstance(GetType(TimeBlock), 3)
                    tb(0) = New TimeBlock(t.Inizio, inizio, t.MarchersAmbulatorio, t.MarchersPaziente, t.MarchersVariabili)
                    tb(1) = New TimeBlock(inizio, fine, t.MarchersAmbulatorio, t.MarchersPaziente, t.MarchersVariabili, nuovoMarcher, tipo)
                    tb(2) = New TimeBlock(fine, t.Fine, t.MarchersAmbulatorio, t.MarchersPaziente, t.MarchersVariabili)
                ElseIf inizio = t.Inizio AndAlso fine < t.Fine Then
                    tb = Array.CreateInstance(GetType(TimeBlock), 2)
                    tb(0) = New TimeBlock(inizio, fine, t.MarchersAmbulatorio, t.MarchersPaziente, t.MarchersVariabili, nuovoMarcher, tipo)
                    tb(1) = New TimeBlock(fine, t.Fine, t.MarchersAmbulatorio, t.MarchersPaziente, t.MarchersVariabili)
                ElseIf inizio > t.Inizio AndAlso fine = t.Fine Then
                    tb = Array.CreateInstance(GetType(TimeBlock), 2)
                    tb(0) = New TimeBlock(t.Inizio, inizio, t.MarchersAmbulatorio, t.MarchersPaziente, t.MarchersVariabili)
                    tb(1) = New TimeBlock(inizio, t.Fine, t.MarchersAmbulatorio, t.MarchersPaziente, t.MarchersVariabili, nuovoMarcher, tipo)
                ElseIf inizio = t.Inizio AndAlso fine = t.Fine Then
                    tb = Array.CreateInstance(GetType(TimeBlock), 1)
                    tb(0) = New TimeBlock(inizio, fine, t.MarchersAmbulatorio, t.MarchersPaziente, t.MarchersVariabili, nuovoMarcher, tipo)
                End If
            End If

            Remove(t)
            For Each newt As TimeBlock In tb
                newt._ParentDay = _dayParent

                List.Insert(posizione, newt)
                posizione += 1
            Next
        End Sub

        Public Function GetTotalMinutesFree() As Integer
            Dim totMinutiGiorno As Integer = 0
            For Each t As TimeBlock In InnerList
                If t.IsFree(0) Then
                    totMinutiGiorno += t.Fine.Subtract(t.Inizio).TotalMinutes
                End If
            Next

            Return totMinutiGiorno
        End Function

        '@ -----------------------------------------------------------------------------
        '@ <summary>
        '@ Conteggia il numero di minuti liiberi nel gionro non considerando un determinato filtro.
        '@ </summary>
        '@ <param name="noMarcher">Marcher da non considerare</param>
        '@ <param name="tipo">Tipo di marcher</param>
        '@ <returns></returns>
        '@ <history>
        '@ 	[ssabbattini]	07/03/2006	Created
        '@ </history>
        '@ -----------------------------------------------------------------------------
        Public Function GetTotalMinutesFree(ByVal noMarcher As Integer, ByVal tipo As MarcherTipo) As Integer
            Dim totMinutiGiorno As Integer = 0
            For Each t As TimeBlock In InnerList
                If t.IsFree(0) Or t.HaveMarcher(noMarcher, tipo) Then
                    totMinutiGiorno += t.Fine.Subtract(t.Inizio).TotalMinutes
                End If
            Next

            Return totMinutiGiorno
        End Function

        Public Function GetEquispacedTime(ByVal num As Integer) As TimeBlock()
            If num > 0 Then
                Dim totMinutiGiorno As Integer = 0
                Dim aMinuti As Integer = 0
                For Each t As TimeBlock In InnerList
                    If t.IsFree(0) Then
                        totMinutiGiorno += t.Fine.Subtract(t.Inizio).TotalMinutes
                    End If
                Next

                If totMinutiGiorno > 0 Then
                    aMinuti = totMinutiGiorno / num

                    Dim retTimeBlock As New ArrayList(num)
                    For Each t As TimeBlock In innerlist
                        If t.IsFree(0) Then
                            Dim dimensione As Integer = t.Fine.Subtract(t.Inizio).TotalMinutes
                            Dim numeroDaPrenotare As Integer = dimensione / aMinuti

                            For i As Integer = 0 To numeroDaPrenotare
                                Dim newT As New TimeBlock(t.Inizio.AddMinutes(i * aMinuti), t.Inizio.AddMinutes((i + 1) * aMinuti))

                                retTimeBlock.Add(newT)
                            Next i
                        End If
                    Next

                    Return retTimeBlock.ToArray(GetType(TimeBlock))
                End If
            End If
            Return Nothing
        End Function


    End Class


End Namespace