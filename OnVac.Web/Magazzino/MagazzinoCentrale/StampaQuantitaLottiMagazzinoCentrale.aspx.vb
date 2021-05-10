Imports System.Collections.Generic

Public Class StampaQuantitaLottiMagazzinoCentrale
    Inherits OnVac.Common.PageBase

#Region " Page Events "

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then
            Me.ShowPrintButtons()
            Me.InizializzaMovimenti()
        End If

    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub ToolBar_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case be.Button.Key

            Case "btnStampaLotto"

                Me.StampaQuantitativiMovimentati(Constants.ReportName.MagazzinoQuantitativiMovimentatiLotto)

            Case "btnStampaConsultorio"

                Me.StampaQuantitativiMovimentati(Constants.ReportName.MagazzinoQuantitativiMovimentatiConsultorio)

            Case "btnStampaElencoMovimenti"

                Me.StampaQuantitativiMovimentati(Constants.ReportName.MagazzinoElencoMovimenti)

            Case "btnPulisci"

                Me.dpkDataRegistrazioneDa.Text = String.Empty
                Me.dpkDataRegistrazioneA.Text = String.Empty

                Me.fmUtente.Codice = String.Empty
                Me.fmUtente.Descrizione = String.Empty
                Me.fmUtente.RefreshDataBind()

                Me.fmLotto.Codice = String.Empty
                Me.fmLotto.Descrizione = String.Empty
                Me.fmLotto.RefreshDataBind()

                Me.fmMagazzino.Codice = String.Empty
                Me.fmMagazzino.Descrizione = String.Empty
                Me.fmMagazzino.RefreshDataBind()

        End Select

    End Sub

#End Region

#Region " OnitModalList Events "

    Private Sub fmUtente_SetUpFiletr(sender As Object) Handles fmUtente.SetUpFiletr

        Me.fmUtente.Filtro = OnVacUtility.GetFiltroUtenteForOnitModalList(False)

    End Sub

#End Region

#Region " Private "

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()
        listPrintButtons.Add(New PrintButton(Constants.ReportName.MagazzinoQuantitativiMovimentatiLotto, "btnStampaLotto"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.MagazzinoQuantitativiMovimentatiConsultorio, "btnStampaConsultorio"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.MagazzinoElencoMovimenti, "btnStampaElencoMovimenti"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Me.ToolBar)

    End Sub

    Private Sub InizializzaMovimenti()

        Me.ddlTipoMovimento.Items.Add(New ListItem(String.Empty, String.Empty))

        Me.ddlTipoMovimento.Items.Add(New ListItem(Biz.BizLotti.GetDescrizioneMovimentoMagazzinoByTipo(Constants.TipoMovimentoMagazzino.Carico),
                                                   Constants.TipoMovimentoMagazzino.Carico))
        Me.ddlTipoMovimento.Items.Add(New ListItem(Biz.BizLotti.GetDescrizioneMovimentoMagazzinoByTipo(Constants.TipoMovimentoMagazzino.Scarico),
                                                   Constants.TipoMovimentoMagazzino.Scarico))
        Me.ddlTipoMovimento.Items.Add(New ListItem(Biz.BizLotti.GetDescrizioneMovimentoMagazzinoByTipo(Constants.TipoMovimentoMagazzino.TrasferimentoDa),
                                                   Constants.TipoMovimentoMagazzino.TrasferimentoDa))
        Me.ddlTipoMovimento.Items.Add(New ListItem(Biz.BizLotti.GetDescrizioneMovimentoMagazzinoByTipo(Constants.TipoMovimentoMagazzino.TrasferimentoA),
                                                   Constants.TipoMovimentoMagazzino.TrasferimentoA))

    End Sub

    Private Sub StampaQuantitativiMovimentati(reportName As String)

        ' Filtri
        Dim filtro As New Filters.FiltriStampaQuantitaLottiMovimentati()

        filtro.CodiceConsultorio = Me.fmMagazzino.Codice
        filtro.DescrizioneConsultorio = Me.fmMagazzino.Descrizione
        filtro.CodiceLotto = Me.fmLotto.Codice
        filtro.DescrizioneLotto = Me.fmLotto.Descrizione
        filtro.DataInizioRegistrazione = Me.dpkDataRegistrazioneDa.Data
        filtro.DataFineRegistrazione = Me.dpkDataRegistrazioneA.Data
        filtro.IdUtente = Me.fmUtente.ValoriAltriCampi("Id")
        filtro.CodiceUtente = Me.fmUtente.Codice
        filtro.DescrizioneUtente = Me.fmUtente.Descrizione
        filtro.Quantita = Me.txtQuantita.Text.Trim()
        If Not String.IsNullOrEmpty(filtro.Quantita) Then filtro.OperatoreConfrontoQuantita = Me.rdblOperatoreConfrontoQuantita.SelectedValue
		filtro.TipoMovimento = Me.ddlTipoMovimento.SelectedValue
		filtro.IdUtenteConnesso = OnVacContext.UserId.ToString()

		Dim controlloFiltriResult As Biz.BizLotti.BizLottiResult = Biz.BizLotti.CheckFiltriStampaQuantitativiMovimentati(filtro)

        If controlloFiltriResult.Result <> Biz.BizLotti.BizLottiResult.ResultType.Success Then

            Me.OnitLayout31.InsertRoutineJS(String.Format("alert(""Stampa non effettuata. {0}"");", controlloFiltriResult.Message))

        Else

            Dim rptFilter As String = Biz.BizLotti.GetReportFilterStampaQuantitativiMovimentati(filtro)

            If reportName = Constants.ReportName.MagazzinoElencoMovimenti Then
                rptFilter = rptFilter + String.Format(" AND (isnull({{V_ANA_UTENTI.UTE_ID}}) OR {{V_ANA_UTENTI.UTE_APP_ID}} = '{0}') ", OnVacContext.AppId)
            End If

            Dim rpt As New ReportParameter()

            ' Filtro date registrazione
            If filtro.DataInizioRegistrazione = Date.MinValue Then
                rpt.AddParameter("FiltroDataInizioRegistrazione", String.Empty)
            Else
                rpt.AddParameter("FiltroDataInizioRegistrazione", filtro.DataInizioRegistrazione.ToString("dd/MM/yyyy"))
            End If

            If filtro.DataFineRegistrazione = Date.MinValue Then
                rpt.AddParameter("FiltroDataFineRegistrazione", String.Empty)
            Else
                rpt.AddParameter("FiltroDataFineRegistrazione", filtro.DataFineRegistrazione.ToString("dd/MM/yyyy"))
            End If

            ' Filtro utente
            If String.IsNullOrEmpty(filtro.CodiceUtente) Then
                rpt.AddParameter("FiltroUtente", String.Empty)
            Else
                rpt.AddParameter("FiltroUtente", String.Format("{0} [{1}]", filtro.DescrizioneUtente, filtro.CodiceUtente))
            End If

            ' Filtro consultorio
            If String.IsNullOrEmpty(filtro.CodiceConsultorio) Then
                rpt.AddParameter("FiltroCentroVaccinale", String.Empty)
            Else
                rpt.AddParameter("FiltroCentroVaccinale", String.Format("{0} [{1}]", filtro.DescrizioneConsultorio, filtro.CodiceConsultorio))
            End If

            ' Filtro lotto
            If String.IsNullOrEmpty(filtro.CodiceLotto) Then
                rpt.AddParameter("FiltroLotto", String.Empty)
            Else
                rpt.AddParameter("FiltroLotto", String.Format("{0} - {1}", filtro.CodiceLotto, filtro.DescrizioneLotto))
            End If

            ' Filtro quantita
            If String.IsNullOrEmpty(filtro.Quantita) Then
                rpt.AddParameter("FiltroQuantita", String.Empty)
            Else
                rpt.AddParameter("FiltroQuantita", String.Format("{0} {1}", filtro.OperatoreConfrontoQuantita, filtro.Quantita))
            End If

            ' Filtro tipo movimento
            If String.IsNullOrEmpty(filtro.TipoMovimento) Then
                rpt.AddParameter("FiltroTipoMovimento", String.Empty)
            Else
                rpt.AddParameter("FiltroTipoMovimento", Biz.BizLotti.GetDescrizioneMovimentoMagazzinoByTipo(filtro.TipoMovimento).ToUpper())
            End If

            ' Visualizzazione anteprima
            If Not OnVacReport.StampaReport(reportName, rptFilter, rpt, Nothing, Nothing, MagazzinoUtility.GetCartellaReport(reportName)) Then

                OnVacUtility.StampaNonPresente(Me.Page, reportName)

            End If

        End If

    End Sub

#End Region

End Class