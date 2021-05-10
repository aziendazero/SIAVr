Imports System.Resources
Imports System.Reflection
Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.Controls


Partial Class StatVaccinazioniRifiutate
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

#Region " Page Events "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            Me.PanelTitolo.InnerText = OnVacUtility.Variabili.CNS.Descrizione

            CaricaVaccinazioni()

            ShowPrintButtons()

        End If

        'lancio della stampa tramite tasto invio (modifica 29/07/2004)
        Select Case Request.Form("__EVENTTARGET")
            Case "Stampa"
                If Toolbar.Items.FromKeyButton("btnStampa").Visible Then
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
    Private Sub fmConsultorio_Change(sender As Object, e As Onit.Controls.OnitModalList.ModalListaEventArgument) Handles fmConsultorio.Change
        OnVacUtility.DisabilitaModale(Me.fmDistretto, IIf(Me.fmConsultorio.Codice <> "" And Me.fmConsultorio.Descrizione <> "", True, False))
    End Sub

    'se è valorizzato il distretto, deve disabilitare il consultorio [modifica 06/07/2005]
    Private Sub fmDistretto_Change(sender As Object, e As Onit.Controls.OnitModalList.ModalListaEventArgument) Handles fmDistretto.Change
        OnVacUtility.DisabilitaModale(Me.fmConsultorio, IIf(Me.fmDistretto.Codice <> "" And Me.fmDistretto.Descrizione <> "", True, False))
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
        listPrintButtons.Add(New PrintButton(Constants.ReportName.VaccinazioniRifiutate, "btnStampa"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Toolbar)

    End Sub

    Private Sub CaricaVaccinazioni()

        Dim dtVaccini As DataTable = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            dtVaccini = genericProvider.AnaVaccinazioni.GetDataTableCodiceDescrizioneVaccinazioni("VAC_DESCRIZIONE")
        End Using

        Dim emptyRow As DataRow = dtVaccini.NewRow()
        emptyRow("VAC_CODICE") = "TUTTE"
        emptyRow("VAC_DESCRIZIONE") = "TUTTE"

        dtVaccini.Rows.InsertAt(emptyRow, 0)

        Me.cmbVaccinazioni.DataValueField = "VAC_CODICE"
        Me.cmbVaccinazioni.DataTextField = "VAC_DESCRIZIONE"

        Me.cmbVaccinazioni.DataSource = dtVaccini
        Me.cmbVaccinazioni.DataBind()

    End Sub

    Private Sub Stampa()

        Dim rpt As New ReportParameter()

        rpt.AddParameter("DataNascitaIniz", odpDataNascitaIniz.Text)
        rpt.AddParameter("DataNascitaFin", odpDataNascitaFin.Text)

        Dim DAM As IDAM = OnVacUtility.OpenDam()

        DAM.QB.NewQuery()
        DAM.QB.AddSelectFields("PAZ_CODICE", "PAZ_NOME", "PAZ_COGNOME", "PAZ_DATA_NASCITA", "PAZ_INDIRIZZO_RESIDENZA", _
                                "prf_genitore", "prf_note_rifiuto", "prf_data_rifiuto", "VAC_DESCRIZIONE", "prf_n_richiamo", _
                                "VAC_OBBLIGATORIA", "CNS_DESCRIZIONE", "COM_DESCRIZIONE", "UTE_DESCRIZIONE")

        DAM.QB.AddTables("T_PAZ_RIFIUTI, T_PAZ_PAZIENTI, T_ANA_VACCINAZIONI, T_ANA_CONSULTORI, T_ANA_COMUNI, T_ANA_UTENTI")
        DAM.QB.AddWhereCondition("PRF_PAZ_CODICE", Comparatori.Uguale, "PAZ_CODICE", DataTypes.Join)
        DAM.QB.AddWhereCondition("PRF_VAC_CODICE", Comparatori.Uguale, "VAC_CODICE", DataTypes.Join)
        DAM.QB.AddWhereCondition("PAZ_COM_CODICE_RESIDENZA", Comparatori.Uguale, "COM_CODICE", DataTypes.OutJoinLeft)
        DAM.QB.AddWhereCondition("PAZ_CNS_CODICE", Comparatori.Uguale, "CNS_CODICE", DataTypes.OutJoinLeft)
        DAM.QB.AddWhereCondition("PRF_UTE_ID", Comparatori.Uguale, "UTE_ID", DataTypes.OutJoinLeft)
        DAM.QB.AddWhereCondition("PAZ_DATA_NASCITA", Comparatori.MaggioreUguale, odpDataNascitaIniz.Text, DataTypes.Data)
        DAM.QB.AddWhereCondition("PAZ_DATA_NASCITA", Comparatori.MinoreUguale, odpDataNascitaFin.Text, DataTypes.Data)

        'filtro sul consultorio
        If fmConsultorio.Codice <> "" And fmConsultorio.Descrizione <> "" Then
            DAM.QB.AddWhereCondition("T_PAZ_PAZIENTI.PAZ_CNS_CODICE", Comparatori.Uguale, fmConsultorio.Codice, DataTypes.Stringa)
            rpt.AddParameter("Consultorio", fmConsultorio.Descrizione)
        Else
            rpt.AddParameter("Consultorio", "TUTTI")
        End If

        'filtro sul comune di residenza
        If fmComuneRes.Codice <> "" And fmComuneRes.Descrizione <> "" Then
            DAM.QB.AddWhereCondition("T_PAZ_PAZIENTI.PAZ_COM_CODICE_RESIDENZA", Comparatori.Uguale, fmComuneRes.Codice, DataTypes.Stringa)
            rpt.AddParameter("ComRes", fmComuneRes.Descrizione)
        Else
            rpt.AddParameter("ComRes", "TUTTI")
        End If

        'filtro sulla circoscrizione
        If fmCircoscrizione.Codice <> "" And fmCircoscrizione.Descrizione <> "" Then
            DAM.QB.AddWhereCondition("T_PAZ_PAZIENTI.PAZ_CIR_CODICE", Comparatori.Uguale, fmCircoscrizione.Codice, DataTypes.Stringa)
            rpt.AddParameter("Circoscriz", fmCircoscrizione.Descrizione)
        Else
            rpt.AddParameter("Circoscriz", "TUTTE")
        End If

        'filtro sul distretto
        If fmDistretto.Codice <> "" And fmDistretto.Descrizione <> "" Then
            DAM.QB.AddWhereCondition("T_PAZ_PAZIENTI.PAZ_DIS_CODICE", Comparatori.Uguale, fmDistretto.Codice, DataTypes.Stringa)
            rpt.AddParameter("Distretto", fmDistretto.Descrizione)
        Else
            rpt.AddParameter("Distretto", "TUTTI")
        End If

        'filtro sul tipo vaccinazione
        If radVaccinazioni.SelectedItem.Value <> "T" Then
            DAM.QB.AddWhereCondition("VAC_OBBLIGATORIA", Comparatori.Uguale, "A", DataTypes.Stringa)
            rpt.AddParameter("TipoVaccinazione", radVaccinazioni.SelectedItem.Text)
        Else
            rpt.AddParameter("TipoVaccinazione", "TUTTE")
        End If

        'filtro sulle vaccinazioni
        If cmbVaccinazioni.SelectedItem.Value <> "TUTTE" Then
            DAM.QB.AddWhereCondition("VAC_CODICE", Comparatori.Uguale, cmbVaccinazioni.SelectedItem.Value, DataTypes.Stringa)
            rpt.AddParameter("NomeVaccinazione", cmbVaccinazioni.SelectedItem.Text)
        Else
            rpt.AddParameter("NomeVaccinazione", "TUTTE")
        End If

        Dim ds As New DSVaccinazioniRifiutate()

        Try
            DAM.BuildDataTable(ds.VaccinazioniRifiutate)
            rpt.set_dataset(ds)

            Using genericProvider As New DAL.DbGenericProvider(DAM)
                Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                    If Not OnVacReport.StampaReport(Constants.ReportName.VaccinazioniRifiutate, String.Empty, rpt, , , bizReport.GetReportFolder(Constants.ReportName.VaccinazioniRifiutate)) Then
                        OnVacUtility.StampaNonPresente(Page, Constants.ReportName.VaccinazioniRifiutate)
                    End If

                End Using
            End Using

        Finally
            OnVacUtility.CloseDam(DAM)
        End Try

    End Sub

#End Region

End Class
