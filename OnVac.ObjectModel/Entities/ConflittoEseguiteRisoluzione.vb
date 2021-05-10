Namespace Entities

    <Serializable>
    Public Class ConflittoEseguiteRisoluzione

        Public Property IdProcessoBatch() As Int64
        Public Property IdVaccinazioneEseguitaCentrale() As Int64
        Public Property IdConflitto() As Int64

        Public Property CodicePazienteCentrale As String
        Public Property CodicePazienteLocale As Long

        Public Property IdVaccinazioneEseguitaLocale As Long
        Public Property CodiceUslInserimento As String

        Public Property StatoRisoluzioneConflitto As String
        Public Property Message As String

        ''' <summary>
        ''' Id (VCC_ID) della vaccinazione eseguita in conflitto che è stata scelta come principale dalla procedura di auto-risoluzione.
        ''' Se la procedura non è riuscita a determinarla, il campo rimane nullo.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IdConflittoRisoluzione As Int64?

    End Class

End Namespace