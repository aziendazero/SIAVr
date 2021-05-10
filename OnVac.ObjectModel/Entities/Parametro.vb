Namespace Entities

    <Serializable()>
    Public Class Parametro

#Region " Properties "

        Private _codice As String
        Public Property Codice() As String
            Get
                Return _codice
            End Get
            Set(ByVal Value As String)
                _codice = Value
            End Set
        End Property

        Private _consultorio As String
        Public Property Consultorio() As String
            Get
                Return _consultorio
            End Get
            Set(ByVal Value As String)
                _consultorio = Value
            End Set
        End Property

        Private _valore As String
        Public Property Valore() As String
            Get
                Return _valore
            End Get
            Set(ByVal Value As String)
                _valore = Value
            End Set
        End Property

        Private _descrizione As String
        Public Property Descrizione() As String
            Get
                Return _descrizione
            End Get
            Set(ByVal Value As String)
                _descrizione = Value
            End Set
        End Property

        Private _centrale As Boolean
        Public Property Centrale() As Boolean
            Get
                Return _centrale
            End Get
            Set(ByVal Value As Boolean)
                _centrale = Value
            End Set
        End Property

#End Region

#Region " Constructors "

        Public Sub New()
            _codice = String.Empty
            _consultorio = String.Empty
            _valore = String.Empty
            _descrizione = String.Empty
            _centrale = False
        End Sub

#End Region

    End Class

End Namespace

