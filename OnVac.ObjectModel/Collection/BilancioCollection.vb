Imports System.Collections.ObjectModel

Namespace Collection
    <Serializable()> _
    Public Class BilancioProgrammatoCollection
        Inherits Collection(Of BilancioProgrammato)

        Public Function toHTMLString() As String
            Dim sbl As New System.Text.StringBuilder
            Dim i As Integer

            For i = 0 To Me.Count - 1
                sbl.Append(Me.Item(i).toString())
                sbl.Append("<br/>")
            Next

            Return sbl.ToString
        End Function

    End Class
End Namespace
