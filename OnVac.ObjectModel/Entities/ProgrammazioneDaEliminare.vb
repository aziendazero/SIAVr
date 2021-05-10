Namespace Entities

    <Serializable()>
    Public Class ProgrammazioneDaEliminare

        Private _codicePaziente As Integer
        Public Property CodicePaziente() As Integer
            Get
                Return _codicePaziente
            End Get
            Set(ByVal Value As Integer)
                _codicePaziente = Value
            End Set
        End Property

        Private _dataConvocazione As Date
        Public Property DataConvocazione() As Date
            Get
                Return _dataConvocazione
            End Get
            Set(ByVal Value As Date)
                _dataConvocazione = Value
            End Set
        End Property

    End Class

End Namespace