Imports System.Collections.Generic

Public Interface ISitiInoculazioneProvider

	Function GetSitiInoculazione() As List(Of SitoInoculazione)
    Function GetCodiceSitoInoculazioneByCodiceACN(codiceACN As String) As String
End Interface
