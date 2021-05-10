Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager

Partial Class StatVaccinazioniProgrammate
    Inherits OnVac.Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub
    Protected WithEvents lblConsultorio As System.Web.UI.WebControls.Label

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

			ucSelezioneConsultori.MostraSoloAperti = False
			ucSelezioneConsultori.ImpostaCnsCorrente = True
			ucSelezioneConsultori.LoadGetCodici()

			Me.CaricamentoDati()

            Me.ShowPrintButtons()

        End If

        'generazione della stampa tramite tasto invio
        Select Case Request.Form("__EVENTTARGET")
            Case "Stampa"
                If Me.Toolbar.Items.FromKeyButton("btnStampa").Visible Then
                    Stampa()
                End If
        End Select

    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub Toolbar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked

        Select Case e.Button.Key
            Case "btnStampa"
                Me.Stampa()
            Case "btnStampaAssociazioni"
                Me.StampaAssociazione()
        End Select

    End Sub

#End Region

#Region " Private "

    Private Sub CaricamentoDati()

        Dim dtStatiAnag As DataTable = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Me.chklStatoAnagrafico.DataValueField = "SAN_CODICE"
            Me.chklStatoAnagrafico.DataTextField = "SAN_DESCRIZIONE"

            Using bizStatiAnagrafici As New Biz.BizStatiAnagrafici(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())
                Me.chklStatoAnagrafico.DataSource = bizStatiAnagrafici.LeggiStatiAnagrafici()
            End Using

            Me.chklStatoAnagrafico.DataBind()

            Me.chklVaccinazioni.DataValueField = "VAC_CODICE"
            Me.chklVaccinazioni.DataTextField = "VAC_DESCRIZIONE"

            Using bizVaccinazioniProgrammate As New Biz.BizVaccinazioneProg(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())
                Me.chklVaccinazioni.DataSource = bizVaccinazioniProgrammate.GetDataTableCodiceDescrizioneVaccinazioni()
            End Using

            Me.chklVaccinazioni.DataBind()

        End Using

    End Sub

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()
        listPrintButtons.Add(New PrintButton(Constants.ReportName.VaccinazioniProgrammate, "btnStampa"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.VaccinazioniProgrammateAssociazioni, "btnStampaAssociazioni"))
        Me.ShowToolbarPrintButtons(listPrintButtons, Toolbar)

    End Sub

    Private Sub Stampa()

        Dim command As New Biz.BizVaccinazioneProg.ReportVaccinazioniProgrammateCommand()

        ' Date Appuntamento
        command.DataAppuntamentoInizio = odpDataAppuntamentoIniz.Data
        command.DataAppuntamentoFine = odpDataAppuntamentoFin.Data

        ' Consultorio
        Dim lista As List(Of String) = ucSelezioneConsultori.GetConsultoriSelezionati()

        Dim descrizione As String = ucSelezioneConsultori.GetDescrizioneConsultoriSelezionatiPerStampe()

        If lista.Count = 0 Then

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizCns As New Biz.BizConsultori(genericProvider, Settings, OnVacContext.CreateBizContextInfos, Nothing)

                    lista = bizCns.GetListCodiceConsultoriUtente(OnVacContext.UserId)
                    descrizione = "TUTTI"

                End Using
            End Using

        End If

        command.ListaConsultorioCodice = lista
		command.ConsultorioDescrizione = descrizione

        ' Medico di base
        command.MedicoCodice = fmMedico.Codice
        command.MedicoDescrizione = fmMedico.Descrizione

        ' Data di Nascita
        command.DataNascitaInizio = odpDataNascitaIniz.Data
        command.DataNascitaFine = odpDataNascitaFin.Data

        ' Stato anagrafico
        If Not chklStatoAnagrafico.SelectedItem Is Nothing AndAlso chklStatoAnagrafico.SelectedItems.Count > 0 Then

            command.ListaDescrizioniStatiAnagraficiSelezionati = (From item As ListItem In chklStatoAnagrafico.SelectedItems
                                                                  Select item.Text).ToList()

            command.ListaCodiciStatiAnagraficiSelezionati = (From item As ListItem In chklStatoAnagrafico.SelectedItems
                                                             Select item.Value).ToList()

        End If

        ' Dose
        If Not String.IsNullOrEmpty(txtNumeroDose.Text) Then
            command.NumeroDose = txtNumeroDose.Text.Trim()
        End If

        ' Vaccinazioni
        If Not chklVaccinazioni.SelectedItems Is Nothing AndAlso chklVaccinazioni.SelectedItems.Count > 0 Then

            command.ListaCodiciVaccinazioniSelezionate = (From codice As String In chklVaccinazioni.SelectedValues
                                                          Select codice).ToList()

        End If

        ' Creazione report
        Dim result As Biz.BizVaccinazioneProg.ReportVaccinazioniProgrammateResult = Biz.BizVaccinazioneProg.GetReportVaccinazioniProgrammate(command)

        If Not result.Success Then Return

        Dim rpt As New ReportParameter()

        If Not result.ParametriReport Is Nothing AndAlso result.ParametriReport.Count > 0 Then

            For Each parametro As KeyValuePair(Of String, String) In result.ParametriReport
                rpt.AddParameter(parametro.Key, parametro.Value)
            Next

        End If

        ' Stampa
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If Not OnVacReport.StampaReport(Constants.ReportName.VaccinazioniProgrammate, result.FiltroReport, rpt, Nothing, Nothing, bizReport.GetReportFolder(Constants.ReportName.VaccinazioniProgrammate)) Then
                    OnVacUtility.StampaNonPresente(Page, Constants.ReportName.VaccinazioniProgrammate)
                End If

            End Using
        End Using

    End Sub

    Private Sub StampaAssociazione()

        Dim command As New Biz.BizVaccinazioneProg.ReportVaccinazioniProgrammateCommand()

        ' Date Appuntamento
        command.DataAppuntamentoInizio = Me.odpDataAppuntamentoIniz.Data
        command.DataAppuntamentoFine = Me.odpDataAppuntamentoFin.Data

        ' Consultorio
        Dim lista As List(Of String) = ucSelezioneConsultori.GetConsultoriSelezionati()
		Dim descrizione As String = ucSelezioneConsultori.GetDescrizioneConsultoriSelezionatiPerStampe()
		If lista.Count = 0 Then
			Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
				Using bizCns As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)
					lista = bizCns.GetListCodiceConsultoriUtente(OnVacContext.UserId)
					descrizione = "TUTTI"
				End Using
			End Using
		End If
		command.ListaConsultorioCodice = lista
		command.ConsultorioDescrizione = descrizione

		' Medico di base
		command.MedicoCodice = Me.fmMedico.Codice
        command.MedicoDescrizione = Me.fmMedico.Descrizione

        ' Data di Nascita
        command.DataNascitaInizio = Me.odpDataNascitaIniz.Data
        command.DataNascitaFine = Me.odpDataNascitaFin.Data

        ' Stato anagrafico
        If Not Me.chklStatoAnagrafico.SelectedItem Is Nothing AndAlso Me.chklStatoAnagrafico.SelectedItems.Count > 0 Then

            command.ListaDescrizioniStatiAnagraficiSelezionati = (From item As ListItem In Me.chklStatoAnagrafico.SelectedItems
                                                                  Select item.Text).ToList()

            command.ListaCodiciStatiAnagraficiSelezionati = (From item As ListItem In Me.chklStatoAnagrafico.SelectedItems
                                                             Select item.Value).ToList()

        End If

        ' Dose
        If Not String.IsNullOrEmpty(Me.txtNumeroDose.Text) Then
            command.NumeroDose = Me.txtNumeroDose.Text.Trim()
        End If

        ' Vaccinazioni
        If Not Me.chklVaccinazioni.SelectedItems Is Nothing AndAlso Me.chklVaccinazioni.SelectedItems.Count > 0 Then

            command.ListaCodiciVaccinazioniSelezionate = (From codice As String In Me.chklVaccinazioni.SelectedValues
                                                          Select codice).ToList()

        End If


        Dim contextInfos As Biz.BizContextInfos = OnVacContext.CreateBizContextInfos()
        Dim result As Biz.BizVaccinazioneProg.ReportVaccinazioniAssociazioniProgrammateResult
        ' Creazione report
        Using dam As IDAM = OnVacUtility.OpenDam()
            Using genericProvider As New DAL.DbGenericProvider(dam)
                Using bizVaccinazioneProg As New Biz.BizVaccinazioneProg(genericProvider, Me.Settings, contextInfos, Nothing)
                    result = bizVaccinazioneProg.GetReportVaccinazioniProgrammateAssociazioni(command)
                End Using
            End Using
        End Using


        If Not result.Success Then Return

        Dim rpt As New ReportParameter()

        If Not result.ParametriReport Is Nothing AndAlso result.ParametriReport.Count > 0 Then

            For Each parametro As KeyValuePair(Of String, String) In result.ParametriReport
                rpt.AddParameter(parametro.Key, parametro.Value)
            Next

        End If

        rpt.set_dataset(result.DstStatVacProgAss)

        ' Stampa
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If Not OnVacReport.StampaReport(Constants.ReportName.VaccinazioniProgrammateAssociazioni, String.Empty, rpt, Nothing, Nothing, bizReport.GetReportFolder(Constants.ReportName.VaccinazioniProgrammateAssociazioni)) Then
                    OnVacUtility.StampaNonPresente(Page, Constants.ReportName.VaccinazioniProgrammateAssociazioni)
                End If

            End Using
        End Using

    End Sub

#End Region

End Class
