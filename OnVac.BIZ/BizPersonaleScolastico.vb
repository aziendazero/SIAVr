Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities
Public Class BizPersonaleScolastico
    Inherits BizClass
#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, parametri As Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)
        MyBase.New(genericprovider, parametri, contextInfos, logOptions)
    End Sub

    Public Sub New(genericProvider As DbGenericProvider, contextInfo As BizContextInfos)

        MyBase.New(genericProvider, Nothing, contextInfo, Nothing)

    End Sub

#End Region

#Region "Setting Personale scolastico"
    Public Function GetDescrizioneSettingScolastico(id As Long) As String

        Return Me.GenericProvider.PersonaleScolastico.GetDescrizioneSettingScolastico(id)

    End Function
#End Region

#Region "Persnale scolastico"
    Public Function GetDescrizioneTipoPersonaleScolastico(id As Long) As String

        Return Me.GenericProvider.PersonaleScolastico.GetDescrizioneTipoPersonaleScolastico(id)

    End Function
#End Region
End Class
