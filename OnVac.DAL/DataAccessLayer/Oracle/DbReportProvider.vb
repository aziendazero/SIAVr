Imports System.Collections.Generic
Imports System.Data.OracleClient

Imports Onit.Database.DataAccessManager

Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.Collection


Namespace DAL.Oracle

    Public Class DbReportProvider
        Inherits DbProvider
        Implements IReportProvider

#Region " Constructors "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " IReportProvider "

        ''' <summary>
        ''' Restituisce i dati del report, in base al nome
        ''' </summary>
        ''' <param name="nomeReport"></param>
        ''' <returns></returns>
        Public Function GetReport(nomeReport As String) As Report Implements IReportProvider.GetReport

            Dim rpt As Report = Nothing

            Using cmd As OracleCommand = New OracleCommand(Queries.Report.OracleQueries.selReport, Connection)

                cmd.Parameters.AddWithValue("nome_rpt", GetStringParam(nomeReport, False))

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim listReports As List(Of Report) = GetReportList(cmd)

                    If Not listReports.IsNullOrEmpty() Then
                        rpt = listReports.First()
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return rpt

        End Function

        ''' <summary>
        ''' Restituisce una lista con i dati dei report specificati, in base al nome.
        ''' Se non trova nessun report restituisce Nothing.
        ''' </summary>
        ''' <param name="nomiReport"></param>
        ''' <returns></returns>
        Public Function GetReports(nomiReport As List(Of String)) As List(Of Report) Implements IReportProvider.GetReports

            Dim listReports As List(Of Report) = Nothing

            If nomiReport.IsNullOrEmpty() Then Return Nothing

            Using cmd As OracleCommand = New OracleCommand(String.Empty, Connection)

                Dim filtroInResult As GetInFilterResult = GetInFilter(nomiReport)

                cmd.Parameters.AddRange(filtroInResult.Parameters)

                cmd.CommandText = String.Format(Queries.Report.OracleQueries.selReportList, filtroInResult.InFilter)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    listReports = GetReportList(cmd)

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listReports

        End Function

        ''' <summary>
        ''' Restituisce tutti i dati che compongono l'intestazione di ogni report relativo all'installazione specificata.
        ''' </summary>
        ''' <param name="codiceUsl"></param>
        ''' <returns></returns>
        Public Function GetDatiIntestazione(codiceUsl As String) As DatiIntestazioneReport Implements IReportProvider.GetDatiIntestazione

            Dim datiIntestazioneReport As DatiIntestazioneReport = Nothing

            Using cmd As New OracleCommand(Queries.Report.OracleQueries.selDatiInstallazione, Connection)

                cmd.Parameters.AddWithValue("ins_usl_codice", GetStringParam(codiceUsl, False))

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim pos_cap As Integer = idr.GetOrdinal("ins_usl_cap")
                            Dim pos_cod_reg As Integer = idr.GetOrdinal("ins_reg_codice")
                            Dim pos_cod_usl As Integer = idr.GetOrdinal("ins_usl_codice")
                            Dim pos_comune As Integer = idr.GetOrdinal("ins_usl_citta")
                            Dim pos_descr_usl As Integer = idr.GetOrdinal("ins_usl_descrizione")
                            Dim pos_descr_usl_rpt As Integer = idr.GetOrdinal("ins_usl_descrizione_report")
                            Dim pos_fax As Integer = idr.GetOrdinal("ins_usl_fax")
                            Dim pos_indir As Integer = idr.GetOrdinal("ins_usl_indirizzo")
                            Dim pos_inst As Integer = idr.GetOrdinal("ins_installazione")
                            Dim pos_piva As Integer = idr.GetOrdinal("ins_usl_partita_iva")
                            Dim pos_prov As Integer = idr.GetOrdinal("ins_usl_provincia")
                            Dim pos_reg As Integer = idr.GetOrdinal("ins_usl_regione")
                            Dim pos_rpt_footer As Integer = idr.GetOrdinal("ins_piede_report")
                            Dim pos_rpt_header As Integer = idr.GetOrdinal("ins_intestazione_report")
                            Dim pos_resp As Integer = idr.GetOrdinal("ins_responsabile")
                            Dim pos_scad_usl As Integer = idr.GetOrdinal("ins_usl_scadenza")
                            Dim pos_tel As Integer = idr.GetOrdinal("ins_usl_telefono")

                            If idr.Read() Then

                                datiIntestazioneReport = New DatiIntestazioneReport()

                                datiIntestazioneReport.CapUsl = idr.GetStringOrDefault(pos_cap)
                                datiIntestazioneReport.CodiceRegione = idr.GetStringOrDefault(pos_cod_reg)
                                datiIntestazioneReport.CodiceUsl = idr.GetStringOrDefault(pos_cod_usl)
                                datiIntestazioneReport.ComuneUsl = idr.GetStringOrDefault(pos_comune)
                                datiIntestazioneReport.DescrizioneUsl = idr.GetStringOrDefault(pos_descr_usl)
                                datiIntestazioneReport.DescrizioneUslPerReport = idr.GetStringOrDefault(pos_descr_usl_rpt)
                                datiIntestazioneReport.FaxUsl = idr.GetStringOrDefault(pos_fax)
                                datiIntestazioneReport.IndirizzoUsl = idr.GetStringOrDefault(pos_indir)
                                datiIntestazioneReport.Installazione = idr.GetStringOrDefault(pos_inst)
                                datiIntestazioneReport.PartitaIvaUsl = idr.GetStringOrDefault(pos_piva)
                                datiIntestazioneReport.ProvinciaUsl = idr.GetStringOrDefault(pos_prov)
                                datiIntestazioneReport.RegioneUsl = idr.GetStringOrDefault(pos_reg)
                                datiIntestazioneReport.ReportFooter = idr.GetStringOrDefault(pos_rpt_footer)
                                datiIntestazioneReport.ReportHeader = idr.GetStringOrDefault(pos_rpt_header)
                                datiIntestazioneReport.Responsabile = idr.GetStringOrDefault(pos_resp)
                                datiIntestazioneReport.ScadenzaUsl = idr.GetStringOrDefault(pos_scad_usl)
                                datiIntestazioneReport.TelefonoUsl = idr.GetStringOrDefault(pos_tel)

                            End If

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return datiIntestazioneReport

        End Function

        ''' <summary>
        ''' Restituisce la cartella in cui è presente il report, in base al nome
        ''' </summary>
        ''' <param name="nomeReport"></param>
        ''' <returns></returns>
        Public Function GetReportFolder(nomeReport As String) As String Implements IReportProvider.GetReportFolder

            Dim folder As String = String.Empty

            Using cmd As New OracleCommand(Queries.Report.OracleQueries.selReportFolder, Connection)

                cmd.Parameters.AddWithValue("nome_rpt", GetStringParam(nomeReport, False))

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing Then folder = obj.ToString()

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return folder

        End Function

#End Region

#Region " Private "

        Private Function GetReportList(cmd As OracleCommand) As List(Of Report)

            Dim listReports As List(Of Report) = Nothing

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    listReports = New List(Of Report)()

                    Dim nome As String = String.Empty

                    Dim rpt As Report = Nothing

                    Dim pos_nome As Integer = idr.GetOrdinal("rpt_nome")
                    Dim pos_installazione As Integer = idr.GetOrdinal("rpt_installazione")
                    Dim pos_cartella As Integer = idr.GetOrdinal("rpt_cartella")
                    Dim pos_dataset As Integer = idr.GetOrdinal("rpt_dataset")

                    While idr.Read()

                        nome = idr.GetStringOrDefault(pos_nome)

                        rpt = New Report(nome)

                        rpt.Nome = nome
                        rpt.Installazione = idr.GetStringOrDefault(pos_installazione)
                        rpt.Cartella = idr.GetStringOrDefault(pos_cartella)
                        rpt.DataSet = idr.GetStringOrDefault(pos_dataset)

                        listReports.Add(rpt)

                    End While

                End If

            End Using

            Return listReports

        End Function

#End Region

    End Class

End Namespace