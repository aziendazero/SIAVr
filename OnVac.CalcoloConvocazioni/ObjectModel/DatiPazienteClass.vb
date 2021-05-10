Namespace ObjectModel

    <Serializable()>
    Friend Class DatiPazienteClass

        Private _codice As Integer
        Public Property Codice() As Integer
            Get
                Return _codice
            End Get
            Set(Value As Integer)
                _codice = Value
            End Set
        End Property

        Private _cognome As String
        Public Property Cognome() As String
            Get
                Return _cognome
            End Get
            Set(Value As String)
                _cognome = Value
            End Set
        End Property

        Private _nome As String
        Public Property Nome() As String
            Get
                Return _nome
            End Get
            Set(Value As String)
                _nome = Value
            End Set
        End Property

        Private _dataNascita As Date
        Public Property DataNascita() As Date
            Get
                Return _dataNascita
            End Get
            Set(Value As Date)
                _dataNascita = Value
            End Set
        End Property

        ''' <summary>
        ''' Età in giorni
        ''' </summary>
        Public ReadOnly Property Eta() As Integer
            Get
                Return Date.Now.Subtract(_dataNascita).Days
            End Get
        End Property

        Private _codiceConsultorio As String
        Public Property CodiceConsultorio() As String
            Get
                Return _codiceConsultorio
            End Get
            Set(Value As String)
                _codiceConsultorio = Value
            End Set
        End Property

        Private _statoVaccinale As String
        Public Property StatoVaccinale() As String
            Get
                Return _statoVaccinale
            End Get
            Set(Value As String)
                _statoVaccinale = Value
            End Set
        End Property

    End Class

End Namespace
