Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Biz
Imports System.Collections.Generic

Partial Class ElencoBilanciMalattia
    Inherits OnVac.Common.PageBase


#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

    Protected WithEvents uscRicBil As OnVac.RicercaBilancio
    Protected WithEvents lblConsultorio As System.Web.UI.WebControls.Label
    Protected WithEvents fmDistretto As Onit.Controls.OnitModalList

#End Region


    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Pulizia e log della session
        Dim sc As New SessionCleaner(Me.Settings)
        sc.Start()

        If Not IsPostBack Then
            fmConsultorio.Codice = OnVacUtility.Variabili.CNS.Codice
            fmConsultorio.RefreshDataBind()

            ShowPrintButtons()
        End If
    End Sub


    Private Sub ShowPrintButtons()
        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()

        listPrintButtons.Add(New PrintButton(Constants.ReportName.StampaBilanciMalattia, "btnStampa"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Toolbar)
    End Sub


    Private Sub Toolbar_ButtonClicked(ByVal sender As System.Object, ByVal e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked
        Dim strFiltro As String

        Select Case (e.Button.Key)

            Case "btnBilancio"

                uscRicBil.ModaleName = "modRicBil"
                uscRicBil.LoadModale()
                modRicBil.VisibileMD = True
                CType(Toolbar.Items.FromKey("btnBilancio"), Infragistics.WebUI.UltraWebToolbar.TBarButton).Enabled = False
                uscRicBil_AnnullaBilancio()

            Case "btnStampa"

                Dim rpt As New ReportParameter()

                strFiltro = "1=1"

                If (odpDataNascitaIniz.Text <> "" And odpDataNascitaFin.Text <> "") Then
                    Dim strDataNascitaIniz(3) As String
                    strDataNascitaIniz = odpDataNascitaIniz.Text.Split("/")
                    Dim strDataNascitaFin(3) As String
                    strDataNascitaFin = odpDataNascitaFin.Text.Split("/")

                    strFiltro &= "AND "
                    strFiltro &= "(DateDiff('d',{V_BILANCI_MALATTIA_PROG.PAZ_DATA_NASCITA},DateSerial(" & strDataNascitaIniz(2) & ", " & strDataNascitaIniz(1) & ", " & strDataNascitaIniz(0) & "))<=0"
                    strFiltro &= " AND DateDiff('d',DateSerial(" & strDataNascitaFin(2) & ", " & strDataNascitaFin(1) & ", " & strDataNascitaFin(0) & "),{V_BILANCI_MALATTIA_PROG.PAZ_DATA_NASCITA})<=0)"

                    rpt.AddParameter("DataNascitaIniz", odpDataNascitaIniz.Text)
                    rpt.AddParameter("DataNascitaFin", odpDataNascitaFin.Text)
                Else
                    rpt.AddParameter("DataNascitaIniz", "")
                    rpt.AddParameter("DataNascitaFin", "")
                End If


                If (odpDataEffettuazioneIniz.Text <> "" And odpDataEffettuazioneFin.Text <> "") Then
                    Dim strDataEffettuazioneIniz(3) As String
                    strDataEffettuazioneIniz = odpDataEffettuazioneIniz.Text.Split("/")

                    Dim strDataEffettuazioneFin(3) As String
                    strDataEffettuazioneFin = odpDataEffettuazioneFin.Text.Split("/")

                    strFiltro &= "AND "
                    strFiltro &= "(DateDiff('d',{V_BILANCI_MALATTIA_PROG.DATA_EFFETTUAZIONE},DateSerial(" & strDataEffettuazioneIniz(2) & ", " & strDataEffettuazioneIniz(1) & ", " & strDataEffettuazioneIniz(0) & "))<=0"
                    strFiltro &= " AND DateDiff('d',DateSerial(" & strDataEffettuazioneFin(2) & ", " & strDataEffettuazioneFin(1) & ", " & strDataEffettuazioneFin(0) & "),{V_BILANCI_MALATTIA_PROG.DATA_EFFETTUAZIONE})<=0)"
                    rpt.AddParameter("DataEffettIniz", odpDataEffettuazioneIniz.Text)
                    rpt.AddParameter("DataEffettFin", odpDataEffettuazioneFin.Text)
                Else
                    rpt.AddParameter("DataEffettIniz", "")
                    rpt.AddParameter("DataEffettFin", "")
                End If

                If Not String.IsNullOrEmpty(txtNumero.Text) Then
                    strFiltro &= " AND {V_BILANCI_MALATTIA_PROG.NUM_BIL}=" & txtNumero.Text
                    rpt.AddParameter("Numero", txtNumero.Text)
                Else
                    rpt.AddParameter("Numero", "TUTTI")
                End If

                If txtMalattia.Codice <> "" And txtMalattia.Descrizione <> "" Then
                    strFiltro &= " AND {V_BILANCI_MALATTIA_PROG.MALATTIA_CODICE}='" & txtMalattia.Codice & "' "
                    rpt.AddParameter("Malattia", txtMalattia.Descrizione)
                Else
                    rpt.AddParameter("Malattia", "TUTTE")
                End If

                If fmConsultorio.Codice <> "" And fmConsultorio.Descrizione <> "" Then
                    strFiltro &= " AND {V_BILANCI_MALATTIA_PROG.CONS_CODICE}='" & fmConsultorio.Codice & "' "
                    rpt.AddParameter("Consultorio", fmConsultorio.Descrizione)
                Else
                    rpt.AddParameter("Consultorio", "TUTTI")
                End If

                If fmComuneRes.Codice <> "" And fmComuneRes.Descrizione <> "" Then
                    strFiltro &= " AND {V_BILANCI_MALATTIA_PROG.RES_COM_CODICE} ='" & fmComuneRes.Codice & "'"
                    rpt.AddParameter("ComRes", fmComuneRes.Descrizione)
                Else
                    rpt.AddParameter("ComRes", "TUTTI")
                End If

                ' Provider per accesso al db (utilizzato per accedere alla t_ana_report)
                Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                    Using bizReport As New BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                        If Not OnVacReport.StampaReport(Constants.ReportName.StampaBilanciMalattia, strFiltro, rpt, , , bizReport.GetReportFolder(Constants.ReportName.StampaBilanciMalattia)) Then
                            OnVacUtility.StampaNonPresente(Page, Constants.ReportName.StampaBilanciMalattia)
                        End If

                    End Using
                End Using

        End Select
    End Sub

#Region " User Controls "

    Private Sub uscRicBil_AnnullaBilancio() Handles uscRicBil.AnnullaBilancio

        Me.Toolbar.Items.FromKeyButton("btnBilancio").Enabled = True

    End Sub

    Private Sub uscRicBil_Pulisci() Handles uscRicBil.Pulisci

        Me.txtNumero.Text = String.Empty
        Me.txtMalattia.Descrizione = String.Empty
        Me.txtMalattia.Codice = String.Empty

    End Sub

    Private Sub uscRicBil_ReturnBilancio(bilancioAnagrafica As Entities.BilancioAnagrafica) Handles uscRicBil.ReturnBilancio

        Me.txtMalattia.Codice = bilancioAnagrafica.CodiceMalattia
        Me.txtMalattia.Descrizione = bilancioAnagrafica.DescrizioneMalattia
        Me.txtNumero.Text = bilancioAnagrafica.NumeroBilancio

    End Sub

#End Region

End Class
