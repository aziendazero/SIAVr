Imports Onit.Database.DataAccessManager


Namespace DAL.Oracle

    Public Class DbAssociabilitaVaccinazioniProvider
        Inherits DbProvider
        Implements IAssociabilitaVaccinazioniProvider

#Region " Costruttori "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " Public "

        ''' <summary>
        ''' Restituisce True se le due vaccinazioni sono associabili (cioè se non sono presenti nella tabella anagrafica delle non associabilità). Altrimenti, restituisce False.
        ''' </summary>
        Public Function VaccinazioniAssociabili(codiceVaccinazione1 As String, codiceVaccinazione2 As String) As Boolean Implements IAssociabilitaVaccinazioniProvider.VaccinazioniAssociabili

            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim strQuery As String = Queries.AssociabilitaVaccinazioni.OracleQueries.chkVacAssociabili

            Dim ownConnection As Boolean = False

            Dim associabili As Boolean = True

            Try
                cmd = New OracleClient.OracleCommand(strQuery, Me.Connection)

                cmd.Parameters.AddWithValue("cod_vac1", codiceVaccinazione1)
                cmd.Parameters.AddWithValue("cod_vac2", codiceVaccinazione2)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Dim obj As Object = cmd.ExecuteScalar()

                If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                    associabili = False
                Else
                    associabili = True
                End If

            Catch ex As Exception

                Throw New Exception("Errore lettura vaccinazioni non associabili", ex)

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return associabili

        End Function

#End Region

    End Class

End Namespace
