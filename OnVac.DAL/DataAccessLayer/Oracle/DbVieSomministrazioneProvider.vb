Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager

Namespace DAL.Oracle

	Public Class DbVieSomministrazioneProvider
		Inherits DbProvider
		Implements IVieSomministrazioneProvider

#Region " Constructors "

		Public Sub New(DAM As IDAM)

			MyBase.New(DAM)

		End Sub

#End Region

#Region " Public "

		''' <summary>
		''' Restituisce le vie di somministrazione dall'anagrafe
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function GetVieSomministrazione() As List(Of ViaSomministrazione) Implements IVieSomministrazioneProvider.GetVieSomministrazione

			Dim list As New List(Of ViaSomministrazione)()

			Using cmd As New OracleClient.OracleCommand("SELECT * FROM T_ANA_VIE_SOMMINISTRAZIONE ORDER BY VII_DESCRIZIONE", Me.Connection)

				Dim ownConnection As Boolean = False

				Try
					ownConnection = Me.ConditionalOpenConnection(cmd)

					Using idr As IDataReader = cmd.ExecuteReader()

						If Not idr Is Nothing Then

							Dim codice As Integer = idr.GetOrdinal("VII_CODICE")
							Dim descrizione As Integer = idr.GetOrdinal("VII_DESCRIZIONE")
							Dim codice_esterno As Integer = idr.GetOrdinal("VII_CODICE_ESTERNO")

							While idr.Read()

								Dim item As New ViaSomministrazione()
								item.Codice = idr.GetString(codice)
								item.Descrizione = idr.GetStringOrDefault(descrizione)
								item.CodiceEsterno = idr.GetStringOrDefault(codice_esterno)

								list.Add(item)

							End While

						End If
					End Using

				Finally
					Me.ConditionalCloseConnection(ownConnection)
				End Try

			End Using

			Return list

		End Function



        Public Function GetCodiceViaSomministrazioneByCodiceACN(codiceACN As String) As String Implements IVieSomministrazioneProvider.GetCodiceViaSomministrazioneByCodiceACN

            Dim codice As String = String.Empty

            Using cmd As New OracleClient.OracleCommand("select VII_CODICE from T_ANA_VIE_SOMMINISTRAZIONE where VII_CODICE_ACN = :VII_CODICE_ACN", Connection)

                cmd.Parameters.AddWithValue("VII_CODICE_ACN", codiceACN)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj Is Nothing OrElse obj Is DBNull.Value Then
                        codice = String.Empty
                    Else
                        codice = obj.ToString()
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return codice

        End Function


#End Region

    End Class

End Namespace