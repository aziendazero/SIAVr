Public Class CommonBilancio

    Public Class StampaBilancioCommand
        Public CodicePaziente As String
        Public IdVisita As Long
        Public IsCentrale As Boolean
        Public NumeroBilancio As Integer?
        Public CodiceMalattia As String
        Public Page As Page
        Public Settings As OnVac.Settings.Settings
    End Class

    Public Class StampaBilancioResult
        Public Success As Boolean
        Public Message As String
    End Class

    Public Shared Function StampaBilancio(command As StampaBilancioCommand) As StampaBilancioResult

        Dim result As New StampaBilancioResult()
        result.Success = True
        result.Message = String.Empty

        Dim bilancioSelezionato As Entities.BilancioAnagrafica = CommonBilancio.GetBilancioAnagrafica(command.NumeroBilancio.Value, command.CodiceMalattia, command.Settings)

        Select Case bilancioSelezionato.NomeReport

            Case Constants.ReportName.AnamnesiDefault
                '---
                ' Report di default per i bilanci
                '---
                StampaReportAnamnesiDefault(command.CodicePaziente, command.IdVisita, command.IsCentrale, bilancioSelezionato, command.Settings, command.Page)

            Case Constants.ReportName.AnamnesiViaggiatori
                '---
                ' Report per anamnesi viaggiatori => non ancora creato, stampo quello di default
                '---
                StampaReportAnamnesiViaggiatori(command.CodicePaziente, command.IdVisita, command.IsCentrale, bilancioSelezionato, command.Settings, command.Page)

            Case String.Empty
                '---
                ' Nome del report nullo
                '---
                result.Success = False
                result.Message = "Per questo bilancio non è stato specificato nessun report."

            Case Else
                '---
                ' Nome del report non previsto
                '---
                result.Success = False
                result.Message = "Il report specificato per questo bilancio non è previsto."

        End Select

        Return result

    End Function

    Private Shared Sub StampaReportAnamnesiDefault(codicePaziente As String, idVisita As Long, isCentrale As Boolean, bilancioSelezionato As Entities.BilancioAnagrafica, settings As OnVac.Settings.Settings, page As Page)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Dim dstAnamnesiDefault As DataSet.AnamnesiDefault = Nothing

            Using bizVisite As New Biz.BizVisite(genericProvider, settings, OnVacContext.CreateBizContextInfos(), Nothing)
                dstAnamnesiDefault = bizVisite.GetDataSetAnamnesiDefault(idVisita, codicePaziente, isCentrale, bilancioSelezionato.GestioneVaccinazioni, bilancioSelezionato.GestioneViaggi)
            End Using

            Dim rpt As New ReportParameter()
            rpt.set_dataset(dstAnamnesiDefault)
            rpt.set_dataset_subreport(dstAnamnesiDefault, "AnamnesiDefault_Vaccinazioni.rpt")
            rpt.set_dataset_subreport(dstAnamnesiDefault, "AnamnesiDefault_Viaggi.rpt")

            rpt.AddParameter("ShowVaccinazioni", (dstAnamnesiDefault.Vaccinazioni.Rows.Count > 0).ToString())
            rpt.AddParameter("ShowViaggio", bilancioSelezionato.GestioneViaggi.ToString())

            Using bizReport As New Biz.BizReport(genericProvider, settings, OnVacContext.CreateBizContextInfos())

                ' N.B. : questo metodo apre l'anteprima di stampa facendo sì che, al chiudi, venga ricaricata la pagina con un HistoryNavigateBack.
                If Not OnVacReport.StampaReport(Constants.ReportName.AnamnesiDefault, String.Empty, rpt, Nothing, Nothing, bizReport.GetReportFolder(Constants.ReportName.AnamnesiDefault)) Then
                    OnVacUtility.StampaNonPresente(page, Constants.ReportName.AnamnesiDefault)
                End If

            End Using
        End Using

    End Sub
    Private Shared Sub StampaReportAnamnesiViaggiatori(codicePaziente As String, idVisita As Long, isCentrale As Boolean, bilancioSelezionato As Entities.BilancioAnagrafica, settings As OnVac.Settings.Settings, page As Page)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Dim dstAnamnesiDefault As DataSet.AnamnesiDefault = Nothing
            Dim ShowPost As Boolean = False

            Using bizVisite As New Biz.BizVisite(genericProvider, settings, OnVacContext.CreateBizContextInfos(), Nothing)
                dstAnamnesiDefault = bizVisite.GetDataSetAnamnesiDefault(idVisita, codicePaziente, isCentrale, bilancioSelezionato.GestioneVaccinazioni, bilancioSelezionato.GestioneViaggi)
                ShowPost = bizVisite.ExisistIdPadreFollowUp(idVisita)
            End Using

            Dim rpt As New ReportParameter()
            rpt.set_dataset(dstAnamnesiDefault)
            rpt.set_dataset_subreport(dstAnamnesiDefault, "AnamnesiDefault_Vaccinazioni.rpt")
            rpt.set_dataset_subreport(dstAnamnesiDefault, "AnamnesiViaggiatori_Viaggi.rpt")
            rpt.set_dataset_subreport(dstAnamnesiDefault, "AnamnesiViaggiatori_Piede.rpt")

            rpt.AddParameter("ShowVaccinazioni", (dstAnamnesiDefault.Vaccinazioni.Rows.Count > 0).ToString())
            rpt.AddParameter("ShowViaggio", bilancioSelezionato.GestioneViaggi.ToString())
            rpt.AddParameter("GiorniFollowUpIpotetico", settings.NUM_GIORNI_FOLLOWUP.ToString())
            rpt.AddParameter("ShowPost", ShowPost.ToString())



            Using bizReport As New Biz.BizReport(genericProvider, settings, OnVacContext.CreateBizContextInfos())

                ' N.B. : questo metodo apre l'anteprima di stampa facendo sì che, al chiudi, venga ricaricata la pagina con un HistoryNavigateBack.
                If Not OnVacReport.StampaReport(Constants.ReportName.AnamnesiViaggiatori, String.Empty, rpt, Nothing, Nothing, bizReport.GetReportFolder(Constants.ReportName.AnamnesiViaggiatori)) Then
                    OnVacUtility.StampaNonPresente(page, Constants.ReportName.AnamnesiDefault)
                End If

            End Using
        End Using

    End Sub

    ''' <summary>
    ''' Restituisce i dati dell'anagrafica relativi al bilancio specificato
    ''' </summary>
    ''' <param name="numeroBilancio"></param>
    ''' <param name="codiceMalattia"></param>
    ''' <param name="settings"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetBilancioAnagrafica(numeroBilancio As Integer, codiceMalattia As String, settings As OnVac.Settings.Settings) As Entities.BilancioAnagrafica

        Dim bilancioSelezionato As Entities.BilancioAnagrafica = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnaBilanci As New Biz.BizAnaBilanci(genericProvider, settings, OnVacContext.CreateBizContextInfos())

                bilancioSelezionato = bizAnaBilanci.GetAnagraficaBilancio(numeroBilancio, codiceMalattia)

            End Using
        End Using

        Return bilancioSelezionato

    End Function

End Class
