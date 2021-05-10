Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager


Partial Class RilevazioneQuantitativa
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

#Region " Eventi Page "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            Me.PanelTitolo.InnerText = OnVacUtility.Variabili.CNS.Descrizione

            Me.lblErrorMessage.Text = String.Empty

            Me.CaricaDdlCategoriaRischio()
            Me.CaricaDdlEsenzioniMalattie()
			Me.CaricaChklStatoAnagrafico()
			ucSelezioneConsultori.MostraSoloAperti = False
			ucSelezioneConsultori.ImpostaCnsCorrente = False
			ucSelezioneConsultori.LoadGetCodici()

			Me.chkFittizia.Visible = Me.Settings.GESVACFITTIZIA

            ' Gestione della visualizzazione dei pulsanti di stampa in base all'installazione
            Me.ShowPrintButtons()

        End If

        'stampa del report tramite tasto invio (solo se il report è presente)
        If Me.Toolbar.Items.FromKeyButton("btnStampa").Visible Then

            Select Case Request.Form("__EVENTTARGET")

                Case "Stampa"
                    Me.Stampa(Constants.ReportName.VacciniSomministrati)

            End Select

        End If

    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub Toolbar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked

        Select Case e.Button.Key

            Case "btnStampa"

                Me.Stampa(Constants.ReportName.VacciniSomministrati)

            Case "btnStampaAmb"

                Me.Stampa(Constants.ReportName.VacciniSomministratiAmbulatorio)

            Case "btnStampaCatRischio"

                Me.Stampa(Constants.ReportName.VaccSommCatRischio)

            Case "btnStampaDose"

                Me.Stampa(Constants.ReportName.VacciniSomministratiDose)

            Case "btnStampaEseMal"

                Me.Stampa(Constants.ReportName.VacciniSomministratiEsenzioneMalattia)

            Case "btnStampaNomeCommerciale"

                Me.Stampa(Constants.ReportName.VacciniSomministratiNomeCommerciale)

        End Select

    End Sub

#End Region

#Region " Private "

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()

        listPrintButtons.Add(New PrintButton(Constants.ReportName.VacciniSomministrati, "btnStampa"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.VacciniSomministratiAmbulatorio, "btnStampaAmb"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.VaccSommCatRischio, "btnStampaCatRischio"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.VacciniSomministratiDose, "btnStampaDose"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.VacciniSomministratiEsenzioneMalattia, "btnStampaEseMal"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.VacciniSomministratiNomeCommerciale, "btnStampaNomeCommerciale"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Toolbar)

    End Sub

    'richiamabile anche dal page_load (modifica 29/07/2004)
    Private Sub Stampa(nomeReport As String)

        Dim filtroReport As New System.Text.StringBuilder("1=1")

        Dim rpt As New ReportParameter()

        ' Data di effettuazione della vaccinazione
        If Not String.IsNullOrEmpty(Me.odpDataEffettuazioneIniz.Text) And Not String.IsNullOrEmpty(Me.odpDataEffettuazioneFin.Text) Then

            filtroReport.AppendFormat(" AND {{T_VAC_ESEGUITE.VES_DATA_EFFETTUAZIONE}} {0}",
                                      Biz.BizReport.GetBetweenDateReportFilter(Me.odpDataEffettuazioneIniz.Data, Me.odpDataEffettuazioneFin.Data))

            rpt.AddParameter("DataEffettuazione1", Me.odpDataEffettuazioneIniz.Text)
            rpt.AddParameter("DataEffettuazione2", Me.odpDataEffettuazioneFin.Text)

        Else
            rpt.AddParameter("DataEffettuazione1", "")
            rpt.AddParameter("DataEffettuazione2", "")
        End If

        ' Data di nascita del paziente
        If Not String.IsNullOrEmpty(Me.odpDataNascitaIniz.Text) And Not String.IsNullOrEmpty(Me.odpDataNascitaFin.Text) Then

            filtroReport.AppendFormat(" AND {{T_PAZ_PAZIENTI.PAZ_DATA_NASCITA}} {0}",
                                      Biz.BizReport.GetBetweenDateReportFilter(Me.odpDataNascitaIniz.Data, Me.odpDataNascitaFin.Data))

            rpt.AddParameter("DataNascita1", Me.odpDataNascitaIniz.Text)
            rpt.AddParameter("DataNascita2", Me.odpDataNascitaFin.Text)

        Else
            rpt.AddParameter("DataNascita1", "")
            rpt.AddParameter("DataNascita2", "")
        End If

        ' Data di registrazione della vaccinazione
        If Not String.IsNullOrEmpty(Me.odpDataRegistrazioneIniz.Text) And Not String.IsNullOrEmpty(Me.odpDataRegistrazioneFin.Text) Then

            filtroReport.AppendFormat(" AND {{T_VAC_ESEGUITE.VES_DATA_REGISTRAZIONE}} {0}",
                                      Biz.BizReport.GetBetweenDateReportFilter(Me.odpDataRegistrazioneIniz.Data, Me.odpDataRegistrazioneFin.Data))

            rpt.AddParameter("DataRegistrazione1", Me.odpDataRegistrazioneIniz.Text)
            rpt.AddParameter("DataRegistrazione2", Me.odpDataRegistrazioneFin.Text)

        Else
            rpt.AddParameter("DataRegistrazione1", "")
            rpt.AddParameter("DataRegistrazione2", "")
        End If

        ' Tipo consultorio (A - adulti, P - pediatrico, V - vaccinatore)
        If Me.chklTipoCns.Items(0).Selected And Me.chklTipoCns.Items(1).Selected And Not Me.chklTipoCns.Items(2).Selected Then
            filtroReport.Append(" AND ({T_ANA_CONSULTORI.CNS_TIPO} ='A' OR {T_ANA_CONSULTORI.CNS_TIPO} ='P' OR (ISNULL({T_VAC_ESEGUITE.VES_CNS_CODICE}) AND ({T_ANA_CONSULTORI_REG.CNS_TIPO} ='A' OR {T_ANA_CONSULTORI_REG.CNS_TIPO} ='P')))")
            rpt.AddParameter("TipoCns", "ADULTI E PEDIATRICO")
        ElseIf Me.chklTipoCns.Items(0).Selected And Not Me.chklTipoCns.Items(1).Selected And Me.chklTipoCns.Items(2).Selected Then
            filtroReport.Append(" AND ({T_ANA_CONSULTORI.CNS_TIPO} ='A' OR {T_ANA_CONSULTORI.CNS_TIPO} ='V' OR (ISNULL({T_VAC_ESEGUITE.VES_CNS_CODICE}) AND ({T_ANA_CONSULTORI_REG.CNS_TIPO} ='A' OR {T_ANA_CONSULTORI_REG.CNS_TIPO} ='V')))")
            rpt.AddParameter("TipoCns", "ADULTI E VACCINATORE")
        ElseIf Me.chklTipoCns.Items(0).Selected And Not Me.chklTipoCns.Items(1).Selected And Not Me.chklTipoCns.Items(2).Selected Then
            filtroReport.Append(" AND ({T_ANA_CONSULTORI.CNS_TIPO} ='A' OR (ISNULL({T_VAC_ESEGUITE.VES_CNS_CODICE}) AND {T_ANA_CONSULTORI_REG.CNS_TIPO} ='A'))")
            rpt.AddParameter("TipoCns", "ADULTI")
        ElseIf Not Me.chklTipoCns.Items(0).Selected And Me.chklTipoCns.Items(1).Selected And Not Me.chklTipoCns.Items(2).Selected Then
            filtroReport.Append(" AND ({T_ANA_CONSULTORI.CNS_TIPO} ='P' OR (ISNULL({T_VAC_ESEGUITE.VES_CNS_CODICE}) AND {T_ANA_CONSULTORI_REG.CNS_TIPO} ='P'))")
            rpt.AddParameter("TipoCns", "PEDIATRICO")
        ElseIf Not Me.chklTipoCns.Items(0).Selected And Not Me.chklTipoCns.Items(1).Selected And Me.chklTipoCns.Items(2).Selected Then
             filtroReport.Append(" AND ({T_ANA_CONSULTORI.CNS_TIPO} ='V' OR (ISNULL({T_VAC_ESEGUITE.VES_CNS_CODICE}) AND {T_ANA_CONSULTORI_REG.CNS_TIPO} ='V'))")
            rpt.AddParameter("TipoCns", "VACCINATORE")
        ElseIf Not Me.chklTipoCns.Items(0).Selected And Me.chklTipoCns.Items(1).Selected And Me.chklTipoCns.Items(2).Selected Then
            filtroReport.Append(" AND ({T_ANA_CONSULTORI.CNS_TIPO} ='P' OR {T_ANA_CONSULTORI.CNS_TIPO} ='V' OR (ISNULL({T_VAC_ESEGUITE.VES_CNS_CODICE}) AND ({T_ANA_CONSULTORI_REG.CNS_TIPO} ='P' OR {T_ANA_CONSULTORI_REG.CNS_TIPO} ='V')))")
            rpt.AddParameter("TipoCns", "PEDIATRICO E VACCINATORE")
        Else
            rpt.AddParameter("TipoCns", "ADULTI PEDIATRICO E VACCINATORE")
        End If

        ' Tipo vaccinazione (VES_STATO: R - Registrata, altrimenti Eseguita)
        Dim strStatoVac As New System.Text.StringBuilder()

        If Me.chklStatoVac.Items(0).Selected And Not Me.chklStatoVac.Items(1).Selected Then
            filtroReport.Append(" AND (isnull({T_VAC_ESEGUITE.VES_STATO}) OR {T_VAC_ESEGUITE.VES_STATO} <> 'R')")
            strStatoVac.Append("ESEGUITE")
        ElseIf Not Me.chklStatoVac.Items(0).Selected And Me.chklStatoVac.Items(1).Selected Then
            filtroReport.Append(" AND {T_VAC_ESEGUITE.VES_STATO} = 'R'")
            strStatoVac.Append("REGISTRATE")
        Else
            strStatoVac.Append("ESEGUITE E REGISTRATE")
        End If

        If Me.Settings.GESVACFITTIZIA Then
            If Me.chkFittizia.Checked Then
                strStatoVac.Append(" (incluse fittizie)")
            Else
                filtroReport.Append(" AND ({T_VAC_ESEGUITE.VES_FLAG_FITTIZIA} <> 'S')")
            End If
        End If
        rpt.AddParameter("StatoVac", strStatoVac.ToString())

        ' Distretto
        If Not String.IsNullOrEmpty(Me.fmDistretto.Codice) And Not String.IsNullOrEmpty(Me.fmDistretto.Descrizione) Then
            filtroReport.AppendFormat(" AND ((ISNULL({{T_VAC_ESEGUITE.VES_CNS_CODICE}}) AND {{T_ANA_CONSULTORI_REG.CNS_DIS_CODICE}} = '{0}') OR ({{T_ANA_CONSULTORI.CNS_DIS_CODICE}} = '{0}'))", Me.fmDistretto.Codice)
            rpt.AddParameter("Distretto", Me.fmDistretto.Descrizione)
        Else
            rpt.AddParameter("Distretto", "TUTTI")
        End If

		' Consultorio
		Dim lista As List(Of String) = ucSelezioneConsultori.GetConsultoriSelezionati
        If lista.Count > 0 Then
            filtroReport.AppendFormat(" AND ({{T_VAC_ESEGUITE.VES_CNS_CODICE}} IN ['{0}'] OR (ISNULL({{T_VAC_ESEGUITE.VES_CNS_CODICE}}) AND {{T_VAC_ESEGUITE.VES_CNS_REGISTRAZIONE}} IN ['{0}'] ))", lista.Aggregate(Function(p, g) p & "', '" & g))
            rpt.AddParameter("Consultorio", ucSelezioneConsultori.GetDescrizioneConsultoriSelezionatiPerStampe())
        Else
            If String.IsNullOrEmpty(Me.fmDistretto.Codice) And String.IsNullOrEmpty(Me.fmDistretto.Descrizione) Then
                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                    Using bizCns As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)
                        lista = bizCns.GetListCodiceConsultoriUtente(OnVacContext.UserId)
                        If lista.Count > 0 Then
                            filtroReport.AppendFormat(" AND ({{T_VAC_ESEGUITE.VES_CNS_CODICE}} IN ['{0}'] OR (ISNULL({{T_VAC_ESEGUITE.VES_CNS_CODICE}}) AND {{T_VAC_ESEGUITE.VES_CNS_REGISTRAZIONE}} IN ['{0}'] ))", lista.Aggregate(Function(p, g) p & "', '" & g))
                        End If
                    End Using
                End Using
            End If
            rpt.AddParameter("Consultorio", "TUTTI")
        End If

        ' Comune di residenza
        If Not String.IsNullOrEmpty(Me.fmComuneRes.Codice) And Not String.IsNullOrEmpty(Me.fmComuneRes.Descrizione) Then
            filtroReport.AppendFormat(" AND ({{T_PAZ_PAZIENTI.PAZ_COM_CODICE_RESIDENZA}} = '{0}')", Me.fmComuneRes.Codice)
            rpt.AddParameter("Comune", Me.fmComuneRes.Descrizione)
        Else
            rpt.AddParameter("Comune", "TUTTI")
        End If

        ' Circoscrizione di residenza
        If Not String.IsNullOrEmpty(Me.fmCircoscrizione.Codice) And Not String.IsNullOrEmpty(Me.fmCircoscrizione.Descrizione) Then
            filtroReport.AppendFormat(" AND ({{T_PAZ_PAZIENTI.PAZ_CIR_CODICE}} = '{0}')", Me.fmCircoscrizione.Codice)
            rpt.AddParameter("Circoscrizione", Me.fmCircoscrizione.Descrizione)
        Else
            rpt.AddParameter("Circoscrizione", "TUTTE")
        End If

        ' Medico in ambulatorio
        Select Case Me.rblMedInAmb.SelectedValue
            Case 1
                filtroReport.Append(" AND ({T_VAC_ESEGUITE.VES_OPE_IN_AMBULATORIO} = 'S')")
                rpt.AddParameter("medInAmb", "SI")
            Case 2
                filtroReport.Append(" AND ({T_VAC_ESEGUITE.VES_OPE_IN_AMBULATORIO} = 'N')")
                rpt.AddParameter("medInAmb", "NO")
            Case Else
                rpt.AddParameter("medInAmb", "TUTTI")
        End Select

        ' Esenzione Malattia
        If Not String.IsNullOrEmpty(Me.ddlEsenzioniMalattie.SelectedValue) Then

            Dim codiceDescrizioneEsenzioneMalattia As String() = Me.ddlEsenzioniMalattie.SelectedValue.Split("|")

            filtroReport.AppendFormat(" AND {{T_VAC_ESEGUITE.VES_MAL_CODICE_MALATTIA}} = '{0}' AND {{T_VAC_ESEGUITE.VES_CODICE_ESENZIONE}} = '{1}'",
                                      codiceDescrizioneEsenzioneMalattia(0), codiceDescrizioneEsenzioneMalattia(1))

            rpt.AddParameter("EsenzioneMalattia", Me.ddlEsenzioniMalattie.SelectedItem.Text)

        Else
            rpt.AddParameter("EsenzioneMalattia", "TUTTE")
        End If

        ' Categoria rischio (il parametro contenente il valore del filtro selezionato è previsto solo per il report VaccSommCatRischio)
        Dim parametroCategoriaRischio As String = String.Empty

        If Me.cmbCatRischio.SelectedItem.Value <> Biz.BizCategorieRischio.CategorieRischio_Tutte Then
            filtroReport.AppendFormat(" AND {{T_PAZ_PAZIENTI.PAZ_RSC_CODICE}} = '{0}'", Me.cmbCatRischio.SelectedItem.Value)
            parametroCategoriaRischio = Me.cmbCatRischio.SelectedItem.Text
        Else
            If nomeReport = Constants.ReportName.VaccSommCatRischio Then
                filtroReport.Append(" AND not isnull({T_PAZ_PAZIENTI.PAZ_RSC_CODICE})")
                parametroCategoriaRischio = Biz.BizCategorieRischio.CategorieRischio_Tutte
            End If
        End If

        If nomeReport = Constants.ReportName.VaccSommCatRischio Then
            rpt.AddParameter("CatRischio", parametroCategoriaRischio)
        End If

        ' Stato anagrafico del paziente
        If Me.chklStatoAnagrafico.SelectedValues.Count > 0 Then
            filtroReport.AppendFormat(" AND {{T_PAZ_PAZIENTI.PAZ_STATO_ANAGRAFICO}} in [{0}]", Me.chklStatoAnagrafico.SelectedValues.ToString(True))
            rpt.AddParameter("StatiAnagrafici", Me.chklStatoAnagrafico.SelectedItems.ToString())
        Else
            rpt.AddParameter("StatiAnagrafici", "TUTTI")
        End If

        ' N.B. : se OnVacContext.UserDescription è Nothing, all'rpt deve essere passata la stringa vuota se no dà errore
        If String.IsNullOrEmpty(OnVacContext.UserDescription) Then
            rpt.AddParameter("UtenteStampa", String.Empty)
        Else
            rpt.AddParameter("UtenteStampa", OnVacContext.UserDescription)
        End If

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If Not OnVacReport.StampaReport(nomeReport, filtroReport.ToString(), rpt, Nothing, Nothing, bizReport.GetReportFolder(nomeReport)) Then
                    OnVacUtility.StampaNonPresente(Page, nomeReport)
                End If

            End Using
        End Using

    End Sub

#Region " Caricamento dati "

    Private Sub CaricaChklStatoAnagrafico()

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizStatiAnagrafici As New Biz.BizStatiAnagrafici(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                Me.chklStatoAnagrafico.DataValueField = "SAN_CODICE"
                Me.chklStatoAnagrafico.DataTextField = "SAN_DESCRIZIONE"
                Me.chklStatoAnagrafico.DataSource = bizStatiAnagrafici.LeggiStatiAnagrafici()
                Me.chklStatoAnagrafico.DataBind()

            End Using
        End Using

    End Sub

    Private Sub CaricaDdlCategoriaRischio()

        Dim dtCategorieRischio As DataTable = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizCategorieRischio As New Biz.BizCategorieRischio(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                dtCategorieRischio = bizCategorieRischio.LoadDataTableCategorieRischio(True, False)

            End Using

        End Using

        Me.cmbCatRischio.DataValueField = "RSC_CODICE"
        Me.cmbCatRischio.DataTextField = "RSC_DESCRIZIONE"
        Me.cmbCatRischio.DataSource = dtCategorieRischio
        Me.cmbCatRischio.DataBind()

    End Sub

    Private Sub CaricaDdlEsenzioniMalattie()

        Me.ddlEsenzioniMalattie.Items.Clear()

        Dim listMalattie As List(Of Entities.Malattia) = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizMalattie As New Biz.BizMalattie(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), New Biz.BizLogOptions(DataLogStructure.TipiArgomento.PAZIENTI, False))

                listMalattie = bizMalattie.LoadMalattieEsenzione()

            End Using

        End Using

        If Not listMalattie Is Nothing Then

            For Each malattia As Entities.Malattia In listMalattie

                Me.ddlEsenzioniMalattie.Items.Add(
                    New ListItem(String.Format("{0} - {1}", malattia.DescrizioneMalattia, malattia.CodiceEsenzione),
                                 String.Format("{0}|{1}", malattia.CodiceMalattia, malattia.CodiceEsenzione)))
            Next

        End If

        Me.ddlEsenzioniMalattie.Items.Insert(0, String.Empty)

    End Sub

#End Region

#End Region

#Region " Eventi finestre modali "

    Private Sub fmCircoscrizione_Change(sender As Object, e As Onit.Controls.OnitModalList.ModalListaEventArgument) Handles fmCircoscrizione.Change

        OnVacUtility.DisabilitaModale(Me.fmComuneRes, IIf(Me.fmCircoscrizione.Codice <> "" And Me.fmCircoscrizione.Descrizione <> "", True, False))

    End Sub

    Private Sub fmComuneRes_Change(sender As Object, e As Onit.Controls.OnitModalList.ModalListaEventArgument) Handles fmComuneRes.Change

        OnVacUtility.DisabilitaModale(Me.fmCircoscrizione, IIf(Me.fmComuneRes.Codice <> "" And Me.fmComuneRes.Descrizione <> "", True, False))

    End Sub

#End Region

End Class
