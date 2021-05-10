Namespace Entities

    <Serializable()> _
    Public Class Ciclo

        Private _codice As String
        Public Property Codice() As String
            Get
                Return _codice
            End Get
            Set(ByVal value As String)
                _codice = value
            End Set
        End Property

        Private _descrizione As String
        Public Property Descrizione() As String
            Get
                Return _descrizione
            End Get
            Set(ByVal value As String)
                _descrizione = value
            End Set
        End Property

        Private _dataIntroduzione As Date
        Public Property DataIntroduzione() As Date
            Get
                Return _dataIntroduzione
            End Get
            Set(ByVal value As Date)
                _dataIntroduzione = value
            End Set
        End Property

        ' Valori possibili: T o F
        Private _standard As Boolean
        Public Property Standard() As Boolean
            Get
                Return _standard
            End Get
            Set(ByVal value As Boolean)
                _standard = value
            End Set
        End Property

        Private _numSedute As Integer
        Public Property NumSedute() As Integer
            Get
                Return _numSedute
            End Get
            Set(ByVal value As Integer)
                _numSedute = value
            End Set
        End Property

        Private _eta As Integer
        Public Property Eta() As Integer
            Get
                Return _eta
            End Get
            Set(ByVal value As Integer)
                _eta = value
            End Set
        End Property

        Private _dataFine As Date
        Public Property DataFine() As Date
            Get
                Return _dataFine
            End Get
            Set(ByVal value As Date)
                _dataFine = value
            End Set
        End Property

        ' Valori possibili: S o N
        Private _alert As Boolean
        Public Property Alert() As Boolean
            Get
                Return _alert
            End Get
            Set(ByVal value As Boolean)
                _alert = value
            End Set
        End Property

        Private _sesso As String
        Public Property Sesso() As String
            Get
                Return _sesso
            End Get
            Set(ByVal value As String)
                _sesso = value
            End Set
        End Property


    End Class


End Namespace
