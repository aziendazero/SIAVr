Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.DAL


Partial Class ModConv
    Inherits Common.UserControlFinestraModalePageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(sender As System.Object, e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Events "

    Public Event Salvato(sender As Object, e As EventArgs)

#End Region

#Region " Private "

    Private Const KEY_MODIFICA_CNV As String = "ModificaConvocazione"
    Private Const KEY_SPOSTA_CNV As String = "SpostaConvocazione"
    Private Const KEY_UNISCI_CNV As String = "UnisciConvocazione"
    Private Const KEY_MANTIENI_APP As String = "MantieniAppuntamento"

#End Region

#Region " Properties "

    'La data di convocazione della convocazione da cambiare
    Public Property DataCnv() As Date
        Get
            Return ViewState("DataCnv")
        End Get
        Set(Value As Date)
            ViewState("DataCnv") = Value
        End Set
    End Property

    ' Flag che indica se la pagina che contiene lo user control è in modalità centrale
    Public Property IsGestioneCentrale As Boolean
        Get
            Return ViewState("IsGestioneCentrale")
        End Get
        Set(Value As Boolean)
            ViewState("IsGestioneCentrale") = Value
        End Set
    End Property

#End Region

#Region " Overrides "

    Public Overrides Sub LoadModale()
        '--
        Dim drConvocazione As DataRow = Me.LoadConvocazione(Me.DataCnv)
        '--
        Me.BindConvocazione(drConvocazione)
        '--
        Me.lblErrore.Text = String.Empty
        '--
        Dim pazienteRitardatario As Boolean
        '--
        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            '--
            Dim dataNascita As DateTime = Date.MinValue
            '--
            Using bizPaziente As New Biz.BizPaziente(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                '--
                dataNascita = bizPaziente.GetDataNascitaPaziente(OnVacUtility.Variabili.PazId, Me.IsGestioneCentrale)
                '--
            End Using
            '--
            If dataNascita = Date.MinValue Then
                Me.odpDataNascita.Text = String.Empty
            Else
                Me.odpDataNascita.Data = dataNascita
            End If
            '--
            Using bizConvocazione As New Biz.BizConvocazione(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)
                '--
                pazienteRitardatario = bizConvocazione.IsRitardatario(OnVacUtility.Variabili.PazId, Me.DataCnv)
                '--
            End Using
            '--
        End Using
        '--
        Me.txtDataConvocazione.Enabled = Not pazienteRitardatario
        Me.txtDataConvocazione.CssClass = IIf(pazienteRitardatario, "textbox_stringa_disabilitato", "textbox_data_obbligatorio")
        '--
        Me.ToolBar.Items.FromKeyButton("btnSalva").Enabled = True
        '--
        Me.txtEtaPaziente.Text = OnVacUtility.ImpostaEtaPazienteConv(Me.DataCnv, Me.IsGestioneCentrale, Me.Settings)
        '--
    End Sub

#End Region

#Region " Events Handles "

    Private Sub ToolBar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btnSalva"
                Me.Salva()

        End Select

    End Sub

    Private Sub OnitLayout31_ConfirmClick(sender As Object, eventArgs As Onit.Controls.PagesLayout.OnitLayout3.ConfirmEventArgs) Handles OnitLayout31.ConfirmClick
        '--
        Select Case eventArgs.Key
            '--
            Case KEY_MODIFICA_CNV
                '--
                If eventArgs.Result Then
                    '--
                    If Me.ModificaConvocazione() Then
                        '--
                        RaiseEvent Salvato(Me, System.EventArgs.Empty)
                        '--
                    End If
                    '--
                End If
                '--
            Case KEY_SPOSTA_CNV
                '--
                If eventArgs.Result Then
                    '--
                    If Me.SpostaConvocazione(Me.txtConsultorio.Codice, False, "Spostamento convocazione") Then
                        '--
                        RaiseEvent Salvato(Me, System.EventArgs.Empty)
                        '--
                    End If
                    '--
                End If
                '--
            Case KEY_UNISCI_CNV
                '--
                If eventArgs.Result Then
                    '--
                    Dim drAltraConvocazioneEsistente As DataRow = Me.LoadConvocazione(Me.txtDataConvocazione.Data)
                    '--
                    Dim dataAppuntamento As DateTime? = Nothing
                    '--
                    If drAltraConvocazioneEsistente.IsNull("CNV_DATA_APPUNTAMENTO") Then
                        If Not String.IsNullOrEmpty(txtDataAppuntamento.Text) Then
                            dataAppuntamento = DateTime.Parse(Me.txtDataAppuntamento.Text + " " + Me.txtOraAppuntamento.Text)
                        End If
                    Else
                        dataAppuntamento = drAltraConvocazioneEsistente("CNV_DATA_APPUNTAMENTO")
                    End If
                    '--
                    If dataAppuntamento.HasValue Then
                        '--
                        Dim msg As String = String.Format("Le convocazioni hanno la data di appuntamento valorizzata:\nunire tutte le convocazioni in una unica, mantenendo l'appuntamento del {0:dd/MM/yyyy HH:mm}?", dataAppuntamento.Value)
                        OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(msg, KEY_MANTIENI_APP, True, True))
                        '--
                    Else
                        '--
                        If Me.SpostaConvocazione(drAltraConvocazioneEsistente("CNV_CNS_CODICE").ToString(), False, "Unione convocazioni (nessun appuntamento)") Then
                            '--
                            RaiseEvent Salvato(Me, System.EventArgs.Empty)
                            '--
                        End If
                        '--
                    End If
                    '--
                End If
                '--
            Case KEY_MANTIENI_APP
                '--
                Dim drAltraConvocazioneEsistente As DataRow = Me.LoadConvocazione(Me.txtDataConvocazione.Data)
                '--
                Dim noteAppuntamento As String = "Unione convocazioni"
                If eventArgs.Result Then
                    noteAppuntamento += " (appuntamento mantenuto)"
                Else
                    noteAppuntamento += " (appuntamento non mantenuto)"
                End If
                If Me.SpostaConvocazione(drAltraConvocazioneEsistente("CNV_CNS_CODICE").ToString(), eventArgs.Result, noteAppuntamento) Then
                    '--
                    RaiseEvent Salvato(Me, System.EventArgs.Empty)
                    '--
                End If
                '--
        End Select
        '--
    End Sub

#End Region

#Region " Private  "

    Private Function LoadConvocazione(dataCnv As DateTime) As DataRow
        '--
        Dim dam As IDAM = OnVacUtility.OpenDam()
        Dim dt As New DataTable()
        '--
        dam.QB.NewQuery()
        dam.QB.AddSelectFields("*")
        dam.QB.AddTables("T_CNV_CONVOCAZIONI")
        dam.QB.AddWhereCondition("CNV_PAZ_CODICE", Comparatori.Uguale, OnVacUtility.Variabili.PazId, DataTypes.Numero)
        dam.QB.AddWhereCondition("CNV_DATA", Comparatori.Uguale, dataCnv, DataTypes.Data)
        '--
        Try
            dam.BuildDataTable(dt)
        Finally
            OnVacUtility.CloseDam(dam)
        End Try
        '--
        If dt.Rows.Count > 0 Then
            Return dt.Rows(0)
        Else
            Return Nothing
        End If
        '--
    End Function

    Private Sub BindConvocazione(drConvocazione As DataRow)
        '--
        Me.txtDurataAppuntamento.Text = drConvocazione("CNV_DURATA_APPUNTAMENTO").ToString()
        '--
        Me.txtConsultorio.Codice = drConvocazione("CNV_CNS_CODICE").ToString()
        Me.txtConsultorio.RefreshDataBind()
        '--
        If Not drConvocazione("CNV_DATA_APPUNTAMENTO") Is DBNull.Value Then
            Dim dataAppuntamento As Date = drConvocazione("CNV_DATA_APPUNTAMENTO")
            Me.txtDataAppuntamento.Text = dataAppuntamento.ToShortDateString()
            Me.txtOraAppuntamento.Text = dataAppuntamento.ToShortTimeString()
        Else
            Me.txtDataAppuntamento.Text = ""
            Me.txtOraAppuntamento.Text = ""
        End If
        '--
        Me.txtDataConvocazione.Text = Me.DataCnv.ToString()
        Me.txtDataInvio.Text = drConvocazione("CNV_DATA_INVIO").ToString()
        '--
        Me.txtTipoAppuntamento.Text = drConvocazione("CNV_TIPO_APPUNTAMENTO").ToString()
        Me.txtUtenteAppuntamento.Text = drConvocazione("CNV_UTE_ID").ToString()
        '--
    End Sub

    'esegue i controlli di validità
    Private Sub Salva()
        '--
        Dim errStr As String = ""
        '--
        ' Controllo dei dati modificabili (data convocazione, durata appuntamento)
        If Not IsDate(txtDataConvocazione.Text) Then
            errStr = "Inserire una data di convocazione\n"
        ElseIf (txtDurataAppuntamento.Text <> String.Empty) AndAlso (Not IsNumeric(txtDurataAppuntamento.Text)) Then
            errStr = "Inserire un valore numerico per la durata dell\'appuntamento\n"
        End If
        '--
        If errStr <> String.Empty Then
            CustomEventUtility.HandleEventsToMessage(Page, "Errore", "alert('" & errStr & "');")
            Return
        End If
        '--
        Dim drAltraConvocazioneEsistente As DataRow = Me.LoadConvocazione(Me.txtDataConvocazione.Data)
        '--
        Dim nuovaDurataAppuntamento As String = String.Empty
        If Not String.IsNullOrEmpty(Me.txtDurataAppuntamento.Text) AndAlso IsNumeric(Me.txtDurataAppuntamento.Text) Then
            nuovaDurataAppuntamento = Me.txtDurataAppuntamento.Text
        End If
        '--
        'Se è stata modificata la data di convocazione
        '--
        If (Me.DataCnv <> Me.txtDataConvocazione.Data) Then
            '--
            If Not drAltraConvocazioneEsistente Is Nothing Then
                '--
                Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Sono state trovate più convocazioni in questa data.\nUnire tutte le convocazioni in una unica?", KEY_UNISCI_CNV, True, True))
                '--
            Else
                '--
                Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Cambiare la data di convocazione?", KEY_SPOSTA_CNV, True, True))
                '--
            End If
            '--
        ElseIf drAltraConvocazioneEsistente("CNV_DURATA_APPUNTAMENTO").ToString() <> nuovaDurataAppuntamento Then
            '--
            Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Si desidera cambiare solo i dati della convocazione?", KEY_MODIFICA_CNV, True, True))
            '--
        Else
            '--
            Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Nessun cambiamento da registrare!\nModificare i parametri della convocazione o premere il tasto chiudi.", "NoModifiche", False, False))
            '--
        End If
        '--
    End Sub

    Private Function SpostaConvocazione(consultorio As String, mantieniAppuntamento As Boolean, noteAppuntamento As String) As Boolean
        '--
        Dim newDurataApp As Integer = Me.Settings.TEMPOSED
        '--
        If Not String.IsNullOrWhiteSpace(Me.txtDurataAppuntamento.Text) Then
            '--
            Dim durata As Integer = 0
            If Integer.TryParse(Me.txtDurataAppuntamento.Text, durata) Then
                newDurataApp = durata
            End If
            '--
        End If
        '--
        Dim spostaConvocazioneBizResult As Biz.BizConvocazione.SpostaConvocazioneBizResult
        '--
        Using dbGenericProvider As New DbGenericProvider(OnVacContext.Connection)
            '--
            Using bizConvocazione As New Biz.BizConvocazione(dbGenericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                '--
                Dim command As New Biz.BizConvocazione.SpostaConvocazioneCommand()
                command.CodicePaziente = Convert.ToInt64(OnVacUtility.Variabili.PazId)
                command.DataConvocazioneNew = Me.txtDataConvocazione.Data
                command.DurataAppuntamentoNew = newDurataApp
                command.DateConvocazioneOld = {Me.DataCnv}
                command.ControlloAssociabilita = True
                command.MantieniAppuntamento = mantieniAppuntamento
                command.OperazioneAutomatica = False
                command.NoteSpostamentoAppuntamento = noteAppuntamento
                '--
                spostaConvocazioneBizResult = bizConvocazione.SpostaConvocazione(command)
                '--
            End Using
            '--
        End Using

        If Not spostaConvocazioneBizResult.DatiConvocazioneScartata Is Nothing AndAlso spostaConvocazioneBizResult.DatiConvocazioneScartata.Length > 0 Then

            Me.lblErrore.Text = spostaConvocazioneBizResult.Messages.ToHtmlString()

        Else

            ' Impostazione della nuova data di convocazione
            Me.DataCnv = Me.txtDataConvocazione.Data

            ' Aggiornamento dei campi nella maschera di modifica

            Dim drConvocazione As DataRow = Me.LoadConvocazione(Me.DataCnv)

            Me.BindConvocazione(drConvocazione)

            Me.lblErrore.Text = "Convocazione modificata con successo."

            Me.ToolBar.Items.FromKeyButton("btnSalva").Enabled = False

        End If

        Return spostaConvocazioneBizResult.Success

    End Function

    Private Function ModificaConvocazione() As Boolean

        Dim durataAppuntamento As Integer? = Nothing

        If Not String.IsNullOrWhiteSpace(Me.txtDurataAppuntamento.Text) Then

            Dim val As Integer = 0
            If Integer.TryParse(Me.txtDurataAppuntamento.Text, val) Then
                durataAppuntamento = val
            End If

        End If

        Dim result As Biz.BizGenericResult = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizConvocazione As New Biz.BizConvocazione(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), New Biz.BizLogOptions(DataLogStructure.TipiArgomento.CNV_MANUALI, False))

                result = bizConvocazione.ModificaDurataAppuntamento(Convert.ToInt64(OnVacUtility.Variabili.PazId), Me.txtDataConvocazione.Data, durataAppuntamento)

            End Using
        End Using

        Me.lblErrore.Text = result.Message

        If result.Success Then
            Me.ToolBar.Items.FromKeyButton("btnSalva").Enabled = False
        End If

        Return result.Success

    End Function

#End Region

End Class
