Namespace Entities

    <Serializable()>
    Public Class Osservazione

        Public Property Id As Int64
        Public Property IdVisita As Integer
        Public Property CodicePaziente As Integer
        Public Property CodicePazienteOld As Integer?

        Public Property NumeroBilancio As Integer
        Public Property CodiceMalattia As String
        Public Property DataVisita As DateTime?

        Public Property SezioneCodice As String
        Public Property SezioneDescrizione As String
        Public Property SezioneNumero As String

        Public Property OsservazioneCodice As String
        Public Property OsservazioneDescrizione As String
        Public Property OsservazioneNumero As Integer
        Public Property OsservazioneDisabilitata As Boolean

        Public Property RispostaCodice As String
        Public Property RispostaDescrizione As String
        Public Property RispostaTesto As String

        Public Property DataVariazione As DateTime?
        Public Property IdUtenteVariazione As Int64?

        Public Property DataRegistrazione As DateTime
        Public Property IdUtenteRegistrazione As Int64

        Public Property NoteAcquisizioneDatiVaccinaliCentrale() As String

        Public Property DataEliminazione() As DateTime?
        Public Property IdUtenteEliminazione() As Int64?

    End Class

End Namespace
