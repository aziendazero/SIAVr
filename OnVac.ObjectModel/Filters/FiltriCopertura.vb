Imports System.Collections
Imports System.Collections.Generic

Namespace Filters

    Public Class FiltriCopertura

        Public giorniVita As Integer
        Public richiamo As Integer
        Public dataNascitaDa As DateTime
        Public dataNascitaA As DateTime

        Public dataEffettuazioneDa As DateTime?
        Public dataEffettuazioneA As DateTime?

        Public codiceComuneResidenza As String
        Public codiceCircoscrizione As String

        Public codiceConsultorio As List(Of String)
        Public tipoVaccinazioni As List(Of String)
        Public Sesso As List(Of String)
        Public StatoAnagrafico As List(Of String)
        Public codiceVaccinazione As List(Of String)

    End Class

    Public Class FiltriCoperturaMedico

        Public Class FiltroVaccinazione

            Public codice As String
            Public descrizione As String
            Public giorniVita As Integer
            Public richiamo As Integer

        End Class

        Public Vaccinazioni As List(Of FiltroVaccinazione)

        Public dataNascitaDa As DateTime
        Public dataNascitaA As DateTime

        Public dataEffettuazioneDa As DateTime?
        Public dataEffettuazioneA As DateTime?

        Public codiceComuneResidenza As String
        Public codiceCircoscrizione As String

        Public codiceConsultorio As List(Of String)
        Public tipoVaccinazioni As List(Of String)
        Public Sesso As List(Of String)
        Public StatoAnagrafico As List(Of String)

        Public codiceMedico As String
        Public tipoMedico As List(Of String)

        Public Sub New()

            Me.Vaccinazioni = New List(Of FiltriCoperturaMedico.FiltroVaccinazione)

        End Sub

    End Class

End Namespace
