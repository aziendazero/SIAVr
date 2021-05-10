Imports System.Collections.Generic
Imports System.Data.OracleClient
Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.Entities


Namespace DAL.Oracle

    Public Class DbAnagraficheProvider
        Inherits DbProvider
        Implements IAnagraficheProvider

#Region " Costruttore "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " IAnaBilancioProvider "

#Region " Anagrafica Origine Etnica "

        ''' <summary>
        ''' Restituisce le anagrafiche delle origine etniche
        ''' </summary>
        ''' <param name="codiceMalattia"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetListaOrigineEtnica(codice As String) As List(Of Entities.OrigineEtnica) Implements IAnagraficheProvider.GetListaOrigineEtnica

            Dim list As List(Of Entities.OrigineEtnica) = Nothing

            Dim ownConnection As Boolean = False

            Try
                Dim whereCodice As String = ""
                If Not codice.IsNullOrEmpty Then
                    whereCodice = " where OET_CODICE = " + codice
                End If
                Dim query As String = String.Format("SELECT * FROM T_ANA_ORIGINE_ETNICA {0} order by OET_CODICE ", whereCodice)

                Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    list = GetListOrigineEtnica(cmd)

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return list

        End Function
        Public Function GetTableOrigineEtnica(codice As String) As DataTable Implements IAnagraficheProvider.GetTableOrigineEtnica
            Dim dta As New DataTable()

            With _DAM.QB

                .NewQuery()

                .AddSelectFields("OET_CODICE,OET_DESCRIZIONE,OET_OBSOLETO,OET_CODICE_ESTERNO")
                .AddTables("T_ANA_ORIGINE_ETNICA")
                If Not codice.IsNullOrEmpty Then
                    .AddWhereCondition("OET_CODICE", Comparatori.Uguale, codice, DataTypes.Stringa)
                End If

                .AddOrderByFields("OET_CODICE")

            End With

            _DAM.BuildDataTable(dta)

            Return dta
        End Function


        Private Function GetListOrigineEtnica(cmd As OracleClient.OracleCommand) As List(Of Entities.OrigineEtnica)

            Dim list As New List(Of Entities.OrigineEtnica)()

            Using idr As IDataReader = cmd.ExecuteReader()
                If Not idr Is Nothing Then

                    Dim oet_codice As Integer = idr.GetOrdinal("OET_CODICE")
                    Dim oet_descrizione As Integer = idr.GetOrdinal("OET_DESCRIZIONE")
                    Dim oet_obsoleto As Integer = idr.GetOrdinal("OET_OBSOLETO")
                    Dim oet_codice_esterno As Integer = idr.GetOrdinal("OET_CODICE_ESTERNO")


                    While idr.Read()

                        Dim item As New Entities.OrigineEtnica()
                        item.Codice = idr.GetInt32(oet_codice)
                        item.Descrizione = idr.GetStringOrDefault(oet_descrizione)
                        item.Obsoleto = idr.GetString(oet_obsoleto)
                        item.CodiceEsterno = idr.GetStringOrDefault(oet_codice_esterno)

                        list.Add(item)

                    End While

                End If
            End Using

            Return list

        End Function
        Public Function GetListaNomiCommercialiIndicazioni(codice As String, filter As String) As List(Of Entities.AnagrafeCodiceDescrizione) Implements IAnagraficheProvider.GetListaNomiCommercialiIndicazioni

            Dim list As List(Of Entities.AnagrafeCodiceDescrizione) = Nothing

            Dim ownConnection As Boolean = False

            Try
                Dim whereCodice As String = ""
                If Not codice.IsNullOrEmpty Then
                    whereCodice = " AND NOI_CODICE = " + codice
                End If
                Dim wherefilter As String = ""
                If Not filter.IsNullOrEmpty Then
                    wherefilter = String.Format("and ( upper(NOI_CODICE) LIKE '%{0}%' OR UPPER(NOI_DESCRIZIONE) LIKE '%{0}%') ", filter.ToUpper())
                End If


                Dim query As String = String.Format("SELECT * FROM T_ANA_NOMI_COM_INDICAZIONI Where NOI_OBSOLETO='N' {0} {1} order by NOI_CODICE ", whereCodice, wherefilter)

                Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    list = GetListNomiCommercialiIndicazione(cmd)

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return list

        End Function

        Public Function GetMotivazioneVariante(codice As Long) As MotivazioneVariante Implements IAnagraficheProvider.GetMotivazioneVariante
            Return DoCommand(Function(cmd)
                                 cmd.CommandText = "SELECT TMG_CODICE as Codice, TMG_MNEMONICO as Mnemonico, TMG_DESCRIZIONE as Descrizione, TMG_VAL_ISS as ValoreISS FROM T_ANA_MOTIVI_GENOTIP  where TMG_CODICE = ?codice"
                                 cmd.AddParameter("codice", codice)
                                 Return cmd.FirstOrDefault(Of MotivazioneVariante)
                             End Function)
        End Function

        Public Function ElencoMotivazioniVariante(filtro As String) As IEnumerable(Of MotivazioneVariante) Implements IAnagraficheProvider.ElencoMotivazioniVariante
            Return DoCommand(Function(cmd)
                                 cmd.CommandText = "SELECT TMG_CODICE as Codice, TMG_MNEMONICO as Mnemonico, TMG_DESCRIZIONE as Descrizione, TMG_VAL_ISS as ValoreISS FROM T_ANA_MOTIVI_GENOTIP "
                                 If Not String.IsNullOrWhiteSpace(filtro) Then
                                     cmd.CommandText += " where UPPER(TMG_DESCRIZIONE) like ?filtro"
                                     cmd.AddParameter("filtro", String.Format("%{0}", filtro.ToUpper()))
                                 End If
                                 Return cmd.Fill(Of MotivazioneVariante)
                             End Function)
        End Function

        Public Function ElencoVariantiCovid(filtro As String) As IEnumerable(Of VarianteCovid) Implements IAnagraficheProvider.ElencoVariantiCovid
            Return DoCommand(Function(cmd)
                                 cmd.CommandText = "SELECT TAV_CODICE AS Codice, TAV_DESCRIZIONE AS descrizione, TAV_MNEMONICO AS mnemonico FROM T_ANA_TIPI_VARIANTI"
                                 If Not String.IsNullOrWhiteSpace(filtro) Then
                                     cmd.CommandText += " where UPPER(tav_descrizione) like ?desc"
                                     cmd.AddParameter("desc", String.Format("%{0}", filtro.ToUpper()))
                                 End If
                                 Return cmd.Fill(Of VarianteCovid)
                             End Function)
        End Function

        Public Function GetVarianteCovid(codice As Long) As VarianteCovid Implements IAnagraficheProvider.GetVarianteCovid
            Return DoCommand(Function(cmd)
                                 cmd.CommandText = "SELECT TAV_CODICE AS Codice, TAV_DESCRIZIONE AS descrizione, TAV_MNEMONICO AS mnemonico FROM T_ANA_TIPI_VARIANTI where tav_codice = ?codice"
                                 cmd.AddParameter("codice", codice)
                                 Return cmd.FirstOrDefault(Of VarianteCovid)
                             End Function)
        End Function

        Public Function GetTipologieTamponi() As IEnumerable(Of TipologiaTampone) Implements IAnagraficheProvider.GetTipologieTamponi
            Return DoCommand(Function(cmd)
                                 cmd.CommandText = "SELECT 
	                                                    TAT_CODICE  AS Codice,
	                                                    TAT_MNEMONICO AS Mnemonico,
	                                                    TAT_DESCRIZIONE AS Descrizione
                                                    FROM T_ANA_TIPI_TAMPONE"
                                 Return cmd.Fill(Of TipologiaTampone)
                             End Function)
        End Function

        Private Function GetListNomiCommercialiIndicazione(cmd As OracleClient.OracleCommand) As List(Of Entities.AnagrafeCodiceDescrizione)

            Dim list As New List(Of Entities.AnagrafeCodiceDescrizione)()

            Using idr As IDataReader = cmd.ExecuteReader()
                If Not idr Is Nothing Then

                    Dim noi_codice As Integer = idr.GetOrdinal("NOI_CODICE")
                    Dim noi_descrizione As Integer = idr.GetOrdinal("NOI_DESCRIZIONE")
                    Dim noi_obsoleto As Integer = idr.GetOrdinal("NOI_OBSOLETO")
                    Dim noi_codice_esterno As Integer = idr.GetOrdinal("NOI_CODICE_ESTERNO")


                    While idr.Read()

                        Dim item As New Entities.AnagrafeCodiceDescrizione()
                        item.Codice = idr.GetString(noi_codice)
                        item.Descrizione = idr.GetStringOrDefault(noi_descrizione)
                        item.Obsoleto = idr.GetString(noi_obsoleto)
                        item.CodiceEsterno = idr.GetStringOrDefault(noi_codice_esterno)

                        list.Add(item)

                    End While

                End If
            End Using

            Return list

        End Function

        Public Function GetListSintomi(Filtro As String) As List(Of Sintomi) Implements IAnagraficheProvider.GetListSintomi
            Dim list As List(Of Entities.Sintomi) = New List(Of Sintomi)

            Dim ownConnection As Boolean = False

            Try

                Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                    If String.IsNullOrWhiteSpace(Filtro) Then
                        cmd.CommandText = "select ASI_CODICE, ASI_DESCRIZIONE, ASI_ORDINE from T_ANA_SINTOMI where ASI_OBSOLETO is null or ASI_OBSOLETO <> 'S' order by ASI_ORDINE"
                    Else
                        cmd.CommandText = "select ASI_CODICE, ASI_DESCRIZIONE, ASI_ORDINE from T_ANA_SINTOMI where (ASI_OBSOLETO is null or ASI_OBSOLETO <> 'S') and ASI_DESCRIZIONE like :filtro order by ASI_ORDINE"
                        cmd.Parameters.AddWithValue("filtro", String.Format("{0}%", Filtro.ToUpper()))
                    End If
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using Reader As IDataReader = cmd.ExecuteReader()
                        Dim iCodice As Integer = Reader.GetOrdinal("ASI_CODICE")
                        Dim iDescrizione As Integer = Reader.GetOrdinal("ASI_DESCRIZIONE")
                        Dim iOrdine As Integer = Reader.GetOrdinal("ASI_ORDINE")
                        While Reader.Read()
                            Dim tmp As New Sintomi
                            tmp.Codice = Reader.GetInt32(iCodice)
                            tmp.Descrizione = Reader.GetString(iDescrizione)
                            tmp.Ordine = Reader.GetInt32(iOrdine)
                            list.Add(tmp)
                        End While

                    End Using


                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return list
        End Function

        Public Function GetListSegnalatore(Filtro As String) As List(Of Segnalatore) Implements IAnagraficheProvider.GetListSegnalatore
            Dim list As New List(Of Entities.Segnalatore)

            Dim ownConnection As Boolean = False

            Try

                Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                    If String.IsNullOrWhiteSpace(Filtro) Then
                        cmd.CommandText = "select ASE_CODICE, ASE_DESCRIZIONE, ASE_ORDINE from T_ANA_SEGNALATORE where ASE_OBSOLETO <> 'S' order by ASE_ORDINE"
                    Else
                        cmd.CommandText = "select ASE_CODICE, ASE_DESCRIZIONE, ASE_ORDINE from T_ANA_SEGNALATORE where ASE_OBSOLETO <> 'S' and UPPER(ASE_DESCRIZIONE) like :filtro order by ASE_ORDINE"
                        cmd.Parameters.AddWithValue("filtro", String.Format("{0}%", Filtro.ToUpper()))
                    End If
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using Reader As IDataReader = cmd.ExecuteReader()
                        Dim iCodice As Integer = Reader.GetOrdinal("ASE_CODICE")
                        Dim iDescrizione As Integer = Reader.GetOrdinal("ASE_DESCRIZIONE")
                        Dim iOrdine As Integer = Reader.GetOrdinal("ASE_ORDINE")
                        While Reader.Read()
                            Dim tmp As New Segnalatore
                            tmp.Codice = Reader.GetInt32(iCodice)
                            tmp.Descrizione = Reader.GetString(iDescrizione)
                            tmp.Ordine = Reader.GetInt32(iOrdine)
                            list.Add(tmp)
                        End While

                    End Using


                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return list
        End Function

        Public Function GetStatoCov(Filtro As String, soloValidi As Boolean) As List(Of StatoEppisodio) Implements IAnagraficheProvider.GetStatoCov
            Return DoCommand(Function(cmd)
                                 If soloValidi Then
                                     cmd.CommandText = "select SEP_CODICE, SEP_DESCRIZIONE, SEP_ORDINE, SEP_DECEDUTO from T_ANA_STATI_EPISODIO where SEP_OBSOLETO <> 'S'"
                                 Else
                                     cmd.CommandText = "select SEP_CODICE, SEP_DESCRIZIONE, SEP_ORDINE, SEP_DECEDUTO from T_ANA_STATI_EPISODIO"
                                 End If
                                 If Not String.IsNullOrWhiteSpace(Filtro) Then
                                     cmd.CommandText += " and UPPER(SEP_DESCRIZIONE) like ?filtro"
                                     cmd.AddParameter("filtro", String.Format("{0}%", Filtro.ToUpper()))
                                 End If
                                 cmd.CommandText += " order by SEP_ORDINE"
                                 Return cmd.Fill(Of StatoEppisodio)
                             End Function)
        End Function

        Public Function GetListTipoAzienda(filtro As String) As List(Of TipoAzienda) Implements IAnagraficheProvider.GetListTipoAzienda
            Dim list As New List(Of Entities.TipoAzienda)

            Dim ownConnection As Boolean = False

            Try

                Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                    If String.IsNullOrWhiteSpace(filtro) Then
                        cmd.CommandText = "select ATA_CODICE, ATA_DESCRIZIONE, ATA_ORDINE from T_ANA_TIPO_AZIENDA where ATA_OBSOLETO <> 'S' order by ATA_ORDINE"
                    Else
                        cmd.CommandText = "select ATA_CODICE, ATA_DESCRIZIONE, ATA_ORDINE from T_ANA_TIPO_AZIENDA where ATA_OBSOLETO <> 'S' and UPPER(ATA_DESCRIZIONE) like :filtro order by ATA_ORDINE"
                        cmd.Parameters.AddWithValue("filtro", String.Format("{0}%", filtro.ToUpper()))
                    End If
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using Reader As IDataReader = cmd.ExecuteReader()
                        Dim iCodice As Integer = Reader.GetOrdinal("ATA_CODICE")
                        Dim iDescrizione As Integer = Reader.GetOrdinal("ATA_DESCRIZIONE")
                        Dim iOrdine As Integer = Reader.GetOrdinal("ATA_ORDINE")
                        While Reader.Read()
                            Dim tmp As New TipoAzienda
                            tmp.Codice = Reader.GetInt32(iCodice)
                            tmp.Descrizione = Reader.GetString(iDescrizione)
                            tmp.Ordine = Reader.GetInt32(iOrdine)
                            list.Add(tmp)
                        End While

                    End Using


                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return list
        End Function

        Public Function GetListTipoContatto(Filtro As String) As List(Of TipoContatto) Implements IAnagraficheProvider.GetListTipoContatto
            Dim list As New List(Of Entities.TipoContatto)

            Dim ownConnection As Boolean = False

            Try

                Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                    If String.IsNullOrWhiteSpace(Filtro) Then
                        cmd.CommandText = "select ATC_CODICE, ATC_DESCRIZIONE, ATC_ORDINE from T_ANA_TIPO_CONTATTO where ATC_OBSOLETO <> 'S' order by ATC_ORDINE"
                    Else
                        cmd.CommandText = "select ATC_CODICE, ATC_DESCRIZIONE, ATC_ORDINE from T_ANA_TIPO_CONTATTO where ATC_OBSOLETO <> 'S' and UPPER(ATC_DESCRIZIONE) like :filtro order by ATC_ORDINE"
                        cmd.Parameters.AddWithValue("filtro", String.Format("{0}%", Filtro.ToUpper()))
                    End If
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using Reader As IDataReader = cmd.ExecuteReader()
                        Dim iCodice As Integer = Reader.GetOrdinal("ATC_CODICE")
                        Dim iDescrizione As Integer = Reader.GetOrdinal("ATC_DESCRIZIONE")
                        Dim iOrdine As Integer = Reader.GetOrdinal("ATC_ORDINE")
                        While Reader.Read()
                            Dim tmp As New TipoContatto
                            tmp.Codice = Reader.GetInt32(iCodice)
                            tmp.Descrizione = Reader.GetString(iDescrizione)
                            tmp.Ordine = Reader.GetInt32(iOrdine)
                            list.Add(tmp)
                        End While

                    End Using


                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return list
        End Function

        Public Function GetReparto(codice As Integer) As Reparto Implements IAnagraficheProvider.GetReparto
            Return DoCommand(Function(cmd)
                                 cmd.CommandText = "select ARE_CODICE as Codice, ARE_DESCRIZIONE as Descrizione, ARE_ORDINE as Ordine from T_ANA_REPARTI where ARE_CODICE = ?cod"
                                 cmd.AddParameter("cod", codice)
                                 Return cmd.FirstOrDefault(Of Reparto)
                             End Function)
        End Function
        Public Function GetReparti(Filtro As String) As List(Of Reparto) Implements IAnagraficheProvider.GetReparti
            Dim list As New List(Of Entities.Reparto)

            Dim ownConnection As Boolean = False

            Try

                Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                    If String.IsNullOrWhiteSpace(Filtro) Then
                        cmd.CommandText = "select ARE_CODICE, ARE_DESCRIZIONE, ARE_ORDINE from T_ANA_REPARTI where ARE_OBSOLETO <> 'S' order by ARE_ORDINE"
                    Else
                        cmd.CommandText = "select ARE_CODICE, ARE_DESCRIZIONE, ARE_ORDINE from T_ANA_REPARTI where ARE_OBSOLETO <> 'S' and UPPER(ARE_DESCRIZIONE) like :filtro order by ARE_ORDINE"
                        cmd.Parameters.AddWithValue("filtro", String.Format("{0}%", Filtro.ToUpper()))
                    End If
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using Reader As IDataReader = cmd.ExecuteReader()
                        Dim iCodice As Integer = Reader.GetOrdinal("ARE_CODICE")
                        Dim iDescrizione As Integer = Reader.GetOrdinal("ARE_DESCRIZIONE")
                        Dim iOrdine As Integer = Reader.GetOrdinal("ARE_ORDINE")
                        While Reader.Read()
                            Dim tmp As New Reparto
                            tmp.Codice = Reader.GetInt32(iCodice)
                            tmp.Descrizione = Reader.GetString(iDescrizione)
                            Dim ordine As Integer? = Reader.GetNullableInt32OrDefault(iOrdine)
                            If ordine.HasValue Then
                                tmp.Ordine = ordine.Value
                            Else
                                tmp.Ordine = Integer.MaxValue
                            End If
                            list.Add(tmp)
                        End While

                    End Using


                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return list
        End Function

#End Region

#Region "RSA"

        Public Function GetRSA(ULSS As String, Filtro As String) As List(Of RSA) Implements IAnagraficheProvider.GetRSA

            Dim result As New List(Of RSA)()

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleCommand()

                    cmd.Connection = Connection

                    Dim query As String = "SELECT RSA_ID, RSA_DESCRIZIONE FROM V_ANA_RSA {0}"
                    Dim filtri As New Text.StringBuilder()

                    If Not String.IsNullOrWhiteSpace(ULSS) Then
                        filtri.Append(" RSA_CODICE_ASL = :RSA_CODICE_ASL AND ")
                        cmd.Parameters.AddWithValue("RSA_CODICE_ASL", ULSS)
                    End If

                    If Not String.IsNullOrWhiteSpace(Filtro) Then
                        filtri.Append(" RSA_DESCRIZIONE LIKE :Filtro AND ")
                        cmd.Parameters.AddWithValue("Filtro", "%" + Filtro + "%")
                    End If

                    If filtri.Length > 0 Then
                        filtri.RemoveLast(4)
                        filtri.Insert(0, " WHERE ")
                    End If

                    cmd.CommandText = String.Format(query, filtri.ToString())

                    ownConnection = ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()

                        If Not _context Is Nothing Then
                            Dim id As Integer = _context.GetOrdinal("RSA_ID")
                            Dim descrizione As Integer = _context.GetOrdinal("RSA_DESCRIZIONE")

                            While _context.Read()

                                Dim RSA As New RSA()
                                RSA.Id = _context.GetStringOrDefault(id)
                                RSA.Descrizione = _context.GetStringOrDefault(descrizione)
                                result.Add(RSA)
                            End While
                        End If
                    End Using
                End Using
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return result

        End Function

        Public Function GetRSAByIdGruppo(idGruppo As String) As List(Of RSA) Implements IAnagraficheProvider.GetRSAByIdGruppo
            If String.IsNullOrWhiteSpace(idGruppo) Then
                Return New List(Of RSA)
            End If
            Return DoCommand(Function(cmd)
                                 cmd.CommandText = "select 
                                                        RSA_ID as Id,
                                                        RSA_DESCRIZIONE as Descrizione
                                                            from t_ana_link_gruppo_rsa 
                                                            join v_ana_rsa on v_ana_rsa.rsa_id = t_ana_link_gruppo_rsa.LGR_RSA_ID 
                                                                where LGR_GRU_ID = ?LGR_GRU_ID"
                                 cmd.AddParameter("LGR_GRU_ID", idGruppo)
                                 Return cmd.Fill(Of RSA)
                             End Function)
        End Function

        Public Function GetCodiceConsultorioMagazzinoRSA(idRSA As String) As String Implements IAnagraficheProvider.GetCodiceConsultorioMagazzinoRSA

            Dim codice As String = String.Empty

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleCommand("SELECT RSA_CNS_CODICE_MAGAZZINO FROM V_ANA_RSA WHERE RSA_ID = :RSA_ID", Connection)

                    cmd.Parameters.AddWithValue("RSA_ID", idRSA)

                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                        codice = obj.ToString()
                    End If

                End Using
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return codice

        End Function

#End Region

#End Region

    End Class


    Class EpisodioFlat
        Property Codice As Long
        Property CodicePaziente As Long
        Property Nome As String
        Property Cognome As String
        Property CodiceCaso As String
        Property DescrizioneCaso As String
        Property CodiceStato As Integer?
        Property DescrizioneStatoEpisodio As String
        Property CodiceDefinizione As Integer?
        Property DescrizioneDefinizione As String
        Property CodiceFonte As Integer?
        Property DescrizioneFonte As String
        Property DataSegnalazione As Date?
        Property DataInizioIsolamento As Date?
        Property DataUltimoContatto As Date?
        Property DataFineIsolamento As Date?
        Property OperatoreSanitario As String
        Property Note As String
        Property Telefono As String
        Property Email As String
        Property CodiceComuneIsolamento As String
        Property DescrizioneComuneIsolamento As String

        Property CodiceDiaria As Long?
        Property DataRilevazioneDiaria As Date
        Property Asintomatico As String
        Property NoteDiaria As String
        Property CodiceSintomo As Integer?
        Property DescrizioneSintomo As String

        Property CodiceContatto As Long?
        Property NoteContatto As String
        Property CodicePazienteContatto As String
        Property NomeContatto As String
        Property CognomeContatto As String
        Property TelefonoContatto As String
        Property CodiceTipoRapporto As String
        Property DescrizioneTipoRapporto As String

        Property CodiceTampone As Long?
        Property DataRapporto As Date?
        Property DataRichiesta As Date?
        Property DataReferto As Date?
        Property CodiceLaboratorio As String
        Property IdCampione As String
        Property DaVisionare As String
        Property NoteTampone As String
        Property CodiceEsitoTampone As String
        Property DescrizioneEsitoTampone As String

        Property CodiceRicovero As Long?
        Property CodiceStruttura As Integer?
        Property DescrizioneStruttura As String
        Property CodiceReparto As Integer?
        Property DescrizioneReparto As String
        Property NoteRicovero As String
        Property InizioRicovero As Date?
        Property FineRicovero As Date?

        Property Azienda As String
        Property Referente As String
        Property TelefonoAzienda As String
        Property EmailAzienda As String
        Property NoteLavoro As String
        Property ComuneLavoro As String
        Property DescrizioneComuneLavoro As String
        Property CodiceTipoAzienda As Integer?
        Property DescrizioneTipoAzienda As String
    End Class
End Namespace
