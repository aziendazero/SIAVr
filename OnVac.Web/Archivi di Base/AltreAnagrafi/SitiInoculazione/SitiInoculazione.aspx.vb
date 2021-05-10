Imports Onit.Controls


Partial Class OnVac_SitiInoculazione
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

        Me.PanelUtility = New OnitDataPanelUtility(ToolBar)
        Me.PanelUtility.FindButtonName = "btnCerca"
        Me.PanelUtility.DeleteButtonName = "btnElimina"
        Me.PanelUtility.SaveButtonName = "btnSalva"
        Me.PanelUtility.CancelButtonName = "btnAnnulla"
        Me.PanelUtility.MasterDataPanel = odpSitiInoculazioneMaster
        Me.PanelUtility.DetailDataPanel = odpSitiInoculazioneDetail
        ' samu (19/02/2004)
        Me.PanelUtility.WZDataGrid = dgrSitiInoculazione
        Me.PanelUtility.WZRicBase = tabFiltri.FindControl("WzFilterKeyBase")
        ' fine samu
        Me.PanelUtility.SetToolbarButtonImages()

        AddHandler Me.PanelUtility.BeforeSave, AddressOf PanelUtility_BeforeSave

        If Not IsPostBack Then
            Me.PanelUtility.InitToolbar()
        End If

    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked
        Me.PanelUtility.ManagingToolbar(e.Button.Key)
    End Sub

#End Region

#Region " Eventi OnitDataPanel e PanelDetail "

    Private Sub PanelUtility_BeforeSave(sender As Object, e As OnitDataPanelUtility.BeforeOperationEventArgs)

        If Me.odpSitiInoculazioneDetail.CurrentOperation = Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord Then
            '--
            Me.txtCodice.Text = Me.txtCodice.Text.Trim()
            Dim result As CheckResult = Me.CheckCampoCodice(Me.txtCodice.Text)
            '--
            If Not result.Success Then
                e.Cancel = True
                Me.OnitLayout31.ShowMsgBox(New PagesLayout.OnitLayout3.OnitLayoutMsgBox("Salvataggio non effettuato.\n" + result.Message, "CodErr", False, False))
                Return
            End If
            '--
        End If

    End Sub

    Private Sub odpSitiInoculazioneMaster_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpSitiInoculazioneMaster.afterOperation
        Me.OnitLayout31.Busy = Me.PanelUtility.CheckToolBarState(operation)
    End Sub

    Private Sub odpSitiInoculazioneMaster_onCreateQuery(ByRef QB As Object) Handles odpSitiInoculazioneMaster.onCreateQuery
        DirectCast(QB, AbstractQB).AddOrderByFields("SII_DESCRIZIONE")
    End Sub

    Private Sub odpSitiInoculazioneDetail_afterSaveData(sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles odpSitiInoculazioneDetail.afterSaveData
        Me.odpSitiInoculazioneMaster.Find()
    End Sub

    Private Sub odpSitiInoculazioneDetail_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpSitiInoculazioneDetail.afterOperation
        Me.OnitLayout31.Busy = Me.PanelUtility.CheckToolBarState(operation)
    End Sub

    Private Sub odpSitiInoculazioneDetail_onCreateQuery(ByRef QB As Object) Handles odpSitiInoculazioneDetail.onCreateQuery
        DirectCast(QB, AbstractQB).AddOrderByFields("SII_DESCRIZIONE")
    End Sub

    'nel caso in cui sia impossibile eliminare il sito di inoculazione (modifica 30/12/2004)
    Private Sub odpSitiInoculazioneMaster_onError(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, err As Onit.Controls.OnitDataPanel.OnitDataPanelError) Handles odpSitiInoculazioneMaster.onError

        If Not err.exc Is Nothing AndAlso err.exc.Message.Contains(Constants.OracleErrors.ORA_02292) Then
            err.exc = New Exception(" (i siti di inoculazione risultano già utilizzati nel programma)")
            err.comment = "Attenzione: non è stato possibile eliminare i siti di inoculazione selezionati!"
        End If

    End Sub

#End Region

End Class
