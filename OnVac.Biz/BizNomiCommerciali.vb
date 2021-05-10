Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.Common.Utility.TypesHelper
Imports Onit.OnAssistnet.OnVac.Common.Utility

Public Class BizNomiCommerciali
    Inherits BizClass

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos)
        MyBase.New(genericprovider, settings, contextInfos, Nothing)
    End Sub

#End Region

#Region " Public "

#Region " Nome Commerciale "

    ''' <summary>
    ''' Restituisce la descrizione del nome commerciale in base al codice
    ''' </summary>
    ''' <param name="codiceNomeCommerciale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDescrizioneNomeCommerciale(codiceNomeCommerciale As String) As String

        Return Me.GenericProvider.NomiCommerciali.GetDescrizioneNomeCommerciale(codiceNomeCommerciale)

    End Function

    ''' <summary>
    ''' Restituisce le informazioni di somministrazione relative al nome commerciale specificato
    ''' </summary>
    ''' <param name="codiceNomeCommerciale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetInfoSomministrazioneDefaultByNomeCommerciale(codiceNomeCommerciale As String) As InfoSomministrazione

        Dim infoSomministrazione As InfoSomministrazione =
            Me.GenericProvider.NomiCommerciali.GetInfoSomministrazioneDefaultNomeCommerciale(codiceNomeCommerciale)

        If Not infoSomministrazione Is Nothing Then
            infoSomministrazione.FlagTipoValorizzazioneSito = Constants.TipoValorizzazioneSitoVia.DaNomeCommerciale
            infoSomministrazione.FlagTipoValorizzazioneVia = Constants.TipoValorizzazioneSitoVia.DaNomeCommerciale
        End If

        Return infoSomministrazione

    End Function

    ''' <summary>
    ''' Restituisce il costo unitario per il nome commerciale specificato
    ''' </summary>
    ''' <param name="codiceNomeCommerciale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCostoUnitarioNomeCommerciale(codiceNomeCommerciale As String) As Double

        Return Me.GenericProvider.NomiCommerciali.GetCostoUnitarioNomeCommerciale(codiceNomeCommerciale)

    End Function

#End Region

#Region " Tipi Pagamento "

#Region " Utility gestione command "

    <Serializable>
    Public Class TipiPagamentoWeb

        Public Property Guid As Guid
        Public Property Descrizione As String
        Public Property CodiceEsterno As String
        Public Property FlagStatoCampoImporto As String
        Public Property FlagStatoCampoEsenzione As String
        Public Property AutoSetImporto As Boolean
        Public Property HasCondizioniPagamento As Boolean
        Public Property CodiceAvn As String

        Public Property DescrizioneMaxLength As Integer
        Public Property CodiceEsternoMaxLength As Integer
        Public Property CodiceAvnMaxLength As Integer

    End Class

    Public Function MapTipoPagamentoToWeb(tipoPag As TipiPagamento) As TipiPagamentoWeb

        Return New TipiPagamentoWeb With {.Guid = tipoPag.GuidPagamento, .Descrizione = tipoPag.Descrizione, .CodiceEsterno = tipoPag.CodiceEsterno, .FlagStatoCampoImporto = GetStringFromNullableInt(tipoPag.FlagStatoCampoImporto), .FlagStatoCampoEsenzione = GetStringFromNullableInt(tipoPag.FlagStatoCampoEsenzione), .CodiceAvn = tipoPag.CodiceAvn, .AutoSetImporto = GetBooleanFromString(tipoPag.AutoSetImporto), .HasCondizioniPagamento = GetBooleanFromString(tipoPag.HasCondizioniPagamento)
        }

    End Function

    Public Function GetBizCommandFromWeb(webCommand As TipiPagamentoWeb) As TipiPagamento

        Return New TipiPagamento With {
            .GuidPagamento = webCommand.Guid,
            .Descrizione = webCommand.Descrizione,
            .CodiceEsterno = webCommand.CodiceEsterno,
            .FlagStatoCampoImporto = [Enum].Parse(GetType(Enumerators.StatoAbilitazioneCampo), Integer.Parse(webCommand.FlagStatoCampoImporto)),
            .FlagStatoCampoEsenzione = [Enum].Parse(GetType(Enumerators.StatoAbilitazioneCampo), Integer.Parse(webCommand.FlagStatoCampoEsenzione)),
            .AutoSetImporto = GetStringFromBoolean(webCommand.AutoSetImporto),
            .HasCondizioniPagamento = GetStringFromBoolean(webCommand.HasCondizioniPagamento),
            .CodiceAvn = webCommand.CodiceAvn
        }

    End Function

    Public Function ControlloCampiTipoPagamento(command As TipiPagamentoWeb) As BizGenericResult

        command.Descrizione.Trim().ToUpper()
        command.CodiceEsterno.Trim().ToUpper()

        Dim result As New BizGenericResult()
        Dim textMessage As New Text.StringBuilder()

        If command.Descrizione.Length > command.DescrizioneMaxLength Then
            result.Success = False
            textMessage.Append("il campo ""Descrizione"" è troppo lungo, ")
        End If

        If String.IsNullOrWhiteSpace(command.Descrizione) Then
            result.Success = False
            textMessage.Append("il campo ""Descrizione"" è vuoto, ")
        End If

        If command.CodiceEsterno.Length > command.CodiceEsternoMaxLength Then
            result.Success = False
            textMessage.Append("il campo ""Codice Esterno"" è troppo lungo, ")
        End If

        If Not result.Success Then
            textMessage = textMessage.Remove(textMessage.Length - 2, 2)
            textMessage.Insert(0, "Attenzione, impossibile completare l'operazione: ")
            textMessage.Append("!")
        End If

        result.Message = textMessage.ToString()

        Return result

    End Function

#End Region

    ''' <summary>
    ''' Restituisce la lista dei tipi di pagamento. Gli stati nulli vengono convertiti in stati disabilitati.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetListTipiPagamento() As List(Of TipiPagamento)

        Dim listTipiPagamento As List(Of TipiPagamento) = GenericProvider.NomiCommerciali.GetListTipiPagamento()

        If Not listTipiPagamento Is Nothing AndAlso listTipiPagamento.Count > 0 Then

            For Each tipoPagamento As TipiPagamento In listTipiPagamento
                If Not tipoPagamento.FlagStatoCampoImporto.HasValue Then tipoPagamento.FlagStatoCampoImporto = Enumerators.StatoAbilitazioneCampo.Disabilitato
                If Not tipoPagamento.FlagStatoCampoEsenzione.HasValue Then tipoPagamento.FlagStatoCampoEsenzione = Enumerators.StatoAbilitazioneCampo.Disabilitato
            Next

        End If

        Return listTipiPagamento

    End Function

    ''' <summary>
    ''' Restituisce i dati del tipo di pagamento specificato
    ''' </summary>
    ''' <param name="guidTipoPagamento"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetTipoPagamento(guidTipoPagamento As Guid) As TipiPagamento

        Dim tipoPagamento As New TipiPagamento
        If Not guidTipoPagamento = Guid.Empty Then
            tipoPagamento = GenericProvider.NomiCommerciali.GetTipoPagamento(guidTipoPagamento.ToByteArray()).FirstOrDefault()
        End If
        If Not tipoPagamento.FlagStatoCampoImporto.HasValue Then tipoPagamento.FlagStatoCampoImporto = Enumerators.StatoAbilitazioneCampo.Disabilitato
        If Not tipoPagamento.FlagStatoCampoEsenzione.HasValue Then tipoPagamento.FlagStatoCampoEsenzione = Enumerators.StatoAbilitazioneCampo.Disabilitato

        Return tipoPagamento

    End Function

    Public Function GetTipoPagamentoWebByGuid(guidPagamento As Guid) As TipiPagamentoWeb

        Dim pagamentoWeb As New TipiPagamentoWeb()
        Dim pagamentoBiz As New TipiPagamento()

        pagamentoBiz = GenericProvider.NomiCommerciali.GetTipoPagamento(guidPagamento.ToByteArray()).First()

        If Not pagamentoBiz.FlagStatoCampoImporto.HasValue Then pagamentoBiz.FlagStatoCampoImporto = Enumerators.StatoAbilitazioneCampo.Disabilitato
        If Not pagamentoBiz.FlagStatoCampoEsenzione.HasValue Then pagamentoBiz.FlagStatoCampoEsenzione = Enumerators.StatoAbilitazioneCampo.Disabilitato

        pagamentoWeb = MapTipoPagamentoToWeb(pagamentoBiz)
        Return pagamentoWeb

    End Function

    ''' <summary>
    ''' Restituisce true se il tipo di pagamento specificato gestisce le condizioni di pagamento
    ''' </summary>
    ''' <param name="guidTipoPagamento"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function HasGestioneCondizioniPagamento(guidTipoPagamento As Byte()) As Boolean

        Dim condizioniPagamento As String = Me.GenericProvider.NomiCommerciali.GetGestioneCondizioniPagamento(guidTipoPagamento)

        Return (condizioniPagamento = "S")

    End Function

    Public Function GetTipiPagamento() As List(Of TipiPagamentoWeb)

        Dim tipiPagamento As List(Of TipiPagamento) = GenericProvider.NomiCommerciali.GetTipiPagamento()
        Dim tipiPagamentoWeb As New List(Of TipiPagamentoWeb)

        If Not tipiPagamento Is Nothing AndAlso tipiPagamento.Count > 0 Then

            For Each tipoPagamento As TipiPagamento In tipiPagamento
                If Not tipoPagamento.FlagStatoCampoImporto.HasValue Then tipoPagamento.FlagStatoCampoImporto = Enumerators.StatoAbilitazioneCampo.Disabilitato
                If Not tipoPagamento.FlagStatoCampoEsenzione.HasValue Then tipoPagamento.FlagStatoCampoEsenzione = Enumerators.StatoAbilitazioneCampo.Disabilitato

                tipiPagamentoWeb.Add(MapTipoPagamentoToWeb(tipoPagamento))
            Next

        End If

        Return tipiPagamentoWeb

    End Function

    Public Function UpdateTipoPagamento(command As TipiPagamentoWeb) As BizGenericResult

        Dim result As New BizGenericResult
        result = ControlloCampiTipoPagamento(command)

        If result.Success Then

            Dim bizCommand As New TipiPagamento()
            bizCommand = GetBizCommandFromWeb(command)

            GenericProvider.NomiCommerciali.UpdateTipoPagamento(bizCommand)

        End If

        Return result

    End Function

    Public Sub EliminaTipoPagamento(guidPagamento As Guid)

        GenericProvider.NomiCommerciali.DeleteTipoPagamento(guidPagamento.ToByteArray())

    End Sub

    Public Function AggiungiTipoPagamento(command As TipiPagamentoWeb) As BizGenericResult

        Dim result As New BizGenericResult()
        result = ControlloCampiTipoPagamento(command)

        If result.Success Then

            Dim bizCommand As New TipiPagamento()
            bizCommand = GetBizCommandFromWeb(command)

            GenericProvider.NomiCommerciali.AddTipoPagamento(bizCommand)

        End If

        Return result

    End Function

#End Region

#Region " Condizioni Pagamento "

    ''' <summary>
    ''' Restituisce le condizioni di pagamento relative al nome commerciale indicato e all'età del paziente specificato
    ''' </summary>
    ''' <param name="codiceNomeCommerciale"></param>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    Public Function GetListCondizioniPagamento(codiceNomeCommerciale As String, codicePaziente As Long) As List(Of CondizioniPagamento)

        If String.IsNullOrWhiteSpace(codiceNomeCommerciale) OrElse codicePaziente <= 0 Then
            Return New List(Of CondizioniPagamento)()
        End If

        Return GenericProvider.NomiCommerciali.GetListCondizioniPagamento(codiceNomeCommerciale, codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce le condizioni di pagamento relative al nome commerciale indicato
    ''' </summary>
    ''' <param name="codiceNomeCommerciale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetListCondizioniPagamento(codiceNomeCommerciale As String) As List(Of CondizioniPagamento)

        Return GenericProvider.NomiCommerciali.GetListCondizioniPagamento(codiceNomeCommerciale)

    End Function

    ''' <summary>
    ''' Restituisce l'oggetto CondizionePagamento in base all'id specificato
    ''' </summary>
    ''' <param name="idCondizionePagamento"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCondizionePagamento(idCondizionePagamento As Integer) As CondizioniPagamento

        Return GenericProvider.NomiCommerciali.GetCondizionePagamento(idCondizionePagamento)

    End Function

    ''' <summary>
    ''' Salvataggio condizione di pagamento specificata
    ''' </summary>
    ''' <param name="condizionePagamento"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SaveCondizionePagamento(condizionePagamento As CondizioniPagamento) As BizGenericResult

        If condizionePagamento Is Nothing Then
            '--
            Return New BizGenericResult(False, "Salvataggio non effettuato: nessun dato da salvare.")
            '--
        End If

        If String.IsNullOrEmpty(condizionePagamento.CodiceNomeCommerciale) Then
            '--
            Return New BizGenericResult(False, "Salvataggio non effettuato: nome commerciale non specificato")
            '--
        End If

        If Not condizionePagamento.EtaInizio.HasValue Then
            '--
            condizionePagamento.EtaInizio = 0
            '--
        End If

        ' Controllo età inizio e fine dell'elemento corrente
        If Not condizionePagamento.EtaFine Is Nothing AndAlso condizionePagamento.EtaInizio.Value > condizionePagamento.EtaFine.Value Then
            '--
            Return New BizGenericResult(False, "Salvataggio non effettuato: l'età iniziale della condizione non deve superare l'età finale.")
            '--
        End If

        ' Controllo condizione pagamento rispetto alle altre dello stesso nome commerciale
        Dim listCondizioniPagamentoEsistenti As List(Of CondizioniPagamento) =
            Me.GetListCondizioniPagamento(condizionePagamento.CodiceNomeCommerciale)

        If Not listCondizioniPagamentoEsistenti Is Nothing AndAlso listCondizioniPagamentoEsistenti.Count > 0 Then

            If condizionePagamento.IdCondizione.HasValue Then

                ' Se l'elemento che voglio salvare è già presente, lo tolgo dalla lista di quelli esistenti che servono per i controlli
                Dim itemToRemove As CondizioniPagamento =
                    listCondizioniPagamentoEsistenti.Where(Function(p) p.IdCondizione.Value = condizionePagamento.IdCondizione.Value).FirstOrDefault()

                If Not itemToRemove Is Nothing Then listCondizioniPagamentoEsistenti.Remove(itemToRemove)

            End If

            ' Controllo l'elemento corrente rispetto a quelli già esistenti
            If Not CheckIntervalliEta(condizionePagamento, listCondizioniPagamentoEsistenti) Then
                '--
                Return New BizGenericResult(False, "Salvataggio non effettuato: l'intervallo di età specificato non è congruente con quelli già esistenti.")
                '--
            End If

        End If

        Dim ownTransaction As Boolean = False

        Try
            If GenericProvider.Transaction Is Nothing Then
                GenericProvider.BeginTransaction()
                ownTransaction = True
            End If

            If Not condizionePagamento.IdCondizione.HasValue Then
                '--
                ' INSERIMENTO
                '--
                condizionePagamento.IdCondizione = GenericProvider.NomiCommerciali.NextValSequenceCondizionePagamento()
                '--
                GenericProvider.NomiCommerciali.InsertCondizionePagamento(condizionePagamento)
                '--
            Else
                '--
                ' MODIFICA
                '--
                GenericProvider.NomiCommerciali.UpdateCondizionePagamento(condizionePagamento)
                '--
            End If

            If ownTransaction Then
                GenericProvider.Commit()
            End If

        Catch ex As Exception

            If ownTransaction Then
                GenericProvider.Rollback()
            End If

            ex.InternalPreserveStackTrace()
            Throw

        End Try

        Return New BizGenericResult()

    End Function

    ''' <summary>
    ''' Cancellazione della condizione di pagamento specificata
    ''' </summary>
    ''' <param name="idCondizionePagamento"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteCondizionePagamento(idCondizionePagamento As Integer) As Integer

        Return GenericProvider.NomiCommerciali.DeleteCondizionePagamento(idCondizionePagamento)

    End Function

#End Region

#End Region

#Region " Private "

    ' Scarta la condizionePagamento se l'intervallo di età si accavalla con gli altri già esistenti
    Private Function CheckIntervalliEta(condizionePagamento As CondizioniPagamento, listCondizioniPagamentoEsistenti As List(Of CondizioniPagamento)) As Boolean

        For Each condizioneEsistente As CondizioniPagamento In listCondizioniPagamentoEsistenti

            If condizioneEsistente.EtaFine Is Nothing OrElse condizionePagamento.EtaInizio.Value < condizioneEsistente.EtaFine.Value Then
                '--
                If condizionePagamento.EtaFine Is Nothing Then Return False
                If condizionePagamento.EtaFine.Value > condizioneEsistente.EtaInizio.Value Then Return False
                '--
            End If

        Next

        Return True

    End Function

#End Region

End Class
