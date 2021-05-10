Imports Onit.Controls
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Common


Partial Class OnVac_Ambulatori
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

    Private Property NuovoInserimento() As Boolean
        Get
            Return ViewState("nuovo_inserimento")
        End Get
        Set(Value As Boolean)
            ViewState("nuovo_inserimento") = Value
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

    Public ReadOnly Property PageTrace() As PageTrace
        Get
            Return Session("PageTrace")
        End Get
    End Property

#End Region

#Region " Events handlers "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Me.PanelUtility = New OnitDataPanelUtility(Me.ToolBar)
        Me.PanelUtility.FindButtonName = "btnCerca"
        Me.PanelUtility.SaveButtonName = "btnSalva"
        Me.PanelUtility.CancelButtonName = "btnAnnulla"
        Me.PanelUtility.OtherButtonsNames = "btnLinkIndisponibilita;btnLinkOrari"
        Me.PanelUtility.MasterDataPanel = Me.odpAmbulatoriMaster
        Me.PanelUtility.DetailDataPanel = Me.odpAmbulatoriDetail
        Me.PanelUtility.WZMsDataGrid = Me.dgrAmbulatoriMaster
        Me.PanelUtility.WZRicBase = filFiltro.FindControl("WzFilterKeyBase")

        If NuovoInserimento Then
            Me.ToolBar.Items.FromKeyButton("btnSalvaCheck").Enabled = True
        Else
            Me.ToolBar.Items.FromKeyButton("btnSalvaCheck").Enabled = False
        End If

        If Not IsPostBack() Then
            If Not PageTrace Is Nothing Then
                CType(filFiltro.FindControl("WzFilterKeyBase"), TextBox).Text = PageTrace.GetValue("WzFilterKeyBase")
            End If
            Me.odpAmbulatoriMaster.Find()
            PanelUtility.InitToolbar()
            Me.OnitLayout31.lock.EndLock(OnVacUtility.Variabili.PazId)
        End If

        PageTrace.Add("WzFilterKeyBase", CType(filFiltro.FindControl("WzFilterKeyBase"), TextBox).Text)

    End Sub

    Private Sub ToolBar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Me.PanelUtility.ManagingToolbar(e.Button.Key)
        Dim saveState As Boolean = False

        Select Case e.Button.Key

            Case "btnSalvaCheck"
                BeforeSave()
                saveState = True
                Me.dgrAmbulatoriMaster.SelectionOption = OnitGrid.OnitGrid.SelectionOptions.rowClick
                Me.ToolBar.Items.FromKeyButton("btnSalvaCheck").Enabled = False
                NuovoInserimento = False

            Case "btnLinkIndisponibilita"
                If dgrAmbulatoriMaster.SelectedItem IsNot Nothing Then
                    Response.Redirect(String.Format("Indisponibilita.aspx?CODICE={0}",
                                                HttpUtility.UrlEncode(OnVacUtility.GetCodeValue(Me.odpAmbulatoriMaster, Me.dgrAmbulatoriMaster))),
                                                False)
                    saveState = True
                End If

            Case "btnLinkOrari"
                If dgrAmbulatoriMaster.SelectedItem IsNot Nothing Then
                    Response.Redirect(String.Format("Orari.aspx?CODICE={0}",
                                                HttpUtility.UrlEncode(OnVacUtility.GetCodeValue(Me.odpAmbulatoriMaster, Me.dgrAmbulatoriMaster))),
                                                False)
                    saveState = True
                End If
            Case "btnNew"
                Me.ToolBar.Items.FromKeyButton("btnSalvaCheck").Enabled = True
                NuovoInserimento = True

            Case "btnEdit"
                Me.ToolBar.Items.FromKeyButton("btnSalvaCheck").Enabled = True

            Case Else
                NuovoInserimento = False

        End Select

        If saveState Then
            OnVacOdpState.SavePanelState(Me.odpAmbulatoriMaster, Me.dgrAmbulatoriMaster)
        End If

    End Sub

#End Region

#Region " Private "

    Private Function GetNumeroAmbulatori(cnsCodice As String) As Integer

        Dim result As Integer
        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            result = genericProvider.Consultori.GetNumeroAmbulatori(cnsCodice)
        End Using

        Return result

    End Function

    Private Sub BeforeSave()

        Dim str As String = String.Empty
        Dim cnsCodice As String = Me.fmConsultorio.Codice.ToUpper()
        Dim maxAmbulatori As Integer

        If NuovoInserimento Then

            If Me.Settings.NUMAMB = -1 Then
                maxAmbulatori = 1
            Else
                maxAmbulatori = Me.Settings.NUMAMB
            End If

            If GetNumeroAmbulatori(cnsCodice) >= maxAmbulatori Then

                str = "Attenzione, questa installazione ha un limite sull\'inserimento di ambulatori che attualmente è " & _
                    maxAmbulatori & ". Per il centro vaccinale " & cnsCodice & " non è possibile inserire ulteriori ambulatori. " & _
                    "Annullare l\'operazione."

                Page.ClientScript.RegisterStartupScript(Me.GetType(), "troppiAmb", String.Format("<script type='text/javascript'> alert('{0}');</script>", str))

            Else
                odpAmbulatoriDetail.SaveData()
            End If
        Else
            odpAmbulatoriDetail.SaveData()
        End If

    End Sub

#End Region

#Region " Eventi datapanel e filter "

	Private Sub odpAmbulatoriMaster_onCreateQuery(ByRef QB As System.Object) Handles odpAmbulatoriMaster.onCreateQuery
		DirectCast(QB, AbstractQB).AddTables("T_ANA_LINK_UTENTI_CONSULTORI")
		DirectCast(QB, AbstractQB).AddWhereCondition("T_ANA_LINK_UTENTI_CONSULTORI.LUC_CNS_CODICE", Database.DataAccessManager.Comparatori.Uguale, "T_ANA_AMBULATORI.AMB_CNS_CODICE", Database.DataAccessManager.DataTypes.Join)
		DirectCast(QB, AbstractQB).AddWhereCondition("LUC_UTE_ID", Database.DataAccessManager.Comparatori.Uguale, OnVacContext.UserId, Database.DataAccessManager.DataTypes.Numero)
		DirectCast(QB, AbstractQB).AddOrderByFields("T_ANA_AMBULATORI.amb_descrizione")
	End Sub

	'nel caso in cui sia impossibile eliminare i Ambulatori già utilizzati (modifica 30/12/2004)
	Private Sub odpAmbulatoriMaster_onError(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, err As Onit.Controls.OnitDataPanel.OnitDataPanelError)

        If Not err.exc Is Nothing AndAlso err.exc.Message.Contains(Constants.OracleErrors.ORA_02292) Then
            err.exc = New Exception(" (gli Ambulatori risultano già utilizzati nel programma)")
            err.comment = "Attenzione: non è stato possibile eliminare gli Ambulatori selezionati!"
        End If

    End Sub

    Private Sub odpAmbulatoriDetail_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpAmbulatoriDetail.afterOperation
        Me.OnitLayout31.Busy = PanelUtility.CheckToolBarState(operation)
    End Sub

    Private Sub odpAmbulatoriDetail_afterSaveData(sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles odpAmbulatoriDetail.afterSaveData
        Me.odpAmbulatoriDetail.LoadData()
        Me.odpAmbulatoriMaster.Find()
    End Sub

    Private Sub odpAmbulatoriDetail_onCreateQuery(ByRef QB As Object) Handles odpAmbulatoriDetail.onCreateQuery
        DirectCast(QB, AbstractQB).AddOrderByFields("amb_descrizione")
    End Sub

    Private Sub Filter_AfterUpdateWzControls(sender As Onit.Controls.OnitDataPanel.wzFilter, e As EventArgs) Handles filFiltro.AfterUpdateWzControls

        If Not Me.Request.QueryString("RicaricaDati") Is Nothing Then
            OnVacOdpState.LoadState(odpAmbulatoriMaster, filFiltro, dgrAmbulatoriMaster)
        End If

    End Sub

    Private Sub Filter_AfterCreateFilters(sender As Onit.Controls.OnitDataPanel.wzFilter, filter As Onit.Controls.OnitDataPanel.FilterCollection) Handles filFiltro.AfterCreateFilters
        OnVacOdpState.SaveFilterState(filFiltro)
    End Sub

#End Region

End Class