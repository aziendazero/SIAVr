Imports System.Collections.Generic
Imports Onit.Controls
Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.Biz
Imports Onit.OnAssistnet.OnVac.Biz.BizVaccinazioniEseguite
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.Web.UI.WebControls.Validators


Partial Class OnVac_VacProg
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

#Region " Public Variables "

    Public strJS As String

#End Region

#Region " Consts "

    Private Const WARNING_DGR_EDIT_MODE As String = "Premere AGGIORNA o ANNULLA della riga che si sta modificando prima di effettuare questa operazione!"

#End Region

#Region " Types "

    Private Class StatoVaccinazione
        Public Const ESEGUITA As String = "eseguita"
        Public Const ESCLUSA As String = "esclusa"
    End Class

    Private Class NomeFlagTipoValorizzazione
        Public Const SITO As String = "FlagSito"
        Public Const VIA As String = "FlagVia"
    End Class

    Private Enum TipoRichiamoMedLogin
        IMPOSTA = 0
        MODIFICA_MEDRESP = 1
        MODIFICA_MEDVAC = 2
        MODIFICA_DATA = 3
    End Enum

    Private Enum DgrVacProgColumnIndex
        CheckBox = 0
        BtnElimina = 1
        BtnModificaConferma = 2
        Associazione = 3
        VacObbligatoria = 4
        ReazioneAvversa = 5
        Vaccinazione = 6
        DoseVacc = 7
        Lotto = 8
        NomeCommerciale = 9
        ViaSomministrazione = 10
        SitoInoculazione = 11
        CondizioneSanitaria = 12
        CondizioneRischio = 13
        BtnPagamento = 14
        Note = 15
        InfoVaccinazione = 16
        VacEseguitaEsclusa = 17
        Ciclo = 18                      ' visible=false
        Seduta = 19                     ' visible=false
        DoseAss = 20                    ' visible=false
        Vaccinatore = 21                ' visible=false
    End Enum

    Private Enum TabIndexConvocazione
        Convocazione = 0
        Esecuzione = 1
        Pagamento = 2
    End Enum

    Private Class TabKeyRitardi
        Public Const TabRitardiVaccinazioni As String = "tabRitardiVacc"
        Public Const TabRitardiBilanci As String = "tabRitardiBilanci"
    End Class

#End Region

#Region " Proprietà "

    Public ReadOnly Property DataConvocazione() As DateTime
        Get
            Return DateTime.Parse(Request.QueryString("DataCnv"))
        End Get
    End Property

    Public ReadOnly Property EditConvocazioni() As Boolean
        Get
            Return Boolean.Parse(Request.QueryString("EditCnv"))
        End Get
    End Property

    Public ReadOnly Property AutoConvocazioni() As Boolean
        Get
            Return Boolean.Parse(Request.QueryString("AutoCnv"))
        End Get
    End Property

    Public Property VaccinazioniEditable() As Boolean
        Get
            Return ViewState("VACEDTBL")
        End Get
        Set(value As Boolean)
            ViewState("VACEDTBL") = value
        End Set
    End Property

    Public Property CodiceLottiUtilizzati() As List(Of String)
        Get
            If ViewState("CLU") Is Nothing Then ViewState("CLU") = New List(Of String)
            Return DirectCast(ViewState("CLU"), List(Of String))
        End Get
        Set(Value As List(Of String))
            ViewState("CLU") = Value
        End Set
    End Property

    Property dt_vacEs() As DataTable
        Get
            Return Session("OnVac_dt_vacEs")
        End Get
        Set(Value As DataTable)
            Session("OnVac_dt_vacEs") = Value
        End Set
    End Property

    Property dt_bilProg() As DataTable
        Get
            If Session("OnVac_dt_bilProg") Is Nothing Then Return Nothing
            Return DirectCast(Session("OnVac_dt_bilProg"), SerializableDataTableContainer).Data
        End Get
        Set(Value As DataTable)
            If Session("OnVac_dt_bilProg") Is Nothing Then
                Session("OnVac_dt_bilProg") = New SerializableDataTableContainer
            End If
            DirectCast(Session("OnVac_dt_bilProg"), SerializableDataTableContainer).Data = Value
        End Set
    End Property

    Public Property dt_vacProg() As DataTable
        Get
            If Session("OnVac_dt_vacProg") Is Nothing Then Return Nothing
            Return DirectCast(Session("OnVac_dt_vacProg"), SerializableDataTableContainer).Data
        End Get
        Set(Value As DataTable)
            If Session("OnVac_dt_vacProg") Is Nothing Then
                Session("OnVac_dt_vacProg") = New SerializableDataTableContainer
            End If
            DirectCast(Session("OnVac_dt_vacProg"), SerializableDataTableContainer).Data = Value
        End Set
    End Property

    ' Restituisce il valore del campo "dose" in edit
    Protected ReadOnly Property doseVacProg() As String
        Get
            If Not dg_vacProg Is Nothing Then
                If dg_vacProg.EditItemIndex > -1 Then
                    Dim txt As TextBox = GetTextBoxControlFromDgVacProg(dg_vacProg.EditItemIndex, "txtNumRich")
                    If Not txt Is Nothing Then
                        Return txt.Text
                    End If
                End If
            End If
            Return String.Empty
        End Get
    End Property

    ' Restituisce il ClientID del campo NumRich
    Protected ReadOnly Property ClientIDTxtNumRich() As String
        Get
            If Not dg_vacProg Is Nothing Then
                If dg_vacProg.EditItemIndex > -1 Then
                    Dim txt As TextBox = GetTextBoxControlFromDgVacProg(dg_vacProg.EditItemIndex, "txtNumRich")
                    If Not txt Is Nothing Then
                        Return txt.ClientID
                    End If
                End If
            End If
            Return String.Empty
        End Get
    End Property

    Protected Property jsLottiAssociazioniSingole() As String
        Get
            Return ViewState("OnVac_Lotti_Ass_Singole")
        End Get
        Set(Value As String)
            ViewState("OnVac_Lotti_Ass_Singole") = Value
        End Set
    End Property

    Private Property VieSomministrazione As List(Of Entities.ViaSomministrazione)
        Get
            If ViewState("VII") Is Nothing Then ViewState("VII") = New List(Of Entities.ViaSomministrazione)()
            Return ViewState("VII")
        End Get
        Set(Value As List(Of Entities.ViaSomministrazione))
            ViewState("VII") = Value
        End Set
    End Property

    Private Property SitiInoculazione As List(Of Entities.SitoInoculazione)
        Get
            If ViewState("SII") Is Nothing Then ViewState("SII") = New List(Of Entities.SitoInoculazione)()
            Return ViewState("SII")
        End Get
        Set(Value As List(Of Entities.SitoInoculazione))
            ViewState("SII") = Value
        End Set
    End Property

    Private ReadOnly Property DescrizioneMalattiaDefault As String
        Get
            If ViewState("DescrMalDefault") Is Nothing Then
                Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                    Using bizMalattie As New BizMalattie(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                        ViewState("DescrMalDefault") = bizMalattie.GetDescrizioneMalattia(Settings.CONDIZIONE_SANITARIA_DEFAULT)
                    End Using
                End Using
            End If
            Return ViewState("DescrMalDefault").ToString()
        End Get
    End Property

    Private ReadOnly Property DescrizioneCategoriaRischioDefault As String
        Get
            If ViewState("DescrRscDefault") Is Nothing Then
                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                    Using bizRischio As New BizCategorieRischio(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                        ViewState("DescrRscDefault") = bizRischio.GetDescrizioneCategoriaRischio(Settings.CONDIZIONE_RISCHIO_DEFAULT)
                    End Using
                End Using
            End If
            Return ViewState("DescrRscDefault").ToString()
        End Get
    End Property

#Region " Paziente "

    Private Property EtaPaziente() As Double
        Get
            Return ViewState("etaPaz")
        End Get
        Set(Value As Double)
            ViewState("etaPaz") = Value
        End Set
    End Property

    Private Property SessoPaziente() As String
        Get
            Return ViewState("sessoPaz")
        End Get
        Set(Value As String)
            ViewState("sessoPaz") = Value
        End Set
    End Property

    Public Property StatusVaccinale() As String
        Get
            Return ViewState("VacProg_StatusVaccinale")
        End Get
        Set(Value As String)
            ViewState("VacProg_StatusVaccinale") = Value
        End Set
    End Property

    ''' <summary>
    ''' Memorizza il risultato della query per controllare se il paziente è deceduto oppure no
    ''' </summary>
    ''' <returns></returns>
    Public Property PazienteDeceduto() As Boolean
        Get
            Return ViewState("OnVac_Paz_Deceduto")
        End Get
        Set(Value As Boolean)
            ViewState("OnVac_Paz_Deceduto") = Value
        End Set
    End Property

    ''' <summary>
    ''' Lista di malattie associate al paziente filtrate per una delle tipologie impostate nel parametro CONDIZIONE_SANITARIA_TIPOLOGIE_MALATTIA.
    ''' E' una lista di elementi (codice malattia - codice vaccinazione).
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property CondizioniSanitariePaziente As List(Of Entities.CondizioneSanitaria)
        Get
            If ViewState("CSPAZ") Is Nothing Then ViewState("CSPAZ") = New List(Of Entities.CondizioneSanitaria)()
            Return DirectCast(ViewState("CSPAZ"), List(Of Entities.CondizioneSanitaria))
        End Get
        Set(Value As List(Of Entities.CondizioneSanitaria))
            ViewState("CSPAZ") = Value
        End Set
    End Property

    ''' <summary>
    ''' Condizione di rischio associata al paziente
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property CondizioniRischioPaziente As List(Of Entities.CondizioneRischio)
        Get
            If ViewState("CRPAZ") Is Nothing Then ViewState("CRPAZ") = New List(Of Entities.CondizioneRischio)()
            Return DirectCast(ViewState("CRPAZ"), List(Of Entities.CondizioneRischio))
        End Get
        Set(Value As List(Of Entities.CondizioneRischio))
            ViewState("CRPAZ") = Value
        End Set
    End Property

#End Region

#Region " Gestione Icone per Note Vaccinazioni "

    Private ReadOnly Property IconaNotaSiEnabled() As String
        Get
            Return "NotaSi.gif"
        End Get
    End Property

    Private ReadOnly Property IconaNotaSiDisabled() As String
        Get
            Return "NotaSi_dis.gif"
        End Get
    End Property

    Private ReadOnly Property IconaNotaNoEnabled() As String
        Get
            Return "NotaNo.gif"
        End Get
    End Property

    Private ReadOnly Property IconaNotaNoDisabled() As String
        Get
            Return "NotaNo_dis.gif"
        End Get
    End Property

    Protected ReadOnly Property UrlIconaNotaSi(enabled As Boolean) As String
        Get
            If enabled Then
                Return "~/images/" + IconaNotaSiEnabled
            Else
                Return "~/images/" + IconaNotaSiDisabled
            End If
        End Get
    End Property

    Protected ReadOnly Property UrlIconaNotaNo(enabled As Boolean) As String
        Get
            If enabled Then
                Return "~/images/" + IconaNotaNoEnabled
            Else
                Return "~/images/" + IconaNotaNoDisabled
            End If
        End Get
    End Property

#End Region

#End Region

#Region " Overrides "

    Protected Overrides Sub OnInit(e As EventArgs)

        MyBase.OnInit(e)

        ' Visualizzazione dati pagamento
        TabConvocazione.Tabs(TabIndexConvocazione.Pagamento).Visible = Settings.PAGAMENTO
        dg_vacProg.Columns(DgrVacProgColumnIndex.BtnPagamento).Visible = Settings.PAGAMENTO

        If Settings.PAGAMENTO Then

            Dim listTipiPagamento As List(Of Entities.TipiPagamento) = Nothing

            Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                Using bizNomiCommerciali As New BizNomiCommerciali(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                    listTipiPagamento = bizNomiCommerciali.GetListTipiPagamento()

                End Using
            End Using

            ddlTipiPagVac.Items.Clear()

            If Not listTipiPagamento Is Nothing AndAlso listTipiPagamento.Count > 0 Then

                For Each tipoPagamento As Entities.TipiPagamento In listTipiPagamento

                    ddlTipiPagVac.Items.Add(New ListItem(tipoPagamento.Descrizione, tipoPagamento.GuidPagamento.ToString()))

                Next

            End If

        End If

        dg_vacProg.Columns(DgrVacProgColumnIndex.NomeCommerciale).Visible = Settings.VACPROG_NOMECOMMERCIALE

        'gestione della scelta della modalità di accesso
        tbModalitaAccesso.Visible = Settings.GESMODALITAACCESSO

        'gestione della scelta della vaccinazione in campagna
        lblInCampagna.Visible = Settings.GESVACCAMPAGNA
        chkInCampagna.Visible = Settings.GESVACCAMPAGNA

        'Visualizzazione Tab Ritardi Bilanci
        Dim tabIndex As Short = TabRitardi.Tabs.Count - 1

        For i As Short = 0 To TabRitardi.Tabs.Count - 1
            If Not TabRitardi.Tabs(i).Key Is Nothing AndAlso TabRitardi.Tabs(i).Key = TabKeyRitardi.TabRitardiBilanci Then
                tabIndex = i
            End If
        Next

        If Settings.GESBIL Then
            TabRitardi.Tabs(tabIndex).Visible = True
        Else
            TabRitardi.Tabs(tabIndex).Visible = False
            ToolBar.Items.FromKeyButton("btn_AddBil").Visible = False
        End If

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            ' Caricamento vie di somministrazione
            Using bizVieSomministrazione As New BizVieSomministrazione(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                VieSomministrazione = bizVieSomministrazione.GetVieSomministrazione()
            End Using

            ' Caricamento siti di inoculazione
            Using bizSitiInoculazione As New BizSitiInoculazione(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                SitiInoculazione = bizSitiInoculazione.GetSitiInoculazione()
            End Using

            ' Caricamento condizioni sanitarie del paziente 
            Using bizMalattie As New BizMalattie(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                CondizioniSanitariePaziente = bizMalattie.GetCondizioniSanitariePaziente(OnVacUtility.Variabili.PazId)
            End Using

            ' Caricamento condizione di rischio associata al paziente
            Using bizRischio As New BizCategorieRischio(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                CondizioniRischioPaziente = bizRischio.GetCondizioniRischioPaziente(OnVacUtility.Variabili.PazId)
            End Using

        End Using

        ' Selezione tab dati di esecuzione
        TabConvocazione.SelectedTabIndex = TabIndexConvocazione.Esecuzione

    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)

        MyBase.OnLoad(e)

        If Not IsPostBack() Then

            ' aggiunta del consultorio di lavoro al titolo e impostazione dati del paziente
            OnVacUtility.ImpostaIntestazioniPagina(OnitLayout31, LayoutTitolo, Nothing, Settings, IsGestioneCentrale)

            ' Visualizzazione e valorizzazione flag visibilità dati vaccinali
            chkFlagVisibilita.Visible = True
            chkFlagVisibilita.Checked = Common.OnVacStoricoVaccinaleCentralizzato.IsVisibilitaCentraleDatiVaccinaliPaziente(OnVacUtility.Variabili.PazId, Settings)

            Dim visualizzaModaleLogin As Boolean = False
            Dim ambulatoriInConsultorio As Boolean = True

            If EditConvocazioni Then
                '--
                ' Se MEDVACLOGIN è "N" non viene mai chiesto il medico vaccinatore
                ' 
                ' Aggiunto il parametro VACPROG_SETVACCINATORE per gestire la richiesta di inserimento di un vaccinatore 
                ' tutte le volte o una sola volta per seduta.
                ' Se il parametro è N, chiede il vaccinatore per ogni paziente
                ' Se il parametro è S, chiede il vaccinatore solo se non è stato impostato in precedenza
                ' 
                ' Se l'ambulatorio non è selezionato chiede di impostarlo
                '--
                ' Medico vaccinatore
                '--
                If Settings.MEDVACLOGIN Then
                    '-- 
                    If String.IsNullOrEmpty(OnVacUtility.Variabili.MedicoVaccinante.Codice) Then
                        '--
                        visualizzaModaleLogin = True
                        '--
                    Else
                        '--
                        If Not Settings.VACPROG_SETVACCINATORE Then
                            '--
                            If OnVacUtility.Variabili.PazId <> OnVacUtility.Variabili.PazIdPrecedente Then
                                '--
                                visualizzaModaleLogin = True
                                '--
                            End If
                            '--
                            OnVacUtility.Variabili.PazIdPrecedente = OnVacUtility.Variabili.PazId
                            '--
                        End If
                        '--
                    End If
                    '--
                End If
                '--
                ' Medico responsabile                '--
                If Settings.MEDLOGIN Then
                    '--
                    txtMedicoResponsabile.Obbligatorio = True
                    '--
                    If String.IsNullOrEmpty(OnVacUtility.Variabili.MedicoResponsabile.Codice) Then
                        '--
                        visualizzaModaleLogin = True
                        '--
                    End If
                    '--
                End If
                '--
                ' Ambulatorio
                If OnVacUtility.Variabili.CNS.Ambulatorio.Codice = 0 Then
                    '--
                    Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                        Using bizConsultori As New BizConsultori(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                            '--
                            Dim ambulatorioDefault As Entities.Ambulatorio = bizConsultori.GetAmbulatorioDefault(OnVacUtility.Variabili.CNS.Codice, True, False)
                            '--
                            If ambulatorioDefault Is Nothing Then
                                '--
                                ambulatoriInConsultorio = False
                                visualizzaModaleLogin = True
                                '--
                            Else
                                '--
                                If ambulatorioDefault Is Nothing OrElse ambulatorioDefault.Codice = 0 Then
                                    '--
                                    visualizzaModaleLogin = True
                                    '--
                                Else
                                    '--
                                    OnVacUtility.Variabili.CNS.Ambulatorio.Codice = ambulatorioDefault.Codice
                                    OnVacUtility.Variabili.AMBConvocazione = ambulatorioDefault.Codice
                                    '--
                                End If
                                '--
                            End If
                            '--
                        End Using
                    End Using
                    '--
                End If
                '--
                VaccinazioniEditable = Not visualizzaModaleLogin
                '--
            End If

            LoadData(True)

            chkInCampagna.Enabled = VaccinazioniEditable

            EnableDdlEseMalPag(VaccinazioniEditable)
            EnableImgBtnMedLogin(EditConvocazioni)

            If visualizzaModaleLogin Then
                ApriModaleMedLogin(TipoRichiamoMedLogin.IMPOSTA)
            Else
                If ambulatoriInConsultorio Then ControllaGestioneBarCodeLotti()
            End If

            ' Mostra/nasconde il pulsante di stampa del certificato vaccinale dalla toolbar
            ShowPrintButtons()

        End If

        strJS &= AddJSCheckDose()

    End Sub

    Protected Overrides Sub OnPreRender(e As EventArgs)

        MyBase.OnPreRender(e)

        OnitLayout31.InsertRoutineJS(strJS)

    End Sub

#End Region

#Region " Events Handlers "

#Region " Toolbar "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked
        '--
        Select Case e.Button.Key
            '--
            Case "btn_Cnv"
                '--
                Me.RedirectToConvocazioniPaziente(OnVacUtility.Variabili.PazId, Me.EditConvocazioni, True, Nothing, Nothing)
                '--
            Case "btn_Salva"
                '--
                If Me.bilanciEsclusi() Then
                    '--
                    Me.OnitLayout31.ShowMsgBox(New PagesLayout.OnitLayout3.OnitLayoutMsgBox("Effettuando il salvataggio, i bilanci esclusi non verranno più programmati!\nProcedere comunque?", "escludiBilancio", True, True))
                    '--
                Else
                    '--
                    If Me.Salva() Then
                        '--
                        Me.LoadData(False)
                        '--
                    End If
                    '--
                End If
                '--
            Case "btn_Annulla"
                '--
                Me.LoadData(False)
                '--
                Me.ControllaStatoConvocazione()
                '--
            Case "btn_Esegui"
                '--
                Dim mostraModale As Boolean = False
                Dim lottiGiaInseriti As New List(Of String)()
                Dim codiciLottiAssSingola As New List(Of String)()
                '--
                Me.cpag.Controls.Clear()
                '--
                For i As Integer = 0 To Me.dg_vacProg.Items.Count - 1
                    '--
                    If DirectCast(Me.dg_vacProg.Items(i).FindControl("cb"), CheckBox).Checked Then
                        '--
                        Dim codiceLotto As String = DirectCast(Me.dg_vacProg.Items(i).FindControl("lblLotto"), Label).Text
                        '--
                        Dim codiciAssociazioniLotto As String() = Me.CaricaCodiciAssociazioniLotto(codiceLotto)
                        '--
                        If codiciAssociazioniLotto.Length > 1 Then
                            '--
                            ' Lotto con più valAssVac (bisogna aprire la modale con lo user control di scelta multipla)
                            '--
                            If Not lottiGiaInseriti.Contains(codiceLotto) Then
                                '--
                                Dim ucScegliAss As ScegliAss = Page.LoadControl("ScegliAss.ascx")
                                '--
                                ucScegliAss.inLotto = codiceLotto
                                ucScegliAss.inNomeCommerciale = DirectCast(Me.dg_vacProg.Items(i).FindControl("lblDescNC"), Label).Text
                                ucScegliAss.codAssDefault = DirectCast(Me.dg_vacProg.Items(i).FindControl("lblCodAss"), Label).Text
                                '-- 
                                Me.cpag.Controls.Add(ucScegliAss)
                                '--
                                lottiGiaInseriti.Add(codiceLotto)
                                '--
                                mostraModale = True
                                '--
                            End If
                            '--
                        Else
                            '--
                            codiciLottiAssSingola.Add(codiceLotto)
                            '--
                        End If
                        '--                          
                    End If
                    '--
                Next
                '--
                Dim lottiAssociazioniSingole As String = String.Empty
                '--
                If codiciLottiAssSingola.Count > 0 Then
                    '--
                    Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                        Using bizLotti As New Biz.BizLotti(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                            '--
                            lottiAssociazioniSingole = bizLotti.GetStringLottiAssociazioni(codiciLottiAssSingola)
                            '--
                        End Using
                    End Using
                    '--
                End If
                '--
                Me.jsLottiAssociazioniSingole = lottiAssociazioniSingole
                '--
                If mostraModale Then
                    Me.ofmSceltaAssociazioni.VisibileMD = True
                Else
                    Me.Esegui()
                End If
                '--
            Case "btn_Escludi"
                '--
                Me.ShowEscludiVac()
                '--
            Case "btn_AssLotti"
                '--
                Me.ShowAssLotti()
                '--
            Case "btn_InsLotto"
                '--
                Me.ShowInsLotto()
                '--
            Case "btn_InsAssociazione"
                '--
                Me.ShowInserisciAssociazione()
                '--
            Case "btn_AddBil"
                '--
                Dim bilanci As Collection.BilancioProgrammatoCollection = CaricaBilanci()
                '--
                If bilanci.Count > 0 Then
                    '--
                    Dim minId As Object = dt_bilProg.Compute("min(ID)", String.Empty)
                    '--
                    Dim r As DataRow = dt_bilProg.NewRow()
                    '--
                    If minId Is DBNull.Value OrElse minId > -1 Then
                        r("ID") = -1
                    Else
                        r("ID") = minId - 1
                    End If
                    '--
                    Me.dt_bilProg.Rows.Add(r)
                    '--
                    Me.dg_bilProg.EditItemIndex = dg_bilProg.Items.Count
                    '--
                    Me.BindDGBilProgrammati()
                    '--
                    Dim ddl As DropDownList = DirectCast(dg_bilProg.Items(dg_bilProg.EditItemIndex).FindControl("cmbBilancio"), DropDownList)
                    '--
                    ddl.DataSource = bilanci
                    ddl.DataTextField = "MalDescNBil"
                    ddl.DataValueField = "NBilNMal"
                    ddl.DataBind()
                    '--
                    If Me.pan_legBil.Visible = False Then
                        Me.pan_legBil.Visible = True
                        Me.LayoutTitolo_bil.Visible = True
                        Me.dg_bilProg.Visible = True
                    End If
                    '--
                    Me.EnableToolbar(True)
                    '--
                Else
                    '--
                    strJS &= "alert(""Nessun bilancio disponibile per l'inserimento"")" & vbCrLf
                    '--
                End If
                '--
            Case "btn_CertificatoVaccinale"
                '--
                OnVacUtility.StampaCertificatoVaccinale(Page, Settings, True, False)
                '--
            Case "btn_VisioneBilanci"

                RedirectToVisioneBilanci()

        End Select
        '--
    End Sub

#End Region

#Region " Login Medico "

    Private Sub btnOKLogin_Click(sender As Object, e As EventArgs) Handles btnOKLogin.Click
        '--
        Dim checkGestioneBarCodeLotti As Boolean = False
        '--
        Dim medicoVaccinatoreChanged As Boolean = False
        Dim dataEsecuzioneChanged As Boolean = False
        Dim vaccinazioniEditableChanged As Boolean = False
        '--
        ' Controllo campi non impostati nello user control di selezione del medico e dell'ambulatorio
        Dim listCampiNonImpostati As New List(Of String)()
        '--
        If Not Me.uscScegliAmb.IsValid Then
            listCampiNonImpostati.Add("ambulatorio")
        End If
        '--
        If Settings.MEDLOGIN Then
            If Not Me.txtMedicoResponsabile.IsValid Then
                listCampiNonImpostati.Add("medico responsabile")
            End If
        End If
        '--
        If listCampiNonImpostati.Count = 0 Then
            '--
            ' Campi obbligatori dello user control impostati
            If Not String.IsNullOrEmpty(Me.odpDataEs.Text) Then
                '--
                If Me.odpDataEs.Data.Date > DateTime.Now.Date Then
                    '--
                    Me.lblErrorLogin.Text = String.Format("Attenzione, la <b>Data</b> di <b>Esecuzione</b> non puo' essere futura.")
                    Me.lblErrorLogin.Visible = True
                    '--
                    Return
                    '--
                Else
                    '--
                    ' TODO: l'ora nel textbox Me.txtOraEs_MedLogin sarebbe da controllare anche qui (è già controllata lato client)
                    ' If parsing orario not ok then Me.txtOraEs_MedLogin.Text = String.Empty
                    '--
                End If
                '--
            Else
                '--
                Me.odpDataEs.Data = DateTime.Now.Date
                Me.txtOraEs_MedLogin.Text = DateTime.Now.ToString("HH:mm", System.Globalization.CultureInfo.InvariantCulture)
                '--
            End If
            '--
            If Not VaccinazioniEditable Then
                '--
                VaccinazioniEditable = True
                vaccinazioniEditableChanged = True
                '--
            End If
            '--
            ' N.B. : qui non è necessario controllare che l'ambulatorio faccia parte del consultorio corrente
            '        perchè lo user control, in questo caso, non permette di cambiare consultorio.
            OnVacUtility.Variabili.CNS.Ambulatorio.Codice = Me.uscScegliAmb.ambCodice
            OnVacUtility.Variabili.AMBConvocazione = Me.uscScegliAmb.ambCodice
            '--
            If Me.txtMedicoResponsabile.IsValid Then
                '--
                If String.IsNullOrEmpty(OnVacUtility.Variabili.MedicoResponsabile.Codice) Then
                    checkGestioneBarCodeLotti = True
                End If
                '--
                OnVacUtility.Variabili.MedicoResponsabile.Codice = Me.txtMedicoResponsabile.Codice
                OnVacUtility.Variabili.MedicoResponsabile.Nome = Me.txtMedicoResponsabile.Descrizione
                '--
            End If
            '--
            If Me.txtMedicoVaccinante.IsValid Then
                '--
                'valorizza il nuovo medico vaccinatore
                medicoVaccinatoreChanged = OnVacUtility.Variabili.MedicoVaccinante.Codice <> Me.txtMedicoVaccinante.Codice
                '--
                OnVacUtility.Variabili.MedicoVaccinante.Codice = Me.txtMedicoVaccinante.Codice
                OnVacUtility.Variabili.MedicoVaccinante.Nome = Me.txtMedicoVaccinante.Descrizione
                '--
            Else
                '--
                ' Sbiancamento del medico vaccinatore
                If String.IsNullOrEmpty(Me.txtMedicoVaccinante.Codice) OrElse String.IsNullOrEmpty(Me.txtMedicoVaccinante.Descrizione) Then
                    '--
                    medicoVaccinatoreChanged = True
                    '--
                    OnVacUtility.Variabili.MedicoVaccinante.Codice = Nothing
                    OnVacUtility.Variabili.MedicoVaccinante.Nome = Nothing
                    '--
                End If
                '--
            End If
            '--
            ' Impostazione data-ora esecuzione
            If String.IsNullOrEmpty(Me.txtOraEs_MedLogin.Text) Then
                If Me.odpDataEs.Data = DateTime.Now.Date Then
                    Me.txtOraEs_MedLogin.Text = DateTime.Now.ToString("HH:mm", System.Globalization.CultureInfo.InvariantCulture)
                Else
                    Me.txtOraEs_MedLogin.Text = "00:00"
                End If
            End If
            '--
            Dim dataOraEsecuzioneSelezionata As DateTime = GetDataOraEffettuazione(Me.odpDataEs.Data, Me.txtOraEs_MedLogin.Text)
            If dataOraEsecuzioneSelezionata = DateTime.MinValue Then dataOraEsecuzioneSelezionata = DateTime.Now
            '--
            OnVacUtility.Variabili.DataEsecuzione = dataOraEsecuzioneSelezionata
            '--
            For Each dr_vacProg As DataRow In dt_vacProg.Rows
                '--
                If dr_vacProg.RowState <> DataRowState.Deleted Then
                    '--
                    If Not Me.IsEsclusaEseguita(dr_vacProg) Then
                        '--
                        dr_vacProg("ves_data_effettuazione") = dataOraEsecuzioneSelezionata.Date
                        dr_vacProg("ves_dataora_effettuazione") = dataOraEsecuzioneSelezionata
                        dataEsecuzioneChanged = True
                        '--
                    End If
                    '--
                    If medicoVaccinatoreChanged Then
                        '--
                        dr_vacProg("ves_med_vaccinante") = OnVacUtility.Variabili.MedicoVaccinante.Codice
                        dr_vacProg("ope_vac") = OnVacUtility.Variabili.MedicoVaccinante.Nome
                        '--
                    End If
                    '--
                End If
                '--
            Next
            '--
            ' Se sono cambiati dei dati: re-bind griglia
            '--
            If vaccinazioniEditableChanged OrElse dataEsecuzioneChanged OrElse medicoVaccinatoreChanged Then
                '--
                Me.BindDGVacProgrammate()
                '--
            End If
            '--
            If vaccinazioniEditableChanged Then
                '--
                If Settings.GESBIL Then
                    '--
                    Me.BindDGBilProgrammati()
                    '--
                End If
                '--
                Me.EnableToolbar(False)
                '--
            End If

            RiempiCampiEsecuzione(False)

            chkInCampagna.Enabled = True

            EnableDdlEseMalPag(True)
            EnableImgBtnMedLogin(True)

            If checkGestioneBarCodeLotti Then
                ControllaGestioneBarCodeLotti()
            End If

            lblErrorLogin.Text = String.Empty
            modMedLogin.VisibileMD = False

        Else
            '--
            ' Campi obbligatori dello user control non impostati
            Dim str As String = String.Join(", ", listCampiNonImpostati.ToArray())

            lblErrorLogin.Text = String.Format("Attenzione, per proseguire sono necessarie le seguenti informazioni: <b>{0}</b>", str)
            lblErrorLogin.Visible = True

        End If

    End Sub

    Private Sub imbDataEs_Click(sender As Object, e As ImageClickEventArgs) Handles imbDataEs.Click

        If Not IsDgrEditable(True) Then ApriModaleMedLogin(TipoRichiamoMedLogin.MODIFICA_DATA)

    End Sub

    Private Sub imbModificaMedicoReferente_Click(sender As Object, e As ImageClickEventArgs) Handles imbModificaMedicoReferente.Click

        If Not IsDgrEditable(True) Then ApriModaleMedLogin(TipoRichiamoMedLogin.MODIFICA_MEDRESP)

    End Sub

    Private Sub imbModificaVaccinatore_Click(sender As Object, e As ImageClickEventArgs) Handles imbModificaVaccinatore.Click

        If Not IsDgrEditable(True) Then ApriModaleMedLogin(TipoRichiamoMedLogin.MODIFICA_MEDVAC)

    End Sub

    Private Sub chkInAmbulatorio_CheckedChanged(sender As Object, e As EventArgs) Handles chkInAmbulatorio.CheckedChanged

        OnVacUtility.Variabili.MedicoInAmbulatorio = chkInAmbulatorio.Checked

    End Sub

    Private Sub ApriModaleMedLogin(tipoRichiamo As TipoRichiamoMedLogin)

        uscScegliAmb.cnsCodice = OnVacUtility.Variabili.CNS.Codice
        uscScegliAmb.cnsDescrizione = OnVacUtility.Variabili.CNS.Descrizione

        uscScegliAmb.ambCodice = OnVacUtility.Variabili.CNS.Ambulatorio.Codice
        uscScegliAmb.ambDescrizione = Me.txtAmbulatorioReadOnly.Text

        uscScegliAmb.databind()

        txtMedicoVaccinante.Filtro = OnVacUtility.GetModalListFilterOperatori(False, True)
        txtMedicoVaccinante.Codice = OnVacUtility.Variabili.MedicoVaccinante.Codice
        txtMedicoVaccinante.Descrizione = OnVacUtility.Variabili.MedicoVaccinante.Nome

        txtMedicoResponsabile.Filtro = OnVacUtility.GetModalListFilterOperatori(True, True)
        txtMedicoResponsabile.Codice = OnVacUtility.Variabili.MedicoResponsabile.Codice
        txtMedicoResponsabile.Descrizione = OnVacUtility.Variabili.MedicoResponsabile.Nome

        odpDataEs.Data = OnVacUtility.Variabili.DataEsecuzione
        txtOraEs_MedLogin.Text = OnVacUtility.Variabili.DataEsecuzione.ToString("HH:mm", System.Globalization.CultureInfo.InvariantCulture)

        lblErrorLogin.Visible = False

        Select Case tipoRichiamo

            Case TipoRichiamoMedLogin.IMPOSTA, TipoRichiamoMedLogin.MODIFICA_MEDRESP

                OnitLayout31.InsertRoutineJS("addProcToEvent(""window.onload"",""document.getElementById('" & Me.txtMedicoResponsabile.ClientID & "').focus();"",""GetFocusToMedResp"")")

            Case TipoRichiamoMedLogin.MODIFICA_MEDVAC

                OnitLayout31.InsertRoutineJS("addProcToEvent(""window.onload"",""document.getElementById('" & Me.txtMedicoVaccinante.ClientID & "').focus();"",""GetFocusToMedVacc"")")

            Case TipoRichiamoMedLogin.MODIFICA_DATA

                OnitLayout31.InsertRoutineJS("addProcToEvent(""window.onload"",""OnitDataPickFocus('" & Me.odpDataEs.ClientID & "',1,false);"",""GetFocusToDataEs"")")

        End Select

        modMedLogin.Attributes.Add("TipoRichiamo", tipoRichiamo)
        modMedLogin.VisibileMD = True

    End Sub

    Private Sub EnableImgBtnMedLogin(bool As Boolean)

        imbDataEs.ImageUrl = If(bool, ResolveUrl("~/Images/modifica.gif"), ResolveUrl("~/Images/modifica_dis.gif"))
        imbModificaMedicoReferente.ImageUrl = If(bool, ResolveUrl("~/Images/modifica.gif"), ResolveUrl("~/Images/modifica_dis.gif"))
        imbModificaVaccinatore.ImageUrl = If(bool, ResolveUrl("~/Images/modifica.gif"), ResolveUrl("~/Images/modifica_dis.gif"))

        imbDataEs.Enabled = bool
        imbModificaMedicoReferente.Enabled = bool
        imbModificaVaccinatore.Enabled = bool

    End Sub

#End Region

#Region " dg_vac_prog "

    Private Sub dg_vacProg_DeleteCommand(source As Object, e As DataGridCommandEventArgs) Handles dg_vacProg.DeleteCommand

        If Not IsDgrEditable(True) Then

            Dim row As DataRow = dt_vacProg.Rows.Find(GetRowKey(e.Item.ItemIndex))

            'deve verificare se è presente l'associazione nella riga da eliminare e, in caso affermativo,
            'controllare se c'è anche nelle altre righe: di conseguenza, queste devono essere eliminate
            Dim associazioneEliminata As String = row("vpr_ass_codice").ToString()

            If Not String.IsNullOrEmpty(associazioneEliminata) Then

                For count As Integer = dt_vacProg.Rows.Count - 1 To 0 Step -1

                    If dt_vacProg.Rows(count).RowState <> DataRowState.Deleted Then

                        If dt_vacProg.Rows(count)("vpr_ass_codice").ToString() = associazioneEliminata Then

                            dt_vacProg.Rows(count).Delete()

                        End If

                    End If

                Next

            Else

                row.Delete()

            End If

            ImpostaLottiUtilizzatiDefaultSeNecessario(False)

            BindDGVacProgrammate()

            OnitLayout31.Busy = True

            EnableToolbar(False)

        End If

    End Sub

    Private Sub dg_vacProg_EditCommand(source As Object, e As DataGridCommandEventArgs) Handles dg_vacProg.EditCommand
        '--
        If Not IsDgrEditable(True) Then
            '--
            dg_vacProg.EditItemIndex = e.Item.ItemIndex
            '--
            BindDGVacProgrammate()

            ' ----------- '
            ' TODO [AVN - modifiche per aggiunta campi]
            'Dim rowVacProg As DataRow = dt_vacProg.Rows.Find(GetRowKey(e.Item.ItemIndex))
            '--
            '' PAGAMENTO
            'ddlTipiPagVac.ClearSelection()
            ''--
            'If Not rowVacProg.IsNull("noc_tpa_guid_tipi_pagamento") Then
            '    ddlTipiPagVac.SelectedValue = New Guid(DirectCast(rowVacProg("noc_tpa_guid_tipi_pagamento"), Byte())).ToString()
            'End If
            ''--
            '' FLAG ESENZIONE: 0/NULL = Disabilitato; 1 = Abilitato; 2 = Obbligatorio
            ''--
            'Dim flagStatoEsenzione As Enumerators.StatoAbilitazioneCampo = GetStatoAbilitazione(rowVacProg("flag_esenzione"))
            ''--
            'Dim codiceMalattia As String = String.Empty
            'If Not rowVacProg("ves_mal_codice_malattia") Is DBNull.Value Then codiceMalattia = rowVacProg("ves_mal_codice_malattia").ToString()
            ''--
            'Dim codiceEsenzione As String = String.Empty
            'If Not rowVacProg("ves_codice_esenzione") Is DBNull.Value Then codiceEsenzione = rowVacProg("ves_codice_esenzione").ToString()
            ''--
            'SetEsenzionePagamento(flagStatoEsenzione, codiceMalattia, codiceEsenzione)
            ''--
            '' FLAG IMPORTO: 0/NULL = Disabilitato; 1 = Abilitato; 2 = Obbligatorio
            ''--
            'Dim flagStatoImporto As Enumerators.StatoAbilitazioneCampo = GetStatoAbilitazione(rowVacProg("flag_importo"))
            ''--
            'SetImportoPagamento(flagStatoImporto, rowVacProg("ves_importo").ToString())
            ' ----------- '

            ' Focus nel campo della dose di vaccinazione
            Dim txtNumRich As TextBox = GetTextBoxControlFromDgVacProg(Me.dg_vacProg.EditItemIndex, "txtNumRich")
            Me.strJS &= String.Format("var doseClientID = '{0}';{1}document.getElementById(doseClientID).focus();{1}document.getElementById(doseClientID).select();{1}",
                                      txtNumRich.ClientID, Environment.NewLine)

            EnableToolbar(True)

        End If

    End Sub

    Private Sub dg_vacProg_UpdateCommand(source As Object, e As DataGridCommandEventArgs) Handles dg_vacProg.UpdateCommand

        Dim rowVacProg As DataRow = dt_vacProg.Rows.Find(GetRowKey(e.Item.ItemIndex))
        Dim codiceVaccinazione As String = rowVacProg("vpr_vac_codice")

        Dim hasError As Boolean = False
        Dim errList As New List(Of String)()

        ' Controlli preliminari
        Dim existsVacEseguita As Boolean = False
        Dim existsVacScaduta As Boolean = False

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizEseguite As New BizVaccinazioniEseguite(genericProvider, Settings, OnVacContext.CreateBizContextInfos)

                existsVacEseguita = bizEseguite.ExistsVaccinazioneEseguita(OnVacUtility.Variabili.PazId, OnVacUtility.Variabili.DataEsecuzione.Date, codiceVaccinazione)
                existsVacScaduta = bizEseguite.ExistsVaccinazioneScaduta(OnVacUtility.Variabili.PazId, OnVacUtility.Variabili.DataEsecuzione.Date, codiceVaccinazione)

            End Using
        End Using

        If existsVacEseguita Then
            errList.Add(String.Format("Esiste gia\' la vaccinazione {0} eseguita in data {1}", codiceVaccinazione, OnVacUtility.Variabili.DataEsecuzione.ToShortDateString()))
            hasError = True
        End If

        If existsVacScaduta Then
            errList.Add("Esiste gia\' una vaccinazione scaduta per la stessa dose e nella stessa data di effettuazione")
            hasError = True
        End If

        If Not FindData(e.Item.ItemIndex) Then
            errList.Add("Il campo 'Data' contiene già la stessa data utilizzata\nper la stessa vaccinazione e la stessa dose relativa ad un'altra convocazione!")
            hasError = True
        End If

        If hasError Then
            Me.strJS &= String.Format("alert(""{0}""){1}", String.Join("\n", errList.ToArray()), vbCrLf)
            Return
        End If
        '---
        ' Fine controlli
        '---

        ' NUMERO DOSE
        ' N.B. : il numero dose è già stato controllato lato client. Non si fa così ma non c'è tempo...
        Dim numeroDose As Integer = 1
        Dim txtNumRich As TextBox = GetTextBoxControlFromDgVacProg(e.Item.ItemIndex, "txtNumRich")

        If Not String.IsNullOrWhiteSpace(txtNumRich.Text) Then
            numeroDose = Convert.ToInt32(txtNumRich.Text)
        End If

        rowVacProg("vpr_n_richiamo") = numeroDose

        ' ----------- '
        ' TODO [AVN - modifiche per aggiunta campi]
        '' VACCINATORE
        'Dim omlMedico As OnitModalList = Me.GetModalListControlFromDgVacProg(e.Item.ItemIndex, "omlMedico")

        'If rowVacProg("ves_med_vaccinante").ToString() <> omlMedico.Codice Then

        '    rowVacProg("ves_med_vaccinante") = omlMedico.Codice
        '    rowVacProg("ope_vac") = omlMedico.Descrizione

        '    CopiaMedicoVaccinazionePerNomeCommerciale(rowVacProg)

        'End If

        '' DATI PAGAMENTO
        'If String.IsNullOrEmpty(Me.ddlEseMalPagVac.SelectedValue) Then
        '    rowVacProg("ves_mal_codice_malattia") = DBNull.Value
        '    rowVacProg("ves_codice_esenzione") = DBNull.Value
        'Else
        '    Dim codiceEsenzioneMalattia As String() = Me.ddlEseMalPagVac.SelectedValue.Split("|")
        '    rowVacProg("ves_mal_codice_malattia") = codiceEsenzioneMalattia(0)
        '    rowVacProg("ves_codice_esenzione") = codiceEsenzioneMalattia(1)
        'End If

        'If String.IsNullOrEmpty(valImpPagVac.Text) Then
        '    rowVacProg("ves_importo") = DBNull.Value
        'Else
        '    rowVacProg("ves_importo") = Double.Parse(Me.valImpPagVac.Text)
        'End If

        'If String.IsNullOrEmpty(ddlTipiPagVac.SelectedValue) Then
        '    rowVacProg("noc_tpa_guid_tipi_pagamento") = DBNull.Value
        '    rowVacProg("flag_importo") = DBNull.Value
        '    rowVacProg("flag_esenzione") = DBNull.Value
        'Else
        '    ' Se il tipo di pagamento è stato variato, allora recupero i flag relativi
        '    If Not rowVacProg.IsNull("noc_tpa_guid_tipi_pagamento") AndAlso
        '       ddlTipiPagVac.SelectedValue <> (New Guid(DirectCast(rowVacProg("noc_tpa_guid_tipi_pagamento"), Byte()))).ToString() Then

        '        Dim tipoPagamentoSelezionato As Entities.TipiPagamento = GetTipoPagamentoSelezionato()

        '        If Not tipoPagamentoSelezionato Is Nothing Then
        '            rowVacProg("noc_tpa_guid_tipi_pagamento") = tipoPagamentoSelezionato.Guid.ToByteArray()
        '            rowVacProg("flag_importo") = DirectCast(tipoPagamentoSelezionato.FlagStatoCampoImporto.Value, Integer)
        '            rowVacProg("flag_esenzione") = DirectCast(tipoPagamentoSelezionato.FlagStatoCampoEsenzione.Value, Integer)
        '        End If
        '    End If
        'End If

        'CopiaPagamentoVaccinazionePerNomeCommercialeEdImpostaTicket(rowVacProg)

        'ddlTipiPagVac.ClearSelection()
        'ddlEseMalPagVac.ClearSelection()
        'valImpPagVac.Text = String.Empty
        ' ----------- '

        dg_vacProg.EditItemIndex = -1

        BindDGVacProgrammate()

        OnitLayout31.Busy = True
        EnableToolbar(False)

    End Sub

    Private Sub dg_vacProg_CancelCommand(source As Object, e As DataGridCommandEventArgs) Handles dg_vacProg.CancelCommand

        dg_vacProg.EditItemIndex = -1

        BindDGVacProgrammate()

        EnableToolbar(False)

    End Sub

    Private Sub dg_vacProg_ItemDataBound(source As Object, e As DataGridItemEventArgs) Handles dg_vacProg.ItemDataBound

        Select Case e.Item.ItemType

            Case ListItemType.Item, ListItemType.EditItem, ListItemType.AlternatingItem

                Dim currentRow As DataRowView = e.Item.DataItem

#Region " Note "

                ' Impostazione immagine e abilitazione del pulsante delle note
                Dim btnNoteVac As ImageButton = DirectCast(e.Item.FindControl("btnNoteVac"), ImageButton)

                If Not btnNoteVac Is Nothing Then

                    ' Abilita/disabilita il pulsante delle note in base allo stato del datagrid (se è in edit => pulsanti disabilitati)
                    EnableImageButtonNote(btnNoteVac, dg_vacProg.EditItemIndex = -1, Not String.IsNullOrWhiteSpace(currentRow("ves_note").ToString()))

                End If

#End Region

#Region " Pagamento "

                ' Abilitazione del pulsante del pagamento
                Dim lnkPagVac As LinkButton = DirectCast(e.Item.FindControl("lnkPagVac"), LinkButton)
                If Not lnkPagVac Is Nothing Then

                    If Settings.PAGAMENTO Then

                        If GetStatoAbilitazione(currentRow("flag_importo")) <> Enumerators.StatoAbilitazioneCampo.Disabilitato OrElse
                           GetStatoAbilitazione(currentRow("flag_esenzione")) <> Enumerators.StatoAbilitazioneCampo.Disabilitato Then

                            ' Abilita/disabilita il pulsante del pagamento in base allo stato del datagrid (se è in edit => pulsanti disabilitati)
                            EnableLinkButtonPagamento(lnkPagVac, (dg_vacProg.EditItemIndex = -1))

                            ' Imposta testo e tooltip del pulsante in base ai dati di riga (importo, esenzione)
                            RefreshLinkButtonPagamento(lnkPagVac, currentRow.Row)

                        Else
                            lnkPagVac.Visible = False
                        End If

                    Else
                        lnkPagVac.Visible = False
                    End If

                End If

#End Region

#Region " Via e Sito "

                ' Dropdownlist Via Somministrazione e Sito Inoculazione
                Dim ddl As DropDownList = DirectCast(e.Item.FindControl("ddlVii"), DropDownList)

                If Not ddl Is Nothing Then
                    BindDdl(ddl, currentRow("ves_vii_codice"), VieSomministrazione)
                    ddl.Enabled = dg_vacProg.EditItemIndex = -1
                End If

                ddl = DirectCast(e.Item.FindControl("ddlSii"), DropDownList)

                If Not ddl Is Nothing Then
                    BindDdl(ddl, currentRow("ves_sii_codice"), SitiInoculazione)
                    ddl.Enabled = dg_vacProg.EditItemIndex = -1
                End If

#End Region

#Region " Condizione sanitaria e di rischio "

                ' Modale Condizione sanitaria
                Dim oml As OnitModalList = DirectCast(e.Item.FindControl("omlCondSanitaria"), OnitModalList)

                If Not oml Is Nothing Then

                    Dim codiceMalattia As String = currentRow("ves_mal_codice_cond_sanitaria").ToString()
                    Dim descrizioneMalattia As String = String.Empty

                    If Not String.IsNullOrWhiteSpace(codiceMalattia) Then

                        ' C'è un codice da associare => ricavo la descrizione

                        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                            Using bizMalattie As New BizMalattie(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                                descrizioneMalattia = bizMalattie.GetDescrizioneMalattia(codiceMalattia)

                            End Using
                        End Using

                    Else

                        ' Imposto la condizione sanitaria, tra quelle del paziente, in base alla vaccinazione associata.
                        Dim command As New BizMalattie.GetCondizioneSanitariaDefaultPazienteCommand()
                        command.CodiceVaccinazione = currentRow("vpr_vac_codice").ToString()
                        command.CondizioniSanitariePaziente = CondizioniSanitariePaziente
                        command.CodiceCondizioneSanitariaDefault = Settings.CONDIZIONE_SANITARIA_DEFAULT
                        command.DescrizioneCondizioneSanitariaDefault = DescrizioneMalattiaDefault
                        command.CodiceNessunaMalattia = Settings.CODNOMAL

                        Dim condizioneSanitaria As Entities.CondizioneSanitaria = BizMalattie.GetCondizioneSanitariaDefaultPaziente(command)
                        codiceMalattia = condizioneSanitaria.CodiceMalattia
                        descrizioneMalattia = condizioneSanitaria.DescrizioneMalattia

                        currentRow("ves_mal_codice_cond_sanitaria") = codiceMalattia

                    End If

                    oml.Codice = codiceMalattia
                    oml.Descrizione = descrizioneMalattia
                    oml.ToolTip = descrizioneMalattia

                End If

                ' Modale Condizione rischio
                oml = DirectCast(e.Item.FindControl("omlCondRischio"), OnitModalList)

                If Not oml Is Nothing Then

                    Dim codiceRischio As String = currentRow("ves_rsc_codice").ToString()
                    Dim descrizioneRischio As String = String.Empty

                    If Not String.IsNullOrWhiteSpace(codiceRischio) Then

                        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                            Using bizRischio As New BizCategorieRischio(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                                descrizioneRischio = bizRischio.GetDescrizioneCategoriaRischio(codiceRischio)

                            End Using
                        End Using

                    Else
                        Dim codiceVaccinazione As String = currentRow("vpr_vac_codice").ToString()
                        Dim existsAntiInfluenzale As Boolean = False

                        ' Verifico se la vaccinazione è anti-influenzale
                        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                            Using bizVac As New BizVaccinazioniAnagrafica(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                                existsAntiInfluenzale = bizVac.ExistsVaccinazioneAntiInfluenzale(codiceVaccinazione)

                            End Using
                        End Using

                        ' Valorizzazione condizione di rischio:
                        ' controllo il campo rischio del paziente:
                        '   - se il paziente ha 1 cond. rischio associata alla vaccinazione corrente => imposto quella
                        '   - altrimenti => imposto default (parametro)
                        If Not CondizioniRischioPaziente Is Nothing AndAlso CondizioniRischioPaziente.Count > 0 Then

                            ' Lista condizioni sanitarie del paziente relative alla vaccinazione della riga corrente
                            Dim list As List(Of Entities.CondizioneRischio) =
                                CondizioniRischioPaziente.Where(Function(p) p.CodiceVaccinazione = codiceVaccinazione).ToList()

                            If Not list Is Nothing AndAlso list.Count = 1 Then
                                codiceRischio = list.First.CodiceCategoriaRischio
                                descrizioneRischio = list.First.DescrizioneCategoriaRischio
                            Else
                                If Not existsAntiInfluenzale Then
                                    codiceRischio = Settings.CONDIZIONE_RISCHIO_DEFAULT
                                    descrizioneRischio = DescrizioneCategoriaRischioDefault
                                End If
                            End If

                        Else

                            If Not existsAntiInfluenzale Then
                                ' Nessuna condizione di rischio associata: imposto il default
                                codiceRischio = Settings.CONDIZIONE_RISCHIO_DEFAULT
                                descrizioneRischio = DescrizioneCategoriaRischioDefault
                            End If

                        End If

                        currentRow("ves_rsc_codice") = codiceRischio

                    End If

                    oml.Codice = codiceRischio
                    oml.Descrizione = descrizioneRischio
                    oml.ToolTip = descrizioneRischio

                End If

#End Region

        End Select

    End Sub

    Protected Function IsVaccinazioneObbligatoria(campoflagVaccinazione As Object) As Boolean
        '--
        If campoflagVaccinazione Is Nothing OrElse campoflagVaccinazione Is DBNull.Value Then Return False
        '--
        If campoflagVaccinazione.ToString() <> Constants.ObbligatorietaVaccinazione.Obbligatoria Then Return False
        '--
        Return True
        '--
    End Function

    Protected Function DescrizioneAssociazioneDose(vpr_ass_codice As Object, ves_ass_n_dose As Object) As String

        Dim result As New Text.StringBuilder()

        If vpr_ass_codice Is DBNull.Value Then
            result.Append("&nbsp;&nbsp;&nbsp;-")
        Else
            result.Append(vpr_ass_codice.ToString())
        End If

        result.AppendFormat(" [{0}]", ves_ass_n_dose.ToString())

        Return result.ToString()

    End Function

    Private Sub EnableLinkButtonPagamento(lnkPagVac As LinkButton, enable As Boolean)

        lnkPagVac.Enabled = enable

    End Sub

    ''' <summary>
    ''' Refresh layout pulsante pagamento sulla riga specificata
    ''' </summary>
    ''' <param name="lnkPagVac"></param>
    ''' <param name="currentRow"></param>
    Private Sub RefreshLinkButtonPagamento(lnkPagVac As LinkButton, currentRow As DataRow)

        Dim importo As Decimal? = Nothing
        If Not currentRow("ves_importo") Is Nothing AndAlso Not currentRow("ves_importo") Is DBNull.Value Then
            importo = Convert.ToDecimal(currentRow("ves_importo"))
        End If

        Dim codiceEsenzione As String = String.Empty
        If Not currentRow("ves_codice_esenzione") Is Nothing AndAlso Not currentRow("ves_codice_esenzione") Is DBNull.Value Then
            codiceEsenzione = currentRow("ves_codice_esenzione").ToString()
        End If

        Dim numberFormatInfo As Globalization.NumberFormatInfo = OnVacUtility.GetNumberFormatInfo(True)

        If importo.HasValue Then
            lnkPagVac.Text = importo.Value.ToString("C", numberFormatInfo)
        Else
            lnkPagVac.Text = numberFormatInfo.CurrencySymbol
        End If

        lnkPagVac.ToolTip = GetLnkPagVacToolTip(importo, codiceEsenzione)

    End Sub

    Private Sub EnableImageButtonNote(btnNoteVac As ImageButton, buttonEnabled As Boolean, iconEnabled As Boolean)

        If iconEnabled Then
            btnNoteVac.ImageUrl = UrlIconaNotaSi(buttonEnabled)
        Else
            btnNoteVac.ImageUrl = UrlIconaNotaNo(buttonEnabled)
        End If

        btnNoteVac.Enabled = buttonEnabled

    End Sub

#End Region

#Region " uwtSceltaAssociazioni "

    Private Sub uwtSceltaAssociazioni_ButtonClicked(sender As System.Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles uwtSceltaAssociazioni.ButtonClicked

        Select Case be.Button.Key

            Case "btn_Conferma"

                Dim ass As String() = AssociazioniScelteLatoClient.Text.Split("|")

                For i As Integer = 0 To dt_vacProg.Rows.Count - 1

                    If dt_vacProg.Rows(i).RowState <> DataRowState.Deleted Then

                        For j As Integer = 0 To ass.Length - 1 Step 3

                            If Not dt_vacProg.Rows(i).IsNull("ves_lot_codice") AndAlso dt_vacProg.Rows(i)("ves_lot_codice").ToString() = ass(j) Then

                                dt_vacProg.Rows(i)("vpr_ass_codice") = ass(j + 1)
                                dt_vacProg.Rows(i)("ass_descrizione") = ass(j + 2)

                                ' Ricalcolo della dose di associazione
                                If Not dt_vacProg.Rows(i)("vpr_ass_codice") Is System.DBNull.Value Then

                                    Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

                                        dt_vacProg.Rows(i)("ves_ass_n_dose") =
                                            genericProvider.VaccinazioniEseguite.GetMaxDoseAssociazione(OnVacUtility.Variabili.PazId, dt_vacProg.Rows(i)("vpr_ass_codice")) + 1

                                    End Using

                                End If

                            End If

                        Next

                    End If

                Next

                Me.Esegui()

                Me.ofmSceltaAssociazioni.VisibileMD = False

            Case "btn_Annulla"

                Me.ofmSceltaAssociazioni.VisibileMD = False

        End Select

    End Sub

#End Region

#Region " Gestione Eventi dello User Control di Inserimento dell'Associazione (uscInsAssociazione) "

    Private Sub uscInsAssociazione_Conferma(sender As Object, confermaEventArgs As InsAssociazione.ConfermaEventArgs) Handles uscInsAssociazione.Conferma
        '--
        If confermaEventArgs.Esito Then
            '--
            Me.modInsAssociazione.VisibileMD = False
            '--
            Dim dataEsecuzioneCorrente As DateTime = DateTime.Now
            If OnVacUtility.Variabili.DataEsecuzione > DateTime.MinValue AndAlso Not DateTime.Equals(OnVacUtility.Variabili.DataEsecuzione, DateTime.Now) Then
                dataEsecuzioneCorrente = OnVacUtility.Variabili.DataEsecuzione
            End If
            '--
            Dim dtVaccinazioniSelezionate As DataTable = confermaEventArgs.VaccinazioniSelezionate
            '--
            For Each vaccinazioneSelezionata As DataRow In dtVaccinazioniSelezionate.Rows
                '--
                Dim rowVacProg As DataRow = dt_vacProg.NewRow()
                '--
                rowVacProg("ves_data_effettuazione") = dataEsecuzioneCorrente
                rowVacProg("ves_dataora_effettuazione") = dataEsecuzioneCorrente
                rowVacProg("ves_sii_codice") = vaccinazioneSelezionata("ass_sii_codice")
                rowVacProg("sii_descrizione") = vaccinazioneSelezionata("sii_descrizione")
                rowVacProg(NomeFlagTipoValorizzazione.SITO) = vaccinazioneSelezionata(NomeFlagTipoValorizzazione.SITO)
                rowVacProg("ves_vii_codice") = vaccinazioneSelezionata("ass_vii_codice")
                rowVacProg("vii_descrizione") = vaccinazioneSelezionata("vii_descrizione")
                rowVacProg(NomeFlagTipoValorizzazione.VIA) = vaccinazioneSelezionata(NomeFlagTipoValorizzazione.VIA)
                rowVacProg("ves_med_vaccinante") = OnVacUtility.Variabili.MedicoVaccinante.Codice
                rowVacProg("ope_vac") = OnVacUtility.Variabili.MedicoVaccinante.Nome
                rowVacProg("vpr_vac_codice") = vaccinazioneSelezionata("vac_codice")
                rowVacProg("vac_cod_sostituta") = vaccinazioneSelezionata("vac_cod_sostituta")
                rowVacProg("vac_obbligatoria") = vaccinazioneSelezionata("vac_obbligatoria")
                rowVacProg("vac_descrizione") = vaccinazioneSelezionata("vac_descrizione")
                rowVacProg("vpr_ass_codice") = vaccinazioneSelezionata("val_ass_codice")
                rowVacProg("ass_descrizione") = vaccinazioneSelezionata("ass_descrizione")
                '--
                Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                    rowVacProg("VPR_N_RICHIAMO") = genericProvider.VaccinazioniEseguite.GetMaxRichiamo(OnVacUtility.Variabili.PazId, vaccinazioneSelezionata("vac_codice")) + 1
                End Using
                '--
                ' E' possibile inserire una vaccinazione esclusa. Se non è scaduta, non è possibile eseguirla.
                '--
                Using dam As IDAM = OnVacUtility.OpenDam()
                    '--
                    With dam.QB
                        .NewQuery()
                        .AddTables("t_vac_reazioni_avverse,t_vac_escluse")
                        .AddSelectFields("max(vra_data_reazione) vra_data_reazione, vex_data_scadenza, vex_data_visita")
                        .AddWhereCondition("vex_paz_codice", Comparatori.Uguale, "vra_paz_codice", DataTypes.OutJoinLeft)
                        .AddWhereCondition("vex_vac_codice", Comparatori.Uguale, "vra_vac_codice", DataTypes.OutJoinLeft)
                        .AddWhereCondition("vex_paz_codice", Comparatori.Uguale, OnVacUtility.Variabili.PazId, DataTypes.Numero)
                        .AddWhereCondition("vex_vac_codice", Comparatori.Uguale, rowVacProg("vpr_vac_codice"), DataTypes.Stringa)
                        .AddGroupByFields("vex_data_scadenza,vex_data_visita")
                    End With
                    '--
                    Using reader As IDataReader = dam.BuildDataReader()
                        '--
                        If reader.Read() Then
                            '--
                            rowVacProg("vra_data_reazione") = reader("vra_data_reazione")
                            rowVacProg("vex_data_visita") = reader("vex_data_visita")
                            rowVacProg("vex_data_scadenza") = reader("vex_data_scadenza")
                            '--
                            If Not rowVacProg("vex_data_visita") Is DBNull.Value Then
                                '--
                                rowVacProg("E") = StatoVaccinazione.ESCLUSA
                                '--                                  
                            End If
                            '--
                        End If
                        '--
                    End Using
                    '--
                    ' Ricalcolo della dose di associazione
                    '--
                    If Not rowVacProg("vpr_ass_codice") Is System.DBNull.Value Then
                        Using dbGenericProvider As New DbGenericProvider(OnVacContext.Connection)
                            rowVacProg("ves_ass_n_dose") = dbGenericProvider.VaccinazioniEseguite.GetMaxDoseAssociazione(OnVacUtility.Variabili.PazId, rowVacProg("vpr_ass_codice")) + 1
                        End Using
                    End If
                    '--
                End Using
                '--
                ' Aggiunta della riga alla tabella bindata in maschera
                '--
                dt_vacProg.Rows.InsertAt(rowVacProg, 0)
                '--
            Next

            ImpostaLottiUtilizzatiDefaultSeNecessario(True)

            BindDGVacProgrammate()

            OnitLayout31.Busy = True
            EnableToolbar(False)

        End If

    End Sub

    Private Sub uscInsAssociazione_Annulla(sender As Object, e As EventArgs) Handles uscInsAssociazione.Annulla
        '--
        Me.ControllaStatoConvocazione()
        '--
        Me.modInsAssociazione.VisibileMD = False
        '--
    End Sub

#End Region

#Region " uscInsDatiEsc "

    Private Sub uscInsDatiEsc_InsDatiEscConferma(datiEsclusioniSelezionate As List(Of OnVac_InsDatiEsc.DatiEsclusione)) Handles uscInsDatiEsc.InsDatiEscConferma

        ' Aggiornamento valori del datatable delle programmate
        If Not datiEsclusioniSelezionate Is Nothing AndAlso datiEsclusioniSelezionate.Count > 0 Then

            For Each esclusione As OnVac_InsDatiEsc.DatiEsclusione In datiEsclusioniSelezionate

                Dim rowVacProg As DataRow = dt_vacProg.Rows.Find(GetRowKey(esclusione.IndiceVaccinazioneProgrammata))

                rowVacProg("ves_data_effettuazione") = esclusione.DataVisita
                rowVacProg("vex_data_visita") = esclusione.DataVisita
                rowVacProg("vex_moe_codice") = esclusione.CodiceMotivoEsclusione

                If String.IsNullOrWhiteSpace(esclusione.CodiceOperatore) Then
                    rowVacProg("vex_ope_codice") = DBNull.Value
                Else
                    rowVacProg("vex_ope_codice") = esclusione.CodiceOperatore
                End If

                If String.IsNullOrWhiteSpace(esclusione.DescrizioneOperatore) Then
                    rowVacProg("ope_ex") = DBNull.Value
                Else
                    rowVacProg("ope_ex") = esclusione.DescrizioneOperatore
                End If

                rowVacProg("ves_lot_codice") = DBNull.Value
                rowVacProg("noc_descrizione") = DBNull.Value
                rowVacProg("ves_noc_codice") = DBNull.Value

                If esclusione.DataScadenza.HasValue Then
                    rowVacProg("vex_data_scadenza") = esclusione.DataScadenza.Value
                Else
                    rowVacProg("vex_data_scadenza") = DBNull.Value
                End If

                rowVacProg("E") = StatoVaccinazione.ESCLUSA
                rowVacProg("SaveEsclusa") = True

                rowVacProg("vex_dose") = esclusione.NumeroDose
                rowVacProg("vex_note") = esclusione.Note
            Next

        End If

        modInsDatiEsc.VisibileMD = False

        ' N.B. : se la gestione è con la pistola non deve associare i lotti di default
        ImpostaLottiUtilizzatiDefaultSeNecessario(False)

        BindDGVacProgrammate()

        OnitLayout31.Busy = True
        EnableToolbar(False)

    End Sub

    Private Sub uscInsDatiEsc_InsDatiEscAnnulla() Handles uscInsDatiEsc.InsDatiEscAnnulla

        modInsDatiEsc.VisibileMD = False

    End Sub

    Private Sub uscInsDatiEsc_ShowMessage(message As String) Handles uscInsDatiEsc.ShowMessage

        ShowAlertMessagge(message)

    End Sub

#End Region

#Region " uscAssLotti "

    Private Sub uscAssLotti_SetData(lottiFuoriEtaInclusi As Boolean, ByRef dtLottiUtilizzabili As DataTable, ByRef codiceLottiUtilizzati As String()) Handles uscAssLotti.SetData

        dtLottiUtilizzabili = Me.RecuperaLottiUtilizzabili(False, lottiFuoriEtaInclusi, False)
        codiceLottiUtilizzati = Me.CodiceLottiUtilizzati.ToArray()

    End Sub

    Private Sub uscAssLotti_AssLottiConferma() Handles uscAssLotti.AssLottiConferma

        ImpostaLottiUtilizzati(uscAssLotti.GetLottiUtilizzati(), RecuperaLottiUtilizzabili(False, True, Settings.PAGAMENTO), True, True)

        BindDGVacProgrammate()

        ' Se è prevista la gestione con la pistola deve selezionare automaticamente le vaccinazioni con il lotto associato
        If Settings.GESBALOT Then

            For count As Integer = 0 To dg_vacProg.Items.Count - 1
                If DirectCast(dg_vacProg.Items(count).FindControl("lblLotto"), Label).Text <> "" Then
                    DirectCast(dg_vacProg.Items(count).FindControl("cb"), CheckBox).Checked = True
                End If
            Next

        End If

        modAssLotto.VisibileMD = False

    End Sub

    Private Sub uscAssLotti_LayoutBusy(disableLayout As Boolean) Handles uscAssLotti.LayoutBusy

        OnitLayout31.Busy = disableLayout

    End Sub

    Private Sub uscAssLotti_ShowMessage(message As String) Handles uscAssLotti.ShowMessage

        OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(message, String.Empty, False, False))

    End Sub

#End Region

#Region " uscInsLotto "

    Private Sub uscInsLotto_InsLotti() Handles uscInsLotto.InsLotti

        ' N.B. : se la gestione è con la pistola non deve associare i lotti di default
        ImpostaLottiUtilizzatiDefaultSeNecessario(True)

        BindDGVacProgrammate()

        modInsLotto.VisibileMD = False

    End Sub

#End Region

#Region " uscScegliAmb "

    Private Sub uscScegliAmb_AmbulatorioCambiato(cnsCodice As String, ambCodice As Integer) Handles uscScegliAmb.AmbulatorioCambiato

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            OnVacUtility.Variabili.MedicoInAmbulatorio = genericProvider.Consultori.GetMedicoInAmb(ambCodice)

        End Using

        OnVacUtility.Variabili.AMBConvocazione = ambCodice

    End Sub

#End Region

#Region " Tab Convocazione "

    ' Gestisce il cambiamento del check nel tab della convocazione per modificare sulla tabella T_CNV_CONVOCAZIONI
    ' il campo CNV_CAMPAGNA. Modifica rischiesta per ottimizzare la statistica sulle vaccinazioni in campagna
    Private Sub chkInCampagna_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkInCampagna.CheckedChanged

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            genericProvider.Convocazione.UpdateCnvCampagna(OnVacUtility.Variabili.PazId, DataConvocazione, chkInCampagna.Checked)

        End Using

    End Sub

#End Region

#Region " Gestione Bilanci "

    Private Sub dg_bilProg_ItemDataBound(source As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dg_bilProg.ItemDataBound

        Select Case e.Item.ItemType

            Case ListItemType.Item, ListItemType.AlternatingItem

                DirectCast(e.Item.FindControl("btnEscBil"), ImageButton).Visible = VaccinazioniEditable
                DirectCast(e.Item.FindControl("btnDelBil"), ImageButton).Visible = VaccinazioniEditable
                DirectCast(e.Item.FindControl("btnEditBil"), ImageButton).Visible = VaccinazioniEditable

        End Select

    End Sub

    Private Sub dg_bilProg_ItemCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_bilProg.ItemCommand

        If e.CommandName = "EscludiBilancio" Then

            If Not IsDgrEditable(True) Then

                Dim row As DataRow

                If DirectCast(dg_bilProg.Items.Item(e.Item.ItemIndex).FindControl("lbl_stato"), Label).Text = Constants.StatiBilancio.UNSOLVED Then

                    Dim filterExpression As String = String.Format("bip_stato <> '{0}' AND bip_mal_codice='{1}'",
                                                                   Constants.StatiBilancio.UNSOLVED,
                                                                   DirectCast(dg_bilProg.Items.Item(e.Item.ItemIndex).FindControl("oml_codMalattiaDis"), TextBox).Text)

                    Dim rows As DataRow() = dt_bilProg.Select()

                    If rows.Length = 0 Then

                        row = dt_bilProg.Rows.Find(Convert.ToDecimal(DirectCast(e.Item.FindControl("lbl_id"), Label).Text))

                        If row("escluso") Is DBNull.Value OrElse row("escluso") = "" Then
                            row("escluso") = "X"
                            row("bip_stato") = Constants.StatiBilancio.UNSOLVED
                        Else
                            row("escluso") = ""
                            row("bip_stato") = Constants.StatiBilancio.UNEXECUTED
                        End If

                        BindDGBilProgrammati()

                    Else

                        strJS &= "alert('Operazione non permessa')" & vbCrLf

                    End If

                Else

                    row = dt_bilProg.Rows.Find(Convert.ToDecimal(DirectCast(e.Item.FindControl("lbl_id"), Label).Text))

                    If row.Item("escluso") Is DBNull.Value OrElse row("escluso") = "" Then
                        row.Item("escluso") = "X"
                        row.Item("bip_stato") = Constants.StatiBilancio.UNSOLVED
                    Else
                        row.Item("escluso") = ""
                        row.Item("bip_stato") = Constants.StatiBilancio.UNEXECUTED
                    End If

                    BindDGBilProgrammati()

                End If

            End If

            OnitLayout31.Busy = True

            Me.EnableToolbar(False)

        End If

    End Sub

    Private Sub dg_bilProg_DeleteCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_bilProg.DeleteCommand

        ' Se entrambi i datagrid sono in edit, non permette l'operazione
        If Not IsDgrEditable(True) Then

            Dim row As DataRow = dt_bilProg.Rows.Find(Convert.ToDecimal(DirectCast(e.Item.FindControl("lbl_id"), Label).Text))

            row.Delete()

            BindDGBilProgrammati()

            OnitLayout31.Busy = True

            Me.EnableToolbar(False)

        End If

    End Sub

    Private Sub dg_bilProg_EditCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_bilProg.EditCommand

        Dim index As Integer = e.Item.DataSetIndex

        If Not IsDgrEditable(True) Then

            Dim bilanci As Collection.BilancioProgrammatoCollection

            dg_bilProg.EditItemIndex = index

            BindDGBilProgrammati()

            ' Bind della ddl della riga in edit
            Dim ddl As DropDownList = DirectCast(dg_bilProg.Items(dg_bilProg.EditItemIndex).FindControl("cmbBilancio"), DropDownList)

            bilanci = CaricaBilanci()

            If bilanci.Count > 0 Then

                ddl.DataSource = bilanci
                ddl.DataTextField = "MalDescNBil"
                ddl.DataValueField = "NBilNMal"
                ddl.DataBind()

                Me.EnableToolbar(True)

            Else

                dg_bilProg.EditItemIndex() = -1

                BindDGBilProgrammati()

                strJS &= "alert(""Nessun bilancio disponibile per la modifica"")" & vbCrLf

            End If

        End If

    End Sub

    Private Sub dg_bilProg_CancelCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_bilProg.CancelCommand
        '--
        For i As Int64 = dt_bilProg.Rows.Count - 1 To 0 Step -1
            '--
            If Me.dt_bilProg.Rows(i).RowState <> DataRowState.Deleted Then
                '--
                If Me.dt_bilProg.Rows(i).IsNull("bip_bil_numero") And Me.dt_bilProg.Rows(i).IsNull("bip_mal_codice") Then
                    '--
                    Me.dt_bilProg.Rows(i).Delete()
                    '--
                End If
                '--
            End If
            '--
        Next
        '--
        Me.dg_bilProg.EditItemIndex = -1
        '--
        Me.BindDGBilProgrammati()
        '--
        Me.EnableToolbar(False)
        '--
    End Sub

    Private Sub dg_bilProg_UpdateCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_bilProg.UpdateCommand

        Dim ddlBilanci As System.Web.UI.WebControls.DropDownList = DirectCast(dg_bilProg.Items.Item(dg_bilProg.EditItemIndex).FindControl("cmbBilancio"), DropDownList)

        Dim row As DataRow = dt_bilProg.Rows.Find(Convert.ToDecimal(DirectCast(e.Item.FindControl("lbl_id"), Label).Text))

        Dim bilSelectedValue() As String = ddlBilanci.SelectedItem.Value.Split("|")
        Dim bilSelectedText() As String = ddlBilanci.SelectedItem.Text.Split("|")

        row("bip_mal_codice") = bilSelectedValue(1).Trim(" ")
        row("bip_bil_numero") = CInt(bilSelectedValue(0).Trim(" "))
        row("mal_descrizione") = bilSelectedText(0).Trim(" ")
        row("bip_cnv_data") = DataConvocazione
        row("bip_stato") = Constants.StatiBilancio.UNEXECUTED

        dg_bilProg.EditItemIndex = -1

        BindDGBilProgrammati()

        OnitLayout31.Busy = True

        Me.EnableToolbar(False)

    End Sub

    Private Sub oml_malattia_change(sender As Object, e As Onit.Controls.OnitModalList.ModalListaEventArgument)

        Dim cmbBilancio As DropDownList = DirectCast(dg_bilProg.Items(dg_bilProg.EditItemIndex).FindControl("cmbBilancio"), DropDownList)

        If Not cmbBilancio Is Nothing Then
            cmbBilancio.DataSource = CaricaBilanci()
            cmbBilancio.DataBind()
            cmbBilancio.SelectedIndex = -1
            cmbBilancio.SelectedValue = -1
        End If

    End Sub

#End Region

#Region " DropDownList Sito e Via "

    Protected Sub ddlSii_SelectedIndexChanged(sender As Object, e As EventArgs)

        ' Riga a cui appartiene la dropdownlist che ha sollevato l'evento
        Dim currentGridItem As DataGridItem = Me.GetCurrentDataGridVacProgItem(sender, "ddlSii")

        ' Copio il sito di inoculazione in ogni riga avente stesso nome commerciale
        If Not currentGridItem Is Nothing Then

            Dim ddl As DropDownList = DirectCast(sender, DropDownList)
            Dim rowVacProg As DataRow = dt_vacProg.Rows.Find(Me.GetRowKey(currentGridItem.ItemIndex))

            If Not rowVacProg Is Nothing Then

                rowVacProg("ves_sii_codice") = ddl.SelectedValue
                rowVacProg("sii_descrizione") = ddl.SelectedItem.Text

                If String.IsNullOrWhiteSpace(ddl.SelectedValue) Then
                    rowVacProg(NomeFlagTipoValorizzazione.SITO) = String.Empty
                Else
                    rowVacProg(NomeFlagTipoValorizzazione.SITO) = Constants.TipoValorizzazioneSitoVia.Manuale
                End If

                CopiaSitoInoculazioneVaccinazionePerNomeCommerciale(rowVacProg)

                BindDGVacProgrammate()

            End If
        End If

    End Sub

    Protected Sub ddlVii_SelectedIndexChanged(sender As Object, e As EventArgs)

        ' Riga a cui appartiene la dropdownlist che ha sollevato l'evento
        Dim currentGridItem As DataGridItem = Me.GetCurrentDataGridVacProgItem(sender, "ddlVii")

        ' Copio la via di somministrazione in ogni riga avente stesso nome commerciale
        If Not currentGridItem Is Nothing Then

            Dim ddl As DropDownList = DirectCast(sender, DropDownList)
            Dim rowVacProg As DataRow = dt_vacProg.Rows.Find(Me.GetRowKey(currentGridItem.ItemIndex))

            rowVacProg("ves_vii_codice") = ddl.SelectedValue
            rowVacProg("vii_descrizione") = ddl.SelectedItem.Text
            '--
            If String.IsNullOrWhiteSpace(ddl.SelectedValue) Then
                rowVacProg(NomeFlagTipoValorizzazione.VIA) = String.Empty
            Else
                rowVacProg(NomeFlagTipoValorizzazione.VIA) = Constants.TipoValorizzazioneSitoVia.Manuale
            End If

            Me.CopiaViaSomministrazioneVaccinazionePerNomeCommerciale(rowVacProg)

            Me.BindDGVacProgrammate()

        End If

    End Sub

    Private Sub BindDdl(Of T As New)(ddl As DropDownList, valoreCampoCodice As Object, dataSourceList As List(Of T))

        If Not ddl Is Nothing AndAlso ddl.Visible Then

            Dim list As List(Of T) = dataSourceList.Clone()
            list.Insert(0, New T())

            ddl.DataValueField = "Codice"
            ddl.DataTextField = "Descrizione"
            ddl.DataSource = list
            ddl.DataBind()
            ddl.ClearSelection()
            ddl.ToolTip = String.Empty

            If Not valoreCampoCodice Is Nothing AndAlso
               Not valoreCampoCodice Is DBNull.Value AndAlso
               Not String.IsNullOrWhiteSpace(valoreCampoCodice.ToString()) Then

                Dim listItem As ListItem = ddl.Items.FindByValue(valoreCampoCodice.ToString())
                If Not listItem Is Nothing Then
                    listItem.Selected = True
                    ddl.ToolTip = listItem.Text
                End If

            End If

        End If

    End Sub

#End Region

#Region " Modali Condizione Sanitaria e Condizioni di Rischio "

    Protected Sub omlCondSanitaria_SetUpFiletr(sender As Object)

        Dim codiceVaccinazioneCorrente As String = GetCodiceVaccinazioneCurrentRow(sender, "omlCondSanitaria")

        Dim omlCondSanitaria As OnitModalList = DirectCast(sender, OnitModalList)

        omlCondSanitaria.Filtro =
            String.Format("VCS_PAZ_CODICE = {0} AND VCS_VAC_CODICE = '{1}' ORDER BY Paz, Def, Descrizione",
                          OnVacUtility.Variabili.PazId, codiceVaccinazioneCorrente)

    End Sub

    Protected Sub omlCondSanitaria_Change(sender As Object, e As OnitModalList.ModalListaEventArgument)

        Dim omlCondSanitaria As OnitModalList = DirectCast(sender, OnitModalList)
        omlCondSanitaria.ToolTip = omlCondSanitaria.Descrizione

        Dim currentGridItem As DataGridItem = GetCurrentDataGridVacProgItem(sender, "omlCondSanitaria")
        If Not currentGridItem Is Nothing Then

            If Not String.IsNullOrWhiteSpace(omlCondSanitaria.Codice) Then

                Dim rowVacProg As DataRow = dt_vacProg.Rows.Find(GetRowKey(currentGridItem.ItemIndex))
                rowVacProg("ves_mal_codice_cond_sanitaria") = omlCondSanitaria.Codice

                ' Per ogni riga selezionata nel datagrid, imposto nella modale il valore specificato, se possibile
                For Each gridItem As DataGridItem In Me.dg_vacProg.Items

                    If gridItem.ItemType <> ListItemType.Header AndAlso gridItem.ItemType <> ListItemType.Footer AndAlso gridItem.ItemType <> ListItemType.Pager AndAlso
                       gridItem.ItemIndex <> currentGridItem.ItemIndex AndAlso
                       DirectCast(gridItem.FindControl("cb"), CheckBox).Checked Then

                        Dim list As List(Of Entities.PazienteCondizioneSanitaria) = Nothing

                        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                            Using bizMalattie As New BizMalattie(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                                Dim codiceVaccinazione As String = DirectCast(gridItem.FindControl("lblCodVac"), Label).Text

                                list = bizMalattie.GetCondizioniSanitariePazienteVaccinazione(OnVacUtility.Variabili.PazId, codiceVaccinazione)

                            End Using
                        End Using

                        If Not list.IsNullOrEmpty() AndAlso list.Any(Function(p) p.CodiceMalattia = omlCondSanitaria.Codice) Then

                            ' Posso impostare la condizione sanitaria selezionata anche per questa vaccinazione del paziente
                            Dim oml As OnitModalList = DirectCast(gridItem.FindControl("omlCondSanitaria"), OnitModalList)
                            oml.Codice = omlCondSanitaria.Codice
                            oml.Descrizione = omlCondSanitaria.Descrizione
                            oml.ValAltriCampi = omlCondSanitaria.ValAltriCampi

                            ' N.B. : Il metodo oml.RefreshDataBind() in ogni riga manda in loop l'applicativo la seconda volta che viene eseguito (in uno stesso postback).

                            Dim row As DataRow = dt_vacProg.Rows.Find(GetRowKey(gridItem.ItemIndex))
                            If Not row Is Nothing Then row("ves_mal_codice_cond_sanitaria") = omlCondSanitaria.Codice

                        End If

                    End If

                Next

            End If
        End If

    End Sub

    Protected Sub omlCondRischio_SetUpFiletr(sender As Object)

        Dim codiceVaccinazioneCorrente As String = GetCodiceVaccinazioneCurrentRow(sender, "omlCondRischio")

        Dim omlCondRischio As OnitModalList = DirectCast(sender, OnitModalList)

        omlCondRischio.Filtro =
            String.Format("VCR_PAZ_CODICE = {0} AND VCR_VAC_CODICE = '{1}' ORDER BY Paz, Def, Descrizione",
                          OnVacUtility.Variabili.PazId, codiceVaccinazioneCorrente)

    End Sub

    Protected Sub omlCondRischio_Change(Sender As Object, e As OnitModalList.ModalListaEventArgument)

        Dim omlCondRischio As OnitModalList = DirectCast(Sender, OnitModalList)
        omlCondRischio.ToolTip = omlCondRischio.Descrizione

        Dim currentGridItem As DataGridItem = GetCurrentDataGridVacProgItem(Sender, "omlCondRischio")
        If Not currentGridItem Is Nothing Then

            Dim rowVacProg As DataRow = dt_vacProg.Rows.Find(GetRowKey(currentGridItem.ItemIndex))
            rowVacProg("ves_rsc_codice") = omlCondRischio.Codice

            ' Per ogni riga selezionata nel datagrid, imposto nella modale il valore specificato, se possibile
            For Each gridItem As DataGridItem In dg_vacProg.Items

                If gridItem.ItemType <> ListItemType.Header AndAlso gridItem.ItemType <> ListItemType.Footer AndAlso gridItem.ItemType <> ListItemType.Pager AndAlso
                   gridItem.ItemIndex <> currentGridItem.ItemIndex AndAlso
                   DirectCast(gridItem.FindControl("cb"), CheckBox).Checked Then

                    Dim list As List(Of Entities.PazienteCondizioneRischio) = Nothing

                    Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                        Using bizRischio As New BizCategorieRischio(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                            Dim codiceVaccinazione As String = DirectCast(gridItem.FindControl("lblCodVac"), Label).Text

                            list = bizRischio.GetCondizioniRischioPazienteVaccinazione(OnVacUtility.Variabili.PazId, codiceVaccinazione)

                        End Using
                    End Using

                    If Not list.IsNullOrEmpty() AndAlso list.Any(Function(p) p.CodiceRischio = omlCondRischio.Codice) Then

                        ' Posso impostare la condizione di rischio selezionata anche per questa vaccinazione del paziente
                        Dim oml As OnitModalList = DirectCast(gridItem.FindControl("omlCondRischio"), OnitModalList)
                        oml.Codice = omlCondRischio.Codice
                        oml.Descrizione = omlCondRischio.Descrizione
                        oml.ValAltriCampi = omlCondRischio.ValAltriCampi

                        ' N.B. : Il metodo oml.RefreshDataBind() in ogni riga manda in loop l'applicativo la seconda volta che viene eseguito (in uno stesso postback).

                        Dim row As DataRow = dt_vacProg.Rows.Find(GetRowKey(gridItem.ItemIndex))
                        If Not row Is Nothing Then row("ves_rsc_codice") = omlCondRischio.Codice

                    End If

                End If

            Next

        End If

    End Sub

    Private Function GetCodiceVaccinazioneCurrentRow(sender As Object, controlId As String) As String

        Dim codiceVaccinazioneCorrente As String = String.Empty

        Dim currentGridItem As DataGridItem = GetCurrentDataGridVacProgItem(sender, controlId)

        If Not currentGridItem Is Nothing Then
            codiceVaccinazioneCorrente = DirectCast(currentGridItem.FindControl("lblCodVac"), Label).Text
        End If

        Return codiceVaccinazioneCorrente

    End Function

#End Region

#Region " Note Vaccinazione "

    ' Apertura modale delle note
    Protected Sub btnNoteVac_Click(sender As Object, e As ImageClickEventArgs)

        ' Riga a cui appartiene l'imagebutton che ha sollevato l'evento
        Dim currentGridItem As DataGridItem = GetCurrentDataGridVacProgItem(sender, "btnNoteVac")

        If Not currentGridItem Is Nothing Then

            hidRowIndex.Value = currentGridItem.ItemIndex.ToString()

            ' Apertura modale note
            modNoteVac.VisibileMD = True

            ' Impostazione dati della riga corrente nei campi della modale
            Dim rowVacProg As DataRow = dt_vacProg.Rows.Find(GetRowKey(currentGridItem.ItemIndex))

            If Not rowVacProg Is Nothing Then
                txtNoteVac.Text = rowVacProg("ves_note").ToString()
            Else
                txtNoteVac.Text = String.Empty
            End If

        End If

    End Sub

    Protected Sub btnNoteVacOk_Click(sender As Object, e As EventArgs)

        btnNoteVacModale_Click(True)

    End Sub

    Protected Sub btnNoteVacAnnulla_Click(sender As Object, e As EventArgs)

        btnNoteVacModale_Click(False)

    End Sub

    Private Sub btnNoteVacModale_Click(ok As Boolean)

        If ok Then
            '--
            ' Premuto OK
            '--
            Dim noteVaccinazione As String = Me.txtNoteVac.Text.TrimEnd()

            ' Se il campo note supera la maxLenght impostata, vengono tagliati i caratteri in eccedenza.
            If noteVaccinazione.Length > txtNoteVac.MaxLength Then
                noteVaccinazione = noteVaccinazione.Substring(0, txtNoteVac.MaxLength)
            End If

            txtNoteVac.Text = noteVaccinazione

            If Not String.IsNullOrWhiteSpace(hidRowIndex.Value) Then

                Dim gridItemIndex As Integer = Convert.ToInt32(hidRowIndex.Value)

                Dim rowVacProg As DataRow = dt_vacProg.Rows.Find(GetRowKey(gridItemIndex))
                If Not rowVacProg Is Nothing Then

                    ' Aggiornamento campo note della riga del dt
                    rowVacProg("ves_note") = noteVaccinazione

                    ' Aggiornamento campo note delle altre righe del dt (x stesso nome commerciale)
                    CopiaNoteVaccinazionePerNomeCommerciale(rowVacProg)

                    ' Cancellazione campo note della modale
                    txtNoteVac.Text = String.Empty

                    ' Bind datagrid per aggiornamento campi modificati (vengono aggiornate anche le icone dei pulsanti Note)
                    BindDGVacProgrammate()

                End If
            End If
        End If

        txtNoteVac.Text = String.Empty
        hidRowIndex.Value = String.Empty
        modNoteVac.VisibileMD = False

    End Sub

#End Region

    Private Function GetDdlSelectedTextFromGrid(dataGridItemIndex As Integer, ddlId As String) As String

        Dim value As String = String.Empty

        Dim ddl As DropDownList = DirectCast(Me.dg_vacProg.Items(dataGridItemIndex).FindControl(ddlId), DropDownList)
        If Not ddl Is Nothing AndAlso ddl.SelectedIndex > -1 Then
            value = ddl.SelectedItem.Text
        End If

        Return value

    End Function

    Private Sub CopiaSitoInoculazioneVaccinazionePerNomeCommerciale(rowVacProg As DataRow)

        Dim codCicRow As String = String.Empty

        If Not rowVacProg.IsNull("vpr_cic_codice") Then
            codCicRow = rowVacProg("vpr_cic_codice")
        End If

        ' Codice del NC relativa alla riga modificata
        Dim codNC As String = rowVacProg("ves_noc_codice").ToString()

        If Not String.IsNullOrWhiteSpace(codNC) Then

            Dim codCicDg As String = String.Empty
            For count As Integer = 0 To dt_vacProg.Rows.Count - 1

                If dt_vacProg.Rows(count).RowState <> DataRowState.Deleted Then

                    ' Modifico solo le righe in cui l'associazione è uguale a quella della riga modificata
                    If codNC = dt_vacProg.Rows(count)("ves_noc_codice").ToString() Then

                        codCicDg = String.Empty

                        If Not dt_vacProg.Rows(count).IsNull("vpr_cic_codice") Then
                            codCicDg = dt_vacProg.Rows(count)("vpr_cic_codice").ToString()
                        End If

                        If Not (rowVacProg("vpr_vac_codice").ToString() = dt_vacProg.Rows(count)("vpr_vac_codice").ToString() And codCicRow = codCicDg) Then

                            dt_vacProg.Rows(count)("ves_sii_codice") = rowVacProg("ves_sii_codice")
                            dt_vacProg.Rows(count)("sii_descrizione") = rowVacProg("sii_descrizione")
                            dt_vacProg.Rows(count)(NomeFlagTipoValorizzazione.SITO) = rowVacProg(NomeFlagTipoValorizzazione.SITO)

                        End If

                    End If

                End If

            Next
        End If

    End Sub

    Private Sub CopiaViaSomministrazioneVaccinazionePerNomeCommerciale(rowVacProg As DataRow)

        Dim codCicRow As String = String.Empty

        If Not rowVacProg.IsNull("vpr_cic_codice") Then
            codCicRow = rowVacProg("vpr_cic_codice")
        End If

        ' Codice del NC relativa alla riga modificata
        Dim codNC As String = rowVacProg("ves_noc_codice").ToString()

        If Not String.IsNullOrWhiteSpace(codNC) Then

            Dim codCicDg As String = String.Empty

            For count As Integer = 0 To dt_vacProg.Rows.Count - 1

                If dt_vacProg.Rows(count).RowState <> DataRowState.Deleted Then

                    ' Modifico solo le righe in cui l'associazione è uguale a quella della riga modificata
                    If codNC = dt_vacProg.Rows(count)("ves_noc_codice").ToString() Then

                        codCicDg = String.Empty

                        If Not dt_vacProg.Rows(count).IsNull("vpr_cic_codice") Then
                            codCicDg = dt_vacProg.Rows(count)("vpr_cic_codice")
                        End If

                        If Not (rowVacProg("vpr_vac_codice").ToString() = dt_vacProg.Rows(count)("vpr_vac_codice").ToString() And codCicRow = codCicDg) Then

                            dt_vacProg.Rows(count)("ves_vii_codice") = rowVacProg("ves_vii_codice")
                            dt_vacProg.Rows(count)("vii_descrizione") = rowVacProg("vii_descrizione")
                            dt_vacProg.Rows(count)(NomeFlagTipoValorizzazione.VIA) = rowVacProg(NomeFlagTipoValorizzazione.VIA)

                        End If

                    End If

                End If

            Next

        End If

    End Sub

#Region " Dati Pagamento (importo ed esenzione) "

    Protected Sub lnkPagVac_Click(sender As Object, e As EventArgs)

        ' Riga a cui appartiene l'imagebutton che ha sollevato l'evento
        Dim currentGridItem As DataGridItem = GetCurrentDataGridVacProgItem(sender, "lnkPagVac")

        If Not currentGridItem Is Nothing Then

            hidRowIndex.Value = currentGridItem.ItemIndex.ToString()

            ' Apertura modale dati di pagamento
            modPagVac.VisibileMD = True

            ddlTipiPagVac.ClearSelection()
            ddlEseMalPagVac.ClearSelection()
            valImpPagVac.Text = String.Empty

            ' Impostazione dati della riga corrente nei campi della modale
            Dim rowVacProg As DataRow = dt_vacProg.Rows.Find(GetRowKey(currentGridItem.ItemIndex))

            If Not rowVacProg Is Nothing Then

                ' FLAG ESENZIONE e IMPORTO: 0/NULL = Disabilitato; 1 = Abilitato; 2 = Obbligatorio
                ' Abilitazione del campo determinata al momento della selezione del lotto, in base al condizioni di pagamento o al default sul tipo pagamento. 
                ' Viene sovrascritta da quella definita nelle condizioni di pagamento, se presenti, o da quella di default per il tipo pagamento selezionato
                Dim flagStatoEsenzione As Enumerators.StatoAbilitazioneCampo = GetStatoAbilitazione(rowVacProg("flag_esenzione"))
                Dim flagStatoImporto As Enumerators.StatoAbilitazioneCampo = GetStatoAbilitazione(rowVacProg("flag_importo"))

                ' TIPO PAGAMENTO
                If Not rowVacProg.IsNull("noc_tpa_guid_tipi_pagamento") Then

                    ddlTipiPagVac.SelectedValue = New Guid(DirectCast(rowVacProg("noc_tpa_guid_tipi_pagamento"), Byte())).ToString()

                    Dim result As GetStatoAbilitazioneCampiResult = GetStatoAbilitazioneCampi(GetTipoPagamentoSelezionato(), rowVacProg("ves_noc_codice").ToString())

                    If result IsNot Nothing Then
                        flagStatoEsenzione = If(result.FlagStatoEsenzione, Enumerators.StatoAbilitazioneCampo.Disabilitato)
                        flagStatoImporto = If(result.FlagStatoImporto, Enumerators.StatoAbilitazioneCampo.Disabilitato)
                    End If

                End If

                ' Dropdown list esenzione
                Dim codiceMalattia As String = String.Empty
                If Not rowVacProg.IsNull("ves_mal_codice_malattia") Then codiceMalattia = rowVacProg("ves_mal_codice_malattia").ToString()

                Dim codiceEsenzione As String = String.Empty
                If Not rowVacProg.IsNull("ves_codice_esenzione") Then codiceEsenzione = rowVacProg("ves_codice_esenzione").ToString()

                SetEsenzionePagamento(flagStatoEsenzione, codiceMalattia, codiceEsenzione)

                ' Campo importo
                SetImportoPagamento(flagStatoImporto, rowVacProg("ves_importo").ToString())

                'Abilitazione dropdown tab pagamento
                EnableDdlEseMalPag(False)

            End If
        End If

    End Sub

    Private Class GetStatoAbilitazioneCampiResult
        Public FlagStatoEsenzione As Enumerators.StatoAbilitazioneCampo?
        Public FlagStatoImporto As Enumerators.StatoAbilitazioneCampo?
    End Class

    Private Function GetStatoAbilitazioneCampi(tipoPagamentoSelezionato As Entities.TipiPagamento, codiceNomeCommerciale As String) As GetStatoAbilitazioneCampiResult

        If tipoPagamentoSelezionato Is Nothing Then Return Nothing

        Dim listCondizioni As List(Of Entities.CondizioniPagamento) = Nothing

        If tipoPagamentoSelezionato.HasCondizioniPagamento = "S" Then

            ' Recupero condizioni in base al tipo di pagamento selezionato e all'età del paziente
            Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                Using bizNomiCommerciali As New BizNomiCommerciali(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                    listCondizioni = bizNomiCommerciali.GetListCondizioniPagamento(codiceNomeCommerciale, Convert.ToInt64(OnVacUtility.Variabili.PazId))

                End Using
            End Using

        End If

        Dim result As New GetStatoAbilitazioneCampiResult()

        If listCondizioni.IsNullOrEmpty() Then
            result.FlagStatoEsenzione = If(tipoPagamentoSelezionato.FlagStatoCampoEsenzione, Enumerators.StatoAbilitazioneCampo.Disabilitato)
            result.FlagStatoImporto = If(tipoPagamentoSelezionato.FlagStatoCampoImporto, Enumerators.StatoAbilitazioneCampo.Disabilitato)
        Else
            result.FlagStatoEsenzione = listCondizioni.First().StatoAbilitazioneEsenzione
            result.FlagStatoImporto = listCondizioni.First().StatoAbilitazioneImporto
        End If

        Return result

    End Function

    ''' <summary>
    ''' DropDownList esenzione nel tab del pagamento => l'esenzione selezionata deve essere impostata in tutte le programmate
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ddlEseMalPag_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlEseMalPag.SelectedIndexChanged
        '--
        If Not String.IsNullOrEmpty(Me.ddlEseMalPag.SelectedValue) Then
            '--
            Dim eseChanged As Boolean = False
            '--
            For Each rowVacProg As DataRow In dt_vacProg.Rows
                '--
                If rowVacProg.RowState <> DataRowState.Deleted AndAlso
                   GetStatoAbilitazione(rowVacProg("flag_esenzione")) <> Enumerators.StatoAbilitazioneCampo.Disabilitato AndAlso
                   Not Me.IsEsclusaEseguita(rowVacProg) Then
                    '--
                    Dim codiceEsenzioneMalattia As String() = Me.ddlEseMalPag.SelectedValue.Split("|")
                    rowVacProg("ves_mal_codice_malattia") = codiceEsenzioneMalattia(0)
                    rowVacProg("ves_codice_esenzione") = codiceEsenzioneMalattia(1)
                    '--
                    eseChanged = True
                    '--
                End If
                '--
            Next
            '--
            If eseChanged Then BindDGVacProgrammate()
            '--
        End If
        '--
    End Sub

    ''' <summary>
    ''' DropDownList Tipo Pagamento nella modale dei dati di pagamento. Vengono applicate, ai campi importo ed esenzione, le configurazioni del tipo selezionato.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ddlTipiPagVac_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlTipiPagVac.SelectedIndexChanged

        If Not String.IsNullOrWhiteSpace(hidRowIndex.Value) Then

            Dim rowIndex As Integer = Convert.ToInt32(hidRowIndex.Value)

            Dim row As DataRow = dt_vacProg.Rows.Find(GetRowKey(rowIndex))

            Dim codiceNomeCommerciale As String = String.Empty

            If Not row.IsNull("ves_noc_codice") Then
                codiceNomeCommerciale = row("ves_noc_codice").ToString()
            End If

            If Not String.IsNullOrWhiteSpace(codiceNomeCommerciale) Then

                Dim tipoPagamento As Entities.TipiPagamento = GetTipoPagamentoSelezionato()
                If tipoPagamento Is Nothing Then Return

                Dim flagStatoEsenzione As Enumerators.StatoAbilitazioneCampo = tipoPagamento.FlagStatoCampoEsenzione
                Dim flagStatoImporto As Enumerators.StatoAbilitazioneCampo = tipoPagamento.FlagStatoCampoImporto

                Dim valoreImporto As String = String.Empty

                If tipoPagamento.AutoSetImporto = "S" And tipoPagamento.FlagStatoCampoImporto <> Enumerators.StatoAbilitazioneCampo.Disabilitato Then

                    Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                        Using bizNomiCommerciali As New BizNomiCommerciali(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                            valoreImporto = bizNomiCommerciali.GetCostoUnitarioNomeCommerciale(codiceNomeCommerciale).ToString()

                        End Using
                    End Using

                End If

                Dim result As GetStatoAbilitazioneCampiResult = GetStatoAbilitazioneCampi(tipoPagamento, codiceNomeCommerciale)

                If result IsNot Nothing Then
                    flagStatoEsenzione = If(result.FlagStatoEsenzione, Enumerators.StatoAbilitazioneCampo.Disabilitato)
                    flagStatoImporto = If(result.FlagStatoImporto, Enumerators.StatoAbilitazioneCampo.Disabilitato)
                End If

                SetEsenzionePagamento(flagStatoEsenzione, String.Empty, String.Empty)
                SetImportoPagamento(flagStatoImporto, valoreImporto)

            End If
        End If

    End Sub

    Protected Sub btnPagVacOk_Click(sender As Object, e As EventArgs)

        btnPagVacModale_Click(True)

    End Sub

    Protected Sub btnPagVacAnnulla_Click(sender As Object, e As EventArgs)

        btnPagVacModale_Click(False)

    End Sub

    Private Sub btnPagVacModale_Click(ok As Boolean)

        If ok Then

            Dim numberFormatInfo As Globalization.NumberFormatInfo = OnVacUtility.GetNumberFormatInfo(True)

            Me.valImpPagVac.Text = Me.valImpPagVac.Text.Trim().
                Replace(",", numberFormatInfo.CurrencyDecimalSeparator).
                Replace(".", numberFormatInfo.CurrencyDecimalSeparator)

            If Not IsImportoValido(valImpPagVac.Text) Then
                ShowAlertMessagge("Importo specificato non valido")
                Return
            End If

            If Not String.IsNullOrWhiteSpace(Me.hidRowIndex.Value) Then

                Dim gridItemIndex As Integer = Convert.ToInt32(Me.hidRowIndex.Value)

                Dim rowVacProg As DataRow = dt_vacProg.Rows.Find(Me.GetRowKey(gridItemIndex))
                If Not rowVacProg Is Nothing Then

                    ' Aggiornamento campi esenzione e malattia della riga del dt
                    If String.IsNullOrWhiteSpace(Me.ddlEseMalPagVac.SelectedValue) Then
                        rowVacProg("ves_mal_codice_malattia") = DBNull.Value
                        rowVacProg("ves_codice_esenzione") = DBNull.Value
                    Else
                        Dim codiceEsenzioneMalattia As String() = Me.ddlEseMalPagVac.SelectedValue.Split("|")
                        rowVacProg("ves_mal_codice_malattia") = codiceEsenzioneMalattia(0)
                        rowVacProg("ves_codice_esenzione") = codiceEsenzioneMalattia(1)
                    End If

                    ' Aggiornamento campo importo della riga del dt
                    If String.IsNullOrWhiteSpace(Me.valImpPagVac.Text) Then
                        rowVacProg("ves_importo") = DBNull.Value
                    Else
                        rowVacProg("ves_importo") = Double.Parse(Me.valImpPagVac.Text)
                    End If

                    ' Aggiornamento campo tipo pagamento della riga del dt
                    If String.IsNullOrWhiteSpace(Me.ddlTipiPagVac.SelectedValue) Then

                        rowVacProg("noc_tpa_guid_tipi_pagamento") = DBNull.Value

                        ' N.B. : flag_importo e flag_esenzione vengono utilizzati per gestire la visualizzazione del pulsante di pagamento nella griglia.
                        '        Devono essere impostati solo al momento dell'impostazione del lotto, e non sovrascritti dalla scelta dell'utente.
                        'rowVacProg("flag_importo") = DBNull.Value
                        'rowVacProg("flag_esenzione") = DBNull.Value

                    Else

                        ' Se il tipo di pagamento è stato variato, allora recupero i flag relativi
                        If Not rowVacProg.IsNull("noc_tpa_guid_tipi_pagamento") Then
                            '--
                            Dim tipoPagamentoSelezionato As Entities.TipiPagamento = GetTipoPagamento(Me.ddlTipiPagVac.SelectedValue)
                            '--
                            If Not tipoPagamentoSelezionato Is Nothing Then

                                rowVacProg("noc_tpa_guid_tipi_pagamento") = tipoPagamentoSelezionato.GuidPagamento.ToByteArray()

                                ' N.B. : flag_importo e flag_esenzione vengono utilizzati per gestire la visualizzazione del pulsante di pagamento nella griglia.
                                '        Devono essere impostati solo al momento dell'impostazione del lotto, e non sovrascritti dalla scelta dell'utente.
                                'rowVacProg("flag_importo") = DirectCast(tipoPagamentoSelezionato.FlagStatoCampoImporto.Value, Integer)
                                'rowVacProg("flag_esenzione") = DirectCast(tipoPagamentoSelezionato.FlagStatoCampoEsenzione.Value, Integer)

                            End If
                            '--
                        End If
                    End If

                    ' Aggiornamento campi relativi al pagamento delle altre righe del dt (x stesso nome commerciale)
                    CopiaPagamentoVaccinazionePerNomeCommercialeEdImpostaToolTip(gridItemIndex, rowVacProg)

                    BindDGVacProgrammate()

                End If
            End If
        End If

        ' Pulizia campi modale
        ddlTipiPagVac.ClearSelection()
        ddlEseMalPagVac.ClearSelection()
        valImpPagVac.Text = String.Empty

        EnableDdlEseMalPag(True)

        hidRowIndex.Value = String.Empty
        modPagVac.VisibileMD = False

    End Sub

    Private Function GetValoreImportoByNomeCommercialeETipoPagamento(codiceNomeCommerciale As String, tipoPagamento As Entities.TipiPagamento) As String

        If String.IsNullOrWhiteSpace(codiceNomeCommerciale) Then Return String.Empty
        If tipoPagamento Is Nothing Then Return String.Empty

        Dim valoreImporto As String = String.Empty

        If tipoPagamento.AutoSetImporto = "S" And tipoPagamento.FlagStatoCampoImporto <> Enumerators.StatoAbilitazioneCampo.Disabilitato Then

            Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                Using bizNomiCommerciali As New BizNomiCommerciali(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                    valoreImporto = bizNomiCommerciali.GetCostoUnitarioNomeCommerciale(codiceNomeCommerciale).ToString()

                End Using
            End Using

        End If

        Return valoreImporto

    End Function

#End Region

#Region " Info Vaccinazione "


    Protected Sub btnInfoVac_Click(sender As Object, e As ImageClickEventArgs)

        ' Riga a cui appartiene l'imagebutton che ha sollevato l'evento
        Dim currentGridItem As DataGridItem = GetCurrentDataGridVacProgItem(sender, "btnInfoVac")

        If Not currentGridItem Is Nothing Then

            Dim rowVacProg As DataRow = dt_vacProg.Rows.Find(GetRowKey(currentGridItem.ItemIndex))

            If Not rowVacProg Is Nothing Then

                lblInfoVacc.Text = String.Format("{0} - {1}", rowVacProg("vpr_vac_codice").ToString(), rowVacProg("vac_descrizione").ToString())

                If rowVacProg.IsNull("vpr_n_richiamo") Then
                    lblInfoDoseVacc.Text = "-"
                Else
                    lblInfoDoseVacc.Text = rowVacProg("vpr_n_richiamo").ToString()
                End If

                lblInfoAss.Text = String.Format("{0} - {1}", rowVacProg("vpr_ass_codice").ToString(), rowVacProg("ass_descrizione").ToString())

                If rowVacProg.IsNull("ves_ass_n_dose") Then
                    lblInfoDoseAss.Text = "-"
                Else
                    lblInfoDoseAss.Text = rowVacProg("ves_ass_n_dose").ToString()
                End If

                lblInfoCiclo.Text = String.Format("{0} - {1}", rowVacProg("vpr_cic_codice").ToString(), rowVacProg("cic_descrizione").ToString())

                If rowVacProg.IsNull("vpr_n_seduta") Then
                    lblInfoSeduta.Text = "-"
                Else
                    lblInfoSeduta.Text = rowVacProg("vpr_n_seduta").ToString()
                End If

                If rowVacProg.IsNull("ope_vac") Then
                    lblInfoVaccinatore.Text = "-"
                Else
                    lblInfoVaccinatore.Text = rowVacProg("ope_vac").ToString()
                End If

                If String.IsNullOrWhiteSpace(OnVacUtility.Variabili.MedicoResponsabile.Nome) Then
                    lblInfoResponsabile.Text = "-"
                Else
                    lblInfoResponsabile.Text = OnVacUtility.Variabili.MedicoResponsabile.Nome
                End If

                modInfoVacc.VisibileMD = True

            End If
        End If

    End Sub

#End Region

#Region " OnitLayout "

    Private Sub OnitLayout31_AlertClick(sender As Object, e As Controls.PagesLayout.OnitLayout3.AlertEventArgs) Handles OnitLayout31.AlertClick

        Dim askAutoCnv As Boolean = False

        Dim keyValues() As String = e.Key.Split("|")

        If keyValues.Count > 1 Then

            Try
                askAutoCnv = Convert.ToBoolean(keyValues(1))
            Catch ex As Exception
                askAutoCnv = False
            End Try

        End If

        Me.RedirectToConvocazioniPaziente(OnVacUtility.Variabili.PazId, Me.EditConvocazioni, askAutoCnv, Nothing, Nothing)

    End Sub

    Private Sub OnitLayout31_ConfirmClick(sender As Object, eventArgs As Onit.Controls.PagesLayout.OnitLayout3.ConfirmEventArgs) Handles OnitLayout31.ConfirmClick

        Select Case eventArgs.Key

            Case "EliminaConvocazione"

                If eventArgs.Result Then

                    Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                        Using bizVaccinazioneProg As New BizVaccinazioneProg(genericProvider, Settings, OnVacContext.CreateBizContextInfos, Nothing)

                            Dim command As New BizVaccinazioneProg.EliminaProgrammazioneCommand()
                            command.CodicePaziente = Convert.ToInt64(OnVacUtility.Variabili.PazId)
                            command.DataConvocazione = DataConvocazione
                            command.EliminaAppuntamenti = True
                            command.EliminaBilanci = False
                            command.EliminaSollecitiBilancio = True
                            command.TipoArgomentoLog = DataLogStructure.TipiArgomento.ELIMINA_PROG
                            command.OperazioneAutomatica = False
                            command.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.EliminazioneConvocazione
                            command.NoteEliminazione = "Eliminazione di tutte le vaccinazioni che compongono la convocazione (da maschera Vaccinazioni Programmate)"

                            ' ----------- '
                            ' TODO [AVN - modifiche per aggiunta campi]: OnVac utilizza EliminaProgrammazioneNoTransactionScope
                            bizVaccinazioneProg.EliminaProgrammazione(command)

                        End Using
                    End Using

                    Me.RedirectToConvocazioniPaziente(OnVacUtility.Variabili.PazId, Me.EditConvocazioni, True, Nothing, Nothing)

                Else

                    Me.ShowInserisciAssociazione()

                End If

            Case "escludiBilancio"

                If eventArgs.Result Then

                    Me.Salva()

                End If

        End Select

    End Sub

#End Region

#End Region

#Region " Private & Protected "

#Region " Convocazioni "

    ''' <summary>
    ''' Controllo della coerenza di una convocazione. Il controllo non contempla (purtroppo) tutti i casi.
    ''' </summary>
    Private Sub ControllaStatoConvocazione()

        Dim msg As Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox

        If dt_vacProg Is Nothing OrElse dt_vacProg.Rows.Count = 0 Then

            If Not Settings.GESBIL Then

                msg = New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Attenzione!\nNon può esistere una convocazione senza vaccinazioni associate (controllare anche che l'associazione inserita abbia associate delle vaccinazioni)!\nLa convocazione verrà eliminata. Continuare?", "EliminaConvocazione", True, True)

            Else

                If (dt_bilProg Is Nothing OrElse dt_bilProg.Rows.Count = 0) Then

                    msg = New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Attenzione!\nNon può esistere una convocazione senza vaccinazioni o bilanci associati!\nLa convocazione verrà eliminata. Continuare?", "EliminaConvocazione", True, True)

                Else

                    msg = New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Attenzione!\nLa convocazione non ha vaccinazioni associate.", "ConvocazioneSoloBilancio", False, False)

                End If

            End If

            Me.OnitLayout31.ShowMsgBox(msg)

        End If

    End Sub

#End Region

#Region " Lotti "

    Private Sub ImpostaLottiUtilizzatiDefaultSeNecessario(valorizzaDatiPagamento As Boolean)

        If Settings.GESMAG Then

            Dim lottiUtilizzatiDefault As New List(Of String)()

            ' Controllo sul lotto attivo: se il programma prevede l'utilizzo della pistola è necessario caricare tutti i lotti senza distinzione
            Dim dtLottiUtilizzabili As DataTable = Me.RecuperaLottiUtilizzabili(Not Settings.GESBALOT, False, Settings.PAGAMENTO)

            For Each drvVacProg As DataRowView In dt_vacProg.DefaultView

                Dim codiceVaccinazione As String = drvVacProg("vpr_vac_codice").ToString()
                Dim codiceAssociazione As String = drvVacProg("vpr_ass_codice").ToString()

                For i As Integer = 0 To dtLottiUtilizzabili.DefaultView.Count - 1

                    Dim valVac As String() = dtLottiUtilizzabili.DefaultView(i)("valVac").ToString().Split("|")

                    If Array.IndexOf(valVac, codiceVaccinazione) > -1 Then

                        lottiUtilizzatiDefault.Add(dtLottiUtilizzabili.DefaultView(i)("codLotto").ToString())
                        Exit For

                    End If

                Next

            Next

            'se la gestione è con la pistola non deve associare i lotti di default
            ImpostaLottiUtilizzati(lottiUtilizzatiDefault.ToArray(), dtLottiUtilizzabili, Not Settings.GESBALOT, valorizzaDatiPagamento)

        End If

    End Sub

    Private Sub ImpostaLottiUtilizzati(lottiUtilizzati As String(), dtLottiUtilizzabili As DataTable, associaLotti As Boolean, valorizzaDatiPagamento As Boolean)

        If associaLotti Then

            CodiceLottiUtilizzati = New List(Of String)(lottiUtilizzati)

            ' Associa i lotti di default alle vaccinazioni da eseguire
            For Each rowVacProg As DataRow In dt_vacProg.Rows
                '--
                If Not rowVacProg.RowState = DataRowState.Deleted AndAlso Not Me.IsEsclusaEseguita(rowVacProg) Then
                    '--
                    For i As Int16 = 0 To CodiceLottiUtilizzati.Count - 1
                        '--
                        Dim rowLotto As DataRow = dtLottiUtilizzabili.Rows.Find(New Object() {CodiceLottiUtilizzati(i)})
                        '--
                        Dim valVac As String() = rowLotto("valVac").ToString().Split("|")
                        '--
                        If Array.IndexOf(valVac, rowVacProg("vpr_vac_codice")) > -1 Then
                            '--
                            rowVacProg("ves_lot_codice") = rowLotto("codLotto")
                            rowVacProg("ves_lot_data_scadenza") = rowLotto("scadenza")
                            rowVacProg("noc_descrizione") = rowLotto("descNC")
                            rowVacProg("ves_noc_codice") = rowLotto("codNC")
                            '--
                            rowVacProg("noc_tpa_guid_tipi_pagamento") = rowLotto("noc_tpa_guid_tipi_pagamento")
                            rowVacProg("flag_importo") = rowLotto("flag_importo")
                            rowVacProg("flag_esenzione") = rowLotto("flag_esenzione")
                            '--
                            rowVacProg("ves_mal_codice_malattia") = DBNull.Value
                            rowVacProg("ves_codice_esenzione") = DBNull.Value
                            rowVacProg("ves_importo") = rowLotto("importo")
                            '--
                            If Settings.SITO_INOCULAZIONE_SET_DEFAULT Or Settings.VIA_SOMMINISTRAZIONE_SET_DEFAULT Then
                                '--
                                ' Valorizzazione sito e/o via se flag non valorizzati o valorizzati in base al nome commerciale 
                                ' e se ci sono info di default per il nome commerciale selezionato
                                '--
                                If IsFlagSitoNullOrEqualTo(rowVacProg, Constants.TipoValorizzazioneSitoVia.DaNomeCommerciale) OrElse
                                   IsFlagViaNullOrEqualTo(rowVacProg, Constants.TipoValorizzazioneSitoVia.DaNomeCommerciale) Then
                                    '--
                                    Dim infoSomministrazione As Entities.InfoSomministrazione = Nothing
                                    '--
                                    ' Ricerca sito e via di default per il nome commerciale
                                    Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                                        Using bizNomiCommerciali As New Biz.BizNomiCommerciali(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                                            infoSomministrazione = bizNomiCommerciali.GetInfoSomministrazioneDefaultByNomeCommerciale(rowVacProg("ves_noc_codice").ToString())
                                        End Using
                                    End Using
                                    '--
                                    If Not infoSomministrazione Is Nothing Then
                                        '--
                                        ' Se parametro true, sito non valorizzato ed esiste sito di default per questo nome commerciale => valorizzo
                                        If Settings.SITO_INOCULAZIONE_SET_DEFAULT AndAlso
                                           Not String.IsNullOrEmpty(infoSomministrazione.CodiceSitoInoculazione) AndAlso
                                           IsFlagSitoNullOrEqualTo(rowVacProg, Constants.TipoValorizzazioneSitoVia.DaNomeCommerciale) Then
                                            '--
                                            rowVacProg("ves_sii_codice") = infoSomministrazione.CodiceSitoInoculazione
                                            rowVacProg("sii_descrizione") = infoSomministrazione.DescrizioneSitoInoculazione
                                            rowVacProg(NomeFlagTipoValorizzazione.SITO) = infoSomministrazione.FlagTipoValorizzazioneSito
                                            '--
                                        End If
                                        '--
                                        ' Se parametro true, via non valorizzata ed esiste via di default per questo nome commerciale => valorizzo
                                        If Settings.VIA_SOMMINISTRAZIONE_SET_DEFAULT AndAlso
                                           Not String.IsNullOrEmpty(infoSomministrazione.CodiceViaSomministrazione) AndAlso
                                           IsFlagViaNullOrEqualTo(rowVacProg, Constants.TipoValorizzazioneSitoVia.DaNomeCommerciale) Then
                                            '--
                                            rowVacProg("ves_vii_codice") = infoSomministrazione.CodiceViaSomministrazione
                                            rowVacProg("vii_descrizione") = infoSomministrazione.DescrizioneViaSomministrazione
                                            rowVacProg(NomeFlagTipoValorizzazione.VIA) = infoSomministrazione.FlagTipoValorizzazioneVia
                                            '--
                                        End If
                                        '--
                                    End If
                                    '--
                                    Exit For
                                    '--
                                End If
                            End If
                            '--
                        End If
                        '--
                    Next
                    '--
                    If CodiceLottiUtilizzati.Count = 0 OrElse rowVacProg.IsNull("ves_lot_codice") OrElse Not CodiceLottiUtilizzati.Contains(rowVacProg("ves_lot_codice")) Then
                        rowVacProg("ves_lot_codice") = DBNull.Value
                        rowVacProg("noc_descrizione") = DBNull.Value
                        rowVacProg("ves_noc_codice") = DBNull.Value
                        rowVacProg("noc_tpa_guid_tipi_pagamento") = DBNull.Value
                        rowVacProg("flag_importo") = DBNull.Value
                        rowVacProg("flag_esenzione") = DBNull.Value
                        rowVacProg("ves_mal_codice_malattia") = DBNull.Value
                        rowVacProg("ves_codice_esenzione") = DBNull.Value
                        rowVacProg("ves_importo") = DBNull.Value
                    End If
                    '--
                End If
                '--
            Next

            If Settings.PAGAMENTO AndAlso valorizzaDatiPagamento Then

                Dim tipoPagamento As Entities.TipiPagamento = GetTipoPagamento(Settings.PAGAMENTO_AUTO_CHECK_TIPO)

                For i As Int16 = 0 To dt_vacProg.Rows.Count - 1

                    Dim row As DataRow = dt_vacProg.Rows(i)

                    If Not row.RowState = DataRowState.Deleted AndAlso Not IsEsclusaEseguita(row) Then

                        If row.IsNull("E") Then

                            If (row("ves_importo") Is Nothing OrElse row("ves_importo") Is DBNull.Value) AndAlso Not tipoPagamento Is Nothing Then

                                Dim valoreImporto As String = String.Empty

                                If Not row("ves_noc_codice") Is Nothing AndAlso Not row("ves_noc_codice") Is DBNull.Value Then
                                    valoreImporto = GetValoreImportoByNomeCommercialeETipoPagamento(row("ves_noc_codice"), tipoPagamento)
                                End If

                                If String.IsNullOrWhiteSpace(valoreImporto) Then
                                    row("ves_importo") = DBNull.Value
                                Else
                                    row("ves_importo") = Double.Parse(valoreImporto)
                                End If

                                row("noc_tpa_guid_tipi_pagamento") = tipoPagamento.GuidPagamento.ToByteArray()

                            End If

                        End If

                    End If
                Next

            End If

        Else
            '--
            Me.CodiceLottiUtilizzati = New List(Of String)()
            '--
        End If
        '--
    End Sub

    Private Function IsSitoValorizzato(row As DataRow) As Boolean

        If row("ves_sii_codice") Is Nothing Then Return False
        If row("ves_sii_codice") Is DBNull.Value Then Return False
        If String.IsNullOrWhiteSpace(row("ves_sii_codice")) Then Return False

        Return True

    End Function

    Private Function IsViaValorizzata(row As DataRow) As Boolean

        If row("ves_vii_codice") Is Nothing Then Return False
        If row("ves_vii_codice") Is DBNull.Value Then Return False
        If String.IsNullOrWhiteSpace(row("ves_vii_codice")) Then Return False

        Return True

    End Function

    Private Function IsFlagSitoNullOrEqualTo(row As DataRow, tipoValorizzazione As String) As Boolean

        If IsFlagNull(row, NomeFlagTipoValorizzazione.SITO) Then Return True
        If row(NomeFlagTipoValorizzazione.SITO).ToString() = tipoValorizzazione Then Return True

        Return False

    End Function

    Private Function IsFlagViaNullOrEqualTo(row As DataRow, tipoValorizzazione As String) As Boolean

        If IsFlagNull(row, NomeFlagTipoValorizzazione.VIA) Then Return True
        If row(NomeFlagTipoValorizzazione.VIA).ToString() = tipoValorizzazione Then Return True

        Return False

    End Function

    Private Function IsFlagNull(row As DataRow, nomeFlag As String) As Boolean

        If row(nomeFlag) Is Nothing Then Return True
        If row(nomeFlag) Is DBNull.Value Then Return True
        If String.IsNullOrWhiteSpace(row(nomeFlag)) Then Return True

        Return False

    End Function

    Private Class CoperturaControllata

        Public NomeCommerciale As String
        Public CoperturaOK As Boolean

        Public Sub New(nomeCommerciale As String, coperturaOK As Boolean)
            Me.NomeCommerciale = nomeCommerciale
            Me.CoperturaOK = coperturaOK
        End Sub

    End Class

    ''' <summary>
    ''' Restituisce true se, dato il nome commerciale specificato, tutte le vaccinazioni che dovrebbe coprire sono effettivamente associate 
    ''' a quel nome commerciale e sono selezionate per l'esecuzione.
    ''' </summary>
    ''' <param name="codiceNomeCommerciale"></param>
    ''' <param name="coperture"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ControllaCoperturaNomeCommerciale(codiceNomeCommerciale As String, coperture As List(Of BizVaccinazioneProg.CopertureNomiCommerciali), nomiCommercialiCopertureControllate As List(Of CoperturaControllata)) As Boolean

        Dim coperturaOK As Boolean = False

        If nomiCommercialiCopertureControllate Is Nothing Then nomiCommercialiCopertureControllate = New List(Of CoperturaControllata)()

        Dim item As CoperturaControllata = nomiCommercialiCopertureControllate.FirstOrDefault(Function(p) p.NomeCommerciale = codiceNomeCommerciale)
        If item Is Nothing Then

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizVaccinazioneProgrammata As New Biz.BizVaccinazioneProg(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                    coperturaOK = bizVaccinazioneProgrammata.IsCoperturaVaccinazioniCompleta(codiceNomeCommerciale, coperture)

                End Using
            End Using

            nomiCommercialiCopertureControllate.Add(New CoperturaControllata(codiceNomeCommerciale, coperturaOK))

        Else
            coperturaOK = item.CoperturaOK
        End If

        Return coperturaOK

    End Function

    Private Function ControllaSitoViaDiversiStessoNomeCommerciale(ByRef descrizioneSitoInoculazione As String, ByRef descrizioneViaSomministrazione As String, ByRef descrizioneNC As String) As Boolean

        For i As Int16 = 0 To Me.dg_vacProg.Items.Count - 1

            If DirectCast(Me.dg_vacProg.Items.Item(i).FindControl("cb"), CheckBox).Checked Then

                If DirectCast(Me.dg_vacProg.Items.Item(i).FindControl("lblDescNC"), Label).Text = descrizioneNC Then

                    If GetDdlSelectedTextFromGrid(i, "ddlVii") <> descrizioneViaSomministrazione OrElse
                       GetDdlSelectedTextFromGrid(i, "ddlSii") <> descrizioneSitoInoculazione Then

                        Return False

                    End If

                End If

            End If

        Next

        Return True

    End Function

    Private Function AvvisaQtaLotto(codiceLotto As String) As String

        If String.IsNullOrEmpty(codiceLotto) Then Return String.Empty

        Dim warningLotto As String = String.Empty

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizLotti As New BizLotti(genericProvider, Settings, New BizContextInfos(OnVacContext.UserId, OnVacContext.Azienda, OnVacContext.AppId, OnVacUtility.Variabili.CNS.Codice, OnVacContext.CodiceUslCorrente, Nothing))

                Dim bizLottiResult As BizLotti.BizLottiResult = bizLotti.ControlloDosiLottoDaEseguire(codiceLotto, OnVacUtility.Variabili.CNSMagazzino.Codice)

                If Not bizLottiResult Is Nothing Then
                    warningLotto = bizLottiResult.Message
                End If

            End Using
        End Using

        Return warningLotto

    End Function

#End Region

#Region " Esecuzione "

    Private Sub Esegui()
        '--
        Dim eseguita As Boolean = False
        '--
        Dim arVacNoEseguiteNoLotto As New ArrayList()
        Dim arVacNoEseguiteAltreVac As New ArrayList()
        Dim arVacNoEseguiteSitoViaDiversi As New ArrayList()
        '--
        Dim codLottiWarningQta As New ArrayList()
        Dim stbDeleteInadempienze As New System.Text.StringBuilder()
        '--
        If Not String.IsNullOrEmpty(jsLottiAssociazioniSingole) Then
            '--
            Dim ass As String() = jsLottiAssociazioniSingole.ToString.Split("|")
            '--
            For i As Integer = 0 To dt_vacProg.Rows.Count - 1
                '--
                If dt_vacProg.Rows(i).RowState <> DataRowState.Deleted Then
                    '--
                    For j As Integer = 0 To ass.Length - 1 Step 3
                        '--
                        If Not dt_vacProg.Rows(i)("ves_lot_codice") Is DBNull.Value AndAlso dt_vacProg.Rows(i)("ves_lot_codice").ToString() = ass(j) Then

                            dt_vacProg.Rows(i)("vpr_ass_codice") = ass(j + 1)
                            dt_vacProg.Rows(i)("ass_descrizione") = ass(j + 2)

                            ' Ricalcolo della dose di associazione
                            If Not dt_vacProg.Rows(i)("vpr_ass_codice") Is DBNull.Value Then

                                Using dbGenericProvider As New DbGenericProvider(OnVacContext.Connection)
                                    dt_vacProg.Rows(i)("ves_ass_n_dose") =
                                        dbGenericProvider.VaccinazioniEseguite.GetMaxDoseAssociazione(OnVacUtility.Variabili.PazId, dt_vacProg.Rows(i)("vpr_ass_codice")) + 1
                                End Using

                            End If
                            '-
                        End If
                        '--
                    Next
                    '--
                End If
                '--
            Next
            '--
        End If

#Region " Sospensione "

        ' Controllo Sospensione
        Dim dataEsecuzioneOld As DateTime

        For i As Integer = 0 To dg_vacProg.Items.Count - 1
            If DirectCast(dg_vacProg.Items.Item(i).FindControl("cb"), CheckBox).Checked Then

                If OnVacUtility.Variabili.DataEsecuzione.Date <> dataEsecuzioneOld Then

                    dataEsecuzioneOld = OnVacUtility.Variabili.DataEsecuzione.Date

                    Dim sospensioneResult As BizGenericResult = Nothing

                    Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                        Using bizVisite As New BizVisite(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                            sospensioneResult = bizVisite.CheckSospensioneVaccinazioniPaziente(Convert.ToInt64(OnVacUtility.Variabili.PazId), OnVacUtility.Variabili.DataEsecuzione.Date)

                        End Using
                    End Using

                    If Not sospensioneResult.Success Then
                        strJS &= String.Format("alert('{0}'){1}", HttpUtility.JavaScriptStringEncode(sospensioneResult.Message), Environment.NewLine)
                        Return
                    End If

                End If

            End If
        Next

#End Region

        ' Caricamento tipi di pagamento possibili, per il controllo relativo all'obbligatorietà dell'importo (se gestito)
        Dim listTipiPagamento As List(Of Entities.TipiPagamento) = Nothing

        If Settings.PAGAMENTO Then

            Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                Using bizNomiCommerciali As New BizNomiCommerciali(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                    listTipiPagamento = bizNomiCommerciali.GetListTipiPagamento()

                End Using
            End Using

        End If

        ' Aggiornamento valori dei controlli sempre in edit e controllo campi
        Dim datiObbligatoriMancanti As Boolean = False
        Dim messageSitoObbligatorio As New Text.StringBuilder()
        Dim messageViaObbligatoria As New Text.StringBuilder()
        Dim messageDatiPagamentoObbligatori As New System.Text.StringBuilder()
        Dim messageCondSanitariaObbligatoria As New Text.StringBuilder()
        Dim messageCondRischioObbligatoria As New Text.StringBuilder()

        Dim datiNonValidi As Boolean = False
        Dim messageDatiNonValidi As New System.Text.StringBuilder()

        For i As Integer = 0 To dg_vacProg.Items.Count - 1
            If DirectCast(dg_vacProg.Items(i).FindControl("cb"), CheckBox).Checked Then

                Dim row As DataRow = dt_vacProg.Rows.Find(GetRowKey(i))

                Dim value1 As String = GetDdlSelectedValueFromGrid(i, "ddlSii")
                If String.IsNullOrWhiteSpace(value1) Then
                    row("ves_sii_codice") = DBNull.Value
                    row(NomeFlagTipoValorizzazione.SITO) = String.Empty
                Else
                    row("ves_sii_codice") = value1
                    row(NomeFlagTipoValorizzazione.SITO) = Constants.TipoValorizzazioneSitoVia.Manuale
                End If

                CopiaSitoInoculazioneVaccinazionePerNomeCommerciale(row)

                value1 = GetDdlSelectedValueFromGrid(i, "ddlVii")
                If String.IsNullOrWhiteSpace(value1) Then
                    row("ves_vii_codice") = DBNull.Value
                    row(NomeFlagTipoValorizzazione.VIA) = String.Empty
                Else
                    row("ves_vii_codice") = value1
                    row(NomeFlagTipoValorizzazione.VIA) = Constants.TipoValorizzazioneSitoVia.Manuale
                End If

                CopiaViaSomministrazioneVaccinazionePerNomeCommerciale(row)

                Dim oml As OnitModalList = GetModalListControlFromDgVacProg(i, "omlCondSanitaria")
                If oml Is Nothing Then
                    row("ves_mal_codice_cond_sanitaria") = DBNull.Value
                Else
                    row("ves_mal_codice_cond_sanitaria") = oml.Codice
                End If

                oml = GetModalListControlFromDgVacProg(i, "omlCondRischio")
                If oml Is Nothing Then
                    row("ves_rsc_codice") = DBNull.Value
                Else
                    row("ves_rsc_codice") = oml.Codice
                End If

                If Settings.CHECK_SITO_INOCULO Then

                    If row.IsNull("ves_sii_codice") OrElse String.IsNullOrWhiteSpace(row("ves_sii_codice").ToString()) Then
                        messageSitoObbligatorio.Append(GetStringVaccinazioneDose(row))
                        datiObbligatoriMancanti = True
                    End If

                End If

                If Settings.CHECK_VIA_SOMMINISTRAZIONE Then

                    If row.IsNull("ves_vii_codice") OrElse String.IsNullOrWhiteSpace(row("ves_vii_codice").ToString()) Then
                        messageViaObbligatoria.Append(GetStringVaccinazioneDose(row))
                        datiObbligatoriMancanti = True
                    End If

                End If

                If Settings.PAGAMENTO Then

                    ' I flag esenzione e importo vanno valutati in base a quelli relativi al tipo di pagamento impostato
                    Dim statoCampoEsenzioneCorrente As Enumerators.StatoAbilitazioneCampo = GetStatoAbilitazione(row("flag_esenzione"))
                    Dim statoCampoImportoCorrente As Enumerators.StatoAbilitazioneCampo = GetStatoAbilitazione(row("flag_importo"))

                    If Not row.IsNull("noc_tpa_guid_tipi_pagamento") Then

                        Dim tipoPagamentoCorrente As Entities.TipiPagamento = GetTipoPagamento(New Guid(DirectCast(row("noc_tpa_guid_tipi_pagamento"), Byte())))

                        Dim result As GetStatoAbilitazioneCampiResult = GetStatoAbilitazioneCampi(tipoPagamentoCorrente, row("ves_noc_codice").ToString())

                        If result IsNot Nothing Then
                            statoCampoEsenzioneCorrente = If(result.FlagStatoEsenzione, Enumerators.StatoAbilitazioneCampo.Disabilitato)
                            statoCampoImportoCorrente = If(result.FlagStatoImporto, Enumerators.StatoAbilitazioneCampo.Disabilitato)
                        End If

                    End If

                    ' Il controllo dei dati relativi al pagamento deve avvenire solo per i nomi commerciali aventi Flag Esenzione o Flag Importo impostati a obbligatori.
                    ' Flag Esenzione e Flag Importo possono valere: 0/NULL = Disabilitato; 1 = Abilitato; 2 = Obbligatorio
                    If statoCampoEsenzioneCorrente = Enumerators.StatoAbilitazioneCampo.Obbligatorio Then

                            If row("ves_codice_esenzione") Is Nothing OrElse row.IsNull("ves_codice_esenzione") Then
                            messageDatiPagamentoObbligatori.Append(GetStringVaccinazioneDose(row))
                            datiObbligatoriMancanti = True
                            End If

                        ElseIf statoCampoImportoCorrente = Enumerators.StatoAbilitazioneCampo.Obbligatorio Then

                            If row("ves_importo") Is Nothing OrElse row.IsNull("ves_importo") Then

                            messageDatiPagamentoObbligatori.Append(GetStringVaccinazioneDose(row))
                            datiObbligatoriMancanti = True

                            ElseIf Not IsImportoValido(row("ves_importo")) Then

                                If messageDatiNonValidi.Length = 0 Then
                                    messageDatiNonValidi.Append("Importo specificato non valido\n")
                                End If
                                datiNonValidi = True

                            End If

                        End If

                    End If

                    If Settings.CONDIZIONE_SANITARIA_OBBLIGATORIA Then

                    If row.IsNull("ves_mal_codice_cond_sanitaria") OrElse String.IsNullOrWhiteSpace(row("ves_mal_codice_cond_sanitaria").ToString()) Then
                        messageCondSanitariaObbligatoria.Append(GetStringVaccinazioneDose(row))
                        datiObbligatoriMancanti = True
                    End If

                End If

                If Settings.CONDIZIONE_RISCHIO_OBBLIGATORIA Then

                    If row.IsNull("ves_rsc_codice") OrElse String.IsNullOrWhiteSpace(row("ves_rsc_codice").ToString()) Then
                        messageCondRischioObbligatoria.Append(GetStringVaccinazioneDose(row))
                        datiObbligatoriMancanti = True
                    End If

                End If

            End If
        Next

        If datiObbligatoriMancanti OrElse datiNonValidi Then

            Dim message As New Text.StringBuilder("alert('ATTENZIONE: esecuzione non effettuata.\n")

            If datiObbligatoriMancanti Then

                message.Append("I seguenti campi sono OBBLIGATORI e non sono stati compilati:\n\n")

                If messageSitoObbligatorio.Length > 0 Then
                    message.AppendFormat("SITO DI INOCULAZIONE non compilato per le seguenti vaccinazioni:\n{0}\n\n", messageSitoObbligatorio)
                End If

                If messageViaObbligatoria.Length > 0 Then
                    message.AppendFormat("VIA DI SOMMINISTRAZIONE non compilata per le seguenti vaccinazioni:\n{0}\n\n", messageViaObbligatoria)
                End If

                If messageDatiPagamentoObbligatori.Length > 0 Then
                    message.AppendFormat("DATI DI PAGAMENTO non compilati per le seguenti vaccinazioni:\n{0}\n", messageDatiPagamentoObbligatori)
                    message.Append("Per impostarli, utilizzare il pulsante ""€"" (Pagamento) per la riga corrispondente.\n\n")
                End If

                If messageCondSanitariaObbligatoria.Length > 0 Then
                    message.AppendFormat("CONDIZIONE SANITARIA non compilata per le seguenti vaccinazioni:\n{0}\n", messageCondSanitariaObbligatoria)
                End If

                If messageCondRischioObbligatoria.Length > 0 Then
                    message.AppendFormat("CONDIZIONE DI RISCHIO non compilata per le seguenti vaccinazioni:\n{0}\n", messageCondRischioObbligatoria)
                End If

            End If

            If datiNonValidi Then

                message.AppendFormat("\n{0}\n", messageDatiNonValidi)

            End If

            message.Append("');")

            strJS &= message.ToString() + Environment.NewLine
            Return

        End If

        ' Replica dei controlli effettuati in registrazione vaccinazioni
        Dim controlList As New ControlloEsecuzione()

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using biz As New BizVaccinazioniEseguite(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                Dim dt_vacEseguite As DataTable = biz.GetDtVaccinazioniEseguite(OnVacUtility.Variabili.PazId)

                For i As Integer = 0 To dg_vacProg.Items.Count - 1

                    If DirectCast(dg_vacProg.Items.Item(i).FindControl("cb"), CheckBox).Checked Then

                        Dim row As DataRow = dt_vacProg.Rows.Find(GetRowKey(i))

                        If row.RowState <> DataRowState.Deleted Then

                            ' Aggiunta delle vaccinazioni programmate che si vuole eseguire 
                            ' al datatable delle vaccianzioni eseguite per i controlli di coerenza
                            Dim newRow As DataRow = dt_vacEseguite.NewRow()

                            newRow("ves_vac_codice") = row("vpr_vac_codice")
                            newRow("vac_descrizione") = row("vpr_vac_codice")
                            newRow("ves_n_richiamo") = row("vpr_n_richiamo")
                            newRow("ves_ass_codice") = row("vpr_ass_codice")
                            newRow("ves_ass_n_dose") = row("ves_ass_n_dose")
                            newRow("ves_sii_codice") = row("ves_sii_codice")
                            newRow("ves_vii_codice") = row("ves_vii_codice")
                            If Not row("ves_data_effettuazione") Is Nothing AndAlso Not row("ves_data_effettuazione") Is DBNull.Value Then
                                newRow("ves_data_effettuazione") = DirectCast(row("ves_data_effettuazione"), DateTime).Date
                            Else
                                newRow("ves_data_effettuazione") = DBNull.Value
                            End If
                            newRow("scaduta") = "N"
                            newRow("ves_mal_codice_cond_sanitaria") = row("ves_mal_codice_cond_sanitaria")
                            newRow("ves_rsc_codice") = row("ves_rsc_codice")
                            newRow("ves_tpa_guid_tipi_pagamento") = row("noc_tpa_guid_tipi_pagamento")
                            dt_vacEseguite.Rows.Add(newRow)

                        End If

                    End If
                Next

                ' Creo il datatable con le associazioni per effettuare i controlli  su di esse
                Dim dtAssociazioni As DataTable = biz.CreaDtAssociazioni(dt_vacEseguite)

                For i As Integer = 0 To dtAssociazioni.Rows.Count - 1

                    ' Effettuo il controllo solo sulle righe inserite da vaccinazioni programmate (non hanno ves_id)
                    If dtAssociazioni.Rows(i)("ves_id") Is System.DBNull.Value AndAlso
                       dtAssociazioni.Rows(i)("ves_ass_codice").ToString <> String.Empty Then

                        biz.CheckDatiAssociazione(i, dtAssociazioni, controlList)

                    End If

                Next

                For i As Integer = 0 To dt_vacEseguite.Rows.Count - 1

                    ' Effettuo il controllo solo sulle righe inserite da vaccinazioni programmate (non hanno ves_id)
                    If dt_vacEseguite.Rows(i)("ves_id") Is DBNull.Value Then

                        biz.CheckDatiVaccinazione(i, dt_vacEseguite, controlList)
                        biz.CheckDataEffettuazione(i, dt_vacEseguite, controlList, OnVacUtility.Variabili.PazId)
                        biz.CheckDatiInoculo(i, dt_vacEseguite, controlList)

                    End If

                Next

            End Using
        End Using

        If controlList.HasError Then
            strJS &= controlList.GetAlertJS()
            Return
        End If

        ' Preparazione strutture per controllo coperture nomi commerciali 
        Dim coperture As New List(Of BizVaccinazioneProg.CopertureNomiCommerciali)()
        Dim nomiCommercialiCopertureControllate As New List(Of CoperturaControllata)()

        For i As Integer = 0 To dg_vacProg.Items.Count - 1

            Dim row As DataRow = dt_vacProg.Rows.Find(GetRowKey(i))

            Dim codiceNomeCommerciale As String = String.Empty
            If Not row.IsNull("ves_noc_codice") Then
                codiceNomeCommerciale = row("ves_noc_codice").ToString()
            End If

            If Not String.IsNullOrWhiteSpace(codiceNomeCommerciale) Then

                Dim codiceVaccinazione As String = String.Empty
                If Not row.IsNull("vpr_vac_codice") Then
                    codiceVaccinazione = row("vpr_vac_codice").ToString()
                End If

                Dim isSelected As Boolean = DirectCast(dg_vacProg.Items.Item(i).FindControl("cb"), CheckBox).Checked

                coperture.Add(New BizVaccinazioneProg.CopertureNomiCommerciali(codiceNomeCommerciale, codiceVaccinazione, isSelected))

            End If
        Next

        Dim warningQuantitaLotti As New System.Text.StringBuilder()

        For i As Integer = 0 To dg_vacProg.Items.Count - 1

            If DirectCast(dg_vacProg.Items.Item(i).FindControl("cb"), CheckBox).Checked Then

                Dim row As DataRow = dt_vacProg.Rows.Find(GetRowKey(i))

                Dim existsEseguita As Boolean

                Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                    Using bizEseguite As New BizVaccinazioniEseguite(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                        existsEseguita = bizEseguite.ExistsVaccinazioneEseguita(OnVacUtility.Variabili.PazId, Date.Parse(row("ves_data_effettuazione").ToString()), row("vpr_vac_codice").ToString())

                    End Using
                End Using

                If Not existsEseguita Then

                    Dim dam As IDAM = OnVacUtility.OpenDam()

                    dam.QB.NewQuery()
                    dam.QB.AddSelectFields("COUNT(*)")
                    dam.QB.AddTables("t_paz_inadempienze")
                    dam.QB.AddWhereCondition("pin_paz_codice", Comparatori.Uguale, OnVacUtility.Variabili.PazId, DataTypes.Numero)
                    dam.QB.AddWhereCondition("pin_vac_codice", Comparatori.Uguale, row("vpr_vac_codice"), DataTypes.Stringa)

                    Try

                        Dim obj As Object = dam.ExecScalar()

                        If Not obj Is Nothing AndAlso Not obj Is DBNull.Value AndAlso Convert.ToInt32(obj) > 0 Then

                            stbDeleteInadempienze.AppendFormat("{0}, ", row("vpr_vac_codice").ToString())

                        End If

                    Finally
                        OnVacUtility.CloseDam(dam)
                    End Try

                    Dim codiceLotto As String = String.Empty
                    If Not row.IsNull("ves_lot_codice") Then
                        codiceLotto = row("ves_lot_codice").ToString()
                    End If

                    Dim codiceNomeCommerciale As String = String.Empty
                    If Not row.IsNull("ves_noc_codice") Then
                        codiceNomeCommerciale = row("ves_noc_codice").ToString()
                    End If

                    If Not codiceLotto = "" Then
                        '------
                        If ControllaCoperturaNomeCommerciale(codiceNomeCommerciale, coperture, nomiCommercialiCopertureControllate) Then
                            '------
                            Dim descrizioneSitoInoculazione As String = GetDdlSelectedTextFromGrid(i, "ddlSii")
                            Dim descrizioneViaSomministrazione As String = GetDdlSelectedTextFromGrid(i, "ddlVii")
                            Dim descrizioneNomeCommerciale As String = DirectCast(dg_vacProg.Items.Item(i).FindControl("lblDescNC"), Label).Text
                            '------
                            If ControllaSitoViaDiversiStessoNomeCommerciale(descrizioneSitoInoculazione, descrizioneViaSomministrazione, descrizioneNomeCommerciale) Then
                                '------
                                row("E") = StatoVaccinazione.ESEGUITA
                                row("SaveEseguita") = True
                                '------
                                row("ves_n_richiamo") = row("vpr_n_richiamo")
                                '------
                                Dim rowTemp As DataRow = dt_vacEs.NewRow()
                                rowTemp("ves_vac_codice") = row("vpr_vac_codice")
                                rowTemp("ves_n_richiamo") = row("vpr_n_richiamo")
                                rowTemp("ves_data_effettuazione") = row("ves_data_effettuazione")
                                rowTemp("ves_dataora_effettuazione") = row("ves_dataora_effettuazione")
                                rowTemp("ves_note") = row("ves_note")
                                '------
                                dt_vacEs.Rows.Add(rowTemp)
                                '------
                                eseguita = True
                                '------
                                If Not codLottiWarningQta.Contains(codiceLotto) Then
                                    '--
                                    codLottiWarningQta.Add(codiceLotto)
                                    '--
                                    Dim avvisoQuantita As String = AvvisaQtaLotto(codiceLotto)

                                    If Not String.IsNullOrEmpty(avvisoQuantita) Then
                                        warningQuantitaLotti.AppendFormat("{0}\n", avvisoQuantita)
                                    End If
                                    '--
                                End If
                                '------
                            Else
                                '------
                                arVacNoEseguiteSitoViaDiversi.Add(DirectCast(dg_vacProg.Items.Item(i).FindControl("lblDescVac"), Label).Text)
                                '------
                            End If
                            '------
                        Else
                            '------
                            arVacNoEseguiteAltreVac.Add(DirectCast(dg_vacProg.Items.Item(i).FindControl("lblDescVac"), Label).Text)
                            '------
                        End If
                        '------
                    Else
                        '------
                        If Settings.GESMAG Then
                            '------
                            arVacNoEseguiteNoLotto.Add(DirectCast(Me.dg_vacProg.Items.Item(i).FindControl("lblDescVac"), Label).Text)
                            '------
                        Else
                            '------
                            row("E") = StatoVaccinazione.ESEGUITA
                            row("SaveEseguita") = True
                            row("ves_n_richiamo") = row("vpr_n_richiamo")
                            '------
                            Dim rowTemp As DataRow = dt_vacEs.NewRow()
                            rowTemp("ves_vac_codice") = row("vpr_vac_codice")
                            rowTemp("ves_n_richiamo") = row("vpr_n_richiamo")
                            rowTemp("ves_data_effettuazione") = row("ves_data_effettuazione")
                            rowTemp("ves_dataora_effettuazione") = row("ves_dataora_effettuazione")
                            rowTemp("ves_note") = row("ves_note")
                            '------
                            dt_vacEs.Rows.Add(rowTemp)
                            '------
                            eseguita = True
                            '------
                        End If
                        '------
                    End If
                    '------
                Else
                    '------
                    strJS &= String.Format("alert('Impossibile eseguire il {0} nella data {1}! Vaccinazione gia\' presente');", row("vpr_vac_codice").ToString, Date.Parse(row("ves_data_effettuazione").ToString))
                    '------
                End If
            End If
        Next

        If warningQuantitaLotti.Length > 0 Then
            strJS &= String.Format("alert('ATTENZIONE:\n{0}');", warningQuantitaLotti.ToString())
        End If

        If eseguita Then
            BindDGVacProgrammate()
            dg_vacProg.SelectedIndex = -1
            OnitLayout31.Busy = True
            EnableToolbar(False)
        End If

        strJS &= "CheckAll('dg_vacProg',false,0,0)" & Environment.NewLine

        If arVacNoEseguiteNoLotto.Count > 0 Then
            Dim stb As New System.Text.StringBuilder()
            For i As Integer = 0 To arVacNoEseguiteNoLotto.Count - 1
                stb.AppendFormat("{0}\n", arVacNoEseguiteNoLotto(i).ToString())
            Next
            strJS &= String.Format("alert(""ATTENZIONE: le vaccinazioni qui elencate non sono state eseguite\nperchè non hanno nessun lotto associato:\n\n{0}""){1}", stb.ToString(), Environment.NewLine)
        End If

        If arVacNoEseguiteAltreVac.Count > 0 Then
            Dim stb As New System.Text.StringBuilder()
            stb.Append("alert(""ATTENZIONE: le vaccinazioni qui elencate non sono state eseguite\nperchè il lotto associato è utilizzato anche per altre vaccinazioni:\n\n")
            For i As Integer = 0 To arVacNoEseguiteAltreVac.Count - 1
                stb.AppendFormat("{0}\n", arVacNoEseguiteAltreVac(i).ToString())
            Next
            strJS &= String.Format("{0}\nPer eseguire una vaccinazione è necessario che siano eseguite\nanche le altre vaccinazioni con lo stesso lotto associato !""){1}", stb.ToString(), Environment.NewLine)
        End If

        If arVacNoEseguiteSitoViaDiversi.Count > 0 Then
            Dim stb As New System.Text.StringBuilder()
            stb.Append("alert(""ATTENZIONE: le vaccinazioni qui elencate non sono state eseguite\nperchè il sito di inoculazione e la via di somministrazione sono diversi per lo stesso nome commerciale:\n\n")
            For i As Integer = 0 To arVacNoEseguiteSitoViaDiversi.Count - 1
                stb.AppendFormat("{0}\n", arVacNoEseguiteSitoViaDiversi(i).ToString())
            Next
            strJS &= String.Format("{0}\nPer eseguire una vaccinazione è necessario che il sito di inoculazione e la via di somministrazione siano uguali per lo stesso nome commerciale !""){1}", stb.ToString, Environment.NewLine)
        End If

        If stbDeleteInadempienze.Length > 0 Then
            stbDeleteInadempienze.Remove(stbDeleteInadempienze.Length - 2, 2)
            strJS &= String.Format("alert('Le seguenti vaccinazioni hanno una inadempienza collegata:\n{0}\nSalvando i dati i record verranno eliminati!');", stbDeleteInadempienze.ToString())
        End If

    End Sub

    Private Function GetDataOraEffettuazione(dataEffettuazione As Date, oraEffettuazione As String) As Date

        Dim strDataOraEffettuazione As String = String.Format("{0:dd/MM/yyyy} {1}", dataEffettuazione, oraEffettuazione)

        Dim dataOraEffettuazione As Date

        Try
            dataOraEffettuazione = Date.Parse(strDataOraEffettuazione)
        Catch ex As Exception
            dataOraEffettuazione = Date.MinValue
        End Try

        Return dataOraEffettuazione

    End Function

    Private Function GetStringVaccinazioneDose(row As DataRow) As String

        Return String.Format("{0} [{1}]; ", row("vpr_vac_codice"), row("vpr_n_richiamo"))

    End Function

#End Region

#Region " Bilanci "

    Private Function bilanciEsclusi() As Boolean

        If Settings.GESBIL Then

            For i As Int16 = 0 To dt_bilProg.Rows.Count - 1
                If dt_bilProg.Rows(i).RowState <> DataRowState.Deleted AndAlso dt_bilProg.Rows(i)("escluso").ToString() = "X" Then
                    Return True
                End If
            Next

        End If

        Return False

    End Function

    Private Sub SalvaModificheBilanci(ByRef DAM As IDAM, dt_bilProg As DataTable)

        Using genericProvider As New DAL.DbGenericProvider(DAM)

            Dim codicePaziente As Integer = OnVacUtility.Variabili.PazId

            Dim bilancioProgrammatoModificato As Entities.BilancioProgrammato
            Dim bilancioProgrammato As Entities.BilancioProgrammato
            Dim bilancioProgrammatoCollection As OnVac.Collection.BilancioProgrammatoCollection

            Using bizBilancio As New Biz.BizBilancioProgrammato(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                For Each row As DataRow In dt_bilProg.Rows

                    Select Case row.RowState

                        Case DataRowState.Added

                            Dim dataConvocazione As Date = row("bip_cnv_data")
                            Dim codiceMalattia As String = row("bip_mal_codice").ToString()
                            Dim numeroBilancio As Short = row("bip_bil_numero").ToString()

                            Dim statoBilancio As String

                            If Not row("escluso") Is DBNull.Value AndAlso row("escluso").ToString() = "X" Then
                                statoBilancio = Constants.StatiBilancio.UNSOLVED
                            Else
                                statoBilancio = row("bip_stato").ToString()
                            End If

                            bilancioProgrammatoCollection = bizBilancio.CercaBilancio(codicePaziente, numeroBilancio, codiceMalattia)

                            If bilancioProgrammatoCollection Is Nothing OrElse bilancioProgrammatoCollection.Count = 0 Then

                                bilancioProgrammato = New Entities.BilancioProgrammato(codiceMalattia, numeroBilancio, dataConvocazione, statoBilancio, "false", codicePaziente)
                                bizBilancio.ProgrammaBilancio(bilancioProgrammato)

                            Else
                                ' [adesimone 080103] Attenzione se restituisce una collection con più di un bilancio:
                                ' questo al momento non è possibile in quanto per una malattia è possibile compilare un solo
                                ' bilancio in una singola data di convocazione
                                bilancioProgrammato = bilancioProgrammatoCollection(0)
                                bilancioProgrammato.Bil_stato = statoBilancio
                                bizBilancio.AggiornaBilancio(bilancioProgrammato)

                            End If

                        Case DataRowState.Deleted

                            Dim dataConvocazione As Date = row("bip_cnv_data", DataRowVersion.Original)
                            Dim codiceMalattia As String = row("bip_mal_codice", DataRowVersion.Original).ToString()
                            Dim numeroBilancio As Short = row("bip_bil_numero", DataRowVersion.Original).ToString()
                            Dim idBilancio As Integer = row("ID", DataRowVersion.Original)

                            bilancioProgrammatoModificato = New Entities.BilancioProgrammato(codiceMalattia, numeroBilancio, dataConvocazione, "", "false", codicePaziente)
                            bilancioProgrammatoModificato.Bil_id = idBilancio

                            bizBilancio.CancellaBilancio(bilancioProgrammatoModificato)

                        Case DataRowState.Modified

                            Dim dataConvocazione As Date = row("bip_cnv_data")
                            Dim codiceMalattia As String = row("bip_mal_codice").ToString()
                            Dim numeroBilancio As Short = row("bip_bil_numero").ToString()

                            Dim statoBilancio As String

                            If (Not row("escluso") Is DBNull.Value AndAlso row("escluso").ToString() = "X") Then
                                statoBilancio = Constants.StatiBilancio.UNSOLVED
                            Else
                                statoBilancio = row("bip_stato").ToString()
                            End If

                            Dim idBilancio As Integer = row("ID")

                            bilancioProgrammatoModificato = New Entities.BilancioProgrammato(codiceMalattia, numeroBilancio, dataConvocazione, statoBilancio, "false", codicePaziente)
                            bilancioProgrammatoModificato.Bil_id = idBilancio

                            bizBilancio.AggiornaBilancio(bilancioProgrammatoModificato)

                    End Select

                Next

            End Using

        End Using

    End Sub

#End Region

#Region " Salvataggio "

    Private Function Salva() As Boolean

        ' Valore restituito dalla funzione
        Dim success As Boolean = True

        ' Messaggi da visualizzare prima dell'eventuale redirect
        Dim failedMessage As New Text.StringBuilder()
        Dim warningMessage As New Text.StringBuilder()

        ' Log
        Dim testataLog As Testata
        Dim listTestateLog As New List(Of Testata)()

        Dim programmazioneEliminata As Boolean = False

#Region " Modalità di accesso "

        Dim modalitaAccesso As String = String.Empty

        If Settings.GESMODALITAACCESSO Then
            If rb_cup.Checked Then
                modalitaAccesso = Constants.ModalitaAccesso.PrenotatiCup
            ElseIf rb_ps.Checked Then
                modalitaAccesso = Constants.ModalitaAccesso.ProntoSoccorso
            ElseIf rb_vol.Checked Then
                modalitaAccesso = Constants.ModalitaAccesso.AccessoVolontario
            Else
                modalitaAccesso = Constants.ModalitaAccesso.AppuntamentoOnVac
            End If
        End If

#End Region

        Dim almostOneE As Boolean = False

        ' Valorizzazione flag di visibilità per i dati vaccinali del paziente
        Dim valoreVisibilitaDatiVaccinaliPaziente As String = String.Empty

        If chkFlagVisibilita.Checked Then
            valoreVisibilitaDatiVaccinaliPaziente = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente
        Else
            valoreVisibilitaDatiVaccinaliPaziente = Constants.ValoriVisibilitaDatiVaccinali.NegatoDaPaziente
        End If

        Dim vaccinazioneEsclusaMotivoCentraleList As New List(Of Entities.VaccinazioneEsclusa)()
        Dim vaccinazioneEsclusaMotivoCentraleEliminataList As New List(Of Entities.VaccinazioneEsclusa)()

        Dim codiceMotiviEsclusioniCentrale As IEnumerable(Of String)

        ' Lista dei lock che vengono creati (uno per ogni lotto eseguito), che verranno chiusi al termine del salvataggio.
        Dim listLockLotti As New List(Of Onit.Shared.Manager.Lock.Lock)()

        ' Elenco vaccinazioni eseguite
        Dim dtVaccinazioniEseguite As DataTable = Nothing

        Try
            ' Valorizzazione lista dei codici dei lotti eseguiti
            Dim codiciLottiEseguiti As List(Of String) = Me.GetCodiciLottiEseguitiFromProgrammate()

            ' Inserimento di un movimento di carico per ripristinare ogni lotto eliminato + log
            For Each codiceLotto As String In codiciLottiEseguiti
                ' Lock di ogni lotto utilizzato presente in codiciLottiEseguiti
                listLockLotti.Add(BizLotti.EnterLockLotto(codiceLotto, OnVacContext.AppId, OnVacUtility.Variabili.CNSMagazzino.Codice))
            Next

            Using transactionScope As New System.Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, OnVacUtility.GetReadCommittedTransactionOptions())
                '--
                Using dbGenericProviderFactory As New DbGenericProviderFactory()
                    '--
                    Dim genericProvider As DbGenericProvider = dbGenericProviderFactory.GetDbGenericProvider(OnVacContext.AppId, OnVacContext.Azienda)

#Region " Gestione Lotti (controllo presenza lotti in magazzino) "
                    '--
                    ' Controllo presenza lotti eseguiti in magazzino (se tra l'esegui e il salva qualcun altro li ha finiti, l'utente non deve poter salvare l'esecuzione)
                    '--
                    If Settings.GESMAG AndAlso Not codiciLottiEseguiti Is Nothing AndAlso codiciLottiEseguiti.Count > 0 Then

                        Using bizLotti As New BizLotti(genericProvider, Settings, New BizContextInfos(OnVacContext.UserId, OnVacContext.Azienda, OnVacContext.AppId, OnVacUtility.Variabili.CNS.Codice, OnVacContext.CodiceUslCorrente, Nothing))
                            '--
                            Dim bizLottiResult As BizLotti.BizLottiResult =
                                bizLotti.CheckPresenzaDosi(codiciLottiEseguiti, OnVacUtility.Variabili.CNSMagazzino.Codice)
                            '--
                            If bizLottiResult.Result = BizLotti.BizLottiResult.ResultType.GenericError Then
                                failedMessage.Append(bizLottiResult.Message)
                                success = False
                            End If
                            '--
                        End Using

                    End If
#End Region

                    If success Then
                        '--
                        codiceMotiviEsclusioniCentrale = genericProvider.MotiviEsclusione.GetCodiciMotiviCentralizzati()
                        '--
                        ' Cerca se la CNV è una CNV da campagna
                        Dim cnvInCampagna As Boolean = genericProvider.Convocazione.IsCnvCampagna(OnVacUtility.Variabili.PazId, DataConvocazione)
                        '--
                        'salvataggio delle modifiche ai bilanci programmati
                        If Settings.GESBIL Then
                            SalvaModificheBilanci(genericProvider.DAM, dt_bilProg)
                        End If

                        Dim dateEffettuazioneLotti As New Hashtable()

                        Dim totaleVaccinazioni As Integer = dt_vacProg.Rows.Count
                        Dim countEseguite As Integer = 0
                        Dim countEscluse As Integer = 0

                        ' Caricamento vaccinazioni eseguite del paziente
                        Using bizVaccinazioniEseguite As New BizVaccinazioniEseguite(dbGenericProviderFactory, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.VAC_PROGRAMMATE))

                            dtVaccinazioniEseguite = bizVaccinazioniEseguite.GetDtVaccinazioniEseguite(OnVacUtility.Variabili.PazId)

#Region " Righe eliminate "
                            '--
                            'devo scorrere una prima volta le righe del datatable per eliminare le vaccinazioni programmate
                            'che potrebbero essere inserite nuovamente con la funzionalità Aggiungi Vac / Aggiungi Ass
                            'abilitate anche quando si eliminano le righe
                            '--
                            For Each row As DataRow In dt_vacProg.Rows
                                '--
                                Select Case row.RowState
                                '--
                                    Case DataRowState.Deleted
                                        '--
                                        ' Eliminazione dalla t_vac_programmate
                                        '--                             
                                        EliminaVaccinazioneProgrammata(genericProvider, row, listTestateLog)
                                        '--
                                End Select
                                '--
                            Next
#End Region

#Region " Righe inserite e modificate "

                            Dim now As DateTime = DateTime.Now

                            For Each row As DataRow In dt_vacProg.Rows
                                '---
                                Select Case row.RowState
                                '---
                                    Case DataRowState.Added, DataRowState.Modified
                                        '--
                                        ' Controllo la colonna "E": indica se la vaccinazione è stata eseguita o esclusa. 
                                        ' Se il valore è null, potrebbe essere stata inserita o modificata.
                                        If Not row("E") Is DBNull.Value Then
                                            '-- 
                                            Select Case row("E").ToString().ToLower()
                                            '--
                                                Case StatoVaccinazione.ESEGUITA
                                                    '--
                                                    countEseguite += 1
                                                    '--
                                                    If row("SaveEseguita") Then
                                                        '--
                                                        If row("ves_id") Is DBNull.Value Then
                                                            '--
                                                            OnVacUtility.ControllaVaccinazioneSostituta(row("vpr_vac_codice"))
                                                            '--
                                                            ' Inserimento nel datatable delle vaccinazioni eseguite
                                                            InsertRowIntoDtEseguite(row, dtVaccinazioniEseguite, modalitaAccesso, cnvInCampagna, valoreVisibilitaDatiVaccinaliPaziente, genericProvider.DAM)
                                                            '--
                                                            'Cancellazione dell'inadempienza
                                                            genericProvider.Paziente.DeleteInadempienza(OnVacUtility.Variabili.PazId, row("vpr_vac_codice").ToString())
                                                            '--
                                                            ' Registrazione della data di esecuzione del lotto
                                                            If Not row("ves_lot_codice") Is System.DBNull.Value Then
                                                                '--
                                                                If Not dateEffettuazioneLotti.ContainsKey(row("VES_LOT_CODICE")) Then
                                                                    '--
                                                                    dateEffettuazioneLotti.Add(row("VES_LOT_CODICE"), row("VES_DATA_EFFETTUAZIONE"))
                                                                    '--
                                                                End If
                                                                '--
                                                            End If
                                                            '--
                                                            If row.RowState = DataRowState.Modified Then
                                                                '--
                                                                ' Cancellazione nella t_vac_programmate
                                                                EliminaVaccinazioneProgrammata(genericProvider, row, listTestateLog)
                                                                '--
                                                            End If
                                                            '--
                                                            almostOneE = True
                                                            '--
                                                        End If
                                                        '--
                                                    End If
                                                '--
                                                Case StatoVaccinazione.ESCLUSA
                                                    '--
                                                    countEscluse += 1
                                                    '--
                                                    If row("SaveEsclusa") Then
                                                        '--
                                                        AggiornaVaccinazioneEsclusa(row, listTestateLog, genericProvider.DAM, vaccinazioneEsclusaMotivoCentraleList, vaccinazioneEsclusaMotivoCentraleEliminataList, codiceMotiviEsclusioniCentrale, valoreVisibilitaDatiVaccinaliPaziente)
                                                        '--
                                                        almostOneE = True
                                                        '--
                                                    End If
                                                    '--                                                     
                                            End Select
                                            '--
                                        Else
                                            '---
                                            Select Case row.RowState
                                            '--
                                                Case DataRowState.Added
                                                    '---
                                                    ' Inserimento nella t_vac_programmate
                                                    '--
                                                    Dim vacProg As Entities.VaccinazioneProgrammata = CreateVaccinazioneProgrammataFromDataRow(row)
                                                    vacProg.DataInserimento = now
                                                    vacProg.IdUtenteInserimento = OnVacContext.UserId

                                                    genericProvider.VaccinazioneProg.InsertVaccinazioneProgrammata(vacProg)

                                                    testataLog = New Testata(DataLogStructure.TipiArgomento.VAC_PROGRAMMATE, Operazione.Inserimento)
                                                    testataLog.Records.Add(NewRecordInserimentoProgrammata(row, DataConvocazione))

                                                    listTestateLog.Add(testataLog)

                                                Case DataRowState.Modified
                                                    '--
                                                    ' Modifica della t_vac_programmate
                                                    '---
                                                    genericProvider.VaccinazioneProg.UpdateVaccinazioneProgrammata(Me.CreateVaccinazioneProgrammataFromDataRow(row))

                                                    testataLog = New Testata(DataLogStructure.TipiArgomento.VAC_PROGRAMMATE, Operazione.Modifica)
                                                    testataLog.Records.Add(NewRecordUpdateProgrammata(row))

                                                    If testataLog.ChangedValues Then listTestateLog.Add(testataLog)

                                            End Select
                                            '---                      
                                        End If
                                        '---
                                End Select
                                '---
                            Next
#End Region

                            bizVaccinazioniEseguite.Salva(OnVacUtility.Variabili.PazId, dtVaccinazioniEseguite)

                        End Using

#Region " Gestione Lotti (scaricamento dosi) "

                        ' Scaricamento dosi per ogni lotto utilizzato 
                        Dim codiciLottiDisattivatiScortaNulla As New System.Text.StringBuilder()
                        '---
                        If Settings.GESMAG AndAlso Not codiciLottiEseguiti.IsNullOrEmpty() Then
                            '--
                            Dim dataRegistrazione As DateTime = Date.Now
                            '--
                            Using bizLotti As New BizLotti(dbGenericProviderFactory, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.VAC_PROGRAMMATE, True))
                                '--
                                For Each codiceLottoEseguito As String In codiciLottiEseguiti
                                    '--
                                    ' Movimento da inserire
                                    '--
                                    Dim movimentoLotto As Entities.MovimentoLotto = GetMovimentoDaInserire(codiceLottoEseguito, dtVaccinazioniEseguite, dateEffettuazioneLotti, dataRegistrazione)
                                    '--
                                    ' Inserimento del movimento su db + log
                                    '--
                                    Dim insertMovimentoResult As BizLotti.BizLottiResult =
                                            bizLotti.ScaricaLottoVaccinazione(movimentoLotto, OnVacUtility.Variabili.CNS.Codice,
                                                                              OnVacUtility.Variabili.CNSMagazzino.Codice,
                                                                              False, listTestateLog)

                                    If insertMovimentoResult.Result = Biz.BizLotti.BizLottiResult.ResultType.GenericError Then

                                        failedMessage.AppendFormat("{0} ", insertMovimentoResult.Message)
                                        success = False

                                    ElseIf insertMovimentoResult.Result = Biz.BizLotti.BizLottiResult.ResultType.LottoDisattivatoScortaNullaWarning Then

                                        codiciLottiDisattivatiScortaNulla.AppendFormat("{0}\n", movimentoLotto.CodiceLotto)

                                    End If

                                Next

                            End Using

                        End If
#End Region

                        If success Then
                            '---
                            ' Messaggio nel caso in cui siano stati disattivati dei lotti per aver raggiunto un numero di dosi pari a 0
                            '---
                            If codiciLottiDisattivatiScortaNulla.Length > 0 Then
                                '--- 
                                warningMessage.AppendFormat("I seguenti lotti sono stati disattivati:\n{0}perchè il numero di dosi è esaurito.", codiciLottiDisattivatiScortaNulla.ToString())
                                '--- 
                            End If

#Region " Vaccinazioni programmate rimaste "

                            dt_vacEs.AcceptChanges()
                            dt_vacProg.AcceptChanges()
                            dt_bilProg.AcceptChanges()

                            If (dt_vacProg.Rows.Count = 0 OrElse Not BizVaccinazioneProg.EsistonoVaccinazioniDaEseguire(dt_vacProg, DataConvocazione)) AndAlso
                               (dt_bilProg.Rows.Count = 0 OrElse CheckBilTuttiE()) Then
                                '--- 
                                'Sono state eseguite o escluse o cancellate tutte le vaccinazioni programmate
                                'Elimina la t_cnv_convocazioni: le altre dovrebbero venire eliminate in cascata
                                '--
                                Using bizVaccinazioneProg As New BizVaccinazioneProg(dbGenericProviderFactory, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.VAC_PROGRAMMATE, True))

                                    Dim command As New BizVaccinazioneProg.EliminaProgrammazioneCommand()
                                    command.CodicePaziente = OnVacUtility.Variabili.PazId
                                    command.DataConvocazione = DataConvocazione
                                    command.EliminaAppuntamenti = True
                                    command.EliminaSollecitiBilancio = True
                                    command.EliminaBilanci = False
                                    command.TipoArgomentoLog = DataLogStructure.TipiArgomento.VAC_PROGRAMMATE
                                    command.OperazioneAutomatica = True

                                    If countEseguite > 0 Then
                                        command.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.Esecuzione
                                    ElseIf countEscluse > 0 Then
                                        command.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.Esclusione
                                    Else
                                        command.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.EliminazioneConvocazione
                                    End If

                                    Dim note As New System.Text.StringBuilder()

                                    If countEseguite > 0 Then note.Append("esecuzione-")
                                    If countEscluse > 0 Then note.Append("esclusione-")
                                    If countEseguite + countEscluse <totaleVaccinazioni Then note.Append("eliminazione-")

                                    note.Remove(note.Length - 1, 1)

                                    command.NoteEliminazione = String.Format("Eliminazione convocazione da Vaccinazioni Programmate per {0} vaccinazioni", note.ToString())

                                    bizVaccinazioneProg.EliminaProgrammazione(command)

                                End Using
                                '--
                                programmazioneEliminata = True
                                '--
                            Else
                                '--- 
                                'Sono rimaste ancora vaccinazioni programmate. 
                                '---
                                If Not String.IsNullOrEmpty(Me.tb_cnv_app.Text) Then
                                    warningMessage.Append("ATTENZIONE: la modifica di una convocazione alla quale è stata già assegnata la data di appuntamento può creare incoerenze nella durata della seduta\n\n")
                                End If
                                '---
                            End If
#End Region
                            '--
                            ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
                            If FlagAbilitazioneVaccUslCorrente Then
                                '--                            '--                              
                                Using bizPaziente As New BizPaziente(dbGenericProviderFactory, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.VAC_PROGRAMMATE, True))
                                    '--
                                    Dim aggiornaDatiVaccinaliCentraliCommand As New BizPaziente.AggiornaDatiVaccinaliCentraliCommand()
                                    aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale = bizPaziente.GenericProvider.Paziente.GetCodiceAusiliario(OnVacUtility.Variabili.PazId)
                                    aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEsclusaEnumerable = vaccinazioneEsclusaMotivoCentraleList.AsEnumerable()
                                    aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEsclusaEliminataEnumerable = vaccinazioneEsclusaMotivoCentraleEliminataList.AsEnumerable()
                                    '--
                                    bizPaziente.AggiornaDatiVaccinaliCentrali(aggiornaDatiVaccinaliCentraliCommand)
                                    '--
                                End Using
                                '--
                            End If

#Region " FSE "
                            If Settings.FSE_GESTIONE Then

                                Dim indicizzazioneResult As BizGenericResult = OnVacUtility.FSEHelper.IndicizzaSuRegistry(
                                    Convert.ToInt64(OnVacUtility.Variabili.PazId),
                                    Constants.TipoDocumentoFSE.CertificatoVaccinale,
                                    Constants.FunzionalitaNotificaFSE.VacProg_Salva,
                                    Constants.EventoNotificaFSE.InserimentoVaccinazione,
                                    Settings,
                                    OnVacUtility.Variabili.MedicoResponsabile.Codice)

                                success = indicizzazioneResult.Success

                                If Not indicizzazioneResult.Success AndAlso Not String.IsNullOrWhiteSpace(indicizzazioneResult.Message) Then
                                    warningMessage.Append("Indicizzazione sul Registry Regionale non avvenuta!")
                                End If

                            End If
#End Region

                            Dim pazienteCorrente As Entities.Paziente = Nothing

                            Using bizPaziente As New BizPaziente(dbGenericProviderFactory, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                                '--
                                ' N.B. : questa maschera è utilizzabile solo in modalità locale e non in centrale
                                pazienteCorrente = bizPaziente.CercaPaziente(OnVacUtility.Variabili.PazId)
                                '--
                            End Using
                            '--
                            OnVacMidSendManager.ModificaPaziente(pazienteCorrente, dtVaccinazioniEseguite)
                            '--
                        End If
                        '--
                    End If
                    '--
                End Using
                '--
                ' Scrittura Log
                For Each testata As Testata In listTestateLog
                    LogBox.WriteData(testata)
                Next
                '--   
                transactionScope.Complete()
                '--
            End Using
            '--
        Finally

            BizLotti.ExitLockLotti(listLockLotti)

        End Try

        Dim doRedirect As Boolean = False

        If success Then

            If programmazioneEliminata Then

                If warningMessage.Length > 0 Then
                    ' Se ci sono dei messaggi, li visualizzo e nell'evento click dell'onitLayout 
                    ' eseguo il redirect (altrimenti li perderei).
                    doRedirect = True
                Else
                    ' Se non ci sono messaggi da visualizzare, eseguo subito il redirect.
                    RedirectToConvocazioniPaziente(OnVacUtility.Variabili.PazId, EditConvocazioni, Not almostOneE, Nothing, Nothing)
                End If

            Else

                If Settings.GESBIL Then
                    RecuperaBilProg()
                    BindDGBilProgrammati()
                End If

                OnitLayout31.Busy = False
                EnableToolbar(False)

            End If

            ' Messaggi all'utente
            If warningMessage.Length > 0 Then
                OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(warningMessage.ToString(), "warningMessage|" + almostOneE.ToString(), False, doRedirect))
            End If

        Else

            ' Messaggio in caso di errore
            OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(failedMessage.ToString(), "saveFailedMessage", False, False))

        End If

        Return success And Not doRedirect

    End Function

    Private Sub EliminaVaccinazioneProgrammata(genericProvider As DbGenericProvider, row As DataRow, listTestateLog As List(Of Testata))
        '--
        Dim codiceVaccinazione As String
        '--
        If row.RowState = DataRowState.Deleted Then
            codiceVaccinazione = row("VPR_VAC_CODICE", DataRowVersion.Original).ToString()
        Else
            codiceVaccinazione = row("VPR_VAC_CODICE").ToString()
        End If
        '--
        Using bizVaccinazioneProg As New Biz.BizVaccinazioneProg(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
            '--
            Dim eliminaVaccinazioniProgrammateCommand As New BizVaccinazioneProg.EliminaVaccinazioniProgrammateCommand()
            eliminaVaccinazioniProgrammateCommand.CodicePaziente = OnVacUtility.Variabili.PazId
            eliminaVaccinazioniProgrammateCommand.CodiceVaccinazioni = {codiceVaccinazione}.AsEnumerable()
            eliminaVaccinazioniProgrammateCommand.DataConvocazione = DataConvocazione
            '--
            bizVaccinazioneProg.EliminaVaccinazioniProgrammate(eliminaVaccinazioniProgrammateCommand)
            '--
        End Using
        '--
        Dim testataLog As New Testata(DataLogStructure.TipiArgomento.VAC_PROGRAMMATE, Operazione.Eliminazione)
        testataLog.Records.Add(Me.NewRecordEliminazioneProgrammata(row))
        '--
        If testataLog.ChangedValues Then listTestateLog.Add(testataLog)
        '--
    End Sub

    Private Sub AggiornaVaccinazioneEsclusa(row As DataRow, listTestateLog As List(Of Testata), dam As IDAM, vaccinazioneEsclusaMotivoCentraleList As List(Of Entities.VaccinazioneEsclusa), vaccinazioneEsclusaEliminataMotivoCentraleList As List(Of Entities.VaccinazioneEsclusa), codiciMotiviCentralizzati As IEnumerable(Of String), valoreVisibilitaDatiVaccinaliPaziente As String)
        '---
        Dim testataLog As Testata = Nothing
        '---
        'recupera, se esistente, il codice della vaccinazione sostituta:
        'viene infatti eseguita/esclusa questa [modifica 04/08/2005]
        '--                 
        OnVacUtility.ControllaVaccinazioneSostituta(row("vpr_vac_codice"))
        '--
        Using genericProvider As New DbGenericProvider(dam)
            Using bizVaccinazioneEsclusa As New BizVaccinazioniEscluse(genericProvider, Settings, OnVacContext.CreateBizContextInfos, OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.VAC_PROGRAMMATE))
                '--
                If row("vex_id") Is DBNull.Value Then
                    '---
                    ' Inserimento nella t_vac_escluse
                    '---         
                    Dim vaccinazioneEsclusa As Entities.VaccinazioneEsclusa = Me.CreateVaccinazioneEsclusaFromDataRow(row, valoreVisibilitaDatiVaccinaliPaziente)
                    '---
                    If codiciMotiviCentralizzati.Contains(vaccinazioneEsclusa.CodiceMotivoEsclusione) Then
                        '---
                        vaccinazioneEsclusaMotivoCentraleList.Add(vaccinazioneEsclusa)
                        '---
                    End If
                    '--
                    Dim salvaVaccinazioneEsclusaCommand As New BizVaccinazioniEscluse.SalvaVaccinazioneEsclusaCommand()
                    salvaVaccinazioneEsclusaCommand.VaccinazioneEsclusa = vaccinazioneEsclusa
                    salvaVaccinazioneEsclusaCommand.Operation = Biz.BizClass.SalvaCommandOperation.Insert
                    '--
                    bizVaccinazioneEsclusa.SalvaVaccinazioneEsclusa(salvaVaccinazioneEsclusaCommand)

                    ' TODO: LOG => spostare nel bizvacescluse => TipiArgomento.VAC_ESCLUSE ???
                    testataLog = New Testata(DataLogStructure.TipiArgomento.VAC_ESCLUSE, Operazione.Inserimento)
                    testataLog.Records.Add(Me.NewRecordInserimentoEsclusa(row))
                    '---
                    listTestateLog.Add(testataLog)
                    '---
                    OnVacUtility.CreaInadempienza(row("vex_moe_codice").ToString(), row("vpr_vac_codice").ToString(), OnVacUtility.Variabili.PazId, row("vex_data_visita"), OnVacContext.UserId.ToString(), dam, Nothing)
                    '--                                  
                Else
                    '---
                    ' Modifica nella t_vac_escluse
                    '---            
                    Dim vaccinazioneEsclusa As Entities.VaccinazioneEsclusa = Me.CreateVaccinazioneEsclusaFromDataRow(row, valoreVisibilitaDatiVaccinaliPaziente)
                    '--
                    If codiciMotiviCentralizzati.Contains(vaccinazioneEsclusa.CodiceMotivoEsclusione) Then
                        '--
                        vaccinazioneEsclusaMotivoCentraleList.Add(vaccinazioneEsclusa)
                        '--
                    ElseIf codiciMotiviCentralizzati.Contains(row("vex_moe_codice", DataRowVersion.Original).ToString()) Then
                        '--
                        vaccinazioneEsclusaEliminataMotivoCentraleList.Add(vaccinazioneEsclusa)
                        '--
                    End If
                    '--
                    Dim salvaVaccinazioneEsclusaCommand As New BizVaccinazioniEscluse.SalvaVaccinazioneEsclusaCommand()
                    salvaVaccinazioneEsclusaCommand.VaccinazioneEsclusa = vaccinazioneEsclusa
                    salvaVaccinazioneEsclusaCommand.VaccinazioneEsclusaOriginale =
                        bizVaccinazioneEsclusa.GenericProvider.VaccinazioniEscluse.GetVaccinazioneEsclusaById(vaccinazioneEsclusa.Id)
                    salvaVaccinazioneEsclusaCommand.Operation = Biz.BizClass.SalvaCommandOperation.Update
                    salvaVaccinazioneEsclusaCommand.OverwriteIfUpdateOperation = False
                    '--
                    Dim salvaEsclusaResult As BizVaccinazioniEscluse.SalvaVaccinazioneEsclusaResult =
                        bizVaccinazioneEsclusa.SalvaVaccinazioneEsclusa(salvaVaccinazioneEsclusaCommand)

                    ' Aggiunta dell'esclusa a quelle da eliminare in centrale
                    If Not salvaEsclusaResult.VaccinazioneEsclusaEliminata Is Nothing Then

                        If codiciMotiviCentralizzati.Contains(salvaEsclusaResult.VaccinazioneEsclusaEliminata.CodiceMotivoEsclusione) Then
                            vaccinazioneEsclusaEliminataMotivoCentraleList.Add(salvaEsclusaResult.VaccinazioneEsclusaEliminata)
                        End If

                    End If
                    '--
                    ' TODO: LOG => spostare nel bizvacescluse => TipiArgomento.VAC_ESCLUSE ???
                    testataLog = New Testata(DataLogStructure.TipiArgomento.VAC_ESCLUSE, Operazione.Modifica)
                    testataLog.Records.Add(Me.NewRecordUpdateEsclusa(row))
                    '--- 
                    If testataLog.ChangedValues Then listTestateLog.Add(testataLog)
                    '--- 
                    'Se è cambiato il motivo di esclusione cancello la vecchia inadempienza e ne creo (se necessario) una nuova
                    '--- 
                    If IsDBNull(row("vex_moe_codice", DataRowVersion.Original)) OrElse row("vex_moe_codice", DataRowVersion.Original) <> row("vex_moe_codice") Then
                        '--
                        OnVacUtility.EliminaInadempienza(row("vpr_vac_codice", DataRowVersion.Original), OnVacUtility.Variabili.PazId, dam, Nothing)
                        OnVacUtility.CreaInadempienza(row("vex_moe_codice"), row("vpr_vac_codice"), OnVacUtility.Variabili.PazId, row("vex_data_visita"), OnVacContext.UserId.ToString(), dam, Nothing)
                        '--
                    End If
                    '--
                End If
                '---
            End Using
        End Using
        '---        
        OnVacUtility.ControllaSeTotalmenteInadempiente(OnVacUtility.Variabili.PazId, dam, Settings)
        '--
    End Sub

    Private Function GetCodiciLottiEseguitiFromProgrammate() As List(Of String)

        Dim codiciLottiEseguiti As New List(Of String)()

        Dim dt As DataTable = dt_vacProg.GetChanges(DataRowState.Added Or DataRowState.Modified)

        If Not dt Is Nothing Then

            For Each row As DataRow In dt.Rows

                If row("E").ToString() = StatoVaccinazione.ESEGUITA AndAlso
                   Not row("ves_lot_codice") Is System.DBNull.Value Then

                    ' Aggiunta del lotto alla lista di quelli eseguiti
                    If Not codiciLottiEseguiti.Contains(row("ves_lot_codice")) Then

                        codiciLottiEseguiti.Add(row("ves_lot_codice"))

                    End If
                End If

            Next

        End If

        Return codiciLottiEseguiti

    End Function

    ' Inserimento nel datatable delle vaccinazioni eseguite
    Private Sub InsertRowIntoDtEseguite(row As DataRow, ByRef dtVaccinazioniEseguite As DataTable, modalitaAccesso As String, cnvInCampagna As Boolean, valoreVisibilitaDatiVaccinaliPaziente As String, dam As IDAM)
        '--
        Dim newRow As DataRow = dtVaccinazioniEseguite.NewRow()
        '--
        newRow("PAZ_CODICE") = OnVacUtility.Variabili.PazId
        newRow("scaduta") = "N"
        newRow("VES_CIC_CODICE") = row("VPR_CIC_CODICE")
        newRow("VES_N_SEDUTA") = row("VPR_N_SEDUTA")
        newRow("VES_VAC_CODICE") = row("VPR_VAC_CODICE")
        newRow("VES_N_RICHIAMO") = row("VPR_N_RICHIAMO")
        newRow("VES_DATA_EFFETTUAZIONE") = OnVacUtility.Variabili.DataEsecuzione.Date
        newRow("VES_DATAORA_EFFETTUAZIONE") = OnVacUtility.Variabili.DataEsecuzione
        newRow("VES_NOTE") = row("ves_note")
        newRow("VES_LOT_CODICE") = row("VES_LOT_CODICE")
        newRow("VES_SII_CODICE") = row("VES_SII_CODICE")
        newRow("VES_VII_CODICE") = row("VES_VII_CODICE")
        newRow("VES_OPE_CODICE") = OnVacUtility.Variabili.MedicoResponsabile.Codice
        newRow("VES_MED_VACCINANTE") = row("VES_MED_VACCINANTE")
        newRow("VES_NOC_CODICE") = row("VES_NOC_CODICE")
        newRow("VES_LOT_DATA_SCADENZA") = row("VES_LOT_DATA_SCADENZA")
        '--
        ' per i nomi commerciali che avevano più di un'associazione è stata mostrata la mascherina 
        ' per scegliere tale associazione, per gli altri viene associato automaticamente
        Dim codiceAssociazione As String = Me.GetCodiceAssociazione(row, dam)
        Dim codiceAntigene As String = GetCodiceAntigene(codiceAssociazione, row("VPR_VAC_CODICE"))
        newRow("VES_ANTIGENE") = codiceAntigene
        '--
        If Not String.IsNullOrEmpty(codiceAssociazione) Then
            newRow("VES_ASS_CODICE") = codiceAssociazione
            newRow("VES_ASS_N_DOSE") = row("VES_ASS_N_DOSE")
        End If
        '--
        newRow("VES_CNS_CODICE") = OnVacUtility.Variabili.CNS.Codice
        newRow("VES_LUOGO") = Constants.CodiceLuogoVaccinazione.CentroVaccinale
        '--
        If OnVacUtility.Variabili.CNS.Ambulatorio.Codice > 0 Then
            newRow("VES_AMB_CODICE") = OnVacUtility.Variabili.CNS.Ambulatorio.Codice
        End If
        '--
        newRow("VES_OPE_IN_AMBULATORIO") = IIf(Me.chkInAmbulatorio.Checked, "S", "N")
        newRow("VES_ACCESSO") = IIf(Not modalitaAccesso Is Nothing, modalitaAccesso, "NULL")
        newRow("VES_CNV_DATA") = DataConvocazione

        If cnvInCampagna Then
            newRow("VES_IN_CAMPAGNA") = "S"
        End If

        If IsDate(txtPrimoAppuntamento.Text) Then
            newRow("VES_CNV_DATA_PRIMO_APP") = Convert.ToDateTime(txtPrimoAppuntamento.Text)
        End If

        newRow("VES_IMPORTO") = row("VES_IMPORTO")
        newRow("VES_MAL_CODICE_MALATTIA") = row("VES_MAL_CODICE_MALATTIA")
        newRow("VES_CODICE_ESENZIONE") = row("VES_CODICE_ESENZIONE")

        newRow("VES_FLAG_VISIBILITA") = valoreVisibilitaDatiVaccinaliPaziente

        newRow("VES_MAL_CODICE_COND_SANITARIA") = row("VES_MAL_CODICE_COND_SANITARIA")
        newRow("VES_RSC_CODICE") = row("VES_RSC_CODICE")

        newRow("VES_TPA_GUID_TIPI_PAGAMENTO") = row("NOC_TPA_GUID_TIPI_PAGAMENTO")

        Dim tipoErogatore As String = GetTipoErogatoreConsultorio(OnVacUtility.Variabili.CNS.Codice)
        newRow("VES_TIPO_EROGATORE") = tipoErogatore

        Dim struttura As Entities.Struttura = GetStruttura(OnVacUtility.Variabili.CNS.Codice, tipoErogatore)
        newRow("VES_COMUNE_O_STATO") = struttura.CodiceComune
        newRow("VES_CODICE_STRUTTURA") = struttura.CodiceStruttura
        newRow("VES_USL_COD_SOMMINISTRAZIONE") = struttura.CodiceAsl

        dtVaccinazioniEseguite.Rows.Add(newRow)

    End Sub

    ''' <summary>
    ''' Restituisce il movimento da inserire in seguito all'esecuzione di un lotto
    ''' </summary>
    ''' <param name="codiceLottoEseguito"></param>
    ''' <param name="dtVaccinazioniEseguite"></param>
    ''' <param name="dateEffettuazioneLotti"></param>
    ''' <param name="dataRegistrazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetMovimentoDaInserire(codiceLottoEseguito As String, dtVaccinazioniEseguite As DataTable, dateEffettuazioneLotti As Hashtable, dataRegistrazione As DateTime) As Entities.MovimentoLotto

        Dim idEsecuzioneAssociazione As String = String.Empty
        Dim dataEffettuazioneLotto As DateTime = DateTime.MinValue
        Dim vaccinazioniEffettuate As New System.Text.StringBuilder()

        If dateEffettuazioneLotti.ContainsKey(codiceLottoEseguito) Then

            dataEffettuazioneLotto = dateEffettuazioneLotti(codiceLottoEseguito)

            If dataEffettuazioneLotto > Date.MinValue Then

                Dim _codiceLottoEseguito = codiceLottoEseguito.Replace("'", "''")
                Dim rowsVaccinazioniEseguite As DataRow() = dtVaccinazioniEseguite.Select(
                    String.Format("paz_codice = {0} and ves_data_effettuazione >= '{1}' and ves_data_effettuazione < '{2}' and ves_lot_codice = '{3}' ",
                                  OnVacUtility.Variabili.PazId.ToString(),
                                  dataEffettuazioneLotto.ToString("dd/MM/yyyy"),
                                  dataEffettuazioneLotto.AddDays(1).ToString("dd/MM/yyyy"),
                                  _codiceLottoEseguito))

                If Not rowsVaccinazioniEseguite Is Nothing AndAlso rowsVaccinazioniEseguite.Count > 0 Then

                    idEsecuzioneAssociazione = rowsVaccinazioniEseguite.Min(Function(row) row("ves_ass_prog"))

                    For Each rowVaccinazione As DataRow In rowsVaccinazioniEseguite
                        vaccinazioniEffettuate.AppendFormat("{0}({1});",
                                                            rowVaccinazione("ves_vac_codice"),
                                                            rowVaccinazione("ves_n_richiamo"))
                    Next

                    If vaccinazioniEffettuate.Length > 0 Then
                        vaccinazioniEffettuate.Remove(vaccinazioniEffettuate.Length - 1, 1)
                    End If

                End If

            End If
        End If

        Dim movimentoLotto As New Entities.MovimentoLotto()
        movimentoLotto.CodiceLotto = codiceLottoEseguito
        movimentoLotto.CodiceConsultorio = OnVacUtility.Variabili.CNSMagazzino.Codice
        movimentoLotto.NumeroDosi = 1
        movimentoLotto.TipoMovimento = Constants.TipoMovimentoMagazzino.Scarico
        movimentoLotto.IdUtente = OnVacContext.UserId
        movimentoLotto.DataRegistrazione = dataRegistrazione
        movimentoLotto.IdEsecuzioneAssociazione = idEsecuzioneAssociazione
        movimentoLotto.Note = Biz.BizLotti.ImpostaNoteMovimentoVaccinazione(OnVacUtility.Variabili.PazId.ToString(),
                                                                            dataEffettuazioneLotto,
                                                                            vaccinazioniEffettuate.ToString(),
                                                                            Constants.TipoMovimentoMagazzino.Scarico)
        Return movimentoLotto

    End Function

#Region " Log "

    Private Function NewRecordInserimentoProgrammata(row As DataRow, dataConvocazione As DateTime) As Record

        Dim recordLog As New Record()

        recordLog.Campi.Add(New Campo("VPR_CIC_CODICE", "", row("VPR_CIC_CODICE").ToString()))
        recordLog.Campi.Add(New Campo("VPR_N_SEDUTA", "", row("VPR_N_SEDUTA").ToString()))
        recordLog.Campi.Add(New Campo("VPR_ASS_CODICE", "", row("VPR_ASS_CODICE").ToString()))
        recordLog.Campi.Add(New Campo("VPR_CNV_DATA", "", dataConvocazione))
        recordLog.Campi.Add(New Campo("VPR_VAC_CODICE", "", row("VPR_VAC_CODICE").ToString()))
        recordLog.Campi.Add(New Campo("VPR_N_RICHIAMO", "", IIf(row("VPR_N_RICHIAMO") Is DBNull.Value, 0, row("VPR_N_RICHIAMO"))))

        Return recordLog

    End Function

    Private Function NewRecordInserimentoEsclusa(row As DataRow) As Record

        Dim recordLog As New Record()

        recordLog.Campi.Add(New Campo("vex_vac_codice", String.Empty, row("vpr_vac_codice").ToString()))
        recordLog.Campi.Add(New Campo("vex_data_visita", String.Empty, row("vex_data_visita").ToString()))
        recordLog.Campi.Add(New Campo("vex_data_scadenza", String.Empty, row("vex_data_scadenza").ToString()))
        recordLog.Campi.Add(New Campo("vex_moe_codice", String.Empty, row("vex_moe_codice").ToString()))
        recordLog.Campi.Add(New Campo("vex_ope_codice", String.Empty, row("vex_ope_codice").ToString()))

        Return recordLog

    End Function

    Private Function NewRecordUpdateProgrammata(row As DataRow) As Record

        Dim recordLog As New Record()

        recordLog.Campi.Add(New Campo("VPR_CIC_CODICE", row("VPR_CIC_CODICE", DataRowVersion.Original).ToString(), row("VPR_CIC_CODICE").ToString()))
        recordLog.Campi.Add(New Campo("VPR_N_SEDUTA", row("VPR_N_SEDUTA", DataRowVersion.Original).ToString(), row("VPR_N_SEDUTA").ToString()))
        recordLog.Campi.Add(New Campo("VPR_ASS_CODICE", row("VPR_ASS_CODICE", DataRowVersion.Original).ToString(), row("VPR_ASS_CODICE").ToString()))
        recordLog.Campi.Add(New Campo("VPR_VAC_CODICE", row("VPR_VAC_CODICE", DataRowVersion.Original).ToString(), row("VPR_VAC_CODICE").ToString()))
        recordLog.Campi.Add(New Campo("VPR_N_RICHIAMO", row("VPR_N_RICHIAMO", DataRowVersion.Original).ToString(), IIf(row("VPR_N_RICHIAMO") Is DBNull.Value, 0, row("VPR_N_RICHIAMO"))))

        Return recordLog

    End Function

    Private Function NewRecordUpdateEsclusa(row As DataRow) As Record

        Dim recordLog As New Record()

        recordLog.Campi.Add(New Campo("VEX_VAC_CODICE", row("VPR_VAC_CODICE", DataRowVersion.Original).ToString(), row("VPR_VAC_CODICE").ToString()))
        recordLog.Campi.Add(New Campo("VEX_DATA_VISITA", row("VEX_DATA_VISITA", DataRowVersion.Original).ToString(), row("VEX_DATA_VISITA").ToString()))
        recordLog.Campi.Add(New Campo("VEX_DATA_SCADENZA", row("VEX_DATA_SCADENZA", DataRowVersion.Original).ToString(), row("VEX_DATA_SCADENZA").ToString()))
        recordLog.Campi.Add(New Campo("VEX_MOE_CODICE", row("VEX_MOE_CODICE", DataRowVersion.Original).ToString(), row("VEX_MOE_CODICE").ToString()))
        recordLog.Campi.Add(New Campo("VEX_OPE_CODICE", row("VEX_OPE_CODICE", DataRowVersion.Original).ToString(), row("VEX_OPE_CODICE").ToString()))

        Return recordLog

    End Function

    Private Function NewRecordEliminazioneProgrammata(row As DataRow) As Record

        Dim recordLog As New Record()

        recordLog.Campi.Add(New Campo("VPR_CIC_CODICE", String.Empty, row("VPR_CIC_CODICE", DataRowVersion.Original).ToString()))
        recordLog.Campi.Add(New Campo("VPR_N_SEDUTA", String.Empty, row("VPR_N_SEDUTA", DataRowVersion.Original).ToString()))
        recordLog.Campi.Add(New Campo("VPR_ASS_CODICE", String.Empty, row("VPR_ASS_CODICE", DataRowVersion.Original).ToString()))
        recordLog.Campi.Add(New Campo("VPR_VAC_CODICE", String.Empty, row("VPR_VAC_CODICE", DataRowVersion.Original).ToString()))
        recordLog.Campi.Add(New Campo("VPR_N_RICHIAMO", String.Empty, IIf(row("VPR_N_RICHIAMO", DataRowVersion.Original) Is DBNull.Value, 0, row("VPR_N_RICHIAMO", DataRowVersion.Original))))

        Return recordLog

    End Function

#End Region

#Region " Caricamento dati "

    ' Visualizza le informazioni sulla convocazione nella maschera delle vaccinazioni programmate
    Private Sub RiempiCampiCnv()
        '--
        Dim dtConvocazione As DataTable
        Dim countSolleciti As Integer
        '--
        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            '--
            dtConvocazione = genericProvider.Convocazione.GetFromKey(OnVacUtility.Variabili.PazId, DataConvocazione)
            '--
            countSolleciti = genericProvider.Convocazione.GetMaxSollecitoVaccinazioni(OnVacUtility.Variabili.PazId, DataConvocazione)
            '--
            If Not dtConvocazione.Rows(0)("cnv_cns_codice") Is DBNull.Value Then
                Me.tb_cnv_cons.Text = genericProvider.Consultori.GetCnsDescrizione(dtConvocazione.Rows(0)("cnv_cns_codice"))
            Else
                Me.tb_cnv_cons.Text = String.Empty
            End If
            '--
            If Not dtConvocazione.Rows(0)("cnv_amb_codice") Is DBNull.Value Then
                Me.tb_cnv_app_amb.Text = genericProvider.Consultori.GetAmbDescrizione(dtConvocazione.Rows(0)("cnv_amb_codice"))
            Else
                Me.tb_cnv_app_amb.Text = String.Empty
            End If
            '--
        End Using
        '--
        If dtConvocazione.Rows(0)("cnv_data_appuntamento") Is DBNull.Value Then
            Me.tb_cnv_app.Text = ""
        Else
            Dim dataCnv As DateTime = DirectCast(dtConvocazione.Rows(0)("cnv_data_appuntamento"), DateTime)
            If dataCnv.ToString("HH.mm") <> "00.00" Then
                Me.tb_cnv_app.Text = dataCnv.ToString("dd/MM/yyyy HH:mm")
            Else
                Me.tb_cnv_app.Text = dataCnv.ToString("dd/MM/yyyy")
            End If
        End If
        '--
        Me.tb_cnv_dataConv.Text = DirectCast(dtConvocazione.Rows(0)("cnv_data"), DateTime).ToString("dd/MM/yyyy")
        '--
        'impostazione dell'età paziente al momento della convocazione
        '--
        Me.tb_cnv_etaConv.Text = OnVacUtility.ImpostaEtaPazienteConv(CDate(tb_cnv_dataConv.Text), Me.IsGestioneCentrale, Settings)
        '--
        If dtConvocazione.Rows(0)("cnv_data_invio") Is DBNull.Value Then
            Me.tb_cnv_dataInv.Text = ""
        Else
            Me.tb_cnv_dataInv.Text = DirectCast(dtConvocazione.Rows(0)("cnv_data_invio"), DateTime).ToString("dd/MM/yyyy")
        End If
        '--
        Me.tb_cnv_durApp.Text = dtConvocazione.Rows(0)("cnv_durata_appuntamento").ToString()
        '--
        'tipo appuntamento e utente che lo ha generato
        '--
        Me.imgTipoAppuntamento.Visible = (Not dtConvocazione.Rows(0)("cnv_data_appuntamento") Is DBNull.Value)
        Me.lblUtenteAppuntamento.Visible = (Not dtConvocazione.Rows(0)("cnv_data_appuntamento") Is DBNull.Value)
        Me.txtUtenteAppuntamento.Visible = (Not dtConvocazione.Rows(0)("cnv_data_appuntamento") Is DBNull.Value)
        '--
        If Not dtConvocazione.Rows(0)("cnv_data_appuntamento") Is DBNull.Value Then
            Me.imgTipoAppuntamento.ImageUrl = "../../Images/" & If(dtConvocazione.Rows(0)("cnv_tipo_appuntamento").ToString() = "A", "automatico.gif", "manuale.gif")
            Me.imgTipoAppuntamento.ToolTip = IIf(dtConvocazione.Rows(0)("cnv_tipo_appuntamento").ToString = "A", "Appuntamento Automatico", "Appuntamento Manuale")
        End If
        '--
        If dtConvocazione.Rows(0)("cnv_campagna").ToString() = "S" Then
            Me.chkInCampagna.Checked = True
        Else
            Me.chkInCampagna.Checked = False
        End If
        '--
        ' Lettura id utente e recupero descrizione
        Dim valoreIdUtente As String = String.Empty
        '--
        If Not dtConvocazione.Rows(0)("cnv_ute_id") Is DBNull.Value Then
            valoreIdUtente = dtConvocazione.Rows(0)("cnv_ute_id").ToString()
        End If
        '--
        Dim idUtente As Integer
        '--
        If String.IsNullOrEmpty(valoreIdUtente) Then
            idUtente = 0
        Else
            Integer.TryParse(valoreIdUtente, idUtente)
        End If
        '--
        Dim descrizioneUtente As String = String.Empty
        '--
        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            '--
            descrizioneUtente = genericProvider.Utenti.GetDescrizioneUtente(idUtente)
            '--
        End Using
        '--
        If Not String.IsNullOrEmpty(descrizioneUtente) Then
            '--
            Me.txtUtenteAppuntamento.ToolTip = descrizioneUtente
            '--
            If descrizioneUtente.Length > 20 Then
                Me.txtUtenteAppuntamento.Text = descrizioneUtente.Substring(0, 20) & "..."
            Else
                Me.txtUtenteAppuntamento.Text = descrizioneUtente
            End If
            '--
        Else
            '--
            Me.txtUtenteAppuntamento.Text = "(nessun operatore)"
            '--
        End If
        '--
        If countSolleciti = 0 Then

            If Not dtConvocazione.Rows(0)("CNV_PRIMO_APPUNTAMENTO") Is DBNull.Value Then
                txtPrimoAppuntamento.Text = CType(dtConvocazione.Rows(0)("CNV_PRIMO_APPUNTAMENTO"), Date).ToString("dd/MM/yyyy HH:mm")
            End If

            lblRitardoInt.Text = "Ritardo: "
            lblRitardo.Text = "Nessuno"
            lblRitardoInt.ForeColor = Color.Black
            lblRitardo.CssClass = "lblRitardo_Nessuno"

        Else
            ' Impostazione del colore rosso se è presente un ritardo (modifica 14/07/2004)
            lblRitardoInt.Text = "Max Ritardo: "
            lblRitardo.Text = countSolleciti & "°"
            lblRitardoInt.ForeColor = Color.Black
            lblRitardoInt1.ForeColor = Color.Black
            lblRitardo1.ForeColor = Color.Red
            lblRitardoInt2.ForeColor = Color.Black
            lblRitardo2.ForeColor = Color.Red
            lblRitardoInt3.ForeColor = Color.Black
            lblRitardo3.ForeColor = Color.Red
            lblRitardoInt4.ForeColor = Color.Black
            lblRitardo4.ForeColor = Color.Red
            lblRitardo.CssClass = "lblRitardo_Sollecito"

            ' Impostazione delle varie date
            SetMessaggiRitardi(countSolleciti)

        End If
        '--
    End Sub

#Region " Gestione messaggi tab ritardi "

    Private Sub SetMessaggiRitardi(countSolleciti As Integer)
        '--
        Using dam As IDAM = OnVacUtility.OpenDam()
            '--
            With dam.QB
                '--
                .NewQuery()
                .AddTables("T_PAZ_RITARDI")
                .AddSelectFields("PRI_DATA_APPUNTAMENTO1", "PRI_DATA_APPUNTAMENTO2", "PRI_DATA_APPUNTAMENTO3", "PRI_DATA_APPUNTAMENTO4", "PRI_CIC_CODICE", "PRI_SED_N_SEDUTA")
                .AddWhereCondition("PRI_PAZ_CODICE", Comparatori.Uguale, OnVacUtility.Variabili.PazId, DataTypes.Numero)
                .AddWhereCondition("PRI_CNV_DATA", Comparatori.Uguale, DataConvocazione, DataTypes.Data)
                '--
            End With
            '--
            Using dr As IDataReader = dam.BuildDataReader()
                '--
                If Not dr Is Nothing Then
                    '--
                    Dim dataApp1Ordinal As Integer = dr.GetOrdinal("PRI_DATA_APPUNTAMENTO1")
                    Dim dataApp2Ordinal As Integer = dr.GetOrdinal("PRI_DATA_APPUNTAMENTO2")
                    Dim dataApp3Ordinal As Integer = dr.GetOrdinal("PRI_DATA_APPUNTAMENTO3")
                    Dim dataApp4Ordinal As Integer = dr.GetOrdinal("PRI_DATA_APPUNTAMENTO4")
                    '--
                    Dim codiceCicloOrdinal As Integer = dr.GetOrdinal("PRI_CIC_CODICE")
                    Dim numeroSedutaOrdinal As Integer = dr.GetOrdinal("PRI_SED_N_SEDUTA")
                    '--
                    Dim dicDateApp1 As New Dictionary(Of String, List(Of String))
                    Dim dicDateApp2 As New Dictionary(Of String, List(Of String))
                    Dim dicDateApp3 As New Dictionary(Of String, List(Of String))
                    Dim dicDateApp4 As New Dictionary(Of String, List(Of String))
                    '--
                    While dr.Read()
                        '--
                        Me.AddDateToList(dr, dataApp1Ordinal, codiceCicloOrdinal, numeroSedutaOrdinal, dicDateApp1)
                        Me.AddDateToList(dr, dataApp2Ordinal, codiceCicloOrdinal, numeroSedutaOrdinal, dicDateApp2)
                        Me.AddDateToList(dr, dataApp3Ordinal, codiceCicloOrdinal, numeroSedutaOrdinal, dicDateApp3)
                        Me.AddDateToList(dr, dataApp4Ordinal, codiceCicloOrdinal, numeroSedutaOrdinal, dicDateApp4)
                        '--
                    End While
                    '--
                    ' Label per avviso
                    '--
                    If countSolleciti >= 1 Then
                        '--
                        Me.lblRitardoInt1.Text = "App. Avviso: "
                        Me.lblRitardo1.Text = Me.SetMessageLabelRitardo(dicDateApp1)
                        '--
                    End If
                    '--
                    ' Label per primo sollecito
                    '--
                    If countSolleciti >= 2 Then
                        '--
                        Me.lblRitardoInt2.Text = "App. 1° Soll: "
                        Me.lblRitardo2.Text = Me.SetMessageLabelRitardo(dicDateApp2)
                        '--
                        Me.SetCellBorders("tdRitardoInt2")
                        Me.SetCellBorders("tdRitardo2")
                        '--
                    End If
                    '--
                    ' Label per secondo sollecitoù
                    '--
                    If countSolleciti >= 3 Then
                        '--
                        Me.lblRitardoInt3.Text = "App. 2° Soll: "
                        Me.lblRitardo3.Text = Me.SetMessageLabelRitardo(dicDateApp3)
                        '--
                        Me.SetCellBorders("tdRitardoInt3")
                        Me.SetCellBorders("tdRitardo3")
                        '--
                    End If
                    '--
                    ' Label per terzo sollecito
                    '--
                    If countSolleciti = 4 Then
                        '--
                        Me.lblRitardoInt4.Text = "App. 3° Soll: "
                        Me.lblRitardo4.Text = Me.SetMessageLabelRitardo(dicDateApp4)
                        '--
                        Me.SetCellBorders("tdRitardoInt4")
                        Me.SetCellBorders("tdRitardo4")
                        '--
                    End If
                    '--
                End If
                '--
            End Using
            '--
        End Using
        '--
    End Sub

    ' La data, in formato stringa, viene aggiunta all'elenco di date solo se non è già presente.
    Private Sub AddDateToList(dr As IDataReader, posDataApp As Integer, posCodiceCiclo As Integer, posNumeroSeduta As Integer, dicDateApp As Dictionary(Of String, List(Of String)))

        If Not dr.IsDBNull(posDataApp) Then

            Dim dateToAdd As String = dr.GetDateTime(posDataApp).ToString("dd/MM/yyyy")


            Dim item As String = (From d As String In dicDateApp.Keys
                                  Where d = dateToAdd
                                  Select d).FirstOrDefault()

            If String.IsNullOrEmpty(item) Then
                dicDateApp.Add(dateToAdd, New List(Of String))
            End If

            Dim cicloToAdd As String = String.Format("{0} [{1}]", dr.GetString(posCodiceCiclo), dr.GetInt32(posNumeroSeduta))

            item = (From c As String In dicDateApp(dateToAdd)
                    Where c = cicloToAdd
                    Select c).FirstOrDefault()

            If String.IsNullOrEmpty(item) Then
                dicDateApp(dateToAdd).Add(cicloToAdd)
            End If

        End If

    End Sub

    Private Function SetMessageLabelRitardo(dataAppDic As Dictionary(Of String, List(Of String))) As String

        Dim msg As New System.Text.StringBuilder()

        If dataAppDic Is Nothing OrElse dataAppDic.Count = 0 Then

            msg.Append("Nessuno")

        Else

            For Each dataAppKeyValuePair As KeyValuePair(Of String, List(Of String)) In dataAppDic

                If msg.Length > 0 Then msg.Append("<br/>")

                msg.AppendFormat("<span title='{0}'>{1}</span>", String.Join(" - ", dataAppKeyValuePair.Value.ToArray()), dataAppKeyValuePair.Key)

            Next

        End If

        Return msg.ToString()

    End Function

    ' Imposta lo stile nella cella specificata
    Private Sub SetCellBorders(cellName As String)

        Dim obj As System.Web.UI.Control = Me.TabRitardi.FindControl(cellName)

        If obj Is Nothing Then Return

        Dim td As HtmlTableCell = DirectCast(obj, HtmlTableCell)
        td.Attributes("class") += " tdRitardiBorderTop"

    End Sub

#End Region

    ' Riempie il datatable dt_vacEs con tutte le vaccinazioni eseguite
    Private Sub RecuperaVacEseguite(dam As IDAM)
        '--
        Me.dt_vacEs = New DataTable()
        '--
        dam.QB.NewQuery()
        dam.QB.AddTables("t_vac_eseguite")
        dam.QB.AddSelectFields("ves_vac_codice, ves_n_richiamo, ves_data_effettuazione, ves_dataora_effettuazione, ves_note")
        dam.QB.AddWhereCondition("ves_paz_codice", Comparatori.Uguale, OnVacUtility.Variabili.PazId, DataTypes.Numero)
        '--
        dam.BuildDataTable(Me.dt_vacEs)
        '--
    End Sub

    Private Sub RecuperaVaccinazioniProgrammate(azzeraOraEsecuzione As Boolean, dam As IDAM)
        '--
        dt_vacProg = Nothing
        '--
        ' Lettura vac prog da db e riempimento dt_vacProg
        Using genericProvider As New DbGenericProvider(dam)
            Using bizVacProg As New BizVaccinazioneProg(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                dt_vacProg = bizVacProg.GetDtVaccinazioniProgrammatePazienteByData(OnVacUtility.Variabili.PazId, DataConvocazione)

            End Using
        End Using
        '--
        ' Flag di esecuzione (valori: eseguita/esclusa/null)
        dt_vacProg.Columns.Add(New DataColumn("E", GetType(String)))

        ' Indica se la vaccinazione esclusa deve essere salvata su db
        dt_vacProg.Columns.Add(New DataColumn("SaveEsclusa", GetType(Boolean)))
        dt_vacProg.Columns("SaveEsclusa").DefaultValue = False

        ' Indica se la vaccinazione eseguita deve essere salvata su db
        dt_vacProg.Columns.Add(New DataColumn("SaveEseguita", GetType(Boolean)))
        dt_vacProg.Columns("SaveEseguita").DefaultValue = False

        ' Indicano il tipo di valorizzazione dei dati relativi a sito e via: da ciclo, da associazione, da nome commerciale, manualmente o null.
        dt_vacProg.Columns.Add(New DataColumn(NomeFlagTipoValorizzazione.SITO, GetType(String)))
        dt_vacProg.Columns.Add(New DataColumn(NomeFlagTipoValorizzazione.VIA, GetType(String)))

        OnVacUtility.addKey(dt_vacProg, "vpr_vac_codice")
        '--
        ' Imposto la data di esecuzione, azzerando l'orario se la data non è quella odierna
        SetDataOraEsecuzione(azzeraOraEsecuzione)
        '--
        For i As Int16 = 0 To dt_vacProg.Rows.Count - 1
            '--
            Dim row As DataRow = dt_vacProg.Rows(i)
            '--
            row("SaveEsclusa") = False
            row("SaveEseguita") = False
            '--
            If row("E") Is DBNull.Value Then
                '--
                If row("ves_data_effettuazione") Is DBNull.Value Then

                    ' --- Imposto data e ora di effettuazione --- '
                    row("ves_data_effettuazione") = OnVacUtility.Variabili.DataEsecuzione.Date
                    row("ves_dataora_effettuazione") = OnVacUtility.Variabili.DataEsecuzione

                    ' --- Imposto il medico vaccinatore della seduta
                    If Not OnVacUtility.Variabili.MedicoVaccinante.Codice Is Nothing Then
                        row("ves_med_vaccinante") = OnVacUtility.Variabili.MedicoVaccinante.Codice
                        row("ope_vac") = OnVacUtility.Variabili.MedicoVaccinante.Nome
                    End If

                    ' --- Imposto i valori di default per sito di inoculazione e via di somministrazione ---
                    Dim infoSomministrazione As Entities.InfoSomministrazione = Nothing

                    Using genericProvider As New DbGenericProvider(dam)
                        Using bizVacProg As New Biz.BizVaccinazioneProg(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                            Dim command As New Biz.BizVaccinazioneProg.GetInfoSomministrazioneCommand()
                            command.CodiceCiclo = row("vpr_cic_codice").ToString()
                            If Not row("vpr_n_seduta") Is DBNull.Value Then command.NumeroSeduta = Convert.ToInt32(row("vpr_n_seduta"))
                            command.CodiceVaccinazione = row("vpr_vac_codice").ToString()
                            command.CodiceAssociazione = row("vpr_ass_codice").ToString()
                            command.CodiceNomeCommerciale = row("ves_noc_codice").ToString()

                            infoSomministrazione = bizVacProg.GetInfoSomministrazione(command)

                            If Not infoSomministrazione Is Nothing Then

                                row("ves_sii_codice") = infoSomministrazione.CodiceSitoInoculazione
                                row("sii_descrizione") = infoSomministrazione.DescrizioneSitoInoculazione
                                row(NomeFlagTipoValorizzazione.SITO) = infoSomministrazione.FlagTipoValorizzazioneSito

                                row("ves_vii_codice") = infoSomministrazione.CodiceViaSomministrazione
                                row("vii_descrizione") = infoSomministrazione.DescrizioneViaSomministrazione
                                row(NomeFlagTipoValorizzazione.VIA) = infoSomministrazione.FlagTipoValorizzazioneVia

                            End If

                        End Using
                    End Using

                Else
                    '--
                    ' Data di effettuazione valorizzata
                    '--
                    If row("ves_n_richiamo").ToString() = row("vpr_n_richiamo").ToString() Then
                        '--
                        row("E") = StatoVaccinazione.ESEGUITA
                        '--
                        ' Anche il campo con l'orario di effettuazione deve essere valorizzato. Se non lo è, lo pongo uguale alla data.
                        '--
                        If row("ves_dataora_effettuazione") Is DBNull.Value Then
                            row("ves_dataora_effettuazione") = row("ves_data_effettuazione")
                        End If
                        '--
                    Else
                        '--
                        row("ves_data_effettuazione") = OnVacUtility.Variabili.DataEsecuzione.Date
                        row("ves_dataora_effettuazione") = OnVacUtility.Variabili.DataEsecuzione
                        '--
                        row("ves_lot_codice") = DBNull.Value
                        '--
                        ' Imposto il medico vaccinatore della seduta
                        '--
                        If OnVacUtility.Variabili.MedicoVaccinante.Codice Is Nothing Then
                            row("ves_med_vaccinante") = OnVacUtility.Variabili.MedicoVaccinante.Codice
                            row("ope_vac") = OnVacUtility.Variabili.MedicoVaccinante.Nome
                        End If
                        '--
                        row("ves_sii_codice") = DBNull.Value
                        row("sii_descrizione") = DBNull.Value
                        row("ves_vii_codice") = DBNull.Value
                        row("vii_descrizione") = DBNull.Value
                        row("ves_noc_codice") = DBNull.Value
                        row("noc_descrizione") = DBNull.Value
                        '--
                    End If
                    '--
                End If
                ' --- Vaccinazione esclusa --- '
                If Not row("vex_data_visita") Is DBNull.Value Then
                    '--
                    row("E") = StatoVaccinazione.ESCLUSA
                    '--
                End If
                '--
            End If

        Next

        dt_vacProg.AcceptChanges()
        dt_vacProg.DefaultView.Sort = "vac_ordine"

        ImpostaLottiUtilizzatiDefaultSeNecessario(True)

    End Sub

    Private Sub RecuperaBilProg()

        dt_bilProg = New DataTable()

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            dt_bilProg = genericProvider.BilancioProgrammato.LoadBilanciMalattie(OnVacUtility.Variabili.PazId, DataConvocazione)

        End Using

        dt_bilProg.PrimaryKey = New DataColumn() {dt_bilProg.Columns("ID")}

    End Sub

    Private Sub RecuperaSollecitiBilProg(DAM As IDAM)

        Dim maxCount As Short = 5
        Dim firstRow As Boolean = True
        Dim str As New System.Text.StringBuilder("")

        If Me.dt_bilProg Is Nothing Or Me.dt_bilProg.Rows.Count = 0 Then
            Exit Sub
        End If

        If Me.dt_bilProg.Rows.Count < maxCount Then
            maxCount = Me.dt_bilProg.Rows.Count
        End If

        For i As Int16 = 0 To maxCount - 1

            With DAM.QB
                .NewQuery()
                .AddSelectFields("bis_bip_id", "bis_data_invio")
                .AddTables("T_BIL_SOLLECITI")
                .AddWhereCondition("BIS_BIP_ID", Comparatori.Uguale, dt_bilProg.Rows(i)("ID"), DataTypes.Numero)
                .AddOrderByFields("BIS_BIP_ID", "ID")
            End With

            Using dtr As IDataReader = DAM.BuildDataReader()

                Dim posBisDataInvio As Integer = dtr.GetOrdinal("bis_data_invio")

                While dtr.Read()

                    If firstRow Then
                        If (Not dt_bilProg.Rows(i)("BIP_BIL_NUMERO") Is DBNull.Value) Then
                            If dt_bilProg.Rows(i)("MAL_DESCRIZIONE") Is Nothing OrElse (dt_bilProg.Rows(i)("MAL_DESCRIZIONE") Is System.DBNull.Value) Then
                                str.AppendFormat("{0} - Malattia non associata<br/>", dt_bilProg.Rows(i)("BIP_BIL_NUMERO").ToString)
                            Else
                                str.AppendFormat("{0} - Malattia: {1}<br/>", dt_bilProg.Rows(i)("BIP_BIL_NUMERO").ToString, dt_bilProg.Rows(i)("MAL_DESCRIZIONE").ToString)
                            End If
                        End If
                        firstRow = False
                    Else
                        str.Append("<br/>")
                    End If

                    If dtr(posBisDataInvio) Is DBNull.Value OrElse dtr(posBisDataInvio) = Date.MinValue Then
                        str.Append("   ")
                    Else
                        str.AppendFormat("   {0:dd/MM/yyyy}", dtr(posBisDataInvio))
                    End If

                End While

            End Using

            firstRow = True

        Next

        If str.ToString() = String.Empty Then
            str.Append("Nessun ritardo")
        End If

        Me.lblRitardiSolleciti.Text = str.ToString()

    End Sub

    ' Valorizza il datatable dtLottiUtilizzabili con tutti i lotti associabili alle vaccinazioni
    Private Function RecuperaLottiUtilizzabili(soloLottiAttivi As Boolean, includiLottiFuoriEta As Boolean, includiImportiDefault As Boolean) As DataTable

        Dim dtLottiUtilizzabili As DataTable = Nothing

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizVacProg As New Biz.BizVaccinazioneProg(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                dtLottiUtilizzabili = bizVacProg.LoadLottiUtilizzabili(DataConvocazione, dt_vacProg, OnVacUtility.Variabili.CNS.Codice,
                                                                       OnVacUtility.Variabili.CNSMagazzino.Codice, Me.SessoPaziente, Me.EtaPaziente,
                                                                       includiLottiFuoriEta, soloLottiAttivi, includiImportiDefault)

            End Using
        End Using

        Return dtLottiUtilizzabili

    End Function

    ''' <summary>
    ''' Imposta tutti i dati da visualizzare nella maschera delle vaccinazioni programmate. 
    ''' Se vengono gestiti i bilanci controllo anche se ci sono dei bilanci programmati e visualizzo anche le convocazioni per SOLO BILANCIO.
    ''' </summary>
    ''' <param name="azzeraOraEsecuzione"></param>
    Private Sub LoadData(azzeraOraEsecuzione As Boolean)
        '--
        Dim maxDataFineSospensione As Date
        '--
        Using dam As IDAM = OnVacUtility.OpenDam()
            '--
            Using genericProvider As New DbGenericProvider(dam)
                '--
                Using bizPaziente As New Biz.BizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                    '--
                    'Sospensione Paziente
                    '--
                    maxDataFineSospensione = bizPaziente.GetMaxDataFineSospensione(OnVacUtility.Variabili.PazId)
                    '--
                    ' Se il paz è deceduto, non devo poter eseguire le vaccinazioni. 
                    ' Qui faccio la query per sapere se è deceduto, nel click del pulsante Esegui faccio il controllo
                    '--
                    Me.PazienteDeceduto = bizPaziente.IsDeceduto(OnVacUtility.Variabili.PazId)
                    '--
                    ' Stato Vaccinale del paziente
                    Me.StatusVaccinale = bizPaziente.GetStatoVaccinaleString(OnVacUtility.Variabili.PazId)
                    '--
                    ' Età e sesso del paziente
                    Dim datiAnagraficiPaziente As Entities.PazienteDatiAnagrafici = bizPaziente.GetDatiAnagraficiPaziente(OnVacUtility.Variabili.PazId, Me.IsGestioneCentrale)
                    '--
                    If datiAnagraficiPaziente Is Nothing Then
                        Me.EtaPaziente = 0
                        Me.SessoPaziente = String.Empty
                    Else
                        Me.EtaPaziente = Date.Now.Subtract(datiAnagraficiPaziente.DataNascita).TotalDays
                        Me.SessoPaziente = datiAnagraficiPaziente.Sesso
                    End If
                    '--
                End Using
                '--
            End Using
            '--
            Me.RecuperaVacEseguite(dam)
            '--
            Me.RecuperaVaccinazioniProgrammate(azzeraOraEsecuzione, dam)
            '--
            If Settings.GESBIL Then
                '--
                Me.RecuperaBilProg()
                '--
                Me.RecuperaSollecitiBilProg(dam)
                '--
            Else
                '--
                'Creo una dt_bilProg che serve solo quando non si gestiscono i bilanci
                'Questo modo è sbrigativo, volendo si può implementare una verifica caso per caso.
                '--
                Me.dt_bilProg = New DataTable()
                '--
            End If
            '--
            If Settings.PAGAMENTO Then
                '--
                Me.FillDdlEsenzioniMalattie(Me.ddlEseMalPagVac)
                '--
                Me.FillDdlEsenzioniMalattie(Me.ddlEseMalPag)
                '--
            End If
            '--
        End Using
        '--
        Me.RiempiCampiCnv()
        '--
        Me.RiempiCampiEsecuzione(azzeraOraEsecuzione)
        '--
        If maxDataFineSospensione >= Date.Now.Date Then
            Me.txtSospensione.Text = String.Format("Il Paziente e' sospeso fino al {0:dd/MM/yyyy}", maxDataFineSospensione)
        Else
            Me.txtSospensione.Text = String.Empty
        End If
        '--
        Me.BindDGVacProgrammate()
        '--  
        If Settings.GESBIL Then Me.BindDGBilProgrammati()
        '--
        OnitLayout31.Busy = False
        '--
        Me.EnableToolbar(False)
        '--
    End Sub

    Private Sub SetDataOraEsecuzione(azzeraOraEsecuzione As Boolean)
        '--
        Dim now As DateTime = DateTime.Now
        If OnVacUtility.Variabili.DataEsecuzione = DateTime.MinValue Then OnVacUtility.Variabili.DataEsecuzione = now
        '--
        If azzeraOraEsecuzione Then
            If DateTime.Equals(OnVacUtility.Variabili.DataEsecuzione.Date, now.Date) Then
                '--
                ' Data corrente => Ora esecuzione corrente
                OnVacUtility.Variabili.DataEsecuzione = now
                '--
            Else
                '--
                ' Altra data => Ora azzerata
                Dim newDate As DateTime = OnVacUtility.Variabili.DataEsecuzione
                OnVacUtility.Variabili.DataEsecuzione = New DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 0)
                '--
            End If
        End If
        '--
    End Sub

    Private Sub RiempiCampiEsecuzione(azzeraOraEsecuzione As Boolean)
        '--
        SetDataOraEsecuzione(azzeraOraEsecuzione)
        '--
        Me.txtDataEs.Text = OnVacUtility.Variabili.DataEsecuzione.ToString("dd/MM/yyyy")
        Me.txtOraEs.Text = OnVacUtility.Variabili.DataEsecuzione.ToString("HH:mm", System.Globalization.CultureInfo.InvariantCulture)
        '--
        Me.txtMedicoReferenteReadOnly.Text = OnVacUtility.Variabili.MedicoResponsabile.Nome
        Me.txtVaccinatoreReadOnly.Text = OnVacUtility.Variabili.MedicoVaccinante.Nome
        '--
        If OnVacUtility.Variabili.CNS.Ambulatorio.Codice = 0 Then
            '--
            Me.txtAmbulatorioReadOnly.Text = "SELEZIONARE UN AMBULATORIO"
            '--
            ' Medico In Ambulatorio -> OnVacUtility
            '--
            If OnVacUtility.Variabili.MedicoInAmbulatorio Is Nothing Then
                '--
                OnVacUtility.Variabili.MedicoInAmbulatorio = Settings.MEDINAMB
                '--
            End If
            '--
        Else
            '--
            Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                '--
                ' Descrizione Ambulatorio
                '--
                Me.txtAmbulatorioReadOnly.Text = genericProvider.Consultori.GetAmbDescrizione(OnVacUtility.Variabili.CNS.Ambulatorio.Codice)
                '--
                ' Medico In Ambulatorio -> OnVacUtility
                '--
                If OnVacUtility.Variabili.MedicoInAmbulatorio Is Nothing Then
                    '--
                    OnVacUtility.Variabili.MedicoInAmbulatorio = genericProvider.Consultori.GetMedicoInAmb(OnVacUtility.Variabili.CNS.Ambulatorio.Codice)
                    '--
                End If
                '--
            End Using
            '--
        End If
        '--
        ' Medico In Ambulatorio -> Checkbox
        '--
        Me.chkInAmbulatorio.Checked = OnVacUtility.Variabili.MedicoInAmbulatorio.Value
        '--
    End Sub

    'controlla se è attiva la associazione del codice a barre con la pistola (modifica 28/12/2004)
    Private Sub ControllaGestioneBarCodeLotti()

        'controllo se viene usato il codice a barre 
        If Settings.GESMAG Then
            '--
            If Settings.GESBALOT Then
                ' --- se è una convocazione per SOLO BILANCIO non mostro automaticamente la modale di scelta del lotto
                If Me.dg_vacProg.Items.Count > 0 Then
                    '--
                    Me.ShowAssLotti()
                    '--
                End If
            End If
            '--
        End If

    End Sub

    Private Function RecuperaVacProgEseguite() As DataTable

        Dim dt As New DataTable()
        Dim dam As IDAM = OnVacUtility.OpenDam()

        dam.QB.NewQuery()
        dam.QB.AddTables("t_vac_programmate")
        dam.QB.AddSelectFields("vpr_vac_codice", "vpr_n_richiamo")
        dam.QB.AddWhereCondition("vpr_paz_codice", Comparatori.Uguale, OnVacUtility.Variabili.PazId, DataTypes.Numero)

        dam.QB.AddUnion(dam.QB.GetSelect)

        dam.QB.NewQuery(False)
        dam.QB.AddTables("t_vac_eseguite")
        dam.QB.AddSelectFields("ves_vac_codice", "ves_n_richiamo as vpr_n_richiamo")
        dam.QB.AddWhereCondition("ves_paz_codice", Comparatori.Uguale, OnVacUtility.Variabili.PazId, DataTypes.Numero)

        dam.QB.AddUnion(dam.QB.GetSelect)

        Dim q1 As String = dam.QB.GetSelect(True)

        dam.QB.NewQuery(False, False)
        dam.QB.AddSelectFields("vpr_vac_codice", "max(vpr_n_richiamo) as vpr_n_richiamo")
        dam.QB.AddTables("(" & q1 & ")")
        dam.QB.AddGroupByFields("vpr_vac_codice")

        Try
            dam.BuildDataTable(dt)

            ' Elimino le vac prog eliminate non ancora salvate
            If Not dt_vacProg Is Nothing AndAlso Not dt Is Nothing Then

                For i As Integer = 0 To dt_vacProg.Rows.Count - 1

                    If dt_vacProg.Rows(i).RowState = DataRowState.Deleted Then

                        ' La elimino da dt perchè in realtà non è più programmata
                        For j As Integer = dt.Rows.Count - 1 To 0 Step -1
                            If (dt.Rows(j).Item("vpr_vac_codice") = dt_vacProg.Rows(i).Item("vpr_vac_codice", DataRowVersion.Original) And
                                dt.Rows(j).Item("vpr_n_richiamo") = dt_vacProg.Rows(i).Item("vpr_n_richiamo", DataRowVersion.Original)) Then
                                dt.Rows(j).Delete()
                            End If
                        Next

                        dt.AcceptChanges()

                    End If
                Next
            End If

        Finally
            OnVacUtility.CloseDam(dam)
        End Try

        Return dt

    End Function

    Private Function CaricaCodiciAssociazioniLotto(lotto As String) As String()
        '--
        Dim associazioniLotto As New List(Of String)
        '--
        Using dam As IDAM = OnVacUtility.OpenDam()
            '--
            dam.QB.NewQuery()
            dam.QB.AddSelectFields("nal_ass_codice")
            dam.QB.AddTables("t_ana_lotti", "t_ana_link_noc_associazioni")
            '--
            If Settings.ASSOCIAZIONI_TIPO_CNS Then
                dam.QB.AddTables("t_ana_associazioni_tipi_cns", "t_ana_consultori")
            End If
            '--
            dam.QB.AddWhereCondition("lot_noc_codice", Comparatori.Uguale, "nal_noc_codice", DataTypes.Join)
            '--
            If Settings.ASSOCIAZIONI_TIPO_CNS Then
                '--
                dam.QB.AddWhereCondition("nal_ass_codice", Comparatori.Uguale, "atc_ass_codice", DataTypes.Join)
                dam.QB.AddWhereCondition("atc_cns_tipo", Comparatori.Uguale, "cns_tipo", DataTypes.Join)
                dam.QB.AddWhereCondition("cns_codice", Comparatori.Uguale, OnVacUtility.Variabili.CNS.Codice, DataTypes.Stringa)
                '--
            End If
            '--
            dam.QB.AddWhereCondition("lot_codice", Comparatori.Uguale, lotto, DataTypes.Stringa)
            '--
            Using reader As IDataReader = dam.BuildDataReader()
                '--
                While reader.Read()
                    '--
                    associazioniLotto.Add(reader.GetString(0))
                    '--
                End While
                '--
            End Using
            '--
        End Using
        '--
        Return associazioniLotto.ToArray()
        '--
    End Function

    Private Function GetCodiceAssociazione(row As DataRow, dam As IDAM) As String
        '--
        Dim codiceAssociazione As String = row("vpr_ass_codice").ToString()
        '-- 
        ' per i nomi commerciali che avevano più di un'associazione è stata mostrata
        ' la mascherina per scegliere tale associazione, per gli altri viene asscociato
        ' automaticamente
        '-- 
        If String.IsNullOrEmpty(codiceAssociazione) Then
            '--
            'la funzione ritorna l'associazione del NC
            '--
            codiceAssociazione = Me.GetCodiceAssociazioneFromNomeCommerciale(row("ves_noc_codice").ToString(), dam)
            '--
        End If
        '--
        Return codiceAssociazione
        '--
    End Function

    'la funzione ritorna il codice dell'associazione dal codice del NC
    Private Function GetCodiceAssociazioneFromNomeCommerciale(nocCodice As String, dam As IDAM) As String

        If nocCodice = String.Empty Then Return String.Empty

        Dim assCodice As String = String.Empty

        With dam.QB
            .NewQuery()
            .AddTables("t_ana_link_noc_associazioni")
            .AddSelectFields("MAX(nal_ass_codice)")
            .AddWhereCondition("nal_noc_codice", Comparatori.Uguale, nocCodice, DataTypes.Stringa)
        End With

        Dim obj As Object = dam.ExecScalar()

        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
            assCodice = obj.ToString()
        End If

        Return assCodice

    End Function

    Private Function CaricaBilanci() As Collection.BilancioProgrammatoCollection

        Dim bilanci As New Collection.BilancioProgrammatoCollection()

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizBilanci As New BizBilancioProgrammato(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                bilanci = bizBilanci.BilanciDaProgrammareManuale(OnVacUtility.Variabili.PazId, "")

            End Using
        End Using

        If dt_bilProg.Rows.Count > 0 Then
            '--
            Dim idBilancioEdit As Nullable(Of Decimal)
            '--
            If dg_bilProg.EditItemIndex <> -1 Then
                '--
                idBilancioEdit = Convert.ToDecimal(DirectCast(dg_bilProg.Items.Item((dg_bilProg.EditItemIndex)).FindControl("lbl_id"), Label).Text)
                '--
            End If
            '--
            Dim ds As New System.Data.DataSet("DSDistinct")
            Dim DtsHelper As New OnVac.Common.Utility.DataSetHelper
            '--
            DtsHelper.setDataSet(ds)
            '--
            Dim dtmal As DataTable = DtsHelper.SelectDistinct("Malattie", dt_bilProg, "bip_mal_codice")
            '--
            If Not dtmal Is Nothing Then
                '--
                For i As Int16 = 0 To dtmal.Rows.Count - 1
                    '--
                    If dtmal.Rows(i).Item("bip_mal_codice").ToString <> "" Then
                        '--
                        For j As Int16 = dt_bilProg.Rows.Count - 1 To 0 Step -1
                            '--
                            If dt_bilProg.Rows(j).RowState <> DataRowState.Deleted Then
                                '--
                                If dt_bilProg.Rows(j)("ID").ToString() <> idBilancioEdit.ToString() Then
                                    '--
                                    If dt_bilProg.Rows(j)("bip_mal_codice").ToString = dtmal.Rows(i)("bip_mal_codice").ToString AndAlso dt_bilProg.Rows(j)("escluso").ToString <> "X" Then
                                        '--
                                        For k As Int16 = bilanci.Count - 1 To 0 Step -1
                                            '--
                                            If bilanci(k).Mal_codice.ToString = dtmal.Rows(i)("bip_mal_codice").ToString Then
                                                '--
                                                bilanci.RemoveAt(k)
                                                '--
                                            End If
                                            '--
                                        Next
                                        '--
                                    End If
                                    '--
                                End If
                                '--
                            End If
                            '--
                        Next
                        '--
                    End If
                    '--
                Next
                '--
            End If
            '--
        End If
        '--
        Return bilanci
        '--
    End Function

#End Region

#Region " Datagrid "

    ' Restituisce true se almeno uno dei due datagrid (vaccinazioni e bilanci) è in edit
    ' Restituisce false se entrambi i datagrid non sono in edit
    ' Se il parametro showWarning è true, avvisa l'utente che il datagrid è in edit.
    Private Function IsDgrEditable(showWarning As Boolean) As Boolean

        If (dg_vacProg.EditItemIndex = -1) And (dg_bilProg.EditItemIndex = -1) Then
            Return False
        Else
            If showWarning Then strJS &= "alert('" + WARNING_DGR_EDIT_MODE + "')" & vbCrLf
            Return True
        End If

    End Function

    Private Function GetRowKey(dg_index As Integer) As Object()

        Return New Object() {DirectCast(dg_vacProg.Items.Item(dg_index).FindControl("lblCodVac"), Label).Text}

    End Function

#Region " Funzioni che restituiscono i controlli nelle righe del dg_vacProg "

    Private Function GetModalListControlFromDgVacProg(rowIndex As Int16, fmName As String) As OnitModalList

        Dim fm As System.Web.UI.Control = dg_vacProg.Items(rowIndex).FindControl(fmName)

        If fm Is Nothing Then Return Nothing

        Return DirectCast(fm, OnitModalList)

    End Function

    Private Function GetTextBoxControlFromDgVacProg(rowIndex As Int16, txtName As String) As TextBox

        Return DirectCast(dg_vacProg.Items(rowIndex).FindControl(txtName), TextBox)

    End Function

    Private Function GetDatePickControlFromDgVacProg(rowIndex As Int16, dpkName As String) As OnitDatePick

        Return DirectCast(dg_vacProg.Items(rowIndex).FindControl(dpkName), OnitDatePick)

    End Function

    Private Function GetDropDownListFromDgVacProg(rowIndex As Int16, ddlName As String) As DropDownList

        Return DirectCast(dg_vacProg.Items(rowIndex).FindControl(ddlName), DropDownList)

    End Function

    Private Function GetDdlSelectedValueFromGrid(dataGridItemIndex As Integer, ddlId As String) As String

        Dim value As String = String.Empty

        Dim ddl As DropDownList = DirectCast(Me.dg_vacProg.Items(dataGridItemIndex).FindControl(ddlId), DropDownList)
        If Not ddl Is Nothing AndAlso ddl.SelectedIndex > -1 Then
            value = ddl.SelectedValue
        End If

        Return value

    End Function

#End Region

    Private Sub BindDGVacProgrammate()
        ' todo !??!
        'Aggiorna lo stato del dataview (caccapupù)
        '--
        dt_vacProg.DefaultView.RowStateFilter = DataViewRowState.None
        dt_vacProg.DefaultView.RowStateFilter = DataViewRowState.CurrentRows

        dg_vacProg.DataSource = dt_vacProg.DefaultView
        dg_vacProg.DataBind()
        '--
        For i As Int16 = 0 To Me.dg_vacProg.Items.Count - 1

            If i <> Me.dg_vacProg.EditItemIndex Then

                Dim vaccinazioneEseguita As Boolean = DirectCast(Me.dg_vacProg.Items(i).FindControl("lb_es"), Label).Visible
                Dim vaccinazioneEsclusa As Boolean = DirectCast(Me.dg_vacProg.Items(i).FindControl("lb_ex"), Label).Visible

                If Not VaccinazioniEditable OrElse vaccinazioneEseguita Then

                    Me.HideDatagridItemButtons(Me.dg_vacProg.Items(i), True)

                Else

                    If vaccinazioneEsclusa Then

                        Me.HideDatagridItemButtons(Me.dg_vacProg.Items(i), False)

                    End If

                End If

            End If

        Next
        '--
        If Settings.PAGAMENTO Then
            '--
            ' Calcola il ticket, sommando una sola volta ogni nome commerciale.
            '--
            Dim codiciNomeCommercialiSommati As New List(Of String)()
            '--
            Dim importoTicket As Double = 0
            '--
            For Each dr_VacProgTemp As DataRow In dt_vacProg.Rows
                '--
                If dr_VacProgTemp.RowState <> DataRowState.Deleted Then
                    '--
                    Dim codiceNomeCommerciale As String = dr_VacProgTemp("ves_noc_codice").ToString()
                    '--
                    If Not codiciNomeCommercialiSommati.Contains(codiceNomeCommerciale) Then
                        '--
                        If Not dr_VacProgTemp("ves_importo") Is DBNull.Value Then
                            '--
                            importoTicket += Convert.ToDouble(dr_VacProgTemp("ves_importo"))
                            '--
                        End If
                        '--
                        codiciNomeCommercialiSommati.Add(codiceNomeCommerciale)
                        '--
                    End If
                    '--
                End If
                '--
            Next
            '--
            Me.txtImpTotPag.Text = importoTicket.ToString("C", OnVacUtility.NumberFormatInfo)
            '--
        End If
        '--
        'se è una convocazione per solo bilancio non mostro il datagrid delle vaccinazioni programmate
        '--
        Me.PanelTitolo_sezione.Visible = (Me.dg_vacProg.Items.Count > 0)
        Me.LayoutTitolo_leg.Visible = (Me.dg_vacProg.Items.Count > 0)
        Me.dg_vacProg.Visible = (Me.dg_vacProg.Items.Count > 0)
        '--
    End Sub

    Private Sub HideDatagridItemButtons(datagridItem As DataGridItem, hideDeleteButton As Boolean)

        ' Nasconde il checkbox
        datagridItem.FindControl("cb").Visible = False

        ' Nasconde il pulsante di delete
        If hideDeleteButton Then
            datagridItem.FindControl("btnDelete").Visible = False
        End If

        ' Nasconde il pulsante di edit
        datagridItem.FindControl("btnEdit").Visible = False

        ' Disabilita i controlli in edit
        Dim ddl As DropDownList = DirectCast(datagridItem.FindControl("ddlSii"), DropDownList)
        If Not ddl Is Nothing Then ddl.Enabled = False

        ddl = DirectCast(datagridItem.FindControl("ddlVii"), DropDownList)
        If Not ddl Is Nothing Then ddl.Enabled = False

        Dim oml As OnitModalList = Me.GetModalListControlFromDgVacProg(datagridItem.ItemIndex, "omlCondSanitaria")
        If Not oml Is Nothing Then oml.Enabled = False

        oml = Me.GetModalListControlFromDgVacProg(datagridItem.ItemIndex, "omlCondRischio")
        If Not oml Is Nothing Then oml.Enabled = False

        ' Disabilita il pulsante dei dati di pagamento
        Dim lnk As LinkButton = DirectCast(datagridItem.FindControl("lnkPagVac"), LinkButton)
        If Not lnk Is Nothing Then
            EnableLinkButtonPagamento(lnk, False)
        End If

        ' Disabilita il pulsante delle note
        Dim btn As ImageButton = DirectCast(datagridItem.FindControl("btnNoteVac"), ImageButton)
        If Not btn Is Nothing Then
            Dim rowVacProg As DataRow = dt_vacProg.Rows.Find(GetRowKey(datagridItem.ItemIndex))
            EnableImageButtonNote(btn, False, Not String.IsNullOrWhiteSpace(rowVacProg("ves_note").ToString()))
        End If

    End Sub

    Private Sub BindDGBilProgrammati()

        dg_bilProg.DataSource = dt_bilProg.DefaultView
        dg_bilProg.DataBind()

        'se non vengono gestiti i bilanci o l'elenco dei bilanci programmati per la convocazione è vuoto, non mostro il datagrid dei bilanci
        pan_legBil.Visible = Settings.GESBIL AndAlso (dg_bilProg.Items.Count > 0)
        LayoutTitolo_bil.Visible = Settings.GESBIL AndAlso (dg_bilProg.Items.Count > 0)
        dg_bilProg.Visible = Settings.GESBIL AndAlso (dg_bilProg.Items.Count > 0)

    End Sub

    Private Function GetCurrentDataGridVacProgItem(sender As Object, controlId As String) As DataGridItem

        For Each item As DataGridItem In dg_vacProg.Items

            If item.FindControl(controlId) Is sender Then
                Return item
            End If

        Next

        Return Nothing

    End Function

#End Region

#Region " Antigene / Codice Vaccinazione AVN "

    Private Function GetCodiceAntigene(codiceAss As String, codiceVac As String) As String
        Dim result As String = String.Empty
        Using dbGenericProviderFactory As New DbGenericProviderFactory()
            '--
            Using genericProvider As DbGenericProvider = dbGenericProviderFactory.GetDbGenericProvider(OnVacContext.AppId, OnVacContext.Azienda)

                Using biz As New BizVaccinazioniAnagrafica(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                    Return biz.GetAntigene(codiceAss, codiceVac)

                End Using

            End Using
        End Using

    End Function

#End Region

#Region " Tipo Erogatore Consultori AVN "

    Private Function GetTipoErogatoreConsultorio(codiceCns As String) As String
        Dim result As String = String.Empty
        Using dbGenericProviderFactory As New DbGenericProviderFactory()
            '--
            Using genericProvider As DbGenericProvider = dbGenericProviderFactory.GetDbGenericProvider(OnVacContext.AppId, OnVacContext.Azienda)

                Using biz As New BizConsultori(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.VAC_PROGRAMMATE, True))

                    Return biz.GetTipoErogatoreConsultorio(codiceCns)

                End Using

            End Using
        End Using

    End Function

#End Region

#Region " Struttura - Consultori AVN "

    Private Function GetStruttura(codiceCns As String, tipoErogatore As String) As Entities.Struttura
        Dim result As New List(Of Entities.Struttura)
        Using dbGenericProviderFactory As New DbGenericProviderFactory()
            '--
            Using genericProvider As DbGenericProvider = dbGenericProviderFactory.GetDbGenericProvider(OnVacContext.AppId, OnVacContext.Azienda)

                Using biz As New BizConsultori(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.VAC_PROGRAMMATE, True))

                    result = biz.GetStrutture(codiceCns, tipoErogatore)

                End Using

            End Using
        End Using

        If result.Count > 0 Then
            Return result.FirstOrDefault()
        End If

        Return New Entities.Struttura()

    End Function

#End Region

#Region " Pagamento "

    Private Sub CopiaPagamentoVaccinazionePerNomeCommercialeEdImpostaToolTip(gridItemIndex As Integer, currentRowVacProg As DataRow)

        Dim codNC As String = currentRowVacProg("ves_noc_codice").ToString()
        If Not String.IsNullOrWhiteSpace(codNC) Then

            For Each rowVacProg As DataRow In dt_vacProg.Rows

                If Not rowVacProg Is currentRowVacProg AndAlso rowVacProg.RowState <> DataRowState.Deleted AndAlso Not Me.IsEsclusaEseguita(rowVacProg) Then

                    If rowVacProg("ves_noc_codice").ToString() = codNC Then

                        rowVacProg("ves_mal_codice_malattia") = currentRowVacProg("ves_mal_codice_malattia")
                        rowVacProg("ves_codice_esenzione") = currentRowVacProg("ves_codice_esenzione")
                        rowVacProg("ves_importo") = currentRowVacProg("ves_importo")
                        rowVacProg("noc_tpa_guid_tipi_pagamento") = currentRowVacProg("noc_tpa_guid_tipi_pagamento")
                        rowVacProg("flag_importo") = currentRowVacProg("flag_importo")
                        rowVacProg("flag_esenzione") = currentRowVacProg("flag_esenzione")

                    End If
                End If

            Next

            SetLnkPagVacToolTipPerNomeCommerciale(currentRowVacProg)

        End If
    End Sub

    Private Sub SetLnkPagVacToolTipPerNomeCommerciale(rowVacProg As DataRow)

        For i As Int16 = 0 To Me.dg_vacProg.Items.Count - 1

            Dim row As DataRow = dt_vacProg.Rows.Find(Me.GetRowKey(Me.dg_vacProg.Items(i).ItemIndex))
            If Not row Is Nothing AndAlso row.RowState <> DataRowState.Deleted AndAlso Not Me.IsEsclusaEseguita(row) Then

                If row("ves_noc_codice").ToString() = rowVacProg("ves_noc_codice").ToString() Then

                    Dim lnk As LinkButton = DirectCast(Me.dg_vacProg.Items(i).FindControl("lnkPagVac"), LinkButton)
                    If Not lnk Is Nothing Then

                        Dim importo As Decimal? = Nothing
                        If Not rowVacProg.IsNull("ves_importo") Then
                            importo = Convert.ToDecimal(rowVacProg("ves_importo"))
                        End If

                        Dim codiceEsenzione As String = String.Empty
                        If Not rowVacProg.IsNull("ves_codice_esenzione") Then
                            codiceEsenzione = rowVacProg("ves_codice_esenzione").ToString()
                        End If

                        lnk.ToolTip = GetLnkPagVacToolTip(importo, codiceEsenzione)

                    End If
                End If
            End If

        Next

    End Sub

    Private Function GetLnkPagVacToolTip(importo As Decimal?, codiceEsenzione As String) As String

        Dim toolTip As New System.Text.StringBuilder("Dati di pagamento")

        If importo.HasValue Then
            toolTip.AppendFormat(" - Importo: {0}", importo.Value.ToString("C", OnVacUtility.GetNumberFormatInfo(True)))
        End If

        If Not String.IsNullOrWhiteSpace(codiceEsenzione) Then
            toolTip.AppendFormat(" - Esenzione: {0}", codiceEsenzione)
        End If

        Return toolTip.ToString()

    End Function

    Private Sub FillDdlEsenzioniMalattie(ddl As DropDownList)
        '--
        ddl.Items.Clear()
        '--
        Dim dam As IDAM = OnVacUtility.OpenDam()
        '--
        dam.QB.NewQuery()
        dam.QB.AddTables("T_ANA_MALATTIE")
        dam.QB.AddSelectFields("MAL_CODICE", "MAL_DESCRIZIONE", "MAL_CODICE_ESENZIONE")
        dam.QB.AddWhereCondition("MAL_CODICE_ESENZIONE", Comparatori.IsNot, DBNull.Value, DataTypes.Stringa)
        '--
        Try
            '--
            Using reader As IDataReader = dam.BuildDataReader()
                While reader.Read()
                    '--
                    Dim codiceMalattia As String = reader.GetString(0)
                    Dim descrizioneMalattia As String = reader.GetString(1)
                    Dim codiceEsenzione As String = reader.GetString(2)
                    '--
                    ddl.Items.Add(New ListItem(String.Format("{0} - {1}", descrizioneMalattia, codiceEsenzione), String.Format("{0}|{1}", codiceMalattia, codiceEsenzione)))
                    '--
                End While
            End Using
            '--
        Finally
            '--
            OnVacUtility.CloseDam(dam)
            '--
        End Try
        '--
        ddl.Items.Insert(0, "")
        '--
    End Sub

    Private Sub EnableDdlEseMalPag(enabled As Boolean)

        ddlEseMalPag.Enabled = enabled
        ddlEseMalPag.CssClass = IIf(enabled, "textbox_stringa", "textbox_stringa_disabilitato")

    End Sub

#End Region

#Region " Note Vaccinazione "

    Protected Function GetNoteVacImageUrl(hasNote As Boolean, enable As Boolean) As String

        If hasNote Then Return UrlIconaNotaSi(enable)

        Return UrlIconaNotaNo(enable)

    End Function

    Private Sub CopiaNoteVaccinazionePerNomeCommerciale(rowVacProg As DataRow)
        '--
        Dim codCicDg As String
        '--
        ' Codice del ciclo relativo alla riga modificata
        '--
        Dim codCicRow As String = rowVacProg("vpr_cic_codice").ToString()
        '--
        ' Codice del NC relativa alla riga modificata
        '--
        Dim codNC As String = rowVacProg("ves_noc_codice").ToString()
        '--
        Dim _note As String = rowVacProg("ves_note").ToString()
        '--
        For count As Integer = 0 To dt_vacProg.Rows.Count - 1
            '--
            If dt_vacProg.Rows(count).RowState <> DataRowState.Deleted Then
                '--
                ' Codice del ciclo dell'i-esima riga del datagrid
                '--
                codCicDg = dt_vacProg.Rows(count)("vpr_cic_codice").ToString()
                '--
                If codNC = dt_vacProg.Rows(count)("ves_noc_codice").ToString() Then
                    '--
                    If Not (rowVacProg("vpr_vac_codice") = dt_vacProg.Rows(count)("vpr_vac_codice") And codCicRow = codCicDg) Then
                        '--
                        dt_vacProg.Rows(count)("ves_note") = _note
                        '--
                    End If
                    '--
                End If
                '--
                ' Impostazione icona note
                '--
                Dim controlBtn As Object = Nothing
                Try
                    controlBtn = dg_vacProg.Items(count).FindControl("btnNoteVac")
                Catch ex As Exception

                End Try

                Dim _btn As ImageButton = TryCast(controlBtn, ImageButton)
                If _btn IsNot Nothing Then
                    _btn.ImageUrl = GetNoteVacImageUrl(_note <> String.Empty, True)
                End If
                '--
            End If
            '--
        Next
        '--
    End Sub

#End Region

#Region " Dati Pagamento (importo ed esenzione) "

    ' Da utilizzare per il controllo dei flag relativi al pagamento (flag_esenzione e flag_importo)
    ' Valori possibili: 0/NULL = Disabilitato; 1 = Abilitato; 2 = Obbligatorio
    Private Function IsFlagPagamentoRequired(valoreFlag As Object) As Boolean

        If valoreFlag Is Nothing OrElse valoreFlag Is DBNull.Value Then Return False

        Dim flagPagamento As Enumerators.StatoAbilitazioneCampo = DirectCast(Convert.ToInt32(valoreFlag), Enumerators.StatoAbilitazioneCampo)

        Return (flagPagamento = Enumerators.StatoAbilitazioneCampo.Obbligatorio)

    End Function

    Private Function GetStatoAbilitazione(fieldValue As Object) As Enumerators.StatoAbilitazioneCampo
        '--
        If fieldValue Is DBNull.Value Then Return Enumerators.StatoAbilitazioneCampo.Disabilitato
        '--
        Return DirectCast(Convert.ToInt32(fieldValue), Enumerators.StatoAbilitazioneCampo)
        '--
    End Function

    Private Sub SetEsenzionePagamento(statoEsenzione As Enumerators.StatoAbilitazioneCampo, codiceMalattia As String, codiceEsenzione As String)
        '--
        Select Case statoEsenzione
            '--
            Case Enumerators.StatoAbilitazioneCampo.Disabilitato
                '--
                Me.ddlEseMalPagVac.ClearSelection()
                '--
                Me.ddlEseMalPagVac.CssClass = "Textbox_stringa_disabilitato"
                Me.ddlEseMalPagVac.Enabled = False
                '--
            Case Enumerators.StatoAbilitazioneCampo.Abilitato,
                 Enumerators.StatoAbilitazioneCampo.Obbligatorio
                '--
                Me.ddlEseMalPagVac.ClearSelection()
                If Not String.IsNullOrEmpty(codiceMalattia) Then
                    Me.ddlEseMalPagVac.SelectedValue = String.Format("{0}|{1}", codiceMalattia, codiceEsenzione)
                End If
                '--
                Me.ddlEseMalPagVac.CssClass = IIf(statoEsenzione = Enumerators.StatoAbilitazioneCampo.Abilitato, "Textbox_stringa", "Textbox_stringa_obbligatorio")
                Me.ddlEseMalPagVac.Enabled = True
                '--
        End Select
        '--
    End Sub

    Private Sub SetImportoPagamento(statoImporto As Enumerators.StatoAbilitazioneCampo, valoreImporto As String)
        '--
        Select Case statoImporto
            '--
            Case Enumerators.StatoAbilitazioneCampo.Disabilitato
                '--
                Me.valImpPagVac.Text = String.Empty
                '--
                Me.valImpPagVac.CssClass = "Textbox_numerico_disabilitato"
                Me.valImpPagVac.Enabled = False
                '--
            Case Enumerators.StatoAbilitazioneCampo.Abilitato,
                 Enumerators.StatoAbilitazioneCampo.Obbligatorio
                '--
                Me.valImpPagVac.Text = valoreImporto
                '--
                Me.valImpPagVac.CssClass = IIf(statoImporto = Enumerators.StatoAbilitazioneCampo.Abilitato, "Textbox_numerico", "Textbox_numerico_obbligatorio")
                Me.valImpPagVac.Enabled = True
                '--
        End Select
        '--
    End Sub

    Private Function GetTipoPagamento(codiceTipoPagamento As String) As Entities.TipiPagamento

        If String.IsNullOrEmpty(codiceTipoPagamento) Then Return Nothing

        Return GetTipoPagamento(New Guid(codiceTipoPagamento))

    End Function

    Private Function GetTipoPagamento(guidTipoPagamento As Guid) As Entities.TipiPagamento

        If guidTipoPagamento = Guid.Empty Then Return Nothing

        Dim tipoPagamento As Entities.TipiPagamento = Nothing

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizNomiCommerciali As New BizNomiCommerciali(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                tipoPagamento = bizNomiCommerciali.GetTipoPagamento(guidTipoPagamento)

            End Using
        End Using

        Return tipoPagamento

    End Function

    Private Function GetTipoPagamentoSelezionato() As Entities.TipiPagamento

        If String.IsNullOrEmpty(ddlTipiPagVac.SelectedValue) Then Return Nothing

        Dim guidTipoPagamentoSelezionato As Guid = New Guid(ddlTipiPagVac.SelectedValue)

        Dim tipoPagamento As Entities.TipiPagamento = Nothing

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizNomiCommerciali As New BizNomiCommerciali(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                tipoPagamento = bizNomiCommerciali.GetTipoPagamento(guidTipoPagamentoSelezionato)

            End Using
        End Using

        Return tipoPagamento

    End Function

#End Region

#Region " Finestra Modale Esclusioni "

    Private Sub ShowEscludiVac()

        ' Creazione lista dati vaccinazioni da escludere
        Dim list As New List(Of OnVac_InsDatiEsc.VaccinazioneProgrammataDaEscludere)()

        For i As Int16 = 0 To dg_vacProg.Items.Count - 1

            If DirectCast(dg_vacProg.Items(i).FindControl("cb"), CheckBox).Checked Then

                Dim rowVacProg As DataRow = dt_vacProg.Rows.Find(GetRowKey(i))

                Dim v As New OnVac_InsDatiEsc.VaccinazioneProgrammataDaEscludere()
                v.IndiceVaccinazioneProgrammata = i
                v.CodiceVaccinazione = rowVacProg("vpr_vac_codice").ToString()
                v.DescrizioneVaccinazione = rowVacProg("vac_descrizione").ToString()

                If rowVacProg.IsNull("vpr_n_richiamo") Then
                    v.DoseVaccinazione = 0
                Else
                    v.DoseVaccinazione = Int16.Parse(rowVacProg("vpr_n_richiamo").ToString(), Globalization.NumberStyles.None, Globalization.CultureInfo.InvariantCulture)
                End If

                list.Add(v)

            End If

        Next

        If list.Count = 0 Then
            OnitLayout31.InsertRoutineJS("alert('Nessuna vaccinazione selezionata per l\'esclusione.');")
        Else
            uscInsDatiEsc.ModaleName = modInsDatiEsc.ClientID
            uscInsDatiEsc.Inizializza(list)
            modInsDatiEsc.VisibileMD = True
        End If

    End Sub

#End Region

#Region " Finestra Modale Inserimento Lotto "

    Private Sub ShowInsLotto()

        uscInsLotto.ModaleName = "modInsLotto"
        uscInsLotto.LoadModale()

        modInsLotto.VisibileMD = True

    End Sub

#End Region

#Region " Finestra Modale Inserimento Associazione "

    Private Sub ShowInserisciAssociazione()

        Dim codiciVaccinazioniProgrammate As New List(Of String)

        For Each dgiVacProg As DataGridItem In dg_vacProg.Items
            codiciVaccinazioniProgrammate.Add(DirectCast(dgiVacProg.FindControl("lblCodVac"), Label).Text)
        Next

        uscInsAssociazione.CodiciVaccinazioniProgrammateConvocazione = codiciVaccinazioniProgrammate.ToArray()
        uscInsAssociazione.DataConvocazione = DataConvocazione

        uscInsAssociazione.ModaleName = "modInsAssociazione"
        uscInsAssociazione.LoadModale()

        modInsAssociazione.VisibileMD = True

    End Sub

#End Region

#Region " Finestra Modale Associa Lotti "

    Private Sub ShowAssLotti()

        uscAssLotti.ModaleName = "modAssLotto"
        uscAssLotti.LoadModale()

        modAssLotto.VisibileMD = True

    End Sub

#End Region

    Private Sub RedirectToVisioneBilanci()

        OnitLayout31.InsertRoutineJS(GetOpenLeftFrameScript(True) + Environment.NewLine)
        Response.Redirect(String.Format("../../HBilanci/VisioneBilanci/VisioneBilanci.aspx?isVacProgRequest=True&DataCnv={0}&EditCnv={1}",
                                        HttpUtility.UrlEncode(DataConvocazione),
                                        HttpUtility.UrlEncode(EditConvocazioni)),
                          True)

    End Sub

    Private Function IsEsclusaEseguita(rowVaccinazioneProgrammata As DataRow) As Boolean
        '--
        Return BizVaccinazioneProg.IsEsclusaEseguita(rowVaccinazioneProgrammata)
        '--
    End Function

    Private Function FindData(index As Int16) As Boolean

        Dim numeroRichiamo As Int16 = GetTextBoxControlFromDgVacProg(index, "txtNumRich").Text
        Dim codiceVaccinazione As String = DirectCast(dg_vacProg.Items(index).FindControl("lblCodVac"), Label).Text

        Dim rowTemp As DataRow

        For i As Integer = 0 To dt_vacEs.Rows.Count - 1

            rowTemp = dt_vacEs.Rows(i)

            If OnVacUtility.Variabili.DataEsecuzione.Date = Convert.ToDateTime(rowTemp("ves_data_effettuazione")).Date And
               numeroRichiamo = rowTemp("ves_n_richiamo").ToString() And
               codiceVaccinazione = rowTemp("ves_vac_codice").ToString() Then

                Return False

            End If

        Next

        Return True

    End Function

    Private Sub EnableToolbar(disableAll As Boolean)

        ToolBar.Items.FromKeyButton("btn_Cnv").Enabled = Not disableAll AndAlso Not OnitLayout31.Busy
        ToolBar.Items.FromKeyButton("btn_Salva").Enabled = Not disableAll AndAlso OnitLayout31.Busy
        ToolBar.Items.FromKeyButton("btn_Annulla").Enabled = Not disableAll AndAlso OnitLayout31.Busy
        ToolBar.Items.FromKeyButton("btn_Esegui").Enabled = Not disableAll AndAlso VaccinazioniEditable AndAlso dt_vacProg.Rows.Count > 0
        ToolBar.Items.FromKeyButton("btn_Escludi").Enabled = Not disableAll AndAlso VaccinazioniEditable AndAlso dt_vacProg.Rows.Count > 0
        ToolBar.Items.FromKeyButton("btn_CertificatoVaccinale").Enabled = Not disableAll AndAlso VaccinazioniEditable
        ToolBar.Items.FromKeyButton("btn_InsAssociazione").Enabled = Not disableAll AndAlso VaccinazioniEditable
        ToolBar.Items.FromKeyButton("btn_AssLotti").Enabled = Not disableAll AndAlso VaccinazioniEditable AndAlso dt_vacProg.Rows.Count > 0
        ToolBar.Items.FromKeyButton("btn_InsLotto").Enabled = Not disableAll AndAlso VaccinazioniEditable AndAlso Settings.GESMAG AndAlso Settings.GESINSLOTTO
        ToolBar.Items.FromKeyButton("btn_AddBil").Enabled = Not disableAll AndAlso VaccinazioniEditable

    End Sub

    Private Function AddJSCheckDose() As String
        '--
        Dim strJStemp As New System.Text.StringBuilder()
        '--
        strJStemp.Append("var arMaxDose={")
        '--
        Dim ht_maxDose As New Hashtable()
        '--
        Dim dtVP As DataTable = RecuperaVacProgEseguite()
        '--
        For i As Integer = 0 To dtVP.Rows.Count - 1
            '--
            If ht_maxDose.ContainsKey(dtVP.Rows(i)("vpr_vac_codice")) Then
                If ht_maxDose.Item(dtVP.Rows(i)("vpr_vac_codice")) < dtVP.Rows(i)("vpr_n_richiamo") Then
                    ht_maxDose.Item(dtVP.Rows(i)("vpr_vac_codice")) = dtVP.Rows(i)("vpr_n_richiamo")
                End If
            Else
                ht_maxDose.Add(dtVP.Rows(i)("vpr_vac_codice"), dtVP.Rows(i)("vpr_n_richiamo"))
            End If
            '--
            OnVacUtility.ControllaVaccinazioneSostituta(dtVP.Rows(i)("vpr_vac_codice"), , , Enumerators.DipendenzaSostituta.Sinistra)
            '--
            If ht_maxDose.ContainsKey(dtVP.Rows(i)("vpr_vac_codice")) Then
                If ht_maxDose.Item(dtVP.Rows(i)("vpr_vac_codice")) < dtVP.Rows(i)("vpr_n_richiamo") Then
                    ht_maxDose.Item(dtVP.Rows(i)("vpr_vac_codice")) = dtVP.Rows(i)("vpr_n_richiamo")
                End If
            Else
                ht_maxDose.Add(dtVP.Rows(i)("vpr_vac_codice"), dtVP.Rows(i)("vpr_n_richiamo"))
            End If
            '--
            OnVacUtility.ControllaVaccinazioneSostituta(dtVP.Rows(i)("vpr_vac_codice"), , , Enumerators.DipendenzaSostituta.Sinistra)
            '--
            If ht_maxDose.ContainsKey(dtVP.Rows(i)("vpr_vac_codice")) Then
                If ht_maxDose.Item(dtVP.Rows(i)("vpr_vac_codice")) < dtVP.Rows(i)("vpr_n_richiamo") Then
                    ht_maxDose.Item(dtVP.Rows(i)("vpr_vac_codice")) = dtVP.Rows(i)("vpr_n_richiamo")
                End If
            Else
                ht_maxDose.Add(dtVP.Rows(i)("vpr_vac_codice"), dtVP.Rows(i)("vpr_n_richiamo"))
            End If
            '--
        Next
        '--
        If Not ht_maxDose Is Nothing AndAlso (ht_maxDose.Count > 0) Then
            '--
            Dim dic As IDictionaryEnumerator = ht_maxDose.GetEnumerator
            '--
            While (dic.MoveNext())
                strJStemp.Append("'" & dic.Key & "':" & dic.Value & ",")
            End While
            '--
            strJStemp.Remove(strJStemp.Length - 1, 1)
            '--
        End If
        '--
        strJStemp.Append("}" & vbCrLf)
        '--
        Return strJStemp.ToString()
        '--
    End Function

    Private Sub ShowPrintButtons()

        ToolBar.Items.FromKeyButton("btn_CertificatoVaccinale").Visible = Settings.VACPROG_CERTIFICATOVACCINALE

        If Settings.VACPROG_CERTIFICATOVACCINALE Then

            Dim listPrintButtons As New List(Of PrintButton)()

            ' Imposta la visibilità del pulsante di stampa del Certificato Vaccinale
            listPrintButtons.Add(New PrintButton(Constants.ReportName.CertificatoVaccinale, "btn_CertificatoVaccinale"))

            ShowToolbarPrintButtons(listPrintButtons, ToolBar)

        End If

    End Sub

    Private Sub ShowAlertMessagge(message As String)

        OnitLayout31.InsertRoutineJS(String.Format("alert('{0}');", HttpUtility.JavaScriptStringEncode(message)))

    End Sub

    Private Function IsImportoValido(importo As String) As Boolean

        If String.IsNullOrWhiteSpace(importo) Then Return True

        Dim numberFormatInfo As Globalization.NumberFormatInfo = OnVacUtility.GetNumberFormatInfo(True)

        importo = importo.Trim().Replace(",", numberFormatInfo.CurrencyDecimalSeparator).Replace(".", numberFormatInfo.CurrencyDecimalSeparator)

        Dim importoInserito As Double

        Return Double.TryParse(importo, importoInserito)

    End Function

    ' Restituisce gli indici delle righe che hanno il check spuntato
    Private Function RitornaCheck() As ArrayList

        Dim arrayCekkati As New ArrayList()

        For i As Integer = 0 To dg_vacProg.Items.Count - 1

            If DirectCast(dg_vacProg.Items.Item(i).FindControl("cb"), CheckBox).Checked Then
                arrayCekkati.Add(i)
            End If

        Next

        Return arrayCekkati

    End Function

    ' Controlla se la convocazione non ha più bilanci da effettuare
    '   TRUE => nessun bilancio da effettuare
    '   FALSE => tutti i bilanci sono eseguiti o esclusi
    Private Function CheckBilTuttiE() As Boolean

        'se tutte le righe sono escluse e non scadute o eseguite, 
        'ritorno TRUE per eliminare la convocazione 
        For i As Int16 = 0 To dt_bilProg.Rows.Count - 1

            If dt_bilProg.Rows(i)("bip_stato") = Constants.StatiBilancio.UNEXECUTED Then
                Return False
            End If

        Next

        Return True

    End Function

    'se la data è odierna, la sostituisce con la dicitura "oggi"
    Public Function FormattaDataVacProg(data As String) As String

        If data = Date.Now.ToString("dd/MM/yyyy") Then Return "Oggi"

        Return data

    End Function

    Private Function CreateVaccinazioneProgrammataFromDataRow(row As DataRow) As Entities.VaccinazioneProgrammata

        Dim vacProg As New Entities.VaccinazioneProgrammata()

        vacProg.DataConvocazione = DataConvocazione
        vacProg.CodicePaziente = OnVacUtility.Variabili.PazId
        vacProg.CodiceVaccinazione = row("vpr_vac_codice")
        If Not row("vpr_cic_codice") Is DBNull.Value Then
            vacProg.CodiceCiclo = row("vpr_cic_codice")
        End If
        If Not row("vpr_ass_codice") Is DBNull.Value Then
            vacProg.CodiceAssociazione = row("vpr_ass_codice")
        End If
        If Not row("vpr_n_seduta") Is DBNull.Value AndAlso Not String.IsNullOrEmpty(row("vpr_n_seduta")) Then
            vacProg.NumeroSeduta = Convert.ToInt16(row("vpr_n_seduta"))
        End If
        If Not row("vpr_n_richiamo") Is DBNull.Value AndAlso Not String.IsNullOrEmpty(row("vpr_n_richiamo")) Then
            vacProg.NumeroRichiamo = Convert.ToInt16(row("vpr_n_richiamo"))
        Else
            vacProg.NumeroRichiamo = 0
        End If

        Return vacProg

    End Function

    Private Function CreateVaccinazioneEsclusaFromDataRow(row As DataRow, valoreVisibilitaDatiVaccinaliPaziente As String) As Entities.VaccinazioneEsclusa

        Dim vacEsclusa As New Entities.VaccinazioneEsclusa()

        vacEsclusa.CodicePaziente = OnVacUtility.Variabili.PazId
        vacEsclusa.CodiceVaccinazione = row("vpr_vac_codice")
        If Not row("vex_data_visita") Is DBNull.Value Then
            vacEsclusa.DataVisita = row("vex_data_visita")
        End If
        If Not row("vex_data_scadenza") Is DBNull.Value Then
            vacEsclusa.DataScadenza = row("vex_data_scadenza")
        End If
        If Not row("vex_moe_codice") Is DBNull.Value Then
            vacEsclusa.CodiceMotivoEsclusione = row("vex_moe_codice")
        End If
        If Not row("vex_ope_codice") Is DBNull.Value Then
            vacEsclusa.CodiceOperatore = row("vex_ope_codice")
        End If
        If Not row("vex_id") Is DBNull.Value Then
            vacEsclusa.Id = row("vex_id")
        End If

        vacEsclusa.FlagVisibilita = valoreVisibilitaDatiVaccinaliPaziente

        If Not row.IsNull("vex_dose") Then
            vacEsclusa.NumeroDose = Int16.Parse(row("vex_dose").ToString(), Globalization.NumberStyles.None, Globalization.CultureInfo.InvariantCulture)
        End If

        If Not row.IsNull("vex_note") Then
            vacEsclusa.Note = row("vex_note").ToString()
        End If

        Return vacEsclusa

    End Function

#End Region

#End Region

#Region " Types "

    <Serializable()>
    Private Structure Cicli

        Public Codice

        Public Seduta

    End Structure

#End Region

#Region " Protected "

    Protected Function BindVisibilityScadenza(flagStatoEsecuzione As Object, dataScadenzaEsclusione As Object) As String

        If flagStatoEsecuzione.ToString() = StatoVaccinazione.ESCLUSA Then

            Return OnVacUtility.LegendaVaccinazioniBindVisibility(dataScadenzaEsclusione, Enumerators.LegendaVaccinazioniItemType.Esclusa)

        End If

        Return Boolean.FalseString

    End Function

#End Region

End Class
