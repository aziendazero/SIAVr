Namespace Entities

    ''' <summary>
    ''' Entità VaccinazioneEseguita utilizzata per i dati da estrarre per l'FSE.
    ''' E' stata creata ad hoc per l'FSE per scambiare il minimo numero di dati possibile e togliere dalla query quanti più join possibile.
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class VaccinazioneFSE

        Public Property Id As Long
        Public Property CodiceVaccinazione As String
        Public Property DescrizioneVaccinazione As String
        Public Property DoseVaccinazione As Integer?
        Public Property CodiceAssociazione As String
        Public Property DescrizioneAssociazione As String
        Public Property DoseAssociazione As Integer?
        Public Property OrdineVaccinazione As Integer

        Public Property TipoVaccinazione As String
        Public Property CodiceCiclo As String
        Public Property NumeroSeduta As Integer?
        Public Property DataEffettuazione As DateTime?
        Public Property DataOraEffettuazione As DateTime?

        Public Property InCampagna As String
        Public Property TipologiaLuogo As Enumerators.TipoStruttura?
        Public Property CodiceLuogo As String
        Public Property DescrizioneLuogo As String
        Public Property CodiceHL7ViaSomministrazione As String
        Public Property DescrizioneViaSomministrazione As String
        Public Property CodiceHL7SitoInoculazione As String
        Public Property DescrizioneSitoInoculazione As String
        Public Property CodiceLotto As String
        Public Property CodiceNomeCommerciale As String
        Public Property DescrizioneNomeCommerciale As String
        Public Property CodiceAicNomeCommerciale As String
        Public Property CodiceCvxVaccinazione As String

        Public Property CodiceMalattia As String
        Public Property DescrizioneMalattia As String
        Public Property CodiceICD9CMMalattia As String
        Public Property CodiceHL7Esenzione As String

        Public Property IsEseguita As Boolean

        Public Property DataRegistrazioneEsclusione As DateTime?
        Public Property CodiceMotivoEsclusione As String
        Public Property DescrizioneMotivoEsclusione As String

    End Class

End Namespace