Imports System.Collections.Generic
Imports Onit.Controls


Partial Class OnVac_Operatori
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

#Region " Private "

	Private PanelUtility As OnitDataPanelUtility


#End Region

#Region " Properties "

	Public ReadOnly Property Connessione() As String
		Get
			Return OnVacContext.Connection.ConnectionString
		End Get
	End Property
	Private ReadOnly Property CodiceOperatoreSelezionato As String
		Get
			If Me.dgrOperatoriMaster.SelectedItem Is Nothing Then
				Return String.Empty
			End If
			Return HttpUtility.HtmlDecode(dgrOperatoriMaster.SelectedItem.Cells(DgrOperatoriColumnIndex.Codice).Text)
		End Get
	End Property

#End Region
#Region "Enum"
	Private Enum DgrOperatoriColumnIndex As Int16
		Codice = 1
		Nome = 2
		Qualifica = 3
		CodiceEst = 4
		Obsoleto = 5
		TelStu = 6
		TelCasa = 7
		Comune = 8
		Cap = 9
		Indirizzo = 10
		CF = 11
		Email = 12
	End Enum
	Private Enum DgrConsultoriColumnIndex As Int16
		CheckBox = 0
		Distretto = 1
		Centro = 2
		ColonnaDefault = 3
		Codice = 4
	End Enum
#End Region
#Region " Eventi Page "

	Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

		Me.PanelUtility = New OnitDataPanelUtility(Me.ToolBar)
		Me.PanelUtility.FindButtonName = "btnCerca"
		Me.PanelUtility.DeleteButtonName = "btnElimina"
		Me.PanelUtility.SaveButtonName = "btnSalva"
		Me.PanelUtility.CancelButtonName = "btnAnnulla"
		Me.PanelUtility.MasterDataPanel = Me.odpOperatori
		Me.PanelUtility.WZMsDataGrid = Me.dgrOperatoriMaster
		Me.PanelUtility.WZRicBase = Me.WzFilter1.FindControl("WzFilterKeyBase")
		Me.PanelUtility.SetToolbarButtonImages()

		AddHandler Me.PanelUtility.BeforeSave, AddressOf PanelUtility_BeforeSave

		If Not IsPostBack() Then

			Me.PanelUtility.InitToolbar()

			'slokkiamo il paziente lokkato...
			Me.OnitLayout31.lock.EndLock(OnVacUtility.Variabili.PazId)

			ShowPrintButtons()

		End If

	End Sub
	Private Sub dgrConsultori_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrConsultori.ItemDataBound

		Select Case e.Item.ItemType

			Case ListItemType.Item, ListItemType.AlternatingItem, ListItemType.SelectedItem, ListItemType.EditItem

				'Dim isInEdit As Boolean = (Me.StatoPagina = PageStatus.Edit)

				Dim consultorioOperatore As Entities.ConsultorioOperatore = DirectCast(e.Item.DataItem, Entities.ConsultorioOperatore)

				Dim chkConsultorio As CheckBox = DirectCast(e.Item.FindControl("chkConsultorio"), CheckBox)
				chkConsultorio.Checked = consultorioOperatore.Abilitato
				' chkConsultorio.Enabled = isInEdit

				'If consultorioUtente.ConsultorioDefault Then
				'    Me.hdIndexFlagDefault.Value = e.Item.ItemIndex.ToString()
				'End If

				'Dim imgDefault As System.Web.UI.WebControls.Image = DirectCast(e.Item.FindControl("imgDefault"), System.Web.UI.WebControls.Image)
				'imgDefault.Visible = consultorioUtente.ConsultorioDefault And Not isInEdit

				'If isInEdit Then

				'    Dim checkedAttribute As String = String.Empty

				'    If consultorioUtente.ConsultorioDefault Then
				'        checkedAttribute = "checked='true'"
				'    End If

				'    e.Item.Cells(DgrConsultoriColumnIndex.ColonnaDefault).Controls.AddAt(
				'        0, New LiteralControl(String.Format("<input type='radio' name='cnsDefault' onclick='flagDefaultChanged(""{0}"", ""{1}"");' {2} />",
				'                                            chkConsultorio.ClientID, e.Item.ItemIndex.ToString(), checkedAttribute)))

				'End If

		End Select

	End Sub

#End Region

#Region " Eventi Toolbar "

	Private Sub ToolBar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

		Me.PanelUtility.ManagingToolbar(e.Button.Key)

		'gestione del pulsante di stampa (modifica 27/08/2004)
		If e.Button.Key = "btnStampa" Then
			Stampa()
		End If
		If e.Button.Key = "btnAssociaCentri" Then
			LoadUsl()
			LoadDistretto("")
			BindConsultori(ddlUls.SelectedValue, ddlDistretto.SelectedValue)
			modAssociaCentri.VisibileMD = True
		End If

	End Sub
	Private Sub ToolBarAssocia_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBarAssocia.ButtonClicked

		Select Case be.Button.Key

			Case "btnFind"
				BindConsultori(ddlUls.SelectedValue, ddlDistretto.SelectedValue)
			Case "btnSave"

				Salva()

				'Me.StatoPagina = PageStatus.View
				Me.BindConsultori(ddlUls.SelectedValue, ddlDistretto.SelectedValue)

			Case "btnAnnulla"

				'Me.StatoPagina = PageStatus.View
				Me.BindConsultori(ddlUls.SelectedValue, ddlDistretto.SelectedValue)

		End Select

	End Sub
	Protected Sub ddlDistretto_SelectedIndexChanged(sender As Object, e As EventArgs)
		BindConsultori(ddlUls.SelectedValue, ddlDistretto.SelectedValue)
	End Sub
	Protected Sub ddlUls_SelectedIndexChanged(sender As Object, e As EventArgs)
		LoadDistretto(ddlUls.SelectedValue)
		BindConsultori(ddlUls.SelectedValue, ddlDistretto.SelectedValue)
	End Sub
#End Region

#Region " Eventi OnitDataPanel e PanelUtility "

	Private Sub PanelUtility_BeforeSave(sender As Object, e As OnitDataPanelUtility.BeforeOperationEventArgs)

        If Me.odpOperatori.CurrentOperation = Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord Then
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

    Private Sub odpOperatori_afterSaveData(sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles odpOperatori.afterSaveData
        Me.odpOperatori.Find()
    End Sub

    Private Sub odpOperatoriafterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpOperatori.afterOperation

        Me.OnitLayout31.Busy = Me.PanelUtility.CheckToolBarState(operation)

        Dim btnStampa As Infragistics.WebUI.UltraWebToolbar.TBarButton = ToolBar.Items.FromKeyButton("btnStampa")
        If Not btnStampa Is Nothing Then
            If operation = OnitDataPanel.OnitDataPanel.CurrentOperationTypes.EditRecord Or
               operation = OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord Or
               operation = OnitDataPanel.OnitDataPanel.CurrentOperationTypes.DeleteRecord Then
                '--
                btnStampa.Enabled = False
                '--
            Else
                '--
                btnStampa.Enabled = True
                '--
            End If
        End If

    End Sub

    Private Sub odpOperatori_onCreateQuery(ByRef QB As System.Object) Handles odpOperatori.onCreateQuery
        DirectCast(QB, AbstractQB).AddOrderByFields("ope_nome")
    End Sub

#End Region

#Region " Private Methods "

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()
        listPrintButtons.Add(New PrintButton(Constants.ReportName.Operatori, "btnStampa"))

        Me.ShowToolbarPrintButtons(listPrintButtons, ToolBar)

    End Sub

	'stampa del report degli operatori (modifica 27/08/2004)
	Private Sub Stampa()

		Dim rpt As New ReportParameter()

		'filtro per recuperare il campo qualifica
		Dim strFiltro As String = "{T_ANA_CODIFICHE.COD_CAMPO} = 'OPE_QUALIFICA'"

		'filtro di ricerca
		Dim filtroRic As String = CType(WzFilter1.FindControl("WzFilterKeyBase"), TextBox).Text
		strFiltro &= " AND (INSTR({T_ANA_OPERATORI.OPE_CODICE}, '" & filtroRic.ToUpper() & "') > 0"
		strFiltro &= " OR INSTR({T_ANA_OPERATORI.OPE_NOME}, '" & filtroRic.ToUpper() & "') > 0)"

		Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
			Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

				If Not OnVacReport.StampaReport(Page.Request.Path, Constants.ReportName.Operatori, strFiltro, , , , bizReport.GetReportFolder(Constants.ReportName.Operatori)) Then
					OnVacUtility.StampaNonPresente(Page, Constants.ReportName.Operatori)
				End If

			End Using
		End Using

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
	Private Sub Salva()

		Dim listConsultoriOperatore As New List(Of Entities.ConsultorioOperatore)()

		For Each datagridItem As DataGridItem In dgrConsultori.Items

			If DirectCast(datagridItem.FindControl("chkConsultorio"), CheckBox).Checked Then

				Dim consultorioOperatore As New Entities.ConsultorioOperatore()
				consultorioOperatore.CodiceConsultorio = HttpUtility.HtmlDecode(datagridItem.Cells(DgrConsultoriColumnIndex.Codice).Text.Trim())

				consultorioOperatore.CodiceOperatore = CodiceOperatoreSelezionato

				listConsultoriOperatore.Add(consultorioOperatore)

			End If

		Next



		Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
			Using bizOp As New Biz.BizOperatori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

				bizOp.SaveConsultoriOperatore(CodiceOperatoreSelezionato, listConsultoriOperatore)

			End Using
		End Using

	End Sub
	Private Sub BindConsultori(usl As String, distretto As String)

		Dim listConsultoriOperatore As New List(Of OnVac.Entities.ConsultorioOperatore)()
		Dim listConsultoriAbilitati As List(Of Entities.ConsultorioOperatore) = Nothing
		Dim listConsultoriAmbiti As List(Of Entities.ConsultorioOperatore) = Nothing

		Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
			Using bizConsultori As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

				' Caricamento di tutti i consultori aperti
				Dim listConsultori As List(Of Entities.Consultorio) =
					bizConsultori.GetListCodiceDescrizioneConsultori(True, Nothing)

				Dim consultorioUtente As Entities.ConsultorioOperatore = Nothing

				For Each consultorio As Entities.Consultorio In listConsultori

					consultorioUtente = New Entities.ConsultorioOperatore()
					consultorioUtente.Abilitato = False
					consultorioUtente.CodiceConsultorio = consultorio.Codice
					consultorioUtente.DescrizioneConsultorio = consultorio.Descrizione

					listConsultoriOperatore.Add(consultorioUtente)

				Next

			End Using
			listConsultoriAmbiti = LoadLinkConsultoriOperatori(String.Empty)
			If Not String.IsNullOrEmpty(CodiceOperatoreSelezionato) Then
				listConsultoriAbilitati = LoadLinkConsultoriOperatori(CodiceOperatoreSelezionato)
			End If
		End Using

		For Each consultorioUsl As Entities.ConsultorioOperatore In listConsultoriAmbiti
			Dim consulAsl As Entities.ConsultorioOperatore =
					listConsultoriOperatore.Where(Function(p) p.CodiceConsultorio = consultorioUsl.CodiceConsultorio).FirstOrDefault()
			If Not consulAsl Is Nothing Then
				consulAsl.CodiceUsl = consultorioUsl.CodiceUsl
				consulAsl.CodiceDistretto = consultorioUsl.CodiceDistretto
				consulAsl.DescrizioneUsl = consultorioUsl.DescrizioneUsl
				consulAsl.DescrizioneDistretto = consultorioUsl.DescrizioneDistretto
			End If
		Next
		If Not listConsultoriAbilitati Is Nothing AndAlso listConsultoriAbilitati.Count > 0 Then


			For Each consultorioAbilitato As Entities.ConsultorioOperatore In listConsultoriAbilitati

				Dim consultorioUtente As Entities.ConsultorioOperatore =
					listConsultoriOperatore.Where(Function(p) p.CodiceConsultorio = consultorioAbilitato.CodiceConsultorio).FirstOrDefault()

				If Not consultorioUtente Is Nothing Then
					consultorioUtente.Abilitato = True
					consultorioUtente.CodiceOperatore = consultorioAbilitato.CodiceOperatore
				End If

			Next

		End If
		If Not String.IsNullOrEmpty(usl) Then
			listConsultoriOperatore = listConsultoriOperatore.Where(Function(p) p.CodiceUsl = usl).ToList()
		End If
		If Not String.IsNullOrEmpty(distretto) Then
			listConsultoriOperatore = listConsultoriOperatore.Where(Function(p) p.CodiceDistretto = distretto).ToList()
		End If
		listConsultoriOperatore = listConsultoriOperatore.OrderBy(Function(p) p.CodiceUsl).ThenBy(Function(p) p.DescrizioneDistretto).ThenBy(Function(p) p.DescrizioneConsultorio).ToList()

		Me.dgrConsultori.DataSource = listConsultoriOperatore
		Me.dgrConsultori.DataBind()

	End Sub
	Private Function LoadLinkConsultoriOperatori(codice As String) As List(Of Entities.ConsultorioOperatore)

		Dim listConsultoriOperatore As List(Of Entities.ConsultorioOperatore) = Nothing

		Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
			Using bizOp As New Biz.BizOperatori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

				listConsultoriOperatore = bizOp.GetListConsultoriOperatori(codice)

			End Using
		End Using

		Return listConsultoriOperatore

	End Function

#End Region

End Class
