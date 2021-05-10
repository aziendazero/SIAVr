Namespace Entities

    <Serializable()>
    Public Class BilancioDaSollecitare

        Public CodicePaziente As Long
        Public IdBilancio As Integer
        Public SollecitiCreati As Integer
        Public UltimaDataInvio As DateTime?
        Public NumeroSollecitiBilancio As Integer
        Public FlagSoloBilancio As Boolean
        Public DataConvocazione As DateTime
        Public CodiceMalattia As String
        Public NumeroBilancio As Integer
        Public StatoBilancio As String

    End Class

End Namespace