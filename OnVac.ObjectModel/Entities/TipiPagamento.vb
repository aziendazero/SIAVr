Namespace Entities

    <Serializable()>
    Public Class TipiPagamento

        Public Property GuidPagamento As Guid
        Public Property Descrizione As String
        Public Property CodiceEsterno As String
        Public Property FlagStatoCampoImporto As Enumerators.StatoAbilitazioneCampo?
        Public Property FlagStatoCampoEsenzione As Enumerators.StatoAbilitazioneCampo?
        Public Property AutoSetImporto As String
        Public Property HasCondizioniPagamento As String
        Public Property CodiceAvn As String

        Public Property DescrizioneMaxLength As Integer
        Public Property CodiceEsternoMaxLength As Integer
        Public Property CodiceAvnMaxLength As Integer

    End Class

End Namespace
