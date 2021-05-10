Imports System.Configuration

Imports Onit.OnAssistnet.OnVac.Log.LogWriterProvider
Imports Onit.OnAssistnet.OnVac.Log.DataLogStructure


Public Class LogBox

#Region " Events "

    Public Shared Event PreWriteLog(logProvider As ILogWriterProvider, testataLog As Testata, ByRef connectionString As String)
    Public Shared Event PreWriteLogGroup(logProvider As ILogWriterProvider, testataLog As Testata(), ByRef connectionString As String)
    Public Shared Event ProcessRow(recordLog As Record, row As DataRow)

#End Region

#Region " Properties "

    Private Shared LogConfig As OnVac.Log.Config.ConfigHandler.LogConfigCollection

    Public Shared ReadOnly Property IsEnabled() As Boolean
        Get
            Return LogConfig.EnableAll
        End Get
    End Property

    Private Shared _LogWritersProvider As LogWriterProviderCollection
    Public Shared ReadOnly Property LogWritersProvider() As LogWriterProviderCollection
        Get
            Return _LogWritersProvider
        End Get
    End Property

    Private Shared _Argomenti As ArgomentoCollection
    Public Shared ReadOnly Property Argomenti() As ArgomentoCollection
        Get
            Return _Argomenti
        End Get
    End Property

    Public Shared ReadOnly Property ThrowExceptions() As Boolean
        Get
            Return LogConfig.ThrowExceptions
        End Get
    End Property

#End Region

#Region " Log Manager "

    Public Shared Sub InitLog()

        LogConfig = DirectCast(ConfigurationManager.GetSection("Log"), OnVac.Log.Config.ConfigHandler.LogConfigCollection)

        _LogWritersProvider = New LogWriterProviderCollection()

        For Each c As OnVac.Log.Config.ConfigHandler.LogConfig In LogConfig
            c.LogProvider.Name = c.Name
            _LogWritersProvider.Add(c.LogProvider)
        Next

        If _Argomenti Is Nothing Then _Argomenti = New ArgomentoCollection()

    End Sub

    Public Shared Sub EnableAll(enable As Boolean)

        For Each logProv As ILogWriterProvider In ConfigurationManager.GetSection("Log")
            logProv.Enabled = enable
        Next

        LogConfig.EnableAll = enable

    End Sub

    Public Shared Sub RegisterLogWriter(log As ILogWriterProvider)

        _LogWritersProvider.Add(log)

    End Sub

    Public Shared Sub RemoveLogWriter(log As ILogWriterProvider)

        _LogWritersProvider.Remove(log)

    End Sub

#End Region

#Region " Write Data "

    ''' <summary>
    ''' Scrittura log
    ''' </summary>
    ''' <param name="argomento"></param>
    ''' <param name="criticita"></param>
    ''' <param name="automatico"></param>
    ''' <param name="maschera"></param>
    ''' <param name="intestazione"></param>
    ''' <param name="campo"></param>
    ''' <param name="valore"></param>
    ''' <remarks></remarks>
    Public Shared Sub WriteData(argomento As String, criticita As Criticita, automatico As Boolean, maschera As String, intestazione As Boolean, campo As String, valore As String)

        Try

            If IsEnabled AndAlso Argomenti.Contains(argomento) Then

                Dim testataLog As New Testata(argomento, Operazione.Generico, automatico, maschera)
                testataLog.Intestazione = True

                Dim recordLog As New Record()
                recordLog.Campi.Add(New Campo(campo, valore))

                testataLog.Records.Add(recordLog)

                For Each logProvider As ILogWriterProvider In _LogWritersProvider

                    If logProvider.HandleCriticita(criticita) Then

                        Dim connString As String = Nothing

                        RaiseEvent PreWriteLog(logProvider, testataLog, connString)

                        logProvider.WriteLog(testataLog, False, connString, Nothing)

                    End If

                Next

            End If

        Catch ex As Exception

            If ThrowExceptions Then

                GetType(System.Exception).GetMethod("InternalPreserveStackTrace", System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic).Invoke(ex, Nothing)
                Throw

            End If

        End Try

    End Sub

    ''' <summary>
    ''' Scrittura log della testata specificata
    ''' </summary>
    ''' <param name="testataLog"></param>
    ''' <remarks></remarks>
    Public Shared Sub WriteData(testataLog As Testata)

        LogBox.WriteData(New Testata() {testataLog})

    End Sub

    ''' <summary>
    ''' Scrittura log delle testate specificate.
    ''' Viene scritto il log solo per le testate aventi argomento attivo.
    ''' </summary>
    ''' <param name="testataLog"></param>
    ''' <remarks></remarks>
    Public Shared Sub WriteData(testateLog As Testata())

        LogBox.WriteData(testateLog, False, String.Empty, String.Empty)

    End Sub

    ''' <summary>
    ''' Scrittura log della testata specificata.
    ''' Viene scritto il log solo se l'argomento è attivo.
    ''' </summary>
    ''' <param name="testataLog"></param>
    ''' <param name="forzaGruppo"></param>
    ''' <param name="connectionString"></param>
    ''' <param name="provider"></param>
    ''' <remarks></remarks>
    Public Shared Sub WriteData(testataLog As Testata, forzaGruppo As Boolean, connectionString As String, provider As String)

        Try

            If Not testataLog Is Nothing Then

                If IsEnabled AndAlso Not testataLog.ArgomentoGenerico Is Nothing AndAlso Argomenti.Contains(testataLog.ArgomentoGenerico.Codice) Then

                    For Each logProvider As ILogWriterProvider In _LogWritersProvider

                        If logProvider.HandleCriticita(testataLog.ArgomentoGenerico.Criticita) Then

                            Dim connString As String = Nothing

                            If String.IsNullOrEmpty(connectionString) Then
                                RaiseEvent PreWriteLog(logProvider, testataLog, connString)
                            Else
                                connString = connectionString
                            End If

                            logProvider.WriteLog(testataLog, forzaGruppo, connString, provider)

                        End If

                    Next

                End If

            End If

        Catch ex As Exception

            If ThrowExceptions Then

                GetType(System.Exception).GetMethod("InternalPreserveStackTrace", System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic).Invoke(ex, Nothing)
                Throw

            End If

        End Try

    End Sub

    ''' <summary>
    ''' Scrittura log delle testate specificate.
    ''' Viene scritto il log solo per le testate aventi argomento attivo.
    ''' </summary>
    ''' <param name="testateLog"></param>
    ''' <param name="forzaGruppo"></param>
    ''' <param name="connectionString"></param>
    ''' <param name="provider"></param>
    ''' <remarks></remarks>
    Public Shared Sub WriteData(testateLog As Testata(), forzaGruppo As Boolean, connectionString As String, provider As String)

        Try

            If IsEnabled AndAlso Not testateLog Is Nothing AndAlso testateLog.Count > 0 Then

                If testateLog.Length = 1 Then

                    WriteData(testateLog(0), forzaGruppo, connectionString, provider)

                Else

                    ' filtro delle testate per i soli argomenti attivi
                    Dim testataLogAttivaList As New ArrayList()

                    For i As Integer = 0 To testateLog.Length - 1

                        If Not testateLog(i).ArgomentoGenerico Is Nothing AndAlso Argomenti.Contains(testateLog(i).ArgomentoGenerico.Codice) Then
                            testataLogAttivaList.Add(testateLog(i))
                        End If

                    Next

                    If testataLogAttivaList.Count > 0 Then

                        For Each logProvider As ILogWriterProvider In _LogWritersProvider

                            Dim connString As String = Nothing

                            If String.IsNullOrEmpty(connectionString) Then
                                RaiseEvent PreWriteLogGroup(logProvider, testateLog, connString)
                            Else
                                connString = connectionString
                            End If

                            logProvider.WriteLog(testataLogAttivaList.ToArray(GetType(Testata)), forzaGruppo, connString, provider)

                        Next

                    End If

                End If

            End If

        Catch ex As Exception

            If ThrowExceptions Then

                GetType(System.Exception).GetMethod("InternalPreserveStackTrace", System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic).Invoke(ex, Nothing)
                Throw

            End If

        End Try

    End Sub

#End Region

#Region " Utility "

    Public Shared Function GetTestataException(ex As Exception) As Testata

        Dim testataLog As New Testata(DataLogStructure.TipiArgomento.EXCEPTION, Operazione.Eccezione, 0, True, ex.Source)

        Dim positionOfInterestingStack As Integer

        testataLog.Stack = ex.StackTrace
        positionOfInterestingStack = testataLog.Stack.IndexOf("at OnVac")

        If positionOfInterestingStack > 0 Then
            testataLog.Stack = testataLog.Stack.Remove(0, positionOfInterestingStack)
        End If

        Dim recordLog As New Record()
        recordLog.Campi.Add(New Campo("METODO", ex.TargetSite.Name))
        recordLog.Campi.Add(New Campo("MESSAGE", ex.Message))
        testataLog.Records.Add(recordLog)

        Return testataLog

    End Function

    Public Shared Function GetTestataDataTable(data As DataTable, argomentoLog As Argomento, campi() As String) As Testata()

        Dim testataLogInserimento As New Testata(argomentoLog, Operazione.Inserimento)
        Dim testataLogModifica As New Testata(argomentoLog, Operazione.Modifica)
        Dim testataLogEliminazione As New Testata(argomentoLog, Operazione.Eliminazione)

        For Each row As DataRow In data.Rows

            Dim recordLog As Record

            If campi Is Nothing Then
                recordLog = GetRecordFromRow(row)
            Else
                recordLog = GetRecordFromRow(row, campi)
            End If

            If recordLog.Campi.Count > 0 Then

                RaiseEvent ProcessRow(recordLog, row)

                Select Case row.RowState

                    Case DataRowState.Added
                        testataLogInserimento.Records.Add(recordLog)

                    Case DataRowState.Deleted
                        testataLogEliminazione.Records.Add(recordLog)

                    Case DataRowState.Modified
                        testataLogModifica.Records.Add(recordLog)

                End Select

            End If

        Next

        Dim testataLogList As New ArrayList()

        If testataLogModifica.Records.Count > 0 Then testataLogList.Add(testataLogModifica)
        If testataLogEliminazione.Records.Count > 0 Then testataLogList.Add(testataLogEliminazione)
        If testataLogInserimento.Records.Count > 0 Then testataLogList.Add(testataLogInserimento)

        Return testataLogList.ToArray(GetType(Testata))

    End Function

    Public Shared Function GetTestataDataTable(data As DataTable, argomento As String, campi() As String) As Testata()

        Return GetTestataDataTable(data, _Argomenti(argomento), campi)

    End Function

    Public Shared Function GetTestataDataTable(data As DataTable, argomentoLog As Argomento) As Testata()

        Return GetTestataDataTable(data, argomentoLog, Nothing)

    End Function

    Public Shared Function GetTestataDataTable(data As DataTable, argomento As String) As Testata()

        Return GetTestataDataTable(data, _Argomenti(argomento))

    End Function

    Public Shared Function GetRecordFromRow(descrizioneComuneCampi As String, row As DataRow) As Record

        Dim recordLog As New Record()

        Dim value As String

        For i As Integer = 0 To row.ItemArray.Length - 1

            If row(i) Is Nothing OrElse row(i) Is System.DBNull.Value Then
                value = "NULL"
            Else
                value = row(i)
            End If

            recordLog.Campi.Add(New Campo(String.Format("{0}: {1}", descrizioneComuneCampi, row.Table.Columns(i).ColumnName), value))

        Next

        Return recordLog

    End Function

    Public Shared Function GetRecordFromRow(row As DataRow) As Record

        Dim recordLog As New Record()

        For Each f As DataColumn In row.Table.Columns

            If row.RowState = DataRowState.Modified Then
                If row(f, DataRowVersion.Original).ToString() <> row(f).ToString() Then recordLog.Campi.Add(New Campo(f.ColumnName, row(f, DataRowVersion.Original).ToString(), row(f).ToString()))
            ElseIf row.RowState = DataRowState.Added Then
                If row(f).ToString <> "" Then recordLog.Campi.Add(New Campo(f.ColumnName, "", row(f).ToString))
            ElseIf row.RowState = DataRowState.Deleted Then
                If row(f, DataRowVersion.Original).ToString <> "" Then recordLog.Campi.Add(New Campo(f.ColumnName, row(f, DataRowVersion.Original).ToString))
            End If

        Next

        Return recordLog

    End Function

    Public Shared Function GetRecordFromRow(row As DataRow, campi As String()) As Record

        Dim recordLog As New Record()

        For Each f As String In campi

            If row.RowState = DataRowState.Modified Then
                If row(f, DataRowVersion.Original).ToString() <> row(f).ToString() Then recordLog.Campi.Add(New Campo(f, row(f, DataRowVersion.Original).ToString(), row(f).ToString()))
            ElseIf row.RowState = DataRowState.Added Then
                If row(f).ToString() <> "" Then recordLog.Campi.Add(New Campo(f, "", row(f).ToString()))
            ElseIf row.RowState = DataRowState.Deleted Then
                If row(f, DataRowVersion.Original).ToString() <> "" Then recordLog.Campi.Add(New Campo(f, row(f, DataRowVersion.Original).ToString()))
            End If

        Next

        Return recordLog

    End Function

    Public Shared Function GetRowDifferences(originalRow As DataRow, modifiedRow As DataRow, campi As String()) As Record

        Dim recordLog As New Record()

        For Each campo As String In campi

            Dim p As DataColumn = originalRow.Table.Columns(campo)

            Dim oldValue As String = originalRow(p.Caption).ToString()
            Dim newValue As String = modifiedRow(p.Caption).ToString()

            If oldValue <> newValue Then
                recordLog.Campi.Add(New Campo(p.ColumnName, oldValue, newValue))
            End If

        Next

        Return recordLog

    End Function

    Public Shared Function GetRowDifferences(originalRow As DataRow, modifiedRow As DataRow) As Record

        Dim recordLog As New Record()

        For Each column As DataColumn In originalRow.Table.Columns

            Dim oldValue As String = originalRow(column.Caption).ToString()
            Dim newValue As String = modifiedRow(column.Caption).ToString()

            If oldValue <> newValue Then
                recordLog.Campi.Add(New Campo(column.ColumnName, oldValue, newValue))
            End If

        Next

        Return recordLog

    End Function

#End Region

End Class
