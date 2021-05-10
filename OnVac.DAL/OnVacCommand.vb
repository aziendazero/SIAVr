Imports System.Collections.Generic
Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions
Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.DAL

Namespace DAL

    Public Enum DbProviders
        Oracle
        SqlClient
    End Enum

    Class OnVacProviders
        Public Shared ReadOnly Devard As String = "Devart.Data.Oracle"
        Public Shared ReadOnly SqlClient As String = "System.Data.SqlClient"
    End Class
    Class OnVacProvidersName
        Public Shared ReadOnly Devard As String = "oracle"
        Public Shared ReadOnly SqlClient As String = "System.Data.SqlClient"
    End Class


    Public Class OnVacCommand
        Implements IDbCommand

        Private internalCommand As IDbCommand
        Private dam As IDAM

        Private _provider As DbProviders

        Public ReadOnly Property ParameterEscape As String
            Get
                Select Case Me._provider
                    Case DbProviders.Oracle
                        Return ":"
                    Case DbProviders.SqlClient
                        Return "@"
                    Case Else
                        Throw New InvalidOperationException("Tipo di provider non riconoscuto")
                End Select
            End Get
        End Property

        Public Sub New(connection As IDbConnection, dam As IDAM)
            Me.New(connection.CreateCommand(), dam)
        End Sub
        Public Sub New(command As IDbCommand, dam As IDAM)
            Me.internalCommand = command
            Me.dam = dam
            Dim CONNECTION_STRING_PATTERN As String = "provider\s*=\s*(?<provider>[^;]+);"

            Dim reg As Regex = New Regex(CONNECTION_STRING_PATTERN, RegexOptions.IgnoreCase)
            Dim Match As Match = reg.Match(dam.ConnectionString)

            Dim _EFProvider As String = Match.Groups.Item("provider").Value
            If String.IsNullOrWhiteSpace(_EFProvider) Then
                _EFProvider = dam.Provider.ToLower()
            End If

            Select Case _EFProvider.ToLower()
                Case OnVacProviders.Devard.ToLower()
                    Me._provider = DbProviders.Oracle
                Case OnVacProvidersName.Devard
                    Me._provider = DbProviders.Oracle
                Case OnVacProviders.SqlClient.ToLower()
                    Me._provider = DbProviders.SqlClient
                Case Else
                    Throw New InvalidOperationException("Tipo di provider non riconoscuto")
            End Select
        End Sub

        Public Property Connection As IDbConnection Implements IDbCommand.Connection
            Get
                Return internalCommand.Connection
            End Get
            Set(value As IDbConnection)
                internalCommand.Connection = value
            End Set
        End Property

        Public Property Transaction As IDbTransaction Implements IDbCommand.Transaction
            Get
                Return internalCommand.Transaction
            End Get
            Set(value As IDbTransaction)
                internalCommand.Transaction = value
            End Set
        End Property

        Public Property CommandText As String Implements IDbCommand.CommandText
            Get
                Return internalCommand.CommandText
            End Get
            Set(value As String)
                If Not String.IsNullOrWhiteSpace(value) Then
                    'il secondo replace serve per adattare le query vecchie anche per sql nel caso siamo in oracle fara : --> :
                    internalCommand.CommandText = value.Trim().Replace("?", Me.ParameterEscape).Replace(":", Me.ParameterEscape)
                Else
                    internalCommand.CommandText = ""
                End If
            End Set
        End Property

        Public Property CommandTimeout As Integer Implements IDbCommand.CommandTimeout
            Get
                Return internalCommand.CommandTimeout
            End Get
            Set(value As Integer)
                internalCommand.CommandTimeout = value
            End Set
        End Property

        Public Property CommandType As CommandType Implements IDbCommand.CommandType
            Get
                Return internalCommand.CommandType
            End Get
            Set(value As CommandType)
                internalCommand.CommandType = value
            End Set
        End Property

        Public ReadOnly Property Parameters As IDataParameterCollection Implements IDbCommand.Parameters
            Get
                Return internalCommand.Parameters
            End Get
        End Property

        Public Property UpdatedRowSource As UpdateRowSource Implements IDbCommand.UpdatedRowSource
            Get
                Return internalCommand.UpdatedRowSource
            End Get
            Set(value As UpdateRowSource)
                internalCommand.UpdatedRowSource = value
            End Set
        End Property

        Public Sub Prepare() Implements IDbCommand.Prepare
            internalCommand.Prepare()
        End Sub

        Public Sub Cancel() Implements IDbCommand.Cancel
            internalCommand.Cancel()
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            internalCommand.Dispose()
        End Sub

        Public Function CreateParameter() As IDbDataParameter Implements IDbCommand.CreateParameter
            Return internalCommand.CreateParameter()
        End Function

        Public Function ExecuteNonQuery() As Integer Implements IDbCommand.ExecuteNonQuery
            Return internalCommand.ExecuteNonQuery()
        End Function

        Public Function ExecuteReader() As IDataReader Implements IDbCommand.ExecuteReader
            Return internalCommand.ExecuteReader()
        End Function

        Public Function ExecuteReader(behavior As CommandBehavior) As IDataReader Implements IDbCommand.ExecuteReader
            Return internalCommand.ExecuteReader(behavior)
        End Function

        Public Function ExecuteScalar() As Object Implements IDbCommand.ExecuteScalar
            Return internalCommand.ExecuteScalar()
        End Function

#Region "Utils"

        Public Function FirstOrDefault(Of T)() As T
            Return Me.internalCommand.FirstOrDefault(Of T)()
        End Function

        Public Function First(Of T)() As T
            Return Me.internalCommand.First(Of T)()
        End Function

        Public Function AddParameter(ByVal name As String, ByVal value As Object) As IDbDataParameter
            Return Me.internalCommand.AddParameter(name, value)
        End Function

        Public Function AddReturnParameter(Of T)(ByVal name As String) As IDbDataParameter
            Return Me.internalCommand.AddReturnParameter(Of T)(name)
        End Function

        Private Function GetParameterType(tipo As Type) As DbType
            If tipo Is Nothing OrElse tipo = GetType(String) Then
                Return DbType.String
            ElseIf tipo Is GetType(Integer) Then
                Return DbType.Int32
            ElseIf tipo Is GetType(Long) Then
                Return DbType.Int64
            ElseIf tipo Is GetType(Decimal) Then
                Return DbType.Decimal
            ElseIf tipo Is GetType(Single) Then
                Return DbType.Single
            ElseIf tipo Is GetType(Double) Then
                Return DbType.Double
            ElseIf tipo Is GetType(Date) Then
                Return DbType.Date
            End If

            If tipo.IsGenericType AndAlso tipo.GetGenericTypeDefinition() Is GetType(Nullable(Of)) Then
                Return GetParameterType(tipo.GetGenericArguments().First())
            End If

            Return DbType.String

        End Function
        Public Function SkipTakeQuery(query As String, skip As Integer, take As Integer?) As String
            Me.CommandText = query
            Return Me.internalCommand.SkipTakeQuery(Me.CommandText, skip, take)
        End Function
        Public Function Fill(Of T)() As List(Of T)
            Return Me.Fill(Of T)(Nothing, Nothing)
        End Function
        Public Function Fill(Of T)(skip As Integer?) As List(Of T)
            Return Me.Fill(Of T)(skip, Nothing)
        End Function
        Public Function Fill(Of T)(skip As Integer?, take As Integer?) As List(Of T)
            Return Me.internalCommand.Fill(Of T)(skip, take)
        End Function

        Public Function SetParameterIn(Of T)(desinenza As String, elementi As IEnumerable(Of T)) As String
            Return Me.internalCommand.SetParameterIn(desinenza, elementi, Me.ParameterEscape)
        End Function

        Public Sub InsertInTable(Of TTAble As Class)(ByVal tableName As String, ByVal valore As TTAble)
            TmpParameter(Sub()
                             Dim query As New StringBuilder("Insert into ")
                             query.AppendFormat("{0}(", tableName)

                             Dim colonne As New List(Of String)
                             Dim parametri As New List(Of String)

                             For Each prop As PropertyInfo In valore.GetType().GetProperties()
                                 Dim columnName As String
                                 Dim attr As DbColumnName = prop.GetCustomAttributes(GetType(DbColumnName), True).FirstOrDefault()
                                 If attr Is Nothing Then
                                     columnName = prop.Name
                                 Else
                                     columnName = attr.LastName
                                 End If

                                 colonne.Add(columnName)
                                 parametri.Add(String.Format("?{0}", prop.Name))


                                 Dim columnValue As Object = prop.GetValue(valore, Nothing)
                                 Me.AddParameter(prop.Name, columnValue)
                             Next

                             query.AppendFormat("{0}) values ({1})", String.Join(",", colonne), String.Join(",", parametri))

                             Me.CommandText = query.ToString()
                             Me.ExecuteNonQuery()

                         End Sub)
        End Sub
        Public Function InsertInTable(Of TOut)(ByVal tableName As String, ByVal valore As Object, returnColumn As String) As TOut
            Return TmpParameter(Function()
                                    Dim query As New StringBuilder("Insert into ")
                                    query.AppendFormat("{0}(", tableName)

                                    Dim colonne As New List(Of String)
                                    Dim parametri As New List(Of String)

                                    For Each prop As PropertyInfo In valore.GetType().GetProperties()
                                        Dim columnName As String
                                        Dim attr As DbColumnName = prop.GetCustomAttributes(GetType(DbColumnName), True).FirstOrDefault()
                                        If attr Is Nothing Then
                                            columnName = prop.Name
                                        Else
                                            columnName = attr.LastName
                                        End If

                                        colonne.Add(columnName)
                                        parametri.Add(String.Format("?{0}", prop.Name))


                                        Dim columnValue As Object = prop.GetValue(valore, Nothing)
                                        Me.AddParameter(prop.Name, columnValue)
                                    Next

                                    query.AppendFormat("{0}) values ({1})", String.Join(",", colonne), String.Join(",", parametri))

                                    If Me._provider = DbProviders.Oracle Then
                                        query.AppendFormat(" RETURNING {0} into ?RETURN_ID", returnColumn)
                                        Dim returnPar As IDbDataParameter = Me.AddReturnParameter(Of TOut)("RETURN_ID")
                                        Me.CommandText = query.ToString()
                                        Me.ExecuteNonQuery()
                                        Return returnPar.ParseScalar(Of TOut)
                                    End If

                                End Function)
        End Function
        Public Sub UpdateTable(Of TModifica As Class, TWhere As Class)(tableName As String, modify As TModifica, condition As TWhere)
            TmpParameter(Sub()
                             Dim query As New StringBuilder()
                             query.AppendFormat("update {0} set ", tableName)

                             Dim modifiche As New List(Of String)

                             For Each prop As PropertyInfo In modify.GetType().GetProperties()
                                 Dim columnName As String
                                 Dim attr As DbColumnName = prop.GetCustomAttributes(GetType(DbColumnName), True).FirstOrDefault()
                                 If attr Is Nothing Then
                                     columnName = prop.Name
                                 Else
                                     columnName = attr.LastName
                                 End If

                                 Dim parName As String = prop.Name

                                 modifiche.Add(String.Format("{0} = ?{1}", columnName, parName))
                                 Me.AddParameter(parName, prop.GetValue(modify, Nothing))
                             Next

                             query.Append(String.Join(", ", modifiche))

                             If Not IsNothing(condition) Then
                                 query.Append(" where ")
                                 Dim condizioni As New List(Of String)

                                 For Each prop As PropertyInfo In condition.GetType().GetProperties()
                                     Dim columnName As String
                                     Dim attr As DbColumnName = prop.GetCustomAttributes(GetType(DbColumnName), True).FirstOrDefault()
                                     If attr Is Nothing Then
                                         columnName = prop.Name
                                     Else
                                         columnName = attr.LastName
                                     End If

                                     Dim par As String = prop.Name
                                     condizioni.Add(String.Format("{0} = ?{1}", columnName, par))
                                     Me.AddParameter(par, prop.GetValue(condition, Nothing))

                                 Next
                                 query.Append(String.Join(" AND ", condizioni))

                             End If
                             Me.CommandText = query.ToString()
                             Me.ExecuteNonQuery()

                         End Sub)
        End Sub

        Private Sub TmpParameter(func As Action)
            TmpParameter(Of Boolean)(Function()
                                         func()
                                         Return True
                                     End Function)
        End Sub
        Private Function TmpParameter(Of T)(func As Func(Of T)) As T
            Dim pars As New List(Of Object)
            For Each p As Object In Me.Parameters
                pars.Add(p)
            Next
            Me.Parameters.Clear()
            Dim ritorno As T = func()
            Me.Parameters.Clear()
            For Each p As Object In pars
                Me.Parameters.Add(p)
            Next
            Return ritorno
        End Function
#End Region
    End Class

End Namespace