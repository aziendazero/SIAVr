Namespace Filtri
    <Serializable()> _
    Public Class FiltroNumeroMassimoNuoviAppuntamentiGiornalieri
        Implements IFiltro

        Sub New()

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
        Sub New(ByVal data As Date, ByVal inizio As Date, ByVal fine As Date)
            Giorno = data.Day
            Mese = data.Month
            Anno = data.Year

            DaOra = inizio
            AOra = fine
        End Sub

        Public Function ApplicaMarcher(ByVal giorni As Timing.DayCollection, ByVal pars As Hashtable) As Timing.DayCollection Implements IFiltro.ApplicaMarcher
            If Not pars(NumeroMassimoNuoviPrenotatiGiornaliero) Is Nothing Then
                For Each d As Timing.Day In giorni
                    If Not d.Full Then
                        Dim conteggio As Integer = 0

                        ApplicaMarcher(d, pars)

                        For Each t As Timing.TimeBlock In d.TimeBlocks
                            If t.HaveMarcher(Priorita, Tipo) Then
                                conteggio += 1
                            End If
                        Next

                        If conteggio >= pars(NumeroMassimoNuoviPrenotatiGiornaliero) Then
                            For Each t As Timing.TimeBlock In d.TimeBlocks
                                t.SetMarcher(Priorita, Tipo)
                            Next
                            d.Full = True
                        End If
                    End If
                Next
            End If

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
                Return 4
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

        Public Shared NumeroMassimoNuoviPrenotatiGiornaliero As String = "NumeroMassimoNuoviPrenotatiGiornaliero"

        Public ReadOnly Property GetMotivoRifiuto() As MotiviRifiuto.IMotivoRifiuto Implements IFiltro.GetMotivoRifiuto
            Get
                Return New MotiviRifiuto.RifiutoMassimoNumero("Massimo prenotati nuovi")
            End Get
        End Property
    End Class
End Namespace
