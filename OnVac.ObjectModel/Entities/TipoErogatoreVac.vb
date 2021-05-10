Namespace Entities

    <Serializable>
    Public Class TipoErogatoreVacc

        Public Property Id As Integer
        Public Property Codice As String
        Public Property Descrizione As String
        Public Property CodiceAvn As String
        Public Property Ordine As Integer?
        Public Property Obsoleto As String
        Public ReadOnly Property IdString() As String
            Get
                Return If(Id = 0, "", Id.ToString())
            End Get
        End Property

    End Class

    <Serializable>
    Public Class TipoErogatoreVaccCommand

        Public Property Id As Integer
        Public Property Codice As String
        Public Property Descrizione As String
        Public Property CodiceAvn As String
        Public Property Ordine As Integer?
        Public Property Obsoleto As String

        Public Property CodiceMaxLength As Integer
        Public Property DescrizioneMaxLength As Integer
        Public Property CodiceAvnMaxLength As String
        Public Property OrdineMaxLength As String

    End Class

End Namespace
