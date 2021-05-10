Imports System.Collections.Generic
Imports System.Text
Imports Onit.OnAssistnet.OnVac.Biz
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Enumerators

Public Class PagamentoVaccinazione
    Inherits OnVac.Common.UserControlPageBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then

            Dim listTipiPagamento As List(Of Entities.TipiPagamento) = Nothing

            Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                Using bizNomiCommerciali As New BizNomiCommerciali(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                    listTipiPagamento = bizNomiCommerciali.GetListTipiPagamento()

                End Using
            End Using

            ddlTipiPagVac.Items.Clear()
            Dim guidPagamentoDefault As New Guid(Settings.TIPOPAGAMENTO_DEFAULT)

            If Not listTipiPagamento Is Nothing AndAlso listTipiPagamento.Count > 0 Then
                For Each tipoPagamento As Entities.TipiPagamento In listTipiPagamento
                    If tipoPagamento.GuidPagamento <> guidPagamentoDefault Then
                        ddlTipiPagVac.Items.Add(New ListItem(tipoPagamento.Descrizione, tipoPagamento.GuidPagamento.ToString()))
                    Else
                        ddlTipiPagVac.Items.Insert(0, New ListItem(tipoPagamento.Descrizione, tipoPagamento.GuidPagamento.ToString()))
                    End If
                Next
            End If

            FillDdlEsenzioniMalattie(Me.ddlEseMalPagVac)
            SetEsenzionePagamento(StatoAbilitazioneCampo.Disabilitato, String.Empty, String.Empty)
            SetImportoPagamento(StatoAbilitazioneCampo.Disabilitato, String.Empty)

        End If

    End Sub

    Protected Sub ddlTipiPagVac_SelectedIndexChanged(sender As Object, e As EventArgs)

        Dim tipoPagamento As Entities.TipiPagamento = GetTipoPagamentoSelezionato()
        If tipoPagamento Is Nothing Then Return

        Dim valoreImporto As String = String.Empty
        SetEsenzionePagamento(tipoPagamento.FlagStatoCampoEsenzione, String.Empty, String.Empty)
        SetImportoPagamento(tipoPagamento.FlagStatoCampoImporto, String.Empty)


    End Sub

#Region " Private "

    Private Sub SetEsenzionePagamento(statoEsenzione As Enumerators.StatoAbilitazioneCampo, codiceMalattia As String, codiceEsenzione As String)
        '--
        Select Case statoEsenzione
            '--
            Case Enumerators.StatoAbilitazioneCampo.Disabilitato
                '--
                Me.ddlEseMalPagVac.ClearSelection()
                '--
                Me.ddlEseMalPagVac.CssClass = "Textbox_stringa_disabilitato"
                Me.ddlEseMalPagVac.Enabled = False
                '--
            Case Enumerators.StatoAbilitazioneCampo.Abilitato,
                 Enumerators.StatoAbilitazioneCampo.Obbligatorio
                '--
                Me.ddlEseMalPagVac.ClearSelection()
                If Not String.IsNullOrEmpty(codiceMalattia) Then
                    Me.ddlEseMalPagVac.SelectedValue = String.Format("{0}|{1}", codiceMalattia, codiceEsenzione)
                End If
                '--
                Me.ddlEseMalPagVac.CssClass = IIf(statoEsenzione = Enumerators.StatoAbilitazioneCampo.Abilitato, "Textbox_stringa", "Textbox_stringa_obbligatorio")
                Me.ddlEseMalPagVac.Enabled = True
                '--
        End Select
        '--
    End Sub

    Private Sub SetImportoPagamento(statoImporto As Enumerators.StatoAbilitazioneCampo, valoreImporto As String)
        '--
        Select Case statoImporto
            '--
            Case Enumerators.StatoAbilitazioneCampo.Disabilitato
                '--
                Me.valImpPagVac.Text = String.Empty
                '--
                Me.valImpPagVac.CssClass = "Textbox_numerico_disabilitato"
                Me.valImpPagVac.Enabled = False
                '--
            Case Enumerators.StatoAbilitazioneCampo.Abilitato,
                 Enumerators.StatoAbilitazioneCampo.Obbligatorio
                '--
                Me.valImpPagVac.Text = valoreImporto
                '--
                Me.valImpPagVac.CssClass = IIf(statoImporto = Enumerators.StatoAbilitazioneCampo.Abilitato, "Textbox_numerico", "Textbox_numerico_obbligatorio")
                Me.valImpPagVac.Enabled = True
                '--
        End Select
        '--
    End Sub

    Private Function GetTipoPagamentoSelezionato() As Entities.TipiPagamento
        Return GetTipoPagamentoSelezionato(ddlTipiPagVac.SelectedValue)
    End Function

    Private Function GetTipoPagamentoSelezionato(guidTipoPagamento As String) As Entities.TipiPagamento
        '--
        If String.IsNullOrEmpty(guidTipoPagamento) Then Return Nothing
        '--
        Dim guidTipoPagamentoSelezionato As Guid = New Guid(Me.ddlTipiPagVac.SelectedValue)
        '--
        Dim tipoPagamento As Entities.TipiPagamento = Nothing
        '--
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizNomiCommerciali As New Biz.BizNomiCommerciali(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                '--
                tipoPagamento = bizNomiCommerciali.GetTipoPagamento(guidTipoPagamentoSelezionato)
                '--
            End Using
        End Using
        '--
        Return tipoPagamento
        '--
    End Function
    Private Sub FillDdlEsenzioniMalattie(ddl As DropDownList)
        '--
        Dim malattie As New List(Of Entities.Malattia)

        ddl.Items.Clear()

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using biz As New Biz.BizMalattie(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                malattie = biz.LoadMalattieEsenzione()
            End Using
        End Using

        ddl.Items.Add(New ListItem())
        For Each val As Entities.Malattia In malattie
            ddl.Items.Add(New ListItem(val.DescrizioneMalattiaCodiceEsenzione, val.CodiceMalattiaCodiceEsenzione))
        Next

    End Sub

#End Region

#Region " Public "

    Public Function GetDatiPagamentoFromControls() As BizRegistrazioneVaccinazioni.DatiPagamentoVaccinazione

        Dim result As New BizRegistrazioneVaccinazioni.DatiPagamentoVaccinazione

        Dim codiceEsenzioneMalattia As String() = Me.ddlEseMalPagVac.SelectedValue.Split("|")

        result.GuidTipoPagamento = New Guid(ddlTipiPagVac.SelectedValue)
        If Not codiceEsenzioneMalattia Is Nothing AndAlso codiceEsenzioneMalattia.Count > 1 Then
            result.CodiceMalattia = codiceEsenzioneMalattia(0)
            result.CodiceEsenzione = codiceEsenzioneMalattia(1)
        Else
            result.CodiceMalattia = String.Empty
            result.CodiceEsenzione = String.Empty
        End If
        result.Importo = valImpPagVac.Text

        Return result

    End Function

    Public Sub SetDatiPagamentoControls(dati As BizRegistrazioneVaccinazioni.DatiPagamentoVaccinazione)

        Dim tipoPagamento As New Entities.TipiPagamento

        If Not dati Is Nothing Then
            ddlTipiPagVac.SelectedValue = dati.GuidTipoPagamento.ToString()
            tipoPagamento = GetTipoPagamentoSelezionato()
            If tipoPagamento Is Nothing Then Return
            SetEsenzionePagamento(tipoPagamento.FlagStatoCampoEsenzione, dati.CodiceMalattia, dati.CodiceEsenzione)
            SetImportoPagamento(tipoPagamento.FlagStatoCampoImporto, dati.Importo)
        Else
            ddlTipiPagVac.SelectedIndex = 0
            SetEsenzionePagamento(Enumerators.StatoAbilitazioneCampo.Disabilitato, String.Empty, String.Empty)
            SetImportoPagamento(Enumerators.StatoAbilitazioneCampo.Disabilitato, String.Empty)
        End If

    End Sub

    Public Sub SetLayout()
        ddlTipiPagVac.Enabled = True
        ddlEseMalPagVac.Enabled = False
        valImpPagVac.Enabled = False
        ddlTipiPagVac.CssClass = "textbox_stringa"
        ddlEseMalPagVac.CssClass = "textbox_stringa_disabilitato"
        valImpPagVac.CssClass = "textbox_stringa_disabilitato"
    End Sub

#End Region

End Class