Imports System.Collections.Generic
Imports System.Linq

Imports Onit.Shared.Manager.OnitProfile
Imports Onit.Database.DataAccessManager

Imports Onit.OnAssistnet.OnVac.DAL


Partial Class PazientiInIngresso
    Inherits OnVac.Common.OnVacMovimentiPageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(sender As System.Object, e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Properties "

    Private Property FiltriRicerca() As MovimentiCNSPazientiInIngressoFilter
        Get
            Return Session("MovimentiCNSPazientiInIngressoFilter")
        End Get
        Set(value As MovimentiCNSPazientiInIngressoFilter)
            Session("MovimentiCNSPazientiInIngressoFilter") = value
        End Set
    End Property

    Protected Overrides ReadOnly Property OnitLayout As Controls.PagesLayout.OnitLayout3
        Get
            Return Me.OnitLayout31
        End Get
    End Property

    Private Property CampoOrdinamento As String
        Get
            If ViewState("CampoOrd") Is Nothing Then ViewState("CampoOrd") = Me.dgrPazienti.Columns(dgrPazientiColumnIndex.Cognome).SortExpression
            Return ViewState("CampoOrd")
        End Get
        Set(value As String)
            ViewState("CampoOrd") = value
        End Set
    End Property

    Private Property VersoOrdinamento As String
        Get
            If ViewState("VersoOrd") Is Nothing Then ViewState("VersoOrd") = "ASC"
            Return ViewState("VersoOrd")
        End Get
        Set(value As String)
            ViewState("VersoOrd") = value
        End Set
    End Property

#End Region

#Region " Overrides "

    Protected Overrides Sub OnInit(e As System.EventArgs)
        '--
        MyBase.OnInit(e)
        '--
    End Sub

#End Region

#Region " Enum "

    Private Enum dgrPazientiColumnIndex
        CodicePaziente = 0
        StatoAnagrafico = 1
        StatoAcquisizione = 2
        BtnEditConfermaAnnulla = 3
        BtnRedirectPaziente = 4
        Cognome = 5
        Nome = 6
        DataDiNascita = 7
        Residenza = 8
        Domicilio = 9
        Immigrazione = 10
        Assistenza = 11
        Provenienza = 12
        StatoAnagraficoDescrizione = 13
    End Enum

#End Region

#Region " Events Handlers "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            Me.SetColumnHeaders()

            Me.ShowPrintButtons()

            Me.uscStatiAnagrafici.ListStatiAnagraficiDaSelezionare = Me.Settings.STATIANAG_MOVCV_PAZ_INTERNI

            Me.SetFiltriRicerca()

        End If

    End Sub

    Private Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender

        Me.SetImmagineOrdinamento()

    End Sub

    Private Sub tlbMovimenti_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles tlbMovimenti.ButtonClicked

        Select Case be.Button.Key

            Case "btnCerca"
                Me.CampoOrdinamento = Nothing
                Me.VersoOrdinamento = Nothing
                Me.CaricaDati(0)

            Case "btnPulisci"
                Me.ResetFilters()

            Case "btnStampaElenco"
                Me.StampaElenco(Constants.ReportName.ElencoImmigrati)

            Case "btnStampaElencoPerComune"
                Me.StampaElenco(Constants.ReportName.ElencoImmigratiComune)

        End Select

    End Sub

    Private Sub btnStampaEtichette_Click(sender As Object, e As System.EventArgs) Handles btnStampaEtichette.Click

        Me.StampaEtichette()

    End Sub

#Region " Datagrid "

    Private Sub dgrPazienti_PageIndexChanged(source As Object, e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dgrPazienti.PageIndexChanged

        Me.CaricaDati(e.NewPageIndex)

    End Sub

    Private Sub dgrPazienti_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrPazienti.ItemDataBound
        '--
        Select Case e.Item.ItemType
            '--
            Case ListItemType.Item, ListItemType.AlternatingItem, ListItemType.EditItem, ListItemType.SelectedItem
                '--
                If Me.StatoPagina = StatoPaginaMovimenti.Modifica Then
                    '--
                    Me.DataGridItemDataBound(e.Item, Me.dgrPazienti.EditItemIndex)
                    '--
                Else
                    '--
                    Me.AddConfirmClickToImageButton(e.Item, "btnRegPaz", "Il paziente e' stato regolarizzato?")
                    '--
                End If
                '--
                Me.SetEditButtonVisibility(e.Item)
                '--
        End Select
        '--
    End Sub

    Private Sub dgrPazienti_ItemCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgrPazienti.ItemCommand

        Select Case e.CommandName

            Case "EditRowMovimenti"

                Me.dgrPazientiEditRowMovimenti(e)

            Case "CancelRowMovimenti"

                Me.dgrPazientiCancelRowMovimenti(e)

            Case "UpdateRowMovimenti"

                Me.dgrPazientiUpdateRowMovimenti(e)

            Case "DatiPaziente"

                Me.RedirectToGestionePaziente(e.Item.Cells(dgrPazientiColumnIndex.CodicePaziente).Text)

            Case "RegPaz"

                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                    Using bizPaziente As Biz.BizPaziente = Biz.BizFactory.Instance.CreateBizPaziente(genericProvider, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.PAZIENTI))

                        bizPaziente.UpdateFlagRegolarizzato(e.Item.Cells(dgrPazientiColumnIndex.CodicePaziente).Text, True)

                    End Using
                End Using

                Me.CaricaDati(0)

            Case "Acquisisci"

                Me.CodicePazienteSelezionato = e.Item.Cells(dgrPazientiColumnIndex.CodicePaziente).Text

                Dim command As New Common.OnVacStoricoVaccinaleCentralizzato.AcquisisciStoricoCommand()
                command.CodicePaziente = Me.CodicePazienteSelezionato
                command.RichiediConfermaSovrascrittura = True
                command.Settings = Me.Settings
                command.OnitLayout3 = Me.OnitLayout31
                command.BizLogOptions = Nothing
                command.Note = "Recupero Storico Vaccinale da maschera PazientiInIngresso"

                Dim acquisisciDatiVaccinaliCentraliResult As Biz.BizPaziente.AcquisisciDatiVaccinaliCentraliResult =
                    Common.OnVacStoricoVaccinaleCentralizzato.AcquisisciDatiVaccinaliCentraliPaziente(command)

                ' TODO: log???

                If acquisisciDatiVaccinaliCentraliResult.StatoAcquisizioneDatiVaccinaliCentrale.HasValue AndAlso
                   acquisisciDatiVaccinaliCentraliResult.StatoAcquisizioneDatiVaccinaliCentrale.Value <> Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Parziale Then

                    e.Item.FindControl("btnAcquisizione").Visible = False
                    e.Item.FindControl("lblAcquisizione").Visible = True

                End If

                'Case "NoteAcquisizione"

                '    Me.CodicePazienteSelezionato = e.Item.Cells(dgrPazientiColumnIndex.CodicePaziente).Text

                '    ' Visualizzazione note acquisizione per il paziente selezionato
                '    Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                '        Using bizPaziente As New Biz.BizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                '            Me.lblNoteAcquisizione.Text = bizPaziente.GetNoteAcquisizione(Me.CodicePazienteSelezionato)

                '        End Using
                '    End Using

                '    Me.fmNoteAcquisizione.VisibileMD = True

        End Select

    End Sub

    Private Sub dgrPazientiEditRowMovimenti(e As System.Web.UI.WebControls.DataGridCommandEventArgs)

        Me.EditMovimento(e.Item.Cells(dgrPazientiColumnIndex.CodicePaziente).Text)

        ' Datagrid in edit
        Me.dgrPazienti.EditItemIndex = e.Item.ItemIndex

        ' Valorizza i filtri nella maschera con i valori contenuti nella struttura
        Me.SetFiltriRicerca()

        ' Riesegue la ricerca e il bind del datagrid
        Me.CaricaDati(Me.dgrPazienti.CurrentPageIndex)

    End Sub

    Private Sub dgrPazientiCancelRowMovimenti(e As System.Web.UI.WebControls.DataGridCommandEventArgs)

        Me.ReloadData(Me.dgrPazienti.CurrentPageIndex)

    End Sub

    Private Sub dgrPazientiUpdateRowMovimenti(e As System.Web.UI.WebControls.DataGridCommandEventArgs)

        Dim codiceStatoAnagraficoOriginale As String = String.Empty

        If e.Item.ItemType = ListItemType.EditItem Then
            codiceStatoAnagraficoOriginale = DirectCast(e.Item.FindControl("lblCodiceStatoAnagraficoEdit"), Label).Text
        End If

        Dim ddlStatoAnagrafico As DropDownList = DirectCast(e.Item.FindControl("ddlStatoAnagrafico"), DropDownList)

        If Me.UpdateStatoAnagrafico(ddlStatoAnagrafico.SelectedValue, codiceStatoAnagraficoOriginale) Then

            Me.ReloadData(Me.dgrPazienti.CurrentPageIndex)

        End If

    End Sub

    Private Sub dgrPazienti_SortCommand(source As Object, e As DataGridSortCommandEventArgs) Handles dgrPazienti.SortCommand

        If e.SortExpression = Me.CampoOrdinamento Then
            If Me.VersoOrdinamento = "ASC" Then
                Me.VersoOrdinamento = "DESC"
            Else
                Me.VersoOrdinamento = "ASC"
            End If
        Else
            Me.CampoOrdinamento = e.SortExpression
            Me.VersoOrdinamento = "ASC"
        End If

        Me.CaricaDati(Me.dgrPazienti.CurrentPageIndex)

    End Sub

#End Region

    Private Sub OnitLayout31_ConfirmClick(source As Object, e As Onit.Controls.PagesLayout.OnitLayout3.ConfirmEventArgs) Handles OnitLayout31.ConfirmClick

        Select Case e.Key

            Case Common.OnVacStoricoVaccinaleCentralizzato.AcquisisciDatiVaccinaliCentraliConfirmKey

                If e.Result Then

                    Dim command As New Common.OnVacStoricoVaccinaleCentralizzato.AcquisisciStoricoCommand()
                    command.CodicePaziente = Me.CodicePazienteSelezionato
                    command.RichiediConfermaSovrascrittura = False
                    command.Settings = Me.Settings
                    command.OnitLayout3 = Me.OnitLayout31
                    command.BizLogOptions = Nothing
                    command.Note = "Recupero Storico Vaccinale da maschera PazientiInIngresso"

                    Dim acquisisciDatiVaccinaliCentraliResult As Biz.BizPaziente.AcquisisciDatiVaccinaliCentraliResult =
                        Common.OnVacStoricoVaccinaleCentralizzato.AcquisisciDatiVaccinaliCentraliPaziente(command)

                    ' TODO: log???

                    Dim selectedItem As DataGridItem = (From item As DataGridItem In dgrPazienti.Items
                                                        Where item.Cells(dgrPazientiColumnIndex.CodicePaziente).Text = Me.CodicePazienteSelezionato
                                                        Select item).FirstOrDefault()

                    If Not selectedItem Is Nothing Then
                        Me.SetAcquisizioneCommand(Me.CodicePazienteSelezionato, selectedItem, acquisisciDatiVaccinaliCentraliResult.StatoAcquisizioneDatiVaccinaliCentrale)
                    End If

                End If

        End Select

    End Sub

#End Region

#Region " Overrides Pagina Base Movimenti "

    Protected Overrides Sub ImpostaLayoutMovimenti(stato As Common.OnVacMovimentiPageBase.StatoPaginaMovimenti)

        MyBase.ImpostaLayoutMovimenti(stato)

        Dim abilita As Boolean = (stato = StatoPaginaMovimenti.Lettura)

        ' Toolbar
        Me.tlbMovimenti.Items.FromKeyButton("btnCerca").Enabled = abilita
        Me.tlbMovimenti.Items.FromKeyButton("btnStampaElenco").Enabled = abilita
        Me.tlbMovimenti.Items.FromKeyButton("btnStampaElencoPerComune").Enabled = abilita
        Me.tlbMovimenti.Items.FromKeyButton("btnStampaEtichette").Enabled = abilita
        Me.tlbMovimenti.Items.FromKeyButton("btnPulisci").Enabled = abilita

        ' Filtri di ricerca
        Me.odpDaNascita.Enabled = abilita
        Me.odpANascita.Enabled = abilita
        Me.dpkDaImm.Enabled = abilita
        Me.dpkAImm.Enabled = abilita
        Me.dpkDaResidenza.Enabled = abilita
        Me.dpkAResidenza.Enabled = abilita
        Me.dpkDaDomicilio.Enabled = abilita
        Me.dpkADomicilio.Enabled = abilita
        Me.dpkDaAssistenza.Enabled = abilita
        Me.dpkAAssistenza.Enabled = abilita
        Me.uscStatiAnagrafici.Enabled = abilita
        Me.rdbPazReg.Enabled = abilita
        Me.rdbPazNoReg.Enabled = abilita
        Me.rdbIgnoraPazReg.Enabled = abilita
        Me.rdbAcquisizione_NessunDatoDaAcquisire.Enabled = abilita
        Me.rdbAcquisizione_NonEffettuata.Enabled = abilita
        Me.rdbAcquisizione_Parziale.Enabled = abilita
        Me.rdbAcquisizione_Totale.Enabled = abilita
        Me.rdbAcquisizione_Tutti.Enabled = abilita

    End Sub

    Protected Overrides Sub EliminaProgrammazioneEffettuata()

        Me.ReloadData(Me.dgrPazienti.CurrentPageIndex)

    End Sub

    Protected Overrides Sub EliminaProgrammazioneNonEffettuata()

        Me.ReloadData(Me.dgrPazienti.CurrentPageIndex)

    End Sub

#End Region

#Region " Private "

    Private Sub ResetFilters()

        Me.odpDaNascita.Text = String.Empty
        Me.odpANascita.Text = String.Empty
        Me.dpkDaImm.Text = String.Empty
        Me.dpkAImm.Text = String.Empty
        Me.dpkDaResidenza.Text = String.Empty
        Me.dpkAResidenza.Text = String.Empty
        Me.dpkDaDomicilio.Text = String.Empty
        Me.dpkADomicilio.Text = String.Empty
        Me.dpkDaAssistenza.Text = String.Empty
        Me.dpkAAssistenza.Text = String.Empty

        Me.rdbPazReg.Checked = False
        Me.rdbIgnoraPazReg.Checked = False
        Me.rdbPazNoReg.Checked = True

        Me.rdbAcquisizione_NonEffettuata.Checked = True

        ' Reimposto la selezione degli stati anagrafici
        Me.uscStatiAnagrafici.LoadStatiAnagrafici()

        ' Aggiorno la struttura con i dati dei campi
        Me.FiltriRicerca = Me.GetFiltriRicerca()

    End Sub

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()

        listPrintButtons.Add(New PrintButton(Constants.ReportName.ElencoImmigrati, "btnStampaElenco"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.ElencoImmigratiComune, "btnStampaElencoPerComune"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.EtichetteImmigrati, "btnStampaEtichette"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Me.tlbMovimenti)

    End Sub

    Private Sub SetImmagineOrdinamento()

        Dim id As String = String.Empty

        Select Case Me.CampoOrdinamento
            Case Me.dgrPazienti.Columns(dgrPazientiColumnIndex.Cognome).SortExpression
                id = "imgCognome"
            Case Me.dgrPazienti.Columns(dgrPazientiColumnIndex.Nome).SortExpression
                id = "imgNome"
            Case Me.dgrPazienti.Columns(dgrPazientiColumnIndex.DataDiNascita).SortExpression
                id = "imgNascita"
            Case Me.dgrPazienti.Columns(dgrPazientiColumnIndex.Residenza).SortExpression
                id = "imgRes"
            Case Me.dgrPazienti.Columns(dgrPazientiColumnIndex.Domicilio).SortExpression
                id = "imgDom"
            Case Me.dgrPazienti.Columns(dgrPazientiColumnIndex.Immigrazione).SortExpression
                id = "imgImm"
            Case Me.dgrPazienti.Columns(dgrPazientiColumnIndex.Assistenza).SortExpression
                id = "imgAss"
            Case Me.dgrPazienti.Columns(dgrPazientiColumnIndex.Provenienza).SortExpression
                id = "imgProv"
            Case Me.dgrPazienti.Columns(dgrPazientiColumnIndex.StatoAnagraficoDescrizione).SortExpression
                id = "imgAnag"
        End Select

        Dim imageUrl As String = String.Empty

        If Me.VersoOrdinamento = "ASC" Then
            imageUrl = Me.ResolveClientUrl("~/Images/arrow_up_small.gif")
        Else
            imageUrl = Me.ResolveClientUrl("~/Images/arrow_down_small.gif")
        End If

        Me.OnitLayout31.InsertRoutineJS(String.Format("ImpostaImmagineOrdinamento('{0}', '{1}')", id, imageUrl))

    End Sub

    Private Sub CaricaDati(currentPageIndex As Int32)
        '--
        FiltriRicerca = GetFiltriRicerca()
        '--
        Dim loadPazientiInIngressoResult As Biz.BizMovimentiEsterniCNS.LoadPazientiInIngressoResult
        '--
        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizMovimenti As New Biz.BizMovimentiEsterniCNS(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                '--
                loadPazientiInIngressoResult = bizMovimenti.LoadPazientiInIngresso(FiltriRicerca, CampoOrdinamento, VersoOrdinamento, currentPageIndex, dgrPazienti.PageSize)
                '--
            End Using
        End Using
        '--
        dgrPazienti.VirtualItemCount = loadPazientiInIngressoResult.CountPazientiInIngresso
        dgrPazienti.CurrentPageIndex = currentPageIndex
        '--
        ' Possibilità di cambio pagina e di ordinamento solo se non in modifica
        dgrPazienti.PagerStyle.Visible = Not IsPageInEdit()
        dgrPazienti.AllowSorting = Not IsPageInEdit()
        '--
        dgrPazienti.DataSource = loadPazientiInIngressoResult.PazientiInIngressoDataTable
        dgrPazienti.DataBind()
        '--
        ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
        SetColumnVisibility("paz_stato_acquisizione", FlagAbilitazioneVaccUslCorrente)
        '--
        For Each dgrItemPaziente As DataGridItem In dgrPazienti.Items
            '--
            Dim statoAcquisizioneCorrente As String
            Dim codicePazienteCorrente As String
            '--
            If dgrItemPaziente.ItemIndex = dgrPazienti.EditItemIndex Then
                statoAcquisizioneCorrente = DirectCast(dgrItemPaziente.FindControl("lblStatoAcquisizioneEdit"), Label).Text
                codicePazienteCorrente = CodicePazienteSelezionato
            Else
                statoAcquisizioneCorrente = DirectCast(dgrItemPaziente.FindControl("lblStatoAcquisizione"), Label).Text
                codicePazienteCorrente = dgrItemPaziente.Cells(dgrPazientiColumnIndex.CodicePaziente).Text
            End If
            '--
            Dim statoAcquisizioneDatiVaccinaliCentrale As Enumerators.StatoAcquisizioneDatiVaccinaliCentrale? = Nothing
            If Not String.IsNullOrEmpty(statoAcquisizioneCorrente) Then
                statoAcquisizioneDatiVaccinaliCentrale = [Enum].Parse(GetType(Enumerators.StatoAcquisizioneDatiVaccinaliCentrale), statoAcquisizioneCorrente)
            End If
            '--
            SetAcquisizioneCommand(codicePazienteCorrente, dgrItemPaziente, statoAcquisizioneDatiVaccinaliCentrale)
            '--
        Next
        '--
        dgrPazienti.SelectedIndex = -1
        '--
        divSezioneMovimenti.InnerText = String.Format(" MOVIMENTI: {0} risultat{1}.", dgrPazienti.VirtualItemCount, IIf(dgrPazienti.VirtualItemCount = 1, "o", "i"))
        '--
    End Sub

    Private Sub ReloadData(currentPageIndex As Int32)

        StatoPagina = StatoPaginaMovimenti.Lettura

        ' Datagrid in sola lettura
        dgrPazienti.EditItemIndex = -1

        ' Valorizza i filtri nella maschera con i valori contenuti nella struttura
        SetFiltriRicerca()

        ' Riesegue la ricerca e il bind del datagrid
        CaricaDati(currentPageIndex)

    End Sub

    Private Function GetFiltriRicerca() As MovimentiCNSPazientiInIngressoFilter

        Dim filter As New MovimentiCNSPazientiInIngressoFilter()

        filter.CodiceConsultorio = OnVacUtility.Variabili.CNS.Codice

        ' Intervallo date di nascita
        If Not String.IsNullOrEmpty(Me.odpDaNascita.Text) Then filter.DataNascitaInizio = Me.odpDaNascita.Data
        If Not String.IsNullOrEmpty(Me.odpANascita.Text) Then filter.DataNascitaFine = Me.odpANascita.Data

        ' Intervallo date di immigrazione
        If Not String.IsNullOrEmpty(Me.dpkDaImm.Text) Then filter.DataImmigrazioneInizio = Me.dpkDaImm.Data
        If Not String.IsNullOrEmpty(Me.dpkAImm.Text) Then filter.DataImmigrazioneFine = Me.dpkAImm.Data

        ' Intervallo date di residenza
        If Not String.IsNullOrEmpty(Me.dpkDaResidenza.Text) Then filter.DataResidenzaInizio = Me.dpkDaResidenza.Data
        If Not String.IsNullOrEmpty(Me.dpkAResidenza.Text) Then filter.DataResidenzaFine = Me.dpkAResidenza.Data

        ' Intervallo date di domicilio
        If Not String.IsNullOrEmpty(Me.dpkDaDomicilio.Text) Then filter.DataDomicilioInizio = Me.dpkDaDomicilio.Data
        If Not String.IsNullOrEmpty(Me.dpkADomicilio.Text) Then filter.DataDomicilioFine = Me.dpkADomicilio.Data

        ' Intervallo date di assistenza
        If Not String.IsNullOrEmpty(Me.dpkDaAssistenza.Text) Then filter.DataAssistenzaInizio = Me.dpkDaAssistenza.Data
        If Not String.IsNullOrEmpty(Me.dpkAAssistenza.Text) Then filter.DataAssistenzaFine = Me.dpkAAssistenza.Data

        ' Stati anagrafici
        filter.StatiAnagrafici = Me.uscStatiAnagrafici.GetListStatiAnagraficiSelezionati()

        ' Regolarizzazione
        If Me.rdbPazReg.Checked Then
            filter.Regolarizzato = True
        ElseIf Me.rdbPazNoReg.Checked Then
            filter.Regolarizzato = False
        End If

        ' Stato acquisizione
        Dim statoAcquisizioneFilter As Integer?

        If Me.rdbAcquisizione_NonEffettuata.Checked Then
            statoAcquisizioneFilter = MovimentiCNSPazientiInIngressoFilter.ValoriFiltroStatoAcquisizione.AcquisizioneNonEffettuata
        ElseIf Me.rdbAcquisizione_NessunDatoDaAcquisire.Checked Then
            statoAcquisizioneFilter = MovimentiCNSPazientiInIngressoFilter.ValoriFiltroStatoAcquisizione.NessunDatoDaAcquisire
        ElseIf Me.rdbAcquisizione_Parziale.Checked Then
            statoAcquisizioneFilter = MovimentiCNSPazientiInIngressoFilter.ValoriFiltroStatoAcquisizione.AcquisizioneParziale
        ElseIf Me.rdbAcquisizione_Totale.Checked Then
            statoAcquisizioneFilter = MovimentiCNSPazientiInIngressoFilter.ValoriFiltroStatoAcquisizione.AcquisizioneTotale
        ElseIf Me.rdbAcquisizione_Tutti.Checked Then
            statoAcquisizioneFilter = Nothing
        End If

        filter.StatoAcquisizioneDatiVaccinaliCentrale = statoAcquisizioneFilter

        Return filter

    End Function

    Private Sub SetFiltriRicerca()

        If Not Me.FiltriRicerca Is Nothing Then

            ' Intervallo date di nascita
            If Not Me.FiltriRicerca.DataNascitaInizio Is Nothing Then
                Me.odpDaNascita.Data = Me.FiltriRicerca.DataNascitaInizio
            End If
            If Not Me.FiltriRicerca.DataNascitaFine Is Nothing Then
                Me.odpANascita.Data = Me.FiltriRicerca.DataNascitaFine
            End If

            ' Intervallo date di immigrazione
            If Not Me.FiltriRicerca.DataImmigrazioneInizio Is Nothing Then
                Me.dpkDaImm.Data = Me.FiltriRicerca.DataImmigrazioneInizio
            End If
            If Not Me.FiltriRicerca.DataImmigrazioneFine Is Nothing Then
                Me.dpkAImm.Data = Me.FiltriRicerca.DataImmigrazioneFine
            End If

            ' Intervallo date di residenza
            If Not Me.FiltriRicerca.DataResidenzaInizio Is Nothing Then
                Me.dpkDaResidenza.Data = Me.FiltriRicerca.DataResidenzaInizio
            End If
            If Not Me.FiltriRicerca.DataResidenzaFine Is Nothing Then
                Me.dpkAResidenza.Data = Me.FiltriRicerca.DataResidenzaFine
            End If

            ' Intervallo date di domicilio
            If Not Me.FiltriRicerca.DataDomicilioInizio Is Nothing Then
                Me.dpkDaDomicilio.Data = Me.FiltriRicerca.DataDomicilioInizio
            End If
            If Not Me.FiltriRicerca.DataDomicilioFine Is Nothing Then
                Me.dpkADomicilio.Data = Me.FiltriRicerca.DataDomicilioFine
            End If

            ' Intervallo date di assistenza
            If Not Me.FiltriRicerca.DataAssistenzaInizio Is Nothing Then
                Me.dpkDaAssistenza.Data = Me.FiltriRicerca.DataAssistenzaInizio
            End If
            If Not Me.FiltriRicerca.DataAssistenzaFine Is Nothing Then
                Me.dpkAAssistenza.Data = Me.FiltriRicerca.DataAssistenzaFine
            End If

            ' Stati anagrafici
            Me.uscStatiAnagrafici.SetStatiAnagrafici(Me.FiltriRicerca.StatiAnagrafici)

            ' Regolarizzazione
            Dim pazReg As Boolean = False
            Dim pazNoReg As Boolean = False
            Dim ignoraPazReg As Boolean = False

            If Not Me.FiltriRicerca.Regolarizzato Is Nothing Then
                If Me.FiltriRicerca.Regolarizzato Then
                    pazReg = True
                Else
                    pazNoReg = True
                End If
            Else
                ignoraPazReg = True
            End If

            Me.rdbPazReg.Checked = pazReg
            Me.rdbPazNoReg.Checked = pazNoReg
            Me.rdbIgnoraPazReg.Checked = ignoraPazReg

            ' Stato acquisizione
            Me.rdbAcquisizione_NessunDatoDaAcquisire.Checked =
                (Me.FiltriRicerca.StatoAcquisizioneDatiVaccinaliCentrale.HasValue AndAlso
                 Me.FiltriRicerca.StatoAcquisizioneDatiVaccinaliCentrale = MovimentiCNSPazientiInIngressoFilter.ValoriFiltroStatoAcquisizione.NessunDatoDaAcquisire)

            Me.rdbAcquisizione_NonEffettuata.Checked =
                (Me.FiltriRicerca.StatoAcquisizioneDatiVaccinaliCentrale.HasValue AndAlso
                 Me.FiltriRicerca.StatoAcquisizioneDatiVaccinaliCentrale = MovimentiCNSPazientiInIngressoFilter.ValoriFiltroStatoAcquisizione.AcquisizioneNonEffettuata)

            Me.rdbAcquisizione_Parziale.Checked =
                (Me.FiltriRicerca.StatoAcquisizioneDatiVaccinaliCentrale.HasValue AndAlso
                 Me.FiltriRicerca.StatoAcquisizioneDatiVaccinaliCentrale = MovimentiCNSPazientiInIngressoFilter.ValoriFiltroStatoAcquisizione.AcquisizioneParziale)

            Me.rdbAcquisizione_Totale.Checked =
                (Me.FiltriRicerca.StatoAcquisizioneDatiVaccinaliCentrale.HasValue AndAlso
                 Me.FiltriRicerca.StatoAcquisizioneDatiVaccinaliCentrale = MovimentiCNSPazientiInIngressoFilter.ValoriFiltroStatoAcquisizione.AcquisizioneTotale)

            Me.rdbAcquisizione_Tutti.Checked =
                (Not Me.FiltriRicerca.StatoAcquisizioneDatiVaccinaliCentrale.HasValue)

        End If

    End Sub

    Private Sub SetAcquisizioneCommand(codicePaziente As Long, datagridItem As DataGridItem, statoAcquisizioneDatiVaccinaliCentrale As Enumerators.StatoAcquisizioneDatiVaccinaliCentrale?)
        '--
        Dim lblVisibility As Boolean = False
        Dim btnVisibility As Boolean = False
        '--
        If FlagAbilitazioneVaccUslCorrente Then
            '--
            If statoAcquisizioneDatiVaccinaliCentrale.HasValue Then
                '--
                Select Case statoAcquisizioneDatiVaccinaliCentrale
                    Case Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Parziale
                        btnVisibility = True
                        'datagridItem.FindControl("btnNoteAcquisizione").Visible = True
                    Case Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Totale, Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Vuota
                        lblVisibility = True
                    Case Else
                        Throw New NotImplementedException()
                End Select
                '--
            Else
                '--
                btnVisibility = True
                '--
            End If
            '--
        End If
        '--
        datagridItem.FindControl("lblAcquisizione").Visible = lblVisibility
        datagridItem.FindControl("btnAcquisizione").Visible = btnVisibility
        '--
    End Sub

    Private Sub SetColumnVisibility(sortExpression As String, visible As Boolean)

        Dim column As DataGridColumn = Me.GetColumnBySortExpression(sortExpression)

        If Not column Is Nothing Then column.Visible = visible

    End Sub

    Private Function GetColumnBySortExpression(sortExpression As String) As DataGridColumn

        If dgrPazienti.Items.Count = 0 Then Return Nothing

        Dim column As DataGridColumn =
            (From item As DataGridColumn In Me.dgrPazienti.Columns
             Where item.SortExpression = sortExpression
             Select item).FirstOrDefault()

        Return column

    End Function

    Private Sub SetColumnHeaders()

        Me.dgrPazienti.Columns(dgrPazientiColumnIndex.Assistenza).HeaderText =
            Me.dgrPazienti.Columns(dgrPazientiColumnIndex.Assistenza).HeaderText.Insert(0, Me.GetOnVacResourceValue(Constants.StringResourcesKey.IntestazioneGrid_DataUslAssistenza))

        Me.dgrPazienti.Columns(dgrPazientiColumnIndex.Provenienza).HeaderText =
            Me.dgrPazienti.Columns(dgrPazientiColumnIndex.Provenienza).HeaderText.Insert(0, Me.GetOnVacResourceValue(Constants.StringResourcesKey.IntestazioneGrid_UslProvenienza))

    End Sub

#Region " Stampe "

    Private Sub StampaElenco(nomeReport As String)

        Me.FiltriRicerca = Me.GetFiltriRicerca()

        Dim datiIntestazione As Entities.DatiIntestazioneReport

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            ' Caricamento dati da stampare 
            Dim dst As DstMovimentiEsterni = Nothing

            Using bizMovimenti As New Biz.BizMovimentiEsterniCNS(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())
                dst = bizMovimenti.LoadPazientiInIngresso(Me.FiltriRicerca, Me.CampoOrdinamento, Me.VersoOrdinamento)
            End Using

            Dim rpt As New ReportParameter()

            rpt.set_dataset(dst)

            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                ' Caricamento parametri intestazione report
                datiIntestazione = bizReport.GetDatiIntestazione()

                ' Parametri intestazione
                rpt.AddParameter("UslCitta", datiIntestazione.ComuneUsl)
                rpt.AddParameter("UslDesc", datiIntestazione.DescrizioneUslPerReport)
                rpt.AddParameter("UslReg", datiIntestazione.RegioneUsl)

                rpt.AddParameter("certificato", "")
                rpt.AddParameter("Ambulatorio", OnVacUtility.Variabili.CNS.Descrizione + "(" + OnVacUtility.Variabili.CNS.Codice + ")")

                rpt.AddParameter("TitoloReport", "Elenco Pazienti In Ingresso")

                ' Filtri
                rpt.AddParameter("Da_data_nascita", Me.GetDateParameterValue(Me.odpDaNascita))
                rpt.AddParameter("A_data_nascita", Me.GetDateParameterValue(Me.odpANascita))

                rpt.AddParameter("Da_data_imm", Me.GetDateParameterValue(Me.dpkDaImm))
                rpt.AddParameter("A_data_imm", Me.GetDateParameterValue(Me.dpkAImm))

                rpt.AddParameter("Da_data_inizio_residenza", Me.GetDateParameterValue(Me.dpkDaResidenza))
                rpt.AddParameter("A_data_inizio_residenza", Me.GetDateParameterValue(Me.dpkAResidenza))

                rpt.AddParameter("Da_data_inizio_domicilio", Me.GetDateParameterValue(Me.dpkDaDomicilio))
                rpt.AddParameter("A_data_inizio_domicilio", Me.GetDateParameterValue(Me.dpkADomicilio))

                rpt.AddParameter("Da_data_inizio_assistenza", Me.GetDateParameterValue(Me.dpkDaDomicilio))
                rpt.AddParameter("A_data_inizio_assistenza", Me.GetDateParameterValue(Me.dpkADomicilio))

                rpt.AddParameter("stato_anagrafico", Me.uscStatiAnagrafici.GetSelectedDescriptions())

                rpt.AddParameter("regolarizzato", IIf(Me.rdbPazReg.Checked, "regolarizzati", IIf(Me.rdbPazNoReg.Checked, "non regolarizzati", "entrambi")))

                ' Stampa
                If Not OnVacReport.StampaReport(Page.Request.Path, nomeReport, String.Empty, rpt, Nothing, Nothing, bizReport.GetReportFolder(nomeReport)) Then
                    OnVacUtility.StampaNonPresente(Page, nomeReport)
                End If

            End Using
        End Using

    End Sub

    Private Sub StampaEtichette()

        Dim dst As DstMovimentiEsterni = Nothing

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizMovimenti As New Biz.BizMovimentiEsterniCNS(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                dst = bizMovimenti.LoadPazientiInIngresso(Me.GetFiltriRicerca(), Me.CampoOrdinamento, Me.VersoOrdinamento)

            End Using
        End Using

        Dim errMsg As String = String.Empty

        If Not Me.UscFiltriEtichette.StampaEtichette(dst, errMsg) Then

            ' Stampa non effettuata, mostro il messaggio di errore
            ClientScript.RegisterClientScriptBlock(GetType(String), "msg_stampa", String.Format("<script type='text/javascript'>alert('{0}');</script>", errMsg))

        End If

    End Sub

#End Region

#End Region

End Class
