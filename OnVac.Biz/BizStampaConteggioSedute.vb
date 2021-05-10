Imports System.Collections.Generic

Imports Onit.OnAssistnet.OnVac.DAL


Public Class BizStampaConteggioSedute
	Inherits BizClass

#Region " Constructors "

	Public Sub New(genericprovider As DbGenericProvider, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)

		MyBase.New(genericprovider, settings, contextInfos, logOptions)

	End Sub

#End Region

#Region " Types "

	Public Class ConteggioSeduteResult

		Public DataSetElencoEsclusione As DSConteggioSedute
		Public ParametriReport As List(Of KeyValuePair(Of String, String))

		Public Sub New()
			Me.DataSetElencoEsclusione = New DSConteggioSedute()
			Me.ParametriReport = New List(Of KeyValuePair(Of String, String))()
		End Sub

		Public Sub AddParameter(key As String, value As String)
			Me.ParametriReport.Add(New KeyValuePair(Of String, String)(key, value))
		End Sub

	End Class

#End Region

#Region " Public "

	Public Function GetConteggioSedute(nomeReport As String, filtriConteggioSedute As IStampaConteggioSedute.FiltriConteggioSedute) As ConteggioSeduteResult

		Dim elencoEsclusioneResult As New ConteggioSeduteResult()

		' --- Dataset --- '
		Select Case nomeReport

			Case Constants.ReportName.ConteggioSedute
				elencoEsclusioneResult.DataSetElencoEsclusione = Me.GenericProvider.StampaConteggioSedute.GetDataSetConteggioSedute(filtriConteggioSedute, ContextInfos.CodiceUsl)
			Case Else
				Throw New NotImplementedException()

		End Select


		' --- Parametri --- '

		' Date di nascita
		elencoEsclusioneResult.AddParameter("PeriodoVac1", filtriConteggioSedute.DataEffettuazioneInizio.ToString("dd/MM/yyyy"))
		elencoEsclusioneResult.AddParameter("PeriodoVac2", filtriConteggioSedute.DataEffettuazioneFine.ToString("dd/MM/yyyy"))

		' Consultorio

		If filtriConteggioSedute.CodiceConsultorio.Count > 0 Then
			elencoEsclusioneResult.AddParameter("Consultorio", filtriConteggioSedute.DescrizioneConsultori)
		Else
			elencoEsclusioneResult.AddParameter("Consultorio", "TUTTI")
		End If

		' Distretto
		If Not String.IsNullOrEmpty(filtriConteggioSedute.CodiceDistretto) And Not String.IsNullOrEmpty(filtriConteggioSedute.DescrizioneDistretto) Then
			elencoEsclusioneResult.AddParameter("Distretto", filtriConteggioSedute.DescrizioneDistretto)
		Else
			elencoEsclusioneResult.AddParameter("Distretto", "TUTTI")
		End If

		' filtri per tipo consultorio (A - adulti, P - pediatrico, V - vaccinatore)
		If (filtriConteggioSedute.FlagTipoCentroAdulti = True) And (filtriConteggioSedute.FlagTipoCentroPediatrico = True) And (filtriConteggioSedute.FlagTipoCentroVaccinatore = False) Then

			elencoEsclusioneResult.AddParameter("TipoCns", "ADULTI E PEDIATRICO")

		ElseIf (filtriConteggioSedute.FlagTipoCentroAdulti = True) And (filtriConteggioSedute.FlagTipoCentroPediatrico = False) And (filtriConteggioSedute.FlagTipoCentroVaccinatore = True) Then

			elencoEsclusioneResult.AddParameter("TipoCns", "ADULTI E VACCINATORE")

		ElseIf (filtriConteggioSedute.FlagTipoCentroAdulti = True) And (filtriConteggioSedute.FlagTipoCentroPediatrico = False) And (filtriConteggioSedute.FlagTipoCentroVaccinatore = False) Then


			elencoEsclusioneResult.AddParameter("TipoCns", "ADULTI")

		ElseIf (filtriConteggioSedute.FlagTipoCentroAdulti = False) And (filtriConteggioSedute.FlagTipoCentroPediatrico = True) And (filtriConteggioSedute.FlagTipoCentroVaccinatore = False) Then


			elencoEsclusioneResult.AddParameter("TipoCns", "PEDIATRICO")

		ElseIf (filtriConteggioSedute.FlagTipoCentroAdulti = False) And (filtriConteggioSedute.FlagTipoCentroPediatrico = False) And (filtriConteggioSedute.FlagTipoCentroVaccinatore = True) Then


			elencoEsclusioneResult.AddParameter("TipoCns", "VACCINATORE")

		ElseIf (filtriConteggioSedute.FlagTipoCentroAdulti = False) And (filtriConteggioSedute.FlagTipoCentroPediatrico = True) And (filtriConteggioSedute.FlagTipoCentroVaccinatore = True) Then


			elencoEsclusioneResult.AddParameter("TipoCns", "PEDIATRICO E VACCINATORE")

		Else
			elencoEsclusioneResult.AddParameter("TipoCns", "ADULTI PEDIATRICO E VACCINATORE")
		End If



		Return elencoEsclusioneResult

	End Function

#End Region

End Class
