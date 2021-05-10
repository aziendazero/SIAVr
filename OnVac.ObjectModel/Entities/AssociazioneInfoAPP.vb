Imports System.Collections.Generic

Namespace Entities

    <Serializable()>
    Public Class AssociazioneInfoAPP

        Public Property CodiceAssociazione As String
        Public Property DescrizioneAssociazione As String
        Public Property Titolo As String
        Public Property Descrizione As String
        Public Property VaccinazioniAPP As List(Of VaccinazioneAPP)

    End Class

End Namespace
