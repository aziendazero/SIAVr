Imports System.Collections.Generic
Imports Onit.OnAssistnet.Data

Public Interface IFirmaDigitaleProvider

    Function GetDocumentoByVisita(idVisita As Integer, codiceAzienda As String) As Entities.ArchiviazioneDIRV.DocumentoFirma
    Function GetIdVisitaByIdDocumento(idDocumento As Long) As Long
    Function GetDocumentoById(idDocumento As Long) As String
    Function GetNomeFileByIdDocumento(idDocumento As Long) As String
    Function GetTokenArchiviazioneDocumento(idDocumento As Long) As String
    Function GetListIdDocumentiDaArchiviare(codiceAzienda As String) As List(Of Long)
    Function GetIdUtenteFirma(idDocumento As Long) As Long?

    Function GetNewIdDocumento() As Long
    Function InsertDocumento(firmaDigitaleInfo As Entities.ArchiviazioneDIRV.FirmaDigitaleInfo, numeroTentativiArchiviazione As Integer) As Integer

    Function UpdateDatiFirmaDocumento(firmaDigitaleInfo As Entities.ArchiviazioneDIRV.FirmaDigitaleInfo) As Integer
    Function UpdateDatiArchiviazioneDocumento(firmaDigitaleInfo As Entities.ArchiviazioneDIRV.FirmaDigitaleInfo, incrementaTentativi As Boolean, deleteDocumentoFirmato As Boolean) As Integer

    Function DeleteDocumento(idDocumento As Long) As Integer
    Function DeleteDocumentiVisite(listIdVisita As List(Of Long)) As Integer

    Function GetDocumentiVisite(param As ParametriGetDocumentiVisite) As List(Of ArchiviazioneDIRV.DocumentoVisita)
    Function CountDocumentiVisite(filtri As ArchiviazioneDIRV.FiltriDocumentiVisite) As Integer
    Function GetIdVisiteDocumenti(filtri As ArchiviazioneDIRV.FiltriDocumentiVisite) As List(Of Long)

End Interface


Public Class ParametriGetDocumentiVisite

    Public Filtri As ArchiviazioneDIRV.FiltriDocumentiVisite
    Public OrderBy As String
    Public PagingOpts As PagingOptions

End Class