Imports System.Collections.Generic
Imports Infragistics.WebUI.UltraWebToolbar

Public Class LogRicezioneACN
	Inherits Common.PageBase

#Region " Types "

	Private Enum DgrColumnIndex
		Check = 0
		Selector = 1
		IdMessaggio = 2
		Operazione = 3
		DataRicezione = 4
		CodCentralePaz = 5
		CodCentraleAlias = 6
		CodLocalePaz = 7
		Cognome = 8
		Nome = 9
		DataNascita = 10
		CodiceFiscale = 11
		RisultatoElaborazione = 12
		IdNotifica = 13
		RisultatoElab = 14
	End Enum

	Private Enum ViewIndex
		ViewRicerca = 0
		ViewDettaglio = 1
	End Enum

#End Region

#Region " Proprietà private "

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
	Private Property IdNotificaSelezionata As Long
		Get
			Return ViewState("IdNotificaSelezionata")
		End Get
		Set(value As Long)
			ViewState("IdNotificaSelezionata") = value
		End Set
	End Property
	Private Property IdMessaggioSelezionata As String
		Get
			Return ViewState("IdMessaggioSelezionata")
		End Get
		Set(value As string)
			ViewState("IdMessaggioSelezionata") = value
		End Set
	End Property

	'Private Property FiltriMaschera() As FiltriCancellazioneConvocazioni
	'    Get
	'        If ViewState("FCC") Is Nothing Then ViewState("FCC") = New FiltriCancellazioneConvocazioni()
	'        Return ViewState("FCC")
	'    End Get
	'    Set(value As FiltriCancellazioneConvocazioni)
	'        ViewState("FCC") = value
	'    End Set
	'End Property

	''' <summary>
	''' Indica se il checkbox "Seleziona tutti" è selezionato o deselezionato
	''' </summary>
	''' <returns></returns>
	Private Property SelezionaTuttiChecked As Boolean
		Get
			If ViewState("CHKALL") Is Nothing Then ViewState("CHKALL") = False
			Return DirectCast(ViewState("CHKALL"), Boolean)
		End Get
		Set(value As Boolean)
			ViewState("CHKALL") = value
		End Set
	End Property

	Private Property FiltriRicercaCorrente As Entities.FiltriLogNotifiche
		Get
			If ViewState("FR") Is Nothing Then ViewState("FR") = New Entities.FiltriLogNotifiche()
			Return DirectCast(ViewState("FR"), Entities.FiltriLogNotifiche)
		End Get
		Set(value As Entities.FiltriLogNotifiche)
			ViewState("FR") = value
		End Set
	End Property

#End Region

#Region " Page "

	Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

		If Not IsPostBack Then

			InizializzaFiltriRicerca()

			ResetFiltriRicerca()

		End If

	End Sub

	Private Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender

		SetImmagineOrdinamento()

	End Sub

#End Region

#Region " Controls Events "

	Private Sub ToolBarMain_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBarMain.ButtonClicked

		Select Case e.Button.Key

			Case "btnCerca"

				RicercaConvocazioni()

			Case "btnPulisci"

				ResetFiltriRicerca()

			Case "btnRielaboraMassivo"

				RilelaboraMassivo()

		End Select

	End Sub

	Private Sub ToolbarDetail_ButtonClicked(sender As Object, be As ButtonEvent) Handles ToolbarDetail.ButtonClicked

		Select Case be.Button.Key

			Case "btnIndietro"
				mainView.ActiveViewIndex = ViewIndex.ViewRicerca
				RicercaConvocazioni()

			Case "btnRielabora"
				Rielabora(Nothing)

		End Select

	End Sub

	Private Sub dgrLogNotifiche_SortCommand(source As Object, e As DataGridSortCommandEventArgs) Handles dgrLogNotifiche.SortCommand

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

		CaricaDati(dgrLogNotifiche.CurrentPageIndex, FiltriRicercaCorrente)

	End Sub

	Private Sub dgrLogNotifiche_PageIndexChanged(source As Object, e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dgrLogNotifiche.PageIndexChanged

		CaricaDati(e.NewPageIndex, FiltriRicercaCorrente)

	End Sub

	Private Sub dgrLogNotifiche_SelectedIndexChanged(sender As Object, e As EventArgs) Handles dgrLogNotifiche.SelectedIndexChanged

		mainView.ActiveViewIndex = ViewIndex.ViewDettaglio
		IdNotificaSelezionata = GetIdNotificaFromDataGridItem(dgrLogNotifiche.SelectedItem)
		IdMessaggioSelezionata = GetIdMessaggioFromDataGridItem(dgrLogNotifiche.SelectedItem)
		CaricaDettaglioNotifiche(IdNotificaSelezionata)

	End Sub

#End Region

#Region " Private "

	Private Function GetIdNotificaFromDataGridItem(item As DataGridItem) As Long

		If item Is Nothing Then Return -1

		Return Convert.ToInt64(HttpUtility.HtmlDecode(item.Cells(DgrColumnIndex.IdNotifica).Text))

	End Function
	Private Function GetIdMessaggioFromDataGridItem(item As DataGridItem) As String

		If item Is Nothing Then Return String.Empty

		Return HttpUtility.HtmlDecode(item.Cells(DgrColumnIndex.IdMessaggio).Text)

	End Function

	Private Sub AddEmptyItemToDropDownList(ddl As DropDownList)

		If ddl.Items.Count = 0 OrElse ddl.Items.FindByValue(String.Empty) Is Nothing Then

			Dim listItem As ListItem = New ListItem()
			listItem.Text = String.Empty
			listItem.Value = String.Empty
			ddl.Items.Insert(0, listItem)

		End If

	End Sub

	Private Sub InizializzaFiltriRicerca()

		Dim listItem As ListItem = Nothing

#Region " OPERAZIONI "

		' Se lato aspx non è stato specificato nulla, di default imposto che le operazioni da visualizzare sono solo quelle anagrafiche
		If ddlOperazione.Items.Count = 0 Then

			Dim op As String

			listItem = New ListItem()
			op = GetStringOperazioneFromEnumValue(Enumerators.OperazioneLogNotifica.InserimentoVaccinazioniEseguite)

			listItem.Text = op
			listItem.Value = op
			ddlOperazione.Items.Add(listItem)

			listItem = New ListItem()
			op = GetStringOperazioneFromEnumValue(Enumerators.OperazioneLogNotifica.ModificaVaccinazioniEseguite)

			listItem.Text = op
			listItem.Value = op
			ddlOperazione.Items.Add(listItem)

			listItem = New ListItem()
			op = GetStringOperazioneFromEnumValue(Enumerators.OperazioneLogNotifica.EliminazioneVaccinazioniEseguite)

			listItem.Text = op
			listItem.Value = op
			ddlOperazione.Items.Add(listItem)

		End If

		AddEmptyItemToDropDownList(ddlOperazione)

#End Region

#Region " STATI "

		' Se lato aspx non è stato specificato nulla, gli stati possibili del messaggio sono tutti quelli previsti dal biz
		If ddlRisultato.Items.Count = 0 Then

			listItem = New ListItem()
			listItem.Text = "OK"
			listItem.Value = Biz.BizLogNotifiche.RisultatoElaborazioneNotifica.ACK_POSITIVO
			ddlRisultato.Items.Add(listItem)

			listItem = New ListItem()
			listItem.Text = "ERROR"
			listItem.Value = Biz.BizLogNotifiche.RisultatoElaborazioneNotifica.ERROR
			ddlRisultato.Items.Add(listItem)

			listItem = New ListItem()
			listItem.Text = "WARNING"
			listItem.Value = Biz.BizLogNotifiche.RisultatoElaborazioneNotifica.WARNING
			ddlRisultato.Items.Add(listItem)

		End If

		AddEmptyItemToDropDownList(ddlRisultato)

#End Region

	End Sub

	Private Sub ResetFiltriRicerca()

		CampoOrdinamento = Nothing
		VersoOrdinamento = Nothing

		FiltriRicercaCorrente = New Entities.FiltriLogNotifiche()

		txtId.Text = String.Empty
		dpkRicezioneDa.Text = String.Empty
		dpkRicezioneA.Text = String.Empty

		ddlOperazione.ClearSelection()
		ddlOperazione.SelectedValue = String.Empty

		ddlRisultato.ClearSelection()
		ddlRisultato.SelectedValue = String.Empty

		dpkNascitaDa.Text = String.Empty
		dpkNascitaA.Text = String.Empty

		txtCodiceCentrale.Text = String.Empty
		txtCodiceLocale.Text = String.Empty
		txtCodiceFiscale.Text = String.Empty

		' Griglia
		dgrLogNotifiche.SelectedIndex = -1
		dgrLogNotifiche.DataSource = Nothing
		dgrLogNotifiche.DataBind()

	End Sub

	Private Function GetFiltriRicerca() As Entities.FiltriLogNotifiche

		Dim filtri As New Entities.FiltriLogNotifiche()

		If Not String.IsNullOrWhiteSpace(txtId.Text) Then
			filtri.IdMessaggio = txtId.Text
		End If
		If Not String.IsNullOrWhiteSpace(dpkRicezioneDa.Text) Then
			filtri.DataRicezioneDa = dpkRicezioneDa.Data
		End If

		If Not String.IsNullOrWhiteSpace(dpkRicezioneA.Text) Then
			filtri.DataRicezioneA = dpkRicezioneA.Data
		End If

		If String.IsNullOrWhiteSpace(ddlOperazione.SelectedValue) Then

			' N.B. : Se non viene selezionata nessuna operazione, creo una lista lista con tutte quelle presenti. 
			'        Questo perchè nella tabella di log sono presenti anche altre operazioni non previste da questa maschera

			Dim list As New List(Of Enumerators.OperazioneLogNotifica)()

			For Each listItem As ListItem In ddlOperazione.Items

				If String.IsNullOrWhiteSpace(listItem.Value) Then Continue For

				list.Add([Enum].Parse(GetType(Enumerators.OperazioneLogNotifica), listItem.Value, True))

			Next

			filtri.Operazioni = list

		Else

			filtri.Operazioni = New List(Of Enumerators.OperazioneLogNotifica)()
			filtri.Operazioni.Add([Enum].Parse(GetType(Enumerators.OperazioneLogNotifica), ddlOperazione.SelectedValue, True))

		End If

		If Not String.IsNullOrWhiteSpace(ddlRisultato.SelectedValue) Then
			filtri.Risultato = ddlRisultato.SelectedValue
		End If

		If Not String.IsNullOrWhiteSpace(dpkNascitaDa.Text) Then
			filtri.DataNascitaDa = dpkNascitaDa.Data
		End If

		If Not String.IsNullOrWhiteSpace(dpkNascitaA.Text) Then
			filtri.DataNascitaA = dpkNascitaA.Data
		End If

		If Not txtCodiceCentrale.Text.IsNullOrEmpty() Then
			filtri.CodiceMPI = txtCodiceCentrale.Text
		End If
		If Not txtCodiceLocale.Text.IsNullOrEmpty() Then
			filtri.CodiceLocale = txtCodiceLocale.Text
		End If
		If Not txtCodiceFiscale.Text.IsNullOrEmpty() Then
			filtri.CF = txtCodiceFiscale.Text
		End If

		Return filtri

	End Function

	Private Sub RicercaConvocazioni()


		SelezionaTuttiChecked = False

		CaricaDati(0)

	End Sub
	Private Sub RilelaboraMassivo()
		Dim listaIdMessaggio As List(Of String) = SetIdMessaggiSelezionate()

		Rielabora(listaIdMessaggio)

		CaricaDati(dgrLogNotifiche.CurrentPageIndex, FiltriRicercaCorrente)
	End Sub

	Private Sub CaricaDati(currentPageIndex As Int32)

		CaricaDati(currentPageIndex, GetFiltriRicerca())

	End Sub

	Private Sub CaricaDati(currentPageIndex As Int32, filtri As Entities.FiltriLogNotifiche)

		If filtri Is Nothing Then filtri = GetFiltriRicerca()

		' Impostazione filtri correnti
		FiltriRicercaCorrente = filtri.Clone()

		Dim result As Biz.BizLogNotifiche.GetLogNotificheRicevuteResult = Nothing

		Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
			Using biz As New Biz.BizLogNotifiche(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

				Dim command As New Biz.BizLogNotifiche.GetLogNotificheRicevuteCommand()

				command.Filtri = FiltriRicercaCorrente
				command.PageIndex = currentPageIndex
				command.PageSize = dgrLogNotifiche.PageSize
				command.CampoOrdinamento = CampoOrdinamento
				command.VersoOrdinamento = VersoOrdinamento

				result = biz.GetLogNotificheRicevuteAcn(command)

			End Using
		End Using

		dgrLogNotifiche.VirtualItemCount = result.CountNotificheRicevute
		dgrLogNotifiche.CurrentPageIndex = currentPageIndex

		dgrLogNotifiche.SelectedIndex = -1
		dgrLogNotifiche.DataSource = result.ListLogNotificheRicevuteMonitor
		dgrLogNotifiche.DataBind()

	End Sub

	Private Sub CaricaDettaglioNotifiche(idNotifica As Long)

		Dim item As Entities.LogNotificaRicevutaACN

		Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
			Using biz As New Biz.BizLogNotifiche(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

				item = biz.GetLogNotificaRicevutaAcnByIdNotifica(idNotifica)

			End Using
		End Using

		If item Is Nothing Then Return

		lblCaptionDettaglio.Text = String.Format("Messaggio selezionato: ID {0}", item.IdMessaggio)
		If item.Operazione.HasValue Then
			lblCaptionDettaglio.Text += String.Format(" - OPERAZIONE: {0}", GetStringOperazioneFromEnumValue(item.Operazione.Value))
		End If

		'Dati del Paziente
		txtCodiceMpi.Text = item.CodiceCentralePaziente
		txtCodiceMpiAlias.Text = item.CodiceRegionale
		txtCodicePaziente.Text = item.CodiceLocalePaziente
		txtCognome.Text = item.Cognome
		txtNome.Text = item.Nome
		txtDataNascita.Text = item.DataNascita.ToShortDateString()
		txtCF.Text = item.CodiceFiscale

		'dati del messaggio
		txtIdMessaggio.Text = item.IdMessaggio.ToString()
		txtDataRicezione.Text = item.DataRicezione.ToShortDateString()
		If item.DataInvioRisposta.HasValue Then
			txtDataInvio.Text = item.DataInvioRisposta.Value.ToShortDateString()
		End If
		txtNumeroRicezione.Text = item.NumeroRicezioni.ToStringOrDefault()
		If item.Operazione.HasValue Then
			txtOpeaz.Text = item.Operazione.ToString()
		End If
		txtRisult.Text = item.MessaggioElaborazione
		imgRisult.ImageUrl = GetUrlIconaRisultato(item.RisultatoElaborazione)
		imgRisult.ToolTip = GetDescrizioneRisultato(item.RisultatoElaborazione, String.Empty)

		If item.RisultatoElaborazione <> Biz.BizLogNotifiche.RisultatoElaborazioneNotifica.ACK_POSITIVO Then
			ToolbarDetail.Items.FromKeyButton("btnRielabora").Visible = True
		Else
			ToolbarDetail.Items.FromKeyButton("btnRielabora").Visible = False
		End If

		If String.IsNullOrWhiteSpace(item.ServiceRequest) Then
			txtMessaggioCompleto.Text = String.Empty
		Else
			Try
				txtMessaggioCompleto.Text = System.Xml.Linq.XDocument.Parse(item.ServiceRequest).ToString()
			Catch ex As Exception
				txtMessaggioCompleto.Text = item.ServiceRequest
			End Try
		End If
		If String.IsNullOrWhiteSpace(item.EntitySerializzata) Then
			txtEntity.Text = String.Empty
		Else
			Try
				txtEntity.Text = System.Xml.Linq.XDocument.Parse(item.EntitySerializzata).ToString()
			Catch ex As Exception
				txtEntity.Text = item.EntitySerializzata
			End Try
		End If

	End Sub

	Private Sub SetImmagineOrdinamento()

		Dim id As String = String.Empty

		Select Case CampoOrdinamento
			Case dgrLogNotifiche.Columns(DgrColumnIndex.IdMessaggio).SortExpression
				id = "imgIdMsg"
			Case dgrLogNotifiche.Columns(DgrColumnIndex.Operazione).SortExpression
				id = "imgOp"
			Case dgrLogNotifiche.Columns(DgrColumnIndex.DataRicezione).SortExpression
				id = "imgDataRicez"
			Case dgrLogNotifiche.Columns(DgrColumnIndex.CodCentralePaz).SortExpression
				id = "imgMpiPaz"
			Case dgrLogNotifiche.Columns(DgrColumnIndex.CodCentraleAlias).SortExpression
				id = "imgMpiAlias"
			Case dgrLogNotifiche.Columns(DgrColumnIndex.CodLocalePaz).SortExpression
				id = "imgCodPaz"
			Case dgrLogNotifiche.Columns(DgrColumnIndex.Cognome).SortExpression
				id = "imgCognome"
			Case dgrLogNotifiche.Columns(DgrColumnIndex.Nome).SortExpression
				id = "imgNome"
			Case dgrLogNotifiche.Columns(DgrColumnIndex.DataNascita).SortExpression
				id = "imgDataNas"
			Case dgrLogNotifiche.Columns(DgrColumnIndex.CodiceFiscale).SortExpression
				id = "imgCF"
			Case dgrLogNotifiche.Columns(DgrColumnIndex.RisultatoElaborazione).SortExpression
				id = "imgRisultato"
		End Select

		Dim imageUrl As String = String.Empty

		If VersoOrdinamento = Constants.VersoOrdinamento.Crescente Then
			imageUrl = Me.ResolveClientUrl("~/Images/arrow_up_small.gif")
		Else
			imageUrl = Me.ResolveClientUrl("~/Images/arrow_down_small.gif")
		End If

		OnitLayout31.InsertRoutineJS(String.Format("ImpostaImmagineOrdinamento('{0}', '{1}')", id, imageUrl))

	End Sub

	Protected Function ConvertToDateString(value As Object, includeTime As Boolean) As String

		If value Is Nothing OrElse value Is DBNull.Value Then Return String.Empty

		Dim dateValue As DateTime = Convert.ToDateTime(value)

		If dateValue = DateTime.MinValue Then Return String.Empty

		Dim format As String = "dd/MM/yyyy"
		If includeTime Then format += " HH:mm"

		Return dateValue.ToString(format)

	End Function

	Protected Function GetStringOperazioneFromEnumValue(value As Object) As String

		If value Is Nothing OrElse value Is DBNull.Value Then Return String.Empty

		Return [Enum].GetName(GetType(Enumerators.OperazioneLogNotifica), value).ToUpper()

	End Function

	Protected Function GetUrlIconaRisultato(value As Object) As String

		Dim imgName As String = "transparent16.gif"

		If Not value Is Nothing AndAlso Not value Is DBNull.Value Then

			Dim risultato As String = value.ToString()

			If Not String.IsNullOrWhiteSpace(risultato) Then

				Select Case risultato
					Case Biz.BizLogNotifiche.RisultatoElaborazioneNotifica.ACK_POSITIVO
						imgName = "success.png"
					Case Biz.BizLogNotifiche.RisultatoElaborazioneNotifica.ERROR
						imgName = "deny.png"
					Case Biz.BizLogNotifiche.RisultatoElaborazioneNotifica.WARNING
						imgName = "alert.png"
				End Select

			End If

		End If

		Return OnVacUtility.GetIconUrl(imgName)

	End Function

	Protected Function GetDescrizioneRisultato(value As Object, msg As Object) As String

		Dim tooltip As String = String.Empty

		If Not value Is Nothing AndAlso Not value Is DBNull.Value Then

			Dim risultato As String = value.ToString()

			If Not String.IsNullOrWhiteSpace(risultato) Then

				Select Case risultato
					Case Biz.BizLogNotifiche.RisultatoElaborazioneNotifica.ACK_POSITIVO
						tooltip = "Success"
					Case Biz.BizLogNotifiche.RisultatoElaborazioneNotifica.ERROR
						tooltip = "Error"
					Case Biz.BizLogNotifiche.RisultatoElaborazioneNotifica.WARNING
						tooltip = "Warning"
				End Select

			End If

		End If

		If Not msg Is Nothing AndAlso Not msg Is DBNull.Value Then

			Dim messaggio As String = msg.ToString()

			If Not String.IsNullOrWhiteSpace(messaggio) Then
				tooltip += Environment.NewLine + messaggio
			End If

		End If

		Return tooltip

	End Function

	Private Sub Rielabora(idMessaggio As List(Of String))
		Dim result As New Biz.BizGenericResult
		Dim messaggi As New List(Of String)
		If idMessaggio Is Nothing Then
			messaggi.Add(IdMessaggioSelezionata)
		Else
			messaggi = idMessaggio
		End If
		Dim testoerr As New System.Text.StringBuilder
		Dim controllo As Boolean = True
		Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
			Using biz As New Biz.BizACN(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
				For Each s As String In messaggi
					result = biz.Reinvia(s)
					If Not result.Success Then
						testoerr.AppendFormat("Il Messaggio {0} non è stato elaborato.", s)
						testoerr.AppendLine()
						controllo = False
					End If
				Next

			End Using
		End Using
		If controllo Then
			ToolbarDetail.Items.FromKeyButton("btnRielabora").Visible = False
			CaricaDettaglioNotifiche(IdNotificaSelezionata)
			testoerr.Append("Rielaborazione avvenuta con successo!")
		End If
		OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(HttpUtility.JavaScriptStringEncode(testoerr.ToString()), "Rielaborazione", False, False))
	End Sub

	Private Sub dgrLogNotifiche_ItemDataBound(sender As Object, e As DataGridItemEventArgs) Handles dgrLogNotifiche.ItemDataBound
		Select Case e.Item.ItemType



			Case ListItemType.AlternatingItem, ListItemType.EditItem, ListItemType.Item, ListItemType.SelectedItem

				' Gestione checkbox selezione


				Dim chkSelezioneItem As CheckBox = DirectCast(e.Item.FindControl("chkSelezioneItem"), CheckBox)

				If GetRisultatoElaborazioneDataGridItem(e.Item) Then
					chkSelezioneItem.Enabled = True
				Else
					chkSelezioneItem.Enabled = False
				End If


		End Select
	End Sub
	Private Function GetRisultatoElaborazioneDataGridItem(item As DataGridItem) As Boolean

		If item Is Nothing Then Return -1

		Return HttpUtility.HtmlDecode(item.Cells(DgrColumnIndex.RisultatoElab).Text) <> "A"

	End Function
	Private Function SetIdMessaggiSelezionate() As List(Of String)
		Dim ret As New List(Of String)
		For Each item As DataGridItem In Me.dgrLogNotifiche.Items

			If DirectCast(item.FindControl("chkSelezioneItem"), CheckBox).Checked Then

				Dim idMessaggio As String = GetIdMessaggioFromDataGridItem(item)
				If Not String.IsNullOrWhiteSpace(idMessaggio) Then
					ret.Add(idMessaggio)
				End If

			End If

		Next
		Return ret

	End Function


#End Region

End Class