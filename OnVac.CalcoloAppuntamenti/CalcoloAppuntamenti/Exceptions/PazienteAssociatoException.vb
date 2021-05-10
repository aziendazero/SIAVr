Namespace Exceptions
    Public Class PazienteAssociatoException
        Inherits System.Exception

        Sub New(ByVal ambulatorio As String)
            MyBase.New(String.Format("Il paziente � gi� associato ad un ambulatorio, impossibile cambiarne l'ubicazione. Rimuoverlo prima dalla collezione dell'ambulatorio.", ambulatorio))
        End Sub
    End Class
End Namespace
