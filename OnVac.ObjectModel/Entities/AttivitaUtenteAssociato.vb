Namespace Entities

    <Serializable>
    Public Class AttivitaUtenteAssociato

        Public Property IdUtenti As Integer
        Public Property Codice As String
        Public Property Descrizione As String
        Public Property Cognome As String
        Public Property Nome As String
        Public Property AppId As String
        Public Property Id As Integer
        Public Property IdAttivita As Integer
        Public Property UslCodice As String


        Public Enum Ordinamento
            Codice = 0
            Descrizione = 1
            Cognome = 2
            Nome = 3
            AppId = 4
            UslCodice = 5
        End Enum

    End Class

End Namespace
