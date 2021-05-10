Imports System.Collections.Generic

Namespace Entities

    Public Class ConflittoVisite

        Public Property IdVisitaCentrale As Int64

        Public Property CodicePazienteCentrale As String
        Public Property Cognome As String
        Public Property Nome As String
        Public Property DataNascita As DateTime?

        Private _visiteInConflitto As List(Of DatiVisitaInConflitto)
        Public Property VisiteInConflitto() As List(Of DatiVisitaInConflitto)
            Get
                Return _visiteInConflitto
            End Get
            Set(value As List(Of DatiVisitaInConflitto))
                _visiteInConflitto = value
            End Set
        End Property

        Public Class DatiVisitaInConflitto

            ' Dati centrali
            Public Property Id() As Int64

            Public Property FlagVisibilitaCentrale() As String

            Public Property CodicePazienteCentrale() As String
            Public Property CodicePaziente() As Int64

            Public Property TipoVisitaCentrale() As String

            Public Property IdVisita() As Int64
            Public Property CodiceUslVisita() As String

            ' Dati locali
            Public Property DataVisita As DateTime

            Public Property CodiceMalattia As String
            Public Property DescrizioneMalattia As String

            Public Property NumeroBilancio As Int32?
            Public Property DescrizioneBilancio As String

            Public Property DataFineSospensione As DateTime?

        End Class

    End Class

End Namespace