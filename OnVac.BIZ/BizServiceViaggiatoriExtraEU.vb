Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities

Public Class BizServiceViaggiatoriExtraEU
    Inherits BizClass
    ''' <summary>
    ''' Biz centralizzato delle funzionalità utilizzate per il ServiceCOVID19
    ''' </summary>
    ''' <param name="genericprovider"></param>
    ''' <param name="settings"></param>
    ''' <param name="contextInfos"></param>

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos)

        MyBase.New(genericprovider, settings, contextInfos, Nothing)

    End Sub

#End Region

    Public Function GetComuni() As List(Of Comune)
        Using BizComuni As New BizComuni(GenericProvider, Settings, ContextInfos, Nothing)
            Return BizComuni.GetComuni()
        End Using
    End Function
    Public Function GetStati(acronimo As String) As List(Of Stato)
        Using BizStatiAnagrafici As New BizStatiAnagrafici(GenericProvider, Settings, ContextInfos)
            Return BizStatiAnagrafici.GetStati(acronimo)
        End Using
    End Function

    Public Function GetPazienteByCF(CF As String) As List(Of InfoAssistito)
        Using BizPaziente As New BizPaziente(GenericProvider, Settings, ContextInfos, Nothing)
            Return BizPaziente.GetPazientiByCF(CF)
        End Using
    End Function

    Public Function NuovoEpisodio(episodio As EpisodioPaziente) As Long
        Using BizServiceCOVID19 As New BizServiceCovid19(GenericProvider, Settings, ContextInfos)
            Return BizServiceCOVID19.SalvaNuovoEpisodio(episodio)
        End Using
    End Function

    Public Function GetEpisodiByIdPaz(id As Long) As List(Of EpisodioPaziente)
        Using BizServiceCOVID19 As New BizServiceCovid19(GenericProvider, Settings, ContextInfos)
            Return BizServiceCOVID19.GetEpisodiByPaziente(id)
        End Using
    End Function
    Public Function GetStatiEpisodio() As List(Of StatiEpisodio)
        Using BizRilevazioniCovid19 As New BizRilevazioniCovid19(GenericProvider, Settings, ContextInfos)
            Return BizRilevazioniCovid19.GetStatiEpisodio()
        End Using
    End Function
    Public Function GetCodifiche(campo As String) As Collection.CodificheCollection
        Using BizCodifiche As New BizCodifiche(GenericProvider, Settings, ContextInfos, Nothing)
            Return BizCodifiche.GetCodifiche(campo)
        End Using
    End Function
End Class
