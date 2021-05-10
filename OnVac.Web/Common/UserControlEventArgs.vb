Namespace Common


    Public Class UserControlEventArgs
        Inherits System.EventArgs


        Private _text As String
        Public Property Text() As String
            Get
                Return _text
            End Get
            Set(ByVal value As String)
                _text = value
            End Set
        End Property

        Public Sub New(ByVal text As String)
            _text = text
        End Sub

    End Class


End Namespace