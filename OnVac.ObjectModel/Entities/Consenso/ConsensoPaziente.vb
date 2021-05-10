Namespace Entities.Consenso

    <Serializable>
    Public Class ConsensoPaziente
        Public Property ProgressivoConsenso As Long
        Public Property CodicePaziente As String
        Public Property DataEvento As DateTime
        Public Property DataScadenza As DateTime?
        Public Property IdLivello As Integer
        Public Property Azienda As String
        Public Property IdEnte As Integer
        Public Property Addetto As String
        Public Property DataRegistrazione As DateTime
        Public Property Applicativo As String
        Public Property CodiceAzienda As String
        Public Property CampoCustom1 As String
        Public Property CampoCustom2 As String
        Public Property CampoCustom3 As String
    End Class

End Namespace