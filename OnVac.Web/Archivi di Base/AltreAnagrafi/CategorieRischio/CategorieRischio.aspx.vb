Imports Infragistics.WebUI.UltraWebToolbar
Imports Onit.Controls
Imports Onit.OnAssistnet.OnVac.Entities
Imports System.Collections.Generic

Partial Class OnVac_CategorieRischio
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

    Private _CodCatRischioCorrente As String
    Private Property CodCatRischio As String
        Get
            Return dgrCatRischio.SelectedItem.Cells(CatRischioGridColumnIndex.Codice).Text
        End Get
        Set(value As String)
            _CodCatRischioCorrente = value
        End Set
    End Property

    Private Property SelezionaTuttiChecked As Boolean
        Get
            If ViewState("CHKALL") Is Nothing Then ViewState("CHKALL") = False
            Return DirectCast(ViewState("CHKALL"), Boolean)
        End Get
        Set(value As Boolean)
            ViewState("CHKALL") = value
        End Set
    End Property

#End Region

#Region " Types "
    Private Enum CatRischioGridColumnIndex
        Selezione = 0
        Codice = 1
        Descrizione = 2
    End Enum

#End Region

#Region " Eventi Page "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        PanelUtility = New OnitDataPanelUtility(ToolBar)
        PanelUtility.FindButtonName = "btnCerca"
        PanelUtility.DeleteButtonName = "btnElimina"
        PanelUtility.SaveButtonName = "btnSalva"
        PanelUtility.CancelButtonName = "btnAnnulla"
        PanelUtility.OtherButtonsNames = "btnVaccinazioni"
        PanelUtility.MasterDataPanel = odpRischioMaster
        PanelUtility.DetailDataPanel = odpRischioDetail
        PanelUtility.WZMsDataGrid = dgrCatRischio
        PanelUtility.WZRicBase = WzFilter1.FindControl("WzFilterKeyBase")
        PanelUtility.SetToolbarButtonImages()

        AddHandler PanelUtility.BeforeSave, AddressOf PanelUtility_BeforeSave

        If Not IsPostBack Then
            PanelUtility.InitToolbar()
            OnitLayout31.lock.EndLock(OnVacUtility.Variabili.PazId)
        End If

        Select Case Request.Form.Item("__EVENTTARGET")

            Case "selectAll"
                SelectAll()

        End Select

    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Me.PanelUtility.ManagingToolbar(e.Button.Key)

        'richiamo sottoanagrafe dei cicli (modifica 30/08/2004)
        Select Case e.Button.Key

            Case "btnCicli"
                'Maurizio 23-05-05
                'Utilizzo il metodo HttpUtility.UrlEncode per far sì che vengano accettate stringhe che contengono caratteri speciali (ad esempio un +)
                'Server.Transfer("RischioCicli.aspx?codCatRischio=" & odpRischioMaster.getCurrentDataRow.Item("rsc_codice"), False)
                Server.Transfer("RischioCicli.aspx?codCatRischio=" & HttpUtility.UrlEncode(odpRischioMaster.getCurrentDataRow.Item("rsc_codice")), False)
                ' TODO: Gestione da terminare?

            Case "btnVaccinazioni"

                Me.OnitLayout31.Busy = True
                Me.fmVaccinazioni.VisibileMD = True

                Dim vaccinazioni As New List(Of VaccinazioneAssociabile)

                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                    Using bizAnag As New Biz.BizVaccinazioniAssociate(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                        vaccinazioni = bizAnag.GetVaccinazioniAssociabili()

                        dgrVaccinazioni.DataSource = vaccinazioni
                        dgrVaccinazioni.DataBind()

                        Dim listVacAssociate As List(Of String) = bizAnag.GetVaccinazioniAssociateACategoriaRischio(CodCatRischio)
                        SetCodiciVaccinazioniSelezionate(listVacAssociate)

                    End Using
                End Using
        End Select

    End Sub

    Private Sub tlbVaccinazioni_ButtonClicked(sender As Object, be As ButtonEvent) Handles tlbVaccinazioni.ButtonClicked

        Select Case be.Button.Key

            Case "btnSalvaVaccinazioni"
                Dim codVac As List(Of String) = GetCodiciVaccinazioniSelezionate()
                Salva(codVac)
                OnitLayout31.Busy = False
                Me.fmVaccinazioni.VisibileMD = False

            Case "btnAnnullaVaccinazioni"
                Me.OnitLayout31.Busy = False
                Me.fmVaccinazioni.VisibileMD = False

        End Select

    End Sub

#End Region

#Region " Eventi OnitDataPanel e PanelDetail "

    Private Sub PanelUtility_BeforeSave(sender As Object, e As OnitDataPanelUtility.BeforeOperationEventArgs)

        If Me.odpRischioDetail.CurrentOperation = Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord Then
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

    Private Sub odpRischioMaster_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpRischioMaster.afterOperation
        Me.OnitLayout31.Busy = PanelUtility.CheckToolBarState(operation)
    End Sub

    Private Sub odpRischioMaster_onCreateQuery(ByRef QB As Object) Handles odpRischioMaster.onCreateQuery
        DirectCast(QB, AbstractQB).AddOrderByFields("RSC_DESCRIZIONE")
    End Sub

    'nel caso in cui sia impossibile eliminare una categoria rischio già utilizzata (modifica 30/12/2004
    Private Sub odpRischioMaster_onError(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, err As Onit.Controls.OnitDataPanel.OnitDataPanelError) Handles odpRischioMaster.onError

        If Not err.exc Is Nothing AndAlso err.exc.Message.Contains(Constants.OracleErrors.ORA_02292) Then
            err.exc = New Exception(" (le categorie rischio risultano già utilizzate nel programma)")
            err.comment = "Attenzione: non è stato possibile eliminare le categorie rischio selezionate!"
        End If

    End Sub

    Private Sub odpRischioDetail_afterSaveData(sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles odpRischioDetail.afterSaveData
        Me.odpRischioMaster.Find()
    End Sub

    Private Sub odpRischioDetail_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpRischioDetail.afterOperation
        Me.OnitLayout31.Busy = PanelUtility.CheckToolBarState(operation)
    End Sub

#End Region

#Region " Private Modale Vaccinazioni "

    Private Sub Salva(codVac As List(Of String))

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using biz As New Biz.BizVaccinazioniAssociate(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                biz.SalvaVaccinazioniAssociateACategoriaRischio(CodCatRischio, codVac)

            End Using
        End Using

    End Sub

    Private Function GetCodiciVaccinazioniSelezionate() As List(Of String)

        Dim listCodVac As New List(Of String)()
        For Each item As DataGridItem In Me.dgrVaccinazioni.Items

            If DirectCast(item.FindControl("chkSelezioneItem"), CheckBox).Checked Then

                Dim key As String = Me.dgrVaccinazioni.DataKeys(item.ItemIndex).ToString()
                listCodVac.Add(key)

            End If

        Next

        Return listCodVac

    End Function

    Private Sub SetCodiciVaccinazioniSelezionate(codiciVac As List(Of String))

        For Each item As DataGridItem In Me.dgrVaccinazioni.Items

            Dim checked As Boolean = False
            Dim key As String = Me.dgrVaccinazioni.DataKeys(item.ItemIndex).ToString()

            If codiciVac.Contains(key) Then
                checked = True
            End If

            DirectCast(item.FindControl("chkSelezioneItem"), CheckBox).Checked = checked

        Next

    End Sub

    ''' <summary>
    ''' Click del checkbox nell'header del datagrid per selezionare/deselezionare tutte le righe del datagrid
    ''' </summary>
    Private Sub SelectAll()

        OnitLayout31.Busy = True

        Dim selezionaTutti As Boolean = False
        Dim listIdVac As List(Of String) = New List(Of String)()

        If Not Boolean.TryParse(Request.Form.Item("__EVENTARGUMENT"), selezionaTutti) Then
            Return
        End If

        Me.SelezionaTuttiChecked = selezionaTutti

        If Me.SelezionaTuttiChecked Then

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using biz As New Biz.BizVaccinazioniAssociate(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                    Dim listVacAssociabili As List(Of VaccinazioneAssociabile) = biz.GetVaccinazioniAssociabili()

                    listIdVac = listVacAssociabili.Select(Function(v) v.CodiceVac).ToList()

                End Using
            End Using

        End If

        If Not listIdVac Is Nothing Then
            SetCodiciVaccinazioniSelezionate(listIdVac)
        End If

    End Sub

#End Region

End Class
