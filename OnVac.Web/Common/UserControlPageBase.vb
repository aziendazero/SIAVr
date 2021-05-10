Imports Onit.OnAssistnet.OnVac


Namespace Common

    ''' <summary>
    ''' Classe che estende la user control di sistema per prevedere l'utilizzo dei parametri applicativi.
    ''' </summary>
    Public Class UserControlPageBase
        Inherits Onit.Shared.Web.UI.UserControl

#Region " Gestione Parametri "

        Private _settings As Settings.Settings = Nothing
        Public ReadOnly Property Settings() As Settings.Settings
            Get
                If _settings Is Nothing Then
                    If Me.Page.GetType().IsSubclassOf(GetType(PageBase)) Then
                        _settings = DirectCast(Me.Page, PageBase).Settings
                    Else
                        _settings = LoadSettingsFromDb(OnVacUtility.Variabili.CNS.Codice)
                    End If
                End If
                Return _settings
            End Get
        End Property

        Friend Shared Function LoadSettingsFromDb(cnsCodice As String) As Settings.Settings

            Dim _sets As Settings.Settings

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                _sets = New Settings.Settings(cnsCodice, genericProvider)

            End Using

            Return _sets

        End Function

#End Region

#Region " Flag Dati Vaccinali Usl Gestite "

        Private Enum TipoFlagUslGestita
            FlagAbilitazioneVacc = 0
            FlagVisibilitaVacc = 1
        End Enum

        Private _FlagAbilitazioneVaccUslCorrente As Boolean? = Nothing

        ''' <summary>
        ''' Flag che indica se la usl corrente è abilitata alla gestione centralizzata dello storico vaccinale
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property FlagAbilitazioneVaccUslCorrente() As Boolean
            Get
                If Not _FlagAbilitazioneVaccUslCorrente.HasValue Then
                    _FlagAbilitazioneVaccUslCorrente = GetFlagVaccUslCorrente(TipoFlagUslGestita.FlagAbilitazioneVacc)
                End If
                Return _FlagAbilitazioneVaccUslCorrente.Value
            End Get
        End Property

        Private _FlagConsensoVaccUslCorrente As Boolean? = Nothing

        ''' <summary>
        ''' Flag che indica se la usl corrente ha dato il consenso alla distribuzione dei propri dati vaccinali
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property FlagConsensoVaccUslCorrente() As Boolean
            Get
                If Not _FlagConsensoVaccUslCorrente.HasValue Then
                    _FlagConsensoVaccUslCorrente = GetFlagVaccUslCorrente(TipoFlagUslGestita.FlagVisibilitaVacc)
                End If
                Return _FlagConsensoVaccUslCorrente.Value
            End Get
        End Property

        Private Function GetFlagVaccUslCorrente(tipoFlag As TipoFlagUslGestita) As Boolean

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                Using bizUsl As New Biz.BizUsl(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    If Not bizUsl.UslGestitaCorrente Is Nothing Then

                        If tipoFlag = TipoFlagUslGestita.FlagAbilitazioneVacc Then

                            Return bizUsl.UslGestitaCorrente.FlagAbilitazioneDatiVaccinaliCentralizzati

                        ElseIf tipoFlag = TipoFlagUslGestita.FlagVisibilitaVacc Then

                            Return bizUsl.UslGestitaCorrente.FlagConsensoDatiVaccinaliCentralizzati

                        Else

                            Throw New NotSupportedException()

                        End If

                    End If

                End Using

            End Using

            Return False

        End Function

#End Region

#Region " Custom Script "

        Protected Friend Sub RegisterStartupScriptCustom(key As String, scriptContent As String)

            ' Define the name and type of the client scripts on the page. 
            Dim cstype As Type = Me.[GetType]()

            ' Get a ClientScriptManager reference from the Page class. 
            Dim cs As ClientScriptManager = Page.ClientScript

            ' Check to see if the startup script is already registered. 
            If Not cs.IsStartupScriptRegistered(cstype, key) Then

                Dim s As New System.Text.StringBuilder()

                s.Append("<script type=text/javascript>")
                s.Append(scriptContent)
                s.Append("</script>")

                cs.RegisterStartupScript(cstype, key, s.ToString())

            End If

        End Sub

#End Region

#Region " StringResources "

        ''' <summary>
        ''' Restituisce il messaggio in base alla stringa di risorse specificata
        ''' </summary>
        ''' <param name="resourceKey"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Function GetOnVacResourceValue(resourceKey As String) As String

            Return GetGlobalResourceObject("Onit.OnAssistnet.OnVac.Web", resourceKey).ToString()

        End Function

#End Region

    End Class

End Namespace

