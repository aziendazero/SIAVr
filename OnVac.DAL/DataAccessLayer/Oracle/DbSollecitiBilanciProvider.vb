Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.Collection

Namespace DAL.Oracle

    Public Class DbSollecitiBilanciProvider
        Implements ISollecitiBilanciProvider

#Region " Private "

        Private _DAM As IDAM
        Private _DR As IDataReader
        Private _RET As Object

#End Region

#Region " Costruttori "

        Public Sub New(ByRef DAM As IDAM)
            Me._DAM = DAM
        End Sub

#End Region

#Region " Metodi di Select "

        'Public Function GetFromKey(ByVal id As Integer) As IDataReader Implements ISollecitiBilanciProvider.GetFromKey

        '    With _DAM.QB

        '        .NewQuery(True)
        '        .AddSelectFields("*")
        '        .AddTables("T_BIL_SOLLECITI")
        '        .AddWhereCondition("id", Comparatori.Uguale, id, DataTypes.Numero)

        '    End With

        '    Return _DAM.BuildDataReader

        'End Function

        Public Function GetKey(bilId As Integer, dataInvio As Date) As Integer Implements ISollecitiBilanciProvider.GetKey

            With _DAM.QB

                .NewQuery(True)
                .AddSelectFields("id")
                .AddTables("T_BIL_SOLLECITI")
                .AddWhereCondition("bis_bip_id", Comparatori.Uguale, bilId, DataTypes.Numero)
                .AddWhereCondition("bis_data_invio", Comparatori.Uguale, dataInvio, DataTypes.Data)

            End With

            Try
                Return _DAM.ExecScalar()

            Catch ex As Exception
                Return False
            End Try

        End Function

#End Region

#Region " Metodi di Delete "

        Public Function DeleteRecord(keypaz As Integer, mal_codice As String) As Boolean Implements ISollecitiBilanciProvider.DeleteRecord

            With _DAM.QB

                .NewQuery(True)
                .AddTables("T_BIL_PROGRAMMATI")
                .AddSelectFields("ID")
                .AddWhereCondition("bip_mal_codice", Comparatori.Uguale, mal_codice, DataTypes.Stringa)
                .AddWhereCondition("bip_paz_codice", Comparatori.Uguale, keypaz, DataTypes.Numero)
                Dim qIdIn As String = "(" & .GetSelect & ")"

                .NewQuery(False, False)
                .AddTables("T_BIL_SOLLECITI")
                .AddWhereCondition("BIS_BIP_ID", Comparatori.In, qIdIn, DataTypes.Replace)

            End With

            Try
                Return _DAM.ExecNonQuery(ExecQueryType.Delete)

            Catch ex As Exception

                If (_DAM.ExistTra) Then
                    _DAM.Rollback()
                End If

                ex.InternalPreserveStackTrace()
                Throw

            End Try

        End Function

#End Region

#Region " Metodi di Update/Insert "

        Public Function Update(id As Integer, dataInvio As Date) As Boolean Implements ISollecitiBilanciProvider.Update

            With _DAM.QB

                .NewQuery()
                .AddTables("T_BIL_SOLLECITI")

                If dataInvio = New Date Then
                    .AddUpdateField("bis_data_invio", System.DBNull.Value, DataTypes.Data)
                Else
                    .AddUpdateField("bis_data_invio", dataInvio, DataTypes.Data)
                End If

                .AddWhereCondition("id", Comparatori.Uguale, id, DataTypes.Numero)

            End With

            Try

                Return _DAM.ExecNonQuery(ExecQueryType.Update)

            Catch ex As Exception

                If (_DAM.ExistTra) Then
                    _DAM.Rollback()
                End If

                ex.InternalPreserveStackTrace()
                Throw

            End Try

        End Function

        Public Function NewRecord(bilID As Integer, dataInvio As Date) As Boolean Implements ISollecitiBilanciProvider.NewRecord

            With _DAM.QB

                .NewQuery()
                .AddTables("T_BIL_SOLLECITI")
                .AddInsertField("bis_bip_id", bilID, DataTypes.Numero)

                If (dataInvio = Date.MinValue) Then
                    .AddInsertField("bis_data_invio", System.DBNull.Value, DataTypes.Data)
                Else
                    .AddInsertField("bis_data_invio", dataInvio, DataTypes.Data)
                End If

            End With

            ' Tolto il rollback (e di conseguenza anche il try...catch, che non serviva più) 
            ' perchè questo metodo viene usato passando la transaction dall'esterno.
            _DAM.ExecNonQuery(ExecQueryType.Insert)

        End Function

#End Region

    End Class

End Namespace

