Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL


Public Class BizComuni
    Inherits BizClass

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)

        MyBase.New(genericprovider, settings, contextInfos, logOptions)

    End Sub

#End Region

#Region " Public "

    ''' <summary>
    ''' Restituisce solo i comuni validi (con data scadenza non valorizzata o futura)
    ''' </summary>
    ''' <param name="ricerca"></param>
    ''' <returns></returns>
    Public Function GetListComuni(ricerca As String) As List(Of Entities.Comune)
        Return GenericProvider.Comuni.GetListComuni(ricerca)
    End Function

    ''' <summary>
    ''' In base al codice del comune, restituisce True se fa parte della provincia specificata nel parametro ISTAT_PROVINCIA, False altrimenti.
    ''' </summary>
    ''' <param name="codiceComune"></param>
    ''' <returns></returns>
    Public Function IsInProvincia(codiceComune As String) As Boolean

        Dim codiceIstat As String = GenericProvider.Comuni.GetIstatByCodice(codiceComune)

        Return (codiceIstat.StartsWith(Settings.ISTAT_PROVINCIA))

    End Function
    ''' <summary>
    ''' Metodo che filtra le regioni esterne al codice regionale inserito e restituisce la lista di tali comuni
    ''' </summary>
    ''' <param name="ricerca"></param>
    ''' <param name="codRegione"></param>
    ''' <returns></returns>
    Public Function GetListComuniEsterniRegione(ricerca As String, codRegione As String) As List(Of Entities.Comune)
        Return GenericProvider.Comuni.GetListComuniEsterniRegione(ricerca, codRegione)
    End Function
    ''' <summary>
    ''' Restituisce il cap in base al codice del comune.
    ''' </summary>
    ''' <param name="codiceComune"></param>
    ''' <returns></returns>
    Public Function GetCapByCodiceComune(codiceComune As String) As String

        Return GenericProvider.Comuni.GetCapByCodiceComune(codiceComune)

    End Function

    ''' <summary>
    ''' Restituisce True se il comune è valido nella data specificata, False altrimenti.
    ''' Se la data non è specificata, esegue il controllo alla data odierna.
    ''' </summary>
    ''' <param name="codiceComune"></param>
    ''' <param name="dataValidita"></param>
    ''' <returns></returns>
    Public Function CheckValiditaComune(codiceComune As String, dataValidita As Date) As Boolean

        If dataValidita = Date.MinValue Then dataValidita = Date.Now.Date

        Return GenericProvider.Comuni.CheckValiditaComune(codiceComune, dataValidita)

    End Function

    ''' <summary>
    ''' Restituisce un oggetto della classe Comune contenente tutti i dati del comune specificato.
    ''' </summary>
    ''' <param name="codiceComune"></param>
    ''' <returns></returns>
    Public Function GetComuneByCodice(codiceComune As String) As Entities.Comune

        Return GenericProvider.Comuni.GetComuneByCodice(codiceComune)

    End Function
    ''' <summary>
    ''' Restituisce tutti i comuni che non sono scaduti, solitmanete si utilizza questa query quando si ha un accesso troppo repentino alla tabella. Esempio un service che accede 1000 volte alla tabella  in 1 minper fare dei controlli, in questo caso si preferisce un unico accesso e si controllano i campi in pancia.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetComuni() As List(Of Entities.Comune)
        Return GenericProvider.Comuni.GetComuni()
    End Function

    ''' <summary>
    ''' Restituisce True se il comune specificato appartiene alla regione definita nel parametro.
    ''' Restituisce False se il codice del comune è null o string.empty
    ''' </summary>
    ''' <param name="codiceComune">Codice interno del comune in anagrafe locale</param>
    ''' <returns></returns>
    Public Function IsComuneInRegione(codiceComune As String) As Boolean

        If String.IsNullOrWhiteSpace(codiceComune) Then
            Return False
        End If

        Dim codiceRegione As String = GenericProvider.Comuni.GetCodiceRegione(codiceComune)

        Return (Settings.CODICE_REGIONE = codiceRegione)

    End Function

#End Region

End Class
