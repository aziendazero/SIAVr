Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager

Imports Onit.OnAssistnet.OnVac.DAL


Public Class DbDistrettoProvider
    Inherits DbProvider
    Implements IDistrettoProvider

#Region " Costruttore "

    Public Sub New(ByRef DAM As IDAM)

        MyBase.New(DAM)

    End Sub

#End Region

#Region " Public "

	Public Function GetCodiceByCodiceEsterno(codiceEsterno As String) As String Implements IDistrettoProvider.GetCodiceByCodiceEsterno

		Dim cmd As OracleClient.OracleCommand = Nothing
		Dim ownConnection As Boolean = False

		Try

			cmd = New OracleClient.OracleCommand(Queries.Distretti.OracleQueries.selCodiceByCodiceEsterno, Me.Connection)

			cmd.Parameters.AddWithValue("dis_codice_esterno", codiceEsterno)

			ownConnection = Me.ConditionalOpenConnection(cmd)

			Return cmd.ExecuteScalar()

		Finally

			Me.ConditionalCloseConnection(ownConnection)

			If Not cmd Is Nothing Then cmd.Dispose()

		End Try

	End Function
	Public Function GetListaDistretto(codice As String, codiceComune As String, codiceUls As String) As List(Of Entities.Distretto) Implements IDistrettoProvider.GetListaDistretto
		Dim list As List(Of Entities.Distretto) = Nothing

		Dim ownConnection As Boolean = False

		Try
			Dim andCodice As String = ""
			If Not codice.IsNullOrEmpty Then
				andCodice = String.Format(" and DIS_CODICE = '{0}' ", codice)
			End If
			Dim andCodiceComune As String = ""
			If Not codiceComune.IsNullOrEmpty Then
				andCodiceComune = String.Format(" and DIS_COM_CODICE = '{0}' ", codiceComune)
			End If
			Dim andCodiceUsl As String = ""
			If Not codiceUls.IsNullOrEmpty Then
				andCodiceUsl = String.Format(" and DIS_USL_CODICE = '{0}' ", codiceUls)
			End If

			Dim query As String = String.Format("SELECT * FROM T_ANA_DISTRETTI where 1=1 {0} {1} {2} ", andCodice, andCodiceComune, andCodiceUsl)

			Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

				ownConnection = Me.ConditionalOpenConnection(cmd)

				list = GetListaDist(cmd)

			End Using

		Finally
			Me.ConditionalCloseConnection(ownConnection)
		End Try

		Return list

	End Function

    Public Function GetListaDistrettoCNSUtente(idutente As Long) As List(Of Entities.DistrettoDDL) Implements IDistrettoProvider.GetListaDistrettoCNSUtente
        Dim list As List(Of Entities.DistrettoDDL) = Nothing

        Dim ownConnection As Boolean = False

        Try


            Dim query As String = String.Format("select distinct DIS_CODICE,DIS_DESCRIZIONE from T_ANA_DISTRETTI JOIN t_ana_consultori on CNS_DIS_CODICE=DIS_CODICE join T_ANA_LINK_UTENTI_CONSULTORI on LUC_CNS_CODICE=CNS_CODICE where LUC_UTE_ID={0} ", idutente)

            Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                list = GetListaDistDDL(cmd)

            End Using

        Finally
            Me.ConditionalCloseConnection(ownConnection)
        End Try

        Return list

    End Function



#End Region
#Region "Private"
    Private Function GetListaDist(cmd As OracleClient.OracleCommand) As List(Of Entities.Distretto)

		Dim list As New List(Of Entities.Distretto)()

		Using idr As IDataReader = cmd.ExecuteReader()
			If Not idr Is Nothing Then

				Dim dis_codice As Integer = idr.GetOrdinal("DIS_CODICE")
				Dim dis_descrizione As Integer = idr.GetOrdinal("DIS_DESCRIZIONE")
				Dim dis_codice_esterno As Integer = idr.GetOrdinal("DIS_CODICE_ESTERNO")
				Dim dis_com_codice As Integer = idr.GetOrdinal("DIS_COM_CODICE")
				Dim dis_usl_codice As Integer = idr.GetOrdinal("DIS_USL_CODICE")


				While idr.Read()

					Dim item As New Entities.Distretto()
					item.Codice = idr.GetString(dis_codice)
					item.Descrizione = idr.GetStringOrDefault(dis_descrizione)
					item.CodiceEsterno = idr.GetStringOrDefault(dis_codice_esterno)
					item.CodiceComune = idr.GetStringOrDefault(dis_com_codice)
					item.CodiceUlss = idr.GetStringOrDefault(dis_usl_codice)


					list.Add(item)

				End While

			End If
		End Using

		Return list

	End Function

    Private Function GetListaDistDDL(cmd As OracleClient.OracleCommand) As List(Of Entities.DistrettoDDL)

        Dim list As New List(Of Entities.DistrettoDDL)()

        Using idr As IDataReader = cmd.ExecuteReader()
            If Not idr Is Nothing Then

                Dim dis_codice As Integer = idr.GetOrdinal("DIS_CODICE")
                Dim dis_descrizione As Integer = idr.GetOrdinal("DIS_DESCRIZIONE")



                While idr.Read()

                    Dim item As New Entities.DistrettoDDL()
                    item.Codice = idr.GetString(dis_codice)
                    item.Descrizione = idr.GetStringOrDefault(dis_descrizione)



                    list.Add(item)

                End While

            End If
        End Using

        Return list

    End Function
#End Region


End Class

