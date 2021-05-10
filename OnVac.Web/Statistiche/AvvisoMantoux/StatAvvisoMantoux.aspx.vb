Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager


Partial Class StatAvvisoMantoux
    Inherits OnVac.Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    Protected WithEvents lblConsultorio As System.Web.UI.WebControls.Label
    Protected WithEvents lblDataIniz As System.Web.UI.WebControls.Label
    Protected WithEvents lblStatoAnagrafico1 As System.Web.UI.WebControls.Label
    Protected WithEvents lblStatoAnagrafico2 As System.Web.UI.WebControls.Label

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

			fldDataInvio.Attributes.Add("title", Me.GetOnVacResourceValue(Constants.StringResourcesKey.DatiPaziente_MantouxDataInvio))

            ShowPrintButtons()

            CaricaStatiAnagrafici()

        End If

    End Sub

#End Region

#Region " Toolbar "

    Private Sub Toolbar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked

        Select Case e.Button.Key

            Case "btnStampaAvviso"
                Stampa(Constants.ReportName.AvvisoMantoux)

            Case "btnStampaElenco"
                Stampa(Constants.ReportName.ElencoMantoux)

            Case "btnStampaElencoCompleto"
                Stampa(Constants.ReportName.ElencoMantouxCompleto)

        End Select

    End Sub

#End Region

#Region " Private "

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()

        listPrintButtons.Add(New PrintButton(Constants.ReportName.AvvisoMantoux, "btnStampaAvviso"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.ElencoMantoux, "btnStampaElenco"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.ElencoMantouxCompleto, "btnStampaElencoCompleto"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Toolbar)

    End Sub

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

    Private Function GetDateTimeCrystalReportFilter(filtroData As DateTime) As String

        Return String.Format(" DateTime({0}, {1}, {2}, 00, 00, 00) ", filtroData.Year, filtroData.Month, filtroData.Day)

    End Function

    Private Sub Stampa(nomeReport As String)

        ' Controlli anche lato client, con alert
        If String.IsNullOrWhiteSpace(odpDataMantouxDa.Text) Or String.IsNullOrWhiteSpace(odpDataMantouxA.Text) Then
            Return
        End If

        If ((String.IsNullOrWhiteSpace(odpDataInvioDa.Text) And Not String.IsNullOrWhiteSpace(odpDataInvioA.Text)) Or
           (Not String.IsNullOrWhiteSpace(odpDataInvioDa.Text) And String.IsNullOrWhiteSpace(odpDataInvioA.Text))) Then
            Return
        End If

        If ((String.IsNullOrWhiteSpace(odpDataNascitaDa.Text) And Not String.IsNullOrWhiteSpace(odpDataNascitaA.Text)) Or
           (Not String.IsNullOrWhiteSpace(odpDataNascitaDa.Text) And String.IsNullOrWhiteSpace(odpDataNascitaA.Text))) Then
            Return
        End If

        ' Filtro intervallo date effettuazione mantoux
        Dim filtro As New System.Text.StringBuilder()
        filtro.AppendFormat("{{T_PAZ_MANTOUX.MAN_DATA}} in {0} to {1} ", GetDateTimeCrystalReportFilter(odpDataMantouxDa.Data), GetDateTimeCrystalReportFilter(odpDataMantouxA.Data))

        ' Filtri solo per avviso ed elenco mantoux (che devono prendere in considerazione solo le mantoux non lette)
        If nomeReport = Constants.ReportName.AvvisoMantoux OrElse nomeReport = Constants.ReportName.ElencoMantoux Then

            filtro.Append(" AND isnull({T_PAZ_MANTOUX.MAN_MM}) ")
            filtro.Append(" AND isnull({T_PAZ_MANTOUX.MAN_OPE_CODICE}) ")
            filtro.Append(" AND isnull({T_PAZ_MANTOUX.MAN_DATA_INVIO}) ")

        End If

		' Filtro sul consultorio
		Dim lista As List(Of String) = ucSelezioneConsultori.GetConsultoriSelezionati
		If lista.Count > 0 Then
			filtro.AppendFormat(" AND ({{T_PAZ_PAZIENTI.PAZ_CNS_CODICE}} IN ['{0}']) ", lista.Aggregate(Function(p, g) p & "', '" & g))
		Else
			Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
				Using bizCns As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)
					lista = bizCns.GetListCodiceConsultoriUtente(OnVacContext.UserId)
					If lista.Count > 0 Then
						filtro.AppendFormat(" AND ({{T_PAZ_PAZIENTI.PAZ_CNS_CODICE}} IN ['{0}']) ", lista.Aggregate(Function(p, g) p & "', '" & g))
					End If
				End Using
			End Using

		End If


		' Filtro sulla data di nascita
		If odpDataNascitaDa.Data > DateTime.MinValue AndAlso odpDataNascitaA.Data > DateTime.MinValue Then
            filtro.AppendFormat(" AND {{T_PAZ_PAZIENTI.PAZ_DATA_NASCITA}} in {0} to {1} ", GetDateTimeCrystalReportFilter(odpDataNascitaDa.Data), GetDateTimeCrystalReportFilter(odpDataNascitaA.Data))
        End If

        ' Filtro sul flag eseguita
        If Not String.IsNullOrWhiteSpace(rblEseguita.SelectedValue) Then

            If rblEseguita.SelectedValue = "S" Then
                filtro.Append(" AND {T_PAZ_MANTOUX.MAN_SINO} = 'S' ")
            Else
                filtro.Append(" AND (isnull({T_PAZ_MANTOUX.MAN_SINO}) OR {T_PAZ_MANTOUX.MAN_SINO} = 'N') ")
            End If

        End If

        ' Filtro sul flag positiva
        If Not String.IsNullOrWhiteSpace(rblPositiva.SelectedValue) Then

            If rblPositiva.SelectedValue = "S" Then
                filtro.Append(" AND {T_PAZ_MANTOUX.MAN_POSITIVA} = 'S' ")
            Else
                filtro.Append(" AND (isnull({T_PAZ_MANTOUX.MAN_POSITIVA}) OR {T_PAZ_MANTOUX.MAN_POSITIVA} = 'N') ")
            End If

        End If

        ' Filtro sullo stato anagrafico
        Dim filtroCodiceStatoAnagrafico As New System.Text.StringBuilder()
        Dim filtroDescrizioneStatoAnagrafico As New System.Text.StringBuilder()

        For i As Int16 = 0 To Me.chklStatoAnagrafico.Items.Count - 1
            If Me.chklStatoAnagrafico.Items(i).Selected Then
                filtroCodiceStatoAnagrafico.AppendFormat(" ({{T_PAZ_PAZIENTI.PAZ_STATO_ANAGRAFICO}} = '{0}') OR ", chklStatoAnagrafico.Items(i).Value)
                filtroDescrizioneStatoAnagrafico.AppendFormat("{0}; ", chklStatoAnagrafico.Items(i).Text)
            End If
        Next

        If filtroCodiceStatoAnagrafico.Length > 0 Then
            filtroCodiceStatoAnagrafico.RemoveLast(3)
            filtro.AppendFormat(" AND ({0}) ", filtroCodiceStatoAnagrafico.ToString())
        End If

        ' Data invio
        If odpDataInvioDa.Data > DateTime.MinValue AndAlso odpDataInvioA.Data > DateTime.MinValue Then
            filtro.AppendFormat(" AND {{T_PAZ_MANTOUX.MAN_DATA_INVIO}} in {0} to {1} ", GetDateTimeCrystalReportFilter(odpDataInvioDa.Data), GetDateTimeCrystalReportFilter(odpDataInvioA.Data))
        End If

        Dim rpt As ReportParameter = Nothing

        If nomeReport = Constants.ReportName.ElencoMantoux OrElse
           nomeReport = Constants.ReportName.ElencoMantouxCompleto Then

            rpt = New ReportParameter()

            rpt.AddParameter("DataMantouxIniz", odpDataMantouxDa.Text)
            rpt.AddParameter("DataMantouxFin", odpDataMantouxA.Text)

            rpt.AddParameter("dataNascitaIniz", odpDataNascitaDa.Text)
            rpt.AddParameter("dataNascitaFin", odpDataNascitaA.Text)

            rpt.AddParameter("flagEseguita", rblEseguita.SelectedValue)
            rpt.AddParameter("flagPositiva", rblPositiva.SelectedValue)

            rpt.AddParameter("statiAnag", filtroDescrizioneStatoAnagrafico.ToString())
			rpt.AddParameter("centroVaccinale", ucSelezioneConsultori.GetDescrizioneConsultoriSelezionatiPerStampe())

			If nomeReport = Constants.ReportName.ElencoMantouxCompleto Then
                rpt.AddParameter("labelDataInvio", lblDataInvio.Text)
                rpt.AddParameter("dataInvioDa", odpDataInvioDa.Text)
                rpt.AddParameter("dataInvioA", odpDataInvioA.Text)
            End If

        End If

        ' Creazione del report
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If Not OnVacReport.StampaReport(nomeReport, filtro.ToString(), rpt, Nothing, Nothing, bizReport.GetReportFolder(nomeReport)) Then
                    OnVacUtility.StampaNonPresente(Page, nomeReport)
                End If

            End Using
        End Using

    End Sub

#End Region

End Class
