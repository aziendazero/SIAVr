Namespace LogWriterProvider

    Public Class LogWriterProviderCollection
        Inherits CollectionBase

        Private _hashTable As New Hashtable()

        Default Public Property Item(index As Integer) As ILogWriterProvider
            Get
                Return CType(List(index), ILogWriterProvider)
            End Get
            Set(Value As ILogWriterProvider)
                List(index) = Value
            End Set
        End Property

        Default Public Property Item(name As String) As ILogWriterProvider
            Get
                Return _hashTable(name)
            End Get
            Set(Value As ILogWriterProvider)
                _hashTable(name) = Value
            End Set
        End Property

        Public Function Add(value As ILogWriterProvider) As Integer
            _hashTable.Add(value.Name, value)
            Return List.Add(value)
        End Function

        Public Sub Insert(index As Integer, value As ILogWriterProvider)
            _hashTable.Add(value.Name, value)
            List.Insert(index, value)
        End Sub

        Public Sub Remove(value As ILogWriterProvider)
            _hashTable.Remove(value.Name)
            List.Remove(value)
        End Sub

        Public Function Contains(value As ILogWriterProvider) As Boolean
            Return List.Contains(value)
        End Function

    End Class

End Namespace
