Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager

Namespace DAL

    Public Class DbCategorieRischioProvider
        Inherits DbProvider
        Implements ICategorieRischioProvider

#Region " Constructors "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " Public "

        ''' <summary>
        ''' Restituisce un datatable composto da codice e descrizione di tutte le categorie rischio, ordinate per descrizione.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LoadDataTableCategorieRischio() As System.Data.DataTable Implements ICategorieRischioProvider.LoadDataTableCategorieRischio

            Me.RefurbishDT()

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("RSC_CODICE, RSC_DESCRIZIONE")
                .AddTables("T_ANA_RISCHIO")
                .AddOrderByFields("RSC_DESCRIZIONE")
            End With

            _DAM.BuildDataTable(_DT)

            Return _DT.Copy()

        End Function

		''' <summary>
		''' Restituisce la descrizione della categoria di rischio in base al codice 
		''' </summary>
		''' <param name="codiceCategoriaRischio"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function GetDescrizioneCategoriaRischio(codiceCategoriaRischio As String) As String Implements ICategorieRischioProvider.GetDescrizioneCategoriaRischio

			Dim descrizione As String = String.Empty

			With _DAM.QB
				.NewQuery()
				.AddSelectFields("RSC_DESCRIZIONE")
				.AddTables("T_ANA_RISCHIO")
				.AddWhereCondition("RSC_CODICE", Comparatori.Uguale, codiceCategoriaRischio, DataTypes.Stringa)
			End With

			Using idr As IDataReader = _DAM.BuildDataReader()
				If Not idr Is Nothing Then
					'--
					Dim rsc_descrizione As Integer = idr.GetOrdinal("RSC_DESCRIZIONE")
					'--
					If idr.Read() Then
						descrizione = idr.GetStringOrDefault(rsc_descrizione)
					End If
					'--
				End If
			End Using

			Return descrizione

		End Function
		Public Function GetCodCategoriaRischioCodiceACN(codiceEsterno As String) As String Implements ICategorieRischioProvider.GetCodCategoriaRischioCodiceACN

			Dim codice As String = String.Empty

			With _DAM.QB
				.NewQuery()
				.AddSelectFields("RSC_CODICE")
				.AddTables("T_ANA_RISCHIO")
				.AddWhereCondition("RSC_CODICE_ACN", Comparatori.Uguale, codiceEsterno, DataTypes.Stringa)
			End With

			Using idr As IDataReader = _DAM.BuildDataReader()
				If Not idr Is Nothing Then
					'--
					Dim rsc_codice As Integer = idr.GetOrdinal("RSC_CODICE")
					'--
					If idr.Read() Then
						codice = idr.GetStringOrDefault(rsc_codice)
					End If
					'--
				End If
			End Using

			Return codice

		End Function

        ''' <summary>
        ''' Restituisce (codice rischio, descrizione rischio, codice vaccinazione) in base alla categoria di rischio associata al paziente specificato.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCondizioniRischioPaziente(codicePaziente As Integer) As List(Of CondizioneRischio) Implements ICategorieRischioProvider.GetCondizioniRischioPaziente

            Dim list As New List(Of CondizioneRischio)()

            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                cmd.CommandText =
                    "select PAZ_RSC_CODICE, RSC_DESCRIZIONE, LRV_VAC_CODICE " +
                    "from T_PAZ_PAZIENTI " +
                    "join T_ANA_LINK_RISCHIO_VAC on PAZ_RSC_CODICE = LRV_RSC_CODICE " +
                    "join T_ANA_RISCHIO on PAZ_RSC_CODICE = RSC_CODICE " +
                    "where paz_codice = :paz_codice "

                cmd.Parameters.AddWithValue("paz_codice", codicePaziente)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim paz_rsc_codice As Integer = idr.GetOrdinal("PAZ_RSC_CODICE")
                            Dim rsc_descrizione As Integer = idr.GetOrdinal("RSC_DESCRIZIONE")
                            Dim lrv_vac_codice As Integer = idr.GetOrdinal("LRV_VAC_CODICE")

                            While idr.Read()

                                Dim item As New Entities.CondizioneRischio()
                                item.CodiceCategoriaRischio = idr.GetString(paz_rsc_codice)
                                item.DescrizioneCategoriaRischio = idr.GetString(rsc_descrizione)
                                item.CodiceVaccinazione = idr.GetString(lrv_vac_codice)

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

        ''' <summary>
        ''' Restituisce le condizioni di rischio del paziente per la vaccinazione specificata
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="codiceVaccinazione"></param>
        ''' <remarks>Utilizza la vista V_CONDIZIONI_RISCHIO</remarks>
        ''' <returns></returns>
        Public Function GetCondizioniRischio(codicePaziente As Long, codiceVaccinazione As String) As List(Of PazienteCondizioneRischio) Implements ICategorieRischioProvider.GetCondizioniRischio

            Dim list As List(Of PazienteCondizioneRischio) = Nothing

            Using cmd As New OracleClient.OracleCommand("select * from V_CONDIZIONI_RISCHIO where VCR_PAZ_CODICE = :paz and VCR_VAC_CODICE = :vac", Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("paz", codicePaziente)
                    cmd.Parameters.AddWithValue("vac", codiceVaccinazione)

                    list = GetListCondizioniRischio(cmd)

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list

        End Function

        ''' <summary>
        ''' Restituisce le condizioni di rischio per tutte le vaccinazioni dell'associazione specificata
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="codiceVaccinazione"></param>
        ''' <returns></returns>
        Public Function GetCondizioniRischioByAssociazione(codicePaziente As Long, codiceAssociazione As String) As List(Of PazienteCondizioneRischio) Implements ICategorieRischioProvider.GetCondizioniRischioByAssociazione

            Dim list As List(Of PazienteCondizioneRischio) = Nothing

            Dim query As String =
                "SELECT V_CONDIZIONI_RISCHIO.* " +
                "FROM T_ANA_LINK_ASS_VACCINAZIONI " +
                "JOIN V_CONDIZIONI_RISCHIO ON VAL_VAC_CODICE = VCR_VAC_CODICE " +
                "WHERE VAL_ASS_CODICE = :ass " +
                "AND VCR_PAZ_CODICE = :paz "

            Using cmd As New OracleClient.OracleCommand(query, Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("paz", codicePaziente)
                    cmd.Parameters.AddWithValue("ass", codiceAssociazione)

                    list = GetListCondizioniRischio(cmd)

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list

        End Function

        Private Function GetListCondizioniRischio(cmd As OracleClient.OracleCommand) As List(Of PazienteCondizioneRischio)

            Dim list As New List(Of PazienteCondizioneRischio)()

            Using idr As IDataReader = cmd.ExecuteReader()
                If Not idr Is Nothing Then

                    Dim vcr_codice_rischio As Integer = idr.GetOrdinal("VCR_CODICE_RISCHIO")
                    Dim vcr_descrizione_rischio As Integer = idr.GetOrdinal("VCR_DESCRIZIONE_RISCHIO")
                    Dim vcr_vac_codice As Integer = idr.GetOrdinal("VCR_VAC_CODICE")
                    Dim vcr_paz_codice As Integer = idr.GetOrdinal("VCR_PAZ_CODICE")
                    Dim vcr_paz_rsc_codice As Integer = idr.GetOrdinal("VCR_PAZ_RSC_CODICE")
                    Dim vcr_rischio_default As Integer = idr.GetOrdinal("VCR_RISCHIO_DEFAULT")

                    While idr.Read()

                        Dim item As New PazienteCondizioneRischio()

                        item.CodicePaziente = idr.GetInt64OrDefault(vcr_paz_codice)
                        item.CodiceRischio = idr.GetStringOrDefault(vcr_codice_rischio)
                        item.CodiceVaccinazione = idr.GetStringOrDefault(vcr_vac_codice)
                        item.DescrizioneRischio = idr.GetStringOrDefault(vcr_descrizione_rischio)
                        item.FlagCondizioneRischioDefault = idr.GetStringOrDefault(vcr_rischio_default)
                        item.FlagRischioPaziente = idr.GetStringOrDefault(vcr_paz_rsc_codice)

                        list.Add(item)

                    End While

                End If
            End Using

            Return list

        End Function

        ''' <summary>
        ''' Restituisce una lista di coppie codice-descrizione per le categorie di rischio specificate.
        ''' </summary>
        ''' <param name="codiciCategorieRischio"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCodiceDescrizioneCategorieRischio(codiciCategorieRischio As List(Of String)) As List(Of KeyValuePair(Of String, String)) Implements ICategorieRischioProvider.GetCodiceDescrizioneCategorieRischio

            Dim list As New List(Of KeyValuePair(Of String, String))()

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim filter As New System.Text.StringBuilder()

                    If Not codiciCategorieRischio Is Nothing AndAlso codiciCategorieRischio.Count > 0 Then

                        If codiciCategorieRischio.Count = 1 Then

                            filter.Append(" = :rsc_codice ")
                            cmd.Parameters.AddWithValue("rsc_codice", codiciCategorieRischio.First())

                        Else

                            Dim filtroInResult As GetInFilterResult = Me.GetInFilter(codiciCategorieRischio)

                            filter.AppendFormat(" in ({0}) ", filtroInResult.InFilter)
                            cmd.Parameters.AddRange(filtroInResult.Parameters)

                        End If

                    End If

                    cmd.CommandText = String.Format("select RSC_CODICE, RSC_DESCRIZIONE from T_ANA_RISCHIO where RSC_CODICE {0}", filter.ToString())

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim rsc_codice As Integer = idr.GetOrdinal("RSC_CODICE")
                            Dim rsc_descrizione As Integer = idr.GetOrdinal("RSC_DESCRIZIONE")

                            While idr.Read()
                                list.Add(New KeyValuePair(Of String, String)(idr.GetString(rsc_codice), idr.GetString(rsc_descrizione)))
                            End While

                        End If

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list

        End Function

        ''' <summary>
        ''' Restituisce codice e descrizione della categoria di rischio del paziente
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        Public Function GetCategoriaRischioPaziente(codicePaziente As Long) As CondizioneRischioCodiceDescrizione Implements ICategorieRischioProvider.GetCategoriaRischioPaziente

            Dim item As CondizioneRischioCodiceDescrizione = Nothing

            Dim query As String = "select paz_rsc_codice, rsc_descrizione from t_paz_pazienti join t_ana_rischio on paz_rsc_codice = rsc_codice where paz_codice = :paz_codice"

            Using cmd As New OracleClient.OracleCommand(query, Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("paz_codice", codicePaziente)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim paz_rsc_codice As Integer = idr.GetOrdinal("paz_rsc_codice")
                            Dim rsc_descrizione As Integer = idr.GetOrdinal("rsc_descrizione")

                            If idr.Read() Then

                                item = New CondizioneRischioCodiceDescrizione()

                                item.Codice = idr.GetStringOrDefault(paz_rsc_codice)
                                item.Descrizione = idr.GetStringOrDefault(rsc_descrizione)

                            End If

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return item

        End Function

#End Region

    End Class

End Namespace