Namespace Entities

    <Serializable()>
    Public Class ConvocazioneAppuntamento

        Public Property Id As Long
        Public Property CodicePaziente As Long
        Public Property DataConvocazione As DateTime
        Public Property Vaccinazioni As String
        Public Property CodiceConsultorio As String
        Public Property CodiceAmbulatorio As Integer?

        Public Property DataAppuntamento As DateTime?
        Public Property DataRegistrazioneAppuntamento As DateTime?
        Public Property IdUtenteRegistrazioneAppuntamento As Long?

        Public Property DurataAppuntamento As Integer?
        Public Property TipoAppuntamento As String

        Public Property DataInvio As DateTime?
        Public Property IdUtenteInvio As Long?

        Public Property Note As String
        Public Property NoteAvvisi As String
        Public Property NoteModificaAppuntamento As String
        Public Property CodicePazienteOld As Long?
        Public Property DataPrimoAppuntamento As DateTime?
        Public Property Campagna As String

        Public Property DataEliminazioneAppuntamento As DateTime?
        Public Property IdUtenteEliminazioneAppuntamento As Long?
        Public Property IdMotivoEliminazioneAppuntamento As String

        ' Obsoleto?
        Public Property EtaPomeriggio As String

    End Class

End Namespace