Imports Onit.Database.DataAccessManager
Imports System.Collections.Generic
Imports System.Data.OracleClient

Namespace DAL
    Public Class DbInterventiProvider
        Inherits DbProvider
        Implements IInterventiProvider

#Region " Costruttori "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region


#Region " Anagrafica "

#Region " Select "

        ''' <summary>
        ''' Caricamento interventi
        ''' </summary>
        Public Function GetInterventi(filtro As String) As List(Of Intervento) Implements IInterventiProvider.GetInterventi

            Dim listInterventi As List(Of Intervento) = Nothing
            Dim whereCond As String = String.Empty

            If Not String.IsNullOrWhiteSpace(filtro) Then
                whereCond = String.Format("where lower(int_descrizione) like '%{0}%' or int_codice like '%{0}%'", filtro.ToLower())
            End If

            Dim query As String = String.Format(Queries.Interventi.OracleQueries.selInterventi, whereCond)

            Using cmd As OracleCommand = New OracleClient.OracleCommand(query, Me.Connection)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim int_codice As Integer = idr.GetOrdinal("INT_CODICE")
                            Dim int_descrizione As Integer = idr.GetOrdinal("INT_DESCRIZIONE")
                            Dim int_tipologia As Integer = idr.GetOrdinal("INT_TIPOLOGIA")
                            Dim int_durata As Integer = idr.GetOrdinal("INT_DURATA")

                            listInterventi = New List(Of Intervento)()

                            Dim intervento As Intervento = Nothing

                            While idr.Read()

                                intervento = New Intervento()

                                intervento.Codice = idr.GetInt32OrDefault(int_codice)
                                intervento.Descrizione = idr.GetStringOrDefault(int_descrizione)
                                intervento.Tipologia = idr.GetStringOrDefault(int_tipologia)
                                intervento.Durata = idr.GetInt32OrDefault(int_durata)

                                listInterventi.Add(intervento)

                            End While

                        End If

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return listInterventi

        End Function

        ''' <summary>
        ''' Caricamento singolo intervento
        ''' </summary>
        Public Function GetIntervento(codiceIntervento As Integer) As Entities.Intervento Implements IInterventiProvider.GetIntervento

            Dim intervento As Entities.Intervento = Nothing

            Using cmd As OracleCommand = New OracleClient.OracleCommand(Queries.Interventi.OracleQueries.selIntervento, Me.Connection)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("cod_int", codiceIntervento)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim int_codice As Integer = idr.GetOrdinal("INT_CODICE")
                            Dim int_descrizione As Integer = idr.GetOrdinal("INT_DESCRIZIONE")
                            Dim int_tipologia As Integer = idr.GetOrdinal("INT_TIPOLOGIA")
                            Dim int_durata As Integer = idr.GetOrdinal("INT_DURATA")

                            If idr.Read() Then

                                intervento = New Intervento()
                                intervento.Codice = idr.GetInt32OrDefault(int_codice)
                                intervento.Descrizione = idr.GetStringOrDefault(int_descrizione)
                                intervento.Tipologia = idr.GetStringOrDefault(int_tipologia)
                                intervento.Durata = idr.GetInt32OrDefault(int_durata)

                            End If
                        End If

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return intervento

        End Function

#End Region

#Region " Insert/Update/Delete "

        ''' <summary>
        ''' Inserisci intervento
        ''' </summary>
        Public Function InsertIntervento(intervento As Entities.Intervento) As Integer Implements IInterventiProvider.InsertIntervento

            Dim idIntervento As Integer = 0

            Using cmd As OracleCommand = New OracleClient.OracleCommand(Queries.Interventi.OracleQueries.insIntervento, Me.Connection)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("descrizione", intervento.Descrizione)
                    cmd.Parameters.AddWithValue("tipologia", intervento.Tipologia)
                    cmd.Parameters.AddWithValue("durata", intervento.Durata)

                    'Id da recuperare per il nuovo inserimento
                    Dim idParam As OracleParameter = New OracleParameter() With
                        {
                            .ParameterName = "id",
                            .OracleType = OracleType.Int32,
                            .Direction = ParameterDirection.Output
                        }
                    cmd.Parameters.Add(idParam)

                    cmd.ExecuteNonQuery()

                    If Not cmd.Parameters("id").Value Is Nothing Then
                        idIntervento = cmd.Parameters("id").Value
                    End If

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return idIntervento

        End Function

        ''' <summary>
        ''' Aggiornamento intervento
        ''' </summary>
        Public Function UpdateIntervento(intervento As Entities.Intervento) As Integer Implements IInterventiProvider.UpdateIntervento

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand(Queries.Interventi.OracleQueries.updIntervento, Me.Connection)

                cmd.Connection = Me.Connection
                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("descrizione", intervento.Descrizione)
                    cmd.Parameters.AddWithValue("tipologia", intervento.Tipologia)
                    cmd.Parameters.AddWithValue("durata", intervento.Durata)
                    cmd.Parameters.AddWithValue("cod_int", intervento.Codice)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Elimina intervento
        ''' </summary>
        Public Function DeleteIntervento(codiceIntervento As Integer) As Integer Implements IInterventiProvider.DeleteIntervento

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand(Queries.Interventi.OracleQueries.delIntervento, Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)
                    cmd.Parameters.AddWithValue("cod_int", codiceIntervento)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

#End Region

#End Region

#Region " Inteventi Paziente "

#Region " Select "

        ''' <summary>
        ''' Caricamento interventi paziente
        ''' </summary>
        ''' <param name="parametri"></param>
        ''' <returns></returns>
        Public Function GetInterventiPaziente(parametri As ParametriGetInterventiPaziente) As List(Of InterventoPaziente) Implements IInterventiProvider.GetInterventiPaziente

            If parametri Is Nothing Then Throw New ArgumentNullException()

            Dim listInterventi As List(Of InterventoPaziente) = Nothing
            Dim whereCond As String = String.Empty

            Dim query As String = String.Format(Queries.Interventi.OracleQueries.selInterventiPaziente, parametri.OrderBy)

            Using cmd As OracleCommand = New OracleCommand(query, Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("cod_paziente", parametri.CodicePaziente)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim pit_codice As Integer = idr.GetOrdinal("pit_codice")
                            Dim pit_data_intervento As Integer = idr.GetOrdinal("pit_data_intervento")
                            Dim pit_durata As Integer = idr.GetOrdinal("pit_durata")
                            Dim pit_note As Integer = idr.GetOrdinal("pit_note")
                            Dim int_codice As Integer = idr.GetOrdinal("int_codice")
                            Dim int_descrizione As Integer = idr.GetOrdinal("int_descrizione")
                            Dim int_tipologia As Integer = idr.GetOrdinal("int_tipologia")
                            Dim pit_ope_codice As Integer = idr.GetOrdinal("pit_ope_codice")
                            Dim ope_nome As Integer = idr.GetOrdinal("ope_nome")

                            listInterventi = New List(Of InterventoPaziente)()

                            Dim intervento As InterventoPaziente = Nothing

                            While idr.Read()

                                intervento = New InterventoPaziente()

                                intervento.Codice = idr.GetInt32OrDefault(pit_codice)
                                intervento.Data = idr.GetDateTimeOrDefault(pit_data_intervento)
                                intervento.Durata = idr.GetInt32OrDefault(pit_durata)
                                intervento.Note = idr.GetStringOrDefault(pit_note)
                                intervento.CodiceIntervento = idr.GetInt32OrDefault(int_codice)
                                intervento.Intervento = idr.GetStringOrDefault(int_descrizione)
                                intervento.Tipologia = idr.GetStringOrDefault(int_tipologia)
                                intervento.Operatore = idr.GetStringOrDefault(ope_nome)
                                intervento.CodiceOperatore = idr.GetStringOrDefault(pit_ope_codice)

                                listInterventi.Add(intervento)

                            End While

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listInterventi

        End Function

        ''' <summary>
        ''' Conteggio interventi paziente
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CountInterventiPaziente(codicePaziente As Long) As Integer Implements IInterventiProvider.CountInterventiPaziente

            Dim count As Integer = 0

            Using cmd As New OracleCommand(Queries.Interventi.OracleQueries.countInterventiPaziente, Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("cod_paziente", codicePaziente)

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

#Region " Insert/Update/Delete "

        ''' <summary>
        ''' Inserisci intervento paziente
        ''' </summary>
        ''' <param name="intervento"></param>
        ''' <param name="codicePaziente"></param>
        ''' <param name="uteId"></param>
        ''' <returns></returns>
        Public Function InsertInterventoPaziente(intervento As InterventoPaziente, codicePaziente As Integer, uteId As Long) As Integer Implements IInterventiProvider.InsertInterventoPaziente

            Dim idIntervento As Integer = 0

            Using cmd As New OracleCommand(Queries.Interventi.OracleQueries.insInterventoPaziente, Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                    cmd.Parameters.AddWithValue("cod_int", intervento.CodiceIntervento)
                    cmd.Parameters.AddWithValue("data_int", intervento.Data)
                    cmd.Parameters.AddWithValue("durata", intervento.Durata)
                    cmd.Parameters.AddWithValue("operatore", intervento.CodiceOperatore)
                    cmd.Parameters.AddWithValue("note", intervento.Note)
                    cmd.Parameters.AddWithValue("uteReg", uteId)
                    cmd.Parameters.AddWithValue("dataReg", DateTime.Now)

                    'Id da recuperare per il nuovo inserimento
                    Dim idParam As OracleParameter = New OracleParameter() With
                        {
                            .ParameterName = "id",
                            .OracleType = OracleType.Int32,
                            .Direction = ParameterDirection.Output
                        }
                    cmd.Parameters.Add(idParam)

                    cmd.ExecuteNonQuery()

                    If Not cmd.Parameters("id").Value Is Nothing Then
                        idIntervento = cmd.Parameters("id").Value
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return idIntervento

        End Function

        ''' <summary>
        ''' Aggiornamento intervento paziente
        ''' </summary>
        ''' <param name="intervento"></param>
        ''' <returns></returns>
        Public Function UpdateInterventoPaziente(intervento As InterventoPaziente) As Integer Implements IInterventiProvider.UpdateInterventoPaziente

            Dim count As Integer = 0

            Using cmd As New OracleCommand(Queries.Interventi.OracleQueries.updInterventoPaziente, Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("cod_int", intervento.CodiceIntervento)
                    cmd.Parameters.AddWithValue("data_int", intervento.Data)
                    cmd.Parameters.AddWithValue("durata", intervento.Durata)
                    cmd.Parameters.AddWithValue("operatore", intervento.CodiceOperatore)
                    cmd.Parameters.AddWithValue("note", intervento.Note)
                    cmd.Parameters.AddWithValue("pit_codice", intervento.Codice)

                    count = cmd.ExecuteNonQuery()

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Eliminazione logica intervento paziente
        ''' </summary>
        ''' <param name="codiceIntervento"></param>
        ''' <param name="uteId"></param>
        ''' <returns></returns>
        Public Function DeleteInterventoPaziente(codiceIntervento As Integer, uteId As Long) As Integer Implements IInterventiProvider.DeleteInterventoPaziente

            Dim count As Integer = 0

            Using cmd As New OracleCommand(Queries.Interventi.OracleQueries.delInterventoPaziente, Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)
                    cmd.Parameters.AddWithValue("uteId", uteId)
                    cmd.Parameters.AddWithValue("data", DateTime.Now)
                    cmd.Parameters.AddWithValue("pit_codice", codiceIntervento)

                    count = cmd.ExecuteNonQuery()

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

#End Region

#End Region

#Region " Stampa Consulenze "

        Public Function GetDataSetConteggioConsulenze(dataNascitaInizio As DateTime?, dataNascitaFine As DateTime?, dataEsecuzioneInizio As DateTime?, dataEsecuzioneFine As DateTime?, codiciConsultori As List(Of String), codiceTipoConsulenza As String, codiceOperatore As String) As DSConteggioConsulenze Implements IInterventiProvider.GetDataSetConteggioConsulenze

            Dim dSConteggioConsulenze As New DSConteggioConsulenze

            With _DAM.QB
                .NewQuery()
                .IsDistinct = True
                .AddSelectFields("PAZ_CNS_CODICE CodCNS, CNS_DESCRIZIONE DescrCNS, PIT_INT_CODICE CodTipoConsulenza, INT_DESCRIZIONE DescrTipoConsulenza, COUNT(PIT_CODICE) ConteggioConsulenze ")
                .AddTables("T_PAZ_INTERVENTI, T_ANA_INTERVENTI, T_PAZ_PAZIENTI, T_ANA_CONSULTORI")
                .AddWhereCondition("PIT_INT_CODICE", Comparatori.Uguale, "INT_CODICE", DataTypes.Join)
                .AddWhereCondition("PIT_PAZ_CODICE", Comparatori.Uguale, "PAZ_CODICE", DataTypes.Join)
                .AddWhereCondition("PAZ_CNS_CODICE", Comparatori.Uguale, "CNS_CODICE", DataTypes.OutJoinLeft)
                .AddWhereCondition("PIT_UTE_ID_ELIMINAZIONE", Comparatori.Is, "NULL", DataTypes.Replace)
                .AddWhereCondition("PIT_DATA_ELIMINAZIONE", Comparatori.Is, "NULL", DataTypes.Replace)
                .AddGroupByFields("PAZ_CNS_CODICE, CNS_DESCRIZIONE, PIT_INT_CODICE, INT_DESCRIZIONE")
                .AddOrderByFields("CNS_DESCRIZIONE, PAZ_CNS_CODICE, INT_DESCRIZIONE, PAZ_CNS_CODICE")
            End With

            SetFiltri(dataNascitaInizio, dataNascitaFine, dataEsecuzioneInizio, dataEsecuzioneFine, codiciConsultori, codiceTipoConsulenza, codiceOperatore, _DAM.QB)

            _DAM.BuildDataTable(dSConteggioConsulenze.ConteggioConsulenze)

            Return dSConteggioConsulenze



        End Function


        Private Sub SetFiltri(dataNascitaInizio As DateTime?, dataNascitaFine As DateTime?, dataEsecuzioneInizio As DateTime?, dataEsecuzioneFine As DateTime?, codiciConsultori As List(Of String), codiceTipoConsulenza As String, codiceOperatore As String, ByRef abstractQB As AbstractQB)

            With abstractQB

                'DATA NASCITA
                If dataNascitaInizio.HasValue Then
                    .AddWhereCondition("PAZ_DATA_NASCITA", Comparatori.MaggioreUguale, dataNascitaInizio.Value, DataTypes.Data)
                End If
                If dataNascitaFine.HasValue Then
                    .AddWhereCondition("PAZ_DATA_NASCITA", Comparatori.MinoreUguale, dataNascitaFine.Value, DataTypes.Data)
                End If

                'DATA ESECUZIONE CONSULENZA
                If dataEsecuzioneInizio.HasValue Then
                    .AddWhereCondition("PIT_DATA_INTERVENTO", Comparatori.MaggioreUguale, dataEsecuzioneInizio.Value, DataTypes.Data)
                End If
                If dataEsecuzioneFine.HasValue Then
                    .AddWhereCondition("PIT_DATA_INTERVENTO", Comparatori.MinoreUguale, dataEsecuzioneFine.Value, DataTypes.Data)
                End If

                ' CONSULTORI
                If Not codiciConsultori Is Nothing AndAlso codiciConsultori.Count > 0 Then
                    Dim filtroIn As New System.Text.StringBuilder()
                    For i As Integer = 0 To codiciConsultori.Count - 1
                        filtroIn.AppendFormat("{0},", .AddCustomParam(codiciConsultori(i).ToUpper()))
                    Next
                    If filtroIn.Length > 0 Then
                        filtroIn.Remove(filtroIn.Length - 1, 1)
                        .AddWhereCondition("PAZ_CNS_CODICE", Comparatori.In, filtroIn.ToString(), DataTypes.Replace)
                    End If
                End If

                ' TIPO CONSULENZA
                If Not String.IsNullOrEmpty(codiceTipoConsulenza) Then
                    .AddWhereCondition("PIT_INT_CODICE", Comparatori.Uguale, codiceTipoConsulenza, DataTypes.Stringa)
                End If

                ' OPERATORE
                If Not String.IsNullOrEmpty(codiceOperatore) Then
                    .AddWhereCondition("PIT_OPE_CODICE", Comparatori.Uguale, codiceOperatore, DataTypes.Stringa)
                End If

            End With

        End Sub

#End Region

    End Class

End Namespace
