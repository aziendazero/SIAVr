Public Class ElaborazioneControlli
    Inherits Common.PageBase

#Region " Types "

    Private Enum DgrColumnIndex
        Selector = 0
        CodiceUtenteCaricamento = 1
        IdCaricamento = 2
        DataInizioCaricamento = 3
        NomeFileOrigine = 4
        StatoCaricamentoImmage = 5
        RigheElabScart = 6
        StatoControllo = 7
        TotaleRecord = 8
        TotaleControllati = 9
        TotaleErrore = 10
        IdControllo = 11
        CodiceCaricamento = 12
        StatoCaricamento = 13
        RigheElabDiff = 14
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

    Private Property IdElaborazione As Long
        Get
            Return ViewState("IdElaborazione")
        End Get
        Set(value As Long)
            ViewState("IdElaborazione") = value
        End Set
    End Property

    Private Property CodiceCaricamento As String
        Get
            Return ViewState("CodiceCaricamento")
        End Get
        Set(value As String)
            ViewState("CodiceCaricamento") = value
        End Set
    End Property

#End Region

#Region " Page "

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then

            ResetFilter()
            Dim codice As String = Request.QueryString.Get("codice")
            Dim page As String = Request.QueryString.Get("page")
            'If Not String.IsNullOrWhiteSpace(Request.QueryString.Get("daData")) Then
            '    odpDaDataCaricamentoFiltro.Text = Request.QueryString.Get("daData")
            'End If
            'If Not String.IsNullOrWhiteSpace(Request.QueryString.Get("aData")) Then
            '    odpADataCaricamentoFiltro.Text = Request.QueryString.Get("aData")
            'End If
            'If Not String.IsNullOrWhiteSpace(Request.QueryString.Get("idProc")) Then
            '    txtIdProcessoFiltro.Text = Request.QueryString.Get("idProc")
            'End If


            Dim intPage As Integer = 0
            If Not String.IsNullOrWhiteSpace(page) Then
                intPage = Convert.ToInt64(page)
            End If
            RicercaElaborazioni(intPage, codice)
        End If

    End Sub

#End Region

#Region " Toolbar Events "
    Private Sub tlbElaborazione_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles tlbElaborazione.ButtonClicked

        Select Case e.Button.Key

            Case "btnCerca"
                RicercaElaborazioni(dgrElaborazioneControlli.CurrentPageIndex, "")
            Case "btnAvvia"
                AvvioControlli()
                RicercaElaborazioni(dgrElaborazioneControlli.CurrentPageIndex, "")
            Case "btnDettagli"
                ApriDettagli()

        End Select
    End Sub
#End Region

#Region "Private"

    Private Sub ResetFilter()
        omlUtenteFiltro.Descrizione = String.Empty
        omlUtenteFiltro.Codice = String.Empty
        odpDaDataCaricamentoFiltro.Text = String.Empty
        odpADataCaricamentoFiltro.Text = String.Empty
        txtIdProcessoFiltro.Text = String.Empty
    End Sub

    Private Sub RicercaElaborazioni(currentPageIndex As Int32, codice As String)

        Dim filtro As Entities.FiltroElaborazioneControlli = FiltriRicerca()
        Dim lista As New Biz.BizElaborazioneControlli.GetListaElaborazioneControlliResult()

        Try
            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizListaControlli As New Biz.BizElaborazioneControlli(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                    Dim command As New Biz.BizElaborazioneControlli.GetElaborazioneControlliCommand
                    command.Filtri = filtro
                    command.PageIndex = currentPageIndex
                    command.PageSize = dgrElaborazioneControlli.PageSize
                    'command.CampoOrdinamento = CampoOrdinamento

                    lista = bizListaControlli.GetListaElaborazioneControlli(command)

                End Using
            End Using

        Catch ex As Exception
            OnitLayout31.ShowMsgBox(New Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(ex.Message, "MessageException", False, False))
            Return
        End Try

        BindingElencoProcessi(lista, codice, currentPageIndex)

    End Sub

    Public Function FiltriRicerca() As Entities.FiltroElaborazioneControlli

        Dim filtro As New Entities.FiltroElaborazioneControlli()

        If Not String.IsNullOrWhiteSpace(omlUtenteFiltro.Codice) AndAlso Not String.IsNullOrWhiteSpace(omlUtenteFiltro.Descrizione) Then
            filtro.IdUtente = Convert.ToInt32(omlUtenteFiltro.ValoriAltriCampi("ID"))
        End If

        If Not String.IsNullOrWhiteSpace(odpDaDataCaricamentoFiltro.Text) Then
            filtro.DaData = odpDaDataCaricamentoFiltro.Data
        End If

        If Not String.IsNullOrWhiteSpace(odpADataCaricamentoFiltro.Text) Then
            filtro.AData = odpADataCaricamentoFiltro.Data
        End If

        If Not String.IsNullOrWhiteSpace(txtIdProcessoFiltro.Text) Then
            filtro.IdProcesso = Convert.ToInt32(txtIdProcessoFiltro.Text)
        End If

        filtro.AppId = OnVacContext.AppId

        Return filtro

    End Function

    Private Sub BindingElencoProcessi(lista As Biz.BizElaborazioneControlli.GetListaElaborazioneControlliResult, codice As String, currentPageIndex As Integer)

        dgrElaborazioneControlli.VirtualItemCount = lista.CountElaborazioneControlli
        dgrElaborazioneControlli.CurrentPageIndex = currentPageIndex

        dgrElaborazioneControlli.SelectedIndex = -1
        dgrElaborazioneControlli.DataSource = lista.ListaElaborazioneControlli
        dgrElaborazioneControlli.DataBind()

        If Not codice.IsNullOrEmpty() Then
            For i As Integer = 0 To dgrElaborazioneControlli.Items.Count - 1
                Dim codiceDgr As String = HttpUtility.HtmlDecode(dgrElaborazioneControlli.Items(i).Cells(DgrColumnIndex.CodiceCaricamento).Text.Trim())
                If Not String.IsNullOrWhiteSpace(codiceDgr) Then
                    If codice = codiceDgr Then
                        dgrElaborazioneControlli.SelectedIndex = i
                        Exit For
                    End If
                End If
            Next
        Else
            If lista.ListaElaborazioneControlli.Count > 0 Then
                dgrElaborazioneControlli.SelectedIndex = 0
            End If
        End If

    End Sub

    Private Sub dgrElaborazioneControlli_SortCommand(source As Object, e As DataGridSortCommandEventArgs) Handles dgrElaborazioneControlli.SortCommand

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

        RicercaElaborazioni(dgrElaborazioneControlli.PageSize, "")

    End Sub

    Private Sub dgrLogNotifiche_PageIndexChanged(source As Object, e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dgrElaborazioneControlli.PageIndexChanged

        RicercaElaborazioni(e.NewPageIndex, "")

    End Sub

    Private Sub dgrLogNotifiche_SelectedIndexChanged(sender As Object, e As EventArgs) Handles dgrElaborazioneControlli.SelectedIndexChanged

        'mainView.ActiveViewIndex = ViewIndex.ViewDettaglio
        IdElaborazione = GetIdNotificaFromDataGridItem(dgrElaborazioneControlli.SelectedItem)
        CodiceCaricamento = GetCodiceCaricamentoFromDataGridItem(dgrElaborazioneControlli.SelectedItem)
        'CaricaDettaglioNotifiche(IdElaborazione)

    End Sub

    Private Function GetIdNotificaFromDataGridItem(item As DataGridItem) As Long

        If item Is Nothing Then Return -1

        Return Convert.ToInt64(HttpUtility.HtmlDecode(item.Cells(DgrColumnIndex.IdCaricamento).Text))

    End Function

    Private Function GetCodiceCaricamentoFromDataGridItem(item As DataGridItem) As String

        If item Is Nothing Then Return -1

        Return HttpUtility.HtmlDecode(item.Cells(DgrColumnIndex.CodiceCaricamento).Text)

    End Function

    Private Sub AvvioControlli()

        If dgrElaborazioneControlli.SelectedIndex <> -1 Then

            Dim item As DataGridItem = dgrElaborazioneControlli.SelectedItem

            If HttpUtility.HtmlDecode(item.Cells(DgrColumnIndex.StatoCaricamento).Text) = "S" AndAlso HttpUtility.HtmlDecode(item.Cells(DgrColumnIndex.IdControllo).Text) = "0" Then

                ' Inserimento testata e esecuzione procedura controlli
                Try
                    Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                        Using bizElaborazioneControlli As New Biz.BizElaborazioneControlli(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                            Dim datiInsert As New Entities.InsertControlliTesta()
                            datiInsert.CodiceCaricamento = New Guid(HttpUtility.HtmlDecode(item.Cells(DgrColumnIndex.CodiceCaricamento).Text))
                            datiInsert.IdCaricamento = Convert.ToInt64(HttpUtility.HtmlDecode(item.Cells(DgrColumnIndex.IdCaricamento).Text))
                            datiInsert.IdUtenteControllo = OnVacContext.UserId
                            datiInsert.StatoControllo = "W"
                            datiInsert.TotaleRecord = Convert.ToInt64(HttpUtility.HtmlDecode(item.Cells(DgrColumnIndex.RigheElabDiff).Text))

                            bizElaborazioneControlli.EseguiControlli(datiInsert)

                        End Using

                    End Using

                Catch ex As Exception
                    OnitLayout31.ShowMsgBox(New Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(ex.Message, "MessageControllo", False, False))
                End Try

            Else
                OnitLayout31.ShowMsgBox(New Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Dati non coerenti per elaborare i controlli!", "MessageControllo", False, False))
            End If

        Else
            OnitLayout31.ShowMsgBox(New Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Riga non selezionata!", "MessageRiga", False, False))
        End If

    End Sub

    Private Sub ApriDettagli()

        If dgrElaborazioneControlli.SelectedItem Is Nothing AndAlso String.IsNullOrWhiteSpace(CodiceCaricamento) Then

            OnitLayout31.InsertRoutineJS("alert('Selezionare un processo per visualizzarne i dettagli.');")
            Return

        Else

            CodiceCaricamento = GetCodiceCaricamentoFromDataGridItem(dgrElaborazioneControlli.SelectedItem)
            Dim filtroDaData As String = odpDaDataCaricamentoFiltro.Text
            Dim filtroAData As String = odpADataCaricamentoFiltro.Text
            Dim filtroIdProc As String = txtIdProcessoFiltro.Text

            If Not String.IsNullOrWhiteSpace(CodiceCaricamento) Then
                Response.Redirect(ResolveClientUrl(String.Format("~/Utility/ElaborazioneControlli/DettagliElaborazioneControlli.aspx?codice={0}&page={1}", CodiceCaricamento, dgrElaborazioneControlli.CurrentPageIndex.ToString())), False)
            End If

        End If

    End Sub

#End Region

#Region "Protected"

    Protected Function ConvertToDateString(value As Object) As String

        If value IsNot Nothing Then

            Dim dateValue As DateTime = DirectCast(value, DateTime)

            If dateValue > DateTime.MinValue Then
                Return dateValue.ToString("dd/MM/yyyy HH:mm")
            End If

        End If

        Return String.Empty

    End Function

#End Region

End Class