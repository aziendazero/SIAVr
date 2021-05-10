Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.DAL

Partial Class ConteggioVaccinazioni
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

			ShowPrintButtons()
        End If

        'per la stampa tramite tasto invio (modifica 29/07/2004)
        Select Case Request.Form("__EVENTTARGET")
            Case "Stampa"
                If Toolbar.Items.FromKeyButton("btnStampa").Visible Then
                    Stampa(Constants.ReportName.ConteggioSedute)
                End If
        End Select

    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub Toolbar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked
        Select Case e.Button.Key
            Case "btnStampa"
                Stampa(Constants.ReportName.ConteggioSedute)
        End Select
    End Sub

#End Region


#Region " Private "

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()

        listPrintButtons.Add(New PrintButton(Constants.ReportName.ConteggioSedute, "btnStampa"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Toolbar)

    End Sub
    Private Function queryPar(codiceCns As String, parametro As String) As String
        Dim query As String

        query = String.Format(
                        "(select par_valore
                          from t_ana_parametri par1
                          where (par1.par_cns_codice = NVL({1},'{0}') or par1.par_cns_codice = '{0}')
                          and par1.par_codice = '{2}'
                          and not exists (
                                select * from t_ana_parametri par2
                                where par2.par_codice = par1.par_codice
                                and par1.par_cns_codice = '{0}'
                                and par2.par_cns_codice = {1}
                                and par2.par_codice = '{2}'
                            ))", Constants.CommonConstants.CodiceConsultorioSistema, codiceCns, parametro)

        Return query
    End Function
    Private Function contaSeduteDellaGiornata(numeroPrefisso As String, mattina As Boolean, pomeriggio As Boolean, codiceCns As String) As String
        Dim numeroSeduteGiorno As String
        Dim whereDistretto As String
        Dim whereStato As String
        Dim whereTipo As String
        Dim whereConsultorio As String
        Dim parOrarioPM As String
        parOrarioPM = queryPar(codiceCns, "ORAPM")

        numeroSeduteGiorno = String.Format("Select count(*) from V_VAC_ESEGUITE_UNION_SCADUTE a{0},t_paz_pazienti b{0}, t_ana_consultori c{0}, t_report_gruppo d{0}, t_ana_circoscrizioni e{0}
WHERE a{0}.ves_paz_codice = b{0}.paz_codice 
And b{0}.paz_cir_codice = e{0}.cir_codice(+) 
And a{0}.ves_cns_codice = c{0}.cns_codice(+) 
And c{0}.cns_codice = d{0}.gruppo(+) 
And a{0}.ves_data_effettuazione= V_VAC_ESEGUITE_UNION_SCADUTE.ves_data_effettuazione 
And NVL(a{0}.ves_cns_codice,'0')=NVL(V_VAC_ESEGUITE_UNION_SCADUTE.ves_cns_codice,'0') 
and b{0}.paz_codice=V_VAC_ESEGUITE_UNION_SCADUTE.VES_paz_codice ", numeroPrefisso)
        If mattina Then
            numeroSeduteGiorno = numeroSeduteGiorno + String.Format(" And to_char(a{0}.ves_dataora_effettuazione,'hh24:mi') <= {1} ", numeroPrefisso, parOrarioPM)
        End If
        If pomeriggio Then
            numeroSeduteGiorno = numeroSeduteGiorno + String.Format(" And to_char(a{0}.ves_dataora_effettuazione,'hh24:mi') >= {1} ", numeroPrefisso, parOrarioPM)
        End If

        If Me.fmDistretto.Codice <> "" And Me.fmDistretto.Descrizione <> "" Then
            whereDistretto = String.Format(" And c{0}.cns_dis_codice = '{1}'", numeroPrefisso, fmDistretto.Codice)
            numeroSeduteGiorno = numeroSeduteGiorno + whereDistretto
        End If
        'filtri per tipo vaccinazione (VES_STATO: R - Registrata, altrimenti Eseguita)
        If (Me.chklStatoVac.Items(0).Selected = True) And (Me.chklStatoVac.Items(1).Selected = False) Then
            whereStato = String.Format(" and (a{0}.ves_stato IS NULL or a{0}.ves_stato <> 'R') ", numeroPrefisso)
            numeroSeduteGiorno = numeroSeduteGiorno + whereStato
        ElseIf (Me.chklStatoVac.Items(0).Selected = False) And (Me.chklStatoVac.Items(1).Selected = True) Then
            whereStato = String.Format(" and a{0}.ves_stato = 'R' ", numeroPrefisso)
            numeroSeduteGiorno = numeroSeduteGiorno + whereStato
        End If

        ' filtri per tipo consultorio (A - adulti, P - pediatrico, V - vaccinatore)
        If (Me.chklTipoCns.Items(0).Selected = True) And (Me.chklTipoCns.Items(1).Selected = True) And (Me.chklTipoCns.Items(2).Selected = False) Then

            whereTipo = String.Format(" and ( c{0}.cns_tipo = 'A' or c{0}.cns_tipo = 'P') ", numeroPrefisso)
            numeroSeduteGiorno = numeroSeduteGiorno + whereTipo
        ElseIf (Me.chklTipoCns.Items(0).Selected = True) And (Me.chklTipoCns.Items(1).Selected = False) And (Me.chklTipoCns.Items(2).Selected = True) Then

            whereTipo = String.Format(" and ( c{0}.cns_tipo = 'A' or c{0}.cns_tipo = 'V') ", numeroPrefisso)
            numeroSeduteGiorno = numeroSeduteGiorno + whereTipo
        ElseIf (Me.chklTipoCns.Items(0).Selected = True) And (Me.chklTipoCns.Items(1).Selected = False) And (Me.chklTipoCns.Items(2).Selected = False) Then
            whereTipo = String.Format(" and c{0}.cns_tipo = 'A' ", numeroPrefisso)
            numeroSeduteGiorno = numeroSeduteGiorno + whereTipo

        ElseIf (Me.chklTipoCns.Items(0).Selected = False) And (Me.chklTipoCns.Items(1).Selected = True) And (Me.chklTipoCns.Items(2).Selected = False) Then

            whereTipo = String.Format(" and c{0}.cns_tipo = 'P' ", numeroPrefisso)
            numeroSeduteGiorno = numeroSeduteGiorno + whereTipo

        ElseIf (Me.chklTipoCns.Items(0).Selected = False) And (Me.chklTipoCns.Items(1).Selected = False) And (Me.chklTipoCns.Items(2).Selected = True) Then

            whereTipo = String.Format(" and c{0}.cns_tipo = 'V' ", numeroPrefisso)
            numeroSeduteGiorno = numeroSeduteGiorno + whereTipo

        ElseIf (Me.chklTipoCns.Items(0).Selected = False) And (Me.chklTipoCns.Items(1).Selected = True) And (Me.chklTipoCns.Items(2).Selected = True) Then

            whereTipo = String.Format(" and ( c{0}.cns_tipo = 'P' or c{0}.cns_tipo = 'V') ", numeroPrefisso)
            numeroSeduteGiorno = numeroSeduteGiorno + whereTipo

        End If
		Dim lista As List(Of String) = ucSelezioneConsultori.GetConsultoriSelezionati
		If lista.Count > 0 Then
			whereConsultorio = String.Format(" and (a{0}.ves_cns_codice IN ('{1}')) ", numeroPrefisso, lista.Aggregate(Function(p, g) p & "', '" & g))
			numeroSeduteGiorno = numeroSeduteGiorno + whereConsultorio
		Else
			Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
				Using bizCns As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)
					lista = bizCns.GetListCodiceConsultoriUtente(OnVacContext.UserId)
					If lista.Count > 0 Then
						whereConsultorio = String.Format(" and (a{0}.ves_cns_codice IN ('{1}')) ", numeroPrefisso, lista.Aggregate(Function(p, g) p & "', '" & g))
						numeroSeduteGiorno = numeroSeduteGiorno + whereConsultorio
					End If

				End Using
			End Using
		End If
		numeroSeduteGiorno = numeroSeduteGiorno + String.Format(" group by b{0}.paz_codice", numeroPrefisso)

        Return numeroSeduteGiorno
    End Function

    'procedura richiamabile anche dal page_load (modifica 29/07/2004)
    Private Sub Stampa(nomeReport As String)

        Dim rpt As New ReportParameter()
		'Dim parOrarioPM As String = queryPar("V_VAC_ESEGUITE_UNION_SCADUTE.ves_cns_codice", "ORAPM")
		'Dim parNumeroVacFasciaOraria As String = queryPar("V_VAC_ESEGUITE_UNION_SCADUTE.ves_cns_codice", "NUM_MIN_VAC_PER_FASCIA_ORARIA")
		Dim ds As New DSConteggioSedute()
		'    Using dam As IDAM = OnVacUtility.OpenDam()


		'        Dim mattino As String = String.Format("CASE WHEN(to_char(ves_dataora_effettuazione,'hh24:mi') < {0} and ({1}) >= {2}) then 1 else 0 end as mattino", parOrarioPM, contaSeduteDellaGiornata("1", True, False, "V_VAC_ESEGUITE_UNION_SCADUTE.ves_cns_codice"), parNumeroVacFasciaOraria)
		'        Dim pomeriggio As String = String.Format("CASE WHEN(to_char(ves_dataora_effettuazione,'hh24:mi') > {0} and ({1}) >= {2} ) then 1 else 0 end as pomeriggio", parOrarioPM, contaSeduteDellaGiornata("2", False, True, "V_VAC_ESEGUITE_UNION_SCADUTE.ves_cns_codice"), parNumeroVacFasciaOraria)

		'        With dam.QB
		'            .NewQuery()
		'            .IsDistinct = True
		'.AddSelectFields("ves_cns_codice,cns_descrizione,ves_dataora_effettuazione,ves_paz_codice ", mattino, pomeriggio)
		'.AddTables("V_VAC_ESEGUITE_UNION_SCADUTE, t_paz_pazienti, t_ana_consultori, t_report_gruppo, t_ana_circoscrizioni")
		'            .AddWhereCondition("ves_paz_codice", Comparatori.Uguale, "paz_codice", DataTypes.Join)
		'            .AddWhereCondition("paz_cir_codice", Comparatori.Uguale, "cir_codice", DataTypes.OutJoinLeft)
		'            '.AddWhereCondition("paz_com_codice_residenza", Comparatori.Uguale, "com_codice", DataTypes.OutJoinLeft)
		'            '.AddWhereCondition("ves_vac_codice", Comparatori.Uguale, "vac_codice", DataTypes.Join)
		'            .AddWhereCondition("ves_cns_codice", Comparatori.Uguale, "cns_codice", DataTypes.OutJoinLeft)
		'            .AddWhereCondition("cns_codice", Comparatori.Uguale, "gruppo", DataTypes.OutJoinLeft)

		'            rpt.AddParameter("PeriodoVac1", odpDataEffettuazioneIniz.Text)
		'            rpt.AddParameter("PeriodoVac2", odpDataEffettuazioneFin.Text)

		'            .AddWhereCondition("ves_data_effettuazione", Comparatori.MaggioreUguale, Me.odpDataEffettuazioneIniz.Data, DataTypes.Data)
		'            .AddWhereCondition("ves_data_effettuazione", Comparatori.MinoreUguale, Me.odpDataEffettuazioneFin.Data, DataTypes.Data)

		'            ' filtro per distretto
		'            If Me.fmDistretto.Codice <> "" And Me.fmDistretto.Descrizione <> "" Then
		'                .AddWhereCondition("cns_dis_codice", Comparatori.Uguale, Me.fmDistretto.Codice, DataTypes.Stringa)
		'                rpt.AddParameter("Distretto", Me.fmDistretto.Descrizione)
		'            Else
		'                rpt.AddParameter("Distretto", "TUTTI")
		'            End If

		'            'filtri per tipo vaccinazione (VES_STATO: R - Registrata, altrimenti Eseguita)
		'            If (Me.chklStatoVac.Items(0).Selected = True) And (Me.chklStatoVac.Items(1).Selected = False) Then
		'                .OpenParanthesis()
		'                .AddWhereCondition("ves_stato", Comparatori.Is, "NULL", DataTypes.Stringa)
		'                .AddWhereCondition("ves_stato", Comparatori.Diverso, "R", DataTypes.Stringa, "OR")
		'                .CloseParanthesis()
		'            ElseIf (Me.chklStatoVac.Items(0).Selected = False) And (Me.chklStatoVac.Items(1).Selected = True) Then
		'                .AddWhereCondition("ves_stato", Comparatori.Uguale, "R", DataTypes.Stringa)
		'            End If

		'            ' filtri per tipo consultorio (A - adulti, P - pediatrico, V - vaccinatore)
		'            If (Me.chklTipoCns.Items(0).Selected = True) And (Me.chklTipoCns.Items(1).Selected = True) And (Me.chklTipoCns.Items(2).Selected = False) Then

		'                .OpenParanthesis()
		'                .AddWhereCondition("cns_tipo", Comparatori.Uguale, "A", DataTypes.Stringa)
		'                .AddWhereCondition("cns_tipo", Comparatori.Uguale, "P", DataTypes.Stringa, "or")
		'                .CloseParanthesis()

		'                rpt.AddParameter("TipoCns", "ADULTI E PEDIATRICO")

		'            ElseIf (Me.chklTipoCns.Items(0).Selected = True) And (Me.chklTipoCns.Items(1).Selected = False) And (Me.chklTipoCns.Items(2).Selected = True) Then

		'                .OpenParanthesis()
		'                .AddWhereCondition("cns_tipo", Comparatori.Uguale, "A", DataTypes.Stringa)
		'                .AddWhereCondition("cns_tipo", Comparatori.Uguale, "V", DataTypes.Stringa, "or")
		'                .CloseParanthesis()
		'                rpt.AddParameter("TipoCns", "ADULTI E VACCINATORE")

		'            ElseIf (Me.chklTipoCns.Items(0).Selected = True) And (Me.chklTipoCns.Items(1).Selected = False) And (Me.chklTipoCns.Items(2).Selected = False) Then

		'                .AddWhereCondition("cns_tipo", Comparatori.Uguale, "A", DataTypes.Stringa)
		'                rpt.AddParameter("TipoCns", "ADULTI")

		'            ElseIf (Me.chklTipoCns.Items(0).Selected = False) And (Me.chklTipoCns.Items(1).Selected = True) And (Me.chklTipoCns.Items(2).Selected = False) Then

		'                .AddWhereCondition("cns_tipo", Comparatori.Uguale, "P", DataTypes.Stringa)
		'                rpt.AddParameter("TipoCns", "PEDIATRICO")

		'            ElseIf (Me.chklTipoCns.Items(0).Selected = False) And (Me.chklTipoCns.Items(1).Selected = False) And (Me.chklTipoCns.Items(2).Selected = True) Then

		'                .AddWhereCondition("cns_tipo", Comparatori.Uguale, "V", DataTypes.Stringa)
		'                rpt.AddParameter("TipoCns", "VACCINATORE")

		'            ElseIf (Me.chklTipoCns.Items(0).Selected = False) And (Me.chklTipoCns.Items(1).Selected = True) And (Me.chklTipoCns.Items(2).Selected = True) Then

		'                .OpenParanthesis()
		'                .AddWhereCondition("cns_tipo", Comparatori.Uguale, "P", DataTypes.Stringa)
		'                .AddWhereCondition("cns_tipo", Comparatori.Uguale, "V", DataTypes.Stringa, "or")
		'                .CloseParanthesis()
		'                rpt.AddParameter("TipoCns", "PEDIATRICO E VACCINATORE")

		'            Else
		'                rpt.AddParameter("TipoCns", "ADULTI PEDIATRICO E VACCINATORE")
		'            End If
		'Dim lista As List(Of String) = ucSelezioneConsultori.GetConsultoriSelezionati
		'If lista.Count > 0 Then

		'	Dim filtroIn As New System.Text.StringBuilder()
		'	For i As Integer = 0 To lista.Count - 1
		'		filtroIn.AppendFormat("{0},", .AddCustomParam(lista(i)))
		'	Next
		'	If filtroIn.Length > 0 Then
		'		filtroIn.Remove(filtroIn.Length - 1, 1)
		'		.AddWhereCondition("ves_cns_codice", Comparatori.In, filtroIn.ToString(), DataTypes.Replace)
		'	End If
		'	rpt.AddParameter("Consultorio", ucSelezioneConsultori.GetDescrizioneConsultoriSelezionatiPerStampe())

		'Else
		'	Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
		'		Using bizCns As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)
		'			lista = bizCns.GetListCodiceConsultoriUtente(OnVacContext.UserId)
		'			If lista.Count > 0 Then
		'				Dim filtroIn As New System.Text.StringBuilder()
		'				For i As Integer = 0 To lista.Count - 1
		'					filtroIn.AppendFormat("{0},", .AddCustomParam(lista(i)))
		'				Next
		'				If filtroIn.Length > 0 Then
		'					filtroIn.Remove(filtroIn.Length - 1, 1)
		'					.AddWhereCondition("ves_cns_codice", Comparatori.In, filtroIn.ToString(), DataTypes.Replace)
		'				End If
		'			End If

		'		End Using
		'	End Using
		'	rpt.AddParameter("Consultorio", "TUTTI")
		'            End If

		'            ' N.B. : se OnVacContext.UserDescription è Nothing, all'rpt deve essere passata la stringa vuota se no dà errore
		'            If String.IsNullOrEmpty(OnVacContext.UserDescription) Then
		'                rpt.AddParameter("UtenteStampa", String.Empty)
		'            Else
		'                rpt.AddParameter("UtenteStampa", OnVacContext.UserDescription)
		'            End If

		'        End With

		'        dam.BuildDataTable(ds.ConteggioSedute)

		'    End Using

		'    rpt.set_dataset(ds)

		' Dati di installazione
		Dim installazione As Entities.Installazione = OnVacUtility.GetDatiInstallazioneCorrente(Me.Settings)


		' Stampa
		Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
			' aggiungere tutta la parte nuova
			Dim ConteggioResult As Biz.BizStampaConteggioSedute.ConteggioSeduteResult = Nothing

			Using bizStampaConteggio As New Biz.BizStampaConteggioSedute(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

				Dim filtriConteggio As New IStampaConteggioSedute.FiltriConteggioSedute()

				' Date di nascita
				filtriConteggio.DataEffettuazioneInizio = Me.odpDataEffettuazioneIniz.Data
				filtriConteggio.DataEffettuazioneFine = odpDataEffettuazioneFin.Data

				' Consultorio
				Dim lista As List(Of String) = ucSelezioneConsultori.GetConsultoriSelezionati
				If lista.Count = 0 Then

					Using bizCns As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)
						lista = bizCns.GetListCodiceConsultoriUtente(OnVacContext.UserId)
						If lista.Count > 0 Then
							filtriConteggio.CodiceConsultorio = lista
							filtriConteggio.DescrizioneConsultori = "TUTTI"
						End If
					End Using
				Else
					filtriConteggio.CodiceConsultorio = lista
					filtriConteggio.DescrizioneConsultori = ucSelezioneConsultori.GetDescrizioneConsultoriSelezionatiPerStampe()
				End If

				' filtri per tipo consultorio (A - adulti, P - pediatrico, V - vaccinatore)
				filtriConteggio.FlagTipoCentroAdulti = chklTipoCns.Items(0).Selected
				filtriConteggio.FlagTipoCentroPediatrico = chklTipoCns.Items(1).Selected
				filtriConteggio.FlagTipoCentroVaccinatore = chklTipoCns.Items(2).Selected

				' Distretto
				filtriConteggio.CodiceDistretto = fmDistretto.Codice
				filtriConteggio.DescrizioneDistretto = fmDistretto.Descrizione

				'filtri per tipo vaccinazione (VES_STATO: R - Registrata, altrimenti Eseguita)
				filtriConteggio.FlagVacciniEseguiti = chklStatoVac.Items(0).Selected
				filtriConteggio.FlagVacciniRegistrati = chklStatoVac.Items(1).Selected

				ConteggioResult = bizStampaConteggio.GetConteggioSedute(nomeReport, filtriConteggio)

			End Using
			rpt.set_dataset(ConteggioResult.DataSetElencoEsclusione)
			If Not ConteggioResult.ParametriReport Is Nothing AndAlso ConteggioResult.ParametriReport.Count > 0 Then

				For Each parametro As KeyValuePair(Of String, String) In ConteggioResult.ParametriReport
					rpt.AddParameter(parametro.Key, parametro.Value)
				Next

			End If


			If installazione Is Nothing Then
				rpt.AddParameter("UslCitta", String.Empty)
				rpt.AddParameter("UslDesc", String.Empty)
				rpt.AddParameter("UslReg", String.Empty)
			Else
				rpt.AddParameter("UslCitta", installazione.UslCitta)
				rpt.AddParameter("UslDesc", installazione.UslDescrizionePerReport)
				rpt.AddParameter("UslReg", installazione.UslRegione)
			End If
			' N.B. : se OnVacContext.UserDescription è Nothing, all'rpt deve essere passata la stringa vuota se no dà errore
			If String.IsNullOrEmpty(OnVacContext.UserDescription) Then
				rpt.AddParameter("UtenteStampa", String.Empty)
			Else
				rpt.AddParameter("UtenteStampa", OnVacContext.UserDescription)
			End If
			Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

				If Not OnVacReport.StampaReport(nomeReport, String.Empty, rpt, , , bizReport.GetReportFolder(nomeReport)) Then
					OnVacUtility.StampaNonPresente(Page, nomeReport)
				End If

			End Using
		End Using

    End Sub

#End Region

End Class
