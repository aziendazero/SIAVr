Imports Onit.OnAssistnet.OnVac.Log.DataLogStructure

Namespace LogWriterProvider
    Public Class FileLogWriterProvider
        Implements ILogWriterProvider

#Region " Constants "

        Const logIntestazione As Boolean = True

#End Region

#Region " Properties "

        Private _Name As String
        Public Property Name() As String Implements ILogWriterProvider.Name
            Get
                Return _Name
            End Get
            Set(Value As String)
                _Name = Value
            End Set
        End Property

        Private _AbsoluteFileName As String
        Public Property AbsoluteFileName() As String
            Get
                Return _AbsoluteFileName
            End Get
            Set(value As String)
                _AbsoluteFileName = value
            End Set
        End Property

        Private _Enabled As Boolean
        Public Property Enabled() As Boolean Implements ILogWriterProvider.Enabled
            Get
                Return _Enabled
            End Get
            Set(Value As Boolean)
                _Enabled = Value
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

#End Region

#Region " Public "

#Region " Overloads "

        ''' <summary>
        ''' Scrittura del log per la testata specificata.
        ''' </summary>
        ''' <param name="testataLog"></param>
        ''' <param name="forceGroup"></param>
        ''' <param name="connectionString"></param>
        ''' <param name="provider"></param>
        ''' <remarks></remarks>
        Public Overloads Sub WriteLog(testataLog As Testata, forceGroup As Boolean, connectionString As String, provider As String) Implements ILogWriterProvider.WriteLog

            If _Enabled AndAlso Not testataLog Is Nothing Then

                Dim sb As New System.Text.StringBuilder()

                sb.AppendFormat("{0}---------------------------{0}", Environment.NewLine)
                If testataLog.Intestazione Then AppendIntestazione(sb, testataLog)
                AppendLog(sb, testataLog)
                sb.AppendFormat("---------------------------{0}", Environment.NewLine)

                PersistToFile(sb.ToString(), AbsoluteLogFileName(testataLog))

            End If

        End Sub

        ''' <summary>
        ''' Scrittura del log per le testate specificate.
        ''' </summary>
        ''' <param name="testateLog"></param>
        ''' <param name="forceGroup"></param>
        ''' <param name="connectionString"></param>
        ''' <param name="provider"></param>
        ''' <remarks></remarks>
        Public Overloads Sub WriteLog(testateLog() As Testata, forceGroup As Boolean, connectionString As String, provider As String) Implements ILogWriterProvider.WriteLog

            If _Enabled AndAlso Not testateLog Is Nothing AndAlso testateLog.Length > 0 Then

                Dim sb As New System.Text.StringBuilder()

                sb.AppendFormat("{0}---------------------------{0}", Environment.NewLine)

                ' Per scrivere l'intestazione di un gruppo prendo le proprietà della prima testata
                If Not testateLog(0) Is Nothing AndAlso testateLog(0).Intestazione Then
                    AppendIntestazione(sb, testateLog(0))
                End If

                For i As Integer = 0 To testateLog.Length - 1
                    AppendLog(sb, testateLog(i))
                Next

                sb.AppendFormat("---------------------------{0}", Environment.NewLine)

                PersistToFile(sb.ToString, AbsoluteLogFileName(testateLog(0)))

            End If

        End Sub

#End Region

        ''' <summary>
        ''' Criticità
        ''' </summary>
        ''' <param name="priorita"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HandleCriticita(priorita As Integer) As Boolean Implements ILogWriterProvider.HandleCriticita

            Return Array.IndexOf(_FiltroCriticitaArray, priorita) > -1

        End Function

        ''' <summary>
        ''' Aggiunge all'oggetto StringBuilder i dati della testata specificata.
        ''' </summary>
        ''' <param name="sb"></param>
        ''' <param name="testataLog"></param>
        ''' <remarks></remarks>
        Public Sub AppendLog(ByRef sb As System.Text.StringBuilder, testataLog As Testata)

            sb.AppendFormat("Argomento: {0}{1}", testataLog.ArgomentoGenerico.Codice, Environment.NewLine)
            sb.AppendFormat("Data Operazione: {0}{1}", testataLog.DataOperazione, Environment.NewLine)
            sb.AppendFormat("Operazione: {0}{1}", testataLog.OperazioneBase(), Environment.NewLine)
            sb.AppendFormat("Automatico: {0}{1}", IIf(testataLog.Automatico, "S", "N"), Environment.NewLine)
            sb.AppendFormat("Paziente: {0}{1}", testataLog.Paziente, Environment.NewLine)
            sb.AppendFormat("Maschera: {0}{1}", testataLog.Maschera, Environment.NewLine)
            sb.AppendFormat("Stack: {0}{1}", testataLog.Stack, Environment.NewLine)

            ' Dati
            Dim campoLog As Campo

            For i As Integer = 0 To testataLog.Records.Count - 1
                For j As Integer = 0 To testataLog.Records(i).Campi.Count - 1
                    campoLog = testataLog.Records(i).Campi(j)
                    sb.AppendFormat("{0}: {1} -> {2}{3}", campoLog.CampoDB, campoLog.ValoreVecchio, campoLog.ValoreNuovo, Environment.NewLine)
                Next
            Next

        End Sub

#End Region

#Region " Private "

        ''' <summary>
        ''' Nome del log, creato in base all'argomento e alla data di creazione
        ''' </summary>
        ''' <param name="testataLog"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AbsoluteLogFileName(testataLog As Testata) As String

            Dim fileName As String = "LogOnVac"

            If Not testataLog Is Nothing AndAlso Not testataLog.ArgomentoGenerico Is Nothing AndAlso testataLog.ArgomentoGenerico.Codice <> String.Empty Then
                fileName = testataLog.ArgomentoGenerico.Codice
            End If

            ' Ogni giorno un file di log diverso
            fileName = String.Format("{0}_{1:yyyyMMdd}", fileName, Date.Now)

            Return String.Format(AbsoluteFileName, fileName)

        End Function

        ''' <summary>
        ''' Crea l'intestazione fissa del log
        ''' </summary>
        ''' <param name="sb"></param>
        ''' <param name="test"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AppendIntestazione(ByRef sb As System.Text.StringBuilder, testataLog As Testata) As String

            sb.AppendFormat("{0}---------------------------{0}", Environment.NewLine)
            sb.AppendFormat("Data operazione: {0:dd/MM/yyyy HH.mm.ss}{1}", Date.Now, Environment.NewLine)
            sb.AppendFormat("Utente Corrente: {0}{1}", testataLog.Utente.ToString(), Environment.NewLine)
            sb.AppendFormat("Computer: {0}{1}", testataLog.ComputerName, Environment.NewLine)
            sb.AppendFormat("Consultorio Corrente: {0}{1}", testataLog.Consultorio, Environment.NewLine)
            sb.AppendFormat("---------------------------{0}", Environment.NewLine)

            Return sb.ToString()

        End Function

        ''' <summary>
        ''' Esegue la scrittura su file. Restituisce true se tutto ok, altrimenti va in eccezione.
        ''' </summary>
        ''' <param name="strToWrite"></param>
        ''' <param name="absoluteFileName"></param>
        Private Function PersistToFile(strToWrite As String, absoluteFileName As String) As Boolean

            Dim fs As IO.FileStream = Nothing
            Dim sw As IO.StreamWriter = Nothing

            Try
                fs = New System.IO.FileStream(absoluteFileName, IO.FileMode.Append)
                sw = New IO.StreamWriter(fs)
                sw.Write(strToWrite)
                sw.Flush()

                ' Lascio che l'eccezione risalga
            Finally
                If Not sw Is Nothing Then sw.Close()
                If Not fs Is Nothing Then fs.Close()
            End Try

            Return True

        End Function

#End Region

    End Class

End Namespace