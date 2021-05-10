Imports System.Collections.Generic

Namespace Entities

    <Serializable()> _
    Public Class VisitaCentrale

        Public Property Id() As Int64
        Public Property TipoVisitaCentrale() As String

        Public Property FlagVisibilitaCentrale() As String
        Public Property DataRevocaVisibilita() As DateTime?

        Public Property CodicePazienteCentrale() As String
        Public Property CodicePaziente() As Int64

        Public Property IdVisita() As Int64
        Public Property CodiceUslVisita() As String

        Public Property DataInserimentoVisita() As DateTime
        Public Property IdUtenteInserimentoVisita() As Int64

        Public Property DataModificaVisita() As DateTime?
        Public Property IdUtenteModificaVisita() As Int64?

        Public Property DataEliminazioneVisita() As DateTime?
        Public Property IdUtenteEliminazioneVisita() As Int64?

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
        Public Property DataRisoluzioneConflitto As DateTime?
        Public Property IdUtenteRisoluzioneConflitto As Int64?

        Public Property IdUtenteUltimaOperazione As Int64

        <Serializable()> _
        Public Class VisitaDistribuita

            Public Property Id() As Int64
            Public Property IdVisitaCentrale() As Int64

            Public Property CodicePaziente() As Int64

            Public Property IdVisita() As Int64
            Public Property CodiceUslVisita() As String

            Public Property DataInserimentoVisita() As DateTime
            Public Property IdUtenteInserimentoVisita() As Int64

            Public Property DataAggiornamentoVisita() As DateTime?

        End Class

    End Class

End Namespace
