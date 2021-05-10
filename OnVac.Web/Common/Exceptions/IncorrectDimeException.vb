Public Class IncorrectDimeException
    Inherits Exception

    Public Sub New()
        MyBase.New("Richiesta al server non valida. Fai click su un pulsante del menu' per continuare.") ', e' probabile che sia stata premuta una sequenza di tasti non corretta
    End Sub

End Class
