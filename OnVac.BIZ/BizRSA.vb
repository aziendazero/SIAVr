Imports System.Collections.Generic

Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities

Public Class BizRSA
    Inherits BizClass

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)

        MyBase.New(genericprovider, settings, contextInfos, logOptions)

    End Sub

#End Region

    Public Function GetRSA(ULSS As String, Filtro As String) As List(Of RSA)
        Return GenericProvider.Anagrafiche.GetRSA(ULSS, Filtro)
    End Function

    Public Function GetRSAByIdGruppo(idGruppo As String) As List(Of RSA)
        Return GenericProvider.Anagrafiche.GetRSAByIdGruppo(idGruppo)
    End Function

    Public Function GetPazientiByRSA(idRSA As String, nome As String, cognome As String, cf As String) As List(Of InfoAssistito)
        Return GenericProvider.Paziente.GetPazientiByRSA(idRSA, nome?.Trim().ToUpper(), cognome?.Trim().ToUpper(), cf)
    End Function

    Public Function GetInoculazione() As List(Of SitoInoculazione)
        Return GenericProvider.SitiInoculazione.GetSitiInoculazione()
    End Function

    Public Function GetVieSomministrazione() As List(Of ViaSomministrazione)
        Return GenericProvider.VieSomministrazione.GetVieSomministrazione()
    End Function

    Public Function GetCodiceConsultorioMagazzinoRSA(idRSA As String) As String
        Return GenericProvider.Anagrafiche.GetCodiceConsultorioMagazzinoRSA(idRSA)
    End Function

    ''' <summary>
    ''' Restituisce true se il paziente è in una RSA (una qualsiasi)
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    Public Function IsPazienteInRSA(codicePaziente As Integer) As Boolean
        Return GenericProvider.Paziente.IsPazienteInRSA(codicePaziente)
    End Function

End Class


