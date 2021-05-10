Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager


Namespace DAL.Oracle

    Public Class DbNomiCommercialiProvider
        Inherits DbProvider
        Implements INomiCommercialiProvider

#Region " Costruttori "

        Public Sub New(ByRef DAM As IDAM)
            MyBase.New(DAM)
        End Sub

#End Region

#Region " Public "

#Region " Nomi Commerciali "

        ''' <summary>
        ''' Restituisce la descrizione del nome commerciale in base al codice
        ''' </summary>
        ''' <param name="codiceNomeCommerciale"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDescrizioneNomeCommerciale(codiceNomeCommerciale As String) As String Implements INomiCommercialiProvider.GetDescrizioneNomeCommerciale

            Dim descrizione As String = String.Empty

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("select noc_descrizione from t_ana_nomi_commerciali where noc_codice = :noc_codice", Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("noc_codice", codiceNomeCommerciale)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        descrizione = obj.ToString()
                    End If

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return descrizione

        End Function

        ''' <summary>
        ''' Recupero delle informazioni di default relative a sito inoculazione e via di somministrazione in base all'associazione.
        ''' </summary>
        ''' <param name="codiceNomeCommerciale"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetInfoSomministrazioneDefaultNomeCommerciale(codiceNomeCommerciale As String) As InfoSomministrazione Implements INomiCommercialiProvider.GetInfoSomministrazioneDefaultNomeCommerciale

            Dim infoSomministrazione As New InfoSomministrazione()

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand(OnVac.Queries.NomiCommerciali.OracleQueries.selSitoViaByNomeCommerciale, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("noc_codice", codiceNomeCommerciale)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim noc_sii_codice As Integer = idr.GetOrdinal("NOC_SII_CODICE")
                            Dim sii_descrizione As Integer = idr.GetOrdinal("SII_DESCRIZIONE")
                            Dim noc_vii_codice As Integer = idr.GetOrdinal("NOC_VII_CODICE")
                            Dim vii_descrizione As Integer = idr.GetOrdinal("VII_DESCRIZIONE")

                            If idr.Read() Then
                                infoSomministrazione.CodiceSitoInoculazione = idr.GetStringOrDefault(noc_sii_codice)
                                infoSomministrazione.DescrizioneSitoInoculazione = idr.GetStringOrDefault(sii_descrizione)
                                infoSomministrazione.CodiceViaSomministrazione = idr.GetStringOrDefault(noc_vii_codice)
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
        ''' Restituisce il costo unitario per il nome commerciale specificato
        ''' </summary>
        ''' <param name="codiceNomeCommerciale"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCostoUnitarioNomeCommerciale(codiceNomeCommerciale As String) As Double Implements INomiCommercialiProvider.GetCostoUnitarioNomeCommerciale

            Dim costoUnitario As Double = 0

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("select noc_costo_unitario from t_ana_nomi_commerciali where noc_codice = :noc_codice", Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("noc_codice", codiceNomeCommerciale)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        costoUnitario = Convert.ToDouble(obj)
                    End If

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return costoUnitario

        End Function

        ''' <summary>
        ''' Restituisce l'elenco delle vaccinazioni coperte dal nome commerciale specificato
        ''' </summary>
        ''' <param name="codiceNomeCommerciale"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCodiciVaccinazioniByNomeCommerciale(codiceNomeCommerciale As String) As List(Of String) Implements INomiCommercialiProvider.GetCodiciVaccinazioniByNomeCommerciale

            Dim listCodiciVaccinazioni As List(Of String) = Nothing

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("select val_vac_codice from t_ana_link_noc_associazioni join t_ana_link_ass_vaccinazioni on nal_ass_codice = val_ass_codice where nal_noc_codice = :nal_noc_codice", Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("nal_noc_codice", codiceNomeCommerciale)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            listCodiciVaccinazioni = New List(Of String)()

                            Dim val_vac_codice As Integer = idr.GetOrdinal("val_vac_codice")

                            While idr.Read()
                                listCodiciVaccinazioni.Add(idr.GetString(val_vac_codice))
                            End While

                        End If

                    End Using
                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return listCodiciVaccinazioni

        End Function

        Public Function GetNomeCommerciale(codiceNomeCommerciale As String, filtro As String) As DataTable Implements INomiCommercialiProvider.GetNomeCommerciale


            With _DAM.QB
                .NewQuery()
                .AddSelectFields("*")
                .AddTables("t_ana_nomi_commerciali")
                If Not codiceNomeCommerciale.IsNullOrEmpty Then
                    .AddWhereCondition("NOC_CODICE", Comparatori.Uguale, codiceNomeCommerciale, DataTypes.Stringa)
                End If
                If Not filtro.IsNullOrEmpty Then
                    .OpenParanthesis()
                    .AddWhereCondition("NOC_CODICE", Comparatori.Like, "%" + filtro + "%", DataTypes.Stringa)
                    .AddWhereCondition("NOC_DESCRIZIONE", Comparatori.Like, "%" + filtro + "%", DataTypes.Stringa, "OR")
                    .CloseParanthesis()
                End If
                .AddOrderByFields("NOC_CODICE")
            End With

            Try

                _DAM.BuildDataTable(_DT)

            Catch ex As Exception

                Me.LogError(ex)
                Me.SetErrorMsg("Errore durante la lettura degli dei nomi commerciali dal database")

                _DT = Nothing

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return _DT




        End Function



        Public Function GetCodiceNomeCommercialeByCodiceAic(codiceAic9 As String) As String Implements INomiCommercialiProvider.GetCodiceNomeCommercialeByCodiceAic

            Dim codice As String = String.Empty

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("select NOC_CODICE from T_ANA_NOMI_COMMERCIALI where NOC_CODICE_AIC = :NOC_CODICE_AIC", Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("NOC_CODICE_AIC", codiceAic9)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        codice = Convert.ToString(obj)
                    End If

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return codice

        End Function



        Public Function GetGuidTipoPagamentoByCodiceACN(codiceACN As String) As Byte() Implements INomiCommercialiProvider.GetGuidTipoPagamentoByCodiceACN

            Dim codice As Byte()

            Using cmd As New OracleClient.OracleCommand("select TPA_GUID from T_ANA_TIPI_PAGAMENTO where TPA_CODICE_ACN = :TPA_CODICE_ACN", Connection)

                cmd.Parameters.AddWithValue("TPA_CODICE_ACN", codiceACN)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj Is Nothing OrElse obj Is DBNull.Value Then
                        codice = Nothing
                    Else
                        codice = DirectCast(obj, Byte())
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return codice

        End Function

#End Region

#Region " Tipi Pagamento "

        ''' <summary>
        ''' Restituisce la lista dei tipi di pagamento
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetListTipiPagamento() As List(Of TipiPagamento) Implements INomiCommercialiProvider.GetListTipiPagamento

            Dim listTipiPagamento As List(Of TipiPagamento) = Nothing

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("select * from t_ana_tipi_pagamento", Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    listTipiPagamento = Me.GetListTipiPagamento(cmd)

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return listTipiPagamento

        End Function

        ''' <summary>
        ''' Restituisce l'oggetto contenente i dati del pagamento, dato il suo identificativo
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetTipoPagamento(guidPagamento As Byte()) As List(Of TipiPagamento) Implements INomiCommercialiProvider.GetTipoPagamento

            Dim listTipiPagamento As New List(Of TipiPagamento)
            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("select * from t_ana_tipi_pagamento where tpa_guid = :tpa_guid", Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("tpa_guid", guidPagamento)

                    listTipiPagamento = GetListTipiPagamento(cmd)

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return listTipiPagamento

        End Function

        ''' <summary>
        ''' Restituisce il contenuto del campo Condizioni Pagamento
        ''' </summary>
        ''' <param name="guidTipoPagamento"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetGestioneCondizioniPagamento(guidTipoPagamento As Byte()) As String Implements INomiCommercialiProvider.GetGestioneCondizioniPagamento

            Dim result As String = String.Empty

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("select tpa_condizioni_pagamento from t_ana_tipi_pagamento where tpa_guid = :tpa_guid", Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("tpa_guid", guidTipoPagamento)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        result = obj.ToString()
                    End If

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return result

        End Function


#Region "Operazioni Tipi Pagamento"

        Public Function GetTipiPagamento() As List(Of TipiPagamento) Implements INomiCommercialiProvider.GetTipiPagamento

            Dim listTipiPagamento As List(Of TipiPagamento) = Nothing

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("select TPA_GUID, TPA_DESCRIZIONE, TPA_CODICE_ESTERNO, TPA_CODICE_AVN from T_ANA_TIPI_PAGAMENTO", Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    listTipiPagamento = Me.GetTipiPagamento(cmd)

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return listTipiPagamento

        End Function

        Public Sub UpdateTipoPagamento(pagamento As TipiPagamento) Implements INomiCommercialiProvider.UpdateTipoPagamento

            Using cmd As New OracleClient.OracleCommand("update T_ANA_TIPI_PAGAMENTO set TPA_DESCRIZIONE = :descrizione, TPA_CODICE_ESTERNO = :codiceEsterno,
                                                         TPA_FLAG_IMPORTO = :flagImporto, TPA_FLAG_ESENZIONE = :flagEsenzione, TPA_AUTO_SET_IMPORTO = :autoSetImporto,
                                                         TPA_CONDIZIONI_PAGAMENTO = :condizioniPagamento, TPA_CODICE_AVN = :codiceAvn WHERE TPA_GUID = :guid", Connection)

                cmd.Parameters.AddWithValue("guid", pagamento.GuidPagamento.ToByteArray())
                cmd.Parameters.AddWithValue("descrizione", pagamento.Descrizione)
                cmd.Parameters.AddWithValue("codiceEsterno", pagamento.CodiceEsterno)

                If pagamento.FlagStatoCampoImporto = Enumerators.StatoAbilitazioneCampo.Disabilitato Then
                    cmd.Parameters.AddWithValue("flagImporto", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("flagImporto", GetIntParam(Convert.ToInt32(pagamento.FlagStatoCampoImporto)))
                End If

                If pagamento.FlagStatoCampoEsenzione = Enumerators.StatoAbilitazioneCampo.Disabilitato Then
                    cmd.Parameters.AddWithValue("flagEsenzione", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("flagEsenzione", GetIntParam(Convert.ToInt32(pagamento.FlagStatoCampoEsenzione)))
                End If

                cmd.Parameters.AddWithValue("autoSetImporto", pagamento.AutoSetImporto)
                cmd.Parameters.AddWithValue("condizioniPagamento", pagamento.HasCondizioniPagamento)
                cmd.Parameters.AddWithValue("codiceAvn", pagamento.CodiceAvn)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.ExecuteNonQuery()

                Finally
                    ConditionalCloseConnection(ownConnection)

                End Try

            End Using

        End Sub

        Public Sub DeleteTipoPagamento(guidPagamento As Byte()) Implements INomiCommercialiProvider.DeleteTipoPagamento

            Using cmd As New OracleClient.OracleCommand("delete from T_ANA_TIPI_PAGAMENTO
                                                         where TPA_GUID = :guidPagamento", Connection)

                cmd.Parameters.AddWithValue("guidPagamento", guidPagamento)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.ExecuteNonQuery()

                Finally
                    ConditionalCloseConnection(ownConnection)

                End Try

            End Using

        End Sub

        Public Sub AddTipoPagamento(pagamento As TipiPagamento) Implements INomiCommercialiProvider.AddTipoPagamento

            Using cmd As New OracleClient.OracleCommand("insert into T_ANA_TIPI_PAGAMENTO (TPA_DESCRIZIONE, TPA_CODICE_ESTERNO, TPA_FLAG_IMPORTO, TPA_FLAG_ESENZIONE, TPA_AUTO_SET_IMPORTO, TPA_CONDIZIONI_PAGAMENTO, TPA_CODICE_AVN)
                                                         values (:descrizione, :codiceEsterno, :flagImporto, :flagEsenzione, :autoSetImporto, :condizioniPagamento, :codiceAvn)", Connection)

                cmd.Parameters.AddWithValue("descrizione", GetStringParam(pagamento.Descrizione))
                cmd.Parameters.AddWithValue("codiceEsterno", GetStringParam(pagamento.CodiceEsterno))

                If pagamento.FlagStatoCampoImporto = Enumerators.StatoAbilitazioneCampo.Disabilitato Then
                    cmd.Parameters.AddWithValue("flagImporto", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("flagImporto", GetIntParam(Convert.ToInt32(pagamento.FlagStatoCampoImporto)))
                End If

                If pagamento.FlagStatoCampoEsenzione = Enumerators.StatoAbilitazioneCampo.Disabilitato Then
                    cmd.Parameters.AddWithValue("flagEsenzione", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("flagEsenzione", GetIntParam(Convert.ToInt32(pagamento.FlagStatoCampoEsenzione)))
                End If

                cmd.Parameters.AddWithValue("autoSetImporto", GetStringParam(pagamento.AutoSetImporto))
                cmd.Parameters.AddWithValue("condizioniPagamento", GetStringParam(pagamento.HasCondizioniPagamento))
                cmd.Parameters.AddWithValue("codiceAvn", GetStringParam(pagamento.CodiceAvn))

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.ExecuteNonQuery()

                Finally
                    ConditionalCloseConnection(ownConnection)

                End Try

            End Using

        End Sub

#End Region
#End Region

#Region " Condizioni Pagamento "

        ''' <summary>
        ''' Restituisce la lista delle condizioni di pagamento per il nome commerciale specificato
        ''' </summary>
        ''' <param name="codiceNomeCommerciale"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetListCondizioniPagamento(codiceNomeCommerciale As String) As List(Of CondizioniPagamento) Implements INomiCommercialiProvider.GetListCondizioniPagamento

            Dim listCondizioniPagamento As List(Of CondizioniPagamento) = Nothing

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("select * from t_noc_condizioni_pagamento where cpg_noc_codice = :cpg_noc_codice order by cpg_da_eta", Connection)

                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("cpg_noc_codice", codiceNomeCommerciale)

                    listCondizioniPagamento = GetListCondizioniPagamento(cmd)

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return listCondizioniPagamento

        End Function

        ''' <summary>
        ''' Restituisce la lista delle condizioni di pagamento in base al nome commerciale e all'età del paziente specificato
        ''' </summary>
        ''' <param name="codiceNomeCommerciale"></param>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        Public Function GetListCondizioniPagamento(codiceNomeCommerciale As String, codicePaziente As Long) As List(Of CondizioniPagamento) Implements INomiCommercialiProvider.GetListCondizioniPagamento

            Dim listCondizioniPagamento As List(Of CondizioniPagamento) = Nothing

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("select * from t_noc_condizioni_pagamento 
where cpg_noc_codice = :cpg_noc_codice
and cpg_da_eta <= (trunc(sysdate) - (select trunc(paz_data_nascita) from t_paz_pazienti where paz_codice = :paz_codice))
and (cpg_a_eta is null or cpg_a_eta > (trunc(sysdate) - (select trunc(paz_data_nascita) from t_paz_pazienti where paz_codice = :paz_codice)))
order by cpg_da_eta", Connection)

                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("cpg_noc_codice", codiceNomeCommerciale)
                    cmd.Parameters.AddWithValue("paz_codice", codicePaziente)

                    listCondizioniPagamento = GetListCondizioniPagamento(cmd)

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return listCondizioniPagamento

        End Function

        ''' <summary>
        ''' Restituisce i dati della condizione di pagamento specificata
        ''' </summary>
        ''' <param name="idCondizionePagamento"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCondizionePagamento(idCondizionePagamento As Integer) As CondizioniPagamento Implements INomiCommercialiProvider.GetCondizionePagamento

            Dim condizionePagamento As CondizioniPagamento = Nothing

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("select * from t_noc_condizioni_pagamento where cpg_id = :cpg_id", Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("cpg_id", idCondizionePagamento)

                    Dim listCondizioniPagamento As List(Of CondizioniPagamento) = Me.GetListCondizioniPagamento(cmd)
                    If Not listCondizioniPagamento Is Nothing AndAlso listCondizioniPagamento.Count > 0 Then
                        condizionePagamento = listCondizioniPagamento.First()
                    End If

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return condizionePagamento

        End Function

        ''' <summary>
        ''' Restituisce il valore della sequence
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function NextValSequenceCondizionePagamento() As Integer Implements INomiCommercialiProvider.NextValSequenceCondizionePagamento

            Dim sequenceValue As Integer = 0

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("select seq_cpg_id.nextval from dual", Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        sequenceValue = Convert.ToInt32(obj)
                    End If

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return sequenceValue

        End Function

        ''' <summary>
        ''' Inserimento della condizione di pagamento specificata
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertCondizionePagamento(condizionePagamento As CondizioniPagamento) As Integer Implements INomiCommercialiProvider.InsertCondizionePagamento

            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("insert into t_noc_condizioni_pagamento (cpg_id, cpg_da_eta, cpg_a_eta, cpg_flag_importo, cpg_flag_esenzione, cpg_noc_codice, cpg_auto_set_importo) values (:cpg_id, :cpg_da_eta, :cpg_a_eta, :cpg_flag_importo, :cpg_flag_esenzione, :cpg_noc_codice, :cpg_auto_set_importo)", Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("cpg_id", condizionePagamento.IdCondizione.Value)
                    cmd.Parameters.AddWithValue("cpg_da_eta", condizionePagamento.EtaInizio.Value)

                    If condizionePagamento.EtaFine.HasValue Then
                        cmd.Parameters.AddWithValue("cpg_a_eta", condizionePagamento.EtaFine.Value)
                    Else
                        cmd.Parameters.AddWithValue("cpg_a_eta", DBNull.Value)
                    End If

                    cmd.Parameters.AddWithValue("cpg_flag_importo", condizionePagamento.StatoAbilitazioneImporto)
                    cmd.Parameters.AddWithValue("cpg_flag_esenzione", condizionePagamento.StatoAbilitazioneEsenzione)
                    cmd.Parameters.AddWithValue("cpg_noc_codice", condizionePagamento.CodiceNomeCommerciale)
                    cmd.Parameters.AddWithValue("cpg_auto_set_importo", IIf(condizionePagamento.ImpostazioneAutomaticaImportoInEsecuzione, "S", "N"))

                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function

        ''' <summary>
        ''' Modifica i dati della condizione di pagamento specificata
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateCondizionePagamento(condizionePagamento As CondizioniPagamento) As Integer Implements INomiCommercialiProvider.UpdateCondizionePagamento

            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("update t_noc_condizioni_pagamento set cpg_da_eta = :cpg_da_eta, cpg_a_eta = :cpg_a_eta, cpg_flag_importo = :cpg_flag_importo, cpg_flag_esenzione = :cpg_flag_esenzione, cpg_auto_set_importo = :cpg_auto_set_importo where cpg_id = :cpg_id", Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("cpg_da_eta", condizionePagamento.EtaInizio.Value)

                    If condizionePagamento.EtaFine.HasValue Then
                        cmd.Parameters.AddWithValue("cpg_a_eta", condizionePagamento.EtaFine.Value)
                    Else
                        cmd.Parameters.AddWithValue("cpg_a_eta", DBNull.Value)
                    End If

                    cmd.Parameters.AddWithValue("cpg_flag_importo", condizionePagamento.StatoAbilitazioneImporto)
                    cmd.Parameters.AddWithValue("cpg_flag_esenzione", condizionePagamento.StatoAbilitazioneEsenzione)
                    cmd.Parameters.AddWithValue("cpg_id", condizionePagamento.IdCondizione)
                    cmd.Parameters.AddWithValue("cpg_auto_set_importo", IIf(condizionePagamento.ImpostazioneAutomaticaImportoInEsecuzione, "S", "N"))

                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function

        ''' <summary>
        ''' Cancellazione della condizione di pagamento specificata
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteCondizionePagamento(idCondizionePagamento As Integer) As Integer Implements INomiCommercialiProvider.DeleteCondizionePagamento

            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("delete from t_noc_condizioni_pagamento where cpg_id = :cpg_id", Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("cpg_id", idCondizionePagamento)

                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function

#End Region

#End Region

#Region " Private "

        Private Function GetListCondizioniPagamento(cmd As OracleClient.OracleCommand) As List(Of CondizioniPagamento)

            Dim listCondizioniPagamento As List(Of CondizioniPagamento) = Nothing

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    Dim cpg_id As Integer = idr.GetOrdinal("CPG_ID")
                    Dim cpg_da_eta As Integer = idr.GetOrdinal("CPG_DA_ETA")
                    Dim cpg_a_eta As Integer = idr.GetOrdinal("CPG_A_ETA")
                    Dim cpg_flag_importo As Integer = idr.GetOrdinal("CPG_FLAG_IMPORTO")
                    Dim cpg_flag_esenzione As Integer = idr.GetOrdinal("CPG_FLAG_ESENZIONE")
                    Dim cpg_noc_codice As Integer = idr.GetOrdinal("CPG_NOC_CODICE")
                    Dim cpg_auto_set_importo As Integer = idr.GetOrdinal("CPG_AUTO_SET_IMPORTO")

                    Dim condizionePagamento As CondizioniPagamento = Nothing
                    listCondizioniPagamento = New List(Of CondizioniPagamento)()

                    While idr.Read()

                        condizionePagamento = New CondizioniPagamento()
                        condizionePagamento.IdCondizione = idr.GetInt32(cpg_id)
                        condizionePagamento.EtaInizio = idr.GetInt32(cpg_da_eta)
                        condizionePagamento.EtaFine = idr.GetNullableInt32OrDefault(cpg_a_eta)
                        condizionePagamento.StatoAbilitazioneImporto = [Enum].Parse(GetType(Enumerators.StatoAbilitazioneCampo), idr.GetInt32OrDefault(cpg_flag_importo))
                        condizionePagamento.StatoAbilitazioneEsenzione = [Enum].Parse(GetType(Enumerators.StatoAbilitazioneCampo), idr.GetInt32OrDefault(cpg_flag_esenzione))
                        condizionePagamento.CodiceNomeCommerciale = idr.GetString(cpg_noc_codice)
                        condizionePagamento.ImpostazioneAutomaticaImportoInEsecuzione = idr.GetBooleanOrDefault(cpg_auto_set_importo)

                        listCondizioniPagamento.Add(condizionePagamento)

                    End While

                End If

            End Using

            Return listCondizioniPagamento

        End Function

        Private Function GetListTipiPagamento(cmd As OracleClient.OracleCommand) As List(Of TipiPagamento)

            Dim listTipiPagamento As List(Of TipiPagamento) = Nothing

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    listTipiPagamento = New List(Of TipiPagamento)()

                    Dim tipoPagamento As TipiPagamento = Nothing

                    Dim tpa_guid As Integer = idr.GetOrdinal("tpa_guid")
                    Dim tpa_descrizione As Integer = idr.GetOrdinal("tpa_descrizione")
                    Dim tpa_codice_esterno As Integer = idr.GetOrdinal("tpa_codice_esterno")
                    Dim tpa_flag_importo As Integer = idr.GetOrdinal("tpa_flag_importo")
                    Dim tpa_flag_esenzione As Integer = idr.GetOrdinal("tpa_flag_esenzione")
                    Dim tpa_auto_set_importo As Integer = idr.GetOrdinal("tpa_auto_set_importo")
                    Dim tpa_condizioni_pagamento As Integer = idr.GetOrdinal("tpa_condizioni_pagamento")
                    Dim tpa_codice_avn As Integer = idr.GetOrdinal("tpa_codice_avn")

                    While idr.Read()

                        tipoPagamento = New TipiPagamento()

                        Dim guidBytesLength As Long = idr.GetBytes(tpa_guid, 0, Nothing, 0, 0)
                        Dim guidBytes(guidBytesLength - 1) As Byte
                        idr.GetBytes(0, 0, guidBytes, 0, guidBytes.Length)

                        tipoPagamento.GuidPagamento = New Guid(guidBytes)
                        tipoPagamento.Descrizione = idr.GetStringOrDefault(tpa_descrizione)
                        tipoPagamento.CodiceEsterno = idr.GetStringOrDefault(tpa_codice_esterno)
                        tipoPagamento.FlagStatoCampoImporto = GetStatoAbilitazione(idr, tpa_flag_importo)
                        tipoPagamento.FlagStatoCampoEsenzione = GetStatoAbilitazione(idr, tpa_flag_esenzione)
                        tipoPagamento.AutoSetImporto = idr.GetStringOrDefault(tpa_auto_set_importo)
                        tipoPagamento.HasCondizioniPagamento = idr.GetStringOrDefault(tpa_condizioni_pagamento)
                        tipoPagamento.CodiceAvn = idr.GetStringOrDefault(tpa_codice_avn)

                        listTipiPagamento.Add(tipoPagamento)

                    End While
                End If

            End Using

            Return listTipiPagamento

        End Function

        Private Function GetTipiPagamento(cmd As OracleClient.OracleCommand) As List(Of TipiPagamento)

            Dim listTipiPagamento As List(Of TipiPagamento) = Nothing

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    listTipiPagamento = New List(Of TipiPagamento)()

                    Dim tipoPagamento As TipiPagamento = Nothing

                    Dim tpa_guid As Integer = idr.GetOrdinal("tpa_guid")
                    Dim tpa_descrizione As Integer = idr.GetOrdinal("tpa_descrizione")
                    Dim tpa_codice_esterno As Integer = idr.GetOrdinal("tpa_codice_esterno")
                    Dim tpa_codice_avn As Integer = idr.GetOrdinal("tpa_codice_avn")

                    While idr.Read()

                        tipoPagamento = New TipiPagamento()

                        ' Lettura guid
                        Dim guidBytesLength As Long = idr.GetBytes(tpa_guid, 0, Nothing, 0, 0)
                        Dim guidBytes(guidBytesLength - 1) As Byte
                        idr.GetBytes(0, 0, guidBytes, 0, guidBytes.Length)

                        tipoPagamento.GuidPagamento = New Guid(guidBytes)
                        tipoPagamento.Descrizione = idr.GetStringOrDefault(tpa_descrizione)
                        tipoPagamento.CodiceEsterno = idr.GetStringOrDefault(tpa_codice_esterno)
                        tipoPagamento.CodiceAvn = idr.GetStringOrDefault(tpa_codice_avn)

                        listTipiPagamento.Add(tipoPagamento)

                    End While
                End If

            End Using

            Return listTipiPagamento

        End Function

        Private Function GetStatoAbilitazione(idr As IDataReader, pos As Integer) As Enumerators.StatoAbilitazioneCampo?

            If idr.IsDBNull(pos) Then Return Nothing

            Return [Enum].Parse(GetType(Enumerators.StatoAbilitazioneCampo), idr.GetInt32(pos))

        End Function

#End Region

    End Class

End Namespace
