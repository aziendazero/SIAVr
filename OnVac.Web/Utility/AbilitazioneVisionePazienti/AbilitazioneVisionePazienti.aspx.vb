Partial Class OnVac_AbilitazioneVisionePazienti
    Inherits OnVac.Common.PageBase


#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Private "

    Private _PanelUtility As OnitDataPanelUtility

#End Region

#Region " Properties "

    Public ReadOnly Property Connessione() As String
        Get
            Return OnVacContext.Connection.ConnectionString
        End Get
    End Property

    Public Property PanelUtility() As OnitDataPanelUtility
        Get
            Return _PanelUtility
        End Get
        Set(ByVal Value As OnitDataPanelUtility)
            _PanelUtility = Value
        End Set
    End Property

    Private _CodiceMedicoUtenteLoggato As String
    Private ReadOnly Property CodiceMedicoUtenteLoggato()
        Get
            If _CodiceMedicoUtenteLoggato Is Nothing Then

                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                    _CodiceMedicoUtenteLoggato = genericProvider.Utenti.GetMedicoDaUtente(OnVacContext.UserId)

                End Using

            End If

            Return _CodiceMedicoUtenteLoggato
        End Get
    End Property

#End Region

#Region " Events handlers "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        PanelUtility = New OnitDataPanelUtility(ToolBarAbilVisPaz)
        PanelUtility.FindButtonName = "btnCerca"
        PanelUtility.DeleteButtonName = "btnElimina"
        PanelUtility.SaveButtonName = "btnSalva"
        PanelUtility.CancelButtonName = "btnAnnulla"
        PanelUtility.MasterDataPanel = OdpAbilVisPazMaster
        PanelUtility.DetailDataPanel = OdpAbilVisPazDetail
        PanelUtility.WZDataGrid = WzDgrAbilVisPaz
        PanelUtility.WZRicBase = WzFilter1.FindControl("WzFilterKeyBase")
        PanelUtility.SetToolbarButtonImages()

        If Not IsPostBack() Then

            PanelUtility.InitToolbar()

            'slokkiamo il paziente lokkato...
            OnitLayout31.lock.EndLock(OnVacUtility.Variabili.PazId)

        End If

    End Sub

    Private Sub ToolBarAbilVisPaz_ButtonClicked(ByVal sender As System.Object, ByVal e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBarAbilVisPaz.ButtonClicked
        PanelUtility.ManagingToolbar(e.Button.Key)
    End Sub

#End Region

#Region " Eventi datapanel "

    Private Sub OdpAbilVisPazMaster_afterOperation(ByVal sender As Onit.Controls.OnitDataPanel.OnitDataPanel, ByVal operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles OdpAbilVisPazMaster.afterOperation
        PanelUtility.CheckToolBarState(operation)
    End Sub

    Private Sub OdpAbilVisPazMaster_onCreateQuery(ByRef QB As Object) Handles OdpAbilVisPazMaster.onCreateQuery

        If Me.CodiceMedicoUtenteLoggato <> Nothing Then
            DirectCast(QB, AbstractQB).AddWhereCondition("MAP_MED_CODICE_MEDICO", Database.DataAccessManager.Comparatori.Uguale, Me.CodiceMedicoUtenteLoggato, Database.DataAccessManager.DataTypes.Stringa)
        End If

        DirectCast(QB, AbstractQB).AddOrderByFields("T_ANA_MEDICI.MED_DESCRIZIONE", "MediciAbilitati.MED_DESCRIZIONE", "MAP_DATA_INIZIO")

    End Sub

    Private Sub OdpAbilVisPazDetail_afterOperation(ByVal sender As Onit.Controls.OnitDataPanel.OnitDataPanel, ByVal operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles OdpAbilVisPazDetail.afterOperation

        Select Case operation
            Case Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord
                '--
                Me.wzfmMedico.Enabled = Me.CodiceMedicoUtenteLoggato = Nothing
                '--
                If Me.CodiceMedicoUtenteLoggato <> Nothing Then
                    Me.wzfmMedico.Codice = Me.CodiceMedicoUtenteLoggato
                    Me.wzfmMedico.RefreshDataBind()
                End If
                '--
                Me.wzodpReg.Data = DateTime.Now
                '--
                Me.wzfmUtente.Codice = OnVacContext.UserId.ToString()
                Me.wzfmUtente.Descrizione = OnVacContext.UserDescription
                '--
        End Select

        OnitLayout31.Busy = PanelUtility.CheckToolBarState(operation)

    End Sub

    Private Sub OdpAbilVisPazDetail_afterSaveData(ByVal sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles OdpAbilVisPazDetail.afterSaveData
        OdpAbilVisPazMaster.Find()
    End Sub

#End Region

End Class
