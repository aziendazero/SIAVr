Imports Onit.OnAssistnet.OnVac.CalcoloAppuntamenti.Timing

Namespace Filtri
    <Serializable()> _
    Public Class FiltroDataValidita
        Implements IFiltro
        Friend _FiltriParent As Filtri.FiltroCollection
        Public Property FiltriParent() As Filtri.FiltroCollection Implements IFiltro.FiltriParent
            Get
                Return _FiltriParent
            End Get
            Set(ByVal Value As Filtri.FiltroCollection)
                _FiltriParent = Value
            End Set
        End Property
        Private _DataValidita As Date
        Public Property DataValidita() As Date
            Get
                Return _DataValidita
            End Get
            Set(ByVal value As Date)
                _DataValidita = value.Date
            End Set
        End Property

        Private _DataCnv As Date
        Public Property DataCnv() As Date
            Get
                Return _DataCnv
            End Get
            Set(ByVal value As Date)
                _DataCnv = value.Date
            End Set
        End Property

        Public Function ApplicaMarcher(ByVal giorni As Timing.DayCollection, ByVal pars As Hashtable) As Timing.DayCollection Implements IFiltro.ApplicaMarcher
            For Each d As Day In giorni
                If Not d.Full Then
                    d = ApplicaMarcher(d, pars)
                End If
            Next

            Return giorni
        End Function

        Public Function ApplicaMarcher(ByVal giorno As Timing.Day, ByVal pars As Hashtable) As Timing.Day Implements IFiltro.ApplicaMarcher
            If giorno.Giorno < _DataCnv OrElse giorno.Giorno > _DataValidita Then
                For Each t As TimeBlock In giorno.TimeBlocks
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
                Return 2
            End Get
        End Property

        Public ReadOnly Property Tipo() As Timing.MarcherTipo Implements IFiltro.Tipo
            Get
                Return MarcherTipo.Paziente
            End Get
        End Property

        Public ReadOnly Property GetMotivoRifiuto() As MotiviRifiuto.IMotivoRifiuto Implements IFiltro.GetMotivoRifiuto
            Get
                Return New MotiviRifiuto.RifiutoDataValidita
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("DaData: {0} - AData: {1}", _DataCnv.ToShortDateString, _DataValidita.ToShortDateString)
        End Function
    End Class
End Namespace
