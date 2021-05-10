Namespace Entities
    Public Class InfoContattoScolastico
        Public Property NomeIndice As String
        Public Property CognomeIndice As String
        Public Property CodiceMeccanografico As String
        Public Property DescrizioneScuola As String
        Public Property CodiceGruppo As String
        Public Property CodiceEpisodioIndice As Long
        Public Property CodiceEpisodioContatto As Long?
        Public Property CodiceStatoEpisodioIndice As Long
        <DbColumnName("DescStatoEpisodioIndice")>
        Public Property DescrizioneStatoEpisodioIndice As String
        Public Property CodiceStatoEpisodioContatto As Long?
        <DbColumnName("DescStatoEpisodioContatto")>
        Public Property DescrizioneStatoEpisodioContatto As String
        Public Property Classe As String
        Public Property Attivo As Boolean
        Public Property NumeroPersone As Integer
    End Class
End Namespace
