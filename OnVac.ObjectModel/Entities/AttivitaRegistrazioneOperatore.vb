Namespace Entities

    <Serializable>
    Public Class AttivitaRegistrazioneOperatore

        Public Property IdAttivitaRegistrazione As Integer
        Public Property IdOperatore As Integer
        Public Property Cognome As String
        Public Property Nome As String
        Public Property CodiceFiscale As String
        Public Property IdUnitaOperativa As Integer?
        Public Property CodiceUnitaOperativa As String
        Public Property DescrizioneUnitaOperativa As String
        Public Property IdQualifica As Integer?
        Public Property CodiceQualifica As String
        Public Property DescrizioneQualifica As String

    End Class

End Namespace
