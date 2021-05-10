Imports System.Collections.Generic

Namespace Entities

    <Serializable>
    Public Class ConvocazioneDaCancellare

        Public Property IdPaziente As Long
        Public Property Data As DateTime

        Public Property PazienteCognome As String
        Public Property PazienteNome As String
        Public Property PazienteDataNascita As DateTime
        Public Property Vaccinazioni As String
        Public Property DataAppuntamento As DateTime?
        
        Public Sub New()
            Vaccinazioni = String.Empty
        End Sub

    End Class

    <Serializable>
    Public Class ConvocazionePK

        Public Property IdPaziente As Long
        Public Property Data As DateTime

    End Class

    <Serializable>
    Public Class FiltriConvocazioneDaCancellare

        Public CodiceCentroVaccinale As String
        Public DescrizioneCentroVaccinale As String

        Public DataNascitaDa As DateTime?
        Public DataNascitaA As DateTime?

        Public Sesso As String

        Public CodiceMalattia As String
        Public DescrizioneMalattia As String

        Public CodiceCategoriaRischio As String
        Public DescrizioneCategoriaRischio As String

        Public CodiciStatiAnagrafici As List(Of String)
        Public DescrizioniStatiAnagrafici As String

        Public DataConvocazioneDa As DateTime?
        Public DataConvocazioneA As DateTime?

        Public CicliSedute As FiltroComposto
        Public AssociazioniDosi As FiltroComposto
        Public VaccinazioniDosi As FiltroComposto

        Public Sub New()
            Me.CodiciStatiAnagrafici = New List(Of String)
        End Sub

    End Class

    <Serializable>
    Public Class FiltroComposto
        Public CodiceValore As List(Of KeyValuePair(Of String, String)) 'Il valore è una stringa che può contenere valori concatenati con la virgola
        Public Valori As List(Of Integer)
    End Class

    <Serializable>
    Public Enum TipoFiltriProgrammazione

        CicloSeduta = 0
        AssociazioneDose = 1
        VaccinazioneDose = 2

    End Enum

    <Serializable>
    Public Class FiltroProgrammazione

        Public Codice As String
        Public Valore As Integer?

    End Class

End Namespace
