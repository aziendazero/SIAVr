Namespace Entities

    ''' <summary>
    ''' Entità VaccinazioneEseguita utilizzata per i dati da estrarre per la APP Vaccinale.
    ''' E' stata creata ad hoc per la APP per scambiare il minimo numero di dati possibile e togliere dalla query quanti più join possibile.
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class VaccinazioneEseguitaAPP

        Public Property Id As Long
        Public Property AppIdAziendaLocale As String

        Public Property CodicePaziente As Long
        Public Property CognomePaziente As String
        Public Property NomePaziente As String
        Public Property DataNascitaPaziente As DateTime

        Public Property DataEffettuazione As DateTime
        Public Property DataOraEffettuazione As DateTime

        Public Property CodiceVaccinazione As String
        Public Property DescrizioneVaccinazione As String
        Public Property DoseVaccinazione As Integer

        Public Property CodiceAssociazione As String
        Public Property DescrizioneAssociazione As String
        Public Property DoseAssociazione As Integer

        Public Property CodiceViaSomministrazione As String
        Public Property DescrizioneViaSomministrazione As String
        Public Property CodiceSitoInoculazione As String
        Public Property DescrizioneSitoInoculazione As String

        Public Property CodiceLotto As String
        Public Property CodiceNomeCommerciale As String
        Public Property DescrizioneNomeCommerciale As String

        Public Property CodiceMedicoResponsabile As String
        Public Property DescrizioneMedicoResponsabile As String
        Public Property CodiceVaccinatore As String
        Public Property DescrizioneVaccinatore As String

        Public Property CodiceCentroVaccinale As String
        Public Property DescrizioneCentroVaccinale As String
        Public Property IndirizzoCentroVaccinale As String
        Public Property ComuneCentroVaccinale As String
        Public Property TelefonoCentroVaccinale As String
        Public Property CodiceAmbulatorio As Integer
        Public Property DescrizioneAmbulatorio As String

        Public Property Stato As String
        Public Property CodiceCiclo As String
        Public Property NumeroSeduta As Integer?
        Public Property Obbligatorieta As String
        Public Property IsScaduta As Boolean
        Public Property IsFittizia As Boolean
        Public Property OrdineVaccinazione As Integer

        Public Property CodiceReazione As String
        Public Property DescrizioneReazione As String
        Public Property DataReazione As DateTime?
        Public Property GravitaReazione As String
        Public Property CodiceReazione1 As String
        Public Property DescrizioneReazione1 As String
        Public Property CodiceReazione2 As String
        Public Property DescrizioneReazione2 As String
        Public Property IdACN As String
        Public Property Provenienza As String

    End Class

End Namespace