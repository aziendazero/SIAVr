Namespace Entities
    Public Class TestataEpisodioCovid
        Public Property CodiceEpisodio As Long
        Public Property CodicePaziente As Long
        Public Property NomePaziente As String
        Public Property CognomePaziente As String
        Public Property Telefono As String
        Public Property Email As String
        Public Property CodiceComuneIsolamento As String
        Public Property DescrizioneComuneIsolamento As String
        Public Property IndirizzoIsolamento As String
        Public Property InternoRegione As String
        Public Property CodiceComuneEsposizione As String
        Public Property DescrizioneComuneEsposizione As String
        Public Property CodiceTipoCaso As String
        Public Property DescrizioneTipoCaso As String
        Public Property CodiceFonteSegnalatore As Integer?
        Public Property DescrizioneFonteSegnalatore As String
        Public Property CodiceStato As Integer?
        Public Property DescrizioneStato As String
        Public Property CodiceUlssIncaricata As String
        Public Property DescrizioneUlssIncaricata As String

        ''Definizione
        Public Property CodiceTipoContatto As Integer?
        Public Property DescrizioneTipoContatto As String
        Public Property DataDecessoCovid As Date?
        Public Property DataInizioSorveglianza As Date?
        Public Property DataFineSorveglianza As Date?
        Public Property Note As String


    End Class

    Public Class SalvaTestataEpisodioCovid
        Inherits TestataEpisodioCovid
        Public Property CodiceEpisodio As Long
    End Class
End Namespace
