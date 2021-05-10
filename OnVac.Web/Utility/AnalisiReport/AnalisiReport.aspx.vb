Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared

Imports Onit.Database.DataAccessManager


Partial Class AnalisiReport
    Inherits OnVac.Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Protected WithEvents OnitCell2 As Onit.Controls.OnitCell
    Protected WithEvents OnitSection2 As Onit.Controls.OnitSection

    Protected WithEvents AjaxValues As System.Web.UI.HtmlControls.HtmlTable
    Protected WithEvents ImageButton As System.Web.UI.WebControls.ImageButton

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Properties "

    Public Property reportName() As String
        Set(ByVal Value As String)
            viewstate("reportName") = Value
        End Set
        Get
            Return viewstate("reportName")
        End Get
    End Property

    Public Property reportFolder() As String
        Set(ByVal Value As String)
            viewstate("reportFolder") = Value
        End Set
        Get
            Return viewstate("reportFolder")
        End Get
    End Property

    ' Purtroppo la stampa attraverso l'apertura della StampaReportPopUp 
    ' prevede il passaggio del dataset al report attraverso la session
    Private Property ReportPopUp_DataSource() As System.Data.DataSet
        Get
            Return Session("ReportFileName_PopUp_dataset")
        End Get
        Set(ByVal Value As System.Data.DataSet)
            Session("ReportFileName_PopUp_dataset") = Value
        End Set
    End Property

    ' Idem per i parametri
    Private Property ReportPar(ByVal rpt As String) As ArrayList
        Get
            Return Session(rpt & "_param")
        End Get
        Set(ByVal Value As ArrayList)
            Session(rpt & "_param") = Value
        End Set
    End Property

#End Region

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then
            RecuperaReport()
            RecuperaInstallazioni()
        End If

    End Sub

    Private Sub RecuperaInstallazioni()

        Dim dt As New DataTable()

        Using dam As IDAM = OnVacUtility.OpenDam()

            With dam.QB
                .NewQuery()
                .AddSelectFields("rownum as id, rpt_installazione")
                .AddTables("(select distinct rpt_installazione from t_ana_report)")
            End With

            dam.BuildDataTable(dt)

        End Using

        Dim dr As DataRow = dt.NewRow()
        dr("id") = "0"
        dr("rpt_installazione") = ""
        dt.Rows.Add(dr)

        Me.ddlInstallazione.DataSource = dt
        Me.ddlInstallazione.DataTextField = "rpt_installazione"
        Me.ddlInstallazione.DataValueField = "id"
        Me.ddlInstallazione.DataBind()
        Me.ddlInstallazione.SelectedValue = "0"

    End Sub

    Private Sub RecuperaReport()

        Dim dt As New DataTable()

        Dim addedRow As Integer = 0
        Dim ht As New Hashtable
        Dim key As String
        Dim j As Integer
        Dim arl As Hashtable

        Dim dam As IDAM = OnVacUtility.OpenDam

        With dam.QB
            .NewQuery()
            .AddSelectFields("*")
            .AddTables("t_ana_report")
            If ddlInstallazione.SelectedValue <> "" AndAlso ddlInstallazione.SelectedValue <> "0" Then
                .AddWhereCondition("rpt_installazione", Comparatori.Uguale, ddlInstallazione.SelectedItem.Text, DataTypes.Stringa)
            End If
            .AddOrderByFields("rpt_nome")
        End With

        Try

            Using idr As IDataReader = dam.BuildDataReader()

                For j = 0 To idr.FieldCount - 1
                    If Not dt.Columns.Contains(idr.GetName(j)) Then
                        dt.Columns.Add(idr.GetName(j), idr.GetFieldType(j))
                    End If
                Next

                While idr.Read()

                    If Not idr("RPT_NOME") Is Nothing AndAlso Not idr("RPT_CARTELLA") Is Nothing Then

                        key = idr("RPT_NOME") & idr("RPT_CARTELLA")

                        Dim installazioni As New System.Text.StringBuilder
                        installazioni.AppendFormat("{0} ", idr("RPT_INSTALLAZIONE"))
                        installazioni.Replace("Report", "")

                        If ht.Contains(key) Then

                            arl = ht(key)

                            installazioni.Append(arl("RPT_INSTALLAZIONE"))
                            arl.Remove("RPT_INSTALLAZIONE")
                            arl.Add("RPT_INSTALLAZIONE", installazioni)

                        Else

                            arl = New Hashtable

                            For j = 0 To idr.FieldCount - 1
                                If idr.GetName(j) = "RPT_INSTALLAZIONE" Then
                                    arl.Add(idr.GetName(j), installazioni)
                                Else
                                    arl.Add(idr.GetName(j), idr(j))
                                End If
                            Next

                            ht.Add(key, arl)

                        End If

                    End If
                End While

            End Using

        Finally
            OnVacUtility.CloseDam(dam)
        End Try

        Dim keysCollection As IDictionaryEnumerator = ht.Keys.GetEnumerator

        While keysCollection.MoveNext()

            arl = CType(ht.Item(keysCollection.Current), Hashtable)

            Dim innerKeysCollection As IDictionaryEnumerator = arl.Keys.GetEnumerator
            Dim itr As New ArrayList
            Dim dr As DataRow = dt.NewRow

            While innerKeysCollection.MoveNext()
                dr(innerKeysCollection.Current) = arl.Item(innerKeysCollection.Current)
            End While
            dt.Rows.Add(dr)

        End While

        dt.AcceptChanges()
        dt.DefaultView.Sort = "RPT_NOME"

        dgrReport.DataSource = dt.DefaultView
        dgrReport.DataBind()

    End Sub

    Private Sub tlbOperazioniGruppo_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles tlbOperazioniGruppo.ButtonClicked

        Select Case be.Button.Key
            Case "btnOpGrpCerca"
                RecuperaReport()
            Case "btnOpGrpStampa"
                Stampa()
        End Select

    End Sub

    Private Sub Stampa()

        ' --- Costruzione dataset --- '
        Dim dstRptDett As Dst_Report
        dstRptDett = _creaDatasetReportDesc()

        If dstRptDett Is Nothing Then
            OnitLayout.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Errore durante il recupero dei dati dei report. Stampa non effettuata.", "alert_err_stampa_rpt", False, False))
            Exit Sub
        End If

        Dim rptName As String = "DescReport"

        ' --- Parametri --- '
        ' Il report contiene i parametri Installazione e FiltroInstallazioneRpt. Il primo è comune a tutti, ed è impostato direttamente
        ' dalla StampaReportPopUp. Va passato a questa maschera solo il secondo, nel formato "<nome_parametro>|<valore>".
        Dim strValoreFiltro As String = String.Empty
        If ddlInstallazione.SelectedValue <> "" AndAlso ddlInstallazione.SelectedValue <> "0" Then
            strValoreFiltro = ddlInstallazione.SelectedItem.Text
        End If
        Dim par_list As New ArrayList
        par_list.Add(String.Format("FiltroInstallazioneRpt|{0}", strValoreFiltro))
        ReportPar(rptName) = par_list

        ' --- Apertura maschera creazione report e anteprima di stampa --- '
        ReportPopUp_DataSource = dstRptDett

        Dim _folder As String = String.Empty

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                _folder = bizReport.GetReportFolder(rptName + ".rpt")

            End Using
        End Using

        If (_folder = String.Empty) Then
            OnVacUtility.StampaNonPresente(Page, rptName + ".rpt")
        Else
            OnitLayout.InsertRoutineJS(String.Format("window.open('../../Stampe/StampaReportPopUp.aspx?report={0}','{0}','top=0,left=0,width=500,height=500,resizable=1')", rptName))
        End If

    End Sub


    ' Creazione dataset per report
    Private Function _creaDatasetReportDesc() As Dst_Report

        Dim dstRptDett As New Dst_Report()

        Dim dam As IDAM = OnVacUtility.OpenDam()

        With dam.QB
            .NewQuery()
            .AddSelectFields("rpt_nome, rpt_installazione, rpt_cartella, rpt_dataset, rpd_descrizione, rpd_utilizzato_da, rpd_standard")
            .AddSelectFields("rpd_maschera, rpd_flag_intestazione, rpd_rep_intestazione, rpd_rep_pie_pagina")
            .AddTables("t_ana_report, t_ana_report_desc")
            .AddWhereCondition("rpt_nome", Comparatori.Uguale, "rpd_nome", DataTypes.OutJoinLeft)
            .AddWhereCondition("rpt_cartella", Comparatori.Uguale, "rpd_cartella", DataTypes.OutJoinLeft)
            If ddlInstallazione.SelectedValue <> "" AndAlso ddlInstallazione.SelectedValue <> "0" Then
                .AddWhereCondition("rpt_installazione", Comparatori.Uguale, ddlInstallazione.SelectedItem.Text, DataTypes.Stringa)
            End If
            .AddOrderByFields("rpt_nome")
        End With

        Try
            Using dr As IDataReader = dam.BuildDataReader()

                If Not dr Is Nothing Then

                    Dim pos_rpt_nome As Integer = dr.GetOrdinal("rpt_nome")
                    Dim pos_rpt_installazione As Integer = dr.GetOrdinal("rpt_installazione")
                    Dim pos_rpt_cartella As Integer = dr.GetOrdinal("rpt_cartella")
                    Dim pos_rpt_dataset As Integer = dr.GetOrdinal("rpt_dataset")
                    Dim pos_rpd_descrizione As Integer = dr.GetOrdinal("rpd_descrizione")
                    Dim pos_rpd_utilizzato_da As Integer = dr.GetOrdinal("rpd_utilizzato_da")
                    Dim pos_rpd_standard As Integer = dr.GetOrdinal("rpd_standard")
                    Dim pos_rpd_maschera As Integer = dr.GetOrdinal("rpd_maschera")
                    Dim pos_rpd_flag_intestazione As Integer = dr.GetOrdinal("rpd_flag_intestazione")
                    Dim pos_rpd_rep_intestazione As Integer = dr.GetOrdinal("rpd_rep_intestazione")
                    Dim pos_rpd_rep_pie_pagina As Integer = dr.GetOrdinal("rpd_rep_pie_pagina")

                    Dim row_rpt_dett As Dst_Report.DettaglioReportRow

                    While dr.Read()

                        row_rpt_dett = dstRptDett.DettaglioReport.NewRow()

                        row_rpt_dett.rpt_nome = dr(pos_rpt_nome).ToString
                        row_rpt_dett.rpt_installazione = dr(pos_rpt_installazione).ToString
                        row_rpt_dett.rpt_cartella = dr(pos_rpt_cartella).ToString
                        row_rpt_dett.rpt_dataset = dr(pos_rpt_dataset).ToString
                        row_rpt_dett.rpd_descrizione = dr(pos_rpd_descrizione).ToString
                        row_rpt_dett.rpd_utilizzato_da = dr(pos_rpd_utilizzato_da).ToString
                        row_rpt_dett.rpd_standard = dr(pos_rpd_standard).ToString
                        row_rpt_dett.rpd_maschera = dr(pos_rpd_maschera).ToString
                        row_rpt_dett.rpd_flag_intestazione = dr(pos_rpd_flag_intestazione).ToString
                        row_rpt_dett.rpd_rep_intestazione = dr(pos_rpd_rep_intestazione).ToString
                        row_rpt_dett.rpd_rep_pie_pagina = dr(pos_rpd_rep_pie_pagina).ToString

                        dstRptDett.DettaglioReport.AddDettaglioReportRow(row_rpt_dett)

                    End While

                End If

            End Using

        Finally
            OnVacUtility.CloseDam(dam)
        End Try

        Return dstRptDett

    End Function

    Private Sub SalvaCampi(reportName As String, reportFolder As String)

        Using dam As IDAM = OnVacUtility.OpenDam()

            With dam.QB
                .NewQuery()
                .AddUpdateField("rpd_descrizione", txtDescrizione.Text, DataTypes.Stringa)
                .AddUpdateField("rpd_utilizzato_da", txtUtilizzato.Text, DataTypes.Stringa)
                .AddUpdateField("rpd_maschera", txtMaschera.Text, DataTypes.Stringa)
                .AddUpdateField("RPD_REP_INTESTAZIONE", txtIntestazione.Text, DataTypes.Stringa)
                .AddUpdateField("RPD_REP_PIE_PAGINA", txtPiede.Text, DataTypes.Stringa)
                .AddUpdateField("RPD_STANDARD", IIf(cbStandard.Checked, "S", "N"), DataTypes.Stringa)
                .AddUpdateField("RPD_FLAG_INTESTAZIONE", IIf(cbIntestazione.Checked, "S", "N"), DataTypes.Stringa)
                .AddTables("t_ana_report_desc")
                .AddWhereCondition("rpd_nome", Comparatori.Uguale, reportName, DataTypes.Stringa)
                .AddWhereCondition("rpd_cartella", Comparatori.Uguale, reportFolder, DataTypes.Stringa)
            End With

            dam.ExecNonQuery(ExecQueryType.Update)

        End Using

    End Sub

    Private Sub CaricaCampi(reportName As String, reportFolder As String)

        Dim dr As DataRow
        Dim dettaglio As New System.Text.StringBuilder()

        Dim _ReportDocument As ReportDocument
        Dim reportPath As String = HttpContext.Current.Request.PhysicalApplicationPath + "Report\" + reportFolder + "\" + reportName

        _ReportDocument = New ReportDocument
        _ReportDocument.Load(reportPath, OpenReportMethod.OpenReportByDefault)

        lblSelected.Text = reportFolder & " -> " & reportName
        txtDescrizione.Text = ""
        txtUtilizzato.Text = ""

        Dim dt As New DataTable()
        dt.Columns.Add("Campo")
        dt.Columns.Add("Dettaglio")
        dt.AcceptChanges()

        Using dam As IDAM = OnVacUtility.OpenDam()

            With dam.QB
                .NewQuery()
                .AddSelectFields("rpd_descrizione", "rpd_utilizzato_da", "rpd_standard", "rpd_maschera", "RPD_FLAG_INTESTAZIONE", "RPD_REP_INTESTAZIONE", "RPD_REP_PIE_PAGINA")
                .AddTables("t_ana_report_desc")
                .AddWhereCondition("rpd_nome", Comparatori.Uguale, reportName, DataTypes.Stringa)
                .AddWhereCondition("rpd_cartella", Comparatori.Uguale, reportFolder, DataTypes.Stringa)
            End With

            Using idr As System.Data.IDataReader = dam.BuildDataReader()

                Dim notExist As Boolean = True

                While idr.Read()

                    notExist = False
                    ' Descrizione
                    If idr("rpd_descrizione") Is Nothing OrElse idr("rpd_descrizione") Is System.DBNull.Value Then
                        txtDescrizione.Text = String.Empty
                    Else
                        txtDescrizione.Text = idr("rpd_descrizione")
                    End If

                    ' Utilizzato da
                    If idr("rpd_utilizzato_da") Is Nothing OrElse idr("rpd_utilizzato_da") Is System.DBNull.Value Then
                        txtUtilizzato.Text = String.Empty
                    Else
                        txtUtilizzato.Text = idr("rpd_utilizzato_da")
                    End If

                    ' standard
                    If idr("rpd_standard") Is Nothing OrElse idr("rpd_standard") Is System.DBNull.Value OrElse idr("rpd_standard") = "N" Then
                        cbStandard.Checked = False
                    Else
                        cbStandard.Checked = True
                    End If

                    ' Maschera
                    If idr("rpd_maschera") Is Nothing OrElse idr("rpd_maschera") Is System.DBNull.Value Then
                        txtMaschera.Text = String.Empty
                    Else
                        txtMaschera.Text = idr("rpd_maschera")
                    End If

                    ' intestazione personalizzata
                    If idr("RPD_FLAG_INTESTAZIONE") Is Nothing OrElse idr("RPD_FLAG_INTESTAZIONE") Is System.DBNull.Value OrElse idr("RPD_FLAG_INTESTAZIONE") = "N" Then
                        cbIntestazione.Checked = False
                    Else
                        cbIntestazione.Checked = True
                    End If

                    ' intestazione
                    If idr("RPD_REP_INTESTAZIONE") Is Nothing OrElse idr("RPD_REP_INTESTAZIONE") Is System.DBNull.Value Then
                        txtIntestazione.Text = String.Empty
                    Else
                        txtIntestazione.Text = idr("RPD_REP_INTESTAZIONE")
                    End If

                    ' piede
                    If idr("RPD_REP_PIE_PAGINA") Is Nothing OrElse idr("RPD_REP_PIE_PAGINA") Is System.DBNull.Value Then
                        txtPiede.Text = String.Empty
                    Else
                        txtPiede.Text = idr("RPD_REP_PIE_PAGINA")
                    End If

                End While

                If notExist Then
                    With dam.QB
                        .NewQuery()
                        .AddInsertField("rpd_nome", reportName, DataTypes.Stringa)
                        .AddInsertField("rpd_cartella", reportFolder, DataTypes.Stringa)
                        .AddTables("t_ana_report_desc")
                    End With

                    dam.ExecNonQuery(ExecQueryType.Insert)

                End If

            End Using

            ' Parametri
            dr = dt.NewRow()
            dr.Item("Campo") = "Parametri"
            dettaglio.Remove(0, dettaglio.Length)
            For i As Integer = 0 To _ReportDocument.ParameterFields.Count - 1
                If _ReportDocument.ParameterFields.Item(i).ReportName = reportName OrElse _ReportDocument.ParameterFields.Item(i).ReportName = "" Then
                    dettaglio.AppendFormat("{0}<br/>", _ReportDocument.ParameterFields.Item(i).Name)
                Else


                End If
            Next
            dr.Item("Dettaglio") = dettaglio.ToString
            dt.Rows.Add(dr)

            ' Sottoreport
            dr = dt.NewRow
            dr.Item("Campo") = "Sottoreport"
            dettaglio.Remove(0, dettaglio.Length)
            For i As Integer = 0 To _ReportDocument.Subreports.Count - 1
                dettaglio.AppendFormat("{0}<br/>", _ReportDocument.Subreports.Item(i).Name)
            Next
            dr.Item("Dettaglio") = dettaglio.ToString
            dt.Rows.Add(dr)

            ' Query
            dr = dt.NewRow
            dr.Item("Campo") = "Query"

            With dam.QB
                .NewQuery()
                .AddSelectFields("*")
                For i As Integer = 0 To _ReportDocument.Database.Tables.Count - 1
                    .AddTables(_ReportDocument.Database.Tables.Item(i).Name)
                Next
                For i As Integer = 0 To _ReportDocument.Database.Links.Count - 1
                    For j As Integer = 0 To _ReportDocument.Database.Links.Item(i).SourceFields.Count - 1
                        Dim sourceField As String = String.Format("{0}.{1}", _ReportDocument.Database.Links.Item(i).SourceTable.Name, _ReportDocument.Database.Links.Item(i).SourceFields.Item(j).Name)
                        Dim destField As String = String.Format("{0}.{1}", _ReportDocument.Database.Links.Item(i).DestinationTable.Name, _ReportDocument.Database.Links.Item(i).DestinationFields.Item(j).Name)

                        Dim joinType As DataTypes

                        Select Case _ReportDocument.Database.Links.Item(i).JoinType
                            Case LinkJoinType.Equal
                                joinType = DataTypes.Join
                            Case LinkJoinType.LeftOuter
                                joinType = DataTypes.OutJoinLeft
                            Case LinkJoinType.RightOuter
                                joinType = DataTypes.OutJoinRight
                            Case Else
                                joinType = DataTypes.Replace
                        End Select

                        .AddWhereCondition(sourceField, Comparatori.Uguale, destField, joinType)

                    Next
                Next

                dr.Item("Dettaglio") = .GetSelect

            End With

        End Using

        dt.Rows.Add(dr)

        ' Datasource
        dr = dt.NewRow
        dr.Item("Campo") = "Datasource"
        dettaglio.Remove(0, dettaglio.Length)
        For i As Integer = 0 To _ReportDocument.DataSourceConnections.Count - 1
            dettaglio.AppendFormat("db: {0}, user: {1}<br/>", _ReportDocument.DataSourceConnections.Item(i).ServerName, _ReportDocument.DataSourceConnections.Item(i).UserID)
        Next
        dr.Item("Dettaglio") = dettaglio.ToString
        dt.Rows.Add(dr)

        dt.AcceptChanges()

        dgrDettaglio.DataSource = dt
        dgrDettaglio.DataBind()

    End Sub

    Sub ImageButton_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton.Click

        reportName = CType(sender, System.Web.UI.WebControls.ImageButton).CommandName
        reportFolder = CType(sender, System.Web.UI.WebControls.ImageButton).CommandArgument

        CaricaCampi(reportName, reportFolder)

    End Sub

    Sub ImgSaveDetail_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgSaveDetail.Click

        SalvaCampi(reportName, reportFolder)

    End Sub

    Private Sub dgrReport_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgrReport.SelectedIndexChanged

    End Sub

    Private Sub dgrReport_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgrReport.ItemCommand

    End Sub
End Class