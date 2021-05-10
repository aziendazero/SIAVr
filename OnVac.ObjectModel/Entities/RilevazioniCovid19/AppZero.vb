Public Class AppZero

End Class

Public Class AppZeroDatiEpisodio
    Public Property CodiceFiscale As String
    Public Property IdEpisodio As Long
End Class

Public Class SintomiApp
    Public Property NomeSintomi As String
    Public Property NumeroSintomoSiavr As Integer
End Class

Public Class DiariaAppZero
    Public Property IdDiaria As Long
    Public Property IdEpisodio As Long
    Public Property UslCodiceRilevazione As String
    Public Property DataRilevazione As DateTime
    Public Property OpeCodiceRilevazione As String
    Public Property FlagAsintomatico As String
    Public Property Note As String
    Public Property UteIdInserimento As Long
    Public Property UslCodiceInserimento As String
    Public Property DataInserimento As String
    Public Property RispostaTelefono As String
    Public Property UteIdModifica As Long
    Public Property DataModifica As DateTime
End Class
