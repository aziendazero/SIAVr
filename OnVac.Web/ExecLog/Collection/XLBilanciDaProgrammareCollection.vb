Imports Onit.OnAssistnet.OnVac.ExecLog.Entities

Namespace ExecLog.Collection
    <Serializable()> _
    Public Class XLBilanciDaProgrammareCollection
        Inherits CollectionBase

        Default Public ReadOnly Property Item(ByVal indice As Integer) As XLBilanciDaProgrammare
            Get
                Return List.Item(indice)
            End Get
        End Property

        Public Function Add(ByVal value As XLBilanciDaProgrammare) As Integer

            Return List.Add(value)

        End Function

        Public Sub Remove(ByVal value As XLBilanciDaProgrammare)

            List.Remove(value)

        End Sub

    End Class
End Namespace
