Imports System.ComponentModel
Imports System.Collections.Generic

Imports Onit.OnAssistnet.OnVac.DAL


Public Class BizCicli
    Inherits Biz.BizClass

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos)

        MyBase.New(genericprovider, settings, contextInfos, Nothing)

    End Sub

#End Region

#Region " Public "

    ''' <summary>
    ''' Caricamento cicli
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LoadCicli() As BindingList(Of Entities.Ciclo)
        '--
        Return Me.GenericProvider.Cicli.LoadCicli()
        '--
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pazCodice"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ExistsCicliConVaccinazione(pazCodice As Integer) As Boolean
        '--
        Return Me.GenericProvider.Cicli.ExistsCicliConVaccinazione(pazCodice)
        '--
    End Function

    ''' <summary>
    ''' Restituisce un datatable con le informazioni sui cicli in base alla categoria di rischio specificata
    ''' </summary>
    ''' <param name="codiceCategoriaRischio"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDtCicliByCategoriaRischio(codiceCategoriaRischio As String) As DataTable
        '--
        Return Me.GenericProvider.Cicli.GetDtCicliByCategoriaRischio(codiceCategoriaRischio)
        '--
    End Function

#Region " OnVac API "

    ''' <summary>
    ''' Restituisce il calendario vaccinale standard, calcolato in base ad età e sesso specificati
    ''' </summary>
    ''' <param name="dataNascita"></param>
    ''' <param name="sesso"></param>
    ''' <returns></returns>
    Public Function GetCalendarioVaccinaleStandard(dataNascita As DateTime, sesso As String) As List(Of Entities.CalendarioVaccinaleGenerico)

        Return Me.GenericProvider.Cicli.GetCalendarioVaccinaleStandard(dataNascita, sesso)

    End Function

    ''' <summary>
    ''' Restituisce il calendario vaccinale standard, calcolato in base ad età e sesso del paziente specificato
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCalendarioVaccinaleStandard(codicePaziente As Long) As List(Of Entities.CalendarioVaccinaleGenerico)

        Dim listCodiciPazienti As New List(Of Long)(1)
        listCodiciPazienti.Add(codicePaziente)

        Dim datiPaziente As List(Of Entities.DatiPazienteAPP) = Me.GenericProvider.Paziente.GetDatiPazientiAPP(listCodiciPazienti)

        If datiPaziente Is Nothing OrElse datiPaziente.Count = 0 Then
            Return New List(Of Entities.CalendarioVaccinaleGenerico)()
        End If

        Return Me.GetCalendarioVaccinaleStandard(datiPaziente.First().DataNascita, datiPaziente.First().Sesso)

    End Function

    ''' <summary>
    ''' Restituisce il calendario vaccinale dei pazienti specificati
    ''' </summary>
    ''' <param name="listCodiciPazienti"></param>
    ''' <returns></returns>
    Public Function GetCalendariVaccinaliPazienti(listCodiciPazienti As List(Of Long)) As List(Of Entities.CalendarioVaccinalePaziente)

        Dim calendariPazienti As New List(Of Entities.CalendarioVaccinalePaziente)()

        If listCodiciPazienti Is Nothing OrElse listCodiciPazienti.Count = 0 Then
            Return New List(Of Entities.CalendarioVaccinalePaziente)()
        End If

        Dim list As List(Of Entities.CalendarioVaccinalePaziente) =
            Me.GenericProvider.Cicli.GetCalendarioVaccinalePazienti(listCodiciPazienti)

        For Each item As Entities.CalendarioVaccinalePaziente In list
            item.AppIdAziendaLocale = Me.ContextInfos.IDApplicazione
        Next

        Return list

    End Function

#End Region


#End Region

End Class
