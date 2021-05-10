Namespace Entities.ArchiviazioneDIRV

    <Serializable>
    Public Class DocumentoFirma
        Public IdDocumento As Long
        Public IdVisita As Long
        Public TestoDocumento As String
        Public IsDocumentoFirmato As Boolean
        Public IsDocumentoArchiviato As Boolean
        Public CodiceAziendaInserimento As String
        Public TokenArchiviazione As String
    End Class

End Namespace