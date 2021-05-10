
Public Class VaccinazioniProgrammateException
    Inherits Exception

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(msg As String, innerException As Exception)
        MyBase.New(msg, innerException)
    End Sub

    Public Sub New(msg As String, codicePaziente As Integer, innerException As Exception)
        MyBase.New(String.Format("{0}{1}Paziente: {2}",
                                 msg, Environment.NewLine, codicePaziente), innerException)
    End Sub

    Public Sub New(msg As String, codicePaziente As Integer, dataConvocazione As Date, codiceCiclo As String, codiceVaccinazione As String, codiceAssociazione As String, numeroSeduta As Integer, numeroRichiamo As Integer, innerException As Exception)
        MyBase.New(String.Format("{0}{1}Paziente: {2}{1}Data Convocazione: {3:dd/MM/yyyy}{1}Ciclo: {4}{1}Seduta: {5}{1}Vaccinazione: {6}{1}Associazione: {7}{1}Richiamo: {8}", 
                                 msg, Environment.NewLine, codicePaziente, dataConvocazione, codiceCiclo, numeroSeduta, codiceVaccinazione, codiceAssociazione, numeroRichiamo), innerException)
    End Sub

End Class


