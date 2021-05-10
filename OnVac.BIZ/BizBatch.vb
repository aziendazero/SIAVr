Imports System.Collections.Generic
Imports Onit.OnBatch.WebService.Proxy
Imports Onit.OnAssistnet.OnVac.DAL


Public Class BizBatch

#Region "Private var"

    Private GenericProvider As DbGenericProvider

    Private _jobId As Integer
    Private _procId As Integer
    Private _uteId As Integer
    Private parameters As New ArrayList()
    Private selectionParameters As New Hashtable()
    Private patients As New ArrayList()

#End Region

#Region "Public Property"

    Protected Property _GenericProvider() As DbGenericProvider
        Get
            Return Me.GenericProvider
        End Get
        Set(Value As DbGenericProvider)
            Me.GenericProvider = Value
        End Set
    End Property

    Public Property jobId() As Integer
        Get
            Return _jobId
        End Get
        Set(Value As Integer)
            _jobId = Value
        End Set
    End Property

    Public Property procId() As Integer
        Get
            Return _procId
        End Get
        Set(Value As Integer)
            _procId = Value
        End Set
    End Property

    Public Property uteId() As Integer
        Get
            Return _uteId
        End Get
        Set(Value As Integer)
            _uteId = Value
        End Set
    End Property

#End Region

#Region " Constructors "

    Public Sub New()

    End Sub

    Public Sub New(ByRef genericprovider As DbGenericProvider)

        Me._GenericProvider = genericprovider

    End Sub

#End Region

#Region " Public Methods "

    Public Class GetProceduresResult
        Public Success As Boolean
        Public Message As String
        Public DataTableProcedure As DataTable
    End Class

    ''' <summary>
    ''' Restituisce un dataset con le procedure presenti nella t_ana_procedure.
    ''' In caso di errore scrive sul log.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetProcedures() As GetProceduresResult

        Dim result As New GetProceduresResult()

        Try
            result.DataTableProcedure = Me.GenericProvider.Procedure.GetProcedure()
            result.Success = True
            result.Message = String.Empty
        Catch ex As Exception
            result.DataTableProcedure = Nothing
            result.Success = False
            result.Message = Me.GenericProvider.Procedure.GetErrorMsg()
        End Try

        Return result

    End Function

    ''' <summary>
    ''' Restituisce la descrizione dell'utente in base all'id specificato
    ''' </summary>
    ''' <param name="uteId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUtenteDescrizione(uteId As Integer) As String

        Return Me.GenericProvider.Utenti.GetDescrizioneUtente(uteId)

    End Function

    ''' <summary>
    ''' Restituisce il codice dell'utente in base all'id specificato
    ''' </summary>
    ''' <param name="uteId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUtenteCodice(uteId As Integer) As String

        Return Me.GenericProvider.Utenti.GetCodiceUtente(uteId)

    End Function

    Public Sub setProvider(ByRef genericprovider As DbGenericProvider)

        Me._GenericProvider = genericprovider

    End Sub

    Public Function fillTable() As Boolean

        Dim strParameters As String = GetParameters()

        Return Me.GenericProvider.Procedure.FillTable(procId, jobId, patients, strParameters, uteId)

    End Function

    Public Function storeParameters() As Boolean

        Return Me.GenericProvider.Procedure.StoreParameters(jobId, selectionParameters)

    End Function

    Public Sub addParameter(param As String)

        parameters.Add(param)

    End Sub

    Public Sub addSelectionParameters(parameterName As String, parameterValue As String)

        selectionParameters.Add(parameterName, parameterValue)

    End Sub

    Public Sub addPatient(codice As String)

        patients.Add(codice)

    End Sub

    Public Sub addPatient(codice As Integer)

        patients.Add(codice)

    End Sub

#Region " Start Batch Procedures "

    Public Class StartBatchProcedureCommand

        Public ProcedureId As Integer
        Public ProcedureDescription As String
        Public ListAppIdConnections As List(Of String)
        Public ListParameters As List(Of BatchProcedureParameter)
        Public ListReportParameters As List(Of BatchProcedureParameter)
        Public StartingAppId As String
        Public StartingCodiceAzienda As String
        Public StartingUserId As Long
        Public PazientiConvocazioniDaElaborare As List(Of Entities.ConvocazionePK)
        Public DataSchedulazione As Date?

        Public Class BatchProcedureParameter

            Public Name As String
            Public Value As String

            Public Sub New()
            End Sub

            Public Sub New(name As String, value As String)
                Me.Name = name
                Me.Value = value
            End Sub

            Public Sub New(name As String, value As Boolean)
                Me.Name = name
                Me.Value = IIf(value, Boolean.TrueString, Boolean.FalseString)
            End Sub

            Public Sub New(name As String, value As Long)
                Me.Name = name
                Me.Value = value.ToString()
            End Sub

            Public Sub New(name As String, value As DateTime)
                Me.Name = name
                Me.Value = value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
            End Sub

        End Class

        Public Sub New()
            ListAppIdConnections = New List(Of String)()
            ListParameters = New List(Of BatchProcedureParameter)()
            ListReportParameters = New List(Of BatchProcedureParameter)()
            PazientiConvocazioniDaElaborare = New List(Of Entities.ConvocazionePK)()
        End Sub

    End Class

    ''' <summary>
    ''' Avvio procedura batch
    ''' </summary>
    ''' <param name="command"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function StartBatchProcedure(command As StartBatchProcedureCommand) As BizGenericResult

        Dim result As BizGenericResult = New BizGenericResult()

        Using worker As New wsBatch.wsBatch()

            Dim userId As Integer = Convert.ToInt32(command.StartingUserId)

            Dim nomeProcedura As String = command.ProcedureDescription
            If String.IsNullOrWhiteSpace(nomeProcedura) Then nomeProcedura = "Processo"

            result.Success = True
            result.Message = String.Format("{0} avviato.", nomeProcedura)

            ' Dati del job
            Dim jobData As New wsBatch.JobInputData()
            jobData.ProcedureId = command.ProcedureId
            jobData.Paused = True
            jobData.TotalItems = 0

            jobData.Input = New wsBatch.InputPort()
            jobData.Input.ApplicationId = command.StartingAppId
            jobData.Input.AziendaCodice = command.StartingCodiceAzienda
            jobData.Input.UserId = userId

            ' Connessioni a db
            If Not command.ListAppIdConnections Is Nothing AndAlso command.ListAppIdConnections.Count > 0 Then

                Dim listConnection As New List(Of wsBatch.PortConnection)()

                For Each appIdConnection As String In command.ListAppIdConnections

                    listConnection.Add(New wsBatch.PortConnection() With
                        {
                            .Name = appIdConnection
                        })

                Next

                jobData.Input.PortConnections = listConnection.ToArray()

            End If

            ' Parametri processo batch
            If Not command.ListParameters Is Nothing AndAlso command.ListParameters.Count > 0 Then

                Dim parameterList As New List(Of wsBatch.PortParameter)()

                For Each commandParameter As StartBatchProcedureCommand.BatchProcedureParameter In command.ListParameters

                    parameterList.Add(New wsBatch.PortParameter() With
                        {
                            .Name = commandParameter.Name,
                            .Value = commandParameter.Value
                        })
                Next

                jobData.Input.PortParameters = parameterList.ToArray()

            End If

            ' TODO [App Auto]: command.DataSchedulazione => da passare a WsBatch!!!

            ' Creazione del job
            Dim jobId As Integer? = 0

            Try
                jobId = worker.CreateJob(jobData)
            Catch ex As Exception
                Common.Utility.EventLogHelper.EventLogWrite(ex, command.StartingAppId)
                jobId = Nothing
            End Try

            If jobId.HasValue AndAlso jobId.Value > 0 Then

                Try
                    ' Inserimento in t_paz_elaborazioni
                    If Not command.PazientiConvocazioniDaElaborare Is Nothing AndAlso command.PazientiConvocazioniDaElaborare.Count > 0 Then
                        Me.GenericProvider.Procedure.InsertPazientiElaborazioni(command.PazientiConvocazioniDaElaborare, command.ProcedureId, jobId, userId)
                    End If

                    ' Inserimento in t_prc_parametri (parametri che verranno visualizzati nel report)
                    If Not command.ListReportParameters Is Nothing AndAlso command.ListReportParameters.Count > 0 Then

                        Dim params As New List(Of KeyValuePair(Of String, String))()

                        For Each reportParameter As StartBatchProcedureCommand.BatchProcedureParameter In command.ListReportParameters
                            params.Add(New KeyValuePair(Of String, String)(reportParameter.Name, reportParameter.Value))
                        Next

                        Me.GenericProvider.Procedure.InsertParametriReport(params, jobId)

                    End If

                Catch ex As Exception
                    Common.Utility.EventLogHelper.EventLogWrite(ex, command.StartingUserId)
                    result.Message = String.Format("Problemi di comunicazione con il database. {0} NON avviato.", nomeProcedura)
                    result.Success = False
                End Try

            Else
                result.Message = String.Format("Problemi di comunicazione con il servizio. {0} NON avviato.", nomeProcedura)
                result.Success = False
            End If

            If result.Success Then
                worker.ResumeJob(jobId)
            End If

        End Using

        Return result

    End Function

    Public Class UpdatePazienteElaborazioneCommand
        Public Progressivo As Long
        Public CodiceErrore As Integer
        Public DescrizioneErrore As String
        Public StatoElaborazione As String
    End Class

    ''' <summary>
    ''' Aggiornamento dati t_paz_elaborazioni
    ''' </summary>
    ''' <param name="command"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdatePazienteElaborazione(command As UpdatePazienteElaborazioneCommand) As Integer

        Dim codiceErrore As Integer? = Nothing

        If command.CodiceErrore > -1 Then
            codiceErrore = command.CodiceErrore
        End If

        Return Me.GenericProvider.Procedure.UpdatePazienteElaborazione(command.Progressivo, codiceErrore, command.DescrizioneErrore, DateTime.Now, command.StatoElaborazione)

    End Function

#Region " EventArgs per refresh totale e refresh parziali "

    Public Class RefreshTotaleElementiDaElaborareEventArgs
        Inherits EventArgs

        Private _TotaleElementi As Integer
        Public ReadOnly Property TotaleElementi As Integer
            Get
                Return _TotaleElementi
            End Get
        End Property

        Public Sub New(totaleElementiDaElaborare As Integer)
            Me._TotaleElementi = totaleElementiDaElaborare
        End Sub

    End Class

    Public Class RefreshParzialeElementiElaboratiEventArgs
        Inherits EventArgs

        Private _NumeroElementiElaborati As Integer
        Public ReadOnly Property NumeroElementiElaborati As Integer
            Get
                Return _NumeroElementiElaborati
            End Get
        End Property

        Private _NumeroErrori As Integer
        Public ReadOnly Property NumeroErrori As Integer
            Get
                Return _NumeroErrori
            End Get
        End Property

        Public Sub New(numeroElementiElaborati As Integer, numeroErrori As Integer)
            Me._NumeroElementiElaborati = numeroElementiElaborati
            Me._NumeroErrori = numeroErrori
        End Sub

    End Class

#End Region

#End Region

#End Region

#Region " Private Methods "

    Private Function GetParameters() As String

        Dim strb As New System.Text.StringBuilder()

        For i As Integer = 0 To parameters.Count - 1
            strb.AppendFormat("{0}|", parameters(i))
        Next

        Return strb.ToString()

    End Function

#End Region

End Class
