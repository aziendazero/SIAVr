Namespace Entities
    <Serializable()>
    Public Class Ricovero
        Public Property CodiceRicovero As Long
        Public Property CodiceStruttura As String
        Public Property DescrizioneStruttura As String
        Public Property CodiceUsl As String
        Public Property CodiceReparto As Integer?
        Public Property DescrizioneReparto As String
        Public Property DataInizio As DateTime?
        Public Property DataFine As DateTime?
        Public Property Note As String
        Public Property UtenteInserimento As Long
        Public Property CodiceInserimentoUsl As String
        Public Property DataInserimento As DateTime?
    End Class
End Namespace
