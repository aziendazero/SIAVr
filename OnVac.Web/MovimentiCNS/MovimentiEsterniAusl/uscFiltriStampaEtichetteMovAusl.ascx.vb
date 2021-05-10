Imports Onit.Database.DataAccessManager

Partial Class uscFiltriStampaEtichetteMovAusl
    Inherits Common.UserControlPageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(sender As System.Object, e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Fields "

    Protected WithEvents btnStampa As System.Web.UI.WebControls.Button
    Protected WithEvents btnAnnulla As System.Web.UI.WebControls.Button

#End Region

#Region " Properties "

    Public Property TipoMovimento() As String
        Get
            Return viewstate("OnVac_TipoMovimento" + Me.ClientID)
        End Get
        Set(Value As String)
            ViewState("OnVac_TipoMovimento" + Me.ClientID) = Value
        End Set
    End Property

#End Region

#Region " Types "

    Private Class TipoEtichette

        Public Const EtichetteEsterne As String = "E"
        Public Const EtichetteInterne As String = "I"
        Public Const EtichetteComune As String = "C"

    End Class

    Private Class TipoFiltro

        Public Const SoloMovimentiNonStampati As String = "N"       ' stampa solo quelli non ancora stampati
        Public Const RistampaMovimentiStampati As String = "R"      ' ristampa quelli già stampati
        Public Const StampaTuttiMovimenti As String = "T"           ' stampare tutti i movimenti

    End Class

    Private Class TipoAssistitiMovimento

        Public Const Emigrati As String = "E"
        Public Const Immigrati As String = "I"

    End Class

#End Region

#Region " Event Handlers "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Me.TipoMovimento = TipoAssistitiMovimento.Emigrati Then

            Me.rdbTipoStampa.Visible = False
            Me.primoRadioButton.Width = "0%"
            Me.spaziatura.Width = "0%"
            Me.secondoRadioButton.Width = "100%"

        ElseIf Me.TipoMovimento = TipoAssistitiMovimento.Immigrati Then

            Me.primoRadioButton.Width = "46%"
            Me.spaziatura.Width = "8%"
            Me.secondoRadioButton.Width = "46%"

        End If

    End Sub

#End Region

#Region " Public Methods "

    Public Function StampaEtichette(dstMovimentiEsterni As DstMovimentiEsterni, ByRef errorMessage As String) As Boolean

        ' Controllo la presenza di dati nel dataset
        If dstMovimentiEsterni Is Nothing OrElse
           dstMovimentiEsterni.Tables Is Nothing OrElse
           dstMovimentiEsterni.Tables.Count = 0 OrElse
           dstMovimentiEsterni.Tables(0).Rows.Count = 0 Then

            errorMessage = "Nessuna etichetta da stampare. Stampa non effettuata."
            Return False

        End If

        ' Filtro il dataset in base al tipo di stampa selezionata
        Dim tipoFiltroReport As String = rdbTipoReport.SelectedValue
        Dim tipoEtichetteStampa As String = rdbTipoStampa.SelectedValue

        Dim dtStampati As DataTable = CaricaMovimentiGiaStampati(dstMovimentiEsterni, tipoEtichetteStampa)

        If Not ImpostaDatasetEtichette(dstMovimentiEsterni, tipoFiltroReport, dtStampati, errorMessage) Then
            Return False
        End If

        ' La funzione ImpostaDatasetEtichette elimina righe dal dataset dstMovimentiEsterni.
        ' Quindi, poichè il dataset può essere rimasto senza righe, rifà il controllo.
        If dstMovimentiEsterni.Tables(0).Rows.Count = 0 Then
            errorMessage = "Nessuna etichetta da stampare. Stampa non effettuata"
            Return False
        End If

        Dim rpt As New ReportParameter()

        ' Dataset
        rpt.set_dataset(dstMovimentiEsterni)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            ' Lettura tipo consultorio corrente
            Dim tipoConsultorio As String = String.Empty

            Using bizConsultori As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                tipoConsultorio = bizConsultori.GetTipoConsultorio(OnVacUtility.Variabili.CNS.Codice)
            End Using

            ' Parametri report
            rpt.AddParameter("Tipo", tipoEtichetteStampa)       ' E:esterne, I:interne, C:comune
            rpt.AddParameter("TipoCNS", tipoConsultorio)        ' A:adulti, P:pediatrico, V:vaccinale, "":nullo

            ' Creazione report
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If Not OnVacReport.StampaReport(Page.Request.Path, Constants.ReportName.EtichetteImmigrati, String.Empty, rpt, , , bizReport.GetReportFolder(Constants.ReportName.EtichetteImmigrati)) Then
                    OnVacUtility.StampaNonPresente(Page, Constants.ReportName.EtichetteImmigrati)
                Else
                    ' Stampa effettuata, quindi inserimento/update della t_stampa_movimenti_ausl su db
                    RegistraMovimentiStampati(dstMovimentiEsterni, tipoFiltroReport, tipoEtichetteStampa, dtStampati, errorMessage)
                End If

            End Using

        End Using

        Return True

    End Function

#End Region

#Region " Private Methods "

    ' tipoFiltroReport:     N = stampa solo quelli non ancora stampati presenti nel dataset
    '                       R = ristampa quelli già stampati (presenti nel dataset)
    '                       T = stampa tutti gli assisiti presenti nel dataset
    '
    ' tipoEtichetteStampa:  E = etichette di tipo esterno
    '                       I = etichette di tipo interno
    '                       C = etichette per il comune
    Private Function ImpostaDatasetEtichette(ByRef dstMovimentiEsterni As DstMovimentiEsterni, tipoFiltroReport As String, dtStampati As DataTable, ByRef errorMessage As String) As Boolean

        Dim listCodici As New ArrayList()

        For i As Integer = 0 To dtStampati.Rows.Count - 1
            listCodici.Add(dtStampati.Rows(i)("sma_paz_codice").ToString())
        Next

        Select Case tipoFiltroReport

            Case TipoFiltro.SoloMovimentiNonStampati

                ' Stampa solo quelli non ancora stampati presenti nel dataset
                If listCodici.Count = 0 Then Return True

                For i As Integer = dstMovimentiEsterni.Tables(0).Rows.Count - 1 To 0 Step -1
                    If listCodici.Contains(dstMovimentiEsterni.Tables(0).Rows(i)("paz_codice").ToString()) Then
                        dstMovimentiEsterni.Tables(0).Rows(i).Delete()
                    End If
                Next

            Case TipoFiltro.RistampaMovimentiStampati

                ' Ristampa quelli già stampati presenti nel dataset
                If listCodici.Count = 0 Then
                    errorMessage = "Nessuna etichetta da ristampare. Stampa non effettuata."
                    Return False
                End If

                For i As Integer = dstMovimentiEsterni.Tables(0).Rows.Count - 1 To 0 Step -1
                    If Not listCodici.Contains(dstMovimentiEsterni.Tables(0).Rows(i)("paz_codice").ToString()) Then
                        dstMovimentiEsterni.Tables(0).Rows(i).Delete()
                    End If
                Next

            Case TipoFiltro.StampaTuttiMovimenti

                ' Nessun filtro, stampo tutti quelli del dataset

        End Select

        dstMovimentiEsterni.Tables(0).AcceptChanges()

        Return True

    End Function

    ' tipoFiltroReport:     N = ho stampato solo quelli non ancora stampati presenti nel dataset, quindi inserisco
    '                           su db tutti i record del dataset
    '                       R = ho ristampato quelli già stampati (presenti nel dataset), quindi effettuo l'update
    '                           dei record presenti nel dataset
    '                       T = ho stampato tutti gli assisiti presenti nel dataset, quindi devo inserire quelli
    '                           non presenti e aggiornare quelli che c'erano già
    Private Function RegistraMovimentiStampati(dstMovimentiEsterni As DstMovimentiEsterni, tipoFiltroReport As String, tipoEtichetteStampa As String, dtStampati As DataTable, ByRef err_msg As String) As Boolean

        Select Case tipoFiltroReport

            Case TipoFiltro.SoloMovimentiNonStampati

                InserimentoDati(dstMovimentiEsterni.Tables(0), tipoEtichetteStampa)

            Case TipoFiltro.RistampaMovimentiStampati

                AggiornamentoDati(dstMovimentiEsterni.Tables(0), tipoEtichetteStampa)

            Case TipoFiltro.StampaTuttiMovimenti

                ' Divido i record già stampati da quelli stampati la prima volta
                Dim listCodici As New ArrayList()

                For i As Integer = 0 To dtStampati.Rows.Count - 1
                    listCodici.Add(dtStampati.Rows(i)("sma_paz_codice").ToString())
                Next

                Dim dtUpdate As New DstMovimentiEsterni.MovimentiEsterniDataTable()
                Dim dtInsert As New DstMovimentiEsterni.MovimentiEsterniDataTable()

                Dim row As DataRow

                For i As Integer = 0 To dstMovimentiEsterni.Tables(0).Rows.Count - 1

                    If listCodici.Contains(dstMovimentiEsterni.Tables(0).Rows(i)("paz_codice").ToString) Then
                        row = dtUpdate.NewRow()
                        row.ItemArray = dstMovimentiEsterni.Tables(0).Rows(i).ItemArray
                        dtUpdate.Rows.Add(row)
                    Else
                        row = dtInsert.NewRow()
                        row.ItemArray = dstMovimentiEsterni.Tables(0).Rows(i).ItemArray
                        dtInsert.Rows.Add(row)
                    End If

                Next

                dtUpdate.AcceptChanges()
                dtInsert.AcceptChanges()

                If dtUpdate.Rows.Count > 0 Then
                    AggiornamentoDati(dtUpdate, tipoEtichetteStampa)
                End If

                If dtInsert.Rows.Count > 0 Then
                    InserimentoDati(dtInsert, tipoEtichetteStampa)
                End If

        End Select

        Return True

    End Function

    ' Carica i movimenti già stampati del tipo specificato, per quanto riguarda i pazione
    Private Function CaricaMovimentiGiaStampati(dstMovimentiEsterni As DstMovimentiEsterni, tipoEtichetteStampa As String) As DataTable

        Dim dt As New DataTable()

        Using dam As IDAM = OnVacUtility.OpenDam()

            ' Creo la stringa con i codici dei paz per la condizione di in
            Dim stbCodici As New System.Text.StringBuilder()

            With dam.QB
                .NewQuery()
                .AddSelectFields("sma_codice, sma_paz_codice, sma_data_stampa, sma_tipo_stampa")
                .AddTables("t_stampa_movimenti_ausl")

                'ciclo ogni 1000 perchè nella query sono ammessi non più di 1000 elementi dentro la condizione di IN 
                .OpenParanthesis()
                Dim maxCount As Integer = 1000
                Dim count As Integer = 0
                Dim firstCondition As Boolean = True
                For i As Integer = 0 To dstMovimentiEsterni.Tables(0).Rows.Count - 1
                    stbCodici.AppendFormat("{0},", dstMovimentiEsterni.Tables(0).Rows(i)("paz_codice").ToString())
                    count += 1
                    If dstMovimentiEsterni.Tables(0).Rows.Count > maxCount AndAlso count = maxCount Then
                        If stbCodici.Length > 0 Then stbCodici.Remove(stbCodici.Length - 1, 1)
                        .AddWhereCondition("sma_paz_codice", Comparatori.In, stbCodici.ToString(), DataTypes.Replace, IIf(firstCondition, "And", "Or"))
                        If firstCondition Then firstCondition = False
                        stbCodici.Clear()
                        count = 0
                    End If
                Next
                If stbCodici.Length > 0 Then
                    stbCodici.Remove(stbCodici.Length - 1, 1)
                    .AddWhereCondition("sma_paz_codice", Comparatori.In, stbCodici.ToString(), DataTypes.Replace, IIf(firstCondition, "And", "Or"))
                End If
                .CloseParanthesis()

                .AddWhereCondition("sma_tipo_stampa", Comparatori.Uguale, tipoEtichetteStampa, DataTypes.Stringa)
            End With

            dam.BuildDataTable(dt)


        End Using

        Return dt

    End Function

    Private Sub InserimentoDati(dtInsert As DataTable, tipoEtichetteStampa As String)

        Using dam As IDAM = OnVacUtility.OpenDam()

            dam.BeginTrans()

            Try

                For i As Integer = 0 To dtInsert.Rows.Count - 1
                    With dam.QB
                        .NewQuery()
                        .AddInsertField("sma_paz_codice", dtInsert.Rows(i)("paz_codice"), DataTypes.Numero)
                        .AddInsertField("sma_data_stampa", Date.Now.Date, DataTypes.Data)
                        .AddInsertField("sma_tipo_stampa", tipoEtichetteStampa, DataTypes.Stringa)
                        .AddTables("t_stampa_movimenti_ausl")
                    End With
                    dam.ExecNonQuery(ExecQueryType.Insert)
                Next

                dam.Commit()

            Catch ex As Exception

                dam.Rollback()

                ex.InternalPreserveStackTrace()
                Throw

            End Try

        End Using

    End Sub

    Private Sub AggiornamentoDati(dtUpdate As DataTable, tipoStampa As String)

        Dim stbCodici As New System.Text.StringBuilder()

        For i As Integer = 0 To dtUpdate.Rows.Count - 1
            stbCodici.AppendFormat("{0},", dtUpdate.Rows(i)("paz_codice").ToString())
        Next

        If stbCodici.Length > 0 Then
            stbCodici.Remove(stbCodici.Length - 1, 1)
        End If

        Using dam As IDAM = OnVacUtility.OpenDam()

            dam.BeginTrans()

            Try
                With dam.QB
                    .NewQuery()
                    .AddUpdateField("sma_data_stampa", Date.Now.Date, DataTypes.Data)
                    .AddWhereCondition("sma_paz_codice", Comparatori.In, stbCodici.ToString, DataTypes.Replace)
                    .AddWhereCondition("sma_tipo_stampa", Comparatori.Uguale, tipoStampa, DataTypes.Stringa)
                    .AddTables("t_stampa_movimenti_ausl")
                End With
                dam.ExecNonQuery(ExecQueryType.Update)

                dam.Commit()

            Catch ex As Exception

                dam.Rollback()

                ex.InternalPreserveStackTrace()
                Throw

            End Try

        End Using

    End Sub

#End Region

End Class
