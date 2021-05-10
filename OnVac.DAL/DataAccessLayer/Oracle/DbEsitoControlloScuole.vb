Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.Data.OracleClient


Namespace DAL.Oracle

    Public Class DbEsitoControlloScuoleProvider
        Inherits DbProvider
        Implements IEsitoControlloScuoleProvider

#Region " Costruttore "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " IEsitoControlloScuoleProvider "

#Region " Elaborazione Controlli "

        Public Function CountListaEsitoControlloScuole(filtro As Entities.FiltroEsitoControlloScuole) As Integer Implements IEsitoControlloScuoleProvider.CountListaEsitoControlloScuole

            Dim count As Integer = 0

            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                Dim query As String = GetCountQueryVCaricamentoControlloScuole()
                If Not String.IsNullOrWhiteSpace(filtro.StatoEsito) Then
                    query = query + String.Format(" and ECS_STATO = '{0}'", filtro.StatoEsito)
                End If

                cmd.CommandText = query
                cmd.Parameters.AddWithValue("ECS_PRC_ID_CONTROLLO", filtro.IdControllo)
                cmd.Parameters.AddWithValue("ECS_CODICE_CARICAMENTO", GetGuidParam(filtro.CodiceCaricamento))
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
        ''' <summary>
        ''' Restituisce l'anagrafica del bilancio specificato.
        ''' </summary>
        ''' <param name="numeroBilancio"></param>
        ''' <param name="codiceMalattia"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetListaEsitoControlloScuole(filtro As Entities.FiltroEsitoControlloScuole, campoOrdinamento As String, versoOrdinamento As String, pagingOptions As Data.PagingOptions) As List(Of Entities.EsitoControlloScuole) Implements IEsitoControlloScuoleProvider.GetListaEsitoControlloScuole

            Dim item As List(Of Entities.EsitoControlloScuole) = Nothing

            Dim ownConnection As Boolean = False

            Try
                ' N.B. : la join con la t_ana_link_malattie_tipologia è per compatibilità con il metodo che veniva usato prima

                Dim query As String = GetSelectQueryVCaricamentoControlloScuole()
                If Not String.IsNullOrWhiteSpace(filtro.StatoEsito) Then
                    query = query + String.Format(" and ECS_STATO = :ECS_STATO")
                End If

                query = query + " order by ECS_COGNOME, ECS_NOME, ECS_DATA_NASCITA"


                Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)
                    cmd.Parameters.AddWithValue("ECS_PRC_ID_CONTROLLO", filtro.IdControllo)
                    cmd.Parameters.AddWithValue("ECS_CODICE_CARICAMENTO", GetGuidParam(filtro.CodiceCaricamento))
                    If Not String.IsNullOrWhiteSpace(filtro.StatoEsito) Then
                        cmd.Parameters.AddWithValue("ECS_STATO", filtro.StatoEsito)
                    End If

                    ' Paginazione
                    If Not pagingOptions Is Nothing Then
                        cmd.AddPaginatedQuery(pagingOptions)
                    End If

                    item = GetListElabControlli(cmd)



                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return item

        End Function

        Private Function GetSelectQueryVCaricamentoControlloScuole() As String

            Return "select * from V_ESITO_CONTROLLO_SCUOLE where ECS_PRC_ID_CONTROLLO = :ECS_PRC_ID_CONTROLLO and ECS_CODICE_CARICAMENTO = :ECS_CODICE_CARICAMENTO "


        End Function

        Private Function GetCountQueryVCaricamentoControlloScuole() As String

            Return "select Count(ECS_PRC_ID_CONTROLLO) from V_ESITO_CONTROLLO_SCUOLE where ECS_PRC_ID_CONTROLLO = :ECS_PRC_ID_CONTROLLO and ECS_CODICE_CARICAMENTO = :ECS_CODICE_CARICAMENTO  "


        End Function

        Private Function GetListElabControlli(cmd As OracleClient.OracleCommand) As List(Of Entities.EsitoControlloScuole)

            Dim list As New List(Of Entities.EsitoControlloScuole)()

            Using idr As IDataReader = cmd.ExecuteReader()
                If Not idr Is Nothing Then

                    Dim ecs_prc_id_controllo As Integer = idr.GetOrdinal("ECS_PRC_ID_CONTROLLO")
                    Dim ecs_codice_caricamento As Integer = idr.GetOrdinal("ECS_CODICE_CARICAMENTO")
                    Dim ecs_id As Integer = idr.GetOrdinal("ECS_ID")
                    Dim ecs_paz_codice As Integer = idr.GetOrdinal("ECS_PAZ_CODICE")
                    Dim ecs_cognome As Integer = idr.GetOrdinal("ECS_COGNOME")
                    Dim ecs_nome As Integer = idr.GetOrdinal("ECS_NOME")
                    Dim ecs_data_nascita As Integer = idr.GetOrdinal("ECS_DATA_NASCITA")
                    Dim ecs_sesso As Integer = idr.GetOrdinal("ECS_SESSO")
                    Dim ecs_codice_fiscale As Integer = idr.GetOrdinal("ECS_CODICE_FISCALE")
                    Dim ecs_comune_scuola As Integer = idr.GetOrdinal("ECS_COMUNE_SCUOLA")
                    Dim ecs_nome_scuola As Integer = idr.GetOrdinal("ECS_NOME_SCUOLA")
                    Dim ecs_indirizzo_scuola As Integer = idr.GetOrdinal("ECS_INDIRIZZO_SCUOLA")
                    Dim ecs_coc_id As Integer = idr.GetOrdinal("ECS_COC_ID")
                    Dim ecs_esito_controllo As Integer = idr.GetOrdinal("ECS_ESITO_CONTROLLO")
                    Dim ecs_idoneo As Integer = idr.GetOrdinal("ECS_IDONEO")
                    Dim ecs_vaccinato As Integer = idr.GetOrdinal("ECS_VACCINATO")
                    Dim ecs_data_controllo As Integer = idr.GetOrdinal("ECS_DATA_CONTROLLO")
                    Dim ecs_errore As Integer = idr.GetOrdinal("ECS_ERRORE")
                    Dim ecs_stato As Integer = idr.GetOrdinal("ECS_STATO")

                    While idr.Read()

                        Dim item As New Entities.EsitoControlloScuole

                        item.IdControllo = idr.GetStringOrDefault(ecs_prc_id_controllo)
                        item.CodiceCaricamento = idr.GetGuidOrDefault(ecs_codice_caricamento)
                        item.IdEsito = idr.GetNullableInt64OrDefault(ecs_id)
                        item.PazCodice = idr.GetNullableInt64OrDefault(ecs_paz_codice)
                        item.Cognome = idr.GetStringOrDefault(ecs_cognome)
                        item.Nome = idr.GetStringOrDefault(ecs_nome)
                        item.DataDiNascita = idr.GetNullableDateTimeOrDefault(ecs_data_nascita)
                        item.Sesso = idr.GetStringOrDefault(ecs_sesso)
                        item.CodiceFiscale = idr.GetStringOrDefault(ecs_codice_fiscale)
                        item.ComuneScuola = idr.GetStringOrDefault(ecs_comune_scuola)
                        item.NomeScuola = idr.GetStringOrDefault(ecs_nome_scuola)
                        item.IndirizzoScuola = idr.GetStringOrDefault(ecs_indirizzo_scuola)
                        item.CocId = idr.GetNullableInt64OrDefault(ecs_coc_id)
                        item.EsitoControllo = idr.GetStringOrDefault(ecs_esito_controllo)
                        item.Idoneo = idr.GetStringOrDefault(ecs_idoneo)
                        item.Vacinato = idr.GetStringOrDefault(ecs_vaccinato)
                        item.DataControllo = idr.GetNullableDateTimeOrDefault(ecs_data_controllo)
                        item.Errore = idr.GetStringOrDefault(ecs_errore)
                        item.Stato = idr.GetStringOrDefault(ecs_stato)

                        list.Add(item)

                    End While

                End If
            End Using

            Return list

        End Function

#End Region





#End Region

    End Class

End Namespace
