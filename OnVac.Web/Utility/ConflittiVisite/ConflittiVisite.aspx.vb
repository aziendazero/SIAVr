Imports System.Collections.Generic

Public Class ConflittiVisite
    Inherits Common.PageBase

#Region " Properties "

    Private Property ClientIdHdIndexFlagVisibilita() As String
        Get
            Return ViewState("ClientIdHdIndexFlagVisibilita")
        End Get
        Set(value As String)
            ViewState("ClientIdHdIndexFlagVisibilita") = value
        End Set
    End Property

    Private Property StatoPagina() As StatoLayoutPagina
        Get
            If ViewState("StatoPagina") Is Nothing Then ViewState("StatoPagina") = StatoLayoutPagina.View
            Return ViewState("StatoPagina")
        End Get
        Set(value As StatoLayoutPagina)
            ViewState("StatoPagina") = value
        End Set
    End Property

#End Region

#Region " Enums "

    Private Enum StatoLayoutPagina
        View = 0
        Edit = 1
    End Enum

    Protected Enum DgrConflittoColumnIndex
        ChkSelezione = 0
        CodicePazienteCentrale = 1
        IdVisitaCentrale = 2
        CognomeNomePaziente = 3
        DataNascita = 4
        ChkDettaglio = 5
        VisiteInConflitto = 6
        ColonnaMargine = 7
    End Enum

    Protected Enum DgrDettaglioColumnIndex
        IdVisitaCentrale = 0
        IdVisitaLocale = 1
        CodicePazienteLocale = 2
        FlagConsenso = 3
        DataVisita = 4
        DescrizioneMalattia = 5
        NumeroBilancio = 6
        DescrizioneBilancio = 7
        DataFineSospensione = 8
        CodiceUslVisita = 9
        TipoVisitaCentrale = 10
    End Enum

#End Region

#Region " Page Events "

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then

            SetLayout(StatoLayoutPagina.View)
            Clear()

        End If

    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub ToolBar_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case be.Button.Key

            Case "btnCerca"

                SetLayout(StatoLayoutPagina.View)
                Cerca(0)

            Case "btnPulisci"

                Clear()

            Case "btnEdit"

                SetLayout(StatoLayoutPagina.Edit)
                Cerca(Me.dgrConflitti.CurrentPageIndex)

                ' Apertura di tutti i dettagli
                Me.ClientScript.RegisterStartupScript(Me.GetType(), "jsShowAll", "<script type='text/javascript'>document.getElementById('imgTuttiDettagli').click();</script>")

            Case "btnRisolviConflitti"

                RisolviConflitti()

            Case "btnAnnulla"

                SetLayout(StatoLayoutPagina.View)
                Cerca(Me.dgrConflitti.CurrentPageIndex)

        End Select

    End Sub

#End Region

#Region " Datagrid Events "

    Private Sub dgrConflitti_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrConflitti.ItemDataBound

        Select Case e.Item.ItemType

            Case ListItemType.AlternatingItem, ListItemType.EditItem, ListItemType.Item, ListItemType.SelectedItem

                Dim visiteInConflitto As List(Of Entities.ConflittoVisite.DatiVisitaInConflitto) =
                    DirectCast(e.Item.DataItem, Entities.ConflittoVisite).VisiteInConflitto

                Dim selectedIndex As Integer = -1

                For selectedIndex = 0 To visiteInConflitto.Count - 1
                    If visiteInConflitto(selectedIndex).FlagVisibilitaCentrale = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente Then
                        Exit For
                    End If
                Next

                ' Hidden Field contenente l'indice dell'elemento selezionato (inizialmente, quello a visibilità "V")
                Dim hdIndexFlagVisibilita As HiddenField = DirectCast(e.Item.FindControl("hdIndexFlagVisibilita"), HiddenField)
                hdIndexFlagVisibilita.Value = selectedIndex.ToString()

                Me.ClientIdHdIndexFlagVisibilita = hdIndexFlagVisibilita.ClientID

                Dim dgrDettaglio As DataGrid = DirectCast(e.Item.FindControl("dgrDettaglio"), DataGrid)
                AddHandler dgrDettaglio.ItemDataBound, AddressOf dgrDettaglio_ItemDataBound

                dgrDettaglio.DataSource = visiteInConflitto
                dgrDettaglio.DataBind()

        End Select

    End Sub

    Private Sub dgrConflitti_PageIndexChanged(source As Object, e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dgrConflitti.PageIndexChanged

        Cerca(e.NewPageIndex)

    End Sub

    Protected Sub dgrDettaglio_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs)

        Select Case e.Item.ItemType

            Case ListItemType.AlternatingItem, ListItemType.EditItem, ListItemType.Item, ListItemType.SelectedItem

                Dim datiVisitaInConflitto As Entities.ConflittoVisite.DatiVisitaInConflitto =
                    DirectCast(e.Item.DataItem, Entities.ConflittoVisite.DatiVisitaInConflitto)

                If Not datiVisitaInConflitto Is Nothing Then

                    Dim dgrDettaglio As DataGrid = DirectCast(sender, DataGrid)

                    ' Flag visibilità - hidden field e aggiunta radiobutton (client side, solo in edit)
                    Dim hdFlagVisibilita As HiddenField = DirectCast(e.Item.FindControl("hdFlagVisibilita"), HiddenField)
                    hdFlagVisibilita.Value = datiVisitaInConflitto.FlagVisibilitaCentrale

                    If Me.StatoPagina = StatoLayoutPagina.Edit Then

                        Dim rdbGroupName As String = String.Format("FlagVisibilita_{0}", dgrDettaglio.ClientID)

                        Dim checkedAttribute As String = String.Empty

                        If datiVisitaInConflitto.FlagVisibilitaCentrale = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente Then
                            checkedAttribute = "checked='true'"
                        End If

                        e.Item.Cells(3).Controls.AddAt(0, New LiteralControl(String.Format("<input type='radio' name='{0}' onclick='flagVisibilitaChanged(""{1}"", ""{2}"");' {3} title='Consenso alla comunicazione dei dati vaccinali da parte del paziente' />",
                                                                                            rdbGroupName, Me.ClientIdHdIndexFlagVisibilita, e.Item.ItemIndex.ToString(), checkedAttribute)))

                    End If

                    ' Flag visibilità - image
                    Dim imgFlagVisibilita As System.Web.UI.WebControls.Image =
                        DirectCast(e.Item.FindControl("imgFlagVisibilita"), System.Web.UI.WebControls.Image)

                    imgFlagVisibilita.ImageUrl = Common.OnVacStoricoVaccinaleCentralizzato.GetImageUrlFlagVisibilita(datiVisitaInConflitto.FlagVisibilitaCentrale, Me.Page)
                    imgFlagVisibilita.ToolTip = Common.OnVacStoricoVaccinaleCentralizzato.GetToolTipFlagVisibilita(datiVisitaInConflitto.FlagVisibilitaCentrale)

                    ' Tipo visita
                    Dim imgTipoVisitaCentrale As System.Web.UI.WebControls.Image =
                        DirectCast(e.Item.FindControl("imgTipoVisitaCentrale"), System.Web.UI.WebControls.Image)

                    Select Case datiVisitaInConflitto.TipoVisitaCentrale

                        Case Constants.TipoVisitaCentrale.Nessuno

                            imgTipoVisitaCentrale.Visible = False

                        Case Constants.TipoVisitaCentrale.Eliminata

                            imgTipoVisitaCentrale.Visible = True
                            imgTipoVisitaCentrale.ImageUrl = Me.ResolveClientUrl("~/images/flagStatoEliminata.gif")
                            imgTipoVisitaCentrale.ToolTip = "Visita ELIMINATA"

                    End Select

                End If

        End Select

    End Sub

#End Region

#Region " Private "

    Private Sub Cerca(currentPageIndex As Integer)

        If Me.ucFiltriRicercaConflitti.IsEmpty() Then
            Me.OnitLayout31.InsertRoutineJS("alert('Ricerca non effettuata: valorizzare almeno un filtro.');")
            Return
        End If

        Dim countRisultati As Integer = 0

        Dim listConflittiVisite As List(Of Entities.ConflittoVisite) = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizVisite As New Biz.BizVisite(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Dim filtriRicercaConflittiDatiVaccinali As Filters.FiltriRicercaConflittiDatiVaccinali = Me.ucFiltriRicercaConflitti.GetFiltriRicerca()

                countRisultati = bizVisite.CountConflittiVisite(filtriRicercaConflittiDatiVaccinali)

                listConflittiVisite = bizVisite.GetConflittiVisite(filtriRicercaConflittiDatiVaccinali, currentPageIndex, Me.dgrConflitti.PageSize)

            End Using

        End Using

        SetLabelRisultati(countRisultati)

        BindDatagrid(listConflittiVisite, countRisultati, currentPageIndex)

        Me.dgrConflitti.SelectedIndex = -1

    End Sub

    Private Sub RisolviConflitti()

        Dim risolviConflittoCommand As New Biz.BizPaziente.RisolviConflittoVisiteCentraleCommand()

        risolviConflittoCommand.IdVisiteCentraleDictionary = New Dictionary(Of Int64, IEnumerable(Of Int64))()

        ' Aggiunta delle entry (id visite "Master" e lista visite in conflitto) al dictionary che verrà utilizzato dal metodo di risoluzione dei conflitti
        For Each dataGridItem As DataGridItem In Me.dgrConflitti.Items

            Dim idVisitaCentraleMaster As Int64
            Dim listIdVisiteInConflitto As New List(Of Int64)()

            If DirectCast(dataGridItem.FindControl("chkSelezione"), CheckBox).Checked Then

                ' Indice di riga della visita selezionata come master
                Dim hdIndexFlagVisibilita As HiddenField = DirectCast(dataGridItem.FindControl("hdIndexFlagVisibilita"), HiddenField)

                If Not hdIndexFlagVisibilita Is Nothing AndAlso Not String.IsNullOrEmpty(hdIndexFlagVisibilita.Value) Then

                    Dim selectedIndex As Int16 = Convert.ToInt16(hdIndexFlagVisibilita.Value)

                    If selectedIndex > -1 Then

                        Dim dgrDettaglio As DataGrid = DirectCast(dataGridItem.FindControl("dgrDettaglio"), DataGrid)

                        idVisitaCentraleMaster = Me.GetIdVisita(dgrDettaglio.Items(selectedIndex))

                        listIdVisiteInConflitto = (From dettaglioItem As DataGridItem In dgrDettaglio.Items
                                                   Where dettaglioItem.ItemIndex <> selectedIndex
                                                   Select Me.GetIdVisita(dettaglioItem)).ToList()

                        risolviConflittoCommand.IdVisiteCentraleDictionary.Add(idVisitaCentraleMaster,
                                                                               listIdVisiteInConflitto.AsEnumerable())

                    End If

                End If

            End If

        Next

        If risolviConflittoCommand.IdVisiteCentraleDictionary.Count = 0 Then

            Me.OnitLayout31.InsertRoutineJS("alert('Risoluzione conflitti non effettuata: nessun conflitto selezionato.');")

        Else

            Using bizPaziente As Biz.BizPaziente = New Biz.BizPaziente(Me.Settings, Nothing, OnVacContext.CreateBizContextInfos(), Nothing)
                bizPaziente.RisolviConflittoVisiteCentrale(risolviConflittoCommand)
            End Using

            SetLayout(StatoLayoutPagina.View)
            Cerca(Me.dgrConflitti.CurrentPageIndex)

        End If

    End Sub

    Private Function GetIdVisita(dettaglioItem As DataGridItem) As Int64

        Return Convert.ToInt64(DirectCast(dettaglioItem.FindControl("lblIdVisitaCentrale"), Label).Text)

    End Function

    Private Sub Clear()

        Me.ucFiltriRicercaConflitti.SetFiltriRicerca(Nothing)

        SetLabelRisultati(-1)

        Me.BindDatagrid(Nothing, 0, 0)

    End Sub

    Private Sub SetLabelRisultati(countRisultati As Integer)

        Dim msg As String

        Select Case countRisultati

            Case -1
                msg = String.Empty

            Case 0
                msg = ": nessun record trovato"

            Case 1
                msg = ": 1 record trovato"

            Case Else
                msg = String.Format(": {0} record trovati", countRisultati.ToString())

        End Select

        Me.lblRisultati.Text = String.Format("Risultati della ricerca{0}", msg)

    End Sub

    Private Sub BindDatagrid(results As List(Of Entities.ConflittoVisite), countRisultati As Integer, currentPageIndex As Integer)

        Me.dgrConflitti.VirtualItemCount = countRisultati
        Me.dgrConflitti.CurrentPageIndex = currentPageIndex

        Me.dgrConflitti.DataSource = results
        Me.dgrConflitti.DataBind()

        Me.dgrConflitti.SelectedIndex = -1

        Me.ToolBar.Items.FromKeyButton("btnEdit").Enabled = (Me.StatoPagina = StatoLayoutPagina.View AndAlso Me.dgrConflitti.Items.Count > 0)

    End Sub

    Private Sub SetLayout(statoLayoutPagina As StatoLayoutPagina)

        Me.StatoPagina = statoLayoutPagina

        Dim isInEdit As Boolean = (statoLayoutPagina = ConflittiVisite.StatoLayoutPagina.Edit)

        ' Toolbar
        Me.ToolBar.Items.FromKeyButton("btnCerca").Enabled = Not isInEdit
        Me.ToolBar.Items.FromKeyButton("btnPulisci").Enabled = Not isInEdit
        Me.ToolBar.Items.FromKeyButton("btnEdit").Enabled = Not isInEdit
        Me.ToolBar.Items.FromKeyButton("btnRisolviConflitti").Enabled = isInEdit
        Me.ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = isInEdit

        ' Filtri
        Me.ucFiltriRicercaConflitti.Enabled = Not isInEdit

        ' Datagrid
        Me.dgrConflitti.Columns(DgrConflittoColumnIndex.ChkSelezione).Visible = isInEdit

        Me.dgrConflitti.AllowPaging = Not isInEdit
        Me.dgrConflitti.AllowCustomPaging = Not isInEdit

        Me.OnitLayout31.Busy = isInEdit

    End Sub

#End Region

#Region " Protected "

    Protected Function BindDateField(value As Object) As String

        If value Is Nothing OrElse value Is DBNull.Value Then Return String.Empty

        Dim dateValue As DateTime

        Try
            dateValue = Convert.ToDateTime(value)
        Catch ex As Exception
            dateValue = DateTime.MinValue
        End Try

        If dateValue = DateTime.MinValue Then Return String.Empty

        Return String.Format("{0:dd/MM/yyyy}", dateValue)

    End Function

#End Region

End Class