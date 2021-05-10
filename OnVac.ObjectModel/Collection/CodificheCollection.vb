
Imports System.Collections.Generic

Namespace Collection


    <Serializable()>
    Public Class CodificheCollection
        Inherits CollectionBase
        Implements IEnumerable(Of Codifica)


        Default Public ReadOnly Property Item(ByVal index As Integer) As Codifica
            Get
                Return CType(List.Item(index), Codifica)
            End Get
        End Property


        Public Function Add(ByVal value As Codifica) As Integer
            Return List.Add(value)
        End Function


        Public Sub Remove(ByVal value As Codifica)
            List.Remove(value)
        End Sub

        Private Function IEnumerable_GetEnumerator() As IEnumerator(Of Codifica) Implements IEnumerable(Of Codifica).GetEnumerator
            Dim ritorno As List(Of Codifica) = New List(Of Codifica)
            For Index As Integer = 0 To Me.Count - 1
                ritorno.Add(Me.Item(Index))
            Next
            Return ritorno.GetEnumerator()
        End Function
    End Class


End Namespace
