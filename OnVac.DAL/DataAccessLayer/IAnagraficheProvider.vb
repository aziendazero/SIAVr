Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.Entities

Public Interface IAnagraficheProvider

    Function GetListaOrigineEtnica(codice As String) As List(Of OrigineEtnica)
    Function GetTableOrigineEtnica(codice As String) As DataTable
    Function GetListaNomiCommercialiIndicazioni(codice As String, filter As String) As List(Of AnagrafeCodiceDescrizione)

    Function GetListSintomi(ByVal Filtro As String) As List(Of Sintomi)
    Function GetListSegnalatore(Filtro As String) As List(Of Segnalatore)
    Function GetStatoCov(Filtro As String, soloValidi As Boolean) As List(Of StatoEppisodio)
    Function GetListTipoAzienda(filtro As String) As List(Of TipoAzienda)
    Function GetListTipoContatto(Filtro As String) As List(Of TipoContatto)
    Function GetReparto(codice As Integer) As Reparto
    Function GetReparti(Filtro As String) As List(Of Reparto)
    Function GetRSA(ULSS As String, Filtro As String) As List(Of RSA)
    Function GetCodiceConsultorioMagazzinoRSA(idRSA As String) As String
    Function GetRSAByIdGruppo(idGruppo As String) As List(Of RSA)
    Function GetTipologieTamponi() As IEnumerable(Of TipologiaTampone)
    Function ElencoVariantiCovid(filtro As String) As IEnumerable(Of VarianteCovid)
    Function GetVarianteCovid(codice As Long) As VarianteCovid
    Function ElencoMotivazioniVariante(filtro As String) As IEnumerable(Of MotivazioneVariante)
    Function GetMotivazioneVariante(codice As Long) As MotivazioneVariante
End Interface