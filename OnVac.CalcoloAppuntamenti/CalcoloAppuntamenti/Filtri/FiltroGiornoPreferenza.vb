Imports Onit.OnAssistnet.OnVac.CalcoloAppuntamenti.Timing

Namespace Filtri
    <Serializable()> _
    Public Class FiltroGiornoPreferenza
        Implements IFiltro

        Sub New()

        End Sub

        Sub New(ByVal bits As Integer)
            _bits = bits
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
        Private _bits As Integer

        Public Sub SetGiorniPreferenza(ByVal value() As DayOfWeek)
            For i As Integer = 0 To value.Length - 1
                _bits = _bits Or 2 ^ value(i)
            Next
        End Sub

        Private Function ContainDay(ByVal day As DayOfWeek) As Boolean
            Return ((_bits And (2 ^ day)) = (2 ^ day))
        End Function

        Public Function ApplicaMarcher(ByVal giorni As Timing.DayCollection, ByVal pars As Hashtable) As Timing.DayCollection Implements IFiltro.ApplicaMarcher
            For Each d As Day In giorni
                If Not d.Full Then
                    If Not ContainDay(d.Giorno.DayOfWeek) Then
                        d = ApplicaMarcher(d, pars)
                    End If
                End If
            Next

            Return giorni
        End Function

        Public Function ApplicaMarcher(ByVal giorno As Timing.Day, ByVal pars As Hashtable) As Timing.Day Implements IFiltro.ApplicaMarcher
            For Each t As TimeBlock In giorno.TimeBlocks
                t.SetMarcher(Priorita, Tipo)
            Next
            Return giorno
        End Function

        Public ReadOnly Property Obbligatorio() As Boolean Implements IFiltro.Obbligatorio
            Get
                Return False
            End Get
        End Property

        Public ReadOnly Property Priorita() As Integer Implements IFiltro.Priorita
            Get
                Return 4
            End Get
        End Property

        Public ReadOnly Property Tipo() As Timing.MarcherTipo Implements IFiltro.Tipo
            Get
                Return MarcherTipo.Paziente
            End Get
        End Property


        Public ReadOnly Property GetMotivoRifiuto() As MotiviRifiuto.IMotivoRifiuto Implements IFiltro.GetMotivoRifiuto
            Get
                Return New MotiviRifiuto.RifiutoGiornoPreferenza
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("Giorni: {0}", _bits.ToString)
        End Function

    End Class
End Namespace
