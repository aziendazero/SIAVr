Namespace Entities

    <Serializable()>
    Public Class StoricoAppuntamento
        Inherits ConvocazioneAppuntamento

        Public Property DescrizioneAmbulatorio As String

        Public Property CodiceUtenteInvio As String
        Public Property DescrizioneUtenteInvio As String

        Public Property CodiceUtenteRegistrazioneAppuntamento As String
        Public Property DescrizioneUtenteRegistrazioneAppuntamento As String

        Public Property CodiceUtenteEliminazioneAppuntamento As String
        Public Property DescrizioneUtenteEliminazioneAppuntamento As String

        Public Property DescrizioneMotivoEliminazioneAppuntamento As String

    End Class

End Namespace
