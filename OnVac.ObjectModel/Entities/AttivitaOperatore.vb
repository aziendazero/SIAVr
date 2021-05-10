Namespace Entities

    <Serializable>
    Public Class AttivitaOperatorePPA

        Public Property IdOperatore As Integer?
        Public Property Cognome As String
        Public Property Nome As String
        Public Property CodiceFiscale As String
        Public Property IdUnitaOperativa As Integer?
        Public Property CodiceUnitaOperativa As String
        Public Property DescrizioneUnitaOperativa As String
        Public Property IdQualifica As Integer?
        Public Property CodiceQualifica As String
        Public Property DescrizioneQualifica As String
        Public Property Obsoleto As Boolean

        Public Enum Ordinamento
            Cognome = 0
            Nome = 1
        End Enum

    End Class

End Namespace
