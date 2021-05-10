Imports Onit.OnAssistnet.OnVac.Filters

Public Interface ICoperturaProvider

    Function GetTotaleAnagrafico(filtro As FiltriCopertura) As Int32
    Function GetCoperturaVaccinale(filtro As FiltriCopertura) As dsCoperturaVaccinale
    Function GetCoperturaVaccinaleCNS(filtro As FiltriCopertura) As dsCoperturaVaccinaleCNS
    Function GetMotiviEsclusione(filtro As FiltriCopertura) As dsMotiviEsclusione
    Function GetElencoNonVaccinati(filtro As FiltriCopertura, codiceUsl As String) As dsNonVaccinati
    Function GetElencoVaccinati(filtro As FiltriCopertura, codiceUsl As String) As DsVaccinati

    Function GetTotaleAnagraficoMedico(filtro As FiltriCoperturaMedico) As Int32
    Function GetCoperturaVaccinaleMedico(filtro As FiltriCoperturaMedico) As dsCoperturaVaccinaleMedico
    Function GetNonVaccinatiMedico(filtro As FiltriCoperturaMedico, codiceUsl As String) As dsNonVaccinatiMedico
    Function GetVaccinatiMedico(filtro As FiltriCoperturaMedico, codiceUsl As String) As dsVaccinatiMedico

End Interface
