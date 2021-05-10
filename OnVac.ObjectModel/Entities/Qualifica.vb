Namespace Entities

    <Serializable>
    Public Class Qualifica

        Public Property Id As Integer
        Public Property Codice As String
        Public Property Descrizione As String
        Public Property Obsoleto As Boolean

        Public Enum Ordinamento
            Codice = 0
            Descrizione = 1
        End Enum

    End Class

End Namespace
