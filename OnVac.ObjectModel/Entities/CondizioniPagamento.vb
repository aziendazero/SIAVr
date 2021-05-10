Namespace Entities

    <Serializable()>
    Public Class CondizioniPagamento
        Public Property IdCondizione As Integer?
        Public Property EtaInizio As Integer?
        Public Property EtaFine As Integer?
        Public Property StatoAbilitazioneImporto As Enumerators.StatoAbilitazioneCampo
        Public Property StatoAbilitazioneEsenzione As Enumerators.StatoAbilitazioneCampo
        Public Property CodiceNomeCommerciale As String
        Public Property ImpostazioneAutomaticaImportoInEsecuzione As Boolean
    End Class

End Namespace
