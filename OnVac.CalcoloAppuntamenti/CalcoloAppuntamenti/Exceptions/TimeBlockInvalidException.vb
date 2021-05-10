Namespace Exceptions
    Public Class TimeBlockInvalidException
        Inherits System.Exception

        Sub New(ByVal inizio As Date, ByVal fine As Date)
            MyBase.New(String.Format("Il periodo da {0} e {1} è più grande del TimeBlock", inizio.ToShortTimeString, fine.ToShortTimeString))
        End Sub

        Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub
    End Class
End Namespace
