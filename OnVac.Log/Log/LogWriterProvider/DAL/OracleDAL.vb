Namespace LogWriterProvider.DAL

    Public NotInheritable Class OracleDAL

#Region " Types "

        Private Enum TipoId
            Gruppo = 0
            Record = 1
            Testata = 2
            Campo = 3
        End Enum

#End Region

#Region " Constants "

        Private Const MAXSIZE_STACK As Integer = 4000
        Private Const MAXSIZE_MASCHERA As Integer = 300
        Private Const MAXSIZE_CAMPO As Integer = 100

#End Region

#Region " Queries "

        Private NotInheritable Class OracleQueries

            Public Const SelectNewIdGruppo As String = "SELECT SEQ_LOG_GRUPPO.NEXTVAL FROM DUAL"
            Public Const SelectNewIdRecord As String = "SELECT SEQ_LOG_RECORD.NEXTVAL FROM DUAL"
            Public Const SelectNewIdTestata As String = "SELECT SEQ_LOG_TESTATA.NEXTVAL FROM DUAL"
            Public Const SelectNewIdCampo As String = "SELECT SEQ_LOG_CAMPO.NEXTVAL FROM DUAL"

            Public Const InsertTestata As String = "INSERT INTO T_LOG_TESTATA (LOT_CODICE, LOT_DATA_OPERAZIONE, LOT_UTENTE, LOT_ARGOMENTO, LOT_OPERAZIONE, LOT_MASCHERA, LOT_PAZIENTE, LOT_AUTOMATICO, LOT_GRUPPO, LOT_STACK) VALUES (:LOT_CODICE, :LOT_DATA_OPERAZIONE, :LOT_UTENTE, :LOT_ARGOMENTO, :LOT_OPERAZIONE, :LOT_MASCHERA, :LOT_PAZIENTE, :LOT_AUTOMATICO, :LOT_GRUPPO, :LOT_STACK)"
            Public Const InsertRecord As String = "INSERT INTO T_LOG_RECORD (LOR_RECORD, LOR_TESTATA) VALUES (:LOR_RECORD, :LOR_TESTATA)"
            Public Const InsertCampo As String = "INSERT INTO T_LOG_CAMPI (LOC_RECORD, LOC_PROGRESSIVO, LOC_CAMPO, LOC_VALORE_VECCHIO, LOC_VALORE_NUOVO) VALUES (:LOC_RECORD, :LOC_PROGRESSIVO, :LOC_CAMPO, :LOC_VALORE_VECCHIO, :LOC_VALORE_NUOVO)"

        End Class

#End Region

#Region " Public "

        ''' <summary>
        ''' Restituisce un nuovo id del gruppo
        ''' </summary>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetNewIdGruppo(connection As IDbConnection) As Integer

            Return GetId(TipoId.Gruppo, connection)

        End Function

        ''' <summary>
        ''' Restituisce un nuovo id del record
        ''' </summary>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetNewIdRecord(connection As IDbConnection) As Integer

            Return GetId(TipoId.Record, connection)

        End Function

        ''' <summary>
        ''' Restituisce un nuovo id della testata
        ''' </summary>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetNewIdTestata(connection As IDbConnection) As Integer
            Return GetId(TipoId.Testata, connection)
        End Function

        ''' <summary>
        ''' Restituisce un nuovo id del campo
        ''' </summary>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetNewIdCampo(connection As IDbConnection) As Integer
            Return GetId(TipoId.Campo, connection)
        End Function

        ''' <summary>
        ''' Inserisce la testata specificata.
        ''' </summary>
        ''' <param name="testataLog"></param>
        ''' <param name="idGruppo"></param>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Sub InsertTestata(testataLog As DataLogStructure.Testata, idGruppo As Integer?, connection As IDbConnection)

            Using cmd As OracleClient.OracleCommand = connection.CreateCommand()

                cmd.CommandText = OracleQueries.InsertTestata

                cmd.Parameters.AddWithValue("LOT_CODICE", testataLog.CodiceTestata.Value)
                cmd.Parameters.AddWithValue("LOT_DATA_OPERAZIONE", GetDateTimeParameterValue(testataLog.DataOperazione))
                cmd.Parameters.AddWithValue("LOT_UTENTE", testataLog.Utente)
                cmd.Parameters.AddWithValue("LOT_ARGOMENTO", testataLog.ArgomentoGenerico.Codice)
                cmd.Parameters.AddWithValue("LOT_OPERAZIONE", testataLog.OperazioneBase)
                cmd.Parameters.AddWithValue("LOT_MASCHERA", GetStringParameterValue(testataLog.Maschera, MAXSIZE_MASCHERA))
                cmd.Parameters.AddWithValue("LOT_PAZIENTE", testataLog.Paziente)
                cmd.Parameters.AddWithValue("LOT_AUTOMATICO", IIf(testataLog.Automatico, "S", "N"))
                cmd.Parameters.AddWithValue("LOT_GRUPPO", GetNullableIntegerParameterValue(idGruppo))
                cmd.Parameters.AddWithValue("LOT_STACK", GetStringParameterValue(testataLog.Stack, MAXSIZE_STACK))

                cmd.ExecuteNonQuery()

            End Using

        End Sub

        ''' <summary>
        ''' Inserimento di una coppia (id record - id testata).
        ''' </summary>
        ''' <param name="idRecord"></param>
        ''' <param name="idTestata"></param>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Sub InsertRecord(idRecord As Integer, idTestata As Integer, connection As IDbConnection)

            Using cmd As OracleClient.OracleCommand = connection.CreateCommand()

                cmd.CommandText = OracleQueries.InsertRecord

                cmd.Parameters.AddWithValue("LOR_RECORD", idRecord)
                cmd.Parameters.AddWithValue("LOR_TESTATA", idTestata)

                cmd.ExecuteNonQuery()

            End Using

        End Sub

        ''' <summary>
        ''' Inserimento campo
        ''' </summary>
        ''' <param name="idRecord"></param>
        ''' <param name="campoLog"></param>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function InsertCampo(idRecord As Integer, campoLog As DataLogStructure.Campo, connection As IDbConnection) As Integer

            Using cmd As OracleClient.OracleCommand = connection.CreateCommand()

                cmd.CommandText = OracleQueries.InsertCampo

                cmd.Parameters.AddWithValue("LOC_RECORD", idRecord)
                cmd.Parameters.AddWithValue("LOC_PROGRESSIVO", campoLog.CodiceCampo.Value)
                cmd.Parameters.AddWithValue("LOC_CAMPO", GetStringParameterValue(campoLog.CampoDB, MAXSIZE_CAMPO))
                cmd.Parameters.AddWithValue("LOC_VALORE_VECCHIO", GetStringParameterValue(campoLog.ValoreVecchio, MAXSIZE_CAMPO))
                cmd.Parameters.AddWithValue("LOC_VALORE_NUOVO", GetStringParameterValue(campoLog.ValoreNuovo, MAXSIZE_CAMPO))

                cmd.ExecuteNonQuery()

            End Using

        End Function

#End Region

#Region " Private "

        Private Shared Function GetId(tipoId As TipoId, connection As IDbConnection) As Integer

            Dim id As Integer = 0

            Using cmd As IDbCommand = connection.CreateCommand()

                Select Case tipoId
                    Case OracleDAL.TipoId.Gruppo
                        cmd.CommandText = OracleQueries.SelectNewIdGruppo
                    Case OracleDAL.TipoId.Record
                        cmd.CommandText = OracleQueries.SelectNewIdRecord
                    Case OracleDAL.TipoId.Testata
                        cmd.CommandText = OracleQueries.SelectNewIdTestata
                    Case OracleDAL.TipoId.Campo
                        cmd.CommandText = OracleQueries.SelectNewIdCampo
                End Select

                Dim obj As Object = cmd.ExecuteScalar()

                If Not IsNothingOrDbNull(obj) Then
                    id = Convert.ToInt32(obj)
                End If

            End Using

            Return id

        End Function

        Private Shared Function IsNothingOrDbNull(objectValue As Object) As Boolean

            If objectValue Is Nothing Then Return True
            If objectValue Is DBNull.Value Then Return True

            Return False

        End Function

        Private Shared Function GetStringParameterValue(stringValue As String) As Object

            Return GetStringParameterValue(stringValue, 0)

        End Function

        Private Shared Function GetStringParameterValue(stringValue As String, maxLength As Integer) As Object

            If String.IsNullOrEmpty(stringValue) Then Return DBNull.Value

            If maxLength > 0 AndAlso stringValue.Length > maxLength Then stringValue = stringValue.Substring(0, maxLength)

            Return stringValue

        End Function

        Private Shared Function GetNullableIntegerParameterValue(intValue As Integer?) As Object

            If Not intValue.HasValue Then Return DBNull.Value
            If intValue.Value = -1 Then Return DBNull.Value

            Return intValue.Value

        End Function

        Private Shared Function GetDateTimeParameterValue(dateTimeValue As DateTime) As Object

            If dateTimeValue = DateTime.MinValue Then Return DBNull.Value

            Return dateTimeValue

        End Function

#End Region

    End Class

End Namespace
