Namespace Entities

    <Serializable>
    Public Class AttivitaTipo

        Public Property Codice As String
        Public Property Descrizione As String
        Public Property Obsoleto As String

        Public Enum Ordinamento
            Codice = 0
            Descrizione = 1
            Obsoleto = 2
        End Enum

    End Class

End Namespace