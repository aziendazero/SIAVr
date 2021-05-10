Public Interface IStampaLibrettoVaccinale

    Function GetDtVaccinazioniPaziente(codicePaziente As Integer) As DataTable
    Function GetDataSetLibrettoVaccinale(codicePaziente As Integer, vaccinatore As String) As DSLibrettoVaccinale

End Interface
