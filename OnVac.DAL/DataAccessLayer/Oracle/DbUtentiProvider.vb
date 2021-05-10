Imports System.Collections.Generic
Imports System.Data.OracleClient

Imports Onit.OnAssistnet.Data
Imports Onit.OnAssistnet.Data.OracleClient

Imports Onit.Database.DataAccessManager


Namespace DAL.Oracle

    Public Class DbUtentiProvider
        Inherits DbProvider
        Implements IUtentiProvider

#Region " Constructors "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " Public "

        Public Function GetDescrizioneUtente(idUtente As Integer) As String Implements IUtentiProvider.GetDescrizioneUtente

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("ute_descrizione")
                .AddTables("t_ana_utenti")
                .AddWhereCondition("ute_id", Comparatori.Uguale, idUtente, DataTypes.Numero)
            End With

            Dim obj As Object = _DAM.ExecScalar()

            If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                Return Convert.ToString(obj)
            End If

            Return String.Empty

        End Function

        Public Function GetCodiceUtente(idUtente As Integer) As String Implements IUtentiProvider.GetCodiceUtente

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("ute_codice")
                .AddTables("t_ana_utenti")
                .AddWhereCondition("ute_id", Comparatori.Uguale, idUtente, DataTypes.Numero)
            End With

            Dim obj As Object = _DAM.ExecScalar()

            If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                Return Convert.ToString(obj)
            End If

            Return String.Empty

        End Function

        Public Function GetCodiceFiscaleUtente(idUtente As Integer) As String Implements IUtentiProvider.GetCodiceFiscaleUtente

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("ute_codice_fiscale")
                .AddTables("t_ana_utenti")
                .AddWhereCondition("ute_id", Comparatori.Uguale, idUtente, DataTypes.Numero)
            End With

            Dim obj As Object = _DAM.ExecScalar()

            If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                Return Convert.ToString(obj)
            End If

            Return String.Empty

        End Function

        Public Function GetMedicoDaUtente(idUtente As Integer) As String Implements IUtentiProvider.GetMedicoDaUtente

            Dim result As String = String.Empty

            Using cmd As OracleCommand = Me.Connection.CreateCommand()

                cmd.Parameters.AddWithValue("id", idUtente)
                cmd.CommandText = Queries.Utenti.OracleQueries.sel_medico_da_codice_esterno

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        result = obj.ToString()
                    End If

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return result

        End Function

        ''' <summary>
        ''' Restituisce un oggetto Utente contenente tutti i dati dell'utente letti da db, in base all'id specificato.
        ''' </summary>
        ''' <param name="idUtente"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetUtente(idUtente As Integer) As OnVac.Entities.Utente Implements IUtentiProvider.GetUtente

            Dim utente As Entities.Utente = Nothing

            Using cmd As OracleCommand = Me.Connection.CreateCommand()

                cmd.Parameters.AddWithValue("uteId", idUtente)
                cmd.CommandText = OnVac.Queries.Utenti.OracleQueries.selUtente

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim listUtenti As List(Of Entities.Utente) = Me.GetListUtenti(cmd)

                    If Not listUtenti.IsNullOrEmpty() Then
                        utente = listUtenti.First()
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return utente

        End Function

        ''' <summary>
        ''' Restituisce il numero di utenti trovati, in base ai filtri
        ''' </summary>
        ''' <param name="codiceDescrizioneLikeFilter"></param>
        ''' <param name="codiceConsultorio"></param>
        ''' <param name="appId"></param>
        ''' <param name="codiceAzienda"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CountUtenti(codiceDescrizioneLikeFilter As String, codiceConsultorio As String, appId As String, codiceAzienda As String) As Integer Implements IUtentiProvider.CountUtenti

            Dim count As Integer

            Using cmd As OracleCommand = Me.Connection.CreateCommand()

                cmd.CommandText = Me.CreateQueryUtenti(cmd, codiceDescrizioneLikeFilter, codiceConsultorio, appId, codiceAzienda, True)

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
        ''' Restituisce una lista di Utenti letti dalla v_ana_utenti, filtrati per appId (se specificato).
        ''' Se il parametro codiceDescrizioneLikeFilter è valorizzato, la ricerca sarà effettuata filtrando (in like) codice e descrizione in base al valore del parametro.
        ''' </summary>
        ''' <param name="codiceDescrizioneLikeFilter"></param>
        ''' <param name="codiceConsultorio"></param>
        ''' <param name="appId"></param>
        ''' <param name="codiceAzienda"></param>
        ''' <param name="pagingOptions"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetListUtenti(codiceDescrizioneLikeFilter As String, codiceConsultorio As String, appId As String, codiceAzienda As String, pagingOptions As PagingOptions) As List(Of Entities.Utente) Implements IUtentiProvider.GetListUtenti

            Dim listUtenti As List(Of Entities.Utente) = Nothing

            Using cmd As OracleCommand = Me.Connection.CreateCommand()

                cmd.CommandText = Me.CreateQueryUtenti(cmd, codiceDescrizioneLikeFilter, codiceConsultorio, appId, codiceAzienda, False)

                If Not pagingOptions Is Nothing Then
                    cmd.AddPaginatedQuery(pagingOptions)
                End If

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    listUtenti = Me.GetListUtenti(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listUtenti

        End Function

        Public Function GetListConsultoriUtente(idUtente As Integer) As List(Of Entities.ConsultorioUtente) Implements IUtentiProvider.GetListConsultoriUtente

            Dim listConsultoriUtente As New List(Of Entities.ConsultorioUtente)()

            Using cmd As OracleCommand = Me.Connection.CreateCommand()

                cmd.Parameters.AddWithValue("LUC_UTE_ID", idUtente)
                cmd.CommandText = Queries.Utenti.OracleQueries.sel_consultori_utente

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim consultorioUtente As Entities.ConsultorioUtente = Nothing

                            Dim luc_ute_id As Int16 = idr.GetOrdinal("LUC_UTE_ID")
                            Dim luc_cns_codice As Int16 = idr.GetOrdinal("LUC_CNS_CODICE")
                            Dim luc_default As Int16 = idr.GetOrdinal("LUC_DEFAULT")
                            Dim cns_descrizione As Int16 = idr.GetOrdinal("CNS_DESCRIZIONE")

                            While idr.Read()

                                consultorioUtente = New Entities.ConsultorioUtente()

                                consultorioUtente.IdUtente = idr.GetInt64(luc_ute_id)
                                consultorioUtente.CodiceConsultorio = idr.GetString(luc_cns_codice)
                                consultorioUtente.ConsultorioDefault = idr.GetBooleanOrDefault(luc_default)
                                consultorioUtente.DescrizioneConsultorio = idr.GetString(cns_descrizione)
                                consultorioUtente.Abilitato = True

                                listConsultoriUtente.Add(consultorioUtente)

                            End While

                        End If

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listConsultoriUtente

        End Function

        ''' <summary>
        ''' Inserisce un'associazione tra utente e consultorio specificati
        ''' </summary>
        ''' <param name="consultorioUtente"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertConsultorioUtente(consultorioUtente As Entities.ConsultorioUtente) As Integer Implements IUtentiProvider.InsertConsultorioUtente

            Dim count As Integer = 0

            Using cmd As New OracleCommand("INSERT INTO T_ANA_LINK_UTENTI_CONSULTORI (LUC_UTE_ID, LUC_CNS_CODICE, LUC_DEFAULT) VALUES (:LUC_UTE_ID, :LUC_CNS_CODICE, :LUC_DEFAULT)", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("LUC_UTE_ID", consultorioUtente.IdUtente)
                    cmd.Parameters.AddWithValue("LUC_CNS_CODICE", consultorioUtente.CodiceConsultorio)
                    cmd.Parameters.AddWithValue("LUC_DEFAULT", IIf(consultorioUtente.ConsultorioDefault, "S", "N"))

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Inserisce un consenso all'utente relativo al codice locale o ausiliario del paziente specificato
        ''' </summary>
        ''' <param name="idUtente"></param>
        ''' <param name="codicePaziente"></param>
        ''' <param name="codiceAusiliarioPaziente"></param>
        ''' <param name="approvazione"></param>
        ''' <returns></returns>
        Public Function InserimentoConsensi(idUtente As Long, codicePaziente As Long, codiceAusiliarioPaziente As String, approvazione As String) As ResultSetPost Implements IUtentiProvider.InserimentoConsensi

            Dim count As Integer = 0

            Dim result As New ResultSetPost()

            Using cmd As New OracleCommand("INSERT INTO T_CONSENSI_UTENTI (COU_UTE_ID, COU_PAZ_CODICE, COU_PAZ_CODICE_AUSILIARIO, COU_DATA_INSERIMENTO, COU_VALORE) VALUES (:COU_UTE_ID, :COU_PAZ_CODICE, :COU_PAZ_CODICE_AUSILIARIO, :COU_DATA_INSERIMENTO, :COU_VALORE)", Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("COU_UTE_ID", idUtente)

                    If codicePaziente = 0 Then
                        cmd.Parameters.AddWithValue("COU_PAZ_CODICE", DBNull.Value)
                    Else
                        cmd.Parameters.AddWithValue("COU_PAZ_CODICE", codicePaziente)
                    End If

                    If String.IsNullOrWhiteSpace(codiceAusiliarioPaziente) Then
                        cmd.Parameters.AddWithValue("COU_PAZ_CODICE_AUSILIARIO", DBNull.Value)
                    Else
                        cmd.Parameters.AddWithValue("COU_PAZ_CODICE_AUSILIARIO", codiceAusiliarioPaziente)
                    End If

                    cmd.Parameters.AddWithValue("COU_DATA_INSERIMENTO", DateTime.Now)
                    cmd.Parameters.AddWithValue("COU_VALORE", approvazione)

                    count = cmd.ExecuteNonQuery()

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            If count = 1 Then
                result.Success = True
                result.Message = "Inserimento avvenuto con successo"
            Else
                result.Success = False
                result.Message = "Inserimento non avvenuto"
            End If

            Return result

        End Function

        ''' <summary>
        ''' Elimina l'associazione tra l'utente specificato e i suoi centri
        ''' </summary>
        ''' <param name="idUtente"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteConsultoriUtente(idUtente As Integer) As Integer Implements IUtentiProvider.DeleteConsultoriUtente

            Dim count As Integer = 0

            Using cmd As New OracleCommand("DELETE FROM T_ANA_LINK_UTENTI_CONSULTORI WHERE LUC_UTE_ID = :LUC_UTE_ID", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("LUC_UTE_ID", idUtente)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Elimina l'associazione tra il consultorio e l'utente specificati
        ''' </summary>
        ''' <param name="codiceConsultorio"></param>
        ''' <param name="idUtente"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteConsultorioUtente(codiceConsultorio As String, idUtente As Integer) As Integer Implements IUtentiProvider.DeleteConsultorioUtente

            Dim count As Integer = 0

            Using cmd As New OracleCommand("DELETE FROM T_ANA_LINK_UTENTI_CONSULTORI WHERE LUC_UTE_ID = :LUC_UTE_ID AND LUC_CNS_CODICE = :LUC_CNS_CODICE", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("LUC_UTE_ID", idUtente)
                    cmd.Parameters.AddWithValue("LUC_CNS_CODICE", codiceConsultorio)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Restituisce true se l'utente appartiene al gruppo specificato.
        ''' </summary>
        ''' <param name="idUtente"></param>
        ''' <param name="idGruppo"></param>
        ''' <returns></returns>
        ''' <remarks>Effettua la query direttamente nel DB del manager</remarks>
        Public Function IsUserInGroup(idUtente As Long, idGruppo As Integer) As Boolean Implements IUtentiProvider.IsUserInGroup

            ' TODO [Manager] : bisognerebbe usare una funzione ad hoc di Onit.Shared.Manager, ma non c'è.

            Dim isInGroup As Boolean = False

            Using conn As New OracleConnection(Onit.Shared.NTier.OnitNTierAppRuntime.Settings.OnitManager.ConnectionString)
                Using cmd As New OracleCommand("select lgu_gru_id from t_ana_link_gruppi_utenti where lgu_ute_id = :lgu_ute_id and lgu_gru_id = :lgu_gru_id", conn)

                    Try
                        cmd.Parameters.AddWithValue("lgu_ute_id", idUtente)
                        cmd.Parameters.AddWithValue("lgu_gru_id", idGruppo)

                        Dim obj As Object = cmd.ExecuteScalar()

                        isInGroup = Not obj Is Nothing AndAlso Not obj Is System.DBNull.Value

                    Catch ex As Exception
                        Return False
                    End Try

                End Using
            End Using

            Return isInGroup

        End Function

#End Region

#Region " Private "

        Private Function CreateQueryUtenti(cmd As OracleCommand, codiceDescrizioneLikeFilter As String, codiceConsultorio As String, appId As String, codiceAzienda As String, countQueryType As Boolean) As String

            Dim query As New System.Text.StringBuilder()

            If countQueryType Then
                query.Append("select count(*) ")
            Else
                query.Append("select ute_id, ute_codice, ute_cognome, ute_nome, ute_descrizione, ute_email, ute_firma, ute_obsoleto ")
            End If

            query.Append("from v_ana_utenti ")

            If Not String.IsNullOrEmpty(codiceConsultorio) Then
                query.Append("join t_ana_link_utenti_consultori on ute_id = luc_ute_id ")
            End If

            query.Append("where ute_azi_codice = :ute_azi_codice ")
            cmd.Parameters.AddWithValue("ute_azi_codice", codiceAzienda)

            If Not String.IsNullOrEmpty(codiceConsultorio) Then
                query.Append("and luc_cns_codice = :luc_cns_codice ")
                cmd.Parameters.AddWithValue("luc_cns_codice", codiceConsultorio)
            End If

            If Not String.IsNullOrEmpty(appId) Then
                query.Append("and ute_app_id = :ute_app_id ")
                cmd.Parameters.AddWithValue("ute_app_id", appId)
            End If

            If Not String.IsNullOrEmpty(codiceDescrizioneLikeFilter) Then
                query.Append("and (ute_codice like :filtro or ")
                query.Append("ute_descrizione like :filtro or ")
                query.Append("ute_cognome like :filtro or ")
                query.Append("ute_nome like :filtro) ")
                cmd.Parameters.AddWithValue("filtro", "%" + codiceDescrizioneLikeFilter + "%")
            End If

            If Not countQueryType Then
                query.Append("order by ute_codice, ute_cognome, ute_nome, ute_id")
            End If

            Return query.ToString()

        End Function

        Private Function GetListUtenti(cmd As OracleCommand) As List(Of Entities.Utente)

            Dim listUtenti As New List(Of Entities.Utente)()

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    Dim utente As Entities.Utente = Nothing

                    Dim ute_id As Int16 = idr.GetOrdinal("ute_id")
                    Dim ute_codice As Int16 = idr.GetOrdinal("ute_codice")
                    Dim ute_cognome As Int16 = idr.GetOrdinal("ute_cognome")
                    Dim ute_nome As Int16 = idr.GetOrdinal("ute_nome")
                    Dim ute_descrizione As Int16 = idr.GetOrdinal("ute_descrizione")
                    Dim ute_email As Int16 = idr.GetOrdinal("ute_email")
                    Dim ute_firma As Int16 = idr.GetOrdinal("ute_firma")
                    Dim ute_obsoleto As Int16 = idr.GetOrdinal("ute_obsoleto")

                    While idr.Read()

                        utente = New Entities.Utente()

                        utente.Id = idr.GetInt32OrDefault(ute_id)
                        utente.Codice = idr.GetStringOrDefault(ute_codice)
                        utente.Cognome = idr.GetStringOrDefault(ute_cognome)
                        utente.Nome = idr.GetStringOrDefault(ute_nome)
                        utente.Descrizione = idr.GetStringOrDefault(ute_descrizione)
                        utente.Email = idr.GetStringOrDefault(ute_email)
                        utente.Firma = idr.GetStringOrDefault(ute_firma)
                        utente.Obsoleto = idr.GetStringOrDefault(ute_obsoleto)

                        listUtenti.Add(utente)

                    End While

                End If

            End Using

            Return listUtenti

        End Function

#End Region

    End Class

End Namespace