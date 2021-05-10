Imports System.Collections.Generic
Imports System.ComponentModel

Imports Onit.Database.DataAccessManager
Imports Onit.Controls


Partial Class StatElencoNonVaccinatiPediatra
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

#Region " Eventi Page "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            PanelTitolo.InnerText = OnVacUtility.Variabili.CNS.Descrizione
			ucSelezioneConsultori.MostraSoloAperti = False
			ucSelezioneConsultori.ImpostaCnsCorrente = True
			ucSelezioneConsultori.LoadGetCodici()
			CaricaDati()

            ShowPrintButtons()

        End If

        'lancio della stampa tramite tasto invio
        Select Case Request.Form("__EVENTTARGET")
            Case "Stampa"
                If Me.Toolbar.Items.FromKeyButton("btnStampa").Visible Then
                    Stampa("N")
                End If
        End Select

    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub Toolbar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked
        Select Case e.Button.Key
            Case "btnStampa"
                Stampa("N")
            Case "btnStampaConNote"
                Stampa("S")
        End Select
    End Sub

#End Region

#Region " Eventi finestre modali "

	'se il consultorio è valorizzato, deve disabilitare il distretto [modifica 05/07/2005]


	'se il distretto è valorizzato, deve disabilitare il consultorio [modifica 05/07/2005]
	Private Sub fmDistretto_Change(sender As Object, e As Onit.Controls.OnitModalList.ModalListaEventArgument) Handles fmDistretto.Change
		'OnVacUtility.DisabilitaModale(Me.fmConsultorio, (Me.fmDistretto.Codice <> "" And Me.fmDistretto.Descrizione <> ""))
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
        listPrintButtons.Add(New PrintButton(Constants.ReportName.ElencoNonVaccinatiPediatra, "btnStampa"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Toolbar)

    End Sub

    Private Sub Stampa(parMostraNote As String)

        ' Controllo dati obbligatori (controllo e alert anche lato client)
        If Me.odpDataNascitaIniz.Text = String.Empty Then Return
        If Me.odpDataNascitaFin.Text = String.Empty Then Return
        If Me.txtSeduta.Text = String.Empty Then Return

        Dim stbFiltro As New System.Text.StringBuilder()
        Dim rpt As New ReportParameter()

        'filtro sulla data di nascita 
        stbFiltro.AppendFormat("{{V_NON_VACCINATI_PEDIATRA.PAZ_DATA_NASCITA}} in DateTime ({0}, {1}, {2}, 00, 00, 00) to DateTime ({3}, {4}, {5}, 00, 00, 00)",
                               Me.odpDataNascitaIniz.Data.Year, Me.odpDataNascitaIniz.Data.Month, Me.odpDataNascitaIniz.Data.Day,
                               Me.odpDataNascitaFin.Data.Year, Me.odpDataNascitaFin.Data.Month, Me.odpDataNascitaFin.Data.Day)

        rpt.AddParameter("MostraNote", parMostraNote)
        rpt.AddParameter("DataNascitaIniz", Me.odpDataNascitaIniz.Text)
        rpt.AddParameter("DataNascitaFin", Me.odpDataNascitaFin.Text)

        ' Filtro sesso
        Dim maschi As Boolean = Me.chklSesso.Items(0).Selected
        Dim femmine As Boolean = Me.chklSesso.Items(1).Selected

        If maschi And Not femmine Then
            stbFiltro.Append(" AND {V_NON_VACCINATI_PEDIATRA.PAZ_SESSO} = 'M' ")
        ElseIf Not maschi And femmine Then
            stbFiltro.Append(" AND {V_NON_VACCINATI_PEDIATRA.PAZ_SESSO} = 'F' ")
        End If

        If (Me.chklSesso.Items(0).Selected = True And Me.chklSesso.Items(1).Selected = False) Then
            rpt.AddParameter("paz_sesso", "M")
        ElseIf (Me.chklSesso.Items(0).Selected = False And Me.chklSesso.Items(1).Selected = True) Then
            rpt.AddParameter("paz_sesso", "F")
        Else
            rpt.AddParameter("paz_sesso", "TUTTI")
        End If

		'filtro sul consultorio
		Dim lista As List(Of String) = ucSelezioneConsultori.GetConsultoriSelezionati
		If lista.Count > 0 Then
			stbFiltro.AppendFormat(" AND ({{V_NON_VACCINATI_PEDIATRA.CNS_CODICE}} IN ['{0}']) ", lista.Aggregate(Function(p, g) p & "', '" & g))
			rpt.AddParameter("Consultorio", ucSelezioneConsultori.GetDescrizioneConsultoriSelezionatiPerStampe())
			rpt.AddParameter("paz_cns_codice", "")
		Else
			Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
				Using bizCns As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)
					lista = bizCns.GetListCodiceConsultoriUtente(OnVacContext.UserId)
					If lista.Count > 0 Then
						stbFiltro.AppendFormat(" AND ({{V_NON_VACCINATI_PEDIATRA.CNS_CODICE}} IN ['{0}']) ", lista.Aggregate(Function(p, g) p & "', '" & g))
					End If

				End Using
			End Using
			rpt.AddParameter("Consultorio", "TUTTI")
            rpt.AddParameter("paz_cns_codice", "")
        End If

        'filtro comune residenza
        If Me.fmComuneRes.Codice <> String.Empty And Me.fmComuneRes.Descrizione <> String.Empty Then
            stbFiltro.AppendFormat(" AND {{V_NON_VACCINATI_PEDIATRA.PAZ_COM_CODICE_RESIDENZA}} = '{0}' ", Me.fmComuneRes.Codice)
            rpt.AddParameter("ComRes", Me.fmComuneRes.Descrizione)
            rpt.AddParameter("paz_com_codice_residenza", fmComuneRes.Codice)
        Else
            rpt.AddParameter("ComRes", "TUTTI")
            rpt.AddParameter("paz_com_codice_residenza", "")
        End If

        'filtro circoscrizione
        If Me.fmCircoscrizione.Codice <> String.Empty And Me.fmCircoscrizione.Descrizione <> String.Empty Then
            stbFiltro.AppendFormat(" AND {{V_NON_VACCINATI_PEDIATRA.PAZ_CIR_CODICE}} = '{0}' ", Me.fmCircoscrizione.Codice)
            rpt.AddParameter("Circoscriz", Me.fmCircoscrizione.Descrizione)
            rpt.AddParameter("paz_cir_codice", fmCircoscrizione.Codice)
        Else
            rpt.AddParameter("Circoscriz", "TUTTE")
            rpt.AddParameter("paz_cir_codice", "")
        End If

        'filtro sul distretto
        If fmDistretto.Codice <> String.Empty And Me.fmDistretto.Descrizione <> String.Empty Then
            stbFiltro.AppendFormat(" AND {{V_NON_VACCINATI_PEDIATRA.DIS_CODICE}} = '{0}' ", Me.fmDistretto.Codice)
            rpt.AddParameter("Distretto", Me.fmDistretto.Descrizione)
            rpt.AddParameter("cns_dis_codice", fmDistretto.Codice)
        Else
            rpt.AddParameter("Distretto", "TUTTI")
            rpt.AddParameter("cns_dis_codice", "")
        End If

        'filtro sul medico di base
        If omlMedicoBase.Codice <> String.Empty And omlMedicoBase.Descrizione <> String.Empty Then
            stbFiltro.AppendFormat(" AND {{V_NON_VACCINATI_PEDIATRA.MED_CODICE}} = '{0}' ", omlMedicoBase.Codice)
        End If

        'filtro sullo stato anagrafico
        Dim stbFiltroStatoAnag As New System.Text.StringBuilder()
        Dim stbCodici As New System.Text.StringBuilder()
        For count As Integer = 0 To chkStatoAnagrafico.Items.Count - 1
            If Me.chkStatoAnagrafico.Items(count).Selected Then
                stbFiltroStatoAnag.AppendFormat(" {{V_NON_VACCINATI_PEDIATRA.PAZ_STATO_ANAGRAFICO}} = '{0}' OR ", Me.chkStatoAnagrafico.Items(count).Value)
                stbCodici.AppendFormat("'{0}',", chkStatoAnagrafico.Items(count).Value)
            End If
        Next

        If stbFiltroStatoAnag.Length > 0 Then
            stbFiltroStatoAnag.Remove(stbFiltroStatoAnag.Length - 3, 3)
            stbFiltro.AppendFormat(" AND ({0}) ", stbFiltroStatoAnag.ToString())
            stbCodici.Remove(stbCodici.Length - 1, 1)
            rpt.AddParameter("paz_stato_anagrafico", String.Format("[{0}]", stbCodici.ToString()))
        Else
            rpt.AddParameter("paz_stato_anagrafico", "")
        End If

        'filtro sul tipo vaccinazione
        If Me.radVaccinazioni.SelectedItem.Value = "A" Then
            stbFiltro.AppendFormat(" AND {{V_NON_VACCINATI_PEDIATRA.VAC_OBBLIGATORIA}} = '{0}' ", Me.radVaccinazioni.SelectedItem.Value)
            rpt.AddParameter("TipoVaccinazione", Me.radVaccinazioni.SelectedItem.Text)
        Else
            rpt.AddParameter("TipoVaccinazione", "TUTTE")
        End If

        'filtro sulla seduta
        stbFiltro.AppendFormat(" AND {{V_NON_VACCINATI_PEDIATRA.SED_N_SEDUTA}} = {0} ", Me.txtSeduta.Text)
        rpt.AddParameter("Seduta", Me.txtSeduta.Text)

        'filtro sui cicli
        stbFiltro.AppendFormat(" AND {{V_NON_VACCINATI_PEDIATRA.CIC_CODICE}} = '{0}' ", Me.cmbCiclo.SelectedItem.Value)
        rpt.AddParameter("Ciclo", Me.cmbCiclo.SelectedItem.Text)
        rpt.AddParameter("deslib1", Me.Settings.DESLIB1)

        'CodiceUsl
        rpt.AddParameter("CodiceUsl", OnVacContext.CodiceUslCorrente)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If Not OnVacReport.StampaReport(Constants.ReportName.ElencoNonVaccinatiPediatra, stbFiltro.ToString(), rpt, Nothing, Nothing, bizReport.GetReportFolder(Constants.ReportName.ElencoNonVaccinatiPediatra)) Then
                    OnVacUtility.StampaNonPresente(Page, Constants.ReportName.ElencoNonVaccinatiPediatra)
                End If

            End Using
        End Using

    End Sub

#Region " Caricamento dati "

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
