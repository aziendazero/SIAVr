Imports System.Collections.Generic
Imports System.Data.OracleClient
Imports System.Text
Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.DAL

Public Class DbTag
    Inherits DbProvider
    Implements ITagDal

    Public Sub New(ByRef mad As IDAM)
        MyBase.New(mad)
    End Sub

    Public Function CercaTag(gruppo As String, filtro As String) As IEnumerable(Of Tag) Implements ITagDal.CercaTag
        Dim ritorno As New List(Of Tag)
        Dim ownConnection As Boolean = False
        Using cmd As New OracleCommand
            cmd.Connection = Connection
            Try
                ownConnection = ConditionalOpenConnection(cmd)
                cmd.CommandText = "select TAG_ID, TAG_GRUPPO, TAG_DESCRIZIONE, TAG_COLORE from T_ANA_TAG WHERE TAG_GRUPPO = :gruppo and TAG_DESCRIZIONE LIKE :filtro"
                cmd.Parameters.AddWithValue("gruppo", gruppo)
                If String.IsNullOrWhiteSpace(filtro) Then
                    filtro = ""
                End If
                cmd.Parameters.AddWithValue("filtro", String.Format("{0}%", filtro.ToUpper()))

                Using reader As OracleDataReader = cmd.ExecuteReader()
                    Dim id As Integer = reader.GetOrdinal("TAG_ID")
                    Dim g As Integer = reader.GetOrdinal("TAG_GRUPPO")
                    Dim descrizione As Integer = reader.GetOrdinal("TAG_DESCRIZIONE")
                    Dim colore As Integer = reader.GetOrdinal("TAG_COLORE")
                    While reader.Read()
                        Dim nuovo As New Tag
                        nuovo.Id = reader.GetInt64(id)
                        nuovo.Gruppo = reader.GetString(g)
                        nuovo.Descrizione = reader.GetString(descrizione)
                        nuovo.Colore = reader.GetStringOrDefault(colore)
                        ritorno.Add(nuovo)
                    End While
                End Using
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
        End Using
        Return ritorno
    End Function

    Public Function GetTags(e As IEnumerable(Of Long)) As IEnumerable(Of Tag) Implements ITagDal.GetTags
        Dim elementi As List(Of Long) = e.ToList()
        Dim ownConnection As Boolean = False
        Dim ritorno As New List(Of Tag)


        Using cmd As New OracleCommand
            Try
                cmd.Connection = Connection
                ownConnection = ConditionalOpenConnection(cmd)
                Dim command As New StringBuilder("select TAG_ID, TAG_GRUPPO, TAG_DESCRIZIONE, TAG_COLORE from T_ANA_TAG WHERE TAG_ID in (")

                For i As Integer = 0 To elementi.Count - 2
                    Dim n As String = String.Format("s_{0}", i)
                    command.Append(String.Format(":{0}, ", n))
                    cmd.Parameters.AddWithValue(n, elementi.Item(i))
                Next

                Dim nomePar As String = String.Format("s_{0}", elementi.Count)
                command.Append(String.Format(":{0})", nomePar))
                cmd.Parameters.AddWithValue(nomePar, elementi.Last())

                cmd.CommandText = command.ToString()

                Using reader As OracleDataReader = cmd.ExecuteReader()
                    Dim id As Integer = reader.GetOrdinal("TAG_ID")
                    Dim g As Integer = reader.GetOrdinal("TAG_GRUPPO")
                    Dim descrizione As Integer = reader.GetOrdinal("TAG_DESCRIZIONE")
                    Dim colore As Integer = reader.GetOrdinal("TAG_COLORE")
                    While reader.Read()
                        Dim nuovo As New Tag
                        nuovo.Id = reader.GetInt64(id)
                        nuovo.Gruppo = reader.GetString(g)
                        nuovo.Descrizione = reader.GetString(descrizione)
                        nuovo.Colore = reader.GetStringOrDefault(colore)
                        ritorno.Add(nuovo)
                    End While
                End Using
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
        End Using
        Return ritorno
    End Function
End Class
