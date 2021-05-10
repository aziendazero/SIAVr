Namespace Exceptions
    Public Class NotExistAmbulatorioException
        Inherits System.Exception

        Sub New(ByVal ambulatorio As String)
            MyBase.New(String.Format("L'ambulatorio {0} associato al paziente non � incluso nella collezione degli ambulatori.", Ambulatorio))
        End Sub
    End Class
End Namespace
