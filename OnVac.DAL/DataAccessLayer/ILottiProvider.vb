Imports System.Collections.Generic

Imports Onit.OnAssistnet.Data


Public Interface ILottiProvider

    Function LoadLottiUtilizzabili(codiceConsultorio As String, codiceConsultorioMagazzino As String, gestioneTipoConsultorio As Boolean, sessoPaziente As String, etaPaziente As Double, dataScadenzaLotto As Date, includiLottiFuoriEta As Boolean, soloLottiAttivi As Boolean, filtraEtaAttivazione As Boolean, includiImportiDefault As Boolean) As DataTable

	Function LoadLottiMagazzinoCentrale(filtriRicerca As Filters.FiltriRicercaLottiMagazzino, listDatiOrdinamento As List(Of Entities.DatiOrdinamento), pagingOptions As PagingOptions, userId As Integer) As List(Of Entities.LottoMagazzino)

    Function LoadLottoMagazzinoCentrale(codiceLotto As String, dUtente As Integer, codDistr As String) As Entities.LottoMagazzino

    Function LoadDettaglioDosiLotto(codiceLotto As String, noScortaNulla As Boolean, userId As Integer, codDistr As String) As List(Of Entities.LottoDettaglioMagazzino)

    Function LoadLottiMagazzino(codiceConsultorio As String, codiceConsultorioMagazzino As String, codiceLotto As String, filtriRicerca As Filters.FiltriRicercaLottiMagazzino) As List(Of Entities.LottoMagazzino)

	Function CountLottiMagazzinoCentrale(ByVal filtriRicerca As Filters.FiltriRicercaLottiMagazzino, idUtente As Integer) As Integer

	Function LoadMovimentiLotto(codiceLotto As String, codiceConsultorioMagazzino As String, startRecordIndex As Integer, endRecordIndex As Integer) As List(Of Entities.MovimentoLotto)

    Function CountMovimentiLotto(codiceLotto As String, codiceConsultorioMagazzino As String) As Integer

    Function GetDosiRimaste(codiceLotto As String, codiceConsultorioMagazzino As String) As Integer

    Function GetDosiRimaste(codiciLotti As List(Of String), codiceConsultorioMagazzino As String) As List(Of KeyValuePair(Of String, Integer))

    Function GetQuantitaMinima(codiceLotto As String, codiceConsultorioMagazzino As String) As Integer

    Function CountLottiConsultorio(codiceConsultorio As String) As Integer

    Function ExistsCodiceLotto(codiceLotto As String) As Boolean

    Function ExistsLottoConsultorio(codiceLotto As String, codiceConsultorio As String) As Boolean

    Function IsLottoInConsultorio(codiceLotto As String, codiceConsultorio As String) As Boolean

    Function IsLottoScaduto(codiceLotto As String) As Boolean

    Function IsActiveAltroLottoStessoNomeCommerciale(codiceLotto As String, codiceNomeCommerciale As String, codiceConsultorio As String) As Boolean

    Function InsertLotto(lottoMagazzino As Entities.LottoMagazzino) As Integer

    Function InsertLottoMagazzinoCentrale(lottoMagazzino As Entities.LottoMagazzino) As Integer

    Function InsertLottoConsultorio(lottoMagazzino As Entities.LottoMagazzino) As Integer

    Function UpdateLottoMagazzinoCentrale(lottoMagazzino As Entities.LottoMagazzino) As Integer

    Function UpdateLotto(lottoMagazzino As Entities.LottoMagazzino) As Integer

    Function UpdateQuantitaMinimaLottoConsultorio(codiceLotto As String, codiceConsultorio As String, quantitaMinima As Integer) As Integer

    Function UpdateDosiRimasteLottoConsultorio(codiceLotto As String, codiceConsultorioMagazzino As String, numeroDosi As Integer) As Integer

    Function UpdateLottoAttivo(codiceLotto As String, codiceConsultorio As String, attivo As Boolean, idUtenteModificaFlagAttivo As Integer?, dataModificaFlagAttivo As DateTime?, etaMinimaAttivazione As Integer?, etaMassimaAttivazione As Integer?, updateEta As Boolean) As Integer

    Function InsertMovimento(movimentoLotto As Entities.MovimentoLotto) As Integer

    Sub DisattivaLotto(codiceLotto As String, idUtenteModificaFlagAttivo As Integer?, dataModificaFlagAttivo As DateTime?)

    Function GetLottiAssociazioni(listCodiciLotti As List(Of String)) As List(Of LottiAssociazioniResult)

    Function GetLottiByAIC(codiceAIC As String, codiceUlss As String) As List(Of LottoRicercaWebService)

    Function GetLottiByCodVacc(codicePrincipioVaccinale As String, codiceUlss As String) As List(Of LottoRicercaWebService)
    Function GetCicloNomeCommercialeByLotto(codiceLotto As String) As String


    Class LottiAssociazioniResult
        Public CodiceLotto As String
        Public CodiceAssociazione As String
        Public DescrizioneAssociazione As String
    End Class

    Function GetEtaAttivazioneLotto(codiceLotto As String, codiceConsultorioLotto As String) As EtaAttivazioneLottoResult

    Class EtaAttivazioneLottoResult
        Public EtaMinima As Integer?
        Public EtaMassima As Integer?
    End Class

    Function GetLotti(idRSA As String, escludiLottiGiacenzaZero As Boolean) As List(Of Lotto)

    Function GetLottoAnagrafe(codiceLotto As String) As LottoAnagrafe

End Interface