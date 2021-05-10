Namespace Entities

    <Serializable>
    Public Class AttivitaVariabileAssociata

        Public Property IdVariabile As Integer
        Public Property IdAttivita As Integer
        Public Property Obbligatorio As Boolean
        Public Property CodiceVariabile As String
        Public Property DescrizioneVariabile As String
        Public Property Ordine As Integer
        Public Property TipoRisposta As String

        Public Enum Ordinamento
            Codice = 0
            Descrizione = 1
            Ordine = 2
        End Enum

    End Class

End Namespace