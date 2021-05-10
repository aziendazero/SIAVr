Imports System.ComponentModel
Imports System.Data.OracleClient
Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager


Namespace DAL.Oracle

    Public Class DbCicliProvider
        Inherits DbProvider
        Implements ICicliProvider

#Region " Constructors "

        Public Sub New(DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " Select "

        ''' <summary>
        ''' Caricamento cicli
        ''' </summary>
        Public Function LoadCicli() As BindingList(Of Ciclo) Implements ICicliProvider.LoadCicli

            Dim listCicli As BindingList(Of Ciclo) = Nothing

            Using cmd As OracleCommand = New OracleClient.OracleCommand(Queries.Cicli.OracleQueries.selCicli, Me.Connection)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim pos_codice As Integer = idr.GetOrdinal("CIC_CODICE")
                            Dim pos_descrizione As Integer = idr.GetOrdinal("CIC_DESCRIZIONE")
                            Dim pos_dataIntroduzione As Integer = idr.GetOrdinal("CIC_DATA_INTRODUZIONE")
                            Dim pos_standard As Integer = idr.GetOrdinal("CIC_STANDARD")
                            Dim pos_nSedute As Integer = idr.GetOrdinal("CIC_N_SEDUTE")
                            Dim pos_eta As Integer = idr.GetOrdinal("CIC_ETA")
                            Dim pos_dataFine As Integer = idr.GetOrdinal("CIC_DATA_FINE")
                            Dim pos_alert As Integer = idr.GetOrdinal("CIC_ALERT")
                            Dim pos_sesso As Integer = idr.GetOrdinal("CIC_SESSO")

                            listCicli = New BindingList(Of Ciclo)()

                            Dim ciclo As Ciclo = Nothing

                            While idr.Read()

                                ciclo = New Ciclo()

                                ciclo.Alert = idr.GetBooleanOrDefault(pos_alert)
                                ciclo.Codice = idr.GetStringOrDefault(pos_codice)
                                ciclo.DataFine = idr.GetDateTimeOrDefault(pos_dataFine)
                                ciclo.DataIntroduzione = idr.GetDateTimeOrDefault(pos_dataIntroduzione)
                                ciclo.Descrizione = idr.GetStringOrDefault(pos_descrizione)
                                ciclo.Eta = idr.GetInt32OrDefault(pos_eta)
                                ciclo.NumSedute = idr.GetInt32OrDefault(pos_nSedute)
                                ciclo.Sesso = idr.GetStringOrDefault(pos_sesso)
                                ciclo.Standard = idr.GetBooleanOrDefault(pos_standard)

                                listCicli.Add(ciclo)

                            End While

                        End If

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return listCicli

        End Function

        ''' <summary>
        ''' Caricamento cicli del paziente
        ''' </summary>
        '''<param name="codicePaziente"></param>
        Public Function LoadCicliPaziente(codicePaziente As Integer) As List(Of CicloPaziente) Implements ICicliProvider.LoadCicliPaziente

            Dim listCicli As New List(Of CicloPaziente)

            Using cmd As OracleCommand = New OracleClient.OracleCommand(Queries.Cicli.OracleQueries.selCicliPaziente, Me.Connection)

                cmd.Parameters.AddWithValue("pazCodice", codicePaziente)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim pac_paz_codice As Integer = idr.GetOrdinal("pac_paz_codice")
                            Dim pac_paz_codice_old As Integer = idr.GetOrdinal("pac_paz_codice_old")
                            Dim pac_cic_codice As Integer = idr.GetOrdinal("pac_cic_codice")
                            Dim cic_descrizione As Integer = idr.GetOrdinal("cic_descrizione")
                            Dim cic_standard As Integer = idr.GetOrdinal("cic_standard")

                            Dim ciclo As CicloPaziente = Nothing

                            While idr.Read()

                                ciclo = New CicloPaziente()

                                ciclo.CodicePaziente = idr.GetDecimal(pac_paz_codice)
                                ciclo.CodicePazienteAlias = idr.GetNullableInt32OrDefault(pac_paz_codice_old)
                                ciclo.CodiceCiclo = idr.GetStringOrDefault(pac_cic_codice)
                                ciclo.DescrizioneCiclo = idr.GetStringOrDefault(cic_descrizione)
                                ciclo.Standard = idr.GetBooleanOrDefault(cic_standard)

                                listCicli.Add(ciclo)

                            End While

                        End If

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return listCicli

        End Function

        '''<summary>Restituisce i cicli standard, in base a età e sesso del paziente specificato.
        '''</summary>
        '''<param name="paziente"></param>
        Public Function GetCodiciCicliStandard(paziente As Paziente) As List(Of String) Implements ICicliProvider.GetCodiciCicliStandard

            Dim listCicli As New List(Of String)()

            Using cmd As OracleCommand = New OracleCommand(Queries.Cicli.OracleQueries.selCicliStandard, Me.Connection)

                cmd.Parameters.AddWithValue("data_nascita", paziente.Data_Nascita)
                cmd.Parameters.AddWithValue("sesso", GetStringParam(paziente.Sesso, False))

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim cic_codice As Integer = idr.GetOrdinal("cic_codice")

                            While idr.Read()

                                listCicli.Add(idr.GetStringOrDefault(cic_codice))

                            End While

                        End If

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return listCicli

        End Function

        '''<summary>Restituisce true se una delle vaccinazioni del ciclo specificato è già presente tra le vaccinazioni dei cicli
        ''' associati al paziente. Restituisce False altrimenti.
        ''' Il controllo avviene sia tra le vaccinazioni che tra le associazioni.
        '''</summary>
        '''<param name="codicePaziente"></param>
        '''<param name="codiceCiclo"></param>
        Public Function ExistsVaccinazioneCicliPaziente(codicePaziente As Integer, codiceCiclo As String) As Boolean Implements ICicliProvider.ExistsVaccinazioneCicliPaziente

            Dim existsVaccinazione As Boolean = False

            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim ownConnection As Boolean = False

            Try

                ' --- Controllo t_ana_vaccinazioni_sedute --- '
                Dim strQuery As String = OnVac.Queries.Cicli.OracleQueries.selExistsVaccinazioniComuni

                cmd = New OracleClient.OracleCommand(strQuery, Me.Connection)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.Parameters.AddWithValue("cod_cic", GetStringParam(codiceCiclo, False))

                Dim obj As Object = cmd.ExecuteScalar()

                If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then

                    existsVaccinazione = True

                Else

                    ' --- Controllo t_ana_associazioni_sedute --- '
                    strQuery = OnVac.Queries.Cicli.OracleQueries.selExistsVaccinazioniComuniAssociazioni

                    cmd.Parameters.Clear()
                    cmd.CommandText = strQuery

                    cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                    cmd.Parameters.AddWithValue("cod_cic", GetStringParam(codiceCiclo, False))

                    obj = cmd.ExecuteScalar()

                    If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then

                        existsVaccinazione = True

                    End If

                End If

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return existsVaccinazione

        End Function

        Public Function ExistsCicliConVaccinazione(codicePaziente As Integer) As Boolean Implements ICicliProvider.ExistsCicliConVaccinazione

            Dim existCicli As Boolean = False

            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim strQuery As String = Queries.Cicli.OracleQueries.selExistsCicloConVaccinazione

            Dim ownConnection As Boolean = False

            Try

                cmd = New OracleClient.OracleCommand(strQuery, Me.Connection)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                cmd.Parameters.AddWithValue("paz_codice", codicePaziente)

                Dim obj As Object = cmd.ExecuteScalar()

                If Not obj Is Nothing AndAlso Not obj Is System.DBNull.Value Then
                    existCicli = True
                Else
                    existCicli = False
                End If

            Catch ex As Exception

                Dim msg As New System.Text.StringBuilder()
                msg.AppendFormat("DAL: Errore ricerca vaccinazioni nei cicli associati al paziente (DbCicliProvider.ExistsCicliConVaccinazione){0}", vbNewLine)
                msg.AppendFormat("Paziente:{0}{1}", codicePaziente, vbNewLine)

                Throw New Exception(msg.ToString, ex)

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return existCicli

        End Function

        Public Function CountCicliIncompatibili(paziente As Paziente) As Integer Implements ICicliProvider.CountCicliIncompatibili

            Dim count As Integer = 0

            Using cmd As OracleCommand = New OracleClient.OracleCommand(String.Empty, Me.Connection)

                cmd.Parameters.AddWithValue("paz_codice", paziente.Paz_Codice)
                cmd.Parameters.AddWithValue("data_nascita", paziente.Data_Nascita)

                If String.IsNullOrEmpty(paziente.Sesso) Then

                    cmd.CommandText = Queries.Cicli.OracleQueries.countCicliIncompatibiliSenzaSesso

                Else

                    cmd.CommandText = Queries.Cicli.OracleQueries.countCicliIncompatibili

                    cmd.Parameters.AddWithValue("sesso", GetStringParam(paziente.Sesso, False))

                End If

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
        ''' Recupero delle informazioni di default relative a sito inoculazione e via di somministrazione in base al ciclo.
        ''' </summary>
        ''' <param name="codiceCiclo"></param>
        ''' <param name="numeroSeduta"></param>
        ''' <param name="codiceVaccinazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetInfoSomministrazioneDefaultCiclo(codiceCiclo As String, numeroSeduta As Integer, codiceVaccinazione As String) As Entities.InfoSomministrazione Implements ICicliProvider.GetInfoSomministrazioneDefaultCiclo

            Dim infoSomministrazione As New Entities.InfoSomministrazione()

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand(OnVac.Queries.Cicli.OracleQueries.selSitoViaByCicloSedutaVaccinazione, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("sas_cic_codice", codiceCiclo)
                    cmd.Parameters.AddWithValue("sas_n_seduta", numeroSeduta)
                    cmd.Parameters.AddWithValue("sas_vac_codice", codiceVaccinazione)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim sas_sii_codice As Integer = idr.GetOrdinal("SAS_SII_CODICE")
                            Dim sii_descrizione As Integer = idr.GetOrdinal("SII_DESCRIZIONE")
                            Dim sas_vii_codice As Integer = idr.GetOrdinal("SAS_VII_CODICE")
                            Dim vii_descrizione As Integer = idr.GetOrdinal("VII_DESCRIZIONE")

                            If idr.Read() Then
                                infoSomministrazione.CodiceSitoInoculazione = idr.GetStringOrDefault(sas_sii_codice)
                                infoSomministrazione.DescrizioneSitoInoculazione = idr.GetStringOrDefault(sii_descrizione)
                                infoSomministrazione.CodiceViaSomministrazione = idr.GetStringOrDefault(sas_vii_codice)
                                infoSomministrazione.DescrizioneViaSomministrazione = idr.GetStringOrDefault(vii_descrizione)
                            End If

                        End If

                    End Using
                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return infoSomministrazione

        End Function

        ''' <summary>
        ''' Restituisce un datatable con le informazioni sui cicli in base alla categoria di rischio specificata
        ''' </summary>
        ''' <param name="codiceCategoriaRischio"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDtCicliByCategoriaRischio(codiceCategoriaRischio As String) As DataTable Implements ICicliProvider.GetDtCicliByCategoriaRischio

            Dim dt As New DataTable()

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand(OnVac.Queries.Cicli.OracleQueries.selCicliByCategoriaRischio, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("rci_rsc_codice", codiceCategoriaRischio)

                    Using ida As New OracleDataAdapter(cmd)
                        ida.Fill(dt)
                    End Using

                End Using

            Finally

                Me.ConditionalCloseConnection(ownConnection)

            End Try

            Return dt

        End Function

        ''' <summary>
        ''' Restituisce la lista dei cicli relativi alla cnv specificata.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataConvocazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCicliConvocazione(codicePaziente As Long, dataConvocazione As DateTime) As List(Of Entities.CicloConvocazione) Implements ICicliProvider.GetCicliConvocazione

            Dim list As List(Of Entities.CicloConvocazione) = Nothing

            Using cmd As OracleCommand = Me.Connection.CreateCommand()

                cmd.CommandText = "select * from t_cnv_cicli where cnc_cnv_paz_codice = :cnc_cnv_paz_codice and cnc_cnv_data = :cnc_cnv_data"

                cmd.Parameters.AddWithValue("cnc_cnv_paz_codice", codicePaziente)
                cmd.Parameters.AddWithValue("cnc_cnv_data", dataConvocazione)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    list = Me.GetCicliConvocazione(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list

        End Function

        ''' <summary>
        ''' Restituisce il ciclo relativo alla convocazione specificata
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataConvocazione"></param>
        ''' <param name="codiceCiclo"></param>
        ''' <param name="numeroSeduta"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCicloConvocazione(codicePaziente As Long, dataConvocazione As DateTime, codiceCiclo As String, numeroSeduta As Integer) As Entities.CicloConvocazione Implements ICicliProvider.GetCicloConvocazione

            Dim ciclo As Entities.CicloConvocazione = Nothing

            Using cmd As OracleCommand = Me.Connection.CreateCommand()

                cmd.CommandText = "select * from t_cnv_cicli where cnc_cnv_paz_codice = :cnc_cnv_paz_codice and cnc_cnv_data = :cnc_cnv_data and cnc_cic_codice = :cnc_cic_codice and cnc_sed_n_seduta = :cnc_sed_n_seduta"

                cmd.Parameters.AddWithValue("cnc_cnv_paz_codice", codicePaziente)
                cmd.Parameters.AddWithValue("cnc_cnv_data", dataConvocazione)
                cmd.Parameters.AddWithValue("cnc_cic_codice", codiceCiclo)
                cmd.Parameters.AddWithValue("cnc_sed_n_seduta", numeroSeduta)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim list As List(Of Entities.CicloConvocazione) = Me.GetCicliConvocazione(cmd)
                    If Not list.IsNullOrEmpty() Then
                        ciclo = list.First()
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return ciclo

        End Function

        Private Function GetCicliConvocazione(cmd As OracleCommand) As List(Of Entities.CicloConvocazione)

            Dim list As New List(Of Entities.CicloConvocazione)()

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    Dim cnc_cnv_paz_codice As Integer = idr.GetOrdinal("CNC_CNV_PAZ_CODICE")
                    Dim cnc_cnv_data As Integer = idr.GetOrdinal("CNC_CNV_DATA")
                    Dim cnc_cic_codice As Integer = idr.GetOrdinal("CNC_CIC_CODICE")
                    Dim cnc_sed_n_seduta As Integer = idr.GetOrdinal("CNC_SED_N_SEDUTA")
                    Dim cnc_cnv_paz_codice_old As Integer = idr.GetOrdinal("CNC_CNV_PAZ_CODICE_OLD")
                    Dim cnc_flag_giorni_posticipo As Integer = idr.GetOrdinal("CNC_FLAG_GIORNI_POSTICIPO")
                    Dim cnc_flag_posticipo_seduta As Integer = idr.GetOrdinal("CNC_FLAG_POSTICIPO_SEDUTA")
                    Dim cnc_n_sollecito As Integer = idr.GetOrdinal("CNC_N_SOLLECITO")
                    Dim cnc_data_invio_sollecito As Integer = idr.GetOrdinal("CNC_DATA_INVIO_SOLLECITO")
                    Dim cnc_data_inserimento As Integer = idr.GetOrdinal("CNC_DATA_INSERIMENTO")
                    Dim cnc_ute_id_inserimento As Integer = idr.GetOrdinal("CNC_UTE_ID_INSERIMENTO")

                    While idr.Read()

                        Dim item As New Entities.CicloConvocazione()
                        item.CodicePaziente = idr.GetInt64(cnc_cnv_paz_codice)
                        item.DataConvocazione = idr.GetDateTime(cnc_cnv_data)
                        item.CodiceCiclo = idr.GetString(cnc_cic_codice)
                        item.NumeroSeduta = idr.GetInt32(cnc_sed_n_seduta)
                        item.CodicePazientePrecedente = idr.GetNullableInt64OrDefault(cnc_cnv_paz_codice_old)
                        item.FlagGiorniPosticipo = idr.GetStringOrDefault(cnc_flag_giorni_posticipo)
                        item.FlagPosticipoSeduta = idr.GetStringOrDefault(cnc_flag_posticipo_seduta)
                        item.NumeroSollecito = idr.GetNullableInt32OrDefault(cnc_n_sollecito)
                        item.DataInvioSollecito = idr.GetNullableDateTimeOrDefault(cnc_data_invio_sollecito)
                        item.DataInserimento = idr.GetNullableDateTimeOrDefault(cnc_data_inserimento)
                        item.IdUtenteInserimento = idr.GetNullableInt64OrDefault(cnc_ute_id_inserimento)

                        list.Add(item)

                    End While

                End If

            End Using

            Return list

        End Function

        ''' <summary>
        ''' Restituisce la data di invio del sollecito specificato
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataConvocazione"></param>
        ''' <param name="codiceCiclo"></param>
        ''' <param name="numeroSeduta"></param>
        ''' <param name="numeroSollecito"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDataInvioSollecito(codicePaziente As Long, dataConvocazione As DateTime, codiceCiclo As String, numeroSeduta As Integer, numeroSollecito As Integer) As DateTime? Implements ICicliProvider.GetDataInvioSollecito

            Dim dataInvio As DateTime? = Nothing

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("CNC_DATA_INVIO_SOLLECITO")
                .AddTables("T_CNV_CICLI")
                .AddWhereCondition("CNC_CNV_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("CNC_CNV_DATA", Comparatori.Uguale, dataConvocazione, DataTypes.Data)
                .AddWhereCondition("CNC_CIC_CODICE", Comparatori.Uguale, codiceCiclo, DataTypes.Stringa)
                .AddWhereCondition("CNC_SED_N_SEDUTA", Comparatori.Uguale, numeroSeduta, DataTypes.Numero)
                .AddWhereCondition("CNC_N_SOLLECITO", Comparatori.Uguale, numeroSollecito, DataTypes.Stringa)
            End With

            Dim obj As Object = _DAM.ExecScalar()
            If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                dataInvio = Convert.ToDateTime(obj)
            End If

            Return dataInvio

        End Function

        ''' <summary>
        ''' Restituisce il codice dei cicli non compatibili con quello indicato, tra i cicli del paziente specificato
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="codiceCiclo"></param>
        ''' <returns></returns>
        Public Function GetCicliIncompatibili(codicePaziente As Long, codiceCiclo As String) As List(Of String) Implements ICicliProvider.GetCicliIncompatibili

            Dim list As New List(Of String)()

            Using cmd As New OracleCommand(Queries.Cicli.OracleQueries.selCicliIncompatibiliPazByCiclo, Connection)

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.Parameters.AddWithValue("cod_cic", codiceCiclo)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            While idr.Read()

                                list.Add(idr.GetString(0))

                            End While

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list

        End Function

        ''' <summary>
        ''' Restituisce true se il ciclo specificato è associato al paziente
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="codiceCiclo"></param>
        ''' <returns></returns>
        Public Function ExistsCicloPaziente(codicePaziente As Long, codiceCiclo As String) As Boolean Implements ICicliProvider.ExistsCicloPaziente

            Using cmd As New OracleCommand("select 1 from t_paz_cicli where pac_paz_codice = :cod_paz and pac_cic_codice = :cod_cic", Connection)

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.Parameters.AddWithValue("cod_cic", codiceCiclo)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj Is Nothing OrElse obj Is DBNull.Value Then
                        Return False
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return True

        End Function

#Region " OnVac API "

        ''' <summary>
        ''' Restituisce l'elenco di vaccinazioni-dosi e le rispettive età in cui verranno effettuate, in base all'età e al sesso specificati.
        ''' </summary>
        ''' <param name="command"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCalendarioVaccinaleStandard(dataNascita As DateTime, sesso As String) As List(Of Entities.CalendarioVaccinaleGenerico) Implements ICicliProvider.GetCalendarioVaccinaleStandard

            Dim listCalendari As New List(Of Entities.CalendarioVaccinaleGenerico)()

            Dim ownConnection As Boolean = False

            Dim query As String =
                "select cic_codice, cic_descrizione, tsd_n_seduta n_seduta, tsd_eta_seduta eta_seduta, " +
                "nvl(sed_vac_codice, sas_vac_codice) vac_codice, nvl(a.vac_descrizione,b.vac_descrizione) vac_descrizione, " +
                "nvl(sed_n_richiamo, sas_n_richiamo) n_richiamo " +
                "from t_ana_cicli " +
                " join t_ana_tempi_sedute on cic_codice = tsd_cic_codice " +
                " left join t_ana_vaccinazioni_sedute on tsd_cic_codice = sed_cic_codice and tsd_n_seduta = sed_n_seduta " +
                " left join t_ana_associazioni_sedute on tsd_cic_codice = sas_cic_codice and tsd_n_seduta = sas_n_seduta " +
                " left join t_ana_vaccinazioni a on sas_vac_codice = a.vac_codice " +
                " left join t_ana_vaccinazioni b on sed_vac_codice = b.vac_codice " +
                "where cic_data_introduzione <= :dataNascita " +
                "and (cic_data_fine >= :dataNascita or cic_data_fine is null) " +
                "and cic_show_in_app = 'S' " +
                "and (cic_sesso = :sesso or cic_sesso = :entrambi) " +
                "order by tsd_cic_codice ASC, tsd_n_seduta ASC "

            Try
                Using cmd As New OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("dataNascita", dataNascita)
                    cmd.Parameters.AddWithValue("sesso", sesso)
                    cmd.Parameters.AddWithValue("entrambi", Constants.TipoSesso.Entrambi)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim calendario As Entities.CalendarioVaccinaleGenerico = Nothing

                            Dim cic_codice As Integer = idr.GetOrdinal("cic_codice")
                            Dim cic_descrizione As Integer = idr.GetOrdinal("cic_descrizione")
                            Dim n_seduta As Integer = idr.GetOrdinal("n_seduta")
                            Dim eta_seduta As Integer = idr.GetOrdinal("eta_seduta")
                            Dim vac_codice As Integer = idr.GetOrdinal("vac_codice")
                            Dim vac_descrizione As Integer = idr.GetOrdinal("vac_descrizione")
                            Dim n_richiamo As Integer = idr.GetOrdinal("n_richiamo")

                            While idr.Read()

                                calendario = New Entities.CalendarioVaccinaleGenerico()
                                calendario.CodiceCiclo = idr.GetString(cic_codice)
                                calendario.DescrizioneCiclo = idr.GetStringOrDefault(cic_descrizione)
                                calendario.SedutaCiclo = idr.GetInt32OrDefault(n_seduta)
                                calendario.EtaSeduta = idr.GetInt32OrDefault(eta_seduta)
                                calendario.CodiceVaccinazione = idr.GetStringOrDefault(vac_codice)
                                calendario.DescrizioneVaccinazione = idr.GetStringOrDefault(vac_descrizione)
                                calendario.DoseVaccinazione = idr.GetInt32OrDefault(n_richiamo)

                                listCalendari.Add(calendario)

                            End While

                        End If

                    End Using

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return listCalendari

        End Function

        ''' <summary>
        ''' Restituisce il calendario vaccinale relativo ai pazienti specificati
        ''' </summary>
        ''' <param name="listCodiciPazienti"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCalendarioVaccinalePazienti(listCodiciPazienti As List(Of Long)) As List(Of Entities.CalendarioVaccinalePaziente) Implements ICicliProvider.GetCalendarioVaccinalePazienti

            Dim listCalendari As New List(Of Entities.CalendarioVaccinalePaziente)()

            Dim ownConnection As Boolean = False

            Dim query As String =
                "select cic_codice, cic_descrizione, tsd_n_seduta n_seduta, tsd_eta_seduta eta_seduta, " +
                "nvl(sed_vac_codice, sas_vac_codice) vac_codice, nvl(a.vac_descrizione, b.vac_descrizione) vac_descrizione, " +
                "nvl(sed_n_richiamo, sas_n_richiamo) n_richiamo, paz_codice, paz_cognome, paz_nome, paz_sesso, paz_data_nascita " +
                "from t_paz_pazienti " +
                " join t_paz_cicli on paz_codice = pac_paz_codice " +
                " join t_ana_cicli on pac_cic_codice = cic_codice " +
                " join t_ana_tempi_sedute on cic_codice = tsd_cic_codice " +
                " left join t_ana_vaccinazioni_sedute on tsd_cic_codice = sed_cic_codice and tsd_n_seduta = sed_n_seduta " +
                " left join t_ana_associazioni_sedute on tsd_cic_codice = sas_cic_codice and tsd_n_seduta = sas_n_seduta " +
                " left join t_ana_vaccinazioni a on sas_vac_codice = a.vac_codice " +
                " left join t_ana_vaccinazioni b on sed_vac_codice = b.vac_codice " +
                "where paz_codice in ({0}) " +
                "order by paz_codice, tsd_cic_codice ASC, tsd_n_seduta ASC"

            Try
                Using cmd As New OracleCommand()

                    cmd.Connection = Me.Connection

                    Dim filtroPazienti As GetInFilterResult = Me.GetInFilter(listCodiciPazienti)

                    cmd.CommandText = String.Format(query, filtroPazienti.InFilter)
                    cmd.Parameters.AddRange(filtroPazienti.Parameters)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim calendario As Entities.CalendarioVaccinalePaziente = Nothing

                            Dim cic_codice As Integer = idr.GetOrdinal("cic_codice")
                            Dim cic_descrizione As Integer = idr.GetOrdinal("cic_descrizione")
                            Dim n_seduta As Integer = idr.GetOrdinal("n_seduta")
                            Dim eta_seduta As Integer = idr.GetOrdinal("eta_seduta")
                            Dim vac_codice As Integer = idr.GetOrdinal("vac_codice")
                            Dim vac_descrizione As Integer = idr.GetOrdinal("vac_descrizione")
                            Dim n_richiamo As Integer = idr.GetOrdinal("n_richiamo")
                            Dim paz_codice As Integer = idr.GetOrdinal("paz_codice")
                            Dim paz_cognome As Integer = idr.GetOrdinal("paz_cognome")
                            Dim paz_nome As Integer = idr.GetOrdinal("paz_nome")
                            Dim paz_sesso As Integer = idr.GetOrdinal("paz_sesso")
                            Dim paz_data_nascita As Integer = idr.GetOrdinal("paz_data_nascita")

                            While idr.Read()

                                calendario = New Entities.CalendarioVaccinalePaziente()
                                calendario.CodiceCiclo = idr.GetString(cic_codice)
                                calendario.DescrizioneCiclo = idr.GetStringOrDefault(cic_descrizione)
                                calendario.SedutaCiclo = idr.GetInt32OrDefault(n_seduta)
                                calendario.EtaSeduta = idr.GetInt32OrDefault(eta_seduta)
                                calendario.CodiceVaccinazione = idr.GetStringOrDefault(vac_codice)
                                calendario.DescrizioneVaccinazione = idr.GetStringOrDefault(vac_descrizione)
                                calendario.DoseVaccinazione = idr.GetInt32OrDefault(n_richiamo)
                                calendario.CodicePaziente = idr.GetInt32OrDefault(paz_codice)
                                calendario.CognomePaziente = idr.GetStringOrDefault(paz_cognome)
                                calendario.NomePaziente = idr.GetStringOrDefault(paz_nome)
                                calendario.SessoPaziente = idr.GetStringOrDefault(paz_sesso)
                                calendario.DataNascitaPaziente = idr.GetDateTimeOrDefault(paz_data_nascita)

                                listCalendari.Add(calendario)

                            End While
                        End If
                    End Using
                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return listCalendari

        End Function

#End Region

#End Region

#Region " Insert/Delete "

        '''<summary>Cancellazione ciclo per il paziente specificato
        '''</summary>
        '''<param name="codicePaziente"></param>
        '''<param name="codiceCiclo"></param>
        Public Function DeleteCicloPaziente(codicePaziente As Integer, codiceCiclo As String) As Integer Implements ICicliProvider.DeleteCicloPaziente

            Dim count As Integer = 0

            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim ownConnection As Boolean = False

            Try

                cmd = New OracleClient.OracleCommand(Queries.Cicli.OracleQueries.delCicloPaziente, Me.Connection)

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.Parameters.AddWithValue("cod_cic", codiceCiclo)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                count = cmd.ExecuteNonQuery()

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return count

        End Function

        ''' <summary>
        ''' Cancella i cicli specificati al paziente 
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="codiciCicli"></param>
        ''' <returns></returns>
        Public Function DeleteCicliPaziente(codicePaziente As Integer, codiciCicli As List(Of String)) As Integer Implements ICicliProvider.DeleteCicliPaziente

            Dim count As Integer = 0

            Using cmd As OracleCommand = Connection.CreateCommand()

                Dim ownConnection As Boolean = False

                Try
                    Dim query As String = "delete from t_paz_cicli where pac_paz_codice = :cod_paz and pac_cic_codice {0}"

                    cmd.Parameters.AddWithValue("cod_paz", codicePaziente)

                    If codiciCicli.Count = 1 Then

                        cmd.CommandText = String.Format(query, " = :pac_cic_codice ")
                        cmd.Parameters.AddWithValue("pac_cic_codice", codiciCicli.Single())

                    Else

                        Dim filtroCicli As GetInFilterResult = GetInFilter(codiciCicli)

                        cmd.CommandText = String.Format(query, " IN ( " + filtroCicli.InFilter + ")")
                        cmd.Parameters.AddRange(filtroCicli.Parameters)

                    End If

                    ownConnection = ConditionalOpenConnection(cmd)

                    count = cmd.ExecuteNonQuery()

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        '''<summary>
        ''' Cancellazione di tutti i cicli del paziente specificato
        '''</summary>
        '''<param name="codicePaziente"></param>
        Public Function DeleteCicliPaziente(codicePaziente As Integer) As Integer Implements ICicliProvider.DeleteCicliPaziente

            With _DAM.QB
                .NewQuery()
                .AddTables("T_PAZ_CICLI")
                .AddWhereCondition("PAC_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Delete)

        End Function

        '''<summary>
        ''' Inserimento ciclo per il paziente specificato
        '''</summary>
        '''<param name="codicePaziente"></param>
        '''<param name="cod_ciclo"></param>
        Public Function InsertCicloPaziente(codicePaziente As Integer, codiceCiclo As String) As Integer Implements ICicliProvider.InsertCicloPaziente

            Dim count As Integer = 0

            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim ownConnection As Boolean = False

            Try
                cmd = New OracleClient.OracleCommand(Queries.Cicli.OracleQueries.insCicloPaziente, Me.Connection)

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.Parameters.AddWithValue("cod_cic", codiceCiclo)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                count = cmd.ExecuteNonQuery()

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return count

        End Function

#End Region

    End Class

End Namespace
