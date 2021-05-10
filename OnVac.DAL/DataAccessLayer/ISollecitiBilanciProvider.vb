
Public Interface ISollecitiBilanciProvider

    Function DeleteRecord(keypaz As Integer, mal_codice As String) As Boolean
    Function Update(id As Integer, dataInvio As Date) As Boolean
    Function NewRecord(bilID As Integer, dataInvio As Date) As Boolean
    Function GetKey(bilId As Integer, dataInvio As Date) As Integer

End Interface
