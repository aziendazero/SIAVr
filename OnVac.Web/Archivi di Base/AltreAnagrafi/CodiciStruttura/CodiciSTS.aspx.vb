Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.Entities

Public Class CodiciSTS
    Inherits Common.PageBase

#Region " Properties "

    Private Property StatoCorrenteLayout As StatoLayout
        Get
            If ViewState("layout") Is Nothing Then ViewState("layout") = StatoLayout.View
            Return ViewState("layout")
        End Get
        Set(value As StatoLayout)
            ViewState("layout") = value
        End Set
    End Property

    Private Sub ClearCampi()

        txtCodice.Text = String.Empty
        txtDescrizione.Text = String.Empty
        odpDataInizioValidita.Text = String.Empty
        odpDataFineValidita.Text = String.Empty
        txtIndirizzo.Text = String.Empty
        omlAsl.Descrizione = String.Empty
        omlAsl.Codice = String.Empty
        txtIndirizzo.Text = String.Empty
        omlComune.Descrizione = String.Empty
        omlComune.Codice = String.Empty

    End Sub

#End Region

#Region "Types"

    Private Enum GridColumnIndex
        Selezione = 0
        Id = 1
        Codice = 2
        Descrizione = 3
        DataInizioValidita = 4
        DataFineValidita = 5
        CodiceAsl = 6
        Indirizzo = 7
        Comune = 8
    End Enum

    Private Enum StatoLayout
        View
        Edit
        Nuovo
        Elimina
        RigaSelezionata
    End Enum

    Protected Function StringToDateTime(val As String) As DateTime?

        Dim conv As DateTime? = Nothing

        Try
            conv = Convert.ToDateTime(val)
        Catch
            Return Nothing
        End Try

        Return conv

    End Function

    Protected Function DateTimeToString(val As DateTime?) As String

        If val.HasValue Then
            Return val.Value.ToString("dd/MM/yyyy")
        Else
            Return String.Empty
        End If

    End Function

#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then

            SetLayout(StatoLayout.View)

        End If
    End Sub

    Private Sub SetLayout(stato As StatoLayout)
        StatoCorrenteLayout = stato

        Select Case stato
            Case StatoLayout.View

                OnitLayout31.Busy = False

                ToolBar.Items.FromKeyButton("btnCerca").Enabled = True
                ToolBar.Items.FromKeyButton("btnNuovo").Enabled = True
                ToolBar.Items.FromKeyButton("btnModifica").Enabled = False
                ToolBar.Items.FromKeyButton("btnElimina").Enabled = False
                ToolBar.Items.FromKeyButton("btnSalva").Enabled = False
                ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = False

                dgrCodiciSts.Enabled = True
                dgrCodiciSts.SelectedIndex = -1

                txtFiltro.Enabled = True
                txtCodice.Enabled = False
                txtDescrizione.Enabled = False
                odpDataInizioValidita.Enabled = False
                odpDataFineValidita.Enabled = False
                omlAsl.Enabled = False
                txtIndirizzo.Enabled = False
                omlComune.Enabled = False

                txtCodice.CssClass = "TextBox_Stringa uppercase"
                txtDescrizione.CssClass = "TextBox_Stringa uppercase"

                ClearCampi()

            Case StatoLayout.RigaSelezionata

                OnitLayout31.Busy = False

                ToolBar.Items.FromKeyButton("btnCerca").Enabled = True
                ToolBar.Items.FromKeyButton("btnNuovo").Enabled = True
                ToolBar.Items.FromKeyButton("btnModifica").Enabled = True
                ToolBar.Items.FromKeyButton("btnElimina").Enabled = True
                ToolBar.Items.FromKeyButton("btnSalva").Enabled = False
                ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = False

                txtCodice.Enabled = False
                txtDescrizione.Enabled = False
                odpDataInizioValidita.Enabled = False
                odpDataFineValidita.Enabled = False
                omlAsl.Enabled = False
                txtIndirizzo.Enabled = False
                omlComune.Enabled = False

            Case StatoLayout.Edit

                OnitLayout31.Busy = True

                ToolBar.Items.FromKeyButton("btnCerca").Enabled = False
                ToolBar.Items.FromKeyButton("btnNuovo").Enabled = False
                ToolBar.Items.FromKeyButton("btnModifica").Enabled = False
                ToolBar.Items.FromKeyButton("btnElimina").Enabled = False
                ToolBar.Items.FromKeyButton("btnSalva").Enabled = True
                ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = True

                dgrCodiciSts.Enabled = False

                txtFiltro.Enabled = False
                txtCodice.Enabled = False
                txtDescrizione.Enabled = True
                odpDataInizioValidita.Enabled = True
                odpDataFineValidita.Enabled = True
                omlAsl.Enabled = True
                txtIndirizzo.Enabled = True
                omlComune.Enabled = True

                txtDescrizione.CssClass = "TextBox_Stringa_Obbligatorio uppercase"

            Case StatoLayout.Elimina

                OnitLayout31.Busy = True

                ToolBar.Items.FromKeyButton("btnCerca").Enabled = False
                ToolBar.Items.FromKeyButton("btnNuovo").Enabled = False
                ToolBar.Items.FromKeyButton("btnModifica").Enabled = False
                ToolBar.Items.FromKeyButton("btnElimina").Enabled = False
                ToolBar.Items.FromKeyButton("btnSalva").Enabled = True
                ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = True

                dgrCodiciSts.Enabled = False

                txtCodice.Enabled = False
                txtDescrizione.Enabled = False
                odpDataInizioValidita.Enabled = False
                odpDataFineValidita.Enabled = False
                omlAsl.Enabled = False
                txtIndirizzo.Enabled = False
                omlComune.Enabled = False

            Case StatoLayout.Nuovo

                OnitLayout31.Busy = True

                ToolBar.Items.FromKeyButton("btnCerca").Enabled = False
                ToolBar.Items.FromKeyButton("btnNuovo").Enabled = False
                ToolBar.Items.FromKeyButton("btnModifica").Enabled = False
                ToolBar.Items.FromKeyButton("btnElimina").Enabled = False
                ToolBar.Items.FromKeyButton("btnSalva").Enabled = True
                ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = True

                dgrCodiciSts.Enabled = False
                dgrCodiciSts.SelectedIndex = -1

                txtFiltro.Enabled = False
                txtCodice.Enabled = True
                txtDescrizione.Enabled = True
                odpDataInizioValidita.Enabled = True
                odpDataFineValidita.Enabled = True
                omlAsl.Enabled = True
                txtIndirizzo.Enabled = True
                omlComune.Enabled = True

                txtCodice.CssClass = "TextBox_Stringa_Obbligatorio uppercase"
                txtDescrizione.CssClass = "TextBox_Stringa_Obbligatorio uppercase"

                dgrCodiciSts.SelectedIndex = 0

                ClearCampi()

        End Select

    End Sub

#Region "Eventi toolbar"

    Private Sub ToolBar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btnCerca"
                SetLayout(StatoLayout.View)
                ShowCodiciSts(0)

            Case "btnModifica"
                SetLayout(StatoLayout.Edit)

            'Case "btnElimina"
            '    SetLayout(StatoLayout.Elimina)
            '    ' metodo elimina malattia
            '    EliminaMalattia(dgrCodiciSTS.SelectedItem.Cells(GridColumnIndex.Codice).Text)

            '    Dim codiceMalattia As String = dgrCodiciSTS.SelectedItem.Cells(GridColumnIndex.Codice).Text
            '    EliminaMalattia(codiceMalattia)

            '    Dim codice As List(Of Entities.CodiceSTS) = GetCodiceSTSByCodice(codiceMalattia)

            '    txtCodice.Text = codice(0).Codice
            '    txtDescrizione.Text = codice(0).Descrizione
            '    txtDataInizioValidita.Text = codice(0).DataInizioValidita
            '    txtDataFineValidita.Text = codice(0).DataFineValidita
            '    txtCodiceASL.Text = codice(0).CodiceAsl
            '    txtIndirizzo.Text = codice(0).Indirizzo
            '    'omlComune

            '    'End If

            '    ShowCodiciSTS(GetCodiciSTS())
            '    SetLayout(StatoLayout.View)

            Case "btnNuovo"
                SetLayout(StatoLayout.Nuovo)

            Case "btnSalva"
                Dim command As New CodiceSTSCommand()

                command.Codice = txtCodice.Text.Trim.ToUpper()
                command.Descrizione = txtDescrizione.Text.Trim.ToUpper()
                command.DataInizioValidita = StringToDateTime(odpDataInizioValidita.Text.Trim)
                command.DataFineValidita = StringToDateTime(odpDataFineValidita.Text.Trim)
                command.CodiceAsl = omlAsl.Codice.Trim.ToUpper()
                command.Indirizzo = txtIndirizzo.Text.Trim.ToUpper()
                command.CodiceComune = omlComune.Codice.ToUpper()

                command.CodiceMaxLength = txtCodice.MaxLength
                command.DescrizioneMaxLength = txtDescrizione.MaxLength
                command.IndirizzoMaxLength = txtIndirizzo.MaxLength

                Select Case StatoCorrenteLayout

                    Case StatoLayout.Edit
                        command.Id = dgrCodiciSts.SelectedItem.Cells(GridColumnIndex.Id).Text
                        AggiornaCodiceSts(command)
                        SetLayout(StatoLayout.View)
                        ShowCodiciSts(dgrCodiciSts.CurrentPageIndex)

                    Case StatoLayout.Nuovo
                        Dim risAggiunta As Biz.BizGenericResult = AggiungiCodiceSts(command)

                        If risAggiunta.Success = False Then
                            OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(HttpUtility.JavaScriptStringEncode(risAggiunta.Message), "AlertMsg", False, False))
                        Else
                            SetLayout(StatoLayout.View)
                            ShowCodiciSts(dgrCodiciSts.CurrentPageIndex)
                        End If

                End Select

            Case "btnAnnulla"
                SetLayout(StatoLayout.View)

        End Select

    End Sub
#End Region

    ' metodo visualizzazione su GridView
    Private Sub ShowCodiciSts(currentPageIndex As Int32)

        dgrCodiciSts.CurrentPageIndex = currentPageIndex

        Dim result As New Biz.BizCodiciStruttura.CodiciStsResult

        Dim filtri As New Biz.BizCodiciStruttura.CodiciStsFiltri With {
                    .FiltroRicerca = txtFiltro.Text.Trim.ToUpper(),
                    .PageIndex = dgrCodiciSts.CurrentPageIndex,
                    .PageSize = dgrCodiciSts.PageSize
                }

        If String.IsNullOrWhiteSpace(filtri.FiltroRicerca) Then
            result = GetResultCodiciSts(filtri)
        Else
            result = GetResultCodiciSTSByFiltro(filtri)
        End If

        dgrCodiciSts.VirtualItemCount = result.CountCodiciSts
        dgrCodiciSts.DataSource = result.ListCodiciSts
        dgrCodiciSts.DataBind()

        If result.CountCodiciSts = 0 Then
            OnitLayout31.InsertRoutineJS("alert('Attenzione, la ricerca non ha prodotto alcun risultato!');")
            dgrCodiciSts.SelectedIndex = -1
        Else
            dgrCodiciSts.SelectedIndex = 0
            dgrCodiciSts_SelectedIndexChanged(New Object, New EventArgs)
        End If

    End Sub

#Region "Eventi input"
    Protected Sub dgrCodiciSts_SelectedIndexChanged(sender As Object, e As EventArgs) Handles dgrCodiciSts.SelectedIndexChanged

        If dgrCodiciSts.SelectedItem IsNot Nothing Then

            SetLayout(StatoLayout.RigaSelezionata)

            Dim idSelezione As String = dgrCodiciSts.SelectedItem.Cells(GridColumnIndex.Id).Text
            Dim codiceSTS As List(Of CodiceSTS) = GetCodiceStsById(idSelezione)

            txtCodice.Text = codiceSTS(0).CodiceSts
            txtDescrizione.Text = codiceSTS(0).Descrizione
            odpDataInizioValidita.Text = DateTimeToString(codiceSTS(0).DataInizioValidita)
            odpDataFineValidita.Text = DateTimeToString(codiceSTS(0).DataFineValidita)
            omlAsl.Codice = codiceSTS(0).CodiceAsl
            omlAsl.Descrizione = codiceSTS(0).DescrizioneAsl
            txtIndirizzo.Text = codiceSTS(0).Indirizzo
            omlComune.Codice = codiceSTS(0).CodiceComune
            omlComune.Descrizione = codiceSTS(0).DescrizioneComune

        Else
            txtCodice.Text = String.Empty
            txtDescrizione.Text = String.Empty
            odpDataInizioValidita.Text = String.Empty
            odpDataFineValidita.Text = String.Empty
            omlAsl.Codice = String.Empty
            omlAsl.Descrizione = String.Empty
            txtIndirizzo.Text = String.Empty
            omlComune.Codice = String.Empty
            omlComune.Descrizione = String.Empty
        End If

    End Sub

    Private Sub dgrCodiciSts_PageIndexChanged(source As Object, e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dgrCodiciSts.PageIndexChanged

        ShowCodiciSts(e.NewPageIndex)

    End Sub

#End Region

#Region "Metodi Codici STS"

    Private Function GetResultCodiciSts(filtri As Biz.BizCodiciStruttura.CodiciStsFiltri) As Biz.BizCodiciStruttura.CodiciStsResult

        Dim result As New Biz.BizCodiciStruttura.CodiciStsResult

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnag As New Biz.BizCodiciStruttura(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                result = bizAnag.GetCodiciSts(filtri)

            End Using
        End Using

        Return result

    End Function

    Private Function GetResultCodiciSTSByFiltro(filtri As Biz.BizCodiciStruttura.CodiciStsFiltri) As Biz.BizCodiciStruttura.CodiciStsResult

        Dim result As New Biz.BizCodiciStruttura.CodiciStsResult

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnag As New Biz.BizCodiciStruttura(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                result = bizAnag.GetCodiciStsByFiltro(filtri)

            End Using
        End Using

        Return result

    End Function

    Private Function GetCodiceStsById(id As String) As IList(Of CodiceSTS)

        Dim codiceSts As List(Of CodiceSTS) = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnag As New Biz.BizCodiciStruttura(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                codiceSts = bizAnag.GetDettaglioCodiceStsById(id)

            End Using
        End Using

        Return codiceSts

    End Function

    Private Sub AggiornaCodiceSts(command As CodiceSTSCommand)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnag As New Biz.BizCodiciStruttura(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                bizAnag.UpdateCodiceSts(command)
            End Using
        End Using

    End Sub

    'Private Sub EliminaMalattia(codice As String)

    '    Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
    '        Using bizAnag As New Biz.BizCodiciSTS(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
    '            bizAnag.EliminaCodiceHps(codiceMalattia)
    '        End Using
    '    End Using

    'End Sub

    Private Function AggiungiCodiceSts(command As CodiceSTSCommand) As Biz.BizGenericResult
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizAnag As New Biz.BizCodiciStruttura(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Dim result As Biz.BizGenericResult = bizAnag.AggiungiCodiceSts(command)

                Return result

            End Using

        End Using
    End Function

#End Region

End Class