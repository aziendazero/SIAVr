Imports Onit.Controls


Partial Class OnVac_Consultori
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

#Region " Properties "

    Private Property OnVacOdpState() As OnitDataPanelState
        Get
            If Session("Stato") Is Nothing Then Session("Stato") = New OnitDataPanelState()
            Return Session("Stato")
        End Get
        Set(Value As OnitDataPanelState)
            Session("Stato") = Value
        End Set
    End Property

    Public ReadOnly Property Connessione() As String
        Get
            Return OnVacContext.Connection.ConnectionString
        End Get
    End Property

    Private _PanelUtility As OnitDataPanelUtility
    Public Property PanelUtility() As OnitDataPanelUtility
        Get
            Return _PanelUtility
        End Get
        Set(Value As OnitDataPanelUtility)
            _PanelUtility = Value
        End Set
    End Property

#End Region

#Region " Page Events "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Me.PanelUtility = New OnitDataPanelUtility(Me.ToolBar)
        PanelUtility.FindButtonName = "btnCerca"
        PanelUtility.DeleteButtonName = "btnElimina"
        PanelUtility.SaveButtonName = "btnSalva"
        PanelUtility.CancelButtonName = "btnAnnulla"
        PanelUtility.OtherButtonsNames = "btnLinkAssComuni;btnLinkAssCircoscrizioni;btnLinkParametri;btnLinkUtentiAbilitati"
        PanelUtility.MasterDataPanel = Me.odpConsultoriMaster
        PanelUtility.DetailDataPanel = Me.odpConsultoriDetail
        PanelUtility.WZMsDataGrid = Me.dgrConsultoriMaster
        PanelUtility.WZRicBase = Me.filFiltro.FindControl("WzFilterKeyBase")

        AddHandler Me.PanelUtility.BeforeSave, AddressOf PanelUtility_BeforeSave

        If Not IsPostBack() Then
            Me.PanelUtility.InitToolbar()
            'slokkiamo il paziente lokkato...
            Me.OnitLayout31.lock.EndLock(OnVacUtility.Variabili.PazId)
        End If

    End Sub

    Private Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles MyBase.PreRender

        If Me.odpConsultoriDetail.CurrentOperation = OnitDataPanel.OnitDataPanel.CurrentOperationTypes.EditRecord Then
            If Me.odpConsultoriDetail.CurrentOperation = OnitDataPanel.OnitDataPanel.CurrentOperationTypes.EditRecord Then

                If Me.ddlTipoCns.SelectedValue = Constants.TipoConsultorio.Vaccinale Then
                    Me.fmPediatraVac.Enabled = True
                Else
                    Me.fmPediatraVac.Enabled = False
                End If

            End If
        End If

    End Sub

#End Region

#Region " Eventi OnitDataPanel e PanelUtility "

    Private Sub PanelUtility_BeforeSave(sender As Object, e As OnitDataPanelUtility.BeforeOperationEventArgs)
        '--
        ' Controllo codice in inserimento
        If odpConsultoriDetail.CurrentOperation = Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord Then
            '--
            txtCnsCodice.Text = txtCnsCodice.Text.Trim()
            Dim result As CheckResult = CheckCampoCodice(txtCnsCodice.Text)
            '--
            If Not result.Success Then
                e.Cancel = True
                OnitLayout31.ShowMsgBox(New PagesLayout.OnitLayout3.OnitLayoutMsgBox("Salvataggio non effettuato.\n" + result.Message, "CodErr", False, False))
                Return
            End If
            '--
        End If
        '--
        ' Controllo campi età paziente
        If Not CheckCampiEtaPaziente() Then
            e.Cancel = True
            OnitLayout31.ShowMsgBox(New PagesLayout.OnitLayout3.OnitLayoutMsgBox("Salvataggio non effettuato.\nI campi che specificato l'età iniziale e finale del paziente devono essere numeri interi.", "NumErr", False, False))
            Return
        End If

        ' Controllo lunghezza campi Stampa 1 e 2 (ed eventuale riduzione se supera la max length impostata)
        SetFieldLength(txtStampa1)
        SetFieldLength(txtStampa2)

    End Sub

    Private Sub SetFieldLength(txt As TextBox)

        txt.Text = txt.Text.Trim()

        If txt.Text.Length > txt.MaxLength Then
            txt.Text = txt.Text.Substring(0, txt.MaxLength)
        End If

    End Sub

    'nel caso in cui sia impossibile eliminare i consultori già utilizzati (modifica 30/12/2004)
    Private Sub odpConsultoriMaster_onError(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, err As Onit.Controls.OnitDataPanel.OnitDataPanelError) Handles odpConsultoriMaster.onError

		If Not err.exc Is Nothing AndAlso err.exc.Message.Contains(Constants.OracleErrors.ORA_02292) Then
			err.exc = New Exception(" (i centri vaccinali risultano già utilizzati nel programma)")
			err.comment = "Attenzione: non è stato possibile eliminare i centri vaccinali selezionati!"
		End If

	End Sub

	Private Sub odpConsultoriMaster_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpConsultoriMaster.afterOperation
        Me.OnitLayout31.Busy = Me.PanelUtility.CheckToolBarState(operation)
    End Sub

	Private Sub odpConsultoriMaster_onCreateQuery(ByRef QB As System.Object) Handles odpConsultoriMaster.onCreateQuery
		DirectCast(QB, AbstractQB).AddTables("T_ANA_LINK_UTENTI_CONSULTORI")
		DirectCast(QB, AbstractQB).AddWhereCondition("LUC_CNS_CODICE", Database.DataAccessManager.Comparatori.Uguale, "CNS_CODICE", Database.DataAccessManager.DataTypes.Join)
		DirectCast(QB, AbstractQB).AddWhereCondition("LUC_UTE_ID", Database.DataAccessManager.Comparatori.Uguale, OnVacContext.UserId, Database.DataAccessManager.DataTypes.Numero)
		DirectCast(QB, AbstractQB).AddOrderByFields("cns_descrizione")
	End Sub

	Private Sub odpConsultoriDetail_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpConsultoriDetail.afterOperation

        Select Case operation
            '--
            Case OnitDataPanel.OnitDataPanel.CurrentOperationTypes.SaveRecord,
                 OnitDataPanel.OnitDataPanel.CurrentOperationTypes.CancelRecord
                '--
                Me.AbilitaEta(False)
                '--
            Case OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord,
                 OnitDataPanel.OnitDataPanel.CurrentOperationTypes.EditRecord,
                 OnitDataPanel.OnitDataPanel.CurrentOperationTypes.DuplicateRecord
                '--
                Me.AbilitaEta(True)
                '--
        End Select

        Me.OnitLayout31.Busy = Me.PanelUtility.CheckToolBarState(operation)

    End Sub

    Private Sub odpConsultoriDetail_afterSaveData(sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles odpConsultoriDetail.afterSaveData
        Me.odpConsultoriMaster.Find()
    End Sub

    Private Sub odpConsultoriDetail_afterUpdateWzControls(sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles odpConsultoriDetail.afterUpdateWzControls

        Dim etaInizio As New Entities.Eta(Me.wzTbEtaInizio.Text)
        Dim etaFine As New Entities.Eta(Me.wzTbEtaFine.Text)

        Me.tbInizioAnni.Text = etaInizio.Anni
        Me.tbInizioMesi.Text = etaInizio.Mesi
        Me.tbInizioGiorni.Text = etaInizio.Giorni

        Me.tbFineAnni.Text = etaFine.Anni
        Me.tbFineMesi.Text = etaFine.Mesi
        Me.tbFineGiorni.Text = etaFine.Giorni

    End Sub

    Private Sub odpConsultoriDetail_BeforeUpdateFromControls(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, currRow As System.Data.DataRow, encoder As Onit.Controls.OnitDataPanel.FieldsEncoder) Handles odpConsultoriDetail.BeforeUpdateFromControls

        If Me.odpConsultoriDetail.CurrentOperation = OnitDataPanel.OnitDataPanel.CurrentOperationTypes.SaveRecord Then
            Me.wzTbEtaFine.Text = New Entities.Eta(Me.tbFineGiorni.Text, Me.tbFineMesi.Text, Me.tbFineAnni.Text).GiorniTotali()
            Me.wzTbEtaInizio.Text = New Entities.Eta(Me.tbInizioGiorni.Text, Me.tbInizioMesi.Text, Me.tbInizioAnni.Text).GiorniTotali()
        End If

    End Sub

    Private Sub odpConsultoriDetail_onCreateQuery(ByRef QB As Object) Handles odpConsultoriDetail.onCreateQuery
        DirectCast(QB, AbstractQB).AddOrderByFields("cns_descrizione")
    End Sub

#End Region

#Region " WzFilter Events "

    Private Sub Filter_AfterUpdateWzControls(sender As Onit.Controls.OnitDataPanel.wzFilter, e As EventArgs) Handles filFiltro.AfterUpdateWzControls

        If Not Request.QueryString("RicaricaDati") Is Nothing Then
            Me.OnVacOdpState.LoadState(Me.odpConsultoriMaster, Me.filFiltro, Me.dgrConsultoriMaster)
        End If

    End Sub

    Private Sub Filter_AfterCreateFilters(sender As Onit.Controls.OnitDataPanel.wzFilter, filter As Onit.Controls.OnitDataPanel.FilterCollection) Handles filFiltro.AfterCreateFilters
        Me.OnVacOdpState.SaveFilterState(Me.filFiltro)
    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub ToolBar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Dim saveState As Boolean = False

        Me.PanelUtility.ManagingToolbar(e.Button.Key)

        Select Case e.Button.Key
            Case "btnLinkAssComuni"
                If dgrConsultoriMaster.SelectedItem IsNot Nothing Then
                    Response.Redirect(String.Format("ComuniAss.aspx?CODICE={0}",
                                    HttpUtility.UrlEncode(OnVacUtility.GetCodeValue(Me.odpConsultoriMaster, Me.dgrConsultoriMaster))),
                                    False)
                    saveState = True
                End If
            Case "btnLinkParametri"
                If dgrConsultoriMaster.SelectedItem IsNot Nothing Then
                    Response.Redirect(String.Format("Parametri.aspx?tipo=1&CODICE={0}",
                                    HttpUtility.UrlEncode(OnVacUtility.GetCodeValue(Me.odpConsultoriMaster, Me.dgrConsultoriMaster))),
                                    False)
                    saveState = True
                End If
            Case "btnLinkAssCircoscrizioni"
                If dgrConsultoriMaster.SelectedItem IsNot Nothing Then
                    Response.Redirect(String.Format("CircoscrizioniAss.aspx?CODICE={0}",
                                    HttpUtility.UrlEncode(OnVacUtility.GetCodeValue(Me.odpConsultoriMaster, Me.dgrConsultoriMaster))),
                                    False)
                    saveState = True
                End If
            Case "btnLinkUtentiAbilitati"
                If dgrConsultoriMaster.SelectedItem IsNot Nothing Then
                    Response.Redirect(String.Format("UtentiAbilitati.aspx?CODICE={0}",
                                    HttpUtility.UrlEncode(OnVacUtility.GetCodeValue(odpConsultoriMaster, Me.dgrConsultoriMaster, True))),
                                    False)
                    saveState = True
                End If
        End Select

        If saveState Then
            Me.OnVacOdpState.SavePanelState(Me.odpConsultoriMaster, Me.dgrConsultoriMaster)
        End If

    End Sub

#End Region

#Region " DropdownList Events "

    Private Sub ddlTipoCns_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlTipoCns.SelectedIndexChanged

        If ddlTipoCns.SelectedValue = "V" Then
            Me.fmPediatraVac.Enabled = True
        Else
            Me.fmPediatraVac.Enabled = False
            Me.fmPediatraVac.Descrizione = String.Empty
            Me.fmPediatraVac.Codice = String.Empty
        End If

    End Sub

#End Region

#Region " Modal Events "

    Private Sub fmMagazzinoConsultorio_Change(Sender As Object, e As Controls.OnitModalList.ModalListaEventArgument) Handles fmMagazzinoConsultorio.Change

        If e.OldCodice <> Me.fmMagazzinoConsultorio.Codice Then
            If LottiInConsultorio(Me.txtCnsCodice.Text) Then

                Me.OnitLayout31.InsertRoutineJS("alert(""Non è possibile aggiornare il magazzino poichè sono presenti dosi associate al centro vaccinale corrente."");")
                Me.fmMagazzinoConsultorio.Codice = e.OldCodice
                Me.fmMagazzinoConsultorio.Descrizione = e.OldDescrizione

            End If
        End If

    End Sub

#End Region

#Region " Private "

    Private Sub AbilitaEta(enable As Boolean)

        SetReadOnlyStyle(Me.tbInizioAnni, Not enable)
        SetReadOnlyStyle(Me.tbInizioMesi, Not enable)
        SetReadOnlyStyle(Me.tbInizioGiorni, Not enable)
        SetReadOnlyStyle(Me.tbFineAnni, Not enable)
        SetReadOnlyStyle(Me.tbFineMesi, Not enable)
        SetReadOnlyStyle(Me.tbFineGiorni, Not enable)

    End Sub

    Private Sub SetReadOnlyStyle(txt As TextBox, isReadOnly As Boolean)
        txt.CssClass = String.Format("textbox_numerico{0}", IIf(isReadOnly, "_disabilitato", ""))
        txt.ReadOnly = isReadOnly
    End Sub

    Private Function CheckCampiEtaPaziente() As Boolean

        If Not Me.CheckCampoNumericoIntero(Me.tbInizioAnni.Text, Me.tbInizioAnni.MaxLength) Then Return False
        If Not Me.CheckCampoNumericoIntero(Me.tbInizioMesi.Text, Me.tbInizioMesi.MaxLength) Then Return False
        If Not Me.CheckCampoNumericoIntero(Me.tbInizioGiorni.Text, Me.tbInizioGiorni.MaxLength) Then Return False

        If Not Me.CheckCampoNumericoIntero(Me.tbFineAnni.Text, Me.tbFineAnni.MaxLength) Then Return False
        If Not Me.CheckCampoNumericoIntero(Me.tbFineMesi.Text, Me.tbFineMesi.MaxLength) Then Return False
        If Not Me.CheckCampoNumericoIntero(Me.tbFineGiorni.Text, Me.tbFineGiorni.MaxLength) Then Return False

        Return True

    End Function

    Private Function LottiInConsultorio(codiceConsultorio As String) As Boolean

        Dim count As Integer = 0

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            count = genericProvider.Lotti.CountLottiConsultorio(codiceConsultorio)
        End Using

        Return (count > 0)

    End Function

#End Region

End Class
