Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.Data.OracleClient


Namespace DAL.Oracle

    Public Class DbLogNotificheProvider
        Inherits DbProvider
        Implements ILogNotificheProvider

#Region " Costruttori "

        Public Sub New(ByRef DAM As IDAM)
            MyBase.New(DAM)
        End Sub

#End Region

#Region " Public "

#Region " Notifiche Ricevute "

        ''' <summary>
        ''' Restituisce il log del messaggio di notifica indicato
        ''' </summary>
        ''' <param name="idMessaggio"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetLogNotificaRicevuta(idMessaggio As String) As LogNotificaRicevuta Implements ILogNotificheProvider.GetLogNotificaRicevuta

            Dim logNotificaRicevuta As LogNotificaRicevuta = Nothing

            Dim query As String =
                " SELECT LNR_ID, LNR_ID_MESSAGGIO, LNR_PAZ_CODICE_CENTRALE, LNR_PAZ_CODICE_CENTRALE_ALIAS, LNR_DATA_RICEZIONE, LNR_PAZIENTE, " +
                " LNR_DATA_INVIO_RISPOSTA, LNR_NUMERO_RICEZIONI, LNR_OPERAZIONE, LNR_RISULTATO_ELABORAZIONE, LNR_MESSAGGIO_ELABORAZIONE, LNR_ESENZIONI, LNR_SERVICE_REQUEST, LNR_SERVICE_RESPONSE, LNR_REPOSITORY_UNIQUE_ID, LNR_DOCUMENT_UNIQUE_ID, " +
                " LNR_ENTITY" +
                " FROM T_LOG_NOTIFICHE_RICEZIONE " +
                " WHERE LNR_ID_MESSAGGIO = :LNR_ID_MESSAGGIO"

            Using cmd As New OracleClient.OracleCommand(query, Connection)

                cmd.Parameters.AddWithValue("LNR_ID_MESSAGGIO", idMessaggio)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If idr IsNot Nothing Then

                            Dim lnr_id As Integer = idr.GetOrdinal("LNR_ID")
                            Dim lnr_id_messaggio As Integer = idr.GetOrdinal("LNR_ID_MESSAGGIO")
                            Dim lnr_paz_codice_centrale As Integer = idr.GetOrdinal("LNR_PAZ_CODICE_CENTRALE")
                            Dim lnr_paz_codice_centrale_alias As Integer = idr.GetOrdinal("LNR_PAZ_CODICE_CENTRALE_ALIAS")
                            Dim lnr_data_ricezione As Integer = idr.GetOrdinal("LNR_DATA_RICEZIONE")
                            Dim lnr_paziente As Integer = idr.GetOrdinal("LNR_PAZIENTE")
                            Dim lnr_data_invio_risposta As Integer = idr.GetOrdinal("LNR_DATA_INVIO_RISPOSTA")
                            Dim lnr_numero_ricezioni As Integer = idr.GetOrdinal("LNR_NUMERO_RICEZIONI")
                            Dim lnr_operazione As Integer = idr.GetOrdinal("LNR_OPERAZIONE")
                            Dim lnr_risultato_elaborazione As Integer = idr.GetOrdinal("LNR_RISULTATO_ELABORAZIONE")
                            Dim lnr_messaggio_elaborazione As Integer = idr.GetOrdinal("LNR_MESSAGGIO_ELABORAZIONE")
                            Dim lnr_esenzioni As Integer = idr.GetOrdinal("LNR_ESENZIONI")
                            Dim lnr_service_request As Integer = idr.GetOrdinal("LNR_SERVICE_REQUEST")
                            Dim lnr_service_response As Integer = idr.GetOrdinal("LNR_SERVICE_RESPONSE")
                            Dim lnr_repository_unique_id As Integer = idr.GetOrdinal("LNR_REPOSITORY_UNIQUE_ID")
                            Dim lnr_document_unique_id As Integer = idr.GetOrdinal("LNR_DOCUMENT_UNIQUE_ID")
                            Dim lnr_entity As Integer = idr.GetOrdinal("LNR_ENTITY")

                            If idr.Read() Then

                                logNotificaRicevuta = New LogNotificaRicevuta()

                                logNotificaRicevuta.IdNotifica = idr.GetInt64(lnr_id)
                                logNotificaRicevuta.IdMessaggio = idr.GetString(lnr_id_messaggio)
                                logNotificaRicevuta.CodiceCentralePaziente = idr.GetString(lnr_paz_codice_centrale)
                                logNotificaRicevuta.CodiceCentralePazienteAlias = idr.GetStringOrDefault(lnr_paz_codice_centrale_alias)
                                logNotificaRicevuta.DataRicezione = idr.GetDateTime(lnr_data_ricezione)
                                logNotificaRicevuta.EntityPazienteSerializzata = idr.GetStringOrDefault(lnr_paziente)
                                logNotificaRicevuta.DataInvioRisposta = idr.GetNullableDateTimeOrDefault(lnr_data_invio_risposta)
                                logNotificaRicevuta.NumeroRicezioni = idr.GetNullableInt32OrDefault(lnr_numero_ricezioni)
                                If idr.IsDBNull(lnr_operazione) Then
                                    logNotificaRicevuta.Operazione = Nothing
                                Else
                                    logNotificaRicevuta.Operazione = [Enum].Parse(GetType(Enumerators.OperazioneLogNotifica), idr.GetString(lnr_operazione), True)
                                End If
                                logNotificaRicevuta.RisultatoElaborazione = idr.GetStringOrDefault(lnr_risultato_elaborazione)
                                logNotificaRicevuta.MessaggioElaborazione = idr.GetStringOrDefault(lnr_messaggio_elaborazione)
                                logNotificaRicevuta.Esenzioni = idr.GetStringOrDefault(lnr_esenzioni)
                                logNotificaRicevuta.ServiceRequest = idr.GetStringOrDefault(lnr_service_request)
                                logNotificaRicevuta.ServiceResponse = idr.GetStringOrDefault(lnr_service_response)
                                logNotificaRicevuta.RepositoryUniqueId = idr.GetStringOrDefault(lnr_repository_unique_id)
                                logNotificaRicevuta.DocumentUniqueId = idr.GetStringOrDefault(lnr_document_unique_id)
                                logNotificaRicevuta.EntitySerializzata = idr.GetStringOrDefault(lnr_entity)

                            End If

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return logNotificaRicevuta

        End Function

        Public Function GetLogNotificaRicevutaAcnByIdNotifica(idNotifica As Long) As LogNotificaRicevutaACN Implements ILogNotificheProvider.GetLogNotificaRicevutaAcnByIdNotifica

            Dim item As LogNotificaRicevutaACN = Nothing

            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                cmd.CommandText = GetQuerySelectLogNotificheRicevuteAcn(False) + " WHERE LNR_ID = :LNR_ID "

                cmd.Parameters.AddWithValue("LNR_ID", idNotifica)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim list As List(Of LogNotificaRicevutaACN) = GetListNotificheRicevuteAcn(cmd)

                    If Not list.IsNullOrEmpty() Then
                        item = list.First()
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return item

        End Function

        Public Function CountLogNotificheRicevuteAcn(filtri As FiltriLogNotifiche) As Integer Implements ILogNotificheProvider.CountLogNotificheRicevuteAcn

            Dim count As Integer = 0

            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                cmd.CommandText =
                    GetQuerySelectLogNotificheRicevuteAcn(True) +
                    SetFiltriQueryNotificheRicevuteAcn(filtri, cmd)

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

        Public Function GetLogNotificheRicevuteAcn(filtri As FiltriLogNotifiche, campoOrdinamento As String, versoOrdinamento As String, pagingOptions As Data.PagingOptions) As List(Of Entities.LogNotificaRicevutaACN) Implements ILogNotificheProvider.GetLogNotificheRicevuteAcn

            Dim list As New List(Of LogNotificaRicevutaACN)()

            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                cmd.CommandText =
                    GetQuerySelectLogNotificheRicevuteAcn(False) +
                    SetFiltriQueryNotificheRicevuteAcn(filtri, cmd) +
                    GetOrderByNotificheRicevute(campoOrdinamento, versoOrdinamento)

                ' Paginazione
                If Not pagingOptions Is Nothing Then
                    cmd.AddPaginatedQuery(pagingOptions)
                End If

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    list = GetListNotificheRicevuteAcn(cmd)

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list

        End Function

        Private Function GetQuerySelectLogNotificheRicevuteAcn(isCount As Boolean) As String

            Dim query As New Text.StringBuilder()

            If isCount Then
                query.Append("SELECT COUNT(LNR_ID) FROM T_LOG_NOTIFICHE_RICEZIONE ")
            Else
                query.Append(" SELECT LNR_ID, LNR_ID_MESSAGGIO, LNR_PAZ_CODICE_CENTRALE, LNR_PAZ_CODICE_CENTRALE_ALIAS, LNR_DATA_RICEZIONE, LNR_PAZIENTE, ")
                query.Append(" LNR_DATA_INVIO_RISPOSTA, LNR_NUMERO_RICEZIONI, LNR_OPERAZIONE, LNR_RISULTATO_ELABORAZIONE, LNR_MESSAGGIO_ELABORAZIONE, LNR_ESENZIONI, ")
                query.Append(" LNR_SERVICE_REQUEST, LNR_SERVICE_RESPONSE,LNR_ENTITY, PAZ_CODICE, PAZ_COGNOME, PAZ_NOME, PAZ_DATA_NASCITA, PAZ_CODICE_FISCALE, PAZ_CODICE_REGIONALE ")
                query.Append(" FROM T_LOG_NOTIFICHE_RICEZIONE ")
            End If

            query.Append(" LEFT JOIN T_PAZ_PAZIENTI ON LNR_PAZ_CODICE_CENTRALE = PAZ_CODICE_AUSILIARIO ")

            Return query.ToString()

        End Function

        Private Function SetFiltriQueryNotificheRicevuteAcn(filtriRicerca As FiltriLogNotifiche, cmd As OracleClient.OracleCommand) As String

            If filtriRicerca Is Nothing Then Return String.Empty

            Dim filtri As New Text.StringBuilder()

            If Not filtriRicerca.IdMessaggio.IsNullOrEmpty Then
                filtri.AppendFormat(" lnr_id_messaggio like '%{0}%' and ", filtriRicerca.IdMessaggio)
            End If

            If filtriRicerca.DataRicezioneDa.HasValue Then
                filtri.Append(" lnr_data_ricezione >= :ricezioneDa and ")
                cmd.Parameters.AddWithValue("ricezioneDa", filtriRicerca.DataRicezioneDa.Value)
            End If

            If filtriRicerca.DataRicezioneA.HasValue Then
                filtri.Append(" lnr_data_ricezione <= :ricezioneA and ")
                cmd.Parameters.AddWithValue("ricezioneA", filtriRicerca.DataRicezioneA.Value)
            End If

            If Not filtriRicerca.Operazioni.IsNullOrEmpty() Then

                Dim filtroOperazioni As New Text.StringBuilder()

                For Each value As Enumerators.OperazioneLogNotifica In filtriRicerca.Operazioni
                    filtroOperazioni.AppendFormat("'{0}',", [Enum].GetName(GetType(Enumerators.OperazioneLogNotifica), value).ToUpper())
                Next

                If filtroOperazioni.Length > 0 Then
                    filtri.AppendFormat(" lnr_operazione in ({0}) and ", filtroOperazioni.RemoveLast(1).ToString())
                End If

            End If

            If filtriRicerca.DataNascitaDa.HasValue Then
                filtri.Append(" paz_data_nascita >= :nascitaDa and ")
                cmd.Parameters.AddWithValue("nascitaDa", filtriRicerca.DataNascitaDa.Value)
            End If

            If filtriRicerca.DataNascitaA.HasValue Then
                filtri.Append(" paz_data_nascita <= :nascitaA and ")
                cmd.Parameters.AddWithValue("nascitaA", filtriRicerca.DataNascitaA.Value)
            End If

            If Not filtriRicerca.Risultato.IsNullOrEmpty() Then
                filtri.AppendFormat(" lnr_risultato_elaborazione = '{0}' and ", filtriRicerca.Risultato)
            End If

            If Not filtriRicerca.CodiceMPI.IsNullOrEmpty() Then
                filtri.AppendFormat(" lnr_paz_codice_centrale like '%{0}%' and ", filtriRicerca.CodiceMPI)
            End If

            If Not filtriRicerca.CodiceLocale.IsNullOrEmpty() Then
                filtri.AppendFormat(" paz_codice like '%{0}%' and ", filtriRicerca.CodiceLocale)
            End If

            If Not filtriRicerca.CF.IsNullOrEmpty() Then
                filtri.AppendFormat(" paz_codice_fiscale = '{0}' and ", filtriRicerca.CF)
            End If

            If filtri.Length > 0 Then
                filtri.RemoveLast(4)
                filtri.Insert(0, " where ")
            End If

            Return filtri.ToString()

        End Function

        Private Function GetOrderByNotificheRicevute(campoOrdinamento As String, versoOrdinamento As String) As String

            Dim orderBy As String = "LNR_DATA_RICEZIONE DESC, LNR_ID_MESSAGGIO DESC"

            If Not String.IsNullOrWhiteSpace(campoOrdinamento) Then

                Select Case campoOrdinamento

                    Case "IdMessaggio"
                        orderBy = String.Format("LNR_ID_MESSAGGIO {0}", versoOrdinamento)

                    Case "Operazione"
                        orderBy = String.Format("LNR_OPERAZIONE {0}, LNR_ID_MESSAGGIO {0}", versoOrdinamento)

                    Case "DataRicezione"
                        orderBy = String.Format("LNR_DATA_RICEZIONE {0}, LNR_ID_MESSAGGIO {0}", versoOrdinamento)

                    Case "CodiceCentralePaziente"
                        orderBy = String.Format("LNR_PAZ_CODICE_CENTRALE {0}, LNR_ID_MESSAGGIO {0}", versoOrdinamento)

                    Case "CodiceCentralePazienteAlias"
                        orderBy = String.Format("LNR_PAZ_CODICE_CENTRALE_ALIAS {0}, LNR_ID_MESSAGGIO {0}", versoOrdinamento)

                    Case "CodiceLocalePaziente"
                        orderBy = String.Format("PAZ_CODICE {0}", versoOrdinamento)

                    Case "Cognome"
                        orderBy = String.Format("PAZ_COGNOME {0}, PAZ_NOME {0}, PAZ_DATA_NASCITA {0}, PAZ_CODICE {0}", versoOrdinamento)

                    Case "Nome"
                        orderBy = String.Format("PAZ_NOME {0}, PAZ_COGNOME {0}, PAZ_DATA_NASCITA {0}, PAZ_CODICE {0}", versoOrdinamento)

                    Case "DataNascita"
                        orderBy = String.Format("PAZ_DATA_NASCITA {0}, PAZ_COGNOME {0}, PAZ_NOME {0}, PAZ_CODICE {0}", versoOrdinamento)

                    Case "CodiceFiscale"
                        orderBy = String.Format("PAZ_CODICE_FISCALE {0}, PAZ_COGNOME {0}, PAZ_NOME {0}, PAZ_DATA_NASCITA {0}, PAZ_CODICE {0}", versoOrdinamento)

                    Case "RisultatoElaborazione"
                        orderBy = String.Format("LNR_RISULTATO_ELABORAZIONE {0}, LNR_ID_MESSAGGIO {0}", versoOrdinamento)

                    Case "IdNotifica"
                        orderBy = String.Format("LNR_ID {0}", versoOrdinamento)

                End Select

            End If

            Return " ORDER BY " + orderBy

        End Function

        Private Function GetListNotificheRicevuteAcn(cmd As OracleClient.OracleCommand) As List(Of LogNotificaRicevutaACN)

            Dim list As New List(Of LogNotificaRicevutaACN)()

            Using idr As IDataReader = cmd.ExecuteReader()
                If idr IsNot Nothing Then

                    Dim lnr_id As Integer = idr.GetOrdinal("LNR_ID")
                    Dim lnr_id_messaggio As Integer = idr.GetOrdinal("LNR_ID_MESSAGGIO")
                    Dim lnr_paz_codice_centrale As Integer = idr.GetOrdinal("LNR_PAZ_CODICE_CENTRALE")
                    Dim lnr_paz_codice_centrale_alias As Integer = idr.GetOrdinal("LNR_PAZ_CODICE_CENTRALE_ALIAS")
                    Dim lnr_data_ricezione As Integer = idr.GetOrdinal("LNR_DATA_RICEZIONE")
                    Dim lnr_paziente As Integer = idr.GetOrdinal("LNR_PAZIENTE")
                    Dim lnr_data_invio_risposta As Integer = idr.GetOrdinal("LNR_DATA_INVIO_RISPOSTA")
                    Dim lnr_numero_ricezioni As Integer = idr.GetOrdinal("LNR_NUMERO_RICEZIONI")
                    Dim lnr_operazione As Integer = idr.GetOrdinal("LNR_OPERAZIONE")
                    Dim lnr_risultato_elaborazione As Integer = idr.GetOrdinal("LNR_RISULTATO_ELABORAZIONE")
                    Dim lnr_messaggio_elaborazione As Integer = idr.GetOrdinal("LNR_MESSAGGIO_ELABORAZIONE")
                    Dim lnr_esenzioni As Integer = idr.GetOrdinal("LNR_ESENZIONI")
                    Dim lnr_service_request As Integer = idr.GetOrdinal("LNR_SERVICE_REQUEST")
                    Dim lnr_service_response As Integer = idr.GetOrdinal("LNR_SERVICE_RESPONSE")
                    Dim lnr_entity As Integer = idr.GetOrdinal("LNR_ENTITY")

                    Dim paz_codice As Integer = idr.GetOrdinal("PAZ_CODICE")
                    Dim paz_cognome As Integer = idr.GetOrdinal("PAZ_COGNOME")
                    Dim paz_nome As Integer = idr.GetOrdinal("PAZ_NOME")
                    Dim paz_data_nascita As Integer = idr.GetOrdinal("PAZ_DATA_NASCITA")
                    Dim paz_codice_fiscale As Integer = idr.GetOrdinal("PAZ_CODICE_FISCALE")
                    Dim paz_codice_regionale As Integer = idr.GetOrdinal("PAZ_CODICE_REGIONALE")

                    While idr.Read()

                        Dim item As New LogNotificaRicevutaACN()

                        item.IdNotifica = idr.GetInt64(lnr_id)
                        item.IdMessaggio = idr.GetString(lnr_id_messaggio)
                        item.CodiceCentralePaziente = idr.GetString(lnr_paz_codice_centrale)
                        item.CodiceCentralePazienteAlias = idr.GetStringOrDefault(lnr_paz_codice_centrale_alias)
                        item.DataRicezione = idr.GetDateTime(lnr_data_ricezione)
                        item.EntityPazienteSerializzata = idr.GetStringOrDefault(lnr_paziente)
                        item.DataInvioRisposta = idr.GetNullableDateTimeOrDefault(lnr_data_invio_risposta)
                        item.NumeroRicezioni = idr.GetNullableInt32OrDefault(lnr_numero_ricezioni)

                        If idr.IsDBNull(lnr_operazione) Then
                            item.Operazione = Nothing
                        Else
                            item.Operazione = [Enum].Parse(GetType(Enumerators.OperazioneLogNotifica), idr.GetString(lnr_operazione), True)
                        End If

                        item.RisultatoElaborazione = idr.GetStringOrDefault(lnr_risultato_elaborazione)
                        item.MessaggioElaborazione = idr.GetStringOrDefault(lnr_messaggio_elaborazione)
                        item.Esenzioni = idr.GetStringOrDefault(lnr_esenzioni)
                        item.ServiceRequest = idr.GetStringOrDefault(lnr_service_request)
                        item.ServiceResponse = idr.GetStringOrDefault(lnr_service_response)
                        item.EntitySerializzata = idr.GetStringOrDefault(lnr_entity)

                        item.CodiceLocalePaziente = idr.GetInt64OrDefault(paz_codice)
                        item.Cognome = idr.GetStringOrDefault(paz_cognome)
                        item.Nome = idr.GetStringOrDefault(paz_nome)
                        item.DataNascita = idr.GetDateTimeOrDefault(paz_data_nascita)
                        item.CodiceFiscale = idr.GetStringOrDefault(paz_codice_fiscale)
                        item.DataNascita = idr.GetDateTimeOrDefault(paz_data_nascita)
                        item.CodiceRegionale = idr.GetStringOrDefault(paz_codice_regionale)

                        list.Add(item)

                    End While

                End If

            End Using

            Return list

        End Function

        ''' <summary>
        ''' Inserimento log notifica specificata
        ''' </summary>
        ''' <param name="logNotificaRicevuta"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertLogNotificaRicevuta(logNotificaRicevuta As LogNotificaRicevuta) As Integer Implements ILogNotificheProvider.InsertLogNotificaRicevuta

            Dim count As Integer = 0

            Dim query As String =
                " INSERT INTO T_LOG_NOTIFICHE_RICEZIONE " +
                " (LNR_ID_MESSAGGIO, LNR_PAZ_CODICE_CENTRALE, LNR_PAZ_CODICE_CENTRALE_ALIAS, LNR_DATA_RICEZIONE, LNR_PAZIENTE, LNR_DATA_INVIO_RISPOSTA, LNR_NUMERO_RICEZIONI, LNR_OPERAZIONE, LNR_RISULTATO_ELABORAZIONE, LNR_MESSAGGIO_ELABORAZIONE, LNR_ESENZIONI, LNR_SERVICE_REQUEST, LNR_SERVICE_RESPONSE, LNR_REPOSITORY_UNIQUE_ID, LNR_DOCUMENT_UNIQUE_ID, LNR_ENTITY ) " +
                " VALUES " +
                " (:LNR_ID_MESSAGGIO, :LNR_PAZ_CODICE_CENTRALE, :LNR_PAZ_CODICE_CENTRALE_ALIAS, :LNR_DATA_RICEZIONE, :LNR_PAZIENTE, :LNR_DATA_INVIO_RISPOSTA, :LNR_NUMERO_RICEZIONI, :LNR_OPERAZIONE, :LNR_RISULTATO_ELABORAZIONE, :LNR_MESSAGGIO_ELABORAZIONE, :LNR_ESENZIONI, :LNR_SERVICE_REQUEST, :LNR_SERVICE_RESPONSE, :LNR_REPOSITORY_UNIQUE_ID, :LNR_DOCUMENT_UNIQUE_ID, :LNR_ENTITY ) "

            Using cmd As New OracleClient.OracleCommand(query, Connection)

                cmd.Parameters.AddWithValue("LNR_ID_MESSAGGIO", logNotificaRicevuta.IdMessaggio)
                cmd.Parameters.AddWithValue("LNR_PAZ_CODICE_CENTRALE", logNotificaRicevuta.CodiceCentralePaziente)
                cmd.Parameters.AddWithValueOrDefault("LNR_PAZ_CODICE_CENTRALE_ALIAS", logNotificaRicevuta.CodiceCentralePazienteAlias)
                cmd.Parameters.AddWithValue("LNR_DATA_RICEZIONE", logNotificaRicevuta.DataRicezione)
                cmd.Parameters.AddWithValueOrDefault("LNR_PAZIENTE", logNotificaRicevuta.EntityPazienteSerializzata)

                If logNotificaRicevuta.DataInvioRisposta.HasValue Then
                    cmd.Parameters.AddWithValue("LNR_DATA_INVIO_RISPOSTA", logNotificaRicevuta.DataInvioRisposta.Value)
                Else
                    cmd.Parameters.AddWithValue("LNR_DATA_INVIO_RISPOSTA", DBNull.Value)
                End If

                If logNotificaRicevuta.NumeroRicezioni.HasValue Then
                    cmd.Parameters.AddWithValue("LNR_NUMERO_RICEZIONI", logNotificaRicevuta.NumeroRicezioni.Value)
                Else
                    cmd.Parameters.AddWithValue("LNR_NUMERO_RICEZIONI", DBNull.Value)
                End If

                If logNotificaRicevuta.Operazione.HasValue Then
                    cmd.Parameters.AddWithValue("LNR_OPERAZIONE", [Enum].GetName(GetType(Enumerators.OperazioneLogNotifica), logNotificaRicevuta.Operazione.Value).ToUpper())
                Else
                    cmd.Parameters.AddWithValue("LNR_OPERAZIONE", DBNull.Value)
                End If

                cmd.Parameters.AddWithValueOrDefault("LNR_RISULTATO_ELABORAZIONE", logNotificaRicevuta.RisultatoElaborazione)
                cmd.Parameters.AddWithValueOrDefault("LNR_MESSAGGIO_ELABORAZIONE", logNotificaRicevuta.MessaggioElaborazione)
                cmd.Parameters.AddWithValueOrDefault("LNR_ESENZIONI", logNotificaRicevuta.Esenzioni)
                cmd.Parameters.AddWithValueOrDefault("LNR_SERVICE_REQUEST", logNotificaRicevuta.ServiceRequest)
                cmd.Parameters.AddWithValueOrDefault("LNR_SERVICE_RESPONSE", logNotificaRicevuta.ServiceResponse)
                cmd.Parameters.AddWithValueOrDefault("LNR_REPOSITORY_UNIQUE_ID", logNotificaRicevuta.RepositoryUniqueId)
                cmd.Parameters.AddWithValueOrDefault("LNR_DOCUMENT_UNIQUE_ID", logNotificaRicevuta.DocumentUniqueId)
                cmd.Parameters.AddWithValueOrDefault("LNR_ENTITY", logNotificaRicevuta.EntitySerializzata)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    count = cmd.ExecuteNonQuery()

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Modifica log notifica ricevuta
        ''' </summary>
        ''' <param name="logNotificaRicevuta"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateLogNotificaRicevuta(logNotificaRicevuta As LogNotificaRicevuta) As Integer Implements ILogNotificheProvider.UpdateLogNotificaRicevuta

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Connection

                Dim query As New Text.StringBuilder(" UPDATE T_LOG_NOTIFICHE_RICEZIONE SET ")

                If logNotificaRicevuta.Operazione.HasValue Then
                    query.Append(" LNR_OPERAZIONE = :LNR_OPERAZIONE, ")
                    cmd.Parameters.AddWithValue("LNR_OPERAZIONE", [Enum].GetName(GetType(Enumerators.OperazioneLogNotifica), logNotificaRicevuta.Operazione.Value).ToUpper())
                End If

                If logNotificaRicevuta.NumeroRicezioni.HasValue Then
                    query.Append(" LNR_NUMERO_RICEZIONI = :LNR_NUMERO_RICEZIONI, ")
                    cmd.Parameters.AddWithValue("LNR_NUMERO_RICEZIONI", logNotificaRicevuta.NumeroRicezioni.Value)
                End If

                If logNotificaRicevuta.DataInvioRisposta.HasValue Then
                    query.Append(" LNR_DATA_INVIO_RISPOSTA = :LNR_DATA_INVIO_RISPOSTA, ")
                    cmd.Parameters.AddWithValue("LNR_DATA_INVIO_RISPOSTA", logNotificaRicevuta.DataInvioRisposta.Value)
                End If

                If Not String.IsNullOrWhiteSpace(logNotificaRicevuta.RisultatoElaborazione) Then
                    query.Append(" LNR_RISULTATO_ELABORAZIONE = :LNR_RISULTATO_ELABORAZIONE, ")
                    cmd.Parameters.AddWithValue("LNR_RISULTATO_ELABORAZIONE", logNotificaRicevuta.RisultatoElaborazione)
                End If

                If Not String.IsNullOrEmpty(logNotificaRicevuta.MessaggioElaborazione) Then
                    query.Append(" LNR_MESSAGGIO_ELABORAZIONE = :LNR_MESSAGGIO_ELABORAZIONE, ")
                    cmd.Parameters.AddWithValue("LNR_MESSAGGIO_ELABORAZIONE", logNotificaRicevuta.MessaggioElaborazione)
                End If

                If Not String.IsNullOrEmpty(logNotificaRicevuta.ServiceRequest) Then
                    query.Append(" LNR_SERVICE_REQUEST = :LNR_SERVICE_REQUEST, ")
                    cmd.Parameters.AddWithValue("LNR_SERVICE_REQUEST", logNotificaRicevuta.ServiceRequest)
                End If

                If Not String.IsNullOrEmpty(logNotificaRicevuta.ServiceResponse) Then
                    query.Append(" LNR_SERVICE_RESPONSE = :LNR_SERVICE_RESPONSE, ")
                    cmd.Parameters.AddWithValue("LNR_SERVICE_RESPONSE", logNotificaRicevuta.ServiceResponse)
                End If

                If Not String.IsNullOrEmpty(logNotificaRicevuta.RepositoryUniqueId) Then
                    query.Append(" LNR_REPOSITORY_UNIQUE_ID = :LNR_REPOSITORY_UNIQUE_ID, ")
                    cmd.Parameters.AddWithValue("LNR_REPOSITORY_UNIQUE_ID", logNotificaRicevuta.RepositoryUniqueId)
                End If

                If Not String.IsNullOrEmpty(logNotificaRicevuta.DocumentUniqueId) Then
                    query.Append(" LNR_DOCUMENT_UNIQUE_ID = :LNR_DOCUMENT_UNIQUE_ID, ")
                    cmd.Parameters.AddWithValue("LNR_DOCUMENT_UNIQUE_ID", logNotificaRicevuta.DocumentUniqueId)
                End If

                query.RemoveLast(2)

                query.Append(" WHERE LNR_ID_MESSAGGIO = :LNR_ID_MESSAGGIO ")
                cmd.Parameters.AddWithValue("LNR_ID_MESSAGGIO", logNotificaRicevuta.IdMessaggio)

                cmd.CommandText = query.ToString()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    count = cmd.ExecuteNonQuery()

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

#End Region

#Region " Notifiche Inviate "

        Public Function GetNextIdNotificaInviata() As Long Implements ILogNotificheProvider.GetNextIdNotificaInviata

            Dim idNotifica As Long = 0

            Using cmd As New OracleClient.OracleCommand("SELECT SEQ_LOG_NOTIFICHE_INVIATE.NEXTVAL FROM DUAL", Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    idNotifica = Convert.ToInt64(obj)

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return idNotifica

        End Function

        ''' <summary>
        ''' Inserimento log notifica inviata
        ''' </summary>
        ''' <param name="logNotificaInviata"></param>
        ''' <param name="codiceUslLocale"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertLogNotificaInviata(logNotificaInviata As LogNotificaInviata, codiceUslLocale As String) As Integer Implements ILogNotificheProvider.InsertLogNotificaInviata

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Connection

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    ' Id progressivo
                    Dim idInsert As Long
                    If logNotificaInviata.IdNotifica <> Nothing Then
                        idInsert = logNotificaInviata.IdNotifica
                    Else
                        idInsert = GetNextIdNotificaInviata()
                    End If
                    logNotificaInviata.IdNotifica = idInsert

                    ' Inserimento log
                    cmd.CommandText =
                        " INSERT INTO T_LOG_NOTIFICHE_INVIO " +
                        " (LNI_ID, LNI_PAZ_CODICE_CENTRALE, LNI_PAZ_CODICE_LOCALE, LNI_DATA_INVIO, LNI_OPERAZIONE, LNI_PAZIENTE, LNI_PAZIENTE_PRECEDENTE, " +
                        "  LNI_DATA_RICEZIONE_RISPOSTA, LNI_RISULTATO_RISPOSTA, LNI_MESSAGGIO_RISPOSTA, LNI_SERVICE_REQUEST, LNI_SERVICE_RESPONSE, " +
                        "  LNI_UTE_ID_INSERIMENTO, LNI_DATA_INSERIMENTO, LNI_STATO, LNI_USL_CODICE_LOCALE, LNI_ID_MESSAGGIO, LNI_POL_ID, LNI_CDA_FSE, LNI_NEW_DOC_UNIQ_ID_FSE, " +
                        "  LNI_REPOS_UNIQ_ID_FSE, LNI_SOURCE_IP_FSE, LNI_DESTI_USER_ID_FSE, LNI_NUMERO_INVII, LNI_OPERATORE) " +
                        " VALUES " +
                        " (:LNI_ID, :LNI_PAZ_CODICE_CENTRALE, :LNI_PAZ_CODICE_LOCALE, :LNI_DATA_INVIO, :LNI_OPERAZIONE, :LNI_PAZIENTE, :LNI_PAZIENTE_PRECEDENTE, " +
                        "  :LNI_DATA_RICEZIONE_RISPOSTA, :LNI_RISULTATO_RISPOSTA, :LNI_MESSAGGIO_RISPOSTA, :LNI_SERVICE_REQUEST, :LNI_SERVICE_RESPONSE, " +
                        "  :LNI_UTE_ID_INSERIMENTO, :LNI_DATA_INSERIMENTO, :LNI_STATO, :LNI_USL_CODICE_LOCALE, :LNI_ID_MESSAGGIO, :LNI_POL_ID, :LNI_CDA_FSE, :LNI_NEW_DOC_UNIQ_ID_FSE, " +
                        "  :LNI_REPOS_UNIQ_ID_FSE, :LNI_SOURCE_IP_FSE, :LNI_DESTI_USER_ID_FSE, :LNI_NUMERO_INVII, :LNI_OPERATORE) "

                    cmd.Parameters.AddWithValue("LNI_ID", logNotificaInviata.IdNotifica)
                    cmd.Parameters.AddWithValueOrDefault("LNI_PAZ_CODICE_CENTRALE", logNotificaInviata.CodiceCentralePaziente)
                    cmd.Parameters.AddWithValueOrDefault("LNI_PAZ_CODICE_LOCALE", logNotificaInviata.CodiceLocalePaziente)

                    If logNotificaInviata.DataInvio.HasValue Then
                        cmd.Parameters.AddWithValue("LNI_DATA_INVIO", logNotificaInviata.DataInvio.Value)
                    Else
                        cmd.Parameters.AddWithValue("LNI_DATA_INVIO", DBNull.Value)
                    End If

                    cmd.Parameters.AddWithValueOrDefault("LNI_PAZIENTE", logNotificaInviata.EntityPazienteSerializzata)
                    cmd.Parameters.AddWithValueOrDefault("LNI_PAZIENTE_PRECEDENTE", logNotificaInviata.EntityPazientePrecedenteSerializzata)

                    If logNotificaInviata.Operazione.HasValue Then
                        cmd.Parameters.AddWithValue("LNI_OPERAZIONE", [Enum].GetName(GetType(Enumerators.OperazioneLogNotifica), logNotificaInviata.Operazione.Value).ToUpper())
                    Else
                        cmd.Parameters.AddWithValue("LNI_OPERAZIONE", DBNull.Value)
                    End If

                    cmd.Parameters.AddWithValueOrDefault("LNI_DATA_RICEZIONE_RISPOSTA", logNotificaInviata.DataRicezioneRisposta)
                    cmd.Parameters.AddWithValueOrDefault("LNI_RISULTATO_RISPOSTA", logNotificaInviata.RisultatoRisposta)
                    cmd.Parameters.AddWithValueOrDefault("LNI_MESSAGGIO_RISPOSTA", logNotificaInviata.MessaggioRisposta)
                    cmd.Parameters.AddWithValueOrDefault("LNI_SERVICE_REQUEST", logNotificaInviata.ServiceRequest)
                    cmd.Parameters.AddWithValueOrDefault("LNI_SERVICE_RESPONSE", logNotificaInviata.ServiceResponse)

                    cmd.Parameters.AddWithValue("LNI_UTE_ID_INSERIMENTO", logNotificaInviata.IdUtenteInserimentoNotifica)
                    cmd.Parameters.AddWithValue("LNI_DATA_INSERIMENTO", logNotificaInviata.DataInserimentoNotifica)
                    cmd.Parameters.AddWithValue("LNI_STATO", Convert.ToInt32(logNotificaInviata.Stato))
                    cmd.Parameters.AddWithValue("LNI_USL_CODICE_LOCALE", GetStringParam(codiceUslLocale))
                    cmd.Parameters.AddWithValue("LNI_ID_MESSAGGIO", GetStringParam(logNotificaInviata.IdMessaggio))

                    If logNotificaInviata.IdPolo.HasValue Then
                        cmd.Parameters.AddWithValue("LNI_POL_ID", logNotificaInviata.IdPolo.Value)
                    Else
                        cmd.Parameters.AddWithValue("LNI_POL_ID", DBNull.Value)
                    End If
                    cmd.Parameters.AddWithValueOrDefault("LNI_CDA_FSE", logNotificaInviata.CdaFse)
                    cmd.Parameters.AddWithValueOrDefault("LNI_NEW_DOC_UNIQ_ID_FSE", logNotificaInviata.NewDocUniqId)
                    cmd.Parameters.AddWithValueOrDefault("LNI_REPOS_UNIQ_ID_FSE", logNotificaInviata.ReposUniqIdFse)
                    cmd.Parameters.AddWithValueOrDefault("LNI_SOURCE_IP_FSE", logNotificaInviata.SourceIpFse)
                    cmd.Parameters.AddWithValueOrDefault("LNI_DESTI_USER_ID_FSE", logNotificaInviata.DestiUserIdFse)

                    If logNotificaInviata.NumeroInvii.HasValue Then
                        cmd.Parameters.AddWithValue("LNI_NUMERO_INVII", logNotificaInviata.NumeroInvii.Value)
                    Else
                        cmd.Parameters.AddWithValue("LNI_NUMERO_INVII", 0)
                    End If

                    cmd.Parameters.AddWithValueOrDefault("LNI_OPERATORE", logNotificaInviata.Operatore)

                    count = cmd.ExecuteNonQuery()

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Modifica log notifica inviata
        ''' </summary>
        ''' <param name="logNotificaInviata"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateLogNotificaInviata(logNotificaInviata As LogNotificaInviata) As Integer Implements ILogNotificheProvider.UpdateLogNotificaInviata

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Connection

                Dim query As New Text.StringBuilder(" UPDATE T_LOG_NOTIFICHE_INVIO SET ")

                If Not String.IsNullOrWhiteSpace(logNotificaInviata.CodiceCentralePaziente) Then
                    query.Append(" LNI_PAZ_CODICE_CENTRALE = :LNI_PAZ_CODICE_CENTRALE, ")
                    cmd.Parameters.AddWithValue("LNI_PAZ_CODICE_CENTRALE", logNotificaInviata.CodiceCentralePaziente)
                End If

                If logNotificaInviata.DataRicezioneRisposta.HasValue Then
                    query.Append(" LNI_DATA_RICEZIONE_RISPOSTA = :LNI_DATA_RICEZIONE_RISPOSTA, ")
                    cmd.Parameters.AddWithValue("LNI_DATA_RICEZIONE_RISPOSTA", logNotificaInviata.DataRicezioneRisposta.Value)
                End If

                If Not String.IsNullOrEmpty(logNotificaInviata.RisultatoRisposta) Then
                    query.Append(" LNI_RISULTATO_RISPOSTA = :LNI_RISULTATO_RISPOSTA, ")
                    cmd.Parameters.AddWithValue("LNI_RISULTATO_RISPOSTA", logNotificaInviata.RisultatoRisposta)
                End If

                If Not String.IsNullOrEmpty(logNotificaInviata.MessaggioRisposta) Then
                    query.Append(" LNI_MESSAGGIO_RISPOSTA = :LNI_MESSAGGIO_RISPOSTA, ")
                    cmd.Parameters.AddWithValue("LNI_MESSAGGIO_RISPOSTA", logNotificaInviata.MessaggioRisposta)
                End If

                If Not String.IsNullOrEmpty(logNotificaInviata.ServiceRequest) Then
                    query.Append(" LNI_SERVICE_REQUEST = :LNI_SERVICE_REQUEST, ")
                    cmd.Parameters.AddWithValue("LNI_SERVICE_REQUEST", logNotificaInviata.ServiceRequest)
                End If

                If Not String.IsNullOrEmpty(logNotificaInviata.ServiceResponse) Then
                    query.Append(" LNI_SERVICE_RESPONSE = :LNI_SERVICE_RESPONSE, ")
                    cmd.Parameters.AddWithValue("LNI_SERVICE_RESPONSE", logNotificaInviata.ServiceResponse)
                End If

                If logNotificaInviata.DataInvio.HasValue Then
                    query.Append(" LNI_DATA_INVIO = :LNI_DATA_INVIO, ")
                    cmd.Parameters.AddWithValue("LNI_DATA_INVIO", logNotificaInviata.DataInvio.Value)
                End If

                query.Append(" LNI_STATO = :LNI_STATO, ")
                cmd.Parameters.AddWithValue("LNI_STATO", Convert.ToInt32(logNotificaInviata.Stato))

                If logNotificaInviata.NumeroInvii.HasValue Then
                    query.Append(" LNI_NUMERO_INVII = :LNI_NUMERO_INVII, ")
                    cmd.Parameters.AddWithValue("LNI_NUMERO_INVII", logNotificaInviata.NumeroInvii.Value)
                End If

                query.RemoveLast(2)

                query.Append(" WHERE LNI_ID = :LNI_ID ")
                cmd.Parameters.AddWithValue("LNI_ID", logNotificaInviata.IdNotifica)

                cmd.CommandText = query.ToString()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    count = cmd.ExecuteNonQuery()

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Restituisce le notifiche da inviare, partendo dalle più vecchie.
        ''' </summary>
        ''' <param name="numeroNotificheDaRecuperare">Limite massimo di notifiche che verranno lette da db. Se vale 0, le legge tutte.</param>
        ''' <param name="statoNotifiche"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetLogNotificheInvioByStato(numeroNotificheDaRecuperare As Integer, statoNotifiche As Enumerators.StatoLogNotificaInviata) As IEnumerable(Of Entities.LogNotificaInviata) Implements ILogNotificheProvider.GetLogNotificheInvioByStato

            Dim list As List(Of LogNotificaInviata)

            Dim query As String =
                GetQuerySelectLogNotificheInviate() +
                " WHERE LNI_STATO = :LNI_STATO " +
                " ORDER BY LNI_DATA_INSERIMENTO ASC "

            Using cmd As New OracleClient.OracleCommand(query, Connection)

                cmd.Parameters.AddWithValue("LNI_STATO", Convert.ToInt32(statoNotifiche))

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    list = GetListNotificheInviate(cmd, numeroNotificheDaRecuperare)

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list.AsEnumerable()

        End Function

        ''' <summary>
        ''' Restituisce la lista di notifiche di un dato paziente per una data operazione. 
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="operazione"></param>
        ''' <returns></returns>
        Public Function GetLogNotificheInvioByOperazione(codicePaziente As Long, operazione As Enumerators.OperazioneLogNotifica) As List(Of Entities.LogNotificaInviata) Implements ILogNotificheProvider.GetLogNotificheInvioByOperazione

            Dim list As List(Of LogNotificaInviata)

            Dim query As String =
                GetQuerySelectLogNotificheInviate() +
                " WHERE LNI_PAZ_CODICE_LOCALE = :LNI_PAZ_CODICE_LOCALE " +
                " AND LNI_OPERAZIONE = :LNI_OPERAZIONE " +
                " ORDER BY LNI_DATA_INSERIMENTO ASC "

            Using cmd As New OracleClient.OracleCommand(query, Connection)

                cmd.Parameters.AddWithValue("LNI_PAZ_CODICE_LOCALE", codicePaziente)
                cmd.Parameters.AddWithValue("LNI_OPERAZIONE", [Enum].GetName(GetType(Enumerators.OperazioneLogNotifica), operazione).ToUpper())

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    list = GetListNotificheInviate(cmd, 0)

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list

        End Function

        Public Function GetLogNotificaInvio(idNotifica As Long) As LogNotificaInviata Implements ILogNotificheProvider.GetLogNotificaInvio

            Dim logNotifica As LogNotificaInviata = Nothing

            Dim ownConnection As Boolean = False

            Dim query As String = GetQuerySelectLogNotificheInviate() + " WHERE LNI_ID = :LNI_ID "

            Using cmd As New OracleClient.OracleCommand(query, Connection)

                cmd.Parameters.AddWithValue("LNI_ID", idNotifica)

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim list As List(Of LogNotificaInviata) = GetListNotificheInviate(cmd, 0)

                    If Not list.IsNullOrEmpty() Then

                        logNotifica = list.First()

                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return logNotifica

        End Function

        ''' <summary>
        ''' Recupera le notifiche da inviare ad FSE
        ''' </summary>
        ''' <param name="maxRecord"></param>
        ''' <returns></returns>
        Public Function GetLogNotificheDaInviareFSE(maxRecord As Integer) As List(Of LogNotificaInviata) Implements ILogNotificheProvider.GetLogNotificheDaInviareFSE

            Dim list As List(Of LogNotificaInviata)

            Dim query As String =
                GetQuerySelectLogNotificheInviate() +
                " WHERE LNI_STATO = :LNI_STATO " +
                " AND (LNI_OPERAZIONE = :LNI_OPERAZIONE_1 OR LNI_OPERAZIONE = :LNI_OPERAZIONE_2) " +
                " ORDER BY LNI_DATA_INSERIMENTO ASC "

            Using cmd As New OracleClient.OracleCommand(query, Connection)

                cmd.Parameters.AddWithValue("LNI_STATO", Convert.ToInt32(Enumerators.StatoLogNotificaInviata.DaInviare))
                cmd.Parameters.AddWithValue("LNI_OPERAZIONE_1", [Enum].GetName(GetType(Enumerators.OperazioneLogNotifica), Enumerators.OperazioneLogNotifica.IndicizzazioneCertVaccFSE).ToUpper())
                cmd.Parameters.AddWithValue("LNI_OPERAZIONE_2", [Enum].GetName(GetType(Enumerators.OperazioneLogNotifica), Enumerators.OperazioneLogNotifica.StoricizzazioneCertVaccFSE).ToUpper())


                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    list = GetListNotificheInviate(cmd, maxRecord)

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list

        End Function

        Private Function GetQuerySelectLogNotificheInviate() As String

            Return " SELECT LNI_ID, LNI_PAZ_CODICE_CENTRALE, LNI_PAZ_CODICE_LOCALE, LNI_DATA_INVIO, LNI_OPERAZIONE, " +
                " LNI_PAZIENTE, LNI_PAZIENTE_PRECEDENTE, LNI_DATA_RICEZIONE_RISPOSTA, LNI_RISULTATO_RISPOSTA, LNI_MESSAGGIO_RISPOSTA, " +
                " LNI_SERVICE_REQUEST, LNI_SERVICE_RESPONSE, LNI_DATA_INSERIMENTO, LNI_UTE_ID_INSERIMENTO, LNI_STATO, LNI_ID_MESSAGGIO, " +
                " LNI_POL_ID, LNI_NUMERO_INVII, LNI_USL_CODICE_LOCALE, " +
                " LNI_CDA_FSE, LNI_NEW_DOC_UNIQ_ID_FSE, LNI_SOURCE_IP_FSE, LNI_DESTI_USER_ID_FSE, LNI_REPOS_UNIQ_ID_FSE, LNI_OPERATORE " +
                " FROM T_LOG_NOTIFICHE_INVIO "

        End Function

        Private Function GetListNotificheInviate(cmd As OracleClient.OracleCommand, numeroNotificheDaRecuperare As Integer) As List(Of LogNotificaInviata)

            Dim list As New List(Of LogNotificaInviata)()

            Using idr As IDataReader = cmd.ExecuteReader()
                If idr IsNot Nothing Then

                    Dim lni_id As Integer = idr.GetOrdinal("LNI_ID")
                    Dim lni_paz_codice_centrale As Integer = idr.GetOrdinal("LNI_PAZ_CODICE_CENTRALE")
                    Dim lni_paz_codice_locale As Integer = idr.GetOrdinal("LNI_PAZ_CODICE_LOCALE")
                    Dim lni_data_invio As Integer = idr.GetOrdinal("LNI_DATA_INVIO")
                    Dim lni_operazione As Integer = idr.GetOrdinal("LNI_OPERAZIONE")
                    Dim lni_paziente As Integer = idr.GetOrdinal("LNI_PAZIENTE")
                    Dim lni_paziente_precedente As Integer = idr.GetOrdinal("LNI_PAZIENTE_PRECEDENTE")
                    Dim lni_data_ricezione_risposta As Integer = idr.GetOrdinal("LNI_DATA_RICEZIONE_RISPOSTA")
                    Dim lni_risultato_risposta As Integer = idr.GetOrdinal("LNI_RISULTATO_RISPOSTA")
                    Dim lni_messaggio_risposta As Integer = idr.GetOrdinal("LNI_MESSAGGIO_RISPOSTA")
                    Dim lni_service_request As Integer = idr.GetOrdinal("LNI_SERVICE_REQUEST")
                    Dim lni_service_response As Integer = idr.GetOrdinal("LNI_SERVICE_RESPONSE")
                    Dim lni_data_inserimento As Integer = idr.GetOrdinal("LNI_DATA_INSERIMENTO")
                    Dim lni_ute_id_inserimento As Integer = idr.GetOrdinal("LNI_UTE_ID_INSERIMENTO")
                    Dim lni_stato As Integer = idr.GetOrdinal("LNI_STATO")
                    Dim lni_id_messaggio As Integer = idr.GetOrdinal("LNI_ID_MESSAGGIO")
                    Dim lni_pol_id As Integer = idr.GetOrdinal("LNI_POL_ID")
                    Dim lni_numero_invii As Integer = idr.GetOrdinal("LNI_NUMERO_INVII")
                    Dim lni_codice_usl As Integer = idr.GetOrdinal("LNI_USL_CODICE_LOCALE")
                    Dim lni_cda_fse As Integer = idr.GetOrdinal("LNI_CDA_FSE")
                    Dim lni_new_doc_uniq_id_fse As Integer = idr.GetOrdinal("LNI_NEW_DOC_UNIQ_ID_FSE")
                    Dim lni_source_ip_fse As Integer = idr.GetOrdinal("LNI_SOURCE_IP_FSE")
                    Dim lni_desti_user_id_fse As Integer = idr.GetOrdinal("LNI_DESTI_USER_ID_FSE")
                    Dim lni_repos_uniq_id_fse As Integer = idr.GetOrdinal("LNI_REPOS_UNIQ_ID_FSE")
                    Dim lni_operatore As Integer = idr.GetOrdinal("LNI_OPERATORE")

                    Dim count As Integer = 0

                    While idr.Read() AndAlso (numeroNotificheDaRecuperare = 0 OrElse numeroNotificheDaRecuperare > count)

                        Dim logNotifica As New LogNotificaInviata()

                        logNotifica.IdNotifica = idr.GetInt64(lni_id)
                        logNotifica.CodiceCentralePaziente = idr.GetStringOrDefault(lni_paz_codice_centrale)
                        logNotifica.CodiceLocalePaziente = idr.GetInt64OrDefault(lni_paz_codice_locale)
                        logNotifica.DataInvio = idr.GetNullableDateTimeOrDefault(lni_data_invio)
                        logNotifica.EntityPazienteSerializzata = idr.GetStringOrDefault(lni_paziente)
                        logNotifica.EntityPazientePrecedenteSerializzata = idr.GetStringOrDefault(lni_paziente_precedente)
                        If idr.IsDBNull(lni_operazione) Then
                            logNotifica.Operazione = Nothing
                        Else
                            logNotifica.Operazione = [Enum].Parse(GetType(Enumerators.OperazioneLogNotifica), idr.GetString(lni_operazione), True)
                        End If
                        logNotifica.DataRicezioneRisposta = idr.GetNullableDateTimeOrDefault(lni_data_ricezione_risposta)
                        logNotifica.RisultatoRisposta = idr.GetStringOrDefault(lni_risultato_risposta)
                        logNotifica.MessaggioRisposta = idr.GetStringOrDefault(lni_messaggio_risposta)
                        logNotifica.ServiceRequest = idr.GetStringOrDefault(lni_service_request)
                        logNotifica.ServiceResponse = idr.GetStringOrDefault(lni_service_response)
                        logNotifica.IdUtenteInserimentoNotifica = idr.GetInt64(lni_ute_id_inserimento)
                        logNotifica.DataInserimentoNotifica = idr.GetNullableDateTimeOrDefault(lni_data_inserimento)
                        logNotifica.Stato = DirectCast(idr.GetInt32(lni_stato), Enumerators.StatoLogNotificaInviata)
                        logNotifica.IdMessaggio = idr.GetStringOrDefault(lni_id_messaggio)
                        logNotifica.IdPolo = idr.GetInt64OrDefault(lni_pol_id)
                        logNotifica.CodiceUsl = idr.GetStringOrDefault(lni_codice_usl)
                        logNotifica.NumeroInvii = idr.GetNullableInt32OrDefault(lni_numero_invii)
                        logNotifica.CdaFse = idr.GetStringOrDefault(lni_cda_fse)
                        logNotifica.NewDocUniqId = idr.GetStringOrDefault(lni_new_doc_uniq_id_fse)
                        logNotifica.SourceIpFse = idr.GetStringOrDefault(lni_source_ip_fse)
                        logNotifica.DestiUserIdFse = idr.GetStringOrDefault(lni_desti_user_id_fse)
                        logNotifica.ReposUniqIdFse = idr.GetStringOrDefault(lni_repos_uniq_id_fse)
                        logNotifica.Operatore = idr.GetStringOrDefault(lni_operatore)

                        list.Add(logNotifica)

                        count += 1

                    End While

                End If
            End Using

            Return list

        End Function

#End Region

#End Region

    End Class

End Namespace

