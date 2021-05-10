Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.Entities

Public Class CodiciHSP
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

                dgrCodiciHsp.Enabled = True
                dgrCodiciHsp.SelectedIndex = -1

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

                dgrCodiciHsp.Enabled = False

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

                dgrCodiciHsp.Enabled = False

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

                dgrCodiciHsp.Enabled = False
                dgrCodiciHsp.SelectedIndex = -1

                txtFiltro.Enabled = False
                txtCodice.Enabled = True
                txtDescrizione.Enabled = True
                odpDataInizioValidita.Enabled = True
                odpDataFineValidita.Enabled = True
                omlAsl.Enabled = True
                txtIndirizzo.Enabled = True
                omlComune.Enabled = True

                dgrCodiciHsp.SelectedIndex = 0

                txtCodice.CssClass = "TextBox_Stringa_Obbligatorio uppercase"
                txtDescrizione.CssClass = "TextBox_Stringa_Obbligatorio uppercase"

                ClearCampi()

        End Select

    End Sub

#Region "Eventi toolbar"

    Private Sub ToolBar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btnCerca"
                SetLayout(StatoLayout.View)
                ShowCodiciHsp(0)

            Case "btnModifica"
                SetLayout(StatoLayout.Edit)

            'Case "btnElimina"
            '    SetLayout(StatoLayout.Elimina)
            '    ' metodo elimina malattia
            '    EliminaMalattia(dgrCodiciHsp.SelectedItem.Cells(GridColumnIndex.Codice).Text)

            '    Dim codiceMalattia As String = dgrCodiciHsp.SelectedItem.Cells(GridColumnIndex.Codice).Text
            '    EliminaMalattia(codiceMalattia)

            '    Dim codice As List(Of Entities.CodiceHSP) = GetCodiceHspByCodice(codiceMalattia)

            '    txtCodice.Text = codice(0).Codice
            '    txtDescrizione.Text = codice(0).Descrizione
            '    txtDataInizioValidita.Text = codice(0).DataInizioValidita
            '    txtDataFineValidita.Text = codice(0).DataFineValidita
            '    txtCodiceASL.Text = codice(0).CodiceAsl
            '    txtIndirizzo.Text = codice(0).Indirizzo
            '    'omlComune

            '    'End If

            '    ShowCodiciHsp(GetCodiciHsp())
            '    SetLayout(StatoLayout.View)

            Case "btnNuovo"
                SetLayout(StatoLayout.Nuovo)

            Case "btnSalva"
                Dim command As New CodiceHSPCommand()

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
                        command.Id = dgrCodiciHsp.SelectedItem.Cells(GridColumnIndex.Id).Text
                        AggiornaCodiceHsp(command)
                        SetLayout(StatoLayout.View)
                        ShowCodiciHsp(dgrCodiciHsp.CurrentPageIndex)

                    Case StatoLayout.Nuovo
                        Dim risAggiunta As Biz.BizGenericResult = AggiungiCodiceHsp(command)

                        If risAggiunta.Success = False Then
                            OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(HttpUtility.JavaScriptStringEncode(risAggiunta.Message), "AlertMsg", False, False))
                        Else
                            SetLayout(StatoLayout.View)
                            ShowCodiciHsp(dgrCodiciHsp.CurrentPageIndex)
                        End If

                End Select

            Case "btnAnnulla"
                SetLayout(StatoLayout.View)

        End Select

    End Sub
#End Region

    ' metodo visualizzazione su GridView
    Private Sub ShowCodiciHsp(currentPageIndex As Int32)

        dgrCodiciHsp.CurrentPageIndex = currentPageIndex

        Dim result As New Biz.BizCodiciStruttura.CodiciHspResult

        Dim filtri As New Biz.BizCodiciStruttura.CodiciHspFiltri With {
                    .FiltroRicerca = txtFiltro.Text.Trim.ToUpper(),
                    .PageIndex = dgrCodiciHsp.CurrentPageIndex,
                    .PageSize = dgrCodiciHsp.PageSize
                }

        If String.IsNullOrWhiteSpace(filtri.FiltroRicerca) Then
            result = GetResultCodiciHsp(filtri)
        Else
            result = GetResultCodiciHspByFiltro(filtri)
        End If

        dgrCodiciHsp.VirtualItemCount = result.CountCodiciHsp
        dgrCodiciHsp.DataSource = result.ListCodiciHsp
        dgrCodiciHsp.DataBind()

        If result.CountCodiciHsp = 0 Then
            OnitLayout31.InsertRoutineJS("alert('Attenzione, la ricerca non ha prodotto alcun risultato!');")
            dgrCodiciHsp.SelectedIndex = -1
        Else
            dgrCodiciHsp.SelectedIndex = 0
            dgrCodiciHsp_SelectedIndexChanged(New Object, New EventArgs)
        End If

    End Sub

#Region "Eventi input"
    Public Sub dgrCodiciHsp_SelectedIndexChanged(sender As Object, e As EventArgs) Handles dgrCodiciHsp.SelectedIndexChanged

        If dgrCodiciHsp.SelectedItem IsNot Nothing Then

            SetLayout(StatoLayout.RigaSelezionata)

            Dim idSelezione As String = dgrCodiciHsp.SelectedItem.Cells(GridColumnIndex.Id).Text
            Dim codiceHsp As List(Of CodiceHSP) = GetCodiceHspById(idSelezione)

            txtCodice.Text = codiceHsp(0).CodiceHsp
            txtDescrizione.Text = codiceHsp(0).Descrizione
            odpDataInizioValidita.Text = DateTimeToString(codiceHsp(0).DataInizioValidita)
            odpDataFineValidita.Text = DateTimeToString(codiceHsp(0).DataFineValidita)
            omlAsl.Codice = codiceHsp(0).CodiceAsl
            omlAsl.Descrizione = codiceHsp(0).DescrizioneAsl
            txtIndirizzo.Text = codiceHsp(0).Indirizzo
            omlComune.Codice = codiceHsp(0).CodiceComune
            omlComune.Descrizione = codiceHsp(0).DescrizioneComune

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

    Private Sub dgrCodiciHsp_PageIndexChanged(source As Object, e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dgrCodiciHsp.PageIndexChanged

        ShowCodiciHsp(e.NewPageIndex)

    End Sub

#End Region

#Region "Metodi codici HSP"

    Private Function GetResultCodiciHsp(filtri As Biz.BizCodiciStruttura.CodiciHspFiltri) As Biz.BizCodiciStruttura.CodiciHspResult

        Dim result As New Biz.BizCodiciStruttura.CodiciHspResult

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnag As New Biz.BizCodiciStruttura(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                result = bizAnag.GetCodiciHsp(filtri)

            End Using
        End Using

        Return result

    End Function

    Private Function GetResultCodiciHspByFiltro(filtri As Biz.BizCodiciStruttura.CodiciHspFiltri) As Biz.BizCodiciStruttura.CodiciHspResult

        Dim result As New Biz.BizCodiciStruttura.CodiciHspResult

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnag As New Biz.BizCodiciStruttura(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                result = bizAnag.GetCodiciHspByFiltro(filtri)

            End Using
        End Using

        Return result

    End Function

    Private Function GetCodiceHspById(id As String) As IList(Of CodiceHSP)

        Dim codiceHsp As List(Of CodiceHSP) = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnag As New Biz.BizCodiciStruttura(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                codiceHsp = bizAnag.GetDettaglioCodiceHspById(id)

            End Using
        End Using

        Return codiceHsp

    End Function

    Private Sub AggiornaCodiceHsp(command As CodiceHSPCommand)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnag As New Biz.BizCodiciStruttura(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                bizAnag.UpdateCodiceHps(command)
            End Using
        End Using

    End Sub

    'Private Sub EliminaMalattia(codice As String)

    '    Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
    '        Using bizAnag As New Biz.BizCodiciHsp(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
    '            bizAnag.EliminaCodiceHps(codiceMalattia)
    '        End Using
    '    End Using

    'End Sub

    Private Function AggiungiCodiceHsp(command As CodiceHSPCommand) As Biz.BizGenericResult
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizAnag As New Biz.BizCodiciStruttura(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Dim result As Biz.BizGenericResult = bizAnag.AggiungiCodiceHsp(command)

                Return result

            End Using

        End Using
    End Function

#End Region

End Class