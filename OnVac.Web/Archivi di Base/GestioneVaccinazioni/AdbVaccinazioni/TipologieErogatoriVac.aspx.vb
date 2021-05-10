Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.Entities

Public Class TipologieErogatoriVac
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

    Private ReadOnly Property CodiceLuogoCorrente As String
        Get
            Return dgrTipiErogatoriVac.SelectedItem.Cells(GridColumnIndex.Codice).Text
        End Get
    End Property

    Private Sub ClearCampi()

        txtCodice.Text = String.Empty
        txtDescrizione.Text = String.Empty
        txtCodiceAvn.Text = String.Empty
        txtOrdine.Text = String.Empty
        chkObsoleto.Checked = False

    End Sub

#End Region

#Region "Types"

    Private Enum GridColumnIndex
        Selezione = 0
        Id = 1
        Codice = 2
        Descrizione = 3
        Ordine = 4
        CodiceAvn = 5
        Obsoleto = 6
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

    Protected Function GetStringBoolean(value As Object) As String

        Select Case value.ToString()
            Case "S"
                Return "SI"

            Case "N"
                Return "NO"

            Case Else
                Return String.Empty
        End Select

    End Function

#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then

            ShowTipiErogatori()
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

                dgrTipiErogatoriVac.Enabled = True

                If dgrTipiErogatoriVac.Items.Count > 0 Then
                    dgrTipiErogatoriVac.SelectedIndex = 0
                    dgrTipiErogatoriVac_SelectedIndexChanged(New Object, New EventArgs)
                Else
                    dgrTipiErogatoriVac.SelectedIndex = -1
                End If

                txtCodice.Enabled = False
                txtDescrizione.Enabled = False
                txtCodiceAvn.Enabled = False
                txtOrdine.Enabled = False
                chkObsoleto.Enabled = False

                txtCodice.CssClass = "TextBox_Stringa uppercase"
                txtDescrizione.CssClass = "TextBox_Stringa uppercase"

            Case StatoLayout.RigaSelezionata

                OnitLayout31.Busy = False

                ToolBar.Items.FromKeyButton("btnCerca").Enabled = True
                ToolBar.Items.FromKeyButton("btnNuovo").Enabled = True
                ToolBar.Items.FromKeyButton("btnModifica").Enabled = True
                ToolBar.Items.FromKeyButton("btnElimina").Enabled = True
                ToolBar.Items.FromKeyButton("btnSalva").Enabled = False
                ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = False

                dgrTipiErogatoriVac.Enabled = True

                txtCodice.Enabled = False
                txtDescrizione.Enabled = False
                txtCodiceAvn.Enabled = False
                txtOrdine.Enabled = False
                chkObsoleto.Enabled = False

                txtCodice.CssClass = "TextBox_Stringa uppercase"
                txtDescrizione.CssClass = "TextBox_Stringa uppercase"

            Case StatoLayout.Edit

                OnitLayout31.Busy = True

                ToolBar.Items.FromKeyButton("btnCerca").Enabled = False
                ToolBar.Items.FromKeyButton("btnNuovo").Enabled = False
                ToolBar.Items.FromKeyButton("btnModifica").Enabled = False
                ToolBar.Items.FromKeyButton("btnElimina").Enabled = False
                ToolBar.Items.FromKeyButton("btnSalva").Enabled = True
                ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = True

                dgrTipiErogatoriVac.Enabled = False

                txtCodice.Enabled = False
                txtDescrizione.Enabled = True
                txtCodiceAvn.Enabled = True
                txtOrdine.Enabled = True
                chkObsoleto.Enabled = True

                txtDescrizione.CssClass = "TextBox_Stringa_Obbligatorio uppercase"

            Case StatoLayout.Elimina

                OnitLayout31.Busy = True

                ToolBar.Items.FromKeyButton("btnCerca").Enabled = False
                ToolBar.Items.FromKeyButton("btnNuovo").Enabled = False
                ToolBar.Items.FromKeyButton("btnModifica").Enabled = False
                ToolBar.Items.FromKeyButton("btnElimina").Enabled = False
                ToolBar.Items.FromKeyButton("btnSalva").Enabled = True
                ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = True

                dgrTipiErogatoriVac.Enabled = False

                txtCodice.Enabled = False
                txtDescrizione.Enabled = False
                txtCodiceAvn.Enabled = False
                txtOrdine.Enabled = False
                chkObsoleto.Enabled = False

            Case StatoLayout.Nuovo

                OnitLayout31.Busy = True

                ToolBar.Items.FromKeyButton("btnCerca").Enabled = False
                ToolBar.Items.FromKeyButton("btnNuovo").Enabled = False
                ToolBar.Items.FromKeyButton("btnModifica").Enabled = False
                ToolBar.Items.FromKeyButton("btnElimina").Enabled = False
                ToolBar.Items.FromKeyButton("btnSalva").Enabled = True
                ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = True

                dgrTipiErogatoriVac.Enabled = False
                dgrTipiErogatoriVac.SelectedIndex = -1

                txtCodice.Enabled = True
                txtDescrizione.Enabled = True
                txtCodiceAvn.Enabled = True
                txtOrdine.Enabled = True
                chkObsoleto.Enabled = True

                txtCodice.CssClass = "TextBox_Stringa_Obbligatorio uppercase"
                txtDescrizione.CssClass = "TextBox_Stringa_Obbligatorio uppercase"

                ClearCampi()

        End Select

    End Sub

#Region "Eventi toolbar"

    Private Sub ToolBar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btnCerca"
                ShowTipiErogatori()
                SetLayout(StatoLayout.View)

            Case "btnModifica"
                SetLayout(StatoLayout.Edit)

            Case "btnElimina"
                SetLayout(StatoLayout.Elimina)
                EliminaTipoErogatore(dgrTipiErogatoriVac.SelectedItem.Cells(GridColumnIndex.Id).Text)

                ShowTipiErogatori()
                SetLayout(StatoLayout.View)

            Case "btnNuovo"
                SetLayout(StatoLayout.Nuovo)

            Case "btnSalva"

                Dim command As New Biz.TipoErogatoreWebCommand()
                Dim result As New Biz.BizGenericResult()

                command.Codice = txtCodice.Text
                command.Descrizione = txtDescrizione.Text
                command.CodiceAvn = txtCodiceAvn.Text
                command.Ordine = txtOrdine.Text
                command.Obsoleto = chkObsoleto.Checked

                command.CodiceMaxLength = txtCodice.MaxLength
                command.DescrizioneMaxLength = txtDescrizione.MaxLength
                command.CodiceAvnMaxLength = txtCodiceAvn.MaxLength
                command.OrdineMaxLength = txtOrdine.MaxLength

                Select Case StatoCorrenteLayout

                    Case StatoLayout.Edit
                        command.Id = dgrTipiErogatoriVac.SelectedItem.Cells(GridColumnIndex.Id).Text
                        result = AggiornaTipoErogatore(command)

                        If Not result.Success Then

                            OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(HttpUtility.JavaScriptStringEncode(result.Message), "AlertMsg", False, False))
                        Else

                            ShowTipiErogatori()
                            dgrTipiErogatoriVac.SelectedIndex = 0
                            dgrTipiErogatoriVac_SelectedIndexChanged(New Object, New EventArgs)

                        End If

                    Case StatoLayout.Nuovo
                        result = AggiungiTipoErogatore(command)
                        If Not result.Success Then

                            OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(HttpUtility.JavaScriptStringEncode(result.Message), "AlertMsg", False, False))
                        Else
                            ShowTipiErogatori()
                            SetLayout(StatoLayout.View)
                        End If

                End Select

            Case "btnAnnulla"

                Select Case StatoCorrenteLayout

                    Case StatoLayout.Edit
                        SetLayout(StatoLayout.RigaSelezionata)
                        dgrTipiErogatoriVac_SelectedIndexChanged(New Object, New EventArgs)

                    Case StatoLayout.Nuovo
                        SetLayout(StatoLayout.View)

                End Select

        End Select

    End Sub

#End Region

#Region "Eventi input"

    Public Sub dgrTipiErogatoriVac_SelectedIndexChanged(sender As Object, e As EventArgs) Handles dgrTipiErogatoriVac.SelectedIndexChanged

        If dgrTipiErogatoriVac.SelectedItem IsNot Nothing Then

            SetLayout(StatoLayout.RigaSelezionata)

            Dim idSelezione As String = dgrTipiErogatoriVac.SelectedItem.Cells(GridColumnIndex.Id).Text
            Dim tipoErogatore As TipoErogatoreVacc = GetTipoErogatoreById(idSelezione)

            txtCodice.Text = tipoErogatore.Codice
            txtDescrizione.Text = tipoErogatore.Descrizione
            txtCodiceAvn.Text = tipoErogatore.CodiceAvn
            txtOrdine.Text = tipoErogatore.Ordine.ToString()

            If tipoErogatore.Obsoleto = "N" Then
                chkObsoleto.Checked = False
            Else
                chkObsoleto.Checked = True
            End If

        Else
            txtCodice.Text = String.Empty
            txtDescrizione.Text = String.Empty
            txtCodiceAvn.Text = String.Empty
            txtOrdine.Text = String.Empty
            chkObsoleto.Checked = False

        End If

    End Sub

#End Region

    ''' <summary>
    ''' Esegue il bind del datagrid e imposta la riga selezionata 
    ''' </summary>
    Private Sub ShowTipiErogatori()

        dgrTipiErogatoriVac.DataSource = GetTipiErogatori()
        dgrTipiErogatoriVac.DataBind()

    End Sub

    Private Function GetTipiErogatori() As List(Of TipoErogatoreVacc)

        Dim result As New List(Of TipoErogatoreVacc)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnag As New Biz.BizErogatori(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                result = bizAnag.GetTipiErogatori()

            End Using
        End Using

        Return result

    End Function

    Private Function GetTipoErogatoreById(id As Integer) As TipoErogatoreVacc

        Dim tipoErogatore As New TipoErogatoreVacc

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnag As New Biz.BizErogatori(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                tipoErogatore = bizAnag.GetDettaglioTipoErogatore(id)

            End Using
        End Using

        Return tipoErogatore

    End Function

    Private Function AggiungiTipoErogatore(command As Biz.TipoErogatoreWebCommand) As Biz.BizGenericResult

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizAnag As New Biz.BizErogatori(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Dim result As Biz.BizGenericResult = bizAnag.InsertTipoErogatore(command)
                Return result

            End Using

        End Using

    End Function

    Private Function AggiornaTipoErogatore(command As Biz.TipoErogatoreWebCommand) As Biz.BizGenericResult

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizAnag As New Biz.BizErogatori(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Dim result As Biz.BizGenericResult = bizAnag.UpdateTipoErogatore(command)
                Return result

            End Using

        End Using

    End Function

    Private Sub EliminaTipoErogatore(id As Integer)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizAnag As New Biz.BizErogatori(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                bizAnag.DeleteTipoErogatore(id)

            End Using

        End Using

    End Sub

End Class