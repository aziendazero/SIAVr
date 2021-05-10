Imports Onit.OnAssistnet.OnVac.Log.DataLogStructure

Namespace LogWriterProvider

    Public NotInheritable Class SupportedProvider
        Public Const OracleClient As String = "OracleClient"
    End Class

    Public Class DbLogWriterProvider
        Implements ILogWriterProvider

#Region " Constants "

        Const SIZE As Integer = 100
        Const SPLITTER As String = "|"

#End Region

#Region " Properties "

        Private _Enabled As Boolean
        Public Property Enabled() As Boolean Implements ILogWriterProvider.Enabled
            Get
                Return _Enabled
            End Get
            Set(Value As Boolean)
                _Enabled = Value
            End Set
        End Property

        Private _DataProvider As String
        Public Property DataProvider() As String
            Get
                Return _DataProvider
            End Get
            Set(Value As String)
                _DataProvider = Value
            End Set
        End Property

        Private _FiltroCriticita As String
        Private _FiltroCriticitaArray As Integer()
        Public Property FiltroCriticita() As String Implements ILogWriterProvider.FiltroCriticita
            Get
                Return _FiltroCriticita
            End Get
            Set(Value As String)
                _FiltroCriticita = Value
                Dim arr As String() = Value.Split(",")
                _FiltroCriticitaArray = Array.CreateInstance(GetType(Integer), arr.Length)
                For i As Integer = 0 To arr.Length - 1
                    _FiltroCriticitaArray(i) = Integer.Parse(arr(i))
                Next
            End Set
        End Property

        Private _Name As String
        Public Property Name() As String Implements ILogWriterProvider.Name
            Get
                Return _Name
            End Get
            Set(Value As String)
                _Name = Value
            End Set
        End Property

#End Region

#Region " Public "

#Region " Overloads "

        ''' <summary>
        ''' Scrittura del log relativo alla testata specificata
        ''' </summary>
        ''' <param name="testataLog"></param>
        ''' <param name="forceGroup"></param>
        ''' <param name="connectionString"></param>
        ''' <param name="provider"></param>
        ''' <remarks></remarks>
        Public Overloads Sub WriteLog(testataLog As Testata, forceGroup As Boolean, connectionString As String, provider As String) Implements ILogWriterProvider.WriteLog

            If Me.Enabled Then
                WriteLog(New Testata() {testataLog}, forceGroup, connectionString, provider)
            End If

        End Sub

        ''' <summary>
        ''' Scrittura del log relativo all'array di testate specificato
        ''' </summary>
        ''' <param name="testateLog"></param>
        ''' <param name="forceGroup"></param>
        ''' <param name="connectionString"></param>
        ''' <param name="provider"></param>
        ''' <remarks></remarks>
        Public Overloads Sub WriteLog(testateLog As DataLogStructure.Testata(), forceGroup As Boolean, connectionString As String, provider As String) Implements ILogWriterProvider.WriteLog

            If Me.Enabled Then
                '--------------------------------------------
                Dim transactionOptions As New Transactions.TransactionOptions()
                transactionOptions.IsolationLevel = Transactions.IsolationLevel.ReadCommitted
                '--------------------------------------------
                Using transactionScope As New System.Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, transactionOptions)
                    '--------------------------------------------
                    Using cn As IDbConnection = GetConnection(connectionString, provider)
                        '--------------------------------------------
                        cn.Open()
                        '--------------------------------------------
                        Using cmd As IDbCommand = cn.CreateCommand()

                            Dim count As Integer = testateLog.Length

                            If count > 1 OrElse forceGroup Then
                                '--
                                ' Log con calcolo id gruppo
                                '--
                                Dim idGruppo As Integer = GetNewGroupId(cn, provider)

                                For Each testataLog As DataLogStructure.Testata In testateLog
                                    WriteTestata(cmd, testataLog, idGruppo, provider)
                                Next

                            Else
                                '--
                                ' Log senza id gruppo
                                '--
                                If count > 0 Then WriteTestata(cmd, testateLog(0), provider)

                            End If

                        End Using
                        '--------------------------------------------                      
                    End Using
                    '--------------------------------------------
                    transactionScope.Complete()
                    '--------------------------------------------
                End Using
                '--------------------------------------------
            End If

        End Sub

#End Region

#Region " Write Log "

        ''' <summary>
        ''' Scrittura della testata specificata, senza id gruppo
        ''' </summary>
        ''' <param name="cmd"></param>
        ''' <param name="testataLog"></param>
        ''' <param name="provider"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function WriteTestata(ByRef cmd As IDbCommand, testataLog As DataLogStructure.Testata, provider As String) As Integer

            Return WriteTestata(cmd, testataLog, -1, provider)

        End Function

        ''' <summary>
        ''' Scrittura della testata specificata, con id gruppo 
        ''' </summary>
        ''' <param name="cmd"></param>
        ''' <param name="testataLog"></param>
        ''' <param name="groupId">impostandolo a -1 verrà inserito nullo</param>
        ''' <param name="provider"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function WriteTestata(ByRef cmd As IDbCommand, testataLog As DataLogStructure.Testata, idGruppo As Integer, provider As String) As Integer

            Dim connectionProvider As String = provider
            If String.IsNullOrEmpty(connectionProvider) Then connectionProvider = Me.DataProvider

            If connectionProvider <> SupportedProvider.OracleClient Then
                Throw New NotSupportedException(String.Format("Provider non supportato: {0}", connectionProvider))
            End If

            ' Sequence per progressivo testata
            Dim codiceTestata As Integer = LogWriterProvider.DAL.OracleDAL.GetNewIdTestata(cmd.Connection)

            testataLog.CodiceTestata = codiceTestata

            ' Inserimento testata
            LogWriterProvider.DAL.OracleDAL.InsertTestata(testataLog, idGruppo, cmd.Connection)

            ' Inserimento record e campi
            For Each recordLog As Record In testataLog.Records

                Dim idRecord As Integer = LogWriterProvider.DAL.OracleDAL.GetNewIdRecord(cmd.Connection)

                WriteRecordTestata(cmd, idRecord, codiceTestata, connectionProvider)

                For Each campoLog As Campo In recordLog.Campi
                    WriteRecord(cmd, idRecord, campoLog, connectionProvider)
                Next

            Next

            Return codiceTestata

        End Function

        ''' <summary>
        ''' Scrittura di una coppia testata-record
        ''' </summary>
        ''' <param name="cmd"></param>
        ''' <param name="idRecord"></param>
        ''' <param name="idTestata"></param>
        ''' <param name="provider"></param>
        ''' <remarks></remarks>
        Public Sub WriteRecordTestata(ByRef cmd As IDbCommand, idRecord As Integer, idTestata As Integer, provider As String)

            Dim connectionProvider As String = provider
            If String.IsNullOrEmpty(connectionProvider) Then connectionProvider = Me.DataProvider

            If connectionProvider <> SupportedProvider.OracleClient Then
                Throw New NotSupportedException(String.Format("Provider non supportato: {0}", connectionProvider))
            End If

            LogWriterProvider.DAL.OracleDAL.InsertRecord(idRecord, idTestata, cmd.Connection)

        End Sub

        ''' <summary>
        ''' Scrive i dati del campo specificato, un record per ogni dato. Restituisce il numero dei record inseriti.
        ''' </summary>
        ''' <param name="cmd"></param>
        ''' <param name="idRecord"></param>
        ''' <param name="campoLog"></param>
        ''' <param name="provider"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function WriteRecord(ByRef cmd As IDbCommand, idRecord As Integer, campoLog As DataLogStructure.Campo, provider As String) As Integer

            Dim connectionProvider As String = provider
            If String.IsNullOrEmpty(connectionProvider) Then connectionProvider = Me.DataProvider

            If connectionProvider <> SupportedProvider.OracleClient Then
                Throw New NotSupportedException(String.Format("Provider non supportato: {0}", connectionProvider))
            End If

            Dim count As Integer = 0

            Dim campiLog As CampoCollection = SplitCampo(campoLog)

            If Not campiLog Is Nothing AndAlso campiLog.Count > 0 Then

                count = campiLog.Count

                For Each campoLogCurrent As DataLogStructure.Campo In campiLog

                    ' Sequence per progressivo campo
                    campoLogCurrent.CodiceCampo = LogWriterProvider.DAL.OracleDAL.GetNewIdCampo(cmd.Connection)

                    ' Inserimento campo
                    LogWriterProvider.DAL.OracleDAL.InsertCampo(idRecord, campoLogCurrent, cmd.Connection)

                Next

            End If

            Return count

        End Function

#End Region

#Region " Criticità "

        Public Function HandleCriticita(priorita As Integer) As Boolean Implements ILogWriterProvider.HandleCriticita

            Return Array.IndexOf(_FiltroCriticitaArray, priorita) > -1

        End Function

#End Region

#End Region

#Region " Private "

        Private Function GetConnection(connectionString As String, provider As String) As IDbConnection

            If String.IsNullOrEmpty(provider) Then
                provider = Me._DataProvider
            End If

            Dim connection As IDbConnection = Nothing

            Select Case provider
                Case SupportedProvider.OracleClient
                    connection = New OracleClient.OracleConnection(connectionString)
                Case Else
                    Throw New NotSupportedException(String.Format("Provider non supportato: {0}", provider))
            End Select

            Return connection

        End Function

        Private Function GetNewGroupId(connection As IDbConnection, provider As String) As Integer

            Dim idGruppo As Integer = 0

            Dim connectionProvider As String = provider
            If String.IsNullOrEmpty(connectionProvider) Then connectionProvider = Me.DataProvider

            Select Case connectionProvider

                Case SupportedProvider.OracleClient
                    idGruppo = LogWriterProvider.DAL.OracleDAL.GetNewIdGruppo(connection)

                Case Else
                    Throw New NotSupportedException(String.Format("Provider non supportato: {0}", connectionProvider))

            End Select

            Return idGruppo

        End Function

        Private Function SplitCampo(campoLog As DataLogStructure.Campo) As CampoCollection

            Dim result As New DataLogStructure.CampoCollection()

            If campoLog.ValoreVecchio Is Nothing Then campoLog.ValoreVecchio = String.Empty
            If campoLog.ValoreNuovo Is Nothing Then campoLog.ValoreNuovo = String.Empty

            Dim currentSize As Integer

            If campoLog.ValoreVecchio.Length >= campoLog.ValoreNuovo.Length Then
                currentSize = campoLog.ValoreVecchio.Length
            Else
                currentSize = campoLog.ValoreNuovo.Length
            End If

            If currentSize > SIZE Then

                Try
                    ' Calcolo degli elementi in cui splittare il campo
                    Dim elements As Integer = currentSize \ SIZE
                    If currentSize Mod SIZE Then elements = elements + 1

                    ' Inserimento della stringa di splitter
                    For i As Integer = 0 To elements - 1
                        If campoLog.ValoreVecchio.Length >= ((i + 1) * SIZE) Then
                            campoLog.ValoreVecchio = campoLog.ValoreVecchio.Insert((i + 1) * SIZE, SPLITTER)
                        End If
                        If campoLog.ValoreNuovo.Length >= ((i + 1) * SIZE) Then
                            campoLog.ValoreNuovo = campoLog.ValoreNuovo.Insert((i + 1) * SIZE, SPLITTER)
                        End If
                    Next

                    ' Creazione della nuova collezione di campi
                    Dim sv As String() = campoLog.ValoreVecchio.Split(SPLITTER)
                    Dim sn As String() = campoLog.ValoreNuovo.Split(SPLITTER)

                    For i As Integer = 0 To elements - 1

                        Dim svCur As String = String.Empty
                        Dim snCur As String = String.Empty

                        If sv.Length > i Then
                            svCur = sv(i)
                        End If
                        If sn.Length > i Then
                            snCur = sn(i)
                        End If

                        result.Add(New Campo(String.Format("{0}_{1}", campoLog.CampoDB, i), svCur, snCur))

                    Next

                Catch ex As Exception

                    result.Add(New Campo("SplitCampo fallito", ex.Message))

                End Try

            Else
                result.Add(campoLog)
            End If

            Return result

        End Function

        Private Function StringToArray(s As String) As String()

            Dim result As String()
            Dim elements As Integer

            If s.Length >= SIZE Then

                elements = s / SIZE

                If s Mod SIZE Then elements = elements + 1

                For i As Integer = 0 To elements - 1
                    s = s.Insert(i * SIZE, SPLITTER)
                Next

                result = s.Split(SPLITTER)

            Else
                result = New String() {s}
            End If

            Return result

        End Function

#End Region

    End Class

End Namespace
