Imports System.Collections.Generic

Public Interface IVieSomministrazioneProvider

	Function GetVieSomministrazione() As List(Of ViaSomministrazione)
    Function GetCodiceViaSomministrazioneByCodiceACN(codiceACN As String) As String
End Interface
