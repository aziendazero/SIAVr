Imports Onit.Database.DataAccessManager
Imports System.Collections.Generic

Partial Class AvvisoInBianco
    Inherits OnVac.Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    Protected WithEvents lblModStampa As System.Web.UI.WebControls.Label
    Protected WithEvents lblEta As System.Web.UI.WebControls.Label
    Protected WithEvents Label1 As System.Web.UI.WebControls.Label

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(sender As System.Object, e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Page Event "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            Me.PanelTitolo.InnerText = OnVacUtility.Variabili.CNS.Descrizione

            ' Gestione della visualizzazione dei pulsanti di stampa in base all'installazione
            ShowPrintButtons()

        End If

        'lancio della stampa tramite tasto invio (modifica 29/07/2004)
        If Me.Toolbar.Items.FromKeyButton("btnStampa").Visible Then
            Select Case Request.Form("__EVENTTARGET")
                Case "Stampa"
                    Stampa()
            End Select
        End If

    End Sub

#End Region

#Region " Toolbar Event "

    Private Sub Toolbar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked
        Select Case e.Button.Key
            Case "btnStampa"
                Stampa()
        End Select
    End Sub

#End Region

#Region " Private "

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()
        listPrintButtons.Add(New PrintButton(Constants.ReportName.AvvisoBianco, "btnStampa"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Toolbar)

    End Sub

    Private Sub Stampa()

        Dim strFiltro As String = String.Empty
        Dim rpt As New ReportParameter()

        'impostazione intestazioni da DB

        Select Case rdlSollecito.SelectedIndex
            Case 1
                rpt.AddParameter("Sollecito", "Primo")
            Case 2
                rpt.AddParameter("Sollecito", "Secondo")
            Case 3
                rpt.AddParameter("Sollecito", "Terzo")
            Case 4
                rpt.AddParameter("Sollecito", "Quarto")
            Case 0
                rpt.AddParameter("Sollecito", "Nessuno")
        End Select

        Select Case rdlEta.SelectedIndex
            Case 1
                rpt.AddParameter("Eta", "Maggiorenne")
            Case 0
                rpt.AddParameter("Eta", "Minorenne")
        End Select

        rpt.AddParameter("CnsStampa", GetCnsStampa())

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If Not OnVacReport.StampaReport(Constants.ReportName.AvvisoBianco, strFiltro, rpt, Nothing, Nothing, bizReport.GetReportFolder(Constants.ReportName.AvvisoBianco)) Then
                    OnVacUtility.StampaNonPresente(Page, Constants.ReportName.AvvisoBianco)
                End If

            End Using
        End Using

    End Sub

    ' carica il CNS_STAMPA associato al consultorio
    Private Function GetCnsStampa() As String

        Dim campoStampa1 As String = String.Empty

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizConsultori As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                campoStampa1 = bizConsultori.GetCampoStampa1(OnVacUtility.Variabili.CNS.Codice)

            End Using
        End Using

        Return campoStampa1

    End Function

#End Region

End Class
