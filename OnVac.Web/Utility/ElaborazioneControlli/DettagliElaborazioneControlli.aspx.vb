Imports Infragistics.WebUI.UltraWebToolbar

Public Class DettagliElaborazioneControlli
    Inherits Common.PageBase

#Region "Type"
    Private Enum DgrColumnIndex
        IconaStatoElaborazione = 0
        Paziente = 1
        Sesso = 2
        DataNascita = 3
        CodiceFiscale = 4
        EsitoControllo = 5
        Errore = 6
        PazCodice = 7
        CocId = 8
        Stato = 9
    End Enum
#End Region
#Region " Stati Elaborazione "

    <Serializable>
    Protected Class InfoStatoElaborazione

        Public StatoElaborazione As Integer
        Public UrlIcona As String
        Public Descrizione As String
        Public ToolTip As String

        Public Sub New(statoElaborazione As Integer, urlIcona As String, descrizione As String, toolTip As String)
            Me.StatoElaborazione = statoElaborazione
            Me.UrlIcona = urlIcona
            Me.Descrizione = descrizione
            Me.ToolTip = toolTip
        End Sub

    End Class
    Private ReadOnly Property CodiceCaricamento() As String
        Get
            Dim codice As String = Me.Request.QueryString.Get("codice")

            If String.IsNullOrWhiteSpace(codice) Then Throw New NullReferenceException("Codice Caricamento del dettaglio non specificato")

            Return codice
        End Get
    End Property

    Private ReadOnly Property Pageindex() As String
        Get
            Dim page As String = Me.Request.QueryString.Get("page")

            If String.IsNullOrWhiteSpace(page) Then Throw New NullReferenceException("Pagina non specifica!")

            Return page
        End Get
    End Property
    'Private ReadOnly Property DaDataFilter() As String
    '    Get
    '        Dim daData As String = Me.Request.QueryString.Get("dadata")

    '        If String.IsNullOrWhiteSpace(daData) Then Throw New NullReferenceException("Data non specificata!")

    '        Return daData
    '    End Get
    'End Property
    'Private ReadOnly Property ADataFilter() As String
    '    Get
    '        Dim aData As String = Me.Request.QueryString.Get("adata")

    '        If String.IsNullOrWhiteSpace(aData) Then Throw New NullReferenceException("Data non specificata!")

    '        Return aData
    '    End Get
    'End Property
    'Private ReadOnly Property IdProcFilter() As String
    '    Get
    '        Dim idProc As String = Me.Request.QueryString.Get("idProc")

    '        If String.IsNullOrWhiteSpace(idProc) Then Throw New NullReferenceException("Processo non valorizzato!")

    '        Return idProc
    '    End Get
    'End Property

    Protected ReadOnly Property StatoInRegola As InfoStatoElaborazione
        Get
            Dim estrattiCorrettamente As Integer = DirectCast(Biz.BizElaborazioneControlli.StatoEstrazioneControllo.InRegola, Integer)
            Return New InfoStatoElaborazione(estrattiCorrettamente, ResolveClientUrl("~/images/consensoPositivo.png"), "In regola", "Paziente in regola")
        End Get
    End Property

    Protected ReadOnly Property StatoParzialmenteInRegola As InfoStatoElaborazione
        Get
            Dim estrazioneInCorso As Integer = DirectCast(Biz.BizElaborazioneControlli.StatoEstrazioneControllo.ParzialmenteInRegola, Integer)
            Return New InfoStatoElaborazione(estrazioneInCorso, ResolveClientUrl("~/images/consensoAltro.png"), "Parzialmente in regola", "Paziente parzialmente in regola")
        End Get
    End Property

    Protected ReadOnly Property StatoElabConErrore As InfoStatoElaborazione
        Get
            Dim estrattiConWarning As Integer = DirectCast(Biz.BizElaborazioneControlli.StatoEstrazioneControllo.ElaboratoConErrore, Integer)
            Return New InfoStatoElaborazione(estrattiConWarning, ResolveClientUrl("~/images/error.png"), "Estrazione con errore", "Estrazione con errore")
        End Get
    End Property

    Protected ReadOnly Property StatoNonInRegola As InfoStatoElaborazione
        Get
            Dim estrattiConErrore As Integer = DirectCast(Biz.BizElaborazioneControlli.StatoEstrazioneControllo.NonInRegola, Integer)
            Return New InfoStatoElaborazione(estrattiConErrore, ResolveClientUrl("~/images/consensoNegativo.png"), "Non in regola", "Paziente non in regola")
        End Get
    End Property
    Protected ReadOnly Property UrlIconaTrasparente As String
        Get
            Return ResolveClientUrl("~/images/transparent16.gif")
        End Get
    End Property
    Private Property IdControllo As Integer
        Get
            Return ViewState("IdControllo")
        End Get
        Set(value As Integer)
            ViewState("IdControllo") = value
        End Set
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
#Region "Eventi"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            cmbStatoElaborazione.Items.Clear()
            cmbStatoElaborazione.Items.Add(New ListItem(String.Empty, String.Empty))
            cmbStatoElaborazione.Items.Add(New ListItem(Me.StatoElabConErrore.Descrizione, Me.StatoElabConErrore.StatoElaborazione))
            cmbStatoElaborazione.Items.Add(New ListItem(Me.StatoInRegola.Descrizione, Me.StatoInRegola.StatoElaborazione))
            cmbStatoElaborazione.Items.Add(New ListItem(Me.StatoParzialmenteInRegola.Descrizione, Me.StatoParzialmenteInRegola.StatoElaborazione))
            cmbStatoElaborazione.Items.Add(New ListItem(Me.StatoNonInRegola.Descrizione, Me.StatoNonInRegola.StatoElaborazione))

            cmbStatoElaborazione.SelectedValue = String.Empty
            ' Legenda
            lblStatoElabConErrore.Text = StatoElabConErrore.Descrizione
            lblStatoElabConErrore.ToolTip = StatoElabConErrore.ToolTip

            lblStatoParzialmenteInRegola.Text = StatoParzialmenteInRegola.Descrizione
            lblStatoParzialmenteInRegola.ToolTip = StatoParzialmenteInRegola.ToolTip

            lblStatoInRegola.Text = StatoInRegola.Descrizione
            lblStatoInRegola.ToolTip = StatoInRegola.ToolTip

            lblStatoNonInRegola.Text = StatoNonInRegola.Descrizione
            lblStatoNonInRegola.ToolTip = StatoNonInRegola.ToolTip
            'Dati della testata
            BindDatiProcessoCorrente()
            RicercaEsito(0)
        End If

    End Sub
#Region "Griglia"
    Private Sub dgrElaborazioneControlli_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrElaborazioneControlli.ItemDataBound

        Select Case e.Item.ItemType
            Case ListItemType.Item, ListItemType.AlternatingItem, ListItemType.EditItem, ListItemType.SelectedItem

                Dim dettaglio As Entities.EsitoControlloScuole = DirectCast(e.Item.DataItem, Entities.EsitoControlloScuole)

                ' Flag Stato Elaborazione
                Dim imgStatoElaborazione As System.Web.UI.WebControls.Image = DirectCast(e.Item.FindControl("imgStatoElaborazione"), System.Web.UI.WebControls.Image)
                If Not imgStatoElaborazione Is Nothing Then

                    If Not String.IsNullOrWhiteSpace(dettaglio.Stato) Then

                        Select Case dettaglio.Stato

                            Case "S"
                                imgStatoElaborazione.ImageUrl = StatoInRegola.UrlIcona
                                imgStatoElaborazione.ToolTip = StatoInRegola.ToolTip

                            Case "E"
                                imgStatoElaborazione.ImageUrl = StatoElabConErrore.UrlIcona
                                imgStatoElaborazione.ToolTip = StatoElabConErrore.ToolTip

                            Case "N"
                                imgStatoElaborazione.ImageUrl = StatoNonInRegola.UrlIcona
                                imgStatoElaborazione.ToolTip = StatoNonInRegola.ToolTip

                            Case "P"
                                imgStatoElaborazione.ImageUrl = StatoParzialmenteInRegola.UrlIcona
                                imgStatoElaborazione.ToolTip = StatoParzialmenteInRegola.ToolTip

                        End Select

                    Else
                        imgStatoElaborazione.ImageUrl = Me.UrlIconaTrasparente
                        imgStatoElaborazione.ToolTip = String.Empty
                    End If

                End If

        End Select

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

        RicercaEsito(dgrElaborazioneControlli.PageSize)

    End Sub

    Private Sub dgrLogNotifiche_PageIndexChanged(source As Object, e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dgrElaborazioneControlli.PageIndexChanged

        RicercaEsito(e.NewPageIndex)

    End Sub
#End Region

#End Region
#Region "Private"
    Private Sub BindDatiProcessoCorrente()
        Dim detTest As Entities.ElaborazioneControlli = Nothing
        Dim filtroTest As New Entities.FiltroSingolaElaborazioneControlli
        filtroTest.AppId = OnVacContext.AppId
        filtroTest.CodiceProcesso = New Guid(CodiceCaricamento)
        Try
            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizControlli As New Biz.BizElaborazioneControlli(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                    detTest = bizControlli.GetElaborazioneControlloXCodiceProc(filtroTest)
                End Using
            End Using
        Catch ex As Exception
            OnitLayout31.ShowMsgBox(New Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(ex.Message, "MessageControllo", False, False))
        End Try
        lblIdCaricamento.Text = detTest.IdCaricamento.ToString()
        lblFileCaricamento.Text = detTest.NomeFileOrigine
        lblDataControllo.Text = GetData(detTest.DataInizioControllo, True)
        lblCodiceUtente.Text = detTest.CodiceUtenteControllo
        lblStatoControllo.Text = detTest.StatoControllo
        If detTest.TotaleRecord.HasValue Then
            lblcaricati.Text = detTest.TotaleRecord.ToString()
        Else
            lblcaricati.Text = String.Empty
        End If
        If detTest.TotaleControllati.HasValue Then
            lblControllati.Text = detTest.TotaleControllati.ToString()
        Else
            lblControllati.Text = String.Empty
        End If
        If detTest.TotaleErrore.HasValue Then
            lblErr.Text = detTest.TotaleErrore.ToString()
        Else
            lblErr.Text = String.Empty
        End If
        If detTest.TotaleVaccinati.HasValue Then
            lblvaccinati.Text = detTest.TotaleVaccinati.ToString()
        Else
            lblvaccinati.Text = String.Empty
        End If
        If detTest.TotaleNonVaccinati.HasValue Then
            lblNonVaccinati.Text = detTest.TotaleNonVaccinati.ToString()
        Else
            lblNonVaccinati.Text = String.Empty
        End If
        If detTest.Copertura.HasValue Then
            lblCopertura.Text = String.Format("{0} %", detTest.Copertura.ToString())
        Else
            lblCopertura.Text = String.Empty
        End If
        If detTest.IdControllo.HasValue Then
            IdControllo = detTest.IdControllo
        Else
            IdControllo = 0
        End If


    End Sub
    Private Sub RicercaEsito(currentPageIndex As Int32)

        Dim Lista As New Biz.BizEsitoControlloScuole.GetListaEsitoControlloScuoleResult
        Try
            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizListaControlli As New Biz.BizEsitoControlloScuole(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                    Dim command As New Biz.BizEsitoControlloScuole.GetEsitoControlloScuoleCommand
                    command.Filtri = FiltroRicerca()
                    command.PageIndex = currentPageIndex
                    command.PageSize = dgrElaborazioneControlli.PageSize
                    'command.CampoOrdinamento = CampoOrdinamento

                    Lista = bizListaControlli.GetListaElaborazioneControlli(command)
                End Using
            End Using
        Catch ex As Exception
            OnitLayout31.ShowMsgBox(New Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(ex.Message, "MessageException", False, False))
            Return
        End Try

        dgrElaborazioneControlli.VirtualItemCount = Lista.CountEsitoControlli
        dgrElaborazioneControlli.CurrentPageIndex = currentPageIndex

        dgrElaborazioneControlli.SelectedIndex = -1
        dgrElaborazioneControlli.DataSource = Lista.ListaEsitoControlli
        dgrElaborazioneControlli.DataBind()
    End Sub

    Private Function FiltroRicerca() As Entities.FiltroEsitoControlloScuole
        Dim filtroEsito As New Entities.FiltroEsitoControlloScuole
        filtroEsito.IdControllo = IdControllo.ToString()
        filtroEsito.CodiceCaricamento = New Guid(CodiceCaricamento)
        If Not String.IsNullOrWhiteSpace(cmbStatoElaborazione.Text) Then
            filtroEsito.StatoEsito = DecodStato(cmbStatoElaborazione.SelectedValue)
        End If
        Return filtroEsito
    End Function
    Private Function DecodStato(stato As String) As String
        Select Case stato
            Case Biz.BizElaborazioneControlli.StatoEstrazioneControllo.InRegola
                Return Constants.DecodeStatoEstrazioneControllo.InRegola
            Case Biz.BizElaborazioneControlli.StatoEstrazioneControllo.ElaboratoConErrore
                Return Constants.DecodeStatoEstrazioneControllo.ElaboratoConErrore
            Case Biz.BizElaborazioneControlli.StatoEstrazioneControllo.NonInRegola
                Return Constants.DecodeStatoEstrazioneControllo.NonInRegola
            Case Biz.BizElaborazioneControlli.StatoEstrazioneControllo.ParzialmenteInRegola
                Return Constants.DecodeStatoEstrazioneControllo.ParzialmenteInRegola
        End Select

        Return String.Empty

    End Function

    ''' <summary>
    ''' Esporta dati del processo 
    ''' </summary>
    ''' <param name="idProcessoControllo"></param>
    ''' <param name="rptName"></param>
    Private Sub EsportaExcel(idProcessoControllo As Integer, rptName As String)

        ' TODO [Veneto-Scuole]: EsportaExcel => fare i controlli commentati

        'If dgrElaborazioni Is Nothing OrElse dgrElaborazioni.Items.Count = 0 Then
        '    Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Nessun dato da esportare.", "MSG", False, False))
        '    Return
        'End If

        'Dim codiceProcedura As Integer = Me.CodiceProcedura
        'If codiceProcedura = -1 Then
        '    Me.OnitLayout31.InsertRoutineJS("alert('Nessun processo selezionato');")
        '    Return
        'End If


        ' Altri controlli => non si può esportare se il controllo non c’è o è in corso.



        Dim url As String = String.Format("{0}?appId={1}&codAzienda={2}&id={3}&format={4}&rpt={5}",
                                          ResolveClientUrl("~/Common/Handlers/ReportViewerExportHandler.ashx"),
                                          OnVacContext.AppId, OnVacContext.Azienda, idProcessoControllo.ToString(),
                                          Report.ReportViewer.TipoEsportazioneReport.XLS, rptName)

        Page.ClientScript.RegisterStartupScript(Me.GetType(), "xlsExportHandler",
                                                String.Format("<script type='text/javascript'> window.open('{0}'); </script>", url))

    End Sub
    Private Sub tlbElaborazione_ButtonClicked(sender As Object, be As ButtonEvent) Handles tlbElaborazione.ButtonClicked
        Select Case be.Button.Key
            Case "btnIndietro"
                Response.Redirect(Me.ResolveClientUrl(String.Format("~/Utility/ElaborazioneControlli/ElaborazioneControlli.aspx?codice={0}&page={1}", CodiceCaricamento.ToString(), Pageindex)), False)
            Case "btnExportExcelCentro"
                If IdControllo > 0 Then
                    EsportaExcel(IdControllo, Constants.ReportName.EstrazioneControlliCentri)
                Else
                    OnitLayout31.ShowMsgBox(New Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Controllo non effettuato", "Controllo stampa", False, False))
                End If

            Case "btnExportExcelScuola"
                If IdControllo > 0 Then
                    EsportaExcel(IdControllo, Constants.ReportName.EstrazioneControlliScuole)
                Else
                    OnitLayout31.ShowMsgBox(New Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Controllo non effettuato", "Controllo stampa", False, False))
                End If

            Case "btnCerca"
                RicercaEsito(dgrElaborazioneControlli.CurrentPageIndex)

        End Select
    End Sub
#End Region


#Region "Proteced"
    Protected Sub lnkCognomeNome_Click(sender As Object, e As EventArgs)

        Dim currentGridItem As DataGridItem = GetCurrentGridItem(sender, "lnkCognomeNome")
        If currentGridItem Is Nothing Then
            Return
        End If

        Dim id As Long? = GetCodicePazienteCorrente(currentGridItem)
        If Not id.HasValue Then
            Me.OnitLayout31.InsertRoutineJS("alert('Selezionare un paziente per visualizzarne i dati');")
            Return
        Else

            Dim codicePazienteSelezionato As String = id.ToString()

            ' Memorizzazione codice paziente per ricerca rapida (l'impostazione del paziente corrente avviene nel metodo RedirectToGestionePaziente)
            Me.UltimoPazienteSelezionato = New Entities.UltimoPazienteSelezionato(String.Empty, codicePazienteSelezionato)

            ' Redirect Dettaglio Paziente
            Me.RedirectToGestionePaziente(codicePazienteSelezionato)

        End If

    End Sub
    Private Function GetCurrentGridItem(sender As Object, controlId As String) As DataGridItem

        For Each item As DataGridItem In Me.dgrElaborazioneControlli.Items

            Dim control As Control = item.FindControl(controlId)
            If control Is sender Then
                Return item
            End If

        Next

        Return Nothing

    End Function
    Private Function GetCodicePazienteCorrente(currentGridItem As DataGridItem) As Long?

        Return GetNullableInt64FromObject(HttpUtility.HtmlDecode(currentGridItem.Cells(DgrColumnIndex.PazCodice).Text.Trim()))

    End Function
    Private Function GetNullableInt64FromObject(value As Object) As Int64?

        If String.IsNullOrWhiteSpace(value) Then Return Nothing

        Dim id As Long = 0
        If Not Int64.TryParse(value, id) Then
            Return Nothing
        End If

        Return id

    End Function

    Protected Function GetUrlIconaStato(risultato As String) As String
        Select Case risultato
            Case "S"
                Return "~/images/verifica_conferma.gif"
            Case "F"
                Return "~/images/verifica_annulla.gif"
            Case "R"
                Return "~/images/elabora.png"
            Case "W"
                Return "~/images/pausa.png"
        End Select

        Return String.Empty
    End Function

    Protected Function GetData(value As Object, includiOrario As Boolean) As String

        If value Is Nothing OrElse value Is DBNull.Value Then Return String.Empty

        Dim dateValue As DateTime
        If Not DateTime.TryParse(value, dateValue) Then
            Return String.Empty
        End If

        If includiOrario Then
            Return dateValue.ToString("dd/MM/yyyy HH.mm", Globalization.CultureInfo.InvariantCulture)
        End If

        Return dateValue.ToString("dd/MM/yyyy", Globalization.CultureInfo.InvariantCulture)

    End Function

    Protected Function GetErrore(value As Object) As String

        If value Is Nothing OrElse value Is DBNull.Value Then Return String.Empty

        Dim errore As String = value.ToString()

        If errore.Length > 20 Then
            errore = errore.Substring(0, 18) + "..."
        End If

        Return errore.ToString()

    End Function


#End Region

End Class