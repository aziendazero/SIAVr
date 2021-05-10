Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL

Public Class BizAnagrafiche
    Inherits BizClass

#Region " Constructors "

    Public Sub New(genericProvider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos)

        MyBase.New(genericProvider, settings, contextInfos, Nothing)

    End Sub

#End Region
#Region "Types"
    Public Enum TipoAnagrafica
        Farmaco
        IndicazioniFarmaco
    End Enum
#End Region

#Region " Public "

#Region " Anagrafica Origini etniche "



    ''' <summary>
    ''' Restituisce le anagrafiche delle origine etniche
    ''' </summary>
    ''' <param name="codiceOrigineEtnica"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetTableAnagraficaOrigineEtniche(codiceOrigineEtnica As String) As DataTable

        Return Me.GenericProvider.Anagrafiche.GetTableOrigineEtnica(codiceOrigineEtnica)

    End Function

	Public Function GetListAnagraficaOrigineEtniche(codiceOrigineEtnica As String) As List(Of Entities.OrigineEtnica)

		Return Me.GenericProvider.Anagrafiche.GetListaOrigineEtnica(codiceOrigineEtnica)

	End Function

    Public Function GetListAnagraficaDistretti(codiceDistretto As String, codiceCoune As String, codiceUsl As String) As List(Of Entities.Distretto)

        Return GenericProvider.Distretto.GetListaDistretto(codiceDistretto, codiceCoune, codiceUsl)

    End Function
    Public Function GetListAnagraficaDistrettiCNSUtente(idUtente As Long) As List(Of Entities.DistrettoDDL)

        Return GenericProvider.Distretto.GetListaDistrettoCNSUtente(idUtente)

    End Function

    Public Function GetListAnagrafica(codice As String, tipo As TipoAnagrafica, filtro As String) As List(Of Entities.AnagrafeCodiceDescrizione)
        Dim lista As New List(Of Entities.AnagrafeCodiceDescrizione)
        If tipo = TipoAnagrafica.Farmaco Then
            Dim dt As DataTable = Nothing
            dt = GenericProvider.NomiCommerciali.GetNomeCommerciale(codice, filtro)
            For Each lis As DataRow In dt.Rows
                Dim ll As New Entities.AnagrafeCodiceDescrizione
                ll.Codice = lis("NOC_CODICE")
                ll.Descrizione = lis("NOC_DESCRIZIONE")
                ll.Obsoleto = lis("NOC_OBSOLETO")
                If Not lis("NOC_CODICE_ESTERNO").ToString().IsNullOrEmpty Then
                    ll.CodiceEsterno = lis("NOC_CODICE_ESTERNO")
                Else
                    ll.CodiceEsterno = String.Empty
                End If

                lista.Add(ll)
            Next
        End If
        If tipo = TipoAnagrafica.IndicazioniFarmaco Then

            lista = GenericProvider.Anagrafiche.GetListaNomiCommercialiIndicazioni(codice, filtro)


        End If

        Return lista

    End Function

#Region "cov19"
    Public Function GetListSintomi(ByVal Filtro As String) As List(Of Entities.Sintomi)
        Return GenericProvider.Anagrafiche.GetListSintomi(Filtro)
    End Function
    Public Function GetListSegnalatore(Filtro As String) As List(Of Entities.Segnalatore)
        Return GenericProvider.Anagrafiche.GetListSegnalatore(Filtro)
    End Function
    Public Function GetStatiEppisodiCov(Filtro As String) As List(Of Entities.StatoEppisodio)
        Return Me.GetStatiEppisodiCov(Filtro, True)
    End Function
    Public Function GetStatiEppisodiCov(Filtro As String, soloValidi As Boolean) As List(Of Entities.StatoEppisodio)
        Return GenericProvider.Anagrafiche.GetStatoCov(Filtro, soloValidi)
    End Function

    Public Function GetListTipoContatto(Filtro As String) As IEnumerable(Of Entities.TipoContatto)
        Return GenericProvider.Anagrafiche.GetListTipoContatto(Filtro)
    End Function

    Public Function GetListTipoAzienda(Filtro As String) As IEnumerable(Of Entities.TipoAzienda)
        Return GenericProvider.Anagrafiche.GetListTipoAzienda(Filtro)
    End Function

    Public Function CercaReparti(Filtro As String) As IEnumerable(Of Entities.Reparto)
        Return GenericProvider.Anagrafiche.GetReparti(Filtro)
    End Function

    Public Function GetReparto(codice As Integer) As Entities.Reparto
        Return GenericProvider.Anagrafiche.GetReparto(codice)
    End Function

    Public Function GetTipologieTamponi() As IEnumerable(Of Entities.TipologiaTampone)
        Return GenericProvider.Anagrafiche.GetTipologieTamponi()
    End Function

    Public Function ElencoVariantiCovid(filtro As String) As IEnumerable(Of Entities.VarianteCovid)
        Return GenericProvider.Anagrafiche.ElencoVariantiCovid(filtro)
    End Function

    Public Function GetVarianteCovid(codice As Long) As Entities.VarianteCovid
        Return GenericProvider.Anagrafiche.GetVarianteCovid(codice)
    End Function

    Public Function ElencoMotivazioniVariante(filtro As String) As IEnumerable(Of Entities.MotivazioneVariante)
        Return GenericProvider.Anagrafiche.ElencoMotivazioniVariante(filtro)
    End Function
#End Region

#End Region

#End Region

#Region " Private "



#End Region

End Class
