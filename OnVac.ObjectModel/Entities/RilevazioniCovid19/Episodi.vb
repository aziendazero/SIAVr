Imports System.Collections.Generic

Namespace Entities


    Public Class EpisodioPaziente
        Public Property Testata As EpisodioTestata
        Public Property Dettaglio As EpisodioDettaglio
        Public Property Paziente As EpisodioDatiPaziente
        Public Property DatoreLavoro As DatoreLavoro
        Public Property Clinica As Clinica
        Public Property Ricoveri As List(Of Ricovero)
        Public Property Tamponi As List(Of Tampone)
        Public Property Diaria As List(Of Diaria)
        Public Property Contatti As List(Of Contatto)
        Public Property Tags As List(Of TagTmp)

        Public Sub New()
            Testata = New EpisodioTestata()
            Dettaglio = New EpisodioDettaglio()
            Paziente = New EpisodioDatiPaziente()
            DatoreLavoro = New DatoreLavoro()
            Clinica = New Clinica()
            Ricoveri = New List(Of Ricovero)
            Tamponi = New List(Of Tampone)
            Diaria = New List(Of Diaria)
            Contatti = New List(Of Contatto)
            Tags = New List(Of TagTmp)
        End Sub
    End Class

    <Serializable()>
    Public Class EpisodioTestata
        Public Property IdEpisodio As Long
        Public Property CodicePaziente As Long?
        Public Property CodiceTipoCaso As String
        Public Property CodiceTipoContatto As Integer?
        Public Property DescrizioneTipoContatto As String
        Public Property CodiceStato As Integer?
        Public Property DescrizioneStato As String
        Public Property IsOperatoreSanitario As Boolean
        Public Property HasPatologiaCroniche As Boolean
        Public Property DataSegnalazione As DateTime?
        Public Property IsAsintomatico As Boolean
        Public Property Note As String
        Public Property DataUltimaDiaria As DateTime?
        Public Property DescrizioneTipoCaso As String
        Public Property TelefonoRilevazione As String
        Public Property EmailRilevazione As String
        Public Property UtenteInserimentoDiaria As Long?
        Public Property NominativoUtenteInserimentoUltimaDiaria As String
        Public Property Attivo As Boolean
    End Class

    <Serializable()>
    Public Class EpisodioDettaglio
        Public Property CodiceRaccoltaOperatore As String
        Public Property TelefonoOperatore As String
        Public Property EmailOperatore As String
        Public Property CodiceRaccoltaUsl As String
        Public Property CodiceSegnalatore As Long?
        Public Property DescrizioneSegnalatore As String
        Public Property CodiceTipoOperatoreSanitario As String
        Public Property EsposizioneComune As String
        Public Property DataInizioIsolamento As DateTime?
        Public Property DataUltimoContatto As DateTime?
        Public Property DataFineIsolamento As DateTime?
        Public Property DataChiusura As DateTime?
        Public Property CodiceUtenteInserimento As Long
        Public Property DescrizioneUtenteInserimento As String
        Public Property UslInserimento As String
        Public Property DataInizioSintomi As DateTime?
        Public Property DescrizioneSintomi As String
        Public Property DataInserimento As DateTime?
        Public Property DataUltimoTampone As DateTime?
        Public Property EsitoUltimoTampone As String
        Public Property UtenteUltimaModifica As Long?
        Public Property DataUltimaModifica As DateTime?
        Public Property CodicePazienteOld As Long
        Public Property DescrizioneTipoOperatoreSanitario As String
        Public Property IndirizzoIsolamento As String
        Public Property TelefonoIsolamento As String
        Public Property EmailIsolamento As String
        Public Property ComuneCodiceIsolamento As String
        Public Property DescrizioneComuneIsolamento As String
        Public Property UtenteEliminazione As Long?
        Public Property UslEliminazione As String
        Public Property DataEliminazione As DateTime?
        Public Property otp As String
        ''' <summary>
        ''' Variabile bool è S o N o Null
        ''' </summary>
        ''' <returns></returns>
        Public Property InternoRegione As String
        Public Property CodiceComune As String
        Public Property SpsId As Long
        Public Property TpsId As Long
        Public Property DataDecessoCovid As Date?
        Public Property GuaritoClinicamente As Boolean
        Public Property DataGuaritoClinicamente As Date?
        Public Property Guarito As Boolean
        Public Property DataGuarigioneVirol As Date?
        Public Property CodiceVariante As Long?
        Public Property DescrizioneVariante As String
        Public Property CodiceMotivoGenotipizzazione As Long?
        Public Property DescrizioneMotivoGenotipizzazione As String
    End Class

    <Serializable()>
    Public Class EpisodioDatiPaziente
        Public Property Cognome As String
        Public Property Nome As String
        Public Property Cf As String
        Public Property DataNascita As DateTime?
        Public Property Sesso As String
        Public Property ComuneResidenza As String
        Public Property TelefonoUno As String
        Public Property TelefonoDue As String
        Public Property TelefonoTre As String
        Public Property CognomeMedicoBase As String
        Public Property NomeMedicoBase As String
        Public Property HasCredenzialiApp As Boolean
        Public Property HasConfermaInformativaCovid As Boolean
    End Class

    <Serializable()>
    Public Class FlatEpisodioPaziente
        Public Property IdEpisodio As Long
        <DbColumnName("CodPaziente")>
        Public Property CodicePaziente As Long?
        Public Property IdUltimaDiaria As Long?
        Public Property DataUltimaDiaria As DateTime?
        Public Property IdSintomoUltimaDiaria As Long
        Public Property DescrizioneSintomoUltimaDiaria As String
        Public Property DataDecessoCovid As DateTime?

        <DbColumnName("TipoCaso")>
        Public Property CodiceTipoCaso As String
        Public Property DescrizioneTipoCaso As String
        Public Property CodiceTipoContatto As Integer?
        Public Property DescrizioneTipoContatto As String
        Public Property IsOperatoreSanitario As Boolean
        Public Property HasPatologiaCroniche As Boolean
        Public Property DataSegnalazione As DateTime?
        Public Property IsAsintomatico As Boolean
        Public Property Note As String
        Public Property CodiceRaccoltaOperatore As String
        Public Property TelefonoOperatore As String
        Public Property EmailOperatore As String
        Public Property CodiceRaccoltaUsl As String
        Public Property CodiceSegnalatore As Long?
        Public Property DescrizioneSegnalatore As String
        Public Property CodiceStato As Integer?
        Public Property DescrizioneStato As String
        <DbColumnName("TipoOperatoreSanitario")>
        Public Property CodiceTipoOperatoreSanitario As String
        Public Property EsposizioneComune As String
        Public Property DataInizioIsolamento As DateTime?
        Public Property DataUltimoContatto As DateTime?
        Public Property DataFineIsolamento As DateTime?
        Public Property DataChiusura As DateTime?
        Public Property IndirizzoIsolamento As String
        Public Property TelefonoIsolamento As String
        Public Property EmailIsolamento As String
        Public Property ComuneCodiceIsolamento As String
        Public Property DescrizioneComuneIsolamento As String
        Public Property DataInizioSintomi As DateTime?
        Public Property DataUltimoTampone As DateTime?
        Public Property EsitoUltimoTampone As String
        Public Property CodiceUtenteInserimento As Long
        Public Property DescrizioneUtenteInserimento As String
        Public Property UslInserimento As String
        Public Property DataInserimento As DateTime?
        Public Property UtenteUltimaModifica As Long?
        Public Property DataUltimaModifica As DateTime?
        Public Property Cognome As String
        Public Property Nome As String

        <DbColumnName("CodiceFiscale")>
        Public Property Cf As String
        Public Property DataNascita As DateTime?
        Public Property Sesso As String
        Public Property CognomeMedicoBase As String
        Public Property NomeMedicoBase As String
        Public Property Attivo As Boolean
        <DbColumnName("UltimoUtenteInserimentoDiaria")>
        Public Property CodiceUtenteDiaria As Long?
        <DbColumnName("NomeUteUltimaDiaria")>
        Public Property NominativoUtenteInserimentoUltimaDiaria As String

        Public Property DataGuaritoClinicamento As DateTime?
        Public Property GuaritoClinicamente As Boolean
        Public Property Otp As String

        Public Property CodiceTag As Long?
        Public Property DescrizioneTag As String
        Public Property CodiceSintomo As Long?
        Public Property DescrizioneSintomo As String
        Public Property CodiceVariante As Long?
        Public Property DescrizioneVariante As String
    End Class

End Namespace
