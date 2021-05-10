Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.DAL


Public Class DbConfigurazioneCertificatoProvider
    Inherits DbProvider
    Implements IConfigurazioneCertificatoProvider

#Region " Costruttori "

    Public Sub New(ByRef DAM As IDAM)

        MyBase.New(DAM)

    End Sub

#End Region

#Region " IConfigurazioneCertificatoProvider "


    ''' <summary>
    ''' Funzione che restituisce dati della configurazione dei certificati
    ''' </summary>
    ''' <param name="id">indica id un certificato.Se 0 recupera tutti i certificati</param>
    ''' <returns></returns>
    Public Function GetConfigurazioneCertificato(id As Integer) As IEnumerable(Of ConfigurazioneCertificazione) Implements IConfigurazioneCertificatoProvider.GetConfigurazioneCertificato
        Dim query As String = ""
        If id = 0 Then
            query = Queries.ConfigurazioneCertificato.OracleQueries.selConfigurazioneControlli
        Else
            query = Queries.ConfigurazioneCertificato.OracleQueries.selConfigurazioneControllo
        End If

        Using cmd As New OracleClient.OracleCommand(query, Connection)

            If id > 0 Then
                cmd.Parameters.AddWithValue("coc_id", id)
            End If
            Dim ownConnection As Boolean = False

            Try

                ownConnection = ConditionalOpenConnection(cmd)

                Using dataReader As IDataReader = cmd.ExecuteReader()

                    Return GetConfigurazioniFromReader(dataReader)

                End Using

            Finally

                ConditionalCloseConnection(ownConnection)

            End Try

        End Using

    End Function

    ''' <summary>
    ''' Recupera la descrizione della scritta del report per certificato dei vaccini.
    ''' </summary>
    ''' <param name="idPaz">id del paziente </param>
    ''' <returns></returns>
    Public Function GetScrittaCertificato(idPaz As String) As String Implements IConfigurazioneCertificatoProvider.GetScrittaCertificato
        Dim query As String = ""
        Dim retScritta As String = String.Empty

        query = Queries.ConfigurazioneCertificato.OracleQueries.selScrittaCetrificato

        Using cmd As New OracleClient.OracleCommand(query, Connection)


            cmd.Parameters.AddWithValue("psc_paz_codice", idPaz)

            Dim ownConnection As Boolean = False

            Try

                ownConnection = ConditionalOpenConnection(cmd)

                Using dataReader As IDataReader = cmd.ExecuteReader()

                    If Not dataReader Is Nothing Then
                        Dim scritta As Int32 = dataReader.GetOrdinal("psc_scritta_certificato")
                        If dataReader.Read() Then
                            If Not dataReader.IsDBNull(scritta) Then retScritta = dataReader.GetString(scritta)
                        End If
                    End If
                End Using

            Finally

                ConditionalCloseConnection(ownConnection)

            End Try

        End Using
        Return retScritta
    End Function

    ''' <summary>
    ''' Inserimento configurazione certificato
    ''' </summary>
    ''' <param name="indirizzoPaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function InsertConfigurazioneCertificato(configurazione As Entities.ConfigurazioneCertificazione) As Integer Implements IConfigurazioneCertificatoProvider.InsertConfigurazioneCertificato

        Dim idConfigurazione As Integer = 0

        Using cmd As New OracleClient.OracleCommand()

            cmd.Connection = Me.Connection

            Dim ownConnection As Boolean = False

            Try
                ownConnection = Me.ConditionalOpenConnection(cmd)

                ' Sequence per il id configurazione
                cmd.CommandText = OnVac.Queries.ConfigurazioneCertificato.OracleQueries.selNextSeqConfigurazioneCertificato
                idConfigurazione = cmd.ExecuteScalar()

                ' Inserimento
                cmd.CommandText = OnVac.Queries.ConfigurazioneCertificato.OracleQueries.insConfigurazione

                cmd.Parameters.Clear()
                cmd.Parameters.AddWithValue("coc_id", idConfigurazione)
                Me.SetParametersInsertUpdateConfigurazione(configurazione, False, cmd)

                cmd.ExecuteNonQuery()

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

        End Using

        Return idConfigurazione

    End Function

    ''' <summary>
    ''' Aggiornamento Configurazioni
    ''' </summary>
    ''' <param name="configurazione"></param>
    Public Sub UpdateConfigurazioneCertificato(configurazione As ConfigurazioneCertificazione) Implements IConfigurazioneCertificatoProvider.UpdateConfigurazioneCertificato

        Using cmd As New OracleClient.OracleCommand(OnVac.Queries.ConfigurazioneCertificato.OracleQueries.updConfigurazione, Me.Connection)

            Dim ownConnection As Boolean = False

            Try
                ownConnection = Me.ConditionalOpenConnection(cmd)

                cmd.Parameters.Clear()
                Me.SetParametersInsertUpdateConfigurazione(configurazione, True, cmd)

                cmd.ExecuteNonQuery()

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

        End Using

    End Sub

    ''' <summary>
    ''' Conacellazione Configurazioni
    ''' </summary>
    ''' <param name="idConfigurazione"></param>
    ''' <returns></returns>
    Public Function DeleteConfigurazioneCertificato(idConfigurazione As Integer) As Integer Implements IConfigurazioneCertificatoProvider.DeleteConfigurazioneCertificato

        Dim count As Integer = 0

        Using cmd As New OracleClient.OracleCommand(Queries.ConfigurazioneCertificato.OracleQueries.delConfigurazione, Me.Connection)

            Dim ownConnection As Boolean = False

            Try
                ownConnection = Me.ConditionalOpenConnection(cmd)

                cmd.Parameters.AddWithValue("coc_id", idConfigurazione)

                count = cmd.ExecuteNonQuery()

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

        End Using

        Return count

    End Function
#Region " Link configurazione certificati vaccini"
    Public Function GetListVacciniConfigurazione(idConfigurazione As Integer) As IEnumerable(Of ConfigurazioneCertificazioneVaccinazioni) Implements IConfigurazioneCertificatoProvider.GetListVacciniConfigurazione
        Dim query As String = ""

        query = Queries.ConfigurazioneCertificato.OracleQueries.selLinkVaccConfig

        Using cmd As New OracleClient.OracleCommand(query, Connection)


            cmd.Parameters.AddWithValue("cov_coc_id", idConfigurazione)

            Dim ownConnection As Boolean = False

            Try

                ownConnection = ConditionalOpenConnection(cmd)

                Using dataReader As IDataReader = cmd.ExecuteReader()

                    Return GetLinkConfVaccFromReader(dataReader)

                End Using

            Finally

                ConditionalCloseConnection(ownConnection)

            End Try

        End Using

    End Function

    Public Function InsertConfigurazioneCertificatoVaccini(configurazioneVaccini As ConfigurazioneCertificazioneVaccinazioni) As Boolean Implements IConfigurazioneCertificatoProvider.InsertConfigurazioneCertificatoVaccini

        Dim OK As Boolean = True

        Using cmd As New OracleClient.OracleCommand()

            cmd.Connection = Me.Connection

            Dim ownConnection As Boolean = False


            Try
                ownConnection = Me.ConditionalOpenConnection(cmd)



                ' Inserimento
                cmd.CommandText = OnVac.Queries.ConfigurazioneCertificato.OracleQueries.insConfigurazioneVacciniDosi

                cmd.Parameters.Clear()

                Me.SetParametersInsertUpdateConfigurazioneVaccini(configurazioneVaccini, True, cmd)

                cmd.ExecuteNonQuery()
            Catch ex As Exception
                OK = False
            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

        End Using

        Return OK

    End Function
    Public Function DeleteConfigurazioneCertVaccini(idConfigurazione As Integer) As Integer Implements IConfigurazioneCertificatoProvider.DeleteConfigurazioneCertVaccini
        Dim count As Integer = 0

        Using cmd As New OracleClient.OracleCommand(Queries.ConfigurazioneCertificato.OracleQueries.delConfigurazioneVacciniDosi, Me.Connection)

            Dim ownConnection As Boolean = False

            Try
                ownConnection = Me.ConditionalOpenConnection(cmd)

                cmd.Parameters.AddWithValue("cov_coc_id", idConfigurazione)

                count = cmd.ExecuteNonQuery()

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

        End Using

        Return count
    End Function

#End Region

#End Region

#Region " Private "

    Private Function GetConfigurazioniFromReader(dataReader As IDataReader) As IEnumerable(Of ConfigurazioneCertificazione)

        Dim COC_ID As Int32 = dataReader.GetOrdinal("COC_ID")
        Dim COC_DATA_NASCITA_DA As Int32 = dataReader.GetOrdinal("COC_DATA_NASCITA_DA")
        Dim COC_DATA_NASCITA_A As Int32 = dataReader.GetOrdinal("COC_DATA_NASCITA_A")
        Dim COC_ETA_ANNO_DA As Int32 = dataReader.GetOrdinal("COC_ETA_ANNO_DA")
        Dim COC_ETA_MESE_DA As Int32 = dataReader.GetOrdinal("COC_ETA_MESE_DA")
        Dim COC_ETA_GIORNO_DA As Int32 = dataReader.GetOrdinal("COC_ETA_GIORNO_DA")
        Dim COC_ETA_ANNO_A As Int32 = dataReader.GetOrdinal("COC_ETA_ANNO_A")
        Dim COC_ETA_MESE_A As Int32 = dataReader.GetOrdinal("COC_ETA_MESE_A")
        Dim COC_ETA_GIORNO_A As Int32 = dataReader.GetOrdinal("COC_ETA_GIORNO_A")
        Dim COC_SESSO As Int32 = dataReader.GetOrdinal("COC_SESSO")
        Dim COC_DATA_CONTROLLO As Int32 = dataReader.GetOrdinal("COC_DATA_CONTROLLO")
        Dim COC_MOT_CODICE_IMMUNITA As Int32 = dataReader.GetOrdinal("COC_MOT_CODICE_IMMUNITA")
        Dim COC_MOT_CODICE_ESONERO As Int32 = dataReader.GetOrdinal("COC_MOT_CODICE_ESONERO")
        Dim COC_TESTO_POSITIVO As Int32 = dataReader.GetOrdinal("COC_TESTO_POSITIVO")
        Dim COC_TESTO_NEGATIVO As Int32 = dataReader.GetOrdinal("COC_TESTO_NEGATIVO")
        Dim COC_TESTO_PARZIALE As Int32 = dataReader.GetOrdinal("COC_TESTO_PARZIALE")
        Dim COC_FLAG_CHECK_APPUNTAMENTI As Int32 = dataReader.GetOrdinal("COC_FLAG_CHECK_APPUNTAMENTI")
        Dim COC_TIPO_CHECK_APPUNTAMENTI As Int32 = dataReader.GetOrdinal("COC_TIPO_CHECK_APPUNTAMENTI")

        Dim config As New List(Of ConfigurazioneCertificazione)

        While dataReader.Read()

            Dim conf As New ConfigurazioneCertificazione()

            conf.Id = dataReader.GetInt32(COC_ID)
            conf.DataNascitaDa = dataReader.GetNullableDateTimeOrDefault(COC_DATA_NASCITA_DA)
            conf.DataNascitaA = dataReader.GetNullableDateTimeOrDefault(COC_DATA_NASCITA_A)
            conf.EtaAnnoDa = dataReader.GetInt32(COC_ETA_ANNO_DA)
            conf.EtaMeseDa = dataReader.GetInt32(COC_ETA_MESE_DA)
            conf.EtaGiornoDa = dataReader.GetInt32(COC_ETA_GIORNO_DA)
            conf.EtaAnnoA = dataReader.GetInt32(COC_ETA_ANNO_A)
            conf.EtaMeseA = dataReader.GetInt32(COC_ETA_MESE_A)
            conf.EtaGiornoA = dataReader.GetInt32(COC_ETA_GIORNO_A)
            conf.Sesso = dataReader.GetString(COC_SESSO)
            conf.DataControllo = dataReader.GetNullableDateTimeOrDefault(COC_DATA_CONTROLLO)
            conf.CodiceMotiviImmunita = dataReader.GetStringOrDefault(COC_MOT_CODICE_IMMUNITA)
            conf.CodiceMotiviEsonero = dataReader.GetStringOrDefault(COC_MOT_CODICE_ESONERO)
            conf.TestoPositivo = dataReader.GetStringOrDefault(COC_TESTO_POSITIVO)
            conf.TestoNegativo = dataReader.GetStringOrDefault(COC_TESTO_NEGATIVO)
            conf.TestoParziale = dataReader.GetStringOrDefault(COC_TESTO_PARZIALE)
            conf.FlgCheckAppuntamenti = dataReader.GetStringOrDefault(COC_FLAG_CHECK_APPUNTAMENTI)
            conf.TipoCheckAppuntamenti = dataReader.GetStringOrDefault(COC_TIPO_CHECK_APPUNTAMENTI)

            config.Add(conf)

        End While

        Return config.AsEnumerable()

    End Function


    Private Function GetLinkConfVaccFromReader(dataReader As IDataReader) As IEnumerable(Of ConfigurazioneCertificazioneVaccinazioni)

        Dim COV_COC_ID As Int32 = dataReader.GetOrdinal("COV_COC_ID")
        Dim COV_VAC_CODICE As Int32 = dataReader.GetOrdinal("COV_VAC_CODICE")
        Dim COV_N_DOSE As Int32 = dataReader.GetOrdinal("COV_N_DOSE")
        Dim VAC_DESCRIZIONE As Int32 = dataReader.GetOrdinal("VAC_DESCRIZIONE")


        Dim linkconfigvacc As New List(Of ConfigurazioneCertificazioneVaccinazioni)

        While dataReader.Read()

            Dim linkconf As New ConfigurazioneCertificazioneVaccinazioni()

            linkconf.IdConfigurazioneCertificato = dataReader.GetInt32(COV_COC_ID)
            linkconf.CodiceVaccino = dataReader.GetString(COV_VAC_CODICE)
            linkconf.NumeroDose = dataReader.GetInt32(COV_N_DOSE)
            linkconf.DescVaccini = dataReader.GetString(VAC_DESCRIZIONE)


            linkconfigvacc.Add(linkconf)

        End While

        Return linkconfigvacc.AsEnumerable()

    End Function
    Private Sub SetParametersInsertUpdateConfigurazione(configurazione As ConfigurazioneCertificazione, setCodeParameter As Boolean, cmd As OracleClient.OracleCommand)

        If setCodeParameter Then cmd.Parameters.AddWithValue("coc_id", configurazione.Id)

        cmd.Parameters.AddWithValue("coc_data_nascita_da", GetDateParam(configurazione.DataNascitaDa))
        cmd.Parameters.AddWithValue("coc_data_nascita_a", GetDateParam(configurazione.DataNascitaA))
        cmd.Parameters.AddWithValue("coc_eta_mese_da", GetIntParam(configurazione.EtaMeseDa))
        cmd.Parameters.AddWithValue("coc_eta_anno_da", GetIntParam(configurazione.EtaAnnoDa))
        cmd.Parameters.AddWithValue("coc_eta_mese_da", GetIntParam(configurazione.EtaMeseDa))
        cmd.Parameters.AddWithValue("coc_eta_giorno_da", GetIntParam(configurazione.EtaGiornoDa))
        cmd.Parameters.AddWithValue("coc_eta_anno_a", GetIntParam(configurazione.EtaAnnoA))
        cmd.Parameters.AddWithValue("coc_eta_mese_a", GetIntParam(configurazione.EtaMeseA))
        cmd.Parameters.AddWithValue("coc_eta_giorno_a", GetIntParam(configurazione.EtaGiornoA))
        cmd.Parameters.AddWithValue("coc_sesso", GetStringParam(configurazione.Sesso))
        cmd.Parameters.AddWithValue("coc_data_controllo", GetDateParam(configurazione.DataControllo))
        cmd.Parameters.AddWithValue("coc_mot_codice_immunita", GetStringParam(configurazione.CodiceMotiviImmunita))
        cmd.Parameters.AddWithValue("coc_mot_codice_esonero", GetStringParam(configurazione.CodiceMotiviEsonero))
        cmd.Parameters.AddWithValue("coc_testo_positivo", GetStringParam(configurazione.TestoPositivo))
        cmd.Parameters.AddWithValue("coc_testo_negativo", GetStringParam(configurazione.TestoNegativo))
        cmd.Parameters.AddWithValue("coc_testo_parziale", GetStringParam(configurazione.TestoParziale))
        cmd.Parameters.AddWithValue("coc_flag_check_appuntamenti", GetStringParam(configurazione.FlgCheckAppuntamenti))
        cmd.Parameters.AddWithValue("coc_tipo_check_appuntamenti", GetStringParam(configurazione.TipoCheckAppuntamenti))

    End Sub

    Private Sub SetParametersInsertUpdateConfigurazioneVaccini(configurazioneVaccini As ConfigurazioneCertificazioneVaccinazioni, setCodeParameter As Boolean, cmd As OracleClient.OracleCommand)
        cmd.Parameters.AddWithValue("cov_coc_id", GetIntParam(configurazioneVaccini.IdConfigurazioneCertificato))
        cmd.Parameters.AddWithValue("cov_vac_codice", GetStringParam(configurazioneVaccini.CodiceVaccino))
        cmd.Parameters.AddWithValue("cov_n_dose", GetIntParam(configurazioneVaccini.NumeroDose))
    End Sub

#End Region

End Class


