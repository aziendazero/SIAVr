Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager

Namespace DAL

    Public Class DbInstallazioniProvider
        Inherits DbProvider
        Implements IInstallazioniProvider

#Region " Constructors "

        Public Sub New(ByRef DAM As IDAM)
            MyBase.New(DAM)
        End Sub

#End Region

#Region " IInstallazioniProvider "

        Public Function GetInstallazione(codiceUslCorrente As String) As Installazione Implements IInstallazioniProvider.GetInstallazione

            Dim installazione As Installazione = Nothing

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("*")
                .AddTables("T_ANA_INSTALLAZIONI")
                .AddWhereCondition("INS_USL_CODICE", Comparatori.Uguale, codiceUslCorrente, DataTypes.Stringa)
            End With

            Using dr As IDataReader = _DAM.BuildDataReader()

                If Not dr Is Nothing Then

                    Dim ins_installazione As Int16 = dr.GetOrdinal("INS_INSTALLAZIONE")
                    Dim ins_reg_codice As Int16 = dr.GetOrdinal("INS_REG_CODICE")
                    Dim ins_usl_codice As Int16 = dr.GetOrdinal("INS_USL_CODICE")
                    Dim ins_usl_descrizione As Int16 = dr.GetOrdinal("INS_USL_DESCRIZIONE")
                    Dim ins_usl_descrizione_report As Int16 = dr.GetOrdinal("INS_USL_DESCRIZIONE_REPORT")
                    Dim ins_usl_indirizzo As Int16 = dr.GetOrdinal("INS_USL_INDIRIZZO")
                    Dim ins_usl_cap As Int16 = dr.GetOrdinal("INS_USL_CAP")
                    Dim ins_usl_citta As Int16 = dr.GetOrdinal("INS_USL_CITTA")
                    Dim ins_usl_provincia As Int16 = dr.GetOrdinal("INS_USL_PROVINCIA")
                    Dim ins_usl_regione As Int16 = dr.GetOrdinal("INS_USL_REGIONE")
                    Dim ins_usl_telefono As Int16 = dr.GetOrdinal("INS_USL_TELEFONO")
                    Dim ins_usl_scadenza As Int16 = dr.GetOrdinal("INS_USL_SCADENZA")
                    Dim ins_usl_sitoweb As Int16 = dr.GetOrdinal("INS_USL_SITOWEB")
                    Dim ins_usl_email As Int16 = dr.GetOrdinal("INS_USL_EMAIL")
                    Dim ins_usl_partita_iva As Int16 = dr.GetOrdinal("INS_USL_PARTITA_IVA")
                    Dim ins_usl_fax As Int16 = dr.GetOrdinal("INS_USL_FAX")
                    Dim ins_intestazione_report As Int16 = dr.GetOrdinal("INS_INTESTAZIONE_REPORT")
                    Dim ins_piede_report As Int16 = dr.GetOrdinal("INS_PIEDE_REPORT")
                    Dim ins_responsabile As Int16 = dr.GetOrdinal("INS_RESPONSABILE")

                    If dr.Read() Then

                        installazione = New Installazione()

                        installazione.CodiceInstallazione = dr.GetString(ins_installazione)
                        installazione.CodiceRegione = dr.GetStringOrDefault(ins_reg_codice)
                        installazione.UslCodice = dr.GetStringOrDefault(ins_usl_codice)
                        installazione.UslDescrizione = dr.GetStringOrDefault(ins_usl_descrizione)
                        installazione.UslDescrizionePerReport = dr.GetStringOrDefault(ins_usl_descrizione_report)
                        installazione.UslIndirizzo = dr.GetStringOrDefault(ins_usl_indirizzo)
                        installazione.UslCap = dr.GetStringOrDefault(ins_usl_cap)
                        installazione.UslCitta = dr.GetStringOrDefault(ins_usl_citta)
                        installazione.UslProvincia = dr.GetStringOrDefault(ins_usl_provincia)
                        installazione.UslRegione = dr.GetStringOrDefault(ins_usl_regione)
                        installazione.UslTelefono = dr.GetStringOrDefault(ins_usl_telefono)
                        installazione.UslScadenza = dr.GetStringOrDefault(ins_usl_scadenza)
                        installazione.UslSitoWeb = dr.GetStringOrDefault(ins_usl_sitoweb)
                        installazione.UslEMail = dr.GetStringOrDefault(ins_usl_email)
                        installazione.UslPartitaIva = dr.GetStringOrDefault(ins_usl_partita_iva)
                        installazione.UslFax = dr.GetStringOrDefault(ins_usl_fax)
                        installazione.IntestazioneReport = dr.GetStringOrDefault(ins_intestazione_report)
                        installazione.PiedeReport = dr.GetStringOrDefault(ins_piede_report)
                        installazione.Responsabile = dr.GetStringOrDefault(ins_responsabile)

                    End If

                End If

            End Using

            Return installazione

        End Function

        ''' <summary>
        ''' Restituisce la descrizione della usl in base al codice usl
        ''' </summary>
        ''' <param name="codiceUsl"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDescrizioneUsl(codiceUsl As String) As String Implements IInstallazioniProvider.GetDescrizioneUsl

            Dim descrizione As String = String.Empty

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                cmd.CommandText = "select ins_usl_descrizione from t_ana_installazioni where ins_usl_codice = :codiceUsl"
                cmd.Parameters.AddWithValue("codiceUsl", codiceUsl)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj Is Nothing OrElse obj Is DBNull.Value Then
                        descrizione = String.Empty
                    Else
                        descrizione = obj.ToString()
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return descrizione

        End Function

#End Region

    End Class

End Namespace
