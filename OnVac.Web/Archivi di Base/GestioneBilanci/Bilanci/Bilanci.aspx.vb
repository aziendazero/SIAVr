Imports System.Collections.Generic
Imports Infragistics.WebUI.UltraWebGrid
Imports Onit.Database.DataAccessManager


Partial Class OnVac_Bilanci
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

#Region " Types "

    Private Enum GridSezioniColumnIndex
        CheckSelezione = 0
        Ordine = 1
        Descrizione = 2
        Codice = 3
    End Enum

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

        Me.PanelUtility = New OnitDataPanelUtility(Me.ToolBarBilanci)

        Me.PanelUtility.FindButtonName = "btnCerca"
        Me.PanelUtility.DeleteButtonName = "btnElimina"
        Me.PanelUtility.SaveButtonName = "btnSalva"
        Me.PanelUtility.CancelButtonName = "btnAnnulla"
        Me.PanelUtility.OtherButtonsNames = "btnLinkOsservazioni"
        Me.PanelUtility.MasterDataPanel = Me.OdpBilanciMaster
        Me.PanelUtility.DetailDataPanel = Me.OdpBilanciDetail
        Me.PanelUtility.WZDataGrid = Me.WzDgrBilanci
        Me.PanelUtility.WZRicBase = Me.WzFilter1.FindControl("WzFilterKeyBase")
        Me.PanelUtility.SetToolbarButtonImages()

        If Not IsPostBack() Then

            Me.PanelUtility.InitToolbar()

            'slokkiamo il paziente lokkato...
            Me.OnitLayout31.lock.EndLock(OnVacUtility.Variabili.PazId)

        End If

    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub ToolBarBilanci_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBarBilanci.ButtonClicked

        Me.PanelUtility.ManagingToolbar(e.Button.Key)

        Select Case e.Button.Key

            Case "btnNew"

                Me.WzNumeroSolleciti.Text = 0

            Case "btnLinkOsservazioni"

                If Not String.IsNullOrWhiteSpace(Me.WzTxtNumero.Text) AndAlso Not String.IsNullOrWhiteSpace(Me.WzTxtMalattie.Codice) Then
                    Response.Redirect(String.Format("Bilanci-Osservazioni.aspx?CODICE={0}|{1}|{2}",
                                                    HttpUtility.UrlEncode(Me.WzTxtNumero.Text),
                                                    HttpUtility.UrlEncode(Me.WzTxtMalattie.Codice),
                                                    HttpUtility.UrlEncode(Me.WzTxtMalattie.Descrizione)),
                                                    False)
                End If

            Case "btnSezioni"

                OpenModaleSezioni()

        End Select

    End Sub

    Private Sub tlbSezioni_ButtonClicked(sender As System.Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles tlbSezioni.ButtonClicked

        Select Case be.Button.Key

            Case "btnNew"

                NuovaSezione()

            Case "btnElimina"

                EliminaSezioni()

            Case "btnSalva"

                Dim result As Biz.BizGenericResult = SalvaSezioni()

                If result.Success Then
                    CloseModaleSezioni()
                Else
                    Me.OnitLayout31.InsertRoutineJS(String.Format("alert('{0}');", HttpUtility.JavaScriptStringEncode(result.Message)))
                End If

            Case "btnAnnulla"

                CloseModaleSezioni()

        End Select

    End Sub

#End Region

#Region " Eventi OnitDataPanel "

    Private Sub odpBilanciDetail_afterSaveData(sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles OdpBilanciDetail.afterSaveData
        Me.OdpBilanciMaster.Find()
    End Sub

    Private Sub odpBilanciDetail_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles OdpBilanciDetail.afterOperation

        Select Case operation
            Case Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.CancelRecord
                Me.AbilitaEta(False)
            Case Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.DuplicateRecord
                Me.AbilitaEta(True)
            Case Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.EditRecord
                Me.AbilitaEta(True)
            Case Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord
                Me.AbilitaEta(True)
            Case Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.SaveRecord
                Me.AbilitaEta(False)
        End Select

        Me.OnitLayout31.Busy = Me.PanelUtility.CheckToolBarState(operation)

    End Sub

    Private Sub OdpBilanciDetail_afterUpdateWzControls(sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles OdpBilanciDetail.afterUpdateWzControls
        ImpostaEta(Me.tbMinEta.Text, Me.tbMinAnni, Me.tbMinMesi, Me.tbMinGiorni)
        ImpostaEta(Me.tbMaxEta.Text, Me.tbMaxAnni, Me.tbMaxMesi, Me.tbMaxGiorni)
        ImpostaEta(Me.wzTbBilSucc.Text, Me.tbBilSuccAnni, Me.tbBilSuccMesi, Me.tbBilSuccGiorni)
        ImpostaEta(Me.wzTbTempoSollecito.Text, Me.tbTempoSollecitoAnni, Me.tbTempoSollecitoMesi, Me.tbTempoSollecitoGiorni)
        ImpostaEta(Me.wzTbTempoCnvPrec.Text, Me.tbTempoCnvPrecAnni, Me.tbTempoCnvPrecMesi, Me.tbTempoCnvPrecGiorni)
        ImpostaEta(Me.wzTbScadDopo.Text, Me.tbScadDopoAnni, Me.tbScadDopoMesi, Me.tbScadDopoGiorni)
    End Sub

    Private Sub OdpBilanciMaster_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles OdpBilanciMaster.afterOperation

        Me.OnitLayout31.Busy = Me.PanelUtility.CheckToolBarState(operation)

    End Sub

    Private Sub OdpBilanciDetail_BeforeUpdateFromControls(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, currRow As System.Data.DataRow, encoder As Onit.Controls.OnitDataPanel.FieldsEncoder) Handles OdpBilanciDetail.BeforeUpdateFromControls

        If Me.OdpBilanciDetail.CurrentOperation = Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.SaveRecord Then
            Me.tbMinEta.Text = New Entities.Eta(Me.tbMinGiorni.Text, Me.tbMinMesi.Text, Me.tbMinAnni.Text).GiorniTotali()
            Me.tbMaxEta.Text = New Entities.Eta(Me.tbMaxGiorni.Text, Me.tbMaxMesi.Text, Me.tbMaxAnni.Text).GiorniTotali()
            Me.wzTbBilSucc.Text = New Entities.Eta(Me.tbBilSuccGiorni.Text, Me.tbBilSuccMesi.Text, Me.tbBilSuccAnni.Text).GiorniTotali()
            Me.wzTbTempoSollecito.Text = New Entities.Eta(Me.tbTempoSollecitoGiorni.Text, Me.tbTempoSollecitoMesi.Text, Me.tbTempoSollecitoAnni.Text).GiorniTotali()
            Me.wzTbTempoCnvPrec.Text = New Entities.Eta(Me.tbTempoCnvPrecGiorni.Text, Me.tbTempoCnvPrecMesi.Text, Me.tbTempoCnvPrecAnni.Text).GiorniTotali()
            Me.wzTbScadDopo.Text = New Entities.Eta(Me.tbScadDopoGiorni.Text, Me.tbScadDopoMesi.Text, Me.tbScadDopoAnni.Text).GiorniTotali()
        End If

    End Sub

    Private Sub OdpBilanciMaster_onCreateQuery(ByRef QB As System.Object) Handles OdpBilanciMaster.onCreateQuery

        DirectCast(QB, AbstractQB).AddOrderByFields("mal_descrizione, bil_numero")

    End Sub

    'nel caso in cui sia impossibile eliminare un bilancio già utilizzato (modifica 30/12/2004)
    Private Sub OdpBilanciMaster_onError(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, err As Onit.Controls.OnitDataPanel.OnitDataPanelError) Handles OdpBilanciMaster.onError

        If Not err.exc Is Nothing AndAlso err.exc.Message.Contains(Constants.OracleErrors.ORA_02292) Then
            err.exc = New Exception(" (i bilanci risultano già utilizzati nel programma)")
            err.comment = "Attenzione: non è stato possibile eliminare i bilanci selezionati!"
        End If

    End Sub

#End Region

#Region " Eventi Datagrid "

    Private Sub WzDgrBilanci_InitializeRow(sender As Object, e As Infragistics.WebUI.UltraWebGrid.RowEventArgs) Handles WzDgrBilanci.InitializeRow

        Dim row As DataRowView = e.Data
        Dim dgr As UltraWebGrid = sender

        Dim cellEtaMin As CellItem = DirectCast(DirectCast(dgr.Columns.FromKey("ETAMINIMA"), TemplatedColumn).CellItems(e.Row.Index), CellItem)
        Dim cellEtaMax As CellItem = DirectCast(DirectCast(dgr.Columns.FromKey("ETAMASSIMA"), TemplatedColumn).CellItems(e.Row.Index), CellItem)

        Dim etaMin As New Entities.Eta(row("BIL_ETA_MINIMA"))
        Dim etaMax As New Entities.Eta(row("BIL_ETA_MASSIMA"))

        DirectCast(cellEtaMin.FindControl("minAnni"), Label).Text = etaMin.Anni
        DirectCast(cellEtaMin.FindControl("minMesi"), Label).Text = etaMin.Mesi
        DirectCast(cellEtaMin.FindControl("minGiorni"), Label).Text = etaMin.Giorni
        DirectCast(cellEtaMax.FindControl("maxAnni"), Label).Text = etaMax.Anni
        DirectCast(cellEtaMax.FindControl("maxMesi"), Label).Text = etaMax.Mesi
        DirectCast(cellEtaMax.FindControl("maxGiorni"), Label).Text = etaMax.Giorni

    End Sub

#End Region

#Region " Private Methods "

    Private Sub ImpostaEta(strEta As String, ByRef txtAnni As TextBox, ByRef txtMesi As TextBox, ByRef txtGiorni As TextBox)

        Dim eta As New Entities.Eta(strEta)

        txtAnni.Text = eta.Anni
        txtMesi.Text = eta.Mesi
        txtGiorni.Text = eta.Giorni

    End Sub

    Private Sub AbilitaEta(abilita As Boolean)
        AbilitaEta(abilita, Me.tbMinAnni, Me.tbMinMesi, Me.tbMinGiorni)
        AbilitaEta(abilita, Me.tbMaxAnni, Me.tbMaxMesi, Me.tbMaxGiorni)
        AbilitaEta(abilita, Me.tbBilSuccAnni, Me.tbBilSuccMesi, Me.tbBilSuccGiorni)
        AbilitaEta(abilita, Me.tbTempoSollecitoAnni, Me.tbTempoSollecitoMesi, Me.tbTempoSollecitoGiorni)
        AbilitaEta(abilita, Me.tbTempoCnvPrecAnni, Me.tbTempoCnvPrecMesi, Me.tbTempoCnvPrecGiorni)
        AbilitaEta(abilita, Me.tbScadDopoAnni, Me.tbScadDopoMesi, Me.tbScadDopoGiorni)
    End Sub

    Private Sub AbilitaEta(abilita As Boolean, ByRef txtAnni As TextBox, ByRef txtMesi As TextBox, ByRef txtGiorni As TextBox)

        Dim css As String = "textbox_numerico w40PX"
        If Not abilita Then css = "textbox_numerico_disabilitato w40PX"

        txtAnni.CssClass = css
        txtAnni.ReadOnly = Not abilita

        txtMesi.CssClass = css
        txtMesi.ReadOnly = Not abilita

        txtGiorni.CssClass = css
        txtGiorni.ReadOnly = Not abilita

    End Sub

#Region " Sezioni "

    Private Sub dgrSezioni_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrSezioni.ItemDataBound

        Select Case e.Item.ItemType

            Case ListItemType.Item, ListItemType.AlternatingItem, ListItemType.EditItem, ListItemType.SelectedItem

                Dim list As List(Of Entities.BilancioSezione) = DirectCast(dgrSezioni.DataSource, List(Of Entities.BilancioSezione))

                If Not list.IsNullOrEmpty() Then

                    If e.Item.ItemIndex = 0 Then

                        Dim btnUp As ImageButton = DirectCast(e.Item.FindControl("btnUp"), ImageButton)

                        btnUp.Enabled = False
                        btnUp.ImageUrl = ResolveClientUrl("~/images/arrow_blue12_up_dis.png")
                        btnUp.ToolTip = String.Empty

                    End If

                    If e.Item.ItemIndex = list.Count - 1 Then

                        Dim btnDown As ImageButton = DirectCast(e.Item.FindControl("btnDown"), ImageButton)

                        btnDown.Enabled = False
                        btnDown.ImageUrl = ResolveClientUrl("~/images/arrow_blue12_down_dis.png")
                        btnDown.ToolTip = String.Empty

                    End If

                End If

        End Select

    End Sub

    Private Sub dgrSezioni_ItemCommand(source As Object, e As DataGridCommandEventArgs) Handles dgrSezioni.ItemCommand

        Dim sourceIndex As Integer = e.Item.ItemIndex

        Dim destinationIndex As Integer

        Select Case e.CommandName

            Case "MoveUp"

                destinationIndex = sourceIndex - 1

            Case "MoveDown"

                destinationIndex = sourceIndex + 1

        End Select

        SwitchSezioni(sourceIndex, destinationIndex)

    End Sub

    Private Sub SwitchSezioni(sourceIndex As Integer, destinationIndex As Integer)

        Dim sezioni As List(Of Entities.BilancioSezione) = GetSezioni()

        Dim sezioneDaSpostare As Entities.BilancioSezione = sezioni(sourceIndex).Clone()

        sezioni.RemoveAt(sourceIndex)

        sezioni.Insert(destinationIndex, sezioneDaSpostare)

        Me.dgrSezioni.DataSource = sezioni
        Me.dgrSezioni.DataBind()

    End Sub

    Private Sub OpenModaleSezioni()

        Dim numeroBilancio As Integer = 0

        If Not String.IsNullOrWhiteSpace(Me.WzTxtNumero.Text) Then

            If Not Integer.TryParse(Me.WzTxtNumero.Text, numeroBilancio) Then
                numeroBilancio = 0
            End If

        End If

        Dim codiceMalattia As String = Me.WzTxtMalattie.Codice

        Me.hidNumeroBilancioSelezionato.Value = numeroBilancio.ToString()
        Me.hidCodiceMalattiaSelezionata.Value = codiceMalattia

        If numeroBilancio > 0 AndAlso Not String.IsNullOrWhiteSpace(codiceMalattia) Then

            Dim listSezioni As List(Of Entities.BilancioSezione) = Nothing

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizAnaBilanci As New Biz.BizAnaBilanci(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                    listSezioni = bizAnaBilanci.GetSezioniBilancio(numeroBilancio, codiceMalattia, False)

                End Using
            End Using

            Me.dgrSezioni.DataSource = listSezioni
            Me.dgrSezioni.DataBind()

            Me.fmSezioni.VisibileMD = True

        End If

    End Sub

    Private Sub CloseModaleSezioni()

        Me.hidNumeroBilancioSelezionato.Value = String.Empty
        Me.hidCodiceMalattiaSelezionata.Value = String.Empty

        Me.fmSezioni.VisibileMD = False

    End Sub

    Private Function GetNumeroBilancioSelezionato() As Integer

        Dim numeroBilancio As Integer = 0

        If Not String.IsNullOrWhiteSpace(Me.hidNumeroBilancioSelezionato.Value) Then

            If Not Integer.TryParse(Me.hidNumeroBilancioSelezionato.Value, numeroBilancio) Then
                numeroBilancio = 0
            End If

        End If

        Return numeroBilancio

    End Function

    Private Function GetSezioni() As List(Of Entities.BilancioSezione)

        Dim numeroBilancio As Integer = GetNumeroBilancioSelezionato()
        Dim codiceMalattia As String = Me.hidCodiceMalattiaSelezionata.Value

        Dim list As New List(Of Entities.BilancioSezione)()

        For Each gridItem As DataGridItem In dgrSezioni.Items

            list.Add(CreateBilancioSezione(gridItem, numeroBilancio, codiceMalattia))

        Next

        Return list

    End Function

    Private Function CreateBilancioSezione(gridItem As DataGridItem, numeroBilancio As Integer, codiceMalattia As String) As Entities.BilancioSezione

        Dim item As New Entities.BilancioSezione()

        item.CodiceSezione = HttpUtility.HtmlDecode(gridItem.Cells(GridSezioniColumnIndex.Codice).Text).Trim()
        item.DescrizioneSezione = DirectCast(gridItem.FindControl("txtDescrizione"), TextBox).Text
        item.Ordine = gridItem.ItemIndex + 1
        item.NumeroBilancio = numeroBilancio
        item.CodiceMalattia = codiceMalattia

        Return item

    End Function

    Private Sub NuovaSezione()

        Dim list As List(Of Entities.BilancioSezione) = GetSezioni()

        Dim numeroBilancio As Integer = GetNumeroBilancioSelezionato()
        Dim codiceMalattia As String = Me.hidCodiceMalattiaSelezionata.Value

        Dim item As New Entities.BilancioSezione()

        item.CodiceSezione = String.Empty
        item.DescrizioneSezione = String.Empty
        item.Ordine = list.Count + 1
        item.NumeroBilancio = numeroBilancio
        item.CodiceMalattia = codiceMalattia

        list.Add(item)

        Me.dgrSezioni.DataSource = list
        Me.dgrSezioni.DataBind()

    End Sub

    Private Sub EliminaSezioni()

        Dim numeroBilancio As Integer = GetNumeroBilancioSelezionato()
        Dim codiceMalattia As String = Me.hidCodiceMalattiaSelezionata.Value

        Dim list As New List(Of Entities.BilancioSezione)()

        For Each gridItem As DataGridItem In dgrSezioni.Items

            ' Gli elementi selezionati vengono eliminati: creo una lista con solo gli elementi NON selezionati.

            Dim chkSelezione As CheckBox = DirectCast(gridItem.FindControl("chkSelezione"), CheckBox)

            If Not chkSelezione Is Nothing AndAlso Not chkSelezione.Checked Then

                list.Add(CreateBilancioSezione(gridItem, numeroBilancio, codiceMalattia))

            End If

        Next

        Me.dgrSezioni.DataSource = list
        Me.dgrSezioni.DataBind()

    End Sub

    Private Function SalvaSezioni() As Biz.BizGenericResult

        Dim listSezioniDaSalvare As List(Of Entities.BilancioSezione) = GetSezioni()

        Dim numeroBilancio As Integer = GetNumeroBilancioSelezionato()
        Dim codiceMalattia As String = Me.hidCodiceMalattiaSelezionata.Value

        Dim result As Biz.BizGenericResult = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using biz As New Biz.BizAnaBilanci(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                result = biz.SalvaSezioni(numeroBilancio, codiceMalattia, listSezioniDaSalvare)

            End Using
        End Using

        Return result

    End Function

#End Region

#End Region

End Class
