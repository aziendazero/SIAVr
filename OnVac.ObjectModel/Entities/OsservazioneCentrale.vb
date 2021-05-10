Imports System.Collections.Generic

Namespace Entities

    <Serializable()> _
    Public Class OsservazioneCentrale

        Public Property Id() As Int64
        Public Property IdVisitaCentrale() As Int64

        Public Property TipoOsservazioneCentrale() As String

        Public Property FlagVisibilitaCentrale() As String
        Public Property DataRevocaVisibilita() As DateTime?

        Public Property CodicePazienteCentrale() As String
        Public Property CodicePaziente() As Int64

        Public Property IdOsservazione() As Int64
        Public Property CodiceUslOsservazione() As String

        Public Property DataInserimentoOsservazione() As DateTime
        Public Property IdUtenteInserimentoOsservazione() As Int64

        Public Property DataModificaOsservazione() As DateTime?
        Public Property IdUtenteModificaOsservazione() As Int64?

        Public Property DataEliminazioneOsservazione() As DateTime?
        Public Property IdUtenteEliminazioneOsservazione() As Int64?

        Private _MergeInfoCentrale As MergeInfoCentrale
        Public ReadOnly Property MergeInfoCentrale As MergeInfoCentrale
            Get
                If _MergeInfoCentrale Is Nothing Then
                    _MergeInfoCentrale = New MergeInfoCentrale()
                End If
                Return _MergeInfoCentrale
            End Get
        End Property

        <Serializable()> _
        Public Class OsservazioneDistribuita

            Public Property Id() As Int64
            Public Property IdOsservazioneCentrale() As Int64

            Public Property CodicePaziente() As Int64

            Public Property IdOsservazione() As Int64
            Public Property CodiceUslOsservazione() As String

            Public Property DataInserimentoOsservazione() As DateTime
            Public Property IdUtenteInserimentoOsservazione() As Int64

            Public Property DataAggiornamentoOsservazione() As DateTime?

        End Class

    End Class


End Namespace