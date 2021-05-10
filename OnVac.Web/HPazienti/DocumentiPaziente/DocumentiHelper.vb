Imports Onit.Database.DataAccessManager

Public Class DocumentiHelper

    Public Shared Sub SaveDocumento(file As HttpPostedFile, descrizione As String, note As String, codiceUslInserimento As String, flagVisibilita As String)

        Dim buffer(file.ContentLength - 1) As Byte
        Dim dt As New DataTable()

        file.InputStream.Read(buffer, 0, file.ContentLength)

        Dim fileInfo As New System.IO.FileInfo(file.FileName)

        Using dam As IDAM = OnVacUtility.OpenDam()

            dam.QB.NewQuery()

            dam.QB.AddTables("T_PAZ_DOCUMENTI")

            dam.QB.AddInsertField("PDO_PAZ_CODICE", OnVacUtility.Variabili.PazId, DataTypes.Numero)
            dam.QB.AddInsertField("PDO_DATA_ARCHIVIAZIONE", DateTime.Now, DataTypes.DataOra)
            dam.QB.AddInsertField("PDO_UTE_ID_ARCHIVIAZIONE", OnVacContext.UserId, DataTypes.Numero)
            dam.QB.AddInsertField("PDO_DESCRIZIONE", descrizione, DataTypes.Stringa)
            dam.QB.AddInsertField("PDO_NOME", fileInfo.Name, DataTypes.Stringa)
            dam.QB.AddInsertField("PDO_NOTE", note, DataTypes.Stringa)
            dam.QB.AddInsertField("PDO_FILE", buffer, DataTypes.Binary)
            dam.QB.AddInsertField("PDO_USL_INSERIMENTO", codiceUslInserimento, DataTypes.Stringa)
            dam.QB.AddInsertField("PDO_FLAG_VISIBILITA", flagVisibilita, DataTypes.Stringa)

            dam.ExecNonQuery(ExecQueryType.Insert)

        End Using

    End Sub

End Class
