Imports Onit.Database.DataAccessManager
Imports System.Collections.Generic


Partial Class EsitoCampagna
    Inherits OnVac.Common.PageBase


#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region


    ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' '
    ' Questa classe esegue le query necessarie a compilare il report statistico sulle campagne di vaccinazione.
    ' La tabella T_CNV_CAMPAGNA viene riempita con alcuni campi riassuntivi delle convocazioni tramite un trigger sulla
    ' tabella T_CNV_CONVOCAZIONI. Il campo T_CNV_CAMPAGNA.cmp_operazione con valori in [INSERT,UPD_N->S,UPD_S->N]
    ' tiene traccia delle operazioni che vengono svolte dall'utente e che possono coinvolgere la statistica.
    '
    ' Per passare i dati al report si utilizza il dataset tipizzato DS_EsitoCampagna
    ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' ' '

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then
            PanelTitolo.InnerText = OnVacUtility.Variabili.CNS.Descrizione

			ucSelezioneConsultori.MostraSoloAperti = False
			ucSelezioneConsultori.ImpostaCnsCorrente = True
			ucSelezioneConsultori.LoadGetCodici()

			ShowPrintButtons()
        End If

        'per la stampa tramite tasto invio (modifica 29/07/2004)
        Select Case Request.Form("__EVENTTARGET")
            Case "Stampa"
                If Toolbar.Items.FromKeyButton("btnStampa").Visible Then
                    Stampa()
                End If
        End Select

    End Sub


    Private Sub ShowPrintButtons()
        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()

        listPrintButtons.Add(New PrintButton(Constants.ReportName.EsitoCampagna, "btnStampa"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Toolbar)
    End Sub


    Private Sub Toolbar_ButtonClicked(ByVal sender As System.Object, ByVal e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked
        Select Case e.Button.Key
            Case "btnStampa"
                Stampa()
        End Select
    End Sub


    Private Sub Stampa()

        Dim rpt As New ReportParameter()

        Dim dam As IDAM = OnVacUtility.OpenDam()

        Dim rptFolder As String = String.Empty

        Try
            ' Dati del report
            Dim dsTip As DS_EsitoCampagna = CreaDatasetEsitoCampagna(dam)
            rpt.set_dataset(dsTip)

            ' Parametri per intestazione
            AddHeaderParameters(rpt)

            ' Lettura directory report
            Using genericProvider As New DAL.DbGenericProvider(dam)
                Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                    rptFolder = bizReport.GetReportFolder(Constants.ReportName.EsitoCampagna)

                End Using
            End Using

        Finally
            OnVacUtility.CloseDam(dam)
        End Try

        rpt.AddParameter("PeriodoVac1", odpDataEffettuazioneIniz.Text)
        rpt.AddParameter("PeriodoVac2", odpDataEffettuazioneFin.Text)
        rpt.AddParameter("Data", odpData.Text)
        rpt.AddParameter("dataNascitaIniz", odpDataNascitaDa.Text)
        rpt.AddParameter("dataNascitaFin", odpDataNascitaA.Text)

        If ucSelezioneConsultori.GetConsultoriSelezionati.Count > 0 Then
            rpt.AddParameter("Consultorio", ucSelezioneConsultori.GetDescrizioneConsultoriSelezionatiPerStampe())
        Else
            rpt.AddParameter("Consultorio", "TUTTI")
        End If

        If Not OnVacReport.StampaReport(Constants.ReportName.EsitoCampagna, String.Empty, rpt, , , rptFolder) Then
            OnVacUtility.StampaNonPresente(Page, Constants.ReportName.EsitoCampagna)
        End If

    End Sub


#Region " Creazione dataset "

    Private Function CreaDatasetEsitoCampagna(ByVal dam As IDAM) As DS_EsitoCampagna
        Dim dsTip As New DS_EsitoCampagna()

        Dim dtStampa As New DataTable()
        dtStampa = dsTip.DS_EsitoCampagna

        strutturaDtPerStampa(dtStampa, dam)

        ricavaNChiamati(dtStampa, dam)
        ricavaNAvvisati(dtStampa, dam)
        ricavaNVaccinati(dtStampa, dam)

        Dim dsUtility As New OnVac.Common.Utility.DataSetHelper()
        dsUtility.DBNull2Zero(dtStampa)
        dsUtility.Dispose()

        ricavaNNonVaccinati(dtStampa)

        dsTip.AcceptChanges()

        Return dsTip
    End Function

    Private Sub ricavaNNonVaccinati(ByRef dt As DataTable)
        For j As Integer = 0 To dt.Rows.Count - 1
            dt.Rows(j)("NNonVaccinati") = dt.Rows(j)("NChiamati") - dt.Rows(j)("NVaccinati")
        Next
    End Sub

    Private Sub strutturaDtPerStampa(ByRef dt As DataTable, ByRef dam As IDAM)
        With dam.QB
            .NewQuery()
            .AddSelectFields("CNS_CODICE", "CNS_DESCRIZIONE", "''", "''", "''", "''")
			.AddTables("T_ANA_CONSULTORI")
			Dim lista As List(Of String) = ucSelezioneConsultori.GetConsultoriSelezionati
			If lista.Count > 0 Then
				Dim filtroIn As New System.Text.StringBuilder()
				For i As Integer = 0 To lista.Count - 1
					filtroIn.AppendFormat("{0},", .AddCustomParam(lista(i)))
				Next

				If filtroIn.Length > 0 Then
					filtroIn.Remove(filtroIn.Length - 1, 1)
					.AddWhereCondition("CNS_CODICE", Comparatori.In, filtroIn.ToString(), DataTypes.Replace)
				End If
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
								.AddWhereCondition("CNS_CODICE", Comparatori.In, filtroIn.ToString(), DataTypes.Replace)
							End If
						End If

					End Using
				End Using
			End If
		End With

        dam.BuildDataTable(dt)
    End Sub

    Private Sub ricavaNChiamati(ByRef dt As DataTable, ByRef dam As IDAM)
        Dim dtInterno As New DataTable()
        With dam.QB
            .NewQuery()
            .AddSelectFields("DECODE (cmp_operazione,'UPD_S->N',-COUNT(cmp_paz_codice),'UPD_N->S', COUNT(cmp_paz_codice),'INSERT', COUNT(cmp_paz_codice))as num")
            .AddSelectFields("CMP_CNS_CODICE")
            .AddTables("T_PAZ_PAZIENTI", "T_CNV_CAMPAGNA")
            .AddWhereCondition("PAZ_CODICE", Comparatori.Uguale, "CMP_PAZ_CODICE", DataTypes.Join)
            .AddWhereCondition("PAZ_DATA_NASCITA", Comparatori.MaggioreUguale, odpDataNascitaDa.Text, DataTypes.Data)
            .AddWhereCondition("PAZ_DATA_NASCITA", Comparatori.MinoreUguale, odpDataNascitaA.Text, DataTypes.Data)
            If odpData.Text <> "" Then
                .AddWhereCondition("CMP_CNV_DATA", Comparatori.Uguale, odpData.Text, DataTypes.Data)
            End If
            .AddGroupByFields("CMP_CNS_CODICE", "CMP_OPERAZIONE")
            Dim sqlStmt As String = dam.QB.GetSelect
            .NewQuery(False, False)
            .AddSelectFields("SUM(num) as numero", "CMP_CNS_CODICE")
            .AddTables("(" & sqlStmt & ")")
            .AddGroupByFields("CMP_CNS_CODICE")
        End With

        dam.BuildDataTable(dtInterno)

        Dim i, j As Integer
        For i = 0 To dtInterno.Rows.Count - 1
            For j = 0 To dt.Rows.Count - 1
                If dt.Rows(j)("CNS_CODICE") = dtInterno.Rows(i)("CMP_CNS_CODICE") Then
                    dt.Rows(j)("NChiamati") = dtInterno.Rows(i)("numero")
                    Exit For
                End If
            Next
        Next

        dt.AcceptChanges()
    End Sub

    Private Sub ricavaNAvvisati(ByRef dt As DataTable, ByRef dam As IDAM)
        Dim dtInterno As New DataTable()

        With dam.QB
            .NewQuery()
            .AddSelectFields("DECODE (cmp_operazione,'UPD_S->N',-COUNT(cmp_paz_codice),'UPD_N->S', COUNT(cmp_paz_codice),'INSERT', COUNT(cmp_paz_codice))as num")
            .AddSelectFields("CMP_CNS_CODICE")
            .AddTables("T_PAZ_PAZIENTI", "T_CNV_CAMPAGNA")
            .AddWhereCondition("PAZ_CODICE", Comparatori.Uguale, "CMP_PAZ_CODICE", DataTypes.Join)
            .AddWhereCondition("PAZ_DATA_NASCITA", Comparatori.MaggioreUguale, odpDataNascitaDa.Text, DataTypes.Data)
            .AddWhereCondition("PAZ_DATA_NASCITA", Comparatori.MinoreUguale, odpDataNascitaA.Text, DataTypes.Data)
            .AddWhereCondition("CMP_CNV_DATA_INVIO", Comparatori.[IsNot], "null", DataTypes.Replace)
            If odpData.Text <> "" Then
                .AddWhereCondition("CMP_CNV_DATA", Comparatori.Uguale, odpData.Text, DataTypes.Data)
            End If
            .AddGroupByFields("CMP_CNS_CODICE", "CMP_OPERAZIONE")

            Dim sqlStmt As String = dam.QB.GetSelect()

            .NewQuery(False, False)
            .AddSelectFields("SUM(num) as numero", "CMP_CNS_CODICE")
            .AddTables("(" & sqlStmt & ")")
            .AddGroupByFields("CMP_CNS_CODICE")
        End With

        dam.BuildDataTable(dtInterno)

        Dim i, j As Integer
        For i = 0 To dtInterno.Rows.Count - 1
            For j = 0 To dt.Rows.Count - 1
                If dt.Rows(j)("CNS_CODICE") = dtInterno.Rows(i)("CMP_CNS_CODICE") Then
                    dt.Rows(j)("NAvvisati") = dtInterno.Rows(i)("numero")
                    Exit For
                End If
            Next
        Next

        dt.AcceptChanges()
    End Sub

    Private Sub ricavaNVaccinati(ByRef dt As DataTable, ByRef dam As IDAM)
        Dim dtInterno As New DataTable()

        With dam.QB
            .NewQuery()
            .AddSelectFields("count(distinct VES_PAZ_CODICE) as numero,CMP_CNS_CODICE")
            .AddTables("T_PAZ_PAZIENTI", "T_VAC_ESEGUITE", "T_CNV_CAMPAGNA")
            .AddWhereCondition("PAZ_CODICE", Comparatori.Uguale, "VES_PAZ_CODICE", DataTypes.Join)
            .AddWhereCondition("PAZ_DATA_NASCITA", Comparatori.MaggioreUguale, odpDataNascitaDa.Text, DataTypes.Data)
            .AddWhereCondition("PAZ_DATA_NASCITA", Comparatori.MinoreUguale, odpDataNascitaA.Text, DataTypes.Data)
            .AddWhereCondition("VES_CNV_DATA", Comparatori.Uguale, "CMP_CNV_DATA", DataTypes.Join)
            If odpData.Text <> "" Then .AddWhereCondition("VES_CNV_DATA", Comparatori.Uguale, odpData.Text, DataTypes.Data)
            .AddWhereCondition("VES_IN_CAMPAGNA", Comparatori.Uguale, "S", DataTypes.Stringa)
            If odpDataEffettuazioneIniz.Text <> "" Then
                .AddWhereCondition("VES_DATA_EFFETTUAZIONE", Comparatori.MaggioreUguale, odpDataEffettuazioneIniz.Text, DataTypes.Data)
            End If
            If odpDataEffettuazioneFin.Text <> "" Then
                .AddWhereCondition("VES_DATA_EFFETTUAZIONE", Comparatori.MinoreUguale, odpDataEffettuazioneFin.Text, DataTypes.Data)
            End If
            .AddGroupByFields("CMP_CNS_CODICE")
        End With

        dam.BuildDataTable(dtInterno)

        Dim i, j As Integer
        For i = 0 To dtInterno.Rows.Count - 1
            For j = 0 To dt.Rows.Count - 1
                If dt.Rows(j)("CNS_CODICE") = dtInterno.Rows(i)("CMP_CNS_CODICE") Then
                    dt.Rows(j)("NVaccinati") = dtInterno.Rows(i)("numero")
                    Exit For
                End If
            Next
        Next

    End Sub

#End Region


#Region " Lettura parametri intestazione report "

    Private Sub AddHeaderParameters(ByRef rpt As ReportParameter)

        Dim installazione As Entities.Installazione = OnVacUtility.GetDatiInstallazioneCorrente(Settings)

        If installazione Is Nothing Then
            rpt.AddParameter("UslCitta", String.Empty)
            rpt.AddParameter("UslDesc", String.Empty)
            rpt.AddParameter("UslReg", String.Empty)
        Else
            rpt.AddParameter("UslCitta", installazione.UslCitta)
            rpt.AddParameter("UslDesc", installazione.UslDescrizionePerReport)
            rpt.AddParameter("UslReg", installazione.UslRegione)
        End If

    End Sub

#End Region


End Class
