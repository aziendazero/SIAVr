''' <summary>
''' Eccezione sollevata a causa di una possibile duplicazione di dati che devono essere univoci.
''' </summary>
Public Class DuplicatedDataException
    Inherits Exception

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(msg As String)
        MyBase.New(msg)
    End Sub

    Public Sub New(msg As String, innerException As Exception)
        MyBase.New(msg, innerException)
    End Sub

End Class
