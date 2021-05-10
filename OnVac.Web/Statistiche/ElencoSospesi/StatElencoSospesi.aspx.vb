Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager


Partial Class StatElencoSospesi
    Inherits OnVac.Common.PageBase

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

#Region " Eventi Page "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            Me.PanelTitolo.InnerText = OnVacUtility.Variabili.CNS.Descrizione

			ucSelezioneConsultori.MostraSoloAperti = False
			ucSelezioneConsultori.ImpostaCnsCorrente = True
			ucSelezioneConsultori.LoadGetCodici()
			ShowPrintButtons()

        End If

        'generazione della stampa tramite pulsante invio
        Select Case Request.Form("__EVENTTARGET")
            Case "Stampa"
                If Me.Toolbar.Items.FromKeyButton("btnStampa").Visible Then
                    Stampa()
                End If
        End Select

    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub Toolbar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked

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

#End Region

#Region " Private "

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()
        listPrintButtons.Add(New PrintButton(Constants.ReportName.ElencoSospesi, "btnStampa"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Toolbar)

    End Sub

    Private Sub Stampa()

        Dim stbFiltro As New System.Text.StringBuilder()
        Dim rpt As New ReportParameter()

        If Me.fmDistretto.Codice <> String.Empty And Me.fmDistretto.Descrizione <> String.Empty Then

            Dim codiciConsultoriDistretto As String = String.Empty

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizConsultori As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                    codiciConsultoriDistretto = bizConsultori.GetFiltroCodiciConsultoriDistretto(Me.fmDistretto.Codice)
                End Using
            End Using

            If Not String.IsNullOrEmpty(codiciConsultoriDistretto) Then
                stbFiltro.AppendFormat(" {{T_PAZ_PAZIENTI.PAZ_CNS_CODICE}} IN [{0}] AND ", codiciConsultoriDistretto)
            End If

            rpt.AddParameter("Consultorio", Me.fmDistretto.Descrizione)

        Else

			'filtro sul consultorio
			Dim lista As List(Of String) = ucSelezioneConsultori.GetConsultoriSelezionati
			If lista.Count > 0 Then
				stbFiltro.AppendFormat(" ({{T_PAZ_PAZIENTI.PAZ_CNS_CODICE}} IN ['{0}']) AND ", lista.Aggregate(Function(p, g) p & "', '" & g))
				rpt.AddParameter("Consultorio", ucSelezioneConsultori.GetDescrizioneConsultoriSelezionatiPerStampe())
			Else
				Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
					Using bizCns As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)
						lista = bizCns.GetListCodiceConsultoriUtente(OnVacContext.UserId)
						If lista.Count > 0 Then
							stbFiltro.AppendFormat(" ({{T_PAZ_PAZIENTI.PAZ_CNS_CODICE}} IN ['{0}']) AND ", lista.Aggregate(Function(p, g) p & "', '" & g))
						End If

					End Using
				End Using
				rpt.AddParameter("Consultorio", "TUTTI")
            End If

        End If

        'filtro sulla data di nascita
        If Me.odpDataNascitaIniz.Text <> String.Empty And Me.odpDataNascitaFin.Text <> String.Empty Then
            stbFiltro.AppendFormat(" {{T_PAZ_PAZIENTI.PAZ_DATA_NASCITA}} in DateTime ({0}, {1}, {2}, 00, 00, 00) to DateTime ({3}, {4}, {5}, 00, 00, 00) AND ",
                                   Me.odpDataNascitaIniz.Data.Year, Me.odpDataNascitaIniz.Data.Month, Me.odpDataNascitaIniz.Data.Day,
                                   Me.odpDataNascitaFin.Data.Year, Me.odpDataNascitaFin.Data.Month, Me.odpDataNascitaFin.Data.Day)

            rpt.AddParameter("DataNascitaIniz", Me.odpDataNascitaIniz.Text)
            rpt.AddParameter("DataNascitaFin", Me.odpDataNascitaFin.Text)
        Else
            rpt.AddParameter("DataNascitaIniz", String.Empty)
            rpt.AddParameter("DataNascitaFin", String.Empty)
        End If

        'filtro sulla data di sospensione
        If Me.odpDataSospensioneIniz.Text <> String.Empty And Me.odpDataSospensioneFin.Text <> String.Empty Then
            stbFiltro.AppendFormat(" {{T_VIS_VISITE.VIS_FINE_SOSPENSIONE}} in DateTime ({0}, {1}, {2}, 00, 00, 00) to DateTime ({3}, {4}, {5}, 00, 00, 00) AND ",
                                   Me.odpDataSospensioneIniz.Data.Year, Me.odpDataSospensioneIniz.Data.Month, Me.odpDataSospensioneIniz.Data.Day,
                                   Me.odpDataSospensioneFin.Data.Year, Me.odpDataSospensioneFin.Data.Month, Me.odpDataSospensioneFin.Data.Day)

            rpt.AddParameter("DataSospensioneIniz", Me.odpDataNascitaIniz.Text)
            rpt.AddParameter("DataSospensioneFin", Me.odpDataNascitaFin.Text)
        Else
            rpt.AddParameter("DataSospensioneIniz", String.Empty)
            rpt.AddParameter("DataSospensioneFin", String.Empty)
        End If

        'filtro sulla condizione di assistito sospeso
        stbFiltro.Append(" NOT(ISNULL({T_VIS_VISITE.VIS_FINE_SOSPENSIONE}))")
        stbFiltro.AppendFormat(" AND {{T_VIS_VISITE.VIS_FINE_SOSPENSIONE}} >= DateTime ({0}, {1}, {2}, 00, 00, 00) ",
                               Date.Now.Year, String.Format(Date.Now.Month, "00"), String.Format(Date.Now.Day, "00"))

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If Not OnVacReport.StampaReport(Constants.ReportName.ElencoSospesi, stbFiltro.ToString(), rpt, Nothing, Nothing, bizReport.GetReportFolder(Constants.ReportName.ElencoSospesi)) Then
                    OnVacUtility.StampaNonPresente(Page, Constants.ReportName.ElencoSospesi)
                End If

            End Using
        End Using

    End Sub

#End Region

End Class
