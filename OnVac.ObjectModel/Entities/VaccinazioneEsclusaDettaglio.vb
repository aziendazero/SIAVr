Namespace Entities

    <Serializable()>
    Public Class VaccinazioneEsclusaDettaglio

        Public Property Id As Integer?
        Public Property CodicePaziente As Integer?
        Public Property DataVisita As Date?
        Public Property CodiceVaccinazione As String
        Public Property DescrizioneVaccinazione As String
        Public Property CodiceMotivoEsclusione As String
        Public Property CodiceOperatore As String
        Public Property DescrizioneOperatore As String
        Public Property DataScadenza As Date?
        Public Property DataRegistrazione As Date?
        Public Property IdUtenteRegistrazione As Integer?
        Public Property UtenteRegistrazione As String
        Public Property CodiceUslInserimento As String
        Public Property CodicePazientePrecedente As Integer?
        Public Property DataModifica As Date?
        Public Property IdUtenteModifica As Integer?
        Public Property UtenteModifica As String
        Public Property DataEliminazione As Date
        Public Property IdUtenteEliminazione As Integer?
        Public Property UtenteEliminazione As String
        Public Property FlagVisibilita As String
        Public Property StatoEliminazione As String 'Constants.StatoVaccinazioniEscluseEliminate

    End Class

End Namespace