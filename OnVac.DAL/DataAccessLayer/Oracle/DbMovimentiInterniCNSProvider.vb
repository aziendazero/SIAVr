Imports System.Data.OracleClient
Imports Onit.Database.DataAccessManager


Namespace DAL.Oracle

    Public Class DbMovimentiInterniCNSProvider
        Inherits DbProvider
        Implements IMovimentiInterniCNSProvider

#Region " Constructors "

        Public Sub New(DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " Public "

        ''' <summary>
        ''' Inserisce un movimento interno per il paziente specificato
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="codiceConsultorioOld"></param>
        ''' <param name="codiceConsultorioNew"></param>
        ''' <param name="dataAssegnazione"></param>
        ''' <param name="flagInvioCartella"></param>
        ''' <param name="flagPresaVisione"></param>
        ''' <param name="flagAutoAdulti"></param>
        Public Function InserimentoMovimentoPaziente(codicePaziente As Integer, codiceConsultorioOld As String, codiceConsultorioNew As String, dataAssegnazione As DateTime, flagInvioCartella As Boolean, flagPresaVisione As Boolean, flagAutoAdulti As Boolean) As Integer Implements IMovimentiInterniCNSProvider.InserimentoMovimentoPaziente

            Dim count As Integer = 0

            Using cmd As OracleCommand = New OracleCommand(Queries.MovimentiInterniCNS.OracleQueries.insMovimentoPaziente, Me.Connection)

                cmd.Parameters.AddWithValue("cnm_paz_codice", codicePaziente)
                cmd.Parameters.AddWithValue("cnm_cns_codice_old", GetStringParam(codiceConsultorioOld, False))
                cmd.Parameters.AddWithValue("cnm_cns_codice_new", GetStringParam(codiceConsultorioNew, False))
                cmd.Parameters.AddWithValue("cnm_data", dataAssegnazione)
                cmd.Parameters.AddWithValue("cnm_invio_cartella", IIf(flagInvioCartella, "S", "N"))
                cmd.Parameters.AddWithValue("cnm_presa_visione", IIf(flagPresaVisione, "S", "N"))
                cmd.Parameters.AddWithValue("cnm_auto_adulti", IIf(flagAutoAdulti, "S", "N"))

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Update campo "Presa Visione" relativo al movimento specificato
        ''' </summary>
        ''' <param name="progressivoMovimento"></param>
        ''' <param name="presaVisione"></param>
        Public Function UpdatePresaVisione(progressivoMovimento As Integer, presaVisione As String) As Integer Implements IMovimentiInterniCNSProvider.UpdatePresaVisione

            Dim num As Integer

            Using cmd As OracleCommand = New OracleCommand(Queries.MovimentiInterniCNS.OracleQueries.updPresaVisionePaziente, Me.Connection)

                cmd.Parameters.AddWithValue("cnm_presa_visione", presaVisione)
                cmd.Parameters.AddWithValue("cnm_progressivo", progressivoMovimento)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    num = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return num

        End Function

        ''' <summary>
        ''' Update campo "Invio Cartella" relativo al movimento specificato
        ''' </summary>
        ''' <param name="progressivoMovimento"></param>
        ''' <param name="invioCartella"></param>
        Public Function UpdateInvioCartella(progressivoMovimento As Integer, invioCartella As Boolean) As Integer Implements IMovimentiInterniCNSProvider.UpdateInvioCartella

            Dim num As Integer

            Using cmd As OracleCommand = New OracleCommand(Queries.MovimentiInterniCNS.OracleQueries.updInviaCartellaPaziente, Me.Connection)

                cmd.Parameters.AddWithValue("cnm_invio_cartella", IIf(invioCartella, "S", "N"))
                cmd.Parameters.AddWithValue("cnm_progressivo", progressivoMovimento)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    num = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return num

        End Function


        Public Function GetDtSmistamenti(smistamentiFilter As AppoggiatiRASMIFiltriRicerca, pagingOptions As MovimentiInterniCNSPagingOptions?, codUslCorrente As String, codCnsDefault As String) As DataTable Implements IMovimentiInterniCNSProvider.GetDtSmistamenti

            Dim dt As New DataTable()

            With _DAM.QB
                '--------- sottoquery dei distretti della usl corrente -------------
                .NewQuery()
                .AddSelectFields("DIS_CODICE")
                .AddTables("t_ana_distretti")
                .AddWhereCondition("DIS_USL_CODICE", Comparatori.Uguale, codUslCorrente, DataTypes.Stringa)

                Dim strQueryDistrettiUslCorrente As String = .GetSelect()


                .NewQuery(False, False)

                .AddSelectFields("paz_cognome, paz_nome, paz_data_nascita, san_descrizione as stato_anagrafico, paz_indirizzo_domicilio, com_descrizione")
                .AddSelectFields("paz_cns_codice, cns_descrizione, paz_data_inserimento, paz_codice, paz_cns_data_assegnazione, paz_stato_anagrafico")
                .AddTables("t_paz_pazienti, t_ana_consultori, t_ana_stati_anagrafici, t_ana_comuni")

                .AddWhereCondition("paz_cns_codice", Comparatori.Uguale, "cns_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("paz_stato_anagrafico", Comparatori.Uguale, "san_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("paz_com_codice_domicilio", Comparatori.Uguale, "com_codice", DataTypes.OutJoinLeft)

                AddLoadSmistamentiWhereCondition(_DAM.QB, smistamentiFilter, strQueryDistrettiUslCorrente, codCnsDefault)

                .AddOrderByFields("paz_cns_data_assegnazione desc, paz_cognome, paz_nome, paz_data_nascita")

            End With

            If Not pagingOptions Is Nothing Then

                _DAM.QB.AddPaginatedOracleQuery(pagingOptions.Value.StartRecordIndex, pagingOptions.Value.EndRecordIndex)

            End If

            Try
                _DAM.BuildDataTable(dt)
            Finally
                _DAM.Dispose(False)
            End Try

            Return dt

        End Function

        Public Function AddLoadSmistamentiWhereCondition(abstractQB As AbstractQB, smistamentiFilter As AppoggiatiRASMIFiltriRicerca, strQueryDistrettiUslCorrente As String, codCnsDefault As String)

            With abstractQB

                If Not smistamentiFilter Is Nothing Then
                    If smistamentiFilter.DataNascitaDa.HasValue Then
                        .AddWhereCondition("paz_data_nascita", Comparatori.MaggioreUguale, smistamentiFilter.DataNascitaDa, DataTypes.Data)
                    End If
                    If smistamentiFilter.DataNascitaA.HasValue Then
                        .AddWhereCondition("paz_data_nascita", Comparatori.MinoreUguale, smistamentiFilter.DataNascitaA, DataTypes.Data)
                    End If
                    If smistamentiFilter.DataCnsDa.HasValue Then
                        .AddWhereCondition("paz_cns_data_assegnazione", Comparatori.MaggioreUguale, smistamentiFilter.DataCnsDa, DataTypes.Data)
                    End If
                    If smistamentiFilter.DataCnsA.HasValue Then
                        .AddWhereCondition("paz_cns_data_assegnazione", Comparatori.MinoreUguale, smistamentiFilter.DataCnsA, DataTypes.Data)
                    End If
                End If

                'ciascun centro vaccinale vede:
                '   i pazienti "smistati" nel proprio cns (tramite la join con i distretti) 
                '   + quelli "smistati" nel cns generico comune a tutta la usl unificata (identificato con il parametro CNS_DEFAULT configurato)
                'OLD: .AddWhereCondition("cns_smistamento", Comparatori.Uguale, "S", DataTypes.Stringa)
                .OpenParanthesis()
                .OpenParanthesis()
                .AddWhereCondition("cns_smistamento", Comparatori.Uguale, "S", DataTypes.Stringa)
                .AddWhereCondition("cns_dis_codice", Comparatori.In, strQueryDistrettiUslCorrente, DataTypes.Replace)
                .CloseParanthesis()
                .AddWhereCondition("cns_codice", Comparatori.Uguale, codCnsDefault, DataTypes.Stringa, "or")
                .CloseParanthesis()

            End With

        End Function


        Public Function CountSmistamenti(smistamentiFilter As AppoggiatiRASMIFiltriRicerca, codUslCorrente As String, codCnsDefault As String) As Integer Implements IMovimentiInterniCNSProvider.CountSmistamenti

            With _DAM.QB
                '--------- sottoquery dei distretti della usl corrente -------------
                .NewQuery()
                .AddSelectFields("DIS_CODICE")
                .AddTables("t_ana_distretti")
                .AddWhereCondition("DIS_USL_CODICE", Comparatori.Uguale, codUslCorrente, DataTypes.Stringa)

                Dim strQueryDistrettiUslCorrente As String = .GetSelect()


                .NewQuery(False, False)

                .AddSelectFields("COUNT(distinct paz_codice)")
                .AddTables("t_paz_pazienti, t_ana_consultori, t_ana_stati_anagrafici, t_ana_comuni")

                .AddWhereCondition("paz_cns_codice", Comparatori.Uguale, "cns_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("paz_stato_anagrafico", Comparatori.Uguale, "san_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("paz_com_codice_domicilio", Comparatori.Uguale, "com_codice", DataTypes.OutJoinLeft)

                AddLoadSmistamentiWhereCondition(_DAM.QB, smistamentiFilter, strQueryDistrettiUslCorrente, codCnsDefault)

            End With

            Try
                Return _DAM.ExecScalar()
            Finally
                _DAM.Dispose(False)
            End Try

        End Function

#End Region

    End Class

End Namespace
