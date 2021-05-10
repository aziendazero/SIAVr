Imports System.Collections.Generic
Imports System.Data.OracleClient
Imports Onit.Database.DataAccessManager


Namespace DAL.Oracle

    Public Class DBVaccinazioniEseguiteProvider
        Inherits DbProvider
        Implements IVaccinazioniEseguiteProvider

#Region " Private variables "

        Private ci As System.Globalization.CultureInfo = System.Globalization.CultureInfo.InvariantCulture
        Private ArgomentoLog As String

#End Region

#Region " Constructors "

        Public Sub New(DAM As IDAM)

            MyBase.New(DAM)

            Me.ArgomentoLog = Log.DataLogStructure.TipiArgomento.VAC_ESEGUITE

        End Sub

#End Region

#Region " Public Methods "

#Region "Vaccinazioni paziente per date"
        Public Function GetVaccinazioniDosePaziente(idPaziente As Long, dataInizio As Date?, dataFine As Date?) As List(Of Entities.VaccinazioneDose) Implements IVaccinazioniEseguiteProvider.GetVaccinazioniDosePaziente

            Dim listVacDoseEseguite As New List(Of Entities.VaccinazioneDose)

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                Dim query As New System.Text.StringBuilder()

                query.AppendFormat("SELECT VES_VAC_CODICE, VES_N_RICHIAMO FROM T_VAC_ESEGUITE ")

                ' Filtro paziente
                query.Append(" WHERE VES_PAZ_CODICE = :VES_PAZ_CODICE ")
                cmd.Parameters.AddWithValue("VES_PAZ_CODICE", idPaziente)

                '' Filtro visibilità
                'query.Append(Me.GetFiltroVisibilita(listVisibilita, cmd))

                If dataInizio.HasValue Then

                    ' Filtro reazione presente nella vaccinazione
                    query.Append(" AND VES_DATA_EFFETTUAZIONE >= :VES_DATA_EFFETTUAZIONE")
                    cmd.Parameters.AddWithValue("VES_DATA_EFFETTUAZIONE", dataInizio.Value)

                End If

                If dataFine.HasValue Then

                    ' Filtro reazione presente nella vaccinazione
                    query.Append(" AND VES_DATA_EFFETTUAZIONE <= :VES_DATA_EFFETTUAZIONE_FINE")
                    cmd.Parameters.AddWithValue("VES_DATA_EFFETTUAZIONE_FINE", dataFine.Value)

                End If
                query.Append(" order by VES_VAC_CODICE, VES_N_RICHIAMO desc")

                cmd.CommandText = query.ToString()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        listVacDoseEseguite = GetVaccinazioneEseguiteDose(idr)

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try
            End Using

            Return listVacDoseEseguite

        End Function
#End Region



#Region " Salvataggi "

        Public Function InsertVaccinazioneEseguita(vaccinazioneEseguita As VaccinazioneEseguita) As Boolean Implements IVaccinazioniEseguiteProvider.InsertVaccinazioneEseguita
            Return Me.InsertVaccinazioneEseguita(vaccinazioneEseguita, "T_VAC_ESEGUITE", "VES", False)
        End Function

        Public Function InsertVaccinazioneEseguitaScaduta(vaccinazioneEseguitaScaduta As VaccinazioneEseguita) As Boolean Implements IVaccinazioniEseguiteProvider.InsertVaccinazioneEseguitaScaduta
            Return Me.InsertVaccinazioneEseguita(vaccinazioneEseguitaScaduta, "T_VAC_SCADUTE", "VSC", False)
        End Function

        Public Function InsertVaccinazioneEseguitaEliminata(vaccinazioneEseguitaEliminata As VaccinazioneEseguita) As Boolean Implements IVaccinazioniEseguiteProvider.InsertVaccinazioneEseguitaEliminata
            Return Me.InsertVaccinazioneEseguita(vaccinazioneEseguitaEliminata, "T_VAC_ESEGUITE_ELIMINATE", "VEE", True)
        End Function

        Public Function InsertVaccinazioneEseguitaScadutaEliminata(vaccinazioneEseguitaScadutaEliminata As VaccinazioneEseguita) As Boolean Implements IVaccinazioniEseguiteProvider.InsertVaccinazioneEseguitaScadutaEliminata
            Return Me.InsertVaccinazioneEseguita(vaccinazioneEseguitaScadutaEliminata, "T_VAC_SCADUTE_ELIMINATE", "VSE", False)
        End Function

        Public Function UpdateVaccinazioneEseguita(vaccinazioneEseguita As Entities.VaccinazioneEseguita) As Boolean Implements IVaccinazioniEseguiteProvider.UpdateVaccinazioneEseguita
            Return Me.UpdateVaccinazioneEseguita(vaccinazioneEseguita, "T_VAC_ESEGUITE", "VES")
        End Function

        Public Function UpdateVaccinazioneEseguitaScaduta(vaccinazioneEseguitaScaduta As Entities.VaccinazioneEseguita) As Boolean Implements IVaccinazioniEseguiteProvider.UpdateVaccinazioneEseguitaScaduta
            Return Me.UpdateVaccinazioneEseguita(vaccinazioneEseguitaScaduta, "T_VAC_SCADUTE", "VSC")
        End Function

        Public Function DeleteVaccinazioneEseguitaById(idVaccinazioneEseguita As Long) As Boolean Implements IVaccinazioniEseguiteProvider.DeleteVaccinazioneEseguitaById

            With _DAM.QB

                .NewQuery()
                .AddTables("T_VAC_ESEGUITE")
                .AddWhereCondition("VES_ID", Comparatori.Uguale, idVaccinazioneEseguita, DataTypes.Numero)

            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Delete)

        End Function

        Public Function DeleteVaccinazioneEseguitaScadutaById(idVaccinazioneEseguita As Long) As Boolean Implements IVaccinazioniEseguiteProvider.DeleteVaccinazioneEseguitaScadutaById

            With _DAM.QB

                .NewQuery()
                .AddTables("T_VAC_SCADUTE")
                .AddWhereCondition("VSC_ID", Comparatori.Uguale, idVaccinazioneEseguita, DataTypes.Numero)

            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Delete)

        End Function

#Region " Reazione Avversa "

        Public Function InsertReazioneAvversa(reazioneAvversa As ReazioneAvversa) As Boolean Implements IVaccinazioniEseguiteProvider.InsertReazioneAvversa
            Return Me.InsertReazioneAvversa(reazioneAvversa, "T_VAC_REAZIONI_AVVERSE", "VRA", "VES", False)
        End Function

        Public Function InsertReazioneAvversaEliminata(reazioneAvversa As Entities.ReazioneAvversa) As Boolean Implements IVaccinazioniEseguiteProvider.InsertReazioneAvversaEliminata
            Return Me.InsertReazioneAvversa(reazioneAvversa, "T_VAC_REAZIONI_AVV_ELIMINATE", "VRE", "VES", True)
        End Function

        Public Function InsertReazioneAvversaScaduta(reazioneAvversa As Entities.ReazioneAvversa) As Boolean Implements IVaccinazioniEseguiteProvider.InsertReazioneAvversaScaduta
            Return Me.InsertReazioneAvversa(reazioneAvversa, "T_VAC_REAZIONI_SCADUTE", "VRS", "VSC", False)
        End Function
        Public Function InsertLinkReazLogInvio(idReazioneAvversa As Long, idLogInvio As Long) As Integer Implements IVaccinazioniEseguiteProvider.InsertLinkReazLogInvio
            Dim count As Integer = 0

            Using cmd As New OracleCommand("INSERT INTO T_ANA_LINK_REAZ_LOG_INVIO (LRL_ID_REAZIONI_AVVERSE, LRL_ID_LOG_NOTIFICHE_INVIO) VALUES (:LRL_ID_REAZIONI_AVVERSE, :LRL_ID_LOG_NOTIFICHE_INVIO)", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("LRL_ID_REAZIONI_AVVERSE", idReazioneAvversa)
                    cmd.Parameters.AddWithValue("LRL_ID_LOG_NOTIFICHE_INVIO", idLogInvio)


                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count
        End Function

        Public Function InsertReazioneAvversaScadutaEliminata(reazioneAvversa As Entities.ReazioneAvversa) As Boolean Implements IVaccinazioniEseguiteProvider.InsertReazioneAvversaScadutaEliminata
            Return Me.InsertReazioneAvversa(reazioneAvversa, "T_VAC_REAZIONI_SCAD_ELIMINATE", "VRL", "VSC", True)
        End Function

        Public Function UpdateReazioneAvversa(reazioneAvversa As ReazioneAvversa) As Boolean Implements IVaccinazioniEseguiteProvider.UpdateReazioneAvversa
            Return Me.UpdateReazioneAvversa(reazioneAvversa, "T_VAC_REAZIONI_AVVERSE", "VRA", "VES")
        End Function

        Public Function UpdateReazioneAvversaScaduta(reazioneAvversa As Entities.ReazioneAvversa) As Boolean Implements IVaccinazioniEseguiteProvider.UpdateReazioneAvversaScaduta
            Return Me.UpdateReazioneAvversa(reazioneAvversa, "T_VAC_REAZIONI_SCADUTE", "VRS", "VSC")
        End Function
        Public Function UpdateReazioneAvversaIdscheda(reazioneAvversa As Entities.ReazioneAvversa) As Boolean Implements IVaccinazioniEseguiteProvider.UpdateReazioneAvversaIdScheda
            Return Me.UpdateReazioneAvversaIdSchedaPrivate(reazioneAvversa, "T_VAC_REAZIONI_AVVERSE", "VRA")
        End Function
        Public Function UpdateReazioneAvversaScadutaIdscheda(reazioneAvversa As Entities.ReazioneAvversa) As Boolean Implements IVaccinazioniEseguiteProvider.UpdateReazioneAvversaScadutaIdScheda
            Return Me.UpdateReazioneAvversaIdSchedaPrivate(reazioneAvversa, "T_VAC_REAZIONI_SCADUTE", "VRS")
        End Function

        Public Function DeleteReazioneAvversaById(idReazioneAvversa As Long) As Boolean Implements IVaccinazioniEseguiteProvider.DeleteReazioneAvversaById

            With _DAM.QB

                .NewQuery()
                .AddTables("T_VAC_REAZIONI_AVVERSE")
                .AddWhereCondition("VRA_ID", Comparatori.Uguale, idReazioneAvversa, DataTypes.Numero)

            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Delete) > 0

        End Function

        Public Function DeleteReazioneAvversaScadutaById(idReazioneAvversaScaduta As Long) As Boolean Implements IVaccinazioniEseguiteProvider.DeleteReazioneAvversaScadutaById

            With _DAM.QB

                .NewQuery()
                .AddTables("T_VAC_REAZIONI_SCADUTE")
                .AddWhereCondition("VRS_ID", Comparatori.Uguale, idReazioneAvversaScaduta, DataTypes.Numero)

            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Delete) > 0

        End Function

#End Region

#End Region

#Region " Exists "

        Public Function EsisteVaccinazioneEseguita(codicePaziente As Integer, dataEsecuzione As Date, codiceVaccinazione As String) As Boolean Implements IVaccinazioniEseguiteProvider.EsisteVaccinazioneEseguita

            Dim result As Boolean = False

            Try

                With _DAM.QB
                    .NewQuery()
                    .AddSelectFields("1")
                    .AddTables("T_VAC_ESEGUITE")
                    .AddWhereCondition("VES_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Stringa)
                    .AddWhereCondition("VES_VAC_CODICE", Comparatori.Uguale, codiceVaccinazione, DataTypes.Stringa)
                    .AddWhereCondition("VES_DATA_EFFETTUAZIONE", Comparatori.Uguale, dataEsecuzione, DataTypes.Data)
                End With

                Dim obj As Object = _DAM.ExecScalar()

                If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                    result = True
                End If

            Catch ex As Exception

                SetErrorMsg("Errore durante controllo EsisteVaccinazioneEseguita")
                LogError(ex, "DBVaccinazioniEseguiteProvider.EsisteVaccinazioneEseguita: errore.")

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return result

        End Function

        ''' <summary>
        ''' Restituisce true se esiste la vaccinazione scaduta indicata
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataEsecuzione"></param>
        ''' <param name="codiceVaccinazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EsisteVaccinazioneScaduta(codicePaziente As Integer, dataEsecuzione As DateTime, codiceVaccinazione As String) As Boolean Implements IVaccinazioniEseguiteProvider.EsisteVaccinazioneScaduta

            Dim result As Boolean = False

            Try
                With _DAM.QB
                    .NewQuery()
                    .AddSelectFields("1")
                    .AddTables("T_VAC_SCADUTE")
                    .AddWhereCondition("VSC_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                    .AddWhereCondition("VSC_VAC_CODICE", Comparatori.Uguale, codiceVaccinazione, DataTypes.Stringa)
                    .AddWhereCondition("VSC_DATA_EFFETTUAZIONE", Comparatori.Uguale, dataEsecuzione.Date, DataTypes.Data)
                End With

                Dim obj As Object = _DAM.ExecScalar()

                If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                    result = True
                End If

            Catch ex As Exception

                SetErrorMsg("Errore durante controllo EsisteVaccinazioneScaduta")

                LogError(ex, "DBVaccinazioniEseguiteProvider.EsisteVaccinazioneScaduta: errore.")

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return result

        End Function

        ''' <summary>
        ''' Restituisce true se esiste l'associazione eseguita indicata, per il paziente, nella data specificata
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataEsecuzione"></param>
        ''' <param name="codiceAssociazione"></param>
        ''' <returns></returns>
        Public Function EsisteAssociazioneEseguita(codicePaziente As Long, dataEsecuzione As Date, codiceAssociazione As String) As Boolean Implements IVaccinazioniEseguiteProvider.EsisteAssociazioneEseguita

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleCommand("SELECT 1 FROM T_VAC_ESEGUITE WHERE VES_PAZ_CODICE = :VES_PAZ_CODICE AND VES_ASS_CODICE = :VES_ASS_CODICE AND VES_DATA_EFFETTUAZIONE = :VES_DATA_EFFETTUAZIONE", Connection)

                    cmd.Parameters.AddWithValue("VES_PAZ_CODICE", codicePaziente)
                    cmd.Parameters.AddWithValue("VES_DATA_EFFETTUAZIONE", dataEsecuzione)
                    cmd.Parameters.AddWithValue("VES_ASS_CODICE", codiceAssociazione)

                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        Return True
                    End If

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return False

        End Function

#End Region

#Region " Select "

        '''<summary>
        ''' Restituisce la dose di associazione max
        '''</summary>
        Public Function GetMaxDoseAssociazione(codicePaziente As Integer, codiceAssociazione As String) As Integer Implements IVaccinazioniEseguiteProvider.GetMaxDoseAssociazione

            Dim result As Integer
            Dim cmd As OracleCommand = Nothing
            Dim strQuery As String = Queries.VaccinazioniEseguite.OracleQueries.selMaxDoseAssociazione

            Dim ownConnection As Boolean = False

            Try
                cmd = New OracleCommand(strQuery, Connection)

                ownConnection = ConditionalOpenConnection(cmd)

                cmd.Parameters.AddWithValue("paz_cod", codicePaziente)
                cmd.Parameters.AddWithValue("ass_codice", codiceAssociazione)

                Dim obj As Object = cmd.ExecuteScalar()

                If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                    result = Convert.ToInt32(obj)
                End If

            Catch ex As Exception

                Dim msg As New Text.StringBuilder()

                msg.AppendFormat("DAL: Errore ricerca vaccinazioni (VaccinazioneProg.GetDoseAssociazioneProgrammataPaziente){0}", vbNewLine)
                msg.AppendFormat("Paziente:{0}{1}", codicePaziente, vbNewLine)
                msg.AppendFormat("Associazione:{0}", codiceAssociazione)

                Throw New Exception(msg.ToString(), ex)

            Finally

                ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return result

        End Function

        Public Function GetVaccinazioniEseguite(codicePaziente As Integer) As DataTable Implements IVaccinazioniEseguiteProvider.GetVaccinazioniEseguite

            Return GetVaccinazioniEseguite(codicePaziente, Nothing)

        End Function

        Public Function GetVaccinazioniEseguite(codicePaziente As Long, listaIdVaccinazione As List(Of Long)) As DataTable Implements IVaccinazioniEseguiteProvider.GetVaccinazioniEseguite

            Dim filtroEseguite As New Text.StringBuilder()
            Dim filtroScadute As New Text.StringBuilder()

            RefurbishDT()

            Try
                _DAM.ClearParam()

                If Not listaIdVaccinazione.IsNullOrEmpty() Then

                    If listaIdVaccinazione.Count = 1 Then

                        filtroEseguite.Append(" AND ves_id = :ves_id ")
                        _DAM.AddParameter("ves_id", listaIdVaccinazione.Single())

                        filtroScadute.Append(" AND vsc_id = :vsc_id ")
                        _DAM.AddParameter("vsc_id", listaIdVaccinazione.Single())

                    Else

                        filtroEseguite.Append(" AND ves_id IN (")
                        filtroScadute.Append(" AND vsc_id IN (")

                        For i As Integer = 0 To listaIdVaccinazione.Count - 1

                            Dim nomeParametroVES As String = String.Format("ves{0}", i.ToString())
                            Dim nomeParametroVSC As String = String.Format("vsc{0}", i.ToString())

                            _DAM.AddParameter(nomeParametroVES, listaIdVaccinazione(i))
                            _DAM.AddParameter(nomeParametroVSC, listaIdVaccinazione(i))

                            filtroEseguite.AppendFormat(":{0},", nomeParametroVES)
                            filtroScadute.AppendFormat(":{0},", nomeParametroVSC)

                        Next

                        filtroEseguite.RemoveLast(1).Append(") ")
                        filtroScadute.RemoveLast(1).Append(") ")

                    End If

                End If

                _DAM.AddParameter("paz_codice", codicePaziente)
                _DAM.BuildDataTable(String.Format(Queries.VaccinazioniEseguite.OracleQueries.selVaccinazioniEseguite, filtroEseguite.ToString(), filtroScadute.ToString()), _DT)

            Catch ex As Exception

                _DT = Nothing

                LogError(ex)

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return _DT.Copy

        End Function

        Public Function GetVaccinazioni(idPaziente As Integer) As List(Of VaccinazioniAPI)

            Dim result As New List(Of VaccinazioniAPI)
            Dim ownConnection As Boolean = False
            Dim query As String = "Select ves_id, ves_paz_codice, ves_dataora_effettuazione, ves_vii_codice, ves_data_effettuazione, " +
                " ves_noc_codice, ves_ass_codice, ves_ass_n_dose, vac_ordine, ves_sii_codice, " +
                "ves_vac_codice, vac_descrizione, ves_n_richiamo, " +
                "ves_lot_codice, sii_descrizione, vii_descrizione, " +
                "noc_descrizione, ass_descrizione, " +
                "'N' scaduta, ves_flag_fittizia, " +
                "ves_cns_codice, ves_ute_id, paz_cognome, paz_nome " +
                "From t_vac_eseguite " +
                "Left Join t_ana_lotti ON ves_lot_codice = lot_codice " +
                "Left Join t_ana_associazioni ON ves_ass_codice = ass_codice " +
                "Left Join t_ana_vaccinazioni ON ves_vac_codice = vac_codice " +
                "Left Join t_ana_nomi_commerciali ON ves_noc_codice = noc_codice " +
                "Left Join t_ana_siti_inoculazione ON ves_sii_codice = sii_codice " +
                "Left Join t_ana_vie_somministrazione ON ves_vii_codice = vii_codice " +
                "JOIN t_paz_pazienti ON ves_paz_codice = paz_codice " +
                "WHERE ves_paz_codice = :idPaziente " +
                "And (ASS_ANTI_INFLUENZALE = 'S' " +
                "OR ASS_ANTI_PNEUMOCOCCO = 'S') " +
                "UNION " +
                "Select vsc_id, vsc_paz_codice, vsc_dataora_effettuazione, vsc_vii_codice, vsc_data_effettuazione, " +
                "vsc_noc_codice, vsc_ass_codice, vsc_ass_n_dose, vac_ordine, vsc_sii_codice, " +
                "vsc_vac_codice, vac_descrizione, vsc_n_richiamo, " +
                "vsc_lot_codice, sii_descrizione, vii_descrizione, " +
                "noc_descrizione, ass_descrizione, " +
                "'S' scaduta, vsc_flag_fittizia, " +
                "vsc_cns_codice, vsc_ute_id, paz_cognome, paz_nome " +
                "From t_vac_scadute " +
                "Left Join t_ana_lotti ON vsc_lot_codice = lot_codice " +
                "Left Join t_ana_associazioni ON vsc_ass_codice = ass_codice " +
                "Left Join t_ana_vaccinazioni ON vsc_vac_codice = vac_codice " +
                "Left Join t_ana_nomi_commerciali ON vsc_noc_codice = noc_codice " +
                "Left Join t_ana_siti_inoculazione ON vsc_sii_codice = sii_codice " +
                "Left Join t_ana_vie_somministrazione ON vsc_vii_codice = vii_codice " +
                "JOIN t_paz_pazienti ON vsc_paz_codice = paz_codice " +
                "WHERE vsc_paz_codice = :idPaziente " +
                "And (ASS_ANTI_INFLUENZALE = 'S' " +
                "OR ASS_ANTI_PNEUMOCOCCO = 'S') " +
                "ORDER BY ves_dataora_effettuazione DESC, vac_ordine, ass_descrizione, vac_descrizione"

            Try
                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    cmd.Parameters.AddWithValue("idPaziente", idPaziente)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()

                        Dim ves_id As Integer = _context.GetOrdinal("ves_id")
                        Dim ves_paz_codice As Integer = _context.GetOrdinal("ves_paz_codice")
                        Dim ves_dataora_effettuazione As Integer = _context.GetOrdinal("ves_dataora_effettuazione")
                        Dim ves_vii_codice As Integer = _context.GetOrdinal("ves_vii_codice")
                        Dim ves_data_effettuazione As Integer = _context.GetOrdinal("ves_data_effettuazione")
                        Dim ves_noc_codice As Integer = _context.GetOrdinal("ves_noc_codice")
                        Dim ves_ass_codice As Integer = _context.GetOrdinal("ves_ass_codice")
                        Dim ves_ass_n_dose As Integer = _context.GetOrdinal("ves_ass_n_dose")
                        Dim vac_ordine As Integer = _context.GetOrdinal("vac_ordine")
                        Dim ves_sii_codice As Integer = _context.GetOrdinal("ves_sii_codice")
                        Dim ves_vac_codice As Integer = _context.GetOrdinal("ves_vac_codice")
                        Dim vac_descrizione As Integer = _context.GetOrdinal("vac_descrizione")
                        Dim ves_n_richiamo As Integer = _context.GetOrdinal("ves_n_richiamo")
                        Dim ves_lot_codice As Integer = _context.GetOrdinal("ves_lot_codice")
                        Dim sii_descrizione As Integer = _context.GetOrdinal("sii_descrizione")
                        Dim vii_descrizione As Integer = _context.GetOrdinal("vii_descrizione")
                        Dim noc_descrizione As Integer = _context.GetOrdinal("noc_descrizione")
                        Dim ass_descrizione As Integer = _context.GetOrdinal("ass_descrizione")
                        Dim ves_flag_fittizia As Integer = _context.GetOrdinal("ves_flag_fittizia")
                        Dim ves_cns_codice As Integer = _context.GetOrdinal("ves_cns_codice")
                        Dim ves_ute_id As Integer = _context.GetOrdinal("ves_ute_id")
                        Dim paz_cognome As Integer = _context.GetOrdinal("paz_cognome")
                        Dim paz_nome As Integer = _context.GetOrdinal("paz_nome")

                        While _context.Read()
                            Dim vaccinazioni As New VaccinazioniAPI()
                            vaccinazioni.ves_id = _context.GetInt64OrDefault(ves_id)
                            vaccinazioni.ves_paz_codice = _context.GetInt32OrDefault(ves_paz_codice)
                            vaccinazioni.ves_dataora_effettuazione = _context.GetDateTimeOrDefault(ves_dataora_effettuazione)
                            vaccinazioni.ves_vii_codice = _context.GetStringOrDefault(ves_vii_codice)
                            vaccinazioni.ves_data_effettuazione = _context.GetDateTimeOrDefault(ves_data_effettuazione)
                            vaccinazioni.ves_noc_codice = _context.GetStringOrDefault(ves_noc_codice)
                            vaccinazioni.ves_ass_codice = _context.GetStringOrDefault(ves_ass_codice)
                            vaccinazioni.ves_ass_n_dose = _context.GetInt32OrDefault(ves_ass_n_dose)
                            vaccinazioni.vac_ordine = _context.GetInt32OrDefault(vac_ordine)
                            vaccinazioni.ves_sii_codice = _context.GetStringOrDefault(ves_sii_codice)
                            vaccinazioni.ves_vac_codice = _context.GetStringOrDefault(ves_vac_codice)
                            vaccinazioni.vac_descrizione = _context.GetStringOrDefault(vac_descrizione)
                            vaccinazioni.ves_n_richiamo = _context.GetInt32OrDefault(ves_n_richiamo)
                            vaccinazioni.ves_lot_codice = _context.GetStringOrDefault(ves_lot_codice)
                            vaccinazioni.sii_descrizione = _context.GetStringOrDefault(ves_lot_codice)
                            vaccinazioni.vii_descrizione = _context.GetStringOrDefault(vii_descrizione)
                            vaccinazioni.noc_descrizione = _context.GetStringOrDefault(noc_descrizione)
                            vaccinazioni.ass_descrizione = _context.GetStringOrDefault(ass_descrizione)
                            vaccinazioni.ves_flag_fittizia = _context.GetStringOrDefault(ves_flag_fittizia)
                            vaccinazioni.ves_cns_codice = _context.GetStringOrDefault(ves_cns_codice)
                            vaccinazioni.ves_ute_id = _context.GetInt64OrDefault(ves_ute_id)
                            vaccinazioni.paz_nome = _context.GetStringOrDefault(paz_nome)
                            vaccinazioni.paz_cognome = _context.GetStringOrDefault(paz_cognome)
                            result.Add(vaccinazioni)
                        End While

                    End Using
                End Using
            Catch ex As Exception
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return result
        End Function

        Public Function GetVaccinazioniEseguiteIntegrazione(listaReazioni As List(Of Integer)) As DataTable Implements IVaccinazioniEseguiteProvider.GetVaccinazioniEseguiteIntegrazione

            RefurbishDT()

            Try
                _DAM.ClearParam()
                Dim J As Integer = 1
                Dim listaParametri As String = ""
                For Each i As Integer In listaReazioni
                    _DAM.AddParameter(String.Format("p{0}", J.ToString()), i)
                    listaParametri = listaParametri + String.Format(":p{0}", J.ToString()) + ","
                    J = J + 1
                Next
                listaParametri = Left(listaParametri, listaParametri.Length - 1)
                _DAM.BuildDataTable(String.Format(OnVac.Queries.VaccinazioniEseguite.OracleQueries.selVaccinazioniEseguiteIntegrazione, listaParametri), _DT)

            Catch ex As Exception

                _DT = Nothing

                LogError(ex)

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return _DT.Copy

        End Function

        Public Function GetVaccinazioniEseguitePaziente(codicePaziente As Integer) As List(Of Entities.VaccinazioneEseguita) Implements IVaccinazioniEseguiteProvider.GetVaccinazioniEseguitePaziente

            Dim vaccinazioneEseguitaList As New List(Of VaccinazioneEseguita)()

            Using cmd As New OracleCommand()

                Dim ownConnection As Boolean = False

                Try
                    cmd.Connection = Connection

                    Dim query As New Text.StringBuilder("SELECT * FROM T_VAC_ESEGUITE WHERE VES_PAZ_CODICE = :VES_PAZ_CODICE ")

                    cmd.Parameters.AddWithValue("VES_PAZ_CODICE", codicePaziente)

                    query.Append(" order by ves_data_effettuazione, ves_ass_codice, ves_vac_codice")

                    cmd.CommandText = query.ToString()

                    ownConnection = ConditionalOpenConnection(cmd)

                    Using reader As IDataReader = cmd.ExecuteReader()

                        vaccinazioneEseguitaList = GetVaccinazioniEseguiteListFromDataReader(reader, "VES", False)

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return vaccinazioneEseguitaList

        End Function

        Public Function GetVaccinazioneEseguitaPaziente(codicePaziente As Long, codiceVaccinazione As String, numeroRichiamo As Int16) As Entities.VaccinazioneEseguita Implements IVaccinazioniEseguiteProvider.GetVaccinazioneEseguitaPaziente

            Dim cmd As OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try
                cmd = New OracleCommand(Queries.VaccinazioniEseguite.OracleQueries.selVaccinazioniEseguitePaziente, Connection)

                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)
                cmd.Parameters.AddWithValue("codiceVaccinazione", codiceVaccinazione)
                cmd.Parameters.AddWithValue("numeroRichiamo", numeroRichiamo)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using reader As IDataReader = cmd.ExecuteReader()

                    Return GetVaccinazioniEseguiteListFromDataReader(reader, "VES", False).FirstOrDefault()

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
                If Not cmd Is Nothing Then cmd.Dispose()
            End Try

        End Function

        Public Function GetVaccinazioneEseguitaPaziente(codicePaziente As Long, codiceVaccinazione As String, numeroRichiamo As Short, dataEffettuazione As Date) As Entities.VaccinazioneEseguita Implements IVaccinazioniEseguiteProvider.GetVaccinazioneEseguitaPaziente

            Dim vaccinazioneEseguita As VaccinazioneEseguita

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try

                cmd = New OracleClient.OracleCommand(Queries.VaccinazioniEseguite.OracleQueries.selVaccinazioniEseguitePaziente2, Me.Connection)

                cmd.Parameters.AddWithValue("ves_paz_codice", codicePaziente)
                cmd.Parameters.AddWithValue("ves_vac_codice", codiceVaccinazione)
                cmd.Parameters.AddWithValue("ves_n_richiamo", numeroRichiamo)
                cmd.Parameters.AddWithValue("ves_data_effettuazione", dataEffettuazione)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using reader As IDataReader = cmd.ExecuteReader()

                    vaccinazioneEseguita = Me.GetVaccinazioniEseguiteListFromDataReader(reader, "VES", False).FirstOrDefault()

                End Using

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return vaccinazioneEseguita

        End Function

        Public Function GetVaccinazioneEseguitaByIdIfExists(idVaccinazioneEseguita As Integer) As Entities.VaccinazioneEseguita Implements IVaccinazioniEseguiteProvider.GetVaccinazioneEseguitaByIdIfExists

            Dim vaccinazioneEseguita As VaccinazioneEseguita

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try

                cmd = New OracleClient.OracleCommand(Queries.VaccinazioniEseguite.OracleQueries.selVaccinazioneEseguitaById, Me.Connection)

                cmd.Parameters.AddWithValue("ves_id", idVaccinazioneEseguita)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using reader As IDataReader = cmd.ExecuteReader()

                    vaccinazioneEseguita = Me.GetVaccinazioniEseguiteListFromDataReader(reader, "VES", False).FirstOrDefault()

                End Using

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return vaccinazioneEseguita

        End Function

        '''<summary>
        ''' Restituisce la vaccinazione eseguite del paziente corrispondente al ves_id specificato
        '''</summary>
        Public Overloads Function GetVaccinazioneEseguitaById(idVaccinazioneEseguita As Integer) As VaccinazioneEseguita Implements IVaccinazioniEseguiteProvider.GetVaccinazioneEseguitaById

            Return Me.GetVaccinazioneEseguitaByIdIfExists(idVaccinazioneEseguita)

        End Function

        '''<summary>
        ''' Restituisce la vaccinazione eseguite del paziente corrispondente al ves_id specificato
        '''</summary>
        Public Overloads Function GetVaccinazioneEseguitaEliminataById(idVaccinazioneEseguitaEliminata As Integer) As VaccinazioneEseguita Implements IVaccinazioniEseguiteProvider.GetVaccinazioneEseguitaEliminataById

            Dim vaccinazioneEseguitaEliminata As VaccinazioneEseguita

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try

                cmd = New OracleClient.OracleCommand(Queries.VaccinazioniEseguite.OracleQueries.selVaccinazioneEseguitaEliminataById, Me.Connection)

                cmd.Parameters.AddWithValue("vee_id", idVaccinazioneEseguitaEliminata)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using reader As IDataReader = cmd.ExecuteReader()

                    vaccinazioneEseguitaEliminata = Me.GetVaccinazioniEseguiteListFromDataReader(reader, "VEE", True).First()

                End Using

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return vaccinazioneEseguitaEliminata

        End Function

        Public Function GetVaccinazioneEseguitaScadutaEliminataById(idVaccinazioneEseguitaScadutaEliminata As Integer) As VaccinazioneEseguita Implements IVaccinazioniEseguiteProvider.GetVaccinazioneEseguitaScadutaEliminataById

            Dim vaccinazioneEseguitaEliminata As VaccinazioneEseguita

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try

                cmd = New OracleClient.OracleCommand(Queries.VaccinazioniEseguite.OracleQueries.selVaccinazioneEseguitaScadutaEliminataById, Me.Connection)

                cmd.Parameters.AddWithValue("vse_id", idVaccinazioneEseguitaScadutaEliminata)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using reader As IDataReader = cmd.ExecuteReader()

                    vaccinazioneEseguitaEliminata = Me.GetVaccinazioniEseguiteListFromDataReader(reader, "VSE", True).First()

                End Using

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return vaccinazioneEseguitaEliminata

        End Function

        Public Function GetVaccinazioneEseguitaScadutaPaziente(codicePaziente As Long, codiceVaccinazione As String, numeroRichiamo As Short, dataEffettuazione As Date) As Entities.VaccinazioneEseguita Implements IVaccinazioniEseguiteProvider.GetVaccinazioneEseguitaScadutaPaziente

            Dim vaccinazioneEseguitaScaduta As VaccinazioneEseguita

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try

                cmd = New OracleClient.OracleCommand(Queries.VaccinazioniEseguite.OracleQueries.selVaccinazioneEseguitaScadutaPaziente, Me.Connection)

                cmd.Parameters.AddWithValue("vsc_paz_codice", codicePaziente)
                cmd.Parameters.AddWithValue("vsc_vac_codice", codiceVaccinazione)
                cmd.Parameters.AddWithValue("vsc_n_richiamo", numeroRichiamo)
                cmd.Parameters.AddWithValue("vsc_data_effettuazione", dataEffettuazione)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using reader As IDataReader = cmd.ExecuteReader()

                    vaccinazioneEseguitaScaduta = Me.GetVaccinazioniEseguiteListFromDataReader(reader, "VSC", False).FirstOrDefault()

                End Using

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return vaccinazioneEseguitaScaduta

        End Function

        Public Function GetVaccinazioneEseguitaScadutaByIdIfExists(idVaccinazioneEseguitaScaduta As Integer) As Entities.VaccinazioneEseguita Implements IVaccinazioniEseguiteProvider.GetVaccinazioneEseguitaScadutaByIdIfExists

            Dim vaccinazioneEseguitaScaduta As VaccinazioneEseguita

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try

                cmd = New OracleClient.OracleCommand(Queries.VaccinazioniEseguite.OracleQueries.selVaccinazioneEseguitaScadutaById, Me.Connection)

                cmd.Parameters.AddWithValue("vsc_id", idVaccinazioneEseguitaScaduta)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using reader As IDataReader = cmd.ExecuteReader()

                    vaccinazioneEseguitaScaduta = Me.GetVaccinazioniEseguiteListFromDataReader(reader, "VSC", False).FirstOrDefault()

                End Using

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return vaccinazioneEseguitaScaduta

        End Function

        Public Function GetVaccinazioneEseguitaScadutaById(idVaccinazioneEseguitaScaduta As Integer) As VaccinazioneEseguita Implements IVaccinazioniEseguiteProvider.GetVaccinazioneEseguitaScadutaById

            Return Me.GetVaccinazioneEseguitaScadutaByIdIfExists(idVaccinazioneEseguitaScaduta)

        End Function

        Public Function GetVaccinazioniEseguiteScadutePaziente(codicePaziente As Integer) As VaccinazioneEseguita() Implements IVaccinazioniEseguiteProvider.GetVaccinazioniEseguiteScadutePaziente

            Dim vaccinazioniEseguiteScadute As VaccinazioneEseguita()

            Using cmd As New OracleCommand()

                cmd.Connection = Connection

                Dim ownConnection As Boolean = False

                Try
                    Dim query As New Text.StringBuilder("SELECT * FROM T_VAC_SCADUTE WHERE VSC_PAZ_CODICE = :VSC_PAZ_CODICE")

                    cmd.Parameters.AddWithValue("VSC_PAZ_CODICE", codicePaziente)

                    cmd.CommandText = query.ToString()

                    ownConnection = ConditionalOpenConnection(cmd)

                    Using reader As IDataReader = cmd.ExecuteReader()

                        vaccinazioniEseguiteScadute = GetVaccinazioniEseguiteListFromDataReader(reader, "VSC", False).ToArray()

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return vaccinazioniEseguiteScadute

        End Function

        Public Function GetReazioneAvversaByIdIfExists(idReazioneAvversa As Integer) As ReazioneAvversa Implements IVaccinazioniEseguiteProvider.GetReazioneAvversaByIdIfExists

            Dim reazioneAvversa As ReazioneAvversa

            Dim cmd As OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try
                cmd = New OracleCommand(Queries.VaccinazioniEseguite.OracleQueries.selReazioneAvversaById, Connection)

                cmd.Parameters.AddWithValue("vra_id", idReazioneAvversa)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using reader As IDataReader = cmd.ExecuteReader()

                    reazioneAvversa = GetReazioneAvversaListFromDataReader(reader, "VRA", "VES", False).FirstOrDefault()

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
                If Not cmd Is Nothing Then cmd.Dispose()
            End Try

            Return reazioneAvversa

        End Function

        Public Function GetReazioneAvversaById(idReazioneAvversa As Integer) As Entities.ReazioneAvversa Implements IVaccinazioniEseguiteProvider.GetReazioneAvversaById

            Return GetReazioneAvversaByIdIfExists(idReazioneAvversa)

        End Function

        Public Function GetReazioniAvversePaziente(codicePaziente As Long) As List(Of ReazioneAvversa) Implements IVaccinazioniEseguiteProvider.GetReazioniAvversePaziente

            Dim reazioniAvverse As List(Of ReazioneAvversa)

            Using cmd As New OracleCommand()

                cmd.Connection = Connection

                Dim ownConnection As Boolean = False

                Try
                    Dim query As New System.Text.StringBuilder("SELECT * FROM T_VAC_REAZIONI_AVVERSE WHERE VRA_PAZ_CODICE = :VRA_PAZ_CODICE ")

                    cmd.Parameters.AddWithValue("VRA_PAZ_CODICE", codicePaziente)

                    cmd.CommandText = query.ToString()

                    ownConnection = ConditionalOpenConnection(cmd)

                    Using reader As IDataReader = cmd.ExecuteReader()

                        reazioniAvverse = GetReazioneAvversaListFromDataReader(reader, "VRA", "VES", False)

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return reazioniAvverse

        End Function

        Public Function GetReazioniAvverseScadutePaziente(codicePaziente As Long) As List(Of ReazioneAvversa) Implements IVaccinazioniEseguiteProvider.GetReazioniAvverseScadutePaziente

            Dim reazioniAvverseScadute As List(Of ReazioneAvversa)

            Using cmd As New OracleCommand()

                Dim ownConnection As Boolean = False

                Try
                    cmd.Connection = Connection

                    Dim query As New Text.StringBuilder("SELECT * FROM T_VAC_REAZIONI_SCADUTE WHERE VRS_PAZ_CODICE = :VRS_PAZ_CODICE ")

                    cmd.Parameters.AddWithValue("VRS_PAZ_CODICE", codicePaziente)

                    cmd.CommandText = query.ToString()

                    ownConnection = ConditionalOpenConnection(cmd)

                    Using reader As IDataReader = cmd.ExecuteReader()

                        reazioniAvverseScadute = GetReazioneAvversaListFromDataReader(reader, "VRS", "VSC", False)

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return reazioniAvverseScadute

        End Function

        Public Function GetReazioneAvversaScadutaByIdIfExists(idReazioneAvversaScaduta As Integer) As ReazioneAvversa Implements IVaccinazioniEseguiteProvider.GetReazioneAvversaScadutaByIdIfExists

            Dim reazioneAvversa As ReazioneAvversa

            Dim cmd As OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try
                cmd = New OracleCommand(Queries.VaccinazioniEseguite.OracleQueries.selReazioneAvversaScadutaById, Connection)

                cmd.Parameters.AddWithValue("vrs_id", idReazioneAvversaScaduta)

                ownConnection = ConditionalOpenConnection(cmd)

                Using reader As IDataReader = cmd.ExecuteReader()

                    reazioneAvversa = GetReazioneAvversaListFromDataReader(reader, "VRS", "VSC", False).FirstOrDefault()

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
                If Not cmd Is Nothing Then cmd.Dispose()
            End Try

            Return reazioneAvversa

        End Function

        Public Function GetReazioneAvversaScadutaById(idReazioneAvversaScaduta As Integer) As Entities.ReazioneAvversa Implements IVaccinazioniEseguiteProvider.GetReazioneAvversaScadutaById

            Return GetReazioneAvversaScadutaByIdIfExists(idReazioneAvversaScaduta)

        End Function

        Public Function GetReazioneAvversaEliminataById(idReazioneAvversaEliminata As Integer) As Entities.ReazioneAvversa Implements IVaccinazioniEseguiteProvider.GetReazioneAvversaEliminataById

            Dim reazioneAvversaEliminata As ReazioneAvversa

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try

                cmd = New OracleClient.OracleCommand(Queries.VaccinazioniEseguite.OracleQueries.selReazioneAvversaEliminataById, Me.Connection)

                cmd.Parameters.AddWithValue("vre_id", idReazioneAvversaEliminata)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using reader As IDataReader = cmd.ExecuteReader()

                    reazioneAvversaEliminata = Me.GetReazioneAvversaListFromDataReader(reader, "VRE", "VES", True).First()

                End Using

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return reazioneAvversaEliminata

        End Function

        Public Function GetReazioneAvversaScadutaEliminataById(idReazioneAvversaScadutaEliminata As Integer) As Entities.ReazioneAvversa Implements IVaccinazioniEseguiteProvider.GetReazioneAvversaScadutaEliminataById

            Dim reazioneAvversaScadutaEliminata As ReazioneAvversa

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try

                cmd = New OracleClient.OracleCommand(Queries.VaccinazioniEseguite.OracleQueries.selReazioneAvversaScadutaEliminataById, Me.Connection)

                cmd.Parameters.AddWithValue("vrl_id", idReazioneAvversaScadutaEliminata)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using reader As IDataReader = cmd.ExecuteReader()

                    reazioneAvversaScadutaEliminata = Me.GetReazioneAvversaListFromDataReader(reader, "VRL", "VSC", True).First()

                End Using

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return reazioneAvversaScadutaEliminata

        End Function

        Public Function GetReazioneAvversaByVaccinazioneEseguita(idVaccinazioneEseguita As Long) As Entities.ReazioneAvversa Implements IVaccinazioniEseguiteProvider.GetReazioneAvversaByVaccinazioneEseguita

            Dim reazioneAvversa As ReazioneAvversa

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try

                cmd = New OracleClient.OracleCommand("SELECT * FROM T_VAC_REAZIONI_AVVERSE WHERE VRA_VES_ID=:VRA_VES_ID", Me.Connection)

                cmd.Parameters.AddWithValue("VRA_VES_ID", idVaccinazioneEseguita)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using reader As IDataReader = cmd.ExecuteReader()

                    Dim reazioneAvversaList As List(Of ReazioneAvversa) = GetReazioneAvversaListFromDataReader(reader, "VRA", "VES", False)
                    If Not reazioneAvversaList Is Nothing AndAlso reazioneAvversaList.Count > 0 Then
                        reazioneAvversa = reazioneAvversaList.FirstOrDefault()
                    End If
                    'reazioneAvversa = Me.GetReazioneAvversaListFromDataReader(reader, "VRA", "VES", False).First()

                End Using

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return reazioneAvversa

        End Function

        Public Function GetReazioneAvversaScadutaByVaccinazioneEseguitaScaduta(idVaccinazioneEseguitaScaduta As Long) As Entities.ReazioneAvversa Implements IVaccinazioniEseguiteProvider.GetReazioneAvversaScadutaByVaccinazioneEseguitaScaduta

            Dim reazioneAvversa As ReazioneAvversa

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try

                cmd = New OracleClient.OracleCommand("SELECT * FROM T_VAC_REAZIONI_SCADUTE WHERE VRS_VSC_ID=:VRS_VSC_ID", Me.Connection)

                cmd.Parameters.AddWithValue("VRS_VSC_ID", idVaccinazioneEseguitaScaduta)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using reader As IDataReader = cmd.ExecuteReader()

                    reazioneAvversa = Me.GetReazioneAvversaListFromDataReader(reader, "VRS", "VSC", False).First()

                End Using

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return reazioneAvversa

        End Function

        Public Function GetProssimaSedutaDaRegistrare(codicePaziente As Integer) As DataTable Implements IVaccinazioniEseguiteProvider.GetProssimaSedutaDaRegistrare

            RefurbishDT()

            Try
                _DAM.ClearParam()

                _DAM.AddParameter("paz_codice", codicePaziente)
                _DAM.BuildDataTable(OnVac.Queries.VaccinazioniEseguite.OracleQueries.selProssimaSedutaDaRegistrare, _DT)

            Catch ex As Exception

                _DT = Nothing

                LogError(ex)

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return _DT.Copy

        End Function

        ''' <summary>
        ''' Restituisce il massimo richiamo per il paziente e la vaccinazione specificati. Restituisce 0 se nessun richiamo presente.
        ''' </summary>
        Public Function GetMaxRichiamo(codicePaziente As Integer, codiceVaccinazione As String) As Integer Implements IVaccinazioniEseguiteProvider.GetMaxRichiamo

            Dim max_richiamo As Integer = 0

            Using cmd As New OracleClient.OracleCommand(Queries.VaccinazioniEseguite.OracleQueries.selMaxRichiamo, Me.Connection)

                Dim ownConnection As Boolean = False

                cmd.Parameters.AddWithValue("cod_vac", codiceVaccinazione)
                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj Is Nothing OrElse obj Is DBNull.Value Then

                        max_richiamo = 0

                    Else

                        Try
                            max_richiamo = Convert.ToInt32(obj)
                        Catch ex As Exception
                            max_richiamo = 0
                        End Try

                    End If

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return max_richiamo

        End Function

        Public Function GetDtVaccinazioniEseguiteById(listIdVaccinazioniEseguite As List(Of Int64)) As DataTable Implements IVaccinazioniEseguiteProvider.GetDtVaccinazioniEseguiteById

            Dim dtVaccinazioniPaziente As New DataTable()

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection
                cmd.CommandText = Me.GetQueryIdVaccinazioniEseguite(cmd, Queries.VaccinazioniEseguite.OracleQueries.selVaccinazioniById, listIdVaccinazioniEseguite)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using dta As New OracleClient.OracleDataAdapter(cmd)

                        dta.Fill(dtVaccinazioniPaziente)

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return dtVaccinazioniPaziente

        End Function

        Public Function GetVaccinazioniEseguiteById(listIdVaccinazioniEseguite As List(Of Int64)) As List(Of Entities.VaccinazioneEseguita) Implements IVaccinazioniEseguiteProvider.GetVaccinazioniEseguiteById

            Dim listVaccinazioniEseguite As List(Of Entities.VaccinazioneEseguita) = Nothing

            Dim ownConnection As Boolean = False

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                cmd.CommandText = Me.GetQueryIdVaccinazioniEseguite(cmd, Queries.VaccinazioniEseguite.OracleQueries.selVaccinazioniEseguiteScaduteEliminateByListaId, listIdVaccinazioniEseguite)

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using reader As OracleClient.OracleDataReader = cmd.ExecuteReader()

                        listVaccinazioniEseguite = Me.GetVaccinazioniEseguiteListFromDataReader(reader, "VES", False)

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listVaccinazioniEseguite

        End Function

        Public Function GetDtReazioniAvverseById(listIdReazioniAvverse As List(Of Int64?)) As DataTable Implements IVaccinazioniEseguiteProvider.GetDtReazioniAvverseById

            Dim dtReazioniPaziente As New DataTable()

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                ' Aggiunta parametri per id reazioni
                Dim filtroId As New System.Text.StringBuilder()

                For i As Integer = 0 To listIdReazioniAvverse.Count - 1

                    Dim paramName As String = String.Format("param{0}", i.ToString())

                    cmd.Parameters.AddWithValue(paramName, listIdReazioniAvverse(i))

                    filtroId.AppendFormat(":{0},", paramName)

                Next

                If filtroId.Length > 0 Then filtroId.RemoveLast(1)

                cmd.CommandText = String.Format(Queries.VaccinazioniEseguite.OracleQueries.selReazioniAvverseById, filtroId.ToString())

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using dta As New OracleClient.OracleDataAdapter(cmd)

                        dta.Fill(dtReazioniPaziente)

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return dtReazioniPaziente

        End Function

        Public Function GetCodiciLottiReazioneByDataEffettuazione(codicePaziente As Integer, dataEffettuazione As DateTime) As List(Of String) Implements IVaccinazioniEseguiteProvider.GetCodiciLottiReazioneByDataEffettuazione

            Return Me.GetCodiciLottiReazioneByData(codicePaziente, dataEffettuazione, True)

        End Function

        Public Function GetCodiciLottiReazioneByDataReazione(codicePaziente As Integer, dataReazione As DateTime) As List(Of String) Implements IVaccinazioniEseguiteProvider.GetCodiciLottiReazioneByDataReazione

            Return Me.GetCodiciLottiReazioneByData(codicePaziente, dataReazione, False)

        End Function

        Public Function GetDtRichiamiVaccinazionePaziente(codicePaziente As String, codiceVaccinazione As String) As DataTable Implements IVaccinazioniEseguiteProvider.GetDtRichiamiVaccinazionePaziente

            Dim dt As New DataTable()

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("ves_vac_codice", "nvl(ves_n_richiamo,0) ves_n_richiamo")
                .AddTables("t_vac_eseguite")
                .AddWhereCondition("ves_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("ves_vac_codice", Comparatori.Uguale, codiceVaccinazione, DataTypes.Stringa)
            End With

            _DAM.BuildDataTable(dt)

            Return dt

        End Function

        Public Function GetDtNumeroDosiAssociazionePaziente(codicePaziente As String, codiceAssociazione As String) As DataTable Implements IVaccinazioniEseguiteProvider.GetDtNumeroDosiAssociazionePaziente

            Dim dt As DataTable = New DataTable()

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("ves_ass_codice", "nvl(ves_ass_n_dose,0) ves_ass_n_dose")
                .AddTables("t_vac_eseguite")
                .AddWhereCondition("ves_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.OutJoinRight)
                .AddWhereCondition("ves_ass_codice", Comparatori.Uguale, codiceAssociazione, DataTypes.Stringa)
                .AddGroupByFields("ves_ass_codice", "ves_ass_n_dose")
            End With

            _DAM.BuildDataTable(dt)

            Return dt

        End Function

#End Region

#Region " Count "

        ''' <summary>
        ''' Restituisce il numero di vaccinazioni eseguite per il paziente specificato
        ''' </summary>
        Public Function CountVaccinazioniEseguite(codicePaziente As Integer) As Integer Implements IVaccinazioniEseguiteProvider.CountVaccinazioniEseguite

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("COUNT(*)")
                .AddTables("T_VAC_ESEGUITE")
                .AddWhereCondition("VES_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
            End With

            Return _DAM.ExecScalar()

        End Function

        ''' <summary>
        ''' Restituisce il numero di vaccinazioni scadute per il paziente specificato
        ''' </summary>
        Public Function CountVaccinazioniScadute(codicePaziente As Integer) As Integer Implements IVaccinazioniEseguiteProvider.CountVaccinazioniScadute

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("COUNT(*)")
                .AddTables("T_VAC_SCADUTE")
                .AddWhereCondition("VSC_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
            End With

            Return _DAM.ExecScalar()

        End Function

        ''' <summary>
        ''' Restituisce il numero di reazioni avverse per il paziente specificato
        ''' </summary>
        Public Function CountReazioniAvverse(codicePaziente As Integer) As Integer Implements IVaccinazioniEseguiteProvider.CountReazioniAvverse

            With _DAM.QB

                .NewQuery()

                .AddSelectFields("1")
                .AddTables("T_STA_REAZIONI_AVVERSE")
                .AddWhereCondition("VRA_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddGroupByFields("VES_ASS_CODICE", "VES_ASS_N_DOSE", "VRA_RES_DATA_EFFETTUAZIONE", "VRA_LOT_CODICE")

                Dim q As String = .GetSelect()

                .NewQuery(False, False)

                .AddSelectFields("COUNT(*)")
                .AddTables(String.Format("({0})", q))

            End With

            Return _DAM.ExecScalar()

        End Function

#End Region

#End Region

#Region " Private Methods "

        Private Function GetVaccinazioneEseguiteDose(idr As OracleClient.OracleDataReader) As List(Of VaccinazioneDose)

            Dim vaccinazioneEseguiteDose As New List(Of VaccinazioneDose)

            If idr.HasRows Then

                Dim ves_vac_codice As Int16 = idr.GetOrdinal("VES_VAC_CODICE")
                Dim ves_n_richiamo As Int16 = idr.GetOrdinal("VES_N_RICHIAMO")

                While idr.Read()

                    Dim vaccinazioneEseguitaDose As New VaccinazioneDose()

                    vaccinazioneEseguitaDose.Codice = idr.GetStringOrDefault(ves_vac_codice)
                    vaccinazioneEseguitaDose.Dose = idr.GetInt64OrDefault(ves_n_richiamo)

                    vaccinazioneEseguiteDose.Add(vaccinazioneEseguitaDose)

                End While

            End If

            Return vaccinazioneEseguiteDose

        End Function

        Private Function GetVaccinazioniEseguiteListFromDataReader(reader As OracleClient.OracleDataReader, dbFieldPrefix As String, deleted As Boolean) As List(Of VaccinazioneEseguita)

            Dim vaccinazioneEseguitaList As New List(Of VaccinazioneEseguita)

            If reader.HasRows Then

                Dim IDOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_ID")
                Dim codicePazienteOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_PAZ_CODICE")
                Dim codicePazientePrecedenteOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_PAZ_CODICE_OLD")
                Dim codiceVaccinazioneOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_VAC_CODICE")
                Dim numeroRichiamoOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_N_RICHIAMO")
                Dim dataEffettuazioneOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_DATA_EFFETTUAZIONE")
                Dim dataOraEffettuazioneOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_DATAORA_EFFETTUAZIONE")
                Dim codiceConsultorioOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_CNS_CODICE")
                Dim statoOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_STATO")
                Dim codiceCicloOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_CIC_CODICE")
                Dim numeroSedutaOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_N_SEDUTA")
                Dim dataRegistrazioneOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_DATA_REGISTRAZIONE")
                Dim codiceLottoOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_LOT_CODICE")
                Dim codiceOperatoreOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_OPE_CODICE")
                Dim idUtenteOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_UTE_ID")
                Dim codiceSitoInoculazioneOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_SII_CODICE")
                Dim codiceNomeCommercialeOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_NOC_CODICE")
                Dim codiceAssociazioneOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_ASS_CODICE")
                Dim luogoOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_LUOGO")
                Dim codiceComuneOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_COMUNE_O_STATO")
                Dim codiceMedicoVaccinanteOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_MED_VACCINANTE")
                Dim dataConvocazioneOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_CNV_DATA")
                Dim dataPrimoAppuntamentoConvocazioneOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_CNV_DATA_PRIMO_APP")
                Dim inCampagnaOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_IN_CAMPAGNA")
                Dim operatoreInAmbulatorioOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_OPE_IN_AMBULATORIO")
                Dim esitoOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_ESITO")
                Dim fittiziaOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_FLAG_FITTIZIA")
                Dim noteOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_NOTE")
                Dim codiceConsultorioRegistrazioneOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_CNS_REGISTRAZIONE")
                Dim accessoOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_ACCESSO")
                Dim codiceAmbulatorioOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_AMB_CODICE")
                Dim codiceViaSomministrazioneOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_VII_CODICE")
                Dim codiceMalattiaOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_MAL_CODICE_MALATTIA")
                Dim codiceEsenzioneOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_CODICE_ESENZIONE")
                Dim importoOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_IMPORTO")
                Dim associazioneDoseOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_ASS_N_DOSE")
                Dim associazioneProgressivoOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_ASS_PROG")
                Dim codiceUslInserimentoOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_USL_INSERIMENTO")
                Dim data_ultima_variazione_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_DATA_ULTIMA_VARIAZIONE")
                Dim ute_id_ultima_variazione_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_UTE_ID_ULTIMA_VARIAZIONE")
                Dim codiceUslScadenzaOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_USL_SCADENZA")
                Dim dataScadenzaOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_DATA_SCADENZA")
                Dim IdUtenteScadenzaOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_UTE_ID_SCADENZA")
                Dim dataRipristinoOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_DATA_RIPRISTINO")
                Dim IdUtenteRipristinoOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_UTE_ID_RIPRISTINO")
                Dim flagVisibilitaOrdinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_FLAG_VISIBILITA")
                Dim note_acquisizione_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_NOTE_ACQUISIZIONE")
                Dim ves_id_acn_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_ID_ACN")
                Dim ves_provenienza_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_PROVENIENZA")
                Dim ves_mal_codice_cond_sanitaria As Integer = reader.GetOrdinal(dbFieldPrefix + "_MAL_CODICE_COND_SANITARIA")
                Dim ves_rsc_codice As Integer = reader.GetOrdinal(dbFieldPrefix + "_RSC_CODICE")
                Dim ves_tpa_pagamento As Integer = reader.GetOrdinal(dbFieldPrefix + "_TPA_GUID_TIPI_PAGAMENTO")
                Dim ves_lot_data_scadenza As Integer = reader.GetOrdinal(dbFieldPrefix + "_LOT_DATA_SCADENZA")
                Dim ves_tipo_associazione_acn As Integer = reader.GetOrdinal(dbFieldPrefix + "_TIPO_ASSOCIAZIONE_ACN")

                Dim dataEliminazioneOrdinal As Integer = -1
                Dim IdUtenteEliminazioneOrdinal As Integer = -1

                If deleted Then
                    dataEliminazioneOrdinal = reader.GetOrdinal(dbFieldPrefix + "_DATA_ELIMINAZIONE")
                    IdUtenteEliminazioneOrdinal = reader.GetOrdinal(dbFieldPrefix + "_UTE_ID_ELIMINAZIONE")
                End If

                While reader.Read

                    Dim vaccinazioneEseguita As New VaccinazioneEseguita()
                    vaccinazioneEseguita.ves_id = reader.GetInt32(IDOrdinal)
                    vaccinazioneEseguita.ves_vac_codice = reader.GetString(codiceVaccinazioneOrdinal)
                    vaccinazioneEseguita.paz_codice = reader.GetInt64(codicePazienteOrdinal)

                    If Not reader.IsDBNull(codicePazientePrecedenteOrdinal) Then
                        vaccinazioneEseguita.ves_paz_codice_old = reader.GetInt32(codicePazientePrecedenteOrdinal)
                    End If
                    If Not reader.IsDBNull(numeroRichiamoOrdinal) Then
                        vaccinazioneEseguita.ves_n_richiamo = reader.GetInt32(numeroRichiamoOrdinal)
                    End If
                    If Not reader.IsDBNull(codiceConsultorioOrdinal) Then
                        vaccinazioneEseguita.ves_cns_codice = reader.GetString(codiceConsultorioOrdinal)
                    End If
                    If Not reader.IsDBNull(dataOraEffettuazioneOrdinal) Then
                        vaccinazioneEseguita.ves_dataora_effettuazione = reader.GetDateTime(dataOraEffettuazioneOrdinal)
                    End If
                    If Not reader.IsDBNull(statoOrdinal) Then
                        vaccinazioneEseguita.ves_stato = reader.GetString(statoOrdinal)
                    End If
                    If Not reader.IsDBNull(codiceCicloOrdinal) Then
                        vaccinazioneEseguita.ves_cic_codice = reader.GetString(codiceCicloOrdinal)
                    End If
                    If Not reader.IsDBNull(numeroSedutaOrdinal) Then
                        vaccinazioneEseguita.ves_n_seduta = reader.GetInt32(numeroSedutaOrdinal)
                    End If
                    If Not reader.IsDBNull(dataRegistrazioneOrdinal) Then
                        vaccinazioneEseguita.ves_data_registrazione = reader.GetDateTime(dataRegistrazioneOrdinal)
                    End If
                    If Not reader.IsDBNull(codiceLottoOrdinal) Then
                        vaccinazioneEseguita.ves_lot_codice = reader.GetString(codiceLottoOrdinal)
                    End If
                    If Not reader.IsDBNull(codiceOperatoreOrdinal) Then
                        vaccinazioneEseguita.ves_ope_codice = reader.GetString(codiceOperatoreOrdinal)
                    End If
                    If Not reader.IsDBNull(idUtenteOrdinal) Then
                        vaccinazioneEseguita.ves_ute_id = reader.GetInt64(idUtenteOrdinal)
                    End If
                    If Not reader.IsDBNull(codiceSitoInoculazioneOrdinal) Then
                        vaccinazioneEseguita.ves_sii_codice = reader.GetString(codiceSitoInoculazioneOrdinal)
                    End If
                    If Not reader.IsDBNull(codiceNomeCommercialeOrdinal) Then
                        vaccinazioneEseguita.ves_noc_codice = reader.GetString(codiceNomeCommercialeOrdinal)
                    End If
                    If Not reader.IsDBNull(codiceAssociazioneOrdinal) Then
                        vaccinazioneEseguita.ves_ass_codice = reader.GetString(codiceAssociazioneOrdinal)
                    End If
                    If Not reader.IsDBNull(associazioneDoseOrdinal) Then
                        vaccinazioneEseguita.ves_ass_n_dose = reader.GetInt32(associazioneDoseOrdinal)
                    End If
                    If Not reader.IsDBNull(associazioneProgressivoOrdinal) Then
                        vaccinazioneEseguita.ves_ass_prog = reader.GetString(associazioneProgressivoOrdinal)
                    End If
                    If Not reader.IsDBNull(luogoOrdinal) Then
                        vaccinazioneEseguita.ves_luogo = reader.GetString(luogoOrdinal)
                    End If
                    If Not reader.IsDBNull(codiceComuneOrdinal) Then
                        vaccinazioneEseguita.ves_comune_o_stato = reader.GetString(codiceComuneOrdinal)
                    End If
                    If Not reader.IsDBNull(dataConvocazioneOrdinal) Then
                        vaccinazioneEseguita.cnv_data = reader.GetDateTime(dataConvocazioneOrdinal)
                    End If
                    If Not reader.IsDBNull(dataPrimoAppuntamentoConvocazioneOrdinal) Then
                        vaccinazioneEseguita.ves_cnv_data_primo_app = reader.GetDateTime(dataPrimoAppuntamentoConvocazioneOrdinal)
                    End If
                    If Not reader.IsDBNull(inCampagnaOrdinal) Then
                        vaccinazioneEseguita.ves_in_campagna = reader.GetString(inCampagnaOrdinal)
                    End If
                    If Not reader.IsDBNull(operatoreInAmbulatorioOrdinal) Then
                        vaccinazioneEseguita.ves_ope_in_ambulatorio = reader.GetString(operatoreInAmbulatorioOrdinal)
                    End If
                    If Not reader.IsDBNull(codiceMedicoVaccinanteOrdinal) Then
                        vaccinazioneEseguita.ves_med_vaccinante_codice = reader.GetString(codiceMedicoVaccinanteOrdinal)
                    End If
                    If Not reader.IsDBNull(esitoOrdinal) Then
                        vaccinazioneEseguita.ves_esito = reader.GetString(esitoOrdinal)
                    End If
                    If Not reader.IsDBNull(fittiziaOrdinal) Then
                        vaccinazioneEseguita.ves_flag_fittizia = reader.GetString(fittiziaOrdinal)
                    End If
                    If Not reader.IsDBNull(noteOrdinal) Then
                        vaccinazioneEseguita.ves_note = reader.GetString(noteOrdinal)
                    End If
                    If Not reader.IsDBNull(codiceConsultorioRegistrazioneOrdinal) Then
                        vaccinazioneEseguita.ves_cns_registrazione = reader.GetString(codiceConsultorioRegistrazioneOrdinal)
                    End If
                    If Not reader.IsDBNull(accessoOrdinal) Then
                        vaccinazioneEseguita.ves_accesso = reader.GetString(accessoOrdinal)
                    End If
                    If Not reader.IsDBNull(codiceAmbulatorioOrdinal) Then
                        vaccinazioneEseguita.ves_amb_codice = reader.GetInt32(codiceAmbulatorioOrdinal)
                    End If
                    If Not reader.IsDBNull(codiceViaSomministrazioneOrdinal) Then
                        vaccinazioneEseguita.ves_vii_codice = reader.GetString(codiceViaSomministrazioneOrdinal)
                    End If
                    If Not reader.IsDBNull(codiceMalattiaOrdinal) Then
                        vaccinazioneEseguita.ves_mal_codice_malattia = reader.GetString(codiceMalattiaOrdinal)
                    End If
                    If Not reader.IsDBNull(codiceEsenzioneOrdinal) Then
                        vaccinazioneEseguita.ves_codice_esenzione = reader.GetString(codiceEsenzioneOrdinal)
                    End If
                    If Not reader.IsDBNull(importoOrdinal) Then
                        vaccinazioneEseguita.ves_importo = reader.GetDecimal(importoOrdinal)
                    End If
                    If Not reader.IsDBNull(codiceUslInserimentoOrdinal) Then
                        vaccinazioneEseguita.ves_usl_inserimento = reader.GetString(codiceUslInserimentoOrdinal)
                    End If
                    If Not reader.IsDBNull(data_ultima_variazione_ordinal) Then
                        vaccinazioneEseguita.ves_data_ultima_variazione = reader.GetDateTime(data_ultima_variazione_ordinal)
                    End If
                    If Not reader.IsDBNull(ute_id_ultima_variazione_ordinal) Then
                        vaccinazioneEseguita.ves_ute_id_ultima_variazione = reader.GetInt64(ute_id_ultima_variazione_ordinal)
                    End If
                    If Not reader.IsDBNull(codiceUslScadenzaOrdinal) Then
                        vaccinazioneEseguita.ves_usl_scadenza = reader.GetString(codiceUslScadenzaOrdinal)
                    End If
                    If Not reader.IsDBNull(dataScadenzaOrdinal) Then
                        vaccinazioneEseguita.ves_data_scadenza = reader.GetDateTime(dataScadenzaOrdinal)
                    End If
                    If Not reader.IsDBNull(IdUtenteScadenzaOrdinal) Then
                        vaccinazioneEseguita.ves_ute_id_scadenza = reader.GetInt64(IdUtenteScadenzaOrdinal)
                    End If
                    If Not reader.IsDBNull(dataRipristinoOrdinal) Then
                        vaccinazioneEseguita.ves_data_ripristino = reader.GetDateTime(dataRipristinoOrdinal)
                    End If
                    If Not reader.IsDBNull(IdUtenteRipristinoOrdinal) Then
                        vaccinazioneEseguita.ves_ute_id_ripristino = reader.GetInt64(IdUtenteRipristinoOrdinal)
                    End If
                    If Not reader.IsDBNull(flagVisibilitaOrdinal) Then
                        vaccinazioneEseguita.ves_flag_visibilita_vac_centrale = reader.GetString(flagVisibilitaOrdinal)
                    End If
                    If Not reader.IsDBNull(note_acquisizione_ordinal) Then
                        vaccinazioneEseguita.ves_note_acquisizione_vac_centrale = reader.GetString(note_acquisizione_ordinal)
                    End If
                    If Not reader.IsDBNull(ves_id_acn_ordinal) Then
                        vaccinazioneEseguita.ves_id_acn = reader.GetString(ves_id_acn_ordinal)
                    End If
                    If Not reader.IsDBNull(ves_provenienza_ordinal) Then
                        vaccinazioneEseguita.ves_provenienza = reader.GetString(ves_provenienza_ordinal)
                    End If

                    If Not reader.IsDBNull(ves_mal_codice_cond_sanitaria) Then
                        vaccinazioneEseguita.ves_mal_codice_cond_sanitaria = reader.GetString(ves_mal_codice_cond_sanitaria)
                    End If
                    If Not reader.IsDBNull(ves_rsc_codice) Then
                        vaccinazioneEseguita.ves_rsc_codice = reader.GetString(ves_rsc_codice)
                    End If
                    If Not reader.IsDBNull(ves_tpa_pagamento) Then
                        vaccinazioneEseguita.ves_tpa_guid_tipi_pagamento = reader.GetGuidOrDefault(ves_tpa_pagamento)
                    End If
                    If Not reader.IsDBNull(ves_lot_data_scadenza) Then
                        vaccinazioneEseguita.ves_lot_data_scadenza = reader.GetDateTime(ves_lot_data_scadenza)
                    End If
                    If Not reader.IsDBNull(ves_tipo_associazione_acn) Then
                        vaccinazioneEseguita.ves_tipo_associazione_acn = reader.GetString(ves_tipo_associazione_acn)
                    End If

                    If deleted Then
                        vaccinazioneEseguita.DataEliminazione = reader.GetNullableDateTimeOrDefault(dataEliminazioneOrdinal)
                        vaccinazioneEseguita.IdUtenteEliminazione = reader.GetNullableInt64OrDefault(IdUtenteEliminazioneOrdinal)
                    End If

                    vaccinazioneEseguitaList.Add(vaccinazioneEseguita)

                End While

            End If

            Return vaccinazioneEseguitaList

        End Function

        Private Function InsertVaccinazioneEseguita(vaccinazioneEseguita As VaccinazioneEseguita, dbTableName As String, dbFieldPrefix As String, deleted As Boolean) As Boolean

            If Not vaccinazioneEseguita.ves_id.HasValue Then

                ' N.B. : senza cancellazione parametri dà errore perchè potrebbero essere rimasti parametri nell'OracleCommand.
                '        L'errore si presenta solo se la query è scritta direttamente nell'istruzione e non costruita col dam.qb
                _DAM.ClearParam()

                vaccinazioneEseguita.ves_id = Convert.ToInt64(_DAM.ExecScalar(Queries.VaccinazioniEseguite.OracleQueries.selNextSeqEseguita))

            End If

            With _DAM.QB

                .NewQuery()

                .AddTables(dbTableName)

                ' Campi not null
                .AddInsertField(dbFieldPrefix + "_PAZ_CODICE", vaccinazioneEseguita.paz_codice, DataTypes.Numero)
                .AddInsertField(dbFieldPrefix + "_VAC_CODICE", vaccinazioneEseguita.ves_vac_codice, DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_N_RICHIAMO", vaccinazioneEseguita.ves_n_richiamo, DataTypes.Numero)
                .AddInsertField(dbFieldPrefix + "_DATA_EFFETTUAZIONE", vaccinazioneEseguita.ves_data_effettuazione, DataTypes.Data)
                .AddInsertField(dbFieldPrefix + "_DATAORA_EFFETTUAZIONE", vaccinazioneEseguita.ves_dataora_effettuazione, DataTypes.DataOra)
                .AddInsertField(dbFieldPrefix + "_ID", vaccinazioneEseguita.ves_id, DataTypes.Numero)

                If Not String.IsNullOrEmpty(vaccinazioneEseguita.ves_flag_fittizia) Then
                    .AddInsertField(dbFieldPrefix + "_FLAG_FITTIZIA", vaccinazioneEseguita.ves_flag_fittizia, DataTypes.Stringa)
                End If

                .AddInsertField(dbFieldPrefix + "_VII_CODICE", GetStringParam(vaccinazioneEseguita.ves_vii_codice), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_SII_CODICE", GetStringParam(vaccinazioneEseguita.ves_sii_codice), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_LUOGO", GetStringParam(vaccinazioneEseguita.ves_luogo), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_OPE_CODICE", GetStringParam(vaccinazioneEseguita.ves_ope_codice), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_MED_VACCINANTE", GetStringParam(vaccinazioneEseguita.ves_med_vaccinante_codice), DataTypes.Stringa)

                ' Campi riservati al primo inserimento
                .AddInsertField(dbFieldPrefix + "_DATA_REGISTRAZIONE", GetDateParam(vaccinazioneEseguita.ves_data_registrazione), DataTypes.DataOra)
                .AddInsertField(dbFieldPrefix + "_UTE_ID", GetLongParam(vaccinazioneEseguita.ves_ute_id), DataTypes.Numero)

                ' Campi ultima variazione
                .AddInsertField(dbFieldPrefix + "_DATA_ULTIMA_VARIAZIONE", GetDateParam(vaccinazioneEseguita.ves_data_ultima_variazione), DataTypes.DataOra)
                .AddInsertField(dbFieldPrefix + "_UTE_ID_ULTIMA_VARIAZIONE", GetLongParam(vaccinazioneEseguita.ves_ute_id_ultima_variazione), DataTypes.Numero)

                .AddInsertField(dbFieldPrefix + "_NOC_CODICE", GetStringParam(vaccinazioneEseguita.ves_noc_codice), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_LOT_CODICE", GetStringParam(vaccinazioneEseguita.ves_lot_codice), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_N_SEDUTA", GetIntParam(vaccinazioneEseguita.ves_n_seduta), DataTypes.Numero)
                .AddInsertField(dbFieldPrefix + "_CIC_CODICE", GetStringParam(vaccinazioneEseguita.ves_cic_codice), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_ASS_CODICE", GetStringParam(vaccinazioneEseguita.ves_ass_codice), DataTypes.Stringa)

                If vaccinazioneEseguita.ves_ass_n_dose > 0 Then
                    .AddInsertField(dbFieldPrefix + "_ASS_N_DOSE", vaccinazioneEseguita.ves_ass_n_dose, DataTypes.Numero)
                End If

                .AddInsertField(dbFieldPrefix + "_STATO", GetStringParam(vaccinazioneEseguita.ves_stato), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_COMUNE_O_STATO", GetStringParam(vaccinazioneEseguita.ves_comune_o_stato), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_CNV_DATA", GetDateParam(vaccinazioneEseguita.cnv_data), DataTypes.Data)
                .AddInsertField(dbFieldPrefix + "_IN_CAMPAGNA", GetStringParam(vaccinazioneEseguita.ves_in_campagna), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_OPE_IN_AMBULATORIO", GetStringParam(vaccinazioneEseguita.ves_ope_in_ambulatorio), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_CNS_CODICE", GetStringParam(vaccinazioneEseguita.ves_cns_codice), DataTypes.Stringa)

                If vaccinazioneEseguita.ves_paz_codice_old > 0 Then
                    .AddInsertField(dbFieldPrefix + "_PAZ_CODICE_OLD", GetIntParam(vaccinazioneEseguita.ves_paz_codice_old), DataTypes.Numero)
                End If

                .AddInsertField(dbFieldPrefix + "_ESITO", GetStringParam(vaccinazioneEseguita.ves_esito), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_NOTE", GetStringParam(vaccinazioneEseguita.ves_note), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_CNS_REGISTRAZIONE", GetStringParam(vaccinazioneEseguita.ves_cns_registrazione), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_ACCESSO", GetStringParam(vaccinazioneEseguita.ves_accesso), DataTypes.Stringa)

                If vaccinazioneEseguita.ves_amb_codice > 0 Then
                    .AddInsertField(dbFieldPrefix + "_AMB_CODICE", vaccinazioneEseguita.ves_amb_codice, DataTypes.Numero)
                End If

                .AddInsertField(dbFieldPrefix + "_IMPORTO", GetDecimalParam(vaccinazioneEseguita.ves_importo), DataTypes.Double)
                .AddInsertField(dbFieldPrefix + "_MAL_CODICE_MALATTIA", GetStringParam(vaccinazioneEseguita.ves_mal_codice_malattia), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_CODICE_ESENZIONE", GetStringParam(vaccinazioneEseguita.ves_codice_esenzione), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_CNV_DATA_PRIMO_APP", GetDateParam(vaccinazioneEseguita.ves_cnv_data_primo_app), DataTypes.DataOra)
                .AddInsertField(dbFieldPrefix + "_ASS_PROG", GetStringParam(vaccinazioneEseguita.ves_ass_prog), DataTypes.Stringa)

                .AddInsertField(dbFieldPrefix + "_USL_INSERIMENTO", GetStringParam(vaccinazioneEseguita.ves_usl_inserimento), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_USL_SCADENZA", GetStringParam(vaccinazioneEseguita.ves_usl_scadenza), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_DATA_SCADENZA", GetDateParam(vaccinazioneEseguita.ves_data_scadenza), DataTypes.DataOra)
                .AddInsertField(dbFieldPrefix + "_UTE_ID_SCADENZA", GetLongParam(vaccinazioneEseguita.ves_ute_id_scadenza), DataTypes.Numero)
                .AddInsertField(dbFieldPrefix + "_DATA_RIPRISTINO", GetDateParam(vaccinazioneEseguita.ves_data_ripristino), DataTypes.DataOra)
                .AddInsertField(dbFieldPrefix + "_UTE_ID_RIPRISTINO", GetLongParam(vaccinazioneEseguita.ves_ute_id_ripristino), DataTypes.Numero)
                .AddInsertField(dbFieldPrefix + "_FLAG_VISIBILITA", GetStringParam(vaccinazioneEseguita.ves_flag_visibilita_vac_centrale), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_NOTE_ACQUISIZIONE", GetStringParam(vaccinazioneEseguita.ves_note_acquisizione_vac_centrale), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_ID_ACN", GetStringParam(vaccinazioneEseguita.ves_id_acn), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_PROVENIENZA", GetStringParam(vaccinazioneEseguita.ves_provenienza), DataTypes.Stringa)

                If deleted Then
                    If vaccinazioneEseguita.DataEliminazione.HasValue Then
                        .AddInsertField(dbFieldPrefix + "_DATA_ELIMINAZIONE", vaccinazioneEseguita.DataEliminazione.Value, DataTypes.DataOra)
                    End If

                    If vaccinazioneEseguita.IdUtenteEliminazione.HasValue Then
                        .AddInsertField(dbFieldPrefix + "_UTE_ID_ELIMINAZIONE", vaccinazioneEseguita.IdUtenteEliminazione.Value, DataTypes.Numero)
                    End If
                End If

                _DAM.QB.AddInsertField(dbFieldPrefix + "_MAL_CODICE_COND_SANITARIA", GetStringParam(vaccinazioneEseguita.ves_mal_codice_cond_sanitaria), DataTypes.Stringa)
                _DAM.QB.AddInsertField(dbFieldPrefix + "_RSC_CODICE", GetStringParam(vaccinazioneEseguita.ves_rsc_codice), DataTypes.Stringa)
                If (vaccinazioneEseguita.ves_tpa_guid_tipi_pagamento = Guid.Empty) Then
                    _DAM.QB.AddInsertField(dbFieldPrefix + "_TPA_GUID_TIPI_PAGAMENTO", DBNull.Value, DataTypes.Binary)
                Else
                    _DAM.QB.AddInsertField(dbFieldPrefix + "_TPA_GUID_TIPI_PAGAMENTO", vaccinazioneEseguita.ves_tpa_guid_tipi_pagamento.ToByteArray(), DataTypes.Binary)
                End If
                If (vaccinazioneEseguita.ves_lot_data_scadenza.HasValue) Then
                    .AddInsertField(dbFieldPrefix + "_LOT_DATA_SCADENZA", GetDateParam(vaccinazioneEseguita.ves_lot_data_scadenza.Value), DataTypes.Data)
                End If
                If (Not String.IsNullOrWhiteSpace(vaccinazioneEseguita.ves_antigene)) Then
                    .AddInsertField(dbFieldPrefix + "_ANTIGENE", GetStringParam(vaccinazioneEseguita.ves_antigene), DataTypes.Stringa)
                End If
                .AddInsertField(dbFieldPrefix + "_TIPO_EROGATORE", GetStringParam(vaccinazioneEseguita.ves_tipo_erogatore), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_TIPO_ASSOCIAZIONE_ACN", GetStringParam(vaccinazioneEseguita.ves_tipo_associazione_acn), DataTypes.Stringa)


                .AddInsertField(dbFieldPrefix + "_CODICE_STRUTTURA", GetStringParam(vaccinazioneEseguita.ves_codice_struttura), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_USL_COD_SOMMINISTRAZIONE", GetStringParam(vaccinazioneEseguita.ves_usl_cod_somministrazione), DataTypes.Stringa)

            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Insert) > 0

        End Function

        Private Function UpdateVaccinazioneEseguita(vaccinazioneEseguita As VaccinazioneEseguita, dbTableName As String, dbFieldPrefix As String) As Boolean

            _DAM.QB.NewQuery()

            _DAM.QB.AddTables(dbTableName)

            _DAM.QB.AddUpdateField(dbFieldPrefix + "_VAC_CODICE", vaccinazioneEseguita.ves_vac_codice, DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_DATA_EFFETTUAZIONE", vaccinazioneEseguita.ves_data_effettuazione, DataTypes.Data)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_DATAORA_EFFETTUAZIONE", vaccinazioneEseguita.ves_dataora_effettuazione, DataTypes.DataOra)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_N_RICHIAMO", vaccinazioneEseguita.ves_n_richiamo, DataTypes.Numero)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_PAZ_CODICE", vaccinazioneEseguita.paz_codice, DataTypes.Numero)
            If vaccinazioneEseguita.ves_paz_codice_old > 0 Then
                _DAM.QB.AddUpdateField(dbFieldPrefix + "_PAZ_CODICE_OLD", GetIntParam(vaccinazioneEseguita.ves_paz_codice_old), DataTypes.Numero)
            Else
                _DAM.QB.AddUpdateField(dbFieldPrefix + "_PAZ_CODICE_OLD", DBNull.Value, DataTypes.Numero)
            End If
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_STATO", GetStringParam(vaccinazioneEseguita.ves_stato), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_CIC_CODICE", GetStringParam(vaccinazioneEseguita.ves_cic_codice), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_N_SEDUTA", GetIntParam(vaccinazioneEseguita.ves_n_seduta), DataTypes.Numero)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_DATA_REGISTRAZIONE", GetDateParam(vaccinazioneEseguita.ves_data_registrazione), DataTypes.DataOra)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_LOT_CODICE", GetStringParam(vaccinazioneEseguita.ves_lot_codice), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_UTE_ID", GetLongParam(vaccinazioneEseguita.ves_ute_id), DataTypes.Numero)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_ASS_CODICE", GetStringParam(vaccinazioneEseguita.ves_ass_codice), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_ASS_PROG", GetStringParam(vaccinazioneEseguita.ves_ass_prog), DataTypes.Stringa)
            If vaccinazioneEseguita.ves_ass_n_dose > 0 Then
                _DAM.QB.AddUpdateField(dbFieldPrefix + "_ASS_N_DOSE", GetIntParam(vaccinazioneEseguita.ves_ass_n_dose), DataTypes.Numero)
            Else
                _DAM.QB.AddUpdateField(dbFieldPrefix + "_ASS_N_DOSE", DBNull.Value, DataTypes.Numero)
            End If

            _DAM.QB.AddUpdateField(dbFieldPrefix + "_CNV_DATA", GetDateParam(vaccinazioneEseguita.cnv_data), DataTypes.Data)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_CNV_DATA_PRIMO_APP", GetDateParam(vaccinazioneEseguita.ves_cnv_data_primo_app), DataTypes.DataOra)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_IN_CAMPAGNA", GetStringParam(vaccinazioneEseguita.ves_in_campagna), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_ESITO", GetStringParam(vaccinazioneEseguita.ves_esito), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_FLAG_FITTIZIA", GetStringParam(vaccinazioneEseguita.ves_flag_fittizia), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_CNS_REGISTRAZIONE", GetStringParam(vaccinazioneEseguita.ves_cns_registrazione), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_ACCESSO", GetStringParam(vaccinazioneEseguita.ves_accesso), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_MAL_CODICE_MALATTIA", GetStringParam(vaccinazioneEseguita.ves_mal_codice_malattia), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_CODICE_ESENZIONE", GetStringParam(vaccinazioneEseguita.ves_codice_esenzione), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_IMPORTO", GetDecimalParam(vaccinazioneEseguita.ves_importo), DataTypes.Double)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_VII_CODICE", GetStringParam(vaccinazioneEseguita.ves_vii_codice), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_SII_CODICE", GetStringParam(vaccinazioneEseguita.ves_sii_codice), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_LUOGO", GetStringParam(vaccinazioneEseguita.ves_luogo), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_OPE_CODICE", GetStringParam(vaccinazioneEseguita.ves_ope_codice), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_MED_VACCINANTE", GetStringParam(vaccinazioneEseguita.ves_med_vaccinante_codice), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_NOC_CODICE", GetStringParam(vaccinazioneEseguita.ves_noc_codice), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_OPE_IN_AMBULATORIO", GetStringParam(vaccinazioneEseguita.ves_ope_in_ambulatorio), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_COMUNE_O_STATO", GetStringParam(vaccinazioneEseguita.ves_comune_o_stato), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_CNS_CODICE", GetStringParam(vaccinazioneEseguita.ves_cns_codice), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_NOTE", GetStringParam(vaccinazioneEseguita.ves_note), DataTypes.Stringa)
            If vaccinazioneEseguita.ves_amb_codice > 0 Then
                _DAM.QB.AddUpdateField(dbFieldPrefix + "_AMB_CODICE", GetIntParam(vaccinazioneEseguita.ves_amb_codice), DataTypes.Numero)
            Else
                _DAM.QB.AddUpdateField(dbFieldPrefix + "_AMB_CODICE", DBNull.Value, DataTypes.Numero)
            End If
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_DATA_ULTIMA_VARIAZIONE", GetDateParam(vaccinazioneEseguita.ves_data_ultima_variazione), DataTypes.DataOra)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_UTE_ID_ULTIMA_VARIAZIONE", GetLongParam(vaccinazioneEseguita.ves_ute_id_ultima_variazione), DataTypes.Numero)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_USL_INSERIMENTO", GetStringParam(vaccinazioneEseguita.ves_usl_inserimento), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_USL_SCADENZA", GetStringParam(vaccinazioneEseguita.ves_usl_scadenza), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_DATA_SCADENZA", GetDateParam(vaccinazioneEseguita.ves_data_scadenza), DataTypes.DataOra)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_UTE_ID_SCADENZA", GetLongParam(vaccinazioneEseguita.ves_ute_id_scadenza), DataTypes.Numero)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_DATA_RIPRISTINO", GetDateParam(vaccinazioneEseguita.ves_data_ripristino), DataTypes.DataOra)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_UTE_ID_RIPRISTINO", GetLongParam(vaccinazioneEseguita.ves_ute_id_ripristino), DataTypes.Numero)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_FLAG_VISIBILITA", GetStringParam(vaccinazioneEseguita.ves_flag_visibilita_vac_centrale), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_NOTE_ACQUISIZIONE", GetStringParam(vaccinazioneEseguita.ves_note_acquisizione_vac_centrale), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_ID_ACN", GetStringParam(vaccinazioneEseguita.ves_id_acn), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_PROVENIENZA", GetStringParam(vaccinazioneEseguita.ves_provenienza), DataTypes.Stringa)

            _DAM.QB.AddUpdateField(dbFieldPrefix + "_MAL_CODICE_COND_SANITARIA", GetStringParam(vaccinazioneEseguita.ves_mal_codice_cond_sanitaria), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_RSC_CODICE", GetStringParam(vaccinazioneEseguita.ves_rsc_codice), DataTypes.Stringa)
            If (vaccinazioneEseguita.ves_tpa_guid_tipi_pagamento = Guid.Empty) Then
                _DAM.QB.AddUpdateField(dbFieldPrefix + "_TPA_GUID_TIPI_PAGAMENTO", DBNull.Value, DataTypes.Binary)
            Else
                _DAM.QB.AddUpdateField(dbFieldPrefix + "_TPA_GUID_TIPI_PAGAMENTO", vaccinazioneEseguita.ves_tpa_guid_tipi_pagamento.ToByteArray(), DataTypes.Binary)
            End If
            If (vaccinazioneEseguita.ves_lot_data_scadenza.HasValue) Then
                _DAM.QB.AddUpdateField(dbFieldPrefix + "_LOT_DATA_SCADENZA", GetDateParam(vaccinazioneEseguita.ves_lot_data_scadenza), DataTypes.Data)
            Else
                _DAM.QB.AddUpdateField(dbFieldPrefix + "_LOT_DATA_SCADENZA", DBNull.Value, DataTypes.Data)
            End If

            _DAM.QB.AddUpdateField(dbFieldPrefix + "_TIPO_EROGATORE", GetStringParam(vaccinazioneEseguita.ves_tipo_erogatore), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_TIPO_ASSOCIAZIONE_ACN", GetStringParam(vaccinazioneEseguita.ves_tipo_associazione_acn), DataTypes.Stringa)

            _DAM.QB.AddUpdateField(dbFieldPrefix + "_CODICE_STRUTTURA", GetStringParam(vaccinazioneEseguita.ves_codice_struttura), DataTypes.Stringa)
            _DAM.QB.AddUpdateField(dbFieldPrefix + "_USL_COD_SOMMINISTRAZIONE", GetStringParam(vaccinazioneEseguita.ves_usl_cod_somministrazione), DataTypes.Stringa)

            _DAM.QB.AddWhereCondition(dbFieldPrefix + "_ID", Comparatori.Uguale, vaccinazioneEseguita.ves_id, DataTypes.Numero)

            Return _DAM.ExecNonQuery(ExecQueryType.Update) > 0

        End Function

        ''' <summary>
        ''' Update flag di visibilità. Vengono modificati anche utente e data di variazione.
        ''' </summary>
        ''' <param name="idVaccinazioneEseguita"></param>
        ''' <param name="flagVisibilita"></param>
        ''' <param name="idUtenteVariazione"></param>
        ''' <param name="dataVariazione"></param>
        ''' <returns></returns>
        Public Function UpdateFlagVisibilitaEseguite(idVaccinazioneEseguita As Long, flagVisibilita As String, idUtenteVariazione As Long, dataVariazione As Date) As Integer Implements IVaccinazioniEseguiteProvider.UpdateFlagVisibilitaEseguite

            Dim count As Integer = 0

            Using cmd As OracleCommand = Connection.CreateCommand()

                cmd.CommandText =
                    " update t_vac_eseguite set " +
                    " ves_flag_visibilita = :ves_flag_visibilita, " +
                    " ves_ute_id_ultima_variazione = :ves_ute_id_ultima_variazione, " +
                    " ves_data_ultima_variazione = :ves_data_ultima_variazione " +
                    " where ves_id = :ves_id "

                cmd.Parameters.AddWithValue("ves_flag_visibilita", flagVisibilita)
                cmd.Parameters.AddWithValue("ves_ute_id_ultima_variazione", idUtenteVariazione)
                cmd.Parameters.AddWithValue("ves_data_ultima_variazione", dataVariazione)
                cmd.Parameters.AddWithValue("ves_id", idVaccinazioneEseguita)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    count = cmd.ExecuteNonQuery()

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Update flag di visibilità. Vengono modificati anche utente e data di variazione.
        ''' </summary>
        ''' <param name="idVaccinazioneScaduta"></param>
        ''' <param name="flagVisibilita"></param>
        ''' <param name="idUtenteVariazione"></param>
        ''' <param name="dataVariazione"></param>
        ''' <returns></returns>
        Public Function UpdateFlagVisibilitaScadute(idVaccinazioneScaduta As Long, flagVisibilita As String, idUtenteVariazione As Long, dataVariazione As Date) As Integer Implements IVaccinazioniEseguiteProvider.UpdateFlagVisibilitaScadute

            Dim count As Integer = 0

            Using cmd As OracleCommand = Connection.CreateCommand()

                cmd.CommandText =
                    " update t_vac_scadute set " +
                    " vsc_flag_visibilita = :vsc_flag_visibilita, " +
                    " vsc_ute_id_ultima_variazione = :vsc_ute_id_ultima_variazione, " +
                    " vsc_data_ultima_variazione = :vsc_data_ultima_variazione " +
                    " where vsc_id = :vsc_id "

                cmd.Parameters.AddWithValue("vsc_flag_visibilita", flagVisibilita)
                cmd.Parameters.AddWithValue("vsc_ute_id_ultima_variazione", idUtenteVariazione)
                cmd.Parameters.AddWithValue("vsc_data_ultima_variazione", dataVariazione)
                cmd.Parameters.AddWithValue("vsc_id", idVaccinazioneScaduta)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    count = cmd.ExecuteNonQuery()

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        Private Function GetQueryIdVaccinazioniEseguite(cmd As OracleClient.OracleCommand, query As String, listIdVaccinazioniEseguite As List(Of Int64)) As String

            Dim filtroId As New System.Text.StringBuilder()

            For i As Integer = 0 To listIdVaccinazioniEseguite.Count - 1

                Dim paramName As String = String.Format("param{0}", i.ToString())

                cmd.Parameters.AddWithValue(paramName, listIdVaccinazioniEseguite(i))

                filtroId.AppendFormat(":{0},", paramName)

            Next

            If filtroId.Length > 0 Then filtroId.Remove(filtroId.Length - 1, 1)

            Return String.Format(query, filtroId.ToString())

        End Function

        Private Function GetCodiciLottiReazioneByData(codicePaziente As Integer, data As DateTime, usaDataEffettuazione As Boolean) As List(Of String)

            Dim listCodiciLotti As List(Of String) = Nothing

            With _DAM.QB

                .NewQuery()

                .AddSelectFields("distinct vra_lot_codice")
                .AddTables("t_sta_reazioni_avverse")
                .AddWhereCondition("vra_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                If usaDataEffettuazione Then
                    .AddWhereCondition("vra_res_data_effettuazione", Comparatori.Uguale, data.Date, DataTypes.Data)
                Else
                    .AddWhereCondition("vra_data_reazione", Comparatori.Uguale, data, DataTypes.Data)
                End If

            End With

            Using idr As IDataReader = _DAM.BuildDataReader()

                If Not idr Is Nothing Then

                    listCodiciLotti = New List(Of String)

                    Dim codice As String

                    While idr.Read()

                        codice = idr.GetStringOrDefault(0)

                        If Not String.IsNullOrEmpty(codice) Then listCodiciLotti.Add(codice)

                    End While

                End If

            End Using

            Return listCodiciLotti

        End Function

#Region " Reazione Avversa "

        Private Function GetReazioneAvversaListFromDataReader(reader As OracleClient.OracleDataReader, dbFieldPrefix As String, idVaccinazioneDbFieldPrefix As String, deleted As Boolean) As List(Of ReazioneAvversa)

            Dim reazioneAvversaList As New List(Of ReazioneAvversa)

            If reader.HasRows Then

                Dim PAZ_CODICE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_PAZ_CODICE")
                Dim VAC_CODICE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_VAC_CODICE")
                Dim N_RICHIAMO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_N_RICHIAMO")
                Dim RES_DATA_EFFETTUAZIONE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_RES_DATA_EFFETTUAZIONE")
                Dim REA_CODICE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_REA_CODICE")
                Dim DATA_REAZIONE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_DATA_REAZIONE")
                Dim VISITA_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_VISITA")
                Dim TERAPIA_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_TERAPIA")
                Dim ESI_CODICE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_ESI_CODICE")
                Dim RE1_CODICE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_RE1_CODICE")
                Dim RE2_CODICE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_RE2_CODICE")
                Dim PAZ_CODICE_OLD_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_PAZ_CODICE_OLD")
                'Dim NOC_CODICE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_NOC_CODICE")
                Dim NOC_DESCRIZIONE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_NOC_DESCRIZIONE")
                Dim LOT_CODICE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_LOT_CODICE")
                Dim LOT_DATA_SCADENZA_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_LOT_DATA_SCADENZA")
                Dim DOSAGGIO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_DOSAGGIO")
                Dim SOMMINISTRAZIONE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_SOMMINISTRAZIONE")
                Dim ORA_EFFETTUAZIONE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_ORA_EFFETTUAZIONE")
                Dim SII_CODICE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_SII_CODICE")
                Dim SOSPESO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_SOSPESO")
                Dim MIGLIORATA_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_MIGLIORATA")
                Dim RIPRESO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_RIPRESO")
                Dim RICOMPARSA_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_RICOMPARSA")
                Dim INDICAZIONI_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_INDICAZIONI")
                Dim CODICEINDICAZIONI_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_NOI_CODICE_INDICAZIONI")
                Dim RICHIAMO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_RICHIAMO")
                Dim LUOGO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_LUOGO")
                Dim FARMACO_CONCOMITANTE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_FARMACO_CONCOMITANTE")
                Dim FARMACO_DESCRIZIONE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_FARMACO_DESCRIZIONE")
                Dim USO_CONCOMITANTE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_USO_CONCOMITANTE")
                Dim CONDIZIONI_CONCOMITANTI_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_CONDIZIONI_CONCOMITANTI")
                Dim QUALIFICA_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_QUALIFICA")
                Dim ALTRA_QUALIFICA_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_ALTRA_QUALIFICA")
                Dim ALTRO_LUOGO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_ALTRO_LUOGO")
                Dim COGNOME_SEGNALATORE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_COGNOME_SEGNALATORE")
                Dim NOME_SEGNALATORE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_NOME_SEGNALATORE")
                Dim INDIRIZZO_SEGNALATORE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_INDIRIZZO_SEGNALATORE")
                Dim TEL_SEGNALATORE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_TEL_SEGNALATORE")
                Dim FAX_SEGNALATORE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_FAX_SEGNALATORE")
                Dim MAIL_SEGNALATORE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_MAIL_SEGNALATORE")
                Dim DATA_ESITO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_DATA_ESITO")
                Dim DATA_COMPILAZIONE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_DATA_COMPILAZIONE")
                Dim REA_ALTRO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_REA_ALTRO")
                Dim GRAVITA_REAZIONE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_GRAVITA_REAZIONE")
                Dim GRAVE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_GRAVE")
                Dim ESITO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_ESITO")
                Dim MOTIVO_DECESSO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_MOTIVO_DECESSO")
                Dim UTE_ID_COMPILAZIONE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_UTE_ID_COMPILAZIONE")
                Dim USL_INSERIMENTO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_USL_INSERIMENTO")
                Dim DATA_VARIAZIONE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_DATA_VARIAZIONE")
                Dim UTE_ID_VARIAZIONE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_UTE_ID_VARIAZIONE")
                Dim VES_ID_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_" + idVaccinazioneDbFieldPrefix + "_ID")
                Dim ID_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_ID")

                Dim ALTRE_INFORMAZIONI_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_altre_informazioni")
                Dim AMBITO_OSSERVAZIONE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_ambito_osservazione")
                Dim AMBITO_STUDIO_TITOLO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_ambito_studio_titolo")
                Dim AMBITO_STUDIO_TIPOLOGIA_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_ambito_studio_tipologia")
                Dim AMBITO_STUDIO_NUMERO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_ambito_studio_numero")
                Dim PESO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_peso")
                Dim ALTEZZA_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_altezza")
                Dim DATA_ULTIMA_MESTRUAZIONE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_data_ultima_mestruazione")
                Dim ALLATTAMENTO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_allattamento")
                Dim GRAVIDANZA_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_gravidanza")
                Dim CAUSA_OSSERVATA_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_causa_osservata")
                Dim FARMCONC1_NOC_CODICE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc1_noc_codice")
                Dim FARMCONC1_NOC_DESCRIZIONE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc1_noc_descrizione")
                Dim FARMCONC1_LOT_CODICE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc1_lot_codice")
                Dim FARMCONC1_DATAORA_EFF_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc1_dataora_eff")
                Dim FARMCONC1_DOSE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc1_dose")
                Dim FARMCONC1_SII_CODICE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc1_sii_codice")
                Dim FARMCONC1_VII_CODICE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc1_vii_codice")
                Dim FARMCONC1_SOSPESO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc1_sospeso")
                Dim FARMCONC1_MIGLIORATA_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc1_migliorata")
                Dim FARMCONC1_RIPRESO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc1_ripreso")
                Dim FARMCONC1_RICOMPARSA_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc1_ricomparsa")
                Dim FARMCONC1_INDICAZIONI_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc1_indicazioni")
                Dim FARMCONC1_COD_INDICAZIONI_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc1_noi_cod_indic")
                Dim FARMCONC1_DOSAGGIO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc1_dosaggio")
                Dim FARMCONC1_RICHIAMO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc1_richiamo")
                Dim FARMCONC2_NOC_CODICE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc2_noc_codice")
                Dim FARMCONC2_NOC_DESCRIZIONE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc2_noc_descrizione")
                Dim FARMCONC2_LOT_CODICE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc2_lot_codice")
                Dim FARMCONC2_DATAORA_EFF_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc2_dataora_eff")
                Dim FARMCONC2_DOSE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc2_dose")
                Dim FARMCONC2_SII_CODICE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc2_sii_codice")
                Dim FARMCONC2_VII_CODICE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc2_vii_codice")
                Dim FARMCONC2_SOSPESO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc2_sospeso")
                Dim FARMCONC2_MIGLIORATA_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc2_migliorata")
                Dim FARMCONC2_RIPRESO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc2_ripreso")
                Dim FARMCONC2_RICOMPARSA_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc2_ricomparsa")
                Dim FARMCONC2_INDICAZIONI_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc2_indicazioni")
                Dim FARMCONC2_COD_INDICAZIONI_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc2_noi_cod_indic")
                Dim FARMCONC2_DOSAGGIO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc2_dosaggio")
                Dim FARMCONC2_RICHIAMO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_farmconc2_richiamo")
                Dim FIRMA_SEGNALATORE_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_firma_segnalatore")
                Dim CODICEORIGINEETNICO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_OET_CODICE")
                Dim FARMCONC1_LOT_DATA_SCADENZA_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_FARMCONC1_LOT_DATA_SCAD")
                Dim FARMCONC2_LOT_DATA_SCADENZA_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_FARMCONC2_LOT_DATA_SCAD")
                Dim ID_SCHEDA_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_id_scheda")
                Dim SEGNALAZIONE_ID_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_segnalazione_id")

                Dim UTENTE_ID_INVIO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_ute_id_invio")
                Dim DATA_INVIO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_data_invio")
                Dim FLAG_INVIO_ordinal As Integer = reader.GetOrdinal(dbFieldPrefix + "_flag_inviato")


                Dim DATA_ELIMINAZIONE_ordinal As Integer
                Dim UTE_ID_ELIMINAZIONE_ordinal As Integer

                If deleted Then
                    DATA_ELIMINAZIONE_ordinal = reader.GetOrdinal(dbFieldPrefix + "_DATA_ELIMINAZIONE")
                    UTE_ID_ELIMINAZIONE_ordinal = reader.GetOrdinal(dbFieldPrefix + "_UTE_ID_ELIMINAZIONE")
                End If

                While reader.Read()

                    Dim reazioneAvversa As New ReazioneAvversa()

                    reazioneAvversa.CodicePaziente = reader.GetInt64(PAZ_CODICE_ordinal)
                    reazioneAvversa.CodiceVaccinazione = reader.GetStringOrDefault(VAC_CODICE_ordinal)
                    reazioneAvversa.NumeroRichiamo = reader.GetInt32(N_RICHIAMO_ordinal)
                    reazioneAvversa.DataEffettuazione = reader.GetDateTime(RES_DATA_EFFETTUAZIONE_ordinal)
                    reazioneAvversa.CodiceReazione = reader.GetStringOrDefault(REA_CODICE_ordinal)
                    reazioneAvversa.CodiceReazione1 = reader.GetStringOrDefault(RE1_CODICE_ordinal)
                    reazioneAvversa.CodiceReazione2 = reader.GetStringOrDefault(RE2_CODICE_ordinal)
                    reazioneAvversa.DataReazione = reader.GetDateTimeOrDefault(DATA_REAZIONE_ordinal)
                    reazioneAvversa.VisiteRicoveri = reader.GetStringOrDefault(VISITA_ordinal)
                    reazioneAvversa.Terapie = reader.GetStringOrDefault(TERAPIA_ordinal)
                    reazioneAvversa.CodiceEsito = reader.GetStringOrDefault(ESI_CODICE_ordinal)
                    reazioneAvversa.CodicePazientePrecedente = reader.GetNullableInt64OrDefault(PAZ_CODICE_OLD_ordinal)
                    reazioneAvversa.DescrizioneNomeCommerciale = reader.GetStringOrDefault(NOC_DESCRIZIONE_ordinal)
                    reazioneAvversa.CodiceLotto = reader.GetStringOrDefault(LOT_CODICE_ordinal)
                    reazioneAvversa.Dosaggio = reader.GetStringOrDefault(DOSAGGIO_ordinal)
                    reazioneAvversa.OraEffettuazione = reader.GetStringOrDefault(ORA_EFFETTUAZIONE_ordinal)
                    reazioneAvversa.CodiceSitoInoculazione = reader.GetStringOrDefault(SII_CODICE_ordinal)
                    reazioneAvversa.Sospeso = reader.GetStringOrDefault(SOSPESO_ordinal)
                    reazioneAvversa.Migliorata = reader.GetStringOrDefault(MIGLIORATA_ordinal)
                    reazioneAvversa.Ripreso = reader.GetStringOrDefault(RIPRESO_ordinal)
                    reazioneAvversa.Ricomparsa = reader.GetStringOrDefault(RICOMPARSA_ordinal)
                    reazioneAvversa.Indicazioni = reader.GetStringOrDefault(INDICAZIONI_ordinal)
                    reazioneAvversa.CodiceIndicazioni = reader.GetStringOrDefault(CODICEINDICAZIONI_ordinal)
                    reazioneAvversa.Richiamo = reader.GetInt32OrDefault(RICHIAMO_ordinal)
                    reazioneAvversa.FarmacoConcomitante = reader.GetStringOrDefault(FARMACO_CONCOMITANTE_ordinal)
                    reazioneAvversa.UsoConcomitante = reader.GetStringOrDefault(USO_CONCOMITANTE_ordinal)
                    reazioneAvversa.CondizioniConcomitanti = reader.GetStringOrDefault(CONDIZIONI_CONCOMITANTI_ordinal)
                    reazioneAvversa.Qualifica = reader.GetStringOrDefault(QUALIFICA_ordinal)
                    reazioneAvversa.AltraQualifica = reader.GetStringOrDefault(ALTRA_QUALIFICA_ordinal)

                    reazioneAvversa.CognomeSegnalatore = reader.GetStringOrDefault(COGNOME_SEGNALATORE_ordinal)
                    reazioneAvversa.NomeSegnalatore = reader.GetStringOrDefault(NOME_SEGNALATORE_ordinal)
                    reazioneAvversa.IndirizzoSegnalatore = reader.GetStringOrDefault(INDIRIZZO_SEGNALATORE_ordinal)
                    reazioneAvversa.FaxSegnalatore = reader.GetStringOrDefault(FAX_SEGNALATORE_ordinal)
                    reazioneAvversa.MailSegnalatore = reader.GetStringOrDefault(MAIL_SEGNALATORE_ordinal)

                    reazioneAvversa.DataEsito = reader.GetDateTimeOrDefault(DATA_ESITO_ordinal)
                    reazioneAvversa.DataCompilazione = reader.GetDateTimeOrDefault(DATA_COMPILAZIONE_ordinal)
                    reazioneAvversa.AltraReazione = reader.GetStringOrDefault(REA_ALTRO_ordinal)
                    reazioneAvversa.GravitaReazione = reader.GetStringOrDefault(GRAVITA_REAZIONE_ordinal)
                    reazioneAvversa.Grave = reader.GetStringOrDefault(GRAVE_ordinal)
                    reazioneAvversa.Esito = reader.GetStringOrDefault(ESITO_ordinal)
                    reazioneAvversa.MotivoDecesso = reader.GetStringOrDefault(MOTIVO_DECESSO_ordinal)

                    reazioneAvversa.IdUtenteCompilazione = reader.GetNullableInt64OrDefault(UTE_ID_COMPILAZIONE_ordinal)
                    reazioneAvversa.CodiceUslInserimento = reader.GetStringOrDefault(USL_INSERIMENTO_ordinal)
                    reazioneAvversa.DataModifica = reader.GetDateTimeOrDefault(DATA_VARIAZIONE_ordinal)
                    reazioneAvversa.IdUtenteModifica = reader.GetNullableInt64OrDefault(UTE_ID_VARIAZIONE_ordinal)

                    reazioneAvversa.IdVaccinazioneEseguita = reader.GetInt64(VES_ID_ordinal)
                    reazioneAvversa.IdReazioneAvversa = reader.GetNullableInt64OrDefault(ID_ordinal)

                    reazioneAvversa.AltreInformazioni = reader.GetStringOrDefault(ALTRE_INFORMAZIONI_ordinal)
                    reazioneAvversa.AmbitoOsservazione = reader.GetStringOrDefault(AMBITO_OSSERVAZIONE_ordinal)
                    reazioneAvversa.AmbitoOsservazione_Studio_Titolo = reader.GetStringOrDefault(AMBITO_STUDIO_TITOLO_ordinal)
                    reazioneAvversa.AmbitoOsservazione_Studio_Tipologia = reader.GetStringOrDefault(AMBITO_STUDIO_TIPOLOGIA_ordinal)
                    reazioneAvversa.AmbitoOsservazione_Studio_Numero = reader.GetStringOrDefault(AMBITO_STUDIO_NUMERO_ordinal)
                    reazioneAvversa.Peso = reader.GetDoubleOrDefault(PESO_ordinal)
                    reazioneAvversa.Altezza = reader.GetInt32OrDefault(ALTEZZA_ordinal)
                    reazioneAvversa.DataUltimaMestruazione = reader.GetNullableDateTimeOrDefault(DATA_ULTIMA_MESTRUAZIONE_ordinal)
                    reazioneAvversa.Allattamento = reader.GetStringOrDefault(ALLATTAMENTO_ordinal)
                    reazioneAvversa.Gravidanza = reader.GetStringOrDefault(GRAVIDANZA_ordinal)
                    reazioneAvversa.CausaReazioneOsservata = reader.GetStringOrDefault(CAUSA_OSSERVATA_ordinal)

                    reazioneAvversa.FarmacoConcomitante1_CodiceNomeCommerciale = reader.GetStringOrDefault(FARMCONC1_NOC_CODICE_ordinal)
                    reazioneAvversa.FarmacoConcomitante1_DescrizioneNomeCommerciale = reader.GetStringOrDefault(FARMCONC1_NOC_DESCRIZIONE_ordinal)
                    reazioneAvversa.FarmacoConcomitante1_CodiceLotto = reader.GetStringOrDefault(FARMCONC1_LOT_CODICE_ordinal)
                    reazioneAvversa.FarmacoConcomitante1_DataOraEffettuazione = reader.GetNullableDateTimeOrDefault(FARMCONC1_DATAORA_EFF_ordinal)
                    reazioneAvversa.FarmacoConcomitante1_Dose = reader.GetNullableInt32OrDefault(FARMCONC1_DOSE_ordinal)
                    reazioneAvversa.FarmacoConcomitante1_CodiceSitoInoculazione = reader.GetStringOrDefault(FARMCONC1_SII_CODICE_ordinal)
                    reazioneAvversa.FarmacoConcomitante1_CodiceViaSomministrazione = reader.GetStringOrDefault(FARMCONC1_VII_CODICE_ordinal)
                    reazioneAvversa.FarmacoConcomitante1_Sospeso = reader.GetStringOrDefault(FARMCONC1_SOSPESO_ordinal)
                    reazioneAvversa.FarmacoConcomitante1_Migliorata = reader.GetStringOrDefault(FARMCONC1_MIGLIORATA_ordinal)
                    reazioneAvversa.FarmacoConcomitante1_Ripreso = reader.GetStringOrDefault(FARMCONC1_RIPRESO_ordinal)
                    reazioneAvversa.FarmacoConcomitante1_Ricomparsa = reader.GetStringOrDefault(FARMCONC1_RICOMPARSA_ordinal)
                    reazioneAvversa.FarmacoConcomitante1_Indicazioni = reader.GetStringOrDefault(FARMCONC1_INDICAZIONI_ordinal)
                    reazioneAvversa.FarmacoConcomitante1_CodiceIndicazioni = reader.GetStringOrDefault(FARMCONC1_COD_INDICAZIONI_ordinal)
                    reazioneAvversa.FarmacoConcomitante1_Dosaggio = reader.GetStringOrDefault(FARMCONC1_DOSAGGIO_ordinal)
                    reazioneAvversa.FarmacoConcomitante1_Richiamo = reader.GetNullableInt32OrDefault(FARMCONC1_RICHIAMO_ordinal)

                    reazioneAvversa.FarmacoConcomitante2_CodiceNomeCommerciale = reader.GetStringOrDefault(FARMCONC2_NOC_CODICE_ordinal)
                    reazioneAvversa.FarmacoConcomitante2_DescrizioneNomeCommerciale = reader.GetStringOrDefault(FARMCONC2_NOC_DESCRIZIONE_ordinal)
                    reazioneAvversa.FarmacoConcomitante2_CodiceLotto = reader.GetStringOrDefault(FARMCONC2_LOT_CODICE_ordinal)
                    reazioneAvversa.FarmacoConcomitante2_DataOraEffettuazione = reader.GetNullableDateTimeOrDefault(FARMCONC2_DATAORA_EFF_ordinal)
                    reazioneAvversa.FarmacoConcomitante2_Dose = reader.GetNullableInt32OrDefault(FARMCONC2_DOSE_ordinal)
                    reazioneAvversa.FarmacoConcomitante2_CodiceSitoInoculazione = reader.GetStringOrDefault(FARMCONC2_SII_CODICE_ordinal)
                    reazioneAvversa.FarmacoConcomitante2_CodiceViaSomministrazione = reader.GetStringOrDefault(FARMCONC2_VII_CODICE_ordinal)
                    reazioneAvversa.FarmacoConcomitante2_Sospeso = reader.GetStringOrDefault(FARMCONC2_SOSPESO_ordinal)
                    reazioneAvversa.FarmacoConcomitante2_Migliorata = reader.GetStringOrDefault(FARMCONC2_MIGLIORATA_ordinal)
                    reazioneAvversa.FarmacoConcomitante2_Ripreso = reader.GetStringOrDefault(FARMCONC2_RIPRESO_ordinal)
                    reazioneAvversa.FarmacoConcomitante2_Ricomparsa = reader.GetStringOrDefault(FARMCONC2_RICOMPARSA_ordinal)
                    reazioneAvversa.FarmacoConcomitante2_Indicazioni = reader.GetStringOrDefault(FARMCONC2_INDICAZIONI_ordinal)
                    reazioneAvversa.FarmacoConcomitante2_CodiceIndicazioni = reader.GetStringOrDefault(FARMCONC2_COD_INDICAZIONI_ordinal)
                    reazioneAvversa.FarmacoConcomitante2_Dosaggio = reader.GetStringOrDefault(FARMCONC2_DOSAGGIO_ordinal)
                    reazioneAvversa.FarmacoConcomitante2_Richiamo = reader.GetNullableInt32OrDefault(FARMCONC2_RICHIAMO_ordinal)

                    reazioneAvversa.FirmaSegnalatore = reader.GetStringOrDefault(FIRMA_SEGNALATORE_ordinal)
                    reazioneAvversa.CodiceOrigineEtnica = reader.GetStringOrDefault(CODICEORIGINEETNICO_ordinal)
                    reazioneAvversa.DataScadenzaLotto = reader.GetDateTimeOrDefault(LOT_DATA_SCADENZA_ordinal)
                    reazioneAvversa.FarmacoConcomitante1_DataScadenzaLotto = reader.GetDateTimeOrDefault(FARMCONC1_LOT_DATA_SCADENZA_ordinal)
                    reazioneAvversa.FarmacoConcomitante2_DataScadenzaLotto = reader.GetDateTimeOrDefault(FARMCONC2_LOT_DATA_SCADENZA_ordinal)
                    reazioneAvversa.IdScheda = reader.GetStringOrDefault(ID_SCHEDA_ordinal)
                    reazioneAvversa.SegnalazioneId = reader.GetStringOrDefault(SEGNALAZIONE_ID_ordinal)
                    reazioneAvversa.UtenteInvio = reader.GetInt32OrDefault(UTENTE_ID_INVIO_ordinal)
                    reazioneAvversa.DataInvio = reader.GetDateTimeOrDefault(DATA_INVIO_ordinal)
                    reazioneAvversa.FlagInviato = reader.GetStringOrDefault(FLAG_INVIO_ordinal)

                    If deleted Then
                        reazioneAvversa.DataEliminazione = reader.GetNullableDateTimeOrDefault(DATA_ELIMINAZIONE_ordinal)
                        reazioneAvversa.IdUtenteEliminazione = reader.GetNullableInt64OrDefault(UTE_ID_ELIMINAZIONE_ordinal)
                    End If

                    ' Obsoleti, mantenuti per compatibilità

                    reazioneAvversa.LuogoDescrizione = reader.GetStringOrDefault(LUOGO_ordinal)
                    reazioneAvversa.AltroLuogoDescrizione = reader.GetStringOrDefault(ALTRO_LUOGO_ordinal)
                    reazioneAvversa.FarmacoDescrizione = reader.GetStringOrDefault(FARMACO_DESCRIZIONE_ordinal)

                    reazioneAvversaList.Add(reazioneAvversa)

                End While

            End If

            Return reazioneAvversaList

        End Function

        Private Function InsertReazioneAvversa(reazioneAvversa As ReazioneAvversa, dbTableName As String, dbFieldPrefix As String, idVaccinazioneDbFieldPrefix As String, deleted As Boolean) As Boolean

            If Not reazioneAvversa.IdReazioneAvversa.HasValue Then

                _DAM.ClearParam()

                reazioneAvversa.IdReazioneAvversa = Convert.ToInt64(_DAM.ExecScalar(Queries.VaccinazioniEseguite.OracleQueries.selNextSeqReazioneAvversa))

            End If

            With _DAM.QB

                .NewQuery()

                .AddTables(dbTableName)

                ' Campi not null
                .AddInsertField(dbFieldPrefix + "_paz_codice", reazioneAvversa.CodicePaziente, DataTypes.Numero)
                .AddInsertField(dbFieldPrefix + "_vac_codice", reazioneAvversa.CodiceVaccinazione, DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_n_richiamo", reazioneAvversa.NumeroRichiamo, DataTypes.Numero)
                .AddInsertField(dbFieldPrefix + "_res_data_effettuazione", reazioneAvversa.DataEffettuazione, DataTypes.Data)
                .AddInsertField(dbFieldPrefix + "_paz_codice_old", GetLongParam(reazioneAvversa.CodicePazientePrecedente), DataTypes.Numero)
                .AddInsertField(dbFieldPrefix + "_id", reazioneAvversa.IdReazioneAvversa, DataTypes.Numero)
                .AddInsertField(dbFieldPrefix + "_" + idVaccinazioneDbFieldPrefix + "_ID", GetLongParam(reazioneAvversa.IdVaccinazioneEseguita), DataTypes.Numero)
                .AddInsertField(dbFieldPrefix + "_rea_codice", reazioneAvversa.CodiceReazione, DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_data_reazione", reazioneAvversa.DataReazione, DataTypes.Data)
                .AddInsertField(dbFieldPrefix + "_re1_codice", GetStringParam(reazioneAvversa.CodiceReazione1), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_re2_codice", GetStringParam(reazioneAvversa.CodiceReazione2), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_visita", GetStringParam(reazioneAvversa.VisiteRicoveri), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_terapia", GetStringParam(reazioneAvversa.Terapie), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_grave", GetStringParam(reazioneAvversa.Grave), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_gravita_reazione", GetStringParam(reazioneAvversa.GravitaReazione), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_rea_altro", GetStringParam(reazioneAvversa.AltraReazione), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_esito", GetStringParam(reazioneAvversa.Esito), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_motivo_decesso", GetStringParam(reazioneAvversa.MotivoDecesso), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_esi_codice", GetStringParam(reazioneAvversa.CodiceEsito), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_data_esito", GetDateParam(reazioneAvversa.DataEsito), DataTypes.Data)
                .AddInsertField(dbFieldPrefix + "_noc_descrizione", GetStringParam(reazioneAvversa.DescrizioneNomeCommerciale), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_lot_codice", GetStringParam(reazioneAvversa.CodiceLotto), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_lot_data_scadenza", GetDateParam(reazioneAvversa.DataScadenzaLotto), DataTypes.Data)
                .AddInsertField(dbFieldPrefix + "_ora_effettuazione", GetStringParam(reazioneAvversa.OraEffettuazione), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_sii_codice", GetStringParam(reazioneAvversa.CodiceSitoInoculazione), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_luogo", GetStringParam(reazioneAvversa.LuogoDescrizione), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_altro_luogo", GetStringParam(reazioneAvversa.AltroLuogoDescrizione), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_farmaco_concomitante", GetStringParam(reazioneAvversa.FarmacoConcomitante), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_farmaco_descrizione", GetStringParam(reazioneAvversa.FarmacoDescrizione), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_uso_concomitante", GetStringParam(reazioneAvversa.UsoConcomitante), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_condizioni_concomitanti", GetStringParam(reazioneAvversa.CondizioniConcomitanti), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_qualifica", GetStringParam(reazioneAvversa.Qualifica), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_altra_qualifica", GetStringParam(reazioneAvversa.AltraQualifica), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_cognome_segnalatore", GetStringParam(reazioneAvversa.CognomeSegnalatore), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_nome_segnalatore", GetStringParam(reazioneAvversa.NomeSegnalatore), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_indirizzo_segnalatore", GetStringParam(reazioneAvversa.IndirizzoSegnalatore), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_tel_segnalatore", GetStringParam(reazioneAvversa.TelSegnalatore), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_fax_segnalatore", GetStringParam(reazioneAvversa.FaxSegnalatore), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_mail_segnalatore", GetStringParam(reazioneAvversa.MailSegnalatore), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_usl_inserimento", GetStringParam(reazioneAvversa.CodiceUslInserimento), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_ute_id_compilazione", GetLongParam(reazioneAvversa.IdUtenteCompilazione), DataTypes.Numero)
                .AddInsertField(dbFieldPrefix + "_data_compilazione", GetDateParam(reazioneAvversa.DataCompilazione), DataTypes.DataOra)
                .AddInsertField(dbFieldPrefix + "_data_variazione", GetDateParam(reazioneAvversa.DataModifica), DataTypes.DataOra)
                .AddInsertField(dbFieldPrefix + "_ute_id_variazione", GetLongParam(reazioneAvversa.IdUtenteModifica), DataTypes.Numero)

                .AddInsertField(dbFieldPrefix + "_altre_informazioni", GetStringParam(reazioneAvversa.AltreInformazioni), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_ambito_osservazione", GetStringParam(reazioneAvversa.AmbitoOsservazione), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_ambito_studio_titolo", GetStringParam(reazioneAvversa.AmbitoOsservazione_Studio_Titolo), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_ambito_studio_tipologia", GetStringParam(reazioneAvversa.AmbitoOsservazione_Studio_Tipologia), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_ambito_studio_numero", GetStringParam(reazioneAvversa.AmbitoOsservazione_Studio_Numero), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_peso", GetDoubleParam(reazioneAvversa.Peso), DataTypes.Double)
                .AddInsertField(dbFieldPrefix + "_altezza", GetIntParam(reazioneAvversa.Altezza), DataTypes.Numero)
                .AddInsertField(dbFieldPrefix + "_data_ultima_mestruazione", GetDateParam(reazioneAvversa.DataUltimaMestruazione), DataTypes.Data)
                .AddInsertField(dbFieldPrefix + "_allattamento", GetStringParam(reazioneAvversa.Allattamento), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_gravidanza", GetStringParam(reazioneAvversa.Gravidanza), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_causa_osservata", GetStringParam(reazioneAvversa.CausaReazioneOsservata), DataTypes.Stringa)

                .AddInsertField(dbFieldPrefix + "_sospeso", GetStringParam(reazioneAvversa.Sospeso), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_migliorata", GetStringParam(reazioneAvversa.Migliorata), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_ripreso", GetStringParam(reazioneAvversa.Ripreso), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_ricomparsa", GetStringParam(reazioneAvversa.Ricomparsa), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_indicazioni", GetStringParam(reazioneAvversa.Indicazioni), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_noi_codice_indicazioni", GetStringParam(reazioneAvversa.CodiceIndicazioni), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_dosaggio", GetStringParam(reazioneAvversa.Dosaggio), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_richiamo", GetStringParam(reazioneAvversa.Richiamo), DataTypes.Stringa)

                .AddInsertField(dbFieldPrefix + "_farmconc1_noc_codice", GetStringParam(reazioneAvversa.FarmacoConcomitante1_CodiceNomeCommerciale), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_farmconc1_noc_descrizione", GetStringParam(reazioneAvversa.FarmacoConcomitante1_DescrizioneNomeCommerciale), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_farmconc1_lot_codice", GetStringParam(reazioneAvversa.FarmacoConcomitante1_CodiceLotto), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_farmconc1_lot_data_scad", GetDateParam(reazioneAvversa.FarmacoConcomitante1_DataScadenzaLotto), DataTypes.Data)
                .AddInsertField(dbFieldPrefix + "_farmconc1_dataora_eff", GetDateParam(reazioneAvversa.FarmacoConcomitante1_DataOraEffettuazione), DataTypes.DataOra)
                .AddInsertField(dbFieldPrefix + "_farmconc1_dose", GetIntParam(reazioneAvversa.FarmacoConcomitante1_Dose), DataTypes.Numero)
                .AddInsertField(dbFieldPrefix + "_farmconc1_sii_codice", GetStringParam(reazioneAvversa.FarmacoConcomitante1_CodiceSitoInoculazione), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_farmconc1_vii_codice", GetStringParam(reazioneAvversa.FarmacoConcomitante1_CodiceViaSomministrazione), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_farmconc1_sospeso", GetStringParam(reazioneAvversa.FarmacoConcomitante1_Sospeso), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_farmconc1_migliorata", GetStringParam(reazioneAvversa.FarmacoConcomitante1_Migliorata), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_farmconc1_ripreso", GetStringParam(reazioneAvversa.FarmacoConcomitante1_Ripreso), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_farmconc1_ricomparsa", GetStringParam(reazioneAvversa.FarmacoConcomitante1_Ricomparsa), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_farmconc1_indicazioni", GetStringParam(reazioneAvversa.FarmacoConcomitante1_Indicazioni), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_farmconc1_noi_cod_indic", GetStringParam(reazioneAvversa.FarmacoConcomitante1_CodiceIndicazioni), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_farmconc1_dosaggio", GetStringParam(reazioneAvversa.FarmacoConcomitante1_Dosaggio), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_farmconc1_richiamo", GetIntParam(reazioneAvversa.FarmacoConcomitante1_Richiamo), DataTypes.Numero)

                .AddInsertField(dbFieldPrefix + "_farmconc2_noc_codice", GetStringParam(reazioneAvversa.FarmacoConcomitante2_CodiceNomeCommerciale), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_farmconc2_noc_descrizione", GetStringParam(reazioneAvversa.FarmacoConcomitante2_DescrizioneNomeCommerciale), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_farmconc2_lot_codice", GetStringParam(reazioneAvversa.FarmacoConcomitante2_CodiceLotto), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_farmconc2_lot_data_scad", GetDateParam(reazioneAvversa.FarmacoConcomitante2_DataScadenzaLotto), DataTypes.Data)
                .AddInsertField(dbFieldPrefix + "_farmconc2_dataora_eff", GetDateParam(reazioneAvversa.FarmacoConcomitante2_DataOraEffettuazione), DataTypes.DataOra)
                .AddInsertField(dbFieldPrefix + "_farmconc2_dose", GetIntParam(reazioneAvversa.FarmacoConcomitante2_Dose), DataTypes.Numero)
                .AddInsertField(dbFieldPrefix + "_farmconc2_sii_codice", GetStringParam(reazioneAvversa.FarmacoConcomitante2_CodiceSitoInoculazione), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_farmconc2_vii_codice", GetStringParam(reazioneAvversa.FarmacoConcomitante2_CodiceViaSomministrazione), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_farmconc2_sospeso", GetStringParam(reazioneAvversa.FarmacoConcomitante2_Sospeso), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_farmconc2_migliorata", GetStringParam(reazioneAvversa.FarmacoConcomitante2_Migliorata), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_farmconc2_ripreso", GetStringParam(reazioneAvversa.FarmacoConcomitante2_Ripreso), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_farmconc2_ricomparsa", GetStringParam(reazioneAvversa.FarmacoConcomitante2_Ricomparsa), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_farmconc2_indicazioni", GetStringParam(reazioneAvversa.FarmacoConcomitante2_Indicazioni), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_farmconc2_noi_cod_indic", GetStringParam(reazioneAvversa.FarmacoConcomitante2_CodiceIndicazioni), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_farmconc2_dosaggio", GetStringParam(reazioneAvversa.FarmacoConcomitante2_Dosaggio), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_farmconc2_richiamo", GetIntParam(reazioneAvversa.FarmacoConcomitante2_Richiamo), DataTypes.Numero)

                .AddInsertField(dbFieldPrefix + "_firma_segnalatore", GetStringParam(reazioneAvversa.FirmaSegnalatore), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_oet_codice", GetStringParam(reazioneAvversa.CodiceOrigineEtnica), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_id_scheda", GetStringParam(reazioneAvversa.IdScheda), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_segnalazione_id", GetStringParam(reazioneAvversa.SegnalazioneId), DataTypes.Stringa)
                .AddInsertField(dbFieldPrefix + "_ute_id_invio", GetIntParam(reazioneAvversa.UtenteInvio), DataTypes.Numero)
                .AddInsertField(dbFieldPrefix + "_data_invio", GetDateParam(reazioneAvversa.DataInvio), DataTypes.DataOra)
                .AddInsertField(dbFieldPrefix + "_flag_inviato", GetStringParam(reazioneAvversa.FlagInviato), DataTypes.Stringa)

                If deleted Then
                    .AddInsertField(dbFieldPrefix + "_ute_id_eliminazione", GetLongParam(reazioneAvversa.IdUtenteEliminazione), DataTypes.Numero)
                    .AddInsertField(dbFieldPrefix + "_data_eliminazione", GetDateParam(reazioneAvversa.DataEliminazione), DataTypes.DataOra)
                End If

            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Insert) > 0

        End Function

        Private Function UpdateReazioneAvversa(reazioneAvversa As ReazioneAvversa, dbTableName As String, dbFieldPrefix As String, idVaccinazioneDbFieldPrefix As String) As Boolean

            With _DAM.QB

                .NewQuery()

                .AddTables(dbTableName)

                .AddUpdateField(dbFieldPrefix + "_paz_codice", reazioneAvversa.CodicePaziente, DataTypes.Numero)
                .AddUpdateField(dbFieldPrefix + "_vac_codice", reazioneAvversa.CodiceVaccinazione, DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_n_richiamo", reazioneAvversa.NumeroRichiamo, DataTypes.Numero)
                .AddUpdateField(dbFieldPrefix + "_res_data_effettuazione", reazioneAvversa.DataEffettuazione, DataTypes.Data)
                .AddUpdateField(dbFieldPrefix + "_paz_codice_old", GetLongParam(reazioneAvversa.CodicePazientePrecedente), DataTypes.Numero)
                .AddUpdateField(dbFieldPrefix + "_" + idVaccinazioneDbFieldPrefix + "_ID", reazioneAvversa.IdVaccinazioneEseguita, DataTypes.Numero)
                .AddUpdateField(dbFieldPrefix + "_rea_codice", reazioneAvversa.CodiceReazione, DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_data_reazione", reazioneAvversa.DataReazione, DataTypes.Data)
                .AddUpdateField(dbFieldPrefix + "_visita", GetStringParam(reazioneAvversa.VisiteRicoveri), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_terapia", GetStringParam(reazioneAvversa.Terapie), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_re1_codice", GetStringParam(reazioneAvversa.CodiceReazione1), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_re2_codice", GetStringParam(reazioneAvversa.CodiceReazione2), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_noc_descrizione", GetStringParam(reazioneAvversa.DescrizioneNomeCommerciale), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_grave", GetStringParam(reazioneAvversa.Grave), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_gravita_reazione", GetStringParam(reazioneAvversa.GravitaReazione), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_rea_altro", GetStringParam(reazioneAvversa.AltraReazione), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_esito", GetStringParam(reazioneAvversa.Esito), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_data_esito", GetDateParam(reazioneAvversa.DataEsito), DataTypes.Data)
                .AddUpdateField(dbFieldPrefix + "_esi_codice", GetStringParam(reazioneAvversa.CodiceEsito), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_motivo_decesso", GetStringParam(reazioneAvversa.MotivoDecesso), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_lot_codice", GetStringParam(reazioneAvversa.CodiceLotto), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_lot_data_scadenza", GetDateParam(reazioneAvversa.DataScadenzaLotto), DataTypes.Data)
                .AddUpdateField(dbFieldPrefix + "_ora_effettuazione", GetStringParam(reazioneAvversa.OraEffettuazione), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_sii_codice", GetStringParam(reazioneAvversa.CodiceSitoInoculazione), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_luogo", GetStringParam(reazioneAvversa.LuogoDescrizione), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_altro_luogo", GetStringParam(reazioneAvversa.AltroLuogoDescrizione), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_farmaco_concomitante", GetStringParam(reazioneAvversa.FarmacoConcomitante), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_farmaco_descrizione", GetStringParam(reazioneAvversa.FarmacoDescrizione), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_uso_concomitante", GetStringParam(reazioneAvversa.UsoConcomitante), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_condizioni_concomitanti", GetStringParam(reazioneAvversa.CondizioniConcomitanti), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_qualifica", GetStringParam(reazioneAvversa.Qualifica), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_altra_qualifica", GetStringParam(reazioneAvversa.AltraQualifica), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_cognome_segnalatore", GetStringParam(reazioneAvversa.CognomeSegnalatore), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_nome_segnalatore", GetStringParam(reazioneAvversa.NomeSegnalatore), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_indirizzo_segnalatore", GetStringParam(reazioneAvversa.IndirizzoSegnalatore), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_tel_segnalatore", GetStringParam(reazioneAvversa.TelSegnalatore), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_fax_segnalatore", GetStringParam(reazioneAvversa.FaxSegnalatore), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_mail_segnalatore", GetStringParam(reazioneAvversa.MailSegnalatore), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_usl_inserimento", GetStringParam(reazioneAvversa.CodiceUslInserimento), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_ute_id_compilazione", GetLongParam(reazioneAvversa.IdUtenteCompilazione), DataTypes.Numero)
                .AddUpdateField(dbFieldPrefix + "_data_compilazione", GetDateParam(reazioneAvversa.DataCompilazione), DataTypes.DataOra)
                .AddUpdateField(dbFieldPrefix + "_data_variazione", GetDateParam(reazioneAvversa.DataModifica), DataTypes.DataOra)
                .AddUpdateField(dbFieldPrefix + "_ute_id_variazione", GetLongParam(reazioneAvversa.IdUtenteModifica), DataTypes.Numero)

                .AddUpdateField(dbFieldPrefix + "_altre_informazioni", GetStringParam(reazioneAvversa.AltreInformazioni), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_ambito_osservazione", GetStringParam(reazioneAvversa.AmbitoOsservazione), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_ambito_studio_titolo", GetStringParam(reazioneAvversa.AmbitoOsservazione_Studio_Titolo), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_ambito_studio_tipologia", GetStringParam(reazioneAvversa.AmbitoOsservazione_Studio_Tipologia), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_ambito_studio_numero", GetStringParam(reazioneAvversa.AmbitoOsservazione_Studio_Numero), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_peso", GetDoubleParam(reazioneAvversa.Peso), DataTypes.Double)
                .AddUpdateField(dbFieldPrefix + "_altezza", GetIntParam(reazioneAvversa.Altezza), DataTypes.Numero)
                .AddUpdateField(dbFieldPrefix + "_data_ultima_mestruazione", GetDateParam(reazioneAvversa.DataUltimaMestruazione), DataTypes.Data)
                .AddUpdateField(dbFieldPrefix + "_allattamento", GetStringParam(reazioneAvversa.Allattamento), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_gravidanza", GetStringParam(reazioneAvversa.Gravidanza), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_causa_osservata", GetStringParam(reazioneAvversa.CausaReazioneOsservata), DataTypes.Stringa)

                .AddUpdateField(dbFieldPrefix + "_dosaggio", GetStringParam(reazioneAvversa.Dosaggio), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_indicazioni", GetStringParam(reazioneAvversa.Indicazioni), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_noi_codice_indicazioni", GetStringParam(reazioneAvversa.CodiceIndicazioni), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_migliorata", GetStringParam(reazioneAvversa.Migliorata), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_richiamo", GetIntParam(reazioneAvversa.Richiamo), DataTypes.Numero)
                .AddUpdateField(dbFieldPrefix + "_ricomparsa", GetStringParam(reazioneAvversa.Ricomparsa), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_ripreso", GetStringParam(reazioneAvversa.Ripreso), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_sospeso", GetStringParam(reazioneAvversa.Sospeso), DataTypes.Stringa)

                .AddUpdateField(dbFieldPrefix + "_farmconc1_noc_codice", GetStringParam(reazioneAvversa.FarmacoConcomitante1_CodiceNomeCommerciale), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_farmconc1_noc_descrizione", GetStringParam(reazioneAvversa.FarmacoConcomitante1_DescrizioneNomeCommerciale), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_farmconc1_lot_codice", GetStringParam(reazioneAvversa.FarmacoConcomitante1_CodiceLotto), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_farmconc1_lot_data_scad", GetDateParam(reazioneAvversa.FarmacoConcomitante1_DataScadenzaLotto), DataTypes.Data)
                .AddUpdateField(dbFieldPrefix + "_farmconc1_dataora_eff", GetDateParam(reazioneAvversa.FarmacoConcomitante1_DataOraEffettuazione), DataTypes.DataOra)
                .AddUpdateField(dbFieldPrefix + "_farmconc1_dose", GetIntParam(reazioneAvversa.FarmacoConcomitante1_Dose), DataTypes.Numero)
                .AddUpdateField(dbFieldPrefix + "_farmconc1_sii_codice", GetStringParam(reazioneAvversa.FarmacoConcomitante1_CodiceSitoInoculazione), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_farmconc1_vii_codice", GetStringParam(reazioneAvversa.FarmacoConcomitante1_CodiceViaSomministrazione), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_farmconc1_sospeso", GetStringParam(reazioneAvversa.FarmacoConcomitante1_Sospeso), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_farmconc1_migliorata", GetStringParam(reazioneAvversa.FarmacoConcomitante1_Migliorata), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_farmconc1_ripreso", GetStringParam(reazioneAvversa.FarmacoConcomitante1_Ripreso), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_farmconc1_ricomparsa", GetStringParam(reazioneAvversa.FarmacoConcomitante1_Ricomparsa), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_farmconc1_indicazioni", GetStringParam(reazioneAvversa.FarmacoConcomitante1_Indicazioni), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_farmconc1_noi_cod_indic", GetStringParam(reazioneAvversa.FarmacoConcomitante1_CodiceIndicazioni), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_farmconc1_dosaggio", GetStringParam(reazioneAvversa.FarmacoConcomitante1_Dosaggio), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_farmconc1_richiamo", GetIntParam(reazioneAvversa.FarmacoConcomitante1_Richiamo), DataTypes.Numero)

                .AddUpdateField(dbFieldPrefix + "_farmconc2_noc_codice", GetStringParam(reazioneAvversa.FarmacoConcomitante2_CodiceNomeCommerciale), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_farmconc2_noc_descrizione", GetStringParam(reazioneAvversa.FarmacoConcomitante2_DescrizioneNomeCommerciale), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_farmconc2_lot_codice", GetStringParam(reazioneAvversa.FarmacoConcomitante2_CodiceLotto), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_farmconc2_lot_data_scad", GetDateParam(reazioneAvversa.FarmacoConcomitante2_DataScadenzaLotto), DataTypes.Data)
                .AddUpdateField(dbFieldPrefix + "_farmconc2_dataora_eff", GetDateParam(reazioneAvversa.FarmacoConcomitante2_DataOraEffettuazione), DataTypes.DataOra)
                .AddUpdateField(dbFieldPrefix + "_farmconc2_dose", GetIntParam(reazioneAvversa.FarmacoConcomitante2_Dose), DataTypes.Numero)
                .AddUpdateField(dbFieldPrefix + "_farmconc2_sii_codice", GetStringParam(reazioneAvversa.FarmacoConcomitante2_CodiceSitoInoculazione), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_farmconc2_vii_codice", GetStringParam(reazioneAvversa.FarmacoConcomitante2_CodiceViaSomministrazione), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_farmconc2_sospeso", GetStringParam(reazioneAvversa.FarmacoConcomitante2_Sospeso), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_farmconc2_migliorata", GetStringParam(reazioneAvversa.FarmacoConcomitante2_Migliorata), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_farmconc2_ripreso", GetStringParam(reazioneAvversa.FarmacoConcomitante2_Ripreso), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_farmconc2_ricomparsa", GetStringParam(reazioneAvversa.FarmacoConcomitante2_Ricomparsa), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_farmconc2_indicazioni", GetStringParam(reazioneAvversa.FarmacoConcomitante2_Indicazioni), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_farmconc2_noi_cod_indic", GetStringParam(reazioneAvversa.FarmacoConcomitante2_CodiceIndicazioni), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_farmconc2_dosaggio", GetStringParam(reazioneAvversa.FarmacoConcomitante2_Dosaggio), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_farmconc2_richiamo", GetIntParam(reazioneAvversa.FarmacoConcomitante2_Richiamo), DataTypes.Numero)

                .AddUpdateField(dbFieldPrefix + "_firma_segnalatore", GetStringParam(reazioneAvversa.FirmaSegnalatore), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_oet_codice", GetStringParam(reazioneAvversa.CodiceOrigineEtnica), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_id_scheda", GetStringParam(reazioneAvversa.IdScheda), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_segnalazione_id", GetStringParam(reazioneAvversa.SegnalazioneId), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_ute_id_invio", GetIntParam(reazioneAvversa.UtenteInvio), DataTypes.Numero)
                .AddUpdateField(dbFieldPrefix + "_data_invio", GetDateParam(reazioneAvversa.DataInvio), DataTypes.DataOra)
                .AddUpdateField(dbFieldPrefix + "_flag_inviato", GetStringParam(reazioneAvversa.FlagInviato), DataTypes.Stringa)

                .AddWhereCondition(dbFieldPrefix + "_id", Comparatori.Uguale, reazioneAvversa.IdReazioneAvversa, DataTypes.Numero)

            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Update) > 0

        End Function

        Private Function UpdateReazioneAvversaIdSchedaPrivate(reazioneAvversa As ReazioneAvversa, dbTableName As String, dbFieldPrefix As String) As Boolean

            With _DAM.QB

                .NewQuery()

                .AddTables(dbTableName)


                .AddUpdateField(dbFieldPrefix + "_id_scheda", GetStringParam(reazioneAvversa.IdScheda), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_segnalazione_id", GetStringParam(reazioneAvversa.SegnalazioneId), DataTypes.Stringa)
                .AddUpdateField(dbFieldPrefix + "_ute_id_invio", GetIntParam(reazioneAvversa.UtenteInvio), DataTypes.Numero)
                .AddUpdateField(dbFieldPrefix + "_data_invio", GetDateParam(reazioneAvversa.DataInvio), DataTypes.DataOra)
                .AddUpdateField(dbFieldPrefix + "_flag_inviato", GetStringParam(reazioneAvversa.FlagInviato), DataTypes.Stringa)
                .AddWhereCondition(dbFieldPrefix + "_id", Comparatori.Uguale, reazioneAvversa.IdReazioneAvversa, DataTypes.Numero)

            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Update) > 0

        End Function

#End Region

#End Region

#Region " OnVac API "

        ''' <summary>
        ''' Restituisce la lista di vaccinazioni eseguite e scadute del paziente specificato
        ''' </summary>
        ''' <param name="listCodiciPazienti"></param>
        ''' <returns></returns>
        Public Function GetListVaccinazioniEseguitePazientiAPP(listCodiciPazienti As List(Of Long)) As List(Of Entities.VaccinazioneEseguitaAPP) Implements IVaccinazioniEseguiteProvider.GetListVaccinazioniEseguitePazientiAPP

            Dim listEseguite As List(Of Entities.VaccinazioneEseguitaAPP) = Nothing

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand()

                    cmd.Connection = Me.Connection

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim query As String = Me.GetQueryVaccinazioniEseguiteAPP(False)

                    Dim filtroPazienti As GetInFilterResult = Me.GetInFilter(listCodiciPazienti)

                    cmd.CommandText = String.Format(query, filtroPazienti.InFilter)
                    cmd.Parameters.AddRange(filtroPazienti.Parameters)

                    listEseguite = Me.GetListVaccinazioniEseguiteAPP(cmd)

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return listEseguite

        End Function

        ''' <summary>
        ''' Restituisce la lista di vaccinazioni eseguite e scadute del paziente nella data specificata
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataEffettuazione"></param>
        ''' <param name="codiceAssociazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetListVaccinazioniEseguitePazienteDataAPP(codicePaziente As Long, dataEffettuazione As DateTime, codiceAssociazione As String) As List(Of Entities.VaccinazioneEseguitaAPP) Implements IVaccinazioniEseguiteProvider.GetListVaccinazioniEseguitePazienteDataAPP

            Dim listEseguite As List(Of Entities.VaccinazioneEseguitaAPP) = Nothing

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand(Me.GetQueryVaccinazioniEseguiteAPP(True), Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)
                    cmd.Parameters.AddWithValue("dataEffettuazione", dataEffettuazione)
                    cmd.Parameters.AddWithValue("codiceAssociazione", codiceAssociazione)

                    listEseguite = Me.GetListVaccinazioniEseguiteAPP(cmd)

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return listEseguite

        End Function

        ''' <summary>
        ''' Restituisce le mantoux per i pazienti specificati
        ''' </summary>
        ''' <param name="codiciPazienti"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetListMantoux(codiciPazienti As List(Of Long)) As List(Of Entities.PazienteMantoux) Implements IVaccinazioniEseguiteProvider.GetListMantoux

            Dim list As New List(Of Entities.PazienteMantoux)()

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand()

                    cmd.Connection = Me.Connection

                    Dim query As New System.Text.StringBuilder()
                    query.Append(" SELECT MAN_PAZ_CODICE, MAN_DESCRIZIONE, MAN_SINO, MAN_DATA, MAN_MM, MAN_DATA_INVIO, MAN_POSITIVA, ")
                    query.Append(" MAN_OPE_CODICE, OPE_NOME, PAZ_COGNOME, PAZ_NOME, PAZ_DATA_NASCITA ")
                    query.Append(" FROM T_PAZ_MANTOUX ")
                    query.Append(" JOIN T_PAZ_PAZIENTI ON MAN_PAZ_CODICE = PAZ_CODICE ")
                    query.Append(" LEFT JOIN T_ANA_OPERATORI ON MAN_OPE_CODICE = OPE_CODICE ")

                    If codiciPazienti.Count = 1 Then

                        query.Append(" WHERE MAN_PAZ_CODICE = :MAN_PAZ_CODICE ")

                        cmd.Parameters.AddWithValue("MAN_PAZ_CODICE", codiciPazienti.First())

                    Else

                        Dim filtroPazienti As GetInFilterResult = Me.GetInFilter(codiciPazienti)

                        query.AppendFormat(" WHERE MAN_PAZ_CODICE in ({0}) ", filtroPazienti.InFilter)

                        cmd.Parameters.AddRange(filtroPazienti.Parameters)

                    End If

                    cmd.CommandText = query.ToString()

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim MAN_PAZ_CODICE As Integer = idr.GetOrdinal("MAN_PAZ_CODICE")
                            Dim MAN_DESCRIZIONE As Integer = idr.GetOrdinal("MAN_DESCRIZIONE")
                            Dim MAN_SINO As Integer = idr.GetOrdinal("MAN_SINO")
                            Dim MAN_DATA As Integer = idr.GetOrdinal("MAN_DATA")
                            Dim MAN_MM As Integer = idr.GetOrdinal("MAN_MM")
                            Dim MAN_DATA_INVIO As Integer = idr.GetOrdinal("MAN_DATA_INVIO")
                            Dim MAN_POSITIVA As Integer = idr.GetOrdinal("MAN_POSITIVA")
                            Dim MAN_OPE_CODICE As Integer = idr.GetOrdinal("MAN_OPE_CODICE")
                            Dim OPE_NOME As Integer = idr.GetOrdinal("OPE_NOME")
                            Dim PAZ_COGNOME As Integer = idr.GetOrdinal("PAZ_COGNOME")
                            Dim PAZ_NOME As Integer = idr.GetOrdinal("PAZ_NOME")
                            Dim PAZ_DATA_NASCITA As Integer = idr.GetOrdinal("PAZ_DATA_NASCITA")

                            While idr.Read()

                                Dim item As New Entities.PazienteMantoux()

                                item.CodicePaziente = idr.GetInt64(MAN_PAZ_CODICE)
                                item.CodiceVaccinatore = idr.GetStringOrDefault(MAN_OPE_CODICE)
                                item.Cognome = idr.GetStringOrDefault(PAZ_COGNOME)
                                item.DataEsecuzione = idr.GetDateTime(MAN_DATA)
                                item.DataInvio = idr.GetDateTimeOrDefault(MAN_DATA_INVIO)
                                item.DataNascita = idr.GetDateTimeOrDefault(PAZ_DATA_NASCITA)
                                item.DescrizioneMantoux = idr.GetStringOrDefault(MAN_DESCRIZIONE)
                                item.DescrizioneVaccinatore = idr.GetStringOrDefault(OPE_NOME)
                                item.FlagEsecuzione = idr.GetBooleanOrDefault(MAN_SINO)
                                item.FlagPositiva = idr.GetBooleanOrDefault(MAN_POSITIVA)
                                item.Millimetri = idr.GetStringOrDefault(MAN_MM)
                                item.Nome = idr.GetStringOrDefault(PAZ_NOME)

                                list.Add(item)

                            End While

                        End If
                    End Using

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return list

        End Function

        Private Function GetListVaccinazioniEseguiteAPP(cmd As OracleClient.OracleCommand) As List(Of Entities.VaccinazioneEseguitaAPP)

            Dim listEseguite As New List(Of Entities.VaccinazioneEseguitaAPP)()

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    Dim ves_id As Integer = idr.GetOrdinal("ves_id")
                    Dim ves_paz_codice As Integer = idr.GetOrdinal("ves_paz_codice")
                    Dim ves_data_effettuazione As Integer = idr.GetOrdinal("ves_data_effettuazione")
                    Dim ves_dataora_effettuazione As Integer = idr.GetOrdinal("ves_dataora_effettuazione")
                    Dim ves_vac_codice As Integer = idr.GetOrdinal("ves_vac_codice")
                    Dim vac_descrizione As Integer = idr.GetOrdinal("vac_descrizione")
                    Dim ves_n_richiamo As Integer = idr.GetOrdinal("ves_n_richiamo")
                    Dim ves_ass_codice As Integer = idr.GetOrdinal("ves_ass_codice")
                    Dim ass_descrizione As Integer = idr.GetOrdinal("ass_descrizione")
                    Dim ves_ass_n_dose As Integer = idr.GetOrdinal("ves_ass_n_dose")
                    Dim ves_sii_codice As Integer = idr.GetOrdinal("ves_sii_codice")
                    Dim sii_descrizione As Integer = idr.GetOrdinal("sii_descrizione")
                    Dim ves_vii_codice As Integer = idr.GetOrdinal("ves_vii_codice")
                    Dim vii_descrizione As Integer = idr.GetOrdinal("vii_descrizione")
                    Dim ves_lot_codice As Integer = idr.GetOrdinal("ves_lot_codice")
                    Dim ves_noc_codice As Integer = idr.GetOrdinal("ves_noc_codice")
                    Dim noc_descrizione As Integer = idr.GetOrdinal("noc_descrizione")
                    Dim ves_ope_codice As Integer = idr.GetOrdinal("ves_ope_codice")
                    Dim resp_ope_nome As Integer = idr.GetOrdinal("resp_ope_nome")
                    Dim ves_med_vaccinante As Integer = idr.GetOrdinal("ves_med_vaccinante")
                    Dim vacc_ope_nome As Integer = idr.GetOrdinal("vacc_ope_nome")
                    Dim ves_cns_codice As Integer = idr.GetOrdinal("ves_cns_codice")
                    Dim cns_descrizione As Integer = idr.GetOrdinal("cns_descrizione")
                    Dim cns_indirizzo As Integer = idr.GetOrdinal("cns_indirizzo")
                    Dim com_descrizione As Integer = idr.GetOrdinal("com_descrizione")
                    Dim cns_n_telefono As Integer = idr.GetOrdinal("cns_n_telefono")
                    Dim ves_amb_codice As Integer = idr.GetOrdinal("ves_amb_codice")
                    Dim amb_descrizione As Integer = idr.GetOrdinal("amb_descrizione")
                    Dim ves_stato As Integer = idr.GetOrdinal("ves_stato")
                    Dim ves_cic_codice As Integer = idr.GetOrdinal("ves_cic_codice")
                    Dim ves_n_seduta As Integer = idr.GetOrdinal("ves_n_seduta")
                    Dim vac_obbligatoria As Integer = idr.GetOrdinal("vac_obbligatoria")
                    Dim scaduta As Integer = idr.GetOrdinal("scaduta")
                    Dim ves_flag_fittizia As Integer = idr.GetOrdinal("ves_flag_fittizia")
                    Dim vac_ordine As Integer = idr.GetOrdinal("vac_ordine")
                    Dim vra_rea_codice As Integer = idr.GetOrdinal("vra_rea_codice")
                    Dim rea_descrizione As Integer = idr.GetOrdinal("rea_descrizione")
                    Dim vra_data_reazione As Integer = idr.GetOrdinal("vra_data_reazione")
                    Dim vra_gravita_reazione As Integer = idr.GetOrdinal("vra_gravita_reazione")
                    Dim vra_re1_codice As Integer = idr.GetOrdinal("vra_re1_codice")
                    Dim rea_descrizione1 As Integer = idr.GetOrdinal("rea_descrizione1")
                    Dim vra_re2_codice As Integer = idr.GetOrdinal("vra_re2_codice")
                    Dim rea_descrizione2 As Integer = idr.GetOrdinal("rea_descrizione2")
                    Dim paz_nome As Integer = idr.GetOrdinal("paz_nome")
                    Dim paz_cognome As Integer = idr.GetOrdinal("paz_cognome")
                    Dim paz_data_nascita As Integer = idr.GetOrdinal("paz_data_nascita")
                    Dim ves_id_acn As Integer = idr.GetOrdinal("ves_id_acn")
                    Dim ves_provenienza As Integer = idr.GetOrdinal("ves_provenienza")

                    While idr.Read()

                        Dim eseguita As New Entities.VaccinazioneEseguitaAPP()

                        eseguita.Id = idr.GetInt64(ves_id)
                        eseguita.CodicePaziente = idr.GetInt64(ves_paz_codice)
                        eseguita.DataEffettuazione = idr.GetDateTime(ves_data_effettuazione)
                        eseguita.DataOraEffettuazione = idr.GetDateTime(ves_dataora_effettuazione)
                        eseguita.CodiceVaccinazione = idr.GetStringOrDefault(ves_vac_codice)
                        eseguita.DescrizioneVaccinazione = idr.GetStringOrDefault(vac_descrizione)
                        eseguita.DoseVaccinazione = idr.GetInt32OrDefault(ves_n_richiamo)
                        eseguita.CodiceAssociazione = idr.GetStringOrDefault(ves_ass_codice)
                        eseguita.DescrizioneAssociazione = idr.GetStringOrDefault(ass_descrizione)
                        eseguita.DoseAssociazione = idr.GetInt32OrDefault(ves_ass_n_dose)
                        eseguita.CodiceSitoInoculazione = idr.GetStringOrDefault(ves_sii_codice)
                        eseguita.DescrizioneSitoInoculazione = idr.GetStringOrDefault(sii_descrizione)
                        eseguita.CodiceViaSomministrazione = idr.GetStringOrDefault(ves_vii_codice)
                        eseguita.DescrizioneViaSomministrazione = idr.GetStringOrDefault(vii_descrizione)
                        eseguita.CodiceLotto = idr.GetStringOrDefault(ves_lot_codice)
                        eseguita.CodiceNomeCommerciale = idr.GetStringOrDefault(ves_noc_codice)
                        eseguita.DescrizioneNomeCommerciale = idr.GetStringOrDefault(noc_descrizione)
                        eseguita.CodiceMedicoResponsabile = idr.GetStringOrDefault(ves_ope_codice)
                        eseguita.DescrizioneMedicoResponsabile = idr.GetStringOrDefault(resp_ope_nome)
                        eseguita.CodiceVaccinatore = idr.GetStringOrDefault(ves_med_vaccinante)
                        eseguita.DescrizioneVaccinatore = idr.GetStringOrDefault(vacc_ope_nome)
                        eseguita.CodiceCentroVaccinale = idr.GetStringOrDefault(ves_cns_codice)
                        eseguita.DescrizioneCentroVaccinale = idr.GetStringOrDefault(cns_descrizione)
                        eseguita.IndirizzoCentroVaccinale = idr.GetStringOrDefault(cns_indirizzo)
                        eseguita.ComuneCentroVaccinale = idr.GetStringOrDefault(com_descrizione)
                        eseguita.TelefonoCentroVaccinale = idr.GetStringOrDefault(cns_n_telefono)
                        eseguita.CodiceAmbulatorio = idr.GetInt32OrDefault(ves_amb_codice)
                        eseguita.DescrizioneAmbulatorio = idr.GetStringOrDefault(amb_descrizione)
                        eseguita.Stato = idr.GetStringOrDefault(ves_stato)
                        eseguita.CodiceCiclo = idr.GetStringOrDefault(ves_cic_codice)
                        eseguita.NumeroSeduta = idr.GetInt32OrDefault(ves_n_seduta)
                        eseguita.Obbligatorieta = idr.GetStringOrDefault(vac_obbligatoria)
                        eseguita.IsScaduta = idr.GetBooleanOrDefault(scaduta)
                        eseguita.IsFittizia = idr.GetBooleanOrDefault(ves_flag_fittizia)
                        eseguita.OrdineVaccinazione = idr.GetInt32OrDefault(vac_ordine)
                        eseguita.CodiceReazione = idr.GetStringOrDefault(vra_rea_codice)
                        eseguita.DescrizioneReazione = idr.GetStringOrDefault(rea_descrizione)
                        eseguita.DataReazione = idr.GetDateTimeOrDefault(vra_data_reazione)
                        eseguita.GravitaReazione = idr.GetStringOrDefault(vra_gravita_reazione)
                        eseguita.CodiceReazione1 = idr.GetStringOrDefault(vra_re1_codice)
                        eseguita.DescrizioneReazione1 = idr.GetStringOrDefault(rea_descrizione1)
                        eseguita.CodiceReazione2 = idr.GetStringOrDefault(vra_re2_codice)
                        eseguita.DescrizioneReazione2 = idr.GetStringOrDefault(rea_descrizione2)
                        eseguita.CognomePaziente = idr.GetStringOrDefault(paz_cognome)
                        eseguita.NomePaziente = idr.GetStringOrDefault(paz_nome)
                        eseguita.DataNascitaPaziente = idr.GetDateTimeOrDefault(paz_data_nascita)
                        eseguita.IdACN = idr.GetStringOrDefault(ves_id_acn)
                        eseguita.Provenienza = idr.GetStringOrDefault(ves_provenienza)

                        listEseguite.Add(eseguita)

                    End While

                End If

            End Using

            Return listEseguite

        End Function

        ''' <summary>
        ''' Query per l'estrazione delle eseguite del paziente. E' la stessa query 
        ''' definita in OnVac.Queries\VaccinazioniEseguite\OracleQueries.cs, ma questa estrae esclusivamente i dati richiesti dalla APP Vaccinale
        ''' </summary>
        ''' <param name="filtraPazienteDataAssociazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetQueryVaccinazioniEseguiteAPP(filtraPazienteDataAssociazione As Boolean) As String

            Dim s As New System.Text.StringBuilder()

            s.Append("SELECT ves_id, ves_paz_codice, ves_dataora_effettuazione, ves_vii_codice, ves_cic_codice, ves_stato, ves_data_effettuazione, ")
            s.Append("      ves_noc_codice, ves_ass_codice, ves_ass_n_dose, vac_obbligatoria, vac_ordine, ves_sii_codice, ")
            s.Append("      vra_rea_codice, vra_re1_codice, vra_re2_codice, ")
            s.Append("      t_ana_reazioni_avverse.rea_descrizione, reazioni1.rea_descrizione rea_descrizione1, ")
            s.Append("      reazioni2.rea_descrizione rea_descrizione2, vra_data_reazione, ")
            s.Append("      ves_vac_codice, vac_descrizione, ves_n_richiamo, ves_n_seduta, ")
            s.Append("      ves_lot_codice, resp.ope_nome resp_ope_nome, vac.ope_nome vacc_ope_nome, ves_ope_codice, ")
            s.Append("      ves_med_vaccinante, sii_descrizione, vii_descrizione, ")
            s.Append("      noc_descrizione, ass_descrizione, ")
            s.Append("      'N' scaduta, ves_flag_fittizia, vra_gravita_reazione, ")
            s.Append("      ves_cns_codice, cns_descrizione, ves_amb_codice, amb_descrizione, cns_indirizzo, cns_n_telefono, com_descrizione, ")
            s.Append("      paz_nome, paz_cognome, paz_data_nascita, ves_id_acn, ves_provenienza ")
            s.Append(" FROM t_vac_eseguite, ")
            s.Append("      t_vac_reazioni_avverse, ")
            s.Append("      t_ana_lotti, ")
            s.Append("      t_ana_reazioni_avverse, ")
            s.Append("      t_ana_reazioni_avverse reazioni1, ")
            s.Append("      t_ana_reazioni_avverse reazioni2, ")
            s.Append("      t_ana_vaccinazioni, ")
            s.Append("      t_ana_nomi_commerciali, ")
            s.Append("      t_ana_siti_inoculazione, ")
            s.Append("      t_ana_vie_somministrazione, ")
            s.Append("      t_ana_operatori resp, ")
            s.Append("      t_ana_operatori vac, ")
            s.Append("      t_ana_associazioni, ")
            s.Append("      t_ana_consultori, ")
            s.Append("      t_ana_ambulatori, ")
            s.Append("      t_ana_comuni, ")
            s.Append("      t_paz_pazienti ")

            If filtraPazienteDataAssociazione Then
                s.Append(" WHERE ves_paz_codice = :codicePaziente ")
                s.Append(" AND ves_data_effettuazione = :dataEffettuazione ")
                s.Append(" AND ves_ass_codice = :codiceAssociazione ")
            Else
                s.Append("WHERE ves_paz_codice IN ({0}) ")
            End If

            s.Append("  AND ves_paz_codice = vra_paz_codice(+) ")
            s.Append("  AND ves_vac_codice = vra_vac_codice(+) ")
            s.Append("  AND ves_n_richiamo = vra_n_richiamo(+) ")
            s.Append("  AND ves_data_effettuazione = vra_res_data_effettuazione(+) ")
            s.Append("  AND ves_lot_codice = lot_codice(+) ")
            s.Append("  AND ass_codice(+) = ves_ass_codice ")
            s.Append("  AND vac_codice = ves_vac_codice ")
            s.Append("  AND resp.ope_codice(+) = ves_ope_codice ")
            s.Append("  AND vac.ope_codice(+) = ves_med_vaccinante ")
            s.Append("  AND sii_codice(+) = ves_sii_codice ")
            s.Append("  AND vii_codice(+) = ves_vii_codice ")
            s.Append("  AND noc_codice(+) = ves_noc_codice ")
            s.Append("  AND vra_rea_codice = t_ana_reazioni_avverse.rea_codice(+) ")
            s.Append("  AND vra_re1_codice = reazioni1.rea_codice(+) ")
            s.Append("  AND vra_re2_codice = reazioni2.rea_codice(+) ")
            s.Append("  AND ves_cns_codice = cns_codice(+) ")
            s.Append("  AND ves_amb_codice = amb_codice(+) ")
            s.Append("  AND cns_com_codice = com_codice(+) ")
            s.Append("  AND ves_paz_codice = paz_codice ")

            s.Append("UNION ")

            s.Append("SELECT vsc_id, vsc_paz_codice, vsc_dataora_effettuazione, vsc_vii_codice, vsc_cic_codice, vsc_stato, vsc_data_effettuazione, ")
            s.Append("      vsc_noc_codice, vsc_ass_codice, vsc_ass_n_dose, vac_obbligatoria, vac_ordine, vsc_sii_codice,  ")
            s.Append("      vrs_rea_codice, vrs_re1_codice, vrs_re2_codice, ")
            s.Append("      t_ana_reazioni_avverse.rea_descrizione, reazioni1.rea_descrizione rea_descrizione1, ")
            s.Append("      reazioni2.rea_descrizione rea_descrizione2, vrs_data_reazione, ")
            s.Append("      vsc_vac_codice, vac_descrizione, vsc_n_richiamo, vsc_n_seduta, ")
            s.Append("      vsc_lot_codice, resp.ope_nome resp_ope_nome, vac.ope_nome vacc_ope_nome, vsc_ope_codice,  ")
            s.Append("      vsc_med_vaccinante, sii_descrizione, vii_descrizione, ")
            s.Append("      noc_descrizione, ass_descrizione,  ")
            s.Append("      'S' scaduta, vsc_flag_fittizia ves_flag_fittizia, vrs_gravita_reazione,  ")
            s.Append("      vsc_cns_codice, cns_descrizione, vsc_amb_codice, amb_descrizione, cns_indirizzo, cns_n_telefono, com_descrizione, ")
            s.Append("      paz_nome, paz_cognome, paz_data_nascita, vsc_id_acn ves_id_acn, vsc_provenienza ves_provenienza ")
            s.Append(" FROM t_vac_scadute, ")
            s.Append("      t_vac_reazioni_scadute, ")
            s.Append("      t_ana_lotti, ")
            s.Append("      t_ana_reazioni_avverse, ")
            s.Append("      t_ana_reazioni_avverse reazioni1, ")
            s.Append("      t_ana_reazioni_avverse reazioni2, ")
            s.Append("      t_ana_vaccinazioni, ")
            s.Append("      t_ana_nomi_commerciali, ")
            s.Append("      t_ana_siti_inoculazione, ")
            s.Append("      t_ana_vie_somministrazione, ")
            s.Append("      t_ana_operatori resp, ")
            s.Append("      t_ana_operatori vac, ")
            s.Append("      t_ana_associazioni, ")
            s.Append("      t_ana_consultori, ")
            s.Append("      t_ana_ambulatori, ")
            s.Append("      t_ana_comuni, ")
            s.Append("      t_paz_pazienti ")

            If filtraPazienteDataAssociazione Then
                s.Append(" WHERE vsc_paz_codice = :codicePaziente ")
                s.Append(" AND vsc_data_effettuazione = :dataEffettuazione ")
                s.Append(" AND vsc_ass_codice = :codiceAssociazione ")
            Else
                s.Append("WHERE vsc_paz_codice IN ({0}) ")
            End If

            s.Append("  AND vsc_paz_codice = vrs_paz_codice(+) ")
            s.Append("  AND vsc_vac_codice = vrs_vac_codice(+) ")
            s.Append("  AND vsc_n_richiamo = vrs_n_richiamo(+) ")
            s.Append("  AND vsc_data_effettuazione = vrs_res_data_effettuazione(+) ")
            s.Append("  AND vsc_lot_codice = lot_codice(+) ")
            s.Append("  AND ass_codice(+) = vsc_ass_codice ")
            s.Append("  AND vac_codice = vsc_vac_codice ")
            s.Append("  AND resp.ope_codice(+) = vsc_ope_codice ")
            s.Append("  AND vac.ope_codice(+) = vsc_med_vaccinante ")
            s.Append("  AND sii_codice(+) = vsc_sii_codice ")
            s.Append("  AND vii_codice(+) = vsc_vii_codice ")
            s.Append("  AND noc_codice(+) = vsc_noc_codice ")
            s.Append("  AND vrs_rea_codice = t_ana_reazioni_avverse.rea_codice(+) ")
            s.Append("  AND vrs_re1_codice = reazioni1.rea_codice(+) ")
            s.Append("  AND vrs_re2_codice = reazioni2.rea_codice(+) ")
            s.Append("  AND vsc_cns_codice = cns_codice(+) ")
            s.Append("  AND vsc_amb_codice = amb_codice(+) ")
            s.Append("  AND cns_com_codice = com_codice(+) ")
            s.Append("  AND vsc_paz_codice = paz_codice ")

            Return s.ToString()

        End Function




#Region " FSE "

        ''' <summary>
        ''' Restituisce la lista di vaccinazioni eseguite del paziente specificato
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        Public Function GetListVaccinazioniEseguitePazienteFSE(codicePaziente As Long) As List(Of Entities.VaccinazioneFSE) Implements IVaccinazioniEseguiteProvider.GetListVaccinazioniEseguitePazienteFSE

            Dim listEseguite As New List(Of Entities.VaccinazioneFSE)()

            'NB: la query rispetta i requisiti che sono nella stampa pdf (es: 
            ' - dato che il parametro StampaLottoNomeCommerciale=False, viene visualizzata ass_stampa come descrizione dell'associazione
            ' - se ves_flag_fittizia='S' non vengono visualizzate le date di effettuazione ves_data_effettuazione e ves_dataora_effettuazione

            'TODO [FSE]: gestire ves_tipo_erogatore e ves_codice_struttura (luogo vaccinazione) una volta che saranno creati i campi nel db per l'AVN. Regole di valorizzazione:
            '            0 = HSP. 1 = STS, 2 = FLS, 3+4 = AMB_MMG_PLS, null = altro
            '           NB: per ora nei test lo setto sempre ves_tipo_erogatore = '2' (FLS) e ves_codice_struttura = '050503' (verificare che ci sia lo ZERO nel dato salvato sul nuovo campo!)
            'TODO [FSE]: la function F_GET_CODICE_MALATTIA_VAC() restituisce la prima malattia trovata in corrispondenza di un dato vac_codice. Gestire diversamente il caso di più malattie associate alla stessa vaccinazione

            Dim query As New System.Text.StringBuilder()
            query.Append("SELECT ves_id, ves_vac_codice, vac_descrizione, ves_n_richiamo, ves_ass_codice, ass_stampa ass_descrizione, ves_ass_n_dose, vac_ordine, ")
            query.Append("      DECODE( NVL(ves_cic_codice,''), '', 'N', CASE WHEN (SELECT MAX(tsd_n_seduta) FROM t_ana_tempi_sedute WHERE ves_cic_codice=tsd_cic_codice)<ves_n_richiamo THEN 'R' ELSE 'C' END ) tipo_vaccinazione, ")
            query.Append("      ves_cic_codice, ves_n_seduta, ves_in_campagna, vii_codice_fse, vii_descrizione, sii_codice_fse, sii_descrizione, ves_lot_codice, ves_noc_codice, ves_codice_esenzione, ")
            query.Append("      DECODE( '2', '0', 'HSP11', '1', 'STS11', '2', 'FLS11', '3', 'AMB_MMG_PLS', '4', 'AMB_MMG_PLS') ves_tipo_erogatore, '050503' ves_codice_struttura, ")
            'query.Append("      DECODE( ves_tipo_erogatore, '0', 'HSP11', '1', 'STS11', '2', 'FLS11', '3', 'AMB_MMG_PLS', '4', 'AMB_MMG_PLS') ves_tipo_erogatore, ves_codice_struttura, ")
            query.Append("      ves_flag_fittizia, ves_data_effettuazione, ves_dataora_effettuazione, ")
            query.Append("      noc_descrizione, noc_codice_aic, mal_codice, mal_descrizione, mal_codice_fse, mal_codice_esenzione, vac_codice_fse ")
            query.Append(" FROM t_vac_eseguite ")
            query.Append(" LEFT JOIN t_ana_associazioni ON ass_codice = ves_ass_codice ")
            query.Append(" INNER JOIN t_ana_vaccinazioni ON vac_codice = ves_vac_codice ")
            query.Append(" LEFT JOIN t_ana_nomi_commerciali ON noc_codice = ves_noc_codice ")
            query.Append(" LEFT JOIN t_ana_siti_inoculazione ON sii_codice = ves_sii_codice ")
            query.Append(" LEFT JOIN t_ana_vie_somministrazione ON vii_codice = ves_vii_codice ")
            query.Append(" LEFT JOIN t_ana_malattie ON mal_codice = F_GET_CODICE_MALATTIA_VAC(ves_vac_codice) ")
            query.Append(" WHERE ves_paz_codice = :codicePaziente ")

            Using cmd As OracleCommand = New OracleCommand(query.ToString(), Me.Connection)

                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim ves_id As Integer = idr.GetOrdinal("ves_id")
                            Dim ves_vac_codice As Integer = idr.GetOrdinal("ves_vac_codice")
                            Dim vac_descrizione As Integer = idr.GetOrdinal("vac_descrizione")
                            Dim ves_n_richiamo As Integer = idr.GetOrdinal("ves_n_richiamo")
                            Dim ves_ass_codice As Integer = idr.GetOrdinal("ves_ass_codice")
                            Dim ass_descrizione As Integer = idr.GetOrdinal("ass_descrizione")
                            Dim ves_ass_n_dose As Integer = idr.GetOrdinal("ves_ass_n_dose")
                            Dim vac_ordine As Integer = idr.GetOrdinal("vac_ordine")
                            Dim tipo_vaccinazione As Integer = idr.GetOrdinal("tipo_vaccinazione")
                            Dim ves_cic_codice As Integer = idr.GetOrdinal("ves_cic_codice")
                            Dim ves_n_seduta As Integer = idr.GetOrdinal("ves_n_seduta")
                            Dim ves_tipo_erogatore As Integer = idr.GetOrdinal("ves_tipo_erogatore")
                            Dim ves_codice_struttura As Integer = idr.GetOrdinal("ves_codice_struttura")
                            Dim ves_flag_fittizia As Integer = idr.GetOrdinal("ves_flag_fittizia")
                            Dim ves_data_effettuazione As Integer = idr.GetOrdinal("ves_data_effettuazione")
                            Dim ves_dataora_effettuazione As Integer = idr.GetOrdinal("ves_dataora_effettuazione")
                            Dim ves_in_campagna As Integer = idr.GetOrdinal("ves_in_campagna")
                            Dim vii_codice_fse As Integer = idr.GetOrdinal("vii_codice_fse")
                            Dim vii_descrizione As Integer = idr.GetOrdinal("vii_descrizione")
                            Dim sii_codice_fse As Integer = idr.GetOrdinal("sii_codice_fse")
                            Dim sii_descrizione As Integer = idr.GetOrdinal("sii_descrizione")
                            Dim ves_lot_codice As Integer = idr.GetOrdinal("ves_lot_codice")
                            Dim ves_noc_codice As Integer = idr.GetOrdinal("ves_noc_codice")
                            Dim ves_codice_esenzione As Integer = idr.GetOrdinal("ves_codice_esenzione")
                            Dim noc_descrizione As Integer = idr.GetOrdinal("noc_descrizione")
                            Dim noc_codice_aic As Integer = idr.GetOrdinal("noc_codice_aic")
                            Dim mal_codice As Integer = idr.GetOrdinal("mal_codice")
                            Dim mal_descrizione As Integer = idr.GetOrdinal("mal_descrizione")
                            Dim mal_codice_fse As Integer = idr.GetOrdinal("mal_codice_fse")
                            Dim mal_codice_esenzione As Integer = idr.GetOrdinal("mal_codice_esenzione")
                            Dim vac_codice_fse As Integer = idr.GetOrdinal("vac_codice_fse")

                            While idr.Read()

                                Dim vacEseguita As New Entities.VaccinazioneFSE()

                                vacEseguita.Id = idr.GetInt64(ves_id)
                                vacEseguita.CodiceVaccinazione = idr.GetStringOrDefault(ves_vac_codice)
                                vacEseguita.DescrizioneVaccinazione = idr.GetStringOrDefault(vac_descrizione)
                                If Not idr.IsDBNull(ves_n_richiamo) Then
                                    vacEseguita.DoseVaccinazione = idr.GetInt32(ves_n_richiamo)
                                End If
                                vacEseguita.CodiceAssociazione = idr.GetStringOrDefault(ves_ass_codice)
                                vacEseguita.DescrizioneAssociazione = idr.GetStringOrDefault(ass_descrizione)
                                If Not idr.IsDBNull(ves_ass_n_dose) Then
                                    vacEseguita.DoseAssociazione = idr.GetInt32(ves_ass_n_dose)
                                End If
                                vacEseguita.OrdineVaccinazione = idr.GetInt32OrDefault(vac_ordine)
                                vacEseguita.TipoVaccinazione = idr.GetStringOrDefault(tipo_vaccinazione)
                                vacEseguita.CodiceCiclo = idr.GetStringOrDefault(ves_cic_codice)
                                If Not idr.IsDBNull(ves_n_seduta) Then
                                    vacEseguita.NumeroSeduta = idr.GetInt32(ves_n_seduta)
                                End If
                                If idr.GetStringOrDefault(ves_flag_fittizia) <> "S" Then
                                    vacEseguita.DataEffettuazione = idr.GetDateTimeOrDefault(ves_data_effettuazione)
                                    vacEseguita.DataOraEffettuazione = idr.GetDateTimeOrDefault(ves_dataora_effettuazione)
                                End If
                                vacEseguita.InCampagna = idr.GetStringOrDefault(ves_in_campagna)
                                If Not idr.IsDBNull(ves_tipo_erogatore) Then
                                    vacEseguita.TipologiaLuogo = [Enum].Parse(GetType(Enumerators.TipoStruttura), idr.GetStringOrDefault(ves_tipo_erogatore))
                                End If
                                vacEseguita.CodiceLuogo = idr.GetStringOrDefault(ves_codice_struttura)
                                vacEseguita.CodiceHL7ViaSomministrazione = idr.GetStringOrDefault(vii_codice_fse)
                                vacEseguita.DescrizioneViaSomministrazione = idr.GetStringOrDefault(vii_descrizione)
                                vacEseguita.CodiceHL7SitoInoculazione = idr.GetStringOrDefault(sii_codice_fse)
                                vacEseguita.DescrizioneSitoInoculazione = idr.GetStringOrDefault(sii_descrizione)
                                vacEseguita.CodiceLotto = idr.GetStringOrDefault(ves_lot_codice)
                                vacEseguita.CodiceNomeCommerciale = idr.GetStringOrDefault(ves_noc_codice)
                                vacEseguita.DescrizioneNomeCommerciale = idr.GetStringOrDefault(noc_descrizione)
                                vacEseguita.CodiceAicNomeCommerciale = idr.GetStringOrDefault(noc_codice_aic)
                                vacEseguita.CodiceCvxVaccinazione = idr.GetStringOrDefault(vac_codice_fse)
                                vacEseguita.CodiceMalattia = idr.GetStringOrDefault(mal_codice)
                                vacEseguita.DescrizioneMalattia = idr.GetStringOrDefault(mal_descrizione)
                                vacEseguita.CodiceICD9CMMalattia = idr.GetStringOrDefault(mal_codice_fse)
                                vacEseguita.CodiceHL7Esenzione = idr.GetStringOrDefault(mal_codice_esenzione)
                                vacEseguita.IsEseguita = True

                                listEseguite.Add(vacEseguita)

                            End While

                        End If

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return listEseguite

        End Function


#End Region

#End Region


#Region " Flussi ACN "


        Public Function GetVaccinazioniEseguiteUnionScadute(filtroCodicePaziente As Long?, filtroIdACN As String, filtroCodAssociazione As String, filtroDataEffettuazione As Date?, filtroProvenienza As String) As List(Of VaccinazioneIntegrazioneDB) Implements IVaccinazioniEseguiteProvider.GetVaccinazioniEseguiteUnionScadute

            Dim vaccinazioniList As New List(Of VaccinazioneIntegrazioneDB)()

            'deve esserci almeno un filtro impostato
            If filtroCodicePaziente.HasValue OrElse Not String.IsNullOrWhiteSpace(filtroIdACN) OrElse Not String.IsNullOrWhiteSpace(filtroCodAssociazione) OrElse filtroDataEffettuazione.HasValue OrElse Not String.IsNullOrWhiteSpace(filtroProvenienza) Then

                Dim cmd As OracleClient.OracleCommand = Nothing

                Dim ownConnection As Boolean = False

                Try

                    Dim query As New Text.StringBuilder()
                    query.Append(" SELECT ves_id, ves_paz_codice, ves_data_effettuazione, ves_dataora_effettuazione, ves_luogo, ves_cns_codice, ")
                    query.Append("       ves_ope_codice, ves_med_vaccinante, ves_ass_codice, ves_ass_n_dose, A.ass_descrizione, ")
                    query.Append("       ves_vac_codice, V.vac_descrizione, ves_n_richiamo, ves_lot_codice, ves_noc_codice, ")
                    query.Append("       ves_vii_codice, ves_sii_codice, ves_mal_codice_malattia, ves_codice_esenzione, ves_in_campagna, ")
                    query.Append("       ves_flag_scaduta, ves_stato, ves_id_acn, ves_ope_in_ambulatorio, ves_note, ves_provenienza ")
                    query.Append(" FROM v_vac_eseguite_union_scadute T ")
                    query.Append(" INNER JOIN t_ana_associazioni A ON T.ves_ass_codice = A.ass_codice ")
                    query.Append(" INNER JOIN t_ana_vaccinazioni V ON T.ves_vac_codice = V.vac_codice ")
                    query.Append(" WHERE 1=1 ")
                    If filtroCodicePaziente.HasValue Then
                        query.Append(" AND ves_paz_codice = :ves_paz_codice ")
                    End If
                    If Not String.IsNullOrWhiteSpace(filtroIdACN) Then
                        query.Append(" AND ves_id_acn = :ves_id_acn ")
                    End If
                    If Not String.IsNullOrWhiteSpace(filtroCodAssociazione) Then
                        query.Append(" AND ves_ass_codice = :ves_ass_codice ")
                    End If
                    If filtroDataEffettuazione.HasValue Then
                        query.Append(" AND ves_data_effettuazione = :ves_data_effettuazione ")
                    End If
                    If Not String.IsNullOrWhiteSpace(filtroProvenienza) Then
                        query.Append(" AND ves_provenienza = :ves_provenienza ")
                    End If

                    cmd = New OracleClient.OracleCommand(query.ToString(), Connection)

                    If filtroCodicePaziente.HasValue Then
                        cmd.Parameters.AddWithValue("ves_paz_codice", filtroCodicePaziente.Value)
                    End If
                    If Not String.IsNullOrWhiteSpace(filtroIdACN) Then
                        cmd.Parameters.AddWithValue("ves_id_acn", filtroIdACN)
                    End If
                    If Not String.IsNullOrWhiteSpace(filtroCodAssociazione) Then
                        cmd.Parameters.AddWithValue("ves_ass_codice", filtroCodAssociazione)
                    End If
                    If filtroDataEffettuazione.HasValue Then
                        cmd.Parameters.AddWithValue("ves_data_effettuazione", filtroDataEffettuazione.Value)
                    End If
                    If Not String.IsNullOrWhiteSpace(filtroProvenienza) Then
                        cmd.Parameters.AddWithValue("ves_provenienza", filtroProvenienza)
                    End If

                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim ves_id As Integer = idr.GetOrdinal("ves_id")
                            Dim ves_paz_codice As Integer = idr.GetOrdinal("ves_paz_codice")
                            Dim ves_data_effettuazione As Integer = idr.GetOrdinal("ves_data_effettuazione")
                            Dim ves_dataora_effettuazione As Integer = idr.GetOrdinal("ves_dataora_effettuazione")
                            Dim ves_luogo As Integer = idr.GetOrdinal("ves_luogo")
                            Dim ves_cns_codice As Integer = idr.GetOrdinal("ves_cns_codice")
                            Dim ves_ope_codice As Integer = idr.GetOrdinal("ves_ope_codice")
                            Dim ves_med_vaccinante As Integer = idr.GetOrdinal("ves_med_vaccinante")
                            Dim ves_ass_codice As Integer = idr.GetOrdinal("ves_ass_codice")
                            Dim ves_ass_n_dose As Integer = idr.GetOrdinal("ves_ass_n_dose")
                            Dim ass_descrizione As Integer = idr.GetOrdinal("ass_descrizione")
                            Dim ves_vac_codice As Integer = idr.GetOrdinal("ves_vac_codice")
                            Dim vac_descrizione As Integer = idr.GetOrdinal("vac_descrizione")
                            Dim ves_n_richiamo As Integer = idr.GetOrdinal("ves_n_richiamo")
                            Dim ves_lot_codice As Integer = idr.GetOrdinal("ves_lot_codice")
                            Dim ves_noc_codice As Integer = idr.GetOrdinal("ves_noc_codice")
                            Dim ves_vii_codice As Integer = idr.GetOrdinal("ves_vii_codice")
                            Dim ves_sii_codice As Integer = idr.GetOrdinal("ves_sii_codice")
                            Dim ves_mal_codice_malattia As Integer = idr.GetOrdinal("ves_mal_codice_malattia")
                            Dim ves_codice_esenzione As Integer = idr.GetOrdinal("ves_codice_esenzione")
                            Dim ves_in_campagna As Integer = idr.GetOrdinal("ves_in_campagna")
                            Dim ves_ope_in_ambulatorio As Integer = idr.GetOrdinal("ves_ope_in_ambulatorio")
                            Dim ves_note As Integer = idr.GetOrdinal("ves_note")
                            Dim ves_flag_scaduta As Integer = idr.GetOrdinal("ves_flag_scaduta")
                            Dim ves_stato As Integer = idr.GetOrdinal("ves_stato")
                            Dim ves_id_acn As Integer = idr.GetOrdinal("ves_id_acn")
                            Dim ves_provenienza As Integer = idr.GetOrdinal("ves_provenienza")
                            'Dim ves_lot_data_scadenza As Integer = idr.GetOrdinal("ves_lot_data_scadenza")

                            While idr.Read()

                                Dim vaccinazioneIntegrazione As New VaccinazioneIntegrazioneDB()

                                vaccinazioneIntegrazione.IdVaccinazione = idr.GetInt64(ves_id)
                                vaccinazioneIntegrazione.CodicePaziente = idr.GetInt64(ves_paz_codice)
                                vaccinazioneIntegrazione.DataEffettuazione = idr.GetDateTimeOrDefault(ves_data_effettuazione)
                                vaccinazioneIntegrazione.DataOraEffettuazione = idr.GetDateTimeOrDefault(ves_dataora_effettuazione)
                                vaccinazioneIntegrazione.Luogo = idr.GetStringOrDefault(ves_luogo)
                                vaccinazioneIntegrazione.CodiceConsultorio = idr.GetStringOrDefault(ves_cns_codice)
                                vaccinazioneIntegrazione.CodiceMedico = idr.GetStringOrDefault(ves_ope_codice)
                                vaccinazioneIntegrazione.CodiceVaccinatore = idr.GetStringOrDefault(ves_med_vaccinante)
                                vaccinazioneIntegrazione.AssociazioneCod = idr.GetStringOrDefault(ves_ass_codice)
                                vaccinazioneIntegrazione.NrDoseAssociazione = idr.GetInt32OrDefault(ves_ass_n_dose)
                                vaccinazioneIntegrazione.AssociazioneDescr = idr.GetStringOrDefault(ass_descrizione)
                                vaccinazioneIntegrazione.CodiceVaccinazione = idr.GetStringOrDefault(ves_vac_codice)
                                vaccinazioneIntegrazione.DescrVaccinazione = idr.GetStringOrDefault(vac_descrizione)
                                vaccinazioneIntegrazione.NrDoseVaccinazione = idr.GetInt32OrDefault(ves_n_richiamo)
                                vaccinazioneIntegrazione.CodiceLotto = idr.GetStringOrDefault(ves_lot_codice)
                                'vaccinazioneIntegrazione.DataScadenzaLotto = idr.GetDateTimeOrDefault(ves_lot_data_scadenza) '
                                vaccinazioneIntegrazione.CodiceNomeCommerciale = idr.GetStringOrDefault(ves_noc_codice)
                                vaccinazioneIntegrazione.CodiceSomministrazione = idr.GetStringOrDefault(ves_vii_codice)
                                vaccinazioneIntegrazione.CodiceInoculazione = idr.GetStringOrDefault(ves_sii_codice)
                                vaccinazioneIntegrazione.CodiceMalattia = idr.GetStringOrDefault(ves_mal_codice_malattia)
                                vaccinazioneIntegrazione.CodiceEsenzione = idr.GetStringOrDefault(ves_codice_esenzione)
                                vaccinazioneIntegrazione.InCampagna = idr.GetStringOrDefault(ves_in_campagna)
                                vaccinazioneIntegrazione.IsMedicoInAmbulatorio = idr.GetBooleanOrDefault(ves_ope_in_ambulatorio)
                                vaccinazioneIntegrazione.Note = idr.GetStringOrDefault(ves_note)
                                vaccinazioneIntegrazione.Scaduta = idr.GetBooleanOrDefault(ves_flag_scaduta)
                                vaccinazioneIntegrazione.Stato = idr.GetStringOrDefault(ves_stato)
                                vaccinazioneIntegrazione.IdACN = idr.GetStringOrDefault(ves_id_acn)
                                If Not String.IsNullOrWhiteSpace(idr.GetStringOrDefault(ves_provenienza)) Then
                                    vaccinazioneIntegrazione.Provenienza = [Enum].Parse(GetType(Enumerators.ProvenienzaVaccinazioni), idr.GetStringOrDefault(ves_provenienza), True)
                                Else
                                    vaccinazioneIntegrazione.Provenienza = Nothing
                                End If


                                vaccinazioniList.Add(vaccinazioneIntegrazione)

                            End While

                        End If

                    End Using

                Finally

                    ConditionalCloseConnection(ownConnection)

                    If Not cmd Is Nothing Then cmd.Dispose()

                End Try

            End If

            Return vaccinazioniList

        End Function


        ''' <summary>
        ''' Funzione che aggiorna il valore della dose vaccinazione nella T_VAC_ESEGUITE
        ''' </summary>
        ''' <param name="ves_id"></param>
        ''' <param name="numeroVac"></param>
        ''' <returns></returns>
        Public Function UpdateNumeroVacVacEseguita(ves_id As Integer, numeroVac As Integer) As Integer Implements IVaccinazioniEseguiteProvider.UpdateNumeroVacVacEseguita

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand("update T_VAC_ESEGUITE set ves_n_richiamo = :ves_n_richiamo where VES_ID = :ves_id ", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("ves_n_richiamo", numeroVac)
                    cmd.Parameters.AddWithValue("ves_id", ves_id)


                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Funzione che aggiorna il valore della dose associazione nella T_VAC_ESEGUITE
        ''' </summary>
        ''' <param name="ves_id"></param>
        ''' <param name="numeroAssociazione"></param>
        ''' <returns></returns>
        Public Function UpdateNumeroAssociazioneVacEseguita(ves_id As Integer, numeroAssociazione As Integer) As Integer Implements IVaccinazioniEseguiteProvider.UpdateNumeroAssociazioneVacEseguita

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand("update T_VAC_ESEGUITE set ves_ass_n_dose = :ves_ass_n_dose where VES_ID = :ves_id ", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("ves_ass_n_dose", numeroAssociazione)
                    cmd.Parameters.AddWithValue("ves_id", ves_id)


                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Funzione che aggiorna il valore della dose vaccinazione nella T_VAC_SCADUTE
        ''' </summary>
        ''' <param name="vsc_id"></param>
        ''' <param name="numeroVac"></param>
        ''' <returns></returns>
        Public Function UpdateNumeroVacVacScadute(vsc_id As Integer, numeroVac As Integer) As Integer Implements IVaccinazioniEseguiteProvider.UpdateNumeroVacVacScadute

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand("update T_VAC_SCADUTE set vsc_n_richiamo = :vsc_n_richiamo where VSC_ID = :vsc_id ", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("vsc_n_richiamo", numeroVac)
                    cmd.Parameters.AddWithValue("vsc_id", vsc_id)


                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Funzione che aggiorna il valore della dose associazione nella T_VAC_SCADUTE
        ''' </summary>
        ''' <param name="vsc_id"></param>
        ''' <param name="numeroAssociazione"></param>
        ''' <returns></returns>
        Public Function UpdateNumeroAssociazioneVacScadute(vsc_id As Integer, numeroAssociazione As Integer) As Integer Implements IVaccinazioniEseguiteProvider.UpdateNumeroAssociazioneVacScadute

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand("update T_VAC_SCADUTE set vsc_ass_n_dose = :vsc_ass_n_dose where VSC_ID = :vsc_id ", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("vsc_ass_n_dose", numeroAssociazione)
                    cmd.Parameters.AddWithValue("vsc_id", vsc_id)


                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

#End Region

    End Class

End Namespace
