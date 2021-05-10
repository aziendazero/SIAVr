Imports System.Collections.Generic

Public Interface IUslProvider
    Function GetDescrizione(codUsl As String) As String
    Function GetCodiceRegioneAusl(codUsl As String) As String
    Function GetCodiciUslByComune(codiceComune As String) As String()
    Function GetUslGestite() As ICollection(Of Usl)
    Function GetUslGestitaByCodiceComune(codiceComune As String) As Usl
    Sub UpdateUslGestita(uslGestita As Usl)
    Function GetCodiceAifa(codUsl As String) As String
    Function GetUslUnificataByCodiceUsl(codiceUsl As String) As UslUnificata
    Function GetUslUnificataByIdApplicazione(idApplicazione As String) As UslUnificata
    Function IsInUslUnificata(codiceUslLocale As String, codiceUslUnificata As String) As Boolean
    Function GetCodiceUslUnificataByIdApplicazioneUslGestita(idApplicazioneUslGestita As String) As String
    Function GetUslDistretto() As List(Of UslDistretto)
    Function GetInfoAziendaByIdApplicativo(idApplicativo As String) As UslApplicativo
    Function IsUnificata(codiceUsl As String) As Boolean
	Function GetUslUnificataByCodiceAcn(codiceAcn As String) As UslUnificata
	Function IsAutenticatoRicercaLotti(codiceUsl As String, user As String, pw As String) As Boolean
End Interface
