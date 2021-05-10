Namespace Common

    <Serializable()> _
    Public Class PageTracker

        Private _htName As Hashtable
        Private _htValue As Hashtable
        Private _sequence As Short = 0
        Private _size As Short

        Public ReadOnly Property PageItem(ByVal pagename As String)
            Get
                Return _htValue(pagename)
            End Get
        End Property

        Public Sub New(ByVal size As Short)
            _size = size
            _htName = New Hashtable
            _htValue = New Hashtable
        End Sub

        Public Sub Add(ByVal value As PageTrace)

            Dim position As Integer = _sequence Mod _size

            If _sequence > _size - 1 Then
                Dim keyToRemove As String = _htName(position)
                _htValue.Remove(keyToRemove)
            End If

            If _htValue.Contains(value.PageName) Then
                _htValue.Remove(value.PageName)
            Else
                _htName.Remove(position)
                _htName.Add(position, value.PageName)
                _sequence += 1
            End If

            _htValue.Add(value.PageName, value)

        End Sub

    End Class

End Namespace