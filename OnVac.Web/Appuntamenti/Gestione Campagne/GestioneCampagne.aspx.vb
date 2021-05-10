Imports System.Threading
Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager

Imports Onit.Shared.Manager.OnitProfile

Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Biz
Imports Onit.OnAssistnet.Web


Partial Class GestioneCampagne
    Inherits OnVac.Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Protected WithEvents OnitModalList1 As Onit.Controls.OnitModalList
    Protected WithEvents OnitModalList2 As Onit.Controls.OnitModalList
    Protected WithEvents ucStatiAnagrafici As Common.Controls.StatiAnagrafici
    Protected WithEvents ucMotivoEsc As MotivoEsc

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Properties "

    Private Property ReportPopUp_DataSource() As System.Data.DataSet
        Get
            Return Session("ReportFileName_PopUp_dataset")
        End Get
        Set(Value As System.Data.DataSet)
            Session("ReportFileName_PopUp_dataset") = Value
        End Set
    End Property

    Private Property dtPazienti() As DataTable
        Get
            Return Session("dtPazienti_Campagne")
        End Get
        Set(Value As DataTable)
            Session("dtPazienti_Campagne") = Value
        End Set
    End Property

    Public Property selTutti() As Boolean
        Get
            Return Session("selTutti")
        End Get
        Set(Value As Boolean)
            Session("selTutti") = Value
        End Set
    End Property

#End Region

#Region " Eventi Page "

    Private Sub Page_Load(sender As Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            Me.dtPazienti = New DataTable()

            Me.txtUltimaData.CssClass = "TextBox_Stringa_Disabilitato"
            Me.odpDataCNV.CssClass = "TextBox_Data_Disabilitato"

            ' Disabilitazione campo scelta consultorio in base a livello utente (da querystring)
            Dim livelloUtente As Enumerators.LivelloUtenteConvocazione = OnVacUtility.GetLivelloUtenteConvocazione(Me.Request.QueryString, True)

            Select Case livelloUtente

                Case Enumerators.LivelloUtenteConvocazione.Default, Enumerators.LivelloUtenteConvocazione.Undefined
                    Me.ucSelezioneConsultori.MostraPulsanteSelezione = False
                    Me.ucSelezioneConsultori.SelezioneMultipla = False

                Case Enumerators.LivelloUtenteConvocazione.Standard
                    Me.ucSelezioneConsultori.MostraPulsanteSelezione = True
                    Me.ucSelezioneConsultori.SelezioneMultipla = False

                Case Enumerators.LivelloUtenteConvocazione.Administrator
                    Me.ucSelezioneConsultori.MostraPulsanteSelezione = True
                    Me.ucSelezioneConsultori.SelezioneMultipla = True

            End Select

            ' Impostazione del consultorio corrente nel campo relativo al consultorio di prenotazione (sola lettura)
            Me.txtConsultorioPrenotazione.Text = String.Format("{0} [{1}]", OnVacUtility.Variabili.CNS.Descrizione, OnVacUtility.Variabili.CNS.Codice)

        End If

        Me.lblTitoloRicerca.Text = "RICERCA PAZIENTI DA CONVOCARE"

        If Me.dtPazienti.Rows.Count <> 0 Then
            Me.lblPazientiTrovati.Text = "Pazienti trovati: " + Me.dtPazienti.Rows.Count.ToString()
        Else
            Me.lblPazientiTrovati.Text = "Pazienti trovati"
        End If

        Select Case Request.Form.Item("__EVENTTARGET")

            Case "selPazienti"

                selTutti = CBool(Request.Form.Item("__EVENTARGUMENT"))

                SelezionaPazienti(True)

        End Select

    End Sub

    Private Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles MyBase.PreRender

        'valorizza il check dei pazienti [modifica 21/09/2006]
        If Me.ogPazienti.Items.Count > 0 Then Me.OnitLayout31.InsertRoutineJS("document.getElementById('chkSelTutti').checked = " & selTutti.ToString().ToLower() & ";")

    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub ToolBar_ButtonClicked(sender As System.Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case be.Button.Key

            Case "btnCerca"

                If String.IsNullOrEmpty(Me.ucSelezioneConsultori.GetConsultoriSelezionati(False)) Then

                    Me.OnitLayout31.InsertRoutineJS("alert('Selezionare almeno un centro vaccinale!');")
                    Return

                End If

                If ((Me.odpDataNascitaIniz.Text <> "" And Me.odpDataNascitaFin.Text <> "") Or
                    (Me.omlCategorieRischio.Codice <> "") Or
                    (Me.omlMalattia.Codice <> "")) Then

                    Me.dtPazienti = CercaPazienti()

                    If Not dtPazienti Is Nothing Then

                        Me.ogPazienti.DataSource = Me.dtPazienti
                        Me.ogPazienti.DataBind()

                        Me.ddlPager.Visible = Me.dtPazienti.Rows.Count > 100

                        If Me.dtPazienti.Rows.Count > 0 Then

                            Me.omlCicloCNV.Enabled = True
                            Me.omlAss.Enabled = Me.omlAssNonEs.Codice = String.Empty
                            Me.ogPazienti.Visible = True
                            Me.odpDataCNV.Enabled = True
                            Me.txtUltimaData.Enabled = True
                            Me.odpDataCNV.CssClass = "TextBox_Data_Obbligatorio"

                        Else

                            Me.omlCicloCNV.Enabled = False
                            Me.omlAss.Enabled = False
                            Me.ogPazienti.Visible = False
                            Me.odpDataCNV.Enabled = False
                            Me.txtUltimaData.Enabled = False
                            Me.odpDataCNV.CssClass = "TextBox_Data_Disabilitato"

                        End If

                        Me.lblPazientiTrovati.Text = "Pazienti trovati: " + Me.dtPazienti.Rows.Count.ToString()
                        Me.txtUltimaData.Text = RicavaUltimaData()

                    Else

                        Me.odpDataCNV.Enabled = False
                        Me.omlCicloCNV.Enabled = False
                        Me.omlAss.Enabled = False
                        Me.ddlPager.Visible = False
                        Me.ogPazienti.Visible = False
                        Me.txtUltimaData.Enabled = False
                        Me.odpDataCNV.CssClass = "TextBox_Data_Disabilitato"

                        Me.lblPazientiTrovati.Text = "Pazienti trovati"

                    End If

                    'selezione di tutti i pazienti [modifica 21/09/2006]
                    selTutti = True

                Else

                    Me.OnitLayout31.InsertRoutineJS("alert('Inserire gli estremi della data di nascita, la categoria a rischio o la malattia!');")

                End If

            Case "btnCrea"

                CreaCNV()

        End Select

    End Sub

#End Region

#Region " Eventi ModalList "

    Private Sub omlAssNonEs_SetUpFiletr(sender As Object) Handles omlAssNonEs.SetUpFiletr
        '--
        Me.SetTipoCNSFilterIfNeeded(Me.omlAssNonEs)
        '--
    End Sub

    Private Sub omlAss_SetUpFiletr(sender As Object) Handles omlAss.SetUpFiletr
        '--
        Me.SetTipoCNSFilterIfNeeded(Me.omlAss)
        '--
    End Sub

    Private Sub SetTipoCNSFilterIfNeeded(omlAss As Onit.Controls.OnitModalList)
        '--
        Dim tabella As String = "T_ANA_ASSOCIAZIONI"
        '--
        If Me.Settings.ASSOCIAZIONI_TIPO_CNS Then
            '--
            tabella = String.Format("{0} INNER JOIN T_ANA_ASSOCIAZIONI_TIPI_CNS ON ASS_CODICE = ATC_ASS_CODICE INNER JOIN T_ANA_CONSULTORI ON ATC_CNS_TIPO = CNS_TIPO AND CNS_CODICE = '{1}'", tabella, OnVacUtility.Variabili.CNS.Codice)
            '--
        End If
        '--
        omlAss.Tabella = tabella
        omlAss.CampoCodice = "ASS_CODICE"
        omlAss.CampoDescrizione = "ASS_DESCRIZIONE"
        omlAss.Filtro = "ASS_OBSOLETO = 'N' OR ASS_OBSOLETO IS NULL ORDER BY ASS_DESCRIZIONE"
        '--
    End Sub

    Private Sub omlAssNonEs_Change(Sender As System.Object, E As Onit.Controls.OnitModalList.ModalListaEventArgument) Handles omlAssNonEs.Change

        Dim assNonEsEmpty As Boolean = String.IsNullOrEmpty(Me.omlAssNonEs.Codice)

        Me.txtDose.ReadOnly = assNonEsEmpty
        Me.txtMesi.ReadOnly = assNonEsEmpty
        Me.txtAnni.ReadOnly = assNonEsEmpty

        Me.txtDose.CssClass = IIf(assNonEsEmpty, "textbox_numerico_disabilitato w100p", "textbox_numerico w100p")
        Me.txtMesi.CssClass = IIf(assNonEsEmpty, "textbox_numerico_disabilitato w100p", "textbox_numerico w100p")
        Me.txtAnni.CssClass = IIf(assNonEsEmpty, "textbox_numerico_disabilitato w100p", "textbox_numerico w100p")

        Me.omlAss.Codice = Me.omlAssNonEs.Codice
        Me.omlAss.Descrizione = Me.omlAssNonEs.Descrizione

        Me.btnMotEsc.Enabled = Not assNonEsEmpty

        If assNonEsEmpty Then

            Me.txtDose.Text = String.Empty
            Me.txtMesi.Text = String.Empty
            Me.txtAnni.Text = String.Empty

            Me.txtMotEsc.Text = String.Empty

        End If

    End Sub

#End Region

#Region " Eventi OnitGrid "

    Private Sub ogPazienti_ChangePageEvnt(sender As Object, e As Onit.Controls.OnitGrid.ChangePageEventArgs) Handles ogPazienti.ChangePageEvnt

        'aggiornamento della selezione dei pazienti [modifica 21/09/2006]
        SelezionaPazienti(False)

        Me.ogPazienti.CurrentPageIndex = e.new_page
        Me.ogPazienti.DataSource = Me.dtPazienti
        Me.ogPazienti.DataBind()
        Me.ogPazienti.SelectedIndex = -1

    End Sub

#End Region

#Region " Eventi Pulsanti "

    Private Sub btnMotEsc_Click(sender As Object, e As System.EventArgs) Handles btnMotEsc.Click

        Me.ucMotivoEsc.ModaleName = "ofmMotivoEsc"
        Me.ucMotivoEsc.MotiviSelezionati = Me.txtMotEsc.Text
        Me.ucMotivoEsc.LoadModale()

        Me.ofmMotivoEsc.VisibileMD = True

        Me.OnitLayout31.Busy = True

    End Sub

#End Region

#Region " Eventi UserControl "

    Private Sub MotivoEsc_InviaCodMotEsc(arrMotEscCod As System.Collections.ArrayList, arrMotEscDesc As System.Collections.ArrayList) Handles ucMotivoEsc.InviaCodMotEsc

        Me.ofmMotivoEsc.VisibileMD = False

        If Not arrMotEscCod Is Nothing AndAlso arrMotEscCod.Count > 0 Then

            Me.txtMotEsc.Text = String.Join(",", arrMotEscCod.ToArray())

        Else

            Me.txtMotEsc.Text = String.Empty

        End If

        Me.OnitLayout31.Busy = False

    End Sub

    Private Sub ucMotivoEsc_RiabilitaLayout() Handles ucMotivoEsc.RiabilitaLayout

        Me.ofmMotivoEsc.VisibileMD = False

        Me.OnitLayout31.Busy = False

    End Sub

    Private Sub ucSelezioneConsultori_OnSelectionError(errorMessage As String) Handles ucSelezioneConsultori.OnSelectionError

        Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(errorMessage, "errMsgSelezioneCns", False, False))

    End Sub

#End Region

#Region " Private Methods "

    Private Function CercaPazienti() As DataTable
        '--
        Dim dt As DataTable = Nothing
        '--
        Using dam As IDAM = OnVacUtility.OpenDam()
            '--
            Dim queryNonEseguita As String = String.Empty
            Dim queryEseguitaData As String = String.Empty
            Dim queryEseguitaDose As String = String.Empty
            Dim queryMotiviEsclusione As String = String.Empty
            '--
            Dim delUnion As Boolean = True
            Dim delParams As Boolean = True
            '--
            If Not String.IsNullOrEmpty(Me.omlAssNonEs.Codice) Then
                '--
                delUnion = False
                delParams = False
                '--
                If Not String.IsNullOrEmpty(Me.txtDose.Text) AndAlso Short.Parse(Me.txtDose.Text) > 1 Then
                    '--
                    dam.QB.NewQuery()
                    dam.QB.AddSelectFields("1")
                    dam.QB.AddTables("T_VAC_ESEGUITE es2")
                    dam.QB.AddWhereCondition("es2.VES_PAZ_CODICE", Comparatori.Uguale, "es1.VES_PAZ_CODICE", DataTypes.Replace)
                    dam.QB.AddWhereCondition("es2.VES_VAC_CODICE", Comparatori.Uguale, "es1.VES_VAC_CODICE", DataTypes.Replace)
                    dam.QB.AddWhereCondition("es2.VES_N_RICHIAMO", Comparatori.Maggiore, "es1.VES_N_RICHIAMO", DataTypes.Replace)
                    '--
                    Dim queryDoseEseguitaMaggiore As String = dam.QB.GetSelect()
                    '--
                    dam.QB.NewQuery(False, False)
                    dam.QB.AddSelectFields("1")
                    dam.QB.AddTables("T_ANA_ASSOCIAZIONI, T_ANA_LINK_ASS_VACCINAZIONI, T_VAC_ESEGUITE es1")
                    dam.QB.AddWhereCondition("ASS_CODICE", Comparatori.Uguale, "VAL_ASS_CODICE", DataTypes.Join)
                    dam.QB.AddWhereCondition("VAL_VAC_CODICE", Comparatori.Uguale, "es1.VES_VAC_CODICE", DataTypes.Join)
                    dam.QB.AddWhereCondition("ASS_CODICE", Comparatori.Uguale, omlAssNonEs.Codice, DataTypes.Stringa)
                    dam.QB.AddWhereCondition("es1.VES_N_RICHIAMO", Comparatori.Uguale, Short.Parse(txtDose.Text) - 1, DataTypes.Numero)
                    dam.QB.AddWhereCondition("", Comparatori.NotExist, "(" & queryDoseEseguitaMaggiore & ")", DataTypes.Replace)
                    dam.QB.AddWhereCondition("es1.VES_PAZ_CODICE", Comparatori.Uguale, "PAZ_CODICE", DataTypes.Replace)
                    '--
                    If Not String.IsNullOrEmpty(Me.txtMesi.Text) OrElse Not String.IsNullOrEmpty(Me.txtAnni.Text) Then
                        '--
                        dam.QB.AddWhereCondition("es1.VES_DATA_EFFETTUAZIONE", Comparatori.MinoreUguale, CalcolaDataEffettuazione(), DataTypes.Data)
                        '--
                    End If
                    '--
                    queryEseguitaDose = dam.QB.GetSelect()
                    '--
                Else
                    '--
                    dam.QB.NewQuery()
                    dam.QB.AddSelectFields("1")
                    dam.QB.AddTables("T_ANA_ASSOCIAZIONI, T_ANA_LINK_ASS_VACCINAZIONI, T_VAC_ESEGUITE")
                    dam.QB.AddWhereCondition("ASS_CODICE", Comparatori.Uguale, "VAL_ASS_CODICE", DataTypes.Join)
                    dam.QB.AddWhereCondition("VAL_VAC_CODICE", Comparatori.Uguale, "VES_VAC_CODICE", DataTypes.Join)
                    dam.QB.AddWhereCondition("ASS_CODICE", Comparatori.Uguale, omlAssNonEs.Codice, DataTypes.Stringa)
                    dam.QB.AddWhereCondition("VES_PAZ_CODICE", Comparatori.Uguale, "PAZ_CODICE", DataTypes.Replace)
                    '--
                    queryNonEseguita = dam.QB.GetSelect()
                    '--
                    If Not String.IsNullOrEmpty(Me.txtMesi.Text) OrElse Not String.IsNullOrEmpty(Me.txtAnni.Text) Then
                        '--
                        dam.QB.NewQuery(False, False)
                        dam.QB.AddSelectFields("1")
                        dam.QB.AddTables("T_VAC_ESEGUITE es2")
                        dam.QB.AddWhereCondition("es2.VES_PAZ_CODICE", Comparatori.Uguale, "es1.VES_PAZ_CODICE", DataTypes.Replace)
                        dam.QB.AddWhereCondition("es2.VES_VAC_CODICE", Comparatori.Uguale, "es1.VES_VAC_CODICE", DataTypes.Replace)
                        dam.QB.AddWhereCondition("es2.VES_DATA_EFFETTUAZIONE", Comparatori.Maggiore, "es1.VES_DATA_EFFETTUAZIONE", DataTypes.Replace)
                        '--
                        Dim queryDataEsecuzioneMaggiore As String = dam.QB.GetSelect()
                        '--
                        dam.QB.NewQuery(False, False)
                        dam.QB.AddSelectFields("1")
                        dam.QB.AddTables("T_ANA_ASSOCIAZIONI, T_ANA_LINK_ASS_VACCINAZIONI, T_VAC_ESEGUITE es1")
                        dam.QB.AddWhereCondition("ASS_CODICE", Comparatori.Uguale, "VAL_ASS_CODICE", DataTypes.Join)
                        dam.QB.AddWhereCondition("VAL_VAC_CODICE", Comparatori.Uguale, "es1.VES_VAC_CODICE", DataTypes.Join)
                        dam.QB.AddWhereCondition("ASS_CODICE", Comparatori.Uguale, omlAssNonEs.Codice, DataTypes.Stringa)
                        dam.QB.AddWhereCondition("es1.VES_PAZ_CODICE", Comparatori.Uguale, "PAZ_CODICE", DataTypes.Replace)
                        dam.QB.AddWhereCondition("es1.VES_DATA_EFFETTUAZIONE", Comparatori.MinoreUguale, CalcolaDataEffettuazione(), DataTypes.Data)
                        dam.QB.AddWhereCondition("", Comparatori.NotExist, "(" & queryDataEsecuzioneMaggiore & ")", DataTypes.Replace)
                        '--
                        queryEseguitaData = dam.QB.GetSelect()
                        '--
                    End If
                    '--
                End If
                '--
                If Not String.IsNullOrEmpty(Me.txtMotEsc.Text) Then
                    '--
                    dam.QB.NewQuery(delUnion, delParams)
                    '--
                    dam.QB.AddSelectFields("1")
                    dam.QB.AddTables("T_ANA_ASSOCIAZIONI, T_ANA_LINK_ASS_VACCINAZIONI, T_VAC_ESCLUSE")
                    '--
                    ' Filtro codici motivi esclusione
                    Dim codiceMotiviEsclusioneCondition As New System.Text.StringBuilder()
                    '--
                    For Each codiceMotivoEsclusione As String In Me.txtMotEsc.Text.Split(",")
                        codiceMotiviEsclusioneCondition.AppendFormat("{0},", dam.QB.AddCustomParam(codiceMotivoEsclusione))
                    Next
                    '--
                    If codiceMotiviEsclusioneCondition.Length > 0 Then
                        codiceMotiviEsclusioneCondition.Remove(codiceMotiviEsclusioneCondition.Length - 1, 1)
                        dam.QB.AddWhereCondition("VEX_MOE_CODICE", Comparatori.In, codiceMotiviEsclusioneCondition.ToString(), DataTypes.Replace)
                    End If
                    '--
                    dam.QB.AddWhereCondition("ASS_CODICE", Comparatori.Uguale, "VAL_ASS_CODICE", DataTypes.Join)
                    dam.QB.AddWhereCondition("VAL_VAC_CODICE", Comparatori.Uguale, "VEX_VAC_CODICE", DataTypes.Join)
                    dam.QB.AddWhereCondition("ASS_CODICE", Comparatori.Uguale, Me.omlAssNonEs.Codice, DataTypes.Stringa)
                    dam.QB.AddWhereCondition("VEX_PAZ_CODICE", Comparatori.Uguale, "PAZ_CODICE", DataTypes.Replace)
                    dam.QB.OpenParanthesis()
                    dam.QB.AddWhereCondition("VEX_DATA_SCADENZA", Comparatori.Is, "Null", DataTypes.Replace)
                    dam.QB.AddWhereCondition("VEX_DATA_SCADENZA", Comparatori.Maggiore, DateTime.Today, DataTypes.Data, "or")
                    dam.QB.CloseParanthesis()
                    '--
                    queryMotiviEsclusione = dam.QB.GetSelect()
                    '--
                End If
                '--
            End If
            '--           
            dam.QB.NewQuery(delUnion, delParams)
            dam.QB.IsDistinct = True
            dam.QB.AddSelectFields("'S' AS Sel, paz_codice, paz_cognome, paz_nome, paz_data_nascita, comRes.com_descrizione as com_descrizione_residenza, paz_indirizzo_residenza")
            dam.QB.AddSelectFields("comDom.com_descrizione as com_descrizione_domicilio, paz_indirizzo_domicilio, paz_cns_codice")
            dam.QB.AddTables("t_paz_pazienti", "t_ana_comuni comRes", "t_ana_comuni comDom")
            dam.QB.AddWhereCondition("paz_com_codice_residenza", Comparatori.Uguale, "comRes.com_codice", DataTypes.OutJoinLeft)
            dam.QB.AddWhereCondition("paz_com_codice_domicilio", Comparatori.Uguale, "comDom.com_codice", DataTypes.OutJoinLeft)
            '--
            Dim consultoriSelezionati As New System.Text.StringBuilder()
            '--
            Dim listCns As List(Of String) = Me.ucSelezioneConsultori.GetConsultoriSelezionati()
            '--
            If Not listCns Is Nothing And listCns.Count > 0 Then
                '--
                For Each codiceCns As String In listCns
                    consultoriSelezionati.AppendFormat("{0},", dam.QB.AddCustomParam(codiceCns))
                Next
                '--
            End If
            '--
            If consultoriSelezionati.Length > 0 Then
                '--
                consultoriSelezionati.Remove(consultoriSelezionati.Length - 1, 1)
                dam.QB.AddWhereCondition("paz_cns_codice", Comparatori.In, consultoriSelezionati.ToString(), DataTypes.Replace)
                '--
            End If
            '--
            If Not String.IsNullOrEmpty(Me.odpDataNascitaIniz.Text) AndAlso Not String.IsNullOrEmpty(Me.odpDataNascitaFin.Text) Then
                dam.QB.AddWhereCondition("paz_data_nascita", Comparatori.MaggioreUguale, Me.odpDataNascitaIniz.Text, DataTypes.Data)
                dam.QB.AddWhereCondition("paz_data_nascita", Comparatori.MinoreUguale, Me.odpDataNascitaFin.Text, DataTypes.Data)
            End If
            '--    
            If Not String.IsNullOrEmpty(Me.ddlSesso.SelectedValue) Then
                dam.QB.AddWhereCondition("paz_sesso", Comparatori.Uguale, Me.ddlSesso.SelectedValue, DataTypes.Stringa)
            End If
            '--
            If Not String.IsNullOrEmpty(Me.omlCategorieRischio.Codice) Then
                dam.QB.AddWhereCondition("paz_rsc_codice", Comparatori.Uguale, Me.omlCategorieRischio.Codice, DataTypes.Stringa)
            End If
            '--  
            If Not String.IsNullOrEmpty(Me.omlMalattia.Codice) Then
                dam.QB.AddTables("t_paz_malattie")
                dam.QB.AddWhereCondition("PMA_PAZ_CODICE", Comparatori.Uguale, "PAZ_CODICE", DataTypes.Join)
                dam.QB.AddWhereCondition("PMA_MAL_CODICE", Comparatori.Uguale, Me.omlMalattia.Codice, DataTypes.Stringa)
            End If
            '--  
            If Not String.IsNullOrEmpty(Me.fmComune.Codice) Then
                dam.QB.AddWhereCondition(dam.QB.FC.IsNull("PAZ_COM_CODICE_DOMICILIO", "PAZ_COM_CODICE_RESIDENZA", DataTypes.Replace), Comparatori.Uguale, Me.fmComune.Codice, DataTypes.Stringa)
            End If
            '-- 
            If Not String.IsNullOrEmpty(queryEseguitaDose) Then
                '-- 
                dam.QB.AddWhereCondition("", Comparatori.Exist, "(" & queryEseguitaDose & ")", DataTypes.Replace)
                '-- 
            ElseIf Not String.IsNullOrEmpty(queryNonEseguita) Then
                '-- 
                If Not String.IsNullOrEmpty(queryEseguitaData) Then
                    dam.QB.OpenParanthesis()
                End If
                '-- 
                dam.QB.AddWhereCondition("", Comparatori.NotExist, "(" & queryNonEseguita & ")", DataTypes.Replace)
                '-- 
                If Not String.IsNullOrEmpty(queryEseguitaData) Then
                    dam.QB.AddWhereCondition("", Comparatori.Exist, "(" & queryEseguitaData & ")", DataTypes.Replace, "OR")
                    dam.QB.CloseParanthesis()
                End If
                '-- 
            End If
            '-- 
            If Not String.IsNullOrEmpty(queryMotiviEsclusione) Then
                dam.QB.AddWhereCondition("", Comparatori.NotExist, "(" & queryMotiviEsclusione & ")", DataTypes.Replace)
            End If
            '-- 
            Dim statiAnagrafici As String = Me.ucStatiAnagrafici.GetSelectedValuesForQuery()
            '-- 
            If Not String.IsNullOrEmpty(statiAnagrafici) Then
                dam.QB.AddWhereCondition("paz_stato_anagrafico", Comparatori.In, statiAnagrafici, DataTypes.Replace)
            End If
            '-- 
            'i pazienti ricercati non devono essere deceduti [modifica 20/09/2006]
            '-- 
            dam.QB.AddWhereCondition("paz_data_decesso", Comparatori.Is, "null", DataTypes.Replace)
            '-- 
            dam.QB.AddOrderByFields("paz_cognome", "paz_nome", "paz_data_nascita")
            '-- 
            dt = New DataTable()
            '-- 
            dam.BuildDataTable(dt)
            '-- 
        End Using
        '-- 
        'chiave primaria sul datatable
        '-- 
        If dt.Rows.Count > 0 Then
            dt.PrimaryKey = New DataColumn() {dt.Columns("paz_codice")}
        End If
        '-- 
        Me.ogPazienti.CurrentPageIndex = 0
        '-- 
        Return dt
        '-- 
    End Function

    Private Function CalcolaDataEffettuazione() As DateTime
        '--
        Dim data As DateTime = DateTime.Now
        '--
        If Me.txtAnni.Text <> String.Empty Then
            '--
            data = data.AddYears(-1 * Short.Parse(Me.txtAnni.Text))
            '--
        End If
        '--
        If Me.txtMesi.Text <> String.Empty Then
            '--
            data = data.AddMonths(-1 * Short.Parse(Me.txtMesi.Text))
            '--
        End If
        '--
        Return data
        '--
    End Function

    ' Prende la dtPazienti e pone nel campo "Sel" di ogni paziente una S o una N in base allo stato del check (paziente selezionato)
    Private Sub SelezionaPazienti(tutti As Boolean)

        If Not tutti Then

            For count As Integer = 0 To Me.ogPazienti.Items.Count - 1
                'verifica la valorizzazione della selezione sul singolo paziente
                Me.dtPazienti.Rows.Find(Me.ogPazienti.Items(count).Cells(0).Text).Item("Sel") = IIf(CType(Me.ogPazienti.Items(count).Cells(1).Controls(1), CheckBox).Checked, "S", "N")
            Next

        Else

            For count As Integer = 0 To Me.dtPazienti.Rows.Count - 1
                'imposta la selezione per tutti i pazienti
                Me.dtPazienti.Rows(count).Item("Sel") = IIf(Me.selTutti, "S", "N")
            Next

            Me.ogPazienti.DataSource = Me.dtPazienti
            Me.ogPazienti.DataBind()

        End If

    End Sub

    ' Lettura della data dell'ultima campagna effettuata nel consultorio
    Private Function RicavaUltimaData() As String

        Dim dataCnvCampagna As Date

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            dataCnvCampagna = genericProvider.Consultori.SelectDataUltimaConvocazioneCampagna(OnVacUtility.Variabili.CNS.Codice)

        End Using

        If dataCnvCampagna = Date.MinValue Then
            Return String.Empty
        End If

        Return String.Format("{0:dd/MM/yyyy}", dataCnvCampagna)

    End Function

#Region " Schedulazione Creazione convocazioni in campagna "

    Private Sub CreaCNV()

        ' --- Data convocazione campagna --- '
        Dim dataCnvCampagna As Date = odpDataCNV.Data

        ' Se non è stata selezionata una data, non parto con la creazione della campagna
        If dataCnvCampagna = Date.MinValue Then
            OnitLayout31.InsertRoutineJS("alert('Nessuna data di convocazione selezionata. La creazione della campagna non e\' stata avviata.');")
            Exit Sub
        End If

        ' --- Ciclo da associare --- '
        ' Se è selezionato un ciclo da associare ai pazienti, assegno il codice altrimenti lascio la stringa vuota
        Dim cicloDaAssociare As String = String.Empty
        If omlCicloCNV.Descrizione <> String.Empty And Me.omlCicloCNV.Codice <> String.Empty Then
            cicloDaAssociare = omlCicloCNV.Codice
        End If

        ' --- Selezione dei pazienti --- '
        SelezionaPazienti(False)

        Dim ds As OnVacAnomalieCampagna = New OnVacAnomalieCampagna()

        Dim dtAnomalieCampagna As DataTable = ds.anomalieCampagna

        ' --- Filtro i pazienti selezionati --- '
        Dim rowsPazSel As DataRow() = dtPazienti.Select("Sel = 'S'")

        ' Se non è stato selezionato nulla, non parto con la creazione della campagna
        If rowsPazSel.Length = 0 Then
            OnitLayout31.InsertRoutineJS("alert('Nessun paziente selezionato. La creazione della campagna non e\' stata avviata.');")
            Exit Sub
        End If

        ' --------- Schedulazione Processi --------- '

        Const CREACAMPAGNAVACCINALE_PROCID As Integer = 7
        Const ASSOCIACICLI_PROCID As Integer = 1

        Dim returnedMessage As New Text.StringBuilder()

        Using genericProvider = New DbGenericProvider(OnVacContext.Connection)

            Dim creaCampagnaVaccinaleBizBatch As New BizBatch()
            Dim associaCicliBizBatch As New BizBatch()

            Dim jobId As Integer

            Dim hasError As Boolean = False

            Using worker As New wsBatch.wsBatch()

                ' Info per connessione a database
                Dim portConnections(0) As wsBatch.PortConnection
                portConnections(0) = New wsBatch.PortConnection()
                portConnections(0).Name = OnVacContext.AppId

                ' --- Schedulazione Processo Creazione Campagna Vaccinale --- '
                creaCampagnaVaccinaleBizBatch.addSelectionParameters("Consultorio", OnVacUtility.Variabili.CNS.Descrizione)
                creaCampagnaVaccinaleBizBatch.addSelectionParameters("Data di Nascita", String.Format("dal {0} al {1}", Me.odpDataNascitaIniz.Data.ToShortDateString(), Me.odpDataNascitaFin.Data.ToShortDateString()))
                creaCampagnaVaccinaleBizBatch.addSelectionParameters("Sesso", Me.ddlSesso.SelectedItem.Text)
                creaCampagnaVaccinaleBizBatch.addSelectionParameters("Malattia", Me.omlMalattia.Descrizione)
                creaCampagnaVaccinaleBizBatch.addSelectionParameters("Categoria a Rischio", Me.omlCategorieRischio.Descrizione)
                creaCampagnaVaccinaleBizBatch.addSelectionParameters("Associazione non eseguita", Me.omlAssNonEs.Descrizione)
                creaCampagnaVaccinaleBizBatch.addSelectionParameters("Dose non eseguita", Me.txtDose.Text)
                creaCampagnaVaccinaleBizBatch.addSelectionParameters("Mesi ultima esecuzione", Me.txtMesi.Text)
                creaCampagnaVaccinaleBizBatch.addSelectionParameters("Anni ultima esecuzione", Me.txtAnni.Text)
                creaCampagnaVaccinaleBizBatch.addSelectionParameters("Motivi di esclusione", Me.ucMotivoEsc.GetSelectedDescriptions())
                creaCampagnaVaccinaleBizBatch.addSelectionParameters("Stato anagrafico", Me.ucStatiAnagrafici.GetSelectedDescriptions())
                creaCampagnaVaccinaleBizBatch.addSelectionParameters("Associazione da programmare", Me.omlAss.Descrizione)
                creaCampagnaVaccinaleBizBatch.addSelectionParameters("Data convocazione", Me.odpDataCNV.Text)
                creaCampagnaVaccinaleBizBatch.addSelectionParameters("Forza convocazione", IIf(Me.chkForzaCNV.Checked, "SI", "NO"))
                creaCampagnaVaccinaleBizBatch.addSelectionParameters("Includi ritardatari", IIf(Me.chkIncRit.Checked, "SI", "NO"))
                creaCampagnaVaccinaleBizBatch.addSelectionParameters("Utente", OnVacContext.UserDescription)

                ' Dati del job
                Dim creaCampagnaVaccinaleJobData As New wsBatch.JobInputData()

                creaCampagnaVaccinaleJobData.ProcedureId = CREACAMPAGNAVACCINALE_PROCID
                creaCampagnaVaccinaleJobData.Paused = True
                creaCampagnaVaccinaleJobData.TotalItems = rowsPazSel.Length
                creaCampagnaVaccinaleJobData.Input = New wsBatch.InputPort()
                creaCampagnaVaccinaleJobData.Input.ApplicationId = OnVacContext.AppId
                creaCampagnaVaccinaleJobData.Input.AziendaCodice = OnVacContext.Azienda
                creaCampagnaVaccinaleJobData.Input.UserId = OnVacContext.UserId

                ' Connessione al db           
                creaCampagnaVaccinaleJobData.Input.PortConnections = portConnections

                ' Creazione del job
                Try
                    jobId = worker.CreateJob(creaCampagnaVaccinaleJobData)
                Catch ex As Exception
                    Common.Utility.EventLogHelper.EventLogWrite(ex, OnVacContext.AppId)
                    jobId = Nothing
                End Try

                If jobId <> Nothing AndAlso jobId > 0 Then

                    ' Procedura di riempimento della tabella T_PAZ_ELABORAZIONI con i pazienti da processare
                    creaCampagnaVaccinaleBizBatch.jobId = jobId
                    creaCampagnaVaccinaleBizBatch.procId = CREACAMPAGNAVACCINALE_PROCID
                    creaCampagnaVaccinaleBizBatch.uteId = OnVacContext.UserId

                    creaCampagnaVaccinaleBizBatch.addParameter(OnVacUtility.Variabili.CNS.Codice)
                    creaCampagnaVaccinaleBizBatch.addParameter(odpDataCNV.Text)
                    creaCampagnaVaccinaleBizBatch.addParameter(IIf(chkForzaCNV.Checked, "S", "N"))
                    creaCampagnaVaccinaleBizBatch.addParameter(IIf(chkIncRit.Checked, "S", "N"))
                    creaCampagnaVaccinaleBizBatch.addParameter(omlAss.Codice)
                    creaCampagnaVaccinaleBizBatch.addParameter(OnVacContext.CodiceUslCorrente)

                    For i As Integer = 0 To rowsPazSel.Length - 1
                        Dim codicePaziente As Integer = rowsPazSel(i)("paz_codice")
                        creaCampagnaVaccinaleBizBatch.addPatient(codicePaziente)
                    Next

                    Try
                        creaCampagnaVaccinaleBizBatch.setProvider(genericProvider)
                        creaCampagnaVaccinaleBizBatch.fillTable()
                        creaCampagnaVaccinaleBizBatch.storeParameters()
                    Catch ex As Exception
                        Common.Utility.EventLogHelper.EventLogWrite(ex, OnVacContext.AppId)
                        returnedMessage.Append("Problemi di comunicazione con il database. Processo di creazione campagna vaccinale NON avviato")
                        hasError = True
                    End Try

                Else

                    returnedMessage.Append("Problemi di comunicazione con il servizio. Processo di creazione campagna vaccinale NON avviato")
                    hasError = True

                End If

                If Not hasError Then
                    ' Richiesta del servizio al web service
                    worker.ResumeJob(jobId)
                    returnedMessage.Append("Processo di creazione campagna vaccinale avviato")
                End If


                ' --- Schedulazione Processo Associa Cicli --- '
                If Me.omlCicloCNV.Codice <> Nothing Then

                    hasError = False

                    associaCicliBizBatch.addSelectionParameters("Consultorio", OnVacUtility.Variabili.CNS.Descrizione)
                    associaCicliBizBatch.addSelectionParameters("Data di Nascita", String.Format("dal {0} al {1}", Me.odpDataNascitaIniz.Data.ToShortDateString(), Me.odpDataNascitaFin.Data.ToShortDateString()))
                    associaCicliBizBatch.addSelectionParameters("Sesso", Me.ddlSesso.SelectedValue)
                    associaCicliBizBatch.addSelectionParameters("Malattia", Me.omlMalattia.Descrizione)
                    associaCicliBizBatch.addSelectionParameters("Categoria a Rischio", Me.omlCategorieRischio.Descrizione)
                    associaCicliBizBatch.addSelectionParameters("Ciclo", Me.omlCicloCNV.Descrizione)
                    associaCicliBizBatch.addSelectionParameters("Utente", OnVacContext.UserDescription)
                    associaCicliBizBatch.addSelectionParameters("Stato anagrafico", Me.ucStatiAnagrafici.GetSelectedDescriptions())

                    ' Dati del job
                    Dim associaCicliJobData As New wsBatch.JobInputData()

                    associaCicliJobData.ProcedureId = ASSOCIACICLI_PROCID
                    associaCicliJobData.Paused = True
                    associaCicliJobData.TotalItems = rowsPazSel.Length
                    associaCicliJobData.Input = New wsBatch.InputPort()
                    associaCicliJobData.Input.ApplicationId = OnVacContext.AppId
                    associaCicliJobData.Input.UserId = OnVacContext.UserId
                    associaCicliJobData.Input.AziendaCodice = OnVacContext.Azienda

                    ' Connessione al db
                    associaCicliJobData.Input.PortConnections = portConnections

                    ' Creazione del job
                    Try
                        jobId = worker.CreateJob(associaCicliJobData)
                    Catch ex As Exception
                        Common.Utility.EventLogHelper.EventLogWrite(ex, OnVacContext.AppId)
                        jobId = Nothing
                    End Try

                    If jobId <> Nothing AndAlso jobId > 0 Then

                        ' Procedura di riempimento della tabella T_PAZ_ELABORAZIONI con i pazienti da processare
                        associaCicliBizBatch.jobId = jobId
                        associaCicliBizBatch.procId = ASSOCIACICLI_PROCID
                        associaCicliBizBatch.uteId = OnVacContext.UserId

                        associaCicliBizBatch.addParameter(Me.omlCicloCNV.Codice)

                        For i As Integer = 0 To rowsPazSel.Length - 1
                            Dim codicePaziente As Integer = rowsPazSel(i)("paz_codice")
                            associaCicliBizBatch.addPatient(codicePaziente)
                        Next

                        Try
                            associaCicliBizBatch.setProvider(genericProvider)
                            associaCicliBizBatch.fillTable()
                            associaCicliBizBatch.storeParameters()
                        Catch ex As Exception
                            Common.Utility.EventLogHelper.EventLogWrite(ex, OnVacContext.AppId)
                            If returnedMessage.Length > 0 Then returnedMessage.Append("\n\n")
                            returnedMessage.Append("Problemi di comunicazione con il database. Processo di associazione ciclo NON avviato")
                            hasError = True
                        End Try

                    Else

                        If returnedMessage.Length > 0 Then returnedMessage.Append("\n\n")
                        returnedMessage.Append("Problemi di comunicazione con il servizio. Processo di associazione ciclo NON avviato")
                        hasError = True

                    End If

                    If Not hasError Then
                        ' Richiesta del servizio al web service
                        worker.ResumeJob(jobId)
                        If returnedMessage.Length > 0 Then returnedMessage.Append("\n\n")
                        returnedMessage.Append("Processo di associazione ciclo avviato")
                    End If

                End If

            End Using

        End Using
        ' ------------------------------------------ '

        ' Messaggio all'utente
        OnitLayout31.InsertRoutineJS(String.Format("alert('{0}');", returnedMessage))

    End Sub

#End Region

#End Region

End Class
