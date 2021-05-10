Public Class EventLogHelper

#Region " Scrittura di un'eccezione "

    ''' <summary>
    ''' Scrittura nell'EventLog di sistema di una entry relativa all'eccezione specificata, di tipo Warning.
    ''' </summary>
    ''' <param name="ex"></param>
    ''' <param name="appName"></param>
    ''' <remarks>Se non è presente la source dell'applicativo su registro, la aggiunge. E' necessario che l'utente del pool di OnVac
    ''' possieda i diritti di scrittura nel registro di sistema, altrimenti l'operazione fallisce.
    ''' </remarks>
    Public Shared Sub EventLogWrite(ex As Exception, appName As String)

        EventLogWriteException(ex, String.Empty, EventLogEntryType.Warning, appName)

    End Sub

    ''' <summary>
    ''' Scrittura nell'EventLog di sistema di una entry relativa all'eccezione specificata, del tipo specificato.
    ''' </summary>
    ''' <param name="ex"></param>
    ''' <param name="entryType"></param>
    ''' <param name="appName"></param>
    ''' <remarks>Se non è presente la source dell'applicativo su registro, la aggiunge. E' necessario che l'utente del pool di OnVac
    ''' possieda i diritti di scrittura nel registro di sistema, altrimenti l'operazione fallisce.
    ''' </remarks>
    Public Shared Sub EventLogWrite(ex As Exception, entryType As EventLogEntryType, appName As String)

        EventLogWriteException(ex, String.Empty, entryType, appName)

    End Sub

    ''' <summary>
    ''' Scrittura nell'EventLog di sistema di una entry relativa all'eccezione specificata, di tipo Warning.
    ''' </summary>
    ''' <param name="ex"></param>
    ''' <param name="errorMessage"></param>
    ''' <param name="appName"></param>
    ''' <remarks></remarks>
    Public Shared Sub EventLogWrite(ex As Exception, errorMessage As String, appName As String)

        EventLogWriteException(ex, errorMessage, EventLogEntryType.Warning, appName)

    End Sub

    ''' <summary>
    ''' Scrittura nell'EventLog di sistema di una entry relativa all'eccezione specificata, del tipo specificato.
    ''' All'inizio della entry è presente il messaggio di errore specificato.
    ''' </summary>
    ''' <param name="ex"></param>
    ''' <param name="errorMessage"></param>
    ''' <param name="entryType"></param>
    ''' <param name="appName"></param>
    ''' <remarks>Se non è presente la source dell'applicativo su registro, la aggiunge. E' necessario che l'utente del pool di OnVac
    ''' possieda i diritti di scrittura nel registro di sistema, altrimenti l'operazione fallisce.
    ''' </remarks>
    Public Shared Sub EventLogWrite(ex As Exception, errorMessage As String, entryType As EventLogEntryType, appName As String)

        EventLogWriteException(ex, errorMessage, entryType, appName)

    End Sub

#End Region

#Region " Scrittura di un messaggio generico "

    ''' <summary>
    ''' Scrittura nell'EventLog di sistema di una entry di tipo Error
    ''' </summary>
    ''' <param name="message"></param>
    ''' <param name="appName"></param>
    ''' <remarks>Se non è presente la source dell'applicativo su registro, la aggiunge. E' necessario che l'utente del pool di OnVac
    ''' possieda i diritti di scrittura nel registro di sistema, altrimenti l'operazione fallisce.
    ''' </remarks>
    Public Shared Sub EventLogWrite(message As String, appName As String)

        WriteEntry(message, EventLogEntryType.Warning, appName)

    End Sub

    ''' <summary>
    ''' Scrittura nell'EventLog di sistema di una entry del tipo specificato.
    ''' </summary>
    ''' <param name="message"></param>
    ''' <param name="entryType"></param>
    ''' <param name="appName"></param>
    ''' <remarks>Se non è presente la source dell'applicativo su registro, la aggiunge. E' necessario che l'utente del pool di OnVac
    ''' possieda i diritti di scrittura nel registro di sistema, altrimenti l'operazione fallisce.
    ''' </remarks>
    Public Shared Sub EventLogWrite(message As String, entryType As EventLogEntryType, appName As String)

        WriteEntry(message, entryType, appName)

    End Sub

#End Region

#Region " Private "

    ''' <summary>
    ''' Scrittura di una entry relativa all'eccezione, preceduta dal messaggio di errore specificato.
    ''' </summary>
    ''' <param name="ex"></param>
    ''' <param name="errorMessage"></param>
    ''' <param name="entryType"></param>
    ''' <param name="appName"></param>
    ''' <remarks></remarks>
    Private Shared Sub EventLogWriteException(ex As Exception, errorMessage As String, entryType As EventLogEntryType, appName As String)

        If ex Is Nothing Then Exit Sub

        ' --- Messaggio da scrivere nell'EventLog --- '
        Dim msg As New System.Text.StringBuilder()

        msg.AppendFormat("{0}{1}", errorMessage, Environment.NewLine)
        msg.AppendLine("------------------")
        msg.AppendFormat("Generata eccezione di tipo: {0}.{1}", ex.GetType().Name, Environment.NewLine)

        ' Exception
        msg.AppendLine("------------------")
        msg.AppendFormat("Exception Message: {0}{2}StackTrace: {1}{2}", ex.Message, ex.StackTrace, Environment.NewLine)
        msg.AppendLine("------------------")

        Dim innerException As Exception = ex.InnerException

        ' Inner Exception
        While Not innerException Is Nothing

            If Not ex.InnerException Is Nothing Then
                msg.AppendFormat("InnerException Message: {0}{2}StackTrace: {1}{2}", ex.InnerException.Message, ex.InnerException.StackTrace, Environment.NewLine)
                msg.AppendLine("------------------")
            End If

            innerException = innerException.InnerException

        End While

        ' --- Scrittura nell'EventLog --- '
        WriteEntry(msg.ToString(), entryType, appName)

    End Sub

    ''' <summary>
    ''' Scrittura nell'EventLog
    ''' </summary>
    ''' <param name="msg"></param>
    ''' <param name="entryType"></param>
    ''' <param name="appName"></param>
    ''' <remarks></remarks>
    <Global.System.Diagnostics.DebuggerNonUserCodeAttribute()>
    Private Shared Sub WriteEntry(msg As String, entryType As EventLogEntryType, appName As String)

        Try
            If Not System.Diagnostics.EventLog.SourceExists(appName) Then
                System.Diagnostics.EventLog.CreateEventSource(appName, "Application")
            End If

            Dim eventLog As New System.Diagnostics.EventLog()
            eventLog.Source = appName
            eventLog.WriteEntry(msg, entryType)

        Catch innerEx As Exception
            ' Probabile event log pieno
        End Try

    End Sub

#End Region

End Class

