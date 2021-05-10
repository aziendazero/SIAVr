Namespace ObjectModel

    <Serializable()>
    Friend Class DatiIntervalliSeduteClass

        Private _intervallo As Integer
        Public Property IntervalloProssima() As Integer
            Get
                Return _intervallo
            End Get
            Set(Value As Integer)
                _intervallo = Value
            End Set
        End Property

        Private _dataEffettuazione As Date
        Public Property DataEffettuazione() As Date
            Get
                Return _dataEffettuazione
            End Get
            Set(Value As Date)
                _dataEffettuazione = Value
            End Set
        End Property

    End Class

End Namespace
