Imports Onit.Database.DataAccessManager
Imports System.Collections.Generic


Namespace DAL.Oracle

    Public Class DbParametriProvider
        Inherits DbProvider
        Implements IParametriProvider

#Region " Costruttori "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " Get Parametri "

        Public Function GetParametriSistema() As List(Of KeyValuePair(Of String, Object)) Implements IParametriProvider.GetParametriSistema

            Return Me.GetParametri(String.Empty, Nothing)

        End Function

        Public Function GetParametriCns(codiceConsultorio As String) As List(Of KeyValuePair(Of String, Object)) Implements IParametriProvider.GetParametriCns

            Return Me.GetParametri(codiceConsultorio, Nothing)

        End Function

        Public Function GetParametriCns(codiceConsultorio As String, listaParametri As List(Of String)) As List(Of KeyValuePair(Of String, Object)) Implements IParametriProvider.GetParametriCns

            Return Me.GetParametri(codiceConsultorio, listaParametri)

        End Function

        Private Function GetParametri(codiceConsultorio As String, listaCodiciParametri As List(Of String)) As List(Of KeyValuePair(Of String, Object))

            Dim parametri As New List(Of KeyValuePair(Of String, Object))()

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                Dim query As String = String.Empty

                If String.IsNullOrEmpty(codiceConsultorio) Then
                    query = OnVac.Queries.Parametri.OracleQueries.selParametriSistema
                Else
                    query = OnVac.Queries.Parametri.OracleQueries.selParametriCns
                    cmd.Parameters.AddWithValue("cod_cns", GetStringParam(codiceConsultorio, False))
                End If

                If Not listaCodiciParametri Is Nothing AndAlso listaCodiciParametri.Count > 0 Then
                    query += String.Format(" and par_codice in ({0}) ", String.Join(",", (From item As String In listaCodiciParametri Select "'" + item + "'").ToArray()))
                End If

                cmd.CommandText = query

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim posCodice As Int16 = idr.GetOrdinal("par_codice")
                            Dim posValore As Int16 = idr.GetOrdinal("par_valore")

                            While idr.Read()

                                If Not idr.IsDBNull(posCodice) Then

                                    Dim item As New KeyValuePair(Of String, Object)(idr.GetString(posCodice), idr(posValore))

                                    parametri.Add(item)

                                End If

                            End While

                        End If

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return parametri

        End Function

#Region " Metodi che restituiscono i parametri in un datatable "

        ' Restituisce un datatable contenente tutti i parametri del consultorio specificato
        Public Function SelectCnsParameters(consultorio As String) As DataTable Implements IParametriProvider.SelectCnsParameters

            Return GetDtParams(consultorio, Nothing)

        End Function

        ' Restituisce un datatable con i valori dei parametri specificati, per il consultorio specificato
        Public Function SelectParametersValue(consultorio As String, ParamArray cod_param_list() As String) As DataTable Implements IParametriProvider.SelectParametersValue

            Return GetDtParams(consultorio, cod_param_list)

        End Function

        ' Restituisce un datatable con i valori di default dei parametri specificati
        Public Function SelectParametersValue(ParamArray cod_param_list() As String) As DataTable Implements IParametriProvider.SelectParametersValue

            Return GetDtParams(Constants.CommonConstants.CodiceConsultorioSistema, cod_param_list)

        End Function

        ' Restituisce un datatable con tutti i parametri generici (par_cns_codice="VAC")
        Public Function SelectGenericParameters() As DataTable Implements IParametriProvider.SelectGenericParameters

            Return GetDtParams(Constants.CommonConstants.CodiceConsultorioSistema, Nothing)

        End Function

        Private Function GetDtParams(consultorio As String, ParamArray cod_param_list() As String) As DataTable

            _DT.Clear()

            setDamQueryBuilder(consultorio, cod_param_list)

            Try

                _DAM.BuildDataTable(_DT)

            Catch ex As Exception

                LogError(ex)
                SetErrorMsg("Errore durante l'accesso ai parametri di configurazione su database.")

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return _DT

        End Function

        Private Sub setDamQueryBuilder(consultorio As String, ParamArray cod_param_list() As String)

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("*")
                .AddTables("T_ANA_PARAMETRI")
                .AddWhereCondition("PAR_CNS_CODICE", Comparatori.Uguale, consultorio, DataTypes.Stringa)

                If Not cod_param_list Is Nothing AndAlso cod_param_list.Length > 0 Then
                    .OpenParanthesis()
                    .AddWhereCondition("PAR_CODICE", Comparatori.Uguale, cod_param_list(0), DataTypes.Stringa)
                    If cod_param_list.Length > 1 Then
                        For i As Integer = 1 To cod_param_list.Length - 1
                            .AddWhereCondition("PAR_CODICE", Comparatori.Uguale, cod_param_list(i), DataTypes.Stringa, "OR")
                        Next
                    End If
                    .CloseParanthesis()
                End If

                .AddOrderByFields("par_codice")

            End With

        End Sub

#End Region

#End Region

#Region " Insert / Update / Delete "

        Public Sub InsertParam(param As OnVac.Entities.Parametro) Implements IParametriProvider.InsertParam

            With _DAM.QB
                .NewQuery()
                .AddTables("t_ana_parametri")
                .AddInsertField("par_codice", param.Codice, DataTypes.Stringa)
                .AddInsertField("par_cns_codice", param.Consultorio, DataTypes.Stringa)
                .AddInsertField("par_valore", param.Valore, DataTypes.Stringa)
                .AddInsertField("par_descrizione", param.Descrizione, DataTypes.Stringa)
                .AddInsertField("par_centrale", IIf(param.Centrale, "S", "N"), DataTypes.Stringa)
            End With

            Try

                _DAM.ExecNonQuery(ExecQueryType.Insert)

            Catch ex As Exception

                Dim stb As New System.Text.StringBuilder()

                stb.AppendFormat("Errore inserimento parametro:{0}", Environment.NewLine)
                stb.AppendFormat("Codice: {0}{1}", param.Codice, Environment.NewLine)
                stb.AppendFormat("Consultorio: {0}{1}", param.Consultorio, Environment.NewLine)
                stb.AppendFormat("Valore: {0}{1}", param.Valore, Environment.NewLine)
                stb.AppendFormat("Eccezione:{0}", Environment.NewLine)

                LogError(ex, stb.ToString())

                SetErrorMsg("Errore durante l'inserimento del parametro su database.")

                ex.InternalPreserveStackTrace()
                Throw

            End Try

        End Sub

        Public Sub UpdateParam(param As OnVac.Entities.Parametro) Implements IParametriProvider.UpdateParam

            With _DAM.QB
                .NewQuery()
                .AddTables("t_ana_parametri")
                .AddUpdateField("par_valore", param.Valore, DataTypes.Stringa)
                .AddWhereCondition("par_codice", Comparatori.Uguale, param.Codice, DataTypes.Stringa)
                .AddWhereCondition("par_cns_codice", Comparatori.Uguale, param.Consultorio, DataTypes.Stringa)
            End With

            Try
                _DAM.ExecNonQuery(ExecQueryType.Update)

            Catch ex As Exception

                Dim stb As New System.Text.StringBuilder()

                stb.AppendFormat("Errore modifica parametro:{0}", Environment.NewLine)
                stb.AppendFormat("Codice: {0}{1}", param.Codice, Environment.NewLine)
                stb.AppendFormat("Consultorio: {0}{1}", param.Consultorio, Environment.NewLine)
                stb.AppendFormat("Valore: {0}{1}", param.Valore, Environment.NewLine)
                stb.AppendFormat("Eccezione:{0}", Environment.NewLine)

                LogError(ex, stb.ToString())

                SetErrorMsg("Errore durante la modifica del parametro su database.")

                ex.InternalPreserveStackTrace()
                Throw

            End Try

        End Sub

        Public Sub DeleteParam(param As OnVac.Entities.Parametro) Implements IParametriProvider.DeleteParam

            With _DAM.QB
                .NewQuery()
                .AddTables("t_ana_parametri")
                .AddWhereCondition("par_codice", Comparatori.Uguale, param.Codice, DataTypes.Stringa)
                .AddWhereCondition("par_cns_codice", Comparatori.Uguale, param.Consultorio, DataTypes.Stringa)
            End With

            Try
                _DAM.ExecNonQuery(ExecQueryType.Delete)

            Catch ex As Exception

                Dim stb As New System.Text.StringBuilder()

                stb.AppendFormat("Errore cancellazione parametro:{0}", Environment.NewLine)
                stb.AppendFormat("Codice: {0}{1}", param.Codice, Environment.NewLine)
                stb.AppendFormat("Consultorio: {0}{1}", param.Consultorio, Environment.NewLine)
                stb.AppendFormat("Eccezione:{0}", Environment.NewLine)

                LogError(ex, stb.ToString())

                SetErrorMsg("Errore durante la cancellazione del parametro su database.")

                ex.InternalPreserveStackTrace()
                Throw

            End Try

        End Sub

#End Region

    End Class

End Namespace