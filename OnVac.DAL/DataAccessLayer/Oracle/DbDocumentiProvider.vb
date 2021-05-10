Imports System.Data.OracleClient
Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.Data.OracleClient

Namespace DAL.Oracle

    Public Class DbDocumentiProvider
        Inherits DbProvider
        Implements IDocumentiProvider


#Region " Costruttori "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region
#Region " API "

        Public Function GetInfoDocumento(id As Integer) As DocumentoModel

            Dim result As DocumentoModel = New DocumentoModel()

            Dim ownConnection As Boolean = False

            Dim query As String = "SELECT PDO_ID, PDO_DATA_ARCHIVIAZIONE, PDO_NOTE, PDO_DESCRIZIONE, PDO_NOME, PDO_SOGGETTO_EMITTENTE, " +
                                   "ST.ST_DESCRIZIONE ST_DESCRIZIONE, ST.ST_ID ST_ID " +
                                   "FROM T_PAZ_DOCUMENTI " +
                                   "LEFT JOIN T_STATI ST ON PDO_ST_ID = ST.ST_ID " +
                                   "WHERE PDO_ID=:PDO_ID"

            Try
                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    cmd.Parameters.AddWithValue("PDO_ID", id)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()

                        Dim doc_id As Integer = _context.GetOrdinal("PDO_ID")
                        Dim doc_dataA As Integer = _context.GetOrdinal("PDO_DATA_ARCHIVIAZIONE")
                        Dim doc_note As Integer = _context.GetOrdinal("PDO_NOTE")
                        Dim doc_des As Integer = _context.GetOrdinal("PDO_DESCRIZIONE")
                        Dim doc_nome As Integer = _context.GetOrdinal("PDO_NOME")
                        Dim doc_st_des As Integer = _context.GetOrdinal("ST_DESCRIZIONE")
                        Dim doc_st_id As Integer = _context.GetOrdinal("ST_ID")
                        Dim doc_sog_em As Integer = _context.GetOrdinal("PDO_SOGGETTO_EMITTENTE")

                        If _context.Read() Then


                            result.ID = _context.GetInt64OrDefault(doc_id)
                            result.DataArchiviazione = _context.GetDateTimeOrDefault(doc_dataA)
                            result.Descrizione = _context.GetStringOrDefault(doc_note)
                            result.Tipologia = _context.GetStringOrDefault(doc_des)
                            result.NomeDocumento = _context.GetStringOrDefault(doc_nome)
                            result.StatoDocumentoDescrizione = _context.GetStringOrDefault(doc_st_des)
                            result.StatoDocumentoId = _context.GetInt64OrDefault(doc_st_id)
                            result.SoggettoEmittente = _context.GetStringOrDefault(doc_sog_em)
                        End If

                    End Using
                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return result
        End Function

        Public Function SetDocumento(Documento As DocumentoModel) As ResultSetPost
            Dim result As ResultSetPost = New ResultSetPost()
            result.Success = True
            result.Message = "Update eseguito con successo "
            Dim ownConnection As Boolean = False
            Try
                Dim query As String = "INSERT INTO T_PAZ_DOCUMENTI (PDO_DATA_ARCHIVIAZIONE, PDO_DESCRIZIONE, PDO_NOTE, PDO_PAZ_CODICE, PDO_NOME, PDO_ST_ID, PDO_DOCUMENTO, PDO_SOGGETTO_EMITTENTE ) " +
                                   " VALUES " +
                                   "(:PDO_DATA_ARCHIVIAZIONE, :PDO_DESCRIZIONE, :PDO_NOTE,:PDO_PAZ_CODICE, :PDO_NOME, :PDO_ST_ID, :PDO_DOCUMENTO, :PDO_SOGGETTO_EMITTENTE) "

                Using cmd As OracleCommand = New OracleCommand(query, Connection)

                    cmd.Parameters.AddWithValueOrDefault("PDO_DATA_ARCHIVIAZIONE", Documento.DataArchiviazione)
                    cmd.Parameters.AddWithValueOrDefault("PDO_NOTE", Documento.Descrizione)
                    cmd.Parameters.AddWithValueOrDefault("PDO_DESCRIZIONE", Documento.Tipologia)
                    cmd.Parameters.AddWithValueOrDefault("PDO_PAZ_CODICE", Documento.CodicePaziente)
                    cmd.Parameters.AddWithValueOrDefault("PDO_NOME", Documento.NomeDocumento)
                    cmd.Parameters.AddWithValueOrDefault("PDO_ST_ID", Documento.StatoDocumentoId)
                    cmd.Parameters.AddWithValueOrDefault("PDO_DOCUMENTO", Documento.Documento64)
                    cmd.Parameters.AddWithValueOrDefault("PDO_SOGGETTO_EMITTENTE", Documento.SoggettoEmittente)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.ExecuteNonQuery()

                End Using
            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try
            Return result
        End Function

#End Region

    End Class
End Namespace