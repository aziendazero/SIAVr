Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager
Imports System.Data.OracleClient


Namespace DAL

    Public Class DbOperatoriProvider
        Inherits DbProvider
        Implements IOperatoriProvider

#Region " Constructors "

        Public Sub New(ByRef dam As IDAM)

            MyBase.New(dam)

        End Sub

#End Region

#Region " Public "

        Public Function GetListOperatori(descrizione As String) As List(Of Operatore) Implements IOperatoriProvider.GetListOperatori

            Return GetListOperatori(descrizione, String.Empty)

        End Function

        Public Function GetListOperatori(descrizione As String, codiceConsultorio As String) As List(Of Operatore) Implements IOperatoriProvider.GetListOperatori

            Dim Ritorno As New List(Of Operatore)()

            Using cmd As OracleCommand = Connection.CreateCommand()

                cmd.CommandText = "select ope_codice, ope_nome, ope_qualifica, ope_codice_esterno, ope_codice_acn, ope_codice_fiscale from t_ana_operatori"

                If Not String.IsNullOrWhiteSpace(codiceConsultorio) Then

                    cmd.CommandText += " join t_ana_link_oper_consultori on loc_ope_codice = ope_codice "

                End If

                Dim filtro As New Text.StringBuilder()

                If Not String.IsNullOrWhiteSpace(codiceConsultorio) Then

                    filtro.Append("loc_cns_codice = :loc_cns_codice and ")
                    cmd.Parameters.AddWithValue("loc_cns_codice", codiceConsultorio)

                End If

                If Not String.IsNullOrWhiteSpace(descrizione) Then

                    filtro.Append("ope_nome like :descrizione and ")
                    cmd.Parameters.AddWithValue("descrizione", String.Format("{0}%", descrizione))

                End If

                If filtro.Length > 0 Then

                    filtro.Insert(0, " where ")
                    filtro.RemoveLast(4)

                    cmd.CommandText += filtro.ToString()

                End If

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim ope_codice As Integer = idr.GetOrdinal("ope_codice")
                            Dim ope_nome As Integer = idr.GetOrdinal("ope_nome")
                            Dim ope_qualifica As Integer = idr.GetOrdinal("ope_qualifica")
                            Dim ope_codice_esterno As Integer = idr.GetOrdinal("ope_codice_esterno")
                            Dim ope_codice_acn As Integer = idr.GetOrdinal("ope_codice_acn")
                            Dim ope_codice_fiscale As Integer = idr.GetOrdinal("ope_codice_fiscale")

                            While idr.Read()

                                Dim Operatore As New Operatore()

                                Operatore.Codice = idr.GetStringOrDefault(ope_codice)
                                Operatore.Nome = idr.GetStringOrDefault(ope_nome)
                                Operatore.Qualifica = idr.GetStringOrDefault(ope_qualifica)
                                Operatore.CodiceEsterno = idr.GetStringOrDefault(ope_codice_esterno)
                                Operatore.CodiceACN = idr.GetStringOrDefault(ope_codice_acn)
                                Operatore.CodiceFiscale = idr.GetStringOrDefault(ope_codice_fiscale)

                                Ritorno.Add(Operatore)

                            End While

                        End If

                    End Using

                    Return Ritorno

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

        End Function

        ''' <summary>
        ''' Carica i dati dell'operatore se il codice operatore corrisponde al codice del medico di base del paziente specificato
        ''' </summary>
        Public Function GetOperatoreMedicoPaziente(codicePaziente As String) As Entities.Operatore Implements IOperatoriProvider.GetOperatoreMedicoPaziente

            Dim operatore As Entities.Operatore = Nothing

            With _DAM.QB

                .NewQuery()
                .AddSelectFields("ope_codice, ope_nome, ope_codice_esterno")
                .AddTables("t_paz_pazienti, t_ana_operatori")
                .AddWhereCondition("paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("paz_med_codice_base", Comparatori.Uguale, "ope_codice", DataTypes.OutJoinLeft)

            End With

            Using idr As IDataReader = _DAM.BuildDataReader()

                If Not idr Is Nothing Then

                    Dim ope_codice As Integer = idr.GetOrdinal("ope_codice")
                    Dim ope_nome As Integer = idr.GetOrdinal("ope_nome")
                    Dim ope_codice_esterno As Integer = idr.GetOrdinal("ope_codice_esterno")

                    If idr.Read() Then

                        operatore = New Entities.Operatore()

                        operatore.Codice = idr.GetStringOrDefault(ope_codice)
                        operatore.Nome = idr.GetStringOrDefault(ope_nome)
                        operatore.CodiceEsterno = idr.GetStringOrDefault(ope_codice_esterno)

                    End If

                End If

            End Using

            Return operatore

        End Function

        Public Function GetListConsultoriOperatori(codiceOper As String) As List(Of Entities.ConsultorioOperatore) Implements IOperatoriProvider.GetListConsultoriOperatori

            Dim listConsultoriOperatore As New List(Of Entities.ConsultorioOperatore)()

            Using cmd As OracleCommand = Me.Connection.CreateCommand()
                Dim query As String = Queries.Operatori.OracleQueries.sel_consultori_operatore
                If Not String.IsNullOrEmpty(codiceOper) Then
                    query = String.Format("{0} {1} ", query, "WHERE LOC_OPE_CODICE = :LOC_OPE_CODICE")
                    cmd.Parameters.AddWithValue("LOC_OPE_CODICE", codiceOper)
                End If
                query = String.Format("{0} {1} ", query, "ORDER BY USL_CODICE, DIS_DESCRIZIONE, CNS_DESCRIZIONE")


                cmd.CommandText = query

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim consultorioOper As Entities.ConsultorioOperatore = Nothing

                            Dim loc_cod_ope As Int16 = idr.GetOrdinal("LOC_OPE_CODICE")
                            Dim cns_codice As Int16 = idr.GetOrdinal("CNS_CODICE")
                            Dim cns_descrizione As Int16 = idr.GetOrdinal("CNS_DESCRIZIONE")
                            Dim dis_codice As Int16 = idr.GetOrdinal("DIS_CODICE")
                            Dim dis_descrizione As Int16 = idr.GetOrdinal("DIS_DESCRIZIONE")
                            Dim dis_usl_codice As Int16 = idr.GetOrdinal("DIS_USL_CODICE")
                            Dim usl_descrizione As Int16 = idr.GetOrdinal("USL_DESCRIZIONE")

                            While idr.Read()

                                consultorioOper = New Entities.ConsultorioOperatore()

                                consultorioOper.CodiceOperatore = idr.GetStringOrDefault(loc_cod_ope)
                                consultorioOper.CodiceConsultorio = idr.GetStringOrDefault(cns_codice)
                                consultorioOper.DescrizioneConsultorio = idr.GetStringOrDefault(cns_descrizione)
                                consultorioOper.Abilitato = True
                                consultorioOper.CodiceDistretto = idr.GetStringOrDefault(dis_codice)
                                consultorioOper.DescrizioneDistretto = idr.GetStringOrDefault(dis_descrizione)
                                consultorioOper.CodiceUsl = idr.GetStringOrDefault(dis_usl_codice)
                                consultorioOper.DescrizioneUsl = idr.GetStringOrDefault(usl_descrizione)

                                listConsultoriOperatore.Add(consultorioOper)

                            End While

                        End If

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listConsultoriOperatore

        End Function
        Public Function InsertConsultorioOperatore(consultorioOperatore As Entities.ConsultorioOperatore) As Integer Implements IOperatoriProvider.InsertConsultorioOperatore

            Dim count As Integer = 0

            Using cmd As New OracleCommand("INSERT INTO T_ANA_LINK_OPER_CONSULTORI (LOC_OPE_CODICE, LOC_CNS_CODICE) VALUES (:LOC_OPE_CODICE, :LOC_CNS_CODICE)", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("LOC_OPE_CODICE", consultorioOperatore.CodiceOperatore)
                    cmd.Parameters.AddWithValue("LOC_CNS_CODICE", consultorioOperatore.CodiceConsultorio)


                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Elimina l'associazione tra l'operatore specificato e i suoi centri
        ''' </summary>
        ''' <param name="CodiceOperatore"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteConsultoriOperatore(codiceOpe As String) As Integer Implements IOperatoriProvider.DeleteConsultoriOperatore

            Dim count As Integer = 0

            Using cmd As New OracleCommand("DELETE FROM T_ANA_LINK_OPER_CONSULTORI WHERE LOC_OPE_CODICE = :LOC_OPE_CODICE", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("LOC_OPE_CODICE", codiceOpe)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Elimina l'associazione tra il consultorio e l'operator specificati
        ''' </summary>
        ''' <param name="codiceConsultorio"></param>
        ''' <param name="codiceOpe"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteConsultorioOperatore(codiceConsultorio As String, codiceOpe As String) As Integer Implements IOperatoriProvider.DeleteConsultorioOperatore

            Dim count As Integer = 0

            Using cmd As New OracleCommand("DELETE FROM T_ANA_LINK_OPER_CONSULTORI WHERE LOC_OPE_CODICE = :LOC_OPE_CODICE AND LOC_CNS_CODICE = :LOC_CNS_CODICE", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("LOC_OPE_CODICE", codiceOpe)
                    cmd.Parameters.AddWithValue("LOC_CNS_CODICE", codiceConsultorio)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function


        Public Function GetOperatoreByCodiceFiscale(codiceFiscale As String) As Operatore Implements IOperatoriProvider.GetOperatoreByCodiceFiscale

            Dim operatore As Operatore = Nothing

            Using cmd As OracleCommand = New OracleCommand("select ope_codice, ope_nome, ope_qualifica, ope_codice_esterno, ope_codice_acn, ope_codice_fiscale from t_ana_operatori where ope_codice_fiscale = :ope_codice_fiscale", Connection)

                cmd.Parameters.AddWithValue("ope_codice_fiscale", GetStringParam(codiceFiscale, False))

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim ope_codice As Integer = idr.GetOrdinal("ope_codice")
                            Dim ope_nome As Integer = idr.GetOrdinal("ope_nome")
                            Dim ope_qualifica As Integer = idr.GetOrdinal("ope_qualifica")
                            Dim ope_codice_esterno As Integer = idr.GetOrdinal("ope_codice_esterno")
                            Dim ope_codice_acn As Integer = idr.GetOrdinal("ope_codice_acn")
                            Dim ope_codice_fiscale As Integer = idr.GetOrdinal("ope_codice_fiscale")

                            If idr.Read() Then

                                operatore = New Operatore()

                                operatore.Codice = idr.GetStringOrDefault(ope_codice)
                                operatore.Nome = idr.GetStringOrDefault(ope_nome)
                                operatore.Qualifica = idr.GetStringOrDefault(ope_qualifica)
                                operatore.CodiceEsterno = idr.GetStringOrDefault(ope_codice_esterno)
                                operatore.CodiceACN = idr.GetStringOrDefault(ope_codice_acn)
                                operatore.CodiceFiscale = idr.GetStringOrDefault(ope_codice_fiscale)

                            End If

                        End If

                    End Using

                Finally

                    ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return operatore

        End Function

        Public Function GetOperatoreById(codice As String) As Operatore Implements IOperatoriProvider.GetOperatoreById

            Dim operatore As Operatore = Nothing

            Using cmd As OracleCommand = New OracleCommand("select ope_codice, ope_nome, ope_qualifica, ope_codice_esterno, ope_codice_acn, ope_codice_fiscale from t_ana_operatori where ope_codice = :ope_codice", Connection)

                cmd.Parameters.AddWithValue("ope_codice", GetStringParam(codice, False))

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim ope_codice As Integer = idr.GetOrdinal("ope_codice")
                            Dim ope_nome As Integer = idr.GetOrdinal("ope_nome")
                            Dim ope_qualifica As Integer = idr.GetOrdinal("ope_qualifica")
                            Dim ope_codice_esterno As Integer = idr.GetOrdinal("ope_codice_esterno")
                            Dim ope_codice_acn As Integer = idr.GetOrdinal("ope_codice_acn")
                            Dim ope_codice_fiscale As Integer = idr.GetOrdinal("ope_codice_fiscale")

                            If idr.Read() Then

                                operatore = New Operatore()

                                operatore.Codice = idr.GetStringOrDefault(ope_codice)
                                operatore.Nome = idr.GetStringOrDefault(ope_nome)
                                operatore.Qualifica = idr.GetStringOrDefault(ope_qualifica)
                                operatore.CodiceEsterno = idr.GetStringOrDefault(ope_codice_esterno)
                                operatore.CodiceACN = idr.GetStringOrDefault(ope_codice_acn)
                                operatore.CodiceFiscale = idr.GetStringOrDefault(ope_codice_fiscale)

                            End If

                        End If

                    End Using

                Finally

                    ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return operatore

        End Function


        Public Function GetOperatoriByIdRSATipoOpe(idRSA As String, Tipo As String, Nome As String) As List(Of Entities.Operatore) Implements IOperatoriProvider.GetOperatoriByIdRSATipoOpe
            Dim result As New List(Of Entities.Operatore)
            Dim s As New System.Text.StringBuilder()

            s.Append("select DISTINCT OPE_CODICE, OPE_NOME, OPE_CODICE_FISCALE ")
            s.Append("from t_ana_operatori ")
            s.Append("join T_ANA_LINK_OPER_CONSULTORI on LOC_OPE_CODICE = OPE_CODICE ")
            s.Append("join V_ANA_RSA ON LOC_CNS_CODICE = RSA_CODICE ")
            s.Append("where RSA_ID = :RSA_ID ")

            If Tipo.ToUpper() = "TUTTO" Then
                    s.Append("AND OPE_QUALIFICA IN ('C','D') ")
                ElseIf Tipo.ToUpper() = "MEDICO" Then
                    s.Append("AND OPE_QUALIFICA = 'C' ")
                Else
                    Return result
                End If
            If String.IsNullOrEmpty(Nome) = False Then
                s.Append("AND OPE_NOME LIKE :OPE_NOME")
            End If

            Dim ownConnection As Boolean = False

            Try
                Using cmd As OracleCommand = New OracleCommand(s.ToString(), Connection)

                    cmd.Parameters.AddWithValue("RSA_ID", idRSA)
                    If String.IsNullOrEmpty(Nome) = False Then
                        cmd.Parameters.AddWithValue("OPE_NOME", "%" + Nome.ToUpper() + "%")
                    End If
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then
                            Dim OPE_CODICE As Integer = idr.GetOrdinal("OPE_CODICE")
                            Dim OPE_NOME As Integer = idr.GetOrdinal("OPE_NOME")
                            Dim OPE_CODICE_FISCALE As Integer = idr.GetOrdinal("OPE_CODICE_FISCALE")

                            While idr.Read()
                                Dim operatore As New Entities.Operatore()

                                operatore.Codice = idr.GetStringOrDefault(OPE_CODICE)
                                operatore.Nome = idr.GetStringOrDefault(OPE_NOME)
                                operatore.CodiceFiscale = idr.GetStringOrDefault(OPE_CODICE_FISCALE)

                                result.Add(operatore)
                            End While
                        End If
                    End Using
                End Using
            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try
            Return result
        End Function
        Public Function GetOperatoriByIdRSATipoOpeDISTINCT(Tipo As String, Nome As String, RsaId As String) As List(Of Entities.Operatore) Implements IOperatoriProvider.GetOperatoriByIdRSATipoOpeDISTINCT
            Dim result As New List(Of Entities.Operatore)
            Dim s As New System.Text.StringBuilder()

            s.Append("select DISTINCT OPE_CODICE, OPE_NOME, OPE_CODICE_FISCALE ")
            s.Append("FROM T_ANA_OPERATORI ")
            s.Append("INNER JOIN T_ANA_LINK_OPER_CONSULTORI ON LOC_OPE_CODICE = OPE_CODICE ")
            s.Append("INNER JOIN t_ana_consultori ON LOC_CNS_CODICE = CNS_CODICE ")
            s.Append("inner join t_ana_distretti on cns_dis_codice = dis_codice ")
            s.Append("where dis_usl_codice = (select RSA_CODICE_ASL from V_ANA_RSA where RSA_ID = :RSA_ID)")
            If Tipo.ToUpper() = "TUTTO" Then
                s.Append("AND OPE_QUALIFICA IN ('C','D') ")
            ElseIf Tipo.ToUpper() = "MEDICO" Then
                s.Append("AND OPE_QUALIFICA = 'C' ")
            Else
                Return result
            End If
            If String.IsNullOrEmpty(Nome) = False Then
                s.Append("AND OPE_NOME LIKE :OPE_NOME")
            End If

            Dim ownConnection As Boolean = False

            Try
                Using cmd As OracleCommand = New OracleCommand(s.ToString(), Connection)

                    cmd.Parameters.AddWithValue("RSA_ID", RsaId)
                    If String.IsNullOrEmpty(Nome) = False Then
                        cmd.Parameters.AddWithValue("OPE_NOME", "%" + Nome.ToUpper() + "%")
                    End If
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then
                            Dim OPE_CODICE As Integer = idr.GetOrdinal("OPE_CODICE")
                            Dim OPE_NOME As Integer = idr.GetOrdinal("OPE_NOME")
                            Dim OPE_CODICE_FISCALE As Integer = idr.GetOrdinal("OPE_CODICE_FISCALE")

                            While idr.Read()
                                Dim operatore As New Entities.Operatore()

                                operatore.Codice = idr.GetStringOrDefault(OPE_CODICE)
                                operatore.Nome = idr.GetStringOrDefault(OPE_NOME)
                                operatore.CodiceFiscale = idr.GetStringOrDefault(OPE_CODICE_FISCALE)

                                result.Add(operatore)
                            End While
                        End If
                    End Using
                End Using
            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try
            Return result
        End Function
        Public Function InsertOperatore(operatore As Entities.Operatore) As Integer Implements IOperatoriProvider.InsertOperatore

            Dim count As Integer = 0

            Dim query As String =
                " INSERT INTO T_ANA_OPERATORI " +
                " (OPE_CODICE, OPE_NOME, OPE_QUALIFICA, OPE_CODICE_ESTERNO, OPE_CODICE_FISCALE, OPE_CODICE_ACN) " +
                " VALUES " +
                " (:OPE_CODICE, :OPE_NOME, :OPE_QUALIFICA, :OPE_CODICE_ESTERNO, :OPE_CODICE_FISCALE, :OPE_CODICE_ACN) "

            Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                cmd.Parameters.AddWithValue("OPE_CODICE", operatore.Codice)
                cmd.Parameters.AddWithValue("OPE_NOME", operatore.Nome)
                cmd.Parameters.AddWithValue("OPE_QUALIFICA", operatore.Qualifica)
                cmd.Parameters.AddWithValue("OPE_CODICE_ESTERNO", operatore.CodiceEsterno)
                cmd.Parameters.AddWithValue("OPE_CODICE_FISCALE", operatore.CodiceFiscale)
                cmd.Parameters.AddWithValue("OPE_CODICE_ACN", operatore.CodiceACN)

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


#Region " Flussi ACN "

        Public Function GetOperatoreByIdACN(IdACN As String) As Operatore Implements IOperatoriProvider.GetOperatoreByIdACN

            Dim operatore As Operatore = Nothing

            Using cmd As OracleCommand = New OracleCommand("select ope_codice, ope_nome, ope_qualifica, ope_codice_esterno, ope_codice_acn, ope_codice_fiscale from t_ana_operatori where ope_codice_acn = :ope_codice_acn", Connection)

                cmd.Parameters.AddWithValue("ope_codice_acn", GetStringParam(IdACN, False))

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim ope_codice As Integer = idr.GetOrdinal("ope_codice")
                            Dim ope_nome As Integer = idr.GetOrdinal("ope_nome")
                            Dim ope_qualifica As Integer = idr.GetOrdinal("ope_qualifica")
                            Dim ope_codice_esterno As Integer = idr.GetOrdinal("ope_codice_esterno")
                            Dim ope_codice_acn As Integer = idr.GetOrdinal("ope_codice_acn")
                            Dim ope_codice_fiscale As Integer = idr.GetOrdinal("ope_codice_fiscale")

                            If idr.Read() Then

                                operatore = New Operatore()

                                operatore.Codice = idr.GetStringOrDefault(ope_codice)
                                operatore.Nome = idr.GetStringOrDefault(ope_nome)
                                operatore.Qualifica = idr.GetStringOrDefault(ope_qualifica)
                                operatore.CodiceEsterno = idr.GetStringOrDefault(ope_codice_esterno)
                                operatore.CodiceACN = idr.GetStringOrDefault(ope_codice_acn)
                                operatore.CodiceFiscale = idr.GetStringOrDefault(ope_codice_fiscale)

                            End If

                        End If

                    End Using

                Finally

                    ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return operatore

        End Function


        ''' <summary>
        ''' Update del solo campo id ACN dell'operatore
        ''' </summary>
        ''' <param name="codiceOperatore"></param>
        ''' <param name="idACN"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateIdACNOperatore(codiceOperatore As String, idACN As String) As Integer Implements IOperatoriProvider.UpdateIdACNOperatore

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand("UPDATE T_ANA_OPERATORI SET OPE_CODICE_ACN = :idACN WHERE OPE_CODICE = :codiceOperatore", Me.Connection)

                cmd.Parameters.AddWithValue("idACN", GetStringParam(idACN, False))
                cmd.Parameters.AddWithValue("codiceOperatore", GetStringParam(codiceOperatore, False))

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

#End Region


#End Region

    End Class

End Namespace