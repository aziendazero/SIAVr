Public Class BizToUserMessageException
    Inherits Exception

    Public Const PlaceHolderKey As String = "PlaceHolderKey"

    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="resKey">chiave nelle risorse</param>
    ''' <param name="defautLanguageMessage">messaggio in italiano, da scrivere in eventuali log</param>
    Public Sub New(resKey As String, defautLanguageMessage As String)
        MyBase.New(defautLanguageMessage)
        Me.ResKey = resKey
        Me.MessageArguments = New Object() {}

    End Sub

    Public Sub New(resKey As String, defautLanguageMessage As String, ParamArray args As Object())
        MyBase.New(String.Format(defautLanguageMessage, If(args, New Object() {})))

        Me.ResKey = resKey
        Me.MessageArguments = If(args, New Object() {})
    End Sub

    Public Property ResKey As String
    Public Property MessageArguments As Object()

End Class