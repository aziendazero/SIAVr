Imports Onit.OnAssistnet.OnVac.Log.DataLogStructure

Namespace LogWriterProvider

    Public Interface ILogWriterProvider

        Property Name() As String
        Property Enabled() As Boolean
        Property FiltroCriticita() As String

        Sub WriteLog(test As Testata, forzaGruppo As Boolean, connectionString As String, provider As String)
        Sub WriteLog(test() As Testata, forzaGruppo As Boolean, connectionString As String, provider As String)

        Function HandleCriticita(priorita As Integer) As Boolean

    End Interface

End Namespace