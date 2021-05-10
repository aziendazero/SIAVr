Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager

Namespace DAL.Oracle

	Public Class DbSitiInoculazioneProvider
		Inherits DbProvider
		Implements ISitiInoculazioneProvider

#Region " Constructors "

		Public Sub New(DAM As IDAM)

			MyBase.New(DAM)

		End Sub

#End Region

#Region " Public "

		''' <summary>
		''' Restituisce i siti di inoculazione dall'anagrafe
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function GetSitiInoculazione() As List(Of SitoInoculazione) Implements ISitiInoculazioneProvider.GetSitiInoculazione

			Dim list As New List(Of SitoInoculazione)()

			Using cmd As New OracleClient.OracleCommand("SELECT * FROM T_ANA_SITI_INOCULAZIONE ORDER BY SII_DESCRIZIONE", Me.Connection)

				Dim ownConnection As Boolean = False

				Try
					ownConnection = Me.ConditionalOpenConnection(cmd)

					Using idr As IDataReader = cmd.ExecuteReader()

						If Not idr Is Nothing Then

							Dim codice As Integer = idr.GetOrdinal("SII_CODICE")
							Dim descrizione As Integer = idr.GetOrdinal("SII_DESCRIZIONE")
							Dim codice_esterno As Integer = idr.GetOrdinal("SII_CODICE_ESTERNO")

							While idr.Read()

								Dim item As New SitoInoculazione()
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



        Public Function GetCodiceSitoInoculazioneByCodiceACN(codiceACN As String) As String Implements ISitiInoculazioneProvider.GetCodiceSitoInoculazioneByCodiceACN

            Dim codice As String = String.Empty

            Using cmd As New OracleClient.OracleCommand("select SII_CODICE from T_ANA_SITI_INOCULAZIONE where SII_CODICE_ACN = :SII_CODICE_ACN", Connection)

                cmd.Parameters.AddWithValue("SII_CODICE_ACN", codiceACN)

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