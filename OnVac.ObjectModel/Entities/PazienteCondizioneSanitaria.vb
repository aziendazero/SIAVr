Namespace Entities

    <Serializable()>
    Public Class PazienteCondizioneSanitaria

        Public Property CodiceMalattia As String
        Public Property DescrizioneMalattia As String
        Public Property CodiceVaccinazione As String
        Public Property CodicePaziente As Long

        ''' <summary>
        ''' Indica se la malattia è associata al paziente
        ''' </summary>
        ''' <returns></returns>
        Public Property FlagMalattiaPaziente As String

        Public Property FlagCondizioneSanitariaDefault As String

    End Class

End Namespace
