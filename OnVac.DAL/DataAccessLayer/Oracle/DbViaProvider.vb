Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.DAL


Public Class DbViaProvider
    Inherits DbProvider
    Implements IViaProvider

#Region " Costruttori "

    Public Sub New(ByRef DAM As IDAM)

        MyBase.New(DAM)

    End Sub

#End Region

#Region " IViaProvider "

    Public Function GetVieByCap(cap As String) As System.Collections.Generic.IEnumerable(Of Entities.Via) Implements IViaProvider.GetVieByCap

        Using cmd As New OracleClient.OracleCommand(Queries.Vie.OracleQueries.selVieByCap, Me.Connection)

            cmd.Parameters.AddWithValue("via_cap", cap)

            Dim ownConnection As Boolean = False

            Try

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using dataReader As IDataReader = cmd.ExecuteReader()

                    Return Me.GetVieFromReader(dataReader)

                End Using

            Finally

                Me.ConditionalCloseConnection(ownConnection)

            End Try

        End Using

    End Function

    Public Function GetVieByComuneAndCodice(codiceComune As String, codiceVia As String) As System.Collections.Generic.IEnumerable(Of Entities.Via) Implements IViaProvider.GetVieByComuneAndCodice

        Using cmd As New OracleClient.OracleCommand(Queries.Vie.OracleQueries.selVieByComuneAndCodice, Me.Connection)

            cmd.Parameters.AddWithValue("via_com_codice", codiceComune)
            cmd.Parameters.AddWithValue("via_codice", codiceVia)

            Dim ownConnection As Boolean = False

            Try

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using dataReader As IDataReader = cmd.ExecuteReader()

                    Return Me.GetVieFromReader(dataReader)

                End Using

            Finally

                Me.ConditionalCloseConnection(ownConnection)

            End Try

        End Using

    End Function

    Public Function GetVieByDistretto(codiceDistretto As String) As System.Collections.Generic.IEnumerable(Of Entities.Via) Implements IViaProvider.GetVieByDistretto

        Using cmd As New OracleClient.OracleCommand(Queries.Vie.OracleQueries.selVieByDistretto, Me.Connection)

            cmd.Parameters.AddWithValue("via_dis_codice", codiceDistretto)

            Dim ownConnection As Boolean = False

            Try

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using dataReader As IDataReader = cmd.ExecuteReader()

                    Return Me.GetVieFromReader(dataReader)

                End Using

            Finally

                Me.ConditionalCloseConnection(ownConnection)

            End Try

        End Using

    End Function

    ''' <summary>
    ''' Restituisce true se ci sono descrizioni della via associate al codice della via e al comune specificati, diverse dalla descrizione indicata.
    ''' </summary>
    Public Function ExistsDescrizioneDiversa(codiceVia As String, codiceComune As String, descrizioneVia As String) As Boolean Implements IViaProvider.ExistsDescrizioneDiversa

        Using cmd As New OracleClient.OracleCommand(Queries.Vie.OracleQueries.existsDescrizioneDiversa, Me.Connection)

            cmd.Parameters.AddWithValue("via_codice", GetStringParam(codiceVia))
            cmd.Parameters.AddWithValue("via_com_codice", GetStringParam(codiceComune))
            cmd.Parameters.AddWithValue("via_descrizione", GetStringParam(descrizioneVia))

            Dim ownConnection As Boolean = False

            Try

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Dim obj As Object = cmd.ExecuteScalar()

                If obj Is Nothing OrElse obj Is DBNull.Value Then

                    Return False

                End If

            Finally

                Me.ConditionalCloseConnection(ownConnection)

            End Try

        End Using

        Return True

    End Function

    ''' <summary>
    ''' Restituisce true se ci sono descrizioni della via associate al codice della via e al comune specificati, diverse dalla descrizione indicata.
    ''' </summary>
    Public Function ExistsDescrizioneDiversa(codiceVia As String, codiceComune As String, descrizioneVia As String, progressivo As Integer) As Boolean Implements IViaProvider.ExistsDescrizioneDiversa

        Using cmd As New OracleClient.OracleCommand(Queries.Vie.OracleQueries.existsDescrizioneMultipla, Me.Connection)

            cmd.Parameters.AddWithValue("via_codice", GetStringParam(codiceVia))
            cmd.Parameters.AddWithValue("via_com_codice", GetStringParam(codiceComune))
            cmd.Parameters.AddWithValue("via_descrizione", GetStringParam(descrizioneVia))
            cmd.Parameters.AddWithValue("via_progressivo", GetIntParam(progressivo))

            Dim ownConnection As Boolean = False

            Try

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Dim obj As Object = cmd.ExecuteScalar()

                If obj Is Nothing OrElse obj Is DBNull.Value Then

                    Return False

                End If

            Finally

                Me.ConditionalCloseConnection(ownConnection)

            End Try

        End Using

        Return True

    End Function

    ''' <summary>
    ''' Restituisce true se c'è un record con flag default impostato a true, relativo al codice della via e al comune specificati.
    ''' </summary>
    Public Function ExistsDefault(codiceVia As String, codiceComune As String) As Boolean Implements IViaProvider.ExistsDefault

        Using cmd As New OracleClient.OracleCommand(Queries.Vie.OracleQueries.existsDefault, Me.Connection)

            cmd.Parameters.AddWithValue("via_codice", GetStringParam(codiceVia))
            cmd.Parameters.AddWithValue("via_com_codice", GetStringParam(codiceComune))

            Dim ownConnection As Boolean = False

            Try

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Dim obj As Object = cmd.ExecuteScalar()

                If obj Is Nothing OrElse obj Is DBNull.Value Then

                    Return False

                End If

            Finally

                Me.ConditionalCloseConnection(ownConnection)

            End Try

        End Using

        Return True

    End Function

    ''' <summary>
    ''' Restituisce true se ci sono altri record relativi al codice della via e al comune specificati, diversi da quello con il progressivo indicato, 
    ''' con flag default impostato a true.
    ''' </summary>
    Public Function ExistsDefault(codiceVia As String, codiceComune As String, progressivo As Integer) As Boolean Implements IViaProvider.ExistsDefault

        Using cmd As New OracleClient.OracleCommand(Queries.Vie.OracleQueries.existsDefaultMultiplo, Me.Connection)

            cmd.Parameters.AddWithValue("via_codice", GetStringParam(codiceVia))
            cmd.Parameters.AddWithValue("via_com_codice", GetStringParam(codiceComune))
            cmd.Parameters.AddWithValue("via_progressivo", GetIntParam(progressivo))

            Dim ownConnection As Boolean = False

            Try

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Dim obj As Object = cmd.ExecuteScalar()

                If obj Is Nothing OrElse obj Is DBNull.Value Then

                    Return False

                End If

            Finally

                Me.ConditionalCloseConnection(ownConnection)

            End Try

        End Using

        Return True

    End Function

    ''' <summary>
    ''' Restituisce una lista di descrizioni relative al codice via. Il codice via non è univoco, per cui restituisce una lista di descrizioni.
    ''' </summary>
    ''' <param name="codiceVia"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDescrizioniViaByCodice(codiceVia As String) As List(Of String) Implements IViaProvider.GetDescrizioniViaByCodice

        Dim listDescrizioniVie As New List(Of String)()

        Using cmd As New OracleClient.OracleCommand(Queries.Vie.OracleQueries.selDescrizioniVieByCodice, Me.Connection)

            cmd.Parameters.AddWithValue("via_codice", GetStringParam(codiceVia))

            Dim ownConnection As Boolean = False

            Try
                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using idr As IDataReader = cmd.ExecuteReader()

                    If Not idr Is Nothing Then

                        Dim via_descrizione As Integer = idr.GetOrdinal("VIA_DESCRIZIONE")

                        While idr.Read()

                            If Not idr.IsDBNull(via_descrizione) Then
                                listDescrizioniVie.Add(idr.GetString(via_descrizione))
                            End If

                        End While

                    End If

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

        End Using

        Return listDescrizioniVie

    End Function

    ''' <summary>
    ''' Inserimento indirizzo codificato
    ''' </summary>
    ''' <param name="indirizzoPaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function InsertIndirizzoCodificato(indirizzoPaziente As Entities.IndirizzoPaziente) As Integer Implements IViaProvider.InsertIndirizzoCodificato

        Dim codiceIndirizzo As Integer = 0

        Using cmd As New OracleClient.OracleCommand()

            cmd.Connection = Me.Connection

            Dim ownConnection As Boolean = False

            Try
                ownConnection = Me.ConditionalOpenConnection(cmd)

                ' Sequence per il codice
                cmd.CommandText = OnVac.Queries.Vie.OracleQueries.selNextSeqIndirizzi
                codiceIndirizzo = cmd.ExecuteScalar()

                ' Inserimento
                cmd.CommandText = OnVac.Queries.Vie.OracleQueries.insIndirizzoPaziente

                cmd.Parameters.Clear()
                cmd.Parameters.AddWithValue("ind_codice", codiceIndirizzo)
                Me.SetParametersInsertUpdateIndirizzoCodificato(indirizzoPaziente, False, cmd)

                cmd.ExecuteNonQuery()

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

        End Using

        Return codiceIndirizzo

    End Function

    ''' <summary>
    ''' Update dell'indirizzo codificato specificato
    ''' </summary>
    ''' <param name="indirizzoPaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Sub UpdateIndirizzoCodificato(indirizzoPaziente As Entities.IndirizzoPaziente) Implements IViaProvider.UpdateIndirizzoCodificato

        Using cmd As New OracleClient.OracleCommand(OnVac.Queries.Vie.OracleQueries.updIndirizzoPaziente, Me.Connection)

            Dim ownConnection As Boolean = False

            Try
                ownConnection = Me.ConditionalOpenConnection(cmd)

                cmd.Parameters.Clear()
                Me.SetParametersInsertUpdateIndirizzoCodificato(indirizzoPaziente, True, cmd)

                cmd.ExecuteNonQuery()

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

        End Using

    End Sub

    ''' <summary>
    ''' Cancellazione dell'indirizzo codificato specificato
    ''' </summary>
    ''' <param name="codiceIndirizzo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteIndirizzoCodificato(codiceIndirizzo As Integer) As Integer Implements IViaProvider.DeleteIndirizzoCodificato

        Dim count As Integer = 0

        Using cmd As New OracleClient.OracleCommand(Queries.Vie.OracleQueries.delIndirizzoPaziente, Me.Connection)

            Dim ownConnection As Boolean = False

            Try
                ownConnection = Me.ConditionalOpenConnection(cmd)

                cmd.Parameters.AddWithValue("ind_codice", codiceIndirizzo)

                count = cmd.ExecuteNonQuery()

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

        End Using

        Return count

    End Function

    Private Sub SetParametersInsertUpdateIndirizzoCodificato(indirizzoPaziente As Entities.IndirizzoPaziente, setCodeParameter As Boolean, cmd As OracleClient.OracleCommand)

        If setCodeParameter Then cmd.Parameters.AddWithValue("ind_codice", indirizzoPaziente.Codice)

        cmd.Parameters.AddWithValue("ind_via_codice", GetStringParam(indirizzoPaziente.Via.Codice))
        cmd.Parameters.AddWithValue("ind_n_civico", GetStringParam(indirizzoPaziente.NCivico))
        cmd.Parameters.AddWithValue("ind_interno", GetStringParam(indirizzoPaziente.Interno))
        cmd.Parameters.AddWithValue("ind_lotto", GetStringParam(indirizzoPaziente.Lotto))
        cmd.Parameters.AddWithValue("ind_palazzina", GetStringParam(indirizzoPaziente.Palazzina))
        cmd.Parameters.AddWithValue("ind_scala", GetStringParam(indirizzoPaziente.Scala))
        cmd.Parameters.AddWithValue("ind_piano", GetStringParam(indirizzoPaziente.Piano))
        cmd.Parameters.AddWithValue("ind_civico_lettera", GetStringParam(indirizzoPaziente.CivicoLettera))

    End Sub

#End Region

#Region " Private "

    Private Function GetVieFromReader(dataReader As IDataReader) As IEnumerable(Of Entities.Via)

        Dim VIA_PROGRESSIVO As Int32 = dataReader.GetOrdinal("VIA_PROGRESSIVO")
        Dim VIA_CODICE As Int32 = dataReader.GetOrdinal("VIA_CODICE")
        Dim VIA_DESCRIZIONE As Int32 = dataReader.GetOrdinal("VIA_DESCRIZIONE")
        Dim VIA_CIVICO_DA As Int32 = dataReader.GetOrdinal("VIA_CIVICO_DA")
        Dim VIA_CIVICO_A As Int32 = dataReader.GetOrdinal("VIA_CIVICO_A")
        Dim VIA_TIPO_NUMERO As Int32 = dataReader.GetOrdinal("VIA_TIPO_NUMERO")
        Dim VIA_CAP As Int32 = dataReader.GetOrdinal("VIA_CAP")
        Dim VIA_COM_CODICE As Int32 = dataReader.GetOrdinal("VIA_COM_CODICE")
        Dim VIA_DIS_CODICE As Int32 = dataReader.GetOrdinal("VIA_DIS_CODICE")
        Dim VIA_CIRCOSCRIZIONE As Int32 = dataReader.GetOrdinal("VIA_CIRCOSCRIZIONE")
        Dim VIA_DEFAULT As Int32 = dataReader.GetOrdinal("VIA_DEFAULT")

        Dim vie As New List(Of Entities.Via)

        While dataReader.Read()

            Dim via As New Via()

            via.Progressivo = dataReader.GetInt32(VIA_PROGRESSIVO)
            via.Codice = dataReader.GetStringOrDefault(VIA_CODICE)
            via.Descrizione = dataReader.GetStringOrDefault(VIA_DESCRIZIONE)
            via.CivicoDa = dataReader.GetStringOrDefault(VIA_CIVICO_DA)
            via.CivicoA = dataReader.GetStringOrDefault(VIA_CIVICO_A)
            via.TipoNumeroCivico = dataReader.GetStringOrDefault(VIA_TIPO_NUMERO)
            via.Cap = dataReader.GetStringOrDefault(VIA_CAP)
            via.CodiceComune = dataReader.GetStringOrDefault(VIA_COM_CODICE)
            via.CodiceDistetto = dataReader.GetStringOrDefault(VIA_DIS_CODICE)
            via.CodiceCircoscrizione = dataReader.GetStringOrDefault(VIA_CIRCOSCRIZIONE)
            via.Default = (dataReader.GetStringOrDefault(VIA_DEFAULT) = "S")

            vie.Add(via)

        End While

        Return vie.AsEnumerable()

    End Function

#End Region

End Class

