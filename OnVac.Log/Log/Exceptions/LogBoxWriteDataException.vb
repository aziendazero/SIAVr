
Public Class LogBoxWriteDataException
    Inherits Exception

    Const ERROR_MESSAGE As String = "Errore di scrittura del Log di OnVac."

    Public Sub New()
        MyBase.New(ERROR_MESSAGE)
    End Sub

    Public Sub New(ex As Exception)
        MyBase.New(ERROR_MESSAGE, ex)
    End Sub

    Public Sub New(ex As Exception, msg As String)
        MyBase.New(ERROR_MESSAGE + " Dettaglio errore: " + msg, ex)
    End Sub

End Class

