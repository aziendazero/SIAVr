Public Class ConvocazioniException
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

    Public Sub New(msg As String, codicePaziente As Integer, dataConvocazione As Date, innerException As Exception)
        MyBase.New(String.Format("{0}{1}Paziente: {2}{1}Data Convocazione: {3:dd/MM/yyyy}",
                                 msg, Environment.NewLine, codicePaziente, dataConvocazione), innerException)
    End Sub

    Public Sub New(msg As String, codicePaziente As Integer, dataConvocazione As Date, codiceConsultorio As String, durata As Int16, innerException As Exception)
        MyBase.New(String.Format("{0}{1}Paziente: {2}{1}Data Convocazione: {3:dd/MM/yyyy}{1}Consultorio: {4}{1}Durata: {5}{1}",
                                 msg, Environment.NewLine, codicePaziente, dataConvocazione, codiceConsultorio, durata), innerException)
    End Sub

End Class
