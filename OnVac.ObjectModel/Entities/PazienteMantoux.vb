Namespace Entities

    <Serializable>
    Public Class PazienteMantoux

        Public Property CodicePaziente As Long
        Public Property Cognome As String
        Public Property Nome As String
        Public Property DataNascita As DateTime
        Public Property DescrizioneMantoux As String
        Public Property DataEsecuzione As DateTime
        Public Property FlagEsecuzione As Boolean?
        Public Property FlagPositiva As Boolean?
        Public Property DataInvio As DateTime?
        Public Property Millimetri As String
        Public Property CodiceVaccinatore As String
        Public Property DescrizioneVaccinatore As String

    End Class

End Namespace
