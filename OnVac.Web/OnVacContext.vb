Imports System.Web.Security

Imports Onit.OnAssistnet.Web
Imports Onit.Shared.Manager.OnitProfile
Imports Onit.Shared.Manager.Security
Imports Onit.OnAssistnet.Web.UI


Public Class OnVacContext
    Inherits OnAssistnetContext

#Region " Properties "

    Public Shared Property MenuDis() As String
        Get
            Return DirectCast(HttpContext.Current.Session("OnVacMenuDis"), String)
        End Get
        Set(Value As String)
            HttpContext.Current.Session("OnVacMenuDis") = Value
        End Set
    End Property

    Public Shared ReadOnly Property Connection() As ConnectionInfo
        Get
            Dim cnInfo As ConnectionInfo = Nothing
            Dim cr As New Onit.Shared.NTier.Security.Crypto([Shared].NTier.Security.Providers.Rijndael)
            If (HttpContext.Current.Session("OnVacContext_Provider") Is Nothing OrElse HttpContext.Current.Session("OnVacContext_CnString") Is Nothing) Then
                ' recupero dal manager
                Dim app As Onit.Shared.Manager.Apps.App = Onit.Shared.Manager.Apps.App.getInstance(OnVacContext.AppId, OnVacContext.Azienda)
                Dim cn As Onit.Shared.NTier.Dal.DAAB.DbConnectionInfo = app.getConnectionInfo()
                ' todo: replace da eliminare quando si migra la onitreport dalla shared
                'replace perchè onit.shared.report non vuole gli spazi
                cnInfo = New ConnectionInfo(cn.Provider.ToString(), cn.ConnectionString.Replace("; ", ";"))

                ' cache in sessione
                HttpContext.Current.Session("OnVacContext_Provider") = cnInfo.Provider
                HttpContext.Current.Session("OnVacContext_CnString") = cr.Encrypt(cnInfo.ConnectionString)
            Else
                cnInfo = New ConnectionInfo(HttpContext.Current.Session("OnVacContext_Provider"), cr.Decrypt(HttpContext.Current.Session("OnVacContext_CnString").ToString()))
            End If

            Return cnInfo
        End Get
    End Property

    Public Shared Function GetEncryptedConnectionString() As String
        Dim result As String = HttpContext.Current.Session("OnVacContext_CnString")
        If (result Is Nothing) Then
            Dim cr As New Onit.Shared.NTier.Security.Crypto([Shared].NTier.Security.Providers.Rijndael)
            Return cr.Encrypt(Connection.ConnectionString)
        Else
            Return result
        End If
    End Function

#End Region

#Region " Public "

    Public Shared Function CreateBizContextInfos() As Biz.BizContextInfos

        Return New Biz.BizContextInfos(OnVacContext.UserId, OnVacContext.Azienda, OnVacContext.AppId, String.Empty, OnVacContext.CodiceUslCorrente, Nothing)

    End Function

    Public Shared Function CreateBizContextInfos(codiceConsultorio As String) As Biz.BizContextInfos

        Return New Biz.BizContextInfos(OnVacContext.UserId, OnVacContext.Azienda, OnVacContext.AppId, codiceConsultorio, OnVacContext.CodiceUslCorrente, Nothing)

    End Function

    ''' <summary>
    ''' Restituisce un oggetto contenente le informazioni di autenticazione per l'utente corrente
    ''' </summary>
    Public Shared Function GetCurrentUserAuthenticationInfo(password As String) As UserAuthenticationInfo

        ' Codice utente corrente
        Dim codiceUtente As String = OnVacContext.UserCode

        ' Lettura parametro web.config
        Dim isLdapMembershipProvider As Boolean

        Try
            isLdapMembershipProvider = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("IsLdapMembershipProvider"))
        Catch ex As Exception
            isLdapMembershipProvider = False
        End Try

        ' Validazione utente
        Dim autenticato As Boolean
        Dim erroreAutenticazione As String = String.Empty

        Try

            Dim user As Onit.Shared.Manager.Entities.T_ANA_UTENTI = UserDbManager.GetCurrentUser()

            If isLdapMembershipProvider Then
                Dim dominio As String = String.Empty
                Dim dom As Onit.Shared.Manager.Entities.T_ANA_DOMINI = UserDbManager.GetCurrentDomain()
                If Not dom Is Nothing Then
                    dominio = dom.DOM_MNEMONICO
                End If
                autenticato = Membership.ValidateUser(String.Format("{0}{1}{2}", dominio, IIf(String.IsNullOrEmpty(dominio), "", "\\"), user.UTE_CODICE), password)
            Else
                autenticato = Membership.ValidateUser(codiceUtente, password)
            End If

        Catch autenticateException As Onit.Shared.Manager.Exceptions.AutenticateException

            erroreAutenticazione = autenticateException.Message

            autenticato = False

        End Try

        If Not autenticato Then

            If String.IsNullOrEmpty(erroreAutenticazione) OrElse erroreAutenticazione.Trim() = String.Empty Then

                erroreAutenticazione = "La password digitata non è valida"

            End If

        End If

        Return New UserAuthenticationInfo(codiceUtente, autenticato, erroreAutenticazione, isLdapMembershipProvider)

    End Function

#End Region

#Region " Types "

    Public Class UserAuthenticationInfo

#Region " Properties "

        Private _userCode As String
        Public Property UserCode() As String
            Get
                Return _userCode
            End Get
            Set(ByVal value As String)
                _userCode = value
            End Set
        End Property

        Private _isAuthenticated As Boolean
        Public Property IsAuthenticated() As Boolean
            Get
                Return _isAuthenticated
            End Get
            Set(ByVal value As Boolean)
                _isAuthenticated = value
            End Set
        End Property

        Private _authenticationError As String
        Public Property AuthenticationError() As String
            Get
                Return _authenticationError
            End Get
            Set(ByVal value As String)
                _authenticationError = value
            End Set
        End Property

        Private _isLdapMembershipProvider As Boolean
        Public Property IsLdapMembershipProvider() As Boolean
            Get
                Return _isLdapMembershipProvider
            End Get
            Set(ByVal value As Boolean)
                _isLdapMembershipProvider = value
            End Set
        End Property

#End Region

#Region " Constructors "

        Public Sub New()

        End Sub

        Public Sub New(userCode As String, isAuthenticated As Boolean, authenticationError As String, isLdapMembershipProvider As Boolean)

            Me.UserCode = userCode

            Me.IsAuthenticated = isAuthenticated

            Me.AuthenticationError = authenticationError

            Me.IsLdapMembershipProvider = isLdapMembershipProvider

        End Sub

#End Region

    End Class

#End Region

    ''' <summary>
    ''' Assicura l'esistenza di uno stato di sessione valido.
    ''' in caso contrario lancia una eccezione
    ''' </summary>
    ''' <remarks></remarks>
    Shared Sub AssertSession()
        If IsSessionTimeout() Then
            Throw New ArgumentNullException()
        End If
    End Sub

    ''' <summary>
    ''' indica se la sessione è scaduta
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function IsSessionTimeout() As Boolean
        Return HttpContext.Current.Session Is Nothing OrElse OnVacContext.AppId Is Nothing
    End Function

End Class

