
Namespace Collection

    <Serializable()> _
    Public Class LuoghiEsecuzioneVacCollection
        Inherits CollectionBase

        Default Public ReadOnly Property Item(ByVal indice As Integer) As LuogoEsecuzioneVac
            Get
                Return CType(List.Item(indice), LuogoEsecuzioneVac)
            End Get
        End Property


        Public Function Add(ByVal item As LuogoEsecuzioneVac) As Integer
            Return List.Add(item)
        End Function


        Public Function Contains(ByVal item As LuogoEsecuzioneVac) As Boolean
            Return List.Contains(item)
        End Function


        Public Sub Remove(ByVal item As LuogoEsecuzioneVac)
            List.Remove(item)
        End Sub


        Public Function Find(ByVal codiceLuogo As String) As LuogoEsecuzioneVac
            Dim _item As LuogoEsecuzioneVac
            For i As Integer = 0 To List.Count - 1
                _item = DirectCast(List(i), LuogoEsecuzioneVac)
                If (_item.CodiceLuogo = codiceLuogo) Then
                    Return _item
                End If
            Next
            Return Nothing
        End Function

    End Class


End Namespace
