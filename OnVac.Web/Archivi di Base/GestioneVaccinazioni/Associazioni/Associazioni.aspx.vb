Imports Onit.Controls
Imports Infragistics.WebUI.UltraWebToolbar


Partial Class OnVac_Associazioni
    Inherits OnVac.Common.PageBase

    Protected WithEvents OdpVaccinazioniDati As Onit.Controls.OnitDataPanel.OnitDataPanel

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

#Region " Types "

    Private Class PageName
        Public Shared VaccinazioniAss As String = "VaccinazioniAss.aspx"
        Public Shared TipiCNSAss As String = "TipiCNSAss.aspx"
        Public Shared InfoAss As String = "InfoAssociazioni.aspx"
    End Class

#End Region

#Region " Eventi Page "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Me.PanelUtility = New OnitDataPanelUtility(Me.ToolBarAssociazioni)
        Me.PanelUtility.FindButtonName = "btnCerca"
        Me.PanelUtility.DeleteButtonName = "btnElimina"
        Me.PanelUtility.SaveButtonName = "btnSalva"
        Me.PanelUtility.CancelButtonName = "btnAnnulla"
        Me.PanelUtility.OtherButtonsNames = "btnVaccinazioni;btnTipiCNS;btnInfo"
        Me.PanelUtility.MasterDataPanel = Me.OdpAssociazioniMaster
        Me.PanelUtility.DetailDataPanel = Me.OdpAssociazioniDetail
        Me.PanelUtility.WZMsDataGrid = Me.WzDgrVaccinazioni
        Me.PanelUtility.WZRicBase = Me.WzFilterKeyBase
        Me.PanelUtility.SetToolbarButtonImages()

        Dim btnInfo As Infragistics.WebUI.UltraWebToolbar.TBarButton = Me.ToolBarAssociazioni.Items.FromKeyButton("btnInfo")
        btnInfo.Image = "~/images/info.png"
        btnInfo.DisabledImage = "~/images/info_dis.png"

        AddHandler Me.PanelUtility.BeforeSave, AddressOf PanelUtility_BeforeSave
        AddHandler Me.PanelUtility.BeforeNew, AddressOf PanelUtility_BeforeNew

        If Not IsPostBack() Then
            Me.PanelUtility.InitToolbar()

            Me.lblSito.Visible = Me.Settings.SITO_INOCULAZIONE_SET_DEFAULT
            Me.fmSitoInoculazione.Visible = Me.Settings.SITO_INOCULAZIONE_SET_DEFAULT

            Me.lblVia.Visible = Me.Settings.VIA_SOMMINISTRAZIONE_SET_DEFAULT
            Me.fmViaSomministrazione.Visible = Me.Settings.VIA_SOMMINISTRAZIONE_SET_DEFAULT

            'slokkiamo il paziente lokkato...
            Me.LayoutAssociazioni.lock.EndLock(OnVacUtility.Variabili.PazId)
        End If

    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub ToolBarAssociazioni_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBarAssociazioni.ButtonClicked

        Select Case e.Button.Key

            Case "btnVaccinazioni"
                RedirectToPage(PageName.VaccinazioniAss)

            Case "btnTipiCNS"
                RedirectToPage(PageName.TipiCNSAss)

            Case "btnInfo"
                RedirectToPage(PageName.InfoAss)

            Case Else
                PanelUtility.ManagingToolbar(e.Button.Key)

        End Select

    End Sub

#End Region

#Region " Eventi OnitDataPanel e PanelUtility "

    Private Sub PanelUtility_BeforeSave(sender As Object, e As OnitDataPanelUtility.BeforeOperationEventArgs)

        If Me.OdpAssociazioniDetail.CurrentOperation = Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord Then
            '--
            Me.WzTbCodAss.Text = Me.WzTbCodAss.Text.Trim()
            Dim result As CheckResult = Me.CheckCampoCodice(Me.WzTbCodAss.Text)
            '--
            If Not result.Success Then
                e.Cancel = True
                Me.LayoutAssociazioni.ShowMsgBox(New PagesLayout.OnitLayout3.OnitLayoutMsgBox("Salvataggio non effettuato.\n" + result.Message, "CodErr", False, False))
                Return
            End If
            '--
        End If

    End Sub

    Private Sub PanelUtility_BeforeNew(sender As Object, e As OnitDataPanelUtility.BeforeOperationEventArgs)

        WzDgrVaccinazioni.SelectionOption = OnitGrid.OnitGrid.SelectionOptions.rowClick

        Me.OdpAssociazioniDetail.NewRecord(False)

        Me.chkShowInApp.Checked = True

        e.Cancel = True

    End Sub

    Private Sub OdpAssociazioniMaster_onCreateQuery(ByRef QB As Object) Handles OdpAssociazioniMaster.onCreateQuery
        DirectCast(QB, AbstractQB).AddOrderByFields("ass_ordine", "ass_descrizione")
    End Sub

    Private Sub OdpAssociazioniMaster_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles OdpAssociazioniMaster.afterOperation
        Me.LayoutAssociazioni.Busy = Me.PanelUtility.CheckToolBarState(operation)
    End Sub

    'nel caso in cui sia impossibile eliminare un'associazione già utilizzata (modifica 30/12/2004)
    Private Sub OdpAssociazioniMaster_onError(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, err As Onit.Controls.OnitDataPanel.OnitDataPanelError) Handles OdpAssociazioniMaster.onError

        If Not err.exc Is Nothing AndAlso err.exc.Message.Contains(Constants.OracleErrors.ORA_02292) Then
            err.exc = New Exception(" (le associazioni risultano già utilizzate nel programma)")
            err.comment = "Attenzione: non è stato possibile eliminare le associazioni selezionate!"
        End If

    End Sub

    Private Sub OdpAssociazioniDetail_afterSaveData(sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles OdpAssociazioniDetail.afterSaveData
        Me.OdpAssociazioniMaster.Find()
    End Sub

    Private Sub OdpAssociazioniDetail_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles OdpAssociazioniDetail.afterOperation
        Me.LayoutAssociazioni.Busy = Me.PanelUtility.CheckToolBarState(operation)
    End Sub

    Private Sub OdpAssociazioniDetail_onCreateQuery(ByRef QB As Object) Handles OdpAssociazioniDetail.onCreateQuery
        DirectCast(QB, AbstractQB).AddOrderByFields("ass_ordine, ass_descrizione")
    End Sub

#End Region

#Region " Private "

    Private Sub RedirectToPage(pageName As String)
        Me.Response.Redirect(String.Format("{0}?ASS={1}|{2}", pageName, HttpUtility.UrlEncode(Me.WzTbCodAss.Text), HttpUtility.UrlEncode(Me.WzTbDescAss.Text)), False)
    End Sub

#End Region

End Class
