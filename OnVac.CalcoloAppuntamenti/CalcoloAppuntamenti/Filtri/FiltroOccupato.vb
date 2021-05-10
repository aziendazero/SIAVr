Namespace Filtri
    <Serializable()> _
    Public Class FiltroOccupato
        Implements IFiltro

        Sub New(ByVal data As Date, ByVal inizio As Date, ByVal fine As Date)
            Giorno = data.Day
            Mese = data.Month
            Anno = data.Year

            DaOra = inizio
            AOra = fine
        End Sub
        Friend _FiltriParent As Filtri.FiltroCollection
        Public Property FiltriParent() As Filtri.FiltroCollection Implements IFiltro.FiltriParent
            Get
                Return _FiltriParent
            End Get
            Set(ByVal Value As Filtri.FiltroCollection)
                _FiltriParent = Value
            End Set
        End Property
        Public Function ApplicaMarcher(ByVal giorni As Timing.DayCollection, ByVal pars As Hashtable) As Timing.DayCollection Implements IFiltro.ApplicaMarcher
            For Each d As Timing.Day In giorni
                If Not d.Full Then
                    d = ApplicaMarcher(d, pars)
                End If
            Next

            Return giorni
        End Function

        Public Function ApplicaMarcher(ByVal giorno As Timing.Day, ByVal pars As Hashtable) As Timing.Day Implements IFiltro.ApplicaMarcher
            If giorno.Giorno.Day = _Giorno AndAlso giorno.Giorno.Month = _Mese AndAlso giorno.Giorno.Year = _Anno Then
                giorno.TimeBlocks.Split(_DaOra, _AOra, Priorita, Tipo)
            End If

            Return giorno
        End Function

        Public ReadOnly Property Obbligatorio() As Boolean Implements IFiltro.Obbligatorio
            Get
                Return True
            End Get
        End Property

        Public ReadOnly Property Priorita() As Integer Implements IFiltro.Priorita
            Get
                Return 1
            End Get
        End Property

        Public ReadOnly Property Tipo() As Timing.MarcherTipo Implements IFiltro.Tipo
            Get
                Return Timing.MarcherTipo.Variabile
            End Get
        End Property

        Private _DaOra As Date
        Public Property DaOra() As Date
            Get
                Return _DaOra
            End Get
            Set(ByVal value As Date)
                _DaOra = Date.MinValue.Add(value.TimeOfDay)
            End Set
        End Property

        Private _AOra As Date
        Public Property AOra() As Date
            Get
                Return _AOra
            End Get
            Set(ByVal value As Date)
                _AOra = Date.MinValue.Add(value.TimeOfDay)
            End Set
        End Property

        Private _Giorno As Integer
        Public Property Giorno() As Integer
            Get
                Return _Giorno
            End Get
            Set(ByVal value As Integer)
                _Giorno = value
            End Set
        End Property

        Private _Mese As Integer
        Public Property Mese() As Integer
            Get
                Return _Mese
            End Get
            Set(ByVal value As Integer)
                _Mese = value
            End Set
        End Property

        Private _Anno As Integer
        Public Property Anno() As Integer
            Get
                Return _Anno
            End Get
            Set(ByVal value As Integer)
                _Anno = value
            End Set
        End Property

        Public ReadOnly Property GetMotivoRifiuto() As MotiviRifiuto.IMotivoRifiuto Implements IFiltro.GetMotivoRifiuto
            Get
                Return Nothing
            End Get
        End Property
    End Class
End Namespace
