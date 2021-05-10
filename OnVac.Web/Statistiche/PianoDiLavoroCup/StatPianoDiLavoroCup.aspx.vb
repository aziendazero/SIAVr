Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager


Partial Class StatPianoDiLavoroCup
    Inherits OnVac.Common.PageBase


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

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then
            PanelTitolo.InnerText = OnVacUtility.Variabili.CNS.Descrizione

			ucSelezioneConsultori.MostraSoloAperti = False
			ucSelezioneConsultori.ImpostaCnsCorrente = True
			ucSelezioneConsultori.LoadGetCodici()

			ShowPrintButtons()
        End If

        'per la stampa tramite tasto invio (modifica 29/07/2004)
        Select Case Request.Form("__EVENTTARGET")
            Case "Stampa"
                If Toolbar.Items.FromKeyButton("btnStampa").Visible Then
                    Stampa()
                End If
        End Select

    End Sub

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()

        listPrintButtons.Add(New PrintButton(Constants.ReportName.PianoDiLavoroCup, "btnStampa"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Toolbar)

    End Sub

    Private Sub Toolbar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked

        Select Case e.Button.Key
            Case "btnStampa"
                Stampa()
        End Select

    End Sub

    Private Sub Stampa()

        Dim strFiltro As String = ""
        Dim rpt As New ReportParameter

        Dim dsNonVaccinati As New System.Data.DataSet
        Dim dsNonPrenotati As New System.Data.DataSet
        Dim dsPrenotatiVaccinati As New System.Data.DataSet

        Dim selAccesso As New System.Text.StringBuilder
        For i As Integer = 0 To chklModAccesso.Items.Count - 1
            If chklModAccesso.Items(i).Selected = True Then
                selAccesso.Append(chklModAccesso.Items(i).Text & ",")
            End If
        Next
        If selAccesso.Length > 0 Then
            selAccesso.Remove(selAccesso.Length - 1, 1)
        Else
            For i As Integer = 0 To chklModAccesso.Items.Count - 1
                selAccesso.Append(chklModAccesso.Items(i).Text & ",")
            Next
            selAccesso.Remove(selAccesso.Length - 1, 1)
        End If

        Dim dtOnvac As DataTable = pazientiVaccinati()

        Dim err As New System.Text.StringBuilder()

        ' Recupero prenotazioni da cup. Se il consultorio selezionato non ha agende cup, 
        ' non effettua la stampa, che andrebbe in errore, ma visualizza un messaggio
        Dim dsTemp As System.Data.DataSet = pazientiPrenotati(err)
        If dsTemp Is Nothing Then
            OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(err.ToString(), "errUniEro", False, False))
            Return
        End If

        ' Datatable prenotazioni cup
        Dim dtCup As DataTable = dsTemp.Tables(0)

        dsNonVaccinati = pazientiPrenotati_nonVaccinati(dtOnvac, dtCup, err.ToString)
        dsNonPrenotati = pazientiVaccinati_nonPrenotati(dtOnvac, dtCup, err.ToString)
        dsPrenotatiVaccinati = pazientiPrenotati_Vaccinati(dtOnvac, dtCup, err.ToString)

        'se è un sottoreport esterno deve essere passata anche l'estensione del file
        '---CMR 31/05/2007--- nel passaggio a Crystal Report XI bisogna passare il datasource anche a tutte le istanze dei sottoreport
        rpt.set_dataset_subreport(dsNonVaccinati, "SottoreportPrenotatiNonVaccinati.rpt")
        rpt.set_dataset_subreport(dsNonPrenotati, "SottoreportVaccinatiNonPrenotati.rpt")
        rpt.set_dataset_subreport(dsNonPrenotati, "SottoreportVaccinatiNonPrenotati.rpt - 01")
        rpt.set_dataset_subreport(dsPrenotatiVaccinati, "SottoreportPrenotatiVaccinati.rpt")

		rpt.AddParameter("CNS", IIf(ucSelezioneConsultori.GetConsultoriSelezionati.Count > 0, ucSelezioneConsultori.GetDescrizioneConsultoriSelezionatiPerStampe(), ""))
		rpt.AddParameter("DataEffettuazioneIniz", odpDataEffettuazioneIniz.Text)
        rpt.AddParameter("DataEffettuazioneFin", odpDataEffettuazioneFin.Text)
        rpt.AddParameter("DataNascitaDa", IIf(odpDataNascitaDa.Text <> "", odpDataNascitaDa.Text, ""))
        rpt.AddParameter("DataNascitaA", IIf(odpDataNascitaA.Text <> "", odpDataNascitaA.Text, ""))
        rpt.AddParameter("Accesso", selAccesso.ToString)
        rpt.AddParameter("AccessoPS", Constants.ModalitaAccesso.ProntoSoccorso)
        rpt.AddParameter("AccessoAV", Constants.ModalitaAccesso.AccessoVolontario)
        rpt.AddParameter("AccessoAO", Constants.ModalitaAccesso.AppuntamentoOnVac)
        rpt.AddParameter("AccessoPC", Constants.ModalitaAccesso.PrenotatiCup)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If Not OnVacReport.StampaReport(Constants.ReportName.PianoDiLavoroCup, strFiltro, rpt, , , bizReport.GetReportFolder(Constants.ReportName.PianoDiLavoroCup)) Then
                    OnVacUtility.StampaNonPresente(Page, Constants.ReportName.PianoDiLavoroCup)
                End If

            End Using
        End Using

    End Sub

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

    Private Function RecuperaUnieroAssociateCns() As String()

        Dim dt As New DataTable()

        Using dam As IDAM = OnVacUtility.OpenDam()

            With dam.QB
                .NewQuery()
                .AddSelectFields("COU_UNI_CODICE")
				.AddTables("T_ANA_LINK_CONSULTORI_UNIERO")
				Dim lista As List(Of String) = ucSelezioneConsultori.GetConsultoriSelezionati
				If lista.Count = 0 Then
					Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
						Using bizCns As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)
							lista = bizCns.GetListCodiceConsultoriUtente(OnVacContext.UserId)
						End Using
					End Using
				End If

				Dim filtroIn As New System.Text.StringBuilder()
					For i As Integer = 0 To lista.Count - 1
						filtroIn.AppendFormat("{0},", .AddCustomParam(lista(i)))
					Next

					If filtroIn.Length > 0 Then
						filtroIn.Remove(filtroIn.Length - 1, 1)
						.AddWhereCondition("COU_CNS_CODICE", Comparatori.In, filtroIn.ToString(), DataTypes.Replace)
					End If
				

				'.AddWhereCondition("COU_CNS_CODICE", Comparatori.Uguale, fmConsultorio.Codice, DataTypes.Stringa)
			End With

            dam.BuildDataTable(dt)

        End Using

        ' Nessuna unità erogatrice associata al consultorio
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            Return Nothing
        End If

        ' Creazione stringa con codici unità erogatrici
        Dim t As New System.Text.StringBuilder()

        For i As Integer = 0 To dt.Rows.Count - 1
            t.AppendFormat("{0},", dt.Rows(i)(0).ToString())
        Next
        If t.Length > 0 Then
            t.Remove(t.Length - 1, 1)
        End If

        Dim s As String() = t.ToString.Split(",")

        Return s

    End Function

    Private Sub completaDt(ByRef r As DataRow, ByVal codiceAusiliario As String, ByVal comCodice As String)

        Using dam As IDAM = OnVacUtility.OpenDam()

            Dim dtPaz As New DataTable()

            With dam.QB
                .NewQuery()
                .AddSelectFields("PAZ_CODICE", "PAZ_CNS_CODICE", "CNS_DESCRIZIONE")
                .AddTables("T_PAZ_PAZIENTI", "T_ANA_CONSULTORI")
                .AddWhereCondition("PAZ_CODICE_AUSILIARIO", Comparatori.Uguale, codiceAusiliario, DataTypes.Stringa)
                .AddWhereCondition("PAZ_CNS_CODICE", Comparatori.Uguale, "CNS_CODICE", DataTypes.OutJoinLeft)
            End With

            dam.BuildDataTable(dtPaz)

            If dtPaz.Rows.Count > 0 Then
                r("paz_codice") = dtPaz.Rows(0)("PAZ_CODICE").ToString
                r("paz_cns_codice") = dtPaz.Rows(0)("PAZ_CNS_CODICE").ToString
                r("paz_cns_descrizione") = dtPaz.Rows(0)("CNS_DESCRIZIONE").ToString
            Else
                r("paz_codice") = "00"
                r("paz_cns_codice") = "00"
                r("paz_cns_descrizione") = "NESSUN CONSULTORIO"
            End If

            With dam.QB
                .NewQuery()
                .AddSelectFields("COM_DESCRIZIONE")
                .AddTables("T_ANA_COMUNI")
                .AddWhereCondition("COM_CODICE", Comparatori.Uguale, comCodice, DataTypes.Stringa)
            End With

            r("paz_comune_residenza") = dam.ExecScalar()

        End Using

    End Sub

    Private Function pazientiPrenotati(ByRef stbErr As System.Text.StringBuilder) As System.Data.DataSet

        Dim WsSgp As OnVac.wsSGP.WsSgp
        Dim stbXmlIn As System.Text.StringBuilder
        Dim stbXmlOut As System.Text.StringBuilder
        Dim ds As System.Data.DataSet
        Dim dtsAppPre As New System.Data.DataSet()

        ' ------------------------------------- ' 

        Try
            ' Recupero il valore del polo dalla t_ana_parametri
            Dim polo As String = Settings.POLO_ONVAC
            Dim di, df As String

            ' Devo interrogare il web service o per l'uni ero selezionata o per tutte quelle abilitate
            Dim CodUniEro() As String = RecuperaUnieroAssociateCns()

            If CodUniEro Is Nothing OrElse CodUniEro.Length = 0 Then
                stbErr.Append("Nessuna agenda Cup associata al centro vaccinale selezionato.\nStampa non effettuata.")
                Return Nothing
            End If

            ' Ciclo di interrogazione del web service (per ogni unità erogatrice)
            Dim idx_uni_ero As Integer
            Dim uni_ero_mnem, uni_ero_descr As String
            Dim dt As DataTable
            Dim x As Integer = 0

            ImpostaStrutturaDataset(dtsAppPre)

            For idx_uni_ero = 0 To CodUniEro.Length - 1
                If (CodUniEro(idx_uni_ero) <> String.Empty) Then
                    dt = RitornaDatiUniero(CodUniEro(idx_uni_ero))
                    uni_ero_mnem = dt.Rows(0)("mnemonico")
                    uni_ero_descr = dt.Rows(0)("descrizione")

                    ' --------- Messaggio di interrogazione dei piani di lavoro --------- '
                    stbXmlIn = New System.Text.StringBuilder
                    stbXmlIn.Append("<?xml version=""1.0"" encoding=""UTF-8""?>")
                    stbXmlIn.Append("<XmlInPianiDiLavoro>")
                    stbXmlIn.Append("<PDLH>")
                    stbXmlIn.AppendFormat("<DominioInviante>{0}</DominioInviante>", polo)
                    stbXmlIn.AppendFormat("<DataOperazione>{0}</DataOperazione>", String.Format("{0:dd/MM/yyyy HH:mm:ss}", Date.Now))
                    stbXmlIn.Append("</PDLH>")
                    stbXmlIn.Append("<PDLQ>")
                    stbXmlIn.AppendFormat("<UnitaErogatrice>{0}</UnitaErogatrice>", uni_ero_mnem)

                    di = Format(odpDataEffettuazioneIniz.Data, "dd/MM/yyyy").ToString
                    df = Format(odpDataEffettuazioneFin.Data, "dd/MM/yyyy").ToString
                    stbXmlIn.AppendFormat("<DataInizio>{0}</DataInizio>", di)
                    stbXmlIn.AppendFormat("<DataFine>{0}</DataFine>", df)

                    stbXmlIn.Append("</PDLQ>")
                    stbXmlIn.Append("</XmlInPianiDiLavoro>")

                    WsSgp = New OnVac.wsSGP.WsSgp
                    stbXmlOut = New System.Text.StringBuilder

                    'Chiamata al metodo del servizio
                    stbXmlOut.Append(WsSgp.PianiDiLavoro(stbXmlIn.ToString))

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
                                    Dim drNewRow, dtrApp As DataRow

                                    'Loop sulle richieste per la valorizzazione del datatable DtsAppPre
                                    'da passare al datagrid gerarchico
                                    For i = 0 To ds.Tables("RIC").Rows.Count - 1

                                        For Each dtrApp In ds.Tables("RIC").Rows(i).GetChildRows("RIC_APP")

                                            ' --------- Inserimento appuntamento --------- '
                                            With ds.Tables("RIC")
                                                drNewRow = dtsAppPre.Tables("PrenotatiCup").NewRow

                                                drNewRow("data_prenotazione") = Format(CType(dtrApp("DataOraAppuntamento"), DateTime), "dd/MM/yyyy")
                                                drNewRow("ora_prenotazione") = Format(CType(dtrApp("DataOraAppuntamento"), DateTime), "H:mm")
                                                drNewRow("paz_codice_ausiliario") = .Rows(i)("CodicePaziente")
                                                drNewRow("paz_cognome") = .Rows(i)("Cognome")
                                                drNewRow("paz_nome") = .Rows(i)("Nome")
                                                drNewRow("paz_com_codice") = .Rows(i)("ComuneResidenza")
                                                drNewRow("paz_cap_residenza") = .Rows(i)("CapResidenza")
                                                If Not IsDBNull(.Rows(i)("DataNascita")) Then
                                                    drNewRow("paz_data_nascita") = .Rows(i)("DataNascita")
                                                End If
                                                completaDt(drNewRow, .Rows(i)("CodicePaziente"), .Rows(i)("ComuneResidenza"))
                                            End With

                                            dtsAppPre.Tables("PrenotatiCup").Rows.Add(drNewRow)
                                        Next
                                    Next

                                End If

                            Else
                                'Segnalazione di errore da parte del web service
                                stbErr.AppendLine("Errore in fase di selezione degli appuntamenti dalle agende Cup.")
                                stbErr.AppendLine("Contattare l\'amministratore del programma.")
                                stbErr.AppendLine("Segue la descrizione del problema segnalata dal WebService:")
                                stbErr.Append(Me.ApplyEscapeJS(ds.Tables("MSA").Rows(0)("MessaggioErrore")))
                            End If
                        End If
                    Else
                        stbErr.AppendLine("Errore in fase di selezione degli appuntamenti dalle agende Cup.")
                        stbErr.AppendLine("Contattare l\'amministratore del programma.")
                        stbErr.AppendLine("Segue descrizione del problema:")
                        stbErr.Append("Nessun messaggio di ritorno dal WebService")
                    End If

                End If
            Next

        Catch exc As Exception
            stbErr.AppendLine("Errore in fase di selezione degli appuntamenti dalle agende Cup.")
            stbErr.AppendLine("Contattare l\'amministratore del programma.")
            stbErr.AppendLine("Segue descrizione del problema:")
            stbErr.Append(Me.ApplyEscapeJS(exc.Message.Substring(0, exc.Message.Length - 1)))
        End Try

        Return dtsAppPre

    End Function

    Private Function pazientiVaccinati() As DataTable

        Dim dtPazientiVaccinati As New DataTable()

		Dim dataNascitaDa As String = odpDataNascitaDa.Text
        Dim dataNascitaA As String = odpDataNascitaA.Text

        Dim selAccesso As New System.Text.StringBuilder

        Dim dam As IDAM = OnVacUtility.OpenDam

        Try
            With dam.QB
                .NewQuery()

                For i As Integer = 0 To chklModAccesso.Items.Count - 1
                    If chklModAccesso.Items(i).Selected = True Then
                        selAccesso.Append(.AddCustomParam(chklModAccesso.Items(i).Value.ToString) & ",")
                    End If
                Next

                If selAccesso.Length > 0 Then
                    selAccesso.Remove(selAccesso.Length - 1, 1)
                End If

                .AddSelectFields("DISTINCT paz_codice", "paz_codice_ausiliario", "paz_nome", "paz_cognome", "paz_data_nascita", "paz_cns_codice", "cns_descrizione paz_cns_descrizione", "paz_com_codice_residenza paz_com_codice", "com_descrizione paz_comune_residenza", "paz_cap_residenza", "ves_data_effettuazione data_effettuazione", "ves_cns_codice", "ves_accesso", "NULL data_prenotazione", "NULL ora_prenotazione")
                .AddTables("t_paz_pazienti", "t_ana_comuni", "t_ana_consultori", "t_vac_eseguite")
                .AddWhereCondition("paz_com_codice_residenza", Comparatori.Uguale, "com_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("paz_cns_codice", Comparatori.Uguale, "cns_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("paz_codice", Comparatori.Uguale, "ves_paz_codice", DataTypes.Join)
                .AddWhereCondition("ves_data_effettuazione", Comparatori.MaggioreUguale, odpDataEffettuazioneIniz.Text, DataTypes.Data)
                .AddWhereCondition("ves_data_effettuazione", Comparatori.MinoreUguale, odpDataEffettuazioneFin.Text, DataTypes.Data)
                .OpenParanthesis()
                .AddWhereCondition("ves_stato", Comparatori.Is, "NULL", DataTypes.Stringa)
                .AddWhereCondition("ves_stato", Comparatori.Diverso, "R", DataTypes.Stringa, "OR")
                .CloseParanthesis()

				Dim lista As List(Of String) = ucSelezioneConsultori.GetConsultoriSelezionati
				If lista.Count = 0 Then
					Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
						Using bizCns As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)
							lista = bizCns.GetListCodiceConsultoriUtente(OnVacContext.UserId)
						End Using
					End Using
				End If

				If lista.Count > 0 Then
					Dim filtroIn As New System.Text.StringBuilder()
					For i As Integer = 0 To lista.Count - 1
						filtroIn.AppendFormat("{0},", .AddCustomParam(lista(i)))
					Next

					If filtroIn.Length > 0 Then
						filtroIn.Remove(filtroIn.Length - 1, 1)
						.AddWhereCondition("ves_cns_codice", Comparatori.In, filtroIn.ToString(), DataTypes.Replace)
					End If
				End If

				If dataNascitaDa <> "" AndAlso dataNascitaA <> "" Then
                    .AddWhereCondition("paz_data_nascita", Comparatori.MaggioreUguale, odpDataNascitaDa.Text, DataTypes.Data)
                    .AddWhereCondition("paz_data_nascita", Comparatori.MinoreUguale, odpDataNascitaA.Text, DataTypes.Data)
                End If

                If selAccesso.Length > 0 Then
                    .OpenParanthesis()
                    .AddWhereCondition("ves_accesso", Comparatori.Is, "NULL", DataTypes.Stringa)
                    .AddWhereCondition("ves_accesso", Comparatori.In, selAccesso.ToString, DataTypes.Stringa, "OR")
                    .CloseParanthesis()
                End If
            End With

            dam.BuildDataTable(dtPazientiVaccinati)

        Finally
            OnVacUtility.CloseDam(dam)
        End Try

        Return dtPazientiVaccinati

    End Function

    Private Sub ImpostaStrutturaDataset(ByRef dst As System.Data.DataSet)

        Dim dt As New DataTable()
        dt.TableName = "PrenotatiCup"

        dt.Columns.Add("paz_codice", GetType(Integer))
        dt.Columns.Add("paz_codice_ausiliario")
        dt.Columns.Add("paz_nome")
        dt.Columns.Add("paz_cognome")
        dt.Columns.Add("paz_data_nascita", GetType(Date))
        dt.Columns.Add("paz_cns_codice")
        dt.Columns.Add("paz_cns_descrizione")
        dt.Columns.Add("paz_com_codice")
        dt.Columns.Add("paz_comune_residenza")
        dt.Columns.Add("paz_cap_residenza")
        dt.Columns.Add("data_effettuazione", GetType(Date))
        dt.Columns.Add("ves_cns_codice")
        dt.Columns.Add("ves_accesso")
        dt.Columns.Add("data_prenotazione", GetType(Date))
        dt.Columns.Add("ora_prenotazione", GetType(DateTime))

        dst.Tables.Add(dt)

    End Sub

    'Funzione che ritorna un dataset contenente l'elenco dei pazienti con appuntamento (prenotato CUP) che non si sono presentati
    Private Function pazientiPrenotati_nonVaccinati(ByRef dtOnvac As DataTable, ByRef dtCup As DataTable, ByVal err As String) As System.Data.DataSet

        Dim dsNonVaccinati As New System.Data.DataSet()
        ImpostaStrutturaDataset(dsNonVaccinati)

        Dim i, j As Integer
        Dim r As DataRow
        Dim trovato As Boolean

        If err = "" Then
            For i = 0 To dtCup.Rows.Count - 1
                trovato = False

                'per ogni paziente prenotato Cup controllo se si è vaccinato (quindi se è presente nel Datatable dei pazienti vaccinati)
                For j = 0 To dtOnvac.Rows.Count - 1
                    If dtCup.Rows(i)("paz_codice") = dtOnvac.Rows(j)("paz_codice") Then
                        trovato = True
                        Exit For
                    End If
                Next

                ' se il paziente prenotato Cup non è fra i pazienti vaccinati, lo inserisco nel dsNonVaccinati
                If Not trovato Then
                    r = dsNonVaccinati.Tables(0).NewRow
                    r.ItemArray = dtCup.Rows(i).ItemArray
                    dsNonVaccinati.Tables(0).Rows.Add(r)
                End If
            Next

            Return dsNonVaccinati
        End If

        Return Nothing

    End Function

    'Funzione che ritorna un dataset contenente l'elenco dei pazienti senza appuntamento CUP che si sono vaccinati (perchè mandati da PS o presentati in forma spontanea)
    Private Function pazientiVaccinati_nonPrenotati(ByRef dtOnvac As DataTable, ByRef dtCup As DataTable, ByVal err As String) As System.Data.DataSet

        Dim dsNonPrenotati As New System.Data.DataSet()
        ImpostaStrutturaDataset(dsNonPrenotati)

        Dim i, j As Integer
        Dim r As DataRow
        Dim trovato As Boolean

        If err = "" Then
            For j = 0 To dtOnvac.Rows.Count - 1
                trovato = False

                'per ogni paziente vaccinato da OnVac controllo se si era prenotato da CUP (quindi se è presente nel Datatable dei pazienti prenotati Cup)
                For i = 0 To dtCup.Rows.Count - 1
                    If dtOnvac.Rows(j)("paz_codice") = dtCup.Rows(i)("paz_codice") Then
                        trovato = True
                        Exit For
                    End If
                Next

                ' se il paziente vaccinato non è fra i pazienti prenotati, lo inserisco nel dsNonPrenotati
                If Not trovato Then
                    r = dsNonPrenotati.Tables(0).NewRow
                    r.ItemArray = dtOnvac.Rows(j).ItemArray
                    dsNonPrenotati.Tables(0).Rows.Add(r)
                End If
            Next

            Return dsNonPrenotati
        End If

        Return Nothing

    End Function

    'Funzione che ritorna un dataset contenente l'elenco dei pazienti con appuntamento CUP che si sono vaccinati nella stessa data o in data diversa da quella di prenotazione
    Private Function pazientiPrenotati_Vaccinati(ByRef dtOnvac As DataTable, ByRef dtCup As DataTable, ByVal err As String) As System.Data.DataSet

        Dim dsPrenotatiVaccinati As New System.Data.DataSet()
        ImpostaStrutturaDataset(dsPrenotatiVaccinati)

        Dim i, j As Integer
        Dim r As DataRow
        Dim trovato As Boolean

        If err = "" Then
            For i = 0 To dtCup.Rows.Count - 1
                trovato = False

                'per ogni paziente prenotato Cup controllo se si è vaccinato (quindi se è presente nel Datatable dei pazienti vaccinati)
                For j = 0 To dtOnvac.Rows.Count - 1
                    If dtCup.Rows(i)("paz_codice") = dtOnvac.Rows(j)("paz_codice") Then
                        trovato = True
                        Exit For
                    End If
                Next

                ' se il paziente prenotato è fra i pazienti vaccinati, lo inserisco nel dsPrenotatiVaccinati
                If trovato Then
                    r = dsPrenotatiVaccinati.Tables(0).NewRow
                    r.ItemArray = dtOnvac.Rows(j).ItemArray
                    r.Item("data_prenotazione") = dtCup.Rows(i).Item("data_prenotazione")
                    r.Item("ora_prenotazione") = dtCup.Rows(i).Item("ora_prenotazione")
                    dsPrenotatiVaccinati.Tables(0).Rows.Add(r)
                End If
            Next

            Return dsPrenotatiVaccinati
        End If

        Return Nothing

    End Function

End Class
