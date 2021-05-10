Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL

Public Class BizElaborazioneControlli
    Inherits BizClass

#Region " Constructors "

    Public Sub New(genericProvider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos)

        MyBase.New(genericProvider, settings, contextInfos, Nothing)

    End Sub

#End Region
#Region "Type"
    <Serializable>
    Public Class GetListaElaborazioneControlliResult

        Public ListaElaborazioneControlli As List(Of Entities.ElaborazioneControlli)
        Public CountElaborazioneControlli As Integer

    End Class
    Public Class GetElaborazioneControlliCommand

        Public Filtri As Entities.FiltroElaborazioneControlli
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
    Public Function GetListaElaborazioneControlli(command As GetElaborazioneControlliCommand) As GetListaElaborazioneControlliResult

        If command Is Nothing Then Throw New ArgumentNullException()

        Dim list As List(Of Entities.ElaborazioneControlli) = Nothing

        Dim count As Integer = GenericProvider.ElaborazioneControlli.CountListaElaborazioneControlli(command.Filtri)

        If count = 0 Then
            list = New List(Of Entities.ElaborazioneControlli)()
        Else
            Dim pagingOptions As New Data.PagingOptions()
            pagingOptions.StartRecordIndex = command.PageIndex * command.PageSize
            pagingOptions.EndRecordIndex = pagingOptions.StartRecordIndex + command.PageSize

            list = GenericProvider.ElaborazioneControlli.GetListaElaborazioneControlli(command.Filtri, command.CampoOrdinamento, command.VersoOrdinamento, pagingOptions)
        End If

        Dim result As New GetListaElaborazioneControlliResult()
        result.CountElaborazioneControlli = count
        result.ListaElaborazioneControlli = list

        Return result

    End Function

    ''' <summary>
    ''' Inserimento testata e esecuzione procedura controlli
    ''' </summary>
    ''' <param name="datiTestata"></param>
    ''' <returns></returns>
    Public Function EseguiControlli(datiTestata As Entities.InsertControlliTesta) As Integer

        Dim id As Integer = GenericProvider.ElaborazioneControlli.InsertTestataControlli(datiTestata)

        Return GenericProvider.ElaborazioneControlli.EseguiStoredProcedureControlli(id)

    End Function

    Public Function GetElaborazioneControlloXCodiceProc(filter As Entities.FiltroSingolaElaborazioneControlli) As Entities.ElaborazioneControlli

        Return GenericProvider.ElaborazioneControlli.GetElaborazioneControlloXCodiceProc(filter)

    End Function

#End Region

#End Region

End Class
