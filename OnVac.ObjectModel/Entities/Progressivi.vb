Namespace Entities

    <Serializable>
    Public Class Progressivi

        Public Property Codice As String
        Public Property Progressivo As Int64
        Public Property Anno As Int16
        Public Property Prefisso As String
        Public Property Lunghezza As Int16
        Public Property Max As Int64
        Public Property CodiceAzienda As String

        Public Sub New()
            Me.Codice = String.Empty
            Me.Progressivo = 0
            Me.Anno = 0
            Me.Prefisso = String.Empty
            Me.Lunghezza = 0
            Me.Max = 0
            Me.CodiceAzienda = String.Empty
        End Sub

    End Class

End Namespace