Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager


Partial Class StatVaccinazioniGiornaliere
    Inherits OnVac.Common.PageBase

#Region " Web Form Designer Generated Code "

    Protected WithEvents lblConsultorio As System.Web.UI.WebControls.Label
    Protected WithEvents uscScegliAmb As OnVac.Common.Controls.SelezioneAmbulatorio

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(sender As System.Object, e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

#Region " Page Events "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            Me.PanelTitolo.InnerText = OnVacUtility.Variabili.CNS.Descrizione

            Me.odpDataEffettuazioneIniz.Text = DateTime.Today
            Me.odpDataEffettuazioneFin.Text = DateTime.Today

            CaricaVaccinazioni()

            Me.chkFittizia.Visible = Me.Settings.GESVACFITTIZIA
            Me.uscScegliAmb.cnsCodice = OnVacUtility.Variabili.CNS.Codice
            Me.uscScegliAmb.cnsDescrizione = OnVacUtility.Variabili.CNS.Descrizione
            Me.uscScegliAmb.ambDescrizione = Constants.AmbulatorioTUTTI.Descrizione
            Me.uscScegliAmb.databind()

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

    Private Sub Toolbar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked

        Select Case e.Button.Key
            Case "btnStampa"
                Stampa()
        End Select

    End Sub

#End Region

#Region " Private "

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()
        listPrintButtons.Add(New PrintButton(Constants.ReportName.VaccinazioniGiornaliere, "btnStampa"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Me.Toolbar)

    End Sub

    Private Sub Stampa()

        ' Controllo campi obbligatori (anche lato client, con alert)
        If Me.odpDataEffettuazioneIniz.Text = String.Empty Or Me.odpDataEffettuazioneFin.Text = String.Empty Then
            Return
        End If

        Dim rpt As New ReportParameter()

        'parametri da passare
        rpt.AddParameter("DataEffettIniz", Me.odpDataEffettuazioneIniz.Text)
        rpt.AddParameter("DataEffettFin", Me.odpDataEffettuazioneFin.Text)
        rpt.AddParameter("DataNascitaIniz", Me.odpDataNascitaIniz.Text)
        rpt.AddParameter("DataNascitaFin", Me.odpDataNascitaFin.Text)

        'utente che esegue la stampa
        Dim user As String = OnVacContext.UserDescription
        If String.IsNullOrWhiteSpace(user) Then user = OnVacContext.UserCode

        rpt.AddParameter("UtenteStampa", user)

        ' Filtro data effettuazione
        Dim reportFilter As New System.Text.StringBuilder()
        reportFilter.AppendFormat("{{T_VAC_ESEGUITE.VES_DATA_EFFETTUAZIONE}} in DateTime ({0}, {1}, {2}, 00, 00, 00) to DateTime ({3}, {4}, {5}, 00, 00, 00)",
                                                Me.odpDataEffettuazioneIniz.Data.Year, Me.odpDataEffettuazioneIniz.Data.Month, Me.odpDataEffettuazioneIniz.Data.Day,
                                                Me.odpDataEffettuazioneFin.Data.Year, Me.odpDataEffettuazioneFin.Data.Month, Me.odpDataEffettuazioneFin.Data.Day)

        ' Filtro consultorio
        If Me.uscScegliAmb.cnsCodice <> String.Empty Then
            If Me.uscScegliAmb.ambCodice <> 0 Then
                reportFilter.AppendFormat(" AND {{T_VAC_ESEGUITE.VES_AMB_CODICE}} = {0}", Me.uscScegliAmb.ambCodice.ToString())
            Else
                reportFilter.AppendFormat(" AND {{T_VAC_ESEGUITE.VES_CNS_CODICE}} = '{0}'", Me.uscScegliAmb.cnsCodice.ToString())
            End If
        End If

        ' Filtro data nascita
        If Me.odpDataNascitaIniz.Text <> String.Empty Then
            reportFilter.AppendFormat(" AND {{T_PAZ_PAZIENTI.PAZ_DATA_NASCITA}} >= Date ({0},{1},{2})",
                                      Me.odpDataNascitaIniz.Data.Year, Me.odpDataNascitaIniz.Data.Month, Me.odpDataNascitaIniz.Data.Day)
        End If

        If Me.odpDataNascitaFin.Text <> String.Empty Then
            reportFilter.AppendFormat(" AND {{T_PAZ_PAZIENTI.PAZ_DATA_NASCITA}} <= Date ({0},{1},{2})",
                                      Me.odpDataNascitaFin.Data.Year, Me.odpDataNascitaFin.Data.Month, Me.odpDataNascitaFin.Data.Day)
        End If

        ' Filtro vaccinazioni
        Dim stbVac As New System.Text.StringBuilder()
        For i As Integer = 0 To Me.chklVaccinazioni.Items.Count - 1
            If Me.chklVaccinazioni.Items(i).Selected Then
                stbVac.AppendFormat("'{0}',", Me.chklVaccinazioni.Items(i).Value)
            End If
        Next
        If stbVac.Length > 0 Then
            stbVac.Remove(stbVac.Length - 1, 1)
            reportFilter.AppendFormat(" AND {{T_ANA_VACCINAZIONI.VAC_CODICE}} IN [{0}]", stbVac.ToString())
        End If

        Dim strStatoVac As String = ""
        If Me.Settings.GESVACFITTIZIA Then
            If Me.chkFittizia.Checked Then
                strStatoVac = ("Incluse fittizie")
            Else
                reportFilter.Append(" AND ({T_VAC_ESEGUITE.VES_FLAG_FITTIZIA} <> 'S')")
            End If
        End If

        rpt.AddParameter("StatoVac", strStatoVac)

        ' Stampa
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If Not OnVacReport.StampaReport(Constants.ReportName.VaccinazioniGiornaliere, reportFilter.ToString(), rpt, Nothing, Nothing, bizReport.GetReportFolder(Constants.ReportName.VaccinazioniGiornaliere)) Then
                    OnVacUtility.StampaNonPresente(Page, Constants.ReportName.VaccinazioniGiornaliere)
                End If

            End Using
        End Using

    End Sub

    Private Sub CaricaVaccinazioni()

        Dim dt As DataTable = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            dt = genericProvider.AnaVaccinazioni.GetDataTableCodiceDescrizioneVaccinazioni("VAC_ORDINE")
        End Using

        Me.chklVaccinazioni.DataValueField = "VAC_CODICE"
        Me.chklVaccinazioni.DataTextField = "VAC_DESCRIZIONE"
        Me.chklVaccinazioni.DataSource = dt
        Me.chklVaccinazioni.DataBind()

    End Sub

#End Region

End Class
