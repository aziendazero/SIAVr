Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Biz


Partial Class AvvisoMaggiorenni
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

#Region " Page Event "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        'Pulizia e log della session
        Dim sc As New SessionCleaner(Me.Settings)
        sc.Start()

        If Not IsPostBack Then

			ucSelezioneConsultori.MostraSoloAperti = False
			ucSelezioneConsultori.ImpostaCnsCorrente = True
			ucSelezioneConsultori.LoadGetCodici()

			CaricamentoDati()

            ShowPrintButtons()

        End If

    End Sub

#End Region

#Region " Event Toolbar "

    Private Sub Toolbar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked

        Dim dataNascitaIniziale As DateTime = Me.odpDataNascitaIniz.Data.AddYears(-18)
        Dim dataNascitaFinale As DateTime = Me.odpDataNascitaFin.Data.AddYears(-18)

        Dim stbFiltro As New System.Text.StringBuilder()

        'filtro data di nascita
        stbFiltro.AppendFormat("{{V_ELENCO_NON_VACCINATI_MAGG.PAZ_DATA_NASCITA}} >= DateTime ({0}, {1}, {2}, 00, 00, 00) ",
                               dataNascitaIniziale.Year.ToString(), dataNascitaIniziale.Month.ToString(), dataNascitaIniziale.Day.ToString())

        stbFiltro.AppendFormat(" AND {{V_ELENCO_NON_VACCINATI_MAGG.PAZ_DATA_NASCITA}} <= DateTime ({0}, {1}, {2}, 00, 00, 00)",
                               dataNascitaFinale.Year.ToString(), dataNascitaFinale.Month.ToString(), dataNascitaFinale.Day.ToString())

		'filtro consultorio
		Dim lista As List(Of String) = ucSelezioneConsultori.GetConsultoriSelezionati
		If lista.Count > 0 Then
			stbFiltro.AppendFormat(" AND {{V_ELENCO_NON_VACCINATI_MAGG.PAZ_CNS_CODICE}} in ['{0}'] ", lista.Aggregate(Function(p, g) p & "', '" & g))
		Else
			Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
				Using bizCns As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)
					lista = bizCns.GetListCodiceConsultoriUtente(OnVacContext.UserId)
					If lista.Count > 0 Then
						stbFiltro.AppendFormat(" AND {{V_ELENCO_NON_VACCINATI_MAGG.PAZ_CNS_CODICE}} in ['{0}'] ", lista.Aggregate(Function(p, g) p & "', '" & g))
					End If

				End Using
			End Using
		End If

		If Not String.IsNullOrEmpty(Me.fmComuneRes.Codice) Then
            stbFiltro.AppendFormat(" AND {{V_ELENCO_NON_VACCINATI_MAGG.PAZ_COM_CODICE_RESIDENZA}} = '{0}' ", Me.fmComuneRes.Codice)
        End If

        ' filtro stati anagrafici
        If Me.chklStatoAnagrafico.SelectedItems.Count > 0 AndAlso Me.chklStatoAnagrafico.SelectedItems.Count < Me.chklStatoAnagrafico.Items.Count Then

            Dim arrayCodiceStatiAnagrafici As String() = (From item As ListItem In Me.chklStatoAnagrafico.SelectedItems
                                                          Select "'" + item.Value + "'").ToArray()

            stbFiltro.AppendFormat(" AND {{V_ELENCO_NON_VACCINATI_MAGG.PAZ_STATO_ANAGRAFICO}} IN [{0}] ", String.Join(",", arrayCodiceStatiAnagrafici))

        End If

        ' flag avvisati
        Select Case Me.rdbFiltroSoggetti.SelectedIndex
            Case 1
                stbFiltro.Append(" AND {V_ELENCO_NON_VACCINATI_MAGG.PAZ_FLAG_STAMPA_MAGGIORENNI} = 'S'")
            Case 2
                stbFiltro.Append(" AND ISNULL({V_ELENCO_NON_VACCINATI_MAGG.PAZ_FLAG_STAMPA_MAGGIORENNI})")
        End Select

        ' Provider per accesso al db (utilizzato per accedere alla t_ana_report)
        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                Dim rpt As New ReportParameter()

                If Not OnVacReport.StampaReport(Constants.ReportName.AvvisoMaggiorenni, stbFiltro.ToString(), rpt, Nothing, Nothing, bizReport.GetReportFolder(Constants.ReportName.AvvisoMaggiorenni)) Then
                    OnVacUtility.StampaNonPresente(Page, Constants.ReportName.AvvisoMaggiorenni)
                Else

                    If Me.rdbFiltroSoggetti.SelectedIndex <> 1 Then

                        Try
                            genericProvider.BeginTransaction()

                            Using bizPaziente As New Biz.BizPaziente(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                                For Each codiceL As String In lista
                                    bizPaziente.UpdateFlagStampaAvvisoMaggiorenni(codiceL, Me.fmComuneRes.Codice, dataNascitaIniziale, dataNascitaFinale)
                                Next
                            End Using

                            Using bizConsultori As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                                bizConsultori.UpdateDateUltimaStampaAvvisoMaggiorenni(OnVacUtility.Variabili.CNS.Codice, Me.odpDataNascitaIniz.Data, Me.odpDataNascitaFin.Data)
                            End Using

                            CaricaDateUltimaStampa(genericProvider)

                            genericProvider.Commit()

                        Catch ex As Exception
                            genericProvider.Rollback()
                            Throw New ApplicationException("Si è verificato un problema durante il salvataggio dei dati della stampa", ex)
                        End Try

                    End If

                End If

            End Using
        End Using

    End Sub

#End Region

#Region " Private "

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()
        listPrintButtons.Add(New PrintButton(Constants.ReportName.AvvisoMaggiorenni, "btnStampa"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Toolbar)

    End Sub

    Private Sub CaricamentoDati()

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            CaricaStatiAnagrafici(genericProvider)
            CaricaDateUltimaStampa(genericProvider)
        End Using

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

    Private Sub CaricaDateUltimaStampa(genericProvider As DAL.DbGenericProvider)

        Dim dateInfoConsultorio As Entities.ConsultorioDateInfo = Nothing

        Using bizConsultori As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
            dateInfoConsultorio = bizConsultori.GetDateInfoConsultorio(OnVacUtility.Variabili.CNS.Codice)
        End Using

        If Not dateInfoConsultorio Is Nothing Then
            OnVacUtility.SetLabelCampoDate(Me.lblOldDataDa, dateInfoConsultorio.DataUltimaStampaMaggiorenniInizio)
            OnVacUtility.SetLabelCampoDate(Me.lblOldDataA, dateInfoConsultorio.DataUltimaStampaMaggiorenniFine)
        End If

    End Sub

#End Region

End Class
