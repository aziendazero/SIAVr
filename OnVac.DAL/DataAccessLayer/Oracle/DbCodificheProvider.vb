Imports Onit.Database.DataAccessManager


Namespace DAL.Oracle

    Public Class DbCodificheProvider
        Inherits DbProvider
        Implements ICodificheProvider

#Region " Costruttori "

        Public Sub New(DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " ICodificheProvider "

        ''' <summary>
        ''' Restituisce una collection di oggetti di tipo codifica, ognuno dei quali rappresenta un record della t_ana_codifiche.
        ''' Se il campo specificato non esiste, restituisce la collection vuota.
        ''' </summary>
        Public Function GetCodifiche(campo As String) As CodificheCollection Implements ICodificheProvider.GetCodifiche

            Dim collCodifiche As CodificheCollection

            Dim ownConnection As Boolean = False

            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                cmd.Parameters.AddWithValue("cod_campo", GetStringParam(campo, False))

                cmd.CommandText = Queries.Codifiche.OracleQueries.selCodifiche

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    collCodifiche = GetListCodifiche(cmd)

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return collCodifiche

        End Function

        ''' <summary>
        ''' Restituisce un oggetto di tipo Codifica, in base al campo e al codice specificati.
        ''' </summary>
        Public Function GetCodifica(campo As String, codice As String) As Codifica Implements ICodificheProvider.GetCodifica

            Dim codifica As Codifica = Nothing

            Dim ownConnection As Boolean = False

            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                cmd.Parameters.AddWithValue("cod_campo", GetStringParam(campo, False))
                cmd.Parameters.AddWithValue("cod_codice", GetStringParam(codice, False))

                cmd.CommandText = Queries.Codifiche.OracleQueries.selCodifica

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim collCodifiche As CodificheCollection = GetListCodifiche(cmd)

                    If collCodifiche.Count > 0 Then
                        codifica = collCodifiche(0)
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return codifica

        End Function

        Private Function GetListCodifiche(cmd As OracleClient.OracleCommand) As CodificheCollection

            Dim collCodifiche As New CodificheCollection()

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    Dim pos_campo As Int16 = idr.GetOrdinal("cod_campo")
                    Dim pos_cod As Int16 = idr.GetOrdinal("cod_codice")
                    Dim pos_descr As Int16 = idr.GetOrdinal("cod_descrizione")
                    Dim pos_default As Int16 = idr.GetOrdinal("cod_default")

                    While idr.Read()

                        Dim codifica As New Codifica()

                        codifica.Campo = idr.GetString(pos_campo)
                        codifica.Codice = idr.GetString(pos_cod)
                        codifica.Descrizione = idr.GetStringOrDefault(pos_descr)
                        codifica.CodificaDefault = idr.GetBooleanOrDefault(pos_default)

                        collCodifiche.Add(codifica)

                    End While

                End If

            End Using

            Return collCodifiche

        End Function

#End Region

    End Class

End Namespace
