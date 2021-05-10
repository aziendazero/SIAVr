Imports System.Collections.Generic
Imports System.Linq
Imports Onit.Database.DataAccessManager
Imports Onit.Controls

Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.Filters

Partial Class StatCoperturaVaccinaleMedico
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

#End Region

#Region " Types "

    Private Class ReportInfo

        Public ReportParameter As ReportParameter
        Public Name As String
        Public Folder As String
        Public Filter As String

    End Class

    Public Enum ReportResultError
        NONE
        NO_DATA
        NOT_INCLUDED
        EXCEPTION
    End Enum

    Private Class ReportResult

        Public Name As String
        Public Message As String

        Public ErrorType As ReportResultError

    End Class

#End Region

#Region " Eventi page "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            Me.PanelTitolo.InnerText = OnVacUtility.Variabili.CNS.Descrizione

            Me.odpDataNascitaIniz.Text = String.Empty
            Me.odpDataNascitaFin.Text = String.Empty

            Me.CaricaStatiAnagrafici()
			Me.CaricaVaccinazioni()
			ucSelezioneConsultori.MostraSoloAperti = False
			ucSelezioneConsultori.ImpostaCnsCorrente = False
			ucSelezioneConsultori.LoadGetCodici()
			' Nasconde i pulsanti di stampa in base alla presenza del report nell'installazione specifica
			Me.ShowPrintButtons()


            If Me.UtenteLoggatoIsMedico Then

                Using dam As IDAM = OnVacUtility.OpenDam()
                    Dim filtro As String = "(MED_CODICE = MAP_MED_CODICE_MEDICO AND MAP_DATA_INIZIO <= {0} AND MAP_DATA_FINE >= {0} AND MAP_MED_CODICE_ABILITATO = '{1}') UNION SELECT MED_CODICE,MED_DESCRIZIONE FROM T_ANA_MEDICI WHERE MED_CODICE = '{1}' ORDER BY MED_DESCRIZIONE"
                    omlMedicoBase.Filtro = String.Format(filtro, dam.QB.FC.ToDate(DateTime.Now.Date), Me.CodiceMedicoUtenteLoggato)
                End Using

                omlMedicoBase.Obbligatorio = True
                omlMedicoBase.Codice = Me.CodiceMedicoUtenteLoggato
                omlMedicoBase.RefreshDataBind()
            Else
                omlMedicoBase.Obbligatorio = False
                omlMedicoBase.Filtro = "1=1 ORDER BY MED_DESCRIZIONE"
            End If

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

        chklStatoAnagrafico.DataValueField = "SAN_CODICE"
        chklStatoAnagrafico.DataTextField = "SAN_DESCRIZIONE"
        chklStatoAnagrafico.DataSource = dtStatiAnag
        chklStatoAnagrafico.DataBind()

    End Sub

    Private Sub CaricaVaccinazioni()

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            rptVaccinazioni.DataSource = genericProvider.AnaVaccinazioni.GetVaccinazioni(chklModVaccinazione.SelectedValues)
            rptVaccinazioni.DataBind()

        End Using

    End Sub

#End Region

#Region " Eventi toolbar "

    Private Sub Toolbar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked

        ' Controllo valorizzazione date di nascita (anche lato client con alert)
        If Me.odpDataNascitaIniz.Text = String.Empty Or Me.odpDataNascitaFin.Text = String.Empty Then
            Me.OnitLayout31.InsertRoutineJS(String.Format("alert('{0}');", Me.ApplyEscapeJS("Date di nascita non impostate")))
            Return
        End If

        ' Controllo valorizzazione date di effettuazione (anche lato client con alert)
        If (odpDataEffettuazioneIniz.Text <> String.Empty And odpDataEffettuazioneFin.Text = String.Empty) Or (odpDataEffettuazioneIniz.Text = String.Empty And odpDataEffettuazioneFin.Text <> String.Empty) Then
            OnitLayout31.InsertRoutineJS(String.Format("alert('{0}');", Me.ApplyEscapeJS("Valorizzare sia la data di inizio sia di fine effettuazione per impostare il filtro. Impossibile stampare il report.")))
            Return
        End If

        If Me.UtenteLoggatoIsMedico AndAlso String.IsNullOrEmpty(omlMedicoBase.Codice) Then
            Me.OnitLayout31.InsertRoutineJS(String.Format("alert('{0}');", Me.ApplyEscapeJS("Il filtro sul medico non è stato impostato.")))
            Return
        End If

        Try

            ' Recupero dati per il report
            Dim reportInfo As ReportInfo = Nothing
            Dim reportResult As ReportResult = Nothing

            Select Case e.Button.Key

                Case "btnStampa"

                    reportResult = Me.StampaCoperturaVaccinazione()

                Case "btnElencoNonVaccinatiPaziente"

                    reportResult = Me.StampaElencoNonVaccinatiPaziente()

                Case "btnElencoNonVaccinati"

                    reportResult = Me.StampaElencoNonVaccinati()

                Case "btnElencoVaccinati"

                    reportResult = Me.StampaElencoVaccinati()

            End Select

            Select Case reportResult.ErrorType

                Case ReportResultError.NONE

                    Me.MultiView.SetActiveView(Me.ViewReport)

                Case ReportResultError.NO_DATA

                    Me.OnitLayout31.InsertRoutineJS("alert(""Stampa non effettuata: nessun dato da stampare in base ai filtri impostati."");")

                Case ReportResultError.NOT_INCLUDED

                    OnVacUtility.StampaNonPresente(Page, reportResult.Name)

                Case ReportResultError.EXCEPTION

                    Me.OnitLayout31.InsertRoutineJS(String.Format("alert('{0}');", Me.ApplyEscapeJS(reportResult.Message)))

            End Select

        Catch ex As Exception
            Me.OnitLayout31.InsertRoutineJS(String.Format("alert('{0}');", Me.ApplyEscapeJS(ex.Message)))
        End Try

    End Sub

#End Region

#Region " Eventi finestre modali "



	'se è valorizzato il distretto, deve disabilitare il consultorio [modifica 06/07/2005]
	Private Sub fmDistretto_Change(ByVal Sender As Object, ByVal E As Onit.Controls.OnitModalList.ModalListaEventArgument) Handles fmDistretto.Change
		'OnVacUtility.DisabilitaModale(fmConsultorio, IIf(fmDistretto.Codice <> "" And fmDistretto.Descrizione <> "", True, False))
		ucSelezioneConsultori.MostraCnsUtente = True
		ucSelezioneConsultori.MostraSoloAperti = False
		ucSelezioneConsultori.ImpostaCnsCorrente = True
		ucSelezioneConsultori.FiltroDistretto = fmDistretto.Codice
		ucSelezioneConsultori.LoadGetCodici()
	End Sub

    Private Sub fmCircoscrizione_Change(ByVal Sender As Object, ByVal E As Onit.Controls.OnitModalList.ModalListaEventArgument) Handles fmCircoscrizione.Change
        OnVacUtility.DisabilitaModale(fmComuneRes, IIf(fmCircoscrizione.Codice <> "" And fmCircoscrizione.Descrizione <> "", True, False))
    End Sub

    Private Sub fmComuneRes_Change(ByVal Sender As Object, ByVal E As Onit.Controls.OnitModalList.ModalListaEventArgument) Handles fmComuneRes.Change
        OnVacUtility.DisabilitaModale(fmCircoscrizione, IIf(fmComuneRes.Codice <> "" And fmComuneRes.Descrizione <> "", True, False))
    End Sub

#End Region

#Region " Eventi checkbox "

    Private Sub chklModVaccinazione_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles chklModVaccinazione.SelectedIndexChanged

        Me.CaricaVaccinazioni()

        'If Me.chklModVaccinazione.SelectedItems.Count = 0 Then
        '    Me.chkVaccinazioni.UnselectAll()
        'Else
        '    Me.chkVaccinazioni.SelectAll()
        'End If

    End Sub

#End Region

#Region " Private "

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()

        listPrintButtons.Add(New PrintButton(Constants.ReportName.CoperturaVaccinaleMedico, "btnStampa"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.ElencoVaccinatiMedico, "btnElencoVaccinati"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.ElencoNonVaccinatiMedico, "btnElencoNonVaccinati"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.ElencoNonVaccinatiPazienteMedico, "btnElencoNonVaccinatiPaziente"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Me.Toolbar)

    End Sub

    Private Function RicavaConsultoriDistretto() As List(Of String)

        Dim result As New List(Of String)()

        Using dam As IDAM = OnVacUtility.OpenDam()

            With dam.QB
                .NewQuery()
                .AddTables("T_ANA_DISTRETTI, T_ANA_CONSULTORI")
                .AddSelectFields("CNS_CODICE")
                .AddWhereCondition("DIS_CODICE", Comparatori.Uguale, Me.fmDistretto.Codice, DataTypes.Stringa)
                .AddWhereCondition("CNS_DIS_CODICE", Comparatori.Uguale, "DIS_CODICE", DataTypes.Join)
            End With

            Using dr As IDataReader = dam.BuildDataReader()
                While dr.Read()
                    result.Add(dr("CNS_CODICE"))
                End While
            End Using

        End Using

        Return result

    End Function

    Private Function GetFiltriCopertura() As FiltriCoperturaMedico

        Dim filtriCopertura As New FiltriCoperturaMedico()

        For i As Integer = 0 To rptVaccinazioni.Items.Count - 1

            Dim lblVaccinazioneCodice As Label = rptVaccinazioni.Items(i).FindControl("lblVaccinazioneCodice")
            Dim chkVaccinazione As CheckBox = rptVaccinazioni.Items(i).FindControl("chkVaccinazione")
            Dim txtNumeroDosi As TextBox = rptVaccinazioni.Items(i).FindControl("txtNumeroDosi")
            Dim txtGiorniVita As TextBox = rptVaccinazioni.Items(i).FindControl("txtGiorniVita")

            If chkVaccinazione.Checked Then

                If Not IsNumeric(txtNumeroDosi.Text) Then
                    Throw New ApplicationException(String.Format("Non è stato impostato un filtro N. dosi corretto sulla vaccinazione {0}.", lblVaccinazioneCodice.Text))
                End If

                If Not IsNumeric(txtGiorniVita.Text) Then
                    Throw New ApplicationException(String.Format("Non è stato impostato un filtro GG vita corretto sulla vaccinazione {0}.", lblVaccinazioneCodice.Text))
                End If

                Dim vac As New FiltriCoperturaMedico.FiltroVaccinazione()
                vac.codice = lblVaccinazioneCodice.Text
                vac.descrizione = chkVaccinazione.Text
                vac.richiamo = Convert.ToInt32(txtNumeroDosi.Text)
                vac.giorniVita = Convert.ToInt32(txtGiorniVita.Text)
                filtriCopertura.Vaccinazioni.Add(vac)

            End If

        Next

        If filtriCopertura.Vaccinazioni.Count = 0 Then
            Throw New ApplicationException("Impossibile stampare il report. Selezionare almeno una vaccinazione.")
        End If

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

        If Not String.IsNullOrEmpty(Me.omlMedicoBase.Codice) Then filtriCopertura.codiceMedico = Me.omlMedicoBase.Codice
        filtriCopertura.tipoMedico = Me.chkTipoMedico.SelectedValues

        Return filtriCopertura

    End Function

    Private Sub AddCommonReportParameter(rpt As Telerik.Reporting.Report)

        Me.SetReportParameter(rpt, "DataNascita1", Me.odpDataNascitaIniz.Text)
        Me.SetReportParameter(rpt, "DataNascita2", Me.odpDataNascitaFin.Text)


        Me.SetReportParameter(rpt, "DataEffettuazioneIniz", Me.odpDataEffettuazioneIniz.Text)
        Me.SetReportParameter(rpt, "DataEffettuazioneFin", Me.odpDataEffettuazioneFin.Text)

        If (Me.chklSesso.Items(0).Selected = True And Me.chklSesso.Items(1).Selected = False) Then
            Me.SetReportParameter(rpt, "Sesso", "1")
        ElseIf (Me.chklSesso.Items(0).Selected = False And Me.chklSesso.Items(1).Selected = True) Then
            Me.SetReportParameter(rpt, "Sesso", "2")
        Else
            Me.SetReportParameter(rpt, "Sesso", "3")
        End If

        If Me.fmComuneRes.Codice <> "" And Me.fmComuneRes.Descrizione <> "" Then
            Me.SetReportParameter(rpt, "ComRes", Me.fmComuneRes.Descrizione)
        Else
            Me.SetReportParameter(rpt, "ComRes", "TUTTI")
        End If

        If Me.fmCircoscrizione.Codice <> "" And Me.fmCircoscrizione.Descrizione <> "" Then
            Me.SetReportParameter(rpt, "Circoscriz", Me.fmCircoscrizione.Descrizione)
        Else
            Me.SetReportParameter(rpt, "Circoscriz", "TUTTE")
        End If

		If ucSelezioneConsultori.GetConsultoriSelezionati.Count > 0 Then
			'If Me.fmDistretto.Codice <> "" And Me.fmDistretto.Descrizione <> "" Then
			'	Me.SetReportParameter(rpt, "Consultorio", Me.fmDistretto.Descrizione)
			'Else
			'	Me.SetReportParameter(rpt, "Consultorio", Me.fmConsultorio.Descrizione)
			'End If
			Me.SetReportParameter(rpt, "Consultorio", ucSelezioneConsultori.GetDescrizioneConsultoriSelezionatiPerStampe())

		Else
			Me.SetReportParameter(rpt, "Consultorio", "TUTTI")
        End If

        If Me.fmDistretto.Codice <> "" And Me.fmDistretto.Descrizione <> "" Then
            Me.SetReportParameter(rpt, "Distretto", Me.fmDistretto.Descrizione)
        Else
            Me.SetReportParameter(rpt, "Distretto", "TUTTI")
        End If

        Dim sAnagrafico As String = String.Empty
        If Me.chklStatoAnagrafico.SelectedValues.Count > 0 Then
            sAnagrafico = Me.chklStatoAnagrafico.SelectedItems.Select(Function(p) p.Text).Aggregate(Function(p, g) p & ", " & g)
        End If
        Me.SetReportParameter(rpt, "SAnagrafico", sAnagrafico)

        If Me.chklModVaccinazione.SelectedValues.Count > 0 Then
            Dim s As String = Me.chklModVaccinazione.SelectedItems.Select(Function(p) p.Text.ToUpper()).Aggregate(Function(p, g) p & ", " & g)
            Me.SetReportParameter(rpt, "TipoVac", s)
        Else
            Me.SetReportParameter(rpt, "TipoVac", "OBBLIGATORIE, RACCOMANDATE, FACOLTATIVE")
        End If

    End Sub

#Region " Stampe "

    Private Sub SetReportParameter(report As Telerik.Reporting.IReportDocument, pName As String, pValue As String)

        Dim param As Telerik.Reporting.ReportParameter = Enumerable.FirstOrDefault(Of Telerik.Reporting.ReportParameter)((From p In report.ReportParameters
                                                                                                                          Where (String.Compare(p.Name, pName, True) = 0)
                                                                                                                          Select p))

        If (Not param Is Nothing) Then
            Select Case param.Type
                Case Telerik.Reporting.ReportParameterType.Boolean
                    param.Value = Boolean.Parse(pValue)
                    Return
                Case Telerik.Reporting.ReportParameterType.DateTime
                    param.Value = DateTime.Parse(pValue)
                    Return
                Case Telerik.Reporting.ReportParameterType.Integer
                    param.Value = Integer.Parse(pValue)
                    Return
                Case Telerik.Reporting.ReportParameterType.Float
                    param.Value = Single.Parse(pValue)
                    Return
                    'Case Telerik.Reporting.ReportParameterType.String
                    '    param.Value = pValue
                    '    Return
            End Select
            param.Value = pValue
        End If
    End Sub

    Private Sub SetReportTableDataSource(report As Telerik.Reporting.Report, reportTableName As String, dataSource As Object, dataMember As String)

        Dim o As Object = report.Items.Find(reportTableName, True)
        Dim t As Telerik.Reporting.Table = DirectCast(DirectCast(o, Telerik.Reporting.ReportItemBase())(0), Telerik.Reporting.Table)
        t.DataSource = dataSource
        t.DataMember = dataMember

    End Sub

    Private Function StampaCoperturaVaccinazione() As ReportResult

        Dim reportResult As New ReportResult
        reportResult.Name = Constants.ReportName.CoperturaVaccinaleMedico

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Dim totaleAnagrafico As Integer = 0
            Dim filtriCopertura As FiltriCoperturaMedico = Me.GetFiltriCopertura()
            Dim dsCoperturaVaccinale As dsCoperturaVaccinaleMedico = genericProvider.Copertura.GetCoperturaVaccinaleMedico(filtriCopertura)

            If dsCoperturaVaccinale Is Nothing OrElse dsCoperturaVaccinale.CoperturaVaccinale Is Nothing OrElse dsCoperturaVaccinale.CoperturaVaccinale.Rows.Count = 0 Then

                reportResult.ErrorType = ReportResultError.NO_DATA
                Return reportResult
            Else

                totaleAnagrafico = genericProvider.Copertura.GetTotaleAnagraficoMedico(filtriCopertura)

            End If

            Dim report As Telerik.Reporting.Report = Me.GetReportInstance(genericProvider, reportResult.Name)

            ' Creating and configuring the ObjectDataSource component:
            Dim objectDataSource As Telerik.Reporting.ObjectDataSource = New Telerik.Reporting.ObjectDataSource()
            objectDataSource.DataSource = dsCoperturaVaccinale
            objectDataSource.DataMember = "CoperturaVaccinale"

            ' Impostazione parametri report
            AddCommonReportParameter(report)
            Me.SetReportParameter(report, "TotaleAnagrafico", totaleAnagrafico)
            Me.SetReportParameter(report, "dettaglioCoperturaVisibile", Me.Settings.GES_CALCOLO_COPERTURA)

            report.DataSource = objectDataSource

            Me.SetReportTableDataSource(report, "tableFiltriVaccinazioni", dsCoperturaVaccinale, "FiltriVaccinazioni")


            Me.ReportViewer.Report = report

        End Using

        Return reportResult

    End Function

    Private Function StampaElencoNonVaccinati() As ReportResult

        Dim reportResult As New ReportResult
        reportResult.Name = Constants.ReportName.ElencoNonVaccinatiMedico

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Dim totaleAnagrafico As Integer = 0
            Dim filtriCopertura As FiltriCoperturaMedico = Me.GetFiltriCopertura()
            Dim dsNonVaccinatiMedico As dsNonVaccinatiMedico = genericProvider.Copertura.GetNonVaccinatiMedico(filtriCopertura, OnVacContext.CodiceUslCorrente)

            If dsNonVaccinatiMedico Is Nothing OrElse dsNonVaccinatiMedico.Medici Is Nothing OrElse dsNonVaccinatiMedico.Medici.Rows.Count = 0 Then

                reportResult.ErrorType = ReportResultError.NO_DATA
                Return reportResult
            Else

                For Each row As dsNonVaccinatiMedico.MediciRow In dsNonVaccinatiMedico.Medici.Rows

                    Dim qry As IEnumerable(Of Int64) = (From p In dsNonVaccinatiMedico.ElencoNonVaccinati.AsEnumerable()
                               Where p.Field(Of String)("PAZ_MED_CODICE_BASE") = row.MED_CODICE And p.Field(Of String)("VAC_CODICE") = row.vac_codice
                               Select p.Field(Of Int64)("PAZ_CODICE")).Distinct()
                    row.NUM_NON_VACCINATI = qry.Count()

                Next

            End If

            Dim report As Telerik.Reporting.Report = Me.GetReportInstance(genericProvider, reportResult.Name)

            ' Creating and configuring the ObjectDataSource component:
            Dim objectDataSource As Telerik.Reporting.ObjectDataSource = New Telerik.Reporting.ObjectDataSource()
            objectDataSource.DataSource = dsNonVaccinatiMedico
            objectDataSource.DataMember = "Medici"

            ' Impostazione parametri report
            AddCommonReportParameter(report)

            report.DataSource = objectDataSource

            ' TODO creare classe wrapper per Telerik.Reporting.Report con metodo GetTable(string tableName)
            Me.SetReportTableDataSource(report, "tableDettaglio", dsNonVaccinatiMedico, "ElencoNonVaccinati")
            Me.SetReportTableDataSource(report, "tableFiltriVaccinazioni", dsNonVaccinatiMedico, "FiltriVaccinazioni")


            Me.ReportViewer.Report = report

        End Using

        Return reportResult

    End Function

    Private Function StampaElencoNonVaccinatiPaziente() As ReportResult

        Dim reportResult As New ReportResult
        reportResult.Name = Constants.ReportName.ElencoNonVaccinatiPazienteMedico

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Dim totaleAnagrafico As Integer = 0
            Dim filtriCopertura As FiltriCoperturaMedico = Me.GetFiltriCopertura()
            Dim dsNonVaccinatiMedico As dsNonVaccinatiMedico = genericProvider.Copertura.GetNonVaccinatiMedico(filtriCopertura, OnVacContext.CodiceUslCorrente)

            If dsNonVaccinatiMedico Is Nothing OrElse dsNonVaccinatiMedico.Medici Is Nothing OrElse dsNonVaccinatiMedico.Medici.Rows.Count = 0 Then

                reportResult.ErrorType = ReportResultError.NO_DATA
                Return reportResult
            Else

                For Each row As dsNonVaccinatiMedico.MediciRow In dsNonVaccinatiMedico.Medici.Rows

                    Dim qry As IEnumerable(Of Int64) = (From p In dsNonVaccinatiMedico.ElencoNonVaccinati.AsEnumerable()
                               Where p.Field(Of String)("PAZ_MED_CODICE_BASE") = row.MED_CODICE
                               Select p.Field(Of Int64)("PAZ_CODICE")).Distinct()
                    row.NUM_NON_VACCINATI = qry.Count()

                Next

            End If

            Dim report As Telerik.Reporting.Report = Me.GetReportInstance(genericProvider, reportResult.Name)

            ' Creating and configuring the ObjectDataSource component:
            Dim objectDataSource As Telerik.Reporting.ObjectDataSource = New Telerik.Reporting.ObjectDataSource()
            objectDataSource.DataSource = dsNonVaccinatiMedico
            objectDataSource.DataMember = "Medici"

            ' Impostazione parametri report
            AddCommonReportParameter(report)
            Me.SetReportParameter(report, "deslib1", Me.Settings.DESLIB1)

            report.DataSource = objectDataSource

            ' TODO creare classe wrapper per Telerik.Reporting.Report con metodo GetTable(string tableName)
            Me.SetReportTableDataSource(report, "tableDettaglio", dsNonVaccinatiMedico, "ElencoNonVaccinati")
            Me.SetReportTableDataSource(report, "tableFiltriVaccinazioni", dsNonVaccinatiMedico, "FiltriVaccinazioni")


            Me.ReportViewer.Report = report

        End Using

        Return reportResult

    End Function

    Private Function StampaElencoVaccinati() As ReportResult

        Dim reportResult As New ReportResult
        reportResult.Name = Constants.ReportName.ElencoVaccinatiMedico

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Dim totaleAnagrafico As Integer = 0
            Dim filtriCopertura As FiltriCoperturaMedico = Me.GetFiltriCopertura()
            Dim dsVaccinatiMedico As dsVaccinatiMedico = genericProvider.Copertura.GetVaccinatiMedico(filtriCopertura, OnVacContext.CodiceUslCorrente)

            If dsVaccinatiMedico Is Nothing OrElse dsVaccinatiMedico.Medici Is Nothing OrElse dsVaccinatiMedico.Medici.Rows.Count = 0 Then

                reportResult.ErrorType = ReportResultError.NO_DATA
                Return reportResult
            Else

                For Each row As dsVaccinatiMedico.MediciRow In dsVaccinatiMedico.Medici.Rows

                    Dim qry As IEnumerable(Of Int64) = (From p In dsVaccinatiMedico.ElencoVaccinati.AsEnumerable()
                               Where p.Field(Of String)("PAZ_MED_CODICE_BASE") = row.MED_CODICE And p.Field(Of String)("VAC_CODICE") = row.vac_codice
                               Select p.Field(Of Int64)("PAZ_CODICE")).Distinct()
                    row.NUM_NON_VACCINATI = qry.Count()

                Next

            End If

            Dim report As Telerik.Reporting.Report = Me.GetReportInstance(genericProvider, reportResult.Name)

            ' Creating and configuring the ObjectDataSource component:
            Dim objectDataSource As Telerik.Reporting.ObjectDataSource = New Telerik.Reporting.ObjectDataSource()
            objectDataSource.DataSource = dsVaccinatiMedico
            objectDataSource.DataMember = "Medici"

            ' Impostazione parametri report
            AddCommonReportParameter(report)
            Me.SetReportParameter(report, "deslib1", Me.Settings.DESLIB1)

            report.DataSource = objectDataSource

            ' TODO creare classe wrapper per Telerik.Reporting.Report con metodo GetTable(string tableName)
            Me.SetReportTableDataSource(report, "tableDettaglio", dsVaccinatiMedico, "ElencoVaccinati")
            Me.SetReportTableDataSource(report, "tableFiltriVaccinazioni", dsVaccinatiMedico, "FiltriVaccinazioni")


            Me.ReportViewer.Report = report

        End Using

        Return reportResult

    End Function

#End Region

#End Region

    Protected Sub btnCloseViewReport_Click(sender As Object, e As EventArgs)
        Me.MultiView.SetActiveView(Me.ViewFiltri)
    End Sub
End Class
