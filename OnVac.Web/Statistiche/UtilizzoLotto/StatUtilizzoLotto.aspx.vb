Imports System.Collections.Generic


Partial Class UtilizzoLotto
    Inherits OnVac.Common.PageBase

    Protected WithEvents lblLotto As System.Web.UI.WebControls.Label


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


    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            Me.PanelTitolo.InnerText = OnVacUtility.Variabili.CNS.Descrizione

			ucSelezioneConsultori.MostraSoloAperti = False
			ucSelezioneConsultori.ImpostaCnsCorrente = True
			ucSelezioneConsultori.LoadGetCodici()

			Me.ShowPrintButtons()

        End If

        'lancio della stampa tramite tasto invio (modifica 29/07/2004)
        Select Case Request.Form("__EVENTTARGET")
            Case "Stampa"
                If Toolbar.Items.FromKeyButton("btnStampa").Visible Then
                    Stampa()
                End If
        End Select

    End Sub

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()
        listPrintButtons.Add(New PrintButton(Constants.ReportName.UtilizzoLotto, "btnStampa"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Toolbar)

    End Sub

    Private Sub Toolbar_ButtonClicked(ByVal sender As System.Object, ByVal e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked
        Select Case e.Button.Key
            Case "btnStampa"
                Me.Stampa()
        End Select
    End Sub

    Private Sub Stampa()

        ' Controllo campi obbligatori (anche lato client, con alert)
        If fmLotto.Codice = String.Empty Or fmLotto.Descrizione = String.Empty Then
            Return
        End If

        ' Parametri
        Dim rpt As New ReportParameter()

        rpt.AddParameter("DataEffettuazione1", odpDataEffettuazioneIniz.Text)
        rpt.AddParameter("DataEffettuazione2", odpDataEffettuazioneFin.Text)
        rpt.AddParameter("Lotto", fmLotto.Descrizione)

        ' Filtri
        Dim stbFiltro As New System.Text.StringBuilder()

        stbFiltro.AppendFormat("{{T_ANA_LOTTI.LOT_CODICE}} = '{0}' ", fmLotto.Codice)

		If ucSelezioneConsultori.GetConsultoriSelezionati.Count > 0 Then
			stbFiltro.AppendFormat(" AND ({{T_ANA_CONSULTORI.CNS_CODICE}} IN ['{0}']) ", ucSelezioneConsultori.GetConsultoriSelezionati.Aggregate(Function(p, g) p & "', '" & g))
		Else
			Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
				Using bizCns As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)
					Dim listaCodici As New List(Of String)
					listaCodici = bizCns.GetListCodiceConsultoriUtente(OnVacContext.UserId)
					If listaCodici.Count > 0 Then
						stbFiltro.AppendFormat(" AND ({{T_ANA_CONSULTORI.CNS_CODICE}} IN ['{0}']) ", listaCodici.Aggregate(Function(p, g) p & "', '" & g))
					End If
				End Using
			End Using
		End If

		If (odpDataEffettuazioneIniz.Text <> String.Empty And odpDataEffettuazioneFin.Text <> String.Empty) Then
            stbFiltro.AppendFormat(" AND {{V_VAC_ESEGUITE_UNION_SCADUTE.VES_DATA_EFFETTUAZIONE}} in DateTime ({0}, {1}, {2}, 00, 00, 00) to DateTime ({3}, {4}, {5}, 00, 00, 00) ", _
                                   odpDataEffettuazioneIniz.Data.Year, odpDataEffettuazioneIniz.Data.Month, odpDataEffettuazioneIniz.Data.Day, _
                                   odpDataEffettuazioneFin.Data.Year, odpDataEffettuazioneFin.Data.Month, odpDataEffettuazioneFin.Data.Day)
        End If

        ' Creazione Report
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If Not OnVacReport.StampaReport(Constants.ReportName.UtilizzoLotto, stbFiltro.ToString(), rpt, , , bizReport.GetReportFolder(Constants.ReportName.UtilizzoLotto)) Then
                    OnVacUtility.StampaNonPresente(Page, Constants.ReportName.UtilizzoLotto)
                End If

            End Using
        End Using

    End Sub

End Class
