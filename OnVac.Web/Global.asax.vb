Imports Microsoft.Security.Application
Imports Onit.OnAssistnet.OnVac.Common
Imports Onit.Shared.Web.Static
Imports System.Reflection

''' Pipeline delle pagine
''' 
''' OnVac.Global.asax   Application_BeginRequest
''' OnVac.Global.asax   Application_AuthenticateRequest
''' Onit.Shared.Web.UI.Page   Page() ctor       
''' OnVac.Global.asax   Global_PreRequestHandlerExecute
''' Onit.OnAssistnet.Web.UI.Page.OnPreInit
''' OnVac.PageBase.PageBase_Init
''' Onit.Shared.Web.UI.Page.OnPreLoad
''' Onit.Shared.Web.UI.Page.OnLoad

Public Class _Global
    Inherits Onit.OnAssistnet.Web.HttpApplication

#Region " Codice generato da Progettazione componenti "

    Public Sub New()
        MyBase.New()

        'Chiamata richiesta da Progettazione componenti.
        InitializeComponent()

        'Aggiungere le eventuali istruzioni di inizializzazione dopo la chiamata a InitializeComponent()

    End Sub

    'Richiesto da Progettazione componenti
    Private components As System.ComponentModel.IContainer

    'NOTA: la procedura che segue è richiesta da Progettazione componenti.
    'Può essere modificata in Progettazione componenti.  
    'Non modificarla nell'editor del codice.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        components = New System.ComponentModel.Container()
    End Sub

#End Region

    Sub Application_Start(sender As Object, e As EventArgs)
        ' Generato all'avvio dell'applicazione
    End Sub

    Sub Session_Start(sender As Object, e As EventArgs)
        ' Generato all'avvio della sessione
    End Sub

    Sub Application_BeginRequest(sender As Object, e As EventArgs)
        ' Generato all'inizio di ogni richiesta
    End Sub

    Sub Application_AuthenticateRequest(sender As Object, e As EventArgs)
        ' Generato durante il tentativo di autenticazione dell'utente
    End Sub

    Sub Application_Error(sender As Object, e As EventArgs)
        ' Generato in caso di errore

        ' Se l'eccezione sollevata è una di quelle custom di OnVac, è presente nella InnerException.
        ' Se la InnerException è Nothing, prendo la BaseException o il LastError, che dovrebbe essere sempre una UnhandledException
        Dim ex As Exception = MyBase.Server.GetLastError().InnerException

        If ex Is Nothing Then
            ex = MyBase.Server.GetLastError().GetBaseException()
            If ex Is Nothing Then ex = MyBase.Server.GetLastError()
        End If

        Dim eventLogAppId As String

        Try
            eventLogAppId = OnVacContext.AppId
        Catch exc As Exception
            eventLogAppId = "OnVac"
        End Try

        ' Log dell'eccezione nell'eventLog e interruzione dell'esecuzione
        Common.Utility.EventLogHelper.EventLogWrite(ex, eventLogAppId)

        MyBase.Response.Clear()

        Dim renderedOutput As New System.Text.StringBuilder()

        If ex.GetType() Is GetType(OnVacConfigurationException) Then

            ' --- Errore di configurazione --- '
            Dim messaggioConfigurazione As String = String.Format("Attenzione! Si è verificato il seguente errore di configurazione:{0}{1}",
                                                                  Environment.NewLine,
                                                                  ex.ToString())

            ' Visualizzazione pagina di errore
            renderedOutput.Append(Me.CreateScriptErrorPage(messaggioConfigurazione))

        ElseIf Not HttpContext.Current.Session Is Nothing AndAlso OnVacContext.AppId Is Nothing Then

            ' --- Timeout della Session --- '

            'Recupero il redirect url dal web.config
            Dim urlRedirectPortale As String = Common.Utility.ConfigHelper.GetParameterValue("redirectPortale")

            'Leggo, dal web.config, il percorso per recuperare la pagina di chiusura della personalMain
            Dim urlChiusuraPersonal As String = Common.Utility.ConfigHelper.GetParameterValue("chiusuraPersonal")

            'Leggo il messaggio da visualizzare all'utente, dal file di risorse specificato
            Dim rm As New System.Resources.ResourceManager("Onit.OnAssistnet.OnVac.Messages", System.Reflection.Assembly.GetExecutingAssembly())

            Dim messaggioSessionEnd As String = rm.GetString("session_end_user_message")

            'Registro lo script di chiusura della finestra attuale, della personalMain (se è ancora aperta) e riapertura di una nuova finestra del portale
            renderedOutput.AppendFormat("<script language=javascript>alert('{0}');window.top.close();window.open('{1}','','');try{{if(typeof(window.top.opener.top)=='object') window.top.opener.top.location ='{2}';}} catch(e){{}}</script>",
                                        messaggioSessionEnd, urlRedirectPortale, urlChiusuraPersonal)

        Else

            ' --- Altra eccezione non gestita --- '

            renderedOutput.AppendFormat(Me.CreateScriptErrorPage(ex.Message.Replace("\", " ")))

        End If

        Response.Write(renderedOutput.ToString())

        'Faccio in modo che l'esecuzione della pagina si blocchi e quindi lo script abbia immediatamente effetto
        MyBase.Response.End()
        MyBase.Server.ClearError()

    End Sub

    Sub Session_End(sender As Object, e As EventArgs)
        ' Generato alla fine della sessione
    End Sub

    Sub Application_End(sender As Object, e As EventArgs)
        ' Generato alla chiusura dell'applicazione

        Dim runtime As HttpRuntime = DirectCast(GetType(HttpRuntime).InvokeMember("_theRuntime", (BindingFlags.GetField Or (BindingFlags.NonPublic Or BindingFlags.Static)), Nothing, Nothing, Nothing), HttpRuntime)

        If Not runtime Is Nothing Then

            Dim shutDownMessage As String =
                CStr(runtime.GetType.InvokeMember("_shutDownMessage", (BindingFlags.GetField Or (BindingFlags.NonPublic Or BindingFlags.Instance)), Nothing, runtime, Nothing))

            Dim shutDownStack As String =
                CStr(runtime.GetType.InvokeMember("_shutDownStack", (BindingFlags.GetField Or (BindingFlags.NonPublic Or BindingFlags.Instance)), Nothing, runtime, Nothing))

            Dim eventLogAppId As String

            Try
                eventLogAppId = OnVacContext.AppId
            Catch exc As Exception
                eventLogAppId = "OnVac"
            End Try

            Common.Utility.EventLogHelper.EventLogWrite(String.Format("{0}{1}{0}{1}_shutDownMessage={2}{0}{1}{0}{1}_shutDownStack={3}", ChrW(13), ChrW(10), shutDownMessage, shutDownStack), eventLogAppId)

        End If

    End Sub

    Private Sub Global_PostRequestHandlerExecute(sender As Object, e As System.EventArgs) Handles MyBase.PostRequestHandlerExecute

    End Sub

    Private Sub Global_PreRequestHandlerExecute(sender As Object, e As System.EventArgs) Handles MyBase.PreRequestHandlerExecute

        If Context.Handler.GetType().IsSubclassOf(GetType(Page)) Then

            Dim currentPage As Page = DirectCast(Context.Handler, Page)

            ' Pagina corrente
            Dim currentPageName As String = Context.Request.AppRelativeCurrentExecutionFilePath.Substring(Context.Request.AppRelativeCurrentExecutionFilePath.LastIndexOf("/") + 1)

            ' Elenco delle pagine per cui non effettuare il controllo della session scaduta
            Dim pagesNoCheck As String = Common.Utility.ConfigHelper.GetParameterValue("PagesWithoutCheckOfSession")
            If pagesNoCheck = String.Empty Then
                Throw New ParamNotPresentException("PagesWithoutCheckOfSession")
            End If

            ' Se la pagina corrente non è tra quelle in elenco, controllo la validità della session
            If pagesNoCheck.ToLower().IndexOf(currentPageName.ToLower()) = -1 Then

                '--------------------------------------------------------------------
                ' Check della sessione ancora valida
                Dim CheckIfExistsSession As Boolean

                Try
                    CheckIfExistsSession = Boolean.Parse(Common.Utility.ConfigHelper.GetParameterValue("CheckIfExistsSession"))
                Catch ex As Exception
                    Throw New ParamNotPresentException("CheckIfExistsSession")
                End Try

                ' Controllo che la variabile monitor della sessione sia valorizzata
                ' e che la sessione sia ancora quella precedente.
                If CheckIfExistsSession AndAlso OnVacContext.IsSessionTimeout() Then
                    Throw New SessionEmptyException(currentPageName)
                End If
                '--------------------------------------------------------------------

                '--------------------------------------------------------------------
                ' Gestione della trace delle pagine
                Dim pageTracker As PageTracker = CType(Session("PageTracker"), PageTracker)

                If pageTracker.PageItem(currentPage.ToString()) Is Nothing Then
                    pageTracker.Add(New PageTrace(currentPage.ToString()))
                End If

                Session("PageTrace") = pageTracker.PageItem(currentPage.ToString())
                '--------------------------------------------------------------------

            End If

        End If

    End Sub

#Region " Private "

    Private Function CreateScriptErrorPage(errorMessage As String)

        Dim resourceManager As New System.Resources.ResourceManager("Onit.OnAssistnet.OnVac.Messages", System.Reflection.Assembly.GetExecutingAssembly())

        Dim errorPage As String = resourceManager.GetString("error_page")

        Dim includeStyleError As String = String.Empty
        Dim includeScriptNavigation As String = String.Empty
        Dim includeScriptLayout As String = String.Empty

        If (HttpContext.Current IsNot Nothing) Then
            Dim page As Page = If(TryCast(HttpContext.Current.Handler, Page), New Page())

            includeStyleError = "<link href=""" + VirtualPathUtility.ToAbsolute("~/Css/OnAssistnet/error.css") + """ rel=""stylesheet"" type=""text/css"" />"
            includeScriptNavigation = "<script src=""" + page.ClientScript.GetWebResourceUrl(GetType(WebResources), "Onit.Shared.Web.Static.Scripts.Navigation.js") + """ type=""text/javascript"" ></script>"
            includeScriptLayout = "<script src=""" + VirtualPathUtility.ToAbsolute("~/Scripts/OnitLayout.js") + """ type=""text/javascript"" ></script>"
        End If

        Try
            Dim checkNavigation As Boolean = Boolean.Parse(ConfigurationManager.AppSettings("CheckNavigation"))

            If Not checkNavigation Then
                includeScriptNavigation = String.Empty
            End If

        Catch ex As Exception

            includeScriptNavigation = String.Empty

        End Try

        Return String.Format(errorPage, includeStyleError, includeScriptNavigation, includeScriptLayout, Date.Now.ToString(), Encoder.HtmlEncode(errorMessage))

    End Function

#End Region

End Class
