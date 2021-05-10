Imports System.Data.OracleClient
Imports Onit.Database.DataAccessManager
Imports System.Collections.Generic


Namespace DAL

    Public Module Extensions


#Region " Public "

#Region " DAM "

        ''' <summary>
        ''' Data una query scritta con l'OnitDAM, restituisce la query paginata corrispondente
        ''' </summary>
        ''' <param name="abstractQB"></param>
        ''' <param name="startRecordIndex"></param>
        ''' <param name="endRecordIndex"></param>
        ''' <remarks></remarks>
        <System.Runtime.CompilerServices.Extension()>
        Public Sub AddPaginatedOracleQuery(ByRef abstractQB As AbstractQB, startRecordIndex As Integer, endRecordIndex As Integer)

            With abstractQB

                Dim query1 As String = .GetSelect()

                .NewQuery(False, True)
                .AddSelectFields("ROWNUM ROW_NUM", "A.*")
                .AddTables(String.Format("({0}) A", query1))
                .AddWhereCondition("ROWNUM", Comparatori.MinoreUguale, endRecordIndex, DataTypes.Numero)

                Dim query2 As String = .GetSelect()

                .NewQuery(False, True)
                .AddSelectFields("*")
                .AddTables(String.Format("({0})", query2))
                .AddWhereCondition("ROW_NUM", Comparatori.Maggiore, startRecordIndex, DataTypes.Numero)

            End With

        End Sub

        ''' <summary>
        ''' Aggiunge una WhereCondition di In al QB del DAM, aggiungendo i valori specificati tra i CustomParam
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="abstractQB"></param>
        ''' <param name="fieldName"></param>
        ''' <param name="values"></param>
        ''' <remarks></remarks>
        <System.Runtime.CompilerServices.Extension()>
        Public Sub AddInWhereCondition(Of T)(ByRef abstractQB As AbstractQB, fieldName As String, values As List(Of T))

            If values Is Nothing OrElse values.Count = 0 Then Return

            Dim filtroIn As New System.Text.StringBuilder()

            For i As Integer = 0 To values.Count - 1
                filtroIn.AppendFormat("{0},", abstractQB.AddCustomParam(values(i)))
            Next

            If filtroIn.Length > 0 Then
                filtroIn.Remove(filtroIn.Length - 1, 1)
                abstractQB.AddWhereCondition(fieldName, Comparatori.In, filtroIn.ToString(), DataTypes.Replace)
            End If

        End Sub

#End Region

#Region " DataReader "

        ''' <summary>
        ''' Restituisce la posizione del campo nella query eseguita dal reader, se il campo è nell'elenco specificato.
        ''' Altrimenti restituisce -1
        ''' </summary>
        <System.Runtime.CompilerServices.Extension()>
        Public Function GetOrdinalOrDefault(ByRef idr As IDataReader, nomeCampo As String, elencoCampi As String) As Integer

            If Not elencoCampi.ToLower().Contains(nomeCampo.ToLower()) Then Return -1

            Return idr.GetOrdinal(nomeCampo)

        End Function

        ''' <summary>
        ''' Restituisce il valore stringa del campo specificato. 
        ''' Restituisce String.Empty se il campo contiene dbnull o se la posizione non è valida.
        ''' </summary>
        <System.Runtime.CompilerServices.Extension()>
        Public Function GetStringOrDefault(ByRef idr As IDataReader, ordinal As Integer) As String

            If IsOrdinalInvalidOrNull(idr, ordinal) Then Return String.Empty

            Return idr.GetString(ordinal)

        End Function

        ''' <summary>
        ''' Restituisce il valore datettime del campo specificato. 
        ''' Restituisce MinValue se il campo contiene dbnull o se la posizione non è valida.
        ''' </summary>
        <System.Runtime.CompilerServices.Extension()>
        Public Function GetDateTimeOrDefault(ByRef idr As IDataReader, ordinal As Integer) As DateTime

            If IsOrdinalInvalidOrNull(idr, ordinal) Then Return DateTime.MinValue

            Return idr.GetDateTime(ordinal)

        End Function

        ''' <summary>
        ''' Restituisce il valore booleano del campo specificato. 
        ''' Restituisce Nothing se il campo contiene dbnull o se la posizione non è valida.
        ''' </summary>
        <System.Runtime.CompilerServices.Extension()>
        Public Function GetBooleanOrDefault(ByRef idr As IDataReader, ordinal As Integer) As Boolean

            If IsOrdinalInvalidOrNull(idr, ordinal) Then Return False

            Dim val As String = idr.GetString(ordinal).ToUpper()
            Return val = "S" OrElse val = "T" OrElse val = "1"

        End Function

        ''' <summary>
        ''' Restituisce il valore Decimal del campo specificato. 
        ''' Restituisce 0 se il campo contiene dbnull o se la posizione non è valida.
        ''' </summary>
        <System.Runtime.CompilerServices.Extension()>
        Public Function GetDecimalOrDefault(ByRef idr As IDataReader, ordinal As Integer) As Decimal

            If IsOrdinalInvalidOrNull(idr, ordinal) Then Return 0

            Return idr.GetDecimal(ordinal)

        End Function

        ''' <summary>
        ''' Restituisce il valore Decimal del campo specificato. 
        ''' Restituisce 0 se il campo contiene dbnull o se la posizione non è valida.
        ''' </summary>
        <System.Runtime.CompilerServices.Extension()>
        Public Function GetNullableDecimalOrDefault(ByRef idr As IDataReader, ordinal As Integer) As Decimal?

            If IsOrdinalInvalidOrNull(idr, ordinal) Then Return Nothing

            Return idr.GetDecimal(ordinal)

        End Function
        ''' <summary>
        ''' Restituisce il valore intero del campo specificato. 
        ''' Restituisce 0 se il campo contiene dbnull o se la posizione non è valida.
        ''' </summary>
        <System.Runtime.CompilerServices.Extension()>
        Public Function GetInt32OrDefault(ByRef idr As IDataReader, ordinal As Integer) As Int32

            If IsOrdinalInvalidOrNull(idr, ordinal) Then Return 0

            Return idr.GetDecimal(ordinal)

        End Function

        ''' <summary>
        ''' Restituisce il valore intero a 64 bit del campo specificato. 
        ''' Restituisce 0 se il campo contiene dbnull o se la posizione non è valida.
        ''' </summary>
        <System.Runtime.CompilerServices.Extension()>
        Public Function GetInt64OrDefault(ByRef idr As IDataReader, ordinal As Integer) As Int64

            If IsOrdinalInvalidOrNull(idr, ordinal) Then Return 0

            Return idr.GetInt64(ordinal)

        End Function

        ''' <summary>
        ''' Restituisce il valore double del campo specificato. 
        ''' Restituisce 0 se il campo contiene dbnull o se la posizione non è valida.
        ''' </summary>
        <System.Runtime.CompilerServices.Extension()>
        Public Function GetDoubleOrDefault(ByRef idr As IDataReader, ordinal As Integer) As Double

            If IsOrdinalInvalidOrNull(idr, ordinal) Then Return 0

            Return idr.GetDouble(ordinal)

        End Function

        ''' <summary>
        ''' Restituisce il valore double del campo specificato. 
        ''' Restituisce 0 se il campo contiene dbnull o se la posizione non è valida.
        ''' </summary>
        <System.Runtime.CompilerServices.Extension()>
        Public Function GetNullableDoubleOrDefault(ByRef idr As IDataReader, ordinal As Integer) As Double?

            If IsOrdinalInvalidOrNull(idr, ordinal) Then Return Nothing

            Return idr.GetDouble(ordinal)

        End Function

        ''' <summary>
        ''' Restituisce il valore intero del campo specificato. 
        ''' Restituisce Nothing se il campo contiene dbnull o se la posizione non è valida.
        ''' </summary>
        <System.Runtime.CompilerServices.Extension()>
        Public Function GetNullableInt32OrDefault(ByRef idr As IDataReader, ordinal As Integer) As Int32?

            If IsOrdinalInvalidOrNull(idr, ordinal) Then Return Nothing

            Return idr.GetDecimal(ordinal)

        End Function

        ''' <summary>
        ''' Restituisce il valore intero del campo specificato. 
        ''' Restituisce Nothing se il campo contiene dbnull o se la posizione non è valida.
        ''' </summary>
        <System.Runtime.CompilerServices.Extension()>
        Public Function GetNullableInt64OrDefault(ByRef idr As IDataReader, ordinal As Integer) As Int64?

            If IsOrdinalInvalidOrNull(idr, ordinal) Then Return Nothing

            Return idr.GetInt64(ordinal)

        End Function

        ''' <summary>
        ''' Restituisce il valore dell'enumerazione specificata.
        ''' Restituisce Nothing se il campo contiene dbnull o se la posizione non è valida.
        ''' </summary>
        <System.Runtime.CompilerServices.Extension()>
        Public Function GetNullableEnumOrDefault(Of TEntity As Structure)(ByRef idr As IDataReader, ordinal As Integer) As TEntity?

            If IsOrdinalInvalidOrNull(idr, ordinal) Then Return Nothing

            Return [Enum].Parse(GetType(TEntity), idr.GetString(ordinal))

        End Function

        ''' <summary>
        ''' Restituisce il valore dell'enumerazione specificata.
        ''' Restituisce Nothing se il campo contiene dbnull o se la posizione non è valida.
        ''' </summary>
        <System.Runtime.CompilerServices.Extension()>
        Public Function GetNullableDateTimeOrDefault(ByRef idr As IDataReader, ordinal As Integer) As DateTime?

            If IsOrdinalInvalidOrNull(idr, ordinal) Then Return Nothing

            Return idr.GetDateTimeOrDefault(ordinal)

        End Function
        <System.Runtime.CompilerServices.Extension()>
        Public Function GetGuidOrDefault(idr As IDataReader, ordinal As Integer) As Guid

            If (IsOrdinalInvalidOrNull(idr, ordinal)) Then Return Guid.Empty

            Dim _size As Long = idr.GetBytes(ordinal, 0, Nothing, 0, 0)
            Dim buffer As Byte() = New [Byte](_size - 1) {}
            _size = idr.GetBytes(ordinal, 0, buffer, 0, CInt(_size))
            Return New Guid(buffer)
        End Function



#End Region

        <System.Runtime.CompilerServices.Extension()>
        Public Function AddParameter(ByRef cmd As IDbCommand, ByVal name As String, ByVal value As Object) As IDbDataParameter
            Dim par As IDbDataParameter = cmd.CreateParameter()
            par.ParameterName = name
            If value Is Nothing Then
                par.Value = DBNull.Value
            Else
                par.DbType = GetParameterType(value.GetType())
                If value.GetType() Is GetType(Boolean) Then
                    Dim t As Boolean = DirectCast(value, Boolean)
                    If t Then
                        par.Value = "S"
                    Else
                        par.Value = "N"
                    End If
                Else
                    par.Value = value
                End If
            End If
            cmd.Parameters.Add(par)
            Return par
        End Function

        <System.Runtime.CompilerServices.Extension()>
        Public Function AddReturnParameter(Of T)(ByRef cmd As IDbCommand, ByVal name As String) As IDbDataParameter
            Dim par As IDbDataParameter = cmd.CreateParameter()
            par.ParameterName = name
            par.Value = DBNull.Value
            par.DbType = GetParameterType(GetType(T))
            par.Direction = ParameterDirection.Output

            cmd.Parameters.Add(par)
            Return par
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
            ElseIf tipo Is GetType(Byte()) Then
                Return DbType.Binary
            End If

            If tipo.IsGenericType AndAlso tipo.GetGenericTypeDefinition() Is GetType(Nullable(Of)) Then
                Return GetParameterType(tipo.GetGenericArguments().First())
            End If

            Return DbType.String

        End Function

        <System.Runtime.CompilerServices.Extension()>
        Public Function ParseScalar(Of T)(ByRef parametro As IDbDataParameter) As T
            Dim value As Object = parametro.Value
            If value Is Nothing OrElse value Is DBNull.Value Then
                Return CType(Nothing, T)
            End If
            Dim destinazione As Type = GetType(T)

            If destinazione.IsGenericType AndAlso destinazione.GetGenericTypeDefinition() = GetType(Nullable(Of)) Then
                destinazione = destinazione.GetGenericArguments().First()
            End If

            Dim c As IConvertible = CType(value, IConvertible)
            Return c.ToType(destinazione, Nothing)

        End Function

#End Region

#Region " Private "

        ''' <summary>
        ''' Restituisce un nome univoco nell'oraclecommand per il parametro specificato
        ''' </summary>
        Private Function GetUniqueParamName(ByVal cmd As OracleCommand, ByVal paramName As String) As String
            Dim countParam As Integer = 0

            Dim unique As Boolean = False

            Dim paramNameToFind As String = paramName

            While Not unique

                countParam = (From p As OracleClient.OracleParameter In cmd.Parameters
                              Where p.ParameterName = paramNameToFind
                              Select p).Count()

                If countParam >= 0 Then

                    paramNameToFind = String.Format("{0}{1}", paramNameToFind, (countParam + 1).ToString())

                End If

                unique = (countParam = 0)

            End While

            Return paramNameToFind

        End Function

        ''' <summary>
        ''' Restituisce true se la posizione specificata nel reader non è valida oppure contiene dbnull
        ''' </summary>
        Private Function IsOrdinalInvalidOrNull(ByVal idr As IDataReader, ByVal ordinal As Integer) As Boolean

            Return (ordinal < 0) OrElse (idr.IsDBNull(ordinal))

        End Function

#End Region

    End Module

End Namespace
