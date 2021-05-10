Namespace Entities


    <Serializable()> _
    Public Class PazienteAlias
        Inherits Paziente

        Private _cognomeMaster As String
        Public Property CognomeMaster() As String
            Get
                Return _cognomeMaster
            End Get
            Set(ByVal value As String)
                _cognomeMaster = value
            End Set
        End Property

        Private _nomeMaster As String
        Public Property NomeMaster() As String
            Get
                Return _nomeMaster
            End Get
            Set(ByVal value As String)
                _nomeMaster = value
            End Set
        End Property

        Private _dataNascitaMaster As DateTime
        Public Property DataNascitaMaster() As DateTime
            Get
                Return _dataNascitaMaster
            End Get
            Set(ByVal value As DateTime)
                _dataNascitaMaster = value
            End Set
        End Property

        Private _codFiscaleMaster As String
        Public Property CodiceFiscaleMaster() As String
            Get
                Return _codFiscaleMaster
            End Get
            Set(ByVal value As String)
                _codFiscaleMaster = value
            End Set
        End Property

        Private _CodicePazienteMaster As Integer
        Public Property CodicePazienteMaster() As Integer
            Get
                Return _CodicePazienteMaster
            End Get
            Set(ByVal Value As Integer)
                _CodicePazienteMaster = Value
            End Set
        End Property

        Private _idUtente As Integer
        Public Property IdUtente() As Integer
            Get
                Return _idUtente
            End Get
            Set(ByVal value As Integer)
                _idUtente = value
            End Set
        End Property

        Private _dataAlias As DateTime
        Public Property DataAlias() As DateTime
            Get
                Return _dataAlias
            End Get
            Set(ByVal value As DateTime)
                _dataAlias = value
            End Set
        End Property


    End Class


End Namespace

