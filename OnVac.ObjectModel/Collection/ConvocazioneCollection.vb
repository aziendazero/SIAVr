
Namespace Collection

    <Serializable()>
    Public Class ConvocazioneCollection
        Inherits CollectionBase

        Default Public ReadOnly Property Item(indice As Integer) As Entities.Convocazione
            Get
                Return DirectCast(Me.List(indice), Entities.Convocazione)
            End Get
        End Property

        Public Function Add(value As Entities.Convocazione) As Integer

            Return Me.List.Add(value)

        End Function

        Public Sub Remove(value As Entities.Convocazione)

            Me.List.Remove(value)

        End Sub

    End Class

End Namespace
