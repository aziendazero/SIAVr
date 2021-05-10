
Public Class CicliException
    Inherits Exception

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(msg As String, innerException As Exception)
        MyBase.New(msg, innerException)
    End Sub

    Public Sub New(msg As String, codicePaziente As Integer, dataConvocazione As Date, codiceCiclo As String, numeroSeduta As Integer, innerException As Exception)
        MyBase.New(String.Format("{0}{1}Paziente: {2}{1}Data Convocazione: {3:dd/MM/yyyy}{1}Ciclo: {4}{1}Seduta: {5}",
                                 msg, Environment.NewLine, codicePaziente, dataConvocazione, codiceCiclo, numeroSeduta), innerException)
    End Sub

End Class


