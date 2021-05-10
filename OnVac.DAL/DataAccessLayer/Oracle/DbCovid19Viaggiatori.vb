Imports System.Collections.Generic
Imports System.Data.OracleClient
Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.Data.OracleClient
Imports Onit.OnAssistnet.OnVac.DAL

Public Class DbCovid19Viaggiatori
    Inherits DbProvider
    Implements ICovid19Viaggiatori

#Region " Constructors "
    Public Sub New(ByRef DAM As IDAM)
        MyBase.New(DAM)
    End Sub
#End Region


    Public Function GetViaggiatori() As List(Of Viaggiatore) Implements ICovid19Viaggiatori.GetViaggiatori
        Dim result As New List(Of Viaggiatore)
        Dim ownConnection As Boolean = False
        Dim query As String = "SELECT VGC_NOME, VGC_COGNOME, VGC_CODICE_FISCALE, VGC_EMAIL, VGC_TELEFONO, VGC_PAESE_PROVENIENTE, VGC_DATA_RIENTRO, VGC_DATA, VGC_PROTOCOLLO, VGC_DESCRIZIONE, VGC_STATO_ELABORAZIONE, VGC_DATA_ELABORAZIONE, VGC_NOTE_ELABORAZIONE, VGC_ULSS " +
                        "FROM T_ANA_VIAGGIATORI_COVID " +
                        "WHERE VGC_STATO_ELABORAZIONE = 15"
        Try
            Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using _context As IDataReader = cmd.ExecuteReader()
                    Dim VGC_NOME As Integer = _context.GetOrdinal("VGC_NOME")
                    Dim VGC_COGNOME As Integer = _context.GetOrdinal("VGC_COGNOME")
                    Dim VGC_CODICE_FISCALE As Integer = _context.GetOrdinal("VGC_CODICE_FISCALE")
                    Dim VGC_EMAIL As Integer = _context.GetOrdinal("VGC_EMAIL")
                    Dim VGC_TELEFONO As Integer = _context.GetOrdinal("VGC_TELEFONO")
                    Dim VGC_PAESE_PROVENIENTE As Integer = _context.GetOrdinal("VGC_PAESE_PROVENIENTE")
                    Dim VGC_DATA_RIENTRO As Integer = _context.GetOrdinal("VGC_DATA_RIENTRO")
                    Dim VGC_DATA As Integer = _context.GetOrdinal("VGC_DATA")
                    Dim VGC_PROTOCOLLO As Integer = _context.GetOrdinal("VGC_PROTOCOLLO")
                    Dim VGC_DESCRIZIONE As Integer = _context.GetOrdinal("VGC_DESCRIZIONE")
                    Dim VGC_STATO_ELABORAZIONE As Integer = _context.GetOrdinal("VGC_STATO_ELABORAZIONE")
                    Dim VGC_DATA_ELABORAZIONE As Integer = _context.GetOrdinal("VGC_DATA_ELABORAZIONE")
                    Dim VGC_NOTE_ELABORAZIONE As Integer = _context.GetOrdinal("VGC_NOTE_ELABORAZIONE")
                    Dim VGC_ULSS As Integer = _context.GetOrdinal("VGC_ULSS")
                    While _context.Read()
                        Dim Viaggiatore As New Viaggiatore
                        Viaggiatore.Nome = _context.GetStringOrDefault(VGC_NOME)
                        Viaggiatore.Cognome = _context.GetStringOrDefault(VGC_COGNOME)
                        Viaggiatore.CodiceFiscale = _context.GetStringOrDefault(VGC_CODICE_FISCALE)
                        Viaggiatore.Email = _context.GetStringOrDefault(VGC_EMAIL)
                        Viaggiatore.Telefono = _context.GetStringOrDefault(VGC_TELEFONO)
                        Viaggiatore.PaeseProveniente = _context.GetStringOrDefault(VGC_PAESE_PROVENIENTE)
                        Viaggiatore.DataRientro = _context.GetDateTimeOrDefault(VGC_DATA_RIENTRO)
                        Viaggiatore.Data = _context.GetDateTimeOrDefault(VGC_DATA)
                        Viaggiatore.Protocollo = _context.GetStringOrDefault(VGC_PROTOCOLLO)
                        Viaggiatore.Descrizione = _context.GetStringOrDefault(VGC_DESCRIZIONE)
                        Viaggiatore.StatoElab = _context.GetInt32OrDefault(VGC_STATO_ELABORAZIONE)
                        Viaggiatore.DataElab = _context.GetDateTimeOrDefault(VGC_DATA_ELABORAZIONE)
                        Viaggiatore.NoteElab = _context.GetStringOrDefault(VGC_NOTE_ELABORAZIONE)
                        Viaggiatore.Ulss = _context.GetStringOrDefault(VGC_ULSS)
                        result.Add(Viaggiatore)
                    End While
                End Using


            End Using
        Finally
            ConditionalCloseConnection(ownConnection)
        End Try
        Return result
    End Function

    Public Function SalvaElaborazioneViaggiatore(statoElab As Integer, dataElab As DateTime, noteElaborazione As String, codiceFiscale As String, dataRientro As DateTime, data As DateTime) Implements ICovid19Viaggiatori.SalvaElaborazioneViaggiatore
        Dim ownConnection As Boolean = False
        Dim query As String = "UPDATE T_ANA_VIAGGIATORI_COVID " +
                              "SET VGC_STATO_ELABORAZIONE = :VGC_STATO_ELABORAZIONE, VGC_DATA_ELABORAZIONE = :VGC_DATA_ELABORAZIONE, VGC_NOTE_ELABORAZIONE = :VGC_NOTE_ELABORAZIONE " +
                              "WHERE VGC_CODICE_FISCALE = :VGC_CODICE_FISCALE AND VGC_DATA_RIENTRO = :VGC_DATA_RIENTRO AND VGC_DATA = :VGC_DATA "

        Try
            Using cmd As OracleCommand = New OracleCommand(query, Connection)

                cmd.Parameters.AddWithValueOrDefault("VGC_STATO_ELABORAZIONE", statoElab)
                cmd.Parameters.AddWithValueOrDefault("VGC_DATA_ELABORAZIONE", dataElab)
                cmd.Parameters.AddWithValueOrDefault("VGC_NOTE_ELABORAZIONE", noteElaborazione)
                cmd.Parameters.AddWithValueOrDefault("VGC_CODICE_FISCALE", codiceFiscale)
                cmd.Parameters.AddWithValueOrDefault("VGC_DATA_RIENTRO", dataRientro)
                cmd.Parameters.AddWithValueOrDefault("VGC_DATA", data)

                ownConnection = ConditionalOpenConnection(cmd)
                cmd.ExecuteNonQuery()

            End Using
        Finally
            ConditionalCloseConnection(ownConnection)
        End Try
    End Function


    Public Function GetViaggiatoriExtraEU() As List(Of Viaggiatore) Implements ICovid19Viaggiatori.GetViaggiatoriExtraEU
        Dim result As New List(Of Viaggiatore)
        Dim ownConnection As Boolean = False
        Dim query As String = "SELECT VEU_NOME, VEU_COGNOME, VEU_CODICE_FISCALE, VEU_EMAIL, VEU_TELEFONO, VEU_PAESE_PROVENIENTE, VEU_DATA_RIENTRO, VEU_DATA, VEU_PROTOCOLLO, VEU_DESCRIZIONE, VEU_STATO_ELABORAZIONE, VEU_DATA_ELABORAZIONE, VEU_NOTE_ELABORAZIONE, VEU_ULSS " +
                        "FROM T_ANA_VIAGGIATORI_EXTRA_UE_C19 " +
                        "WHERE VEU_STATO_ELABORAZIONE = 15"
        Try
            Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using _context As IDataReader = cmd.ExecuteReader()
                    Dim VEU_NOME As Integer = _context.GetOrdinal("VEU_NOME")
                    Dim VEU_COGNOME As Integer = _context.GetOrdinal("VEU_COGNOME")
                    Dim VEU_CODICE_FISCALE As Integer = _context.GetOrdinal("VEU_CODICE_FISCALE")
                    Dim VEU_EMAIL As Integer = _context.GetOrdinal("VEU_EMAIL")
                    Dim VEU_TELEFONO As Integer = _context.GetOrdinal("VEU_TELEFONO")
                    Dim VEU_PAESE_PROVENIENTE As Integer = _context.GetOrdinal("VEU_PAESE_PROVENIENTE")
                    Dim VEU_DATA_RIENTRO As Integer = _context.GetOrdinal("VEU_DATA_RIENTRO")
                    Dim VEU_DATA As Integer = _context.GetOrdinal("VEU_DATA")
                    Dim VEU_PROTOCOLLO As Integer = _context.GetOrdinal("VEU_PROTOCOLLO")
                    Dim VEU_DESCRIZIONE As Integer = _context.GetOrdinal("VEU_DESCRIZIONE")
                    Dim VEU_STATO_ELABORAZIONE As Integer = _context.GetOrdinal("VEU_STATO_ELABORAZIONE")
                    Dim VEU_DATA_ELABORAZIONE As Integer = _context.GetOrdinal("VEU_DATA_ELABORAZIONE")
                    Dim VEU_NOTE_ELABORAZIONE As Integer = _context.GetOrdinal("VEU_NOTE_ELABORAZIONE")
                    Dim VEU_ULSS As Integer = _context.GetOrdinal("VEU_ULSS")
                    While _context.Read()
                        Dim Viaggiatore As New Viaggiatore
                        Viaggiatore.Nome = _context.GetStringOrDefault(VEU_NOME)
                        Viaggiatore.Cognome = _context.GetStringOrDefault(VEU_COGNOME)
                        Viaggiatore.CodiceFiscale = _context.GetStringOrDefault(VEU_CODICE_FISCALE)
                        Viaggiatore.Email = _context.GetStringOrDefault(VEU_EMAIL)
                        Viaggiatore.Telefono = _context.GetStringOrDefault(VEU_TELEFONO)
                        Viaggiatore.PaeseProveniente = _context.GetStringOrDefault(VEU_PAESE_PROVENIENTE)
                        Viaggiatore.DataRientro = _context.GetDateTimeOrDefault(VEU_DATA_RIENTRO)
                        Viaggiatore.Data = _context.GetDateTimeOrDefault(VEU_DATA)
                        Viaggiatore.Protocollo = _context.GetStringOrDefault(VEU_PROTOCOLLO)
                        Viaggiatore.Descrizione = _context.GetStringOrDefault(VEU_DESCRIZIONE)
                        Viaggiatore.StatoElab = _context.GetInt32OrDefault(VEU_STATO_ELABORAZIONE)
                        Viaggiatore.DataElab = _context.GetDateTimeOrDefault(VEU_DATA_ELABORAZIONE)
                        Viaggiatore.NoteElab = _context.GetStringOrDefault(VEU_NOTE_ELABORAZIONE)
                        Viaggiatore.Ulss = _context.GetStringOrDefault(VEU_ULSS)
                        result.Add(Viaggiatore)
                    End While
                End Using


            End Using
        Finally
            ConditionalCloseConnection(ownConnection)
        End Try
        Return result
    End Function

    Public Function SalvaElaborazioneViaggiatoreExtraEU(statoElab As Integer, dataElab As DateTime, noteElaborazione As String, codiceFiscale As String, dataRientro As DateTime, data As DateTime) Implements ICovid19Viaggiatori.SalvaElaborazioneViaggiatoreExtraEU
        Dim ownConnection As Boolean = False
        Dim query As String = "UPDATE T_ANA_VIAGGIATORI_EXTRA_UE_C19 " +
                              "SET VEU_STATO_ELABORAZIONE = :VEU_STATO_ELABORAZIONE, VEU_DATA_ELABORAZIONE = :VEU_DATA_ELABORAZIONE, VEU_NOTE_ELABORAZIONE = :VEU_NOTE_ELABORAZIONE " +
                              "WHERE VEU_CODICE_FISCALE = :VEU_CODICE_FISCALE AND VEU_DATA_RIENTRO = :VEU_DATA_RIENTRO AND VEU_DATA = :VEU_DATA "

        Try
            Using cmd As OracleCommand = New OracleCommand(query, Connection)

                cmd.Parameters.AddWithValueOrDefault("VEU_STATO_ELABORAZIONE", statoElab)
                cmd.Parameters.AddWithValueOrDefault("VEU_DATA_ELABORAZIONE", dataElab)
                cmd.Parameters.AddWithValueOrDefault("VEU_NOTE_ELABORAZIONE", noteElaborazione)
                cmd.Parameters.AddWithValueOrDefault("VEU_CODICE_FISCALE", codiceFiscale)
                cmd.Parameters.AddWithValueOrDefault("VEU_DATA_RIENTRO", dataRientro)
                cmd.Parameters.AddWithValueOrDefault("VEU_DATA", data)

                ownConnection = ConditionalOpenConnection(cmd)
                cmd.ExecuteNonQuery()

            End Using
        Finally
            ConditionalCloseConnection(ownConnection)
        End Try
    End Function


End Class
