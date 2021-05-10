Imports System.Collections.Generic

Imports Onit.OnAssistnet.OnVac
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.DataSet
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.Common.Utility


Public Class BizMotiviEsclusione
    Inherits BizClass

#Region " Constructors "

    Public Sub New(ByVal contextInfos As BizContextInfos)
        MyBase.New(contextInfos, Nothing)
    End Sub

    Public Sub New(ByRef genericprovider As DbGenericProvider, ByRef settings As Settings.Settings, ByVal contextInfos As BizContextInfos)
        MyBase.New(genericprovider, settings, contextInfos, Nothing)
    End Sub

#End Region

    ''' <summary>
    ''' Restituisce la data di scadenza dell'esclusione, in base al motivo impostato
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="codiceEsclusione"></param>
    ''' <param name="codiceVaccinazione"></param>
    ''' <param name="dataVisita"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetScadenzaMotivoEsclusione(codicePaziente As Integer, codiceEsclusione As String, codiceVaccinazione As String, dataVisita As Date) As DateTime

        Dim dataScadenza As DateTime

        Dim motivoEsclusione As MotivoEsclusione =
            Me.GenericProvider.MotiviEsclusione.GetMotivoEsclusione(codiceEsclusione)

        If Not motivoEsclusione Is Nothing Then

            Dim dataScadenzaCalcolata As DateTime? = Nothing

            ' Lista dei dati relativi al calcolo delle scadenza (prendo in considerazione solo per quelli relativi alla vaccinazione per cui si sta registrando l'esclusione)
            Dim listaScadenze As List(Of MotivoEsclusione.Scadenza) = motivoEsclusione.Scadenze.Where(Function(p) p.CodiciVaccinazioni.Contains(codiceVaccinazione)).ToList()

            Select Case motivoEsclusione.CalcoloScadenza

                Case Enumerators.MotiviEsclusioneCalcoloScadenza.NessunCalcolo
                    '---
                    ' Nessuna data di scadenza calcolata
                    '---

                Case Enumerators.MotiviEsclusioneCalcoloScadenza.Registrazione

                    Dim datiPrimaScadenza As MotivoEsclusione.Scadenza = listaScadenze.FirstOrDefault()
                    If Not datiPrimaScadenza Is Nothing Then
                        dataScadenza = Date.Now.AddMonths(datiPrimaScadenza.Mesi).AddYears(datiPrimaScadenza.Anni).Date
                    End If

                Case Enumerators.MotiviEsclusioneCalcoloScadenza.Nascita,
                     Enumerators.MotiviEsclusioneCalcoloScadenza.Visita

                    Dim dataPartenza As DateTime

                    If motivoEsclusione.CalcoloScadenza = Enumerators.MotiviEsclusioneCalcoloScadenza.Nascita Then
                        dataPartenza = Me.GenericProvider.Paziente.GetDataNascita(codicePaziente)
                    Else
                        dataPartenza = dataVisita
                    End If

                    If dataPartenza > Date.MinValue And dataPartenza.Date <= Date.Now.Date Then

                        For Each datiScadenza As MotivoEsclusione.Scadenza In listaScadenze

                            dataScadenzaCalcolata = dataPartenza.AddYears(datiScadenza.Anni).AddMonths(datiScadenza.Mesi)

                            If dataScadenzaCalcolata.Value > DateTime.Now.Date Then
                                Exit For
                            End If

                            dataScadenzaCalcolata = Nothing
                        Next

                        If Not dataScadenzaCalcolata Is Nothing Then
                            dataScadenza = dataScadenzaCalcolata.Value
                        End If

                    End If

            End Select
        End If

        Return dataScadenza

    End Function

    Public Function GetDataTableMotiviEsclusione() As DataTable

        Return Me.GenericProvider.MotiviEsclusione.GetDataTableMotiviEsclusione()

    End Function

    ''' <summary>
    ''' Restituisce i motivi di esclusione marcati come centralizzati
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCodiciMotiviCentralizzati() As List(Of String)

        Return GenericProvider.MotiviEsclusione.GetCodiciMotiviCentralizzati().ToList()

    End Function

End Class