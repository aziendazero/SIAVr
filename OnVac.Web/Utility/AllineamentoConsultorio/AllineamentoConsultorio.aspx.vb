Imports Onit.Shared.Manager.OnitProfile

Partial Class AllineamentoConsultorio
    Inherits Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub
    Protected WithEvents staStatiAnagrafici As OnVac.Common.Controls.StatiAnagrafici
    Protected WithEvents uscCnvStatiAnagrafici As OnVac.Common.Controls.StatiAnagrafici

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(sender As System.Object, e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Private "

    Private Const DGR_PAGE = 100

#End Region

#Region " Page Events "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            ' --- Filtro Consultorio --- '
            Me.omlConsultorio.Codice = OnVacUtility.Variabili.CNS.Codice
            Me.omlConsultorio.RefreshDataBind()

            ' --- Aggiornamento consultorio per pazienti deceduti --- '
            Select Case Me.Settings.AUTOAGGIORNACNSDECEDUTI
                Case Onit.OnAssistnet.OnVac.Settings.Settings.FunctionalityStatus.DISABLED
                    Me.chkElaboraDeceduti.Checked = False
                    Me.chkElaboraDeceduti.Enabled = False
                Case Onit.OnAssistnet.OnVac.Settings.Settings.FunctionalityStatus.ENABLED
                    Me.chkElaboraDeceduti.Checked = False
                    Me.chkElaboraDeceduti.Enabled = True
                Case Onit.OnAssistnet.OnVac.Settings.Settings.FunctionalityStatus.ENABLED_SELECTED
                    Me.chkElaboraDeceduti.Checked = True
                    Me.chkElaboraDeceduti.Enabled = True
            End Select

            ' --- Aggiornamento consultorio adulti --- '
            Select Case Me.Settings.AUTOAGGIORNACNSADU
                Case Onit.OnAssistnet.OnVac.Settings.Settings.FunctionalityStatus.DISABLED
                    Me.chkElaboraCambioConsultorio.Checked = False
                    Me.chkElaboraCambioConsultorio.Enabled = False
                Case Onit.OnAssistnet.OnVac.Settings.Settings.FunctionalityStatus.ENABLED
                    Me.chkElaboraCambioConsultorio.Checked = False
                    Me.chkElaboraCambioConsultorio.Enabled = True
                Case Onit.OnAssistnet.OnVac.Settings.Settings.FunctionalityStatus.ENABLED_SELECTED
                    Me.chkElaboraCambioConsultorio.Checked = True
                    Me.chkElaboraCambioConsultorio.Enabled = True
            End Select

            For i As Integer = 0 To Me.ddlCriterioAssociazione.Items.Count - 1
                If Me.ddlCriterioAssociazione.Items(i).Value = Me.Settings.AUTOAGGIORNACNSADU_CRITERIOSELEZIONE Then
                    Me.ddlCriterioAssociazione.SelectedIndex = i
                    Exit For
                End If
            Next

            ' --- Calcolo automatico convocazioni --- '
            If Me.Settings.AUTOCNVAPP Then

                ' Abilito e seleziono il check per il calcolo automatico delle cnv
                Me.chkElaboraConvocazioni.Checked = True
                Me.chkElaboraConvocazioni.Enabled = True

                ' Se il parametro che considera i filtri sulla data di nascita è true, abilito i campi per filtrare le date di nascita.
                ' Altrimenti, i campi di filtro sulla data di nascita sono disabilitati perchè non vengono presi in considerazione dalla query.
                If Me.Settings.CNVAUTOFILTRAETA Then
                    Me.dpkCnvDaNascita.Enabled = True
                    Me.dpkCnvANascita.Enabled = True
                    Me.dpkCnvDaNascita.CssClassLayout = "Textbox_stringa"
                    Me.dpkCnvANascita.CssClassLayout = "Textbox_stringa"
                Else
                    Me.dpkCnvDaNascita.Enabled = False
                    Me.dpkCnvANascita.Enabled = False
                    Me.dpkCnvDaNascita.CssClassLayout = "Textbox_stringa_disabilitato"
                    Me.dpkCnvANascita.CssClassLayout = "Textbox_stringa_disabilitato"
                End If

            Else
                ' Disabilito e non seleziono il check per il calcolo automatico delle cnv
                Me.chkElaboraConvocazioni.Checked = False
                Me.chkElaboraConvocazioni.Enabled = False
            End If

        End If

    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub ToolBar_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case be.Button.Key

            Case "btnStart"

                Dim jobId As Integer

                Dim returnedMessage As New System.Text.StringBuilder()
                Dim hasError As Boolean = False
                Dim parArray As ArrayList = Nothing

                ' --- Deceduti --- '
                If chkElaboraDeceduti.Checked Then

                    parArray = New ArrayList()

                    jobId = StartProcedure(3, OnVacContext.UserId.ToString(), parArray)

                    If jobId <> Nothing AndAlso jobId > 0 Then
                        returnedMessage.Append("Processo di elaborazione dei deceduti avviato \n")
                    Else
                        returnedMessage.Append("Problemi con il servizio. Processo di elaborazione dei deceduti NON avviato \n")
                    End If

                End If

                ' --- Cambio Consultorio --- '
                If Me.chkElaboraCambioConsultorio.Checked Then

                    parArray = New ArrayList()
                    Dim par As wsBatch.PortParameter = Nothing

                    par = New wsBatch.PortParameter()
                    par.Name = "Stato Anagrafico"
                    par.Value = Me.staStatiAnagrafici.GetSelectedDescriptions()
                    parArray.Add(par)

                    par = New wsBatch.PortParameter()
                    par.Name = "StatoAnagraficoCodici"
                    par.Value = Me.staStatiAnagrafici.GetSelectedValuesForQuery()
                    parArray.Add(par)

                    ' Parametro: aggiornamento consultorio in base 
                    par = New wsBatch.PortParameter()
                    par.Name = "Aggiorna anche pazienti con appuntamento"
                    par.Value = Me.ddlAggiornaAncheCnvConApp.SelectedValue
                    parArray.Add(par)

                    ' Parametro: aggiornamento consultorio in base al criterio
                    If Me.ddlCriterioAssociazione.SelectedValue <> String.Empty Then
                        par = New wsBatch.PortParameter()
                        par.Name = "Criterio"
                        par.Value = Me.ddlCriterioAssociazione.SelectedValue
                        parArray.Add(par)
                    End If

                    If Me.ddlSesso.SelectedValue <> String.Empty Then
                        par = New wsBatch.PortParameter()
                        par.Name = "Sesso"
                        par.Value = ddlSesso.SelectedValue
                        parArray.Add(par)
                    End If

                    If Me.odpDaDataNascita.Data <> Date.MinValue Then
                        ' Parametro: aggiornamento consultorio in base alla data di nascita
                        par = New wsBatch.PortParameter()
                        par.Name = "Da data"
                        par.Value = Me.odpDaDataNascita.Data
                        parArray.Add(par)
                    End If

                    If Me.odpADataNascita.Data <> Date.MinValue Then
                        ' Parametro: aggiornamento consultorio in base alla data di nascita
                        par = New wsBatch.PortParameter()
                        par.Name = "A data"
                        par.Value = Me.odpADataNascita.Data
                        parArray.Add(par)
                    End If

                    jobId = StartProcedure(4, OnVacContext.UserId.ToString(), parArray)

                    If jobId <> Nothing AndAlso jobId > 0 Then
                        returnedMessage.Append("Processo di cambio centro avviato \n")
                    Else
                        returnedMessage.Append("Problemi con il servizio. Processo di cambio centro NON avviato \n")
                    End If

                End If

                ' --- Calcolo Convocazioni --- '
                If Me.chkElaboraConvocazioni.Checked Then

                    ' Parametri
                    parArray = New ArrayList()
                    Dim par As wsBatch.PortParameter = Nothing

                    ' Parametro: Stati Anagrafici
                    ' Passo il parametro come stringa di codici separati da virgole, senza apici
                    par = New wsBatch.PortParameter()
                    par.Name = "CodStatiAnagrafici"
                    par.Value = Me.uscCnvStatiAnagrafici.GetSelectedValues()
                    parArray.Add(par)


                    ' Parametro: Descrizione Stati Anagrafici (solo per il report)
                    Dim descrizioneStatiAnagrafici As String = Me.uscCnvStatiAnagrafici.GetSelectedDescriptions()

                    If descrizioneStatiAnagrafici <> String.Empty Then
                        par = New wsBatch.PortParameter()
                        par.Name = "Stati Anagrafici"
                        par.Value = descrizioneStatiAnagrafici
                        parArray.Add(par)
                    End If

                    ' Parametro: Data Nascita Da
                    If Me.dpkCnvDaNascita.Data > Date.MinValue Then
                        par = New wsBatch.PortParameter()
                        par.Name = "Da data"
                        par.Value = Me.dpkCnvDaNascita.Data
                        parArray.Add(par)
                    End If

                    ' Parametro: Data Nascita A
                    If Me.dpkCnvANascita.Data > Date.MinValue Then
                        par = New wsBatch.PortParameter()
                        par.Name = "A data"
                        par.Value = Me.dpkCnvANascita.Data
                        parArray.Add(par)
                    End If

                    ' Parametro: Sesso
                    If Me.ddlCnvSesso.SelectedValue <> String.Empty Then
                        par = New wsBatch.PortParameter()
                        par.Name = "Sesso"
                        par.Value = Me.ddlCnvSesso.SelectedValue
                        parArray.Add(par)
                    End If

                    ' Parametro: Malattia associata ai pazienti
                    If Me.omlCnvMalattia.Codice <> String.Empty And Me.omlCnvMalattia.Descrizione <> String.Empty Then
                        par = New wsBatch.PortParameter()
                        par.Name = "Malattia"
                        par.Value = Me.omlCnvMalattia.Codice
                        parArray.Add(par)
                    End If

                    ' Parametro: Categoria di rischio dei pazienti
                    If Me.omlCnvCategorieRischio.Codice <> String.Empty And Me.omlCnvCategorieRischio.Descrizione <> String.Empty Then
                        par = New wsBatch.PortParameter()
                        par.Name = "Categoria rischio"
                        par.Value = Me.omlCnvCategorieRischio.Codice
                        parArray.Add(par)
                    End If

                    ' Attivazione della procedura
                    jobId = StartProcedure(5, OnVacContext.UserId.ToString(), parArray)

                    If jobId <> Nothing AndAlso jobId > 0 Then
                        returnedMessage.Append("Processo di calcolo automatico convocazioni avviato \n")
                    Else
                        returnedMessage.Append("Problemi con il servizio. Processo di calcolo automatico convocazioni NON avviato \n")
                    End If

                End If

                ' --- Messaggio all'utente --- '
                OnitLayout31.InsertRoutineJS(String.Format("alert('{0}');", returnedMessage.ToString()))

        End Select

    End Sub

#End Region

#Region " Private "

    Private Function StartProcedure(procId As Integer, idUtente As String, parArray As ArrayList) As Integer

        ' filtro comune a tutte le procedure batch
        Dim portParameter As New wsBatch.PortParameter()
        portParameter.Name = "Consultorio"
        portParameter.Value = omlConsultorio.Codice

        parArray.Add(portParameter)

        portParameter = New wsBatch.PortParameter()
        portParameter.Name = "CodiceUslCorrente"
        portParameter.Value = OnVacContext.CodiceUslCorrente

        parArray.Add(portParameter)

        ' Info per connessione a database
        Dim portConnection(0) As wsBatch.PortConnection
        portConnection(0) = New wsBatch.PortConnection()
        portConnection(0).Name = OnVacContext.AppId

        Dim jobInputData As New wsBatch.JobInputData()
        jobInputData.ProcedureId = procId
        jobInputData.Paused = True

        jobInputData.Input = New wsBatch.InputPort()
        jobInputData.Input.PortConnections = portConnection
        jobInputData.Input.PortParameters = parArray.ToArray(GetType(wsBatch.PortParameter))
        jobInputData.Input.ApplicationId = OnVacContext.AppId
        jobInputData.Input.UserId = idUtente
        jobInputData.Input.AziendaCodice = OnVacContext.Azienda

        Dim jobId As Integer
        Dim worker As New wsBatch.wsBatch()

        Try
            ' Richiesta del servizio al web service
            jobId = worker.CreateJob(jobInputData)

            If jobId <> Nothing AndAlso jobId > 0 Then
                ' Richiesta del servizio al web service
                worker.ResumeJob(jobId)
            End If

        Catch ex As Exception
            OnVac.Common.Utility.EventLogHelper.EventLogWrite(ex, String.Format("Errore durante l'avvio della procedura batch (id procedura: {0})", procId), OnVacContext.AppId)
            jobId = Nothing
        Finally
            If Not worker Is Nothing Then worker.Dispose()
        End Try

        Return jobId

    End Function

#End Region

End Class
