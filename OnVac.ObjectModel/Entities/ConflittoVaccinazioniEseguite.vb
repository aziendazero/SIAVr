Imports System.Collections.Generic

Namespace Entities

    Public Class ConflittoVaccinazioniEseguite

        Public Property IdVaccinazioneEseguitaCentrale() As Int64

        Public Property CodicePazienteCentrale As String
        Public Property Cognome As String
        Public Property Nome As String
        Public Property DataNascita As DateTime?

        Private _vaccinazioniEseguiteInConflitto As List(Of DatiVaccinazioneInConflitto)

        ''' <summary>
        ''' Lista vaccinazioni in conflitto (compresa quella "master").
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property VaccinazioniEseguiteInConflitto() As List(Of DatiVaccinazioneInConflitto)
            Get
                If _vaccinazioniEseguiteInConflitto Is Nothing Then _vaccinazioniEseguiteInConflitto = New List(Of DatiVaccinazioneInConflitto)()
                Return _vaccinazioniEseguiteInConflitto
            End Get
            Set(value As List(Of DatiVaccinazioneInConflitto))
                _vaccinazioniEseguiteInConflitto = value
            End Set
        End Property

        Public Class DatiVaccinazioneInConflitto

            ' Dati letti dal db centrale

            ''' <summary>
            ''' Id della vaccinazione in centrale (campo VCC_ID)
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Id() As Int64

            Public Property FlagVisibilitaCentrale() As String

            Public Property CodicePazienteCentrale() As String
            Public Property CodicePaziente() As Int64

            Public Property TipoVaccinazioneEseguitaCentrale() As String

            ''' <summary>
            ''' Id della vaccinazione in locale (campo VCC_VES_ID)
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property IdVaccinazioneEseguita() As Int64
            Public Property CodiceUslVaccinazioneEseguita() As String

            Public Property IdReazioneAvversa() As Int64?
            Public Property TipoReazioneAvversa() As String

            Public Property DataInserimentoCentrale As DateTime

            ' Dati letti in locale (dalla ULSS proprietaria del dato)
            Public Property CodiceVaccinazione As String
            Public Property DoseVaccinazione As Int32

            Public Property CodiceAssociazione As String
            Public Property DoseAssociazione As Int32

            Public Property CodiceNomeCommerciale As String
            Public Property CodiceLotto As String

            Public Property DataEffettuazione As DateTime

            Public Property DataRegistrazione As DateTime?

            ''' <summary>
            ''' Campo VES_STATO
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Stato As String

        End Class

    End Class

End Namespace