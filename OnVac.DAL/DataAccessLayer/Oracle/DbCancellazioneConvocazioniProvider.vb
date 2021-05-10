Imports Onit.Database.DataAccessManager
Imports System.Data.OracleClient
Imports Onit.OnAssistnet.Data
Imports System.Collections.Generic
Imports Onit.OnAssistnet.Data.OracleClient

Namespace DAL
    Public Class DbCancellazioneConvocazioniProvider
        Inherits DbProvider
        Implements ICancellazioneConvocazioniProvider

#Region " Costruttori "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

        ''' <summary>
        ''' Restituisce la lista di convocazioni in base ai filtri passati
        ''' </summary>
        Public Function GetConvocazioniPerUtilityCancellazione(param As ParametriGetCnvPerCancellazione) As List(Of Entities.ConvocazioneDaCancellare) Implements ICancellazioneConvocazioniProvider.GetConvocazioniPerUtilityCancellazione

            If param Is Nothing Then Throw New ArgumentNullException()

            Dim lstConvocazioni As New List(Of Entities.ConvocazioneDaCancellare)()

            Using cmd As OracleCommand = Me.Connection.CreateCommand()

                ' Impostazione filtri di ricerca
                Dim filtri As String = Me.SetFiltriQuery(param.Filtri, cmd)

                cmd.CommandText = String.Format(GetStringQuery(), filtri, param.OrderBy)

                'Paginazione
                If Not param.PagingOpts Is Nothing Then

                    cmd.AddPaginatedQuery(param.PagingOpts)

                End If

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim paz_codice As Integer = idr.GetOrdinal("paz_codice")
                            Dim paz_cognome As Integer = idr.GetOrdinal("paz_cognome")
                            Dim paz_nome As Integer = idr.GetOrdinal("paz_nome")
                            Dim paz_data_nascita As Integer = idr.GetOrdinal("paz_data_nascita")
                            Dim cnv_data As Integer = idr.GetOrdinal("cnv_data")
                            Dim cnv_data_appuntamento As Integer = idr.GetOrdinal("cnv_data_appuntamento")
                            Dim vaccinazioni As Integer = idr.GetOrdinal("vaccinazioni")

                            While idr.Read()

                                Dim item As New ConvocazioneDaCancellare()
                                item.Data = idr.GetDateTimeOrDefault(cnv_data)
                                item.DataAppuntamento = idr.GetNullableDateTimeOrDefault(cnv_data_appuntamento)
                                item.IdPaziente = idr.GetInt64OrDefault(paz_codice)
                                item.PazienteCognome = idr.GetStringOrDefault(paz_cognome)
                                item.PazienteDataNascita = idr.GetDateTimeOrDefault(paz_data_nascita)
                                item.PazienteNome = idr.GetStringOrDefault(paz_nome)
                                item.Vaccinazioni = idr.GetStringOrDefault(vaccinazioni) 'Campo calcolato da una funzione sul db

                                lstConvocazioni.Add(item)

                            End While

                        End If

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return lstConvocazioni

        End Function

        ''' <summary>
        ''' Restituisce il numero di convocazioni in base ai filtri passati
        ''' </summary>
        Public Function CountConvocazioniPerUtilityCancellazione(param As FiltriConvocazioneDaCancellare) As Integer Implements ICancellazioneConvocazioniProvider.CountConvocazioniPerUtilityCancellazione

            If param Is Nothing Then Throw New ArgumentNullException()

            Dim count As Integer = 0

            Using cmd As OracleCommand = Me.Connection.CreateCommand()

                ' Impostazione filtri di ricerca
                Dim filtri As String = Me.SetFiltriQuery(param, cmd)

                cmd.CommandText = String.Format(GetCountStringQuery(), filtri)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        count = Convert.ToInt32(obj)
                    End If

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Restituisce le chiavi primarie delle convocazioni in base ai filtri passati
        ''' </summary>
        Public Function GetIdConvocazioniPerUtilityCancellazione(param As FiltriConvocazioneDaCancellare) As List(Of Entities.ConvocazionePK) Implements ICancellazioneConvocazioniProvider.GetIdConvocazioniPerUtilityCancellazione

            If param Is Nothing Then Throw New ArgumentNullException()

            Dim listIdCnv As New List(Of Entities.ConvocazionePK)()

            Using cmd As OracleCommand = Me.Connection.CreateCommand()

                ' Impostazione filtri di ricerca
                cmd.CommandText =
                    String.Format(GetPrimaryKeysStringQuery(), Me.SetFiltriQuery(param, cmd))

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim paz_codice As Integer = idr.GetOrdinal("paz_codice")
                            Dim cnv_data As Integer = idr.GetOrdinal("cnv_data")

                            Dim pk As Entities.ConvocazionePK = Nothing

                            While idr.Read()

                                pk = New ConvocazionePK()
                                pk.IdPaziente = idr.GetInt64(paz_codice)
                                pk.Data = idr.GetDateTime(cnv_data)

                                listIdCnv.Add(pk)

                            End While

                        End If

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listIdCnv

        End Function

        ''' <summary>
        ''' Restituisce le chiavi primarie (da T_PAZ_ELABORAZIONI) delle convocazioni da cancellare 
        ''' </summary>
        Public Function GetConvocazioniDaEliminareByJobId(jobId As Long) As List(Of Entities.ConvocazionePK) Implements ICancellazioneConvocazioniProvider.GetConvocazioniDaEliminareByJobId

            Dim lstConvocazioni As New List(Of Entities.ConvocazionePK)()

            Using cmd As OracleCommand = Me.Connection.CreateCommand()

                cmd.CommandText = "select plb_paz_codice, plb_data_convocazione " +
                                  "from t_paz_elaborazioni where plb_id = :job"

                cmd.Parameters.AddWithValue("job", jobId)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim plb_paz_codice As Integer = idr.GetOrdinal("plb_paz_codice")
                            Dim plb_data_convocazione As Integer = idr.GetOrdinal("plb_data_convocazione")

                            While idr.Read()

                                Dim item As New Entities.ConvocazionePK()
                                item.Data = idr.GetDateTimeOrDefault(plb_data_convocazione)

                                Dim idPaziente As String = idr.GetStringOrDefault(plb_paz_codice)

                                If Not String.IsNullOrWhiteSpace(idPaziente) Then
                                    item.IdPaziente = Long.Parse(idr.GetStringOrDefault(plb_paz_codice))
                                End If

                                lstConvocazioni.Add(item)

                            End While

                        End If

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return lstConvocazioni

        End Function

        Public Function FiltraConvocazioniDaEliminare(param As ParametriFiltraCnvDaEliminare) As List(Of Entities.ConvocazionePK) Implements ICancellazioneConvocazioniProvider.FiltraConvocazioniDaEliminare

            If param Is Nothing Then Throw New ArgumentNullException()

            Dim lstConvocazioni As New List(Of Entities.ConvocazionePK)()

            If (param.CnvKeys IsNot Nothing AndAlso param.CnvKeys.Count > 0) Then

                Using cmd As OracleCommand = Me.Connection.CreateCommand()

                    cmd.CommandText = "select cnv_paz_codice, cnv_data " +
                                        "from t_cnv_convocazioni "

                    'Aggiunta del filtro in or delle chiavi
                    Dim lstKeys As List(Of KeyValuePair(Of Long, DateTime)) = param.CnvKeys.Select(Function(x) New KeyValuePair(Of Long, DateTime)(x.IdPaziente, x.Data)).ToList()
                    cmd.AddOrFiltersWhereClause("cnv_paz_codice", "cnv_data", lstKeys)

                    If param.EscludiCnvConAppuntamenti Then

                        'Filtro per escludere le convocazioni che hanno appuntamenti
                        cmd.CommandText += " and cnv_data_appuntamento is null "

                    End If

                    If param.EscludiCnvConSolleciti Then

                        'Filtro per escludere le convocazioni che hanno solleciti
                        cmd.CommandText += " and not exists ( " +
                            "select distinct 1 from t_cnv_cicli " +
                            "where cnc_cnv_paz_codice = cnv_paz_codice " +
                            "and cnc_cnv_data = cnv_data " +
                            "and nvl(cnc_n_sollecito, 0) > 0 ) "

                    End If

                    Dim ownConnection As Boolean = False

                    Try
                        ownConnection = Me.ConditionalOpenConnection(cmd)

                        Using idr As IDataReader = cmd.ExecuteReader()

                            If Not idr Is Nothing Then

                                Dim cnv_paz_codice As Integer = idr.GetOrdinal("cnv_paz_codice")
                                Dim cnv_data As Integer = idr.GetOrdinal("cnv_data")

                                While idr.Read()

                                    Dim item As New Entities.ConvocazionePK()
                                    item.IdPaziente = idr.GetInt64(cnv_paz_codice)
                                    item.Data = idr.GetDateTime(cnv_data)

                                    lstConvocazioni.Add(item)

                                End While

                            End If

                        End Using

                    Finally
                        Me.ConditionalCloseConnection(ownConnection)
                    End Try

                End Using

            End If

            Return lstConvocazioni

        End Function

        ''' <summary>
        ''' Elimina le vaccinazioni programmate del paziente in data specificata. 
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataConvocazione"></param>
        ''' <param name="codiceVac"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EliminaVaccinazioniProgrammate(codicePaziente As Long, dataConvocazione As DateTime, codiceVac As String) As Integer Implements ICancellazioneConvocazioniProvider.EliminaVaccinazioniProgrammate

            Using cmd As OracleCommand = Me.Connection.CreateCommand()

                cmd.CommandText = "delete from t_vac_programmate " +
                    "where vpr_paz_codice = :cod_paz " +
                    "and vpr_cnv_data = :dat_cnv "

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.Parameters.AddWithValue("dat_cnv", dataConvocazione)

                'Eventuale ulteriore filtro sul codice vac
                If (Not String.IsNullOrWhiteSpace(codiceVac)) Then

                    cmd.CommandText += "and vpr_vac_codice = :cod_vac"
                    cmd.Parameters.AddWithValue("cod_vac", codiceVac)

                End If

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Return cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

        End Function

        Public Function GetVaccinazioniProgrammateDaEliminare(param As ParametriVacProgDaEliminare) As List(Of Entities.VaccinazioneProgrammata) Implements ICancellazioneConvocazioniProvider.GetVaccinazioniProgrammateDaEliminare

            If param Is Nothing Then Throw New ArgumentNullException()

            Dim lstVac As New List(Of Entities.VaccinazioneProgrammata)()

            If (param.CnvKeys IsNot Nothing AndAlso param.CnvKeys.Count > 0 AndAlso param.FiltriProgrammazione IsNot Nothing AndAlso param.FiltriProgrammazione.Count > 0) Then

                Using cmd As OracleCommand = Me.Connection.CreateCommand()

                    cmd.CommandText = "select vpr_paz_codice, vpr_cnv_data, vpr_ass_codice, " +
                            "vpr_vac_codice, vpr_n_richiamo, vpr_cic_codice, vpr_n_seduta " +
                            "from t_vac_programmate "

                    'Aggiunta del filtro in or delle chiavi convocazione
                    Dim lstKeys As List(Of KeyValuePair(Of Long, DateTime)) = param.CnvKeys.Select(Function(x) New KeyValuePair(Of Long, DateTime)(x.IdPaziente, x.Data)).ToList()
                    cmd.AddOrFiltersWhereClause("vpr_paz_codice", "vpr_cnv_data", lstKeys)

                    'Aggiunta del filtro in or delle chiavi vac programmate
                    Dim lstFiltri As List(Of KeyValuePair(Of String, Integer?)) = param.FiltriProgrammazione.Select(Function(x) New KeyValuePair(Of String, Integer?)(x.Codice, x.Valore)).ToList()

                    If param.TipoFiltri.HasValue Then

                        Select Case param.TipoFiltri.Value

                            Case TipoFiltriProgrammazione.AssociazioneDose
                                cmd.AddOrFiltersAndClause("vpr_ass_codice", "vpr_n_richiamo", lstFiltri)

                            Case TipoFiltriProgrammazione.CicloSeduta
                                cmd.AddOrFiltersAndClause("vpr_cic_codice", "vpr_n_seduta", lstFiltri)

                            Case TipoFiltriProgrammazione.VaccinazioneDose
                                cmd.AddOrFiltersAndClause("vpr_vac_codice", "vpr_n_richiamo", lstFiltri)

                        End Select

                    End If

                    Dim ownConnection As Boolean = False

                    Try
                        ownConnection = Me.ConditionalOpenConnection(cmd)

                        Using idr As IDataReader = cmd.ExecuteReader()

                            If Not idr Is Nothing Then

                                Dim vpr_paz_codice As Integer = idr.GetOrdinal("vpr_paz_codice")
                                Dim vpr_cnv_data As Integer = idr.GetOrdinal("vpr_cnv_data")
                                Dim vpr_ass_codice As Integer = idr.GetOrdinal("vpr_ass_codice")
                                Dim vpr_vac_codice As Integer = idr.GetOrdinal("vpr_vac_codice")
                                Dim vpr_n_richiamo As Integer = idr.GetOrdinal("vpr_n_richiamo")
                                Dim vpr_cic_codice As Integer = idr.GetOrdinal("vpr_cic_codice")
                                Dim vpr_n_seduta As Integer = idr.GetOrdinal("vpr_n_seduta")

                                While idr.Read()

                                    Dim item As New Entities.VaccinazioneProgrammata()
                                    item.CodicePaziente = idr.GetInt64(vpr_paz_codice)
                                    item.DataConvocazione = idr.GetDateTime(vpr_cnv_data)
                                    item.CodiceAssociazione = idr.GetStringOrDefault(vpr_ass_codice)
                                    item.CodiceVaccinazione = idr.GetString(vpr_vac_codice)
                                    item.NumeroRichiamo = idr.GetInt32(vpr_n_richiamo)
                                    item.CodiceCiclo = idr.GetStringOrDefault(vpr_cic_codice)
                                    item.NumeroSeduta = idr.GetNullableInt32OrDefault(vpr_n_seduta)

                                    lstVac.Add(item)

                                End While

                            End If

                        End Using

                    Finally
                        Me.ConditionalCloseConnection(ownConnection)
                    End Try

                End Using

            End If

            Return lstVac

        End Function

        Public Function EliminaCicliEmpty(codicePaziente As Long, dataConvocazione As DateTime, codiceCiclo As String, numSeduta As Integer?) As Integer Implements ICancellazioneConvocazioniProvider.EliminaCicliEmpty

            Dim result As Integer = -1

            If (Not String.IsNullOrWhiteSpace(codiceCiclo) AndAlso numSeduta.HasValue) Then

                'Conteggio vaccinazioni programmate ancora presenti per il ciclo selezionato
                Dim count As Integer = CountVaccinazioniProgrammateByCiclo(codicePaziente, dataConvocazione, codiceCiclo, numSeduta.Value)

                If (count = 0) Then

                    'Non sono presenti vaccinazioni programmate per il ciclo indicato. Posso eliminare il ciclo.

                    Using cmd As OracleCommand = Me.Connection.CreateCommand()

                        cmd.CommandText = "delete from t_cnv_cicli " +
                            "where cnc_cnv_paz_codice = :codicePaz and cnc_cnv_data = :dataCnv " +
                            "and cnc_cic_codice = :ciclo and cnc_sed_n_seduta = :seduta "

                        cmd.Parameters.AddWithValue("codicePaz", codicePaziente)
                        cmd.Parameters.AddWithValue("dataCnv", dataConvocazione)
                        cmd.Parameters.AddWithValue("ciclo", codiceCiclo)
                        cmd.Parameters.AddWithValue("seduta", numSeduta)

                        Dim ownConnection As Boolean = False

                        Try
                            ownConnection = Me.ConditionalOpenConnection(cmd)

                            result = cmd.ExecuteNonQuery()

                        Finally
                            Me.ConditionalCloseConnection(ownConnection)
                        End Try

                    End Using

                End If

            End If

            Return result

        End Function

        ''' <summary>
        ''' Recupera tutti i record della t_paz_elaborazioni in base a job id, paziente e data di convocazione.
        ''' </summary>
        ''' <param name="jobId"></param>
        ''' <param name="listCodiciPazientiDateCnv"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetElaborazioniJobCorrente(jobId As Long, listCodiciPazientiDateCnv As List(Of Entities.ConvocazionePK)) As List(Of JobItem) Implements ICancellazioneConvocazioniProvider.GetElaborazioniJobCorrente

            Dim listElaborazioni As New List(Of JobItem)()

            Using cmd As OracleCommand = Me.Connection.CreateCommand()

                cmd.CommandText = "select plb_progressivo, plb_paz_codice, plb_data_convocazione " +
                                  "from t_paz_elaborazioni where plb_id = :plb_id "

                Dim filterList As List(Of KeyValuePair(Of String, DateTime)) = listCodiciPazientiDateCnv.Select(
                    Function(item) New KeyValuePair(Of String, DateTime)(item.IdPaziente.ToString(), item.Data)).ToList()

                cmd.AddOrFiltersAndClause("plb_paz_codice", "plb_data_convocazione", filterList)
                cmd.Parameters.AddWithValue("plb_id", jobId)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim plb_progressivo As Integer = idr.GetOrdinal("plb_progressivo")
                            Dim plb_paz_codice As Integer = idr.GetOrdinal("plb_paz_codice")
                            Dim plb_data_convocazione As Integer = idr.GetOrdinal("plb_data_convocazione")

                            While idr.Read()

                                Dim item As New JobItem()
                                item.Progressivo = idr.GetInt64(plb_progressivo)
                                item.CodicePaziente = idr.GetString(plb_paz_codice)
                                item.DataConvocazione = idr.GetDateTime(plb_data_convocazione)

                                listElaborazioni.Add(item)

                            End While

                        End If

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listElaborazioni

        End Function

#Region " Private methods "

        ''' <summary>
        ''' Restituisce una stringa creata in base a tutti i filtri specificati e aggiunge i parametri al command.
        ''' </summary>
        Private Function SetFiltriQuery(filtriRicerca As Entities.FiltriConvocazioneDaCancellare, cmd As OracleCommand) As String

            If filtriRicerca Is Nothing Then Return String.Empty

            Dim filtri As New System.Text.StringBuilder()

            'Filtri obbligatori
            '---------------Filtri obbligatori-----------------------------

            filtri.Append(" where cnv_cns_codice = :codCentroVac ")
            cmd.Parameters.AddWithValue("codCentroVac", filtriRicerca.CodiceCentroVaccinale)

            '--------------------------------------------------------------

            '---------------Filtri paziente--------------------------------

            If filtriRicerca.DataNascitaDa.HasValue Then
                filtri.Append(" and paz_data_nascita >= :dataNascitaDa ")
                cmd.Parameters.AddWithValue("dataNascitaDa", filtriRicerca.DataNascitaDa.Value)
            End If

            If filtriRicerca.DataNascitaA.HasValue Then
                filtri.Append(" and paz_data_nascita <= :dataNascitaA ")
                cmd.Parameters.AddWithValue("dataNascitaA", filtriRicerca.DataNascitaA.Value)
            End If

            If Not String.IsNullOrWhiteSpace(filtriRicerca.Sesso) Then
                filtri.Append(" and paz_sesso = :sesso ")
                cmd.Parameters.AddWithValue("sesso", filtriRicerca.Sesso)
            End If

            If Not String.IsNullOrWhiteSpace(filtriRicerca.CodiceCategoriaRischio) Then
                filtri.Append(" and paz_rsc_codice = :catRischio ")
                cmd.Parameters.AddWithValue("catRischio", filtriRicerca.CodiceCategoriaRischio)
            End If

            If Not String.IsNullOrWhiteSpace(filtriRicerca.CodiceMalattia) Then
                filtri.Append(" and exists (select 1 from t_paz_malattie where pma_paz_codice = paz_codice and pma_mal_codice = :malattia) ")
                cmd.Parameters.AddWithValue("malattia", filtriRicerca.CodiceMalattia)
            End If

            If filtriRicerca.CodiciStatiAnagrafici IsNot Nothing AndAlso filtriRicerca.CodiciStatiAnagrafici.Count > 0 Then
                filtri.Append(String.Format(" and paz_stato_anagrafico in ({0}) ", filtriRicerca.CodiciStatiAnagrafici.GetStringForQuery()))
            End If

            '--------------------------------------------------------------

            '---------------Filtri convocazioni----------------------------

            If filtriRicerca.DataConvocazioneDa.HasValue Then
                filtri.Append(" and cnv_data >= :dataCnvDa ")
                cmd.Parameters.AddWithValue("dataCnvDa", filtriRicerca.DataConvocazioneDa.Value)
            End If

            If filtriRicerca.DataConvocazioneA.HasValue Then
                filtri.Append(" and cnv_data <= :dataCnvA ")
                cmd.Parameters.AddWithValue("dataCnvA", filtriRicerca.DataConvocazioneA.Value)
            End If

            If filtriRicerca.CicliSedute IsNot Nothing Then

                'Cicli
                If filtriRicerca.CicliSedute.CodiceValore IsNot Nothing AndAlso filtriRicerca.CicliSedute.CodiceValore.Count > 0 Then

                    filtri.Append("and ( ")

                    For Each pair As KeyValuePair(Of String, String) In filtriRicerca.CicliSedute.CodiceValore

                        filtri.Append(String.Format("( vpr_cic_codice = '{0}' ", pair.Key))

                        If Not String.IsNullOrWhiteSpace(pair.Value) Then
                            'Numero sedute del ciclo
                            filtri.Append(String.Format(" and vpr_n_seduta in ({0})", pair.Value))
                        End If

                        filtri.Append(") ")

                        If filtriRicerca.CicliSedute.CodiceValore.IndexOf(pair) < filtriRicerca.CicliSedute.CodiceValore.Count - 1 Then
                            filtri.Append(" or ")
                        End If
                    Next

                    filtri.Append(" )")

                End If

                'Numero sedute
                If filtriRicerca.CicliSedute.Valori IsNot Nothing AndAlso filtriRicerca.CicliSedute.Valori.Count > 0 Then
                    filtri.Append(String.Format(" and vpr_n_seduta in ({0}) ", filtriRicerca.CicliSedute.Valori.GetStringForQuery()))
                End If

            End If

            If filtriRicerca.AssociazioniDosi IsNot Nothing Then

                'Associazioni
                If filtriRicerca.AssociazioniDosi.CodiceValore IsNot Nothing AndAlso filtriRicerca.AssociazioniDosi.CodiceValore.Count > 0 Then

                    filtri.Append("and ( ")

                    For Each pair As KeyValuePair(Of String, String) In filtriRicerca.AssociazioniDosi.CodiceValore

                        filtri.Append(String.Format("( vpr_ass_codice = '{0}' ", pair.Key))

                        If Not String.IsNullOrWhiteSpace(pair.Value) Then
                            'Numero dosi dell'associazione
                            filtri.Append(String.Format(" and vpr_n_richiamo in ({0})", pair.Value))
                        End If

                        filtri.Append(") ")

                        If filtriRicerca.AssociazioniDosi.CodiceValore.IndexOf(pair) < filtriRicerca.AssociazioniDosi.CodiceValore.Count - 1 Then
                            filtri.Append(" or ")
                        End If
                    Next

                    filtri.Append(" )")

                End If

                'Numero dosi
                If filtriRicerca.AssociazioniDosi.Valori IsNot Nothing AndAlso filtriRicerca.AssociazioniDosi.Valori.Count > 0 Then
                    filtri.Append(String.Format(" and vpr_n_richiamo in ({0}) ", filtriRicerca.AssociazioniDosi.Valori.GetStringForQuery()))
                End If

            End If

            If filtriRicerca.VaccinazioniDosi IsNot Nothing Then

                'Vaccini
                If filtriRicerca.VaccinazioniDosi.CodiceValore IsNot Nothing AndAlso filtriRicerca.VaccinazioniDosi.CodiceValore.Count > 0 Then

                    filtri.Append("and ( ")

                    For Each pair As KeyValuePair(Of String, String) In filtriRicerca.VaccinazioniDosi.CodiceValore

                        filtri.Append(String.Format("( vpr_vac_codice = '{0}' ", pair.Key))

                        If Not String.IsNullOrWhiteSpace(pair.Value) Then
                            'Numero dosi della vaccinazione
                            filtri.Append(String.Format(" and vpr_n_richiamo in ({0})", pair.Value))
                        End If

                        filtri.Append(") ")

                        If filtriRicerca.VaccinazioniDosi.CodiceValore.IndexOf(pair) < filtriRicerca.VaccinazioniDosi.CodiceValore.Count - 1 Then
                            filtri.Append(" or ")
                        End If
                    Next

                    filtri.Append(" )")

                End If

                'Numero dosi
                If filtriRicerca.VaccinazioniDosi.Valori IsNot Nothing AndAlso filtriRicerca.VaccinazioniDosi.Valori.Count > 0 Then
                    filtri.Append(String.Format(" and vpr_n_richiamo in ({0}) ", filtriRicerca.VaccinazioniDosi.Valori.GetStringForQuery()))
                End If

            End If

            '--------------------------------------------------------------

            Return filtri.ToString()

        End Function

        Private Function GetStringQuery()

            Return "select distinct paz_codice, paz_cognome, paz_nome, paz_data_nascita, cnv_data, cnv_data_appuntamento, " +
                    "trova_vaccinazioni (cnv_paz_codice, cnv_data) as vaccinazioni " +
                    "from t_vac_programmate " +
                    "inner join t_cnv_convocazioni on vpr_paz_codice = cnv_paz_codice and vpr_cnv_data = cnv_data " +
                    "inner join t_paz_pazienti on vpr_paz_codice = paz_codice " +
                    "{0} order by {1}"

        End Function

        Private Function GetCountStringQuery()

            Return "select count(*) from (select distinct paz_codice, cnv_data " +
                    "from t_vac_programmate " +
                    "inner join t_cnv_convocazioni on vpr_paz_codice = cnv_paz_codice and vpr_cnv_data = cnv_data " +
                    "inner join t_paz_pazienti on vpr_paz_codice = paz_codice " +
                    "{0} )"

        End Function

        Private Function GetPrimaryKeysStringQuery()

            Return "select distinct paz_codice, cnv_data " +
                    "from t_vac_programmate " +
                    "inner join t_cnv_convocazioni on vpr_paz_codice = cnv_paz_codice and vpr_cnv_data = cnv_data " +
                    "inner join t_paz_pazienti on vpr_paz_codice = paz_codice " +
                    "{0} "

        End Function

        Private Function CountVaccinazioniProgrammateByCiclo(codicePaziente As Long, dataConvocazione As DateTime, codiceCiclo As String, numSeduta As Integer) As Integer

            Dim count As Integer = 0

            Using cmd As OracleCommand = Me.Connection.CreateCommand()

                cmd.CommandText = "select count(*) " +
                        "from t_vac_programmate " +
                        "where vpr_paz_codice = :codicePaz and vpr_cnv_data = :dataCnv " +
                        "and vpr_cic_codice = :ciclo and vpr_n_seduta = :seduta "

                cmd.Parameters.AddWithValue("codicePaz", codicePaziente)
                cmd.Parameters.AddWithValue("dataCnv", dataConvocazione)
                cmd.Parameters.AddWithValue("ciclo", codiceCiclo)
                cmd.Parameters.AddWithValue("seduta", numSeduta)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        count = Convert.ToInt32(obj)
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

#End Region

    End Class

End Namespace
