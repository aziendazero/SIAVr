Imports System.Collections.Generic

Namespace Entities

    Public Class InfoEpisodioCovid

        Public Sub New()
            Me.Paziente = New EpisodioDatiPaziente()
            Me.Tags = New List(Of Tag)
            Me.Sintomi = New Dictionary(Of Long, String)()
        End Sub
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
        Public Property CodiceVariante As Long?
        Public Property DescrizioneVariante As String
        Public Property Paziente As EpisodioDatiPaziente

        Public Property Tags As IEnumerable(Of Tag)
        Public Property Sintomi As Dictionary(Of Long, String)
    End Class
End Namespace