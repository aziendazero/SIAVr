Namespace Filtri
    <Serializable()> _
    Public Class FiltroDataMinima
        Implements Filtri.IFiltro

        Sub New()
            _Giorno = Date.Now.Date
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
        Public Overloads Function ApplicaMarcher(ByVal giorni As Timing.DayCollection, ByVal pars As System.Collections.Hashtable) As Timing.DayCollection Implements Filtri.IFiltro.ApplicaMarcher
            For Each d As Timing.Day In giorni
                If Not d.Full Then
                    d = ApplicaMarcher(d, pars)
                End If
            Next

            Return giorni
        End Function

        Public Overloads Function ApplicaMarcher(ByVal giorno As Timing.Day, ByVal pars As System.Collections.Hashtable) As Timing.Day Implements Filtri.IFiltro.ApplicaMarcher
            If giorno.Giorno < _Giorno Then
                For Each t As Timing.TimeBlock In giorno.TimeBlocks
                    t.SetMarcher(Priorita, Tipo)
                Next
            End If

            Return giorno
        End Function

        Public ReadOnly Property GetMotivoRifiuto() As MotiviRifiuto.IMotivoRifiuto Implements Filtri.IFiltro.GetMotivoRifiuto
            Get
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property Obbligatorio() As Boolean Implements Filtri.IFiltro.Obbligatorio
            Get
                Return True
            End Get
        End Property

        Public ReadOnly Property Priorita() As Integer Implements Filtri.IFiltro.Priorita
            Get
                Return 2
            End Get
        End Property

        Public ReadOnly Property Tipo() As Timing.MarcherTipo Implements Filtri.IFiltro.Tipo
            Get
                Return Timing.MarcherTipo.Ambulatorio
            End Get
        End Property

        Private _Giorno As Date
        Public Property Giorno() As Date
            Get
                Return _Giorno
            End Get
            Set(ByVal Value As Date)
                _Giorno = Value
            End Set
        End Property
    End Class
End Namespace