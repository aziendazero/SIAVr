Public Interface ICittadinanzeProvider

    Function GetCodiceCittadinanzaByCodiceIstat(codiceIstatCittadinanza As String) As String
    Function GetCodiceCittadinanzaByCodiceIstat(codiceIstatCittadinanza As String, dataValidita As Date) As String

End Interface
