Namespace Filtri
    <Serializable()> _
    Public Class FiltroNumeroMassimoAppuntamentiBlocco
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
            Dim m As New Filtri.FiltroOccupato(Date.MinValue, Date.MinValue, Date.MinValue)

            For Each d As Timing.Day In giorni
                For i As Int16 = 0 To _FiltriParent.NumeroMassimoBlocchiPersonalizzatiGiornalieri

                    Dim parName As String = GetNewParameterID(d.Giorno, i)
                    If Not pars(parName) Is Nothing Then
                        Dim pb As ParameterBlocco = pars(parName)

                        If pb.Numero = -1 Then pb.Numero = Integer.MaxValue
                        If pb.Durata < 0 Then pb.Durata = 0

                        If Not d.Full Then
                            'Dim conteggio As Integer = 0
                            pb.Conteggio = 0

                            ApplicaMarcher(d, pars)

                            For Each t As Timing.TimeBlock In d.TimeBlocks
                                If t.Inizio >= pb.DaOra AndAlso t.Fine <= pb.AOra Then
                                    If t.HaveMarcher(m.Priorita, m.Tipo) Then
                                        pb.Conteggio += 1
                                    End If
                                End If
                            Next

                            If pb.Conteggio >= pb.Numero Then
                                For Each t As Timing.TimeBlock In d.TimeBlocks
                                    If t.Inizio >= pb.DaOra AndAlso t.Fine <= pb.AOra Then
                                        t.SetMarcher(Priorita, Tipo)
                                    End If
                                Next
                                'd.Full = True
                            End If
                        End If
                    End If
                Next
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
                Return 32
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

        Public Shared NumeroMassimoPrenotatiBlocco As String = "NumeroMassimoPrenotatiBlocco"

        Public Shared Function GetNewParameterID(ByVal data As Date, ByVal index As Int16) As String
            Return String.Format("{0}_{1}_{2}", NumeroMassimoPrenotatiBlocco, data, index)
        End Function

        Public ReadOnly Property GetMotivoRifiuto() As MotiviRifiuto.IMotivoRifiuto Implements IFiltro.GetMotivoRifiuto
            Get
                Return New MotiviRifiuto.RifiutoMassimoNumero("Massimo prenotati blocco")
            End Get
        End Property
    End Class

    <Serializable()> _
    Public Class ParameterBlocco
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

        Private _Numero As Integer
        Public Property Numero() As Integer
            Get
                Return _Numero
            End Get
            Set(ByVal value As Integer)
                _Numero = value
            End Set
        End Property

        Private _Durata As Integer
        Public Property Durata() As Integer
            Get
                Return _Durata
            End Get
            Set(ByVal value As Integer)
                _Durata = value
            End Set
        End Property

        Private _Conteggio As Integer
        Public Property Conteggio() As Integer
            Get
                Return _Conteggio
            End Get
            Set(ByVal value As Integer)
                _Conteggio = value
            End Set
        End Property

        Private _OverrideDurata As Boolean
        Public Property OverrideDurata() As Boolean
            Get
                Return _OverrideDurata
            End Get
            Set(ByVal value As Boolean)
                _OverrideDurata = value
            End Set
        End Property
    End Class
End Namespace
