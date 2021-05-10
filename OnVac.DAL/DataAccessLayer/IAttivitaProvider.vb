Imports System.Collections.Generic
Imports Onit.OnAssistnet.Data

Public Interface IAttivitaProvider

#Region " Anagrafe Tipi Attività "

    Function GetElencoAttivitaTipo(filtroGenerico As String, soloValidi As Boolean, campoOrdinamento As Entities.AttivitaTipo.Ordinamento?, versoOrdinamento As String, pagingOptions As Onit.OnAssistnet.Data.PagingOptions) As IEnumerable(Of Entities.AttivitaTipo)
    Function CountElencoAttivitaTipo(filtroGenerico As String, soloValidi As Boolean) As Integer
    Function GetAttivitaTipo(codiceAttivitaTipo As String) As Entities.AttivitaTipo
    Function InsertAttivitaTipo(attivitaTipo As Entities.AttivitaTipo) As Integer
    Function UpdateAttivitaTipo(attivitaTipo As Entities.AttivitaTipo) As Integer

#End Region

#Region " Anagrafe Attività "

    Function GetElencoAttivitaAnagrafe(filtroGenerico As String, soloValidi As Boolean, campoOrdinamento As Entities.AttivitaAnagrafe.Ordinamento?, versoOrdinamento As String, pagingOptions As Onit.OnAssistnet.Data.PagingOptions, idUtenteConnesso As Integer, appId As String) As IEnumerable(Of Entities.AttivitaAnagrafe)
    Function CountElencoAttivitaAnagrafe(filtroGenerico As String, soloValidi As Boolean, idUtenteConnesso As Integer, appId As String) As Integer
    Function GetAttivitaAnagrafe(id As Integer) As Entities.AttivitaAnagrafe
    Function GetAttivitaAnagrafeByCodice(codiceAttivita As String) As IEnumerable(Of Entities.AttivitaAnagrafe)
    Function GetNextIdAttivitaAnagrafe() As Integer
    Function InsertAttivitaAnagrafe(attivitaAnagrafe As Entities.AttivitaAnagrafe) As Integer
    Function UpdateAttivitaAnagrafe(attivitaAnagrafe As Entities.AttivitaAnagrafe) As Integer

#End Region

#Region " Anagrafe Variabili "

    Function GetElencoVariabiliDaAssociare(idAttivita As Integer?, filtroGenerico As String, soloValidi As Boolean, campoOrdinamento As Entities.AttivitaVariabileDaAssociare.Ordinamento?, versoOrdinamento As String, pagingOptions As Onit.OnAssistnet.Data.PagingOptions) As IEnumerable(Of Entities.AttivitaVariabileDaAssociare)
    Function CountElencoVariabiliDaAssociare(idAttivita As Integer?, filtroGenerico As String, soloValidi As Boolean) As Integer
    Function GetElencoVariabiliAssociate(idAttivita As Integer, filtroGenerico As String, campoOrdinamento As Entities.AttivitaVariabileAssociata.Ordinamento?, versoOrdinamento As String, pagingOptions As Onit.OnAssistnet.Data.PagingOptions) As IEnumerable(Of Entities.AttivitaVariabileAssociata)
    Function CountElencoVariabiliAssociate(idAttivita As Integer, filtroGenerico As String) As Integer

    Function GetMaxOrdineVariabiliAssociate(idAttivita As Integer) As Integer

    Function GetNextIdVariabileAssociata() As Integer
    Function InsertVariabileAssociata(variabileAssociata As Entities.AttivitaVariabileAssociata) As Integer
    Function UpdateOrdineVariabileAssociata(idAttivitaVariabile As Integer, ordine As Integer) As Integer
    Function DeleteVariabileAssociata(idAttivitaVariabile As Integer) As Integer

#End Region

#Region " Registrazione Attività "

    Function GetElencoAttivitaRegistrazione(filtroGenerico As String, filtroSoloValidi As Boolean, appId As String, dataEsecuzioneInizio As DateTime?, dataEsecuzioneFine As DateTime?, dataRegistrazioneInizio As DateTime?, dataRegistrazioneFine As DateTime?, idUtenteRegistrazione As Long?, campoOrdinamento As AttivitaRegistrazione.Ordinamento?, versoOrdinamento As String, pagingOptions As Onit.OnAssistnet.Data.PagingOptions, idUtenteconnesso As Integer, codiceUsl As String) As IEnumerable(Of AttivitaRegistrazione)
    Function CountElencoAttivitaRegistrazione(filtroGenerico As String, filtroSoloValidi As Boolean, dataEsecuzioneInizio As DateTime?, dataEsecuzioneFine As DateTime?, dataRegistrazioneInizio As DateTime?, dataRegistrazioneFine As DateTime?, idUtenteRegistrazione As Long?, codiceUsl As String) As Integer
    Function GetAttivitaRegistrazione(idAttivitaRegistrazione As Integer, appId As String, idUtenteConnesso As Integer) As AttivitaRegistrazione

    Function GetNextIdAttivitaRegistrazione() As Integer
    Function InsertAttivitaRegistrazione(attivitaRegistrazione As AttivitaRegistrazione, codiceUsl As String) As Integer
    Function UpdateAttivitaRegistrazione(attivitaRegistrazione As AttivitaRegistrazione) As Integer

    Function GetRispostePossibiliAttivita(idAttivitaAnagrafe As Integer) As IEnumerable(Of AttivitaRegistrazioneRisposta)
    Function GetValoriAttivitaRegistrazione(idAttivitaRegistrazione As Integer) As IEnumerable(Of AttivitaRegistrazioneValore)

    Function GetAttivitaRegistrazioneValore(idAttivitaRegistrazioneValore As Long) As AttivitaRegistrazioneValore
    Function GetNextIdAttivitaRegistrazioneValore() As Long
    Function InsertAttivitaRegistrazioneValore(valore As AttivitaRegistrazioneValore) As Integer
    Function UpdateAttivitaRegistrazioneValore(valore As AttivitaRegistrazioneValore) As Integer

    Function GetOperatoriAttivitaRegistrazione(idAttivitaRegistrazione As Integer) As IEnumerable(Of AttivitaRegistrazioneOperatore)
    Function InsertOperatoreAttivitaRegistrazione(idAttivitaRegistrazione As Integer, idOperatore As Integer) As Integer
    Function DeleteOperatoreAttivitaRegistrazione(idAttivitaRegistrazione As Integer, idOperatore As Integer) As Integer

    Function GetOperatoriPPA(idAttivitaRegistrazione As Integer?, idOperatoriDaEscludere As IEnumerable(Of Integer), filtroGenerico As String, filtroSoloValidi As Boolean, campoOrdinamento As Entities.AttivitaVariabileDaAssociare.Ordinamento?, versoOrdinamento As String, pagingOptions As Onit.OnAssistnet.Data.PagingOptions) As IEnumerable(Of Entities.AttivitaOperatorePPA)
    Function CountOperatoriPPA(idAttivitaRegistrazione As Integer?, idOperatoriDaEscludere As IEnumerable(Of Integer), filtroGenerico As String, filtroSoloValidi As Boolean) As Integer
    Function GetOperatorePPA(idOperatore As Integer) As Entities.AttivitaOperatorePPA
    Function GetOperatoriPPA(cognome As String, nome As String, idUnitaOperativa As Integer?, idQualifica As Integer?, filtroSoloValidi As Boolean) As IEnumerable(Of Entities.AttivitaOperatorePPA)
    Function GetNextIdAttivitaOperatorePPA() As Integer
    Function InsertOperatorePPA(operatorePPA As AttivitaOperatorePPA) As Integer

    Function GetQualifiche(filtroGenerico As String, filtroSoloValidi As Boolean, campoOrdinamento As Entities.Qualifica.Ordinamento?, versoOrdinamento As String, pagingOptions As Onit.OnAssistnet.Data.PagingOptions) As IEnumerable(Of Entities.Qualifica)
    Function CountQualifiche(filtroGenerico As String, filtroSoloValidi As Boolean) As Integer
    Function GetUnitaOperative(filtroGenerico As String, filtroSoloValidi As Boolean, campoOrdinamento As Entities.UnitaOperativa.Ordinamento?, versoOrdinamento As String, pagingOptions As Onit.OnAssistnet.Data.PagingOptions) As IEnumerable(Of Entities.UnitaOperativa)
    Function CountUnitaOperative(filtroGenerico As String, filtroSoloValidi As Boolean) As Integer
    Function GetElencoUtentiAssociati(idAttivita As Integer, filtroGenerico As String, campoOrdinamento As AttivitaUtenteAssociato.Ordinamento?, versoOrdinamento As String, pagingOptions As PagingOptions) As IEnumerable(Of AttivitaUtenteAssociato)
    Function GetElencoUtentiDaAssociare(idAttivita As Integer?, filtroGenerico As String, soloValidi As Boolean, campoOrdinamento As AttivitaUtentiDaAssociare.Ordinamento?, versoOrdinamento As String, pagingOptions As PagingOptions) As IEnumerable(Of AttivitaUtentiDaAssociare)
    Function GetNextIdUtenteAssociato() As Integer
    Function InsertUtenteAssociato(variabileAssociata As AttivitaUtenteAssociato) As Integer
    Function DeleteUtenteAssociato(id As Integer) As Integer

#End Region

#Region " Scuole "

    Function GetElencoScuole(filtroGenerico As String, soloValidi As Boolean, campoOrdinamento As Entities.Scuola.Ordinamento?, versoOrdinamento As String, pagingOptions As Onit.OnAssistnet.Data.PagingOptions) As IEnumerable(Of Entities.Scuola)
    Function CountElencoScuole(filtroGenerico As String, soloValidi As Boolean) As Integer
    Function GetScuola(codiceScuola As String) As Entities.Scuola
    Function UpdateObbligatorioVariabileAssociata(idAttivitaVariabile As Integer, obbligatorio As String) As Integer

#End Region

End Interface
