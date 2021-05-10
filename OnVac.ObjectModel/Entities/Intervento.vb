Namespace Entities

    <Serializable()>
    Public Class Intervento
        Public Property Codice As Integer?
        Public Property Descrizione As String
        Public Property Tipologia As String
        Public Property Durata As Integer?
    End Class

    <Serializable()>
    Public Class InterventoPaziente
        Public Property Codice As Integer?
        Public Property CodiceIntervento As Integer
        Public Property Intervento As String
        'Public Property CodicePaziente As Integer
        Public Property Data As Date
        Public Property Durata As Integer?
        Public Property Tipologia As String
        Public Property CodiceOperatore As String
        Public Property Operatore As String
        Public Property Note As String
    End Class

End Namespace