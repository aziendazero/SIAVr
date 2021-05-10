Imports System.Collections.Generic
Imports Onit.Controls.PagesLayout


Public Class ElencoCondizioniPagamento
    Inherits Common.UserControlPageBase

#Region " Types "

    Private Enum LayoutStatus
        View = 0
        Insert = 1
        Edit = 2
    End Enum

    Private Enum DgrColumnIndex
        SelectionColumn = 0
        EtaInizio = 1
        EtaFine = 2
        CodEsenzione = 3
        Importo = 4
        CaricamentoAutomaticoImporto = 5
        IdCondizione = 6
    End Enum

#End Region

#Region " Properties "

    Private Property StatoLayout() As LayoutStatus
        Get
            '--
            If ViewState("SL") Is Nothing Then ViewState("SL") = LayoutStatus.View
            '--
            Return DirectCast(ViewState("SL"), LayoutStatus)
            '--
        End Get
        Set(value As LayoutStatus)
            '--
            ViewState("SL") = value
            '--
            Dim isInEdit As Boolean = (value <> LayoutStatus.View)
            '--
            Me.tlbCondizioni.Items.FromKeyButton("btnChiudi").Enabled = Not isInEdit
            Me.tlbCondizioni.Items.FromKeyButton("btnNuovo").Enabled = Not isInEdit
            Me.tlbCondizioni.Items.FromKeyButton("btnModifica").Enabled = Not isInEdit AndAlso Me.dgrCondizioni.Items.Count > 0 AndAlso Me.dgrCondizioni.SelectedIndex >= 0
            Me.tlbCondizioni.Items.FromKeyButton("btnElimina").Enabled = Not isInEdit AndAlso Me.dgrCondizioni.Items.Count > 0 AndAlso Me.dgrCondizioni.SelectedIndex >= 0
            Me.tlbCondizioni.Items.FromKeyButton("btnSalva").Enabled = isInEdit
            Me.tlbCondizioni.Items.FromKeyButton("btnAnnulla").Enabled = isInEdit
            '--
            Me.ucEtaInizio.Enabled = isInEdit
            Me.ucEtaFine.Enabled = isInEdit
            Me.ddlEsenzione.Enabled = isInEdit
            Me.ddlImporto.Enabled = isInEdit
            Me.chkAutoImporto.Enabled = isInEdit
            '--
        End Set
    End Property

    Private Property CodiceNomeCommercialeCorrente() As String
        Get
            Return ViewState("NOC")
        End Get
        Set(value As String)
            ViewState("NOC") = value
        End Set
    End Property

#End Region

#Region " Eventi "

    Public Event Close()
    Public Event ShowMessage(message As String)

#End Region

#Region " Eventi Controlli "

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

    End Sub

    Private Sub ToolBar_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles tlbCondizioni.ButtonClicked

        Select Case be.Button.Key

            Case "btnChiudi"

                RaiseEvent Close()

            Case "btnNuovo"

                BindCombo(Me.ddlEsenzione, False)
                BindCombo(Me.ddlImporto, False)
                ClearDettaglio()
                Me.dgrCondizioni.SelectedIndex = -1

                Me.StatoLayout = LayoutStatus.Insert

            Case "btnModifica"

                If Me.dgrCondizioni.SelectedItem Is Nothing Then
                    Me.Onitlayout31.ShowMsgBox(New OnitLayout3.OnitLayoutMsgBox("Impossibile modificare: nessun elemento selezionato.", "ERR", False, False))
                    Return
                End If

                Me.StatoLayout = LayoutStatus.Edit

            Case "btnElimina"

                If EliminaSelezionato() Then
                    BindData()
                    Me.StatoLayout = LayoutStatus.View
                End If

            Case "btnSalva"

                Dim result As Biz.BizGenericResult = Salva()

                If Not result.Success Then
                    Me.Onitlayout31.ShowMsgBox(New OnitLayout3.OnitLayoutMsgBox(result.Message, "ERR", False, False))
                    Return
                End If

                BindData()
                Me.StatoLayout = LayoutStatus.View

            Case "btnAnnulla"

                BindData()
                Me.StatoLayout = LayoutStatus.View

        End Select

    End Sub

#Region " Eventi Datagrid "

    Private Sub dgrCondizioni_ItemDataBound(sender As Object, e As DataGridItemEventArgs) Handles dgrCondizioni.ItemDataBound

        If e.Item.DataItem Is Nothing Then Return

        Select Case e.Item.ItemType

            Case ListItemType.Item, ListItemType.AlternatingItem, ListItemType.EditItem, ListItemType.SelectedItem

                Dim condizionePagamento As Entities.CondizioniPagamento = DirectCast(e.Item.DataItem, Entities.CondizioniPagamento)

                Dim lbl As Label = DirectCast(e.Item.FindControl("lblEtaInizio"), Label)
                If Not lbl Is Nothing Then lbl.Text = GetDescrizioneEta(condizionePagamento.EtaInizio)

                lbl = DirectCast(e.Item.FindControl("lblEtaFine"), Label)
                If Not lbl Is Nothing Then lbl.Text = GetDescrizioneEta(condizionePagamento.EtaFine)

                lbl = DirectCast(e.Item.FindControl("lblCodEsenzione"), Label)
                If Not lbl Is Nothing Then lbl.Text = GetDescrizioneStatoAbilitazione(condizionePagamento.StatoAbilitazioneEsenzione)

                lbl = DirectCast(e.Item.FindControl("lblImporto"), Label)
                If Not lbl Is Nothing Then lbl.Text = GetDescrizioneStatoAbilitazione(condizionePagamento.StatoAbilitazioneImporto)

                lbl = DirectCast(e.Item.FindControl("lblAutoImporto"), Label)
                If Not lbl Is Nothing Then lbl.Text = IIf(condizionePagamento.ImpostazioneAutomaticaImportoInEsecuzione, "SI", "NO")

        End Select

    End Sub

    Private Sub dgrCondizioni_ItemCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgrCondizioni.ItemCommand

        Select Case e.CommandName

            Case "Select"

                Dim value As String = Me.dgrCondizioni.Items(e.Item.ItemIndex).Cells(DgrColumnIndex.IdCondizione).Text
                If Not String.IsNullOrEmpty(value) Then
                    SetDettaglio(Convert.ToInt32(value))
                End If

        End Select

    End Sub

#End Region

#End Region

#Region " Public "

    Public Sub CaricaDati(codiceNomeCommerciale As String)

        Me.CodiceNomeCommercialeCorrente = codiceNomeCommerciale
        Dim descrizioneNomeCommerciale As String = GetDescrizioneNomeCommercialeCorrente()
        Me.lblTitolo.Text = String.Format("Nome Commerciale: {0} - {1}", Me.CodiceNomeCommercialeCorrente, descrizioneNomeCommerciale)

        BindData()
        Me.StatoLayout = LayoutStatus.View

    End Sub

#End Region

#Region " Private "

    Private Function GetDescrizioneNomeCommercialeCorrente() As String

        Dim descrizione As String = String.Empty

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizNomiCommerciali As New Biz.BizNomiCommerciali(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                descrizione = bizNomiCommerciali.GetDescrizioneNomeCommerciale(Me.CodiceNomeCommercialeCorrente)

            End Using
        End Using

        Return descrizione

    End Function

    Private Sub BindCombo(ddl As DropDownList, includeEmptyItem As Boolean)

        ddl.ClearSelection()
        ddl.Items.Clear()

        If includeEmptyItem Then ddl.Items.Add(New ListItem(String.Empty, String.Empty))

        ddl.Items.Add(CreateListItemFromStatoAbilitazione(Enumerators.StatoAbilitazioneCampo.Disabilitato))
        ddl.Items.Add(CreateListItemFromStatoAbilitazione(Enumerators.StatoAbilitazioneCampo.Abilitato))
        ddl.Items.Add(CreateListItemFromStatoAbilitazione(Enumerators.StatoAbilitazioneCampo.Obbligatorio))

    End Sub

    Private Sub SetComboValue(ddl As DropDownList, value As Enumerators.StatoAbilitazioneCampo)

        ddl.ClearSelection()

        Dim item As ListItem = ddl.Items.FindByValue(value)
        If Not item Is Nothing Then
            item.Selected = True
        End If

    End Sub

    Private Function CreateListItemFromStatoAbilitazione(statoAbilitazione As Enumerators.StatoAbilitazioneCampo) As ListItem

        Dim item As New ListItem()
        item.Text = [Enum].GetName(GetType(Enumerators.StatoAbilitazioneCampo), statoAbilitazione)
        item.Value = statoAbilitazione

        Return item

    End Function

    Private Sub BindData()

        ' Caricamento dati
        Dim listCondizioniPagamento As List(Of Entities.CondizioniPagamento) = Nothing

        If Not String.IsNullOrEmpty(Me.CodiceNomeCommercialeCorrente) Then

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizNomiCommerciali As New Biz.BizNomiCommerciali(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                    listCondizioniPagamento = bizNomiCommerciali.GetListCondizioniPagamento(Me.CodiceNomeCommercialeCorrente)

                End Using
            End Using

        End If

        ' Bind DropDownList
        Dim sourceEmpty As Boolean = (listCondizioniPagamento Is Nothing OrElse listCondizioniPagamento.Count = 0)

        BindCombo(Me.ddlEsenzione, sourceEmpty)
        BindCombo(Me.ddlImporto, sourceEmpty)

        ' Bind Datagrid
        Me.dgrCondizioni.DataSource = listCondizioniPagamento
        Me.dgrCondizioni.DataBind()

        If sourceEmpty Then
            ClearDettaglio()
        Else
            Me.dgrCondizioni.SelectedIndex = 0
            Dim value As String = Me.dgrCondizioni.SelectedItem.Cells(DgrColumnIndex.IdCondizione).Text
            If Not String.IsNullOrEmpty(value) Then
                SetDettaglio(Convert.ToInt32(value))
            End If
        End If

    End Sub

    Private Function GetDescrizioneEta(giorniTotali As Integer?) As String

        If Not giorniTotali.HasValue Then Return String.Empty

        Dim eta As New Entities.Eta(giorniTotali.Value)

        Dim descrizioneEta As New System.Text.StringBuilder()

        If eta.Anni = 1 Then
            descrizioneEta.Append("1 anno, ")
        Else
            descrizioneEta.AppendFormat("{0} anni, ", eta.Anni)
        End If

        If eta.Mesi = 1 Then
            descrizioneEta.Append("1 mese, ")
        Else
            descrizioneEta.AppendFormat("{0} mesi, ", eta.Mesi)
        End If

        If eta.Giorni = 1 Then
            descrizioneEta.Append("1 giorno")
        Else
            descrizioneEta.AppendFormat("{0} giorni", eta.Giorni)
        End If

        Return descrizioneEta.ToString()

    End Function

    Private Function GetDescrizioneStatoAbilitazione(codiceStato As Enumerators.StatoAbilitazioneCampo?) As String

        If Not codiceStato.HasValue Then codiceStato = Enumerators.StatoAbilitazioneCampo.Disabilitato

        Return [Enum].GetName(GetType(Enumerators.StatoAbilitazioneCampo), codiceStato.Value).ToUpper()

    End Function

    Private Function GetCondizionePagamentoFromDettaglio() As Entities.CondizioniPagamento

        Dim condizionePagamento As New Entities.CondizioniPagamento()

        condizionePagamento.EtaInizio = Me.ucEtaInizio.GetGiorniTotali()
        condizionePagamento.EtaFine = Me.ucEtaFine.GetGiorniTotali()
        condizionePagamento.StatoAbilitazioneEsenzione = Me.ddlEsenzione.SelectedItem.Value
        condizionePagamento.StatoAbilitazioneImporto = Me.ddlImporto.SelectedItem.Value
        condizionePagamento.CodiceNomeCommerciale = Me.CodiceNomeCommercialeCorrente
        condizionePagamento.ImpostazioneAutomaticaImportoInEsecuzione = Me.chkAutoImporto.Checked

        If String.IsNullOrEmpty(Me.hidIdCondizione.Value) Then
            condizionePagamento.IdCondizione = Nothing
        Else
            condizionePagamento.IdCondizione = Convert.ToInt32(Me.hidIdCondizione.Value)
        End If

        Return condizionePagamento

    End Function

    Private Sub SetDettaglio(idCondizione As Integer)

        Dim condizionePagamento As Entities.CondizioniPagamento = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizNomiCommerciali As New Biz.BizNomiCommerciali(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                condizionePagamento = bizNomiCommerciali.GetCondizionePagamento(idCondizione)

            End Using
        End Using

        If condizionePagamento Is Nothing Then
            ClearDettaglio()
        Else
            Me.hidIdCondizione.Value = idCondizione.ToString()
            Me.ucEtaInizio.SetGiorni(condizionePagamento.EtaInizio)
            Me.ucEtaFine.SetGiorni(condizionePagamento.EtaFine)
            SetComboValue(Me.ddlEsenzione, condizionePagamento.StatoAbilitazioneEsenzione)
            SetComboValue(Me.ddlImporto, condizionePagamento.StatoAbilitazioneImporto)
            Me.chkAutoImporto.Checked = condizionePagamento.ImpostazioneAutomaticaImportoInEsecuzione
        End If

    End Sub

    Private Sub ClearDettaglio()
        Me.hidIdCondizione.Value = String.Empty
        Me.ucEtaInizio.SetGiorni(Nothing)
        Me.ucEtaFine.SetGiorni(Nothing)
        Me.ddlEsenzione.ClearSelection()
        Me.ddlImporto.ClearSelection()
        Me.chkAutoImporto.Checked = False
    End Sub

    Private Function Salva() As Biz.BizGenericResult

        Dim result As Biz.BizGenericResult

        Dim condizionePagamento As Entities.CondizioniPagamento = GetCondizionePagamentoFromDettaglio()

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizNomiCommerciali As New Biz.BizNomiCommerciali(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                result = bizNomiCommerciali.SaveCondizionePagamento(condizionePagamento)

            End Using
        End Using

        Return result

    End Function

    Private Function EliminaSelezionato() As Boolean

        If Me.dgrCondizioni.SelectedItem Is Nothing Then
            Me.Onitlayout31.ShowMsgBox(New OnitLayout3.OnitLayoutMsgBox("Impossibile effettuare l'eliminazione: nessun elemento selezionato.", "ERR", False, False))
            Return False
        End If

        Dim value As String = HttpUtility.HtmlDecode(Me.dgrCondizioni.SelectedItem.Cells(DgrColumnIndex.IdCondizione).Text).Trim()

        If String.IsNullOrEmpty(value) Then
            Me.Onitlayout31.ShowMsgBox(New OnitLayout3.OnitLayoutMsgBox("Impossibile effettuare l'eliminazione: nessun elemento selezionato.", "ERR", False, False))
            Return False
        End If

        Dim idCondizionePagamento As Integer = Convert.ToInt32(value)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizNomiCommerciali As New Biz.BizNomiCommerciali(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                bizNomiCommerciali.DeleteCondizionePagamento(idCondizionePagamento)

            End Using
        End Using

        Return True

    End Function

#End Region

End Class