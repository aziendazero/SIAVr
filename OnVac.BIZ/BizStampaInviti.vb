Imports System.Text.RegularExpressions

Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Biz
Imports System.Collections.Generic


Public Class BizStampaInviti
    Inherits Biz.BizClass

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos)

        MyBase.New(genericprovider, settings, contextInfos, Nothing)

    End Sub

#End Region

#Region " Public "

    ''' <summary>
    ''' Salvataggio della data di invio
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataConvocazione"></param>
    ''' <param name="tipoStampaAppuntamento"></param>
    ''' <param name="dataStampa"></param>
    ''' <param name="isPazienteRitardatario"></param>
    ''' <param name="noteAvvisi"></param>
    ''' <remarks></remarks>
    Public Sub SalvaDataInvio(codicePaziente As Int32, dataConvocazione As Date, tipoStampaAppuntamento As String, dataStampa As DateTime, isPazienteRitardatario As Boolean, noteAvvisi As String)

        If tipoStampaAppuntamento <> Constants.TipoStampaAppuntamento.BilanciMalattia Then

            ' Update convocazione
            GenericProvider.Convocazione.UpdateDataInvio(codicePaziente, dataConvocazione, dataStampa, noteAvvisi)

            ' Update storico appuntamenti (se presente => in caso di storico assente per dati recuperati, non trova nulla)
            Dim idStoricoAppuntamenti As Long? = GenericProvider.Prenotazioni.GetLastIdStoricoAppuntamenti(codicePaziente, dataConvocazione)

            If idStoricoAppuntamenti.HasValue Then
                GenericProvider.Prenotazioni.UpdateStoricoAppuntamenti_Invio(idStoricoAppuntamenti, ContextInfos.IDUtente, dataStampa, noteAvvisi)
            End If

        End If

        'aggiornamento data invio della T_CNV_CICLI
        'deve essere fatto solamente se si spedisce l'avviso => Constants.TipoStampaAppuntamento.Avvisi, AvvisoBilancio, CampagnaAdulti, ElencoAvvisi, EtichetteAvvisi
        If Regex.Match(tipoStampaAppuntamento, "A").Success Then
            If isPazienteRitardatario Then
                Me.GenericProvider.Convocazione.UpdateDataInvioSollecitoVaccinazioni(codicePaziente, dataConvocazione, dataStampa)
            End If
        End If

        'aggiornamento della T_BIL_SOLLECITI
        'deve essere fatto solamente se si spedisce il bilancio => Constants.TipoStampaAppuntamento.AvvisoBilancio, Bilanci, BilanciMalattia, ElencoBilanci, ElencoBilanciMalattia
        If Regex.Match(tipoStampaAppuntamento, "B").Success Then

            Dim listIdBilanci As List(Of Integer) = Nothing

            If Regex.Match(tipoStampaAppuntamento, "M").Success Then
                ' => Constants.TipoStampaAppuntamento.BilanciMalattia, ElencoBilanciMalattia
                listIdBilanci = RicavaIdBilanciAssociati(codicePaziente, dataConvocazione, True, False)
            Else
                listIdBilanci = RicavaIdBilanciAssociati(codicePaziente, dataConvocazione, False, True)
            End If

            For Each idBilancio As Integer In listIdBilanci
                GenericProvider.BilancioProgrammato.UpdateDataInvio(idBilancio, dataStampa)
                GenericProvider.SollecitiBilanci.Update(idBilancio, dataStampa)
            Next

        End If

    End Sub

    ''' <summary>
    ''' Salvataggio della data di invio dell'avviso per ogni paziente i cui dati della convocazione sono compresi nel periodo specificato.
    ''' Salvataggio delle date di inizio e fine del periodo di stampa, se richiesto.
    ''' </summary>
    ''' <param name="command"></param>
    ''' <remarks></remarks>
    Public Sub SalvaDateStampa(command As Entities.PazientiAvvisiCommand)
        '--
        ' Salvataggio data invio per ogni paziente
        '--
        Dim listPazientiAvvisi As List(Of Entities.PazienteAvviso) = Nothing

        If command.IsPostel Then
            listPazientiAvvisi = Me.GenericProvider.AppuntamentiGiorno.GetListPazientiAvvisiPostel(command)
        Else
            listPazientiAvvisi = Me.GenericProvider.AppuntamentiGiorno.GetListPazientiAvvisi(command)
        End If
        '--
        If Not listPazientiAvvisi Is Nothing AndAlso listPazientiAvvisi.Count > 0 Then
            '--
            Dim dataStampa As DateTime = Date.Now
            '--
            Try
                For Each pazienteAvviso As Entities.PazienteAvviso In listPazientiAvvisi
                    '--
                    Me.GenericProvider.BeginTransaction()
                    Me.SalvaDataInvio(pazienteAvviso.CodicePaziente, pazienteAvviso.DataConvocazione, command.TipoStampaAppuntamento, dataStampa, pazienteAvviso.IsPazienteRitardatario, command.NoteAvvisi)
                    Me.GenericProvider.Commit()
                    '--
                Next
                '--
            Catch ex As Exception
                Me.GenericProvider.Rollback()
                ex.InternalPreserveStackTrace()
                Throw
            End Try
            '--
        End If
        '--
        ' Salvataggio data iniziale e finale del periodo, per avvisi e bilanci, solo se il tipo di stampa non è filtrata per "solo già avvisati". 
        ' Se il consultorio non è specificato, le date vengono salvate in tutti i consultori aperti.
        '--
        If command.FiltroPazientiAvvisati <> Enumerators.FiltroAvvisati.SoloAvvisati Then
            '--
            If command.TipoStampaAppuntamento = Constants.TipoStampaAppuntamento.Avvisi Then
                '--
                If String.IsNullOrEmpty(command.CodiceConsultorio) Then
                    Me.GenericProvider.Consultori.UpdateDateUltimaStampaAvvisi(command.DataInizioAppuntamento, command.DataFineAppuntamento)
                Else
                    Me.GenericProvider.Consultori.UpdateDateUltimaStampaAvvisi(command.CodiceConsultorio, command.DataInizioAppuntamento, command.DataFineAppuntamento)
                End If
                '--
            ElseIf command.TipoStampaAppuntamento = Constants.TipoStampaAppuntamento.Bilanci Then
                '--
                If String.IsNullOrEmpty(command.CodiceConsultorio) Then
                    Me.GenericProvider.Consultori.UpdateDateUltimaStampaAvvisiBilancio(command.DataInizioAppuntamento, command.DataFineAppuntamento)
                Else
                    Me.GenericProvider.Consultori.UpdateDateUltimaStampaAvvisiBilancio(command.CodiceConsultorio, command.DataInizioAppuntamento, command.DataFineAppuntamento)
                End If
                '--
            End If
            '--
        End If
        '--
    End Sub

    ''' <summary>
    ''' Controlla se un paziente è presente nella v_avvisi
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataConvocazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ControllaAvviso(codicePaziente As Integer, dataConvocazione As Date) As Boolean
        '--
        Return Me.GenericProvider.AppuntamentiGiorno.ControllaAvviso(codicePaziente, dataConvocazione)
        '--
    End Function

    ''' <summary>
    ''' Controlla se un paziente è presente nella v_bilanci
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataConvocazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ControllaSoloBilancio(codicePaziente As Integer, dataConvocazione As DateTime) As Boolean
        '--
        Return Me.GenericProvider.AppuntamentiGiorno.ControllaSoloBilancio(codicePaziente, dataConvocazione)
        '--
    End Function

    ''' <summary>
    ''' Controlla se la convocazione ha un bilancio associato
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataConvocazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ControllaBilancioAssociato(codicePaziente As Integer, dataConvocazione As Date) As Boolean

        Dim dtBilancio As New DataTable()

        dtBilancio = CercaBilanci(codicePaziente, dataConvocazione)

        If dtBilancio.Rows.Count > 0 Then
            For i As Integer = 0 To dtBilancio.Rows.Count - 1
                If Not dtBilancio.Rows(i)("BIP_BIL_NUMERO") Is DBNull.Value Then

                    If dtBilancio.Rows(i)("BIP_BIL_NUMERO") = "5" Or dtBilancio.Rows(i)("BIP_BIL_NUMERO") = "7" Then
                        ' vedi sotto...
                    End If

                    Return True

                End If
            Next
        End If

        Return False

    End Function

    Public Function CercaBilanci(codicePaziente As Integer, dataConvocazione As DateTime) As DataTable

        Dim dt As DataTable

        Using bizBil As New BizBilancioProgrammato(Me.GenericProvider, Me.Settings, Me.ContextInfos, Nothing)

            dt = bizBil.CercaBilanciDt(codicePaziente, dataConvocazione)

        End Using

        Return dt

    End Function

#End Region

#Region " Private "

    ''' <summary>
    ''' Restituisce un arraylist contenente gli id dei bilanci in base a paziente e data di convocazione
    ''' </summary>
    ''' <param name="pazCodice"></param>
    ''' <param name="dataCnv"></param>
    ''' <param name="includiMalattiaCronica"></param>
    ''' <param name="includiNessunaMalattia"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function RicavaIdBilanciAssociati(pazCodice As Integer, dataCnv As Date, includiMalattiaCronica As Boolean, includiNessunaMalattia As Boolean) As List(Of Integer)

        Dim listId As New List(Of Integer)()

        Dim dtBilancio As DataTable = CercaBilanci(pazCodice, dataCnv)

        If dtBilancio.Rows.Count > 0 Then

            For i As Int16 = 0 To dtBilancio.Rows.Count - 1

                If Not dtBilancio.Rows(i)("BIP_BIL_NUMERO") Is DBNull.Value Then

                    If Not dtBilancio.Rows(i)("ID") Is DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(dtBilancio.Rows(i)("ID").ToString()) Then

                        If dtBilancio.Rows(i)("BIP_MAL_CODICE").ToString() = Me.Settings.CODNOMAL Then
                            If includiNessunaMalattia Then
                                listId.Add(Convert.ToInt32(dtBilancio.Rows(i)("ID")))
                            End If
                        Else
                            If includiMalattiaCronica Then
                                listId.Add(Convert.ToInt32(dtBilancio.Rows(i)("ID")))
                            End If
                        End If

                    End If

                    If dtBilancio.Rows(i)("BIP_BIL_NUMERO") = "5" Or dtBilancio.Rows(i)("BIP_BIL_NUMERO") = "7" Then
                        ' nessuno si ricorda cosa deve fare in questo caso
                    End If

                End If
            Next

        End If

        Return listId

    End Function

#End Region

End Class
