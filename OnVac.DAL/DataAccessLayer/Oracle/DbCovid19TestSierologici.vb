Imports System.Collections.Generic
Imports System.Data.OracleClient
Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.Data.OracleClient
Imports Onit.OnAssistnet.OnVac.DAL

Public Class DbCovid19TestSierologici
    Inherits DbProvider
    Implements ICovid19TestSierologici

#Region " Constructors "
    Public Sub New(ByRef DAM As IDAM)
        MyBase.New(DAM)
    End Sub
#End Region

    Public Function GetTestSierologiciByCodiceFiscale(codiceFiscale As String) As List(Of TestRapidoScaricoScreening) Implements ICovid19TestSierologici.GetTestSierologiciByCodiceFiscale
        Return DoCommand(Function(cmd)
                             Dim result As New List(Of TestRapidoScaricoScreening)
                             cmd.CommandText = "SELECT TSS_CAMPIONE_NR, TSS_COGNOME, TSS_NOME, TSS_CODICE_FISCALE, TSS_DATA_DI_NASCITA, TSS_LUOGO_DI_NASCITA, TSS_SESSO, TSS_RESIDENZA, TSS_DATA_PRELIEVO, TSS_CENTRO, TSS_ULSS_RES, TSS_ULSS_RICHIEDENTE, TSS_DATA_REFERTO, TSS_IGG, TSS_SPS_ID, SPS_DESCRIZIONE, TSS_TPS_ID, TPS_DESCRIZIONE, TSS_TEST_VALIDO, TSS_IGM, TSS_MARCA 
                                FROM T_PAZ_TEST_SIEROLOGICI 
                                left JOIN T_TIPO_SETTING_P_SCOLASTICO on TSS_SPS_ID = SPS_ID
                                left join T_TIPO_PERSONALE_SCOLASTICO on TSS_TPS_ID = TPS_ID
                                WHERE TSS_CODICE_FISCALE = ?TSS_CODICE_FISCALE "


                             cmd.AddParameter("TSS_CODICE_FISCALE", codiceFiscale)

                             Using _context As IDataReader = cmd.ExecuteReader()
                                 Dim scs_campione_nr As Integer = _context.GetOrdinal("TSS_CAMPIONE_NR")
                                 Dim scs_cognome As Integer = _context.GetOrdinal("TSS_COGNOME")
                                 Dim scs_nome As Integer = _context.GetOrdinal("TSS_NOME")
                                 Dim scs_codice_fiscale As Integer = _context.GetOrdinal("TSS_CODICE_FISCALE")
                                 Dim scs_data_di_Nascita As Integer = _context.GetOrdinal("TSS_DATA_DI_NASCITA")
                                 Dim scs_luogo_di_nascita As Integer = _context.GetOrdinal("TSS_LUOGO_DI_NASCITA")
                                 Dim scs_sesso As Integer = _context.GetOrdinal("TSS_SESSO")
                                 Dim scs_Residenza As Integer = _context.GetOrdinal("TSS_RESIDENZA")
                                 Dim scs_DataPrelievo As Integer = _context.GetOrdinal("TSS_DATA_PRELIEVO")
                                 Dim scs_centro As Integer = _context.GetOrdinal("TSS_CENTRO")
                                 Dim scs_ulss_res As Integer = _context.GetOrdinal("TSS_ULSS_RES")
                                 Dim scs_ulss_richiedente As Integer = _context.GetOrdinal("TSS_ULSS_RICHIEDENTE")
                                 Dim scs_data_referto As Integer = _context.GetOrdinal("TSS_DATA_REFERTO")
                                 Dim scs_esito As Integer = _context.GetOrdinal("TSS_IGG")

                                 Dim codiceSettings As Integer = _context.GetOrdinal("TSS_SPS_ID")
                                 Dim descrizioneSettings As Integer = _context.GetOrdinal("SPS_DESCRIZIONE")
                                 Dim codiceScolastico As Integer = _context.GetOrdinal("TSS_TPS_ID")
                                 Dim descrizioneScolastico As Integer = _context.GetOrdinal("TPS_DESCRIZIONE")

                                 Dim valido As Integer = _context.GetOrdinal("TSS_TEST_VALIDO")
                                 Dim esitoIGM As Integer = _context.GetOrdinal("TSS_IGM")
                                 Dim marca As Integer = _context.GetOrdinal("TSS_MARCA")

                                 While _context.Read()
                                     Dim test As TestRapidoScaricoScreening = New TestRapidoScaricoScreening()
                                     test.IdTest = _context.GetInt64OrDefault(scs_campione_nr)
                                     test.Cognome = _context.GetStringOrDefault(scs_cognome)
                                     test.Nome = _context.GetStringOrDefault(scs_nome)
                                     test.CodiceFiscale = _context.GetStringOrDefault(scs_codice_fiscale)
                                     test.DataDiNascita = _context.GetDateTimeOrDefault(scs_data_di_Nascita)
                                     test.LuogoDiNascita = _context.GetStringOrDefault(scs_luogo_di_nascita)
                                     test.Sesso = _context.GetStringOrDefault(scs_sesso)
                                     test.Residenza = _context.GetStringOrDefault(scs_Residenza)
                                     test.DataPrelievo = _context.GetDateTimeOrDefault(scs_DataPrelievo)
                                     test.Centro = _context.GetStringOrDefault(scs_centro)
                                     test.UlssRes = _context.GetStringOrDefault(scs_ulss_res)
                                     test.UlssRichiedente = _context.GetStringOrDefault(scs_ulss_richiedente)
                                     test.DataReferto = _context.GetDateTimeOrDefault(scs_data_referto)
                                     test.EsitoIGG = _context.GetStringOrDefault(scs_esito)
                                     test.CodiceSettingsSPS = _context.GetNullableInt64OrDefault(codiceSettings)
                                     test.DescrizioneSettings = _context.GetStringOrDefault(descrizioneSettings)
                                     test.CodiceTipoScolasticoTPS = _context.GetNullableInt64OrDefault(codiceScolastico)
                                     test.DescrizioneScolastico = _context.GetStringOrDefault(descrizioneScolastico)
                                     test.Marca = _context.GetStringOrDefault(marca)
                                     test.Valido = SNtoBool(_context.GetStringOrDefault(valido))
                                     test.EsitoIGM = _context.GetStringOrDefault(esitoIGM)
                                     result.Add(test)
                                 End While
                             End Using
                             Return result

                         End Function)
    End Function

    Public Function GetTestSierologiciPaziente(codicePaziente As Long) As List(Of TestSierologicoPaziente) Implements ICovid19TestSierologici.GetTestSierologiciPaziente
        Return DoCommand(Function(cmd)
                             cmd.CommandText = "SELECT
                                                    TSS_TIPO_TEST AS Tipologia,
                                                    TSS_CAMPIONE_NR AS CodiceCampione,
                                                    TSS_PAZ_CODICE AS CodicePaziente,
                                                    NVL(p.PAZ_NOME, TSS_NOME) AS Nome,
                                                    nvl(p.PAZ_COGNOME, TSS_COGNOME) AS Cognome,
                                                    nvl(p.PAZ_CODICE_FISCALE, TSS_CODICE_FISCALE) AS CodiceFiscale,
                                                    nvl(p.PAZ_DATA_NASCITA, TSS_DATA_DI_NASCITA) AS DataDiNascita,
                                                    TSS_LUOGO_DI_NASCITA AS LuogoDiNascita,
                                                    nvl(p.PAZ_SESSO, TSS_SESSO) AS Sesso,
                                                    TSS_RESIDENZA AS Residenza,
                                                    TSS_DATA_PRELIEVO AS DataPrelievo,
                                                    TSS_CENTRO AS Centro,
                                                    TSS_ULSS_RICHIEDENTE  AS UlssRichiedente,
                                                    TSS_DATA_REFERTO AS DataReferto,
                                                    TSS_IGG AS IGG,
                                                    TSS_IGG_UNITA_MISURA AS IGGUnita,
                                                    TSS_IGG_DESCRIZIONE_RANGE AS IGGDescrizione,
                                                    TSS_IGM AS IGM,
                                                    TSS_IGM_UNITA_MISURA AS IGMUnita,
                                                    TSS_IGM_DESCRIZIONE_RANGE AS IGMDescrizione,
                                                    TSS_ESITO_ANTIGENE AS EsitoRapido,
                                                    TSS_SPS_ID AS CodiceSettingScolastico,
                                                    sps_descrizione AS descrizioneSettingScolastico,
                                                    TSS_TPS_ID AS CodicePersonaleScolastico,
                                                    tps_descrizione AS DescrizionePersonaleScolastico,
                                                    TSS_TEST_VALIDO AS Valido,
                                                    TSS_MARCA AS Marca,
                                                    TSS_RICHIEDENTE  AS Richiedente,
                                                    TSS_CENTRO as CodiceLaboratorio,
                                                    TSS_CENTRO_EXT DescrizioneLaboratorio
                                                        FROM V_PAZ_TEST_SIEROLOGICI 
                                                        left JOIN T_TIPO_SETTING_P_SCOLASTICO on TSS_SPS_ID = SPS_ID
                                                        left join T_TIPO_PERSONALE_SCOLASTICO on TSS_TPS_ID = TPS_ID
                                                        LEFT JOIN T_PAZ_PAZIENTI p ON p.paz_codice = tss_paz_codice "
                             cmd.CommandText += " WHERE TSS_PAZ_CODICE = ?paz"
                             cmd.AddParameter("paz", codicePaziente)
                             Return cmd.Fill(Of TestSierologicoPaziente)
                         End Function)
    End Function

    Private Function _InsertTestSierologico(test As TestRapidoScaricoScreening) As Long
        Return DoCommand(Function(cmd)
                             Dim campNumero As Long = 0

                             cmd.CommandText = "select SEQ_TSS_CAMPIONE_NR.nextval from DUAL"

                             campNumero = Convert.ToInt64(cmd.ExecuteScalar())

                             cmd.Parameters.Clear()
                             cmd.CommandText = "INSERT INTO T_PAZ_TEST_SIEROLOGICI (TSS_CAMPIONE_NR, TSS_COGNOME, TSS_NOME, TSS_CODICE_FISCALE, TSS_DATA_DI_NASCITA, TSS_DATA_PRELIEVO, TSS_LUOGO_DI_NASCITA, TSS_SESSO, TSS_RESIDENZA, TSS_CENTRO, TSS_ULSS_RES, TSS_ULSS_RICHIEDENTE, TSS_DATA_REFERTO, TSS_IGG, TSS_SPS_ID, TSS_TPS_ID, TSS_IGM, TSS_TEST_VALIDO, TSS_MARCA) " +
            "values(:TSS_CAMPIONE_NR, :TSS_COGNOME, :TSS_NOME, :TSS_CODICE_FISCALE, :TSS_DATA_DI_NASCITA, :TSS_DATA_PRELIEVO, :TSS_LUOGO_DI_NASCITA, :TSS_SESSO, :TSS_RESIDENZA, :TSS_CENTRO, :TSS_ULSS_RES, :TSS_ULSS_RICHIEDENTE, :TSS_DATA_REFERTO, :TSS_IGG, :TSS_SPS_ID, :TSS_TPS_ID, :TSS_IGM, :TSS_TEST_VALIDO, :TSS_MARCA) "

                             cmd.AddParameter("TSS_CAMPIONE_NR", campNumero)
                             cmd.AddParameter("TSS_COGNOME", test.Cognome)
                             cmd.AddParameter("TSS_NOME", test.Nome)
                             cmd.AddParameter("TSS_CODICE_FISCALE", test.CodiceFiscale)
                             cmd.AddParameter("TSS_DATA_DI_NASCITA", test.DataDiNascita)
                             cmd.AddParameter("TSS_DATA_PRELIEVO", test.DataPrelievo)
                             cmd.AddParameter("TSS_LUOGO_DI_NASCITA", test.LuogoDiNascita)
                             cmd.AddParameter("TSS_SESSO", test.Sesso)
                             cmd.AddParameter("TSS_RESIDENZA", test.Residenza)
                             cmd.AddParameter("TSS_CENTRO", test.Centro)
                             cmd.AddParameter("TSS_ULSS_RES", test.UlssRes)
                             cmd.AddParameter("TSS_ULSS_RICHIEDENTE", test.UlssRichiedente)
                             cmd.AddParameter("TSS_DATA_REFERTO", test.DataReferto)
                             cmd.AddParameter("TSS_IGG", test.EsitoIGG)
                             cmd.AddParameter("TSS_IGM", test.EsitoIGM)
                             cmd.AddParameter("TSS_MARCA", test.Marca)
                             cmd.AddParameter("TSS_TEST_VALIDO", BoolToSN(test.Valido))

                             If test.CodiceSettingsSPS.HasValue Then
                                 cmd.AddParameter("TSS_SPS_ID", test.CodiceSettingsSPS.Value)
                             Else
                                 cmd.AddParameter("TSS_SPS_ID", DBNull.Value)
                             End If


                             If test.CodiceTipoScolasticoTPS.HasValue Then
                                 cmd.AddParameter("TSS_TPS_ID", test.CodiceTipoScolasticoTPS.Value)
                             Else
                                 cmd.AddParameter("TSS_TPS_ID", DBNull.Value)
                             End If

                             cmd.ExecuteNonQuery()
                             Return campNumero

                         End Function)
    End Function

    Public Function InsertTestSierologico(test As TestRapidoScaricoScreening) As ResultTestSierologici Implements ICovid19TestSierologici.InsertTestSierologico
        Dim result As New ResultTestSierologici()

        Dim ownConnection As Boolean = False
        Try
            Using cmd As New OracleClient.OracleCommand()
                cmd.Connection = Connection
                ownConnection = ConditionalOpenConnection(cmd)
                result.IdTest = _InsertTestSierologico(test)
                result.Success = True
                result.Message = "Insert eseguito con successo"
            End Using
        Catch ex As Exception
            result.Message = ex.Message
            result.Success = False
            result.IdTest = 0
        Finally
            ConditionalCloseConnection(ownConnection)
        End Try
        Return result
    End Function
    Private Function _InsertScaricoScreening(test As TestRapidoScaricoScreening) As ResultSetPost
        Return DoCommand(Function(cmd)

                             Dim result As New ResultSetPost()

                             cmd.CommandText = "INSERT INTO T_ANA_SCARICO_SCREENING (SCS_CAMPIONE_NR, SCS_COGNOME, SCS_NOME, SCS_CODICE_FISCALE, SCS_DATA_DI_NASCITA, SCS_DATA_PRELIEVO, SCS_LUOGO_DI_NASCITA, SCS_SESSO, SCS_RESIDENZA, SCS_CENTRO, SCS_ULSS_RES, SCS_ULSS_RICHIEDENTE, SCS_DATA_REFERTO, SCS_TPS_ID, SCS_SPS_ID, SCS_IGG, SCS_IGM, SCS_TEST_VALIDO, SCS_MARCA) " +
            "values(:SCS_CAMPIONE_NR, :SCS_COGNOME, :SCS_NOME, :SCS_CODICE_FISCALE, :SCS_DATA_DI_NASCITA, :SCS_DATA_PRELIEVO, :SCS_LUOGO_DI_NASCITA, :SCS_SESSO, :SCS_RESIDENZA, :SCS_CENTRO, :SCS_ULSS_RES, :SCS_ULSS_RICHIEDENTE, :SCS_DATA_REFERTO, :SCS_TPS_ID, :SCS_SPS_ID, :SCS_IGG, :SCS_IGM, :SCS_TEST_VALIDO, :SCS_MARCA)"

                             cmd.AddParameter("SCS_CAMPIONE_NR", test.IdTest)
                             cmd.AddParameter("SCS_COGNOME", test.Cognome)
                             cmd.AddParameter("SCS_NOME", test.Nome)
                             cmd.AddParameter("SCS_CODICE_FISCALE", test.CodiceFiscale)
                             cmd.AddParameter("SCS_DATA_DI_NASCITA", test.DataDiNascita)
                             cmd.AddParameter("SCS_DATA_PRELIEVO", test.DataPrelievo)
                             cmd.AddParameter("SCS_LUOGO_DI_NASCITA", test.LuogoDiNascita)
                             cmd.AddParameter("SCS_SESSO", test.Sesso)
                             cmd.AddParameter("SCS_RESIDENZA", test.Residenza)
                             cmd.AddParameter("SCS_CENTRO", test.Centro)
                             cmd.AddParameter("SCS_ULSS_RES", test.UlssRes)
                             cmd.AddParameter("SCS_ULSS_RICHIEDENTE", test.UlssRichiedente)
                             cmd.AddParameter("SCS_DATA_REFERTO", test.DataReferto)
                             cmd.AddParameter("SCS_IGG", test.EsitoIGG)
                             cmd.AddParameter("SCS_TEST_VALIDO", BoolToSN(test.Valido))
                             cmd.AddParameter("SCS_IGM", test.EsitoIGM)
                             cmd.AddParameter("SCS_MARCA", test.Marca)

                             If test.CodiceSettingsSPS.HasValue Then
                                 cmd.AddParameter("SCS_SPS_ID", test.CodiceSettingsSPS.Value)
                             Else
                                 cmd.AddParameter("SCS_SPS_ID", DBNull.Value)
                             End If


                             If test.CodiceTipoScolasticoTPS.HasValue Then
                                 cmd.AddParameter("SCS_TPS_ID", test.CodiceTipoScolasticoTPS.Value)
                             Else
                                 cmd.AddParameter("SCS_TPS_ID", DBNull.Value)
                             End If
                             cmd.ExecuteNonQuery()

                             result.Message = "Insert eseguito con successo"
                             result.Success = True
                             Return result

                         End Function)
    End Function

    Public Function InsertScaricoScreening(test As TestRapidoScaricoScreening) As ResultSetPost Implements ICovid19TestSierologici.InsertScaricoScreening
        Try
            Return _InsertScaricoScreening(test)
        Catch ex As Exception
            Dim result As New ResultSetPost()
            result.Message = ex.Message
            result.Success = False
            Return result
        End Try
    End Function

    Public Function InsertTest(test As TestRapidoScaricoScreening) As ResultTestSierologici Implements ICovid19TestSierologici.InsertTest
        Try
            Return DoCommand(Function(cmd)
                                 Dim codice As Long = _InsertTestSierologico(test)
                                 test.IdTest = codice
                                 cmd.Parameters.Clear()
                                 Dim result As ResultSetPost = _InsertScaricoScreening(test)
                                 Return New ResultTestSierologici With {
                                .Message = result.Message,
                                .IdTest = codice,
                                .Success = result.Success
                             }
                             End Function, IsolationLevel.ReadCommitted)
        Catch ex As Exception
            Return New ResultTestSierologici With {
                .Message = ex.Message,
                .Success = False
            }
        End Try

    End Function
End Class

