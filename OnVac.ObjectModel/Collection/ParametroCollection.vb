Namespace Collection

    <Serializable()>
    Public Class ParametroCollection
        Inherits CollectionBase

        Public ReadOnly Property Item(index As Integer) As Parametro
            Get
                Return CType(List.Item(index), Parametro)
            End Get
        End Property

        Public Function Add(value As Parametro) As Integer

            Return List.Add(value)

        End Function

        Public Sub Remove(value As Parametro)

            List.Remove(value)

        End Sub

    End Class

End Namespace