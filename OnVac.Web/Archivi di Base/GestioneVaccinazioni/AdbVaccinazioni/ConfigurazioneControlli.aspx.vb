Imports System.Collections.Generic
Imports Infragistics.WebUI.UltraWebToolbar
Imports Onit.Controls.PagesLayout
Imports Onit.OnAssistnet.OnVac.Biz
Imports Onit.OnAssistnet.OnVac.Entities

Public Class ConfigurazioneControlli
    Inherits Common.PageBase


#Region " Classes "

    <Serializable()>
    Public Class FiltriConfigurazioneCertificati

        'Public StatiAnagrafici As List(Of String)

        Public Associazioni As List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro1)
        Public DosiAssociazioni As List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro2)

        Public Cicli As List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro1)
        Public Sedute As List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro2)

        Public Vaccinazioni As List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro1)
        Public DosiVaccinazioni As List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro2)

        Public Sub New()
            'StatiAnagrafici = New List(Of String)
            Associazioni = New List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro1)
            DosiAssociazioni = New List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro2)
            Cicli = New List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro1)
            Associazioni = New List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro1)
            Sedute = New List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro2)
            Vaccinazioni = New List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro1)
            DosiVaccinazioni = New List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro2)
        End Sub

    End Class

#End Region
#Region " Types "

    Private Enum StatoLayoutPagina
        View = 0
        Insert = 1
        Edit = 2
    End Enum

    Private Enum DgrColumnIndex
        SelectionColumn = 0
        Id = 1
    End Enum

#End Region


#Region " Properties "

    Private Property StatoLayout As StatoLayoutPagina
        Get
            If ViewState("LAYOUT") Is Nothing Then ViewState("LAYOUT") = StatoLayoutPagina.View
            Return DirectCast(ViewState("LAYOUT"), StatoLayoutPagina)
        End Get
        Set(value As StatoLayoutPagina)
            ViewState("LAYOUT") = value
            SetToolbarLayout(value)
        End Set
    End Property

    Private Property FiltriMaschera() As FiltriConfigurazioneCertificati
        Get
            If ViewState("FCCConfigurazione") Is Nothing Then ViewState("FCCConfigurazione") = New FiltriConfigurazioneCertificati()
            Return ViewState("FCCConfigurazione")
        End Get
        Set(value As FiltriConfigurazioneCertificati)
            ViewState("FCCConfigurazione") = value
        End Set
    End Property

    'contiene i codici dei motivi di immunita
    Property arrImmunitaCod() As ArrayList
        Get
            Return Session("CodImmunitaInseriti")
        End Get
        Set(Value As ArrayList)
            Session("CodImmunitaInseriti") = Value
        End Set
    End Property

    'contiene le descrizioni dei motivi di immunita
    Property arrImmunitaDesc() As ArrayList
        Get
            Return Session("DescImmunitaInseriti")
        End Get
        Set(Value As ArrayList)
            Session("DescImmunitaInseriti") = Value
        End Set
    End Property
    'contiene i codici dei motivi di esonero
    Property arrEsoneroCod() As ArrayList
        Get
            Return Session("CodEsoneroInseriti")
        End Get
        Set(Value As ArrayList)
            Session("CodEsoneroInseriti") = Value
        End Set
    End Property

    'contiene le descrizioni dei motivi di esonero
    Property arrEsoneroDesc() As ArrayList
        Get
            Return Session("DescEsoneroInseriti")
        End Get
        Set(Value As ArrayList)
            Session("DescEsoneroInseriti") = Value
        End Set
    End Property
    Public Property IdConfigSelezionato() As Integer
        Get
            Return Session("IdConfigSelezionato")
        End Get
        Set(ByVal Value As Integer)
            Session("IdConfigSelezionato") = Value
        End Set
    End Property

#End Region

#Region " Page "

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then

            StatoLayout = StatoLayoutPagina.View
            FiltriMaschera = New FiltriConfigurazioneCertificati()
            UscFiltroPrenotazioneSelezioneMultipla1.ControlloDosi = 2
            UscFiltroPrenotazioneSelezioneMultipla1.TipoVisualizzazione = 2
            CaricaGrid(0)
        End If
    End Sub

#End Region
#Region " Toolbar "

    Private Sub ToolBar_ButtonClicked(sender As Object, be As ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case be.Button.Key

            Case "btnNuovo"
                ClearDettaglio()
                dgrConfigurazioneCertificati.SelectedIndex = -1
                StatoLayout = StatoLayoutPagina.Insert
                Me.OnitLayout31.Busy = True

            Case "btnModifica"
                If dgrConfigurazioneCertificati.SelectedItem Is Nothing Then
                    OnitLayout31.ShowMsgBox(New OnitLayout3.OnitLayoutMsgBox("Impossibile modificare: nessun elemento selezionato.", "ERR", False, False))
                    Return
                End If
                StatoLayout = StatoLayoutPagina.Edit
                Me.OnitLayout31.Busy = True

            Case "btnElimina"
                If EliminaSelezionato() Then
                    CaricaGrid(0)
                    StatoLayout = StatoLayoutPagina.View
                End If


            Case "btnSalva"
                Dim result As Biz.BizGenericResult = Salva()
                If Not result.Success Then
                    Me.OnitLayout31.ShowMsgBox(New OnitLayout3.OnitLayoutMsgBox(result.Message, "ERR", False, False))
                    Return
                End If
                CaricaGrid(0)
                StatoLayout = StatoLayoutPagina.View
                Me.OnitLayout31.Busy = False

            Case "btnAnnulla"
                LoadDetail(IdConfigSelezionato)
                StatoLayout = StatoLayoutPagina.View
                Me.OnitLayout31.Busy = False

        End Select

    End Sub





#End Region
#Region " Private "
    ''' <summary>
    ''' Gestione della toolbar
    ''' </summary>
    ''' <param name="stato"></param>
    Private Sub SetToolbarLayout(stato As StatoLayoutPagina)

        Dim inEdit As Boolean = (stato = StatoLayoutPagina.Edit Or stato = StatoLayoutPagina.Insert)

        ToolBar.Items.FromKeyButton("btnNuovo").Enabled = Not inEdit
        ToolBar.Items.FromKeyButton("btnModifica").Enabled = Not inEdit
        ToolBar.Items.FromKeyButton("btnElimina").Enabled = Not inEdit
        ToolBar.Items.FromKeyButton("btnSalva").Enabled = inEdit
        ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = inEdit
        enableCampi(inEdit)

    End Sub
    ''' <summary>
    ''' Gestione enable dei campi dettagli
    ''' </summary>
    ''' <param name="enableCampo"></param>
    Private Sub enableCampi(enableCampo As Boolean)

        Dim cssString As String = "Textbox_Stringa"
        Dim cssStringObbl As String = "TextBox_Stringa_Obbligatorio"
        Dim cssData As String = "Textbox_Data"
        Dim cssDataObbl As String = "TextBox_Data_Obbligatorio"

        Dim urlima As String = "../../../images/filtro_vaccinazioni.gif"

        If Not enableCampo Then
            cssString = "Textbox_Stringa_Disabilitato"
            cssStringObbl = "Textbox_Stringa_Disabilitato"
            cssData = "Textbox_Data_Disabilitato"
            cssDataObbl = "Textbox_Data_Disabilitato"
            urlima = "../../../images/filtro_vaccinazioni_dis.gif"
            ddlTipoControllo.Enabled = enableCampo
        Else
            loadDdlTipoControllo(cbAppuntamenti.Checked)
        End If

        ucEtaInizio.Enabled = enableCampo
        ucEtaFine.Enabled = enableCampo

        odpDataNascitaDa.Enabled = enableCampo
        odpDataNascitaDa.CssClass = cssDataObbl
        odpDataNascitaA.Enabled = enableCampo
        odpDataNascitaA.CssClass = cssDataObbl

        ddlSesso.Enabled = enableCampo
        ddlSesso.CssClass = cssStringObbl

        odpDataControllo.Enabled = enableCampo
        odpDataControllo.CssClass = cssData

        btnImgVaccinazioniDosi.ImageUrl = urlima
        btnImgVaccinazioniDosi.Enabled = enableCampo

        txtTestoPositivo.Enabled = enableCampo
        txtTestoNegativo.Enabled = enableCampo

        buttonMotiviImmunita.Enabled = enableCampo
        buttonMotiviEsonero.Enabled = enableCampo

        txtTestoEsoneri.Enabled = enableCampo

        cbAppuntamenti.Enabled = enableCampo

    End Sub

    ''' <summary>
    ''' Clear campi dettagli
    ''' </summary>
    Private Sub ClearDettaglio()

        ucEtaFine.SetGiorni(Nothing)
        ucEtaInizio.SetGiorni(Nothing)

        txtTestoEsoneri.Text = String.Empty
        txtTestoNegativo.Text = String.Empty
        txtTestoPositivo.Text = String.Empty

        lblMotiviImmunita.Text = String.Empty
        lblMotiviEsonero.Text = String.Empty
        lblVaccinazioniDosi.Text = String.Empty

        FiltriMaschera.Vaccinazioni = Nothing

        ddlSesso.ClearSelection()
        odpDataNascitaDa.Text = String.Empty
        odpDataNascitaA.Text = String.Empty
        odpDataControllo.Text = String.Empty
        cbAppuntamenti.Checked = True

        loadDdlTipoControllo(True)

    End Sub

    ''' <summary>
    ''' Salvataggio di una configurazione
    ''' </summary>
    ''' <returns></returns>
    Private Function Salva() As BizGenericResult

        Dim result As Biz.BizGenericResult

        Dim configurazione As Entities.ConfigurazioneCertificazione = GetConfigurazioniFromDettaglio()
        Dim vaccinidosi As List(Of Entities.ConfigurazioneCertificazioneVaccinazioni) = GetConfigurazioniVaccDosiFromDettaglio()

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizConfig As New Biz.BizConfigurazioneCertificato(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                result = bizConfig.SaveConfigurazione(configurazione, vaccinidosi)

            End Using
        End Using

        Return result

    End Function

    ''' <summary>
    ''' Cancellazione di una configurazione
    ''' </summary>
    ''' <returns></returns>
    Private Function EliminaSelezionato() As Boolean

        Dim result As Biz.BizGenericResult
        If Me.dgrConfigurazioneCertificati.SelectedItem Is Nothing Then
            OnitLayout31.ShowMsgBox(New OnitLayout3.OnitLayoutMsgBox("Impossibile effettuare l'eliminazione: nessun elemento selezionato.", "ERR", False, False))
            Return False
        End If

        Dim value As String = HttpUtility.HtmlDecode(Me.dgrConfigurazioneCertificati.SelectedItem.Cells(DgrColumnIndex.Id).Text).Trim()

        If String.IsNullOrEmpty(value) Then
            OnitLayout31.ShowMsgBox(New OnitLayout3.OnitLayoutMsgBox("Impossibile effettuare l'eliminazione: nessun elemento selezionato.", "ERR", False, False))
            Return False
        End If

        Dim idconfig As Integer = Convert.ToInt32(value)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizConfig As New BizConfigurazioneCertificato(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                result = bizConfig.DeleteConfigurazioni(idconfig)

            End Using
        End Using

        If Not result.Success Then
            OnitLayout31.ShowMsgBox(New OnitLayout3.OnitLayoutMsgBox(result.Message, "ERR", False, False))
            Return False
        End If

        Return True

    End Function

    ''' <summary>
    ''' Recupera i dati del dettaglio selezionato
    ''' </summary>
    ''' <returns></returns>
    Private Function GetConfigurazioniVaccDosiFromDettaglio() As List(Of Entities.ConfigurazioneCertificazioneVaccinazioni)

        Dim vacDosi As New List(Of Entities.ConfigurazioneCertificazioneVaccinazioni)

        If Not FiltriMaschera.Vaccinazioni Is Nothing Then
            For Each vac As UscFiltroPrenotazioneSelezioneMultipla.Filtro1 In FiltriMaschera.Vaccinazioni
                Dim vaccinoNDose As New Entities.ConfigurazioneCertificazioneVaccinazioni
                vaccinoNDose.IdConfigurazioneCertificato = IdConfigSelezionato
                vaccinoNDose.CodiceVaccino = vac.Codice
                If Not String.IsNullOrWhiteSpace(vac.Valore) Then
                    vaccinoNDose.NumeroDose = Convert.ToInt32(vac.Valore)
                Else
                    vaccinoNDose.NumeroDose = Nothing
                End If
                vacDosi.Add(vaccinoNDose)
            Next
        End If

        Return vacDosi

    End Function

    ''' <summary>
    ''' Funzione che recupera tutti i dati del dettaglio
    ''' </summary>
    ''' <returns></returns>
    Private Function GetConfigurazioniFromDettaglio() As ConfigurazioneCertificazione

        Dim config As New Entities.ConfigurazioneCertificazione()

        If StatoLayout = StatoLayoutPagina.Insert Then
            config.IsNew = True
        Else
            config.IsNew = False
            config.Id = IdConfigSelezionato
        End If

        ' data nascita da
        If Not String.IsNullOrWhiteSpace(odpDataNascitaDa.Text) Then
            config.DataNascitaDa = odpDataNascitaDa.Data
        Else
            config.DataNascitaDa = Nothing
        End If

        ' data nascita da
        If Not String.IsNullOrWhiteSpace(odpDataNascitaA.Text) Then
            config.DataNascitaA = odpDataNascitaA.Data
        Else
            config.DataNascitaA = Nothing
        End If

        ' Eta inizio
        If Not String.IsNullOrWhiteSpace(ucEtaInizio.GetValueCampoAnno()) Then
            config.EtaAnnoDa = Convert.ToInt32(ucEtaInizio.GetValueCampoAnno())
        Else
            config.EtaAnnoDa = Nothing
        End If
        If Not String.IsNullOrWhiteSpace(ucEtaInizio.GetValueCampoMese()) Then
            config.EtaMeseDa = Convert.ToInt32(ucEtaInizio.GetValueCampoMese())
        Else
            config.EtaMeseDa = Nothing
        End If
        If Not String.IsNullOrWhiteSpace(ucEtaInizio.GetValueCampoGiorno()) Then
            config.EtaGiornoDa = Convert.ToInt32(ucEtaInizio.GetValueCampoGiorno())
        Else
            config.EtaGiornoDa = Nothing
        End If

        ' Eta fine
        If Not String.IsNullOrWhiteSpace(ucEtaFine.GetValueCampoAnno()) Then
            config.EtaAnnoA = Convert.ToInt32(ucEtaFine.GetValueCampoAnno())
        Else
            config.EtaAnnoA = Nothing
        End If
        If Not String.IsNullOrWhiteSpace(ucEtaFine.GetValueCampoMese()) Then
            config.EtaMeseA = Convert.ToInt32(ucEtaFine.GetValueCampoMese())
        Else
            config.EtaMeseA = Nothing
        End If
        If Not String.IsNullOrWhiteSpace(ucEtaFine.GetValueCampoGiorno()) Then
            config.EtaGiornoA = Convert.ToInt32(ucEtaFine.GetValueCampoGiorno())
        Else
            config.EtaGiornoA = Nothing
        End If

        'sesso
        config.Sesso = ddlSesso.SelectedValue

        'data controllo
        If Not String.IsNullOrWhiteSpace(odpDataControllo.Text) Then
            config.DataControllo = odpDataControllo.Data
        Else
            config.DataControllo = Nothing
        End If

        config.TestoPositivo = txtTestoPositivo.Text
        config.TestoNegativo = txtTestoNegativo.Text
        config.TestoParziale = txtTestoEsoneri.Text

        config.CodiceMotiviImmunita = lblMotiviImmunita.Text
        config.CodiceMotiviEsonero = lblMotiviEsonero.Text

        If cbAppuntamenti.Checked Then
            config.FlgCheckAppuntamenti = "S"
            config.TipoCheckAppuntamenti = ddlTipoControllo.SelectedValue
        Else
            config.TipoCheckAppuntamenti = ddlTipoControllo.SelectedValue
            config.FlgCheckAppuntamenti = "N"
        End If

        Return config

    End Function

    ''' <summary>
    ''' Apertura look up cicli sedute
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub btnImgVaccinazioniDosi_Click(sender As Object, e As ImageClickEventArgs)

        'Set degli eventuali filtri vaccinazioni
        If Not FiltriMaschera.Vaccinazioni Is Nothing AndAlso FiltriMaschera.Vaccinazioni.Count > 0 Then

            Dim vaccinazioni = FiltriMaschera.Vaccinazioni.ConvertToDataTable()
            UscFiltroPrenotazioneSelezioneMultipla1.setValoriSelezionatiFiltro1(vaccinazioni)
        Else
            UscFiltroPrenotazioneSelezioneMultipla1.setValoriSelezionatiFiltro1(Nothing)
        End If

        fmFiltroAssociazioniDosi.VisibileMD = True

    End Sub

    ''' <summary>
    ''' Salvataggio selezioni della lookup Cicli sedute
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnOk_FiltroCicliSedute_Click(sender As System.Object, e As System.EventArgs) Handles btnOk_FiltroAssociazioniDosi.Click

        'Chiusura della modale
        Me.fmFiltroAssociazioniDosi.VisibileMD = False

        'Aggiornamento dei filtri nel viewstate
        Dim dtVaccini As DataTable = Me.UscFiltroPrenotazioneSelezioneMultipla1.getValoriSelezionatiFiltro1()
        FiltriMaschera.Vaccinazioni = dtVaccini.ConvertToList(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro1)()

        'Aggiornamento label
        lblVaccinazioniDosi.Text = Me.UscFiltroPrenotazioneSelezioneMultipla1.getStringaFormattata()

    End Sub

    ''' <summary>
    ''' Annulla lookup cicli sedute
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnAnnulla_FiltroCicliSedute_Click(sender As System.Object, e As System.EventArgs) Handles btnAnnulla_FiltroAssociazioniDosi.Click

        fmFiltroAssociazioniDosi.VisibileMD = False

    End Sub

    ''' <summary>
    ''' Apertura look up delle dei motivi di escusione
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub buttonMotiviEscusioni_Click(sender As Object, e As System.EventArgs) Handles buttonMotiviImmunita.Click

        'caricamento della modale contenente l'elenco dei motivi di esclusione
        InsMotivoImmunita.ModaleName = "modInsMotivoImmunita"
        InsMotivoImmunita.MotiviSelezionati = lblMotiviImmunita.Text
        InsMotivoImmunita.LoadModale()

        modInsMotivoImmunita.VisibileMD = True

        OnitLayout31.Busy = True

    End Sub

    Private Sub buttonMotiviEsonero_Click(sender As Object, e As System.EventArgs) Handles buttonMotiviEsonero.Click

        'caricamento della modale contenente l'elenco dei motivi di esclusione
        InsMotivoEsonero.ModaleName = "modInsMotivoEsonero"
        InsMotivoEsonero.MotiviSelezionati = lblMotiviEsonero.Text
        InsMotivoEsonero.LoadModale()

        modInsMotivoEsonero.VisibileMD = True

        OnitLayout31.Busy = True

    End Sub

    ''' <summary>
    ''' Recupero dati della griglia
    ''' </summary>
    ''' <param name="id"></param>
    Private Sub CaricaGrid(id As Integer)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizCertificato As New Biz.BizConfigurazioneCertificato(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                dgrConfigurazioneCertificati.DataSource = bizCertificato.GetConfigurazioni(id)
                dgrConfigurazioneCertificati.DataBind()

            End Using
        End Using

    End Sub

    ''' <summary>
    ''' Recupero dati dettaglio
    ''' </summary>
    ''' <param name="id"></param>
    Private Sub LoadDetail(id As Integer)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizCertificato As New Biz.BizConfigurazioneCertificato(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Dim detail As List(Of Entities.ConfigurazioneCertificazione) = bizCertificato.GetConfigurazioni(id)

                For Each list As Entities.ConfigurazioneCertificazione In detail

                    ucEtaInizio.SetValueAnnoMeseGiorno(list.EtaAnnoDa, list.EtaMeseDa, list.EtaGiornoDa)
                    ucEtaFine.SetValueAnnoMeseGiorno(list.EtaAnnoA, list.EtaMeseA, list.EtaGiornoA)
                    ddlSesso.SelectedValue = list.Sesso

                    If Not list.DataControllo.HasValue Then
                        odpDataControllo.Text = String.Empty
                    Else
                        odpDataControllo.Text = list.DataControllo
                    End If

                    ' Data di nascita da
                    If Not list.DataNascitaDa.HasValue Then
                        odpDataNascitaDa.Text = String.Empty
                    Else
                        odpDataNascitaDa.Text = list.DataNascitaDa
                    End If

                    'Data di nascita a 
                    If Not list.DataNascitaA.HasValue Then
                        odpDataNascitaA.Text = String.Empty
                    Else
                        odpDataNascitaA.Text = list.DataNascitaA
                    End If

                    lblVaccinazioniDosi.Text = list.listDescVacciniDosi

                    Dim vacFiltr As New List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro1)()

                    For Each fil As Entities.Filtro In list.filtri
                        Dim filtro As New UscFiltroPrenotazioneSelezioneMultipla.Filtro1
                        filtro.Codice = fil.Codice
                        filtro.Valore = fil.Valore
                        vacFiltr.Add(filtro)
                    Next

                    FiltriMaschera.Vaccinazioni = vacFiltr

                    ''CHE VALORI DEVO METTERE PER LA LISTA
                    txtTestoPositivo.Text = list.TestoPositivo
                    txtTestoNegativo.Text = list.TestoNegativo
                    txtTestoEsoneri.Text = list.TestoParziale
                    lblMotiviImmunita.Text = list.CodiceMotiviImmunita
                    lblMotiviEsonero.Text = list.CodiceMotiviEsonero

                    If list.FlgCheckAppuntamenti = "S" Then
                        cbAppuntamenti.Checked = True
                    Else
                        cbAppuntamenti.Checked = False
                    End If

                Next

            End Using
        End Using

    End Sub

#End Region

#Region " User Control Motivo Esclusione "

    'recupera i codici dei motivi di Immunita selezionati nella modale 
    Private Sub InsMotivoImmunita_InviaCodMotEsc(arrMotEscCod As System.Collections.ArrayList, arrMotEscDesc As System.Collections.ArrayList) Handles InsMotivoImmunita.InviaCodMotEsc

        modInsMotivoImmunita.VisibileMD = False

        'visualizzazione dei codici dei motivi di esclusione valorizzati
        arrImmunitaCod = arrMotEscCod
        arrImmunitaDesc = arrMotEscDesc

        If arrImmunitaCod.Count > 0 Then

            lblMotiviImmunita.Text = "" & arrImmunitaCod(0)

            For i As Integer = 1 To arrImmunitaCod.Count - 1
                lblMotiviImmunita.Text &= "," & arrImmunitaCod(i)
            Next

        Else
            lblMotiviImmunita.Text = String.Empty
        End If

    End Sub

    'riabilita il layout alla chiusura della modale dei motivi di immunita
    Private Sub InsMotivoImmunita_RiabilitaLayout() Handles InsMotivoImmunita.RiabilitaLayout

        modInsMotivoImmunita.VisibileMD = False

        'i codici dei motivi di esclusione devono essere vuoti
        Me.arrImmunitaCod = Nothing
        Me.arrImmunitaDesc = Nothing

    End Sub

    'recupera i codici dei motivi di esonero selezionati nella modale 
    Private Sub InsMotivoEsonero_InviaCodMotEsonero(arrMotEsoneroCod As System.Collections.ArrayList, arrMotEsoneroDesc As System.Collections.ArrayList) Handles InsMotivoEsonero.InviaCodMotEsc

        modInsMotivoEsonero.VisibileMD = False

        'visualizzazione dei codici dei motivi di esclusione valorizzati
        arrEsoneroCod = arrMotEsoneroCod
        arrEsoneroDesc = arrMotEsoneroDesc

        If arrEsoneroCod.Count > 0 Then

            lblMotiviEsonero.Text = "" & arrEsoneroCod(0)

            For i As Integer = 1 To arrEsoneroCod.Count - 1
                lblMotiviEsonero.Text &= "," & arrEsoneroCod(i)
            Next

        Else
            lblMotiviEsonero.Text = String.Empty
        End If

    End Sub

    'riabilita il layout alla chiusura della modale dei motivi di esonero
    Private Sub InsMotivoEsonero_RiabilitaLayout() Handles InsMotivoEsonero.RiabilitaLayout

        modInsMotivoEsonero.VisibileMD = False

        'i codici dei motivi di esclusione devono essere vuoti
        arrEsoneroCod = Nothing
        arrEsoneroDesc = Nothing

    End Sub

#End Region

#Region " Eventi Datagrid "

    Private Sub dgrConfigurazioneCertificati_ItemCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgrConfigurazioneCertificati.ItemCommand

        Select Case e.CommandName

            Case "Select"

                Dim value As String = dgrConfigurazioneCertificati.Items(e.Item.ItemIndex).Cells(DgrColumnIndex.Id).Text
                If Not String.IsNullOrEmpty(value) Then
                    IdConfigSelezionato = Convert.ToInt32(value)
                End If

                LoadDetail(IdConfigSelezionato)

        End Select

    End Sub

    Private Sub cbAppuntamenti_CheckedChanged(sender As Object, e As EventArgs) Handles cbAppuntamenti.CheckedChanged

        loadDdlTipoControllo(cbAppuntamenti.Checked)

    End Sub

    Private Sub loadDdlTipoControllo(check As Boolean)
        If Not check Then
            ddlTipoControllo.Enabled = False
            ddlTipoControllo.ClearSelection()
            ddlTipoControllo.Items.Clear()
            ddlTipoControllo.Items.Insert(0, New ListItem(String.Empty, String.Empty))
            ddlTipoControllo.DataBind()
        Else
            ddlTipoControllo.Enabled = True
            ddlTipoControllo.Items.Clear()
            ddlTipoControllo.Items.Insert(0, New ListItem("PARZIALE", "1"))
            ddlTipoControllo.Items.Insert(1, New ListItem("POSITIVO", "0"))
            ddlTipoControllo.DataBind()
        End If
    End Sub

#End Region

End Class