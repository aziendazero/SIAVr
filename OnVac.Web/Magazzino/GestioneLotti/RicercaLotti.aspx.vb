Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager


Partial Public Class RicercaLotti
    Inherits OnVac.Common.PageBase

#Region " Enums "

    Private Enum DgrLottiColumnIndex
        ButtonStampa = 0
        ButtonAttiva = 1
        ButtonModifica = 2
        CodiceLotto = 3
        DescrizioneLotto = 4
        DescrizioneNomeCommerciale = 5
        DataPreparazioneString = 6
        DataScadenzaString = 7
        DosiRimaste = 8
        LabelScortaNulla = 9
        LabelScortaInsufficiente = 10
        LabelLottoInConsultorio = 11
        LabelLottoAttivo = 12
        DataPreparazioneDate = 13           ' nascosta
        DataScadenzaDate = 14               ' nascosta
        CodiceConsultorio = 15              ' nascosta
        Attivo = 16                         ' nascosta
        CodiceNomeCommerciale = 17          ' nascosta
        QuantitaMinima = 18                 ' nascosta
        Obsoleto = 19                       ' nascosta
    End Enum

#End Region

#Region " Types "

    Private Class CheckResult

        Public Success As Boolean
        Public Message As String

        Public Sub New(success As Boolean, message As String)
            Me.Success = success
            Me.Message = message
        End Sub

    End Class

#End Region

#Region " Properties "

    Private Property Campo1OrdinamentoDatagrid() As String
        Get
            If ViewState("CampoOrdinamento1_DatagridLotti") Is Nothing Then ViewState("CampoOrdinamento1_DatagridLotti") = String.Empty
            Return ViewState("CampoOrdinamento1_DatagridLotti")
        End Get
        Set(value As String)
            ViewState("CampoOrdinamento1_DatagridLotti") = value
        End Set
    End Property

    Private Property Campo2OrdinamentoDatagrid() As String
        Get
            If ViewState("CampoOrdinamento2_DatagridLotti") Is Nothing Then ViewState("CampoOrdinamento2_DatagridLotti") = String.Empty
            Return ViewState("CampoOrdinamento2_DatagridLotti")
        End Get
        Set(value As String)
            ViewState("CampoOrdinamento2_DatagridLotti") = value
        End Set
    End Property

    Private Property Campo3OrdinamentoDatagrid() As String
        Get
            If ViewState("CampoOrdinamento3_DatagridLotti") Is Nothing Then ViewState("CampoOrdinamento3_DatagridLotti") = String.Empty
            Return ViewState("CampoOrdinamento3_DatagridLotti")
        End Get
        Set(value As String)
            ViewState("CampoOrdinamento3_DatagridLotti") = value
        End Set
    End Property

    Private Property Verso1OrdinamentoDatagrid() As String
        Get
            If ViewState("VersoOrdinamento1_DatagridLotti") Is Nothing Then ViewState("VersoOrdinamento1_DatagridLotti") = "ASC"
            Return ViewState("VersoOrdinamento1_DatagridLotti")
        End Get
        Set(value As String)
            ViewState("VersoOrdinamento1_DatagridLotti") = value
        End Set
    End Property

    Private Property Verso2OrdinamentoDatagrid() As String
        Get
            If ViewState("VersoOrdinamento2_DatagridLotti") Is Nothing Then ViewState("VersoOrdinamento2_DatagridLotti") = "ASC"
            Return ViewState("VersoOrdinamento2_DatagridLotti")
        End Get
        Set(value As String)
            ViewState("VersoOrdinamento2_DatagridLotti") = value
        End Set
    End Property

    Private Property Verso3OrdinamentoDatagrid() As String
        Get
            If ViewState("VersoOrdinamento3_DatagridLotti") Is Nothing Then ViewState("VersoOrdinamento3_DatagridLotti") = "ASC"
            Return ViewState("VersoOrdinamento3_DatagridLotti")
        End Get
        Set(value As String)
            ViewState("VersoOrdinamento3_DatagridLotti") = value
        End Set
    End Property

#End Region

#Region " Page Events "

    Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        MagazzinoUtility.SetToolbarButtonImages("btnCerca", "cerca.gif", Me.ToolBar)
        MagazzinoUtility.SetToolbarButtonImages("btnStampa", "stampa.gif", Me.ToolBar)

    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then

            ' Titolo pagina
            Me.LayoutTitolo.Text = OnVacUtility.Variabili.CNSMagazzino.Descrizione
            OnVacUtility.ImpostaCnsLavoro(OnitLayout31)

            ' Lettura filtri di ricerca da querystring
            Dim filtroLottiMagazzino As Filters.FiltriRicercaLottiMagazzino = MagazzinoUtility.GetFiltersFromQueryString(Request.QueryString)

            If Not filtroLottiMagazzino Is Nothing Then
                ' Se nella querystring ci sono i filtri, lo imposto nello user control
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
                Me.Cerca()

            Else

                ' Visualizzazione datagrid vuoto
                Me.BindDatagridLotti(Nothing)

            End If

            ' Pulsante di stampa visibile solo se la stampa è abilitata
            If Not MagazzinoUtility.ExistsReport(Constants.ReportName.MagazzinoLotti) Then

                Me.ToolBar.Items.FromKeyButton("btnStampa").Visible = False

            End If

        End If

    End Sub

    Private Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender

        Me.RegisterScriptHeaderVisibility()

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

#End Region

#Region " Layout "

    Private Sub BindDatagridLotti(listLottiMagazzino As List(Of Entities.LottoMagazzino))

        If listLottiMagazzino Is Nothing Then listLottiMagazzino = New List(Of Entities.LottoMagazzino)()

        Me.dgrLotti.DataSource = listLottiMagazzino
        Me.dgrLotti.DataBind()

        ' Se è abilitata la gestione dei lotti con i codici a barre, non deve essere visualizzato 
        ' il dato sull'attivazione del lotto, nè il pulsante di attivazione/disattivazione.
        If Me.Settings.GESBALOT Then

            Me.lblAttivo.Visible = False

            Me.dgrLotti.Columns.Item(DgrLottiColumnIndex.LabelLottoAttivo).Visible = False
            Me.dgrLotti.Columns.Item(DgrLottiColumnIndex.ButtonAttiva).Visible = False

        End If

    End Sub

    Private Sub RegisterScriptHeaderVisibility()

        Dim scriptVisibility As New System.Text.StringBuilder()

        scriptVisibility.AppendLine("<script type='text/javascript'>")

        If Me.Campo1OrdinamentoDatagrid = dgrLotti.Columns(DgrLottiColumnIndex.CodiceLotto).SortExpression Then
            Me.CreaScriptVisibility("cl", (Me.Verso1OrdinamentoDatagrid = "ASC"), scriptVisibility)
        Else
            Me.CreaScriptVisibility("cl", scriptVisibility)
        End If

        If Me.Campo1OrdinamentoDatagrid = dgrLotti.Columns(DgrLottiColumnIndex.DescrizioneLotto).SortExpression Then
            Me.CreaScriptVisibility("dl", (Me.Verso1OrdinamentoDatagrid = "ASC"), scriptVisibility)
        Else
            Me.CreaScriptVisibility("dl", scriptVisibility)
        End If

        If Me.Campo1OrdinamentoDatagrid = dgrLotti.Columns(DgrLottiColumnIndex.DescrizioneNomeCommerciale).SortExpression Then
            Me.CreaScriptVisibility("nc", (Me.Verso1OrdinamentoDatagrid = "ASC"), scriptVisibility)
        Else
            Me.CreaScriptVisibility("nc", scriptVisibility)
        End If

        If Me.Campo1OrdinamentoDatagrid = dgrLotti.Columns(DgrLottiColumnIndex.DataPreparazioneString).SortExpression Then
            Me.CreaScriptVisibility("dp", (Me.Verso1OrdinamentoDatagrid = "ASC"), scriptVisibility)
        Else
            Me.CreaScriptVisibility("dp", scriptVisibility)
        End If

        If Me.Campo1OrdinamentoDatagrid = dgrLotti.Columns(DgrLottiColumnIndex.DataScadenzaString).SortExpression Then
            Me.CreaScriptVisibility("ds", (Me.Verso1OrdinamentoDatagrid = "ASC"), scriptVisibility)
        Else
            Me.CreaScriptVisibility("ds", scriptVisibility)
        End If

        If Me.Campo1OrdinamentoDatagrid = dgrLotti.Columns(DgrLottiColumnIndex.DosiRimaste).SortExpression Then
            Me.CreaScriptVisibility("dr", (Me.Verso1OrdinamentoDatagrid = "ASC"), scriptVisibility)
        Else
            Me.CreaScriptVisibility("dr", scriptVisibility)
        End If

        If Me.Campo1OrdinamentoDatagrid = dgrLotti.Columns(DgrLottiColumnIndex.LabelLottoInConsultorio).SortExpression Then
            Me.CreaScriptVisibility("cn", (Me.Verso1OrdinamentoDatagrid = "ASC"), scriptVisibility)
        Else
            Me.CreaScriptVisibility("cn", scriptVisibility)
        End If

        If Me.Campo1OrdinamentoDatagrid = dgrLotti.Columns(DgrLottiColumnIndex.LabelLottoAttivo).SortExpression Then
            Me.CreaScriptVisibility("at", (Me.Verso1OrdinamentoDatagrid = "ASC"), scriptVisibility)
        Else
            Me.CreaScriptVisibility("at", scriptVisibility)
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

#Region " Toolbar Events "

    Private Sub ToolBar_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case (be.Button.Key)

            Case "btnCerca"

                Me.Cerca()

            Case "btnStampa"

                Me.StampaMagazzinoLotti()

            Case "btnInserisci"

                Me.RedirectToDettaglio(String.Empty)

            Case "btnPulisci"

                Me.Pulisci()

        End Select

    End Sub

    Private Sub Pulisci()

        ' Cancello i filtri, imposto solo il filtro sui lotti scaduti 
        Me.ucFiltriRicerca.Clear()
        Me.ucFiltriRicerca.SetFiltroLottiScaduti(True)

        ' Visualizzazione datagrid vuoto
        Me.BindDatagridLotti(Nothing)

    End Sub

#End Region

#Region " Ricerca "

    ' Impostazione campi di default per l'ordinamento del datagrid, in base ai parametri relativi al magazzino
    Private Sub SetCampiOrdinamentoDefault()

        ' L'ordinamento di default dovrebbe essere: 
        ' se il parametro GESBALOT è true   -> consultorio, "", dosi 
        ' se il parametro GESBALOT è false  -> consultorio, attivo, dosi 
        ' Consultorio e lotto attivo vengono inverti dal biz quando glieli passo
        If Me.Settings.GESBALOT Then

            Me.Campo1OrdinamentoDatagrid = Me.dgrLotti.Columns(DgrLottiColumnIndex.LabelLottoInConsultorio).SortExpression
            Me.Campo2OrdinamentoDatagrid = Me.dgrLotti.Columns(DgrLottiColumnIndex.DosiRimaste).SortExpression
            Me.Campo3OrdinamentoDatagrid = String.Empty

            Me.Verso1OrdinamentoDatagrid = "ASC"
            Me.Verso2OrdinamentoDatagrid = "DESC"
            Me.Verso3OrdinamentoDatagrid = "ASC"

        Else

            Me.Campo1OrdinamentoDatagrid = Me.dgrLotti.Columns(DgrLottiColumnIndex.LabelLottoInConsultorio).SortExpression
            Me.Campo2OrdinamentoDatagrid = Me.dgrLotti.Columns(DgrLottiColumnIndex.LabelLottoAttivo).SortExpression
            Me.Campo3OrdinamentoDatagrid = Me.dgrLotti.Columns(DgrLottiColumnIndex.DosiRimaste).SortExpression

            Me.Verso1OrdinamentoDatagrid = "ASC"
            Me.Verso2OrdinamentoDatagrid = "ASC"
            Me.Verso3OrdinamentoDatagrid = "DESC"

        End If

    End Sub

    ' Impostazione campi specificati per l'ordinamento del datagrid.
    Private Sub SetCampiOrdinamentoDatagrid(sortExpression As String)

        ' Impostazione Colonna e verso dell'ordinamento
        If Me.Campo1OrdinamentoDatagrid = sortExpression Then
            If Me.Verso1OrdinamentoDatagrid = "ASC" Then
                Me.Verso1OrdinamentoDatagrid = "DESC"
            Else
                Me.Verso1OrdinamentoDatagrid = "ASC"
            End If
        Else
            Me.Verso1OrdinamentoDatagrid = "ASC"
        End If

        Me.Campo1OrdinamentoDatagrid = sortExpression

        ' Impostazione del secondo campo di ordinamento (o codice lotto o nome commerciale, in base al primo campo)
        Me.Campo2OrdinamentoDatagrid = dgrLotti.Columns(DgrLottiColumnIndex.CodiceLotto).SortExpression

        If Me.Campo1OrdinamentoDatagrid = dgrLotti.Columns(DgrLottiColumnIndex.LabelLottoAttivo).SortExpression Or
           Me.Campo1OrdinamentoDatagrid = dgrLotti.Columns(DgrLottiColumnIndex.CodiceConsultorio).SortExpression Then

            Me.Campo2OrdinamentoDatagrid = dgrLotti.Columns(DgrLottiColumnIndex.DescrizioneNomeCommerciale).SortExpression

        End If

        ' Impostazione del terzo campo di ordinamento (fisso a codice lotto)
        Me.Campo3OrdinamentoDatagrid = dgrLotti.Columns(DgrLottiColumnIndex.CodiceLotto).SortExpression

        ' Verso2 e verso3 rimangono sempre asc (come vecchia versione)
        Me.Verso2OrdinamentoDatagrid = "ASC"
        Me.Verso3OrdinamentoDatagrid = "ASC"

    End Sub

    Private Sub Cerca()

        Dim orderedList As List(Of Entities.LottoMagazzino) = Nothing

        Dim listLottiMagazzino As List(Of Entities.LottoMagazzino) = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizLotti As New Biz.BizLotti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                listLottiMagazzino = bizLotti.LoadLottiMagazzino(OnVacUtility.Variabili.CNS.Codice,
                                                                 OnVacUtility.Variabili.CNSMagazzino.Codice,
                                                                 Me.ucFiltriRicerca.GetFiltriCorrenti(True))

                If String.IsNullOrEmpty(Me.Campo1OrdinamentoDatagrid) Then

                    ' Ordinamento di default
                    Me.SetCampiOrdinamentoDefault()

                End If

                ' Ordinamento in base alla colonna indicata. Se è la colonna Attivo o Consultorio, l'ordinamento va invertito.
                orderedList = bizLotti.OrdinaListaLottiMagazzino(listLottiMagazzino,
                                                                 Me.Campo1OrdinamentoDatagrid,
                                                                 Me.Verso1OrdinamentoDatagrid,
                                                                 Me.Campo2OrdinamentoDatagrid,
                                                                 Me.Verso2OrdinamentoDatagrid,
                                                                 Me.Campo3OrdinamentoDatagrid,
                                                                 Me.Verso3OrdinamentoDatagrid,
                                                                 New String() {Me.dgrLotti.Columns(DgrLottiColumnIndex.LabelLottoInConsultorio).SortExpression,
                                                                               Me.dgrLotti.Columns(DgrLottiColumnIndex.LabelLottoAttivo).SortExpression})

            End Using

        End Using

        Me.BindDatagridLotti(orderedList)

    End Sub

#End Region

#Region " Eventi Datagrid Lotti "

    Private Sub dgrLotti_ItemCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgrLotti.ItemCommand

        Select Case e.CommandName

            Case "AttDisLotto"

                Me.ToggleAttivazioneLotto(e.Item)

            Case "Stampa"

                Me.StampaLottoSelezionato(e.Item)

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

                ' Lotto attivo
                Me.SetDatagridLottiLabelVisibility(e.Item, "lb_att", lottoMagazzino.Attivo)

                ' Lotto in consultorio
                Me.SetDatagridLottiLabelVisibility(e.Item, "lb_cns", (lottoMagazzino.CodiceConsultorio = OnVacUtility.Variabili.CNSMagazzino.Codice))

        End Select

    End Sub

    Private Sub dgrLotti_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles dgrLotti.SelectedIndexChanged

        Me.RedirectToDettaglio(Me.dgrLotti.SelectedItem.Cells(DgrLottiColumnIndex.CodiceLotto).Text)

    End Sub

    Private Sub dgrLotti_SortCommand(source As Object, e As System.Web.UI.WebControls.DataGridSortCommandEventArgs) Handles dgrLotti.SortCommand

        Me.SetCampiOrdinamentoDatagrid(e.SortExpression)

        Me.Cerca()

    End Sub

    Private Sub SetDatagridLottiLabelVisibility(datagridItem As DataGridItem, labelName As String, visible As Boolean)

        DirectCast(datagridItem.FindControl(labelName), System.Web.UI.WebControls.Label).Visible = visible

    End Sub

#End Region

#Region " Stampe "

    ''' <summary>
    ''' Stampa l'elenco dei lotti visualizzato
    ''' </summary>
    Private Sub StampaMagazzinoLotti()

        ' Lotti da stampare
        Dim listLottiMagazzino As New List(Of Entities.LottoMagazzino)()
        Dim lottoMagazzino As Entities.LottoMagazzino

        For i As Integer = 0 To Me.dgrLotti.Items.Count - 1

            lottoMagazzino = New Entities.LottoMagazzino()

            lottoMagazzino.CodiceLotto = Me.dgrLotti.Items(i).Cells(DgrLottiColumnIndex.CodiceLotto).Text
            lottoMagazzino.DescrizioneLotto = Me.dgrLotti.Items(i).Cells(DgrLottiColumnIndex.DescrizioneLotto).Text
            lottoMagazzino.DescrizioneNomeCommerciale = Me.dgrLotti.Items(i).Cells(DgrLottiColumnIndex.DescrizioneNomeCommerciale).Text

            Dim lblDataPreparazione As Label = DirectCast(Me.dgrLotti.Items(i).Cells(DgrLottiColumnIndex.DataPreparazioneString).FindControl("lblDataPreparazione"), Label)

            If String.IsNullOrEmpty(lblDataPreparazione.Text) Then
                lottoMagazzino.DataPreparazione = Date.MinValue
            Else
                Try
                    lottoMagazzino.DataPreparazione = Convert.ToDateTime(lblDataPreparazione.Text)
                Catch ex As Exception
                    lottoMagazzino.DataPreparazione = Date.MinValue
                End Try
            End If

            lottoMagazzino.DataScadenza = Me.dgrLotti.Items(i).Cells(DgrLottiColumnIndex.DataScadenzaDate).Text
            lottoMagazzino.DosiRimaste = MagazzinoUtility.GetIntegerFromDataGridItem(Me.dgrLotti.Items(i), DgrLottiColumnIndex.DosiRimaste)
            lottoMagazzino.QuantitaMinima = MagazzinoUtility.GetIntegerFromDataGridItem(Me.dgrLotti.Items(i), DgrLottiColumnIndex.QuantitaMinima)
            lottoMagazzino.CodiceConsultorio = Me.dgrLotti.Items(i).Cells(DgrLottiColumnIndex.CodiceConsultorio).Text

            Dim attivo As Boolean? = MagazzinoUtility.GetBooleanFromDataGridItem(Me.dgrLotti.Items(i), DgrLottiColumnIndex.Attivo)
            If attivo Is Nothing Then attivo = False

            lottoMagazzino.Attivo = attivo

            Dim obsoleto As Boolean? = MagazzinoUtility.GetBooleanFromDataGridItem(Me.dgrLotti.Items(i), DgrLottiColumnIndex.Obsoleto)
            If obsoleto Is Nothing Then obsoleto = False

            lottoMagazzino.Obsoleto = obsoleto

            listLottiMagazzino.Add(lottoMagazzino)

        Next

        ' Campi di ordinamento
        Dim listDatiOrdinamento As List(Of Entities.DatiOrdinamento) = Me.GetListaCampiOrdinamento()

        ' Stampa
        MagazzinoUtility.StampaMagazzinoLotti(listLottiMagazzino, listDatiOrdinamento, Me.Page, Me.Settings.GESBALOT, False)

    End Sub

    ''' <summary>
    ''' Stampa il lotto selezionato
    ''' </summary>
    Private Sub StampaLottoSelezionato(datagridItem As DataGridItem)

        Dim codiceLotto As String = datagridItem.Cells(DgrLottiColumnIndex.CodiceLotto).Text
        Dim descrizioneLotto As String = datagridItem.Cells(DgrLottiColumnIndex.DescrizioneLotto).Text

        Dim rpt As New ReportParameter()
        rpt.AddParameter("DataEffettuazione1", String.Empty)
        rpt.AddParameter("DataEffettuazione2", String.Empty)
        rpt.AddParameter("Lotto", descrizioneLotto)

        Dim stbFiltro As New System.Text.StringBuilder()
        stbFiltro.AppendFormat("({{T_ANA_LOTTI.LOT_CODICE}} = '{0}')", codiceLotto)

        ' Stampa (con history.back)
        If Not OnVacReport.StampaReport(Constants.ReportName.UtilizzoLotto, stbFiltro.ToString(), rpt, , , MagazzinoUtility.GetCartellaReport(Constants.ReportName.UtilizzoLotto)) Then
            OnVacUtility.StampaNonPresente(Page, Constants.ReportName.UtilizzoLotto)
        End If

    End Sub

#End Region

#Region " Attivazione/disattivazione lotto "

    Private Sub ToggleAttivazioneLotto(datagridItem As DataGridItem)

        Dim codiceLotto As String = HttpUtility.HtmlDecode(datagridItem.Cells(DgrLottiColumnIndex.CodiceLotto).Text).Trim()
        Dim descrizioneLotto As String = HttpUtility.HtmlDecode(datagridItem.Cells(DgrLottiColumnIndex.DescrizioneLotto).Text).Trim()
        Dim codiceConsultorioLotto As String = HttpUtility.HtmlDecode(datagridItem.Cells(DgrLottiColumnIndex.CodiceConsultorio).Text).Trim()

        Dim attivo As Boolean? = MagazzinoUtility.GetBooleanFromDataGridItem(datagridItem, DgrLottiColumnIndex.Attivo)

        If Not attivo Is Nothing AndAlso Not attivo.Value Then

            ' Lotto da attivare: controlla se è attivabile nel consultorio corrente.
            Dim codiceNomeCommerciale As String = HttpUtility.HtmlDecode(datagridItem.Cells(DgrLottiColumnIndex.CodiceNomeCommerciale).Text).Trim()

            Dim result As CheckResult = Me.CheckLottoAttivabile(codiceLotto, codiceNomeCommerciale, codiceConsultorioLotto)

            If Not result.Success Then

                ' Messaggio all'utente 
                Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Impossibile eseguire l'attivazione del lotto:\n" + result.Message, String.Empty, False, False))
                Return

            End If

        End If

        Me.OnitLayout31.Busy = True

        ' Conferma attivazione/disattivazione
        Me.OpenModalAttivazioneLotto(codiceLotto, descrizioneLotto, codiceConsultorioLotto, Not attivo.Value)

    End Sub

    ' Controlla se il lotto si può attivare nel consultorio specificato e restituisce un messaggio in caso negativo.
    ' Restituisce il motivo per cui il lotto non è attivabile. Se è attivabile restituisce stringa vuota
    Private Function CheckLottoAttivabile(codiceLotto As String, codiceNomeCommerciale As String, codiceConsultorioLotto As String) As CheckResult

        Dim success As Boolean = True
        Dim msgErroreAttivazione As String = String.Empty

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizLotti As New Biz.BizLotti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                Dim bizLottiResult As Biz.BizLotti.BizLottiResult =
                    bizLotti.IsLottoAttivabile(False, codiceLotto, codiceNomeCommerciale, codiceConsultorioLotto,
                                               OnVacUtility.Variabili.CNS.Codice, OnVacUtility.Variabili.CNSMagazzino.Codice)

                If bizLottiResult.Result <> Biz.BizLotti.BizLottiResult.ResultType.Success Then

                    success = False
                    msgErroreAttivazione = bizLottiResult.Message

                End If

            End Using

        End Using

        Return New CheckResult(success, msgErroreAttivazione)

    End Function

#Region " Finestra modale attivazione lotto "

    Private Sub OpenModalAttivazioneLotto(codiceLotto As String, descrizioneLotto As String, codiceConsultorioLotto As String, attiva As Boolean)
        '--
        Me.txtCodiceLottoDaAttivare.Text = codiceLotto
        Me.txtFlagAttivazioneLotto.Text = IIf(attiva, "S", "N")
        '--
        Me.ucEtaMinAttivazione.Clear()
        Me.ucEtaMaxAttivazione.Clear()
        '--
        If attiva Then
            '--
            Me.modAttivazioneLotto.Title = "&nbsp;Attivazione lotto"
            Me.lblWarningOperazione.Text = "Il lotto verrà ATTIVATO."
            '--
            Me.ucEtaMinAttivazione.Visible = Me.Settings.ASSOCIA_LOTTI_ETA
            Me.ucEtaMaxAttivazione.Visible = Me.Settings.ASSOCIA_LOTTI_ETA
            '--
            ' Recupero età minima e massima di attivazione, se gestite
            Dim totGiorniMinAttivazione As Integer? = Nothing
            Dim totGiorniMaxAttivazione As Integer? = Nothing
            '--
            If Me.Settings.ASSOCIA_LOTTI_ETA Then
                '--
                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                    Using bizLotti As New Biz.BizLotti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())
                        '--
                        Dim etaAttivazione As Biz.BizLotti.EtaAttivazioneLottoResult = bizLotti.GetEtaAttivazioneLotto(codiceLotto, codiceConsultorioLotto)
                        '--
                        If Not etaAttivazione Is Nothing Then
                            totGiorniMinAttivazione = etaAttivazione.EtaMinima
                            totGiorniMaxAttivazione = etaAttivazione.EtaMassima
                        End If
                        '--
                    End Using
                End Using
                '--
            End If
            '--
            Me.ucEtaMinAttivazione.SetGiorni(totGiorniMinAttivazione)
            Me.ucEtaMaxAttivazione.SetGiorni(totGiorniMaxAttivazione)
            '--
        Else
            '--
            Me.modAttivazioneLotto.Title = "&nbsp;Disattivazione lotto"
            Me.lblWarningOperazione.Text = "Il lotto verrà DISATTIVATO."
            '--
            Me.ucEtaMinAttivazione.Visible = False
            Me.ucEtaMaxAttivazione.Visible = False
            '--
        End If
        '--
        Me.lblCodiceLotto.Text = "Codice: " + codiceLotto
        Me.lblDescrizioneLotto.Text = "Descrizione: " + descrizioneLotto
        '--
        Me.modAttivazioneLotto.VisibileMD = True
        '--
    End Sub

    Private Sub CloseModalAttivazioneLotto()

        Me.txtCodiceLottoDaAttivare.Text = String.Empty
        Me.txtFlagAttivazioneLotto.Text = String.Empty

        Me.modAttivazioneLotto.VisibileMD = False

    End Sub

    Private Sub btnSalvaAttivazioneLotto_Click(sender As Object, e As System.EventArgs) Handles btnSalvaAttivazioneLotto.Click

        Dim attiva As Boolean = (Me.txtFlagAttivazioneLotto.Text = "S")

        ' Controlli età attivazione
        Dim totGiorniEtaMinima As Integer? = Nothing
        Dim totGiorniEtaMassima As Integer? = Nothing

        If attiva Then
            '--
            totGiorniEtaMinima = Me.ucEtaMinAttivazione.GetGiorniTotali()
            totGiorniEtaMassima = Me.ucEtaMaxAttivazione.GetGiorniTotali()
            '--
            If totGiorniEtaMinima.HasValue AndAlso totGiorniEtaMassima.HasValue AndAlso totGiorniEtaMinima.Value > totGiorniEtaMassima.Value Then
                '--
                Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Impossibile eseguire l'attivazione del lotto:\nl'Eta minima non può superare l'Eta massima.", String.Empty, False, False))
                Return
                '--
            End If
            '--
        End If

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            genericProvider.BeginTransaction()

            Try
                Dim listTestateLog As New List(Of Testata)()

                Using bizLotti As New Biz.BizLotti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                    If attiva Then
                        bizLotti.AttivaLotto(Me.txtCodiceLottoDaAttivare.Text, OnVacUtility.Variabili.CNS.Codice, 0, totGiorniEtaMinima, totGiorniEtaMassima, listTestateLog)
                    Else
                        bizLotti.DisattivaLotto(Me.txtCodiceLottoDaAttivare.Text, OnVacUtility.Variabili.CNS.Codice, listTestateLog)
                    End If

                End Using

                genericProvider.Commit()

                ' Scrittura Log
                For Each testata As Testata In listTestateLog
                    LogBox.WriteData(testata)
                Next

            Catch exc As Exception

                If Not genericProvider Is Nothing Then genericProvider.Rollback()

                exc.InternalPreserveStackTrace()
                Throw

            End Try

        End Using

        Me.CloseModalAttivazioneLotto()

        Me.OnitLayout31.Busy = False

        Me.Cerca()

    End Sub

    Private Sub btnAnnullaAttivazioneLotto_Click(sender As Object, e As System.EventArgs) Handles btnAnnullaAttivazioneLotto.Click

        Me.CloseModalAttivazioneLotto()

        Me.OnitLayout31.Busy = False

    End Sub

#End Region

#End Region

#Region " Private "

    Private Sub RedirectToDettaglio(codiceLottoSelezionato As String)

        Server.Transfer(MagazzinoUtility.GetRedirectUrl(".\DettaglioLotto.aspx",
                                                        Me.ucFiltriRicerca.GetUltimiFiltriUtilizzati(),
                                                        Me.GetCampiOrdinamentoForQueryString(),
                                                        Me.GetVersiOrdinamentoForQueryString(),
                                                        Me.dgrLotti.Items.Count > 0,
                                                        codiceLottoSelezionato, -1), False)
    End Sub

    Private Function GetCampiOrdinamentoForQueryString() As String

        Return String.Join(",", New String() {Me.Campo1OrdinamentoDatagrid, Me.Campo2OrdinamentoDatagrid, Me.Campo3OrdinamentoDatagrid})

    End Function

    Private Function GetVersiOrdinamentoForQueryString() As String

        Return String.Join(",", New String() {Me.Verso1OrdinamentoDatagrid, Me.Verso2OrdinamentoDatagrid, Me.Verso3OrdinamentoDatagrid})

    End Function

    Private Function GetListaCampiOrdinamento() As List(Of Entities.DatiOrdinamento)

        Dim listDatiOrdinamento As New List(Of Entities.DatiOrdinamento)

        listDatiOrdinamento.Add(Biz.BizLotti.CreateDatiOrdinamento(Me.Campo1OrdinamentoDatagrid, Me.Verso1OrdinamentoDatagrid, Nothing))
        listDatiOrdinamento.Add(Biz.BizLotti.CreateDatiOrdinamento(Me.Campo2OrdinamentoDatagrid, Me.Verso2OrdinamentoDatagrid, Nothing))
        listDatiOrdinamento.Add(Biz.BizLotti.CreateDatiOrdinamento(Me.Campo3OrdinamentoDatagrid, Me.Verso3OrdinamentoDatagrid, Nothing))

        Return listDatiOrdinamento

    End Function

    Private Function GetGiorniTotaliAttivazioneFromCampi(txtAnni As TextBox, txtMesi As TextBox, txtGiorni As TextBox) As Integer?

        Dim etaAttivazione As Entities.Eta = Biz.BizLotti.GetEtaFromValoreCampi(txtAnni.Text, txtMesi.Text, txtGiorni.Text)
        '--
        Dim totGiorniEta As Integer? = Nothing
        '--
        If etaAttivazione Is Nothing Then
            '--
            txtAnni.Text = String.Empty
            txtMesi.Text = String.Empty
            txtGiorni.Text = String.Empty
            '--
        Else
            '--
            txtAnni.Text = etaAttivazione.Anni.ToString()
            txtMesi.Text = etaAttivazione.Mesi.ToString()
            txtGiorni.Text = etaAttivazione.Giorni.ToString()
            '--
            totGiorniEta = etaAttivazione.GiorniTotali
            '--
        End If
        '--
        Return totGiorniEta
        '--
    End Function

#End Region

End Class
