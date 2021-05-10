Imports System.Web.Script.Serialization
Imports Onit.Shared.Web.Static

Public Class FirmaDigitaleArchiviazioneSostitutiva
    Inherits Common.UserControlPageBase

#Region " Types "

    <Serializable>
    Public Class SignResult
        Public Property Ok As Boolean
        Public Property Message As String
    End Class

#End Region

#Region " Properties "

    Public Property MostraPulsanteIndietro() As Boolean
        Get
            If ViewState("showBtn") Is Nothing Then ViewState("showBtn") = True
            Return Convert.ToBoolean(ViewState("showBtn"))
        End Get
        Set(value As Boolean)
            ViewState("showBtn") = value
        End Set
    End Property

    Protected ReadOnly Property UrlServizioFirmaDigitale As String
        Get
            Return My.MySettings.Default.DigitalSignatureServiceURL
        End Get
    End Property

    Protected Property IdDocumento As Long
        Get
            If ViewState("IdDoc") Is Nothing Then ViewState("IdDoc") = 0
            Return Convert.ToInt64(ViewState("IdDoc"))
        End Get
        Set(value As Long)
            ViewState("IdDoc") = value
        End Set
    End Property

    Protected ReadOnly Property IdUtenteCorrente As String
        Get
            Return OnVacContext.UserId.ToString()
        End Get
    End Property

    Protected ReadOnly Property IdApplicativoCorrente As String
        Get
            Return OnVacContext.AppId
        End Get
    End Property

    Protected ReadOnly Property CodiceAziendaCorrente As String
        Get
            Return OnVacContext.Azienda
        End Get
    End Property

#End Region

#Region " Public Events "

    Public Event ShowMessage(message As String)

    Public Sub OnShowMessage(message As String)
        RaiseEvent ShowMessage(message)
    End Sub

    Public Event ClickIndietro(idDocumento As Long)

    Public Sub OnClickIndietro(idDocumento As Long)
        RaiseEvent ClickIndietro(idDocumento)
    End Sub

    Public Event FirmaDigitaleCompleted(success As Boolean, message As String)

    Public Sub OnFirmaDigitaleCompleted(success As Boolean, message As String)
        RaiseEvent FirmaDigitaleCompleted(success, message)
    End Sub

#End Region

#Region " UserControl Events "

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        Page.ClientScript.RegisterClientScriptInclude("json2", ResolveUrl("~/scripts/json/json2.js"))
        Page.ClientScript.RegisterClientScriptInclude("jquery", Page.ClientScript.GetWebResourceUrl(GetType(WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js"))
        Page.ClientScript.RegisterClientScriptInclude("signature", ResolveClientUrl("~/common/scripts/signature.js"))

        Me.toolbarFirma.Items.FromKeyButton("btnIndietro").Visible = Me.MostraPulsanteIndietro

    End Sub

#End Region

#Region " Controls Events "

    Private Sub toolbarFirma_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles toolbarFirma.ButtonClicked

        Select Case be.Button.Key

            Case "btnIndietro"

                OnClickIndietro(Me.IdDocumento)

        End Select

    End Sub

    Private Sub lnkPostBk_Click(sender As Object, e As EventArgs) Handles lnkPostBk.Click

        If String.IsNullOrWhiteSpace(Me.txtResult.Value) Then
            OnShowMessage("Errore in fase di firma. Non è stato possibile firmare digitalmente il documento.")
            Return
        End If

        Dim des As New JavaScriptSerializer()
        Dim firmaResult As SignResult = des.Deserialize(Of SignResult)(Me.txtResult.Value)

        Dim message As New System.Text.StringBuilder()

        If String.IsNullOrWhiteSpace(firmaResult.Message) Then
            If firmaResult.Ok Then
                message.AppendLine("Firma digitale effettuata.")
            Else
                message.AppendLine("Errore in fase di firma.")
            End If
        Else
            message.AppendLine(firmaResult.Message)
        End If

        If firmaResult.Ok Then

            Dim archiviazioneResult As Biz.BizFirmaDigitale.ArchiviazioneSostitutivaResult = Nothing

            ' Tentativo di invio del documento al servizio di archiviazione sostitutiva
            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizFirma As New Biz.BizFirmaDigitale(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                    archiviazioneResult = bizFirma.InvioAnamnesiArchiviazioneSostitutiva(Me.IdDocumento)

                End Using
            End Using

            message.AppendLine(archiviazioneResult.Message)

        End If

        OnFirmaDigitaleCompleted(firmaResult.Ok, message.ToString())

    End Sub

#End Region

#Region " Public "

    Public Sub AnteprimaAnamnesi(idVisita As Integer)

        Dim xmlResult As Biz.BizFirmaDigitale.CreaDocumentoResult

        ' --- Creazione XML --- '
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizFirma As New Biz.BizFirmaDigitale(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                xmlResult = bizFirma.CreaDocumentoXMLAnamnesi(idVisita)

            End Using
        End Using

        If xmlResult.ResultType = Biz.BizFirmaDigitale.CreaDocumentoResultType.Success Then

            IdDocumento = xmlResult.IdDocumento

            ' --- Visualizzazione XML da firmare --- '
            iframeAnteprimaAnamnesi.Attributes("src") =
                ResolveClientUrl(String.Format("../handlers/FirmaDigitaleAnamnesiHandler.ashx?docId={0}&appId={1}&codAzienda={2}&uteId={3}&ulss={4}",
                                               xmlResult.IdDocumento.ToString(), OnVacContext.AppId, OnVacContext.Azienda, OnVacContext.UserId, OnVacContext.CodiceUslCorrente))

        Else
            OnShowMessage(xmlResult.Message)
        End If

    End Sub

#End Region

End Class
