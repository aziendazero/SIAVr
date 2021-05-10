Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities

Public Class BizDocumenti
    Inherits BizClass

#Region " Costruttori "

    Public Sub New(dbGenericProvider As DbGenericProvider, settings As Settings.Settings, uslGestitaAllineaSettingsProvider As BizUslGestitaAllineaSettingsProvider, contextInfos As BizContextInfos, logOptions As BizLogOptions)
        MyBase.New(dbGenericProvider, settings, uslGestitaAllineaSettingsProvider, contextInfos, logOptions)
    End Sub

    Public Sub New(dbGenericProviderFactory As DbGenericProviderFactory, settings As Settings.Settings, uslGestitaAllineaSettingsProvider As BizUslGestitaAllineaSettingsProvider, contextInfos As BizContextInfos, logOptions As BizLogOptions)
        MyBase.New(dbGenericProviderFactory, settings, uslGestitaAllineaSettingsProvider, contextInfos, logOptions)
    End Sub

    Public Sub New(dbGenericProviderFactory As DbGenericProviderFactory, settings As Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)
        MyBase.New(dbGenericProviderFactory, settings, Nothing, contextInfos, logOptions)
    End Sub

    Public Sub New(ByRef settings As Settings.Settings, uslGestitaAllineaSettingsProvider As BizUslGestitaAllineaSettingsProvider, ByVal contextInfos As BizContextInfos, ByVal logOptions As BizLogOptions)
        MyBase.New(settings, uslGestitaAllineaSettingsProvider, contextInfos, logOptions)
    End Sub

    Public Sub New(dbGenericProvider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)
        Me.New(dbGenericProvider, settings, Nothing, contextInfos, logOptions)
    End Sub

#End Region
#Region " OnVac API "
#Region " Get "
    Public Function GetInfoDocumento(Id As Integer) As DocumentoModel
        Dim result As New DocumentoModel
        result = GenericProvider.Documenti.GetInfoDocumento(Id)
        Return result
    End Function

#End Region
#Region " Post "
    Public Function SetDocumento(Documento As DocumentoModel) As ResultSetPost
        Dim result As New ResultSetPost()
        result.Success = True
        If Documento.Documento64.IsNullOrEmpty() Then
            result.Message = "Documento64 non presente"
            result.Success = False
        End If

        If Documento.Tipologia.IsNullOrEmpty() Then
            result.Message = "Tipologia di documento non presente"
            result.Success = False
        End If

        If result.Success Then
            result = GenericProvider.Documenti.SetDocumento(Documento)
        End If

        Return result
    End Function
#End Region
#End Region

End Class
