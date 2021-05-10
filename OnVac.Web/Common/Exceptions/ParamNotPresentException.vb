Public Class ParamNotPresentException
    Inherits Exception

    Public Sub New()
        MyBase.New("Parameter not present")
    End Sub

    Public Sub New(parameterName As String)
        MyBase.New(String.Format("Parameter not present: {0}", parameterName))
    End Sub

End Class
