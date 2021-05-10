Imports System.Collections.Generic

Imports Onit.OnAssistnet.Data

Imports Onit.OnAssistnet.MID
Imports Onit.OnAssistnet.MID.Models

Imports Onit.OnAssistnet.OnVac.MID
Imports Onit.OnAssistnet.OnVac.MID.Models
Imports Onit.OnAssistnet.OnVac.MID.Factories

Imports Onit.OnAssistnet.OnVac.Common.Utility.Extensions
Imports Onit.Database.DataAccessManager


Partial Class UnmergePazienti
    Inherits Common.PageBase


#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub


    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region


#Region " Enums "

    Private Enum LayoutState
        Load = 0
        Unmerge = 1
    End Enum

    Private Enum DatagridColumnIndex
        Selector = 0
        CodiceAlias = 1
        CodiceMaster = 2
        CognomeAlias = 3
        NomeAlias = 4
        DataNascitaAlias = 5
    End Enum

#End Region


#Region " Page Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            OnVacUtility.ImpostaCnsLavoro(OnitLayout31)

            Me.SetLabelRisultati(-1)

            Me.SetToolbarLayout(LayoutState.Load)

            Me.dgrAlias.SelectedIndex = -1


            ' TODO: UNMERGE => Filtro Consultorio impostato al consultorio corrente ???
            'omlConsultorio.Codice = OnVacUtility.Variabili.CNS.Codice
            'omlConsultorio.RefreshDataBind()


        End If

    End Sub

#End Region


#Region " Controls Events "

#Region " Toolbar "

    Private Sub ToolBar_ButtonClicked(ByVal sender As Object, ByVal be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case be.Button.Key

            Case "btnCerca"

                Me.RicercaAlias(0)

            Case "btnInfoMaster"

                Me.ShowMasterInfo()

            Case "btnUnmerge"

                Me.ExecuteUnmerge()

            Case "btnPulisci"

                Me.ClearFields(True, True, True)

            Case "btnTestMerge", "btnTestReceiveMerge", "btnTestReceiveInserisci", "btnTestReceiveModifica", "btnTestSend"

#If DEBUG Then
                ' Test per Send, Release e Merge
                If be.Button.Key = "btnTestSend" Then
                    'Me.TestSend()
                ElseIf be.Button.Key = "btnTestReceiveInserisci" Then
                    'Me.TestReceiveInserisci()
                ElseIf be.Button.Key = "btnTestReceiveModifica" Then
                    'Me.TestReceiveModifica()
                ElseIf be.Button.Key = "btnTestReceiveMerge" Then
                    'Me.TestReceiveMerge()
                Else
                    Me.TestMerge()
                End If
#Else
                ' Send, Receive e Merge sono implementate solo per test in debug.
                ' Nella versione di release non devono poter essere utilizzate!
                Throw New NotImplementedException()
#End If

        End Select

    End Sub

#End Region

#Region " LinkButtons "

    Private Sub btnPulisciFiltriAlias_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPulisciFiltriAlias.Click

        Me.ClearFields(False, True, False)

    End Sub

    Private Sub btnPulisciFiltriMaster_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPulisciFiltriMaster.Click

        Me.ClearFields(True, False, False)

    End Sub

#End Region

#Region " Datagrid "

    Private Sub dgrAlias_PageIndexChanged(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dgrAlias.PageIndexChanged

        Me.RicercaAlias(e.NewPageIndex)

    End Sub

    Protected Sub dgrAlias_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgrAlias.SelectedIndexChanged

        If dgrAlias.SelectedIndex >= 0 Then
            Me.SetToolbarLayout(LayoutState.Unmerge)
        End If

    End Sub

#End Region

#End Region


#Region " Private "

    Private Sub RicercaAlias(ByVal currentPageIndex As Integer)

        ' Creazione struttura filtri di ricerca x Master
        Dim masterFilters As New OnVac.Filters.FiltriRicercaPaziente()
        masterFilters.Cognome = txtCognomeMaster.Text.Trim().ToUpper()
        masterFilters.Nome = txtNomeMaster.Text.Trim().ToUpper()
        masterFilters.DataNascita_Da = dpkDataNascitaMaster.Data
        masterFilters.DataNascita_A = dpkDataNascitaMaster.Data
        masterFilters.Sesso = ddlSessoMaster.SelectedValue
        masterFilters.CodiceFiscale = txtCodiceFiscaleMaster.Text.Trim().ToUpper()
        masterFilters.CodiceComuneNascita = modComuneNascitaMaster.Codice

        ' Creazione struttura filtri di ricerca x Alias
        Dim aliasFilters As New OnVac.Filters.FiltriRicercaPaziente()
        aliasFilters.Cognome = txtCognomeAlias.Text.Trim().ToUpper()
        aliasFilters.Nome = txtNomeAlias.Text.Trim().ToUpper()
        aliasFilters.DataNascita_Da = dpkDataNascitaAlias.Data
        aliasFilters.DataNascita_A = dpkDataNascitaAlias.Data
        aliasFilters.Sesso = ddlSessoAlias.SelectedValue
        aliasFilters.CodiceFiscale = txtCodiceFiscaleAlias.Text.Trim().ToUpper()
        aliasFilters.CodiceComuneNascita = modComuneNascitaAlias.Codice
        aliasFilters.Consultorio = modConsultorioAlias.Codice

        If aliasFilters.IsEmpty() AndAlso masterFilters.IsEmpty() Then
            Me.ShowMessage("Ricerca non effettuata: nessun filtro impostato.")
            Return
        End If

        If Not aliasFilters.IsEmpty() AndAlso Not masterFilters.IsEmpty() Then
            Me.ShowMessage("Ricerca non effettuata: impostare un solo tipo di filtri (o sul master, o sull'alias).")
            Return
        End If

        Dim countAlias As Integer = 0
        Dim listAlias As List(Of Entities.PazienteAlias)

        If Not aliasFilters.IsEmpty() Then

            ' Ricerca diretta nella t_tmp_pazienti_alias
            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                countAlias = genericProvider.AliasPazienti.CountAliasToLoad(aliasFilters)

                Dim startIndex As Integer = currentPageIndex * Me.dgrAlias.PageSize

                If startIndex > countAlias - 1 Then
                    startIndex = 0
                    currentPageIndex = 0
                End If

                Dim pagingOptions As New PagingOptions()
                pagingOptions.StartRecordIndex = startIndex
                pagingOptions.EndRecordIndex = startIndex + Me.dgrAlias.PageSize

                listAlias = genericProvider.AliasPazienti.LoadAlias(aliasFilters, pagingOptions)

            End Using

        Else

            ' Ricerca nella t_tmp_pazienti_alias filtrando per i dati del master (t_paz_pazienti)
            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                countAlias = genericProvider.AliasPazienti.CountAliasToLoadFromPazienti(masterFilters)

                Dim startIndex As Integer = currentPageIndex * Me.dgrAlias.PageSize

                If startIndex > countAlias - 1 Then
                    startIndex = 0
                    currentPageIndex = 0
                End If

                Dim pagingOptions As New PagingOptions()
                pagingOptions.StartRecordIndex = startIndex
                pagingOptions.EndRecordIndex = startIndex + Me.dgrAlias.PageSize

                listAlias = genericProvider.AliasPazienti.LoadAliasFromPazienti(masterFilters, pagingOptions)

            End Using

        End If

        Me.dgrAlias.VirtualItemCount = countAlias

        Me.dgrAlias.CurrentPageIndex = currentPageIndex
        Me.dgrAlias.DataSource = listAlias
        Me.dgrAlias.DataBind()

        Me.dgrAlias.SelectedIndex = -1

        Me.SetLabelRisultati(countAlias)

        Me.SetToolbarLayout(LayoutState.Load)

    End Sub

    Private Sub ShowMasterInfo()

        uscDatiVaccinaliMaster.CaricaDati(Me.GetCodiceMaster(), Me.GetCodiceAlias())

        modInfoMaster.VisibileMD = True

    End Sub

    Private Sub ExecuteUnmerge()

        Dim msg As String = String.Empty
        Dim result As Boolean = False

        ' Se non c'è nessuna riga selezionata nel datagrid
        If dgrAlias.SelectedIndex < 0 Then

            msg = "Nessun paziente selezionato."
            result = False

        Else

            ' Codice Master
            Dim codMaster As Integer = Me.GetCodiceMaster()

            ' Codice Alias
            Dim codAlias As Integer = Me.GetCodiceAlias()

            ' Risultato
            Dim bizResult As Biz.BizResult = Nothing

            Using dam As IDAM = OnVacUtility.OpenDam()

                Dim dbGenericProvider As DAL.DbGenericProvider = Nothing

                Dim bizPaz As Biz.BizPaziente = Nothing

                Try

                    dam.BeginTrans()

                    dbGenericProvider = New DAL.DbGenericProvider(dam)

                    bizPaz = Biz.BizFactory.Instance.CreateBizPaziente(dbGenericProvider, Settings, OnVacContext.CreateBizContextInfos(), New Biz.BizLogOptions(Log.DataLogStructure.TipiArgomento.UNMERGEPAZIENTI))

                    ' Esecuzione unmerge
                    bizResult = bizPaz.DisunisciPazienti(codMaster, codAlias)

                    ' Invio messaggio unmerge
                    If bizResult.Success Then

                        Dim pazienteMaster As Entities.Paziente = Nothing
                        Dim pazienteAlias As Entities.Paziente = Nothing

                        Using bizPaziente As New Biz.BizPaziente(dbGenericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                            ' N.B. : questa maschera è utilizzabile solo in modalità locale e non in centrale
                            pazienteMaster = bizPaziente.CercaPaziente(codMaster)
                            pazienteAlias = bizPaziente.CercaPaziente(codAlias)

                        End Using

                        OnVacMidSendManager.DisunisciPazienti(pazienteMaster, Nothing, pazienteAlias, Nothing, Me.Settings)

                    End If

                    dam.Commit()

                Catch ex As Exception

                    If Not dbGenericProvider Is Nothing Then dbGenericProvider.Dispose()
                    If Not bizPaz Is Nothing Then bizPaz.Dispose()

                    bizResult = New Biz.BizResult(False, ex)

                End Try

            End Using

            result = bizResult.Success

            If result Then

                msg = "Unmerge effettuato con successo." + bizResult.Messages.ToString()

            Else

                Dim errorMessage As String = String.Empty

                Dim resultMessage As String = bizResult.Messages.ToString()

                If resultMessage.Contains(Constants.OracleErrors.ORA_00001) Then

                    ' Errore Oracle: violata restrizione di unicità -> Messaggio leggibile all'utente
                    errorMessage = "Il codice del paziente alias da ripristinare è già presente in anagrafe."

                ElseIf resultMessage.Contains(Constants.OracleErrors.ORA_02291) Then

                    ' Errore Oracle: violata restrizione di integrità -> Messaggio leggibile all'utente
                    errorMessage = "Errore di integrità nei dati del paziente alias da ripristinare."

                Else

                    errorMessage = resultMessage

                End If

                OnVac.Common.Utility.EventLogHelper.EventLogWrite(resultMessage, OnVacContext.AppId)

                msg = "Unmerge non effettuato. " + errorMessage

            End If

        End If

        ' Messaggio all'utente
        Me.ShowMessage(msg)

        ' Se tutto ok, riesegue la ricerca per aggiornare l'elenco dei risultati
        If result Then Me.RicercaAlias(0)

    End Sub

    ' Pulisce i filtri e i risultati
    Private Sub ClearFields(ByVal clearMasterFilters As Boolean, ByVal clearAliasFilters As Boolean, ByVal clearResults As Boolean)

        If clearMasterFilters Then
            txtCognomeMaster.Text = String.Empty
            txtNomeMaster.Text = String.Empty
            dpkDataNascitaMaster.Text = String.Empty

            ddlSessoMaster.ClearSelection()
            txtCodiceFiscaleMaster.Text = String.Empty

            modComuneNascitaMaster.Codice = String.Empty
            modComuneNascitaMaster.Descrizione = String.Empty
            modComuneNascitaMaster.RefreshDataBind()
        End If

        If clearAliasFilters Then
            txtCognomeAlias.Text = String.Empty
            txtNomeAlias.Text = String.Empty
            dpkDataNascitaAlias.Text = String.Empty

            ddlSessoAlias.ClearSelection()
            txtCodiceFiscaleAlias.Text = String.Empty

            modConsultorioAlias.Codice = String.Empty
            modConsultorioAlias.Descrizione = String.Empty
            modConsultorioAlias.RefreshDataBind()

            modComuneNascitaAlias.Codice = String.Empty
            modComuneNascitaAlias.Descrizione = String.Empty
            modComuneNascitaAlias.RefreshDataBind()
        End If

        If clearResults Then
            dgrAlias.DataSource = Nothing
            dgrAlias.DataBind()

            Me.SetLabelRisultati(-1)
        End If

    End Sub

    Private Sub SetLabelRisultati(ByVal countAlias As Integer)
        Dim msg As String

        Select Case countAlias
            Case -1
                msg = String.Empty
            Case 0
                msg = ": nessun paziente trovato"
            Case 1
                msg = ": 1 paziente trovato"
            Case Else
                msg = String.Format(": {0} pazienti trovati", countAlias.ToString())
        End Select

        lblRisultati.Text = String.Format("Risultati della ricerca{0}", msg)

    End Sub

    Private Sub SetToolbarLayout(ByVal state As LayoutState)

        Dim enableUnmergeButton As Boolean = (state = LayoutState.Unmerge)

        ToolBar.Items.FromKeyButton("btnCerca").Enabled = True
        ToolBar.Items.FromKeyButton("btnInfoMaster").Enabled = enableUnmergeButton
        ToolBar.Items.FromKeyButton("btnUnmerge").Enabled = enableUnmergeButton
        ToolBar.Items.FromKeyButton("btnPulisci").Enabled = True
    End Sub

    ' Codice Master della riga selezionata nel datagrid
    Private Function GetCodiceMaster() As Integer
        If dgrAlias.SelectedIndex = -1 Then
            Return -1
        End If

        Dim strCodice As String = dgrAlias.SelectedItem.Cells(DatagridColumnIndex.CodiceMaster).Text

        Dim codMaster As Integer = -1
        If Not String.IsNullOrEmpty(strCodice) Then
            codMaster = Convert.ToInt32(strCodice)
        End If

        Return codMaster
    End Function

    ' Codice Alias della riga selezionata nel datagrid
    Private Function GetCodiceAlias() As Integer
        If dgrAlias.SelectedIndex = -1 Then
            Return -1
        End If

        Dim strCodice As String = dgrAlias.SelectedItem.Cells(DatagridColumnIndex.CodiceAlias).Text

        Dim codAlias As Integer = -1
        If Not String.IsNullOrEmpty(strCodice) Then
            codAlias = Convert.ToInt32(strCodice)
        End If

        Return codAlias
    End Function

    Private Sub ShowMessage(ByVal msg As String)

        Dim rnd As New Random()
        Dim key As Integer = rnd.Next(0, 255)

        Dim msgKey As String = String.Format("msg{0}", key.ToString())

        Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(Me.ApplyEscapeJS(msg), msgKey, False, False))

    End Sub

#End Region


#Region " Test "

    'Private Sub TestReceiveMerge()

    '    ' --- Pazienti di prova --- '
    '    ' 450613 PROVA*ONIT MASTER UNO 01/01/2009
    '    ' 700072 PROVA*ONIT MASTER DUE 01/01/2009
    '    ' ------------------------- '

    '    Dim codMaster As Integer = 450613
    '    Dim codAlias As Integer = 700072

    '    Dim allineaPazienteReceiveModel As IAllineaPazienteReceiveModel = _
    '        AllineaPazienteModelFactory.Instance().CreateReceiveModel(OnVacContext.AppId, OnVacContext.Azienda)

    '    If Not allineaPazienteReceiveModel Is Nothing Then

    '        Dim pazienteMasterContract As Onit.OnAssistnet.Contracts.Paziente
    '        Dim pazienteAliasContract As Onit.OnAssistnet.Contracts.Paziente

    '        ' --- Test --- '

    '        '' Test 1 - Master e Alias entrambi presenti
    '        'pazienteMasterContract = Me.MapEntityToContract(codMaster, True)
    '        'pazienteAliasContract = Me.MapEntityToContract(codAlias, True)


    '        '' Test 2 - Master non presente, Alias presente
    '        'Dim pazienteMasterEntity As Entities.Paziente = Me.LoadPaziente(codMaster.ToString())

    '        'pazienteMasterEntity.Paz_Codice = "123456" ' Codice non esistente

    '        'pazienteMasterContract = Me.MapEntityToContract(pazienteMasterEntity, False)

    '        'pazienteAliasContract = Me.MapEntityToContract(codAlias, True)


    '        ' Test 3 - Master e Alias entrambi non presenti
    '        Dim pazienteMasterEntity As Entities.Paziente = Me.LoadPaziente(codMaster.ToString())

    '        pazienteMasterEntity.Paz_Codice = "123457" ' Codice non esistente

    '        pazienteMasterContract = Me.MapEntityToContract(pazienteMasterEntity, False)

    '        Dim pazienteAliasEntity As Entities.Paziente = Me.LoadPaziente(codAlias.ToString())

    '        pazienteAliasEntity.Paz_Codice = "123458" ' Codice non esistente

    '        pazienteAliasContract = Me.MapEntityToContract(pazienteAliasEntity, False)

    '        ' ------------ '
    '        Dim allineaPazienteReceiveModelResult As AllineaPazienteReceiveModelResult = _
    '                allineaPazienteReceiveModel.UnisciPazienti(pazienteMasterContract, pazienteAliasContract, Me.GetAllineaPazienteInfo(OperazionePaziente.Unificazione))

    '        If Not allineaPazienteReceiveModelResult.Success Then
    '            Throw New ApplicationException(String.Format("Errore in IAllineaPazienteReceiveModel.UnisciPaziente:{0}{1}", Environment.NewLine, String.Join(Environment.NewLine, allineaPazienteReceiveModelResult.GetMessagesDescriptions().ToArray())))
    '        End If

    '    End If

    'End Sub

    'Private Sub TestReceiveInserisci()

    '    Dim codicePazienteTest As Integer = 450613

    '    Dim pazienteEntity As Entities.Paziente = Me.LoadPaziente(codicePazienteTest)

    '    ' Dati di test per inserimento
    '    pazienteEntity.Paz_Codice = -1

    '    ' Valorizzazione struttura Contract da passare al mid
    '    Dim pazienteContract As Onit.OnAssistnet.Contracts.Paziente = Me.MapEntityToContract(pazienteEntity, True)

    '    ' --- Receive --- '
    '    Dim allineaPazienteReceiveModel As IAllineaPazienteReceiveModel = _
    '        AllineaPazienteModelFactory.Instance().CreateReceiveModel(OnVacContext.AppId, OnVacContext.Azienda)

    '    If Not allineaPazienteReceiveModel Is Nothing Then

    '        Dim allineaPazienteReceiveModelResult As Onit.OnAssistnet.MID.Models.AllineaPazienteReceiveModelResult = _
    '                allineaPazienteReceiveModel.InserisciPaziente(pazienteContract, Me.GetAllineaPazienteInfo(OperazionePaziente.Iscrizione))

    '        If Not allineaPazienteReceiveModelResult.Success Then
    '            Throw New ApplicationException(String.Format("Errore in IAllineaPazienteReceiveModel.InserisciPaziente:{0}{1}", Environment.NewLine, String.Join(Environment.NewLine, allineaPazienteReceiveModelResult.GetMessagesDescriptions().ToArray())))
    '        End If

    '    End If

    'End Sub

    'Private Sub TestReceiveModifica()

    '    Dim codicePazienteTest As Integer = 450613

    '    ' Valorizzazione struttura Contract da passare al mid
    '    Dim pazienteContract As Onit.OnAssistnet.Contracts.Paziente = Me.MapEntityToContract(codicePazienteTest, True)


    '    ' --- Modifiche x test --- '
    '    pazienteContract.DatiAnagrafici.Telefono1 = "0987654321"


    '    ' --- Receive --- '
    '    Dim allineaPazienteReceiveModel As IAllineaPazienteReceiveModel = _
    '        AllineaPazienteModelFactory.Instance().CreateReceiveModel(OnVacContext.AppId, OnVacContext.Azienda)

    '    If Not allineaPazienteReceiveModel Is Nothing Then

    '        Dim allineaPazienteReceiveModelResult As Onit.OnAssistnet.MID.Models.AllineaPazienteReceiveModelResult = _
    '                allineaPazienteReceiveModel.ModificaPaziente(pazienteContract, Me.GetAllineaPazienteInfo(OperazionePaziente.Variazione))

    '        If Not allineaPazienteReceiveModelResult.Success Then
    '            Throw New ApplicationException(String.Format("Errore in IAllineaPazienteReceiveModel.ModificaPaziente:{0}{1}", Environment.NewLine, String.Join(Environment.NewLine, allineaPazienteReceiveModelResult.GetMessagesDescriptions().ToArray())))
    '        End If

    '    End If

    'End Sub

    Private Sub TestMerge()

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Dim bizPaz As Biz.BizPaziente = Biz.BizFactory.Instance.CreateBizPaziente(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), New Biz.BizLogOptions(Log.DataLogStructure.TipiArgomento.UNMERGEPAZIENTI))

            ' --- Pazienti di prova --- '
            ' 450613 PROVA*ONIT MASTER UNO 01/01/2009
            ' 700072 PROVA*ONIT MASTER DUE 01/01/2009
            ' ------------------------- '

            Dim codMaster As Integer = 450613
            Dim codAlias As Integer = 700072

            bizPaz.UnisciPazienti(codMaster, codAlias)

        End Using

    End Sub

    'Private Sub TestSend()

    '    ' --- Paziente --- '
    '    Dim codPazienteToSend As Integer = 450613

    '    Dim paziente As Entities.Paziente = Me.LoadPaziente(codPazienteToSend)

    '    If paziente Is Nothing Then
    '        Return
    '    End If

    '    Dim pazientePrecedente As Entities.Paziente = paziente.Clone()


    '    ' --- Send --- '
    '    Dim allineaPazienteSendModel As IAllineaPazienteSendModel(Of Entities.Paziente) = AllineaPazienteModelFactory.Instance.CreateSendModel(OnVacContext.AppId, OnVacContext.Azienda)

    '    If Not allineaPazienteSendModel Is Nothing Then

    '        Dim allineaPazienteSendModelResult As Onit.OnAssistnet.MID.Models.AllineaPazienteSendModelResult = _
    '                allineaPazienteSendModel.ModificaPaziente(paziente, pazientePrecedente, Me.GetAllineaPazienteInfo(OperazionePaziente.Variazione))

    '        If Not allineaPazienteSendModelResult.Success Then
    '            Throw New ApplicationException(String.Format("Errore in IAllineaPazienteSendModel.ModificaPaziente:{0}{1}", Environment.NewLine, String.Join(Environment.NewLine, allineaPazienteSendModelResult.GetMessagesDescriptions().ToArray())))
    '        End If

    '    End If

    'End Sub

#Region " Mapping Entities.Paziente -> Contracts.Paziente "

    ' Restituisce un oggetto Onit.OnAssistnet.Contracts.Paziente, impostando i dati del paziente specificato 
    Private Function MapEntityToContract(codPaziente As Integer, usaAziendale As Boolean) As Onit.OnAssistnet.Contracts.Paziente

        ' Caricamento paziente dall'anagrafe
        Dim pazienteEntity As Entities.Paziente = Me.LoadPaziente(codPaziente.ToString())

        If pazienteEntity Is Nothing Then Return Nothing

        Return Me.MapEntityToContract(pazienteEntity, usaAziendale)

    End Function

    ' Restituisce un oggetto Onit.OnAssistnet.Contracts.Paziente, impostando i dati del paziente specificato 
    Private Function MapEntityToContract(pazienteEntity As Entities.Paziente, usaAziendale As Boolean)

        Dim codAziendale As String = pazienteEntity.CodiceAusiliario

        If Not usaAziendale Then
            ' Codice aziendale generato casualmente solo per fare dei test senza generare errore di chiave duplicata 
            ' in inserimento, a causa del constraint sul paz_codice_ausiliario.
            Dim random As New Random()
            codAziendale = Chr(random.Next(65, 91)).ToString() _
                         + Chr(random.Next(65, 91)).ToString() _
                         + random.Next(0, 9999999).ToString()
        End If

        Dim pazienteContract As Onit.OnAssistnet.Contracts.Paziente = Contracts.PazienteFactory.InstantiatePaziente()

        Dim id As Onit.OnAssistnet.Contracts.ID

        ' Codice locale
        id = New Onit.OnAssistnet.Contracts.ID()
        id.TipoCodificatoID = Onit.OnAssistnet.Contracts.TipoCodificatoID.Locale
        id.IDValue = pazienteEntity.Paz_Codice

        pazienteContract.DatiAnagrafici.AddID(id)

        ' Codice aziendale
        id = New Onit.OnAssistnet.Contracts.ID()
        id.TipoCodificatoID = Onit.OnAssistnet.Contracts.TipoCodificatoID.Aziendale
        id.IDValue = codAziendale

        pazienteContract.DatiAnagrafici.AddID(id)

        ' Codice fiscale
        id = New Onit.OnAssistnet.Contracts.ID()
        id.TipoCodificatoID = Onit.OnAssistnet.Contracts.TipoCodificatoID.Fiscale
        id.IDValue = pazienteEntity.PAZ_CODICE_FISCALE

        pazienteContract.DatiAnagrafici.AddID(id)

        ' Tessera sanitaria
        id = New Onit.OnAssistnet.Contracts.ID()
        id.TipoCodificatoID = Onit.OnAssistnet.Contracts.TipoCodificatoID.Sanitario
        id.IDValue = pazienteEntity.Tessera

        pazienteContract.DatiAnagrafici.AddID(id)

        ' Codice regionale
        id = New Onit.OnAssistnet.Contracts.ID()
        id.TipoCodificatoID = Onit.OnAssistnet.Contracts.TipoCodificatoID.Regionale
        id.IDValue = pazienteEntity.PAZ_CODICE_REGIONALE

        pazienteContract.DatiAnagrafici.AddID(id)

        pazienteContract.DatiAnagrafici.Cognome = pazienteEntity.PAZ_COGNOME
        pazienteContract.DatiAnagrafici.Nome = pazienteEntity.PAZ_NOME
        pazienteContract.DatiAnagrafici.Sesso = pazienteEntity.Sesso
        pazienteContract.DatiAnagrafici.DataNascita = pazienteEntity.Data_Nascita
        pazienteContract.DatiAnagrafici.CittadinanzaCodice = pazienteEntity.Cittadinanza_Codice
        pazienteContract.DatiAnagrafici.CodiceMedicoBase = pazienteEntity.MedicoBase_Codice

        pazienteContract.DatiAnagrafici.LivelloCertificazione = pazienteEntity.LivelloCertificazione.ToString()

        Dim indirizzo As Onit.OnAssistnet.Contracts.Indirizzo

        ' Nascita
        indirizzo = New Onit.OnAssistnet.Contracts.Indirizzo()
        indirizzo.ComuneCodiceXMPI = pazienteEntity.ComuneNascita_Codice
        indirizzo.TipoCodificato = Contracts.TipoCodificatoIndirizzo.Nee
        indirizzo.DataInizio = pazienteEntity.Data_Nascita
        indirizzo.DataInizioSpecified = True

        pazienteContract.DatiAnagrafici.AddIndirizzo(indirizzo)

        ' Residenza
        indirizzo = New Onit.OnAssistnet.Contracts.Indirizzo()
        indirizzo.ComuneCodiceXMPI = pazienteEntity.ComuneResidenza_Codice
        indirizzo.TipoCodificato = Contracts.TipoCodificatoIndirizzo.Legal
        indirizzo.ViaECivico = pazienteEntity.IndirizzoResidenza
        indirizzo.Cap = pazienteEntity.ComuneResidenza_Cap

        If pazienteEntity.DataInserimento <> Date.MinValue Then
            indirizzo.DataInizio = pazienteEntity.DataInserimento
            indirizzo.DataInizioSpecified = True
        End If

        If (pazienteEntity.DataEmigrazione <> Date.MinValue) Then
            indirizzo.DataFine = pazienteEntity.DataEmigrazione
            indirizzo.DataFineSpecified = True
        End If

        pazienteContract.DatiAnagrafici.AddIndirizzo(indirizzo)

        ' Domicilio
        indirizzo = New Onit.OnAssistnet.Contracts.Indirizzo()
        indirizzo.ComuneCodiceXMPI = pazienteEntity.ComuneDomicilio_Codice
        indirizzo.TipoCodificato = Contracts.TipoCodificatoIndirizzo.Home
        indirizzo.ViaECivico = pazienteEntity.IndirizzoDomicilio
        indirizzo.Cap = pazienteEntity.ComuneDomicilio_Cap

        If (pazienteEntity.DataInserimento <> Date.MinValue) Then
            indirizzo.DataInizio = pazienteEntity.DataInserimento
            indirizzo.DataInizioSpecified = True
        End If

        If (pazienteEntity.DataEmigrazione <> Date.MinValue) Then
            indirizzo.DataFine = pazienteEntity.DataEmigrazione
            indirizzo.DataFineSpecified = True
        End If

        pazienteContract.DatiAnagrafici.AddIndirizzo(indirizzo)

        ' Emigrazione
        If (Not String.IsNullOrEmpty(pazienteEntity.ComuneEmigrazione_Codice)) Then
            indirizzo = New Onit.OnAssistnet.Contracts.Indirizzo()
            indirizzo.ComuneCodiceXMPI = pazienteEntity.ComuneEmigrazione_Codice
            indirizzo.TipoCodificato = Contracts.TipoCodificatoIndirizzo.Current

            If pazienteEntity.DataEmigrazione > Date.MinValue Then
                indirizzo.DataInizio = pazienteEntity.DataEmigrazione
                indirizzo.DataInizioSpecified = True
            End If

            pazienteContract.DatiAnagrafici.AddIndirizzo(indirizzo)
        End If

        ' Immigrazione
        If (Not String.IsNullOrEmpty(pazienteEntity.ComuneProvenienza_Codice)) Then
            indirizzo = New Onit.OnAssistnet.Contracts.Indirizzo()
            indirizzo.ComuneCodiceXMPI = pazienteEntity.ComuneProvenienza_Codice
            indirizzo.TipoCodificato = Contracts.TipoCodificatoIndirizzo.Foreign

            If pazienteEntity.DataImmigrazione > Date.MinValue Then
                indirizzo.DataInizio = pazienteEntity.DataImmigrazione
                indirizzo.DataInizioSpecified = True
            End If

            pazienteContract.DatiAnagrafici.AddIndirizzo(indirizzo)
        End If

        pazienteContract.DatiSanitari.USLCodiceResidenza = pazienteEntity.UslResidenza_Codice
        pazienteContract.DatiSanitari.USLCodiceAssistenza = pazienteEntity.UslAssistenza_Codice

        pazienteContract.DatiAnagrafici.Telefono1 = pazienteEntity.Telefono1
        pazienteContract.DatiAnagrafici.Telefono2 = pazienteEntity.Telefono2

        pazienteContract.DatiAnagrafici.USLCodiceDefault = OnVacContext.CodiceUslCorrente
        pazienteContract.DatiSanitari.Distretto = pazienteEntity.Distretto_Codice

        ' Medico di base
        pazienteContract.DatiSanitari.MedicoBaseAUSLCodice = pazienteEntity.MedicoBase_Codice

        If Not String.IsNullOrEmpty(pazienteEntity.MedicoBase_Codice) Then

            Dim idMedico As New Onit.OnAssistnet.Contracts.ID()

            idMedico.IDValue = pazienteEntity.MedicoBase_Codice
            idMedico.TipoCodificatoID = Contracts.TipoCodificatoID.Regionale

            If pazienteEntity.MedicoBase_DataDecorrenza > Date.MinValue Then
                idMedico.DataInizioValidita = pazienteEntity.MedicoBase_DataDecorrenza
                idMedico.DataInizioValiditaSpecified = True
            End If

        End If

        If pazienteEntity.MedicoBase_DataDecorrenza > Date.MinValue Then
            pazienteContract.DatiSanitari.DataSceltaMedico = pazienteEntity.MedicoBase_DataDecorrenza
            pazienteContract.DatiSanitari.DataSceltaMedicoSpecified = True
        End If

        If pazienteEntity.MedicoBase_DataScadenza > Date.MinValue Then
            pazienteContract.DatiSanitari.DataScadenzaMedico = pazienteEntity.MedicoBase_DataScadenza
            pazienteContract.DatiSanitari.DataScadenzaMedicoSpecified = True
        End If

        ' Decesso
        If pazienteEntity.DataDecesso > Date.MinValue Then
            pazienteContract.DatiAnagrafici.DataDecesso = pazienteEntity.DataDecesso
            pazienteContract.DatiAnagrafici.DataDecessoSpecified = True
        End If

        ' Data immigrazione
        If pazienteEntity.DataImmigrazione > Date.MinValue Then
            pazienteContract.DatiAnagrafici.DataImmigrazione = pazienteEntity.DataImmigrazione
            pazienteContract.DatiAnagrafici.DataImmigrazioneSpecified = True
        End If

        Return pazienteContract

    End Function

#End Region

#Region " Caricamento paziente "

    Private Function LoadPaziente(codicePaziente As String) As Entities.Paziente

        Dim pazienteEntity As Entities.Paziente = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizPaziente As New Biz.BizPaziente(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                ' N.B. : questa maschera è utilizzabile solo in modalità locale e non in centrale
                pazienteEntity = bizPaziente.CercaPaziente(codicePaziente)

            End Using

            If pazienteEntity Is Nothing Then Return Nothing

        End Using

        Return pazienteEntity

    End Function

#End Region

#Region " AllineaPazienteInfo "

    Private Function GetAllineaPazienteInfo(ByVal operazione As Onit.OnAssistnet.MID.OperazionePaziente) As AllineaPazienteInfo

        Dim allineaPazienteInfo As New AllineaPazienteInfo()

        allineaPazienteInfo.Operazione = operazione
        allineaPazienteInfo.UserID = OnVacContext.UserId
        allineaPazienteInfo.EventDate = DateTime.Now

        Return allineaPazienteInfo

    End Function

#End Region

#End Region


End Class
