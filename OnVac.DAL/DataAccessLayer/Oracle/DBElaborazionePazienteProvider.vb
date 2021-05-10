Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Text

Imports Onit.Database.DataAccessManager

Imports Onit.OnAssistnet.Data.OracleClient


Namespace DAL.Oracle

    Public Class DBElaborazioneProvider
        Inherits DbProvider
        Implements IElaborazionePazienteProvider

#Region " Costruttori "

        Public Sub New(DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " Public "

        Public Function GetElaborazioniVaccinazionePazienteByIDProcesso(idProcessoElaborazione As Long) As System.Collections.Generic.IEnumerable(Of Entities.ElaborazioneVaccinazionePaziente) Implements IElaborazionePazienteProvider.GetElaborazioniVaccinazionePazienteByIDProcesso

            Dim filter As New IElaborazionePazienteProvider.ElaborazioneVaccinazionePazienteFilter()
            filter.IdProcessoAcquisizione = idProcessoElaborazione

            Return Me.GetElaborazioniVaccinazionePaziente(filter, Nothing, "PEV_VES_DATA_EFFETTUAZIONE")

        End Function

        Public Function GetElaborazioniVaccinazionePaziente(filter As IElaborazionePazienteProvider.ElaborazioneVaccinazionePazienteFilter, pagingOptions As Onit.OnAssistnet.Data.PagingOptions) As IEnumerable(Of Entities.ElaborazioneVaccinazionePaziente) Implements IElaborazionePazienteProvider.GetElaborazioniVaccinazionePaziente

            Return Me.GetElaborazioniVaccinazionePaziente(filter, pagingOptions, "PEV_PAZ_CODICE_FISCALE, PEV_PAZ_COGNOME, PEV_PAZ_NOME, PEV_PAZ_SESSO, PEV_PAZ_DATA_NASCITA, COM_DESCRIZIONE_NASCITA, PEV_VES_DATA_EFFETTUAZIONE, PEV_VES_ASS_CODICE, PEV_VES_VAC_CODICE")

        End Function

        Public Function CountElaborazioniVaccinazionePaziente(filter As IElaborazionePazienteProvider.ElaborazioneVaccinazionePazienteFilter) As Long Implements IElaborazionePazienteProvider.CountElaborazioniVaccinazionePaziente

            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim ownConnection As Boolean = False

            Try
                cmd = New OracleClient.OracleCommand()

                cmd.Connection = Connection

                Dim commandText As New System.Text.StringBuilder()

                AddFilterWhereConditions(filter, commandText, cmd.Parameters)

                cmd.CommandText = String.Format("SELECT COUNT(*) FROM T_PAZ_ELAB_VACCINAZIONI{0}{1}", IIf(commandText.Length = 0, String.Empty, " WHERE "), commandText.ToString())

                ownConnection = ConditionalOpenConnection(cmd)

                Return Convert.ToInt64(cmd.ExecuteScalar())

            Finally

                ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

        End Function

        Public Sub UpdateElaborazioneVaccinazionePaziente(ByVal elaborazioneVaccinazionePaziente As Entities.ElaborazioneVaccinazionePaziente) Implements IElaborazionePazienteProvider.UpdateElaborazioneVaccinazionePaziente

            Dim dataElaborazioneTemp As Object = Nothing
            Dim codicePazienteAcquisizioneTemp As Object = Nothing

            If elaborazioneVaccinazionePaziente.DataAcquisizione.HasValue Then
                dataElaborazioneTemp = elaborazioneVaccinazionePaziente.DataAcquisizione.Value
            Else
                dataElaborazioneTemp = DBNull.Value
            End If

            If elaborazioneVaccinazionePaziente.CodicePazienteAcquisizione.HasValue Then
                codicePazienteAcquisizioneTemp = elaborazioneVaccinazionePaziente.CodicePazienteAcquisizione.Value
            Else
                codicePazienteAcquisizioneTemp = DBNull.Value
            End If

            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim ownConnection As Boolean = False

            Try
                cmd = New OracleClient.OracleCommand("UPDATE T_PAZ_ELAB_VACCINAZIONI SET PEV_STATO_ACQUISIZIONE = :PEV_STATO_ACQUISIZIONE, PEV_MESSAGGIO_ACQUISIZIONE = :PEV_MESSAGGIO_ACQUISIZIONE, PEV_DATA_ACQUISIZIONE = :PEV_DATA_ACQUISIZIONE, PEV_PAZ_CODICE_ACQUISIZIONE = :PEV_PAZ_CODICE_ACQUISIZIONE WHERE PEV_ID = :PEV_ID", Me.Connection)

                cmd.Parameters.AddWithValue("PEV_STATO_ACQUISIZIONE", Convert.ToInt32(elaborazioneVaccinazionePaziente.StatoAcquisizione))
                cmd.Parameters.AddWithValue("PEV_MESSAGGIO_ACQUISIZIONE", IIf(String.IsNullOrEmpty(elaborazioneVaccinazionePaziente.MessaggioAcquisizione), DBNull.Value, elaborazioneVaccinazionePaziente.MessaggioAcquisizione))
                cmd.Parameters.AddWithValue("PEV_DATA_ACQUISIZIONE", dataElaborazioneTemp)
                cmd.Parameters.AddWithValue("PEV_PAZ_CODICE_ACQUISIZIONE", codicePazienteAcquisizioneTemp)
                cmd.Parameters.AddWithValue("PEV_ID", elaborazioneVaccinazionePaziente.Id)

                ownConnection = ConditionalOpenConnection(cmd)

                cmd.ExecuteNonQuery()

            Finally

                ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

        End Sub

        Public Function UpdateElaborazioniVaccinazionePaziente(ByVal idProcessoElaborazione As Long, ByVal statoElaborazione As Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente, ByVal statoElaborazionePrecedente As Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente) As Int64 Implements IElaborazionePazienteProvider.UpdateElaborazioniVaccinazionePaziente

            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim ownConnection As Boolean = False

            Try
                cmd = New OracleClient.OracleCommand("UPDATE T_PAZ_ELAB_VACCINAZIONI SET  PEV_PRC_ID_ACQUISIZIONE = :PEV_PRC_ID_ACQUISIZIONE,  PEV_STATO_ACQUISIZIONE = :PEV_STATO_ACQUISIZIONE WHERE PEV_STATO_ACQUISIZIONE = :PEV_STATO_ELAB_PREC", Me.Connection)

                cmd.Parameters.AddWithValue("PEV_PRC_ID_ACQUISIZIONE", idProcessoElaborazione)
                cmd.Parameters.AddWithValue("PEV_STATO_ACQUISIZIONE", Convert.ToInt32(statoElaborazione))
                cmd.Parameters.AddWithValue("PEV_STATO_ELAB_PREC", Convert.ToInt32(statoElaborazionePrecedente))

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Return cmd.ExecuteNonQuery()

            Finally

                ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

        End Function

#End Region

#Region " Private "

        Private Function GetElaborazioniVaccinazionePaziente(filter As IElaborazionePazienteProvider.ElaborazioneVaccinazionePazienteFilter, pagingOptions As Onit.OnAssistnet.Data.PagingOptions, orderByConditions As String) As IEnumerable(Of Entities.ElaborazioneVaccinazionePaziente)

            Dim elaborazioniVaccinazionePaziente As New List(Of ElaborazioneVaccinazionePaziente)

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try
                cmd = New OracleClient.OracleCommand()
                cmd.Connection = Connection

                Dim commandText As New System.Text.StringBuilder()

                AddFilterWhereConditions(filter, commandText, cmd.Parameters)

                cmd.CommandText = String.Format("SELECT T_PAZ_ELAB_VACCINAZIONI.*, T_ANA_COMUNI_NASCITA.COM_DESCRIZIONE COM_DESCRIZIONE_NASCITA, T_ANA_VACCINAZIONI.VAC_DESCRIZIONE, T_ANA_ASSOCIAZIONI.ASS_DESCRIZIONE, T_ANA_OPERATORI.OPE_NOME FROM T_PAZ_ELAB_VACCINAZIONI LEFT OUTER JOIN T_ANA_COMUNI T_ANA_COMUNI_NASCITA ON  PEV_PAZ_COM_CODICE_NASCITA = T_ANA_COMUNI_NASCITA.COM_CODICE INNER JOIN T_ANA_VACCINAZIONI ON  PEV_VES_VAC_CODICE = VAC_CODICE INNER JOIN T_ANA_ASSOCIAZIONI ON  PEV_VES_ASS_CODICE = ASS_CODICE LEFT OUTER JOIN T_ANA_OPERATORI ON PEV_VES_OPE_CODICE = OPE_CODICE{0}{1}{2}{3}", IIf(commandText.Length = 0, String.Empty, " WHERE "), commandText.ToString(), IIf(String.IsNullOrEmpty(orderByConditions), String.Empty, " ORDER BY "), orderByConditions)

                If Not pagingOptions Is Nothing Then
                    cmd.AddPaginatedQuery(pagingOptions)
                End If

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using dataReader As IDataReader = cmd.ExecuteReader()

                    Dim pev_id_ordinal As Int16 = dataReader.GetOrdinal("PEV_ID")
                    Dim pev_paz_codice_ordinal As Int16 = dataReader.GetOrdinal("PEV_PAZ_CODICE")
                    Dim pev_paz_codice_regionale_ordinal As Int16 = dataReader.GetOrdinal("PEV_PAZ_CODICE_REGIONALE")
                    Dim pev_paz_tessera_ordinal As Int16 = dataReader.GetOrdinal("PEV_PAZ_TESSERA")
                    Dim pev_paz_codice_fiscale_ordinal As Int16 = dataReader.GetOrdinal("PEV_PAZ_CODICE_FISCALE")
                    Dim pev_paz_cognome_ordinal As Int16 = dataReader.GetOrdinal("PEV_PAZ_COGNOME")
                    Dim pev_paz_nome_ordinal As Int16 = dataReader.GetOrdinal("PEV_PAZ_NOME")
                    Dim pev_paz_data_nascita_ordinal As Int16 = dataReader.GetOrdinal("PEV_PAZ_DATA_NASCITA")
                    Dim pev_paz_com_codice_nascita_ordinal As Int16 = dataReader.GetOrdinal("PEV_PAZ_COM_CODICE_NASCITA")
                    Dim com_descrizione_nascita_ordinal As Int16 = dataReader.GetOrdinal("COM_DESCRIZIONE_NASCITA")
                    Dim pev_paz_sesso_ordinal As Int16 = dataReader.GetOrdinal("PEV_PAZ_SESSO")
                    Dim pev_ves_vac_codice_ordinal As Int16 = dataReader.GetOrdinal("PEV_VES_VAC_CODICE")
                    Dim vac_descrizione_ordinal As Int16 = dataReader.GetOrdinal("VAC_DESCRIZIONE")
                    Dim pev_ves_ass_codice_ordinal As Int16 = dataReader.GetOrdinal("PEV_VES_ASS_CODICE")
                    Dim ass_descrizione_ordinal As Int16 = dataReader.GetOrdinal("ASS_DESCRIZIONE")
                    Dim pev_ves_data_effettuazione_ordinal As Int16 = dataReader.GetOrdinal("PEV_VES_DATA_EFFETTUAZIONE")
                    Dim pev_ves_ope_codice_ordinal As Int16 = dataReader.GetOrdinal("PEV_VES_OPE_CODICE")
                    Dim ope_nome_ordinal As Int16 = dataReader.GetOrdinal("OPE_NOME")
                    Dim pev_prc_id_elaborazione_ordinal As Int16 = dataReader.GetOrdinal("PEV_PRC_ID_ACQUISIZIONE")
                    Dim pev_stato_elaborazione_ordinal As Int16 = dataReader.GetOrdinal("PEV_STATO_ACQUISIZIONE")
                    Dim pev_data_elaborazione_ordinal As Int16 = dataReader.GetOrdinal("PEV_DATA_ACQUISIZIONE")
                    Dim pev_messaggio_elaborazione_ordinal As Int16 = dataReader.GetOrdinal("PEV_MESSAGGIO_ACQUISIZIONE")
                    Dim pev_paz_codice_acquisizione_ordinal As Int16 = dataReader.GetOrdinal("PEV_PAZ_CODICE_ACQUISIZIONE")
                    'Dim pev_imr_codice_ris_caricamento_ordinal As Int16 = dataReader.GetOrdinal("PEV_IMR_CODICE_RIS_CARICAMENTO")

                    While dataReader.Read()

                        Dim elaborazioneVaccinazionePaziente As New ElaborazioneVaccinazionePaziente()

                        elaborazioneVaccinazionePaziente.Id = dataReader.GetInt64(pev_id_ordinal)

                        If Not dataReader.IsDBNull(pev_paz_codice_ordinal) Then elaborazioneVaccinazionePaziente.CodicePaziente = dataReader.GetInt64(pev_paz_codice_ordinal)
                        If Not dataReader.IsDBNull(pev_paz_codice_regionale_ordinal) Then elaborazioneVaccinazionePaziente.CodiceRegionalePaziente = dataReader.GetString(pev_paz_codice_regionale_ordinal)
                        If Not dataReader.IsDBNull(pev_paz_tessera_ordinal) Then elaborazioneVaccinazionePaziente.TesseraSanitariaPaziente = dataReader.GetString(pev_paz_tessera_ordinal)
                        If Not dataReader.IsDBNull(pev_paz_codice_fiscale_ordinal) Then elaborazioneVaccinazionePaziente.CodiceFiscalePaziente = dataReader.GetString(pev_paz_codice_fiscale_ordinal)
                        If Not dataReader.IsDBNull(pev_paz_cognome_ordinal) Then elaborazioneVaccinazionePaziente.CognomePaziente = dataReader.GetString(pev_paz_cognome_ordinal)
                        If Not dataReader.IsDBNull(pev_paz_nome_ordinal) Then elaborazioneVaccinazionePaziente.NomePaziente = dataReader.GetString(pev_paz_nome_ordinal)
                        If Not dataReader.IsDBNull(pev_paz_data_nascita_ordinal) Then elaborazioneVaccinazionePaziente.DataNascitaPaziente = dataReader.GetDateTime(pev_paz_data_nascita_ordinal)
                        If Not dataReader.IsDBNull(pev_paz_com_codice_nascita_ordinal) Then elaborazioneVaccinazionePaziente.CodiceComuneNascitaPaziente = dataReader.GetString(pev_paz_com_codice_nascita_ordinal)
                        If Not dataReader.IsDBNull(com_descrizione_nascita_ordinal) Then elaborazioneVaccinazionePaziente.DescrizioneComuneNascitaPaziente = dataReader.GetString(com_descrizione_nascita_ordinal)
                        If Not dataReader.IsDBNull(pev_paz_sesso_ordinal) Then elaborazioneVaccinazionePaziente.SessoPaziente = dataReader.GetString(pev_paz_sesso_ordinal)
                        If Not dataReader.IsDBNull(pev_ves_vac_codice_ordinal) Then elaborazioneVaccinazionePaziente.CodiceVaccinazione = dataReader.GetString(pev_ves_vac_codice_ordinal)
                        If Not dataReader.IsDBNull(vac_descrizione_ordinal) Then elaborazioneVaccinazionePaziente.DescrizioneVaccinazione = dataReader.GetString(vac_descrizione_ordinal)
                        If Not dataReader.IsDBNull(pev_ves_ass_codice_ordinal) Then elaborazioneVaccinazionePaziente.CodiceAssociazione = dataReader.GetString(pev_ves_ass_codice_ordinal)
                        If Not dataReader.IsDBNull(ass_descrizione_ordinal) Then elaborazioneVaccinazionePaziente.DescrizioneAssociazione = dataReader.GetString(ass_descrizione_ordinal)
                        If Not dataReader.IsDBNull(pev_ves_data_effettuazione_ordinal) Then elaborazioneVaccinazionePaziente.DataEffettuazione = dataReader.GetDateTime(pev_ves_data_effettuazione_ordinal)
                        If Not dataReader.IsDBNull(pev_ves_ope_codice_ordinal) Then elaborazioneVaccinazionePaziente.CodiceOperatore = dataReader.GetString(pev_ves_ope_codice_ordinal)
                        If Not dataReader.IsDBNull(ope_nome_ordinal) Then elaborazioneVaccinazionePaziente.NomeOperatore = dataReader.GetString(ope_nome_ordinal)
                        If Not dataReader.IsDBNull(pev_prc_id_elaborazione_ordinal) Then elaborazioneVaccinazionePaziente.IdProcessoAcquisizione = dataReader.GetInt64(pev_prc_id_elaborazione_ordinal)
                        If Not dataReader.IsDBNull(pev_stato_elaborazione_ordinal) Then elaborazioneVaccinazionePaziente.StatoAcquisizione = DirectCast([Enum].Parse(GetType(Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente), dataReader.GetInt32(pev_stato_elaborazione_ordinal).ToString()), Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente)
                        If Not dataReader.IsDBNull(pev_data_elaborazione_ordinal) Then elaborazioneVaccinazionePaziente.DataAcquisizione = dataReader.GetDateTime(pev_data_elaborazione_ordinal)
                        If Not dataReader.IsDBNull(pev_messaggio_elaborazione_ordinal) Then elaborazioneVaccinazionePaziente.MessaggioAcquisizione = dataReader.GetString(pev_messaggio_elaborazione_ordinal)
                        If Not dataReader.IsDBNull(pev_paz_codice_acquisizione_ordinal) Then elaborazioneVaccinazionePaziente.CodicePazienteAcquisizione = dataReader.GetInt64(pev_paz_codice_acquisizione_ordinal)

                        elaborazioniVaccinazionePaziente.Add(elaborazioneVaccinazionePaziente)

                    End While

                End Using

            Finally

                ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return elaborazioniVaccinazionePaziente.AsEnumerable()

        End Function

        Private Sub AddFilterWhereConditions(filter As IElaborazionePazienteProvider.ElaborazioneVaccinazionePazienteFilter, commandText As StringBuilder, parameters As OracleClient.OracleParameterCollection)

            If Not filter Is Nothing Then

                If filter.Id.HasValue Then

                    commandText.Append(" PEV_ID = :PEV_ID")

                    parameters.AddWithValue("PEV_ID", filter.Id.Value)

                End If

                If filter.IdProcessoAcquisizione.HasValue Then

                    If commandText.Length > 0 Then commandText.Append(" AND")

                    commandText.Append(" PEV_PRC_ID_ACQUISIZIONE = :PEV_PRC_ID_ACQUISIZIONE")

                    parameters.AddWithValue("PEV_PRC_ID_ACQUISIZIONE", filter.IdProcessoAcquisizione.Value)

                End If

                If filter.StatoAcquisizione.HasValue Then

                    If commandText.Length > 0 Then commandText.Append(" AND")

                    commandText.Append(" PEV_STATO_ACQUISIZIONE = :PEV_STATO_ACQUISIZIONE")

                    parameters.AddWithValue("PEV_STATO_ACQUISIZIONE", Convert.ToInt16(filter.StatoAcquisizione.Value))

                End If

                If filter.DataAcquisizioneDa.HasValue Then

                    If commandText.Length > 0 Then commandText.Append(" AND")

                    commandText.Append(" PEV_DATA_ACQUISIZIONE >= :PEV_DATA_ACQ_DA")

                    parameters.AddWithValue("PEV_DATA_ACQ_DA", filter.DataAcquisizioneDa)

                End If

                If filter.DataAcquisizioneA.HasValue Then

                    If commandText.Length > 0 Then commandText.Append(" AND")

                    commandText.Append(" PEV_DATA_ACQUISIZIONE <= :PEV_DATA_ACQ_A")

                    parameters.AddWithValue("PEV_DATA_ACQ_A", filter.DataAcquisizioneA)

                End If

            End If

        End Sub

#End Region

    End Class

End Namespace
