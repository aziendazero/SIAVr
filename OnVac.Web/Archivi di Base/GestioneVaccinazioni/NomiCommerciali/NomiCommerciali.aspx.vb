Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager
Imports Onit.Controls
Imports Infragistics.WebUI.UltraWebGrid


Partial Class OnVac_NomiCommerciali
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

    Public ReadOnly Property Connessione() As String
        Get
            Return OnVacContext.Connection.ConnectionString
        End Get
    End Property

    Private _currentRecord As Integer
    Private Property CurrentRecord() As Integer
        Get
            Return _currentRecord
        End Get
        Set(Value As Integer)
            _currentRecord = Value
        End Set
    End Property

    Private Property OnVacOdpState() As OnitDataPanelState
        Get
            If Session("StatoNOC") Is Nothing Then Session("StatoNOC") = New OnitDataPanelState()
            Return Session("StatoNOC")
        End Get
        Set(Value As OnitDataPanelState)
            Session("StatoNOC") = Value
        End Set
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

#Region " Eventi Page "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Me.PanelUtility = New OnitDataPanelUtility(Me.ToolBarNomiCom)

        Me.PanelUtility.FindButtonName = "btnCerca"
        Me.PanelUtility.DeleteButtonName = "btnElimina"
        Me.PanelUtility.SaveButtonName = "btnSalva"
        Me.PanelUtility.CancelButtonName = "btnAnnulla"
        Me.PanelUtility.OtherButtonsNames = "btnAssociazioni;btnPagamento"
        Me.PanelUtility.MasterDataPanel = Me.OdpNomiCommercialiMaster
        Me.PanelUtility.DetailDataPanel = Me.OdpNomiCommercialiDetail
        Me.PanelUtility.WZDataGrid = Me.WzDgrNomiCom
        Me.PanelUtility.WZRicBase = Me.WzFilter1.FindControl("WzFilterKeyBase")
        Me.PanelUtility.SetToolbarButtonImages()

        AddHandler Me.PanelUtility.BeforeSave, AddressOf PanelUtility_BeforeSave

        If Not IsPostBack() Then

            ' Azzero lo stato del dataPanel per non impostare filtri "sporchi"
            If Request.QueryString("NOC") Is Nothing Then
                Me.OnVacOdpState = Nothing
            End If

            Me.CurrentRecord = Me.OdpNomiCommercialiMaster.CurrentRecord

            Me.PanelUtility.InitToolbar()

            Me.lblSito.Visible = Me.Settings.SITO_INOCULAZIONE_SET_DEFAULT
            Me.fmSitoInoculazione.Visible = Me.Settings.SITO_INOCULAZIONE_SET_DEFAULT

            Me.lblVia.Visible = Me.Settings.VIA_SOMMINISTRAZIONE_SET_DEFAULT
            Me.fmViaSomministrazione.Visible = Me.Settings.VIA_SOMMINISTRAZIONE_SET_DEFAULT

            'slokkiamo il paziente lokkato...
            Me.Onitlayout31.lock.EndLock(OnVacUtility.Variabili.PazId)

        End If

    End Sub

    Protected Overrides Sub OnPreRender(e As System.EventArgs)
        '--
        MyBase.OnPreRender(e)
        '--
        If Me.Settings.PAGAMENTO Then
            '--
            Dim enableBtnPagamento As Boolean = False
            '--
            Select Case OdpNomiCommercialiDetail.CurrentOperation
                '--
                Case Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.EditRecord,
                     Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord
                    '--
                    Me.ddlTipPag.CssClass = "textbox_stringa"
                    Me.ddlTipPag.Enabled = True
                    '--
                Case Else
                    '--
                    Me.ddlTipPag.CssClass = "textbox_stringa_disabilitato"
                    Me.ddlTipPag.Enabled = False
                    '--
                    Dim currentRow As DataRow = Me.OdpNomiCommercialiDetail.getCurrentDataRow()
                    '--
                    If Not currentRow Is Nothing Then
                        '--
                        Dim tipoPagamento As Object = currentRow("NOC_TPA_GUID_TIPI_PAGAMENTO")
                        If Not tipoPagamento Is DBNull.Value Then
                            '--
                            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                                Using bizNomiCommerciali As New Biz.BizNomiCommerciali(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())
                                    '--
                                    enableBtnPagamento = bizNomiCommerciali.HasGestioneCondizioniPagamento(DirectCast(tipoPagamento, Byte()))
                                    '--
                                End Using
                            End Using
                            '--
                        End If
                        '--
                    End If
            End Select
            '--
            Me.ToolBarNomiCom.Items.FromKeyButton("btnPagamento").Enabled = enableBtnPagamento
            '--
        Else
            '--
            Me.ToolBarNomiCom.Items.FromKeyButton("btnPagamento").Enabled = False
            '--
            Me.ddlTipPag.Enabled = False
            Me.ddlTipPag.CssClass = "textbox_stringa_disabilitato"
            '--
            Me.valCosto.Enabled = False
            Me.valCosto.CssClass = "textbox_numerico_disabilitato"
            '--
        End If
        '--
    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub ToolBarNomiCom_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBarNomiCom.ButtonClicked

        Select Case e.Button.Key

            Case "btnAssociazioni"

                Me.OnVacOdpState.SavePanelState(Me.OdpNomiCommercialiMaster, WzDgrNomiCom)

                Me.Response.Redirect(String.Format("NomiCommercialiAss.aspx?NOC={0}|{1}",
                                                   HttpUtility.UrlEncode(Me.WzCodNomeCom.Text),
                                                   HttpUtility.UrlEncode(Me.WzDescNomeCom.Text)),
                                     False)

            Case "btnPagamento"

                ' Caricamento condizioni di pagamento del nome commerciale selezionato
                Dim currentRow As DataRow = Me.OdpNomiCommercialiMaster.getCurrentDataRow()

                If Not currentRow Is Nothing Then
                    Me.ucElencoCondizioniPagamento.CaricaDati(currentRow("noc_codice").ToString())
                End If

                Me.Onitlayout31.Busy = True
                Me.fmCondizioniPagamento.VisibileMD = True

            Case Else

                Me.PanelUtility.ManagingToolbar(e.Button.Key)

        End Select

    End Sub

#End Region

#Region " Eventi DataPanel e PanelUtility "

    Private Sub PanelUtility_BeforeSave(sender As Object, e As OnitDataPanelUtility.BeforeOperationEventArgs)

        If Me.OdpNomiCommercialiDetail.CurrentOperation = Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord Then
            '--
            Me.WzCodNomeCom.Text = Me.WzCodNomeCom.Text.Trim()
            Dim result As CheckResult = Me.CheckCampoCodice(Me.WzCodNomeCom.Text)
            '--
            If Not result.Success Then
                e.Cancel = True
                Me.Onitlayout31.ShowMsgBox(New PagesLayout.OnitLayout3.OnitLayoutMsgBox("Salvataggio non effettuato.\n" + result.Message, "CodErr", False, False))
                Return
            End If
            '--
        End If

    End Sub

    Private Sub OdpNomiCommercialiMaster_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles OdpNomiCommercialiMaster.afterOperation

        Me.Onitlayout31.Busy = PanelUtility.CheckToolBarState(operation)

    End Sub

    Private Sub OdpNomiCommercialiDetail_beforeLoadData(sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles OdpNomiCommercialiDetail.beforeLoadData
        '--
        Me.ddlTipPag.Items.Clear()
        '--
        If Me.Settings.PAGAMENTO Then
            '--
            Dim listTipiPagamento As List(Of Entities.TipiPagamento) = Nothing
            '--
            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizNomiCommerciali As New Biz.BizNomiCommerciali(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())
                    '--
                    listTipiPagamento = bizNomiCommerciali.GetListTipiPagamento()
                    '--
                End Using
            End Using
            '--
            If Not listTipiPagamento Is Nothing AndAlso listTipiPagamento.Count > 0 Then
                '--
                For Each tipoPagamento As Entities.TipiPagamento In listTipiPagamento
                    Me.ddlTipPag.Items.Add(New ListItem(tipoPagamento.Descrizione, tipoPagamento.GuidPagamento.ToString()))
                Next
                '--
            End If
            '--
        End If
        '--
    End Sub

    Private Sub OdpNomiCommercialiDetail_afterSaveData(sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles OdpNomiCommercialiDetail.afterSaveData

        Me.OdpNomiCommercialiMaster.Find()

    End Sub

    Private Sub OdpNomiCommercialiDetail_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles OdpNomiCommercialiDetail.afterOperation

        Select Case operation
            Case Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.CancelRecord,
                 Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.SaveRecord
                '--
                Me.AbilitaEta(False)
                '--
            Case Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.DuplicateRecord,
                 Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.EditRecord,
                 Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord
                '--
                Me.AbilitaEta(True)
                '--
        End Select

        Me.Onitlayout31.Busy = Me.PanelUtility.CheckToolBarState(operation)

    End Sub

    Private Sub OdpNomiCommercialiDetail_afterUpdateWzControls(sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles OdpNomiCommercialiDetail.afterUpdateWzControls
        '--
        Dim etaInizio As Entities.Eta = New Entities.Eta(Me.wzTbEtaInizio.Text)
        Dim etaFine As Entities.Eta = New Entities.Eta(Me.wzTbEtaFine.Text)
        '--
        Me.tbInizioAnni.Text = etaInizio.Anni
        Me.tbInizioMesi.Text = etaInizio.Mesi
        Me.tbInizioGiorni.Text = etaInizio.Giorni
        '--
        Me.tbFineAnni.Text = etaFine.Anni
        Me.tbFineMesi.Text = etaFine.Mesi
        Me.tbFineGiorni.Text = etaFine.Giorni
        '--
        Dim currentRow As DataRow = Me.OdpNomiCommercialiDetail.getCurrentDataRow()
        '--
        If Not currentRow Is Nothing Then
            '--
            Dim guidPagamento As Object = currentRow("NOC_TPA_GUID_TIPI_PAGAMENTO")
            '--
            If guidPagamento Is DBNull.Value Then
                Me.ddlTipPag.ClearSelection()
            Else
                Me.ddlTipPag.SelectedValue = New Guid(DirectCast(guidPagamento, Byte())).ToString()
            End If
            '--
        End If
        '--
    End Sub

    Private Sub OdpNomiCommercialiDetail_BeforeUpdateFromControls(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, currRow As System.Data.DataRow, encoder As Onit.Controls.OnitDataPanel.FieldsEncoder) Handles OdpNomiCommercialiDetail.BeforeUpdateFromControls
        '--
        If OdpNomiCommercialiDetail.CurrentOperation = Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.SaveRecord Then
            '--
            Dim currentRow As DataRow = Me.OdpNomiCommercialiDetail.getCurrentDataRow()
            '--
            If Not currentRow Is Nothing Then
                '--
                Me.wzTbEtaFine.Text = New Entities.Eta(Me.tbFineGiorni.Text, Me.tbFineMesi.Text, Me.tbFineAnni.Text).GiorniTotali()
                Me.wzTbEtaInizio.Text = New Entities.Eta(Me.tbInizioGiorni.Text, Me.tbInizioMesi.Text, Me.tbInizioAnni.Text).GiorniTotali()
                '--
                If Not String.IsNullOrEmpty(Me.ddlTipPag.SelectedValue) Then
                    currentRow("NOC_TPA_GUID_TIPI_PAGAMENTO") = New Guid(Me.ddlTipPag.SelectedValue).ToByteArray()
                Else
                    currentRow("NOC_TPA_GUID_TIPI_PAGAMENTO") = DBNull.Value
                End If
                '--
            End If
            '--
        End If
        '--
    End Sub

    Private Sub OdpNomiCommercialiDetail_beforeOperation(sender As Controls.OnitDataPanel.OnitDataPanel, operation As Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles OdpNomiCommercialiDetail.beforeOperation

        If Me.OdpNomiCommercialiDetail.CurrentOperation = Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord Then

            Me.WzCodNomeCom.Text = Me.WzCodNomeCom.Text.Trim()

        End If

    End Sub

    Private Sub OdpNomiCommercialiDetail_onCreateQuery(ByRef qb As Object) Handles OdpNomiCommercialiDetail.onCreateQuery
        '--
        DirectCast(qb, AbstractQB).AddSelectFields("NOC_TPA_GUID_TIPI_PAGAMENTO")
        '--
    End Sub

    Private Sub OdpNomiCommercialiDetail_afterSaveRecord(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, row As DataRow, encoder As Onit.Controls.OnitDataPanel.FieldsEncoder, dam As Object) Handles OdpNomiCommercialiDetail.afterSaveRecord
        '--
        If OdpNomiCommercialiDetail.CurrentOperation <> Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.DeleteRecord Then
            '--
            DirectCast(dam, IDAM).QB.NewQuery()
            DirectCast(dam, IDAM).QB.AddTables("T_ANA_NOMI_COMMERCIALI")
            '--
            If Not row("NOC_TPA_GUID_TIPI_PAGAMENTO") Is DBNull.Value Then
                DirectCast(dam, IDAM).QB.AddUpdateField("NOC_TPA_GUID_TIPI_PAGAMENTO", DirectCast(row("NOC_TPA_GUID_TIPI_PAGAMENTO"), Byte()), DataTypes.Binary)
            Else
                DirectCast(dam, IDAM).QB.AddUpdateField("NOC_TPA_GUID_TIPI_PAGAMENTO", DBNull.Value, DataTypes.Binary)
            End If
            '--
            DirectCast(dam, IDAM).QB.AddWhereCondition("NOC_CODICE", Comparatori.Uguale, row("NOC_CODICE"), DataTypes.Stringa)
            DirectCast(dam, IDAM).ExecNonQuery(ExecQueryType.Update)
            '--
        End If
        '--
    End Sub

#End Region

#Region " WzFilter Events "

    Private Sub WzFilter1_AfterCreateFilters(sender As OnitDataPanel.wzFilter, Filters As OnitDataPanel.FilterCollection) Handles WzFilter1.AfterCreateFilters

        Me.OnVacOdpState.SaveFilterState(Me.WzFilter1)

    End Sub

    Private Sub WzFilter1_AfterUpdateWzControls(sender As OnitDataPanel.wzFilter, e As EventArgs) Handles WzFilter1.AfterUpdateWzControls

        If Not Request.QueryString("NOC") Is Nothing Then
            Me.OnVacOdpState.LoadState(Me.OdpNomiCommercialiMaster, Me.WzFilter1, Me.WzDgrNomiCom)
        End If

    End Sub

#End Region

#Region " Eventi Datagrid "

    Private Sub WzDgrNomiCom_InitializeRow(sender As Object, e As Infragistics.WebUI.UltraWebGrid.RowEventArgs) Handles WzDgrNomiCom.InitializeRow

        Dim row As DataRowView = e.Data
        Dim dgr As UltraWebGrid = sender

        Dim cellEtaInizio As CellItem = DirectCast(DirectCast(dgr.Columns.FromKey("ETAINIZIALE"), TemplatedColumn).CellItems(e.Row.Index), Infragistics.WebUI.UltraWebGrid.CellItem)
        Dim cellEtaFine As CellItem = DirectCast(DirectCast(dgr.Columns.FromKey("ETAFINE"), TemplatedColumn).CellItems(e.Row.Index), Infragistics.WebUI.UltraWebGrid.CellItem)
        Dim etaInizio As Entities.Eta = New Entities.Eta(row("NOC_ETA_INIZIO"))
        Dim etaFine As Entities.Eta = New Entities.Eta(row("NOC_ETA_FINE"))

        DirectCast(cellEtaInizio.FindControl("inizioAnni"), Label).Text = etaInizio.Anni
        DirectCast(cellEtaInizio.FindControl("inizioMesi"), Label).Text = etaInizio.Mesi
        DirectCast(cellEtaInizio.FindControl("inizioGiorni"), Label).Text = etaInizio.Giorni

        DirectCast(cellEtaFine.FindControl("fineAnni"), Label).Text = etaFine.Anni
        DirectCast(cellEtaFine.FindControl("fineMesi"), Label).Text = etaFine.Mesi
        DirectCast(cellEtaFine.FindControl("fineGiorni"), Label).Text = etaFine.Giorni

        If e.Row.Index = CurrentRecord Then
            e.Row.Activate()
            e.Row.Selected = True
        Else
            e.Row.Activate()
            e.Row.Selected = False
        End If

    End Sub

#End Region

#Region " Eventi User Control "

    Private Sub ucElencoCondizioniPagamento_Close() Handles ucElencoCondizioniPagamento.Close
        Me.Onitlayout31.Busy = False
        Me.fmCondizioniPagamento.VisibileMD = False
    End Sub

#End Region

#Region " Private "

    Private Sub AbilitaEta(abilita As Boolean)

        Dim cssClass As String = String.Empty

        If abilita Then
            cssClass = "textbox_numerico w40PX"
        Else
            cssClass = "textbox_numerico_disabilitato w40PX"
        End If

        Me.tbFineAnni.CssClass = cssClass
        Me.tbFineAnni.ReadOnly = Not abilita
        Me.tbFineMesi.CssClass = cssClass
        Me.tbFineMesi.ReadOnly = Not abilita
        Me.tbFineGiorni.CssClass = cssClass
        Me.tbFineGiorni.ReadOnly = Not abilita

        Me.tbInizioAnni.CssClass = cssClass
        Me.tbInizioAnni.ReadOnly = Not abilita
        Me.tbInizioMesi.CssClass = cssClass
        Me.tbInizioMesi.ReadOnly = Not abilita
        Me.tbInizioGiorni.CssClass = cssClass
        Me.tbInizioGiorni.ReadOnly = Not abilita

    End Sub

#End Region

End Class

