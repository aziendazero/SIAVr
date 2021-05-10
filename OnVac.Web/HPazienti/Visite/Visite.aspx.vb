Imports System.Collections.Generic
Imports Onit.Controls.OnitDataPanel.OnitDataPanel
Imports Onit.Controls.PagesLayout.OnitLayout3
Imports Onit.OnAssistnet.Extensions
Imports Onit.OnAssistnet.OnVac.Common.Utility.Extensions

Partial Class OnVac_Visite
    Inherits OnVac.Common.PageBase

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

#Region " Consts "

    Const NOTE_COLUMN_INDEX As Int16 = 8
    Const USL_INSERIMENTO_COLUMN_INDEX As Int16 = 14
    Const NOTE_MAX_LENGTH As Int16 = 30

#End Region

#Region " Private variables "

    Private _PanelUtility As OnitDataPanelUtility

#End Region

#Region " Properties "

#Region " OnitDataPanel "

    Public ReadOnly Property PazId() As String
        Get
            Return OnVacUtility.Variabili.PazId
        End Get
    End Property

    Public ReadOnly Property Consultorio() As String
        Get
            Return OnVacUtility.Variabili.CNS.Codice
        End Get
    End Property

    Public ReadOnly Property Connessione() As String
        Get
            Return OnVacContext.Connection.ConnectionString
        End Get
    End Property

#End Region

    Private Property CurrentRecord() As Integer
        Get
            Return ViewState("CurrentRecord")
        End Get
        Set(value As Integer)
            ViewState("CurrentRecord") = value
        End Set
    End Property

    Public Property PanelUtility() As OnitDataPanelUtility
        Get
            Return _PanelUtility
        End Get
        Set(Value As OnitDataPanelUtility)
            _PanelUtility = Value
        End Set
    End Property

    Public ReadOnly Property SospObbligatoria() As String
        Get
            Return Me.Settings.SOSPOBBLIGATORIA.ToString()
        End Get
    End Property

    Private Property StoricoVaccinaleCentralizzatoDaRecuperare() As String
        Get
            If ViewState("StoricoVaccinaleCentralizzatoDaRecuperare") Is Nothing Then ViewState("StoricoVaccinaleCentralizzatoDaRecuperare") = False
            Return ViewState("StoricoVaccinaleCentralizzatoDaRecuperare")
        End Get
        Set(value As String)
            ViewState("StoricoVaccinaleCentralizzatoDaRecuperare") = value
        End Set
    End Property

    Private Property ShowMessaggioAggiornamentoAnagrafeCentrale() As Boolean
        Get
            If ViewState("OnVac_ShowMessaggioAggiornamentoAnagrafeCentrale") Is Nothing Then
                ViewState("OnVac_ShowMessaggioAggiornamentoAnagrafeCentrale") = True
            End If
            Return ViewState("OnVac_ShowMessaggioAggiornamentoAnagrafeCentrale")
        End Get
        Set(Value As Boolean)
            ViewState("OnVac_ShowMessaggioAggiornamentoAnagrafeCentrale") = Value
        End Set
    End Property

    Private Property IsRowVisible() As Boolean
        Get
            If ViewState("IsRowVisible") Is Nothing Then
                ViewState("IsRowVisible") = True
            End If
            Return ViewState("IsRowVisible")
        End Get
        Set(Value As Boolean)
            ViewState("IsRowVisible") = Value
        End Set
    End Property

    Private Property IsRowEditable() As Boolean
        Get
            If ViewState("IsRowEditable") Is Nothing Then
                ViewState("IsRowEditable") = True
            End If
            Return ViewState("IsRowEditable")
        End Get
        Set(Value As Boolean)
            ViewState("IsRowEditable") = Value
        End Set
    End Property

    Private ReadOnly Property ValoreVisibilitaDefaultPaziente() As String
        Get
            If ViewState("ValoreVisibilitaDefaultPaziente") Is Nothing Then
                ViewState("ValoreVisibilitaDefaultPaziente") = Me.GetValoreVisibilitaDatiVaccinaliDefault(OnVacUtility.Variabili.PazId)
            End If
            Return ViewState("ValoreVisibilitaDefaultPaziente").ToString()
        End Get
    End Property

#End Region

#Region " Eventi pagina "

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        PanelUtility = New OnitDataPanelUtility(Me.ToolBar)

        PanelUtility.FindButtonName = "btnCerca"
        PanelUtility.SaveButtonName = "btnSalva"
        PanelUtility.CancelButtonName = "btnAnnulla"
        PanelUtility.OtherButtonsNames = "btnElimina"
        PanelUtility.MasterDataPanel = odpVisiteMaster
        PanelUtility.DetailDataPanel = odpVisiteDetail
        PanelUtility.WZMsDataGrid = dgrVisiteMaster
        PanelUtility.WZRicBase = filFiltro.FindControl("WzFilterKeyBase")
        PanelUtility.SetToolbarButtonImages()

        AddHandler PanelUtility.BeforeEdit, AddressOf PanelUtility_BeforeEdit

        If Not IsPostBack() Then

            ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK

            PanelUtility.InitToolbar()

            OnVacUtility.ImpostaIntestazioniPagina(OnitLayout31, LayoutTitolo, Nothing, Settings, IsGestioneCentrale)

            ' Visibilità pulsante Recupera Storico
            ToolBar.Items.FromKeyButton("btnRecuperaStoricoVacc").Visible = FlagConsensoVaccUslCorrente

            Dim bcolumn As Onit.Controls.OnitDataPanel.wzBoundColumn
            Dim tcolumn As Onit.Controls.OnitDataPanel.wzTemplateColumn

            ' Visibilità colonna Usl Inserimento
            bcolumn = dgrVisiteMaster.getColumnByKey("UslInserimentoVisita")
            If Not bcolumn Is Nothing Then bcolumn.Hidden = False

            ' Visibilità colonna Flag Visibilità
            tcolumn = dgrVisiteMaster.getColumnByKey("VIS_FLAG_VISIBILITA")
            If Not tcolumn Is Nothing Then tcolumn.Visible = True

            tcolumn = dgrVisiteMaster.getColumnByKey("FLAG_FIRMA")
            If Not tcolumn Is Nothing Then tcolumn.Visible = Settings.FIRMADIGITALE_ANAMNESI_ON

            ' Visibilità Checkbox Flag Visibilità (dettagli)
            lblFlagVisibilita.Visible = True
            chkFlagVisibilita.Visible = True

            StoricoVaccinaleCentralizzatoDaRecuperare = False

            If FlagConsensoVaccUslCorrente Then

                Dim statoAcquisizioneDatiVaccinaliCentrale As Enumerators.StatoAcquisizioneDatiVaccinaliCentrale? =
                        Common.OnVacStoricoVaccinaleCentralizzato.GetStatoAcquisizioneDatiVaccinaliCentralePaziente(OnVacUtility.Variabili.PazId)

                If Not statoAcquisizioneDatiVaccinaliCentrale.HasValue OrElse
                   statoAcquisizioneDatiVaccinaliCentrale = Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Parziale Then

                    StoricoVaccinaleCentralizzatoDaRecuperare = True

                    OnitLayout31.InsertRoutineJS(Common.OnVacStoricoVaccinaleCentralizzato.AlertMessageRecuperoStoricoVaccinale)
                    SetToolbarStatus(True)

                Else
                    SetToolbarStatus(False)
                End If

            Else
                SetToolbarStatus(False)
            End If

        End If

    End Sub

    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles MyBase.PreRender

        ' Abilitazione pulsanti Modifica ed Elimina della Toolbar in base allo stato dei pannelli (sia Master che Detail)
        Dim enableEditButtons As Boolean = False

        If StoricoVaccinaleCentralizzatoDaRecuperare OrElse
            odpVisiteMaster.CurrentOperation <> CurrentOperationTypes.None OrElse
            odpVisiteDetail.CurrentOperation <> CurrentOperationTypes.None Then
            enableEditButtons = False
        Else
            enableEditButtons = (dgrVisiteMaster.Items.Count > 0)
        End If

        ToolBar.Items.FromKeyButton("btnEdit").Enabled = enableEditButtons AndAlso IsRowVisible AndAlso IsRowEditable
        ToolBar.Items.FromKeyButton("btnElimina").Enabled = enableEditButtons AndAlso IsRowVisible AndAlso IsRowEditable

    End Sub

#End Region

#Region " Toolbar "

    Protected Function GetMessaggioConfermaEliminazioneVisita() As String

        '"ATTENZIONE: premendo 'Ok', la visita selezionata verrà eliminata definitivamente. Potrebbero essere presenti anamnesi firmate digitalmente. Continuare con l'eliminazione?"

        Dim msg As New Text.StringBuilder()

        msg.AppendLine("ATTENZIONE: premendo 'Ok', la visita selezionata verrà eliminata definitivamente.")
        msg.AppendLine()
        msg.AppendLine("Potrebbero essere presenti anamnesi firmate digitalmente, che verranno eliminate.")

        ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
        If FlagAbilitazioneVaccUslCorrente Then

            ' Controllo per visualizzare alert solo se la usl corrente ha dato visibilità
            If FlagConsensoVaccUslCorrente AndAlso Settings.ALERT_AGGIORNAMENTO_DATI_CENTRALIZZATI Then

                msg.AppendLine(Common.OnVacStoricoVaccinaleCentralizzato.MessaggioAggiornamentoAnagrafeCentrale)

            End If

            msg.AppendLine("Non saranno eliminate le visite inserite da un'azienda diversa da quella corrente.")

        End If

        If Settings.GIORNI_VARIAZIONE_VACCINAZIONE_ESEGUITA > 0 Then

            msg.Append("Non saranno eliminate le visite registrate piu' di ")
            If Settings.GIORNI_VARIAZIONE_VACCINAZIONE_ESEGUITA = 1 Then
                msg.Append("1 giorno fa.")
            Else
                msg.AppendFormat("{0} giorni fa.", Settings.GIORNI_VARIAZIONE_VACCINAZIONE_ESEGUITA.ToString())
            End If

            msg.AppendLine()

        End If

        msg.AppendLine()
        msg.Append("Continuare con l'eliminazione?")

        Return HttpUtility.JavaScriptStringEncode(msg.ToString())

    End Function

    Private Sub ToolBar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btnElimina"

                ' ------------------------------------ '
                ' [Unificazione Ulss]: Eliminato controllo usl inserimento
                ' ------------------------------------ '
                '' Controllo visita inserita dalla usl corrente
                'Dim currentRow As DataRow = Me.odpVisiteMaster.getCurrentDataRow()

                'Dim codiceUslInserimento As String = String.Empty
                'If Not currentRow.IsNull("VIS_USL_INSERIMENTO") Then codiceUslInserimento = currentRow("VIS_USL_INSERIMENTO").ToString()

                'If Not String.IsNullOrWhiteSpace(codiceUslInserimento) Then

                '    Dim gestioneStoricoVacc As New Common.OnVacStoricoVaccinaleCentralizzato(Me.Settings)
                '    If Not gestioneStoricoVacc.CheckUslInserimentoCorrente(codiceUslInserimento) Then

                '        Me.OnitLayout31.InsertRoutineJS(Common.OnVacStoricoVaccinaleCentralizzato.AlertMessageCancellazioneVisitaNoUslCorrente)
                '        Return

                '    End If
                'End If
                ' ------------------------------------ '

                DeleteVisitaCorrente()

            Case "btnSalva"

                Dim confirmRequired As Boolean = False

                If txtFineSospensione.Data <> DateTime.MinValue Then

                    ' Controllo se esistono convocazioni precedenti la data massima di fine sospensione
                    If EsistonoCnvPrecedentiFineSospensione(txtFineSospensione.Data) Then

                        ' Se esistono cnv con data precedente la fine sospensione, visualizzo la modale di conferma.
                        fmUnisciCnv.VisibileMD = True
                        confirmRequired = True

                    End If

                End If

                If Not confirmRequired Then
                    InsertUpdateVisita()
                End If

            Case "btnRecuperaStoricoVacc"

                RecuperaStoricoVaccinale()

            Case Else
                If e.Button.Key = "btnNew" Then
                    Table_Dettaglio.Visible = True
                    Lbl_StatoDetail.Visible = False
                End If

                PanelUtility.ManagingToolbar(e.Button.Key)

        End Select

    End Sub

#End Region

#Region " Eventi datapanel "

    Private Sub odpVisiteDetail_afterUpdateWzControls(sender As Controls.OnitDataPanel.OnitDataPanel) Handles odpVisiteDetail.afterUpdateWzControls

        ' Valorizzazione checkbox visibilità
        Dim valoreVisibilita As String = String.Empty

        If odpVisiteMaster.getCurrentDataTable().Rows.Count > 0 Then
            valoreVisibilita = odpVisiteMaster.getCurrentDataRow("VIS_FLAG_VISIBILITA", DataRowVersion.Current).ToString()
        End If

        chkFlagVisibilita.Checked = (valoreVisibilita = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente)

    End Sub

    Private Sub odpVisiteDetail_afterSaveData(sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles odpVisiteDetail.afterSaveData

        odpVisiteMaster.Find()

    End Sub

    Private Sub odpVisiteDetail_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpVisiteDetail.afterOperation

        OnitLayout31.Busy = PanelUtility.CheckToolBarState(operation)

        Select Case operation

            Case CurrentOperationTypes.EditRecord, CurrentOperationTypes.NewRecord

                If Settings.SOSPOBBLIGATORIA Then
                    fmMotivoSospensione.Obbligatorio = True
                    fmMotivoSospensione.CssClass = "textBox_Stringa_Obbligatorio"
                    txtFineSospensione.CssClass = "textBox_Stringa_Obbligatorio"
                End If

                If operation = CurrentOperationTypes.NewRecord Then

                    If Settings.BILANCI_PREVALORIZZA_OPERATORI Then

                        ' Impostazione automatica medico con il valore impostato come medico responsabile nelle vac prog.
                        fmMedico.Codice = OnVacUtility.Variabili.MedicoResponsabile.Codice
                        fmMedico.Descrizione = OnVacUtility.Variabili.MedicoResponsabile.Nome

                        ' Impostazione automatica rilevatore: se è già stato memorizzato un valore, imposto il rilevatore delle OnVacUtility.
                        ' Altrimenti, imposto come rilevatore il vaccinatore selezionato nelle vac prog.
                        If String.IsNullOrWhiteSpace(OnVacUtility.Variabili.MedicoRilevatore.Codice) Then
                            fmRilevatore.Codice = OnVacUtility.Variabili.MedicoVaccinante.Codice
                            fmRilevatore.Descrizione = OnVacUtility.Variabili.MedicoVaccinante.Nome
                        Else
                            fmRilevatore.Codice = OnVacUtility.Variabili.MedicoRilevatore.Codice
                            fmRilevatore.Descrizione = OnVacUtility.Variabili.MedicoRilevatore.Nome
                        End If

                    End If

                    ' Impostazione filtri modali operatori
                    fmMedico.Filtro = OnVacUtility.GetModalListFilterOperatori(True, True)
                    fmRilevatore.Filtro = OnVacUtility.GetModalListFilterOperatori(False, True)

                    ' Valorizzazione flag di visibilità per i dati vaccinali del paziente
                    chkFlagVisibilita.Enabled = True
                    chkFlagVisibilita.Checked = (ValoreVisibilitaDefaultPaziente = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente)

                Else

                    ' In edit, non posso modificare la malattia se la visita è legata ad un bilancio
                    Dim bilancio As String = odpVisiteMaster.getCurrentDataRow("VIS_N_BILANCIO", DataRowVersion.Current).ToString()

                    fmMalattia.Enabled = String.IsNullOrEmpty(bilancio)

                    ' Abilitazione flag visibilità
                    ' N.B. : non dovrebbe essere abilitato se la usl di inserimento è diversa da quella corrente, ma in questo caso non viene visualizzato il dettaglio
                    chkFlagVisibilita.Enabled = True

                End If

            Case CurrentOperationTypes.SaveRecord, CurrentOperationTypes.CancelRecord

                fmMotivoSospensione.CssClass = "TextBox_Stringa_Disabilitato"
                txtFineSospensione.CssClass = "TextBox_Stringa_Disabilitato"

        End Select

    End Sub

    Private Sub odpVisiteMaster_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpVisiteMaster.afterOperation

        OnitLayout31.Busy = PanelUtility.CheckToolBarState(operation)

    End Sub

    Private Sub odpVisiteMaster_onCreateQuery(ByRef QB As System.Object) Handles odpVisiteMaster.onCreateQuery

        DirectCast(QB, AbstractQB).AddOrderByFields("vis_data_visita DESC, vis_id DESC")

    End Sub

    Private Sub odpVisiteDetail_onCreateQuery(ByRef QB As System.Object) Handles odpVisiteDetail.onCreateQuery

        DirectCast(QB, AbstractQB).AddOrderByFields("vis_data_visita DESC, vis_id DESC")

    End Sub

    Private Sub odpVisiteDetail_onError(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, err As Onit.Controls.OnitDataPanel.OnitDataPanelError) Handles odpVisiteDetail.onError

        err.mute = True

        If Not IsNothing(err.exc) Then

            ' Se c'è un'eccezione, controlla se il codice oracle è relativo all'errore di chiave duplicata.
            If err.exc.Message.ToString().Contains(Constants.OracleErrors.ORA_00001) Then

                ' La chiave è diventata VIS_ID, gestita da trigger: non verrà più generato un errore di chiave duplicata
                ' Altrimenti, sollevo un'eccezione così da loggarla nell'EventLog
                'Me.OnitLayout31.ShowMsgBox(New OnitLayoutMsgBox("Non è possibile inserire due visite con la stessa data!!", "", False, False))
                Throw New DuplicatedDataException("Chiave duplicata in salvataggio Visita.", err.exc)

            End If

        Else

            ' Altrimenti, stampa la descrizione dell'errore
            OnitLayout31.ShowMsgBox(New OnitLayoutMsgBox(err.comment, "", False, False))

        End If

        odpVisiteDetail.CancelData()

    End Sub

#Region " Controlli pre modifica/cancellazione di una visita "

    Private Sub PanelUtility_BeforeEdit(sender As Object, e As OnitDataPanelUtility.BeforeOperationEventArgs)

        Dim currentRow As DataRow = odpVisiteMaster.getCurrentDataRow()

        If GetFlagFirmaDigitaleVisitaCorrente(currentRow) Then

            OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(Biz.BizFirmaDigitale.Messages.NO_EDIT_ANAMNESI_FIRMATA, "errFirma", False, False))

            e.Cancel = True
            Return

        End If

        Dim controlloVisitaResult As ControlloVisitaResult = ControlloVisitaCorrente(currentRow, False)

        If Not controlloVisitaResult.Success Then

            OnitLayout31.InsertRoutineJS(controlloVisitaResult.Message)

            e.Cancel = True
            Return

        End If

        ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
        If FlagAbilitazioneVaccUslCorrente Then

            ShowAlertAggiornamentoStoricoCentralizzato()

        End If

    End Sub

    Private Class ControlloVisitaResult

        Public Success As Boolean
        Public Message As String

        Public Sub New()
        End Sub

        Public Sub New(success As Boolean, message As String)
            Me.Success = success
            Me.Message = message
        End Sub

    End Class

    Private Function ControlloVisitaCorrente(currentRow As DataRow, isDeleteOperation As Boolean) As ControlloVisitaResult

        If Not currentRow Is Nothing Then

            ' ------------------------------------ '
            ' [Unificazione Ulss]: il controllo sulla usl inserimento qui va mantenuto
            ' ------------------------------------ '
            Dim gestioneStoricoVacc As New Common.OnVacStoricoVaccinaleCentralizzato(Settings)

            ' Controllo visita inserita dalla usl corrente (solo se viene gestito lo storico centralizzato)
            ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
            If FlagAbilitazioneVaccUslCorrente AndAlso Not gestioneStoricoVacc.CheckVisitaStessaUsl(currentRow) Then

                Dim message As String = Common.OnVacStoricoVaccinaleCentralizzato.AlertMessageModificaVisitaNoUslCorrente

                If isDeleteOperation Then message = Common.OnVacStoricoVaccinaleCentralizzato.AlertMessageCancellazioneVisitaNoUslCorrente

                Return New ControlloVisitaResult(False, message)

            End If
            ' ------------------------------------ '

            ' Controllo numero giorni trascorsi dalla registrazione (indipendentemente dalla gestione dello storico centralizzato)
            If Not OnVacUtility.CheckGiorniTrascorsiVariazioneDatiVaccinali(currentRow("vis_data_registrazione"), Me.Settings) Then

                Dim message As String = "la modifica"

                If isDeleteOperation Then message = "l\'eliminazione"

                Return New ControlloVisitaResult(False, String.Format("alert('Il numero di giorni trascorsi dalla data di registrazione è superiore al limite massimo impostato ({0}): impossibile effettuare {1}.');",
                                                                      Settings.GIORNI_VARIAZIONE_VACCINAZIONE_ESEGUITA.ToString(), message))

            End If

        End If

        Return New ControlloVisitaResult(True, String.Empty)

    End Function


    ''' <summary>
    ''' Restituisce il valore del flag firma, per la visita selezionata
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetFlagFirmaDigitaleVisitaCorrente(currentRow As DataRow) As Boolean

        If currentRow Is Nothing Then Return False
        If currentRow("VIS_UTE_ID_FIRMA") Is Nothing OrElse currentRow.IsNull("VIS_UTE_ID_FIRMA") Then Return False

        If String.IsNullOrEmpty(currentRow("VIS_UTE_ID_FIRMA").ToString()) Then Return False

        Return True

    End Function

#End Region

#End Region

#Region " Eventi datagrid "

    Private Sub dgrVisiteMaster_InitializeRow(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrVisiteMaster.ItemDataBound
        ' Se il campo note è troppo lungo, il testo viene tagliato altrimenti allargherebbe la colonna 
        ' e le successive non verrebbero visualizzate
        Dim s As String = e.Item.Cells(NOTE_COLUMN_INDEX).Text
        If Not String.IsNullOrEmpty(s) AndAlso s.Length > NOTE_MAX_LENGTH Then
            e.Item.Cells(NOTE_COLUMN_INDEX).Text = s.Substring(0, NOTE_MAX_LENGTH) + " [...]"
        End If

        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim enableEditDelete As Boolean = False
            Dim isPrimaRiga As Boolean = e.Item.ItemIndex = 0
            If isPrimaRiga Then
                If OnVacContext.CodiceUslCorrente = e.Item.Cells(USL_INSERIMENTO_COLUMN_INDEX).Text Then
                    IsRowVisible = True
                    IsRowEditable = True
                Else
                    If CtrlIsInUslUnificata(e.Item.Cells(USL_INSERIMENTO_COLUMN_INDEX).Text) Then
                        IsRowVisible = True
                        IsRowEditable = True
                    Else
                        IsRowVisible = DirectCast(e.Item.FindControl("HiddenField_FLAGVISIBILITA"), HiddenField).Value = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente
                        IsRowEditable = DirectCast(e.Item.FindControl("HiddenField_FLAGVISIBILITA"), HiddenField).Value = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente
                    End If
                End If
                SetVisitaDetail(IsRowVisible)
            End If
        End If

    End Sub

    Private Function CtrlIsInUslUnificata(codiceUsl As String) As Boolean

        'verifico se il codice usl è una vecchia usl che fa parte della nuova ulss
        If Not String.IsNullOrWhiteSpace(codiceUsl) Then
            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizUsl As New Biz.BizUsl(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    If bizUsl.IsInUslUnificata(codiceUsl, OnVacContext.CodiceUslCorrente) Then
                        Return True
                    End If

                End Using
            End Using
        End If
        Return False

    End Function

    Private Sub dgrVisiteMaster_SelectedIndexChanged(sender As Object, e As EventArgs) Handles dgrVisiteMaster.SelectedIndexChanged
        If OnVacContext.CodiceUslCorrente = dgrVisiteMaster.SelectedItem.Cells(USL_INSERIMENTO_COLUMN_INDEX).Text Then
            IsRowVisible = True
            IsRowEditable = True
        Else
            If CtrlIsInUslUnificata(dgrVisiteMaster.SelectedItem.Cells(USL_INSERIMENTO_COLUMN_INDEX).Text) Then
                IsRowVisible = True
                IsRowEditable = True
            Else
                IsRowVisible = DirectCast(dgrVisiteMaster.SelectedItem.FindControl("HiddenField_FLAGVISIBILITA"), HiddenField).Value = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente
                IsRowEditable = DirectCast(dgrVisiteMaster.SelectedItem.FindControl("HiddenField_FLAGVISIBILITA"), HiddenField).Value = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente
            End If
        End If
        SetVisitaDetail(IsRowVisible)
    End Sub

    Private Sub SetVisitaDetail(isVisible As Boolean)

        Table_Dettaglio.Visible = isVisible
        Lbl_StatoDetail.Visible = Not isVisible

    End Sub

#End Region

#Region " Pulsanti Modale conferma spostamento "

    Private Sub btnSpostaAnnulla_Click(sender As System.Object, e As System.EventArgs) Handles btnSpostaAnnulla.Click

        Me.fmUnisciCnv.VisibileMD = False

        Me.odpVisiteDetail.CancelData()
        Me.odpVisiteDetail.LoadData()

        'Me.dgrVisiteMaster.needSelPostBack = wzDataGrid.needPosts.True
        Me.dgrVisiteMaster.SelectionOption = Onit.Controls.OnitGrid.OnitGrid.SelectionOptions.rowClick

    End Sub

    Private Sub btnSpostaOK_Click(sender As System.Object, e As System.EventArgs) Handles btnSpostaOK.Click

        Me.InsertUpdateVisita()

        Me.fmUnisciCnv.VisibileMD = False

    End Sub

#End Region

#Region " Private Methods "

    Private Sub DeleteVisitaCorrente()

        Dim currentRow As DataRow = Me.odpVisiteDetail.getCurrentDataRow()
        If Not currentRow Is Nothing Then

            Dim idVisita As Long = 0
            If Long.TryParse(currentRow("vis_id"), idVisita) Then

                Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, OnVacUtility.GetReadCommittedTransactionOptions())

                    Using dbGenericProviderFactory As New Biz.DbGenericProviderFactory()

                        Dim visitaEliminataList As New List(Of Entities.Visita)()
                        Dim osservazioneEliminataList As New List(Of Entities.Osservazione)()

                        ' Eliminazione osservazioni e visite
                        Dim listIdVisita As New List(Of Long)()

                        Using bizVisite As New Biz.BizVisite(dbGenericProviderFactory, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                            Dim visitaDeleted As Entities.Visita = bizVisite.GenericProvider.Visite.GetVisitaById(idVisita)

                            Dim deleteVisitaAndOsservazioniResult As Biz.BizVisite.EliminaVisitaAndOsservazioniResult =
                                bizVisite.DeleteVisitaAndOsservazioni(visitaDeleted)

                            osservazioneEliminataList.AddRange(deleteVisitaAndOsservazioniResult.OsservazioniEliminate)

                            visitaEliminataList.Add(visitaDeleted)

                            listIdVisita.Add(idVisita)

                        End Using

                        ' Eliminazione documenti (in attesa di firma, firmati e archiviati)
                        Using bizFirma As New Biz.BizFirmaDigitale(dbGenericProviderFactory, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                            bizFirma.DeleteDocumentiVisite(listIdVisita)

                        End Using

                        ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
                        If FlagAbilitazioneVaccUslCorrente Then

                            Using bizPaziente As Biz.BizPaziente = New Biz.BizPaziente(dbGenericProviderFactory, Me.Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.GEST_BIL, True))

                                Dim aggiornaDatiVaccinaliCentraliCommand As New Biz.BizPaziente.AggiornaDatiVaccinaliCentraliCommand()
                                aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale = bizPaziente.GenericProvider.Paziente.GetCodiceAusiliario(OnVacUtility.Variabili.PazId)
                                aggiornaDatiVaccinaliCentraliCommand.VisitaEliminataEnumerable = visitaEliminataList.AsEnumerable()
                                aggiornaDatiVaccinaliCentraliCommand.OsservazioneEliminataEnumerable = osservazioneEliminataList.AsEnumerable()

                                bizPaziente.AggiornaDatiVaccinaliCentrali(aggiornaDatiVaccinaliCentraliCommand)

                            End Using

                        End If

                    End Using

                    transactionScope.Complete()

                End Using

            End If
        End If

        Me.Server.Transfer(Me.Request.RawUrl)

    End Sub

    Private Sub InsertUpdateVisita()

        ' Ci sono problemi di layout quando si clicca annulla dello user control, 
        ' oppure quando viene visualizzato il messaggio di errore.

        Me.CurrentRecord = Me.odpVisiteDetail.CurrentRecord

        Dim visitaBizResult As Biz.BizResult

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, OnVacUtility.GetReadCommittedTransactionOptions())

            Using dbGenericProviderFactory As New Biz.DbGenericProviderFactory()

                Dim visita As Entities.Visita = Nothing
                Dim osservazioneEnumerable As IEnumerable(Of Entities.Osservazione) = Nothing

                Dim rilevatoreModificato As Boolean = False

                Using bizVisite As New Biz.BizVisite(dbGenericProviderFactory, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    Select Case Me.odpVisiteDetail.CurrentOperation

                        Case CurrentOperationTypes.NewRecord

                            visita = New Entities.Visita()

                            Me.SetVisita(visita)

                            If Not String.IsNullOrWhiteSpace(visita.RilevatoreCodice) AndAlso visita.RilevatoreCodice <> OnVacUtility.Variabili.MedicoRilevatore.Codice Then
                                rilevatoreModificato = True
                            End If

                            Dim insertVisitaCommand As New Biz.BizVisite.InserisciVisitaCommand()
                            insertVisitaCommand.Visita = visita
                            insertVisitaCommand.Note = "Inserimento visita da maschera Visite"

                            visitaBizResult = bizVisite.InsertVisita(insertVisitaCommand)

                        Case CurrentOperationTypes.EditRecord

                            visita = bizVisite.GenericProvider.Visite.GetVisitaById(Me.odpVisiteDetail.getCurrentDataRow()("vis_id"))

                            Dim visitaOriginale As Entities.Visita = visita.Clone()

                            Me.SetVisita(visita)

                            If Not String.IsNullOrWhiteSpace(visita.RilevatoreCodice) AndAlso visita.RilevatoreCodice <> visitaOriginale.RilevatoreCodice Then
                                rilevatoreModificato = True
                            End If

                            Dim updateVisitaCommand As New Biz.BizVisite.ModificaVisitaCommand()
                            updateVisitaCommand.Visita = visita
                            updateVisitaCommand.VisitaOriginale = visitaOriginale
                            updateVisitaCommand.Note = "Update visita da maschera Visite"
                            updateVisitaCommand.SovrascriviInfoModifica = True

                            If visitaOriginale.DataVisita <> visita.DataVisita Then

                                updateVisitaCommand.Osservazioni = bizVisite.GenericProvider.Visite.GetOsservazioniByVisita(visita.IdVisita)

                                For Each osservazione As Entities.Osservazione In updateVisitaCommand.Osservazioni
                                    osservazione.DataVisita = visita.DataVisita
                                Next

                            End If

                            visitaBizResult = bizVisite.UpdateVisita(updateVisitaCommand, False)

                            If visitaBizResult.Success Then
                                osservazioneEnumerable = updateVisitaCommand.Osservazioni
                            End If

                        Case Else
                            Throw New NotImplementedException()

                    End Select

                End Using

                If visitaBizResult.Success Then

                    ' Impostazione del rilevatore (se modificato rispetto al valore originale)
                    If Me.Settings.BILANCI_PREVALORIZZA_OPERATORI AndAlso rilevatoreModificato AndAlso Me.fmRilevatore.IsValid Then

                        If Not String.IsNullOrWhiteSpace(Me.fmRilevatore.Codice) Then
                            OnVacUtility.Variabili.MedicoRilevatore.Codice = Me.fmRilevatore.Codice
                            OnVacUtility.Variabili.MedicoRilevatore.Nome = Me.fmRilevatore.Descrizione
                        Else
                            ' N.B. : se il rilevatore viene lasciato vuoto, non lo sbianco nelle onvacutility perchè in gestione bilanci è obbligatorio.
                        End If

                    End If

                    ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
                    If FlagAbilitazioneVaccUslCorrente Then

                        Using bizPaziente As Biz.BizPaziente = New Biz.BizPaziente(dbGenericProviderFactory, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.GEST_BIL, True))

                            Dim aggiornaDatiVaccinaliCentraliCommand As New Biz.BizPaziente.AggiornaDatiVaccinaliCentraliCommand()
                            aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale = bizPaziente.GenericProvider.Paziente.GetCodiceAusiliario(OnVacUtility.Variabili.PazId)
                            aggiornaDatiVaccinaliCentraliCommand.VisitaEnumerable = {visita}
                            aggiornaDatiVaccinaliCentraliCommand.OsservazioneEnumerable = osservazioneEnumerable

                            bizPaziente.AggiornaDatiVaccinaliCentrali(aggiornaDatiVaccinaliCentraliCommand)

                        End Using

                    End If

                Else
                    Me.OnitLayout31.InsertRoutineJS(String.Format("alert(""Impossibile eseguire l\'operazione:\n{0}"")", visitaBizResult.Messages.ToJavascriptString()))
                End If

            End Using

            transactionScope.Complete()

        End Using

        If visitaBizResult.Success Then
            Me.Server.Transfer(Me.Request.RawUrl)
        End If

    End Sub

    'Restituisce false se non esistono convocazioni precedenti la data di fine sospensione impostata (oppure se non è stata impostata nessuna data).
    'Se ci sono convocazioni con data inferiore alla data di fine sospensione, restituisce true.
    Private Function EsistonoCnvPrecedentiFineSospensione(dataFineSospensione As DateTime) As Boolean

        Dim cnvPrecedentiFineSospensione As Boolean = False

        If dataFineSospensione > DateTime.MinValue Then

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                Using bizCnv As New Biz.BizConvocazione(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)

                    Dim listDateConvocazione As IEnumerable(Of DateTime) = bizCnv.GetDateConvocazioniPaziente(OnVacUtility.Variabili.PazId, dataFineSospensione)

                    cnvPrecedentiFineSospensione = (Not listDateConvocazione Is Nothing AndAlso listDateConvocazione.Count > 0)

                End Using

            End Using

        End If

        Return cnvPrecedentiFineSospensione

    End Function

    Private Sub ScriviLog(rowLogVisite As DataRow, recordLog As Record)

        Dim logOperation As New Operazione()

        Select Case rowLogVisite.RowState
            Case DataRowState.Added
                logOperation = Operazione.Inserimento
            Case DataRowState.Deleted
                logOperation = Operazione.Eliminazione
            Case DataRowState.Modified
                logOperation = Operazione.Modifica
            Case DataRowState.Unchanged
                logOperation = Operazione.Generico
        End Select

        Dim testataLog As New Testata(TipiArgomento.VISITE, logOperation, False)
        Dim recordLogVisite As New Record()

        recordLogVisite = LogBox.GetRecordFromRow(rowLogVisite)

        testataLog.Records.Add(recordLogVisite)

        If Not recordLog Is Nothing Then
            testataLog.Records.Add(recordLog)
        End If

        LogBox.WriteData(testataLog)

    End Sub

    Private Sub SetToolbarStatus(enableRecuperaStoricoVaccOnly As Boolean)

        ToolBar.Items.FromKeyButton("btnNew").Enabled = Not enableRecuperaStoricoVaccOnly
        ToolBar.Items.FromKeyButton("btnEdit").Enabled = Not enableRecuperaStoricoVaccOnly
        ToolBar.Items.FromKeyButton("btnElimina").Enabled = Not enableRecuperaStoricoVaccOnly
        ToolBar.Items.FromKeyButton("btnRecuperaStoricoVacc").Enabled = enableRecuperaStoricoVaccOnly

    End Sub

    Private Function ShowAlertAggiornamentoStoricoCentralizzato() As Boolean

        ' Controllo per visualizzare alert solo se la usl corrente ha dato visibilità
        ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
        If FlagConsensoVaccUslCorrente Then

            If Settings.ALERT_AGGIORNAMENTO_DATI_CENTRALIZZATI AndAlso ShowMessaggioAggiornamentoAnagrafeCentrale Then

                OnitLayout31.InsertRoutineJS(Common.OnVacStoricoVaccinaleCentralizzato.AlertMessageAggiornamentoAnagrafeCentrale)

                ShowMessaggioAggiornamentoAnagrafeCentrale = False

            End If

        End If

    End Function

    Private Sub RecuperaStoricoVaccinale()

        Dim command As New Common.OnVacStoricoVaccinaleCentralizzato.AcquisisciStoricoCommand()
        command.CodicePaziente = OnVacUtility.Variabili.PazId
        command.RichiediConfermaSovrascrittura = False
        command.Settings = Settings
        command.OnitLayout3 = OnitLayout31
        command.BizLogOptions = Nothing
        command.Note = "Recupero Storico Vaccinale da maschera Visite"

        Dim acquisisciDatiVaccinaliCentraliResult As Biz.BizPaziente.AcquisisciDatiVaccinaliCentraliResult =
            Common.OnVacStoricoVaccinaleCentralizzato.AcquisisciDatiVaccinaliCentraliPaziente(command)

        OnitLayout31.InsertRoutineJS(String.Format("document.location.href='{0}'", HttpUtility.UrlEncode(Request.RawUrl.Substring(Request.RawUrl.LastIndexOf("/") + 1))))

    End Sub

    Private Sub SetVisita(visita As Entities.Visita)

        visita.CodicePaziente = OnVacUtility.Variabili.PazId
        visita.CodiceConsultorio = OnVacUtility.Variabili.CNS.Codice

        If String.IsNullOrWhiteSpace(visita.CodiceUslInserimento) Then
            visita.CodiceUslInserimento = OnVacContext.CodiceUslCorrente
        End If

        visita.DataVisita = txtDataVisita.Data
        visita.MalattiaCodice = fmMalattia.Codice
        visita.MedicoCodice = fmMedico.Codice
        visita.RilevatoreCodice = fmRilevatore.Codice
        visita.MotivoSospensioneCodice = fmMotivoSospensione.Codice
        visita.DataFineSospensione = txtFineSospensione.Data

        If txtNote.Text.Length > txtNote.MaxLength Then
            txtNote.Text = txtNote.Text.Substring(0, txtNote.MaxLength)
        End If

        visita.Note = txtNote.Text

        If Not ddlVaccinabile.SelectedItem Is Nothing Then
            visita.Vaccinabile = ddlVaccinabile.SelectedValue
        Else
            visita.Vaccinabile = String.Empty
        End If

        visita.FlagVisibilitaDatiVaccinaliCentrale = Common.OnVacStoricoVaccinaleCentralizzato.GetValoreVisibilitaDatiVaccinali(chkFlagVisibilita)

    End Sub

#End Region

#Region " Protected Methods "

    Protected Function BindFlagVisibilitaImageUrlValue(dataItem As Object) As String

        Return Common.OnVacStoricoVaccinaleCentralizzato.GetImageUrlFlagVisibilita(dataItem, "VIS_FLAG_VISIBILITA", Me)

    End Function

    Protected Function BindFlagVisibilitaToolTipValue(dataItem As Object) As String

        Return Common.OnVacStoricoVaccinaleCentralizzato.GetToolTipFlagVisibilita(dataItem, "VIS_FLAG_VISIBILITA")

    End Function

    Protected Function BindFlagFirmaImageUrlValue(dataItem As Object) As String

        Return OnVacUtility.GetFlagFirmaImageUrlValue(DataBinder.Eval(dataItem, "VIS_UTE_ID_FIRMA"), DataBinder.Eval(dataItem, "VIS_UTE_ID_ARCHIVIAZIONE"), Me.Page)

    End Function

    Protected Function BindFlagFirmaToolTipValue(dataItem As Object) As String

        Return OnVacUtility.GetFlagFirmaToolTipValue(DataBinder.Eval(dataItem, "VIS_UTE_ID_FIRMA"), DataBinder.Eval(dataItem, "VIS_UTE_ID_ARCHIVIAZIONE"))

    End Function



#End Region

End Class
