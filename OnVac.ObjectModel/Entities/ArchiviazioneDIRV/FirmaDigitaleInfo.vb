Namespace Entities.ArchiviazioneDIRV

    <Serializable>
    Public Class FirmaDigitaleInfo

        Public IdDocumento As Long
        Public TipoDocumento As String
        Public DataInserimento As DateTime?
        Public IdUtenteInserimento As Long?
        Public CodiceAziendaInserimento As String
        Public IdVisita As Long?
        Public DataFirma As DateTime?
        Public IdUtenteFirma As Long?
        Public CodiceAziendaFirma As String
        Public NomeFile As String
        Public Documento As String
        Public DataArchiviazione As DateTime?
        Public IdUtenteArchiviazione As Long?
        Public TokenArchiviazione As String
        Public CodiceErroreDIRV As String
        Public DescrizioneErroreDIRV As String

    End Class

    ''' <summary>
    ''' Info su firma digitale e archiviazione sostituitiva relative ad una visita
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable>
    Public Class InfoFirmaArchiviazioneVisita

        Public IdVisita As Long
        Public IdDocumento As Long?
        Public UtenteFirma As String
        Public DataFirma As DateTime?
        Public UtenteArchiviazione As String
        Public DataArchiviazione As DateTime?

    End Class

    <Serializable>
    Public Class DocumentoVisita

        Public Property IdVisita As Long
        Public Property PazienteInfo As String
        Public Property DataVisita As DateTime
        Public Property UtenteVisita As String
        Public Property DataRegistrazione As DateTime
        Public Property CodiceAziendaInserimento As String

        Public Property IdUtenteFirma As Long?
        Public Property DataFirma As DateTime?
        Public Property UtenteFirma As String
        Public Property IdUtenteArchiviazione As Long?
        Public Property DataArchiviazione As DateTime?
        Public Property UtenteArchiviazione As String
        Public Property IdDocumento As Long?

        Public Property UtenteRilevatore As String

    End Class

    <Serializable>
    Public Class FiltriDocumentiVisite

        Public IdUtenteRegistrazione As Long?
        Public IdUtenteFirma As Long?
        Public IdUtenteRilevatore As String
        Public DataDa As DateTime
        Public DataA As DateTime
        Public FiltroStato As FiltroStatoDocumento
        Public UslCorrente As String

    End Class

    <Serializable>
    Public Enum FiltroStatoDocumento
        DaFirmare
        FirmatiNonArchiviati
        FirmatiArchiviati
        Tutti
    End Enum

End Namespace