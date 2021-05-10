Namespace Entities

    <Serializable>
    Public Class AttivitaRegistrazioneValore

        Public Property IdAttivitaRegistrazione As Integer

        Public Property IdAttivitaVariabile As Integer
        Public Property CodiceVariabile As String
        Public Property DescrizioneVariabile As String
        Public Property TipoDatiRisposta As String
        Public Property Obbligatorio As Boolean
        Public Property Ordine As Integer
        Public Property TipoRisposta As String

        Public Property IdAttivitaRegistrazioneValore As Long?
        Public Property CodiceRisposta As String
        Public Property ValoreRisposta As String
        Public Property DataRegistrazione As DateTime?
        Public Property IdUtenteRegistrazione As Long?
        Public Property DataVariazione As DateTime?
        Public Property IdUtenteVariazione As Long?

    End Class

End Namespace
