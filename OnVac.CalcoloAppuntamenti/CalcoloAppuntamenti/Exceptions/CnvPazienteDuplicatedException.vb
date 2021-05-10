Namespace Exceptions
    Public Class CnvPazienteDuplicatedException
        Inherits System.Exception

        Sub New(ByVal paziente As CnvPaziente)
            MyBase.New(String.Format("Il paziente {0} {1} con data di convocazione {2} esiste già nella collezione.", paziente.Nome, paziente.Cognome, paziente.DataConvocazione))
        End Sub
    End Class
End Namespace