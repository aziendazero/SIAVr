Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager
Imports System.Data.OracleClient
Imports Onit.OnAssistnet.Data.OracleClient

Namespace DAL

    Public Class DbFirmaDigitaleProvider
        Inherits DbProvider
        Implements IFirmaDigitaleProvider

#Region " Costruttori "

        Public Sub New(ByRef DAM As IDAM)
            MyBase.New(DAM)
        End Sub

#End Region

#Region " Public "

        ''' <summary>
        ''' Restituisce il documento in base alla visita e all'azienda specificati
        ''' </summary>
        ''' <param name="idVisita"></param>
        ''' <param name="codiceAzienda"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDocumentoByVisita(idVisita As Integer, codiceAzienda As String) As ArchiviazioneDIRV.DocumentoFirma Implements IFirmaDigitaleProvider.GetDocumentoByVisita

            Dim documentoFirma As ArchiviazioneDIRV.DocumentoFirma = Nothing

            Using cmd As New OracleCommand()

                cmd.Connection = Me.Connection
                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.CommandText = "SELECT DOC_ID, DOC_ID_VISITA, DOC_FILE, DOC_USL_INSERIMENTO, DOC_UTE_ID_FIRMA, DOC_TOKEN_ARCHIVIAZIONE " +
                                      "FROM T_DOCUMENTI_FIRMA WHERE DOC_ID_VISITA = :DOC_ID_VISITA AND " +
                                      "(DOC_USL_INSERIMENTO = :DOC_USL_INSERIMENTO OR " +
                                      " EXISTS (SELECT 1 FROM T_ANA_DISTRETTI WHERE DIS_CODICE = DOC_USL_INSERIMENTO AND DIS_USL_CODICE = :DOC_USL_INSERIMENTO))"

                    cmd.Parameters.AddWithValue("DOC_ID_VISITA", idVisita)
                    cmd.Parameters.AddWithValue("DOC_USL_INSERIMENTO", codiceAzienda)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim doc_id As Integer = idr.GetOrdinal("DOC_ID")
                            Dim doc_id_visita As Integer = idr.GetOrdinal("DOC_ID_VISITA")
                            Dim doc_file As Integer = idr.GetOrdinal("DOC_FILE")
                            Dim doc_usl_inserimento As Integer = idr.GetOrdinal("DOC_USL_INSERIMENTO")
                            Dim doc_ute_id_firma As Integer = idr.GetOrdinal("DOC_UTE_ID_FIRMA")
                            Dim doc_token_archiviazione As Integer = idr.GetOrdinal("DOC_TOKEN_ARCHIVIAZIONE")

                            If idr.Read() Then
                                documentoFirma = New ArchiviazioneDIRV.DocumentoFirma()
                                documentoFirma.IdDocumento = idr.GetInt64(doc_id)
                                documentoFirma.IdVisita = idr.GetInt64OrDefault(doc_id_visita)
                                documentoFirma.TestoDocumento = idr.GetStringOrDefault(doc_file)
                                documentoFirma.CodiceAziendaInserimento = idr.GetString(doc_usl_inserimento)

                                Dim idUtenteFirma As Long? = idr.GetNullableInt64OrDefault(doc_ute_id_firma)
                                documentoFirma.IsDocumentoFirmato = idUtenteFirma.HasValue

                                documentoFirma.TokenArchiviazione = idr.GetStringOrDefault(doc_token_archiviazione)
                                documentoFirma.IsDocumentoArchiviato = Not String.IsNullOrWhiteSpace(documentoFirma.TokenArchiviazione)
                            End If

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return documentoFirma

        End Function

        ''' <summary>
        ''' Restituisce l'id della visita relativa al documento specificato.
        ''' </summary>
        ''' <param name="idDocumento"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetIdVisitaByIdDocumento(idDocumento As Long) As Long Implements IFirmaDigitaleProvider.GetIdVisitaByIdDocumento

            ' TODO [firma digitale]: ricerca visita in base a id documento => non è implementato il caso in cui non venga trovato l'id

            Dim idVisita As Long

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection
                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.CommandText = "SELECT DOC_ID_VISITA FROM T_DOCUMENTI_FIRMA WHERE DOC_ID = :DOC_ID"
                    cmd.Parameters.AddWithValue("DOC_ID", idDocumento)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        idVisita = Convert.ToInt32(obj)
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return idVisita

        End Function

        ''' <summary>
        ''' Restituisce il documento in base all'id specificato
        ''' </summary>
        ''' <param name="idDocumento"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDocumentoById(idDocumento As Long) As String Implements IFirmaDigitaleProvider.GetDocumentoById

            Dim documento As String = String.Empty

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection
                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.CommandText = "SELECT DOC_FILE FROM T_DOCUMENTI_FIRMA WHERE DOC_ID = :DOC_ID"
                    cmd.Parameters.AddWithValue("DOC_ID", idDocumento)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        documento = obj.ToString()
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return documento

        End Function

        ''' <summary>
        ''' Restituisce il nome del file relativo al documento specificato
        ''' </summary>
        ''' <param name="idDocumento"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetNomeFileByIdDocumento(idDocumento As Long) As String Implements IFirmaDigitaleProvider.GetNomeFileByIdDocumento

            Dim nomeFile As String = String.Empty

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection
                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.CommandText = "SELECT DOC_NOME_FILE FROM T_DOCUMENTI_FIRMA WHERE DOC_ID = :DOC_ID"
                    cmd.Parameters.AddWithValue("DOC_ID", idDocumento)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        nomeFile = obj.ToString()
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return nomeFile

        End Function

        ''' <summary>
        ''' Se presente, restituisce il token di archiviazione del documento specificato. Altrimenti restituisce una stringa vuota.
        ''' </summary>
        ''' <param name="idDocumento"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetTokenArchiviazioneDocumento(idDocumento As Long) As String Implements IFirmaDigitaleProvider.GetTokenArchiviazioneDocumento

            Dim token As String = String.Empty

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection
                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.CommandText = "SELECT DOC_TOKEN_ARCHIVIAZIONE FROM T_DOCUMENTI_FIRMA WHERE DOC_ID = :DOC_ID"
                    cmd.Parameters.AddWithValue("DOC_ID", idDocumento)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        token = obj.ToString()
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return token

        End Function

        ''' <summary>
        ''' Restituisce l'id del documento preso dalla sequence.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetNewIdDocumento() As Long Implements IFirmaDigitaleProvider.GetNewIdDocumento

            Dim idDocumento As Long = 0

            Using cmd As New OracleClient.OracleCommand("SELECT SEQ_DOC.NEXTVAL FROM DUAL", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        idDocumento = Convert.ToInt32(obj)
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return idDocumento

        End Function

        ''' <summary>
        ''' Restituisce la lista di id dei documenti firmati ma non archiviati, relativi all'azienda specificata.
        ''' </summary>
        ''' <param name="codiceAzienda"></param>
        ''' <returns></returns>
        ''' <remarks>SOLO CENTRALE</remarks>
        Public Function GetListIdDocumentiDaArchiviare(codiceAzienda As String) As List(Of Long) Implements IFirmaDigitaleProvider.GetListIdDocumentiDaArchiviare

            Dim listId As New List(Of Long)()

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                cmd.CommandText = "select DOC_ID from T_DOCUMENTI_FIRMA where DOC_USL_FIRMA = :DOC_USL_FIRMA " +
                                  "and not DOC_UTE_ID_FIRMA is null and DOC_UTE_ID_ARCHIVIAZIONE is null"

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("DOC_USL_FIRMA", codiceAzienda)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim doc_id As Integer = idr.GetOrdinal("DOC_ID")

                            While idr.Read()
                                listId.Add(idr.GetInt64(doc_id))
                            End While

                        End If
                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listId

        End Function

        ''' <summary>
        ''' Restituisce l'id dell'utente che ha firmato il documento, oppure null se il documento non è firmato.
        ''' </summary>
        ''' <param name="idDocumento"></param>
        ''' <returns></returns>
        ''' <remarks>SOLO CENTRALE</remarks>
        Public Function GetIdUtenteFirma(idDocumento As Long) As Long? Implements IFirmaDigitaleProvider.GetIdUtenteFirma

            Dim idUtenteFirma As Long? = Nothing

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection
                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.CommandText = "SELECT DOC_UTE_ID_FIRMA FROM T_DOCUMENTI_FIRMA WHERE DOC_ID = :DOC_ID"
                    cmd.Parameters.AddWithValue("DOC_ID", idDocumento)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        idUtenteFirma = Convert.ToInt64(obj)
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return idUtenteFirma

        End Function

        ''' <summary>
        ''' Inserimento dei dati relativi ad un documento da firmare/firmato sul database centrale. Restituisce l'id del documento inserito.
        ''' </summary>
        ''' <param name="firmaDigitaleInfo"></param>
        ''' <param name="numeroTentativiArchiviazione"></param>
        ''' <returns></returns>
        ''' <remarks>SOLO CENTRALE</remarks>
        Public Function InsertDocumento(firmaDigitaleInfo As Entities.ArchiviazioneDIRV.FirmaDigitaleInfo, numeroTentativiArchiviazione As Integer) As Integer Implements IFirmaDigitaleProvider.InsertDocumento

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection
                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.CommandText = "INSERT INTO T_DOCUMENTI_FIRMA (DOC_ID, DOC_TIPO_DOCUMENTO, DOC_DATA_INSERIMENTO, DOC_UTE_ID_INSERIMENTO, DOC_USL_INSERIMENTO, DOC_ID_VISITA, " +
                        "DOC_DATA_FIRMA, DOC_UTE_ID_FIRMA, DOC_USL_FIRMA, DOC_NOME_FILE, DOC_FILE, DOC_N_TENTATIVI, DOC_DATA_ARCHIVIAZIONE, DOC_UTE_ID_ARCHIVIAZIONE, DOC_TOKEN_ARCHIVIAZIONE) " +
                        "VALUES (:DOC_ID, :DOC_TIPO_DOCUMENTO, :DOC_DATA_INSERIMENTO, :DOC_UTE_ID_INSERIMENTO, :DOC_USL_INSERIMENTO, :DOC_ID_VISITA, " +
                        ":DOC_DATA_FIRMA, :DOC_UTE_ID_FIRMA, :DOC_USL_FIRMA, :DOC_NOME_FILE, :DOC_FILE, :DOC_N_TENTATIVI, :DOC_DATA_ARCHIVIAZIONE, :DOC_UTE_ID_ARCHIVIAZIONE, :DOC_TOKEN_ARCHIVIAZIONE)"

                    cmd.Parameters.AddWithValue("DOC_ID", firmaDigitaleInfo.IdDocumento)
                    cmd.Parameters.AddWithValue("DOC_TIPO_DOCUMENTO", firmaDigitaleInfo.TipoDocumento)

                    cmd.Parameters.AddWithValue("DOC_DATA_INSERIMENTO", firmaDigitaleInfo.DataInserimento)
                    cmd.Parameters.AddWithValue("DOC_UTE_ID_INSERIMENTO", firmaDigitaleInfo.IdUtenteInserimento)
                    cmd.Parameters.AddWithValue("DOC_USL_INSERIMENTO", firmaDigitaleInfo.CodiceAziendaInserimento)

                    cmd.Parameters.AddWithValue("DOC_ID_VISITA", DbProvider.GetLongParam(firmaDigitaleInfo.IdVisita))

                    cmd.Parameters.AddWithValue("DOC_DATA_FIRMA", DbProvider.GetDateParam(firmaDigitaleInfo.DataFirma))
                    cmd.Parameters.AddWithValue("DOC_UTE_ID_FIRMA", DbProvider.GetLongParam(firmaDigitaleInfo.IdUtenteFirma))
                    cmd.Parameters.AddWithValue("DOC_USL_FIRMA", DbProvider.GetStringParam(firmaDigitaleInfo.CodiceAziendaFirma))
                    cmd.Parameters.AddWithValue("DOC_NOME_FILE", DbProvider.GetStringParam(firmaDigitaleInfo.NomeFile))

                    cmd.Parameters.AddWithValue("DOC_N_TENTATIVI", numeroTentativiArchiviazione)
                    cmd.Parameters.AddWithValue("DOC_DATA_ARCHIVIAZIONE", DbProvider.GetDateParam(firmaDigitaleInfo.DataArchiviazione))
                    cmd.Parameters.AddWithValue("DOC_UTE_ID_ARCHIVIAZIONE", DbProvider.GetLongParam(firmaDigitaleInfo.IdUtenteArchiviazione))
                    cmd.Parameters.AddWithValue("DOC_TOKEN_ARCHIVIAZIONE", DbProvider.GetStringParam(firmaDigitaleInfo.TokenArchiviazione))

                    If firmaDigitaleInfo.Documento Is Nothing Then
                        cmd.Parameters.AddWithValue("DOC_FILE", DBNull.Value)
                    Else
                        cmd.Parameters.AddWithValue("DOC_FILE", firmaDigitaleInfo.Documento)
                    End If

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Cancellazione del record con id specificato dalla t_documenti_firma 
        ''' </summary>
        ''' <param name="idDocumento"></param>
        ''' <returns></returns>
        ''' <remarks>SOLO CENTRALE</remarks>
        Public Function DeleteDocumento(idDocumento As Long) As Integer Implements IFirmaDigitaleProvider.DeleteDocumento

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand("DELETE FROM T_DOCUMENTI_FIRMA WHERE DOC_ID = :DOC_ID", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)
                    cmd.Parameters.AddWithValue("DOC_ID", idDocumento)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Cancellazione dei documenti relativi alle visite specificate
        ''' </summary>
        ''' <param name="listIdVisita"></param>
        ''' <returns></returns>
        ''' <remarks>SOLO CENTRALE</remarks>
        Public Function DeleteDocumentiVisite(listIdVisita As List(Of Long)) As Integer Implements IFirmaDigitaleProvider.DeleteDocumentiVisite

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                Dim query As String = "DELETE FROM T_DOCUMENTI_FIRMA WHERE DOC_ID_VISITA IN ({0})"

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim filter As GetInFilterResult = Me.GetInFilter(listIdVisita)

                    cmd.CommandText = String.Format(query, filter.InFilter)
                    cmd.Parameters.AddRange(filter.Parameters)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Update dei dati relativi al documento da firmare/firmato sul db centrale. 
        ''' </summary>
        ''' <param name="firmaDigitaleInfo"></param>
        ''' <param name="incrementaTentativi"></param>
        ''' <returns></returns>
        ''' <remarks>SOLO CENTRALE</remarks>
        Public Function UpdateDatiFirmaDocumento(firmaDigitaleInfo As Entities.ArchiviazioneDIRV.FirmaDigitaleInfo) As Integer Implements IFirmaDigitaleProvider.UpdateDatiFirmaDocumento

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection
                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim query As New System.Text.StringBuilder("UPDATE T_DOCUMENTI_FIRMA SET ")

                    If firmaDigitaleInfo.DataFirma.HasValue Then
                        query.Append(" DOC_DATA_FIRMA = :DOC_DATA_FIRMA, ")
                        cmd.Parameters.AddWithValue("DOC_DATA_FIRMA", firmaDigitaleInfo.DataFirma.Value)
                    End If

                    If firmaDigitaleInfo.IdUtenteFirma.HasValue Then
                        query.Append(" DOC_UTE_ID_FIRMA = :DOC_UTE_ID_FIRMA, ")
                        cmd.Parameters.AddWithValue("DOC_UTE_ID_FIRMA", firmaDigitaleInfo.IdUtenteFirma.Value)
                    End If

                    If Not String.IsNullOrWhiteSpace(firmaDigitaleInfo.CodiceAziendaFirma) Then
                        query.Append(" DOC_USL_FIRMA = :DOC_USL_FIRMA, ")
                        cmd.Parameters.AddWithValue("DOC_USL_FIRMA", firmaDigitaleInfo.CodiceAziendaFirma)
                    End If

                    If Not String.IsNullOrWhiteSpace(firmaDigitaleInfo.Documento) Then
                        query.Append(" DOC_FILE = :DOC_FILE, ")
                        cmd.Parameters.AddWithValue("DOC_FILE", firmaDigitaleInfo.Documento)
                    End If

                    query.Remove(query.Length - 2, 2)

                    query.Append(" where DOC_ID = :DOC_ID ")
                    cmd.Parameters.AddWithValue("DOC_ID", firmaDigitaleInfo.IdDocumento)

                    cmd.CommandText = query.ToString()
                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Update dei dati di archiviazione del documento.
        ''' </summary>
        ''' <param name="firmaDigitaleInfo"></param>
        ''' <param name="incrementaTentativi"></param>
        ''' <param name="deleteDocumentoFirmato"></param>
        ''' <returns></returns>
        ''' <remarks>SOLO CENTRALE</remarks>
        Public Function UpdateDatiArchiviazioneDocumento(firmaDigitaleInfo As Entities.ArchiviazioneDIRV.FirmaDigitaleInfo, incrementaTentativi As Boolean, deleteDocumentoFirmato As Boolean) As Integer Implements IFirmaDigitaleProvider.UpdateDatiArchiviazioneDocumento

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection
                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim query As New System.Text.StringBuilder("UPDATE T_DOCUMENTI_FIRMA SET ")

                    If firmaDigitaleInfo.DataArchiviazione.HasValue Then
                        query.Append(" DOC_DATA_ARCHIVIAZIONE = :DOC_DATA_ARCHIVIAZIONE, ")
                        cmd.Parameters.AddWithValue("DOC_DATA_ARCHIVIAZIONE", firmaDigitaleInfo.DataArchiviazione.Value)
                    End If

                    If firmaDigitaleInfo.IdUtenteArchiviazione.HasValue Then
                        query.Append(" DOC_UTE_ID_ARCHIVIAZIONE = :DOC_UTE_ID_ARCHIVIAZIONE, ")
                        cmd.Parameters.AddWithValue("DOC_UTE_ID_ARCHIVIAZIONE", firmaDigitaleInfo.IdUtenteArchiviazione.Value)
                    End If

                    If Not String.IsNullOrWhiteSpace(firmaDigitaleInfo.TokenArchiviazione) Then
                        query.Append(" DOC_TOKEN_ARCHIVIAZIONE = :DOC_TOKEN_ARCHIVIAZIONE, ")
                        cmd.Parameters.AddWithValue("DOC_TOKEN_ARCHIVIAZIONE", firmaDigitaleInfo.TokenArchiviazione)
                    End If

                    If String.IsNullOrWhiteSpace(firmaDigitaleInfo.CodiceErroreDIRV) Then
                        query.Append(" DOC_CODICE_ERRORE_DIRV = null, ")
                    Else
                        query.Append(" DOC_CODICE_ERRORE_DIRV = :DOC_CODICE_ERRORE_DIRV, ")
                        cmd.Parameters.AddWithValue("DOC_CODICE_ERRORE_DIRV", firmaDigitaleInfo.CodiceErroreDIRV)
                    End If

                    If String.IsNullOrWhiteSpace(firmaDigitaleInfo.DescrizioneErroreDIRV) Then
                        query.Append(" DOC_DESCRIZIONE_ERRORE_DIRV = null, ")
                    Else
                        query.Append(" DOC_DESCRIZIONE_ERRORE_DIRV = :DOC_DESCRIZIONE_ERRORE_DIRV, ")
                        cmd.Parameters.AddWithValue("DOC_DESCRIZIONE_ERRORE_DIRV", firmaDigitaleInfo.DescrizioneErroreDIRV)
                    End If

                    If incrementaTentativi Then
                        query.Append(" DOC_N_TENTATIVI = DOC_N_TENTATIVI + 1, ")
                    End If

                    If deleteDocumentoFirmato Then
                        query.Append(" DOC_FILE = null, ")
                    End If

                    query.Remove(query.Length - 2, 2)

                    query.Append(" where DOC_ID = :DOC_ID ")
                    cmd.Parameters.AddWithValue("DOC_ID", firmaDigitaleInfo.IdDocumento)

                    cmd.CommandText = query.ToString()
                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

#Region " Gestione documentale "

        Public Function GetDocumentiVisite(param As ParametriGetDocumentiVisite) As List(Of ArchiviazioneDIRV.DocumentoVisita) Implements IFirmaDigitaleProvider.GetDocumentiVisite

            If param Is Nothing Then Throw New ArgumentNullException()

            Dim lstDocumenti As List(Of ArchiviazioneDIRV.DocumentoVisita) = Nothing

            Using cmd As OracleCommand = Me.Connection.CreateCommand()

                Dim query As String = Queries.FirmaDigitale.OracleQueries.selectDocumentiVisite

                ' Impostazione filtri di ricerca
                Dim filtri As String = Me.SetFiltriDocumentiVisite(param.Filtri, cmd)

                cmd.CommandText = String.Format(query, filtri, param.OrderBy)

                'Paginazione
                If Not param.PagingOpts Is Nothing Then

                    cmd.AddPaginatedQuery(param.PagingOpts)

                End If

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim vis_id As Integer = idr.GetOrdinal("vis_id")
                            Dim paz_cognome As Integer = idr.GetOrdinal("paz_cognome")
                            Dim paz_nome As Integer = idr.GetOrdinal("paz_nome")
                            Dim paz_data_nascita As Integer = idr.GetOrdinal("paz_data_nascita")
                            Dim vis_data_visita As Integer = idr.GetOrdinal("vis_data_visita")
                            Dim vis_data_registrazione As Integer = idr.GetOrdinal("vis_data_registrazione")
                            Dim vis_usl_inserimento As Integer = idr.GetOrdinal("vis_usl_inserimento")
                            Dim ute_visita As Integer = idr.GetOrdinal("ute_visita")
                            Dim vis_ute_id_firma As Integer = idr.GetOrdinal("vis_ute_id_firma")
                            Dim ute_firma As Integer = idr.GetOrdinal("ute_firma")
                            Dim vis_data_firma As Integer = idr.GetOrdinal("vis_data_firma")
                            Dim vis_ute_id_archiviazione As Integer = idr.GetOrdinal("vis_ute_id_archiviazione")
                            Dim ute_arc As Integer = idr.GetOrdinal("ute_arc")
                            Dim vis_data_archiviazione As Integer = idr.GetOrdinal("vis_data_archiviazione")
                            Dim vis_doc_id_documento As Integer = idr.GetOrdinal("vis_doc_id_documento")
                            Dim ute_rilevatore As Integer = idr.GetOrdinal("ute_rilevatore")

                            lstDocumenti = New List(Of ArchiviazioneDIRV.DocumentoVisita)()

                            Dim doc As ArchiviazioneDIRV.DocumentoVisita = Nothing

                            While idr.Read()

                                doc = New ArchiviazioneDIRV.DocumentoVisita()

                                doc.IdVisita = idr.GetInt64OrDefault(vis_id)
                                doc.DataVisita = idr.GetDateTimeOrDefault(vis_data_visita)
                                doc.DataRegistrazione = idr.GetDateTimeOrDefault(vis_data_registrazione)
                                doc.CodiceAziendaInserimento = idr.GetStringOrDefault(vis_usl_inserimento)
                                doc.UtenteVisita = idr.GetStringOrDefault(ute_visita)
                                doc.IdUtenteFirma = idr.GetNullableInt64OrDefault(vis_ute_id_firma)
                                doc.UtenteFirma = idr.GetStringOrDefault(ute_firma)
                                doc.DataFirma = idr.GetNullableDateTimeOrDefault(vis_data_firma)
                                doc.IdUtenteArchiviazione = idr.GetNullableInt64OrDefault(vis_ute_id_archiviazione)
                                doc.UtenteArchiviazione = idr.GetStringOrDefault(ute_arc)
                                doc.DataArchiviazione = idr.GetNullableDateTimeOrDefault(vis_data_archiviazione)
                                doc.IdDocumento = idr.GetNullableInt64OrDefault(vis_doc_id_documento)

                                doc.UtenteRilevatore = idr.GetStringOrDefault(ute_rilevatore)

                                Dim paziente As String = String.Empty

                                If Not String.IsNullOrWhiteSpace(idr.GetStringOrDefault(paz_cognome)) Then
                                    paziente += String.Format("{0} ", idr.GetStringOrDefault(paz_cognome))
                                End If

                                If Not String.IsNullOrWhiteSpace(idr.GetStringOrDefault(paz_nome)) Then
                                    paziente += String.Format("{0} ", idr.GetStringOrDefault(paz_nome))
                                End If

                                If idr.GetNullableDateTimeOrDefault(paz_data_nascita) IsNot Nothing Then
                                    Dim dataNascita As DateTime = idr.GetDateTimeOrDefault(paz_data_nascita)
                                    paziente += String.Format("- {0} ", dataNascita.ToShortDateString())
                                End If
                                doc.PazienteInfo = paziente

                                lstDocumenti.Add(doc)

                            End While

                        End If

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return lstDocumenti

        End Function

        Public Function CountDocumentiVisite(filtriRicerca As ArchiviazioneDIRV.FiltriDocumentiVisite) As Integer Implements IFirmaDigitaleProvider.CountDocumentiVisite

            If filtriRicerca Is Nothing Then Throw New ArgumentNullException()

            Dim count As Integer = 0

            Using cmd As OracleCommand = Me.Connection.CreateCommand()

                ' Impostazione filtri di ricerca
                Dim filtri As String = Me.SetFiltriDocumentiVisite(filtriRicerca, cmd)

                cmd.CommandText = String.Format(OnVac.Queries.FirmaDigitale.OracleQueries.countDocumentiVisite, filtri)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        count = Convert.ToInt32(obj)
                    End If

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Restituisce gli id delle visite in base ai filtri specificati
        ''' </summary>
        ''' <param name="filtri"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetIdVisiteDocumenti(filtri As ArchiviazioneDIRV.FiltriDocumentiVisite) As List(Of Long) Implements IFirmaDigitaleProvider.GetIdVisiteDocumenti

            If filtri Is Nothing Then Throw New ArgumentNullException()

            Dim listIdVisite As New List(Of Long)()

            Using cmd As OracleCommand = Connection.CreateCommand()

                ' Impostazione filtri di ricerca
                cmd.CommandText =
                    String.Format(Queries.FirmaDigitale.OracleQueries.selectIdVisiteDocumenti, SetFiltriDocumentiVisite(filtri, cmd))

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim vis_id As Integer = idr.GetOrdinal("vis_id")

                            While idr.Read()
                                listIdVisite.Add(idr.GetInt64(vis_id))
                            End While

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listIdVisite

        End Function

#Region " Private methods "

        ''' <summary>
        ''' Restituisce una stringa creata in base a tutti i filtri specificati e aggiunge i parametri al command.
        ''' </summary>
        Private Function SetFiltriDocumentiVisite(filtriRicerca As ArchiviazioneDIRV.FiltriDocumentiVisite, cmd As OracleCommand) As String

            If filtriRicerca Is Nothing Then Return String.Empty

            Dim filtri As New System.Text.StringBuilder()

            'Filtri obbligatori
            filtri.Append(" where vis_mal_codice is not null and vis_n_bilancio is not null ")  ' anamnesi

            filtri.Append(" and (vis_data_visita >= :dataDa and vis_data_visita < :dataA) ")   ' periodo di visita
            cmd.Parameters.AddWithValue("dataDa", filtriRicerca.DataDa.Date)
            cmd.Parameters.AddWithValue("dataA", filtriRicerca.DataA.AddDays(1).Date)

            ' usl inserimento specificata
            filtri.Append(" and (")
            filtri.Append("  vis_usl_inserimento = :uslInserimento ")
            filtri.Append("  or exists (select 1 from t_ana_distretti where dis_codice = vis_usl_inserimento and dis_usl_codice = :uslInserimento)")
            filtri.Append(" ) ")

            cmd.Parameters.AddWithValue("uslInserimento", filtriRicerca.UslCorrente)

            'Filtro utente registrazione
            If filtriRicerca.IdUtenteRegistrazione.HasValue Then
                filtri.AppendFormat(" and vis_ute_id = :uteVisita ")
                cmd.Parameters.AddWithValue("uteVisita", filtriRicerca.IdUtenteRegistrazione.Value)
            End If
            'Filtro utente firma
            If filtriRicerca.IdUtenteFirma.HasValue Then
                filtri.AppendFormat(" and vis_ute_id_firma = :uteFirma")
                cmd.Parameters.AddWithValue("uteFirma", filtriRicerca.IdUtenteFirma.Value)
            End If

            'Filtro utente rilevatore
            If Not String.IsNullOrWhiteSpace(filtriRicerca.IdUtenteRilevatore) Then
                filtri.AppendFormat(" and vis_ope_codice_rilevatore = :uteRilevatore")
                cmd.Parameters.AddWithValue("uteRilevatore", filtriRicerca.IdUtenteRilevatore)
            End If

            'Filtro stato
            Select Case filtriRicerca.FiltroStato

                Case ArchiviazioneDIRV.FiltroStatoDocumento.DaFirmare
                    filtri.AppendFormat(" and vis_ute_id_firma is null ")

                Case ArchiviazioneDIRV.FiltroStatoDocumento.FirmatiNonArchiviati
                    filtri.AppendFormat(" and (vis_ute_id_firma is not null and vis_ute_id_archiviazione is null) ")

                Case ArchiviazioneDIRV.FiltroStatoDocumento.FirmatiArchiviati
                    filtri.AppendFormat(" and (vis_ute_id_firma is not null and vis_ute_id_archiviazione is not null) ")

                Case ArchiviazioneDIRV.FiltroStatoDocumento.Tutti
                Case Else

            End Select

            Return filtri.ToString()

        End Function

#End Region

#End Region

#End Region

    End Class

End Namespace