Imports Onit.Database.DataAccessManager
Imports System.Collections.Generic


Namespace DAL.Oracle

    Public Class DbEventiNotificheProvider
        Inherits DbProvider
        Implements IEventiNotificaProvider

#Region " Costruttori "

        Public Sub New(ByRef DAM As IDAM)
            MyBase.New(DAM)
        End Sub

#End Region

#Region " Public "

        Public Function GetEventoNotificaByCodice(codiceEvento As String) As EventoNotifica Implements IEventiNotificaProvider.GetEventoNotificaByCodice

            Dim evento As New EventoNotifica()

            Dim query As New Text.StringBuilder()

            query.Append("SELECT * ")
            query.Append(" FROM t_ana_eventi ")
            query.Append(" WHERE evn_codice = :evn_codice ")

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Connection

                cmd.Parameters.AddWithValue("evn_codice", GetStringParam(codiceEvento))
                cmd.CommandText = query.ToString()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim evn_id As Integer = idr.GetOrdinal("evn_id")
                            Dim evn_codice As Integer = idr.GetOrdinal("evn_codice")
                            Dim evn_descrizione As Integer = idr.GetOrdinal("evn_descrizione")

                            If idr.Read() Then
                                evento.IdEvento = idr.GetInt32(evn_id)
                                evento.Codice = idr.GetStringOrDefault(evn_codice)
                                evento.Descrizione = idr.GetStringOrDefault(evn_descrizione)
                            End If

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return evento

        End Function

        Public Function GetFunzionalitaNotificaByCodice(codiceFunzionalita As String) As FunzionalitaNotifica Implements IEventiNotificaProvider.GetFunzionalitaNotificaByCodice

            Dim funzionalita As New FunzionalitaNotifica()

            Dim query As New Text.StringBuilder()

            query.Append("SELECT * ")
            query.Append(" FROM t_ana_funzionalita ")
            query.Append(" WHERE fun_codice = :fun_codice ")

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Connection

                cmd.Parameters.AddWithValue("fun_codice", GetStringParam(codiceFunzionalita))
                cmd.CommandText = query.ToString()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim fun_id As Integer = idr.GetOrdinal("fun_id")
                            Dim fun_codice As Integer = idr.GetOrdinal("fun_codice")
                            Dim fun_descrizione As Integer = idr.GetOrdinal("fun_descrizione")

                            If idr.Read() Then
                                funzionalita.IdFunzionalita = idr.GetInt32(fun_id)
                                funzionalita.Codice = idr.GetStringOrDefault(fun_codice)
                                funzionalita.Descrizione = idr.GetStringOrDefault(fun_descrizione)
                            End If

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return funzionalita

        End Function

        ''' <summary>
        ''' Restituisce la lista dei poli a cui inviare la notifica relativa all'evento e funzionalità specificati
        ''' </summary>
        ''' <param name="idEvento"></param>
        ''' <param name="idFunzionalita"></param>
        ''' <returns></returns>
        Public Function GetIdPoliAbilitatiNotifica(idEvento As Integer, idFunzionalita As Integer) As List(Of Integer) Implements IEventiNotificaProvider.GetIdPoliAbilitatiNotifica

            Dim list As New List(Of Integer)()

            Dim query As New Text.StringBuilder()

            query.Append("SELECT pol_id ")
            query.Append(" FROM t_ana_link_eventi_poli_funz ")
            query.Append(" JOIN t_ana_poli on epf_pol_id = pol_id ")
            query.Append(" WHERE pol_abilitato_notifiche_vac = 'S' ")
            query.Append(" AND epf_invio_abilitato = 'S' ")
            query.Append(" AND epf_evn_id = :epf_evn_id ")
            query.Append(" AND epf_fun_id = :epf_fun_id ")

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Connection
                cmd.CommandText = query.ToString()

                cmd.Parameters.AddWithValue("epf_evn_id", idEvento)
                cmd.Parameters.AddWithValue("epf_fun_id", idFunzionalita)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim pol_id As Integer = idr.GetOrdinal("pol_id")

                            While idr.Read()

                                list.Add(idr.GetInt32OrDefault(pol_id))

                            End While

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list

        End Function

#End Region

    End Class

End Namespace
