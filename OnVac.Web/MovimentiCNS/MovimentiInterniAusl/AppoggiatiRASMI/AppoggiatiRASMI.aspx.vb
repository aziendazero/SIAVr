Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager


Partial Class AppoggiatiRASMI
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

    Private Property FiltriRicerca() As AppoggiatiRASMIFiltriRicerca
        Get
            Return Session("AppoggiatiRASMIFiltriRicerca")
        End Get
        Set(value As AppoggiatiRASMIFiltriRicerca)
            Session("AppoggiatiRASMIFiltriRicerca") = value
        End Set
    End Property

    Protected Overrides ReadOnly Property OnitLayout As Controls.PagesLayout.OnitLayout3
        Get
            Return Me.OnitLayout31
        End Get
    End Property

    Private Property idxPrecRowSelected() As Integer
        Get
            Return ViewState("idxPrecRowSelected")
        End Get
        Set(ByVal Value As Integer)
            ViewState("idxPrecRowSelected") = Value
        End Set
    End Property

#End Region

#Region " Enum "

    Protected Enum IndexColonneDatagridPazienti
        RowSelect = 0
        Cognome = 1
        Nome = 2
        DataNascita = 3
        DescrizioneStatoAnagrafico = 4
        Descrizione = 5
        DataInserimento = 6
        Indirizzo = 7
        Comune = 8
        BtnViewPaziente = 9
        CodSedeVaccinale = 10
        CodPaziente = 11
        CodSedeVaccinalePrec = 12
        StatoAnagrafico = 13
        StatoAnagraficoPrec = 14
    End Enum

#End Region

#Region " Page Events "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            ShowPrintButtons()

            SetFiltriRicerca()

            ClearDetail()

            lblStatoAnagrafico.Visible = Settings.MOVCV_EDIT_STATO_ANAGRAFICO
            ddlStatoAnagrafico.Visible = Settings.MOVCV_EDIT_STATO_ANAGRAFICO

        End If

    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub tlbMovimenti_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles tlbMovimenti.ButtonClicked

        Select Case be.Button.Key

            Case "btnCerca"

                CaricaDati(0)

            Case "btnSalva"

                SalvaDati()
                CaricaDati(DatagridPazienti.CurrentPageIndex)

                StatoPagina = StatoPaginaMovimenti.Lettura

            Case "btnAnnulla"

                CaricaDati(DatagridPazienti.CurrentPageIndex)

                StatoPagina = StatoPaginaMovimenti.Lettura

            Case "btnStampaElenco"

                StampaElencoMovimentiInterni(TipoMovimentoCns.MovimentoInterno.Smistamento,
                                                DatagridPazienti,
                                                odpDaNascita,
                                                odpANascita,
                                                dpkDaConsultorio,
                                                dpkAConsultorio)

            Case "btnPulisci"
                ' Sbiancamento campi di filtro -> lato client

        End Select

    End Sub

#End Region

#Region " Datagrid Events "

    Private Sub DatagridPazienti_PageIndexChanged(source As Object, e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles DatagridPazienti.PageIndexChanged

        CaricaDati(e.NewPageIndex)

    End Sub

    Private Sub DatagridPazienti_ItemCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles DatagridPazienti.ItemCommand

        Select Case e.CommandName

            Case "DatiPaziente"
                RedirectToGestionePaziente(DatagridPazienti.Items(e.Item.ItemIndex).Cells(IndexColonneDatagridPazienti.CodPaziente).Text)

        End Select

    End Sub




    Private Sub DatagridPazienti_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles DatagridPazienti.SelectedIndexChanged

        Dim datagridSelectedItem As DataGridItem = DatagridPazienti.SelectedItem

        SelectMovimento(datagridSelectedItem.Cells(IndexColonneDatagridPazienti.CodPaziente).Text, True)

        ' Calcolo età paziente
        Dim d As Date = Date.Parse(datagridSelectedItem.Cells(IndexColonneDatagridPazienti.DataNascita).Text)
        Dim giorniEtaPaz As Integer = Date.Now.Date.Subtract(d).Days

        ' Filtro modale
        txtConsultorioVacc.Filtro = "cns_data_apertura <= SYSDATE"
        txtConsultorioVacc.Filtro += " AND (cns_data_chiusura > SYSDATE OR cns_data_chiusura IS NULL)"
        txtConsultorioVacc.Filtro += " AND (cns_da_eta <= " + giorniEtaPaz.ToString()
        txtConsultorioVacc.Filtro += " AND cns_a_eta >= " + giorniEtaPaz.ToString() + ") "
        txtConsultorioVacc.Filtro += "ORDER BY Descrizione"

        txtConsultorioVacc.Enabled = True

        ' Nella riga precedentemente selezionata devo scrivere il consultorio selezionato nel dettaglio
        If Not IsNothing(idxPrecRowSelected) AndAlso idxPrecRowSelected <> -1 AndAlso idxPrecRowSelected <> DatagridPazienti.SelectedIndex Then

            'item precedente
            Dim datagridPrecItem As DataGridItem = DatagridPazienti.Items(idxPrecRowSelected)

            AggiornaDatiRigaSelezionata(datagridPrecItem,
                                           txtConsultorioVacc.Codice,
                                           txtConsultorioVacc.Descrizione,
                                           ddlStatoAnagrafico.SelectedValue,
                                           ddlStatoAnagrafico.SelectedItem.Text)
        End If

        ' Il campo consultorio deve partire nullo
        txtConsultorioVacc.Codice = String.Empty
        txtConsultorioVacc.Descrizione = String.Empty

        ' Bind dropdownlist (solo se non è già bindata)
        If ddlStatoAnagrafico.Items Is Nothing OrElse ddlStatoAnagrafico.Items.Count = 0 Then

            BindDdlStatoAnagrafico(ddlStatoAnagrafico, Nothing)

        End If

        ddlStatoAnagrafico.ClearSelection()

        ' Valorizzazione dropdownlist con lo stato anagrafico presente nella riga selezionata 
        Dim listItem As ListItem =
            ddlStatoAnagrafico.Items.FindByValue(datagridSelectedItem.Cells(IndexColonneDatagridPazienti.StatoAnagrafico).Text)

        If Not listItem Is Nothing Then

            ' Se stato anagrafico trovato lo seleziono
            listItem.Selected = True

            ' Elimino l'elemento vuoto (se c'è)
            Dim emptyItem As ListItem = ddlStatoAnagrafico.Items.FindByValue(String.Empty)

            If Not emptyItem Is Nothing Then
                ddlStatoAnagrafico.Items.Remove(emptyItem)
            End If

        Else

            ' Se stato anagrafico non trovato o nullo, aggiungo l'elemento nullo e lo seleziono
            If ddlStatoAnagrafico.Items.Count > 0 Then

                ddlStatoAnagrafico.Items.Insert(0, New ListItem(String.Empty, String.Empty))
                ddlStatoAnagrafico.Items(0).Selected = True

            End If

        End If

        'setto l'indexRow della riga precedente
        idxPrecRowSelected = DatagridPazienti.SelectedIndex

        ' Possibilità di cambio pagina solo se non in modifica
        DatagridPazienti.PagerStyle.Visible = False

    End Sub


#End Region

#Region " DropDownList events "

    Private Sub ddlStatoAnagrafico_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlStatoAnagrafico.SelectedIndexChanged

        If Not String.IsNullOrEmpty(ddlStatoAnagrafico.SelectedValue) Then

            ConfermaCancellazioneProgrammazione(ddlStatoAnagrafico.SelectedValue, False)

        End If

    End Sub

#End Region

#Region " Overrides "

    Protected Overrides Sub ImpostaLayoutMovimenti(stato As StatoPaginaMovimenti)

        MyBase.ImpostaLayoutMovimenti(stato)

        Dim abilita As Boolean = (stato = StatoPaginaMovimenti.Lettura)

        ' Toolbar
        tlbMovimenti.Items.FromKeyButton("btnCerca").Enabled = abilita
        tlbMovimenti.Items.FromKeyButton("btnSalva").Enabled = Not abilita
        tlbMovimenti.Items.FromKeyButton("btnAnnulla").Enabled = Not abilita
        tlbMovimenti.Items.FromKeyButton("btnStampaElenco").Enabled = abilita
        tlbMovimenti.Items.FromKeyButton("btnPulisci").Enabled = abilita

        ' Filtri di ricerca
        odpDaNascita.Enabled = abilita
        odpANascita.Enabled = abilita
        dpkDaConsultorio.Enabled = abilita
        dpkAConsultorio.Enabled = abilita

        ' Modale consultorio
        txtConsultorioVacc.Enabled = Not abilita

        ' Dropdownlist stato anagrafico
        ddlStatoAnagrafico.Enabled = Not abilita

    End Sub

    Protected Overrides Sub EliminaProgrammazioneEffettuata()

        ' Non deve fare nulla: pagina in edit, riga selezionata e l'utente può decidere se salvare o annullare

    End Sub

    Protected Overrides Sub EliminaProgrammazioneNonEffettuata()

        ' Non deve fare nulla: pagina in edit, riga selezionata e l'utente può decidere se salvare o annullare

    End Sub

#End Region

#Region " Private "

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()

        listPrintButtons.Add(New PrintButton(Constants.ReportName.ElencoMovimenti, "btnStampaElenco"))

        ShowToolbarPrintButtons(listPrintButtons, tlbMovimenti)

    End Sub

    Private Sub CaricaDati(currentPageIndex As Int32)

        Dim dt As New DataTable()

        Dim countMovimenti As Int32 = 0

        Dim pagingOptions As New MovimentiInterniCNSPagingOptions()
        pagingOptions.StartRecordIndex = currentPageIndex * DatagridPazienti.PageSize
        pagingOptions.EndRecordIndex = pagingOptions.StartRecordIndex + DatagridPazienti.PageSize

        FiltriRicerca = GetFiltriRicerca()

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizMovimentiInterni As New Biz.BizMovimentiInterniCNS(genericProvider, Settings, OnVacContext.CreateBizContextInfos, Nothing)

                dt = bizMovimentiInterni.GetDtSmistamenti(FiltriRicerca, pagingOptions)

                countMovimenti = bizMovimentiInterni.CountSmistamenti(FiltriRicerca)

            End Using

        End Using

        DatagridPazienti.VirtualItemCount = countMovimenti
        DatagridPazienti.CurrentPageIndex = currentPageIndex

        ' Possibilità di cambio pagina solo se non in modifica
        DatagridPazienti.PagerStyle.Visible = (Not StatoPagina = StatoPaginaMovimenti.Modifica)

        DatagridPazienti.DataSource = dt
        DatagridPazienti.DataBind()

        DatagridPazienti.SelectedIndex = -1

        'sbiancamento della property idxPrecRowSelected
        idxPrecRowSelected = DatagridPazienti.SelectedIndex

        divSezioneMovimenti.InnerText = String.Format(" MOVIMENTI: {0} risultat{1}.", DatagridPazienti.VirtualItemCount, IIf(DatagridPazienti.VirtualItemCount = 1, "o", "i"))

        ClearDetail()

    End Sub

    Private Sub ClearDetail()

        ' Sbiancamento campi modale
        txtConsultorioVacc.Codice = String.Empty
        txtConsultorioVacc.Descrizione = String.Empty
        txtConsultorioVacc.RefreshDataBind()

        ' Sbiancamento dropdownlist
        ddlStatoAnagrafico.ClearSelection()
        ddlStatoAnagrafico.Items.Clear()
        ddlStatoAnagrafico.DataSource = Nothing
        ddlStatoAnagrafico.DataBind()

    End Sub

    Private Sub SetTableCell(datagridItem As DataGridItem, columnIndex As IndexColonneDatagridPazienti, text As String, itemModified As Boolean, toolTip As String)

        ' Descrizione consultorio
        Dim item As TableCell = datagridItem.Cells(columnIndex)

        item.Text = text
        item.Font.Italic = itemModified
        item.ToolTip = toolTip

    End Sub

    Private Sub AggiornaDatiRigaSelezionata(datagridItem As DataGridItem, codiceConsultorio As String, descrizioneConsultorio As String, codiceStatoAnagrafico As String, descrizioneStatoAnagrafico As String)

        ' Codice e Descrizione Consultorio
        If Not String.IsNullOrEmpty(codiceConsultorio) Then

            ' Se consultorio diverso da quello originale
            If codiceConsultorio <> GetTextFromDatagridItem(datagridItem, IndexColonneDatagridPazienti.CodSedeVaccinalePrec) Then

                SetTableCell(datagridItem, IndexColonneDatagridPazienti.CodSedeVaccinale, codiceConsultorio, True, String.Empty)
                SetTableCell(datagridItem, IndexColonneDatagridPazienti.Descrizione, descrizioneConsultorio, True, "Centro vaccinale modificato")

            Else

                SetTableCell(datagridItem, IndexColonneDatagridPazienti.CodSedeVaccinale, codiceConsultorio, False, String.Empty)
                SetTableCell(datagridItem, IndexColonneDatagridPazienti.Descrizione, descrizioneConsultorio, False, String.Empty)

            End If

        End If

        ' Codice e Descrizione Stato Anagrafico
        If Not String.IsNullOrEmpty(codiceStatoAnagrafico) Then

            ' Se stato anagrafico diverso da quello già selezionato
            If codiceStatoAnagrafico <> GetTextFromDatagridItem(datagridItem, IndexColonneDatagridPazienti.StatoAnagraficoPrec) Then

                Me.SetTableCell(datagridItem, IndexColonneDatagridPazienti.StatoAnagrafico, codiceStatoAnagrafico, True, String.Empty)
                Me.SetTableCell(datagridItem, IndexColonneDatagridPazienti.DescrizioneStatoAnagrafico, descrizioneStatoAnagrafico, True, "Stato anagrafico modificato")

            Else

                Me.SetTableCell(datagridItem, IndexColonneDatagridPazienti.StatoAnagrafico, codiceStatoAnagrafico, False, String.Empty)
                Me.SetTableCell(datagridItem, IndexColonneDatagridPazienti.DescrizioneStatoAnagrafico, descrizioneStatoAnagrafico, False, String.Empty)

            End If

        End If

    End Sub

    ' Registra la variazione di consultorio nella t_paz_pazienti
    Private Sub SalvaDati()

        ' Controllo che il valore del consultorio nell'ultima riga selezionata 
        ' corrisponda a quello del dettaglio (tranne il caso in cui il dettaglio sia vuoto)
        If Not IsNothing(DatagridPazienti.SelectedItem) Then

            AggiornaDatiRigaSelezionata(DatagridPazienti.SelectedItem,
                                           txtConsultorioVacc.Codice,
                                           txtConsultorioVacc.Descrizione,
                                           ddlStatoAnagrafico.SelectedValue,
                                           ddlStatoAnagrafico.SelectedItem.Text)
        End If

        Dim listPazientiSmistamento As New List(Of Entities.MovimentoCNS.PazienteSmistamento)()

        For i As Integer = 0 To DatagridPazienti.Items.Count - 1

            ' Codice del paziente
            Dim codicePaziente As String = GetTextFromDatagridItem(DatagridPazienti.Items(i), IndexColonneDatagridPazienti.CodPaziente)

            ' Codice del consultorio assegnato
            Dim codiceConsultorioSelezionato As String = Me.GetTextFromDatagridItem(DatagridPazienti.Items(i), IndexColonneDatagridPazienti.CodSedeVaccinale)

            ' Codice del consultorio precedente
            Dim codiceConsultorioPrecedente As String = Me.GetTextFromDatagridItem(DatagridPazienti.Items(i), IndexColonneDatagridPazienti.CodSedeVaccinalePrec)

            ' Codice dello stato anagrafico assegnato
            Dim codiceStatoAnagraficoSelezionato As String = Me.GetTextFromDatagridItem(DatagridPazienti.Items(i), IndexColonneDatagridPazienti.StatoAnagrafico)

            ' Codice dello stato anagrafico precedente
            Dim codiceStatoAnagraficoPrecedente As String = Me.GetTextFromDatagridItem(DatagridPazienti.Items(i), IndexColonneDatagridPazienti.StatoAnagraficoPrec)

            If (Not String.IsNullOrEmpty(codiceConsultorioSelezionato) AndAlso codiceConsultorioSelezionato <> codiceConsultorioPrecedente) OrElse
               (Not String.IsNullOrEmpty(codiceStatoAnagraficoSelezionato) AndAlso codiceStatoAnagraficoSelezionato <> codiceStatoAnagraficoPrecedente) Then

                listPazientiSmistamento.Add(New Entities.MovimentoCNS.PazienteSmistamento() With
                                            {
                                                .CodicePaziente = codicePaziente,
                                                .CodiceConsultorioCorrente = codiceConsultorioSelezionato,
                                                .CodiceConsultorioPrecedente = codiceConsultorioPrecedente,
                                                .StatoAnagraficoCorrente = GetEnumValueFromStringStatoAnagrafico(codiceStatoAnagraficoSelezionato),
                                                .StatoAnagraficoPrecedente = GetEnumValueFromStringStatoAnagrafico(codiceStatoAnagraficoPrecedente)
                                            })
            End If

        Next

        If listPazientiSmistamento Is Nothing OrElse listPazientiSmistamento.Count = 0 Then

            OnitLayout.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Salvataggio non effettuato: nessuna modifica da salvare.", "alertStatoAnag", False, False))

        Else

            Dim bizResultMovimenti As Biz.BizMovimentiInterniCNS.BizRisultatoMovimentiInterniCNS = Nothing

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                Using bizMovimenti As New Biz.BizMovimentiInterniCNS(genericProvider, Settings, OnVacContext.CreateBizContextInfos, Nothing)

                    bizResultMovimenti = bizMovimenti.SalvataggioDatiPazientiSmistamento(listPazientiSmistamento)

                End Using

            End Using

            If Not bizResultMovimenti.Successo Then

                OnitLayout.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(bizResultMovimenti.Messaggio, "alertStatoAnag", False, False))

            End If

        End If

    End Sub

    Private Function GetTextFromDatagridItem(dataGridItem As DataGridItem, indexColumn As IndexColonneDatagridPazienti) As String

        Return HttpUtility.HtmlDecode(dataGridItem.Cells(indexColumn).Text).Trim()

    End Function

    Private Function GetEnumValueFromStringStatoAnagrafico(codiceStatoAnagrafico As String)

        If String.IsNullOrEmpty(codiceStatoAnagrafico) Then
            Return Nothing
        End If

        Dim enumValueStatoAnagrafico As Enumerators.StatoAnagrafico?
        Try
            enumValueStatoAnagrafico = [Enum].Parse(GetType(Enumerators.StatoAnagrafico), codiceStatoAnagrafico)
        Catch ex As Exception
            enumValueStatoAnagrafico = Nothing
        End Try

        Return enumValueStatoAnagrafico

    End Function

    'Private Sub ReloadData()

    '    StatoPagina = StatoPaginaMovimenti.Lettura

    '    ' Datagrid in sola lettura
    '    DatagridPazienti.EditItemIndex = -1

    '    ' Riesegue la ricerca e il bind del datagrid
    '    SetFiltriRicerca()
    '    CaricaDati(DatagridPazienti.CurrentPageIndex)

    'End Sub

    Private Function GetFiltriRicerca() As AppoggiatiRASMIFiltriRicerca

        Dim appoggiatiRASMIFiltriRicerca As New AppoggiatiRASMIFiltriRicerca()

        If Not String.IsNullOrEmpty(odpDaNascita.Text) Then
            appoggiatiRASMIFiltriRicerca.DataNascitaDa = odpDaNascita.Data
        End If

        If Not String.IsNullOrEmpty(odpANascita.Text) Then
            appoggiatiRASMIFiltriRicerca.DataNascitaA = odpANascita.Data
        End If

        If Not String.IsNullOrEmpty(dpkDaConsultorio.Text) Then
            appoggiatiRASMIFiltriRicerca.DataCnsDa = dpkDaConsultorio.Data
        End If

        If Not String.IsNullOrEmpty(dpkAConsultorio.Text) Then
            appoggiatiRASMIFiltriRicerca.DataCnsA = dpkAConsultorio.Data
        End If

        Return appoggiatiRASMIFiltriRicerca

    End Function

    Private Sub SetFiltriRicerca()

        If Not FiltriRicerca Is Nothing Then

            If Not FiltriRicerca.DataNascitaDa Is Nothing Then
                odpDaNascita.Data = FiltriRicerca.DataNascitaDa
            End If

            If Not FiltriRicerca.DataNascitaA Is Nothing Then
                odpANascita.Data = FiltriRicerca.DataNascitaA
            End If

            If Not FiltriRicerca.DataCnsDa Is Nothing Then
                dpkDaConsultorio.Data = FiltriRicerca.DataCnsDa
            End If

            If Not FiltriRicerca.DataCnsA Is Nothing Then
                dpkAConsultorio.Data = FiltriRicerca.DataCnsA
            End If

        End If

    End Sub

    Private Overloads Sub StampaElencoMovimentiInterni(tipoMovimento As String, dgrPazienti As DataGrid, dpkDaNascita As Onit.Web.UI.WebControls.Validators.OnitDatePick, dpkANascita As Onit.Web.UI.WebControls.Validators.OnitDatePick, dpkDaConsultorio As Onit.Web.UI.WebControls.Validators.OnitDatePick, dpkAConsultorio As Onit.Web.UI.WebControls.Validators.OnitDatePick)

        ' Valorizzazione dataset da passare al report
        Dim dst As New DstMovimentiInterni()

        Dim dt As New DataTable()
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizMovimentiInterni As New Biz.BizMovimentiInterniCNS(genericProvider, Settings, OnVacContext.CreateBizContextInfos, Nothing)

                dt = bizMovimentiInterni.GetDtSmistamenti(FiltriRicerca, Nothing)

            End Using

        End Using

        Dim newRow As DataRow = Nothing

        For i As Integer = 0 To dt.Rows.Count - 1

            newRow = dst.Tables("Movimenti").NewRow()

            ' Dati per report
            If Not String.IsNullOrEmpty(dt.Rows(i)("paz_codice").ToString()) Then
                newRow("CodPaziente") = CLng(dt.Rows(i)("paz_codice").ToString())
            End If
            newRow("Cognome") = dt.Rows(i)("paz_cognome").ToString()
            newRow("Nome") = dt.Rows(i)("paz_nome").ToString()
            If Not String.IsNullOrEmpty(dt.Rows(i)("paz_data_nascita").ToString()) Then
                newRow("DataNascita") = CDate(dt.Rows(i)("paz_data_nascita").ToString())
            End If
            newRow("Indirizzo") = dt.Rows(i)("paz_indirizzo_domicilio").ToString()
            newRow("StatoAnagrafico") = dt.Rows(i)("stato_anagrafico").ToString()
            If Not String.IsNullOrEmpty(dt.Rows(i)("paz_data_inserimento").ToString()) Then
                newRow("DataInserimento") = CDate(dt.Rows(i)("paz_data_inserimento").ToString())
            End If

            dst.Tables("Movimenti").Rows.Add(newRow)

        Next

        Dim rpt As New ReportParameter()

        rpt.set_dataset(dst)

        ' Parametri intestazione
        Dim datiIntestazione As Entities.DatiIntestazioneReport = GetDatiIntestazioneReport()

        rpt.AddParameter("UslCitta", datiIntestazione.ComuneUsl)
        rpt.AddParameter("UslDesc", datiIntestazione.DescrizioneUslPerReport)
        rpt.AddParameter("UslReg", datiIntestazione.RegioneUsl)

        ' Filtri
        rpt.AddParameter("Da_data_nascita", GetDateParameterValue(dpkDaNascita))
        rpt.AddParameter("A_data_nascita", GetDateParameterValue(dpkANascita))
        rpt.AddParameter("Da_data_cns", GetDateParameterValue(dpkDaConsultorio))
        rpt.AddParameter("A_data_cns", GetDateParameterValue(dpkAConsultorio))

        ' Altri parametri
        rpt.AddParameter("Ambulatorio", OnVacUtility.Variabili.CNS.Descrizione + "(" + OnVacUtility.Variabili.CNS.Codice + ")")
        rpt.AddParameter("Tipo", tipoMovimento)
        rpt.AddParameter("TotAssistiti", dt.Rows.Count.ToString())

        ' Stampa
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If Not OnVacReport.StampaReport(Page.Request.Path, Constants.ReportName.ElencoMovimenti, String.Empty, rpt, Nothing, Nothing, bizReport.GetReportFolder(Constants.ReportName.ElencoMovimenti)) Then
                    OnVacUtility.StampaNonPresente(Page, Constants.ReportName.ElencoMovimenti)
                End If

            End Using
        End Using

    End Sub

#End Region

End Class
