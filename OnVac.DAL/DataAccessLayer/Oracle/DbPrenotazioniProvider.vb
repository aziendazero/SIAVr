Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.Data.OracleClient
Imports Onit.OnAssistnet.OnVac.Filters


Namespace DAL.Oracle

    Public Class DbPrenotazioniProvider
        Inherits DbProvider
        Implements IPrenotazioniProvider

#Region " Enum "

        Public Enum Entity
            Ambulatorio
            Consultorio
        End Enum

#End Region

#Region " Costruttori "

        Public Sub New(ByRef DAM As IDAM)
            MyBase.New(DAM)
        End Sub

#End Region

#Region " IPrenotazioniProvider "

        ''' <summary>
        ''' Carica gli orari di appuntamento dell'ambulatorio
        ''' </summary>
        ''' <param name="codiceAmbulatorio">Codice ambulatorio</param>
        Public Function GetOrariAppuntamento(codiceAmbulatorio As Integer) As DataTable Implements IPrenotazioniProvider.GetOrariAppuntamento

            RefurbishDT()

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("ORA_GIORNO, ORA_AM_INIZIO, ORA_AM_FINE, ORA_PM_INIZIO, ORA_PM_FINE")
                .AddTables("T_ANA_ORARI_APPUNTAMENTI")
                .AddWhereCondition("ORA_AMB_CODICE", Comparatori.Uguale, codiceAmbulatorio, DataTypes.Stringa)
            End With

            Try
                _DAM.BuildDataTable(_DT)

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return _DT.Copy

        End Function

        ''' <summary>
        ''' Controllo che la data/ora in cui si vuole prenotare non finisca con il sovrapporsi
        ''' con un altro appuntamento già prenotato
        ''' </summary>
        ''' <param name="tmpData"></param>
        ''' <param name="durataAppuntamento"></param>
        ''' <param name="codiceAmbulatorio">Codice ambulatorio</param>
        ''' <param name="orario"></param>
        ''' <param name="warning"></param>
        Public Function AppuntamentoSovrapposto(tmpData As String, durataAppuntamento As Integer, codiceAmbulatorio As Integer, orario As String) As Boolean Implements IPrenotazioniProvider.AppuntamentoSovrapposto

            Dim val As Integer = 0
            Dim dataOra As New DateTime(tmpData.Split("/")(2), tmpData.Split("/")(1), tmpData.Split("/")(0), orario.Split(".")(0), orario.Split(".")(1), 0)

            With _DAM.QB
                .NewQuery()
                .AddTables("T_CNV_CONVOCAZIONI")
                .AddSelectFields("count(*)")
                .AddWhereCondition("1", Comparatori.Uguale, "1", DataTypes.Replace)
                .OpenParanthesis()
                .OpenParanthesis()
                .AddWhereCondition("CNV_DATA_APPUNTAMENTO", Comparatori.MaggioreUguale, dataOra, DataTypes.DataOra)
                .AddWhereCondition("CNV_DATA_APPUNTAMENTO", Comparatori.Minore, dataOra.AddMinutes(durataAppuntamento), DataTypes.DataOra)
                .CloseParanthesis()
                .OpenParanthesis()
                .AddWhereCondition("CNV_DATA_APPUNTAMENTO + to_number(CNV_DURATA_APPUNTAMENTO)/1440", Comparatori.Maggiore, "to_date('" & dataOra & "','dd/MM/yyyy hh24:mi:ss')", DataTypes.Replace, "or")
                .AddWhereCondition("CNV_DATA_APPUNTAMENTO + to_number(CNV_DURATA_APPUNTAMENTO)/1440", Comparatori.MinoreUguale, "to_date('" & dataOra.AddMinutes(durataAppuntamento) & "','dd/MM/yyyy hh24:mi:ss')", DataTypes.Replace)
                .CloseParanthesis()
                .OpenParanthesis()
                .AddWhereCondition("CNV_DATA_APPUNTAMENTO", Comparatori.MinoreUguale, dataOra, DataTypes.Data, "or")
                .AddWhereCondition("CNV_DATA_APPUNTAMENTO + to_number(CNV_DURATA_APPUNTAMENTO)/1440", Comparatori.Maggiore, "to_date('" & dataOra.AddMinutes(durataAppuntamento) & "','dd/MM/yyyy hh24:mi:ss')", DataTypes.Replace)
                .CloseParanthesis()
                .CloseParanthesis()
                .AddWhereCondition("CNV_AMB_CODICE", Comparatori.Uguale, codiceAmbulatorio, DataTypes.Numero)
            End With

            Try
                val += CType(_DAM.ExecScalar, Integer)
            Catch ex As Exception
                LogError(ex)
                SetErrorMsg("Si è verificato un problema durante il tentativo di validazione dei dati in 'appuntamentoSovrapposto'")
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            If val > 0 Then
                Return True
            Else
                Return False
            End If

        End Function

        Public Function OrarioCompresoInOrarioAperturaAmb(tmpData As String, durataAppuntamento As Integer, codiceAmbulatorio As Integer, orario As String) As Boolean Implements IPrenotazioniProvider.OrarioCompresoInOrarioAperturaAmb

            Dim val As Integer = 0
            Dim dataOra As New DateTime(tmpData.Split("/")(2), tmpData.Split("/")(1), tmpData.Split("/")(0), orario.Split(".")(0), orario.Split(".")(1), 0)

            With _DAM.QB

                '''''''''''
                .NewQuery()
                .AddTables("T_ANA_FESTIVITA")
                .AddSelectFields("1")
                .AddWhereCondition("to_char(to_date('" & dataOra & "','DD/MM/YYYY hh24:mi:ss'),'DD/MM')", Comparatori.Uguale, "to_char(FES_DATA,'DD/MM')", DataTypes.Replace)
                Dim q1 As String = .GetSelect
                '''''''
                .NewQuery(False, True)
                .AddTables("T_ANA_FASCE_INDISPONIBILITA")
                .AddSelectFields("1")
                .AddWhereCondition("1", Comparatori.Uguale, "1", DataTypes.Replace)
                .OpenParanthesis()
                .OpenParanthesis()
                .AddWhereCondition("FAS_ORA_INIZIO", Comparatori.MinoreUguale, dataOra.ToString, DataTypes.DataOra)
                .AddWhereCondition("FAS_ORA_FINE", Comparatori.Maggiore, dataOra.ToString, DataTypes.DataOra)
                .CloseParanthesis()
                .OpenParanthesis()
                .AddWhereCondition("to_date('" & OnVac.Constants.CommonConstants.RECURSIVE_YEAR & "' || to_char(to_date('" & dataOra & "','dd/mm/yyyy hh24:mi:ss'),'/DD/MM HH:MI'),'YYYY/DD/MM HH24:MI')", Comparatori.MaggioreUguale, "FAS_ORA_INIZIO", DataTypes.Replace, "or")
                .AddWhereCondition("to_date('" & OnVac.Constants.CommonConstants.RECURSIVE_YEAR & "' || to_char(to_date('" & dataOra & "','dd/mm/yyyy hh24:mi:ss'),'/DD/MM HH:MI'),'YYYY/DD/MM HH24:MI')", Comparatori.Minore, "FAS_ORA_FINE", DataTypes.Replace)
                .CloseParanthesis()
                .CloseParanthesis()
                'il consultorio deve essere quello specificato per l'appuntamento [modifica 09/05/2005]
                .AddWhereCondition("FAS_AMB_CODICE", Comparatori.Uguale, codiceAmbulatorio, DataTypes.Numero)

                Dim q2 As String = .GetSelect
                '''''''''
                .NewQuery(False, True)
                .AddTables("T_ANA_ORARI_APPUNTAMENTI")
                .AddSelectFields("count(*)")
                .AddWhereCondition("1", Comparatori.Uguale, "1", DataTypes.Replace)
                .OpenParanthesis()
                .OpenParanthesis()
                .AddWhereCondition("to_char(to_date('" & dataOra & "','dd/mm/yyyy hh24:mi:ss'),'HH24:MI')", Comparatori.MaggioreUguale, "to_char(ORA_AM_INIZIO,'HH24:MI')", DataTypes.Replace)
                .AddWhereCondition("to_char(to_date('" & dataOra & "','dd/mm/yyyy hh24:mi:ss'),'HH24:MI')", Comparatori.Minore, "to_char(ORA_AM_FINE,'HH24:MI')", DataTypes.Replace)
                .CloseParanthesis()
                .OpenParanthesis()
                .AddWhereCondition("to_char(to_date('" & dataOra & "','dd/mm/yyyy hh24:mi:ss'),'HH24:MI')", Comparatori.MaggioreUguale, "to_char(ORA_PM_INIZIO,'HH24:MI')", DataTypes.Replace, "or")
                .AddWhereCondition("to_char(to_date('" & dataOra & "','dd/mm/yyyy hh24:mi:ss'),'HH24:MI')", Comparatori.Minore, "to_char(ORA_PM_FINE,'HH24:MI')", DataTypes.Replace)
                .CloseParanthesis()
                .CloseParanthesis()
                'il consultorio deve essere quello specificato per l'appuntamento [modifica 09/05/2005]
                .AddWhereCondition("ORA_AMB_CODICE", Comparatori.Uguale, codiceAmbulatorio, DataTypes.Numero)
                'Brigoz:porcata...per avere il valore giusto con la trunc aggiungo 1 giorno..non sapevo come fare
                .AddWhereCondition("trunc(to_date('" & CType(tmpData, DateTime).AddDays(1).ToString.Split()(0) & "','dd/mm/yyyy'))-trunc(to_date('" & CType(tmpData, DateTime).AddDays(1).ToString.Split()(0) & "','dd/mm/yyyy'),'IW')", Comparatori.Uguale, "ORA_GIORNO", DataTypes.Replace)
                .AddWhereCondition("", Comparatori.NotExist, "(" & q1 & ")", DataTypes.Replace)
                .AddWhereCondition("", Comparatori.NotExist, "(" & q2 & ")", DataTypes.Replace)
                val = CType(_DAM.ExecScalar, Integer)

                ''''''''''
                .NewQuery()
                .AddSelectFields("1")
                .AddTables("T_ANA_FESTIVITA")
                .AddWhereCondition("to_char(to_date('" & dataOra & "','dd/mm/yyyy hh24:mi:ss'),'DD/MM')", Comparatori.Uguale, "to_char(FES_DATA,'DD/MM')", DataTypes.Replace)
                Dim q3 As String = .GetSelect
                '''''''''
                .NewQuery(False, True)
                .AddTables("T_ANA_FASCE_INDISPONIBILITA")
                .AddSelectFields("1")
                .AddWhereCondition("1", Comparatori.Uguale, "1", DataTypes.Replace)
                .OpenParanthesis()
                .OpenParanthesis()
                .AddWhereCondition("FAS_ORA_INIZIO", Comparatori.MinoreUguale, dataOra.ToString, DataTypes.DataOra)
                .AddWhereCondition("FAS_ORA_FINE", Comparatori.Maggiore, dataOra.ToString, DataTypes.DataOra)
                .CloseParanthesis()
                .OpenParanthesis()
                .AddWhereCondition("to_date('" & OnVac.Constants.CommonConstants.RECURSIVE_YEAR & "' || to_char(to_date('" & dataOra & "','dd/mm/yyyy hh24:mi:ss') ,'/MM/DD HH:MI'),'YYYY/MM/DD HH24:MI')", Comparatori.MaggioreUguale, "FAS_ORA_INIZIO", DataTypes.Replace, "or")
                .AddWhereCondition("to_date('" & OnVac.Constants.CommonConstants.RECURSIVE_YEAR & "' || to_char(to_date('" & dataOra & "','dd/mm/yyyy hh24:mi:ss') ,'/MM/DD HH:MI'),'YYYY/MM/DD HH24:MI')", Comparatori.Minore, "FAS_ORA_FINE", DataTypes.Replace)
                .CloseParanthesis()
                .CloseParanthesis()
                'il consultorio deve essere quello specificato per l'appuntamento [modifica 09/05/2005]
                .AddWhereCondition("FAS_AMB_CODICE", Comparatori.Uguale, codiceAmbulatorio, DataTypes.Numero)
                Dim q4 As String = .GetSelect

                .NewQuery(False, True)
                .AddTables("T_ANA_ORARI_GIORNALIERI")
                .AddSelectFields("count(*)")
                .AddWhereCondition("1", Comparatori.Uguale, "1", DataTypes.Replace)
                .OpenParanthesis()
                .OpenParanthesis()
                .AddWhereCondition("to_char(to_date('" & dataOra & "','dd/mm/yyyy hh24:mi:ss'),'HH24:MI')", Comparatori.MaggioreUguale, "to_char(ORG_AM_INIZIO,'HH24:MI')", DataTypes.Replace)
                .AddWhereCondition("to_char(to_date('" & dataOra & "','dd/mm/yyyy hh24:mi:ss'),'HH24:MI')", Comparatori.Minore, "to_char(ORG_AM_FINE,'HH24:MI')", DataTypes.Replace)
                .CloseParanthesis()
                .OpenParanthesis()
                .AddWhereCondition("to_char(to_date('" & dataOra & "','dd/mm/yyyy hh24:mi:ss'),'HH24:MI')", Comparatori.MaggioreUguale, "to_char(ORG_PM_INIZIO,'HH24:MI')", DataTypes.Replace, "or")
                .AddWhereCondition("to_char(to_date('" & dataOra & "','dd/mm/yyyy hh24:mi:ss'),'HH24:MI')", Comparatori.Minore, "to_char(ORG_PM_FINE,'HH24:MI')", DataTypes.Replace)
                .CloseParanthesis()
                .CloseParanthesis()
                'il consultorio deve essere quello specificato per l'appuntamento [modifica 09/05/2005]
                .AddWhereCondition("ORG_AMB_CODICE", Comparatori.Uguale, codiceAmbulatorio, DataTypes.Numero)
                'Brigoz:porcata...per avere il valore giusto con la trunc aggiungo 1 giorno..non sapevo come fare
                .AddWhereCondition("trunc(to_date('" & CType(tmpData, DateTime).AddDays(1).ToString.Split()(0) & "','dd/mm/yyyy'))-trunc(to_date('" & CType(tmpData, DateTime).AddDays(1).ToString.Split()(0) & "','dd/mm/yyyy'),'IW')", Comparatori.Uguale, "ORG_GIORNO", DataTypes.Replace)
                .AddWhereCondition("", Comparatori.NotExist, "(" & q3 & ")", DataTypes.Replace)
                .AddWhereCondition("", Comparatori.NotExist, "(" & q4 & ")", DataTypes.Replace)

            End With

            Try
                val += CType(_DAM.ExecScalar, Integer)

            Catch ex As Exception
                LogError(ex)
                SetErrorMsg("Si è verificato un problema durante il tentativo di validazione dei dati.")
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            If val > 0 Then
                Return True
            Else
                Return False
            End If

        End Function

        ''' <summary>
        ''' Ricerca tutte le convocazioni (con e senza appuntamenti) del paziente
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="codiceConsultorioCorrente"></param>
        ''' <param name="soloConsultorioCorrente"></param>
        Public Function CercaConvocazioniAppuntamentiPaziente(codicePaziente As Integer, codiceConsultorioCorrente As String, soloConsultorioCorrente As Boolean) As DataTable Implements IPrenotazioniProvider.CercaConvocazioniAppuntamentiPaziente

            RefurbishDT()

            With _DAM.QB
                .NewQuery()

                .AddSelectFields("PAZ_NOME, PAZ_COGNOME, CNV_DATA, CNV_DATA_APPUNTAMENTO, CNV_DURATA_APPUNTAMENTO, OPE_NOME")
                .AddSelectFields("CNV_CNS_CODICE, CNV_AMB_CODICE, CNV_PRIMO_APPUNTAMENTO, CNS_DESCRIZIONE, AMB_DESCRIZIONE, CNV_NOTE_AVVISI, CNV_ID_CONVOCAZIONE")

                .AddTables("T_PAZ_PAZIENTI, T_CNV_CONVOCAZIONI, T_ANA_OPERATORI, T_ANA_CONSULTORI, T_ANA_AMBULATORI")

                .AddWhereCondition("CNV_PAZ_CODICE", Comparatori.Uguale, "PAZ_CODICE", DataTypes.Join)
                .AddWhereCondition("OPE_CODICE", Comparatori.Uguale, "PAZ_MED_CODICE_BASE", DataTypes.OutJoinRight)
                .AddWhereCondition("PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("CNV_CNS_CODICE", Comparatori.Uguale, "CNS_CODICE", DataTypes.OutJoinLeft)
                .AddWhereCondition("CNV_AMB_CODICE", Comparatori.Uguale, "AMB_CODICE", DataTypes.OutJoinLeft)

                If soloConsultorioCorrente Then
                    .AddWhereCondition("CNV_CNS_CODICE", Comparatori.Uguale, codiceConsultorioCorrente, DataTypes.Stringa)
                End If

                .AddOrderByFields("CNV_DATA")
            End With

            Try
                _DAM.BuildDataTable(_DT)

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return _DT.Copy()

        End Function

        Public Function GetFiltriApertura(codiceAmbulatorio As Integer, fuoriOrario As Boolean, giorno As DayOfWeek) As DataTable Implements IPrenotazioniProvider.GetFiltriApertura

            RefurbishDT()

            With _DAM.QB

                .NewQuery()

                If Not fuoriOrario Then
                    .NewQuery()
                    .AddSelectFields("ORA_AM_INIZIO AM_INIZIO", "ORA_AM_FINE AM_FINE", "ORA_PM_INIZIO PM_INIZIO", "ORA_PM_FINE PM_FINE", "ORA_GIORNO GIORNO")
                    .AddTables("T_ANA_ORARI_APPUNTAMENTI")
                    .AddWhereCondition("ORA_AMB_CODICE", Comparatori.Uguale, codiceAmbulatorio, DataTypes.Numero)
                    .AddOrderByFields("ORA_GIORNO")
                Else
                    .NewQuery()
                    .AddSelectFields("ORG_AM_INIZIO AM_INIZIO")
                    .AddSelectFields("ORG_AM_FINE AM_FINE")
                    .AddSelectFields("ORG_PM_INIZIO PM_INIZIO")
                    .AddSelectFields("ORG_PM_FINE PM_FINE")
                    .AddSelectFields("ORG_GIORNO GIORNO")
                    .AddTables("T_ANA_ORARI_GIORNALIERI")
                    .AddWhereCondition("ORG_AMB_CODICE", Comparatori.Uguale, codiceAmbulatorio, DataTypes.Numero)
                    .AddOrderByFields("ORG_GIORNO")
                End If

                If giorno > -1 Then
                    If Not fuoriOrario Then
                        .AddWhereCondition("ORA_GIORNO", Comparatori.Uguale, giorno, DataTypes.Numero)
                    Else
                        .AddWhereCondition("ORG_GIORNO", Comparatori.Uguale, giorno, DataTypes.Numero)
                    End If
                End If

            End With

            Try
                _DAM.BuildDataTable(_DT)

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return _DT.Copy

        End Function

        Public Function GetFiltriApertura(codiceAmbulatorio As Integer, ByRef cnsControl As Boolean) As DataTable Implements IPrenotazioniProvider.GetFiltriApertura

            RefurbishDT()

            Try

                With _DAM.QB

                    .NewQuery()
                    .AddSelectFields(.FC.IsNull("ORA_AM_INIZIO", "01/01/1900 08.00.00", DataTypes.DataOra))
                    .AddSelectFields(.FC.IsNull("ORA_AM_FINE", "01/01/1900 8.00.00", DataTypes.DataOra))
                    .AddSelectFields(.FC.IsNull("ORA_PM_INIZIO", "01/01/1900 14.00.00", DataTypes.DataOra))
                    .AddSelectFields(.FC.IsNull("ORA_PM_FINE", "01/01/1900 14.00.00", DataTypes.DataOra))
                    .AddSelectFields("ORA_GIORNO GIORNO")
                    .AddTables("T_ANA_ORARI_APPUNTAMENTI")
                    .AddWhereCondition("ORA_AMB_CODICE", Comparatori.Uguale, codiceAmbulatorio, DataTypes.Stringa)
                    'DAM.QB.AddWhereCondition(ORA_GIORNO, Comparatori.Uguale, appuntamento.DayOfWeek, DataTypes.Numero)
                End With
                _DAM.BuildDataTable(_DT)

                'se gli orari degli appuntamenti non sono impostati, è necessario ricavarli dalla T_ANA_ORARI_GIORNALIERI (modifica 26/07/2004)
                If _DT.Rows.Count = 0 Then

                    RefurbishDT()

                    With _DAM.QB
                        .NewQuery()
                        .AddSelectFields(.FC.IsNull("ORG_AM_INIZIO", "01/01/1900 08.00.00", DataTypes.DataOra))
                        .AddSelectFields(.FC.IsNull("ORG_AM_FINE", "01/01/1900 8.00.00", DataTypes.DataOra))
                        .AddSelectFields(.FC.IsNull("ORG_PM_INIZIO", "01/01/1900 14.00.00", DataTypes.DataOra))
                        .AddSelectFields(.FC.IsNull("ORG_PM_FINE", "01/01/1900 14.00.00", DataTypes.DataOra))
                        .AddSelectFields("ORG_GIORNO GIORNO")
                        .AddTables("T_ANA_ORARI_GIORNALIERI")
                        .AddWhereCondition("ORG_AMB_CODICE", Comparatori.Uguale, codiceAmbulatorio, DataTypes.Stringa)
                        '.AddWhereCondition("ORG_GIORNO", Comparatori.Uguale, appuntamento.DayOfWeek, DataTypes.Numero)
                    End With

                    _DAM.BuildDataTable(_DT)

                    cnsControl = True

                End If

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return _DT.Copy

        End Function

        Public Function GetFestivita(codiceAmbulatorio As Integer) As DataTable Implements IPrenotazioniProvider.GetFestivita

            RefurbishDT()

            With _DAM.QB

                .NewQuery()
                .AddSelectFields("FES_DATA DATA", "TO_DATE(NULL) INIZIO", "TO_DATE(NULL) FINE", "TO_CHAR(FES_DATA,'YYYY') RICORSIVITA")
                .AddTables("T_ANA_FESTIVITA")
                .AddUnion(.GetSelect())

                .NewQuery(False)
                .AddSelectFields("FAS_DATA", "FAS_ORA_INIZIO", "FAS_ORA_FINE", "TO_CHAR (FAS_DATA, 'YYYY') RICORSIVITA")
                .AddTables("T_ANA_FASCE_INDISPONIBILITA")
                .AddWhereCondition("FAS_AMB_CODICE", Comparatori.Uguale, codiceAmbulatorio, DataTypes.Numero)
                .AddUnion(.GetSelect())

            End With

            Try
                _DAM.BuildDataTable(_DT)

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return _DT.Copy

        End Function

        Public Function GetPrenotati(codiceAmbulatorio As Integer, dayStart As Date, dayEnd As Date) As DataTable Implements IPrenotazioniProvider.GetPrenotati

            RefurbishDT()

            With _DAM.QB
                .NewQuery()
                .IsDistinct = True
                .AddSelectFields("PAZ_CODICE")
                .AddTables("V_BILANCI")
                .AddWhereCondition("T_CNV_CONVOCAZIONI.CNV_PAZ_CODICE", Comparatori.Uguale, "PAZ_CODICE", DataTypes.Join)
                .AddWhereCondition("T_CNV_CONVOCAZIONI.CNV_DATA", Comparatori.Uguale, "V_BILANCI.CNV_DATA", DataTypes.Join)
                Dim strQuerySoloBilancio As String = .GetSelect

                .NewQuery(False, False)
                .AddSelectFields("CNV_DATA_APPUNTAMENTO", "CNV_DATA_APPUNTAMENTO AS CNV_FINE_APPUNTAMENTO", _
                                 "CNV_DURATA_APPUNTAMENTO", "(" & strQuerySoloBilancio & ") AS PAZ_SOLO_BILANCIO",
                                 "TROVA_VACCINAZIONI (CNV_PAZ_CODICE, CNV_DATA) AS VACCINAZIONI",
                                 "CNV_AMB_CODICE AS AMB_CODICE", "T_ANA_AMBULATORI.AMB_DESCRIZIONE AS AMB_DES")
                .AddTables("T_CNV_CONVOCAZIONI")
                .AddTables("T_ANA_AMBULATORI")
                .AddWhereCondition("T_ANA_AMBULATORI.AMB_CODICE", Comparatori.Uguale, "CNV_AMB_CODICE", DataTypes.Join)
                'la ricerca deve essere effettuata sul consultorio di appuntamento specificato [modifica 09/05/2005]
                '.AddWhereCondition(CNV_CNS_CODICE, Comparatori.Uguale, p_CnsCodice, DataTypes.Stringa)
                .AddWhereCondition("CNV_AMB_CODICE", Comparatori.Uguale, codiceAmbulatorio, DataTypes.Numero)
                .AddWhereCondition("CNV_DATA_APPUNTAMENTO", Comparatori.MaggioreUguale, dayStart, DataTypes.Data)
                .AddWhereCondition("CNV_DATA_APPUNTAMENTO", Comparatori.MinoreUguale, dayEnd, DataTypes.Data)
                .AddOrderByFields("CNV_DATA_APPUNTAMENTO")
            End With

            Try
                _DAM.BuildDataTable(_DT)

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return _DT.Copy

        End Function

        Public Function GetGiorniApertura(codiceAmbulatorio As Integer) As DataTable Implements IPrenotazioniProvider.GetGiorniApertura

            RefurbishDT()

            With _DAM.QB
                .NewQuery()
                .AddSelectFields(.FC.IsNull("ORG_AM_INIZIO", "01/01/1900 08.00.00", DataTypes.DataOra) & " " & "ORG_AM_INIZIO")
                .AddSelectFields(.FC.IsNull("ORG_AM_FINE", "01/01/1900 08.00.00", DataTypes.DataOra) & " " & "ORG_AM_FINE")
                .AddSelectFields(.FC.IsNull("ORG_PM_INIZIO", "01/01/1900 14.00.00", DataTypes.DataOra) & " " & "ORG_PM_INIZIO")
                .AddSelectFields(.FC.IsNull("ORG_PM_FINE", "01/01/1900 14.00.00", DataTypes.DataOra) & " " & "ORG_PM_FINE")
                .AddWhereCondition("ORG_AMB_CODICE", Comparatori.Uguale, codiceAmbulatorio, DataTypes.Numero)
                .AddTables("T_ANA_ORARI_GIORNALIERI")
            End With

            Try
                _DAM.BuildDataTable(_DT)

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return _DT.Copy

        End Function

        Public Function GetGiorniFestivi(codiceAmbulatorio As Integer) As DataTable Implements IPrenotazioniProvider.GetGiorniFestivi

            RefurbishDT()

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("FES_DATA", "FES_DESCRIZIONE")
                .AddSelectFields(.FC.Convert(.FC.Concat("TO_CHAR(FES_DATA,'DD/MM')", "'/" & OnVac.Constants.CommonConstants.RECURSIVE_YEAR & " 00:00:00'"), "DATE", "", "dd/MM/yyyy HH24:MI:SS", "") & "INIZIO_AM")
                .AddSelectFields(.FC.Convert(.FC.Concat("TO_CHAR(FES_DATA,'DD/MM')", "'/" & OnVac.Constants.CommonConstants.RECURSIVE_YEAR & " 23:00:00'"), "DATE", "", "dd/MM/yyyy HH24:MI:SS", "") & "FINE_AM")
                .AddSelectFields(.FC.Convert(.FC.Concat("TO_CHAR(FES_DATA,'DD/MM')", "'/" & OnVac.Constants.CommonConstants.RECURSIVE_YEAR & " 00:00:00'"), "DATE", "", "dd/MM/yyyy HH24:MI:SS", "") & "INIZIO_PM")
                .AddSelectFields(.FC.Convert(.FC.Concat("TO_CHAR(FES_DATA,'DD/MM')", "'/" & OnVac.Constants.CommonConstants.RECURSIVE_YEAR & " 23:00:00'"), "DATE", "", "dd/MM/yyyy HH24:MI:SS", "") & "FINE_PM")
                .AddSelectFields("0 TIPO")
                .AddTables("T_ANA_FESTIVITA")
                .AddUnion(.GetSelect())

                .NewQuery(False)
                .AddSelectFields("FAS_DATA", "FAS_DESCRIZIONE", "FAS_ORA_INIZIO as INIZIO_AM", "FAS_ORA_FINE as FINE_AM", _
                                 "FAS_ORA_INIZIO as INIZIO_PM", "FAS_ORA_FINE as FINE_PM", "1 TIPO")
                'deve controllare le indisponibilità per il consultorio selezionato [modifica 08/07/2005]
                .AddWhereCondition("FAS_AMB_CODICE", Comparatori.Uguale, codiceAmbulatorio, DataTypes.Numero)
                .AddTables("T_ANA_FASCE_INDISPONIBILITA")
                .AddUnion(.GetSelect())
            End With

            Try
                _DAM.BuildDataTable(_DT)

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return _DT.Copy

        End Function

        Public Function ControllaOrariApertura(codiceAmbulatorio As Integer) As Boolean Implements IPrenotazioniProvider.ControllaOrariApertura

            Dim obj As Int16

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("1")
                .AddTables("T_ANA_ORARI_GIORNALIERI")
                .AddWhereCondition("ORG_AMB_CODICE", Comparatori.Uguale, codiceAmbulatorio, DataTypes.Numero)
            End With

            Try
                obj = _DAM.ExecScalar()

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            If obj > 0 Then
                Return False
            Else
                Return True
            End If

        End Function

        Public Function BuildDtFeste() As DataTable Implements IPrenotazioniProvider.BuildDtFeste

            RefurbishDT()

            With _DAM.QB
                .NewQuery()
                .AddSelectFields(.FC.Convert(.FC.Concat(.FC.SubStr(.FC.Convert("FES_DATA", "CHAR", "", "dd/MM/yyyy", ""), 1, 6), OnVac.Constants.CommonConstants.RECURSIVE_YEAR), "DATE", "", "dd/MM/yyyy", "") & " FES_DATA", "FES_DESCRIZIONE")
                .AddSelectFields(.FC.Convert("'01/01/1900 00:00'", "DATE", "", "dd/MM/yyyy HH24:MI", "") & " INIZIO")
                .AddSelectFields(.FC.Convert("'01/01/1900 23:59'", "DATE", "", "dd/MM/yyyy HH24:MI", "") & " FINE")
                .AddTables("T_ANA_FESTIVITA")
            End With

            _DAM.BuildDataTable(_DT)

            Return _DT.Copy()

        End Function

        Public Function BuildDtIndisponibilita(codiceAmbulatorio As Integer) As DataTable Implements IPrenotazioniProvider.BuildDtIndisponibilita

            RefurbishDT()

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("FAS_DATA, FAS_DESCRIZIONE, FAS_ORA_INIZIO INIZIO, FAS_ORA_FINE FINE, 'Indisponibile per ' || FAS_DESCRIZIONE || ' dalle ' || to_char(FAS_ORA_INIZIO,'hh24:mi') || ' alle ' || to_char(FAS_ORA_FINE,'hh24:mi') as PERIODO_INDISP")
                .AddTables("T_ANA_FASCE_INDISPONIBILITA")
                .AddWhereCondition("FAS_AMB_CODICE", Comparatori.Uguale, codiceAmbulatorio, DataTypes.Numero)
            End With

            _DAM.BuildDataTable(_DT)

            Return _DT.Copy
        End Function

        Public Function CaricaOrariPersonalizzati(codiceAmbulatorio As Integer) As DataTable Implements IPrenotazioniProvider.CaricaOrariPersonalizzati

            RefurbishDT()

            With _DAM.QB
                .NewQuery()
                .AddTables("T_ANA_ORARI_PERSONALIZZATI")
                .AddSelectFields("ORP_CODICE, ORP_GIORNO, ORP_ORA_INIZIO, ORP_ORA_FINE, ORP_NUM_PAZIENTI, ORP_DURATA")
                .AddSelectFields("DECODE(orp_giorno,'LUN',1,'MAR',2,'MER',3,'GIO',4,'VEN',5,'SAB',6,'DOM',7) ordine")
                .AddWhereCondition("ORP_AMB_CODICE", Comparatori.Uguale, codiceAmbulatorio, DataTypes.Numero)
                .AddOrderByFields("ordine, ORP_ORA_INIZIO")
            End With

            Try
                _DAM.BuildDataTable(_DT)
            Catch ex As Exception
                _DT = Nothing
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return _DT.Copy

        End Function

        Public Function SalvaOrariPersonalizzati(codiceAmbulatorio As Integer, ByRef dtOrariPersonalizzati As DataTable) As Boolean Implements IPrenotazioniProvider.SalvaOrariPersonalizzati

            Dim ok As Boolean = True
            Dim i As Integer

            _DAM.BeginTrans()

            ' --- Cancellazione --- '
            With _DAM.QB
                .NewQuery()
                .AddTables("T_ANA_ORARI_PERSONALIZZATI")
                .AddWhereCondition("ORP_AMB_CODICE", Comparatori.Uguale, codiceAmbulatorio, DataTypes.Numero)
            End With

            Try
                _DAM.ExecNonQuery(ExecQueryType.Delete)
                '
                ' --- Inserimento --- '
                For i = 0 To dtOrariPersonalizzati.Rows.Count - 1
                    With _DAM.QB
                        .NewQuery()
                        .AddTables("T_ANA_ORARI_PERSONALIZZATI")
                        .AddInsertField("ORP_AMB_CODICE", codiceAmbulatorio, DataTypes.Numero)
                        .AddInsertField("ORP_GIORNO", dtOrariPersonalizzati.Rows(i)("ORP_GIORNO", DataRowVersion.Current), DataTypes.Stringa)
                        .AddInsertField("ORP_ORA_INIZIO", dtOrariPersonalizzati.Rows(i)("ORP_ORA_INIZIO", DataRowVersion.Current), DataTypes.DataOra)
                        .AddInsertField("ORP_ORA_FINE", dtOrariPersonalizzati.Rows(i)("ORP_ORA_FINE", DataRowVersion.Current), DataTypes.DataOra)
                        .AddInsertField("ORP_NUM_PAZIENTI", dtOrariPersonalizzati.Rows(i)("ORP_NUM_PAZIENTI", DataRowVersion.Current), DataTypes.Numero)
                        .AddInsertField("ORP_DURATA", dtOrariPersonalizzati.Rows(i)("ORP_DURATA", DataRowVersion.Current), DataTypes.Numero)
                    End With

                    _DAM.ExecNonQuery(ExecQueryType.Insert)

                Next
                _DAM.Commit()
                '
            Catch ex As Exception
                _DAM.Rollback()
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return ok

        End Function

        Public Function CercaConvocazioni(codiceConsultorio As List(Of String), durataSedutaBilancioDefault As Integer, maxNumSollDefault As Integer, validitaPerSoloBilancio As Integer, ByRef filtri As FiltriGestioneAppuntamenti, usaOrdineAlfabetico As Boolean) As DataTable Implements IPrenotazioniProvider.CercaConvocazioni

            If codiceConsultorio Is Nothing OrElse codiceConsultorio.Count = 0 Then
                Throw New ApplicationException("Impossibile richiamare il metodo CercaConvocazioni senza almeno un consultorio impostato")
            End If

            RefurbishDT()

            ' --------------------------- QUERY GESTIONE APPUNTAMENTI --------------------------- '

            With _DAM.QB

                ' --------- Query Malattia specificata nella modale --------- '
                ' Se è stata specificata una malattia per cui filtrare, creo la sottoquery che userò nei filtri
                Dim strQueryMalattia As String = ""

                If filtri.malattia <> "" Then

                    ' Tabella malattie del paziente 
                    .NewQuery(False, False)
                    .AddSelectFields("1")
                    .AddTables("T_PAZ_MALATTIE")
                    .AddWhereCondition("PMA_MAL_CODICE", Comparatori.Uguale, filtri.malattia, DataTypes.Stringa)
                    .AddWhereCondition("PAZ_CODICE", Comparatori.Uguale, "PMA_PAZ_CODICE", DataTypes.OutJoinLeft)

                    strQueryMalattia = .GetSelect

                End If

                ' --------- Query Vaccinazioni specificate nei check --------- '
                ' Se sono state specificate vaccinazioni per cui filtrare, creo la sottoquery che userò nei filtri
                Dim strQueryTipoVaccinazioni As String = ""
                '
                If filtri.chkObb Or filtri.chkFac Or filtri.chkRac Then

                    ' Vado nelle tabelle vac_programmate e anagrafica_vac per il tipo di vac
                    Dim strTipoVac As New System.Text.StringBuilder

                    .NewQuery(False, False)

                    If filtri.chkObb Then
                        strTipoVac.AppendFormat("{0},", .AddCustomParam("A"))
                    End If
                    If filtri.chkFac Then
                        strTipoVac.AppendFormat("{0},", .AddCustomParam("C"))
                    End If
                    If filtri.chkRac Then
                        strTipoVac.AppendFormat("{0},", .AddCustomParam("B"))
                    End If
                    strTipoVac.Remove(strTipoVac.Length - 1, 1)
                    '
                    .AddSelectFields("1")
                    .AddTables("T_VAC_PROGRAMMATE, T_ANA_VACCINAZIONI")
                    .AddWhereCondition("CNV_DATA", Comparatori.Uguale, "VPR_CNV_DATA", DataTypes.Join)
                    .AddWhereCondition("PAZ_CODICE", Comparatori.Uguale, "VPR_PAZ_CODICE", DataTypes.Join)
                    .AddWhereCondition("VPR_VAC_CODICE", Comparatori.Uguale, "VAC_CODICE", DataTypes.Join)
                    .AddWhereCondition("VAC_OBBLIGATORIA", Comparatori.In, strTipoVac.ToString, DataTypes.Replace)

                    strQueryTipoVaccinazioni = .GetSelect

                End If

                ' ------------------------------------------------------------------------ 

                ' --------- Query Vaccinazioni Esluse  --------- '
                Dim strQueryExistsVaccinazioniProgrammataNonEsclusa As String = ""

                If Not filtri.vaccinazioniTutteEscluse Then

                    .NewQuery(False, False)
                    .AddSelectFields("1")
                    .AddTables("T_VAC_PROGRAMMATE", "T_VAC_ESCLUSE")
                    .AddWhereCondition("CNV_DATA", Comparatori.Uguale, "VPR_CNV_DATA", DataTypes.Join)
                    .AddWhereCondition("PAZ_CODICE", Comparatori.Uguale, "VPR_PAZ_CODICE", DataTypes.Join)
                    .AddWhereCondition("VPR_VAC_CODICE", Comparatori.Uguale, "VEX_VAC_CODICE", DataTypes.OutJoinLeft)
                    .AddWhereCondition("VPR_PAZ_CODICE", Comparatori.Uguale, "VEX_PAZ_CODICE", DataTypes.OutJoinLeft)
                    .OpenParanthesis()
                    .AddWhereCondition("VEX_PAZ_CODICE", Comparatori.Is, "NULL", DataTypes.Replace)
                    .AddWhereCondition("VEX_DATA_SCADENZA", Comparatori.MinoreUguale, DateTime.Now.Date, DataTypes.Data, "OR")
                    .CloseParanthesis()

                    strQueryExistsVaccinazioniProgrammataNonEsclusa = .GetSelect

                End If
                ' -------------------------------------------------

                .NewQuery(False, False)

                .IsDistinct = True

                ' --- Select Fields --- '
                .AddSelectFields("PAZ_CODICE, PAZ_COGNOME, PAZ_NOME, PAZ_DATA_NASCITA, PAZ_STATO")
                .AddSelectFields("DECODE(p1.TIPO_EXTRACOMUNITARI,1,0,p1.TIPO_IMMI_NON_EXTRA_PRIMA) TIPO_IMMI_NON_EXTRA_PRIMA, p1.TIPO_EXTRACOMUNITARI TIPO_EXTRACOMUNITARI")
                .AddSelectFields("CNV_DATA, MED_DESCRIZIONE, CNV_CNS_CODICE, VACCINAZIONI, ASSOCIAZIONI, SEDUTE, DOSI, CICLI, CNV_DATA_APPUNTAMENTO, CNV_TIPO_APPUNTAMENTO, PAZ_STATO_ANAGRAFICO, COMUNE_RESIDENZA, COMUNE_DOMICILIO")
                .AddSelectFields("SEL")

                If durataSedutaBilancioDefault > 0 Then
                    .AddSelectFields("decode(decode(med_tipo,2,'SP','NP')|| decode(min(bip_bil_numero),0,'NB','SB')" &
                    ",'SBNP'," & durataSedutaBilancioDefault & ",CNV_DURATA_APPUNTAMENTO) CNV_DURATA_APPUNTAMENTO")
                Else
                    .AddSelectFields("CNV_DURATA_APPUNTAMENTO")
                End If

                .AddSelectFields("MIN(decode(" + .FC.IsNull("CNC_N_SOLLECITO", 0, DataTypes.Numero) + ", 0, CNV_DATA,SYSDATE) + decode(TSD_VALIDITA,NULL," & validitaPerSoloBilancio & ",TSD_VALIDITA)) MASSIMO")
                .AddSelectFields("NVL(MAX(CNC_N_SOLLECITO), 0) SOLLECITO")

                .AddSelectFields("PAZ_GIORNO, PAZ_ETA_GIORNI, MED_TIPO", "decode(decode(min(bip_bil_numero),NULL,'NB',0,'NB','SB') ||  " &
                                 " decode(med_tipo,2,'SP','NP'),'SBNP','S','N') TEMPO_BIL ", "CRONICO")
                .AddSelectFields("DECODE(VACCINAZIONI,NULL,1,'',1,NULL) SOLOBILANCIO")
                .AddSelectFields(String.Format("trova_termine_perentorio (paz_codice, cnv_data,{0}) TP", maxNumSollDefault))

                ' --- Tabella   --- '
                .AddTables("v_paz_da_convocare p1")

                ' --- Condizioni di filtro --- '
                .AddWhereCondition("", Comparatori.NotExist, String.Format("(SELECT 1 FROM T_VIS_VISITE WHERE VIS_PAZ_CODICE = PAZ_CODICE AND VIS_FINE_SOSPENSIONE IS NOT NULL AND VIS_FINE_SOSPENSIONE >= {0})", .AddCustomParam(DateTime.Now.Date)), DataTypes.Replace)

                ' filtri immigrati
                If filtri.chkExtracom And filtri.chkNonExtracomPrima Then
                    .OpenParanthesis()
                    .AddWhereCondition("DECODE(p1.TIPO_EXTRACOMUNITARI,1,0,p1.TIPO_IMMI_NON_EXTRA_PRIMA)", Comparatori.Maggiore, "0", DataTypes.Numero)
                    .AddWhereCondition("p1.TIPO_EXTRACOMUNITARI", Comparatori.Maggiore, "0", DataTypes.Numero, "OR")
                    .CloseParanthesis()
                ElseIf filtri.chkExtracom Then
                    .AddWhereCondition("p1.TIPO_EXTRACOMUNITARI", Comparatori.Maggiore, "0", DataTypes.Numero)
                ElseIf filtri.chkNonExtracomPrima Then
                    .AddWhereCondition("DECODE(p1.TIPO_EXTRACOMUNITARI,1,0,p1.TIPO_IMMI_NON_EXTRA_PRIMA)", Comparatori.Maggiore, "0", DataTypes.Numero)
                End If

                ' Cronici
                If filtri.chkCronici Then
                    .AddWhereCondition("CRONICO", Comparatori.Uguale, "C", DataTypes.Stringa)
                End If

                ' Data di nascita compresa nel periodo specificato
                If filtri.DaDataNascita <> Nothing Then
                    .AddWhereCondition("PAZ_DATA_NASCITA", Comparatori.MaggioreUguale, filtri.DaDataNascita, DataTypes.Data)
                    If filtri.ADataNascita <> Nothing Then
                        .AddWhereCondition("PAZ_DATA_NASCITA", Comparatori.MinoreUguale, filtri.ADataNascita, DataTypes.Data)
                    End If
                End If

                ' Se nella maschera è stato spuntato il check "Data Singola", la data di convocazione 
                ' del paziente deve essere quella specificata, altrimenti deve essere minore o uguale.
                If filtri.chkDataSingola Then
                    .AddWhereCondition("CNV_DATA", Comparatori.Uguale, filtri.FinoAData, DataTypes.Data)
                Else
                    .AddWhereCondition("CNV_DATA", Comparatori.MinoreUguale, filtri.FinoAData, DataTypes.Data)
                End If

                ' Filtro sui centri vaccinali
                Dim pString As New System.Text.StringBuilder
                For i As Integer = 0 To codiceConsultorio.Count - 1
                    pString.AppendFormat("{0},", .AddCustomParam(codiceConsultorio(i)))
                Next
                If pString.ToString.Length > 1 Then
                    pString.Remove(pString.ToString.Length - 1, 1)
                End If
                .AddWhereCondition("CNV_CNS_CODICE", Comparatori.In, pString.ToString(), DataTypes.Replace)

                ' Medico    
                If filtri.fmMedCodice <> "" Then
                    .AddWhereCondition("paz_med_codice_base", Comparatori.Uguale, filtri.fmMedCodice, DataTypes.Stringa)
                End If

                ' Stato anagrafico tra quelli specificati nella maschera
                If Not filtri.StatiAnagrafici Is Nothing AndAlso filtri.StatiAnagrafici.Count > 0 Then
                    .AddWhereCondition("PAZ_STATO_ANAGRAFICO", Comparatori.In,
                                       String.Format("'{0}'", filtri.StatiAnagrafici.Aggregate(Function(p, g) p & "','" & g)),
                                       DataTypes.Replace)
                End If

                ' Categoria a rischio specificata nella modale
                If filtri.categ_rischio <> "" Then
                    .AddWhereCondition("paz_rsc_codice", Comparatori.Uguale, filtri.categ_rischio, DataTypes.Stringa)
                End If

                ' Malattia specificata nella modale
                If strQueryMalattia <> "" Then
                    .AddWhereCondition("", Comparatori.Exist, "(" + strQueryMalattia + ")", DataTypes.Replace)
                End If


                If strQueryTipoVaccinazioni <> "" Then
                    .AddWhereCondition("", Comparatori.Exist, "(" + strQueryTipoVaccinazioni + ")", DataTypes.Replace)
                End If

                ' Aggiunto filtro per sesso del paziente
                If filtri.sesso <> "" Then
                    .AddWhereCondition("PAZ_SESSO", Comparatori.Uguale, filtri.sesso, DataTypes.Stringa)
                End If

                ' Aggiunto filtro per comune di domicilio se valorizzato, altrimenti di residenza del paziente
                If filtri.codiceComune <> "" Then
                    .AddWhereCondition(.FC.IsNull("PAZ_COM_CODICE_DOMICILIO", "PAZ_COM_CODICE_RESIDENZA", DataTypes.Replace), Comparatori.Uguale, filtri.codiceComune, DataTypes.Stringa)
                End If

                ' Vaccinazioni Escluse
                If Not filtri.vaccinazioniTutteEscluse Then
                    .OpenParanthesis()
                    .AddWhereCondition("", Comparatori.Exist, String.Format("({0})", strQueryExistsVaccinazioniProgrammataNonEsclusa), DataTypes.Replace)
                    '.OpenParanthesis()
                    .AddWhereCondition("bip_bil_numero", Comparatori.IsNot, "NULL", DataTypes.Replace, "OR")
                    'TODO: CERCA CONVOCAZIONI => bip_stato ????
                    '.AddWhereCondition("bip_stato", Comparatori.Diverso, , DataTypes.Stringa)
                    '.CloseParanthesis()
                    .CloseParanthesis()
                End If

                ' --- Group by --- '
                .AddGroupByFields("PAZ_CODICE, PAZ_COGNOME, PAZ_NOME, PAZ_DATA_NASCITA, PAZ_STATO, CNV_DATA, MED_DESCRIZIONE")
                .AddGroupByFields("CNV_CNS_CODICE, VACCINAZIONI, ASSOCIAZIONI, SEDUTE, DOSI, CICLI, CNV_DATA_APPUNTAMENTO")
                .AddGroupByFields("CNV_TIPO_APPUNTAMENTO, PAZ_STATO_ANAGRAFICO, COMUNE_RESIDENZA, COMUNE_DOMICILIO")
                .AddGroupByFields("SEL, CNV_DURATA_APPUNTAMENTO, PAZ_GIORNO, PAZ_ETA_GIORNI")
                .AddGroupByFields("TIPO_IMMI_NON_EXTRA_PRIMA, TIPO_EXTRACOMUNITARI, MED_TIPO, CRONICO")

                ' --------- Filtro Solo Solleciti --------- '
                ' Per gestire i ritardatari direttamente dai solleciti invece che dallo stato vaccinale
                If filtri.chkSoloRitardatari Then
                    .AddHavingFields("MAX(CNC_N_SOLLECITO) > 0")
                End If

                ' --- Order by --- '
                .AddOrderByFields("CNV_DATA")
                If usaOrdineAlfabetico Then
                    .AddOrderByFields("PAZ_COGNOME, PAZ_NOME")
                End If
                .AddOrderByFields("PAZ_DATA_NASCITA")

            End With

            ' Creazione del datatable
            _DAM.BuildDataTable(_DT)

            ' Impostazione della chiave primaria del datatable
            _DT.PrimaryKey = New DataColumn() {_DT.Columns("PAZ_CODICE"), _DT.Columns("CNV_DATA")}

            Return _DT.Copy

        End Function

        Public Function UltimaDataCnvConsultorio(codiceConsultorio As String) As String Implements IPrenotazioniProvider.UltimaDataCnvConsultorio

            Dim result As String

            With _DAM.QB
                .NewQuery()
                .AddTables("T_ANA_CONSULTORI")
                .AddSelectFields("CNS_CERCA_DCNV")
                .AddWhereCondition("CNS_CODICE", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)
            End With

            Try
                result = _DAM.ExecScalar() & ""
            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return result

        End Function

        Public Sub SalvaUltimaDataCnvConsultorio(codiceConsultorio As String, data As Date) Implements IPrenotazioniProvider.SalvaUltimaDataCnvConsultorio

            With _DAM.QB
                .NewQuery()
                .AddTables("T_ANA_CONSULTORI")
                .AddUpdateField("CNS_CERCA_DCNV", data, DataTypes.Data)
                .AddWhereCondition("CNS_CODICE", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)
            End With

            Try
                _DAM.ExecNonQuery(ExecQueryType.Update)
            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

        End Sub

        Public Sub FillDtMattino(dtMattino As DataTable, codiceAmbulatorio As Integer, data As Date, ByRef oraAMPM As DateTime, maxNumSollDefault As Integer) Implements IPrenotazioniProvider.FillDtMattino

            Me.FillDt_MattinoPomeriggio(dtMattino, True, codiceAmbulatorio, data, oraAMPM, Nothing, maxNumSollDefault)

        End Sub

        Public Sub FillDtPomeriggio(dtPomeriggio As DataTable, codiceAmbulatorio As Integer, data As Date, ByRef oraMinPM As DateTime, maxNumSollDefault As Integer) Implements IPrenotazioniProvider.FillDtPomeriggio

            Me.FillDt_MattinoPomeriggio(dtPomeriggio, False, codiceAmbulatorio, data, Nothing, oraMinPM, maxNumSollDefault)

        End Sub

        Private Sub FillDt_MattinoPomeriggio(ByRef dt As DataTable, mattino As Boolean, codiceAmbulatorio As Integer,
                                             data As Date, ByRef oraAMPM As DateTime, ByRef oraMinPM As DateTime, maxNumSollDefault As Integer)

            _DAM.ClearParam()

            _DAM.AddParameter("max_num_soll_default", maxNumSollDefault)
            _DAM.AddParameter("amb_codice", codiceAmbulatorio)

            If mattino Then
                _DAM.AddParameter("data_inizio", New Date(data.Year, data.Month, data.Day))
                _DAM.AddParameter("data_fine", New Date(data.Year, data.Month, data.Day, oraAMPM.Hour, oraAMPM.Minute, 0))
            Else
                _DAM.AddParameter("data_inizio", New Date(data.Year, data.Month, data.Day, oraMinPM.Hour, oraMinPM.Minute, 0))
                _DAM.AddParameter("data_fine", New Date(data.Year, data.Month, data.Day, 23, 59, 59))
            End If

            Try
                _DAM.BuildDataTable(OnVac.Queries.VaccinazioniProgrammate.OracleQueries.selAppuntamentiMattinoPomeriggio, dt)
            Catch ex As Exception
                dt = Nothing
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

        End Sub

        Public Function GetAppuntamenti(codiceAmbulatorio As Integer) As DataTable Implements IPrenotazioniProvider.GetAppuntamenti

            RefurbishDT()

            Try
                _DAM.ClearParam()
                _DAM.AddParameter("amb_codice", codiceAmbulatorio)
                _DAM.BuildDataTable(OnVac.Queries.VaccinazioniProgrammate.OracleQueries.selCountProgrammateAmbulatorio, _DT)
            Catch ex As Exception
                _DT = Nothing
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return _DT.Copy()

        End Function

        ''' <summary>
        ''' Restituisce la data del primo appuntamento per la convocazione specificata.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataConvocazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDataPrimoAppuntamento(codicePaziente As Long, dataConvocazione As DateTime) As DateTime? Implements IPrenotazioniProvider.GetDataPrimoAppuntamento

            Dim dataPrimoAppuntamento As DateTime? = Nothing

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.CommandText = "select CNV_PRIMO_APPUNTAMENTO from T_CNV_CONVOCAZIONI where CNV_PAZ_CODICE = :CNV_PAZ_CODICE and CNV_DATA = :CNV_DATA"
                    cmd.Parameters.AddWithValue("CNV_PAZ_CODICE", codicePaziente)
                    cmd.Parameters.AddWithValue("CNV_DATA", dataConvocazione)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        dataPrimoAppuntamento = Convert.ToDateTime(obj)
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return dataPrimoAppuntamento

        End Function

        ''' <summary>
        ''' Salvataggio data primo appuntamento per la convocazione specificata.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataConvocazione"></param>
        ''' <param name="dataPrimoAppuntamento"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateDataPrimoAppuntamento(codicePaziente As Long, dataConvocazione As DateTime, dataPrimoAppuntamento As DateTime) As Integer Implements IPrenotazioniProvider.UpdateDataPrimoAppuntamento

            Dim count As Integer = 0

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.CommandText = "update T_CNV_CONVOCAZIONI set CNV_PRIMO_APPUNTAMENTO = :CNV_PRIMO_APPUNTAMENTO where CNV_PAZ_CODICE = :CNV_PAZ_CODICE and CNV_DATA = :CNV_DATA"
                    cmd.Parameters.AddWithValue("CNV_PRIMO_APPUNTAMENTO", dataPrimoAppuntamento)
                    cmd.Parameters.AddWithValue("CNV_PAZ_CODICE", codicePaziente)
                    cmd.Parameters.AddWithValue("CNV_DATA", dataConvocazione)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Update dei dati di appuntamento relativi alla convocazione specificata.
        ''' </summary>
        ''' <param name="convocazioneAppuntamento"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateConvocazione_DatiAppuntamento(convocazioneAppuntamento As Entities.ConvocazioneAppuntamento) As Integer Implements IPrenotazioniProvider.UpdateConvocazione_DatiAppuntamento

            With _DAM.QB

                .NewQuery()

                .AddTables("T_CNV_CONVOCAZIONI")

                .AddWhereCondition("CNV_PAZ_CODICE", Comparatori.Uguale, convocazioneAppuntamento.CodicePaziente, DataTypes.Numero)
                .AddWhereCondition("CNV_DATA", Comparatori.Uguale, convocazioneAppuntamento.DataConvocazione, DataTypes.Data)

                If convocazioneAppuntamento.DataAppuntamento.HasValue Then
                    .AddUpdateField("CNV_DATA_APPUNTAMENTO", convocazioneAppuntamento.DataAppuntamento.Value, DataTypes.DataOra)
                Else
                    .AddUpdateField("CNV_DATA_APPUNTAMENTO", "NULL", DataTypes.Replace)
                End If

                If convocazioneAppuntamento.IdUtenteRegistrazioneAppuntamento.HasValue Then
                    .AddUpdateField("CNV_UTE_ID", convocazioneAppuntamento.IdUtenteRegistrazioneAppuntamento.Value, DataTypes.Numero)
                Else
                    .AddUpdateField("CNV_UTE_ID", "NULL", DataTypes.Replace)
                End If

                ' N.B. : se la data del primo appuntamento nella entity è nulla, non devo impostarla a null su db perchè, se c'era già, non va cancellata!
                'If convocazioneAppuntamento.DataPrimoAppuntamento.HasValue Then
                '    .AddUpdateField("CNV_PRIMO_APPUNTAMENTO", convocazioneAppuntamento.DataPrimoAppuntamento.Value, DataTypes.DataOra)
                'End If

                If convocazioneAppuntamento.DataInvio.HasValue Then
                    .AddUpdateField("CNV_DATA_INVIO", convocazioneAppuntamento.DataInvio.Value, DataTypes.DataOra)
                Else
                    .AddUpdateField("CNV_DATA_INVIO", "NULL", DataTypes.Replace)
                End If


                If Not String.IsNullOrWhiteSpace(convocazioneAppuntamento.CodiceConsultorio) Then
                    .AddUpdateField("CNV_CNS_CODICE", convocazioneAppuntamento.CodiceConsultorio, DataTypes.Stringa)
                End If

                If convocazioneAppuntamento.CodiceAmbulatorio.HasValue Then
                    .AddUpdateField("CNV_AMB_CODICE", convocazioneAppuntamento.CodiceAmbulatorio.Value, DataTypes.Numero)
                Else
                    .AddUpdateField("CNV_AMB_CODICE", "NULL", DataTypes.Replace)
                End If

                If String.IsNullOrWhiteSpace(convocazioneAppuntamento.TipoAppuntamento) Then
                    .AddUpdateField("CNV_TIPO_APPUNTAMENTO", "NULL", DataTypes.Replace)
                Else
                    .AddUpdateField("CNV_TIPO_APPUNTAMENTO", convocazioneAppuntamento.TipoAppuntamento, DataTypes.Stringa)
                End If

                If String.IsNullOrWhiteSpace(convocazioneAppuntamento.NoteModificaAppuntamento) Then
                    .AddUpdateField("CNV_NOTE_MODIFICA_APPUNTAMENTO", "NULL", DataTypes.Replace)
                Else
                    .AddUpdateField("CNV_NOTE_MODIFICA_APPUNTAMENTO", convocazioneAppuntamento.NoteModificaAppuntamento, DataTypes.Stringa)
                End If

                If String.IsNullOrWhiteSpace(convocazioneAppuntamento.NoteAvvisi) Then
                    .AddUpdateField("CNV_NOTE_AVVISI", "NULL", DataTypes.Replace)
                Else
                    .AddUpdateField("CNV_NOTE_AVVISI", convocazioneAppuntamento.NoteAvvisi, DataTypes.Stringa)
                End If

                If convocazioneAppuntamento.DurataAppuntamento.HasValue Then
                    .AddUpdateField("CNV_DURATA_APPUNTAMENTO", convocazioneAppuntamento.DurataAppuntamento.Value, DataTypes.Numero)
                End If

            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Update)

        End Function

        ''' <summary>
        ''' Restituisce il valore dell'id preso dalla sequence, per l'inserimento.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetNextIdStoricoAppuntamenti() As Long Implements IPrenotazioniProvider.GetNextIdStoricoAppuntamenti

            Dim id As Long = 0

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.CommandText = "select SEQ_CNA_ID_APPUNTAMENTI.nextval from dual"

                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        id = Convert.ToInt64(obj)
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return id

        End Function

        ''' <summary>
        ''' Inserimento in tabella t_cnv_appuntamenti
        ''' </summary>
        ''' <param name="convocazioneAppuntamento"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertStoricoAppuntamenti(convocazioneAppuntamento As Entities.ConvocazioneAppuntamento) As Integer Implements IPrenotazioniProvider.InsertStoricoAppuntamenti

            Dim count As Integer = 0

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.CommandText = "insert into T_CNV_APPUNTAMENTI " +
                        " (CNA_ID, CNA_PAZ_CODICE, CNA_CNV_DATA, CNA_VACCINAZIONI, CNA_CNS_CODICE, CNA_AMB_CODICE, CNA_DATA_APPUNTAMENTO, CNA_DURATA_APPUNTAMENTO, " +
                        "  CNA_ETA_POMERIGGIO, CNA_TIPO_APPUNTAMENTO, CNA_DATA_INVIO, CNA_UTE_ID_INVIO, CNA_NOTE, CNA_NOTE_AVVISI, CNA_NOTE_MODIFICA_APPUNTAMENTO ,CNA_PAZ_CODICE_OLD, CNA_PRIMO_APPUNTAMENTO, " +
                        "  CNA_CAMPAGNA, CNA_DATA_REGISTRAZIONE_APP, CNA_UTE_ID_REGISTRAZIONE_APP, CNA_DATA_ELIMINAZIONE, CNA_UTE_ID_ELIMINAZIONE, CNA_MEA_ID_MOTIVO_ELIMINAZIONE) " +
                        " values (" +
                        " :CNA_ID, :CNA_PAZ_CODICE, :CNA_CNV_DATA, :CNA_VACCINAZIONI, :CNA_CNS_CODICE, :CNA_AMB_CODICE, :CNA_DATA_APPUNTAMENTO, :CNA_DURATA_APPUNTAMENTO, " +
                        " :CNA_ETA_POMERIGGIO, :CNA_TIPO_APPUNTAMENTO, :CNA_DATA_INVIO, :CNA_UTE_ID_INVIO, :CNA_NOTE, :CNA_NOTE_AVVISI, :CNA_NOTE_MODIFICA_APPUNTAMENTO, :CNA_PAZ_CODICE_OLD, :CNA_PRIMO_APPUNTAMENTO, " +
                        " :CNA_CAMPAGNA, :CNA_DATA_REGISTRAZIONE_APP, :CNA_UTE_ID_REGISTRAZIONE_APP, :CNA_DATA_ELIMINAZIONE, :CNA_UTE_ID_ELIMINAZIONE, :CNA_MEA_ID_MOTIVO_ELIMINAZIONE) "

                    cmd.Parameters.AddWithValue("CNA_ID", convocazioneAppuntamento.Id)
                    cmd.Parameters.AddWithValue("CNA_PAZ_CODICE", convocazioneAppuntamento.CodicePaziente)
                    cmd.Parameters.AddWithValue("CNA_CNV_DATA", convocazioneAppuntamento.DataConvocazione)
                    cmd.Parameters.AddWithValue("CNA_VACCINAZIONI", GetStringParam(convocazioneAppuntamento.Vaccinazioni))
                    cmd.Parameters.AddWithValue("CNA_CNS_CODICE", GetStringParam(convocazioneAppuntamento.CodiceConsultorio))

                    If convocazioneAppuntamento.CodiceAmbulatorio.HasValue Then
                        cmd.Parameters.AddWithValue("CNA_AMB_CODICE", convocazioneAppuntamento.CodiceAmbulatorio.Value)
                    Else
                        cmd.Parameters.AddWithValue("CNA_AMB_CODICE", DBNull.Value)
                    End If

                    If convocazioneAppuntamento.DataAppuntamento.HasValue Then
                        cmd.Parameters.AddWithValue("CNA_DATA_APPUNTAMENTO", convocazioneAppuntamento.DataAppuntamento.Value)
                    Else
                        cmd.Parameters.AddWithValue("CNA_DATA_APPUNTAMENTO", DBNull.Value)
                    End If

                    If convocazioneAppuntamento.DurataAppuntamento.HasValue Then
                        cmd.Parameters.AddWithValue("CNA_DURATA_APPUNTAMENTO", convocazioneAppuntamento.DurataAppuntamento.Value)
                    Else
                        cmd.Parameters.AddWithValue("CNA_DURATA_APPUNTAMENTO", DBNull.Value)
                    End If

                    cmd.Parameters.AddWithValue("CNA_ETA_POMERIGGIO", GetStringParam(convocazioneAppuntamento.EtaPomeriggio))
                    cmd.Parameters.AddWithValue("CNA_TIPO_APPUNTAMENTO", GetStringParam(convocazioneAppuntamento.TipoAppuntamento))

                    If convocazioneAppuntamento.DataInvio.HasValue Then
                        cmd.Parameters.AddWithValue("CNA_DATA_INVIO", convocazioneAppuntamento.DataInvio.Value)
                    Else
                        cmd.Parameters.AddWithValue("CNA_DATA_INVIO", DBNull.Value)
                    End If

                    If convocazioneAppuntamento.IdUtenteInvio.HasValue Then
                        cmd.Parameters.AddWithValue("CNA_UTE_ID_INVIO", convocazioneAppuntamento.IdUtenteInvio.Value)
                    Else
                        cmd.Parameters.AddWithValue("CNA_UTE_ID_INVIO", DBNull.Value)
                    End If

                    cmd.Parameters.AddWithValue("CNA_NOTE", GetStringParam(convocazioneAppuntamento.Note))
                    cmd.Parameters.AddWithValue("CNA_NOTE_AVVISI", GetStringParam(convocazioneAppuntamento.NoteAvvisi))
                    cmd.Parameters.AddWithValue("CNA_NOTE_MODIFICA_APPUNTAMENTO", GetStringParam(convocazioneAppuntamento.NoteAvvisi))

                    If convocazioneAppuntamento.IdUtenteInvio.HasValue Then
                        cmd.Parameters.AddWithValue("CNA_PAZ_CODICE_OLD", convocazioneAppuntamento.IdUtenteInvio.Value)
                    Else
                        cmd.Parameters.AddWithValue("CNA_PAZ_CODICE_OLD", DBNull.Value)
                    End If

                    If convocazioneAppuntamento.DataPrimoAppuntamento.HasValue Then
                        cmd.Parameters.AddWithValue("CNA_PRIMO_APPUNTAMENTO", convocazioneAppuntamento.DataPrimoAppuntamento.Value)
                    Else
                        cmd.Parameters.AddWithValue("CNA_PRIMO_APPUNTAMENTO", DBNull.Value)
                    End If

                    cmd.Parameters.AddWithValue("CNA_CAMPAGNA", GetStringParam(convocazioneAppuntamento.Campagna))

                    If convocazioneAppuntamento.DataRegistrazioneAppuntamento.HasValue Then
                        cmd.Parameters.AddWithValue("CNA_DATA_REGISTRAZIONE_APP", convocazioneAppuntamento.DataRegistrazioneAppuntamento.Value)
                    Else
                        cmd.Parameters.AddWithValue("CNA_DATA_REGISTRAZIONE_APP", DBNull.Value)
                    End If

                    If convocazioneAppuntamento.IdUtenteRegistrazioneAppuntamento.HasValue Then
                        cmd.Parameters.AddWithValue("CNA_UTE_ID_REGISTRAZIONE_APP", convocazioneAppuntamento.IdUtenteRegistrazioneAppuntamento.Value)
                    Else
                        cmd.Parameters.AddWithValue("CNA_UTE_ID_REGISTRAZIONE_APP", DBNull.Value)
                    End If

                    If convocazioneAppuntamento.DataEliminazioneAppuntamento.HasValue Then
                        cmd.Parameters.AddWithValue("CNA_DATA_ELIMINAZIONE", convocazioneAppuntamento.DataEliminazioneAppuntamento.Value)
                    Else
                        cmd.Parameters.AddWithValue("CNA_DATA_ELIMINAZIONE", DBNull.Value)
                    End If

                    If convocazioneAppuntamento.IdUtenteEliminazioneAppuntamento.HasValue Then
                        cmd.Parameters.AddWithValue("CNA_UTE_ID_ELIMINAZIONE", convocazioneAppuntamento.IdUtenteEliminazioneAppuntamento.Value)
                    Else
                        cmd.Parameters.AddWithValue("CNA_UTE_ID_ELIMINAZIONE", DBNull.Value)
                    End If

                    cmd.Parameters.AddWithValue("CNA_MEA_ID_MOTIVO_ELIMINAZIONE", GetStringParam(convocazioneAppuntamento.IdMotivoEliminazioneAppuntamento))

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Restituisce lo storico più recente (in base all'id), relativamente alla convocazione specificata.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataConvocazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetLastIdStoricoAppuntamenti(codicePaziente As Long, dataConvocazione As DateTime) As Long? Implements IPrenotazioniProvider.GetLastIdStoricoAppuntamenti

            Dim idConvocazioneAppuntamento As Long? = Nothing

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.CommandText =
                        " select max(CNA_ID) from T_CNV_APPUNTAMENTI " +
                        " where CNA_PAZ_CODICE = :CNA_PAZ_CODICE " +
                        " and CNA_CNV_DATA = :CNA_CNV_DATA"

                    cmd.Parameters.AddWithValue("CNA_PAZ_CODICE", codicePaziente)
                    cmd.Parameters.AddWithValue("CNA_CNV_DATA", dataConvocazione)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        idConvocazioneAppuntamento = Convert.ToInt64(obj)
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return idConvocazioneAppuntamento

        End Function

        ''' <summary>
        ''' Update dati di eliminazione per lo storico specificato (in base all'id).
        ''' </summary>
        ''' <param name="idConvocazioneAppuntamento"></param>
        ''' <param name="idUtenteEliminazione"></param>
        ''' <param name="dataEliminazione"></param>
        ''' <param name="motivoEliminazione"></param>
        ''' <param name="noteEliminazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateStoricoAppuntamenti_Eliminazione(idConvocazioneAppuntamento As Long, idUtenteEliminazione As Long, dataEliminazione As DateTime, motivoEliminazione As String, noteEliminazione As String, noteModificaAppuntamento As String) As Integer Implements IPrenotazioniProvider.UpdateStoricoAppuntamenti_Eliminazione

            Dim count As Integer = 0

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.CommandText = "update T_CNV_APPUNTAMENTI " +
                        " set CNA_DATA_ELIMINAZIONE = :CNA_DATA_ELIMINAZIONE, " +
                        " CNA_UTE_ID_ELIMINAZIONE = :CNA_UTE_ID_ELIMINAZIONE, " +
                        " CNA_MEA_ID_MOTIVO_ELIMINAZIONE = :CNA_MEA_ID_MOTIVO_ELIMINAZIONE, " +
                        " CNA_NOTE_MODIFICA_APPUNTAMENTO = :CNA_NOTE_MODIFICA_APPUNTAMENTO, " +
                        " CNA_NOTE = :CNA_NOTE " +
                        " where CNA_ID = :CNA_ID"

                    cmd.Parameters.AddWithValue("CNA_DATA_ELIMINAZIONE", dataEliminazione)
                    cmd.Parameters.AddWithValue("CNA_UTE_ID_ELIMINAZIONE", idUtenteEliminazione)
                    cmd.Parameters.AddWithValue("CNA_MEA_ID_MOTIVO_ELIMINAZIONE", motivoEliminazione)

                    If String.IsNullOrWhiteSpace(noteEliminazione) Then
                        cmd.Parameters.AddWithValue("CNA_NOTE", DBNull.Value)
                    Else
                        cmd.Parameters.AddWithValue("CNA_NOTE", noteEliminazione)
                    End If

                    If String.IsNullOrWhiteSpace(noteModificaAppuntamento) Then
                        cmd.Parameters.AddWithValue("CNA_NOTE_MODIFICA_APPUNTAMENTO", DBNull.Value)
                    Else
                        cmd.Parameters.AddWithValue("CNA_NOTE_MODIFICA_APPUNTAMENTO", noteModificaAppuntamento)
                    End If

                    cmd.Parameters.AddWithValue("CNA_ID", idConvocazioneAppuntamento)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Valorizza la data e l'utente di invio nello storico appuntamenti specificato.
        ''' </summary>
        ''' <param name="idConvocazioneAppuntamento"></param>
        ''' <param name="idUtenteInvio"></param>
        ''' <param name="dataInvio"></param>
        ''' <param name="noteAvvisi"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateStoricoAppuntamenti_Invio(idConvocazioneAppuntamento As Long, idUtenteInvio As Long, dataInvio As DateTime, noteAvvisi As String) As Integer Implements IPrenotazioniProvider.UpdateStoricoAppuntamenti_DatiInvio

            Dim count As Integer = 0

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.CommandText = "update T_CNV_APPUNTAMENTI " +
                        " set CNA_DATA_INVIO = :CNA_DATA_INVIO, " +
                        " CNA_UTE_ID_INVIO = :CNA_UTE_ID_INVIO, " +
                        " CNA_NOTE_AVVISI = :CNA_NOTE_AVVISI " +
                        " where CNA_ID = :CNA_ID"

                    cmd.Parameters.AddWithValue("CNA_DATA_INVIO", dataInvio)
                    cmd.Parameters.AddWithValue("CNA_UTE_ID_INVIO", idUtenteInvio)
                    If String.IsNullOrWhiteSpace(noteAvvisi) Then
                        cmd.Parameters.AddWithValue("CNA_NOTE_AVVISI", DBNull.Value)
                    Else
                        cmd.Parameters.AddWithValue("CNA_NOTE_AVVISI", noteAvvisi)
                    End If
                    cmd.Parameters.AddWithValue("CNA_ID", idConvocazioneAppuntamento)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Restituisce un oggetto ConvocazioneAppuntamento con i dati dello storico più recente, in base a paziente e data di convocazione.
        ''' Se non trova nessuno storico per la convocazione specificata, restituisce Nothing.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataConvocazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetLastStoricoAppuntamenti(codicePaziente As Long, dataConvocazione As DateTime) As Entities.ConvocazioneAppuntamento Implements IPrenotazioniProvider.GetLastStoricoAppuntamenti

            Dim convocazioneAppuntamento As Entities.ConvocazioneAppuntamento = Nothing

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.CommandText =
                        " select CNA_ID, CNA_PAZ_CODICE, CNA_CNV_DATA, CNA_VACCINAZIONI, CNA_CNS_CODICE, CNA_AMB_CODICE, " +
                        " CNA_DATA_APPUNTAMENTO, CNA_DURATA_APPUNTAMENTO, CNA_ETA_POMERIGGIO, CNA_TIPO_APPUNTAMENTO, CNA_DATA_INVIO, " +
                        " CNA_UTE_ID_INVIO, CNA_NOTE, CNA_NOTE_AVVISI, CNA_NOTE_MODIFICA_APPUNTAMENTO, CNA_PAZ_CODICE_OLD, CNA_PRIMO_APPUNTAMENTO, CNA_CAMPAGNA, CNA_DATA_REGISTRAZIONE_APP, " +
                        " CNA_UTE_ID_REGISTRAZIONE_APP, CNA_DATA_ELIMINAZIONE, CNA_UTE_ID_ELIMINAZIONE, CNA_MEA_ID_MOTIVO_ELIMINAZIONE " +
                        " from T_CNV_APPUNTAMENTI where CNA_ID = ( " +
                        " select max(CNA_ID) from T_CNV_APPUNTAMENTI " +
                        " where CNA_PAZ_CODICE = :CNA_PAZ_CODICE " +
                        " and CNA_CNV_DATA = :CNA_CNV_DATA)"

                    cmd.Parameters.AddWithValue("CNA_PAZ_CODICE", codicePaziente)
                    cmd.Parameters.AddWithValue("CNA_CNV_DATA", dataConvocazione)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim cna_id As Integer = idr.GetOrdinal("CNA_ID")
                            Dim cna_paz_codice As Integer = idr.GetOrdinal("CNA_PAZ_CODICE")
                            Dim cna_cnv_data As Integer = idr.GetOrdinal("CNA_CNV_DATA")
                            Dim cna_vaccinazioni As Integer = idr.GetOrdinal("CNA_VACCINAZIONI")
                            Dim cna_cns_codice As Integer = idr.GetOrdinal("CNA_CNS_CODICE")
                            Dim cna_amb_codice As Integer = idr.GetOrdinal("CNA_AMB_CODICE")
                            Dim cna_data_appuntamento As Integer = idr.GetOrdinal("CNA_DATA_APPUNTAMENTO")
                            Dim cna_durata_appuntamento As Integer = idr.GetOrdinal("CNA_DURATA_APPUNTAMENTO")
                            Dim cna_eta_pomeriggio As Integer = idr.GetOrdinal("CNA_ETA_POMERIGGIO")
                            Dim cna_tipo_appuntamento As Integer = idr.GetOrdinal("CNA_TIPO_APPUNTAMENTO")
                            Dim cna_data_invio As Integer = idr.GetOrdinal("CNA_DATA_INVIO")
                            Dim cna_ute_id_invio As Integer = idr.GetOrdinal("CNA_UTE_ID_INVIO")
                            Dim cna_note As Integer = idr.GetOrdinal("CNA_NOTE")
                            Dim cna_note_avvisi As Integer = idr.GetOrdinal("CNA_NOTE_AVVISI")
                            Dim cna_note_modifica_appuntamento As Integer = idr.GetOrdinal("CNA_NOTE_MODIFICA_APPUNTAMENTO")
                            Dim cna_paz_codice_old As Integer = idr.GetOrdinal("CNA_PAZ_CODICE_OLD")
                            Dim cna_primo_appuntamento As Integer = idr.GetOrdinal("CNA_PRIMO_APPUNTAMENTO")
                            Dim cna_campagna As Integer = idr.GetOrdinal("CNA_CAMPAGNA")
                            Dim cna_data_registrazione_app As Integer = idr.GetOrdinal("CNA_DATA_REGISTRAZIONE_APP")
                            Dim cna_ute_id_registrazione_app As Integer = idr.GetOrdinal("CNA_UTE_ID_REGISTRAZIONE_APP")
                            Dim cna_data_eliminazione As Integer = idr.GetOrdinal("CNA_DATA_ELIMINAZIONE")
                            Dim cna_ute_id_eliminazione As Integer = idr.GetOrdinal("CNA_UTE_ID_ELIMINAZIONE")
                            Dim cna_mea_id_motivo_eliminazione As Integer = idr.GetOrdinal("CNA_MEA_ID_MOTIVO_ELIMINAZIONE")

                            If idr.Read() Then
                                convocazioneAppuntamento = New Entities.ConvocazioneAppuntamento()
                                convocazioneAppuntamento.Id = idr.GetInt64(cna_id)
                                convocazioneAppuntamento.CodicePaziente = idr.GetInt64(cna_paz_codice)
                                convocazioneAppuntamento.DataConvocazione = idr.GetDateTime(cna_cnv_data)
                                convocazioneAppuntamento.Vaccinazioni = idr.GetStringOrDefault(cna_vaccinazioni)
                                convocazioneAppuntamento.CodiceConsultorio = idr.GetStringOrDefault(cna_cns_codice)
                                convocazioneAppuntamento.CodiceAmbulatorio = idr.GetNullableInt32OrDefault(cna_amb_codice)
                                convocazioneAppuntamento.DataAppuntamento = idr.GetNullableDateTimeOrDefault(cna_data_appuntamento)
                                convocazioneAppuntamento.DurataAppuntamento = idr.GetNullableInt32OrDefault(cna_durata_appuntamento)
                                convocazioneAppuntamento.EtaPomeriggio = idr.GetStringOrDefault(cna_eta_pomeriggio)
                                convocazioneAppuntamento.TipoAppuntamento = idr.GetStringOrDefault(cna_tipo_appuntamento)
                                convocazioneAppuntamento.DataInvio = idr.GetNullableDateTimeOrDefault(cna_data_invio)
                                convocazioneAppuntamento.IdUtenteInvio = idr.GetNullableInt64OrDefault(cna_ute_id_invio)
                                convocazioneAppuntamento.Note = idr.GetStringOrDefault(cna_note)
                                convocazioneAppuntamento.NoteAvvisi = idr.GetStringOrDefault(cna_note_avvisi)
                                convocazioneAppuntamento.NoteModificaAppuntamento = idr.GetStringOrDefault(cna_note_modifica_appuntamento)
                                convocazioneAppuntamento.CodicePazienteOld = idr.GetNullableInt64OrDefault(cna_paz_codice_old)
                                convocazioneAppuntamento.DataPrimoAppuntamento = idr.GetNullableDateTimeOrDefault(cna_primo_appuntamento)
                                convocazioneAppuntamento.Campagna = idr.GetStringOrDefault(cna_campagna)
                                convocazioneAppuntamento.DataRegistrazioneAppuntamento = idr.GetNullableDateTimeOrDefault(cna_data_registrazione_app)
                                convocazioneAppuntamento.IdUtenteRegistrazioneAppuntamento = idr.GetNullableInt64OrDefault(cna_ute_id_registrazione_app)
                                convocazioneAppuntamento.DataEliminazioneAppuntamento = idr.GetNullableDateTimeOrDefault(cna_data_eliminazione)
                                convocazioneAppuntamento.IdUtenteEliminazioneAppuntamento = idr.GetNullableInt64OrDefault(cna_ute_id_eliminazione)
                                convocazioneAppuntamento.IdMotivoEliminazioneAppuntamento = idr.GetStringOrDefault(cna_mea_id_motivo_eliminazione)
                            End If

                        End If
                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return convocazioneAppuntamento

        End Function

        ''' <summary>
        ''' Restituisce la data di appuntamento per la convocazione specificata, se presente.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataConvocazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDataAppuntamento(codicePaziente As Long, dataConvocazione As DateTime) As DateTime? Implements IPrenotazioniProvider.GetDataAppuntamento

            Dim dataAppuntamento As DateTime? = Nothing

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.CommandText = "select CNV_DATA_APPUNTAMENTO from T_CNV_CONVOCAZIONI where CNV_PAZ_CODICE = :CNV_PAZ_CODICE and CNV_DATA = :CNV_DATA"
                    cmd.Parameters.AddWithValue("CNV_PAZ_CODICE", codicePaziente)
                    cmd.Parameters.AddWithValue("CNV_DATA", dataConvocazione)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        dataAppuntamento = Convert.ToDateTime(obj)
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return dataAppuntamento

        End Function

        ''' <summary>
        ''' Restituisce tutti i motivi di eliminazione, non obsoleti, selezionabili dagli utenti.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetMotiviEliminazioneAppuntamento() As List(Of KeyValuePair(Of String, String)) Implements IPrenotazioniProvider.GetMotiviEliminazioneAppuntamento

            Dim list As New List(Of KeyValuePair(Of String, String))()

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.CommandText = "select MEA_ID, MEA_DESCRIZIONE from T_ANA_MOTIVI_ELIMINAZIONE_APP where MEA_OBSOLETO = 'N' AND MEA_SELEZIONABILE = 'S'"

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim mea_id As Integer = idr.GetOrdinal("MEA_ID")
                            Dim mea_descrizione As Integer = idr.GetOrdinal("MEA_DESCRIZIONE")

                            While idr.Read()

                                list.Add(New KeyValuePair(Of String, String)(idr.GetString(mea_id), idr.GetString(mea_descrizione)))

                            End While

                        End If
                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list

        End Function

        ''' <summary>
        '''  Restituisce il totale di elementi presenti nello storico appuntamenti del paziente
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataConvocazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CountStoricoAppuntamenti(codicePaziente As Long, dataConvocazione As DateTime?) As Integer Implements IPrenotazioniProvider.CountStoricoAppuntamenti

            Dim count As Integer = 0

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim query As String = "select count(*) from T_CNV_APPUNTAMENTI where CNA_PAZ_CODICE = :cna_paz_codice {0}"

                    cmd.Parameters.AddWithValue("cna_paz_codice", codicePaziente)

                    Dim filtroConvocazione As String = String.Empty
                    If dataConvocazione.HasValue Then
                        filtroConvocazione = " and cna_cnv_data = :cna_cnv_data  "
                        cmd.Parameters.AddWithValue("cna_cnv_data", dataConvocazione.Value)
                    End If

                    cmd.CommandText = String.Format(query, filtroConvocazione)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        count = Convert.ToInt32(obj)
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Restituisce lo storico appuntamenti della convocazione specificata.
        ''' Se la data di convocazione è nulla, restituisce lo storico relativo a tutte le convocazioni del paziente.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataConvocazione"></param>
        ''' <param name="orderBy"></param>
        ''' <param name="pagingOptions"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetStoricoAppuntamenti(codicePaziente As Long, dataConvocazione As DateTime?, orderBy As String, pagingOptions As Onit.OnAssistnet.Data.PagingOptions) As List(Of StoricoAppuntamento) Implements IPrenotazioniProvider.GetStoricoAppuntamenti

            Dim list As New List(Of StoricoAppuntamento)()

            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim filtroConvocazione As String = String.Empty
                    If dataConvocazione.HasValue Then
                        filtroConvocazione = " and cna_cnv_data = :cna_cnv_data  "
                        cmd.Parameters.AddWithValue("cna_cnv_data", dataConvocazione.Value)
                    End If

                    Dim query As String = "select CNA_ID, CNA_PAZ_CODICE, CNA_CNV_DATA, CNA_VACCINAZIONI, CNA_CNS_CODICE, CNA_AMB_CODICE, AMB_DESCRIZIONE, " +
                        " CNA_DATA_APPUNTAMENTO, CNA_DURATA_APPUNTAMENTO, CNA_ETA_POMERIGGIO, CNA_TIPO_APPUNTAMENTO, CNA_NOTE_AVVISI, CNA_NOTE_MODIFICA_APPUNTAMENTO," +
                        " CNA_DATA_INVIO, CNA_UTE_ID_INVIO, t_ute_invio.ute_codice UTE_CODICE_INVIO, t_ute_invio.ute_descrizione UTE_DESCRIZIONE_INVIO, " +
                        " CNA_NOTE, CNA_PAZ_CODICE_OLD, CNA_PRIMO_APPUNTAMENTO, CNA_CAMPAGNA, " +
                        " CNA_DATA_REGISTRAZIONE_APP, CNA_UTE_ID_REGISTRAZIONE_APP, t_ute_reg.ute_codice UTE_CODICE_REG, t_ute_reg.ute_descrizione UTE_DESCRIZIONE_REG, " +
                        " CNA_DATA_ELIMINAZIONE, CNA_UTE_ID_ELIMINAZIONE, t_ute_del.ute_codice UTE_CODICE_DEL, t_ute_del.ute_descrizione UTE_DESCRIZIONE_DEL, " +
                        " CNA_MEA_ID_MOTIVO_ELIMINAZIONE, MEA_DESCRIZIONE " +
                        " from T_CNV_APPUNTAMENTI " +
                        " left join T_ANA_AMBULATORI on CNA_AMB_CODICE = AMB_CODICE " +
                        " left join T_ANA_UTENTI t_ute_invio on CNA_UTE_ID_INVIO = t_ute_invio.ute_id " +
                        " left join T_ANA_UTENTI t_ute_reg on CNA_UTE_ID_REGISTRAZIONE_APP = t_ute_reg.ute_id " +
                        " left join T_ANA_UTENTI t_ute_del on CNA_UTE_ID_ELIMINAZIONE = t_ute_del.ute_id " +
                        " left join T_ANA_MOTIVI_ELIMINAZIONE_APP on CNA_MEA_ID_MOTIVO_ELIMINAZIONE = mea_id " +
                        " where CNA_PAZ_CODICE = :cna_paz_codice {0}" +
                        " order by {1}"

                    cmd.CommandText = String.Format(query, filtroConvocazione, orderBy)

                    cmd.Parameters.AddWithValue("cna_paz_codice", codicePaziente)

                    If Not pagingOptions Is Nothing Then
                        cmd.AddPaginatedQuery(pagingOptions)
                    End If

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim cna_id As Integer = idr.GetOrdinal("CNA_ID")
                            Dim cna_paz_codice As Integer = idr.GetOrdinal("CNA_PAZ_CODICE")
                            Dim cna_cnv_data As Integer = idr.GetOrdinal("CNA_CNV_DATA")
                            Dim cna_vaccinazioni As Integer = idr.GetOrdinal("CNA_VACCINAZIONI")
                            Dim cna_cns_codice As Integer = idr.GetOrdinal("CNA_CNS_CODICE")
                            Dim cna_amb_codice As Integer = idr.GetOrdinal("CNA_AMB_CODICE")
                            Dim amb_descrizione As Integer = idr.GetOrdinal("AMB_DESCRIZIONE")
                            Dim cna_data_appuntamento As Integer = idr.GetOrdinal("CNA_DATA_APPUNTAMENTO")
                            Dim cna_durata_appuntamento As Integer = idr.GetOrdinal("CNA_DURATA_APPUNTAMENTO")
                            Dim cna_eta_pomeriggio As Integer = idr.GetOrdinal("CNA_ETA_POMERIGGIO")
                            Dim cna_tipo_appuntamento As Integer = idr.GetOrdinal("CNA_TIPO_APPUNTAMENTO")
                            Dim cna_data_invio As Integer = idr.GetOrdinal("CNA_DATA_INVIO")
                            Dim cna_ute_id_invio As Integer = idr.GetOrdinal("CNA_UTE_ID_INVIO")
                            Dim ute_codice_invio As Integer = idr.GetOrdinal("UTE_CODICE_INVIO")
                            Dim ute_descrizione_invio As Integer = idr.GetOrdinal("UTE_DESCRIZIONE_INVIO")
                            Dim cna_note As Integer = idr.GetOrdinal("CNA_NOTE")
                            Dim cna_note_avvisi As Integer = idr.GetOrdinal("CNA_NOTE_AVVISI")
                            Dim cna_note_modifica_appuntamento As Integer = idr.GetOrdinal("CNA_NOTE_MODIFICA_APPUNTAMENTO")
                            Dim cna_paz_codice_old As Integer = idr.GetOrdinal("CNA_PAZ_CODICE_OLD")
                            Dim cna_primo_appuntamento As Integer = idr.GetOrdinal("CNA_PRIMO_APPUNTAMENTO")
                            Dim cna_campagna As Integer = idr.GetOrdinal("CNA_CAMPAGNA")
                            Dim cna_data_registrazione_app As Integer = idr.GetOrdinal("CNA_DATA_REGISTRAZIONE_APP")
                            Dim cna_ute_id_registrazione_app As Integer = idr.GetOrdinal("CNA_UTE_ID_REGISTRAZIONE_APP")
                            Dim ute_codice_reg As Integer = idr.GetOrdinal("UTE_CODICE_REG")
                            Dim ute_descrizione_reg As Integer = idr.GetOrdinal("UTE_DESCRIZIONE_REG")
                            Dim cna_data_eliminazione As Integer = idr.GetOrdinal("CNA_DATA_ELIMINAZIONE")
                            Dim cna_ute_id_eliminazione As Integer = idr.GetOrdinal("CNA_UTE_ID_ELIMINAZIONE")
                            Dim ute_codice_del As Integer = idr.GetOrdinal("UTE_CODICE_DEL")
                            Dim ute_descrizione_del As Integer = idr.GetOrdinal("UTE_DESCRIZIONE_DEL")
                            Dim cna_mea_id_motivo_eliminazione As Integer = idr.GetOrdinal("CNA_MEA_ID_MOTIVO_ELIMINAZIONE")
                            Dim mea_descrizione As Integer = idr.GetOrdinal("MEA_DESCRIZIONE")

                            While idr.Read()

                                Dim storico As New Entities.StoricoAppuntamento()
                                storico.Id = idr.GetInt64(cna_id)
                                storico.CodicePaziente = idr.GetInt64(cna_paz_codice)
                                storico.DataConvocazione = idr.GetDateTime(cna_cnv_data)
                                storico.Vaccinazioni = idr.GetStringOrDefault(cna_vaccinazioni)
                                storico.CodiceConsultorio = idr.GetStringOrDefault(cna_cns_codice)
                                storico.CodiceAmbulatorio = idr.GetNullableInt32OrDefault(cna_amb_codice)
                                storico.DataAppuntamento = idr.GetNullableDateTimeOrDefault(cna_data_appuntamento)
                                storico.DurataAppuntamento = idr.GetNullableInt32OrDefault(cna_durata_appuntamento)
                                storico.EtaPomeriggio = idr.GetStringOrDefault(cna_eta_pomeriggio)
                                storico.TipoAppuntamento = idr.GetStringOrDefault(cna_tipo_appuntamento)
                                storico.DataInvio = idr.GetNullableDateTimeOrDefault(cna_data_invio)
                                storico.IdUtenteInvio = idr.GetNullableInt64OrDefault(cna_ute_id_invio)
                                storico.Note = idr.GetStringOrDefault(cna_note)
                                storico.NoteAvvisi = idr.GetStringOrDefault(cna_note_avvisi)
                                storico.NoteModificaAppuntamento = idr.GetStringOrDefault(cna_note_modifica_appuntamento)
                                storico.CodicePazienteOld = idr.GetNullableInt64OrDefault(cna_paz_codice_old)
                                storico.DataPrimoAppuntamento = idr.GetNullableDateTimeOrDefault(cna_primo_appuntamento)
                                storico.Campagna = idr.GetStringOrDefault(cna_campagna)
                                storico.DataRegistrazioneAppuntamento = idr.GetNullableDateTimeOrDefault(cna_data_registrazione_app)
                                storico.IdUtenteRegistrazioneAppuntamento = idr.GetNullableInt64OrDefault(cna_ute_id_registrazione_app)
                                storico.DataEliminazioneAppuntamento = idr.GetNullableDateTimeOrDefault(cna_data_eliminazione)
                                storico.IdUtenteEliminazioneAppuntamento = idr.GetNullableInt64OrDefault(cna_ute_id_eliminazione)
                                storico.IdMotivoEliminazioneAppuntamento = idr.GetStringOrDefault(cna_mea_id_motivo_eliminazione)
                                storico.DescrizioneAmbulatorio = idr.GetStringOrDefault(amb_descrizione)
                                storico.CodiceUtenteInvio = idr.GetStringOrDefault(ute_codice_invio)
                                storico.DescrizioneUtenteInvio = idr.GetStringOrDefault(ute_descrizione_invio)
                                storico.CodiceUtenteRegistrazioneAppuntamento = idr.GetStringOrDefault(ute_codice_reg)
                                storico.DescrizioneUtenteRegistrazioneAppuntamento = idr.GetStringOrDefault(ute_descrizione_reg)
                                storico.CodiceUtenteEliminazioneAppuntamento = idr.GetStringOrDefault(ute_codice_del)
                                storico.DescrizioneUtenteEliminazioneAppuntamento = idr.GetStringOrDefault(ute_descrizione_del)
                                storico.DescrizioneMotivoEliminazioneAppuntamento = idr.GetStringOrDefault(mea_descrizione)

                                list.Add(storico)

                            End While

                        End If
                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list

        End Function

#End Region

    End Class

End Namespace
