Imports System.Web
Imports System.Web.Services
Imports Onit.Database.DataAccessManager

Public Class HtmlViewHandler
    Implements System.Web.IHttpHandler

    Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest

        Dim doc As String = context.Request.QueryString.Get("docId")
        Dim idApplicazione As String = context.Request.QueryString.Get("appId")
        Dim codiceAzienda As String = context.Request.QueryString.Get("codAzienda")
        Dim ute As String = context.Request.QueryString.Get("uteId")
        Dim ulss As String = context.Request.QueryString.Get("ulss")

        context.Response.ContentType = "text/html"

        Dim html As String = String.Empty

        If String.IsNullOrEmpty(doc) Then

            html = "<html><body onload=""alert('Id documento non specificato');""><body><html>"

        Else

            ' N.B. : OnVacContext non è disponibile nel metodo ProcessRequest dell'handler
            Using dam As IDAM = OnVacUtility.OpenDam(idApplicazione, codiceAzienda)
                Using genericProvider As New DAL.DbGenericProvider(dam)

                    Dim idDocumento As Long = Convert.ToInt64(doc)
                    Dim idUtente As Long = Convert.ToInt64(ute)

                    Dim settings As New Settings.Settings(genericProvider)
                    Dim bizContextInfos As New Biz.BizContextInfos(idUtente, codiceAzienda, idApplicazione, String.Empty, ulss, Nothing)

                    Using bizFirma As New Biz.BizFirmaDigitale(genericProvider, settings, bizContextInfos)

                        Dim result As Biz.BizFirmaDigitale.CreaDocumentoResult = bizFirma.CreaDocumentoHTMLAnamnesi(idDocumento)

                        If result.ResultType = Biz.BizFirmaDigitale.CreaDocumentoResultType.Success Then
                            html = result.Documento
                        ElseIf result.ResultType = Biz.BizFirmaDigitale.CreaDocumentoResultType.ErroreDocumentoFirmato Then
                            html = String.Empty
                        Else
                            html = String.Format("<html><body onload=""alert('{0}');""><body><html>", HttpUtility.JavaScriptStringEncode(result.Message))
                        End If

                    End Using
                End Using
            End Using

        End If

        If Not String.IsNullOrWhiteSpace(html) Then context.Response.Write(html)

    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class