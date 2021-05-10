Namespace Timing
    <Serializable()> _
    Public Class Day

        Public Event DayChanged(ByVal d As Day, ByVal oldDate As Date, ByVal newDate As Date)

        Sub New(ByVal g As Date)
            _Giorno = g.Date
            _TimeBlocks = New TimeBlockCollection(Me)

            Dim t As New TimeBlock(Day.MinOrario, Day.MaxOrario)
            _TimeBlocks.Add(t)
        End Sub

        Private _Giorno As Date
        Public Property Giorno() As Date
            Get
                Return _Giorno
            End Get
            Set(ByVal value As Date)
                RaiseEvent DayChanged(Me, _Giorno, value.Date)
                _Giorno = value.Date
            End Set
        End Property

        Private _Full As Boolean
        Public Property Full() As Boolean
            Get
                Return _Full
            End Get
            Set(ByVal value As Boolean)
                _Full = value
            End Set
        End Property

        Private _TimeBlocks As TimeBlockCollection
        Public ReadOnly Property TimeBlocks() As TimeBlockCollection
            Get
                Return _TimeBlocks
            End Get
        End Property

        Public Function CheckIsFull(ByVal filters As Filtri.FiltroCollection) As Boolean
            If Not Full Then
                Dim markFull As Boolean = True
                For Each t As TimeBlock In _TimeBlocks
                    For Each f As Filtri.IFiltro In filters
                        If f.Obbligatorio Then
                            markFull = markFull And t.HaveMarcher(f.Priorita, f.Tipo)
                        End If
                    Next
                Next
                _Full = markFull
                Return markFull
            End If
            Return False
        End Function

        Public Shared MinOrario As Date = Date.MinValue
        Public Shared MaxOrario As Date = Date.MinValue.AddTicks(TimeSpan.TicksPerDay - 1)
    End Class
End Namespace