Imports System.Collections.Generic
Imports System.Linq

Imports Onit.Database.DataAccessManager
Imports Onit.Controls

Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.Filters

Partial Class CoperturaVaccinale
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

#Region " Types "

    Private Class ReportInfo

        Public ReportParameter As ReportParameter
        Public Name As String
        Public Folder As String
        Public Filter As String

    End Class

#End Region

#Region " Eventi page "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            Me.PanelTitolo.InnerText = OnVacUtility.Variabili.CNS.Descrizione

            Me.odpDataNascitaIniz.Text = String.Empty
            Me.odpDataNascitaFin.Text = String.Empty

            'impostazione giorni (120 anni) nel campo GiorniVita
            Me.txtGiorniVita.Text = (120 * 365)

            Me.CaricaStatiAnagrafici()
			Me.CaricaVaccinazioni()
			ucSelezioneConsultori.MostraSoloAperti = False
			ucSelezioneConsultori.ImpostaCnsCorrente = False
			ucSelezioneConsultori.LoadGetCodici()

			' Nasconde i pulsanti di stampa in base alla presenza del report nell'installazione specifica
			Me.ShowPrintButtons()

        End If

	End Sub

#End Region

#Region " Caricamento dati "

    Private Sub CaricaStatiAnagrafici()

        Dim dtStatiAnag As DataTable = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizStatiAnagrafici As New Biz.BizStatiAnagrafici(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                dtStatiAnag = bizStatiAnagrafici.LeggiStatiAnagrafici()

            End Using
        End Using

        Me.chklStatoAnagrafico.DataValueField = "SAN_CODICE"
        Me.chklStatoAnagrafico.DataTextField = "SAN_DESCRIZIONE"
        Me.chklStatoAnagrafico.DataSource = dtStatiAnag
        Me.chklStatoAnagrafico.DataBind()

    End Sub

    Private Sub CaricaVaccinazioni()

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Me.chkVaccinazioni.DataValueField = "VAC_CODICE"
            Me.chkVaccinazioni.DataTextField = "VAC_DESCRIZIONE"
            Me.chkVaccinazioni.DataSource = genericProvider.AnaVaccinazioni.GetVaccinazioni(chklModVaccinazione.SelectedValues)
            Me.chkVaccinazioni.DataBind()

        End Using

    End Sub

#End Region

#Region " Eventi toolbar "

    Private Sub Toolbar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked

        ' Controllo valorizzazione date di nascita (anche lato client con alert)
        If Me.odpDataNascitaIniz.Text = String.Empty Or Me.odpDataNascitaFin.Text = String.Empty Then
            Return
        End If

        ' Controllo numero dosi (anche lato client con alert, compreso il controllo sul formato)
        If String.IsNullOrEmpty(Me.txtNumeroDosi.Text) OrElse Not IsNumeric(Me.txtNumeroDosi.Text) Then
            Return
        End If

        ' Controllo giorni vita (anche lato client con alert, compreso il controllo sul formato)
        If String.IsNullOrEmpty(Me.txtGiorniVita.Text) OrElse Not IsNumeric(Me.txtGiorniVita.Text) Then
            Return
        End If

        ' Controllo selezione vaccinazioni
        If Me.chkVaccinazioni.SelectedValues.Count = 0 Then
            Me.OnitLayout31.InsertRoutineJS("alert(""Impossibile stampare il report. Selezionare almeno una vaccinazione."");")
            Return
        End If

        ' Recupero dati per il report
        Dim reportInfo As ReportInfo = Nothing

        Select Case e.Button.Key

            Case "btnCoperturaAss"

                reportInfo = Me.StampaCoperturaAssociazione()

            Case "btnStampa"

                reportInfo = Me.StampaCoperturaVaccinazione()

            Case "btnStampaCns"

                reportInfo = Me.StampaCoperturaConsultorio()

            Case "btnMotiviInadempienti"

                reportInfo = Me.StampaMotiviInadempienti()

            Case "btnElencoNonVaccinati"

                reportInfo = Me.StampaElencoNonVaccinati()

            Case "btnElencoVaccinati"

                reportInfo = Me.StampaElencoVaccinati()

        End Select

        If reportInfo Is Nothing Then
            Me.OnitLayout31.InsertRoutineJS("alert(""Stampa non effettuata: nessun dato da stampare in base ai filtri impostati."");")
            Return
        Else
            ' Creazione del report
            If Not OnVacReport.StampaReport(reportInfo.Name, reportInfo.Filter, reportInfo.ReportParameter, Nothing, Nothing, reportInfo.Folder) Then
                OnVacUtility.StampaNonPresente(Page, reportInfo.Name)
            End If
        End If

    End Sub

#End Region

#Region " Eventi finestre modali "



	'se è valorizzato il distretto, deve disabilitare il consultorio [modifica 06/07/2005]
	Private Sub fmDistretto_Change(Sender As Object, E As Onit.Controls.OnitModalList.ModalListaEventArgument) Handles fmDistretto.Change
		'OnVacUtility.DisabilitaModale(fmConsultorio, IIf(fmDistretto.Codice <> "" And fmDistretto.Descrizione <> "", True, False))
		ucSelezioneConsultori.MostraCnsUtente = True
		ucSelezioneConsultori.MostraSoloAperti = False
		ucSelezioneConsultori.ImpostaCnsCorrente = True
		ucSelezioneConsultori.FiltroDistretto = fmDistretto.Codice
		ucSelezioneConsultori.LoadGetCodici()
	End Sub

    Private Sub fmCircoscrizione_Change(Sender As Object, E As Onit.Controls.OnitModalList.ModalListaEventArgument) Handles fmCircoscrizione.Change
        OnVacUtility.DisabilitaModale(fmComuneRes, IIf(fmCircoscrizione.Codice <> "" And fmCircoscrizione.Descrizione <> "", True, False))
    End Sub

    Private Sub fmComuneRes_Change(Sender As Object, E As Onit.Controls.OnitModalList.ModalListaEventArgument) Handles fmComuneRes.Change
        OnVacUtility.DisabilitaModale(fmCircoscrizione, IIf(fmComuneRes.Codice <> "" And fmComuneRes.Descrizione <> "", True, False))
    End Sub

#End Region

#Region " Eventi checkbox "

    Private Sub chklModVaccinazione_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles chklModVaccinazione.SelectedIndexChanged

        Me.CaricaVaccinazioni()

        If Me.chklModVaccinazione.SelectedItems.Count = 0 Then
            Me.chkVaccinazioni.UnselectAll()
        Else
            Me.chkVaccinazioni.SelectAll()
        End If

    End Sub

#End Region

#Region " Private "

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()

        listPrintButtons.Add(New PrintButton(Constants.ReportName.CoperturaVaccinale, "btnStampa"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.CoperturaVaccinaleAssociazione, "btnCoperturaAss"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.MotiviInadempienti, "btnMotiviInadempienti"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.CoperturaVaccinati, "btnElencoVaccinati"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.CoperturaNonVaccinati, "btnElencoNonVaccinati"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.CoperturaVaccinaleCNS, "btnStampaCns"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Me.Toolbar)

    End Sub

    Private Function RicavaConsultoriDistretto() As List(Of String)

        Dim result As New List(Of String)()

        Using dam As IDAM = OnVacUtility.OpenDam()

            With dam.QB
                .NewQuery()
				.AddTables("T_ANA_DISTRETTI, T_ANA_CONSULTORI,T_ANA_LINK_UTENTI_CONSULTORI")
				.AddSelectFields("CNS_CODICE")
                .AddWhereCondition("DIS_CODICE", Comparatori.Uguale, Me.fmDistretto.Codice, DataTypes.Stringa)
				.AddWhereCondition("CNS_DIS_CODICE", Comparatori.Uguale, "DIS_CODICE", DataTypes.Join)
				.AddWhereCondition("LUC_UTE_ID", Comparatori.Uguale, OnVacContext.UserId, DataTypes.Numero)
				.AddWhereCondition("LUC_CNS_CODICE", Comparatori.Uguale, "CNS_CODICE", DataTypes.Join)
			End With

            Using dr As IDataReader = dam.BuildDataReader()
                While dr.Read()
                    result.Add(dr("CNS_CODICE"))
                End While
            End Using

        End Using

        Return result

    End Function

    Private Function GetFiltriCopertura() As FiltriCopertura

        Dim filtriCopertura As New FiltriCopertura()

        filtriCopertura.giorniVita = Integer.Parse(Me.txtGiorniVita.Text)
        filtriCopertura.richiamo = Integer.Parse(Me.txtNumeroDosi.Text)
        filtriCopertura.dataNascitaDa = Me.odpDataNascitaIniz.Data
        filtriCopertura.dataNascitaA = Me.odpDataNascitaFin.Data

        filtriCopertura.dataEffettuazioneDa = IIf(Me.odpDataEffettuazioneIniz.Data <> Date.MinValue, Me.odpDataEffettuazioneIniz.Data, Nothing)
        filtriCopertura.dataEffettuazioneA = IIf(Me.odpDataEffettuazioneFin.Data <> Date.MinValue, Me.odpDataEffettuazioneFin.Data, Nothing)

        filtriCopertura.codiceComuneResidenza = Me.fmComuneRes.Codice
        filtriCopertura.codiceCircoscrizione = Me.fmCircoscrizione.Codice

		If ucSelezioneConsultori.GetConsultoriSelezionati().Count = 0 Then
			If String.IsNullOrEmpty(Me.fmDistretto.Codice) Then
				Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
					Using bizCns As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)
						filtriCopertura.codiceConsultorio = bizCns.GetListCodiceConsultoriUtente(OnVacContext.UserId)
					End Using
				End Using
			Else
				filtriCopertura.codiceConsultorio = RicavaConsultoriDistretto()
			End If
		Else
			filtriCopertura.codiceConsultorio = New List(Of String)()
			filtriCopertura.codiceConsultorio = ucSelezioneConsultori.GetConsultoriSelezionati()
			'filtriCopertura.codiceConsultorio.Add(Me.fmConsultorio.Codice)
		End If

        filtriCopertura.tipoVaccinazioni = Me.chklModVaccinazione.SelectedValues
        filtriCopertura.Sesso = Me.chklSesso.SelectedValues
        filtriCopertura.StatoAnagrafico = Me.chklStatoAnagrafico.SelectedValues
        filtriCopertura.codiceVaccinazione = Me.chkVaccinazioni.SelectedValues

        Return filtriCopertura

    End Function

    Private Sub AddCommonReportParameter(rpt As ReportParameter)

        Dim strtmp As String = String.Empty

        rpt.AddParameter("Ndosi", Me.txtNumeroDosi.Text)
        rpt.AddParameter("DataNascita1", Me.odpDataNascitaIniz.Text)
        rpt.AddParameter("DataNascita2", Me.odpDataNascitaFin.Text)
        rpt.AddParameter("GiorniVita", Me.txtGiorniVita.Text)
        rpt.AddParameter("DataEffettuazioneIniz", Me.odpDataEffettuazioneIniz.Text)
        rpt.AddParameter("DataEffettuazioneFin", Me.odpDataEffettuazioneFin.Text)

        If (Me.chklSesso.Items(0).Selected = True And Me.chklSesso.Items(1).Selected = False) Then
            rpt.AddParameter("Sesso", "1")
        ElseIf (Me.chklSesso.Items(0).Selected = False And Me.chklSesso.Items(1).Selected = True) Then
            rpt.AddParameter("Sesso", "2")
        Else
            rpt.AddParameter("Sesso", "3")
        End If

        If Me.fmComuneRes.Codice <> "" And Me.fmComuneRes.Descrizione <> "" Then
            rpt.AddParameter("ComRes", Me.fmComuneRes.Descrizione)
        Else
            rpt.AddParameter("ComRes", "TUTTI")
        End If

        If Me.fmCircoscrizione.Codice <> "" And Me.fmCircoscrizione.Descrizione <> "" Then
            rpt.AddParameter("Circoscriz", Me.fmCircoscrizione.Descrizione)
        Else
            rpt.AddParameter("Circoscriz", "TUTTE")
        End If

		If ucSelezioneConsultori.GetConsultoriSelezionati.Count > 0 Then
			'If Me.fmDistretto.Codice <> "" And Me.fmDistretto.Descrizione <> "" Then
			'	rpt.AddParameter("Consultorio", Me.fmDistretto.Descrizione)
			'Else
			'	'rpt.AddParameter("Consultorio", Me.fmConsultorio.Descrizione)

			'End If

			rpt.AddParameter("Consultorio", ucSelezioneConsultori.GetDescrizioneConsultoriSelezionatiPerStampe())

		Else
			rpt.AddParameter("Consultorio", "TUTTI")
        End If

        If Me.fmDistretto.Codice <> "" And Me.fmDistretto.Descrizione <> "" Then
            rpt.AddParameter("Distretto", Me.fmDistretto.Descrizione)
        Else
            rpt.AddParameter("Distretto", "TUTTI")
        End If

        For i As Int16 = 0 To Me.chklStatoAnagrafico.Items.Count - 1
            If Me.chklStatoAnagrafico.Items(i).Selected Then
                strtmp &= Me.chklStatoAnagrafico.Items(i).Text & " "
            End If
        Next
        rpt.AddParameter("SAnagrafico", strtmp)

        If Me.chklModVaccinazione.SelectedValues.Count > 0 Then
            Dim s As String = Me.chklModVaccinazione.SelectedItems.Select(Function(p) p.Text.ToUpper()).Aggregate(Function(p, g) p & ", " & g)
            rpt.AddParameter("TipoVac", s)
        Else
            rpt.AddParameter("TipoVac", "OBBLIGATORIE, RACCOMANDATE, FACOLTATIVE")
        End If

    End Sub

#Region " Stampe "

	Private Function StampaCoperturaAssociazione() As ReportInfo

        Dim rpt As New ReportParameter()
        Dim reportFilter As New System.Text.StringBuilder()
        Dim reportFolder As String = String.Empty

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Dim filtriCopertura As FiltriCopertura = Me.GetFiltriCopertura()

            AddCommonReportParameter(rpt)
            rpt.AddParameter("TotaleAnagrafico", genericProvider.Copertura.GetTotaleAnagrafico(filtriCopertura))

            ' Filtro data di nascita dei pazienti
            reportFilter.AppendFormat("{{T_PAZ_PAZIENTI.PAZ_DATA_NASCITA}} in DateTime ({0},{1},{2}, 00, 00, 00) to DateTime ({3},{4},{5}, 00, 00, 00) ",
                                      Me.odpDataNascitaIniz.Data.Year, Me.odpDataNascitaIniz.Data.Month, Me.odpDataNascitaIniz.Data.Day,
                                      Me.odpDataNascitaFin.Data.Year, Me.odpDataNascitaFin.Data.Month, Me.odpDataNascitaFin.Data.Day)

            ' Filtro distretto (ricava i consultori associati al distretto specificato e li filtra)
            If Me.fmDistretto.Codice <> String.Empty And Me.fmDistretto.Descrizione <> String.Empty Then

                Dim filtroDistretto As New System.Text.StringBuilder()

                Dim listCodiciConsultori As List(Of String) = Me.RicavaConsultoriDistretto()

                If Not listCodiciConsultori Is Nothing AndAlso listCodiciConsultori.Count > 0 Then

                    For i As Int16 = 0 To listCodiciConsultori.Count - 1
                        If Not String.IsNullOrEmpty(listCodiciConsultori(i)) Then
                            filtroDistretto.AppendFormat("{{T_PAZ_PAZIENTI.PAZ_CNS_CODICE}} = '{0}' OR ", listCodiciConsultori(i))
                        End If
                    Next
                    If filtroDistretto.Length > 0 Then
                        filtroDistretto.Remove(filtroDistretto.Length - 3, 3)
                        reportFilter.AppendFormat(" AND ({0}) ", filtroDistretto.ToString())
                    End If

                End If

            End If

			' Filtro consultorio
			'If Me.fmConsultorio.Codice <> String.Empty And Me.fmConsultorio.Descrizione <> String.Empty Then
			'	reportFilter.AppendFormat(" AND {{T_PAZ_PAZIENTI.PAZ_CNS_CODICE}} = '{0}' ", Me.fmConsultorio.Codice)
			'End If
			If ucSelezioneConsultori.GetConsultoriSelezionati.Count > 0 Then
				reportFilter.AppendFormat(" AND ({{T_PAZ_PAZIENTI.PAZ_CNS_CODICE}} IN ['{0}']) ", ucSelezioneConsultori.GetConsultoriSelezionati.Aggregate(Function(p, g) p & "', '" & g))
			Else

				Using bizCns As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)
					Dim listaCodici As New List(Of String)
					listaCodici = bizCns.GetListCodiceConsultoriUtente(OnVacContext.UserId)
					If listaCodici.Count > 0 Then
						reportFilter.AppendFormat(" AND ({{T_PAZ_PAZIENTI.PAZ_CNS_CODICE}} IN ['{0}']) ", listaCodici.Aggregate(Function(p, g) p & "', '" & g))
					End If
				End Using

			End If

			' Filtro comune residenza
			If Me.fmComuneRes.Codice <> String.Empty And Me.fmComuneRes.Descrizione <> String.Empty Then
                reportFilter.AppendFormat(" AND {{T_PAZ_PAZIENTI.PAZ_COM_CODICE_RESIDENZA}} = '{0}' ", Me.fmComuneRes.Codice)
            End If

            ' Filtro circoscrizione
            If Me.fmCircoscrizione.Codice <> String.Empty And Me.fmCircoscrizione.Descrizione <> String.Empty Then
                reportFilter.AppendFormat(" AND {{T_PAZ_PAZIENTI.PAZ_CIR_CODICE}} = '{0}' ", Me.fmCircoscrizione.Codice)
            End If

            ' Filtro stato anagrafico
            Dim filtroStatoAnagrafico As New System.Text.StringBuilder()
            For i As Int16 = 0 To Me.chklStatoAnagrafico.Items.Count - 1
                If Me.chklStatoAnagrafico.Items(i).Selected Then
                    filtroStatoAnagrafico.AppendFormat(" ({{T_PAZ_PAZIENTI.PAZ_STATO_ANAGRAFICO}} = '{0}') OR ", Me.chklStatoAnagrafico.Items(i).Value)
                End If
            Next
            If filtroStatoAnagrafico.Length > 0 Then
                filtroStatoAnagrafico.Remove(filtroStatoAnagrafico.Length - 3, 3)
                reportFilter.AppendFormat(" AND ({0}) ", filtroStatoAnagrafico.ToString())
            End If

            ' Filtro data effettuazione
            If Me.odpDataEffettuazioneIniz.Text <> String.Empty And Me.odpDataEffettuazioneFin.Text <> String.Empty Then

                reportFilter.AppendFormat(" AND {{T_VAC_ESEGUITE.VES_DATA_EFFETTUAZIONE}} in DateTime ({0},{1},{2}, 00, 00, 00) to DateTime ({3},{4},{5}, 00, 00, 00)",
                                          Me.odpDataEffettuazioneIniz.Data.Year, Me.odpDataEffettuazioneIniz.Data.Month, Me.odpDataEffettuazioneIniz.Data.Day,
                                          Me.odpDataEffettuazioneFin.Data.Year, Me.odpDataEffettuazioneFin.Data.Month, Me.odpDataEffettuazioneFin.Data.Day)

            End If

            ' Filtro giorni di vita (dato obbligatorio, se non presente non chiama la funzione di stampa)
            reportFilter.AppendFormat(" AND DateDiff('d', {{T_PAZ_PAZIENTI.PAZ_DATA_NASCITA}}, {{T_VAC_ESEGUITE.VES_DATA_EFFETTUAZIONE}}) <= {0} ", Me.txtGiorniVita.Text)

            ' Filtro numero dosi (dato obbligatorio, se non presente non chiama la funzione di stampa)
            reportFilter.AppendFormat(" AND ({{T_VAC_ESEGUITE.VES_N_RICHIAMO}} = {0}) ", Me.txtNumeroDosi.Text)

            ' Filtro tipo vaccinazioni
            If Me.chklModVaccinazione.SelectedValues.Count > 0 Then
                reportFilter.AppendFormat(" AND ({{T_ANA_VACCINAZIONI.VAC_OBBLIGATORIA}} in [{0}])  ", Me.chklModVaccinazione.SelectedValues.ToString(True))
            End If

            ' Filtro tipo vaccinazioni
            If Me.chkVaccinazioni.SelectedValues.Count > 0 Then
                reportFilter.AppendFormat(" AND ({{T_VAC_ESEGUITE.VES_VAC_CODICE}} in [{0}])  ", Me.chkVaccinazioni.SelectedValues.ToString(True))
            End If

            ' Filtro sesso
            Dim maschi As Boolean = Me.chklSesso.Items(0).Selected
            Dim femmine As Boolean = Me.chklSesso.Items(1).Selected

            If maschi And Not femmine Then
                reportFilter.Append(" AND {T_PAZ_PAZIENTI.PAZ_SESSO} = 'M' ")
            ElseIf Not maschi And femmine Then
                reportFilter.Append(" AND {T_PAZ_PAZIENTI.PAZ_SESSO} = 'F' ")
            End If

            ' Recupero directory del report
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                reportFolder = bizReport.GetReportFolder(Constants.ReportName.CoperturaVaccinaleAssociazione)
            End Using

        End Using

        Dim reportInfo As New ReportInfo()

        reportInfo.Filter = reportFilter.ToString()
        reportInfo.Folder = reportFolder
        reportInfo.Name = Constants.ReportName.CoperturaVaccinaleAssociazione
        reportInfo.ReportParameter = rpt

        Return reportInfo

    End Function

    Private Function StampaCoperturaVaccinazione() As ReportInfo

        Dim rpt As New ReportParameter()
        Dim reportFolder As String = String.Empty

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Dim filtriCopertura As FiltriCopertura = Me.GetFiltriCopertura()

            Dim dsCoperturaVaccinale As dsCoperturaVaccinale = genericProvider.Copertura.GetCoperturaVaccinale(filtriCopertura)

            If dsCoperturaVaccinale Is Nothing OrElse dsCoperturaVaccinale.CoperturaVaccinale Is Nothing OrElse dsCoperturaVaccinale.CoperturaVaccinale.Rows.Count = 0 Then
                Return Nothing
            End If

            rpt.set_dataset(dsCoperturaVaccinale)

            ' Impostazione parametri report
            AddCommonReportParameter(rpt)
            rpt.AddParameter("TotaleAnagrafico", genericProvider.Copertura.GetTotaleAnagrafico(filtriCopertura))
            rpt.AddParameter("dettaglioCoperturaVisibile", IIf(Me.Settings.GES_CALCOLO_COPERTURA, "S", "N"))

            ' Recupero directory del report
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                reportFolder = bizReport.GetReportFolder(Constants.ReportName.CoperturaVaccinale)
            End Using

        End Using

        Dim reportInfo As New ReportInfo()

        reportInfo.Filter = String.Empty
        reportInfo.Folder = reportFolder
        reportInfo.Name = Constants.ReportName.CoperturaVaccinale
        reportInfo.ReportParameter = rpt

        Return reportInfo

    End Function

    Private Function StampaCoperturaConsultorio() As ReportInfo

        Dim rpt As New ReportParameter()
        Dim reportFolder As String = String.Empty

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Dim filtriCopertura As FiltriCopertura = Me.GetFiltriCopertura()

            Dim dsCoperturaVaccinaleCNS As dsCoperturaVaccinaleCNS = genericProvider.Copertura.GetCoperturaVaccinaleCNS(filtriCopertura)

            If dsCoperturaVaccinaleCNS Is Nothing OrElse dsCoperturaVaccinaleCNS.CoperturaVaccinaleCNS Is Nothing OrElse dsCoperturaVaccinaleCNS.CoperturaVaccinaleCNS.Rows.Count = 0 Then
                Return Nothing
            End If

            rpt.set_dataset(dsCoperturaVaccinaleCNS)

            ' Impostazione parametri report
            AddCommonReportParameter(rpt)
            rpt.AddParameter("TotaleAnagrafico", genericProvider.Copertura.GetTotaleAnagrafico(filtriCopertura))
            rpt.AddParameter("dettaglioCoperturaVisibile", IIf(Me.Settings.GES_CALCOLO_COPERTURA, "S", "N"))

            ' Recupero directory del report
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                reportFolder = bizReport.GetReportFolder(Constants.ReportName.CoperturaVaccinaleCNS)
            End Using

        End Using

        Dim reportInfo As New ReportInfo()

        reportInfo.Filter = String.Empty
        reportInfo.Folder = reportFolder
        reportInfo.Name = Constants.ReportName.CoperturaVaccinaleCNS
        reportInfo.ReportParameter = rpt

        Return reportInfo

    End Function

    Private Function StampaMotiviInadempienti() As ReportInfo

        Dim rpt As New ReportParameter()
        Dim reportFolder As String = String.Empty

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Dim filtriCopertura As FiltriCopertura = Me.GetFiltriCopertura()

            Dim dsMotiviEsclusione As dsMotiviEsclusione = genericProvider.Copertura.GetMotiviEsclusione(filtriCopertura)

            If dsMotiviEsclusione Is Nothing OrElse dsMotiviEsclusione.V_MOTIVI_ESCLUSIONE Is Nothing OrElse dsMotiviEsclusione.V_MOTIVI_ESCLUSIONE.Rows.Count = 0 Then
                Return Nothing
            End If

            rpt.set_dataset(dsMotiviEsclusione)

            ' Impostazione parametri report
            AddCommonReportParameter(rpt)
            rpt.AddParameter("TotaleAnagrafico", genericProvider.Copertura.GetTotaleAnagrafico(filtriCopertura))

            ' Recupero directory del report
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                reportFolder = bizReport.GetReportFolder(Constants.ReportName.MotiviInadempienti)
            End Using

        End Using

        Dim reportInfo As New ReportInfo()

        reportInfo.Filter = String.Empty
        reportInfo.Folder = reportFolder
        reportInfo.Name = Constants.ReportName.MotiviInadempienti
        reportInfo.ReportParameter = rpt

        Return reportInfo

    End Function

    Private Function StampaElencoNonVaccinati() As ReportInfo

        Dim rpt As New ReportParameter()
        Dim reportFolder As String = String.Empty

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Dim filtriCopertura As FiltriCopertura = Me.GetFiltriCopertura()

            Dim dsNonVaccinati As dsNonVaccinati = genericProvider.Copertura.GetElencoNonVaccinati(filtriCopertura, OnVacContext.CodiceUslCorrente)

            If dsNonVaccinati Is Nothing OrElse dsNonVaccinati.V_NON_VACCINATI Is Nothing OrElse dsNonVaccinati.V_NON_VACCINATI.Rows.Count = 0 Then
                Return Nothing
            End If

            rpt.set_dataset(dsNonVaccinati)

            ' Impostazione parametri report
            AddCommonReportParameter(rpt)
            rpt.AddParameter("TotaleAnagrafico", genericProvider.Copertura.GetTotaleAnagrafico(filtriCopertura))

            ' Recupero directory del report
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                reportFolder = bizReport.GetReportFolder(Constants.ReportName.CoperturaNonVaccinati)
            End Using

        End Using

        Dim reportInfo As New ReportInfo()

        reportInfo.Filter = String.Empty
        reportInfo.Folder = reportFolder
        reportInfo.Name = Constants.ReportName.CoperturaNonVaccinati
        reportInfo.ReportParameter = rpt

        Return reportInfo

    End Function

    Private Function StampaElencoVaccinati() As ReportInfo

        Dim rpt As New ReportParameter()
        Dim reportFolder As String = String.Empty

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Dim filtriCopertura As FiltriCopertura = Me.GetFiltriCopertura()

            Dim dsVaccinati As DsVaccinati = genericProvider.Copertura.GetElencoVaccinati(filtriCopertura, OnVacContext.CodiceUslCorrente)

            If dsVaccinati Is Nothing OrElse dsVaccinati.ELENCO_VACCINATI Is Nothing OrElse dsVaccinati.ELENCO_VACCINATI.Rows.Count = 0 Then
                Return Nothing
            End If

            rpt.set_dataset(dsVaccinati)

            ' Impostazione parametri report
            AddCommonReportParameter(rpt)
            rpt.AddParameter("TotaleAnagrafico", genericProvider.Copertura.GetTotaleAnagrafico(filtriCopertura))

            ' Recupero directory del report
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                reportFolder = bizReport.GetReportFolder(Constants.ReportName.CoperturaVaccinati)
            End Using

        End Using

        Dim reportInfo As New ReportInfo()

        reportInfo.Filter = String.Empty
        reportInfo.Folder = reportFolder
        reportInfo.Name = Constants.ReportName.CoperturaVaccinati
        reportInfo.ReportParameter = rpt

        Return reportInfo

    End Function

#End Region

#End Region

End Class
