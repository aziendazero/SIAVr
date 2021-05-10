Imports Onit.OnAssistnet.OnVac.CalcoloAppuntamenti.Timing

Namespace Filtri
    <Serializable()> _
    Public Class FiltroMattinaPomeriggio
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
        Private _InizioPomeriggio As Date
        Public Property InizioPomeriggio() As Date
            Get
                Return _InizioPomeriggio
            End Get
            Set(ByVal value As Date)
                _InizioPomeriggio = Date.MinValue.Add(value.TimeOfDay)
            End Set
        End Property

        Private _MattinaPomeriggio As MattinaPomeriggio
        Public Property MattinaPomeriggio() As MattinaPomeriggio
            Get
                Return _MattinaPomeriggio
            End Get
            Set(ByVal value As MattinaPomeriggio)
                _MattinaPomeriggio = value
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
            Dim index As Integer
            Dim tDivide As TimeBlock = giorno.TimeBlocks.GetItemByStart(_InizioPomeriggio, index)

            giorno.TimeBlocks.Split(_InizioPomeriggio, tDivide.Fine, 0, Tipo)

            For Each t As TimeBlock In giorno.TimeBlocks
                If _MattinaPomeriggio = MattinaPomeriggio.Mattina Then
                    If t.Inizio >= _InizioPomeriggio Then
                        t.SetMarcher(Priorita, Tipo)
                    End If
                Else
                    If t.Inizio < _InizioPomeriggio Then
                        t.SetMarcher(Priorita, Tipo)
                    End If
                End If
            Next

            Return giorno
        End Function

        Public Overridable ReadOnly Property Obbligatorio() As Boolean Implements IFiltro.Obbligatorio
            Get
                Return False
            End Get
        End Property

        Public Overridable ReadOnly Property Priorita() As Integer Implements IFiltro.Priorita
            Get
                Return 1
            End Get
        End Property

        Public Overridable ReadOnly Property Tipo() As Timing.MarcherTipo Implements IFiltro.Tipo
            Get
                Return MarcherTipo.Paziente
            End Get
        End Property


        Public ReadOnly Property GetMotivoRifiuto() As MotiviRifiuto.IMotivoRifiuto Implements IFiltro.GetMotivoRifiuto
            Get
                Return New MotiviRifiuto.RifiutoMattinaPomeriggio
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("Preferenza: {0}", _MattinaPomeriggio.ToString())
        End Function
    End Class
End Namespace
