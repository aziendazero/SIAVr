Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL

Public Class BizEsitoControlloScuole
    Inherits BizClass

#Region " Constructors "

    Public Sub New(genericProvider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos)

        MyBase.New(genericProvider, settings, contextInfos, Nothing)

    End Sub

#End Region
#Region "Type"
    <Serializable>
    Public Class GetListaEsitoControlloScuoleResult

        Public ListaEsitoControlli As List(Of Entities.EsitoControlloScuole)
        Public CountEsitoControlli As Integer

    End Class
    Public Class GetEsitoControlloScuoleCommand

        Public Filtri As Entities.FiltroEsitoControlloScuole
        Public CampoOrdinamento As String
        Public VersoOrdinamento As String
        Public PageIndex As Integer
        Public PageSize As Integer

    End Class
    Public Enum StatoEstrazioneControllo
        ElaboratoConErrore = 0
        InRegola = 1
        ParzialmenteInRegola = 2
        NonInRegola = 3
    End Enum
#End Region

#Region " Public "

#Region " ElaborazioneControlli "

    ''' <summary>
    ''' Restituisce i dati relativi al bilancio specificato.
    ''' </summary>
    ''' <param name="numeroBilancio"></param>
    ''' <param name="codiceMalattia"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetListaElaborazioneControlli(command As GetEsitoControlloScuoleCommand) As GetListaEsitoControlloScuoleResult
        If command Is Nothing Then Throw New ArgumentNullException()

        Dim list As List(Of Entities.EsitoControlloScuole) = Nothing

        Dim count As Integer = GenericProvider.EsitoControlloScuole.CountListaEsitoControlloScuole(command.Filtri)
        If count = 0 Then
            list = New List(Of Entities.EsitoControlloScuole)
        Else
            Dim pagingOptions As New Data.PagingOptions()
            pagingOptions.StartRecordIndex = command.PageIndex * command.PageSize
            pagingOptions.EndRecordIndex = pagingOptions.StartRecordIndex + command.PageSize

            list = GenericProvider.EsitoControlloScuole.GetListaEsitoControlloScuole(command.Filtri, command.CampoOrdinamento, command.VersoOrdinamento, pagingOptions)
        End If

        Dim result As New GetListaEsitoControlloScuoleResult()
        result.CountEsitoControlli = count
        result.ListaEsitoControlli = list
        Return result

    End Function




#End Region





#End Region


End Class
