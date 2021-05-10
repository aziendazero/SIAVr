Imports System.Collections.Generic

Namespace Entities

    <Serializable>
    Public Class FieldToCheck(Of T)

        Public Value As T
        Public Description As String
        Public MaxLength As Integer
        Public Required As Boolean

    End Class

End Namespace