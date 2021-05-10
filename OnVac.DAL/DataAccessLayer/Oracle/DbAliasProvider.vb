Imports System.Collections.Generic
Imports System.Data.OracleClient
Imports System.Text.RegularExpressions
Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.Data
Imports Onit.OnAssistnet.Data.OracleClient

Namespace DAL

    Public Class DbAliasProvider
        Inherits DbProvider
        Implements IAliasProvider

#Region " Private members "

        Private IdUtente As Int64

#End Region

#Region " Properties "

        Private _message As String
        Public ReadOnly Property Message() As String
            Get
                Return _message
            End Get
        End Property

#End Region

#Region " Constructors "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

            Me.IdUtente = -1
            Me._message = String.Empty

        End Sub

        Public Sub New(ByRef DAM As IDAM, idUtente As Int64)

            MyBase.New(DAM)

            Me.IdUtente = idUtente
            Me._message = String.Empty

        End Sub

#End Region

#Region " Public methods "

#Region " Load Alias "

        ''' <summary>
        ''' Restituisce l'elenco dei pazienti trovati, in base ai filtri di ricerca
        ''' </summary>
        ''' <param name="filtriRicerca"></param>
        ''' <param name="pagingOptions"></param>
        ''' <returns></returns>
        Public Function LoadAlias(filtriRicerca As Filters.FiltriRicercaPaziente, pagingOptions As PagingOptions) As List(Of PazienteAlias) Implements IAliasProvider.LoadAlias

            Return LoadPazientiAlias(filtriRicerca, pagingOptions, False)

        End Function

        ''' <summary>
        ''' Restituisce l'elenco dei pazienti trovati, in base ai filtri di ricerca
        ''' </summary>
        ''' <param name="filtriRicerca"></param>
        ''' <param name="pagingOptions"></param>
        ''' <returns></returns>
        Public Function LoadAliasFromPazienti(filtriRicerca As Filters.FiltriRicercaPaziente, pagingOptions As PagingOptions) As List(Of PazienteAlias) Implements IAliasProvider.LoadAliasFromPazienti

            Return LoadPazientiAlias(filtriRicerca, pagingOptions, True)

        End Function

        ''' <summary>
        ''' Se fromPazienti è true, il caricamento avviene filtrando in base ai dati del master (sulla t_paz_pazienti)
        ''' </summary>
        ''' <param name="filtriRicerca"></param>
        ''' <param name="pagingOptions"></param>
        ''' <param name="fromPazienti"></param>
        ''' <returns></returns>
        Private Function LoadPazientiAlias(filtriRicerca As Filters.FiltriRicercaPaziente, pagingOptions As PagingOptions, fromPazienti As Boolean) As List(Of PazienteAlias)

            Dim listPazientiAlias As List(Of PazienteAlias)

            Using cmd As OracleCommand = Connection.CreateCommand()

                Dim query As String

                ' Impostazione filtri di ricerca
                Dim filtri As String = SetFiltriCaricamento(filtriRicerca, cmd)

                ' Lettura campi della tabella t_tmp_pazienti_alias
                ' La funzione restituisce l'elenco dei campi della tabella dell'alias. 
                ' La query di ricerca degli alias filtrando per il paziente utilizza i prefissi 'paz' (t_paz_pazienti)
                ' e 'alias' (t_tmp_pazienti_alias) per non generare ambiguità con i nomi delle colonne. 
                ' Perciò devo aggiungere i prefissi a tutti i campi, a seconda della query che utilizzo.

                Dim elencoCampi As String = GetElencoCampiTabellaAlias(False).ToLower()

                If fromPazienti Then

                    query = OnVac.Queries.Alias.OracleQueries.selPazientiAliasFromPazienti

                    ' Nella select devo utilizzare il prefisso alias
                    elencoCampi = Regex.Replace(elencoCampi, "^paz_", "alias.paz_", RegexOptions.IgnoreCase)
                    elencoCampi = Regex.Replace(elencoCampi, ",paz_", ",alias.paz_", RegexOptions.IgnoreCase)

                Else

                    query = OnVac.Queries.Alias.OracleQueries.selPazientiAlias

                    ' Nella select devo utilizzare il prefisso paz
                    elencoCampi = Regex.Replace(elencoCampi, "^paz_", "paz.paz_", RegexOptions.IgnoreCase)
                    elencoCampi = Regex.Replace(elencoCampi, ",paz_", ",paz.paz_", RegexOptions.IgnoreCase)

                End If

                cmd.CommandText = String.Format(query, elencoCampi, filtri)

                If Not pagingOptions Is Nothing Then

                    cmd.AddPaginatedQuery(pagingOptions)

                End If

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        listPazientiAlias = GetListPazientiAlias(idr, elencoCampi)

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listPazientiAlias

        End Function

#End Region

#Region " Count Alias "

        ''' <summary>
        ''' Restituisce il totale degli elementi da caricare
        ''' </summary>
        ''' <param name="filtriRicerca"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CountAliasToLoad(filtriRicerca As Filters.FiltriRicercaPaziente) As Integer Implements IAliasProvider.CountAliasToLoad

            Return CountPazientiAlias(filtriRicerca, False)

        End Function

        ''' <summary>
        ''' Restituisce il totale degli elementi da caricare, filtrati in base ai dati del paziente master
        ''' </summary>
        ''' <param name="filtriRicerca"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CountAliasToLoadFromPazienti(filtriRicerca As Filters.FiltriRicercaPaziente) As Integer Implements IAliasProvider.CountAliasToLoadFromPazienti

            Return CountPazientiAlias(filtriRicerca, True)

        End Function

        ''' <summary>
        ''' Se fromPazienti è true, il caricamento avviene filtrando in base ai dati del master (sulla t_paz_pazienti)
        ''' </summary>
        ''' <param name="filtriRicerca"></param>
        ''' <param name="fromPazienti"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CountPazientiAlias(filtriRicerca As Filters.FiltriRicercaPaziente, fromPazienti As Boolean) As Integer

            Dim count As Integer = 0

            Using cmd As OracleCommand = Connection.CreateCommand()

                ' Impostazione filtri di ricerca
                Dim filtri As String = SetFiltriCaricamento(filtriRicerca, cmd)

                If fromPazienti Then
                    cmd.CommandText = String.Format(OnVac.Queries.Alias.OracleQueries.cntPazientiAliasFromPazienti, filtri.ToString())
                Else
                    cmd.CommandText = String.Format(OnVac.Queries.Alias.OracleQueries.cntPazientiAlias, filtri.ToString())
                End If

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        count = Convert.ToInt32(obj)
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

#End Region

#Region " Delete Alias "

        ''' <summary>
        ''' Cancellazione dell'alias specificato dalla t_tmp_pazienti_alias
        ''' </summary>
        ''' <param name="codicePazienteAlias"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteAliasFromTemp(codicePazienteAlias As Integer) As Integer Implements IAliasProvider.DeleteAliasFromTemp

            Return DeleteAlias(codicePazienteAlias, True)

        End Function

        ''' <summary>
        ''' Cancellazione dell'alias specificato dalla t_paz_pazienti
        ''' </summary>
        ''' <param name="codicePazienteAlias"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteAliasFromPazienti(codicePazienteAlias As Integer) As Integer Implements IAliasProvider.DeleteAliasFromPazienti

            Return DeleteAlias(codicePazienteAlias, False)

        End Function

        ''' <summary>
        ''' Cancellazione dell'alias dalla tabella specificata
        ''' </summary>
        ''' <param name="codicePazienteAlias"></param>
        ''' <param name="fromTemp"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function DeleteAlias(codicePazienteAlias As Integer, fromTemp As Boolean) As Integer

            Dim count As Integer = 0

            Using cmd As OracleCommand = Connection.CreateCommand()

                If fromTemp Then
                    cmd.CommandText = OnVac.Queries.Alias.OracleQueries.delAliasFromTemp
                Else
                    cmd.CommandText = OnVac.Queries.Alias.OracleQueries.delAliasFromPazienti
                End If

                cmd.Parameters.AddWithValue("cod_paz", codicePazienteAlias)

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

#End Region

#Region " Load info alias in colonne alias "

        ''' <summary>
        ''' Restituisce una lista di elementi, definiti nella tabella t_ana_colonne_alias
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LoadColonneAlias() As List(Of ColonneAlias) Implements IAliasProvider.LoadColonneAlias

            Dim listColonneAlias As New List(Of ColonneAlias)()

            Using cmd As New OracleCommand(OnVac.Queries.Alias.OracleQueries.selColonneAlias, Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As OracleDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim colonneAlias As ColonneAlias = Nothing

                            Dim col_tabella As Integer = idr.GetOrdinal("col_tabella")
                            Dim col_colonna As Integer = idr.GetOrdinal("col_colonna")
                            Dim col_colonna_old As Integer = idr.GetOrdinal("col_colonna_old")
                            Dim col_indice As Integer = idr.GetOrdinal("col_indice")
                            Dim col_campi As Integer = idr.GetOrdinal("col_campi")
                            Dim col_ordine As Integer = idr.GetOrdinal("col_ordine")
                            Dim col_campo_ordinamento As Integer = idr.GetOrdinal("col_campo_ordinamento")

                            While idr.Read()

                                colonneAlias = New ColonneAlias()

                                colonneAlias.Tabella = idr.GetStringOrDefault(col_tabella)
                                colonneAlias.CampoCodicePaziente = idr.GetStringOrDefault(col_colonna)
                                colonneAlias.CampoCodicePazienteOld = idr.GetStringOrDefault(col_colonna_old)
                                colonneAlias.SetCampiIndice(idr.GetStringOrDefault(col_indice))
                                colonneAlias.SetCampi(idr.GetStringOrDefault(col_campi))
                                colonneAlias.Ordine = idr.GetInt32OrDefault(col_ordine)
                                colonneAlias.CampoOrdinamento = idr.GetStringOrDefault(col_campo_ordinamento)

                                listColonneAlias.Add(colonneAlias)

                            End While

                        End If
                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listColonneAlias

        End Function

#End Region

#Region " Copia dell'alias da anagrafe pazienti ad anagrafe pazienti alias "

        ''' <summary>
        ''' Creazione copia dell'alias nella t_tmp_pazienti_alias
        ''' </summary>
        ''' <param name="codicePazienteMaster"></param>
        ''' <param name="codicePazienteAlias"></param>
        ''' <param name="dataAlias"></param>
        ''' <param name="idUtente"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CloneAlias(codicePazienteMaster As Integer, codicePazienteAlias As Integer, dataAlias As DateTime, idUtente As Integer) As Integer Implements IAliasProvider.CloneAlias

            Dim count As Integer = 0

            Using cmd As OracleCommand = Connection.CreateCommand()

                Dim elencoCampi As String = GetElencoCampiTabellaAlias(True)

                ' Inserimento alias nella t_tmp_pazienti_alias
                cmd.CommandText = String.Format(Queries.Alias.OracleQueries.insAliasIntoTemp, elencoCampi, elencoCampi)

                cmd.Parameters.AddWithValue("codMaster", codicePazienteMaster)
                cmd.Parameters.AddWithValue("dataAlias", GetDateParam(dataAlias))
                cmd.Parameters.AddWithValue("idUtente", GetIntParam(idUtente))
                cmd.Parameters.AddWithValue("codAlias", GetIntParam(codicePazienteAlias))

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

#End Region

#Region " Merge dati vaccinali "

        ''' <summary>
        ''' Inserimento dati dell'alias in base al contenuto impostato nel record della t_ana_colonne_alias passato per parametro.
        ''' Nella tabella in cui si stanno inserendo i dati, il campo paz_codice viene valorizzato con il codice del master
        ''' e il campo paz_codice_old con il codice dell'alias.
        ''' </summary>
        ''' <param name="datiColonneAlias"></param>
        ''' <param name="codicePazienteMaster"></param>
        ''' <param name="codicePazienteAlias"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertDatiMergeAlias(datiColonneAlias As ColonneAlias, codicePazienteMaster As Integer, codicePazienteAlias As Integer) As Integer Implements IAliasProvider.InsertDatiMergeAlias

            Dim count As Integer = 0

            With _DAM.QB

                .NewQuery()

                Dim sql As String = CreateSubQueryExistsValoriAliasInMaster(datiColonneAlias.Tabella, datiColonneAlias.CampoCodicePaziente, codicePazienteMaster, datiColonneAlias.CampiIndice)

                .NewQuery(False, False)

                .AddSelectFields(codicePazienteMaster)      ' verrà inserito nel paz_codice

                For Each campo As String In datiColonneAlias.Campi
                    If campo <> datiColonneAlias.CampoCodicePaziente Then
                        .AddSelectFields(campo)
                    End If
                Next

                .AddSelectFields(codicePazienteAlias)       ' verrà inserito nel paz_codice_old

                .AddTables(datiColonneAlias.Tabella + " ALIAS")

                .AddWhereCondition("ALIAS." + datiColonneAlias.CampoCodicePaziente, Comparatori.Uguale, codicePazienteAlias, DataTypes.Numero)
                .OpenParanthesis()
                .AddWhereCondition("", Comparatori.NotExist, "(" + sql + ")", DataTypes.Replace)
                .CloseParanthesis()

                ' Ordinamento dei dati che verranno inseriti, per mantenere l'ordine originale, se serve.
                If Not String.IsNullOrWhiteSpace(datiColonneAlias.CampoOrdinamento) Then
                    .AddOrderByFields(datiColonneAlias.CampoOrdinamento)
                End If

                sql = .GetSelect()

                .NewQuery(False, False)

                .AddTables(datiColonneAlias.Tabella)
                .AddInsertField(datiColonneAlias.GetStringCampi() + "," + datiColonneAlias.CampoCodicePazienteOld, sql, DataTypes.Replace)

            End With

            count = _DAM.ExecNonQuery(ExecQueryType.Insert)

            Return count

        End Function

        ''' <summary>
        ''' Cancellazione dati dell'alias dalla tabella impostata nella t_ana_colonne_alias
        ''' </summary>
        ''' <param name="nomeTabella"></param>
        ''' <param name="nomeCampoCodicePaziente"></param>
        ''' <param name="codicePazienteAlias"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteDatiMergeAlias(nomeTabella As String, nomeCampoCodicePaziente As String, codicePazienteAlias As Integer) As Integer Implements IAliasProvider.DeleteDatiMergeAlias

            With _DAM.QB
                .NewQuery()
                .AddTables(nomeTabella)
                .AddWhereCondition(nomeCampoCodicePaziente, Comparatori.Uguale, codicePazienteAlias, DataTypes.Numero)
            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Delete)

        End Function

#End Region

#Region " Unmerge dati vaccinali "

        ''' <summary>
        ''' Inserimento dati vaccinali dell'alias in base al record della t_ana_colonne_alias passato per parametro.
        ''' Nella tabella in cui si stanno inserendo i dati, il campo paz_codice, in cui era presente il codice del master, 
        ''' viene valorizzato con il codice dell'alias e il campo paz_codice_old (valorizzato con il codice dell'alias) 
        ''' viene cancellato.
        ''' </summary>
        ''' <param name="datiColonneAlias"></param>
        ''' <param name="codicePazienteMaster"></param>
        ''' <param name="codicePazienteAlias"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertDatiUnmergeAlias(datiColonneAlias As ColonneAlias, codicePazienteMaster As Integer, codicePazienteAlias As Integer) As Integer Implements IAliasProvider.InsertDatiUnmergeAlias

            ' Struttura query:

            ' insert into {0}
            ' ({1},{2},{3})
            ' select :codAlias, {2}, null 
            ' from {0} Alias 
            ' where Alias.{1} = :codMaster 
            ' and Alias.{3} = :codAlias 
            ' and not exists 
            ' (
            '    select 1 from {0} Master 
            '    where {1} = :codAlias
            '    {4}
            ' )
            ' {5}   [ => order by datiColonneAlias.CampoOrdinamento ]
            '
            '/*
            ' Dati della t_ana_colonne_alias:
            '   0: tabella
            '   1: campoCodice
            '   2: campi (escluso codice)
            '   3: campoCodiceOld
            '   4: filtri sugli indici (escluso codice) [ --> and Master.<campoIndice> = Alias.<campoIndice> ]
            '   5: campo ordinamento dati da inserire
            '*/

            Dim count As Integer = 0

            Using cmd As OracleCommand = Connection.CreateCommand()

                ' Costruzione elenco campi da inserire, escluso codice paziente
                Dim campi As String() = (From campo As String In datiColonneAlias.Campi
                                         Where campo <> datiColonneAlias.CampoCodicePaziente
                                         Select campo).ToArray()

                Dim elencoCampi As String = String.Join(",", campi)

                ' Costruzione filtri sui campi indice escluso il codice paziente
                Dim filtri As New Text.StringBuilder()

                For Each campoIndice As String In datiColonneAlias.CampiIndice
                    If campoIndice <> datiColonneAlias.CampoCodicePaziente Then
                        filtri.AppendFormat(" and Master.{0} = Alias.{0} ", campoIndice)
                    End If
                Next

                Dim orderBy As String = String.Empty

                If Not String.IsNullOrWhiteSpace(datiColonneAlias.CampoOrdinamento) Then
                    orderBy = String.Format(" order by {0}", datiColonneAlias.CampoOrdinamento)
                End If

                ' Inserimento dati vaccinali alias
                cmd.CommandText = String.Format(Queries.Alias.OracleQueries.insDatiUnmergeAlias,
                                                datiColonneAlias.Tabella,
                                                datiColonneAlias.CampoCodicePaziente,
                                                elencoCampi,
                                                datiColonneAlias.CampoCodicePazienteOld,
                                                filtri.ToString(),
                                                orderBy)

                cmd.Parameters.AddWithValue("codMaster", codicePazienteMaster)
                cmd.Parameters.AddWithValue("codAlias", codicePazienteAlias)

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
        ''' Cancellazione dati vaccinali del master relativi all'alias dalla tabella impostata nella t_ana_colonne_alias
        ''' </summary>
        ''' <param name="nomeTabella"></param>
        ''' <param name="nomeCampoCodicePaziente"></param>
        ''' <param name="nomeCampoCodicePazienteOld"></param>
        ''' <param name="codicePazienteMaster"></param>
        ''' <param name="codicePazienteAlias"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteDatiUnmergeAlias(nomeTabella As String, nomeCampoCodicePaziente As String, nomeCampoCodicePazienteOld As String, codicePazienteMaster As Integer, codicePazienteAlias As Integer) As Integer Implements IAliasProvider.DeleteDatiUnmergeAlias

            Dim count As Integer = 0

            ' delete from <tabella>
            ' where <cod paziente> = :codMaster
            ' and <cod paziente old> = :codAlias

            Using cmd As OracleCommand = Connection.CreateCommand()

                ' Cancellazione dati vaccinali master relativi all'alias
                cmd.CommandText = String.Format(Queries.Alias.OracleQueries.delDatiUnmergeAlias,
                                                nomeTabella,
                                                nomeCampoCodicePaziente,
                                                nomeCampoCodicePazienteOld)

                cmd.Parameters.AddWithValue("codMaster", codicePazienteMaster)
                cmd.Parameters.AddWithValue("codAlias", codicePazienteAlias)

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

#End Region

#Region " Info Merge "

        ''' <summary>
        ''' Restituisce una lista di elementi contenenti le informazioni per effettuare il merge, 
        ''' definiti nelle tabelle t_ana_tabelle_merge e t_ana_link_tabelle_merge
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LoadMergeInfo() As List(Of MergeInfo) Implements IAliasProvider.LoadMergeInfo

            Dim listMergeInfo As New List(Of MergeInfo)()

            Using cmd As New OracleCommand(OnVac.Queries.Alias.OracleQueries.selMergeInfo, Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As OracleDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim mergeInfo As MergeInfo = Nothing

                            Dim nome_tabella_padre As Integer = idr.GetOrdinal("nome_tabella_padre")
                            Dim campi_indice_padre As Integer = idr.GetOrdinal("campi_indice_padre")
                            Dim campo_fk_padre As Integer = idr.GetOrdinal("campo_fk_padre")
                            Dim campi_select_padre As Integer = idr.GetOrdinal("campi_select_padre")
                            Dim campo_cod_paziente_padre As Integer = idr.GetOrdinal("campo_cod_paziente_padre")
                            Dim campo_cod_paziente_old_padre As Integer = idr.GetOrdinal("campo_cod_paziente_old_padre")
                            Dim ordine_padre As Integer = idr.GetOrdinal("ordine_padre")
                            Dim nome_tabella_figlia As Integer = idr.GetOrdinal("nome_tabella_figlia")
                            Dim campi_indice_figlia As Integer = idr.GetOrdinal("campi_indice_figlia")
                            Dim campo_fk_figlia As Integer = idr.GetOrdinal("campo_fk_figlia")
                            Dim campi_select_figlia As Integer = idr.GetOrdinal("campi_select_figlia")
                            Dim campo_cod_paziente_figlia As Integer = idr.GetOrdinal("campo_cod_paziente_figlia")
                            Dim campo_cod_paziente_old_figlia As Integer = idr.GetOrdinal("campo_cod_paziente_old_figlia")
                            Dim ordine_figlia As Integer = idr.GetOrdinal("ordine_figlia")
                            Dim nome_sequence As Integer = idr.GetOrdinal("nome_sequence")

                            While idr.Read()

                                mergeInfo = New MergeInfo()

                                mergeInfo.NomeTabellaPadre = idr.GetStringOrDefault(nome_tabella_padre)
                                mergeInfo.CampiIndicePadre = GetListCampi(idr.GetStringOrDefault(campi_indice_padre))
                                mergeInfo.CampoFkPadre = idr.GetStringOrDefault(campo_fk_padre)
                                mergeInfo.CampiSelectPadre = GetListCampi(idr.GetStringOrDefault(campi_select_padre))
                                mergeInfo.CampoCodicePazientePadre = idr.GetStringOrDefault(campo_cod_paziente_padre)
                                mergeInfo.CampoCodicePazienteOldPadre = idr.GetStringOrDefault(campo_cod_paziente_old_padre)
                                mergeInfo.OrdinePadre = idr.GetInt32OrDefault(ordine_padre)

                                mergeInfo.NomeTabellaFiglia = idr.GetStringOrDefault(nome_tabella_figlia)
                                mergeInfo.CampiIndiceFiglia = GetListCampi(idr.GetStringOrDefault(campi_indice_figlia))
                                mergeInfo.CampoFkFiglia = idr.GetStringOrDefault(campo_fk_figlia)
                                mergeInfo.CampiSelectFiglia = GetListCampi(idr.GetStringOrDefault(campi_select_figlia))
                                mergeInfo.CampoCodicePazienteFiglia = idr.GetStringOrDefault(campo_cod_paziente_figlia)
                                mergeInfo.CampoCodicePazienteOldFiglia = idr.GetStringOrDefault(campo_cod_paziente_old_figlia)
                                mergeInfo.OrdineFiglia = idr.GetInt32OrDefault(ordine_figlia)

                                mergeInfo.NomeSequence = idr.GetStringOrDefault(nome_sequence)

                                listMergeInfo.Add(mergeInfo)

                            End While

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listMergeInfo

        End Function

        ''' <summary>
        ''' Selezione dei dati dell'alias da inserire nel master, in base al contenuto impostato nella struttura mergeInfo.
        ''' Non vengono selezionati valori già presenti, controllati in base ai campi specificati tra i campi indice.
        ''' </summary>
        ''' <param name="mergeInfo"></param>
        ''' <param name="codicePazienteMaster"></param>
        ''' <param name="codicePazienteAlias"></param>
        ''' <param name="unmergeOperation"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDtValoriAlias(mergeInfo As MergeInfo, codicePazienteMaster As Integer, codicePazienteAlias As Integer, unmergeOperation As Boolean) As DataTable Implements IAliasProvider.GetDtValoriAlias

            Dim dtValoriMerge As New DataTable()

            With _DAM.QB

                ' Sottoquery per escludere dal risultato i valori dell'alias già presenti nel master
                .NewQuery()

                Dim queryExistsValoriAliasInMaster As String

                If unmergeOperation Then
                    queryExistsValoriAliasInMaster =
                        CreateSubQueryExistsValoriAliasInMaster(mergeInfo.NomeTabellaPadre, mergeInfo.CampoCodicePazientePadre, codicePazienteAlias, mergeInfo.CampiIndicePadre)
                Else
                    queryExistsValoriAliasInMaster =
                        CreateSubQueryExistsValoriAliasInMaster(mergeInfo.NomeTabellaPadre, mergeInfo.CampoCodicePazientePadre, codicePazienteMaster, mergeInfo.CampiIndicePadre)
                End If

                ' Query di select dei valori che verranno inseriti (paz_codice e paz_codice old vengono gestiti a parte)
                .NewQuery(False, False)

                .AddSelectFields(mergeInfo.CampoFkPadre) ' vecchio valore della sequence

                ' Campi da duplicare (da alias a master), esclusi paz_codice e paz_codice_old che verranno gestiti a parte
                Dim campiSelect As String() = (From campo As String In mergeInfo.CampiSelectPadre
                                               Where campo <> mergeInfo.CampoCodicePazientePadre And
                                                     campo <> mergeInfo.CampoCodicePazienteOldPadre And
                                                     campo <> mergeInfo.CampoFkPadre
                                               Select campo).ToArray()

                .AddSelectFields(String.Join(",", campiSelect))

                .AddTables(mergeInfo.NomeTabellaPadre + " ALIAS")

                If unmergeOperation Then
                    .AddWhereCondition("ALIAS." + mergeInfo.CampoCodicePazientePadre, Comparatori.Uguale, codicePazienteMaster, DataTypes.Numero)
                    .AddWhereCondition("ALIAS." + mergeInfo.CampoCodicePazienteOldPadre, Comparatori.Uguale, codicePazienteAlias, DataTypes.Numero)
                Else
                    .AddWhereCondition("ALIAS." + mergeInfo.CampoCodicePazientePadre, Comparatori.Uguale, codicePazienteAlias, DataTypes.Numero)
                End If

                .AddWhereCondition("", Comparatori.NotExist, "(" + queryExistsValoriAliasInMaster + ")", DataTypes.Replace)

            End With

            _DAM.BuildDataTable(dtValoriMerge)

            Return dtValoriMerge

        End Function

        Public Function GetNextSequenceValue(sequenceName As String) As Integer Implements IAliasProvider.GetNextSequenceValue

            Dim nextVal As Integer = 0

            Using cmd As New OracleCommand(String.Format(OnVac.Queries.Alias.OracleQueries.selNextSequenceValue, sequenceName), Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        nextVal = Convert.ToInt32(obj)
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return nextVal

        End Function

        Public Function InsertValoreAliasPadre(mergeInfo As MergeInfo, codicePazienteMaster As Integer, codicePazienteAlias As Integer, sequenceId As Integer, rowDatiAlias As DataRow, unmergeOperation As Boolean) As Integer Implements IAliasProvider.InsertValoreAliasPadre

            Dim count As Integer = 0

            Using cmd As New OracleCommand()

                cmd.Connection = Connection

                Dim commandText As String = "insert into {0} ({1}) values ({2})"

                Dim nomeCampi As New System.Text.StringBuilder()
                Dim nomeParametri As New System.Text.StringBuilder()

                If unmergeOperation Then
                    AddToCommand(mergeInfo.CampoCodicePazientePadre, codicePazienteAlias, nomeCampi, nomeParametri, cmd)
                    AddToCommand(mergeInfo.CampoCodicePazienteOldPadre, Nothing, nomeCampi, nomeParametri, cmd)
                Else
                    AddToCommand(mergeInfo.CampoCodicePazientePadre, codicePazienteMaster, nomeCampi, nomeParametri, cmd)
                    AddToCommand(mergeInfo.CampoCodicePazienteOldPadre, codicePazienteAlias, nomeCampi, nomeParametri, cmd)
                End If

                AddToCommand(mergeInfo.CampoFkPadre, sequenceId, nomeCampi, nomeParametri, cmd)

                ' Parto da 1 perchè il campo 0 è la vecchia sequence, che non devo inserire
                For i As Int16 = 1 To rowDatiAlias.Table.Columns.Count - 1
                    AddToCommand(rowDatiAlias.Table.Columns(i).ColumnName, rowDatiAlias(i), nomeCampi, nomeParametri, cmd)
                Next

                If nomeCampi.Length > 0 Then nomeCampi.Remove(nomeCampi.Length - 2, 2)
                If nomeParametri.Length > 0 Then nomeParametri.Remove(nomeParametri.Length - 2, 2)

                cmd.CommandText = String.Format(commandText, mergeInfo.NomeTabellaPadre, nomeCampi.ToString(), nomeParametri.ToString())

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

        Public Function InsertValoriAliasFiglia(mergeInfo As MergeInfo, codicePazienteMaster As Integer, codicePazienteAlias As Integer, oldSequenceId As Integer, newSequenceId As Integer, unmergeOperation As Boolean) As Integer Implements IAliasProvider.InsertValoriAliasFiglia

            Dim count As Integer = 0

            With _DAM.QB

                .NewQuery()
                Dim queryExistsValoriAliasInMaster As String

                If unmergeOperation Then
                    queryExistsValoriAliasInMaster =
                        CreateSubQueryExistsValoriAliasInMaster(mergeInfo.NomeTabellaFiglia, mergeInfo.CampoCodicePazienteFiglia, codicePazienteAlias, mergeInfo.CampiIndiceFiglia)
                Else
                    queryExistsValoriAliasInMaster =
                        CreateSubQueryExistsValoriAliasInMaster(mergeInfo.NomeTabellaFiglia, mergeInfo.CampoCodicePazienteFiglia, codicePazienteMaster, mergeInfo.CampiIndiceFiglia)
                End If

                .NewQuery(False, False)

                .AddTables(mergeInfo.NomeTabellaFiglia + " ALIAS")

                If unmergeOperation Then
                    .AddSelectFields(codicePazienteAlias, "NULL")
                Else
                    .AddSelectFields(codicePazienteMaster, codicePazienteAlias)
                End If

                .AddSelectFields(newSequenceId)

                Dim campiSelect As String() = (From campo As String In mergeInfo.CampiSelectFiglia
                                               Where campo <> mergeInfo.CampoCodicePazienteFiglia And
                                                     campo <> mergeInfo.CampoCodicePazienteOldFiglia And
                                                     campo <> mergeInfo.CampoFkFiglia
                                               Select campo).ToArray()

                Dim campiSelectFiglia As String = String.Join(",", campiSelect)

                .AddSelectFields(campiSelectFiglia)

                If unmergeOperation Then
                    .AddWhereCondition("ALIAS." + mergeInfo.CampoCodicePazienteFiglia, Comparatori.Uguale, codicePazienteMaster, DataTypes.Numero)
                    .AddWhereCondition("ALIAS." + mergeInfo.CampoCodicePazienteOldFiglia, Comparatori.Uguale, codicePazienteAlias, DataTypes.Numero)
                Else
                    .AddWhereCondition("ALIAS." + mergeInfo.CampoCodicePazienteFiglia, Comparatori.Uguale, codicePazienteAlias, DataTypes.Numero)
                End If
                .AddWhereCondition("ALIAS." + mergeInfo.CampoFkFiglia, Comparatori.Uguale, oldSequenceId, DataTypes.Numero)
                .AddWhereCondition("", Comparatori.NotExist, "(" + queryExistsValoriAliasInMaster + ")", DataTypes.Replace)

                Dim querySelect As String = .GetSelect()

                .NewQuery(False, False)

                Dim nomiCampiInsert As String = String.Format("{0},{1},{2},{3}", mergeInfo.CampoCodicePazienteFiglia,
                                                                                 mergeInfo.CampoCodicePazienteOldFiglia,
                                                                                 mergeInfo.CampoFkFiglia,
                                                                                 campiSelectFiglia)
                .AddTables(mergeInfo.NomeTabellaFiglia)
                .AddInsertField(nomiCampiInsert, querySelect, DataTypes.Replace)

            End With

            count = _DAM.ExecNonQuery(ExecQueryType.Insert)

            Return count

        End Function

#End Region

#Region " Info Merge Update "

        Public Function LoadMergeUpdateInfo() As List(Of MergeUpdateInfo) Implements IAliasProvider.LoadMergeUpdateInfo

            Dim list As New List(Of MergeUpdateInfo)()

            Using cmd As New OracleCommand(Queries.Alias.OracleQueries.selMergeUpdateInfo, Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As OracleDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim tmu_nome_tabella As Integer = idr.GetOrdinal("TMU_NOME_TABELLA")
                            Dim tmu_campo_cod_paziente As Integer = idr.GetOrdinal("TMU_CAMPO_COD_PAZIENTE")
                            Dim tmu_campo_cod_paziente_old As Integer = idr.GetOrdinal("TMU_CAMPO_COD_PAZIENTE_OLD")
                            Dim tmu_ordine As Integer = idr.GetOrdinal("TMU_ORDINE")

                            While idr.Read()

                                Dim item As New MergeUpdateInfo()

                                item.NomeTabella = idr.GetStringOrDefault(tmu_nome_tabella)
                                item.CampoCodicePaziente = idr.GetStringOrDefault(tmu_campo_cod_paziente)
                                item.CampoCodicePazienteOld = idr.GetStringOrDefault(tmu_campo_cod_paziente_old)
                                item.Ordine = idr.GetInt32OrDefault(tmu_ordine)

                                list.Add(item)

                            End While

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list

        End Function

        Public Function UpdateTabellaMerge(mergeUpdateInfo As MergeUpdateInfo, codicePazienteMaster As Long, codicePazienteAlias As Long) As Integer Implements IAliasProvider.UpdateTabellaMerge

            Dim count As Integer = 0

            Using cmd As New OracleCommand()

                cmd.Connection = Connection

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.CommandText = String.Format("update {0} set {1} = {2}, {2} = :paz_codice_master where {2} = :paz_codice_alias",
                                      mergeUpdateInfo.NomeTabella, mergeUpdateInfo.CampoCodicePazienteOld, mergeUpdateInfo.CampoCodicePaziente)

                    cmd.Parameters.AddWithValue("paz_codice_master", codicePazienteMaster)
                    cmd.Parameters.AddWithValue("paz_codice_alias", codicePazienteAlias)

                    Dim obj As Object = cmd.ExecuteNonQuery()

                    If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                        count = Convert.ToInt32(obj)
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

#End Region

#End Region

#Region " Private methods "

        ''' <summary>
        ''' Restituisce una stringa creata in base a tutti i filtri specificati e aggiunge i parametri al command
        ''' </summary>
        ''' <param name="filtriRicerca"></param>
        ''' <param name="cmd"></param>
        ''' <returns></returns>
        Private Function SetFiltriCaricamento(filtriRicerca As Filters.FiltriRicercaPaziente, cmd As OracleCommand) As String

            If filtriRicerca Is Nothing Then Return String.Empty

            Dim filtri As New Text.StringBuilder()

            filtri.Append("where 1=1 ")

            If filtriRicerca.Codice > 0 Then
                filtri.AppendFormat(" and paz.paz_codice = :pazCodice ")
                cmd.Parameters.AddWithValue("pazCodice", filtriRicerca.Codice)
            End If

            If Not String.IsNullOrEmpty(filtriRicerca.CodiceAusiliario) Then
                filtri.AppendFormat(" and paz.paz_codice_ausiliario = :pazCodiceAux ")
                cmd.Parameters.AddWithValue("pazCodiceAux", filtriRicerca.CodiceAusiliario)
            End If

            If Not String.IsNullOrEmpty(filtriRicerca.Cognome) Then
                filtri.AppendFormat(" and paz.paz_cognome like :pazCognome ")
                cmd.Parameters.AddWithValue("pazCognome", filtriRicerca.Cognome + "%")
            End If

            If Not String.IsNullOrEmpty(filtriRicerca.Nome) Then
                filtri.AppendFormat(" and paz.paz_nome like :pazNome ")
                cmd.Parameters.AddWithValue("pazNome", filtriRicerca.Nome + "%")
            End If

            If filtriRicerca.DataNascita_Da <> Date.MinValue AndAlso filtriRicerca.DataNascita_Da <> Date.MaxValue Then
                filtri.AppendFormat(" and paz.paz_data_nascita >= :pazDataNascitaDa ")
                cmd.Parameters.AddWithValue("pazDataNascitaDa", filtriRicerca.DataNascita_Da)
            End If

            If filtriRicerca.DataNascita_A <> Date.MinValue AndAlso filtriRicerca.DataNascita_A <> Date.MaxValue Then
                filtri.AppendFormat(" and paz.paz_data_nascita <= :pazDataNascitaA ")
                cmd.Parameters.AddWithValue("pazDataNascitaA", filtriRicerca.DataNascita_A)
            End If

            If Not String.IsNullOrEmpty(filtriRicerca.Sesso) Then
                filtri.AppendFormat(" and paz.paz_sesso = :pazSesso ")
                cmd.Parameters.AddWithValue("pazSesso", filtriRicerca.Sesso)
            End If

            If Not String.IsNullOrEmpty(filtriRicerca.Consultorio) Then
                filtri.AppendFormat(" and paz.paz_cns_codice = :pazCns ")
                cmd.Parameters.AddWithValue("pazCns", filtriRicerca.Consultorio)
            End If

            If Not String.IsNullOrEmpty(filtriRicerca.CodiceComuneNascita) Then
                filtri.AppendFormat(" and paz.paz_com_codice_nascita = :pazComNas ")
                cmd.Parameters.AddWithValue("pazComNas", filtriRicerca.CodiceComuneNascita)
            End If

            If Not String.IsNullOrEmpty(filtriRicerca.CodiceFiscale) Then
                filtri.AppendFormat(" and paz.paz_codice_fiscale = :pazCodFisc ")
                cmd.Parameters.AddWithValue("pazCodFisc", filtriRicerca.CodiceFiscale)
            End If

            Return filtri.ToString()

        End Function

        ''' <summary>
        ''' Restituisce una stringa contenente i campi della t_tmp_pazienti_alias, letti dal catalogo di oracle
        ''' Se il parametro vale true, devono essere esclusi i campi paz_codice_master, paz_data_alias, paz_ute_id
        ''' </summary>
        Private Function GetElencoCampiTabellaAlias(soloDatiAnagrafici As Boolean) As String

            Dim listAliasColumns As New List(Of String)()

            Using cmd As OracleCommand = Connection.CreateCommand()

                If soloDatiAnagrafici Then
                    cmd.CommandText = Queries.Alias.OracleQueries.selColumnsDatiAliasFromCatalog
                Else
                    cmd.CommandText = Queries.Alias.OracleQueries.selAllColumnsFromCatalog
                End If

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then
                            Dim column_name As Integer = idr.GetOrdinal("column_name")

                            While idr.Read()

                                listAliasColumns.Add(idr.GetString(column_name))

                            End While
                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return String.Join(",", listAliasColumns.ToArray())

        End Function

        Private Function GetListPazientiAlias(dr As IDataReader, elencoCampi As String) As List(Of PazienteAlias)

            Dim listAlias As New List(Of PazienteAlias)()

            If Not dr Is Nothing Then

                Dim paz_codice As Integer = dr.GetOrdinalOrDefault("paz_codice", elencoCampi)

                Dim paz_cognome As Integer = dr.GetOrdinalOrDefault("paz_cognome", elencoCampi)
                Dim paz_nome As Integer = dr.GetOrdinalOrDefault("paz_nome", elencoCampi)
                Dim paz_data_nascita As Integer = dr.GetOrdinalOrDefault("paz_data_nascita", elencoCampi)
                Dim paz_codice_fiscale As Integer = dr.GetOrdinalOrDefault("paz_codice_fiscale", elencoCampi)
                Dim paz_sesso As Integer = dr.GetOrdinalOrDefault("paz_sesso", elencoCampi)

                Dim paz_indirizzo_residenza As Integer = dr.GetOrdinalOrDefault("paz_indirizzo_residenza", elencoCampi)
                Dim paz_com_codice_residenza As Integer = dr.GetOrdinalOrDefault("paz_com_codice_residenza", elencoCampi)
                Dim res_descrizione As Integer = dr.GetOrdinal("res_descrizione")
                Dim res_prv As Integer = dr.GetOrdinal("res_prv")
                Dim res_cap As Integer = dr.GetOrdinal("res_cap")

                Dim paz_indirizzo_domicilio As Integer = dr.GetOrdinalOrDefault("paz_indirizzo_domicilio", elencoCampi)
                Dim paz_com_codice_domicilio As Integer = dr.GetOrdinalOrDefault("paz_com_codice_domicilio", elencoCampi)
                Dim dom_descrizione As Integer = dr.GetOrdinal("dom_descrizione")
                Dim dom_prv As Integer = dr.GetOrdinal("dom_prv")
                Dim dom_cap As Integer = dr.GetOrdinal("dom_cap")

                Dim paz_med_codice_base As Integer = dr.GetOrdinalOrDefault("paz_med_codice_base", elencoCampi)
                Dim med_descrizione As Integer = dr.GetOrdinal("med_descrizione")
                Dim paz_cns_codice As Integer = dr.GetOrdinalOrDefault("paz_cns_codice", elencoCampi)

                Dim paz_flag_cessato As Integer = dr.GetOrdinalOrDefault("paz_flag_cessato", elencoCampi)
                Dim paz_aire As Integer = dr.GetOrdinalOrDefault("paz_aire", elencoCampi)
                Dim paz_cir_codice As Integer = dr.GetOrdinalOrDefault("paz_cir_codice", elencoCampi)
                Dim paz_cit_codice As Integer = dr.GetOrdinalOrDefault("paz_cit_codice", elencoCampi)
                Dim paz_cns_codice_old As Integer = dr.GetOrdinalOrDefault("paz_cns_codice_old", elencoCampi)
                Dim paz_cns_data_assegnazione As Integer = dr.GetOrdinalOrDefault("paz_cns_data_assegnazione", elencoCampi)
                Dim paz_cns_terr_codice As Integer = dr.GetOrdinalOrDefault("paz_cns_terr_codice", elencoCampi)
                Dim paz_codice_ausiliario As Integer = dr.GetOrdinalOrDefault("paz_codice_ausiliario", elencoCampi)
                Dim paz_codice_regionale As Integer = dr.GetOrdinalOrDefault("paz_codice_regionale", elencoCampi)
                Dim paz_com_codice_nascita As Integer = dr.GetOrdinalOrDefault("paz_com_codice_nascita", elencoCampi)
                Dim paz_com_comune_emigrazione As Integer = dr.GetOrdinalOrDefault("paz_com_comune_emigrazione", elencoCampi)
                Dim paz_com_comune_provenienza As Integer = dr.GetOrdinalOrDefault("paz_com_comune_provenienza", elencoCampi)
                Dim paz_data_agg_da_anag As Integer = dr.GetOrdinalOrDefault("paz_data_agg_da_anag", elencoCampi)
                Dim paz_data_aire As Integer = dr.GetOrdinalOrDefault("paz_data_aire", elencoCampi)
                Dim paz_data_decesso As Integer = dr.GetOrdinalOrDefault("paz_data_decesso", elencoCampi)
                Dim paz_data_decorrenza_med As Integer = dr.GetOrdinalOrDefault("paz_data_decorrenza_med", elencoCampi)
                Dim paz_data_emigrazione As Integer = dr.GetOrdinalOrDefault("paz_data_emigrazione", elencoCampi)
                Dim paz_data_immigrazione As Integer = dr.GetOrdinalOrDefault("paz_data_immigrazione", elencoCampi)
                Dim paz_data_inserimento As Integer = dr.GetOrdinalOrDefault("paz_data_inserimento", elencoCampi)
                Dim paz_data_irreperibilita As Integer = dr.GetOrdinalOrDefault("paz_data_irreperibilita", elencoCampi)
                Dim paz_data_revoca_med As Integer = dr.GetOrdinalOrDefault("paz_data_revoca_med", elencoCampi)
                Dim paz_data_scadenza_med As Integer = dr.GetOrdinalOrDefault("paz_data_scadenza_med", elencoCampi)
                Dim paz_dis_codice As Integer = dr.GetOrdinalOrDefault("paz_dis_codice", elencoCampi)
                Dim paz_irreperibile As Integer = dr.GetOrdinalOrDefault("paz_irreperibile", elencoCampi)
                Dim paz_locale As Integer = dr.GetOrdinalOrDefault("paz_locale", elencoCampi)
                Dim paz_regolarizzato As Integer = dr.GetOrdinalOrDefault("paz_regolarizzato", elencoCampi)
                Dim paz_stato As Integer = dr.GetOrdinalOrDefault("paz_stato", elencoCampi)
                Dim paz_stato_anagrafico As Integer = dr.GetOrdinalOrDefault("paz_stato_anagrafico", elencoCampi)
                Dim paz_telefono_1 As Integer = dr.GetOrdinalOrDefault("paz_telefono_1", elencoCampi)
                Dim paz_telefono_2 As Integer = dr.GetOrdinalOrDefault("paz_telefono_2", elencoCampi)
                Dim paz_telefono_3 As Integer = dr.GetOrdinalOrDefault("paz_telefono_3", elencoCampi)
                Dim paz_email As Integer = dr.GetOrdinalOrDefault("paz_email", elencoCampi)
                Dim paz_tessera As Integer = dr.GetOrdinalOrDefault("paz_tessera", elencoCampi)
                Dim paz_usl_codice_assistenza As Integer = dr.GetOrdinalOrDefault("paz_usl_codice_assistenza", elencoCampi)
                Dim paz_usl_codice_residenza As Integer = dr.GetOrdinalOrDefault("paz_usl_codice_residenza", elencoCampi)
                Dim paz_anonimo As Integer = dr.GetOrdinalOrDefault("paz_anonimo", elencoCampi)
                Dim paz_azi_codice As Integer = dr.GetOrdinalOrDefault("paz_azi_codice", elencoCampi)
                Dim paz_cat_codice As Integer = dr.GetOrdinalOrDefault("paz_cat_codice", elencoCampi)
                Dim paz_cag_codice As Integer = dr.GetOrdinalOrDefault("paz_cag_codice", elencoCampi)
                Dim paz_cancellato As Integer = dr.GetOrdinalOrDefault("paz_cancellato", elencoCampi)
                Dim paz_codice_demografico As Integer = dr.GetOrdinalOrDefault("paz_codice_demografico", elencoCampi)
                Dim paz_completare As Integer = dr.GetOrdinalOrDefault("paz_completare", elencoCampi)
                Dim paz_data_aggiornamento As Integer = dr.GetOrdinalOrDefault("paz_data_aggiornamento", elencoCampi)
                Dim paz_data_agg_da_comune As Integer = dr.GetOrdinalOrDefault("paz_data_agg_da_comune", elencoCampi)
                Dim paz_data_cancellazione As Integer = dr.GetOrdinalOrDefault("paz_data_cancellazione", elencoCampi)
                Dim paz_flag_stampa_maggiorenni As Integer = dr.GetOrdinalOrDefault("paz_flag_stampa_maggiorenni", elencoCampi)
                Dim paz_giorno As Integer = dr.GetOrdinalOrDefault("paz_giorno", elencoCampi)
                Dim paz_ind_codice_res As Integer = dr.GetOrdinalOrDefault("paz_ind_codice_res", elencoCampi)
                Dim paz_ind_codice_dom As Integer = dr.GetOrdinalOrDefault("paz_ind_codice_dom", elencoCampi)
                Dim paz_padre As Integer = dr.GetOrdinalOrDefault("paz_padre", elencoCampi)
                Dim paz_madre As Integer = dr.GetOrdinalOrDefault("paz_madre", elencoCampi)
                Dim paz_occasionale As Integer = dr.GetOrdinalOrDefault("paz_occasionale", elencoCampi)
                Dim paz_plb_id As Integer = dr.GetOrdinalOrDefault("paz_plb_id", elencoCampi)
                Dim paz_posizione_vaccinale_ok As Integer = dr.GetOrdinalOrDefault("paz_posizione_vaccinale_ok", elencoCampi)
                Dim paz_reg_assistenza As Integer = dr.GetOrdinalOrDefault("paz_reg_assistenza", elencoCampi)
                Dim paz_richiesta_cartella As Integer = dr.GetOrdinalOrDefault("paz_richiesta_cartella", elencoCampi)
                Dim paz_richiesta_certificato As Integer = dr.GetOrdinalOrDefault("paz_richiesta_certificato", elencoCampi)
                Dim paz_rsc_codice As Integer = dr.GetOrdinalOrDefault("paz_rsc_codice", elencoCampi)
                Dim paz_stato_acquisizione_imi As Integer = dr.GetOrdinalOrDefault("paz_stato_acquisizione_imi", elencoCampi)
                Dim paz_stato_anagrafico_dett As Integer = dr.GetOrdinalOrDefault("paz_stato_anagrafico_dett", elencoCampi)
                Dim paz_stato_notifica_emi As Integer = dr.GetOrdinalOrDefault("paz_stato_notifica_emi", elencoCampi)
                Dim paz_sta_certificato_emi As Integer = dr.GetOrdinalOrDefault("paz_sta_certificato_emi", elencoCampi)
                Dim paz_tipo As Integer = dr.GetOrdinalOrDefault("paz_tipo", elencoCampi)
                Dim paz_tipo_occasionalita As Integer = dr.GetOrdinalOrDefault("paz_tipo_occasionalita", elencoCampi)
                Dim paz_livello_certificazione As Integer = dr.GetOrdinalOrDefault("paz_livello_certificazione", elencoCampi)
                Dim paz_codice_esterno As Integer = dr.GetOrdinalOrDefault("paz_codice_esterno", elencoCampi)
                Dim paz_data_scadenza_ssn As Integer = dr.GetOrdinalOrDefault("paz_data_scadenza_ssn", elencoCampi)

                Dim nas_descrizione As Integer = dr.GetOrdinal("nas_descrizione")
                Dim cit_stato As Integer = dr.GetOrdinal("cit_stato")

                Dim master_cognome As Integer = dr.GetOrdinal("master_cognome")
                Dim master_nome As Integer = dr.GetOrdinal("master_nome")
                Dim master_data_nascita As Integer = dr.GetOrdinal("master_data_nascita")
                Dim master_codice_fiscale As Integer = dr.GetOrdinal("master_codice_fiscale")

                Dim paz_codice_master As Integer = dr.GetOrdinalOrDefault("paz_codice_master", elencoCampi)
                Dim paz_data_alias As Integer = dr.GetOrdinalOrDefault("paz_data_alias", elencoCampi)
                Dim paz_ute_id As Integer = dr.GetOrdinalOrDefault("paz_ute_id", elencoCampi)

                Dim paz_id_acn As Integer = dr.GetOrdinalOrDefault("paz_id_acn", elencoCampi)
                Dim paz_categoria_cittadino As Integer = dr.GetOrdinalOrDefault("paz_categoria_cittadino", elencoCampi)
                Dim paz_motivo_cessazione_assist As Integer = dr.GetOrdinalOrDefault("paz_motivo_cessazione_assist", elencoCampi)

                Dim pazienteAlias As PazienteAlias = Nothing

                While dr.Read()

                    pazienteAlias = New PazienteAlias()

                    pazienteAlias.Paz_Codice = dr.GetInt32OrDefault(paz_codice)

                    pazienteAlias.PAZ_COGNOME = dr.GetStringOrDefault(paz_cognome)
                    pazienteAlias.PAZ_NOME = dr.GetStringOrDefault(paz_nome)
                    pazienteAlias.Data_Nascita = dr.GetDateTimeOrDefault(paz_data_nascita)
                    pazienteAlias.PAZ_CODICE_FISCALE = dr.GetStringOrDefault(paz_codice_fiscale)
                    pazienteAlias.Sesso = dr.GetStringOrDefault(paz_sesso)

                    pazienteAlias.IndirizzoResidenza = dr.GetStringOrDefault(paz_indirizzo_residenza)
                    pazienteAlias.ComuneResidenza_Codice = dr.GetStringOrDefault(paz_com_codice_residenza)
                    pazienteAlias.ComuneResidenza_Descrizione = dr.GetStringOrDefault(res_descrizione)
                    pazienteAlias.ComuneResidenza_Provincia = dr.GetStringOrDefault(res_prv)
                    pazienteAlias.ComuneResidenza_Cap = dr.GetStringOrDefault(res_cap)

                    pazienteAlias.IndirizzoDomicilio = dr.GetStringOrDefault(paz_indirizzo_domicilio)
                    pazienteAlias.ComuneDomicilio_Codice = dr.GetStringOrDefault(paz_com_codice_domicilio)
                    pazienteAlias.ComuneDomicilio_Descrizione = dr.GetStringOrDefault(dom_descrizione)
                    pazienteAlias.ComuneDomicilio_Provincia = dr.GetStringOrDefault(dom_prv)
                    pazienteAlias.ComuneDomicilio_Cap = dr.GetStringOrDefault(dom_cap)

                    pazienteAlias.MedicoBase_Codice = dr.GetStringOrDefault(paz_med_codice_base)
                    pazienteAlias.MedicoBase_Descrizione = dr.GetStringOrDefault(med_descrizione)
                    pazienteAlias.Paz_Cns_Codice = dr.GetStringOrDefault(paz_cns_codice)

                    pazienteAlias.FlagCessato = dr.GetStringOrDefault(paz_flag_cessato)
                    pazienteAlias.FlagAire = dr.GetStringOrDefault(paz_aire)
                    pazienteAlias.Circoscrizione_Codice = dr.GetStringOrDefault(paz_cir_codice)
                    pazienteAlias.Cittadinanza_Codice = dr.GetStringOrDefault(paz_cit_codice)
                    pazienteAlias.Paz_Cns_Codice_Old = dr.GetStringOrDefault(paz_cns_codice_old)
                    pazienteAlias.Paz_Cns_Data_Assegnazione = dr.GetDateTimeOrDefault(paz_cns_data_assegnazione)
                    pazienteAlias.Paz_Cns_Terr_Codice = dr.GetStringOrDefault(paz_cns_terr_codice)
                    pazienteAlias.CodiceAusiliario = dr.GetStringOrDefault(paz_codice_ausiliario)
                    pazienteAlias.PAZ_CODICE_REGIONALE = dr.GetStringOrDefault(paz_codice_regionale)
                    pazienteAlias.ComuneNascita_Codice = dr.GetStringOrDefault(paz_com_codice_nascita)
                    pazienteAlias.ComuneEmigrazione_Codice = dr.GetStringOrDefault(paz_com_comune_emigrazione)
                    pazienteAlias.ComuneProvenienza_Codice = dr.GetStringOrDefault(paz_com_comune_provenienza)
                    pazienteAlias.DataAggiornamentoDaAnagrafe = dr.GetNullableDateTimeOrDefault(paz_data_agg_da_anag)
                    pazienteAlias.DataAire = dr.GetDateTimeOrDefault(paz_data_aire)
                    pazienteAlias.DataDecesso = dr.GetDateTimeOrDefault(paz_data_decesso)
                    pazienteAlias.MedicoBase_DataDecorrenza = dr.GetDateTimeOrDefault(paz_data_decorrenza_med)
                    pazienteAlias.DataEmigrazione = dr.GetDateTimeOrDefault(paz_data_emigrazione)
                    pazienteAlias.DataImmigrazione = dr.GetDateTimeOrDefault(paz_data_immigrazione)
                    pazienteAlias.DataInserimento = dr.GetDateTimeOrDefault(paz_data_inserimento)
                    pazienteAlias.DataIrreperibilita = dr.GetDateTimeOrDefault(paz_data_irreperibilita)
                    pazienteAlias.MedicoBase_DataRevoca = dr.GetDateTimeOrDefault(paz_data_revoca_med)
                    pazienteAlias.MedicoBase_DataScadenza = dr.GetDateTimeOrDefault(paz_data_scadenza_med)
                    pazienteAlias.Distretto_Codice = dr.GetStringOrDefault(paz_dis_codice)
                    pazienteAlias.FlagIrreperibilita = dr.GetStringOrDefault(paz_irreperibile)
                    pazienteAlias.FlagLocale = dr.GetStringOrDefault(paz_locale)
                    pazienteAlias.FlagRegolarizzato = dr.GetStringOrDefault(paz_regolarizzato)
                    pazienteAlias.Stato = dr.GetNullableEnumOrDefault(Of Enumerators.StatiVaccinali)(paz_stato)
                    pazienteAlias.StatoAnagrafico = dr.GetNullableEnumOrDefault(Of Enumerators.StatoAnagrafico)(paz_stato_anagrafico)
                    pazienteAlias.Telefono1 = dr.GetStringOrDefault(paz_telefono_1)
                    pazienteAlias.Telefono2 = dr.GetStringOrDefault(paz_telefono_2)
                    pazienteAlias.Telefono3 = dr.GetStringOrDefault(paz_telefono_3)
                    pazienteAlias.Email = dr.GetStringOrDefault(paz_email)
                    pazienteAlias.Tessera = dr.GetStringOrDefault(paz_tessera)
                    pazienteAlias.UslAssistenza_Codice = dr.GetStringOrDefault(paz_usl_codice_assistenza)
                    pazienteAlias.UslResidenza_Codice = dr.GetStringOrDefault(paz_usl_codice_residenza)
                    pazienteAlias.FlagAnonimo = dr.GetStringOrDefault(paz_anonimo)
                    pazienteAlias.CodiceAzienda = dr.GetStringOrDefault(paz_azi_codice)
                    pazienteAlias.CodiceCategoria1 = dr.GetStringOrDefault(paz_cat_codice)
                    pazienteAlias.CodiceCategoria2 = dr.GetStringOrDefault(paz_cag_codice)
                    pazienteAlias.FlagCancellato = dr.GetStringOrDefault(paz_cancellato)
                    pazienteAlias.CodiceDemografico = dr.GetStringOrDefault(paz_codice_demografico)
                    pazienteAlias.FlagCompletare = dr.GetStringOrDefault(paz_completare)
                    pazienteAlias.DataAggiornamento = dr.GetDateTimeOrDefault(paz_data_aggiornamento)
                    pazienteAlias.DataAggiornamentoDaComune = dr.GetDateTimeOrDefault(paz_data_agg_da_comune)
                    pazienteAlias.DataCancellazione = dr.GetDateTimeOrDefault(paz_data_cancellazione)
                    pazienteAlias.FlagStampaMaggiorenni = dr.GetStringOrDefault(paz_flag_stampa_maggiorenni)
                    pazienteAlias.Giorno = dr.GetStringOrDefault(paz_giorno)
                    pazienteAlias.CodiceIndirizzoResidenza = dr.GetNullableInt32OrDefault(paz_ind_codice_res)
                    pazienteAlias.CodiceIndirizzoDomicilio = dr.GetNullableInt32OrDefault(paz_ind_codice_dom)
                    pazienteAlias.Padre = dr.GetStringOrDefault(paz_padre)
                    pazienteAlias.Madre = dr.GetStringOrDefault(paz_madre)
                    pazienteAlias.FlagOccasionale = dr.GetStringOrDefault(paz_occasionale)
                    pazienteAlias.IdElaborazione = dr.GetNullableInt32OrDefault(paz_plb_id)
                    pazienteAlias.FlagPosizioneVaccinale = dr.GetStringOrDefault(paz_posizione_vaccinale_ok)
                    pazienteAlias.RegAssistenza = dr.GetStringOrDefault(paz_reg_assistenza)
                    pazienteAlias.FlagRichiestaCartella = dr.GetStringOrDefault(paz_richiesta_cartella)
                    pazienteAlias.FlagRichiestaCertificato = dr.GetStringOrDefault(paz_richiesta_certificato)
                    pazienteAlias.CodiceCategoriaRischio = dr.GetStringOrDefault(paz_rsc_codice)
                    pazienteAlias.StatoAcquisizioneImmigrazione = dr.GetStringOrDefault(paz_stato_acquisizione_imi)
                    pazienteAlias.StatoAnagraficoDettaglio = dr.GetStringOrDefault(paz_stato_anagrafico_dett)
                    pazienteAlias.StatoNotificaEmigrazione = dr.GetStringOrDefault(paz_stato_notifica_emi)
                    pazienteAlias.FlagStampaCertificatoEmigrazione = dr.GetStringOrDefault(paz_sta_certificato_emi)
                    pazienteAlias.Tipo = dr.GetStringOrDefault(paz_tipo)
                    pazienteAlias.TipoOccasionalita = dr.GetStringOrDefault(paz_tipo_occasionalita)
                    pazienteAlias.LivelloCertificazione = dr.GetInt32OrDefault(paz_livello_certificazione)
                    pazienteAlias.CodiceEsterno = dr.GetStringOrDefault(paz_codice_esterno)
                    pazienteAlias.DataScadenzaSSN = dr.GetNullableDateTimeOrDefault(paz_data_scadenza_ssn)

                    pazienteAlias.ComuneNascita_Descrizione = dr.GetStringOrDefault(nas_descrizione)
                    pazienteAlias.Cittadinanza_Descrizione = dr.GetStringOrDefault(cit_stato)

                    pazienteAlias.CognomeMaster = dr.GetStringOrDefault(master_cognome)
                    pazienteAlias.NomeMaster = dr.GetStringOrDefault(master_nome)
                    pazienteAlias.DataNascitaMaster = dr.GetDateTimeOrDefault(master_data_nascita)
                    pazienteAlias.CodiceFiscaleMaster = dr.GetStringOrDefault(master_codice_fiscale)

                    pazienteAlias.CodicePazienteMaster = dr.GetInt32OrDefault(paz_codice_master)
                    pazienteAlias.DataAlias = dr.GetDateTimeOrDefault(paz_data_alias)
                    pazienteAlias.IdUtente = dr.GetInt32OrDefault(paz_ute_id)

                    pazienteAlias.IdACN = dr.GetStringOrDefault(paz_id_acn)
                    pazienteAlias.CategoriaCittadino = dr.GetStringOrDefault(paz_categoria_cittadino)
                    pazienteAlias.MotivoCessazioneAssistenza = dr.GetStringOrDefault(paz_motivo_cessazione_assist)

                    listAlias.Add(pazienteAlias)

                End While

            End If

            Return listAlias

        End Function

        ''' <summary>
        ''' Restituisce la lista dei campi specificati nella stringa, che devono essere separati da virgole.
        ''' </summary>
        ''' <param name="elencoCampi"></param>
        ''' <returns></returns>
        Private Function GetListCampi(elencoCampi As String) As List(Of String)

            If String.IsNullOrEmpty(elencoCampi) Then Return New List(Of String)()

            Dim arrayCampi As String() = elencoCampi.Split(",")

            If arrayCampi Is Nothing OrElse arrayCampi.Length = 0 Then Return New List(Of String)()

            For i As Int16 = 0 To arrayCampi.Length - 1
                arrayCampi(i) = arrayCampi(i).Trim()
            Next

            Return arrayCampi.ToList()

        End Function

        ''' <summary>
        ''' Creazione query di controllo dell'esistenza dei record relativi al paziente nella tabella, verificando i campi specificati
        ''' </summary>
        ''' <param name="nomeTabella"></param>
        ''' <param name="nomeCampoCodicePaziente"></param>
        ''' <param name="codicePaziente"></param>
        ''' <param name="listCampiDaControllare"></param>
        ''' <returns></returns>
        Private Function CreateSubQueryExistsValoriAliasInMaster(nomeTabella As String, nomeCampoCodicePaziente As String, codicePaziente As Integer, listCampiDaControllare As List(Of String)) As String

            With _DAM.QB

                .NewQuery()

                .AddSelectFields("1")
                .AddTables(nomeTabella + " MASTER")
                .AddWhereCondition("MASTER." + nomeCampoCodicePaziente, Comparatori.Uguale, codicePaziente, DataTypes.Numero)

                Dim elencoCampi As List(Of String) = (From campo As String In listCampiDaControllare
                                                      Where campo <> nomeCampoCodicePaziente
                                                      Select campo).ToList()

                For Each nomeCampo As String In elencoCampi
                    .AddWhereCondition("MASTER." + nomeCampo, Comparatori.Uguale, "ALIAS." + nomeCampo, DataTypes.Join)
                Next

            End With

            Return _DAM.QB.GetSelect()

        End Function

        ''' <summary>
        ''' Aggiunta al command del parametro con nome e valore specificati.
        ''' Aggiunta del campo allo stringbuilder dei nomi dei campi e aggiunta del nome del parametro allo stringbuilder dei valori
        ''' </summary>
        ''' <param name="nomeCampo"></param>
        ''' <param name="valoreCampo"></param>
        ''' <param name="nomeCampi"></param>
        ''' <param name="nomeParametri"></param>
        ''' <param name="cmd"></param>
        Private Sub AddToCommand(nomeCampo As String, valoreCampo As Object, nomeCampi As System.Text.StringBuilder, nomeParametri As Text.StringBuilder, cmd As OracleCommand)

            nomeCampi.AppendFormat("{0}, ", nomeCampo)
            nomeParametri.AppendFormat(":{0}, ", nomeCampo)

            If valoreCampo Is Nothing Then
                cmd.Parameters.AddWithValue(nomeCampo, DBNull.Value)
            Else
                cmd.Parameters.AddWithValue(nomeCampo, valoreCampo)
            End If

        End Sub

#End Region

#Region " Merge Massivo "

        ''' <summary>
        ''' Restituisce una lista di coppie di codici master-alias da unificare.
        ''' </summary>
        ''' <param name="dataUltimaElaborazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCodiciPazientiDaUnificare(dataUltimaElaborazione As DateTime?) As List(Of KeyValuePair(Of Integer, Integer)) Implements IAliasProvider.GetCodiciPazientiDaUnificare

            Dim list As New List(Of KeyValuePair(Of Integer, Integer))()

            Using cmd As OracleCommand = Connection.CreateCommand()

                Dim filtro As String = String.Empty

                If dataUltimaElaborazione.HasValue Then
                    filtro = "where pmr_data_merge > :data_merge"
                    cmd.Parameters.AddWithValue("data_merge", dataUltimaElaborazione)
                End If

                cmd.CommandText = String.Format("select pmr_paz_codice_master, pmr_paz_codice_alias from t_paz_merge {0} ", filtro)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If idr IsNot Nothing Then

                            Dim pmr_paz_codice_master As Integer = idr.GetOrdinal("pmr_paz_codice_master")
                            Dim pmr_paz_codice_alias As Integer = idr.GetOrdinal("pmr_paz_codice_alias")

                            While idr.Read()
                                list.Add(New KeyValuePair(Of Integer, Integer)(idr.GetInt32(pmr_paz_codice_master), idr.GetInt32(pmr_paz_codice_alias)))
                            End While

                        End If
                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list

        End Function

#End Region

    End Class

End Namespace
