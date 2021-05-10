Namespace Entities

    <Serializable>
    Public Class CodiceHSP

        Public Property Id As String

        Public Property CodiceHsp As String

        Public Property Descrizione As String

        Public Property DataInizioValidita As DateTime?

        Public Property DataFineValidita As DateTime?

        Public Property CodiceAsl As String

        Public Property DescrizioneAsl As String

        Public Property Indirizzo As String

        Public Property CodiceComune As String

        Public Property DescrizioneComune As String

        Public Property OspedaleDiComunita As Boolean

    End Class

    Public Class CodiceHSPCommand

        Public Id As String

        Public Codice As String

        Public Descrizione As String

        Public DataInizioValidita As DateTime?

        Public DataFineValidita As DateTime?

        Public CodiceAsl As String

        Public Indirizzo As String

        Public CodiceComune As String

        ' properties per controllo lunghezza campi inseriti
        Public CodiceMaxLength As Integer

        Public DescrizioneMaxLength As Integer

        Public IndirizzoMaxLength As Integer

    End Class

End Namespace
