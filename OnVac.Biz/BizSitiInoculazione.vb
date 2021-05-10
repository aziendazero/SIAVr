Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL

Public Class BizSitiInoculazione
	Inherits BizClass

#Region " Constructors "

	Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos)

		MyBase.New(genericprovider, settings, contextInfos, Nothing)

	End Sub

#End Region

#Region " Public "

	''' <summary>
	''' Restituisce i siti di inoculazione
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function GetSitiInoculazione() As List(Of Entities.SitoInoculazione)

		Return GenericProvider.SitiInoculazione.GetSitiInoculazione()

	End Function

#End Region

End Class
