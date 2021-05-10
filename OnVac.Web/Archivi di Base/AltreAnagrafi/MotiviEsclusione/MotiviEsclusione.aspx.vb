Imports Onit.Controls
Imports Onit.OnAssistnet.OnVac.Entities


Partial Class OnVac_MotiviEsclusione
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
        Me.PanelUtility.MasterDataPanel = odpMotivoEsclusioneMaster
        Me.PanelUtility.DetailDataPanel = odpMotivoEsclusioneDetail
        Me.PanelUtility.WZDataGrid = dgrEsclusioni
        Me.PanelUtility.WZRicBase = TabFiltri.FindControl("WzFilterKeyBase")
        Me.PanelUtility.SetToolbarButtonImages()

        AddHandler Me.PanelUtility.BeforeSave, AddressOf PanelUtility_BeforeSave

        If Not IsPostBack Then

            Me.PanelUtility.InitToolbar()

            'slokkiamo il paziente lokkato...
            Me.OnitLayout31.lock.EndLock(OnVacUtility.Variabili.PazId)

            ' Visibilità della lista per specificare il tipo di calcolo per il report della copertura
            Me.lblCalcoloCoperture.Visible = Me.Settings.GES_CALCOLO_COPERTURA
            Me.rblCalcoloCoperture.Visible = Me.Settings.GES_CALCOLO_COPERTURA

            ' Visibilità della selezione del tipo di calcolo della scadenza
            Me.lblCalcoloScadenza.Visible = Me.Settings.GES_CALCOLO_SCADENZA_ESCLUSIONE
            Me.ddlCalcoloScadenza.Visible = Me.Settings.GES_CALCOLO_SCADENZA_ESCLUSIONE

            Me.ToolBar.Items.FromKeyButton("btnGestioneScadenza").Visible = Me.Settings.GES_CALCOLO_SCADENZA_ESCLUSIONE

        End If

    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btnGestioneScadenza"

                Dim codiceEsclusione As String = Me.GetCodiceEsclusione()
                Me.Response.Redirect(String.Format("./GestioneScadenze.aspx?moe_codice={0}", codiceEsclusione))

            Case "btnElimina"

                Me.ToolBar.Items.FromKeyButton("btnGestioneScadenza").Enabled = False
                Me.PanelUtility.ManagingToolbar(e.Button.Key)

            Case "btnAnnulla", "btnSalva"

                Me.EnableGestioneScadenze()
                Me.PanelUtility.ManagingToolbar(e.Button.Key)

            Case Else

                Me.PanelUtility.ManagingToolbar(e.Button.Key)

        End Select

    End Sub

#End Region

#Region " Eventi OnitDataPanel e PanelUtility "

    Private Sub PanelUtility_BeforeSave(sender As Object, e As OnitDataPanelUtility.BeforeOperationEventArgs)

        If Me.odpMotivoEsclusioneDetail.CurrentOperation = Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord Then
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

    Private Sub odpMotivoEsclusioneDetail_afterSaveData(sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles odpMotivoEsclusioneDetail.afterSaveData
        Me.PanelUtility.Find("moe_codice")
    End Sub

    Private Sub odpMotivoEsclusioneDetail_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpMotivoEsclusioneDetail.afterOperation
        Me.OnitLayout31.Busy = Me.PanelUtility.CheckToolBarState(operation)
        Me.EnableGestioneScadenze()
    End Sub

    Private Sub odpMotivoEsclusioneMaster_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpMotivoEsclusioneMaster.afterOperation
        Me.OnitLayout31.Busy = Me.PanelUtility.CheckToolBarState(operation)
    End Sub

    Private Sub odpMotivoEsclusioneMaster_onCreateQuery(ByRef QB As Object) Handles odpMotivoEsclusioneMaster.onCreateQuery
        DirectCast(QB, AbstractQB).AddOrderByFields("MOE_DESCRIZIONE")
    End Sub

    'nel caso in cui si tenti di eliminare un motivo di esclusione già utilizzato (modifica 30/12/2004)
    Private Sub odpMotivoEsclusioneMaster_onError(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, err As Onit.Controls.OnitDataPanel.OnitDataPanelError) Handles odpMotivoEsclusioneMaster.onError

        If Not err.exc Is Nothing AndAlso err.exc.Message.Contains(Constants.OracleErrors.ORA_02292) Then
            err.exc = New ApplicationException(" (i motivi di esclusione risultano già utilizzati nel programma)")
            err.comment = "Attenzione: non è stato possibile eliminare i motivi di esclusione selezionati!"
        End If

    End Sub

    Private Sub odpMotivoEsclusioneDetail_beforeSaveToDb(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, dt As System.Data.DataTable, encoder As Onit.Controls.OnitDataPanel.FieldsEncoder, dam As Object) Handles odpMotivoEsclusioneDetail.beforeSaveToDb

        Dim defaultAssegnato As Boolean = False

        Dim currentDataTable As DataTable = Me.odpMotivoEsclusioneDetail.getCurrentDataTable()
        If Not currentDataTable Is Nothing Then

            For i As Integer = 0 To currentDataTable.Rows.Count - 1

                If currentDataTable.Rows(i)("MOE_DEFAULT_INAD").ToString() = "S" Then
                    If defaultAssegnato Then
                        Throw New ApplicationException("Non è possibile avere più di un motivo di esclusione contrassegnato come motivo di default per le inadempienze!!")
                    Else
                        defaultAssegnato = True
                    End If
                End If

            Next

        End If

    End Sub

    Private Sub odpMotivoEsclusioneDetail_beforeOperation(sender As Controls.OnitDataPanel.OnitDataPanel, operation As Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpMotivoEsclusioneDetail.beforeOperation

        Me.ddlCalcoloScadenza.BindingField.Editable = OnitDataPanel.BindingFieldValue.editPositions.always

        If operation = OnitDataPanel.OnitDataPanel.CurrentOperationTypes.EditRecord Then

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                Dim codiceEsclusione As String = Me.GetCodiceEsclusione()
                Dim motivoEsclusione As MotivoEsclusione = genericProvider.MotiviEsclusione.GetMotivoEsclusione(codiceEsclusione)

                If Not motivoEsclusione Is Nothing AndAlso motivoEsclusione.Scadenze.Count > 0 Then
                    Me.ddlCalcoloScadenza.BindingField.Editable = OnitDataPanel.BindingFieldValue.editPositions.never
                End If

            End Using

        End If

    End Sub

#End Region

#Region " Private "

    Private Function GetCodiceEsclusione()

        Dim pkey As String = String.Empty

        Dim drow As DataRow = Me.odpMotivoEsclusioneMaster.getCurrentDataRow()
        If Not IsNothing(drow) Then
            pkey = Me.odpMotivoEsclusioneMaster.GetCurrentTableEncoder().getValOf(drow, "MOE_CODICE", "T_ANA_MOTIVI_ESCLUSIONE", True).ToString()
        End If

        If String.IsNullOrEmpty(pkey) Then Throw New ApplicationException("Chiave per l'esclusione non trovata")

        Return pkey

    End Function

    Private Sub EnableGestioneScadenze()

        Select Case Me.odpMotivoEsclusioneDetail.CurrentOperation

            Case OnitDataPanel.OnitDataPanel.CurrentOperationTypes.MoveRecord,
                 OnitDataPanel.OnitDataPanel.CurrentOperationTypes.None

                Me.ToolBar.Items.FromKeyButton("btnGestioneScadenza").Enabled =
                    (Me.ddlCalcoloScadenza.SelectedValue <> Enumerators.MotiviEsclusioneCalcoloScadenza.NessunCalcolo)

            Case Else

                Me.ToolBar.Items.FromKeyButton("btnGestioneScadenza").Enabled = False

        End Select

    End Sub

#End Region

End Class
