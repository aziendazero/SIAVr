Namespace Filtri
    <Serializable()> _
    Public Class FiltroTempoInvio
        Implements IFiltro

        Sub New()

        End Sub

        Sub New(ByVal data As Date)
            _DaData = data.Date
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
                    ApplicaMarcher(d, pars)
                End If
            Next

            Return giorni
        End Function

        Public Function ApplicaMarcher(ByVal giorno As Timing.Day, ByVal pars As Hashtable) As Timing.Day Implements IFiltro.ApplicaMarcher
            'Contreggia realmente i giorni lavorativi
            Dim d As Integer = NumeroGiorni

            Dim c1 As Integer = 0
            If Not _ConsideraSabato Then
                While d > 0
                    If DaData.AddDays(c1).DayOfWeek <> DayOfWeek.Saturday Then
                        d -= 1
                    End If
                    c1 += 1
                End While
            End If

            Dim c2 As Integer = 0
            d = NumeroGiorni
            If Not _ConsideraDomenica Then
                While d > 0
                    If DaData.AddDays(c2).DayOfWeek <> DayOfWeek.Sunday Then
                        d -= 1
                    End If
                    c2 += 1
                End While
            End If

            If giorno.Giorno < DaData.AddDays(NumeroGiorni + (c1 - NumeroGiorni) + (c2 - NumeroGiorni)) Then
                For Each t As Timing.TimeBlock In giorno.TimeBlocks
                    t.SetMarcher(Priorita, Tipo)
                Next
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
                Return 8
            End Get
        End Property

        Public ReadOnly Property Tipo() As Timing.MarcherTipo Implements IFiltro.Tipo
            Get
                Return Timing.MarcherTipo.Ambulatorio
            End Get
        End Property

        Private _DaData As Date
        Public Property DaData() As Date
            Get
                Return _DaData
            End Get
            Set(ByVal value As Date)
                _DaData = value.Date
            End Set
        End Property

        Private _NumeroGiorni As Integer
        Public Property NumeroGiorni() As Integer
            Get
                Return _NumeroGiorni
            End Get
            Set(ByVal value As Integer)
                _NumeroGiorni = value
            End Set
        End Property

        Private _ConsideraSabato As Boolean
        Public Property ConsideraSabato() As Boolean
            Get
                Return _ConsideraSabato
            End Get
            Set(ByVal value As Boolean)
                _ConsideraSabato = value
            End Set
        End Property

        Private _ConsideraDomenica As Boolean
        Public Property ConsideraDomenica() As Boolean
            Get
                Return _ConsideraDomenica
            End Get
            Set(ByVal value As Boolean)
                _ConsideraDomenica = value
            End Set
        End Property

        Public ReadOnly Property GetMotivoRifiuto() As MotiviRifiuto.IMotivoRifiuto Implements IFiltro.GetMotivoRifiuto
            Get
                Return New MotiviRifiuto.RifiutoDataValidita
            End Get
        End Property
    End Class
End Namespace
