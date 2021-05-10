Imports System.Collections.Generic

Namespace Entities

    <Serializable()>
    Public Class VaccinazioneInfoAPP

        Public Property CodiceVaccinazione As String
        Public Property DescrizioneVaccinazione As String
        Public Property Titolo As String
        Public Property Descrizione As String
        Public Property AssociazioniAPP As List(Of Entities.AssociazioneAPP)

    End Class

End Namespace
