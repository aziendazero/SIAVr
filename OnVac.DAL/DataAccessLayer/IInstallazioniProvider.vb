Public Interface IInstallazioniProvider

    Function GetInstallazione(codiceInstallazione As String) As Entities.Installazione
    Function GetDescrizioneUsl(codiceUsl As String) As String

End Interface
