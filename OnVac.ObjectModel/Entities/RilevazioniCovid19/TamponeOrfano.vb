Namespace Entities
    Public Class TamponeOrfano
        Inherits TamponeDiFrontiera

        Public Property IdPaziente As Long?
        Public Property DescrizioneUlss As String
    End Class


    Public Class RicercaTamponiOrfani
        Public Property CodiceUlss As String
        Public Property DataReferto As Date?
        Public Property DataPrelievo As Date?
        Public Property Esito As String
        Public Property Skip As Integer?
        Public Property Take As Integer?

        Public Property PazienteMancante As Boolean
        Public Property UlssMancante As Boolean
        Public Property Nome As String
        Public Property Cognome As String
        Public Property DataNascita As DateTime?
    End Class
End Namespace
