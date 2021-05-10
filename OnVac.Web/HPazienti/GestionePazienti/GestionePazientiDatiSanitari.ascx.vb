Imports System.Collections.Generic
Imports System.Text
Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.Biz
Imports Onit.OnAssistnet.OnVac.Common
Imports Onit.OnAssistnet.OnVac.DAL


Partial Public Class GestionePazientiDatiSanitari
    Inherits UserControlPageBase

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

#Region " Classes "

    Public Class PazienteClass
        Public Property Codice As Integer
        Public Property DataNascita As Date
        Public Property CategorieRischio As String
        Public Property Sesso As String
    End Class

#End Region

#Region " Properties "

    Public Property ControlState() As ControlStateEnum
        Get
            Return ViewState("gestionepazientidatisanitari_controlstate")
        End Get
        Set(value As ControlStateEnum)
            If dgrCicli.EditItemIndex > -1 Or dgrMalattie.EditItemIndex > -1 Or dgrMantoux.EditItemIndex > -1 Then
                value = ControlStateEnum.LOCK
            End If
            Dim previous As ControlStateEnum = ViewState("gestionepazientidatisanitari_controlstate")
            If previous <> value Then
                ViewState("gestionepazientidatisanitari_controlstate") = value
                RaiseEvent OnStateChanged(Me, New StateChangedEventArgs(value))
            End If
        End Set
    End Property

    Private _paziente As PazienteClass
    Public Property Paziente() As PazienteClass
        Get
            Return _paziente
        End Get
        Set(value As PazienteClass)
            _paziente = value
        End Set
    End Property

#Region " Session properties "

    Public Property gestisciCategoriaARischio() As Boolean
        Get
            Return Session("DatiSanitariPazienti_gestisciCategoriaARischio")
        End Get
        Set(Value As Boolean)
            Session("DatiSanitariPazienti_gestisciCategoriaARischio") = Value
        End Set
    End Property

    Public Property dtaCicli() As DataTable
        Get
            Return Session("DatiSanitariPazienti_dtaCicli")
        End Get
        Set(Value As DataTable)
            Session("DatiSanitariPazienti_dtaCicli") = Value
        End Set
    End Property

    'datatable contenente i cicli eliminati per il paziente
    Public Property dtaCicliEliminati() As DataTable
        Get
            Return Session("DatiSanitariPazienti_dtaCicliEliminati")
        End Get
        Set(Value As DataTable)
            Session("DatiSanitariPazienti_dtaCicliEliminati") = Value
        End Set
    End Property

    'datatable contenente i cicli aggiunti per il paziente
    Public Property dtaCicliAggiunti() As DataTable
        Get
            Return Session("DatiSanitariPazienti_dtaCicliAggiunti")
        End Get
        Set(Value As DataTable)
            Session("DatiSanitariPazienti_dtaCicliAggiunti") = Value
        End Set
    End Property

    Public Property dtaMantoux() As DataTable
        Get
            Return Session("DatiSanitariPazienti_dtaMantoux")
        End Get
        Set(Value As DataTable)
            Session("DatiSanitariPazienti_dtaMantoux") = Value
        End Set
    End Property

    Public Property dtaMalattie() As dsMalattie.MalattieDataTable
        Get
            Return Session("DatiSanitariPazienti_dtaMalattie")
        End Get
        Set(Value As dsMalattie.MalattieDataTable)
            Session("DatiSanitariPazienti_dtaMalattie") = Value
        End Set
    End Property

#End Region

#End Region

#Region " Enums "

    Public Enum ControlStateEnum
        VIEW
        EDIT
        LOCK
    End Enum

    Private Enum ColumnIndexDgrCicli
        ButtonColumn = 0
        Ciclo = 1
    End Enum

    Private Enum ColumnIndexDgrMantoux
        ButtonColumn = 0
        EditColumn = 1
        DataMantoux = 2
        EseguitaDa = 3
        MM = 4
        Medico = 5
        FlagEseguita = 6
        FlagPositiva = 7
        DataInvio = 8
    End Enum

    Private Enum ColumnIndexDgrMalattie
        ButtonColumn = 0
        EditColumn = 1
        Malattia = 2
        FlagFollowUp = 3
        FlagNuovaDiagnosi = 4
        DataDiagnosi = 5
        DataUltimaVisita = 6
        BilancioPartenza = 7
        FrecceOrdineBilanci = 8
        NumeroGravitaMalattia = 9
    End Enum

#End Region

#Region " Events "

    Public Event OnInsertRoutineJS(sender As Object, e As InsertRoutineJSEventArgs)
    Public Event OnStateChanged(sender As Object, e As StateChangedEventArgs)
    Public Event OnAlert(sender As Object, e As UserControlEventArgs)

    Public Class StateChangedEventArgs

        Public ControlState As ControlStateEnum

        Public Sub New(controlState As ControlStateEnum)
            Me.ControlState = controlState
        End Sub

    End Class

    Public Class InsertRoutineJSEventArgs

        Public JS As String

        Public Sub New(js As String)
            Me.JS = js
        End Sub

    End Class

#End Region

#Region " Public Variables "

    Public CicliEliminatiStrJS As String
    Public CicliEliminatiMessaggioStrJS As String
    Public ControllaElimProg As Boolean = True
    Public ImpostaCicli As Boolean = False

#End Region

#Region " Costruttori "

    Public Sub New()

        Me.Paziente = New PazienteClass()

    End Sub

#End Region

#Region " Inizializzazione e caricamento dati "

    Protected Overrides Sub OnInit(e As EventArgs)

        MyBase.OnInit(e)

        If Not IsPostBack() Then

            gestisciCategoriaARischio = False

            'inizializzo eventuali oggetti del layout e sessioni
            If Not dtaCicli Is Nothing Then dtaCicli.Dispose()
            If Not dtaMantoux Is Nothing Then dtaMantoux.Clear()
            If Not dtaMalattie Is Nothing Then dtaMalattie.Clear()

            dtaCicli = New DataTable()
            dtaMantoux = New DataTable()
            dtaMalattie = New dsMalattie.MalattieDataTable()

            'inizializzazione del datatable di sessione dei cicli eliminati
            dtaCicliEliminati = New DataTable()
            dtaCicliEliminati.Columns.Add(New DataColumn("PAC_CIC_CODICE"))
            dtaCicliEliminati.Columns.Add(New DataColumn("CIC_DESCRIZIONE"))

        End If

    End Sub

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        Diagnostics.Trace.Write("GestionePazientiDatiSanitari_Page_Load")

        If dgrMalattie.EditItemIndex > -1 Then

            Dim control As Control = dgrMalattie.Items(dgrMalattie.EditItemIndex).FindControl("txtMalattia")

            If Not control Is Nothing Then
                AddHandler DirectCast(control, Onit.Controls.OnitModalList).Change, AddressOf txtMalattia_change
            End If

        End If

        Select Case Request.Form("__EVENTTARGET")

            Case "Nuovo"

                Datagrid_ItemCommand(FindControl(Request.Form("__EVENTARGUMENT")), New DataGridCommandEventArgs(Nothing, "Nuovo", New CommandEventArgs("Nuovo", Request.Form("__EVENTARGUMENT"))))

        End Select

    End Sub

    Public Sub BindControls()

        OnVacSceltaCicli.DataNascita = Paziente.DataNascita
        OnVacSceltaCicli.Sesso = Paziente.Sesso

    End Sub

    Sub ReloadAll(DAM As IDAM)

        Dim testataLogCicliAggiuntiAutomatico As Testata = New Testata(DataLogStructure.TipiArgomento.PAZIENTI, Operazione.Inserimento, True)

        ' ---- Cicli ---- '
        dtaCicli.Rows.Clear()

        DAM.QB.NewQuery()
        DAM.QB.AddSelectFields("PAC_CIC_CODICE, CIC_DESCRIZIONE")
        DAM.QB.AddTables("T_ANA_CICLI, T_PAZ_CICLI")
        DAM.QB.AddWhereCondition("PAC_CIC_CODICE", Comparatori.Uguale, "CIC_CODICE", DataTypes.Join)
        DAM.QB.AddWhereCondition("PAC_PAZ_CODICE", Comparatori.Uguale, Paziente.Codice, DataTypes.Numero)

        DAM.BuildDataTable(dtaCicli)

        ' Impostazione cicli di default: se non ci soono cicli aggiunge quelli di default presi dalla T_ANA_CICLI
        ' Controllo che sia impostata la data di nascita
        If dtaCicli.Rows.Count = 0 And Paziente.DataNascita <> Date.MinValue AndAlso Not testataLogCicliAggiuntiAutomatico Is Nothing Then

            dtaCicli.Rows.Clear()

            DAM.QB.NewQuery()
            DAM.QB.AddSelectFields(Paziente.Codice, "CIC_CODICE")
            DAM.QB.AddTables("T_ANA_CICLI")
            DAM.QB.AddWhereCondition("CIC_DATA_INTRODUZIONE", Comparatori.MinoreUguale, Paziente.DataNascita, DataTypes.Data)
            DAM.QB.OpenParanthesis()
            DAM.QB.AddWhereCondition("CIC_DATA_FINE", Comparatori.MaggioreUguale, Paziente.DataNascita, DataTypes.Data)
            DAM.QB.AddWhereCondition("CIC_DATA_FINE", Comparatori.Is, "NULL", DataTypes.Data, "OR")
            DAM.QB.CloseParanthesis()
            DAM.QB.AddWhereCondition("CIC_STANDARD", Comparatori.Uguale, "T", DataTypes.Stringa)
            '-- filtro in base al sesso 21/02/2008 MGR
            If Paziente.Sesso <> String.Empty Then
                DAM.QB.OpenParanthesis()
                DAM.QB.AddWhereCondition("T_ANA_CICLI.cic_sesso", Comparatori.Uguale, Paziente.Sesso, DataTypes.Stringa)
                DAM.QB.AddWhereCondition("T_ANA_CICLI.cic_sesso", Comparatori.Uguale, "E", DataTypes.Stringa, "OR")
                DAM.QB.CloseParanthesis()
            End If
            '-- fine MGR

            If Paziente.Codice >= 0 Then

                Dim SQL As String = DAM.QB.GetSelect()
                DAM.QB.NewQuery(False, False)
                DAM.QB.AddInsertField("PAC_PAZ_CODICE, PAC_CIC_CODICE", SQL, DataTypes.Replace)
                DAM.QB.AddTables("T_PAZ_CICLI")
                DAM.ExecNonQuery(ExecQueryType.Insert)

                'Riesegue la query per recuperare i cicli di default
                DAM.QB.NewQuery()
                DAM.QB.AddSelectFields("PAC_CIC_CODICE, CIC_DESCRIZIONE")
                DAM.QB.AddTables("T_ANA_CICLI, T_PAZ_CICLI")
                DAM.QB.AddWhereCondition("PAC_CIC_CODICE", Comparatori.Uguale, "CIC_CODICE", DataTypes.Join)
                DAM.QB.AddWhereCondition("PAC_PAZ_CODICE", Comparatori.Uguale, Paziente.Codice, DataTypes.Numero)
            Else
                DAM.QB.AddSelectFields("CIC_DESCRIZIONE", "CIC_CODICE AS PAC_CIC_CODICE")
            End If

            DAM.BuildDataTable(dtaCicli)

#Region "OnVac.Log "

            Dim recordLog As New Record()

            For i As Int16 = 0 To dtaCicli.Rows.Count - 1
                recordLog.Campi.Add(New Campo("PAC_CIC_CODICE", "", dtaCicli.Rows(i)("PAC_CIC_CODICE")))
            Next

            testataLogCicliAggiuntiAutomatico.Records.Add(recordLog)

            If testataLogCicliAggiuntiAutomatico.Records.Count > 0 Then LogBox.WriteData(testataLogCicliAggiuntiAutomatico)

#End Region

            'registrazione cicli aggiunti [modifica 09/06/2006]
            If dtaCicliAggiunti Is Nothing Then
                dtaCicliAggiunti = New DataTable()
                dtaCicliAggiunti = dtaCicli.Clone()
            End If

            Dim rowCicliAggiunti As DataRow

            For count As Integer = 0 To dtaCicli.Rows.Count - 1
                rowCicliAggiunti = dtaCicliAggiunti.NewRow()
                rowCicliAggiunti.Item("PAC_CIC_CODICE") = dtaCicli.Rows(count)("PAC_CIC_CODICE")
                rowCicliAggiunti.Item("CIC_DESCRIZIONE") = dtaCicli.Rows(count)("CIC_DESCRIZIONE")
                dtaCicliAggiunti.Rows.Add(rowCicliAggiunti)
            Next

        End If
        ' fine SIO 18/8/2003

        dgrCicli.DataSource = dtaCicli
        dgrCicli.DataBind()

        Using genericProvider As New DbGenericProvider(DAM)

            Using bizMalattie As New BizMalattie(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), New BizLogOptions(DataLogStructure.TipiArgomento.PAZIENTI, True))
                dtaMalattie = bizMalattie.LoadMalattiePazienteDefault(Paziente.Codice)
            End Using

            Using bizPaziente As New BizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                dtaMantoux.Rows.Clear()
                dtaMantoux = bizPaziente.GetDtMantouxPaziente(Paziente.Codice)
            End Using

        End Using

        dgrMalattie.DataSource = dtaMalattie
        dgrMalattie.DataBind()

        dgrMantoux.DataSource = dtaMantoux
        dgrMantoux.DataBind()

    End Sub

    Sub ReloadMalattie()
        dgrMalattie.DataSource = dtaMalattie
        dgrMalattie.DataBind()
    End Sub

    Sub ReloadMalattie(dtMalattie As dsMalattie.MalattieDataTable)
        dtaMalattie = dtMalattie
        ReloadMalattie()
    End Sub

    Public Sub AddMalattie(dtMalattie As dsMalattie.MalattieDataTable)

        For i As Integer = 0 To dtMalattie.Rows.Count - 1

            Dim rCentrale As dsMalattie.MalattieRow = dtMalattie.Rows(i)

            Dim inserisciMalattia As Boolean = True

            For j As Integer = 0 To dtaMalattie.Rows.Count - 1
                Dim rLocale As dsMalattie.MalattieRow = dtaMalattie.Rows(j)
                If rCentrale.PMA_MAL_CODICE = rLocale.PMA_MAL_CODICE Then
                    inserisciMalattia = False
                    Exit For
                End If
            Next

            If inserisciMalattia Then
                dtaMalattie.ImportRow(rCentrale)
            End If
        Next

    End Sub

    Private Sub CaricaBilancioPartenza(idxRiga As Integer)

        Dim codiceMalattia As String = DirectCast(dgrMalattie.Items(idxRiga).FindControl("txtMalattia"), Onit.Controls.OnitModalList).Codice

        Dim dt As New DataTable()

        ' Caricamento valori di riempimento
        Using DAM As IDAM = OnVacUtility.OpenDam()

            DAM.QB.NewQuery()

            DAM.QB.AddSelectFields("BIL_NUMERO")
            DAM.QB.AddTables("T_ANA_BILANCI")
            DAM.QB.AddWhereCondition("BIL_MAL_CODICE", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)
            DAM.QB.AddOrderByFields("BIL_NUMERO")

            DAM.BuildDataTable(dt)

        End Using

        ' Bind dropDownList cmbBilancioPartenza
        Dim cmbBilancioPartenza As DropDownList = DirectCast(dgrMalattie.Items(idxRiga).FindControl("cmbBilancioPartenza"), System.Web.UI.WebControls.DropDownList)

        If cmbBilancioPartenza Is Nothing Then Exit Sub

        If dt.Rows.Count = 0 Then
            cmbBilancioPartenza.Enabled = False
            Exit Sub
        End If

        cmbBilancioPartenza.DataSource = dt
        cmbBilancioPartenza.DataBind()

        Dim bilSelected As String = String.Empty

        If Not dtaMalattie Is Nothing Then

            Dim dv As New DataView(dtaMalattie)
            dv.RowFilter = String.Format("PMA_MAL_CODICE='{0}'", codiceMalattia)

            If dv.Count > 0 Then
                bilSelected = dv(0)("PMA_N_BILANCIO_PARTENZA").ToString()
            End If

        End If

        If bilSelected <> String.Empty Then

            Dim listItem As ListItem = cmbBilancioPartenza.Items.FindByValue(bilSelected)

            If Not listItem Is Nothing Then
                listItem.Selected = True
            Else
                cmbBilancioPartenza.Items(0).Selected = True
            End If

        End If

    End Sub

    'elimina la programmazione alla modifica della categoria rischio
    Private Sub EliminaProgrammazioneRischio(codNuovaCat As String)

        Using dam As IDAM = OnVacUtility.OpenDam()

            'se si elimina la categoria rischio deve solamente eliminare la programmazione
            If codNuovaCat = String.Empty Then

                ' Cancellazione convocazioni che non hanno cicli e/o data di appuntamento associati e cancellazione cicli del paziente
                Using genericProvider As New DbGenericProvider(dam)

                    EliminaProgrammazione(genericProvider)

                End Using

                ReloadAll(dam)

                Exit Sub

            End If

            Dim dtCicliCat As New DataTable()
            Dim dtCicliPaz As New DataTable()

            'recupero dei cicli associati alla categoria
            With dam.QB

                .NewQuery()

                .AddSelectFields("CIC_CODICE PAC_CIC_CODICE", "CIC_DESCRIZIONE")
                .AddTables("T_ANA_CICLI", "T_ANA_LINK_RISCHIO_CICLI")

                .AddWhereCondition("RCI_CIC_CODICE", Comparatori.Uguale, "CIC_CODICE", DataTypes.OutJoinLeft)
                .AddWhereCondition("RCI_RSC_CODICE", Comparatori.Uguale, codNuovaCat, DataTypes.Stringa)

                'filtro sulla data di nascita del paziente
                .OpenParanthesis()
                .AddWhereCondition("CIC_DATA_INTRODUZIONE", Comparatori.MinoreUguale, Paziente.DataNascita, DataTypes.Data)
                .AddWhereCondition("CIC_DATA_INTRODUZIONE", Comparatori.Is, "null", DataTypes.Replace, "OR")
                .CloseParanthesis()

                .OpenParanthesis()
                .AddWhereCondition("CIC_DATA_FINE", Comparatori.MaggioreUguale, Paziente.DataNascita, DataTypes.Data)
                .AddWhereCondition("CIC_DATA_FINE", Comparatori.Is, "null", DataTypes.Replace, "OR")
                .CloseParanthesis()

                '-- filtro in base al sesso 21/02/2008 MGR
                If Paziente.Sesso <> String.Empty Then
                    .OpenParanthesis()
                    .AddWhereCondition("cic_sesso", Comparatori.Uguale, Paziente.Sesso, DataTypes.Stringa)
                    .AddWhereCondition("cic_sesso", Comparatori.Uguale, "E", DataTypes.Stringa, "OR")
                    .CloseParanthesis()
                End If
                '-- fine MGR

            End With

            dam.BuildDataTable(dtCicliCat)

            'Maurizio 23-05-05
            'aggiorno il datatable principale con i cicli associati alla Categoria Rischio selezionata
            dtaCicli.Dispose()

            'Maurizio 07-06-05
            'se il datatable principale ha meno righe del datatable dei cicli associati alla categoria occorre aggiungere le righe mancanti
            For j As Integer = dtaCicli.Rows.Count To dtCicliCat.Rows.Count - 1
                dtaCicli.Rows.Add(dtaCicli.NewRow())
            Next

            For i As Integer = 0 To dtCicliCat.Rows.Count - 1
                dtaCicli.Rows(i).Item(0) = dtCicliCat.Rows(i).Item(0)
            Next

            'recupero dei cicli associati al paziente
            With dam.QB
                .NewQuery()
                .AddSelectFields("PAC_CIC_CODICE")
                .AddTables("T_PAZ_CICLI")
                .AddWhereCondition("PAC_PAZ_CODICE", Comparatori.Uguale, Paziente.Codice, DataTypes.Numero)
            End With

            dam.BuildDataTable(dtCicliPaz)

            If dtCicliCat.Rows.Count <> 0 Then

                'se i cicli della categoria sono uguali a quelli del paziente, no eliminazione
                Dim cicliCatDiversiCicliPaz As Boolean = False
                Dim soloCicliCat As Boolean = False

                If dtCicliPaz.Rows.Count > 0 Then

                    For count As Integer = 0 To dtCicliCat.Rows.Count - 1

                        For cont As Integer = 0 To dtCicliPaz.Rows.Count - 1
                            If dtCicliCat.Rows(count)("PAC_CIC_CODICE") <> dtCicliPaz.Rows(cont)("PAC_CIC_CODICE") Then
                                cicliCatDiversiCicliPaz = True
                                Exit For
                            End If
                        Next

                        If cicliCatDiversiCicliPaz Then Exit For

                    Next

                Else

                    soloCicliCat = True

                End If

                If cicliCatDiversiCicliPaz Or soloCicliCat Then

                    Using genericProvider As New DbGenericProvider(dam)

                        If Not soloCicliCat Then

                            ' Elimina convocazioni e cicli del paziente
                            EliminaProgrammazione(genericProvider)

                        End If

                        ' Inserimento cicli
                        For count As Integer = 0 To dtCicliCat.Rows.Count - 1
                            genericProvider.Cicli.InsertCicloPaziente(Paziente.Codice, dtCicliCat.Rows(count)("PAC_CIC_CODICE").ToString())
                        Next

                    End Using

                    ImpostaCicli = True

                End If

            End If

        End Using

    End Sub

    ' Elimina la programmazione e i cicli impostati per il paziente
    Private Sub EliminaProgrammazione(genericProvider As DbGenericProvider)

        ' Cancellazione programmazione del paziente
        Using bizVaccinazioneProg As New BizVaccinazioneProg(genericProvider, Settings, OnVacContext.CreateBizContextInfos, Nothing)

            Dim command As New BizVaccinazioneProg.EliminaProgrammazioneCommand()
            command.CodicePaziente = Paziente.Codice
            command.DataConvocazione = Nothing
            command.EliminaAppuntamenti = False
            command.EliminaBilanci = False
            command.EliminaSollecitiBilancio = False
            command.TipoArgomentoLog = DataLogStructure.TipiArgomento.ELIMINA_PROG
            command.OperazioneAutomatica = False

            ' N.B. : non vengono cancellate le cnv con appuntamenti, quindi non c'è bisogno di specificare i dati di eliminazione perchè non li usa.
            command.IdMotivoEliminazione = String.Empty
            command.NoteEliminazione = String.Empty

            bizVaccinazioneProg.EliminaProgrammazione(command)

        End Using

        ' Cancellazione dei cicli precedenti
        genericProvider.Cicli.DeleteCicliPaziente(Paziente.Codice)

    End Sub

#End Region

#Region " Public Methods "

    ''' <summary>
    ''' Reperisce LATO CLIENT il valore di un campo da un datatable eseguendo alcune conversioni di tipo
    ''' </summary>
    ''' <param name="index">Indice di riga. E' quasi sempre 0</param>
    ''' <param name="item">Il nome della colonna</param>
    ''' <param name="dtaTable">Il datatable contenete i dati. Se non impostato genera eccezione</param>
    Public Function RitornaValore(index As Integer, item As String, Optional dtaTable As Object = Nothing) As String

        Dim ret As Object

        If dtaTable Is Nothing Then Throw New Exception("dtaTable is nothing")
        If dtaTable.GetType Is GetType(DataRowView) Then
            ret = dtaTable(item)
        Else
            ret = dtaTable.Rows(index).Item(item)
        End If

        If ret Is DBNull.Value Then ret = ret & ""
        If IsDate(ret) Then ret = CType(ret.Date, String)

        Return ret

    End Function

    'Cambia i dati per un paziente
    Public Sub ModificaPaziente(DAM As IDAM)

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, OnVacUtility.GetReadCommittedTransactionOptions)

            ' Check della presenza della malattia con codice CODNOMAL
            Dim found As Boolean = False

            For i As Int16 = 0 To dtaMalattie.Rows.Count - 1
                If dtaMalattie.Rows(i)("PMA_MAL_CODICE") = Settings.CODNOMAL Then
                    found = True
                    Exit For
                End If
            Next

            If Not found Then Throw New ApplicationException("Malattia 'NESSUNA' non presente in fase di salvataggio")

            Dim strErrMsg As String = ""
            Dim aggiunto, eliminato As Boolean

            ' Variabili per il log
            Dim testataLogCicliAggiunti As Testata = New Testata(DataLogStructure.TipiArgomento.PAZIENTI, Operazione.Inserimento, False)
            Dim testataLogCicliEliminati As Testata = New Testata(DataLogStructure.TipiArgomento.PAZIENTI, Operazione.Eliminazione, False)
            Dim testataLogMantouxEliminate As Testata = New Testata(DataLogStructure.TipiArgomento.PAZIENTI, Operazione.Eliminazione, False)
            Dim testataLogMantouxAggiunte As Testata = New Testata(DataLogStructure.TipiArgomento.PAZIENTI, Operazione.Inserimento, False)
            Dim testataLogMantouxModificate As Testata = New Testata(DataLogStructure.TipiArgomento.PAZIENTI, Operazione.Modifica, False)

            Dim recordLog1, recordLog2 As Record

            dtaCicli.AcceptChanges()

            '---------------------------------------------------------------------
            ' Eliminazione della programmazione relativa alle categorie a rischio
            '---------------------------------------------------------------------
            If gestisciCategoriaARischio Then
                EliminaProgrammazioneRischio(Paziente.CategorieRischio)
            End If
            '--
            If Not ImpostaCicli Then
                '--
                Dim dtCicliPrecedenti As New DataTable()

                DAM.QB.NewQuery()
                DAM.QB.AddSelectFields("PAC_CIC_CODICE")
                DAM.QB.AddTables("T_PAZ_CICLI")
                DAM.QB.AddWhereCondition("PAC_PAZ_CODICE", Comparatori.Uguale, Paziente.Codice, DataTypes.Numero)
                DAM.BuildDataTable(dtCicliPrecedenti)

                DAM.QB.NewQuery()
                DAM.QB.AddTables("T_PAZ_CICLI")
                DAM.QB.AddWhereCondition("PAC_PAZ_CODICE", Comparatori.Uguale, Paziente.Codice, DataTypes.Numero)
                DAM.ExecNonQuery(ExecQueryType.Delete)

                DAM.QB.NewQuery()
                DAM.QB.AddTables("T_PAZ_CICLI")
                DAM.QB.AddInsertField("PAC_PAZ_CODICE", Paziente.Codice, DataTypes.Numero)
                DAM.QB.AddInsertField("PAC_CIC_CODICE", " ", DataTypes.Stringa)

                For i As Int16 = 0 To dtaCicli.Rows.Count - 1
                    DAM.QB.ChangeValue(1, ExecQueryType.Insert, dtaCicli.Rows(i).Item("PAC_CIC_CODICE"))
                    DAM.ExecNonQuery(ExecQueryType.Insert)
                Next

                '------------------------------------
                ' LOG 
                recordLog1 = New Record()
                '--
                For i As Int16 = 0 To dtaCicli.Rows.Count - 1

                    aggiunto = True

                    For j As Int16 = 0 To dtCicliPrecedenti.Rows.Count - 1
                        If dtaCicli.Rows(i).RowState <> DataRowState.Deleted AndAlso dtaCicli.Rows(i)("PAC_CIC_CODICE") = dtCicliPrecedenti.Rows(j).Item("PAC_CIC_CODICE") Then
                            aggiunto = False
                            Exit For
                        End If
                    Next

                    If aggiunto Then recordLog1.Campi.Add(New Campo("PAC_CIC_CODICE", "", dtaCicli.Rows(i)("PAC_CIC_CODICE")))

                Next
                '--
                If recordLog1.Campi.Count > 0 Then testataLogCicliAggiunti.Records.Add(recordLog1)
                '--
                recordLog2 = New Record()
                '--
                For i As Int16 = 0 To dtCicliPrecedenti.Rows.Count - 1

                    eliminato = True

                    For j As Int16 = 0 To dtaCicli.Rows.Count - 1
                        If dtaCicli.Rows(j).RowState <> DataRowState.Deleted AndAlso dtaCicli.Rows(j)("PAC_CIC_CODICE") = dtCicliPrecedenti.Rows(i)("PAC_CIC_CODICE") Then
                            eliminato = False
                            Exit For
                        End If
                    Next

                    If eliminato Then recordLog2.Campi.Add(New Campo("PAC_CIC_CODICE", dtCicliPrecedenti.Rows(i)("PAC_CIC_CODICE")))

                Next
                '--
                If recordLog2.Campi.Count > 0 Then testataLogCicliEliminati.Records.Add(recordLog2)
                '--
                If Not testataLogCicliAggiunti Is Nothing AndAlso testataLogCicliAggiunti.Records.Count > 0 Then LogBox.WriteData(testataLogCicliAggiunti)
                If Not testataLogCicliEliminati Is Nothing AndAlso testataLogCicliEliminati.Records.Count > 0 Then LogBox.WriteData(testataLogCicliEliminati)
                '------------------------------------
            End If


            '--------------------------------------------------------------------------------------
            ' Elimina la programmazione associata all'eliminazione dei cicli (modifica 17/04/2004)
            '--------------------------------------------------------------------------------------
            If dtaCicliEliminati.Rows.Count <> 0 And ControllaElimProg Then
                'deve eliminare la programmazione associata al ciclo specificato [modifica 24/01/2006]
                EliminaProgrammazioneCicli(Paziente.Codice, dtaCicliEliminati)
            End If

            dtaCicliEliminati.Rows.Clear()

            CicliEliminatiStrJS = String.Empty
            CicliEliminatiMessaggioStrJS = String.Empty


            '-----------------------------------------------------------------
            ' Cancellazione mantoux precedenti + inserimento mantoux correnti
            '-----------------------------------------------------------------
            Dim dtMantouxPrecedenti As New DataTable()

            DAM.QB.NewQuery()
            DAM.QB.AddSelectFields("MAN_DATA, MAN_DESCRIZIONE, MAN_SINO, MAN_MM, MAN_DATA_INVIO, MAN_OPE_CODICE, MAN_POSITIVA")
            DAM.QB.AddTables("T_PAZ_MANTOUX")
            DAM.QB.AddWhereCondition("MAN_PAZ_CODICE", Comparatori.Uguale, Paziente.Codice, DataTypes.Numero)
            DAM.BuildDataTable(dtMantouxPrecedenti)

            DAM.QB.NewQuery()
            DAM.QB.AddTables("T_PAZ_MANTOUX")
            DAM.QB.AddWhereCondition("MAN_PAZ_CODICE", Comparatori.Uguale, Paziente.Codice, DataTypes.Numero)
            DAM.ExecNonQuery(ExecQueryType.Delete)

            For i As Int16 = 0 To dtaMantoux.Rows.Count - 1
                DAM.QB.NewQuery()
                DAM.QB.AddTables("T_PAZ_MANTOUX")
                DAM.QB.AddInsertField("MAN_PAZ_CODICE", Paziente.Codice, DataTypes.Numero)
                DAM.QB.AddInsertField("MAN_DATA", dtaMantoux.Rows(i)("MAN_DATA"), DataTypes.Data)
                DAM.QB.AddInsertField("MAN_DESCRIZIONE", dtaMantoux.Rows(i)("MAN_DESCRIZIONE"), DataTypes.Stringa)
                DAM.QB.AddInsertField("MAN_SINO", dtaMantoux.Rows(i)("MAN_SINO"), DataTypes.Stringa)
                DAM.QB.AddInsertField("MAN_POSITIVA", dtaMantoux.Rows(i)("MAN_POSITIVA"), DataTypes.Stringa)
                DAM.QB.AddInsertField("MAN_MM", dtaMantoux.Rows(i)("MAN_MM"), DataTypes.Stringa)
                DAM.QB.AddInsertField("MAN_OPE_CODICE", dtaMantoux.Rows(i)("MAN_OPE_CODICE"), DataTypes.Stringa)
                DAM.QB.AddInsertField("MAN_DATA_INVIO", dtaMantoux.Rows(i)("MAN_DATA_INVIO"), DataTypes.Data)
                DAM.ExecNonQuery(ExecQueryType.Insert)
            Next

            '------------------------------------
            ' LOG
            Dim modificato As Boolean
            Dim recordLog3 As New Record()

            recordLog1 = New Record()

            For i As Int16 = 0 To dtaMantoux.Rows.Count - 1

                aggiunto = True
                modificato = False

                For j As Int16 = 0 To dtMantouxPrecedenti.Rows.Count - 1

                    If dtaMantoux.Rows(i).RowState <> DataRowState.Deleted AndAlso dtaMantoux.Rows(i)("MAN_DATA") = dtMantouxPrecedenti.Rows(j)("MAN_DATA") Then

                        aggiunto = False

                        'controllo modifica
                        If Not dtaMantoux.Rows(i).ItemArray Is dtMantouxPrecedenti.Rows(j).ItemArray Then

                            modificato = True
                            aggiunto = False

                            If (dtaMantoux.Rows(i)("MAN_DESCRIZIONE") & "") <> (dtMantouxPrecedenti.Rows(j)("MAN_DESCRIZIONE") & "") Then recordLog3.Campi.Add(New Campo("MAN_DESCRIZIONE", dtMantouxPrecedenti.Rows(j)("MAN_DESCRIZIONE") & "", dtaMantoux.Rows(i)("MAN_DESCRIZIONE") & ""))
                            If (dtaMantoux.Rows(i)("MAN_SINO") & "") <> (dtMantouxPrecedenti.Rows(j)("MAN_SINO") & "") Then recordLog3.Campi.Add(New Campo("MAN_SINO", dtMantouxPrecedenti.Rows(j)("MAN_SINO") & "", dtaMantoux.Rows(i)("MAN_SINO") & ""))
                            If (dtaMantoux.Rows(i)("MAN_POSITIVA") & "") <> (dtMantouxPrecedenti.Rows(j)("MAN_POSITIVA") & "") Then recordLog3.Campi.Add(New Campo("MAN_POSITIVA", dtMantouxPrecedenti.Rows(j)("MAN_POSITIVA") & "", dtaMantoux.Rows(i)("MAN_POSITIVA") & ""))
                            If (dtaMantoux.Rows(i)("MAN_MM") & "") <> (dtMantouxPrecedenti.Rows(j)("MAN_MM") & "") Then recordLog3.Campi.Add(New Campo("MAN_MM", dtMantouxPrecedenti.Rows(j)("MAN_MM") & "", dtaMantoux.Rows(i)("MAN_MM") & ""))
                            If (dtaMantoux.Rows(i)("MAN_DATA_INVIO") & "") <> (dtMantouxPrecedenti.Rows(j)("MAN_DATA_INVIO") & "") Then recordLog3.Campi.Add(New Campo("MAN_DATA_INVIO", dtMantouxPrecedenti.Rows(j)("MAN_DATA_INVIO") & "", dtaMantoux.Rows(i)("MAN_DATA_INVIO") & ""))
                            If (dtaMantoux.Rows(i)("MAN_OPE_CODICE") & "") <> (dtMantouxPrecedenti.Rows(j)("MAN_OPE_CODICE") & "") Then recordLog3.Campi.Add(New Campo("MAN_OPE_CODICE", dtMantouxPrecedenti.Rows(j)("MAN_OPE_CODICE") & "", dtaMantoux.Rows(i)("MAN_OPE_CODICE") & ""))

                            If recordLog3.Campi.Count > 0 Then testataLogMantouxModificate.Records.Add(recordLog3)
                            recordLog3 = New Record()

                        End If

                        Exit For

                    End If

                Next

                If aggiunto Then
                    recordLog1.Campi.Add(New Campo("MAN_DATA", "", dtaMantoux.Rows(i)("MAN_DATA")))
                    recordLog1.Campi.Add(New Campo("MAN_DESCRIZIONE", "", dtaMantoux.Rows(i)("MAN_DESCRIZIONE") & ""))
                    recordLog1.Campi.Add(New Campo("MAN_SINO", "", dtaMantoux.Rows(i)("MAN_SINO") & ""))
                    recordLog1.Campi.Add(New Campo("MAN_POSITIVA", "", dtaMantoux.Rows(i)("MAN_POSITIVA") & ""))
                    recordLog1.Campi.Add(New Campo("MAN_MM", "", dtaMantoux.Rows(i)("MAN_MM") & ""))
                    recordLog1.Campi.Add(New Campo("MAN_DATA_INVIO", "", dtaMantoux.Rows(i)("MAN_DATA_INVIO") & ""))
                    recordLog1.Campi.Add(New Campo("MAN_OPE_CODICE", "", dtaMantoux.Rows(i)("MAN_OPE_CODICE") & ""))
                    If recordLog1.Campi.Count > 0 Then testataLogMantouxAggiunte.Records.Add(recordLog1)
                    recordLog1 = New Record()
                End If

            Next

            recordLog2 = New Record()
            recordLog3 = New Record()

            For i As Int16 = 0 To dtMantouxPrecedenti.Rows.Count - 1

                eliminato = True

                For j As Int16 = 0 To dtaMantoux.Rows.Count - 1
                    If dtaMantoux.Rows(j).RowState <> DataRowState.Deleted AndAlso dtaMantoux.Rows(j)("MAN_DATA") = dtMantouxPrecedenti.Rows(i)("MAN_DATA") Then
                        eliminato = False
                        Exit For
                    End If
                Next

                If eliminato Then
                    recordLog2.Campi.Add(New Campo("MAN_DATA", dtMantouxPrecedenti.Rows(i)("MAN_DATA")))
                    recordLog2.Campi.Add(New Campo("MAN_DESCRIZIONE", dtMantouxPrecedenti.Rows(i)("MAN_DESCRIZIONE") & ""))
                    recordLog2.Campi.Add(New Campo("MAN_SINO", dtMantouxPrecedenti.Rows(i)("MAN_SINO") & ""))
                    recordLog2.Campi.Add(New Campo("MAN_POSITIVA", dtMantouxPrecedenti.Rows(i)("MAN_POSITIVA") & ""))
                    recordLog2.Campi.Add(New Campo("MAN_MM", dtMantouxPrecedenti.Rows(i)("MAN_MM") & ""))
                    recordLog2.Campi.Add(New Campo("MAN_DATA_INVIO", dtMantouxPrecedenti.Rows(i)("MAN_DATA_INVIO") & ""))
                    recordLog2.Campi.Add(New Campo("MAN_OPE_CODICE", dtMantouxPrecedenti.Rows(i)("MAN_OPE_CODICE") & ""))

                    If recordLog2.Campi.Count > 0 Then testataLogMantouxEliminate.Records.Add(recordLog2)
                    recordLog2 = New Record()
                End If

            Next

            If Not testataLogMantouxAggiunte Is Nothing AndAlso testataLogMantouxAggiunte.Records.Count > 0 Then LogBox.WriteData(testataLogMantouxAggiunte)
            If Not testataLogMantouxEliminate Is Nothing AndAlso testataLogMantouxEliminate.Records.Count > 0 Then LogBox.WriteData(testataLogMantouxEliminate)
            If Not testataLogMantouxModificate Is Nothing AndAlso testataLogMantouxModificate.Records.Count > 0 Then LogBox.WriteData(testataLogMantouxModificate)
            '------------------------------------

            '--------------------------------------------------------------
            ' Salvataggio delle malattie + eliminazione convocazioni vuote
            '--------------------------------------------------------------
            Using genericProvider As New DbGenericProvider(DAM)

                Using bizMalattie As New BizMalattie(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), New BizLogOptions(DataLogStructure.TipiArgomento.PAZIENTI, False))

                    Dim result As BizMalattie.SalvaMalattiePazienteResult = bizMalattie.SalvaMalattiePaziente(Paziente.Codice, dtaMalattie)

                    If Not String.IsNullOrEmpty(result.Message) Then
                        RaiseEvent OnInsertRoutineJS(Me, New InsertRoutineJSEventArgs(String.Format("alert('{0}');", result.Message)))
                    End If

                End Using

                Using bizConvocazione As New BizConvocazione(genericProvider, Settings, OnVacContext.CreateBizContextInfos, Nothing)

                    ' Se ci sono convocazioni senza vaccinazioni o bilanci programmati, le elimino
                    Dim command As New BizConvocazione.EliminaConvocazioneEmptyCommand()
                    command.CodicePaziente = Paziente.Codice
                    command.DataConvocazione = Nothing
                    command.DataEliminazione = DateTime.Now
                    command.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.VariazioneCiclo
                    command.NoteEliminazione = "Eliminazione convocazioni paziente da maschera GestionePaziente"

                    bizConvocazione.EliminaConvocazioneEmpty(command)

                    'Aggiorno lo stato del controllo
                    ControlState = ControlStateEnum.VIEW

                End Using

            End Using
            '--------------------------------------------------------------

            transactionScope.Complete()

        End Using

    End Sub

    'controllo sui cicli eliminati da passare al client
    Public Sub RefreshMessaggiCicliEliminati()

        CicliEliminatiStrJS = String.Empty
        CicliEliminatiMessaggioStrJS = String.Empty

        Dim dtCicliAlert As New DataTable()

        'impostazione del nuovo stato
        Dim dam As IDAM = OnVacUtility.OpenDam()
        dam.QB.NewQuery()
        dam.QB.AddSelectFields("CIC_CODICE")
        dam.QB.AddTables("T_ANA_CICLI")
        dam.QB.AddWhereCondition("CIC_ALERT", Comparatori.Uguale, "S", DataTypes.Stringa)
        Try
            dam.BuildDataTable(dtCicliAlert)
        Finally
            OnVacUtility.CloseDam(dam)
        End Try

        If Not dtaCicliEliminati Is Nothing Then

            If dtaCicliEliminati.Rows.Count > 0 Then

                Dim stb As New System.Text.StringBuilder()

                For count As Integer = 0 To dtaCicliEliminati.Rows.Count - 1

                    '------------- Added by Alessandro De Simone 18/09/07 ------------
                    ' Controllo se il ciclo eliminato aveva delle convocazioni in campagna

                    dam = OnVacUtility.OpenDam()
                    dam.QB.NewQuery()
                    dam.QB.AddSelectFields("CNV_CAMPAGNA")
                    dam.QB.AddTables("T_ANA_CICLI,T_PAZ_CICLI,T_CNV_CICLI,T_CNV_CONVOCAZIONI")
                    dam.QB.AddWhereCondition("PAC_PAZ_CODICE", Comparatori.Uguale, Paziente.Codice, DataTypes.Numero)
                    dam.QB.AddWhereCondition("PAC_PAZ_CODICE", Comparatori.Uguale, "CNV_PAZ_CODICE", DataTypes.Join)
                    dam.QB.AddWhereCondition("PAC_PAZ_CODICE", Comparatori.Uguale, "CNC_CNV_PAZ_CODICE", DataTypes.Join)
                    dam.QB.AddWhereCondition("PAC_PAZ_CODICE", Comparatori.Uguale, "CNV_PAZ_CODICE", DataTypes.Join)
                    dam.QB.AddWhereCondition("PAC_CIC_CODICE", Comparatori.Uguale, dtaCicliEliminati.Rows(count)("PAC_CIC_CODICE"), DataTypes.Stringa)
                    dam.QB.AddWhereCondition("PAC_CIC_CODICE", Comparatori.Uguale, "CIC_CODICE", DataTypes.Join)
                    dam.QB.AddWhereCondition("PAC_CIC_CODICE", Comparatori.Uguale, "CNC_CIC_CODICE", DataTypes.Join)
                    dam.QB.AddWhereCondition("CNC_CNV_DATA", Comparatori.Uguale, "CNV_DATA", DataTypes.Join)

                    Try
                        Dim obj As Object = dam.ExecScalar()

                        If Not obj Is Nothing Then
                            Dim strCampagna As String = obj.ToString()
                            If strCampagna = "S" Then
                                stb.AppendFormat("\r{0} (con convocazione IN CAMPAGNA)", dtaCicliEliminati.Rows(count)("CIC_DESCRIZIONE").ToString())
                            Else
                                stb.AppendFormat("\r{0}", dtaCicliEliminati.Rows(count)("CIC_DESCRIZIONE").ToString())
                            End If
                        Else
                            stb.AppendFormat("\r{0}", dtaCicliEliminati.Rows(count)("CIC_DESCRIZIONE").ToString())
                        End If

                        CicliEliminatiStrJS &= stb.ToString().Replace("'", "\'")

                    Finally
                        OnVacUtility.CloseDam(dam)
                    End Try

                    If stb.Length > 0 Then stb.Remove(0, stb.Length)

                    For i As Int16 = 0 To dtCicliAlert.Rows.Count - 1
                        If (dtaCicliEliminati.Rows(count)("PAC_CIC_CODICE") = dtCicliAlert.Rows(i)("CIC_CODICE")) Then
                            stb.AppendFormat("\r{0}", dtaCicliEliminati.Rows(count)("CIC_DESCRIZIONE").ToString())
                        End If
                    Next

                    CicliEliminatiMessaggioStrJS &= stb.ToString().Replace("'", "\'")

                Next

            End If

        End If

    End Sub

    Public Sub Annulla()

        dtaCicliEliminati.Rows.Clear()

    End Sub

    'imposta lo stato del paziente secondo il controllo sui cicli
    Public Function ImpostaCicliEStatoVaccinale(ByRef genericProvider As DbGenericProvider)

        'controllo aggiunta cicli
        Dim count1, count2 As Integer
        Dim controllo As Boolean = False
        Dim control As Boolean = False

        'controllo che siano stati effettivamente aggiunti dei cicli dal richiamo della modale
        If Not dtaCicliAggiunti Is Nothing Then

            If dtaCicliAggiunti.Rows.Count > 0 Then

                Dim dtCicli As New DataTable()

                Using DAM As IDAM = OnVacUtility.OpenDam()

                    With DAM.QB
                        .NewQuery()
                        .AddSelectFields("PAC_CIC_CODICE, CIC_DESCRIZIONE")
                        .AddTables("T_ANA_CICLI, T_PAZ_CICLI")
                        .AddWhereCondition("PAC_CIC_CODICE", Comparatori.Uguale, "CIC_CODICE", DataTypes.Join)
                        .AddWhereCondition("PAC_PAZ_CODICE", Comparatori.Uguale, Paziente.Codice, DataTypes.Numero)
                    End With

                    DAM.BuildDataTable(dtCicli)

                End Using

                If dtCicli.Rows.Count = 0 Then
                    controllo = True
                Else
                    For count1 = 0 To dtaCicliAggiunti.Rows.Count - 1
                        control = False
                        For count2 = 0 To dtCicli.Rows.Count - 1
                            If dtaCicliAggiunti.Rows(count1)("PAC_CIC_CODICE") = dtCicli.Rows(count2)("PAC_CIC_CODICE") Then
                                control = True
                            End If
                        Next
                        If Not control Then
                            controllo = True
                            Exit For
                        End If
                    Next
                End If

            End If

            ' Modifica dello stato vaccinale del paziente
            If controllo Then

                Using bizPaziente As New Biz.BizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    bizPaziente.UpdateStatoVaccinalePaziente(Paziente.Codice, Enumerators.StatiVaccinali.InCorso)

                End Using

            End If

            'azzeramento del datatable contenente i cicli aggiunti
            dtaCicliAggiunti.Rows.Clear()

            Return controllo

        End If

        Return controllo

    End Function

    ' Abilita/Disabilita tutti i controlli
    Public Sub AbilitaLayout(enable As Boolean)

        dgrCicli.Columns(ColumnIndexDgrCicli.ButtonColumn).Visible = enable
        dgrMantoux.Columns(ColumnIndexDgrMantoux.ButtonColumn).Visible = enable
        dgrMalattie.Columns(ColumnIndexDgrMalattie.ButtonColumn).Visible = enable

        dgrMantoux.Columns(ColumnIndexDgrMantoux.EditColumn).Visible = enable
        dgrMalattie.Columns(ColumnIndexDgrMalattie.EditColumn).Visible = enable

        For Each item As DataGridItem In dgrMalattie.Items

            AbilitaPulsanteFreccia(item, "imgFrecciaSu", enable)
            AbilitaPulsanteFreccia(item, "imgFrecciaGiu", enable)

        Next

    End Sub

    Private Sub AbilitaPulsanteFreccia(item As DataGridItem, btnName As String, enable As Boolean)

        Dim objFreccia As Object = item.FindControl(btnName)

        If Not objFreccia Is Nothing Then

            DirectCast(objFreccia, LinkButton).Enabled = enable

        End If

    End Sub

    ' Campi da nascondere e disabilitare a seconda dei parametri
    Public Sub DisabilitaCampi()

        'se non vengono gestiti i bilanci, nella riga in edit del datagrid delle malattie non devono essere abilitati i campi relativi
        If Not Settings.GESBIL Then

            If dgrMalattie.EditItemIndex > -1 Then

                ' Checkbox nuova diagnosi
                Dim chk As CheckBox = DirectCast(dgrMalattie.Items(dgrMalattie.EditItemIndex).FindControl("chkNuovaDiagnosi"), CheckBox)
                If Not chk Is Nothing Then chk.Enabled = False

                ' Dropdownlist bilancio di partenza
                Dim ddl As DropDownList = DirectCast(dgrMalattie.Items(dgrMalattie.EditItemIndex).FindControl("cmbBilancioPartenza"), DropDownList)
                ddl.Enabled = False
                ddl.CssClass = "TextBox_stringa_disabilitato"

            End If

        End If

    End Sub

    Public Function ControllaCompatibilitaCicli() As Boolean

        If dtaCicli.Rows.Count = 0 Then
            Return True
        End If

        Dim DAM As IDAM = OnVacUtility.OpenDam()

        With DAM.QB

            .NewQuery(False, False)
            .AddTables("T_ANA_VACCINAZIONI_SEDUTE")
            .AddSelectFields("SED_VAC_CODICE as VAC_CODICE", "SED_N_RICHIAMO as N_RICHIAMO")
            For i As Integer = 0 To dtaCicli.Rows.Count - 1
                .AddWhereCondition("SED_CIC_CODICE", Comparatori.Uguale, dtaCicli.Rows(i)("PAC_CIC_CODICE"), DataTypes.Stringa, "OR")
            Next
            Dim qs1 As String = .GetSelect()

            .NewQuery(False, False)
            .AddTables("T_ANA_ASSOCIAZIONI_SEDUTE")
            .AddSelectFields("SAS_VAC_CODICE as VAC_CODICE", "SAS_N_RICHIAMO as N_RICHIAMO")
            For i As Integer = 0 To dtaCicli.Rows.Count - 1
                .AddWhereCondition("SAS_CIC_CODICE", Comparatori.Uguale, dtaCicli.Rows(i)("PAC_CIC_CODICE"), DataTypes.Stringa, "OR")
            Next

            Dim qs2 As String = .GetSelect()

            .NewQuery(False, False)
            .AddSelectFields("count(*)")
            .AddTables(String.Format("({0} union all {1})", qs1, qs2))
            .AddGroupByFields("VAC_CODICE", "N_RICHIAMO")
            .AddHavingFields("COUNT(*)>1")

        End With

        Dim check As Boolean

        Try
            Using idr As IDataReader = DAM.BuildDataReader()
                If idr.Read() Then
                    check = False
                Else
                    check = True
                End If
            End Using

        Catch ex As Exception
            Return True
        Finally
            OnVacUtility.CloseDam(DAM)
        End Try

        Return check

    End Function

    'controlla se alla convocazione selezionata per l'eliminazione è associato un appuntamento
    'o un sollecito: in questo caso deve chiedere conferma
    'RICHIAMATA LATO CLIENT
    Public Function ControllaConvocazioneEliminaCicli() As Boolean

        Using DAM As IDAM = OnVacUtility.OpenDam()

            Dim dtVacCicliCnv As DataTable
            Dim dtVacCnvProg As DataTable

            For count As Integer = 0 To dtaCicliEliminati.Rows.Count - 1

                With DAM.QB

                    'recupero delle vaccinazioni associate ai cicli selezionati
                    .NewQuery()
                    .AddSelectFields("SAS_VAC_CODICE, VPR_CNV_DATA")
                    .AddTables("V_ANA_ASS_VACC_SEDUTE, T_VAC_PROGRAMMATE")
                    .AddWhereCondition("SAS_CIC_CODICE", Comparatori.Uguale, dtaCicliEliminati.Rows(count)("PAC_CIC_CODICE"), DataTypes.Stringa)
                    .AddWhereCondition("VPR_PAZ_CODICE", Comparatori.Uguale, Paziente.Codice, DataTypes.Numero)
                    .AddWhereCondition("VPR_VAC_CODICE", Comparatori.Uguale, "SAS_VAC_CODICE", DataTypes.Join)
                    .IsDistinct = True

                    dtVacCicliCnv = New DataTable()
                    DAM.BuildDataTable(dtVacCicliCnv)

                    'recupero delle vaccinazioni programmate relative al paziente
                    .NewQuery()
                    .AddSelectFields("VPR_VAC_CODICE", "VPR_CNV_DATA")
                    .AddTables("T_CNV_CONVOCAZIONI", "T_VAC_PROGRAMMATE", "T_CNV_CICLI")
                    .AddWhereCondition("VPR_PAZ_CODICE", Comparatori.Uguale, OnVacUtility.Variabili.PazId, DataTypes.Numero)
                    .AddWhereCondition("CNV_PAZ_CODICE", Comparatori.Uguale, "VPR_PAZ_CODICE", DataTypes.Join)
                    .AddWhereCondition("CNV_DATA", Comparatori.Uguale, "VPR_CNV_DATA", DataTypes.Join)
                    .AddWhereCondition("CNV_PAZ_CODICE", Comparatori.Uguale, "CNC_CNV_PAZ_CODICE", DataTypes.OutJoinLeft)
                    .AddWhereCondition("CNV_DATA", Comparatori.Uguale, "CNC_CNV_DATA", DataTypes.OutJoinLeft)
                    .OpenParanthesis()
                    .AddWhereCondition("CNV_DATA_APPUNTAMENTO", Comparatori.[IsNot], "NULL", DataTypes.Replace)
                    .AddWhereCondition("CNC_N_SOLLECITO", Comparatori.Maggiore, 0, DataTypes.Numero, "OR")
                    .CloseParanthesis()

                    .IsDistinct = True
                    dtVacCnvProg = New DataTable()
                    DAM.BuildDataTable(dtVacCnvProg)

                    'elimino le vaccinazioni che dovrebbero essere eliminate con i cicli
                    Dim rowVac As DataRow
                    If dtVacCnvProg.Rows.Count > 0 Then
                        For countVac As Integer = 0 To dtVacCicliCnv.Rows.Count - 1
                            If dtVacCnvProg.Select("VPR_VAC_CODICE = '" & dtVacCicliCnv.Rows(countVac)("SAS_VAC_CODICE") & "' AND VPR_CNV_DATA = '" & dtVacCicliCnv.Rows(countVac)("VPR_CNV_DATA") & "'").Length > 0 Then
                                rowVac = dtVacCnvProg.Select("VPR_VAC_CODICE = '" & dtVacCicliCnv.Rows(countVac)("SAS_VAC_CODICE") & "' AND VPR_CNV_DATA = '" & dtVacCicliCnv.Rows(countVac)("VPR_CNV_DATA") & "'")(0)
                                rowVac.Delete()
                                dtVacCnvProg.AcceptChanges()
                                If dtVacCnvProg.Select("VPR_CNV_DATA = '" & dtVacCicliCnv.Rows(countVac)("VPR_CNV_DATA") & "'").Length = 0 Then
                                    Return True
                                End If
                            End If
                        Next
                    End If

                End With

            Next

            Return False

        End Using

    End Function

#End Region

#Region " Datagrid "

    Private Sub dgrMantoux_UpdateCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgrMantoux.UpdateCommand

        Dim dgrSource As DataGrid = DirectCast(source, DataGrid)

        Dim txtMantouxData As Onit.Web.UI.WebControls.Validators.OnitDatePick = DirectCast(dgrMantoux.Items(dgrMantoux.EditItemIndex).FindControl("txtMantouxData"), Onit.Web.UI.WebControls.Validators.OnitDatePick)

        'controllo che sia stata inserita la data
        If txtMantouxData.Text = "" Then
            RaiseEvent OnInsertRoutineJS(Me, New InsertRoutineJSEventArgs("alert('La data della mantoux non è stata valorizzata. Impossibile proseguire!!')"))
            Exit Sub
        End If

        'controllo sulle date: se precedenti alla data di nascita mando un messaggio e non salvo
        If txtMantouxData.Data < Paziente.DataNascita Then
            RaiseEvent OnInsertRoutineJS(Me, New InsertRoutineJSEventArgs("alert('La data della mantoux è precedente alla data di nascita!');"))
            Exit Sub
        End If

        'Controlla che un ciclo non esista già
        For i As Int16 = 0 To dtaMantoux.Rows.Count - 1

            If (dtaMantoux.Rows(i)("MAN_DATA") & "") = txtMantouxData.Text And i <> dgrMantoux.EditItemIndex Then
                WriteAlert("Impossibile inserire due record con la stessa data!")
                Exit Sub
            End If

        Next

        'controllo sulle date: se posteriori alla data odierna mando un messaggio ma permetto di salvare
        If txtMantouxData.Data > Date.Today Then
            RaiseEvent OnInsertRoutineJS(Me, New InsertRoutineJSEventArgs("alert('La data della mantoux è successiva alla data odierna!');"))
        End If

        Try
            'Lo salva e aggiorna il datagrid
            Dim sel As Int16 = e.Item.ItemIndex + dgrSource.PageSize * dgrSource.CurrentPageIndex

            Dim txtMantouxNum As TextBox = DirectCast(dgrMantoux.Items(dgrMantoux.EditItemIndex).FindControl("txtMantouxNum"), TextBox)
            Dim txtMantouxMedico As Onit.Controls.OnitModalList = DirectCast(dgrMantoux.Items(dgrMantoux.EditItemIndex).FindControl("txtMantouxMedico"), Onit.Controls.OnitModalList)
            Dim chkPositivaSiNo As CheckBox = DirectCast(dgrMantoux.Items(dgrMantoux.EditItemIndex).FindControl("chkPositivaSiNo"), CheckBox)
            Dim txtMantouxDataInvio As Onit.Web.UI.WebControls.Validators.OnitDatePick = DirectCast(dgrMantoux.Items(dgrMantoux.EditItemIndex).FindControl("txtMantouxDataInvio"), Onit.Web.UI.WebControls.Validators.OnitDatePick)

            dtaMantoux.Rows(sel).Item("MAN_DATA") = txtMantouxData.Text
            dtaMantoux.Rows(sel).Item("MAN_DESCRIZIONE") = DirectCast(dgrMantoux.Items(dgrMantoux.EditItemIndex).FindControl("txtMantouxDescrizione"), TextBox).Text
            dtaMantoux.Rows(sel).Item("MAN_MM") = IIf(txtMantouxNum.Text = "", DBNull.Value, txtMantouxNum.Text)
            dtaMantoux.Rows(sel).Item("MAN_OPE_CODICE") = txtMantouxMedico.Codice
            dtaMantoux.Rows(sel).Item("OPE_NOME") = txtMantouxMedico.Descrizione
            dtaMantoux.Rows(sel).Item("MAN_SINO") = IIf(DirectCast(dgrMantoux.Items(dgrMantoux.EditItemIndex).FindControl("chkSiNo"), CheckBox).Checked, "S", "N")
            dtaMantoux.Rows(sel).Item("MAN_POSITIVA") = IIf(chkPositivaSiNo.Checked, "S", "N")
            dtaMantoux.Rows(sel).Item("MAN_DATA_INVIO") = IIf(txtMantouxDataInvio.Text = "", DBNull.Value, txtMantouxDataInvio.Text)

            'se la manotux è positiva gli mando un messaggio per ricordare di escludere la mantoux [modifica 15/12/2006]
            If chkPositivaSiNo.Checked Then
                RaiseEvent OnInsertRoutineJS(Me, New InsertRoutineJSEventArgs("alert('La mantoux è positiva. Ricordati di inserire l\'esclusione!')"))
            End If

            dgrMantoux.DataSource = dtaMantoux
            dgrMantoux.EditItemIndex = -1
            dgrMantoux.DataBind()

            ControlState = ControlStateEnum.EDIT

        Catch ex As Exception
            RaiseEvent OnInsertRoutineJS(Me, New InsertRoutineJSEventArgs("Si è verificato un problema durante il tentativo di salvataggio dei dati in memoria nella funzione dgrMantoux_UpdateCommand')"))
        End Try

    End Sub

    Private Sub dgrMalattie_UpdateCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgrMalattie.UpdateCommand

        Dim txtMalattia As Onit.Controls.OnitModalList = DirectCast(dgrMalattie.Items(dgrMalattie.EditItemIndex).FindControl("txtMalattia"), Onit.Controls.OnitModalList)
        Dim txtNumeroMalattia As TextBox = DirectCast(dgrMalattie.Items(dgrMalattie.EditItemIndex).FindControl("txtNMalattia"), TextBox)

        'Controlla che sia stato inserito qualcosa
        If txtMalattia.Codice = "" Then

            WriteAlert("Impostare un valore per il campo!")
            Exit Sub

        Else

            'Controlla che sia stato inserito il numero della malattia (modifica 10/06/2004)
            Dim regEx As New RegularExpressions.Regex("^\d+$")

            If txtNumeroMalattia.Text.Trim() = "" Then
                WriteAlert("Impostare un valore per il campo 'Numero'!")
                Exit Sub
            ElseIf (Not regEx.IsMatch(txtNumeroMalattia.Text)) OrElse txtNumeroMalattia.Text.Trim() = "0" Then
                WriteAlert("Il campo 'Numero' deve essere intero e maggiore di zero!")
                Exit Sub
            End If

        End If

        ' Controlli relativi alle malattie specificate
        For i As Int16 = 0 To dtaMalattie.Rows.Count - 1

            ' Codice malattia
            If (dtaMalattie.Rows(i)("PMA_MAL_CODICE") & "") = txtMalattia.Codice And i <> dgrMalattie.EditItemIndex Then

                WriteAlert("Impossibile inserire due malattie con lo stesso codice!")
                Exit Sub

            End If

            ' Gravità
            If (dtaMalattie.Rows(i)("PMA_N_MALATTIA") & "") = txtNumeroMalattia.Text And i <> dgrMalattie.EditItemIndex Then

                WriteAlert("Impossibile inserire due malattie con la stessa gravità!")
                Exit Sub

            End If

        Next

        ' Controllo se, nella riga corrente, si sta sovrascrivendo la malattia "Nessuna" con una malattia diversa
        If (dtaMalattie.Rows(dgrMalattie.EditItemIndex)("PMA_MAL_CODICE") & "") = Settings.CODNOMAL AndAlso txtMalattia.Codice <> Settings.CODNOMAL Then

            WriteAlert("Impossibile sostituire questa malattia!")
            Exit Sub

        End If

        '---CMR 10/07/2007--- controllo FollowUp = "S" se c'è la gestione dei bilanci
        Dim chkFollowUp As CheckBox = DirectCast(dgrMalattie.Items(dgrMalattie.EditItemIndex).FindControl("chkFollowUp"), CheckBox)
        Dim chkNuovaDiagnosi As CheckBox = DirectCast(dgrMalattie.Items(dgrMalattie.EditItemIndex).FindControl("chkNuovaDiagnosi"), CheckBox)
        Dim txtDataDiagnosi As Onit.Web.UI.WebControls.Validators.OnitDatePick = DirectCast(dgrMalattie.Items(dgrMalattie.EditItemIndex).FindControl("txtDataDiagnosi"), Onit.Web.UI.WebControls.Validators.OnitDatePick)
        Dim txtDataUltimaVisita As Onit.Web.UI.WebControls.Validators.OnitDatePick = DirectCast(dgrMalattie.Items(dgrMalattie.EditItemIndex).FindControl("txtDataUltimaVisita"), Onit.Web.UI.WebControls.Validators.OnitDatePick)
        Dim cmbBilancioPartenza As DropDownList = DirectCast(dgrMalattie.Items(dgrMalattie.EditItemIndex).FindControl("cmbBilancioPartenza"), DropDownList)

        If Settings.GESBIL AndAlso chkFollowUp.Checked Then

            ' Se flag di visita della malattia è true, controllo se si tratta di una nuova diagnosi o di una vecchia diagnosi
            If ControlloFlagVisita(txtMalattia.Codice) Then

                If chkNuovaDiagnosi.Checked Then

                    ' Nuova diagnosi => deve essere obbligatoriamente inserita la data di diagnosi e il bilancio di partenza deve essere il minore

                    If txtDataDiagnosi.Text = "" Then
                        WriteAlert("Per seguire l'iter dei bilanci, questa malattia prevede una data di diagnosi non nulla!")
                        Exit Sub
                    End If
                    cmbBilancioPartenza.Items(0).Selected = True

                Else

                    ' Vecchia diagnosi => è obbligatoria la data di ultima visita e il numero di bilancio di partenza

                    If txtDataUltimaVisita.Text = "" Then
                        WriteAlert("Per seguire l'iter dei bilanci, questa malattia prevede una data di ultima visita!")
                        Exit Sub
                    End If
                    If cmbBilancioPartenza.SelectedValue = "0" Then
                        WriteAlert("Per seguire l'iter dei bilanci, il numero del bilancio di partenza deve essere diverso da zero!")
                        Exit Sub
                    End If

                End If

            End If

        End If

        'controllo sulle date: se posteriori alla data odierna mando un messaggio ma permetto di salvare
        If txtDataDiagnosi.Text <> "" AndAlso txtDataDiagnosi.Data > Date.Today Then
            RaiseEvent OnInsertRoutineJS(Me, New InsertRoutineJSEventArgs("alert(""La data di diagnosi è successiva alla data odierna!"");"))
        End If

        If txtDataUltimaVisita.Text <> "" AndAlso txtDataUltimaVisita.Data > Date.Today Then
            RaiseEvent OnInsertRoutineJS(Me, New InsertRoutineJSEventArgs("alert(""La data di ultima visita è successiva alla data odierna!"");"))
        End If

        'controllo sulle date: se precedenti alla data di nascita mando un messaggio e non salvo
        If txtDataDiagnosi.Text <> "" AndAlso txtDataDiagnosi.Data < Paziente.DataNascita Then
            RaiseEvent OnInsertRoutineJS(Me, New InsertRoutineJSEventArgs("alert(""La data di diagnosi è precedente alla data di nascita!"");"))
            Exit Sub
        End If

        If txtDataUltimaVisita.Text <> "" AndAlso txtDataUltimaVisita.Data < Paziente.DataNascita Then
            RaiseEvent OnInsertRoutineJS(Me, New InsertRoutineJSEventArgs("alert(""La data di ultima visita è precedente alla data di nascita!"");"))
            Exit Sub
        End If
        '--------------------

        dtaMalattie.DefaultView.Sort = ""
        dtaMalattie.DefaultView.Sort = "PMA_N_MALATTIA"
        'Lo salva e aggiorna il datagrid

        Dim sel As Int16 = e.Item.ItemIndex + source.PageSize * source.CurrentPageIndex

        dtaMalattie.DefaultView.Item(sel).Item("PMA_MAL_CODICE") = txtMalattia.Codice
        dtaMalattie.DefaultView.Item(sel).Item("MAL_DESCRIZIONE") = txtMalattia.Descrizione
        dtaMalattie.DefaultView.Item(sel).Item("PMA_N_MALATTIA") = txtNumeroMalattia.Text
        dtaMalattie.DefaultView.Item(sel).Item("PMA_FOLLOW_UP") = IIf(chkFollowUp.Checked, "S", "N")
        dtaMalattie.DefaultView.Item(sel).Item("PMA_NUOVA_DIAGNOSI") = IIf(chkNuovaDiagnosi.Checked, "S", "N")
        dtaMalattie.DefaultView.Item(sel).Item("PMA_DATA_DIAGNOSI") = IIf(txtDataDiagnosi.Text = "", DBNull.Value, txtDataDiagnosi.Data)
        dtaMalattie.DefaultView.Item(sel).Item("PMA_DATA_ULTIMA_VISITA") = IIf(txtDataUltimaVisita.Text = "", DBNull.Value, txtDataUltimaVisita.Data)
        dtaMalattie.DefaultView.Item(sel).Item("PMA_N_BILANCIO_PARTENZA") = IIf(cmbBilancioPartenza.SelectedValue = "", "0", cmbBilancioPartenza.SelectedValue)

        dgrMalattie.DataSource = dtaMalattie.DefaultView
        dgrMalattie.EditItemIndex = -1
        dgrMalattie.DataBind()

        ControlState = ControlStateEnum.EDIT

        ImpostaHandlerFrecciaSuGiu()

    End Sub

    Private Sub Datagrid_DeleteCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgrCicli.DeleteCommand, dgrMantoux.DeleteCommand, dgrMalattie.DeleteCommand

        Dim dgrSource As DataGrid = DirectCast(source, DataGrid)

        Dim dtSource As DataTable = Nothing

        'Esegue una scelta in base al datagrid utilizzato
        Select Case dgrSource.ID

            Case "dgrCicli"

                dtSource = dtaCicli

                ' Script di eliminazione programmazione cicli al salvataggio
                dtaCicliEliminati.ImportRow(dtSource.Rows(e.Item.ItemIndex + dgrSource.PageSize * dgrSource.CurrentPageIndex))

                RefreshMessaggiCicliEliminati()

                dtSource.Rows(e.Item.ItemIndex + dgrSource.PageSize * dgrSource.CurrentPageIndex).Delete()

            Case "dgrMalattie"

                dtSource = dtaMalattie

                ' Controllo l'eliminazione della malattia default
                Dim mal_codice As String = dtSource.DefaultView.Item(e.Item.ItemIndex + dgrSource.PageSize * dgrSource.CurrentPageIndex)("PMA_MAL_CODICE").ToString()
                If mal_codice = Settings.CODNOMAL Then
                    WriteAlert("Impossibile eliminare questa malattia!")
                    Exit Sub
                End If

                RefreshMessaggiCicliEliminati()

                dtSource.DefaultView.Item(e.Item.ItemIndex + dgrSource.PageSize * dgrSource.CurrentPageIndex).Delete()
                dtSource.DefaultView.Sort = ""
                dtSource.DefaultView.Sort = "PMA_N_MALATTIA"

            Case "dgrMantoux"

                dtSource = dtaMantoux

                RefreshMessaggiCicliEliminati()

                dtSource.Rows(e.Item.ItemIndex + dgrSource.PageSize * dgrSource.CurrentPageIndex).Delete()

        End Select

        dtSource.AcceptChanges()

        dgrSource.DataSource = dtSource.DefaultView
        dgrSource.EditItemIndex = -1
        dgrSource.CurrentPageIndex = 0
        dgrSource.DataBind()

        ControlState = ControlStateEnum.EDIT

    End Sub

    Private Sub Datagrid_PageIndexChanged(source As Object, e As DataGridPageChangedEventArgs) Handles dgrCicli.PageIndexChanged, dgrMantoux.PageIndexChanged, dgrMalattie.PageIndexChanged

        Dim dgrSource = DirectCast(source, DataGrid)

        Dim dtSource As DataTable = GetDataTableToBind(dgrSource.ID)

        'Cambia pagina nel datagrid dei cicli o delle mantoux
        dgrSource.CurrentPageIndex = e.NewPageIndex
        dgrSource.DataSource = dtSource.DefaultView
        dgrSource.DataBind()

    End Sub

    Private Sub Datagrid_CancelCommand(source As Object, e As DataGridCommandEventArgs) Handles dgrMantoux.CancelCommand, dgrMalattie.CancelCommand

        Dim dgrSource = DirectCast(source, DataGrid)

        Dim dtSource As DataTable = GetDataTableToBind(dgrSource.ID)

        If dtSource.Rows(e.Item.ItemIndex)(0) & "" = "" Then
            dtSource.Rows(e.Item.ItemIndex).Delete()
            dtSource.AcceptChanges()
        End If

        dgrSource.DataSource = dtSource
        dgrSource.EditItemIndex = -1
        dgrSource.DataBind()

        ControlState = ControlStateEnum.EDIT

    End Sub

    Private Sub Datagrid_EditCommand(source As Object, e As DataGridCommandEventArgs) Handles dgrMantoux.EditCommand, dgrMalattie.EditCommand

        Dim dgrSource As DataGrid = DirectCast(source, DataGrid)

        Dim dtSource As DataTable = GetDataTableToBind(dgrSource.ID)

        dgrSource.DataSource = dtSource.DefaultView
        dgrSource.EditItemIndex = e.Item.ItemIndex
        dgrSource.DataBind()

        ControlState = ControlStateEnum.LOCK

    End Sub

    Public Sub Datagrid_ItemCommand(source As Object, e As DataGridCommandEventArgs) Handles dgrCicli.ItemCommand, dgrMalattie.ItemCommand, dgrMantoux.ItemCommand

        'Salva le modifiche a un ciclo
        Select Case e.CommandName

            Case "Conferma"

                'Controlla che un ciclo non esista già
                For i As Int16 = 0 To dtaCicli.Rows.Count - 1
                    If dtaCicli.Rows(i)("PAC_CIC_CODICE") & "" = DirectCast(dgrCicli.Items(dgrCicli.EditItemIndex).FindControl("txtCicliEdit"), Onit.Controls.OnitModalList).Codice And
                       i <> dgrCicli.EditItemIndex Then

                        WriteAlert("Impossibile inserire un ciclo più di una volta!")
                        Exit Sub

                    End If
                Next i

                'Lo salva e aggiorna il datagrid
                Dim txtCicliEdit As Onit.Controls.OnitModalList = DirectCast(dgrCicli.Items(dgrCicli.EditItemIndex).FindControl("txtCicliEdit"), Onit.Controls.OnitModalList)

                dtaCicli.Rows(dtaCicli.Rows.Count - 1)("PAC_CIC_CODICE") = txtCicliEdit.Codice
                dtaCicli.Rows(dtaCicli.Rows.Count - 1)("CIC_DESCRIZIONE") = txtCicliEdit.Descrizione

                dgrCicli.DataSource = dtaCicli
                dgrCicli.EditItemIndex = -1
                dgrCicli.DataBind()

            Case "Nuovo"

                ' Controllo valorizzazione data di nascita
                If Paziente.DataNascita = Date.MinValue Then
                    WriteAlert("E' necessario specificare la data di nascita prima di inserire i cicli.")
                    Exit Sub
                End If

                PulsantiAggiungiClick(e.CommandArgument)

                ControlState = ControlStateEnum.LOCK

            Case "Edit"

                ' Lancio un LOCK perche' non voglio salvare finche' non viene confermato lo stato del controllo
                ControlState = ControlStateEnum.LOCK

        End Select

    End Sub

    Private Function GetDataTableToBind(dataGridId As String) As DataTable

        Dim dtToBind As DataTable = Nothing

        Select Case dataGridId

            Case "dgrCicli"
                dtToBind = dtaCicli

            Case "dgrMalattie"
                dtToBind = dtaMalattie
                dtToBind.DefaultView.Sort = ""
                dtToBind.DefaultView.Sort = "PMA_N_MALATTIA"

            Case "dgrMantoux"
                dtToBind = dtaMantoux

        End Select

        Return dtToBind

    End Function

#End Region

#Region " User Controls "

    Private Sub OnVacSceltaCicli_OnAnnulla(sender As Object) Handles OnVacSceltaCicli.OnAnnulla

        fmSceltaCicli.VisibileMD = False

        ControlState = ControlStateEnum.EDIT

    End Sub

    'scatta alla chiusura della modale aggiungendo i cicli
    Private Sub OnVacSceltaCicli_OnConferma(dtCicliScelti As System.Data.DataTable) Handles OnVacSceltaCicli.OnConferma

        Dim rigaNuova, rigaScelta, rigaNuovaB As DataRow

        fmSceltaCicli.VisibileMD = False

        dtaCicliAggiunti = dtaCicli.Clone()
        dtaCicliAggiunti.Clear()

        For Each rigaScelta In dtCicliScelti.Rows

            rigaNuova = dtaCicli.NewRow()
            rigaNuovaB = dtaCicliAggiunti.NewRow()

            rigaNuova("PAC_CIC_CODICE") = rigaScelta("CIC_CODICE")
            rigaNuova("CIC_DESCRIZIONE") = rigaScelta("CIC_DESCRIZIONE")

            rigaNuovaB("PAC_CIC_CODICE") = rigaScelta("CIC_CODICE")
            rigaNuovaB("CIC_DESCRIZIONE") = rigaScelta("CIC_DESCRIZIONE")

            dtaCicli.Rows.Add(rigaNuova)
            dtaCicliAggiunti.Rows.Add(rigaNuovaB)

        Next

        dgrCicli.DataSource = dtaCicli
        dgrCicli.DataBind()

        If dtCicliScelti.Rows.Count > 0 Then
            ControlState = ControlStateEnum.EDIT
        End If

    End Sub

#End Region

#Region " Gestione Malattia "

    Private Sub dgrMalattie_ItemCreated(sender As Object, e As DataGridItemEventArgs) Handles dgrMalattie.ItemCreated

        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then

            DirectCast(e.Item.FindControl("imgFrecciaSu"), LinkButton).Text = "<img style='border:0px' src='" + ResolveUrl("~/images/sopra.gif") + "' />"
            DirectCast(e.Item.FindControl("imgFrecciaGiu"), LinkButton).Text = "<img style='border:0px' src='" + ResolveUrl("~/images/sotto.gif") + "' />"

        End If

    End Sub

    Private Sub dgrMalattie_PreRender(sender As Object, e As EventArgs) Handles dgrMalattie.PreRender

        For Each item As DataGridItem In dgrMalattie.Items

            If item.ItemIndex <> dgrMalattie.EditItemIndex Then

                ' Se la malattia è di tipologia non modificabile, rimuove i controlli di riga per eliminare e modificare
                Dim codiceMalattia As String = String.Empty

                Dim control As Control = item.FindControl("txtMalattieDis")

                If Not control Is Nothing Then
                    codiceMalattia = DirectCast(control, Onit.Controls.OnitModalList).Codice
                End If

                If Not String.IsNullOrWhiteSpace(codiceMalattia) Then

                    Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                        Using bizMalattie As New BizMalattie(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                            If Not bizMalattie.IsTipologiaMalattiaModificabile(codiceMalattia) Then

                                Dim ctrl As Control = item.Cells(ColumnIndexDgrMalattie.ButtonColumn).FindControl("imgEliminaMalattia")
                                If Not ctrl Is Nothing Then
                                    ctrl.Visible = False
                                End If

                                ctrl = item.Cells(ColumnIndexDgrMalattie.EditColumn).FindControl("imgModificaMalattia")
                                If Not ctrl Is Nothing Then
                                    ctrl.Visible = False
                                End If

                            End If

                        End Using
                    End Using

                End If

            End If

        Next

        If dgrMalattie.EditItemIndex > -1 Then

            CaricaBilancioPartenza(dgrMalattie.EditItemIndex)

        End If

    End Sub

    Sub ImpostaHandlerFrecciaSuGiu()

        For i As Integer = 0 To dgrMalattie.Items.Count - 1

            If dgrMalattie.Items(i).ItemType = ListItemType.AlternatingItem Or dgrMalattie.Items(i).ItemType = ListItemType.Item Then

                Dim imgSu As LinkButton = DirectCast(dgrMalattie.Items(i).FindControl("imgFrecciaSu"), LinkButton)
                imgSu.CommandArgument = i

                Dim imgGiu As LinkButton = DirectCast(dgrMalattie.Items(i).FindControl("imgFrecciaGiu"), LinkButton)
                imgGiu.CommandArgument = i

                AddHandler imgSu.Click, AddressOf SpostaSu
                AddHandler imgGiu.Click, AddressOf SpostaGiu

            End If

        Next

    End Sub

    Sub SpostaSu(obj As Object, e As EventArgs)

        Dim startIndex As Integer = Integer.Parse(DirectCast(obj, LinkButton).CommandArgument)

        If startIndex > 0 Then

            SpostaRigaMalattia(startIndex, startIndex - 1)

            ControlState = ControlStateEnum.EDIT

        End If

    End Sub

    Sub SpostaGiu(obj As Object, e As EventArgs)

        Dim startIndex As Integer = Integer.Parse(DirectCast(obj, LinkButton).CommandArgument)

        If startIndex < dgrMalattie.Items.Count - 1 Then

            SpostaRigaMalattia(startIndex, startIndex + 1)

            ControlState = ControlStateEnum.EDIT

        End If

    End Sub

    Private Sub SpostaRigaMalattia(startIndex As Integer, destinationIndex As Integer)

        dtaMalattie.AcceptChanges()
        dtaMalattie.DefaultView.Sort = "PMA_N_MALATTIA"

        Dim numeroMalattiaRigaDaSpostare As String = dtaMalattie.DefaultView(startIndex)("PMA_N_MALATTIA").ToString()
        Dim numeroMalattiaRigaDestinazione As String = dtaMalattie.DefaultView(destinationIndex)("PMA_N_MALATTIA").ToString()

        Dim codiceMalattiaRigaDaSpostare As String = dtaMalattie.DefaultView(startIndex)("PMA_MAL_CODICE").ToString()
        Dim codiceMalattiaRigaDestinazione As String = dtaMalattie.DefaultView(destinationIndex)("PMA_MAL_CODICE").ToString()

        For i As Integer = 0 To dtaMalattie.Rows.Count - 1
            If dtaMalattie.Rows(i)("PMA_MAL_CODICE") = codiceMalattiaRigaDaSpostare Then
                dtaMalattie.Rows(i)("PMA_N_MALATTIA") = numeroMalattiaRigaDestinazione
                Exit For
            End If
        Next

        For i As Integer = 0 To dtaMalattie.Rows.Count - 1
            If dtaMalattie.Rows(i)("PMA_MAL_CODICE") = codiceMalattiaRigaDestinazione Then
                dtaMalattie.Rows(i)("PMA_N_MALATTIA") = numeroMalattiaRigaDaSpostare
                Exit For
            End If
        Next

        dtaMalattie.DefaultView.Sort = ""
        dtaMalattie.DefaultView.Sort = "PMA_N_MALATTIA"

        dgrMalattie.DataSource = dtaMalattie.DefaultView
        dgrMalattie.DataBind()

    End Sub

    Public Function RecuperaFiltroModaleMalattia() As String

        Dim filtro As New StringBuilder(" MAL_OBSOLETO = 'N' ")

        ' Devono essere visualizzate le malattie dei tipi elencati nel parametro GESTPAZ_TIPOLOGIA_MALATTIA
        If Settings.GESTPAZ_TIPOLOGIA_MALATTIA.Count > 0 Then

            filtro.AppendFormat(" AND MML_MTI_CODICE IN ({0}) ", GetFiltroInTipiMalattie(Settings.GESTPAZ_TIPOLOGIA_MALATTIA))

        End If

        ' Non devono essere visualizzate le malattie dei tipi elencati nel parametro GESTPAZ_MALATTIE_NON_MODIFICABILI
        If Not Settings.GESTPAZ_MALATTIE_NON_MODIFICABILI.IsNullOrEmpty() Then

            filtro.AppendFormat(" AND NOT EXISTS (SELECT 1 FROM T_ANA_LINK_MALATTIE_TIPOLOGIA WHERE MML_MAL_CODICE = MAL_CODICE AND MML_MTI_CODICE IN ({0})) ", GetFiltroInTipiMalattie(Settings.GESTPAZ_MALATTIE_NON_MODIFICABILI))

        End If

        ' Non devono essere visualizzate le malattie già presenti
        If dtaMalattie.Rows.Count > 0 Then

            Dim stbMal As New StringBuilder()

            For count As Integer = 0 To dtaMalattie.Rows.Count - 1
                ' Se il codice della malattia è '', non lo deve mettere tra i filtri altrimenti fallisce tutta la query
                If dtaMalattie.Rows(count)("pma_mal_codice").ToString() <> String.Empty Then
                    stbMal.AppendFormat("'{0}',", dtaMalattie.Rows(count)("pma_mal_codice"))
                End If
            Next

            If stbMal.Length > 0 Then
                stbMal.RemoveLast(1)
                filtro.AppendFormat(" AND MAL_CODICE NOT IN ({0}) ", stbMal.ToString())
            End If

        End If

        ' Ordinamento modale
        If String.IsNullOrEmpty(Settings.GESTPAZ_CAMPO_ORDINAMENTO_MALATTIA) Then
            filtro.Append(" ORDER BY Descrizione ")
        Else
            filtro.Append(" ORDER BY ")
            filtro.Append(Settings.GESTPAZ_CAMPO_ORDINAMENTO_MALATTIA)
        End If

        Return filtro.ToString().Replace(",''", "")

    End Function

    Private Function GetFiltroInTipiMalattie(tipologieMalattia As List(Of String)) As String

        Dim filtro As New StringBuilder()

        For i As Integer = 0 To tipologieMalattia.Count - 1
            filtro.AppendFormat("'{0}',", tipologieMalattia(i))
        Next

        If filtro.Length > 1 Then filtro.RemoveLast(1)

        Return filtro.ToString()

    End Function

    Private Sub txtMalattia_change(sender As Object, e As Onit.Controls.OnitModalList.ModalListaEventArgument)

        If Not Settings.GESBIL Then Return

        Dim codiceMalattia As String = DirectCast(dgrMalattie.Items(dgrMalattie.EditItemIndex).FindControl("txtMalattia"), Onit.Controls.OnitModalList).Codice

        If codiceMalattia <> "" Then

            DirectCast(dgrMalattie.Items(dgrMalattie.EditItemIndex).FindControl("chkNuovaDiagnosi"), CheckBox).Checked = True
            DirectCast(dgrMalattie.Items(dgrMalattie.EditItemIndex).FindControl("chkFollowUp"), CheckBox).Checked = True
            DirectCast(dgrMalattie.Items(dgrMalattie.EditItemIndex).FindControl("cmbBilancioPartenza"), DropDownList).Enabled = False

            If ControlloFlagVisita(codiceMalattia) Then
                DirectCast(dgrMalattie.Items(dgrMalattie.EditItemIndex).FindControl("txtDataDiagnosi"), Onit.Web.UI.WebControls.Validators.OnitDatePick).CssClass = "TextBox_Data_Obbligatorio"
                DirectCast(dgrMalattie.Items(dgrMalattie.EditItemIndex).FindControl("txtDataUltimaVisita"), Onit.Web.UI.WebControls.Validators.OnitDatePick).CssClass = "TextBox_Data"
            End If

        End If

    End Sub

    Private Function ControlloFlagVisita(codiceMalattia As String) As Boolean

        Dim flagVisita As Boolean = False

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizMalattie As New BizMalattie(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                flagVisita = bizMalattie.GetFlagVisita(codiceMalattia)

            End Using
        End Using

        Return flagVisita

    End Function

    Private Sub btnChkNuovaDiagnosi_Click(sender As Object, e As EventArgs) Handles btnChkNuovaDiagnosi.Click

        If Not Settings.GESBIL Then Return

        If dgrMalattie.EditItemIndex > -1 Then

            Dim codiceMalattia As String = DirectCast(dgrMalattie.Items(dgrMalattie.EditItemIndex).FindControl("txtMalattia"), Onit.Controls.OnitModalList).Codice

            If DirectCast(dgrMalattie.Items(dgrMalattie.EditItemIndex).FindControl("chkFollowUp"), CheckBox).Checked = True AndAlso ControlloFlagVisita(codiceMalattia) Then

                If DirectCast(dgrMalattie.Items(dgrMalattie.EditItemIndex).FindControl("chkNuovaDiagnosi"), CheckBox).Checked = True Then
                    DirectCast(dgrMalattie.Items(dgrMalattie.EditItemIndex).FindControl("txtDataDiagnosi"), Onit.Web.UI.WebControls.Validators.OnitDatePick).CssClass = "TextBox_Data_Obbligatorio"
                    DirectCast(dgrMalattie.Items(dgrMalattie.EditItemIndex).FindControl("txtDataUltimaVisita"), Onit.Web.UI.WebControls.Validators.OnitDatePick).CssClass = "TextBox_Data"
                    DirectCast(dgrMalattie.Items(dgrMalattie.EditItemIndex).FindControl("cmbBilancioPartenza"), DropDownList).Enabled = False
                Else
                    DirectCast(dgrMalattie.Items(dgrMalattie.EditItemIndex).FindControl("txtDataDiagnosi"), Onit.Web.UI.WebControls.Validators.OnitDatePick).CssClass = "TextBox_Data"
                    DirectCast(dgrMalattie.Items(dgrMalattie.EditItemIndex).FindControl("txtDataUltimaVisita"), Onit.Web.UI.WebControls.Validators.OnitDatePick).CssClass = "TextBox_Data_Obbligatorio"
                    DirectCast(dgrMalattie.Items(dgrMalattie.EditItemIndex).FindControl("cmbBilancioPartenza"), DropDownList).Enabled = True
                End If

            End If

        End If

    End Sub

#End Region

#Region " Private "

    Private Sub PulsantiAggiungiClick(sender As String)

        'Esegue una scelta in base al pulsante premuto
        Dim dgrSource As DataGrid = Nothing
        Dim dtSource As DataTable = Nothing

        Dim exclCodes As New ArrayList()

        Select Case sender

            Case "dgrCicli"

                dtSource = dtaCicli
                dgrSource = Nothing

                For Each currRow As DataRow In dtSource.Rows
                    If Not IsDBNull(currRow("PAC_CIC_CODICE")) Then
                        exclCodes.Add(currRow("PAC_CIC_CODICE"))
                    End If
                Next

                If dgrCicli.EditItemIndex = -1 Then

                    OnVacSceltaCicli.DataNascita = Paziente.DataNascita
                    OnVacSceltaCicli.Sesso = Paziente.Sesso
                    OnVacSceltaCicli.CaricaCicli(exclCodes)

                    fmSceltaCicli.VisibileMD = True

                End If

            Case "dgrMantoux"

                dtSource = dtaMantoux
                dgrSource = dgrMantoux

            Case "dgrMalattie"

                dtSource = dtaMalattie
                dgrSource = dgrMalattie

        End Select

        If Not dgrSource Is Nothing Then

            If dgrSource.EditItemIndex = -1 Then

                dtSource.Rows.Add(dtSource.NewRow())

                Select Case sender
                    Case "dgrMalattie"
                        Dim obj As Object = dtSource.Compute("max(PMA_N_MALATTIA)", Nothing)
                        If obj Is Nothing OrElse obj Is System.DBNull.Value OrElse obj.ToString() = "" Then
                            obj = 0
                        End If
                        dtSource.Rows(dtSource.Rows.Count - 1)("PMA_N_MALATTIA") = Integer.Parse(obj) + 1
                        dtSource.DefaultView.Sort = ""
                        dtSource.DefaultView.Sort = "PMA_N_MALATTIA"
                End Select

                dgrSource.DataSource = dtSource.DefaultView
                dgrSource.EditItemIndex = (dtSource.DefaultView.Count - 1) Mod dgrSource.PageSize
                dgrSource.CurrentPageIndex = Math.Ceiling(dtSource.DefaultView.Count / dgrSource.PageSize) - 1
                dgrSource.DataBind()

            Else
                WriteAlert("E' necessario confermare la riga in edit prima di aggiungerne un'altra.")
            End If

        End If

    End Sub

    Private Sub EliminaProgrammazioneCicli(idPaziente As Integer, cicliEliminati As DataTable)

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, OnVacUtility.GetReadCommittedTransactionOptions)

            Using DAM As IDAM = OnVacUtility.OpenDam()

                Dim dtVacCicliCnv As DataTable
                Dim strQueryExists As String = ""

                For count As Integer = 0 To cicliEliminati.Rows.Count - 1

                    With DAM.QB

                        'recupero delle vaccinazioni associate ai cicli selezionati
                        .NewQuery()
                        .AddSelectFields("SAS_VAC_CODICE", "VPR_CNV_DATA", "SAS_CIC_CODICE")
                        .AddTables("V_ANA_ASS_VACC_SEDUTE", "T_VAC_PROGRAMMATE")
                        .AddWhereCondition("SAS_CIC_CODICE", Comparatori.Uguale, cicliEliminati.Rows(count)("PAC_CIC_CODICE"), DataTypes.Stringa)
                        .AddWhereCondition("VPR_PAZ_CODICE", Comparatori.Uguale, idPaziente, DataTypes.Numero)
                        .AddWhereCondition("VPR_VAC_CODICE", Comparatori.Uguale, "SAS_VAC_CODICE", DataTypes.Join)

                        .IsDistinct = True

                        dtVacCicliCnv = New DataTable()

                        DAM.BuildDataTable(dtVacCicliCnv)

                        'controllo se esistono effettivamente delle vaccinazioni da eliminare
                        For countVac As Integer = 0 To dtVacCicliCnv.Rows.Count - 1

                            .NewQuery()
                            .AddTables("T_CNV_CICLI")
                            .AddWhereCondition("CNC_CNV_PAZ_CODICE", Comparatori.Uguale, idPaziente, DataTypes.Numero)
                            .AddWhereCondition("CNC_CNV_DATA", Comparatori.Uguale, dtVacCicliCnv.Rows(countVac)("VPR_CNV_DATA"), DataTypes.Data)
                            .AddWhereCondition("CNC_CIC_CODICE", Comparatori.Uguale, dtVacCicliCnv.Rows(countVac)("SAS_CIC_CODICE"), DataTypes.Stringa)

                            DAM.ExecNonQuery(ExecQueryType.Delete)

                            .NewQuery()
                            .AddTables("T_VAC_PROGRAMMATE")
                            .AddWhereCondition("VPR_PAZ_CODICE", Comparatori.Uguale, idPaziente, DataTypes.Numero)
                            .AddWhereCondition("VPR_VAC_CODICE", Comparatori.Uguale, dtVacCicliCnv.Rows(countVac)("SAS_VAC_CODICE"), DataTypes.Stringa)
                            .AddWhereCondition("VPR_CNV_DATA", Comparatori.Uguale, dtVacCicliCnv.Rows(countVac)("VPR_CNV_DATA"), DataTypes.Data)

                            DAM.ExecNonQuery(ExecQueryType.Delete)

                        Next

                    End With
                Next

            End Using

            transactionScope.Complete()

        End Using

    End Sub

    Private Sub WriteAlert(message As String)

        RaiseEvent OnAlert(Me, New Onit.OnAssistnet.OnVac.Common.UserControlEventArgs(message))

    End Sub

#End Region

#Region " Protected "

    Protected Function ClientIdModaleMalattia() As String

        If dgrMalattie.EditItemIndex > -1 Then
            Return DirectCast(dgrMalattie.Items(dgrMalattie.EditItemIndex).FindControl("txtMalattia"), Onit.Controls.OnitModalList).ClientID()
        End If

        Return String.Empty

    End Function

    Protected Function BindBooleanValue(itemValue As Object) As String

        If itemValue Is Nothing OrElse itemValue Is DBNull.Value Then
            Return Boolean.FalseString
        End If

        If itemValue.ToString() = "S" Then
            Return Boolean.TrueString
        End If

        Return Boolean.FalseString

    End Function

#End Region

End Class
