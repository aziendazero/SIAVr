Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL

Public Class BizTag
    Inherits BizClass

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)
        MyBase.New(genericprovider, settings, contextInfos, logOptions)
    End Sub

#Region "Public"
    Public Function CercaTag(gruppo As String, filtro As String) As IEnumerable(Of Entities.Tag)
        Return GenericProvider.Tag.CercaTag(gruppo, filtro)
    End Function

    Public Function GetTags(elementi As IEnumerable(Of Long)) As IEnumerable(Of Entities.Tag)
        If elementi Is Nothing Or Not elementi.Any() Then
            Return New List(Of Entities.Tag)
        End If
        Return GenericProvider.Tag.GetTags(elementi.ToArray())
    End Function

    Public Function GetTag(id As Long) As Entities.Tag
        Dim tmp As New List(Of Long)
        tmp.Add(id)
        Return GetTags(tmp).FirstOrDefault()
    End Function
#End Region
End Class
