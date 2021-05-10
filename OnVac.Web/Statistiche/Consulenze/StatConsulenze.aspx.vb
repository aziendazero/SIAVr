Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL


Public Class StatConsulenze
    Inherits OnVac.Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As Object

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Eventi Page "

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            PanelTitolo.InnerText = OnVacUtility.Variabili.CNS.Descrizione

            fldCentroVacc.Attributes.Add("title", GetOnVacResourceValue(Constants.StringResourcesKey.StatConsulenze_CentroVaccinale))
            fldTipoConsulenza.Attributes.Add("title", GetOnVacResourceValue(Constants.StringResourcesKey.StatConsulenze_TipoConsulenza))
            fldDataNascita.Attributes.Add("title", GetOnVacResourceValue(Constants.StringResourcesKey.StatConsulenze_PazienteDataNascita))
            fldOperatore.Attributes.Add("title", GetOnVacResourceValue(Constants.StringResourcesKey.StatConsulenze_Operatore))
            fldDataEsecuzione.Attributes.Add("title", GetOnVacResourceValue(Constants.StringResourcesKey.StatConsulenze_DataEsecuzione))

            ucSelezioneConsultori.MostraSoloAperti = False
            ucSelezioneConsultori.ImpostaCnsCorrente = True
            ucSelezioneConsultori.LoadGetCodici()
            CaricaDati()
            ShowPrintButtons()

        End If

        'generazione della stampa tramite pulsante invio
        Select Case Request.Form("__EVENTTARGET")
            Case "Stampa"
                If Toolbar.Items.FromKeyButton("btnStampa").Visible Then
                    Stampa()
                End If
        End Select

    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub Toolbar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked

        Select Case e.Button.Key

            Case "btnStampa"
                Stampa()

            Case "btnElencoConsulenze"
                EsportaDati(Constants.ReportName.EstrazioneElencoConsulenze, Report.ReportViewer.TipoEsportazioneReport.XLS)

        End Select

    End Sub

#End Region

#Region " Private "

#Region " Caricamento dati "

    Private Sub CaricaDati()

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            CaricaTipoConsulenze(genericProvider)
        End Using

    End Sub

    Private Sub CaricaTipoConsulenze(genericProvider As DbGenericProvider)

        Dim lstInterventi As List(Of Entities.Intervento) = Nothing

        Using bizInterventi As New Biz.BizInterventi(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

            lstInterventi = bizInterventi.GetInterventi(String.Empty)

        End Using

        'Aggiungo un elemento vuoto
        lstInterventi.Insert(0, New Entities.Intervento())

        ddlTipoConsulenza.DataTextField = "Descrizione"
        ddlTipoConsulenza.DataValueField = "Codice"
        ddlTipoConsulenza.DataSource = lstInterventi
        ddlTipoConsulenza.DataBind()

    End Sub

#End Region

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of PrintButton)()
        listPrintButtons.Add(New PrintButton(Constants.ReportName.Consulenze, "btnStampa"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.EstrazioneElencoConsulenze, "btnElencoConsulenze"))

        ShowToolbarPrintButtons(listPrintButtons, Toolbar)

    End Sub

    Private Sub Stampa()

        Dim rpt As New ReportParameter()
        Dim nomeReport As String = Constants.ReportName.Consulenze

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            Dim ConteggioResult As Biz.BizInterventi.ConteggioConsulenzeResult = Nothing

            Using bizInterventi As New Biz.BizInterventi(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                Dim filtriConteggio As New Biz.BizInterventi.FiltriConteggioConsulenze()

                ' Date di nascita
                If odpDataNascitaIniz.Data <> Nothing Then filtriConteggio.DataNascitaInizio = odpDataNascitaIniz.Data
                If odpDataNascitaFin.Data <> Nothing Then filtriConteggio.DataNascitaFine = odpDataNascitaFin.Data

                ' Date di esecuzione
                If odpDataEsecuzioneIniz.Data <> Nothing Then filtriConteggio.DataEsecuzioneInizio = odpDataEsecuzioneIniz.Data
                If odpDataEsecuzioneFin.Data <> Nothing Then filtriConteggio.DataEsecuzioneFine = odpDataEsecuzioneFin.Data

                ' Consultorio
                Dim lista As List(Of String) = ucSelezioneConsultori.GetConsultoriSelezionati
                If lista.Count = 0 Then
                    Using bizCns As New Biz.BizConsultori(genericProvider, Settings, OnVacContext.CreateBizContextInfos, Nothing)
                        lista = bizCns.GetListCodiceConsultoriUtente(OnVacContext.UserId)
                        If lista.Count > 0 Then
                            filtriConteggio.CodiceConsultorio = lista
                            filtriConteggio.DescrizioneConsultori = "TUTTI"
                        End If
                    End Using
                Else
                    filtriConteggio.CodiceConsultorio = lista
                    filtriConteggio.DescrizioneConsultori = ucSelezioneConsultori.GetDescrizioneConsultoriSelezionatiPerStampe()
                End If

                ' Tipo consulenza
                filtriConteggio.CodiceTipoConsulenza = ddlTipoConsulenza.SelectedValue
                filtriConteggio.DescrizioneTipoConsulenza = ddlTipoConsulenza.SelectedItem.Text

                ' Operatore
                filtriConteggio.CodiceOperatore = omlOperatore.Codice
                filtriConteggio.DescrizioneTipoConsulenza = omlOperatore.Descrizione

                ConteggioResult = bizInterventi.GetConteggioConsulenze(nomeReport, filtriConteggio)

            End Using

            rpt.set_dataset(ConteggioResult.DataSetConteggioConsulenze)

            If Not ConteggioResult.ParametriReport Is Nothing AndAlso ConteggioResult.ParametriReport.Count > 0 Then
                For Each parametro As KeyValuePair(Of String, String) In ConteggioResult.ParametriReport
                    rpt.AddParameter(parametro.Key, parametro.Value)
                Next
            End If

            ' N.B. : se OnVacContext.UserDescription è Nothing, all'rpt deve essere passata la stringa vuota se no dà errore
            If String.IsNullOrEmpty(OnVacContext.UserDescription) Then
                rpt.AddParameter("UtenteStampa", String.Empty)
            Else
                rpt.AddParameter("UtenteStampa", OnVacContext.UserDescription)
            End If

            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If Not OnVacReport.StampaReport(nomeReport, String.Empty, rpt, , , bizReport.GetReportFolder(nomeReport)) Then
                    OnVacUtility.StampaNonPresente(Page, nomeReport)
                End If

            End Using

        End Using

    End Sub


    Private Sub EsportaDati(rptName As String, tipoEsportazione As Report.ReportViewer.TipoEsportazioneReport)

        Dim filtriQuery As New Text.StringBuilder()

        ' Date di nascita
        If odpDataNascitaIniz.Data <> Nothing Then filtriQuery.AppendFormat("f-dataNascDa={0}&", odpDataNascitaIniz.Data.ToShortDateString())
        If odpDataNascitaFin.Data <> Nothing Then filtriQuery.AppendFormat("f-dataNascA={0}&", odpDataNascitaFin.Data.ToShortDateString())

        ' Date di esecuzione
        If odpDataEsecuzioneIniz.Data <> Nothing Then filtriQuery.AppendFormat("f-dataEsecDa={0}&", odpDataEsecuzioneIniz.Data.ToShortDateString())
        If odpDataEsecuzioneFin.Data <> Nothing Then filtriQuery.AppendFormat("f-dataEsecA={0}&", odpDataEsecuzioneFin.Data.ToShortDateString())

        ' Consultorio
        Dim lista As List(Of String) = ucSelezioneConsultori.GetConsultoriSelezionati
        If lista.Count = 0 Then
            Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                Using bizCns As New Biz.BizConsultori(genericProvider, Settings, OnVacContext.CreateBizContextInfos, Nothing)
                    lista = bizCns.GetListCodiceConsultoriUtente(OnVacContext.UserId)
                End Using
            End Using
        End If
        If lista.Count > 0 Then
            filtriQuery.Append("f-cns=")
            Dim i As Integer = lista.Count()
            For Each codCNS As String In lista
                filtriQuery.AppendFormat("{0}{1}", codCNS, IIf(i > 1, "|", ""))
                i -= 1
            Next
            filtriQuery.Append("&")
        End If

        ' Tipo consulenza
        If Not String.IsNullOrEmpty(ddlTipoConsulenza.SelectedValue) Then filtriQuery.AppendFormat("f-tipoConsulenza={0}&", ddlTipoConsulenza.SelectedValue)

        ' Operatore
        If Not String.IsNullOrEmpty(omlOperatore.Codice) Then filtriQuery.AppendFormat("f-codOperatore={0}&", omlOperatore.Codice)


        Dim filtri As String = String.Empty
        If filtriQuery.ToString() <> String.Empty Then
            filtri = filtriQuery.ToString().Remove(filtriQuery.Length - 1, 1)
        End If

        Dim url As String = String.Format("{0}?appId={1}&codAzienda={2}&id={3}&format={4}&rpt={5}{6}",
                                  ResolveClientUrl("~/Common/Handlers/ReportViewerExportHandler.ashx"),
                                  OnVacContext.AppId, OnVacContext.Azienda, "0",
                                  tipoEsportazione, rptName, IIf(String.IsNullOrWhiteSpace(filtri), "", "&" + filtri))

        Dim key As String = "Export"
        Select Case tipoEsportazione
            Case Report.ReportViewer.TipoEsportazioneReport.XLS
                key = "xlsExportHandler"
        End Select

        Page.ClientScript.RegisterStartupScript(Me.GetType(), key,
                                                String.Format("<script type='text/javascript'> window.open('{0}'); </script>", url))

    End Sub


#End Region

End Class
