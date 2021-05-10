Namespace Common

    <Serializable()> _
    Public Class PageTrace

        Private _ht As Hashtable
        Private _pagename As String

        Public Property PageName() As String
            Get
                Return _pagename
            End Get
            Set(ByVal Value As String)
                _pagename = Value
            End Set
        End Property

        Public Sub New(ByVal pagename As String)
            _ht = New Hashtable
            _pagename = pagename
        End Sub

        Public Sub Add(ByVal key As String, ByVal value As String)
            If _ht.Contains(key) Then
                _ht.Remove(key)
            End If

            _ht.Add(key, value)

        End Sub

        Public Function GetValue(ByVal key As String) As String
            Return _ht(key)
        End Function
    End Class

End Namespace