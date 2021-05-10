Imports Onit.Controls


Partial Class OnVac_Vaccinazioni
    Inherits OnVac.Common.PageBase

#Region " Fields "



#End Region

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

    'contiene i codici dei motivi di esclusione
    Property arrEscCod() As ArrayList
        Get
            Return Session("CodEscInseriti")
        End Get
        Set(Value As ArrayList)
            Session("CodEscInseriti") = Value
        End Set
    End Property

    'contiene le descrizioni dei motivi di esclusione
    Property arrEscDesc() As ArrayList
        Get
            Return Session("DescEscInseriti")
        End Get
        Set(Value As ArrayList)
            Session("DescEscInseriti") = Value
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

        Me.PanelUtility = New OnitDataPanelUtility(Me.ToolBarVaccinazioni)
        Me.PanelUtility.FindButtonName = "btnCerca"
        Me.PanelUtility.DeleteButtonName = "btnElimina"
        Me.PanelUtility.SaveButtonName = "btnSalva"
        Me.PanelUtility.CancelButtonName = "btnAnnulla"
        Me.PanelUtility.OtherButtonsNames = "btnInfo;btnCodificaAssociazione"
        Me.PanelUtility.MasterDataPanel = Me.OdpVaccinazioniMaster
        Me.PanelUtility.DetailDataPanel = Me.OdpVaccinazioniDetail
        Me.PanelUtility.WZMsDataGrid = Me.WzDgrVaccinazioni
        Me.PanelUtility.WZRicBase = Me.WzFilter1.FindControl("WzFilterKeyBase")
        Me.PanelUtility.SetToolbarButtonImages()

        Dim btnInfo As Infragistics.WebUI.UltraWebToolbar.TBarButton = Me.ToolBarVaccinazioni.Items.FromKeyButton("btnInfo")
        btnInfo.Image = "~/images/info.png"
        btnInfo.DisabledImage = "~/images/info_dis.png"

        AddHandler Me.PanelUtility.BeforeSave, AddressOf PanelUtility_BeforeSave
        AddHandler Me.PanelUtility.BeforeNew, AddressOf PanelUtility_BeforeNew

        If Not IsPostBack() Then

            Me.PanelUtility.InitToolbar()

            'slokkiamo il paziente lokkato...
            Me.OnitLayout31.lock.EndLock(OnVacUtility.Variabili.PazId)

        End If

    End Sub

#End Region

#Region " Eventi OnitDataPanel e PanelUtility "

    Private Sub PanelUtility_BeforeSave(sender As Object, e As OnitDataPanelUtility.BeforeOperationEventArgs)

        If Me.OdpVaccinazioniDetail.CurrentOperation = Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord Then
            '--
            Me.WzTbCodVac.Text = Me.WzTbCodVac.Text.Trim()
            Dim result As CheckResult = Me.CheckCampoCodice(Me.WzTbCodVac.Text)
            '--
            If Not result.Success Then
                e.Cancel = True
                Me.OnitLayout31.ShowMsgBox(New PagesLayout.OnitLayout3.OnitLayoutMsgBox("Salvataggio non effettuato.\n" + result.Message, "CodErr", False, False))
                Return
            End If
            '--
        End If

    End Sub

    Private Sub PanelUtility_BeforeNew(sender As Object, e As OnitDataPanelUtility.BeforeOperationEventArgs)

        WzDgrVaccinazioni.SelectionOption = OnitGrid.OnitGrid.SelectionOptions.rowClick

        Me.OdpVaccinazioniDetail.NewRecord(False)

        Me.chkShowInApp.Checked = True
        Me.chkEstraiAvn.Checked = True
        Me.ddlTipoCodifica.SelectedValue = "V"

        e.Cancel = True

    End Sub

    Private Sub OdpVaccinazioniMaster_onCreateQuery(ByRef QB As Object) Handles OdpVaccinazioniMaster.onCreateQuery
        DirectCast(QB, AbstractQB).AddOrderByFields("vac_ordine", "vac_obbligatoria", "vac_descrizione")
    End Sub

    Private Sub OdpVaccinazioniMaster_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles OdpVaccinazioniMaster.afterOperation
        Me.PanelUtility.CheckToolBarState(operation)
    End Sub

    'nel caso in cui sia impossibile eliminare le vaccinazioni già utilizzate (modifica 30/12/2004)
    Private Sub OdpVaccinazioniMaster_onError(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, err As Onit.Controls.OnitDataPanel.OnitDataPanelError)

        If Not err.exc Is Nothing AndAlso err.exc.Message.Contains(Constants.OracleErrors.ORA_02292) Then
            err.exc = New Exception(" (le vaccinazioni risultano già utilizzate nel programma)")
            err.comment = "Attenzione: non è stato possibile eliminare le vaccinazioni selezionate!"
        End If

    End Sub

    Private Sub OdpVaccinazioniDetail_afterSaveData(sender As OnitDataPanel.OnitDataPanel) Handles OdpVaccinazioniDetail.afterSaveData
        Me.OdpVaccinazioniMaster.Find()
    End Sub

    Private Sub OdpVaccinazioniDetail_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles OdpVaccinazioniDetail.afterOperation

        Me.OnitLayout31.Busy = Me.PanelUtility.CheckToolBarState(operation)

        Me.EnableControlli18Anni()

    End Sub

    Private Sub OdpVaccinazioniDetail_onCreateQuery(ByRef QB As Object) Handles OdpVaccinazioniDetail.onCreateQuery
        DirectCast(QB, AbstractQB).AddOrderByFields("vac_ordine", "vac_obbligatoria", "vac_descrizione")
    End Sub

    'valorizza il datatable con tutte le vaccinazioni sostitute [modifica 08/08/2005]
    Private Sub OdpVaccinazioniDetail_afterSaveToDb(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, dt As System.Data.DataTable, encoder As Onit.Controls.OnitDataPanel.FieldsEncoder, dam As Object) Handles OdpVaccinazioniDetail.afterSaveToDb

        Dim fRow As DataRow

        For Each row As DataRow In dt.Rows
            fRow = OnVacUtility.dt_VacSostitute.Rows.Find(row("VAC_CODICE"))
            If Not fRow Is Nothing Then
                fRow("VAC_COD_SOSTITUTA") = row("VAC_COD_SOSTITUTA")
            End If
        Next

    End Sub

#End Region

#Region " Controls Events "

    Private Sub ToolBarVaccinazioni_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBarVaccinazioni.ButtonClicked

        Select Case e.Button.Key

            Case "btnInfo"
                Me.Response.Redirect(String.Format("InfoVaccinazioni.aspx?VAC={0}|{1}", HttpUtility.UrlEncode(Me.WzTbCodVac.Text), HttpUtility.UrlEncode(Me.WzTbDescVac.Text)), False)

            Case "btnCodificaAssociazione"

                ' Caricamento codifiche esterne per associazione della vaccinazione selezionata
                Dim currentRow As DataRow = Me.OdpVaccinazioniMaster.getCurrentDataRow()

                If Not currentRow Is Nothing Then
                    Me.ucCodificaAssociazione.CaricaDati(currentRow("vac_codice").ToString())
                End If

                Me.OnitLayout31.Busy = True
                Me.fmCodificaAssociazioni.VisibileMD = True

            Case Else
                Me.PanelUtility.ManagingToolbar(e.Button.Key)

        End Select

    End Sub

    Private Sub chkControllo18anni_CheckedChanged(sender As Object, e As System.EventArgs) Handles chkControllo18anni.CheckedChanged

        Me.EnableControlli18Anni()

        Me.ddlSceltaSesso.ClearSelection()
        Me.txtMinimoDosi.Text = String.Empty
        Me.WzTbCodMotEscl.Text = String.Empty
        Me.InsMotivoEsc1.MotiviSelezionati = String.Empty

    End Sub

    Private Sub btnMotiviEsclusione_Click(sender As Object, e As System.EventArgs) Handles btnMotiviEsclusione.Click

        'caricamento della modale contenente l'elenco dei motivi di esclusione
        Me.InsMotivoEsc1.ModaleName = "modInsMotivoEsc"
        Me.InsMotivoEsc1.MotiviSelezionati = WzTbCodMotEscl.Text
        Me.InsMotivoEsc1.LoadModale()

        Me.modInsMotivoEsc.VisibileMD = True

        Me.OnitLayout31.Busy = True

    End Sub

#End Region

#Region " Usercontrol Motivo Esclusione "

    'recupera i codici dei motivi di esclusione selezionati nella modale (modifica 08/07/2004)
    Private Sub MotivoEsc_InviaCodMotEsc(arrMotEscCod As System.Collections.ArrayList, arrMotEscDesc As System.Collections.ArrayList) Handles InsMotivoEsc1.InviaCodMotEsc

        Me.modInsMotivoEsc.VisibileMD = False

        'visualizzazione dei codici dei motivi di esclusione valorizzati
        Me.arrEscCod = arrMotEscCod
        Me.arrEscDesc = arrMotEscDesc

        If Me.arrEscCod.Count > 0 Then

            Me.WzTbCodMotEscl.Text = "" & Me.arrEscCod(0)

            For i As Integer = 1 To arrEscCod.Count - 1
                Me.WzTbCodMotEscl.Text &= "," & Me.arrEscCod(i)
            Next

        Else
            Me.WzTbCodMotEscl.Text = String.Empty
        End If

    End Sub

    'riabilita il layout alla chiusura della modale dei motivi di esclusione (modifica 08/07/2004)
    Private Sub MotivoEsc_RiabilitaLayout() Handles InsMotivoEsc1.RiabilitaLayout

        Me.modInsMotivoEsc.VisibileMD = False

        'i codici dei motivi di esclusione devono essere vuoti
        Me.arrEscCod = Nothing
        Me.arrEscDesc = Nothing

    End Sub

#End Region

#Region " User Control Codifica Vaccinazioni per Età e per associazione "

    Private Sub ucCodificaAssociazione_Close() Handles ucCodificaAssociazione.Close
        Me.OnitLayout31.Busy = False
        Me.fmCodificaAssociazioni.VisibileMD = False
    End Sub

#End Region

#Region " Private "

    Private Sub EnableControlli18Anni()

        Dim bool As Boolean = False

        Select Case OdpVaccinazioniDetail.CurrentOperation

            Case Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord,
                 Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.EditRecord

                bool = Me.chkControllo18anni.Checked

        End Select

        Me.ddlSceltaSesso.Enabled = bool
        Me.ddlSceltaSesso.CssClass = IIf(bool, Me.ddlSceltaSesso.CssStyles.CssRequired, Me.ddlSceltaSesso.CssStyles.CssDisabled)

        Me.txtMinimoDosi.Enabled = bool
        Me.txtMinimoDosi.CssClass = IIf(bool, Me.txtMinimoDosi.CssStyles.CssRequired, Me.txtMinimoDosi.CssStyles.CssDisabled)

        Me.btnMotiviEsclusione.Enabled = bool

    End Sub

#End Region

End Class
