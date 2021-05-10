Public Class TestRapido
    Public Property IdTest As Long
    Public Property Cognome As String
    Public Property Nome As String
    Public Property CodiceFiscale As String
    Public Property DataDiNascita As DateTime
    Public Property DataPrelievo As Date
    Public Property LuogoDiNascita As String
    Public Property Sesso As String
    Public Property Residenza As String
    Public Property Centro As String
    Public Property UlssRes As String
    Public Property UlssRichiedente As String
    Public Property DataReferto As Date
    Public Property MotivoEsecuzione As String
    Public Property Cellulare As String
    Public Property CellulareConsenso As String
    Public Property PrimaRilevazione As Date?
End Class

Public Class TestRapidoScaricoScreening
    Inherits TestRapido

    Public Property EsitoIGM As String
    Public Property EsitoIGG As String
    Public Property Marca As String
    Public Property Valido As Boolean

    Public Property CodiceSettingsSPS As Long?
    Public Property DescrizioneSettings As String
    Public Property CodiceTipoScolasticoTPS As Long?
    Public Property DescrizioneScolastico As String

End Class

Public Class DettaglioTestRapido
    Public Property IdTest As Long
    Public Property UlssRichiedente As String
    Public Property DataPrelievo As DateTime
    Public Property DataReferto As DateTime
    Public Property TestValido As String
    Public Property IGG As String
    Public Property IGM As String
    Public Property Marca As String
    Public Property Foto As Byte()

    Public Property IdUtenteRilevazione As Integer?
    Public Property Cellulare As String
    Public Property CellulareConsenso As String
    Public Property PrimaRilevazione As Date?

End Class

Public Class DettaglioAntigeneTestRapido
    Inherits DettaglioTestRapido
    Public Property EsitoTar As Integer?
End Class
