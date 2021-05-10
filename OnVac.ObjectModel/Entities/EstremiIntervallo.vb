Namespace Entities

    ''' <summary>
    ''' Classe che rappresenta un generico intervallo, esponendo come proprietà pubbliche i due estremi.
    ''' </summary>
    Public Class EstremiIntervallo


        Private _estremo_inf As Integer
        Public Property EstremoInferiore() As Integer
            Get
                Return _estremo_inf
            End Get
            Set(ByVal Value As Integer)
                _estremo_inf = Value
            End Set
        End Property


        Private _estremo_sup As Integer
        Public Property EstremoSuperiore() As Integer
            Get
                Return _estremo_sup
            End Get
            Set(ByVal Value As Integer)
                _estremo_sup = Value
            End Set
        End Property



        Public Sub New()
        End Sub


        Public Sub New(ByVal estremo_inferiore As Integer, ByVal estremo_superiore As Integer)
            _estremo_inf = estremo_inferiore
            _estremo_sup = estremo_superiore
        End Sub


    End Class


End Namespace
