Imports System.Collections.Generic
Imports System.Reflection
Imports System.Text

Namespace DAL
    Public Module SqlUtil

        <System.Runtime.CompilerServices.Extension()>
        Public Function SkipTakeQuery(ByRef command As IDbCommand, query As String, skip As Integer, take As Integer?) As String
            Dim q As String
            If take.HasValue AndAlso take > 0 Then
                Dim s As Integer
                If skip > 0 Then
                    s = skip
                Else
                    s = 0
                End If
                q = String.Format("select * from ( select ROWNUM as pos, tab.* from ( {0} ) tab ) tab2 where pos > :skip and pos < :take", query)
                command.AddParameter("skip", s)
                command.AddParameter("take", s + take.Value + 1)
            Else
                q = query
            End If
            command.CommandText = q
            Return q
        End Function


        <System.Runtime.CompilerServices.Extension()>
        Public Function SetParameterIn(Of T)(ByRef command As IDbCommand, desinenza As String, elementi As IEnumerable(Of T), parameterEscape As String) As String
            Dim ritorno As New StringBuilder()
            Dim elenco As List(Of T) = elementi.Distinct().ToList()
            Dim nome As String
            For i As Integer = 0 To elenco.Count - 2
                nome = String.Format("{0}_{1}", desinenza, i)
                command.AddParameter(nome, elenco.Item(i))
                ritorno.Append(String.Format("{1}{0}, ", nome, parameterEscape.Trim()))
            Next
            nome = String.Format("{0}_{1}", desinenza, elenco.Count)
            command.AddParameter(nome, elenco.Last())
            ritorno.Append(String.Format("{1}{0}", nome, parameterEscape.Trim()))
            Return ritorno.ToString()
        End Function

        ''' <summary>
        ''' Legge il risultato della query e lo riporta all'interno della classe fornendo una lista della classe
        ''' I risultati vengono riempiti tramite il nome della property o il nome assegnato dall'attributo DbColumnName
        ''' 
        ''' Il tipo di dato può essere uno scalare (Int, Long, String....) in quel caso lo filla con la prima colonna
        ''' 
        ''' In caso il tipo T sia una classe occorre che abbia il costruttore vuoto
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="command"></param>
        ''' <returns></returns>
        <System.Runtime.CompilerServices.Extension()>
        Public Function Fill(Of T)(ByRef command As IDbCommand) As List(Of T)
            Return command.Fill(Of T)(Nothing, Nothing)
        End Function

        <System.Runtime.CompilerServices.Extension()>
        Public Function Fill(Of T)(ByRef command As IDbCommand, skip As Integer?) As List(Of T)
            Return command.Fill(Of T)(skip, Nothing)
        End Function

        <System.Runtime.CompilerServices.Extension()>
        Public Function Fill(Of T)(ByRef command As IDbCommand, skip As Integer?, take As Integer?) As List(Of T)
            Dim toSkip As Boolean = skip.HasValue AndAlso skip.Value > 0
            Dim toTake As Boolean = take.HasValue AndAlso take.Value > 0
            Dim skipped As Integer = 0

            Dim tipo As Type = GetType(T)
            Dim tipoNullabile As Type = Nullable.GetUnderlyingType(tipo)

            Dim result As New List(Of T)

            Using reader As IDataReader = command.ExecuteReader()
                If tipo.IsPrimitive OrElse tipo = GetType(String) OrElse (Not IsNothing(tipoNullabile) AndAlso tipoNullabile.IsPrimitive) OrElse (tipo = GetType(Date) OrElse tipoNullabile = GetType(Date)) Then
                    While reader.Read()
                        If toSkip AndAlso skipped < skip.Value Then
                            skipped += 1
                            Continue While
                        End If

                        result.Add(ReadScalar(reader, 0, tipo))

                        If toTake AndAlso result.Count = take.Value Then
                            Return result
                        End If
                    End While
                Else
                    Dim bindings As List(Of KeyValuePair(Of PropertyInfo, Integer)) = BindOrdinals(tipo, reader)

                    While reader.Read()
                        If toSkip AndAlso skipped < skip.Value Then
                            skipped += 1
                            Continue While
                        End If

                        result.Add(ReadObject(Of T)(reader, bindings))

                        If toTake AndAlso result.Count = take.Value Then
                            Return result
                        End If
                    End While
                End If
            End Using
            Return result
        End Function

        <System.Runtime.CompilerServices.Extension()>
        Public Function Singolo(Of T)(ByRef command As IDbCommand) As T
            Return command.Fill(Of T)(0, 2).Single()
        End Function

        <System.Runtime.CompilerServices.Extension()>
        Public Function SingoloOrDefault(Of T)(ByRef command As IDbCommand) As T
            Return command.Fill(Of T)(0, 2).SingleOrDefault()
        End Function

        <System.Runtime.CompilerServices.Extension()>
        Public Function First(Of T)(ByRef command As IDbCommand) As T
            Return command.Fill(Of T)(0, 2).First()
        End Function

        <System.Runtime.CompilerServices.Extension()>
        Public Function FirstOrDefault(Of T)(ByRef command As IDbCommand) As T
            Return command.Fill(Of T)(0, 1).FirstOrDefault()
        End Function

        Private Function BindOrdinals(obj As Type, reader As IDataReader) As List(Of KeyValuePair(Of PropertyInfo, Integer))
            Dim dbNames As New Dictionary(Of String, Integer)
            For i As Integer = 0 To reader.FieldCount - 1
                dbNames.Add(reader.GetName(i).ToLower(), i)
            Next

            Dim result As New List(Of KeyValuePair(Of PropertyInfo, Integer))

            For Each prop As PropertyInfo In obj.GetProperties()
                Dim attr As IEnumerable(Of DbColumnName) = prop.GetCustomAttributes(GetType(DbColumnName), True)

                Dim nomi As New List(Of String)
                nomi.AddRange(attr.SelectMany(Function(x)
                                                  Return x.ColumnName
                                              End Function))
                nomi.Add(prop.Name)

                For Each n As String In nomi
                    If dbNames.ContainsKey(n.ToLower()) Then
                        result.Add(New KeyValuePair(Of PropertyInfo, Integer)(prop, dbNames.Item(n.ToLower())))
                        Exit For
                    End If
                Next
            Next

            Return result
        End Function

        Private Function ReadObject(Of T)(reader As IDataReader, bindings As List(Of KeyValuePair(Of PropertyInfo, Integer))) As T
            Dim result As T = Activator.CreateInstance(Of T)

            For Each bind As KeyValuePair(Of PropertyInfo, Integer) In bindings
                bind.Key.SetValue(result, ReadScalar(reader, bind.Value, bind.Key.PropertyType), Nothing)
            Next

            Return result
        End Function

        Private Function ReadScalar(reader As IDataReader, ordinal As Integer, type As Type) As Object
            Dim interno As Type = Nullable.GetUnderlyingType(type)
            If interno IsNot Nothing Then
                If reader.IsDBNull(ordinal) Then
                    Return Nothing
                Else
                    Return ReadScalar(reader, ordinal, interno)
                End If
            End If

            If type.IsEnum Then
                Dim valoreDb As Integer = reader.GetInt32OrDefault(ordinal)
                Return CTypeDynamic(valoreDb, type)
            ElseIf type Is GetType(String) Then
                Return reader.GetStringOrDefault(ordinal)
            ElseIf type Is GetType(Integer) Then
                Return reader.GetInt32OrDefault(ordinal)
            ElseIf type Is GetType(Long) Then
                Return reader.GetInt64OrDefault(ordinal)
            ElseIf type Is GetType(Single) Then
                Return reader.GetFloat(ordinal)
            ElseIf type Is GetType(Double) Then
                Return reader.GetDoubleOrDefault(ordinal)
            ElseIf type Is GetType(Date) Then
                Return reader.GetDateTimeOrDefault(ordinal)
            ElseIf type Is GetType(Boolean) Then
                Dim tipoColonna As Type = reader.GetFieldType(ordinal)
                If tipoColonna Is GetType(String) Then
                    Dim valore As String = reader.GetStringOrDefault(ordinal)
                    Return String.Equals(valore, "s", StringComparison.InvariantCultureIgnoreCase)
                Else
                    Dim valore As Integer = reader.GetInt32OrDefault(ordinal)
                    Return valore > 0
                End If
            End If

            Return reader.GetStringOrDefault(ordinal)
        End Function

    End Module

End Namespace