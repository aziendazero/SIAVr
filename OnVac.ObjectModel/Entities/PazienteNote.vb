Namespace Entities

    <Serializable>
    Public Class PazienteNote

        Public Property CodiceNota As String
        Public Property DescrizioneNota As String
        Public Property OrdineNota As Integer?
        Public Property FlagNotaModificabile As Boolean?
        Public Property IdNota As Long?
        Public Property CodicePaziente As Long
        Public Property TestoNota As String
        Public Property CodiceAzienda As String
        Public Property IdUtenteUltimaModifica As Long?
        Public Property DataUltimaModifica As DateTime?
        Public Property FlagNotaVisibileConvocazioni As Boolean?

    End Class

End Namespace