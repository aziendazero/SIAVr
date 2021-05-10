Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.Biz

Public Class RicercaPaziente
    Inherits Common.PageBase

#Region " Private Properties "

    ''' <summary>
    ''' Indica se la modale di rilevazione del consenso si è aperta automaticamente a causa di un tentativo di redirect al dettaglio di un paziente con stato consenso bloccante
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property AperturaAutomaticaRilevazioneConsensoBloccante() As Boolean
        Get
            If ViewState("AARC") Is Nothing Then ViewState("AARC") = False
            Return ViewState("AARC")
        End Get
        Set(Value As Boolean)
            ViewState("AARC") = Value
        End Set
    End Property

    Private Property SearchPerformed() As Boolean
        Get
            If ViewState("SP") Is Nothing Then ViewState("SP") = False
            Return ViewState("SP")
        End Get
        Set(Value As Boolean)
            ViewState("SP") = Value
        End Set
    End Property

    ''' <summary>
    ''' Dati del paziente trovato utilizzando la ricerca in anagrafe nazionale (per evitare di doverla rifare alla selezione del paziente)
    ''' </summary>
    ''' <returns></returns>
    Private Property PazienteQPv2 As Entities.Paziente
        Get
            Return ViewState("PazQPv2")
        End Get
        Set(Value As Entities.Paziente)
            ViewState("PazQPv2") = Value
        End Set
    End Property

#End Region

#Region " Private variables "

    ' Flag che indica se deve essere eseguita la ricerca con il pannello
    Private doDataPanelSearch As Boolean = False

    Dim iconeConsensi As GestioneConsenso.Wcf.Proxy.ConsensiService.IconeConsensi

#End Region

#Region " Consts "

    Private Const MESSAGGIO_MERGE As String = "MM"
    Private Const CONFIRM_MERGE As String = "CM"
    Private Const OPEN_RILEVAZIONE_CONSENSO As String = "OPEN_CONS"

#End Region

#Region " Page "

    Private Sub RicercaPaziente_Load(sender As Object, e As EventArgs) Handles Me.Load

        ' Pulisco il valore di OnVacUtility.Variabili.PazId
        OnVacUtility.ClearPazId()

        'Pulizia e log della session
        Dim sc As New SessionCleaner(Settings)
        sc.Start()

        'Pulizia dell'history
        DirectCast(Me.Page, Onit.Shared.Web.UI.Page).HistoryClear()

        If Not Request.QueryString.Item("menu_dis") Is Nothing Then
            OnVacContext.MenuDis = Request.QueryString.Item("menu_dis").ToString()
        End If

        If Not IsPostBack() Then

            Inizializzazione()

        End If

        Dim script As New Text.StringBuilder()
        script.AppendLine("<script type='text/javascript'>")
        script.AppendFormat("var idFmNascita='{0}';", fmComuneNascita.ClientID).AppendLine()

        If Settings.RICERCA_PAZ_SHOW_FILTRO_COMUNE_RESIDENZA Then
            script.AppendFormat("var idFmResidenza='{0}';", fmComuneResidenza.ClientID).AppendLine()
        Else
            script.AppendLine("var idFmResidenza='';")
        End If

        script.AppendLine("</script>")

        ClientScript.RegisterClientScriptBlock(Me.GetType(), "checkValiditaFm", script.ToString())

        Select Case Request.Form("__EVENTTARGET")

            Case "cerca"

                doDataPanelSearch = True

            Case "RefreshFromPopup"
                '--
                ' Se ho appena chiuso la modale di rilevazione del consenso, che si era aperta automaticamente perchè lo stato del cons del paz era bloccante,
                ' allora ricontrollo lo stato del consenso e, se non è più bloccante, faccio subito il redirect al dettaglio del paziente selezionato.
                '--
                Dim redirect As Boolean = False

                If AperturaAutomaticaRilevazioneConsensoBloccante Then

                    AperturaAutomaticaRilevazioneConsensoBloccante = False
                    '--
                    ' N.B. : al click del pulsante di dettaglio viene memorizzato il paziente selezionato, poi viene aperta la pop-up di rilevazione.
                    '        in questo punto, sfrutto il codice memorizzato nella struttura per fare il redirect, quindi deve essere stata valorizzata!   
                    '--
                    Dim consensoGlobalePaziente As Entities.Consenso.StatoConsensoPaziente =
                        OnVacUtility.GetConsensoUltimoPazienteSelezionato(UltimoPazienteSelezionato, Settings)

                    If consensoGlobalePaziente Is Nothing Then

                        redirect = False

                    ElseIf consensoGlobalePaziente.Controllo = Enumerators.ControlloConsenso.Bloccante Then

                        redirect = False

                    ElseIf (IsProntoSoccorso Or IsGestioneCentrale) AndAlso consensoGlobalePaziente.BloccoAccessiEsterni Then

                        redirect = False
                        AlertClientMsg(consensoGlobalePaziente.DescrizioneStatoConsenso + Environment.NewLine + "Impossibile accedere ai dati del paziente.")

                    Else

                        redirect = True
                        RedirectToDetail(UltimoPazienteSelezionato.CodicePazienteLocale, UltimoPazienteSelezionato.CodicePazienteCentrale)

                    End If

                End If

                ' Se non effettuo il redirect, eseguo la ricerca per aggiornare i dati
                If Not redirect Then
                    doDataPanelSearch = True
                End If

        End Select

    End Sub

    Private Sub Inizializzazione()

        ' Imposta il focus sul campo indicato dal parametro
        ImpostaFocus()

        ' Flag che indica se è stata effettuata almeno una ricerca 
        SearchPerformed = False

        ' Impostazione del titolo e visualizzazione pulsanti toolbar
        Dim showButtonMerge As Boolean

        If Request.QueryString("alias") = "true" Then

            showButtonMerge = True
            OnitLayout31.Titolo = "Ricerca Pazienti - Gestione Alias"

        Else

            showButtonMerge = False

            If IsGestioneCentrale Then
                OnitLayout31.Titolo = "Ricerca Pazienti Centrale"
            ElseIf IsProntoSoccorso Then
                OnitLayout31.Titolo = "Ricerca Pazienti - Pronto Soccorso"
            Else
                OnitLayout31.Titolo = "Ricerca Pazienti"
            End If

        End If

        Dim isRicercaStandard As Boolean = Not IsGestioneCentrale AndAlso Not IsProntoSoccorso

        tlbRicerca.FindItemByValue("btnNew").Visible = isRicercaStandard
        tlbRicerca.FindItemByValue("btnAlias").Visible = showButtonMerge AndAlso isRicercaStandard
        tlbRicerca.FindItemByValue("btnUltimoPaz").Visible = isRicercaStandard
        tlbRicerca.FindItemByValue("btnUltimaRicerca").Visible = isRicercaStandard

        If Not isRicercaStandard Then
            Dim separator As Telerik.Web.UI.RadToolBarItem = tlbRicerca.FindItemByValue("sepRicercheRapide")
            If Not separator Is Nothing Then tlbRicerca.Items.Remove(separator)
        End If

        ' Aggiunta del consultorio di lavoro al titolo
        OnVacUtility.ImpostaCnsLavoro(OnitLayout31)

        fmOnVacAlias.VisibileMD = False

        If Not showButtonMerge Then

            Dim btnMerge As Telerik.Web.UI.RadToolBarItem = tlbRicerca.FindItemByValue("btnAlias")

            If Not btnMerge Is Nothing Then
                tlbRicerca.Items.Remove(btnMerge)
            End If

            ShowDgrColumn("chkColumn", False)

        Else

            ShowDgrColumn("chkColumn", True)

        End If

        ' Filtro sul consultorio
        If Settings.RICERCA_PAZ_SHOW_FILTRO_CNS Then

            omlConsultorio.Visible = True
            lblFiltroCns.Visible = True

            If Settings.RICERCA_PAZ_FILTRO_CNS_SET_DEFAULT Then
                omlConsultorio.Codice = OnVacUtility.Variabili.CNS.Codice
                omlConsultorio.RefreshDataBind()
            End If

        Else

            omlConsultorio.Visible = False
            lblFiltroCns.Visible = False

        End If

        ' Filtro sul comune di residenza
        lblComuneResidenza.Visible = Settings.RICERCA_PAZ_SHOW_FILTRO_COMUNE_RESIDENZA
        fmComuneResidenza.Visible = Settings.RICERCA_PAZ_SHOW_FILTRO_COMUNE_RESIDENZA

        ' Filtro sul codice paziente e visualizzazione colonne
        'lblPazCodice.Visible = False
        'txtPazCodice.Visible = False
        trCodicePaziente.Visible = False

        ShowDgrColumn("Codice", False)
        ShowDgrColumn("Ausiliario", False)
        ShowDgrColumn("Regionale", False)
        ShowDgrColumn("ULSSD", False)
        ShowDgrColumn("Fonte", False)
        ShowDgrColumn("Cancellato", False)

        If isRicercaStandard Then

            ShowDgrColumn("Ausiliario", Settings.RICERCA_PAZ_SHOW_CODICE_AUSILIARIO)
            ShowDgrColumn("Regionale", Settings.RICERCA_PAZ_SHOW_CODICE_REGIONALE)
            ShowDgrTemplateColumn("Cancellato", Settings.RICERCA_PAZ_SHOW_FLAG_CANCELLATO)

            ' Visibilità dei campi codice del paziente
            Select Case Settings.RICERCA_PAZ_SHOW_CODICE_PAZIENTE

                Case Enumerators.TipoAutorizzazione.SoloSuperUtenti

                    If Not String.IsNullOrEmpty(Settings.ID_GRUPPO_SUPERUSER) Then

                        Dim isSuperUser As Boolean = OnVacUtility.IsCurrentUserInGroup(Settings.ID_GRUPPO_SUPERUSER)

                        'lblPazCodice.Visible = isSuperUser
                        'txtPazCodice.Visible = isSuperUser
                        trCodicePaziente.Visible = isSuperUser

                        ShowDgrColumn("Codice", isSuperUser)

                    End If

                Case Enumerators.TipoAutorizzazione.TuttiUtenti

                    'lblPazCodice.Visible = True
                    'txtPazCodice.Visible = True
                    trCodicePaziente.Visible = True

                    ShowDgrColumn("Codice", True)

            End Select

        End If

        ' --- Gestione consenso --- '
        ' Visualizzazione pulsante "Consensi" e colonna del datagrid in base ai parametri di gestione dei consensi
        tlbRicerca.FindItemByValue("btnConsenso").Visible = Settings.CONSENSO_GES
        ShowDgrTemplateColumn("StatoConsenso", Settings.CONSENSO_GES AndAlso Settings.CONSENSO_SEMAFORI_VISIBILI)

        ' -------------------------------------------------------------------------------------------------------------- '
        ' TODO [QPv2]: i flag vaccinazioni in ricerca per ora non sono gestiti
        '
        ' --- Gestione vaccinazioni --- '
        '' Visualizzazione colonna del datagrid in base al parametro di visualizzazione vaccinazioni in ricerca
        'ShowDgrTemplateColumn("Vaccinazioni", Settings.RICERCA_PAZ_SHOW_FLAG_VACCINAZIONI AndAlso isRicercaStandard)
        ShowDgrTemplateColumn("Vaccinazioni", False)

        '' --- Gestione appuntamenti --- '
        '' Visualizzazione colonna del datagrid in base al parametro di visualizzazione appuntamenti in ricerca
        'ShowDgrTemplateColumn("Appuntamenti", Settings.RICERCA_PAZ_SHOW_FLAG_APPUNTAMENTI AndAlso isRicercaStandard)
        ShowDgrTemplateColumn("Appuntamenti", False)

        '' --- Gestione esclusioni --- '
        '' Visualizzazione colonna del datagrid in base al parametro di visualizzazione esclusioni in ricerca
        'ShowDgrTemplateColumn("Escluse", Settings.RICERCA_PAZ_SHOW_FLAG_ESCLUSIONI AndAlso isRicercaStandard)
        ShowDgrTemplateColumn("Escluse", False)
        ' -------------------------------------------------------------------------------------------------------------- '

        AbilitaPulsantiToolbar(False)

    End Sub

    Private Sub RicercaPaziente_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        If doDataPanelSearch Then

            SetDatiUltimaRicercaEffettuata()

            If UltimaRicercaPaziente.IsEmpty() Then
                EseguiRicercaUltimoPaziente()
            Else
                EseguiUltimaRicercaEffettuata()
            End If

        End If

    End Sub

#End Region

#Region " Toolbar "

    Protected Sub tlbRicerca_ButtonClick(sender As Object, e As Telerik.Web.UI.RadToolBarEventArgs)

        Select Case e.Item.Value

            Case "btnFind"

                SetDatiUltimaRicercaEffettuata()
                RicercaPazienti()

            Case "btnAlias"

                GestioneAlias()

            Case "btnSeleziona"

                SelezionePaziente()

            Case "btnNew"

                Nuovo()

            Case "btnConsenso"

                If dgrPazienti.SelectedIndex < 0 Then
                    OnitLayout31.ShowMsgBox(New Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Selezionare un paziente", "no_selezione_paz", False, False))
                    Return
                End If

                If Settings.CONSENSO_LOCALE Then
                    '--
                    ' Consenso gestito localmente e paziente non presente in locale => non rilevo il consenso
                    '--
                    Dim codicePazienteLocale As String = GetCodiceLocalePazienteFromDataGridItem(dgrPazienti.SelectedItem)

                    If String.IsNullOrEmpty(codicePazienteLocale) Then
                        OnitLayout31.ShowMsgBox(New Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Il paziente non è presente in locale: impossibile rilevare il consenso.", "no_consenso_paz", False, False))
                        Return
                    End If

                End If

                ' Apertura popup rilevazione consenso
                ApriRilevazioneConsenso(False)

            Case "btnUltimoPaz"

                EseguiRicercaUltimoPaziente()

            Case "btnUltimaRicerca"

                EseguiUltimaRicercaEffettuata()

        End Select

    End Sub

    Private Sub Nuovo()

        Dim infodetail As New Controls.OnitDataPanel.infoDetailStruct() With {.destDataPanelId = "odpDettaglioPaziente", .destinationPage = "GestionePazienti.aspx",
            .isValid = True, .alertMessage = "", .operation = Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord,
            .senderDataPanelId = "odpRicercaPaziente", .senderPage = "RicercaPaziente.aspx"}

        infodetail.filters.Add(New Onit.Controls.OnitDataPanel.Filter With {.Comparator = Onit.Controls.OnitDataPanel.Filter.FilterComparators.AlwaysFalse,
            .connectionName = "locale", .Operator = Onit.Controls.OnitDataPanel.Filter.FilterOperators.And})

        ' aggiunta campi di pre-inserimento dalla ricerca
        AddInfodetailPreinsert(infodetail, "PAZ_COGNOME", txtCognome.Text)
        AddInfodetailPreinsert(infodetail, "PAZ_NOME", txtNome.Text)
        AddInfodetailPreinsert(infodetail, "PAZ_COM_CODICE_NASCITA", fmComuneNascita.Codice)
        AddInfodetailPreinsert(infodetail, "PAZ_SESSO", ddlSesso.SelectedValue)
        AddInfodetailPreinsert(infodetail, "PAZ_DATA_NASCITA", odpDataNascita.Text)
        AddInfodetailPreinsert(infodetail, "PAZ_CODICE_FISCALE", txtCodFiscale.Text)
        AddInfodetailPreinsert(infodetail, "PAZ_TESSERA", txtTesseraSan.Text)
        AddInfodetailPreinsert(infodetail, "PAZ_COM_CODICE_RESIDENZA", fmComuneResidenza.Codice)
        AddInfodetailPreinsert(infodetail, "PAZ_CNS_CODICE", omlConsultorio.Codice)

        Page.Session("OniDataPanel_InfoDetail") = infodetail
        Response.Redirect(infodetail.destinationPage, False)

    End Sub

    Private Shared Sub AddInfodetailPreinsert(infodetail As Controls.OnitDataPanel.infoDetailStruct, sourceField As String, value As String)
        infodetail.preinsertValues.Add(New Onit.Controls.OnitDataPanel.BindingFieldValue() With {.Connection = "centrale", .Description = "Filter value", .Editable = Onit.Controls.OnitDataPanel.BindingFieldValue.editPositions.always,
                                       .Hidden = False, .SourceField = sourceField, .SourceTable = "t_paz_pazienti_centrale", .Target = Nothing, .Value = value})
    End Sub

#End Region

#Region " Eventi OnitLayout "

    Private Sub OnitLayout31_AlertClick(sender As Object, e As Controls.PagesLayout.OnitLayout3.AlertEventArgs) Handles OnitLayout31.AlertClick

        Select Case e.Key

            Case OPEN_RILEVAZIONE_CONSENSO
                ApriRilevazioneConsenso(True)

            Case MESSAGGIO_MERGE
                ' Aggiornamento risultati
                EseguiUltimaRicercaEffettuata()

        End Select

    End Sub

    Private Sub OnitLayout31_ConfirmClick(sender As Object, e As Controls.PagesLayout.OnitLayout3.ConfirmEventArgs) Handles OnitLayout31.ConfirmClick

        If e.Key.StartsWith(CONFIRM_MERGE) Then

            Dim params As String() = e.Key.Split("*")

            Dim idPazienteMaster As Int64 = Convert.ToInt64(params(1))
            Dim arrayIdPazientiAlias As Int64() = params(2).Split(";").Select(Function(p) Convert.ToInt64(p)).ToArray()

            If e.Result Then
                MergePazienti(idPazienteMaster, arrayIdPazientiAlias)
            End If

        End If

    End Sub

#End Region

#Region " Protected Methods "

    Protected Function HideLeftFrameIfNeeded() As String

        If Not Page.IsPostBack Then
            Return GetOpenLeftFrameScript(False)
        End If

        Return String.Empty

    End Function

    Protected Function GetValoreFonte(value As Object) As String

        If Not value Is Nothing AndAlso Not value Is DBNull.Value Then

            Dim fonte As Enumerators.FonteAnagrafica
            If [Enum].TryParse(value, fonte) Then

                Select Case fonte
                    Case Enumerators.FonteAnagrafica.AnagrafeLocale
                        Return "L"
                    Case Enumerators.FonteAnagrafica.AnagrafeCentrale
                        Return "C"
                    Case Enumerators.FonteAnagrafica.Mista
                        Return "L+C"
                End Select

            End If

        End If

        Return String.Empty

    End Function

    Protected Function GetCssClassFonte(value As Object) As String

        If Not value Is Nothing AndAlso Not value Is DBNull.Value Then

            Dim fonte As Enumerators.FonteAnagrafica
            If [Enum].TryParse(value, fonte) Then

                Select Case fonte
                    Case Enumerators.FonteAnagrafica.AnagrafeCentrale
                        Return "legenda-anagrafe-centrale"
                    Case Enumerators.FonteAnagrafica.AnagrafeLocale
                        Return "legenda-anagrafe-locale"
                    Case Enumerators.FonteAnagrafica.Mista
                        Return "legenda-anagrafe-mista"
                End Select

            End If

        End If

        Return String.Empty

    End Function

#End Region

#Region " Private Methods "

    ''' <summary>
    '''  Imposta il focus nel campo indicato dal parametro. Se il campo non c'è lo imposta nel campo cognome.
    ''' </summary>
    Private Sub ImpostaFocus()

        Dim ctlToFocus As Control = pnlFiltri.FindControl(Settings.RICERCA_PAZ_FOCUS)

        Dim clientId As String = txtCognome.ClientID

        If Not ctlToFocus Is Nothing Then
            clientId = ctlToFocus.ClientID
        End If

        Dim strJs As String = String.Format("<script type='text/javascript'> document.getElementById('{0}').focus();</script>", clientId)
        ClientScript.RegisterStartupScript(Page.GetType(), "script_focus", strJs, False)

    End Sub

    Private Sub ShowDgrColumn(key As String, show As Boolean)

        DirectCast(dgrPazienti.getColumnByKey(key), DataGridColumn).Visible = show

    End Sub

    Private Sub ShowDgrTemplateColumn(key As String, show As Boolean)

        DirectCast(dgrPazienti.getColumnByKey(key), Controls.OnitGrid.OnitTemplateColumn).Visible = show

    End Sub

    Private Sub AbilitaPulsantiToolbar(enable As Boolean)

        Dim isRicercaStandard As Boolean = Not IsGestioneCentrale AndAlso Not IsProntoSoccorso

        ' Pulsante Inserimento Paziente (viene abilitato solo se il parametro relativo è true e solo dopo la prima ricerca)
        Dim item As Telerik.Web.UI.RadToolBarItem = tlbRicerca.FindItemByValue("btnNew")
        If Not item Is Nothing Then
            item.Enabled = Settings.INSERIMENTO_PAZIENTE_ABILITATO AndAlso SearchPerformed AndAlso isRicercaStandard
        End If

        ' Pulsante Consenso
        item = tlbRicerca.FindItemByValue("btnConsenso")
        If Not item Is Nothing Then
            item.Enabled = enable
        End If

        ' Pulsante Seleziona 
        item = tlbRicerca.FindItemByValue("btnSeleziona")
        If Not item Is Nothing Then
            item.Enabled = enable
        End If

        ' Pulsante Alias
        item = tlbRicerca.FindItemByValue("btnAlias")
        If Not item Is Nothing Then
            item.Enabled = (enable And isRicercaStandard)
        End If

    End Sub

    Private Sub RicercaPazienti()

        PazienteQPv2 = Nothing

        Dim f As New Entities.FiltroRicercaPaziente()

        If Not String.IsNullOrWhiteSpace(txtPazCodice.Text) And IsNumeric(txtPazCodice.Text) Then
            f.CodiceLocale = Integer.Parse(txtPazCodice.Text)
        End If

        f.CodiceCentrale = txtPazCodiceAusiliario.Text.Trim().ToUpper()

        f.Cognome = txtCognome.Text
        f.Nome = txtNome.Text

        f.CodiceComuneNascita = fmComuneNascita.Codice
        f.Sesso = ddlSesso.SelectedValue

        If Not String.IsNullOrWhiteSpace(odpDataNascita.Text) Then
            f.DataNascita = odpDataNascita.Data
        End If

        If Not String.IsNullOrWhiteSpace(txtAnnoNascita.Text) And IsNumeric(txtAnnoNascita.Text) Then
            f.AnnoNascita = Integer.Parse(txtAnnoNascita.Text)
        End If

        f.CodiceTesseraSanitaria = txtTesseraSan.Text
        f.CodiceFiscale = txtCodFiscale.Text

        f.CodiceComuneResidenza = fmComuneResidenza.Codice

        If Not String.IsNullOrWhiteSpace(omlConsultorio.Codice) Then
            f.CodiceCentroVaccinale = omlConsultorio.Codice
        End If

        Dim risultatoRicerca As Entities.RicercaPazientiResult

        Dim listaPazientiRicerca As List(Of Entities.PazienteTrovato)

        Try
            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizPaziente As New BizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    risultatoRicerca = bizPaziente.RicercaPazientiCentrale(f)

                    listaPazientiRicerca = risultatoRicerca.ListaPazienti

                End Using
            End Using

        Catch ex As BizToUserMessageException
            OnitLayout31.ShowMsgBox(New Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(ex.Message, "BizToUserMessageException", False, False))
            listaPazientiRicerca = New List(Of Entities.PazienteTrovato)()
            Return
        End Try

        If Not listaPazientiRicerca.IsNullOrEmpty() AndAlso risultatoRicerca.MaxRecordRaggiunto Then
            AlertClientMsg(GetOnVacResourceValue(Constants.StringResourcesKey.Ricerca_AlertMaxNrRisultati).Replace("{0}", listaPazientiRicerca.Count.ToString()).Replace("<br/>", Environment.NewLine))
        End If

        SearchPerformed = True

        PazienteQPv2 = risultatoRicerca.PazienteQPv2

        ' Icone stato consenso per i pazienti trovati, in base al codice ausiliario
        Dim listaCodiciPaziente As IEnumerable(Of String) =
            listaPazientiRicerca.Where(Function(p1) Not String.IsNullOrWhiteSpace(p1.CodiceCentrale)).Select(Function(p2) p2.CodiceCentrale)

        If listaCodiciPaziente.IsNullOrEmpty() OrElse Not Settings.CONSENSO_GES Then

            iconeConsensi = New GestioneConsenso.Wcf.Proxy.ConsensiService.IconeConsensi()

        Else

            Dim codiceAziendaRegistrazione As String = OnVacUtility.GetCodiceAziendaRegistrazione(Settings)

            Using consenso As New GestioneConsenso.Wcf.Proxy.ConsensiService.ConsensiServiceClient()
                iconeConsensi = consenso.GetUltimoConsensoIconaPazienti(listaCodiciPaziente.ToArray(), OnVacContext.Azienda, Settings.CONSENSO_APP_ID, codiceAziendaRegistrazione)
            End Using

        End If

        dgrPazienti.DataSource = listaPazientiRicerca
        dgrPazienti.DataBind()
        dgrPazienti.SelectedIndex = If(listaPazientiRicerca.Any(), 0, -1)

        lblRisultati.Text = "Risultati della ricerca: " + listaPazientiRicerca.Count.ToString()
        If listaPazientiRicerca.Count = 1 Then
            lblRisultati.Text += " paziente trovato"
        Else
            lblRisultati.Text += " pazienti trovati"
        End If

        If risultatoRicerca.InterrogatoServizioQPv2 Then
            lblRisultati.Text += " - La ricerca è stata effettuata anche sull'Anagrafe Nazionale in base al solo codice fiscale"
        End If

        AbilitaPulsantiToolbar(listaPazientiRicerca.Count > 0)

    End Sub

    Private Sub ApriRilevazioneConsenso(autoEdit As Boolean)

        Dim codiceAusiliarioPaziente As String = GetCodiceAusiliarioPazienteFromDataGridItem(dgrPazienti.SelectedItem)

        If String.IsNullOrWhiteSpace(codiceAusiliarioPaziente) Then
            AlertClientMsg(Settings.CONSENSO_MSG_NO_COD_CENTRALE)
        Else
            modConsenso.VisibileMD = True
            frameConsenso.Attributes.Add("src", GetUrlMascheraRilevazioneConsenso(codiceAusiliarioPaziente, autoEdit))
        End If

    End Sub

    Private Sub SelezionePaziente()

        If dgrPazienti.SelectedIndex < 0 Then
            OnitLayout31.ShowMsgBox(New Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Selezionare un paziente", "no_selezione_paz", False, False))
            Return
        End If

        ' Id del paziente selezionato nella griglia
        Dim id As String = dgrPazienti.DataKeys(dgrPazienti.SelectedIndex)

        Dim codicePaziente As String = GetCodiceLocalePazienteFromDataGridItem(dgrPazienti.SelectedItem)
        Dim codiceAusiliarioPaziente As String = GetCodiceAusiliarioPazienteFromDataGridItem(dgrPazienti.SelectedItem)

        If codicePaziente = "0" Then codicePaziente = String.Empty

        ' ---
        ' QPv2
        ' ---
        ' Se codici locale e centrale entrambi nulli, allora il paziente proviene da qpv2 => va inserito sia in centrale che in locale
        '
        If String.IsNullOrWhiteSpace(codicePaziente) AndAlso String.IsNullOrWhiteSpace(codiceAusiliarioPaziente) AndAlso PazienteQPv2 IsNot Nothing Then

            Try
                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                    Using bizPaziente As BizPaziente = BizFactory.Instance.CreateBizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), New BizLogOptions(DataLogStructure.TipiArgomento.PAZIENTI, False))

                        Dim insertCommand As New BizPaziente.InserisciPazienteCommand() With {
                            .Paziente = PazienteQPv2,
                            .FromVAC = True,
                            .ForzaInserimento = True
                        }

                        Dim insertResult As BizResult = bizPaziente.InserisciPaziente(insertCommand)

                        codicePaziente = insertCommand.Paziente.Paz_Codice
                        codiceAusiliarioPaziente = insertCommand.Paziente.CodiceAusiliario

                    End Using
                End Using

            Catch ex As Exception
                Common.Utility.EventLogHelper.EventLogWrite(ex, "RicercaPaziente - Inserimento paziente QPv2", Diagnostics.EventLogEntryType.Information, OnVacContext.AppId)
                AlertClientMsg("Impossibile inserire il paziente proveniente da Anagrafe Nazionale. " + Environment.NewLine + ex.Message)
                Return
            End Try

        End If

        ' ---
        ' Solo centrale
        ' ---
        ' Se codice locale nullo e codice centrale valorizzato, allora il paziente è presente solo in centrale => va inserito in locale
        '
        If String.IsNullOrWhiteSpace(codicePaziente) AndAlso Not String.IsNullOrWhiteSpace(codiceAusiliarioPaziente) Then
            Try
                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                    Using bizPaziente As New BizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), New BizLogOptions(DataLogStructure.TipiArgomento.PAZIENTI, False))

                        Dim paz As Entities.Paziente = bizPaziente.InserisciPazienteFromCentrale(codiceAusiliarioPaziente)

                        If paz IsNot Nothing Then
                            codicePaziente = paz.Paz_Codice
                            codiceAusiliarioPaziente = paz.CodiceAusiliario
                        End If

                    End Using
                End Using

            Catch ex As Exception
                Common.Utility.EventLogHelper.EventLogWrite(ex, "RicercaPaziente - Inserimento paziente da centrale", Diagnostics.EventLogEntryType.Information, OnVacContext.AppId)
                AlertClientMsg("Impossibile inserire il paziente proveniente da Anagrafe Centrale. " + Environment.NewLine + ex.Message)
                Return
            End Try
        End If

        ' ---
        ' Selezione paziente
        ' ---
        UltimoPazienteSelezionato = New Entities.UltimoPazienteSelezionato(codiceAusiliarioPaziente, codicePaziente)

        ' ---
        ' Gestione consenso paziente
        ' ---
        Dim hasErrors As Boolean = False

        If Settings.CONSENSO_GES Then

            ' N.B. : in Veneto il consenso è sempre in base al codice ausiliario

            If IsProntoSoccorso AndAlso String.IsNullOrWhiteSpace(codicePaziente) Then
                '--
                ' Ricerca PS e paziente non presente in locale => no redirect alle eseguite
                ' Redirect bloccato nel metodo RedirectToDetail con msg all'utente.
                '--
            ElseIf Settings.CONSENSO_LOCALE AndAlso String.IsNullOrWhiteSpace(codicePaziente) Then
                '--
                ' Consenso gestito in LOCALE e paziente non presente in locale  => no redirect alle eseguite
                ' Redirect bloccato nel metodo RedirectToDetail con msg all'utente.
                '--
            Else
                '--
                ' Copre i casi:
                '   1. Consenso gestito in LOCALE e paziente presente in locale.
                '   2. Consenso gestito in CENTRALE.
                '--
                Dim consensoGlobalePaz As Entities.Consenso.StatoConsensoPaziente = Nothing

                If Not String.IsNullOrWhiteSpace(codiceAusiliarioPaziente) Then
                    consensoGlobalePaz = OnVacUtility.GetConsensoGlobalePaziente(codiceAusiliarioPaziente, Settings)
                End If

                If consensoGlobalePaz Is Nothing Then

                    AlertClientMsg("Impossibile aprire il dettaglio dei dati del paziente. Nessun consenso trovato.")
                    hasErrors = True

                ElseIf consensoGlobalePaz.Controllo = Enumerators.ControlloConsenso.Bloccante Then
                    '-- 
                    ' Valore del consenso BLOCCANTE => blocco l'accesso al dettaglio e, se il param auto_edit vale true, apro la modale di rilevazione del consenso.
                    '--
                    ' N.B.: la modale di rilevazione viene aperta sempre (se il parametro lo prevede), indipendentemente dalla gestione del consenso,
                    '        perchè siamo nel caso in cui il consenso è locale e il paziente in locale c'è, oppure il consenso è centrale.
                    Dim consensoBloccanteMessage As String =
                        consensoGlobalePaz.DescrizioneStatoConsenso + Environment.NewLine + "Impossibile aprire il dettaglio dei dati del paziente."

                    If Settings.CONSENSO_BLOCCANTE_AUTO_EDIT Then

                        AperturaAutomaticaRilevazioneConsensoBloccante = True

                        ' Messaggio all'utente e apertura pop-up rilevazione del consenso in edit (nell'evento AlertClick dell'OnitLayout)
                        consensoBloccanteMessage += Environment.NewLine + "Verrà aperta la maschera di rilevazione del consenso."
                        OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(
                                                HttpUtility.JavaScriptStringEncode(consensoBloccanteMessage), OPEN_RILEVAZIONE_CONSENSO, False, True))

                    Else

                        ' Messaggio all'utente
                        AlertClientMsg(consensoBloccanteMessage)

                    End If

                    hasErrors = True

                ElseIf (IsProntoSoccorso Or IsGestioneCentrale) AndAlso consensoGlobalePaz.BloccoAccessiEsterni Then
                    '--
                    ' Accesso da maschera in configurazione PS o Centrale: 
                    ' se il livello è marcato come bloccante per accessi esterni, non permetto all'utente di fare il redirect al dettaglio.
                    '--
                    AlertClientMsg(consensoGlobalePaz.DescrizioneStatoConsenso + Environment.NewLine + "Impossibile accedere ai dati del paziente.")
                    hasErrors = True

                End If

            End If

        End If

        If Not hasErrors Then
            RedirectToDetail(codicePaziente, codiceAusiliarioPaziente)
        End If

    End Sub

    Private Sub RedirectToDetail(codicePaziente As String, codiceAusiliarioPaziente As String)

        If String.IsNullOrWhiteSpace(codicePaziente) AndAlso String.IsNullOrWhiteSpace(codiceAusiliarioPaziente) Then
            Return
        End If

        Dim bloccoLocale As Boolean = False

        Dim gestioneConsensoERedirect As Boolean = False
        Dim destinazione As ConsensoTrattamentoDatiUtente.DestinazioneRedirect

        ' Casi in cui viene bloccato il redirect:
        ' 1- codice paziente locale nullo e maschera in configurazione Pronto Soccorso, oppure
        ' 2- codice paziente nullo e consenso gestito in locale
        If String.IsNullOrWhiteSpace(codicePaziente) AndAlso (IsProntoSoccorso OrElse Settings.CONSENSO_LOCALE) Then
            bloccoLocale = True
        End If

        If IsProntoSoccorso Then
            '--
            ' PRONTO SOCCORSO
            '--
            If bloccoLocale Then
                gestioneConsensoERedirect = False
                AlertClientMsg("Impossibile visualizzare il dettaglio del paziente. Paziente non presente in anagrafe locale.")
            Else
                gestioneConsensoERedirect = True
                destinazione = ConsensoTrattamentoDatiUtente.DestinazioneRedirect.VacEseguitePS
            End If

        ElseIf IsGestioneCentrale Then
            '--
            ' ANAGRAFE CENTRALE
            '--

            ' TODO [CONSENSO]: da decommentare se si vuole bloccare l'accesso ai dati da centrale, in caso di consenso bloccante
            'If bloccoLocale Then
            '    ' Caso consenso locale e paz non in locale => se la maschera è in configurazione centrale, devo bloccare l'accesso ai dati del paziente
            '    AlertClientMsg("Impossibile visualizzare il dettaglio del paziente. Paziente non presente in anagrafe locale.")
            'Else

            gestioneConsensoERedirect = True
            destinazione = ConsensoTrattamentoDatiUtente.DestinazioneRedirect.VacEseguiteCentrale

            'End If

        Else

            gestioneConsensoERedirect = True
            destinazione = ConsensoTrattamentoDatiUtente.DestinazioneRedirect.DettaglioPaziente

        End If

        If gestioneConsensoERedirect Then

            If GetRichiestaConsensoTrattamentoDatiUtente(OnVacUtility.Variabili.CNS.Codice) Then

                If Not String.IsNullOrWhiteSpace(codicePaziente) Then
                    ucConsensoUtente.CodicePaziente = Convert.ToInt64(codicePaziente)
                End If

                ucConsensoUtente.CodiceAusiliarioPaziente = codiceAusiliarioPaziente
                ucConsensoUtente.CodiceConsultorio = OnVacUtility.Variabili.CNS.Codice
                ucConsensoUtente.Destinazione = destinazione

                fmConsensoUtente.VisibileMD = True

            Else

                RedirectToDestinazione(codicePaziente, codiceAusiliarioPaziente, destinazione)

            End If

        End If

    End Sub

    Private Sub RedirectToVacEseguitePS(codicePaziente As Long)

        OnVacUtility.Variabili.PazId = codicePaziente

        Response.Redirect(ResolveClientUrl("~/Pronto Soccorso/VacEseguitePS.aspx?LoadLeftFramePS=false"))

    End Sub

    Private Sub RedirectToVacEseguiteCentrale(codiceAusiliarioPaziente As String)

        OnVacUtility.Variabili.PazId = codiceAusiliarioPaziente

        Response.Redirect(ResolveClientUrl("~/HPazienti/VacEseguite/VacEseguite.aspx?isCentrale=true"))

    End Sub

    Private Sub RedirectToDestinazione(codicePaziente As Long, codiceAusiliarioPaziente As String, destinazione As ConsensoTrattamentoDatiUtente.DestinazioneRedirect)

        Select Case destinazione

            Case ConsensoTrattamentoDatiUtente.DestinazioneRedirect.NessunaSelezione
                ' nessun redirect

            Case ConsensoTrattamentoDatiUtente.DestinazioneRedirect.DettaglioPaziente
                RedirectToGestionePaziente(codicePaziente.ToString())

            Case ConsensoTrattamentoDatiUtente.DestinazioneRedirect.VacEseguitePS
                RedirectToVacEseguitePS(codicePaziente)

            Case ConsensoTrattamentoDatiUtente.DestinazioneRedirect.VacEseguiteCentrale
                RedirectToVacEseguiteCentrale(codiceAusiliarioPaziente)

                'Case ConsensoTrattamentoDatiUtente.DestinazioneRedirect.ConvocazioniPaziente
                ' Non previsto da ricerca paziente

        End Select

    End Sub

#Region " Ricerche rapide "

    Private Sub EseguiRicercaUltimoPaziente()

        If UltimoPazienteSelezionato Is Nothing OrElse
            (String.IsNullOrWhiteSpace(UltimoPazienteSelezionato.CodicePazienteLocale) And String.IsNullOrWhiteSpace(UltimoPazienteSelezionato.CodicePazienteCentrale)) Then

            OnitLayout31.ShowMsgBox(New Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Ricerca non effettuata: nessun paziente selezionato in precedenza", "no_ultimo_paz", False, False))
            Return

        End If

        ' Imposta solo il filtro sul codice del paziente, effettua la ricerca e cancella il filtro
        ClearFilterFields()

        txtPazCodice.Text = UltimoPazienteSelezionato.CodicePazienteLocale
        txtPazCodiceAusiliario.Text = UltimoPazienteSelezionato.CodicePazienteCentrale

        RicercaPazienti()

        txtPazCodice.Text = String.Empty
        txtPazCodiceAusiliario.Text = String.Empty

    End Sub

    Private Sub EseguiUltimaRicercaEffettuata()

        If UltimaRicercaPaziente Is Nothing OrElse UltimaRicercaPaziente.IsEmpty() Then
            OnitLayout31.ShowMsgBox(New Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Ricerca non effettuata: nessuna ricerca eseguita in precedenza", "no_ultima_ricerca", False, False))
            Return
        End If

        ' Impostazione dei filtri di ricerca utilizzati in precedenza - unbind sui controlli
        ImpostaFiltriUltimaRicerca()

        RicercaPazienti()

    End Sub

    Private Sub ClearFilterFields()

        txtPazCodice.Text = String.Empty
        txtPazCodiceAusiliario.Text = String.Empty
        txtCognome.Text = String.Empty
        txtNome.Text = String.Empty
        ddlSesso.ClearSelection()
        odpDataNascita.Text = String.Empty
        txtAnnoNascita.Text = String.Empty
        fmComuneNascita.Codice = String.Empty
        fmComuneNascita.Descrizione = String.Empty
        fmComuneNascita.ValAltriCampi = String.Empty
        fmComuneNascita.RefreshDataBind()
        txtCodFiscale.Text = String.Empty
        txtTesseraSan.Text = String.Empty
        fmComuneResidenza.Codice = String.Empty
        fmComuneResidenza.Descrizione = String.Empty
        fmComuneResidenza.ValAltriCampi = String.Empty
        fmComuneResidenza.RefreshDataBind()
        omlConsultorio.Codice = String.Empty
        omlConsultorio.Descrizione = String.Empty
        omlConsultorio.ValAltriCampi = String.Empty
        omlConsultorio.RefreshDataBind()

    End Sub

    Private Sub ImpostaFiltriUltimaRicerca()

        txtPazCodice.Text = UltimaRicercaPaziente.CodicePaziente
        txtPazCodiceAusiliario.Text = UltimaRicercaPaziente.CodiceAusiliarioPaziente

        txtCognome.Text = UltimaRicercaPaziente.Cognome
        txtNome.Text = UltimaRicercaPaziente.Nome

        ddlSesso.ClearSelection()
        Dim item As ListItem = ddlSesso.Items.FindByValue(UltimaRicercaPaziente.Sesso)
        If Not item Is Nothing Then item.Selected = True

        If UltimaRicercaPaziente.DataNascita.HasValue Then
            odpDataNascita.Data = UltimaRicercaPaziente.DataNascita.Value
        Else
            odpDataNascita.Text = String.Empty
        End If

        txtAnnoNascita.Text = UltimaRicercaPaziente.AnnoNascita
        txtCodFiscale.Text = UltimaRicercaPaziente.CodiceFiscale
        txtTesseraSan.Text = UltimaRicercaPaziente.TesseraSanitaria

        omlConsultorio.Codice = UltimaRicercaPaziente.CodiceConsultorio
        omlConsultorio.Descrizione = UltimaRicercaPaziente.DescrizioneConsultorio
        omlConsultorio.RefreshDataBind()

        fmComuneNascita.Codice = UltimaRicercaPaziente.CodiceComuneNascita
        fmComuneNascita.Descrizione = UltimaRicercaPaziente.DescrizioneComuneNascita
        fmComuneNascita.RefreshDataBind()

        fmComuneResidenza.Codice = UltimaRicercaPaziente.CodiceComuneResidenza
        fmComuneResidenza.Descrizione = UltimaRicercaPaziente.DescrizioneComuneResidenza
        fmComuneResidenza.RefreshDataBind()

    End Sub

    Private Sub SetDatiUltimaRicercaEffettuata()

        UltimaRicercaPaziente = New Entities.UltimaRicercaPazientiEffettuata()

        UltimaRicercaPaziente.CodicePaziente = txtPazCodice.Text.Trim()
        UltimaRicercaPaziente.CodiceAusiliarioPaziente = txtPazCodiceAusiliario.Text.Trim().ToUpper()

        UltimaRicercaPaziente.Cognome = txtCognome.Text.Trim().ToUpper()
        UltimaRicercaPaziente.Nome = txtNome.Text.Trim().ToUpper()

        If Not ddlSesso.SelectedItem Is Nothing Then
            UltimaRicercaPaziente.Sesso = ddlSesso.SelectedItem.Value
        End If

        If Not String.IsNullOrEmpty(odpDataNascita.Text) Then
            UltimaRicercaPaziente.DataNascita = odpDataNascita.Data
        End If

        UltimaRicercaPaziente.AnnoNascita = txtAnnoNascita.Text
        UltimaRicercaPaziente.CodiceFiscale = txtCodFiscale.Text.Trim().ToUpper()
        UltimaRicercaPaziente.TesseraSanitaria = txtTesseraSan.Text.Trim()

        If Not String.IsNullOrEmpty(omlConsultorio.Codice) AndAlso Not String.IsNullOrEmpty(omlConsultorio.Descrizione) Then
            UltimaRicercaPaziente.CodiceConsultorio = omlConsultorio.Codice
            UltimaRicercaPaziente.DescrizioneConsultorio = omlConsultorio.Descrizione
        End If

        If Not String.IsNullOrEmpty(fmComuneNascita.Codice) AndAlso Not String.IsNullOrEmpty(fmComuneNascita.Descrizione) Then
            UltimaRicercaPaziente.CodiceComuneNascita = fmComuneNascita.Codice
            UltimaRicercaPaziente.DescrizioneComuneNascita = fmComuneNascita.Descrizione
        End If

        If Not String.IsNullOrEmpty(fmComuneResidenza.Codice) AndAlso Not String.IsNullOrEmpty(fmComuneResidenza.Descrizione) Then
            UltimaRicercaPaziente.CodiceComuneResidenza = fmComuneResidenza.Codice
            UltimaRicercaPaziente.DescrizioneComuneResidenza = fmComuneResidenza.Descrizione
        End If

    End Sub

#End Region

#Region " Datagrid "

    Protected Sub dgrPazienti_ItemDataBound(sender As Object, e As DataGridItemEventArgs) Handles dgrPazienti.ItemDataBound

        Select Case e.Item.ItemType

            Case ListItemType.Item, ListItemType.AlternatingItem, ListItemType.SelectedItem

                If Settings.CONSENSO_GES AndAlso Settings.CONSENSO_SEMAFORI_VISIBILI Then

                    ' Stato Consenso
                    Dim imgStatoConsenso As System.Web.UI.WebControls.Image = DirectCast(e.Item.FindControl("imgStatoConsenso"), System.Web.UI.WebControls.Image)

                    Dim pazCodiceLocale As String = GetCodiceLocalePazienteFromDataGridItem(e.Item)

                    If Settings.CONSENSO_LOCALE AndAlso String.IsNullOrEmpty(pazCodiceLocale) Then

                        ' Rilevazione consenso in locale e paziente non in locale => no immagine stato consenso
                        imgStatoConsenso.Visible = False

                    Else

                        Dim pazCodiceAusiliario As String = GetCodiceAusiliarioPazienteFromDataGridItem(e.Item)

                        If String.IsNullOrWhiteSpace(pazCodiceAusiliario) Then

                            ' Caso in cui il risultato arriva da QPv2
                            imgStatoConsenso.Visible = False

                        Else

                            Dim statoConsenso As GestioneConsenso.Wcf.Proxy.ConsensiService.IconeConsensiItem =
                                iconeConsensi.Items.Where(Function(p) p.CodicePaziente = pazCodiceAusiliario).FirstOrDefault()

                            If statoConsenso Is Nothing Then
                                imgStatoConsenso.ImageUrl = "~/Images/consensoAltro.png"
                                imgStatoConsenso.ToolTip = "Stato consenso: Sconosciuto"
                            Else
                                Dim icona As Onit.OnAssistnet.GestioneConsenso.Wcf.Proxy.ConsensiService.Icona = iconeConsensi.Icone.Where(Function(p) p.ID = statoConsenso.IconaID).FirstOrDefault()
                                imgStatoConsenso.ImageUrl = icona.Url
                                imgStatoConsenso.ToolTip = String.Format("Stato consenso: {0}", icona.Descrizione)
                            End If

                        End If

                    End If

                End If

                ' Usl di assistenza e domicilio
                Dim ulssCodice As String = HttpUtility.HtmlDecode(e.Item.Cells(dgrPazienti.getColumnNumberByKey("ULSS")).Text).Trim()
                Dim ulssDCodice As String = HttpUtility.HtmlDecode(e.Item.Cells(dgrPazienti.getColumnNumberByKey("ULSSD")).Text).Trim()

                If String.IsNullOrWhiteSpace(ulssCodice) AndAlso Not String.IsNullOrWhiteSpace(ulssDCodice) Then
                    e.Item.Cells(dgrPazienti.getColumnNumberByKey("ULSS")).Text = ulssDCodice
                End If

        End Select

    End Sub

    Private Function GetCodiceAusiliarioPazienteFromDataGridItem(item As DataGridItem) As String

        Return HttpUtility.HtmlDecode(item.Cells(dgrPazienti.getColumnNumberByKey("Ausiliario")).Text).Trim()

    End Function

    Private Function GetCodiceLocalePazienteFromDataGridItem(item As DataGridItem) As String

        Return HttpUtility.HtmlDecode(item.Cells(dgrPazienti.getColumnNumberByKey("Codice")).Text).Trim()

    End Function

#End Region

#Region " Consenso Utente "

    Private Sub ucConsensoUtente_ConsensoTrattamentoDatiUtenteAccettato() Handles ucConsensoUtente.ConsensoTrattamentoDatiUtenteAccettato

        ' TODO [Consenso Utente]: dove impostare UltimoPazienteSelezionato?
        '' Memorizzazione codice paziente per ricerca rapida (l'impostazione del paziente corrente quando si effettua il redirect vero e proprio)
        'UltimoPazienteSelezionato = New Entities.UltimoPazienteSelezionato(ucConsensoUtente.CodiceAusiliarioPaziente, ucConsensoUtente.CodicePaziente)

        Dim codicePaziente As Long = ucConsensoUtente.CodicePaziente
        Dim codiceAusiliarioPaziente As String = ucConsensoUtente.CodiceAusiliarioPaziente
        Dim destinazione As ConsensoTrattamentoDatiUtente.DestinazioneRedirect = ucConsensoUtente.Destinazione

        ucConsensoUtente.ClearParameters()
        fmConsensoUtente.VisibileMD = False

        RedirectToDestinazione(codicePaziente, codiceAusiliarioPaziente, destinazione)

    End Sub

    Private Sub ucConsensoUtente_ConsensoTrattamentoDatiUtenteRifiutato() Handles ucConsensoUtente.ConsensoTrattamentoDatiUtenteRifiutato

        ucConsensoUtente.ClearParameters()
        fmConsensoUtente.VisibileMD = False

    End Sub

#End Region

#Region " Merge "

    Private Sub GestioneAlias()

        Dim dtMerge As New DataTable()
        dtMerge.Columns.Add("PAZ_CODICE")
        dtMerge.Columns.Add("PAZ_CODICE_AUSILIARIO")
        dtMerge.Columns.Add("PAZ_CANCELLATO")
        dtMerge.Columns.Add("PAZ_COGNOME")
        dtMerge.Columns.Add("PAZ_NOME")
        dtMerge.Columns.Add("PAZ_SESSO")
        dtMerge.Columns.Add("PAZ_DATA_NASCITA")
        dtMerge.Columns.Add("COMUNE_DI_NASCITA")
        dtMerge.Columns.Add("PAZ_CODICE_FISCALE")
        dtMerge.Columns.Add("PAZ_TESSERA")
        dtMerge.Columns.Add("COMUNE_DI_RESIDENZA")
        dtMerge.Columns.Add("PAZ_INDIRIZZO_RESIDENZA")

        For i As Integer = 0 To dgrPazienti.Items.Count - 1

            Dim chk As CheckBox = dgrPazienti.Items(i).FindControl(Onit.Controls.OnitGrid.OnitMultiSelColumn.CheckBoxName())

            If chk IsNot Nothing AndAlso chk.Checked Then

                Dim newRow As DataRow = dtMerge.NewRow()

                newRow("PAZ_CODICE") = GetCodiceLocalePazienteFromDataGridItem(dgrPazienti.Items(i))
                newRow("PAZ_CODICE_AUSILIARIO") = GetCodiceAusiliarioPazienteFromDataGridItem(dgrPazienti.Items(i))
                newRow("PAZ_COGNOME") = HttpUtility.HtmlDecode(dgrPazienti.Items(i).Cells(dgrPazienti.getColumnNumberByKey("Cognome")).Text).Trim()
                newRow("PAZ_NOME") = HttpUtility.HtmlDecode(dgrPazienti.Items(i).Cells(dgrPazienti.getColumnNumberByKey("Nome")).Text).Trim()
                newRow("PAZ_SESSO") = HttpUtility.HtmlDecode(dgrPazienti.Items(i).Cells(dgrPazienti.getColumnNumberByKey("Sesso")).Text).Trim()
                newRow("PAZ_DATA_NASCITA") = HttpUtility.HtmlDecode(dgrPazienti.Items(i).Cells(dgrPazienti.getColumnNumberByKey("DataNascita")).Text).Trim()
                newRow("COMUNE_DI_NASCITA") = HttpUtility.HtmlDecode(dgrPazienti.Items(i).Cells(dgrPazienti.getColumnNumberByKey("ComuneNascita")).Text).Trim()
                newRow("PAZ_CODICE_FISCALE") = HttpUtility.HtmlDecode(dgrPazienti.Items(i).Cells(dgrPazienti.getColumnNumberByKey("Fiscale")).Text).Trim()
                newRow("PAZ_TESSERA") = HttpUtility.HtmlDecode(dgrPazienti.Items(i).Cells(dgrPazienti.getColumnNumberByKey("Tessera")).Text).Trim()
                newRow("COMUNE_DI_RESIDENZA") = HttpUtility.HtmlDecode(dgrPazienti.Items(i).Cells(dgrPazienti.getColumnNumberByKey("ComuneResidenza")).Text).Trim()
                newRow("PAZ_INDIRIZZO_RESIDENZA") = HttpUtility.HtmlDecode(dgrPazienti.Items(i).Cells(dgrPazienti.getColumnNumberByKey("IndirizzoResidenza")).Text).Trim()

                Dim lblCanc As Label = dgrPazienti.Items(i).Cells(dgrPazienti.getColumnNumberByKey("Cancellato")).FindControl("lblCanc")
                If Not lblCanc Is Nothing Then
                    newRow("PAZ_CANCELLATO") = lblCanc.Text
                End If

                dtMerge.Rows.Add(newRow)

            End If

        Next

        dtMerge.AcceptChanges()

        If dtMerge.Rows.Count < 2 Then

            AlertClientMsg("Impossibile eseguire il merge: devono essere selezionati due pazienti.")

        Else

            fmOnVacAlias.VisibileMD = True
            OnVacAlias1.ImpostaDati(dtMerge)

        End If

    End Sub

#Region " Eventi User Control Alias "

    Private Sub OnVacAlias1_AliasNonEseguibile(sender As Object, e As AliasNonEseguibileArgs) Handles OnVacAlias1.AliasNonEseguibile

        fmOnVacAlias.VisibileMD = Not e.ChiudiControllo
        OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(e.Messaggio, "msgAliasNonEseguibile", False, False))

    End Sub

    Private Sub OnVacAlias1_ConfermaAlias(sender As Object, e As OnVacAliasArgs) Handles OnVacAlias1.ConfermaAlias

        fmOnVacAlias.VisibileMD = False

        If e.Conferma Then

            Dim checkResult As BizPaziente.CheckCodiciRegionaliMasterAliasResult

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizPaziente As BizPaziente = BizFactory.Instance.CreateBizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.MERGEPAZIENTI))

                    checkResult = bizPaziente.CheckCodiciRegionaliMasterAlias(e.PazMasterId, e.pazAliasIDs)

                End Using
            End Using

            Select Case checkResult.Result

                Case BizPaziente.CheckCodiciRegionaliMasterAliasResult.ResultType.Failure

                    ' Se il controllo dei codici regionali restituisce lo stato Failure => NO MERGE
                    OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(checkResult.Message, "msgNoMergeCodReg", False, False))

                Case BizPaziente.CheckCodiciRegionaliMasterAliasResult.ResultType.Warning

                    ' Se il controllo dei codici regionali restituisce lo stato Warning => CONFERMA ALL'UTENTE
                    Dim key As String = String.Format("{0}*{1}*{2}", CONFIRM_MERGE, e.PazMasterId.ToString(), String.Join(";", e.pazAliasIDs))

                    OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("ATTENZIONE: " + checkResult.Message + " Continuare?", key, True, True))

                Case BizPaziente.CheckCodiciRegionaliMasterAliasResult.ResultType.Success

                    ' Se il controllo dei codici regionali restituisce lo stato Success => MERGE
                    MergePazienti(e.PazMasterId, e.pazAliasIDs)

                Case Else

                    Throw New NotImplementedException("CheckCodiciRegionaliMasterAlias")

            End Select

        End If

    End Sub

#End Region

    Private Sub MergePazienti(idPazienteMaster As Long, arrayIdPazientiAlias As Long())

        Dim javascriptMessage As New Text.StringBuilder()

        Dim success As Boolean = True

        ' Esecuzione Alias
        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, OnVacUtility.GetReadCommittedTransactionOptions())

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                Dim pazienteMaster As Entities.Paziente
                Dim pazientiAliasList As New List(Of Entities.Paziente)()
                Dim idPazientiAlias As New List(Of String)()

                Using bizPaziente As BizPaziente = BizFactory.Instance.CreateBizPaziente(genericProvider, Settings,
                                                                                         OnVacContext.CreateBizContextInfos(OnVacUtility.Variabili.CNS.Codice),
                                                                                         OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.MERGEPAZIENTI))
                    pazienteMaster = bizPaziente.CercaPaziente(idPazienteMaster)

                    idPazientiAlias.AddRange(arrayIdPazientiAlias.Select(Function(p) p.ToString()).ToList())

                    Dim pazientiAlias As Collection.PazienteCollection = bizPaziente.CercaPazienti(String.Join(",", idPazientiAlias.ToArray()))

                    For Each pazienteAlias As Entities.Paziente In pazientiAlias

                        Dim unisciPazientiCommand As New BizPaziente.UnisciPazientiCommand()
                        unisciPazientiCommand.PazienteMaster = pazienteMaster
                        unisciPazientiCommand.PazienteAlias = pazienteAlias

                        Dim bizResult As BizResult = bizPaziente.UnisciPazienti(unisciPazientiCommand)

                        If javascriptMessage.Length > 0 Then javascriptMessage.AppendLine()

                        If bizResult.Success Then
                            javascriptMessage.Append("Merge effettuato con successo")
                        Else
                            javascriptMessage.Append("Merge non effettuato")
                        End If

                        If pazientiAlias.Count > 1 Then
                            javascriptMessage.AppendLine(String.Format(": ALIAS [{0}]", pazienteAlias.Paz_Codice))
                        End If

                        If bizResult.Success Then
                            pazientiAliasList.Add(pazienteAlias)
                        Else
                            Dim message As String = bizResult.GetResultMessageCollection(False).ToString()
                            If Not String.IsNullOrEmpty(message) Then
                                javascriptMessage.AppendLine(": ")
                                javascriptMessage.AppendLine(message)
                            End If
                        End If

#Region " Log "
                        ' --- Log --- '
                        If bizResult.Messages.Count > 0 Then

                            Dim testataLog As New DataLogStructure.Testata(DataLogStructure.TipiArgomento.MERGEPAZIENTI, Operazione.Generico, pazienteMaster.Paz_Codice, False)
                            Dim recordLog As New DataLogStructure.Record()

                            recordLog.Campi.Add(New DataLogStructure.Campo(String.Empty, bizResult.Messages.ToString()))
                            testataLog.Records.Add(recordLog)

                            Biz.BizClass.WriteLog(New List(Of DataLogStructure.Testata)({testataLog}), True, OnVacContext.Connection)

                        End If
                        ' ----------- '
#End Region

                    Next

                End Using

                If pazientiAliasList.Count > 0 Then
                    OnVacMidSendManager.UnisciPazienti(pazienteMaster, Nothing, pazientiAliasList.ToArray())
                End If

            End Using

            transactionScope.Complete()

        End Using

        ' N. B. : mostro all'utente il messaggio con il risultato del merge prima di rifare la ricerca, altrimenti il messaggio non verrebbe visualizzato. 
        '         La ricerca ne potrebbe mostrare un altro ("max risultati") che impedirebbe la visualizzazione del risultato del merge, se lo metto dopo.

        OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(HttpUtility.JavaScriptStringEncode(javascriptMessage.ToString()), MESSAGGIO_MERGE, False, True))

    End Sub

#End Region

#End Region

End Class