Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities

Public Class BizCategorieRischio
    Inherits Biz.BizClass

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos)
        '--
        MyBase.New(genericprovider, settings, contextInfos, Nothing)
        '--
    End Sub

#End Region

#Region " Types "

    ''' <summary>
    ''' Stringa utilizzata come codice e descrizione per l'elemento "TUTTE" aggiunto nell'elenco delle categorie rischio
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property CategorieRischio_Tutte As String
        Get
            Return "TUTTE"
        End Get
    End Property

#End Region

#Region " Public "

    ''' <summary>
    ''' Restituisce un datatable con codice e descrizione della categoria di rischio.
    ''' Se il parametro addEmptyRow è True, la prima riga del datatable sarà una riga vuota.
    ''' </summary>
    ''' <param name="aggiungiRigaTutte">indica se deve essere aggiunta una riga con codice e descrizione "TUTTE"</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LoadDataTableCategorieRischio(aggiungiRigaTutte As Boolean, aggiungiRigaVuota As Boolean) As DataTable

        Dim dtCategorieRischio As DataTable = Me.GenericProvider.CategorieRischio.LoadDataTableCategorieRischio()

        If aggiungiRigaTutte Or aggiungiRigaVuota Then

            If dtCategorieRischio Is Nothing Then dtCategorieRischio = New DataTable()

            Dim newRow As DataRow = Nothing

            If aggiungiRigaTutte Then

                newRow = dtCategorieRischio.NewRow()
                newRow("RSC_CODICE") = "TUTTE"
                newRow("RSC_DESCRIZIONE") = "TUTTE"

                dtCategorieRischio.Rows.InsertAt(newRow, 0)

            End If

            If aggiungiRigaVuota Then

                newRow = dtCategorieRischio.NewRow()
                newRow("RSC_CODICE") = String.Empty
                newRow("RSC_DESCRIZIONE") = String.Empty

                dtCategorieRischio.Rows.InsertAt(newRow, 0)

            End If

        End If

        Return dtCategorieRischio

    End Function

    ''' <summary>
    ''' Restituisce la descrizione della categoria di rischio in base al codice
    ''' </summary>
    ''' <param name="codiceCategoriaRischio"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDescrizioneCategoriaRischio(codiceCategoriaRischio As String) As String
        '--
        Return GenericProvider.CategorieRischio.GetDescrizioneCategoriaRischio(codiceCategoriaRischio)
        '--
    End Function

    ''' <summary>
    ''' Restituisce una lista di entità CondizioneRischio contenente codice rischio, descrizione rischio e vaccinazione associata,
    ''' in base alla categoria di rischio relativa al paziente specificato.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCondizioniRischioPaziente(codicePaziente As Integer) As List(Of CondizioneRischio)

        Return GenericProvider.CategorieRischio.GetCondizioniRischioPaziente(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce le condizioni di rischio relative al paziente e alla vaccinazione specificati
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="codiceVaccinazione"></param>
    ''' <returns></returns>
    Public Function GetCondizioniRischioPazienteVaccinazione(codicePaziente As Long, codiceVaccinazione As String) As List(Of PazienteCondizioneRischio)

        Return GenericProvider.CategorieRischio.GetCondizioniRischio(codicePaziente, codiceVaccinazione)

    End Function
    Public Function GetCondizioniRischioByAssociazione(codicePaziente As Long, codiceAssociazione As String) As List(Of PazienteCondizioneRischio)
        Return GenericProvider.CategorieRischio.GetCondizioniRischioByAssociazione(codicePaziente, codiceAssociazione)
    End Function

    ''' <summary>
    ''' Restituisce le coppie codice-descrizione per le categorie di rischio specificate
    ''' </summary>
    ''' <param name="codiciCategorieRischio"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCodiceDescrizioneCategorieRischio(codiciCategorieRischio As List(Of String)) As List(Of KeyValuePair(Of String, String))

        Return GenericProvider.CategorieRischio.GetCodiceDescrizioneCategorieRischio(codiciCategorieRischio)

    End Function

    ''' <summary>
    ''' Restituisce codice e descrizione della condizione di rischio del paziente specificato
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    Public Function GetCondizioneRischioPaziente(codicePaziente As Long) As CondizioneRischioCodiceDescrizione

        Return GenericProvider.CategorieRischio.GetCategoriaRischioPaziente(codicePaziente)

    End Function

#End Region

End Class
