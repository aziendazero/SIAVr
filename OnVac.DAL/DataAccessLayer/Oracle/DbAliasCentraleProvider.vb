Imports System.Collections.Generic
Imports System.Data.OracleClient
Imports System.Text.RegularExpressions

Imports Onit.OnAssistnet.Data
Imports Onit.OnAssistnet.Data.OracleClient

Imports Onit.Database.DataAccessManager


Namespace DAL

    Public Class DbAliasCentraleProvider
        Inherits DbProvider
        Implements IAliasCentraleProvider

#Region " Private members "

        Private IdUtente As Int64

#End Region

#Region " Constructors "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

            Me.IdUtente = -1

        End Sub

        Public Sub New(ByRef DAM As IDAM, idUtente As Int64)

            MyBase.New(DAM)

            Me.IdUtente = idUtente

        End Sub

#End Region

#Region " Public methods "

#Region " Copia dell'alias da anagrafe pazienti ad anagrafe pazienti alias "

        ''' <summary>
        ''' Creazione copia dell'alias nella t_tmp_pazienti_alias
        ''' </summary>
        Public Function CloneAlias(codicePazienteMaster As String, codicePazienteAlias As String, dataAlias As DateTime, idUtente As Integer, aziendaProvenienza As String) As Integer Implements IAliasCentraleProvider.CloneAlias

            Dim count As Integer = 0

            Using cmd As OracleCommand = Me.Connection.CreateCommand()

                Dim elencoCampi As String = Me.GetElencoCampiTabellaAlias(True)

                ' Inserimento alias nella t_tmp_pazienti_alias
                cmd.CommandText = String.Format(Queries.Alias.OracleQueries.insAliasIntoTempCentrale, elencoCampi, elencoCampi)

                cmd.Parameters.AddWithValue("codMaster", codicePazienteMaster)
                cmd.Parameters.AddWithValue("dataAlias", GetDateParam(dataAlias))
                cmd.Parameters.AddWithValue("idUtente", GetIntParam(idUtente))
                cmd.Parameters.AddWithValue("codAlias", GetStringParam(codicePazienteAlias))
                cmd.Parameters.AddWithValue("paz_pac_azienda", GetStringParam(aziendaProvenienza))

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    count = cmd.ExecuteNonQuery()

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return count

        End Function

#End Region

#End Region

#Region " Private methods "

        ''' <summary>
        ''' Restituisce una stringa contenente i campi della t_tmp_pazienti_alias, letti dal catalogo di oracle
        ''' Se il parametro vale true, devono essere esclusi i campi paz_codice_master, paz_data_alias, paz_ute_id
        ''' </summary>
        Private Function GetElencoCampiTabellaAlias(soloDatiAnagrafici As Boolean) As String

            Dim listAliasColumns As New List(Of String)

            Using cmd As OracleCommand = Me.Connection.CreateCommand()

                If soloDatiAnagrafici Then
                    cmd.CommandText = Queries.Alias.OracleQueries.selColumnsDatiAliasFromCatalogCentrale
                Else
                    cmd.CommandText = Queries.Alias.OracleQueries.selAllColumnsFromCatalogCentrale
                End If

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then
                            Dim column_name As Integer = idr.GetOrdinal("column_name")

                            While idr.Read()

                                listAliasColumns.Add(idr.GetString(column_name))

                            End While
                        End If

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return String.Join(",", listAliasColumns.ToArray())

        End Function

#End Region

    End Class

End Namespace
