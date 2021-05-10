Namespace Entities

    Public Class LogDatiVaccinali

        Public Property Id As Int64
        Public Property DataOperazione As DateTime

        Private _Utente As Utente
        Public ReadOnly Property Utente As Utente
            Get
                If _Utente Is Nothing Then
                    _Utente = New Utente()
                End If
                Return _Utente
            End Get
        End Property

        Private _Usl As Usl
        Public ReadOnly Property Usl As Usl
            Get
                If _Usl Is Nothing Then
                    _Usl = New Usl()
                End If
                Return _Usl
            End Get
        End Property

        Private _Paziente As Paziente
        Public ReadOnly Property Paziente As Paziente
            Get
                If _Paziente Is Nothing Then
                    _Paziente = New Paziente()
                End If
                Return _Paziente
            End Get
        End Property

        Private _Argomento As Log.DataLogStructure.Argomento
        Public Property Argomento() As Log.DataLogStructure.Argomento
            Get
                If _Argomento Is Nothing Then _Argomento = New Log.DataLogStructure.Argomento()
                Return _Argomento
            End Get
            Set(value As Log.DataLogStructure.Argomento)
                _Argomento = value
            End Set
        End Property

        Public Property Operazione As Log.DataLogStructure.Operazione
        Public Property Stato As Enumerators.StatoLogDatiVaccinaliCentrali
        Public Property Note As String

    End Class

End Namespace
