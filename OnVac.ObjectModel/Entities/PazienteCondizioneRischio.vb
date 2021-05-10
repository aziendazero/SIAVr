Namespace Entities

    <Serializable()>
    Public Class PazienteCondizioneRischio

        Public Property CodiceRischio As String
        Public Property DescrizioneRischio As String
        Public Property CodiceVaccinazione As String
        Public Property CodicePaziente As Long

        ''' <summary>
        ''' Indica se la condizione di rischio è associata al paziente
        ''' </summary>
        ''' <returns></returns>
        Public Property FlagRischioPaziente As String

        Public Property FlagCondizioneRischioDefault As String

    End Class

End Namespace