Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL

Public Class BizVieSomministrazione
	Inherits BizClass

#Region " Constructors "

	Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos)

		MyBase.New(genericprovider, settings, contextInfos, Nothing)

	End Sub

#End Region

#Region " Public "

	''' <summary>
	''' Restituisce le vie di somministrazione
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function GetVieSomministrazione() As List(Of Entities.ViaSomministrazione)

		Return GenericProvider.VieSomministrazione.GetVieSomministrazione()

	End Function

#End Region

End Class
