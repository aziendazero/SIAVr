Imports System.Collections.Generic
Imports Onit.Controls.PagesLayout

Public Class Interventi
    Inherits OnVac.Common.PageBase

#Region " Types "

    Private Enum LayoutStatus
        View = 0
        Insert = 1
        Edit = 2
    End Enum

    Private Enum DgrColumnIndex
        Codice = 0
        Descrizione = 1
        Tipologia = 2
        Durata = 3
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
            Me.tlbInterventi.Items.FromKeyButton("btnCerca").Enabled = Not isInEdit
            Me.tlbInterventi.Items.FromKeyButton("btnNuovo").Enabled = Not isInEdit
            Me.tlbInterventi.Items.FromKeyButton("btnModifica").Enabled = Not isInEdit AndAlso Me.dgrInterventi.Items.Count > 0 AndAlso Me.dgrInterventi.SelectedIndex >= 0
            Me.tlbInterventi.Items.FromKeyButton("btnElimina").Enabled = Not isInEdit AndAlso Me.dgrInterventi.Items.Count > 0 AndAlso Me.dgrInterventi.SelectedIndex >= 0
            Me.tlbInterventi.Items.FromKeyButton("btnSalva").Enabled = isInEdit
            Me.tlbInterventi.Items.FromKeyButton("btnAnnulla").Enabled = isInEdit
            '--
            Me.txtCodice.ReadOnly = True
            Me.txtDescrizione.ReadOnly = Not isInEdit
            Me.ddlTipologia.Enabled = isInEdit
            Me.txtDurata.ReadOnly = Not isInEdit
            Me.dgrInterventi.Enabled = Not isInEdit

            Me.txtCodice.CssClass = "textbox_stringa_disabilitato"
            Me.txtDescrizione.CssClass = IIf(isInEdit, "textbox_stringa_obbligatorio", "textbox_stringa_disabilitato")
            Me.ddlTipologia.CssClass = IIf(isInEdit, "textbox_stringa_obbligatorio", "textbox_stringa_disabilitato")
            Me.txtDurata.CssClass = IIf(isInEdit, "textbox_stringa_obbligatorio", "textbox_stringa_disabilitato")

            Me.WzFilterKeyBase.Enabled = Not isInEdit
            '--
        End Set
    End Property

#End Region

#Region " Eventi Controlli "

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then

            BindData()
            Me.StatoLayout = LayoutStatus.View

        End If

    End Sub

    Private Sub dgrInterventi_ItemDataBound(sender As Object, e As DataGridItemEventArgs) Handles dgrInterventi.ItemDataBound

        If e.Item.DataItem Is Nothing Then Return

        Select Case e.Item.ItemType

            Case ListItemType.Item, ListItemType.AlternatingItem, ListItemType.EditItem, ListItemType.SelectedItem

                Dim intervento As Entities.Intervento = DirectCast(e.Item.DataItem, Entities.Intervento)

                Dim lbl As Label = DirectCast(e.Item.FindControl("lblTipologia"), Label)
                If Not lbl Is Nothing Then lbl.Text = Biz.BizInterventi.GetDescrizioneTipologiaIntervento(intervento.Tipologia)

        End Select

    End Sub

    Private Sub dgrInterventi_ItemCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgrInterventi.ItemCommand

        Select Case e.CommandName

            Case "Select"

                Dim value As String = Me.dgrInterventi.Items(e.Item.ItemIndex).Cells(DgrColumnIndex.Codice).Text
                If Not String.IsNullOrEmpty(value) Then
                    SetDettaglio(Convert.ToInt32(value))
                End If

        End Select

    End Sub

    Private Sub tlbInterventi_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles tlbInterventi.ButtonClicked

        Select Case be.Button.Key
            Case "btnCerca"
                BindData()

            Case "btnNuovo"
                BindCombo(Me.ddlTipologia, True)
                ClearDettaglio()
                Me.dgrInterventi.SelectedIndex = -1

                Me.StatoLayout = LayoutStatus.Insert

            Case "btnModifica"
                If Me.dgrInterventi.SelectedItem Is Nothing Then
                    Me.OnitLayout31.ShowMsgBox(New OnitLayout3.OnitLayoutMsgBox("Impossibile modificare: nessun elemento selezionato.", "ERR", False, False))
                    Return
                End If

                Me.StatoLayout = LayoutStatus.Edit

            Case "btnElimina"
                If EliminaSelezionato() Then
                    BindData()
                    Me.StatoLayout = LayoutStatus.View
                End If

            Case "btnSalva"

                If String.IsNullOrWhiteSpace(Me.txtDescrizione.Text) Or String.IsNullOrWhiteSpace(Me.txtDurata.Text) Or String.IsNullOrWhiteSpace(Me.ddlTipologia.SelectedItem.Value) Then
                    Me.OnitLayout31.ShowMsgBox(New OnitLayout3.OnitLayoutMsgBox("Valorizzare tutti i campi obbligatori.", "ERR", False, False))
                    Return
                End If

                Dim result As Biz.BizGenericResult = Salva()

                If Not result.Success Then
                    Me.OnitLayout31.ShowMsgBox(New OnitLayout3.OnitLayoutMsgBox(result.Message, "ERR", False, False))
                    Return
                End If

                BindData()
                Me.StatoLayout = LayoutStatus.View

            Case "btnAnnulla"
                BindData()
                Me.StatoLayout = LayoutStatus.View

        End Select
    End Sub

#End Region

#Region " Private "

    Private Sub BindCombo(ddl As DropDownList, includeEmptyItem As Boolean)

        ddl.ClearSelection()
        ddl.Items.Clear()

        If includeEmptyItem Then ddl.Items.Add(New ListItem(String.Empty, String.Empty))

        ddl.Items.Add(New ListItem(Biz.BizInterventi.GetDescrizioneTipologiaIntervento(Constants.TipologiaInterventi.Assistenziale), Constants.TipologiaInterventi.Assistenziale))
        ddl.Items.Add(New ListItem(Biz.BizInterventi.GetDescrizioneTipologiaIntervento(Constants.TipologiaInterventi.Amministrativo), Constants.TipologiaInterventi.Amministrativo))
        ddl.Items.Add(New ListItem(Biz.BizInterventi.GetDescrizioneTipologiaIntervento(Constants.TipologiaInterventi.Altro), Constants.TipologiaInterventi.Altro))

    End Sub

    Private Sub ClearDettaglio()
        Me.txtCodice.Text = String.Empty
        Me.txtDescrizione.Text = String.Empty
        Me.txtDurata.Text = String.Empty
        Me.ddlTipologia.ClearSelection()
    End Sub

    Private Function GetInterventoFromDettaglio() As Entities.Intervento

        Dim intervento As New Entities.Intervento()

        intervento.Descrizione = Me.txtDescrizione.Text
        intervento.Tipologia = Me.ddlTipologia.SelectedItem.Value

        If String.IsNullOrWhiteSpace(Me.txtCodice.Text) Then
            intervento.Codice = Nothing
        Else
            intervento.Codice = Convert.ToInt32(Me.txtCodice.Text)
        End If

        If String.IsNullOrWhiteSpace(Me.txtDurata.Text) Then
            intervento.Durata = Nothing
        Else
            intervento.Durata = Convert.ToInt32(Me.txtDurata.Text)
        End If

        Return intervento

    End Function

    Private Function Salva() As Biz.BizGenericResult

        Dim result As Biz.BizGenericResult

        Dim intervento As Entities.Intervento = GetInterventoFromDettaglio()

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizInterventi As New Biz.BizInterventi(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                result = bizInterventi.SalvaIntervento(intervento)

            End Using
        End Using

        Return result

    End Function

    Private Function EliminaSelezionato() As Boolean

        If Me.dgrInterventi.SelectedItem Is Nothing Then
            Me.OnitLayout31.ShowMsgBox(New OnitLayout3.OnitLayoutMsgBox("Impossibile effettuare l'eliminazione: nessun elemento selezionato.", "ERR", False, False))
            Return False
        End If

        Dim value As String = HttpUtility.HtmlDecode(Me.dgrInterventi.SelectedItem.Cells(DgrColumnIndex.Codice).Text).Trim()

        If String.IsNullOrEmpty(value) Then
            Me.OnitLayout31.ShowMsgBox(New OnitLayout3.OnitLayoutMsgBox("impossibile effettuare l'eliminazione: nessun elemento selezionato.", "err", False, False))
            Return False
        End If

        Dim idIntervento As Integer = Convert.ToInt32(value)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizInterventi As New Biz.BizInterventi(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                bizInterventi.EliminaIntervento(idIntervento)

            End Using
        End Using

        Return True

    End Function

    Private Sub BindData()

        ' Caricamento dati
        Dim listInterventi As List(Of Entities.Intervento) = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizInterventi As New Biz.BizInterventi(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                listInterventi = bizInterventi.GetInterventi(Me.WzFilterKeyBase.Text)

            End Using
        End Using

        Dim sourceEmpty As Boolean = (listInterventi Is Nothing OrElse listInterventi.Count = 0)

        'Bind dropdownlist Tipologia Intervento
        BindCombo(Me.ddlTipologia, True)

        ' Bind Datagrid
        Me.dgrInterventi.SelectedIndex = -1
        Me.dgrInterventi.DataSource = listInterventi
        Me.dgrInterventi.DataBind()

        If sourceEmpty Then
            ClearDettaglio()
        Else
            Me.dgrInterventi.SelectedIndex = 0
            Dim value As String = HttpUtility.HtmlDecode(Me.dgrInterventi.SelectedItem.Cells(DgrColumnIndex.Codice).Text)
            If Not String.IsNullOrEmpty(value) Then
                SetDettaglio(Convert.ToInt32(value))
            End If
        End If

    End Sub

    Private Sub SetDettaglio(idIntervento As Integer)

        Dim intervento As Entities.Intervento = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizInterventi As New Biz.BizInterventi(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                intervento = bizInterventi.GetIntervento(idIntervento)

            End Using
        End Using

        If intervento Is Nothing Then
            ClearDettaglio()
        Else
            Me.txtCodice.Text = idIntervento.ToString()
            Me.txtDescrizione.Text = intervento.Descrizione
            Me.txtDurata.Text = IIf(intervento.Durata.HasValue, intervento.Durata.ToString, String.Empty)
            SetComboValue(Me.ddlTipologia, intervento.Tipologia)
        End If

    End Sub

    Private Sub SetComboValue(ddl As DropDownList, value As String)

        ddl.ClearSelection()

        Dim item As ListItem = ddl.Items.FindByValue(value)
        If Not item Is Nothing Then
            item.Selected = True
        End If

    End Sub

#End Region
End Class