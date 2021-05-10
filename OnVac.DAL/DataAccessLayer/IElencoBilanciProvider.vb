
Public Interface IElencoBilanciProvider

    Function fillDtElencoBilanci(ByRef dt As DataTable, codCns As String, codAmb As Integer, strDataInizio As String, strDataFine As String, filtroPazAvvisati As OnVac.Enumerators.FiltroAvvisati, malattia As String, codiceUsl As String) As Boolean

End Interface
