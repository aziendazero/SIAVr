Namespace Entities
    <Serializable()>
    Public Class DatoreLavoro
        Public Property DenominazioneAzienda As String
        Public Property RiferimentoDatoreLavoro As String
        Public Property CodiceComuneSede As String
        Public Property DescrizioneComuneSede As String
        Public Property ContattoTelefonico As String
        Public Property ContattoEmail As String
        Public Property Note As String
        Public Property UtenteInserimento As Long
        Public Property CodiceInserimentoUsl As String
        Public Property DataInserimento As DateTime?
        Public Property CodiceTipoAzienda As Integer?
        Public Property DescrizioneTipoAzienda As String
    End Class

End Namespace
