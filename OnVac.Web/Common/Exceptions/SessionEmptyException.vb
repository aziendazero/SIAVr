Public Class SessionEmptyException
    Inherits Exception

    Public Sub New()
        MyBase.New("Session is empty")
    End Sub

    Public Sub New(pageName As String)
        MyBase.New(String.Format("Session in {0} is empty", pageName))
    End Sub

End Class
