Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.Entities

Public Interface ITipiErogatoriProvider

    Function GetTipiErogatori() As List(Of TipoErogatoreVacc)

    Function GetDettaglioTipoErogatore(id As Integer) As List(Of TipoErogatoreVacc)
    Function GetDettaglioTipoErogatoreFromCodice(codice As String) As List(Of TipoErogatoreVacc)

    Function UpdateTipoErogatore(item As TipoErogatoreVaccCommand) As Integer

    Function DeleteTipoErogatore(id As Integer) As Integer

    Function InsertTipoErogatore(item As TipoErogatoreVaccCommand) As Integer

    Function GetTipiErogatoriFromLuogoEsecuzione(codiceLuogoEsecuzione As String) As List(Of TipoErogatoreVacc)

    Function InsertLinkTipiErogatoreLuogo(idTipiErogatore As List(Of Integer), codiceLuogo As String) As Integer

    Function DeleteTipoErogatoreFromLuogo(codiceLuogo As String) As Integer

End Interface
