Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities


Partial Class StampaCertificati
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

#Region " Page "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            PanelTitolo.InnerText = OnVacUtility.Variabili.CNS.Descrizione

            ucSelezioneConsultori.MostraSoloAperti = False
            ucSelezioneConsultori.ImpostaCnsCorrente = True
            ucSelezioneConsultori.LoadGetCodici()

            ucSelezioneConsultoriVaccinazioni.MostraSoloAperti = False
            ucSelezioneConsultoriVaccinazioni.ImpostaCnsCorrente = False
            ucSelezioneConsultoriVaccinazioni.LoadGetCodici()

            CaricaVaccinazioni()

            ShowPrintButtons()

        End If

    End Sub

#End Region

#Region " Toolbar "

    Private Sub Toolbar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked

        ' Controllo campi obbligatori
        Dim msg As New Text.StringBuilder()

        If odpDataNascitaIniz.Text = String.Empty OrElse odpDataNascitaFin.Text = String.Empty Then
            msg.AppendLine("L'intervallo di nascita è obbligatorio ").AppendLine()
        End If

        If ucSelezioneConsultori.GetConsultoriSelezionati.Count = 0 AndAlso ucSelezioneConsultoriVaccinazioni.GetConsultoriSelezionati.Count = 0 Then
            msg.AppendLine("Selezionare almeno un centro vaccinale di appartenenza o di esecuzione ").AppendLine()
        End If

        If msg.Length > 0 Then

            msg.Insert(0, "Impossibile generare il report. Sono stati rilevati i seguenti errori:").AppendLine()

            OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(HttpUtility.JavaScriptStringEncode(msg.ToString()), "err", False, False))
            Return

        End If

        Dim rpt As ReportParameter = Nothing

        Dim nomeReport As String = String.Empty

        Select Case e.Button.Key

            Case "btnStampaVaccinale"
                nomeReport = Constants.ReportName.CertificatoVaccinaleMassivo
                rpt = SetParametriCertificatoVaccinale(False)

            Case "btnStampaVaccinaleLotti"
                nomeReport = Constants.ReportName.CertificatoVaccinaleMassivo
                rpt = SetParametriCertificatoVaccinale(True)

                'Case "btnStampaDiscrezionale"
                '    nomeReport = Constants.ReportName.CertificatoDiscrezionale
                '    rpt = SetParametriCertificatoDiscrezionale()

                'Case "btnStampaMantoux"
                '    nomeReport = Constants.ReportName.CertificatoMantoux
                '    rpt = SetParametriCertificatoMantoux()

        End Select

        Stampa(nomeReport, rpt)

    End Sub

#End Region

#Region " Private "

    Private Sub CaricaVaccinazioni()

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            chkVaccinazioni.DataValueField = "VAC_CODICE"
            chkVaccinazioni.DataTextField = "VAC_DESCRIZIONE"
            chkVaccinazioni.DataSource = genericProvider.AnaVaccinazioni.GetVaccinazioni(Nothing)
            chkVaccinazioni.DataBind()

        End Using

    End Sub

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()

        listPrintButtons.Add(New PrintButton(Constants.ReportName.CertificatoVaccinaleMassivo, "btnStampaVaccinale"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.CertificatoVaccinaleMassivo, "btnStampaVaccinaleLotti"))

        ShowToolbarPrintButtons(listPrintButtons, Toolbar)

    End Sub

    ''' <summary>
    ''' Restituisce una struttura con i parametri utilizzati dal report Certificato Vaccinale
    ''' </summary>
    ''' <returns></returns>
    Private Function SetParametriCertificatoVaccinale(stampaLotto As Boolean) As ReportParameter

        Dim cns As Cns = LoadDatiConsultorioCorrente()

        Return OnVacUtility.GetReportParameterCertificatoVaccinale(cns, Settings, True, stampaLotto, False)

    End Function

    ''' <summary>
    ''' Restituisce una struttura con i dati del consultorio corrente
    ''' </summary>
    ''' <returns></returns>
    Private Function LoadDatiConsultorioCorrente() As Cns

        Dim cns As Cns = Nothing

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizCns As New Biz.BizConsultori(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                cns = bizCns.GetConsultorio(OnVacUtility.Variabili.CNS.Codice)

            End Using
        End Using

        Return cns

    End Function

    Private Sub Stampa(nomeReport As String, rpt As ReportParameter)

        If rpt Is Nothing Then rpt = New ReportParameter()

        ' Filtri comuni a tutti e 3 i report
        Dim reportFilter As New Text.StringBuilder()

        ' Data nascita
        reportFilter.AppendFormat("{{T_PAZ_PAZIENTI.PAZ_DATA_NASCITA}} in DateTime ({0}, {1}, {2}, 00, 00, 00) to DateTime ({3}, {4}, {5}, 00, 00, 00) ",
                                  odpDataNascitaIniz.Data.Year, odpDataNascitaIniz.Data.Month, odpDataNascitaIniz.Data.Day,
                                  odpDataNascitaFin.Data.Year, odpDataNascitaFin.Data.Month, odpDataNascitaFin.Data.Day)

        ' Data effettuazione
        If odpDataVacIniz.Data > Date.MinValue Then
            reportFilter.AppendFormat(" AND {{T_VAC_ESEGUITE.VES_DATA_EFFETTUAZIONE}} >= DateTime ({0}, {1}, {2}, 00, 00, 00)",
                                      odpDataVacIniz.Data.Year, odpDataVacIniz.Data.Month, odpDataVacIniz.Data.Day)
        End If

        If odpDataVacFin.Data > Date.MinValue Then
            reportFilter.AppendFormat(" AND {{T_VAC_ESEGUITE.VES_DATA_EFFETTUAZIONE}} <= DateTime ({0}, {1}, {2}, 23, 59, 59) ",
                                      odpDataVacFin.Data.Year, odpDataVacFin.Data.Month, odpDataVacFin.Data.Day)
        End If

        ' Consultorio paziente
        If ucSelezioneConsultori.GetConsultoriSelezionati.Count > 0 Then
            reportFilter.AppendFormat(" AND ({{T_PAZ_PAZIENTI.PAZ_CNS_CODICE}} IN ['{0}']) ", ucSelezioneConsultori.GetConsultoriSelezionati.Aggregate(Function(p, g) p & "', '" & g))
            'Else
            '    Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            '        Using bizCns As New Biz.BizConsultori(genericProvider, Settings, OnVacContext.CreateBizContextInfos, Nothing)

            '            Dim listaCodici As New List(Of String)()

            '            listaCodici = bizCns.GetListCodiceConsultoriUtente(OnVacContext.UserId)

            '            If listaCodici.Count > 0 Then
            '                reportFilter.AppendFormat(" AND ({{T_PAZ_PAZIENTI.PAZ_CNS_CODICE}} IN ['{0}']) ", listaCodici.Aggregate(Function(p, g) p & "', '" & g))
            '            End If

            '        End Using
            '    End Using
        End If

        ' Consultorio effettuazione
        If ucSelezioneConsultoriVaccinazioni.GetConsultoriSelezionati.Count > 0 Then
            reportFilter.AppendFormat(" AND ({{T_VAC_ESEGUITE.VES_CNS_CODICE}} IN ['{0}']) ", ucSelezioneConsultoriVaccinazioni.GetConsultoriSelezionati.Aggregate(Function(p, g) p & "', '" & g))
            'Else
            '    Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            '        Using bizCns As New Biz.BizConsultori(genericProvider, Settings, OnVacContext.CreateBizContextInfos, Nothing)

            '            Dim listaCodici As New List(Of String)()

            '            listaCodici = bizCns.GetListCodiceConsultoriUtente(OnVacContext.UserId)

            '            If listaCodici.Count > 0 Then
            '                reportFilter.AppendFormat(" AND ({{T_VAC_ESEGUITE.VES_CNS_CODICE}} IN ['{0}']) ", listaCodici.Aggregate(Function(p, g) p & "', '" & g))
            '            End If

            '        End Using
            '    End Using
        End If

        ' Filtro numero dosi
        If Not String.IsNullOrWhiteSpace(txtNumeroDosi.Text) Then
            reportFilter.AppendFormat(" AND ({{T_VAC_ESEGUITE.VES_N_RICHIAMO}} = {0}) ", txtNumeroDosi.Text)
        End If

        ' Filtro vaccinazioni
        If chkVaccinazioni.SelectedValues.Count > 0 Then
            reportFilter.AppendFormat(" AND ({{T_VAC_ESEGUITE.VES_VAC_CODICE}} in [{0}])  ", chkVaccinazioni.SelectedValues.ToString(True))
        End If

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If Not OnVacReport.StampaReport(nomeReport, reportFilter.ToString(), rpt, Nothing, Nothing, bizReport.GetReportFolder(nomeReport)) Then
                    OnVacUtility.StampaNonPresente(Page, nomeReport)
                End If

            End Using
        End Using

    End Sub

#Region " Vecchia versione con certificati discrezionale e mantoux "

    ' N.B. : per essere stampati massivamente andrebbero modificati, per colpa dei filtri che sono stati aggiunti

    'Private Function SetParametriCertificatoDiscrezionale() As ReportParameter

    '    Dim rpt As New ReportParameter()

    '    rpt.AddParameter("DataNascita1", odpDataNascitaIniz.Text)
    '    rpt.AddParameter("DataNascita2", odpDataNascitaFin.Text)

    '    Dim cns As Cns = LoadDatiConsultorioCorrente()

    '    If cns Is Nothing Then
    '        rpt.AddParameter("DescrizioneComune", String.Empty)
    '    Else
    '        rpt.AddParameter("DescrizioneComune", cns.Descrizione)
    '    End If

    '    rpt.AddParameter("cnsStampaCodice", OnVacUtility.Variabili.CNS.Codice)
    '    rpt.AddParameter("notaValidita", Me.Settings.CERTIFICATO_VACCINALE_NOTA_VALIDITA)

    '    Return rpt

    'End Function

    'Private Function SetParametriCertificatoMantoux() As ReportParameter

    '    Dim rpt As New ReportParameter()

    '    rpt.AddParameter("soloMantoux", "N")
    '    rpt.AddParameter("headerDataInvio", Me.GetOnVacResourceValue(Constants.StringResourcesKey.DatiPaziente_MantouxDataInvio))
    '    rpt.AddParameter("cnsStampaCodice", OnVacUtility.Variabili.CNS.Codice)
    '    rpt.AddParameter("notaValidita", Me.Settings.CERTIFICATO_VACCINALE_NOTA_VALIDITA)

    '    Return rpt

    'End Function

#End Region

#End Region

End Class
