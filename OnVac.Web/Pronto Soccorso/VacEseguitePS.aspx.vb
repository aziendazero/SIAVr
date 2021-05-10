Imports Onit.Database.DataAccessManager
Imports Onit.Controls
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities
Imports System.Collections.Generic


Partial Class VacEseguitePS
    Inherits OnVac.Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(sender As System.Object, e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Properties "

    ''' <summary>
    ''' Array di boolean per indicare, per ogni colonna, se l'ordinamento del datatable è ascending o descending.
    ''' No comment.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property ord() As Boolean()
        Get
            Return ViewState("ord")
        End Get
        Set(Value As Boolean())
            ViewState("ord") = Value
        End Set
    End Property

    Private Property dt_vacEseguite() As DataTable
        Get
            If Session("OnVac_dt_vacEseguitePS") Is Nothing Then Return Nothing
            Return DirectCast(Session("OnVac_dt_vacEseguitePS"), SerializableDataTableContainer).Data
        End Get
        Set(Value As DataTable)
            If Session("OnVac_dt_vacEseguitePS") Is Nothing Then
                Session("OnVac_dt_vacEseguitePS") = New SerializableDataTableContainer()
            End If
            DirectCast(Session("OnVac_dt_vacEseguitePS"), SerializableDataTableContainer).Data = Value
        End Set
    End Property

    Private Property sectionShow() As Sections
        Get
            Return ViewState("OnVac_sectionShow")
        End Get
        Set(Value As Sections)
            ViewState("OnVac_sectionShow") = Value
        End Set
    End Property

    Private _CodiceMedicoUtenteLoggato As String
    Private ReadOnly Property CodiceMedicoUtenteLoggato() As String
        Get
            If _CodiceMedicoUtenteLoggato Is Nothing Then

                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                    _CodiceMedicoUtenteLoggato = genericProvider.Utenti.GetMedicoDaUtente(OnVacContext.UserId)
                End Using

            End If

            Return _CodiceMedicoUtenteLoggato
        End Get
    End Property

    Private ReadOnly Property UtenteLoggatoIsMedico() As Boolean
        Get
            Return Not String.IsNullOrEmpty(Me.CodiceMedicoUtenteLoggato)
        End Get
    End Property

    Protected ReadOnly Property LoadLeftFramePS() As Boolean
        Get
            Dim loadLeftFrame As String = Me.Request.QueryString("LoadLeftFramePS")
            If loadLeftFrame = Nothing Then Return False
            Return Boolean.Parse(loadLeftFrame)
        End Get
    End Property

#End Region

#Region " Enum "

    Enum Sections
        VacEseguiteList
        ReazioniAvverseDetail
    End Enum

#End Region

#Region " Public "

    Public strJS As String

#End Region

#Region " Page Events "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack() Then

            OnVacUtility.ImpostaIntestazioniPagina(Me.OnitLayout31, Me.LayoutTitolo, Nothing, Me.Settings, Me.IsGestioneCentrale)
            LoadData()

        End If

        ReloadSection()

    End Sub

#End Region

#Region " Datagrid Events "

    Private Sub dg_vacEseguite_SortCommand(source As Object, e As System.Web.UI.WebControls.DataGridSortCommandEventArgs) Handles dg_vacEseguite.SortCommand

        Select Case e.SortExpression

            Case "vac_descrizione"

                ord(0) = Not ord(0)
                SetOrderBy(ord(0), e.SortExpression, "ordDesc")

            Case "ves_n_richiamo"

                ord(1) = Not ord(1)
                SetOrderBy(ord(1), e.SortExpression, "ordDosi")

            Case "ves_data_effettuazione"

                ord(2) = Not ord(2)
                SetOrderBy(ord(2), e.SortExpression, "ordData")

            Case "ves_lot_codice"

                ord(3) = Not ord(3)
                SetOrderBy(ord(3), e.SortExpression, "ordLotto")

            Case "noc_descrizione"

                ord(4) = Not ord(4)
                SetOrderBy(ord(4), e.SortExpression, "ordNC")

            Case "ass_descrizione"

                ord(5) = Not ord(5)
                SetOrderBy(ord(5), e.SortExpression, "ordAss", "ass_descrizione")

            Case "ope_nome"

                ord(6) = Not ord(6)
                SetOrderBy(ord(6), e.SortExpression, "ordOp")

            Case "cns_descrizione"

                ord(7) = Not ord(7)
                SetOrderBy(ord(7), e.SortExpression, "ordCNS")

            Case "sii_descrizione"

                ord(8) = Not ord(8)
                SetOrderBy(ord(8), e.SortExpression, "ordSII")

            Case "ute_descrizione"

                ord(9) = Not ord(9)
                SetOrderBy(ord(9), e.SortExpression, "ordUtente", "ute_descrizione")

        End Select

        Me.dg_vacEseguite.DataSource = Me.dt_vacEseguite.DefaultView
        Me.dg_vacEseguite.DataBind()

        StrJSNascondiReaz()

    End Sub

    Private Sub SetOrderBy(ascendingOrder As Boolean, sortExpression As String, idOrdField As String)

        SetOrderBy(ascendingOrder, sortExpression, idOrdField, "vac_descrizione")

    End Sub

    Private Sub SetOrderBy(ascendingOrder As Boolean, sortExpression As String, idOrdField As String, altricampiOrderBy As String)

        Dim versoOrdinamento As String
        Dim upDown As String

        If ascendingOrder Then
            versoOrdinamento = "ASC"
            upDown = "up"
        Else
            versoOrdinamento = "DESC"
            upDown = "down"
        End If

        Me.dt_vacEseguite.DefaultView.Sort = String.Format("{0} {1}, vac_obbligatoria, {2}", sortExpression, versoOrdinamento, altricampiOrderBy)

        strJS &= String.Format("document.getElementById('{0}').style.display = 'inline'; document.getElementById('{0}').src = '../images/arrow_{1}_small.gif';", idOrdField, upDown)
        strJS &= Environment.NewLine

    End Sub

    Private Sub dg_vacEseguite_ItemCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_vacEseguite.ItemCommand

        If e.CommandName = "ReazioneCmd" Then

            Dim key(2) As Object
            key(0) = DirectCast(e.Item.FindControl("tb_codVac"), Label).Text
            key(1) = DirectCast(e.Item.FindControl("tb_n_rich"), Label).Text
            key(2) = Date.Parse(DirectCast(e.Item.FindControl("tb_data_eff"), Label).Text)

            Dim row As DataRow = Me.dt_vacEseguite.Rows.Find(key)

            Me.ReazAvverseDetail.StatoControlloCorrente = OnVac.ReazAvverseDetail.StatoControllo.SolaLettura

            Me.ReazAvverseDetail.LoadDataIntoControl(Me.ReazAvverseDetail.CreateListEseguiteReazioniSelezionate(Nothing, row, Me.IsGestioneCentrale))

            StrJSNascondiVac()

        End If

    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btn_Aggiorna"

                LoadData()

            Case "btn_Indietro"

                StrJSNascondiReaz()

            Case "btn_Esclusione"

                CaricaEscluse()
                modEsclusioni.VisibileMD = True

            Case "btn_CertificatoVaccinale"

                OnVacUtility.StampaCertificatoVaccinale(Page, Settings, True, False)

        End Select

    End Sub

#End Region

#Region " OnitLayout Events "

    Private Sub OnitLayout31_AlertClick(sender As Object, e As Onit.Controls.PagesLayout.OnitLayout3.AlertEventArgs) Handles OnitLayout31.AlertClick

        Select Case e.Key

            Case "A"
                Response.Redirect("RicPazPS.aspx")

        End Select

    End Sub

#End Region

#Region " Private/Protected Methods "

    Private Sub LoadData()

        ord = New Boolean() {True, True, True, True, True, True, True, True, True, True}

        If Not Me.dt_vacEseguite Is Nothing Then Me.dt_vacEseguite.Dispose()
        Me.dt_vacEseguite = New DataTable()

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using biz As New Biz.BizVaccinazioniEseguite(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                Me.dt_vacEseguite = biz.GetVaccinazioniEseguite(OnVacUtility.Variabili.PazId, Me.IsGestioneCentrale)

            End Using
        End Using

        Me.dg_vacEseguite.DataSource = Me.dt_vacEseguite
        Me.dg_vacEseguite.DataBind()

        OnVacUtility.addKey(Me.dt_vacEseguite, "ves_vac_codice", "ves_n_richiamo", "ves_data_effettuazione")

        sectionShow = Sections.VacEseguiteList

        If Not Me.UtenteLoggatoIsMedico AndAlso Me.dt_vacEseguite.Rows.Count = 0 Then

            Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Nessun dato disponibile a fronte del paziente selezionato", "A", False, False))

        End If

    End Sub

    Private Sub CaricaEscluse()

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizEscluse As New Biz.BizVaccinazioniEscluse(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                dt_vacExPS = bizEscluse.GetVaccinazioniEscluse(OnVacUtility.Variabili.PazId, Me.IsGestioneCentrale)

            End Using
        End Using

        Dim col As New DataColumn("s", GetType(Boolean))
        dt_vacExPS.Columns.Add(col)
        dt_vacExPS.AcceptChanges()

        SetFlagScadenza()

        dg_vacEx.DataSource = dt_vacExPS.DefaultView
        dg_vacEx.DataBind()

        SetDataTablePrimaryKey()

        ' Se non è gestito lo storico centralizzato, le colonne usl di appartenenza e flag visibilità non devono essere visibili
        'SetColumnVisibility("usl_inserimento_vex_descr", FlagAbilitazioneVaccUslCorrente)
        'SetColumnVisibility("vex_flag_visibilita", FlagAbilitazioneVaccUslCorrente)

    End Sub

    Private Sub SetDataTablePrimaryKey()

        If Me.IsGestioneCentrale Then
            OnVacUtility.addKey(dt_vacExPS, "vex_vac_codice", "vex_id")
        Else
            OnVacUtility.addKey(dt_vacExPS, "vex_vac_codice")
        End If

    End Sub
    Public Property dt_vacExPS() As DataTable
        Get
            Return Session("OnVac_dt_vacExPS")
        End Get
        Set(Value As DataTable)
            Session("OnVac_dt_vacExPS") = Value
        End Set
    End Property
    Private Sub SetFlagScadenza()

        Dim row As DataRow

        For Each row In Me.dt_vacExPS.Rows
            If (row("vex_data_scadenza") Is DBNull.Value OrElse row("vex_data_scadenza") = DateTime.MinValue OrElse row("vex_data_scadenza") > Date.Now) Then
                row("s") = False
            Else
                row("s") = True
            End If
        Next

        Me.dt_vacExPS.AcceptChanges()

    End Sub
    Sub ReloadSection()

        Select Case sectionShow

            Case Sections.ReazioniAvverseDetail
                StrJSNascondiVac()

            Case Sections.VacEseguiteList
                StrJSNascondiReaz()

        End Select

    End Sub

    Private Sub StrJSNascondiVac()

        strJS &= "document.getElementById('divLegenda').style.display='none';" + Environment.NewLine
        strJS &= "document.getElementById('pan_VacEs').style.display='none';" + Environment.NewLine
        strJS &= "document.getElementById('pan_ReazAvv').style.display='';" + Environment.NewLine
        strJS &= "document.getElementById('LayoutTitolo_sezione').firstChild.nodeValue='REAZIONE AVVERSA'" + Environment.NewLine

        sectionShow = Sections.ReazioniAvverseDetail

        Me.ToolBar.Items.FromKeyButton("btn_Aggiorna").Enabled = False
        Me.ToolBar.Items.FromKeyButton("btn_Indietro").Enabled = True

    End Sub

    Private Sub StrJSNascondiReaz()

        strJS &= "document.getElementById('divLegenda').style.display='';" + Environment.NewLine
        strJS &= "document.getElementById('pan_ReazAvv').style.display='none';" + Environment.NewLine
        strJS &= "document.getElementById('pan_VacEs').style.display='';" + Environment.NewLine
        strJS &= "document.getElementById('LayoutTitolo_sezione').firstChild.nodeValue='ELENCO VACCINAZIONI'" + Environment.NewLine

        sectionShow = Sections.VacEseguiteList

        Me.ToolBar.Items.FromKeyButton("btn_Aggiorna").Enabled = True
        Me.ToolBar.Items.FromKeyButton("btn_Indietro").Enabled = False

    End Sub

    Private Sub AddReazAvv()

        StrJSNascondiVac()

    End Sub

    ' Richiamata nell'aspx, quando viene fatto il bound del dg_vacEseguite
    Protected Function GetDescrizioneLuogo(codiceLuogo As String)

        Return OnVacUtility.GetDescrizioneLuogo(codiceLuogo, Me.Settings)

    End Function

    Protected Function ShowLeftFrameIfNeeded() As String

        ' N.B. : LoadLeftFramePS è sempre false, la left non viene mai aperta
        If Not Me.IsPostBack AndAlso Me.LoadLeftFramePS Then
            Return Me.GetOpenLeftFrameScript(True)
        End If

        Return String.Empty

    End Function

    Protected Function BindFlagVisibilitaImageUrlValue(dataItem As Object) As String

        Return Common.OnVacStoricoVaccinaleCentralizzato.GetImageUrlFlagVisibilita(dataItem, "VEX_FLAG_VISIBILITA", Me)

    End Function

    Protected Function BindFlagVisibilitaToolTipValue(dataItem As Object) As String

        Return Common.OnVacStoricoVaccinaleCentralizzato.GetToolTipFlagVisibilita(dataItem, "VEX_FLAG_VISIBILITA")

    End Function

#End Region

End Class
