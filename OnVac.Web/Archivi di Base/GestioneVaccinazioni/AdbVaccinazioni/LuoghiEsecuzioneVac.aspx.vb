Imports System.Collections.Generic
Imports Infragistics.WebUI.UltraWebToolbar

Public Class LuoghiEsecuzioneVac
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
            Return dgrLuoghiEsecuzioneVac.SelectedItem.Cells(GridColumnIndex.Codice).Text
        End Get
    End Property

    Private Function CodiceCampoCorrente(item As DataGridItem)
        Return item.Cells(GridModalColumnList.CodiceCampo).Text
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

#Region "Types"
    Private Enum GridColumnIndex
        Selezione = 0
        Codice = 1
        Descrizione = 2
        Tipo = 3
        Ordine = 4
        CampiObbligatori = 5
        Default_CNS = 6
    End Enum
    Private Enum GridTipoErogatore
        Selezione = 0
        Id = 1
        Codice = 2
        Descrizione = 3
    End Enum

    ' Enum colonne dgr modale
    Private Enum GridModalColumnList
        CodiceCampo = 1
    End Enum

    Private Enum StatoLayout
        View
        Edit
        Nuovo
        RigaSelezionata
    End Enum
#End Region

#Region " Page "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then

            LoadLuoghiEsecuzioneVacc()

            Dim item As New ListItem()
            item.Text = "Non definito"
            item.Value = Constants.TipoLuogoEsecuzioneVaccinazione.NonDefinito

            ddlTipo.Items.Add(item)

            item = New ListItem()
            item.Text = "0 - In azienda"
            item.Value = Constants.TipoLuogoEsecuzioneVaccinazione.InAzienda

            ddlTipo.Items.Add(item)

            item = New ListItem()
            item.Text = "1 - Fuori azienda"
            item.Value = Constants.TipoLuogoEsecuzioneVaccinazione.FuoriAzienda

            ddlTipo.Items.Add(item)

            item = New ListItem()
            item.Text = "2 - Fuori regione"
            item.Value = Constants.TipoLuogoEsecuzioneVaccinazione.FuoriRegione

            ddlTipo.Items.Add(item)

            item = New ListItem()
            item.Text = "3 - Estero"
            item.Value = Constants.TipoLuogoEsecuzioneVaccinazione.Estero

            ddlTipo.Items.Add(item)

        End If

    End Sub
#End Region

#Region " Toolbar "
    Private Sub ToolBar_ButtonClicked(sender As Object, be As ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case be.Button.Key

            Case "btnCerca"
                SetLayout(StatoLayout.View)
                LoadLuoghiEsecuzioneVacc()

            Case "btnNuovo"

                SetLayout(StatoLayout.Nuovo)
                SetDettaglio(Nothing)
                dgrLuoghiEsecuzioneVac.SelectedIndex = -1

            Case "btnModifica"

                If dgrLuoghiEsecuzioneVac.SelectedIndex >= 0 Then
                    SetLayout(StatoLayout.Edit)
                Else
                    OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Selezionare un elemento", "err", False, False))
                End If

            Case "btnElimina"
                If dgrLuoghiEsecuzioneVac.SelectedIndex >= 0 Then
                    SetLayout(StatoLayout.Edit)
                Else
                    OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Selezionare un elemento", "err", False, False))
                End If

                Elimina()
                LoadLuoghiEsecuzioneVacc()

            Case "btnSalva"
                Salva()

            Case "btnAnnulla"
                Select Case StatoCorrenteLayout

                    Case StatoLayout.Edit
                        SetLayout(StatoLayout.RigaSelezionata)
                        dgrLuoghiEsecuzioneVac_SelectedIndexChanged(New Object, New EventArgs)

                    Case StatoLayout.Nuovo
                        SetLayout(StatoLayout.View)

                End Select

            Case "btnCampiObbligatori"

                Me.OnitLayout31.Busy = True
                Me.fmCampiObbligatori.VisibileMD = True

                Dim campiObbl As New List(Of Entities.CampoObbligatorio)

                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                    Using bizAnag As New Biz.BizLuoghiEsecuzioneVaccinazioni(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                        campiObbl = bizAnag.GetCampiObbligatori()

                    End Using
                End Using

                dgrCampiObbligatori.DataSource = campiObbl
                dgrCampiObbligatori.DataBind()

                'check a seconda di quali campi obbligatori sono già settati per il luogo corrente
                For Each item As DataGridItem In dgrCampiObbligatori.Items

                    Dim codLuogo As String = CodiceLuogoCorrente
                    Dim codCampo As String = CodiceCampoCorrente(item)

                    Dim result As Boolean = False

                    Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                        Using bizAnag As New Biz.BizLuoghiEsecuzioneVaccinazioni(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                            result = bizAnag.IsCampoInLuogoCampiObbligatori(codLuogo, codCampo)

                        End Using
                    End Using

                    DirectCast(item.FindControl("chkObblPerLuogo"), CheckBox).Checked = result

                    If result Then

                        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                            Using bizAnag As New Biz.BizLuoghiEsecuzioneVaccinazioni(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                                Dim dettagliCampo As New Entities.CampoObbligLuogoVacc()
                                dettagliCampo = bizAnag.GetDettagliCampoObbligatorioLuogo(codLuogo, codCampo)

                                If dettagliCampo.DataInizioObblig.HasValue Then
                                    DirectCast(item.FindControl("dpkDataInizioValidita"), Onit.Web.UI.WebControls.Validators.OnitDatePick).Text = dettagliCampo.DataInizioObblig.Value.ToString("dd/MM/yyyy")
                                Else
                                    DirectCast(item.FindControl("dpkDataInizioValidita"), Onit.Web.UI.WebControls.Validators.OnitDatePick).Text = String.Empty
                                End If

                            End Using
                        End Using

                    End If

                    'DirectCast(item.FindControl("dpkDataInizioValidita"), Onit.Web.UI.WebControls.Validators.OnitDatePick).Enabled = result

                    'If Not result Then
                    '    DirectCast(item.FindControl("dpkDataInizioValidita"), Onit.Web.UI.WebControls.Validators.OnitDatePick).CssClass = "dataPick_Disabilitato"

                    'End If

                Next

            Case "btnTipiErogatore"
                Me.OnitLayout31.Busy = True
                Me.fmTipoErogatore.VisibileMD = True

                Dim tipiErogatore As New List(Of Entities.TipoErogatoreVacc)
                Dim linkTipoErogatoreLuogo As New List(Of Entities.TipoErogatoreVacc)

                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                    Using bizErog As New Biz.BizErogatori(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                        tipiErogatore = bizErog.GetTipiErogatori()
                        linkTipoErogatoreLuogo = bizErog.GetTipiErogatoriFromLuogoEsecuzione(CodiceLuogoCorrente)

                    End Using
                End Using
                dgrTipoErogatore.DataSource = tipiErogatore
                dgrTipoErogatore.DataBind()
                For Each item As DataGridItem In dgrTipoErogatore.Items

                    If linkTipoErogatoreLuogo.Select(Function(f) f.Id).Contains(item.Cells(GridTipoErogatore.Id).Text) Then
                        DirectCast(item.FindControl("chkTipoErogatorePerLuogo"), CheckBox).Checked = True
                    Else
                        DirectCast(item.FindControl("chkTipoErogatorePerLuogo"), CheckBox).Checked = False
                    End If
                Next

        End Select

    End Sub

    Private Sub tlbCampiObbligatori_ButtonClicked(sender As Object, be As ButtonEvent) Handles tlbCampiObbligatori.ButtonClicked

        Select Case be.Button.Key

            Case "btnSalvaCampiObbligatori"

                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                    Using bizAnag As New Biz.BizLuoghiEsecuzioneVaccinazioni(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                        bizAnag.DeleteCampiObbligatoriLuogo(CodiceLuogoCorrente)

                    End Using
                End Using

                Dim campiObbligatori As New List(Of Entities.CampoObbligLuogoVacc)

                For Each item As DataGridItem In dgrCampiObbligatori.Items

                    If DirectCast(item.FindControl("chkObblPerLuogo"), CheckBox).Checked Then

                        Dim luogoCampoObbl As New Entities.CampoObbligLuogoVacc()
                        Dim dataInvioVal As String = DirectCast(item.FindControl("dpkDataInizioValidita"), Onit.Web.UI.WebControls.Validators.OnitDatePick).Text

                        luogoCampoObbl.CodLuogo = CodiceLuogoCorrente
                        luogoCampoObbl.CodCampo = CodiceCampoCorrente(item)

                        If Not String.IsNullOrWhiteSpace(dataInvioVal) Then
                            luogoCampoObbl.DataInizioObblig = dataInvioVal

                        End If

                        campiObbligatori.Add(luogoCampoObbl)
                    End If

                Next

                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                    Using bizAnag As New Biz.BizLuoghiEsecuzioneVaccinazioni(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                        bizAnag.InsertCampiObbligatoriLuogo(campiObbligatori)

                    End Using
                End Using

                Me.OnitLayout31.Busy = False
                Me.fmCampiObbligatori.VisibileMD = False

            Case "btnAnnullaCampiObbligatori"
                Me.OnitLayout31.Busy = False
                Me.fmCampiObbligatori.VisibileMD = False

        End Select

    End Sub

    Private Sub tlbTipoErogatore_ButtonClicked(sender As Object, be As ButtonEvent) Handles tlbTipoErogatore.ButtonClicked
        Select Case be.Button.Key
            Case "btnSalvaTipoErogatore"

                Dim listIdTipoErogatore As New List(Of Integer)

                For Each item As DataGridItem In dgrTipoErogatore.Items

                    If DirectCast(item.FindControl("chkTipoErogatorePerLuogo"), CheckBox).Checked Then
                        listIdTipoErogatore.Add(item.Cells(GridTipoErogatore.Id).Text)
                    End If

                Next

                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                    Using biz As New Biz.BizErogatori(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                        biz.DeleteTipoErogatoreFromLuogo(CodiceLuogoCorrente)
                        biz.InsertLinkTipiErogatoreLuogo(listIdTipoErogatore, CodiceLuogoCorrente)
                    End Using
                End Using
            Case "btnAnnullaTipoErogatore"
        End Select

        dgrTipoErogatore.DataSource = Nothing
        dgrTipoErogatore.DataBind()

        OnitLayout31.Busy = False
        fmTipoErogatore.VisibileMD = False
    End Sub

#End Region

    Private Sub LoadLuoghiEsecuzioneVacc()

        Dim list As List(Of Entities.LuoghiEsecuzioneVaccinazioni) = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnag As New Biz.BizLuoghiEsecuzioneVaccinazioni(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                list = bizAnag.GetLuoghiEsecuzioneVaccinazioni()

            End Using
        End Using

        dgrLuoghiEsecuzioneVac.DataSource = list
        dgrLuoghiEsecuzioneVac.DataBind()

        If list.Count > 0 Then
            dgrLuoghiEsecuzioneVac.SelectedIndex = 0
            SetLayout(StatoLayout.RigaSelezionata)

            Dim luogoEsecuzioneVaccinazione As Entities.LuoghiEsecuzioneVaccinazioni

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizAnag As New Biz.BizLuoghiEsecuzioneVaccinazioni(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                    Dim id As String = dgrLuoghiEsecuzioneVac.SelectedItem.Cells(GridColumnIndex.Codice).Text

                    luogoEsecuzioneVaccinazione = bizAnag.GetLuogoEsecuzioneVaccinazione(id)

                End Using
            End Using

            SetDettaglio(luogoEsecuzioneVaccinazione)
        Else
            SetDettaglio(Nothing)
        End If

    End Sub

#Region " Datagrid "
    Protected Sub dgrLuoghiEsecuzioneVac_SelectedIndexChanged(sender As Object, e As EventArgs)

        SetLayout(StatoLayout.RigaSelezionata)
        txtCodice.Text = String.Empty
        txtDescrizione.Text = String.Empty
        txtOrdine.Text = String.Empty

        Dim luogoEsecuzioneVaccinazione As Entities.LuoghiEsecuzioneVaccinazioni

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnag As New Biz.BizLuoghiEsecuzioneVaccinazioni(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                Dim id As String = dgrLuoghiEsecuzioneVac.SelectedItem.Cells(GridColumnIndex.Codice).Text

                luogoEsecuzioneVaccinazione = bizAnag.GetLuogoEsecuzioneVaccinazione(id)

            End Using
        End Using

        SetDettaglio(luogoEsecuzioneVaccinazione)

    End Sub

#End Region
    Private Sub SetDettaglio(luogoEsecuzioneVaccinazione As Entities.LuoghiEsecuzioneVaccinazioni)

        If luogoEsecuzioneVaccinazione Is Nothing Then

            txtCodice.Text = String.Empty
            txtDescrizione.Text = String.Empty
            txtOrdine.Text = String.Empty
            ddlTipo.SelectedValue = Constants.TipoLuogoEsecuzioneVaccinazione.NonDefinito
            txtOrdine.Text = String.Empty
            chkObsoleto.Checked = False
            chkEstraiAvn.Checked = True

        Else

            txtCodice.Text = luogoEsecuzioneVaccinazione.Codice
            txtDescrizione.Text = luogoEsecuzioneVaccinazione.Descrizione
            ddlTipo.Text = luogoEsecuzioneVaccinazione.Tipo

            If luogoEsecuzioneVaccinazione.Ordine.HasValue Then
                txtOrdine.Text = luogoEsecuzioneVaccinazione.Ordine
            End If

            If luogoEsecuzioneVaccinazione.Obsoleto = "N" Then
                chkObsoleto.Checked = False
            Else
                chkObsoleto.Checked = True
            End If

            If luogoEsecuzioneVaccinazione.FlagEstraiAvn = "S" Then
                chkEstraiAvn.Checked = True
            Else
                chkEstraiAvn.Checked = False
            End If

        End If

    End Sub

    Private Sub SetLayout(stato As StatoLayout)

        StatoCorrenteLayout = stato
        Select Case stato
            Case StatoLayout.View

                OnitLayout31.Busy = False

                ToolBar.Items.FromKeyButton("btnCerca").Enabled = True
                ToolBar.Items.FromKeyButton("btnNuovo").Enabled = True
                ToolBar.Items.FromKeyButton("btnModifica").Enabled = True
                ToolBar.Items.FromKeyButton("btnElimina").Enabled = True
                ToolBar.Items.FromKeyButton("btnSalva").Enabled = False
                ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = False
                ToolBar.Items.FromKeyButton("btnCampiObbligatori").Enabled = False

                dgrLuoghiEsecuzioneVac.Enabled = True

                If dgrLuoghiEsecuzioneVac.Items.Count > 0 Then
                    dgrLuoghiEsecuzioneVac.SelectedIndex = 0
                    dgrLuoghiEsecuzioneVac_SelectedIndexChanged(New Object, New EventArgs)
                Else
                    dgrLuoghiEsecuzioneVac.SelectedIndex = -1
                End If

                txtCodice.Enabled = False
                txtDescrizione.Enabled = False
                ddlTipo.Enabled = False
                txtOrdine.Enabled = False
                chkObsoleto.Enabled = False
                chkEstraiAvn.Enabled = False

                txtCodice.CssClass = "TextBox_Stringa uppercase"
                txtDescrizione.CssClass = "TextBox_Stringa uppercase"
                ddlTipo.CssClass = "TextBox_Stringa_disabilitato"
            Case StatoLayout.Edit

                OnitLayout31.Busy = True

                ToolBar.Items.FromKeyButton("btnCerca").Enabled = False
                ToolBar.Items.FromKeyButton("btnNuovo").Enabled = False
                ToolBar.Items.FromKeyButton("btnModifica").Enabled = False
                ToolBar.Items.FromKeyButton("btnElimina").Enabled = False
                ToolBar.Items.FromKeyButton("btnSalva").Enabled = True
                ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = True
                ToolBar.Items.FromKeyButton("btnCampiObbligatori").Enabled = False

                dgrLuoghiEsecuzioneVac.Enabled = False

                txtCodice.Enabled = False
                txtDescrizione.Enabled = True
                ddlTipo.Enabled = True
                txtOrdine.Enabled = True
                chkObsoleto.Enabled = True
                chkEstraiAvn.Enabled = True

                txtCodice.CssClass = String.Empty
                txtDescrizione.CssClass = "TextBox_Stringa_Obbligatorio uppercase"
                chkObsoleto.CssClass = "TextBox_Stringa_Obbligatorio uppercase"
                ddlTipo.CssClass = "TextBox_Stringa uppercase"
            Case StatoLayout.Nuovo

                OnitLayout31.Busy = True

                ToolBar.Items.FromKeyButton("btnCerca").Enabled = False
                ToolBar.Items.FromKeyButton("btnNuovo").Enabled = False
                ToolBar.Items.FromKeyButton("btnModifica").Enabled = False
                ToolBar.Items.FromKeyButton("btnElimina").Enabled = False
                ToolBar.Items.FromKeyButton("btnSalva").Enabled = True
                ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = True
                ToolBar.Items.FromKeyButton("btnCampiObbligatori").Enabled = False

                dgrLuoghiEsecuzioneVac.Enabled = False

                txtCodice.Enabled = True
                txtDescrizione.Enabled = True
                ddlTipo.Enabled = True
                txtOrdine.Enabled = True
                chkObsoleto.Enabled = True
                chkEstraiAvn.Enabled = True

                txtCodice.CssClass = "TextBox_Stringa_Obbligatorio uppercase"
                txtDescrizione.CssClass = "TextBox_Stringa_Obbligatorio uppercase"
                chkObsoleto.CssClass = "TextBox_Stringa_Obbligatorio uppercase"
                ddlTipo.CssClass = "TextBox_Stringa uppercase"

            Case StatoLayout.RigaSelezionata

                OnitLayout31.Busy = False

                ToolBar.Items.FromKeyButton("btnCerca").Enabled = True
                ToolBar.Items.FromKeyButton("btnNuovo").Enabled = True
                ToolBar.Items.FromKeyButton("btnModifica").Enabled = True
                ToolBar.Items.FromKeyButton("btnElimina").Enabled = True
                ToolBar.Items.FromKeyButton("btnSalva").Enabled = False
                ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = False
                ToolBar.Items.FromKeyButton("btnCampiObbligatori").Enabled = True

                dgrLuoghiEsecuzioneVac.Enabled = True

                txtCodice.Enabled = False
                txtDescrizione.Enabled = False
                ddlTipo.Enabled = False
                txtOrdine.Enabled = False
                chkObsoleto.Enabled = False
                chkEstraiAvn.Enabled = False

                txtCodice.CssClass = "TextBox_Stringa uppercase"
                txtDescrizione.CssClass = "TextBox_Stringa uppercase"
                ddlTipo.CssClass = "TextBox_Stringa_disabilitato"
        End Select

    End Sub

    Private Sub Salva()

        Dim command As New Entities.LuoghiEsecuzioneVaccCommand()

        command.Codice = txtCodice.Text.Trim()
        command.Descrizione = txtDescrizione.Text.Trim()
        command.Tipo = ddlTipo.Text.Trim()

        If Not String.IsNullOrWhiteSpace(txtOrdine.Text) Then
            command.Ordine = txtOrdine.Text
        End If

        If chkObsoleto.Checked Then
            command.Obsoleto = "S"
        Else
            command.Obsoleto = "N"
        End If

        If chkEstraiAvn.Checked Then
            command.FlagEstraiAvn = "S"
        Else
            command.FlagEstraiAvn = "N"
        End If

        command.CodiceMaxLength = txtCodice.MaxLength
        command.DescrizioneMaxLength = txtDescrizione.MaxLength
        command.OrdineMaxLength = txtOrdine.MaxLength


        Dim result As Biz.BizGenericResult = Nothing
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnag As New Biz.BizLuoghiEsecuzioneVaccinazioni(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If StatoCorrenteLayout = StatoLayout.Nuovo Then
                    result = bizAnag.InsertLuoghiEsecuzioneVaccinazioni(command)
                ElseIf StatoCorrenteLayout = StatoLayout.Edit Then
                    result = bizAnag.UpdateLuoghiEsecuzioneVaccinazioni(command)
                End If

            End Using

            If result.Success Then
                SetLayout(StatoLayout.View)
                LoadLuoghiEsecuzioneVacc()
            Else
                OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(HttpUtility.JavaScriptStringEncode("Salvataggio non effettuato: " + result.Message), "err", False, False))
            End If
        End Using

    End Sub

    Private Sub Elimina()

        Dim codice As String = dgrLuoghiEsecuzioneVac.SelectedItem.Cells(GridColumnIndex.Codice).Text

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnag As New Biz.BizLuoghiEsecuzioneVaccinazioni(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If bizAnag.DeleteCampiObbligatoriLuogo(codice).Success AndAlso bizAnag.DeleteLuoghiEsecuzioneVaccinazioni(codice).Success Then
                    SetLayout(StatoLayout.View)
                    LoadLuoghiEsecuzioneVacc()
                Else
                    OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(HttpUtility.JavaScriptStringEncode("Selezionare un elemento"), "err", False, False))
                End If

            End Using
        End Using

    End Sub

End Class