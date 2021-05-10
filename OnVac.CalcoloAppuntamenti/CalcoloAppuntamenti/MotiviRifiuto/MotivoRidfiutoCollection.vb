Namespace MotiviRifiuto
    Public Class MotivoRifiutoCollection
        Inherits CollectionBase

        Default Public Property Item(ByVal index As Integer) As IMotivoRifiuto
            Get
                Return CType(List(index), IMotivoRifiuto)
            End Get
            Set(ByVal Value As IMotivoRifiuto)
                List(index) = Value
            End Set
        End Property

        Public Function Add(ByVal value As IMotivoRifiuto) As Integer
            Return List.Add(value)
        End Function

        Public Sub Remove(ByVal value As IMotivoRifiuto)
            List.Remove(value)
        End Sub
    End Class
End Namespace