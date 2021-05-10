Imports System.Data.OracleClient
Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager


Namespace DAL.Oracle

    Public Class DbMotiviEsclusioneProvider
        Inherits DbProvider
        Implements IMotiviEsclusioneProvider

#Region " Costruttori "

        Public Sub New(DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " IMotiviEsclusioneProvider "

        Public Function GetCodiciMotiviCentralizzati() As IEnumerable(Of String) Implements IMotiviEsclusioneProvider.GetCodiciMotiviCentralizzati

            Dim motiviCentralizzati As New List(Of String)()

            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim ownConnection As Boolean = False

            Try

                cmd = New OracleClient.OracleCommand("SELECT MOE_CODICE FROM T_ANA_MOTIVI_ESCLUSIONE WHERE MOE_FLAG_CENTRALIZZATO = :MOE_FLAG_CENTRALIZZATA", Me.Connection)
                cmd.Parameters.AddWithValue("MOE_FLAG_CENTRALIZZATA", "S")

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using reader As OracleDataReader = cmd.ExecuteReader()

                    While reader.Read()
                        motiviCentralizzati.Add(reader.GetString(0))
                    End While

                End Using

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return motiviCentralizzati.AsEnumerable()

        End Function

        Public Function GetMotivoEsclusione(codiceEsclusione As String) As MotivoEsclusione Implements IMotiviEsclusioneProvider.GetMotivoEsclusione

            Dim cmd As OracleCommand = Nothing
            Dim motivoEsclusione As New MotivoEsclusione()

            Dim ownConnection As Boolean = False

            Try

                cmd = Me.Connection.CreateCommand

                cmd.Parameters.AddWithValue("moe_codice", GetStringParam(codiceEsclusione, False))
                cmd.CommandText = OnVac.Queries.MotiviEsclusione.OracleQueries.selCalcoloScadenzaMotivoEsclusione

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Dim obj As Object = cmd.ExecuteScalar()

                If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then

                    motivoEsclusione.CodiceEsclusione = codiceEsclusione
                    motivoEsclusione.CalcoloScadenza = DirectCast([Enum].Parse(GetType(Enumerators.MotiviEsclusioneCalcoloScadenza), obj.ToString()), Enumerators.MotiviEsclusioneCalcoloScadenza)

                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("mos_moe_codice", GetStringParam(codiceEsclusione, False))
                    cmd.CommandText = OnVac.Queries.MotiviEsclusione.OracleQueries.selScadenzeMotiviEsclusione

                    Using idr As OracleDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim mos_id As Integer = idr.GetOrdinal("mos_id")
                            Dim mos_mesi As Integer = idr.GetOrdinal("mos_mesi")
                            Dim mos_anni As Integer = idr.GetOrdinal("mos_anni")
                            Dim mos_lst_vac_codice As Integer = idr.GetOrdinal("mos_lst_vac_codice")

                            While idr.Read()

                                Dim item As New MotivoEsclusione.Scadenza()
                                item.ID = idr.GetInt32(mos_id)
                                item.Mesi = idr.GetInt32OrDefault(mos_mesi)
                                item.Anni = idr.GetInt32OrDefault(mos_anni)
                                item.CodiciVaccinazioni = New List(Of String)(idr.GetStringOrDefault(mos_lst_vac_codice).Split("|"))

                                motivoEsclusione.Scadenze.Add(item)

                            End While

                        End If

                    End Using

                End If

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return motivoEsclusione

        End Function

        Public Function MotivoEsclusioneGeneraInadempienza(codiceMotivoEsclusione As String) As Boolean Implements IMotiviEsclusioneProvider.MotivoEsclusioneGeneraInadempienza

            With _DAM.QB
                .NewQuery()
                .AddTables("T_ANA_MOTIVI_ESCLUSIONE")
                .AddSelectFields("MOE_GENERA_INAD")
                .AddWhereCondition("MOE_CODICE", Comparatori.Uguale, codiceMotivoEsclusione, DataTypes.Stringa)
            End With

            Dim obj As Object = _DAM.ExecScalar()

            If obj Is Nothing OrElse obj Is DBNull.Value OrElse obj.ToString() = "N" Then
                Return False
            End If

            Return True

        End Function

        Public Function GetDataTableMotiviEsclusione() As DataTable Implements IMotiviEsclusioneProvider.GetDataTableMotiviEsclusione

            Dim dt As New DataTable()

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("MOE_CODICE, MOE_DESCRIZIONE")
                .AddTables("T_ANA_MOTIVI_ESCLUSIONE")
                .AddOrderByFields("MOE_DESCRIZIONE")
            End With

            _DAM.BuildDataTable(dt)

            Return dt

        End Function

        ''' <summary>
        ''' Restituisce il motivo di esclusione configurato come default per l'inadempienza. Filtra i motivi obsoleti.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCodiceMotivoEsclusioneDefaultInadempienza() As String Implements IMotiviEsclusioneProvider.GetCodiceMotivoEsclusioneDefaultInadempienza

            With _DAM.QB
                .NewQuery()
                .AddTables("T_ANA_MOTIVI_ESCLUSIONE")
                .AddSelectFields("MOE_CODICE")
                .AddWhereCondition("MOE_DEFAULT_INAD", Comparatori.Uguale, "S", DataTypes.Stringa)
                .AddWhereCondition("MOE_OBSOLETO", Comparatori.Uguale, "N", DataTypes.Stringa)
            End With

            Dim obj As Object = _DAM.ExecScalar()

            If obj Is Nothing OrElse obj Is DBNull.Value Then
                Return String.Empty
            End If

            Return obj.ToString()

        End Function

#Region " Gestione scadenze "

        Public Function GetScadenzeEsclusioneDt(codiceEsclusione As String) As System.Data.DataTable Implements IMotiviEsclusioneProvider.GetScadenzeEsclusioneDt

            Dim cmd As OracleCommand = Nothing
            Dim adp As OracleDataAdapter = Nothing

            Dim dt As New DataTable()

            Dim ownConnection As Boolean = False

            Try
                cmd = Me.Connection.CreateCommand()

                cmd.Parameters.AddWithValue("mos_moe_codice", GetStringParam(codiceEsclusione, False))
                cmd.CommandText = Queries.MotiviEsclusione.OracleQueries.selScadenzeMotiviEsclusione

                ownConnection = Me.ConditionalOpenConnection(cmd)

                adp = New OracleDataAdapter(cmd)
                adp.Fill(dt)

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not adp Is Nothing Then adp.Dispose()
                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return dt

        End Function

        Public Function DeleteScadenzaEsclusione(id As Integer) As Integer Implements IMotiviEsclusioneProvider.DeleteScadenzaEsclusione

            Dim cmd As OracleCommand = Nothing
            Dim ownConnection As Boolean = False

            Try

                cmd = Me.Connection.CreateCommand

                cmd.CommandText = OnVac.Queries.MotiviEsclusione.OracleQueries.delScadenzaMotiviEsclusione
                cmd.Parameters.AddWithValue("mos_id", GetStringParam(id, False))

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Return cmd.ExecuteNonQuery()

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

        End Function

        Public Function InsertScadenzaEsclusione(codiceEsclusione As String, mesi As Integer, anni As Integer, codiciVaccinazioni As String) As Integer Implements IMotiviEsclusioneProvider.InsertScadenzaEsclusione

            Dim cmd As OracleCommand = Nothing
            Dim ownConnection As Boolean = False

            Try
                cmd = Me.Connection.CreateCommand()

                cmd.CommandText = OnVac.Queries.MotiviEsclusione.OracleQueries.insScadenzaMotiviEsclusione

                cmd.Parameters.AddWithValue("mos_moe_codice", GetStringParam(codiceEsclusione, False))
                cmd.Parameters.AddWithValue("mos_mesi", GetStringParam(mesi, False))
                cmd.Parameters.AddWithValue("mos_anni", GetStringParam(anni, False))
                cmd.Parameters.AddWithValue("mos_lst_vac_codice", GetStringParam(codiciVaccinazioni, False))

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Return cmd.ExecuteNonQuery()

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

        End Function

        Public Function UpdateScadenzaEsclusione(id As Integer, mesi As Integer, anni As Integer, codiciVaccinazioni As String) As Integer Implements IMotiviEsclusioneProvider.UpdateScadenzaEsclusione

            Dim cmd As OracleCommand = Nothing
            Dim ownConnection As Boolean = False

            Try

                cmd = Me.Connection.CreateCommand()

                cmd.CommandText = OnVac.Queries.MotiviEsclusione.OracleQueries.updScadenzaMotiviEsclusione

                cmd.Parameters.AddWithValue("mos_id", GetStringParam(id, False))
                cmd.Parameters.AddWithValue("mos_mesi", GetStringParam(mesi, False))
                cmd.Parameters.AddWithValue("mos_anni", GetStringParam(anni, False))
                cmd.Parameters.AddWithValue("mos_lst_vac_codice", GetStringParam(codiciVaccinazioni, False))

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Return cmd.ExecuteNonQuery()

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

        End Function

#End Region

#End Region

    End Class

End Namespace