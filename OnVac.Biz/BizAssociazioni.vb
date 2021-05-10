Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL


Public Class BizAssociazioni
    Inherits BizClass

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos)
        MyBase.New(genericprovider, settings, contextInfos, Nothing)
    End Sub

#End Region

#Region " Constants "

    Public Const MAXLENGTH_TITOLO As Integer = 100

#End Region

#Region " Types "

    Public Class DatiSomministrazioneAssociazione
        Public CodiceAssociazione As String
        Public InfoSomministrazioneAssociazione As Entities.InfoSomministrazione

        Public Sub New()
            Me.InfoSomministrazioneAssociazione = New Entities.InfoSomministrazione()
        End Sub

        Public Sub New(codiceAssociazione As String, infoSomministrazione As Entities.InfoSomministrazione)
            Me.CodiceAssociazione = codiceAssociazione
            Me.InfoSomministrazioneAssociazione = infoSomministrazione
        End Sub

    End Class

#End Region

#Region " Public "

    ''' <summary>
    ''' Restituisce un datatable con l'elenco delle associazioni che possono essere aggiunte alla convocazione dell'utente.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDtAssociazioniInseribiliInConvocazione(codicePaziente As Integer, dataConvocazione As Date, codiceConsultorio As String, codiciVaccinazioniProgrammateConvocazione As String()) As DataTable

        Return Me.GenericProvider.Associazioni.LoadAssociazioniDaInserire(codicePaziente, dataConvocazione, Me.Settings.ASSOCIAZIONI_TIPO_CNS,
                                                                          codiceConsultorio, codiciVaccinazioniProgrammateConvocazione,
                                                                          Me.Settings.SITO_INOCULAZIONE_SET_DEFAULT, Me.Settings.VIA_SOMMINISTRAZIONE_SET_DEFAULT)

    End Function

    ''' <summary>
    ''' Restituisce le informazioni di somministrazione relative all'associazione specificata
    ''' </summary>
    ''' <param name="codiceAssociazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetInfoSomministrazioneDefaultByAssociazione(codiceAssociazione As String) As Entities.InfoSomministrazione

        Dim infoSomministrazione As Entities.InfoSomministrazione =
            Me.GenericProvider.Associazioni.GetInfoSomministrazioneDefaultAssociazione(codiceAssociazione)

        If Not infoSomministrazione Is Nothing Then
            infoSomministrazione.FlagTipoValorizzazioneSito = Constants.TipoValorizzazioneSitoVia.DaAssociazione
            infoSomministrazione.FlagTipoValorizzazioneVia = Constants.TipoValorizzazioneSitoVia.DaAssociazione
        End If

        Return infoSomministrazione

    End Function

    ''' <summary>
    ''' Caricamento vaccinazioni che compongono le associazioni specificate e aggiunta dati di somministrazione relativi, specificati nella lista.
    ''' </summary>
    ''' <param name="listDatiSomministrazioneAssociazioni"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDtVaccinazioniAssociazioni(listDatiSomministrazioneAssociazioni As List(Of DatiSomministrazioneAssociazione)) As DataTable
        '--
        If listDatiSomministrazioneAssociazioni Is Nothing OrElse listDatiSomministrazioneAssociazioni.Count = 0 Then Return Nothing
        '--
        Dim listCodiciAssociazioni As List(Of String) =
            listDatiSomministrazioneAssociazioni.Select(Function(p) p.CodiceAssociazione).ToList()
        '--
        ' Caricamento vaccinazioni che compongono le associazioni specificate
        Dim dt As DataTable = Me.GenericProvider.Associazioni.GetDtVaccinazioniAssociazioni(listCodiciAssociazioni)
        '--
        dt.Columns.Add(New DataColumn("ass_vii_codice", GetType(String)))
        dt.Columns.Add(New DataColumn("vii_descrizione", GetType(String)))
        dt.Columns.Add(New DataColumn("FlagVia", GetType(String)))
        dt.Columns.Add(New DataColumn("ass_sii_codice", GetType(String)))
        dt.Columns.Add(New DataColumn("sii_descrizione", GetType(String)))
        dt.Columns.Add(New DataColumn("FlagSito", GetType(String)))
        '--
        For Each row As DataRow In dt.Rows
            '--
            Dim codiceAssociazione As String = row("VAL_ASS_CODICE").ToString()
            '--
            Dim infoSomministrazione As Entities.InfoSomministrazione =
                listDatiSomministrazioneAssociazioni.Where(Function(p) p.CodiceAssociazione = codiceAssociazione).
                                                     Select(Function(p) p.InfoSomministrazioneAssociazione).
                                                     FirstOrDefault()
            '--
            If Not infoSomministrazione Is Nothing Then
                '--
                row("ass_vii_codice") = infoSomministrazione.CodiceViaSomministrazione
                row("vii_descrizione") = infoSomministrazione.DescrizioneViaSomministrazione
                row("FlagVia") = infoSomministrazione.FlagTipoValorizzazioneVia
                '--
                row("ass_sii_codice") = infoSomministrazione.CodiceSitoInoculazione
                row("sii_descrizione") = infoSomministrazione.DescrizioneSitoInoculazione
                row("FlagSito") = infoSomministrazione.FlagTipoValorizzazioneSito
                '--
            End If
            '--
        Next
        '--
        dt.AcceptChanges()
        '--
        Return dt
        '--
    End Function

#End Region

#Region " Info Associazioni "

    Public Function GetAssociazioneInfo(codiceAssociazione As String) As Entities.AssociazioneInfo

        Return Me.GenericProviderCentrale.Associazioni.GetAssociazioneInfo(codiceAssociazione)

    End Function

    Public Class SaveAssociazioneInfoResult

        Public Success As Boolean
        Public Message As String

        Public Sub New()
            Me.New(True, String.Empty)
        End Sub

        Public Sub New(success As Boolean, message As String)
            Me.Success = success
            Me.Message = message
        End Sub

    End Class

    Public Function SaveAssociazioneInfo(id As Integer?, codiceAssociazione As String, infoTitolo As String, infoDescrizione As String) As SaveAssociazioneInfoResult

        If String.IsNullOrWhiteSpace(codiceAssociazione) Then
            Return New SaveAssociazioneInfoResult(False, "Salvataggio non effettuato: codice associazione mancante.")
        End If

        If Not String.IsNullOrWhiteSpace(infoTitolo) Then
            If infoTitolo.Length > MAXLENGTH_TITOLO Then
                infoTitolo = infoTitolo.Substring(0, MAXLENGTH_TITOLO)
            End If
        End If

        Dim info As New Entities.AssociazioneInfo()

        info.Id = id
        info.CodiceAssociazione = codiceAssociazione

        If String.IsNullOrWhiteSpace(infoTitolo) Then
            info.Titolo = String.Empty
        Else
            info.Titolo = infoTitolo
        End If

        If String.IsNullOrWhiteSpace(infoDescrizione) Then
            info.Descrizione = String.Empty
        Else
            info.Descrizione = infoDescrizione
        End If

        If Not info.Id.HasValue Then

            Me.GenericProviderCentrale.Associazioni.InsertAssociazioneInfo(info)

        Else

            If String.IsNullOrEmpty(info.Titolo) AndAlso String.IsNullOrEmpty(info.Descrizione) Then
                Me.GenericProviderCentrale.Associazioni.DeleteAssociazioneInfo(info.Id.Value)
            Else
                Me.GenericProviderCentrale.Associazioni.UpdateAssociazioneInfo(info)
            End If

        End If

        Return New SaveAssociazioneInfoResult()

    End Function

    Public Function GetListCodiciAssociazioni() As List(Of String)
        Return GenericProvider.Associazioni.GetListCodiciAssociazioni()
    End Function
#End Region

#Region " OnVac APP "

    ''' <summary>
    ''' Restituisce la lista (codice e descrizione) delle associazioni presenti in anagrafe, includendo solo quelle configurate per la visualizzazione nella APP.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetListAssociazioniAPP() As List(Of Entities.AssociazioneAPP)

        Return Me.GenericProvider.Associazioni.GetListAssociazioniAPP()

    End Function

    ''' <summary>
    ''' Restituisce le info relative all'associazione specificata, incluse tutte le vaccinazioni ad essa relative.
    ''' </summary>
    ''' <param name="codiceAssociazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAssociazioneInfoAPP(codiceAssociazione As String) As Entities.AssociazioneInfoAPP

        Dim infoApp As New Entities.AssociazioneInfoAPP()

        Dim info As Entities.AssociazioneInfo = Me.GenericProvider.Associazioni.GetAssociazioneInfo(codiceAssociazione)
        If Not info Is Nothing Then
            infoApp.CodiceAssociazione = info.CodiceAssociazione
            infoApp.DescrizioneAssociazione = info.DescrizioneAssociazione
            infoApp.Titolo = info.Titolo
            infoApp.Descrizione = info.Descrizione
            infoApp.VaccinazioniAPP = Me.GenericProvider.AnaVaccinazioni.GetListVaccinazioniAPP(codiceAssociazione)
        End If

        Return infoApp

    End Function

#End Region

End Class