Imports System.Collections.Generic

Public Class LogEventi
    Inherits OnVac.Common.PageBase

#Region " Properties "

    Private Property FiltriRicerca() As Biz.BizLog.LogDatiVaccinaliFilter
        Get
            Return Session("FiltriRicercaLogDatiVaccinali")
        End Get
        Set(value As Biz.BizLog.LogDatiVaccinaliFilter)
            Session("FiltriRicercaLogDatiVaccinali") = value
        End Set
    End Property

#End Region

#Region " Eventi Pagina "

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then

            LoadArgomenti()

            BindOperazioni()

            SetLabelRisultati(-1)

            Me.dgrLog.SelectedIndex = -1

            SetFiltriRicerca()

        End If

    End Sub

#End Region

#Region " Eventi Toobar "

    Private Sub ToolBar_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case be.Button.Key

            Case "btnCerca"

                Cerca(0)

            Case "btnPulisci"

                ClearFiltersAndResults()

            Case "btnStampaElencoLog"

                StampaElencoLog()

        End Select

    End Sub

#End Region

#Region " Eventi Datagrid "

    Private Sub dgrLog_ItemCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgrLog.ItemCommand

        Select Case e.CommandName

            Case "DatiPaziente"

                Dim codicePaziente As String = DirectCast(e.Item.FindControl("lblCodicePaziente"), Label).Text

                Me.UltimoPazienteSelezionato = New Entities.UltimoPazienteSelezionato(String.Empty, codicePaziente)

                Me.RedirectToGestionePaziente(codicePaziente)

            Case "ShowDettaglioLog"

                ShowDettaglioLog(e.Item)

        End Select

    End Sub

    Private Sub dgrLog_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrLog.ItemDataBound

        Select Case e.Item.ItemType

            Case ListItemType.AlternatingItem, ListItemType.EditItem, ListItemType.Item, ListItemType.SelectedItem

                Dim imgRisultato As System.Web.UI.WebControls.Image = DirectCast(e.Item.FindControl("imgRisultato"), System.Web.UI.WebControls.Image)

                Dim logDatiVaccinali As Entities.LogDatiVaccinali = DirectCast(e.Item.DataItem, Entities.LogDatiVaccinali)

                Select Case logDatiVaccinali.Stato

                    Case Enumerators.StatoLogDatiVaccinaliCentrali.Error

                        imgRisultato.ImageUrl = Me.ResolveClientUrl("~/images/deny.png")

                    Case Enumerators.StatoLogDatiVaccinaliCentrali.Success

                        imgRisultato.ImageUrl = Me.ResolveClientUrl("~/images/success.png")

                    Case Enumerators.StatoLogDatiVaccinaliCentrali.Warning

                        imgRisultato.ImageUrl = Me.ResolveClientUrl("~/images/alert.gif")

                End Select

        End Select

    End Sub

    Private Sub dgrLog_PageIndexChanged(source As Object, e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dgrLog.PageIndexChanged

        Me.Cerca(e.NewPageIndex)

    End Sub

#End Region

#Region " Private "

    Private Sub LoadArgomenti()

        Dim listArgomentiLog As List(Of DataLogStructure.Argomento) = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizLog As New Biz.BizLog(genericProvider, OnVacContext.CreateBizContextInfos())

                listArgomentiLog = bizLog.GetListArgomenti(New String() {DataLogStructure.TipiArgomento.VAC_ESEGUITE,
                                                                         DataLogStructure.TipiArgomento.VAC_SCADUTE,
                                                                         DataLogStructure.TipiArgomento.REAZ_AVVERSE,
                                                                         DataLogStructure.TipiArgomento.REAZ_AVVERSE_SCADUTE,
                                                                         DataLogStructure.TipiArgomento.VAC_ESCLUSE,
                                                                         DataLogStructure.TipiArgomento.VISITE,
                                                                         DataLogStructure.TipiArgomento.GEST_BIL,
                                                                         DataLogStructure.TipiArgomento.VAC_PROGRAMMATE,
                                                                         DataLogStructure.TipiArgomento.MERGEPAZIENTI})

            End Using
        End Using

        listArgomentiLog.Insert(0, New DataLogStructure.Argomento(String.Empty))

        Me.ddlArgomento.DataSource = listArgomentiLog
        Me.ddlArgomento.DataBind()

    End Sub

    Private Sub BindOperazioni()

        Dim listOperazioni As New List(Of KeyValuePair(Of String, String))()

        listOperazioni.Add(GetOperazioneToBind(Nothing))
        listOperazioni.Add(GetOperazioneToBind(DataLogStructure.Operazione.Inserimento))
        listOperazioni.Add(GetOperazioneToBind(DataLogStructure.Operazione.Eliminazione))
        listOperazioni.Add(GetOperazioneToBind(DataLogStructure.Operazione.Modifica))
        listOperazioni.Add(GetOperazioneToBind(DataLogStructure.Operazione.Generico))
        'listOperazioni.Add(GetOperazioneToBind(DataLogStructure.Operazione.Eccezione))

        Me.ddlTipoOperazione.DataSource = listOperazioni
        Me.ddlTipoOperazione.DataBind()

    End Sub

    Private Function GetOperazioneToBind(operazioneLog As DataLogStructure.Operazione?) As KeyValuePair(Of String, String)

        If Not operazioneLog.HasValue Then Return New KeyValuePair(Of String, String)(String.Empty, String.Empty)

        Return New KeyValuePair(Of String, String)(operazioneLog.Value, operazioneLog.Value.ToString())

    End Function

    Private Sub ClearFiltersAndResults()

        Me.dpkDataOperazioneDa.Text = String.Empty
        Me.dpkDataOperazioneA.Text = String.Empty

        Me.ddlTipoOperazione.ClearSelection()
        Me.ddlArgomento.ClearSelection()

        Me.rdbRisultatoSuccesso.Checked = False
        Me.rdbRisultatoWarning.Checked = False
        Me.rdbRisultatoErrore.Checked = False
        Me.rdbRisultatoTutti.Checked = True

        Me.txtCognome.Text = String.Empty
        Me.txtNome.Text = String.Empty
        Me.txtCodiceFiscale.Text = String.Empty

        Me.dpkDataNascitaDa.Text = String.Empty
        Me.dpkDataNascitaA.Text = String.Empty

        Me.fmConsultorio.Codice = String.Empty
        Me.fmConsultorio.Descrizione = String.Empty
        Me.fmConsultorio.RefreshDataBind()

        ' Ricarica gli stati anagrafici per preimpostare quelli configurati
        Me.ucStatiAnagrafici.LoadStatiAnagrafici()

        ' Aggiorno la struttura con i dati dei campi
        Me.FiltriRicerca = GetFiltriRicerca()

        Me.SetLabelRisultati(-1)

        Me.BindDatagrid(Nothing, 0, 0)

        Me.dgrLog.SelectedIndex = -1

    End Sub

    ' Controllo filtri (esclusi risultato e stati anagrafici): se almeno un filtro è valorizzato restituisce true, altrimenti false.
    Private Function CheckFiltriRicercaValorizzati() As Boolean

        If Not String.IsNullOrEmpty(Me.ddlArgomento.SelectedValue) Then Return True

        If Not String.IsNullOrEmpty(Me.dpkDataOperazioneDa.Text) Then Return True
        If Not String.IsNullOrEmpty(Me.dpkDataOperazioneA.Text) Then Return True

        If Not String.IsNullOrEmpty(Me.ddlTipoOperazione.SelectedValue) Then Return True

        If Not String.IsNullOrEmpty(Me.txtCognome.Text) Then Return True
        If Not String.IsNullOrEmpty(Me.txtNome.Text) Then Return True
        If Not String.IsNullOrEmpty(Me.txtCodiceFiscale.Text) Then Return True

        If Not String.IsNullOrEmpty(Me.dpkDataNascitaDa.Text) Then Return True
        If Not String.IsNullOrEmpty(Me.dpkDataNascitaA.Text) Then Return True

        If Not String.IsNullOrEmpty(Me.fmConsultorio.Codice) Then Return True
        If Not String.IsNullOrEmpty(Me.fmConsultorio.Descrizione) Then Return True

        Return False

    End Function

    Private Sub Cerca(currentPageIndex As Integer)

        If Not Me.CheckFiltriRicercaValorizzati() Then

            Me.OnitLayout31.InsertRoutineJS("alert('Ricerca non effettuata: valorizzare almeno un filtro.');")
            Return

        End If

        Me.FiltriRicerca = GetFiltriRicerca()

        Dim logsDatiVaccinali As Entities.LogDatiVaccinali()

        Dim countRisultati As Integer = 0

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizLog As New Biz.BizLog(genericProvider, OnVacContext.CreateBizContextInfos())

                countRisultati = bizLog.CountLogsDatiVaccinali(Me.FiltriRicerca)

                Dim startIndex As Integer = currentPageIndex * Me.dgrLog.PageSize

                If startIndex > countRisultati - 1 Then
                    startIndex = 0
                    currentPageIndex = 0
                End If

                logsDatiVaccinali = bizLog.GetLogsDatiVaccinali(Me.FiltriRicerca, currentPageIndex, Me.dgrLog.PageSize)

            End Using

        End Using

        Me.BindDatagrid(logsDatiVaccinali, countRisultati, currentPageIndex)

        Me.SetLabelRisultati(countRisultati)

    End Sub

    Private Sub ShowDettaglioLog(currentDatagridItem As DataGridItem)

        Dim logDatiVaccinali As Entities.LogDatiVaccinali = Nothing

        Dim lblIdLog As Label = DirectCast(currentDatagridItem.FindControl("lblIdLog"), Label)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizLog As New Biz.BizLog(genericProvider, OnVacContext.CreateBizContextInfos())

                logDatiVaccinali = bizLog.GetLogDatiVaccinali(lblIdLog.Text)

            End Using

        End Using

        If logDatiVaccinali Is Nothing Then

            Me.OnitLayout31.InsertRoutineJS("alert('Nessun dato trovato per il record selezionato.');")

        Else

            Me.lblDettaglioCodiceAzienda.Text = logDatiVaccinali.Usl.Codice
            Me.lblDettaglioDescrizioneAzienda.Text = logDatiVaccinali.Usl.Descrizione

            Me.lblDettaglioCodiceUtente.Text = logDatiVaccinali.Utente.Codice
            Me.lblDettaglioDescrizioneUtente.Text = logDatiVaccinali.Utente.Descrizione

            Me.fmDettaglioLog.VisibileMD = True

        End If

    End Sub

    Private Sub StampaElencoLog()

        Me.FiltriRicerca = GetFiltriRicerca()

        Dim dstLogDatiVaccinali As New DstLogDatiVaccinali()

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizLog As New Biz.BizLog(genericProvider, OnVacContext.CreateBizContextInfos())

                Dim logsDatiVaccinali As Entities.LogDatiVaccinali() = bizLog.GetLogsDatiVaccinali(Me.FiltriRicerca)

                If Not logsDatiVaccinali Is Nothing AndAlso logsDatiVaccinali.Count > 0 Then

                    Dim rowLogDatiVaccinali As DstLogDatiVaccinali.LogDatiVaccinaliRow

                    For Each logDatiVaccinali As Entities.LogDatiVaccinali In logsDatiVaccinali

                        rowLogDatiVaccinali = dstLogDatiVaccinali.Tables("LogDatiVaccinali").NewRow()

                        rowLogDatiVaccinali("IdLog") = logDatiVaccinali.Id

                        rowLogDatiVaccinali("DataOperazione") = logDatiVaccinali.DataOperazione

                        rowLogDatiVaccinali("CodiceOperazione") = Convert.ToInt32(logDatiVaccinali.Operazione)
                        rowLogDatiVaccinali("DescrizioneOperazione") = logDatiVaccinali.Operazione.ToString()

                        rowLogDatiVaccinali("CodiceStatoOperazione") = Convert.ToInt32(logDatiVaccinali.Stato)
                        rowLogDatiVaccinali("DescrizioneStatoOperazione") = logDatiVaccinali.Stato.ToString()

                        rowLogDatiVaccinali("Note") = logDatiVaccinali.Note

                        rowLogDatiVaccinali("CodicePaziente") = logDatiVaccinali.Paziente.Paz_Codice
                        rowLogDatiVaccinali("CognomePaziente") = logDatiVaccinali.Paziente.PAZ_COGNOME
                        rowLogDatiVaccinali("NomePaziente") = logDatiVaccinali.Paziente.PAZ_NOME
                        rowLogDatiVaccinali("DataNascitaPaziente") = logDatiVaccinali.Paziente.Data_Nascita
                        rowLogDatiVaccinali("CodiceConsultorioPaziente") = logDatiVaccinali.Paziente.Paz_Cns_Codice
                        If logDatiVaccinali.Paziente.StatoAnagrafico.HasValue Then
                            rowLogDatiVaccinali("CodiceStatoAnagraficoPaziente") = Convert.ToInt32(logDatiVaccinali.Paziente.StatoAnagrafico)
                            rowLogDatiVaccinali("DescrizioneStatoAnagraficoPaziente") = logDatiVaccinali.Paziente.StatoAnagrafico.ToString().Replace("_", " ")
                        End If

                        rowLogDatiVaccinali("CodiceUsl") = logDatiVaccinali.Usl.Codice
                        rowLogDatiVaccinali("DescrizioneUsl") = logDatiVaccinali.Usl.Descrizione

                        rowLogDatiVaccinali("IdUtente") = logDatiVaccinali.Utente.Id
                        rowLogDatiVaccinali("CodiceUtente") = logDatiVaccinali.Utente.Codice
                        rowLogDatiVaccinali("DescrizioneUtente") = logDatiVaccinali.Utente.Descrizione

                        rowLogDatiVaccinali("CodiceArgomento") = logDatiVaccinali.Argomento.Codice
                        rowLogDatiVaccinali("DescrizioneArgomento") = logDatiVaccinali.Argomento.Descrizione

                        dstLogDatiVaccinali.Tables("LogDatiVaccinali").Rows.Add(rowLogDatiVaccinali)

                    Next

                End If

            End Using

        End Using

        If dstLogDatiVaccinali.Tables("LogDatiVaccinali") Is Nothing OrElse dstLogDatiVaccinali.Tables("LogDatiVaccinali").Rows.Count = 0 Then

            Me.OnitLayout31.InsertRoutineJS("alert('Stampa non effettuata: nessun dato da stampare.');")
            Return

        End If

        Dim rpt As New ReportParameter()

        rpt.set_dataset(dstLogDatiVaccinali)

        rpt.AddParameter("DescrizioneConsultorioCorrente", OnVacUtility.Variabili.CNS.Descrizione)

        If Not OnVacReport.StampaReport(Constants.ReportName.ElencoLogDatiVaccinali, String.Empty, rpt, Nothing, Nothing, MagazzinoUtility.GetCartellaReport(Constants.ReportName.ElencoLogDatiVaccinali)) Then

            OnVacUtility.StampaNonPresente(Me.Page, Constants.ReportName.ElencoLogDatiVaccinali)

        End If

    End Sub

    Private Function GetFiltriRicerca() As Biz.BizLog.LogDatiVaccinaliFilter

        Dim logDatiVaccinaliFilter As New Biz.BizLog.LogDatiVaccinaliFilter()

        ' --- Filtri operazione ---'

        logDatiVaccinaliFilter.CodiceArgomento = Me.ddlArgomento.SelectedValue

        If Not String.IsNullOrEmpty(Me.dpkDataOperazioneDa.Text) Then logDatiVaccinaliFilter.DataOperazioneMinima = Me.dpkDataOperazioneDa.Data
        If Not String.IsNullOrEmpty(Me.dpkDataOperazioneA.Text) Then logDatiVaccinaliFilter.DataOperazioneMassima = Me.dpkDataOperazioneA.Data

        If Not String.IsNullOrEmpty(Me.ddlTipoOperazione.SelectedValue) Then logDatiVaccinaliFilter.Operazione = Me.ddlTipoOperazione.SelectedValue

        If Me.rdbRisultatoErrore.Checked Then
            logDatiVaccinaliFilter.StatoOperazione = Enumerators.StatoLogDatiVaccinaliCentrali.Error
        ElseIf Me.rdbRisultatoSuccesso.Checked Then
            logDatiVaccinaliFilter.StatoOperazione = Enumerators.StatoLogDatiVaccinaliCentrali.Success
        ElseIf Me.rdbRisultatoWarning.Checked Then
            logDatiVaccinaliFilter.StatoOperazione = Enumerators.StatoLogDatiVaccinaliCentrali.Warning
        End If

        ' --- Filtri paziente ---'

        If Not String.IsNullOrEmpty(Me.txtCognome.Text) Then
            Me.txtCognome.Text = Me.txtCognome.Text.Trim().ToUpper()
            logDatiVaccinaliFilter.CognomePaziente = Me.txtCognome.Text
        End If

        If Not String.IsNullOrEmpty(Me.txtNome.Text) Then
            Me.txtNome.Text = Me.txtNome.Text.Trim().ToUpper()
            logDatiVaccinaliFilter.NomePaziente = Me.txtNome.Text
        End If

        If Not String.IsNullOrEmpty(Me.txtCodiceFiscale.Text) Then
            Me.txtCodiceFiscale.Text = Me.txtCodiceFiscale.Text.Trim().ToUpper()
            logDatiVaccinaliFilter.CodiceFiscalePaziente = Me.txtCodiceFiscale.Text
        End If

        If Not String.IsNullOrEmpty(Me.dpkDataNascitaDa.Text) Then logDatiVaccinaliFilter.DataNascitaPazienteMinima = Me.dpkDataNascitaDa.Data
        If Not String.IsNullOrEmpty(Me.dpkDataNascitaA.Text) Then logDatiVaccinaliFilter.DataNascitaPazienteMassima = Me.dpkDataNascitaA.Data

        logDatiVaccinaliFilter.CodiceCentroVaccinalePaziente = Me.fmConsultorio.Codice
        logDatiVaccinaliFilter.DescrizioneCentroVaccinalePaziente = Me.fmConsultorio.Descrizione

        logDatiVaccinaliFilter.StatiAnagraficiPaziente = Me.ucStatiAnagrafici.GetStatiAnagraficiSelezionati()

        Return logDatiVaccinaliFilter

    End Function

    Private Sub SetFiltriRicerca()

        If Not Me.FiltriRicerca Is Nothing Then

            ' --- Filtri operazione ---'

            Me.ddlArgomento.SelectedValue = Me.FiltriRicerca.CodiceArgomento

            If Me.FiltriRicerca.DataOperazioneMinima.HasValue Then Me.dpkDataOperazioneDa.Data = Me.FiltriRicerca.DataOperazioneMinima.Value
            If Me.FiltriRicerca.DataOperazioneMassima.HasValue Then Me.dpkDataOperazioneA.Data = Me.FiltriRicerca.DataOperazioneMassima.Value

            If Me.FiltriRicerca.Operazione.HasValue Then Me.ddlTipoOperazione.SelectedValue = Me.FiltriRicerca.Operazione

            Me.rdbRisultatoTutti.Checked = False
            Me.rdbRisultatoErrore.Checked = False
            Me.rdbRisultatoSuccesso.Checked = False
            Me.rdbRisultatoWarning.Checked = False

            If Not Me.FiltriRicerca.StatoOperazione.HasValue Then

                Me.rdbRisultatoTutti.Checked = True

            Else

                Select Case Me.FiltriRicerca.StatoOperazione
                    Case Enumerators.StatoLogDatiVaccinaliCentrali.Error
                        Me.rdbRisultatoErrore.Checked = True
                    Case Enumerators.StatoLogDatiVaccinaliCentrali.Success
                        Me.rdbRisultatoSuccesso.Checked = True
                    Case Enumerators.StatoLogDatiVaccinaliCentrali.Warning
                        Me.rdbRisultatoWarning.Checked = True
                End Select

            End If

            ' --- Filtri paziente ---'

            If Not String.IsNullOrEmpty(Me.FiltriRicerca.CognomePaziente) Then
                Me.txtCognome.Text = Me.FiltriRicerca.CognomePaziente
            End If

            If Not String.IsNullOrEmpty(Me.FiltriRicerca.NomePaziente) Then
                Me.txtNome.Text = Me.FiltriRicerca.NomePaziente
            End If

            If Not String.IsNullOrEmpty(Me.FiltriRicerca.CodiceFiscalePaziente) Then
                Me.txtCodiceFiscale.Text = Me.FiltriRicerca.CodiceFiscalePaziente
            End If

            If Me.FiltriRicerca.DataNascitaPazienteMinima.HasValue Then Me.dpkDataNascitaDa.Data = Me.FiltriRicerca.DataNascitaPazienteMinima.Value
            If Me.FiltriRicerca.DataNascitaPazienteMassima.HasValue Then Me.dpkDataNascitaA.Data = Me.FiltriRicerca.DataNascitaPazienteMassima.Value

            Me.fmConsultorio.Codice = Me.FiltriRicerca.CodiceCentroVaccinalePaziente
            Me.fmConsultorio.Descrizione = Me.FiltriRicerca.DescrizioneCentroVaccinalePaziente
            Me.fmConsultorio.RefreshDataBind()

            Dim listStatiAnagrafici As New List(Of String)()

            If Not Me.FiltriRicerca.StatiAnagraficiPaziente Is Nothing AndAlso Me.FiltriRicerca.StatiAnagraficiPaziente.Count > 0 Then

                For Each statoAnagrafico As Enumerators.StatoAnagrafico In Me.FiltriRicerca.StatiAnagraficiPaziente
                    listStatiAnagrafici.Add(Convert.ToInt16(statoAnagrafico).ToString())
                Next

            End If

            Me.ucStatiAnagrafici.SetStatiAnagrafici(listStatiAnagrafici)

        End If

    End Sub

    Private Sub BindDatagrid(results() As Entities.LogDatiVaccinali, countRisultati As Integer, currentPageIndex As Integer)

        Me.dgrLog.VirtualItemCount = countRisultati
        Me.dgrLog.CurrentPageIndex = currentPageIndex

        Me.dgrLog.DataSource = results
        Me.dgrLog.DataBind()

        Me.dgrLog.SelectedIndex = -1

    End Sub

    Private Sub SetLabelRisultati(countRisultati As Integer)

        Dim msg As String

        Select Case countRisultati

            Case -1
                msg = String.Empty

            Case 0
                msg = ": nessun record trovato"

            Case 1
                msg = ": 1 record trovato"

            Case Else
                msg = String.Format(": {0} record trovati", countRisultati.ToString())

        End Select

        Me.lblRisultati.Text = String.Format("Risultati della ricerca{0}", msg)

    End Sub

#End Region

End Class