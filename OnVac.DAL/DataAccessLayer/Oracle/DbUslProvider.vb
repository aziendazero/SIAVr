Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager

Namespace DAL.Oracle

    Public Class DbUslProvider
        Inherits DbProvider
        Implements IUslProvider

#Region " Constructors "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " Public "

        ''' <summary>
        ''' Restituisce la descrizione dell'usl in base al codice.
        ''' </summary>
        Public Function GetDescrizione(codUsl As String) As String Implements IUslProvider.GetDescrizione

            Dim descrizione As String = String.Empty

            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                cmd.CommandText = Queries.Usl.OracleQueries.selDescrizione
                cmd.Parameters.AddWithValue("codUsl", GetStringParam(codUsl, False))

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj Is Nothing OrElse obj Is DBNull.Value Then
                        descrizione = String.Empty
                    Else
                        descrizione = obj.ToString()
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return descrizione

        End Function


        ''' <summary>
        ''' Restituisce la descrizione dell'usl in base al codice.
        ''' </summary>
        Public Function GetCodiceRegioneAusl(codUsl As String) As String Implements IUslProvider.GetCodiceRegioneAusl

            Dim codiceRegione As String = String.Empty

            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                cmd.CommandText = Queries.Usl.OracleQueries.selCodiceRegione
                cmd.Parameters.AddWithValue("codUsl", GetStringParam(codUsl, False))

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj Is Nothing OrElse obj Is DBNull.Value Then
                        codiceRegione = String.Empty
                    Else
                        codiceRegione = obj.ToString()
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return codiceRegione

        End Function

		''' <summary>
		''' Restituisce una collection di oggetti di tipo Usl, contenente i dati di tutte le usl presenti nella tabella t_usl_gestite.
		''' Se non ci sono usl gestite, restituisce una collection vuota.
		''' </summary>
		Public Function GetUslGestite() As ICollection(Of Usl) Implements IUslProvider.GetUslGestite

			Dim uslGestiteList As List(Of Usl) = Nothing

			Using cmd As New OracleClient.OracleCommand(
				"SELECT UGS_APP_ID, UGS_USL_CODICE, UGS_CONSENSO_VACC_CENTR, UGS_ABILITAZIONE_VACC_CENTR, UGS_UNIFICATA FROM T_USL_GESTITE",
				Connection)

				Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    uslGestiteList = GetListUslGestite(cmd)

                Catch ex As Exception

                    Dim msg As New Text.StringBuilder()
                    msg.AppendLine("Query: SELECT UGS_APP_ID, UGS_USL_CODICE, UGS_CONSENSO_VACC_CENTR, UGS_ABILITAZIONE_VACC_CENTR, UGS_UNIFICATA FROM T_USL_GESTITE")
                    msg.AppendFormat("Connection: {0}", Connection.ConnectionString).AppendLine()

                    Common.Utility.EventLogHelper.EventLogWrite(ex, msg.ToString(), "OnVac")

                    ex.InternalPreserveStackTrace()
                    Throw

                Finally
					ConditionalCloseConnection(ownConnection)
				End Try

			End Using

			Return uslGestiteList

		End Function
		Public Function GetListaUslDistretto() As List(Of UslDistretto) Implements IUslProvider.GetUslDistretto

			Dim uslGestiteList As List(Of UslDistretto) = Nothing

			Using cmd As New OracleClient.OracleCommand(
				"SELECT DISTINCT USL_CODICE,USL_DESCRIZIONE FROM T_ANA_USL INNER JOIN T_ANA_DISTRETTI ON DIS_USL_CODICE = USL_CODICE ORDER BY USL_DESCRIZIONE",
				Connection)

				Dim ownConnection As Boolean = False

				Try
					ownConnection = ConditionalOpenConnection(cmd)

					uslGestiteList = GetListUslDistretti(cmd)

				Finally
					ConditionalCloseConnection(ownConnection)
				End Try

			End Using

			Return uslGestiteList

		End Function

		Public Function GetUslGestitaByCodiceComune(codiceComune As String) As Usl Implements IUslProvider.GetUslGestitaByCodiceComune

            Dim uslGestita As Usl = Nothing

            Using cmd As New OracleClient.OracleCommand(
                "SELECT UGS_APP_ID, UGS_USL_CODICE, UGS_CONSENSO_VACC_CENTR, UGS_ABILITAZIONE_VACC_CENTR, UGS_UNIFICATA 
                 FROM T_USL_GESTITE INNER JOIN T_ANA_LINK_COMUNI_USL ON UGS_USL_CODICE = LCU_USL_CODICE WHERE LCU_COM_CODICE = :codiceComune",
                Connection)

                cmd.Parameters.AddWithValue("codiceComune", codiceComune)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim list As List(Of Usl) = GetListUslGestite(cmd)

                    If Not list.IsNullOrEmpty() Then

                        uslGestita = list.FirstOrDefault()

                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return uslGestita

        End Function

        ''' <summary>
        ''' Restituisce un array di stringhe contenente i codici delle usl associate al comune specificato.
        ''' Se non ci sono usl associate al comune, o se il comune specificato è nullo, restituisce un array vuoto.
        ''' </summary>
        Public Function GetCodiciUslByComune(codiceComune As String) As String() Implements IUslProvider.GetCodiciUslByComune

            Dim codiciUsl As New List(Of String)()

            Using cmd As New OracleClient.OracleCommand(Queries.Usl.OracleQueries.selUslByComune, Me.Connection)

                cmd.Parameters.AddWithValue("codiceComune", GetStringParam(codiceComune, False))

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using oracleReader As OracleClient.OracleDataReader = cmd.ExecuteReader()

                        Dim lcu_usl_codice_ordinal As Integer = oracleReader.GetOrdinal("LCU_USL_CODICE")

                        While oracleReader.Read()

                            codiciUsl.Add(oracleReader(lcu_usl_codice_ordinal))

                        End While

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return codiciUsl.ToArray()

        End Function

        ''' <summary>
        ''' Restituisce una collection di oggetti di tipo Usl, contenente i dati di tutte le usl presenti nella tabella t_usl_gestite.
        ''' Se non ci sono usl gestite, restituisce una collection vuota.
        ''' </summary>
        Public Sub UpdateUslGestita(uslGestita As Usl) Implements IUslProvider.UpdateUslGestita

            Using cmd As New OracleClient.OracleCommand("update  T_USL_GESTITE set UGS_CONSENSO_VACC_CENTR = :UGS_CONSENSO_VACC_CENTR, UGS_ABILITAZIONE_VACC_CENTR = :UGS_ABILITAZIONE_VACC_CENTR WHERE UGS_APP_ID = :UGS_APP_ID and UGS_USL_CODICE = :UGS_USL_CODICE",
                                                        Connection)

                cmd.Parameters.AddWithValue("UGS_CONSENSO_VACC_CENTR", IIf(uslGestita.FlagConsensoDatiVaccinaliCentralizzati, "S", "N"))
                cmd.Parameters.AddWithValue("UGS_ABILITAZIONE_VACC_CENTR", IIf(uslGestita.FlagAbilitazioneDatiVaccinaliCentralizzati, "S", "N"))
                cmd.Parameters.AddWithValue("UGS_APP_ID", GetStringParam(uslGestita.IDApplicazione))
                cmd.Parameters.AddWithValue("UGS_USL_CODICE", GetStringParam(uslGestita.Codice))

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim rCount As Integer = cmd.ExecuteNonQuery()

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

        End Sub

        Public Function GetCodiceAifa(codUsl As String) As String Implements IUslProvider.GetCodiceAifa

			Dim codiceAifa As String = String.Empty

			Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

				cmd.CommandText = Queries.Usl.OracleQueries.selCodiceAifa
				cmd.Parameters.AddWithValue("codUsl", GetStringParam(codUsl, False))

				Dim ownConnection As Boolean = False

				Try
					ownConnection = ConditionalOpenConnection(cmd)

					Dim obj As Object = cmd.ExecuteScalar()

					If obj Is Nothing OrElse obj Is DBNull.Value Then
						codiceAifa = String.Empty
					Else
						codiceAifa = obj.ToString()
					End If

				Finally
					ConditionalCloseConnection(ownConnection)
				End Try

			End Using

			Return codiceAifa

		End Function

        ''' <summary>
        ''' Restituisce la usl unificata in base al codice
        ''' </summary>
        ''' <param name="codiceUsl"></param>
        ''' <returns></returns>
        Public Function GetUslUnificataByCodiceUsl(codiceUsl As String) As UslUnificata Implements IUslProvider.GetUslUnificataByCodiceUsl

            Dim uslUnificata As UslUnificata = Nothing

            Using cmd As New OracleClient.OracleCommand(GetQuerySelectUslUnificate() + " WHERE USU_CODICE = :USU_CODICE", Connection)

                cmd.Parameters.AddWithValue("USU_CODICE", codiceUsl)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    uslUnificata = GetUslUnificata(cmd)

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return uslUnificata

        End Function

		''' <summary>
		''' Restituisce la usl unificata in base all'app_id
		''' </summary>
		''' <param name="idApplicazione"></param>
		''' <returns></returns>
		Public Function GetUslUnificataByIdApplicazione(idApplicazione As String) As UslUnificata Implements IUslProvider.GetUslUnificataByIdApplicazione

			Dim uslUnificata As UslUnificata = Nothing

            Using cmd As New OracleClient.OracleCommand(GetQuerySelectUslUnificate() + " WHERE USU_APP_ID = :USU_APP_ID", Connection)

                cmd.Parameters.AddWithValue("USU_APP_ID", idApplicazione)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    uslUnificata = GetUslUnificata(cmd)

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return uslUnificata

		End Function
		Public Function GetUslUnificataByCodiceAcn(codiceAcn As String) As UslUnificata Implements IUslProvider.GetUslUnificataByCodiceAcn

			Dim uslUnificata As UslUnificata = Nothing

            Using cmd As New OracleClient.OracleCommand(GetQuerySelectUslUnificate() + " WHERE USU_CODICE_ACN = :USU_CODICE_ACN", Connection)

                cmd.Parameters.AddWithValue("USU_CODICE_ACN", codiceAcn)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    uslUnificata = GetUslUnificata(cmd)

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return uslUnificata

		End Function

		Public Function IsInUslUnificata(codiceUslLocale As String, codiceUslUnificata As String) As Boolean Implements IUslProvider.IsInUslUnificata

            Dim result As Boolean = False

            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                cmd.CommandText = "SELECT 1 FROM T_ANA_DISTRETTI WHERE DIS_CODICE = :DIS_CODICE AND DIS_USL_CODICE = :DIS_USL_CODICE"
                cmd.Parameters.AddWithValue("dis_codice", codiceUslLocale)
                cmd.Parameters.AddWithValue("dis_usl_codice", codiceUslUnificata)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        result = True
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return result

        End Function

        ''' <summary>
        ''' Partendo dall'app_id di una ulss gestita, restituisce il codice della usl unificata relativa
        ''' </summary>
        ''' <param name="idApplicazioneUslGestita"></param>
        ''' <returns></returns>
        Public Function GetCodiceUslUnificataByIdApplicazioneUslGestita(idApplicazioneUslGestita As String) As String Implements IUslProvider.GetCodiceUslUnificataByIdApplicazioneUslGestita

            Dim codiceUsl As String = String.Empty

            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                cmd.CommandText = "select dis_usl_codice from t_usl_gestite join t_ana_distretti on ugs_usl_codice = dis_codice where ugs_app_id = :ugs_app_id"
                cmd.Parameters.AddWithValue("ugs_app_id", idApplicazioneUslGestita)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        codiceUsl = obj.ToString()
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return codiceUsl

        End Function

        ''' <summary>
        ''' Restituisce il codice usl e l'id applicativo (nuovi se già unificati, altrimenti vecchi) in base all'app_id
        ''' </summary>
        ''' <param name="idApplicativo"></param>
        ''' <returns></returns>
        Public Function GetInfoAziendaByIdApplicativo(idApplicativo As String) As UslApplicativo Implements IUslProvider.GetInfoAziendaByIdApplicativo

            Dim info As UslApplicativo = Nothing

            Using cmd As New OracleClient.OracleCommand("SELECT MUU_USL_CODICE_NEW, MUU_APP_ID_NEW FROM T_MAP_ULSS_UNIFICAZIONE WHERE MUU_APP_ID_OLD = :MUU_APP_ID_OLD", Connection)

                cmd.Parameters.AddWithValue("MUU_APP_ID_OLD", idApplicativo)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim muu_usl_codice_new As Integer = idr.GetOrdinal("MUU_USL_CODICE_NEW")
                            Dim muu_app_id_new As Integer = idr.GetOrdinal("MUU_APP_ID_NEW")

                            If idr.Read() Then

                                Dim codiceUsl_new As String = idr.GetStringOrDefault(muu_usl_codice_new)
                                Dim idAppl_new As String = idr.GetStringOrDefault(muu_app_id_new)

                                info = New UslApplicativo()

                                info.CodiceUsl = codiceUsl_new
                                info.IdApplicazione = idAppl_new

                            End If

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return info

        End Function

		''' <summary>
		''' Restituisce true se la usl specificata fa parte di quelle già unificate
		''' </summary>
		''' <param name="codiceUsl"></param>
		''' <returns></returns>
		Public Function IsUnificata(codiceUsl As String) As Boolean Implements IUslProvider.IsUnificata

			Dim result As Boolean = False

			Using cmd As New OracleClient.OracleCommand("select 1 from T_MAP_ULSS_UNIFICAZIONE join T_USL_UNIFICATE On MUU_USL_CODICE_NEW = USU_CODICE where MUU_USL_CODICE_OLD = :MUU_USL_CODICE_OLD", Connection)

				cmd.Parameters.AddWithValue("MUU_USL_CODICE_OLD", codiceUsl)

				Dim ownConnection As Boolean = False

				Try
					ownConnection = ConditionalOpenConnection(cmd)

					Dim obj As Object = cmd.ExecuteScalar()

					result = (obj IsNot Nothing AndAlso obj IsNot DBNull.Value)

				Finally
					ConditionalCloseConnection(ownConnection)
				End Try

			End Using

			Return result

		End Function

		Public Function IsAutenticatoRicercaLotti(codiceUsl As String, user As String, pw As String) As Boolean Implements IUslProvider.IsAutenticatoRicercaLotti

			Dim result As Boolean = False

			Using cmd As New OracleClient.OracleCommand("select 1 from T_USL_UNIFICATE where USU_CODICE = :USU_CODICE and USU_RICERCA_LOTTI_USER=:USU_RICERCA_LOTTI_USER and USU_RICERCA_LOTTI_PW=:USU_RICERCA_LOTTI_PW", Connection)

				cmd.Parameters.AddWithValue("USU_CODICE", codiceUsl)
				cmd.Parameters.AddWithValue("USU_RICERCA_LOTTI_USER", user)
                cmd.Parameters.AddWithValue("USU_RICERCA_LOTTI_PW", pw)

                Dim ownConnection As Boolean = False

				Try
					ownConnection = ConditionalOpenConnection(cmd)

					Dim obj As Object = cmd.ExecuteScalar()

					result = (obj IsNot Nothing AndAlso obj IsNot DBNull.Value)

				Finally
					ConditionalCloseConnection(ownConnection)
				End Try

			End Using

			Return result

		End Function

#End Region

#Region " Private "

		Private Function GetListUslGestite(cmd As OracleClient.OracleCommand) As List(Of Usl)

			Dim uslGestiteList As New List(Of Usl)()

			Using oracleReader As OracleClient.OracleDataReader = cmd.ExecuteReader()

				Dim ugs_app_id As Integer = oracleReader.GetOrdinal("UGS_APP_ID")
				Dim ugs_usl_codice As Integer = oracleReader.GetOrdinal("UGS_USL_CODICE")
				Dim ugs_consenso_vacc_centr As Integer = oracleReader.GetOrdinal("UGS_CONSENSO_VACC_CENTR")
				Dim ugs_abilitazione_vacc_centr As Integer = oracleReader.GetOrdinal("UGS_ABILITAZIONE_VACC_CENTR")
				Dim ugs_unificata As Integer = oracleReader.GetOrdinal("UGS_UNIFICATA")

				While oracleReader.Read()

					Dim uslGestita As New Usl()

					uslGestita.Codice = oracleReader.GetString(ugs_usl_codice)
					uslGestita.IDApplicazione = oracleReader.GetStringOrDefault(ugs_app_id)
					uslGestita.FlagConsensoDatiVaccinaliCentralizzati = oracleReader.GetBooleanOrDefault(ugs_consenso_vacc_centr)
					uslGestita.FlagAbilitazioneDatiVaccinaliCentralizzati = oracleReader.GetBooleanOrDefault(ugs_abilitazione_vacc_centr)
					uslGestita.IsApplicazioneUnificata = oracleReader.GetBooleanOrDefault(ugs_unificata)

					uslGestiteList.Add(uslGestita)

				End While

			End Using

			Return uslGestiteList

		End Function
		Private Function GetListUslDistretti(cmd As OracleClient.OracleCommand) As List(Of UslDistretto)

			Dim uslDistrettoList As New List(Of UslDistretto)()

			Using oracleReader As OracleClient.OracleDataReader = cmd.ExecuteReader()


				Dim usl_codice As Integer = oracleReader.GetOrdinal("USL_CODICE")
				Dim usl_descrizione As Integer = oracleReader.GetOrdinal("USL_DESCRIZIONE")


				While oracleReader.Read()

					Dim uslDistretto As New UslDistretto()

					uslDistretto.Codice = oracleReader.GetString(usl_codice)
					uslDistretto.Descrizione = oracleReader.GetStringOrDefault(usl_descrizione)


					uslDistrettoList.Add(uslDistretto)

				End While

			End Using

			Return uslDistrettoList

		End Function

        Private Function GetQuerySelectUslUnificate() As String

            Return "SELECT USU_CODICE, USU_APP_ID, USU_CODICE_AIFA, USU_CODICE_ACN, USU_REPOSITORY_UNIQUE_ID FROM T_USL_UNIFICATE "

        End Function

        Private Function GetUslUnificata(cmd As OracleClient.OracleCommand) As UslUnificata

            Dim uslUnificata As UslUnificata = Nothing

            Using idr As IDataReader = cmd.ExecuteReader()

                Dim usu_codice As Integer = idr.GetOrdinal("USU_CODICE")
                Dim usu_app_id As Integer = idr.GetOrdinal("USU_APP_ID")
                Dim usu_codice_aifa As Integer = idr.GetOrdinal("USU_CODICE_AIFA")
                Dim usu_codice_acn As Integer = idr.GetOrdinal("USU_CODICE_ACN")
                Dim usu_repository_unique_id As Integer = idr.GetOrdinal("USU_REPOSITORY_UNIQUE_ID")

                If idr.Read() Then

                    uslUnificata = New UslUnificata()

                    uslUnificata.CodiceUsl = idr.GetString(usu_codice)
                    uslUnificata.IdApplicazione = idr.GetString(usu_app_id)
                    uslUnificata.CodiceAifaUsl = idr.GetStringOrDefault(usu_codice_aifa)
                    uslUnificata.CodiceAcn = idr.GetStringOrDefault(usu_codice_acn)
                    uslUnificata.RepositoryUniqueId = idr.GetStringOrDefault(usu_repository_unique_id)

                End If

            End Using

            Return uslUnificata

        End Function

#End Region

    End Class

End Namespace
