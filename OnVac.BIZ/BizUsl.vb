Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities

Public Class BizUsl
	Inherits BizClass

#Region " Constructors "

	Public Sub New(genericprovider As DbGenericProvider, parametri As Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)
		MyBase.New(genericprovider, parametri, contextInfos, logOptions)
	End Sub

	Public Sub New(genericProvider As DbGenericProvider, contextInfo As BizContextInfos)

		MyBase.New(genericProvider, Nothing, contextInfo, Nothing)

	End Sub

#End Region

#Region " Public "

    ''' <summary>
    ''' Restituisce la descrizione della usl in base al codice specificato
    ''' </summary>
    ''' <param name="codiceUsl"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDescrizione(codiceUsl As String) As String
		If String.IsNullOrWhiteSpace(codiceUsl) Then
			Return ""
		End If
		Return Me.GenericProvider.Usl.GetDescrizione(codiceUsl)

    End Function

    Public Function GetCodiceRegione(codiceUsl As String) As String

		Return Me.GenericProvider.Usl.GetCodiceRegioneAusl(codiceUsl)

	End Function

	Public Function GetCodiceAifa(codiceUsl As String) As String

		Return Me.GenericProvider.Usl.GetCodiceAifa(codiceUsl)

	End Function
	Public Function GetCodiceByAcn(codiceAcn As String) As UslUnificata

		Return Me.GenericProvider.Usl.GetUslUnificataByCodiceAcn(codiceAcn)

	End Function

	Public Function GetListaUslDistretto() As List(Of Entities.UslDistretto)

		Return GenericProvider.Usl.GetListaUslDistretto()

	End Function

	Public Function IsInUslUnificata(codiceUslLocale As String, codiceUslUnificata As String) As Boolean

		Return GenericProvider.Usl.IsInUslUnificata(codiceUslLocale, codiceUslUnificata)

	End Function

	''' <summary>
	''' Restituisce sempre il codice della usl unificata, in base all'id dell'applicazione specificato.
	''' Se l'app_id è relativo ad una vecchia usl, restituisce comunque la nuova usl di cui la vecchia fa parte.
	''' </summary>
	''' <param name="idApplicazione"></param>
	''' <returns></returns>
	Public Function GetCodiceUslUnificataByIdApplicazione(idApplicazione As String) As String

		Dim uslUnificata As Entities.UslUnificata = GenericProvider.Usl.GetUslUnificataByIdApplicazione(idApplicazione)

		If Not uslUnificata Is Nothing Then
			Return uslUnificata.CodiceUsl
		End If

		Dim codiceUsl As String = GenericProvider.Usl.GetCodiceUslUnificataByIdApplicazioneUslGestita(idApplicazione)

		If Not String.IsNullOrWhiteSpace(codiceUsl) Then
			Return codiceUsl
		End If

		' Se siamo qui, o è arrivato il codice OnVac050 (che fa parte delle usl gestite) o è arrivato un codice inesistente
		Dim ulss As IEnumerable(Of Entities.Usl) = UslGestite.Where(Function(p) p.IDApplicazione = idApplicazione AndAlso p.IsApplicazioneUnificata)

		If Not ulss.IsNullOrEmpty() Then
			Return ulss.First().Codice
		End If

		Return String.Empty

	End Function


	''' <summary>
	''' Restituisce il codice della usl e l'id applicativo in base all'app id specificato (che potrebbe essere sia vecchio o nuovo).
	''' </summary>
	''' <param name="idApplicativo"></param>
	''' <returns></returns>
	Public Function GetInfoAziendaByIdApplicativo(idApplicativo As String) As Entities.UslApplicativo

		Dim result As Entities.UslApplicativo = GenericProvider.Usl.GetInfoAziendaByIdApplicativo(idApplicativo)

		Return result

	End Function

	''' <summary>
	''' Restituisce true se la usl specificata è una di quelle già unificate
	''' </summary>
	''' <param name="codiceUsl"></param>
	''' <returns></returns>
	Public Function IsUnificata(codiceUsl As String) As Boolean

		Return GenericProvider.Usl.IsUnificata(codiceUsl)

	End Function

	Public Function IsAutenticatoRicercaLotti(codiceUsl As String, user As String, password As String) As Boolean

		Return GenericProvider.Usl.IsAutenticatoRicercaLotti(codiceUsl, user, password)

	End Function

#End Region

End Class
