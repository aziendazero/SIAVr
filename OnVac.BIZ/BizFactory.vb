Imports System.Reflection

Imports Onit.Shared.NTier.Dal.DAAB
Imports Onit.Shared.Manager.Apps

Imports Onit.OnAssistnet.OnVac.DAL

Public Class BizFactory

#Region " Singleton "

    Private Shared ReadOnly syncRoot As Object

    Shared Sub New()
        syncRoot = New Object()
    End Sub

    Protected Sub New()

    End Sub

    Private Shared _Instance As BizFactory
    Public Shared ReadOnly Property Instance() As BizFactory
        Get
            If _Instance Is Nothing Then
                SyncLock (syncRoot)
                    If _Instance Is Nothing Then
                        _Instance = New BizFactory()
                    End If
                End SyncLock
            End If
            Return _Instance
        End Get
    End Property

#Region " BizPaziente "

    Public Function CreateBizPaziente(genericprovider As DbGenericProvider, settings As Settings.Settings, uslGestitaAllineaSettingsProvider As BizPaziente.BizUslGestitaAllineaSettingsProvider, contextInfos As BizContextInfos, logOptions As BizLogOptions) As BizPaziente

        If String.IsNullOrEmpty(settings.BIZ_PAZIENTE_TYPE) Then
            Return New BizPaziente(genericprovider, settings, contextInfos, logOptions)
        Else
            Return DirectCast(Activator.CreateInstance(Me.GetTypeParam(settings.BIZ_PAZIENTE_TYPE), New Object() {genericprovider, settings, uslGestitaAllineaSettingsProvider, contextInfos, logOptions}), BizPaziente)
        End If

    End Function

    Public Function CreateBizPaziente(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions) As BizPaziente

        Return Me.CreateBizPaziente(genericprovider, settings, Nothing, contextInfos, logOptions)

    End Function

    Public Function CreateBizPaziente(genericprovider As DbGenericProvider, contextInfos As BizContextInfos, logOptions As BizLogOptions) As BizPaziente

        Dim settings As New Settings.Settings(contextInfos.CodiceCentroVaccinale, genericprovider)

        Return Me.CreateBizPaziente(genericprovider, settings, contextInfos, logOptions)

    End Function

#End Region

    Private Function GetTypeParam(valore As String) As Type

        If String.IsNullOrEmpty(valore) Then Return Nothing

        Dim type As Type = type.GetType(valore.ToString())

        If (type Is Nothing) Then
            Throw New TypeLoadException(String.Format("Type not found: {0}", valore))
        End If

        Return type

    End Function

#End Region

End Class
