Imports System.Collections.Generic

Partial Class VaccinazioniEseguiteCampagna
    Inherits OnVac.Common.PageBase


    Protected WithEvents lblModStampa As System.Web.UI.WebControls.Label
    Protected WithEvents lblDataNascita1 As System.Web.UI.WebControls.Label
    Protected WithEvents CheckBoxList1 As System.Web.UI.WebControls.CheckBoxList


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
            PanelTitolo.InnerText = OnVacUtility.Variabili.CNS.Descrizione

            odpDataEffettuazioneIniz.Text = DateTime.Today
            odpDataEffettuazioneFin.Text = DateTime.Today

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

        listPrintButtons.Add(New PrintButton(Constants.ReportName.VaccinazioniEseguiteCampagna, "btnStampa"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Toolbar)
    End Sub


    Private Sub Toolbar_ButtonClicked(ByVal sender As System.Object, ByVal e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked
        Select Case e.Button.Key
            Case "btnStampa"
                Stampa()
        End Select
    End Sub


    Private Sub Stampa()

        ' Controllo campi obbligatori (anche lato client, con alert)
        If odpDataEffettuazioneIniz.Text = String.Empty Or odpDataEffettuazioneFin.Text = String.Empty Then
            Return
        End If

        Dim strFiltro As String = String.Empty

        Dim rpt As New ReportParameter()

        rpt.AddParameter("PeriodoVac1", odpDataEffettuazioneIniz.Text)
        rpt.AddParameter("PeriodoVac2", odpDataEffettuazioneFin.Text)
        rpt.AddParameter("DataNascita1", odpDataNascitaIniz.Text)
        rpt.AddParameter("DataNascita2", odpDataNascitaFin.Text)
        rpt.AddParameter("Data", odpData.Text)

        ' Filtro data effettuazione
        If odpDataEffettuazioneIniz.Text <> String.Empty And odpDataEffettuazioneFin.Text <> String.Empty Then
            strFiltro = String.Format("{{T_VAC_ESEGUITE.VES_DATA_EFFETTUAZIONE}} in DateTime ({0}, {1}, {2}, 00, 00, 00) ", _
                                      odpDataEffettuazioneIniz.Data.Year, odpDataEffettuazioneIniz.Data.Month, odpDataEffettuazioneIniz.Data.Day)
            strFiltro += String.Format(" to DateTime ({0}, {1}, {2}, 00, 00, 00) AND ", _
                                       odpDataEffettuazioneFin.Data.Year, odpDataEffettuazioneFin.Data.Month, odpDataEffettuazioneFin.Data.Day)
        End If
        strFiltro += " {T_VAC_ESEGUITE.VES_IN_CAMPAGNA} = 'S' "

        ' Filtro data convocazione
        If odpData.Text <> "" Then
            strFiltro += String.Format(" AND {{T_VAC_ESEGUITE.VES_CNV_DATA}} = DateTime ({0}, {1}, {2}) ", _
                           odpData.Data.Year, odpData.Data.Month, odpData.Data.Day)
        End If

        ' Filtro intervallo data di nascita (inizio)
        If odpDataNascitaIniz.Text <> String.Empty Then
            strFiltro += String.Format(" AND {{T_PAZ_PAZIENTI.PAZ_DATA_NASCITA}} >= DateTime ({0}, {1}, {2}, 00, 00, 00) ", _
                                       odpDataNascitaIniz.Data.Year, odpDataNascitaIniz.Data.Month, odpDataNascitaIniz.Data.Day)
        End If

        ' Filtro intervallo data di nascita (fine)
        If odpDataNascitaFin.Text <> String.Empty Then
            strFiltro += String.Format(" AND {{T_PAZ_PAZIENTI.PAZ_DATA_NASCITA}} <= DateTime ({0}, {1}, {2}, 00, 00, 00)", _
                                       odpDataNascitaFin.Data.Year, odpDataNascitaFin.Data.Month, odpDataNascitaFin.Data.Day)
        End If

        If (chklModVaccinazione.Items(0).Selected = False And chklModVaccinazione.Items(1).Selected = False And chklModVaccinazione.Items(2).Selected = False) Then
            rpt.AddParameter("TipoVac", "OBBLIGATORIE RACCOMANDATE E FACOLTATIVE")
        ElseIf (chklModVaccinazione.Items(0).Selected = True And chklModVaccinazione.Items(1).Selected = False And chklModVaccinazione.Items(2).Selected = False) Then
            strFiltro &= " AND {T_ANA_VACCINAZIONI.VAC_OBBLIGATORIA} = 'A'"
            rpt.AddParameter("TipoVac", "OBBLIGATORIE")
        ElseIf (chklModVaccinazione.Items(0).Selected = False And chklModVaccinazione.Items(1).Selected = True And chklModVaccinazione.Items(2).Selected = False) Then
            strFiltro &= " AND {T_ANA_VACCINAZIONI.VAC_OBBLIGATORIA} = 'B'"
            rpt.AddParameter("TipoVac", "RACCOMANDATE")
        ElseIf (chklModVaccinazione.Items(0).Selected = True And chklModVaccinazione.Items(1).Selected = False And chklModVaccinazione.Items(2).Selected = True) Then
            strFiltro &= " AND ({T_ANA_VACCINAZIONI.VAC_OBBLIGATORIA} = 'A'"
            strFiltro &= " OR {T_ANA_VACCINAZIONI.VAC_OBBLIGATORIA} = 'C')"
            rpt.AddParameter("TipoVac", "OBBLIGATORIE E FACOLTATICE")
        ElseIf (chklModVaccinazione.Items(0).Selected = False And chklModVaccinazione.Items(1).Selected = True And chklModVaccinazione.Items(2).Selected = True) Then
            strFiltro &= " AND ({T_ANA_VACCINAZIONI.VAC_OBBLIGATORIA} = 'B'"
            strFiltro &= " OR {T_ANA_VACCINAZIONI.VAC_OBBLIGATORIA} = 'C')"
            rpt.AddParameter("TipoVac", "RACCOMANDATE E FACOLTATIVE")
        ElseIf (chklModVaccinazione.Items(0).Selected = False And chklModVaccinazione.Items(1).Selected = False And chklModVaccinazione.Items(2).Selected = True) Then
            strFiltro &= " AND {T_ANA_VACCINAZIONI.VAC_OBBLIGATORIA} = 'C'"
            rpt.AddParameter("TipoVac", "FACOLTATIVE")
        ElseIf (chklModVaccinazione.Items(0).Selected = True And chklModVaccinazione.Items(1).Selected = True And chklModVaccinazione.Items(2).Selected = False) Then
            strFiltro &= " AND ({T_ANA_VACCINAZIONI.VAC_OBBLIGATORIA} = 'A'"
            strFiltro &= " OR {T_ANA_VACCINAZIONI.VAC_OBBLIGATORIA} = 'B')"
            rpt.AddParameter("TipoVac", "OBBLIGATORIE E RACCOMANDATE")
        Else
            rpt.AddParameter("TipoVac", "OBBLIGATORIE RACCOMANDATE E FACOLTATIVE")
        End If


        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If Not OnVacReport.StampaReport(Constants.ReportName.VaccinazioniEseguiteCampagna, strFiltro, rpt, , , bizReport.GetReportFolder(Constants.ReportName.VaccinazioniEseguiteCampagna)) Then
                    OnVacUtility.StampaNonPresente(Page, Constants.ReportName.VaccinazioniEseguiteCampagna)
                End If

            End Using
        End Using

    End Sub


End Class
