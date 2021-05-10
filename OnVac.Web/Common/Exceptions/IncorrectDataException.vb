Public Class IncorrectDataException
    Inherits Exception

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(msg As String, innerException As Exception)
        MyBase.New(msg, innerException)
    End Sub

End Class

