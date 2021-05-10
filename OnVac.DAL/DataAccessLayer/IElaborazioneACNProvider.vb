Imports System.Collections.Generic
Imports Onit.OnAssistnet.Data
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.Enumerators

Public Interface IElaborazioneACNProvider

	Function GetElaborazioniVaccinazioneACNByIDProcesso(idProcessoElaborazione As Long, numeroElaborazioni As Integer) As List(Of Entities.ElaborazioneACN)
	Function GetElaborazioniVaccinazioneACN(filter As FiltroElaborazioneACN, pagingOptions As PagingOptions) As List(Of ElaborazioneACN)
	Function CountElaborazioniVaccinazioneACN(filter As FiltroElaborazioneACN) As Long
	Sub UpdateElaborazioneVaccinazioneAcn(elaborazioneVaccinazionePaziente As ElaborazioneACN)
	Function UpdateElaborazioniVaccinazioneAcn(ByVal idProcessoElaborazione As Long, ByVal statoElaborazione As Enumerators.StatoAcquisizioneElaborazioneVaccinazioneAcn, ByVal statoElaborazionePrecedente As Enumerators.StatoAcquisizioneElaborazioneVaccinazioneAcn) As Long
End Interface