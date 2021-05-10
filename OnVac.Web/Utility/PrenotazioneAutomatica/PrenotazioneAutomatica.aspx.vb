Imports Infragistics.WebUI.UltraWebToolbar
Imports Onit.OnAssistnet.OnVac.Biz

Public Class PrenotazioneAutomatica
    Inherits Common.PageBase

#Region " Properties "

    Private Property DtFiltroAssociazioniSel As DataTable
        Get
            Return Session("DtFiltroAssociazioniSel")
        End Get
        Set(Value As DataTable)
            Session("DtFiltroAssociazioniSel") = Value
        End Set
    End Property

    Private Property DtFiltroDosiSel As DataTable
        Get
            Return Session("DtFiltroDosiSel")
        End Get
        Set(Value As DataTable)
            Session("DtFiltroDosiSel") = Value
        End Set
    End Property

#End Region

#Region " Page "

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        If Not IsPostBack Then

            DtFiltroAssociazioniSel = Nothing
            DtFiltroDosiSel = Nothing

            ' Filtro modale consultorio prenotazione
            Dim filter As New Text.StringBuilder()

            filter.Append(" cns_data_apertura <= SYSDATE and (cns_data_chiusura > SYSDATE or cns_data_chiusura is null) ")
            filter.Append(" AND cns_dis_codice = dis_codice ")
            filter.AppendFormat(" AND dis_usl_codice = '{0}' ", OnVacContext.CodiceUslCorrente)

            filter.Append("order by cns_descrizione")

            fmConsultorioPrenotazione.Filtro = filter.ToString()

        End If

    End Sub

#End Region

    Private Sub ToolBar_ButtonClicked(sender As Object, be As ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case be.Button.Key

            Case "btnAvvia"

                Dim result As BizGenericResult = AvviaPrenotazioneAutomatica()

                Dim msg As String = "Processo di prenotazione automatica appuntamenti schedulato correttamente"

                If Not result.Success Then
                    msg = result.Message
                End If

                OnitLayout31.InsertRoutineJS(String.Format("alert('{0}');", HttpUtility.JavaScriptStringEncode(msg)))

        End Select

    End Sub

    Private Class ControlloFiltriResult
        Public Property Success As Boolean
        Public Property Message As String
    End Class

    Private Function ControlloFiltri() As ControlloFiltriResult

        Dim message As New Text.StringBuilder()

        ' TODO [App Auto]: controllo campi

        If dpkDataNascitaDa.Data > dpkDataNascitaA.Data Then
            message.AppendLine(" - La data di inizio INTERVALLO DI NASCITA non può essere successiva alla data di fine")
        End If

        If dpkDataConvocazione.Data = Date.MinValue OrElse dpkDataConvocazione.Data < Date.Today Then
            message.AppendLine(" - La data limite di CONVOCAZIONE è obbligatoria e non può essere precedente rispetto alla data di oggi")
        End If

        If dpkPrenotazioneDa.Data = Date.MinValue OrElse dpkPrenotazioneA.Data = Date.MinValue Then
            message.AppendLine(" - Le date di inizio e fine INTERVALLO DI PRENOTAZIONE sono obbligatorie")
        ElseIf dpkPrenotazioneDa.Data > dpkPrenotazioneA.Data Then
            message.AppendLine(" - La data di inizio INTERVALLO DI PRENOTAZIONE non può essere successiva alla data di fine")
        End If

        If dpkDataSchedulazione.Data = Date.MinValue Then
            message.AppendLine(" - Data e ora di SCHEDULAZIONE dell'attività sono obbligatorie")
        End If

        If message.Length = 0 Then
            Return New ControlloFiltriResult() With {.Success = True, .Message = String.Empty}
        End If

        Dim errorMessage As New Text.StringBuilder()
        errorMessage.AppendLine("Processo di prenotazione automatica appuntamenti NON avviato.")
        errorMessage.AppendLine(message.ToString())

        Return New ControlloFiltriResult() With {
            .Success = False,
            .Message = errorMessage.ToString()
        }

    End Function

    Private Function AvviaPrenotazioneAutomatica() As BizGenericResult

        Dim checkResult As ControlloFiltriResult = ControlloFiltri()

        If Not checkResult.Success Then
            Return New BizGenericResult() With {.Success = False, .Message = checkResult.Message}
        End If

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using biz As New BizGestioneAppuntamenti(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), New BizLogOptions(Log.DataLogStructure.TipiArgomento.APPUNTAMENTO, True))

                Dim command As New BizGestioneAppuntamenti.StartBatchPrenotazioneAutomaticaCommand()

                command.DataNascitaInizio = dpkDataNascitaDa.Data
                command.DataNascitaFine = dpkDataNascitaA.Data
                command.Sesso = ddlSesso.SelectedValue
                command.DataUltimaConvocazione = dpkDataConvocazione.Data
                command.CodiciConsultoriRicerca = ucSelezioneConsultori.GetConsultoriSelezionati()
                command.CodiceDistretto = fmDistretto.Codice

                ' Lettura filtro Associazioni-Dosi (stringa separata da |)
                ' N.B. : filtro dosi previsto ma non valorizzato da maschera (è nascosto)
                command.FiltroAssociazioniRicerca = UscFiltroAssociazioniDosi.getStringaFiltro1("|")
                command.FiltroDosiRicerca = UscFiltroAssociazioniDosi.getStringaFiltro2("|")

                command.TipoComunicazione = rblTipoComunicazione.SelectedValue

                command.DataPrenotazioneInizio = dpkPrenotazioneDa.Data
                command.DataPrenotazioneFine = dpkPrenotazioneA.Data

                Dim maxPaz As Integer = 0
                If Not Integer.TryParse(txtNumPazientiGiorno.Text, maxPaz) Then maxPaz = 0

                command.MaxPazientiGiorno = maxPaz

                command.CodiceConsultorioPrenotazione = fmConsultorioPrenotazione.Codice

                command.DataSchedulazione = dpkDataSchedulazione.Data.
                    AddHours(Convert.ToInt32(ddlOraSchedulazione.SelectedValue)).
                    AddMinutes(Convert.ToInt32(ddlMinutiSchedulazione.SelectedValue))

                ' TODO [App Auto]: parametri per report? Altri parametri?

                Return biz.StartBatchPrenotazioneAutomatica(command)

            End Using
        End Using

    End Function

    Protected Sub fmDistretto_SetUpFiletr(sender As Object)

        ' Solo i distretti appartenenti alla USL corrente
        fmDistretto.Filtro = String.Format("DIS_USL_CODICE = '{0}'", OnVacContext.CodiceUslCorrente)

    End Sub

#Region " Filtro Associazioni-Dosi "

    Private Sub btnImgAssociazioniDosi_Click(sender As Object, e As EventArgs) Handles btnImgAssociazioniDosi.Click

        UscFiltroAssociazioniDosi.setValoriSelezionatiFiltro1(DtFiltroAssociazioniSel)
        UscFiltroAssociazioniDosi.setValoriSelezionatiFiltro2(DtFiltroDosiSel)

        fmFiltroAssociazioniDosi.VisibileMD = True

    End Sub

    Private Sub btnOk_FiltroAssociazioniDosi_Click(sender As Object, e As EventArgs) Handles btnOk_FiltroAssociazioniDosi.Click

        fmFiltroAssociazioniDosi.VisibileMD = False

        DtFiltroAssociazioniSel = UscFiltroAssociazioniDosi.getValoriSelezionatiFiltro1()
        DtFiltroDosiSel = UscFiltroAssociazioniDosi.getValoriSelezionatiFiltro2()

        lblAssociazioniDosi.Text = UscFiltroAssociazioniDosi.getStringaFormattata()

    End Sub

    Private Sub btnAnnulla_FiltroAssociazioniDosi_Click(sender As Object, e As EventArgs) Handles btnAnnulla_FiltroAssociazioniDosi.Click

        fmFiltroAssociazioniDosi.VisibileMD = False

    End Sub

#End Region

End Class