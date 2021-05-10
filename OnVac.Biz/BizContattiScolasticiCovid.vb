Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities

Public Class BizContattiScolasticiCovid
    Inherits BizClass

    Public Sub New(genericProvider As DbGenericProvider, settings As Settings.Settings, contextInfo As BizContextInfos)
        MyBase.New(genericProvider, settings, contextInfo, Nothing)
    End Sub

    Public Function CercaInfoContattiScolastici(filtri As FiltriContattiScolastici) As IEnumerable(Of InfoContattoScolastico)
        Return GenericProvider.ContattiScolasticiCovid.CercaInfoContattiScolastici(filtri)
    End Function

    Public Function CercaScuole(q As String) As IEnumerable(Of InfoScuola)
        Return GenericProvider.ContattiScolasticiCovid.CercaScuole(q)
    End Function

    Public Function GetScuola(codiceMeccanografico As String) As InfoScuola
        Return GenericProvider.ContattiScolasticiCovid.GetScuola(codiceMeccanografico)
    End Function

    Public Function GetInformazioniTestata(codiceEpisodioIndice As Long) As InfoAggiuntiveTestataImportazione
        Dim dataPositivita As Date? = GenericProvider.ContattiScolasticiCovid.DataPositivizzazione(codiceEpisodioIndice)

        Return New InfoAggiuntiveTestataImportazione With {
            .DataPositivizzazione = dataPositivita
        }
    End Function

    Public Function GetDettaglioImportazione(codiceGruppo As String) As DettaglioImportazione
        Return GenericProvider.ContattiScolasticiCovid.GetDettaglioImportazione(codiceGruppo)
    End Function

    Public Function SalvaDettaglioImportazione(cmd As SalvaDettaglioImportazioneCMD) As DettaglioImportazione
        GenericProvider.ContattiScolasticiCovid.SalvaDettaglioImportazione(cmd)
        Return GetDettaglioImportazione(cmd.CodiceGruppo)
    End Function

    Public Function InformazioniContattiEpisodio(codiceImportazione As String) As IEnumerable(Of InformazioneContattoEsportazione)
        Return GenericProvider.ContattiScolasticiCovid.InformazioniContattiEpisodio(codiceImportazione)
    End Function

    Public Sub ModificaClasseElaborazione(codiceElaborazione As Long, classe As String)
        GenericProvider.ContattiScolasticiCovid.ModificaClasseElaborazione(codiceElaborazione, classe)
    End Sub
End Class
