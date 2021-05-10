Imports System.Collections.Generic

Namespace Entities
    Public Class InfoFocolaio
        Public Sub New()
            Me.Episodi = New List(Of Long)
        End Sub

        Public Property CodiceFocolaio As Integer
        Public Property Descrizione As String
        Public Property Scolastico As Boolean
        Public Property Plesso As String
        Public Property Istituto As String
        Public Property GiorniChiusura As Integer?

        Public Property Episodi As IEnumerable(Of Long)
    End Class
End Namespace
