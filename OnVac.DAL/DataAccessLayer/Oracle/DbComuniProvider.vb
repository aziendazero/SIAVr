Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager

Namespace DAL.Oracle

    Public Class DbComuniProvider
        Inherits DbProvider
        Implements IComuniProvider

#Region " Constructors "

        Public Sub New(DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

        ''' <summary>
        ''' Restituisce il codice Istat del comune in base al codice.
        ''' </summary>
        ''' <param name="codiceComune"></param>
        ''' <returns></returns>
        Public Function GetIstatByCodice(codiceComune As String) As String Implements IComuniProvider.GetIstatByCodice

            Dim codiceIstat As String = String.Empty

            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                cmd.Parameters.AddWithValue("com_codice", GetStringParam(codiceComune, False))

                cmd.CommandText = Queries.Comuni.OracleQueries.selIstatByCodice

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj Is Nothing OrElse obj Is DBNull.Value Then
                        codiceIstat = String.Empty
                    Else
                        codiceIstat = obj.ToString()
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return codiceIstat

        End Function


        ''' <summary>
        ''' Restituisce il cap del comune in base al codice.
        ''' </summary>
        ''' <param name="codiceComune"></param>
        ''' <returns></returns>
        Public Function GetCapByCodiceComune(codiceComune As String) As String Implements IComuniProvider.GetCapByCodiceComune

            Dim cap As String = String.Empty

            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                cmd.Parameters.AddWithValue("com_codice", GetStringParam(codiceComune, False))

                cmd.CommandText = Queries.Comuni.OracleQueries.selCapByCodice

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj Is Nothing OrElse obj Is DBNull.Value Then
                        cap = String.Empty
                    Else
                        cap = obj.ToString()
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return cap

        End Function

        ''' <summary>
        ''' Restituisce true se il comune è valido alla data specificata, false altrimenti. 
        ''' Il controllo è effettuato sui campi :
        '''  - com_obsoleto, che vale S se il comune è valido oppure N se il comune è obsoleto;
        '''  - com_data_inizio_validita e com_data_fine_validita, che rappresentano l'intervallo di validità
        '''    all'interno del quale si deve trovare la data di validità specificata.
        ''' </summary>
        ''' <param name="codiceComune"></param>
        ''' <param name="dataValidita"></param>
        ''' <returns></returns>
        Public Function CheckValiditaComune(codiceComune As String, dataValidita As Date) As Boolean Implements IComuniProvider.CheckValiditaComune

            Dim toCheck As Boolean = True

            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                cmd.Parameters.AddWithValue("cod_com", GetStringParam(codiceComune, False))
                cmd.Parameters.AddWithValue("data_val", GetDateParam(dataValidita))

                cmd.CommandText = Queries.Comuni.OracleQueries.chkValiditaComune

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj Is Nothing OrElse obj Is DBNull.Value Then
                        toCheck = False
                    Else
                        toCheck = True
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return toCheck

        End Function

        Public Function GetListComuni(ricerca As String) As List(Of Comune) Implements IComuniProvider.GetListComuni

            Dim elenco As New List(Of Comune)()

            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                Dim query As String =
                    "select com_codice, com_codice_esterno, com_descrizione, com_provincia, com_istat, com_catastale, com_cap, " +
                    "com_data_inizio_validita, com_data_fine_validita, com_obsoleto, com_scadenza, com_motivo_scadenza " +
                    "from t_ana_comuni " +
                    "where (com_scadenza is null or com_scadenza = 'N') "

                If Not String.IsNullOrWhiteSpace(ricerca) Then
                    query += "and com_descrizione like :d"
                    cmd.Parameters.AddWithValue("d", GetStringParam(String.Format("{0}%", ricerca), False))
                End If

                cmd.CommandText = query

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim com_codice As Int16 = idr.GetOrdinal("com_codice")
                            Dim com_codice_esterno As Int16 = idr.GetOrdinal("com_codice_esterno")
                            Dim com_descrizione As Int16 = idr.GetOrdinal("com_descrizione")
                            Dim com_cap As Int16 = idr.GetOrdinal("com_cap")
                            Dim com_provincia As Int16 = idr.GetOrdinal("com_provincia")
                            Dim com_istat As Int16 = idr.GetOrdinal("com_istat")
                            Dim com_catastale As Int16 = idr.GetOrdinal("com_catastale")
                            Dim com_data_inizio_validita As Int16 = idr.GetOrdinal("com_data_inizio_validita")
                            Dim com_data_fine_validita As Int16 = idr.GetOrdinal("com_data_fine_validita")
                            Dim com_scadenza As Int16 = idr.GetOrdinal("com_scadenza")
                            Dim com_motivo_scadenza As Int16 = idr.GetOrdinal("com_motivo_scadenza")
                            Dim com_obsoleto As Int16 = idr.GetOrdinal("com_obsoleto")

                            While idr.Read()

                                Dim comune As Comune = New Comune()

                                comune.Codice = idr.GetStringOrDefault(com_codice)
                                comune.CodiceEsterno = idr.GetStringOrDefault(com_codice_esterno)
                                comune.Descrizione = idr.GetStringOrDefault(com_descrizione)
                                comune.Cap = idr.GetStringOrDefault(com_cap)
                                comune.Provincia = idr.GetStringOrDefault(com_provincia)
                                comune.Istat = idr.GetStringOrDefault(com_istat)
                                comune.Catastale = idr.GetStringOrDefault(com_catastale)
                                comune.DataInizioValidita = idr.GetDateTimeOrDefault(com_data_inizio_validita)
                                comune.DataFineValidita = idr.GetDateTimeOrDefault(com_data_fine_validita)
                                comune.Scadenza = idr.GetBooleanOrDefault(com_scadenza)
                                comune.MotivoScadenza = idr.GetStringOrDefault(com_motivo_scadenza)
                                comune.Obsoleto = idr.GetBooleanOrDefault(com_obsoleto)

                                elenco.Add(comune)

                            End While

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return elenco

        End Function

        Public Function GetListComuniEsterniRegione(ricerca As String, codiceRegione As String) As List(Of Comune) Implements IComuniProvider.GetListComuniEsterniRegione

            Dim elenco As New List(Of Comune)()

            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                Dim query As String =
                    "select com_codice, com_codice_esterno, com_descrizione, com_provincia, com_istat, com_catastale, com_cap, " +
                    "com_data_inizio_validita, com_data_fine_validita, com_obsoleto, com_scadenza, com_motivo_scadenza " +
                    "from t_ana_comuni " +
                    "where (com_scadenza is null or com_scadenza = 'N') and com_reg_codice <> :codReg "

                cmd.Parameters.AddWithValue("codReg", GetStringParam(codiceRegione, False))
                '' cmd.Parameters.AddWithValue("comCodice", GetStringParam(codiceComune, False))

                If Not String.IsNullOrWhiteSpace(ricerca) Then
                    query += "and com_descrizione like :d"
                    cmd.Parameters.AddWithValue("d", GetStringParam(String.Format("{0}%", ricerca), False))
                End If

                cmd.CommandText = query

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim com_codice As Int16 = idr.GetOrdinal("com_codice")
                            Dim com_codice_esterno As Int16 = idr.GetOrdinal("com_codice_esterno")
                            Dim com_descrizione As Int16 = idr.GetOrdinal("com_descrizione")
                            Dim com_cap As Int16 = idr.GetOrdinal("com_cap")
                            Dim com_provincia As Int16 = idr.GetOrdinal("com_provincia")
                            Dim com_istat As Int16 = idr.GetOrdinal("com_istat")
                            Dim com_catastale As Int16 = idr.GetOrdinal("com_catastale")
                            Dim com_data_inizio_validita As Int16 = idr.GetOrdinal("com_data_inizio_validita")
                            Dim com_data_fine_validita As Int16 = idr.GetOrdinal("com_data_fine_validita")
                            Dim com_scadenza As Int16 = idr.GetOrdinal("com_scadenza")
                            Dim com_motivo_scadenza As Int16 = idr.GetOrdinal("com_motivo_scadenza")
                            Dim com_obsoleto As Int16 = idr.GetOrdinal("com_obsoleto")

                            While idr.Read()

                                Dim comune As Comune = New Comune()

                                comune.Codice = idr.GetStringOrDefault(com_codice)
                                comune.CodiceEsterno = idr.GetStringOrDefault(com_codice_esterno)
                                comune.Descrizione = idr.GetStringOrDefault(com_descrizione)
                                comune.Cap = idr.GetStringOrDefault(com_cap)
                                comune.Provincia = idr.GetStringOrDefault(com_provincia)
                                comune.Istat = idr.GetStringOrDefault(com_istat)
                                comune.Catastale = idr.GetStringOrDefault(com_catastale)
                                comune.DataInizioValidita = idr.GetDateTimeOrDefault(com_data_inizio_validita)
                                comune.DataFineValidita = idr.GetDateTimeOrDefault(com_data_fine_validita)
                                comune.Scadenza = idr.GetBooleanOrDefault(com_scadenza)
                                comune.MotivoScadenza = idr.GetStringOrDefault(com_motivo_scadenza)
                                comune.Obsoleto = idr.GetBooleanOrDefault(com_obsoleto)

                                elenco.Add(comune)

                            End While

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return elenco

        End Function
        Public Function GetComuni() As List(Of Comune) Implements IComuniProvider.GetComuni

            Dim elenco As New List(Of Comune)()

            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                Dim query As String =
                    "select com_codice, com_codice_esterno, com_descrizione, com_provincia, com_istat, com_catastale, com_cap, " +
                    "com_data_inizio_validita, com_data_fine_validita, com_obsoleto, com_scadenza, com_motivo_scadenza " +
                    "from t_ana_comuni " +
                    "where com_scadenza = 'N' "

                cmd.CommandText = query

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim com_codice As Int16 = idr.GetOrdinal("com_codice")
                            Dim com_codice_esterno As Int16 = idr.GetOrdinal("com_codice_esterno")
                            Dim com_descrizione As Int16 = idr.GetOrdinal("com_descrizione")
                            Dim com_cap As Int16 = idr.GetOrdinal("com_cap")
                            Dim com_provincia As Int16 = idr.GetOrdinal("com_provincia")
                            Dim com_istat As Int16 = idr.GetOrdinal("com_istat")
                            Dim com_catastale As Int16 = idr.GetOrdinal("com_catastale")
                            Dim com_data_inizio_validita As Int16 = idr.GetOrdinal("com_data_inizio_validita")
                            Dim com_data_fine_validita As Int16 = idr.GetOrdinal("com_data_fine_validita")
                            Dim com_scadenza As Int16 = idr.GetOrdinal("com_scadenza")
                            Dim com_motivo_scadenza As Int16 = idr.GetOrdinal("com_motivo_scadenza")
                            Dim com_obsoleto As Int16 = idr.GetOrdinal("com_obsoleto")

                            While idr.Read()

                                Dim comune As Comune = New Comune()

                                comune.Codice = idr.GetStringOrDefault(com_codice)
                                comune.CodiceEsterno = idr.GetStringOrDefault(com_codice_esterno)
                                comune.Descrizione = idr.GetStringOrDefault(com_descrizione)
                                comune.Cap = idr.GetStringOrDefault(com_cap)
                                comune.Provincia = idr.GetStringOrDefault(com_provincia)
                                comune.Istat = idr.GetStringOrDefault(com_istat)
                                comune.Catastale = idr.GetStringOrDefault(com_catastale)
                                comune.DataInizioValidita = idr.GetDateTimeOrDefault(com_data_inizio_validita)
                                comune.DataFineValidita = idr.GetDateTimeOrDefault(com_data_fine_validita)
                                comune.Scadenza = idr.GetBooleanOrDefault(com_scadenza)
                                comune.MotivoScadenza = idr.GetStringOrDefault(com_motivo_scadenza)
                                comune.Obsoleto = idr.GetBooleanOrDefault(com_obsoleto)

                                elenco.Add(comune)

                            End While

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return elenco

        End Function
        ''' <summary>
        ''' Restituisce un oggetto della classe Comune contenente tutti i dati del comune specificato.
        ''' </summary>
        ''' <param name="codiceComune"></param>
        ''' <returns></returns>
        Public Function GetComuneByCodice(codiceComune As String) As Comune Implements IComuniProvider.GetComuneByCodice

            Dim comune As Comune = Nothing

            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                cmd.Parameters.AddWithValue("comCodice", GetStringParam(codiceComune, False))

                cmd.CommandText = Queries.Comuni.OracleQueries.selComuneByCodiceInterno

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim com_codice As Int16 = idr.GetOrdinal("com_codice")
                            Dim com_codice_esterno As Int16 = idr.GetOrdinal("com_codice_esterno")
                            Dim com_descrizione As Int16 = idr.GetOrdinal("com_descrizione")
                            Dim com_cap As Int16 = idr.GetOrdinal("com_cap")
                            Dim com_provincia As Int16 = idr.GetOrdinal("com_provincia")
                            Dim com_istat As Int16 = idr.GetOrdinal("com_istat")
                            Dim com_catastale As Int16 = idr.GetOrdinal("com_catastale")
                            Dim com_data_inizio_validita As Int16 = idr.GetOrdinal("com_data_inizio_validita")
                            Dim com_data_fine_validita As Int16 = idr.GetOrdinal("com_data_fine_validita")
                            Dim com_scadenza As Int16 = idr.GetOrdinal("com_scadenza")
                            Dim com_motivo_scadenza As Int16 = idr.GetOrdinal("com_motivo_scadenza")
                            Dim com_obsoleto As Int16 = idr.GetOrdinal("com_obsoleto")

                            If idr.Read() Then

                                comune = New Comune()

                                comune.Codice = idr.GetStringOrDefault(com_codice)
                                comune.CodiceEsterno = idr.GetStringOrDefault(com_codice_esterno)
                                comune.Descrizione = idr.GetStringOrDefault(com_descrizione)
                                comune.Cap = idr.GetStringOrDefault(com_cap)
                                comune.Provincia = idr.GetStringOrDefault(com_provincia)
                                comune.Istat = idr.GetStringOrDefault(com_istat)
                                comune.Catastale = idr.GetStringOrDefault(com_catastale)
                                comune.DataInizioValidita = idr.GetDateTimeOrDefault(com_data_inizio_validita)
                                comune.DataFineValidita = idr.GetDateTimeOrDefault(com_data_fine_validita)
                                comune.Scadenza = idr.GetBooleanOrDefault(com_scadenza)
                                comune.MotivoScadenza = idr.GetStringOrDefault(com_motivo_scadenza)
                                comune.Obsoleto = idr.GetBooleanOrDefault(com_obsoleto)

                            End If

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return comune

        End Function

        ''' <summary>
        ''' Restituisce il codice del comune in base al codice Istat valido alla data specificata.
        ''' </summary>
        ''' <param name="codiceIstatComune"></param>
        ''' <param name="dataValidita"></param>
        ''' <returns></returns>
        Public Function GetCodiceComuneByCodiceIstat(codiceIstatComune As String, dataValidita As Date) As String Implements IComuniProvider.GetCodiceComuneByCodiceIstat

            Dim codiceComune As String = String.Empty

            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                cmd.Parameters.AddWithValue("codiceIstat", GetStringParam(codiceIstatComune, False))
                cmd.Parameters.AddWithValue("dataValidita", dataValidita)

                cmd.CommandText = Queries.Comuni.OracleQueries.selCodComuneByIstatStorico

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        codiceComune = obj.ToString()
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return codiceComune

        End Function

        ''' <summary>
        ''' Restituisce il codice del comune in base al codice Istat, senza considerare le date di validità.
        ''' </summary>
        ''' <param name="codiceIstatComune"></param>
        ''' <returns></returns>
        Public Function GetCodiceComuneByCodiceIstat(codiceIstatComune As String) As String Implements IComuniProvider.GetCodiceComuneByCodiceIstat

            Dim codiceComune As String = String.Empty

            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                cmd.Parameters.AddWithValue("codiceIstat", GetStringParam(codiceIstatComune, False))

                cmd.CommandText = Queries.Comuni.OracleQueries.selCodComuniByIstat

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        codiceComune = obj.ToString()
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return codiceComune

        End Function

        ''' <summary>
        ''' Restituisce il codice della regione del comune specificato
        ''' </summary>
        ''' <param name="codiceComune"></param>
        ''' <returns></returns>
        Public Function GetCodiceRegione(codiceComune As String) As String Implements IComuniProvider.GetCodiceRegione

            Dim codiceRegione As String = String.Empty

            Using cmd As New OracleClient.OracleCommand("select com_reg_codice from t_ana_comuni where com_codice = :com_codice", Connection)

                cmd.Parameters.AddWithValue("com_codice", codiceComune)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then

                        codiceRegione = obj.ToString()

                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return codiceRegione

        End Function
    End Class

End Namespace

