Namespace Entities

    <Serializable()>
    Public Class Comune
        Public Property Codice As String
        Public Property CodiceEsterno As String
        Public Property Descrizione As String
        Public Property Provincia As String
        Public Property Istat As String
        Public Property Catastale As String
        Public Property Scadenza As Boolean
        Public Property MotivoScadenza As String
        Public Property Cap As String
        Public Property DataInizioValidita As Date
        Public Property DataFineValidita As Date
        Public Property Obsoleto As Boolean
    End Class

End Namespace