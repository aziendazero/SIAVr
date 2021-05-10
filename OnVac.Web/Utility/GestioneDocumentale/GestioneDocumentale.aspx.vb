Imports System.Collections.Generic
Imports System.Web.Script.Serialization
Imports Onit.OnAssistnet.OnVac.FirmaDigitaleArchiviazioneSostitutiva

Public Class GestioneDocumentale
    Inherits Common.PageBase

#Region " Constants "

    Private CONFIRM_FIRMA As String = "CF"

#End Region

#Region " Properties "

    ''' <summary>
    ''' Lista contenente gli id delle visite selezionate per la firma/archiviazione
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Property IdVisiteSelezionate() As List(Of Long)
        Get
            If ViewState("IDVIS") Is Nothing Then ViewState("IDVIS") = New List(Of Long)()
            Return DirectCast(ViewState("IDVIS"), List(Of Long))
        End Get
        Set(value As List(Of Long))
            ViewState("IDVIS") = value
        End Set
    End Property

    Private Property CampoOrdinamento As String
        Get
            If ViewState("CampoOrd") Is Nothing Then ViewState("CampoOrd") = String.Empty 'Me.dgrDocumenti.Columns(dgrDocumentiColumnIndex.DataVisita).SortExpression
            Return ViewState("CampoOrd").ToString()
        End Get
        Set(value As String)
            ViewState("CampoOrd") = value
        End Set
    End Property

    Private Property VersoOrdinamento As String
        Get
            If ViewState("VersoOrd") Is Nothing Then ViewState("VersoOrd") = String.Empty
            Return ViewState("VersoOrd").ToString()
        End Get
        Set(value As String)
            ViewState("VersoOrd") = value
        End Set
    End Property

    ''' <summary>
    ''' Filtri impostati per la ricerca corrente
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property FiltriRicercaCorrente As Entities.ArchiviazioneDIRV.FiltriDocumentiVisite
        Get
            If ViewState("FR") Is Nothing Then ViewState("FR") = New Entities.ArchiviazioneDIRV.FiltriDocumentiVisite()
            Return DirectCast(ViewState("FR"), Entities.ArchiviazioneDIRV.FiltriDocumentiVisite)
        End Get
        Set(value As Entities.ArchiviazioneDIRV.FiltriDocumentiVisite)
            ViewState("FR") = value
        End Set
    End Property

    ''' <summary>
    ''' Indica se il checkbox "Seleziona tutti" è selezionato o deselezionato
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property SelezionaTuttiChecked As Boolean
        Get
            If ViewState("CHKALL") Is Nothing Then ViewState("CHKALL") = False
            Return DirectCast(ViewState("CHKALL"), Boolean)
        End Get
        Set(value As Boolean)
            ViewState("CHKALL") = value
        End Set
    End Property

    ''' <summary>
    ''' Messaggio relativo ai controlli pre-firma sulle visite selezionate.
    ''' Verrà visualizzato al termine della procedura di firma, assieme al messaggio restituito dalla procedura stessa.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property MessaggioRisultatoControlli As String
        Get
            If ViewState("MSG") Is Nothing Then ViewState("MSG") = String.Empty
            Return ViewState("MSG").ToString()
        End Get
        Set(value As String)
            ViewState("MSG") = value
        End Set
    End Property

#End Region

#Region " Enum "

    Private Enum dgrDocumentiColumnIndex
        Seleziona = 0
        CampiNascosti = 1
        Paziente = 2
        DataVisita = 3
        UtenteVisita = 4
        UtenteFirma = 5
        UtenteRilevatore = 6
        Stato = 7
    End Enum

#End Region

#Region " Page Events "

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        If Not IsPostBack Then

            ResetFilters()

            omlRilevatore.Filtro = OnVacUtility.GetModalListFilterOperatori(False, True)

        End If

        Select Case Request.Form.Item("__EVENTTARGET")

            Case "selectAll"

                SelectAll()

        End Select

    End Sub

    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        SetImmagineOrdinamento()

    End Sub

#End Region

#Region " Controls Events "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btnCerca"

                RicercaDocumenti()

            Case "btnFirma"

                MessaggioRisultatoControlli = String.Empty

                SetIdVisiteSelezionate()

                If IdVisiteSelezionate.Count = 0 Then
                    ShowMessage(Biz.BizFirmaDigitale.Messages.NO_DOCUMENTI_SELEZIONATI)
                Else
                    OnitLayout31.ShowMsgBox(New Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(
                                            Biz.BizFirmaDigitale.Messages.CONFIRM_FIRMA_MULTIPLA, CONFIRM_FIRMA, True, True))
                End If

            Case "btnPulisci"

                ResetFilters()

        End Select

    End Sub

    Private Sub omlUtenteRegistrazione_SetUpFiletr(sender As Object) Handles omlUtenteRegistrazione.SetUpFiletr

        omlUtenteRegistrazione.Filtro = OnVacUtility.GetFiltroUtenteForOnitModalList(False)

    End Sub

    Private Sub omlUtenteFirma_SetUpFiletr(sender As Object) Handles omlUtenteFirma.SetUpFiletr

        omlUtenteFirma.Filtro = OnVacUtility.GetFiltroUtenteForOnitModalList(False)

    End Sub

    Private Sub OnitLayout31_ConfirmClick(sender As Object, e As Controls.PagesLayout.OnitLayout3.ConfirmEventArgs) Handles OnitLayout31.ConfirmClick

        If e.Key = CONFIRM_FIRMA Then

            If e.Result Then
                FirmaDigitale()
            End If

        End If

    End Sub

#Region " Datagrid "

    Private Sub dgrDocumenti_ItemDataBound(sender As Object, e As DataGridItemEventArgs) Handles dgrDocumenti.ItemDataBound

        Select Case e.Item.ItemType

            Case ListItemType.Header

                DirectCast(e.Item.FindControl("chkSelezioneHeader"), HtmlInputCheckBox).Checked = SelezionaTuttiChecked

            Case ListItemType.AlternatingItem, ListItemType.EditItem, ListItemType.Item, ListItemType.SelectedItem

                ' Gestione checkbox selezione
                If IdVisiteSelezionate.Count > 0 Then

                    Dim chkSelezioneItem As CheckBox = DirectCast(e.Item.FindControl("chkSelezioneItem"), CheckBox)
                    If Not chkSelezioneItem Is Nothing Then

                        Dim idVisita As Long = Convert.ToInt64(HttpUtility.HtmlDecode(DirectCast(e.Item.FindControl("hidIdVisita"), HiddenField).Value))
                        chkSelezioneItem.Checked = IdVisiteSelezionate.Contains(idVisita)

                    End If
                End If

                ' Gestione flag firma/archiviazione
                Dim btnFlagFirma As ImageButton = DirectCast(e.Item.FindControl("btnFlagFirma"), ImageButton)

                If Not btnFlagFirma Is Nothing Then

                    Dim hidUteFirma As HiddenField = DirectCast(e.Item.FindControl("hidUteFirma"), HiddenField)
                    Dim hidUteArchiviazione As HiddenField = DirectCast(e.Item.FindControl("hidUteArchiviazione"), HiddenField)

                    If Not hidUteFirma Is Nothing AndAlso Not hidUteArchiviazione Is Nothing Then

                        Dim flagFirma As String = HttpUtility.HtmlDecode(hidUteFirma.Value)
                        Dim flagArchiviazione As String = HttpUtility.HtmlDecode(hidUteArchiviazione.Value)

                        If String.IsNullOrEmpty(flagFirma) Then
                            ' NO firma, NO archiviazione
                            btnFlagFirma.Visible = False
                        Else
                            ' SI firma
                            btnFlagFirma.Visible = True
                            btnFlagFirma.ImageUrl = OnVacUtility.GetFlagFirmaImageUrlValue(flagFirma, flagArchiviazione, Me.Page)
                            btnFlagFirma.ToolTip = OnVacUtility.GetFlagFirmaToolTipValue(flagFirma, flagArchiviazione)
                        End If
                    End If
                End If

        End Select

    End Sub

    Private Sub dgrDocumenti_ItemCommand(source As Object, e As DataGridCommandEventArgs) Handles dgrDocumenti.ItemCommand

        Select Case e.CommandName

            Case "InfoArchiviazione"

                ' Click pulsante flag firma/archiviazione
                Dim idVisitaCorrente As String = DirectCast(e.Item.FindControl("hidIdVisita"), HiddenField).Value

                If String.IsNullOrWhiteSpace(idVisitaCorrente) Then
                    OnitLayout31.InsertRoutineJS(String.Format("alert(""{0}"");", "Nessuna visita selezionata."))
                Else
                    Dim codiceAziendaInserimento As String = DirectCast(e.Item.FindControl("hidCodAzInserimento"), HiddenField).Value

                    ucInfoFirma.SetInfoFirmaDigitaleArchiviazioneSostitutiva(Convert.ToInt64(idVisitaCorrente), codiceAziendaInserimento)
                    fmInfoArchiviazione.VisibileMD = True
                End If

        End Select

    End Sub

    Private Sub dgrDocumenti_SortCommand(source As Object, e As DataGridSortCommandEventArgs) Handles dgrDocumenti.SortCommand

        SetIdVisiteSelezionate()

        If e.SortExpression = CampoOrdinamento Then
            If VersoOrdinamento = "ASC" Then
                VersoOrdinamento = "DESC"
            Else
                VersoOrdinamento = "ASC"
            End If
        Else
            CampoOrdinamento = e.SortExpression
            VersoOrdinamento = "ASC"
        End If

        CaricaDati(dgrDocumenti.CurrentPageIndex, FiltriRicercaCorrente)

    End Sub

    Private Sub dgrDocumenti_PageIndexChanged(source As Object, e As DataGridPageChangedEventArgs) Handles dgrDocumenti.PageIndexChanged

        SetIdVisiteSelezionate()

        CaricaDati(e.NewPageIndex, FiltriRicercaCorrente)

    End Sub

#End Region

#End Region

#Region " Private "

#Region " Controlli sulla visita "

    Private Class ControlloVisitaResult
        Public Success As Boolean
        Public Message As String
    End Class

    Private Function ControlloVisitaSelezionata() As ControlloVisitaResult

        Dim result As New ControlloVisitaResult()

        'Recupero le info per il controllo sulla visita selezionata
        Dim codiceUslInserimento As String = GetCodiceUslInserimentoVisitaSelezionata()
        Dim dataRegistrazione As DateTime = GetDataRegistrazioneVisitaSelezionata()

        ' ------------------------------------ '
        ' [Unificazione Ulss]: Eliminato controllo usl inserimento
        ' ------------------------------------ '
        'Dim gestioneStoricoVacc As New Common.OnVacStoricoVaccinaleCentralizzato(Settings)

        'If Me.FlagAbilitazioneVaccUslCorrente AndAlso Not gestioneStoricoVacc.CheckUslInserimentoCorrente(codiceUslInserimento) Then

        '    ' Controllo usl inserimento visita (se viene gestito lo storico vaccinale centralizzato)
        '    result.Message = Common.OnVacStoricoVaccinaleCentralizzato.AlertMessageModificaVisitaNoUslCorrente
        '    result.Success = False

        'Else
        ' ------------------------------------ '

        If Not OnVacUtility.CheckGiorniTrascorsiVariazioneDatiVaccinali(dataRegistrazione, Settings) Then

            ' Controllo giorni trascorsi da modifica (indipendentemente dalla gestione dello storico vaccinale centralizzato)
            result.Message = String.Format("alert('Il numero di giorni trascorsi dalla data di registrazione del bilancio è superiore al limite massimo impostato ({0}): impossibile effettuare la modifica.');",
                                           Settings.GIORNI_VARIAZIONE_VACCINAZIONE_ESEGUITA.ToString())

            result.Success = False

        Else

            ' Controllo utente: se l'utente è un medico può modificare il bilancio solo se sono trascorsi meno giorni di quelli parametrizzati.
            If UtenteLoggatoIsMedico AndAlso Date.Now.Subtract(dataRegistrazione).Days >= Settings.GIORNI_MODIFICA_BILANCIO_MEDICO Then

                result.Message = String.Format("alert('Attenzione, sono passati più di {0} giorni dalla data di registrazione del bilancio. Contattare il centro vaccinale per la modifica.');",
                                               Settings.GIORNI_MODIFICA_BILANCIO_MEDICO)

                result.Success = False

            Else
                result.Success = True
            End If

        End If

        Return result

    End Function

    ''' <summary>
    ''' Restituisce il valore del flag firma, per la visita selezionata
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetFlagFirmaDigitaleVisitaSelezionata() As Boolean

        Dim uteFirma As String = GetHiddenValueVisitaSelezionata("hidUteFirma")

        If String.IsNullOrWhiteSpace(uteFirma) Then
            Return False
        Else
            Return True
        End If

    End Function

    Private Function GetCodiceUslInserimentoVisitaSelezionata() As String

        Return GetHiddenValueVisitaSelezionata("hidCodAzInserimento")

    End Function

    Private Function GetDataRegistrazioneVisitaSelezionata() As DateTime

        Dim dataReg As String = GetHiddenValueVisitaSelezionata("hidDataRegistrazione")

        If String.IsNullOrWhiteSpace(dataReg) Then
            Return DateTime.MinValue
        Else
            Return DateTime.Parse(dataReg)
        End If

    End Function

#End Region

    Private Sub RicercaDocumenti()

        CampoOrdinamento = Nothing
        VersoOrdinamento = Nothing

        IdVisiteSelezionate = New List(Of Long)()
        SelezionaTuttiChecked = False

        CaricaDati(0)

    End Sub

    Private Sub CaricaDati(currentPageIndex As Integer)

        CaricaDati(currentPageIndex, GetFiltriRicerca())

    End Sub

    Private Sub CaricaDati(currentPageIndex As Integer, filtri As Entities.ArchiviazioneDIRV.FiltriDocumentiVisite)

        If filtri Is Nothing Then filtri = GetFiltriRicerca()

        ' Check filtri obbligatori
        If filtri.DataDa = DateTime.MinValue OrElse filtri.DataA = DateTime.MinValue Then
            ShowMessage("Inserire tutti i filtri data per proseguire con la ricerca.")
            Return
        End If

        ' Impostazione filtri correnti
        FiltriRicercaCorrente = filtri.Clone()

        Dim result As Biz.BizFirmaDigitale.GetDocumentiVisiteResult = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizDoc As New Biz.BizFirmaDigitale(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                Dim paramToPass As New Biz.BizFirmaDigitale.ParametriGetDocumentiVisiteBiz()
                paramToPass.Filtri = FiltriRicercaCorrente
                paramToPass.PageIndex = currentPageIndex
                paramToPass.PageSize = dgrDocumenti.PageSize
                paramToPass.CampoOrdinamento = CampoOrdinamento
                paramToPass.VersoOrdinamento = VersoOrdinamento

                result = bizDoc.GetDocumentiVisite(paramToPass)

            End Using
        End Using

        dgrDocumenti.VirtualItemCount = result.CountDocumenti
        dgrDocumenti.CurrentPageIndex = currentPageIndex

        dgrDocumenti.SelectedIndex = -1
        dgrDocumenti.DataSource = result.DocumentiVisite
        dgrDocumenti.DataBind()

    End Sub

    Private Function GetHiddenValueVisitaSelezionata(idHidControl As String) As String

        If dgrDocumenti.SelectedIndex = -1 Then Return Nothing

        Dim hid As HiddenField = DirectCast(dgrDocumenti.SelectedItem.FindControl(idHidControl), HiddenField)
        If hid Is Nothing Then Return Nothing

        If Not String.IsNullOrEmpty(HttpUtility.HtmlDecode(hid.Value)) Then Return HttpUtility.HtmlDecode(hid.Value)

        Return String.Empty

    End Function

    Private Sub ResetFilters()

        omlUtenteRegistrazione.ValoriAltriCampi("Id") = String.Empty
        omlUtenteRegistrazione.Codice = String.Empty
        omlUtenteRegistrazione.Descrizione = String.Empty

        omlUtenteFirma.ValoriAltriCampi("Id") = String.Empty
        omlUtenteFirma.Codice = String.Empty
        omlUtenteFirma.Descrizione = String.Empty

        omlRilevatore.ValoriAltriCampi("Id") = String.Empty
        omlRilevatore.Codice = String.Empty
        omlRilevatore.Descrizione = String.Empty

        'Set filtri date (ultimi 15 giorni)
        Dim data As DateTime = DateTime.Today
        dpkFiltroDataCompilazioneA.Data = data
        dpkFiltroDataCompilazioneDa.Data = data.AddDays(-15)

        rdbFiltroStatoDaFirmare.Checked = True
        rdbFiltroStatoFirmNoArc.Checked = False
        rdbFiltroStatoFirmArc.Checked = False
        rdbFiltroStatoTutti.Checked = False

        dgrDocumenti.SelectedIndex = -1
        dgrDocumenti.DataSource = Nothing
        dgrDocumenti.DataBind()

        IdVisiteSelezionate = New List(Of Long)()
        FiltriRicercaCorrente = New Entities.ArchiviazioneDIRV.FiltriDocumentiVisite()
        SelezionaTuttiChecked = False
        MessaggioRisultatoControlli = String.Empty

    End Sub

    Private Function GetFiltriRicerca() As Entities.ArchiviazioneDIRV.FiltriDocumentiVisite

        Dim filtri As New Entities.ArchiviazioneDIRV.FiltriDocumentiVisite()

        filtri.DataDa = dpkFiltroDataCompilazioneDa.Data
        filtri.DataA = dpkFiltroDataCompilazioneA.Data

        'Filtro utente registrazione
        If Not String.IsNullOrWhiteSpace(omlUtenteRegistrazione.ValoriAltriCampi("Id")) Then
            filtri.IdUtenteRegistrazione = Long.Parse(omlUtenteRegistrazione.ValoriAltriCampi("Id"))
        End If

        'Filtro utente firma
        If Not String.IsNullOrWhiteSpace(omlUtenteFirma.ValoriAltriCampi("Id")) Then
            filtri.IdUtenteFirma = Long.Parse(omlUtenteFirma.ValoriAltriCampi("Id"))
        End If

        'Filtro utente Rilevatore
        If Not String.IsNullOrWhiteSpace(omlRilevatore.ValoriAltriCampi("Codice")) Then
            filtri.IdUtenteRilevatore = omlRilevatore.ValoriAltriCampi("Codice")
        End If

        'Filtro stato documenti
        If rdbFiltroStatoDaFirmare.Checked Then
            filtri.FiltroStato = Entities.ArchiviazioneDIRV.FiltroStatoDocumento.DaFirmare
        ElseIf rdbFiltroStatoFirmNoArc.Checked Then
            filtri.FiltroStato = Entities.ArchiviazioneDIRV.FiltroStatoDocumento.FirmatiNonArchiviati
        ElseIf rdbFiltroStatoFirmArc.Checked Then
            filtri.FiltroStato = Entities.ArchiviazioneDIRV.FiltroStatoDocumento.FirmatiArchiviati
        ElseIf rdbFiltroStatoTutti.Checked Then
            filtri.FiltroStato = Entities.ArchiviazioneDIRV.FiltroStatoDocumento.Tutti
        End If

        filtri.UslCorrente = OnVacContext.CodiceUslCorrente

        Return filtri

    End Function

    Private Sub SetImmagineOrdinamento()

        Dim id As String = String.Empty

        Select Case CampoOrdinamento
            Case dgrDocumenti.Columns(dgrDocumentiColumnIndex.Paziente).SortExpression
                id = "imgPaz"
            Case dgrDocumenti.Columns(dgrDocumentiColumnIndex.DataVisita).SortExpression
                id = "imgDat"
            Case dgrDocumenti.Columns(dgrDocumentiColumnIndex.UtenteVisita).SortExpression
                id = "imgUte"
            Case dgrDocumenti.Columns(dgrDocumentiColumnIndex.UtenteFirma).SortExpression
                id = "imgFir"
            Case dgrDocumenti.Columns(dgrDocumentiColumnIndex.UtenteRilevatore).SortExpression
                id = "imgRil"
        End Select

        Dim imageUrl As String = String.Empty

        If VersoOrdinamento = "ASC" Then
            imageUrl = ResolveClientUrl("~/Images/arrow_up_small.gif")
        Else
            imageUrl = ResolveClientUrl("~/Images/arrow_down_small.gif")
        End If

        OnitLayout31.InsertRoutineJS(String.Format("ImpostaImmagineOrdinamento('{0}', '{1}')", id, imageUrl))

    End Sub

    Private Sub SetIdVisiteSelezionate()

        For Each item As DataGridItem In dgrDocumenti.Items

            Dim idVisita As Long = Convert.ToInt64(HttpUtility.HtmlDecode(DirectCast(item.FindControl("hidIdVisita"), HiddenField).Value))

            If DirectCast(item.FindControl("chkSelezioneItem"), CheckBox).Checked Then
                If Not IdVisiteSelezionate.Contains(idVisita) Then
                    IdVisiteSelezionate.Add(idVisita)
                End If
            Else
                If IdVisiteSelezionate.Contains(idVisita) Then
                    IdVisiteSelezionate.Remove(idVisita)
                End If
            End If

        Next

    End Sub

    ''' <summary>
    ''' Click del checkbox nell'header del datagrid per selezionare/deselezionare tutte le righe del datagrid
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SelectAll()

        Dim selezionaTutti As Boolean = False

        If Not Boolean.TryParse(Request.Form.Item("__EVENTARGUMENT"), selezionaTutti) Then
            Return
        End If

        IdVisiteSelezionate = New List(Of Long)()
        SelezionaTuttiChecked = selezionaTutti

        If SelezionaTuttiChecked Then

            Dim listIdVisite As List(Of Long) = Nothing

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizFirma As New Biz.BizFirmaDigitale(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                    listIdVisite = bizFirma.GetIdVisiteDocumenti(FiltriRicercaCorrente)
                End Using
            End Using

            If Not listIdVisite.IsNullOrEmpty() Then
                IdVisiteSelezionate = listIdVisite.Clone()
            End If

        End If

        CaricaDati(dgrDocumenti.CurrentPageIndex, FiltriRicercaCorrente)

    End Sub

    Private Sub ShowMessage(message As String)

        OnitLayout31.ShowMsgBox(New Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(HttpUtility.JavaScriptStringEncode(message), "MSG", False, False))

    End Sub

#End Region

#Region " Protected "

    Protected Function BindFlagFirmaImageUrlValue(dataItem As Object) As String

        Return OnVacUtility.GetFlagFirmaImageUrlValue(DataBinder.Eval(dataItem, "IdUtenteFirma"), DataBinder.Eval(dataItem, "IdUtenteArchiviazione"), Me.Page)

    End Function

    Protected Function BindFlagFirmaToolTipValue(dataItem As Object) As String

        Return OnVacUtility.GetFlagFirmaToolTipValue(DataBinder.Eval(dataItem, "IdUtenteFirma"), DataBinder.Eval(dataItem, "IdUtenteArchiviazione"))

    End Function

#End Region

#Region " Firma Digitale "

    Private Sub FirmaDigitale()

        SetIdVisiteSelezionate()

        If IdVisiteSelezionate Is Nothing OrElse IdVisiteSelezionate.Count = 0 Then
            ShowMessage(Biz.BizFirmaDigitale.Messages.NO_DOCUMENTI_SELEZIONATI)
            Return
        End If

        ' Controlli sulle visite selezionate e creazione documenti XML da firmare
        Dim result As Biz.BizFirmaDigitale.ControlliFirmaMassivaResult = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizFirma As New Biz.BizFirmaDigitale(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                result = bizFirma.CreaDocumentiXMLAnamnesi(IdVisiteSelezionate, UtenteLoggatoIsMedico)

            End Using
        End Using

        If Not result.Success Then
            ShowMessage("Firma digitale non effettuata." + Environment.NewLine + Environment.NewLine + result.Message)
            Return
        End If

        ' Il messaggio con i risultati dei controlli verrà visualizzato dopo la firma
        MessaggioRisultatoControlli = result.Message

        ' Render chiamata javascript per firma digitale multipla
        Dim js As New Text.StringBuilder("signDocument(")
        js.AppendFormat("'{0}', ", My.MySettings.Default.DigitalSignatureServiceURL)    ' url servizio firma digitale DigitalSignatureService.asmx
        js.AppendFormat("[{0}], ", String.Join(",", result.IdDocumentiDaFirmare))       ' array con gli id dei documenti da firmare
        js.AppendFormat("{0}, ", OnVacContext.UserId.ToString())                        ' id utente corrente
        js.AppendFormat("'{0}', ", OnVacContext.AppId)                                  ' id applicativo corrente
        js.AppendFormat("'{0}', ", OnVacContext.Azienda)                                ' codice azienda
        js.Append("SignatureCallBack);")                                                ' nome funzione javascript che verrà richiamata al termine della firma

        ' N.B. : c'è un bug per cui non sempre viene impostata la variabile window.imgWait (nella libreria Onit.Shared.Web), il javascript va in errore e, a causa di ciò, non viene chiamata la applet di firma.
        ' Con questo pezzo di codice, in caso sia non definita, viene impostata la variabile con il percorso dell'immagine da visualizzare 
        Dim imgWait As String = ResolveClientUrl("~/images/wait.png")

        'ClientScript.RegisterClientScriptBlock(GetType(Page), "js",
        '    String.Format("<script type='text/javascript'>try {{ showWaitScreen(2000, true); }} catch (ex) {{}} {0}</script>", js.ToString()))

        ClientScript.RegisterClientScriptBlock(GetType(Page), "OnVac_signDoc",
            String.Format("<script type='text/javascript'>try {{ if (window.imgWait == null) window.imgWait = '{0}'; showWaitScreen(2000, true); }} catch (ex) {{}} {1}</script>", imgWait, js.ToString()))

    End Sub

    ' Richiamato lato client dalla SignatureCallBack eseguita dopo la firma
    Private Sub lnkPostBk_Click(sender As Object, e As EventArgs) Handles lnkPostBk.Click

        Dim msgRisultatoControlli As String = MessaggioRisultatoControlli
        MessaggioRisultatoControlli = String.Empty

        Dim message As New Text.StringBuilder()

        ' Nel campo Hidden "txtResult" è contenuto il risultato ottenuto dalla firma digitale eseguita dall'applet
        If String.IsNullOrWhiteSpace(txtResult.Value) Then
            message.AppendLine("Errore in fase di firma. Non è stato possibile firmare digitalmente i documenti.")
            message.AppendLine()
            message.AppendLine(msgRisultatoControlli)
            ShowMessage(message.ToString())
            Return
        End If

        message.AppendLine("Esito firma digitale")

        If Not String.IsNullOrWhiteSpace(msgRisultatoControlli) Then
            message.AppendLine()
            message.AppendLine(msgRisultatoControlli)
        End If

        Dim des As New JavaScriptSerializer()
        Dim firmeResult As SignResult() = des.Deserialize(Of SignResult())(txtResult.Value)

        Dim ok As Integer = firmeResult.Count(Function(p) p.Ok)
        Dim no As Integer = firmeResult.Count - ok

        message.AppendLine()
        message.AppendFormat("Firma digitale effettuata correttamente per {0} document{1} su {2}.", ok.ToString(), IIf(ok = 1, "o", "i"), firmeResult.Count.ToString())
        message.AppendLine()
        message.AppendFormat("Firma digitale non effettuata per {0} document{1} su {2}.", no.ToString(), IIf(no = 1, "o", "i"), firmeResult.Count.ToString())
        message.AppendLine()

        ShowMessage(message.ToString())

        ' Refresh
        IdVisiteSelezionate = New List(Of Long)()
        SelezionaTuttiChecked = False
        CaricaDati(0, FiltriRicercaCorrente)

    End Sub

#End Region

End Class