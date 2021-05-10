Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.Controls


Partial Class ElencoEsclusione
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

            Me.PanelTitolo.InnerText = OnVacUtility.Variabili.CNS.Descrizione
			ucSelezioneConsultori.MostraSoloAperti = False
			ucSelezioneConsultori.ImpostaCnsCorrente = False
			ucSelezioneConsultori.LoadGetCodici()
			Me.CaricaDati()

            Me.ShowPrintButtons()

        End If

        'lancio della stampa tramite tasto invio
        Select Case Request.Form("__EVENTTARGET")

            Case "Stampa"

                If Me.Toolbar.Items.FromKeyButton("btnStampa").Visible Then
                    Me.Stampa(Constants.ReportName.ElencoEsclusione)
                End If

        End Select

    End Sub

#End Region

#Region " Eventi toolbar "

    Private Sub Toolbar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked

        Select Case e.Button.Key

            Case "btnStampa"

                Me.Stampa(Constants.ReportName.ElencoEsclusione)

            Case "btnStampaEsclusioni"

                Me.Stampa(Constants.ReportName.ElencoEsclusioneVaccinazioni)

        End Select

    End Sub

#End Region

#Region " Private "

#Region " Caricamento dati "

    Private Sub CaricaDati()

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Me.CaricaVaccinazioni(genericProvider)
            Me.CaricaMotiviEsclusione(genericProvider)
            Me.CaricaStatiAnagrafici(genericProvider)
        End Using

    End Sub

    Private Sub CaricaVaccinazioni(genericProvider As DAL.DbGenericProvider)

        Dim dtVaccini As DataTable =
            genericProvider.AnaVaccinazioni.GetDataTableCodiceDescrizioneVaccinazioni("VAC_DESCRIZIONE")

        Dim emptyRow As DataRow = dtVaccini.NewRow()
        emptyRow("VAC_CODICE") = "TUTTE"
        emptyRow("VAC_DESCRIZIONE") = "TUTTE"

        Dim drc As DataRowCollection = dtVaccini.Rows()
        drc.InsertAt(emptyRow, 0)

        Me.cmbVaccinazioni.DataValueField = "VAC_CODICE"
        Me.cmbVaccinazioni.DataTextField = "VAC_DESCRIZIONE"
        Me.cmbVaccinazioni.DataSource = dtVaccini
        Me.cmbVaccinazioni.DataBind()

    End Sub

    Private Sub CaricaMotiviEsclusione(genericProvider As DAL.DbGenericProvider)

        Dim dt As DataTable = Nothing

        Using bizMotiviEsclusione As New Biz.BizMotiviEsclusione(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())
            dt = bizMotiviEsclusione.GetDataTableMotiviEsclusione()
        End Using

        Dim emptyRow As DataRow = dt.NewRow()
        emptyRow("MOE_CODICE") = "TUTTE"
        emptyRow("MOE_DESCRIZIONE") = "TUTTE"

        Dim drc As DataRowCollection = dt.Rows()
        drc.InsertAt(emptyRow, 0)

        Me.cmbMotiviEsclusione.DataValueField = "MOE_CODICE"
        Me.cmbMotiviEsclusione.DataTextField = "MOE_DESCRIZIONE"
        Me.cmbMotiviEsclusione.DataSource = dt
        Me.cmbMotiviEsclusione.DataBind()

    End Sub

    Private Sub CaricaStatiAnagrafici(genericProvider As DAL.DbGenericProvider)

        Dim dtStatiAnag As DataTable = Nothing

        Using bizStatiAnagrafici As New Biz.BizStatiAnagrafici(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())
            dtStatiAnag = bizStatiAnagrafici.LeggiStatiAnagrafici()
        End Using

        Me.chklStatoAnagrafico.DataValueField = "SAN_CODICE"
        Me.chklStatoAnagrafico.DataTextField = "SAN_DESCRIZIONE"
        Me.chklStatoAnagrafico.DataSource = dtStatiAnag
        Me.chklStatoAnagrafico.DataBind()

    End Sub

#End Region

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()

        listPrintButtons.Add(New PrintButton(Constants.ReportName.ElencoEsclusione, "btnStampa"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.ElencoEsclusioneVaccinazioni, "btnStampaEsclusioni"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Me.Toolbar)

    End Sub

    Private Sub Stampa(nomeReport As String)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Dim elencoEsclusioneResult As Biz.BizStampaElencoEsclusione.ElencoEsclusioneResult = Nothing

            Using bizStampaElencoEsclusione As New Biz.BizStampaElencoEsclusione(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Dim filtriElencoEsclusione As New IStampaElencoEsclusione.FiltriElencoEsclusione()

                ' Date di nascita
                filtriElencoEsclusione.DataNascitaInizio = Me.odpDataNascitaIniz.Data
                filtriElencoEsclusione.DataNascitaFine = Me.odpDataNascitaFin.Data

				' Consultorio
				Dim lista As List(Of String) = ucSelezioneConsultori.GetConsultoriSelezionati
				If lista.Count = 0 Then

					Using bizCns As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)
						lista = bizCns.GetListCodiceConsultoriUtente(OnVacContext.UserId)
						If lista.Count > 0 Then
							filtriElencoEsclusione.CodiceConsultorio = lista
							filtriElencoEsclusione.DescrizioneConsultorio = "TUTTI"
						End If
					End Using
				Else
					filtriElencoEsclusione.CodiceConsultorio = lista
					filtriElencoEsclusione.DescrizioneConsultorio = ucSelezioneConsultori.GetDescrizioneConsultoriSelezionatiPerStampe()
				End If



				' Comune di residenza
				filtriElencoEsclusione.CodiceComuneResidenza = Me.fmComuneRes.Codice
                filtriElencoEsclusione.DescrizioneComuneResidenza = Me.fmComuneRes.Descrizione

                ' Circoscrizione
                filtriElencoEsclusione.CodiceCircoscrizione = Me.fmCircoscrizione.Codice
                filtriElencoEsclusione.DescrizioneCircoscrizione = Me.fmCircoscrizione.Descrizione

                ' Distretto
                filtriElencoEsclusione.CodiceDistretto = Me.fmDistretto.Codice
                filtriElencoEsclusione.DescrizioneDistretto = Me.fmDistretto.Descrizione

                ' Vaccinazioni obbligatorie
                filtriElencoEsclusione.SoloVaccinazioniObbligatorie = (Me.radVaccinazioni.SelectedItem.Value = Constants.ObbligatorietaVaccinazione.Obbligatoria)

                ' Motivo esclusione
                If Me.cmbMotiviEsclusione.SelectedItem.Value <> "TUTTE" Then
                    filtriElencoEsclusione.CodiceMotivoEsclusione = Me.cmbMotiviEsclusione.SelectedItem.Value
                    filtriElencoEsclusione.DescrizioneMotivoEsclusione = Me.cmbMotiviEsclusione.SelectedItem.Text
                End If

                ' Vaccinazioni
                If Me.cmbVaccinazioni.SelectedItem.Value <> "TUTTE" Then
                    filtriElencoEsclusione.CodiceVaccinazione = Me.cmbVaccinazioni.SelectedItem.Value
                    filtriElencoEsclusione.DescrizioneVaccinazione = Me.cmbVaccinazioni.SelectedItem.Text
                End If

                ' Stato anagrafico
                If Me.chklStatoAnagrafico.SelectedItems.Count > 0 Then

                    Dim arrayDescrizioneStatiAnagrafici As String() = (From item As ListItem In Me.chklStatoAnagrafico.SelectedItems
                                                                       Select item.Text).ToArray()

                    Dim arrayCodiceStatiAnagrafici As String() = (From item As ListItem In Me.chklStatoAnagrafico.SelectedItems
                                                                  Select "'" + item.Value + "'").ToArray()

                    filtriElencoEsclusione.CodiciStatiAnagrafici = String.Join(",", arrayCodiceStatiAnagrafici)
                    filtriElencoEsclusione.DescrizioniStatiAnagrafici = String.Join(";", arrayDescrizioneStatiAnagrafici)

                End If

                elencoEsclusioneResult = bizStampaElencoEsclusione.GetElencoEsclusione(nomeReport, filtriElencoEsclusione)

            End Using

            Dim rpt As New ReportParameter()

            rpt.set_dataset(elencoEsclusioneResult.DataSetElencoEsclusione)

            If Not elencoEsclusioneResult.ParametriReport Is Nothing AndAlso elencoEsclusioneResult.ParametriReport.Count > 0 Then

                For Each parametro As KeyValuePair(Of String, String) In elencoEsclusioneResult.ParametriReport
                    rpt.AddParameter(parametro.Key, parametro.Value)
                Next

            End If

            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If Not OnVacReport.StampaReport(nomeReport, String.Empty, rpt, Nothing, Nothing, bizReport.GetReportFolder(nomeReport)) Then
                    OnVacUtility.StampaNonPresente(Page, nomeReport)
                End If

            End Using
        End Using

    End Sub

#End Region

#Region " Eventi finestre modali "

	'se è valorizzato il consultorio, deve disabilitare il distretto [modifica 06/07/2005]


	'se è valorizzato il distretto, deve disabilitare il consultorio [modifica 06/07/2005]
	Private Sub fmDistretto_Change(sender As Object, e As Onit.Controls.OnitModalList.ModalListaEventArgument) Handles fmDistretto.Change

		'OnVacUtility.DisabilitaModale(Me.fmConsultorio, IIf(Me.fmDistretto.Codice <> "" And Me.fmDistretto.Descrizione <> "", True, False))

	End Sub

    Private Sub fmCircoscrizione_Change(sender As Object, e As Onit.Controls.OnitModalList.ModalListaEventArgument) Handles fmCircoscrizione.Change

        OnVacUtility.DisabilitaModale(Me.fmComuneRes, IIf(Me.fmCircoscrizione.Codice <> "" And Me.fmCircoscrizione.Descrizione <> "", True, False))

    End Sub

    Private Sub fmComuneRes_Change(sender As Object, e As Onit.Controls.OnitModalList.ModalListaEventArgument) Handles fmComuneRes.Change

        OnVacUtility.DisabilitaModale(Me.fmCircoscrizione, IIf(Me.fmComuneRes.Codice <> "" And Me.fmComuneRes.Descrizione <> "", True, False))

    End Sub

#End Region

End Class
