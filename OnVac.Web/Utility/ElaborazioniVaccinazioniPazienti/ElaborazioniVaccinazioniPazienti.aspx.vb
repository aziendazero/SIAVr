Imports System.Collections.Generic

Imports Onit.OnAssistnet.Data

Imports Onit.OnAssistnet.MID
Imports Onit.OnAssistnet.MID.Models

Imports Onit.OnAssistnet.OnVac.MID
Imports Onit.OnAssistnet.OnVac.MID.Models
Imports Onit.OnAssistnet.OnVac.MID.Factories

Imports Onit.OnAssistnet.OnVac.Common.Utility.Extensions
Imports Onit.Database.DataAccessManager


Partial Class ElaborazioniVaccinazioniPazienti
    Inherits Common.PageBase


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

#Region " Enums "

    Public Enum DgrElabVacPazIdColumnIndexes

        Id = 0
        CodiceFiscalePaziente = 2
        CognomePaziente = 3
        NomePaziente = 4
        SessoPaziente = 5
        DataNascitaPaziente = 5
        DescrizioneComuneNascitaPaziente = 6
        DataEffettuazione = 7
        DescrizioneAssociazione = 8
        DescrizioneVaccinazione = 9
        CodicePaziente = 11
        CodiceRegionalePaziente = 12
        TesseraSanitariaPaziente = 13
        NomeOperatore = 14
        DataAcquisizione = 15
        MessaggioAcquisizione = 16
        IdProcessoAcquisizione = 17

    End Enum

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

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            OnVacUtility.ImpostaCnsLavoro(OnitLayout31)

            Me.SetLabelRisultati(-1)

            Me.dgrElabVacPaz.SelectedIndex = -1

            Me.ddlFtrStatiAcq.Items.Add(New ListItem("Da Acquisire", Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.DaAcquisire.ToString("d")))
            Me.ddlFtrStatiAcq.Items.Add(New ListItem("Acquisita Correttamente", Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.AcquisitaCorrettamente.ToString("d")))
            Me.ddlFtrStatiAcq.Items.Add(New ListItem("Acquisizione In Corso", Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.AcquisizioneInCorso.ToString("d")))
            Me.ddlFtrStatiAcq.Items.Add(New ListItem("Nessun Paziente Corrispondente", Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.NoCorrispondenzaPaziente.ToString("d")))
            Me.ddlFtrStatiAcq.Items.Add(New ListItem("Pazienti Corrispondenti Multipli", Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.CorrispondenzaPazienteMultipla.ToString("d")))
            Me.ddlFtrStatiAcq.Items.Add(New ListItem("Vaccinazione Esistente", Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.VaccinazioneEsistente.ToString("d")))
            Me.ddlFtrStatiAcq.Items.Add(New ListItem("Vaccinazione Esistente In Data Successiva", Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.VaccinazioneEsistenteInDataSuccessiva.ToString("d")))
            Me.ddlFtrStatiAcq.Items.Add(New ListItem("Consenso Paziente Bloccante", Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.ConsensoPazienteBloccante.ToString("d")))
            Me.ddlFtrStatiAcq.Items.Add(New ListItem("Eccezione", Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.Eccezione.ToString("d")))

            Me.dlsLegStatiAcq.DataSource = Me.ddlFtrStatiAcq.Items
            Me.dlsLegStatiAcq.DataBind()

            Me.ddlFtrStatiAcq.Items.Insert(0, String.Empty)
            Me.ddlFtrStatiAcq.SelectedIndex = 0

        End If

    End Sub

#End Region

#Region " Controls Events "

#Region " Toolbar "

    Private Sub ToolBar_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case be.Button.Key

            Case "btnCerca"

                Me.Cerca(0)

            Case "btnAcquisisci"

                Dim schedulaAcquisizioneMessage As New System.Text.StringBuilder

                Dim risultatoSchedulazioneAcquisizioneElaborazione As Biz.BizElaborazionePaziente.RisultatoSchedulazioneAcquisizioneElaborazione = Nothing

                Using bizElaborazionePaziente As New Biz.BizElaborazionePaziente(Me.Settings, Nothing, OnVacContext.CreateBizContextInfos(), Nothing)
                    risultatoSchedulazioneAcquisizioneElaborazione = bizElaborazionePaziente.SchedulaAcquisizioneElaborazioniVaccinazionePaziente()
                End Using

                If risultatoSchedulazioneAcquisizioneElaborazione.Success Then

                    If risultatoSchedulazioneAcquisizioneElaborazione.AcquisizioneElaborazioneSchedulateCount > 0 Then
                        schedulaAcquisizioneMessage.AppendFormat("Operazione eseguita con successo.\n{0} elaborazioni in acquisizione.", risultatoSchedulazioneAcquisizioneElaborazione.AcquisizioneElaborazioneSchedulateCount)
                    Else
                        schedulaAcquisizioneMessage.Append("Attenzione !!!\nNessuna elaborazione da acquisire.")
                    End If

                    Me.Cerca(Me.dgrElabVacPaz.CurrentPageIndex)

                Else
                    schedulaAcquisizioneMessage.Append(String.Format("Errore !!!\nImpossibile eseguire l\'operazione.\n{0}", risultatoSchedulazioneAcquisizioneElaborazione.Message))
                End If

                Me.ClientScript.RegisterStartupScript(Me.GetType(), "schedulaAcquisizioneMessage", String.Format("alert('{0}');", schedulaAcquisizioneMessage.ToString()), True)

            Case "btnCambiaVista"

                Dim codicePazienteDataGridColumn As DataGridColumn = Me.dgrElabVacPaz.Columns(DgrElabVacPazIdColumnIndexes.CodicePaziente)
                Dim codiceRegionalePazienteDataGridColumn As DataGridColumn = Me.dgrElabVacPaz.Columns(DgrElabVacPazIdColumnIndexes.CodiceRegionalePaziente)
                Dim tesseraSanitariaPazienteDataGridColumn As DataGridColumn = Me.dgrElabVacPaz.Columns(DgrElabVacPazIdColumnIndexes.TesseraSanitariaPaziente)
                Dim nomeOperatoreDataGridColumn As DataGridColumn = Me.dgrElabVacPaz.Columns(DgrElabVacPazIdColumnIndexes.NomeOperatore)
                Dim dataAcquisizioneDataGridColumn As DataGridColumn = Me.dgrElabVacPaz.Columns(DgrElabVacPazIdColumnIndexes.DataAcquisizione)
                Dim messaggioAcquisizioneDataGridColumn As DataGridColumn = Me.dgrElabVacPaz.Columns(DgrElabVacPazIdColumnIndexes.MessaggioAcquisizione)
                Dim idProcessoAcquisizioneDataGridColumn As DataGridColumn = Me.dgrElabVacPaz.Columns(DgrElabVacPazIdColumnIndexes.IdProcessoAcquisizione)

                Dim vistaCompleta As Boolean = Not codicePazienteDataGridColumn.Visible

                codicePazienteDataGridColumn.Visible = vistaCompleta
                codiceRegionalePazienteDataGridColumn.Visible = vistaCompleta
                tesseraSanitariaPazienteDataGridColumn.Visible = vistaCompleta
                nomeOperatoreDataGridColumn.Visible = vistaCompleta
                dataAcquisizioneDataGridColumn.Visible = vistaCompleta
                messaggioAcquisizioneDataGridColumn.Visible = vistaCompleta
                idProcessoAcquisizioneDataGridColumn.Visible = vistaCompleta

                Me.dgrElabVacPaz.Width = IIf(vistaCompleta, New Unit("2000", UnitType.Pixel), New Unit("100", UnitType.Percentage))

                be.Button.Text = String.Format("Vista {0}", IIf(vistaCompleta, "Ridotta", "Completa"))

        End Select

    End Sub

#End Region

#Region " Datagrid "

    Private Sub dgrElabVacPaz_ItemCommand(sender As Object, e As DataGridCommandEventArgs) Handles dgrElabVacPaz.ItemCommand

        Select Case e.CommandName

            Case "showMsqAcq"

                Using bizElaborazionePaziente As Biz.BizElaborazionePaziente = New Biz.BizElaborazionePaziente(OnVacContext.CreateBizContextInfos(), Nothing)

                    Dim id As Long = Convert.ToInt64(e.Item.Cells(DgrElabVacPazIdColumnIndexes.Id).Text)
                    Dim elaborazioneVaccinazionePaziente As Entities.ElaborazioneVaccinazionePaziente = bizElaborazionePaziente.GetElaborazioneVaccinazionePaziente(id)

                    Me.txtAcqInfo.Text = elaborazioneVaccinazionePaziente.MessaggioAcquisizione

                    Me.modAcqInfo.VisibileMD = True

                End Using

        End Select

    End Sub

    Private Sub dgrElabVacPaz_PageIndexChanged(source As Object, e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dgrElabVacPaz.PageIndexChanged

        Me.Cerca(e.NewPageIndex)

    End Sub

#End Region

#End Region

#Region " Protected "

    Protected Function GetCssClassStatoAcquisizione(statoAcquisizione As Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente)
        Dim cssClass As String
        Select Case statoAcquisizione
            Case Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.DaAcquisire
                cssClass = "BlueSquare"
            Case Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.AcquisizioneInCorso
                cssClass = "GoldSquare"
            Case Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.NoCorrispondenzaPaziente
                cssClass = "GraySquare"
            Case Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.CorrispondenzaPazienteMultipla
                cssClass = "SilverSquare"
            Case Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.AcquisitaCorrettamente
                cssClass = "GreenSquare"
            Case Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.VaccinazioneEsistente
                cssClass = "PurpleSquare"
            Case Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.VaccinazioneEsistenteInDataSuccessiva
                cssClass = "MistyPurpleSquare"
            Case Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.ConsensoPazienteBloccante
                cssClass = "PowderBlueSquare"
            Case Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.Eccezione
                cssClass = "RedSquare"
            Case Else
                Throw New NotImplementedException(String.Format("Stato Acquisizione non implementato: {0}", statoAcquisizione))
        End Select
        Return cssClass
    End Function

#End Region

#Region " Private "

    Private Sub Cerca(currentPageIndex As Integer)

        Dim elaborazioneVaccinazionePazienteFilter As New IElaborazionePazienteProvider.ElaborazioneVaccinazionePazienteFilter

        If Not String.IsNullOrEmpty(Me.ddlFtrStatiAcq.SelectedValue) Then
            elaborazioneVaccinazionePazienteFilter.StatoAcquisizione = [Enum].Parse(GetType(Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente), Me.ddlFtrStatiAcq.SelectedValue)
        End If

        If Not String.IsNullOrEmpty(Me.dpkFltAcqDa.Text) Then
            elaborazioneVaccinazionePazienteFilter.DataAcquisizioneDa = Me.dpkFltAcqDa.Data.Date
        End If

        If Not String.IsNullOrEmpty(Me.dpkFltAcqA.Text) Then
            elaborazioneVaccinazionePazienteFilter.DataAcquisizioneA = Me.dpkFltAcqA.Data.Date.Add(New TimeSpan(23, 59, 59))
        End If

        If Not String.IsNullOrEmpty(Me.txtFltIdPrcAcq.Text) Then
            elaborazioneVaccinazionePazienteFilter.IdProcessoAcquisizione = Convert.ToInt64(Me.txtFltIdPrcAcq.Text)
        End If

        Using bizElaborazionePaziente As New Biz.BizElaborazionePaziente(OnVacContext.CreateBizContextInfos(), Nothing)

            Me.dgrElabVacPaz.VirtualItemCount = Convert.ToInt32(bizElaborazionePaziente.CountElaborazioniVaccinazionePaziente(elaborazioneVaccinazionePazienteFilter))

            Dim startIndex As Integer = currentPageIndex * Me.dgrElabVacPaz.PageSize

            If startIndex > Me.dgrElabVacPaz.VirtualItemCount - 1 Then
                startIndex = 0
                currentPageIndex = 0
            End If

            Dim pagingOptions As New PagingOptions()
            pagingOptions.StartRecordIndex = startIndex
            pagingOptions.EndRecordIndex = startIndex + Me.dgrElabVacPaz.PageSize

            Me.dgrElabVacPaz.DataSource = bizElaborazionePaziente.GetElaborazioniVaccinazionePaziente(elaborazioneVaccinazionePazienteFilter, pagingOptions)

            Me.dgrElabVacPaz.CurrentPageIndex = currentPageIndex
            Me.dgrElabVacPaz.DataBind()

            Me.dgrElabVacPaz.SelectedIndex = -1

            Me.dgrElabVacPaz.Visible = (Me.dgrElabVacPaz.VirtualItemCount > 0)

            Me.SetLabelRisultati(Me.dgrElabVacPaz.VirtualItemCount)

        End Using

    End Sub

    Private Sub SetLabelRisultati(countAlias As Integer)

        Dim msg As String

        Select Case countAlias
            Case -1
                msg = String.Empty
            Case 0
                msg = ": nessuna elaborazione trovata"
            Case 1
                msg = ": 1 elaborazione trovata"
            Case Else
                msg = String.Format(": {0} elaborazioni trovate", countAlias.ToString())
        End Select

        lblRisultati.Text = String.Format("Risultati della ricerca{0}", msg)

    End Sub

#End Region

End Class
