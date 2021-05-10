Public Class VaccinazioniSostituteException
    Inherits Exception

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(msg As String, innerException As Exception)
        MyBase.New(msg, innerException)
    End Sub

    Public Sub New(msg As String, codicePaziente As Integer, codiceVaccinazione As String, innerException As Exception)
        MyBase.New(String.Format("{0}{1}Paziente: {2}{1}Vaccinazione: {3}",
                                 msg, Environment.NewLine, codicePaziente, codiceVaccinazione), innerException)
    End Sub

End Class
