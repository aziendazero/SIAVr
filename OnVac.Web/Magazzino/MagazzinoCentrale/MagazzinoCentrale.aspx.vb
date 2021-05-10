Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.Data


Public Class MagazzinoCentrale
    Inherits OnVac.Common.PageBase

#Region " Enums "

    Private Enum DgrLottiColumnIndex
        ButtonDettaglio = 0
        ButtonModifica = 1
        CodiceLotto = 2
        DescrizioneLotto = 3
        DescrizioneNomeCommerciale = 4
        DosiRimaste = 5
        DataPreparazioneString = 6
        DataScadenzaString = 7
        LabelScortaNulla = 8
        LabelScortaInsufficiente = 9
        DataPreparazioneDate = 10           ' nascosta
        DataScadenzaDate = 11               ' nascosta
        CodiceNomeCommerciale = 12          ' nascosta
        QuantitaMinima = 13                 ' nascosta
        Obsoleto = 14                       ' nascosta
    End Enum

    Private Enum DgrDettaglioDosiColumnIndex
        ButtonMovimenti = 0
        DescrizioneConsultorio = 1
        DosiRimaste = 2
        QuantitaMinima = 3
        Attivo = 4
        CodiceConsultorio = 5               ' nascosta
        CodiceLotto = 6                     ' nascosta
        DescrizioneLotto = 7                ' nascosta
    End Enum

#End Region

#Region " Properties "

    Private Property Campo1OrdinamentoDatagrid() As String
        Get
            If ViewState("CampoOrdinamento1_MagazzinoCentrale") Is Nothing Then ViewState("CampoOrdinamento1_MagazzinoCentrale") = String.Empty
            Return ViewState("CampoOrdinamento1_MagazzinoCentrale")
        End Get
        Set(value As String)
            ViewState("CampoOrdinamento1_MagazzinoCentrale") = value
        End Set
    End Property

    Private Property Campo2OrdinamentoDatagrid() As String
        Get
            If ViewState("CampoOrdinamento2_MagazzinoCentrale") Is Nothing Then ViewState("CampoOrdinamento2_MagazzinoCentrale") = String.Empty
            Return ViewState("CampoOrdinamento2_MagazzinoCentrale")
        End Get
        Set(value As String)
            ViewState("CampoOrdinamento2_MagazzinoCentrale") = value
        End Set
    End Property

    Private Property Campo3OrdinamentoDatagrid() As String
        Get
            If ViewState("CampoOrdinamento3_MagazzinoCentrale") Is Nothing Then ViewState("CampoOrdinamento3_MagazzinoCentrale") = String.Empty
            Return ViewState("CampoOrdinamento3_MagazzinoCentrale")
        End Get
        Set(value As String)
            ViewState("CampoOrdinamento3_MagazzinoCentrale") = value
        End Set
    End Property

    Private Property Verso1OrdinamentoDatagrid() As String
        Get
            If ViewState("VersoOrdinamento1_MagazzinoCentrale") Is Nothing Then ViewState("VersoOrdinamento1_MagazzinoCentrale") = "ASC"
            Return ViewState("VersoOrdinamento1_MagazzinoCentrale")
        End Get
        Set(value As String)
            ViewState("VersoOrdinamento1_MagazzinoCentrale") = value
        End Set
    End Property

    Private Property Verso2OrdinamentoDatagrid() As String
        Get
            If ViewState("VersoOrdinamento2_MagazzinoCentrale") Is Nothing Then ViewState("VersoOrdinamento2_MagazzinoCentrale") = "ASC"
            Return ViewState("VersoOrdinamento2_MagazzinoCentrale")
        End Get
        Set(value As String)
            ViewState("VersoOrdinamento2_MagazzinoCentrale") = value
        End Set
    End Property

    Private Property Verso3OrdinamentoDatagrid() As String
        Get
            If ViewState("VersoOrdinamento3_MagazzinoCentrale") Is Nothing Then ViewState("VersoOrdinamento3_MagazzinoCentrale") = "ASC"
            Return ViewState("VersoOrdinamento3_MagazzinoCentrale")
        End Get
        Set(value As String)
            ViewState("VersoOrdinamento3_MagazzinoCentrale") = value
        End Set
    End Property

    ' Lista dei dettagli aperti nel datagrid, individuati per codice lotto
    Public Property CurrentDetailsList() As List(Of String)
        Get
            If ViewState("CurrentDetailsList_MagazzinoCentrale") Is Nothing Then ViewState("CurrentDetailsList_MagazzinoCentrale") = New List(Of String)
            Return ViewState("CurrentDetailsList_MagazzinoCentrale")
        End Get
        Set(value As List(Of String))
            ViewState("CurrentDetailsList_MagazzinoCentrale") = value
        End Set
    End Property

#End Region

#Region " Page Events "

    Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        MagazzinoUtility.SetToolbarButtonImages("btnCerca", "cerca.gif", ToolBar)
        MagazzinoUtility.SetToolbarButtonImages("btnStampa", "stampa.gif", ToolBar)

    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then

            Me.ClearCurrentDetails()

            ' Titolo pagina
            OnVacUtility.ImpostaCnsLavoro(Me.OnitLayout31)

            ' Lettura filtri di ricerca da querystring
            Dim filtroLottiMagazzino As Filters.FiltriRicercaLottiMagazzino =
                MagazzinoUtility.GetFiltersFromQueryString(Me.Request.QueryString)

            If Not filtroLottiMagazzino Is Nothing Then
                ' Se nella querystring ci sono i filtri, li imposto nello user control
                Me.ucFiltriRicerca.SetFiltri(filtroLottiMagazzino)
            Else
                ' Imposto solo il filtro sui lotti scaduti
                Me.ucFiltriRicerca.SetFiltroLottiScaduti(True)
            End If

            ' Impostazione campi ordinamento da querystring
            Me.SetOrdinamentoFromQueryString()

            ' Lettura flag di effettuazione della ricerca
            If Me.GetFlagRicercaFromQueryString() Then

                ' Eseguo la ricerca in base ai filtri impostati
                Me.RicercaLotti(Me.GetDatagridCurrentPageFromQueryString(), True, True)

            Else

                ' Visualizzazione datagrid vuoto
                Me.BindDatagridLotti(Nothing, 0, 0)

            End If

            ' Pulsante di stampa visibile solo se la stampa è abilitata
            If Not MagazzinoUtility.ExistsReport(Constants.ReportName.MagazzinoLotti) Then

                Me.ToolBar.Items.FromKeyButton("btnStampa").Visible = False

            End If

        End If

    End Sub

    Private Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender

        Me.RegisterScriptHeaderVisibilityDgrLotti()

    End Sub

#End Region

#Region " Querystring "

    Private Function GetFlagRicercaFromQueryString() As Boolean

        Dim ricerca As String = Request.QueryString.Get("cerca")

        If Not String.IsNullOrEmpty(ricerca) Then
            Return Convert.ToBoolean(ricerca)
        End If

        Return False

    End Function

    Private Sub SetOrdinamentoFromQueryString()

        Dim campiOrdinamento As String() = Me.SplitValoriOrdinamento(Request.QueryString.Get("ord"))

        Me.Campo1OrdinamentoDatagrid = campiOrdinamento(0)
        Me.Campo2OrdinamentoDatagrid = campiOrdinamento(1)
        Me.Campo3OrdinamentoDatagrid = campiOrdinamento(2)

        Dim versiOrdinamento As String() = Me.SplitValoriOrdinamento(Request.QueryString.Get("ver"))

        Me.Verso1OrdinamentoDatagrid = versiOrdinamento(0)
        Me.Verso2OrdinamentoDatagrid = versiOrdinamento(1)
        Me.Verso3OrdinamentoDatagrid = versiOrdinamento(2)

    End Sub

    Private Function SplitValoriOrdinamento(valoriOrdinamento As String) As String()

        If String.IsNullOrEmpty(valoriOrdinamento) Then

            Return New String() {String.Empty, String.Empty, String.Empty}

        End If

        Return valoriOrdinamento.Split(",")

    End Function

    Private Function GetDatagridCurrentPageFromQueryString() As Integer

        If String.IsNullOrEmpty(Request.QueryString.Get("page")) Then Return 0

        Dim currentPageIndex As Integer = 0

        If Not Integer.TryParse(Request.QueryString.Get("page"), currentPageIndex) Then Return 0

        Return currentPageIndex

    End Function

#End Region

#Region " Layout "

    Private Sub BindDatagridLotti(listLottiMagazzino As List(Of Entities.LottoMagazzino), countLotti As Integer, currentPageIndex As Integer)

        If listLottiMagazzino Is Nothing Then listLottiMagazzino = New List(Of Entities.LottoMagazzino)()

        Me.dgrLotti.VirtualItemCount = countLotti
        Me.dgrLotti.CurrentPageIndex = currentPageIndex

        Me.dgrLotti.DataSource = listLottiMagazzino
        Me.dgrLotti.DataBind()

        Me.dgrLotti.PagerStyle.Visible = (Me.dgrLotti.Items.Count > 0)

    End Sub

    Private Sub SetDatagridLottiLabelVisibility(datagridItem As DataGridItem, labelName As String, visible As Boolean)

        DirectCast(datagridItem.FindControl(labelName), System.Web.UI.WebControls.Label).Visible = visible

    End Sub

    Private Sub AggiornaLabelRisultati(countLotti As Integer?)

        If countLotti Is Nothing Then
            Me.LayoutTitolo_sezione.Text = "ELENCO LOTTI"
        Else
            Me.LayoutTitolo_sezione.Text = String.Format("ELENCO LOTTI: {0} risultat{1}", countLotti.ToString(), IIf(countLotti = 1, "o", "i"))
        End If

    End Sub

    Private Sub ImpostaFrecceOrdinamentoDettaglio(datagridItem As DataGridItem, prefissoImmagine As String, ordinamentoAsc As Boolean)

        datagridItem.FindControl(String.Format("{0}_up", prefissoImmagine)).Visible = ordinamentoAsc
        datagridItem.FindControl(String.Format("{0}_down", prefissoImmagine)).Visible = Not ordinamentoAsc

    End Sub

    Private Sub NascondiFrecceOrdinamentoDettaglio(datagridItem As DataGridItem, prefissoImmagine As String)

        datagridItem.FindControl(String.Format("{0}_up", prefissoImmagine)).Visible = False
        datagridItem.FindControl(String.Format("{0}_down", prefissoImmagine)).Visible = False

    End Sub

#Region " Script per visualizzare le frecce di ordinamento nell'header del datagrid lotti "

    Private Sub RegisterScriptHeaderVisibilityDgrLotti()

        Dim scriptVisibility As New System.Text.StringBuilder()

        scriptVisibility.AppendLine("<script type='text/javascript'>")

        If Me.Campo1OrdinamentoDatagrid = Me.dgrLotti.Columns(DgrLottiColumnIndex.CodiceLotto).SortExpression Then
            Me.CreaScriptVisibility("cl", (Me.Verso1OrdinamentoDatagrid = "ASC"), scriptVisibility)
        Else
            Me.CreaScriptVisibility("cl", scriptVisibility)
        End If

        If Me.Campo1OrdinamentoDatagrid = Me.dgrLotti.Columns(DgrLottiColumnIndex.DescrizioneLotto).SortExpression Then
            Me.CreaScriptVisibility("dl", (Me.Verso1OrdinamentoDatagrid = "ASC"), scriptVisibility)
        Else
            Me.CreaScriptVisibility("dl", scriptVisibility)
        End If

        If Me.Campo1OrdinamentoDatagrid = Me.dgrLotti.Columns(DgrLottiColumnIndex.DescrizioneNomeCommerciale).SortExpression Then
            Me.CreaScriptVisibility("nc", (Me.Verso1OrdinamentoDatagrid = "ASC"), scriptVisibility)
        Else
            Me.CreaScriptVisibility("nc", scriptVisibility)
        End If

        If Me.Campo1OrdinamentoDatagrid = Me.dgrLotti.Columns(DgrLottiColumnIndex.DataPreparazioneString).SortExpression Then
            Me.CreaScriptVisibility("dp", (Me.Verso1OrdinamentoDatagrid = "ASC"), scriptVisibility)
        Else
            Me.CreaScriptVisibility("dp", scriptVisibility)
        End If

        If Me.Campo1OrdinamentoDatagrid = Me.dgrLotti.Columns(DgrLottiColumnIndex.DataScadenzaString).SortExpression Then
            Me.CreaScriptVisibility("ds", (Me.Verso1OrdinamentoDatagrid = "ASC"), scriptVisibility)
        Else
            Me.CreaScriptVisibility("ds", scriptVisibility)
        End If

        If Me.Campo1OrdinamentoDatagrid = Me.dgrLotti.Columns(DgrLottiColumnIndex.DosiRimaste).SortExpression Then
            Me.CreaScriptVisibility("dr", (Me.Verso1OrdinamentoDatagrid = "ASC"), scriptVisibility)
        Else
            Me.CreaScriptVisibility("dr", scriptVisibility)
        End If

        scriptVisibility.AppendLine("function setHeaderVisibility(id, visible) {")
        scriptVisibility.AppendLine("if (document.getElementById(id) != null) {")
        scriptVisibility.AppendLine("if (visible) {")
        scriptVisibility.AppendLine("   document.getElementById(id).style.visibility = 'visible';")
        scriptVisibility.AppendLine("} else {")
        scriptVisibility.AppendLine("   document.getElementById(id).style.visibility = 'hidden';")
        scriptVisibility.AppendLine("}}")
        scriptVisibility.AppendLine("return;}")
        scriptVisibility.AppendLine("</script>")

        Page.ClientScript.RegisterStartupScript(Me.GetType(), "jsHeaderVisibility", scriptVisibility.ToString())

    End Sub

    Private Sub CreaScriptVisibility(prefix As String, up As Boolean, ByRef scriptVisibility As System.Text.StringBuilder)

        scriptVisibility.AppendFormat("setHeaderVisibility('{0}_up', {1});", prefix, up.ToString().ToLower())
        scriptVisibility.AppendFormat("setHeaderVisibility('{0}_down', {1});", prefix, (Not up).ToString().ToLower())

    End Sub

    Private Sub CreaScriptVisibility(prefix As String, ByRef scriptVisibility As System.Text.StringBuilder)

        scriptVisibility.AppendFormat("setHeaderVisibility('{0}_up', false);", prefix)
        scriptVisibility.AppendFormat("setHeaderVisibility('{0}_down', false);", prefix)

    End Sub

#End Region

#End Region

#Region " Toolbar Events "

    Private Sub ToolBar_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case be.Button.Key

            Case "btnCerca"

                Me.RicercaLotti(0, True, True)

            Case "btnStampa"

                Me.StampaMagazzinoLotti()

            Case "btnInserisci"

                Me.RedirectToDettaglio(String.Empty, -1)

            Case "btnPulisci"

                Me.Pulisci()

        End Select

    End Sub

    Private Sub Pulisci()

        ' Cancello i filtri, imposto solo il filtro sui lotti scaduti 
        Me.ucFiltriRicerca.Clear()
        Me.ucFiltriRicerca.SetFiltroLottiScaduti(True)

        ' Visualizzazione datagrid vuoto
        Me.BindDatagridLotti(Nothing, 0, 0)

        ' Cancellazione risultati label
        Me.AggiornaLabelRisultati(Nothing)

        ' Cancellazione indici dettagli correnti
        Me.ClearCurrentDetails()

    End Sub

#End Region

#Region " Ricerca "

    ' Impostazione campi di default per l'ordinamento del datagrid, in base ai parametri relativi al magazzino
    Private Sub SetCampiOrdinamentoDefault()

        Me.Campo1OrdinamentoDatagrid = Me.dgrLotti.Columns(DgrLottiColumnIndex.DosiRimaste).SortExpression
        Me.Verso1OrdinamentoDatagrid = "DESC"

        Me.Campo2OrdinamentoDatagrid = Me.dgrLotti.Columns(DgrLottiColumnIndex.CodiceLotto).SortExpression
        Me.Verso2OrdinamentoDatagrid = "ASC"

        Me.Campo3OrdinamentoDatagrid = Me.dgrLotti.Columns(DgrLottiColumnIndex.DescrizioneNomeCommerciale).SortExpression
        Me.Verso3OrdinamentoDatagrid = "ASC"

    End Sub

    ' Impostazione campi specificati per l'ordinamento del datagrid.
    Private Sub SetCampiOrdinamentoDatagrid(sortExpression As String)

        ' Impostazione campo e verso dell'ordinamento
        If Me.Campo1OrdinamentoDatagrid = sortExpression Then
            Me.Verso1OrdinamentoDatagrid = IIf(Me.Verso1OrdinamentoDatagrid = "ASC", "DESC", "ASC")
        Else
            Me.Verso1OrdinamentoDatagrid = "ASC"
        End If

        Me.Campo1OrdinamentoDatagrid = sortExpression

        ' Impostazione del secondo e terzo campo di ordinamento, in base al primo
        Select Case Me.Campo1OrdinamentoDatagrid

            Case Me.dgrLotti.Columns(DgrLottiColumnIndex.DosiRimaste).SortExpression

                Me.Campo2OrdinamentoDatagrid = Me.dgrLotti.Columns(DgrLottiColumnIndex.CodiceLotto).SortExpression
                Me.Verso2OrdinamentoDatagrid = "ASC"

                Me.Campo3OrdinamentoDatagrid = Me.dgrLotti.Columns(DgrLottiColumnIndex.DescrizioneNomeCommerciale).SortExpression
                Me.Verso3OrdinamentoDatagrid = "ASC"

            Case Me.dgrLotti.Columns(DgrLottiColumnIndex.CodiceLotto).SortExpression

                Me.Campo2OrdinamentoDatagrid = Me.dgrLotti.Columns(DgrLottiColumnIndex.DosiRimaste).SortExpression
                Me.Verso2OrdinamentoDatagrid = "DESC"

                Me.Campo3OrdinamentoDatagrid = Me.dgrLotti.Columns(DgrLottiColumnIndex.DescrizioneNomeCommerciale).SortExpression
                Me.Verso3OrdinamentoDatagrid = "ASC"

            Case Else

                Me.Campo2OrdinamentoDatagrid = Me.dgrLotti.Columns(DgrLottiColumnIndex.DosiRimaste).SortExpression
                Me.Verso2OrdinamentoDatagrid = "DESC"

                Me.Campo3OrdinamentoDatagrid = Me.dgrLotti.Columns(DgrLottiColumnIndex.CodiceLotto).SortExpression
                Me.Verso3OrdinamentoDatagrid = "ASC"

        End Select

    End Sub

    Private Sub RicercaLotti(currentPageIndex As Integer, chiudiDettaglioLottiCorrenti As Boolean, usaFiltriCorrenti As Boolean)

        ' Chiusura dettagli
        If chiudiDettaglioLottiCorrenti Then
            Me.ClearCurrentDetails()
        End If

        ' Impostazione ordinamento di default se non specificato
        If String.IsNullOrEmpty(Me.Campo1OrdinamentoDatagrid) Then
            Me.SetCampiOrdinamentoDefault()
        End If

        Dim orderedList As List(Of Entities.LottoMagazzino) = Nothing

        Dim listLottiMagazzino As List(Of Entities.LottoMagazzino) = Nothing

        Dim countLotti As Integer = 0

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizLotti As New Biz.BizLotti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                Dim filtriRicerca As Filters.FiltriRicercaLottiMagazzino = Nothing

                If usaFiltriCorrenti Then
                    filtriRicerca = Me.ucFiltriRicerca.GetFiltriCorrenti(True)
                Else
                    filtriRicerca = Me.ucFiltriRicerca.GetUltimiFiltriUtilizzati()
                End If

				countLotti = bizLotti.CountLottiMagazzinoCentrale(filtriRicerca, OnVacContext.UserId)

				listLottiMagazzino = Me.LoadLottiMagazzinoCentrale(bizLotti, True, filtriRicerca, currentPageIndex, countLotti)

            End Using

        End Using

        Me.BindDatagridLotti(listLottiMagazzino, countLotti, currentPageIndex)

        ' Visualizzazione numero risultati
        Me.AggiornaLabelRisultati(countLotti)

    End Sub

    ' Restituisce la lista di elementi in base ai filtri impostati dall'utente.
    Private Function LoadLottiMagazzinoCentrale(bizLotti As Biz.BizLotti, doPaging As Boolean, filtriRicerca As Filters.FiltriRicercaLottiMagazzino, currentPageIndex As Integer, countLotti As Integer) As List(Of Entities.LottoMagazzino)

        Dim listLottiMagazzino As List(Of Entities.LottoMagazzino) = Nothing

        ' Ordinamento
        Dim listDatiOrdinamento As List(Of Entities.DatiOrdinamento) = Me.GetListaCampiOrdinamento()

        ' Paginazione
        Dim pagingOptions As PagingOptions = Nothing

        If doPaging Then

            Dim startIndex As Integer = currentPageIndex * Me.dgrLotti.PageSize

            If startIndex > countLotti - 1 Then
                startIndex = 0
                currentPageIndex = 0
            End If

            pagingOptions = New PagingOptions()

            pagingOptions.StartRecordIndex = startIndex
            pagingOptions.EndRecordIndex = startIndex + Me.dgrLotti.PageSize

        End If

		listLottiMagazzino = bizLotti.LoadLottiMagazzinoCentrale(filtriRicerca, listDatiOrdinamento, pagingOptions, OnVacContext.UserId)

		Return listLottiMagazzino

    End Function

#End Region

#Region " Eventi Datagrid Lotti "

    Private Sub dgrLotti_ItemCreated(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrLotti.ItemCreated

        Select Case e.Item.ItemType

            Case ListItemType.Item, ListItemType.AlternatingItem, ListItemType.EditItem, ListItemType.SelectedItem

                Dim objDettaglio As Control = e.Item.Cells(DgrLottiColumnIndex.DescrizioneNomeCommerciale).FindControl("dgrDettaglio")

                If Not objDettaglio Is Nothing Then

                    Dim dgrDettaglioDosi As DataGrid = DirectCast(objDettaglio, DataGrid)

                    AddHandler dgrDettaglioDosi.SortCommand, AddressOf dgrDettaglioDosi_SortCommand
                    AddHandler dgrDettaglioDosi.ItemCommand, AddressOf dgrDettaglioDosi_ItemCommand
                    AddHandler dgrDettaglioDosi.ItemCreated, AddressOf dgrDettaglioDosi_ItemCreated

                End If

        End Select

    End Sub

    Private Sub dgrLotti_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrLotti.ItemDataBound

        Select Case e.Item.ItemType

            Case ListItemType.Item, ListItemType.AlternatingItem, ListItemType.EditItem, ListItemType.SelectedItem

                ' Entità associata alla riga corrente
                Dim lottoMagazzino As Entities.LottoMagazzino = Nothing

                Try
                    lottoMagazzino = DirectCast(e.Item.DataItem, Entities.LottoMagazzino)
                Catch ex As Exception
                    lottoMagazzino = Nothing
                End Try

                If lottoMagazzino Is Nothing Then Exit Sub

                ' Descrizione nome commerciale
                Dim lblDescrizioneNomeCommerciale As Label =
                    DirectCast(e.Item.Cells(DgrLottiColumnIndex.DescrizioneNomeCommerciale).FindControl("lblDescrizioneNomeCommerciale"), Label)

                lblDescrizioneNomeCommerciale.Text = lottoMagazzino.DescrizioneNomeCommerciale

                ' Panel dettagli lotto
                Dim panelDettaglio As Panel = DirectCast(e.Item.Cells(DgrLottiColumnIndex.DescrizioneNomeCommerciale).FindControl("panelDettaglio"), Panel)

                If panelDettaglio.Style.Item("display") Is Nothing Then
                    panelDettaglio.Style.Item("display") = "none"
                Else
                    panelDettaglio.Style.Add("display", "none")
                End If

                ' Datagrid dettagli lotto
                Dim dgrDettaglioDosi As DataGrid = DirectCast(e.Item.Cells(DgrLottiColumnIndex.DescrizioneNomeCommerciale).FindControl("dgrDettaglio"), DataGrid)

                If Me.CurrentDetailsList.Contains(lottoMagazzino.CodiceLotto) Then

                    ' Dettaglio da aprire e ricaricare
                    Dim btnDettaglioDosi As ImageButton =
                        DirectCast(e.Item.Cells(DgrLottiColumnIndex.DescrizioneNomeCommerciale).FindControl("btnDettaglioDosi"), System.Web.UI.WebControls.Image)

                    Me.ShowDettaglioDosi(btnDettaglioDosi, panelDettaglio)

                    ' Mantengo l'ordinamento impostato
                    Dim campoOrdinamento As String = dgrDettaglioDosi.Attributes("CampoOrdinamento")
                    Dim versoOrdinamento As String = dgrDettaglioDosi.Attributes("VersoOrdinamento")

                    Me.CaricaDettaglioDosi(dgrDettaglioDosi, lottoMagazzino.CodiceLotto, lottoMagazzino.DescrizioneLotto,
                                           campoOrdinamento, versoOrdinamento)
                Else

                    Me.BindDatagridDettaglioDosi(dgrDettaglioDosi, Nothing)

                End If

                ' Data preparazione
                Dim lblDataPreparazione As Label = DirectCast(e.Item.Cells(DgrLottiColumnIndex.DataPreparazioneString).FindControl("lblDataPreparazione"), Label)

                If lottoMagazzino.DataPreparazione = Date.MinValue Then
                    lblDataPreparazione.Text = String.Empty
                Else
                    lblDataPreparazione.Text = lottoMagazzino.DataPreparazione.ToString("dd/MM/yyyy")
                End If

                ' Data scadenza
                Dim lblDataScadenza As Label = DirectCast(e.Item.Cells(DgrLottiColumnIndex.DataScadenzaString).FindControl("lblDataScadenza"), Label)

                If lottoMagazzino.DataScadenza = Date.MinValue Then
                    lblDataScadenza.Text = String.Empty
                Else
                    lblDataScadenza.Text = lottoMagazzino.DataScadenza.ToString("dd/MM/yyyy")

                    If lottoMagazzino.DataScadenza < Date.Now Then
                        e.Item.Cells(DgrLottiColumnIndex.DataScadenzaString).ForeColor = Color.Red
                    End If
                End If

                ' Dosi rimaste e quantità minima
                If lottoMagazzino.DosiRimaste = 0 Then

                    Me.SetDatagridLottiLabelVisibility(e.Item, "lb_null", True)
                    Me.SetDatagridLottiLabelVisibility(e.Item, "lb_alert", False)

                Else

                    Me.SetDatagridLottiLabelVisibility(e.Item, "lb_null", False)
                    Me.SetDatagridLottiLabelVisibility(e.Item, "lb_alert", lottoMagazzino.DosiRimaste < lottoMagazzino.QuantitaMinima)

                End If

        End Select

    End Sub

    Private Sub dgrLotti_PageIndexChanged(source As Object, e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dgrLotti.PageIndexChanged

        Me.RicercaLotti(e.NewPageIndex, False, False)

    End Sub

    Private Sub dgrLotti_ItemCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgrLotti.ItemCommand

        Select Case e.CommandName

            Case "DettaglioDosi"

                Dim codiceLotto As String = e.Item.Cells(DgrLottiColumnIndex.CodiceLotto).Text

                Dim descrizioneLotto As String = e.Item.Cells(DgrLottiColumnIndex.DescrizioneLotto).Text

                Dim btnDettaglioDosi As ImageButton = DirectCast(e.Item.Cells(DgrLottiColumnIndex.DescrizioneNomeCommerciale).FindControl("btnDettaglioDosi"), System.Web.UI.WebControls.Image)

                Dim panelDettaglio As Panel = DirectCast(e.Item.Cells(DgrLottiColumnIndex.DescrizioneNomeCommerciale).FindControl("panelDettaglio"), Panel)

                Dim dgrDettaglioDosi As DataGrid = DirectCast(e.Item.Cells(DgrLottiColumnIndex.DescrizioneNomeCommerciale).FindControl("dgrDettaglio"), DataGrid)

                If Me.ToggleDettaglioDosiVisibility(btnDettaglioDosi, panelDettaglio) Then

                    Me.CurrentDetailsList.Add(codiceLotto)

                    Me.CaricaDettaglioDosi(dgrDettaglioDosi, codiceLotto, descrizioneLotto, String.Empty, String.Empty)

                Else

                    Me.CurrentDetailsList.Remove(codiceLotto)

                End If

        End Select

    End Sub

    Private Sub dgrLotti_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles dgrLotti.SelectedIndexChanged

        Me.RedirectToDettaglio(Me.dgrLotti.SelectedItem.Cells(DgrLottiColumnIndex.CodiceLotto).Text, Me.dgrLotti.CurrentPageIndex)

    End Sub

    Private Sub dgrLotti_SortCommand(source As Object, e As System.Web.UI.WebControls.DataGridSortCommandEventArgs) Handles dgrLotti.SortCommand

        Me.SetCampiOrdinamentoDatagrid(e.SortExpression)

        Me.RicercaLotti(Me.dgrLotti.CurrentPageIndex, False, False)

    End Sub

#End Region

#Region " Dettaglio dosi per magazzino "

    Private Sub ShowDettaglioDosi(btnDettaglioDosi As ImageButton, panelDettaglio As Panel)

        btnDettaglioDosi.ImageUrl="~/Images/meno.gif"
        btnDettaglioDosi.ToolTip = "Nasconde i dettagli delle dosi per magazzino"

        Me.TogglePanelVisibility(panelDettaglio, True)

    End Sub

    ' Mostra/Nasconde il dettaglio delle dosi e imposta il pulsante della riga del dgrLotti
    ' Restituisce true se il dettaglio è visibile, false altrimenti
    Private Function ToggleDettaglioDosiVisibility(btnDettaglioDosi As ImageButton, panelDettaglio As Panel) As Boolean

        Dim dettaglioVisible As Boolean = False

        If btnDettaglioDosi.ImageUrl.IndexOf("piu.gif") >= 0 Then

            btnDettaglioDosi.ImageUrl="~/Images/meno.gif"
            btnDettaglioDosi.ToolTip = "Nasconde i dettagli delle dosi per magazzino"

            dettaglioVisible = True

        Else

            btnDettaglioDosi.ImageUrl="~/Images/piu.gif"
            btnDettaglioDosi.ToolTip = "Mostra i dettagli delle dosi per magazzino"

            dettaglioVisible = False

        End If

        Me.TogglePanelVisibility(panelDettaglio, dettaglioVisible)

        Return dettaglioVisible

    End Function

    Private Sub TogglePanelVisibility(panelDettaglio As Panel, showDetail As Boolean)

        Dim displayValue As String = String.Empty

        If showDetail Then
            displayValue = "block"
        Else
            displayValue = "none"
        End If

        If panelDettaglio.Style.Item("display") Is Nothing Then
            panelDettaglio.Style.Add("display", displayValue)
        Else
            panelDettaglio.Style.Item("display") = displayValue
        End If

    End Sub

    Private Sub CaricaDettaglioDosi(dgrDettaglioDosi As DataGrid, codiceLotto As String, descrizioneLotto As String, campoOrdinamento As String, versoOrdinamento As String)

        ' Ordinamento iniziale del datagrid, se non specificato
        If String.IsNullOrEmpty(campoOrdinamento) Then campoOrdinamento = "DescrizioneConsultorio"
        If String.IsNullOrEmpty(versoOrdinamento) Then versoOrdinamento = "ASC"

        Dim listDettaglioDosiLotto As List(Of Entities.LottoDettaglioMagazzino) = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizLotti As New Biz.BizLotti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                ' Per il check di scorta nulla, utilizza lo stato attualmente impostato dall'utente, non lo stato in cui era all'ultima ricerca.
                Dim filtriRicerca As Filters.FiltriRicercaLottiMagazzino = Me.ucFiltriRicerca.GetFiltriCorrenti(False)

                listDettaglioDosiLotto = bizLotti.LoadDettaglioDosiLotto(codiceLotto, filtriRicerca.NoLottiScortaNulla, filtriRicerca.CodiceDistretto,
                                                                         campoOrdinamento, versoOrdinamento,
                                                                         "DescrizioneConsultorio", "ASC",
                                                                         "DosiRimaste", "DESC")
            End Using

        End Using

        Me.BindDatagridDettaglioDosi(dgrDettaglioDosi, listDettaglioDosiLotto)

        Me.AddAttributeToDatagrid(dgrDettaglioDosi, "CodiceLotto", codiceLotto)
        Me.AddAttributeToDatagrid(dgrDettaglioDosi, "DescrizioneLotto", descrizioneLotto)
        Me.AddAttributeToDatagrid(dgrDettaglioDosi, "CampoOrdinamento", campoOrdinamento)
        Me.AddAttributeToDatagrid(dgrDettaglioDosi, "VersoOrdinamento", versoOrdinamento)

    End Sub

    Private Sub BindDatagridDettaglioDosi(dgrDettaglioDosi As DataGrid, listLottoDettaglioMagazzino As List(Of Entities.LottoDettaglioMagazzino))

        AddHandler dgrDettaglioDosi.ItemDataBound, AddressOf dgrDettaglioDosi_ItemDataBound

        dgrDettaglioDosi.DataSource = listLottoDettaglioMagazzino
        dgrDettaglioDosi.DataBind()

    End Sub

    Private Sub AddAttributeToDatagrid(dgr As DataGrid, attributeName As String, attributeValue As String)

        If String.IsNullOrEmpty(dgr.Attributes(attributeName)) Then
            dgr.Attributes.Add(attributeName, attributeValue)
        Else
            dgr.Attributes(attributeName) = attributeValue
        End If

    End Sub

    Private Function GetDescrizioneConsultorioFromDettaglio(dgrDettaglioDosi As DataGrid, rowIndex As Integer)

        Return DirectCast(dgrDettaglioDosi.Items(rowIndex).Cells(DgrDettaglioDosiColumnIndex.DescrizioneConsultorio).FindControl("lblDescrizioneConsultorioDettaglioItem"), Label).Text

    End Function

    Private Sub SortDatagridDettaglio(dgrDettaglioDosi As DataGrid, sortExpression As String)

        ' Lettura dell'ordinamento attuale dall'attributo aggiunto al datagrid corrente
        Dim campoOrdinamento As String = dgrDettaglioDosi.Attributes("CampoOrdinamento")
        Dim versoOrdinamento As String = dgrDettaglioDosi.Attributes("VersoOrdinamento")

        If campoOrdinamento = sortExpression Then
            ' Stesso campo di ordinamento: inverto il verso
            versoOrdinamento = IIf(versoOrdinamento = "ASC", "DESC", "ASC")
        Else
            ' Nuovo ordinamento
            campoOrdinamento = sortExpression
            versoOrdinamento = "ASC"
        End If

        ' Modifica attributi campo e verso nel datagrid corrente
        dgrDettaglioDosi.Attributes("CampoOrdinamento") = campoOrdinamento
        dgrDettaglioDosi.Attributes("VersoOrdinamento") = versoOrdinamento

        ' Lettura di codice e descrizione del lotto corrente
        Dim codiceLotto As String = dgrDettaglioDosi.Attributes("CodiceLotto")
        Dim descrizioneLotto As String = dgrDettaglioDosi.Attributes("DescrizioneLotto")

        ' Ricarico il datagrid ordinato
        Me.CaricaDettaglioDosi(dgrDettaglioDosi, codiceLotto, descrizioneLotto, campoOrdinamento, versoOrdinamento)

    End Sub

#End Region

#Region " Eventi Datagrid Dettaglio Dosi "

    Private Sub dgrDettaglioDosi_ItemCreated(sender As Object, e As DataGridItemEventArgs)

        Select Case e.Item.ItemType

            Case ListItemType.Header

                Dim dgrDettaglioDosi As DataGrid = DirectCast(sender, DataGrid)

                ' Mostra/Nasconde le frecce di ordinamento del datagrid di dettaglio
                Dim campoOrdinamento As String = dgrDettaglioDosi.Attributes("CampoOrdinamento")
                If String.IsNullOrEmpty(campoOrdinamento) Then campoOrdinamento = "DescrizioneConsultorio"

                Dim versoOrdinamento As String = dgrDettaglioDosi.Attributes("VersoOrdinamento")
                If String.IsNullOrEmpty(versoOrdinamento) Then versoOrdinamento = "ASC"

                Dim ordinamentoAsc As Boolean = (versoOrdinamento = "ASC")

                If campoOrdinamento = dgrDettaglioDosi.Columns(DgrDettaglioDosiColumnIndex.DescrizioneConsultorio).SortExpression Then
                    Me.ImpostaFrecceOrdinamentoDettaglio(e.Item, "mag", ordinamentoAsc)
                Else
                    Me.NascondiFrecceOrdinamentoDettaglio(e.Item, "mag")
                End If

                If campoOrdinamento = dgrDettaglioDosi.Columns(DgrDettaglioDosiColumnIndex.DosiRimaste).SortExpression Then
                    Me.ImpostaFrecceOrdinamentoDettaglio(e.Item, "dosi", ordinamentoAsc)
                Else
                    Me.NascondiFrecceOrdinamentoDettaglio(e.Item, "dosi")
                End If

                If campoOrdinamento = dgrDettaglioDosi.Columns(DgrDettaglioDosiColumnIndex.QuantitaMinima).SortExpression Then
                    Me.ImpostaFrecceOrdinamentoDettaglio(e.Item, "qmin", ordinamentoAsc)
                Else
                    Me.NascondiFrecceOrdinamentoDettaglio(e.Item, "qmin")
                End If

                If campoOrdinamento = dgrDettaglioDosi.Columns(DgrDettaglioDosiColumnIndex.Attivo).SortExpression Then
                    Me.ImpostaFrecceOrdinamentoDettaglio(e.Item, "att", ordinamentoAsc)
                Else
                    Me.NascondiFrecceOrdinamentoDettaglio(e.Item, "att")
                End If

        End Select

    End Sub

    Private Sub dgrDettaglioDosi_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs)

        Select Case e.Item.ItemType

            Case ListItemType.Item, ListItemType.AlternatingItem, ListItemType.EditItem, ListItemType.SelectedItem

                ' Entità associata alla riga corrente
                Dim lottoDettaglioMagazzino As Entities.LottoDettaglioMagazzino = Nothing

                Try
                    lottoDettaglioMagazzino = DirectCast(e.Item.DataItem, Entities.LottoDettaglioMagazzino)
                Catch ex As Exception
                    lottoDettaglioMagazzino = Nothing
                End Try

                If lottoDettaglioMagazzino Is Nothing Then Exit Sub

                ' Descrizione Consultorio
                Dim lblDescrizioneConsultorio As Label = DirectCast(e.Item.Cells(DgrDettaglioDosiColumnIndex.DescrizioneConsultorio).FindControl("lblDescrizioneConsultorioDettaglioItem"), Label)
                lblDescrizioneConsultorio.Text = lottoDettaglioMagazzino.DescrizioneConsultorio

                ' Dosi Rimaste
                Dim lblDosiRimaste As Label = DirectCast(e.Item.Cells(DgrDettaglioDosiColumnIndex.DosiRimaste).FindControl("lblDosiRimasteDettaglioItem"), Label)
                lblDosiRimaste.Text = lottoDettaglioMagazzino.DosiRimaste

                ' Quantita minima
                Dim lblQuantitaMinima As Label = DirectCast(e.Item.Cells(DgrDettaglioDosiColumnIndex.QuantitaMinima).FindControl("lblQuantitaMinimaDettaglioItem"), Label)
                lblQuantitaMinima.Text = lottoDettaglioMagazzino.QuantitaMinima

                ' Flag lotto attivo
                Dim lblLottoAttivo As Label = DirectCast(e.Item.Cells(DgrDettaglioDosiColumnIndex.Attivo).FindControl("lblLottoAttivoDettaglioItem"), Label)
                lblLottoAttivo.Text = IIf(lottoDettaglioMagazzino.Attivo, "S", "N")

        End Select

    End Sub

    Private Sub dgrDettaglioDosi_ItemCommand(source As Object, e As DataGridCommandEventArgs)

        ' Recupero l'indice della riga del datagrid lotti
        Dim dgrDettaglioDosi As DataGrid = DirectCast(source, DataGrid)

        Dim codiceLotto As String = dgrDettaglioDosi.Attributes("CodiceLotto")
        Dim descrizioneLotto As String = dgrDettaglioDosi.Attributes("DescrizioneLotto")

        Select Case e.CommandName

            Case "InsertMovimento"

                ' Apertura modale di inserimento del movimento per il lotto corrente e il centro vaccinale relativo alla riga selezionata
                Me.ShowModaleMovimento(True,
                                       e.Item.Cells(DgrDettaglioDosiColumnIndex.CodiceConsultorio).Text,
                                       Me.GetDescrizioneConsultorioFromDettaglio(dgrDettaglioDosi, e.Item.ItemIndex),
                                       codiceLotto,
                                       descrizioneLotto)

            Case "InsertLottoConsultorio"

                ' Apertura modale di inserimento dati del lotto per creazione associazione lotto-consultorio
                Me.ShowModaleLottoConsultorio(True, codiceLotto, descrizioneLotto)

            Case "SortDettaglio"

                Me.SortDatagridDettaglio(dgrDettaglioDosi, e.CommandArgument.ToString())

        End Select

    End Sub

    Private Sub dgrDettaglioDosi_SortCommand(source As Object, e As System.Web.UI.WebControls.DataGridSortCommandEventArgs)

        Dim dgrDettaglioDosi As DataGrid = DirectCast(source, DataGrid)

        If dgrDettaglioDosi.Items.Count > 0 Then

            Me.SortDatagridDettaglio(dgrDettaglioDosi, e.SortExpression)

        End If

    End Sub

#End Region

#Region " Modale Inserimento Movimento Lotto "

    Private Sub ShowModaleMovimento(showModal As Boolean, codiceConsultorio As String, descrizioneConsultorio As String, codiceLotto As String, descrizioneLotto As String)

        Me.ClearCampiMovimento()

        If showModal Then

            ' Valorizzazione label e campi nascosti
            Me.lblMagazzinoMovimento.Text = String.Format("Magazzino: {0} [{1}]", descrizioneConsultorio, codiceConsultorio)
            Me.lblLottoMovimento.Text = String.Format("Lotto: {0} [{1}]", descrizioneLotto, codiceLotto)

            Me.txtCodiceLottoMovimento.Value = codiceLotto
            Me.txtDescrizioneLottoMovimento.Value = descrizioneLotto
            Me.txtCodiceConsultorioMovimento.Value = codiceConsultorio
            Me.txtDescrizioneConsultorioMovimento.Value = descrizioneConsultorio

			Me.fmMagazzinoDestinazioneMovimento.Filtro =
				String.Format("cns_codice <> '{0}' and (cns_cns_magazzino is null or cns_cns_magazzino = cns_codice) and cns_data_chiusura is null and luc_cns_codice = cns_codice and luc_ute_id = {1} order by cns_descrizione",
							  codiceConsultorio.Replace("'", "''"), OnVacContext.UserId)
		Else

            ' Sbiancamento label e campi nascosti
            Me.lblMagazzinoMovimento.Text = String.Empty
            Me.lblLottoMovimento.Text = String.Empty

            Me.txtCodiceLottoMovimento.Value = String.Empty
            Me.txtDescrizioneLottoMovimento.Value = String.Empty
            Me.txtCodiceConsultorioMovimento.Value = String.Empty
            Me.txtDescrizioneConsultorioMovimento.Value = String.Empty

        End If

        ' Apertura/Chiusura modale di inserimento movimento
        Me.modMovimentoLotto.VisibileMD = showModal

    End Sub

    Private Sub btnSalvaMovimento_Click(sender As Object, e As System.EventArgs) Handles btnSalvaMovimento.Click
        
        Dim inserisciMovimentoResult As Biz.BizLotti.BizLottiResult = Me.InserisciMovimentoLotto()

        If inserisciMovimentoResult.Result = Biz.BizLotti.BizLottiResult.ResultType.GenericError Then

            Me.OnitLayout31.InsertRoutineJS(String.Format("alert(""{0}"");", inserisciMovimentoResult.Message))

        Else

            If inserisciMovimentoResult.Result = Biz.BizLotti.BizLottiResult.ResultType.LottoDisattivatoScortaNullaWarning Then
                Me.OnitLayout31.InsertRoutineJS(String.Format("alert(""{0}"");", inserisciMovimentoResult.Message))
            End If

            Me.ShowModaleMovimento(False, Nothing, Nothing, Nothing, Nothing)

            Me.RicercaLotti(Me.dgrLotti.CurrentPageIndex, False, True)

        End If

    End Sub

    Private Sub btnChiudiModaleMovimento_Click(sender As Object, e As System.EventArgs) Handles btnChiudiModaleMovimento.Click

        Me.ShowModaleMovimento(False, Nothing, Nothing, Nothing, Nothing)

        Me.RicercaLotti(Me.dgrLotti.CurrentPageIndex, False, True)

    End Sub

    Private Sub ddlTipoMovimento_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlTipoMovimento.SelectedIndexChanged

        Me.fmMagazzinoDestinazioneMovimento.Codice = String.Empty
        Me.fmMagazzinoDestinazioneMovimento.Descrizione = String.Empty
        Me.fmMagazzinoDestinazioneMovimento.RefreshDataBind()

        If Me.ddlTipoMovimento.SelectedValue = Constants.TipoMovimentoMagazzino.TrasferimentoA Then

            Me.fmMagazzinoDestinazioneMovimento.Enabled = True

        Else

            Me.fmMagazzinoDestinazioneMovimento.Enabled = False

        End If

    End Sub

    Private Function InserisciMovimentoLotto() As Biz.BizLotti.BizLottiResult

        Dim inserisciMovimentoResult As Biz.BizLotti.BizLottiResult

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizLotti As New Biz.BizLotti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                inserisciMovimentoResult = bizLotti.InserisciMovimentoMagazzinoCentrale(Me.txtCodiceLottoMovimento.Value,
                                                                                        Me.lblQtaMovimento.Text,
                                                                                        Me.txtQtaMovimento.Text,
                                                                                        Me.ddlTipoMovimento.SelectedValue,
                                                                                        Me.txtCodiceConsultorioMovimento.Value,
                                                                                        Me.fmMagazzinoDestinazioneMovimento.Codice,
                                                                                        Me.fmMagazzinoDestinazioneMovimento.Descrizione,
                                                                                        Me.txtNoteMovimento.Text)
            End Using

        End Using

        Return inserisciMovimentoResult

    End Function

    Private Sub CaricaQuantitaLotto(datagridCurrentItem As DataGridItem)

        Dim codiceLotto As String = datagridCurrentItem.Cells(DgrLottiColumnIndex.CodiceLotto).Text
        Dim filtriRicerca As Filters.FiltriRicercaLottiMagazzino = Nothing

        filtriRicerca = Me.ucFiltriRicerca.GetFiltriCorrenti(True)

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizLotti As New Biz.BizLotti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                Dim lottoMagazzino As Entities.LottoMagazzino = bizLotti.LoadLottoMagazzinoCentrale(codiceLotto, OnVacContext.UserId, filtriRicerca.CodiceDistretto)

                If Not lottoMagazzino Is Nothing Then

                    datagridCurrentItem.Cells(DgrLottiColumnIndex.DosiRimaste).Text = lottoMagazzino.DosiRimaste

                End If

            End Using

        End Using

    End Sub

    Private Sub ClearCampiMovimento()

        Me.txtQtaMovimento.Text = String.Empty

        Me.ddlTipoMovimento.ClearSelection()

        Me.fmMagazzinoDestinazioneMovimento.Codice = String.Empty
        Me.fmMagazzinoDestinazioneMovimento.Descrizione = String.Empty
        Me.fmMagazzinoDestinazioneMovimento.RefreshDataBind()
        Me.fmMagazzinoDestinazioneMovimento.Enabled = False

        Me.txtNoteMovimento.Text = String.Empty

    End Sub

#End Region

#Region " Modale Inserimento Lotto-Consultorio "

    Private Sub ShowModaleLottoConsultorio(showModal As Boolean, codiceLotto As String, descrizioneLotto As String)

        Me.ClearCampiLottoConsultorio()

        If showModal Then

            ' Valorizzazione label e campi nascosti
            Me.lblLottoConsultorio.Text = String.Format("Lotto: {0} [{1}]", descrizioneLotto, codiceLotto)

            Me.txtCodiceLottoConsultorio.Value = codiceLotto
            Me.txtDescrizioneLottoConsultorio.Value = descrizioneLotto

            Me.fmMagazzinoLottoConsultorio.Filtro =
                String.Format("(cns_cns_magazzino is null or cns_cns_magazzino = cns_codice) and cns_data_chiusura is null and not exists (select cns_codice from t_ana_lotti left join t_lot_lotti_consultori on lot_codice = lcn_lot_codice where lot_codice = '{0}' and lcn_cns_codice = cns_codice) order by cns_descrizione",
                              codiceLotto.Replace("'", "''"))
        Else

            ' Sbiancamento label e campi nascosti
            Me.lblLottoConsultorio.Text = String.Empty

            Me.txtCodiceLottoConsultorio.Value = String.Empty
            Me.txtDescrizioneLottoConsultorio.Value = String.Empty

        End If

        Me.modInserimentoLottoConsultorio.VisibileMD = showModal

    End Sub

    Private Sub btnSalvaLottoConsultorio_Click(sender As Object, e As System.EventArgs) Handles btnSalvaLottoConsultorio.Click

        ' Salvataggio
        Dim bizLottiResult As Biz.BizLotti.BizLottiResult = Me.SalvaLottoConsultorio()

        If bizLottiResult.Result = Biz.BizLotti.BizLottiResult.ResultType.GenericError Then

            ' Se il salvataggio non è andato a buon fine, visualizzo il messaggio di errore
            Me.OnitLayout31.InsertRoutineJS(String.Format("alert(""{0}"");", bizLottiResult.Message))

        Else

            ' Se il salvataggio è andato a buon fine, chiudo la modale e ricarico il datagrid
            Me.ShowModaleLottoConsultorio(False, Nothing, Nothing)

            Me.RicercaLotti(Me.dgrLotti.CurrentPageIndex, False, True)

        End If

    End Sub

    Private Sub btnAnnullaLottoConsultorio_Click(sender As Object, e As System.EventArgs) Handles btnAnnullaLottoConsultorio.Click

        Me.ShowModaleLottoConsultorio(False, Nothing, Nothing)

        Me.RicercaLotti(Me.dgrLotti.CurrentPageIndex, False, True)

    End Sub

    Private Function SalvaLottoConsultorio() As Biz.BizLotti.BizLottiResult

        Dim salvaLottoConsultorioResult As Biz.BizLotti.BizLottiResult

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizLotti As New Biz.BizLotti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                salvaLottoConsultorioResult =
                    bizLotti.InsertLottoConsultorioMagazzinoCentrale(Me.txtCodiceLottoConsultorio.Value,
                                                                     Me.txtDescrizioneLottoConsultorio.Value,
                                                                     Me.lblMagazzinoLottoConsultorio.Text,
                                                                     Me.fmMagazzinoLottoConsultorio.Codice,
                                                                     Me.lblDosiLottoConsultorio.Text,
                                                                     Me.txtDosiLottoConsultorio.Text,
                                                                     Me.lblQtaMinimaLottoConsultorio.Text,
                                                                     Me.txtQtaMinimaLottoConsultorio.Text)
            End Using

        End Using

        Return salvaLottoConsultorioResult

    End Function

    Private Sub ClearCampiLottoConsultorio()

        Me.fmMagazzinoLottoConsultorio.Codice = String.Empty
        Me.fmMagazzinoLottoConsultorio.Descrizione = String.Empty
        Me.fmMagazzinoLottoConsultorio.RefreshDataBind()

        Me.txtQtaMinimaLottoConsultorio.Text = String.Empty
        Me.txtDosiLottoConsultorio.Text = String.Empty

    End Sub

#End Region

#Region " Private "

    Private Sub RedirectToDettaglio(codiceLottoSelezionato As String, currentPageIndex As Integer)

        Server.Transfer(MagazzinoUtility.GetRedirectUrl(".\DettaglioLottoMagazzinoCentrale.aspx",
                                                        Me.ucFiltriRicerca.GetUltimiFiltriUtilizzati(),
                                                        Me.GetCampiOrdinamentoForQueryString(),
                                                        Me.GetVersiOrdinamentoForQueryString(),
                                                        Me.dgrLotti.Items.Count > 0,
                                                        codiceLottoSelezionato,
                                                        currentPageIndex), False)
    End Sub

    Private Function GetCampiOrdinamentoForQueryString() As String

        Return String.Join(",", New String() {Me.Campo1OrdinamentoDatagrid, Me.Campo2OrdinamentoDatagrid, Me.Campo3OrdinamentoDatagrid})

    End Function

    Private Function GetVersiOrdinamentoForQueryString() As String

        Return String.Join(",", New String() {Me.Verso1OrdinamentoDatagrid, Me.Verso2OrdinamentoDatagrid, Me.Verso3OrdinamentoDatagrid})

    End Function

    Private Sub ClearCurrentDetails()

        Me.CurrentDetailsList.Clear()

    End Sub

    Private Function GetListaCampiOrdinamento() As List(Of Entities.DatiOrdinamento)

        Dim listDatiOrdinamento As New List(Of Entities.DatiOrdinamento)()

        listDatiOrdinamento.Add(Biz.BizLotti.CreateDatiOrdinamento(Me.Campo1OrdinamentoDatagrid, Me.Verso1OrdinamentoDatagrid, Nothing))
        listDatiOrdinamento.Add(Biz.BizLotti.CreateDatiOrdinamento(Me.Campo2OrdinamentoDatagrid, Me.Verso2OrdinamentoDatagrid, Nothing))
        listDatiOrdinamento.Add(Biz.BizLotti.CreateDatiOrdinamento(Me.Campo3OrdinamentoDatagrid, Me.Verso3OrdinamentoDatagrid, Nothing))

        Return listDatiOrdinamento

    End Function

#End Region

#Region " Stampe "

    ''' <summary>
    ''' Stampa l'elenco dei lotti visualizzato
    ''' </summary>
    Private Sub StampaMagazzinoLotti()

        ' Caricamento lotti da stampare
        Dim listLottiMagazzino As List(Of Entities.LottoMagazzino) = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizLotti As New Biz.BizLotti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                listLottiMagazzino = Me.LoadLottiMagazzinoCentrale(bizLotti, False, Me.ucFiltriRicerca.GetUltimiFiltriUtilizzati(), 0, 0)

            End Using

        End Using

        ' Campi di ordinamento
        Dim listDatiOrdinamento As List(Of Entities.DatiOrdinamento) = Me.GetListaCampiOrdinamento()

        ' Stampa
        MagazzinoUtility.StampaMagazzinoLotti(listLottiMagazzino, listDatiOrdinamento, Me.Page, True, True)

    End Sub

#End Region

End Class