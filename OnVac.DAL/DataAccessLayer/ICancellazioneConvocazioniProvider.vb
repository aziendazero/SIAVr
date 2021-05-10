Imports Onit.OnAssistnet.Data
Imports System.Collections.Generic

Public Interface ICancellazioneConvocazioniProvider

    Function GetConvocazioniPerUtilityCancellazione(param As ParametriGetCnvPerCancellazione) As List(Of Entities.ConvocazioneDaCancellare)
    Function CountConvocazioniPerUtilityCancellazione(param As FiltriConvocazioneDaCancellare) As Integer
    Function GetIdConvocazioniPerUtilityCancellazione(param As FiltriConvocazioneDaCancellare) As List(Of Entities.ConvocazionePK)

    Function GetConvocazioniDaEliminareByJobId(jobId As Long) As List(Of Entities.ConvocazionePK)
    Function FiltraConvocazioniDaEliminare(param As ParametriFiltraCnvDaEliminare) As List(Of Entities.ConvocazionePK)
    Function EliminaVaccinazioniProgrammate(codicePaziente As Long, dataConvocazione As DateTime, codiceVac As String) As Integer
    Function GetVaccinazioniProgrammateDaEliminare(param As ParametriVacProgDaEliminare) As List(Of Entities.VaccinazioneProgrammata)
    Function EliminaCicliEmpty(codicePaziente As Long, dataConvocazione As DateTime, codiceCiclo As String, numSeduta As Integer?) As Integer

    Function GetElaborazioniJobCorrente(jobId As Long, listCodiciPazientiDateCnv As List(Of Entities.ConvocazionePK)) As List(Of JobItem)

End Interface

Public Class ParametriGetCnvPerCancellazione

    Public Filtri As FiltriConvocazioneDaCancellare
    Public OrderBy As String
    Public PagingOpts As PagingOptions

End Class

Public Class ParametriFiltraCnvDaEliminare

    Public CnvKeys As List(Of Entities.ConvocazionePK)
    Public EscludiCnvConSolleciti As Boolean
    Public EscludiCnvConAppuntamenti As Boolean

End Class

Public Class ParametriVacProgDaEliminare

    Public CnvKeys As List(Of Entities.ConvocazionePK)
    Public FiltriProgrammazione As List(Of Entities.FiltroProgrammazione)
    Public TipoFiltri As Entities.TipoFiltriProgrammazione?

End Class

Public Class JobItem

    Public Progressivo As Long
    Public CodicePaziente As String
    Public DataConvocazione As DateTime

End Class