Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.DAL

Partial Class NotePaziente
    Inherits Common.UserControlFinestraModalePageBase

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

#Region " Events "

    Public Event ChiudiNote()

    Protected Sub OnChiudiNote()
        RaiseEvent ChiudiNote()
    End Sub

#End Region

#Region " Properties "

    Public Property HasReazioniAvvese() As Boolean
        Get
            Return ReazAvverse.Visible
        End Get
        Set(ByVal Value As Boolean)
            ReazAvverse.Visible = Value
        End Set
    End Property

#End Region

#Region " Public "

    Public Overrides Sub LoadModale()

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            Using bizPaziente As New Biz.BizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                Dim listaNote As List(Of Entities.PazienteNote) = bizPaziente.GetNotePaziente(OnVacUtility.Variabili.PazId, OnVacContext.CodiceUslCorrente)
                If listaNote.Count > 0 Then

                    listaNote.RemoveAll(Function(x) x.FlagNotaVisibileConvocazioni.HasValue = False Or x.FlagNotaVisibileConvocazioni = False)

                    rptNote.DataSource = listaNote
                    rptNote.DataBind()

                End If
            End Using

        End Using

    End Sub

#End Region

End Class
