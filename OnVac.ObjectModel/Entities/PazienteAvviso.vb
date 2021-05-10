Namespace Entities

    <Serializable>
    Public Class PazienteAvviso
        Public CodicePaziente As Integer
        Public DataConvocazione As DateTime
        Public IsPazienteRitardatario As Boolean
    End Class

    <Serializable>
    Public Class PazientiAvvisiCommand
        Public TipoStampaAppuntamento As String
        Public DataInizioAppuntamento As DateTime
        Public DataFineAppuntamento As DateTime
        Public DataInizioNascita As DateTime?
        Public DataFineNascita As DateTime?
        Public CodiceCittadinanza As String
        Public CodiceConsultorio As String
        Public CodiceAmbulatorio As String
        Public Distretti As String
        Public userId As Long?
        Public FiltroPazientiAvvisati As Enumerators.FiltroAvvisati
        Public NoteAvvisi As String
        Public IsPostel As Boolean
    End Class

End Namespace
