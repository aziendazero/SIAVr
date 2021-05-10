Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.Log.DataLogStructure

Public Class BizCodiciStruttura
    Inherits BizClass

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)

        Me.New(genericprovider, settings, contextInfos, Nothing, logOptions)

    End Sub

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos, bizUslGestitaAllineaSettingsProvider As BizUslGestitaAllineaSettingsProvider, logOptions As BizLogOptions)

        MyBase.New(genericprovider, settings, bizUslGestitaAllineaSettingsProvider, contextInfos, logOptions)

    End Sub

#End Region

#Region "Types"

    <Serializable>
    Public Class CodiciHspResult

        Public ListCodiciHsp As List(Of CodiceHSP)
        Public CountCodiciHsp As Integer

    End Class

    <Serializable>
    Public Class CodiciHspFiltri

        Public FiltroRicerca As String
        Public PageIndex As Integer
        Public PageSize As Integer

    End Class

    <Serializable>
    Public Class CodiciStsResult

        Public ListCodiciSts As List(Of CodiceSTS)
        Public CountCodiciSts As Integer

    End Class

    <Serializable>
    Public Class CodiciStsFiltri

        Public FiltroRicerca As String
        Public PageIndex As Integer
        Public PageSize As Integer

    End Class

#End Region

#Region " Public "

#Region " HSP "
    Public Function GetCodiciHsp(filtri As CodiciHspFiltri) As CodiciHspResult

        Dim result As New CodiciHspResult
        Dim pagingOptions As New Data.PagingOptions()

        pagingOptions.StartRecordIndex = filtri.PageIndex * filtri.PageSize
        pagingOptions.EndRecordIndex = pagingOptions.StartRecordIndex + filtri.PageSize

        result.ListCodiciHsp = GenericProvider.CodiciStruttura.GetCodiciHsp(pagingOptions)
        result.CountCodiciHsp = GenericProvider.CodiciStruttura.GetCountCodiciHsp()

        Return result

    End Function

    Public Function GetCodiciHspByFiltro(filtri As CodiciHspFiltri) As CodiciHspResult

        Dim result As New CodiciHspResult
        Dim pagingOptions As New Data.PagingOptions()

        pagingOptions.StartRecordIndex = filtri.PageIndex * filtri.PageSize
        pagingOptions.EndRecordIndex = pagingOptions.StartRecordIndex + filtri.PageSize

        result.ListCodiciHsp = GenericProvider.CodiciStruttura.GetCodiciHspByFiltro(filtri.FiltroRicerca, pagingOptions)
        result.CountCodiciHsp = GenericProvider.CodiciStruttura.GetCountCodiciHspByFiltro(filtri.FiltroRicerca)

        Return result

    End Function

    Public Function GetCodiciHspValidiByFiltro(filtri As CodiciHspFiltri) As CodiciHspResult

        Dim result As New CodiciHspResult
        Dim pagingOptions As New Data.PagingOptions()

        pagingOptions.StartRecordIndex = filtri.PageIndex * filtri.PageSize
        pagingOptions.EndRecordIndex = pagingOptions.StartRecordIndex + filtri.PageSize

        result.ListCodiciHsp = GenericProvider.CodiciStruttura.GetCodiciHspValidiByFiltro(filtri.FiltroRicerca, pagingOptions)
        result.CountCodiciHsp = GenericProvider.CodiciStruttura.GetCountCodiciHspValidiByFiltro(filtri.FiltroRicerca)

        Return result

    End Function

    Public Function GetDettaglioCodiceHspById(id As String) As List(Of CodiceHSP)

        Return GenericProvider.CodiciStruttura.GetDettaglioCodiceHspById(id)

    End Function

    Public Sub UpdateCodiceHps(command As CodiceHSPCommand)

        command.Codice.Trim().ToUpper()
        command.Descrizione.Trim().ToUpper()
        command.CodiceAsl.Trim().ToUpper()
        command.Indirizzo.Trim().ToUpper()
        command.CodiceComune.Trim().ToUpper()

        GenericProvider.CodiciStruttura.UpdateCodiceHsp(command)

    End Sub

    'Public Sub EliminaCodiceHsp(codice As String)

    '    GenericProvider.CodiciHsp.EliminaCodiceHps(codice)

    'End Sub

    Public Function AggiungiCodiceHsp(command As CodiceHSPCommand) As BizGenericResult

        command.Codice.Trim().ToUpper()
        command.Descrizione.Trim().ToUpper()
        command.CodiceAsl.Trim().ToUpper()
        command.Indirizzo.Trim().ToUpper()
        command.CodiceComune.Trim().ToUpper()

        Dim result As New BizGenericResult()

        result.Success = True
        result.Message = String.Empty

        Dim controllo As Boolean = GenericProvider.CodiciStruttura.IsCodiceInCodiciHsp(command.Codice)

        ' controllo lunghezza campi inserimento
        If command.Codice.Length > command.CodiceMaxLength Then
            result.Success = False
            result.Message = "Impossibile completare l'operazione. Il campo codice è troppo lungo!"

        ElseIf command.Descrizione.Length > command.DescrizioneMaxLength Then
            result.Success = False
            result.Message = "Impossibile completare l'operazione. Il campo descrizione è troppo lungo!"

        ElseIf command.Indirizzo.Length > command.IndirizzoMaxLength Then
            result.Success = False
            result.Message = "Impossibile completare l'operazione. Il campo indirizzo è troppo lungo!"

        ElseIf String.IsNullOrWhiteSpace(command.Codice) AndAlso String.IsNullOrWhiteSpace(command.Descrizione) Then
            result.Success = False
            result.Message = "Impossibile completare l'operazione. I campi codice e descrizione sono vuoti!"

        ElseIf String.IsNullOrWhiteSpace(command.Descrizione) Then
            result.Success = False
            result.Message = "Impossibile completare l'operazione. Il campo descrizione è vuoto!"

        ElseIf String.IsNullOrWhiteSpace(command.Codice) Then
            result.Success = False
            result.Message = "Impossibile completare l'operazione. Il campo codice è vuoto!"

        ElseIf controllo = False Then
            GenericProvider.CodiciStruttura.AggiungiCodiceHsp(command)
        Else
            result.Success = False
            result.Message = "Impossibile completare l'operazione. Codice già esistente!"
        End If

        Return result

    End Function

#End Region

#Region " STS "

    Public Function GetCodiciSts(filtri As CodiciStsFiltri) As CodiciStsResult

        Dim result As New CodiciStsResult
        Dim pagingOptions As New Data.PagingOptions()

        pagingOptions.StartRecordIndex = filtri.PageIndex * filtri.PageSize
        pagingOptions.EndRecordIndex = pagingOptions.StartRecordIndex + filtri.PageSize

        result.ListCodiciSts = GenericProvider.CodiciStruttura.GetCodiciSts(pagingOptions)
        result.CountCodiciSts = GenericProvider.CodiciStruttura.GetCountCodiciSts()

        Return result

    End Function

    Public Function GetCodiciStsByFiltro(filtri As CodiciStsFiltri) As CodiciStsResult

        Dim result As New CodiciStsResult
        Dim pagingOptions As New Data.PagingOptions()

        pagingOptions.StartRecordIndex = filtri.PageIndex * filtri.PageSize
        pagingOptions.EndRecordIndex = pagingOptions.StartRecordIndex + filtri.PageSize

        result.ListCodiciSts = GenericProvider.CodiciStruttura.GetCodiciStsByFiltro(filtri.FiltroRicerca, pagingOptions)
        result.CountCodiciSts = GenericProvider.CodiciStruttura.GetCountCodiciStsByFiltro(filtri.FiltroRicerca)

        Return result

    End Function

    Public Function GetDettaglioCodiceStsById(id As String) As List(Of CodiceSTS)

        Return GenericProvider.CodiciStruttura.GetDettaglioCodiceStsById(id)

    End Function

    Public Sub UpdateCodiceSts(command As CodiceSTSCommand)

        command.Codice.Trim().ToUpper()
        command.Descrizione.Trim().ToUpper()
        command.CodiceAsl.Trim().ToUpper()
        command.Indirizzo.Trim().ToUpper()
        command.CodiceComune.Trim().ToUpper()

        GenericProvider.CodiciStruttura.UpdateCodiceSts(command)

    End Sub

    'Public Sub EliminaCodiceHsp(codice As String)

    '    GenericProvider.CodiciHsp.EliminaCodiceHps(codice)

    'End Sub

    Public Function AggiungiCodiceSts(command As CodiceSTSCommand) As BizGenericResult

        command.Codice.Trim().ToUpper()
        command.Descrizione.Trim().ToUpper()
        command.CodiceAsl.Trim().ToUpper()
        command.Indirizzo.Trim().ToUpper()
        command.CodiceComune.Trim().ToUpper()

        Dim result As New BizGenericResult()

        result.Success = True
        result.Message = String.Empty

        Dim controllo As Boolean = GenericProvider.CodiciStruttura.IsCodiceInCodiciSts(command.Codice)

        ' controllo lunghezza campi inserimento
        If command.Codice.Length > command.CodiceMaxLength Then
            result.Success = False
            result.Message = "Impossibile completare l'operazione. Il campo codice è troppo lungo!"

        ElseIf command.Descrizione.Length > command.DescrizioneMaxLength Then
            result.Success = False
            result.Message = "Impossibile completare l'operazione. Il campo descrizione è troppo lungo!"

        ElseIf command.Indirizzo.Length > command.IndirizzoMaxLength Then
            result.Success = False
            result.Message = "Impossibile completare l'operazione. Il campo indirizzo è troppo lungo!"

        ElseIf String.IsNullOrWhiteSpace(command.Codice) AndAlso String.IsNullOrWhiteSpace(command.Descrizione) Then
            result.Success = False
            result.Message = "Impossibile completare l'operazione. I campi codice e descrizione sono vuoti!"

        ElseIf String.IsNullOrWhiteSpace(command.Descrizione) Then
            result.Success = False
            result.Message = "Impossibile completare l'operazione. Il campo descrizione è vuoto!"

        ElseIf String.IsNullOrWhiteSpace(command.Codice) Then
            result.Success = False
            result.Message = "Impossibile completare l'operazione. Il campo codice è vuoto!"

        ElseIf controllo = False Then
            GenericProvider.CodiciStruttura.AggiungiCodiceSts(command)
        Else
            result.Success = False
            result.Message = "Impossibile completare l'operazione. Codice già esistente!"
        End If

        Return result

    End Function

#End Region

#End Region

End Class