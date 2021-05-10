Imports System.Collections.Generic

Namespace Common

    Public MustInherit Class OnVacMovimentiPageBase
        Inherits OnVac.Common.PageBase

#Region " Constants "

        Protected Const KEY_CONFERMA_ELIMINA_PROG As String = "confermaEliminazioneProg"

#End Region

#Region " Types "

        Protected Enum StatoPaginaMovimenti
            Lettura = 0
            Modifica = 1
        End Enum

        Protected Class TipoMovimentoCns

            Public Class MovimentoInterno

                Public Const Ingresso As String = "I"
                Public Const Uscita As String = "U"
                Public Const Smistamento As String = "R"    ' perchè era "RASMI"...

            End Class

        End Class

#End Region

#Region " Properties "

        Protected Property StatoPagina As StatoPaginaMovimenti
            Get
                If ViewState("StatoPaginaMovimenti") Is Nothing Then ViewState("StatoPaginaMovimenti") = StatoPaginaMovimenti.Lettura
                Return ViewState("StatoPaginaMovimenti")
            End Get
            Set(value As StatoPaginaMovimenti)
                ViewState("StatoPaginaMovimenti") = value
                ImpostaLayoutMovimenti(value)
            End Set
        End Property

        Protected Property CodicePazienteSelezionato As Integer?
            Get
                Return ViewState("CodicePazienteSelezionato")
            End Get
            Set(value As Integer?)
                ViewState("CodicePazienteSelezionato") = value
            End Set
        End Property

        Protected MustOverride ReadOnly Property OnitLayout As Onit.Controls.PagesLayout.OnitLayout3

#End Region

#Region " Page Events "

        Protected Overrides Sub OnLoad(e As EventArgs)

            MyBase.OnLoad(e)

            AddHandler Me.OnitLayout.ConfirmClick, AddressOf OnitLayoutConfirmClick

            If Not IsPostBack Then

                Me.StatoPagina = StatoPaginaMovimenti.Lettura

                Me.CodicePazienteSelezionato = Nothing

                OnVacUtility.ImpostaCnsLavoro(Me.OnitLayout)

            End If

        End Sub

        Protected Overrides Sub OnInit(e As System.EventArgs)

            MyBase.OnInit(e)

            Me.ClientScript.RegisterStartupScript(Me.GetType(),
                                                  "scriptMovimenti",
                                                  String.Format("<script src='{0}' type='text/javascript' ></script>", Me.ResolveClientUrl("~/Common/Scripts/scriptMovimenti.js")))
        End Sub

#End Region

#Region " Protected Methods "

        Protected ReadOnly Property IsPageInEdit() As Boolean
            Get
                Return Me.StatoPagina = StatoPaginaMovimenti.Modifica
            End Get
        End Property

        Protected Function UpdateStatoAnagrafico(codiceStatoAnagrafico As String, codiceStatoAnagraficoOriginale As String) As Boolean

            If Not Me.CodicePazienteSelezionato.HasValue Then
                Throw New Exception("Operazione non effettuata: codice paziente mancante.")
            End If

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                ' Salvataggio stato anagrafico del paziente
                Using bizPaziente As New Biz.BizPaziente(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    If Not bizPaziente.UpdateStatoAnagrafico(Me.CodicePazienteSelezionato.Value, codiceStatoAnagrafico, codiceStatoAnagraficoOriginale) Then

                        Me.OnitLayout.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Salvataggio non effettuato: stato anagrafico non previsto", "alertStatoAnag", False, False))
                        Return False

                    End If

                End Using

                ' Controllo programmazione da cancellare in base a stato anagrafico
                If Me.ConfermaCancellazioneProgrammazione(codiceStatoAnagrafico, True, genericProvider) Then

                    Return False

                End If

            End Using

            Return True

        End Function

        Protected Function ConfermaCancellazioneProgrammazione(codiceStatoAnagrafico As String, addSaveMessage As Boolean) As Boolean

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                Return Me.ConfermaCancellazioneProgrammazione(codiceStatoAnagrafico, addSaveMessage, genericProvider)

            End Using

        End Function

        Protected Sub EliminaProgrammazione(codicePaziente As Integer)

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizVacProg As New Biz.BizVaccinazioneProg(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                    Dim command As New Biz.BizVaccinazioneProg.EliminaProgrammazioneCommand()
                    command.CodicePaziente = codicePaziente
                    command.DataConvocazione = Nothing
                    command.EliminaAppuntamenti = True
                    command.EliminaSollecitiBilancio = True
                    command.EliminaBilanci = False
                    command.TipoArgomentoLog = TipiArgomento.ELIMINA_PROG
                    command.OperazioneAutomatica = True
                    command.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.VariazioneStatoAnagrafico
                    command.NoteEliminazione = "Eliminazione convocazioni paziente per variazione stato anagrafico da maschera MovCV"

                    bizVacProg.EliminaProgrammazione(command)

                End Using
            End Using

        End Sub

        Protected Overridable Sub DataGridItemDataBound(currentDataGridItem As DataGridItem, editItemIndex As Integer)

            Dim enable As Boolean = (Me.StatoPagina = StatoPaginaMovimenti.Lettura)

            ' Abilita/Disabilita i pulsanti della griglia, dove presenti
            EnableImageButton(currentDataGridItem, "btnEditGrid", enable)
            EnableImageButton(currentDataGridItem, "btnPresaVisione", enable)
            EnableImageButton(currentDataGridItem, "btnRichiestaCertificato", enable)
            EnableImageButton(currentDataGridItem, "btnRegPaz", enable)
            EnableImageButton(currentDataGridItem, "btnCertEmi", enable)
            EnableImageButton(currentDataGridItem, "btnNotifica", enable)

            ' Pulsante acquisizione
            ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
            If FlagAbilitazioneVaccUslCorrente Then EnableImageButton(currentDataGridItem, "btnAcquisizione", enable)

            If currentDataGridItem.ItemIndex = editItemIndex Then

                ' STATO ANAGRAFICO

                ' N.B.
                ' Il parametro MOVCV_EDIT_STATO_ANAGRAFICO viene utilizzato (in ogni maschera a parte RASMI) per abilitare/disabilitare 
                ' direttamente il pulsante di edit e non il singolo controllo relativo allo stato anagrafico. 
                ' Questo perchè, nei datagrid delle varie maschere, lo stato anagrafico è l'unico campo modificabile.

                Dim codiceStatoAnagrafico As String = String.Empty

                ' Label stato anagrafico
                Dim lblCodiceStatoAnagraficoEdit As Label = DirectCast(currentDataGridItem.FindControl("lblCodiceStatoAnagraficoEdit"), Label)

                If Not lblCodiceStatoAnagraficoEdit Is Nothing Then
                    codiceStatoAnagrafico = lblCodiceStatoAnagraficoEdit.Text
                End If

                ' Dropdownlist stato anagrafico
                Dim ddlStatoAnagrafico As DropDownList = DirectCast(currentDataGridItem.FindControl("ddlStatoAnagrafico"), DropDownList)

                ' Bind della dropdownlist solo per la riga in edit
                If Not ddlStatoAnagrafico Is Nothing Then
                    BindDdlStatoAnagrafico(ddlStatoAnagrafico, codiceStatoAnagrafico)
                End If

                ' PULSANTE NOTE ACQUISIZIONE
                ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
                If FlagAbilitazioneVaccUslCorrente Then

                    Dim btnNoteAcquisizione As ImageButton = DirectCast(currentDataGridItem.FindControl("btnNoteAcquisizione"), ImageButton)

                    If Not btnNoteAcquisizione Is Nothing Then
                        btnNoteAcquisizione.Enabled = False
                    End If

                End If

            End If

        End Sub

        Protected Sub AddConfirmClickToImageButton(currentDataGridItem As DataGridItem, buttonId As String, confirmMessage As String)

            Dim btn As ImageButton = DirectCast(currentDataGridItem.FindControl(buttonId), ImageButton)

            confirmMessage = HttpUtility.JavaScriptStringEncode(confirmMessage)

            btn.Attributes.Add("onclick", "if(!confirm('" + confirmMessage + "')) StopPreventDefault(event);")

        End Sub

        Protected Sub SetEditButtonVisibility(currentDataGridItem As DataGridItem)

            ' N.B.
            ' La visibilità del pulsante di edit viene impostata in base al parametro MOVCV_EDIT_STATO_ANAGRAFICO
            ' perchè, in ogni riga dei datagrid delle pagine, il solo controllo editabile è lo stato anagrafico.

            Dim btnEditGrid As ImageButton = DirectCast(currentDataGridItem.FindControl("btnEditGrid"), ImageButton)

            If Not btnEditGrid Is Nothing Then

                btnEditGrid.Visible = Me.Settings.MOVCV_EDIT_STATO_ANAGRAFICO

            End If

        End Sub

        Protected Sub BindDdlStatoAnagrafico(ddlStatoAnagrafico As DropDownList, codiceStatoAnagraficoSelezionato As String)

            If Not ddlStatoAnagrafico Is Nothing Then

                Dim dtStatiAnagrafici As DataTable = Nothing

                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                    Using bizStatiAnagrafici As New Biz.BizStatiAnagrafici(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                        dtStatiAnagrafici = bizStatiAnagrafici.LeggiStatiAnagrafici()

                    End Using
                End Using

                Dim dataView As New DataView(dtStatiAnagrafici)
                If Not Settings.MOVCV_STATI_ANAGRAFICI_DA_ESCLUDERE.IsNullOrEmpty() Then
                    dataView.RowFilter = String.Format(" SAN_CODICE not in('{0}') ", String.Join("','", Settings.MOVCV_STATI_ANAGRAFICI_DA_ESCLUDERE.ToArray()))
                End If

                ddlStatoAnagrafico.DataTextField = "SAN_DESCRIZIONE"
                ddlStatoAnagrafico.DataValueField = "SAN_CODICE"
                ddlStatoAnagrafico.DataSource = dataView
                ddlStatoAnagrafico.DataBind()

                If Not String.IsNullOrEmpty(codiceStatoAnagraficoSelezionato) AndAlso ddlStatoAnagrafico.Items.Count > 0 Then

                    Dim listItem As ListItem = ddlStatoAnagrafico.Items.FindByValue(codiceStatoAnagraficoSelezionato)

                    If Not listItem Is Nothing Then listItem.Selected = True

                End If

            End If

        End Sub

        Protected Sub EditMovimento(codicePaziente As Integer)

            Me.StatoPagina = StatoPaginaMovimenti.Modifica

            Me.CodicePazienteSelezionato = codicePaziente

        End Sub

        Protected Sub SelectMovimento(codicePaziente As Integer, pageInEdit As Boolean)

            Me.StatoPagina = StatoPaginaMovimenti.Modifica

            Me.CodicePazienteSelezionato = codicePaziente

        End Sub

        Protected Sub StampaElencoMovimentiInterni(tipoMovimento As String, dgrPazienti As Onit.Controls.OnitGrid.OnitGrid, dpkDaNascita As Onit.Web.UI.WebControls.Validators.OnitDatePick, dpkANascita As Onit.Web.UI.WebControls.Validators.OnitDatePick, dpkDaConsultorio As Onit.Web.UI.WebControls.Validators.OnitDatePick, dpkAConsultorio As Onit.Web.UI.WebControls.Validators.OnitDatePick)

            ' Valorizzazione dataset da passare al report
            Dim dst As New DstMovimentiInterni()
            Dim newRow As DataRow = Nothing

            For i As Integer = 0 To dgrPazienti.Items.Count - 1

                newRow = dst.Tables("Movimenti").NewRow()

                ' Dati per report
                newRow("CodPaziente") = Me.GetTextFromDatagridItem(dgrPazienti, dgrPazienti.Items(i), "CodPaziente")
                newRow("Cognome") = Me.GetTextFromDatagridItem(dgrPazienti, dgrPazienti.Items(i), "Cognome")
                newRow("Nome") = Me.GetTextFromDatagridItem(dgrPazienti, dgrPazienti.Items(i), "Nome")

                If Not String.IsNullOrEmpty(Me.GetTextFromDatagridItem(dgrPazienti, dgrPazienti.Items(i), "DataNascita")) Then
                    newRow("DataNascita") = CDate(Me.GetTextFromDatagridItem(dgrPazienti, dgrPazienti.Items(i), "DataNascita"))
                End If

                newRow("Indirizzo") = Me.GetTextFromDatagridItem(dgrPazienti, dgrPazienti.Items(i), "Indirizzo")

                Select Case tipoMovimento

                    Case TipoMovimentoCns.MovimentoInterno.Smistamento

                        newRow("StatoAnagrafico") = Me.GetTextFromDatagridItem(dgrPazienti, dgrPazienti.Items(i), "DescrizioneStatoAnagrafico")

                        If Not String.IsNullOrEmpty(Me.GetTextFromDatagridItem(dgrPazienti, dgrPazienti.Items(i), "DataInserimento")) Then
                            newRow("DataInserimento") = CDate(Me.GetTextFromDatagridItem(dgrPazienti, dgrPazienti.Items(i), "DataInserimento"))
                        End If

                    Case TipoMovimentoCns.MovimentoInterno.Ingresso,
                         TipoMovimentoCns.MovimentoInterno.Uscita

                        If tipoMovimento = TipoMovimentoCns.MovimentoInterno.Ingresso Then

                            newRow("AmbVacc_Cod") = Me.GetTextFromDatagridItem(dgrPazienti, dgrPazienti.Items(i), "CodConsOld")

                            If Not String.IsNullOrEmpty(Me.GetTextFromDatagridItem(dgrPazienti, dgrPazienti.Items(i), "PresaVisione")) Then
                                newRow("PresaVisione") = Me.GetTextFromDatagridItem(dgrPazienti, dgrPazienti.Items(i), "PresaVisione")
                            End If

                        Else

                            newRow("AmbVacc_Cod") = Me.GetTextFromDatagridItem(dgrPazienti, dgrPazienti.Items(i), "CodConsNew")

                        End If

                        newRow("StatoAnagrafico") = Me.GetDescrizioneStatoAnagraficoFromLabel(dgrPazienti.Items(i))

                        newRow("AmbVacc_Desc") = Me.GetTextFromDatagridItem(dgrPazienti, dgrPazienti.Items(i), "Descrizione")

                        If Not String.IsNullOrEmpty(Me.GetTextFromDatagridItem(dgrPazienti, dgrPazienti.Items(i), "DataAssegnazione")) Then
                            newRow("DataAggiornamento") = CDate(Me.GetTextFromDatagridItem(dgrPazienti, dgrPazienti.Items(i), "DataAssegnazione"))
                        End If

                        If Not String.IsNullOrEmpty(Me.GetTextFromDatagridItem(dgrPazienti, dgrPazienti.Items(i), "InvioCartella")) Then
                            newRow("InvioCartella") = Me.GetTextFromDatagridItem(dgrPazienti, dgrPazienti.Items(i), "InvioCartella")
                        End If

                End Select

                dst.Tables("Movimenti").Rows.Add(newRow)

            Next

            Dim rpt As New ReportParameter()

            rpt.set_dataset(dst)

            ' Parametri intestazione
            Dim datiIntestazione As Entities.DatiIntestazioneReport = GetDatiIntestazioneReport()

            rpt.AddParameter("UslCitta", datiIntestazione.ComuneUsl)
            rpt.AddParameter("UslDesc", datiIntestazione.DescrizioneUslPerReport)
            rpt.AddParameter("UslReg", datiIntestazione.RegioneUsl)

            ' Filtri
            rpt.AddParameter("Da_data_nascita", GetDateParameterValue(dpkDaNascita))
            rpt.AddParameter("A_data_nascita", GetDateParameterValue(dpkANascita))
            rpt.AddParameter("Da_data_cns", GetDateParameterValue(dpkDaConsultorio))
            rpt.AddParameter("A_data_cns", GetDateParameterValue(dpkAConsultorio))

            ' Altri parametri
            rpt.AddParameter("Ambulatorio", OnVacUtility.Variabili.CNS.Descrizione + "(" + OnVacUtility.Variabili.CNS.Codice + ")")
            rpt.AddParameter("Tipo", tipoMovimento)
            rpt.AddParameter("TotAssistiti", dgrPazienti.Items.Count.ToString())

            ' Stampa
            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                    If Not OnVacReport.StampaReport(Page.Request.Path, Constants.ReportName.ElencoMovimenti, String.Empty, rpt, Nothing, Nothing, bizReport.GetReportFolder(Constants.ReportName.ElencoMovimenti)) Then
                        OnVacUtility.StampaNonPresente(Page, Constants.ReportName.ElencoMovimenti)
                    End If

                End Using
            End Using

        End Sub

        Protected Function GetDateParameterValue(dpk As Onit.Web.UI.WebControls.Validators.OnitDatePick) As String

            If dpk.Data = DateTime.MinValue Then Return String.Empty

            Return dpk.Text

        End Function

#End Region

#Region " Overrides/MustOverrides "

        Protected Overridable Sub ImpostaLayoutMovimenti(stato As StatoPaginaMovimenti)

            ' Top e Left Frame
            Me.OnitLayout.Busy = (stato = StatoPaginaMovimenti.Modifica)

        End Sub

        Protected MustOverride Sub EliminaProgrammazioneEffettuata()

        Protected MustOverride Sub EliminaProgrammazioneNonEffettuata()

#End Region

#Region " Overridable "

        ''' <summary>
        ''' Metodo di redirect alla pagina di gestione del paziente.
        ''' Imposta il codice dell'ultimo paziente selezionato, utilizzabile per la ricerca rapida, e richiama il metodo della pagina base
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <remarks></remarks>
        Public Overrides Sub RedirectToGestionePaziente(codicePaziente As String)

            Me.UltimoPazienteSelezionato = New Entities.UltimoPazienteSelezionato(String.Empty, codicePaziente)

            MyBase.RedirectToGestionePaziente(codicePaziente)

        End Sub

#End Region

#Region " Control Events "

        Private Sub OnitLayoutConfirmClick(sender As Object, e As Onit.Controls.PagesLayout.OnitLayout3.ConfirmEventArgs)

            If e.Key = KEY_CONFERMA_ELIMINA_PROG Then

                If e.Result Then
                    Me.EliminaProgrammazione(Me.CodicePazienteSelezionato)
                    Me.EliminaProgrammazioneEffettuata()
                Else
                    Me.EliminaProgrammazioneNonEffettuata()
                End If

            End If

        End Sub

#End Region

#Region " Private "

        ' Restituisce true se viene richiesta conferma all'utente, false altrimenti
        Private Function ConfermaCancellazioneProgrammazione(codiceStatoAnagrafico As String, addSaveMessage As Boolean, genericProvider As DAL.DbGenericProvider) As Boolean

            Dim isStatoAnagraficoCancProg As Boolean = False

            Using bizStatiAnagrafici As New Biz.BizStatiAnagrafici(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                isStatoAnagraficoCancProg = bizStatiAnagrafici.IsStatoAnagraficoCancellazioneProgrammazione(codiceStatoAnagrafico)

            End Using

            If isStatoAnagraficoCancProg Then

                Dim countProgDaEliminare As Integer = 0

                Using bizVacProg As New Biz.BizVaccinazioneProg(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)

                    countProgDaEliminare = bizVacProg.CountProgrammazioneDaEliminare(Me.CodicePazienteSelezionato, Nothing, True, True, False)

                End Using

                ' Richiesta all'utente
                If countProgDaEliminare > 0 Then

                    Dim messaggioConferma As String =
                        IIf(addSaveMessage, "Salvataggio effettuato.\n", String.Empty) +
                        "Lo stato anagrafico selezionato e\' fra quelli per cui e\' prevista la CANCELLAZIONE DELLA PROGRAMMAZIONE VACCINALE relativa al paziente. L\'operazione NON E\' ANNULLABILE, le modifche verranno apportate direttamente su database! Continuare?"

                    Me.OnitLayout.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(messaggioConferma, KEY_CONFERMA_ELIMINA_PROG, True, True))

                    Return True

                End If

            End If

            Return False

        End Function

        Protected Function GetDatiIntestazioneReport() As Entities.DatiIntestazioneReport

            Dim datiIntestazione As Entities.DatiIntestazioneReport = Nothing

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                    Try
                        datiIntestazione = bizReport.GetDatiIntestazione()
                    Catch ex As Exception
                        datiIntestazione =
                            New Entities.DatiIntestazioneReport() With {
                                .ComuneUsl = String.Empty,
                                .DescrizioneUslPerReport = String.Empty,
                                .RegioneUsl = String.Empty
                        }
                    End Try

                End Using
            End Using

            Return datiIntestazione

        End Function

        Private Function GetTextFromDatagridItem(dgrPazienti As Onit.Controls.OnitGrid.OnitGrid, dataGridItem As DataGridItem, key As String) As String

            Return HttpUtility.HtmlDecode(dataGridItem.Cells(dgrPazienti.getColumnNumberByKey(key)).Text).Trim()

        End Function

        Private Function GetDescrizioneStatoAnagraficoFromLabel(datagriditem As DataGridItem) As String

            Dim lblStatoAnagrafico As Label = DirectCast(datagriditem.FindControl("lblStatoAnagrafico"), Label)

            If lblStatoAnagrafico Is Nothing Then Return String.Empty

            Return HttpUtility.HtmlDecode(lblStatoAnagrafico.Text).Trim()

        End Function

        Private Sub EnableImageButton(currentDataGridItem As DataGridItem, buttonId As String, enable As Boolean)

            Dim btn As ImageButton = DirectCast(currentDataGridItem.FindControl(buttonId), ImageButton)

            If Not btn Is Nothing Then

                btn.Enabled = enable

                Dim currentImage As String = btn.ImageUrl

                If currentImage.IndexOf("_dis") > -1 Then

                    If enable Then
                        ' Immagine disabilitata -> da abilitare

                        Dim nome As String = currentImage.Substring(0, currentImage.LastIndexOf("_dis."))
                        Dim estensione As String = currentImage.Substring(currentImage.LastIndexOf("."))

                        btn.ImageUrl = nome + estensione

                    End If

                Else

                    If Not enable Then
                        ' Immagine abilitata -> da disabilitare

                        Dim nome As String = currentImage.Substring(0, currentImage.LastIndexOf("."))
                        Dim estensione As String = currentImage.Substring(currentImage.LastIndexOf("."))

                        btn.ImageUrl = String.Format("{0}_dis{1}", nome, estensione)

                    End If

                End If

            End If

        End Sub

#End Region

    End Class

End Namespace
