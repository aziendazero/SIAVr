Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager


Partial Class StatisticaVaccinazioni
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
            PanelTitolo.InnerText = OnVacUtility.Variabili.CNS.Descrizione

            odpDataEffettuazioneIniz.Text = DateTime.Today
            odpDataEffettuazioneFin.Text = DateTime.Today
			ucSelezioneConsultori.MostraSoloAperti = False
			ucSelezioneConsultori.ImpostaCnsCorrente = False
			ucSelezioneConsultori.LoadGetCodici()
			CaricaDati()

            ShowPrintButtons()
        End If

        'per la stampa tramite tasto invio (modifica 29/07/2004)
        Select Case Request.Form("__EVENTTARGET")
            Case "Stampa"
                If Toolbar.Items.FromKeyButton("btnStampa").Visible Then
                    Stampa(Constants.ReportName.VaccinazioniEseguite)
                End If
        End Select

    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub Toolbar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked
        Select Case e.Button.Key
            Case "btnStampa"
                Stampa(Constants.ReportName.VaccinazioniEseguite)
            Case "btnStampaComune"
                Stampa(Constants.ReportName.VaccinazioniEseguiteXComune)
            Case "btnStampaCircoscriz"
                Stampa(Constants.ReportName.VaccinazioniEseguiteXCircosc)
            Case "btnStampaTotCns"
                Stampa(Constants.ReportName.VaccinazioniEseguiteTotCNS)
            Case "btnStampaEseMal"
                Stampa(Constants.ReportName.VaccinazioniEseguiteXEsenzioneMalattia)
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

        listPrintButtons.Add(New PrintButton(Constants.ReportName.VaccinazioniEseguite, "btnStampa"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.VaccinazioniEseguiteXComune, "btnStampaComune"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.VaccinazioniEseguiteXCircosc, "btnStampaCircoscriz"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.VaccinazioniEseguiteTotCNS, "btnStampaTotCns"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.VaccinazioniEseguiteXEsenzioneMalattia, "btnStampaEseMal"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Toolbar)

    End Sub

    'procedura richiamabile anche dal page_load (modifica 29/07/2004)
    Private Sub Stampa(nomeReport As String)

        Dim rpt As New ReportParameter()

        Dim dam As IDAM = OnVacUtility.OpenDam()

        With dam.QB
            .NewQuery()
            .AddSelectFields("paz_data_nascita, ves_paz_codice, ves_vac_codice, ves_cns_codice, ves_stato, ves_n_richiamo",
                             "ves_data_effettuazione, vac_descrizione, cns_descrizione, paz_cir_codice, cir_descrizione",
                             "paz_com_codice_residenza, com_descrizione, gruppo, ves_stato, ves_mal_codice_malattia, ves_codice_esenzione")
            .AddTables("t_vac_eseguite, t_paz_pazienti, t_ana_vaccinazioni, t_ana_consultori, t_report_gruppo, t_ana_circoscrizioni, t_ana_comuni")
            .AddWhereCondition("ves_paz_codice", Comparatori.Uguale, "paz_codice", DataTypes.Join)
            .AddWhereCondition("paz_cir_codice", Comparatori.Uguale, "cir_codice", DataTypes.OutJoinLeft)
            .AddWhereCondition("paz_com_codice_residenza", Comparatori.Uguale, "com_codice", DataTypes.OutJoinLeft)
            .AddWhereCondition("ves_vac_codice", Comparatori.Uguale, "vac_codice", DataTypes.Join)
            .AddWhereCondition("ves_cns_codice", Comparatori.Uguale, "cns_codice", DataTypes.OutJoinLeft)
            .AddWhereCondition("cns_codice", Comparatori.Uguale, "gruppo", DataTypes.OutJoinLeft)

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

            'filtro per stato anagrafico
            Dim tmp As New ArrayList()
            For i As Integer = 0 To Me.chklStatoAnagrafico.Items.Count - 1
                If Me.chklStatoAnagrafico.Items(i).Selected Then
                    tmp.Add(i)
                End If
            Next

            If tmp.Count > 1 Then
                .OpenParanthesis()
                .AddWhereCondition("paz_stato_anagrafico", Comparatori.Uguale, Me.chklStatoAnagrafico.Items(tmp(0)).Value, DataTypes.Stringa)
                For i As Integer = 1 To tmp.Count - 1
                    .AddWhereCondition("paz_stato_anagrafico", Comparatori.Uguale, Me.chklStatoAnagrafico.Items(tmp(i)).Value, DataTypes.Stringa, "OR")
                Next
                .CloseParanthesis()
            ElseIf tmp.Count = 1 Then
                .AddWhereCondition("paz_stato_anagrafico", Comparatori.Uguale, Me.chklStatoAnagrafico.Items(tmp(0)).Value, DataTypes.Stringa)
            End If

            ' filtro per distretto
            If Me.fmDistretto.Codice <> "" And Me.fmDistretto.Descrizione <> "" Then
                .AddWhereCondition("cns_dis_codice", Comparatori.Uguale, Me.fmDistretto.Codice, DataTypes.Stringa)
                rpt.AddParameter("Distretto", Me.fmDistretto.Descrizione)
            Else
                rpt.AddParameter("Distretto", "TUTTI")
            End If

            'filtri per tipo vaccinazione (VES_STATO: R - Registrata, altrimenti Eseguita)
            If (Me.chklStatoVac.Items(0).Selected = True) And (Me.chklStatoVac.Items(1).Selected = False) Then
                .OpenParanthesis()
                .AddWhereCondition("ves_stato", Comparatori.Is, "NULL", DataTypes.Stringa)
                .AddWhereCondition("ves_stato", Comparatori.Diverso, "R", DataTypes.Stringa, "OR")
                .CloseParanthesis()
            ElseIf (Me.chklStatoVac.Items(0).Selected = False) And (Me.chklStatoVac.Items(1).Selected = True) Then
                .AddWhereCondition("ves_stato", Comparatori.Uguale, "R", DataTypes.Stringa)
            End If

            ' filtri per tipo consultorio (A - adulti, P - pediatrico, V - vaccinatore)
            If (Me.chklTipoCns.Items(0).Selected = True) And (Me.chklTipoCns.Items(1).Selected = True) And (Me.chklTipoCns.Items(2).Selected = False) Then

                .OpenParanthesis()
                .AddWhereCondition("cns_tipo", Comparatori.Uguale, "A", DataTypes.Stringa)
                .AddWhereCondition("cns_tipo", Comparatori.Uguale, "P", DataTypes.Stringa, "or")
                .CloseParanthesis()

                rpt.AddParameter("TipoCns", "ADULTI E PEDIATRICO")

            ElseIf (Me.chklTipoCns.Items(0).Selected = True) And (Me.chklTipoCns.Items(1).Selected = False) And (Me.chklTipoCns.Items(2).Selected = True) Then

                .OpenParanthesis()
                .AddWhereCondition("cns_tipo", Comparatori.Uguale, "A", DataTypes.Stringa)
                .AddWhereCondition("cns_tipo", Comparatori.Uguale, "V", DataTypes.Stringa, "or")
                .CloseParanthesis()
                rpt.AddParameter("TipoCns", "ADULTI E VACCINATORE")

            ElseIf (Me.chklTipoCns.Items(0).Selected = True) And (Me.chklTipoCns.Items(1).Selected = False) And (Me.chklTipoCns.Items(2).Selected = False) Then

                .AddWhereCondition("cns_tipo", Comparatori.Uguale, "A", DataTypes.Stringa)
                rpt.AddParameter("TipoCns", "ADULTI")

            ElseIf (Me.chklTipoCns.Items(0).Selected = False) And (Me.chklTipoCns.Items(1).Selected = True) And (Me.chklTipoCns.Items(2).Selected = False) Then

                .AddWhereCondition("cns_tipo", Comparatori.Uguale, "P", DataTypes.Stringa)
                rpt.AddParameter("TipoCns", "PEDIATRICO")

            ElseIf (Me.chklTipoCns.Items(0).Selected = False) And (Me.chklTipoCns.Items(1).Selected = False) And (Me.chklTipoCns.Items(2).Selected = True) Then

                .AddWhereCondition("cns_tipo", Comparatori.Uguale, "V", DataTypes.Stringa)
                rpt.AddParameter("TipoCns", "VACCINATORE")

            ElseIf (Me.chklTipoCns.Items(0).Selected = False) And (Me.chklTipoCns.Items(1).Selected = True) And (Me.chklTipoCns.Items(2).Selected = True) Then

                .OpenParanthesis()
                .AddWhereCondition("cns_tipo", Comparatori.Uguale, "P", DataTypes.Stringa)
                .AddWhereCondition("cns_tipo", Comparatori.Uguale, "V", DataTypes.Stringa, "or")
                .CloseParanthesis()
                rpt.AddParameter("TipoCns", "PEDIATRICO E VACCINATORE")

            Else
                rpt.AddParameter("TipoCns", "ADULTI PEDIATRICO E VACCINATORE")
            End If
			Dim lista As List(Of String) = ucSelezioneConsultori.GetConsultoriSelezionati
			If lista.Count > 0 Then

				Dim filtroIn As New System.Text.StringBuilder()

				For i As Integer = 0 To lista.Count - 1
					filtroIn.AppendFormat("{0},", .AddCustomParam(lista(i)))
				Next

				If filtroIn.Length > 0 Then
					filtroIn.Remove(filtroIn.Length - 1, 1)
					.AddWhereCondition("ves_cns_codice", Comparatori.In, filtroIn.ToString(), DataTypes.Replace)
				End If
				rpt.AddParameter("Consultorio", ucSelezioneConsultori.GetDescrizioneConsultoriSelezionatiPerStampe())

			Else
				Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
					Using bizCns As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)
						' calcolo la lista dei centri associati all'utente
						Dim listaCodici As New List(Of String)
						listaCodici = bizCns.GetListCodiceConsultoriUtente(OnVacContext.UserId)

						If listaCodici.Count > 0 Then
							Dim sListConsTutti As New System.Text.StringBuilder()
							' Aggiungo valori nei parametri
							For i As Integer = 0 To listaCodici.Count - 1
								sListConsTutti.AppendFormat("{0},", .AddCustomParam(listaCodici(i)))
							Next
							' se la stringa ha dei valori aggiungo 
							If sListConsTutti.Length > 0 Then
								sListConsTutti.Remove(sListConsTutti.Length - 1, 1)
								.AddWhereCondition("ves_cns_codice", Comparatori.In, sListConsTutti.ToString(), DataTypes.Replace)
							End If
						End If

					End Using
				End Using
				rpt.AddParameter("Consultorio", "TUTTI")
            End If

            If Me.fmComuneRes.Codice <> "" And Me.fmComuneRes.Descrizione <> "" Then

                .AddWhereCondition("paz_com_codice_residenza", Comparatori.Uguale, Me.fmComuneRes.Codice, DataTypes.Stringa)
                rpt.AddParameter("Comune", fmComuneRes.Descrizione)

            Else
                rpt.AddParameter("Comune", "TUTTI")
            End If

            If Me.fmCircoscrizione.Codice <> "" And Me.fmCircoscrizione.Descrizione <> "" Then

                .AddWhereCondition("paz_cir_codice", Comparatori.Uguale, Me.fmCircoscrizione.Codice, DataTypes.Stringa)
                rpt.AddParameter("Circoscrizione", Me.fmCircoscrizione.Descrizione)

            Else
                rpt.AddParameter("Circoscrizione", "TUTTE")
            End If

            Dim obbligatorie As Boolean = Me.chklModVaccinazione.Items(0).Selected
            Dim raccomandate As Boolean = Me.chklModVaccinazione.Items(1).Selected
            Dim facoltative As Boolean = Me.chklModVaccinazione.Items(2).Selected

            If Not obbligatorie And Not raccomandate And Not facoltative Then

                rpt.AddParameter("TipoVac", "OBBLIGATORIE RACCOMANDATE E FACOLTATIVE")

            ElseIf obbligatorie And Not raccomandate And Not facoltative Then

                .AddWhereCondition("vac_obbligatoria", Comparatori.Uguale, Constants.ObbligatorietaVaccinazione.Obbligatoria, DataTypes.Stringa)
                rpt.AddParameter("TipoVac", "OBBLIGATORIE")

            ElseIf Not obbligatorie And raccomandate And Not facoltative Then

                .AddWhereCondition("vac_obbligatoria", Comparatori.Uguale, Constants.ObbligatorietaVaccinazione.Raccomandata, DataTypes.Stringa)
                rpt.AddParameter("TipoVac", "RACCOMANDATE")

            ElseIf Not obbligatorie And Not raccomandate And facoltative Then

                .AddWhereCondition("vac_obbligatoria", Comparatori.Uguale, Constants.ObbligatorietaVaccinazione.Facoltativa, DataTypes.Stringa)
                rpt.AddParameter("TipoVac", "FACOLTATIVE")

            ElseIf obbligatorie And Not raccomandate And facoltative Then

                .OpenParanthesis()
                .AddWhereCondition("vac_obbligatoria", Comparatori.Uguale, Constants.ObbligatorietaVaccinazione.Obbligatoria, DataTypes.Stringa)
                .AddWhereCondition("vac_obbligatoria", Comparatori.Uguale, Constants.ObbligatorietaVaccinazione.Facoltativa, DataTypes.Stringa, "or")
                .CloseParanthesis()
                rpt.AddParameter("TipoVac", "OBBLIGATORIE E FACOLTATIVE")

            ElseIf Not obbligatorie And raccomandate And facoltative Then

                .OpenParanthesis()
                .AddWhereCondition("vac_obbligatoria", Comparatori.Uguale, Constants.ObbligatorietaVaccinazione.Raccomandata, DataTypes.Stringa)
                .AddWhereCondition("vac_obbligatoria", Comparatori.Uguale, Constants.ObbligatorietaVaccinazione.Facoltativa, DataTypes.Stringa, "or")
                .CloseParanthesis()
                rpt.AddParameter("TipoVac", "RACCOMANDATE E FACOLTATIVE")

            ElseIf obbligatorie And raccomandate And Not facoltative Then

                .OpenParanthesis()
                .AddWhereCondition("vac_obbligatoria", Comparatori.Uguale, Constants.ObbligatorietaVaccinazione.Obbligatoria, DataTypes.Stringa)
                .AddWhereCondition("vac_obbligatoria", Comparatori.Uguale, Constants.ObbligatorietaVaccinazione.Raccomandata, DataTypes.Stringa, "or")
                .CloseParanthesis()
                rpt.AddParameter("TipoVac", "OBBLIGATORIE E RACCOMANDATE")

            Else
                rpt.AddParameter("TipoVac", "OBBLIGATORIE RACCOMANDATE E FACOLTATIVE")
            End If

            If Not String.IsNullOrEmpty(Me.ddlEsenzioniMalattie.SelectedValue) Then

                .AddWhereCondition("ves_mal_codice_malattia", Comparatori.Uguale, Me.ddlEsenzioniMalattie.SelectedValue.Split("|")(0), DataTypes.Stringa)
                .AddWhereCondition("ves_codice_esenzione", Comparatori.Uguale, Me.ddlEsenzioniMalattie.SelectedValue.Split("|")(1), DataTypes.Stringa)

                rpt.AddParameter("EsenzioniMalattie", Me.ddlEsenzioniMalattie.SelectedItem.Text)

            Else
                rpt.AddParameter("EsenzioniMalattie", "TUTTE")
            End If

            ' N.B. : se OnVacContext.UserDescription è Nothing, all'rpt deve essere passata la stringa vuota se no dà errore
            If String.IsNullOrEmpty(OnVacContext.UserDescription) Then
                rpt.AddParameter("UtenteStampa", String.Empty)
            Else
                rpt.AddParameter("UtenteStampa", OnVacContext.UserDescription)
            End If

        End With

        Dim ds As New DSVaccinazioniEseguite()

        Try
            dam.BuildDataTable(ds.VaccinazioniEseguite)
        Finally
            OnVacUtility.CloseDam(dam)
        End Try

        rpt.set_dataset(ds)

        ' Dati di installazione
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

                If Not OnVacReport.StampaReport(nomeReport, String.Empty, rpt, , , bizReport.GetReportFolder(nomeReport)) Then
                    OnVacUtility.StampaNonPresente(Page, nomeReport)
                End If

            End Using
        End Using

    End Sub

#Region " Caricamento dati "

    Private Sub CaricaDati()

        Using dam As IDAM = OnVacUtility.OpenDam()
            CaricaDdlEsenzioniMalattie(dam)
            CaricaStatiAnagrafici(dam)
        End Using

    End Sub

    Private Sub CaricaDdlEsenzioniMalattie(dam As IDAM)
        '--
        ddlEsenzioniMalattie.Items.Clear()
        '--
        dam.QB.NewQuery()
        dam.QB.AddTables("T_ANA_MALATTIE")
        dam.QB.AddSelectFields("MAL_CODICE", "MAL_DESCRIZIONE", "MAL_CODICE_ESENZIONE")
        dam.QB.AddWhereCondition("MAL_CODICE_ESENZIONE", Comparatori.IsNot, DBNull.Value, DataTypes.Stringa)
        '--
        Using reader As IDataReader = dam.BuildDataReader()
            While reader.Read()
                '--
                Dim codiceMalattia As String = reader.GetString(0)
                Dim descrizioneMalattia As String = reader.GetString(1)
                Dim codiceEsenzione As String = reader.GetString(2)
                '--
                ddlEsenzioniMalattie.Items.Add(New ListItem(String.Format("{0} - {1}", descrizioneMalattia, codiceEsenzione), String.Format("{0}|{1}", codiceMalattia, codiceEsenzione)))
                '--
            End While
        End Using
        '--
        ddlEsenzioniMalattie.Items.Insert(0, "")
        '--
    End Sub

    Private Sub CaricaStatiAnagrafici(dam As IDAM)

        Dim dtStatiAnag As DataTable

        Using genericProvider As New DAL.DbGenericProvider(dam)
            Using bizStatiAnagrafici As New Biz.BizStatiAnagrafici(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                dtStatiAnag = bizStatiAnagrafici.LeggiStatiAnagrafici()

            End Using
        End Using

        Me.chklStatoAnagrafico.DataValueField = "SAN_CODICE"
        Me.chklStatoAnagrafico.DataTextField = "SAN_DESCRIZIONE"
        Me.chklStatoAnagrafico.DataSource = dtStatiAnag
        Me.chklStatoAnagrafico.DataBind()

    End Sub

#End Region

#End Region

End Class
