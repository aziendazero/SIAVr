Imports System.Collections.Generic
Imports System.Data.OracleClient
Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.Data.OracleClient

Namespace DAL.Oracle

    Public Class DbAttivitaProvider
        Inherits DbProvider
        Implements IAttivitaProvider

#Region " Constructors "

        Public Sub New(DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " IAttivitaProvider "

#Region " Anagrafe Tipi Attività "

        ''' <summary>
        ''' Restituisce i tipi attività in base ai filtri e con l'ordinamento impostato
        ''' </summary>
        ''' <param name="filtroGenerico"></param>
        ''' <param name="campoOrdinamento"></param>
        ''' <param name="versoOrdinamento"></param>
        ''' <param name="pagingOptions"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetElencoAttivitaTipo(filtroGenerico As String, soloValidi As Boolean, campoOrdinamento As Entities.AttivitaTipo.Ordinamento?, versoOrdinamento As String, pagingOptions As Onit.OnAssistnet.Data.PagingOptions) As IEnumerable(Of Entities.AttivitaTipo) Implements IAttivitaProvider.GetElencoAttivitaTipo

            Dim list As List(Of Entities.AttivitaTipo) = Nothing

            Using cmd As New OracleCommand()

                cmd.Connection = Me.Connection

                ' Query di select con eventuali filtri
                SetCommandSelectAttivitaTipo(cmd, filtroGenerico, soloValidi, False)

                ' Ordinamento
                If campoOrdinamento.HasValue Then

                    cmd.CommandText += String.Format(" ORDER BY {0} {1}", GetCampoAttivitaTipo(campoOrdinamento.Value), versoOrdinamento)

                End If

                ' Paginazione
                If Not pagingOptions Is Nothing Then

                    cmd.AddPaginatedQuery(pagingOptions)

                End If

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    list = GetListAttivitaTipo(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list.AsEnumerable()

        End Function

        ''' <summary>
        ''' Restituisce il numero di tipi attività presenti in anagrafe, in base al filtro impostati.
        ''' </summary>
        ''' <param name="filtroGenerico"></param>
        ''' <param name="soloValidi"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CountElencoAttivitaTipo(filtroGenerico As String, soloValidi As Boolean) As Integer Implements IAttivitaProvider.CountElencoAttivitaTipo

            Dim count As Integer = 0

            Using cmd As New OracleCommand()

                cmd.Connection = Me.Connection

                ' Query di select con eventuali filtri
                SetCommandSelectAttivitaTipo(cmd, filtroGenerico, soloValidi, True)

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
        ''' Restituisce il tipo attività in base al codice specificato
        ''' </summary>
        ''' <param name="codiceAttivitaTipo"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAttivitaTipo(codiceAttivitaTipo As String) As AttivitaTipo Implements IAttivitaProvider.GetAttivitaTipo

            Dim item As Entities.AttivitaTipo = Nothing

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection
                cmd.CommandText = GetQuerySelectAttivitaTipo(False) + " WHERE TAT_CODICE = :TAT_CODICE "

                cmd.Parameters.AddWithValue("TAT_CODICE", codiceAttivitaTipo)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim list As List(Of Entities.AttivitaTipo) = GetListAttivitaTipo(cmd)
                    If Not list.IsNullOrEmpty() Then
                        item = list.First()
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return item

        End Function

        ''' <summary>
        ''' Inserimento tipo attività
        ''' </summary>
        ''' <param name="attivitaTipo"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertAttivitaTipo(attivitaTipo As AttivitaTipo) As Integer Implements IAttivitaProvider.InsertAttivitaTipo

            Dim count As Integer = 0

            Using cmd As New OracleCommand("INSERT INTO T_ANA_TIPI_ATTIVITA (TAT_CODICE, TAT_DESCRIZIONE, TAT_OBSOLETO) VALUES (:TAT_CODICE, :TAT_DESCRIZIONE, :TAT_OBSOLETO)", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("TAT_CODICE", attivitaTipo.Codice)
                    cmd.Parameters.AddWithValue("TAT_DESCRIZIONE", attivitaTipo.Descrizione)
                    cmd.Parameters.AddWithValue("TAT_OBSOLETO", attivitaTipo.Obsoleto)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Update tipo attività
        ''' </summary>
        ''' <param name="attivitaTipo"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateAttivitaTipo(attivitaTipo As AttivitaTipo) As Integer Implements IAttivitaProvider.UpdateAttivitaTipo

            Dim count As Integer = 0

            Using cmd As New OracleCommand("UPDATE T_ANA_TIPI_ATTIVITA SET TAT_DESCRIZIONE = :TAT_DESCRIZIONE, TAT_OBSOLETO = :TAT_OBSOLETO WHERE TAT_CODICE = :TAT_CODICE", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("TAT_DESCRIZIONE", attivitaTipo.Descrizione)
                    cmd.Parameters.AddWithValue("TAT_OBSOLETO", attivitaTipo.Obsoleto)
                    cmd.Parameters.AddWithValue("TAT_CODICE", attivitaTipo.Codice)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

#Region " Private "

        Private Function GetQuerySelectAttivitaTipo(isCount As Boolean) As String

            Dim query As New System.Text.StringBuilder()

            If isCount Then
                query.Append("SELECT COUNT(TAT_CODICE) ")
            Else
                query.Append("SELECT TAT_CODICE, TAT_DESCRIZIONE, TAT_OBSOLETO ")
            End If

            query.Append(" FROM T_ANA_TIPI_ATTIVITA ")

            Return query.ToString()

        End Function

        Private Sub SetCommandSelectAttivitaTipo(cmd As OracleCommand, filtroGenerico As String, soloValidi As Boolean, isCount As Boolean)

            Dim filtro As New System.Text.StringBuilder()

            If soloValidi Then
                filtro.Append(" TAT_OBSOLETO = :TAT_OBSOLETO AND ")
                cmd.Parameters.AddWithValue("TAT_OBSOLETO", "N")
            End If

            If Not String.IsNullOrWhiteSpace(filtroGenerico) Then

                filtro.Append(" (UPPER(TAT_CODICE) LIKE UPPER(:TAT_CODICE) OR UPPER(TAT_DESCRIZIONE) LIKE UPPER(:TAT_DESCRIZIONE)) AND ")

                Dim filtroLike As String = String.Format("%{0}%", filtroGenerico)

                cmd.Parameters.AddWithValue("TAT_CODICE", filtroLike)
                cmd.Parameters.AddWithValue("TAT_DESCRIZIONE", filtroLike)

            End If

            If filtro.Length > 0 Then
                filtro.RemoveLast(4)
                filtro.Insert(0, " WHERE ")
            End If

            cmd.CommandText = GetQuerySelectAttivitaTipo(isCount) + filtro.ToString()

        End Sub

        Private Function GetCampoAttivitaTipo(campo As Entities.AttivitaTipo.Ordinamento) As String

            Select Case campo

                Case Entities.AttivitaTipo.Ordinamento.Codice
                    Return "TAT_CODICE"

                Case Entities.AttivitaTipo.Ordinamento.Descrizione
                    Return "TAT_DESCRIZIONE"

                Case Entities.AttivitaTipo.Ordinamento.Obsoleto
                    Return "TAT_OBSOLETO"

                Case Else
                    Throw New NotImplementedException("Entities.AttivitaTipo.Ordinamento: valore non previsto")

            End Select

            Return String.Empty

        End Function

        Private Function GetListAttivitaTipo(cmd As OracleCommand) As List(Of Entities.AttivitaTipo)

            Dim list As New List(Of Entities.AttivitaTipo)()

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    Dim tat_codice As Integer = idr.GetOrdinal("TAT_CODICE")
                    Dim tat_descrizione As Integer = idr.GetOrdinal("TAT_DESCRIZIONE")
                    Dim tat_obsoleto As Integer = idr.GetOrdinal("TAT_OBSOLETO")

                    Dim item As Entities.AttivitaTipo = Nothing

                    While idr.Read()

                        item = New Entities.AttivitaTipo()

                        item.Codice = idr.GetString(tat_codice)
                        item.Descrizione = idr.GetString(tat_descrizione)
                        item.Obsoleto = idr.GetString(tat_obsoleto)

                        list.Add(item)

                    End While

                End If

            End Using

            Return list

        End Function

#End Region

#End Region

#Region " Anagrafe Attività "

        ''' <summary>
        ''' Restituisce le attività presenti in anagrafe, in base al filtro impostati.
        ''' </summary>
        ''' <param name="filtroGenerico"></param>
        ''' <param name="soloValidi"></param>
        ''' <param name="campoOrdinamento"></param>
        ''' <param name="versoOrdinamento"></param>
        ''' <param name="pagingOptions"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetElencoAttivitaAnagrafe(filtroGenerico As String, soloValidi As Boolean, campoOrdinamento As Entities.AttivitaAnagrafe.Ordinamento?, versoOrdinamento As String, pagingOptions As Onit.OnAssistnet.Data.PagingOptions, idUtenteConnesso As Integer, appId As String) As IEnumerable(Of Entities.AttivitaAnagrafe) Implements IAttivitaProvider.GetElencoAttivitaAnagrafe

            Dim list As List(Of Entities.AttivitaAnagrafe) = Nothing

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                ' Query di select con eventuali filtri
                SetCommandSelectAttivitaAnagrafe(cmd, filtroGenerico, soloValidi, False, idUtenteConnesso, appId)

                ' Ordinamento
                If campoOrdinamento.HasValue Then

                    cmd.CommandText += String.Format(" ORDER BY {0} {1}", GetCampoAttivitaAnagrafe(campoOrdinamento.Value), versoOrdinamento)

                End If

                ' Paginazione
                If Not pagingOptions Is Nothing Then

                    cmd.AddPaginatedQuery(pagingOptions)

                End If

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    list = GetListAttivitaAnagrafe(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list.AsEnumerable()

        End Function

        ''' <summary>
        ''' Restituisce il numero di attività presenti in anagrafe, in base ai filtri impostati.
        ''' </summary>
        ''' <param name="filtroGenerico"></param>
        ''' <param name="soloValidi"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CountElencoAttivitaAnagrafe(filtroGenerico As String, soloValidi As Boolean, idUtenteConnesso As Integer, appId As String) As Integer Implements IAttivitaProvider.CountElencoAttivitaAnagrafe

            Dim count As Integer = 0

            Using cmd As New OracleCommand()

                cmd.Connection = Me.Connection

                ' Query di select con eventuali filtri
                SetCommandSelectAttivitaAnagrafe(cmd, filtroGenerico, soloValidi, True, idUtenteConnesso, appId)

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
        ''' Restituisce l'anagrafica dell'attività specificata
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAttivitaAnagrafe(id As Integer) As Entities.AttivitaAnagrafe Implements IAttivitaProvider.GetAttivitaAnagrafe

            Dim item As Entities.AttivitaAnagrafe = Nothing

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection
                cmd.CommandText = GetQuerySelectAttivitaAnagrafe(False) + " WHERE ATA_ID = :ID "

                cmd.Parameters.AddWithValue("ID", id)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim list As List(Of Entities.AttivitaAnagrafe) = GetListAttivitaAnagrafe(cmd)
                    If Not list.IsNullOrEmpty() Then
                        item = list.First()
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return item

        End Function

        ''' <summary>
        ''' Restituisce le attività in base al codice
        ''' </summary>
        ''' <param name="codiceAttivita"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAttivitaAnagrafeByCodice(codiceAttivita As String) As IEnumerable(Of Entities.AttivitaAnagrafe) Implements IAttivitaProvider.GetAttivitaAnagrafeByCodice

            Dim list As New List(Of Entities.AttivitaAnagrafe)()

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection
                cmd.CommandText = GetQuerySelectAttivitaAnagrafe(False) + " WHERE ATA_CODICE = :ATA_CODICE "

                cmd.Parameters.AddWithValue("ATA_CODICE", codiceAttivita)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    list = GetListAttivitaAnagrafe(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list.AsEnumerable()

        End Function

        ''' <summary>
        ''' Restituisce il valore della sequence per l'id dell'anagrafica attività da inserire
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetNextIdAttivitaAnagrafe() As Integer Implements IAttivitaProvider.GetNextIdAttivitaAnagrafe

            Dim nextVal As Integer = 0

            Using cmd As New OracleCommand("SELECT SEQ_ANA_ATTIVITA.NEXTVAL FROM DUAL", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then
                        nextVal = Convert.ToInt32(obj)
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return nextVal

        End Function

        ''' <summary>
        ''' Inserimento anagrafica attività
        ''' </summary>
        ''' <param name="attivitaAnagrafe"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertAttivitaAnagrafe(attivitaAnagrafe As Entities.AttivitaAnagrafe) As Integer Implements IAttivitaProvider.InsertAttivitaAnagrafe

            Dim count As Integer = 0

            Using cmd As New OracleCommand("INSERT INTO T_ANA_ATTIVITA (ATA_ID, ATA_CODICE, ATA_DESCRIZIONE, ATA_OBSOLETO, ATA_SCUOLA) VALUES (:ATA_ID, :ATA_CODICE, :ATA_DESCRIZIONE, :ATA_OBSOLETO, :ATA_SCUOLA)", Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("ATA_ID", attivitaAnagrafe.Id)
                    cmd.Parameters.AddWithValue("ATA_CODICE", attivitaAnagrafe.Codice)
                    cmd.Parameters.AddWithValue("ATA_DESCRIZIONE", attivitaAnagrafe.Descrizione)
                    cmd.Parameters.AddWithValue("ATA_OBSOLETO", attivitaAnagrafe.Obsoleto)
                    cmd.Parameters.AddWithValue("ATA_SCUOLA", attivitaAnagrafe.Scuola)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Modifica anagrafica attività
        ''' </summary>
        ''' <param name="attivitaAnagrafe"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateAttivitaAnagrafe(attivitaAnagrafe As Entities.AttivitaAnagrafe) As Integer Implements IAttivitaProvider.UpdateAttivitaAnagrafe

            Dim count As Integer = 0

            Using cmd As New OracleCommand("UPDATE T_ANA_ATTIVITA SET ATA_CODICE = :ATA_CODICE, ATA_DESCRIZIONE = :ATA_DESCRIZIONE, ATA_OBSOLETO = :ATA_OBSOLETO, ATA_SCUOLA = :ATA_SCUOLA WHERE ATA_ID = :ATA_ID", Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("ATA_CODICE", attivitaAnagrafe.Codice)
                    cmd.Parameters.AddWithValue("ATA_DESCRIZIONE", attivitaAnagrafe.Descrizione)
                    cmd.Parameters.AddWithValue("ATA_OBSOLETO", attivitaAnagrafe.Obsoleto)
                    cmd.Parameters.AddWithValue("ATA_SCUOLA", attivitaAnagrafe.Scuola)
                    cmd.Parameters.AddWithValue("ATA_ID", attivitaAnagrafe.Id)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

#Region " Private "

        Private Function GetQuerySelectAttivitaAnagrafe(isCount As Boolean) As String

            Dim query As New System.Text.StringBuilder()

            If isCount Then
                query.Append("SELECT COUNT(ATA_ID) ")
            Else
                query.Append("SELECT ATA_ID, ATA_CODICE, ATA_DESCRIZIONE, ATA_OBSOLETO, ATA_SCUOLA ")
            End If

            query.Append(" FROM T_ANA_ATTIVITA ")

            Return query.ToString()

        End Function

        Private Sub SetCommandSelectAttivitaAnagrafe(cmd As OracleCommand, filtroGenerico As String, soloValidi As Boolean, isCount As Boolean, idutenteConnesso As Integer, appId As String)

            Dim filtro As New System.Text.StringBuilder()

            Dim query As New System.Text.StringBuilder()

            If soloValidi Then
                filtro.Append(" ATA_OBSOLETO = :ATA_OBSOLETO AND ")
                cmd.Parameters.AddWithValue("ATA_OBSOLETO", "N")
            End If

            If Not String.IsNullOrWhiteSpace(filtroGenerico) Then

                filtro.Append(" (UPPER(ATA_CODICE) LIKE UPPER(:ATA_CODICE) OR UPPER(ATA_DESCRIZIONE) LIKE UPPER(:ATA_DESCRIZIONE)) AND ")

                Dim filtroLike As String = String.Format("%{0}%", filtroGenerico)

                cmd.Parameters.AddWithValue("ATA_CODICE", filtroLike)
                cmd.Parameters.AddWithValue("ATA_DESCRIZIONE", filtroLike)

            End If

            If idutenteConnesso > 0 Then
                query.Append("JOIN T_ANA_ATTIVITA_UTENTI ON AAU_ATA_ID=ATA_ID AND AAU_UTE_ID = :AAU_UTE_ID AND AAU_APP_ID =:AAU_APP_ID")
                cmd.Parameters.AddWithValue("AAU_UTE_ID", idutenteConnesso)
                cmd.Parameters.AddWithValue("AAU_APP_ID", appId)
            End If

            If filtro.Length > 0 Then
                filtro.RemoveLast(4)
                filtro.Insert(0, " WHERE ")
            End If

            cmd.CommandText = GetQuerySelectAttivitaAnagrafe(isCount) + query.ToString() + filtro.ToString()

        End Sub

        Private Function GetCampoAttivitaAnagrafe(campo As Entities.AttivitaAnagrafe.Ordinamento) As String

            Select Case campo

                Case AttivitaAnagrafe.Ordinamento.Id
                    Return "ATA_ID"

                Case AttivitaAnagrafe.Ordinamento.Codice
                    Return "ATA_CODICE"

                Case Entities.AttivitaAnagrafe.Ordinamento.Descrizione
                    Return "ATA_DESCRIZIONE"

                Case Entities.AttivitaAnagrafe.Ordinamento.Obsoleto
                    Return "ATA_OBSOLETO"

                Case Entities.AttivitaAnagrafe.Ordinamento.Scuola
                    Return "ATA_SCUOLA"

                Case Else
                    Throw New NotImplementedException("Entities.AttivitaAnagrafe.Ordinamento: valore non previsto")

            End Select

            Return String.Empty

        End Function

        Private Function GetListAttivitaAnagrafe(cmd As OracleCommand) As List(Of Entities.AttivitaAnagrafe)

            Dim list As New List(Of Entities.AttivitaAnagrafe)()

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    Dim ata_id As Integer = idr.GetOrdinal("ATA_ID")
                    Dim ata_codice As Integer = idr.GetOrdinal("ATA_CODICE")
                    Dim ata_descrizione As Integer = idr.GetOrdinal("ATA_DESCRIZIONE")
                    Dim ata_obsoleto As Integer = idr.GetOrdinal("ATA_OBSOLETO")
                    Dim ata_scuola As Integer = idr.GetOrdinal("ATA_SCUOLA")

                    Dim item As Entities.AttivitaAnagrafe = Nothing

                    While idr.Read()

                        item = New Entities.AttivitaAnagrafe()

                        item.Id = idr.GetInt32(ata_id)
                        item.Codice = idr.GetStringOrDefault(ata_codice)
                        item.Descrizione = idr.GetStringOrDefault(ata_descrizione)
                        item.Obsoleto = idr.GetStringOrDefault(ata_obsoleto)
                        item.Scuola = idr.GetStringOrDefault(ata_scuola)

                        list.Add(item)

                    End While

                End If

            End Using

            Return list

        End Function

#End Region

#End Region

#Region " Anagrafe Variabili "

        ''' <summary>
        ''' Restituisce le osservazioni in anagrafica, aventi flag_osservazione di tipo attività (= "A"), in base ai filtri impostati.
        ''' Non vengono restituite le variabili già associate all'id, se valorizzato.
        ''' Se specificato, esclude le obsolete.
        ''' </summary>
        ''' <param name="idAttivita"></param>
        ''' <param name="filtroGenerico"></param>
        ''' <param name="soloValidi"></param>
        ''' <param name="campoOrdinamento"></param>
        ''' <param name="versoOrdinamento"></param>
        ''' <param name="pagingOptions"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetElencoVariabiliDaAssociare(idAttivita As Integer?, filtroGenerico As String, soloValidi As Boolean, campoOrdinamento As Entities.AttivitaVariabileDaAssociare.Ordinamento?, versoOrdinamento As String, pagingOptions As Onit.OnAssistnet.Data.PagingOptions) As IEnumerable(Of Entities.AttivitaVariabileDaAssociare) Implements IAttivitaProvider.GetElencoVariabiliDaAssociare

            Dim list As New List(Of Entities.AttivitaVariabileDaAssociare)()

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                SetCommandSelectVariabiliDaAssociare(cmd, idAttivita, filtroGenerico, soloValidi, False)

                ' Ordinamento
                If campoOrdinamento.HasValue Then

                    cmd.CommandText += String.Format(" ORDER BY {0} {1}", GetCampoVariabileDaAssociare(campoOrdinamento.Value), versoOrdinamento)

                End If

                ' Paginazione
                If Not pagingOptions Is Nothing Then

                    cmd.AddPaginatedQuery(pagingOptions)

                End If

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim oss_codice As Integer = idr.GetOrdinal("OSS_CODICE")
                            Dim oss_descrizione As Integer = idr.GetOrdinal("OSS_DESCRIZIONE")
                            Dim oss_tipo_risposta As Integer = idr.GetOrdinal("OSS_TIPO_RISPOSTA")
                            Dim oss_sesso As Integer = idr.GetOrdinal("OSS_SESSO")

                            Dim item As Entities.AttivitaVariabileDaAssociare = Nothing

                            While idr.Read()

                                item = New Entities.AttivitaVariabileDaAssociare()

                                item.Codice = idr.GetString(oss_codice)
                                item.Descrizione = idr.GetString(oss_descrizione)
                                item.TipoRisposta = idr.GetStringOrDefault(oss_tipo_risposta)
                                item.Sesso = idr.GetStringOrDefault(oss_sesso)

                                list.Add(item)

                            End While

                        End If

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list.AsEnumerable()

        End Function
        ''' <summary>
        ''' Elenco degli utenti da associare all'attivita'
        ''' </summary>
        ''' <param name="idAttivita"></param>
        ''' <param name="filtroGenerico"></param>
        ''' <param name="soloValidi"></param>
        ''' <param name="campoOrdinamento"></param>
        ''' <param name="versoOrdinamento"></param>
        ''' <param name="pagingOptions"></param>
        ''' <returns></returns>
        Public Function GetElencoUtentiDaAssociare(idAttivita As Integer?, filtroGenerico As String, soloValidi As Boolean, campoOrdinamento As Entities.AttivitaUtentiDaAssociare.Ordinamento?, versoOrdinamento As String, pagingOptions As Onit.OnAssistnet.Data.PagingOptions) As IEnumerable(Of Entities.AttivitaUtentiDaAssociare) Implements IAttivitaProvider.GetElencoUtentiDaAssociare

            Dim list As New List(Of Entities.AttivitaUtentiDaAssociare)()

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                SetCommandSelectUtentiDaAssociare(cmd, idAttivita, filtroGenerico, soloValidi, False)

                ' Ordinamento
                If campoOrdinamento.HasValue Then

                    cmd.CommandText += String.Format(" ORDER BY {0} {1}", GetCampoUtentiDaAssociare(campoOrdinamento.Value), versoOrdinamento)

                End If

                ' Paginazione
                If Not pagingOptions Is Nothing Then

                    cmd.AddPaginatedQuery(pagingOptions)

                End If

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim ute_id As Integer = idr.GetOrdinal("UTE_ID")
                            Dim ute_codice As Integer = idr.GetOrdinal("UTE_CODICE")
                            Dim ute_descrizione As Integer = idr.GetOrdinal("UTE_DESCRIZIONE")
                            Dim ute_cognome As Integer = idr.GetOrdinal("UTE_COGNOME")
                            Dim ute_nome As Integer = idr.GetOrdinal("UTE_NOME")
                            Dim ute_appId As Integer = idr.GetOrdinal("UTE_APP_ID")
                            Dim ugs_usl_codice As Integer = idr.GetOrdinal("UGS_USL_CODICE")

                            Dim item As Entities.AttivitaUtentiDaAssociare = Nothing

                            While idr.Read()

                                item = New Entities.AttivitaUtentiDaAssociare()

                                item.IdUtente = idr.GetInt32(ute_id)
                                item.Codice = idr.GetString(ute_codice)
                                item.Descrizione = idr.GetStringOrDefault(ute_descrizione)
                                item.Cognome = idr.GetStringOrDefault(ute_cognome)
                                item.Nome = idr.GetStringOrDefault(ute_nome)
                                item.AppId = idr.GetStringOrDefault(ute_appId)
                                item.UslCodice = idr.GetStringOrDefault(ugs_usl_codice)

                                list.Add(item)

                            End While

                        End If

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list.AsEnumerable()

        End Function

        ''' <summary>
        ''' Restituisce il numero di attività presenti in anagrafe, in base ai filtri impostati.
        ''' </summary>
        ''' <param name="idAttivita"></param>
        ''' <param name="filtroGenerico"></param>
        ''' <param name="soloValidi"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CountElencoVariabiliDaAssociare(idAttivita As Integer?, filtroGenerico As String, soloValidi As Boolean) As Integer Implements IAttivitaProvider.CountElencoVariabiliDaAssociare

            Dim count As Integer = 0

            Using cmd As New OracleCommand()

                cmd.Connection = Me.Connection

                ' Query di select con eventuali filtri
                SetCommandSelectVariabiliDaAssociare(cmd, idAttivita, filtroGenerico, soloValidi, True)

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
        ''' Restituisce l'elenco delle variabili associate all'attività specificata
        ''' </summary>
        ''' <param name="idAttivita"></param>
        ''' <param name="filtroGenerico"></param>
        ''' <param name="campoOrdinamento"></param>
        ''' <param name="versoOrdinamento"></param>
        ''' <param name="pagingOptions"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetElencoVariabiliAssociate(idAttivita As Integer, filtroGenerico As String, campoOrdinamento As Entities.AttivitaVariabileAssociata.Ordinamento?, versoOrdinamento As String, pagingOptions As Onit.OnAssistnet.Data.PagingOptions) As IEnumerable(Of Entities.AttivitaVariabileAssociata) Implements IAttivitaProvider.GetElencoVariabiliAssociate

            Dim list As List(Of Entities.AttivitaVariabileAssociata) = Nothing

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                SetCommandSelectVariabiliAssociate(cmd, idAttivita, filtroGenerico, False)

                ' Paginazione
                If Not pagingOptions Is Nothing Then

                    cmd.AddPaginatedQuery(pagingOptions)

                End If

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    list = GetListAttivitaVariabili(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list.AsEnumerable()

        End Function
        ''' <summary>
        ''' Restituisce l'elenco degli utenti associati all'attività specificata
        ''' </summary>
        ''' <param name="idAttivita"></param>
        ''' <param name="filtroGenerico"></param>
        ''' <param name="campoOrdinamento"></param>
        ''' <param name="versoOrdinamento"></param>
        ''' <param name="pagingOptions"></param>
        ''' <returns></returns>
        Public Function GetElencoUtentiAssociati(idAttivita As Integer, filtroGenerico As String, campoOrdinamento As Entities.AttivitaUtenteAssociato.Ordinamento?, versoOrdinamento As String, pagingOptions As Onit.OnAssistnet.Data.PagingOptions) As IEnumerable(Of Entities.AttivitaUtenteAssociato) Implements IAttivitaProvider.GetElencoUtentiAssociati

            Dim list As List(Of Entities.AttivitaUtenteAssociato) = Nothing

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                SetCommandSelectUtentiAssociati(cmd, idAttivita, filtroGenerico, False)

                ' Paginazione
                If Not pagingOptions Is Nothing Then

                    cmd.AddPaginatedQuery(pagingOptions)

                End If

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    list = GetListAttivitaUtentiAssociati(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list.AsEnumerable()

        End Function

        ''' <summary>
        ''' Restituisce il numero di variabili associate all'attività specificata.
        ''' </summary>
        ''' <param name="idAttivita"></param>
        ''' <param name="filtroGenerico"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CountElencoVariabiliAssociate(idAttivita As Integer, filtroGenerico As String) As Integer Implements IAttivitaProvider.CountElencoVariabiliAssociate

            Dim count As Integer = 0

            Using cmd As New OracleCommand()

                cmd.Connection = Me.Connection

                ' Query di select con eventuali filtri
                SetCommandSelectVariabiliAssociate(cmd, idAttivita, filtroGenerico, True)

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
        ''' Restituisce il numero d'ordine più alto per le variabili dell'attività specificata
        ''' </summary>
        ''' <param name="idAttivita"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetMaxOrdineVariabiliAssociate(idAttivita As Integer) As Integer Implements IAttivitaProvider.GetMaxOrdineVariabiliAssociate

            Dim max As Integer = 0

            Using cmd As New OracleCommand("SELECT MAX(AAV_ORDINE) FROM T_ANA_ATTIVITA_VARIABILI WHERE AAV_ATA_ID = :AAV_ATA_ID", Me.Connection)

                cmd.Parameters.AddWithValue("AAV_ATA_ID", idAttivita)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        max = Convert.ToInt32(obj)
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return max

        End Function

        ''' <summary>
        ''' Restituisce il valore della sequence per l'id della variabile che si vuole associare
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetNextIdVariabileAssociata() As Integer Implements IAttivitaProvider.GetNextIdVariabileAssociata

            Dim nextVal As Integer = 0

            Using cmd As New OracleCommand("SELECT SEQ_ANA_ATTIVITA_VARIABILI.NEXTVAL FROM DUAL", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then
                        nextVal = Convert.ToInt32(obj)
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return nextVal

        End Function
        ''' <summary>
        '''  Restituisce il valore della sequence per l'id della associazione dell'utente
        ''' </summary>
        ''' <returns></returns>
        Public Function GetNextIdUtenteAssociato() As Integer Implements IAttivitaProvider.GetNextIdUtenteAssociato

            Dim nextVal As Integer = 0

            Using cmd As New OracleCommand("SELECT SEQ_ATTIVITA_UTENTI.NEXTVAL FROM DUAL", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then
                        nextVal = Convert.ToInt32(obj)
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return nextVal

        End Function
        ''' <summary>
        ''' Inserimento associazione variabile - attività
        ''' </summary>
        ''' <param name="variabileAssociata"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertVariabileAssociata(variabileAssociata As Entities.AttivitaVariabileAssociata) As Integer Implements IAttivitaProvider.InsertVariabileAssociata

            Dim count As Integer = 0

            Using cmd As New OracleCommand("INSERT INTO T_ANA_ATTIVITA_VARIABILI (AAV_ID, AAV_ATA_ID, AAV_OSS_CODICE, AAV_ORDINE) VALUES (:AAV_ID, :AAV_ATA_ID, :AAV_OSS_CODICE, :AAV_ORDINE)", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("AAV_ID", variabileAssociata.IdVariabile)
                    cmd.Parameters.AddWithValue("AAV_ATA_ID", variabileAssociata.IdAttivita)
                    cmd.Parameters.AddWithValue("AAV_OSS_CODICE", variabileAssociata.CodiceVariabile)
                    cmd.Parameters.AddWithValue("AAV_ORDINE", variabileAssociata.Ordine)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function
        ''' <summary>
        ''' Insert 
        ''' </summary>
        ''' <param name="utenteAssociato"></param>
        ''' <returns></returns>
        Public Function InsertUtenteAssociato(utenteAssociato As Entities.AttivitaUtenteAssociato) As Integer Implements IAttivitaProvider.InsertUtenteAssociato

            Dim count As Integer = 0

            Using cmd As New OracleCommand("INSERT INTO T_ANA_ATTIVITA_UTENTI (AAU_ID, AAU_ATA_ID, AAU_UTE_ID, AAU_APP_ID) VALUES (:AAU_ID, :AAU_ATA_ID, :AAU_UTE_ID, :AAU_APP_ID)", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("AAU_ID", utenteAssociato.Id)
                    cmd.Parameters.AddWithValue("AAU_ATA_ID", utenteAssociato.IdAttivita)
                    cmd.Parameters.AddWithValue("AAU_UTE_ID", utenteAssociato.IdUtenti)
                    cmd.Parameters.AddWithValue("AAU_APP_ID", utenteAssociato.AppId)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Modifica campo ordine dell'associazione variabile - attività
        ''' </summary>
        ''' <param name="idAttivitaVariabile"></param>
        ''' <param name="ordine"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateOrdineVariabileAssociata(idAttivitaVariabile As Integer, ordine As Integer) As Integer Implements IAttivitaProvider.UpdateOrdineVariabileAssociata

            Dim count As Integer = 0

            Using cmd As New OracleCommand("UPDATE T_ANA_ATTIVITA_VARIABILI SET AAV_ORDINE = :AAV_ORDINE WHERE AAV_ID = :AAV_ID", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("AAV_ORDINE", ordine)
                    cmd.Parameters.AddWithValue("AAV_ID", idAttivitaVariabile)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function
        Public Function UpdateObbligatorioVariabileAssociata(idAttivitaVariabile As Integer, obbligatorio As String) As Integer Implements IAttivitaProvider.UpdateObbligatorioVariabileAssociata

            Dim count As Integer = 0

            Using cmd As New OracleCommand("UPDATE T_ANA_ATTIVITA_VARIABILI SET AAV_OBBLIGATORIO = :AAV_OBBLIGATORIO WHERE AAV_ID = :AAV_ID", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("AAV_OBBLIGATORIO", obbligatorio)
                    cmd.Parameters.AddWithValue("AAV_ID", idAttivitaVariabile)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Eliminazione associazione variabile - attività
        ''' </summary>
        ''' <param name="idAttivitaVariabile"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteVariabileAssociata(idAttivitaVariabile As Integer) As Integer Implements IAttivitaProvider.DeleteVariabileAssociata

            Dim count As Integer = 0

            Using cmd As New OracleCommand("DELETE FROM T_ANA_ATTIVITA_VARIABILI WHERE AAV_ID = :AAV_ID", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("AAV_ID", idAttivitaVariabile)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function
        ''' <summary>
        ''' Delete associazione utente attivita
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Public Function DeleteUtenteAssociato(id As Integer) As Integer Implements IAttivitaProvider.DeleteUtenteAssociato

            Dim count As Integer = 0

            Using cmd As New OracleCommand("DELETE FROM T_ANA_ATTIVITA_UTENTI WHERE AAU_ID = :AAU_ID", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("AAU_ID", id)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

#Region " Private "

        Private Function GetQuerySelectVariabiliDaAssociare(isCount As Boolean) As String

            Dim query As New System.Text.StringBuilder()

            If isCount Then
                query.Append("SELECT COUNT(OSS_CODICE) ")
            Else
                query.Append("SELECT OSS_CODICE, OSS_DESCRIZIONE, OSS_TIPO_RISPOSTA, OSS_SESSO ")
            End If

            query.Append(" FROM T_ANA_OSSERVAZIONI ")
            query.Append(" WHERE OSS_FLAG_TIPOLOGIA = 'A' ")

            Return query.ToString()

        End Function

        Private Function GetQuerySelectUtentiDaAssociare(isCount As Boolean) As String

            Dim query As New System.Text.StringBuilder()

            If isCount Then
                query.Append("SELECT COUNT(UTE_CODICE) ")
            Else
                query.Append("SELECT UTE_ID, UTE_CODICE, UTE_DESCRIZIONE, UTE_COGNOME, UTE_NOME, UTE_APP_ID,UGS_USL_CODICE ")
            End If

            query.Append(" FROM V_ANA_UTENTI ")
            query.Append(" JOIN T_USL_GESTITE ON UGS_APP_ID= UTE_APP_ID")
            'query.Append(" WHERE UTE_OBSOLETO = 'V'")

            Return query.ToString()

        End Function

        Private Sub SetCommandSelectVariabiliDaAssociare(cmd As OracleCommand, idAttivita As Integer?, filtroGenerico As String, soloValidi As Boolean, isCount As Boolean)

            Dim query As New System.Text.StringBuilder()

            query.Append(GetQuerySelectVariabiliDaAssociare(isCount))

            ' Non carica le variabili già associate
            If idAttivita.HasValue Then
                query.Append(" AND NOT EXISTS (SELECT 1 FROM T_ANA_ATTIVITA_VARIABILI WHERE OSS_CODICE = AAV_OSS_CODICE AND AAV_ATA_ID = :AAV_ATA_ID) ")
                cmd.Parameters.AddWithValue("AAV_ATA_ID", idAttivita.Value)
            End If

            ' Non carica le variabili obsolete
            If soloValidi Then
                query.Append(" AND (OSS_OBSOLETO IS NULL OR OSS_OBSOLETO = :ATA_OBSOLETO) ")
                cmd.Parameters.AddWithValue("ATA_OBSOLETO", "N")
            End If

            If Not String.IsNullOrWhiteSpace(filtroGenerico) Then

                query.Append(" AND (UPPER(OSS_CODICE) LIKE UPPER(:OSS_CODICE) OR UPPER(OSS_DESCRIZIONE) LIKE UPPER(:OSS_DESCRIZIONE)) ")

                Dim filtroLike As String = String.Format("%{0}%", filtroGenerico)

                cmd.Parameters.AddWithValue("OSS_CODICE", filtroLike)
                cmd.Parameters.AddWithValue("OSS_DESCRIZIONE", filtroLike)

            End If

            cmd.CommandText = query.ToString()

        End Sub

        Private Sub SetCommandSelectUtentiDaAssociare(cmd As OracleCommand, idAttivita As Integer?, filtroGenerico As String, soloValidi As Boolean, isCount As Boolean)

            Dim query As New System.Text.StringBuilder()

            query.Append(GetQuerySelectUtentiDaAssociare(isCount))

            ' Non carica le variabili già associate
            If idAttivita.HasValue Then
                query.Append(" AND NOT EXISTS (SELECT 1 FROM T_ANA_ATTIVITA_UTENTI WHERE UTE_ID = AAU_UTE_ID AND UTE_APP_ID = AAU_APP_ID AND AAU_ATA_ID = :AAU_ATA_ID) ")
                cmd.Parameters.AddWithValue("AAU_ATA_ID", idAttivita.Value)
            End If

            ' Non carica le variabili obsolete
            If soloValidi Then
                query.Append(" AND (UTE_OBSOLETO IS NULL OR UTE_OBSOLETO = :UTE_OBSOLETO) ")
                cmd.Parameters.AddWithValue("UTE_OBSOLETO", "N")
            End If

            If Not String.IsNullOrWhiteSpace(filtroGenerico) Then

                query.Append(" AND (UPPER(UTE_CODICE) LIKE UPPER(:UTE_CODICE) OR UPPER(UTE_DESCRIZIONE) LIKE UPPER(:UTE_DESCRIZIONE) OR UPPER(UTE_APP_ID) LIKE UPPER(:UTE_APP_ID))")

                Dim filtroLike As String = String.Format("%{0}%", filtroGenerico)

                cmd.Parameters.AddWithValue("UTE_CODICE", filtroLike)
                cmd.Parameters.AddWithValue("UTE_DESCRIZIONE", filtroLike)
                cmd.Parameters.AddWithValue("UTE_APP_ID", filtroLike)

            End If

            cmd.CommandText = query.ToString()

        End Sub

        Private Function GetCampoVariabileDaAssociare(campo As Entities.AttivitaVariabileDaAssociare.Ordinamento) As String

            Select Case campo

                Case Entities.AttivitaVariabileDaAssociare.Ordinamento.Codice
                    Return "OSS_CODICE"

                Case Entities.AttivitaVariabileDaAssociare.Ordinamento.Descrizione
                    Return "OSS_DESCRIZIONE"

                Case Else
                    Throw New NotImplementedException("Entities.AttivitaVariabileDaAssociare.Ordinamento: valore non previsto")

            End Select

            Return String.Empty

        End Function

        Private Function GetCampoUtentiDaAssociare(campo As Entities.AttivitaUtentiDaAssociare.Ordinamento) As String

            Select Case campo

                Case Entities.AttivitaUtentiDaAssociare.Ordinamento.Codice
                    Return "UTE_CODICE"

                Case Entities.AttivitaUtentiDaAssociare.Ordinamento.Descrizione
                    Return "UTE_DESCRIZIONE"

                Case Entities.AttivitaUtentiDaAssociare.Ordinamento.AppId
                    Return "UTE_APP_ID"

                Case Entities.AttivitaUtentiDaAssociare.Ordinamento.Cognome
                    Return "UTE_COGNOME"

                Case Entities.AttivitaUtentiDaAssociare.Ordinamento.Nome
                    Return "UTE_NOME"

                Case Entities.AttivitaUtentiDaAssociare.Ordinamento.UslCodice
                    Return "UGS_USL_CODICE"

                Case Else
                    Throw New NotImplementedException("Entities.AttivitaVariabileDaAssociare.Ordinamento: valore non previsto")

            End Select

            Return String.Empty

        End Function

        Private Function GetQuerySelectVariabiliAssociate(isCount As Boolean) As String

            Dim query As New System.Text.StringBuilder()

            If isCount Then
                query.Append("SELECT COUNT(AAV_ID) ")
            Else
                query.Append("SELECT AAV_ID, AAV_ATA_ID, AAV_OSS_CODICE, AAV_ORDINE, AAV_OBBLIGATORIO, OSS_DESCRIZIONE, OSS_TIPO_RISPOSTA ")
            End If

            query.Append(" FROM T_ANA_ATTIVITA_VARIABILI ")

            Return query.ToString()

        End Function

        Private Function GetQuerySelectUtentiAssociati(isCount As Boolean) As String

            Dim query As New System.Text.StringBuilder()

            If isCount Then
                query.Append("SELECT COUNT(AAU_ID) ")
            Else
                query.Append("SELECT AAU_ID, AAU_ATA_ID, AAU_UTE_ID, AAU_APP_ID, UTE_DESCRIZIONE, UTE_COGNOME, UTE_NOME,UTE_CODICE, UGS_USL_CODICE ")
            End If

            query.Append(" FROM T_ANA_ATTIVITA_UTENTI ")

            Return query.ToString()

        End Function

        Private Sub SetCommandSelectVariabiliAssociate(cmd As OracleCommand, idAttivita As Integer, filtroGenerico As String, isCount As Boolean)

            Dim query As New System.Text.StringBuilder(GetQuerySelectVariabiliAssociate(isCount))

            If Not isCount Then
                query.Append(" JOIN T_ANA_OSSERVAZIONI ON AAV_OSS_CODICE = OSS_CODICE ")
            End If

            query.Append(" WHERE AAV_ATA_ID = :AAV_ATA_ID ")
            cmd.Parameters.AddWithValue("AAV_ATA_ID", idAttivita)

            If Not String.IsNullOrWhiteSpace(filtroGenerico) Then

                query.Append(" AND (UPPER(AAV_OSS_CODICE) LIKE UPPER(:AAV_OSS_CODICE) OR UPPER(OSS_DESCRIZIONE) LIKE UPPER(:OSS_DESCRIZIONE)) ")

                Dim filtroLike As String = String.Format("%{0}%", filtroGenerico)

                cmd.Parameters.AddWithValue("AAV_OSS_CODICE", filtroLike)
                cmd.Parameters.AddWithValue("OSS_DESCRIZIONE", filtroLike)

            End If

            If Not isCount Then
                query.Append(" ORDER BY AAV_ORDINE ")
            End If

            cmd.CommandText = query.ToString()

        End Sub

        Private Sub SetCommandSelectUtentiAssociati(cmd As OracleCommand, idAttivita As Integer, filtroGenerico As String, isCount As Boolean)

            Dim query As New System.Text.StringBuilder(GetQuerySelectUtentiAssociati(isCount))

            If Not isCount Then
                query.Append(" JOIN V_ANA_UTENTI ON AAU_UTE_ID = UTE_ID AND AAU_APP_ID = UTE_APP_ID")
                query.Append(" JOIN T_USL_GESTITE ON UGS_APP_ID= UTE_APP_ID")
            End If

            query.Append(" WHERE AAU_ATA_ID = :AAU_ATA_ID ")
            cmd.Parameters.AddWithValue("AAU_ATA_ID", idAttivita)

            If Not String.IsNullOrWhiteSpace(filtroGenerico) Then

                query.Append(" AND (UPPER(UTE_CODICE) LIKE UPPER(:UTE_CODICE) OR UPPER(UTE_DESCRIZIONE) LIKE UPPER(:UTE_DESCRIZIONE)) OR UPPER(UTE_COGNOME) LIKE UPPER(:UTE_COGNOME)) OR UPPER(UTE_NOME) LIKE UPPER(:UTE_NOME))")

                Dim filtroLike As String = String.Format("%{0}%", filtroGenerico)

                cmd.Parameters.AddWithValue("UTE_CODICE", filtroLike)
                cmd.Parameters.AddWithValue("UTE_DESCRIZIONE", filtroLike)
                cmd.Parameters.AddWithValue("UTE_COGNOME", filtroLike)
                cmd.Parameters.AddWithValue("UTE_NOME", filtroLike)

            End If

            If Not isCount Then
                query.Append(" ORDER BY UTE_CODICE, UGS_USL_CODICE")
            End If

            cmd.CommandText = query.ToString()

        End Sub

        Private Function GetListAttivitaVariabili(cmd As OracleCommand) As List(Of Entities.AttivitaVariabileAssociata)

            Dim list As New List(Of Entities.AttivitaVariabileAssociata)()

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    Dim aav_id As Integer = idr.GetOrdinal("AAV_ID")
                    Dim aav_ata_id As Integer = idr.GetOrdinal("AAV_ATA_ID")
                    Dim aav_oss_codice As Integer = idr.GetOrdinal("AAV_OSS_CODICE")
                    Dim aav_ordine As Integer = idr.GetOrdinal("AAV_ORDINE")
                    Dim oss_descrizione As Integer = idr.GetOrdinal("OSS_DESCRIZIONE")
                    Dim oss_tipo_risposta As Integer = idr.GetOrdinal("OSS_TIPO_RISPOSTA")
                    Dim aav_obb As Integer = idr.GetOrdinal("AAV_OBBLIGATORIO")

                    Dim item As Entities.AttivitaVariabileAssociata = Nothing

                    While idr.Read()

                        item = New Entities.AttivitaVariabileAssociata()

                        item.CodiceVariabile = idr.GetStringOrDefault(aav_oss_codice)
                        item.DescrizioneVariabile = idr.GetStringOrDefault(oss_descrizione)
                        item.IdAttivita = idr.GetInt32OrDefault(aav_ata_id)
                        item.IdVariabile = idr.GetInt32OrDefault(aav_id)
                        item.Ordine = idr.GetInt32OrDefault(aav_ordine)
                        item.Obbligatorio = idr.GetBooleanOrDefault(aav_obb)
                        item.TipoRisposta = idr.GetStringOrDefault(oss_tipo_risposta)

                        list.Add(item)

                    End While

                End If

            End Using

            Return list

        End Function

        Private Function GetListAttivitaUtentiAssociati(cmd As OracleCommand) As List(Of Entities.AttivitaUtenteAssociato)

            Dim list As New List(Of Entities.AttivitaUtenteAssociato)()

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    Dim aau_id As Integer = idr.GetOrdinal("AAU_ID")
                    Dim aau_ata_id As Integer = idr.GetOrdinal("AAU_ATA_ID")
                    Dim aau_ute_id As Integer = idr.GetOrdinal("AAU_UTE_ID")
                    Dim aau_app_id As Integer = idr.GetOrdinal("AAU_APP_ID")
                    Dim ute_descrizione As Integer = idr.GetOrdinal("UTE_DESCRIZIONE")
                    Dim ute_cognome As Integer = idr.GetOrdinal("UTE_COGNOME")
                    Dim ute_nome As Integer = idr.GetOrdinal("UTE_NOME")
                    Dim ute_codice As Integer = idr.GetOrdinal("UTE_CODICE")
                    Dim ugs_usl_codice As Integer = idr.GetOrdinal("UGS_USL_CODICE")

                    Dim item As Entities.AttivitaUtenteAssociato = Nothing

                    While idr.Read()

                        item = New Entities.AttivitaUtenteAssociato()

                        item.Id = idr.GetInt32OrDefault(aau_id)
                        item.IdAttivita = idr.GetInt32OrDefault(aau_ata_id)
                        item.AppId = idr.GetStringOrDefault(aau_app_id)
                        item.IdUtenti = idr.GetInt32OrDefault(aau_ute_id)
                        item.Descrizione = idr.GetStringOrDefault(ute_descrizione)
                        item.Cognome = idr.GetStringOrDefault(ute_cognome)
                        item.Nome = idr.GetStringOrDefault(ute_nome)
                        item.Codice = idr.GetStringOrDefault(ute_codice)
                        item.UslCodice = idr.GetStringOrDefault(ugs_usl_codice)


                        list.Add(item)

                    End While

                End If

            End Using

            Return list

        End Function

#End Region

#End Region

#Region " Registrazione Attività "

        ''' <summary>
        ''' Restituisce le attività registrate, in base ai filtri impostati.
        ''' </summary>
        ''' <param name="filtroGenerico">Filtro su codice e descrizione, per attività e tipo attività</param>
        ''' <param name="filtroSoloValidi"></param>
        ''' <param name="appId"></param>
        ''' <param name="dataEsecuzioneInizio"></param>
        ''' <param name="dataEsecuzioneFine"></param>
        ''' <param name="dataRegistrazioneInizio"></param>
        ''' <param name="dataRegistrazioneFine"></param>
        ''' <param name="idUtenteRegistrazione"></param>
        ''' <param name="campoOrdinamento"></param>
        ''' <param name="versoOrdinamento"></param>
        ''' <param name="pagingOptions"></param>
        ''' <param name="idUtenteconnesso"></param>
        ''' <param name="codiceUsl"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetElencoAttivitaRegistrazione(filtroGenerico As String, filtroSoloValidi As Boolean, appId As String, dataEsecuzioneInizio As DateTime?, dataEsecuzioneFine As DateTime?,
                                                       dataRegistrazioneInizio As DateTime?, dataRegistrazioneFine As DateTime?, idUtenteRegistrazione As Long?,
                                                       campoOrdinamento As AttivitaRegistrazione.Ordinamento?, versoOrdinamento As String,
                                                       pagingOptions As Onit.OnAssistnet.Data.PagingOptions, idUtenteconnesso As Integer, codiceUsl As String) As IEnumerable(Of AttivitaRegistrazione) Implements IAttivitaProvider.GetElencoAttivitaRegistrazione

            Dim list As List(Of AttivitaRegistrazione) = Nothing

            Using cmd As New OracleCommand()

                cmd.Connection = Connection

                ' Query di select con eventuali filtri
                SetCommandSelectAttivitaRegistrazione(cmd, appId, filtroGenerico, filtroSoloValidi, dataEsecuzioneInizio, dataEsecuzioneFine,
                                                      dataRegistrazioneInizio, dataRegistrazioneFine, idUtenteRegistrazione, False, idUtenteconnesso, codiceUsl)

                ' Ordinamento
                If campoOrdinamento.HasValue Then

                    cmd.CommandText += String.Format(" ORDER BY {0} {1}", GetCampoAttivitaRegistrazione(campoOrdinamento.Value), versoOrdinamento)

                End If

                ' Paginazione
                If Not pagingOptions Is Nothing Then

                    cmd.AddPaginatedQuery(pagingOptions)

                End If

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    list = GetListAttivitaRegistrazione(cmd)

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list.AsEnumerable()

        End Function

        ''' <summary>
        ''' Restituisce il totale delle attività registrate, in base ai filtri impostati
        ''' </summary>
        ''' <param name="filtroGenerico"></param>
        ''' <param name="filtroSoloValidi"></param>
        ''' <param name="dataEsecuzioneInizio"></param>
        ''' <param name="dataEsecuzioneFine"></param>
        ''' <param name="dataRegistrazioneInizio"></param>
        ''' <param name="dataRegistrazioneFine"></param>
        ''' <param name="idUtenteRegistrazione"></param>
        ''' <param name="codiceUsl"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CountElencoAttivitaRegistrazione(filtroGenerico As String, filtroSoloValidi As Boolean, dataEsecuzioneInizio As DateTime?, dataEsecuzioneFine As DateTime?,
                                                         dataRegistrazioneInizio As DateTime?, dataRegistrazioneFine As DateTime?, idUtenteRegistrazione As Long?,
                                                         codiceUsl As String) As Integer Implements IAttivitaProvider.CountElencoAttivitaRegistrazione

            Dim count As Integer = 0

            Using cmd As New OracleCommand()

                cmd.Connection = Connection

                ' Query di select con eventuali filtri
                SetCommandSelectAttivitaRegistrazione(cmd, String.Empty, filtroGenerico, filtroSoloValidi, dataEsecuzioneInizio, dataEsecuzioneFine,
                                                      dataRegistrazioneInizio, dataRegistrazioneFine, idUtenteRegistrazione, True, 0, codiceUsl)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        count = Convert.ToInt32(obj)
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Restituisce l'attività specificata
        ''' </summary>
        ''' <param name="idAttivitaRegistrazione"></param>
        ''' <param name="appId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAttivitaRegistrazione(idAttivitaRegistrazione As Integer, appId As String, idUtenteConnesso As Integer) As Entities.AttivitaRegistrazione Implements IAttivitaProvider.GetAttivitaRegistrazione

            Dim item As Entities.AttivitaRegistrazione = Nothing

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection
                cmd.CommandText = GetQuerySelectAttivitaRegistrazione(False) + " WHERE ATT_ID = :ATT_ID"

                cmd.Parameters.AddWithValue("UTE_APP_ID", appId)
                cmd.Parameters.AddWithValue("ATT_ID", idAttivitaRegistrazione)
                cmd.Parameters.AddWithValue("AAU_UTE_ID", idUtenteConnesso)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim list As List(Of Entities.AttivitaRegistrazione) = GetListAttivitaRegistrazione(cmd)
                    If Not list.IsNullOrEmpty() Then
                        item = list.First()
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return item

        End Function

        ''' <summary>
        ''' Restituisce il valore della sequence per l'id dell'attività da inserire
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetNextIdAttivitaRegistrazione() As Integer Implements IAttivitaProvider.GetNextIdAttivitaRegistrazione

            Dim nextVal As Integer = 0

            Using cmd As New OracleCommand("SELECT SEQ_ATTIVITA.NEXTVAL FROM DUAL", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then
                        nextVal = Convert.ToInt32(obj)
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return nextVal

        End Function

        ''' <summary>
        ''' Inserimento attività registrata
        ''' </summary>
        ''' <param name="attivitaRegistrazione"></param>
        ''' <param name="codiceUsl"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertAttivitaRegistrazione(attivitaRegistrazione As AttivitaRegistrazione, codiceUsl As String) As Integer Implements IAttivitaProvider.InsertAttivitaRegistrazione

            Dim count As Integer = 0

            Using cmd As New OracleCommand("INSERT INTO T_ATTIVITA (ATT_ID, ATT_TAT_CODICE, ATT_ATA_ID, ATT_DATA_ATTIVITA, ATT_DATA_REGISTRAZIONE, ATT_UTE_ID_REGISTRAZIONE, ATT_SCU_CODICE, ATT_AZI_CODICE) VALUES (:ATT_ID, :ATT_TAT_CODICE, :ATT_ATA_ID, :ATT_DATA_ATTIVITA, :ATT_DATA_REGISTRAZIONE, :ATT_UTE_ID_REGISTRAZIONE, :ATT_SCU_CODICE, :ATT_AZI_CODICE)", Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("ATT_ID", attivitaRegistrazione.Id)
                    cmd.Parameters.AddWithValue("ATT_TAT_CODICE", GetStringParam(attivitaRegistrazione.CodiceAttivitaTipo))
                    cmd.Parameters.AddWithValue("ATT_ATA_ID", attivitaRegistrazione.IdAttivitaAnagrafe)
                    cmd.Parameters.AddWithValue("ATT_DATA_ATTIVITA", attivitaRegistrazione.DataAttivita)
                    cmd.Parameters.AddWithValue("ATT_DATA_REGISTRAZIONE", attivitaRegistrazione.DataRegistrazione)
                    cmd.Parameters.AddWithValue("ATT_UTE_ID_REGISTRAZIONE", attivitaRegistrazione.IdUtenteRegistrazione)
                    cmd.Parameters.AddWithValue("ATT_SCU_CODICE", GetStringParam(attivitaRegistrazione.CodiceScuola))
                    cmd.Parameters.AddWithValue("ATT_AZI_CODICE", codiceUsl)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Modifica attività registrazione
        ''' </summary>
        ''' <param name="attivitaRegistrazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateAttivitaRegistrazione(attivitaRegistrazione As Entities.AttivitaRegistrazione) As Integer Implements IAttivitaProvider.UpdateAttivitaRegistrazione

            Dim count As Integer = 0

            Using cmd As New OracleCommand("UPDATE T_ATTIVITA SET ATT_TAT_CODICE = :ATT_TAT_CODICE, ATT_ATA_ID = :ATT_ATA_ID, ATT_DATA_ATTIVITA = :ATT_DATA_ATTIVITA, ATT_DATA_VARIAZIONE = :ATT_DATA_VARIAZIONE, ATT_UTE_ID_VARIAZIONE = :ATT_UTE_ID_VARIAZIONE, ATT_DATA_ELIMINAZIONE = :ATT_DATA_ELIMINAZIONE, ATT_UTE_ID_ELIMINAZIONE = :ATT_UTE_ID_ELIMINAZIONE, ATT_SCU_CODICE = :ATT_SCU_CODICE WHERE ATT_ID = :ATT_ID", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("ATT_TAT_CODICE", GetStringParam(attivitaRegistrazione.CodiceAttivitaTipo))
                    cmd.Parameters.AddWithValue("ATT_ATA_ID", attivitaRegistrazione.IdAttivitaAnagrafe)
                    cmd.Parameters.AddWithValue("ATT_DATA_ATTIVITA", attivitaRegistrazione.DataAttivita)

                    cmd.Parameters.AddWithValue("ATT_DATA_VARIAZIONE", GetDateParam(attivitaRegistrazione.DataVariazione))

                    If attivitaRegistrazione.IdUtenteVariazione.HasValue Then
                        cmd.Parameters.AddWithValue("ATT_UTE_ID_VARIAZIONE", attivitaRegistrazione.IdUtenteVariazione.Value)
                    Else
                        cmd.Parameters.AddWithValue("ATT_UTE_ID_VARIAZIONE", DBNull.Value)
                    End If

                    cmd.Parameters.AddWithValue("ATT_DATA_ELIMINAZIONE", GetDateParam(attivitaRegistrazione.DataEliminazione))

                    If attivitaRegistrazione.IdUtenteEliminazione.HasValue Then
                        cmd.Parameters.AddWithValue("ATT_UTE_ID_ELIMINAZIONE", attivitaRegistrazione.IdUtenteEliminazione.Value)
                    Else
                        cmd.Parameters.AddWithValue("ATT_UTE_ID_ELIMINAZIONE", DBNull.Value)
                    End If

                    cmd.Parameters.AddWithValue("ATT_SCU_CODICE", GetStringParam(attivitaRegistrazione.CodiceScuola))

                    cmd.Parameters.AddWithValue("ATT_ID", attivitaRegistrazione.Id)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Restituisce la lista di risposte possibili per le variabili con risposte codificate associate all'attività specificata
        ''' </summary>
        ''' <param name="idAttivitaAnagrafe"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetRispostePossibiliAttivita(idAttivitaAnagrafe As Integer) As IEnumerable(Of Entities.AttivitaRegistrazioneRisposta) Implements IAttivitaProvider.GetRispostePossibiliAttivita

            Dim list As New List(Of Entities.AttivitaRegistrazioneRisposta)()

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                cmd.CommandText =
                    " select AAV_ID, RIS_CODICE, RIS_DESCRIZIONE, RIO_DEFAULT " +
                    " from T_ANA_ATTIVITA_VARIABILI " +
                    " join T_ANA_OSSERVAZIONI on AAV_OSS_CODICE = OSS_CODICE " +
                    " join T_ANA_LINK_OSS_RISPOSTE on OSS_CODICE = RIO_OSS_CODICE " +
                    " join T_ANA_RISPOSTE on RIO_RIS_CODICE = RIS_CODICE " +
                    " where AAV_ATA_ID = :AAV_ATA_ID " +
                    " and OSS_TIPO_RISPOSTA in (:OSS_TIPO_RISPOSTA1, :OSS_TIPO_RISPOSTA2) " +
                    " and (RIS_OBSOLETO is null or RIS_OBSOLETO = 'N') " +
                    " order by RIO_OSS_CODICE, RIO_N_RISPOSTA "

                cmd.Parameters.AddWithValue("AAV_ATA_ID", idAttivitaAnagrafe)
                cmd.Parameters.AddWithValue("OSS_TIPO_RISPOSTA1", Constants.TipoRispostaOsservazioneBilancio.CodificataSingola)
                cmd.Parameters.AddWithValue("OSS_TIPO_RISPOSTA2", Constants.TipoRispostaOsservazioneBilancio.CodificataMultipla)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim aav_id As Integer = idr.GetOrdinal("AAV_ID")
                            Dim ris_codice As Integer = idr.GetOrdinal("RIS_CODICE")
                            Dim ris_descrizione As Integer = idr.GetOrdinal("RIS_DESCRIZIONE")
                            Dim rio_default As Integer = idr.GetOrdinal("RIO_DEFAULT")


                            Dim item As Entities.AttivitaRegistrazioneRisposta = Nothing

                            While idr.Read()

                                item = New Entities.AttivitaRegistrazioneRisposta()

                                item.IdAttivitaVariabile = idr.GetInt32OrDefault(aav_id)
                                item.CodiceRisposta = idr.GetStringOrDefault(ris_codice)
                                item.DescrizioneRisposta = idr.GetStringOrDefault(ris_descrizione)
                                item.IsDefault = idr.GetBooleanOrDefault(rio_default)

                                list.Add(item)

                            End While

                        End If

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list.AsEnumerable()

        End Function

        ''' <summary>
        ''' Restituisce i valori relativi all'attività rilevata
        ''' </summary>
        ''' <param name="idAttivitaRegistrazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetValoriAttivitaRegistrazione(idAttivitaRegistrazione As Integer) As IEnumerable(Of Entities.AttivitaRegistrazioneValore) Implements IAttivitaProvider.GetValoriAttivitaRegistrazione

            Dim list As List(Of Entities.AttivitaRegistrazioneValore) = Nothing

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                cmd.CommandText =
                    " select ATT_ID, ATV_ID, ATV_RIS_CODICE, NVL(RIS_DESCRIZIONE, ATV_RISPOSTA) VALORE, " +
                    " ATV_DATA_REGISTRAZIONE, ATV_UTE_ID_REGISTRAZIONE, ATV_DATA_VARIAZIONE, ATV_UTE_ID_VARIAZIONE, " +
                    " AAV_ID, AAV_OSS_CODICE, OSS_DESCRIZIONE, OSS_TIPO_RISPOSTA, AAV_ORDINE, AAV_OBBLIGATORIO, OSS_TIPO_DATI_RISPOSTA" +
                    " from T_ATTIVITA " +
                    " join T_ANA_ATTIVITA_VARIABILI on ATT_ATA_ID = AAV_ATA_ID " +
                    " join T_ANA_OSSERVAZIONI on OSS_CODICE = AAV_OSS_CODICE " +
                    " left join T_ATTIVITA_VALORI on ATV_ATT_ID = ATT_ID  and ATV_AAV_ID = AAV_ID " +
                    " left join T_ANA_RISPOSTE on RIS_CODICE = ATV_RIS_CODICE " +
                    " where ATT_ID = :ATT_ID " +
                    " AND OSS_DATA_INIZIO_VALIDITA <= ATT_DATA_REGISTRAZIONE" +
                    " AND (OSS_DATA_FINE_VALIDITA+1 > ATT_DATA_REGISTRAZIONE OR OSS_DATA_FINE_VALIDITA Is null)" +
                    " order by AAV_ORDINE "

                cmd.Parameters.AddWithValue("ATT_ID", idAttivitaRegistrazione)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    list = GetListAttivitaRegistrazioneValore(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list.AsEnumerable()

        End Function

        ''' <summary>
        ''' Restituisce il valore specificato
        ''' </summary>
        ''' <param name="idAttivitaRegistrazioneValore"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAttivitaRegistrazioneValore(idAttivitaRegistrazioneValore As Long) As Entities.AttivitaRegistrazioneValore Implements IAttivitaProvider.GetAttivitaRegistrazioneValore

            Dim item As Entities.AttivitaRegistrazioneValore = Nothing

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                cmd.CommandText =
                    " select ATV_ATT_ID ATT_ID, ATV_ID, ATV_RIS_CODICE, NVL(RIS_DESCRIZIONE, ATV_RISPOSTA) VALORE, " +
                    " ATV_DATA_REGISTRAZIONE, ATV_UTE_ID_REGISTRAZIONE, ATV_DATA_VARIAZIONE, ATV_UTE_ID_VARIAZIONE, " +
                    " AAV_ID, AAV_OSS_CODICE, OSS_DESCRIZIONE, OSS_TIPO_RISPOSTA, AAV_ORDINE, AAV_OBBLIGATORIO, OSS_TIPO_DATI_RISPOSTA " +
                    " from T_ATTIVITA_VALORI " +
                    " join T_ANA_ATTIVITA_VARIABILI on ATV_AAV_ID = AAV_ID " +
                    " join T_ANA_OSSERVAZIONI on AAV_OSS_CODICE = OSS_CODICE " +
                    " left join T_ANA_RISPOSTE on ATV_RIS_CODICE = RIS_CODICE " +
                    " where ATV_ID = :ATV_ID "

                cmd.Parameters.AddWithValue("ATV_ID", idAttivitaRegistrazioneValore)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim list As List(Of Entities.AttivitaRegistrazioneValore) = GetListAttivitaRegistrazioneValore(cmd)

                    If Not list.IsNullOrEmpty() Then
                        item = list.First()
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return item

        End Function

        ''' <summary>
        ''' Restituisce il valore della sequence per l'id del valore da inserire
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetNextIdAttivitaRegistrazioneValore() As Long Implements IAttivitaProvider.GetNextIdAttivitaRegistrazioneValore

            Dim nextVal As Integer = 0

            Using cmd As New OracleCommand("SELECT SEQ_ATTIVITA_VALORI.NEXTVAL FROM DUAL", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then
                        nextVal = Convert.ToInt32(obj)
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return nextVal

        End Function

        ''' <summary>
        ''' Inserimento valore per l'attività registrata
        ''' </summary>
        ''' <param name="valore"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertAttivitaRegistrazioneValore(valore As Entities.AttivitaRegistrazioneValore) As Integer Implements IAttivitaProvider.InsertAttivitaRegistrazioneValore

            Dim count As Integer = 0

            Using cmd As New OracleCommand("INSERT INTO T_ATTIVITA_VALORI (ATV_ID, ATV_ATT_ID, ATV_AAV_ID, ATV_RIS_CODICE, ATV_RISPOSTA, ATV_DATA_REGISTRAZIONE, ATV_UTE_ID_REGISTRAZIONE, ATV_DATA_VARIAZIONE, ATV_UTE_ID_VARIAZIONE) VALUES (:ATV_ID, :ATV_ATT_ID, :ATV_AAV_ID, :ATV_RIS_CODICE, :ATV_RISPOSTA, :ATV_DATA_REGISTRAZIONE, :ATV_UTE_ID_REGISTRAZIONE, :ATV_DATA_VARIAZIONE, :ATV_UTE_ID_VARIAZIONE)", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("ATV_ID", valore.IdAttivitaRegistrazioneValore)
                    cmd.Parameters.AddWithValue("ATV_ATT_ID", valore.IdAttivitaRegistrazione)
                    cmd.Parameters.AddWithValue("ATV_AAV_ID", valore.IdAttivitaVariabile)
                    cmd.Parameters.AddWithValue("ATV_RIS_CODICE", GetStringParam(valore.CodiceRisposta))
                    cmd.Parameters.AddWithValue("ATV_RISPOSTA", GetStringParam(valore.ValoreRisposta))
                    cmd.Parameters.AddWithValue("ATV_DATA_REGISTRAZIONE", valore.DataRegistrazione.Value)
                    cmd.Parameters.AddWithValue("ATV_UTE_ID_REGISTRAZIONE", valore.IdUtenteRegistrazione.Value)

                    If valore.DataVariazione.HasValue Then
                        cmd.Parameters.AddWithValue("ATV_DATA_VARIAZIONE", valore.DataVariazione.Value)
                    Else
                        cmd.Parameters.AddWithValue("ATV_DATA_VARIAZIONE", DBNull.Value)
                    End If

                    If valore.IdUtenteVariazione.HasValue Then
                        cmd.Parameters.AddWithValue("ATV_UTE_ID_VARIAZIONE", valore.IdUtenteVariazione)
                    Else
                        cmd.Parameters.AddWithValue("ATV_UTE_ID_VARIAZIONE", DBNull.Value)
                    End If

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Modifica valore per l'attività registrata
        ''' </summary>
        ''' <param name="valore"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateAttivitaRegistrazioneValore(valore As Entities.AttivitaRegistrazioneValore) As Integer Implements IAttivitaProvider.UpdateAttivitaRegistrazioneValore

            Dim count As Integer = 0

            Using cmd As New OracleCommand("UPDATE T_ATTIVITA_VALORI SET ATV_ATT_ID = :ATV_ATT_ID, ATV_AAV_ID = :ATV_AAV_ID, ATV_RIS_CODICE = :ATV_RIS_CODICE, ATV_RISPOSTA = :ATV_RISPOSTA, ATV_DATA_VARIAZIONE = :ATV_DATA_VARIAZIONE, ATV_UTE_ID_VARIAZIONE = :ATV_UTE_ID_VARIAZIONE WHERE ATV_ID = :ATV_ID", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("ATV_ATT_ID", valore.IdAttivitaRegistrazione)
                    cmd.Parameters.AddWithValue("ATV_AAV_ID", valore.IdAttivitaVariabile)
                    cmd.Parameters.AddWithValue("ATV_RIS_CODICE", GetStringParam(valore.CodiceRisposta))
                    cmd.Parameters.AddWithValue("ATV_RISPOSTA", GetStringParam(valore.ValoreRisposta))

                    If valore.DataVariazione.HasValue Then
                        cmd.Parameters.AddWithValue("ATV_DATA_VARIAZIONE", valore.DataVariazione.Value)
                    Else
                        cmd.Parameters.AddWithValue("ATV_DATA_VARIAZIONE", DBNull.Value)
                    End If

                    If valore.IdUtenteVariazione.HasValue Then
                        cmd.Parameters.AddWithValue("ATV_UTE_ID_VARIAZIONE", valore.IdUtenteVariazione)
                    Else
                        cmd.Parameters.AddWithValue("ATV_UTE_ID_VARIAZIONE", DBNull.Value)
                    End If

                    cmd.Parameters.AddWithValue("ATV_ID", valore.IdAttivitaRegistrazioneValore)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

#Region " Private "

        Private Function GetQuerySelectAttivitaRegistrazione(isCount As Boolean) As String

            Dim query As New System.Text.StringBuilder()

            If isCount Then
                query.Append(" SELECT COUNT(ATT_ID) ")
            Else
                query.Append(" SELECT ATT_ID, ATT_TAT_CODICE, TAT_DESCRIZIONE, ATT_ATA_ID, ATA_CODICE, ATA_DESCRIZIONE, ")
                query.Append(" ATT_DATA_ATTIVITA, ATT_DATA_REGISTRAZIONE, ATT_UTE_ID_REGISTRAZIONE, UTE_CODICE, ")
                query.Append(" ATT_DATA_VARIAZIONE, ATT_UTE_ID_VARIAZIONE, ATT_DATA_ELIMINAZIONE, ATT_UTE_ID_ELIMINAZIONE, ")
                query.Append(" ATT_SCU_CODICE, SCU_DESCRIZIONE ")
            End If

            query.Append(" FROM T_ATTIVITA ")
            query.Append(" LEFT JOIN T_ANA_TIPI_ATTIVITA ON ATT_TAT_CODICE = TAT_CODICE ")
            query.Append(" JOIN T_ANA_ATTIVITA ON ATT_ATA_ID = ATA_ID ")

            If Not isCount Then
                'query.Append(" LEFT JOIN V_ANA_UTENTI ON ATT_UTE_ID_REGISTRAZIONE = UTE_ID AND UTE_APP_ID = :UTE_APP_ID ")
                query.Append(" LEFT JOIN T_ANA_UTENTI ON ATT_UTE_ID_REGISTRAZIONE = UTE_ID ")
                query.Append(" JOIN T_ANA_ATTIVITA_UTENTI ON AAU_ATA_ID = ATA_ID AND AAU_APP_ID = :UTE_APP_ID AND AAU_UTE_ID = :AAU_UTE_ID ")
                query.Append(" LEFT JOIN V_ANA_SCUOLE_PPA ON ATT_SCU_CODICE = SCU_CODICE ")
            End If

            Return query.ToString()

        End Function

        Private Sub SetCommandSelectAttivitaRegistrazione(cmd As OracleCommand, appId As String, filtroGenerico As String, filtroSoloValidi As Boolean, dataEsecuzioneInizio As DateTime?, dataEsecuzioneFine As DateTime?, dataRegistrazioneInizio As DateTime?, dataRegistrazioneFine As DateTime?, idUtenteRegistrazione As Long?, isCount As Boolean, idUtenteconnesso As Integer, codiceUsl As String)

            Dim filtro As New Text.StringBuilder()

            filtro.Append(" WHERE ATT_AZI_CODICE = :ATT_AZI_CODICE AND ")
            cmd.Parameters.AddWithValue("ATT_AZI_CODICE", codiceUsl)

            If Not String.IsNullOrWhiteSpace(filtroGenerico) Then

                filtro.Append(" (")

                ' Controllo che il filtro sia un numero, se si aggiungo filtro per id
                Dim intTemp As Integer

                If Int32.TryParse(filtroGenerico, intTemp) Then
                    filtro.Append(" ATT_ID = :ATT_ID OR ")
                    cmd.Parameters.AddWithValue("ATT_ID", filtroGenerico)
                End If

                filtro.Append(" UPPER(ATA_CODICE) LIKE UPPER(:ATA_CODICE) OR ")
                filtro.Append(" UPPER(ATA_DESCRIZIONE) LIKE UPPER(:ATA_DESCRIZIONE) ")
                filtro.Append(" ) AND ")

                Dim filtroLike As String = String.Format("%{0}%", filtroGenerico)

                cmd.Parameters.AddWithValue("ATA_CODICE", filtroLike)
                cmd.Parameters.AddWithValue("ATA_DESCRIZIONE", filtroLike)

            End If

            If filtroSoloValidi Then

                filtro.Append(" (ATT_DATA_ELIMINAZIONE IS NULL OR ATT_DATA_ELIMINAZIONE > SYSDATE) AND ")

            End If

            If dataEsecuzioneInizio.HasValue Then

                filtro.Append(" ATT_DATA_ATTIVITA >= :ATT_DATA_ATTIVITA_INIZIO AND ")
                cmd.Parameters.AddWithValue("ATT_DATA_ATTIVITA_INIZIO", dataEsecuzioneInizio.Value)

            End If

            If dataEsecuzioneFine.HasValue Then

                filtro.Append(" ATT_DATA_ATTIVITA <= :ATT_DATA_ATTIVITA_FINE AND ")
                cmd.Parameters.AddWithValue("ATT_DATA_ATTIVITA_FINE", dataEsecuzioneFine.Value)

            End If

            If dataRegistrazioneInizio.HasValue Then

                filtro.Append(" ATT_DATA_REGISTRAZIONE >= :ATT_DATA_REGISTRAZIONE_INIZIO AND ")
                cmd.Parameters.AddWithValue("ATT_DATA_REGISTRAZIONE_INIZIO", dataRegistrazioneInizio.Value)

            End If

            If dataRegistrazioneFine.HasValue Then

                filtro.Append(" ATT_DATA_REGISTRAZIONE <= :ATT_DATA_REGISTRAZIONE_FINE AND ")
                cmd.Parameters.AddWithValue("ATT_DATA_REGISTRAZIONE_FINE", dataRegistrazioneFine.Value)

            End If

            If idUtenteRegistrazione.HasValue Then

                filtro.Append(" ATT_UTE_ID_REGISTRAZIONE = :ATT_UTE_ID_REGISTRAZIONE AND ")
                cmd.Parameters.AddWithValue("ATT_UTE_ID_REGISTRAZIONE", idUtenteRegistrazione.Value)

            End If

            filtro.RemoveLast(4)

            If Not isCount Then
                cmd.Parameters.AddWithValue("UTE_APP_ID", appId)
                cmd.Parameters.AddWithValue("AAU_UTE_ID", idUtenteconnesso)
            End If

            cmd.CommandText = GetQuerySelectAttivitaRegistrazione(isCount) + filtro.ToString()

        End Sub

        Private Function GetListAttivitaRegistrazione(cmd As OracleCommand) As List(Of Entities.AttivitaRegistrazione)

            Dim list As New List(Of Entities.AttivitaRegistrazione)()

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    Dim att_id As Integer = idr.GetOrdinal("ATT_ID")
                    Dim att_tat_codice As Integer = idr.GetOrdinal("ATT_TAT_CODICE")
                    Dim tat_descrizione As Integer = idr.GetOrdinal("TAT_DESCRIZIONE")
                    Dim att_ata_id As Integer = idr.GetOrdinal("ATT_ATA_ID")
                    Dim ata_codice As Integer = idr.GetOrdinal("ATA_CODICE")
                    Dim ata_descrizione As Integer = idr.GetOrdinal("ATA_DESCRIZIONE")
                    Dim att_data_attivita As Integer = idr.GetOrdinal("ATT_DATA_ATTIVITA")
                    Dim att_data_registrazione As Integer = idr.GetOrdinal("ATT_DATA_REGISTRAZIONE")
                    Dim att_ute_id_registrazione As Integer = idr.GetOrdinal("ATT_UTE_ID_REGISTRAZIONE")
                    Dim ute_codice As Integer = idr.GetOrdinal("UTE_CODICE")
                    Dim att_data_variazione As Integer = idr.GetOrdinal("ATT_DATA_VARIAZIONE")
                    Dim att_ute_id_variazione As Integer = idr.GetOrdinal("ATT_UTE_ID_VARIAZIONE")
                    Dim att_data_eliminazione As Integer = idr.GetOrdinal("ATT_DATA_ELIMINAZIONE")
                    Dim att_ute_id_eliminazione As Integer = idr.GetOrdinal("ATT_UTE_ID_ELIMINAZIONE")
                    Dim att_scu_codice As Integer = idr.GetOrdinal("ATT_SCU_CODICE")
                    Dim scu_descrizione As Integer = idr.GetOrdinal("SCU_DESCRIZIONE")

                    Dim item As Entities.AttivitaRegistrazione = Nothing

                    While idr.Read()

                        item = New Entities.AttivitaRegistrazione()

                        item.Id = idr.GetInt32OrDefault(att_id)
                        item.CodiceAttivitaTipo = idr.GetStringOrDefault(att_tat_codice)
                        item.DescrizioneAttivitaTipo = idr.GetStringOrDefault(tat_descrizione)
                        item.IdAttivitaAnagrafe = idr.GetInt32OrDefault(att_ata_id)
                        item.CodiceAttivitaAnagrafe = idr.GetStringOrDefault(ata_codice)
                        item.DescrizioneAttivitaAnagrafe = idr.GetStringOrDefault(ata_descrizione)
                        item.DataAttivita = idr.GetDateTime(att_data_attivita)
                        item.DataRegistrazione = idr.GetDateTime(att_data_registrazione)
                        item.IdUtenteRegistrazione = idr.GetInt64(att_ute_id_registrazione)
                        item.CodiceUtenteRegistrazione = idr.GetStringOrDefault(ute_codice)
                        item.DataVariazione = idr.GetDateTimeOrDefault(att_data_variazione)
                        item.IdUtenteVariazione = idr.GetNullableInt64OrDefault(att_ute_id_variazione)
                        item.DataEliminazione = idr.GetDateTimeOrDefault(att_data_eliminazione)
                        item.IdUtenteEliminazione = idr.GetNullableInt64OrDefault(att_ute_id_eliminazione)
                        item.CodiceScuola = idr.GetStringOrDefault(att_scu_codice)
                        item.DescrizioneScuola = idr.GetStringOrDefault(scu_descrizione)

                        list.Add(item)

                    End While

                End If

            End Using

            Return list

        End Function

        Private Function GetCampoAttivitaRegistrazione(campo As Entities.AttivitaRegistrazione.Ordinamento) As String

            Select Case campo

                Case Entities.AttivitaRegistrazione.Ordinamento.CodiceAttivitaTipo
                    Return "ATT_TAT_CODICE"

                Case Entities.AttivitaRegistrazione.Ordinamento.DescrizioneAttivitaTipo
                    Return "TAT_DESCRIZIONE"

                Case Entities.AttivitaRegistrazione.Ordinamento.CodiceAttivitaAnagrafe
                    Return "ATA_CODICE"

                Case Entities.AttivitaRegistrazione.Ordinamento.DescrizioneAttivitaAnagrafe
                    Return "ATA_DESCRIZIONE"

                Case Entities.AttivitaRegistrazione.Ordinamento.CodiceUtenteRegistrazione
                    Return "UTE_CODICE"

                Case Entities.AttivitaRegistrazione.Ordinamento.DataAttivita
                    Return "ATT_DATA_ATTIVITA"

                Case Entities.AttivitaRegistrazione.Ordinamento.DataRegistrazione
                    Return "ATT_DATA_REGISTRAZIONE"

                Case Entities.AttivitaRegistrazione.Ordinamento.Id
                    Return "ATT_ID"

                Case Entities.AttivitaRegistrazione.Ordinamento.CodiceScuola
                    Return "ATT_SCU_CODICE"

                Case Entities.AttivitaRegistrazione.Ordinamento.DescrizioneScuola
                    Return "SCU_DESCRIZIONE"

                Case Else
                    Throw New NotImplementedException("Entities.AttivitaRegistrazione.Ordinamento: valore non previsto")

            End Select

            Return String.Empty

        End Function

        Private Function GetListAttivitaRegistrazioneValore(cmd As OracleCommand) As List(Of Entities.AttivitaRegistrazioneValore)

            Dim list As New List(Of Entities.AttivitaRegistrazioneValore)()

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    Dim att_id As Integer = idr.GetOrdinal("ATT_ID")
                    Dim atv_id As Integer = idr.GetOrdinal("ATV_ID")
                    Dim aav_id As Integer = idr.GetOrdinal("AAV_ID")
                    Dim aav_oss_codice As Integer = idr.GetOrdinal("AAV_OSS_CODICE")
                    Dim oss_descrizione As Integer = idr.GetOrdinal("OSS_DESCRIZIONE")
                    Dim oss_tipo_risposta As Integer = idr.GetOrdinal("OSS_TIPO_RISPOSTA")
                    Dim aav_ordine As Integer = idr.GetOrdinal("AAV_ORDINE")
                    Dim atv_ris_codice As Integer = idr.GetOrdinal("ATV_RIS_CODICE")
                    Dim valore As Integer = idr.GetOrdinal("VALORE")
                    Dim atv_data_registrazione As Integer = idr.GetOrdinal("ATV_DATA_REGISTRAZIONE")
                    Dim atv_ute_id_registrazione As Integer = idr.GetOrdinal("ATV_UTE_ID_REGISTRAZIONE")
                    Dim atv_data_variazione As Integer = idr.GetOrdinal("ATV_DATA_VARIAZIONE")
                    Dim atv_ute_id_variazione As Integer = idr.GetOrdinal("ATV_UTE_ID_VARIAZIONE")
                    Dim aav_obbligatorio As Integer = idr.GetOrdinal("AAV_OBBLIGATORIO")
                    Dim oss_tipo_dati_risposta As Integer = idr.GetOrdinal("OSS_TIPO_DATI_RISPOSTA")

                    Dim item As Entities.AttivitaRegistrazioneValore = Nothing

                    While idr.Read()

                        item = New Entities.AttivitaRegistrazioneValore()

                        item.IdAttivitaRegistrazione = idr.GetInt32OrDefault(att_id)
                        item.IdAttivitaRegistrazioneValore = idr.GetInt64OrDefault(atv_id)
                        item.IdAttivitaVariabile = idr.GetInt32OrDefault(aav_id)
                        item.CodiceVariabile = idr.GetStringOrDefault(aav_oss_codice)
                        item.DescrizioneVariabile = idr.GetStringOrDefault(oss_descrizione)
                        item.TipoRisposta = idr.GetStringOrDefault(oss_tipo_risposta)
                        item.Ordine = idr.GetInt32OrDefault(aav_ordine)
                        item.CodiceRisposta = idr.GetStringOrDefault(atv_ris_codice)
                        item.ValoreRisposta = idr.GetStringOrDefault(valore)
                        item.DataRegistrazione = idr.GetNullableDateTimeOrDefault(atv_data_registrazione)
                        item.IdUtenteRegistrazione = idr.GetNullableInt64OrDefault(atv_ute_id_registrazione)
                        item.DataVariazione = idr.GetNullableDateTimeOrDefault(atv_data_variazione)
                        item.IdUtenteVariazione = idr.GetNullableInt64OrDefault(atv_ute_id_variazione)
                        item.Obbligatorio = idr.GetBooleanOrDefault(aav_obbligatorio)
                        item.TipoDatiRisposta = idr.GetStringOrDefault(oss_tipo_dati_risposta)

                        list.Add(item)

                    End While

                End If

            End Using

            Return list

        End Function

#End Region

#End Region

#Region " Operatori "

        ''' <summary>
        ''' Elenco operatori associati all'attività registrata specificata
        ''' </summary>
        ''' <param name="idAttivitaRegistrazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetOperatoriAttivitaRegistrazione(idAttivitaRegistrazione As Integer) As IEnumerable(Of Entities.AttivitaRegistrazioneOperatore) Implements IAttivitaProvider.GetOperatoriAttivitaRegistrazione

            Dim list As New List(Of Entities.AttivitaRegistrazioneOperatore)()

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                Dim query As New System.Text.StringBuilder()
                query.Append(" SELECT AOP_ATT_ID, AOP_OPP_ID, OPP_COGNOME, OPP_NOME, OPP_CF, ")
                query.Append(" OPP_QUA_ID, QUA_CODICE, QUA_DESCRIZIONE, ")
                query.Append(" OPP_UOP_ID, UOP_CODICE, UOP_DESCRIZIONE ")
                query.Append(" FROM T_ATTIVITA_OPERATORI ")
                query.Append(" JOIN T_ANA_OPERATORI_PPA ON AOP_OPP_ID = OPP_ID ")
                query.Append(" LEFT JOIN T_ANA_QUALIFICHE ON OPP_QUA_ID = QUA_ID ")
                query.Append(" LEFT JOIN T_ANA_UNITA_OPERATIVE ON OPP_UOP_ID = UOP_ID ")
                query.Append(" WHERE AOP_ATT_ID = :AOP_ATT_ID ")
                query.Append(" ORDER BY OPP_COGNOME, OPP_NOME, AOP_OPP_ID ")

                cmd.Parameters.AddWithValue("AOP_ATT_ID", idAttivitaRegistrazione)

                cmd.CommandText = query.ToString()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    list = GetListOperatoriAttivitaRegistrazione(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list.AsEnumerable()

        End Function

        ''' <summary>
        ''' Inserimento operatore associato all'attività registrata
        ''' </summary>
        ''' <param name="idAttivitaRegistrazione"></param>
        ''' <param name="idOperatore"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertOperatoreAttivitaRegistrazione(idAttivitaRegistrazione As Integer, idOperatore As Integer) As Integer Implements IAttivitaProvider.InsertOperatoreAttivitaRegistrazione

            Dim count As Integer = 0

            Using cmd As New OracleCommand("INSERT INTO T_ATTIVITA_OPERATORI (AOP_ATT_ID, AOP_OPP_ID) VALUES (:AOP_ATT_ID, :AOP_OPP_ID)", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("AOP_ATT_ID", idAttivitaRegistrazione)
                    cmd.Parameters.AddWithValue("AOP_OPP_ID", idOperatore)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Cancellazione dell'associazione tra operatore e attività registrata
        ''' </summary>
        ''' <param name="idAttivitaRegistrazione"></param>
        ''' <param name="idOperatore"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteOperatoreAttivitaRegistrazione(idAttivitaRegistrazione As Integer, idOperatore As Integer) As Integer Implements IAttivitaProvider.DeleteOperatoreAttivitaRegistrazione

            Dim count As Integer = 0

            Using cmd As New OracleCommand("DELETE FROM T_ATTIVITA_OPERATORI WHERE AOP_ATT_ID = :AOP_ATT_ID AND AOP_OPP_ID = :AOP_OPP_ID", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("AOP_ATT_ID", idAttivitaRegistrazione)
                    cmd.Parameters.AddWithValue("AOP_OPP_ID", idOperatore)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Elenco di tutti gli operatori PPA
        ''' </summary>
        ''' <param name="idAttivitaRegistrazione"></param>
        ''' <param name="idOperatoriDaEscludere"></param>
        ''' <param name="filtroGenerico"></param>
        ''' <param name="filtroSoloValidi"></param>
        ''' <param name="campoOrdinamento"></param>
        ''' <param name="versoOrdinamento"></param>
        ''' <param name="pagingOptions"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetOperatoriPPA(idAttivitaRegistrazione As Integer?, idOperatoriDaEscludere As IEnumerable(Of Integer), filtroGenerico As String, filtroSoloValidi As Boolean, campoOrdinamento As Entities.AttivitaVariabileDaAssociare.Ordinamento?, versoOrdinamento As String, pagingOptions As Onit.OnAssistnet.Data.PagingOptions) As IEnumerable(Of Entities.AttivitaOperatorePPA) Implements IAttivitaProvider.GetOperatoriPPA

            Dim list As List(Of Entities.AttivitaOperatorePPA) = Nothing

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                SetCommandSelectOperatoriPPA(cmd, idAttivitaRegistrazione, idOperatoriDaEscludere, filtroGenerico, filtroSoloValidi, False)

                ' Ordinamento
                If campoOrdinamento.HasValue Then

                    cmd.CommandText += String.Format(" ORDER BY {0}", GetCampoOperatorePPA(campoOrdinamento.Value, versoOrdinamento))

                End If

                ' Paginazione
                If Not pagingOptions Is Nothing Then

                    cmd.AddPaginatedQuery(pagingOptions)

                End If

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    list = GetListOperatoriPPA(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list.AsEnumerable()

        End Function

        ''' <summary>
        ''' Conteggio degli operatori PPA
        ''' </summary>
        ''' <param name="idAttivitaRegistrazione"></param>
        ''' <param name="idOperatoriDaEscludere"></param>
        ''' <param name="filtroGenerico"></param>
        ''' <param name="filtroSoloValidi"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CountOperatoriPPA(idAttivitaRegistrazione As Integer?, idOperatoriDaEscludere As IEnumerable(Of Integer), filtroGenerico As String, filtroSoloValidi As Boolean) As Integer Implements IAttivitaProvider.CountOperatoriPPA

            Dim count As Integer = 0

            Using cmd As New OracleCommand()

                cmd.Connection = Me.Connection

                ' Query di count con eventuali filtri
                SetCommandSelectOperatoriPPA(cmd, idAttivitaRegistrazione, idOperatoriDaEscludere, filtroGenerico, filtroSoloValidi, True)

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
        ''' Restituisce l'operatore PPA specificato
        ''' </summary>
        ''' <param name="idOperatore"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetOperatorePPA(idOperatore As Integer) As Entities.AttivitaOperatorePPA Implements IAttivitaProvider.GetOperatorePPA

            Dim item As Entities.AttivitaOperatorePPA = Nothing

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                cmd.CommandText =
                    GetQuerySelectOperatoriPPA(False) +
                    " WHERE OPP_ID = :OPP_ID "

                cmd.Parameters.AddWithValue("OPP_ID", idOperatore)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim list As List(Of Entities.AttivitaOperatorePPA) = GetListOperatoriPPA(cmd)

                    If Not list.IsNullOrEmpty() Then
                        item = list.First()
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return item

        End Function

        ''' <summary>
        ''' Restituisce tutti gli operatori PPA presenti in anagrafica, in base a cognome, nome, unità operativa e qualifica specificati.
        ''' </summary>
        ''' <param name="cognome"></param>
        ''' <param name="nome"></param>
        ''' <param name="idUnitaOperativa"></param>
        ''' <param name="idQualifica"></param>
        ''' <param name="filtroSoloValidi"></param>
        ''' <returns></returns>
        Public Function GetOperatoriPPA(cognome As String, nome As String, idUnitaOperativa As Integer?, idQualifica As Integer?, filtroSoloValidi As Boolean) As IEnumerable(Of Entities.AttivitaOperatorePPA) Implements IAttivitaProvider.GetOperatoriPPA

            Dim list As List(Of Entities.AttivitaOperatorePPA) = Nothing

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                Dim query As New System.Text.StringBuilder()

                query.Append(GetQuerySelectOperatoriPPA(False))

                Dim filtro As New System.Text.StringBuilder()

                If Not String.IsNullOrWhiteSpace(cognome) Then
                    filtro.Append(" UPPER(OPP_COGNOME) LIKE UPPER(:OPP_COGNOME) AND ")
                    cmd.Parameters.AddWithValue("OPP_COGNOME", cognome)
                End If

                If Not String.IsNullOrWhiteSpace(nome) Then
                    filtro.Append(" UPPER(OPP_NOME) LIKE UPPER(:OPP_NOME) AND ")
                    cmd.Parameters.AddWithValue("OPP_NOME", nome)
                End If

                If idUnitaOperativa.HasValue Then
                    filtro.Append(" OPP_UOP_ID = :OPP_UOP_ID AND ")
                    cmd.Parameters.AddWithValue("OPP_UOP_ID", idUnitaOperativa.Value)
                End If

                If idQualifica.HasValue Then
                    filtro.Append(" OPP_QUA_ID = :OPP_QUA_ID AND ")
                    cmd.Parameters.AddWithValue("OPP_QUA_ID", idQualifica.Value)
                End If

                ' Non carica gli obsoleti
                If filtroSoloValidi Then
                    filtro.Append(" OPP_OBSOLETO = :OPP_OBSOLETO AND ")
                    cmd.Parameters.AddWithValue("OPP_OBSOLETO", "N")
                End If

                If filtro.Length > 0 Then
                    filtro.RemoveLast(4)
                    query.AppendFormat(" WHERE {0}", filtro.ToString())
                End If

                cmd.CommandText = query.ToString()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    list = GetListOperatoriPPA(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list.AsEnumerable()

        End Function

        ''' <summary>
        ''' Restituisce il valore della sequence per l'id dell'operatore PPA da inserire
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetNextIdAttivitaOperatorePPA() As Integer Implements IAttivitaProvider.GetNextIdAttivitaOperatorePPA

            Dim nextVal As Integer = 0

            Using cmd As New OracleCommand("SELECT SEQ_OPERATORI_PPA.NEXTVAL FROM DUAL", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then
                        nextVal = Convert.ToInt32(obj)
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return nextVal

        End Function

        ''' <summary>
        ''' Inserimento operatore PPA in anagrafica
        ''' </summary>
        ''' <param name="operatorePPA"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertOperatorePPA(operatorePPA As AttivitaOperatorePPA) As Integer Implements IAttivitaProvider.InsertOperatorePPA

            Dim count As Integer = 0

            Using cmd As New OracleCommand("INSERT INTO T_ANA_OPERATORI_PPA (OPP_ID, OPP_COGNOME, OPP_NOME, OPP_CF, OPP_UOP_ID, OPP_QUA_ID, OPP_OBSOLETO) VALUES (:OPP_ID, :OPP_COGNOME, :OPP_NOME, :OPP_CF, :OPP_UOP_ID, :OPP_QUA_ID, :OPP_OBSOLETO)", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("OPP_ID", operatorePPA.IdOperatore.Value)
                    cmd.Parameters.AddWithValue("OPP_COGNOME", operatorePPA.Cognome)
                    cmd.Parameters.AddWithValue("OPP_NOME", operatorePPA.Nome)
                    cmd.Parameters.AddWithValue("OPP_CF", GetStringParam(operatorePPA.CodiceFiscale))
                    cmd.Parameters.AddWithValue("OPP_UOP_ID", GetIntParam(operatorePPA.IdUnitaOperativa))
                    cmd.Parameters.AddWithValue("OPP_QUA_ID", GetIntParam(operatorePPA.IdQualifica))

                    If operatorePPA.Obsoleto Then
                        cmd.Parameters.AddWithValue("OPP_OBSOLETO", "S")
                    Else
                        cmd.Parameters.AddWithValue("OPP_OBSOLETO", "N")
                    End If

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Restituisce la lista di qualifiche in base ai filtri impostati
        ''' </summary>
        ''' <param name="filtroGenerico"></param>
        ''' <param name="filtroSoloValidi"></param>
        ''' <param name="campoOrdinamento"></param>
        ''' <param name="versoOrdinamento"></param>
        ''' <param name="pagingOptions"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetQualifiche(filtroGenerico As String, filtroSoloValidi As Boolean, campoOrdinamento As Entities.Qualifica.Ordinamento?, versoOrdinamento As String, pagingOptions As Onit.OnAssistnet.Data.PagingOptions) As IEnumerable(Of Entities.Qualifica) Implements IAttivitaProvider.GetQualifiche

            Dim list As List(Of Entities.Qualifica) = Nothing

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                SetCommandSelectQualifiche(cmd, filtroGenerico, filtroSoloValidi, False)

                ' Ordinamento
                If campoOrdinamento.HasValue Then

                    cmd.CommandText += String.Format(" ORDER BY {0}", GetCampoQualifica(campoOrdinamento.Value, versoOrdinamento))

                End If

                ' Paginazione
                If Not pagingOptions Is Nothing Then

                    cmd.AddPaginatedQuery(pagingOptions)

                End If

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    list = GetListQualifiche(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list.AsEnumerable()

        End Function

        ''' <summary>
        ''' Conteggio qualifiche in base ai filtri impostati
        ''' </summary>
        ''' <param name="filtroGenerico"></param>
        ''' <param name="filtroSoloValidi"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CountQualifiche(filtroGenerico As String, filtroSoloValidi As Boolean) As Integer Implements IAttivitaProvider.CountQualifiche

            Dim count As Integer = 0

            Using cmd As New OracleCommand()

                cmd.Connection = Me.Connection

                ' Query di count con eventuali filtri
                SetCommandSelectQualifiche(cmd, filtroGenerico, filtroSoloValidi, True)

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
        ''' Restituisce la lista di unità operative in base ai filtri impostati
        ''' </summary>
        ''' <param name="filtroGenerico"></param>
        ''' <param name="filtroSoloValidi"></param>
        ''' <param name="campoOrdinamento"></param>
        ''' <param name="versoOrdinamento"></param>
        ''' <param name="pagingOptions"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetUnitaOperative(filtroGenerico As String, filtroSoloValidi As Boolean, campoOrdinamento As Entities.UnitaOperativa.Ordinamento?, versoOrdinamento As String, pagingOptions As Onit.OnAssistnet.Data.PagingOptions) As IEnumerable(Of Entities.UnitaOperativa) Implements IAttivitaProvider.GetUnitaOperative

            Dim list As List(Of Entities.UnitaOperativa) = Nothing

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                SetCommandSelectUnitaOperative(cmd, filtroGenerico, filtroSoloValidi, False)

                ' Ordinamento
                If campoOrdinamento.HasValue Then

                    cmd.CommandText += String.Format(" ORDER BY {0}", GetCampoUnitaOperativa(campoOrdinamento.Value, versoOrdinamento))

                End If

                ' Paginazione
                If Not pagingOptions Is Nothing Then

                    cmd.AddPaginatedQuery(pagingOptions)

                End If

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    list = GetListUnitaOperative(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list.AsEnumerable()

        End Function

        ''' <summary>
        ''' Conteggio unita operative in base ai filtri impostati
        ''' </summary>
        ''' <param name="filtroGenerico"></param>
        ''' <param name="filtroSoloValidi"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CountUnitaOperative(filtroGenerico As String, filtroSoloValidi As Boolean) As Integer Implements IAttivitaProvider.CountUnitaOperative

            Dim count As Integer = 0

            Using cmd As New OracleCommand()

                cmd.Connection = Me.Connection

                ' Query di count con eventuali filtri
                SetCommandSelectUnitaOperative(cmd, filtroGenerico, filtroSoloValidi, True)

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

#Region " Private "

        Private Function GetListOperatoriAttivitaRegistrazione(cmd As OracleCommand) As List(Of Entities.AttivitaRegistrazioneOperatore)

            Dim list As New List(Of Entities.AttivitaRegistrazioneOperatore)()

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    Dim aop_att_id As Integer = idr.GetOrdinal("AOP_ATT_ID")
                    Dim aop_opp_id As Integer = idr.GetOrdinal("AOP_OPP_ID")
                    Dim opp_cognome As Integer = idr.GetOrdinal("OPP_COGNOME")
                    Dim opp_nome As Integer = idr.GetOrdinal("OPP_NOME")
                    Dim opp_cf As Integer = idr.GetOrdinal("OPP_CF")
                    Dim opp_uop_id As Integer = idr.GetOrdinal("OPP_UOP_ID")
                    Dim uop_codice As Integer = idr.GetOrdinal("UOP_CODICE")
                    Dim uop_descrizione As Integer = idr.GetOrdinal("UOP_DESCRIZIONE")
                    Dim opp_qua_id As Integer = idr.GetOrdinal("OPP_QUA_ID")
                    Dim qua_codice As Integer = idr.GetOrdinal("QUA_CODICE")
                    Dim qua_descrizione As Integer = idr.GetOrdinal("QUA_DESCRIZIONE")

                    Dim item As Entities.AttivitaRegistrazioneOperatore = Nothing

                    While idr.Read()

                        item = New Entities.AttivitaRegistrazioneOperatore()

                        item.IdAttivitaRegistrazione = idr.GetInt32(aop_att_id)
                        item.IdOperatore = idr.GetInt32(aop_opp_id)
                        item.Cognome = idr.GetString(opp_cognome)
                        item.Nome = idr.GetString(opp_nome)
                        item.CodiceFiscale = idr.GetStringOrDefault(opp_cf)
                        item.IdUnitaOperativa = idr.GetNullableInt32OrDefault(opp_uop_id)
                        item.CodiceUnitaOperativa = idr.GetStringOrDefault(uop_codice)
                        item.DescrizioneUnitaOperativa = idr.GetStringOrDefault(uop_descrizione)
                        item.IdQualifica = idr.GetNullableInt32OrDefault(opp_qua_id)
                        item.CodiceQualifica = idr.GetStringOrDefault(qua_codice)
                        item.DescrizioneQualifica = idr.GetStringOrDefault(qua_descrizione)

                        list.Add(item)

                    End While

                End If

            End Using

            Return list

        End Function

        Private Function GetQuerySelectOperatoriPPA(isCount As Boolean) As String

            Dim query As New System.Text.StringBuilder()

            If isCount Then
                query.Append("SELECT COUNT(OPP_ID) ")
            Else
                query.Append("SELECT OPP_ID, OPP_COGNOME, OPP_NOME, OPP_CF, OPP_UOP_ID, OPP_QUA_ID, OPP_OBSOLETO, ")
                query.Append("QUA_CODICE, QUA_DESCRIZIONE, UOP_CODICE, UOP_DESCRIZIONE ")
            End If

            query.Append(" FROM T_ANA_OPERATORI_PPA ")

            If Not isCount Then
                query.Append(" LEFT JOIN T_ANA_QUALIFICHE ON OPP_QUA_ID = QUA_ID ")
                query.Append(" LEFT JOIN T_ANA_UNITA_OPERATIVE ON OPP_UOP_ID = UOP_ID ")
            End If

            Return query.ToString()

        End Function

        Private Sub SetCommandSelectOperatoriPPA(cmd As OracleCommand, idAttivitaRegistrazione As Integer?, idOperatoriDaEscludere As IEnumerable(Of Integer), filtroGenerico As String, filtroSoloValidi As Boolean, isCount As Boolean)

            Dim query As New System.Text.StringBuilder()

            query.Append(GetQuerySelectOperatoriPPA(isCount))

            Dim filtro As New System.Text.StringBuilder()

            ' Non carica gli operatori già associati (in base all'id dell'attività)
            If idAttivitaRegistrazione.HasValue Then
                filtro.Append(" NOT EXISTS (SELECT 1 FROM T_ATTIVITA_OPERATORI WHERE AOP_OPP_ID = OPP_ID AND AOP_ATT_ID = :AOP_ATT_ID) AND ")
                cmd.Parameters.AddWithValue("AOP_ATT_ID", idAttivitaRegistrazione.Value)
            End If

            ' Non carica gli operatori già associati (in base all'id degli operatori)
            If Not idOperatoriDaEscludere.IsNullOrEmpty() Then
                Dim filter As GetInFilterResult = GetInFilter(idOperatoriDaEscludere.ToList())
                filtro.AppendFormat(" OPP_ID NOT IN ({0}) AND ", filter.InFilter)
                cmd.Parameters.AddRange(filter.Parameters)
            End If

            ' Non carica gli obsoleti
            If filtroSoloValidi Then
                filtro.Append(" OPP_OBSOLETO = :OPP_OBSOLETO AND ")
                cmd.Parameters.AddWithValue("OPP_OBSOLETO", "N")
            End If

            If Not String.IsNullOrWhiteSpace(filtroGenerico) Then

                filtro.Append(" (UPPER(OPP_COGNOME) LIKE UPPER(:OPP_COGNOME) OR UPPER(OPP_NOME) LIKE UPPER(:OPP_NOME)) AND ")

                Dim filtroLike As String = String.Format("%{0}%", filtroGenerico)

                cmd.Parameters.AddWithValue("OPP_COGNOME", filtroLike)
                cmd.Parameters.AddWithValue("OPP_NOME", filtroLike)

            End If

            If filtro.Length > 0 Then
                filtro.RemoveLast(4)
                query.AppendFormat(" WHERE {0}", filtro.ToString())
            End If

            cmd.CommandText = query.ToString()

        End Sub

        Private Function GetCampoOperatorePPA(campo As Entities.AttivitaOperatorePPA.Ordinamento, verso As String) As String

            Select Case campo

                Case Entities.AttivitaOperatorePPA.Ordinamento.Cognome
                    Return String.Format("OPP_COGNOME {0}, OPP_NOME {0}, OPP_ID", verso)

                Case Entities.AttivitaOperatorePPA.Ordinamento.Nome
                    Return String.Format("OPP_NOME {0}, OPP_COGNOME {0}, OPP_ID", verso)

                Case Else
                    Throw New NotImplementedException("Entities.AttivitaOperatorePPA.Ordinamento: valore non previsto")

            End Select

            Return String.Empty

        End Function

        Private Function GetListOperatoriPPA(cmd As OracleCommand) As List(Of Entities.AttivitaOperatorePPA)

            Dim list As New List(Of Entities.AttivitaOperatorePPA)()

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    Dim opp_id As Integer = idr.GetOrdinal("OPP_ID")
                    Dim opp_cognome As Integer = idr.GetOrdinal("OPP_COGNOME")
                    Dim opp_nome As Integer = idr.GetOrdinal("OPP_NOME")
                    Dim opp_cf As Integer = idr.GetOrdinal("OPP_CF")
                    Dim opp_uop_id As Integer = idr.GetOrdinal("OPP_UOP_ID")
                    Dim uop_codice As Integer = idr.GetOrdinal("UOP_CODICE")
                    Dim uop_descrizione As Integer = idr.GetOrdinal("UOP_DESCRIZIONE")
                    Dim opp_qua_id As Integer = idr.GetOrdinal("OPP_QUA_ID")
                    Dim qua_codice As Integer = idr.GetOrdinal("QUA_CODICE")
                    Dim qua_descrizione As Integer = idr.GetOrdinal("QUA_DESCRIZIONE")
                    Dim opp_obsoleto As Integer = idr.GetOrdinal("OPP_OBSOLETO")

                    Dim item As Entities.AttivitaOperatorePPA = Nothing

                    While idr.Read()

                        item = New Entities.AttivitaOperatorePPA()

                        item.IdOperatore = idr.GetInt32(opp_id)
                        item.Cognome = idr.GetString(opp_cognome)
                        item.Nome = idr.GetString(opp_nome)
                        item.CodiceFiscale = idr.GetStringOrDefault(opp_cf)
                        item.IdUnitaOperativa = idr.GetNullableInt32OrDefault(opp_uop_id)
                        item.CodiceUnitaOperativa = idr.GetStringOrDefault(uop_codice)
                        item.DescrizioneUnitaOperativa = idr.GetStringOrDefault(uop_descrizione)
                        item.IdQualifica = idr.GetNullableInt32OrDefault(opp_qua_id)
                        item.CodiceQualifica = idr.GetStringOrDefault(qua_codice)
                        item.DescrizioneQualifica = idr.GetStringOrDefault(qua_descrizione)
                        item.Obsoleto = idr.GetBooleanOrDefault(opp_obsoleto)

                        list.Add(item)

                    End While

                End If

            End Using

            Return list

        End Function

        Private Function GetQuerySelectQualifiche(isCount As Boolean) As String

            Dim query As New System.Text.StringBuilder()

            If isCount Then
                query.Append("SELECT COUNT(QUA_ID) ")
            Else
                query.Append("SELECT QUA_ID, QUA_CODICE, QUA_DESCRIZIONE, QUA_OBSOLETO ")
            End If

            query.Append(" FROM T_ANA_QUALIFICHE ")

            Return query.ToString()

        End Function

        Private Sub SetCommandSelectQualifiche(cmd As OracleCommand, filtroGenerico As String, filtroSoloValidi As Boolean, isCount As Boolean)

            Dim query As New System.Text.StringBuilder()

            query.Append(GetQuerySelectQualifiche(isCount))

            Dim filtro As New System.Text.StringBuilder()

            ' Non carica gli obsoleti
            If filtroSoloValidi Then
                filtro.Append(" QUA_OBSOLETO = :QUA_OBSOLETO AND ")
                cmd.Parameters.AddWithValue("QUA_OBSOLETO", "N")
            End If

            If Not String.IsNullOrWhiteSpace(filtroGenerico) Then

                filtro.Append(" (UPPER(QUA_CODICE) LIKE UPPER(:QUA_CODICE) OR UPPER(QUA_DESCRIZIONE) LIKE UPPER(:QUA_DESCRIZIONE)) AND ")

                Dim filtroLike As String = String.Format("%{0}%", filtroGenerico)

                cmd.Parameters.AddWithValue("QUA_CODICE", filtroLike)
                cmd.Parameters.AddWithValue("QUA_DESCRIZIONE", filtroLike)

            End If

            If filtro.Length > 0 Then
                filtro.RemoveLast(4)
                query.AppendFormat(" WHERE {0}", filtro.ToString())
            End If

            cmd.CommandText = query.ToString()

        End Sub

        Private Function GetCampoQualifica(campo As Entities.Qualifica.Ordinamento, verso As String) As String

            Select Case campo

                Case Entities.Qualifica.Ordinamento.Codice
                    Return String.Format("QUA_CODICE {0}, QUA_ID", verso)

                Case Entities.Qualifica.Ordinamento.Descrizione
                    Return String.Format("QUA_DESCRIZIONE {0}, QUA_ID", verso)

                Case Else
                    Throw New NotImplementedException("Entities.Qualifica.Ordinamento: valore non previsto")

            End Select

            Return String.Empty

        End Function

        Private Function GetListQualifiche(cmd As OracleCommand) As List(Of Entities.Qualifica)

            Dim list As New List(Of Entities.Qualifica)()

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    Dim qua_id As Integer = idr.GetOrdinal("QUA_ID")
                    Dim qua_codice As Integer = idr.GetOrdinal("QUA_CODICE")
                    Dim qua_descrizione As Integer = idr.GetOrdinal("QUA_DESCRIZIONE")
                    Dim qua_obsoleto As Integer = idr.GetOrdinal("QUA_OBSOLETO")

                    Dim item As Entities.Qualifica = Nothing

                    While idr.Read()

                        item = New Entities.Qualifica()

                        item.Id = idr.GetInt32(qua_id)
                        item.Codice = idr.GetString(qua_codice)
                        item.Descrizione = idr.GetString(qua_descrizione)
                        item.Obsoleto = idr.GetBooleanOrDefault(qua_obsoleto)

                        list.Add(item)

                    End While

                End If

            End Using

            Return list

        End Function

        Private Function GetListUnitaOperative(cmd As OracleCommand) As List(Of Entities.UnitaOperativa)

            Dim list As New List(Of Entities.UnitaOperativa)()

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    Dim uop_id As Integer = idr.GetOrdinal("UOP_ID")
                    Dim uop_codice As Integer = idr.GetOrdinal("UOP_CODICE")
                    Dim uop_descrizione As Integer = idr.GetOrdinal("UOP_DESCRIZIONE")
                    Dim uop_obsoleto As Integer = idr.GetOrdinal("UOP_OBSOLETO")

                    Dim item As Entities.UnitaOperativa = Nothing

                    While idr.Read()

                        item = New Entities.UnitaOperativa()

                        item.Id = idr.GetInt32(uop_id)
                        item.Codice = idr.GetString(uop_codice)
                        item.Descrizione = idr.GetString(uop_descrizione)
                        item.Obsoleto = idr.GetBooleanOrDefault(uop_obsoleto)

                        list.Add(item)

                    End While

                End If

            End Using

            Return list

        End Function

        Private Function GetCampoUnitaOperativa(campo As Entities.UnitaOperativa.Ordinamento, verso As String) As String

            Select Case campo

                Case Entities.UnitaOperativa.Ordinamento.Codice
                    Return String.Format("UOP_CODICE {0}, UOP_ID", verso)

                Case Entities.UnitaOperativa.Ordinamento.Descrizione
                    Return String.Format("UOP_DESCRIZIONE {0}, UOP_ID", verso)

                Case Else
                    Throw New NotImplementedException("Entities.UnitaOperativa.Ordinamento: valore non previsto")

            End Select

            Return String.Empty

        End Function

        Private Function GetQuerySelectUnitaOperative(isCount As Boolean) As String

            Dim query As New System.Text.StringBuilder()

            If isCount Then
                query.Append("SELECT COUNT(UOP_ID) ")
            Else
                query.Append("SELECT UOP_ID, UOP_CODICE, UOP_DESCRIZIONE, UOP_OBSOLETO ")
            End If

            query.Append(" FROM T_ANA_UNITA_OPERATIVE ")

            Return query.ToString()

        End Function

        Private Sub SetCommandSelectUnitaOperative(cmd As OracleCommand, filtroGenerico As String, filtroSoloValidi As Boolean, isCount As Boolean)

            Dim query As New System.Text.StringBuilder()

            query.Append(GetQuerySelectUnitaOperative(isCount))

            Dim filtro As New System.Text.StringBuilder()

            ' Non carica gli obsoleti
            If filtroSoloValidi Then
                filtro.Append(" UOP_OBSOLETO = :UOP_OBSOLETO AND ")
                cmd.Parameters.AddWithValue("UOP_OBSOLETO", "N")
            End If

            If Not String.IsNullOrWhiteSpace(filtroGenerico) Then

                filtro.Append(" (UPPER(UOP_CODICE) LIKE UPPER(:UOP_CODICE) OR UPPER(UOP_DESCRIZIONE) LIKE UPPER(:UOP_DESCRIZIONE)) AND ")

                Dim filtroLike As String = String.Format("%{0}%", filtroGenerico)

                cmd.Parameters.AddWithValue("UOP_CODICE", filtroLike)
                cmd.Parameters.AddWithValue("UOP_DESCRIZIONE", filtroLike)

            End If

            If filtro.Length > 0 Then
                filtro.RemoveLast(4)
                query.AppendFormat(" WHERE {0}", filtro.ToString())
            End If

            cmd.CommandText = query.ToString()

        End Sub

#End Region

#End Region

#Region " Scuole "

        ''' <summary>
        ''' Restituisce le attività presenti in anagrafe, in base al filtro impostati.
        ''' </summary>
        ''' <param name="filtroGenerico"></param>
        ''' <param name="soloValidi"></param>
        ''' <param name="campoOrdinamento"></param>
        ''' <param name="versoOrdinamento"></param>
        ''' <param name="pagingOptions"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetElencoScuole(filtroGenerico As String, soloValidi As Boolean, campoOrdinamento As Entities.Scuola.Ordinamento?, versoOrdinamento As String, pagingOptions As Onit.OnAssistnet.Data.PagingOptions) As IEnumerable(Of Entities.Scuola) Implements IAttivitaProvider.GetElencoScuole

            Dim list As List(Of Entities.Scuola) = Nothing

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                ' Query di select con eventuali filtri
                SetCommandSelectScuole(cmd, filtroGenerico, soloValidi, False)

                ' Ordinamento
                If campoOrdinamento.HasValue Then

                    cmd.CommandText += String.Format(" ORDER BY {0} {1}", GetCampoScuola(campoOrdinamento.Value), versoOrdinamento)

                End If

                ' Paginazione
                If Not pagingOptions Is Nothing Then

                    cmd.AddPaginatedQuery(pagingOptions)

                End If

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    list = GetListScuole(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list.AsEnumerable()

        End Function

        ''' <summary>
        ''' Restituisce il numero di attività presenti in anagrafe, in base ai filtri impostati.
        ''' </summary>
        ''' <param name="filtroGenerico"></param>
        ''' <param name="soloValidi"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CountElencoScuole(filtroGenerico As String, soloValidi As Boolean) As Integer Implements IAttivitaProvider.CountElencoScuole

            Dim count As Integer = 0

            Using cmd As New OracleCommand()

                cmd.Connection = Me.Connection

                ' Query di select con eventuali filtri
                SetCommandSelectScuole(cmd, filtroGenerico, soloValidi, True)

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
        ''' Restituisce l'entity Scuola specificata
        ''' </summary>
        ''' <param name="codiceScuola"></param>
        ''' <returns></returns>
        Public Function GetScuola(codiceScuola As String) As Entities.Scuola Implements IAttivitaProvider.GetScuola

            Dim item As Entities.Scuola = Nothing

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection
                cmd.CommandText = GetQuerySelectScuole(False) + " WHERE SCU_CODICE = :SCU_CODICE "

                cmd.Parameters.AddWithValue("SCU_CODICE", codiceScuola)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim list As List(Of Entities.Scuola) = GetListScuole(cmd)
                    If Not list.IsNullOrEmpty() Then
                        item = list.First()
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return item

        End Function

#Region " Private "

        Private Function GetQuerySelectScuole(isCount As Boolean) As String

            Dim query As New System.Text.StringBuilder()

            If isCount Then
                query.Append("SELECT COUNT(SCU_CODICE) ")
            Else
                query.Append("SELECT SCU_CODICE, SCU_DESCRIZIONE, SCU_OBSOLETO ")
            End If

            query.Append(" FROM V_ANA_SCUOLE_PPA ")

            Return query.ToString()

        End Function

        Private Sub SetCommandSelectScuole(cmd As OracleCommand, filtroGenerico As String, soloValidi As Boolean, isCount As Boolean)

            Dim filtro As New System.Text.StringBuilder()

            Dim query As New System.Text.StringBuilder()

            If soloValidi Then
                filtro.Append(" SCU_OBSOLETO = :SCU_OBSOLETO AND ")
                cmd.Parameters.AddWithValue("SCU_OBSOLETO", "N")
            End If

            If Not String.IsNullOrWhiteSpace(filtroGenerico) Then

                filtro.Append(" (UPPER(SCU_CODICE) LIKE UPPER(:SCU_CODICE) OR UPPER(SCU_DESCRIZIONE) LIKE UPPER(:SCU_DESCRIZIONE)) AND ")

                Dim filtroLike As String = String.Format("%{0}%", filtroGenerico)

                cmd.Parameters.AddWithValue("SCU_CODICE", filtroLike)
                cmd.Parameters.AddWithValue("SCU_DESCRIZIONE", filtroLike)

            End If

            If filtro.Length > 0 Then
                filtro.RemoveLast(4)
                filtro.Insert(0, " WHERE ")
            End If

            cmd.CommandText = GetQuerySelectScuole(isCount) + query.ToString() + filtro.ToString()

        End Sub

        Private Function GetCampoScuola(campo As Entities.AttivitaAnagrafe.Ordinamento) As String

            Select Case campo

                Case Scuola.Ordinamento.Codice
                    Return "SCU_CODICE"

                Case Scuola.Ordinamento.Descrizione
                    Return "SCU_DESCRIZIONE"

                Case Scuola.Ordinamento.Obsoleto
                    Return "SCU_OBSOLETO"

                Case Else
                    Throw New NotImplementedException("Entities.Scuola.Ordinamento: valore non previsto")

            End Select

            Return String.Empty

        End Function

        Private Function GetListScuole(cmd As OracleCommand) As List(Of Entities.Scuola)

            Dim list As New List(Of Entities.Scuola)()

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    Dim scu_codice As Integer = idr.GetOrdinal("SCU_CODICE")
                    Dim scu_descrizione As Integer = idr.GetOrdinal("SCU_DESCRIZIONE")
                    Dim scu_obsoleto As Integer = idr.GetOrdinal("SCU_OBSOLETO")

                    Dim item As Entities.Scuola = Nothing

                    While idr.Read()

                        item = New Entities.Scuola()

                        item.Codice = idr.GetStringOrDefault(scu_codice)
                        item.Descrizione = idr.GetStringOrDefault(scu_descrizione)
                        item.Obsoleto = idr.GetBooleanOrDefault(scu_obsoleto)

                        list.Add(item)

                    End While

                End If

            End Using

            Return list

        End Function

#End Region

#End Region

#End Region

    End Class

End Namespace