Namespace Exceptions
    Public Class NotValidMarcherException
        Inherits System.Exception

        Sub New()
            MyBase.New(String.Format("Il marcher non è valido. Il valore deve essere un esponenete di due."))
        End Sub

        Sub New(ByVal marcher As Double)
            MyBase.New(String.Format("Il marcher non è valido. Il valore deve essere un esponenete di due. " & marcher))
        End Sub
    End Class
End Namespace
