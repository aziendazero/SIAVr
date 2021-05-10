Imports System.Collections.Generic
Imports System.ComponentModel

Imports Onit.Database.DataAccessManager
Imports Onit.Controls


Partial Class StatElencoNonVaccinati
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

#Region " Page Events "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            Me.PanelTitolo.InnerText = OnVacUtility.Variabili.CNS.Descrizione

            CaricaDati()
            ShowPrintButtons()

        End If

        'lancio della stampa tramite tasto invio (modifica 29/07/2004)
        Select Case Request.Form("__EVENTTARGET")
            Case "Stampa"
                If Me.Toolbar.Items.FromKeyButton("btnStampa").Visible Then
                    Stampa()
                End If
        End Select
    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub Toolbar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked

        Select Case e.Button.Key
            Case "btnStampa"
                Stampa()
        End Select

    End Sub

#End Region

#Region " Eventi finestre modali "

	'se è valorizzato il consultorio, deve disabilitare il distretto [modifica 06/07/2005]


	'se è valorizzato il distretto, deve disabilitare il consultorio [modifica 06/07/2005]
	Private Sub fmDistretto_Change(sender As Object, e As Onit.Controls.OnitModalList.ModalListaEventArgument) Handles fmDistretto.Change
		'OnVacUtility.DisabilitaModale(Me.fmConsultorio, IIf(Me.fmDistretto.Codice <> "" And Me.fmDistretto.Descrizione <> "", True, False))
		ucSelezioneConsultori.MostraCnsUtente = True
		ucSelezioneConsultori.MostraSoloAperti = False
		ucSelezioneConsultori.ImpostaCnsCorrente = True
		ucSelezioneConsultori.FiltroDistretto = fmDistretto.Codice
		ucSelezioneConsultori.LoadGetCodici()
	End Sub
    Private Sub fmCircoscrizione_Change(sender As Object, e As Onit.Controls.OnitModalList.ModalListaEventArgument) Handles fmCircoscrizione.Change
        OnVacUtility.DisabilitaModale(Me.fmComuneRes, IIf(Me.fmCircoscrizione.Codice <> "" And Me.fmCircoscrizione.Descrizione <> "", True, False))
    End Sub

    Private Sub fmComuneRes_Change(sender As Object, e As Onit.Controls.OnitModalList.ModalListaEventArgument) Handles fmComuneRes.Change
        OnVacUtility.DisabilitaModale(Me.fmCircoscrizione, IIf(Me.fmComuneRes.Codice <> "" And Me.fmComuneRes.Descrizione <> "", True, False))
    End Sub

#End Region

#Region " Private "

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()
        listPrintButtons.Add(New PrintButton(Constants.ReportName.ElencoNonVaccinati, "btnStampa"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Toolbar)

    End Sub

    Private Sub Stampa()

        ' Controllo filtri obbligatori (date di nascita e seduta, già controllati lato client con eventuale alert)
        If Me.odpDataNascitaIniz.Text = String.Empty Then Return
        If Me.odpDataNascitaFin.Text = String.Empty Then Return
        If Me.txtSeduta.Text = String.Empty Then Return

        Dim stbFiltro As New System.Text.StringBuilder()
        Dim rpt As New ReportParameter()

        'filtro sulla data di nascita
        stbFiltro.AppendFormat("{{V_ELENCO_NON_VACCINATI.PAZ_DATA_NASCITA}} in DateTime ({0}, {1}, {2}, 00, 00, 00) to DateTime ({3}, {4}, {5}, 00, 00, 00) ",
                               Me.odpDataNascitaIniz.Data.Year, Me.odpDataNascitaIniz.Data.Month, Me.odpDataNascitaIniz.Data.Day,
                               Me.odpDataNascitaFin.Data.Year, Me.odpDataNascitaFin.Data.Month, Me.odpDataNascitaFin.Data.Day)

        rpt.AddParameter("DataNascitaIniz", Me.odpDataNascitaIniz.Text)
        rpt.AddParameter("DataNascitaFin", Me.odpDataNascitaFin.Text)

		'filtro sul consultorio
		Dim lista As List(Of String) = ucSelezioneConsultori.GetConsultoriSelezionati
		If lista.Count > 0 Then
			stbFiltro.AppendFormat(" AND ({{V_ELENCO_NON_VACCINATI.CNS_CODICE}} IN ['{0}']) ", lista.Aggregate(Function(p, g) p & "', '" & g))
			rpt.AddParameter("Consultorio", ucSelezioneConsultori.GetDescrizioneConsultoriSelezionatiPerStampe())
		Else
			Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
				Using bizCns As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)
					lista = bizCns.GetListCodiceConsultoriUtente(OnVacContext.UserId)
					If lista.Count > 0 Then
						stbFiltro.AppendFormat(" AND ({{V_ELENCO_NON_VACCINATI.CNS_CODICE}} IN ['{0}']) ", lista.Aggregate(Function(p, g) p & "', '" & g))
					End If

				End Using
			End Using
			rpt.AddParameter("Consultorio", "TUTTI")
        End If

        'filtro comune residenza
        If Me.fmComuneRes.Codice <> String.Empty And Me.fmComuneRes.Descrizione <> String.Empty Then
            stbFiltro.AppendFormat(" AND {{V_ELENCO_NON_VACCINATI.PAZ_COM_CODICE_RESIDENZA}} = '{0}' ", Me.fmComuneRes.Codice)
            rpt.AddParameter("ComRes", Me.fmComuneRes.Descrizione)
        Else
            rpt.AddParameter("ComRes", "TUTTI")
        End If

        'filtro circoscrizione
        If Me.fmCircoscrizione.Codice <> String.Empty And Me.fmCircoscrizione.Descrizione <> String.Empty Then
            stbFiltro.AppendFormat(" AND {{V_ELENCO_NON_VACCINATI.PAZ_CIR_CODICE}} = '{0}' ", Me.fmCircoscrizione.Codice)
            rpt.AddParameter("Circoscriz", Me.fmCircoscrizione.Descrizione)
        Else
            rpt.AddParameter("Circoscriz", "TUTTE")
        End If

        'filtro sul distretto
        If Me.fmDistretto.Codice <> "" And Me.fmDistretto.Descrizione <> "" Then
            stbFiltro.AppendFormat(" AND {{V_ELENCO_NON_VACCINATI.DIS_CODICE}} = '{0}' ", Me.fmDistretto.Codice)
            rpt.AddParameter("Distretto", Me.fmDistretto.Descrizione)
        Else
            rpt.AddParameter("Distretto", "TUTTI")
        End If

        'filtro sullo stato anagrafico
        Dim stbFiltroStatiAnag As New System.Text.StringBuilder()
        For i As Integer = 0 To Me.chkStatoAnagrafico.Items.Count - 1
            If Me.chkStatoAnagrafico.Items(i).Selected Then
                stbFiltroStatiAnag.AppendFormat(" {{V_ELENCO_NON_VACCINATI.PAZ_STATO_ANAGRAFICO}} = '{0}' OR ", Me.chkStatoAnagrafico.Items(i).Value)
            End If
        Next

        If stbFiltroStatiAnag.Length > 0 Then
            stbFiltroStatiAnag.Remove(stbFiltroStatiAnag.Length - 3, 3)
            stbFiltro.AppendFormat(" AND ({0}) ", stbFiltroStatiAnag.ToString())
        End If

        'filtro sul tipo vaccinazione
        If Me.radVaccinazioni.SelectedItem.Value = "A" Then
            stbFiltro.AppendFormat(" AND {{V_ELENCO_NON_VACCINATI.VAC_OBBLIGATORIA}} = '{0}' ", Me.radVaccinazioni.SelectedItem.Value)
            rpt.AddParameter("TipoVaccinazione", Me.radVaccinazioni.SelectedItem.Text)
        Else
            rpt.AddParameter("TipoVaccinazione", "TUTTE")
        End If

        'filtro sulla seduta
        stbFiltro.AppendFormat(" AND {{V_ELENCO_NON_VACCINATI.SED_N_SEDUTA}} = {0} ", Me.txtSeduta.Text)
        rpt.AddParameter("Seduta", Me.txtSeduta.Text)

        'filtro sui cicli
        stbFiltro.AppendFormat(" AND {{V_ELENCO_NON_VACCINATI.CIC_CODICE}} = '{0}' ", Me.cmbCiclo.SelectedItem.Value)
        rpt.AddParameter("Ciclo", Me.cmbCiclo.SelectedItem.Text)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If Not OnVacReport.StampaReport(Constants.ReportName.ElencoNonVaccinati, stbFiltro.ToString(), rpt, Nothing, Nothing, bizReport.GetReportFolder(Constants.ReportName.ElencoNonVaccinati)) Then
                    OnVacUtility.StampaNonPresente(Page, Constants.ReportName.ElencoNonVaccinati)
                End If

            End Using
        End Using

    End Sub

#Region " Caricamento Dati "

    Private Sub CaricaDati()

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            CaricaCicli(genericProvider)
            CaricaStatiAnagrafici(genericProvider)
        End Using

    End Sub

    Private Sub CaricaCicli(genericProvider As DAL.DbGenericProvider)

        Dim listCicli As BindingList(Of Entities.Ciclo) = Nothing

        Using bizCicli As New Biz.BizCicli(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())
            listCicli = bizCicli.LoadCicli()
        End Using

        Me.cmbCiclo.DataTextField = "Descrizione"
        Me.cmbCiclo.DataValueField = "Codice"
        Me.cmbCiclo.DataSource = listCicli
        Me.cmbCiclo.DataBind()

    End Sub

    Private Sub CaricaStatiAnagrafici(genericProvider As DAL.DbGenericProvider)

        Dim dtStatiAnag As DataTable = Nothing

        Using bizStatiAnagrafici As New Biz.BizStatiAnagrafici(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())
            dtStatiAnag = bizStatiAnagrafici.LeggiStatiAnagrafici()
        End Using

        Me.chkStatoAnagrafico.DataValueField = "SAN_CODICE"
        Me.chkStatoAnagrafico.DataTextField = "SAN_DESCRIZIONE"
        Me.chkStatoAnagrafico.DataSource = dtStatiAnag
        Me.chkStatoAnagrafico.DataBind()

    End Sub

#End Region

#End Region

End Class
