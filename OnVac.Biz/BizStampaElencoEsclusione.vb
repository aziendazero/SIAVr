Imports System.Collections.Generic

Imports Onit.OnAssistnet.OnVac.DAL


Public Class BizStampaElencoEsclusione
    Inherits BizClass

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)

        MyBase.New(genericprovider, settings, contextInfos, logOptions)

    End Sub

#End Region

#Region " Types "

    Public Class ElencoEsclusioneResult

        Public DataSetElencoEsclusione As DataSet.DSElencoEsclusione
        Public ParametriReport As List(Of KeyValuePair(Of String, String))

        Public Sub New()
            Me.DataSetElencoEsclusione = New DataSet.DSElencoEsclusione()
            Me.ParametriReport = New List(Of KeyValuePair(Of String, String))()
        End Sub

        Public Sub AddParameter(key As String, value As String)
            Me.ParametriReport.Add(New KeyValuePair(Of String, String)(key, value))
        End Sub

    End Class

#End Region

#Region " Public "

    Public Function GetElencoEsclusione(nomeReport As String, filtriElencoEsclusione As IStampaElencoEsclusione.FiltriElencoEsclusione) As ElencoEsclusioneResult

        Dim elencoEsclusioneResult As New ElencoEsclusioneResult()

        ' --- Dataset --- '
        Select Case nomeReport

            Case Constants.ReportName.ElencoEsclusione
                elencoEsclusioneResult.DataSetElencoEsclusione = Me.GenericProvider.StampaElencoEsclusione.GetDataSetElencoEsclusione(filtriElencoEsclusione, ContextInfos.CodiceUsl)

            Case Constants.ReportName.ElencoEsclusioneVaccinazioni
                elencoEsclusioneResult.DataSetElencoEsclusione = Me.GenericProvider.StampaElencoEsclusione.GetDataSetElencoEsclusioneVaccinazioni(filtriElencoEsclusione)

            Case Else
                Throw New NotImplementedException()

        End Select


        ' --- Parametri --- '

        ' Date di nascita
        elencoEsclusioneResult.AddParameter("DataNascitaIniz", filtriElencoEsclusione.DataNascitaInizio.ToString("dd/MM/yyyy"))
        elencoEsclusioneResult.AddParameter("DataNascitaFin", filtriElencoEsclusione.DataNascitaFine.ToString("dd/MM/yyyy"))

		' Consultorio

		elencoEsclusioneResult.AddParameter("Consultorio", filtriElencoEsclusione.DescrizioneConsultorio)

		' Comune di residenza
		If Not String.IsNullOrEmpty(filtriElencoEsclusione.CodiceComuneResidenza) And Not String.IsNullOrEmpty(filtriElencoEsclusione.DescrizioneComuneResidenza) Then
            elencoEsclusioneResult.AddParameter("ComRes", filtriElencoEsclusione.DescrizioneComuneResidenza)
        Else
            elencoEsclusioneResult.AddParameter("ComRes", "TUTTI")
        End If

        ' Circoscrizione
        If Not String.IsNullOrEmpty(filtriElencoEsclusione.CodiceCircoscrizione) And Not String.IsNullOrEmpty(filtriElencoEsclusione.DescrizioneCircoscrizione) Then
            elencoEsclusioneResult.AddParameter("Circoscriz", filtriElencoEsclusione.DescrizioneCircoscrizione)
        Else
            elencoEsclusioneResult.AddParameter("Circoscriz", "TUTTE")
        End If

        ' Distretto
        If Not String.IsNullOrEmpty(filtriElencoEsclusione.CodiceDistretto) And Not String.IsNullOrEmpty(filtriElencoEsclusione.DescrizioneDistretto) Then
            elencoEsclusioneResult.AddParameter("Distretto", filtriElencoEsclusione.DescrizioneDistretto)
        Else
            elencoEsclusioneResult.AddParameter("Distretto", "TUTTI")
        End If

        ' Vaccinazioni obbligatorie
        If filtriElencoEsclusione.SoloVaccinazioniObbligatorie Then
            elencoEsclusioneResult.AddParameter("TipoVaccinazione", "Obbligatorie")
        Else
            elencoEsclusioneResult.AddParameter("TipoVaccinazione", "TUTTE")
        End If

        ' Motivo esclusione
        If Not String.IsNullOrEmpty(filtriElencoEsclusione.CodiceMotivoEsclusione) Then
            elencoEsclusioneResult.AddParameter("Esclusione", filtriElencoEsclusione.DescrizioneMotivoEsclusione)
        Else
            elencoEsclusioneResult.AddParameter("Esclusione", "TUTTI")
        End If

        ' Vaccinazioni
        If Not String.IsNullOrEmpty(filtriElencoEsclusione.CodiceVaccinazione) Then
            elencoEsclusioneResult.AddParameter("NomeVaccinazione", filtriElencoEsclusione.DescrizioneVaccinazione)
        Else
            elencoEsclusioneResult.AddParameter("NomeVaccinazione", "TUTTE")
        End If

        ' Stato anagrafico
        If Not String.IsNullOrEmpty(filtriElencoEsclusione.CodiciStatiAnagrafici) Then
            elencoEsclusioneResult.AddParameter("StatoAnagrafico", filtriElencoEsclusione.DescrizioniStatiAnagrafici)
        Else
            elencoEsclusioneResult.AddParameter("StatoAnagrafico", "TUTTI")
        End If

        Return elencoEsclusioneResult

    End Function

#End Region

End Class
