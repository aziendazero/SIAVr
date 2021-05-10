Imports System.Collections.Generic

Public Interface IContattiScolasticiCovidProvider
    Function CercaInfoContattiScolastici(filtri As FiltriContattiScolastici) As IEnumerable(Of InfoContattoScolastico)
    Function CercaScuole(q As String) As IEnumerable(Of InfoScuola)
    Function GetScuola(codiceMeccanografico As String) As InfoScuola
    Function DataPositivizzazione(codiceEpisodioIndice As Long) As Date?
    Function GetDettaglioImportazione(codiceGruppo As String) As DettaglioImportazione
    Sub SalvaDettaglioImportazione(command As SalvaDettaglioImportazioneCMD)
    Function InformazioniContattiEpisodio(codiceImportazione As String) As List(Of InformazioneContattoEsportazione)
    Sub ModificaClasseElaborazione(codiceElaborazione As Long, classe As String)
End Interface
