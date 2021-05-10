Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL


Public Class BizAnaVaccinazioni
    Inherits BizClass

#Region " Constants "

    Public Const MAXLENGTH_TITOLO As Integer = 100

#End Region

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos)
        MyBase.New(genericprovider, settings, contextInfos, Nothing)
    End Sub

#End Region

#Region " Public "

    Public Function GetVaccinazioneInfo(codiceVaccinazione As String) As Entities.VaccinazioneInfo

        Return Me.GenericProviderCentrale.AnaVaccinazioni.GetVaccinazioneInfo(codiceVaccinazione)

    End Function

    Public Class SaveVaccinazioneInfoResult

        Public Success As Boolean
        Public Message As String

        Public Sub New()
            Me.New(True, String.Empty)
        End Sub

        Public Sub New(success As Boolean, message As String)
            Me.Success = success
            Me.Message = message
        End Sub

    End Class

    Public Function SaveVaccinazioneInfo(id As Integer?, codiceVaccinazione As String, infoTitolo As String, infoDescrizione As String) As SaveVaccinazioneInfoResult

        If String.IsNullOrWhiteSpace(codiceVaccinazione) Then
            Return New SaveVaccinazioneInfoResult(False, "Salvataggio non effettuato: codice vaccinazione mancante.")
        End If

        If Not String.IsNullOrWhiteSpace(infoTitolo) Then
            If infoTitolo.Length > MAXLENGTH_TITOLO Then
                infoTitolo = infoTitolo.Substring(0, MAXLENGTH_TITOLO)
            End If
        End If

        Dim info As New Entities.VaccinazioneInfo()

        info.Id = id
        info.CodiceVaccinazione = codiceVaccinazione

        If String.IsNullOrWhiteSpace(infoTitolo) Then
            info.Titolo = String.Empty
        Else
            info.Titolo = infoTitolo
        End If

        If String.IsNullOrWhiteSpace(infoDescrizione) Then
            info.Descrizione = String.Empty
        Else
            info.Descrizione = infoDescrizione
        End If

        If Not info.Id.HasValue Then

            Me.GenericProviderCentrale.AnaVaccinazioni.InsertVaccinazioneInfo(info)

        Else

            If String.IsNullOrEmpty(info.Titolo) AndAlso String.IsNullOrEmpty(info.Descrizione) Then
                Me.GenericProviderCentrale.AnaVaccinazioni.DeleteVaccinazioneInfo(info.Id.Value)
            Else
                Me.GenericProviderCentrale.AnaVaccinazioni.UpdateVaccinazioneInfo(info)
            End If

        End If

        Return New SaveVaccinazioneInfoResult()

    End Function

#End Region

#Region " OnVac APP "

    ''' <summary>
    ''' Restituisce la lista (codice e descrizione) delle vaccinazioni presenti in anagrafe, includendo solo quelle configurate per la visualizzazione nella APP.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetListVaccinazioniAPP() As List(Of Entities.VaccinazioneAPP)

        Return Me.GenericProvider.AnaVaccinazioni.GetListVaccinazioniAPP()

    End Function

    ''' <summary>
    ''' Restituisce le info relative all'associazione specificata, incluse tutte le vaccinazioni ad essa relative.
    ''' </summary>
    ''' <param name="codiceVaccinazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetVaccinazioneInfoAPP(codiceVaccinazione As String) As Entities.VaccinazioneInfoAPP

        Dim infoApp As New Entities.VaccinazioneInfoAPP()

        Dim info As Entities.VaccinazioneInfo = Me.GenericProvider.AnaVaccinazioni.GetVaccinazioneInfo(codiceVaccinazione)
        If Not info Is Nothing Then
            infoApp.CodiceVaccinazione = info.CodiceVaccinazione
            infoApp.DescrizioneVaccinazione = info.DescrizioneVaccinazione
            infoApp.Titolo = info.Titolo
            infoApp.Descrizione = info.Descrizione
            infoApp.AssociazioniAPP = Me.GenericProvider.Associazioni.GetListAssociazioniAPP(codiceVaccinazione)
        End If

        Return infoApp

    End Function

#End Region

End Class
