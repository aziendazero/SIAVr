Imports System.Collections.Generic

Namespace Entities

    <Serializable()> _
    Public Class VaccinazioneEseguitaCentrale

        Public Property Id() As Int64

        Public Property FlagVisibilitaCentrale() As String
        Public Property DataRevocaVisibilita() As DateTime?

        Public Property CodicePazienteCentrale() As String
        Public Property CodicePaziente() As Int64

        Public Property TipoVaccinazioneEseguitaCentrale() As String

        Public Property IdVaccinazioneEseguita() As Int64
        Public Property CodiceUslVaccinazioneEseguita() As String

        Public Property DataInserimentoVaccinazioneEseguita() As DateTime
        Public Property IdUtenteInserimentoVaccinazioneEseguita() As Int64

        Public Property DataModificaVaccinazioneEseguita() As DateTime?
        Public Property IdUtenteModificaVaccinazioneEseguita() As Int64?

        Public Property DataEliminazioneVaccinazioneEseguita() As DateTime?
        Public Property IdUtenteEliminazioneVaccinazioneEseguita() As Int64?

        Public Property DataScadenzaVaccinazioneEseguita() As DateTime?
        Public Property IdUtenteScadenzaVaccinazioneEseguita() As Int64?
        Public Property CodiceUslScadenzaVaccinazioneEseguita() As String

        Public Property DataRipristinoVaccinazioneEseguita() As DateTime?
        Public Property IdUtenteRipristinoVaccinazioneEseguita() As Int64?

        Public Property CodiceUslUltimaOperazioneVaccinazioneEseguita As String
        Public Property IdUtenteUltimaOperazioneVaccinazioneEseguita As Int64

        Public Property CodiceUslUltimaOperazioneReazioneAvversa As String
        Public Property IdUtenteUltimaOperazioneReazioneAvversa As Int64?

        Private _MergeInfoCentrale As MergeInfoCentrale
        Public ReadOnly Property MergeInfoCentrale As MergeInfoCentrale
            Get
                If _MergeInfoCentrale Is Nothing Then
                    _MergeInfoCentrale = New MergeInfoCentrale()
                End If
                Return _MergeInfoCentrale
            End Get
        End Property



#Region " Reazione Avversa "

        Public Property IdReazioneAvversa() As Int64?
        Public Property TipoReazioneAvversaCentrale() As String
        Public Property CodiceUslReazioneAvversa() As String

        Public Property DataInserimentoReazioneAvversa() As DateTime?
        Public Property IdUtenteInserimentoReazioneAvversa() As Int64?

        Public Property DataModificaReazioneAvversa() As DateTime?
        Public Property IdUtenteModificaReazioneAvversa() As Int64?

        Public Property DataEliminazioneReazioneAvversa() As DateTime?
        Public Property IdUtenteEliminazioneReazioneAvversa() As Int64?

#End Region

        Public Property IdConflitto As Int64?
        Public Property DataRisoluzioneConflitto() As DateTime?
        Public Property IdUtenteRisoluzioneConflitto As Int64?

        <Serializable()> _
        Public Class VaccinazioneEseguitaDistribuita

            Public Property Id() As Int64
            Public Property IdVaccinazioneEseguitaCentrale() As Int64

            Public Property CodicePaziente() As Int64

            Public Property IdVaccinazioneEseguita() As Int64
            Public Property IdReazioneAvversa() As Int64?
            Public Property CodiceUslVaccinazioneEseguita() As String

            Public Property IdReazioneAvversaVaccinazioneEseguitaCentrale() As Int64?
            Public Property CodiceUslReazioneAvversaVaccinazioneEseguitaCentrale() As String  

            Public Property DataInserimentoVaccinazioneEseguita() As DateTime
            Public Property IdUtenteInserimentoVaccinazioneEseguita() As Int64

            Public Property DataAggiornamentoVaccinazioneEseguita() As DateTime?

        End Class

    End Class


End Namespace