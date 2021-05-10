Imports System.Collections.Generic

Partial Class StatElencoAssistiti
    Inherits Common.PageBase

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

#Region " Eventi page "

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            PanelTitolo.InnerText = OnVacUtility.Variabili.CNS.Descrizione

            ucSelezioneConsultori.MostraSoloAperti = False
            ucSelezioneConsultori.ImpostaCnsCorrente = True
            ucSelezioneConsultori.LoadGetCodici()

            ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
            fldStatoAcquisizione.Visible = FlagConsensoVaccUslCorrente

            CaricamentoDati()

            ShowPrintButtons()

        End If

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

#Region " Eventi Toolbar "

    Private Sub Toolbar_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked

        Select Case be.Button.Key

            Case "btnStampa"
                Me.Stampa(Constants.ReportName.ElencoAssistiti)

            Case "btnStampaEtichette"
                Me.Stampa(Constants.ReportName.EtichetteAssistiti)

            Case "btnStampaEtichetteSpedizione"
                Me.Stampa(Constants.ReportName.EtichetteSpedizioneAssistiti)

            Case "btnStampaDocumentazione"
                Me.Stampa(Constants.ReportName.DocumentazioneAssistiti)

        End Select

    End Sub

#End Region

#Region " Private "

#Region " Caricamento dati "

    Private Sub CaricamentoDati()

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Me.CaricaMalattie(genericProvider)
            Me.CaricaStatiAnagrafici(genericProvider)
            Me.CaricaDdlCategoriaRischio(genericProvider)

        End Using

    End Sub

    Private Sub CaricaMalattie(genericProvider As DAL.DbGenericProvider)

        Dim dt As DataTable = Nothing

        Using bizMalattie As New Biz.BizMalattie(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

            dt = bizMalattie.GetDtCodiceDescrizioneMalattie(False, True)

        End Using

        Me.cmbMalCronica.DataValueField = "MAL_CODICE"
        Me.cmbMalCronica.DataTextField = "MAL_DESCRIZIONE"
        Me.cmbMalCronica.DataSource = dt
        Me.cmbMalCronica.DataBind()

    End Sub

    Private Sub CaricaStatiAnagrafici(genericProvider As DAL.DbGenericProvider)

        Dim dtStatiAnag As DataTable = Nothing

        Using bizStatiAnagrafici As New Biz.BizStatiAnagrafici(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())
            dtStatiAnag = bizStatiAnagrafici.LeggiStatiAnagrafici()
        End Using

        Me.chklStatoAnagrafico.DataValueField = "SAN_CODICE"
        Me.chklStatoAnagrafico.DataTextField = "SAN_DESCRIZIONE"
        Me.chklStatoAnagrafico.DataSource = dtStatiAnag
        Me.chklStatoAnagrafico.DataBind()

    End Sub

    Private Sub CaricaDdlCategoriaRischio(genericProvider As DAL.DbGenericProvider)

        Dim dtCategorieRischio As DataTable = Nothing

        Using bizCategorieRischio As New Biz.BizCategorieRischio(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

            dtCategorieRischio = bizCategorieRischio.LoadDataTableCategorieRischio(True, True)

        End Using

        Me.cmbCatRischio.DataValueField = "RSC_CODICE"
        Me.cmbCatRischio.DataTextField = "RSC_DESCRIZIONE"
        Me.cmbCatRischio.DataSource = dtCategorieRischio
        Me.cmbCatRischio.DataBind()

    End Sub

#End Region

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()

        listPrintButtons.Add(New PrintButton(Constants.ReportName.ElencoAssistiti, "btnStampa"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.EtichetteAssistiti, "btnStampaEtichette"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.EtichetteSpedizioneAssistiti, "btnStampaEtichetteSpedizione"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.DocumentazioneAssistiti, "btnStampaDocumentazione"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Toolbar)

    End Sub

    Private Sub Stampa(nomeReport As String)

        ' Controllo valorizzazione filtri sulla data di nascita
        If (Me.odpDataNascitaIniz.Data <> DateTime.MinValue And Me.odpDataNascitaFin.Data = DateTime.MinValue) OrElse
           (Me.odpDataNascitaIniz.Data = DateTime.MinValue And Me.odpDataNascitaFin.Data <> DateTime.MinValue) Then

            Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("I campi 'Data Nascita' non sono impostati correttamente. Impossibile stampare il report!!", "alertDateNascita", False, False))
            Return

        End If

        Dim rpt As New ReportParameter()

        Dim filtroReport As New System.Text.StringBuilder("1=1")

        ' Regolarizzazione
        If Me.ddlRegolarizzazione.SelectedValue <> "" Then

            If Me.ddlRegolarizzazione.SelectedValue = "S" Then
                filtroReport.Append(" AND {T_PAZ_PAZIENTI.PAZ_REGOLARIZZATO} = 'S'")
            Else
                filtroReport.Append(" AND (isnull({T_PAZ_PAZIENTI.PAZ_REGOLARIZZATO}) OR {T_PAZ_PAZIENTI.PAZ_REGOLARIZZATO} = 'N')")
            End If

            rpt.AddParameter("Regolarizzazione", Me.ddlRegolarizzazione.SelectedItem.Text)

        Else
            rpt.AddParameter("Regolarizzazione", "")
        End If

		' Consultorio
		Dim lista As List(Of String) = ucSelezioneConsultori.GetConsultoriSelezionati
		If lista.Count > 0 Then

			filtroReport.AppendFormat(" AND ({{T_PAZ_PAZIENTI.PAZ_CNS_CODICE}} IN ['{0}'])", lista.Aggregate(Function(p, g) p & "', '" & g))

			rpt.AddParameter("Consultorio", ucSelezioneConsultori.GetDescrizioneConsultoriSelezionatiPerStampe())

		Else
			Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
				Using bizCns As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)
					lista = bizCns.GetListCodiceConsultoriUtente(OnVacContext.UserId)
					If lista.Count > 0 Then
						filtroReport.AppendFormat(" AND ({{T_PAZ_PAZIENTI.PAZ_CNS_CODICE}} IN ['{0}'])", lista.Aggregate(Function(p, g) p & "', '" & g))
					End If

				End Using
			End Using
			rpt.AddParameter("Consultorio", "TUTTI")
        End If

        ' Residenza
        If Me.fmComuneRes.Codice <> String.Empty And Me.fmComuneRes.Descrizione <> String.Empty Then

            filtroReport.AppendFormat(" AND {{T_PAZ_PAZIENTI.PAZ_COM_CODICE_RESIDENZA}} = '{0}'", Me.fmComuneRes.Codice)

            rpt.AddParameter("Comune", Me.fmComuneRes.Descrizione)

        Else
            rpt.AddParameter("Comune", "TUTTI")
        End If

        ' Circoscrizione
        If Me.fmCircoscrizione.Codice <> "" And Me.fmCircoscrizione.Descrizione <> "" Then

            filtroReport.AppendFormat(" AND {{T_PAZ_PAZIENTI.PAZ_CIR_CODICE}} = '{0}'", Me.fmCircoscrizione.Codice)

            rpt.AddParameter("Circoscrizione", Me.fmCircoscrizione.Descrizione)

        Else
            rpt.AddParameter("Circoscrizione", "TUTTE")
        End If

        ' Date di nascita
        If Me.odpDataNascitaIniz.Text <> "" And Me.odpDataNascitaFin.Text <> "" Then

            filtroReport.AppendFormat(" AND {{T_PAZ_PAZIENTI.PAZ_DATA_NASCITA}} {0}",
                                      Biz.BizReport.GetBetweenDateReportFilter(Me.odpDataNascitaIniz.Data, Me.odpDataNascitaFin.Data))

            rpt.AddParameter("DataNascitaIniz", Me.odpDataNascitaIniz.Text)
            rpt.AddParameter("DataNascitaFin", Me.odpDataNascitaFin.Text)

        Else
            rpt.AddParameter("DataNascitaIniz", "")
            rpt.AddParameter("DataNascitaFin", "")
        End If

        ' Stato anagrafico
        If Not chklStatoAnagrafico.SelectedItem Is Nothing Then

            filtroReport.Append(" AND (")

            Dim statiCheck As New ArrayList()
            Dim parStatoAnagrafico As New Text.StringBuilder()

            For count As Integer = 0 To chklStatoAnagrafico.Items.Count - 1

                If chklStatoAnagrafico.Items(count).Selected Then
                    statiCheck.Add(chklStatoAnagrafico.Items(count).Value)
                    parStatoAnagrafico.AppendFormat("{0}, ", chklStatoAnagrafico.Items(count).Text)
                End If

            Next

            If parStatoAnagrafico.Length > 0 Then
                parStatoAnagrafico.Remove(parStatoAnagrafico.Length - 2, 2)
            End If

            rpt.AddParameter("StatoAnagrafico", parStatoAnagrafico.ToString())

            For count As Integer = 0 To statiCheck.Count - 1

                If count <> statiCheck.Count - 1 Then
                    filtroReport.AppendFormat(" ({{T_PAZ_PAZIENTI.PAZ_STATO_ANAGRAFICO}} = '{0}') OR", statiCheck(count).ToString())
                Else
                    filtroReport.AppendFormat(" ({{T_PAZ_PAZIENTI.PAZ_STATO_ANAGRAFICO}} = '{0}')", statiCheck(count).ToString())
                End If

            Next

            filtroReport.Append(")")

        Else
            rpt.AddParameter("StatoAnagrafico", "TUTTI")
        End If

        If txtStatusVaccEnabled.Text = "True" Then

            ' Status vaccinale
            If ddlStatusVaccinale.SelectedItem.Value <> "" Then

                filtroReport.AppendFormat(" AND {{T_PAZ_PAZIENTI.PAZ_STATO}} = '{0}'", ddlStatusVaccinale.SelectedItem.Value)

                rpt.AddParameter("StatusVaccinale", ddlStatusVaccinale.SelectedItem.Text)

            Else
                rpt.AddParameter("StatusVaccinale", "TUTTI")
            End If

            rpt.AddParameter("TipoStampa", "VACCINAZIONE")

        Else

            ' Malattia cronica
            If cmbMalCronica.SelectedItem.Value <> "" Then

                filtroReport.AppendFormat(" AND {{T_PAZ_MALATTIE.PMA_MAL_CODICE}} = '{0}'", cmbMalCronica.SelectedItem.Value)

                rpt.AddParameter("StatusVaccinale", cmbMalCronica.SelectedItem.Text)

            Else

                filtroReport.Append(" AND not isnull({T_PAZ_MALATTIE.PMA_MAL_CODICE}) AND {T_PAZ_MALATTIE.PMA_MAL_CODICE} <> '0'")

                rpt.AddParameter("StatusVaccinale", "TUTTE")

            End If

            rpt.AddParameter("TipoStampa", "MALATTIA")

        End If

        ' Sesso
        If ddlSesso.SelectedValue = "M" Or ddlSesso.SelectedValue = "F" Then

            filtroReport.AppendFormat(" AND {{T_PAZ_PAZIENTI.PAZ_SESSO}} = '{0}'", ddlSesso.SelectedValue)

            If nomeReport = Constants.ReportName.ElencoAssistiti Then
                rpt.AddParameter("Sesso", ddlSesso.SelectedValue)
            End If

        Else

            If nomeReport = Constants.ReportName.ElencoAssistiti Then
                rpt.AddParameter("Sesso", "ENTRAMBI")
            End If

        End If

        ' Categoria rischio (il parametro contenente il valore del filtro selezionato è previsto solo per il report ElencoAssistiti)
        Dim parametroCategoriaRischio As String = "NON SPECIFICATA"

        If Not String.IsNullOrEmpty(cmbCatRischio.SelectedItem.Value) Then

            If cmbCatRischio.SelectedItem.Value = Biz.BizCategorieRischio.CategorieRischio_Tutte Then

                filtroReport.Append(" AND not isnull({T_PAZ_PAZIENTI.PAZ_RSC_CODICE})")

                parametroCategoriaRischio = Biz.BizCategorieRischio.CategorieRischio_Tutte

            Else

                filtroReport.AppendFormat(" AND {{T_PAZ_PAZIENTI.PAZ_RSC_CODICE}} = '{0}'", cmbCatRischio.SelectedItem.Value)

                parametroCategoriaRischio = cmbCatRischio.SelectedItem.Text

            End If

        End If

        ' Paziente locale
        Dim parametroPazienteLocale As String = "NON SPECIFICATO"

        Select Case Me.ddlPazienteLocale.SelectedItem.Value
            Case "S"
                filtroReport.Append(" AND {T_PAZ_PAZIENTI.PAZ_LOCALE} = 'S'")
                parametroPazienteLocale = "S"
            Case "N"
                filtroReport.Append(" AND (ISNULL({T_PAZ_PAZIENTI.PAZ_LOCALE}) OR {T_PAZ_PAZIENTI.PAZ_LOCALE} = 'N')")
                parametroPazienteLocale = "N"
        End Select

        ' Gestione manuale
        Dim parametroGestioneManuale As String = "NON SPECIFICATA"

        Select Case ddlGestioneManuale.SelectedItem.Value
            Case "S"
                filtroReport.Append(" AND {T_PAZ_PAZIENTI.PAZ_COMPLETARE} = 'S'")
                parametroGestioneManuale = "S"
            Case "N"
                filtroReport.Append(" AND (ISNULL({T_PAZ_PAZIENTI.PAZ_COMPLETARE}) OR {T_PAZ_PAZIENTI.PAZ_COMPLETARE} = 'N')")
                parametroGestioneManuale = "N"
        End Select

        If nomeReport = Constants.ReportName.ElencoAssistiti Then
            rpt.AddParameter("CatRischio", parametroCategoriaRischio)
            rpt.AddParameter("PazLocale", parametroPazienteLocale)
            rpt.AddParameter("GestioneManuale", parametroGestioneManuale)
        End If

        ' Stato acquisizione
        If Not String.IsNullOrEmpty(ddlStatoAcquisizione.SelectedValue) Then

            If ddlStatoAcquisizione.SelectedValue = "-1" Then
                filtroReport.Append(" AND isnull({{T_PAZ_PAZIENTI.PAZ_STATO_ACQUISIZIONE}})")
            Else
                filtroReport.AppendFormat(" AND {{T_PAZ_PAZIENTI.PAZ_STATO_ACQUISIZIONE}} = {0}", Me.ddlStatoAcquisizione.SelectedValue)
            End If

            If nomeReport = Constants.ReportName.ElencoAssistiti Then
                rpt.AddParameter("StatoAcquisizione", ddlStatoAcquisizione.SelectedItem.Text)
            End If

        Else

            If nomeReport = Constants.ReportName.ElencoAssistiti Then
                ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
                If FlagConsensoVaccUslCorrente Then
                    rpt.AddParameter("StatoAcquisizione", "NON SPECIFICATO")
                Else
                    rpt.AddParameter("StatoAcquisizione", String.Empty)
                End If
            End If

        End If

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If Not OnVacReport.StampaReport(nomeReport, filtroReport.ToString(), rpt, Nothing, Nothing, bizReport.GetReportFolder(nomeReport)) Then
                    OnVacUtility.StampaNonPresente(Page, nomeReport)
                End If

            End Using
        End Using

    End Sub

#End Region

End Class
