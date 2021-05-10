Imports System.Collections.Generic

Namespace Entities

    Public Class ConflittoVaccinazioniEscluse

        Public Property IdVaccinazioneEsclusaCentrale() As Int64

        Public Property CodicePazienteCentrale As String
        Public Property Cognome As String
        Public Property Nome As String
        Public Property DataNascita As DateTime?

        Private _vaccinazioniEscluseInConflitto As List(Of DatiEsclusioneInConflitto)
        Public Property VaccinazioniEscluseInConflitto() As List(Of DatiEsclusioneInConflitto)
            Get
                If _vaccinazioniEscluseInConflitto Is Nothing Then _vaccinazioniEscluseInConflitto = New List(Of DatiEsclusioneInConflitto)()
                Return _vaccinazioniEscluseInConflitto
            End Get
            Set(value As List(Of DatiEsclusioneInConflitto))
                _vaccinazioniEscluseInConflitto = value
            End Set
        End Property

        Public Class DatiEsclusioneInConflitto

            ' Dati centrali
            Public Property Id() As Int64

            Public Property FlagVisibilitaCentrale() As String

            Public Property CodicePazienteCentrale() As String
            Public Property CodicePaziente() As Int64

            Public Property TipoVaccinazioneEsclusaCentrale() As String

            Public Property IdVaccinazioneEsclusa() As Int64
            Public Property CodiceUslVaccinazioneEsclusa() As String

            ' Dati locali
            Public Property CodiceVaccinazione As String

            Public Property CodiceMotivoEsclusione As String
            Public Property DescrizioneMotivoEsclusione As String

            Public Property DataEsclusione As DateTime

            Public Property DataScadenza As DateTime?

        End Class

    End Class

End Namespace