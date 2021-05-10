Namespace Entities
    Public Class Tag
        Public Property Id As Long
        Public Property Descrizione As String
        Public Property Gruppo As String
        Public Property Colore As String
    End Class

    Public Class TagTmp
        Public Property Id As String
        Public Property Descrizione As String
        Public Property Gruppo As String
        Public Property Colore As String
    End Class

    <Serializable()>
    Public Class ExportTag
        Public Property IdEpisodio As Long
        Public Property Descrizione As String
    End Class
End Namespace
