Public Interface IMediciProvider

    Function GetMedico(ByRef codiceMedico As String) As Entities.Medico

    Sub InsertMedico(medico As Entities.Medico)

    Function GetCodiciMediciAbilitati(ByRef codiceMedico As String, data As DateTime) As String()

    Function ExistsMedico(codiceMedico As String) As Boolean
    Function GetMedicoByCodiceFiscale(codiceFiscale As String) As Medico
End Interface
