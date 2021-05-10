Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL


Public Class BizInterventi
    Inherits BizClass

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos)
        MyBase.New(genericprovider, settings, contextInfos, Nothing)
    End Sub

#End Region

#Region " Types "

    Private Class DescrizioneTipologia

        Public Const Assistenziale As String = "ASSISTENZIALE"
        Public Const Amministrativo As String = "AMMINISTRATIVO"
        Public Const Altro As String = "ALTRO"

    End Class

    Public Class ParametriGetInterventiPazienteBiz

        Public CodicePaziente As Integer
        Public CampoOrdinamento As String
        Public VersoOrdinamento As String

    End Class

    Public Class ConteggioConsulenzeResult

        Public DataSetConteggioConsulenze As DSConteggioConsulenze
        Public ParametriReport As List(Of KeyValuePair(Of String, String))

        Public Sub New()
            Me.DataSetConteggioConsulenze = New DSConteggioConsulenze()
            Me.ParametriReport = New List(Of KeyValuePair(Of String, String))()
        End Sub

        Public Sub AddParameter(key As String, value As String)
            Me.ParametriReport.Add(New KeyValuePair(Of String, String)(key, value))
        End Sub

    End Class

#End Region

#Region " Shared "

    Public Shared Function GetDescrizioneTipologiaIntervento(tipo As String) As String

        Dim descrizione As String = String.Empty

        If String.IsNullOrWhiteSpace(tipo) Then Return descrizione

        Select Case tipo

            Case Constants.TipologiaInterventi.Assistenziale
                descrizione = DescrizioneTipologia.Assistenziale

            Case Constants.TipologiaInterventi.Amministrativo
                descrizione = DescrizioneTipologia.Amministrativo

            Case Constants.TipologiaInterventi.Altro
                descrizione = DescrizioneTipologia.Altro

        End Select

        Return descrizione

    End Function

#End Region

#Region " Anagrafica "

    Public Function GetInterventi(filtro As String) As List(Of Entities.Intervento)

        Return GenericProvider.Interventi.GetInterventi(filtro.ToUpper())

    End Function

    Public Function GetIntervento(codiceIntervento As Integer) As Entities.Intervento

        Return GenericProvider.Interventi.GetIntervento(codiceIntervento)

    End Function

    Public Function SalvaIntervento(intervento As Entities.Intervento) As BizGenericResult

        If intervento Is Nothing Then

            Return New BizGenericResult(False, "Salvataggio non effettuato: nessun dato da salvare.")

        End If

        intervento.Descrizione = intervento.Descrizione.ToUpper()

        If Not intervento.Codice.HasValue Then
            '--
            ' INSERIMENTO
            '--
            Dim idIntervento As Integer = GenericProvider.Interventi.InsertIntervento(intervento)
            intervento.Codice = idIntervento

        Else
            '--
            ' MODIFICA
            '--
            GenericProvider.Interventi.UpdateIntervento(intervento)

        End If

        Return New BizGenericResult()

    End Function

    Public Function EliminaIntervento(codiceIntervento As Integer) As Integer

        Return GenericProvider.Interventi.DeleteIntervento(codiceIntervento)

    End Function

#End Region

#Region " Interventi paziente "

    Public Function GetInterventiPaziente(param As ParametriGetInterventiPazienteBiz) As List(Of Entities.InterventoPaziente)

        If param Is Nothing Then Throw New ArgumentNullException()

        Dim paramToPass As New ParametriGetInterventiPaziente()
        paramToPass.CodicePaziente = param.CodicePaziente
        paramToPass.OrderBy = GetOrderByInterventiPaziente(param.CampoOrdinamento, param.VersoOrdinamento)

        Dim lstInterventi As List(Of Entities.InterventoPaziente) = GenericProvider.Interventi.GetInterventiPaziente(paramToPass)

        If lstInterventi Is Nothing Then
            lstInterventi = New List(Of Entities.InterventoPaziente)()
        End If

        'Recupero la descrizione della tipologia intervento
        For Each intervento As Entities.InterventoPaziente In lstInterventi
            intervento.Tipologia = GetDescrizioneTipologiaIntervento(intervento.Tipologia)
        Next

        Return lstInterventi

    End Function

    Public Function SalvaInterventoPaziente(intervento As Entities.InterventoPaziente, codicePaziente As Integer, uteId As Long) As BizGenericResult

        If intervento Is Nothing Then

            Return New BizGenericResult(False, "Salvataggio non effettuato: nessun dato da salvare.")

        End If

        If Not intervento.Codice.HasValue Then
            '--
            ' INSERIMENTO
            '--
            Dim idIntervento As Integer = GenericProvider.Interventi.InsertInterventoPaziente(intervento, codicePaziente, uteId)
            intervento.Codice = idIntervento

        Else
            '--
            ' MODIFICA
            '--
            GenericProvider.Interventi.UpdateInterventoPaziente(intervento)

        End If

        Return New BizGenericResult()

    End Function

    Public Function EliminaInterventoPaziente(codiceIntervento As Integer, uteId As Long) As Integer

        Return GenericProvider.Interventi.DeleteInterventoPaziente(codiceIntervento, uteId)

    End Function

    Public Function CountInterventiPaziente(codicePaziente As Long) As Integer

        Return GenericProvider.Interventi.CountInterventiPaziente(codicePaziente)

    End Function

#Region " Private "

    Private Function GetOrderByInterventiPaziente(campoOrdinamento As String, versoOrdinamento As String) As String

        'Ordinamento di default
        Dim orderBy As String = "pit_data_intervento desc, int_descrizione "

        'Ordinamento manuale
        If Not String.IsNullOrWhiteSpace(campoOrdinamento) Then

            'Aggiornamento campoOrdinamento
            Select Case campoOrdinamento
                Case "Intervento"
                    orderBy = String.Format("int_descrizione {0}", versoOrdinamento)
                Case "Data"
                    orderBy = String.Format("pit_data_intervento {0}", versoOrdinamento)
                Case "Tipologia"
                    orderBy = String.Format("int_tipologia {0}", versoOrdinamento)
                Case "Durata"
                    orderBy = String.Format("pit_durata {0}", versoOrdinamento)
                Case "Operatore"
                    orderBy = String.Format("ope_nome {0}", versoOrdinamento)
                Case "Note"
                    orderBy = String.Format("pit_note {0}", versoOrdinamento)
                Case Else
            End Select

        End If

        Return orderBy

    End Function

#End Region

#End Region

#Region " Stampa Conteggio Consulenze "

    Public Class FiltriConteggioConsulenze
        Public DataNascitaInizio As DateTime?
        Public DataNascitaFine As DateTime?
        Public DataEsecuzioneInizio As DateTime?
        Public DataEsecuzioneFine As DateTime?
        Public CodiceConsultorio As List(Of String)
        Public DescrizioneConsultori As String
        Public CodiceTipoConsulenza As String
        Public DescrizioneTipoConsulenza As String
        Public CodiceOperatore As String
        Public DescrizioneOperatore As String
    End Class

    Public Function GetConteggioConsulenze(nomeReport As String, filtri As FiltriConteggioConsulenze) As ConteggioConsulenzeResult

        Dim result As New ConteggioConsulenzeResult()

        ' --- Dataset --- '
        Select Case nomeReport

            Case Constants.ReportName.Consulenze
                result.DataSetConteggioConsulenze = GenericProvider.Interventi.GetDataSetConteggioConsulenze(
                    filtri.DataNascitaInizio, filtri.DataNascitaFine, filtri.DataEsecuzioneInizio, filtri.DataEsecuzioneFine,
                    filtri.CodiceConsultorio, filtri.CodiceTipoConsulenza, filtri.CodiceOperatore)
            Case Else
                Throw New NotImplementedException()

        End Select


        ' --- Parametri --- '

        ' Date di nascita
        If filtri.DataNascitaInizio.HasValue Then
            result.AddParameter("DataNascitaIniz", filtri.DataNascitaInizio.Value.ToString("dd/MM/yyyy"))
        Else
            result.AddParameter("DataNascitaIniz", String.Empty)
        End If
        If filtri.DataNascitaFine.HasValue Then
            result.AddParameter("DataNascitaFin", filtri.DataNascitaFine.Value.ToString("dd/MM/yyyy"))
        Else
            result.AddParameter("DataNascitaFin", String.Empty)
        End If

        ' Date di esecuzione
        If filtri.DataEsecuzioneInizio.HasValue Then
            result.AddParameter("DataEsecuzioneIniz", filtri.DataEsecuzioneInizio.Value.ToString("dd/MM/yyyy"))
        Else
            result.AddParameter("DataEsecuzioneIniz", String.Empty)
        End If
        If filtri.DataEsecuzioneFine.HasValue Then
            result.AddParameter("DataEsecuzioneFin", filtri.DataEsecuzioneFine.Value.ToString("dd/MM/yyyy"))
        Else
            result.AddParameter("DataEsecuzioneFin", String.Empty)
        End If

        ' Consultorio
        If Not filtri.CodiceConsultorio Is Nothing AndAlso filtri.CodiceConsultorio.Count > 0 Then
            result.AddParameter("DescrCentriVaccinali", filtri.DescrizioneConsultori)
        Else
            result.AddParameter("DescrCentriVaccinali", "TUTTI")
        End If

        ' Tipo consulenza
        If Not String.IsNullOrEmpty(filtri.CodiceTipoConsulenza) And Not String.IsNullOrEmpty(filtri.DescrizioneTipoConsulenza) Then
            result.AddParameter("DescrTipoConsulenza", filtri.DescrizioneTipoConsulenza)
        Else
            result.AddParameter("DescrTipoConsulenza", "TUTTI")
        End If

        ' Operatore
        If Not String.IsNullOrEmpty(filtri.CodiceOperatore) And Not String.IsNullOrEmpty(filtri.DescrizioneOperatore) Then
            result.AddParameter("DescrOperatore", filtri.DescrizioneOperatore)
        Else
            result.AddParameter("DescrOperatore", "TUTTI")
        End If

        Return result

    End Function

#End Region

End Class