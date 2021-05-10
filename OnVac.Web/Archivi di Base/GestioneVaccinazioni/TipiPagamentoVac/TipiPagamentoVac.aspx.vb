Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.Database.DataAccessManager
Imports Onit.Controls
Imports Infragistics.WebUI.UltraWebGrid
Imports Onit.OnAssistnet.OnVac.Biz
Imports Onit.OnAssistnet.OnVac.Biz.BizNomiCommerciali

Partial Class TipiPagamentoVac
    Inherits OnVac.Common.PageBase

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

        txtDescrizione.Text = String.Empty
        txtCodiceEsterno.Text = String.Empty
        txtCodiceAvn.Text = String.Empty
        ddlImporto.SelectedIndex = 0
        ddlEsenzione.SelectedIndex = 0
        chkImportoAuto.Checked = False
        chkCondPagamento.Checked = False

    End Sub

#End Region

#Region "Types"
    Private Enum GridColumnIndex
        Guid = 1
        Descrizione = 2
        CodiceEsterno = 3
        CodiceAvn = 4
    End Enum

    Private Enum StatoLayout
        View
        Edit
        Nuovo
        Elimina
        RigaSelezionata
    End Enum
#End Region

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load, Me.Load
        If Not IsPostBack Then

            ShowTipiPagamento()
            SetLayout(StatoLayout.View)

        End If
    End Sub

    Private Sub SetLayout(stato As StatoLayout)
        StatoCorrenteLayout = stato

        Select Case stato
            Case StatoLayout.View

                OnitLayout31.Busy = False

                ToolBar.Items.FromKeyButton("btnNuovo").Enabled = True
                ToolBar.Items.FromKeyButton("btnModifica").Enabled = False
                ToolBar.Items.FromKeyButton("btnElimina").Enabled = False
                ToolBar.Items.FromKeyButton("btnSalva").Enabled = False
                ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = False

                dgrTipiPagamento.Enabled = True

                If dgrTipiPagamento.Items.Count > 0 Then
                    dgrTipiPagamento.SelectedIndex = 0
                    dgrTipiPagamento_SelectedIndexChanged(New Object, New EventArgs)
                Else
                    dgrTipiPagamento.SelectedIndex = -1
                End If

                txtDescrizione.Enabled = False
                txtCodiceEsterno.Enabled = False
                txtCodiceAvn.Enabled = False
                ddlImporto.Enabled = False
                ddlEsenzione.Enabled = False
                chkImportoAuto.Enabled = False
                chkCondPagamento.Enabled = False

                txtDescrizione.CssClass = "TextBox_Stringa uppercase"

            Case StatoLayout.RigaSelezionata

                OnitLayout31.Busy = False

                ToolBar.Items.FromKeyButton("btnNuovo").Enabled = True
                ToolBar.Items.FromKeyButton("btnModifica").Enabled = True
                ToolBar.Items.FromKeyButton("btnElimina").Enabled = True
                ToolBar.Items.FromKeyButton("btnSalva").Enabled = False
                ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = False

                dgrTipiPagamento.Enabled = True

                txtDescrizione.Enabled = False
                txtCodiceEsterno.Enabled = False
                txtCodiceAvn.Enabled = False
                ddlImporto.Enabled = False
                ddlEsenzione.Enabled = False
                chkImportoAuto.Enabled = False
                chkCondPagamento.Enabled = False

                txtDescrizione.CssClass = "TextBox_Stringa uppercase"

            Case StatoLayout.Edit

                OnitLayout31.Busy = True

                ToolBar.Items.FromKeyButton("btnNuovo").Enabled = False
                ToolBar.Items.FromKeyButton("btnModifica").Enabled = False
                ToolBar.Items.FromKeyButton("btnElimina").Enabled = False
                ToolBar.Items.FromKeyButton("btnSalva").Enabled = True
                ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = True

                dgrTipiPagamento.Enabled = False

                txtDescrizione.Enabled = True
                txtCodiceEsterno.Enabled = True
                txtCodiceAvn.Enabled = True
                ddlImporto.Enabled = True
                ddlEsenzione.Enabled = True
                chkImportoAuto.Enabled = True
                chkCondPagamento.Enabled = True

                txtDescrizione.CssClass = "TextBox_Stringa_Obbligatorio uppercase"

            Case StatoLayout.Elimina

                OnitLayout31.Busy = True

                ToolBar.Items.FromKeyButton("btnNuovo").Enabled = False
                ToolBar.Items.FromKeyButton("btnModifica").Enabled = False
                ToolBar.Items.FromKeyButton("btnElimina").Enabled = False
                ToolBar.Items.FromKeyButton("btnSalva").Enabled = True
                ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = True

                dgrTipiPagamento.Enabled = False

                txtDescrizione.Enabled = False
                txtCodiceEsterno.Enabled = False
                txtCodiceAvn.Enabled = False
                ddlImporto.Enabled = False
                ddlEsenzione.Enabled = False
                chkImportoAuto.Enabled = False
                chkCondPagamento.Enabled = False

            Case StatoLayout.Nuovo

                OnitLayout31.Busy = True

                ToolBar.Items.FromKeyButton("btnNuovo").Enabled = False
                ToolBar.Items.FromKeyButton("btnModifica").Enabled = False
                ToolBar.Items.FromKeyButton("btnElimina").Enabled = False
                ToolBar.Items.FromKeyButton("btnSalva").Enabled = True
                ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = True

                dgrTipiPagamento.Enabled = False
                dgrTipiPagamento.SelectedIndex = -1

                txtDescrizione.Enabled = True
                txtCodiceEsterno.Enabled = True
                txtCodiceAvn.Enabled = True
                ddlImporto.Enabled = True
                ddlEsenzione.Enabled = True
                chkImportoAuto.Enabled = True
                chkCondPagamento.Enabled = True

                txtDescrizione.CssClass = "TextBox_Stringa_Obbligatorio uppercase"
                ClearCampi()
        End Select

    End Sub

#Region "Eventi toolbar"

    Private Sub ToolBar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btnModifica"
                SetLayout(StatoLayout.Edit)

            Case "btnElimina"
                SetLayout(StatoLayout.Elimina)
                Dim guidPagamento
                guidPagamento = New Guid(dgrTipiPagamento.SelectedItem.Cells(GridColumnIndex.Guid).Text)

                EliminaTipoPagamento(guidPagamento)
                ShowTipiPagamento()
                SetLayout(StatoLayout.View)

            Case "btnNuovo"
                SetLayout(StatoLayout.Nuovo)

            Case "btnSalva"
                Dim command As New TipiPagamentoWeb()
                Dim result As New BizGenericResult()

                command.Descrizione = txtDescrizione.Text.Trim.ToUpper()
                command.CodiceEsterno = txtCodiceEsterno.Text.Trim.ToUpper()
                command.FlagStatoCampoImporto = ddlImporto.SelectedValue
                command.FlagStatoCampoEsenzione = ddlEsenzione.SelectedValue
                command.AutoSetImporto = chkImportoAuto.Checked
                command.HasCondizioniPagamento = chkCondPagamento.Checked
                command.CodiceAvn = txtCodiceAvn.Text.Trim.ToUpper()

                command.DescrizioneMaxLength = txtDescrizione.MaxLength
                command.CodiceEsternoMaxLength = txtCodiceEsterno.MaxLength
                command.DescrizioneMaxLength = txtDescrizione.MaxLength
                command.CodiceAvnMaxLength = txtCodiceAvn.MaxLength

                Select Case StatoCorrenteLayout

                    Case StatoLayout.Edit
                        command.Guid = New Guid(dgrTipiPagamento.SelectedItem.Cells(GridColumnIndex.Guid).Text)
                        result = AggiornaTipoPagamento(command)

                        If Not result.Success Then

                            OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(HttpUtility.JavaScriptStringEncode(result.Message), "AlertMsg", False, False))
                        Else

                            Dim currentIndex As Integer = dgrTipiPagamento.SelectedIndex
                            ShowTipiPagamento()
                            dgrTipiPagamento.SelectedIndex = currentIndex
                            dgrTipiPagamento_SelectedIndexChanged(New Object, New EventArgs)

                        End If

                    Case StatoLayout.Nuovo
                        result = AggiungiTipoPagamento(command)

                        If result.Success = False Then
                            OnitLayout31.ShowMsgBox(New PagesLayout.OnitLayout3.OnitLayoutMsgBox(HttpUtility.JavaScriptStringEncode(result.Message), "AlertMsg", False, False))

                        Else
                            SetLayout(StatoLayout.View)
                            ShowTipiPagamento()
                        End If

                End Select

            Case "btnAnnulla"

                Select Case StatoCorrenteLayout

                    Case StatoLayout.Edit
                        SetLayout(StatoLayout.RigaSelezionata)

                    Case StatoLayout.Nuovo
                        SetLayout(StatoLayout.View)

                End Select

        End Select

    End Sub
#End Region

    Private Sub ShowTipiPagamento()

        Dim pagamenti As List(Of TipiPagamentoWeb) = GetTipiPagamento()
        dgrTipiPagamento.DataSource = pagamenti
        dgrTipiPagamento.DataBind()

    End Sub

#Region "Eventi input"

    Protected Sub dgrTipiPagamento_SelectedIndexChanged(sender As Object, e As EventArgs)

        If dgrTipiPagamento.SelectedItem IsNot Nothing Then

            SetLayout(StatoLayout.RigaSelezionata)

            Dim guidSelezione
            guidSelezione = New Guid(dgrTipiPagamento.SelectedItem.Cells(GridColumnIndex.Guid).Text)

            Dim pagamento As TipiPagamentoWeb = GetDettagliTipoPagamentoByGuid(guidSelezione)

            txtDescrizione.Text = pagamento.Descrizione
            txtCodiceEsterno.Text = pagamento.CodiceEsterno
            ddlImporto.SelectedValue = pagamento.FlagStatoCampoImporto.ToString()
            ddlEsenzione.SelectedValue = pagamento.FlagStatoCampoEsenzione.ToString()
            chkImportoAuto.Checked = pagamento.AutoSetImporto
            chkCondPagamento.Checked = pagamento.HasCondizioniPagamento
            txtCodiceAvn.Text = pagamento.CodiceAvn

        End If

    End Sub

#End Region

#Region "Metodi tipiPagamento"
    Private Function GetTipiPagamento() As List(Of TipiPagamentoWeb)

        Dim list As New List(Of TipiPagamentoWeb)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnag As New BizNomiCommerciali(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                list = bizAnag.GetTipiPagamento()

            End Using
        End Using

        Return list

    End Function

    Private Function GetDettagliTipoPagamentoByGuid(guidPagamento As Guid) As TipiPagamentoWeb

        Dim pagamento As New TipiPagamentoWeb()
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnag As New BizNomiCommerciali(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                Return bizAnag.GetTipoPagamentoWebByGuid(guidPagamento)

            End Using
        End Using

    End Function

    Private Function AggiornaTipoPagamento(command As TipiPagamentoWeb) As BizGenericResult

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnag As New BizNomiCommerciali(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                Return bizAnag.UpdateTipoPagamento(command)

            End Using
        End Using

    End Function

    Private Sub EliminaTipoPagamento(guidPagamento As Guid)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnag As New BizNomiCommerciali(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                bizAnag.EliminaTipoPagamento(guidPagamento)

            End Using
        End Using

    End Sub

    Private Function AggiungiTipoPagamento(command As TipiPagamentoWeb) As BizGenericResult
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizAnag As New BizNomiCommerciali(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                Return bizAnag.AggiungiTipoPagamento(command)

            End Using

        End Using
    End Function

#End Region

End Class