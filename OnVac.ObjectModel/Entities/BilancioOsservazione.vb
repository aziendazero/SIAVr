Namespace Entities

    <Serializable>
    Public Class BilancioOsservazione

        Public Property NumeroBilancio As Integer
        Public Property CodiceMalattia As String
        Public Property CodiceOsservazione As String
        Public Property NumeroOsservazione As Integer?
        Public Property CodiceSezione As String
        Public Property IsObbligatoria As Boolean

    End Class

    <Serializable>
    Public Class BilancioOsservazioneAssociata
        Inherits BilancioOsservazione

        Public Property DescrizioneOsservazione As String
        Public Property TipoRisposta As String

    End Class

    <Serializable>
    Public Class BilancioOsservazioneAnagrafica

        Public Property CodiceOsservazione As String
        Public Property DescrizioneOsservazione As String
        Public Property TipoRisposta As String

    End Class

End Namespace