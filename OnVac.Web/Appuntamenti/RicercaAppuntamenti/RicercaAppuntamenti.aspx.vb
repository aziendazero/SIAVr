Imports System.Collections.Generic
Imports System.Text
Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.Biz
Imports Onit.OnAssistnet.OnVac.CalcoloAppuntamenti
Imports Onit.OnAssistnet.OnVac.CalcoloAppuntamenti.Timing
Imports Onit.OnAssistnet.OnVac.DAL


Partial Class OnVac_RicercaAppuntamenti
    Inherits Common.PageBase

#Region " Constants "

    Private Const DATA_APPUNTAMENTO_NON_IMPOSTATA As String = "(Data appuntamento non impostata)"

#End Region

#Region " Private/Protected Variables "

    Private appuntamentoSpostato As Boolean = False

    Protected DurataAppuntamentoCNV As Int16 = 0
    Protected disabilitaElimPrenotazione As Boolean = False
    Protected disabilitaElimSoloBilancio As Boolean = False

#End Region

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

#Region " Classi "

    Private Class CercaPeriodiLiberiResult
        Public GiorniDisponibili As List(Of DateTime)
        Public Appuntamenti As DataTable
        Public OrariValidi As DataTable
        Public AppuntamentiTotali As List(Of AppuntamentoPrenotato)
    End Class

    <Serializable>
    Private Class AppuntamentoPrenotato
        Public Property CNV_DATA_APPUNTAMENTO As DateTime
        Public Property CNV_FINE_APPUNTAMENTO As DateTime
        Public Property CNV_DURATA_APPUNTAMENTO As Decimal
        Public Property PAZ_SOLO_BILANCIO As Decimal
        Public Property VACCINAZIONI As String
        Public Property AMB_CODICE As Decimal
        Public Property AMB_DES As String
    End Class

    <Serializable>
    Private Class AperturaAmbulatorio
        Public Property AM_INIZIO As DateTime
        Public Property AM_FINE As DateTime
        Public Property PM_INIZIO As DateTime
        Public Property PM_FINE As DateTime
        Public Property GIORNO As String
    End Class

    <Serializable>
    Private Class Disponibilita
        Public Property Data As DateTime
        Public Property Ora As DateTime
        Public Property ambCodice As Integer
        Public Property Amb As String
        Public Property Vaccinazioni As String
    End Class

    <Serializable>
    Private Class GiorniConAppuntamenti

#Region " Properties "

        Private Property ListGiorni As List(Of DateTime)

        Public Property IndiceGiornoSelezionato As Integer

        Public ReadOnly Property DataSelezionata As String
            Get
                If Me.ListGiorni.IsNullOrEmpty() OrElse Me.IndiceGiornoSelezionato = -1 Then
                    Return String.Empty
                End If
                Return Me.ListGiorni(Me.IndiceGiornoSelezionato).ToShortDateString()
            End Get
        End Property

        Public ReadOnly Property HasValue As Boolean
            Get
                Return Not Me.ListGiorni.IsNullOrEmpty()
            End Get
        End Property

        Public ReadOnly Property Count As Integer
            Get
                Return Me.ListGiorni.Count
            End Get
        End Property

#End Region

#Region " Constructors "

        Public Sub New()

            InitProperties(Nothing)

        End Sub

        Public Sub New(dtAppuntamenti As DataTable)

            Dim listGiorni As List(Of DateTime) = CreateListGiorniConAppuntamenti(dtAppuntamenti)

            InitProperties(listGiorni)

        End Sub

        Public Sub New(listGiorni As List(Of DateTime))

            InitProperties(listGiorni)

        End Sub

#End Region

#Region " Public Methods "

        Public Function Contains(dateValue As DateTime) As Boolean

            If Me.ListGiorni.IsNullOrEmpty() Then Return False

            Return Me.ListGiorni.Contains(dateValue)

        End Function

        Public Sub SetGiornoSelezionato(dateValue As DateTime)

            If Not Me.ListGiorni.IsNullOrEmpty() Then

                Me.IndiceGiornoSelezionato = Me.ListGiorni.FindIndex(Function(p) p = dateValue)

            End If

        End Sub

#End Region

#Region " Private Methods "

        Private Sub InitProperties(listGiorni As List(Of DateTime))

            If listGiorni.IsNullOrEmpty() Then
                Me.ListGiorni = New List(Of DateTime)()
                Me.IndiceGiornoSelezionato = -1
            Else
                Me.ListGiorni = listGiorni.Clone()
                Me.IndiceGiornoSelezionato = 0
            End If

        End Sub

        Private Function CreateListGiorniConAppuntamenti(dtAppuntamenti As DataTable) As List(Of DateTime)

            If dtAppuntamenti Is Nothing OrElse dtAppuntamenti.Rows.Count = 0 Then
                Return Nothing
            End If

            Dim list As New List(Of DateTime)()

            For Each row As DataRow In dtAppuntamenti.Rows

                Dim dataCorrente As DateTime = Convert.ToDateTime(row("cnv_data_appuntamento")).Date

                If Not list.Contains(dataCorrente) Then
                    list.Add(dataCorrente)
                End If

            Next

            If Not list.IsNullOrEmpty() Then
                list.Sort()
            End If

            Return list

        End Function

#End Region

    End Class

#End Region

#Region " Enums "

    Enum PartiGiornata
        Mattina
        Pomeriggio
        Entrambi
    End Enum

    Enum TipoMessaggioLabelWarning
        None = 0
        Alert = 1
        Info = 2
        Deny = 3
        [Error] = 4
    End Enum

#End Region

#Region " Proprietà "

    Public Property p_dtaDateAppuntamenti() As DataTable
        Get
            Return Session("OnVac_p_dtaDateAppuntamenti")
        End Get
        Set(Value As DataTable)
            Session("OnVac_p_dtaDateAppuntamenti") = Value
        End Set
    End Property

    Public Property EliminaBilancio() As Boolean
        Get
            Return ViewState("eliminaBilancio")
        End Get
        Set(Value As Boolean)
            ViewState("eliminaBilancio") = Value
        End Set
    End Property

    Public Property p_dtaAppuntamenti() As DataTable
        Get
            Return Session("OnVac_p_dtaAppuntamenti")
        End Get
        Set(Value As DataTable)
            Session("OnVac_p_dtaAppuntamenti") = Value
        End Set
    End Property

    Public Property p_DurataAppuntamento() As Int16
        Get
            Return ViewState("OnVac_p_DurataAppuntamento")
        End Get
        Set(Value As Int16)
            ViewState("OnVac_p_DurataAppuntamento") = Value
        End Set
    End Property

    Public Property p_DataConvocazione() As DateTime
        Get
            Return ViewState("OnVac_p_DataConvocazione")
        End Get
        Set(Value As DateTime)
            ViewState("OnVac_p_DataConvocazione") = Value
        End Set
    End Property

    Public Property p_OldDate() As DateTime
        Get
            Return ViewState("OnVac_p_OldDate")
        End Get
        Set(Value As DateTime)
            ViewState("OnVac_p_OldDate") = Value
        End Set
    End Property

    Public Property p_dtaOrariValidi() As DataTable
        Get
            Return Session("OnVac_p_dtaOrariValidi")
        End Get
        Set(Value As DataTable)
            Session("OnVac_p_dtaOrariValidi") = Value
        End Set
    End Property

    Public Property p_dtaFestivita() As DataTable
        Get
            Return Session("OnVac_p_dtaFestivita")
        End Get
        Set(Value As DataTable)
            Session("OnVac_p_dtaFestivita") = Value
        End Set
    End Property

    Public Property p_dtaOrariAppuntamento() As Hashtable
        Get
            Return ViewState("OnVac_p_dtaOrariAppuntamento")
        End Get
        Set(Value As Hashtable)
            ViewState("OnVac_p_dtaOrariAppuntamento") = Value
        End Set
    End Property

    'parametro per la conferma sul client della cancellazione della data di invio
    'allo spostamento della data di appuntamento
    Public Property cancellaDataInvio() As Boolean
        Get
            Return Session("cancellaDataInvio")
        End Get
        Set(Value As Boolean)
            Session("cancellaDataInvio") = Value
        End Set
    End Property

    'data di appuntamento assegnata 
    Private Property DataApp() As String
        Get
            Return Session("RicApp_dataApp")
        End Get
        Set(Value As String)
            Session("RicApp_dataApp") = Value
        End Set
    End Property

    'ora di appuntamento assegnata
    Private Property OraApp() As String
        Get
            Return Session("RicApp_oraApp")
        End Get
        Set(Value As String)
            Session("RicApp_oraApp") = Value
        End Set
    End Property

    'filtro da passare al report del bilancio in bianco [modifica 29/04/05]
    Private WriteOnly Property filtroReport() As String
        Set(Value As String)
            Session("BilancioInBianco") = Value
        End Set
    End Property

    'parametri da passare al report del bilancio in bianco [modifica 03/02/05]
    Private WriteOnly Property paramReport() As ArrayList
        Set(Value As ArrayList)
            Session("BilancioInBianco_param") = Value
        End Set
    End Property

    'variabile che indica se il paziente è senza pediatra: la convocazione del paziente
    'deve avere il bilancio e il paziente non deve avere il pediatra (med_codice nullo o diverso da 2)
    '[modifica 21/02/2007 CMR]
    Private Property controllaPazSenzaPediatra() As Boolean
        Get
            Return Session("controllaPazSenzaPediatra")
        End Get
        Set(Value As Boolean)
            Session("controllaPazSenzaPediatra") = Value
        End Set
    End Property

    'variabile che indica se il paziente è un solo bilancio: la devo utilizzare 
    'come proprietà perchè non devo perderne l'informazione durante la ricerca
    'del giorno disponibile [modifica 22/06/2005]
    Private Property controllaPazSoloBilancio() As Boolean
        Get
            Return Session("controllaPazSoloBilancio")
        End Get
        Set(Value As Boolean)
            Session("controllaPazSoloBilancio") = Value
        End Set
    End Property

    Public Property PazienteScelto() As String
        Get
            Return Session("PAZIENTE_SCELTO")
        End Get
        Set(Value As String)
            Session("PAZIENTE_SCELTO") = Value
        End Set
    End Property

    Public ReadOnly Property CodiceAmbulatorioCorrente() As Integer
        Get
            If Not dltOrariDisponibili.SelectedItem Is Nothing Then
                Return Integer.Parse(DirectCast(dltOrariDisponibili.SelectedItem.FindControl("hfCodAmb"), HiddenField).Value)
            Else
                Return Me.uscScegliAmb.ambCodice
            End If
        End Get
    End Property

    Private Property LivelloUtenteConvocazione() As Enumerators.LivelloUtenteConvocazione
        Get
            If ViewState("LivelloUtenteConvocazione") Is Nothing Then Return Enumerators.LivelloUtenteConvocazione.Default
            Return ViewState("LivelloUtenteConvocazione")
        End Get
        Set(value As Enumerators.LivelloUtenteConvocazione)
            ViewState("LivelloUtenteConvocazione") = value
        End Set
    End Property

    Private Property MantieniCnsAppuntamento() As Boolean?
        Get
            Return Session("MantieniCnsAppuntamento")
        End Get
        Set(value As Boolean?)
            Session("MantieniCnsAppuntamento") = value
        End Set
    End Property

    Private ReadOnly Property UrlIconaSelezione As String
        Get
            Return Me.ResolveClientUrl("~/images/sel.gif")
        End Get
    End Property

    Private ReadOnly Property UrlIconaSelezioneApp As String
        Get
            Return Me.ResolveClientUrl("~/images/sel_app.png")
        End Get
    End Property

    Private ReadOnly Property UrlIconaTrasparente As String
        Get
            Return Me.ResolveClientUrl("~/images/transparent16.gif")
        End Get
    End Property

    ' Giorni di intervallo tra inizio e fine disponibilità
    Protected ReadOnly Property GiorniIntervalloDateDisponibili As Int16
        Get
            Return 15
        End Get
    End Property

    Private Property MostraPrenotazioni() As Boolean?
        Get
            Return Session("MostraPrenotazioni")
        End Get
        Set(value As Boolean?)
            Session("MostraPrenotazioni") = value
        End Set
    End Property

    ''' <summary>
    ''' Appuntamenti prenotati nel periodo ricercato
    ''' </summary>
    Private Property AppuntamentiTotali() As List(Of AppuntamentoPrenotato)
        Get
            Return ViewState("AppuntamentiTotali")
        End Get
        Set(Value As List(Of AppuntamentoPrenotato))
            ViewState("AppuntamentiTotali") = Value
        End Set
    End Property

    ''' <summary>
    ''' Appuntamenti prenotati nel mese visualizzato dal calendario
    ''' </summary>
    Private Property AppuntamentiTotali_Calendario() As List(Of AppuntamentoPrenotato)
        Get
            Return ViewState("AppuntamentiTotali_Calendario")
        End Get
        Set(Value As List(Of AppuntamentoPrenotato))
            ViewState("AppuntamentiTotali_Calendario") = Value
        End Set
    End Property

    ''' <summary>
    ''' Disponibilità nel mese visualizzato dal calendario
    ''' </summary>
    Private Property Disponibilita_Calendario() As List(Of Disponibilita)
        Get
            Return ViewState("Disponibilita_Calendario")
        End Get
        Set(Value As List(Of Disponibilita))
            ViewState("Disponibilita_Calendario") = Value
        End Set
    End Property

    ''' <summary>
    ''' Contiene le date con appuntamenti disponibili, in base alla ricerca effettuata.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property GiorniAppuntamentiDisponibili As GiorniConAppuntamenti
        Get
            If ViewState("GiorniDisp") Is Nothing Then ViewState("GiorniDisp") = New GiorniConAppuntamenti()
            Return ViewState("GiorniDisp")
        End Get
        Set(value As GiorniConAppuntamenti)
            ViewState("GiorniDisp") = value
        End Set
    End Property

    ''' <summary>
    ''' Contiene le date con appuntamenti già prenotati, in base alla ricerca effettuata.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property GiorniAppuntamentiPrenotati As GiorniConAppuntamenti
        Get
            If ViewState("GiorniPren") Is Nothing Then ViewState("GiorniPren") = New GiorniConAppuntamenti()
            Return ViewState("GiorniPren")
        End Get
        Set(value As GiorniConAppuntamenti)
            ViewState("GiorniPren") = value
        End Set
    End Property

#End Region

#Region " Inizializzazione proprietà "

    Private Sub InitSession(ByRef genericProvider As DbGenericProvider)

        If Not Me.p_dtaDateAppuntamenti Is Nothing Then Me.p_dtaDateAppuntamenti.Dispose()
        If Not Me.p_dtaFestivita Is Nothing Then Me.p_dtaFestivita.Dispose()
        If Not Me.p_dtaAppuntamenti Is Nothing Then Me.p_dtaAppuntamenti.Dispose()

        Me.p_dtaDateAppuntamenti = New DataTable()
        Me.p_dtaFestivita = New DataTable()
        Me.p_dtaAppuntamenti = New DataTable()
        Me.p_dtaOrariAppuntamento = New Hashtable()

        Me.p_OldDate = DateTime.MinValue

        Me.GiorniAppuntamentiDisponibili = New GiorniConAppuntamenti()
        Me.GiorniAppuntamentiPrenotati = New GiorniConAppuntamenti()

        'slokkiamo il paziente lokkato...
        Me.OnitLayout31.lock.EndLock(OnVacUtility.Variabili.PazId)

    End Sub

#End Region

#Region " Page Events "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Me.lblWarning.Text = String.Empty
        Me.lblErrore.Text = String.Empty
        Me.fmError.VisibileMD = False

        If Not IsPostBack() Then

            ' Se specificato in querystring, impostazione del livello utente
            Me.LivelloUtenteConvocazione = OnVacUtility.GetLivelloUtenteConvocazione(Me.Request.QueryString, False)

            If Me.LivelloUtenteConvocazione = Enumerators.LivelloUtenteConvocazione.Administrator Then

                ' Valore del checkbox memorizzato in session
                If Not Me.MantieniCnsAppuntamento.HasValue Then Me.MantieniCnsAppuntamento = True

                Me.chkMantieniCnsAppuntamento.Checked = Me.MantieniCnsAppuntamento
                Me.chkMantieniCnsAppuntamento.Visible = True

            Else

                Me.chkMantieniCnsAppuntamento.Checked = False
                Me.chkMantieniCnsAppuntamento.Visible = False

            End If

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                OnVacUtility.ImpostaIntestazioniPagina(Me.OnitLayout31, Me.LayoutTitolo, genericProvider, Me.Settings, Me.IsGestioneCentrale)

                'Inizializzazione delle proprietà
                InitSession(genericProvider)

                LoadMotiviEliminazioneAppuntamento(genericProvider)

                'azzera la variabile che indica se il paziente è un solo bilancio e la variabile che indica se il paziente è un senza pediatra
                'azzeramento della variabile di sessione per la cancellazione della data invio
                controllaPazSoloBilancio = False
                controllaPazSenzaPediatra = False
                cancellaDataInvio = False

                ' Check della sessione
                OnVacContext.AssertSession()

                'Se non è stato impostato alcun paziente visualizza la maschera di ricerca (come fa ad arrivare qui senza paziente impostato???)
                If OnVacUtility.IsPazIdEmpty() Then
                    Server.Transfer(Me.ResolveClientUrl("~/HPazienti/GestionePazienti/ricercaPaziente.aspx"))
                    Return
                End If

                'Aggiorna le informazioni sulle convocazioni del paziente
                AggiornaInformazioni(genericProvider)

                ' Impostazione consultorio e ambulatorio di appuntamento 
                If Not Request.QueryString Is Nothing AndAlso Request.QueryString.Count > 0 Then
                    SetCnsAmbAppuntamento(Request.QueryString("cns"), Request.QueryString("amb"), genericProvider)
                End If

            End Using

            Me.uscScegliAmb.databind()

            'Me.txtStatoBtnAssegna.Text = "N"

            ' Campo Note Avvisi visibile solo in base al relativo parametro 
            Me.dypAvvisi.Visible = Me.Settings.GES_NOTE_AVVISI
            Me.divNoteAvvisi.Visible = Me.Settings.GES_NOTE_AVVISI

            AbilitaRicercaAppuntamento(False)

            ShowPrintButtons()

            'Nascondi calendario
            dypCalendario.Visible = False

            'Impostazione valore iniziale di "Mostra anche le prenotazioni" (Valore del checkbox memorizzato in session)
            If Not Me.MostraPrenotazioni.HasValue Then Me.MostraPrenotazioni = True
            Me.chkShowPrenotazioni.Checked = Me.MostraPrenotazioni

        End If

        Me.p_OldDate = Date.MinValue

    End Sub

    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles MyBase.PreRender

        'controllo se sono rimaste delle righe nel datalist
        If dltDateAppuntamenti.SelectedIndex >= 0 AndAlso dltDateAppuntamenti.Items.Count > 0 Then

            'se l'appuntamento è stato spostato deve selezionare nuovamente la data di appuntamento
            If appuntamentoSpostato Then

                dltDateAppuntamenti.SelectedIndex = 0
                SelezionaDataAppuntamento()

                pickNuovaData.Text = String.Empty
                textNuovaOra.Text = String.Empty
                textNuovaDurata.Text = String.Empty

            End If

        End If

        If SelezionataCnvAltroCnsSolaVisualizzazione() Then

            ToolBar.Items.FromKeyButton("btnConferma").Enabled = False
            ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = False
            ToolBar.Items.FromKeyButton("btnAnnullaPrenotazione").Enabled = False
            ToolBar.Items.FromKeyButton("btnEliminaSoloBilancio").Enabled = False

            ToolBar.Items.FromKeyButton("btnStampaAppuntamento").Enabled = True

            uscScegliAmb.SetEnabled(False)

        Else

            uscScegliAmb.SetEnabled(True)

            'controllo se sono rimaste delle righe nel datalist
            If dltDateAppuntamenti.SelectedIndex >= 0 AndAlso dltDateAppuntamenti.Items.Count > 0 Then

                If DirectCast(dltDateAppuntamenti.Items(dltDateAppuntamenti.SelectedIndex).FindControl("lblData"), Label).Text <> "" And
                   DirectCast(dltDateAppuntamenti.Items(dltDateAppuntamenti.SelectedIndex).FindControl("lblOra"), Label).Text <> "" Then

                    If ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = False Then
                        ToolBar.Items.FromKeyButton("btnAnnullaPrenotazione").Enabled = True
                        ToolBar.Items.FromKeyButton("btnStoricoAppuntamento").Enabled = True
                        ToolBar.Items.FromKeyButton("btnStampaAppuntamento").Enabled = True
                    End If

                Else
                    ToolBar.Items.FromKeyButton("btnAnnullaPrenotazione").Enabled = False
                    ToolBar.Items.FromKeyButton("btnStoricoAppuntamento").Enabled = True
                    ToolBar.Items.FromKeyButton("btnStampaAppuntamento").Enabled = False
                End If

            Else
                ToolBar.Items.FromKeyButton("btnAnnullaPrenotazione").Enabled = False
                ToolBar.Items.FromKeyButton("btnStoricoAppuntamento").Enabled = False
                ToolBar.Items.FromKeyButton("btnStampaAppuntamento").Enabled = False
            End If

            'se la convocazione è per solo bilancio abilito il bottone per eliminare la convocazione
            If PazienteSoloBilancio() And Me.ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = False Then
                ToolBar.Items.FromKeyButton("btnEliminaSoloBilancio").Enabled = True
            Else
                ToolBar.Items.FromKeyButton("btnEliminaSoloBilancio").Enabled = False
            End If

            If disabilitaElimPrenotazione Then
                ToolBar.Items.FromKeyButton("btnAnnullaPrenotazione").Enabled = False
                ToolBar.Items.FromKeyButton("btnStampaAppuntamento").Enabled = False
            End If

            If disabilitaElimSoloBilancio Then ToolBar.Items.FromKeyButton("btnEliminaSoloBilancio").Enabled = False

        End If

        ' Abilita/disabilita i controlli relativi all'assegnazione dell'appuntamento (campi data, ora, durata e pulsante)
        ImpostaLayoutControlliAssegnazione()

    End Sub

#End Region

#Region " Messaggi ritardi "

    Private Sub SetDateAppuntamentiRitardi(maxSoll As Int32, dataConvocazione As DateTime)
        '--
        Dim dateAppuntamentiMsg As New System.Text.StringBuilder()
        '--
        If maxSoll > 0 Then
            '--
            Dim dicDateApp1 As New Dictionary(Of String, List(Of String))()
            Dim dicDateApp2 As New Dictionary(Of String, List(Of String))()
            Dim dicDateApp3 As New Dictionary(Of String, List(Of String))()
            Dim dicDateApp4 As New Dictionary(Of String, List(Of String))()
            '--
            Using dam As IDAM = OnVacUtility.OpenDam()
                '--
                With dam.QB
                    .NewQuery()
                    .AddTables("T_PAZ_RITARDI")
                    .AddSelectFields("PRI_DATA_APPUNTAMENTO1, PRI_DATA_APPUNTAMENTO2, PRI_DATA_APPUNTAMENTO3, PRI_DATA_APPUNTAMENTO4, PRI_CIC_CODICE, PRI_SED_N_SEDUTA")
                    .AddWhereCondition("PRI_PAZ_CODICE", Comparatori.Uguale, OnVacUtility.Variabili.PazId, DataTypes.Numero)
                    .AddWhereCondition("PRI_CNV_DATA", Comparatori.Uguale, dataConvocazione, DataTypes.Data)
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
                        While dr.Read()
                            '--
                            Me.AddDateToList(dr, dataApp1Ordinal, codiceCicloOrdinal, numeroSedutaOrdinal, dicDateApp1)
                            Me.AddDateToList(dr, dataApp2Ordinal, codiceCicloOrdinal, numeroSedutaOrdinal, dicDateApp2)
                            Me.AddDateToList(dr, dataApp3Ordinal, codiceCicloOrdinal, numeroSedutaOrdinal, dicDateApp3)
                            Me.AddDateToList(dr, dataApp4Ordinal, codiceCicloOrdinal, numeroSedutaOrdinal, dicDateApp4)
                            '--
                        End While
                        '--
                    End If
                    '--
                End Using
            End Using
            '--
            ' Label per avviso
            '--
            If maxSoll >= 1 Then
                dateAppuntamentiMsg.AppendFormat("App. Avviso: {0}", BuildMessageLabelRitardo(dicDateApp1))
            End If
            '--
            ' Label per primo sollecito
            '--
            If maxSoll >= 2 Then
                If dateAppuntamentiMsg.Length > 0 Then dateAppuntamentiMsg.Append("<br/>")
                dateAppuntamentiMsg.AppendFormat("App. 1° Soll: {0}", BuildMessageLabelRitardo(dicDateApp2))
            End If
            '--
            ' Label per secondo sollecito
            '--
            If maxSoll >= 3 Then
                If dateAppuntamentiMsg.Length > 0 Then dateAppuntamentiMsg.Append("<br/>")
                dateAppuntamentiMsg.AppendFormat("App. 2° Soll: {0}", BuildMessageLabelRitardo(dicDateApp3))
            End If
            '--
            ' Label per terzo sollecito
            '--
            If maxSoll = 4 Then
                If dateAppuntamentiMsg.Length > 0 Then dateAppuntamentiMsg.Append("<br/>")
                dateAppuntamentiMsg.AppendFormat("App. 3° Soll: {0}", BuildMessageLabelRitardo(dicDateApp4))
            End If
            '--
        Else
            '--
            ' Nessun sollecito
            '--
            dateAppuntamentiMsg.Append("Nessuno")
            '--
        End If
        '--
        Me.lblSollecito.Text = dateAppuntamentiMsg.ToString()
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
                dicDateApp.Add(dateToAdd, New List(Of String)())
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

    Private Function BuildMessageLabelRitardo(dataAppDic As Dictionary(Of String, List(Of String))) As String

        Dim msg As New System.Text.StringBuilder()

        If dataAppDic Is Nothing OrElse dataAppDic.Count = 0 Then
            msg.Append("Nessuno")
        Else
            For Each dataAppKeyValuePair As KeyValuePair(Of String, List(Of String)) In dataAppDic
                If msg.Length > 0 Then msg.Append(" - ")
                msg.AppendFormat("<span title='{0}'>{1}</span>",
                                 String.Join(" - ", dataAppKeyValuePair.Value.ToArray()),
                                 dataAppKeyValuePair.Key)
            Next
        End If

        Return msg.ToString()

    End Function

#End Region

#Region " Eventi Datalist "

    Private Sub dltDateAppuntamenti_ItemCommand(source As Object, e As System.Web.UI.WebControls.DataListCommandEventArgs) Handles dltDateAppuntamenti.ItemCommand

        Select Case e.CommandName

            Case "SelezionaData"

                Me.dltDateAppuntamenti.SelectedIndex = e.Item.ItemIndex
                Me.SelezionaDataAppuntamento()
                Me.pickNuovaData.Text = ""
                Me.textNuovaOra.Text = ""

                If SelezionataCnvAltroCnsSolaVisualizzazione() Then

                    AddMessageToLabelWarning(TipoMessaggioLabelWarning.Deny, True, "Non è possibile effettuare prenotazioni sulla convocazione selezionata perchè è relativa ad un centro vaccinale in sola visualizzazione.", False)

                Else

                    If Me.chkMantieniCnsAppuntamento.Checked Then

                        ' Codice consultorio della convocazione selezionata
                        Dim cnsCnvSelezionata As String =
                            DirectCast(Me.dltDateAppuntamenti.SelectedItem.FindControl("lblCnsCnvSelezionata"), Label).Text

                        ' Imposta amb di appuntamento
                        ' = a quello di convocazione se cns corrente, oppure = a 'tutti' o a quello del cns di convocazione (se cns selezionato ha solo 1 amb)
                        If cnsCnvSelezionata <> Me.uscScegliAmb.cnsCodice Then

                            SetCnsAmbAppuntamento(cnsCnvSelezionata, Nothing)

                            CancellaDatiPerModificaAmbulatorio()

                            'Nel caso in cui venga impostato l'amb di app devo aggiornare lo stato del calendario
                            GestioneAbilitazioneCalendario()

                        End If

                        ' Cancello la label perchè altrimenti verrebbe scritta due volte: 
                        ' è stata impostata dal metodo SelezionaDataAppuntamento() e viene impostata nuovamente dentro a ImpostaDurataAppuntamento()
                        Me.lblWarning.Text = String.Empty

                        ' In questo caso, viene mantenuto il consultorio di appuntamento uguale a quello della convocazione selezionata.
                        ' Il consultorio potrebbe essere diverso rispetto alla selezione precedente, perciò va ricalcolata la durata appuntamento.
                        ImpostaDurataAppuntamento()

                    End If

                End If

                'Gestione visualizzazione automatica calendario
                If btnCalendario.Enabled Then
                    ApriCalendario()
                End If

        End Select

    End Sub

    Private Sub dltOrariDisponibili_ItemCommand(source As Object, e As System.Web.UI.WebControls.DataListCommandEventArgs) Handles dltOrariDisponibili.ItemCommand

        Select Case e.CommandName

            Case "Seleziona"

                Me.pickNuovaData.Data = Me.GiorniAppuntamentiDisponibili.DataSelezionata
                Me.textNuovaOra.Text = CType(DirectCast(Me.dltOrariDisponibili.Items(e.Item.ItemIndex).FindControl("Label8"), Label).Text, DateTime).ToString("HH:mm")
                Me.textNuovaDurata.Text = Me.p_DurataAppuntamento
                Me.dltOrariDisponibili.SelectedIndex = e.Item.ItemIndex
                Me.BindDataListOrariDisponibili(True)

        End Select

    End Sub

    Private Sub dltVaccinazioni_DataBinding(sender As Object, e As System.EventArgs) Handles dltVaccinazioni.DataBinding

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            dltVaccinazioni.DataSource =
                genericProvider.VaccinazioneProg.GetVacProgNotEseguiteAndNotEscluse(OnVacUtility.Variabili.PazId, p_dtaDateAppuntamenti.Rows(dltDateAppuntamenti.SelectedIndex)("CNV_DATA"))

        End Using

    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btnConferma"

                ' Controllo che ci sia un appuntamento selezionato
                If Me.dltDateAppuntamenti.SelectedIndex < 0 Then

                    Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Salvataggio non effettuato: nessuna prenotazione selezionata.",
                                                                                                          "AlertSalvataggio", False, False))
                    Return

                End If

                Dim existsAppuntamento As Boolean = False

                Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                    Using bizRicercaAppuntamenti As New BizRicercaAppuntamenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(OnVacUtility.Variabili.CNS.Codice), Nothing)

                        existsAppuntamento = bizRicercaAppuntamenti.ExistsDataAppuntamento(Convert.ToInt64(OnVacUtility.Variabili.PazId), Me.p_DataConvocazione)

                    End Using
                End Using

                If existsAppuntamento Then

                    ' E' obbligatorio selezionare un motivo per lo spostamento dell'appuntamento
                    Me.ddlMotivoModifica.ClearSelection()
                    Me.txtNoteModifica.Text = String.Empty
                    Me.fmMotivoNote.VisibileMD = True

                Else

                    Salva(Constants.MotiviEliminazioneAppuntamento.EliminazioneConvocazione, String.Empty)

                End If

            Case "btnAnnulla"

                AbilitaPulsantiConfermaAnnulla(False)
                Me.OnitLayout31.Busy = False

                'riabilitazione delle date di appuntamento
                DisabilitaDltDateAppuntamenti(False)

                'stato di assegnazione per la modifica del consultorio di appuntamento
                'Me.txtStatoBtnAssegna.Text = "N"

                'abilito nuovamente i campi appuntamento
                AbilitaRicercaAppuntamento(True)

            Case "btnStampaAppuntamento"

                If Not IsNothing(dltDateAppuntamenti.SelectedItem) And p_DataConvocazione.ToString("dd/MM/yyyy") <> "" Then
                    btnStampa_Click()
                Else
                    Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Selezionare una prenotazione per effettuare la stampa.", "", False, False))
                End If

            Case "btnAnnullaPrenotazione"

                AddMessageToLabelWarning(TipoMessaggioLabelWarning.Info, True, "La data di appuntamento per la convocazione selezionata è stata eliminata. Salvare per rendere effettive le modifiche.", False)

                Me.OnitLayout31.Busy = True

                AbilitaPulsantiConfermaAnnulla(True)
                DisabilitaDltDateAppuntamenti(True)
                AbilitaRicercaAppuntamento(False)

            Case "btnBilancioBianco"

                StampaBilancioBianco()

            Case "btnEliminaSoloBilancio"

                Me.EliminaBilancio = True

                AddMessageToLabelWarning(TipoMessaggioLabelWarning.Info, True, "La convocazione per solo bilancio selezionata è stata eliminata. Salvare per rendere effettive le modifiche.", False)

                Me.OnitLayout31.Busy = True

                AbilitaPulsantiConfermaAnnulla(True)
                DisabilitaDltDateAppuntamenti(True)
                AbilitaRicercaAppuntamento(False)

            Case "btnStoricoAppuntamento"

                ShowModaleStoricoAppuntamento()

        End Select

    End Sub

#End Region

#Region " Stampa del bilancio in bianco "

    'propone una modale in cui si possono specificare i filtri per la stampa del bilancio [modifica 28/04/2005]
    Private Sub StampaBilancioBianco()

        Dim dtBilanci As DataTable = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizBilancioProgrammato As New Biz.BizBilancioProgrammato(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                dtBilanci = bizBilancioProgrammato.GetDtBilanciNessunaMalattia()
            End Using
        End Using

        Me.ddlBilancio.DataSource = dtBilanci
        Me.ddlBilancio.DataBind()

        Me.fmStampaBilancioBianco.VisibileMD = True

        Me.OnitLayout31.Busy = True

    End Sub

    Private Sub uwtBilancioBianco_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles uwtBilancioBianco.ButtonClicked

        Select Case be.Button.Key

            Case "btnChiudi"

                Me.fmStampaBilancioBianco.VisibileMD = False
                Me.OnitLayout31.Busy = False

            Case "btnStampa"

                Me.fmStampaBilancioBianco.VisibileMD = False
                Me.OnitLayout31.Busy = False
                filtroReport = "{T_PAZ_PAZIENTI.PAZ_CODICE} = " & OnVacUtility.Variabili.PazId

                Dim parametri As New ArrayList()
                parametri.Add("Bilancio" & "|" & Me.ddlBilancio.SelectedValue)

                paramReport = parametri

                If GetCartellaReport(Constants.ReportName.BilancioInBianco) = String.Empty Then
                    OnVacUtility.StampaNonPresente(Page, Constants.ReportName.BilancioInBianco)
                Else
                    Dim nomeRpt As String = Constants.ReportName.BilancioInBianco.Replace(".rpt", String.Empty)
                    Me.OnitLayout31.InsertRoutineJS(String.Format("window.open('../../Stampe/StampaReportPopUp.aspx?report={0}','{0}','top=0,left=0,width=500,height=500,resizable=1')", nomeRpt))
                End If

        End Select

    End Sub

#End Region

#Region " UserControl Events "

    Private Sub uscScegliAmb_AmbulatorioCambiato(cnsCodice As String, ambCodice As Integer) Handles uscScegliAmb.AmbulatorioCambiato

        ImpostaDurataAppuntamento()

        CancellaDatiPerModificaAmbulatorio()

        'blocco i bottoni che scorrono i giorni
        Me.GiorniAppuntamentiDisponibili = New GiorniConAppuntamenti()
        Me.GiorniAppuntamentiPrenotati = New GiorniConAppuntamenti()
        GestisciButtonsDateDisponibili(True)

        'Gestione calendario
        GestioneAbilitazioneCalendario()
        If Me.dypCalendario.Visible Then
            AggiornaCalendario(Me.Calendario.SelectedDate)
        End If

    End Sub

    Private Sub uscScegliAmb_GetDatePeriodoValidita(ByRef dataInizio As Date?, ByRef dataFine As Date?) Handles uscScegliAmb.GetDatePeriodoValidita

        If Me.odpDispInizio.Data > DateTime.MinValue Then
            dataInizio = Me.odpDispInizio.Data
        End If

        If Me.odpDispFine.Data > DateTime.MinValue Then
            dataFine = Me.odpDispFine.Data
        End If

    End Sub

#End Region

#Region " Checkbox Events "

    Private Sub chkMantieniCnsAppuntamento_CheckedChanged(sender As Object, e As System.EventArgs) Handles chkMantieniCnsAppuntamento.CheckedChanged

        Me.MantieniCnsAppuntamento = Me.chkMantieniCnsAppuntamento.Checked

    End Sub

    Private Sub chkShowPrenotazioni_CheckedChanged(sender As Object, e As System.EventArgs) Handles chkShowPrenotazioni.CheckedChanged

        Me.MostraPrenotazioni = Me.chkShowPrenotazioni.Checked

        CercaPeriodiLiberi()

    End Sub

#End Region

#Region " Button Events "

    Private Sub btnCercaData_Click(sender As System.Object, e As System.EventArgs) Handles btnCercaData.Click

        CercaPeriodiLiberi()

    End Sub

    Private Sub btnAssegna_Click(sender As System.Object, e As System.EventArgs) Handles btnAssegna.Click

        Dim libero As Boolean = True
        Dim data As DateTime

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            If genericProvider.Paziente.PazienteDeceduto(OnVacUtility.Variabili.PazId) Then
                MostraErrore("<br>Il paziente è deceduto. E' impossibile assegnare un appuntamento!")
                Return
            End If

            Me.OnitLayout31.Busy = True
            AbilitaPulsantiConfermaAnnulla(True)

            Dim cnsAppuntamento_SED_MANU As Integer = -1
            Dim cnsAppuntamento_TEMPOBIL As Integer = -1

            'memorizza in sessione la durata di appuntamento
            If String.IsNullOrWhiteSpace(Me.textNuovaDurata.Text) Then

                Dim parametriConsultorioAppuntamento As Biz.BizRicercaAppuntamenti.ParametriConsultorioAppuntamento = ImpostaDurataAppuntamento()
                cnsAppuntamento_SED_MANU = parametriConsultorioAppuntamento.SedManu
                cnsAppuntamento_TEMPOBIL = parametriConsultorioAppuntamento.TempoBil

            Else

                Dim durata As Int16 = 0
                If Not Int16.TryParse(Me.textNuovaDurata.Text, durata) Then
                    MostraErrore("Il campo durata non è valido!")
                    Me.OnitLayout31.Busy = False
                    AbilitaPulsantiConfermaAnnulla(False)
                    Return
                End If

                Me.p_DurataAppuntamento = durata

            End If

            ' Flag per controllo sovrapposizioni
            libero = True

            ' Controllo errori input 
            If String.IsNullOrWhiteSpace(Me.pickNuovaData.Text) OrElse String.IsNullOrWhiteSpace(Me.textNuovaOra.Text) Then
                MostraErrore("<br>E' necessario impostare una data e un orario prima di assegnare l'appuntamento!!!!")
                Me.OnitLayout31.Busy = False
                AbilitaPulsantiConfermaAnnulla(False)
                Return
            End If

            If Not IsDate(Me.pickNuovaData.Text) Then
                MostraErrore("Il campo data non è una data valida!")
                Me.OnitLayout31.Busy = False
                AbilitaPulsantiConfermaAnnulla(False)
                Return
            End If

            If Not IsDate(Me.textNuovaOra.Text) Then
                MostraErrore("Il campo ora non è un orario valido!")
                Me.OnitLayout31.Busy = False
                AbilitaPulsantiConfermaAnnulla(False)
                Return
            End If

            If Date.Parse(Me.pickNuovaData.Text) < DateTime.Today Then
                MostraErrore("Il campo data appuntamento è minore della data corrente!")
                Me.OnitLayout31.Busy = False
                AbilitaPulsantiConfermaAnnulla(False)
                Return
            End If

            Dim str As String = String.Empty

            Try
                Using bizRicercaAppuntamenti As New Biz.BizRicercaAppuntamenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    str = bizRicercaAppuntamenti.CheckAppuntamento(Me.pickNuovaData.Text, Me.p_DurataAppuntamento, Me.CodiceAmbulatorioCorrente, Me.textNuovaOra.Text)

                End Using

            Catch ex As Exception
                SetMessageToLabelWarning(TipoMessaggioLabelWarning.Alert, False, genericProvider.DbError.ErrorMessage, True)
                AbilitaPulsantiConfermaAnnulla(False)
                Return
            End Try

            If str <> "" Then
                MostraErrore("<br>" & str)
            End If

            If AppuntamentoAnterioreConvocazione() Then
                MostraErrore("La data selezionata per l'appuntamento è antecedente la data di convocazione!<br>")
            End If

            'controllo da effettuare per evitare che per i consultori con APPLIBERO = S si verifichi errore
            'perchè non è valorizzata la tabella delle indisponibilità
            If Me.pickNuovaData.Enabled AndAlso p_dtaFestivita.Rows.Count < 1 Then
                CaricaGiorniIndisponibilita(Me.CodiceAmbulatorioCorrente)
            End If

            data = Date.Parse(Me.pickNuovaData.Text & " " & Me.textNuovaOra.Text)

            If cnsAppuntamento_SED_MANU = -1 Or cnsAppuntamento_TEMPOBIL = -1 Then

                cnsAppuntamento_SED_MANU = Me.Settings.SED_MANU
                cnsAppuntamento_TEMPOBIL = Me.Settings.TEMPOBIL

                If Me.uscScegliAmb.cnsCodice <> OnVacUtility.Variabili.CNS.Codice Then

                    ' Caricamento SED_MANU e TEMPOBIL relativi al consultorio in cui viene dato l'appuntamento
                    Dim parametriConsultorioAppuntamento As Biz.BizRicercaAppuntamenti.ParametriConsultorioAppuntamento = Me.GetParametriConsultorioAppuntamento()

                    cnsAppuntamento_SED_MANU = parametriConsultorioAppuntamento.SedManu
                    cnsAppuntamento_TEMPOBIL = parametriConsultorioAppuntamento.TempoBil

                End If

            End If

            If Not IsFree(data, Me.p_dtaAppuntamenti, "AM", False, cnsAppuntamento_SED_MANU, cnsAppuntamento_TEMPOBIL) Then
                libero = False
            End If
            If Not IsFree(data, Me.p_dtaAppuntamenti, "PM", False, cnsAppuntamento_SED_MANU, cnsAppuntamento_TEMPOBIL) Then
                libero = False
            End If

            If libero Then

                AddMessageToLabelWarning(TipoMessaggioLabelWarning.Info, True, "La data di appuntamento per la convocazione selezionata è stata impostata correttamente. Salvare per rendere effettive le modifiche.", False)
                Me.OnitLayout31.Busy = True

                DisabilitaDltDateAppuntamenti(True)

            Else

                AddMessageToLabelWarning(TipoMessaggioLabelWarning.Alert, True, "La data di appuntamento è stata impostata, ma l'appuntamento si sovrappone con un altro!!! Salvare per rendere effettive le modifiche.", False)

                DisabilitaDltDateAppuntamenti(True)

            End If

            If genericProvider.Paziente.pazienteChiamabile(OnVacUtility.Variabili.PazId) = "N" Then

                Dim statoAnag As String = genericProvider.Paziente.statoAnag(OnVacUtility.Variabili.PazId)
                AddMessageToLabelWarning(TipoMessaggioLabelWarning.Alert, True, "Il paziente è " & statoAnag & "!", False)

            End If

        End Using

        'memorizza in sessione la data e l'orario di appuntamento [modifica 03/02/2005]
        Me.DataApp = Me.pickNuovaData.Text
        Me.OraApp = Me.textNuovaOra.Text

        'stato assegnato per il pulsante di modifica consultorio di appuntamento [modifica 09/05/2005]
        'Me.txtStatoBtnAssegna.Text = "S"

        'riabilito eventualmente i campi per l'assegnazione dell'appuntamento [modifica 10/05/2005]
        AbilitaRicercaAppuntamento(True)

    End Sub

    'esegue l'apertura e la stampa del report degli appuntamenti
    Private Sub btnStampa_Click()

        Dim strFiltro As String = String.Empty

        ' N.B. : in questa maschera pazId deve essere SEMPRE valorizzato
        Dim codicePaziente As Int32 = Convert.ToInt32(OnVacUtility.Variabili.PazId)

        ' COS'E' STO SCHIFO?!?
        Dim dataConvocazione As DateTime = Me.p_DataConvocazione.ToString("dd/MM/yyyy")

        Dim dataStampa As DateTime = Date.Now

        If txtNoteAvvisi.Text.Length > txtNoteAvvisi.MaxLength Then
            txtNoteAvvisi.Text = txtNoteAvvisi.Text.Substring(0, txtNoteAvvisi.MaxLength)
        End If

        Dim dataAppuntamento As String = DirectCast(Me.dltDateAppuntamenti.SelectedItem.FindControl("lblData"), Label).Text
        Dim oraAppuntamento As String = DirectCast(Me.dltDateAppuntamenti.SelectedItem.FindControl("lblOra"), Label).Text

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            Try
                genericProvider.BeginTransaction()

                Dim isPazienteRitardatario As Boolean
                Dim isTerminePerentorio As Boolean

                Using bizConvocazione As New Biz.BizConvocazione(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)

                    isPazienteRitardatario = bizConvocazione.IsRitardatario(codicePaziente, dataConvocazione)

                    If isPazienteRitardatario Then isTerminePerentorio = bizConvocazione.TerminePerentorio(codicePaziente, dataConvocazione)

                End Using

                If DirectCast(Me.dltDateAppuntamenti.SelectedItem.FindControl("lblData"), Label).Text = DATA_APPUNTAMENTO_NON_IMPOSTATA And Not isTerminePerentorio Then

                    Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Selezionare una prenotazione valida per effettuare la stampa.", "", False, False))

                Else

                    Using bizStampa As New BizStampaInviti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                        Dim dataScompostaConv(3) As String
                        Dim tipoStampa As String = ""

                        If isTerminePerentorio Then

                            tipoStampa = Constants.TipoStampaAppuntamento.Avvisi

                        ElseIf bizStampa.ControllaSoloBilancio(codicePaziente, dataConvocazione) Then

                            tipoStampa = Constants.TipoStampaAppuntamento.Bilanci

                        ElseIf bizStampa.ControllaAvviso(codicePaziente, dataConvocazione) Then

                            tipoStampa = Constants.TipoStampaAppuntamento.Avvisi

                            If bizStampa.ControllaBilancioAssociato(codicePaziente, dataConvocazione) Then

                                tipoStampa = Constants.TipoStampaAppuntamento.AvvisoBilancio

                            End If

                        End If

                        Dim pageUrl As String = Request.Path

                        If Not Request.QueryString Is Nothing AndAlso Request.QueryString.Count > 0 Then
                            pageUrl = pageUrl + "?" + Request.QueryString.ToString()
                        End If

                        Select Case tipoStampa

                            Case Constants.TipoStampaAppuntamento.Avvisi, Constants.TipoStampaAppuntamento.AvvisoBilancio

                                Dim dataScomposta(3) As String
                                Dim oraScomposta(2) As String

                                'controllo se la data di appuntamento si riferisce ad un termine perentorio [modifica 19/04/2005]
                                If dataAppuntamento = DATA_APPUNTAMENTO_NON_IMPOSTATA Then
                                    dataAppuntamento = "01/01/2100"
                                    oraAppuntamento = "00.00"
                                End If

                                dataScomposta = dataAppuntamento.Split("/")
                                oraScomposta = oraAppuntamento.Split(".")

                                strFiltro = "{V_AVVISI.PAZ_CODICE}=" & codicePaziente &
                                             " AND year({V_AVVISI.CNV_DATA_APPUNTAMENTO}) = " & CInt(dataScomposta(2)) &
                                             " AND month({V_AVVISI.CNV_DATA_APPUNTAMENTO}) = " & CInt(dataScomposta(1)) &
                                             " AND day({V_AVVISI.CNV_DATA_APPUNTAMENTO}) = " & CInt(dataScomposta(0)) &
                                             " AND hour({V_AVVISI.CNV_DATA_APPUNTAMENTO}) = " & CInt(oraScomposta(0)) &
                                             " AND minute({V_AVVISI.CNV_DATA_APPUNTAMENTO}) = " & CInt(oraScomposta(1)) &
                                             " AND year({V_AVVISI.CNV_DATA}) = " & dataConvocazione.Year &
                                             " AND month({V_AVVISI.CNV_DATA}) = " & dataConvocazione.Month &
                                             " AND day({V_AVVISI.CNV_DATA}) = " & dataConvocazione.Day

                                ' Parametro per stampare le note inserite
                                Dim rpt As New ReportParameter()
                                If Me.Settings.GES_NOTE_AVVISI Then
                                    rpt.AddParameter("NoteAvviso", txtNoteAvvisi.Text)
                                Else
                                    rpt.AddParameter("NoteAvviso", "")
                                End If

                                If Not OnVacReport.StampaReport(pageUrl, Constants.ReportName.AvvisoAppuntamento, strFiltro, rpt, Nothing, Nothing, GetCartellaReport(Constants.ReportName.AvvisoAppuntamento)) Then
                                    OnVacUtility.StampaNonPresente(Page, Constants.ReportName.AvvisoAppuntamento)
                                End If

                                bizStampa.SalvaDataInvio(codicePaziente, dataConvocazione, tipoStampa, dataStampa, isPazienteRitardatario, txtNoteAvvisi.Text)

                                genericProvider.Commit()

                            Case Constants.TipoStampaAppuntamento.Bilanci

                                strFiltro = "{V_BILANCI.PAZ_CODICE}=" & codicePaziente &
                                            " AND year({V_BILANCI.CNV_DATA}) = " & dataConvocazione.Year &
                                            " AND month({V_BILANCI.CNV_DATA}) = " & dataConvocazione.Month &
                                            " AND day({V_BILANCI.CNV_DATA}) = " & dataConvocazione.Day &
                                            " AND {V_BILANCI.CNV_CNS_CODICE}='" & OnVacUtility.Variabili.CNS.Codice & "' "

                                If Not OnVacReport.StampaReport(pageUrl, Constants.ReportName.AvvisoSoloBilancio, strFiltro, Nothing, Nothing, Nothing, GetCartellaReport(Constants.ReportName.AvvisoSoloBilancio)) Then
                                    OnVacUtility.StampaNonPresente(Page, Constants.ReportName.AvvisoSoloBilancio)
                                End If

                                bizStampa.SalvaDataInvio(codicePaziente, dataConvocazione, tipoStampa, dataStampa, isPazienteRitardatario, txtNoteAvvisi.Text)

                                genericProvider.Commit()

                            Case Else

                                OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(
                                                        "Per questa prenotazione non è possibile effettuare la stampa (probabilmente " &
                                                        "la convocazione ha associato un bilancio per malattia cronica).", "", False, False))

                                genericProvider.Rollback()

                        End Select

                    End Using
                End If

            Catch ex As Exception

                genericProvider.Rollback()

                ex.InternalPreserveStackTrace()
                Throw

            End Try

        End Using

    End Sub

    'chiude il messaggio degli orari di apertura del consultorio (modifica 20/07/2004)
    Private Sub btnOrariApertura_Click(sender As Object, e As System.EventArgs) Handles btnOrariApertura.Click

        Me.fmMessaggioOrariApertura.VisibileMD = False

    End Sub

    'carica i dati del giorno libero (per un appuntamento) successivo a quello visualizzato
    Private Sub btnNext_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnNext.Click

        If Me.GiorniAppuntamentiDisponibili.HasValue Then

            Me.dltOrariDisponibili.SelectedIndex = -1
            Me.GiorniAppuntamentiDisponibili.IndiceGiornoSelezionato += 1

            BindDataListOrariDisponibili(True)

        ElseIf Me.GiorniAppuntamentiPrenotati.HasValue Then

            Me.dltOrariDisponibili.SelectedIndex = -1
            Me.GiorniAppuntamentiPrenotati.IndiceGiornoSelezionato += 1

            BindDataListOrariDisponibili(True)

        End If

    End Sub

    'carica i dati del giorno libero (per un appuntamento) precedente a quello visualizzato
    Private Sub btnPrev_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnPrev.Click

        If Me.GiorniAppuntamentiDisponibili.HasValue Then

            Me.dltOrariDisponibili.SelectedIndex = -1
            Me.GiorniAppuntamentiDisponibili.IndiceGiornoSelezionato -= 1

            BindDataListOrariDisponibili(True)

        ElseIf Me.GiorniAppuntamentiPrenotati.HasValue Then

            Me.dltOrariDisponibili.SelectedIndex = -1
            Me.GiorniAppuntamentiPrenotati.IndiceGiornoSelezionato -= 1

            BindDataListOrariDisponibili(True)

        End If

    End Sub

    'carica i dati del primo giorno disponibile per l'appuntamento
    Private Sub btnFirst_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnFirst.Click

        If Me.GiorniAppuntamentiDisponibili.HasValue Then

            Me.dltOrariDisponibili.SelectedIndex = -1
            Me.GiorniAppuntamentiDisponibili.IndiceGiornoSelezionato = 0

            BindDataListOrariDisponibili(True)

        ElseIf Me.GiorniAppuntamentiPrenotati.HasValue Then

            Me.dltOrariDisponibili.SelectedIndex = -1
            Me.GiorniAppuntamentiPrenotati.IndiceGiornoSelezionato = 0

            BindDataListOrariDisponibili(True)

        End If

    End Sub

    'carica i dati dell'ultimo giorno disponibile per l'appuntamento
    Private Sub btnLast_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnLast.Click

        If Me.GiorniAppuntamentiDisponibili.HasValue Then

            Me.dltOrariDisponibili.SelectedIndex = -1
            Me.GiorniAppuntamentiDisponibili.IndiceGiornoSelezionato = Me.GiorniAppuntamentiDisponibili.Count - 1

            BindDataListOrariDisponibili(True)

        ElseIf Me.GiorniAppuntamentiPrenotati.HasValue Then

            Me.dltOrariDisponibili.SelectedIndex = -1
            Me.GiorniAppuntamentiPrenotati.IndiceGiornoSelezionato = Me.GiorniAppuntamentiPrenotati.Count - 1

            BindDataListOrariDisponibili(True)

        End If

    End Sub

    Private Sub btnFmError_Click(sender As Object, e As System.EventArgs) Handles btnFmError.Click

        Me.fmError.Visible = False

    End Sub

#End Region

#Region " Ricerca/Selezione Appuntamenti "

    Private Sub AggiornaLabelCnsAmb()

        If Me.p_dtaDateAppuntamenti.Rows(Me.dltDateAppuntamenti.SelectedIndex)("CNV_CNS_CODICE") Is Nothing OrElse
           Me.p_dtaDateAppuntamenti.Rows(Me.dltDateAppuntamenti.SelectedIndex)("CNV_CNS_CODICE") Is DBNull.Value Then

            Me.lblCnsAmb.Text = String.Empty

        Else

            Dim ambulatorio As String = String.Empty

            If Not Me.p_dtaDateAppuntamenti.Rows(Me.dltDateAppuntamenti.SelectedIndex)("AMB_DESCRIZIONE") Is Nothing AndAlso
               Not Me.p_dtaDateAppuntamenti.Rows(Me.dltDateAppuntamenti.SelectedIndex)("AMB_DESCRIZIONE") Is DBNull.Value Then

                ambulatorio = " - " + Me.p_dtaDateAppuntamenti.Rows(Me.dltDateAppuntamenti.SelectedIndex)("AMB_DESCRIZIONE").ToString()

            End If

            Me.lblCnsAmb.Text = String.Format("{0} [{1}]{2}",
                                              Me.p_dtaDateAppuntamenti.Rows(Me.dltDateAppuntamenti.SelectedIndex)("CNS_DESCRIZIONE").ToString(),
                                              Me.p_dtaDateAppuntamenti.Rows(Me.dltDateAppuntamenti.SelectedIndex)("CNV_CNS_CODICE").ToString(),
                                              ambulatorio)
        End If

        Me.lblCnsAmb.ToolTip = Me.lblCnsAmb.Text

    End Sub

    Private Sub SelezionaDataAppuntamento()

        dltVaccinazioni.DataBind()
        lblConvocazione.DataBind()
        lblMedico.DataBind()
        lblDurata.DataBind()
        lblIdAppuntamento.DataBind()

        Me.AggiornaLabelCnsAmb()

        Me.lblPrimoAppuntamento.DataBind()

        'per ricercare il bilancio associato
        Me.lblBilancio.DataBind()

        If Not Me.p_dtaDateAppuntamenti.Rows(Me.dltDateAppuntamenti.SelectedIndex)("CNV_NOTE_AVVISI") Is DBNull.Value Then
            Me.txtNoteAvvisi.Text = Me.p_dtaDateAppuntamenti.Rows(Me.dltDateAppuntamenti.SelectedIndex)("CNV_NOTE_AVVISI")
        Else
            Me.txtNoteAvvisi.Text = String.Empty
        End If

        If Not Me.p_dtaDateAppuntamenti.Rows(Me.dltDateAppuntamenti.SelectedIndex)("CNV_DURATA_APPUNTAMENTO") Is DBNull.Value Then
            Me.DurataAppuntamentoCNV = Me.p_dtaDateAppuntamenti.Rows(Me.dltDateAppuntamenti.SelectedIndex)("CNV_DURATA_APPUNTAMENTO")
        End If

        Me.p_DataConvocazione = Me.p_dtaDateAppuntamenti.Rows(Me.dltDateAppuntamenti.SelectedIndex)("CNV_DATA")
        '--
        ' Informazioni su solleciti/ritardi
        Dim maxSollecito As Integer = 0
        '--
        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizConvocazione As New BizConvocazione(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                maxSollecito = bizConvocazione.GetMaxSollecitoVaccinazioni(OnVacUtility.Variabili.PazId, Me.p_DataConvocazione)

            End Using
        End Using
        '--
        If maxSollecito = 0 Then
            'impostazione del colore della label del ritardo (modifica 14/07/2004)
            lblRitardo.ForeColor = Color.Black
            lblRitardoInt.ForeColor = Color.Black
            lblRitardo.Text = ""
        Else
            'impostazione del colore della label del ritardo (modifica 14/07/2004)
            lblRitardo.ForeColor = Color.Red
            lblRitardoInt.ForeColor = Color.Red
            lblRitardo.Text = maxSollecito
        End If
        '--
        ' Valorizzazione label solleciti in base ai ritardi del paziente
        Me.SetDateAppuntamentiRitardi(maxSollecito, Me.p_DataConvocazione)
        '--
        ' Calcolo date dell'intervallo di disponibilità degli appuntamenti
        Me.odpDispInizio.Text = IIf(Me.p_DataConvocazione > Date.Now.Date, Me.p_DataConvocazione, Date.Now.Date)
        Me.odpDispFine.Text = CDate(Me.odpDispInizio.Text).AddDays(Me.GiorniIntervalloDateDisponibili)
        ' --
        ' Imposta la durata dell'appuntamento in base ai parametri del consultorio di appuntamento selezionato
        Dim parametriConsultorioAppuntamento As Biz.BizRicercaAppuntamenti.ParametriConsultorioAppuntamento = ImpostaDurataAppuntamento()
        '--
        Dim cnsAppuntamento_SED_MANU As Integer = parametriConsultorioAppuntamento.SedManu
        '--
        ' Azzera l'elenco degli orari disponibili
        BindDataListOrariDisponibili(False)
        '--
        'controlla se nella convocazione è già valorizzata la data di invio e lo memorizza in sessione
        Me.cancellaDataInvio = Me.DataInvioValorizzata(Me.p_DataConvocazione)
        '--
        'controlla se la convocazione del paziente è per il solo bilancio o se la cnv selezionata è su un altro cns in sola visualizzazione
        Dim disabilitaControlliRicercaAppuntamento As Boolean =
            (PazienteSoloBilancio() And Me.p_DurataAppuntamento = 0) Or
            (Me.p_DurataAppuntamento = 0 And cnsAppuntamento_SED_MANU = 0) Or
            SelezionataCnvAltroCnsSolaVisualizzazione()

        Me.AbilitaRicercaAppuntamento(Not disabilitaControlliRicercaAppuntamento)

        'blocco i bottoni che scorrono i giorni
        Me.GiorniAppuntamentiDisponibili = New GiorniConAppuntamenti()
        Me.GiorniAppuntamentiPrenotati = New GiorniConAppuntamenti()
        Me.GestisciButtonsDateDisponibili(True)

        Me.CaricaCalendario()

    End Sub

    'Carica gli orari per tutta la settimana
    Private Sub CaricaGiorniIndisponibilita(ambCodice As Integer)

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            Me.p_dtaFestivita.Rows.Clear()
            Me.p_dtaFestivita = genericProvider.Prenotazioni.GetGiorniFestivi(ambCodice)

        End Using

    End Sub

    'Carica la durata dell'appuntamento selezionato
    Sub CaricaDurataAppuntamento(cnsAppuntamento_SED_MANU As Integer, cnsAppuntamento_TEMPOBIL As Integer)

        controllaPazSoloBilancio = PazienteSoloBilancio()
        controllaPazSenzaPediatra = PazienteSenzaPediatra()

        'deve recuperare il valore di SED_MANU se questo è > 0, altrimenti CNV_DURATA_APPUNTAMENTO
        'modifica 21/02/2007 Chiara --- aggiunto il controllo del paziente senza pediatra che deve avere una durata pari a TEMPOBIL
        If cnsAppuntamento_SED_MANU > 0 And Not controllaPazSoloBilancio And Not controllaPazSenzaPediatra Then

            Me.p_DurataAppuntamento = cnsAppuntamento_SED_MANU

            AddMessageToLabelWarning(TipoMessaggioLabelWarning.Alert, True, "La durata prevista per ogni appuntamento del centro vaccinale scelto è <b>" & cnsAppuntamento_SED_MANU & " </b>minuti. Tale valore è quello utilizzato per il calcolo degli appuntamenti, indipendentemente dalla durata appuntamento specificata nella convocazione selezionata.", False)

        Else

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Me.p_DurataAppuntamento = genericProvider.Convocazione.GetDurataAppuntamento(OnVacUtility.Variabili.PazId, Me.p_DataConvocazione.ToShortDateString())
            End Using

            'Tutti Warning

            'visualizza un messaggio di avvertimento differente nel caso in cui il paziente sia senza pediatra o
            'la convocazione del paziente sia esclusivamente per il bilancio [modifica 16/02/2005]
            If controllaPazSoloBilancio Then

                If cnsAppuntamento_TEMPOBIL > 0 Then
                    Me.p_DurataAppuntamento = cnsAppuntamento_TEMPOBIL
                    AddMessageToLabelWarning(TipoMessaggioLabelWarning.Alert, True, "Le convocazioni con solo bilancio non hanno in generale una durata di appuntamento, se però l'appuntamento è assegnato o cambiato da questa maschera, verrà assegnata una durata di <b>" & Me.p_DurataAppuntamento & " </b>minuti.", False)
                Else
                    AddMessageToLabelWarning(TipoMessaggioLabelWarning.Alert, True, "Le convocazioni con solo bilancio non hanno in generale una durata di appuntamento, quindi è sufficiente assegnare la data all'interno della fascia costituita dagli orari di apertura del centro vaccinale.", False)
                End If

            Else

                If controllaPazSenzaPediatra Then
                    If cnsAppuntamento_TEMPOBIL > 0 Then
                        Me.p_DurataAppuntamento = cnsAppuntamento_TEMPOBIL
                        AddMessageToLabelWarning(TipoMessaggioLabelWarning.Alert, True, "Le convocazioni per i pazienti che non hanno il pediatra hanno in generale una durata di appuntamento di <b>" & Me.p_DurataAppuntamento & " </b>minuti.", False)
                    End If
                Else
                    If Me.p_DurataAppuntamento = 0 And cnsAppuntamento_SED_MANU = 0 Then
                        AddMessageToLabelWarning(TipoMessaggioLabelWarning.Alert, True, "Per i pazienti ritardatari è sufficiente assegnare la data all'interno della fascia costituita dagli orari di apertura del centro vaccinale.", False)
                    End If
                End If

            End If

        End If

    End Sub

    '*******************************
    'La funzione controlla la disponibilità di appuntamento compatibilmente con le date
    'di festa, gli orari di indisponibilità e la sovrapposizione con altri appuntamenti
    '---- IN:
    'dataOra:           data e ora dell'appuntamento
    'dtAppuntamenti:    tabella in cui cercare gli appuntamenti già fissati
    'AMPM:              Indica il momento della giornata AM o PM
    '---- OUT (boolean):
    'Restituisce True se un appuntamento è disponibile altrimenti False
    '*******************************
    Private Function IsFree(ByRef dataOra As DateTime, dtAppuntamenti As DataTable, AMPM As String, controlloSovrapposizioni As Boolean,
                            cnsAppuntamento_SED_MANU As Integer, cnsAppuntamento_TEMPOBIL As Integer) As Boolean

        Dim cntFestivita As String = dataOra.ToString("dd/MM") & "/" & OnVac.Constants.CommonConstants.RECURSIVE_YEAR & " " & dataOra.ToString("HH:mm")
        Dim r() As DataRow

        'Controlla se la data cade in una festa
        r = Me.p_dtaFestivita.Select("INIZIO_" & AMPM & "<=Convert('" & cntFestivita & "','System.DateTime') AND FINE_" & AMPM & ">Convert('" & cntFestivita & "','System.DateTime')")
        If r.GetUpperBound(0) >= 0 Then
            dataOra = r(0)("FINE_" & AMPM)
            Return False
        End If

        'Controlla se la data cade in un periodo di indisponibilità ricorsivo
        r = Me.p_dtaFestivita.Select("INIZIO_AM<=Convert('" & cntFestivita & "','System.DateTime') AND FINE_PM>Convert('" & cntFestivita & "','System.DateTime')")
        If r.GetUpperBound(0) >= 0 Then
            dataOra = r(0)("FINE_" & AMPM)
            Return False
        End If

        'Controlla se la data cade in un periodo di indisponibilità non ricorsivo
        r = Me.p_dtaFestivita.Select("INIZIO_" & AMPM & "<=Convert('" & dataOra & "','System.DateTime') AND FINE_" & AMPM & ">Convert('" & dataOra & "','System.DateTime')")
        If r.GetUpperBound(0) >= 0 Then
            dataOra = r(0)("FINE_" & AMPM)
            Return False
        End If

        If dtAppuntamenti Is Nothing OrElse dtAppuntamenti.Rows.Count = 0 Then
            Return True
        End If

        If controlloSovrapposizioni Then

            'Controlla che non ci siano sovrapposizioni
            r = dtAppuntamenti.Select("CNV_FINE_APPUNTAMENTO >Convert('" & dataOra & "','System.DateTime') AND CNV_DATA_APPUNTAMENTO <Convert('" & dataOra.AddMinutes(p_DurataAppuntamento) & "','System.DateTime')")
            If r.GetUpperBound(0) >= 0 Then
                '---
                'NUOVA VERSIONE [modifica 22/06/2005]
                'se il paziente è un solo bilancio, la durata dell'appuntamento deve essere considerata secondo il valore del parametro TEMPOBIL (se valorizzato)
                If Not r(0)("PAZ_SOLO_BILANCIO") Is DBNull.Value AndAlso cnsAppuntamento_TEMPOBIL > 0 Then
                    dataOra = dataOra.AddMinutes(cnsAppuntamento_TEMPOBIL)
                    Return False
                End If
                '---
                'se il parametro SED_MANU è valorizzato, l'appuntamento deve comunque avere una durata pari
                'a questo parametro e non controllare la durata effettiva dell'appuntamento
                If cnsAppuntamento_SED_MANU > 0 Then

                    If r(0)("CNV_DATA_APPUNTAMENTO") > dataOra Then
                        '---
                        'NUOVA VERSIONE
                        'se l'appuntamento è già assegnato, deve considerare la durata effettiva 
                        'e non quella di uno dei parametri [modifica 11/08/2005]
                        If Not r(0)("CNV_DATA_APPUNTAMENTO") Is DBNull.Value Then
                            dataOra = r(0)("CNV_DATA_APPUNTAMENTO").AddMinutes(r(0)("CNV_DURATA_APPUNTAMENTO"))
                        Else
                            dataOra = r(0)("CNV_DATA_APPUNTAMENTO").AddMinutes(cnsAppuntamento_SED_MANU)
                        End If
                        '---
                    Else
                        If Not r(0)("CNV_DATA_APPUNTAMENTO") Is DBNull.Value Then
                            dataOra = dataOra.AddMinutes(r(0)("CNV_DURATA_APPUNTAMENTO"))
                        Else
                            dataOra = dataOra.AddMinutes(cnsAppuntamento_SED_MANU)
                        End If
                    End If

                    Return False

                End If
                '---
                dataOra = r(0)("CNV_FINE_APPUNTAMENTO")
                '---
                Return False
                '---
            End If
        End If

        'Tutto OK
        Return True

    End Function

    Sub CercaPeriodiLiberi()
        ' TODO [Ricerca appuntamenti]: Rimuovi appuntamenti prima dell'orario corrente in data odierna (se visualizzata dal datalist)?

        If Me.textNuovaDurata.Text <> "" Then
            Me.p_DurataAppuntamento = Me.textNuovaDurata.Text
        End If
        '--
        If String.IsNullOrEmpty(Me.odpDispInizio.Text) Then
            '--
            MostraErrore("Il campo data inizio deve essere valorizzato !")
            Return
            '--
        End If
        '--
        If Me.odpDispInizio.Data.Date < DateTime.Now.Date Then
            '--
            MostraErrore("Il campo data inizio è minore della data corrente !")
            Return
            '--
        End If
        '--
        If String.IsNullOrEmpty(Me.odpDispFine.Text) Then
            '--
            MostraErrore("Il campo data fine deve essere valorizzato!")
            Return
            '--
        End If
        '--
        If Me.odpDispFine.Data.Date < Me.odpDispInizio.Data.Date Then
            '--
            MostraErrore("Il campo data fine deve essere maggiore del campo data inizio!")
            Return
            '--
        End If
        '--
        Dim result As CercaPeriodiLiberiResult = Me.CercaPeriodiLiberi(Me.odpDispInizio.Data.Date, Me.odpDispFine.Data.Date)
        '--
        Me.p_dtaAppuntamenti = result.Appuntamenti
        Me.p_dtaOrariValidi = result.OrariValidi
        Me.AppuntamentiTotali = result.AppuntamentiTotali
        Me.GiorniAppuntamentiDisponibili = New GiorniConAppuntamenti(result.GiorniDisponibili)
        Me.GiorniAppuntamentiPrenotati = New GiorniConAppuntamenti(Me.p_dtaAppuntamenti)
        '--
        BindDataListOrariDisponibili(True)
        '--
        AbilitaRicercaAppuntamento(True)
        '--
    End Sub

    Private Function CercaPeriodiLiberi(dataMinima As DateTime, dataMassima As DateTime) As CercaPeriodiLiberiResult
        '--
        Dim giorniDisponibili As New List(Of DateTime)()
        Dim dtaAppuntamentiAmbulatorio As New DataTable()
        Dim listAppuntamentiTotali As New List(Of AppuntamentoPrenotato)()
        '--
        Dim dtaOrariValidi As New DataTable()
        dtaOrariValidi.Columns.Add("Data", GetType(DateTime))
        dtaOrariValidi.Columns.Add("Ora", GetType(DateTime))
        dtaOrariValidi.Columns.Add("ambCodice", GetType(Integer))
        dtaOrariValidi.Columns.Add("Amb", GetType(String))
        dtaOrariValidi.Columns.Add("Vaccinazioni", GetType(String))
        '--
        Dim paz As New CnvPaziente(OnVacUtility.Variabili.PazId, Date.MinValue, False, False)
        '--
        Dim consultorioCalcoloAppuntamenti As New Consultorio()
        consultorioCalcoloAppuntamenti.DaData = dataMinima
        consultorioCalcoloAppuntamenti.AData = dataMassima
        '--
        ' TODO [Ricerca appuntamenti]: gestione multipla degli ambulatori
        '--
        If Me.uscScegliAmb.ambCodice <> 0 Then
            SetAmbulatorioCalcoloAppuntamenti(Me.uscScegliAmb.ambCodice, Me.uscScegliAmb.ambDescrizione, consultorioCalcoloAppuntamenti)
        Else
            SetListAmbulatoriCalcoloAppuntamenti(Me.uscScegliAmb.cnsCodice, consultorioCalcoloAppuntamenti)
        End If
        '--
        '[Imposto i filtri per ciascun ambulatorio]
        '--
        For Each amb As Ambulatorio In consultorioCalcoloAppuntamenti.Ambulatori
            '--
            dtaAppuntamentiAmbulatorio.Rows.Clear()
            '--
            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                dtaAppuntamentiAmbulatorio = genericProvider.Prenotazioni.GetPrenotati(CInt(amb.Codice), dataMinima, dataMassima.AddDays(1))
            End Using
            '--
            If Not dtaAppuntamentiAmbulatorio Is Nothing AndAlso dtaAppuntamentiAmbulatorio.Rows.Count > 0 Then

                Dim list As List(Of AppuntamentoPrenotato) = dtaAppuntamentiAmbulatorio.ConvertToList(Of AppuntamentoPrenotato)()

                If Not list Is Nothing AndAlso list.Count > 0 Then
                    listAppuntamentiTotali.AddRange(list)
                End If

            End If
            '--
            Dim filtriOcc As Filtri.FiltroCollection = RestituisciPrenotati(dtaAppuntamentiAmbulatorio)
            '--
            For Each f As Filtri.IFiltro In filtriOcc
                amb.FiltriAmbulatorio.Add(f)
            Next
            '--
            Dim filtriOrari As Filtri.FiltroCollection = CaricaFiltriApertura(Convert.ToInt32(amb.Codice), False, -1, PartiGiornata.Entrambi)
            '--
            For Each f As Filtri.IFiltro In filtriOrari
                amb.FiltriAmbulatorio.Add(f)
            Next
            '--
            Dim filtriFesta As Filtri.FiltroCollection = CaricaFiltriFeste(CInt(amb.Codice))
            '--
            For Each f As Filtri.IFiltro In filtriFesta
                amb.FiltriAmbulatorio.Add(f)
            Next
            '--
        Next
        '--
        For Each amb As Ambulatorio In consultorioCalcoloAppuntamenti.Ambulatori
            '--
            If consultorioCalcoloAppuntamenti.Pazienti.Count > 0 Then
                consultorioCalcoloAppuntamenti.Pazienti.Remove(paz)
            End If
            '--
            paz.Ambulatorio = CInt(amb.Codice)
            '--
            consultorioCalcoloAppuntamenti.Pazienti.Add(paz)
            '--
            '27/08/2008 MGR se utilizzo la ricerca appuntamenti della classe consultorio per gli ambulatori successivi al primo
            'vengono duplicati i record del singolo orario tante volte quanto è l'indice dell'ambulatorio in esame, qs perchè la funzione
            'FindAppuntamenti in qs classe ha a sua volta un ciclo for per tutti gli ambulatori.
            'utilizzando la funzione nella classe Ambulatorio qs problema non si verifica.
            'calcoloAppuntamenti.FindAppuntamenti()
            '--
            amb.FindAppuntamenti()
            '--
            CaricaGiorniIndisponibilita(amb.Codice)
            '--
            If Not amb.Giorni Is Nothing AndAlso amb.Giorni.Count > 0 Then
                For Each d As Day In amb.Giorni
                    For Each t As TimeBlock In d.TimeBlocks
                        If t.IsFree(0) Then
                            '--
                            Dim oraInizio As Date = t.Inizio
                            Dim orafine As Date = t.Fine
                            '--
                            Dim numAppLiberi = Math.Floor((orafine.TimeOfDay.TotalMinutes - oraInizio.TimeOfDay.TotalMinutes) / p_DurataAppuntamento)
                            '--
                            For i As Integer = 0 To numAppLiberi - 1
                                '--
                                If Not giorniDisponibili.Contains(d.Giorno) Then
                                    giorniDisponibili.Add(d.Giorno)
                                End If
                                '--
                                Dim r As DataRow = dtaOrariValidi.NewRow()
                                '--
                                r("Data") = d.Giorno
                                r("Ora") = oraInizio.AddMinutes(i * p_DurataAppuntamento)
                                r("ambCodice") = amb.Codice
                                r("Amb") = amb.Descrizione
                                r("Vaccinazioni") = String.Empty
                                '--
                                dtaOrariValidi.Rows.Add(r)
                                '--
                            Next
                        End If
                    Next
                Next
            End If
        Next
        '--
        If giorniDisponibili.Count > 0 Then
            giorniDisponibili.Sort()
        End If
        '--
        Me.dltOrariDisponibili.Visible = True

        'Gestione visualizzazione prenotazioni
        If Me.MostraPrenotazioni AndAlso listAppuntamentiTotali IsNot Nothing Then

            For Each item As AppuntamentoPrenotato In listAppuntamentiTotali

                Dim rov As DataRow = dtaOrariValidi.NewRow()
                rov("Data") = item.CNV_DATA_APPUNTAMENTO.Date
                rov("Ora") = New DateTime(1, 1, 1, item.CNV_DATA_APPUNTAMENTO.Hour, item.CNV_DATA_APPUNTAMENTO.Minute, item.CNV_DATA_APPUNTAMENTO.Second)
                rov("ambCodice") = item.AMB_CODICE.ToString()
                rov("Amb") = item.AMB_DES
                rov("Vaccinazioni") = IIf(Not String.IsNullOrWhiteSpace(item.VACCINAZIONI), item.VACCINAZIONI, "Solo bilancio")

                dtaOrariValidi.Rows.Add(rov)

            Next

        End If

        Dim result As New CercaPeriodiLiberiResult()
        result.Appuntamenti = dtaAppuntamentiAmbulatorio
        result.GiorniDisponibili = giorniDisponibili
        result.OrariValidi = dtaOrariValidi
        result.AppuntamentiTotali = listAppuntamentiTotali

        Return result

    End Function

    Private Sub SetAmbulatorioCalcoloAppuntamenti(codiceAmbulatorio As Integer, descrizioneAmbulatorio As String, consultorioCalcoloAppuntamenti As CalcoloAppuntamenti.Consultorio)

        Dim ambulatorioSelezionato As Entities.Ambulatorio = Nothing

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizConsultori As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                ambulatorioSelezionato = bizConsultori.GetAmbulatorio(codiceAmbulatorio)

            End Using
        End Using

        Dim ambulatorioCalcoloAppuntamenti As CalcoloAppuntamenti.Ambulatorio =
            New CalcoloAppuntamenti.Ambulatorio(codiceAmbulatorio, False, consultorioCalcoloAppuntamenti)

        ambulatorioCalcoloAppuntamenti.Descrizione = descrizioneAmbulatorio

        If Not ambulatorioSelezionato Is Nothing Then
            ambulatorioCalcoloAppuntamenti.DataAperturaAmbulatorio = ambulatorioSelezionato.DataApertura
            ambulatorioCalcoloAppuntamenti.DataChiusuraAmbulatorio = ambulatorioSelezionato.DataChiusura
        End If

        consultorioCalcoloAppuntamenti.Ambulatori.Add(ambulatorioCalcoloAppuntamenti)

    End Sub

    Private Sub SetListAmbulatoriCalcoloAppuntamenti(codiceConsultorioSelezionato As String, consultorioCalcoloAppuntamenti As CalcoloAppuntamenti.Consultorio)

        Dim listAmbulatori As List(Of Entities.Ambulatorio) = Nothing

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizConsultori As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                listAmbulatori = bizConsultori.GetAmbulatori(codiceConsultorioSelezionato, True)

            End Using
        End Using

        If Not listAmbulatori.IsNullOrEmpty() Then

            For Each ambulatorio As Entities.Ambulatorio In listAmbulatori

                Dim ambulatorioCalcoloAppuntamenti As New CalcoloAppuntamenti.Ambulatorio(ambulatorio.Codice, False, consultorioCalcoloAppuntamenti)
                ambulatorioCalcoloAppuntamenti.Descrizione = ambulatorio.Descrizione
                ambulatorioCalcoloAppuntamenti.DataAperturaAmbulatorio = ambulatorio.DataApertura
                ambulatorioCalcoloAppuntamenti.DataChiusuraAmbulatorio = ambulatorio.DataChiusura

                consultorioCalcoloAppuntamenti.Ambulatori.Add(ambulatorioCalcoloAppuntamenti)

            Next

        End If

    End Sub

    Function RestituisciPrenotati(dtAppuntamenti As DataTable) As Filtri.FiltroCollection
        '--
        Dim retFilter As New Filtri.FiltroCollection()
        '--
        For Each r As DataRow In dtAppuntamenti.Rows
            '--
            Dim f As New Filtri.FiltroOccupato(r("CNV_DATA_APPUNTAMENTO").date, r("CNV_DATA_APPUNTAMENTO"), r("CNV_DATA_APPUNTAMENTO").addminutes(r("CNV_DURATA_APPUNTAMENTO")))
            '--
            retFilter.Add(f)
            '--
        Next
        '--
        Return retFilter
        '--
    End Function

    Function CaricaFiltriFeste(ambCodice As Integer) As Filtri.FiltroCollection
        '--
        Dim dta As DataTable
        '--
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            dta = genericProvider.Prenotazioni.GetFestivita(ambCodice)
        End Using
        '--
        Dim filters As New Filtri.FiltroCollection()
        '--
        For Each r As DataRow In dta.Rows
            '--
            Dim f As New Filtri.FiltroIndisponibilitaGenerica
            Dim data As Date = r("DATA")
            '--
            If (Not r("RICORSIVITA") Is DBNull.Value AndAlso r("RICORSIVITA").ToString() = OnVac.Constants.CommonConstants.RECURSIVE_YEAR) Then
                f.Ricorsivita = Filtri.Ricorsivita.Annuale
            Else
                f.Ricorsivita = Filtri.Ricorsivita.Singola
            End If
            '--
            f.Anno = data.Year
            f.Mese = data.Month
            f.Giorno = data.Day
            '--
            If Not r("INIZIO") Is DBNull.Value Then
                f.DaOra = r("INIZIO")
                f.AOra = r("FINE")
            Else
                f.DaOra = Timing.Day.MinOrario
                f.AOra = Timing.Day.MaxOrario
            End If
            '--
            filters.Add(f)
            '--
        Next
        '--
        Return filters
        '--
    End Function

    Function CaricaFiltriApertura(ambCodice As Integer, fuoriOrario As Boolean, giorno As DayOfWeek, parte As PartiGiornata) As Filtri.FiltroCollection

        Dim f As New Filtri.FiltroCollection()

        Dim dta As DataTable = GetFiltriApertura(ambCodice, fuoriOrario, giorno)

        Dim oraPm As Object = Me.Settings.ORAPM
        Dim oraAm As Object = Date.Parse("08:00")
        If oraPm Is Nothing Then oraPm = "13:00"

        oraPm = Date.Parse(oraPm)

        If dta.Rows.Count > 0 Then

            For Each r As DataRow In dta.Rows

                Dim defMat As Boolean = Not r("AM_INIZIO") Is DBNull.Value AndAlso Not r("AM_FINE") Is DBNull.Value
                Dim defPom As Boolean = Not r("PM_INIZIO") Is DBNull.Value AndAlso Not r("PM_FINE") Is DBNull.Value

                If parte = PartiGiornata.Mattina Then
                    If defMat And defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), Timing.Day.MinOrario, r("AM_INIZIO")))
                        f.Add(CreaOrarioApertura(r("GIORNO"), r("PM_INIZIO"), Timing.Day.MaxOrario))
                    ElseIf Not defMat AndAlso Not defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), Timing.Day.MinOrario, oraAm))
                        f.Add(CreaOrarioApertura(r("GIORNO"), oraPm, Timing.Day.MaxOrario))
                    ElseIf defMat AndAlso Not defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), Timing.Day.MinOrario, r("AM_INIZIO")))
                        f.Add(CreaOrarioApertura(r("GIORNO"), oraPm, Timing.Day.MaxOrario))
                    ElseIf Not defMat AndAlso defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), Timing.Day.MinOrario, oraAm))
                        f.Add(CreaOrarioApertura(r("GIORNO"), r("PM_INIZIO"), Timing.Day.MaxOrario))
                    End If
                ElseIf parte = PartiGiornata.Pomeriggio Then
                    If defMat And defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), Timing.Day.MinOrario, r("PM_INIZIO")))
                    ElseIf Not defMat AndAlso Not defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), Timing.Day.MinOrario, oraPm))
                    ElseIf defMat AndAlso Not defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), Timing.Day.MinOrario, oraPm))
                    ElseIf Not defMat AndAlso defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), Timing.Day.MinOrario, r("PM_INIZIO")))
                    End If
                Else
                    If defMat And defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), Timing.Day.MinOrario, r("AM_INIZIO")))
                        f.Add(CreaOrarioApertura(r("GIORNO"), r("AM_FINE"), r("PM_INIZIO")))
                        f.Add(CreaOrarioApertura(r("GIORNO"), r("PM_FINE"), Timing.Day.MaxOrario))
                    ElseIf Not defMat AndAlso Not defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), Timing.Day.MinOrario, Timing.Day.MaxOrario))
                    ElseIf defMat AndAlso Not defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), Timing.Day.MinOrario, r("AM_INIZIO")))
                        f.Add(CreaOrarioApertura(r("GIORNO"), r("AM_FINE"), Timing.Day.MaxOrario))
                    ElseIf Not defMat AndAlso defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), Timing.Day.MinOrario, r("PM_INIZIO")))
                        f.Add(CreaOrarioApertura(r("GIORNO"), r("PM_FINE"), Timing.Day.MaxOrario))
                    End If
                End If
            Next
        Else
            ' non sono impostati gli orari di apertura
            For i As Integer = 0 To 6
                f.Add(CreaOrarioApertura(i, Timing.Day.MinOrario, Timing.Day.MaxOrario))
            Next
        End If

        Return f

    End Function

    Function CreaOrarioApertura(giorno As DayOfWeek, minDate As Date, maxDate As Date) As Filtri.IFiltro

        Dim fAp As New Filtri.FiltroIndisponibilitaGenerica()

        fAp.Ricorsivita = Filtri.Ricorsivita.Settimanale
        fAp.GiornoSettimana = giorno
        fAp.DaOra = minDate
        fAp.AOra = maxDate

        Return fAp

    End Function

#End Region

#Region " Private/Protected Methods "

    ' Calcola i valori dei parametri SED_MANU, TEMPOBIL e TEMPOSED in base al consultorio di appuntamento
    ' e li utilizza per impostare la durata dell'appuntamento. Restituisce i valori dei parametri utilizzati.
    Private Function ImpostaDurataAppuntamento() As Biz.BizRicercaAppuntamenti.ParametriConsultorioAppuntamento

        Dim parametriConsultorioAppuntamento As New Biz.BizRicercaAppuntamenti.ParametriConsultorioAppuntamento()

        If Me.uscScegliAmb.cnsCodice <> OnVacUtility.Variabili.CNS.Codice Then

            ' Caricamento SED_MANU, TEMPOBIL e TEMPOSED relativi al consultorio in cui viene dato l'appuntamento
            parametriConsultorioAppuntamento = Me.GetParametriConsultorioAppuntamento()

        Else

            ' Valori dei parametri SED_MANU, TEMPOBIL e TEMPOSED relativi al consultorio corrente
            parametriConsultorioAppuntamento.SedManu = Me.Settings.SED_MANU
            parametriConsultorioAppuntamento.TempoBil = Me.Settings.TEMPOBIL
            parametriConsultorioAppuntamento.TempoSed = Me.Settings.TEMPOSED

        End If

        Me.CaricaDurataAppuntamento(parametriConsultorioAppuntamento.SedManu, parametriConsultorioAppuntamento.TempoBil)

        Me.textNuovaDurata.Text = Me.p_DurataAppuntamento

        Return parametriConsultorioAppuntamento

    End Function

    ' Caricamento valori dei parametri relativi al consultorio in cui viene dato l'appuntamento
    Private Function GetParametriConsultorioAppuntamento() As Biz.BizRicercaAppuntamenti.ParametriConsultorioAppuntamento

        Dim parametriConsultorioAppuntamento As Biz.BizRicercaAppuntamenti.ParametriConsultorioAppuntamento

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizRicercaAppuntamenti As New Biz.BizRicercaAppuntamenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                parametriConsultorioAppuntamento = bizRicercaAppuntamenti.GetParametriConsultorioAppuntamento(Me.uscScegliAmb.cnsCodice)

            End Using
        End Using

        Return parametriConsultorioAppuntamento

    End Function

    Private Sub Salva(idMotivoModificaAppuntamento As String, noteModificaAppuntamento As String)

        Dim result As BizRicercaAppuntamenti.SalvaAppuntamentiResult = Nothing

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizRicercaAppuntamenti As New BizRicercaAppuntamenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(OnVacUtility.Variabili.CNS.Codice), Nothing)

                Dim command As New BizRicercaAppuntamenti.SalvaAppuntamentiCommand()
                command.CodicePaziente = OnVacUtility.Variabili.PazId
                command.DataConvocazione = Me.p_DataConvocazione

                command.EliminaBilancio = Me.EliminaBilancio

                If Me.EliminaBilancio Then
                    command.DataAppuntamento = Nothing ' in questo caso non la usa
                    command.NoteAppuntamento = "Eliminazione appuntamento (con bilancio) da maschera RicercaAppuntamenti"
                    command.IdMotivoEliminazioneAppuntamento = Constants.MotiviEliminazioneAppuntamento.EliminazioneConvocazione
                Else
                    ' Logica della vecchia versione
                    If String.IsNullOrWhiteSpace(Me.pickNuovaData.Text) Then
                        command.DataAppuntamento = Nothing
                        command.NoteAppuntamento = "Eliminazione appuntamento da maschera RicercaAppuntamenti"
                    Else
                        command.DataAppuntamento = Date.Parse(Me.DataApp & " " & Me.OraApp)
                        command.NoteAppuntamento = "Prenotazione appuntamento da maschera RicercaAppuntamenti"
                    End If
                    command.IdMotivoEliminazioneAppuntamento = idMotivoModificaAppuntamento
                End If

                'If command.DataAppuntamento Is Nothing Then
                '    ' ELIMINAZIONE APPUNTAMENTO => devo memorizzare le note dell'utente nelle NoteAppuntamento
                '    command.NoteAppuntamento += ": " + noteModificaAppuntamento
                'Else
                '    ' SPOSTAMENTO APPUNTAMENTO => devo memorizzare le note dell'utente nelle NoteSpostamentoAppuntamento
                '    command.NoteSpostamentoAppuntamento = noteModificaAppuntamento
                'End If
                command.NoteUtenteModificaAppuntamento = noteModificaAppuntamento
                command.DurataAppuntamento = p_DurataAppuntamento
                command.CodiceConsultorioAppuntamento = Me.uscScegliAmb.cnsCodice
                command.CodiceAmbulatorio = CodiceAmbulatorioCorrente
                command.IsSoloBilancio = controllaPazSoloBilancio
                'command.IdConvocazione = p_dtaDateAppuntamenti.Rows(dltDateAppuntamenti.SelectedIndex)("CNV_ID_CONVOCAZIONE")

                result = bizRicercaAppuntamenti.SalvaAppuntamento(command)

                If result.Success Then

                    If result.AppuntamentoSpostato Then
                        Me.appuntamentoSpostato = True
                    End If

                    If Not Me.EliminaBilancio Then
                        ' TODO: ma questo campo serve???
                        'stato di assegnazione per la modifica del consultorio di appuntamento [modifica 09/05/2005]
                        Me.txtStatoBtnAssegna.Text = "N"
                    End If

                Else
                    SetMessageToLabelWarning(TipoMessaggioLabelWarning.None, False, "E' avvenuto un errore durante il salvataggio: " + result.Message, False)
                End If

                AggiornaInformazioni(genericProvider)

            End Using
        End Using

        Me.OnitLayout31.Busy = False

        'abilito nuovamente i campi appuntamento
        AbilitaRicercaAppuntamento(True)

        'se tutti gli appuntamenti sono stati spostati (o se è stato prenotato un appuntamento 
        'su un altro cns), è necessario riproporre la maschera come al primo caricamento
        If Me.dltDateAppuntamenti.Items.Count = 0 Or Me.EliminaBilancio Or Me.uscScegliAmb.cnsCodice <> OnVacUtility.Variabili.CNS.Codice Then

            Me.EliminaBilancio = False

            Dim queryStringLivello As String = String.Empty

            Dim levelCNV As String = Request.QueryString.Get("levelCNV")
            If Not String.IsNullOrEmpty(levelCNV) Then queryStringLivello = String.Format("levelCNV={0}&", levelCNV)

            Page.Server.Transfer(String.Format("RicercaAppuntamenti.aspx?{0}cns={1}&amb={2}", queryStringLivello, Me.uscScegliAmb.cnsCodice, Me.uscScegliAmb.ambCodice))

        Else

            'ricarica le informazione della convocazione selezionata
            If Me.dltDateAppuntamenti.SelectedIndex >= 0 AndAlso Me.dltDateAppuntamenti.Items.Count > 0 Then
                SelezionaDataAppuntamento()
            End If

            AbilitaPulsantiConfermaAnnulla(False)

            'riabilitazione delle date di appuntamento
            DisabilitaDltDateAppuntamenti(False)

        End If

    End Sub

    ' Cancella i dati dopo la modifica di un ambulatorio
    Private Sub CancellaDatiPerModificaAmbulatorio()

        BindDataListOrariDisponibili(False)

        Me.p_dtaFestivita.Rows.Clear()

        Me.pickNuovaData.Text = String.Empty
        Me.textNuovaOra.Text = String.Empty

    End Sub

    Private Sub SetCnsAmbAppuntamento(codiceCnsAppuntamento As String, codiceAmbAppuntamento As String)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            SetCnsAmbAppuntamento(codiceCnsAppuntamento, codiceAmbAppuntamento, genericProvider)

        End Using

    End Sub

    Private Sub SetCnsAmbAppuntamento(codiceCnsAppuntamento As String, codiceAmbAppuntamento As String, genericProvider As DAL.DbGenericProvider)

        Dim cnsAppImpostato As Boolean = False
        Dim ambAppImpostato As Boolean = False

        ' Impostazione consultorio (se codice presente e trovo la descrizione corrispondente)
        If Not String.IsNullOrEmpty(codiceCnsAppuntamento) Then

            Dim descrizioneCnsAppuntamento As String = String.Empty

            If codiceCnsAppuntamento = OnVacUtility.Variabili.CNS.Codice Then

                ' Se il codice del cns di appuntamento è lo stesso di quello di lavoro, 
                ' prendo la descrizione direttamente dal campo Nome
                descrizioneCnsAppuntamento = OnVacUtility.Variabili.CNS.Descrizione

            Else

                ' Caricamento descrizione consultorio
                descrizioneCnsAppuntamento = genericProvider.Consultori.GetCnsDescrizione(codiceCnsAppuntamento)

            End If

            ' Se ho trovato anche la descrizione, imposto cod e descr del consultorio
            If Not String.IsNullOrEmpty(descrizioneCnsAppuntamento) Then

                Me.uscScegliAmb.cnsCodice = codiceCnsAppuntamento
                Me.uscScegliAmb.cnsDescrizione = descrizioneCnsAppuntamento

                cnsAppImpostato = True

                ' Impostazione ambulatorio 

                If String.IsNullOrEmpty(codiceAmbAppuntamento) Then

                    ' Impostazione ambulatorio in base all'ambulatorio di convocazione, selezionato nelle programmate (se valorizzato e facente parte del consultorio corrente)
                    If Me.Settings.RICERCA_APP_SET_AMB_CONVOCAZIONE AndAlso
                       OnVacUtility.Variabili.AMBConvocazione.HasValue AndAlso
                       codiceCnsAppuntamento = OnVacUtility.Variabili.CNS.Codice Then

                        ' N.B. : non controllo che l'ambulatorio selezionato appartenga al consultorio di appuntamento ma che il consultorio di appuntamento sia quello corrente, 
                        ' perchè non deve essere possibile (nelle programmate) selezionare un ambulatorio di un consultorio diverso da quello corrente.
                        codiceAmbAppuntamento = OnVacUtility.Variabili.AMBConvocazione.Value.ToString()

                    End If

                End If

                If Not String.IsNullOrEmpty(codiceAmbAppuntamento) Then

                    ' Se codice presente e trovo la descrizione corrispondente, imposto l'ambulatorio
                    ambAppImpostato = ImpostaAmbulatorioAppuntamento(codiceAmbAppuntamento, genericProvider)

                End If

            End If

        End If

        ' Di default, se cns non impostato, lo imposto a quello di lavoro
        If Not cnsAppImpostato Then
            Me.uscScegliAmb.cnsDescrizione = OnVacUtility.Variabili.CNS.Descrizione
            Me.uscScegliAmb.cnsCodice = OnVacUtility.Variabili.CNS.Codice
        End If

        If Not ambAppImpostato Then

            ' Se consultorio appuntamento = consultorio corrente, 
            ' imposto l'ambulatorio di convocazione (se è stato selezionato nelle programmate e se è previsto dalla configurazione).
            If Me.Settings.RICERCA_APP_SET_AMB_CONVOCAZIONE AndAlso
               OnVacUtility.Variabili.AMBConvocazione.HasValue AndAlso
               Me.uscScegliAmb.cnsCodice = OnVacUtility.Variabili.CNS.Codice Then

                ' Lettura descrizione ambulatorio in base al codice e impostazione ambulatorio appuntamento
                ambAppImpostato = ImpostaAmbulatorioAppuntamento(OnVacUtility.Variabili.AMBConvocazione.Value.ToString(), genericProvider)

            End If

            ' Se ambulatorio non impostato, seleziono quello di default:
            ' se ce n'è più di uno associato al cns selezionato, imposto a "Tutti" 
            ' se c'è un solo ambulatorio, lo imposto in automatico
            If Not ambAppImpostato Then
                '--
                Try
                    '--
                    Using bizConsultori As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                        '--
                        Dim ambulatorio As Entities.Ambulatorio = bizConsultori.GetAmbulatorioDefault(Me.uscScegliAmb.cnsCodice, True, True)
                        '--
                        Me.uscScegliAmb.ambCodice = ambulatorio.Codice
                        Me.uscScegliAmb.ambDescrizione = ambulatorio.Descrizione
                        '--
                    End Using
                    '--
                Catch ex As Exception
                    MostraErrore(ex.ToString())
                End Try
                '--
            End If

        End If

        Me.uscScegliAmb.databind()

    End Sub

    Private Function ImpostaAmbulatorioAppuntamento(codiceAmbAppuntamento As String, genericProvider As DAL.DbGenericProvider) As Boolean

        ' Lettura descrizione ambulatorio in base al codice
        Dim descrizioneAmbAppuntamento As String = genericProvider.Consultori.GetAmbDescrizione(codiceAmbAppuntamento)

        If Not String.IsNullOrEmpty(descrizioneAmbAppuntamento) Then

            Me.uscScegliAmb.ambCodice = codiceAmbAppuntamento
            Me.uscScegliAmb.ambDescrizione = descrizioneAmbAppuntamento

            Return True

        End If

        Return False

    End Function

    ' Restituisce true se c'è una convocazione selezionata su un consultorio diverso da quello corrente
    ' e il livello dell'utente o il parametro CONVOCAZIONI_ALTRI_CONSULTORI non permettono la modifica
    Private Function SelezionataCnvAltroCnsSolaVisualizzazione() As Boolean

        ' Se non c'è nessuna cnv selezionata -> RETURN FALSE
        If Me.dltDateAppuntamenti.SelectedItem Is Nothing Then Return False

        ' Se la convocazione selezionata è sul consultorio corrente -> RETURN FALSE
        Dim cnsCnvSelezionata As String =
            DirectCast(Me.dltDateAppuntamenti.SelectedItem.FindControl("lblCnsCnvSelezionata"), Label).Text

        If cnsCnvSelezionata = OnVacUtility.Variabili.CNS.Codice Then
            Return False
        End If

        ' Controllo livello del singolo utente
        If Me.LivelloUtenteConvocazione <> Enumerators.LivelloUtenteConvocazione.Undefined Then

            ' Se il livello dell'utente è specificato e non è Administrator -> RETURN TRUE
            ' Se il livello dell'utente è Administrator -> RETURN FALSE
            Return (Me.LivelloUtenteConvocazione <> Enumerators.LivelloUtenteConvocazione.Administrator)

        End If

        ' Controllo parametro globale
        If Me.Settings.CONVOCAZIONI_ALTRI_CONSULTORI <> Enumerators.TipoGestioneConvocazioniAltriConsultori.Modificabili Then

            ' Se il livello dell'utente non è specificato e il parametro non vale "Modificabili" -> RETURN TRUE
            Return True

        End If

        Return False

    End Function

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of PrintButton)()
        listPrintButtons.Add(New PrintButton(Constants.ReportName.BilancioInBianco, "btnBilancioBianco"))

        ShowToolbarPrintButtons(listPrintButtons, ToolBar)

        Dim rptList As New List(Of String)()
        rptList.Add(Constants.ReportName.AvvisoAppuntamento)
        rptList.Add(Constants.ReportName.AvvisoSoloBilancio)

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizRpt As New BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                ToolBar.Items.FromKeyButton("btnStampaAppuntamento").Visible = bizRpt.ExistsReportList(rptList)

            End Using
        End Using

    End Sub

    Protected Function AbilitaImageDisponibilitaOrario(dlItem As DataListItem) As Boolean

        'Nel caso in cui si tratti di una riga del datatable con vaccinazioni (e quindi una prenotazione già fatta)
        'non deve essere possibile cliccare l'immagine per la selezione
        If dlItem.DataItem.Row.Item("Vaccinazioni") Is Nothing OrElse
           dlItem.DataItem.Row.Item("Vaccinazioni") Is DBNull.Value OrElse
           Not String.IsNullOrWhiteSpace(dlItem.DataItem.Row.Item("Vaccinazioni")) Then

            Return False

        End If

        Return True

    End Function

    'controlla se l'orario coincide con quello di appuntamento
    Protected Function GetImageUrlDisponibilitaOrario(dlItem As DataListItem) As String

        If dlItem.DataItem.Row.Item("Vaccinazioni") Is Nothing OrElse
           dlItem.DataItem.Row.Item("Vaccinazioni") Is DBNull.Value OrElse
           Not String.IsNullOrWhiteSpace(dlItem.DataItem.Row.Item("Vaccinazioni")) Then
            '--
            'Nel caso in cui si tratti di una riga del datatable con vaccinazioni (e quindi una prenotazione già fatta)
            'non deve essere visualizzata l'immagine per selezionare
            Return Me.UrlIconaTrasparente
            '--
        End If

        Dim ambCodice As Integer = CInt(dlItem.DataItem.Row.Item("ambCodice"))

        If Not Me.p_dtaOrariAppuntamento.Contains(ambCodice) Then
            Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

                'caricamento datatable di sessione contenente gli orari d'appuntamento del consultorio
                Me.p_dtaOrariAppuntamento.Add(ambCodice, genericProvider.Prenotazioni.GetOrariAppuntamento(ambCodice))

            End Using
        End If

        Dim dtaOrariAppuntamento As DataTable = Me.p_dtaOrariAppuntamento(ambCodice)

        If dtaOrariAppuntamento.Rows.Count = 0 Then Return Me.UrlIconaSelezione

        Dim dataApp As Date = DataBinder.Eval(dlItem.DataItem, "Data")
        Dim oraApp As DateTime = CDate(DataBinder.Eval(dlItem.DataItem, "Ora")).ToString("HH:mm")

        For count As Integer = 0 To dtaOrariAppuntamento.Rows.Count - 1

            Dim oraAMInizio As DateTime
            Dim oraAMFine As DateTime
            Dim oraPMInizio As DateTime
            Dim oraPMFine As DateTime

            If dtaOrariAppuntamento.Rows(count)("ora_giorno") = CInt(dataApp.DayOfWeek) Then

                If Not dtaOrariAppuntamento.Rows(count)("ora_am_inizio") Is DBNull.Value Then

                    oraAMInizio = CDate(dtaOrariAppuntamento.Rows(count)("ora_am_inizio")).ToString("HH:mm")
                    oraAMFine = CDate(dtaOrariAppuntamento.Rows(count)("ora_am_fine")).ToString("HH:mm")

                    If (oraApp >= oraAMInizio) And (oraApp <= oraAMFine) Then Return Me.UrlIconaSelezioneApp

                End If

                If Not dtaOrariAppuntamento.Rows(count)("ora_pm_inizio") Is DBNull.Value Then

                    oraPMInizio = CDate(dtaOrariAppuntamento.Rows(count)("ora_pm_inizio")).ToString("HH:mm")
                    oraPMFine = CDate(dtaOrariAppuntamento.Rows(count)("ora_pm_fine")).ToString("HH:mm")

                    If (oraApp >= oraPMInizio) And (oraApp <= oraPMFine) Then Return Me.UrlIconaSelezioneApp

                End If

            End If

        Next

        Return Me.UrlIconaSelezione

    End Function

    'Aggiorna la visualizzazione delle informazioni sulle convocazioni del paziente
    Sub AggiornaInformazioni(ByRef genericProvider As DbGenericProvider)

        p_dtaDateAppuntamenti.Rows.Clear()

        Using bizCnv As New BizConvocazione(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)

            p_dtaDateAppuntamenti = bizCnv.GetDtConvocazioniAppuntamentiPaziente(OnVacUtility.Variabili.PazId,
                                                                                    OnVacUtility.Variabili.CNS.Codice,
                                                                                    Me.LivelloUtenteConvocazione)
        End Using

        dltDateAppuntamenti.DataSource = p_dtaDateAppuntamenti
        dltDateAppuntamenti.DataBind()

    End Sub

    'ritorna il bilancio associato alla convocazione (modifica 05/08/2004)
    Function CercaBilanci(codicePaziente As Integer, dataConvocazione As Date) As DataTable

        Dim dt As DataTable = Nothing

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            Using bizBil As New BizBilancioProgrammato(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                dt = bizBil.CercaBilanciDt(codicePaziente, dataConvocazione)
            End Using

        End Using

        Return dt

    End Function

    Protected Function GetDescrizioneBilancioMalattia(dataConvocazione As Date) As String

        Dim descrizione As String = String.Empty

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            Using bizBilanci As New BizBilancioProgrammato(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                descrizione = bizBilanci.GetDescrizioneBilancioMalattia(OnVacUtility.Variabili.PazId, dataConvocazione, Constants.StatiBilancio.UNEXECUTED)

            End Using

        End Using

        Return descrizione

    End Function

    Private Sub AbilitaRicercaAppuntamento(enable As Boolean)

        Me.odpDispInizio.Enabled = enable
        Me.odpDispInizio.CssClass = IIf(enable, "TextBox_Data_Obbligatorio", "TextBox_Data_Disabilitato")

        Me.odpDispFine.Enabled = enable
        Me.odpDispFine.CssClass = IIf(enable, "TextBox_Data_Obbligatorio", "TextBox_Data_Disabilitato")

        Me.btnCercaData.Enabled = enable

        If Not enable Then
            Me.pickNuovaData.Text = String.Empty
            Me.textNuovaOra.Text = String.Empty
            Me.textNuovaDurata.Text = String.Empty
        End If

        GestioneAbilitazioneCalendario()

    End Sub

    'controlla se la convocazione del paziente ha almeno un bilancio e il paziente è senza pediatra [modifica 21/02/2007] CMR
    Private Function PazienteSenzaPediatra() As Boolean

        Dim pazSenzaPediatra As Boolean = False

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            Using ricercaAppBiz As New Biz.BizRicercaAppuntamenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                pazSenzaPediatra = ricercaAppBiz.PazienteSenzaPediatra(OnVacUtility.Variabili.PazId, p_DataConvocazione.ToShortDateString())

            End Using

        End Using

        Return pazSenzaPediatra

    End Function

    'controlla se la convocazione del paziente è per la sola consegna del bilancio [modifica 16/02/2005]
    Private Function PazienteSoloBilancio() As Boolean

        Dim dtBil As DataTable = Nothing

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            Using bizBil As New BizBilancioProgrammato(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Try
                    Return bizBil.VaccinazioneAssociata(OnVacUtility.Variabili.PazId, Me.p_DataConvocazione.ToShortDateString())
                Catch ex As Exception
                    SetMessageToLabelWarning(TipoMessaggioLabelWarning.Error, False, "È avvenuto un errore in <b>PazienteSoloBilancio</b>: " & ex.Message, False)
                End Try

            End Using

        End Using

        Return False

    End Function

    'controlla se la convocazione ha la data di invio valorizzata [modifica 03/02/2005]
    Private Function DataInvioValorizzata(dataConvocazione As Date)

        If dataConvocazione = Nothing Then Return False

        Dim dataInvio As Date = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            dataInvio = genericProvider.Convocazione.GetDataInvio(OnVacUtility.Variabili.PazId, dataConvocazione)
        End Using

        If dataInvio = New Date Then Return False

        Return True

    End Function

    Private Function CountTotaleAppuntamenti(dataSelezionata As DateTime, ambSelezionato As Integer, appuntamentiTotali As List(Of AppuntamentoPrenotato)) As Integer

        Dim countAppuntamenti As Integer = 0

        If appuntamentiTotali IsNot Nothing AndAlso appuntamentiTotali.Count > 0 Then

            'Recupero gli appuntamenti nella data selezionata
            Dim lstApp As IEnumerable(Of AppuntamentoPrenotato) = appuntamentiTotali.Where(Function(app) app.CNV_DATA_APPUNTAMENTO.Date = dataSelezionata)

            'Nel caso in cui sia selezionato un ambulatorio, devo filtrare anche per il suo codice
            If ambSelezionato > 0 Then
                lstApp = lstApp.Where(Function(app) app.AMB_CODICE = ambSelezionato)
            End If

            countAppuntamenti = lstApp.Count()

        End If

        Return countAppuntamenti

    End Function

    Private Sub BindDataListOrariDisponibili(mostraOrari As Boolean)

        If mostraOrari Then
            '--
            Dim currentDay As String = String.Empty
            Dim totAppuntamenti As String = String.Empty
            '--
            If Me.GiorniAppuntamentiDisponibili.HasValue Then
                '--
                ' Ci sono appuntamenti disponibili, che vanno visualizzati divisi per giorno
                '--
                Dim dataSelezionata As DateTime = Me.GiorniAppuntamentiDisponibili.DataSelezionata
                currentDay = GetCurrentDayDescription(dataSelezionata, Me.GiorniAppuntamentiDisponibili.IndiceGiornoSelezionato + 1, Me.GiorniAppuntamentiDisponibili.Count)
                '--
                totAppuntamenti = CountTotaleAppuntamenti(dataSelezionata, Me.CodiceAmbulatorioCorrente, Me.AppuntamentiTotali).ToString()
                '--
                Me.p_dtaOrariValidi.DefaultView.Sort = "Ora"
                Me.p_dtaOrariValidi.DefaultView.RowFilter = String.Format("Data='{0}'", dataSelezionata)
                '--
            Else
                '--
                ' In questo caso, ci sono 2 possibilità: 
                ' 1 - non ci sono disponibilità nè appuntamenti già presi, 
                ' 2 - non ci sono disponibilità ma ci sono appuntamenti già presi (che vanno visualizzati, divisi per giorni).
                '--
                If Me.GiorniAppuntamentiPrenotati.HasValue Then
                    Me.p_dtaOrariValidi.DefaultView.Sort = "Ora"
                    Me.p_dtaOrariValidi.DefaultView.RowFilter = String.Format("Data='{0}'", Me.GiorniAppuntamentiPrenotati.DataSelezionata)
                End If

                If Not Me.p_dtaOrariValidi.DefaultView Is Nothing Then
                    '--
                    If Me.p_dtaOrariValidi.DefaultView.Count > 0 Then
                        '--
                        Dim dataSelezionata As DateTime = DirectCast(Me.p_dtaOrariValidi.DefaultView(0)("Data"), DateTime)
                        currentDay = GetCurrentDayDescription(dataSelezionata, 1, 1)
                        '--
                    End If
                    '--
                    totAppuntamenti = Me.p_dtaOrariValidi.DefaultView.Count.ToString()
                    '--
                End If
            End If
            '--
            If Me.odpDispInizio.Data = Me.odpDispFine.Data Then
                currentDay = GetCurrentDayDescription(Me.odpDispInizio.Data, 1, 1)
            End If
            '--
            Me.lblCurrentDay.Text = currentDay
            '--
            Me.dltOrariDisponibili.DataSource = Me.p_dtaOrariValidi.DefaultView
            Me.dltOrariDisponibili.DataBind()
            '--
            Me.lblTotAppuntamenti.Text = totAppuntamenti
            '--
            Me.GestisciButtonsDateDisponibili(False)
            '--
            'Aggiornamento del calendario nel caso in cui sia aperto
            CaricaCalendario()
            '--
        Else
            '--
            Me.dltOrariDisponibili.Visible = False
            Me.dltOrariDisponibili.SelectedIndex = -1
            Me.dltOrariDisponibili.DataSource = Nothing
            Me.dltOrariDisponibili.DataBind()
            '--
        End If

    End Sub

    Private Function GetCurrentDayDescription(dataSelezionata As DateTime, numeroGiornoCorrente As Integer, totaleGiorniDisponibili As Integer) As String

        Return String.Format("{0} {1} ({2}/{3})", dataSelezionata.ToString("ddd"), dataSelezionata.ToString("dd/MM/yyyy"), numeroGiornoCorrente.ToString(), totaleGiorniDisponibili.ToString())

    End Function

    Private Sub AbilitaPulsantiConfermaAnnulla(enable As Boolean)

        ' Se c'è una convocazione selezionata, controllo se è su un consultorio diverso da quello corrente
        ' In questo caso non devo permettere la prenotazione
        If Me.SelezionataCnvAltroCnsSolaVisualizzazione() Then enable = False

        Me.ToolBar.Items.FromKeyButton("btnConferma").Enabled = enable
        Me.ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = enable

    End Sub

    'disabilita/abilita il datalist delle date appuntamento [modifica 03/02/2005]
    Private Sub DisabilitaDltDateAppuntamenti(enable As Boolean)

        For count As Integer = 0 To Me.dltDateAppuntamenti.Items.Count - 1
            DirectCast(Me.dltDateAppuntamenti.Items(count).FindControl("ImageButton1"), System.Web.UI.WebControls.Image).Enabled = Not enable
            dltDateAppuntamenti.Enabled = Not enable
        Next

        Me.disabilitaElimPrenotazione = enable
        Me.disabilitaElimSoloBilancio = enable

    End Sub

    Private Function GetCartellaReport(nomeReport As String) As String

        Dim folder As String = String.Empty

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                folder = bizReport.GetReportFolder(nomeReport)

            End Using
        End Using

        Return folder

    End Function

    Private Sub ImpostaLayoutControlliAssegnazione()

        Dim enable As Boolean = Me.CodiceAmbulatorioCorrente > 0 AndAlso
                                Me.Settings.APPLIBERO AndAlso
                                Me.dltDateAppuntamenti.SelectedIndex >= 0

        Me.btnAssegna.Enabled = (Me.CodiceAmbulatorioCorrente > 0 AndAlso
                                (Me.Settings.APPLIBERO OrElse Not dltOrariDisponibili.SelectedItem Is Nothing) AndAlso
                                Me.dltDateAppuntamenti.SelectedIndex >= 0)

        ' Se la convocazione selezionata è in sola visualizzazione, 
        ' disabilito tutti i campi relativi all'assegnazione dell'appuntamento
        If SelezionataCnvAltroCnsSolaVisualizzazione() Then
            enable = False
            Me.btnAssegna.Enabled = False
        End If

        Me.pickNuovaData.Enabled = enable
        Me.textNuovaOra.ReadOnly = Not enable
        Me.textNuovaDurata.ReadOnly = Not enable

        If enable Then

            Me.pickNuovaData.CssClass = "Textbox_data_obbligatorio"
            Me.textNuovaOra.CssClass = "Textbox_data_obbligatorio"
            Me.textNuovaDurata.CssClass = "Textbox_numerico"

        Else

            Me.pickNuovaData.CssClass = "Textbox_data_disabilitato"
            Me.textNuovaOra.CssClass = "Textbox_data_disabilitato"
            Me.textNuovaDurata.CssClass = "Textbox_numerico_disabilitato"

        End If

    End Sub

    'Se la data scelta per l'appuntamento è antecedente la data di convocazione devo mostrare
    'un messaggio di alert per avvertire l'utente della situazione.
    Private Function AppuntamentoAnterioreConvocazione() As Boolean

        Dim dataApp As DateTime = pickNuovaData.Data
        Dim dataCnv As DateTime = p_DataConvocazione

        If DateTime.op_LessThan(dataApp, dataCnv) Then
            Return True
        End If

        Return False

    End Function

    Sub MostraErrore(strErrore As String)

        Me.lblErrore.Text &= strErrore
        Me.fmError.VisibileMD = True

    End Sub

    Private Function GetFiltriApertura(ambCodice As Integer, fuoriOrario As Boolean, giorno As DayOfWeek) As DataTable

        Dim dta As New DataTable()
        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using ricercaAppBiz As New Biz.BizRicercaAppuntamenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                dta = genericProvider.Prenotazioni.GetFiltriApertura(ambCodice, fuoriOrario, giorno)
            End Using
        End Using

        Return dta

    End Function

#Region " Pulsanti scorrimento date disponibili "

    'gestione dei bottoni per spostarsi tra i giorni disponibili per l'appuntamento
    Private Sub GestisciButtonsDateDisponibili(clearCurrentDay As Boolean)

        Dim index As Integer
        Dim count As Integer

        If Me.GiorniAppuntamentiDisponibili.HasValue Then

            index = Me.GiorniAppuntamentiDisponibili.IndiceGiornoSelezionato
            count = Me.GiorniAppuntamentiDisponibili.Count
            
        Else

            If clearCurrentDay Then
                Me.lblCurrentDay.Text = String.Empty
            End If

            If Me.GiorniAppuntamentiPrenotati.HasValue Then
                index = Me.GiorniAppuntamentiPrenotati.IndiceGiornoSelezionato
                count = Me.GiorniAppuntamentiPrenotati.Count
            Else
                ' disabilita tutti i pulsanti
                index = -1
                count = 0
            End If

        End If

        EnableButtonDateDisponibili(Me.btnFirst, index > 0, "first")
        EnableButtonDateDisponibili(Me.btnNext, index + 1 < count, "next")
        EnableButtonDateDisponibili(Me.btnPrev, index > 0, "prev")
        EnableButtonDateDisponibili(Me.btnLast, index <> count - 1, "last")

    End Sub

    Private Sub EnableButtonDateDisponibili(btn As ImageButton, enable As Boolean, imageName As String)

        Dim dis As String = String.Empty

        If Not enable Then dis = "_dis"

        btn.Enabled = enable
        btn.ImageUrl = Me.ResolveClientUrl(String.Format("~/images/{0}{1}.gif", imageName, dis))

    End Sub

#End Region

#Region " Label Warning "

    Private Sub SetMessageToLabelWarning(tipoMessaggio As TipoMessaggioLabelWarning, addNewLineBefore As Boolean, messaggio As String, addNewLineAfter As Boolean)

        Me.lblWarning.Text = String.Empty

        AddMessageToLabelWarning(tipoMessaggio, addNewLineBefore, messaggio, addNewLineAfter)

    End Sub

    Private Sub AddMessageToLabelWarning(tipoMessaggio As TipoMessaggioLabelWarning, addNewLineBefore As Boolean, messaggio As String, addNewLineAfter As Boolean)

        If addNewLineBefore Then
            Me.lblWarning.Text += "<br/>"
        End If

        Select Case tipoMessaggio

            Case TipoMessaggioLabelWarning.None
                Me.lblWarning.Text += ""

            Case TipoMessaggioLabelWarning.Alert
                Me.lblWarning.Text += "<img src='../../images/alert.gif' align='absmiddle' /> "

            Case TipoMessaggioLabelWarning.Info
                Me.lblWarning.Text += "<img src='../../images/info.png' align='absmiddle' /> "

            Case TipoMessaggioLabelWarning.Deny
                Me.lblWarning.Text += "<img src='../../images/deny.png' align='absmiddle' /> "

            Case TipoMessaggioLabelWarning.Error
                Me.lblWarning.Text += "<img src='../../images/error.gif' align='absmiddle' /> "

        End Select

        Me.lblWarning.Text += messaggio

        If addNewLineAfter Then
            Me.lblWarning.Text += "<br/>"
        End If

    End Sub

#End Region

#End Region

#Region " Calendario "

    Private Sub btnCalendario_Click(sender As Object, e As EventArgs) Handles btnCalendario.Click

        If dypCalendario.Visible Then
            'Chiudi il calendario
            dypCalendario.Visible = False
            btnCalendario.ToolTip = "Apri calendario"
        Else
            'Apri il calendario
            ApriCalendario()
        End If

    End Sub

    Private Sub ApriCalendario()

        If Me.btnCercaData.Enabled AndAlso Me.CodiceAmbulatorioCorrente > 0 Then

            CaricaCalendario()
            dypCalendario.Visible = True
            btnCalendario.ToolTip = "Chiudi calendario"

        End If

    End Sub

    Private Sub Calendario_SelectionChanged(sender As System.Object, e As System.EventArgs) Handles Calendario.SelectionChanged

        Dim selectedDate As DateTime = Me.Calendario.SelectedDate

        If selectedDate.Date >= DateTime.Today Then

            If Me.GiorniAppuntamentiDisponibili.Contains(selectedDate) Then

                'La data selezionata dal calendario è presente in GiorniAppuntamentiDisponibili: la imposto come data selezionata e aggiorno il datalist degli orari disponibili.
                Me.GiorniAppuntamentiDisponibili.SetGiornoSelezionato(selectedDate)
                BindDataListOrariDisponibili(True)

            Else

                BindDataListOrariDisponibili(False)

                'La data selezionata dal calendario non è presente in GiorniAppuntamentiDisponibili: aggiorno il periodo di ricerca e rieffettuo la ricerca con il giorno selezionato
                Me.odpDispInizio.Data = selectedDate
                Me.odpDispFine.Data = selectedDate

                CercaPeriodiLiberi()

            End If

        Else
            'Se viene selezionata una data precedente a quella odierna, non si deve cambiare la selezione
            MostraErrore("La data selezionata è minore della data corrente !")
            SelezionaDataCalendario()
        End If

    End Sub

    Private Sub GestioneAbilitazioneCalendario()

        'Il pulsante del calendario è abilitato nel caso in cui sia abilitata la ricerca 
        'e non sia selezionato "Tutti" come ambulatorio del cv
        Dim enable As Boolean = Me.btnCercaData.Enabled AndAlso Me.CodiceAmbulatorioCorrente > 0

        Me.btnCalendario.Enabled = enable
        If enable Then
            Me.btnCalendario.CssClass = "btn-calendar"
        Else
            Me.btnCalendario.CssClass = "btn-calendar-disabled"
        End If

        If Not enable Then
            Me.dypCalendario.Visible = False
        End If

    End Sub

    ''' <summary>
    ''' Carica tutte le informazioni necessarie alla visualizzazione del calendario
    ''' (in questo metodo viene anche settato il giorno selezionato sul calendario)
    ''' </summary>
    Private Sub CaricaCalendario()

        'If Me.dypCalendario.Visible Then

        'Impostazione della data selezionata sul calendario
        Dim selectedDate As DateTime = SelezionaDataCalendario()

        'Carica tutte le informazioni necessarie alla visualizzazione del calendario
        AggiornaCalendario(selectedDate)

        'End If

    End Sub

    ''' <summary>
    ''' Carica tutte le informazioni necessarie alla visualizzazione del calendario
    ''' NOTA:Il calendario viene visualizzato e gestito solo quando c'è un solo amb selezionato
    ''' </summary>
    Private Sub AggiornaCalendario(selectedDate As DateTime)

        'Recupero primo ed ultimo giorno del mese
        Dim inizioMese As DateTime = New DateTime(selectedDate.Year, selectedDate.Month, 1)
        Dim fineMese As DateTime = New DateTime(selectedDate.Year, selectedDate.Month, DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month))

        'Recupero le informazioni sulle disponibilità e sugli appuntamenti prenotati
        Dim infoApp As CercaPeriodiLiberiResult = Me.CercaPeriodiLiberi(inizioMese, fineMese)

        'Set di tutte le info necessarie alla visualizzazione del calendario
        'Appuntamenti Prenotati
        If (infoApp.OrariValidi IsNot Nothing) Then
            AppuntamentiTotali_Calendario = infoApp.AppuntamentiTotali
        Else
            AppuntamentiTotali_Calendario = New List(Of AppuntamentoPrenotato)()
        End If

        'Festività/Indisponibilità
        If (p_dtaFestivita Is Nothing OrElse p_dtaFestivita.Rows.Count = 0) Then
            CaricaGiorniIndisponibilita(Me.CodiceAmbulatorioCorrente)
        End If

        'Disponibilità
        If (infoApp.OrariValidi IsNot Nothing) Then

            Dim listDisp As List(Of Disponibilita) = infoApp.OrariValidi.ConvertToList(Of Disponibilita)()

            If Not listDisp Is Nothing AndAlso listDisp.Count > 0 Then
                'infoApp.OrariValidi comprende anche i prenotati... Quindi filtro
                Disponibilita_Calendario = listDisp.Where(Function(d) String.IsNullOrWhiteSpace(d.Vaccinazioni)).ToList()
            Else
                Disponibilita_Calendario = New List(Of Disponibilita)()
            End If

        End If

    End Sub

    Private Function SelezionaDataCalendario() As DateTime

        Dim selectedDate As DateTime

        'Set della data odierna nel calendario
        Me.Calendario.TodaysDate = DateTime.Today

        ' Gestione data selezionata nel calendario:
        ' 1- Se presente prendo la data selezionata nella griglia delle disponibilità
        ' 2- Se non presente la (1) prendo la data di partenza di ricerca della disponibilità
        ' 3- Se non presente la (2) prendo la data odierna
        If Me.GiorniAppuntamentiDisponibili.HasValue Then

            selectedDate = Me.GiorniAppuntamentiDisponibili.DataSelezionata

        ElseIf Not String.IsNullOrWhiteSpace(Me.odpDispInizio.Text) AndAlso Me.odpDispInizio.Data <> DateTime.MinValue Then

            selectedDate = Me.odpDispInizio.Data

        Else

            selectedDate = DateTime.Today

        End If

        ' Impostazione della data selezionata nel calendario
        Me.Calendario.SelectedDate = selectedDate
        Me.Calendario.VisibleDate = selectedDate

        Return selectedDate

    End Function

    Private Sub Calendario_DayRender(sender As System.Object, e As System.Web.UI.WebControls.DayRenderEventArgs) Handles Calendario.DayRender

        Dim tooltip As New StringBuilder()
        Dim giorno As DateTime = e.Day.Date

        'Impostazione domenica
        If giorno.DayOfWeek = DayOfWeek.Sunday Then
            e.Cell.CssClass = "Festa"
            e.Cell.ToolTip = "Domenica"
        End If

        'Impostazione festività/indisponibilità
        If Not p_dtaFestivita Is Nothing AndAlso p_dtaFestivita.Rows.Count > 0 Then

            Dim cntData As String = giorno.ToString("dd/MM") & "/" & OnVac.Constants.CommonConstants.RECURSIVE_YEAR

            'Cerca le feste/indisponibilità
            Dim rowsIndisp As List(Of DataRow)
            rowsIndisp = p_dtaFestivita.Select("FES_DATA = Convert('" & cntData & "','System.DateTime')").ToList()
            If (rowsIndisp Is Nothing OrElse rowsIndisp.Count = 0) Then rowsIndisp = p_dtaFestivita.Select("FES_DATA = Convert('" & giorno & "','System.DateTime')").ToList()
            If (rowsIndisp IsNot Nothing AndAlso rowsIndisp.Count > 0) Then
                e.Cell.CssClass = "Indisponibile"

                For Each rowIndisp As DataRow In rowsIndisp
                    If (rowIndisp.Item("TIPO") = 1) Then
                        'Nel caso di fasce di indisponibilità viene aggiunto anche l'orario nel tooltip
                        Dim oraInizio As DateTime = rowIndisp.Item("INIZIO_AM")
                        Dim oraFine As DateTime = rowIndisp.Item("FINE_PM")
                        tooltip.AppendLine(String.Format("{0} ({1}-{2})", rowIndisp.Item("FES_DESCRIZIONE"), oraInizio.ToString("HH:mm"), oraFine.ToString("HH:mm")))
                    Else
                        tooltip.Clear()
                        tooltip.Append(rowIndisp.Item("FES_DESCRIZIONE"))
                        Exit For
                    End If
                Next

            End If

        End If

        'Impostazione disponibilità/prenotato
        Dim lstPrenoGiorno As List(Of AppuntamentoPrenotato) = AppuntamentiTotali_Calendario.Where(Function(d) d.AMB_CODICE = CodiceAmbulatorioCorrente AndAlso d.CNV_DATA_APPUNTAMENTO.Date = giorno.Date).ToList()
        Dim lstDispGiorno As List(Of Disponibilita) = Disponibilita_Calendario.Where(Function(d) d.ambCodice = CodiceAmbulatorioCorrente AndAlso d.Data.Date = giorno.Date).ToList()
        If (lstDispGiorno IsNot Nothing AndAlso lstDispGiorno.Count > 0) Then

            'É presente almeno una disponibilità in giornata
            e.Cell.ForeColor = Color.Green
            e.Cell.Style("font-weight") = "bold"
            tooltip.AppendLine(String.Format("{0} Disponibilità", lstDispGiorno.Count))

            If (lstPrenoGiorno IsNot Nothing AndAlso lstPrenoGiorno.Count > 0) Then
                If (lstPrenoGiorno.Count = 1) Then
                    tooltip.AppendLine(String.Format("{0} Prenotazione", lstPrenoGiorno.Count))
                Else
                    tooltip.AppendLine(String.Format("{0} Prenotazioni", lstPrenoGiorno.Count))
                End If
            End If

        Else

            If (lstPrenoGiorno IsNot Nothing AndAlso lstPrenoGiorno.Count > 0) Then

                'Gli orari disponibili sono stati tutti occupati
                e.Cell.ForeColor = Color.Red
                e.Cell.Style("font-weight") = "normal"
                tooltip.AppendLine("Nessuna Disponibilità")

                If (lstPrenoGiorno.Count = 1) Then
                    tooltip.AppendLine(String.Format("{0} Prenotazione", lstPrenoGiorno.Count))
                Else
                    tooltip.AppendLine(String.Format("{0} Prenotazioni", lstPrenoGiorno.Count))
                End If

            End If

        End If

        e.Cell.ToolTip = tooltip.ToString()

    End Sub

    Private Sub Calendario_VisibleMonthChanged(sender As Object, e As System.Web.UI.WebControls.MonthChangedEventArgs) Handles Calendario.VisibleMonthChanged

        AggiornaCalendario(e.NewDate)

    End Sub

#End Region

    Private Sub LoadMotiviEliminazioneAppuntamento(genericProvider As DbGenericProvider)

        Dim list As List(Of BizRicercaAppuntamenti.MotivoEliminazioneAppuntamento)

        Using bizRicercaAppuntamenti As New BizRicercaAppuntamenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

            list = bizRicercaAppuntamenti.GetMotiviEliminazioneAppuntamento()

            If Not list.IsNullOrEmpty() Then
                list.Insert(0, New BizRicercaAppuntamenti.MotivoEliminazioneAppuntamento())
            End If

        End Using

        Me.ddlMotivoModifica.DataValueField = "Codice"
        Me.ddlMotivoModifica.DataTextField = "Descrizione"
        Me.ddlMotivoModifica.DataSource = list
        Me.ddlMotivoModifica.DataBind()

    End Sub

    Private Sub btnMotivoModifica_Ok_Click(sender As Object, e As EventArgs) Handles btnMotivoModifica_Ok.Click

        If String.IsNullOrWhiteSpace(Me.ddlMotivoModifica.SelectedValue) Then
            Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Quando si modifica o elimina l'appuntamento, è necessario selezionare un motivo.", "MSG_MOTIVO", False, False))
            Return
        End If

        Dim note As String = Me.txtNoteModifica.Text.Trim()
        If note.Length > Me.txtNoteModifica.MaxLength Then
            Me.txtNoteModifica.Text = note.Substring(0, Me.txtNoteModifica.MaxLength)
        End If

        Salva(Me.ddlMotivoModifica.SelectedValue, note)

        Me.fmMotivoNote.VisibileMD = False

    End Sub

    Private Sub btnMotivoModifica_Annulla_Click(sender As Object, e As EventArgs) Handles btnMotivoModifica_Annulla.Click

        Me.fmMotivoNote.VisibileMD = False

    End Sub

    Private Sub ShowModaleStoricoAppuntamento()

        If Me.p_DataConvocazione = DateTime.MinValue Then
            Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Selezionare una convocazione per visualizzarne lo storico appuntamenti.",
                                                                                                  "AlertStorico", False, False))
            Return
        End If

        Me.ucStoricoAppuntamenti.LoadStoricoAppuntamenti(Convert.ToInt64(OnVacUtility.Variabili.PazId), Me.p_DataConvocazione)

        Me.fmStoricoAppuntamenti.VisibileMD = True

    End Sub

End Class
