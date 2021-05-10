Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager

Partial Class StatReazioniAvverse
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

            Me.odpDataEffettuazioneIniz.Text = DateTime.Today
			Me.odpDataEffettuazioneFin.Text = DateTime.Today
			ucSelezioneConsultori.MostraSoloAperti = False
			ucSelezioneConsultori.ImpostaCnsCorrente = False
			ucSelezioneConsultori.LoadGetCodici()

			ShowPrintButtons()

        End If

        'per la stampa tramite tasto invio (modifica 29/07/2004)
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
        listPrintButtons.Add(New PrintButton(Constants.ReportName.StatisticaReazioniAvverse, "btnStampa"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Toolbar)

    End Sub

    Private Sub Stampa()

        Dim rpt As New ReportParameter()

        Dim ds As New DSStatReazioniAvverse()

        Using dam As IDAM = OnVacUtility.OpenDam()

            With dam.QB
                .NewQuery()
                .AddSelectFields("a.*, gruppo")

                .AddTables("t_paz_pazienti, t_ana_consultori, t_report_gruppo, t_ana_circoscrizioni, t_ana_comuni",
                           String.Format("({0}) a", Queries.VaccinazioniEseguite.OracleQueries.selReazioniAvverseAssociazioneDose))

                .AddWhereCondition("ves_paz_codice", Comparatori.Uguale, "paz_codice", DataTypes.Join)
                .AddWhereCondition("paz_cir_codice", Comparatori.Uguale, "cir_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("paz_com_codice_residenza", Comparatori.Uguale, "com_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("paz_cns_codice", Comparatori.Uguale, "cns_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("paz_cns_codice", Comparatori.Uguale, "gruppo", DataTypes.OutJoinLeft)

                rpt.AddParameter("PeriodoVac1", Me.odpDataEffettuazioneIniz.Text)
                rpt.AddParameter("PeriodoVac2", Me.odpDataEffettuazioneFin.Text)

                .AddWhereCondition("ves_data_effettuazione", Comparatori.MaggioreUguale, Me.odpDataEffettuazioneIniz.Data, DataTypes.Data)
                .AddWhereCondition("ves_data_effettuazione", Comparatori.MinoreUguale, Me.odpDataEffettuazioneFin.Data, DataTypes.Data)

                If (Me.odpDataNascitaIniz.Text <> "" And Me.odpDataNascitaFin.Text <> "") Then

                    rpt.AddParameter("DataNascita1", Me.odpDataNascitaIniz.Text)
                    rpt.AddParameter("DataNascita2", Me.odpDataNascitaFin.Text)

                    .AddWhereCondition("paz_data_nascita", Comparatori.MaggioreUguale, Me.odpDataNascitaIniz.Data, DataTypes.Data)
                    .AddWhereCondition("paz_data_nascita", Comparatori.MinoreUguale, Me.odpDataNascitaFin.Data, DataTypes.Data)

                Else
                    rpt.AddParameter("DataNascita1", "")
                    rpt.AddParameter("DataNascita2", "")
                End If

                If (Me.odpDataReazioneIniz.Text <> "" And Me.odpDataReazioneFin.Text <> "") Then

                    rpt.AddParameter("DataReazioneDa", Me.odpDataReazioneIniz.Text)
                    rpt.AddParameter("DataReazioneA", Me.odpDataReazioneFin.Text)

                    .AddWhereCondition("vra_data_reazione", Comparatori.MaggioreUguale, Me.odpDataReazioneIniz.Data, DataTypes.Data)
                    .AddWhereCondition("vra_data_reazione", Comparatori.MinoreUguale, Me.odpDataReazioneFin.Data, DataTypes.Data)

                Else
                    rpt.AddParameter("DataReazioneDa", "")
                    rpt.AddParameter("DataReazioneA", "")
                End If

                ' filtro per distretto
                If fmDistretto.Codice <> "" And fmDistretto.Descrizione <> "" Then
                    .AddWhereCondition("paz_dis_codice", Comparatori.Uguale, fmDistretto.Codice, DataTypes.Stringa)
                    rpt.AddParameter("Distretto", fmDistretto.Descrizione)
                Else
                    rpt.AddParameter("Distretto", "TUTTI")
                End If

				Dim lista As List(Of String) = ucSelezioneConsultori.GetConsultoriSelezionati
				If lista.Count > 0 Then
					Dim filtroIn As New System.Text.StringBuilder()
					For i As Integer = 0 To lista.Count - 1
						filtroIn.AppendFormat("{0},", .AddCustomParam(lista(i)))
					Next
					If filtroIn.Length > 0 Then
						filtroIn.Remove(filtroIn.Length - 1, 1)
						.AddWhereCondition("paz_cns_codice", Comparatori.In, filtroIn.ToString(), DataTypes.Replace)
					End If

					rpt.AddParameter("Consultorio", ucSelezioneConsultori.GetDescrizioneConsultoriSelezionatiPerStampe())
				Else
					Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
						Using bizCns As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)
							lista = bizCns.GetListCodiceConsultoriUtente(OnVacContext.UserId)
							If lista.Count > 0 Then
								Dim filtroIn As New System.Text.StringBuilder()
								For i As Integer = 0 To lista.Count - 1
									filtroIn.AppendFormat("{0},", .AddCustomParam(lista(i)))
								Next
								If filtroIn.Length > 0 Then
									filtroIn.Remove(filtroIn.Length - 1, 1)
									.AddWhereCondition("paz_cns_codice", Comparatori.In, filtroIn.ToString(), DataTypes.Replace)
								End If
							End If

						End Using
					End Using
					rpt.AddParameter("Consultorio", "TUTTI")
                End If

                If Me.fmComuneRes.Codice <> "" And Me.fmComuneRes.Descrizione <> "" Then
                    .AddWhereCondition("paz_com_codice_residenza", Comparatori.Uguale, Me.fmComuneRes.Codice, DataTypes.Stringa)
                    rpt.AddParameter("Comune", Me.fmComuneRes.Descrizione)
                Else
                    rpt.AddParameter("Comune", "TUTTI")
                End If

                If Me.fmCircoscrizione.Codice <> "" And Me.fmCircoscrizione.Descrizione <> "" Then
                    .AddWhereCondition("paz_cir_codice", Comparatori.Uguale, Me.fmCircoscrizione.Codice, DataTypes.Stringa)
                    rpt.AddParameter("Circoscrizione", Me.fmCircoscrizione.Descrizione)
                Else
                    rpt.AddParameter("Circoscrizione", "TUTTE")
                End If

                ' N.B. : se OnVacContext.UserDescription è Nothing, all'rpt deve essere passata la stringa vuota se no dà errore
                If String.IsNullOrEmpty(OnVacContext.UserDescription) Then
                    rpt.AddParameter("UtenteStampa", String.Empty)
                Else
                    rpt.AddParameter("UtenteStampa", OnVacContext.UserDescription)
                End If

            End With

            dam.BuildDataTable(ds.ReazioniAvverse)

        End Using

        rpt.set_dataset(ds)

        ' Parametri intestazione report
        Dim installazione As Entities.Installazione = OnVacUtility.GetDatiInstallazioneCorrente(Me.Settings)

        If installazione Is Nothing Then
            rpt.AddParameter("UslCitta", String.Empty)
            rpt.AddParameter("UslDesc", String.Empty)
            rpt.AddParameter("UslReg", String.Empty)
        Else
            rpt.AddParameter("UslCitta", installazione.UslCitta)
            rpt.AddParameter("UslDesc", installazione.UslDescrizionePerReport)
            rpt.AddParameter("UslReg", installazione.UslRegione)
        End If

        ' Stampa
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If Not OnVacReport.StampaReport(Constants.ReportName.StatisticaReazioniAvverse, String.Empty, rpt, Nothing, Nothing, bizReport.GetReportFolder(Constants.ReportName.StatisticaReazioniAvverse)) Then
                    OnVacUtility.StampaNonPresente(Page, Constants.ReportName.StatisticaReazioniAvverse)
                End If

            End Using
        End Using

    End Sub

#End Region

End Class
