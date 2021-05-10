Namespace Entities

    <Serializable>
    Public Class AttivitaRegistrazione

        Public Property Id As Integer

        Public Property CodiceAttivitaTipo As String
        Public Property DescrizioneAttivitaTipo As String

        Public Property IdAttivitaAnagrafe As Integer
        Public Property CodiceAttivitaAnagrafe As String
        Public Property DescrizioneAttivitaAnagrafe As String

        ''' <summary>
        ''' Data in cui l'attività è stata eseguita
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DataAttivita As DateTime

        ''' <summary>
        ''' Data in cui l'attività è stata registrata
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DataRegistrazione As DateTime

        Public Property IdUtenteRegistrazione As Long
        Public Property CodiceUtenteRegistrazione As String

        Public Property DataVariazione As DateTime
        Public Property IdUtenteVariazione As Long?

        Public Property DataEliminazione As DateTime
        Public Property IdUtenteEliminazione As Long?

        Public Property CodiceScuola As String
        Public Property DescrizioneScuola As String

        Public Enum Ordinamento
            Id = 0
            CodiceAttivitaTipo = 1
            DescrizioneAttivitaTipo = 2
            CodiceAttivitaAnagrafe = 3
            DescrizioneAttivitaAnagrafe = 4
            DataAttivita = 5
            DataRegistrazione = 6
            CodiceUtenteRegistrazione = 7
            CodiceScuola = 8
            DescrizioneScuola = 9
        End Enum

    End Class

End Namespace