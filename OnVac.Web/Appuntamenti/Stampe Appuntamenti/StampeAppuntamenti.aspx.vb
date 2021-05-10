Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Biz
Imports Onit.OnAssistnet.OnVac.DataSet


Partial Class StampeAppuntamenti
    Inherits OnVac.Common.PageBase

#Region " Types "

    'tipo di stampa generata [modifica 11/04/2005]
    Private Enum TipoFile
        Avviso
        Bilancio
    End Enum

    ' ----------------------------------------------------------------------------------------------------------- '
    ' TipoStampa => Constants.TipoStampaAppuntamento
    ' ----------------------------------------------------------------------------------------------------------- '
    '   Value="A"       >Avvisi<                                Constants.ReportName.AvvisoAppuntamento
    '	Value="EA"      >Elenco Avvisi<                         Constants.ReportName.AppuntamentiDelGiorno
    '	Value="ETA"     >Etichette Avvisi<                      Constants.ReportName.EtichetteAvvisoAppuntamento
    '	Value="B"       >Bilanci<                               Constants.ReportName.AvvisoSoloBilancio
    '	Value="EB"      >Elenco Bilanci<                        Constants.ReportName.ElencoSoloBilanci
    '	Value="BM"      >Bilanci per Malattia cronica<          Constants.ReportName.BilanciMalattia
    '   Value="CA"      >Avvisi Campagna Adulti<                Constants.ReportName.AvvisoCampagnaAdulti
    '   Value="EBM"     >Elenco bilanci per malattia cronica<   Constants.ReportName.ElencoBilanciMalattia
    '   Value="EAA"     >Etichette assisititi<                  Constants.ReportName.EtichetteAssisititiAvvisi
    ' ----------------------------------------------------------------------------------------------------------- '

#End Region

#Region " Properties "

    Private Property FiltriMaschera() As FiltriAssociazioniDosi
        Get
            If ViewState("FCC") Is Nothing Then ViewState("FCC") = New FiltriAssociazioniDosi()
            Return ViewState("FCC")
        End Get
        Set(value As FiltriAssociazioniDosi)
            ViewState("FCC") = value
        End Set
    End Property

#End Region

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(sender As System.Object, e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo Ë richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Eventi pagina "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        'Pulizia e log della session
        Dim sc As New SessionCleaner(Me.Settings)
        sc.Start()

        ' Carica sempre le date delle ultime stampe, cosÏ vengono sempre aggiornate (anche dopo un HistoryBack)
        Me.CaricaDateUltimaStampa()

        If Not IsPostBack Then

            Me.CaricaMalattie()

            OnVacUtility.BindTipoSoggettiAvviso(Me.rdbFiltroSoggetti)

            ' Le note possono essere introdotte (e stampate) solo se il parametro corrispondente Ë true.
            Me.txtNoteAvviso.Enabled = Me.Settings.GES_NOTE_AVVISI

            ' Nella radiobuttonlist delle modalit‡ di stampa, 
            ' devono essere visualizzate solo le stampe presenti nell'installazione corrente
            Me.ShowOpzioniStampa()

            Me.ShowTipoAvvisoPostel()

        End If

    End Sub

#End Region

#Region " Eventi controlli "

    Private Sub Toolbar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked
        '--
        Dim strFiltro As String
        Dim tipoStampaSelezionata As String = String.Empty
        '--
        Select Case e.Button.Key
            '--
            Case "btnStampa"
                '--
                'a seconda della stampa scelta, va modificato anche il filtro
                '--
                tipoStampaSelezionata = optModalit‡Stampa.SelectedValue()
                '--
                ' Provider per accesso al db (utilizzato per accedere alla t_ana_report)
                '--
                Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                    '--
                    ' Info sul report AppuntamentiDelGiorno
                    '--
                    Using reportBiz As New BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                        '--
                        Select Case tipoStampaSelezionata
                        '--
                            Case Constants.TipoStampaAppuntamento.EtichetteAvvisi, Constants.TipoStampaAppuntamento.Avvisi, Constants.TipoStampaAppuntamento.CampagnaAdulti, Constants.TipoStampaAppuntamento.EtichetteAssistitiAvvisi
                                '--
                                Dim dataInizio As Date = Me.odpDataIniz.Data
                                Dim dataFine As Date = Me.odpDataFin.Data.AddDays(1)
                                '--
                                Dim sbFiltroAvvisi As New System.Text.StringBuilder()
                                '--                       
                                sbFiltroAvvisi.Append("(")
                                sbFiltroAvvisi.Append("(")
                                sbFiltroAvvisi.AppendFormat("{{V_AVVISI.CNV_DATA_APPUNTAMENTO}} >=  Date({0}, {1}, {2}) AND {{V_AVVISI.CNV_DATA_APPUNTAMENTO}} < Date({3}, {4}, {5})", dataInizio.Year, dataInizio.Month, dataInizio.Day, dataFine.Year, dataFine.Month, dataFine.Day)
                                sbFiltroAvvisi.Append(")")
                                sbFiltroAvvisi.Append(" OR ")
                                sbFiltroAvvisi.Append("(")
                                sbFiltroAvvisi.Append("IsNull({V_AVVISI.CNV_DATA_INVIO}) AND {V_AVVISI.CNV_DATA_APPUNTAMENTO} = Date(2100, 1, 1) AND {V_AVVISI.SOLLECITO_SEDUTA_CICLO} > {V_AVVISI.NUM_SOLLECITI}  AND {V_AVVISI.SEDUTA_CICLO_OBBLIGATORIA} = 'S'")
                                sbFiltroAvvisi.Append(")")
                                sbFiltroAvvisi.Append(")")
                                '--
                                Select Case Me.GetFiltroPazientiAvvisati()
                                    Case Enumerators.FiltroAvvisati.SoloAvvisati
                                        sbFiltroAvvisi.Append(" AND Not IsNull({V_AVVISI.CNV_DATA_INVIO})")
                                    Case Enumerators.FiltroAvvisati.SoloNonAvvisati
                                        sbFiltroAvvisi.Append(" AND IsNull({V_AVVISI.CNV_DATA_INVIO})")
                                End Select
                                '--
                                If Me.uscScegliAmb.cnsCodice <> "" And Me.uscScegliAmb.cnsDescrizione <> "" Then
                                    sbFiltroAvvisi.AppendFormat(" AND {{V_AVVISI.CNV_CNS_CODICE}}='{0}'", Me.uscScegliAmb.cnsCodice)
                                End If
                                If Me.uscScegliAmb.ambCodice <> 0 Then
                                    sbFiltroAvvisi.AppendFormat(" AND {{V_AVVISI.CNV_AMB_CODICE}}= {0} ", Me.uscScegliAmb.ambCodice)
                                End If
                                '--
                                sbFiltroAvvisi.Append(GetFiltroReport_AssociazioniDosi())
                                '--
                                Select Case tipoStampaSelezionata
                                '--
                                    Case Constants.TipoStampaAppuntamento.EtichetteAvvisi
                                        '--
                                        'Stampa Etichette per avvisi appuntamenti
                                        '--
                                        Dim rpt As New ReportParameter()
                                        '--
                                        If Not OnVacReport.StampaReport(Constants.ReportName.EtichetteAvvisoAppuntamento, sbFiltroAvvisi.ToString(), rpt, Nothing, Nothing, reportBiz.GetReportFolder(Constants.ReportName.EtichetteAvvisoAppuntamento)) Then
                                            OnVacUtility.StampaNonPresente(Page, Constants.ReportName.EtichetteAvvisoAppuntamento)
                                        End If
                                    '--
                                    Case Constants.TipoStampaAppuntamento.EtichetteAssistitiAvvisi
                                        '--
                                        'Stampa Etichette assisititi
                                        '--
                                        Dim rpt As New ReportParameter()
                                        '--
                                        If Not OnVacReport.StampaReport(Constants.ReportName.EtichetteAssistitiAvvisi, sbFiltroAvvisi.ToString(), rpt, Nothing, Nothing, reportBiz.GetReportFolder(Constants.ReportName.EtichetteAssistitiAvvisi)) Then
                                            OnVacUtility.StampaNonPresente(Page, Constants.ReportName.EtichetteAssistitiAvvisi)
                                        End If
                                    '--
                                    Case Constants.TipoStampaAppuntamento.Avvisi, Constants.TipoStampaAppuntamento.CampagnaAdulti
                                        '--
                                        'l'avviso per la campagna adulti deve recuperare gli stessi dati dell'avviso classico
                                        '--
                                        Dim rpt As New ReportParameter()

                                        If Settings.GES_NOTE_AVVISI Then
                                            rpt.AddParameter("NoteAvviso", Me.txtNoteAvviso.Text)
                                        Else
                                            rpt.AddParameter("NoteAvviso", String.Empty)
                                        End If

                                        Dim stampaReport As Boolean = True
                                        Dim nomeReport As String = String.Empty
                                        '--
                                        Select Case tipoStampaSelezionata
                                        '--
                                            Case Constants.TipoStampaAppuntamento.Avvisi
                                                '--
                                                'stampa il pdf solo se non Ë spuntato il check di export del tracciato postel
                                                '--
                                                stampaReport = Not Me.chkExportPostel.Checked
                                                nomeReport = Constants.ReportName.AvvisoAppuntamento
                                            '--
                                            Case Constants.TipoStampaAppuntamento.CampagnaAdulti
                                                '--
                                                stampaReport = True
                                                nomeReport = Constants.ReportName.AvvisoCampagnaAdulti
                                                '--
                                        End Select
                                        '--
                                        If stampaReport Then

                                            If OnVacReport.StampaReport(nomeReport, sbFiltroAvvisi.ToString(), rpt, Nothing, Nothing, reportBiz.GetReportFolder(nomeReport)) Then
                                                '--
                                                ' Salvataggio data invio (nel caso del postel, viene effettuata nell'handler dopo l'esportazione)
                                                SalvaDateConsultorio(tipoStampaSelezionata)
                                                '--
                                                ' Salvataggio nome del file per la stampa dell'ultimo avviso
                                                SalvaFileUltimaStampa(rpt.ReportFileName, TipoFile.Avviso)
                                                '--
                                            Else
                                                '--
                                                OnVacUtility.StampaNonPresente(Page, nomeReport)
                                                '--
                                            End If

                                        Else

                                            ' Export postel se selezionato dall'utente
                                            Dim command As New OnVacUtility.OpenPostelHandlerCommand()
                                            command.CodiceConsultorio = Me.uscScegliAmb.cnsCodice
                                            command.CodiceAmbulatorio = Me.uscScegliAmb.ambCodice
                                            command.DataInizioAppuntamenti = Me.odpDataIniz.Data
                                            command.DataFineAppuntamenti = Me.odpDataFin.Data
                                            command.FiltroPazientiAvvisati = GetFiltroPazientiAvvisati()
                                            command.TipoAvviso = Me.rblExportPostel.SelectedValue
                                            command.ArgomentoExport = Me.Settings.EXPORT_POSTEL_ARGOMENTO
                                            command.CurrentPage = Me.Page
                                            command.FiltroAssociazioniDosi = GetFiltroAssociazioniDosi()

                                            OnVacUtility.OpenPostelHandler(command)

                                        End If

                                End Select
                            '--
                            Case Constants.TipoStampaAppuntamento.ElencoAvvisi
                                '--
                                Dim rptInfo As Entities.Report = reportBiz.GetReport(Constants.ReportName.AppuntamentiDelGiorno)
                                '--
                                If rptInfo Is Nothing Then
                                    Me.OnitLayout31.InsertRoutineJS("alert('Report non trovato');")
                                    Exit Sub
                                End If
                                '--
                                ' Definizione report e parametri
                                '--
                                Dim rpt As New ReportParameter()
                                rpt.AddParameter("OrarioAperturaPom", Settings.ORAPM)
                                '--
                                ' Filtro da passare al report (vecchia versione)
                                '--
                                strFiltro = String.Empty
                                '--
                                ' Filtro pazienti avvisati (nuova versione)
                                '--
                                Dim filtroPazAvvisati As OnVac.Enumerators.FiltroAvvisati = Me.GetFiltroPazientiAvvisati()
                                '--
                                ' VERSIONE PER REPORT CON BILANCI
                                '--
                                Dim dsAppuntamentiBil As AppuntamentiGiornoBilanci = Nothing
                                '--
                                Using bizApp As New BizAppuntamentiGiorno(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                                    dsAppuntamentiBil = bizApp.CreaDataSetAppuntamentiGiornoBilanci(
                                        Me.uscScegliAmb.cnsCodice, Me.uscScegliAmb.ambCodice, Me.odpDataIniz.Text, Me.odpDataFin.Text, filtroPazAvvisati, GetFiltroAssociazioniDosi())

                                End Using
                                '--
                                rpt.set_dataset(dsAppuntamentiBil)
                                '--
                                ' Parametro presente solo in questa versione del report
                                '--
                                Dim descrizioneCns As String = Me.uscScegliAmb.cnsDescrizione
                                If Me.uscScegliAmb.ambCodice <> 0 Then
                                    descrizioneCns = String.Format("{0} - {1}", Me.uscScegliAmb.cnsDescrizione, Me.uscScegliAmb.ambDescrizione)
                                End If
                                rpt.AddParameter("CnsDescrizione", descrizioneCns)
                                '--
                                If Not OnVacReport.StampaReport(rptInfo.Nome, strFiltro, rpt, Nothing, Nothing, rptInfo.Cartella) Then
                                    OnVacUtility.StampaNonPresente(Page, rptInfo.Nome)
                                End If
                            '--
                            Case Constants.TipoStampaAppuntamento.Bilanci
                                '---
                                'STAMPA DEL SOLO BILANCIO
                                '---
                                strFiltro = "DateDiff('d',{V_BILANCI.CNV_DATA_APPUNTAMENTO},DateSerial(" &
                                            odpDataIniz.Data.Year.ToString() & ", " &
                                            odpDataIniz.Data.Month.ToString() & ", " &
                                            odpDataIniz.Data.Day.ToString() & "))<=0"
                                '--
                                strFiltro &= " AND DateDiff('d',DateSerial(" &
                                             odpDataFin.Data.Year.ToString() & ", " &
                                             odpDataFin.Data.Month.ToString() & ", " &
                                             odpDataFin.Data.Day.ToString() & "),{V_BILANCI.CNV_DATA_APPUNTAMENTO})<=0"
                                '--
                                'Filtro sulla data di invio
                                '--
                                Select Case Me.GetFiltroPazientiAvvisati()
                                    Case Enumerators.FiltroAvvisati.SoloAvvisati
                                        strFiltro &= " AND NOT(ISNULL({V_BILANCI.CNV_DATA_INVIO}))"
                                    Case Enumerators.FiltroAvvisati.SoloNonAvvisati
                                        strFiltro &= " AND ISNULL({V_BILANCI.CNV_DATA_INVIO}) "
                                End Select
                                '--
                                'filtro sul consultorio 
                                '--
                                If Me.uscScegliAmb.cnsCodice <> "" And Me.uscScegliAmb.cnsDescrizione <> "" Then
                                    strFiltro &= " AND {V_BILANCI.CNV_CNS_CODICE}='" & Me.uscScegliAmb.cnsCodice & "' "
                                End If
                                If Me.uscScegliAmb.ambCodice <> 0 Then
                                    strFiltro &= " AND {V_BILANCI.CNV_AMB_CODICE}= " & Me.uscScegliAmb.ambCodice
                                End If
                                '--
                                'i bilanci 61 e 62 non devono comparire nella stampa
                                '--
                                strFiltro &= " AND ({V_BILANCI.BIL_OBBLIGATORIO} = 'S')"
                                '--
                                'stampa
                                '--
                                Dim rpt As New ReportParameter()
                                '--
                                If Not OnVacReport.StampaReport(Constants.ReportName.AvvisoSoloBilancio, strFiltro, rpt, Nothing, Nothing, reportBiz.GetReportFolder(Constants.ReportName.AvvisoSoloBilancio)) Then
                                    '--
                                    OnVacUtility.StampaNonPresente(Page, Constants.ReportName.AvvisoSoloBilancio)
                                    '--
                                Else
                                    '--
                                    'salvataggio data invio per solo bilancio
                                    '--
                                    SalvaDateConsultorio(Constants.TipoStampaAppuntamento.Bilanci)
                                    '--
                                    'salvataggio nome del file per la stampa dell'ultimo avviso
                                    '--
                                    SalvaFileUltimaStampa(rpt.ReportFileName, TipoFile.Bilancio)
                                    '--
                                End If
                            '--
                            Case Constants.TipoStampaAppuntamento.ElencoBilanci
                                '--
                                'bilanci con malattia
                                '--
                                strFiltro = "DateDiff('d',{V_BILANCI.CNV_DATA_APPUNTAMENTO},DateSerial(" &
                                            odpDataIniz.Data.Year.ToString() & ", " &
                                            odpDataIniz.Data.Month.ToString() & ", " &
                                            odpDataIniz.Data.Day.ToString() & "))<=0"
                                '--
                                strFiltro &= " AND DateDiff('d',DateSerial(" &
                                             odpDataFin.Data.Year.ToString() & ", " &
                                             odpDataFin.Data.Month.ToString() & ", " &
                                             odpDataFin.Data.Day.ToString() & "),{V_BILANCI.CNV_DATA_APPUNTAMENTO})<=0"
                                '--
                                Select Case Me.GetFiltroPazientiAvvisati()
                                    Case Enumerators.FiltroAvvisati.SoloAvvisati
                                        strFiltro &= " AND NOT(ISNULL({V_BILANCI.CNV_DATA_INVIO}))"
                                    Case Enumerators.FiltroAvvisati.SoloNonAvvisati
                                        strFiltro &= " AND ISNULL({V_BILANCI.CNV_DATA_INVIO})"
                                End Select
                                '--
                                If Me.uscScegliAmb.cnsCodice <> "" And Me.uscScegliAmb.cnsDescrizione <> "" Then
                                    strFiltro &= " AND {V_BILANCI.CNV_CNS_CODICE}='" & Me.uscScegliAmb.cnsCodice & "' "
                                End If
                                If Me.uscScegliAmb.ambCodice <> 0 Then
                                    strFiltro &= " AND {V_BILANCI.CNV_AMB_CODICE}= " & Me.uscScegliAmb.ambCodice
                                End If
                                '--
                                strFiltro &= " AND ({V_BILANCI.BIL_OBBLIGATORIO} = 'S')"
                                '--
                                Dim orarioAperturaPomeridiana As String = Me.Settings.ORAPM
                                '--
                                Dim rpt As New ReportParameter()
                                rpt.AddParameter("OrarioAperturaPom", orarioAperturaPomeridiana)
                                '--
                                'stampa
                                '--
                                If Not OnVacReport.StampaReport(Constants.ReportName.ElencoSoloBilanci, strFiltro, rpt, Nothing, Nothing, reportBiz.GetReportFolder(Constants.ReportName.ElencoSoloBilanci)) Then
                                    OnVacUtility.StampaNonPresente(Page, Constants.ReportName.ElencoSoloBilanci)
                                End If
                            '--
                            Case Constants.TipoStampaAppuntamento.ElencoBilanciMalattia
                                '--
                                Dim rptInfo As Entities.Report = reportBiz.GetReport(Constants.ReportName.ElencoBilanciMalattia)
                                '--
                                If rptInfo Is Nothing Then
                                    Me.OnitLayout31.InsertRoutineJS("alert('Report non trovato');")
                                    Exit Sub
                                End If
                                '--
                                ' Definizione report e parametri
                                '--
                                Dim orarioAperturaPomeridiana As String = Me.Settings.ORAPM
                                '--
                                Dim rpt As New ReportParameter()
                                rpt.AddParameter("OrarioAperturaPom", orarioAperturaPomeridiana)
                                '--
                                ' Filtro pazienti avvisati 
                                '--
                                Dim filtroPazAvvisati As OnVac.Enumerators.FiltroAvvisati = GetFiltroPazientiAvvisati()
                                '--
                                Dim elencoBilanciBiz As New BizElencoBilanci(genericProvider)
                                Dim dsElencoBilanci As New ElencoBilanciDS()
                                elencoBilanciBiz.fillDtElencoBilanci(dsElencoBilanci.elencoBilanci, Me.uscScegliAmb.cnsCodice, Me.uscScegliAmb.ambCodice,
                                                                     Me.odpDataIniz.Text, Me.odpDataFin.Text, filtroPazAvvisati, Me.cmbMalCronica.SelectedValue(), OnVacContext.CodiceUslCorrente)
                                '--
                                rpt.set_dataset(dsElencoBilanci)
                                '--
                                'stampa
                                '--
                                If Not OnVacReport.StampaReport(Constants.ReportName.ElencoBilanciMalattia, "", rpt, Nothing, Nothing, reportBiz.GetReportFolder(Constants.ReportName.ElencoBilanciMalattia)) Then
                                    OnVacUtility.StampaNonPresente(Page, Constants.ReportName.ElencoBilanciMalattia)
                                End If
                            '--
                            Case Constants.TipoStampaAppuntamento.BilanciMalattia
                                '--
                                'bilanci con malattia
                                '--
                                strFiltro = "DateDiff('d',{V_BILANCI_MALATTIA.CNV_DATA_APPUNTAMENTO},DateSerial(" &
                                            Me.odpDataIniz.Data.Year.ToString() & ", " &
                                            Me.odpDataIniz.Data.Month.ToString() & ", " &
                                            Me.odpDataIniz.Data.Day.ToString() & "))<=0"
                                '--
                                strFiltro &= " AND DateDiff('d',DateSerial(" &
                                             Me.odpDataFin.Data.Year.ToString() & ", " &
                                             Me.odpDataFin.Data.Month.ToString() & ", " &
                                             Me.odpDataFin.Data.Day.ToString() & "),{V_BILANCI_MALATTIA.CNV_DATA_APPUNTAMENTO})<=0"
                                '--
                                Select Case Me.GetFiltroPazientiAvvisati()
                                    Case Enumerators.FiltroAvvisati.SoloAvvisati
                                        strFiltro &= " AND NOT(ISNULL({V_BILANCI_MALATTIA.CNV_DATA_INVIO}))"
                                    Case Enumerators.FiltroAvvisati.SoloNonAvvisati
                                        strFiltro &= " AND ISNULL({V_BILANCI_MALATTIA.CNV_DATA_INVIO})"
                                End Select
                                '--
                                If Me.uscScegliAmb.cnsCodice <> "" And Me.uscScegliAmb.cnsDescrizione <> "" Then
                                    strFiltro &= " AND {V_BILANCI_MALATTIA.CNV_CNS_CODICE}='" & Me.uscScegliAmb.cnsCodice & "' "
                                End If
                                If Me.uscScegliAmb.ambCodice <> 0 Then
                                    strFiltro &= " AND {V_BILANCI_MALATTIA.CNV_AMB_CODICE}= " & Me.uscScegliAmb.ambCodice
                                End If
                                '--
                                Dim rpt As New ReportParameter()
                                '--
                                'stampa
                                '--
                                If Not OnVacReport.StampaReport(Constants.ReportName.BilanciMalattia, strFiltro, rpt, Nothing, Nothing, reportBiz.GetReportFolder(Constants.ReportName.BilanciMalattia)) Then
                                    OnVacUtility.StampaNonPresente(Page, Constants.ReportName.BilanciMalattia)
                                End If
                                '--
                                'salvataggio data invio per solo bilancio [modifica 08/06/2005]
                                '--
                                SalvaDateConsultorio(Constants.TipoStampaAppuntamento.BilanciMalattia)
                                '--
                                'salvataggio nome del file per la stampa dell'ultimo avviso [modifica 11/04/2005]
                                '--
                                SalvaFileUltimaStampa(rpt.ReportFileName, TipoFile.Bilancio)
                                '--
                        End Select
                        '--
                    End Using
                End Using
                '--
            Case "btnStampaUltimoAvviso"
                '--
                GeneraUltimaStampa(TipoFile.Avviso)
                '--
            Case "btnStampaUltimoBilancio"
                '--
                GeneraUltimaStampa(TipoFile.Bilancio)
                '--
        End Select
        '--
    End Sub

    Private Sub uscScegliAmb_Load(sender As Object, e As System.EventArgs) Handles uscScegliAmb.Load

        If Not IsPostBack Then
            '--
            Me.uscScegliAmb.cnsCodice = OnVacUtility.Variabili.CNS.Codice
            Me.uscScegliAmb.cnsDescrizione = OnVacUtility.Variabili.CNS.Descrizione
            '--
            Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                '--
                Using bizConsultori As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                    '--
                    Dim ambulatorioDefault As Entities.Ambulatorio = bizConsultori.GetAmbulatorioDefault(Me.uscScegliAmb.cnsCodice, True, True)
                    '--
                    Me.uscScegliAmb.ambCodice = ambulatorioDefault.Codice
                    Me.uscScegliAmb.ambDescrizione = ambulatorioDefault.Descrizione
                    '--
                End Using
                '--
            End Using
            '--
            Me.uscScegliAmb.databind()
            '--
        End If

    End Sub

#Region " Associazioni Dosi "

#Region " Types "

    <Serializable()>
    Public Class FiltriAssociazioniDosi

        Public Associazioni As List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro1)
        Public DosiAssociazioni As List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro2)

        Public Sub New()
            Associazioni = New List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro1)()
            DosiAssociazioni = New List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro2)()
        End Sub

    End Class

#End Region

    Private Sub btnImgAssociazioniDosi_Click(sender As Object, e As ImageClickEventArgs) Handles btnImgAssociazioniDosi.Click

        'Set degli eventuali filtri associazioni
        If FiltriMaschera.Associazioni.Count > 0 Then

            Dim associazioni As DataTable = FiltriMaschera.Associazioni.ConvertToDataTable()
            Me.UscFiltroAssociazioniDosi.setValoriSelezionatiFiltro1(associazioni)

        End If

        'Set degli eventuali filtri dosi associazioni
        If FiltriMaschera.DosiAssociazioni.Count > 0 Then

            Dim dosiAssociazioni = FiltriMaschera.DosiAssociazioni.ConvertToDataTable()
            Me.UscFiltroAssociazioniDosi.setValoriSelezionatiFiltro2(dosiAssociazioni)

        End If

        'Apertura della modale
        Me.fmFiltroAssociazioniDosi.VisibileMD = True

    End Sub

    Private Sub btnOk_FiltroAssociazioniDosi_Click(sender As Object, e As EventArgs) Handles btnOk_FiltroAssociazioniDosi.Click

        'Chiusura della modale
        Me.fmFiltroAssociazioniDosi.VisibileMD = False

        ' Aggiornamento dei filtri nel viewstate
        Dim dtAssociazioni As DataTable = Me.UscFiltroAssociazioniDosi.getValoriSelezionatiFiltro1()
        Me.FiltriMaschera.Associazioni = dtAssociazioni.ConvertToList(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro1)()
        Dim dtDosiAssociazioni As DataTable = Me.UscFiltroAssociazioniDosi.getValoriSelezionatiFiltro2()
        Me.FiltriMaschera.DosiAssociazioni = dtDosiAssociazioni.ConvertToList(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro2)()

        'Aggiornamento label
        Me.lblAssociazioniDosi.Text = Me.UscFiltroAssociazioniDosi.getStringaFormattata()

    End Sub

    Private Sub btnAnnulla_FiltroAssociazioniDosi_Click(sender As Object, e As EventArgs) Handles btnAnnulla_FiltroAssociazioniDosi.Click

        Me.fmFiltroAssociazioniDosi.VisibileMD = False

    End Sub

    Private Function GetFiltroAssociazioniDosi() As Entities.FiltroComposto

        Dim filtroAssociazioniDosi As Entities.FiltroComposto = Nothing

        If FiltriMaschera.Associazioni.Count > 0 OrElse FiltriMaschera.DosiAssociazioni.Count > 0 Then

            Dim associazioniSelezionate As String = UscFiltroAssociazioniDosi.getStringaFiltro1("|")
            Dim dosiSelezionate As String = UscFiltroAssociazioniDosi.getStringaFiltro2("|")

            filtroAssociazioniDosi = New Entities.FiltroComposto()

            'Associazioni
            filtroAssociazioniDosi.CodiceValore =
                FiltriMaschera.Associazioni.Select(Function(ass) New KeyValuePair(Of String, String)(ass.Codice, ass.Valore)).ToList()

            'Dosi
            Dim dosi As List(Of String) = FiltriMaschera.DosiAssociazioni.Select(Function(dos) dos.Codice).ToList()
            If Not dosi.IsNullOrEmpty() Then
                filtroAssociazioniDosi.Valori = dosi.Select(Function(dos) Integer.Parse(dos)).ToList()
            End If

        End If

        Return filtroAssociazioniDosi

    End Function

    Private Function GetFiltroReport_AssociazioniDosi() As String

        Dim filtro As New System.Text.StringBuilder()

        Dim filtroAssociazioniDosi As Entities.FiltroComposto = GetFiltroAssociazioniDosi()

        If Not filtroAssociazioniDosi Is Nothing Then

            If Not filtroAssociazioniDosi.CodiceValore.IsNullOrEmpty() Then

                filtro.Append(" AND (")

                For Each pair As KeyValuePair(Of String, String) In filtroAssociazioniDosi.CodiceValore

                    If String.IsNullOrWhiteSpace(pair.Value) Then

                        filtro.AppendFormat(" InStr( {{V_AVVISI.ASSOCIAZIONI_DOSI}}, '|{0};') > 0 OR ", pair.Key)

                    Else

                        Dim values As String() = pair.Value.Split(",")

                        For Each value As String In values
                            filtro.AppendFormat(" InStr( {{V_AVVISI.ASSOCIAZIONI_DOSI}}, '|{0};{1}|') > 0 OR ", pair.Key, value)
                        Next

                    End If

                Next

                filtro.RemoveLast(3)
                filtro.Append(") ")

            End If

            If Not filtroAssociazioniDosi.Valori.IsNullOrEmpty() Then

                filtro.Append(" AND (")

                For Each dose As Integer In filtroAssociazioniDosi.Valori
                    filtro.AppendFormat(" InStr( {{V_AVVISI.ASSOCIAZIONI_DOSI}}, ';{0}|') > 0 OR ", dose)
                Next

                filtro.RemoveLast(3)
                filtro.Append(") ")

            End If

        End If

        Return filtro.ToString()

    End Function

#End Region

#End Region

#Region " Private Methods "

    Private Sub CaricaMalattie()

        Dim dt As DataTable

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizMalattie As New BizMalattie(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                dt = bizMalattie.GetDtCodiceDescrizioneMalattie(False, True)

            End Using
        End Using

        cmbMalCronica.DataValueField = "MAL_CODICE"
        cmbMalCronica.DataTextField = "MAL_DESCRIZIONE"
        cmbMalCronica.DataSource = dt
        cmbMalCronica.DataBind()

    End Sub

    ''' <summary>
    ''' Legge il valore selezionato dal radiobutton dei pazienti avvisati e restituisce l'enumerazione corrispondente
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetFiltroPazientiAvvisati() As OnVac.Enumerators.FiltroAvvisati

        Return DirectCast(Convert.ToInt32(Me.rdbFiltroSoggetti.SelectedValue), Enumerators.FiltroAvvisati)

    End Function

#Region " Gestione visualizzazione radiobutton stampe "

    Private Sub ShowTipoAvvisoPostel()

        Dim i As Integer = rblExportPostel.Items.Count

        While i > 0
            Dim cItem As ListItem = rblExportPostel.Items(i - 1)
            If Not String.IsNullOrEmpty(cItem.Value) AndAlso Not Me.Settings.EXPORT_POSTEL_TIPO_AVVISO_VISIBILE.Contains(cItem.Value) Then
                rblExportPostel.Items.Remove(cItem)
            End If
            i = i - 1
        End While

    End Sub

    Private Sub ShowOpzioniStampa()

        ' Caricamento report
        Dim rptList As List(Of Entities.Report)

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizRpt As New BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                Dim listRptNames As New List(Of String)()
                listRptNames.Add(Constants.ReportName.AvvisoAppuntamento)
                listRptNames.Add(Constants.ReportName.AppuntamentiDelGiorno)
                listRptNames.Add(Constants.ReportName.EtichetteAvvisoAppuntamento)
                listRptNames.Add(Constants.ReportName.AvvisoSoloBilancio)
                listRptNames.Add(Constants.ReportName.ElencoSoloBilanci)
                listRptNames.Add(Constants.ReportName.BilanciMalattia)
                listRptNames.Add(Constants.ReportName.AvvisoCampagnaAdulti)
                listRptNames.Add(Constants.ReportName.ElencoBilanciMalattia)
                listRptNames.Add(Constants.ReportName.EtichetteAssistitiAvvisi)

                rptList = bizRpt.GetReports(listRptNames)

            End Using
        End Using

        ' Visualizzazione radiobuttons
        Dim selectedValue As String = optModalit‡Stampa.SelectedValue
        optModalit‡Stampa.ClearSelection()
        optModalit‡Stampa.Items.Clear()

        Dim rdbList As New List(Of ListItem)()
        rdbList.Add(New ListItem("Avvisi", Constants.TipoStampaAppuntamento.Avvisi))
        rdbList.Add(New ListItem("Elenco Avvisi", Constants.TipoStampaAppuntamento.ElencoAvvisi))
        rdbList.Add(New ListItem("Etichette Avvisi", Constants.TipoStampaAppuntamento.EtichetteAvvisi))
        rdbList.Add(New ListItem("Bilanci", Constants.TipoStampaAppuntamento.Bilanci))
        rdbList.Add(New ListItem("Elenco Bilanci", Constants.TipoStampaAppuntamento.ElencoBilanci))
        rdbList.Add(New ListItem("Avvisi Campagna Adulti", Constants.TipoStampaAppuntamento.CampagnaAdulti))
        rdbList.Add(New ListItem("Bilanci per Malattia cronica", Constants.TipoStampaAppuntamento.BilanciMalattia))
        rdbList.Add(New ListItem("Elenco Bilanci per Malattia cronica", Constants.TipoStampaAppuntamento.ElencoBilanciMalattia))
        rdbList.Add(New ListItem("Etichette Assistiti", Constants.TipoStampaAppuntamento.EtichetteAssistitiAvvisi))

        For i As Integer = 0 To rdbList.Count - 1
            If Me.ShowReportOption(rdbList(i).Value, rptList) Then
                Me.optModalit‡Stampa.Items.Add(rdbList(i))
            End If
        Next

        ' Visualizzazione pulsante "stampa selezionati"
        If Me.optModalit‡Stampa.Items.Count > 0 Then
            Dim itemToSelect As ListItem = Me.optModalit‡Stampa.Items.FindByValue(selectedValue)

            If Not itemToSelect Is Nothing Then
                itemToSelect.Selected = True
            Else
                Me.optModalit‡Stampa.Items(0).Selected = True
            End If
        Else
            Me.Toolbar.Items.FromKeyButton("btnStampa").Visible = False
        End If

        ' Visualizzazione CheckBox Postel
        If Me.optModalit‡Stampa.Items.Contains(New ListItem("Avvisi", Constants.TipoStampaAppuntamento.Avvisi)) Then

            ' Se tra i report disponibili c'Ë il report degli avvisi, la visualizzazione 
            ' del checkbox di esportazione del tracciato postel Ë gestita dal parametro
            Me.chkExportPostel.Visible = Not String.IsNullOrEmpty(Me.Settings.EXPORT_POSTEL_ARGOMENTO)
            Me.rblExportPostel.Visible = Not String.IsNullOrEmpty(Me.Settings.EXPORT_POSTEL_ARGOMENTO)
        Else

            ' Se il report degli avvisi non Ë disponibile, non visualizzo 
            ' nemmeno il checkbox di esportazione del tracciato postel
            Me.chkExportPostel.Visible = False
            Me.rblExportPostel.Visible = False

        End If

    End Sub

    Private Function ShowReportOption(tipoStampaSelezionata As String, rptList As List(Of Entities.Report))

        Dim nomeReport As String = String.Empty

        Select Case tipoStampaSelezionata
            Case Constants.TipoStampaAppuntamento.Avvisi
                nomeReport = Constants.ReportName.AvvisoAppuntamento
            Case Constants.TipoStampaAppuntamento.ElencoAvvisi
                nomeReport = Constants.ReportName.AppuntamentiDelGiorno
            Case Constants.TipoStampaAppuntamento.EtichetteAvvisi
                nomeReport = Constants.ReportName.EtichetteAvvisoAppuntamento
            Case Constants.TipoStampaAppuntamento.Bilanci
                nomeReport = Constants.ReportName.AvvisoSoloBilancio
            Case Constants.TipoStampaAppuntamento.ElencoBilanci
                nomeReport = Constants.ReportName.ElencoSoloBilanci
            Case Constants.TipoStampaAppuntamento.BilanciMalattia
                nomeReport = Constants.ReportName.BilanciMalattia
            Case Constants.TipoStampaAppuntamento.CampagnaAdulti
                nomeReport = Constants.ReportName.AvvisoCampagnaAdulti
            Case Constants.TipoStampaAppuntamento.ElencoBilanciMalattia
                nomeReport = Constants.ReportName.ElencoBilanciMalattia
            Case Constants.TipoStampaAppuntamento.EtichetteAssistitiAvvisi
                nomeReport = Constants.ReportName.EtichetteAssistitiAvvisi
        End Select

        Dim rpt As Entities.Report = (From r As Entities.Report In rptList
                                      Where r.Nome = nomeReport
                                      Select r).FirstOrDefault()

        Return (Not rpt Is Nothing)

    End Function

#End Region

#Region " Gestione ultima stampa eseguita "

    'apre una nuova finestra con l'ultima stampa effettuata [modifica 11/04/2005]
    Private Sub GeneraUltimaStampa(tipoFile As TipoFile)

        Dim nomeFile As String = String.Empty
        Dim controllaUltimoFile As Boolean = False

        Using dam As IDAM = OnVacUtility.OpenDam()

            'controllo se Ë memorizzato un ultimo file di stampa
            With dam.QB
                .NewQuery()
                .AddSelectFields(IIf(tipoFile = TipoFile.Avviso, "CNS_PDF_ULTIMO_AVVISO", "CNS_PDF_ULTIMO_BILANCIO"))
                .AddTables("T_ANA_CONSULTORI")
                .AddWhereCondition("CNS_CODICE", Comparatori.Uguale, OnVacUtility.Variabili.CNS.Codice, DataTypes.Stringa)
            End With

            Dim objNome As Object = dam.ExecScalar()

            If Not objNome Is Nothing And Not objNome Is DBNull.Value Then
                nomeFile = objNome.ToString()
            Else
                controllaUltimoFile = True
            End If

            If Not controllaUltimoFile Then
                If Dir(ConfigurationManager.AppSettings.Get("staPath").ToString & nomeFile & ".pdf", FileAttribute.Normal) <> "" Then
                    'generazione ultima stampa avvisi/bilanci
                    OnitLayout31.InsertRoutineJS("window.open('../../Stampe/StampaReportPopUp.aspx?report=" & nomeFile & "&stampaDirettaPdf=S','" & IIf(tipoFile = TipoFile.Avviso, "Stampa_Ultimi_Avvisi", "Stampa_Ultimi_Bilanci") & "','top=0,left=0,width=500,height=500,resizable=1');")
                Else
                    controllaUltimoFile = True
                End If
            End If

            If controllaUltimoFile Then OnitLayout31.InsertRoutineJS("alert('Attenzione: ultimo file di stampa non presente!\r» necessario effettuare una nuova stampa" & IIf(tipoFile = TipoFile.Avviso, " degli avvisi ", " dei bilanci ") & "per visualizzarlo successivamente.');")

        End Using

    End Sub

    'salvataggio del file creato nella cartella temporanea 
    'per il recupero dell'ultima stampa [modifica 11/04/2005]
    Private Sub SalvaFileUltimaStampa(nomeFile As String, tipoFile As TipoFile)

        If Not String.IsNullOrEmpty(nomeFile) Then

            'elimina l'estensione
            nomeFile = nomeFile.Replace(".pdf", "")

            Using dam As IDAM = OnVacUtility.OpenDam()

                With dam.QB
                    .NewQuery()
                    .AddTables("T_ANA_CONSULTORI")
                    .AddUpdateField(IIf(tipoFile = TipoFile.Avviso, "CNS_PDF_ULTIMO_AVVISO", "CNS_PDF_ULTIMO_BILANCIO"), nomeFile, DataTypes.Stringa)
                    .AddWhereCondition("CNS_CODICE", Comparatori.Uguale, OnVacUtility.Variabili.CNS.Codice, DataTypes.Stringa)
                End With
                dam.ExecNonQuery(ExecQueryType.Update)

            End Using

        End If

    End Sub

    Private Sub CaricaDateUltimaStampa()

        Dim dateInfoConsultorio As Entities.ConsultorioDateInfo = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizConsultori As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                dateInfoConsultorio = bizConsultori.GetDateInfoConsultorio(OnVacUtility.Variabili.CNS.Codice)

            End Using
        End Using

        If Not dateInfoConsultorio Is Nothing Then
            OnVacUtility.SetLabelCampoDate(Me.lblOldDataDa, dateInfoConsultorio.DataUltimaStampaAvvisoInizio)
            OnVacUtility.SetLabelCampoDate(Me.lblOldDataA, dateInfoConsultorio.DataUltimaStampaAvvisoFine)
            OnVacUtility.SetLabelCampoDate(Me.lblOldDataDaBil, dateInfoConsultorio.DataUltimaStampaBilancioInizio)
            OnVacUtility.SetLabelCampoDate(Me.lblOldDataABil, dateInfoConsultorio.DataUltimaStampaBilancioFine)
        End If

    End Sub

#End Region

#Region " Salvataggio data invio "

    ' Salvataggio della data invio e delle date del periodo di stampa. Non effettuata in caso di export postel.
    Private Sub SalvaDateConsultorio(tipoStampaSelezionata As String)
        '--
        If txtNoteAvviso.Text.Length > txtNoteAvviso.MaxLength Then
            txtNoteAvviso.Text = txtNoteAvviso.Text.Substring(0, txtNoteAvviso.MaxLength)
        End If

        Dim command As New Entities.PazientiAvvisiCommand()
        command.CodiceConsultorio = Me.uscScegliAmb.cnsCodice
        command.CodiceAmbulatorio = uscScegliAmb.ambCodice
        command.DataInizioAppuntamento = Me.odpDataIniz.Data
        command.DataFineAppuntamento = Me.odpDataFin.Data
        command.FiltroPazientiAvvisati = Me.GetFiltroPazientiAvvisati()
        command.TipoStampaAppuntamento = tipoStampaSelezionata
        command.NoteAvvisi = txtNoteAvviso.Text
        command.IsPostel = Me.chkExportPostel.Checked
        '--
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizStampa As New Biz.BizStampaInviti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())
                '--
                bizStampa.SalvaDateStampa(command)
                '--
            End Using
        End Using
        '--
    End Sub

#End Region

#End Region

End Class
