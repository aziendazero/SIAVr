Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager


Partial Class ElencoRitardatari
    Inherits OnVac.Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Page Events "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            Me.PanelTitolo.InnerText = OnVacUtility.Variabili.CNS.Descrizione

            Me.odpDataConvocazioneFin.Data = DateTime.Today

			ucSelezioneConsultori.MostraSoloAperti = False
			ucSelezioneConsultori.ImpostaCnsCorrente = True
			ucSelezioneConsultori.LoadGetCodici()

			OnVacUtility.DisabilitaModale(Me.fmDistretto, False)

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

#Region " Eventi Toolbar "

    Private Sub Toolbar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked

        Select Case e.Button.Key
            Case "btnStampa"
                Stampa()
        End Select

    End Sub

#End Region

#Region " Eventi finestre modali "

	'se è valorizzato il consultorio, deve disabilitare il distretto [modifica 06/07/2005]


	'se è valorizzato il distretto, deve disabilitare il consultorio[modifica 06/07/2005]
	Private Sub fmDistretto_Change(sender As Object, e As Onit.Controls.OnitModalList.ModalListaEventArgument) Handles fmDistretto.Change
		ucSelezioneConsultori.MostraCnsUtente = True
		ucSelezioneConsultori.MostraSoloAperti = False
		ucSelezioneConsultori.ImpostaCnsCorrente = True
		ucSelezioneConsultori.FiltroDistretto = fmDistretto.Codice
		ucSelezioneConsultori.LoadGetCodici()
	End Sub

#End Region

#Region " Private "

    Private Sub CaricaDati()

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Me.CaricaStatiAnagrafici(genericProvider)
            Me.LoadVaccinazioni(genericProvider)
        End Using

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

    Private Sub LoadVaccinazioni(genericProvider As DAL.DbGenericProvider)

        Dim dtVaccinazioni As DataTable = genericProvider.AnaVaccinazioni.GetVaccinazioni(Nothing)

        Me.chkVaccinazioni.DataValueField = "VAC_CODICE"
        Me.chkVaccinazioni.DataTextField = "VAC_DESCRIZIONE"
        Me.chkVaccinazioni.DataSource = dtVaccinazioni
        Me.chkVaccinazioni.DataBind()

    End Sub

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()

        listPrintButtons.Add(New PrintButton(Constants.ReportName.ElencoRitardatari, "btnStampa"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Toolbar)

    End Sub

    Private Sub Stampa()

        ' Controllo campi obbligatori (anche lato client, con alert)
        If Me.odpDataConvocazioneFin.Text = String.Empty Then
            Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Impossibile stampare il report: il campo 'Data di Convocazione A' non è valorizzato.", "msgNoStampa", False, False))
            Return
        End If

        ' Controllo validità campo Dose, se valorizzato (anche lato client, con alert)
        Dim numeroDosi As Integer = 0

        Dim strNumeroDosi As String = Me.txtNumeroDosi.Text.Trim()
        If Not String.IsNullOrEmpty(strNumeroDosi) Then
            If Not Integer.TryParse(strNumeroDosi, numeroDosi) Then
                Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Impossibile stampare il report: il campo 'Numero Dosi' deve essere un numero valido.", "msgNoStampa", False, False))
                Return
            End If
        End If

        Dim stbFiltro As New System.Text.StringBuilder()
        Dim rpt As New ReportParameter()

        ' DATA CONVOCAZIONE
        rpt.AddParameter("DataConvocazione1", Me.odpDataConvocazioneIniz.Text)
        rpt.AddParameter("DataConvocazione2", Me.odpDataConvocazioneFin.Text)

        If Me.odpDataConvocazioneIniz.Text <> String.Empty Then
            stbFiltro.AppendFormat("{{T_CNV_CONVOCAZIONI.CNV_DATA}} in DateTime ({0}, {1}, {2}, 00, 00, 00) to DateTime ({3}, {4}, {5} , 00, 00, 00)",
                                   Me.odpDataConvocazioneIniz.Data.Year, Me.odpDataConvocazioneIniz.Data.Month, Me.odpDataConvocazioneIniz.Data.Day,
                                   Me.odpDataConvocazioneFin.Data.Year, Me.odpDataConvocazioneFin.Data.Month, Me.odpDataConvocazioneFin.Data.Day)
        Else
            stbFiltro.AppendFormat("{{T_CNV_CONVOCAZIONI.CNV_DATA}} <= DateTime ({0}, {1}, {2}, 00, 00, 00)",
                                   Me.odpDataConvocazioneFin.Data.Year, Me.odpDataConvocazioneFin.Data.Month, Me.odpDataConvocazioneFin.Data.Day)
        End If

        ' NUMERO SOLLECITO
        If Me.chkEsatta.Checked Then
            rpt.AddParameter("RicercaEsatta", "=")
        Else
            rpt.AddParameter("RicercaEsatta", ">=")
        End If

        Select Case Me.rdlNumeroSollecito.SelectedValue
            Case 1
                rpt.AddParameter("NSollecito", "Primo")
            Case 2
                rpt.AddParameter("NSollecito", "Secondo")
            Case 3
                rpt.AddParameter("NSollecito", "Terzo")
            Case 4
                rpt.AddParameter("NSollecito", "Quarto")
        End Select

        stbFiltro.AppendFormat(" AND {{T_CNV_CICLI.CNC_N_SOLLECITO}} {0} {1}", IIf(Me.chkEsatta.Checked, "=", ">="), Me.rdlNumeroSollecito.SelectedValue)

        If Me.fmDistretto.Codice <> String.Empty And Me.fmDistretto.Descrizione <> String.Empty Then

            ' DISTRETTO
            Dim codiciConsultoriDistretto As String = String.Empty

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizConsultori As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                    codiciConsultoriDistretto = bizConsultori.GetFiltroCodiciConsultoriDistretto(Me.fmDistretto.Codice)
                End Using
            End Using

            If Not String.IsNullOrEmpty(codiciConsultoriDistretto) Then
                stbFiltro.AppendFormat(" AND {{T_CNV_CONVOCAZIONI.CNV_CNS_CODICE}} IN [{0}] ", codiciConsultoriDistretto)
            End If

			rpt.AddParameter("Consultorio", ucSelezioneConsultori.GetDescrizioneConsultoriSelezionatiPerStampe())

		Else

			' CONSULTORIO
			If ucSelezioneConsultori.GetConsultoriSelezionati.Count > 0 Then
				stbFiltro.AppendFormat(" AND ({{T_CNV_CONVOCAZIONI.CNV_CNS_CODICE}} IN ['{0}']) ", ucSelezioneConsultori.GetConsultoriSelezionati.Aggregate(Function(p, g) p & "', '" & g))
				rpt.AddParameter("Consultorio", ucSelezioneConsultori.GetDescrizioneConsultoriSelezionatiPerStampe())
			Else
				Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
					Using bizCns As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)
						Dim listaCodici As New List(Of String)
						listaCodici = bizCns.GetListCodiceConsultoriUtente(OnVacContext.UserId)
						If listaCodici.Count > 0 Then
							stbFiltro.AppendFormat(" AND ({{T_CNV_CONVOCAZIONI.CNV_CNS_CODICE}} IN ['{0}']) ", listaCodici.Aggregate(Function(p, g) p & "', '" & g))
						End If
					End Using
				End Using
				rpt.AddParameter("Consultorio", "TUTTI")
            End If

        End If

        ' DATA DI NASCITA
        rpt.AddParameter("DataNascita1", Me.dpkDataNascitaDa.Text)
        rpt.AddParameter("DataNascita2", Me.dpkDataNascitaA.Text)

        If Not String.IsNullOrEmpty(Me.dpkDataNascitaDa.Text) Then
            stbFiltro.AppendFormat(" AND {{T_PAZ_PAZIENTI.PAZ_DATA_NASCITA}} >= DateTime ({0}, {1}, {2}, 00, 00, 00) ",
                                   Me.dpkDataNascitaDa.Data.Year, Me.dpkDataNascitaDa.Data.Month, Me.dpkDataNascitaDa.Data.Day)
        End If

        If Not String.IsNullOrEmpty(Me.dpkDataNascitaA.Text) Then
            stbFiltro.AppendFormat(" AND {{T_PAZ_PAZIENTI.PAZ_DATA_NASCITA}} <= DateTime ({0}, {1}, {2}, 23, 59, 59) ",
                                   Me.dpkDataNascitaA.Data.Year, Me.dpkDataNascitaA.Data.Month, Me.dpkDataNascitaA.Data.Day)
        End If

		' NUMERO DOSI
		If numeroDosi > 0 Then
            rpt.AddParameter("NumeroDosi", numeroDosi.ToString())
            stbFiltro.AppendFormat(" AND {{T_VAC_PROGRAMMATE.VPR_N_RICHIAMO}} = {0} ", numeroDosi)
        Else
            rpt.AddParameter("NumeroDosi", String.Empty)
        End If

        ' STATO ANAGRAFICO
        If Me.chklStatoAnagrafico.SelectedItems.Count > 0 Then

            Dim arrayDescrizioneStatiAnagrafici As String() = (From item As ListItem In Me.chklStatoAnagrafico.SelectedItems
                                                               Select item.Text).ToArray()

            rpt.AddParameter("StatiAnagrafici", String.Join(";", arrayDescrizioneStatiAnagrafici))

            Dim arrayCodiceStatiAnagrafici As String() = (From item As ListItem In Me.chklStatoAnagrafico.SelectedItems
                                                          Select "'" + item.Value + "'").ToArray()

            stbFiltro.AppendFormat(" AND {{T_PAZ_PAZIENTI.PAZ_STATO_ANAGRAFICO}} IN [{0}] ", String.Join(",", arrayCodiceStatiAnagrafici))

        Else
            rpt.AddParameter("StatiAnagrafici", String.Empty)
        End If

        ' VACCINAZIONI
        If Me.chkVaccinazioni.SelectedItems.Count > 0 Then
            stbFiltro.AppendFormat(" AND {{T_VAC_PROGRAMMATE.VPR_VAC_CODICE}} in [{0}]  ", Me.chkVaccinazioni.SelectedValues.ToString(True))
        End If

        If Me.chkVaccinazioni.SelectedItems.Count = 0 Or Me.chkVaccinazioni.SelectedItems.Count = Me.chkVaccinazioni.Items.Count Then
            rpt.AddParameter("Vaccinazioni", "TUTTE")
        Else
            Dim selectedItems As String() = (From item As ListItem In Me.chkVaccinazioni.SelectedItems Select item.Value).ToArray()
            rpt.AddParameter("Vaccinazioni", String.Join(";", selectedItems))
        End If

        'CODICE USL
        rpt.AddParameter("CodiceUsl", OnVacContext.CodiceUslCorrente)

        ' Creazione report
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If Not OnVacReport.StampaReport(Constants.ReportName.ElencoRitardatari, stbFiltro.ToString(), rpt, Nothing, Nothing, bizReport.GetReportFolder(Constants.ReportName.ElencoRitardatari)) Then
                    OnVacUtility.StampaNonPresente(Page, Constants.ReportName.ElencoRitardatari)
                End If

            End Using
        End Using

    End Sub

#End Region

End Class
