Imports System.Collections.Generic
Imports System.Data.OracleClient
Imports Onit.Database.DataAccessManager


Namespace DAL.Oracle

    Public Class DbStatiAnagraficiProvider
        Inherits DbProvider
        Implements IStatiAnagraficiProvider

#Region " Constructors "

        Public Sub New(ByRef DAM As IDAM)
            MyBase.New(DAM)
        End Sub

#End Region

#Region " Public "

        ''' <summary>
        ''' Restituisce tutti gli stati anagrafici presenti in tabella
        ''' </summary>
        ''' <remarks>In caso di errore scrive sul log
        ''' </remarks>
        Public Function GetStatiAnagrafici() As DataTable Implements IStatiAnagraficiProvider.GetStatiAnagrafici

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("*")
                .AddTables("t_ana_stati_anagrafici")
                .AddOrderByFields("to_number(san_codice)")
            End With

            Try

                _DAM.BuildDataTable(_DT)

            Catch ex As Exception

                Me.LogError(ex)
                Me.SetErrorMsg("Errore durante la lettura degli stati anagrafici dal database")

                _DT = Nothing

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return _DT

        End Function

        ' Restituisce lo stato anagrafico di default in caso di anagrafe locale (quello che ha il flag a S)
        Public Function GetStatoAnagraficoDefault() As String Implements IStatiAnagraficiProvider.GetStatoAnagraficoDefault

            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim statoAnag As String = String.Empty

            Dim ownConnection As Boolean = False

            Try
                cmd = Me.Connection.CreateCommand()

                cmd.CommandText = OnVac.Queries.StatiAnagrafici.OracleQueries.selStatoAnagDefault

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Dim obj As Object = cmd.ExecuteScalar()

                If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then
                    statoAnag = obj.ToString()
                Else
                    statoAnag = String.Empty
                End If

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return statoAnag

        End Function

        ' Restituisce lo stato anagrafico di default in caso di anagrafe locale (quello che ha il flag a S)
        Public Function GetStatoAnagCodificatoDefault() As Enumerators.StatoAnagrafico Implements IStatiAnagraficiProvider.GetStatoAnagCodificatoDefault

            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim statoAnag As String = String.Empty

            Dim ownConnection As Boolean = False

            Try
                cmd = Me.Connection.CreateCommand()

                cmd.CommandText = OnVac.Queries.StatiAnagrafici.OracleQueries.selStatoAnagDefault

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Dim obj As Object = cmd.ExecuteScalar()
                If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then
                    statoAnag = obj.ToString
                Else
                    statoAnag = String.Empty
                End If

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            If statoAnag = String.Empty Then Return Nothing

            Return System.Enum.Parse(GetType(Enumerators.StatoAnagrafico), statoAnag)

        End Function

        ' Restituisce una lista di codici degli stati anagrafici per cui cancellare la programmazione vaccinale
        Public Function GetStatiAnagraficiCancellazioneProgrammazione() As List(Of String) Implements IStatiAnagraficiProvider.GetStatiAnagraficiCancellazioneProgrammazione

            Dim listCodici As List(Of String) = Nothing

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("san_codice")
                .AddTables("t_ana_stati_anagrafici")
                .AddWhereCondition("san_chiamata", Comparatori.Uguale, "N", DataTypes.Stringa)
                .AddOrderByFields("san_codice")
            End With

            Using idr As IDataReader = _DAM.BuildDataReader()

                If Not idr Is Nothing Then

                    listCodici = New List(Of String)

                    Dim san_codice As Integer = idr.GetOrdinal("san_codice")

                    While idr.Read()

                        listCodici.Add(idr.GetStringOrDefault(san_codice))

                    End While

                End If

            End Using

            Return listCodici

        End Function

        Public Function IsStatoAnagraficoAttivo(codiceStatoAnagrafico As String) As Boolean Implements IStatiAnagraficiProvider.IsStatoAnagraficoAttivo

            Dim isAttivo As Boolean = False

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("san_chiamata")
                .AddTables("t_ana_stati_anagrafici")
                .AddWhereCondition("san_codice", Comparatori.Uguale, codiceStatoAnagrafico, DataTypes.Stringa)
            End With

            Dim san_chiamata As Object = _DAM.ExecScalar

            If Not san_chiamata Is Nothing Then
                isAttivo = (san_chiamata.ToString() = "S")
            End If

            Return isAttivo

        End Function

        Public Function GetStatiAnagraficiAttivi() As IEnumerable(Of Enumerators.StatoAnagrafico) Implements IStatiAnagraficiProvider.GetStatiAnagraficiAttivi

            Dim list As New List(Of Enumerators.StatoAnagrafico)()

            Using cmd As New OracleClient.OracleCommand("select san_codice from t_ana_stati_anagrafici where san_chiamata = 'S'", Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim san_codice As Integer = idr.GetOrdinal("san_codice")

                            While idr.Read()

                                If Not idr.IsDBNull(san_codice) Then
                                    list.Add(idr.GetNullableEnumOrDefault(Of Enumerators.StatoAnagrafico)(san_codice).Value)
                                End If

                            End While

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list.AsEnumerable()

        End Function

        Public Function GetErrorMsg() As String Implements IStatiAnagraficiProvider.GetErrorMsg

            Return ERROR_MSG

        End Function

        ''' <summary>
        ''' Restituisce lo stato anagrafico a partire dalle categiorie cittadino dell'AURV tramite la tabella di mapping
        ''' </summary>
        ''' <param name="categoriaCittadino"></param>
        ''' <returns></returns>
        Public Function GetStatoAnagraficoFromCategoriaCittadino(categoriaCittadino As String) As String Implements IStatiAnagraficiProvider.GetStatoAnagraficoFromCategoriaCittadino

            Dim codiceStatoAnag As String = String.Empty

            Dim ownConnection As Boolean = False

            Try
                Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                    cmd.CommandText = OnVac.Queries.StatiAnagrafici.OracleQueries.selStatoAnagFromCategoriaCittadino
                    cmd.Parameters.AddWithValue(":sac_categoria_cittadino", categoriaCittadino)

                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        codiceStatoAnag = obj.ToString()
                    End If

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return codiceStatoAnag

        End Function

        Public Function GetStati(acronimo As String) As List(Of Stato) Implements IStatiAnagraficiProvider.GetStati
            Dim result As New List(Of Stato)
            Dim ownConnection As Boolean = False

            Dim query As String = "SELECT ST_ID, ST_SIGLA_TIPO, ST_STATO, ST_DES_SIGLA, ST_DESCRIZIONE " +
                                    "FROM T_STATI " +
                                    "WHERE ST_SIGLA_TIPO = :ST_SIGLA_TIPO "
            Try
                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    cmd.Parameters.AddWithValue("ST_SIGLA_TIPO", acronimo)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()
                        Dim ST_ID As Integer = _context.GetOrdinal("ST_ID")
                        Dim ST_SIGLA_TIPO As Integer = _context.GetOrdinal("ST_SIGLA_TIPO")
                        Dim ST_STATO As Integer = _context.GetOrdinal("ST_STATO")
                        Dim ST_DES_SIGLA As Integer = _context.GetOrdinal("ST_DES_SIGLA")
                        Dim ST_DESCRIZIONE As Integer = _context.GetOrdinal("ST_DESCRIZIONE")

                        While _context.Read()
                            Dim stato As New Stato()
                            stato.Id = _context.GetInt32OrDefault(ST_ID)
                            stato.SiglaTipo = _context.GetStringOrDefault(ST_SIGLA_TIPO)
                            stato.Stato = _context.GetInt32OrDefault(ST_STATO)
                            stato.DesSigla = _context.GetStringOrDefault(ST_DES_SIGLA)
                            stato.Descrizione = _context.GetStringOrDefault(ST_DESCRIZIONE)

                            result.Add(stato)
                        End While

                    End Using
                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return result
        End Function
#End Region

    End Class

End Namespace
