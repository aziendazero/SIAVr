Imports Onit.Database.DataAccessManager
Imports System.Xml
Imports System.Collections.Generic

Partial Class OnVac_CalendarioCup
    Inherits OnVac.Common.PageBase

#Region " Codice generato da Progettazione Web Form "



    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Private Properties "

    Private Property dtsAppPre() As System.Data.DataSet
        Get
            Return Session("CalendarioCup_dtsAppPre")
        End Get
        Set(ByVal Value As System.Data.DataSet)
            Session("CalendarioCup_dtsAppPre") = Value
        End Set
    End Property

    Private Property espandiTestate() As System.Text.StringBuilder
        Get
            Return ViewState("espandiTestate")
        End Get
        Set(ByVal Value As System.Text.StringBuilder)
            ViewState("espandiTestate") = Value
        End Set
    End Property

    Private Property espandiPazienti() As System.Text.StringBuilder
        Get
            Return ViewState("espandiPazienti")
        End Get
        Set(ByVal Value As System.Text.StringBuilder)
            ViewState("espandiPazienti") = Value
        End Set
    End Property

    Private Property strEspandiComprimi() As String
        Get
            Return ViewState("strEspandiComprimi")
        End Get
        Set(ByVal Value As String)
            ViewState("strEspandiComprimi") = Value
        End Set
    End Property

#End Region

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Select Case Request.Form("__EVENTTARGET")
            Case "Ricerca"
                Cerca()
        End Select

        If Not IsPostBack Then

            txtData.Text = DateTime.Today

            txtConsultorio.Codice = OnVacUtility.Variabili.CNS.Codice
            txtConsultorio.RefreshDataBind()

            ShowPrintButtons()

            Cerca()

        End If

    End Sub

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()

        listPrintButtons.Add(New PrintButton(Constants.ReportName.AppuntamentiDelGiornoCup, "btnStampa"))

        Me.ShowToolbarPrintButtons(listPrintButtons, ToolBar)

    End Sub

    Private Function datasetStampa() As System.Data.DataSet

        Dim ds As New System.Data.DataSet()

        ds = dtsAppPre.Copy()
        ds.Tables("Appuntamenti").Columns.Add("CodicePaziente", GetType(Integer))
        ds.Tables("Appuntamenti").Columns.Add("DescComuneResidenza")
        ds.Tables("Appuntamenti").Columns.Add("MedicoBase")
        ds.Tables("Appuntamenti").Columns.Add("NotePaziente")
        ds.Tables("Appuntamenti").Columns.Add("DataUltimaVac", GetType(DateTime))

        Using dam As IDAM = OnVacUtility.OpenDam()

            Dim dtPaz As New DataTable()
            Dim dtComRes As New DataTable()

            Dim s As New System.Text.StringBuilder()

            With dam.QB
                .NewQuery()
                .AddSelectFields("pno_testo_note")
                .AddTables("t_ana_tipo_note, t_paz_note")
                .AddWhereCondition("tno_codice", Comparatori.Uguale, "pno_tno_codice", DataTypes.Join)
                .AddWhereCondition("pno_paz_codice", Comparatori.Uguale, "paz_codice", DataTypes.Join)
                .OpenParanthesis()
                .AddWhereCondition("pno_azi_codice", Comparatori.Uguale, OnVacContext.CodiceUslCorrente, DataTypes.Stringa)
                .AddWhereCondition("pno_azi_codice", Comparatori.In, String.Format("select dis_codice from t_ana_distretti where dis_usl_codice = '{0}'", OnVacContext.CodiceUslCorrente), DataTypes.Replace, "OR")
                .CloseParanthesis()
                .AddWhereCondition("pno_tno_codice", Comparatori.Uguale, Constants.CodiceTipoNotaPaziente.Appuntamenti, DataTypes.Stringa)
            End With
            Dim queryNotaLibero1 As String = dam.QB.GetSelect()


            dam.QB.NewQuery(False, False)


            With dam.QB

                For i As Integer = 0 To ds.Tables("Appuntamenti").Rows.Count - 1
                    s.AppendFormat("{0},", dam.QB.AddCustomParam(ds.Tables("Appuntamenti").Rows(i)("CodicePazienteAusiliario")))
                Next
                If s.ToString.Length > 1 Then s.Remove(s.ToString.Length - 1, 1)

                .AddSelectFields("PAZ_CODICE,PAZ_CODICE_AUSILIARIO,replace(MED_DESCRIZIONE,'*',' ') as MED_DESCRIZIONE")
                .AddSelectFields("(" + queryNotaLibero1 + ") PAZ_LIBERO_1")
                .AddSelectFields("max(VES_DATA_EFFETTUAZIONE) as DataUltimaVac")
                .AddTables("T_PAZ_PAZIENTI,T_ANA_MEDICI,T_VAC_ESEGUITE")
                .AddWhereCondition("PAZ_CODICE_AUSILIARIO", Comparatori.In, s.ToString, DataTypes.Replace)
                .AddWhereCondition("MED_CODICE", Comparatori.Uguale, "PAZ_MED_CODICE_BASE", DataTypes.OutJoinLeft)
                .AddWhereCondition("PAZ_CODICE", Comparatori.Uguale, "VES_PAZ_CODICE", DataTypes.OutJoinLeft)
                .AddGroupByFields("PAZ_CODICE,PAZ_CODICE_AUSILIARIO,MED_DESCRIZIONE,PAZ_LIBERO_1")

            End With

            dam.BuildDataTable(dtPaz)

            s = New System.Text.StringBuilder()

            With dam.QB

                .NewQuery()

                For i As Integer = 0 To ds.Tables("Appuntamenti").Rows.Count - 1
                    s.AppendFormat("{0},", dam.QB.AddCustomParam(ds.Tables("Appuntamenti").Rows(i)("ComuneResidenza").ToString))
                Next
                If s.ToString.Length > 1 Then s.Remove(s.ToString.Length - 1, 1)

                .AddSelectFields("COM_CODICE,COM_DESCRIZIONE")
                .AddTables("T_ANA_COMUNI")
                .AddWhereCondition("COM_CODICE", Comparatori.In, s.ToString, DataTypes.Replace)

            End With

            dam.BuildDataTable(dtComRes)

            For i As Integer = 0 To dtPaz.Rows.Count - 1
                For j As Integer = 0 To ds.Tables("Appuntamenti").Rows.Count - 1

                    If ds.Tables("Appuntamenti").Rows(j)("CodicePazienteAusiliario").ToString() = dtPaz.Rows(i)("PAZ_CODICE_AUSILIARIO").ToString() Then

                        ds.Tables("Appuntamenti").Rows(j)("CodicePaziente") = dtPaz.Rows(i)("PAZ_CODICE")
                        ds.Tables("Appuntamenti").Rows(j)("MedicoBase") = dtPaz.Rows(i)("MED_DESCRIZIONE").ToString()
                        ds.Tables("Appuntamenti").Rows(j)("NotePaziente") = dtPaz.Rows(i)("PAZ_LIBERO_1").ToString()
                        ds.Tables("Appuntamenti").Rows(j)("DataUltimaVac") = dtPaz.Rows(i)("DataUltimaVac")

                    End If

                Next
            Next

            For i As Integer = 0 To dtComRes.Rows.Count - 1
                For j As Integer = 0 To ds.Tables("Appuntamenti").Rows.Count - 1

                    If ds.Tables("Appuntamenti").Rows(j)("ComuneResidenza").ToString() = dtComRes.Rows(i)("COM_CODICE").ToString() Then

                        ds.Tables("Appuntamenti").Rows(j)("DescComuneResidenza") = dtComRes.Rows(i)("COM_DESCRIZIONE").ToString()

                    End If

                Next
            Next

        End Using

        Return ds

    End Function

    Private Sub ToolBar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked
        '--
        Select Case e.Button.Key
            '--
            Case "btnCerca"
                '--
                Cerca()
                '--
            Case "btnStampa"
                '--
                Dim rpt As New ReportParameter()
                rpt.AddParameter("Consultorio", txtConsultorio.Descrizione)
                '--
                If dgrTestate.Items.Count > 0 Then
                    '--
                    Dim ds As System.Data.DataSet = datasetStampa()
                    '--
                    rpt.set_dataset(ds)
                    '--
                    Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                        Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                            '--
                            If Not OnVacReport.StampaReport(Page.Request.Path, Constants.ReportName.AppuntamentiDelGiornoCup, String.Empty, rpt, , , bizReport.GetReportFolder(Constants.ReportName.AppuntamentiDelGiornoCup)) Then
                                OnVacUtility.StampaNonPresente(Page, Constants.ReportName.AppuntamentiDelGiornoCup)
                            End If
                            '--
                        End Using
                    End Using
                    '--
                Else
                    '--
                    OnitLayout31.InsertRoutineJS("alert('Nessun Paziente da stampare!');")
                    '--
                End If
                '--
        End Select
        '--
    End Sub

    Sub Cerca()

        Dim stbErr As New System.Text.StringBuilder()

        CaricamentoAppuntamentiAgendeCup(stbErr)

        If stbErr.ToString() = "" Then
            CaricaDataGrid()
        Else
            Me.OnitLayout31.InsertRoutineJS("alert('" & stbErr.ToString & "');")
        End If

    End Sub

    Private Sub CaricaDataGrid()

        espandiTestate = New System.Text.StringBuilder()
        espandiPazienti = New System.Text.StringBuilder()

        dtsAppPre.Tables("Appuntamenti").DefaultView.Sort = "Data,Ora"

        Me.dgrTestate.DataSource = dtsAppPre.Tables("Appuntamenti").DefaultView
        Me.dgrTestate.DataBind()

        'espansione/compressione
        strEspandiComprimi = "function EspandiComprimi(espandi){" & Chr(13) & Chr(10)
        strEspandiComprimi &= espandiTestate.ToString
        strEspandiComprimi &= espandiPazienti.ToString
        strEspandiComprimi &= "}"

        Me.OnitLayout31.InsertRoutineJS(strEspandiComprimi)

    End Sub

    Private Sub dgrTestate_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrTestate.ItemDataBound

        If e.Item.ItemType <> ListItemType.Header And e.Item.ItemType <> ListItemType.Footer Then

            Dim dgr As DataGrid = e.Item.FindControl("dgrRecord")
            Dim codice As Label = e.Item.FindControl("lblRichiesta")

            AddHandler dgr.ItemDataBound, AddressOf dgrRecord_ItemDataBound

            dtsAppPre.Tables("Prestazioni").DefaultView.RowFilter = String.Format("NumeroRichiesta = '{0}'", codice.Text)

            dgr.DataSource = dtsAppPre.Tables("Prestazioni").DefaultView
            dgr.DataBind()

            Dim img As HtmlImage = e.Item.FindControl("imgEspandi2")
            Dim tbl As HtmlTable = e.Item.FindControl("TableTestate")
            img.Attributes.Add("onclick", String.Format("Espandi('{0}','{1}','{2}','{3}')", dgr.ClientID, img.ClientID, tbl.ClientID, 2))
            img.Attributes.Add("stato", True)

            If dtsAppPre.Tables("Prestazioni").DefaultView.Count = 0 Then
                img.Style.Add("display", "none")
            Else
                espandiTestate.Append(String.Format("EspandiTutto(espandi,'{0}','{1}','{2}','{3}');", dgr.ClientID, img.ClientID, tbl.ClientID, 2) & Chr(13) & Chr(10))
            End If

        End If

    End Sub

    Private Sub dgrRecord_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs)

        If e.Item.ItemType <> ListItemType.Header And e.Item.ItemType <> ListItemType.Footer Then

        End If

    End Sub

    Private Function GetPazienteLocale(codiceCentrale As String) As List(Of String)

        Dim listCodiciPazienti As List(Of String)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizPaziente As New Biz.BizPaziente(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                listCodiciPazienti = bizPaziente.GetCodicePazientiByCodiceAusiliario(codiceCentrale)

            End Using

        End Using

        Return listCodiciPazienti

    End Function

    Private Sub dgrTestate_ItemCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgrTestate.ItemCommand

        Dim listCodiciPazienti As List(Of String) = Me.GetPazienteLocale(DirectCast(e.Item.FindControl("lblCodPaziente"), Label).Text)

        If listCodiciPazienti.Count = 1 Then

            OnVacUtility.Variabili.PazId = listCodiciPazienti.First()

            Select Case e.CommandName
                Case "Nome"
                    Me.RedirectToConvocazioniPaziente(OnVacUtility.Variabili.PazId)
            End Select

        ElseIf listCodiciPazienti.Count > 1 Then

            Me.OnitLayout31.InsertRoutineJS("alert('E\' presente piu\' di un paziente con lo stesso codice centrale, ricercare in anagrafe per cognome e nome')")

        ElseIf listCodiciPazienti.Count = 0 Then

            Me.OnitLayout31.InsertRoutineJS("alert('Paziente non presente in anagrafe locale, ricercare in anagrafe centrale per cognome e nome')")

        End If

    End Sub

    Private Function RecuperaUnieroAssociateCns() As String()

        Dim dt As New DataTable()

        Using dam As IDAM = OnVacUtility.OpenDam()

            With dam.QB
                .NewQuery()
                .AddSelectFields("COU_UNI_CODICE")
                .AddTables("T_ANA_LINK_CONSULTORI_UNIERO")
                .AddWhereCondition("COU_CNS_CODICE", Comparatori.Uguale, txtConsultorio.Codice, DataTypes.Stringa)
            End With
            dam.BuildDataTable(dt)

        End Using

        Dim t As New System.Text.StringBuilder()

        For i As Integer = 0 To dt.Rows.Count - 1
            t.AppendFormat("{0},", dt.Rows(i)(0).ToString())
        Next
        If t.Length > 0 Then t.Remove(t.Length - 1, 1)

        Return t.ToString().Split(",")

    End Function

    Private Function RitornaDatiUniero(codice As String) As DataTable

        Dim dt As New DataTable()

        Using dam As IDAM = OnVacUtility.OpenDam()

            With dam.QB
                .NewQuery()
                .AddSelectFields("UNI_MNEMONICO as mnemonico,UNI_DESCRIZIONE as descrizione")
                .AddTables("T_ANA_UNITA_EROGATRICI")
                .AddWhereCondition("UNI_CODICE", Comparatori.Uguale, codice, DataTypes.Numero)
            End With
            dam.BuildDataTable(dt)

        End Using

        Return dt

    End Function

    Private Sub ImpostaStrutturaDataset(ByRef dst As System.Data.DataSet)

        Dim dt As New DataTable()
        dt.TableName = "Appuntamenti"

        dt.Columns.Add("NumeroRichiesta")
        dt.Columns.Add("Data", GetType(DateTime))
        dt.Columns.Add("Ora", GetType(DateTime))
        dt.Columns.Add("CodicePazienteAusiliario")
        dt.Columns.Add("ComuneResidenza")
        dt.Columns.Add("CapResidenza")
        dt.Columns.Add("Cognome")
        dt.Columns.Add("Nome")
        dt.Columns.Add("DataNascita")
        dt.Columns.Add("TipoAgenda")
        dt.Columns.Add("TipoRichiesta")

        Dim dt2 As New DataTable()
        dt2.TableName = "Prestazioni"

        dt2.Columns.Add("NumeroRichiesta")
        dt2.Columns.Add("PrestazioneMnemonico")
        dt2.Columns.Add("PrestazioneDescrizione")
        dt2.Columns.Add("ProfiloMnemonico")
        dt2.Columns.Add("ProfiloDescrizione")

        dst.Tables.Add(dt)
        dst.Tables.Add(dt2)

    End Sub

    Private Sub CaricamentoAppuntamentiAgendeCup(ByRef stbErr As System.Text.StringBuilder)

        Dim WsSgp As OnVac.wsSGP.WsSgp
        Dim stbXmlIn As System.Text.StringBuilder
        Dim stbXmlOut As System.Text.StringBuilder
        Dim ds As System.Data.DataSet

        Dim uni_ero_mnem As String = String.Empty
        Dim dt As DataTable
        Dim x As Integer = 0

        Dim di, df As String
        Dim CodUniEro() As String

        Try
            ' Devo interrogare il web service o per l'uni ero selezionata o per tutte quelle abilitate
            ' Ciclo di interrogazione del web service (per ogni unità erogatrice)
            CodUniEro = RecuperaUnieroAssociateCns()

            dtsAppPre = New System.Data.DataSet
            ImpostaStrutturaDataset(dtsAppPre)

            For idx_uni_ero As Integer = 0 To CodUniEro.Length - 1

                dt = RitornaDatiUniero(CodUniEro(idx_uni_ero))
                If dt.Rows.Count <> 0 Then
                    uni_ero_mnem = dt.Rows(0)("mnemonico")
                End If

                ' --------- Messaggio di interrogazione dei piani di lavoro --------- '
                stbXmlIn = New System.Text.StringBuilder
                stbXmlIn.Append("<?xml version=""1.0"" encoding=""UTF-8""?>")
                stbXmlIn.Append("<XmlInPianiDiLavoro>")
                stbXmlIn.Append("<PDLH>")
                stbXmlIn.AppendFormat("<DominioInviante>{0}</DominioInviante>", Me.Settings.POLO_ONVAC)
                stbXmlIn.AppendFormat("<DataOperazione>{0}</DataOperazione>", String.Format("{0:dd/MM/yyyy HH:mm:ss}", Date.Now))
                stbXmlIn.Append("</PDLH>")
                stbXmlIn.Append("<PDLQ>")
                stbXmlIn.AppendFormat("<UnitaErogatrice>{0}</UnitaErogatrice>", uni_ero_mnem)

                di = Format(txtData.Data, "dd/MM/yyyy").ToString
                df = di
                stbXmlIn.AppendFormat("<DataInizio>{0}</DataInizio>", di)
                stbXmlIn.AppendFormat("<DataFine>{0}</DataFine>", df)

                stbXmlIn.Append("</PDLQ>")
                stbXmlIn.Append("</XmlInPianiDiLavoro>")

                WsSgp = New OnVac.wsSGP.WsSgp
                stbXmlOut = New System.Text.StringBuilder
                '''''''''''''''''''''''''''''''''''''''''''''''''''''''
                '' Chiamata al metodo del servizio
                ''
                '' if NOT (stiamo lavorando in VPN o dentro USL) then
                '
                stbXmlOut.Append(WsSgp.PianiDiLavoro(stbXmlIn.ToString))
                ''
                ''    elseif (per provare in Onit)
                ''
                'stbXmlOut = New System.Text.StringBuilder
                'stbXmlOut.Append("<?xml version=""1.0"" encoding=""UTF-8""?><XmlOutPianiDiLavoro><PDLH><DominioInviante>700</DominioInviante><DataOperazione>24/08/2006 11.54.59</DataOperazione></PDLH><MSA><CodiceAck>EN</CodiceAck><CodiceErrore></CodiceErrore><MessaggioErrore></MessaggioErrore></MSA><RIC><GestioneRichiesta>0</GestioneRichiesta><ChiaveSGP>200600367775</ChiaveSGP><TipoRichiesta>CVARIE</TipoRichiesta><CodicePaziente>RSPGPP30B18C573F</CodicePaziente><Cognome>RASPONI</Cognome><Nome>GIUSEPPE</Nome><DataNascita>18/02/1930</DataNascita><Sesso>M</Sesso><ComuneNascita>040007</ComuneNascita><CodiceFiscale>RSPGPP30B18C573F</CodiceFiscale><TesseraSanitaria>9489344</TesseraSanitaria><ComuneResidenza>040007</ComuneResidenza><IndirizzoResidenza>VIA SAVIO 865</IndirizzoResidenza><CapResidenza>47023</CapResidenza><ComuneDomicilio></ComuneDomicilio><IndirizzoDomicilio></IndirizzoDomicilio><CapDomicilio></CapDomicilio><Cittadinanza>100</Cittadinanza><MedicoRichiedenteCodice>20065</MedicoRichiedenteCodice><MedicoRichiedenteTipo>01</MedicoRichiedenteTipo><MedicoRichiedenteCognomeNome>BUCCELLI</MedicoRichiedenteCognomeNome><MedicoRichiedenteCodiceFiscale>BCCGLC62S14C573J</MedicoRichiedenteCodiceFiscale><Impegnativa></Impegnativa><DataImpegnativa></DataImpegnativa><TipoTariffa>C_VARIE</TipoTariffa><ModalitaAccesso>01</ModalitaAccesso><Provenienza></Provenienza><Presidio></Presidio><Reparto></Reparto><CodiceRicovero></CodiceRicovero><Telefono1>330556</Telefono1><Telefono2></Telefono2><Telefono3></Telefono3><UslResidenza>080039</UslResidenza><StatoPaziente></StatoPaziente><QuesitoDiagnostico>N</QuesitoDiagnostico><StatoRichiesta>C</StatoRichiesta><Note></Note><DataOraRichiesta>24/07/2006 8.55.24</DataOraRichiesta><CodiceEsenzione></CodiceEsenzione><APP><UnitaErogatrice>DSPVACCPCE</UnitaErogatrice><DataOraAppuntamento>23/08/2006 8.30.00</DataOraAppuntamento><DataOraRegistrazione>24/07/2006 8.55.24</DataOraRegistrazione><PREST><CodicePrestazione>SSIPVA01</CodicePrestazione><DescrizionePrestazione>VACCINAZIONE ANTIDIFTOTETANICA</DescrizionePrestazione><CodiceProfilo></CodiceProfilo><DescrizioneProfilo></DescrizioneProfilo><Durata>10</Durata></PREST></APP></RIC><RIC><GestioneRichiesta>0</GestioneRichiesta><ChiaveSGP>200600375224</ChiaveSGP><TipoRichiesta>CVARIE</TipoRichiesta><CodicePaziente>CRMMRT65D67C573C</CodicePaziente><Cognome>CREMASCHI</Cognome><Nome>OMBRETTA</Nome><DataNascita>27/04/1965</DataNascita><Sesso>F</Sesso><ComuneNascita>040007</ComuneNascita><CodiceFiscale>CRMMRT65D67C573C</CodiceFiscale><TesseraSanitaria>9327240</TesseraSanitaria><ComuneResidenza>040007</ComuneResidenza><IndirizzoResidenza>VIA BARACCA FRANCESCO 192</IndirizzoResidenza><CapResidenza>47023</CapResidenza><ComuneDomicilio></ComuneDomicilio><IndirizzoDomicilio></IndirizzoDomicilio><CapDomicilio></CapDomicilio><Cittadinanza>100</Cittadinanza><MedicoRichiedenteCodice>6407</MedicoRichiedenteCodice><MedicoRichiedenteTipo>01</MedicoRichiedenteTipo><MedicoRichiedenteCognomeNome>TURCI</MedicoRichiedenteCognomeNome><MedicoRichiedenteCodiceFiscale>TRCGRL45S15C573A</MedicoRichiedenteCodiceFiscale><Impegnativa></Impegnativa><DataImpegnativa></DataImpegnativa><TipoTariffa>C_VARIE</TipoTariffa><ModalitaAccesso>01</ModalitaAccesso><Provenienza></Provenienza><Presidio></Presidio><Reparto></Reparto><CodiceRicovero></CodiceRicovero><Telefono1>0547 27461</Telefono1><Telefono2></Telefono2><Telefono3></Telefono3><UslResidenza>080039</UslResidenza><StatoPaziente></StatoPaziente><QuesitoDiagnostico>N</QuesitoDiagnostico><StatoRichiesta>C</StatoRichiesta><Note></Note><DataOraRichiesta>28/07/2006 7.47.36</DataOraRichiesta><CodiceEsenzione></CodiceEsenzione><APP><UnitaErogatrice>DSPVACCPCE</UnitaErogatrice><DataOraAppuntamento>23/08/2006 10.00.00</DataOraAppuntamento><DataOraRegistrazione>28/07/2006 7.47.36</DataOraRegistrazione><PREST><CodicePrestazione>SSIPVA01</CodicePrestazione><DescrizionePrestazione>VACCINAZIONE ANTIDIFTOTETANICA</DescrizionePrestazione><CodiceProfilo></CodiceProfilo><DescrizioneProfilo></DescrizioneProfilo><Durata>10</Durata></PREST></APP><APP><UnitaErogatrice>DSPVACCPCE</UnitaErogatrice><DataOraAppuntamento>23/08/2006 10.10.00</DataOraAppuntamento><DataOraRegistrazione>28/07/2006 7.47.36</DataOraRegistrazione><PREST><CodicePrestazione>SSIPVA03</CodicePrestazione><DescrizionePrestazione>VACCINAZIONE ANTIEPATITE 'B'</DescrizionePrestazione><CodiceProfilo></CodiceProfilo><DescrizioneProfilo></DescrizioneProfilo><Durata>10</Durata></PREST></APP></RIC><RIC><GestioneRichiesta>0</GestioneRichiesta><ChiaveSGP>200600376033</ChiaveSGP><TipoRichiesta>CVARIE</TipoRichiesta><CodicePaziente>BZZBBR69P64----E</CodicePaziente><Cognome>BAZZOCCHI</Cognome><Nome>BARBARA</Nome><DataNascita>24/09/1969</DataNascita><Sesso>F</Sesso><ComuneNascita>999236</ComuneNascita><CodiceFiscale>BZZBBR69P64Z130H</CodiceFiscale><TesseraSanitaria>9298470</TesseraSanitaria><ComuneResidenza>040007</ComuneResidenza><IndirizzoResidenza>VIA SAVIO 509</IndirizzoResidenza><CapResidenza>47023</CapResidenza><ComuneDomicilio></ComuneDomicilio><IndirizzoDomicilio>338-4156379</IndirizzoDomicilio><CapDomicilio></CapDomicilio><Cittadinanza>100</Cittadinanza><MedicoRichiedenteCodice>15305</MedicoRichiedenteCodice><MedicoRichiedenteTipo>01</MedicoRichiedenteTipo><MedicoRichiedenteCognomeNome>MINOTTI</MedicoRichiedenteCognomeNome><MedicoRichiedenteCodiceFiscale>MNTDNL53T51C573N</MedicoRichiedenteCodiceFiscale><Impegnativa></Impegnativa><DataImpegnativa></DataImpegnativa><TipoTariffa>C_VARIE</TipoTariffa><ModalitaAccesso>01</ModalitaAccesso><Provenienza></Provenienza><Presidio></Presidio><Reparto></Reparto><CodiceRicovero></CodiceRicovero><Telefono1>0547 601791</Telefono1><Telefono2>2645</Telefono2><Telefono3></Telefono3><UslResidenza>080039</UslResidenza><StatoPaziente></StatoPaziente><QuesitoDiagnostico>N</QuesitoDiagnostico><StatoRichiesta>C</StatoRichiesta><Note></Note><DataOraRichiesta>28/07/2006 11.57.32</DataOraRichiesta><CodiceEsenzione></CodiceEsenzione><APP><UnitaErogatrice>DSPVACCPCE</UnitaErogatrice><DataOraAppuntamento>23/08/2006 8.40.00</DataOraAppuntamento><DataOraRegistrazione>28/07/2006 11.57.32</DataOraRegistrazione><PREST><CodicePrestazione>SSIPVA01</CodicePrestazione><DescrizionePrestazione>VACCINAZIONE ANTIDIFTOTETANICA</DescrizionePrestazione><CodiceProfilo></CodiceProfilo><DescrizioneProfilo></DescrizioneProfilo><Durata>10</Durata></PREST></APP><APP><UnitaErogatrice>DSPVACCPCE</UnitaErogatrice><DataOraAppuntamento>23/08/2006 8.50.00</DataOraAppuntamento><DataOraRegistrazione>28/07/2006 11.57.32</DataOraRegistrazione><PREST><CodicePrestazione>SSIPVA02</CodicePrestazione><DescrizionePrestazione>VACCINAZIONE ANTIEPATITE 'A'</DescrizionePrestazione><CodiceProfilo></CodiceProfilo><DescrizioneProfilo></DescrizioneProfilo><Durata>10</Durata></PREST></APP></RIC><RIC><GestioneRichiesta>0</GestioneRichiesta><ChiaveSGP>200600381439</ChiaveSGP><TipoRichiesta>CVARIE</TipoRichiesta><CodicePaziente>CPSLSN64C150000E</CodicePaziente><Cognome>CAPASSO</Cognome><Nome>ALESSANDRO</Nome><DataNascita>15/03/1964</DataNascita><Sesso>M</Sesso><ComuneNascita>063049</ComuneNascita><CodiceFiscale>CPSLSN64C15F839Y</CodiceFiscale><TesseraSanitaria>9507457</TesseraSanitaria><ComuneResidenza>040007</ComuneResidenza><IndirizzoResidenza>VIA DE SICA VITTORIO 290</IndirizzoResidenza><CapResidenza>47023</CapResidenza><ComuneDomicilio></ComuneDomicilio><IndirizzoDomicilio></IndirizzoDomicilio><CapDomicilio></CapDomicilio><Cittadinanza>100</Cittadinanza><MedicoRichiedenteCodice>13502</MedicoRichiedenteCodice><MedicoRichiedenteTipo>01</MedicoRichiedenteTipo><MedicoRichiedenteCognomeNome>MARIANI</MedicoRichiedenteCognomeNome><MedicoRichiedenteCodiceFiscale>MRNPGR52R08C573R</MedicoRichiedenteCodiceFiscale><Impegnativa></Impegnativa><DataImpegnativa></DataImpegnativa><TipoTariffa>C_VARIE</TipoTariffa><ModalitaAccesso>01</ModalitaAccesso><Provenienza></Provenienza><Presidio></Presidio><Reparto></Reparto><CodiceRicovero></CodiceRicovero><Telefono1>0547 601180</Telefono1><Telefono2>INT. 2789</Telefono2><Telefono3>28675</Telefono3><UslResidenza>080039</UslResidenza><StatoPaziente></StatoPaziente><QuesitoDiagnostico>N</QuesitoDiagnostico><StatoRichiesta>C</StatoRichiesta><Note></Note><DataOraRichiesta>01/08/2006 12.14.18</DataOraRichiesta><CodiceEsenzione></CodiceEsenzione><APP><UnitaErogatrice>DSPVACCPCE</UnitaErogatrice><DataOraAppuntamento>23/08/2006 9.00.00</DataOraAppuntamento><DataOraRegistrazione>01/08/2006 12.14.18</DataOraRegistrazione><PREST><CodicePrestazione>SSIPVA01</CodicePrestazione><DescrizionePrestazione>VACCINAZIONE ANTIDIFTOTETANICA</DescrizionePrestazione><CodiceProfilo></CodiceProfilo><DescrizioneProfilo></DescrizioneProfilo><Durata>10</Durata></PREST></APP></RIC><RIC><GestioneRichiesta>0</GestioneRichiesta><ChiaveSGP>200600382686</ChiaveSGP><TipoRichiesta>CVARIE</TipoRichiesta><CodicePaziente>GRVDTL61A46C573F</CodicePaziente><Cognome>GARAVELLI</Cognome><Nome>DONATELLA</Nome><DataNascita>06/01/1961</DataNascita><Sesso>F</Sesso><ComuneNascita>040007</ComuneNascita><CodiceFiscale>GRVDTL61A46C573F</CodiceFiscale><TesseraSanitaria>9537262</TesseraSanitaria><ComuneResidenza>040007</ComuneResidenza><IndirizzoResidenza>VIA FORNACE MALTA, 63</IndirizzoResidenza><CapResidenza>47023</CapResidenza><ComuneDomicilio></ComuneDomicilio><IndirizzoDomicilio></IndirizzoDomicilio><CapDomicilio></CapDomicilio><Cittadinanza>100</Cittadinanza><MedicoRichiedenteCodice>13476</MedicoRichiedenteCodice><MedicoRichiedenteTipo>01</MedicoRichiedenteTipo><MedicoRichiedenteCognomeNome>PATRIGNANI</MedicoRichiedenteCognomeNome><MedicoRichiedenteCodiceFiscale>PTRMDL53P47H294A</MedicoRichiedenteCodiceFiscale><Impegnativa></Impegnativa><DataImpegnativa></DataImpegnativa><TipoTariffa>C_VARIE</TipoTariffa><ModalitaAccesso>01</ModalitaAccesso><Provenienza></Provenienza><Presidio></Presidio><Reparto></Reparto><CodiceRicovero></CodiceRicovero><Telefono1>347 8918932</Telefono1><Telefono2></Telefono2><Telefono3></Telefono3><UslResidenza>080112</UslResidenza><StatoPaziente></StatoPaziente><QuesitoDiagnostico>N</QuesitoDiagnostico><StatoRichiesta>C</StatoRichiesta><Note></Note><DataOraRichiesta>02/08/2006 8.33.58</DataOraRichiesta><CodiceEsenzione></CodiceEsenzione><APP><UnitaErogatrice>DSPVACCPCE</UnitaErogatrice><DataOraAppuntamento>23/08/2006 9.30.00</DataOraAppuntamento><DataOraRegistrazione>02/08/2006 8.33.58</DataOraRegistrazione><PREST><CodicePrestazione>SSIPVA01</CodicePrestazione><DescrizionePrestazione>VACCINAZIONE ANTIDIFTOTETANICA</DescrizionePrestazione><CodiceProfilo></CodiceProfilo><DescrizioneProfilo></DescrizioneProfilo><Durata>10</Durata></PREST></APP></RIC><RIC><GestioneRichiesta>0</GestioneRichiesta><ChiaveSGP>200600382758</ChiaveSGP><TipoRichiesta>CVARIE</TipoRichiesta><CodicePaziente>GNTFRZ58A28A809H</CodicePaziente><Cognome>GENTILI</Cognome><Nome>FABRIZIO</Nome><DataNascita>28/01/1958</DataNascita><Sesso>M</Sesso><ComuneNascita>040003</ComuneNascita><CodiceFiscale>GNTFRZ58A28A809H</CodiceFiscale><TesseraSanitaria>9368054</TesseraSanitaria><ComuneResidenza>040015</ComuneResidenza><IndirizzoResidenza>VIA MONTANARI 569 INT.4</IndirizzoResidenza><CapResidenza>47035</CapResidenza><ComuneDomicilio></ComuneDomicilio><IndirizzoDomicilio></IndirizzoDomicilio><CapDomicilio></CapDomicilio><Cittadinanza>100</Cittadinanza><MedicoRichiedenteCodice></MedicoRichiedenteCodice><MedicoRichiedenteTipo>00</MedicoRichiedenteTipo><MedicoRichiedenteCognomeNome></MedicoRichiedenteCognomeNome><MedicoRichiedenteCodiceFiscale></MedicoRichiedenteCodiceFiscale><Impegnativa></Impegnativa><DataImpegnativa></DataImpegnativa><TipoTariffa>C_VARIE</TipoTariffa><ModalitaAccesso>01</ModalitaAccesso><Provenienza></Provenienza><Presidio></Presidio><Reparto></Reparto><CodiceRicovero></CodiceRicovero><Telefono1>657799</Telefono1><Telefono2></Telefono2><Telefono3></Telefono3><UslResidenza>080039</UslResidenza><StatoPaziente></StatoPaziente><QuesitoDiagnostico>N</QuesitoDiagnostico><StatoRichiesta>C</StatoRichiesta><Note></Note><DataOraRichiesta>02/08/2006 8.56.05</DataOraRichiesta><CodiceEsenzione></CodiceEsenzione><APP><UnitaErogatrice>DSPVACCPCE</UnitaErogatrice><DataOraAppuntamento>23/08/2006 9.40.00</DataOraAppuntamento><DataOraRegistrazione>02/08/2006 8.56.05</DataOraRegistrazione><PREST><CodicePrestazione>SSIPVA01</CodicePrestazione><DescrizionePrestazione>VACCINAZIONE ANTIDIFTOTETANICA</DescrizionePrestazione><CodiceProfilo></CodiceProfilo><DescrizioneProfilo></DescrizioneProfilo><Durata>10</Durata></PREST></APP></RIC><RIC><GestioneRichiesta>0</GestioneRichiesta><ChiaveSGP>200600383734</ChiaveSGP><TipoRichiesta>CVARIE</TipoRichiesta><CodicePaziente>LNDNDR68H30----B</CodicePaziente><Cognome>LEANDRI</Cognome><Nome>ANDREA</Nome><DataNascita>30/06/1968</DataNascita><Sesso>M</Sesso><ComuneNascita>999241</ComuneNascita><CodiceFiscale>LNDNDR68H30Z133K</CodiceFiscale><TesseraSanitaria>9372006</TesseraSanitaria><ComuneResidenza>040007</ComuneResidenza><IndirizzoResidenza>VIA SAVIO N. 2415 INT. 2</IndirizzoResidenza><CapResidenza>47023</CapResidenza><ComuneDomicilio></ComuneDomicilio><IndirizzoDomicilio></IndirizzoDomicilio><CapDomicilio></CapDomicilio><Cittadinanza>100</Cittadinanza><MedicoRichiedenteCodice>15165</MedicoRichiedenteCodice><MedicoRichiedenteTipo>01</MedicoRichiedenteTipo><MedicoRichiedenteCognomeNome>GRIFFI</MedicoRichiedenteCognomeNome><MedicoRichiedenteCodiceFiscale>GRFPPN54B02I444Y</MedicoRichiedenteCodiceFiscale><Impegnativa></Impegnativa><DataImpegnativa></DataImpegnativa><TipoTariffa>C_VARIE</TipoTariffa><ModalitaAccesso>01</ModalitaAccesso><Provenienza></Provenienza><Presidio></Presidio><Reparto></Reparto><CodiceRicovero></CodiceRicovero><Telefono1></Telefono1><Telefono2>334036</Telefono2><Telefono3></Telefono3><UslResidenza>080039</UslResidenza><StatoPaziente></StatoPaziente><QuesitoDiagnostico>N</QuesitoDiagnostico><StatoRichiesta>C</StatoRichiesta><Note></Note><DataOraRichiesta>02/08/2006 17.31.38</DataOraRichiesta><CodiceEsenzione></CodiceEsenzione><APP><UnitaErogatrice>DSPVACCPCE</UnitaErogatrice><DataOraAppuntamento>23/08/2006 10.30.00</DataOraAppuntamento><DataOraRegistrazione>02/08/2006 17.31.38</DataOraRegistrazione><PREST><CodicePrestazione>SSIPVA01</CodicePrestazione><DescrizionePrestazione>VACCINAZIONE ANTIDIFTOTETANICA</DescrizionePrestazione><CodiceProfilo></CodiceProfilo><DescrizioneProfilo></DescrizioneProfilo><Durata>10</Durata></PREST></APP></RIC><RIC><GestioneRichiesta>0</GestioneRichiesta><ChiaveSGP>200600385016</ChiaveSGP><TipoRichiesta>CVARIE</TipoRichiesta><CodicePaziente>CHFSMN73R16C573R</CodicePaziente><Cognome>CHIFARI</Cognome><Nome>SIMONE</Nome><DataNascita>16/10/1973</DataNascita><Sesso>M</Sesso><ComuneNascita>040007</ComuneNascita><CodiceFiscale>CHFSMN73R16C573R</CodiceFiscale><TesseraSanitaria>9317519</TesseraSanitaria><ComuneResidenza>040007</ComuneResidenza><IndirizzoResidenza>VIA MANCINI URBANO LIDIO 75</IndirizzoResidenza><CapResidenza>47023</CapResidenza><ComuneDomicilio></ComuneDomicilio><IndirizzoDomicilio></IndirizzoDomicilio><CapDomicilio></CapDomicilio><Cittadinanza>100</Cittadinanza><MedicoRichiedenteCodice></MedicoRichiedenteCodice><MedicoRichiedenteTipo>00</MedicoRichiedenteTipo><MedicoRichiedenteCognomeNome></MedicoRichiedenteCognomeNome><MedicoRichiedenteCodiceFiscale></MedicoRichiedenteCodiceFiscale><Impegnativa></Impegnativa><DataImpegnativa></DataImpegnativa><TipoTariffa>C_VARIE</TipoTariffa><ModalitaAccesso>01</ModalitaAccesso><Provenienza></Provenienza><Presidio></Presidio><Reparto></Reparto><CodiceRicovero></CodiceRicovero><Telefono1>0547 380450</Telefono1><Telefono2></Telefono2><Telefono3></Telefono3><UslResidenza>080039</UslResidenza><StatoPaziente></StatoPaziente><QuesitoDiagnostico>N</QuesitoDiagnostico><StatoRichiesta>C</StatoRichiesta><Note></Note><DataOraRichiesta>03/08/2006 10.50.20</DataOraRichiesta><CodiceEsenzione></CodiceEsenzione><APP><UnitaErogatrice>DSPVACCPCE</UnitaErogatrice><DataOraAppuntamento>23/08/2006 10.50.00</DataOraAppuntamento><DataOraRegistrazione>03/08/2006 10.50.20</DataOraRegistrazione><PREST><CodicePrestazione>SSIPVA01</CodicePrestazione><DescrizionePrestazione>VACCINAZIONE ANTIDIFTOTETANICA</DescrizionePrestazione><CodiceProfilo></CodiceProfilo><DescrizioneProfilo></DescrizioneProfilo><Durata>10</Durata></PREST></APP></RIC><RIC><GestioneRichiesta>0</GestioneRichiesta><ChiaveSGP>200600385187</ChiaveSGP><TipoRichiesta>CVARIE</TipoRichiesta><CodicePaziente>MRCCST68A70C573E</CodicePaziente><Cognome>MARCHIONNI</Cognome><Nome>CRISTINA</Nome><DataNascita>30/01/1968</DataNascita><Sesso>F</Sesso><ComuneNascita>040007</ComuneNascita><CodiceFiscale>MRCCST68A70C573E</CodiceFiscale><TesseraSanitaria>9392022</TesseraSanitaria><ComuneResidenza>040007</ComuneResidenza><IndirizzoResidenza>VIA MAGELLANO FERDINANDO N. 216</IndirizzoResidenza><CapResidenza>47023</CapResidenza><ComuneDomicilio></ComuneDomicilio><IndirizzoDomicilio></IndirizzoDomicilio><CapDomicilio></CapDomicilio><Cittadinanza>100</Cittadinanza><MedicoRichiedenteCodice>20205</MedicoRichiedenteCodice><MedicoRichiedenteTipo>01</MedicoRichiedenteTipo><MedicoRichiedenteCognomeNome>CAPELLI</MedicoRichiedenteCognomeNome><MedicoRichiedenteCodiceFiscale>CPLGRN56E02D935U</MedicoRichiedenteCodiceFiscale><Impegnativa></Impegnativa><DataImpegnativa></DataImpegnativa><TipoTariffa>C_VARIE</TipoTariffa><ModalitaAccesso>01</ModalitaAccesso><Provenienza></Provenienza><Presidio></Presidio><Reparto></Reparto><CodiceRicovero></CodiceRicovero><Telefono1></Telefono1><Telefono2>338 3344927</Telefono2><Telefono3></Telefono3><UslResidenza>080112</UslResidenza><StatoPaziente></StatoPaziente><QuesitoDiagnostico>N</QuesitoDiagnostico><StatoRichiesta>C</StatoRichiesta><Note></Note><DataOraRichiesta>03/08/2006 11.40.32</DataOraRichiesta><CodiceEsenzione></CodiceEsenzione><APP><UnitaErogatrice>DSPVACCPCE</UnitaErogatrice><DataOraAppuntamento>23/08/2006 11.00.00</DataOraAppuntamento><DataOraRegistrazione>03/08/2006 11.40.32</DataOraRegistrazione><PREST><CodicePrestazione>SSIPVA03</CodicePrestazione><DescrizionePrestazione>VACCINAZIONE ANTIEPATITE 'B'</DescrizionePrestazione><CodiceProfilo></CodiceProfilo><DescrizioneProfilo></DescrizioneProfilo><Durata>10</Durata></PREST></APP></RIC><RIC><GestioneRichiesta>0</GestioneRichiesta><ChiaveSGP>200600385194</ChiaveSGP><TipoRichiesta>CVARIE</TipoRichiesta><CodicePaziente>CMPTTR36L16C573C</CodicePaziente><Cognome>CAMPANA</Cognome><Nome>ETTORE</Nome><DataNascita>16/07/1936</DataNascita><Sesso>M</Sesso><ComuneNascita>040007</ComuneNascita><CodiceFiscale>CMPTTR36L16C573C</CodiceFiscale><TesseraSanitaria>0657421</TesseraSanitaria><ComuneResidenza>040007</ComuneResidenza><IndirizzoResidenza>VIA ANCONA 1A N. 290</IndirizzoResidenza><CapResidenza>47023</CapResidenza><ComuneDomicilio>040007</ComuneDomicilio><IndirizzoDomicilio>VIA ANCONA 290</IndirizzoDomicilio><CapDomicilio>47023</CapDomicilio><Cittadinanza>100</Cittadinanza><MedicoRichiedenteCodice>18111</MedicoRichiedenteCodice><MedicoRichiedenteTipo>01</MedicoRichiedenteTipo><MedicoRichiedenteCognomeNome>FIORAVANTI</MedicoRichiedenteCognomeNome><MedicoRichiedenteCodiceFiscale>FRVFNC57S11C573Z</MedicoRichiedenteCodiceFiscale><Impegnativa></Impegnativa><DataImpegnativa></DataImpegnativa><TipoTariffa>C_VARIE</TipoTariffa><ModalitaAccesso>01</ModalitaAccesso><Provenienza></Provenienza><Presidio></Presidio><Reparto></Reparto><CodiceRicovero></CodiceRicovero><Telefono1>661500</Telefono1><Telefono2>334901 CASA PROT</Telefono2><Telefono3></Telefono3><UslResidenza>080111</UslResidenza><StatoPaziente></StatoPaziente><QuesitoDiagnostico>N</QuesitoDiagnostico><StatoRichiesta>C</StatoRichiesta><Note></Note><DataOraRichiesta>03/08/2006 11.42.48</DataOraRichiesta><CodiceEsenzione></CodiceEsenzione><APP><UnitaErogatrice>DSPVACCPCE</UnitaErogatrice><DataOraAppuntamento>23/08/2006 11.20.00</DataOraAppuntamento><DataOraRegistrazione>03/08/2006 11.42.48</DataOraRegistrazione><PREST><CodicePrestazione>SSIPVA01</CodicePrestazione><DescrizionePrestazione>VACCINAZIONE ANTIDIFTOTETANICA</DescrizionePrestazione><CodiceProfilo></CodiceProfilo><DescrizioneProfilo></DescrizioneProfilo><Durata>10</Durata></PREST></APP></RIC><RIC><GestioneRichiesta>0</GestioneRichiesta><ChiaveSGP>200600385339</ChiaveSGP><TipoRichiesta>CVARIE</TipoRichiesta><CodicePaziente>GNNCLS30A20F139I</CodicePaziente><Cognome>GIANNINI</Cognome><Nome>CELSO</Nome><DataNascita>20/01/1930</DataNascita><Sesso>M</Sesso><ComuneNascita>040020</ComuneNascita><CodiceFiscale>GNNCLS30A20F139I</CodiceFiscale><TesseraSanitaria>9367654</TesseraSanitaria><ComuneResidenza>040007</ComuneResidenza><IndirizzoResidenza>VIA MANCINI URBANO LIDIO 96</IndirizzoResidenza><CapResidenza>47023</CapResidenza><ComuneDomicilio></ComuneDomicilio><IndirizzoDomicilio></IndirizzoDomicilio><CapDomicilio></CapDomicilio><Cittadinanza>100</Cittadinanza><MedicoRichiedenteCodice>13473</MedicoRichiedenteCodice><MedicoRichiedenteTipo>01</MedicoRichiedenteTipo><MedicoRichiedenteCognomeNome>MICUCCI</MedicoRichiedenteCognomeNome><MedicoRichiedenteCodiceFiscale>MCCRNN53C01L750A</MedicoRichiedenteCodiceFiscale><Impegnativa></Impegnativa><DataImpegnativa></DataImpegnativa><TipoTariffa>C_VARIE</TipoTariffa><ModalitaAccesso>01</ModalitaAccesso><Provenienza></Provenienza><Presidio></Presidio><Reparto></Reparto><CodiceRicovero></CodiceRicovero><Telefono1>382518</Telefono1><Telefono2></Telefono2><Telefono3></Telefono3><UslResidenza>080039</UslResidenza><StatoPaziente></StatoPaziente><QuesitoDiagnostico>N</QuesitoDiagnostico><StatoRichiesta>C</StatoRichiesta><Note></Note><DataOraRichiesta>03/08/2006 12.45.15</DataOraRichiesta><CodiceEsenzione></CodiceEsenzione><APP><UnitaErogatrice>DSPVACCPCE</UnitaErogatrice><DataOraAppuntamento>23/08/2006 11.10.00</DataOraAppuntamento><DataOraRegistrazione>03/08/2006 12.45.15</DataOraRegistrazione><PREST><CodicePrestazione>SSIPVA01</CodicePrestazione><DescrizionePrestazione>VACCINAZIONE ANTIDIFTOTETANICA</DescrizionePrestazione><CodiceProfilo></CodiceProfilo><DescrizioneProfilo></DescrizioneProfilo><Durata>10</Durata></PREST></APP></RIC><RIC><GestioneRichiesta>0</GestioneRichiesta><ChiaveSGP>200600388892</ChiaveSGP><TipoRichiesta>CVARIE</TipoRichiesta><CodicePaziente>VCNNMR83P62C573P</CodicePaziente><Cognome>VICINI</Cognome><Nome>ANNAMARIA</Nome><DataNascita>22/09/1983</DataNascita><Sesso>F</Sesso><ComuneNascita>040007</ComuneNascita><CodiceFiscale>VCNNMR83P62C573P</CodiceFiscale><TesseraSanitaria>9492091</TesseraSanitaria><ComuneResidenza>040007</ComuneResidenza><IndirizzoResidenza>VIA PIAVE 151</IndirizzoResidenza><CapResidenza>47023</CapResidenza><ComuneDomicilio></ComuneDomicilio><IndirizzoDomicilio></IndirizzoDomicilio><CapDomicilio></CapDomicilio><Cittadinanza>100</Cittadinanza><MedicoRichiedenteCodice>19313</MedicoRichiedenteCodice><MedicoRichiedenteTipo>01</MedicoRichiedenteTipo><MedicoRichiedenteCognomeNome>BERANKOVA</MedicoRichiedenteCognomeNome><MedicoRichiedenteCodiceFiscale>BRNLGO54R58Z105I</MedicoRichiedenteCodiceFiscale><Impegnativa></Impegnativa><DataImpegnativa></DataImpegnativa><TipoTariffa>C_VARIE</TipoTariffa><ModalitaAccesso>01</ModalitaAccesso><Provenienza></Provenienza><Presidio></Presidio><Reparto></Reparto><CodiceRicovero></CodiceRicovero><Telefono1>0547 29420</Telefono1><Telefono2></Telefono2><Telefono3></Telefono3><UslResidenza>080039</UslResidenza><StatoPaziente></StatoPaziente><QuesitoDiagnostico>N</QuesitoDiagnostico><StatoRichiesta>C</StatoRichiesta><Note></Note><DataOraRichiesta>05/08/2006 11.38.09</DataOraRichiesta><CodiceEsenzione></CodiceEsenzione><APP><UnitaErogatrice>DSPVACCPCE</UnitaErogatrice><DataOraAppuntamento>23/08/2006 10.20.00</DataOraAppuntamento><DataOraRegistrazione>05/08/2006 11.38.09</DataOraRegistrazione><PREST><CodicePrestazione>SSIPVA01</CodicePrestazione><DescrizionePrestazione>VACCINAZIONE ANTIDIFTOTETANICA</DescrizionePrestazione><CodiceProfilo></CodiceProfilo><DescrizioneProfilo></DescrizioneProfilo><Durata>10</Durata></PREST></APP></RIC><RIC><GestioneRichiesta>0</GestioneRichiesta><ChiaveSGP>200600412983</ChiaveSGP><TipoRichiesta>CVARIE</TipoRichiesta><CodicePaziente>DLCDRT49R620000O</CodicePaziente><Cognome>DOLCINI</Cognome><Nome>DOROTEA</Nome><DataNascita>22/10/1949</DataNascita><Sesso>F</Sesso><ComuneNascita>037006</ComuneNascita><CodiceFiscale>DLCDRT49R62A944U</CodiceFiscale><TesseraSanitaria>9341973</TesseraSanitaria><ComuneResidenza>040007</ComuneResidenza><IndirizzoResidenza>VIA DON GIOVANNI MINZONI 413</IndirizzoResidenza><CapResidenza>47023</CapResidenza><ComuneDomicilio></ComuneDomicilio><IndirizzoDomicilio></IndirizzoDomicilio><CapDomicilio></CapDomicilio><Cittadinanza>100</Cittadinanza><MedicoRichiedenteCodice>18111</MedicoRichiedenteCodice><MedicoRichiedenteTipo>01</MedicoRichiedenteTipo><MedicoRichiedenteCognomeNome>FIORAVANTI</MedicoRichiedenteCognomeNome><MedicoRichiedenteCodiceFiscale>FRVFNC57S11C573Z</MedicoRichiedenteCodiceFiscale><Impegnativa></Impegnativa><DataImpegnativa></DataImpegnativa><TipoTariffa>C_VARIE</TipoTariffa><ModalitaAccesso>01</ModalitaAccesso><Provenienza></Provenienza><Presidio></Presidio><Reparto></Reparto><CodiceRicovero></CodiceRicovero><Telefono1>0547 611561</Telefono1><Telefono2></Telefono2><Telefono3></Telefono3><UslResidenza>080039</UslResidenza><StatoPaziente></StatoPaziente><QuesitoDiagnostico>N</QuesitoDiagnostico><StatoRichiesta>C</StatoRichiesta><Note></Note><DataOraRichiesta>22/08/2006 9.24.18</DataOraRichiesta><CodiceEsenzione></CodiceEsenzione><APP><UnitaErogatrice>DSPVACCPCE</UnitaErogatrice><DataOraAppuntamento>23/08/2006 9.10.00</DataOraAppuntamento><DataOraRegistrazione>22/08/2006 9.24.18</DataOraRegistrazione><PREST><CodicePrestazione>SSIPVA01</CodicePrestazione><DescrizionePrestazione>VACCINAZIONE ANTIDIFTOTETANICA</DescrizionePrestazione><CodiceProfilo></CodiceProfilo><DescrizioneProfilo></DescrizioneProfilo><Durata>10</Durata></PREST></APP></RIC><RIC><GestioneRichiesta>0</GestioneRichiesta><ChiaveSGP>200600413144</ChiaveSGP><TipoRichiesta>CVARIE</TipoRichiesta><CodicePaziente>09530582</CodicePaziente><Cognome>GIOZZET</Cognome><Nome>PATRICIA</Nome><DataNascita>07/03/1967</DataNascita><Sesso>F</Sesso><ComuneNascita>999215</ComuneNascita><CodiceFiscale>GZZPRC67C47Z110H</CodiceFiscale><TesseraSanitaria>9530582</TesseraSanitaria><ComuneResidenza>040007</ComuneResidenza><IndirizzoResidenza>VIA DEL PRIOLO 810</IndirizzoResidenza><CapResidenza>47023</CapResidenza><ComuneDomicilio></ComuneDomicilio><IndirizzoDomicilio></IndirizzoDomicilio><CapDomicilio></CapDomicilio><Cittadinanza>100</Cittadinanza><MedicoRichiedenteCodice>18535</MedicoRichiedenteCodice><MedicoRichiedenteTipo>01</MedicoRichiedenteTipo><MedicoRichiedenteCognomeNome>MAMBELLI</MedicoRichiedenteCognomeNome><MedicoRichiedenteCodiceFiscale>MMBMLS53B61C573L</MedicoRichiedenteCodiceFiscale><Impegnativa></Impegnativa><DataImpegnativa></DataImpegnativa><TipoTariffa>C_VARIE</TipoTariffa><ModalitaAccesso>01</ModalitaAccesso><Provenienza></Provenienza><Presidio></Presidio><Reparto></Reparto><CodiceRicovero></CodiceRicovero><Telefono1>0547 661456</Telefono1><Telefono2></Telefono2><Telefono3></Telefono3><UslResidenza>080112</UslResidenza><StatoPaziente></StatoPaziente><QuesitoDiagnostico>N</QuesitoDiagnostico><StatoRichiesta>C</StatoRichiesta><Note></Note><DataOraRichiesta>22/08/2006 10.09.59</DataOraRichiesta><CodiceEsenzione></CodiceEsenzione><APP><UnitaErogatrice>DSPVACCPCE</UnitaErogatrice><DataOraAppuntamento>23/08/2006 9.50.00</DataOraAppuntamento><DataOraRegistrazione>22/08/2006 10.09.59</DataOraRegistrazione><PREST><CodicePrestazione>SSIPVA01</CodicePrestazione><DescrizionePrestazione>VACCINAZIONE ANTIDIFTOTETANICA</DescrizionePrestazione><CodiceProfilo></CodiceProfilo><DescrizioneProfilo></DescrizioneProfilo><Durata>10</Durata></PREST></APP></RIC><RIC><GestioneRichiesta>0</GestioneRichiesta><ChiaveSGP>200600413542</ChiaveSGP><TipoRichiesta>CVARIE</TipoRichiesta><CodicePaziente>SVRNSC79C56C573S</CodicePaziente><Cognome>SEVERI</Cognome><Nome>NATASCIA</Nome><DataNascita>16/03/1979</DataNascita><Sesso>F</Sesso><ComuneNascita>040007</ComuneNascita><CodiceFiscale>SVRNSC79C56C573S</CodiceFiscale><TesseraSanitaria>9432187</TesseraSanitaria><ComuneResidenza>040007</ComuneResidenza><IndirizzoResidenza>VIA MARIANA 76</IndirizzoResidenza><CapResidenza>47023</CapResidenza><ComuneDomicilio></ComuneDomicilio><IndirizzoDomicilio></IndirizzoDomicilio><CapDomicilio></CapDomicilio><Cittadinanza>100</Cittadinanza><MedicoRichiedenteCodice>15783</MedicoRichiedenteCodice><MedicoRichiedenteTipo>01</MedicoRichiedenteTipo><MedicoRichiedenteCognomeNome>MERAVIGLI</MedicoRichiedenteCognomeNome><MedicoRichiedenteCodiceFiscale>MRVVCN55S13C573V</MedicoRichiedenteCodiceFiscale><Impegnativa></Impegnativa><DataImpegnativa></DataImpegnativa><TipoTariffa>C_VARIE</TipoTariffa><ModalitaAccesso>01</ModalitaAccesso><Provenienza></Provenienza><Presidio></Presidio><Reparto></Reparto><CodiceRicovero></CodiceRicovero><Telefono1>0547 632750</Telefono1><Telefono2></Telefono2><Telefono3></Telefono3><UslResidenza>080039</UslResidenza><StatoPaziente></StatoPaziente><QuesitoDiagnostico>N</QuesitoDiagnostico><StatoRichiesta>C</StatoRichiesta><Note></Note><DataOraRichiesta>22/08/2006 11.51.19</DataOraRichiesta><CodiceEsenzione></CodiceEsenzione><APP><UnitaErogatrice>DSPVACCPCE</UnitaErogatrice><DataOraAppuntamento>23/08/2006 9.20.00</DataOraAppuntamento><DataOraRegistrazione>22/08/2006 11.51.19</DataOraRegistrazione><PREST><CodicePrestazione>SSIPVA01</CodicePrestazione><DescrizionePrestazione>VACCINAZIONE ANTIDIFTOTETANICA</DescrizionePrestazione><CodiceProfilo></CodiceProfilo><DescrizioneProfilo></DescrizioneProfilo><Durata>10</Durata></PREST></APP></RIC><RIC><GestioneRichiesta>0</GestioneRichiesta><ChiaveSGP>200600415354</ChiaveSGP><TipoRichiesta>CVARIE</TipoRichiesta><CodicePaziente>SNTMLN71L19C573L</CodicePaziente><Cognome>SINTUZZI</Cognome><Nome>EMILIANO</Nome><DataNascita>19/07/1971</DataNascita><Sesso>M</Sesso><ComuneNascita>040007</ComuneNascita><CodiceFiscale>SNTMLN71L19C573L</CodiceFiscale><TesseraSanitaria>9435504</TesseraSanitaria><ComuneResidenza>040007</ComuneResidenza><IndirizzoResidenza>CSO COMANDINI UBALDO 18</IndirizzoResidenza><CapResidenza>47023</CapResidenza><ComuneDomicilio></ComuneDomicilio><IndirizzoDomicilio></IndirizzoDomicilio><CapDomicilio></CapDomicilio><Cittadinanza>100</Cittadinanza><MedicoRichiedenteCodice>19686</MedicoRichiedenteCodice><MedicoRichiedenteTipo>01</MedicoRichiedenteTipo><MedicoRichiedenteCognomeNome>NERI</MedicoRichiedenteCognomeNome><MedicoRichiedenteCodiceFiscale>NREDVD56A11H542I</MedicoRichiedenteCodiceFiscale><Impegnativa></Impegnativa><DataImpegnativa></DataImpegnativa><TipoTariffa>C_VARIE</TipoTariffa><ModalitaAccesso>01</ModalitaAccesso><Provenienza></Provenienza><Presidio></Presidio><Reparto></Reparto><CodiceRicovero></CodiceRicovero><Telefono1>0547 23569</Telefono1><Telefono2></Telefono2><Telefono3>328 5436658</Telefono3><UslResidenza>080112</UslResidenza><StatoPaziente></StatoPaziente><QuesitoDiagnostico>N</QuesitoDiagnostico><StatoRichiesta>C</StatoRichiesta><Note></Note><DataOraRichiesta>23/08/2006 10.28.22</DataOraRichiesta><CodiceEsenzione></CodiceEsenzione><APP><UnitaErogatrice>DSPVACCPCE</UnitaErogatrice><DataOraAppuntamento>23/08/2006 10.40.00</DataOraAppuntamento><DataOraRegistrazione>23/08/2006 10.28.22</DataOraRegistrazione><PREST><CodicePrestazione>SSIPVA07</CodicePrestazione><DescrizionePrestazione>VACCINAZIONE ANTIEPATITE A - B</DescrizionePrestazione><CodiceProfilo></CodiceProfilo><DescrizioneProfilo></DescrizioneProfilo><Durata>10</Durata></PREST></APP></RIC></XmlOutPianiDiLavoro>")
                ''
                '' End If


                'Gestione del messaggio XML di ritorno
                If stbXmlOut.Length > 0 Then

                    'Passaggio dei dati dal formato "stringa XML" al formato dataset
                    ds = New System.Data.DataSet
                    Dim strXml As New System.IO.StringReader(stbXmlOut.ToString)
                    ds.ReadXml(strXml)

                    'Controllo di eventuali segnalazioni di errore dal web service
                    If ds.Tables.Count > 1 Then
                        If ds.Tables("MSA").Rows(0)("CodiceErrore") = "" Then 'Nessun errore riscontrato

                            'Selezione delle richieste presenti
                            If ds.Tables.Count > 2 Then
                                Dim i As Integer
                                Dim drNewRow, dtrApp, dtrPre As DataRow

                                'Loop sulle richieste per la valorizzazione del datatable DtsAppPre
                                'da passare al datagrid gerarchico
                                For i = 0 To ds.Tables("RIC").Rows.Count - 1

                                    For Each dtrApp In ds.Tables("RIC").Rows(i).GetChildRows("RIC_APP")

                                        ' --------- Inserimento appuntamento --------- '
                                        With ds.Tables("RIC")
                                            drNewRow = dtsAppPre.Tables("Appuntamenti").NewRow
                                            drNewRow("NumeroRichiesta") = .Rows(i)("ChiaveSGP")

                                            drNewRow("Data") = Format(CType(dtrApp("DataOraAppuntamento"), DateTime), "dd/MM/yyyy")
                                            drNewRow("Ora") = Format(CType(dtrApp("DataOraAppuntamento"), DateTime), "H:mm")
                                            drNewRow("CodicePazienteAusiliario") = .Rows(i)("CodicePaziente")
                                            drNewRow("Cognome") = .Rows(i)("Cognome") + " " + .Rows(i)("Nome")
                                            drNewRow("Nome") = .Rows(i)("Nome")
                                            drNewRow("ComuneResidenza") = .Rows(i)("ComuneResidenza")
                                            drNewRow("CapResidenza") = .Rows(i)("CapResidenza")
                                            If Not IsDBNull(.Rows(i)("DataNascita")) Then
                                                drNewRow("DataNascita") = .Rows(i)("DataNascita")
                                            End If
                                            drNewRow("TipoAgenda") = "Cup"
                                            drNewRow("TipoRichiesta") = .Rows(i)("TipoRichiesta")

                                        End With

                                        ' --------- Inserimento dati appuntamento --------- '
                                        dtsAppPre.Tables("Appuntamenti").Rows.Add(drNewRow)

                                        ' --------- Inserimento prestazioni appuntamento --------- '
                                        For Each dtrPre In dtrApp.GetChildRows("APP_PREST")
                                            drNewRow = dtsAppPre.Tables("Prestazioni").NewRow
                                            drNewRow("NumeroRichiesta") = ds.Tables("RIC").Rows(i)("ChiaveSGP")

                                            drNewRow("PrestazioneMnemonico") = dtrPre("CodicePrestazione")
                                            drNewRow("PrestazioneDescrizione") = dtrPre("DescrizionePrestazione")
                                            If dtrPre("CodiceProfilo") <> dtrPre("CodicePrestazione") Then
                                                drNewRow("ProfiloMnemonico") = dtrPre("CodiceProfilo")
                                                drNewRow("ProfiloDescrizione") = dtrPre("DescrizioneProfilo")
                                            End If

                                            dtsAppPre.Tables("Prestazioni").Rows.Add(drNewRow)
                                        Next ' For Each dtrPre 

                                    Next ' For Each dtrApp
                                Next ' For i = 0 To ds.Tables("RIC").Rows.Count - 1
                            End If ' ds.Tables.Count > 2

                        Else 'Segnalazione di errore da parte del web service
                            stbErr.AppendLine("Errore in fase di selezione degli appuntamenti dalle agende Cup.")
                            stbErr.AppendLine("Contattare l\'amministratore del programma.")
                            stbErr.AppendLine("Segue la descrizione del problema segnalata dal WebService:")
                            stbErr.Append(Me.ApplyEscapeJS(ds.Tables("MSA").Rows(0)("MessaggioErrore")))
                        End If ' if ds.Tables("MSA").Rows(0)("CodiceErrore") = ""
                    End If ' if ds.Tables.Count > 1
                Else
                    stbErr.AppendLine("Errore in fase di selezione degli appuntamenti dalle agende Cup.")
                    stbErr.AppendLine("Contattare l\'amministratore del programma.")
                    stbErr.AppendLine("Segue descrizione del problema:")
                    stbErr.Append("Nessun messaggio di ritorno dal WebService")
                End If ' if stbXmlOut.Length > 0

            Next

        Catch exc As Exception
            stbErr.AppendLine("Errore in fase di selezione degli appuntamenti dalle agende Cup.")
            stbErr.AppendLine("Contattare l\'amministratore del programma.")
            stbErr.AppendLine("Segue descrizione del problema:")
            stbErr.Append(Me.ApplyEscapeJS(exc.Message.Substring(0, exc.Message.Length - 1)))
        End Try

    End Sub

End Class
