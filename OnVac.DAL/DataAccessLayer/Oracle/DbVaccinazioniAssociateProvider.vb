Imports Onit.Database.DataAccessManager
Imports System.Collections.Generic
Imports System.Data.OracleClient

Namespace DAL
    Public Class DbVaccinazioniAssociateProvider
        Inherits DbProvider
        Implements IVaccinazioniAssociateProvider


#Region " Costruttori "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

        ''' <summary>
        ''' Restituisce la lista di vaccinazioni associabili
        ''' </summary>
        Public Function GetVaccinazioniAssociabili() As List(Of VaccinazioneAssociabile) Implements IVaccinazioniAssociateProvider.GetVaccinazioniAssociabili

            Dim lstVaccinazioni As New List(Of Entities.VaccinazioneAssociabile)()

            Using cmd As OracleCommand = Me.Connection.CreateCommand()

                cmd.CommandText = "select vac_codice, vac_descrizione from t_ana_vaccinazioni order by vac_codice"

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim vac_codice As Integer = idr.GetOrdinal("vac_codice")
                            Dim vac_descrizione As Integer = idr.GetOrdinal("vac_descrizione")

                            While idr.Read()

                                Dim item As New VaccinazioneAssociabile()
                                item.CodiceVac = idr.GetStringOrDefault(vac_codice)
                                item.DescrizioneVac = idr.GetStringOrDefault(vac_descrizione)
                                lstVaccinazioni.Add(item)

                            End While

                        End If

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return lstVaccinazioni

        End Function

        ''' <summary>
        ''' Restituisce la lista di vaccinazioni associate alla malattia selezionata
        ''' </summary>
        Public Function GetCodiciVaccinazioniAssociateAMalattia(codiceMalattia As String) As List(Of String) Implements IVaccinazioniAssociateProvider.GetCodiciVaccinazioniAssociateAMalattia

            If String.IsNullOrWhiteSpace(codiceMalattia) Then Throw New ArgumentNullException()

            Dim lstVaccinazioni As New List(Of String)()

            Using cmd As OracleCommand = Me.Connection.CreateCommand()

                cmd.CommandText = "select lmv_vac_codice from t_ana_link_malattie_vac where lmv_mal_codice = :mal_codice"
                cmd.Parameters.AddWithValue("mal_codice", codiceMalattia)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim lmv_vac_codice As Integer = idr.GetOrdinal("lmv_vac_codice")

                            While idr.Read()

                                lstVaccinazioni.Add(idr.GetStringOrDefault(lmv_vac_codice))

                            End While

                        End If

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return lstVaccinazioni

        End Function

        ''' <summary>
        ''' Elimina la lista di vaccinazioni associate alla malattia selezionata
        ''' </summary>
        Public Function DeleteVaccinazioniAssociateAMalattia(codiceMalattia As String) As Integer Implements IVaccinazioniAssociateProvider.DeleteVaccinazioniAssociateAMalattia

            Dim count As Integer = 0

            If String.IsNullOrWhiteSpace(codiceMalattia) Then Throw New ArgumentNullException()

            Using cmd As OracleCommand = Me.Connection.CreateCommand()

                cmd.CommandText = "delete from t_ana_link_malattie_vac where lmv_mal_codice = :mal_codice"
                cmd.Parameters.AddWithValue("mal_codice", codiceMalattia)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Inserisci la lista di vaccinazioni associate alla malattia selezionata
        ''' </summary>
        Public Function InsertVaccinazioniAssociateAMalattia(codiceMalattia As String, codiciVac As List(Of String)) As Integer Implements IVaccinazioniAssociateProvider.InsertVaccinazioniAssociateAMalattia

            Dim count As Integer = 0

            If (String.IsNullOrWhiteSpace(codiceMalattia) OrElse codiciVac Is Nothing) Then Throw New ArgumentNullException()

            Using cmd As OracleCommand = Me.Connection.CreateCommand()

                cmd.CommandText = "insert into t_ana_link_malattie_vac (lmv_mal_codice, lmv_vac_codice) values (:mal_codice, :vac_codice)"

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    For Each codVac As String In codiciVac

                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("mal_codice", codiceMalattia)
                        cmd.Parameters.AddWithValue("vac_codice", codVac)

                        count += cmd.ExecuteNonQuery()

                    Next

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Restituisce la lista di vaccinazioni associate alla categoria rischio selezionata
        ''' </summary>
        Function GetCodiciVaccinazioniAssociateACategoriaRischio(codiceCatRis As String) As List(Of String) Implements IVaccinazioniAssociateProvider.GetCodiciVaccinazioniAssociateACategoriaRischio

            If String.IsNullOrWhiteSpace(codiceCatRis) Then Throw New ArgumentNullException()

            Dim lstVaccinazioni As New List(Of String)()

            Using cmd As OracleCommand = Me.Connection.CreateCommand()

                cmd.CommandText = "select lrv_vac_codice from t_ana_link_rischio_vac where lrv_rsc_codice = :rsc_codice"
                cmd.Parameters.AddWithValue("rsc_codice", codiceCatRis)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim lrv_vac_codice As Integer = idr.GetOrdinal("lrv_vac_codice")

                            While idr.Read()

                                lstVaccinazioni.Add(idr.GetStringOrDefault(lrv_vac_codice))

                            End While

                        End If

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return lstVaccinazioni

        End Function

        ''' <summary>
        ''' Elimina la lista di vaccinazioni associate alla categoria rischio
        ''' </summary>
        Public Function DeleteVaccinazioniAssociateACategoriaRischio(codiceCatRis As String) As Integer Implements IVaccinazioniAssociateProvider.DeleteVaccinazioniAssociateACategoriaRischio

            Dim count As Integer = 0

            If String.IsNullOrWhiteSpace(codiceCatRis) Then Throw New ArgumentNullException()

            Using cmd As OracleCommand = Me.Connection.CreateCommand()

                cmd.CommandText = "delete from t_ana_link_rischio_vac where lrv_rsc_codice = :rsc_codice"
                cmd.Parameters.AddWithValue("rsc_codice", codiceCatRis)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Inserisci la lista di vaccinazioni associate alla categoria rischio
        ''' </summary>
        Public Function InsertVaccinazioniAssociateACategoriaRischio(codiceCatRis As String, codiciVac As List(Of String)) As Integer Implements IVaccinazioniAssociateProvider.InsertVaccinazioniAssociateACategoriaRischio

            Dim count As Integer = 0

            If (String.IsNullOrWhiteSpace(codiceCatRis) OrElse codiciVac Is Nothing) Then Throw New ArgumentNullException()

            Using cmd As OracleCommand = Me.Connection.CreateCommand()

                cmd.CommandText = "insert into t_ana_link_rischio_vac (lrv_rsc_codice, lrv_vac_codice) values (:rsc_codice, :vac_codice)"

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    For Each codVac As String In codiciVac

                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("rsc_codice", codiceCatRis)
                        cmd.Parameters.AddWithValue("vac_codice", codVac)

                        count += cmd.ExecuteNonQuery()

                    Next

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

    End Class

End Namespace
