Imports System.Collections.Generic
Imports Infragistics.WebUI.UltraWebToolbar
Imports Onit.OnAssistnet.OnVac.DAL

Public Class NoteAnagPaziente
    Inherits Common.PageBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                ' Impostazione titolo pagina e label dati paziente
                OnVacUtility.ImpostaIntestazioniPagina(OnitLayout31, LayoutTitolo, genericProvider, Settings, IsGestioneCentrale)

                'Caricamento note
                LoadNote()

            End Using

        End If

    End Sub



    Private Sub LoadNote()

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            Using bizPaziente As New Biz.BizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                Dim listaNote As List(Of Entities.PazienteNote) = bizPaziente.GetNotePaziente(OnVacUtility.Variabili.PazId, OnVacContext.CodiceUslCorrente)
                If listaNote.Count > 0 Then

                        rptNote.DataSource = listaNote
                        rptNote.DataBind()

                        'abilitazione dei btn della toolbar
                        Dim enable As Boolean = False
                        For Each nota As Entities.PazienteNote In listaNote
                            If nota.FlagNotaModificabile Then
                                enable = True
                                Exit For
                            End If
                        Next
                        EnableToolbarBtn(enable)

                    End If
                End Using

        End Using

    End Sub

    Private Sub EnableToolbarBtn(bool As Boolean)

        ToolbarNote.Items.FromKeyButton("btnSalva").Enabled = bool
        ToolbarNote.Items.FromKeyButton("btnAnnulla").Enabled = bool

    End Sub

    Private Sub ToolbarNote_ButtonClicked(sender As Object, be As ButtonEvent) Handles ToolbarNote.ButtonClicked
        Select Case be.Button.Key
            Case "btnSalva"
                SalvaNote()
                'viene rifatto il load delle note per caricare gli hidden field
                LoadNote()

            Case "btnAnnulla"
                LoadNote()

        End Select
    End Sub


    Private Sub SalvaNote()

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            Using bizPaziente As New Biz.BizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                For Each item As RepeaterItem In rptNote.Items
                    Dim idNota As Long? = Nothing
                    Dim idNotaValue As Long = Nothing
                    If Long.TryParse(DirectCast(item.FindControl("hdfIdNota"), HiddenField).Value, idNotaValue) Then
                        idNota = idNotaValue
                    End If
                    Dim codiceNota As String = DirectCast(item.FindControl("hdfCodiceNota"), HiddenField).Value
                    Dim txtTestoNote As TextBox = DirectCast(item.FindControl("txtTestoNote"), TextBox)

                    If txtTestoNote.Enabled Then
                        bizPaziente.UpdateNotePaziente(idNota, OnVacUtility.Variabili.PazId, OnVacContext.CodiceUslCorrente, codiceNota, txtTestoNote.Text)
                    End If
                Next

            End Using

        End Using

    End Sub

End Class