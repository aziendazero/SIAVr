Namespace Exceptions
    Public Class DataIntervalNotSetException
        Inherits System.Exception

        Sub New()
            MyBase.New("Le date di inizio e di fine per la ricerca degli appuntamenti non sono state impostate.")
        End Sub
    End Class
End Namespace
