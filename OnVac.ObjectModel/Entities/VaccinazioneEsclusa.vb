Namespace Entities

    <Serializable()>
    Public Class VaccinazioneEsclusa

        Public Property CodicePaziente As Integer
        Public Property DataVisita As DateTime
        Public Property CodiceVaccinazione As String
        Public Property CodiceMotivoEsclusione As String
        Public Property DescrizioneMotivoEsclusione As String
        Public Property CodiceOperatore As String
        Public Property DescrizioneOperatore As String
        Public Property DataScadenza As DateTime
        Public Property CodicePazientePrecedente As Integer?
        Public Property CodiceUslInserimento As String
        Public Property Id As Integer

        Public Property FlagVisibilita As String
        Public Property NoteAcquisizioneDatiVaccinaliCentrale As String

        Public Property DataRegistrazione As DateTime
        Public Property IdUtenteRegistrazione As Int64
        Public Property DataModifica As DateTime?
        Public Property IdUtenteModifica As Int64?
        Public Property DataEliminazione As DateTime?
        Public Property IdUtenteEliminazione As Int64?

        Public Property NumeroDose As Integer?
        Public Property Note As String

    End Class

End Namespace



