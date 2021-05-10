Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager


Partial Class MovimentiInIngresso
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

#Region " Types "

    <Serializable()>
    Private Class MovimentiInIngressoFiltriRicerca
        Public DataNascitaDa As DateTime?
        Public DataNascitaA As DateTime?
        Public DataCnsDa As DateTime?
        Public DataCnsA As DateTime?
        Public AutoMovAdulti As String
    End Class

#End Region

#Region " Properties "

    Private Property FiltriRicerca() As MovimentiInIngressoFiltriRicerca
        Get
            Return Session("MovimentiInIngressoFiltriRicerca")
        End Get
        Set(value As MovimentiInIngressoFiltriRicerca)
            Session("MovimentiInIngressoFiltriRicerca") = value
        End Set
    End Property

    Protected Overrides ReadOnly Property OnitLayout As Controls.PagesLayout.OnitLayout3
        Get
            Return Me.OnitLayout31
        End Get
    End Property

#End Region

#Region " Page events "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            Me.ShowPrintButtons()

            Me.SetFiltriRicerca()

        End If

    End Sub

#End Region

#Region " Toolbar events "

    Private Sub tlbMovimenti_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles tlbMovimenti.ButtonClicked

        Select Case be.Button.Key

            Case "btnCerca"

                Me.CaricaDati()

            Case "btnStampaElenco"

                Me.StampaElencoMovimentiInterni(TipoMovimentoCns.MovimentoInterno.Ingresso,
                                                Me.dgrPazienti,
                                                Me.odpDaNascita,
                                                Me.odpANascita,
                                                Me.dpkDaConsultorio,
                                                Me.dpkAConsultorio)

            Case "btnPulisci"
                ' Sbiancamento campi di filtro -> lato client

        End Select

    End Sub

#End Region

#Region " Datagrid events "

    Private Sub dgrPazienti_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrPazienti.ItemDataBound

        Select Case e.Item.ItemType

            Case ListItemType.Item, ListItemType.AlternatingItem, ListItemType.EditItem, ListItemType.SelectedItem

                If Me.StatoPagina = StatoPaginaMovimenti.Modifica Then
                    Me.DataGridItemDataBound(e.Item, Me.dgrPazienti.EditItemIndex)
                Else
                    Me.AddConfirmClickToImageButton(e.Item, "btnPresaVisione", "Prendere in visione il paziente (se il paziente è DOMICILIATO, verrà anche REGOLARIZZATO)?")
                End If

                ' Visibilità pulsante di edit della riga
                Me.SetEditButtonVisibility(e.Item)

        End Select

    End Sub

    Private Sub dgrPazienti_ItemCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgrPazienti.ItemCommand

        Select Case e.CommandName

            Case "EditRowMovimenti"
                Me.dgrPazientiEditRowMovimenti(e)

            Case "CancelRowMovimenti"
                Me.dgrPazientiCancelRowMovimenti(e)

            Case "UpdateRowMovimenti"
                Me.dgrPazientiUpdateRowMovimenti(e)

            Case "PresaVisione"
                Me.PresaVisionePaziente(e.Item.ItemIndex)
                Me.CaricaDati()

            Case "DatiPaziente"
                Me.RedirectToGestionePaziente(Me.dgrPazienti.Items(e.Item.ItemIndex).Cells(Me.dgrPazienti.getColumnNumberByKey("CodPaziente")).Text)

        End Select

    End Sub

    Private Sub dgrPazientiEditRowMovimenti(e As System.Web.UI.WebControls.DataGridCommandEventArgs)

        Me.EditMovimento(e.Item.Cells(Me.dgrPazienti.getColumnNumberByKey("CodPaziente")).Text)

        ' Datagrid in edit
        Me.dgrPazienti.EditItemIndex = e.Item.ItemIndex

        ' Riesegue la ricerca e il bind del datagrid
        Me.SetFiltriRicerca()
        Me.CaricaDati()

    End Sub

    Private Sub dgrPazientiCancelRowMovimenti(e As System.Web.UI.WebControls.DataGridCommandEventArgs)

        Me.ReloadData()

    End Sub

    Private Sub dgrPazientiUpdateRowMovimenti(e As System.Web.UI.WebControls.DataGridCommandEventArgs)

        Dim codiceStatoAnagraficoOriginale As String = String.Empty

        If e.Item.ItemType = ListItemType.EditItem Then
            codiceStatoAnagraficoOriginale = DirectCast(e.Item.FindControl("lblCodiceStatoAnagraficoEdit"), Label).Text
        End If

        Dim ddlStatoAnagrafico As DropDownList = DirectCast(e.Item.FindControl("ddlStatoAnagrafico"), DropDownList)

        If Me.UpdateStatoAnagrafico(ddlStatoAnagrafico.SelectedValue, codiceStatoAnagraficoOriginale) Then

            Me.ReloadData()

        End If

    End Sub

    Private Sub ReloadData()

        Me.StatoPagina = StatoPaginaMovimenti.Lettura

        ' Datagrid in sola lettura
        Me.dgrPazienti.EditItemIndex = -1

        ' Riesegue la ricerca e il bind del datagrid
        Me.SetFiltriRicerca()
        Me.CaricaDati()

    End Sub

#End Region

#Region " Private "

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()

        listPrintButtons.Add(New PrintButton(Constants.ReportName.ElencoMovimenti, "btnStampaElenco"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Me.tlbMovimenti)

    End Sub

    Private Sub SetFiltriRicerca()

        If Not Me.FiltriRicerca Is Nothing Then

            If Not Me.FiltriRicerca.DataNascitaDa Is Nothing Then
                Me.odpDaNascita.Data = Me.FiltriRicerca.DataNascitaDa
            End If

            If Not Me.FiltriRicerca.DataNascitaA Is Nothing Then
                Me.odpANascita.Data = Me.FiltriRicerca.DataNascitaA
            End If

            If Not Me.FiltriRicerca.DataCnsDa Is Nothing Then
                Me.dpkDaConsultorio.Data = Me.FiltriRicerca.DataCnsDa
            End If

            If Not Me.FiltriRicerca.DataCnsA Is Nothing Then
                Me.dpkAConsultorio.Data = Me.FiltriRicerca.DataCnsA
            End If

            Dim autoMovAdulti As Boolean = False
            Dim autoMovAdultiNo As Boolean = False
            Dim autoMovAdultiIgnora As Boolean = False

            If Not Me.FiltriRicerca.AutoMovAdulti Is Nothing Then
                If Me.FiltriRicerca.AutoMovAdulti Then
                    autoMovAdulti = True
                Else
                    autoMovAdultiNo = True
                End If
            Else
                autoMovAdultiIgnora = True
            End If

            Me.rdbAutoMovAdulti.Checked = autoMovAdulti
            Me.rdbAutoMovAdultiNo.Checked = autoMovAdultiNo
            Me.rdbAutoMovAdultiIgnora.Checked = autoMovAdultiIgnora

        End If

    End Sub

    Private Sub CaricaDati()

        Me.FiltriRicerca = New MovimentiInIngressoFiltriRicerca()

        Dim dt As New DataTable()

        Dim DAM As IDAM = OnVacUtility.OpenDam()

        With DAM.QB

            .NewQuery()

            .AddSelectFields("paz_cognome, paz_nome, paz_data_nascita, san_descrizione as stato_anagrafico,cnm_cns_codice_old, cns_descrizione")
            .AddSelectFields("paz_cns_data_assegnazione, cnm_data, paz_codice, cnm_progressivo")
            .AddSelectFields("cnm_invio_cartella, cnm_auto_adulti, paz_data_aggiornamento, paz_stato_anagrafico")
            .AddSelectFields("paz_indirizzo_residenza, cnm_presa_visione")

            .AddTables("T_CNS_MOVIMENTI, T_PAZ_PAZIENTI, T_ANA_CONSULTORI, T_ANA_STATI_ANAGRAFICI")

            If Me.odpDaNascita.Text <> String.Empty Then
                .AddWhereCondition("paz_data_nascita", Comparatori.MaggioreUguale, Me.odpDaNascita.Data, DataTypes.Data)
                Me.FiltriRicerca.DataNascitaDa = Me.odpDaNascita.Data
            Else
                Me.FiltriRicerca.DataNascitaDa = Nothing
            End If

            If Me.odpANascita.Text <> String.Empty Then
                .AddWhereCondition("paz_data_nascita", Comparatori.MinoreUguale, Me.odpANascita.Data, DataTypes.Data)
                Me.FiltriRicerca.DataNascitaA = Me.odpANascita.Data
            Else
                Me.FiltriRicerca.DataNascitaA = Nothing
            End If

            If Me.dpkDaConsultorio.Text <> String.Empty Then
                .AddWhereCondition("cnm_data", Comparatori.MaggioreUguale, Me.dpkDaConsultorio.Data, DataTypes.Data)
                Me.FiltriRicerca.DataCnsDa = Me.dpkDaConsultorio.Data
            Else
                Me.FiltriRicerca.DataCnsDa = Nothing
            End If

            If Me.dpkAConsultorio.Text <> String.Empty Then
                .AddWhereCondition("cnm_data", Comparatori.MinoreUguale, Me.dpkAConsultorio.Data, DataTypes.Data)
                Me.FiltriRicerca.DataCnsA = Me.dpkAConsultorio.Data
            Else
                Me.FiltriRicerca.DataCnsA = Nothing
            End If

            If Me.rdbAutoMovAdulti.Checked Then
                .AddWhereCondition("cnm_auto_adulti", Comparatori.Uguale, "S", DataTypes.Stringa)
                Me.FiltriRicerca.AutoMovAdulti = True
            ElseIf Me.rdbAutoMovAdultiNo.Checked Then
                .OpenParanthesis()
                .AddWhereCondition("cnm_auto_adulti", Comparatori.Uguale, "N", DataTypes.Stringa)
                .AddWhereCondition("cnm_auto_adulti", Comparatori.Is, "NULL", DataTypes.Replace, "OR")
                .CloseParanthesis()
                Me.FiltriRicerca.AutoMovAdulti = False
            End If

            .AddWhereCondition("cnm_cns_codice_new", Comparatori.Uguale, OnVacUtility.Variabili.CNS.Codice, DataTypes.Stringa)

            .OpenParanthesis()
            .AddWhereCondition("cnm_presa_visione", Comparatori.Uguale, "N", DataTypes.Stringa)
            .AddWhereCondition("cnm_presa_visione", Comparatori.Is, "NULL", DataTypes.Replace, "OR")
            .CloseParanthesis()

            .AddWhereCondition("cnm_paz_codice", Comparatori.Uguale, "paz_codice", DataTypes.Join)
            .AddWhereCondition("cnm_cns_codice_old", Comparatori.Uguale, "cns_codice", DataTypes.OutJoinLeft)
            .AddWhereCondition("paz_stato_anagrafico", Comparatori.Uguale, "san_codice", DataTypes.OutJoinLeft)

        End With

        Try
            DAM.BuildDataTable(dt)
        Finally
            OnVacUtility.CloseDam(DAM)
        End Try

        Dim dv As New DataView(dt)
        dv.Sort = "cnm_data desc,paz_cognome, paz_nome, paz_data_nascita"

        Me.dgrPazienti.DataSource = dv
        Me.dgrPazienti.DataBind()

        Me.divSezioneMovimenti.InnerText = String.Format(" MOVIMENTI: {0} risultat{1}.", dv.Count, IIf(dv.Count = 1, "o", "i"))

    End Sub

    ' Modifica il record nella t_cns_movimenti impostando a S il flag di presa visione del paziente
    Private Sub PresaVisionePaziente(idxRiga As Integer)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizMovimentiInterni As New Biz.BizMovimentiInterniCNS(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)

                bizMovimentiInterni.ImpostaPresaVisionePaziente(Me.dgrPazienti.Items(idxRiga).Cells(Me.dgrPazienti.getColumnNumberByKey("CodPaziente")).Text,
                                                                Me.dgrPazienti.Items(idxRiga).Cells(Me.dgrPazienti.getColumnNumberByKey("Progressivo")).Text)
            End Using

        End Using

    End Sub

#End Region

#Region " Overrides "

    Protected Overrides Sub ImpostaLayoutMovimenti(stato As StatoPaginaMovimenti)

        MyBase.ImpostaLayoutMovimenti(stato)

        Dim abilita As Boolean = (stato = StatoPaginaMovimenti.Lettura)

        ' Toolbar
        Me.tlbMovimenti.Items.FromKeyButton("btnCerca").Enabled = abilita
        Me.tlbMovimenti.Items.FromKeyButton("btnStampaElenco").Enabled = abilita
        Me.tlbMovimenti.Items.FromKeyButton("btnPulisci").Enabled = abilita

        ' Filtri di ricerca
        Me.odpDaNascita.Enabled = abilita
        Me.odpANascita.Enabled = abilita
        Me.dpkDaConsultorio.Enabled = abilita
        Me.dpkAConsultorio.Enabled = abilita
        Me.rdbAutoMovAdulti.Enabled = abilita
        Me.rdbAutoMovAdultiIgnora.Enabled = abilita
        Me.rdbAutoMovAdultiNo.Enabled = abilita

    End Sub

    Protected Overrides Sub EliminaProgrammazioneEffettuata()

        Me.ReloadData()

    End Sub

    Protected Overrides Sub EliminaProgrammazioneNonEffettuata()

        Me.ReloadData()

    End Sub

#End Region

End Class
