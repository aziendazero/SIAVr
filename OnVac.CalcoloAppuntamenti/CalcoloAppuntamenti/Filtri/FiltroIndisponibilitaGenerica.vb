Imports Onit.OnAssistnet.OnVac.CalcoloAppuntamenti.Timing

Namespace Filtri
    <Serializable()> _
    Public Class FiltroIndisponibilitaGenerica
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

        Private _Ricorsivita As Ricorsivita
        Public Property Ricorsivita() As Ricorsivita
            Get
                Return _Ricorsivita
            End Get
            Set(ByVal value As Ricorsivita)
                _Ricorsivita = value
                _DaOra = Date.MinValue.Add(_DaOra.TimeOfDay)
                _AOra = Date.MinValue.Add(_AOra.TimeOfDay)
            End Set
        End Property

        Private _GiornoSettimana As DayOfWeek
        Public Property GiornoSettimana() As DayOfWeek
            Get
                Return _GiornoSettimana
            End Get
            Set(ByVal value As DayOfWeek)
                _GiornoSettimana = value
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
            Select Case _Ricorsivita
                Case Ricorsivita.Settimanale
                    If giorno.Giorno.DayOfWeek = _GiornoSettimana Then
                        giorno.TimeBlocks.Split(_DaOra, _AOra, Priorita, Tipo)
                        If _DaOra = Day.MinOrario And _AOra = Day.MaxOrario Then
                            giorno.Full = True
                        End If
                    End If

                Case Ricorsivita.Singola
                    If giorno.Giorno.Day = _Giorno AndAlso giorno.Giorno.Month = _Mese AndAlso giorno.Giorno.Year = _Anno Then
                        giorno.TimeBlocks.Split(_DaOra, _AOra, Priorita, Tipo)
                        If _DaOra = Day.MinOrario And _AOra = Day.MaxOrario Then
                            giorno.Full = True
                        End If
                    End If

                Case Ricorsivita.Annuale
                    If giorno.Giorno.Day = _Giorno AndAlso giorno.Giorno.Month = _Mese Then
                        giorno.TimeBlocks.Split(_DaOra, _AOra, Priorita, Tipo)
                        If _DaOra = Day.MinOrario And _AOra = Day.MaxOrario Then
                            giorno.Full = True
                        End If
                    End If

                Case Ricorsivita.Mensile
                    If giorno.Giorno.Day = _Giorno Then
                        giorno.TimeBlocks.Split(_DaOra, _AOra, Priorita, Tipo)
                        If _DaOra = Day.MinOrario And _AOra = Day.MaxOrario Then
                            giorno.Full = True
                        End If
                    End If

            End Select

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
                Return MarcherTipo.Ambulatorio
            End Get
        End Property

        Public ReadOnly Property GetMotivoRifiuto() As MotiviRifiuto.IMotivoRifiuto Implements IFiltro.GetMotivoRifiuto
            Get
                Return Nothing
            End Get
        End Property
    End Class
End Namespace
