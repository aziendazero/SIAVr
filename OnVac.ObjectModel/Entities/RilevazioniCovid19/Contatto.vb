Namespace Entities
    <Serializable()>
    Public Class Contatto
        Public Property CodiceContatto As Long
        Public Property CodiceTipoRapporto As String
        Public Property DescrizioneTipoRapporto As String
        Public Property Note As String
        Public Property UtenteInserimento As Long
        Public Property CodiceInserimentoUsl As String
        Public Property DataInserimento As DateTime?
        Public Property CodicePaziente As Long?
        Public Property DescrizionePaziente As String
        Public Property Telefono As String
        Public Property CodiceImportazione As String
        Public Property CodiceEpisodioContatto As Long?
    End Class
End Namespace
