Imports System.Collections.Generic

Public Class VaccinazioniMal
    Inherits OnVac.Common.PageBase

#Region " Proprietà private "

    Private Property CodiceMalattia As String
        Get
            If ViewState("CodiceMalattia") Is Nothing Then ViewState("CodiceMalattia") = String.Empty
            Return ViewState("CodiceMalattia")
        End Get
        Set(value As String)
            ViewState("CodiceMalattia") = value
        End Set
    End Property

    ''' <summary>
    ''' Indica se il checkbox "Seleziona tutti" è selezionato o deselezionato
    ''' </summary>
    Private Property SelezionaTuttiChecked As Boolean
        Get
            If ViewState("CHKALL") Is Nothing Then ViewState("CHKALL") = False
            Return DirectCast(ViewState("CHKALL"), Boolean)
        End Get
        Set(value As Boolean)
            ViewState("CHKALL") = value
        End Set
    End Property

#End Region


#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not IsPostBack() Then

            CodiceMalattia = Request.QueryString("MAL").ToString.Split("|")(0) 'il valore è passato dalla pag. 'Malattie.aspx'
            LayoutTitolo.Text = Request.QueryString("MAL").ToString.Split("|")(1)

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using biz As New Biz.BizVaccinazioniAssociate(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                    Dim listVacAssociabili As List(Of Entities.VaccinazioneAssociabile) = biz.GetVaccinazioniAssociabili()

                    'databind della griglia
                    dgrVaccinazioni.DataSource = listVacAssociabili
                    dgrVaccinazioni.DataBind()

                    'Selezione delle vaccinazioni associate
                    Dim listVacAssociate As List(Of String) = biz.GetCodiciVaccinazioniAssociateAMalattia(CodiceMalattia)
                    SetCodiciVaccinazioniSelezionate(listVacAssociate)

                End Using
            End Using

        End If

        Select Case Request.Form.Item("__EVENTTARGET")

            Case "selectAll"

                Me.SelectAll()

        End Select

    End Sub

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key
            Case "btnSalva"
                Dim codVac As List(Of String) = GetCodiciVaccinazioniSelezionate()
                Salva(codVac)
                OnitLayout31.Busy = False

        End Select

    End Sub

    Protected Sub chkSelezioneItem_CheckedChanged(sender As Object, e As EventArgs)
        OnitLayout31.Busy = True
    End Sub

#End Region

    Private Sub Salva(codVac As List(Of String))

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using biz As New Biz.BizVaccinazioniAssociate(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                biz.SalvaVaccinazioniAssociateAMalattia(Me.CodiceMalattia, codVac)

            End Using
        End Using


    End Sub

    Private Function GetCodiciVaccinazioniSelezionate() As List(Of String)

        Dim listCodVac As New List(Of String)()

        For Each item As DataGridItem In Me.dgrVaccinazioni.Items

            If DirectCast(item.FindControl("chkSelezioneItem"), CheckBox).Checked Then

                Dim key As String = Me.dgrVaccinazioni.DataKeys(item.ItemIndex).ToString()
                listCodVac.Add(key)

            End If

        Next

        Return listCodVac

    End Function

    Private Sub SetCodiciVaccinazioniSelezionate(codiciVac As List(Of String))

        For Each item As DataGridItem In Me.dgrVaccinazioni.Items

            Dim checked As Boolean = False
            Dim key As String = Me.dgrVaccinazioni.DataKeys(item.ItemIndex).ToString()

            If codiciVac.Contains(key) Then
                checked = True
            End If

            DirectCast(item.FindControl("chkSelezioneItem"), CheckBox).Checked = checked

        Next

    End Sub

    ''' <summary>
    ''' Click del checkbox nell'header del datagrid per selezionare/deselezionare tutte le righe del datagrid
    ''' </summary>
    Private Sub SelectAll()

        OnitLayout31.Busy = True

        Dim selezionaTutti As Boolean = False
        Dim listIdVac As List(Of String) = New List(Of String)()

        If Not Boolean.TryParse(Request.Form.Item("__EVENTARGUMENT"), selezionaTutti) Then
            Return
        End If

        Me.SelezionaTuttiChecked = selezionaTutti

        If Me.SelezionaTuttiChecked Then

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using biz As New Biz.BizVaccinazioniAssociate(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                    Dim listVacAssociabili As List(Of Entities.VaccinazioneAssociabile) = biz.GetVaccinazioniAssociabili()

                    listIdVac = listVacAssociabili.Select(Function(v) v.CodiceVac).ToList()

                End Using
            End Using

        End If

        If Not listIdVac Is Nothing Then
            'Set dei check delle vaccinazioni selezionate
            SetCodiciVaccinazioniSelezionate(listIdVac)
        End If

    End Sub

End Class