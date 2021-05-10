Imports System.Collections.Generic
Imports System.Data.OracleClient
Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.Data.OracleClient
Imports Onit.OnAssistnet.OnVac.DAL

Public Class DbCovid19Tamponi
    Inherits DbProvider
    Implements ICovid19Tamponi

#Region " Constructors "
    Public Sub New(ByRef DAM As IDAM)
        MyBase.New(DAM)
    End Sub
#End Region


    Public Function GetTamponiOrfani(ricerca As RicercaTamponiOrfani) As List(Of TamponeOrfano) Implements ICovid19Tamponi.GetTamponiOrfani
        Return DoCommand(Function(cmd)
                             Dim result As New List(Of TamponeOrfano)
                             Dim query As New Text.StringBuilder("SELECT ATC_ID, 
                                                                    ATC_PAZ_CODICE, 
                                                                    ATC_CF, ATC_DATEOFBIRTH, 
                                                                    ATC_DOMICILE_ADDR, 
                                                                    ATC_DOMICILE_CIV,
                                                                    ATC_DOMICILE_CAP, 
                                                                    ATC_DOMICILE_COM,
                                                                    ATC_DOMICILE_PROV, 
                                                                    ATC_GENDER, 
                                                                    ATC_MICROBIOLOGY_CODE, 
                                                                    ATC_MICROBIOLOGY_NAME,
                                                                    ATC_MPI, 
                                                                    ATC_NAME, ATC_SURNAME,
                                                                    ATC_NOTE, ATC_NUMREQ,
                                                                    ATC_PLACEOFBIRTH, 
                                                                    ATC_RESIDENCE_ADDR,
                                                                    ATC_RESIDENCE_CIV,
                                                                    ATC_RESIDENCE_CAP,
                                                                    ATC_RESIDENCE_COM, 
                                                                    ATC_RESIDENCE_PROV, 
                                                                    ATC_REQUESTOR, 
                                                                    ATC_RESULT, 
                                                                    ATC_SAMPLING_DATE, 
                                                                    ATC_SAMPLING_DATE_RESPONSE,
                                                                    ATC_ULSS,
                                                                    usl_descrizione,
                                                                    ATC_DTDECESSO,
                                                                    ATC_DESCCITTADINANZA,
                                                                    ATC_COMRES, ATC_COMDOM,
                                                                    ATC_DOCTOR_MPI, 
                                                                    ATC_DOCTOR_NAME, 
                                                                    ATC_DOCTOR_SURNAME,
                                                                    ATC_CFMEDICO, 
                                                                    ATC_GUARITO, 
                                                                    ATC_DTGUARIGIONE, 
                                                                    ATC_ST_ID, 
                                                                    ATC_ACQUISITO,
                                                                    ATC_LOG_ERRORE 
                                                                                        FROM T_ANA_TAMPONI_COVID 
                                                                                        left join t_ana_usl on ATC_ULSS = USL_CODICE
                                                                                        LEFT JOIN T_PAZ_PAZIENTI ON atc_paz_codice = PAZ_CODICE 
                                                                                        WHERE ATC_ST_ID = 12 ")


                             If Not String.IsNullOrWhiteSpace(ricerca.Esito) Then
                                 query.Append(" AND UPPER(ATC_RESULT) = :ATC_RESULT ")
                                 cmd.AddParameter("ATC_RESULT", ricerca.Esito.ToUpper())
                             End If
                             If ricerca.DataPrelievo.HasValue() Then
                                 Dim inizio As Date = ricerca.DataPrelievo.Value.Date
                                 Dim fine As Date = ricerca.DataPrelievo.Value.Date.AddDays(1)

                                 query.Append(" AND ATC_SAMPLING_DATE >= :ATC_SAMPLING_DATE_A AND ATC_SAMPLING_DATE < :ATC_SAMPLING_DATE_B ")
                                 cmd.AddParameter("ATC_SAMPLING_DATE_A", inizio)
                                 cmd.AddParameter("ATC_SAMPLING_DATE_B", fine)
                             End If
                             If ricerca.DataReferto.HasValue() Then
                                 Dim a As Date = ricerca.DataReferto.Value.Date
                                 Dim b As Date = ricerca.DataReferto.Value.Date.AddDays(1)

                                 query.Append(" AND ATC_SAMPLING_DATE_RESPONSE > :ATC_SAMPLING_DATE_RESPONSE_A AND ATC_SAMPLING_DATE_RESPONSE < :ATC_SAMPLING_DATE_RESPONSE_B")
                                 cmd.AddParameter("ATC_SAMPLING_DATE_RESPONSE_A", a)
                                 cmd.AddParameter("ATC_SAMPLING_DATE_RESPONSE_B", b)
                             End If

                             ''AND 
                             ''(ATC_ULSS IS NULL OR (ATC_ULSS = :ATC_ULSS AND ATC_PAZ_CODICE IS NULL))
                             If ricerca.PazienteMancante AndAlso ricerca.UlssMancante Then
                                 query.Append(" AND (ATC_ULSS IS NULL OR (ATC_ULSS = :ATC_ULSS AND ATC_PAZ_CODICE IS NULL))")
                                 cmd.AddParameter("ATC_ULSS", ricerca.CodiceUlss)
                             ElseIf ricerca.PazienteMancante Then
                                 query.Append(" AND (ATC_ULSS = :ATC_ULSS OR ATC_ULSS is null) AND ATC_PAZ_CODICE IS NULL")
                                 cmd.AddParameter("ATC_ULSS", ricerca.CodiceUlss)
                             ElseIf ricerca.UlssMancante Then
                                 query.Append(" AND ATC_ULSS IS NULL")
                             End If

                             If Not String.IsNullOrWhiteSpace(ricerca.Nome) Then
                                 query.Append(" AND ((ATC_PAZ_CODICE is null and LOWER(ATC_NAME) like ?nome) or (ATC_PAZ_CODICE is not null and LOWER(PAZ_NOME) like ?nome))")
                                 cmd.AddParameter("nome", String.Format("{0}%", ricerca.Nome.ToLower()))
                             End If
                             If Not String.IsNullOrWhiteSpace(ricerca.Cognome) Then
                                 query.Append(" AND ((ATC_PAZ_CODICE is null and LOWER(ATC_SURNAME) like ?cognome) or (ATC_PAZ_CODICE is not null and LOWER(PAZ_COGNOME) like ?cognome))")
                                 cmd.AddParameter("cognome", String.Format("{0}%", ricerca.Cognome.ToLower()))
                             End If
                             If ricerca.DataNascita.HasValue Then
                                 query.Append(" AND ((ATC_PAZ_CODICE is null and ATC_DATEOFBIRTH = ?dataNascita) or (ATC_PAZ_CODICE is not null and paz_data_nascita = ?dataNascita))")
                                 cmd.AddParameter("dataNascita", ricerca.DataNascita.Value)
                             End If

                             cmd.SkipTakeQuery(String.Format("{0} ORDER BY atc_sampling_date", query.ToString()), ricerca.Skip, ricerca.Take)
                             Using _context As IDataReader = cmd.ExecuteReader()

                                 Dim ATC_ID As Integer = _context.GetOrdinal("ATC_ID")
                                 Dim ATC_PAZ_CODICE As Integer = _context.GetOrdinal("ATC_PAZ_CODICE")
                                 Dim ATC_CF As Integer = _context.GetOrdinal("ATC_CF")
                                 Dim ATC_DATEOFBIRTH As Integer = _context.GetOrdinal("ATC_DATEOFBIRTH")
                                 Dim ATC_DOMICILE_ADDR As Integer = _context.GetOrdinal("ATC_DOMICILE_ADDR")
                                 Dim ATC_DOMICILE_CIV As Integer = _context.GetOrdinal("ATC_DOMICILE_CIV")
                                 Dim ATC_DOMICILE_CAP As Integer = _context.GetOrdinal("ATC_DOMICILE_CAP")
                                 Dim ATC_DOMICILE_COM As Integer = _context.GetOrdinal("ATC_DOMICILE_COM")
                                 Dim ATC_DOMICILE_PROV As Integer = _context.GetOrdinal("ATC_DOMICILE_PROV")
                                 Dim ATC_GENDER As Integer = _context.GetOrdinal("ATC_GENDER")
                                 Dim ATC_MICROBIOLOGY_CODE As Integer = _context.GetOrdinal("ATC_MICROBIOLOGY_CODE")
                                 Dim ATC_MICROBIOLOGY_NAME As Integer = _context.GetOrdinal("ATC_MICROBIOLOGY_NAME")
                                 Dim ATC_MPI As Integer = _context.GetOrdinal("ATC_MPI")
                                 Dim ATC_NAME As Integer = _context.GetOrdinal("ATC_NAME")
                                 Dim ATC_SURNAME As Integer = _context.GetOrdinal("ATC_SURNAME")
                                 Dim ATC_NOTE As Integer = _context.GetOrdinal("ATC_NOTE")
                                 Dim ATC_NUMREQ As Integer = _context.GetOrdinal("ATC_NUMREQ")
                                 Dim ATC_PLACEOFBIRTH As Integer = _context.GetOrdinal("ATC_PLACEOFBIRTH")
                                 Dim ATC_RESIDENCE_ADDR As Integer = _context.GetOrdinal("ATC_RESIDENCE_ADDR")
                                 Dim ATC_RESIDENCE_CIV As Integer = _context.GetOrdinal("ATC_RESIDENCE_CIV")
                                 Dim ATC_RESIDENCE_CAP As Integer = _context.GetOrdinal("ATC_RESIDENCE_CAP")
                                 Dim ATC_RESIDENCE_COM As Integer = _context.GetOrdinal("ATC_RESIDENCE_COM")
                                 Dim ATC_RESIDENCE_PROV As Integer = _context.GetOrdinal("ATC_RESIDENCE_PROV")
                                 Dim ATC_REQUESTOR As Integer = _context.GetOrdinal("ATC_REQUESTOR")
                                 Dim ATC_RESULT As Integer = _context.GetOrdinal("ATC_RESULT")
                                 Dim ATC_SAMPLING_DATE As Integer = _context.GetOrdinal("ATC_SAMPLING_DATE")
                                 Dim ATC_SAMPLING_DATE_RESPONSE As Integer = _context.GetOrdinal("ATC_SAMPLING_DATE_RESPONSE")
                                 Dim ATC_ULSS As Integer = _context.GetOrdinal("ATC_ULSS")
                                 Dim ULS_DESCRIZIONE As Integer = _context.GetOrdinal("usl_descrizione")
                                 Dim ATC_DTDECESSO As Integer = _context.GetOrdinal("ATC_DTDECESSO")
                                 Dim ATC_DESCCITTADINANZA As Integer = _context.GetOrdinal("ATC_DESCCITTADINANZA")
                                 Dim ATC_COMRES As Integer = _context.GetOrdinal("ATC_COMRES")
                                 Dim ATC_COMDOM As Integer = _context.GetOrdinal("ATC_COMDOM")
                                 Dim ATC_DOCTOR_MPI As Integer = _context.GetOrdinal("ATC_DOCTOR_MPI")
                                 Dim ATC_DOCTOR_NAME As Integer = _context.GetOrdinal("ATC_DOCTOR_NAME")
                                 Dim ATC_DOCTOR_SURNAME As Integer = _context.GetOrdinal("ATC_DOCTOR_SURNAME")
                                 Dim ATC_CFMEDICO As Integer = _context.GetOrdinal("ATC_CFMEDICO")
                                 Dim ATC_GUARITO As Integer = _context.GetOrdinal("ATC_GUARITO")
                                 Dim ATC_DTGUARIGIONE As Integer = _context.GetOrdinal("ATC_DTGUARIGIONE")
                                 Dim ATC_ST_ID As Integer = _context.GetOrdinal("ATC_ST_ID")
                                 Dim ATC_ACQUISITO As Integer = _context.GetOrdinal("ATC_ACQUISITO")
                                 Dim ATC_LOG_ERRORE As Integer = _context.GetOrdinal("ATC_LOG_ERRORE")

                                 While _context.Read()
                                     Dim Tampone As New TamponeOrfano()

                                     Tampone.IdPaziente = _context.GetNullableInt64OrDefault(ATC_PAZ_CODICE)
                                     Tampone.Id = _context.GetInt32OrDefault(ATC_ID)
                                     Tampone.CodiceFiscale = _context.GetStringOrDefault(ATC_CF)
                                     Tampone.DataDiNascita = _context.GetDateTimeOrDefault(ATC_DATEOFBIRTH)
                                     Tampone.Domicilio = _context.GetStringOrDefault(ATC_DOMICILE_ADDR)
                                     Tampone.DomicilioCivico = _context.GetStringOrDefault(ATC_DOMICILE_CIV)
                                     Tampone.DomicilioCap = _context.GetStringOrDefault(ATC_DOMICILE_CAP)
                                     Tampone.DomicilioComune = _context.GetStringOrDefault(ATC_DOMICILE_COM)
                                     Tampone.DomicilioProvincia = _context.GetStringOrDefault(ATC_DOMICILE_PROV)
                                     Tampone.Sesso = _context.GetStringOrDefault(ATC_GENDER)
                                     Tampone.CodiceMicrobiologico = _context.GetStringOrDefault(ATC_MICROBIOLOGY_CODE)
                                     Tampone.NomeMicrobiologico = _context.GetStringOrDefault(ATC_MICROBIOLOGY_NAME)
                                     Tampone.MPI = _context.GetDoubleOrDefault(ATC_MPI)
                                     Tampone.Nome = _context.GetStringOrDefault(ATC_NAME)
                                     Tampone.Cognome = _context.GetStringOrDefault(ATC_SURNAME)
                                     Tampone.Note = _context.GetStringOrDefault(ATC_NOTE)
                                     Tampone.NumreQ = _context.GetInt64(ATC_NUMREQ).ToString()
                                     Tampone.LuogoDiNascita = _context.GetStringOrDefault(ATC_PLACEOFBIRTH)
                                     Tampone.IndirizzoResidenza = _context.GetStringOrDefault(ATC_RESIDENCE_ADDR)
                                     Tampone.ResidenzaCivico = _context.GetStringOrDefault(ATC_RESIDENCE_CIV)
                                     Tampone.ResidenzaCap = _context.GetStringOrDefault(ATC_RESIDENCE_CAP)
                                     Tampone.ResidenzaCom = _context.GetStringOrDefault(ATC_RESIDENCE_COM)
                                     Tampone.ResidenzaProv = _context.GetStringOrDefault(ATC_RESIDENCE_PROV)
                                     Tampone.Richiedente = _context.GetStringOrDefault(ATC_REQUESTOR)
                                     Tampone.Risultato = _context.GetStringOrDefault(ATC_RESULT)
                                     Tampone.SamplingDate = _context.GetDateTimeOrDefault(ATC_SAMPLING_DATE)
                                     Tampone.SamplingDateResponse = _context.GetNullableDateTimeOrDefault(ATC_SAMPLING_DATE_RESPONSE)
                                     Tampone.Ulss = _context.GetStringOrDefault(ATC_ULSS)
                                     Tampone.DescrizioneUlss = _context.GetStringOrDefault(ULS_DESCRIZIONE)
                                     Tampone.DTDDecesso = _context.GetDateTimeOrDefault(ATC_DTDECESSO)
                                     Tampone.DescCittadinanza = _context.GetStringOrDefault(ATC_DESCCITTADINANZA)
                                     Tampone.Comres = _context.GetStringOrDefault(ATC_COMRES)
                                     Tampone.ComDom = _context.GetStringOrDefault(ATC_COMDOM)
                                     Tampone.DoctorMpi = _context.GetStringOrDefault(ATC_DOCTOR_MPI)
                                     Tampone.DottoreNome = _context.GetStringOrDefault(ATC_DOCTOR_NAME)
                                     Tampone.DottoreCognome = _context.GetStringOrDefault(ATC_DOCTOR_SURNAME)
                                     Tampone.CfMedico = _context.GetStringOrDefault(ATC_CFMEDICO)
                                     Tampone.Guarito = _context.GetStringOrDefault(ATC_GUARITO)
                                     Tampone.DataGuarigione = _context.GetDateTimeOrDefault(ATC_DTGUARIGIONE)
                                     Tampone.Stato_ID = _context.GetInt32OrDefault(ATC_ST_ID)
                                     Tampone.Acquisito = _context.GetInt64OrDefault(ATC_ACQUISITO)
                                     Tampone.LogErrore = _context.GetStringOrDefault(ATC_LOG_ERRORE)
                                     result.Add(Tampone)
                                 End While
                             End Using
                             Return result
                         End Function)
    End Function
    Public Function GetTamponiById(Id As Long) As List(Of TamponeDiFrontiera) Implements ICovid19Tamponi.GetTamponiById
        Dim result As New List(Of TamponeDiFrontiera)
        Dim ownConnection As Boolean = False
        Dim query As String = "SELECT ATC_ID, ATC_CF, ATC_DATEOFBIRTH, ATC_DOMICILE_ADDR, ATC_DOMICILE_CIV, ATC_DOMICILE_CAP, ATC_DOMICILE_COM, ATC_DOMICILE_PROV, ATC_GENDER, ATC_MICROBIOLOGY_CODE, ATC_MICROBIOLOGY_NAME, 
            ATC_MPI, ATC_NAME, ATC_SURNAME, ATC_NOTE, ATC_NUMREQ, ATC_PLACEOFBIRTH, ATC_RESIDENCE_ADDR, ATC_RESIDENCE_CIV, ATC_RESIDENCE_CAP, ATC_RESIDENCE_COM, ATC_RESIDENCE_PROV, ATC_REQUESTOR, 
            ATC_RESULT, ATC_SAMPLING_DATE, ATC_SAMPLING_DATE_RESPONSE, ATC_ULSS, ATC_DTDECESSO, ATC_DESCCITTADINANZA, ATC_COMRES, ATC_COMDOM, ATC_DOCTOR_MPI, ATC_DOCTOR_NAME, ATC_DOCTOR_SURNAME, ATC_CFMEDICO, 
            ATC_GUARITO, ATC_DTGUARIGIONE, ATC_ST_ID, ATC_ACQUISITO, ATC_LOG_ERRORE 
            FROM T_ANA_TAMPONI_COVID 
            WHERE ATC_PAZ_CODICE = :ATC_PAZ_CODICE 
            ORDER BY ATC_SAMPLING_DATE_RESPONSE ASC"
        Try
            Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                cmd.Parameters.AddWithValue("ATC_PAZ_CODICE", Id)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using _context As IDataReader = cmd.ExecuteReader()

                    Dim ATC_ID As Integer = _context.GetOrdinal("ATC_ID")
                    Dim ATC_CF As Integer = _context.GetOrdinal("ATC_CF")
                    Dim ATC_DATEOFBIRTH As Integer = _context.GetOrdinal("ATC_DATEOFBIRTH")
                    Dim ATC_DOMICILE_ADDR As Integer = _context.GetOrdinal("ATC_DOMICILE_ADDR")
                    Dim ATC_DOMICILE_CIV As Integer = _context.GetOrdinal("ATC_DOMICILE_CIV")
                    Dim ATC_DOMICILE_CAP As Integer = _context.GetOrdinal("ATC_DOMICILE_CAP")
                    Dim ATC_DOMICILE_COM As Integer = _context.GetOrdinal("ATC_DOMICILE_COM")
                    Dim ATC_DOMICILE_PROV As Integer = _context.GetOrdinal("ATC_DOMICILE_PROV")
                    Dim ATC_GENDER As Integer = _context.GetOrdinal("ATC_GENDER")
                    Dim ATC_MICROBIOLOGY_CODE As Integer = _context.GetOrdinal("ATC_MICROBIOLOGY_CODE")
                    Dim ATC_MICROBIOLOGY_NAME As Integer = _context.GetOrdinal("ATC_MICROBIOLOGY_NAME")
                    Dim ATC_MPI As Integer = _context.GetOrdinal("ATC_MPI")
                    Dim ATC_NAME As Integer = _context.GetOrdinal("ATC_NAME")
                    Dim ATC_SURNAME As Integer = _context.GetOrdinal("ATC_SURNAME")
                    Dim ATC_NOTE As Integer = _context.GetOrdinal("ATC_NOTE")
                    Dim ATC_NUMREQ As Integer = _context.GetOrdinal("ATC_NUMREQ")
                    Dim ATC_PLACEOFBIRTH As Integer = _context.GetOrdinal("ATC_PLACEOFBIRTH")
                    Dim ATC_RESIDENCE_ADDR As Integer = _context.GetOrdinal("ATC_RESIDENCE_ADDR")
                    Dim ATC_RESIDENCE_CIV As Integer = _context.GetOrdinal("ATC_RESIDENCE_CIV")
                    Dim ATC_RESIDENCE_CAP As Integer = _context.GetOrdinal("ATC_RESIDENCE_CAP")
                    Dim ATC_RESIDENCE_COM As Integer = _context.GetOrdinal("ATC_RESIDENCE_COM")
                    Dim ATC_RESIDENCE_PROV As Integer = _context.GetOrdinal("ATC_RESIDENCE_PROV")
                    Dim ATC_REQUESTOR As Integer = _context.GetOrdinal("ATC_REQUESTOR")
                    Dim ATC_RESULT As Integer = _context.GetOrdinal("ATC_RESULT")
                    Dim ATC_SAMPLING_DATE As Integer = _context.GetOrdinal("ATC_SAMPLING_DATE")
                    Dim ATC_SAMPLING_DATE_RESPONSE As Integer = _context.GetOrdinal("ATC_SAMPLING_DATE_RESPONSE")
                    Dim ATC_ULSS As Integer = _context.GetOrdinal("ATC_ULSS")
                    Dim ATC_DTDECESSO As Integer = _context.GetOrdinal("ATC_DTDECESSO")
                    Dim ATC_DESCCITTADINANZA As Integer = _context.GetOrdinal("ATC_DESCCITTADINANZA")
                    Dim ATC_COMRES As Integer = _context.GetOrdinal("ATC_COMRES")
                    Dim ATC_COMDOM As Integer = _context.GetOrdinal("ATC_COMDOM")
                    Dim ATC_DOCTOR_MPI As Integer = _context.GetOrdinal("ATC_DOCTOR_MPI")
                    Dim ATC_DOCTOR_NAME As Integer = _context.GetOrdinal("ATC_DOCTOR_NAME")
                    Dim ATC_DOCTOR_SURNAME As Integer = _context.GetOrdinal("ATC_DOCTOR_SURNAME")
                    Dim ATC_CFMEDICO As Integer = _context.GetOrdinal("ATC_CFMEDICO")
                    Dim ATC_GUARITO As Integer = _context.GetOrdinal("ATC_GUARITO")
                    Dim ATC_DTGUARIGIONE As Integer = _context.GetOrdinal("ATC_DTGUARIGIONE")
                    Dim ATC_ST_ID As Integer = _context.GetOrdinal("ATC_ST_ID")
                    Dim ATC_ACQUISITO As Integer = _context.GetOrdinal("ATC_ACQUISITO")
                    Dim ATC_LOG_ERRORE As Integer = _context.GetOrdinal("ATC_LOG_ERRORE")

                    While _context.Read()
                        Dim Tampone As New TamponeDiFrontiera()
                        Tampone.Id = _context.GetInt64OrDefault(ATC_ID)
                        Tampone.CodiceFiscale = _context.GetStringOrDefault(ATC_CF)
                        Tampone.DataDiNascita = _context.GetDateTimeOrDefault(ATC_DATEOFBIRTH)
                        Tampone.Domicilio = _context.GetStringOrDefault(ATC_DOMICILE_ADDR)
                        Tampone.DomicilioCivico = _context.GetStringOrDefault(ATC_DOMICILE_CIV)
                        Tampone.DomicilioCap = _context.GetStringOrDefault(ATC_DOMICILE_CAP)
                        Tampone.DomicilioComune = _context.GetStringOrDefault(ATC_DOMICILE_COM)
                        Tampone.DomicilioProvincia = _context.GetStringOrDefault(ATC_DOMICILE_PROV)
                        Tampone.Sesso = _context.GetStringOrDefault(ATC_GENDER)
                        Tampone.CodiceMicrobiologico = _context.GetStringOrDefault(ATC_MICROBIOLOGY_CODE)
                        Tampone.NomeMicrobiologico = _context.GetStringOrDefault(ATC_MICROBIOLOGY_NAME)
                        Tampone.MPI = _context.GetDoubleOrDefault(ATC_MPI)
                        Tampone.Nome = _context.GetStringOrDefault(ATC_NAME)
                        Tampone.Cognome = _context.GetStringOrDefault(ATC_SURNAME)
                        Tampone.Note = _context.GetStringOrDefault(ATC_NOTE)
                        Tampone.NumreQ = _context.GetInt64(ATC_NUMREQ).ToString()
                        Tampone.LuogoDiNascita = _context.GetStringOrDefault(ATC_PLACEOFBIRTH)
                        Tampone.IndirizzoResidenza = _context.GetStringOrDefault(ATC_RESIDENCE_ADDR)
                        Tampone.ResidenzaCivico = _context.GetStringOrDefault(ATC_RESIDENCE_CIV)
                        Tampone.ResidenzaCap = _context.GetStringOrDefault(ATC_RESIDENCE_CAP)
                        Tampone.ResidenzaCom = _context.GetStringOrDefault(ATC_RESIDENCE_COM)
                        Tampone.ResidenzaProv = _context.GetStringOrDefault(ATC_RESIDENCE_PROV)
                        Tampone.Richiedente = _context.GetStringOrDefault(ATC_REQUESTOR)
                        Tampone.Risultato = _context.GetStringOrDefault(ATC_RESULT)
                        Tampone.SamplingDate = _context.GetDateTimeOrDefault(ATC_SAMPLING_DATE)
                        Tampone.SamplingDateResponse = _context.GetDateTimeOrDefault(ATC_SAMPLING_DATE_RESPONSE)
                        Tampone.Ulss = _context.GetStringOrDefault(ATC_ULSS)
                        Tampone.DTDDecesso = _context.GetDateTimeOrDefault(ATC_DTDECESSO)
                        Tampone.DescCittadinanza = _context.GetStringOrDefault(ATC_DESCCITTADINANZA)
                        Tampone.Comres = _context.GetStringOrDefault(ATC_COMRES)
                        Tampone.ComDom = _context.GetStringOrDefault(ATC_COMDOM)
                        Tampone.DoctorMpi = _context.GetStringOrDefault(ATC_DOCTOR_MPI)
                        Tampone.DottoreNome = _context.GetStringOrDefault(ATC_DOCTOR_NAME)
                        Tampone.DottoreCognome = _context.GetStringOrDefault(ATC_DOCTOR_SURNAME)
                        Tampone.CfMedico = _context.GetStringOrDefault(ATC_CFMEDICO)
                        Tampone.Guarito = _context.GetStringOrDefault(ATC_GUARITO)
                        Tampone.DataGuarigione = _context.GetDateTimeOrDefault(ATC_DTGUARIGIONE)
                        Tampone.Stato_ID = _context.GetInt32OrDefault(ATC_ST_ID)
                        Tampone.Acquisito = _context.GetInt64OrDefault(ATC_ACQUISITO)
                        Tampone.LogErrore = _context.GetStringOrDefault(ATC_LOG_ERRORE)
                        result.Add(Tampone)
                    End While
                End Using
            End Using
        Finally
            ConditionalCloseConnection(ownConnection)
        End Try
        Return result
    End Function
    Public Function GetTamponiFrontieraPerPazientiNonIdentificati() As List(Of TamponeDiFrontiera) Implements ICovid19Tamponi.GetTamponiFrontieraPerPazientiNonIdentificati
        Dim result As New List(Of TamponeDiFrontiera)
        Dim ownConnection As Boolean = False
        Dim query As String = "SELECT ATC_ID, ATC_CF, ATC_DATEOFBIRTH, ATC_DOMICILE_ADDR, ATC_DOMICILE_CIV, ATC_DOMICILE_CAP, ATC_DOMICILE_COM, ATC_DOMICILE_PROV, ATC_GENDER, ATC_MICROBIOLOGY_CODE, ATC_MICROBIOLOGY_NAME, " +
            "ATC_MPI, ATC_NAME, ATC_SURNAME, ATC_NOTE, ATC_NUMREQ, ATC_PLACEOFBIRTH, ATC_RESIDENCE_ADDR, ATC_RESIDENCE_CIV, ATC_RESIDENCE_CAP, ATC_RESIDENCE_COM, ATC_RESIDENCE_PROV, ATC_REQUESTOR, " +
            "ATC_RESULT, ATC_SAMPLING_DATE, ATC_SAMPLING_DATE_RESPONSE, ATC_ULSS, ATC_DTDECESSO, ATC_DESCCITTADINANZA, ATC_COMRES, ATC_COMDOM, ATC_DOCTOR_MPI, ATC_DOCTOR_NAME, ATC_DOCTOR_SURNAME, ATC_CFMEDICO, " +
            "ATC_GUARITO, ATC_DTGUARIGIONE, ATC_ST_ID, ATC_ACQUISITO, ATC_LOG_ERRORE " +
            "FROM T_ANA_TAMPONI_COVID " +
            "WHERE ATC_ST_ID = 12 AND (UPPER(ATC_LOG_ERRORE) Like'PAZIENTE NON REGISTRATO%' OR UPPER(ATC_LOG_ERRORE) = 'SOGGETTO NON IDENTIFICATO' OR UPPER(ATC_LOG_ERRORE) = 'ULSS NON VALORIZZATA') AND trunc(ATC_DATA_ELABORAZIONE) >trunc(SYSDATE)-1 AND (ATC_PAZ_CODICE <= 0 OR ATC_PAZ_CODICE IS NULL)" +
            "ORDER BY ATC_SAMPLING_DATE_RESPONSE ASC"
        Try
            Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using _context As IDataReader = cmd.ExecuteReader()

                    Dim ATC_ID As Integer = _context.GetOrdinal("ATC_ID")
                    Dim ATC_CF As Integer = _context.GetOrdinal("ATC_CF")
                    Dim ATC_DATEOFBIRTH As Integer = _context.GetOrdinal("ATC_DATEOFBIRTH")
                    Dim ATC_DOMICILE_ADDR As Integer = _context.GetOrdinal("ATC_DOMICILE_ADDR")
                    Dim ATC_DOMICILE_CIV As Integer = _context.GetOrdinal("ATC_DOMICILE_CIV")
                    Dim ATC_DOMICILE_CAP As Integer = _context.GetOrdinal("ATC_DOMICILE_CAP")
                    Dim ATC_DOMICILE_COM As Integer = _context.GetOrdinal("ATC_DOMICILE_COM")
                    Dim ATC_DOMICILE_PROV As Integer = _context.GetOrdinal("ATC_DOMICILE_PROV")
                    Dim ATC_GENDER As Integer = _context.GetOrdinal("ATC_GENDER")
                    Dim ATC_MICROBIOLOGY_CODE As Integer = _context.GetOrdinal("ATC_MICROBIOLOGY_CODE")
                    Dim ATC_MICROBIOLOGY_NAME As Integer = _context.GetOrdinal("ATC_MICROBIOLOGY_NAME")
                    Dim ATC_MPI As Integer = _context.GetOrdinal("ATC_MPI")
                    Dim ATC_NAME As Integer = _context.GetOrdinal("ATC_NAME")
                    Dim ATC_SURNAME As Integer = _context.GetOrdinal("ATC_SURNAME")
                    Dim ATC_NOTE As Integer = _context.GetOrdinal("ATC_NOTE")
                    Dim ATC_NUMREQ As Integer = _context.GetOrdinal("ATC_NUMREQ")
                    Dim ATC_PLACEOFBIRTH As Integer = _context.GetOrdinal("ATC_PLACEOFBIRTH")
                    Dim ATC_RESIDENCE_ADDR As Integer = _context.GetOrdinal("ATC_RESIDENCE_ADDR")
                    Dim ATC_RESIDENCE_CIV As Integer = _context.GetOrdinal("ATC_RESIDENCE_CIV")
                    Dim ATC_RESIDENCE_CAP As Integer = _context.GetOrdinal("ATC_RESIDENCE_CAP")
                    Dim ATC_RESIDENCE_COM As Integer = _context.GetOrdinal("ATC_RESIDENCE_COM")
                    Dim ATC_RESIDENCE_PROV As Integer = _context.GetOrdinal("ATC_RESIDENCE_PROV")
                    Dim ATC_REQUESTOR As Integer = _context.GetOrdinal("ATC_REQUESTOR")
                    Dim ATC_RESULT As Integer = _context.GetOrdinal("ATC_RESULT")
                    Dim ATC_SAMPLING_DATE As Integer = _context.GetOrdinal("ATC_SAMPLING_DATE")
                    Dim ATC_SAMPLING_DATE_RESPONSE As Integer = _context.GetOrdinal("ATC_SAMPLING_DATE_RESPONSE")
                    Dim ATC_ULSS As Integer = _context.GetOrdinal("ATC_ULSS")
                    Dim ATC_DTDECESSO As Integer = _context.GetOrdinal("ATC_DTDECESSO")
                    Dim ATC_DESCCITTADINANZA As Integer = _context.GetOrdinal("ATC_DESCCITTADINANZA")
                    Dim ATC_COMRES As Integer = _context.GetOrdinal("ATC_COMRES")
                    Dim ATC_COMDOM As Integer = _context.GetOrdinal("ATC_COMDOM")
                    Dim ATC_DOCTOR_MPI As Integer = _context.GetOrdinal("ATC_DOCTOR_MPI")
                    Dim ATC_DOCTOR_NAME As Integer = _context.GetOrdinal("ATC_DOCTOR_NAME")
                    Dim ATC_DOCTOR_SURNAME As Integer = _context.GetOrdinal("ATC_DOCTOR_SURNAME")
                    Dim ATC_CFMEDICO As Integer = _context.GetOrdinal("ATC_CFMEDICO")
                    Dim ATC_GUARITO As Integer = _context.GetOrdinal("ATC_GUARITO")
                    Dim ATC_DTGUARIGIONE As Integer = _context.GetOrdinal("ATC_DTGUARIGIONE")
                    Dim ATC_ST_ID As Integer = _context.GetOrdinal("ATC_ST_ID")
                    Dim ATC_ACQUISITO As Integer = _context.GetOrdinal("ATC_ACQUISITO")
                    Dim ATC_LOG_ERRORE As Integer = _context.GetOrdinal("ATC_LOG_ERRORE")

                    While _context.Read()
                        Dim Tampone As New TamponeDiFrontiera()
                        Tampone.Id = _context.GetInt32OrDefault(ATC_ID)
                        Tampone.CodiceFiscale = _context.GetStringOrDefault(ATC_CF)
                        Tampone.DataDiNascita = _context.GetDateTimeOrDefault(ATC_DATEOFBIRTH)
                        Tampone.Domicilio = _context.GetStringOrDefault(ATC_DOMICILE_ADDR)
                        Tampone.DomicilioCivico = _context.GetStringOrDefault(ATC_DOMICILE_CIV)
                        Tampone.DomicilioCap = _context.GetStringOrDefault(ATC_DOMICILE_CAP)
                        Tampone.DomicilioComune = _context.GetStringOrDefault(ATC_DOMICILE_COM)
                        Tampone.DomicilioProvincia = _context.GetStringOrDefault(ATC_DOMICILE_PROV)
                        Tampone.Sesso = _context.GetStringOrDefault(ATC_GENDER)
                        Tampone.CodiceMicrobiologico = _context.GetStringOrDefault(ATC_MICROBIOLOGY_CODE)
                        Tampone.NomeMicrobiologico = _context.GetStringOrDefault(ATC_MICROBIOLOGY_NAME)
                        Tampone.MPI = _context.GetDoubleOrDefault(ATC_MPI)
                        Tampone.Nome = _context.GetStringOrDefault(ATC_NAME)
                        Tampone.Cognome = _context.GetStringOrDefault(ATC_SURNAME)
                        Tampone.Note = _context.GetStringOrDefault(ATC_NOTE)
                        Tampone.NumreQ = _context.GetInt64(ATC_NUMREQ).ToString()
                        Tampone.LuogoDiNascita = _context.GetStringOrDefault(ATC_PLACEOFBIRTH)
                        Tampone.IndirizzoResidenza = _context.GetStringOrDefault(ATC_RESIDENCE_ADDR)
                        Tampone.ResidenzaCivico = _context.GetStringOrDefault(ATC_RESIDENCE_CIV)
                        Tampone.ResidenzaCap = _context.GetStringOrDefault(ATC_RESIDENCE_CAP)
                        Tampone.ResidenzaCom = _context.GetStringOrDefault(ATC_RESIDENCE_COM)
                        Tampone.ResidenzaProv = _context.GetStringOrDefault(ATC_RESIDENCE_PROV)
                        Tampone.Richiedente = _context.GetStringOrDefault(ATC_REQUESTOR)
                        Tampone.Risultato = _context.GetStringOrDefault(ATC_RESULT)
                        Tampone.SamplingDate = _context.GetDateTimeOrDefault(ATC_SAMPLING_DATE)
                        Tampone.SamplingDateResponse = _context.GetDateTimeOrDefault(ATC_SAMPLING_DATE_RESPONSE)
                        Tampone.Ulss = _context.GetStringOrDefault(ATC_ULSS)
                        Tampone.DTDDecesso = _context.GetDateTimeOrDefault(ATC_DTDECESSO)
                        Tampone.DescCittadinanza = _context.GetStringOrDefault(ATC_DESCCITTADINANZA)
                        Tampone.Comres = _context.GetStringOrDefault(ATC_COMRES)
                        Tampone.ComDom = _context.GetStringOrDefault(ATC_COMDOM)
                        Tampone.DoctorMpi = _context.GetStringOrDefault(ATC_DOCTOR_MPI)
                        Tampone.DottoreNome = _context.GetStringOrDefault(ATC_DOCTOR_NAME)
                        Tampone.DottoreCognome = _context.GetStringOrDefault(ATC_DOCTOR_SURNAME)
                        Tampone.CfMedico = _context.GetStringOrDefault(ATC_CFMEDICO)
                        Tampone.Guarito = _context.GetStringOrDefault(ATC_GUARITO)
                        Tampone.DataGuarigione = _context.GetDateTimeOrDefault(ATC_DTGUARIGIONE)
                        Tampone.Stato_ID = _context.GetInt32OrDefault(ATC_ST_ID)
                        Tampone.Acquisito = _context.GetInt64OrDefault(ATC_ACQUISITO)
                        Tampone.LogErrore = _context.GetStringOrDefault(ATC_LOG_ERRORE)
                        result.Add(Tampone)
                    End While
                End Using
            End Using
        Finally
            ConditionalCloseConnection(ownConnection)
        End Try
        Return result
    End Function

    Public Function GetTamponiFrontiera() As List(Of TamponeDiFrontiera) Implements ICovid19Tamponi.GetTamponiFrontiera
        Dim result As New List(Of TamponeDiFrontiera)
        Dim ownConnection As Boolean = False
        Dim query As String = "SELECT ATC_ID, ATC_CF, ATC_DATEOFBIRTH, ATC_DOMICILE_ADDR, ATC_DOMICILE_CIV, ATC_DOMICILE_CAP, ATC_DOMICILE_COM, ATC_DOMICILE_PROV, ATC_GENDER, ATC_MICROBIOLOGY_CODE, ATC_MICROBIOLOGY_NAME, " +
            "ATC_MPI, ATC_NAME, ATC_SURNAME, ATC_NOTE, ATC_NUMREQ, ATC_PLACEOFBIRTH, ATC_RESIDENCE_ADDR, ATC_RESIDENCE_CIV, ATC_RESIDENCE_CAP, ATC_RESIDENCE_COM, ATC_RESIDENCE_PROV, ATC_REQUESTOR, " +
            "ATC_RESULT, ATC_SAMPLING_DATE, ATC_SAMPLING_DATE_RESPONSE, ATC_ULSS, ATC_DTDECESSO, ATC_DESCCITTADINANZA, ATC_COMRES, ATC_COMDOM, ATC_DOCTOR_MPI, ATC_DOCTOR_NAME, ATC_DOCTOR_SURNAME, ATC_CFMEDICO, " +
            "ATC_GUARITO, ATC_DTGUARIGIONE, ATC_ST_ID, ATC_ACQUISITO, ATC_LOG_ERRORE, ATC_PAZ_CODICE " +
            "FROM T_ANA_TAMPONI_COVID " +
            "WHERE ATC_ST_ID = 11 " +
            "OR (ATC_ST_ID = 12 AND UPPER(ATC_LOG_ERRORE) = 'NULLABLE OBJECT MUST HAVE A VALUE.' AND trunc(ATC_DATA_ELABORAZIONE) >= trunc(SYSDATE)) " +
            "ORDER BY ATC_SAMPLING_DATE_RESPONSE ASC"
        Try
            Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using _context As IDataReader = cmd.ExecuteReader()

                    Dim ATC_ID As Integer = _context.GetOrdinal("ATC_ID")
                    Dim ATC_CF As Integer = _context.GetOrdinal("ATC_CF")
                    Dim ATC_DATEOFBIRTH As Integer = _context.GetOrdinal("ATC_DATEOFBIRTH")
                    Dim ATC_DOMICILE_ADDR As Integer = _context.GetOrdinal("ATC_DOMICILE_ADDR")
                    Dim ATC_DOMICILE_CIV As Integer = _context.GetOrdinal("ATC_DOMICILE_CIV")
                    Dim ATC_DOMICILE_CAP As Integer = _context.GetOrdinal("ATC_DOMICILE_CAP")
                    Dim ATC_DOMICILE_COM As Integer = _context.GetOrdinal("ATC_DOMICILE_COM")
                    Dim ATC_DOMICILE_PROV As Integer = _context.GetOrdinal("ATC_DOMICILE_PROV")
                    Dim ATC_GENDER As Integer = _context.GetOrdinal("ATC_GENDER")
                    Dim ATC_MICROBIOLOGY_CODE As Integer = _context.GetOrdinal("ATC_MICROBIOLOGY_CODE")
                    Dim ATC_MICROBIOLOGY_NAME As Integer = _context.GetOrdinal("ATC_MICROBIOLOGY_NAME")
                    Dim ATC_MPI As Integer = _context.GetOrdinal("ATC_MPI")
                    Dim ATC_NAME As Integer = _context.GetOrdinal("ATC_NAME")
                    Dim ATC_SURNAME As Integer = _context.GetOrdinal("ATC_SURNAME")
                    Dim ATC_NOTE As Integer = _context.GetOrdinal("ATC_NOTE")
                    Dim ATC_NUMREQ As Integer = _context.GetOrdinal("ATC_NUMREQ")
                    Dim ATC_PLACEOFBIRTH As Integer = _context.GetOrdinal("ATC_PLACEOFBIRTH")
                    Dim ATC_RESIDENCE_ADDR As Integer = _context.GetOrdinal("ATC_RESIDENCE_ADDR")
                    Dim ATC_RESIDENCE_CIV As Integer = _context.GetOrdinal("ATC_RESIDENCE_CIV")
                    Dim ATC_RESIDENCE_CAP As Integer = _context.GetOrdinal("ATC_RESIDENCE_CAP")
                    Dim ATC_RESIDENCE_COM As Integer = _context.GetOrdinal("ATC_RESIDENCE_COM")
                    Dim ATC_RESIDENCE_PROV As Integer = _context.GetOrdinal("ATC_RESIDENCE_PROV")
                    Dim ATC_REQUESTOR As Integer = _context.GetOrdinal("ATC_REQUESTOR")
                    Dim ATC_RESULT As Integer = _context.GetOrdinal("ATC_RESULT")
                    Dim ATC_SAMPLING_DATE As Integer = _context.GetOrdinal("ATC_SAMPLING_DATE")
                    Dim ATC_SAMPLING_DATE_RESPONSE As Integer = _context.GetOrdinal("ATC_SAMPLING_DATE_RESPONSE")
                    Dim ATC_ULSS As Integer = _context.GetOrdinal("ATC_ULSS")
                    Dim ATC_DTDECESSO As Integer = _context.GetOrdinal("ATC_DTDECESSO")
                    Dim ATC_DESCCITTADINANZA As Integer = _context.GetOrdinal("ATC_DESCCITTADINANZA")
                    Dim ATC_COMRES As Integer = _context.GetOrdinal("ATC_COMRES")
                    Dim ATC_COMDOM As Integer = _context.GetOrdinal("ATC_COMDOM")
                    Dim ATC_DOCTOR_MPI As Integer = _context.GetOrdinal("ATC_DOCTOR_MPI")
                    Dim ATC_DOCTOR_NAME As Integer = _context.GetOrdinal("ATC_DOCTOR_NAME")
                    Dim ATC_DOCTOR_SURNAME As Integer = _context.GetOrdinal("ATC_DOCTOR_SURNAME")
                    Dim ATC_CFMEDICO As Integer = _context.GetOrdinal("ATC_CFMEDICO")
                    Dim ATC_GUARITO As Integer = _context.GetOrdinal("ATC_GUARITO")
                    Dim ATC_DTGUARIGIONE As Integer = _context.GetOrdinal("ATC_DTGUARIGIONE")
                    Dim ATC_ST_ID As Integer = _context.GetOrdinal("ATC_ST_ID")
                    Dim ATC_ACQUISITO As Integer = _context.GetOrdinal("ATC_ACQUISITO")
                    Dim ATC_LOG_ERRORE As Integer = _context.GetOrdinal("ATC_LOG_ERRORE")
                    Dim ATC_PAZ_CODICE As Integer = _context.GetOrdinal("ATC_PAZ_CODICE")

                    While _context.Read()
                        Dim Tampone As New TamponeDiFrontiera()
                        Tampone.Id = _context.GetInt32OrDefault(ATC_ID)
                        Tampone.CodiceFiscale = _context.GetStringOrDefault(ATC_CF)
                        Tampone.DataDiNascita = _context.GetDateTimeOrDefault(ATC_DATEOFBIRTH)
                        Tampone.Domicilio = _context.GetStringOrDefault(ATC_DOMICILE_ADDR)
                        Tampone.DomicilioCivico = _context.GetStringOrDefault(ATC_DOMICILE_CIV)
                        Tampone.DomicilioCap = _context.GetStringOrDefault(ATC_DOMICILE_CAP)
                        Tampone.DomicilioComune = _context.GetStringOrDefault(ATC_DOMICILE_COM)
                        Tampone.DomicilioProvincia = _context.GetStringOrDefault(ATC_DOMICILE_PROV)
                        Tampone.Sesso = _context.GetStringOrDefault(ATC_GENDER)
                        Tampone.CodiceMicrobiologico = _context.GetStringOrDefault(ATC_MICROBIOLOGY_CODE)
                        Tampone.NomeMicrobiologico = _context.GetStringOrDefault(ATC_MICROBIOLOGY_NAME)
                        Tampone.MPI = _context.GetDoubleOrDefault(ATC_MPI)
                        Tampone.Nome = _context.GetStringOrDefault(ATC_NAME)
                        Tampone.Cognome = _context.GetStringOrDefault(ATC_SURNAME)
                        Tampone.Note = _context.GetStringOrDefault(ATC_NOTE)
                        Tampone.NumreQ = _context.GetInt64(ATC_NUMREQ).ToString()
                        Tampone.LuogoDiNascita = _context.GetStringOrDefault(ATC_PLACEOFBIRTH)
                        Tampone.IndirizzoResidenza = _context.GetStringOrDefault(ATC_RESIDENCE_ADDR)
                        Tampone.ResidenzaCivico = _context.GetStringOrDefault(ATC_RESIDENCE_CIV)
                        Tampone.ResidenzaCap = _context.GetStringOrDefault(ATC_RESIDENCE_CAP)
                        Tampone.ResidenzaCom = _context.GetStringOrDefault(ATC_RESIDENCE_COM)
                        Tampone.ResidenzaProv = _context.GetStringOrDefault(ATC_RESIDENCE_PROV)
                        Tampone.Richiedente = _context.GetStringOrDefault(ATC_REQUESTOR)
                        Tampone.Risultato = _context.GetStringOrDefault(ATC_RESULT)
                        Tampone.SamplingDate = _context.GetDateTimeOrDefault(ATC_SAMPLING_DATE)
                        Tampone.SamplingDateResponse = _context.GetDateTimeOrDefault(ATC_SAMPLING_DATE_RESPONSE)
                        Tampone.Ulss = _context.GetStringOrDefault(ATC_ULSS)
                        Tampone.DTDDecesso = _context.GetDateTimeOrDefault(ATC_DTDECESSO)
                        Tampone.DescCittadinanza = _context.GetStringOrDefault(ATC_DESCCITTADINANZA)
                        Tampone.Comres = _context.GetStringOrDefault(ATC_COMRES)
                        Tampone.ComDom = _context.GetStringOrDefault(ATC_COMDOM)
                        Tampone.DoctorMpi = _context.GetStringOrDefault(ATC_DOCTOR_MPI)
                        Tampone.DottoreNome = _context.GetStringOrDefault(ATC_DOCTOR_NAME)
                        Tampone.DottoreCognome = _context.GetStringOrDefault(ATC_DOCTOR_SURNAME)
                        Tampone.CfMedico = _context.GetStringOrDefault(ATC_CFMEDICO)
                        Tampone.Guarito = _context.GetStringOrDefault(ATC_GUARITO)
                        Tampone.DataGuarigione = _context.GetDateTimeOrDefault(ATC_DTGUARIGIONE)
                        Tampone.Stato_ID = _context.GetInt32OrDefault(ATC_ST_ID)
                        Tampone.Acquisito = _context.GetInt64OrDefault(ATC_ACQUISITO)
                        Tampone.LogErrore = _context.GetStringOrDefault(ATC_LOG_ERRORE)
                        Tampone.PazCodice = _context.GetInt64OrDefault(ATC_PAZ_CODICE)
                        result.Add(Tampone)
                    End While
                End Using
            End Using
        Finally
            ConditionalCloseConnection(ownConnection)
        End Try
        Return result
    End Function
    Public Function UpdateError(errore As String, IdTampone As Integer, pazCodice As Long, dataElaborazione As DateTime) Implements ICovid19Tamponi.UpdateError
        Dim ownConnection As Boolean = False
        Dim query As String = "UPDATE T_ANA_TAMPONI_COVID " +
                              "SET ATC_LOG_ERRORE = :ATC_LOG_ERRORE, ATC_ST_ID = 12, ATC_PAZ_CODICE = :PAZCODICE, ATC_DATA_ELABORAZIONE = :ATC_DATA_ELABORAZIONE " +
                              "WHERE ATC_ID = :ATC_ID"

        Try
            Using cmd As OracleCommand = New OracleCommand(query, Connection)

                cmd.Parameters.AddWithValueOrDefault("ATC_LOG_ERRORE", errore)
                cmd.Parameters.AddWithValueOrDefault("ATC_ID", IdTampone)
                cmd.Parameters.AddWithValueOrDefault("PAZCODICE", pazCodice)
                cmd.Parameters.AddWithValueOrDefault("ATC_DATA_ELABORAZIONE", dataElaborazione)

                ownConnection = ConditionalOpenConnection(cmd)
                cmd.ExecuteNonQuery()

            End Using
        Finally
            ConditionalCloseConnection(ownConnection)
        End Try
    End Function
    Public Function UpdateErrorNoPaz(errore As String, IdTampone As Integer, dataElaborazione As DateTime) Implements ICovid19Tamponi.UpdateErrorNoPaz
        Dim ownConnection As Boolean = False
        Dim query As String = "UPDATE T_ANA_TAMPONI_COVID " +
                              "SET ATC_LOG_ERRORE = :ATC_LOG_ERRORE, ATC_ST_ID = 12, ATC_DATA_ELABORAZIONE = :ATC_DATA_ELABORAZIONE " +
                              "WHERE ATC_ID = :ATC_ID"

        Try
            Using cmd As OracleCommand = New OracleCommand(query, Connection)

                cmd.Parameters.AddWithValueOrDefault("ATC_LOG_ERRORE", errore)
                cmd.Parameters.AddWithValueOrDefault("ATC_ID", IdTampone)
                cmd.Parameters.AddWithValueOrDefault("ATC_DATA_ELABORAZIONE", dataElaborazione)

                ownConnection = ConditionalOpenConnection(cmd)
                cmd.ExecuteNonQuery()

            End Using
        Finally
            ConditionalCloseConnection(ownConnection)
        End Try
    End Function
    Public Function UpdateStatoElab(IdTampone As Integer, statoElab As Integer, pazCodice As Long, dataElab As DateTime) Implements ICovid19Tamponi.UpdateStatoElab
        Dim ownConnection As Boolean = False
        Dim query As String = "UPDATE T_ANA_TAMPONI_COVID " +
                              "SET ATC_ST_ID = :ATC_ST_ID, ATC_DATA_ELABORAZIONE = :ATC_DATA_ELABORAZIONE, ATC_PAZ_CODICE = :ATC_PAZ_CODICE " +
                              "WHERE ATC_ID = :ATC_ID"

        Try
            Using cmd As OracleCommand = New OracleCommand(query, Connection)

                cmd.Parameters.AddWithValueOrDefault("ATC_ID", IdTampone)
                cmd.Parameters.AddWithValueOrDefault("ATC_ST_ID", statoElab)
                cmd.Parameters.AddWithValueOrDefault("ATC_DATA_ELABORAZIONE", dataElab)
                cmd.Parameters.AddWithValueOrDefault("ATC_PAZ_CODICE", pazCodice)
                ownConnection = ConditionalOpenConnection(cmd)
                cmd.ExecuteNonQuery()

            End Using
        Finally
            ConditionalCloseConnection(ownConnection)
        End Try
    End Function
    Public Function UpdateStatoElab(IdTampone As Integer, statoElab As Integer, dataGuarigione As DateTime, messaggio As String, pazCodice As Long, dataElab As DateTime) Implements ICovid19Tamponi.UpdateStatoElab
        Dim ownConnection As Boolean = False
        Dim query As String = "UPDATE T_ANA_TAMPONI_COVID " +
                              "SET ATC_ST_ID = :ATC_ST_ID, ATC_DTGUARIGIONE = :ATC_DTGUARIGIONE, ATC_LOG_ERRORE = :ATC_LOG_ERRORE, ATC_DATA_ELABORAZIONE = :ATC_DATA_ELABORAZIONE, ATC_PAZ_CODICE = :ATC_PAZ_CODICE " +
                              "WHERE ATC_ID = :ATC_ID"

        Try
            Using cmd As OracleCommand = New OracleCommand(query, Connection)

                cmd.Parameters.AddWithValueOrDefault("ATC_ID", IdTampone)
                cmd.Parameters.AddWithValueOrDefault("ATC_ST_ID", statoElab)
                cmd.Parameters.AddWithValueOrDefault("ATC_DTGUARIGIONE", dataGuarigione)
                cmd.Parameters.AddWithValueOrDefault("ATC_LOG_ERRORE", messaggio)
                cmd.Parameters.AddWithValueOrDefault("ATC_DATA_ELABORAZIONE", dataElab)
                cmd.Parameters.AddWithValueOrDefault("ATC_PAZ_CODICE", pazCodice)


                ownConnection = ConditionalOpenConnection(cmd)
                cmd.ExecuteNonQuery()

            End Using
        Finally
            ConditionalCloseConnection(ownConnection)
        End Try
    End Function
    Public Function UpdateStatoElabSDG(IdTampone As Integer, statoElab As Integer, messaggio As String, pazCodice As Long, dataElab As DateTime) Implements ICovid19Tamponi.UpdateStatoElabSDG
        Dim ownConnection As Boolean = False
        Dim query As String = "UPDATE T_ANA_TAMPONI_COVID " +
                              "SET ATC_ST_ID = :ATC_ST_ID, ATC_LOG_ERRORE = :ATC_LOG_ERRORE, ATC_DATA_ELABORAZIONE = :ATC_DATA_ELABORAZIONE, ATC_PAZ_CODICE = :ATC_PAZ_CODICE " +
                              "WHERE ATC_ID = :ATC_ID"

        Try
            Using cmd As OracleCommand = New OracleCommand(query, Connection)

                cmd.Parameters.AddWithValueOrDefault("ATC_ID", IdTampone)
                cmd.Parameters.AddWithValueOrDefault("ATC_ST_ID", statoElab)
                cmd.Parameters.AddWithValueOrDefault("ATC_LOG_ERRORE", messaggio)
                cmd.Parameters.AddWithValueOrDefault("ATC_DATA_ELABORAZIONE", dataElab)
                cmd.Parameters.AddWithValueOrDefault("ATC_PAZ_CODICE", pazCodice)

                ownConnection = ConditionalOpenConnection(cmd)
                cmd.ExecuteNonQuery()

            End Using
        Finally
            ConditionalCloseConnection(ownConnection)
        End Try
    End Function
    Public Function UpdateStatoElab(IdTampone As Integer, statoElab As Integer, messaggio As String, pazCodice As Long, dataElab As DateTime) Implements ICovid19Tamponi.UpdateStatoElab2
        Dim ownConnection As Boolean = False
        Dim query As String = "UPDATE T_ANA_TAMPONI_COVID " +
                              "SET ATC_ST_ID = :ATC_ST_ID, ATC_LOG_ERRORE = :ATC_LOG_ERRORE, ATC_DATA_ELABORAZIONE = :ATC_DATA_ELABORAZIONE, ATC_PAZ_CODICE = :ATC_PAZ_CODICE " +
                              "WHERE ATC_ID = :ATC_ID"

        Try
            Using cmd As OracleCommand = New OracleCommand(query, Connection)

                cmd.Parameters.AddWithValueOrDefault("ATC_ID", IdTampone)
                cmd.Parameters.AddWithValueOrDefault("ATC_ST_ID", statoElab)
                cmd.Parameters.AddWithValueOrDefault("ATC_LOG_ERRORE", messaggio)
                cmd.Parameters.AddWithValueOrDefault("ATC_DATA_ELABORAZIONE", dataElab)
                cmd.Parameters.AddWithValueOrDefault("ATC_PAZ_CODICE", pazCodice)

                ownConnection = ConditionalOpenConnection(cmd)
                cmd.ExecuteNonQuery()

            End Using
        Finally
            ConditionalCloseConnection(ownConnection)
        End Try
    End Function



    Private Sub InsertLogTamponeOrfano(idTampone As Integer, idPaz As Long, dataModifica As DateTime, note As String)
        DoCommand(Sub(cmd)
                      cmd.CommandText = "INSERT INTO T_LOG_TAMPONI_COVID_MOD " +
                                "(TCM_ATC_ID, TCM_PAZ_CODICE_MODIFICA, TCM_DATA_MODIFICA, TCM_NOTE_LOG ) " +
                                "VALUES " +
                                "(:TCM_ATC_ID, :TCM_PAZ_CODICE_MODIFICA, :TCM_DATA_MODIFICA, :TCM_NOTE_LOG) "

                      cmd.AddParameter("TCM_ATC_ID", idTampone)
                      cmd.AddParameter("TCM_PAZ_CODICE_MODIFICA", idPaz)
                      cmd.AddParameter("TCM_DATA_MODIFICA", dataModifica)
                      cmd.AddParameter("TCM_NOTE_LOG", note)
                      cmd.ExecuteNonQuery()
                  End Sub, IsolationLevel.ReadCommitted)
    End Sub
    Public Function GetAllTamponiById(Id As Long) As List(Of TamponeDatiRidotti) Implements ICovid19Tamponi.GetAllTamponiById
        Return DoCommand(Function(cmd)
                             cmd.CommandText = "SELECT 
	                                                e.PES_ID AS CodiceEpisodio,
	                                                t.PET_ID_CAMPIONE AS CodiceCampione,
	                                                t.PET_CODICE_LABORATORIO AS CodiceLaboratorio,
                                                    NULL as DescrizioneLaboratorio,
	                                                e.PES_USL_CODICE_INSERIMENTO AS CodiceUlssInserimento,
	                                                e.PES_USL_CODICE_RACCOLTA AS CodiceUlssRaccolta,
	                                                t.PET_DATA_TAMPONE AS DataTampone,
	                                                t.PET_DATA_REFERTO AS DataReferto,
	                                                ct.COD_DESCRIZIONE AS Esito,
	                                                e.PES_PAZ_CODICE AS CodicePaziente,
	                                                u.USL_DESCRIZIONE AS DescrizioneUlssInserimento
                                                FROM T_PAZ_EPISODI e
                                                JOIN T_PAZ_EPISODI_TAMPONI t ON t.PET_PES_ID = e.PES_ID 
                                                LEFT JOIN T_ANA_CODIFICHE ct ON ct.COD_CODICE = t.PET_ESITO AND ct.COD_CAMPO = 'PET_ESITO'
                                                LEFT JOIN T_ANA_USL u ON u.usl_codice = e.PES_USL_CODICE_INSERIMENTO 
                                                WHERE pes_paz_codice = ?paz_codice
                                                UNION (
	                                                SELECT 
		                                                NULL AS CODICEEPISODIO,
		                                                to_char(tt.ATC_NUMREQ),
		                                                tt.ATC_MICROBIOLOGY_CODE,
                                                        tt.ATC_MICROBIOLOGY_NAME,
		                                                tt.ATC_ULSS AS CodiceUlssInserimento,
		                                                NULL,
		                                                tt.ATC_SAMPLING_DATE,
		                                                tt.ATC_SAMPLING_DATE_RESPONSE,
		                                                tt.ATC_RESULT,
		                                                tt.ATC_PAZ_CODICE,
		                                                uu.USL_DESCRIZIONE 
	                                                FROM T_ANA_TAMPONI_COVID tt 
	                                                LEFT JOIN t_ana_usl uu ON uu.usl_codice = tt.ATC_ULSS 
	                                                WHERE tt.ATC_PAZ_CODICE = ?paz_codice
	                                                )"
                             cmd.AddParameter("paz_codice", Id)

                             Return cmd.Fill(Of TamponeDatiRidotti)
                         End Function)
    End Function

    Public Sub UpdateUlssTamponeOrfanp(idTampone As Long, ulss As String) Implements ICovid19Tamponi.UpdateUlssTamponeOrfanp
        DoCommand(Sub(cmd)
                      cmd.CommandText = "update T_ANA_TAMPONI_COVID set ATC_ULSS = :ulss where ATC_ID = :ID"
                      cmd.AddParameter("ulss", ulss)
                      cmd.AddParameter("ID", idTampone)
                      cmd.ExecuteNonQuery()
                      RielaboraTamponeOrfano(idTampone)
                  End Sub, IsolationLevel.ReadCommitted)
    End Sub

    Public Sub UpdateTamponeOrfano(idTampone As Long, codiceFiscale As String, pazCodice As Long, dataModifica As DateTime, note As String, ulss As String) Implements ICovid19Tamponi.UpdateTamponeOrfano
        DoCommand(Sub(cmd)
                      cmd.CommandText = "UPDATE T_ANA_TAMPONI_COVID " +
                             "SET ATC_CF = :ATC_CF, ATC_PAZ_CODICE = :ATC_PAZ_CODICE "


                      cmd.AddParameter("ATC_CF", codiceFiscale)
                      cmd.AddParameter("ATC_PAZ_CODICE", pazCodice)
                      cmd.AddParameter("ATC_ID", idTampone)

                      If Not String.IsNullOrWhiteSpace(ulss) Then
                          cmd.CommandText += ", ATC_ULSS = :ATC_ULSS "
                          cmd.AddParameter("ATC_ULSS", ulss)
                      End If

                      cmd.CommandText += " WHERE ATC_ID = :ATC_ID"
                      cmd.ExecuteNonQuery()

                      RielaboraTamponeOrfano(idTampone)

                      InsertLogTamponeOrfano(idTampone, pazCodice, dataModifica, note)

                  End Sub, IsolationLevel.ReadCommitted)

    End Sub

    Private Sub RielaboraTamponeOrfano(idTampone As Long)
        DoCommand(Sub(cmd)
                      Dim codicePaziente As Long? = Nothing
                      Dim data As Date?

                      cmd.CommandText = "select ATC_PAZ_CODICE, ATC_ULSS, ATC_SAMPLING_DATE_RESPONSE from T_ANA_TAMPONI_COVID where ATC_ID = ?id"
                      cmd.AddParameter("id", idTampone)
                      Dim rielabora As Boolean = False
                      Using reader As IDataReader = cmd.ExecuteReader()
                          If reader.Read() Then
                              codicePaziente = reader.GetNullableInt64OrDefault(0)

                              rielabora = codicePaziente.HasValue AndAlso Not String.IsNullOrWhiteSpace(reader.GetStringOrDefault(1))
                              data = reader.GetNullableDateTimeOrDefault(2)
                          End If
                      End Using


                      If rielabora AndAlso data.HasValue AndAlso codicePaziente.HasValue Then
                          cmd.UpdateTable("T_ANA_TAMPONI_COVID", New With {Key .ATC_ST_ID = 11}, New With {Key .ATC_ID = idTampone})
                          cmd.Parameters.Clear()
                          cmd.CommandText = "update T_ANA_TAMPONI_COVID set ATC_ST_ID = ?nuovoStato where ATC_PAZ_CODICE is not null and ATC_PAZ_CODICE = ?paz and ATC_ULSS is not null and ATC_SAMPLING_DATE_RESPONSE > ?data"
                          cmd.AddParameter("paz", codicePaziente)
                          cmd.AddParameter("nuovoStato", 11)
                          cmd.AddParameter("data", data.Value)
                          cmd.ExecuteNonQuery()
                      End If

                  End Sub, IsolationLevel.ReadCommitted)
    End Sub

    Public Function ElencoEsitiOrfani(q As String) As IEnumerable(Of String) Implements ICovid19Tamponi.ElencoEsitiOrfani
        Return DoCommand(Function(cmd)
                             cmd.CommandText = "SELECT DISTINCT UPPER(ATC_RESULT) FROM T_ANA_TAMPONI_COVID tatc"
                             If Not String.IsNullOrWhiteSpace(q) Then
                                 cmd.CommandText += " WHERE UPPER(ATC_RESULT) LIKE ?Q"
                                 cmd.AddParameter("Q", q.ToUpper())
                             End If
                             Return cmd.Fill(Of String)
                         End Function)
    End Function

    Function ContaTamponiPaziente(id As Long) As Integer Implements ICovid19Tamponi.ContaTamponiPaziente
        Dim codice As Integer = DoCommand(Function(cmd)
                                              cmd.CommandText = "SELECT count(*) FROM (
                                                    SELECT PET_ID_CAMPIONE, pes_paz_codice FROM T_PAZ_EPISODI_TAMPONI
                                                    JOIN T_PAZ_EPISODI ON pet_pes_id = pes_id
                                                    WHERE PES_PAZ_CODICE = ?paz and PET_TIPO_TAMPONE <> 2
                                                    UNION (
	                                                    SELECT to_char(ATC_NUMREQ), atc_paz_codice FROM T_ANA_TAMPONI_COVID WHERE ATC_PAZ_CODICE = ?paz
                                                    )
                                                ) a WHERE pet_id_campione IS NOT NULL"
                                              cmd.AddParameter("paz", id)
                                              Return cmd.FirstOrDefault(Of Integer)
                                          End Function)
        Dim no As Integer = DoCommand(Function(cmd)
                                          cmd.CommandText = "SELECT count(*) FROM (
                                                                SELECT PET_ID_CAMPIONE, pes_paz_codice FROM T_PAZ_EPISODI_TAMPONI
                                                                JOIN T_PAZ_EPISODI ON pet_pes_id = pes_id
                                                                WHERE PES_PAZ_CODICE = ?paz AND pet_id_campione IS NULL AND PET_TIPO_TAMPONE <> 2
                                                                UNION (
	                                                                SELECT to_char(ATC_NUMREQ), atc_paz_codice FROM T_ANA_TAMPONI_COVID WHERE ATC_PAZ_CODICE = ?paz AND ATC_NUMREQ IS null
                                                                )
                                                            ) a"
                                          cmd.AddParameter("paz", id)
                                          Return cmd.FirstOrDefault(Of Integer)()
                                      End Function)
        Return codice + no
    End Function

End Class
