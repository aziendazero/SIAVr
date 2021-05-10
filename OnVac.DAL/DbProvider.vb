Imports Onit.Database.DataAccessManager
Imports System.Collections.Generic


Namespace DAL

    Public Class DbProvider

#Region " Properties "

        Private _cmd As IDbCommand

        Private dt As DataTable
        Protected Property _DT() As DataTable
            Get
                Return dt
            End Get
            Set(ByVal Value As DataTable)
                dt = Value
            End Set
        End Property

        Private dam As IDAM
        Protected ReadOnly Property _DAM() As IDAM
            Get
                Return dam
            End Get
        End Property

        Private ret As Object
        Protected Property _RET() As Object
            Get
                Return ret
            End Get
            Set(Value As Object)
                ret = Value
            End Set
        End Property

        Private _err_msg As String
        Protected Property ERROR_MSG() As String
            Get
                Return _err_msg
            End Get
            Set(Value As String)
                _err_msg = Value
            End Set
        End Property

        Public ReadOnly Property Provider() As String
            Get
                Return Me.dam.Provider
            End Get
        End Property

        Public ReadOnly Property Connection() As System.Data.IDbConnection
            Get
                Return Me.dam.Connection
            End Get
        End Property

        Public ReadOnly Property Transaction() As System.Data.IDbTransaction
            Get
                Return Me.dam.Transaction
            End Get
        End Property

#End Region

#Region " Constructors "

        Public Sub New(ByRef mad As IDAM)

            Me.dam = mad

            Me._DT = New DataTable()

        End Sub

        'Protected Friend Sub New(ByVal provider As String, ByRef conn As IDbConnection, ByRef tx As IDbTransaction)
        '    _provider = provider
        '    _conn = conn
        '    _tx = tx

        '    dam = DAMBuilder.CreateDAM(_conn, _tx)     ' ??? Se si potesse, sarebbe da eliminare

        '    _DT = New DataTable                         ' ??? Evitare di usarlo, se possibile
        'End Sub

        'Protected Friend Sub New(ByVal provider As String, ByRef conn As IDbConnection, ByRef tx As IDbTransaction, ByRef mad As IDAM)
        '    dam = mad

        '    _provider = provider
        '    _conn = conn
        '    _tx = tx

        '    _DT = New DataTable
        'End Sub

#End Region

#Region " Gestione errori e log "

        ''' <summary>
        ''' Registra l'evento eccezione in un file di testo
        ''' </summary>
        ''' <param name="ex">Eccezione da loggare</param>
        ''' <remarks> Il file di log è impostato
        ''' </remarks>
        Protected Sub LogError(ex As Exception)

            Dim testataLog As New Testata(DataLogStructure.TipiArgomento.DATAACCESSLAYER, Operazione.Generico)
            Dim recordLog As New Record()

            testataLog.Intestazione = True
            testataLog.Stack = ex.StackTrace

            recordLog.Campi.Add(New Campo("Message", ex.Message))

            testataLog.Records.Add(recordLog)

            LogBox.WriteData(testataLog)

        End Sub

        ''' <summary>
        ''' Registra l'evento eccezione in un file di testo, assieme ad una descrizione
        ''' </summary>
        ''' <param name="ex">Eccezione da loggare</param>
        ''' <param name="errorMessage">Descrizione da loggare</param>
        ''' <remarks> Il file di log è impostato 
        ''' </remarks>
        Protected Sub LogError(ex As Exception, errorMessage As String)

            Dim testataLog As New Testata(DataLogStructure.TipiArgomento.DATAACCESSLAYER, Operazione.Generico)
            Dim recordLog As New Record()

            testataLog.Intestazione = True
            testataLog.Stack = ex.StackTrace

            recordLog.Campi.Add(New Campo("Message", String.Format("{0} - {1}", errorMessage, ex.Message)))

            testataLog.Records.Add(recordLog)

            LogBox.WriteData(testataLog)

        End Sub

        ''' <summary>
        ''' Registra il messaggio di errore da restituire all'utente
        ''' </summary>
        ''' <param name="errorMessage">Messaggio di errore</param>
        ''' <remarks> Il file di log è impostato 
        ''' </remarks>
        Protected Sub SetErrorMsg(errorMessage As String)

            Me.ERROR_MSG = errorMessage

        End Sub

#End Region

#Region " Gestione Connection e Transaction "

        ' Restituisce true se apre la connection relativa al command passato per parametro
        Protected Friend Function ConditionalOpenConnection(cmd As IDbCommand) As Boolean

            Dim ownConnectionOpened As Boolean = False

            If Not Me.Transaction Is Nothing Then

                cmd.Transaction = Me.Transaction

            Else

                If Me.Connection.State = ConnectionState.Closed Then

                    Me.Connection.Open()
                    ownConnectionOpened = True

                End If

            End If

            Return ownConnectionOpened

        End Function

        Protected Friend Sub ConditionalCloseConnection(ownConnection As Boolean)

            If ownConnection AndAlso Not Me.Connection Is Nothing AndAlso Me.Connection.State = ConnectionState.Open Then

                Me.Connection.Close()

            End If

        End Sub

        Private Function DoTransaction(Of T, TCommand As IDbCommand)(cmd As TCommand, func As Func(Of TCommand, T)) As T
            Return DoTransaction(cmd, IsolationLevel.ReadCommitted, func)
        End Function

        Private Function DoTransaction(Of T, TCommand As IDbCommand)(cmd As TCommand, isolationLevel As IsolationLevel, func As Func(Of TCommand, T)) As T
            Using tr As IDbTransaction = cmd.Connection.BeginTransaction(isolationLevel)
                Try
                    cmd.Transaction = tr
                    Dim result As T = func(cmd)
                    tr.Commit()
                    Return result
                Catch ex As Exception
                    tr.Rollback()
                    Throw ex
                End Try
            End Using
        End Function
        Private Sub DoTransaction(Of TCommand As IDbCommand)(cmd As TCommand, func As Action(Of TCommand))
            DoTransaction(cmd, IsolationLevel.ReadCommitted, func)
        End Sub
        Private Sub DoTransaction(Of TCommand As IDbCommand)(cmd As TCommand, isolationLevel As IsolationLevel, func As Action(Of TCommand))
            If cmd.Transaction Is Nothing Then
                Using tr As IDbTransaction = cmd.Connection.BeginTransaction(isolationLevel)
                    Try
                        cmd.Transaction = tr
                        func(cmd)
                        tr.Commit()
                    Catch ex As Exception
                        tr.Rollback()
                        Throw ex
                    End Try
                End Using
            Else
                func(cmd)
            End If
        End Sub

        Protected Friend Function DoCommand(Of T)(func As Func(Of OnVacCommand, T)) As T
            If Connection.State <> ConnectionState.Open Then
                Try
                    Connection.Open()
                    Me._cmd = New OnVacCommand(Connection, Me.dam)
                    Using _cmd
                        Return func(_cmd)
                    End Using
                Finally
                    Connection.Close()
                End Try
            Else
                If _cmd Is Nothing Then
                    _cmd = New OnVacCommand(Connection, Me.dam)
                    Using _cmd
                        Return func(_cmd)
                    End Using
                End If

                Dim parametri As New List(Of Object)
                If _cmd.Parameters IsNot Nothing Then
                    For Each pa As Object In _cmd.Parameters
                        parametri.Add(pa)
                    Next
                End If
                Me._cmd.Parameters.Clear()
                Dim returnValue As T = func(Me._cmd)
                Me._cmd.Parameters.Clear()
                For Each pa As Object In parametri
                    Me._cmd.Parameters.Add(pa)
                Next
                Return returnValue
            End If
        End Function

        Protected Friend Sub DoCommand(func As Action(Of OnVacCommand))
            DoCommand(Function(cmd)
                          func(cmd)
                          Return True
                      End Function)
        End Sub

        Protected Friend Function DoCommand(Of T)(func As Func(Of OnVacCommand, T), isolationLevel As IsolationLevel) As T
            Return DoCommand(Function(cmd)
                                 Return DoTransaction(cmd, isolationLevel, func)
                             End Function)
        End Function

        Protected Friend Sub DoCommand(func As Action(Of OnVacCommand), isolationLevel As IsolationLevel)
            DoCommand(Sub(cmd)
                          DoTransaction(cmd, isolationLevel, func)
                      End Sub)
        End Sub

#End Region

#Region " Protected "

        Protected Function GetDimensioneColonnaDaCatalogo(nomeTabella As String, nomeColonna As String) As Integer

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("data_length")
                .AddTables("user_tab_columns")
                .AddWhereCondition("table_name", Comparatori.Uguale, nomeTabella, DataTypes.Stringa)
                .AddWhereCondition("column_name", Comparatori.Uguale, nomeColonna, DataTypes.Stringa)
            End With

            Return _DAM.ExecScalar()

        End Function

#Region " Passaggio parametri alla query "

        ''' <summary>
        ''' Restituisce dbnull se la stringa passata per parametro è vuota. Altrimenti restituisce la stringa stessa.
        ''' </summary>
        ''' <param name="stringValue"></param>
        ''' <param name="toUppercase"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Shared Function GetStringParam(stringValue As String, toUppercase As Boolean) As Object

            If String.IsNullOrEmpty(stringValue) Then Return DBNull.Value

            If toUppercase Then Return stringValue.ToUpper()

            Return stringValue

        End Function

        Protected Friend Shared Function GetStringParam(stringValue As String) As Object

            Return GetStringParam(stringValue, False)

        End Function

        ''' <summary>
        ''' Restituisce dbnull se la data passata per parametro è minvalue. Altrimenti restituisce la data stessa.
        ''' </summary>
        ''' <param name="dateValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Shared Function GetDateParam(dateValue As DateTime) As Object

            If dateValue = Date.MinValue Then Return DBNull.Value

            Return dateValue

        End Function

        Protected Friend Shared Function GetDateParam(dateValue As DateTime?) As Object

            If Not dateValue.HasValue Then Return DBNull.Value

            Return GetDateParam(dateValue.Value)

        End Function

        ''' <summary>
        ''' Restituisce dbnull se il numero passato per parametro è -1. Altrimenti restituisce il numero stesso.
        ''' </summary>
        ''' <param name="intValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Shared Function GetIntParam(intValue As Integer) As Object

            If intValue = -1 Then Return DBNull.Value

            Return intValue

        End Function

        ''' <summary>
        ''' Restituisce dbnull se il parametro è null. Altrimenti restituisce il numero stesso.
        ''' </summary>
        ''' <param name="intValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Shared Function GetIntParam(intValue As Integer?) As Object

            If Not intValue.HasValue Then Return DBNull.Value

            Return GetIntParam(intValue.Value)

        End Function

        ''' <summary>
        ''' Restituisce dbnull se il parametro è null. Altrimenti restituisce il numero stesso.
        ''' </summary>
        ''' <param name="longValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Shared Function GetLongParam(longValue As Long) As Object

            If longValue = -1 Then Return DBNull.Value

            Return longValue

        End Function

        ''' <summary>
        ''' Restituisce dbnull se il parametro è null. Altrimenti restituisce il numero stesso.
        ''' </summary>
        ''' <param name="longValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Shared Function GetLongParam(longValue As Long?) As Object

            If Not longValue.HasValue Then Return DBNull.Value

            Return GetLongParam(longValue.Value)

        End Function

        ''' <summary>
        ''' Restituisce dbnull se il parametro è null. Altrimenti restituisce il numero stesso.
        ''' </summary>
        ''' <param name="doubleValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Shared Function GetDoubleParam(doubleValue As Double?) As Object

            If Not doubleValue.HasValue Then Return DBNull.Value

            Return doubleValue.Value

        End Function

        Protected Friend Function GetEnumParam(Of TEnum As Structure)(value As Object, format As String, nullIfUndefined As Boolean) As Object

            If value Is Nothing Then Return DBNull.Value

            If System.Enum.IsDefined(GetType(TEnum), value) Then
                Return Convert.ToInt32(value)
            End If

            If nullIfUndefined Then Return DBNull.Value

            Return value

        End Function

        ''' <summary>
        ''' Restituisce dbnull se il parametro è null. Altrimenti restituisce il numero stesso.
        ''' </summary>
        ''' <param name="decimalValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Shared Function GetDecimalParam(decimalValue As Decimal?) As Object

            If Not decimalValue.HasValue Then Return DBNull.Value

            Return decimalValue.Value

        End Function
        Protected Friend Shared Function GetGuidParam(guidValue As Guid) As Object
            If guidValue = Guid.Empty Then
                Return DBNull.Value
            End If
            Return guidValue.ToByteArray()
        End Function


#End Region

#Region " Metodo per clausola e parametri di IN "

        Protected Class GetInFilterResult
            Public InFilter As String
            Public Parameters() As OracleClient.OracleParameter
        End Class

        ''' <summary>
        ''' Restituisce una struttura contenente la stringa da utilizzare come filtro per la query di IN e l'elenco di parametri da aggiungere all'OracleCommand
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="values"></param>
        ''' <returns></returns>
        Protected Function GetInFilter(Of T)(values As List(Of T)) As GetInFilterResult

            Dim filtroIn As New Text.StringBuilder()
            Dim parameters As New List(Of OracleClient.OracleParameter)()

            Dim paramName As String = String.Empty

            For i As Integer = 0 To values.Count - 1

                paramName = String.Format(":p{0}", i.ToString())

                filtroIn.AppendFormat("{0},", paramName)

                parameters.Add(New OracleClient.OracleParameter(paramName, values(i)))

            Next

            If filtroIn.Length > 0 Then filtroIn.Remove(filtroIn.Length - 1, 1)

            Return New GetInFilterResult() With {
                .InFilter = filtroIn.ToString(),
                .Parameters = parameters.ToArray()
            }

        End Function

#End Region

        ''' <summary>
        ''' Resetta il datatable della classe base. Elimina tutto, primary key e constrain comprese
        ''' </summary>
        Protected Sub RefurbishDT()

            If Not Me._DT Is Nothing Then

                Me._DT.PrimaryKey = Nothing
                Me._DT.ParentRelations.Clear()
                Me._DT.ChildRelations.Clear()
                Me._DT.Constraints.Clear()
                Me._DT.Rows.Clear()
                Me._DT.Columns.Clear()
                Me._DT.Clear()

            End If

        End Sub

#End Region
#Region "Util"
        ' Converte i flag (S/N) in boolean
        Protected Function SNtoBool(value As String) As Boolean
            If Not String.IsNullOrWhiteSpace(value) Then
                Return String.Equals(value, "S", StringComparison.InvariantCultureIgnoreCase)
            End If
            Return False
        End Function

        Protected Function BoolToSN(value As Boolean) As String
            If (value) Then
                Return "S"
            End If
            Return "N"
        End Function
#End Region
    End Class

End Namespace

