Imports System.Collections.Generic
Imports Onit.Web.UI.WebControls.Validators
Imports Onit.Controls.PagesLayout

Public Class InterventiPaziente
    Inherits Common.PageBase

#Region " Types "

    Protected Enum StatoPaginaInterventi
        Lettura = 0
        Modifica = 1
    End Enum

    Private Enum DgrColumnIndex
        Codice = 0
        BtnEditConferma = 1
        BtnDeleteAnnulla = 2
        Intervento = 3
        Data = 4
        Tipologia = 5
        Durata = 6
        Operatore = 7
        CheckOperatore = 8
        Note = 9
    End Enum

#End Region

#Region " Properties "

    Protected Property StatoPagina As StatoPaginaInterventi
        Get
            If ViewState("StatoPaginaInterventi") Is Nothing Then ViewState("StatoPaginaInterventi") = StatoPaginaInterventi.Lettura
            Return ViewState("StatoPaginaInterventi")
        End Get
        Set(value As StatoPaginaInterventi)
            ViewState("StatoPaginaInterventi") = value
            ImpostaLayoutInterventi(value)
        End Set
    End Property

    Protected ReadOnly Property OnitLayout As Controls.PagesLayout.OnitLayout3
        Get
            Return Me.OnitLayout31
        End Get
    End Property

    Private Property CampoOrdinamento As String
        Get
            If ViewState("CampoOrd") Is Nothing Then ViewState("CampoOrd") = String.Empty
            Return ViewState("CampoOrd")
        End Get
        Set(value As String)
            ViewState("CampoOrd") = value
        End Set
    End Property

    Private Property VersoOrdinamento As String
        Get
            If ViewState("VersoOrd") Is Nothing Then ViewState("VersoOrd") = String.Empty
            Return ViewState("VersoOrd")
        End Get
        Set(value As String)
            ViewState("VersoOrd") = value
        End Set
    End Property

#End Region

#Region " Page Events "

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        If Not IsPostBack Then

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                ' Impostazione titolo pagina e label dati paziente
                OnVacUtility.ImpostaIntestazioniPagina(OnitLayout31, LayoutTitolo, genericProvider, Settings, IsGestioneCentrale)

                'Caricamento dati griglia
                Inizializza()

            End Using

        End If

    End Sub

    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        SetImmagineOrdinamento()

    End Sub

#End Region

#Region " Eventi Controlli "

    Private Sub tlbInterventi_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles tlbInterventi.ButtonClicked

        Select Case be.Button.Key

            Case "btnNuovo"

                InserisciNuovoInterventoDataGrid()

        End Select

    End Sub

    Private Sub dgrInterventi_SortCommand(source As Object, e As DataGridSortCommandEventArgs) Handles dgrInterventi.SortCommand

        If e.SortExpression = CampoOrdinamento Then
            If VersoOrdinamento = Constants.VersoOrdinamento.Crescente Then
                VersoOrdinamento = Constants.VersoOrdinamento.Decrescente
            Else
                VersoOrdinamento = Constants.VersoOrdinamento.Crescente
            End If
        Else
            CampoOrdinamento = e.SortExpression
            VersoOrdinamento = Constants.VersoOrdinamento.Crescente
        End If

        ' Caricamento dati
        Dim listInterventi As List(Of Entities.InterventoPaziente) = CaricaInterventiPaziente()

        ' Bind Datagrid
        dgrInterventi.DataSource = listInterventi
        dgrInterventi.DataBind()

    End Sub

    Private Sub dgrInterventi_ItemCommand(source As Object, e As DataGridCommandEventArgs) Handles dgrInterventi.ItemCommand

        Select Case e.CommandName

            Case "EditRowInterventi"

                ModificaInterventoDataGrid(e)

            Case "CancelRowInterventi"

                'Caricamento dati griglia
                Inizializza()

            Case "ConfirmRowInterventi"

                If CheckValoriSalvataggio(e) Then

                    Dim intervento = UnbindDataGridItem(e.Item)
                    Dim result As Biz.BizGenericResult = SalvaInterventoDataGrid(intervento)

                    If result.Success Then
                        Inizializza()
                    End If

                End If

            Case "DeleteRowInterventi"

                EliminaInterventoDataGrid(e.Item.Cells(DgrColumnIndex.Codice).Text)

                'Caricamento dati aggiornati griglia
                Inizializza()

        End Select

    End Sub

    Private Sub dgrInterventi_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrInterventi.ItemDataBound
        '--
        Select Case e.Item.ItemType
            '--
            Case ListItemType.Item, ListItemType.AlternatingItem, ListItemType.EditItem, ListItemType.SelectedItem
                '--
                If Me.StatoPagina = StatoPaginaInterventi.Modifica Then
                    '--
                    Me.AddConfirmClickToImageButton(e.Item, "btnAnnullaGrid", "Annullare le modifiche?")
                    'Me.AddConfirmClickToImageButton(e.Item, "btnConfermaGrid", "Proseguire con il salvataggio?")
                    Me.DataGridItemDataBound(e.Item, Me.dgrInterventi.EditItemIndex)
                    '--
                Else
                    '--
                    Me.AddConfirmClickToImageButton(e.Item, "btnDeleteGrid", "Proseguire con l'eliminazione?")
                    '--
                End If
                '--
        End Select
        '--
    End Sub

    Protected Sub ddlIntervento_SelectedIndexChanged(sender As Object, e As EventArgs)

        'Precaricamento info legate all'intervento selezionato
        Dim ddlIntervento As DropDownList = DirectCast(sender, DropDownList)
        Dim selectedValue As String = ddlIntervento.SelectedValue

        Dim currentDataGridItem As DataGridItem = Me.dgrInterventi.Items(Me.dgrInterventi.EditItemIndex)

        If currentDataGridItem IsNot Nothing Then

            'Recupero i controlli da aggiornare
            Dim lblCodiceIntervento As Label = DirectCast(currentDataGridItem.FindControl("lblCodiceIntervento"), Label)
            Dim lblTipologia As Label = DirectCast(currentDataGridItem.FindControl("lblTipologia"), Label)
            Dim txtDurata As TextBox = DirectCast(currentDataGridItem.FindControl("txtDurata"), TextBox)

            If Not String.IsNullOrWhiteSpace(selectedValue) Then

                'Recupero i dati dell'intervento
                Dim intervento As Entities.Intervento = GetIntervento(Integer.Parse(selectedValue))
                'Set dei nuovi dati
                lblCodiceIntervento.Text = intervento.Codice
                lblTipologia.Text = Biz.BizInterventi.GetDescrizioneTipologiaIntervento(intervento.Tipologia)
                If String.IsNullOrWhiteSpace(txtDurata.Text) Then
                    txtDurata.Text = intervento.Durata
                End If

            Else

                lblCodiceIntervento.Text = String.Empty
                lblTipologia.Text = String.Empty
                txtDurata.Text = String.Empty

            End If

        End If

    End Sub

    Protected Sub omlOperatore_SetUpFiletr(sender As Object)

        Dim filtro As String = Nothing

        Dim currentDataGridItem As DataGridItem = Me.dgrInterventi.Items(Me.dgrInterventi.EditItemIndex)
        'Recupero il check per il filtro operatori
        Dim chkShowAllOp As CheckBox = DirectCast(currentDataGridItem.FindControl("chkShowAllOp"), CheckBox)

        If chkShowAllOp.Checked Then
            filtro = "(OPE_OBSOLETO='N' OR OPE_OBSOLETO IS NULL) order by OPE_NOME"
        Else
            filtro = "OPE_QUALIFICA IN ('C', 'D') AND (OPE_OBSOLETO='N' OR OPE_OBSOLETO IS NULL) order by OPE_NOME"
        End If

        'Set del filtro per la modale degli operatori
        DirectCast(sender, Onit.Controls.OnitModalList).Filtro = filtro

    End Sub

#End Region

#Region " Private "

    Private Function CaricaInterventiPaziente() As List(Of Entities.InterventoPaziente)

        Dim listInterventi As List(Of Entities.InterventoPaziente) = Nothing

        Dim paramToPass As New Biz.BizInterventi.ParametriGetInterventiPazienteBiz()
        paramToPass.CodicePaziente = OnVacUtility.Variabili.PazId
        paramToPass.CampoOrdinamento = CampoOrdinamento
        paramToPass.VersoOrdinamento = VersoOrdinamento

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizInterventi As New Biz.BizInterventi(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                listInterventi = bizInterventi.GetInterventiPaziente(paramToPass)

            End Using
        End Using

        Return listInterventi

    End Function

    Private Function GetIntervento(codiceIntervento As Integer) As Entities.Intervento

        Dim intervento As Entities.Intervento = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizInterventi As New Biz.BizInterventi(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                intervento = bizInterventi.GetIntervento(codiceIntervento)

            End Using
        End Using

        Return intervento

    End Function

    Private Sub Inizializza()

        'Ordinamento
        CampoOrdinamento = Nothing
        VersoOrdinamento = Nothing

        'Griglia in visualizzazione
        dgrInterventi.EditItemIndex = -1

        'Stato pagina in visualizzazione
        StatoPagina = StatoPaginaInterventi.Lettura

        ' Caricamento dati
        Dim listInterventi As List(Of Entities.InterventoPaziente) = CaricaInterventiPaziente()

        ' Bind Datagrid
        dgrInterventi.DataSource = listInterventi
        dgrInterventi.DataBind()

    End Sub

    Protected Sub ImpostaLayoutInterventi(stato As StatoPaginaInterventi)

        ' Top e Left Frame
        OnitLayout.Busy = (stato = StatoPaginaInterventi.Modifica)

        Dim abilita As Boolean = (stato = StatoPaginaInterventi.Lettura)

        ' Toolbar
        tlbInterventi.Items.FromKeyButton("btnNuovo").Enabled = abilita

        ' Blocco il sorting della griglia
        If abilita Then
            dgrInterventi.AllowSorting = True
        Else
            dgrInterventi.AllowSorting = False
        End If

    End Sub

    Protected Sub AddConfirmClickToImageButton(currentDataGridItem As DataGridItem, buttonId As String, confirmMessage As String)

        Dim btn As ImageButton = DirectCast(currentDataGridItem.FindControl(buttonId), ImageButton)

        If btn IsNot Nothing Then
            confirmMessage = HttpUtility.JavaScriptStringEncode(confirmMessage)
            btn.Attributes.Add("onclick", "if(!confirm('" + confirmMessage + "')) StopPreventDefault(event);")
        End If

    End Sub

    Private Sub EnableImageButton(currentDataGridItem As DataGridItem, buttonId As String, enable As Boolean)

        Dim btn As ImageButton = DirectCast(currentDataGridItem.FindControl(buttonId), ImageButton)

        If Not btn Is Nothing Then

            btn.Enabled = enable

            Dim currentImage As String = btn.ImageUrl

            If currentImage.IndexOf("_dis") > -1 Then

                If enable Then
                    ' Immagine disabilitata -> da abilitare

                    Dim nome As String = currentImage.Substring(0, currentImage.LastIndexOf("_dis."))
                    Dim estensione As String = currentImage.Substring(currentImage.LastIndexOf("."))

                    btn.ImageUrl = nome + estensione

                End If

            Else

                If Not enable Then
                    ' Immagine abilitata -> da disabilitare

                    Dim nome As String = currentImage.Substring(0, currentImage.LastIndexOf("."))
                    Dim estensione As String = currentImage.Substring(currentImage.LastIndexOf("."))

                    btn.ImageUrl = String.Format("{0}_dis{1}", nome, estensione)

                End If

            End If

        End If

    End Sub

    Protected Sub DataGridItemDataBound(currentDataGridItem As DataGridItem, editItemIndex As Integer)

        Dim enable As Boolean = (Me.StatoPagina = StatoPaginaInterventi.Lettura)

        ' Abilita/Disabilita i pulsanti della griglia, dove presenti
        EnableImageButton(currentDataGridItem, "btnEditGrid", enable)
        EnableImageButton(currentDataGridItem, "btnDeleteGrid", enable)

        If currentDataGridItem.ItemIndex = editItemIndex Then

            ' CodiceIntervento
            Dim codiceIntervento As String = String.Empty
            Dim lblCodiceIntervento As Label = DirectCast(currentDataGridItem.FindControl("lblCodiceIntervento"), Label)
            If Not lblCodiceIntervento Is Nothing Then
                codiceIntervento = lblCodiceIntervento.Text
            End If

            ' Bind della dropdownlist intervento solo per la riga in edit
            Dim ddlIntervento As DropDownList = DirectCast(currentDataGridItem.FindControl("ddlIntervento"), DropDownList)
            If Not ddlIntervento Is Nothing Then
                BindDdlIntervento(ddlIntervento, codiceIntervento)
            End If

            ' CodiceOperatore
            Dim codiceOperatore As String = String.Empty
            Dim lblCodiceOperatore As Label = DirectCast(currentDataGridItem.FindControl("lblCodiceOperatore"), Label)
            If Not lblCodiceOperatore Is Nothing Then
                codiceOperatore = lblCodiceOperatore.Text
            End If

            ' Bind della ModalList solo per la riga in edit
            Dim omlOperatore As Onit.Controls.OnitModalList = DirectCast(currentDataGridItem.FindControl("omlOperatore"), Onit.Controls.OnitModalList)
            If Not omlOperatore Is Nothing Then
                omlOperatore.Codice = codiceOperatore
                omlOperatore.RefreshDataBind()
            End If

            'Set default check
            Dim chkShowAllOp As CheckBox = DirectCast(currentDataGridItem.FindControl("chkShowAllOp"), CheckBox)
            If Not chkShowAllOp Is Nothing Then
                chkShowAllOp.Checked = False
            End If

            'NOTA: Il resto è bindato da aspx

        End If

    End Sub

    Protected Sub BindDdlIntervento(ddlIntervento As DropDownList, codiceInterventoSelezionato As String)

        If Not ddlIntervento Is Nothing Then

            Dim lstInterventi As List(Of Entities.Intervento) = Nothing

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizInterventi As New Biz.BizInterventi(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                    lstInterventi = bizInterventi.GetInterventi(String.Empty)

                End Using
            End Using

            'Aggiungo un elmento vuoto
            lstInterventi.Insert(0, New Entities.Intervento())

            ddlIntervento.DataTextField = "Descrizione"
            ddlIntervento.DataValueField = "Codice"
            ddlIntervento.DataSource = lstInterventi
            ddlIntervento.DataBind()

            If Not String.IsNullOrEmpty(codiceInterventoSelezionato) AndAlso ddlIntervento.Items.Count > 0 Then

                Dim listItem As ListItem = ddlIntervento.Items.FindByValue(codiceInterventoSelezionato)

                If Not listItem Is Nothing Then listItem.Selected = True

            End If

        End If

    End Sub

    Private Sub InserisciNuovoInterventoDataGrid()

        Me.StatoPagina = StatoPaginaInterventi.Modifica

        ' Caricamento dati
        Dim listInterventi As List(Of Entities.InterventoPaziente) = CaricaInterventiPaziente()

        'Inserimento nuova riga intervento paziente
        Dim newIntPaz As Entities.InterventoPaziente = New Entities.InterventoPaziente()
        newIntPaz.Data = Date.Today

        If Not String.IsNullOrWhiteSpace(OnVacUtility.Variabili.MedicoVaccinante.Codice) Then
            'Prevalorizzazione con il vaccinatore delle programmate
            newIntPaz.CodiceOperatore = OnVacUtility.Variabili.MedicoVaccinante.Codice
        End If

        listInterventi.Insert(0, newIntPaz)

        'Setto l'index in edit
        dgrInterventi.EditItemIndex = 0

        ' Bind Datagrid
        dgrInterventi.DataSource = listInterventi
        dgrInterventi.DataBind()

    End Sub

    Private Sub ModificaInterventoDataGrid(e As System.Web.UI.WebControls.DataGridCommandEventArgs)

        StatoPagina = StatoPaginaInterventi.Modifica

        ' Caricamento dati
        Dim listInterventi As List(Of Entities.InterventoPaziente) = CaricaInterventiPaziente()

        'Setto l'index in edit
        dgrInterventi.EditItemIndex = e.Item.ItemIndex

        ' Bind Datagrid
        dgrInterventi.DataSource = listInterventi
        dgrInterventi.DataBind()

    End Sub

    Private Sub EliminaInterventoDataGrid(idIntervento As Integer)

        'Elimina intervento paziente
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizInterventi As New Biz.BizInterventi(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                bizInterventi.EliminaInterventoPaziente(idIntervento, OnVacContext.UserId)

            End Using
        End Using

    End Sub

    Private Function SalvaInterventoDataGrid(intervento As Entities.InterventoPaziente) As Biz.BizGenericResult

        'Salva intervento paziente
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizInterventi As New Biz.BizInterventi(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                Return bizInterventi.SalvaInterventoPaziente(intervento, OnVacUtility.Variabili.PazId, OnVacContext.UserId)

            End Using
        End Using

    End Function

    Private Function UnbindDataGridItem(currentDataGridItem As DataGridItem) As Entities.InterventoPaziente

        Dim intervento As Entities.InterventoPaziente = New Entities.InterventoPaziente()

        'Codice
        If dgrInterventi.DataKeys(currentDataGridItem.ItemIndex) IsNot Nothing Then
            Dim key As String = dgrInterventi.DataKeys(currentDataGridItem.ItemIndex).ToString()
            If Not String.IsNullOrWhiteSpace(key) Then
                intervento.Codice = Integer.Parse(key)
            End If
        End If

        'Intervento
        Dim ddlIntervento As DropDownList = DirectCast(currentDataGridItem.FindControl("ddlIntervento"), DropDownList)
        intervento.CodiceIntervento = Integer.Parse(ddlIntervento.SelectedValue)

        'Data
        Dim odpData As OnitDatePick = DirectCast(currentDataGridItem.FindControl("odpData"), OnitDatePick)
        intervento.Data = odpData.Data

        'Durata
        Dim txtDurata As OnitJsValidator = DirectCast(currentDataGridItem.FindControl("txtDurata"), OnitJsValidator)
        If Not String.IsNullOrWhiteSpace(txtDurata.Text) Then
            intervento.Durata = Integer.Parse(txtDurata.Text)
        End If

        'Operatore
        Dim omlOperatore As Onit.Controls.OnitModalList = DirectCast(currentDataGridItem.FindControl("omlOperatore"), Onit.Controls.OnitModalList)
        intervento.CodiceOperatore = omlOperatore.Codice

        'Note
        Dim txtNote As TextBox = DirectCast(currentDataGridItem.FindControl("txtNote"), TextBox)
        intervento.Note = txtNote.Text

        Return intervento

    End Function

    Private Function GetDataNascitaPaziente() As DateTime

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Return genericProvider.Paziente.GetDataNascita(OnVacUtility.Variabili.PazId)

        End Using

    End Function

    Private Function CheckValoriSalvataggio(e As System.Web.UI.WebControls.DataGridCommandEventArgs) As Boolean

        'Intervento
        Dim ddlIntervento As DropDownList = DirectCast(e.Item.FindControl("ddlIntervento"), DropDownList)
        If String.IsNullOrWhiteSpace(ddlIntervento.SelectedValue) Then
            OnitLayout31.ShowMsgBox(New OnitLayout3.OnitLayoutMsgBox("Impostare un Intervento.", "err", False, False))
            Return False
        End If

        'Durata
        Dim txtDurata As OnitJsValidator = DirectCast(e.Item.FindControl("txtDurata"), OnitJsValidator)
        If String.IsNullOrWhiteSpace(txtDurata.Text) Then
            OnitLayout31.ShowMsgBox(New OnitLayout3.OnitLayoutMsgBox("Impostare una durata per l'intervento.", "err", False, False))
            Return False
        End If

        'Operatore
        Dim omlOperatore As Onit.Controls.OnitModalList = DirectCast(e.Item.FindControl("omlOperatore"), Onit.Controls.OnitModalList)
        If Not omlOperatore.IsValid Then
            OnitLayout31.ShowMsgBox(New OnitLayout3.OnitLayoutMsgBox("L'operatore impostato non è valido.", "err", False, False))
            Return False
        End If
        If String.IsNullOrWhiteSpace(omlOperatore.Codice) Then
            OnitLayout31.ShowMsgBox(New OnitLayout3.OnitLayoutMsgBox("Impostare l'operatore.", "err", False, False))
            Return False
        End If

        'Data
        Dim odpData As OnitDatePick = DirectCast(e.Item.FindControl("odpData"), OnitDatePick)
        If String.IsNullOrWhiteSpace(odpData.Text) Then
            OnitLayout31.ShowMsgBox(New OnitLayout3.OnitLayoutMsgBox("Impostare una data per l'intervento.", "err", False, False))
            Return False
        End If

        Dim dataNascita As DateTime = GetDataNascitaPaziente()
        If odpData.Data < dataNascita Then
            OnitLayout31.ShowMsgBox(New OnitLayout3.OnitLayoutMsgBox("La data non può essere antecedente alla data di nascita del paziente.", "err", False, False))
            Return False
        End If
        If odpData.Data > DateTime.Today Then
            OnitLayout31.ShowMsgBox(New OnitLayout3.OnitLayoutMsgBox("La data non può essere futura.", "err", False, False))
            Return False
        End If

        Return True

    End Function

    Private Sub SetImmagineOrdinamento()

        Dim id As String = String.Empty

        Select Case CampoOrdinamento
            Case dgrInterventi.Columns(DgrColumnIndex.Intervento).SortExpression
                id = "imgInt"
            Case dgrInterventi.Columns(DgrColumnIndex.Data).SortExpression
                id = "imgDat"
            Case dgrInterventi.Columns(DgrColumnIndex.Tipologia).SortExpression
                id = "imgTip"
            Case dgrInterventi.Columns(DgrColumnIndex.Durata).SortExpression
                id = "imgDur"
            Case dgrInterventi.Columns(DgrColumnIndex.Operatore).SortExpression
                id = "imgOp"
            Case dgrInterventi.Columns(DgrColumnIndex.Note).SortExpression
                id = "imgNot"
        End Select

        Dim imageUrl As String = String.Empty

        If VersoOrdinamento = "ASC" Then
            imageUrl = ResolveClientUrl("~/Images/arrow_up_small.gif")
        Else
            imageUrl = ResolveClientUrl("~/Images/arrow_down_small.gif")
        End If

        OnitLayout31.InsertRoutineJS(String.Format("ImpostaImmagineOrdinamento('{0}', '{1}')", id, imageUrl))

    End Sub

#End Region

End Class