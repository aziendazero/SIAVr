Imports Onit.OnAssistnet.OnVac.Biz
Imports Onit.OnAssistnet.OnVac.DAL

Public Class TestRicercaQPv2
    Inherits Common.PageBase

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub

    Private Sub btnRicerca_Click(sender As Object, e As EventArgs) Handles btnRicerca.Click

        ' ----------------------------------------------------------------------------
        ' TEST CHIAMATA API
        ' ----------------------------------------------------------------------------

        Dim qpv2Result As BizPaziente.RicercaQPv2Result

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizPaziente As New BizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                qpv2Result = bizPaziente.RicercaQPv2("123")
            End Using
        End Using

        If Not qpv2Result.Success Then
            OnitLayout31.ShowMsgBox(New Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(qpv2Result.Message, "AlertQPv2", False, False))
            Return
        End If

    End Sub

End Class