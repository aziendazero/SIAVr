Imports System.Collections.Generic

Namespace Entities

    <Serializable()> _
    Public Class VaccinazioneEsclusaCentrale

        Public Property Id() As Int64
        Public Property TipoVaccinazioneEsclusaCentrale() As String

        Public Property FlagVisibilitaCentrale() As String
        Public Property DataRevocaVisibilita() As DateTime?

        Public Property CodicePazienteCentrale() As String
        Public Property CodicePaziente() As Int64

        Public Property IdVaccinazioneEsclusa() As Int64
        Public Property CodiceUslVaccinazioneEsclusa() As String

        Public Property DataInserimentoVaccinazioneEsclusa() As DateTime
        Public Property IdUtenteInserimentoVaccinazioneEsclusa() As Int64

        Public Property DataModificaVaccinazioneEsclusa() As DateTime?
        Public Property IdUtenteModificaVaccinazioneEsclusa() As Int64?

        Public Property DataEliminazioneVaccinazioneEsclusa() As DateTime?
        Public Property IdUtenteEliminazioneVaccinazioneEsclusa() As Int64?

        Private _MergeInfoCentrale As MergeInfoCentrale
        Public ReadOnly Property MergeInfoCentrale As MergeInfoCentrale
            Get
                If _MergeInfoCentrale Is Nothing Then
                    _MergeInfoCentrale = New MergeInfoCentrale()
                End If
                Return _MergeInfoCentrale
            End Get
        End Property

        Public Property IdConflitto As Int64?
        Public Property DataRisoluzioneConflitto() As DateTime?
        Public Property IdUtenteRisoluzioneConflitto As Int64?

        Public Property IdUtenteUltimaOperazione As Int64

        <Serializable()> _
        Public Class VaccinazioneEsclusaDistribuita

            Public Property Id() As Int64
            Public Property IdVaccinazioneEsclusaCentrale() As Int64

            Public Property CodicePaziente() As Int64

            Public Property IdVaccinazioneEsclusa() As Int64
            Public Property CodiceUslVaccinazioneEsclusa() As String

            Public Property DataInserimentoVaccinazioneEsclusa() As DateTime
            Public Property IdUtenteInserimentoVaccinazioneEsclusa() As Int64

            Public Property DataAggiornamentoVaccinazioneEsclusa() As DateTime?

        End Class

    End Class

End Namespace