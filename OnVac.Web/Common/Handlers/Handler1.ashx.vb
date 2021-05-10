Public Class Handler1
    Implements IHttpHandler

    Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest

        Dim outByte As Byte()

        Dim codicePaziente As Long = 4893022

        Dim paz As String = context.Request.QueryString.Get("paz")
        If Not String.IsNullOrWhiteSpace(paz) Then
            codicePaziente = Convert.ToInt64(paz)
        End If

        Dim idUtente As Long = 26

        Dim ute As String = context.Request.QueryString.Get("ute")
        If Not String.IsNullOrWhiteSpace(ute) Then
            idUtente = Convert.ToInt64(ute)
        End If

        Dim idApplicazione As String = context.Request.QueryString.Get("app")

        If String.IsNullOrWhiteSpace(idApplicazione) Then
            idApplicazione = "OnVac501"
        End If

        Dim codiceAzienda As String = context.Request.QueryString.Get("azi")

        If String.IsNullOrWhiteSpace(codiceAzienda) Then
            codiceAzienda = "050000"
        End If

        Dim cns As String = context.Request.QueryString.Get("cns")

        If String.IsNullOrWhiteSpace(cns) Then
            cns = "VAC"
        End If

        Dim ulss As String = context.Request.QueryString.Get("ulss")

        Using dbGenericProviderFactory As New Biz.DbGenericProviderFactory()
            Using genericProvider As DAL.DbGenericProvider = dbGenericProviderFactory.GetDbGenericProvider(idApplicazione, codiceAzienda)

                Dim bizContextInfos As New Biz.BizContextInfos(idUtente, codiceAzienda, idApplicazione, cns, ulss, Nothing)

                Using bizPaz As New Biz.BizPaziente(genericProvider, New Settings.Settings(genericProvider), bizContextInfos, Nothing)

                    Dim rpt As String = context.Request.QueryString.Get("rpt")

                    If rpt = "COV" Then

                        Dim ms As IO.MemoryStream =
                            bizPaz.GetComunicazioneSorveglianzaCOVID19(
                                New Biz.BizPaziente.GetComunicazioneSorveglianzaCOVID19Command() With {
                                    .CodicePaziente = codicePaziente,
                                    .PercorsoReport = "C:\projects\OnVac.Veneto\src\OnVac.Web\Report\ReportComuni\ComunicazioneSorveglianzaCOVID19.rpt",
                                    .CodiceUslCorrente = ulss,
                                    .DataInizioSorveglianza = Date.Now.AddDays(-1),
                                    .DataFineSorveglianza = Date.Now.AddDays(14),
                                    .DataStampa = Date.Now
                                })

                        outByte = ms.ToArray()

                    ElseIf rpt = "COVNEG" Then

                        Dim idEpisodio As String = context.Request.QueryString.Get("ep")

                        Dim ms As IO.MemoryStream =
                            bizPaz.GetCertificatoNegativizzazioneCOVID19(
                                New Biz.BizPaziente.GetCertificatoNegativizzazioneCOVID19Command() With {
                                    .CodicePaziente = codicePaziente,
                                    .PercorsoReport = "C:\projects\OnVac.Veneto\src\OnVac.Web\Report\ReportComuni\CertNegativizzazioneCOVID19.rpt",
                                    .CodiceUslCorrente = ulss,
                                    .DataStampa = Date.Now,
                                    .IdEpisodio = idEpisodio
                                })

                        outByte = ms.ToArray()

                    ElseIf rpt = "COVTESTNEG" Then

                        Dim ms As IO.MemoryStream =
                            bizPaz.GetCertificatoTestAntigeneRapidoNegativoCOVID19(
                                New Biz.BizPaziente.GetCertificatoTestAntigeneRapidoNegativoCOVID19Command() With {
                                    .CodicePaziente = codicePaziente,
                                    .PercorsoReport = "C:\projects\OnVac.Veneto\src\OnVac.Web\Report\ReportComuni\CertTestAntigeneRapidoNegativoCOVID19.rpt",
                                    .CodiceUslCorrente = ulss,
                                    .DataStampa = Date.Now,
                                    .DataTest = Date.Now.AddDays(-2)
                                })

                        outByte = ms.ToArray()

                    ElseIf rpt = "COVTESTNEG_TAR" Then

                        Dim campione As String = context.Request.QueryString.Get("campione")
                        Dim centro As String = context.Request.QueryString.Get("centro")

                        Dim numeroCampione As Long? = Nothing
                        Dim parseResult As Long

                        If Long.TryParse(campione, parseResult) Then
                            numeroCampione = parseResult
                        End If

                        Dim ms As IO.MemoryStream =
                            bizPaz.GetCertificatoTestAntigeneRapidoNegativoCOVID19(
                                New Biz.BizPaziente.GetCertificatoTestAntigeneRapidoNegativoCOVID19Command() With {
                                    .NumeroCampione = numeroCampione,
                                    .CodiceCentro = centro,
                                    .PercorsoReport = "C:\projects\OnVac.Veneto\src\OnVac.Web\Report\ReportComuni\CertTestAntigeneRapidoNegativoCOVID19.rpt",
                                    .PercorsoReport_TAR = "C:\projects\OnVac.Veneto\src\OnVac.Web\Report\ReportComuni\CertTestAntigeneRapidoNegativoCOVID19_TAR.rpt",
                                    .CodiceUslCorrente = ulss,
                                    .DataStampa = Date.Now,
                                    .DataTest = Date.Now.AddDays(-2)
                                })

                        outByte = ms.ToArray()

                        Else

                            outByte = bizPaz.GetCertificatoVaccinale(
                        New Biz.BizPaziente.GetCertificatoVaccinaleCommand() With {
                            .CodicePaziente = codicePaziente,
                            .ReportName = "C:\projects\OnVac.Veneto\src\OnVac.Web\Report\ReportComuni\CertificatoVaccinaleAPP.rpt",
                            .StampaNotaValidita = False,
                            .StampaLottoNomeCommerciale = False,
                            .StampaScrittaCertificato = True,
                            .CodiceUslCorrente = ulss
                        })

                    End If

                End Using

            End Using
        End Using

        context.Response.ContentType = Constants.MIMEContentType.APPLICATION_PDF
        context.Response.AddHeader("Content-disposition", "filename=test.pdf")
        context.Response.OutputStream.Write(outByte, 0, outByte.Length)
        context.Response.OutputStream.Flush()
        context.Response.End()

    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class