Imports System.Collections.Generic

Public Class ConflittiVaccinazioni
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
        IdVaccinazioneEseguitaCentrale = 2
        CognomeNomePaziente = 3
        DataNascita = 4
        ChkDettaglio = 5
        VaccinazioniInConflitto = 6
        ColonnaMargine = 7
    End Enum

    Protected Enum DgrDettaglioColumnIndex
        IdVaccinazioneCentrale = 0
        IdVaccinazioneLocale = 1
        CodicePazienteLocale = 2
        FlagConsenso = 3
        DataEffettuazione = 4
        Associazione = 5
        Vaccinazione = 6
        CodiceNomeCommerciale = 7
        CodiceLotto = 8
        CodiceUslVaccinazioneEseguita = 9
        ImgReazioneAvversa = 10
        TipoVaccinazioneEseguitaCentrale = 11
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

                Dim vaccinazioniEseguiteInConflitto As List(Of Entities.ConflittoVaccinazioniEseguite.DatiVaccinazioneInConflitto) =
                    DirectCast(e.Item.DataItem, Entities.ConflittoVaccinazioniEseguite).VaccinazioniEseguiteInConflitto

                Dim selectedIndex As Integer = -1

                For selectedIndex = 0 To vaccinazioniEseguiteInConflitto.Count - 1
                    If vaccinazioniEseguiteInConflitto(selectedIndex).FlagVisibilitaCentrale = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente Then
                        Exit For
                    End If
                Next

                ' Hidden Field contenente l'indice dell'elemento selezionato (inizialmente, quello a visibilità "V")
                Dim hdIndexFlagVisibilita As HiddenField = DirectCast(e.Item.FindControl("hdIndexFlagVisibilita"), HiddenField)
                hdIndexFlagVisibilita.Value = selectedIndex.ToString()

                Me.ClientIdHdIndexFlagVisibilita = hdIndexFlagVisibilita.ClientID

                Dim dgrDettaglio As DataGrid = DirectCast(e.Item.FindControl("dgrDettaglio"), DataGrid)
                AddHandler dgrDettaglio.ItemDataBound, AddressOf dgrDettaglio_ItemDataBound

                dgrDettaglio.DataSource = vaccinazioniEseguiteInConflitto
                dgrDettaglio.DataBind()

        End Select

    End Sub

    Private Sub dgrConflitti_PageIndexChanged(source As Object, e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dgrConflitti.PageIndexChanged

        Cerca(e.NewPageIndex)

    End Sub

    Protected Sub dgrDettaglio_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs)

        Select Case e.Item.ItemType

            Case ListItemType.AlternatingItem, ListItemType.EditItem, ListItemType.Item, ListItemType.SelectedItem

                Dim datiVaccinazioneInConflitto As Entities.ConflittoVaccinazioniEseguite.DatiVaccinazioneInConflitto =
                    DirectCast(e.Item.DataItem, Entities.ConflittoVaccinazioniEseguite.DatiVaccinazioneInConflitto)

                If Not datiVaccinazioneInConflitto Is Nothing Then

                    Dim dgrDettaglio As DataGrid = DirectCast(sender, DataGrid)

                    ' Flag visibilità - hidden field e aggiunta radiobutton (client side, solo in edit)
                    Dim hdFlagVisibilita As HiddenField = DirectCast(e.Item.FindControl("hdFlagVisibilita"), HiddenField)
                    hdFlagVisibilita.Value = datiVaccinazioneInConflitto.FlagVisibilitaCentrale

                    If Me.StatoPagina = StatoLayoutPagina.Edit Then

                        Dim rdbGroupName As String = String.Format("FlagVisibilita_{0}", dgrDettaglio.ClientID)

                        Dim checkedAttribute As String = String.Empty

                        If datiVaccinazioneInConflitto.FlagVisibilitaCentrale = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente Then
                            checkedAttribute = "checked='true'"
                        End If

                        e.Item.Cells(3).Controls.AddAt(0, New LiteralControl(String.Format("<input type='radio' name='{0}' onclick='flagVisibilitaChanged(""{1}"", ""{2}"");' {3} title='Consenso alla comunicazione dei dati vaccinali da parte del paziente' />",
                                                                                            rdbGroupName, Me.ClientIdHdIndexFlagVisibilita, e.Item.ItemIndex.ToString(), checkedAttribute)))

                    End If

                    ' Flag visibilità - image
                    Dim imgFlagVisibilita As System.Web.UI.WebControls.Image =
                        DirectCast(e.Item.FindControl("imgFlagVisibilita"), System.Web.UI.WebControls.Image)

                    imgFlagVisibilita.ImageUrl = Common.OnVacStoricoVaccinaleCentralizzato.GetImageUrlFlagVisibilita(datiVaccinazioneInConflitto.FlagVisibilitaCentrale, Me.Page)
                    imgFlagVisibilita.ToolTip = Common.OnVacStoricoVaccinaleCentralizzato.GetToolTipFlagVisibilita(datiVaccinazioneInConflitto.FlagVisibilitaCentrale)

                    ' Reazione avversa
                    Dim imgReazioneAvversa As System.Web.UI.WebControls.Image =
                        DirectCast(e.Item.FindControl("imgReazioneAvversa"), System.Web.UI.WebControls.Image)

                    If datiVaccinazioneInConflitto.IdReazioneAvversa.HasValue AndAlso
                       datiVaccinazioneInConflitto.TipoReazioneAvversa <> Constants.TipoReazioneAvversaCentrale.Eliminata Then

                        imgReazioneAvversa.Visible = True
                        imgReazioneAvversa.ImageUrl = Me.ResolveClientUrl("~/images/reazioniavverse.gif")
                        imgReazioneAvversa.ToolTip = "Reazione avversa presente"

                    Else

                        imgReazioneAvversa.Visible = False

                    End If

                    ' Tipo vaccinazione
                    Dim imgTipoVaccinazioneEseguitaCentrale As System.Web.UI.WebControls.Image =
                        DirectCast(e.Item.FindControl("imgTipoVaccinazioneEseguitaCentrale"), System.Web.UI.WebControls.Image)

                    If String.IsNullOrEmpty(datiVaccinazioneInConflitto.TipoVaccinazioneEseguitaCentrale) Then

                        imgTipoVaccinazioneEseguitaCentrale.Visible = False

                    Else

                        imgTipoVaccinazioneEseguitaCentrale.Visible = True

                        Select Case datiVaccinazioneInConflitto.TipoVaccinazioneEseguitaCentrale

                            Case Constants.TipoVaccinazioneEseguitaCentrale.Eliminata
                                imgTipoVaccinazioneEseguitaCentrale.ImageUrl = Me.ResolveClientUrl("~/images/flagStatoEliminata.gif")
                                imgTipoVaccinazioneEseguitaCentrale.ToolTip = "Vaccinazione ELIMINATA"

                            Case Constants.TipoVaccinazioneEseguitaCentrale.Programmata
                                imgTipoVaccinazioneEseguitaCentrale.ImageUrl = Me.ResolveClientUrl("~/images/flagStatoProgrammata.png")
                                imgTipoVaccinazioneEseguitaCentrale.ToolTip = "Vaccinazione PROGRAMMATA"

                            Case Constants.TipoVaccinazioneEseguitaCentrale.Recuperata
                                imgTipoVaccinazioneEseguitaCentrale.ImageUrl = Me.ResolveClientUrl("~/images/flagStatoRecuperata.png")
                                imgTipoVaccinazioneEseguitaCentrale.ToolTip = "Vaccinazione RECUPERATA"

                            Case Constants.TipoVaccinazioneEseguitaCentrale.Registrata
                                imgTipoVaccinazioneEseguitaCentrale.ImageUrl = Me.ResolveClientUrl("~/images/flagStatoRegistrata.png")
                                imgTipoVaccinazioneEseguitaCentrale.ToolTip = "Vaccinazione REGISTRATA"

                            Case Constants.TipoVaccinazioneEseguitaCentrale.Ripristinata
                                imgTipoVaccinazioneEseguitaCentrale.ImageUrl = Me.ResolveClientUrl("~/images/flagStatoRipristinata.png")
                                imgTipoVaccinazioneEseguitaCentrale.ToolTip = "Vaccinazione RIPRISTINATA"

                            Case Constants.TipoVaccinazioneEseguitaCentrale.Scaduta
                                imgTipoVaccinazioneEseguitaCentrale.ImageUrl = Me.ResolveClientUrl("~/images/flagStatoScaduta.png")
                                imgTipoVaccinazioneEseguitaCentrale.ToolTip = "Vaccinazione SCADUTA"

                            Case Constants.TipoVaccinazioneEseguitaCentrale.ScadutaEliminata
                                imgTipoVaccinazioneEseguitaCentrale.ImageUrl = Me.ResolveClientUrl("~/images/flagStatoScadutaEliminata.png")
                                imgTipoVaccinazioneEseguitaCentrale.ToolTip = "Vaccinazione SCADUTA ELIMINATA"

                        End Select

                    End If

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

        Dim listConflittiVaccinazioniEseguite As List(Of Entities.ConflittoVaccinazioniEseguite) = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizVaccinazioniEseguite As New Biz.BizVaccinazioniEseguite(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                Dim filtriRicercaConflittiDatiVaccinali As Filters.FiltriRicercaConflittiDatiVaccinali = Me.ucFiltriRicercaConflitti.GetFiltriRicerca()

                countRisultati =
                    bizVaccinazioniEseguite.CountConflittiVaccinazioniEseguite(filtriRicercaConflittiDatiVaccinali)

                listConflittiVaccinazioniEseguite =
                    bizVaccinazioniEseguite.GetConflittiVaccinazioniEseguite(filtriRicercaConflittiDatiVaccinali, currentPageIndex, Me.dgrConflitti.PageSize)

            End Using

        End Using

        SetLabelRisultati(countRisultati)

        BindDatagrid(listConflittiVaccinazioniEseguite, countRisultati, currentPageIndex)

        Me.dgrConflitti.SelectedIndex = -1

    End Sub

    Private Sub RisolviConflitti()

        Dim risolviConflittoCommand As New Biz.BizPaziente.RisolviConflittoVaccinazioniEseguiteCentraleCommand()

        risolviConflittoCommand.IdVaccinazioniEseguiteCentralePazienteDictionary = New Dictionary(Of Int64, IEnumerable(Of Int64))()

        ' Aggiunta delle entry (id vaccinazioni "Master" e lista vaccinazioni in conflitto) al dictionary che verrà utilizzato dal metodo di risoluzione dei conflitti
        For Each dataGridItem As DataGridItem In Me.dgrConflitti.Items

            Dim listIdVaccinazioniEseguiteInConflitto As New List(Of Int64)()

            If DirectCast(dataGridItem.FindControl("chkSelezione"), CheckBox).Checked Then

                ' Indice di riga della vaccinazione selezionata come master
                Dim hdIndexFlagVisibilita As HiddenField = DirectCast(dataGridItem.FindControl("hdIndexFlagVisibilita"), HiddenField)

                If Not hdIndexFlagVisibilita Is Nothing AndAlso Not String.IsNullOrEmpty(hdIndexFlagVisibilita.Value) Then

                    Dim selectedIndex As Int16 = Convert.ToInt16(hdIndexFlagVisibilita.Value)

                    If selectedIndex > -1 Then

                        Dim dgrDettaglio As DataGrid = DirectCast(dataGridItem.FindControl("dgrDettaglio"), DataGrid)

                        Dim idVaccinazioneEseguitaCentrale As Int64 = Me.GetIdVaccinazione(dgrDettaglio.Items(selectedIndex))

                        listIdVaccinazioniEseguiteInConflitto = (From dettaglioItem As DataGridItem In dgrDettaglio.Items
                                                                 Where dettaglioItem.ItemIndex <> selectedIndex
                                                                 Select Me.GetIdVaccinazione(dettaglioItem)).ToList()


                        ' Imposto id vaccinazione master e lista id vaccinazioni in conflitto
                        risolviConflittoCommand.IdVaccinazioniEseguiteCentralePazienteDictionary.Add(idVaccinazioneEseguitaCentrale,
                                                                                             listIdVaccinazioniEseguiteInConflitto.AsEnumerable())

                    End If

                End If

            End If

        Next

        If risolviConflittoCommand.IdVaccinazioniEseguiteCentralePazienteDictionary.Count = 0 Then

            Me.OnitLayout31.InsertRoutineJS("alert('Risoluzione conflitti non effettuata: nessun conflitto selezionato.');")

        Else

            Using bizPaziente As Biz.BizPaziente = New Biz.BizPaziente(Me.Settings, Nothing, OnVacContext.CreateBizContextInfos(), Nothing)
                bizPaziente.RisolviConflittoVaccinazioniEseguiteCentrale(risolviConflittoCommand)
            End Using

            SetLayout(StatoLayoutPagina.View)
            Cerca(Me.dgrConflitti.CurrentPageIndex)

        End If

    End Sub

    Private Function GetIdVaccinazione(dettaglioItem As DataGridItem) As Int64

        Return Convert.ToInt64(DirectCast(dettaglioItem.FindControl("lblIdVaccinazioneCentrale"), Label).Text)

    End Function

    Private Function GetIdReazione(dettaglioItem As DataGridItem) As Int64

        Dim valueIdReazione As String = DirectCast(dettaglioItem.FindControl("hdIdReazioneAvversa"), HiddenField).Value

        Dim tipoReazione As String = DirectCast(dettaglioItem.FindControl("hdTipoReazioneAvversa"), HiddenField).Value

        If tipoReazione = Constants.TipoReazioneAvversaCentrale.Eliminata OrElse String.IsNullOrEmpty(valueIdReazione) Then Return 0

        Return Convert.ToInt64(valueIdReazione)

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

    Private Sub BindDatagrid(results As List(Of Entities.ConflittoVaccinazioniEseguite), countRisultati As Integer, currentPageIndex As Integer)

        Me.dgrConflitti.VirtualItemCount = countRisultati
        Me.dgrConflitti.CurrentPageIndex = currentPageIndex

        Me.dgrConflitti.DataSource = results
        Me.dgrConflitti.DataBind()

        Me.dgrConflitti.SelectedIndex = -1

        Me.ToolBar.Items.FromKeyButton("btnEdit").Enabled = (Me.StatoPagina = StatoLayoutPagina.View AndAlso Me.dgrConflitti.Items.Count > 0)

    End Sub

    Private Sub SetLayout(statoLayoutPagina As StatoLayoutPagina)

        Me.StatoPagina = statoLayoutPagina

        Dim isInEdit As Boolean = (statoLayoutPagina = ConflittiVaccinazioni.StatoLayoutPagina.Edit)

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
