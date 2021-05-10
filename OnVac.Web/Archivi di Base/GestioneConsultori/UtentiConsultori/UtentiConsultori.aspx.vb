Imports System.Collections.Generic

Imports Onit.OnAssistnet.Data


Public Class UtentiConsultori
    Inherits Common.PageBase

#Region " Property "

    Protected Property StatoPagina As PageStatus
        Get
            If ViewState("StatoPagina") Is Nothing Then ViewState("StatoPagina") = PageStatus.View
            Return ViewState("StatoPagina")
        End Get
        Set(value As PageStatus)
            ViewState("StatoPagina") = value
            Me.SetLayout()
        End Set
    End Property

    Private ReadOnly Property IdUtenteSelezionato As Integer
        Get
            If Me.dgrUtenti.SelectedItem Is Nothing Then
                Return -1
            End If
            Return Convert.ToInt32(HttpUtility.HtmlDecode(Me.dgrUtenti.SelectedItem.Cells(DgrUtentiColumnIndex.IdUtente).Text))
        End Get
    End Property

#End Region

#Region " Enums "

    Protected Enum PageStatus As Int16
        View = 0
        Edit = 1
    End Enum

    Private Enum DgrUtentiColumnIndex As Int16
        SelectorColumn = 0
        IdUtente = 1
        Codice = 2
        Descrizione = 3
        Cognome = 4
        Nome = 5
    End Enum

    Private Enum DgrConsultoriColumnIndex As Int16
        CheckBox = 0
        Descrizione = 1
        Codice = 2
        ColonnaDefault = 3
        HiddenConsultorioDefault = 4
    End Enum

#End Region

#Region " Page Events "

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then

            Me.StatoPagina = PageStatus.View

			Me.dgrUtenti.SelectedIndex = -1
			LoadUsl()
			LoadDistretto("")
			Me.BindConsultori()

        End If

    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub ToolBar_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case be.Button.Key

            Case "btnModifica"

                If Me.IdUtenteSelezionato = -1 Then
                    Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Selezionare un utente per effettuare la modifica.", "noModifica", False, False))
                Else
                    Me.StatoPagina = PageStatus.Edit
                    Me.hdIndexFlagDefault.Value = String.Empty
                    Me.BindConsultori()
                End If

            Case "btnSalva"

                Me.Salva()

                Me.StatoPagina = PageStatus.View
                Me.BindConsultori()

            Case "btnAnnulla"

                Me.StatoPagina = PageStatus.View
                Me.BindConsultori()

        End Select

    End Sub

#End Region

#Region " User Control Events "

    Private Sub ucFiltroRicercaUtenti_Cerca(filtro As String) Handles ucFiltroRicercaUtenti.Cerca

        Me.dgrUtenti.SelectedIndex = -1

        Me.LoadUtenti(0, filtro)

        Me.BindConsultori()

    End Sub

#End Region

#Region " Datagrid Events "

    Private Sub dgrUtenti_PageIndexChanged(source As Object, e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dgrUtenti.PageIndexChanged

        Me.LoadUtenti(e.NewPageIndex, Me.ucFiltroRicercaUtenti.GetFilter())

        Me.dgrUtenti.SelectedIndex = -1

        Me.BindConsultori()

    End Sub

    Private Sub dgrUtenti_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles dgrUtenti.SelectedIndexChanged

        Me.BindConsultori()

    End Sub

    Private Sub dgrConsultori_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrConsultori.ItemDataBound

        Select Case e.Item.ItemType

            Case ListItemType.Item, ListItemType.AlternatingItem, ListItemType.SelectedItem, ListItemType.EditItem

                Dim isInEdit As Boolean = (Me.StatoPagina = PageStatus.Edit)

                Dim consultorioUtente As Entities.ConsultorioUtente = DirectCast(e.Item.DataItem, Entities.ConsultorioUtente)

                Dim chkConsultorio As CheckBox = DirectCast(e.Item.FindControl("chkConsultorio"), CheckBox)
                chkConsultorio.Checked = consultorioUtente.Abilitato
                chkConsultorio.Enabled = isInEdit

                If consultorioUtente.ConsultorioDefault Then
                    Me.hdIndexFlagDefault.Value = e.Item.ItemIndex.ToString()
                End If

                Dim imgDefault As System.Web.UI.WebControls.Image = DirectCast(e.Item.FindControl("imgDefault"), System.Web.UI.WebControls.Image)
                imgDefault.Visible = consultorioUtente.ConsultorioDefault And Not isInEdit

                If isInEdit Then

                    Dim checkedAttribute As String = String.Empty

                    If consultorioUtente.ConsultorioDefault Then
                        checkedAttribute = "checked='true'"
                    End If

                    e.Item.Cells(DgrConsultoriColumnIndex.ColonnaDefault).Controls.AddAt(
                        0, New LiteralControl(String.Format("<input type='radio' name='cnsDefault' onclick='flagDefaultChanged(""{0}"", ""{1}"");' {2} />",
                                                            chkConsultorio.ClientID, e.Item.ItemIndex.ToString(), checkedAttribute)))

                End If

        End Select

    End Sub

#End Region

#Region " Private "

    Private Sub SetLayout()

        Dim isInEdit As Boolean = (Me.StatoPagina = PageStatus.Edit)

        Me.OnitLayout31.Busy = isInEdit

        Me.ToolBar.Items.FromKeyButton("btnModifica").Enabled = Not isInEdit
        Me.ToolBar.Items.FromKeyButton("btnSalva").Enabled = isInEdit
        Me.ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = isInEdit

        Me.ucFiltroRicercaUtenti.Enabled = Not isInEdit

        Me.dgrUtenti.PagerStyle.Visible = Not isInEdit
        Me.dgrUtenti.Columns(DgrUtentiColumnIndex.SelectorColumn).Visible = Not isInEdit

    End Sub

    Private Sub BindConsultori()

        Dim listConsultoriUtente As New List(Of Entities.ConsultorioUtente)()
        Dim listConsultoriAbilitati As List(Of Entities.ConsultorioUtente) = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizConsultori As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

				' Caricamento di tutti i consultori aperti
				Dim listConsultori As List(Of Entities.ConsultorioAperti) =
					bizConsultori.GetListCodiceDescrizioneConsultori(True, Nothing, 0, ddlDistretto.SelectedValue, ddlUls.SelectedValue)

				Dim consultorioUtente As Entities.ConsultorioUtente = Nothing

				For Each consultorio As Entities.ConsultorioAperti In listConsultori

					consultorioUtente = New Entities.ConsultorioUtente()
					consultorioUtente.Abilitato = False
					consultorioUtente.CodiceConsultorio = consultorio.Codice
					consultorioUtente.DescrizioneConsultorio = consultorio.Descrizione
					consultorioUtente.ConsultorioDefault = False

					listConsultoriUtente.Add(consultorioUtente)

				Next

			End Using

            If Me.IdUtenteSelezionato >= 0 Then
                listConsultoriAbilitati = Me.LoadLinkConsultoriUtente(Me.IdUtenteSelezionato)
            End If

        End Using

        If Not listConsultoriAbilitati Is Nothing AndAlso listConsultoriAbilitati.Count > 0 Then

            For Each consultorioAbilitato As Entities.ConsultorioUtente In listConsultoriAbilitati

                Dim consultorioUtente As Entities.ConsultorioUtente =
                    listConsultoriUtente.Where(Function(p) p.CodiceConsultorio = consultorioAbilitato.CodiceConsultorio).FirstOrDefault()

                If Not consultorioUtente Is Nothing Then
                    consultorioUtente.Abilitato = True
                    consultorioUtente.ConsultorioDefault = consultorioAbilitato.ConsultorioDefault
                End If

            Next

        End If

        Me.dgrConsultori.DataSource = listConsultoriUtente
        Me.dgrConsultori.DataBind()

    End Sub

	Private Sub LoadUtenti(currentPageIndex As Integer, filtro As String)

		Dim result As Biz.BizUtenti.GetListUtentiResult = Nothing

		Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
			Using bizUtenti As New Biz.BizUtenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

				Dim command As New Biz.BizUtenti.GetListUtentiCommand()
				command.AppId = OnVacContext.AppId
				command.CodiceAzienda = OnVacContext.Azienda
				command.Filtro = filtro
				command.PageIndex = currentPageIndex
				command.PageSize = Me.dgrUtenti.PageSize

				result = bizUtenti.GetListUtenti(command)

			End Using
		End Using

		Me.dgrUtenti.VirtualItemCount = result.TotalCountUtenti
		Me.dgrUtenti.CurrentPageIndex = result.CurrentPageIndex

		Me.dgrUtenti.DataSource = result.ListUtenti
		Me.dgrUtenti.DataBind()

		Me.dgrUtenti.SelectedIndex = -1

	End Sub

	Private Sub LoadDistretto(codiceUlss As String)
		ddlDistretto.Items.Clear()

		Dim listaDistr As List(Of OnVac.Entities.Distretto) = Nothing
		Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
			Using bizAnagrafica As New Biz.BizAnagrafiche(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())
				listaDistr = bizAnagrafica.GetListAnagraficaDistretti("", "", codiceUlss)
			End Using
		End Using
		ddlDistretto.DataTextField = "DesUlssDistretto"
		ddlDistretto.DataValueField = "Codice"

		If listaDistr.Count > 0 Then
			listaDistr.Insert(0, New Entities.Distretto() With {.Codice = String.Empty, .Descrizione = String.Empty, .CodiceEsterno = String.Empty, .CodiceComune = String.Empty, .CodiceUlss = String.Empty})
			ddlDistretto.SelectedValue = String.Empty
			ddlDistretto.DataSource = listaDistr.OrderBy(Function(p) p.Descrizione)
		End If
		ddlDistretto.DataBind()
	End Sub
	Private Sub LoadUsl()
		ddlUls.Items.Clear()

		Dim listaUsl As List(Of OnVac.Entities.UslDistretto) = Nothing
		Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
			Using bizAsl As New Biz.BizUsl(genericProvider, OnVacContext.CreateBizContextInfos())
				listaUsl = bizAsl.GetListaUslDistretto()
			End Using
		End Using
		ddlUls.DataTextField = "DesCodice"
		ddlUls.DataValueField = "Codice"

		If listaUsl.Count > 0 Then
			listaUsl.Insert(0, New Entities.UslDistretto() With {.Codice = String.Empty, .Descrizione = String.Empty})
			ddlUls.SelectedValue = String.Empty
			ddlUls.DataSource = listaUsl.OrderBy(Function(p) p.Descrizione)
		End If
		ddlUls.DataBind()
	End Sub
	Protected Sub ddlDistretto_SelectedIndexChanged(sender As Object, e As EventArgs)
		BindConsultori()
	End Sub
	Protected Sub ddlUls_SelectedIndexChanged(sender As Object, e As EventArgs)
		LoadDistretto(ddlUls.SelectedValue)
		BindConsultori()
	End Sub

	Private Function LoadLinkConsultoriUtente(idUtente As Integer) As List(Of Entities.ConsultorioUtente)

        Dim listConsultoriUtente As List(Of Entities.ConsultorioUtente) = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizUtenti As New Biz.BizUtenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                listConsultoriUtente = bizUtenti.GetListConsultoriUtente(idUtente)

            End Using
        End Using

        Return listConsultoriUtente

    End Function

    Private Sub Salva()

        Dim listConsultoriUtente As New List(Of Entities.ConsultorioUtente)()

        For Each datagridItem As DataGridItem In dgrConsultori.Items

            If DirectCast(datagridItem.FindControl("chkConsultorio"), CheckBox).Checked Then

                Dim consultorioUtente As New Entities.ConsultorioUtente()
                consultorioUtente.CodiceConsultorio = HttpUtility.HtmlDecode(datagridItem.Cells(DgrConsultoriColumnIndex.Codice).Text.Trim())

                If Not String.IsNullOrEmpty(Me.hdIndexFlagDefault.Value) Then
                    consultorioUtente.ConsultorioDefault = (Me.hdIndexFlagDefault.Value = datagridItem.ItemIndex.ToString())
                End If

                consultorioUtente.IdUtente = Me.IdUtenteSelezionato

                listConsultoriUtente.Add(consultorioUtente)

            End If

        Next

        ' Se nessun default è stato impostato, seleziono come default il primo della lista
        If listConsultoriUtente.Count > 0 AndAlso String.IsNullOrEmpty(Me.hdIndexFlagDefault.Value) Then

            listConsultoriUtente(0).ConsultorioDefault = True

        End If

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizUtenti As New Biz.BizUtenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                bizUtenti.SaveConsultoriUtente(Me.IdUtenteSelezionato, listConsultoriUtente)

            End Using
        End Using

    End Sub

#End Region

End Class