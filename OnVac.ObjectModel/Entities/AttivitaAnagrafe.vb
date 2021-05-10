Namespace Entities

    <Serializable>
    Public Class AttivitaAnagrafe

        Public Property Id As Integer
        Public Property Codice As String
        Public Property Descrizione As String
        Public Property Obsoleto As String
        Public Property Scuola As String

        Public Enum Ordinamento
            Id = 0
            Codice = 1
            Descrizione = 2
            Obsoleto = 3
            Scuola = 4
        End Enum

    End Class

End Namespace