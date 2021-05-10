Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager


Partial Public Class DettaglioLotto
    Inherits OnVac.Common.PageBase

#Region " Properties "

    Private Property CodiceLottoSelezionato() As String
        Get
            If ViewState("CodiceLottoSelezionato") Is Nothing Then
                ViewState("CodiceLottoSelezionato") = Request.QueryString.Get("codSel")
            End If

            Return ViewState("CodiceLottoSelezionato")
        End Get
        Set(value As String)
            ViewState("CodiceLottoSelezionato") = value
        End Set
    End Property

    Private Property StatoPagina() As MagazzinoEnums.StatiPagina
        Get
            If ViewState("StatoPaginaDettaglio") Is Nothing Then
                ViewState("StatoPaginaDettaglio") = MagazzinoEnums.StatiPagina.VisualizzazioneDati
            End If

            Return ViewState("StatoPaginaDettaglio")
        End Get
        Set(value As MagazzinoEnums.StatiPagina)
            ViewState("StatoPaginaDettaglio") = value
        End Set
    End Property

#End Region

#Region " Enums "

    Private Enum DgrMovimentiColumnIndex
        DataRegistrazioneString = 0
        Quantita = 1
        Movimento = 2
        LuogoTrasferimento = 3
        Operatore = 4
        UnitaMisura = 5
        Note = 6
        DataRegistrazioneDate = 7           ' nascosta
    End Enum

#End Region

#Region " Classes "

    Private Class OnitLayout31ConfirmKeys
        Public Shared ReadOnly Property ConfirmSequestroLotto() As String
            Get
                Return "ConfirmSequestroLotto"
            End Get
        End Property
    End Class

#End Region

#Region " Page Events "

    Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        MagazzinoUtility.SetToolbarButtonImages("btnIndietro", "prev.gif", Me.ToolBar)
        MagazzinoUtility.SetToolbarButtonImages("btnSalva", "salva.gif", Me.ToolBar)
        MagazzinoUtility.SetToolbarButtonImages("btnAnnulla", "annulla.gif", Me.ToolBar)

    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then

            ' Lettura querystring per apertura in inserimento
            If String.IsNullOrEmpty(Me.CodiceLottoSelezionato) Then

                Me.SetDatiInserimentoLotto()
                Me.SetLayoutState(MagazzinoEnums.StatiPagina.InserimentoDati)

            Else

                Me.CaricamentoDatiLotto(Me.CodiceLottoSelezionato)
                Me.CaricamentoMovimentiLotto(Me.CodiceLottoSelezionato, 0)

                Me.SetLayoutState(MagazzinoEnums.StatiPagina.VisualizzazioneDati)

            End If

        End If

    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub ToolBar_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case be.Button.Key

            Case "btnIndietro"

                Me.RedirectToRicerca()

            Case "btnModifica"

                Me.SetLayoutState(MagazzinoEnums.StatiPagina.ModificaDati)

            Case "btnMovimenti"

                Me.InsertMovimento()

                Me.SetLayoutState(MagazzinoEnums.StatiPagina.InserimentoMovimento)

            Case "btnSalva"

                If Me.StatoPagina <> MagazzinoEnums.StatiPagina.InserimentoMovimento AndAlso Me.ConfermaSequestro() Then

                    ' Fa scattare l'evento ConfirmClick dell'OnitLayout31
                    Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(
                                               "Attenzione: il lotto verrà reso inutilizzabile per tutti i centri vaccinali. Continuare?",
                                               OnitLayout31ConfirmKeys.ConfirmSequestroLotto, True, True))
                Else

                    Me.Salva(False)

                End If

            Case "btnAnnulla"

                If String.IsNullOrEmpty(Me.CodiceLottoSelezionato) Then

                    Me.RedirectToRicerca()

                Else

                    Me.CaricamentoDatiLotto(Me.CodiceLottoSelezionato)
                    Me.CaricamentoMovimentiLotto(Me.CodiceLottoSelezionato, Me.dgrMovimenti.CurrentPageIndex)

                    Me.SetLayoutState(MagazzinoEnums.StatiPagina.VisualizzazioneDati)

                End If

        End Select

    End Sub

    Private Sub RedirectToRicerca()

        Dim codice As String = Me.CodiceLottoSelezionato

        Me.CodiceLottoSelezionato = String.Empty

        Server.Transfer(MagazzinoUtility.GetRedirectUrl(".\RicercaLotti.aspx", Request.QueryString, codice))

    End Sub

#End Region

#Region " OnitLayout Events "

    Private Sub OnitLayout31_ConfirmClick(sender As Object, e As Controls.PagesLayout.OnitLayout3.ConfirmEventArgs) Handles OnitLayout31.ConfirmClick

        Select Case e.Key

            Case OnitLayout31ConfirmKeys.ConfirmSequestroLotto

                ' Richiesta di continuare con il salvataggio del lotto sequestrato: 
                ' prevede l'obsolescenza del lotto su tutti i consultori
                If e.Result Then
                    Me.Salva(True)
                End If

        End Select

    End Sub

#End Region

#Region " Layout "

    Private Sub SetLayoutState(stato As MagazzinoEnums.StatiPagina)

        Me.StatoPagina = stato
        Me.SetToolbarState(stato)

        ' Dati lotto e movimenti
        Select Case stato

            Case MagazzinoEnums.StatiPagina.VisualizzazioneDati
                ' uc e datagrid disabilitati
                Me.ucDatiLotto.SetEnable(False)

                Me.OnitLayout31.Busy = False

            Case MagazzinoEnums.StatiPagina.InserimentoDati, MagazzinoEnums.StatiPagina.ModificaDati
                ' uc abilitato, datagrid disabilitato
                Me.ucDatiLotto.SetEnable(True)

                Page.ClientScript.RegisterStartupScript(Me.GetType(), "jsFocusLotto", Me.ucDatiLotto.GetJSFocusLotto())

                Me.OnitLayout31.Busy = True

            Case MagazzinoEnums.StatiPagina.InserimentoMovimento
                ' uc disabilitato, datagrid abilitato
                Me.ucDatiLotto.SetEnable(False)

                Me.OnitLayout31.Busy = True

        End Select

    End Sub

    Private Sub SetToolbarState(stato As MagazzinoEnums.StatiPagina)

        Me.ToolBar.Items.FromKeyButton("btnIndietro").Enabled = (stato = MagazzinoEnums.StatiPagina.VisualizzazioneDati)
        Me.ToolBar.Items.FromKeyButton("btnModifica").Enabled = (stato = MagazzinoEnums.StatiPagina.VisualizzazioneDati)
        Me.ToolBar.Items.FromKeyButton("btnMovimenti").Enabled = (stato = MagazzinoEnums.StatiPagina.VisualizzazioneDati)
        Me.ToolBar.Items.FromKeyButton("btnSalva").Enabled = (stato <> MagazzinoEnums.StatiPagina.VisualizzazioneDati)
        Me.ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = (stato <> MagazzinoEnums.StatiPagina.VisualizzazioneDati)

    End Sub

    Private Sub BindDatagridMovimenti(listMovimenti As List(Of Entities.MovimentoLotto), countMovimenti As Integer, currentPageIndex As Integer, editItemIndex As Integer)

        Dim isEditMovimento As Boolean = (editItemIndex > -1)

        Me.dgrMovimenti.VirtualItemCount = countMovimenti
        Me.dgrMovimenti.CurrentPageIndex = currentPageIndex
        Me.dgrMovimenti.EditItemIndex = editItemIndex

        Me.dgrMovimenti.DataSource = listMovimenti
        Me.dgrMovimenti.DataBind()

        Me.dgrMovimenti.PagerStyle.Visible = Not isEditMovimento

        ' Colonna "Unità di misura" visibile solo in inserimento di un movimento 
        Me.dgrMovimenti.Columns(DgrMovimentiColumnIndex.UnitaMisura).Visible = isEditMovimento

        ' Radiobutton "Scatole" visibile solo in inserimento di un movimento e solo se il parametro è "S"
        If isEditMovimento AndAlso Not Me.Settings.GESDOSISCATOLA Then

            Dim rbtScatole As RadioButton = DirectCast(Me.dgrMovimenti.Items(editItemIndex).FindControl("rbtScatole"), RadioButton)

            If Not rbtScatole Is Nothing Then
                rbtScatole.Visible = False
            End If

        End If

    End Sub

    Private Sub RegisterScriptHideModaleTrasferimento(datagridItem As DataGridItem)

        Dim obj As Object = datagridItem.FindControl("ddlTipoMovimento")
        If obj Is Nothing Then Exit Sub

        Dim ddlTipoMovimento As DropDownList = DirectCast(obj, DropDownList)

        Dim scriptHideDDL As New System.Text.StringBuilder()
        scriptHideDDL.Append("<script type='text/javascript'>")
        scriptHideDDL.AppendFormat("AbilitaTrasferimento(null, document.getElementById('{0}'));", ddlTipoMovimento.ClientID)
        scriptHideDDL.Append("</script>")

        Page.ClientScript.RegisterStartupScript(Me.GetType(), "hideDDL", scriptHideDDL.ToString())

    End Sub

    Private Sub RegisterScriptFocusQuantita()

        Dim obj As Object = Me.dgrMovimenti.Items(Me.dgrMovimenti.EditItemIndex).FindControl("txtQuantita")
        If obj Is Nothing Then Exit Sub

        Dim txtQuantita As TextBox = DirectCast(obj, TextBox)

        Dim scriptFocusQta As New System.Text.StringBuilder()
        scriptFocusQta.Append("<script type='text/javascript'>")
        scriptFocusQta.AppendFormat("var txt = document.getElementById('{0}');", txtQuantita.ClientID)
        scriptFocusQta.Append("if (txt != null) txt.focus();")
        scriptFocusQta.Append("</script>")

        Page.ClientScript.RegisterStartupScript(Me.GetType(), "focusQuantita", scriptFocusQta.ToString())

    End Sub

#End Region

#Region " Dati lotto e movimenti "

    ' Imposta lo user control per l'inserimento, assegnando i valori scritti nei filtri ai campi dello user control.
    Private Sub SetDatiInserimentoLotto()

        Me.ucDatiLotto.Modalita = InsDatiLotto.Mode.Nuovo

        Dim filtroLottiMagazzino As Filters.FiltriRicercaLottiMagazzino = MagazzinoUtility.GetFiltersFromQueryString(Request.QueryString)

        Dim newLotto As New Entities.LottoMagazzino()
        newLotto.CodiceLotto = filtroLottiMagazzino.CodiceLotto
        newLotto.DescrizioneLotto = filtroLottiMagazzino.DescrizioneLotto
        newLotto.CodiceNomeCommerciale = filtroLottiMagazzino.CodiceNomeCommerciale
        newLotto.DescrizioneNomeCommerciale = filtroLottiMagazzino.DescrizioneNomeCommerciale

        Me.ucDatiLotto.SetLottoMagazzino(newLotto)

    End Sub

    ' Caricamento dati del lotto e impostazione valori nello user control
    Private Sub CaricamentoDatiLotto(codiceLotto As String)

        Dim lottoMagazzino As Entities.LottoMagazzino = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizLotti As New Biz.BizLotti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                lottoMagazzino = bizLotti.LoadLottoMagazzino(OnVacUtility.Variabili.CNS.Codice, OnVacUtility.Variabili.CNSMagazzino.Codice, codiceLotto)

            End Using
        End Using

        If lottoMagazzino Is Nothing Then lottoMagazzino = New Entities.LottoMagazzino()

        Me.ucDatiLotto.Modalita = InsDatiLotto.Mode.Modifica
        Me.ucDatiLotto.SetLottoMagazzino(lottoMagazzino)

    End Sub

    ' Caricamento movimenti con paginazione su db
    Private Sub CaricamentoMovimentiLotto(codiceLotto As String, currentPageIndex As Integer)

        Dim countMovimenti As Integer = 0

        Dim listMovimenti As List(Of Entities.MovimentoLotto) = Nothing

        Dim pagingOptions As New Onit.OnAssistnet.Data.PagingOptions()
        pagingOptions.StartRecordIndex = currentPageIndex * Me.dgrMovimenti.PageSize
        pagingOptions.EndRecordIndex = pagingOptions.StartRecordIndex + Me.dgrMovimenti.PageSize

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizLotti As New Biz.BizLotti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                listMovimenti = bizLotti.LoadMovimentiLotto(codiceLotto, OnVacUtility.Variabili.CNSMagazzino.Codice, pagingOptions)

                countMovimenti = bizLotti.CountMovimentiLotto(codiceLotto, OnVacUtility.Variabili.CNSMagazzino.Codice)

            End Using
        End Using

        Me.BindDatagridMovimenti(listMovimenti, countMovimenti, currentPageIndex, -1)

    End Sub

    ' Ricarica i movimenti e inserisce una riga in edit ad inizio datagrid
    Private Sub InsertMovimento()

        ' Caricamento movimenti (sempre prima pagina)
        Dim listMovimenti As List(Of Entities.MovimentoLotto) = Nothing

        Dim pagingOptions As New Onit.OnAssistnet.Data.PagingOptions()
        pagingOptions.StartRecordIndex = 0
        pagingOptions.EndRecordIndex = Me.dgrMovimenti.PageSize

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizLotti As New Biz.BizLotti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                listMovimenti = bizLotti.LoadMovimentiLotto(Me.CodiceLottoSelezionato, OnVacUtility.Variabili.CNSMagazzino.Codice, pagingOptions)

            End Using

        End Using

        ' Aggiunta nuovo movimento "vuoto" ai risultati (come primo elemento)
        Dim movimentoLotto As New Entities.MovimentoLotto()
        movimentoLotto.DataRegistrazione = Date.Now
        movimentoLotto.CodiceLotto = Me.CodiceLottoSelezionato
        movimentoLotto.CodiceConsultorio = OnVacUtility.Variabili.CNSMagazzino.Codice
        movimentoLotto.IdUtente = OnVacContext.UserId

        listMovimenti.Insert(0, movimentoLotto)

        Me.BindDatagridMovimenti(listMovimenti, dgrMovimenti.VirtualItemCount, 0, 0)

        Me.RegisterScriptFocusQuantita()

    End Sub

#End Region

#Region " Salvataggio "

    Private Sub Salva(disattivaLotto As Boolean)

        If Me.SalvaOperazione(disattivaLotto) Then

            Me.CodiceLottoSelezionato = Me.ucDatiLotto.GetCodiceLotto()

            Me.CaricamentoDatiLotto(Me.CodiceLottoSelezionato)
            Me.CaricamentoMovimentiLotto(Me.CodiceLottoSelezionato, Me.dgrMovimenti.CurrentPageIndex)

            Me.SetLayoutState(MagazzinoEnums.StatiPagina.VisualizzazioneDati)

        End If

    End Sub

    Private Function SalvaOperazione(disattivaLotto As Boolean) As Boolean

        Dim ok As Boolean = True

        If Me.StatoPagina = MagazzinoEnums.StatiPagina.InserimentoMovimento Then

            ' Inserimento movimento e aggiornamento dosi rimaste lotto-consultorio.
            ' In caso di trasferimento viene inserito anche il movimento oppposto nel consultorio di destinazione
            ok = Me.InserisciMovimentoLotto()

        Else

            ' Inserimento/Modifica ed eventuale disattivazione del lotto su tutti i cns
            ok = Me.SalvaDatiLotto(disattivaLotto)

        End If

        Return ok

    End Function

    Private Function SalvaDatiLotto(disattivaLotto As Boolean) As Boolean

        Dim resultMessage As String = String.Empty

        Dim bizLottiResult As Biz.BizLotti.BizLottiResult

        Dim lottoMagazzino As Entities.LottoMagazzino = Me.ucDatiLotto.GetLottoMagazzino()
        lottoMagazzino.CodiceConsultorio = OnVacUtility.Variabili.CNSMagazzino.Codice

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizLotti As New Biz.BizLotti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                Dim listTestateLog As New List(Of Log.DataLogStructure.Testata)()

                If Me.StatoPagina = MagazzinoEnums.StatiPagina.InserimentoDati Then

                    ' Inserimento lotto (anagrafica, lotto-consultorio, primo carico se quantità iniziale > 0, eventuale disattivazione del lotto)
                    ' e scrittura del log (tranne che in caso di errore)
                    bizLottiResult = bizLotti.InsertLottoMagazzino(lottoMagazzino, True, OnVacUtility.Variabili.CNS.Codice,
                                                                   OnVacUtility.Variabili.CNSMagazzino.Codice, Me.ucDatiLotto.GetQuantitaIniziale(),
                                                                   (Me.ucDatiLotto.GetUnitaMisuraDose() = Enumerators.UnitaMisuraLotto.Scatola),
                                                                   disattivaLotto, Me.Settings.ASSOCIA_LOTTI_ETA)

                    ' Se non è possibile attivare il lotto, l'inserimento avviene comunque, 
                    ' ma il lotto viene inserito non attivo e l'utente viene avvertito.
                    If bizLottiResult.Result = Biz.BizLotti.BizLottiResult.ResultType.IsActiveLottoWarning Then
                        resultMessage = String.Format("Il lotto è stato inserito ma non è stato attivato. {0}", bizLottiResult.Message)
                    Else
                        resultMessage = bizLottiResult.Message
                    End If

                Else

                    ' Modifica lotto (anagrafica, lotto-consultorio, eventuale inserimento se lotto non in cns, eventuale disattivazione del lotto 
                    ' modifica età se flag true) e scrittura del log (tranne che in caso di errore)
                    bizLottiResult = bizLotti.UpdateLottoMagazzino(lottoMagazzino, OnVacUtility.Variabili.CNS.Codice, OnVacUtility.Variabili.CNSMagazzino.Codice,
                                                                   disattivaLotto, Me.Settings.ASSOCIA_LOTTI_ETA)

                    resultMessage = bizLottiResult.Message

                End If

            End Using
        End Using

        If Not String.IsNullOrEmpty(resultMessage) Then
            Me.OnitLayout31.InsertRoutineJS(String.Format("alert(""{0}"");", resultMessage))
        End If

        Return (bizLottiResult.Result <> Biz.BizLotti.BizLottiResult.ResultType.GenericError)

    End Function

    Private Function InserisciMovimentoLotto() As Boolean

        Dim inserisciMovimentoResult As Biz.BizLotti.BizLottiResult

        ' Dati del lotto corrente
        Dim lottoMagazzino As Entities.LottoMagazzino = Me.ucDatiLotto.GetLottoMagazzino()

        ' Riga del datagrid con i dati inseriti dall'utente
        Dim datagridEditItem As DataGridItem = Me.dgrMovimenti.Items(Me.dgrMovimenti.EditItemIndex)

        ' Movimento che verrà inserito
        Dim movimentoLotto As New Entities.MovimentoLotto()

        ' Numero dosi
        Dim dosi As String = DirectCast(datagridEditItem.FindControl("txtQuantita"), TextBox).Text.Trim()

        If String.IsNullOrEmpty(dosi) Then
            ShowMessageAndSetLayoutMovimenti(datagridEditItem, "Salvataggio non effettuato: la 'Quantità' deve essere specificata.")
            Return False
        End If

        Try
            movimentoLotto.NumeroDosi = Convert.ToInt32(dosi)
        Catch ex As Exception
            ShowMessageAndSetLayoutMovimenti(datagridEditItem, "Salvataggio non effettuato: il valore di 'Quantità' non è un numero!")
            Return False
        End Try

        ' Flag Dosi/Scatola
        Dim rbtScatole As RadioButton = DirectCast(datagridEditItem.FindControl("rbtScatole"), RadioButton)

        ' Tipo movimento, calcolo dosi rimaste e luogo di trasferimento
        movimentoLotto.TipoMovimento = DirectCast(datagridEditItem.FindControl("ddlTipoMovimento"), DropDownList).SelectedValue

        Select Case movimentoLotto.TipoMovimento

            Case Constants.TipoMovimentoMagazzino.Carico, Constants.TipoMovimentoMagazzino.Scarico

                movimentoLotto.CodiceConsultorioTrasferimento = String.Empty
                movimentoLotto.DescrizioneConsultorio = String.Empty

            Case Constants.TipoMovimentoMagazzino.TrasferimentoA

                Dim fmLuogoTrasferimento As Onit.Controls.OnitModalList = DirectCast(datagridEditItem.FindControl("fmLuogoTrasferimento"), Onit.Controls.OnitModalList)

                movimentoLotto.CodiceConsultorioTrasferimento = fmLuogoTrasferimento.Codice
                movimentoLotto.DescrizioneConsultorio = fmLuogoTrasferimento.Descrizione

        End Select

        ' Impostazione dati del movimento
        movimentoLotto.DataRegistrazione = Me.GetDateTimeFromString(DirectCast(datagridEditItem.FindControl("txtDataRegistrazione"), TextBox).Text)
        movimentoLotto.Note = DirectCast(datagridEditItem.FindControl("txtNote"), TextBox).Text
        movimentoLotto.CodiceLotto = Me.CodiceLottoSelezionato
        movimentoLotto.CodiceConsultorio = OnVacUtility.Variabili.CNSMagazzino.Codice
        movimentoLotto.IdUtente = OnVacContext.UserId

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizLotti As New Biz.BizLotti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                inserisciMovimentoResult =
                    bizLotti.InserisciMovimentoMagazzino(movimentoLotto, OnVacUtility.Variabili.CNS.Codice, OnVacUtility.Variabili.CNSMagazzino.Codice, rbtScatole.Checked)

            End Using
        End Using

        If inserisciMovimentoResult.Result <> Biz.BizLotti.BizLottiResult.ResultType.Success Then
            ShowMessageAndSetLayoutMovimenti(datagridEditItem, inserisciMovimentoResult.Message)
        End If

        Return (inserisciMovimentoResult.Result <> Biz.BizLotti.BizLottiResult.ResultType.GenericError)

    End Function

    Private Sub ShowMessageAndSetLayoutMovimenti(datagridEditItem As DataGridItem, errorMessage As String)

        If Not String.IsNullOrWhiteSpace(errorMessage) Then
            Me.OnitLayout31.InsertRoutineJS(String.Format("alert(""{0}"");", errorMessage))
        End If

        RegisterScriptHideModaleTrasferimento(datagridEditItem)
        RegisterScriptFocusQuantita()

    End Sub

#End Region

#Region " Eventi Datagrid Movimenti "

    Private Sub dgrMovimenti_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrMovimenti.ItemDataBound

        Select Case e.Item.ItemType

            Case ListItemType.EditItem

                ' Entità associata alla riga corrente
                Dim movimentoLotto As Entities.MovimentoLotto = Me.GetMovimentoFromDatagridItem(e.Item)
                If movimentoLotto Is Nothing Then Exit Sub

                ' Data registrazione edit
                Dim ctrlDataRegistrazione As Control = e.Item.FindControl("txtDataRegistrazione")

                If Not ctrlDataRegistrazione Is Nothing Then
                    DirectCast(ctrlDataRegistrazione, TextBox).Text = movimentoLotto.DataRegistrazione.ToString("dd/MM/yyyy HH:mm:ss")
                End If

                ' Luogo trasferimento edit
                Dim ctrlLuogoTrasferimento As Control = e.Item.FindControl("fmLuogoTrasferimento")

                If Not ctrlLuogoTrasferimento Is Nothing Then

					DirectCast(ctrlLuogoTrasferimento, Onit.Controls.OnitModalList).Filtro =
						String.Format("cns_codice <> '{0}' and (cns_cns_magazzino is null or cns_cns_magazzino = cns_codice) and cns_data_chiusura is null and cns_codice = LUC_CNS_CODICE and LUC_UTE_ID = {1} ",
									  OnVacUtility.Variabili.CNSMagazzino.Codice.Replace("'", "''"), OnVacContext.UserId)
				End If

                ' Script per richiamare la funzione javascript che mostra/nasconde la modale con il consultorio di trasferimento
                Me.RegisterScriptHideModaleTrasferimento(e.Item)

                ' Utente edit
                Me.SetDatagridItemModalList(e.Item, "fmOperatoreEdit", movimentoLotto.IdUtente)

            Case ListItemType.Item, ListItemType.AlternatingItem, ListItemType.SelectedItem

                ' Entità associata alla riga corrente
                Dim movimentoLotto As Entities.MovimentoLotto = Me.GetMovimentoFromDatagridItem(e.Item)
                If movimentoLotto Is Nothing Then Exit Sub

                ' Data registrazione
                Dim ctrlDataRegistrazione As Control = e.Item.FindControl("lblDataRegistrazione")

                If Not ctrlDataRegistrazione Is Nothing Then

                    Dim lblDataRegistrazione As Label = DirectCast(ctrlDataRegistrazione, Label)

                    If movimentoLotto.DataRegistrazione = Date.MinValue Then
                        lblDataRegistrazione.Text = String.Empty
                    Else
                        lblDataRegistrazione.Text = movimentoLotto.DataRegistrazione.ToString("dd/MM/yyyy HH:mm:ss")
                    End If

                End If

                ' Quantità
                Me.SetDatagridItemLabel(e.Item, "lblQuantita", movimentoLotto.NumeroDosi.ToString())

                ' Luogo trasferimento
                Me.SetDatagridItemLabel(e.Item, "lblLuogoTrasferimento", movimentoLotto.DescrizioneConsultorio)

                ' Utente
                Me.SetDatagridItemModalList(e.Item, "fmOperatore", movimentoLotto.IdUtente)

                ' Tipo Movimento
                Dim ctrlTipoMovimento As Control = e.Item.FindControl("lblTipoMovimento")

                If Not ctrlTipoMovimento Is Nothing Then

                    Dim lblTipoMovimento As Label = DirectCast(ctrlTipoMovimento, Label)

                    Select Case movimentoLotto.TipoMovimento

                        Case Constants.TipoMovimentoMagazzino.Carico
                            lblTipoMovimento.Text = "CARICO"

                        Case Constants.TipoMovimentoMagazzino.Scarico
                            lblTipoMovimento.Text = "SCARICO"

                        Case Constants.TipoMovimentoMagazzino.TrasferimentoA
                            lblTipoMovimento.Text = "TRASFERIMENTO A"

                        Case Else
                            lblTipoMovimento.Text = "TRASFERIMENTO DA"

                    End Select

                End If

                ' Note
                Me.SetDatagridItemLabel(e.Item, "lblNote", movimentoLotto.Note)

        End Select

    End Sub

    Private Sub dgrMovimenti_PageIndexChanged(source As Object, e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dgrMovimenti.PageIndexChanged

        If Me.dgrMovimenti.EditItemIndex = -1 Then
            Me.CaricamentoMovimentiLotto(Me.CodiceLottoSelezionato, e.NewPageIndex)
        End If

    End Sub

#End Region

#Region " Private Methods "

    Private Function GetMovimentoFromDatagridItem(datagridItem As DataGridItem) As Entities.MovimentoLotto

        If datagridItem.DataItem Is Nothing Then Return Nothing

        Dim movimentoLotto As Entities.MovimentoLotto = Nothing

        Try
            movimentoLotto = DirectCast(datagridItem.DataItem, Entities.MovimentoLotto)
        Catch ex As Exception
            movimentoLotto = Nothing
        End Try

        Return movimentoLotto

    End Function

    Private Sub SetDatagridItemLabel(datagridItem As DataGridItem, id As String, value As String)

        Dim ctrl As Control = datagridItem.FindControl(id)

        If Not ctrl Is Nothing Then
            DirectCast(ctrl, Label).Text = value
        End If

    End Sub

    Private Sub SetDatagridItemModalList(datagridItem As DataGridItem, id As String, codice As String)

        Dim ctrl As Control = datagridItem.FindControl(id)

        If Not ctrl Is Nothing Then

            Dim fm As Onit.Controls.OnitModalList = DirectCast(ctrl, Onit.Controls.OnitModalList)
            fm.Codice = codice
            fm.RefreshDataBind()

        End If

    End Sub

    Private Function GetDateTimeFromString(value As String) As DateTime

        Dim dataScadenza As DateTime

        If String.IsNullOrEmpty(value) Then
            dataScadenza = Date.MinValue
        Else
            Try
                dataScadenza = Convert.ToDateTime(value)
            Catch ex As Exception
                dataScadenza = Date.MinValue
            End Try
        End If

        Return dataScadenza

    End Function

    ' Richiesta di conferma del sequestro del lotto:
    ' deve avvenire solo se il lotto originale non era sequestrato e il lotto modificato è sequestrato,
    ' oppure se il lotto è nuovo (appena inserito).
    Private Function ConfermaSequestro() As Boolean

        Dim lottoMagazzino As Entities.LottoMagazzino = Me.ucDatiLotto.GetLottoMagazzino()

        ' Se tra i dati correnti del lotto, il check del sequestro non è stato spuntato, 
        ' non deve chiedere conferma del sequestro
        If Not lottoMagazzino.Obsoleto Then Return False

        ' Caricamento dati originali del lotto
        Dim lottoMagazzinoOriginale As Entities.LottoMagazzino = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizLotti As New Biz.BizLotti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                lottoMagazzinoOriginale = bizLotti.LoadLottoMagazzino(OnVacUtility.Variabili.CNS.Codice, OnVacUtility.Variabili.CNSMagazzino.Codice, lottoMagazzino.CodiceLotto)

            End Using
        End Using

        If lottoMagazzinoOriginale Is Nothing Then

            ' In caso di nuovo lotto chiedo sempre la conferma del sequestro 
            Return True

        Else

            ' In caso di lotto già esistente, chiedo la conferma del sequestro se l'originale su db non era già sequestrato
            If Not lottoMagazzinoOriginale.Obsoleto Then Return True

        End If

        Return False

    End Function

#End Region

End Class
