Namespace Entities

    <Serializable>
    Public Class AttivitaVariabileDaAssociare

        Public Property Codice As String
        Public Property Descrizione As String
        Public Property TipoRisposta As String
        Public Property Sesso As String

        Public Enum Ordinamento
            Codice = 0
            Descrizione = 1
        End Enum

    End Class

End Namespace
