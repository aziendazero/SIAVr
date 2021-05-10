Namespace Entities

    <Serializable()>
    Public Class CalendarioVaccinaleGenerico
        Public Property CodiceCiclo As String
        Public Property DescrizioneCiclo As String
        Public Property SedutaCiclo As Integer
        Public Property CodiceVaccinazione As String
        Public Property DescrizioneVaccinazione As String
        Public Property DoseVaccinazione As Integer
        Public Property EtaSeduta As Integer
    End Class

    <Serializable()>
    Public Class CalendarioVaccinalePaziente
        Public Property CodiceCiclo As String
        Public Property DescrizioneCiclo As String
        Public Property SedutaCiclo As Integer
        Public Property CodiceVaccinazione As String
        Public Property DescrizioneVaccinazione As String
        Public Property DoseVaccinazione As Integer
        Public Property EtaSeduta As Integer
        Public Property CodicePaziente As Long
        Public Property AppIdAziendaLocale As String
        Public Property CognomePaziente As String
        Public Property NomePaziente As String
        Public Property SessoPaziente As String
        Public Property DataNascitaPaziente As DateTime?
    End Class

End Namespace