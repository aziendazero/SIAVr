Public Interface ICodificheProvider

    Function GetCodifiche(campo As String) As Collection.CodificheCollection
    Function GetCodifica(campo As String, codice As String) As Entities.Codifica

End Interface
