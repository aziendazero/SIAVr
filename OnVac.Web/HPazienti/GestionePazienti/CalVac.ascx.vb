Imports Onit.Database.DataAccessManager
Imports Onit.Controls
Imports Onit.OnAssistnet.OnVac.Collection
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Biz

Imports System.Collections.Generic


Partial Class OnVac_CalVac
    Inherits Common.UserControlPageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Variables"

    Private listaSedute As List(Of SedutaCalendario)
    Protected ds As DSCalendarioVaccinale

#End Region

#Region " Public "

    Public Sub LoadData()

        Me.lb_cic.Text = String.Empty
        ShowPrintButtons()

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using biz As New Biz.BizConvocazione(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)

                ds = biz.GetCalendarioVaccinaleDS(OnVacUtility.Variabili.PazId)
                listaSedute = biz.GetCalendarioVaccinale(ds)

            End Using

            Dim cicTemp As String = ""
            Dim sedTemp As Int16 = 0

            For rowDb As Integer = 0 To ds.dtCicSedVac.Rows.Count - 1

                If (cicTemp <> ds.dtCicSedVac.Rows(rowDb)("cic_codice") OrElse sedTemp <> ds.dtCicSedVac.Rows(rowDb)("tsd_n_seduta")) Then

                    If (cicTemp <> ds.dtCicSedVac.Rows(rowDb)("cic_codice")) Then
                        If (rowDb > 0) Then lb_cic.Text &= " - "
                        lb_cic.Text &= ds.dtCicSedVac.Rows(rowDb)("cic_descrizione")
                    End If

                    cicTemp = ds.dtCicSedVac.Rows(rowDb)("cic_codice")
                    sedTemp = ds.dtCicSedVac.Rows(rowDb)("tsd_n_seduta")

                End If

            Next

        End Using

    End Sub

    Public Overrides Function toString() As String

        Dim content As New System.Text.StringBuilder()
        Dim color As String
        
        content.AppendFormat("<table width='100%' cellspacing='0' cellpadding='2' >{0}", vbCrLf)

        Dim listOrdered As List(Of SedutaCalendario) = listaSedute.OrderBy(Function(p) p.Eta).ToList()

        For i As Integer = 0 To listOrdered.Count - 1

            Dim seduta As SedutaCalendario = listOrdered(i)
            '
            content.AppendFormat("<tr width='100%' height='18px' style='font-size:13px;font-weight:bold;font-family:arial;background-color:white'>{0}", vbCrLf)
            '
            content.AppendFormat("<td width='150px' align='left'>{0}", vbCrLf)
            content.AppendFormat("SEDUTA N.{0}{1}", (i + 1).ToString, vbCrLf)
            content.AppendFormat("</td>{0}", vbCrLf)
            '
            content.AppendFormat("<td width='150px' align='center'>{0}", vbCrLf)
            content.AppendFormat("DURATA: {0}min.{1}", seduta.Durata.ToString, vbCrLf)
            content.AppendFormat("</td>{0}", vbCrLf)
            '
          
            Dim _eta As New Entities.Eta(seduta.Eta)
            content.AppendFormat("<td width='200px' align='left' colspan='2'>{0}", vbCrLf)
            content.AppendFormat("ETA': {0} Anni {1} Mesi {2} Giorni{3}", _eta.Anni.ToString, _eta.Mesi.ToString, _eta.Giorni.ToString, vbCrLf)
            content.AppendFormat("</td>{0}", vbCrLf)
            '
            content.AppendFormat("<td  align='center'>{0}</td>{0}", vbCrLf)
            '
            content.AppendFormat("</tr>{0}", vbCrLf)
            '
            '
            content.AppendFormat("<tr width='100%' class='header'>{0}", vbCrLf)
            '
            content.AppendFormat("<td width='150px' align='left'>{0}Ciclo{0}</td>", vbCrLf)
            '
            content.AppendFormat("<td width='150px' align='center'>{0}Seduta del Ciclo{0}</td>", vbCrLf)
            '
            content.AppendFormat("<td width='200px' align='left'>{0}Vaccinazione{0}</td>", vbCrLf)
            '
            content.AppendFormat("<td width='100px' align='center'>{0}Dose{0}</td>", vbCrLf)
            '
            content.AppendFormat("<td  align='center'>{0}</td>", vbCrLf)
            '
            content.AppendFormat("</tr>{0}", vbCrLf)
            '
            '
            For j As Integer = 0 To seduta.Vaccinazioni.Count - 1

                Dim v As SedutaCalendario.Vaccinazione = seduta.Vaccinazioni(j)
                If (j Mod 2 = 0) Then
                    color = "item"
                Else
                    color = "alternating"
                End If
                content.AppendFormat("<tr width='100%' class='{0}'>{1}", color, vbCrLf)
                content.AppendFormat("<td align='left'width='150px'>{0}", vbCrLf)

                content.AppendFormat("{0}{1}", v.CicloDescrizione, vbCrLf)
                content.AppendFormat("</td>{0}", vbCrLf)
                '
                content.AppendFormat("<td width='150px' align='center'>{0}", vbCrLf)
                content.AppendFormat("{0}{1}", v.CicloSeduta, vbCrLf)
                content.AppendFormat("</td>{0}", vbCrLf)
                '
                content.AppendFormat("<td width='200px' align='left'{0}>", vbCrLf)
                content.AppendFormat("{0}{1}", v.Descrizione, vbCrLf)
                content.AppendFormat("</td>{0}", vbCrLf)
                '
                content.AppendFormat("<td width='100px' align='center'>{0}", vbCrLf)
                content.AppendFormat("{0}{1}", v.Dose, vbCrLf)
                content.AppendFormat("</td>{0}", vbCrLf)
                '
                content.AppendFormat("<td  align='center'>{0}", vbCrLf)
                content.AppendFormat("</td>{0}", vbCrLf)
                '
                content.AppendFormat("</tr>{0}", vbCrLf)
            Next
            '
            '
            content.AppendFormat("<tr width='100%' height='1px' class='header'>{0}", vbCrLf)
            '
            content.AppendFormat("<td colspan='5'>{0}</td>", vbCrLf)

            content.AppendFormat("</tr>{0}", vbCrLf)
        Next
        '
        '
        content.AppendFormat("<tr width='100%' height='10px' class='header'>{0}", vbCrLf)
        '
        content.AppendFormat("<td colspan='5'>{0}</td>", vbCrLf)
        '
        content.AppendFormat("</tr>{0}", vbCrLf)
        '
        '
        content.AppendFormat("</table>{0}", vbCrLf)

        Return content.ToString()

    End Function

    Public Sub DisposeTables()

        ds.Dispose()

    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub ToolBarCalVac_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBarCalVac.ButtonClicked

        Select Case e.Button.Key

            Case "btnCalVacStampa"

                Dim rpt As New ReportParameter()
                LoadData()
                rpt.set_dataset(ds)

                Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                    Using bizReport As New BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                        If Not OnVacReport.StampaReport(Page.Request.Path, Constants.ReportName.CalendarioVaccinale, "", rpt, , , bizReport.GetReportFolder(Constants.ReportName.CalendarioVaccinale)) Then
                            OnVacUtility.StampaNonPresente(Page, Constants.ReportName.CalendarioVaccinale)
                        End If

                    End Using
                End Using

        End Select

    End Sub

#End Region

#Region " Private "

    Private Sub ShowPrintButtons()

        Dim existsRpt As Boolean = False

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                existsRpt = bizReport.ExistsReport(Constants.ReportName.CalendarioVaccinale)

            End Using
        End Using

        ToolBarCalVac.Items.FromKeyButton("btnCalVacStampa").Visible = existsRpt

    End Sub

#End Region

End Class