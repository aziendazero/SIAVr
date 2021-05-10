Namespace Entities
    Public Class StatoEppisodio
        <DbColumnName("SEP_CODICE")>
        Public Property Codice As Integer
        <DbColumnName("SEP_DESCRIZIONE")>
        Public Property Descrizione As String
        <DbColumnName("SEP_ORDINE")>
        Public Property Ordine As Integer
        <DbColumnName("SEP_DECEDUTO")>
        Public Property StatoDecesso As Boolean
    End Class
End Namespace
