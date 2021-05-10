Imports System.Collections.Generic
Imports Onit.Controls.PagesLayout


Public Class CodificaEsternaVaccinazioneAssociazione
    Inherits Common.UserControlPageBase

#Region " Types "

    Private Enum LayoutStatus
        View = 0
        Insert = 1
        Edit = 2
    End Enum

    Private Enum DgrColumnIndex
        Associazione = 0
        Obsoleto = 1
        CodiceEsterno = 2
        CodiceAssociazione = 3
    End Enum

#End Region

#Region " Properties "

    Private Property CodiceVaccinazioneCorrente() As String
        Get
            Return ViewState("VAC")
        End Get
        Set(value As String)
            ViewState("VAC") = value
        End Set
    End Property

#End Region

#Region " Eventi "

    Public Event Close()

#End Region

#Region " Eventi Controlli "

    Private Sub ToolBar_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles tlbCodifica.ButtonClicked

        Select Case be.Button.Key

            Case "btnSalva"

                Dim result As Biz.BizGenericResult = Salva()

                If Not result.Success Then
                    Me.Onitlayout31.ShowMsgBox(New OnitLayout3.OnitLayoutMsgBox(result.Message, "ERR", False, False))
                    Return
                End If

                BindData()

                RaiseEvent Close()

            Case "btnAnnulla"

                RaiseEvent Close()

        End Select

    End Sub

    Private Sub dgrCodifiche_ItemDataBound(sender As Object, e As DataGridItemEventArgs) Handles dgrCodifiche.ItemDataBound

        If e.Item.DataItem Is Nothing Then Return

        Select Case e.Item.ItemType

            Case ListItemType.Item, ListItemType.AlternatingItem, ListItemType.EditItem, ListItemType.SelectedItem

                Dim codifica As Entities.CodificaEsternaVaccinazioneAssociazione = DirectCast(e.Item.DataItem, Entities.CodificaEsternaVaccinazioneAssociazione)

                Dim lbl As Label = DirectCast(e.Item.FindControl("lblAssociazione"), Label)
                If Not lbl Is Nothing Then lbl.Text = String.Format("{0} - {1}", codifica.DescrizioneAssociazione, codifica.CodiceAssociazione)

                Dim txtCodEsterno As TextBox = DirectCast(e.Item.FindControl("txtCodiceEsterno"), TextBox)
                If Not txtCodEsterno Is Nothing Then txtCodEsterno.Text = codifica.CodiceEsterno

        End Select

    End Sub

#End Region

#Region " Public "

    Public Sub CaricaDati(codiceVaccinazione As String)

        Me.CodiceVaccinazioneCorrente = codiceVaccinazione
        Dim descrizione As String = GetDescrizioneVaccinazione()
        Me.lblTitolo.Text = String.Format("Vaccinazione: {0} - {1}", Me.CodiceVaccinazioneCorrente, descrizione)

        BindData()

    End Sub

#End Region

#Region " Private "

    Private Function GetDescrizioneVaccinazione() As String

        Dim descrizione As String = String.Empty

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizVacc As New Biz.BizVaccinazioniAnagrafica(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                descrizione = bizVacc.GetDescrizioneVaccinazione(Me.CodiceVaccinazioneCorrente)

            End Using
        End Using

        Return descrizione

    End Function

    Private Sub BindData()

        ' Caricamento dati
        Dim list As List(Of Entities.CodificaEsternaVaccinazioneAssociazione) = Nothing

        If Not String.IsNullOrEmpty(Me.CodiceVaccinazioneCorrente) Then

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizVacc As New Biz.BizVaccinazioniAnagrafica(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                    list = bizVacc.GetListCodiciEsterniVaccinazioneAssociazione(Me.CodiceVaccinazioneCorrente)

                End Using
            End Using

        End If

        ' Bind Datagrid
        Me.dgrCodifiche.DataSource = list
        Me.dgrCodifiche.DataBind()

    End Sub

    Private Function Salva() As Biz.BizGenericResult

        Dim list As New List(Of Entities.CodificaEsternaVaccinazioneAssociazione)()

        Dim maxLengthCodiceEsterno As Integer = 0

        If dgrCodifiche.Items.Count > 0 Then

            maxLengthCodiceEsterno = DirectCast(dgrCodifiche.Items(0).FindControl("txtCodiceEsterno"), TextBox).MaxLength

            For Each item As DataGridItem In dgrCodifiche.Items

                Dim codifica As Entities.CodificaEsternaVaccinazioneAssociazione = GetCodificaEsternaVaccinazioneEtaFromDettaglio(item)

                If Not codifica Is Nothing Then
                    list.Add(codifica)
                End If

            Next

        End If

        Dim result As Biz.BizGenericResult

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizVacc As New Biz.BizVaccinazioniAnagrafica(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                result = bizVacc.SaveCodificaEsternaVaccinazioneAssociazione(CodiceVaccinazioneCorrente, list, maxLengthCodiceEsterno)

            End Using
        End Using

        Return result

    End Function

    Private Function GetCodificaEsternaVaccinazioneEtaFromDettaglio(gridItem As DataGridItem) As Entities.CodificaEsternaVaccinazioneAssociazione

        Dim codiceEsterno As String = HttpUtility.HtmlDecode(DirectCast(gridItem.FindControl("txtCodiceEsterno"), TextBox).Text).Trim()

        If String.IsNullOrWhiteSpace(codiceEsterno) Then Return Nothing

        Dim item As New Entities.CodificaEsternaVaccinazioneAssociazione()
        item.CodiceVaccinazione = Me.CodiceVaccinazioneCorrente
        item.CodiceAssociazione = HttpUtility.HtmlDecode(gridItem.Cells(DgrColumnIndex.CodiceAssociazione).Text)
        item.CodiceEsterno = codiceEsterno

        Return item

    End Function

#End Region

End Class