Public Class BizLogOptions

    Public ReadOnly CodiceArgomento As String
    Public ReadOnly Automatico As Boolean

    Public Sub New(codiceArgomento As String, automatico As Boolean)

        Me.CodiceArgomento = codiceArgomento
        Me.Automatico = automatico

    End Sub

    Public Sub New(codiceArgomento As String)

        Me.new(codiceArgomento, False)

    End Sub

End Class