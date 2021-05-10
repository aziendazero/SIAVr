Namespace Entities

    <Serializable>
    Public Class BilancioAnagrafica

        Public Property NumeroBilancio As Integer
        Public Property DescrizioneBilancio As String
        Public Property CodiceMalattia As String
        Public Property DescrizioneMalattia As String
        Public Property EtaMinima As Integer?
        Public Property EtaMassima As Integer?
        Public Property Obbligatorio As Boolean
        Public Property GestioneCranio As Boolean
        Public Property GestioneAltezza As Boolean
        Public Property GestionePeso As Boolean
        Public Property GestioneVaccinazioni As Boolean
        Public Property GestioneViaggi As Boolean
        Public Property NomeReport As String

    End Class

End Namespace
