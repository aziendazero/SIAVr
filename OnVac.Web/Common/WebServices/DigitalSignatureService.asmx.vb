Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports Onit.Database.DataAccessManager


<WebService(Namespace:="http://tempuri.org/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
<System.Web.Script.Services.ScriptService>
Public Class DigitalSignatureService
    Inherits System.Web.Services.WebService

    ''' <summary>
    ''' Restituisce un oggetto contenente il documento XML che deve essere firmato digitalmente e il suo id.
    ''' </summary>
    ''' <param name="idDocumento"></param>
    ''' <param name="idUtente"></param>
    ''' <param name="idApplicazione"></param>
    ''' <param name="codiceAzienda"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod>
    Public Function GetDocument(idDocumento As Long, idUtente As Long, idApplicazione As String, codiceAzienda As String) As Document

        Dim documentoXML As String = String.Empty

        Using dam As IDAM = OnVacUtility.OpenDam(idApplicazione, codiceAzienda)
            Using genericProvider As New DAL.DbGenericProvider(dam)

                Dim settings As New Settings.Settings(genericProvider)
                Dim bizContextInfos As New Biz.BizContextInfos(idUtente, codiceAzienda, idApplicazione)

                Using bizFirma As New Biz.BizFirmaDigitale(genericProvider, settings, bizContextInfos)

                    documentoXML = bizFirma.GetDocumentoDaFirmare(idDocumento)

                End Using
            End Using
        End Using

        Return New Document() With {.Xml = documentoXML, .Id = idDocumento}

    End Function

    ''' <summary>
    ''' Effettua l'update del documento firmato, compresi i dati dell'utente che ha firmato.
    ''' </summary>
    ''' <param name="signed"></param>
    ''' <param name="idUtente"></param>
    ''' <param name="idApplicazione"></param>
    ''' <param name="codiceAzienda"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod>
    Public Function UploadSignedDocument(signed As SignedDocument, idUtente As Long, idApplicazione As String, codiceAzienda As String) As UploadResult

        Dim success As Boolean = False

        Using dam As IDAM = OnVacUtility.OpenDam(idApplicazione, codiceAzienda)
            Using genericProvider As New DAL.DbGenericProvider(dam)

                Dim settings As New Settings.Settings(genericProvider)
                Dim bizContextInfos As New Biz.BizContextInfos(idUtente, codiceAzienda, idApplicazione)

                Using bizFirma As New Biz.BizFirmaDigitale(genericProvider, settings, bizContextInfos)

                    success = bizFirma.UpdateDocumentoFirmato(signed.Id, signed.SignedXml)

                End Using
            End Using
        End Using

        Dim message As String = "Firma digitale effettuata."
        If Not success Then message = "Errore durante la procedura di firma digitale del documento."

        Return New UploadResult() With {
            .Ok = success,
            .Message = message
        }

    End Function

End Class

#Region " Types "

<Serializable>
Public Class Document
    Public Property Id As Long
    Public Property Xml As String
End Class

<Serializable>
Public Class UploadResult
    Public Property Ok As Boolean
    Public Property Message As String
End Class

<Serializable>
Public Class SignedDocument
    Public Property Id As Long
    Public Property SignedXml As String
End Class

#End Region