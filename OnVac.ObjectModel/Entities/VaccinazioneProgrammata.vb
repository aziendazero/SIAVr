Imports System.Collections.ObjectModel

Namespace Entities

    <Serializable()>
    Public Class VaccinazioneProgrammata

        Public Property DataConvocazione As DateTime
        Public Property CodicePaziente As Int64
        Public Property CodiceVaccinazione As String
        Public Property NumeroRichiamo As Int16?
        Public Property CodiceCiclo As String
        Public Property NumeroSeduta As Int16?
        Public Property CodiceAssociazione As String
        Public Property CodicePazientePrecedente As Int64?
        Public Property DataInserimento As DateTime?
        Public Property IdUtenteInserimento As Int64?

    End Class

End Namespace