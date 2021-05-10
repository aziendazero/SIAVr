Imports Onit.Database.DataAccessManager


Namespace DAL.Oracle

    Public Class DbMediciProvider
        Inherits DbProvider
        Implements IMediciProvider

#Region " Costruttori "

        Public Sub New(DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " Public "

        ''' <summary>
        ''' Restituisce un oggetto della classe Medico con i dati del medico selezionato in base al codice.
        ''' </summary>
        Public Function GetMedico(ByRef codiceMedico As String) As Entities.Medico Implements IMediciProvider.GetMedico

            Dim medico As Entities.Medico = Nothing

            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim ownConnection As Boolean = False

            Try
                cmd = Me.Connection.CreateCommand()

                cmd.Parameters.AddWithValue("cod_medico", GetStringParam(codiceMedico, False))
                cmd.CommandText = Queries.Medici.OracleQueries.selMedicoByCodice

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using idr As IDataReader = cmd.ExecuteReader()

                    If Not idr Is Nothing Then

                        Dim pos_cod As Integer = idr.GetOrdinal("med_codice")
                        Dim pos_descr As Integer = idr.GetOrdinal("med_descrizione")
                        Dim pos_cognome As Integer = idr.GetOrdinal("med_cognome")
                        Dim pos_nome As Integer = idr.GetOrdinal("med_nome")
                        Dim pos_cod_reg As Integer = idr.GetOrdinal("med_codice_regionale")
                        Dim pos_cod_fisc As Integer = idr.GetOrdinal("med_codice_fiscale")
                        Dim pos_tipo As Integer = idr.GetOrdinal("med_tipo")
                        Dim pos_descr_tipo As Integer = idr.GetOrdinal("tme_descrizione")
                        Dim pos_data_iscr As Integer = idr.GetOrdinal("med_data_iscrizione")
                        Dim pos_data_scad As Integer = idr.GetOrdinal("med_scadenza")

                        If idr.Read() Then

                            medico = New Entities.Medico()

                            medico.Codice = idr(pos_cod).ToString()
                            medico.Descrizione = idr(pos_descr).ToString()
                            medico.Cognome = idr(pos_cognome).ToString()
                            medico.Nome = idr(pos_nome).ToString()
                            medico.CodiceRegionale = idr(pos_cod_reg).ToString()
                            medico.CodiceFiscale = idr(pos_cod_fisc).ToString()
                            medico.Tipo = idr(pos_tipo).ToString()
                            medico.DescrizioneTipo = idr(pos_descr_tipo).ToString()
                            If idr.IsDBNull(pos_data_iscr) Then
                                medico.DataIscrizione = Date.MinValue
                            Else
                                medico.DataIscrizione = idr(pos_data_iscr)
                            End If
                            If idr.IsDBNull(pos_data_scad) Then
                                medico.DataScadenza = Date.MinValue
                            Else
                                medico.DataScadenza = idr(pos_data_scad)
                            End If

                        End If

                    End If

                End Using

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return medico

        End Function

        ''' <summary>
        ''' Restituisce un array contenente i codici dei medici abilitati
        ''' </summary>
        Public Function GetCodiciMediciAbilitati(ByRef codiceMedico As String, data As DateTime) As String() Implements IMediciProvider.GetCodiciMediciAbilitati

            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim idr As IDataReader = Nothing

            Dim codiciMedici As New ArrayList()

            Dim ownConnection As Boolean = False

            Try

                cmd = Me.Connection.CreateCommand

                cmd.Parameters.AddWithValue("cod_medico", GetStringParam(codiceMedico, False))
                cmd.Parameters.AddWithValue("data", GetDateParam(data))
                cmd.CommandText = OnVac.Queries.Medici.OracleQueries.selCodiciMediciAbilitatiByCodiceAndDate

                ownConnection = Me.ConditionalOpenConnection(cmd)

                idr = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    While idr.Read()

                        codiciMedici.Add(idr.GetString(0))

                    End While

                End If

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()
                If Not idr Is Nothing AndAlso Not idr.IsClosed Then idr.Close()

            End Try

            Return codiciMedici.ToArray(GetType(String))

        End Function

        Public Sub InsertMedico(medico As Entities.Medico) Implements IMediciProvider.InsertMedico

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try

                cmd = Me.Connection.CreateCommand()

                cmd.CommandText = OnVac.Queries.Medici.OracleQueries.insMedico

                cmd.Parameters.AddWithValue("med_codice", GetStringParam(medico.Codice, False))
                cmd.Parameters.AddWithValue("med_codice_regionale", GetStringParam(medico.CodiceRegionale, False))
                cmd.Parameters.AddWithValue("med_codice_fiscale", GetStringParam(medico.CodiceFiscale, False))
                cmd.Parameters.AddWithValue("med_nome", GetStringParam(medico.Nome, False))
                cmd.Parameters.AddWithValue("med_cognome", GetStringParam(medico.Cognome, False))
                cmd.Parameters.AddWithValue("med_descrizione", GetStringParam(medico.Descrizione, False))

                ownConnection = Me.ConditionalOpenConnection(cmd)

                cmd.ExecuteNonQuery()

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try


        End Sub

        Public Function ExistsMedico(codiceMedico As String) As Boolean Implements IMediciProvider.ExistsMedico

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try

                cmd = Me.Connection.CreateCommand()

                cmd.CommandText = "SELECT 1 FROM T_ANA_MEDICI WHERE MED_CODICE = :MED_CODICE"

                cmd.Parameters.AddWithValue("MED_CODICE", GetStringParam(codiceMedico, False))

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Return Not cmd.ExecuteScalar() Is Nothing

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

        End Function

        ''' <summary>
        ''' Carica i dati del medico in base al codice fiscale
        ''' </summary>
        Public Function GetMedicoByCodiceFiscale(codiceFiscale As String) As Entities.Medico Implements IMediciProvider.GetMedicoByCodiceFiscale

            Dim medico As Entities.Medico = Nothing

            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim ownConnection As Boolean = False

            Try
                cmd = Me.Connection.CreateCommand()

                cmd.Parameters.AddWithValue("med_codice_fiscale", GetStringParam(codiceFiscale, False))
                cmd.CommandText = Queries.Medici.OracleQueries.selMedicoByCodiceFiscale

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using idr As IDataReader = cmd.ExecuteReader()

                    If Not idr Is Nothing Then

                        Dim pos_cod As Integer = idr.GetOrdinal("med_codice")
                        Dim pos_descr As Integer = idr.GetOrdinal("med_descrizione")
                        Dim pos_cognome As Integer = idr.GetOrdinal("med_cognome")
                        Dim pos_nome As Integer = idr.GetOrdinal("med_nome")
                        Dim pos_cod_reg As Integer = idr.GetOrdinal("med_codice_regionale")
                        Dim pos_cod_fisc As Integer = idr.GetOrdinal("med_codice_fiscale")
                        Dim pos_tipo As Integer = idr.GetOrdinal("med_tipo")
                        Dim pos_data_iscr As Integer = idr.GetOrdinal("med_data_iscrizione")
                        Dim pos_data_scad As Integer = idr.GetOrdinal("med_scadenza")

                        If idr.Read() Then

                            medico = New Entities.Medico()

                            medico.Codice = idr(pos_cod).ToString()
                            medico.Descrizione = idr(pos_descr).ToString()
                            medico.Cognome = idr(pos_cognome).ToString()
                            medico.Nome = idr(pos_nome).ToString()
                            medico.CodiceRegionale = idr(pos_cod_reg).ToString()
                            medico.CodiceFiscale = idr(pos_cod_fisc).ToString()
                            medico.Tipo = idr(pos_tipo).ToString()
                            If idr.IsDBNull(pos_data_iscr) Then
                                medico.DataIscrizione = Date.MinValue
                            Else
                                medico.DataIscrizione = idr(pos_data_iscr)
                            End If
							'If idr.IsDBNull(pos_data_scad) Then
							'    medico.DataScadenza = Date.MinValue
							'Else
							'    medico.DataScadenza = idr(pos_data_scad)
							'End If

						End If

                    End If

                End Using

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return medico

        End Function


#End Region

    End Class

End Namespace