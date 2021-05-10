Namespace Entities

    <Serializable()>
    Public Class VaccinazioneEsclusaAPP
        Public Property Id As Long
        Public Property DataVisita As DateTime
        Public Property CodiceVaccinazione As String
        Public Property DescrizioneVaccinazione As String
        Public Property CodiceMotivoEsclusione As String
        Public Property DescrizioneMotivoEsclusione As String
        Public Property DataScadenza As DateTime?
        Public Property CodicePaziente As Long
        Public Property CognomePaziente As String
        Public Property NomePaziente As String
        Public Property DataNascitaPaziente As DateTime
        Public Property AppIdAziendaLocale As String
    End Class

End Namespace