Imports System.Collections.Generic
Imports System.Data.OracleClient
Imports System.IO
Imports System.Text
Imports System.Web.Script.Serialization
Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.Data.OracleClient
Imports Onit.OnAssistnet.OnVac.Entities.RicercaEpisodi

Namespace DAL.Oracle
    Public Class DbRilevazioniCovid19Provider
        Inherits DbProvider
        Implements IRilevazioniCovid19Provider

        Private Const CodiceTipoCasoContattoScolastico As String = "SL"

#Region " Consts "

        'TODO: sposta query definite nei metodi nella region delle costanti
        Private Const QueryInsertTamponi As String = "INSERT INTO T_PAZ_EPISODI_TAMPONI (
                                                        PET_PES_ID,
                                                        PET_USL_CODICE,
                                                        PET_DATA_TAMPONE,
                                                        PET_ESITO,
                                                        PET_NOTE,
                                                        PET_UTE_ID_INSERIMENTO,
                                                        PET_USL_CODICE_INSERIMENTO,
                                                        PET_DATA_INSERIMENTO,
                                                        PET_ID_CAMPIONE,
                                                        PET_CODICE_LABORATORIO,
                                                        PET_FLG_DA_VISIONARE,
                                                        PET_DATA_RICHIESTA,
                                                        PET_DATA_REFERTO,
                                                        PET_TLM_ID,
                                                        PET_TIPO_TAMPONE
                                                        )
                                                        VALUES (
                                                        :PET_PES_ID,
                                                        :PET_USL_CODICE,
                                                        :PET_DATA_TAMPONE,
                                                        :PET_ESITO,
                                                        :PET_NOTE,
                                                        :PET_UTE_ID_INSERIMENTO,
                                                        :PET_USL_CODICE_INSERIMENTO,
                                                        :PET_DATA_INSERIMENTO,
                                                        :PET_ID_CAMPIONE,
                                                        :PET_CODICE_LABORATORIO,
                                                        :PET_FLG_DA_VISIONARE,
                                                        :PET_DATA_RICHIESTA,
                                                        :PET_DATA_REFERTO,
                                                        :PET_TLM_ID,
                                                        :PET_TIPO_TAMPONE
                                                        )"

        Private Const QueryUpdateTamponi As String = "UPDATE T_PAZ_EPISODI_TAMPONI SET
                                                        PET_USL_CODICE = :PET_USL_CODICE,
                                                        PET_DATA_TAMPONE = :PET_DATA_TAMPONE,
                                                        PET_ESITO = :PET_ESITO,
                                                        PET_NOTE = :PET_NOTE,
                                                        PET_ID_CAMPIONE = :PET_ID_CAMPIONE,
                                                        PET_CODICE_LABORATORIO = :PET_CODICE_LABORATORIO,
                                                        PET_FLG_DA_VISIONARE = :PET_FLG_DA_VISIONARE,
                                                        PET_DATA_RICHIESTA = :PET_DATA_RICHIESTA,
                                                        PET_DATA_REFERTO = :PET_DATA_REFERTO,
                                                        PET_TLM_ID = :PET_TLM_ID,
                                                        PET_TIPO_TAMPONE = :PET_TIPO_TAMPONE
                                                            WHERE PET_ID = :idTampone"

        Private Const QueryInsertContatti As String = "INSERT INTO T_PAZ_EPISODI_CONTATTI (
                                                        PEC_PES_ID,
                                                        PEC_TIPO_RAPPORTO,
                                                        PEC_NOTE,
                                                        PEC_UTE_ID_INSERIMENTO,
                                                        PEC_USL_CODICE_INSERIMENTO,
                                                        PEC_DATA_INSERIMENTO,
                                                        PEC_PAZ_CODICE,
                                                        PEC_TELEFONO,
                                                        PEC_PES_ID_CONTATTO,
                                                        PEC_ECE_GRUPPO
                                                        )
                                                        VALUES (
                                                        :PEC_PES_ID,
                                                        :PEC_TIPO_RAPPORTO,
                                                        :PEC_NOTE,
                                                        :PEC_UTE_ID_INSERIMENTO,
                                                        :PEC_USL_CODICE_INSERIMENTO,
                                                        :PEC_DATA_INSERIMENTO,
                                                        :PEC_PAZ_CODICE,
                                                        :PEC_TELEFONO,
                                                        :PEC_PES_ID_CONTATTO,
                                                        :PEC_ECE_GRUPPO
                                                        )"


        Private Const QueryUpdateContatti As String = "UPDATE T_PAZ_EPISODI_CONTATTI SET
                                                        PEC_TIPO_RAPPORTO = :PEC_TIPO_RAPPORTO,
                                                        PEC_NOTE = :PEC_NOTE,
                                                        PEC_DATA_INSERIMENTO = :PEC_DATA_INSERIMENTO,
                                                        PEC_TELEFONO = :PEC_TELEFONO,
                                                        PEC_ECE_GRUPPO = :PEC_ECE_GRUPPO
                                                            WHERE PEC_ID = :idContatto"

        Private Const QueryRicercaEpisodiAttiviByPaziente As String = "select PES_ID 
                                                                          from T_PAZ_EPISODI 
                                                                            left outer join T_ANA_STATI_EPISODIO on PES_SEP_CODICE = SEP_CODICE
                                                                              where PES_PAZ_CODICE = :codicePaziente
                                                                              AND (SEP_ATTIVO = 'S' or SEP_ATTIVO is null)
                                                                              and ROWNUM = 1"

        Private Const QueryTipoCasoDefault As String = "select COD_CODICE from T_ANA_CODIFICHE WHERE COD_CAMPO = 'PES_TIPO_CASO' and COD_DEFAULT = 'S' and ROWNUM = 1"

        Private Const QueryInsertEpisodioPerContatto As String = "INSERT INTO T_PAZ_EPISODI (
                                                                    PES_ID,
                                                                    PES_PAZ_CODICE,
                                                                    PES_TIPO_CASO,
                                                                    PES_DATA_INSERIMENTO,
                                                                    PES_DATA_SEGNALAZIONE,
                                                                    PES_USL_CODICE_RACCOLTA,
                                                                    PES_USL_CODICE_INSERIMENTO,
                                                                    PES_UTE_ID_INSERIMENTO,
                                                                    PES_TELEFONO,
                                                                    PES_NOTE
                                                                    )
                                                                    VALUES (
                                                                    :PES_ID,
                                                                    :PES_PAZ_CODICE,
                                                                    :PES_TIPO_CASO,
                                                                    :PES_DATA_INSERIMENTO,
                                                                    :PES_DATA_SEGNALAZIONE,
                                                                    :PES_USL_CODICE_RACCOLTA,
                                                                    :PES_USL_CODICE_INSERIMENTO,
                                                                    :PES_UTE_ID_INSERIMENTO,
                                                                    :PES_TELEFONO,
                                                                    :PES_NOTE
                                                                    )"



        Private Const QueryInserimentoDatoreLavoroEmpty As String = "INSERT INTO T_PAZ_EPISODI_DATORE_LAVORO (
                                                                        PEL_PES_ID
                                                                        )
                                                                        VALUES (
                                                                        :PEL_PES_ID
                                                                        )"

        Private Const QueryInserimentoClinicaEmpty As String = "INSERT INTO T_PAZ_EPISODI_CLINICA (
                                                                    PCL_PES_ID
                                                                    )
                                                                    VALUES (
                                                                    :PCL_PES_ID
                                                                    )"

        Private Const QueryDeleteContattiRiferimento As String = "DELETE FROM T_PAZ_EPISODI_CONTATTI
                                                                    WHERE PEC_PES_ID_CONTATTO = :idEpisodioRiferimento"

        Private Const QueryInsertEpisodioEliminato As String = "insert into T_PAZ_EPISODI_ELIMINATI(
                                                                                                    PEE_PES_ID,
                                                                                                    PEE_PAZ_CODICE,
                                                                                                    PEE_TIPO_CASO,
                                                                                                    PEE_ATC_CODICE,
                                                                                                    PEE_USL_CODICE_RACCOLTA,
                                                                                                    PEE_ASE_CODICE,
                                                                                                    PEE_SEP_CODICE,
                                                                                                    PEE_FLAG_OPERATORE_SANITARIO,
                                                                                                    PEE_TIPO_OPERATORE_SANITARIO,
                                                                                                    PEE_PATOLOGIE_CRONICHE,
                                                                                                    PEE_DATA_SEGNALAZIONE,
                                                                                                    PEE_COMUNE_ESPOSIZIONE,
                                                                                                    PEE_DATA_INIZIO_ISOLAMENTO,
                                                                                                    PEE_DATA_ULTIMO_CONTATTO,
                                                                                                    PEE_DATA_FINE_ISOLAMENTO,
                                                                                                    PEE_DATA_CHIUSURA,
                                                                                                    PEE_ASINTOMATICO,
                                                                                                    PEE_DATA_INIZIO_SINTOMI,
                                                                                                    PEE_NOTE,
                                                                                                    PEE_UTE_ID_INSERIMENTO,
                                                                                                    PEE_USL_CODICE_INSERIMENTO,
                                                                                                    PEE_DATA_INSERIMENTO,
                                                                                                    PEE_UTE_ID_ULTIMA_MODIFCA,
                                                                                                    PEE_DATA_ULTIMA_MODIFICA,
                                                                                                    PEE_PAZ_CODICE_OLD,
                                                                                                    PEE_INDIRIZZO_ISOLAMENTO,
                                                                                                    PEE_COM_CODICE_ISOLAMENTO,
                                                                                                    PEE_TELEFONO,
                                                                                                    PEE_EMAIL,
                                                                                                    PEE_DATA_DECESSO_COVID,
                                                                                                    PEE_DETTAGLIO,
                                                                                                    PEE_UTE_ID_ELIMINAZIONE,
                                                                                                    PEE_USL_CODICE_ELIMINAZIONE,
                                                                                                    PEE_DATA_ELIMINAZIONE
                                                                                                    ) 
                                                                                                    select 
                                                                                                    PES_ID,
                                                                                                    PES_PAZ_CODICE,
                                                                                                    PES_TIPO_CASO,
                                                                                                    PES_ATC_CODICE,
                                                                                                    PES_USL_CODICE_RACCOLTA,
                                                                                                    PES_ASE_CODICE,
                                                                                                    PES_SEP_CODICE,
                                                                                                    PES_FLAG_OPERATORE_SANITARIO,
                                                                                                    PES_TIPO_OPERATORE_SANITARIO,
                                                                                                    PES_PATOLOGIE_CRONICHE,
                                                                                                    PES_DATA_SEGNALAZIONE,
                                                                                                    PES_COMUNE_ESPOSIZIONE,
                                                                                                    PES_DATA_INIZIO_ISOLAMENTO,
                                                                                                    PES_DATA_ULTIMO_CONTATTO,
                                                                                                    PES_DATA_FINE_ISOLAMENTO,
                                                                                                    PES_DATA_CHIUSURA,
                                                                                                    PES_ASINTOMATICO,
                                                                                                    PES_DATA_INIZIO_SINTOMI,
                                                                                                    PES_NOTE,
                                                                                                    PES_UTE_ID_INSERIMENTO,
                                                                                                    PES_USL_CODICE_INSERIMENTO,
                                                                                                    PES_DATA_INSERIMENTO,
                                                                                                    PES_UTE_ID_ULTIMA_MODIFCA,
                                                                                                    PES_DATA_ULTIMA_MODIFICA,
                                                                                                    PES_PAZ_CODICE_OLD,
                                                                                                    PES_INDIRIZZO_ISOLAMENTO,
                                                                                                    PES_COM_CODICE_ISOLAMENTO,
                                                                                                    PES_TELEFONO,
                                                                                                    PES_EMAIL,
                                                                                                    PES_DATA_DECESSO_COVID,
                                                                                                    :dettaglio as Dettaglio,
                                                                                                    :uteEliminazione as UteEliminazione,
                                                                                                    :uslEliminazione as UslEliminazione,
                                                                                                    :dataEliminazione as DataEliminazione
                                                                                                      from T_PAZ_EPISODI
                                                                                                        where PES_ID = :idEpisodio"

        Private Const QueryInvioCredenzialiApp As String = "update T_PAZ_PAZIENTI SET 
                                                                PAZ_CREDENZIALI_APP = 'S'
                                                                    where PAZ_CODICE = :pazCodice"

        Private Const QueryUpdateUtenteInserimentoTampone As String = "UPDATE T_PAZ_EPISODI_TAMPONI SET
                                                                          PET_UTE_ID_INSERIMENTO = :PET_UTE_ID_INSERIMENTO
                                                                            where PET_ID = :idTampone"

        Private Const QueryInsertClinica As String = "INSERT INTO T_PAZ_EPISODI_CLINICA (
                                                                                        PCL_PES_ID,
                                                                                        PCL_TUMORE,
                                                                                        PCL_DIABETE,
                                                                                        PCL_MAL_CARDIOVASCOLARI,
                                                                                        PCL_DEF_IMMUNITARI,
                                                                                        PCL_MAL_RESPIRATORIE,
                                                                                        PCL_MAL_RENALI,
                                                                                        PCL_MAL_METABOLICHE,
                                                                                        PCL_ALTRO,
                                                                                        PCL_OBESITA_BMI_30_40,
                                                                                        PCL_OBESITA_BMI_MAGGIORE_40,
                                                                                        PCL_UTE_ID_INSERIMENTO,
                                                                                        PCL_USL_CODICE_INSERIMENTO,
                                                                                        PCL_DATA_INSERIMENTO,
                                                                                        PCL_NOTE
                                                                                        )
                                                                                        VALUES (
                                                                                        :PCL_PES_ID,
                                                                                        :PCL_TUMORE,
                                                                                        :PCL_DIABETE,
                                                                                        :PCL_MAL_CARDIOVASCOLARI,
                                                                                        :PCL_DEF_IMMUNITARI,
                                                                                        :PCL_MAL_RESPIRATORIE,
                                                                                        :PCL_MAL_RENALI,
                                                                                        :PCL_MAL_METABOLICHE,
                                                                                        :PCL_ALTRO,
                                                                                        :PCL_OBESITA_BMI_30_40,
                                                                                        :PCL_OBESITA_BMI_MAGGIORE_40,
                                                                                        :PCL_UTE_ID_INSERIMENTO,
                                                                                        :PCL_USL_CODICE_INSERIMENTO,
                                                                                        :PCL_DATA_INSERIMENTO,
                                                                                        :PCL_NOTE
                                                                                        )"

        Private Const QueryUpdateClinica As String = "UPDATE T_PAZ_EPISODI_CLINICA SET
                                                          PCL_TUMORE = :PCL_TUMORE,
                                                          PCL_DIABETE = :PCL_DIABETE,
                                                          PCL_MAL_CARDIOVASCOLARI = :PCL_MAL_CARDIOVASCOLARI,
                                                          PCL_DEF_IMMUNITARI = :PCL_DEF_IMMUNITARI,
                                                          PCL_MAL_RESPIRATORIE = :PCL_MAL_RESPIRATORIE,
                                                          PCL_MAL_RENALI = :PCL_MAL_RENALI,
                                                          PCL_MAL_METABOLICHE = :PCL_MAL_METABOLICHE,
                                                          PCL_ALTRO = :PCL_ALTRO,
                                                          PCL_OBESITA_BMI_30_40 = :PCL_OBESITA_BMI_30_40,
                                                          PCL_OBESITA_BMI_MAGGIORE_40 = :PCL_OBESITA_BMI_MAGGIORE_40,
                                                          PCL_NOTE = :PCL_NOTE
                                                            WHERE PCL_PES_ID = :idEpisodio"

#End Region

#Region " Constructors "
        Public Sub New(ByRef DAM As IDAM)
            MyBase.New(DAM)
        End Sub
#End Region

#Region " Metodi Pubblici "
        Public Function GetCodiceGuaritoClinicamente() As Integer? Implements IRilevazioniCovid19Provider.GetCodiceGuaritoClinicamente
            Return DoCommand(Function(cmd)
                                 cmd.CommandText = "SELECT SEP_CODICE FROM T_ANA_STATI_EPISODIO tase WHERE SEP_GUARITO_CLINICO = 'S'"
                                 Return cmd.FirstOrDefault(Of Integer?)
                             End Function)
        End Function

        Public Function GetCodiceGuarito() As Integer? Implements IRilevazioniCovid19Provider.GetCodiceGuarito
            Return DoCommand(Function(cmd)
                                 cmd.CommandText = "SELECT SEP_CODICE FROM T_ANA_STATI_EPISODIO where SEP_DESCRIZIONE = :descrizione"
                                 cmd.AddParameter("descrizione", "GUARITO")
                                 Return cmd.FirstOrDefault(Of Integer?)
                             End Function)
        End Function


        Public Function GetTipiEventiRicovero() As List(Of TipoEventoRicovero) Implements IRilevazioniCovid19Provider.GetTipiEventiRicovero
            Return DoCommand(Function(cmd)
                                 cmd.CommandText = "select ATR_CODICE as Codice, 
                                                            ATR_DESCRIZIONE as Descrizione, 
                                                            ATR_APERTO as Aperto,
                                                            ATR_CHIUSO as Chiuso,
                                                            ATR_COLORE as Colore,
                                                            ATR_RIP_STC as CodiceStatoClinico,
                                                            c.COD_DESCRIZIONE as DescrizioneStatoClinico
                                                        from T_ANA_TIPI_RICOVERO
                                                        LEFT JOIN T_ANA_CODIFICHE c ON c.COD_CODICE = ATR_RIP_STC AND c.COD_CAMPO = 'RIP_STC'
                                                            where ATR_OBSOLETO is null or ATR_OBSOLETO = 'N'"
                                 Return cmd.Fill(Of TipoEventoRicovero)
                             End Function)
        End Function
        Public Function GetDatiPazienti(codicePaziente As List(Of Long)) As List(Of DatiPaziente) Implements IRilevazioniCovid19Provider.GetDatiPazienti
            Return DoCommand(Function(cmd)
                                 cmd.CommandText = String.Format("select PAZ_CODICE as Codice, PAZ_COGNOME Cognome,
                                               PAZ_NOME Nome,
                                               PAZ_CODICE_FISCALE Cf,
                                               PAZ_DATA_NASCITA DataNascita,
                                               PAZ_SESSO Sesso,
                                               COMUNE_NASCITA.COM_DESCRIZIONE ComuneNascita,
                                               COMUNE_RESIDENZA.COM_DESCRIZIONE ComuneResidenza,
                                               PAZ_INDIRIZZO_RESIDENZA IndirizzoResidenza,
                                               COMUNE_DOMICILIO.COM_DESCRIZIONE ComuneDomicilio,
                                               PAZ_INDIRIZZO_DOMICILIO IndirizzoDomicilio,
                                               PAZ_TELEFONO_1 TelefonoUno,
                                               PAZ_TELEFONO_2 TelefonoDue,
                                               PAZ_TELEFONO_3 TelefonoTre,
                                               PAZ_EMAIL Email,
                                               MED_NOME NomeMedico,
                                               MED_COGNOME CognomeMedico,
                                               MED_CODICE_FISCALE CfMedico
                                                from T_PAZ_PAZIENTI
                                                  left outer join T_ANA_COMUNI COMUNE_NASCITA on PAZ_COM_CODICE_NASCITA = COMUNE_NASCITA.COM_CODICE
                                                  left outer join T_ANA_COMUNI COMUNE_RESIDENZA on PAZ_COM_CODICE_RESIDENZA = COMUNE_RESIDENZA.COM_CODICE
                                                  left outer join T_ANA_COMUNI COMUNE_DOMICILIO on PAZ_COM_CODICE_DOMICILIO = COMUNE_DOMICILIO.COM_CODICE
                                                  left outer join T_ANA_MEDICI on PAZ_MED_CODICE_BASE = MED_CODICE
                                                    where PAZ_CODICE IN ({0})", cmd.SetParameterIn("P", codicePaziente))

                                 Return cmd.Fill(Of DatiPaziente)

                                 'Dim res As New List(Of DatiPaziente)
                                 'Using dr As OracleDataReader = cmd.ExecuteReader()
                                 '    If Not dr Is Nothing Then
                                 '        Dim Codice As Integer = dr.GetOrdinal("Codice")
                                 '        Dim Cognome As Integer = dr.GetOrdinal("Cognome")
                                 '        Dim Nome As Integer = dr.GetOrdinal("Nome")
                                 '        Dim Cf As Integer = dr.GetOrdinal("Cf")
                                 '        Dim DataNascita As Integer = dr.GetOrdinal("DataNascita")
                                 '        Dim Sesso As Integer = dr.GetOrdinal("Sesso")
                                 '        Dim ComuneNascita As Integer = dr.GetOrdinal("ComuneNascita")
                                 '        Dim ComuneResidenza As Integer = dr.GetOrdinal("ComuneResidenza")
                                 '        Dim IndirizzoResidenza As Integer = dr.GetOrdinal("IndirizzoResidenza")
                                 '        Dim ComuneDomicilio As Integer = dr.GetOrdinal("ComuneDomicilio")
                                 '        Dim IndirizzoDomicilio As Integer = dr.GetOrdinal("IndirizzoDomicilio")
                                 '        Dim TelefonoUno As Integer = dr.GetOrdinal("TelefonoUno")
                                 '        Dim TelefonoDue As Integer = dr.GetOrdinal("TelefonoDue")
                                 '        Dim TelefonoTre As Integer = dr.GetOrdinal("TelefonoTre")
                                 '        Dim Email As Integer = dr.GetOrdinal("Email")
                                 '        Dim NomeMedico As Integer = dr.GetOrdinal("NomeMedico")
                                 '        Dim CognomeMedico As Integer = dr.GetOrdinal("CognomeMedico")
                                 '        Dim CfMedico As Integer = dr.GetOrdinal("CfMedico")

                                 '        While dr.Read
                                 '            Dim result As New DatiPaziente
                                 '            result.Codice = dr.GetInt64(Codice)
                                 '            result.Cognome = dr.GetStringOrDefault(Cognome)
                                 '            result.Nome = dr.GetStringOrDefault(Nome)
                                 '            result.Cf = dr.GetStringOrDefault(Cf)
                                 '            result.DataNascita = dr.GetNullableDateTimeOrDefault(DataNascita)
                                 '            result.Sesso = dr.GetStringOrDefault(Sesso)
                                 '            result.ComuneNascita = dr.GetStringOrDefault(ComuneNascita)
                                 '            result.ComuneResidenza = dr.GetStringOrDefault(ComuneResidenza)
                                 '            result.IndirizzoResidenza = dr.GetStringOrDefault(IndirizzoResidenza)
                                 '            result.ComuneDomicilio = dr.GetStringOrDefault(ComuneDomicilio)
                                 '            result.IndirizzoDomicilio = dr.GetStringOrDefault(IndirizzoDomicilio)
                                 '            result.TelefonoUno = dr.GetStringOrDefault(TelefonoUno)
                                 '            result.TelefonoDue = dr.GetStringOrDefault(TelefonoDue)
                                 '            result.TelefonoTre = dr.GetStringOrDefault(TelefonoTre)
                                 '            result.Email = dr.GetStringOrDefault(Email)
                                 '            result.NomeMedico = dr.GetStringOrDefault(NomeMedico)
                                 '            result.CognomeMedico = dr.GetStringOrDefault(CognomeMedico)
                                 '            result.CfMedico = dr.GetStringOrDefault(CfMedico)
                                 '            res.Add(result)
                                 '        End While
                                 '    End If
                                 'End Using
                             End Function)
        End Function
        Public Function GetDatiPaziente(codicePaziente As Long) As DatiPaziente Implements IRilevazioniCovid19Provider.GetDatiPaziente
            Dim result As DatiPaziente = Nothing
            Using cmd As OracleCommand = New OracleCommand
                cmd.Connection = Connection
                Dim ownConnection As Boolean = False
                Try
                    ownConnection = ConditionalOpenConnection(cmd)
                    Dim query As String = "select PAZ_COGNOME Cognome,
                                               PAZ_NOME Nome,
                                               PAZ_CODICE_FISCALE Cf,
                                               PAZ_DATA_NASCITA DataNascita,
                                               PAZ_SESSO Sesso,
                                               COMUNE_NASCITA.COM_DESCRIZIONE ComuneNascita,
                                               COMUNE_RESIDENZA.COM_DESCRIZIONE ComuneResidenza,
                                               PAZ_INDIRIZZO_RESIDENZA IndirizzoResidenza,
                                               COMUNE_DOMICILIO.COM_DESCRIZIONE ComuneDomicilio,
                                               PAZ_INDIRIZZO_DOMICILIO IndirizzoDomicilio,
                                               PAZ_TELEFONO_1 TelefonoUno,
                                               PAZ_TELEFONO_2 TelefonoDue,
                                               PAZ_TELEFONO_3 TelefonoTre,
                                               PAZ_EMAIL Email,
                                               MED_NOME NomeMedico,
                                               MED_COGNOME CognomeMedico,
                                               MED_CODICE_FISCALE CfMedico
                                                from T_PAZ_PAZIENTI
                                                  left outer join T_ANA_COMUNI COMUNE_NASCITA on PAZ_COM_CODICE_NASCITA = COMUNE_NASCITA.COM_CODICE
                                                  left outer join T_ANA_COMUNI COMUNE_RESIDENZA on PAZ_COM_CODICE_RESIDENZA = COMUNE_RESIDENZA.COM_CODICE
                                                  left outer join T_ANA_COMUNI COMUNE_DOMICILIO on PAZ_COM_CODICE_DOMICILIO = COMUNE_DOMICILIO.COM_CODICE
                                                  left outer join T_ANA_MEDICI on PAZ_MED_CODICE_BASE = MED_CODICE
                                                    where PAZ_CODICE = :codicePaziente"

                    cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)
                    cmd.CommandText = query

                    Using dr As OracleDataReader = cmd.ExecuteReader()
                        If Not dr Is Nothing Then
                            Dim Cognome As Integer = dr.GetOrdinal("Cognome")
                            Dim Nome As Integer = dr.GetOrdinal("Nome")
                            Dim Cf As Integer = dr.GetOrdinal("Cf")
                            Dim DataNascita As Integer = dr.GetOrdinal("DataNascita")
                            Dim Sesso As Integer = dr.GetOrdinal("Sesso")
                            Dim ComuneNascita As Integer = dr.GetOrdinal("ComuneNascita")
                            Dim ComuneResidenza As Integer = dr.GetOrdinal("ComuneResidenza")
                            Dim IndirizzoResidenza As Integer = dr.GetOrdinal("IndirizzoResidenza")
                            Dim ComuneDomicilio As Integer = dr.GetOrdinal("ComuneDomicilio")
                            Dim IndirizzoDomicilio As Integer = dr.GetOrdinal("IndirizzoDomicilio")
                            Dim TelefonoUno As Integer = dr.GetOrdinal("TelefonoUno")
                            Dim TelefonoDue As Integer = dr.GetOrdinal("TelefonoDue")
                            Dim TelefonoTre As Integer = dr.GetOrdinal("TelefonoTre")
                            Dim Email As Integer = dr.GetOrdinal("Email")
                            Dim NomeMedico As Integer = dr.GetOrdinal("NomeMedico")
                            Dim CognomeMedico As Integer = dr.GetOrdinal("CognomeMedico")
                            Dim CfMedico As Integer = dr.GetOrdinal("CfMedico")

                            If dr.Read Then
                                result = New DatiPaziente
                                result.Cognome = dr.GetStringOrDefault(Cognome)
                                result.Nome = dr.GetStringOrDefault(Nome)
                                result.Cf = dr.GetStringOrDefault(Cf)
                                result.DataNascita = dr.GetNullableDateTimeOrDefault(DataNascita)
                                result.Sesso = dr.GetStringOrDefault(Sesso)
                                result.ComuneNascita = dr.GetStringOrDefault(ComuneNascita)
                                result.ComuneResidenza = dr.GetStringOrDefault(ComuneResidenza)
                                result.IndirizzoResidenza = dr.GetStringOrDefault(IndirizzoResidenza)
                                result.ComuneDomicilio = dr.GetStringOrDefault(ComuneDomicilio)
                                result.IndirizzoDomicilio = dr.GetStringOrDefault(IndirizzoDomicilio)
                                result.TelefonoUno = dr.GetStringOrDefault(TelefonoUno)
                                result.TelefonoDue = dr.GetStringOrDefault(TelefonoDue)
                                result.TelefonoTre = dr.GetStringOrDefault(TelefonoTre)
                                result.Email = dr.GetStringOrDefault(Email)
                                result.NomeMedico = dr.GetStringOrDefault(NomeMedico)
                                result.CognomeMedico = dr.GetStringOrDefault(CognomeMedico)
                                result.CfMedico = dr.GetStringOrDefault(CfMedico)
                            End If
                        End If
                    End Using
                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try
            End Using
            Return result
        End Function

        Public Function GetEpisodiByPaziente(codicePaziente As Long) As List(Of EpisodioPaziente) Implements IRilevazioniCovid19Provider.GetEpisodiByPaziente
            Dim result As List(Of EpisodioPaziente) = New List(Of EpisodioPaziente)
            Using cmd As OracleCommand = New OracleCommand
                cmd.Connection = Connection
                Dim ownConnection As Boolean = False
                Try
                    ownConnection = ConditionalOpenConnection(cmd)
                    Dim query As String = "select 
                                                            PAZ_NOME as Nome,
                                                            PAZ_COGNOME as Cognome,
                                                            PAZ_DATA_NASCITA as DataNascita,
                                                            PAZ_SESSO as Sesso,
                                                            PAZ_CODICE_FISCALE as CodiceFiscale,
                                                            PAZ_USL_CODICE_RESIDENZA as CodiceUslResidenza,
                                                            PES_ID IdEpisodio,
                                                            PES_PAZ_CODICE CodicePaziente,
                                                            PES_TIPO_CASO TipoCaso,
                                                            CTIPOCASO.COD_DESCRIZIONE DescrizioneTipoCaso,
                                                            PES_ATC_CODICE CodiceTipoContatto,
                                                            ATC_DESCRIZIONE DescrizioneTipoContatto,
                                                            PES_FLAG_OPERATORE_SANITARIO IsOperatoreSanitario,
                                                            PES_PATOLOGIE_CRONICHE HasPatologiaCroniche,
                                                            PES_DATA_SEGNALAZIONE DataSegnalazione,
                                                            PES_ASINTOMATICO IsAsintomatico,
                                                            PES_NOTE Note,
                                                            PES_OPE_CODICE_RACCOLTA CodiceRaccoltaOperatore,
                                                            PES_OPE_TELEFONO TelefonoOperatore,
                                                            OPE_EMAIL EmailOperatore,
                                                            PES_USL_CODICE_RACCOLTA CodiceRaccoltaUsl,
                                                            PES_ASE_CODICE CodiceSegnalatore,
                                                            ASE_DESCRIZIONE DescrizioneSegnalatore,
                                                            PES_SEP_CODICE CodiceStato,
                                                            SEP_DESCRIZIONE DescrizioneStato,
                                                            SEP_ATTIVO Attivo,
                                                            PES_TIPO_OPERATORE_SANITARIO TipoOperatoreSanitario,
                                                            PES_COMUNE_ESPOSIZIONE EsposizioneComune,
                                                            PES_DATA_INIZIO_ISOLAMENTO DataInizioIsolamento,
                                                            PES_DATA_ULTIMO_CONTATTO DataUltimoContatto,
                                                            PES_DATA_FINE_ISOLAMENTO DataFineIsolamento,
                                                            PES_INDIRIZZO_ISOLAMENTO IndirizzoIsolamento,
                                                            COM_DESCRIZIONE DescrizioneComuneIsolamento,
                                                            PES_TELEFONO TelefonoIsolamento,
                                                            PES_EMAIL EmailIsolamento,
                                                            PES_DATA_CHIUSURA DataChiusura,
                                                            PES_DATA_INIZIO_SINTOMI DataInizioSintomi,
                                                            PES_UTE_ID_INSERIMENTO CodiceUtenteInserimento,
                                                            UTE_DESCRIZIONE DescrizioneUtenteInserimento,
                                                            PES_USL_CODICE_INSERIMENTO UslInserimento,
                                                            PES_DATA_INSERIMENTO DataInserimento,
                                                            PES_UTE_ID_ULTIMA_MODIFCA UtenteUltimaModifica,
                                                            PES_DATA_ULTIMA_MODIFICA DataUltimaModifica,
                                                            J.PED_RISPOSTA_TELEFONO as UltimaRisposta,
                                                            J.PED_UTE_ID_INSERIMENTO as UltimoUtenteInserimentoDiaria,
                                                            '' as NomeUteUltimaDiaria,
                                                            J.PED_DATA_RILEVAZIONE as DataUltimaDiaria,
                                                            J.PED_ID as IdUltimaDiaria,
                                                            J.PDS_ID as IdSintomoUltimaDiaria,
                                                            J.ASI_DESCRIZIONE as DescrizioneSintomoUltimaDiaria,
                                                            T.PET_DATA_TAMPONE as DataUltimoTampone,
                                                            T.PET_ESITO as EsitoUltimoTampone,
                                                            MED_COGNOME CognomeMedicoBase,
                                                            MED_NOME NomeMedicoBase,
                                                            PAZ_CONFERMA_INFORMATIVA_COVID as ConfermaInformativaCovid,
                                                            NVL(PES_TELEFONO, NVL(PAZ_TELEFONO_1, NVL(PAZ_TELEFONO_2, PAZ_TELEFONO_3))) TelefonoIsolamento
                                                from T_PAZ_EPISODI
                                                                join T_PAZ_PAZIENTI on PES_PAZ_CODICE = PAZ_CODICE
                                                                     left outer join T_ANA_MEDICI on PAZ_MED_CODICE_BASE = MED_CODICE
                                                                left outer join T_ANA_SEGNALATORE on PES_ASE_CODICE = ASE_CODICE
                                                                left outer join T_ANA_UTENTI on PES_UTE_ID_INSERIMENTO = UTE_ID
                                                                left outer join T_ANA_STATI_EPISODIO on PES_SEP_CODICE = SEP_CODICE
                                                                left outer join T_ANA_TIPO_CONTATTO on PES_ATC_CODICE = ATC_CODICE
                                                                left outer join T_ANA_COMUNI on NVL(PES_COM_CODICE_ISOLAMENTO, NVL(PAZ_COM_CODICE_DOMICILIO, PAZ_COM_CODICE_RESIDENZA)) = COM_CODICE
                                                                left outer join T_ANA_CODIFICHE CTIPOCASO on CTIPOCASO.COD_CAMPO = 'PES_TIPO_CASO' and CTIPOCASO.COD_CODICE = PES_TIPO_CASO
                                                                left outer join (
                                                                                    select 
                                                                                        O.PED_PES_ID, 
                                                                                        o.PED_ID, 
                                                                                        o.PED_RISPOSTA_TELEFONO, 
                                                                                        o.PED_UTE_ID_INSERIMENTO, 
                                                                                        o.PED_DATA_RILEVAZIONE, 
                                                                                        PDS_ID, 
                                                                                        ASI_CODICE,
                                                                                        ASI_DESCRIZIONE
                                                                                            from T_PAZ_EPISODI_DIARIA o
                                                                                            left outer join T_PAZ_EPISODI_DIARIA_SINTOMI on PED_ID = PDS_PED_ID
                                                                                                            left outer join t_ana_sintomi on PDS_ASI_CODICE = ASI_CODICE
                                                                                                                where o.PED_DATA_RILEVAZIONE = (
                                                                                                                                                select MAX(PED_DATA_RILEVAZIONE) 
                                                                                                                                                    from T_PAZ_EPISODI_DIARIA k 
                                                                                                                                                        where k.PED_PES_ID = o.PED_PES_ID
                                                                                                                                                )
                                                                                ) j on PES_ID = j.PED_PES_ID
                                                                left outer join (
                                                                                    select T.PET_PES_ID, t.PET_ESITO, t.PET_DATA_TAMPONE
                                                                                        from T_PAZ_EPISODI_TAMPONI t
                                                                                            where t.PET_DATA_TAMPONE = (select MAX(PET_DATA_TAMPONE) from T_PAZ_EPISODI_TAMPONI p where p.PET_PES_ID = t.PET_PES_ID)
                                                                                ) t on PES_ID = t.PET_PES_ID
                                                        where PES_PAZ_CODICE = :codicePaziente"
                    cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)
                    cmd.CommandText = query
                    result = LeggiEpisodi(cmd)
                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try
            End Using
            Return result
        End Function

        Public Function RicercaEpisodiWorkList(ricerca As RicercaEpisodi) As List(Of InfoEpisodioCovid) Implements IRilevazioniCovid19Provider.RicercaEpisodiWorkList
            Return DoCommand(Function(cmd)
                                 Dim comando As String = "SELECT 
	                                                tag.TAG_ID AS CodiceTag,
	                                                tag.TAG_DESCRIZIONE AS DescrizioneTag,
	                                                sintomi2.ASI_CODICE AS CodiceSintomo,
	                                                SINTOMI2.ASI_DESCRIZIONE AS DescrizioneSintomo,
	                                                paginazione.*,
	                                                eppsin.pds_id AS IdSintomoUltimaDiaria,
	                                                SINTOMI.ASI_DESCRIZIONE AS DescrizioneSintomoUltimaDiaria
	                                                FROM (
	                                                SELECT tab.*, ROWNUM AS Numero FROM (
		                                                SELECT 
			                                                epp.PES_ID AS IdEpisodio,
			                                                epp.PES_PAZ_CODICE as CodicePaziente,
			                                                epp.PES_DATA_DECESSO_COVID AS DataDecessoCovid,
			                                                epp.PES_DATA_GUARITO_CLINICAMENTE AS DataGuaritoClinicamento,
			                                                epp.PES_GUARITO_CLINICAMENTE AS GuaritoClinicamente,
                                                            epp.PES_VARIANTE_VIRUS as CodiceVariante,
                                                            variante.tav_descrizione as DescrizioneVariante,
			                                                lastD.PED_ID AS IdUltimaDiaria,
			                                                lastD.PED_DATA_RILEVAZIONE AS DataUltimaDiaria,
			                                                lastD.PED_UTE_ID_INSERIMENTO AS UltimoUtenteInserimentoDiaria,
			                                                lastD.ULTIMARISPOSTA AS UltimaRisposta,
			                                                utD.UTE_DESCRIZIONE AS NomeUteUltimaDiaria,
			                                                epp.PES_TIPO_CASO AS TipoCaso,
			                                                epp.PES_FLAG_OPERATORE_SANITARIO AS IsOperatoreSanitario,
			                                                epp.PES_PATOLOGIE_CRONICHE AS HasPatologiaCroniche,
			                                                epp.PES_DATA_SEGNALAZIONE AS DataSegnalazione,
			                                                nvl(epp.PES_ASINTOMATICO, 'S') AS IsAsintomatico,
			                                                epp.PES_NOTE AS Note,
			                                                epp.PES_OPE_CODICE_RACCOLTA AS CodiceRaccoltaOperatore,
			                                                epp.PES_USL_CODICE_RACCOLTA AS CodiceRaccoltaUsl,
			                                                epp.PES_ASE_CODICE AS CodiceSegnalatore,
			                                                epp.PES_SEP_CODICE AS CodiceStato,
			                                                epp.PES_TIPO_OPERATORE_SANITARIO AS TipoOperatoreSanitario,
			                                                epp.PES_DATA_FINE_ISOLAMENTO AS DataFineIsolamento,
			                                                epp.PES_COMUNE_ESPOSIZIONE AS EsposizioneComune,
			                                                epp.PES_DATA_INIZIO_ISOLAMENTO AS DataInizioIsolamento,
			                                                epp.PES_DATA_ULTIMO_CONTATTO AS DataUltimoContatto,
		                                                    epp.PES_AUTO_POS AS AutoPositivo,
			                                                epp.PES_OPE_TELEFONO AS TelefonoOperatore,
			                                                epp.PES_ATC_CODICE AS CodiceTipoContatto,
			                                                epp.PES_USL_CODICE_INSERIMENTO AS UslInserimento,
			                                                epp.PES_DATA_INSERIMENTO AS DataInserimento,
			                                                epp.PES_UTE_ID_ULTIMA_MODIFCA AS UtenteUltimaModifica,
			                                                epp.PES_DATA_ULTIMA_MODIFICA AS DataUltimaModifica,
			                                                epp.PES_DATA_CHIUSURA AS DataChiusura,
			                                                epp.PES_EMAIL AS EmailIsolamento,
			                                                epp.PES_DATA_INIZIO_SINTOMI AS DataInizioSintomi,
			                                                epp.PES_UTE_ID_INSERIMENTO AS CodiceUtenteInserimento,
			                                                epp.PES_COM_CODICE_ISOLAMENTO AS ComuneCodiceIsolamento,
			                                                paz.PAZ_NOME AS Nome,
			                                                paz.PAZ_COGNOME AS Cognome,
			                                                paz.PAZ_DATA_NASCITA AS DataNascita,
			                                                paz.PAZ_SESSO AS Sesso,
			                                                paz.PAZ_CODICE_FISCALE AS CodiceFiscale,
			                                                paz.PAZ_USL_CODICE_RESIDENZA AS CodiceUslResidenza,
			                                                paz.PAZ_CONFERMA_INFORMATIVA_COVID AS ConfermaInformativaCovid,
			                                                med.MED_NOME AS NomeMedicoBase,
			                                                med.MED_COGNOME AS CognomeMedicoBase,
			                                                sta.SEP_DESCRIZIONE AS DescrizioneStato,
			                                                sta.SEP_ATTIVO AS Attivo,
			                                                con.ATC_DESCRIZIONE AS DescrizioneTipoContatto,
			                                                seg.ASE_DESCRIZIONE AS DescrizioneSegnalatore,
			                                                com.COM_DESCRIZIONE AS DescrizioneComuneIsolamento,
			                                                OPE_EMAIL AS EmailOperatore,
			                                                NVL(epp.PES_INDIRIZZO_ISOLAMENTO, NVL(paz.PAZ_INDIRIZZO_DOMICILIO, paz.PAZ_INDIRIZZO_RESIDENZA)) AS IndirizzoIsolamento,
			                                                NVL(epp.PES_TELEFONO, NVL(paz.PAZ_TELEFONO_1, NVL(paz.PAZ_TELEFONO_2, paz.PAZ_TELEFONO_3))) AS TelefonoIsolamento,
			                                                ute.UTE_DESCRIZIONE AS DescrizioneUtenteInserimento,
			                                                NVL(epp.PES_COM_CODICE_ISOLAMENTO, NVL(paz.PAZ_COM_CODICE_DOMICILIO, paz.PAZ_COM_CODICE_RESIDENZA)) AS CodiceComuneIsolamento,
			                                                CTIPOCASO.COD_DESCRIZIONE AS DescrizioneTipoCaso,
			                                                lastT.PET_DATA_TAMPONE AS DataUltimoTampone,
			                                                epp.PES_OTP AS otp,
			                                                lastT.PET_ESITO AS EsitoUltimoTampone
		                                                FROM T_PAZ_EPISODI epp
			                                                JOIN T_PAZ_PAZIENTI paz ON epp.PES_PAZ_CODICE = paz.PAZ_CODICE
			                                                LEFT OUTER JOIN T_ANA_MEDICI med ON paz.PAZ_MED_CODICE_BASE = med.MED_CODICE
			                                                LEFT OUTER JOIN T_ANA_STATI_EPISODIO sta ON epp.PES_SEP_CODICE = sta.SEP_CODICE 
			                                                LEFT OUTER JOIN T_ANA_TIPO_CONTATTO con ON epp.PES_ATC_CODICE = con.ATC_CODICE 
			                                                LEFT OUTER JOIN T_ANA_SEGNALATORE seg ON epp.PES_ASE_CODICE = seg.ASE_CODICE 
			                                                LEFT OUTER JOIN T_ANA_COMUNI com on NVL(epp.PES_COM_CODICE_ISOLAMENTO, NVL(paz.PAZ_COM_CODICE_DOMICILIO, paz.PAZ_COM_CODICE_RESIDENZA)) = com.COM_CODICE
			                                                LEFT OUTER JOIN T_ANA_UTENTI ute on epp.PES_UTE_ID_INSERIMENTO = ute.UTE_ID
			                                                LEFT OUTER JOIN T_ANA_CODIFICHE CTIPOCASO on CTIPOCASO.COD_CAMPO = 'PES_TIPO_CASO' and CTIPOCASO.COD_CODICE = epp.PES_TIPO_CASO
			                                                LEFT OUTER JOIN (
				                                                SELECT distinct
					                                                d.PED_PES_ID,
					                                                FIRST_VALUE(d.PED_ID) OVER (PARTITION BY d.PED_PES_ID ORDER BY d.PED_DATA_RILEVAZIONE DESC, d.PED_ID DESC) AS PED_ID, 
					                                                FIRST_VALUE(d.PED_UTE_ID_INSERIMENTO) OVER (PARTITION BY d.PED_PES_ID ORDER BY d.PED_DATA_RILEVAZIONE DESC, d.PED_ID DESC) AS PED_UTE_ID_INSERIMENTO,
					                                                FIRST_VALUE(d.PED_DATA_RILEVAZIONE) OVER (PARTITION BY d.PED_PES_ID ORDER BY d.PED_DATA_RILEVAZIONE DESC, d.PED_ID DESC) AS PED_DATA_RILEVAZIONE,
					                                                FIRST_VALUE(d.PED_RISPOSTA_TELEFONO) OVER (PARTITION BY d.PED_PES_ID ORDER BY d.PED_DATA_RILEVAZIONE DESC, d.PED_ID DESC) AS UltimaRisposta
					                                                FROM T_PAZ_EPISODI_DIARIA d
			                                                ) lastD ON epp.PES_ID = lastD.PED_PES_ID
			                                                LEFT OUTER JOIN (
				                                                SELECT DISTINCT
					                                                t.PET_PES_ID,
					                                                FIRST_VALUE(t.PET_ESITO) OVER (PARTITION BY t.PET_PES_ID ORDER BY t.PET_DATA_TAMPONE DESC, t.PET_ID DESC) AS PET_ESITO,
					                                                FIRST_VALUE(t.PET_DATA_TAMPONE) OVER (PARTITION BY t.PET_PES_ID ORDER BY t.PET_DATA_TAMPONE DESC, t.PET_ID DESC) AS PET_DATA_TAMPONE 
					                                                FROM T_PAZ_EPISODI_TAMPONI t 
			                                                ) lastT ON epp.PES_ID = lastT.PET_PES_ID
			                                                LEFT OUTER JOIN t_ana_utenti utD ON lastD.PED_UTE_ID_INSERIMENTO = utD.UTE_ID 
                                                            left outer join T_ANA_TIPI_VARIANTI variante on epp.PES_VARIANTE_VIRUS = variante.tav_codice
                                                            order by DATAULTIMADIARIA nulls first, cognome, NOME
	                                                ) tab {0}
                                                ) paginazione
                                                LEFT JOIN T_PAZ_EPISODI_DIARIA_SINTOMI eppSin ON eppsin.PDS_PED_ID = IdUltimaDiaria
                                                LEFT JOIN T_ANA_SINTOMI sintomi ON eppSin.PDS_ASI_CODICE = sintomi.ASI_CODICE
                                                LEFT JOIN T_ANA_LINK_EPISODI_TAG lTag ON ltag.LET_PES_ID = paginazione.idEpisodio
                                                LEFT JOIN T_ANA_TAG tag ON ltag.LET_TAG_ID = tag.TAG_ID 
                                                LEFT JOIN T_PAZ_EPISODI_DIARIA diarie ON idEpisodio = diarie.PED_PES_ID 
                                                LEFT JOIN T_PAZ_EPISODI_DIARIA_SINTOMI lsintomi ON diarie.PED_ID = LSINTOMI.PDS_PED_ID
                                                LEFT JOIN T_ANA_SINTOMI sintomi2 ON sintomi2.ASI_CODICE = LSINTOMI.PDS_ASI_CODICE"

                                 If ricerca.Skip.HasValue AndAlso ricerca.Take.HasValue Then
                                     comando += " WHERE numero > ?skip and numero <= ?take"
                                     cmd.AddParameter("skip", ricerca.Skip)
                                     cmd.AddParameter("take", ricerca.Take.Value + ricerca.Skip)
                                 End If

                                 Dim condizione As New List(Of String)

                                 If ricerca.StatoRilevazione.HasValue AndAlso ricerca.StatoRilevazione > 0 Then
                                     condizione.Add("CodiceStato = :codStato")
                                     cmd.AddParameter("codStato", ricerca.StatoRilevazione.Value)
                                 End If
                                 If Not String.IsNullOrWhiteSpace(ricerca.Tipologia) Then
                                     condizione.Add("TipoCaso = :codTipo")
                                     cmd.AddParameter("codTipo", ricerca.Tipologia)
                                 End If
                                 If ricerca.DataUltimaDiaria.HasValue Then
                                     condizione.Add("(DATAULTIMADIARIA is null or DATAULTIMADIARIA < :ultimaDiaria)")
                                     cmd.AddParameter("ultimaDiaria", ricerca.DataUltimaDiaria.Value.Date)
                                 End If
                                 If ricerca.DataFineIsolamentoPresunto.HasValue Then
                                     condizione.Add("DATAFINEISOLAMENTO >= :fineIsolamentoA and DATAFINEISOLAMENTO < :fineIsolamentoB and DATAFINEISOLAMENTO is not null")
                                     cmd.AddParameter("fineIsolamentoA", ricerca.DataFineIsolamentoPresunto.Value.Date)
                                     cmd.AddParameter("fineIsolamentoB", ricerca.DataFineIsolamentoPresunto.Value.Date.AddDays(1))
                                 End If
                                 If Not String.IsNullOrWhiteSpace(ricerca.CodiceUslResidenza) Then
                                     condizione.Add("CodiceUslResidenza = :uslResidenza")
                                     cmd.AddParameter("uslResidenza", ricerca.CodiceUslResidenza)
                                 End If
                                 If Not String.IsNullOrWhiteSpace(ricerca.CodiceUslInCarico) Then
                                     condizione.Add("CodiceRaccoltaUsl = :uslCarico")
                                     cmd.AddParameter("uslCarico", ricerca.CodiceUslInCarico)
                                 End If
                                 If ricerca.SoloSintomatici Then
                                     condizione.Add("IsAsintomatico = 'N'")
                                 End If
                                 If ricerca.AutoPositivi Then
                                     condizione.Add("AutoPositivo = 'S'")
                                 End If
                                 If ricerca.CodicePazienteIndice.HasValue Then
                                     condizione.Add("(
                                            EXISTS (
                                                    select 1 from T_PAZ_EPISODI_CONTATTI
                                                        inner join T_PAZ_EPISODI on PEC_PES_ID = PES_ID
                                                            where TAB.CODICEPAZIENTE = PEC_PAZ_CODICE and PES_PAZ_CODICE = :codicePazienteIndice
                                                    )
                                            OR TAB.CODICEPAZIENTE = :codicePazienteIndice
                                        )")
                                     cmd.AddParameter("codicePazienteIndice", ricerca.CodicePazienteIndice.Value)
                                 End If

                                 If (ricerca.TamponiDaVisionare) Then
                                     condizione.Add("EXISTS (
                                                select 1 from T_PAZ_EPISODI_TAMPONI t
                                                    WHERE t.PET_FLG_DA_VISIONARE = 'S'
                                                    and TAB.IDEPISODIO  = t.PET_PES_ID
                                                )")
                                 End If

                                 If (ricerca.DataRefertoTampone.HasValue) Then
                                     condizione.Add("EXISTS (
                                                select 1 from T_PAZ_EPISODI_TAMPONI t
                                                    WHERE t.PET_DATA_REFERTO is not null
                                                    AND t.PET_DATA_REFERTO = :dataReferto
                                                    AND TAB.IDEPISODIO  = t.PET_PES_ID
                                                )")
                                     cmd.AddParameter("dataReferto", ricerca.DataRefertoTampone.Value.Date)
                                 End If

                                 If (ricerca.MancataRisposta) Then
                                     condizione.Add("UltimaRisposta = 'N'")
                                 End If

                                 If (ricerca.UltimaDiariaFromApp.HasValue) Then
                                     condizione.Add("UltimoUtenteInserimentoDiaria = :utenteInserimento")
                                     cmd.AddParameter("utenteInserimento", ricerca.UltimaDiariaFromApp.Value)
                                 End If

                                 If (Not ricerca.StatoAttivazioneApp = AttivazioneApp.Tutte) Then
                                     Select Case ricerca.StatoAttivazioneApp
                                         Case AttivazioneApp.Attive
                                             condizione.Add("ConfermaInformativaCovid = 'S'")
                                         Case AttivazioneApp.NonAttive
                                             condizione.Add("(ConfermaInformativaCovid = 'N' or ConfermaInformativaCovid is null)")
                                     End Select
                                 End If

                                 If Not String.IsNullOrWhiteSpace(ricerca.CodiceComuneIsolamento) Then
                                     condizione.Add("CodiceComuneIsolamento = :codiceComuneIsolamento")
                                     cmd.AddParameter("codiceComuneIsolamento", ricerca.CodiceComuneIsolamento)
                                 End If

                                 If ricerca.Tags.Any() Then
                                     Dim exists As New StringBuilder("exists (select 1 from T_ANA_LINK_EPISODI_TAG where TAB.IDEPISODIO = LET_PES_ID and (")
                                     Dim nomeParametro As String
                                     For i As Integer = 0 To ricerca.Tags.Length - 2
                                         nomeParametro = String.Format("T_{0}", i)
                                         exists.Append(String.Format("LET_TAG_ID = :{0} or ", nomeParametro))
                                         cmd.AddParameter(nomeParametro, ricerca.Tags(i))
                                     Next
                                     nomeParametro = String.Format("T_{0}", ricerca.Tags.Length - 1)
                                     exists.Append(String.Format("LET_TAG_ID = :{0}", nomeParametro))
                                     cmd.AddParameter(nomeParametro, ricerca.Tags.Last())
                                     exists.Append("))")
                                     condizione.Add(exists.ToString())
                                 End If

                                 If Not String.IsNullOrWhiteSpace(ricerca.Nome) Then
                                     condizione.Add("lower(NOME) like ?nome")
                                     cmd.AddParameter("nome", String.Format("{0}%", ricerca.Nome.Trim().ToLower()))
                                 End If

                                 If Not String.IsNullOrWhiteSpace(ricerca.Cognome) Then
                                     condizione.Add("lower(COGNOME) like ?cognome")
                                     cmd.AddParameter("cognome", String.Format("{0}%", ricerca.Cognome.Trim().ToLower()))
                                 End If

                                 If ricerca.DataNascita.HasValue Then
                                     condizione.Add("DataNascita = ?data")
                                     cmd.AddParameter("data", ricerca.DataNascita.Value.Date)
                                 End If

                                 If ricerca.EscludiPazientiRSA Then
                                     condizione.Add("not exists (select 1 from T_PAZ_RSA where PRS_PAZ_CODICE = CodicePaziente)")
                                 End If

                                 If ricerca.CodiceVariante.HasValue AndAlso ricerca.CodiceVariante.Value > 0 Then
                                     condizione.Add("CodiceVariante = ?variante")
                                     cmd.AddParameter("variante", ricerca.CodiceVariante)
                                 End If

                                 If condizione.Any() Then
                                     cmd.CommandText = String.Format(comando, String.Format(" WHERE {0}", String.Join(" AND ", condizione)))
                                 Else
                                     cmd.CommandText = String.Format(comando, " ")
                                 End If

                                 Dim flat As List(Of FlatEpisodioPaziente) = cmd.Fill(Of FlatEpisodioPaziente)

                                 Return flat.GroupBy(Function(f) f.IdEpisodio).Select(Function(f)
                                                                                          Dim e As FlatEpisodioPaziente = f.First()
                                                                                          Dim ritorno As New InfoEpisodioCovid With {
                                                                                        .Attivo = e.Attivo,
                                                                                        .CodiceVariante = e.CodiceVariante,
                                                                                        .DescrizioneVariante = e.DescrizioneVariante,
                                                                                        .CodiceComune = Nothing,
                                                                                        .CodicePaziente = e.CodicePaziente,
                                                                                        .CodicePazienteOld = 0,
                                                                                        .CodiceRaccoltaOperatore = e.CodiceRaccoltaOperatore,
                                                                                        .CodiceRaccoltaUsl = e.CodiceRaccoltaUsl,
                                                                                        .CodiceSegnalatore = e.CodiceSegnalatore,
                                                                                        .CodiceStato = e.CodiceStato,
                                                                                        .CodiceTipoCaso = e.CodiceTipoCaso,
                                                                                        .CodiceTipoContatto = e.CodiceTipoContatto,
                                                                                        .CodiceTipoOperatoreSanitario = e.CodiceTipoOperatoreSanitario,
                                                                                        .CodiceUtenteInserimento = e.CodiceUtenteInserimento,
                                                                                        .ComuneCodiceIsolamento = e.ComuneCodiceIsolamento,
                                                                                        .DataChiusura = e.DataChiusura,
                                                                                        .DataDecessoCovid = e.DataDecessoCovid,
                                                                                        .DataEliminazione = Nothing,
                                                                                        .DataFineIsolamento = e.DataFineIsolamento,
                                                                                        .DataGuaritoClinicamente = e.DataGuaritoClinicamento,
                                                                                        .DataInizioIsolamento = e.DataInizioIsolamento,
                                                                                        .DataInizioSintomi = e.DataInizioSintomi,
                                                                                        .DataInserimento = e.DataInserimento,
                                                                                        .DataSegnalazione = e.DataSegnalazione,
                                                                                        .DataUltimaDiaria = e.DataUltimaDiaria,
                                                                                        .DataUltimaModifica = e.DataUltimaModifica,
                                                                                        .DataUltimoContatto = e.DataUltimoContatto,
                                                                                        .DataUltimoTampone = e.DataUltimoTampone,
                                                                                        .DescrizioneComuneIsolamento = e.DescrizioneComuneIsolamento,
                                                                                        .DescrizioneSegnalatore = e.DescrizioneSegnalatore,
                                                                                        .DescrizioneSintomi = e.DescrizioneSegnalatore,
                                                                                        .DescrizioneStato = e.DescrizioneStato,
                                                                                        .DescrizioneTipoCaso = e.DescrizioneTipoCaso,
                                                                                        .DescrizioneTipoContatto = e.DescrizioneTipoContatto,
                                                                                        .DescrizioneTipoOperatoreSanitario = e.DescrizioneTipoContatto,
                                                                                        .DescrizioneUtenteInserimento = e.DescrizioneUtenteInserimento,
                                                                                        .EmailIsolamento = e.EmailIsolamento,
                                                                                        .EmailOperatore = e.EmailOperatore,
                                                                                        .EmailRilevazione = "",
                                                                                        .EsitoUltimoTampone = e.EsitoUltimoTampone,
                                                                                        .EsposizioneComune = e.EsposizioneComune,
                                                                                        .GuaritoClinicamente = e.GuaritoClinicamente,
                                                                                        .HasPatologiaCroniche = e.HasPatologiaCroniche,
                                                                                        .IdEpisodio = e.IdEpisodio,
                                                                                        .IndirizzoIsolamento = e.IndirizzoIsolamento,
                                                                                        .InternoRegione = e.IndirizzoIsolamento,
                                                                                        .IsAsintomatico = e.IsAsintomatico,
                                                                                        .IsOperatoreSanitario = e.IsOperatoreSanitario,
                                                                                        .NominativoUtenteInserimentoUltimaDiaria = e.NominativoUtenteInserimentoUltimaDiaria,
                                                                                        .Note = e.Note,
                                                                                        .otp = e.Otp,
                                                                                        .TelefonoIsolamento = e.TelefonoIsolamento,
                                                                                        .SpsId = 0,
                                                                                        .TelefonoOperatore = e.TelefonoOperatore,
                                                                                        .TelefonoRilevazione = e.TelefonoIsolamento,
                                                                                        .TpsId = 0,
                                                                                        .UslEliminazione = "",
                                                                                        .UslInserimento = e.UslInserimento,
                                                                                        .UtenteEliminazione = Nothing,
                                                                                        .UtenteInserimentoDiaria = e.CodiceUtenteDiaria,
                                                                                        .UtenteUltimaModifica = e.UtenteUltimaModifica
                                                                                   }
                                                                                          ritorno.Paziente = New EpisodioDatiPaziente With {
                                                                                        .ComuneResidenza = "",
                                                                                        .HasConfermaInformativaCovid = False,
                                                                                        .HasCredenzialiApp = False,
                                                                                        .Cf = e.Cf,
                                                                                        .Cognome = e.Cognome,
                                                                                        .CognomeMedicoBase = e.CognomeMedicoBase,
                                                                                        .DataNascita = e.DataNascita,
                                                                                        .Nome = e.Nome,
                                                                                        .NomeMedicoBase = e.NomeMedicoBase,
                                                                                        .Sesso = e.Sesso,
                                                                                        .TelefonoDue = "",
                                                                                        .TelefonoTre = "",
                                                                                        .TelefonoUno = ""
                                                                                   }
                                                                                          ritorno.Tags = f.Where(Function(x) x.CodiceTag.HasValue).Select(Function(x)
                                                                                                                                                              Return New Tag With {.Id = x.CodiceTag.Value, .Descrizione = x.DescrizioneTag}
                                                                                                                                                          End Function).ToList()

                                                                                          Dim sintomi As IEnumerable(Of KeyValuePair(Of Long, String)) = f.Where(Function(x) x.CodiceSintomo.HasValue).Select(Function(x)
                                                                                                                                                                                                                  Return New KeyValuePair(Of Long, String)(x.CodiceSintomo.Value, x.DescrizioneSintomo)
                                                                                                                                                                                                              End Function)

                                                                                          ritorno.Sintomi = New Dictionary(Of Long, String)()
                                                                                          For Each s As KeyValuePair(Of Long, String) In sintomi
                                                                                              If Not ritorno.Sintomi.ContainsKey(s.Key) Then
                                                                                                  ritorno.Sintomi.Add(s.Key, s.Value)
                                                                                              End If
                                                                                          Next

                                                                                          Return ritorno
                                                                                      End Function).ToList()


                             End Function)
        End Function

        Public Function GetIdEpisodiByPaziente(codicePaziente As Long) As List(Of Long) Implements IRilevazioniCovid19Provider.GetIdEpisodiByPaziente
            Dim list As List(Of Long) = New List(Of Long)
            Dim ownConnection As Boolean = False
            Dim query As String = "SELECT PES_ID " +
                                    "FROM T_PAZ_EPISODI " +
                                    "WHERE PES_PAZ_CODICE = :paz_codice AND (PES_SEP_CODICE IN (SELECT SEP_CODICE FROM T_ANA_STATI_EPISODIO WHERE SEP_ATTIVO ='S') OR PES_SEP_CODICE IS NULL) "
            'Dim query As String = "SELECT PES_ID " +
            '                        "FROM T_PAZ_EPISODI tpe " +
            '                        "INNER JOIN T_ANA_STATI_EPISODIO ON PES_SEP_CODICE = SEP_CODICE AND SEP_ATTIVO ='S' " +
            '                        "WHERE PES_PAZ_CODICE = :paz_codice "
            Try
                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    cmd.Parameters.AddWithValue("paz_codice", codicePaziente)
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()

                        Dim pes_id As Integer = _context.GetOrdinal("PES_ID")

                        While _context.Read()
                            Dim id As Long = _context.GetInt64OrDefault(pes_id)
                            list.Add(id)
                        End While
                    End Using
                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return list
        End Function
        Public Function RicercaEpisodi_Old(ricerca As RicercaEpisodi) As List(Of EpisodioPaziente) Implements IRilevazioniCovid19Provider.RicercaEpisodi
            Using cmd As OracleCommand = New OracleCommand
                cmd.Connection = Connection
                Dim ownConnection As Boolean = False
                Try
                    'il campo "UltimaDiaria" prende la data dell'ultima diaria di ogni episodio
                    ownConnection = ConditionalOpenConnection(cmd)
                    Dim query As String = "select 
	                                            J.PED_ID as IdUltimaDiaria,
	                                            COMP.*,
	                                            J.PDS_ID as IdSintomoUltimaDiaria,
	                                            J.ASI_DESCRIZIONE as DescrizioneSintomoUltimaDiaria,
                                                J.UTE_DESCRIZIONE as NomeUteUltimaDiaria,
	                                            T.PET_DATA_TAMPONE as DataUltimoTampone,
	                                            T.PET_ESITO as EsitoUltimoTampone
		                                            from 
		                                            (
			                                            select ROWNUM as numero, tab.* from 
			                                            (
				                                            select 
				                                            lastD.PED_UTE_ID_INSERIMENTO as UltimoUtenteInserimentoDiaria,
				                                            lastD.PED_DATA_RILEVAZIONE as DataUltimaDiaria,
				                                            lastD.ULTIMARISPOSTA as UltimaRisposta,
				                                            PAZ_NOME as Nome,
				                                            PAZ_COGNOME as Cognome,
				                                            PAZ_DATA_NASCITA as DataNascita,
				                                            PAZ_SESSO as Sesso,
				                                            PAZ_CODICE_FISCALE as CodiceFiscale,
				                                            PAZ_USL_CODICE_RESIDENZA as CodiceUslResidenza,
				                                            PES_ID IdEpisodio,
				                                            PES_PAZ_CODICE CodicePaziente,
				                                            PES_TIPO_CASO TipoCaso,
				                                            SEP_ATTIVO Attivo,
                                                            PES_AUTO_POS AutoPositivo,
				                                            CTIPOCASO.COD_DESCRIZIONE DescrizioneTipoCaso,
				                                            PES_ATC_CODICE CodiceTipoContatto,
				                                            ATC_DESCRIZIONE DescrizioneTipoContatto,
				                                            PES_FLAG_OPERATORE_SANITARIO IsOperatoreSanitario,
				                                            PES_PATOLOGIE_CRONICHE HasPatologiaCroniche,
				                                            PES_DATA_SEGNALAZIONE DataSegnalazione,
				                                            PES_ASINTOMATICO IsAsintomatico,
				                                            PES_NOTE Note,
				                                            PES_OPE_CODICE_RACCOLTA CodiceRaccoltaOperatore,
				                                            PES_OPE_TELEFONO TelefonoOperatore,
				                                            OPE_EMAIL EmailOperatore,
				                                            PES_USL_CODICE_RACCOLTA CodiceRaccoltaUsl,
				                                            PES_ASE_CODICE CodiceSegnalatore,
				                                            ASE_DESCRIZIONE DescrizioneSegnalatore,
				                                            PES_SEP_CODICE CodiceStato,
				                                            SEP_DESCRIZIONE DescrizioneStato,
				                                            PES_TIPO_OPERATORE_SANITARIO TipoOperatoreSanitario,
				                                            PES_COMUNE_ESPOSIZIONE EsposizioneComune,
				                                            PES_DATA_INIZIO_ISOLAMENTO DataInizioIsolamento,
				                                            PES_DATA_ULTIMO_CONTATTO DataUltimoContatto,
				                                            PES_DATA_FINE_ISOLAMENTO DataFineIsolamento,
				                                            NVL(PES_INDIRIZZO_ISOLAMENTO, NVL(PAZ_INDIRIZZO_DOMICILIO, PAZ_INDIRIZZO_RESIDENZA)) IndirizzoIsolamento,
				                                            COM_DESCRIZIONE DescrizioneComuneIsolamento,
				                                            NVL(PES_TELEFONO, NVL(PAZ_TELEFONO_1, NVL(PAZ_TELEFONO_2, PAZ_TELEFONO_3))) TelefonoIsolamento,
				                                            PES_EMAIL EmailIsolamento,
				                                            PES_DATA_CHIUSURA DataChiusura,
				                                            PES_DATA_INIZIO_SINTOMI DataInizioSintomi,
				                                            PES_UTE_ID_INSERIMENTO CodiceUtenteInserimento,
				                                            UTE_DESCRIZIONE DescrizioneUtenteInserimento,
				                                            PES_USL_CODICE_INSERIMENTO UslInserimento,
				                                            PES_DATA_INSERIMENTO DataInserimento,
				                                            PES_UTE_ID_ULTIMA_MODIFCA UtenteUltimaModifica,
				                                            PES_DATA_ULTIMA_MODIFICA DataUltimaModifica,
				                                            MED_COGNOME CognomeMedicoBase,
				                                            MED_NOME NomeMedicoBase,
				                                            PAZ_CONFERMA_INFORMATIVA_COVID as ConfermaInformativaCovid,
                                                            NVL(PES_COM_CODICE_ISOLAMENTO, NVL(PAZ_COM_CODICE_DOMICILIO, PAZ_COM_CODICE_RESIDENZA)) CodiceComuneIsolamento
					                                            from T_PAZ_EPISODI
					                                            join T_PAZ_PAZIENTI on PES_PAZ_CODICE = PAZ_CODICE
					                                            left outer join T_ANA_MEDICI on PAZ_MED_CODICE_BASE = MED_CODICE
					                                            left outer join T_ANA_SEGNALATORE on PES_ASE_CODICE = ASE_CODICE
					                                            left outer join T_ANA_UTENTI on PES_UTE_ID_INSERIMENTO = UTE_ID
					                                            left outer join T_ANA_STATI_EPISODIO on PES_SEP_CODICE = SEP_CODICE
					                                            left outer join T_ANA_TIPO_CONTATTO on PES_ATC_CODICE = ATC_CODICE
					                                            left outer join T_ANA_COMUNI on NVL(PES_COM_CODICE_ISOLAMENTO, NVL(PAZ_COM_CODICE_DOMICILIO, PAZ_COM_CODICE_RESIDENZA)) = COM_CODICE
					                                            left outer join T_ANA_CODIFICHE CTIPOCASO on CTIPOCASO.COD_CAMPO = 'PES_TIPO_CASO' and CTIPOCASO.COD_CODICE = PES_TIPO_CASO
					                                            left outer join (
										                                            select 
											                                            o.PED_UTE_ID_INSERIMENTO, o.PED_DATA_RILEVAZIONE, o.PED_PES_ID,
											                                            o.PED_RISPOSTA_TELEFONO as UltimaRisposta,
											                                            o.PED_DATA_RILEVAZIONE as DataUltimaDiaria
												                                            from T_PAZ_EPISODI_DIARIA o
													                                            where o.PED_DATA_RILEVAZIONE = (
																					                                            select 
																						                                            MAX(K.PED_DATA_RILEVAZIONE) from T_PAZ_EPISODI_DIARIA k 
																							                                            where k.PED_PES_ID = o.PED_PES_ID 
																					                                            )
									                                            ) lastD on PES_ID = LASTD.PED_PES_ID
					                                            order by DATAULTIMADIARIA nulls first, cognome, NOME
			                                            ) tab {0}
		                                            ) comp
		                                            left outer join (
							                                            select 
								                                            O.PED_PES_ID, 
								                                            o.PED_ID, 
								                                            o.PED_RISPOSTA_TELEFONO, 
								                                            o.PED_UTE_ID_INSERIMENTO, 
								                                            o.PED_DATA_RILEVAZIONE, 
								                                            PDS_ID,
                                                                            ASI_CODICE,
                                                                            u.UTE_DESCRIZIONE,
								                                            ASI_DESCRIZIONE
									                                            from T_PAZ_EPISODI_DIARIA o
                                                                                inner join T_ANA_UTENTI u on o.PED_UTE_ID_INSERIMENTO = u.UTE_ID
									                                            left outer join T_PAZ_EPISODI_DIARIA_SINTOMI on PED_ID = PDS_PED_ID
										                                            left outer join t_ana_sintomi on PDS_ASI_CODICE = ASI_CODICE
											                                            where o.PED_DATA_RILEVAZIONE = (
																			                                            select MAX(K.PED_DATA_RILEVAZIONE) 
																					                                            from T_PAZ_EPISODI_DIARIA k 
																						                                            where k.PED_PES_ID = o.PED_PES_ID 
																			                                            )
						                                            ) j on COMP.IDEPISODIO = j.PED_PES_ID
		                                            left outer join (
							                                            select 
								                                            T.PET_PES_ID, 
								                                            t.PET_ESITO, 
								                                            t.PET_DATA_TAMPONE
									                                            from T_PAZ_EPISODI_TAMPONI t
										                                            where t.PET_DATA_TAMPONE = (
																	                                            select MAX(PET_DATA_TAMPONE) 
																		                                            from T_PAZ_EPISODI_TAMPONI p 
																			                                            where p.PET_PES_ID = t.PET_PES_ID
																	                                            )
						                                            ) t on COMP.IDEPISODIO = t.PET_PES_ID"

                    Dim condizione As New List(Of String)
                    If ricerca.Skip.HasValue AndAlso ricerca.Take.HasValue Then
                        query += " WHERE NUMERO > :skip and NUMERO <= :takeN"
                        cmd.Parameters.AddWithValue("skip", ricerca.Skip)
                        cmd.Parameters.AddWithValue("takeN", ricerca.Skip + ricerca.Take)
                    End If

                    If ricerca.StatoRilevazione.HasValue AndAlso ricerca.StatoRilevazione > 0 Then
                        condizione.Add("CodiceStato = :codStato")
                        cmd.Parameters.AddWithValue("codStato", ricerca.StatoRilevazione.Value)
                    End If
                    If Not String.IsNullOrWhiteSpace(ricerca.Tipologia) Then
                        condizione.Add("TipoCaso = :codTipo")
                        cmd.Parameters.AddWithValue("codTipo", ricerca.Tipologia)
                    End If
                    If ricerca.DataUltimaDiaria.HasValue Then
                        condizione.Add("(DATAULTIMADIARIA is null or DATAULTIMADIARIA < :ultimaDiaria)")
                        cmd.Parameters.AddWithValue("ultimaDiaria", ricerca.DataUltimaDiaria.Value.Date)
                    End If
                    If ricerca.DataFineIsolamentoPresunto.HasValue Then
                        condizione.Add("DATAFINEISOLAMENTO >= :fineIsolamentoA and DATAFINEISOLAMENTO < :fineIsolamentoB and DATAFINEISOLAMENTO is not null")
                        cmd.Parameters.AddWithValue("fineIsolamentoA", ricerca.DataFineIsolamentoPresunto.Value.Date)
                        cmd.Parameters.AddWithValue("fineIsolamentoB", ricerca.DataFineIsolamentoPresunto.Value.Date.AddDays(1))
                    End If
                    If Not String.IsNullOrWhiteSpace(ricerca.CodiceUslResidenza) Then
                        condizione.Add("CodiceUslResidenza = :uslResidenza")
                        cmd.Parameters.AddWithValue("uslResidenza", ricerca.CodiceUslResidenza)
                    End If
                    If Not String.IsNullOrWhiteSpace(ricerca.CodiceUslInCarico) Then
                        condizione.Add("CodiceRaccoltaUsl = :uslCarico")
                        cmd.Parameters.AddWithValue("uslCarico", ricerca.CodiceUslInCarico)
                    End If
                    If ricerca.SoloSintomatici Then
                        condizione.Add("IsAsintomatico = 'N'")
                    End If
                    If ricerca.AutoPositivi Then
                        condizione.Add("AutoPositivo = 'S'")
                    End If
                    If ricerca.CodicePazienteIndice.HasValue Then
                        condizione.Add("(
                                            EXISTS (
                                                    select 1 from T_PAZ_EPISODI_CONTATTI
                                                        inner join T_PAZ_EPISODI on PEC_PES_ID = PES_ID
                                                            where TAB.CODICEPAZIENTE = PEC_PAZ_CODICE and PES_PAZ_CODICE = :codicePazienteIndice
                                                    )
                                            OR TAB.CODICEPAZIENTE = :codicePazienteIndice
                                        )")
                        cmd.Parameters.AddWithValue("codicePazienteIndice", ricerca.CodicePazienteIndice.Value)
                    End If

                    If (ricerca.TamponiDaVisionare) Then
                        condizione.Add("EXISTS (
                                                select 1 from T_PAZ_EPISODI_TAMPONI t
                                                    WHERE t.PET_FLG_DA_VISIONARE = 'S'
                                                    and TAB.IDEPISODIO  = t.PET_PES_ID
                                                )")
                    End If

                    If (ricerca.DataRefertoTampone.HasValue) Then
                        condizione.Add("EXISTS (
                                                select 1 from T_PAZ_EPISODI_TAMPONI t
                                                    WHERE t.PET_DATA_REFERTO is not null
                                                    AND t.PET_DATA_REFERTO = :dataReferto
                                                    AND TAB.IDEPISODIO  = t.PET_PES_ID
                                                )")
                        cmd.Parameters.AddWithValue("dataReferto", ricerca.DataRefertoTampone.Value.Date)
                    End If

                    If (ricerca.MancataRisposta) Then
                        condizione.Add("UltimaRisposta = 'N'")
                    End If

                    If (ricerca.UltimaDiariaFromApp.HasValue) Then
                        condizione.Add("UltimoUtenteInserimentoDiaria = :utenteInserimento")
                        cmd.Parameters.AddWithValue("utenteInserimento", ricerca.UltimaDiariaFromApp.Value)
                    End If

                    If (Not ricerca.StatoAttivazioneApp = AttivazioneApp.Tutte) Then
                        Select Case ricerca.StatoAttivazioneApp
                            Case AttivazioneApp.Attive
                                condizione.Add("ConfermaInformativaCovid = 'S'")
                            Case AttivazioneApp.NonAttive
                                condizione.Add("(ConfermaInformativaCovid = 'N' or ConfermaInformativaCovid is null)")
                        End Select
                    End If

                    If Not String.IsNullOrWhiteSpace(ricerca.CodiceComuneIsolamento) Then
                        condizione.Add("CodiceComuneIsolamento = :codiceComuneIsolamento")
                        cmd.Parameters.AddWithValue("codiceComuneIsolamento", ricerca.CodiceComuneIsolamento)
                    End If

                    If ricerca.Tags.Any() Then
                        Dim exists As New StringBuilder("exists (select 1 from T_ANA_LINK_EPISODI_TAG where TAB.IDEPISODIO = LET_PES_ID and (")
                        Dim nomeParametro As String
                        For i As Integer = 0 To ricerca.Tags.Length - 2
                            nomeParametro = String.Format("T_{0}", i)
                            exists.Append(String.Format("LET_TAG_ID = :{0} or ", nomeParametro))
                            cmd.Parameters.AddWithValue(nomeParametro, ricerca.Tags(i))
                        Next
                        nomeParametro = String.Format("T_{0}", ricerca.Tags.Length - 1)
                        exists.Append(String.Format("LET_TAG_ID = :{0}", nomeParametro))
                        cmd.Parameters.AddWithValue(nomeParametro, ricerca.Tags.Last())
                        exists.Append("))")
                        condizione.Add(exists.ToString())
                    End If

                    If condizione.Any() Then
                        query = String.Format(query, String.Format("Where {0}", String.Join(" AND ", condizione)))
                    Else
                        query = String.Format(query, "")
                    End If

                    cmd.CommandText = query
                    Return LeggiEpisodi(cmd)
                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try
            End Using
        End Function

        Public Function CercaCasiIndice(idEpisodi As List(Of Long)) As List(Of CasoIndice) Implements IRilevazioniCovid19Provider.CercaCasiIndice
            If IsNothing(idEpisodi) OrElse Not idEpisodi.Any() Then
                Return New List(Of CasoIndice)
            End If
            Return DoCommand(Function(cmd)

                                 Dim take As Integer = 400
                                 Dim pos As Integer = 0
                                 Dim elenco As IEnumerable(Of Long) = idEpisodi.Skip(pos).Take(take)

                                 Dim ritorno As New List(Of CasoIndice)
                                 While elenco.Any()
                                     ritorno.AddRange(_CercaIndici(elenco))
                                     pos += take
                                     elenco = idEpisodi.Skip(pos).Take(take)
                                 End While
                                 Return ritorno

                             End Function)
        End Function

        Private Function _CercaIndici(idEpisodi As IEnumerable(Of Long)) As List(Of CasoIndice)
            Return DoCommand(Function(cmd)

                                 cmd.CommandText = String.Format("SELECT distinct
	                                                        PEC_PES_ID, PAZ_NOME as Nome, PAZ_COGNOME as Cognome, PEC_PES_ID_CONTATTO as PazCodiceByEpisodio
                                                        FROM T_PAZ_EPISODI_CONTATTI tpec 
                                                        JOIN T_PAZ_EPISODI tpe  ON PEC_PES_ID = PES_ID 
                                                        JOIN T_PAZ_PAZIENTI tpp ON PES_PAZ_CODICE = PAZ_CODICE 
                                                            WHERE PEC_PES_ID_CONTATTO in ({0})", cmd.SetParameterIn("I", idEpisodi))
                                 Return cmd.Fill(Of CasoIndice)
                             End Function)
        End Function

        Public Function RicercaTags(ricerca As RicercaTags) As Dictionary(Of String, String) Implements IRilevazioniCovid19Provider.RicercaTags
            Dim result As Dictionary(Of String, String) = New Dictionary(Of String, String)

            Using cmd As OracleCommand = New OracleCommand
                cmd.Connection = Connection
                Dim ownConnection As Boolean = False
                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim query As String = "SELECT * FROM (
                                                      SELECT TAG_ID, TAG_DESCRIZIONE, ROWNUM as NUMERO
                                                      FROM T_ANA_TAG
                                                        WHERE TAG_GRUPPO = 'COVID19-RV'
                                                        {0}
                                                        )
                                            WHERE NUMERO > :skip and NUMERO <= :takeN"

                    cmd.Parameters.AddWithValue("skip", ricerca.Skip)
                    cmd.Parameters.AddWithValue("takeN", ricerca.Skip + ricerca.Take)

                    If Not String.IsNullOrWhiteSpace(ricerca.Descrizione) Then
                        query = String.Format(query, " AND TAG_DESCRIZIONE like :descrizione")
                        cmd.Parameters.AddWithValue("descrizione", ricerca.Descrizione + "%")
                    Else
                        query = String.Format(query, "")
                    End If

                    cmd.CommandText = query
                    Using dr As OracleDataReader = cmd.ExecuteReader()
                        If Not dr Is Nothing Then
                            Dim IdTag As Integer = dr.GetOrdinal("TAG_ID")
                            Dim Descrizione As Integer = dr.GetOrdinal("TAG_DESCRIZIONE")
                            While dr.Read()
                                result.Add(dr.GetInt64(IdTag).ToString(), dr.GetStringOrDefault(Descrizione))
                            End While
                        End If
                    End Using
                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try
            End Using
            Return result
        End Function

        Private Function LeggiEpisodi(cmd As OracleCommand) As List(Of EpisodioPaziente)
            Dim result As New List(Of EpisodioPaziente)
            Dim flatEpisodi As New List(Of FlatEpisodioPaziente)
            Dim keysSintomi As List(Of Long) = New List(Of Long)

            Using dr As OracleDataReader = cmd.ExecuteReader()
                If Not dr Is Nothing Then
                    Dim IdEpisodio As Integer = dr.GetOrdinal("IdEpisodio")
                    Dim CodPaziente As Integer = dr.GetOrdinal("CodicePaziente")
                    Dim DataUltimaDiaria As Integer = dr.GetOrdinal("DataUltimaDiaria")
                    Dim IdUltimaDiaria As Integer = dr.GetOrdinal("IdUltimaDiaria")
                    Dim IdSintomoUltimaDiaria As Integer = dr.GetOrdinal("IdSintomoUltimaDiaria")
                    Dim DescrizioneSintomoUltimaDiaria As Integer = dr.GetOrdinal("DescrizioneSintomoUltimaDiaria")
                    Dim TipoCaso As Integer = dr.GetOrdinal("TipoCaso")
                    Dim DescrizioneTipoCaso As Integer = dr.GetOrdinal("DescrizioneTipoCaso")
                    Dim CodiceTipoContatto As Integer = dr.GetOrdinal("CodiceTipoContatto")
                    Dim DescrizioneTipoContatto As Integer = dr.GetOrdinal("DescrizioneTipoContatto")
                    Dim IsOperatoreSanitario As Integer = dr.GetOrdinal("IsOperatoreSanitario")
                    Dim HasPatologiaCroniche As Integer = dr.GetOrdinal("HasPatologiaCroniche")
                    Dim DataSegnalazione As Integer = dr.GetOrdinal("DataSegnalazione")
                    Dim IsAsintomatico As Integer = dr.GetOrdinal("IsAsintomatico")
                    Dim Note As Integer = dr.GetOrdinal("Note")
                    Dim CodiceRaccoltaOperatore As Integer = dr.GetOrdinal("CodiceRaccoltaOperatore")
                    Dim TelefonoOperatore As Integer = dr.GetOrdinal("TelefonoOperatore")
                    Dim EmailOperatore As Integer = dr.GetOrdinal("EmailOperatore")
                    Dim CodiceRaccoltaUsl As Integer = dr.GetOrdinal("CodiceRaccoltaUsl")
                    Dim CodiceSegnalatore As Integer = dr.GetOrdinal("CodiceSegnalatore")
                    Dim DescrizioneSegnalatore As Integer = dr.GetOrdinal("DescrizioneSegnalatore")
                    Dim CodiceStato As Integer = dr.GetOrdinal("CodiceStato")
                    Dim DescrizioneStato As Integer = dr.GetOrdinal("DescrizioneStato")
                    Dim Attivo As Integer = dr.GetOrdinal("Attivo")
                    Dim TipoOperatoreSanitario As Integer = dr.GetOrdinal("TipoOperatoreSanitario")
                    Dim EsposizioneComune As Integer = dr.GetOrdinal("EsposizioneComune")
                    Dim DataInizioIsolamento As Integer = dr.GetOrdinal("DataInizioIsolamento")
                    Dim IndirizzoIsolamento As Integer = dr.GetOrdinal("IndirizzoIsolamento")
                    Dim TelefonoIsolamento As Integer = dr.GetOrdinal("TelefonoIsolamento")
                    Dim EmailIsolamento As Integer = dr.GetOrdinal("EmailIsolamento")
                    Dim DescrizioneComuneIsolamento As Integer = dr.GetOrdinal("DescrizioneComuneIsolamento")
                    Dim DataUltimoContatto As Integer = dr.GetOrdinal("DataUltimoContatto")
                    Dim DataFineIsolamento As Integer = dr.GetOrdinal("DataFineIsolamento")
                    Dim DataChiusura As Integer = dr.GetOrdinal("DataChiusura")
                    Dim DataInizioSintomi As Integer = dr.GetOrdinal("DataInizioSintomi")
                    Dim DataUltimoTampone As Integer = dr.GetOrdinal("DataUltimoTampone")
                    Dim EsitoUltimoTampone As Integer = dr.GetOrdinal("EsitoUltimoTampone")
                    Dim CodiceUtenteInserimento As Integer = dr.GetOrdinal("CodiceUtenteInserimento")
                    Dim DescrizioneUtenteInserimento As Integer = dr.GetOrdinal("DescrizioneUtenteInserimento")
                    Dim UslInserimento As Integer = dr.GetOrdinal("UslInserimento")
                    Dim DataInserimento As Integer = dr.GetOrdinal("DataInserimento")
                    Dim UtenteUltimaModifica As Integer = dr.GetOrdinal("UtenteUltimaModifica")
                    Dim DataUltimaModifica As Integer = dr.GetOrdinal("DataUltimaModifica")
                    Dim Nome As Integer = dr.GetOrdinal("Nome")
                    Dim Cognome As Integer = dr.GetOrdinal("Cognome")
                    Dim DataNascita As Integer = dr.GetOrdinal("DataNascita")
                    Dim Sesso As Integer = dr.GetOrdinal("Sesso")
                    Dim UtenteInserimentoDiaria As Integer = dr.GetOrdinal("UltimoUtenteInserimentoDiaria")
                    Dim NomeUteUltimaDiaria As Integer = dr.GetOrdinal("NomeUteUltimaDiaria")
                    Dim CodiceFiscale As Integer = dr.GetOrdinal("CodiceFiscale")
                    Dim CognomeMedicoBase As Integer = dr.GetOrdinal("CognomeMedicoBase")
                    Dim NomeMedicoBase As Integer = dr.GetOrdinal("NomeMedicoBase")

                    While dr.Read()
                        Dim ricercaEpisodio As FlatEpisodioPaziente = New FlatEpisodioPaziente()
                        ricercaEpisodio.Nome = dr.GetStringOrDefault(Nome)
                        ricercaEpisodio.Cognome = dr.GetStringOrDefault(Cognome)
                        ricercaEpisodio.DataNascita = dr.GetNullableDateTimeOrDefault(DataNascita)
                        ricercaEpisodio.Sesso = dr.GetStringOrDefault(Sesso)
                        ricercaEpisodio.Cf = dr.GetStringOrDefault(CodiceFiscale)
                        ricercaEpisodio.CognomeMedicoBase = dr.GetStringOrDefault(CognomeMedicoBase)
                        ricercaEpisodio.NomeMedicoBase = dr.GetStringOrDefault(NomeMedicoBase)
                        ricercaEpisodio.DataUltimaDiaria = dr.GetNullableDateTimeOrDefault(DataUltimaDiaria)
                        ricercaEpisodio.IdEpisodio = dr.GetInt64(IdEpisodio)
                        ricercaEpisodio.CodicePaziente = dr.GetInt64(CodPaziente)
                        ricercaEpisodio.CodiceTipoCaso = dr.GetStringOrDefault(TipoCaso)
                        ricercaEpisodio.DescrizioneTipoCaso = dr.GetStringOrDefault(DescrizioneTipoCaso)
                        ricercaEpisodio.CodiceTipoContatto = dr.GetInt32OrDefault(CodiceTipoContatto)
                        ricercaEpisodio.CodiceStato = dr.GetInt32OrDefault(CodiceStato)
                        ricercaEpisodio.DescrizioneStato = dr.GetStringOrDefault(DescrizioneStato)
                        ricercaEpisodio.IsOperatoreSanitario = SNtoBool(dr.GetStringOrDefault(IsOperatoreSanitario))
                        ricercaEpisodio.HasPatologiaCroniche = SNtoBool(dr.GetStringOrDefault(HasPatologiaCroniche))
                        ricercaEpisodio.DataSegnalazione = dr.GetNullableDateTimeOrDefault(DataSegnalazione)
                        ricercaEpisodio.NominativoUtenteInserimentoUltimaDiaria = dr.GetStringOrDefault(NomeUteUltimaDiaria)
                        Dim asintomaticoStr As String = dr.GetStringOrDefault(IsAsintomatico)
                        If String.IsNullOrWhiteSpace(asintomaticoStr) Then
                            ricercaEpisodio.IsAsintomatico = True
                        Else
                            ricercaEpisodio.IsAsintomatico = SNtoBool(asintomaticoStr)
                        End If
                        ricercaEpisodio.Note = dr.GetStringOrDefault(Note)
                        ricercaEpisodio.CodiceRaccoltaOperatore = dr.GetStringOrDefault(CodiceRaccoltaOperatore)
                        ricercaEpisodio.TelefonoOperatore = dr.GetStringOrDefault(TelefonoOperatore)
                        ricercaEpisodio.EmailOperatore = dr.GetStringOrDefault(EmailOperatore)
                        ricercaEpisodio.CodiceRaccoltaUsl = dr.GetStringOrDefault(CodiceRaccoltaUsl)
                        ricercaEpisodio.CodiceSegnalatore = dr.GetInt32OrDefault(CodiceSegnalatore)
                        ricercaEpisodio.DescrizioneSegnalatore = dr.GetStringOrDefault(DescrizioneSegnalatore)
                        ricercaEpisodio.CodiceTipoOperatoreSanitario = dr.GetStringOrDefault(TipoOperatoreSanitario)
                        ricercaEpisodio.EsposizioneComune = dr.GetStringOrDefault(EsposizioneComune)
                        ricercaEpisodio.DataInizioIsolamento = dr.GetNullableDateTimeOrDefault(DataInizioIsolamento)
                        ricercaEpisodio.DataUltimoContatto = dr.GetNullableDateTimeOrDefault(DataUltimoContatto)
                        ricercaEpisodio.DataFineIsolamento = dr.GetNullableDateTimeOrDefault(DataFineIsolamento)
                        ricercaEpisodio.DescrizioneComuneIsolamento = dr.GetStringOrDefault(DescrizioneComuneIsolamento)
                        ricercaEpisodio.IndirizzoIsolamento = dr.GetStringOrDefault(IndirizzoIsolamento)
                        ricercaEpisodio.TelefonoIsolamento = dr.GetStringOrDefault(TelefonoIsolamento)
                        ricercaEpisodio.EmailIsolamento = dr.GetStringOrDefault(EmailIsolamento)
                        ricercaEpisodio.DataChiusura = dr.GetNullableDateTimeOrDefault(DataChiusura)
                        ricercaEpisodio.DataInizioSintomi = dr.GetNullableDateTimeOrDefault(DataInizioSintomi)
                        ricercaEpisodio.DataUltimoTampone = dr.GetNullableDateTimeOrDefault(DataUltimoTampone)
                        ricercaEpisodio.EsitoUltimoTampone = dr.GetStringOrDefault(EsitoUltimoTampone)
                        ricercaEpisodio.CodiceUtenteInserimento = dr.GetInt64(CodiceUtenteInserimento)
                        ricercaEpisodio.DescrizioneUtenteInserimento = dr.GetStringOrDefault(DescrizioneUtenteInserimento)
                        ricercaEpisodio.UslInserimento = dr.GetStringOrDefault(UslInserimento)
                        ricercaEpisodio.DataInserimento = dr.GetNullableDateTimeOrDefault(DataInserimento)
                        ricercaEpisodio.UtenteUltimaModifica = dr.GetNullableInt64OrDefault(UtenteUltimaModifica)
                        ricercaEpisodio.DataUltimaModifica = dr.GetNullableDateTimeOrDefault(DataUltimaModifica)
                        ricercaEpisodio.IdUltimaDiaria = dr.GetNullableInt64OrDefault(IdUltimaDiaria)
                        ricercaEpisodio.IdSintomoUltimaDiaria = dr.GetInt64OrDefault(IdSintomoUltimaDiaria)
                        ricercaEpisodio.DescrizioneSintomoUltimaDiaria = dr.GetStringOrDefault(DescrizioneSintomoUltimaDiaria)
                        ricercaEpisodio.CodiceUtenteDiaria = dr.GetNullableInt64OrDefault(UtenteInserimentoDiaria)

                        'se l'episodio non ha uno stato associato, deve essere considerato attivo
                        Dim episodioAttivoString As String = dr.GetStringOrDefault(Attivo)
                        If String.IsNullOrWhiteSpace(episodioAttivoString) Then
                            ricercaEpisodio.Attivo = True
                        Else
                            ricercaEpisodio.Attivo = SNtoBool(episodioAttivoString)
                        End If

                        flatEpisodi.Add(ricercaEpisodio)

                    End While

                    result = flatEpisodi.GroupBy(Function(x) x.IdEpisodio).Select(Function(g)
                                                                                      Dim el As FlatEpisodioPaziente = g.First()
                                                                                      Return New EpisodioPaziente With
                                                                                      {
                                                                                        .Testata = New EpisodioTestata With {
                                                                                            .IdEpisodio = g.Key,
                                                                                            .Attivo = el.Attivo,
                                                                                            .CodicePaziente = el.CodicePaziente,
                                                                                            .CodiceStato = el.CodiceStato,
                                                                                            .CodiceTipoCaso = el.CodiceTipoCaso,
                                                                                            .CodiceTipoContatto = el.CodiceTipoContatto,
                                                                                            .DataSegnalazione = el.DataSegnalazione,
                                                                                            .DataUltimaDiaria = el.DataUltimaDiaria,
                                                                                            .DescrizioneStato = el.DescrizioneStato,
                                                                                            .DescrizioneTipoCaso = el.DescrizioneTipoCaso,
                                                                                            .DescrizioneTipoContatto = el.DescrizioneTipoContatto,
                                                                                            .EmailRilevazione = el.EmailIsolamento,
                                                                                            .HasPatologiaCroniche = el.HasPatologiaCroniche,
                                                                                            .IsAsintomatico = el.IsAsintomatico,
                                                                                            .IsOperatoreSanitario = el.IsOperatoreSanitario,
                                                                                            .Note = el.Note,
                                                                                            .TelefonoRilevazione = el.TelefonoIsolamento,
                                                                                            .UtenteInserimentoDiaria = el.CodiceUtenteDiaria,
                                                                                            .NominativoUtenteInserimentoUltimaDiaria = el.NominativoUtenteInserimentoUltimaDiaria
                                                                                        },
                                                                                        .Contatti = Nothing,
                                                                                        .DatoreLavoro = New DatoreLavoro(),
                                                                                        .Diaria = g.Where(Function(x) x.IdUltimaDiaria.HasValue).GroupBy(Function(tmp) tmp.IdUltimaDiaria).Select(Function(d)
                                                                                                                                                                                                      Return New Diaria With {
                                                                                                                                                            .CodiceDiaria = d.Key,
                                                                                                                                                            .Sintomi = d.Select(Function(s) New Sintomo With {
                                                                                                                                                                .CodiceSintomo = s.IdSintomoUltimaDiaria,
                                                                                                                                                                .Descrizione = s.DescrizioneSintomoUltimaDiaria
                                                                                                                                                            }).ToList()
                                                                                                                                                         }
                                                                                                                                                                                                  End Function).ToList(),
                                                                                        .Dettaglio = New EpisodioDettaglio With {
                                                                                            .CodicePazienteOld = 0,
                                                                                            .CodiceRaccoltaOperatore = "",
                                                                                            .DataChiusura = el.DataChiusura,
                                                                                            .CodiceRaccoltaUsl = el.CodiceRaccoltaUsl,
                                                                                            .CodiceSegnalatore = el.CodiceSegnalatore,
                                                                                            .CodiceTipoOperatoreSanitario = el.CodiceTipoOperatoreSanitario,
                                                                                            .CodiceUtenteInserimento = el.CodiceUtenteInserimento,
                                                                                            .ComuneCodiceIsolamento = el.ComuneCodiceIsolamento,
                                                                                            .DataEliminazione = Nothing,
                                                                                            .DataFineIsolamento = el.DataFineIsolamento,
                                                                                            .DataInizioIsolamento = el.DataInizioIsolamento,
                                                                                            .DataInizioSintomi = el.DataInizioSintomi,
                                                                                            .DataInserimento = el.DataInserimento,
                                                                                            .DataUltimaModifica = el.DataUltimaModifica,
                                                                                            .DataUltimoContatto = el.DataUltimoContatto,
                                                                                            .DataUltimoTampone = el.DataUltimoTampone,
                                                                                            .DescrizioneComuneIsolamento = el.DescrizioneComuneIsolamento,
                                                                                            .DescrizioneSegnalatore = el.DescrizioneSegnalatore,
                                                                                            .DescrizioneSintomi = "",
                                                                                            .DescrizioneTipoOperatoreSanitario = "",
                                                                                            .DescrizioneUtenteInserimento = el.DescrizioneUtenteInserimento,
                                                                                            .EmailIsolamento = el.EmailIsolamento,
                                                                                            .EmailOperatore = el.EmailOperatore,
                                                                                            .EsitoUltimoTampone = el.EsitoUltimoTampone,
                                                                                            .EsposizioneComune = el.EsposizioneComune,
                                                                                            .IndirizzoIsolamento = el.IndirizzoIsolamento,
                                                                                            .TelefonoIsolamento = el.TelefonoIsolamento,
                                                                                            .TelefonoOperatore = el.TelefonoOperatore,
                                                                                            .UslInserimento = el.UslInserimento,
                                                                                            .UslEliminazione = Nothing,
                                                                                            .UtenteEliminazione = Nothing,
                                                                                            .UtenteUltimaModifica = el.UtenteUltimaModifica
                                                                                        },
                                                                                        .Paziente = New EpisodioDatiPaziente With {
                                                                                            .Cf = el.Cf,
                                                                                            .Cognome = el.Cognome,
                                                                                            .CognomeMedicoBase = el.CognomeMedicoBase,
                                                                                            .ComuneResidenza = el.DescrizioneComuneIsolamento,
                                                                                            .DataNascita = el.DataNascita,
                                                                                            .HasConfermaInformativaCovid = False,
                                                                                            .HasCredenzialiApp = False,
                                                                                            .Nome = el.Nome,
                                                                                            .NomeMedicoBase = el.NomeMedicoBase,
                                                                                            .Sesso = el.Sesso,
                                                                                            .TelefonoDue = "",
                                                                                            .TelefonoTre = "",
                                                                                            .TelefonoUno = ""
                                                                                        },
                                                                                        .Ricoveri = Nothing,
                                                                                        .Tags = Nothing,
                                                                                        .Tamponi = Nothing
                                                                                      }
                                                                                  End Function).ToList()
                End If
            End Using
            cmd.Parameters.Clear()

            'recupero i cluster
            Dim exportTags As List(Of ExportTag) = New List(Of ExportTag)
            cmd.CommandText = "select LET_PES_ID, 
                                      TAG_DESCRIZIONE
                                        from T_ANA_LINK_EPISODI_TAG
                                        inner join T_ANA_TAG on LET_TAG_ID = TAG_ID"
            Using dr As OracleDataReader = cmd.ExecuteReader()
                If Not dr Is Nothing Then
                    Dim IdEpisodio As Integer = dr.GetOrdinal("LET_PES_ID")
                    Dim TagDescrizione As Integer = dr.GetOrdinal("TAG_DESCRIZIONE")

                    While dr.Read()
                        Dim tag As ExportTag = New ExportTag
                        tag.IdEpisodio = dr.GetInt64(IdEpisodio)
                        tag.Descrizione = dr.GetStringOrDefault(TagDescrizione)
                        exportTags.Add(tag)
                    End While
                End If
            End Using

            'recupero i cluster nella lista episodi ritornata nella GUI
            For Each episodio As EpisodioPaziente In result
                episodio.Tags = exportTags.Where(Function(x) x.IdEpisodio = episodio.Testata.IdEpisodio).Select(Function(y)
                                                                                                                    Return New TagTmp With
                                                                                                                    {
                                                                                                                    .Descrizione = y.Descrizione
                                                                                                                    }
                                                                                                                End Function).ToList()
            Next

            Return result
        End Function

        Public Function GetEpisodio(idEpisodio As Long) As EpisodioPaziente Implements IRilevazioniCovid19Provider.GetEpisodio
            Dim episodio As EpisodioPaziente = Nothing
            Using cmd As OracleCommand = New OracleCommand
                cmd.Connection = Connection
                Dim ownConnection As Boolean = False
                Try
                    ownConnection = ConditionalOpenConnection(cmd)
                    Dim query As String = "select 
                                              (select COD_DESCRIZIONE from T_ANA_CODIFICHE where COD_CODICE = PET_ESITO and COD_CAMPO = 'PET_ESITO') as DESCRIZIONEESITO,
                                              (select COD_DESCRIZIONE from T_ANA_CODIFICHE where COD_CODICE = PEC_TIPO_RAPPORTO and COD_CAMPO = 'PEC_TIPO_RAPPORTO') as DESCRIZIONETIPORAPPORTO,
                                              (select COD_DESCRIZIONE from T_ANA_CODIFICHE where COD_CODICE = PES_TIPO_OPERATORE_SANITARIO and COD_CAMPO = 'PES_TIPO_OPERATORE_SANITARIO') as OPERATORESANITARIO,
                                              (select COD_DESCRIZIONE from T_ANA_CODIFICHE where COD_CODICE = TAB.PES_TIPO_CASO and COD_CAMPO = 'PES_TIPO_CASO') as DESCRIZIONETIPOCASO,
                                              (select COM_DESCRIZIONE from T_ANA_COMUNI where COM_CODICE = NVL(TAB.PES_COM_CODICE_ISOLAMENTO, NVL(TAB.SOG_COM_CODICE_DOMICILIO, TAB.SOG_COM_CODICE_RESIDENZA))) as DESCRIZIONECOMUNEISOLAMENTO,
                                              (select COM_DESCRIZIONE from T_ANA_COMUNI where COM_CODICE = TAB.PEL_COM_CODICE) as DESCRIZIONECOMUNELAVORO,
                                            tab.*
                                             from ( select *
                                              FROM T_PAZ_EPISODI
                                                left outer join T_ANA_MOTIVI_GENOTIP on PES_MOTIVO_GENOTIP = TMG_CODICE
                                                left outer join T_ANA_TIPI_VARIANTI on PES_VARIANTE_VIRUS = TAV_CODICE
                                                left outer join T_ANA_SEGNALATORE on PES_ASE_CODICE = ASE_CODICE
                                                left outer join T_ANA_STATI_EPISODIO on PES_SEP_CODICE = SEP_CODICE
                                                left outer join T_ANA_TIPO_CONTATTO on PES_ATC_CODICE = ATC_CODICE
                                                left outer join T_PAZ_EPISODI_DATORE_LAVORO on PES_ID = PEL_PES_ID
                                                                left outer join T_ANA_TIPO_AZIENDA  on PEL_ATA_CODICE = ATA_CODICE
                                                                left outer join T_ANA_COMUNI ON PEL_COM_CODICE = COM_DESCRIZIONE
                                                left outer join T_PAZ_EPISODI_RICOVERI on PES_ID = PER_PES_ID
                                                                left outer join T_ANA_REPARTI on PER_ARE_CODICE = ARE_CODICE
                                                                left outer join T_ANA_CODICI_HSP on PER_HSP_ID = HSP_ID
                                                left outer join T_PAZ_EPISODI_TAMPONI on PES_ID = PET_PES_ID
                                                    LEFT JOIN T_ANA_TIPI_TAMPONE ON pet_tipo_Tampone = TAT_CODICE 
                                                left outer join T_ANA_LABORATORI_MICRO on PET_TLM_ID = TLM_ID
                                                left outer join T_PAZ_EPISODI_DIARIA on PES_ID = PED_PES_ID
                                                                left outer join T_PAZ_EPISODI_DIARIA_SINTOMI on PED_ID = PDS_PED_ID
                                                                                left outer join T_ANA_SINTOMI on PDS_ASI_CODICE = ASI_CODICE
                                                left outer join T_PAZ_EPISODI_CONTATTI on PES_ID = PEC_PES_ID
                                                                left outer join T_PAZ_PAZIENTI on PEC_PAZ_CODICE = PAZ_CODICE
                                                left outer join T_ANA_LINK_EPISODI_TAG on PES_ID = LET_PES_ID
                                                                left outer join T_ANA_TAG on LET_TAG_ID = TAG_ID
                                                inner join (
                                                                select PAZ_CODICE as SOG_CODICE,
                                                                       PAZ_CREDENZIALI_APP as CREDENZIALI,
                                                                       PAZ_CONFERMA_INFORMATIVA_COVID as INFORMATIVA,
                                                                       PAZ_COM_CODICE_DOMICILIO as SOG_COM_CODICE_DOMICILIO,
                                                                       PAZ_COM_CODICE_RESIDENZA as SOG_COM_CODICE_RESIDENZA
                                                                            from T_PAZ_PAZIENTI
                                                            ) sog on PES_PAZ_CODICE = SOG_CODICE
                                                left outer join T_PAZ_EPISODI_CLINICA on PES_ID = PCL_PES_ID
                                                WHERE PES_ID = :codiceEpisodio) tab"
                    cmd.Parameters.AddWithValue("codiceEpisodio", idEpisodio)
                    cmd.CommandText = query

                    Using dr As OracleDataReader = cmd.ExecuteReader()
                        If Not dr Is Nothing Then
                            Dim keyEpisodio As Long = -1
                            Dim keysRicoveri As List(Of Long) = New List(Of Long)
                            Dim keysTampone As List(Of Long) = New List(Of Long)
                            Dim keysContatto As List(Of Long) = New List(Of Long)
                            Dim keysDiaria As List(Of Long) = New List(Of Long)
                            Dim keysSintomi As List(Of Long) = New List(Of Long)
                            Dim keysTag As List(Of Long) = New List(Of Long)

                            Dim ordinals As Dictionary(Of String, Integer) = GetOrdinals(dr)

                            While dr.Read()
                                'se è la prima lettura creo l'episodio, altrimenti aggiungo i campi di dettaglio
                                If keyEpisodio < 0 Then
                                    episodio = New EpisodioPaziente()
                                    keyEpisodio = dr.GetInt64(ordinals.Item("PES_ID"))
                                    episodio.Testata.IdEpisodio = keyEpisodio
                                    episodio.Paziente.Nome = dr.GetStringOrDefault(ordinals.Item("PAZ_NOME"))
                                    episodio.Paziente.Cognome = dr.GetStringOrDefault(ordinals.Item("PAZ_COGNOME"))
                                    episodio.Paziente.HasCredenzialiApp = SNtoBool(dr.GetStringOrDefault(ordinals.Item("CREDENZIALI")))
                                    episodio.Paziente.HasConfermaInformativaCovid = SNtoBool(dr.GetStringOrDefault(ordinals.Item("INFORMATIVA")))
                                    episodio.Testata.CodicePaziente = dr.GetInt64(ordinals.Item("PES_PAZ_CODICE"))
                                    episodio.Testata.CodiceTipoCaso = dr.GetStringOrDefault(ordinals.Item("PES_TIPO_CASO"))
                                    episodio.Testata.DescrizioneTipoCaso = dr.GetStringOrDefault(ordinals.Item("DESCRIZIONETIPOCASO"))
                                    episodio.Testata.CodiceTipoContatto = dr.GetNullableInt32OrDefault(ordinals.Item("PES_ATC_CODICE"))
                                    episodio.Testata.DescrizioneTipoContatto = dr.GetStringOrDefault(ordinals.Item("ATC_DESCRIZIONE"))
                                    episodio.Testata.CodiceStato = dr.GetNullableInt32OrDefault(ordinals.Item("PES_SEP_CODICE"))
                                    episodio.Testata.DescrizioneStato = dr.GetStringOrDefault(ordinals.Item("SEP_DESCRIZIONE"))
                                    episodio.Testata.IsOperatoreSanitario = SNtoBool(dr.GetStringOrDefault(ordinals.Item("PES_FLAG_OPERATORE_SANITARIO")))
                                    episodio.Testata.HasPatologiaCroniche = SNtoBool(dr.GetStringOrDefault(ordinals.Item("PES_PATOLOGIE_CRONICHE")))
                                    episodio.Testata.DataSegnalazione = dr.GetNullableDateTimeOrDefault(ordinals.Item("PES_DATA_SEGNALAZIONE"))
                                    episodio.Testata.IsAsintomatico = SNtoBool(dr.GetStringOrDefault(ordinals.Item("PES_ASINTOMATICO")))
                                    episodio.Testata.Note = dr.GetStringOrDefault(ordinals.Item("PES_NOTE"))
                                    episodio.Testata.TelefonoRilevazione = dr.GetStringOrDefault(ordinals.Item("PES_TELEFONO"))
                                    episodio.Testata.EmailRilevazione = dr.GetStringOrDefault(ordinals.Item("PES_EMAIL"))

                                    episodio.Dettaglio.CodiceMotivoGenotipizzazione = dr.GetNullableInt64OrDefault(ordinals.Item("TMG_CODICE"))
                                    episodio.Dettaglio.DescrizioneMotivoGenotipizzazione = dr.GetStringOrDefault(ordinals.Item("TMG_DESCRIZIONE"))
                                    episodio.Dettaglio.CodiceVariante = dr.GetNullableInt64OrDefault(ordinals.Item("PES_VARIANTE_VIRUS"))
                                    episodio.Dettaglio.DescrizioneVariante = dr.GetStringOrDefault(ordinals.Item("TAV_DESCRIZIONE"))
                                    episodio.Dettaglio.DataDecessoCovid = dr.GetNullableDateTimeOrDefault(ordinals.Item("PES_DATA_DECESSO_COVID"))
                                    episodio.Dettaglio.otp = dr.GetStringOrDefault(ordinals.Item("PES_OTP"))
                                    episodio.Dettaglio.InternoRegione = dr.GetStringOrDefault(ordinals.Item("PES_INTERNO"))
                                    episodio.Dettaglio.CodiceComune = dr.GetStringOrDefault(ordinals.Item("PES_COM_CODICE"))
                                    episodio.Dettaglio.CodiceRaccoltaOperatore = dr.GetStringOrDefault(ordinals.Item("PES_OPE_CODICE_RACCOLTA"))
                                    episodio.Dettaglio.TelefonoOperatore = dr.GetStringOrDefault(ordinals.Item("PES_OPE_TELEFONO"))
                                    episodio.Dettaglio.EmailOperatore = dr.GetStringOrDefault(ordinals.Item("OPE_EMAIL"))
                                    episodio.Dettaglio.CodiceRaccoltaUsl = dr.GetStringOrDefault(ordinals.Item("PES_USL_CODICE_RACCOLTA"))
                                    episodio.Dettaglio.CodiceSegnalatore = dr.GetNullableInt32OrDefault(ordinals.Item("PES_ASE_CODICE"))
                                    episodio.Dettaglio.DescrizioneSegnalatore = dr.GetStringOrDefault(ordinals.Item("ASE_DESCRIZIONE"))
                                    episodio.Dettaglio.DescrizioneTipoOperatoreSanitario = dr.GetStringOrDefault(ordinals.Item("OPERATORESANITARIO"))
                                    episodio.Dettaglio.CodiceTipoOperatoreSanitario = dr.GetStringOrDefault(ordinals.Item("PES_TIPO_OPERATORE_SANITARIO"))
                                    episodio.Dettaglio.EsposizioneComune = dr.GetStringOrDefault(ordinals.Item("PES_COMUNE_ESPOSIZIONE"))
                                    episodio.Dettaglio.DataInizioIsolamento = dr.GetNullableDateTimeOrDefault(ordinals.Item("PES_DATA_INIZIO_ISOLAMENTO"))
                                    episodio.Dettaglio.DataUltimoContatto = dr.GetNullableDateTimeOrDefault(ordinals.Item("PES_DATA_ULTIMO_CONTATTO"))
                                    episodio.Dettaglio.DataFineIsolamento = dr.GetNullableDateTimeOrDefault(ordinals.Item("PES_DATA_FINE_ISOLAMENTO"))
                                    episodio.Dettaglio.DataChiusura = dr.GetNullableDateTimeOrDefault(ordinals.Item("PES_DATA_CHIUSURA"))
                                    episodio.Dettaglio.DataInizioSintomi = dr.GetNullableDateTimeOrDefault(ordinals.Item("PES_DATA_INIZIO_SINTOMI"))
                                    episodio.Dettaglio.CodiceUtenteInserimento = dr.GetInt64(ordinals.Item("PES_UTE_ID_INSERIMENTO"))
                                    episodio.Dettaglio.UslInserimento = dr.GetStringOrDefault(ordinals.Item("PES_USL_CODICE_INSERIMENTO"))
                                    episodio.Dettaglio.DataInserimento = dr.GetNullableDateTimeOrDefault(ordinals.Item("PES_DATA_INSERIMENTO"))
                                    episodio.Dettaglio.UtenteUltimaModifica = dr.GetNullableInt64OrDefault(ordinals.Item("PES_UTE_ID_ULTIMA_MODIFCA"))
                                    episodio.Dettaglio.DataUltimaModifica = dr.GetNullableDateTimeOrDefault(ordinals.Item("PES_DATA_ULTIMA_MODIFICA"))
                                    episodio.Dettaglio.CodicePazienteOld = dr.GetInt64OrDefault(ordinals.Item("PES_PAZ_CODICE_OLD"))
                                    episodio.Dettaglio.ComuneCodiceIsolamento = dr.GetStringOrDefault(ordinals.Item("PES_COM_CODICE_ISOLAMENTO"))
                                    episodio.Dettaglio.DescrizioneComuneIsolamento = dr.GetStringOrDefault(ordinals.Item("DESCRIZIONECOMUNEISOLAMENTO"))
                                    episodio.Dettaglio.IndirizzoIsolamento = dr.GetStringOrDefault(ordinals.Item("PES_INDIRIZZO_ISOLAMENTO"))
                                    episodio.Dettaglio.SpsId = dr.GetInt64OrDefault(ordinals.Item("PES_SPS_ID"))
                                    episodio.Dettaglio.TpsId = dr.GetInt64OrDefault(ordinals.Item("PES_TPS_ID"))
                                    episodio.Dettaglio.GuaritoClinicamente = SNtoBool(dr.GetStringOrDefault(ordinals.Item("PES_GUARITO_CLINICAMENTE")))
                                    episodio.Dettaglio.DataGuaritoClinicamente = dr.GetNullableDateTimeOrDefault(ordinals.Item("PES_DATA_GUARITO_CLINICAMENTE"))
                                    episodio.Dettaglio.Guarito = SNtoBool(dr.GetStringOrDefault(ordinals.Item("PES_GUARITO")))
                                    episodio.Dettaglio.DataGuarigioneVirol = dr.GetNullableDateTimeOrDefault(ordinals.Item("PES_DATA_GUARITO_VIROLOGIC"))

                                    episodio.DatoreLavoro.DenominazioneAzienda = dr.GetStringOrDefault(ordinals.Item("PEL_DENOMINAZIONE_AZIENDA"))
                                    episodio.DatoreLavoro.RiferimentoDatoreLavoro = dr.GetStringOrDefault(ordinals.Item("PEL_RIFERIMENTO"))
                                    episodio.DatoreLavoro.CodiceComuneSede = dr.GetStringOrDefault(ordinals.Item("PEL_COM_CODICE"))
                                    episodio.DatoreLavoro.DescrizioneComuneSede = dr.GetStringOrDefault(ordinals.Item("DESCRIZIONECOMUNELAVORO"))
                                    episodio.DatoreLavoro.ContattoTelefonico = dr.GetStringOrDefault(ordinals.Item("PEL_TELEFONO"))
                                    episodio.DatoreLavoro.ContattoEmail = dr.GetStringOrDefault(ordinals.Item("PEL_EMAIL"))
                                    episodio.DatoreLavoro.Note = dr.GetStringOrDefault(ordinals.Item("PEL_NOTE"))
                                    episodio.DatoreLavoro.CodiceInserimentoUsl = dr.GetStringOrDefault(ordinals.Item("PEL_USL_CODICE_INSERIMENTO"))
                                    episodio.DatoreLavoro.DataInserimento = dr.GetNullableDateTimeOrDefault(ordinals.Item("PEL_DATA_INSERIMENTO"))
                                    episodio.DatoreLavoro.CodiceTipoAzienda = dr.GetInt32OrDefault(ordinals.Item("PEL_ATA_CODICE"))
                                    episodio.DatoreLavoro.DescrizioneTipoAzienda = dr.GetStringOrDefault(ordinals.Item("ATA_DESCRIZIONE"))

                                    episodio.Clinica.Tumore = SNtoBool(dr.GetStringOrDefault(ordinals.Item("PCL_TUMORE")))
                                    episodio.Clinica.Diabete = SNtoBool(dr.GetStringOrDefault(ordinals.Item("PCL_DIABETE")))
                                    episodio.Clinica.MalattiaCardiovascolare = SNtoBool(dr.GetStringOrDefault(ordinals.Item("PCL_MAL_CARDIOVASCOLARI")))
                                    episodio.Clinica.ImmunoDeficienza = SNtoBool(dr.GetStringOrDefault(ordinals.Item("PCL_DEF_IMMUNITARI")))
                                    episodio.Clinica.MalattiaRespiratoria = SNtoBool(dr.GetStringOrDefault(ordinals.Item("PCL_MAL_RESPIRATORIE")))
                                    episodio.Clinica.MalattiaRenale = SNtoBool(dr.GetStringOrDefault(ordinals.Item("PCL_MAL_RENALI")))
                                    episodio.Clinica.MalattiaMetabolica = SNtoBool(dr.GetStringOrDefault(ordinals.Item("PCL_MAL_METABOLICHE")))
                                    episodio.Clinica.Altro = SNtoBool(dr.GetStringOrDefault(ordinals.Item("PCL_ALTRO")))
                                    episodio.Clinica.Obesita_BMI_30_40 = SNtoBool(dr.GetStringOrDefault(ordinals.Item("PCL_OBESITA_BMI_30_40")))
                                    episodio.Clinica.Obesita_BMI_Maggiore_40 = SNtoBool(dr.GetStringOrDefault(ordinals.Item("PCL_OBESITA_BMI_MAGGIORE_40")))
                                    episodio.Clinica.Note = dr.GetStringOrDefault(ordinals.Item("PCL_NOTE"))
                                End If

                                'se è presente un Tag e non è ancora stato aggiunto viene letto
                                Dim idTag As Long = dr.GetInt64OrDefault(ordinals.Item("TAG_ID"))

                                If (idTag > 0 AndAlso Not keysTag.Contains(idTag)) Then
                                    keysTag.Add(idTag)
                                    episodio.Tags.Add(New TagTmp With {
                                        .Id = idTag.ToString(),
                                        .Descrizione = dr.GetStringOrDefault(ordinals.Item("TAG_DESCRIZIONE"))
                                    })
                                End If

                                'se è presente una chiave Ricovero e non è ancora stata aggiunta viene creata con il rispettivo reparto
                                Dim idRicovero As Long = dr.GetInt64OrDefault(ordinals.Item("PER_ID"))

                                If (idRicovero > 0 AndAlso Not keysRicoveri.Contains(idRicovero)) Then
                                    keysRicoveri.Add(idRicovero)
                                    Dim ricovero As Ricovero = New Ricovero()
                                    ricovero.CodiceRicovero = idRicovero
                                    ricovero.CodiceStruttura = dr.GetStringOrDefault(ordinals.Item("PER_HSP_ID"))
                                    ricovero.DescrizioneStruttura = dr.GetStringOrDefault(ordinals.Item("HSP_DESCRIZIONE"))
                                    ricovero.CodiceUsl = dr.GetStringOrDefault(ordinals.Item("PER_USL_CODICE"))
                                    ricovero.CodiceReparto = dr.GetInt32OrDefault(ordinals.Item("ARE_CODICE"))
                                    ricovero.DescrizioneReparto = dr.GetStringOrDefault(ordinals.Item("ARE_DESCRIZIONE"))
                                    ricovero.DataInizio = dr.GetNullableDateTimeOrDefault(ordinals.Item("PER_DATA_INIZIO_RICOVERO"))
                                    ricovero.DataFine = dr.GetNullableDateTimeOrDefault(ordinals.Item("PER_DATA_FINE_RICOVERO"))
                                    ricovero.Note = dr.GetStringOrDefault(ordinals.Item("PER_NOTE"))
                                    ricovero.UtenteInserimento = dr.GetInt64(ordinals.Item("PER_UTE_ID_INSERIMENTO"))
                                    ricovero.CodiceInserimentoUsl = dr.GetStringOrDefault(ordinals.Item("PER_USL_CODICE_INSERIMENTO"))
                                    ricovero.DataInserimento = dr.GetNullableDateTimeOrDefault(ordinals.Item("PER_DATA_INSERIMENTO"))

                                    episodio.Ricoveri.Add(ricovero)
                                End If

                                'se è presente una chiave Tampone e non è ancora stata aggiunta viene creata
                                Dim idTampone As Long = dr.GetInt64OrDefault(ordinals.Item("PET_ID"))

                                If (idTampone > 0 AndAlso Not keysTampone.Contains(idTampone)) Then
                                    keysTampone.Add(idTampone)
                                    Dim tampone As Tampone = New Tampone()
                                    tampone.CodiceTampone = idTampone
                                    tampone.CodiceUsl = dr.GetStringOrDefault(ordinals.Item("PET_USL_CODICE"))
                                    tampone.DataTampone = dr.GetNullableDateTimeOrDefault(ordinals.Item("PET_DATA_TAMPONE"))
                                    tampone.CodiceEsito = dr.GetStringOrDefault(ordinals.Item("PET_ESITO"))
                                    tampone.DescrizioneEsito = dr.GetStringOrDefault(ordinals.Item("DESCRIZIONEESITO"))
                                    tampone.Note = dr.GetStringOrDefault(ordinals.Item("PET_NOTE"))
                                    tampone.UtenteInserimento = dr.GetInt64OrDefault(ordinals.Item("PET_UTE_ID_INSERIMENTO"))
                                    tampone.CodiceInserimentoUsl = dr.GetStringOrDefault(ordinals.Item("PET_USL_CODICE_INSERIMENTO"))
                                    tampone.DataInserimento = dr.GetNullableDateTimeOrDefault(ordinals.Item("PET_DATA_INSERIMENTO"))
                                    tampone.CodiceCampione = dr.GetStringOrDefault(ordinals.Item("PET_ID_CAMPIONE"))
                                    tampone.CodiceLab = dr.GetStringOrDefault(ordinals.Item("PET_CODICE_LABORATORIO"))
                                    tampone.DaVisionare = SNtoBool(dr.GetStringOrDefault(ordinals.Item("PET_FLG_DA_VISIONARE")))
                                    tampone.DataRichiesta = dr.GetNullableDateTimeOrDefault(ordinals.Item("PET_DATA_RICHIESTA"))
                                    tampone.DataReferto = dr.GetNullableDateTimeOrDefault(ordinals.Item("PET_DATA_REFERTO"))

                                    tampone.CodiceLaboratorio = dr.GetNullableInt64OrDefault(ordinals.Item("TLM_ID"))
                                    tampone.DescrizioneLaboratorio = dr.GetStringOrDefault(ordinals.Item("TLM_CODICE_LAB"))
                                    tampone.CodiceTipologia = dr.GetNullableInt64OrDefault(ordinals.Item("TAT_CODICE"))
                                    tampone.MnemonicoTipologia = dr.GetStringOrDefault(ordinals.Item("TAT_MNEMONICO"))
                                    tampone.DescrizioneTipologia = dr.GetStringOrDefault(ordinals.Item("TAT_DESCRIZIONE"))
                                    episodio.Tamponi.Add(tampone)
                                End If

                                'se è presente una chiave Contatto e non è ancora stata aggiunta viene creta con il rispettivo paziente
                                Dim idContatto As Long = dr.GetInt64OrDefault(ordinals.Item("PEC_ID"))

                                If (idContatto > 0 AndAlso Not keysContatto.Contains(idContatto)) Then
                                    keysContatto.Add(idContatto)
                                    Dim contatto As Contatto = New Contatto()
                                    contatto.CodiceContatto = idContatto
                                    contatto.CodiceTipoRapporto = dr.GetStringOrDefault(ordinals.Item("PEC_TIPO_RAPPORTO"))
                                    contatto.DescrizioneTipoRapporto = dr.GetStringOrDefault(ordinals.Item("DESCRIZIONETIPORAPPORTO"))
                                    contatto.Note = dr.GetStringOrDefault(ordinals.Item("PEC_NOTE"))
                                    contatto.UtenteInserimento = dr.GetInt64(ordinals.Item("PEC_UTE_ID_INSERIMENTO"))
                                    contatto.CodiceInserimentoUsl = dr.GetStringOrDefault(ordinals.Item("PEC_USL_CODICE_INSERIMENTO"))
                                    contatto.DataInserimento = dr.GetNullableDateTimeOrDefault(ordinals.Item("PEC_DATA_INSERIMENTO"))
                                    contatto.CodicePaziente = dr.GetNullableInt64OrDefault(ordinals.Item("PEC_PAZ_CODICE"))
                                    contatto.CodiceImportazione = dr.GetStringOrDefault(ordinals.Item("PEC_ECE_GRUPPO"))

                                    Dim data As Date? = dr.GetNullableDateTimeOrDefault(ordinals.Item("PAZ_DATA_NASCITA"))
                                    contatto.DescrizionePaziente = dr.GetStringOrDefault(ordinals.Item("PAZ_COGNOME")) + " " +
                                        dr.GetStringOrDefault(ordinals.Item("PAZ_NOME"))
                                    If data.HasValue Then
                                        contatto.DescrizionePaziente += " - " + data.Value.ToShortDateString()
                                    End If
                                    contatto.Telefono = dr.GetStringOrDefault(ordinals.Item("PEC_TELEFONO"))
                                    contatto.CodiceEpisodioContatto = dr.GetNullableInt64OrDefault(ordinals.Item("PEC_PES_ID_CONTATTO"))
                                    episodio.Contatti.Add(contatto)
                                End If

                                Dim idDiaria As Long = dr.GetInt64OrDefault(ordinals.Item("PED_ID"))

                                If (idDiaria > 0 AndAlso Not keysDiaria.Contains(idDiaria)) Then
                                    keysDiaria.Add(idDiaria)
                                    Dim diaria As Diaria = New Diaria()
                                    diaria.CodiceDiaria = idDiaria
                                    diaria.CodiceRilevazioneUsl = dr.GetStringOrDefault(ordinals.Item("PED_USL_CODICE_RILEVAZIONE"))
                                    diaria.DataRilevazione = dr.GetNullableDateTimeOrDefault(ordinals.Item("PED_DATA_RILEVAZIONE"))
                                    diaria.Asintomatico = SNtoBool(dr.GetStringOrDefault(ordinals.Item("PED_FLAG_ASINTOMATICO")))
                                    diaria.Note = dr.GetStringOrDefault(ordinals.Item("PED_NOTE"))
                                    diaria.UtenteInserimento = dr.GetInt64(ordinals.Item("PED_UTE_ID_INSERIMENTO"))
                                    diaria.CodiceInserimentoUsl = dr.GetStringOrDefault(ordinals.Item("PED_USL_CODICE_INSERIMENTO"))
                                    diaria.DataInserimento = dr.GetNullableDateTimeOrDefault(ordinals.Item("PED_DATA_INSERIMENTO"))
                                    diaria.RispostaTelefono = SNtoBool(dr.GetStringOrDefault(ordinals.Item("PED_RISPOSTA_TELEFONO")))

                                    episodio.Diaria.Add(diaria)
                                End If

                                Dim idSintomo As Long = dr.GetInt64OrDefault(ordinals.Item("PDS_ID"))

                                If (idSintomo > 0 AndAlso Not keysSintomi.Contains(idSintomo)) Then
                                    keysSintomi.Add(idSintomo)
                                    Dim diaria As Diaria = (From item In episodio.Diaria
                                                            Where item.CodiceDiaria = idDiaria
                                                            Select item).First

                                    Dim sintomo As Sintomo = New Sintomo()
                                    sintomo.idSintomo = idSintomo
                                    sintomo.CodiceSintomo = dr.GetInt32OrDefault(ordinals.Item("PDS_ASI_CODICE"))
                                    sintomo.Note = dr.GetStringOrDefault(ordinals.Item("PDS_NOTE"))
                                    sintomo.UtenteInserimento = dr.GetNullableInt64OrDefault(ordinals.Item("PDS_UTE_ID_INSERIMENTO"))
                                    sintomo.CodiceInserimentoUsl = dr.GetStringOrDefault(ordinals.Item("PDS_USL_CODICE_INSERIMENTO"))
                                    sintomo.DataInserimento = dr.GetNullableDateTimeOrDefault(ordinals.Item("PDS_DATA_INSERIMENTO"))
                                    sintomo.Descrizione = dr.GetStringOrDefault(ordinals.Item("ASI_DESCRIZIONE"))
                                    sintomo.Ordine = dr.GetInt32OrDefault(ordinals.Item("ASI_ORDINE"))

                                    diaria.Sintomi.Add(sintomo)
                                End If
                            End While
                        End If
                    End Using
                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try
            End Using
            Return episodio
        End Function

        Private Function setValue(obje As Object) As Object
            If obje = Nothing Then
                Return DBNull.Value
            End If
            Return obje
        End Function
        Private Function setValueLongDefault(obje As Object) As Long
            If obje = Nothing Then
                Return 0
            End If
            Return obje
        End Function
        Public Function InsertEpisodio(episodio As EpisodioPaziente) As Long Implements IRilevazioniCovid19Provider.InsertEpisodio
            Dim newIdEpisodio As Long = 0
            Using cmd As OracleCommand = New OracleCommand
                cmd.Connection = Connection
                Dim ownConnection As Boolean = False
                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    'get progressivo T_PAZ_EPISODIO
                    cmd.CommandText = "select SEQ_PAZ_EPISODI.nextval from dual"
                    newIdEpisodio = cmd.ExecuteScalar()
                    episodio.Testata.IdEpisodio = newIdEpisodio

                    'INSERT T_PAZ_EPISODIO
                    Dim queryInsertEpisodio As String = "INSERT INTO T_PAZ_EPISODI (
                                                              PES_ID,
                                                              PES_PAZ_CODICE,
                                                              PES_TIPO_CASO,
                                                              PES_ATC_CODICE,
                                                              PES_USL_CODICE_RACCOLTA,
                                                              PES_ASE_CODICE,
                                                              PES_SEP_CODICE,
                                                              PES_FLAG_OPERATORE_SANITARIO,
                                                              PES_TIPO_OPERATORE_SANITARIO,
                                                              PES_PATOLOGIE_CRONICHE,
                                                              PES_DATA_DECESSO_COVID,
                                                              PES_DATA_SEGNALAZIONE,
                                                              PES_COMUNE_ESPOSIZIONE,
                                                              PES_DATA_INIZIO_ISOLAMENTO,
                                                              PES_DATA_ULTIMO_CONTATTO,
                                                              PES_DATA_FINE_ISOLAMENTO,
                                                              PES_DATA_CHIUSURA,
                                                              PES_ASINTOMATICO,
                                                              PES_DATA_INIZIO_SINTOMI,
                                                              PES_NOTE,
                                                              PES_UTE_ID_INSERIMENTO,
                                                              PES_USL_CODICE_INSERIMENTO,
                                                              PES_DATA_INSERIMENTO,
                                                              PES_UTE_ID_ULTIMA_MODIFCA,
                                                              PES_DATA_ULTIMA_MODIFICA,
                                                              PES_PAZ_CODICE_OLD,
                                                              PES_INDIRIZZO_ISOLAMENTO,
                                                              PES_COM_CODICE_ISOLAMENTO,
                                                              PES_TELEFONO,
                                                              PES_EMAIL,
                                                              PES_INTERNO,
                                                              PES_COM_CODICE,
                                                              PES_SPS_ID,
                                                              PES_TPS_ID,
                                                              PES_GUARITO_CLINICAMENTE,
                                                              PES_DATA_GUARITO_CLINICAMENTE,
                                                              PES_GUARITO,
                                                              PES_DATA_GUARITO_VIROLOGIC,
                                                              PES_VARIANTE_VIRUS,
                                                              PES_MOTIVO_GENOTIP
                                                            )
                                                            VALUES (
                                                            :PES_ID,
                                                            :PES_PAZ_CODICE,
                                                            :PES_TIPO_CASO,
                                                            :PES_ATC_CODICE,
                                                            :PES_USL_CODICE_RACCOLTA,
                                                            :PES_ASE_CODICE,
                                                            :PES_SEP_CODICE,
                                                            :PES_FLAG_OPERATORE_SANITARIO,
                                                            :PES_TIPO_OPERATORE_SANITARIO,
                                                            :PES_PATOLOGIE_CRONICHE,
                                                            :PES_DATA_DECESSO_COVID,
                                                            :PES_DATA_SEGNALAZIONE,
                                                            :PES_COMUNE_ESPOSIZIONE,
                                                            :PES_DATA_INIZIO_ISOLAMENTO,
                                                            :PES_DATA_ULTIMO_CONTATTO,
                                                            :PES_DATA_FINE_ISOLAMENTO,
                                                            :PES_DATA_CHIUSURA,
                                                            :PES_ASINTOMATICO,
                                                            :PES_DATA_INIZIO_SINTOMI,
                                                            :PES_NOTE,
                                                            :PES_UTE_ID_INSERIMENTO,
                                                            :PES_USL_CODICE_INSERIMENTO,
                                                            :PES_DATA_INSERIMENTO,
                                                            :PES_UTE_ID_ULTIMA_MODIFCA,
                                                            :PES_DATA_ULTIMA_MODIFICA,
                                                            :PES_PAZ_CODICE_OLD,
                                                            :PES_INDIRIZZO_ISOLAMENTO,
                                                            :PES_COM_CODICE_ISOLAMENTO,
                                                            :PES_TELEFONO,
                                                            :PES_EMAIL,
                                                            :PES_INTERNO,
                                                            :PES_COM_CODICE,
                                                            :PES_SPS_ID,
                                                            :PES_TPS_ID,
                                                            :PES_GUARITO_CLINICAMENTE,
                                                            :PES_DATA_GUARITO_CLINICAMENTE,
                                                            :PES_GUARITO,
                                                            :PES_DATA_GUARITO_VIROLOGIC,
                                                            :PES_VARIANTE_VIRUS,
                                                            :PES_MOTIVO_GENOTIP)"

                    cmd.Parameters.AddWithValue("PES_ID", newIdEpisodio)
                    cmd.Parameters.AddWithValue("PES_UTE_ID_INSERIMENTO", setValue(episodio.Dettaglio.CodiceUtenteInserimento))
                    cmd.Parameters.AddWithValue("PES_USL_CODICE_INSERIMENTO", setValue(episodio.Dettaglio.UslInserimento))
                    cmd.Parameters.AddWithValue("PES_DATA_INSERIMENTO", setValue(episodio.Dettaglio.DataInserimento))
                    cmd.Parameters.AddWithValue("PES_SPS_ID", setValueLongDefault(episodio.Dettaglio.SpsId))
                    cmd.Parameters.AddWithValue("PES_TPS_ID", setValueLongDefault(episodio.Dettaglio.TpsId))
                    AddCommonParametersEpisodio(cmd, episodio)

                    cmd.CommandText = queryInsertEpisodio
                    cmd.ExecuteNonQuery()
                    cmd.Parameters.Clear()
                    'FINE T_PAZ_EPISODIO

                    'INSERT T_PAZ_DATORE_LAVORO
                    Dim queryInsertDatoreLavoro As String = "INSERT INTO T_PAZ_EPISODI_DATORE_LAVORO(
                                                            PEL_PES_ID,
                                                            PEL_DENOMINAZIONE_AZIENDA,
                                                            PEL_RIFERIMENTO,
                                                            PEL_COM_CODICE,
                                                            PEL_TELEFONO,
                                                            PEL_EMAIL,
                                                            PEL_ATA_CODICE,
                                                            PEL_NOTE,
                                                            PEL_UTE_ID_INSERIMENTO,
                                                            PEL_USL_CODICE_INSERIMENTO,
                                                            PEL_DATA_INSERIMENTO
                                                        )
                                                        VALUES (
                                                        :PEL_PES_ID,
                                                        :PEL_DENOMINAZIONE_AZIENDA,
                                                        :PEL_RIFERIMENTO,
                                                        :PEL_COM_CODICE,
                                                        :PEL_TELEFONO,
                                                        :PEL_EMAIL,
                                                        :PEL_ATA_CODICE,
                                                        :PEL_NOTE,
                                                        :PEL_UTE_ID_INSERIMENTO,
                                                        :PEL_USL_CODICE_INSERIMENTO,
                                                        :PEL_DATA_INSERIMENTO
                                                        )"
                    cmd.Parameters.AddWithValue("PEL_PES_ID", newIdEpisodio)
                    cmd.Parameters.AddWithValue("PEL_UTE_ID_INSERIMENTO", setValue(episodio.DatoreLavoro.UtenteInserimento))
                    cmd.Parameters.AddWithValue("PEL_USL_CODICE_INSERIMENTO", setValue(episodio.DatoreLavoro.CodiceInserimentoUsl))
                    cmd.Parameters.AddWithValue("PEL_DATA_INSERIMENTO", setValue(episodio.DatoreLavoro.DataInserimento))
                    AddCommonParametersDatoreLavoro(cmd, episodio)

                    cmd.CommandText = queryInsertDatoreLavoro
                    cmd.ExecuteNonQuery()
                    cmd.Parameters.Clear()
                    'FINE T_PAZ_DATORE_LAVORO

                    'INSERT CLINICA
                    cmd.CommandText = QueryInsertClinica
                    cmd.Parameters.AddWithValue("PCL_PES_ID", newIdEpisodio)
                    cmd.Parameters.AddWithValue("PCL_UTE_ID_INSERIMENTO", setValue(episodio.Clinica.UtenteInserimento))
                    cmd.Parameters.AddWithValue("PCL_USL_CODICE_INSERIMENTO", setValue(episodio.Clinica.CodiceInserimentoUsl))
                    cmd.Parameters.AddWithValue("PCL_DATA_INSERIMENTO", setValue(episodio.Clinica.DataInserimento))
                    AddCommonParametersClinica(cmd, episodio)
                    cmd.ExecuteNonQuery()
                    cmd.Parameters.Clear()
                    'FINE INSERT CLINICA

                    'INSERT TAG
                    If (episodio.Tags.Count > 0) Then
                        InsertTags(cmd, episodio)
                    End If
                    'FINE INSERT TAG

                    'INSERT T_PAZ_EPISODI_RICOVERI (se presenti)
                    If (episodio.Ricoveri.Count > 0) Then
                        Dim queryInsertRicovero As String = "INSERT INTO T_PAZ_EPISODI_RICOVERI (
                                                              PER_PES_ID,
                                                              PER_HSP_ID,
                                                              PER_USL_CODICE,
                                                              PER_ARE_CODICE,
                                                              PER_DATA_INIZIO_RICOVERO,
                                                              PER_DATA_FINE_RICOVERO,
                                                              PER_NOTE,
                                                              PER_UTE_ID_INSERIMENTO,
                                                              PER_USL_CODICE_INSERIMENTO,
                                                              PER_DATA_INSERIMENTO
                                                            )
                                                            VALUES (
                                                              :PER_PES_ID,
                                                              :PER_HSP_ID,
                                                              :PER_USL_CODICE,
                                                              :PER_ARE_CODICE,
                                                              :PER_DATA_INIZIO_RICOVERO,
                                                              :PER_DATA_FINE_RICOVERO,
                                                              :PER_NOTE,
                                                              :PER_UTE_ID_INSERIMENTO,
                                                              :PER_USL_CODICE_INSERIMENTO,
                                                              :PER_DATA_INSERIMENTO
                                                            )"
                        cmd.CommandText = queryInsertRicovero

                        For i As Integer = 0 To episodio.Ricoveri.Count - 1
                            Dim ricoveroToAdd As Ricovero = episodio.Ricoveri.ElementAt(i)
                            cmd.Parameters.AddWithValue("PER_PES_ID", newIdEpisodio)
                            cmd.Parameters.AddWithValue("PER_UTE_ID_INSERIMENTO", setValue(ricoveroToAdd.UtenteInserimento))
                            cmd.Parameters.AddWithValue("PER_USL_CODICE_INSERIMENTO", setValue(ricoveroToAdd.CodiceInserimentoUsl))
                            cmd.Parameters.AddWithValue("PER_DATA_INSERIMENTO", setValue(ricoveroToAdd.DataInserimento))
                            AddCommonParametersRicoveri(cmd, ricoveroToAdd)

                            cmd.ExecuteNonQuery()
                            cmd.Parameters.Clear()
                        Next
                    End If
                    'FINE T_PAZ_EPISODI_RICOVERI

                    'INSERT T_PAZ_EPISODI_TAMPONI (se presenti)
                    If (episodio.Tamponi.Count > 0) Then
                        Dim query As String = QueryInsertTamponi

                        cmd.CommandText = query

                        For i As Integer = 0 To episodio.Tamponi.Count - 1
                            Dim tamponeToAdd As Tampone = episodio.Tamponi.ElementAt(i)
                            cmd.Parameters.AddWithValue("PET_PES_ID", newIdEpisodio)
                            cmd.Parameters.AddWithValue("PET_UTE_ID_INSERIMENTO", setValue(tamponeToAdd.UtenteInserimento))
                            cmd.Parameters.AddWithValue("PET_USL_CODICE_INSERIMENTO", setValue(tamponeToAdd.CodiceInserimentoUsl))
                            cmd.Parameters.AddWithValue("PET_DATA_INSERIMENTO", setValue(tamponeToAdd.DataInserimento))
                            AddCommonParametersTamponi(cmd, tamponeToAdd)

                            cmd.ExecuteNonQuery()
                            cmd.Parameters.Clear()
                        Next
                    End If
                    'FINE T_PAZ_EPISODI_TAMPONI

                    'INSERT T_PAZ_EPISODI_CONTATTI
                    If (episodio.Contatti.Count > 0) Then
                        'recupero tipo contatto di default
                        cmd.CommandText = QueryTipoCasoDefault
                        Dim tipoCasoDefault As String = cmd.ExecuteScalar

                        For Each contattoToAdd As Contatto In episodio.Contatti
                            If (contattoToAdd.CodicePaziente.HasValue) Then
                                CreaContattiAndEpisodiContatti(cmd, contattoToAdd, episodio, tipoCasoDefault)
                            End If
                        Next
                    End If
                    'FINE T_PAZ_EPISODI_CONTATTI

                    'INSERT T_PAZ_DIARIA
                    If (episodio.Diaria.Count > 0) Then
                        Dim queryInsertDiaria As String = "INSERT INTO T_PAZ_EPISODI_DIARIA (
                                                            PED_ID,
                                                            PED_PES_ID,
                                                            PED_USL_CODICE_RILEVAZIONE,
                                                            PED_DATA_RILEVAZIONE,
                                                            PED_FLAG_ASINTOMATICO,
                                                            PED_NOTE,
                                                            PED_UTE_ID_INSERIMENTO,
                                                            PED_USL_CODICE_INSERIMENTO,
                                                            PED_DATA_INSERIMENTO,
                                                            PED_RISPOSTA_TELEFONO
                                                            )
                                                            VALUES (
                                                            :PED_ID,
                                                            :PED_PES_ID,
                                                            :PED_USL_CODICE_RILEVAZIONE,
                                                            :PED_DATA_RILEVAZIONE,
                                                            :PED_FLAG_ASINTOMATICO,
                                                            :PED_NOTE,
                                                            :PED_UTE_ID_INSERIMENTO,
                                                            :PED_USL_CODICE_INSERIMENTO,
                                                            :PED_DATA_INSERIMENTO,
                                                            :PED_RISPOSTA_TELEFONO
                                                            )"

                        For i As Integer = 0 To episodio.Diaria.Count - 1
                            cmd.CommandText = "select SEQ_PAZ_EPISODI_DIARIA.nextval from dual"
                            Dim newIdDiaria As Long = cmd.ExecuteScalar()
                            cmd.CommandText = queryInsertDiaria
                            Dim diariaToAdd As Diaria = episodio.Diaria.ElementAt(i)

                            cmd.Parameters.AddWithValue("PED_ID", newIdDiaria)
                            cmd.Parameters.AddWithValue("PED_PES_ID", newIdEpisodio)
                            cmd.Parameters.AddWithValue("PED_UTE_ID_INSERIMENTO", setValue(diariaToAdd.UtenteInserimento))
                            cmd.Parameters.AddWithValue("PED_USL_CODICE_INSERIMENTO", setValue(diariaToAdd.CodiceInserimentoUsl))
                            cmd.Parameters.AddWithValue("PED_DATA_INSERIMENTO", setValue(diariaToAdd.DataInserimento))
                            AddCommonParametersDiaria(cmd, diariaToAdd)

                            cmd.ExecuteNonQuery()
                            cmd.Parameters.Clear()

                            'INSERT SINTOMI
                            If (diariaToAdd.Sintomi.Count > 0) Then
                                Dim queryInsertSintomo As String = "INSERT INTO T_PAZ_EPISODI_DIARIA_SINTOMI (
                                                                            PDS_PED_ID,
                                                                            PDS_ASI_CODICE,
                                                                            PDS_NOTE,
                                                                            PDS_UTE_ID_INSERIMENTO,
                                                                            PDS_USL_CODICE_INSERIMENTO,
                                                                            PDS_DATA_INSERIMENTO
                                                                            )
                                                                            VALUES (
                                                                            :PDS_PED_ID,
                                                                            :PDS_ASI_CODICE,
                                                                            :PDS_NOTE,
                                                                            :PDS_UTE_ID_INSERIMENTO,
                                                                            :PDS_USL_CODICE_INSERIMENTO,
                                                                            :PDS_DATA_INSERIMENTO
                                                                            )"
                                cmd.CommandText = queryInsertSintomo

                                For k As Integer = 0 To diariaToAdd.Sintomi.Count - 1
                                    Dim sintomoToAdd As Sintomo = diariaToAdd.Sintomi.ElementAt(k)
                                    cmd.Parameters.AddWithValue("PDS_PED_ID", newIdDiaria)
                                    cmd.Parameters.AddWithValue("PDS_UTE_ID_INSERIMENTO", setValue(sintomoToAdd.UtenteInserimento))
                                    cmd.Parameters.AddWithValue("PDS_USL_CODICE_INSERIMENTO", setValue(sintomoToAdd.CodiceInserimentoUsl))
                                    cmd.Parameters.AddWithValue("PDS_DATA_INSERIMENTO", setValue(sintomoToAdd.DataInserimento))
                                    AddCommonParametersSintomi(cmd, sintomoToAdd)

                                    cmd.ExecuteNonQuery()
                                    cmd.Parameters.Clear()
                                Next
                            End If
                            'FINE INSERT SINTOMI
                        Next
                    End If
                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try
            End Using
            Return newIdEpisodio
        End Function

        Public Sub UpdateEpisodio(episodio As EpisodioPaziente) Implements IRilevazioniCovid19Provider.UpdateEpisodio
            Using cmd As OracleCommand = New OracleCommand
                cmd.Connection = Connection
                Dim ownConnection As Boolean = False
                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    'UPDATE T_PAZ_EPISODIO
                    Dim queryUpdateEpisodio As String = "UPDATE T_PAZ_EPISODI SET
                                                            PES_PAZ_CODICE = :PES_PAZ_CODICE,
                                                            PES_DATA_DECESSO_COVID = :PES_DATA_DECESSO_COVID,
                                                            PES_TIPO_CASO = :PES_TIPO_CASO,
                                                            PES_ATC_CODICE = :PES_ATC_CODICE,
                                                            PES_USL_CODICE_RACCOLTA = :PES_USL_CODICE_RACCOLTA,
                                                            PES_ASE_CODICE = :PES_ASE_CODICE,
                                                            PES_SEP_CODICE = :PES_SEP_CODICE,
                                                            PES_FLAG_OPERATORE_SANITARIO = :PES_FLAG_OPERATORE_SANITARIO,
                                                            PES_TIPO_OPERATORE_SANITARIO = :PES_TIPO_OPERATORE_SANITARIO,
                                                            PES_PATOLOGIE_CRONICHE = :PES_PATOLOGIE_CRONICHE,
                                                            PES_DATA_SEGNALAZIONE = :PES_DATA_SEGNALAZIONE,
                                                            PES_COMUNE_ESPOSIZIONE = :PES_COMUNE_ESPOSIZIONE,
                                                            PES_DATA_INIZIO_ISOLAMENTO = :PES_DATA_INIZIO_ISOLAMENTO,
                                                            PES_DATA_ULTIMO_CONTATTO = :PES_DATA_ULTIMO_CONTATTO,
                                                            PES_DATA_FINE_ISOLAMENTO = :PES_DATA_FINE_ISOLAMENTO,
                                                            PES_DATA_CHIUSURA = :PES_DATA_CHIUSURA,
                                                            PES_ASINTOMATICO = :PES_ASINTOMATICO,
                                                            PES_DATA_INIZIO_SINTOMI = :PES_DATA_INIZIO_SINTOMI,
                                                            PES_NOTE = :PES_NOTE,
                                                            PES_UTE_ID_ULTIMA_MODIFCA = :PES_UTE_ID_ULTIMA_MODIFCA,
                                                            PES_DATA_ULTIMA_MODIFICA = :PES_DATA_ULTIMA_MODIFICA,
                                                            PES_PAZ_CODICE_OLD = :PES_PAZ_CODICE_OLD,
                                                            PES_INDIRIZZO_ISOLAMENTO = :PES_INDIRIZZO_ISOLAMENTO,
                                                            PES_COM_CODICE_ISOLAMENTO = :PES_COM_CODICE_ISOLAMENTO,
                                                            PES_TELEFONO = :PES_TELEFONO,
                                                            PES_EMAIL = :PES_EMAIL,
                                                            PES_OTP = :PES_OTP,
                                                            PES_COM_CODICE = :PES_COM_CODICE,
                                                            PES_INTERNO = :PES_INTERNO,
                                                            PES_SPS_ID = :PES_SPS_ID,
                                                            PES_TPS_ID = :PES_TPS_ID,
                                                            PES_GUARITO_CLINICAMENTE = :PES_GUARITO_CLINICAMENTE,
                                                            PES_DATA_GUARITO_CLINICAMENTE = :PES_DATA_GUARITO_CLINICAMENTE,
                                                            PES_AUTO_POS = 'N',
                                                            PES_GUARITO = :PES_GUARITO,
                                                            PES_DATA_GUARITO_VIROLOGIC = :PES_DATA_GUARITO_VIROLOGIC,
                                                            PES_VARIANTE_VIRUS = :PES_VARIANTE_VIRUS,
                                                            PES_MOTIVO_GENOTIP = :PES_MOTIVO_GENOTIP
                                                                WHERE PES_ID = :idEpisodio"

                    cmd.Parameters.AddWithValue("idEpisodio", episodio.Testata.IdEpisodio)
                    AddCommonParametersEpisodio(cmd, episodio)

                    cmd.Parameters.AddWithValue("PES_OTP", setValue(episodio.Dettaglio.otp))
                    cmd.Parameters.AddWithValue("PES_SPS_ID", setValueLongDefault(episodio.Dettaglio.SpsId))
                    cmd.Parameters.AddWithValue("PES_TPS_ID", setValueLongDefault(episodio.Dettaglio.TpsId))

                    cmd.CommandText = queryUpdateEpisodio
                    cmd.ExecuteNonQuery()
                    cmd.Parameters.Clear()
                    'FINE T_PAZ_EPISODIO

                    'UPDATE T_PAZ_DATORE_LAVORO
                    Dim queryUpdateDatoreLavoro As String = "UPDATE T_PAZ_EPISODI_DATORE_LAVORO SET
                                                    PEL_DENOMINAZIONE_AZIENDA = :PEL_DENOMINAZIONE_AZIENDA,
                                                    PEL_RIFERIMENTO = :PEL_RIFERIMENTO,
                                                    PEL_COM_CODICE = :PEL_COM_CODICE,
                                                    PEL_TELEFONO = :PEL_TELEFONO,
                                                    PEL_EMAIL = :PEL_EMAIL,
                                                    PEL_ATA_CODICE = :PEL_ATA_CODICE,
                                                    PEL_NOTE = :PEL_NOTE
                                                        WHERE PEL_PES_ID = :idEpisodio"
                    cmd.Parameters.AddWithValue("idEpisodio", episodio.Testata.IdEpisodio)
                    AddCommonParametersDatoreLavoro(cmd, episodio)

                    cmd.CommandText = queryUpdateDatoreLavoro
                    cmd.ExecuteNonQuery()
                    cmd.Parameters.Clear()
                    'FINE T_PAZ_DATORE_LAVORO

                    'UPDATE CLINICA
                    cmd.CommandText = QueryUpdateClinica
                    cmd.Parameters.AddWithValue("idEpisodio", episodio.Testata.IdEpisodio)
                    AddCommonParametersClinica(cmd, episodio)
                    cmd.ExecuteNonQuery()
                    cmd.Parameters.Clear()
                    'FINE CLINICA

                    'UPDATE TAGS
                    If (episodio.Tags.Count > 0) Then
                        InsertTags(cmd, episodio)
                    End If
                    'FINE TAGS

                    'UPDATE T_PAZ_EPISODI_RICOVERI
                    If (episodio.Ricoveri.Count > 0) Then
                        Dim queryUpdateRicovero As String = "UPDATE T_PAZ_EPISODI_RICOVERI SET
                                                                PER_HSP_ID = :PER_HSP_ID,
                                                                PER_USL_CODICE = :PER_USL_CODICE,
                                                                PER_ARE_CODICE = :PER_ARE_CODICE,
                                                                PER_DATA_INIZIO_RICOVERO = :PER_DATA_INIZIO_RICOVERO,
                                                                PER_DATA_FINE_RICOVERO = :PER_DATA_FINE_RICOVERO,
                                                                PER_NOTE = :PER_NOTE
                                                                    WHERE PER_ID = :idRicovero"

                        Dim queryInsertRicovero As String = "INSERT INTO T_PAZ_EPISODI_RICOVERI (
                                                              PER_PES_ID,
                                                              PER_HSP_ID,
                                                              PER_USL_CODICE,
                                                              PER_ARE_CODICE,
                                                              PER_DATA_INIZIO_RICOVERO,
                                                              PER_DATA_FINE_RICOVERO,
                                                              PER_NOTE,
                                                              PER_UTE_ID_INSERIMENTO,
                                                              PER_USL_CODICE_INSERIMENTO,
                                                              PER_DATA_INSERIMENTO
                                                            )
                                                            VALUES (
                                                              :PER_PES_ID,
                                                              :PER_HSP_ID,
                                                              :PER_USL_CODICE,
                                                              :PER_ARE_CODICE,
                                                              :PER_DATA_INIZIO_RICOVERO,
                                                              :PER_DATA_FINE_RICOVERO,
                                                              :PER_NOTE,
                                                              :PER_UTE_ID_INSERIMENTO,
                                                              :PER_USL_CODICE_INSERIMENTO,
                                                              :PER_DATA_INSERIMENTO
                                                            )"

                        For Each ricovero As Ricovero In episodio.Ricoveri
                            If ricovero.CodiceRicovero = 0 Then
                                cmd.CommandText = queryInsertRicovero

                                cmd.Parameters.AddWithValue("PER_PES_ID", episodio.Testata.IdEpisodio)
                                cmd.Parameters.AddWithValue("PER_UTE_ID_INSERIMENTO", setValue(ricovero.UtenteInserimento))
                                cmd.Parameters.AddWithValue("PER_USL_CODICE_INSERIMENTO", setValue(ricovero.CodiceInserimentoUsl))
                                cmd.Parameters.AddWithValue("PER_DATA_INSERIMENTO", setValue(ricovero.DataInserimento))
                            Else
                                cmd.CommandText = queryUpdateRicovero
                                cmd.Parameters.AddWithValue("idRicovero", ricovero.CodiceRicovero)
                            End If

                            AddCommonParametersRicoveri(cmd, ricovero)
                            cmd.ExecuteNonQuery()
                            cmd.Parameters.Clear()
                        Next
                    End If
                    'FINE T_PAZ_EPISODI_RICOVERI

                    'UPDATE T_PAZ_EPISODI_TAMPONI
                    If (episodio.Tamponi.Count > 0) Then
                        Dim updateTampone As String = QueryUpdateTamponi

                        Dim insertTampone As String = QueryInsertTamponi

                        For Each tampone As Tampone In episodio.Tamponi
                            If tampone.CodiceTampone = 0 Then
                                cmd.CommandText = insertTampone
                                cmd.Parameters.AddWithValue("PET_PES_ID", episodio.Testata.IdEpisodio)
                                cmd.Parameters.AddWithValue("PET_UTE_ID_INSERIMENTO", setValue(tampone.UtenteInserimento))
                                cmd.Parameters.AddWithValue("PET_USL_CODICE_INSERIMENTO", setValue(tampone.CodiceInserimentoUsl))
                                cmd.Parameters.AddWithValue("PET_DATA_INSERIMENTO", setValue(tampone.DataInserimento))
                            Else
                                cmd.CommandText = updateTampone
                                cmd.Parameters.AddWithValue("idTampone", tampone.CodiceTampone)
                            End If

                            AddCommonParametersTamponi(cmd, tampone)
                            cmd.ExecuteNonQuery()
                            cmd.Parameters.Clear()
                        Next
                    End If
                    'FINE T_PAZ_EPISODI_TAMPONI

                    'UPDATE T_PAZ_EPISODI_CONTATTI
                    If (episodio.Contatti.Count > 0) Then
                        cmd.CommandText = QueryTipoCasoDefault
                        Dim tipoCasoDefault As String = cmd.ExecuteScalar
                        For Each contatto As Contatto In episodio.Contatti
                            If (contatto.CodicePaziente.HasValue) Then
                                If contatto.CodiceContatto = 0 Then
                                    'se il codice contatto = 0, allora questo contatto è stato inserito
                                    CreaContattiAndEpisodiContatti(cmd, contatto, episodio, tipoCasoDefault)
                                Else
                                    'se il codice contatto > 0, allora il contatto era già esistente ed eseguo solo l'update.
                                    cmd.CommandText = QueryUpdateContatti
                                    cmd.Parameters.AddWithValue("idContatto", contatto.CodiceContatto)
                                    cmd.Parameters.AddWithValue("PEC_DATA_INSERIMENTO", contatto.DataInserimento)
                                    AddCommonParametersContatti(cmd, contatto)
                                    cmd.ExecuteNonQuery()
                                    cmd.Parameters.Clear()
                                End If
                            End If
                        Next
                    End If
                    'FINE T_PAZ_EPISODI_CONTATTI

                    'UPDATE T_PAZ_DIARIA
                    If (episodio.Diaria.Count > 0) Then
                        For Each diaria As Diaria In episodio.Diaria.Where(Function(x)
                                                                               Return x.CodiceDiaria = 0
                                                                           End Function)
                            InsertDiariaModuloSorveglianza(diaria, episodio.Testata.IdEpisodio, cmd)
                            cmd.Parameters.Clear()
                        Next


                        For Each diaria As Diaria In episodio.Diaria.Where(Function(x)
                                                                               Return x.CodiceDiaria > 0
                                                                           End Function)
                            Dim queryUpdateDiaria As String = "UPDATE T_PAZ_EPISODI_DIARIA SET
                                                            PED_USL_CODICE_RILEVAZIONE = :PED_USL_CODICE_RILEVAZIONE,
                                                            PED_DATA_RILEVAZIONE = :PED_DATA_RILEVAZIONE,
                                                            PED_FLAG_ASINTOMATICO = :PED_FLAG_ASINTOMATICO,
                                                            PED_NOTE = :PED_NOTE,
                                                            PED_UTE_ID_MODIFICA = :PED_UTE_ID_MODIFICA,
                                                            PED_RISPOSTA_TELEFONO = :PED_RISPOSTA_TELEFONO
                                                                WHERE PED_ID = :idDiaria"

                            If diaria.UtenteModifica.HasValue Then
                                cmd.Parameters.AddWithValue("PED_UTE_ID_MODIFICA", diaria.UtenteModifica.Value)
                            Else
                                cmd.Parameters.AddWithValue("PED_UTE_ID_MODIFICA", diaria.UtenteInserimento)
                            End If

                            cmd.CommandText = queryUpdateDiaria
                            cmd.Parameters.AddWithValue("idDiaria", diaria.CodiceDiaria)
                            AddCommonParametersDiaria(cmd, diaria)
                            cmd.ExecuteNonQuery()
                            cmd.Parameters.Clear()

                            'UPDATE T_PAZ_EPISODI_DIARIA_SINTOMI
                            AggiornaSintomiDiaria(diaria.CodiceDiaria, diaria.Sintomi.Select(Function(x As Sintomo)
                                                                                                 Return x.CodiceSintomo
                                                                                             End Function).ToList(), cmd, diaria.UtenteInserimento)

                            'FINE T_PAZ_EPISODI_DIARIA_SINTOMI
                        Next
                    End If
                    'FINE T_PAZ_DIARIA

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try
            End Using
        End Sub

        Public Sub DeleteRicoveri(ricoveriDelete As List(Of Long)) Implements IRilevazioniCovid19Provider.DeleteRicoveri
            Using cmd As OracleCommand = New OracleCommand
                cmd.Connection = Connection
                Dim ownConnection As Boolean = False
                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.CommandText = "DELETE FROM T_PAZ_EPISODI_RICOVERI WHERE PER_ID = :idRicovero"

                    For Each idRicovero As Long In ricoveriDelete
                        cmd.Parameters.AddWithValue("idRicovero", idRicovero)
                        cmd.ExecuteNonQuery()
                        cmd.Parameters.Clear()
                    Next
                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try
            End Using
        End Sub

        Public Sub DeleteTamponi(tamponiDelete As List(Of Long)) Implements IRilevazioniCovid19Provider.DeleteTamponi
            Using cmd As OracleCommand = New OracleCommand
                cmd.Connection = Connection
                Dim ownConnection As Boolean = False
                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.CommandText = "DELETE FROM T_PAZ_EPISODI_TAMPONI WHERE PET_ID = :idTampone"

                    For Each idTampone As Long In tamponiDelete
                        cmd.Parameters.AddWithValue("idTampone", idTampone)
                        cmd.ExecuteNonQuery()
                        cmd.Parameters.Clear()
                    Next
                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try
            End Using
        End Sub

        Public Sub DeleteDiaria(diariaDelete As List(Of Long)) Implements IRilevazioniCovid19Provider.DeleteDiaria
            Using cmd As OracleCommand = New OracleCommand
                cmd.Connection = Connection
                Dim ownConnection As Boolean = False
                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    For Each idDiaria As Long In diariaDelete
                        'a fronte di una eliminazione di una diaria, vengono prima eliminati i record della T_PAZ_EPISODI_DIARIA_SINTOMI, che rappresentano i sintomi ad essa associati.
                        cmd.CommandText = "DELETE FROM T_PAZ_EPISODI_DIARIA_SINTOMI WHERE PDS_PED_ID = :idDiaria"
                        cmd.Parameters.AddWithValue("idDiaria", idDiaria)
                        cmd.ExecuteNonQuery()
                        'in seguito viene eliminata la diaria
                        cmd.CommandText = "DELETE FROM T_PAZ_EPISODI_DIARIA WHERE PED_ID = :idDiaria"
                        cmd.ExecuteNonQuery()
                        cmd.Parameters.Clear()
                    Next
                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try
            End Using
        End Sub

        Public Sub DeleteContatti(contattiDelete As List(Of Long)) Implements IRilevazioniCovid19Provider.DeleteContatti
            Using cmd As OracleCommand = New OracleCommand
                cmd.Connection = Connection
                Dim ownConnection As Boolean = False
                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.CommandText = "DELETE FROM T_PAZ_EPISODI_CONTATTI WHERE PEC_ID = :idContatto"

                    For Each idContatto As Long In contattiDelete
                        cmd.Parameters.AddWithValue("idContatto", idContatto)
                        cmd.ExecuteNonQuery()
                        cmd.Parameters.Clear()
                    Next
                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try
            End Using
        End Sub

        Public Function RecuperaPazientiCreatoriDelContatto(codicePazienteEpisodio As Long) As List(Of DatiPazienteCreatoreContatto) Implements IRilevazioniCovid19Provider.RecuperaPazientiCreatoriDelContatto
            Dim result As List(Of DatiPazienteCreatoreContatto) = New List(Of DatiPazienteCreatoreContatto)
            Using cmd As OracleCommand = New OracleCommand
                cmd.Connection = Connection
                Dim ownConnection As Boolean = False
                Try
                    ownConnection = ConditionalOpenConnection(cmd)
                    Dim query As String = "select 
                                               PAZ_CODICE as Codice,
                                               PAZ_COGNOME Cognome,
                                               PAZ_NOME Nome,
                                               PAZ_CODICE_FISCALE Cf,
                                               PAZ_DATA_NASCITA DataNascita,
                                               COM_DESCRIZIONE ComuneResidenza,
                                               PEC_DATA_INSERIMENTO DataInserimentoContatto
                                                    FROM T_PAZ_EPISODI_CONTATTI
                                                    inner join T_PAZ_EPISODI on PEC_PES_ID = PES_ID
                                                                inner join T_PAZ_PAZIENTI on PES_PAZ_CODICE = PAZ_CODICE
                                                                left outer join T_ANA_COMUNI on PAZ_COM_CODICE_RESIDENZA = COM_CODICE
                                                    WHERE PEC_PAZ_CODICE = :codicePaziente"

                    cmd.Parameters.AddWithValue("codicePaziente", codicePazienteEpisodio)
                    cmd.CommandText = query

                    Using dr As OracleDataReader = cmd.ExecuteReader()
                        If Not dr Is Nothing Then
                            Dim Codice As Integer = dr.GetOrdinal("Codice")
                            Dim Cognome As Integer = dr.GetOrdinal("Cognome")
                            Dim Nome As Integer = dr.GetOrdinal("Nome")
                            Dim Cf As Integer = dr.GetOrdinal("Cf")
                            Dim DataNascita As Integer = dr.GetOrdinal("DataNascita")
                            Dim ComuneResidenza As Integer = dr.GetOrdinal("ComuneResidenza")
                            Dim DataInserimentoContatto As Integer = dr.GetOrdinal("DataInserimentoContatto")

                            While dr.Read()
                                Dim creatoreContatto As DatiPazienteCreatoreContatto = New DatiPazienteCreatoreContatto()
                                creatoreContatto.Cognome = dr.GetStringOrDefault(Cognome)
                                creatoreContatto.Nome = dr.GetStringOrDefault(Nome)
                                creatoreContatto.DataNascita = dr.GetNullableDateTimeOrDefault(DataNascita)
                                creatoreContatto.Cf = dr.GetStringOrDefault(Cf)
                                creatoreContatto.ComuneResidenza = dr.GetStringOrDefault(ComuneResidenza)
                                creatoreContatto.DataInserimentoContatto = dr.GetNullableDateTimeOrDefault(DataInserimentoContatto)
                                creatoreContatto.CodicePaziente = dr.GetInt64(Codice)
                                result.Add(creatoreContatto)
                            End While
                        End If
                    End Using
                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try
            End Using
            Return result
        End Function

        Public Sub DeleteTags(idEpisodio As Long) Implements IRilevazioniCovid19Provider.DeleteTags
            Using cmd As OracleCommand = New OracleCommand
                cmd.Connection = Connection
                Dim ownConnection As Boolean = False
                Try
                    ownConnection = ConditionalOpenConnection(cmd)
                    cmd.CommandText = "DELETE FROM T_ANA_LINK_EPISODI_TAG WHERE LET_PES_ID = :idEpisodio"
                    cmd.Parameters.AddWithValue("idEpisodio", idEpisodio)
                    cmd.ExecuteNonQuery()
                    cmd.Parameters.Clear()
                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try
            End Using
        End Sub

        Public Sub DeleteContattiRiferimento(idEpisodio As Long) Implements IRilevazioniCovid19Provider.DeleteContattiRiferimento
            Using cmd As OracleCommand = New OracleCommand
                cmd.Connection = Connection
                Dim ownConnection As Boolean = False
                Try
                    ownConnection = ConditionalOpenConnection(cmd)
                    cmd.CommandText = QueryDeleteContattiRiferimento
                    cmd.Parameters.AddWithValue("idEpisodioRiferimento", idEpisodio)
                    cmd.ExecuteNonQuery()
                    cmd.Parameters.Clear()
                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try
            End Using
        End Sub

        Private Function CreateLob(cmd As OracleCommand, lobtype As OracleType) As OracleLob
            cmd.CommandText = "DECLARE A CLOB; " +
                                 "BEGIN " +
                                    "DBMS_LOB.CREATETEMPORARY(A, FALSE); " +
                                    ":LOC := A; " +
                                 "END;"

            Dim p As OracleParameter = cmd.Parameters.Add("LOC", lobtype)
            p.Direction = ParameterDirection.Output

            cmd.ExecuteNonQuery()

            Return CType(p.Value, OracleLob)

        End Function

        Public Sub DeleteEpisodio(episodio As EpisodioPaziente) Implements IRilevazioniCovid19Provider.DeleteEpisodio
            Using cmd As OracleCommand = New OracleCommand
                Dim ownConnection As Boolean = False
                ownConnection = ConditionalOpenConnection(cmd)
                Using tx As OracleTransaction = Connection.BeginTransaction()
                    cmd.Connection = Connection
                    cmd.Transaction = tx
                    Try
                        Dim serializer As New JavaScriptSerializer

                        'la configurazione dell'episodio viene salvata in formato Json, in un campo della T_PAZ_EPISODI_ELIMINATI
                        Dim configEpisodio = New With {
                            Key .DatoreLavoro = episodio.DatoreLavoro,
                            Key .Clinica = episodio.Clinica,
                            Key .Contatti = episodio.Contatti,
                            Key .Ricoveri = episodio.Ricoveri,
                            Key .Tamponi = episodio.Tamponi,
                            Key .Diaria = episodio.Diaria,
                            Key .Tags = episodio.Tags
                        }
                        Dim tmpLob As OracleLob = CreateLob(cmd, OracleType.Clob)
                        cmd.Parameters.Clear()

                        cmd.Parameters.AddWithValue("uteEliminazione", setValue(episodio.Dettaglio.UtenteEliminazione))
                        cmd.Parameters.AddWithValue("uslEliminazione", setValue(episodio.Dettaglio.UslEliminazione))
                        cmd.Parameters.AddWithValue("dataEliminazione", setValue(episodio.Dettaglio.DataEliminazione))

                        Dim ser As String = serializer.Serialize(configEpisodio)

                        Dim p As OracleParameter = cmd.Parameters.Add("dettaglio", OracleType.Clob)
                        'p.DbType = DbType.Object
                        cmd.CommandText = QueryInsertEpisodioEliminato

                        tmpLob.BeginBatch(OracleLobOpenMode.ReadWrite)
                        Using str As New MemoryStream()
                            Using w As New StreamWriter(str, UTF8Encoding.Unicode)
                                w.Write(ser)
                                w.Flush()
                                str.Position = 0
                                str.CopyTo(tmpLob)
                            End Using
                        End Using
                        tmpLob.EndBatch()
                        p.Value = tmpLob

                        'cmd.Parameters.Add(p)

                        cmd.Parameters.AddWithValue("idEpisodio", episodio.Testata.IdEpisodio)
                        cmd.ExecuteNonQuery()
                        cmd.Parameters.Clear()

                        'dopo aver inserito l'episodio nella T_PAZ_EPISODI_ELIMINATI, lo cancello fisicamente dalla T_PAZ_EPISODI insieme ai record che referenziano quell'episodio (datore di lavoro, contatti, diarie ...)
                        cmd.Parameters.AddWithValue("idEpisodio", episodio.Testata.IdEpisodio)

                        'eliminazione datore lavoro
                        cmd.CommandText = "DELETE FROM T_PAZ_EPISODI_DATORE_LAVORO WHERE PEL_PES_ID = :idEpisodio"
                        cmd.ExecuteNonQuery()

                        'eliminazione clinica
                        cmd.CommandText = "DELETE FROM T_PAZ_EPISODI_CLINICA WHERE PCL_PES_ID = :idEpisodio"
                        cmd.ExecuteNonQuery()

                        'eliminazione tag collegati
                        cmd.CommandText = "DELETE FROM T_ANA_LINK_EPISODI_TAG WHERE LET_PES_ID = :idEpisodio"
                        cmd.ExecuteNonQuery()

                        'eliminazione ricoveri
                        cmd.CommandText = "DELETE FROM T_PAZ_EPISODI_RICOVERI WHERE PER_PES_ID = :idEpisodio"
                        cmd.ExecuteNonQuery()

                        'eliminazione tamponi
                        cmd.CommandText = "DELETE FROM T_PAZ_EPISODI_TAMPONI WHERE PET_PES_ID = :idEpisodio"
                        cmd.ExecuteNonQuery()

                        'eliminazione contatti
                        cmd.CommandText = "DELETE FROM T_PAZ_EPISODI_CONTATTI WHERE PEC_PES_ID = :idEpisodio"
                        cmd.ExecuteNonQuery()

                        'elimino tutti i sintomi di tutte le diarie dell'episodio.
                        cmd.CommandText = "DELETE from T_PAZ_EPISODI_DIARIA_SINTOMI
                                              where PDS_PED_ID IN (
                                                                  select PED_ID FROM T_PAZ_EPISODI_DIARIA WHERE PED_PES_ID = :idEpisodio
                                                                  )"
                        cmd.ExecuteNonQuery()

                        'elimino le diarie
                        cmd.CommandText = "DELETE FROM T_PAZ_EPISODI_DIARIA WHERE PED_PES_ID = :idEpisodio"
                        cmd.ExecuteNonQuery()

                        'elimino l'episodio
                        cmd.CommandText = "DELETE FROM T_PAZ_EPISODI WHERE PES_ID = :idEpisodio"
                        cmd.ExecuteNonQuery()
                        tx.Commit()
                    Catch e As Exception
                        tx.Rollback()
                        Throw e
                    Finally
                        ConditionalCloseConnection(ownConnection)
                    End Try
                End Using

            End Using
        End Sub

        Public Sub MarcaPazienteInvioCredenzialiApp(pazCodice As Long) Implements IRilevazioniCovid19Provider.MarcaPazienteInvioCredenzialiApp
            Using cmd As OracleCommand = New OracleCommand
                cmd.Connection = Connection
                Dim ownConnection As Boolean = False
                Try
                    ownConnection = ConditionalOpenConnection(cmd)
                    cmd.CommandText = QueryInvioCredenzialiApp
                    cmd.Parameters.AddWithValue("pazCodice", pazCodice)
                    cmd.ExecuteNonQuery()
                    cmd.Parameters.Clear()
                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try
            End Using
        End Sub

        Public Function GetUltimaDiaria(codicePaziente As Long) As Diaria Implements IRilevazioniCovid19Provider.GetUltimaDiaria

            Using cmd As New OracleCommand()

                cmd.Connection = Connection

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.CommandText = "select PED_ID, PED_NOTE, PED_DATA_RILEVAZIONE, PED_USL_CODICE_RILEVAZIONE, 
                                         PED_UTE_ID_INSERIMENTO, PED_USL_CODICE_INSERIMENTO, PED_DATA_INSERIMENTO, 
                                         PED_UTE_ID_MODIFICA, PED_DATA_MODIFICA,
                                         PED_RISPOSTA_TELEFONO, PED_PES_ID, ASI_CODICE, ASI_DESCRIZIONE
                                       from T_PAZ_EPISODI
                                       join T_PAZ_EPISODI_DIARIA on PES_ID = PED_PES_ID
                                       left join T_PAZ_EPISODI_DIARIA_SINTOMI on PDS_PED_ID = PED_ID
                                       left join T_ANA_SINTOMI on PDS_ASI_CODICE = ASI_CODICE
                                       where PES_PAZ_CODICE = :codicePaziente"

                    cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

                    Using reader As OracleDataReader = cmd.ExecuteReader()

                        Dim listaTmp As New List(Of FlatDiaria)()

                        Dim ped_id As Integer = reader.GetOrdinal("PED_ID")
                        Dim ped_note As Integer = reader.GetOrdinal("PED_NOTE")
                        Dim ped_data_rilevazione As Integer = reader.GetOrdinal("PED_DATA_RILEVAZIONE")
                        Dim ped_usl_codice_rilevazione As Integer = reader.GetOrdinal("PED_USL_CODICE_RILEVAZIONE")
                        Dim ped_ute_id_inserimento As Integer = reader.GetOrdinal("PED_UTE_ID_INSERIMENTO")
                        Dim ped_usl_codice_inserimento As Integer = reader.GetOrdinal("PED_USL_CODICE_INSERIMENTO")
                        Dim ped_data_inserimento As Integer = reader.GetOrdinal("PED_DATA_INSERIMENTO")
                        Dim ped_ute_id_modifica As Integer = reader.GetOrdinal("PED_UTE_ID_MODIFICA")
                        Dim ped_data_modifica As Integer = reader.GetOrdinal("PED_DATA_MODIFICA")
                        Dim ped_risposta_telefono As Integer = reader.GetOrdinal("PED_RISPOSTA_TELEFONO")
                        Dim ped_pes_id As Integer = reader.GetOrdinal("PED_PES_ID")
                        Dim asi_codice As Integer = reader.GetOrdinal("ASI_CODICE")
                        Dim asi_descrizione As Integer = reader.GetOrdinal("ASI_DESCRIZIONE")

                        While reader.Read()

                            Dim tmp As New FlatDiaria()

                            tmp.CodiceDiaria = reader.GetInt64(ped_id)
                            tmp.Note = reader.GetStringOrDefault(ped_note)
                            tmp.DataRilevazione = reader.GetDateTime(ped_data_rilevazione)
                            tmp.CodiceRilevazioneUsl = reader.GetStringOrDefault(ped_usl_codice_rilevazione)
                            tmp.UtenteInserimento = reader.GetInt64OrDefault(ped_ute_id_inserimento)
                            tmp.CodiceInserimentoUsl = reader.GetStringOrDefault(ped_usl_codice_inserimento)
                            tmp.DataInserimento = reader.GetDateTimeOrDefault(ped_data_inserimento)
                            tmp.UtenteModifica = reader.GetNullableInt64OrDefault(ped_ute_id_modifica)
                            tmp.DataModifica = reader.GetNullableDateTimeOrDefault(ped_data_modifica)
                            tmp.RispostaTelefono = reader.GetBooleanOrDefault(ped_risposta_telefono)
                            tmp.CodiceEpisodio = reader.GetInt64(ped_pes_id)
                            tmp.CodiceSintomo = reader.GetInt32OrDefault(asi_codice)
                            tmp.DescrizioneSintomo = reader.GetStringOrDefault(asi_descrizione)

                            listaTmp.Add(tmp)

                        End While

                        Dim ritorno As List(Of Diaria) = listaTmp.GroupBy(Function(x As FlatDiaria) x.CodiceDiaria).Select(Function(x)
                                                                                                                               Dim diaria As New Diaria()
                                                                                                                               diaria.CodiceDiaria = x.Key
                                                                                                                               diaria.Note = x.First().Note
                                                                                                                               diaria.DataRilevazione = x.First().DataRilevazione
                                                                                                                               diaria.CodiceEpisodio = x.First().CodiceEpisodio
                                                                                                                               diaria.CodiceRilevazioneUsl = x.First().CodiceRilevazioneUsl
                                                                                                                               diaria.UtenteInserimento = x.First().UtenteInserimento
                                                                                                                               diaria.DataInserimento = x.First().DataInserimento
                                                                                                                               diaria.UtenteModifica = x.First().UtenteModifica
                                                                                                                               diaria.DataModifica = x.First().DataModifica
                                                                                                                               diaria.RispostaTelefono = x.First().RispostaTelefono
                                                                                                                               diaria.Asintomatico = Not x.Any()
                                                                                                                               diaria.Sintomi = x.Select(Function(s)
                                                                                                                                                             Return New Sintomo With {
                                                                                                                                                                  .CodiceSintomo = s.CodiceSintomo,
                                                                                                                                                                  .Descrizione = s.DescrizioneSintomo
                                                                                                                                                                }
                                                                                                                                                         End Function).ToList()
                                                                                                                               Return diaria
                                                                                                                           End Function).ToList()
                        Return ritorno.OrderByDescending(Function(x) x.DataRilevazione).ThenByDescending(Function(x) x.CodiceDiaria).FirstOrDefault()
                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

        End Function

        Public Function GetSituazioneDiaria(codiceDiaria As Long) As SituazioneDiaria Implements IRilevazioniCovid19Provider.GetSituazioneDiaria
            Using cmd As OracleCommand = New OracleCommand
                cmd.Connection = Connection
                Dim ownConnection As Boolean = False
                Try
                    ownConnection = ConditionalOpenConnection(cmd)
                    Dim ritorno As New SituazioneDiaria With {
                        .sintomi = New List(Of SituazioneDiaria.SituazioneSintomo)
                    }
                    cmd.CommandText = "select 
                                            PED_ID,
                                            PED_NOTE,
                                            PDS_ID,
                                            PDS_ASI_CODICE
                                             from T_PAZ_EPISODI_DIARIA
                                             left join T_PAZ_EPISODI_DIARIA_SINTOMI on PDS_PED_ID = PED_ID
                                            where PED_ID = :codiceDiaria"
                    cmd.Parameters.AddWithValue("codiceDiaria", codiceDiaria)

                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        Dim iCodiceDiaria As Integer = reader.GetOrdinal("PED_ID")
                        Dim iNote As Integer = reader.GetOrdinal("PED_NOTE")
                        Dim iCodiceLink As Integer = reader.GetOrdinal("PDS_ID")
                        Dim iCodiceSintomo As Integer = reader.GetOrdinal("PDS_ASI_CODICE")
                        While reader.Read()
                            ritorno.codiceDiaria = reader.GetInt64(iCodiceDiaria)
                            ritorno.note = reader.GetStringOrDefault(iNote)
                            Dim codiceLink As Long = reader.GetInt64OrDefault(iCodiceLink)
                            If codiceLink > 0 Then
                                ritorno.sintomi.Add(New SituazioneDiaria.SituazioneSintomo With {
                                                    .codiceLink = codiceLink,
                                                    .codiceSintomo = reader.GetInt32(iCodiceSintomo)
                                                    })
                            End If
                        End While
                    End Using

                    Return ritorno
                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try
            End Using
        End Function

        Public Sub AggiornaDiaria(codiceDiaria As Long, note As String, nuoviSintomi As IEnumerable(Of Integer), linkSintomiDaEliminare As List(Of Long), idUtente As Long) Implements IRilevazioniCovid19Provider.AggiornaDiaria

            Using cmd As New OracleCommand()

                cmd.Connection = Connection

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)
                    Dim presenzaSintomi As Boolean = nuoviSintomi IsNot Nothing And nuoviSintomi.Any()
                    Dim now As DateTime = DateTime.Now

                    If linkSintomiDaEliminare.Any() Then
                        Dim p As String
                        'elimino i sintomi
                        Dim command As New StringBuilder("delete from T_PAZ_EPISODI_DIARIA_SINTOMI where PDS_ID IN (")
                        If linkSintomiDaEliminare.Skip(1).Any() Then
                            For i As Integer = 0 To linkSintomiDaEliminare.Count() - 2
                                p = String.Format("S_{0}", i)
                                command.Append(String.Format(":{0}, ", p))
                                cmd.Parameters.AddWithValue(p, linkSintomiDaEliminare.Item(i))
                            Next
                        End If
                        p = String.Format("S_{0}", linkSintomiDaEliminare.Count + 1)
                        command.Append(String.Format(":{0})", p))
                        cmd.Parameters.AddWithValue(p, linkSintomiDaEliminare.Last())
                        cmd.CommandText = command.ToString()
                        cmd.ExecuteNonQuery()
                        cmd.Parameters.Clear()
                    End If

                    If presenzaSintomi Then

                        cmd.CommandText = "insert into T_PAZ_EPISODI_DIARIA_SINTOMI (PDS_PED_ID, PDS_ASI_CODICE, PDS_DATA_INSERIMENTO) VALUES (:codiceDiaria, :codiceSintomo, :data)"

                        For Each sintomo As Integer In nuoviSintomi
                            cmd.Parameters.AddWithValue("codiceDiaria", codiceDiaria)
                            cmd.Parameters.AddWithValue("codiceSintomo", sintomo)
                            cmd.Parameters.AddWithValue("data", now)
                            cmd.ExecuteNonQuery()
                            cmd.Parameters.Clear()
                        Next

                    End If

                    cmd.CommandText = "update T_PAZ_EPISODI_DIARIA set PED_NOTE = :note, PED_UTE_ID_MODIFICA = :uteModifica, 
                                            PED_DATA_MODIFICA = :dataModifica, PED_FLAG_ASINTOMATICO = :asintomatico where PED_ID = :codice"
                    cmd.Parameters.AddWithValue("codice", codiceDiaria)
                    cmd.Parameters.AddWithValue("note", setValue(note))
                    cmd.Parameters.AddWithValue("uteModifica", idUtente)
                    cmd.Parameters.AddWithValue("dataModifica", now)
                    cmd.Parameters.AddWithValue("asintomatico", BoolToSN(Not presenzaSintomi))

                    cmd.ExecuteNonQuery()

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

        End Sub

        'Diaria "auto-inserita" dal paziente tramite app OnVac. 
        Public Sub InsertDiariaApp(codiceEpisodio As Long, codicePaziente As Long, sintomi As List(Of Integer), note As String, idUtente As Long) Implements IRilevazioniCovid19Provider.InsertDiariaApp
            Using cmd As New OracleCommand()
                cmd.Connection = Connection
                Dim ownConnection As Boolean = False
                Try
                    ownConnection = ConditionalOpenConnection(cmd)
                    Dim now As DateTime = DateTime.Now
                    Dim presenzaSintomi As Boolean = sintomi IsNot Nothing And sintomi.Any()
                    cmd.CommandText = "select SEQ_PAZ_EPISODI_DIARIA.nextval from dual"
                    Dim newIdDiaria As Long = cmd.ExecuteScalar()

                    cmd.CommandText = "INSERT INTO T_PAZ_EPISODI_DIARIA (
                                            PED_ID,
                                            PED_PES_ID,
                                            PED_DATA_RILEVAZIONE,
                                            PED_FLAG_ASINTOMATICO,
                                            PED_NOTE,
                                            PED_UTE_ID_INSERIMENTO,
                                            PED_DATA_INSERIMENTO,
                                            PED_RISPOSTA_TELEFONO
                                            )
                                            VALUES (
                                            :PED_ID,
                                            :PED_PES_ID,
                                            :PED_DATA_RILEVAZIONE,
                                            :PED_FLAG_ASINTOMATICO,
                                            :PED_NOTE,
                                            :PED_UTE_ID_INSERIMENTO,
                                            :PED_DATA_INSERIMENTO,
                                            :PED_RISPOSTA_TELEFONO
                                            )"
                    cmd.Parameters.AddWithValue("PED_ID", newIdDiaria)
                    cmd.Parameters.AddWithValue("PED_PES_ID", codiceEpisodio)
                    cmd.Parameters.AddWithValue("PED_DATA_RILEVAZIONE", now.Date)
                    cmd.Parameters.AddWithValue("PED_FLAG_ASINTOMATICO", BoolToSN(Not presenzaSintomi))
                    cmd.Parameters.AddWithValue("PED_NOTE", setValue(note))
                    cmd.Parameters.AddWithValue("PED_UTE_ID_INSERIMENTO", idUtente)
                    cmd.Parameters.AddWithValue("PED_DATA_INSERIMENTO", now)
                    cmd.Parameters.AddWithValue("PED_RISPOSTA_TELEFONO", "S")

                    cmd.ExecuteNonQuery()
                    cmd.Parameters.Clear()

                    If presenzaSintomi Then
                        cmd.CommandText = "insert into T_PAZ_EPISODI_DIARIA_SINTOMI (PDS_PED_ID, PDS_ASI_CODICE, PDS_DATA_INSERIMENTO) VALUES (:codiceDiaria, :codiceSintomo, :data)"
                        For Each sintomo As Integer In sintomi
                            cmd.Parameters.AddWithValue("codiceDiaria", newIdDiaria)
                            cmd.Parameters.AddWithValue("codiceSintomo", sintomo)
                            cmd.Parameters.AddWithValue("data", now)
                            cmd.ExecuteNonQuery()
                            cmd.Parameters.Clear()
                        Next
                    End If
                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try
            End Using
        End Sub

        ''' <summary>
        ''' Imposta il flag di lettura dell'informativa per il paziente specificato
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="informativaLetta"></param>
        ''' <returns></returns>
        Public Function UpdateFlagLetturaInformativaCovid(codicePaziente As Long, informativaLetta As Boolean) As Integer Implements IRilevazioniCovid19Provider.UpdateFlagLetturaInformativaCovid

            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleCommand("UPDATE T_PAZ_PAZIENTI SET PAZ_CONFERMA_INFORMATIVA_COVID = :PAZ_CONFERMA_INFORMATIVA_COVID WHERE PAZ_CODICE = :PAZ_CODICE", Connection)

                    cmd.Parameters.AddWithValueOrDefault("PAZ_CODICE", codicePaziente)
                    cmd.Parameters.AddWithValueOrDefault("PAZ_CONFERMA_INFORMATIVA_COVID", If(informativaLetta, "S", "N"))

                    ownConnection = ConditionalOpenConnection(cmd)

                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function
        Public Function GetSettingScolastisco() As List(Of SettingScolastico) Implements IRilevazioniCovid19Provider.GetSettingScolastisco

            Dim result As New List(Of SettingScolastico)()
            Dim ownConnection As Boolean = False
            Dim query = "Select SPS_ID, SPS_DESCRIZIONE " +
                        "From T_TIPO_SETTING_P_SCOLASTICO "

            Try
                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()

                        Dim SPS_ID As Integer = _context.GetOrdinal("SPS_ID")
                        Dim SPS_DESCRIZIONE As Integer = _context.GetOrdinal("SPS_DESCRIZIONE")

                        While _context.Read()
                            Dim setting As New SettingScolastico()

                            setting.Id = _context.GetInt64OrDefault(SPS_ID)
                            setting.Descrizione = _context.GetStringOrDefault(SPS_DESCRIZIONE)

                            result.Add(setting)
                        End While

                    End Using
                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return result

        End Function
        Public Function GetTipoScolastico() As List(Of TipoScolastico) Implements IRilevazioniCovid19Provider.GetTipoScolastico

            Dim result As New List(Of TipoScolastico)()
            Dim ownConnection As Boolean = False
            Dim query = "Select TPS_ID, TPS_DESCRIZIONE " +
                        "From T_TIPO_PERSONALE_SCOLASTICO "

            Try
                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()

                        Dim TPS_ID As Integer = _context.GetOrdinal("TPS_ID")
                        Dim TPS_DESCRIZIONE As Integer = _context.GetOrdinal("TPS_DESCRIZIONE")

                        While _context.Read()
                            Dim tipoS As New TipoScolastico()

                            tipoS.Id = _context.GetInt64OrDefault(TPS_ID)
                            tipoS.Descrizione = _context.GetStringOrDefault(TPS_DESCRIZIONE)

                            result.Add(tipoS)
                        End While

                    End Using
                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return result

        End Function
        Public Function GetFlagLetturaInformativaCovid(codicePaziente As Long) As String Implements IRilevazioniCovid19Provider.GetFlagLetturaInformativaCovid

            Dim informativaLetta As String = String.Empty

            Using cmd As New OracleCommand()

                cmd.Connection = Connection

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.CommandText = "SELECT PAZ_CONFERMA_INFORMATIVA_COVID FROM T_PAZ_PAZIENTI WHERE PAZ_CODICE = :PAZ_CODICE"

                    cmd.Parameters.AddWithValue("PAZ_CODICE", codicePaziente)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                        informativaLetta = obj.ToString()
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return informativaLetta

        End Function

        ''' <summary>
        ''' Restituisce tutti i tamponi dell'episodio specificato, con i soli dati utili al report CertificatoNegativizzazioneCOVID19.rpt
        ''' </summary>
        ''' <param name="idEpisodio"></param>
        ''' <returns></returns>
        Public Function GetTamponiCertificatoNeg(idEpisodio As Long) As List(Of TamponeCertificatoNeg) Implements IRilevazioniCovid19Provider.GetTamponiCertificatoNeg

            Dim list As New List(Of TamponeCertificatoNeg)()

            Using cmd As New OracleCommand()

                cmd.Connection = Connection

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.CommandText = "SELECT PET_ID, PET_DATA_TAMPONE, PET_ESITO FROM T_PAZ_EPISODI_TAMPONI WHERE PET_PES_ID = :PET_PES_ID"

                    cmd.Parameters.AddWithValue("PET_PES_ID", idEpisodio)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim pet_id As Integer = idr.GetOrdinal("PET_ID")
                            Dim pet_data_tampone As Integer = idr.GetOrdinal("PET_DATA_TAMPONE")
                            Dim pet_esito As Integer = idr.GetOrdinal("PET_ESITO")

                            While idr.Read()

                                Dim item As New TamponeCertificatoNeg()

                                item.CodiceTampone = idr.GetInt64(pet_id)
                                item.DataTampone = idr.GetNullableDateTimeOrDefault(pet_data_tampone)
                                item.CodiceEsito = idr.GetStringOrDefault(pet_esito)

                                list.Add(item)

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

#Region "Private"
        Private Function AggiornaSintomiDiaria(codiceDiaria As Long, sintomi As IEnumerable(Of Integer), cmd As OracleCommand, codiceUtente As Long)

            Dim sintomiEsistenti As New List(Of Integer)
            cmd.CommandText = "select PDS_ID as CodiceLink, PDS_ASI_CODICE as CodiceSintomo from T_PAZ_EPISODI_DIARIA_SINTOMI where PDS_PED_ID = :idDiaria"
            cmd.Parameters.AddWithValue("idDiaria", codiceDiaria)
            Using reader As OracleDataReader = cmd.ExecuteReader()
                Dim pos As Integer = reader.GetOrdinal("CodiceSintomo")
                While reader.Read()
                    sintomiEsistenti.Add(reader.GetInt32(pos))
                End While
            End Using

            Dim sintomiDaAggiungere As IEnumerable(Of Integer) = sintomi.Where(Function(x As Integer)
                                                                                   Return Not sintomiEsistenti.Contains(x)
                                                                               End Function).ToList()

            Dim sintomiDaEliminare As List(Of Integer) = sintomiEsistenti.Where(Function(x As Integer)
                                                                                    Return Not sintomi.Contains(x)
                                                                                End Function).ToList()

            If sintomiDaAggiungere.Any() Then
                cmd.Parameters.Clear()
                cmd.CommandText = "insert into T_PAZ_EPISODI_DIARIA_SINTOMI(PDS_PED_ID, PDS_ASI_CODICE, PDS_DATA_INSERIMENTO, PDS_UTE_ID_INSERIMENTO) values(:idDiaria, :idSintomo, :oggi, :PDS_UTE_ID_INSERIMENTO)"
                For Each sintomo As Integer In sintomiDaAggiungere
                    cmd.Parameters.AddWithValue("idDiaria", codiceDiaria)
                    cmd.Parameters.AddWithValue("oggi", DateTime.Now)
                    cmd.Parameters.AddWithValue("idSintomo", sintomo)
                    cmd.Parameters.AddWithValue("PDS_UTE_ID_INSERIMENTO", codiceUtente)
                    cmd.ExecuteNonQuery()
                Next
            End If


            If sintomiDaEliminare.Any() Then
                cmd.Parameters.Clear()
                Dim command As New StringBuilder("delete from T_PAZ_EPISODI_DIARIA_SINTOMI where PDS_PED_ID = :idDiaria and PDS_ASI_CODICE IN(")
                cmd.Parameters.AddWithValue("idDiaria", codiceDiaria)
                For i As Integer = 0 To sintomiDaEliminare.Count - 2
                    Dim name As String = String.Format("S_{0}", i)
                    command.Append(String.Format(":{0}, ", name))
                    cmd.Parameters.AddWithValue(name, sintomiDaEliminare.Item(i))
                Next
                command.Append(String.Format(":S_{0})", Integer.MaxValue))
                cmd.Parameters.AddWithValue(String.Format("S_{0}", Integer.MaxValue), sintomiDaEliminare.Last())
                cmd.CommandText = command.ToString()
                cmd.ExecuteNonQuery()
            End If
            cmd.Parameters.Clear()

        End Function

        'crea l'episodio per il paziente indicato come contatto e ne restituisce l'id.
        Private Function CreaEpisodioContatto(cmd As OracleCommand, codicePazienteContatto As Long, dataInserimento As Date?, uslInserimento As String, uslRaccolta As String, utenteInserimento As Long,
                                      dataSegnalazione As Date?, telefono As String, tipoCaso As String, note As String) As Long
            cmd.Parameters.Clear()

            'insert episodio
            cmd.CommandText = "select SEQ_PAZ_EPISODI.nextval from dual"
            Dim newIdEpisodio As Long = cmd.ExecuteScalar()

            cmd.CommandText = QueryInsertEpisodioPerContatto
            cmd.Parameters.AddWithValue("PES_ID", newIdEpisodio)
            cmd.Parameters.AddWithValue("PES_PAZ_CODICE", setValue(codicePazienteContatto))
            cmd.Parameters.AddWithValue("PES_TIPO_CASO", setValue(tipoCaso))
            cmd.Parameters.AddWithValue("PES_DATA_INSERIMENTO", setValue(dataInserimento))
            cmd.Parameters.AddWithValue("PES_USL_CODICE_RACCOLTA", setValue(uslRaccolta))
            cmd.Parameters.AddWithValue("PES_USL_CODICE_INSERIMENTO", setValue(uslInserimento))
            cmd.Parameters.AddWithValue("PES_UTE_ID_INSERIMENTO", setValue(utenteInserimento))
            cmd.Parameters.AddWithValue("PES_DATA_SEGNALAZIONE", setValue(dataSegnalazione))
            cmd.Parameters.AddWithValue("PES_TELEFONO", setValue(telefono))
            cmd.Parameters.AddWithValue("PES_NOTE", setValue(note))
            cmd.ExecuteNonQuery()
            cmd.Parameters.Clear()

            'insert datore lavoro
            cmd.CommandText = QueryInserimentoDatoreLavoroEmpty
            cmd.Parameters.AddWithValue("PEL_PES_ID", newIdEpisodio)
            cmd.ExecuteNonQuery()
            cmd.Parameters.Clear()

            'insert clinica
            cmd.CommandText = QueryInserimentoClinicaEmpty
            cmd.Parameters.AddWithValue("PCL_PES_ID", newIdEpisodio)
            cmd.ExecuteNonQuery()
            cmd.Parameters.Clear()

            Return newIdEpisodio
        End Function

        'Metodo di inserimento diaria, chiamato al salvataggio di un episodio dal modulo sorveglianza
        Private Sub InsertDiariaModuloSorveglianza(diariaToAdd As Diaria, idEpisodio As Long, cmd As OracleCommand)
            Dim queryInsertDiaria As String = "INSERT INTO T_PAZ_EPISODI_DIARIA (
                                                            PED_ID,
                                                            PED_PES_ID,
                                                            PED_USL_CODICE_RILEVAZIONE,
                                                            PED_DATA_RILEVAZIONE,
                                                            PED_FLAG_ASINTOMATICO,
                                                            PED_NOTE,
                                                            PED_UTE_ID_INSERIMENTO,
                                                            PED_USL_CODICE_INSERIMENTO,
                                                            PED_DATA_INSERIMENTO,
                                                            PED_RISPOSTA_TELEFONO
                                                            )
                                                            VALUES (
                                                            :PED_ID,
                                                            :PED_PES_ID,
                                                            :PED_USL_CODICE_RILEVAZIONE,
                                                            :PED_DATA_RILEVAZIONE,
                                                            :PED_FLAG_ASINTOMATICO,
                                                            :PED_NOTE,
                                                            :PED_UTE_ID_INSERIMENTO,
                                                            :PED_USL_CODICE_INSERIMENTO,
                                                            :PED_DATA_INSERIMENTO,
                                                            :PED_RISPOSTA_TELEFONO
                                                            )"

            cmd.CommandText = "select SEQ_PAZ_EPISODI_DIARIA.nextval from dual"
            Dim newIdDiaria As Long = cmd.ExecuteScalar()
            cmd.CommandText = queryInsertDiaria

            cmd.Parameters.AddWithValue("PED_ID", newIdDiaria)
            cmd.Parameters.AddWithValue("PED_PES_ID", idEpisodio)
            cmd.Parameters.AddWithValue("PED_UTE_ID_INSERIMENTO", setValue(diariaToAdd.UtenteInserimento))
            cmd.Parameters.AddWithValue("PED_USL_CODICE_INSERIMENTO", setValue(diariaToAdd.CodiceInserimentoUsl))
            cmd.Parameters.AddWithValue("PED_DATA_INSERIMENTO", setValue(diariaToAdd.DataInserimento))
            AddCommonParametersDiaria(cmd, diariaToAdd)

            cmd.ExecuteNonQuery()
            cmd.Parameters.Clear()

            'INSERT SINTOMI
            If (diariaToAdd.Sintomi.Count > 0) Then
                Dim queryInsertSintomo As String = "INSERT INTO T_PAZ_EPISODI_DIARIA_SINTOMI (
                                                                            PDS_PED_ID,
                                                                            PDS_ASI_CODICE,
                                                                            PDS_NOTE,
                                                                            PDS_UTE_ID_INSERIMENTO,
                                                                            PDS_USL_CODICE_INSERIMENTO,
                                                                            PDS_DATA_INSERIMENTO
                                                                            )
                                                                            VALUES (
                                                                            :PDS_PED_ID,
                                                                            :PDS_ASI_CODICE,
                                                                            :PDS_NOTE,
                                                                            :PDS_UTE_ID_INSERIMENTO,
                                                                            :PDS_USL_CODICE_INSERIMENTO,
                                                                            :PDS_DATA_INSERIMENTO
                                                                            )"
                cmd.CommandText = queryInsertSintomo

                For k As Integer = 0 To diariaToAdd.Sintomi.Count - 1
                    Dim sintomoToAdd As Sintomo = diariaToAdd.Sintomi.ElementAt(k)
                    cmd.Parameters.AddWithValue("PDS_PED_ID", newIdDiaria)
                    cmd.Parameters.AddWithValue("PDS_UTE_ID_INSERIMENTO", setValue(sintomoToAdd.UtenteInserimento))
                    cmd.Parameters.AddWithValue("PDS_USL_CODICE_INSERIMENTO", setValue(sintomoToAdd.CodiceInserimentoUsl))
                    cmd.Parameters.AddWithValue("PDS_DATA_INSERIMENTO", setValue(sintomoToAdd.DataInserimento))
                    AddCommonParametersSintomi(cmd, sintomoToAdd)

                    cmd.ExecuteNonQuery()
                    cmd.Parameters.Clear()
                Next
            End If

        End Sub

        Private Function GetOrdinals(dataReader As IDataReader) As Dictionary(Of String, Integer)
            Dim result As Dictionary(Of String, Integer) = New Dictionary(Of String, Integer)
            If (dataReader IsNot Nothing And Not dataReader.IsClosed) Then
                For i As Integer = 0 To dataReader.FieldCount - 1
                    result.Add(dataReader.GetName(i), i)
                Next
            End If
            Return result
        End Function

        Private Sub InsertTags(cmd As OracleCommand, episodio As EpisodioPaziente)

            Dim queryInsertTag As String = "INSERT INTO T_ANA_TAG (
                                                            TAG_GRUPPO,
                                                            TAG_DESCRIZIONE,
                                                            TAG_COLORE
                                                            )
                                                            VALUES (
                                                            :TAG_GRUPPO,
                                                            :TAG_DESCRIZIONE,
                                                            :TAG_COLORE
                                                            ) RETURNING TAG_ID into :ritorno"

            Dim queryInsertLinkTag As String = "INSERT into T_ANA_LINK_EPISODI_TAG (
                                                            LET_PES_ID,
                                                            LET_TAG_ID
                                                            )
                                                            VALUES (
                                                            :LET_PES_ID,
                                                            :LET_TAG_ID
                                                            )"

            Dim idTag As Long = 0

            For Each item As TagTmp In episodio.Tags.Where(Function(x As TagTmp)
                                                               Return Not String.IsNullOrWhiteSpace(x.Descrizione)
                                                           End Function)
                If (Not Long.TryParse(item.Id, idTag)) Then
                    'se il parse della key fallisce, significa che è necessario creare il record Tag sul DB.
                    'cmd.CommandText = "select S_T_ANA_TAG.nextval from dual"
                    'idTag = cmd.ExecuteScalar()

                    cmd.Parameters.AddWithValue("TAG_GRUPPO", item.Gruppo)
                    cmd.Parameters.AddWithValue("TAG_DESCRIZIONE", setValue(item.Descrizione.ToUpper()))

                    Dim par As New OracleParameter("ritorno", DBNull.Value)
                    par.DbType = DbType.Int64
                    par.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(par)

                    'Colore attualmente non gestito
                    cmd.Parameters.AddWithValue("TAG_COLORE", DBNull.Value)
                    cmd.CommandText = queryInsertTag
                    cmd.ExecuteNonQuery()
                    cmd.Parameters.Clear()
                    idTag = Convert.ToInt64(par.Value)
                End If

                'insert link episodio tag
                cmd.Parameters.AddWithValue("LET_PES_ID", episodio.Testata.IdEpisodio)
                cmd.Parameters.AddWithValue("LET_TAG_ID", idTag)
                cmd.CommandText = queryInsertLinkTag
                cmd.ExecuteNonQuery()
                cmd.Parameters.Clear()
            Next
        End Sub

        'aggiungere i parametri comuni alle query di inserimento ed update della T_PAZ_EPISODI
        Private Sub AddCommonParametersEpisodio(cmd As IDbCommand, episodio As EpisodioPaziente)
            cmd.AddParameter("PES_MOTIVO_GENOTIP", episodio.Dettaglio.CodiceMotivoGenotipizzazione)
            cmd.AddParameter("PES_VARIANTE_VIRUS", episodio.Dettaglio.CodiceVariante)
            cmd.AddParameter("PES_DATA_DECESSO_COVID", episodio.Dettaglio.DataDecessoCovid)
            cmd.AddParameter("PES_PAZ_CODICE", episodio.Testata.CodicePaziente)
            cmd.AddParameter("PES_TIPO_CASO", episodio.Testata.CodiceTipoCaso)
            cmd.AddParameter("PES_ATC_CODICE", episodio.Testata.CodiceTipoContatto)
            cmd.AddParameter("PES_USL_CODICE_RACCOLTA", episodio.Dettaglio.CodiceRaccoltaUsl)
            cmd.AddParameter("PES_ASE_CODICE", episodio.Dettaglio.CodiceSegnalatore)
            cmd.AddParameter("PES_SEP_CODICE", episodio.Testata.CodiceStato)
            cmd.AddParameter("PES_FLAG_OPERATORE_SANITARIO", BoolToSN(episodio.Testata.IsOperatoreSanitario))
            cmd.AddParameter("PES_TIPO_OPERATORE_SANITARIO", episodio.Dettaglio.CodiceTipoOperatoreSanitario)
            cmd.AddParameter("PES_PATOLOGIE_CRONICHE", BoolToSN(episodio.Testata.HasPatologiaCroniche))
            cmd.AddParameter("PES_DATA_SEGNALAZIONE", episodio.Testata.DataSegnalazione)
            cmd.AddParameter("PES_COMUNE_ESPOSIZIONE", episodio.Dettaglio.EsposizioneComune)
            cmd.AddParameter("PES_DATA_INIZIO_ISOLAMENTO", episodio.Dettaglio.DataInizioIsolamento)
            cmd.AddParameter("PES_DATA_ULTIMO_CONTATTO", episodio.Dettaglio.DataUltimoContatto)
            cmd.AddParameter("PES_DATA_FINE_ISOLAMENTO", episodio.Dettaglio.DataFineIsolamento)
            cmd.AddParameter("PES_DATA_CHIUSURA", episodio.Dettaglio.DataChiusura)
            cmd.AddParameter("PES_ASINTOMATICO", BoolToSN(episodio.Testata.IsAsintomatico))
            cmd.AddParameter("PES_DATA_INIZIO_SINTOMI", episodio.Dettaglio.DataInizioSintomi)
            cmd.AddParameter("PES_NOTE", episodio.Testata.Note)
            cmd.AddParameter("PES_UTE_ID_ULTIMA_MODIFCA", episodio.Dettaglio.UtenteUltimaModifica)
            cmd.AddParameter("PES_DATA_ULTIMA_MODIFICA", episodio.Dettaglio.DataUltimaModifica)
            cmd.AddParameter("PES_PAZ_CODICE_OLD", episodio.Dettaglio.CodicePazienteOld)
            cmd.AddParameter("PES_INDIRIZZO_ISOLAMENTO", episodio.Dettaglio.IndirizzoIsolamento)
            cmd.AddParameter("PES_COM_CODICE_ISOLAMENTO", episodio.Dettaglio.ComuneCodiceIsolamento)
            cmd.AddParameter("PES_TELEFONO", episodio.Testata.TelefonoRilevazione)
            cmd.AddParameter("PES_EMAIL", episodio.Testata.EmailRilevazione)
            cmd.AddParameter("PES_INTERNO", episodio.Dettaglio.InternoRegione)
            cmd.AddParameter("PES_COM_CODICE", episodio.Dettaglio.CodiceComune)
            cmd.AddParameter("PES_GUARITO_CLINICAMENTE", episodio.Dettaglio.GuaritoClinicamente)
            cmd.AddParameter("PES_DATA_GUARITO_CLINICAMENTE", episodio.Dettaglio.DataGuaritoClinicamente)
            cmd.AddParameter("PES_GUARITO", episodio.Dettaglio.Guarito)
            cmd.AddParameter("PES_DATA_GUARITO_VIROLOGIC", episodio.Dettaglio.DataGuarigioneVirol)
        End Sub

        Private Sub AddCommonParametersDatoreLavoro(cmd As OracleCommand, episodio As EpisodioPaziente)
            cmd.Parameters.AddWithValue("PEL_DENOMINAZIONE_AZIENDA", setValue(episodio.DatoreLavoro.DenominazioneAzienda))
            cmd.Parameters.AddWithValue("PEL_RIFERIMENTO", setValue(episodio.DatoreLavoro.RiferimentoDatoreLavoro))
            cmd.Parameters.AddWithValue("PEL_COM_CODICE", setValue(episodio.DatoreLavoro.CodiceComuneSede))
            cmd.Parameters.AddWithValue("PEL_TELEFONO", setValue(episodio.DatoreLavoro.ContattoTelefonico))
            cmd.Parameters.AddWithValue("PEL_EMAIL", setValue(episodio.DatoreLavoro.ContattoEmail))
            cmd.Parameters.AddWithValue("PEL_ATA_CODICE", setValue(episodio.DatoreLavoro.CodiceTipoAzienda))
            cmd.Parameters.AddWithValue("PEL_NOTE", setValue(episodio.DatoreLavoro.Note))
        End Sub

        Private Sub AddCommonParametersClinica(cmd As OracleCommand, episodio As EpisodioPaziente)
            cmd.Parameters.AddWithValue("PCL_TUMORE", BoolToSN(episodio.Clinica.Tumore))
            cmd.Parameters.AddWithValue("PCL_DIABETE", BoolToSN(episodio.Clinica.Diabete))
            cmd.Parameters.AddWithValue("PCL_MAL_CARDIOVASCOLARI", BoolToSN(episodio.Clinica.MalattiaCardiovascolare))
            cmd.Parameters.AddWithValue("PCL_DEF_IMMUNITARI", BoolToSN(episodio.Clinica.ImmunoDeficienza))
            cmd.Parameters.AddWithValue("PCL_MAL_RESPIRATORIE", BoolToSN(episodio.Clinica.MalattiaRespiratoria))
            cmd.Parameters.AddWithValue("PCL_MAL_RENALI", BoolToSN(episodio.Clinica.MalattiaRenale))
            cmd.Parameters.AddWithValue("PCL_MAL_METABOLICHE", BoolToSN(episodio.Clinica.MalattiaMetabolica))
            cmd.Parameters.AddWithValue("PCL_ALTRO", BoolToSN(episodio.Clinica.Altro))
            cmd.Parameters.AddWithValue("PCL_OBESITA_BMI_30_40", BoolToSN(episodio.Clinica.Obesita_BMI_30_40))
            cmd.Parameters.AddWithValue("PCL_OBESITA_BMI_MAGGIORE_40", BoolToSN(episodio.Clinica.Obesita_BMI_Maggiore_40))
            cmd.Parameters.AddWithValue("PCL_NOTE", setValue(episodio.Clinica.Note))
        End Sub

        Private Sub AddCommonParametersRicoveri(cmd As OracleCommand, ricovero As Ricovero)
            cmd.Parameters.AddWithValue("PER_HSP_ID", setValue(ricovero.CodiceStruttura))
            cmd.Parameters.AddWithValue("PER_USL_CODICE", setValue(ricovero.CodiceUsl))
            cmd.Parameters.AddWithValue("PER_ARE_CODICE", setValue(ricovero.CodiceReparto))
            cmd.Parameters.AddWithValue("PER_DATA_INIZIO_RICOVERO", setValue(ricovero.DataInizio))
            cmd.Parameters.AddWithValue("PER_DATA_FINE_RICOVERO", setValue(ricovero.DataFine))
            cmd.Parameters.AddWithValue("PER_NOTE", setValue(ricovero.Note))
        End Sub

        Private Sub AddCommonParametersTamponi(cmd As IDbCommand, tampone As Tampone)
            cmd.AddParameter("PET_USL_CODICE", tampone.CodiceUsl)
            cmd.AddParameter("PET_DATA_TAMPONE", tampone.DataTampone)
            cmd.AddParameter("PET_ESITO", tampone.CodiceEsito)
            cmd.AddParameter("PET_NOTE", tampone.Note)
            cmd.AddParameter("PET_ID_CAMPIONE", tampone.CodiceCampione)
            cmd.AddParameter("PET_CODICE_LABORATORIO", tampone.CodiceLab)
            cmd.AddParameter("PET_FLG_DA_VISIONARE", tampone.DaVisionare)
            cmd.AddParameter("PET_DATA_RICHIESTA", tampone.DataRichiesta)
            cmd.AddParameter("PET_DATA_REFERTO", tampone.DataReferto)
            cmd.AddParameter("PET_TLM_ID", tampone.CodiceLaboratorio)
            cmd.AddParameter("PET_TIPO_TAMPONE", tampone.CodiceTipologia)
        End Sub

        Private Sub AddCommonParametersContatti(cmd As OracleCommand, contatto As Contatto)
            cmd.Parameters.AddWithValue("PEC_ECE_GRUPPO", setValue(contatto.CodiceImportazione))
            cmd.Parameters.AddWithValue("PEC_TIPO_RAPPORTO", setValue(contatto.CodiceTipoRapporto))
            cmd.Parameters.AddWithValue("PEC_NOTE", setValue(contatto.Note))
            cmd.Parameters.AddWithValue("PEC_TELEFONO", setValue(contatto.Telefono))
        End Sub

        Private Sub AddCommonParametersDiaria(cmd As OracleCommand, diaria As Diaria)
            cmd.Parameters.AddWithValue("PED_USL_CODICE_RILEVAZIONE", setValue(diaria.CodiceRilevazioneUsl))
            cmd.Parameters.AddWithValue("PED_DATA_RILEVAZIONE", setValue(diaria.DataRilevazione))
            cmd.Parameters.AddWithValue("PED_FLAG_ASINTOMATICO", BoolToSN(diaria.Asintomatico))
            cmd.Parameters.AddWithValue("PED_NOTE", setValue(diaria.Note))
            cmd.Parameters.AddWithValue("PED_RISPOSTA_TELEFONO", BoolToSN(diaria.RispostaTelefono))
        End Sub

        Private Sub AddCommonParametersSintomi(cmd As OracleCommand, sintomo As Sintomo)
            cmd.Parameters.AddWithValue("PDS_ASI_CODICE", setValue(sintomo.CodiceSintomo))
            cmd.Parameters.AddWithValue("PDS_NOTE", setValue(sintomo.Note))
        End Sub

        'Metodo che inserisce i nuovi contatti a DB e crea episodi per i soggetti, indicati come contatti, che non hanno un episodio attivo.
        Private Sub CreaContattiAndEpisodiContatti(cmd As OracleCommand, contattoToAdd As Contatto, episodio As EpisodioPaziente, codiceTipoCaso As String)
            Dim tipoCaso As String
            If String.IsNullOrWhiteSpace(contattoToAdd.CodiceImportazione) Then
                tipoCaso = codiceTipoCaso
            Else
                tipoCaso = CodiceTipoCasoContattoScolastico
            End If
            cmd.Parameters.Clear()
            Dim idEpisodioAttivo As Long? = RicercaEpisodioAttiviByPaziente(cmd, contattoToAdd.CodicePaziente.Value)

            If (Not idEpisodioAttivo.HasValue) Then
                'se non è stato trovato nessun episodio attivo per il paziente indicato come contatto, si crea un episodio.
                idEpisodioAttivo = CreaEpisodioContatto(cmd, contattoToAdd.CodicePaziente.Value, episodio.Dettaglio.DataInserimento, episodio.Dettaglio.UslInserimento,
                                                        episodio.Dettaglio.CodiceRaccoltaUsl, episodio.Dettaglio.CodiceUtenteInserimento,
                                                        episodio.Testata.DataSegnalazione, contattoToAdd.Telefono, tipoCaso, contattoToAdd.Note)
            End If

            'quando viene creato il contatto, si referenzia l'episodio preesistente attivo per il paziente oppure l'episodio appena creato.
            cmd.CommandText = QueryInsertContatti
            cmd.Parameters.AddWithValue("PEC_PES_ID", episodio.Testata.IdEpisodio)
            cmd.Parameters.AddWithValue("PEC_UTE_ID_INSERIMENTO", setValue(contattoToAdd.UtenteInserimento))
            cmd.Parameters.AddWithValue("PEC_USL_CODICE_INSERIMENTO", setValue(contattoToAdd.CodiceInserimentoUsl))
            cmd.Parameters.AddWithValue("PEC_DATA_INSERIMENTO", setValue(contattoToAdd.DataInserimento))
            cmd.Parameters.AddWithValue("PEC_PES_ID_CONTATTO", setValue(idEpisodioAttivo))
            cmd.Parameters.AddWithValue("PEC_PAZ_CODICE", setValue(contattoToAdd.CodicePaziente))
            AddCommonParametersContatti(cmd, contattoToAdd)

            cmd.ExecuteNonQuery()
            cmd.Parameters.Clear()
        End Sub

        'restituisce l'episodio attivo del paziente
        Private Function RicercaEpisodioAttiviByPaziente(cmd As OracleCommand, codicePaziente As Long) As Long?
            Dim result As Long? = Nothing
            cmd.Parameters.Clear()
            cmd.CommandText = QueryRicercaEpisodiAttiviByPaziente
            cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

            Using dr As OracleDataReader = cmd.ExecuteReader()
                If Not dr Is Nothing Then
                    Dim Codice As Integer = dr.GetOrdinal("PES_ID")
                    If dr.Read() Then
                        result = dr.GetInt64(Codice)
                    End If
                End If
            End Using
            cmd.Parameters.Clear()
            Return result
        End Function


#End Region

#Region " Funzionalita Di Frontiera "



        Public Function GetResultEsiti() As List(Of ResultEsiti) Implements IRilevazioniCovid19Provider.GetResultEsiti
            Dim result As New List(Of ResultEsiti)
            Dim ownConnection As Boolean = False
            Dim query As String = "SELECT LRE_ATC_RESULT, LRE_PET_ESITO " +
                                    "FROM T_ANA_LINK_RESULT_ESITI "
            Try
                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()

                        Dim LRE_ATC_RESULT As Integer = _context.GetOrdinal("LRE_ATC_RESULT")
                        Dim LRE_PET_ESITO As Integer = _context.GetOrdinal("LRE_PET_ESITO")


                        While _context.Read()
                            Dim EsitoTampone As New ResultEsiti()
                            EsitoTampone.AtcResult = _context.GetStringOrDefault(LRE_ATC_RESULT)
                            EsitoTampone.PetEsito = _context.GetStringOrDefault(LRE_PET_ESITO)

                            result.Add(EsitoTampone)
                        End While
                    End Using
                End Using
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return result
        End Function
        Public Function GetStatiEpisodio() As List(Of StatiEpisodio) Implements IRilevazioniCovid19Provider.GetStatiEpisodio
            Dim result As New List(Of StatiEpisodio)
            Dim ownConnection As Boolean = False
            Dim query As String = "SELECT SEP_CODICE, SEP_DESCRIZIONE " +
                                    "FROM T_ANA_STATI_EPISODIO "
            Try
                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()

                        Dim SEP_CODICE As Integer = _context.GetOrdinal("SEP_CODICE")
                        Dim SEP_DESCRIZIONE As Integer = _context.GetOrdinal("SEP_DESCRIZIONE")


                        While _context.Read()
                            Dim EsitoEpisodio As New StatiEpisodio()
                            EsitoEpisodio.SepCodice = _context.GetInt32OrDefault(SEP_CODICE)
                            EsitoEpisodio.SepDescrizione = _context.GetStringOrDefault(SEP_DESCRIZIONE)

                            result.Add(EsitoEpisodio)
                        End While
                    End Using
                End Using
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return result
        End Function

        Public Function EpisodiApertiPerPaziente(codicePaziente As Long) As List(Of Long) Implements IRilevazioniCovid19Provider.EpisodiApertiPerPaziente
            Return DoCommand(Function(cmd)
                                 cmd.CommandText = "select PES_ID 
                                                        FROM T_PAZ_EPISODI 
                                                        WHERE PES_PAZ_CODICE = ?PES_PAZ_CODICE"
                                 cmd.AddParameter("PES_PAZ_CODICE", codicePaziente)
                                 Return cmd.Fill(Of Long)
                             End Function)
        End Function

        ''' <summary>
        ''' Restituisce un test rapido
        ''' </summary>
        ''' <param name="idTest"></param>
        ''' <param name="ulssRichiedente"></param>
        ''' <returns></returns>
        Public Function GetTestRapidoById(idTest As String, ulssRichiedente As String) As TestRapido Implements IRilevazioniCovid19Provider.GetTestRapidoById
            Dim result As TestRapido = New TestRapido()
            Dim ownConnection As Boolean = False
            Dim query As String = "SELECT SCS_CAMPIONE_NR, SCS_COGNOME, SCS_NOME, SCS_CODICE_FISCALE, SCS_DATA_DI_NASCITA, SCS_LUOGO_DI_NASCITA, SCS_SESSO, SCS_RESIDENZA, SCS_CENTRO, SCS_ULSS_RES, SCS_ULSS_RICHIEDENTE, SCS_DATA_REFERTO, SCS_CELLULARE, SCS_CELLULARE_CONSENSO, SCS_DATA_PRIMO_ESITO " +
                                    "FROM T_ANA_SCARICO_SCREENING " +
                                    "WHERE SCS_CAMPIONE_NR = :SCS_CAMPIONE_NR AND SCS_ULSS_RICHIEDENTE = :SCS_ULSS_RICHIEDENTE"
            Try
                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    cmd.Parameters.AddWithValue("SCS_CAMPIONE_NR", idTest)
                    cmd.Parameters.AddWithValue("SCS_ULSS_RICHIEDENTE", ulssRichiedente)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()
                        Dim scs_campione_nr As Integer = _context.GetOrdinal("SCS_CAMPIONE_NR")
                        Dim scs_cognome As Integer = _context.GetOrdinal("SCS_COGNOME")
                        Dim scs_nome As Integer = _context.GetOrdinal("SCS_NOME")
                        Dim scs_codice_fiscale As Integer = _context.GetOrdinal("SCS_CODICE_FISCALE")
                        Dim scs_data_di_Nascita As Integer = _context.GetOrdinal("SCS_DATA_DI_NASCITA")
                        Dim scs_luogo_di_nascita As Integer = _context.GetOrdinal("SCS_LUOGO_DI_NASCITA")
                        Dim scs_sesso As Integer = _context.GetOrdinal("SCS_SESSO")
                        Dim scs_Residenza As Integer = _context.GetOrdinal("SCS_RESIDENZA")
                        Dim scs_centro As Integer = _context.GetOrdinal("SCS_CENTRO")
                        Dim scs_ulss_res As Integer = _context.GetOrdinal("SCS_ULSS_RES")
                        Dim scs_ulss_richiedente As Integer = _context.GetOrdinal("SCS_ULSS_RICHIEDENTE")
                        Dim scs_data_referto As Integer = _context.GetOrdinal("SCS_DATA_REFERTO")
                        Dim scs_cellulare As Integer = _context.GetOrdinal("SCS_CELLULARE")
                        Dim scs_cellulare_consenso As Integer = _context.GetOrdinal("SCS_CELLULARE_CONSENSO")
                        Dim scs_primo_esito As Integer = _context.GetOrdinal("SCS_DATA_PRIMO_ESITO")

                        If _context.Read() Then

                            result.IdTest = _context.GetInt64OrDefault(scs_campione_nr)
                            result.Cognome = _context.GetStringOrDefault(scs_cognome)
                            result.Nome = _context.GetStringOrDefault(scs_nome)
                            result.CodiceFiscale = _context.GetStringOrDefault(scs_codice_fiscale)
                            result.DataDiNascita = _context.GetDateTimeOrDefault(scs_data_di_Nascita)
                            result.LuogoDiNascita = _context.GetStringOrDefault(scs_luogo_di_nascita)
                            result.Sesso = _context.GetStringOrDefault(scs_sesso)
                            result.Residenza = _context.GetStringOrDefault(scs_Residenza)
                            result.Centro = _context.GetStringOrDefault(scs_centro)
                            result.UlssRes = _context.GetStringOrDefault(scs_ulss_res)
                            result.UlssRichiedente = _context.GetStringOrDefault(scs_ulss_richiedente)
                            result.DataReferto = _context.GetDateTimeOrDefault(scs_data_referto)
                            result.Cellulare = _context.GetStringOrDefault(scs_cellulare)
                            result.CellulareConsenso = _context.GetStringOrDefault(scs_cellulare_consenso)
                            result.PrimaRilevazione = _context.GetNullableDateTimeOrDefault(scs_primo_esito)

                        End If
                    End Using
                End Using
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return result
        End Function
        Public Function GetTestRapidoTarById(idTest As String, ulssRichiedente As String) As TestRapido Implements IRilevazioniCovid19Provider.GetTestRapidoTarById
            Dim result As TestRapido = New TestRapido()
            Dim ownConnection As Boolean = False
            Dim query As String = "SELECT SCT_CAMPIONE_NR, SCT_COGNOME, SCT_NOME, SCT_CODICE_FISCALE, SCT_DATA_DI_NASCITA, SCT_LUOGO_DI_NASCITA, SCT_SESSO, SCT_RESIDENZA, SCT_CENTRO, SCT_ULSS_RES, SCT_ULSS_RICHIEDENTE, SCT_DATA_REFERTO, SCT_MOTIVO_ESECUZIONE, SCT_CELLULARE, SCT_CELLULARE_CONSENSO, SCT_DATA_PRIMO_ESITO " +
                                    "FROM T_ANA_SCARICO_SCREENING_TAR " +
                                    "WHERE SCT_CAMPIONE_NR = :SCT_CAMPIONE_NR AND SCT_ULSS_RICHIEDENTE = :SCT_ULSS_RICHIEDENTE"
            Try
                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    cmd.Parameters.AddWithValue("SCT_CAMPIONE_NR", Convert.ToInt64(idTest))
                    cmd.Parameters.AddWithValue("SCT_ULSS_RICHIEDENTE", ulssRichiedente)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()
                        Dim sct_campione_nr As Integer = _context.GetOrdinal("SCT_CAMPIONE_NR")
                        Dim sct_cognome As Integer = _context.GetOrdinal("SCT_COGNOME")
                        Dim sct_nome As Integer = _context.GetOrdinal("SCT_NOME")
                        Dim sct_codice_fiscale As Integer = _context.GetOrdinal("SCT_CODICE_FISCALE")
                        Dim sct_data_di_Nascita As Integer = _context.GetOrdinal("SCT_DATA_DI_NASCITA")
                        Dim sct_luogo_di_nascita As Integer = _context.GetOrdinal("SCT_LUOGO_DI_NASCITA")
                        Dim sct_sesso As Integer = _context.GetOrdinal("SCT_SESSO")
                        Dim sct_Residenza As Integer = _context.GetOrdinal("SCT_RESIDENZA")
                        Dim sct_centro As Integer = _context.GetOrdinal("SCT_CENTRO")
                        Dim sct_ulss_res As Integer = _context.GetOrdinal("SCT_ULSS_RES")
                        Dim sct_ulss_richiedente As Integer = _context.GetOrdinal("SCT_ULSS_RICHIEDENTE")
                        Dim sct_data_referto As Integer = _context.GetOrdinal("SCT_DATA_REFERTO")
                        Dim sct_motivo_esecuzione As Integer = _context.GetOrdinal("SCT_MOTIVO_ESECUZIONE")
                        Dim sct_cellulare As Integer = _context.GetOrdinal("SCT_CELLULARE")
                        Dim sct_cellulare_consenso As Integer = _context.GetOrdinal("SCT_CELLULARE_CONSENSO")
                        Dim sct_primo_esito As Integer = _context.GetOrdinal("SCT_DATA_PRIMO_ESITO")

                        If _context.Read() Then

                            result.IdTest = _context.GetInt64OrDefault(sct_campione_nr)
                            result.Cognome = _context.GetStringOrDefault(sct_cognome)
                            result.Nome = _context.GetStringOrDefault(sct_nome)
                            result.CodiceFiscale = _context.GetStringOrDefault(sct_codice_fiscale)
                            result.DataDiNascita = _context.GetDateTimeOrDefault(sct_data_di_Nascita)
                            result.LuogoDiNascita = _context.GetStringOrDefault(sct_luogo_di_nascita)
                            result.Sesso = _context.GetStringOrDefault(sct_sesso)
                            result.Residenza = _context.GetStringOrDefault(sct_Residenza)
                            result.Centro = _context.GetStringOrDefault(sct_centro)
                            result.UlssRes = _context.GetStringOrDefault(sct_ulss_res)
                            result.UlssRichiedente = _context.GetStringOrDefault(sct_ulss_richiedente)
                            result.DataReferto = _context.GetDateTimeOrDefault(sct_data_referto)
                            result.MotivoEsecuzione = _context.GetStringOrDefault(sct_motivo_esecuzione)
                            result.Cellulare = _context.GetStringOrDefault(sct_cellulare)
                            result.CellulareConsenso = _context.GetStringOrDefault(sct_cellulare_consenso)
                            result.PrimaRilevazione = _context.GetNullableDateTimeOrDefault(sct_primo_esito)

                        End If
                    End Using
                End Using
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return result
        End Function
        ''' <summary>
        ''' Restituisce i test rapidi
        ''' </summary>
        ''' <param name="codiceFiscale"></param>
        ''' <param name="UlssRichiedente"></param>
        ''' <returns></returns>
        Public Function GetTestRapidoByCodiceFiscale(codiceFiscale As String, UlssRichiedente As String) As List(Of TestRapido) Implements IRilevazioniCovid19Provider.GetTestRapidoByCodiceFiscale
            Dim result As List(Of TestRapido) = New List(Of TestRapido)
            Dim ownConnection As Boolean = False
            Dim query As String = "SELECT SCS_CAMPIONE_NR, SCS_COGNOME, SCS_NOME, SCS_CODICE_FISCALE, SCS_DATA_DI_NASCITA, SCS_LUOGO_DI_NASCITA, SCS_SESSO, SCS_RESIDENZA, SCS_DATA_PRELIEVO, SCS_CENTRO, SCS_ULSS_RES, SCS_ULSS_RICHIEDENTE, SCS_DATA_REFERTO, SCS_CELLULARE, SCS_CELLULARE_CONSENSO, SCS_DATA_PRIMO_ESITO " +
                                    "FROM T_ANA_SCARICO_SCREENING " +
                                    "WHERE SCS_CODICE_FISCALE = :SCS_CODICE_FISCALE AND SCS_ULSS_RICHIEDENTE = :SCS_ULSS_RICHIEDENTE"
            Try
                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    cmd.Parameters.AddWithValue("SCS_CODICE_FISCALE", codiceFiscale)
                    cmd.Parameters.AddWithValue("SCS_ULSS_RICHIEDENTE", UlssRichiedente)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()
                        Dim scs_campione_nr As Integer = _context.GetOrdinal("SCS_CAMPIONE_NR")
                        Dim scs_cognome As Integer = _context.GetOrdinal("SCS_COGNOME")
                        Dim scs_nome As Integer = _context.GetOrdinal("SCS_NOME")
                        Dim scs_codice_fiscale As Integer = _context.GetOrdinal("SCS_CODICE_FISCALE")
                        Dim scs_data_di_Nascita As Integer = _context.GetOrdinal("SCS_DATA_DI_NASCITA")
                        Dim scs_luogo_di_nascita As Integer = _context.GetOrdinal("SCS_LUOGO_DI_NASCITA")
                        Dim scs_sesso As Integer = _context.GetOrdinal("SCS_SESSO")
                        Dim scs_Residenza As Integer = _context.GetOrdinal("SCS_RESIDENZA")
                        Dim scs_DataPrelievo As Integer = _context.GetOrdinal("SCS_DATA_PRELIEVO")
                        Dim scs_centro As Integer = _context.GetOrdinal("SCS_CENTRO")
                        Dim scs_ulss_res As Integer = _context.GetOrdinal("SCS_ULSS_RES")
                        Dim scs_ulss_richiedente As Integer = _context.GetOrdinal("SCS_ULSS_RICHIEDENTE")
                        Dim scs_data_referto As Integer = _context.GetOrdinal("SCS_DATA_REFERTO")
                        Dim scs_cellulare As Integer = _context.GetOrdinal("SCS_CELLULARE")
                        Dim scs_cellulare_consenso As Integer = _context.GetOrdinal("SCS_CELLULARE_CONSENSO")
                        Dim scs_primo_esito As Integer = _context.GetOrdinal("SCS_DATA_PRIMO_ESITO")

                        While _context.Read()
                            Dim test As TestRapido = New TestRapido()
                            test.IdTest = _context.GetInt64OrDefault(scs_campione_nr)
                            test.Cognome = _context.GetStringOrDefault(scs_cognome)
                            test.Nome = _context.GetStringOrDefault(scs_nome)
                            test.CodiceFiscale = _context.GetStringOrDefault(scs_codice_fiscale)
                            test.DataDiNascita = _context.GetDateTimeOrDefault(scs_data_di_Nascita)
                            test.LuogoDiNascita = _context.GetStringOrDefault(scs_luogo_di_nascita)
                            test.Sesso = _context.GetStringOrDefault(scs_sesso)
                            test.Residenza = _context.GetStringOrDefault(scs_Residenza)
                            test.DataPrelievo = _context.GetDateTimeOrDefault(scs_DataPrelievo)
                            test.Centro = _context.GetStringOrDefault(scs_centro)
                            test.UlssRes = _context.GetStringOrDefault(scs_ulss_res)
                            test.UlssRichiedente = _context.GetStringOrDefault(scs_ulss_richiedente)
                            test.DataReferto = _context.GetDateTimeOrDefault(scs_data_referto)
                            test.Cellulare = _context.GetStringOrDefault(scs_cellulare)
                            test.CellulareConsenso = _context.GetStringOrDefault(scs_cellulare_consenso)
                            test.PrimaRilevazione = _context.GetNullableDateTimeOrDefault(scs_primo_esito)

                            result.Add(test)
                        End While
                    End Using
                End Using
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return result
        End Function
        Public Function GetTestRapidoTarByCodiceFiscale(codiceFiscale As String, UlssRichiedente As String) As List(Of TestRapido) Implements IRilevazioniCovid19Provider.GetTestRapidoTarByCodiceFiscale
            Dim result As List(Of TestRapido) = New List(Of TestRapido)
            Dim ownConnection As Boolean = False
            Dim query As String = "SELECT SCT_CAMPIONE_NR, SCT_COGNOME, SCT_NOME, SCT_CODICE_FISCALE, SCT_DATA_DI_NASCITA, SCT_LUOGO_DI_NASCITA, SCT_SESSO, SCT_RESIDENZA, SCT_DATA_PRELIEVO, SCT_CENTRO, SCT_ULSS_RES, SCT_ULSS_RICHIEDENTE, SCT_DATA_REFERTO, SCT_MOTIVO_ESECUZIONE, SCT_CELLULARE, SCT_CELLULARE_CONSENSO, SCT_DATA_PRIMO_ESITO " +
                                    "FROM T_ANA_SCARICO_SCREENING_TAR " +
                                    "WHERE SCT_CODICE_FISCALE = :SCT_CODICE_FISCALE AND SCT_ULSS_RICHIEDENTE = :SCT_ULSS_RICHIEDENTE"
            Try
                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    cmd.Parameters.AddWithValue("SCT_CODICE_FISCALE", codiceFiscale)
                    cmd.Parameters.AddWithValue("SCT_ULSS_RICHIEDENTE", UlssRichiedente)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()
                        Dim sct_campione_nr As Integer = _context.GetOrdinal("SCT_CAMPIONE_NR")
                        Dim sct_cognome As Integer = _context.GetOrdinal("SCT_COGNOME")
                        Dim sct_nome As Integer = _context.GetOrdinal("SCT_NOME")
                        Dim sct_codice_fiscale As Integer = _context.GetOrdinal("SCT_CODICE_FISCALE")
                        Dim sct_data_di_Nascita As Integer = _context.GetOrdinal("SCT_DATA_DI_NASCITA")
                        Dim sct_luogo_di_nascita As Integer = _context.GetOrdinal("SCT_LUOGO_DI_NASCITA")
                        Dim sct_sesso As Integer = _context.GetOrdinal("SCT_SESSO")
                        Dim sct_Residenza As Integer = _context.GetOrdinal("SCT_RESIDENZA")
                        Dim sct_DataPrelievo As Integer = _context.GetOrdinal("SCT_DATA_PRELIEVO")
                        Dim sct_centro As Integer = _context.GetOrdinal("SCT_CENTRO")
                        Dim sct_ulss_res As Integer = _context.GetOrdinal("SCT_ULSS_RES")
                        Dim sct_ulss_richiedente As Integer = _context.GetOrdinal("SCT_ULSS_RICHIEDENTE")
                        Dim sct_data_referto As Integer = _context.GetOrdinal("SCT_DATA_REFERTO")
                        Dim sct_motivo_esecuzione As Integer = _context.GetOrdinal("SCT_MOTIVO_ESECUZIONE")
                        Dim sct_cellulare As Integer = _context.GetOrdinal("SCT_CELLULARE")
                        Dim sct_cellulare_consenso As Integer = _context.GetOrdinal("SCT_CELLULARE_CONSENSO")
                        Dim sct_primo_esito As Integer = _context.GetOrdinal("SCT_DATA_PRIMO_ESITO")

                        While _context.Read()
                            Dim test As TestRapido = New TestRapido()
                            test.IdTest = _context.GetInt64OrDefault(sct_campione_nr)
                            test.Cognome = _context.GetStringOrDefault(sct_cognome)
                            test.Nome = _context.GetStringOrDefault(sct_nome)
                            test.CodiceFiscale = _context.GetStringOrDefault(sct_codice_fiscale)
                            test.DataDiNascita = _context.GetDateTimeOrDefault(sct_data_di_Nascita)
                            test.LuogoDiNascita = _context.GetStringOrDefault(sct_luogo_di_nascita)
                            test.Sesso = _context.GetStringOrDefault(sct_sesso)
                            test.Residenza = _context.GetStringOrDefault(sct_Residenza)
                            test.DataPrelievo = _context.GetDateTimeOrDefault(sct_DataPrelievo)
                            test.Centro = _context.GetStringOrDefault(sct_centro)
                            test.UlssRes = _context.GetStringOrDefault(sct_ulss_res)
                            test.UlssRichiedente = _context.GetStringOrDefault(sct_ulss_richiedente)
                            test.DataReferto = _context.GetDateTimeOrDefault(sct_data_referto)
                            test.MotivoEsecuzione = _context.GetStringOrDefault(sct_motivo_esecuzione)
                            test.Cellulare = _context.GetStringOrDefault(sct_cellulare)
                            test.CellulareConsenso = _context.GetStringOrDefault(sct_cellulare_consenso)
                            test.PrimaRilevazione = _context.GetNullableDateTimeOrDefault(sct_primo_esito)
                            result.Add(test)
                        End While
                    End Using
                End Using
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return result
        End Function
        Public Function GetTestRapidoByCFeId(idTest As String, codiceFiscale As String, ulssRichiedente As String) As TestRapido Implements IRilevazioniCovid19Provider.GetTestRapidoByCFeId
            Dim result As TestRapido = New TestRapido()
            Dim ownConnection As Boolean = False
            Dim query As String = "SELECT SCS_CAMPIONE_NR, SCS_COGNOME, SCS_NOME, SCS_CODICE_FISCALE, SCS_DATA_DI_NASCITA, SCS_LUOGO_DI_NASCITA, SCS_SESSO, SCS_RESIDENZA, SCS_CENTRO, SCS_ULSS_RES, SCS_ULSS_RICHIEDENTE, SCS_DATA_REFERTO, SCS_CELLULARE, SCS_CELLULARE_CONSENSO, SCS_DATA_PRIMO_ESITO " +
                                    "FROM T_ANA_SCARICO_SCREENING " +
                                    "WHERE SCS_CODICE_FISCALE = :SCS_CODICE_FISCALE AND SCS_CAMPIONE_NR = :SCS_CAMPIONE_NR AND SCS_ULSS_RICHIEDENTE = :SCS_ULSS_RICHIEDENTE"
            Try
                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    cmd.Parameters.AddWithValue("SCS_CODICE_FISCALE", codiceFiscale)
                    cmd.Parameters.AddWithValue("SCS_CAMPIONE_NR", idTest)
                    cmd.Parameters.AddWithValue("SCS_ULSS_RICHIEDENTE", ulssRichiedente)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()

                        Dim scs_campione_nr As Integer = _context.GetOrdinal("SCS_CAMPIONE_NR")
                        Dim scs_cognome As Integer = _context.GetOrdinal("SCS_COGNOME")
                        Dim scs_nome As Integer = _context.GetOrdinal("SCS_NOME")
                        Dim scs_codice_fiscale As Integer = _context.GetOrdinal("SCS_CODICE_FISCALE")
                        Dim scs_data_di_Nascita As Integer = _context.GetOrdinal("SCS_DATA_DI_NASCITA")
                        Dim scs_luogo_di_nascita As Integer = _context.GetOrdinal("SCS_LUOGO_DI_NASCITA")
                        Dim scs_sesso As Integer = _context.GetOrdinal("SCS_SESSO")
                        Dim scs_Residenza As Integer = _context.GetOrdinal("SCS_RESIDENZA")
                        Dim scs_centro As Integer = _context.GetOrdinal("SCS_CENTRO")
                        Dim scs_ulss_res As Integer = _context.GetOrdinal("SCS_ULSS_RES")
                        Dim scs_ulss_richiedente As Integer = _context.GetOrdinal("SCS_ULSS_RICHIEDENTE")
                        Dim scs_data_referto As Integer = _context.GetOrdinal("SCS_DATA_REFERTO")
                        Dim scs_cellulare As Integer = _context.GetOrdinal("SCS_CELLULARE")
                        Dim scs_cellulare_consenso As Integer = _context.GetOrdinal("SCS_CELLULARE_CONSENSO")
                        Dim scs_primo_esito As Integer = _context.GetOrdinal("SCS_DATA_PRIMO_ESITO")

                        If _context.Read() Then
                            result.IdTest = _context.GetInt64OrDefault(scs_campione_nr)
                            result.Cognome = _context.GetStringOrDefault(scs_cognome)
                            result.Nome = _context.GetStringOrDefault(scs_nome)
                            result.CodiceFiscale = _context.GetStringOrDefault(scs_codice_fiscale)
                            result.DataDiNascita = _context.GetDateTimeOrDefault(scs_data_di_Nascita)
                            result.LuogoDiNascita = _context.GetStringOrDefault(scs_luogo_di_nascita)
                            result.Sesso = _context.GetStringOrDefault(scs_sesso)
                            result.Residenza = _context.GetStringOrDefault(scs_Residenza)
                            result.Centro = _context.GetStringOrDefault(scs_centro)
                            result.UlssRes = _context.GetStringOrDefault(scs_ulss_res)
                            result.UlssRichiedente = _context.GetStringOrDefault(scs_ulss_richiedente)
                            result.DataReferto = _context.GetDateTimeOrDefault(scs_data_referto)
                            result.Cellulare = _context.GetStringOrDefault(scs_cellulare)
                            result.CellulareConsenso = _context.GetStringOrDefault(scs_cellulare_consenso)
                            result.PrimaRilevazione = _context.GetNullableDateTimeOrDefault(scs_primo_esito)
                        End If
                    End Using
                End Using
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return result
        End Function
        Public Function GetTestRapidoTarByCFeId(idTest As String, codiceFiscale As String, ulssRichiedente As String) As TestRapido Implements IRilevazioniCovid19Provider.GetTestRapidoTarByCFeId
            Dim result As TestRapido = New TestRapido()
            Dim ownConnection As Boolean = False
            Dim query As String = "SELECT SCT_CAMPIONE_NR, SCT_COGNOME, SCT_NOME, SCT_CODICE_FISCALE, SCT_DATA_DI_NASCITA, SCT_LUOGO_DI_NASCITA, SCT_SESSO, SCT_RESIDENZA, SCT_CENTRO, SCT_ULSS_RES, SCT_ULSS_RICHIEDENTE, SCT_DATA_REFERTO, SCT_MOTIVO_ESECUZIONE, SCT_CELLULARE, SCT_CELLULARE_CONSENSO, SCT_DATA_PRIMO_ESITO " +
                                    "FROM T_ANA_SCARICO_SCREENING_TAR " +
                                    "WHERE SCT_CODICE_FISCALE = :SCT_CODICE_FISCALE AND SCT_CAMPIONE_NR = :SCT_CAMPIONE_NR AND SCT_ULSS_RICHIEDENTE = :SCT_ULSS_RICHIEDENTE"
            Try
                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    cmd.Parameters.AddWithValue("SCT_CODICE_FISCALE", codiceFiscale)
                    cmd.Parameters.AddWithValue("SCT_CAMPIONE_NR", Convert.ToInt64(idTest))
                    cmd.Parameters.AddWithValue("SCT_ULSS_RICHIEDENTE", ulssRichiedente)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()

                        Dim sct_campione_nr As Integer = _context.GetOrdinal("SCT_CAMPIONE_NR")
                        Dim sct_cognome As Integer = _context.GetOrdinal("SCT_COGNOME")
                        Dim sct_nome As Integer = _context.GetOrdinal("SCT_NOME")
                        Dim sct_codice_fiscale As Integer = _context.GetOrdinal("SCT_CODICE_FISCALE")
                        Dim sct_data_di_Nascita As Integer = _context.GetOrdinal("SCT_DATA_DI_NASCITA")
                        Dim sct_luogo_di_nascita As Integer = _context.GetOrdinal("SCT_LUOGO_DI_NASCITA")
                        Dim sct_sesso As Integer = _context.GetOrdinal("SCT_SESSO")
                        Dim sct_Residenza As Integer = _context.GetOrdinal("SCT_RESIDENZA")
                        Dim sct_centro As Integer = _context.GetOrdinal("SCT_CENTRO")
                        Dim sct_ulss_res As Integer = _context.GetOrdinal("SCT_ULSS_RES")
                        Dim sct_ulss_richiedente As Integer = _context.GetOrdinal("SCT_ULSS_RICHIEDENTE")
                        Dim sct_data_referto As Integer = _context.GetOrdinal("SCT_DATA_REFERTO")
                        Dim sct_motivo_esecuzione As Integer = _context.GetOrdinal("SCT_MOTIVO_ESECUZIONE")
                        Dim sct_cellulare As Integer = _context.GetOrdinal("SCT_CELLULARE")
                        Dim sct_cellulare_consenso As Integer = _context.GetOrdinal("SCT_CELLULARE_CONSENSO")
                        Dim sct_primo_esito As Integer = _context.GetOrdinal("SCT_DATA_PRIMO_ESITO")

                        If _context.Read() Then
                            result.IdTest = _context.GetInt64OrDefault(sct_campione_nr)
                            result.Cognome = _context.GetStringOrDefault(sct_cognome)
                            result.Nome = _context.GetStringOrDefault(sct_nome)
                            result.CodiceFiscale = _context.GetStringOrDefault(sct_codice_fiscale)
                            result.DataDiNascita = _context.GetDateTimeOrDefault(sct_data_di_Nascita)
                            result.LuogoDiNascita = _context.GetStringOrDefault(sct_luogo_di_nascita)
                            result.Sesso = _context.GetStringOrDefault(sct_sesso)
                            result.Residenza = _context.GetStringOrDefault(sct_Residenza)
                            result.Centro = _context.GetStringOrDefault(sct_centro)
                            result.UlssRes = _context.GetStringOrDefault(sct_ulss_res)
                            result.UlssRichiedente = _context.GetStringOrDefault(sct_ulss_richiedente)
                            result.DataReferto = _context.GetDateTimeOrDefault(sct_data_referto)
                            result.MotivoEsecuzione = _context.GetStringOrDefault(sct_motivo_esecuzione)
                            result.Cellulare = _context.GetStringOrDefault(sct_cellulare)
                            result.CellulareConsenso = _context.GetStringOrDefault(sct_cellulare_consenso)
                            result.PrimaRilevazione = _context.GetNullableDateTimeOrDefault(sct_primo_esito)
                        End If
                    End Using
                End Using
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return result
        End Function

        ''' <summary>
        ''' Restituisce i centri dato il gruppo
        ''' </summary>
        ''' <param name="gruppo"></param>
        ''' <returns></returns>
        Public Function GetCentriByGruppo(gruppo As String) As List(Of String) Implements IRilevazioniCovid19Provider.GetCentriByGruppo
            Dim result As New List(Of String)
            Dim ownConnection As Boolean = False
            Dim query As String = "SELECT LCG_CENTRO " +
                                    "FROM T_ANA_LINK_CENTRO_GRUPPOCENTRI " +
                                    "WHERE LCG_GRUPPO = :LCG_GRUPPO"
            Try
                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    cmd.Parameters.AddWithValue("LCG_GRUPPO", gruppo)
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()

                        Dim lcg_centro As Integer = _context.GetOrdinal("LCG_CENTRO")
                        While _context.Read()
                            Dim centro As String
                            centro = _context.GetStringOrDefault(lcg_centro)
                            result.Add(centro)
                        End While
                    End Using
                End Using
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return result
        End Function

        Public Function GetTestRapidoByCFeIdeGr(idTest As String, codiceFiscale As String, gruppo As String) As TestRapido Implements IRilevazioniCovid19Provider.GetTestRapidoByCFeIdeGr
            Dim result As TestRapido = New TestRapido()
            Dim ownConnection As Boolean = False
            Dim query As String = "SELECT SCS_CAMPIONE_NR, SCS_COGNOME, SCS_NOME, SCS_CODICE_FISCALE, SCS_DATA_DI_NASCITA, SCS_LUOGO_DI_NASCITA, SCS_SESSO, SCS_RESIDENZA, SCS_CENTRO, SCS_ULSS_RES, SCS_ULSS_RICHIEDENTE, SCS_DATA_REFERTO, SCS_CELLULARE, SCS_CELLULARE_CONSENSO, SCS_DATA_PRIMO_ESITO" +
                                    "FROM T_ANA_SCARICO_SCREENING " +
                                    "LEFT JOIN T_ANA_LINK_CENTRO_GRUPPOCENTRI ON T_ANA_LINK_CENTRO_GRUPPOCENTRI.LCG_CENTRO = T_ANA_SCARICO_SCREENING.SCS_CENTRO " +
                                    "WHERE SCS_CODICE_FISCALE = :SCS_CODICE_FISCALE AND SCS_CAMPIONE_NR = :SCS_CAMPIONE_NR AND LCG_GRUPPO = :LCG_GRUPPO"
            Try
                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    cmd.Parameters.AddWithValue("SCS_CODICE_FISCALE", codiceFiscale)
                    cmd.Parameters.AddWithValue("SCS_CAMPIONE_NR", idTest)
                    cmd.Parameters.AddWithValue("LCG_GRUPPO", gruppo)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()

                        Dim scs_campione_nr As Integer = _context.GetOrdinal("SCS_CAMPIONE_NR")
                        Dim scs_cognome As Integer = _context.GetOrdinal("SCS_COGNOME")
                        Dim scs_nome As Integer = _context.GetOrdinal("SCS_NOME")
                        Dim scs_codice_fiscale As Integer = _context.GetOrdinal("SCS_CODICE_FISCALE")
                        Dim scs_data_di_Nascita As Integer = _context.GetOrdinal("SCS_DATA_DI_NASCITA")
                        Dim scs_luogo_di_nascita As Integer = _context.GetOrdinal("SCS_LUOGO_DI_NASCITA")
                        Dim scs_sesso As Integer = _context.GetOrdinal("SCS_SESSO")
                        Dim scs_Residenza As Integer = _context.GetOrdinal("SCS_RESIDENZA")
                        Dim scs_centro As Integer = _context.GetOrdinal("SCS_CENTRO")
                        Dim scs_ulss_res As Integer = _context.GetOrdinal("SCS_ULSS_RES")
                        Dim scs_ulss_richiedente As Integer = _context.GetOrdinal("SCS_ULSS_RICHIEDENTE")
                        Dim scs_data_referto As Integer = _context.GetOrdinal("SCS_DATA_REFERTO")
                        Dim scs_cellulare As Integer = _context.GetOrdinal("SCS_CELLULARE")
                        Dim scs_cellulare_consenso As Integer = _context.GetOrdinal("SCS_CELLULARE_CONSENSO")
                        Dim scs_primo_esito As Integer = _context.GetOrdinal("SCS_DATA_PRIMO_ESITO")

                        If _context.Read() Then
                            result.IdTest = _context.GetInt64OrDefault(scs_campione_nr)
                            result.Cognome = _context.GetStringOrDefault(scs_cognome)
                            result.Nome = _context.GetStringOrDefault(scs_nome)
                            result.CodiceFiscale = _context.GetStringOrDefault(scs_codice_fiscale)
                            result.DataDiNascita = _context.GetDateTimeOrDefault(scs_data_di_Nascita)
                            result.LuogoDiNascita = _context.GetStringOrDefault(scs_luogo_di_nascita)
                            result.Sesso = _context.GetStringOrDefault(scs_sesso)
                            result.Residenza = _context.GetStringOrDefault(scs_Residenza)
                            result.Centro = _context.GetStringOrDefault(scs_centro)
                            result.UlssRes = _context.GetStringOrDefault(scs_ulss_res)
                            result.UlssRichiedente = _context.GetStringOrDefault(scs_ulss_richiedente)
                            result.DataReferto = _context.GetDateTimeOrDefault(scs_data_referto)
                            result.Cellulare = _context.GetStringOrDefault(scs_cellulare)
                            result.CellulareConsenso = _context.GetStringOrDefault(scs_cellulare_consenso)
                            result.PrimaRilevazione = _context.GetNullableDateTimeOrDefault(scs_primo_esito)
                        End If
                    End Using
                End Using
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return result
        End Function
        Public Function GetTestRapidoTarByCFeIdeGr(idTest As String, codiceFiscale As String, gruppo As String) As TestRapido Implements IRilevazioniCovid19Provider.GetTestRapidoTarByCFeIdeGr
            Dim result As TestRapido = New TestRapido()
            Dim ownConnection As Boolean = False
            Dim query As String = "SELECT SCT_CAMPIONE_NR, SCT_COGNOME, SCT_NOME, SCT_CODICE_FISCALE, SCT_DATA_DI_NASCITA, SCT_LUOGO_DI_NASCITA, SCT_SESSO, SCT_RESIDENZA, SCT_CENTRO, SCT_ULSS_RES, SCT_ULSS_RICHIEDENTE, SCT_DATA_REFERTO, SCT_MOTIVO_ESECUZIONE, SCT_CELLULARE, SCT_CELLULARE_CONSENSO, SCT_DATA_PRIMO_ESITO " +
                                    "FROM T_ANA_SCARICO_SCREENING_TAR " +
                                    "LEFT JOIN T_ANA_LINK_CENTRO_GRUPPOCENTRI ON T_ANA_LINK_CENTRO_GRUPPOCENTRI.LCG_CENTRO = T_ANA_SCARICO_SCREENING_TAR.SCT_CENTRO " +
                                    "WHERE SCT_CODICE_FISCALE = :SCT_CODICE_FISCALE AND SCT_CAMPIONE_NR = :SCT_CAMPIONE_NR AND LCG_GRUPPO = :LCG_GRUPPO"
            Try
                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    cmd.Parameters.AddWithValue("SCT_CODICE_FISCALE", codiceFiscale)
                    cmd.Parameters.AddWithValue("SCT_CAMPIONE_NR", Convert.ToInt64(idTest))
                    cmd.Parameters.AddWithValue("LCG_GRUPPO", gruppo)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()

                        Dim sct_campione_nr As Integer = _context.GetOrdinal("SCT_CAMPIONE_NR")
                        Dim sct_cognome As Integer = _context.GetOrdinal("SCT_COGNOME")
                        Dim sct_nome As Integer = _context.GetOrdinal("SCT_NOME")
                        Dim sct_codice_fiscale As Integer = _context.GetOrdinal("SCT_CODICE_FISCALE")
                        Dim sct_data_di_Nascita As Integer = _context.GetOrdinal("SCT_DATA_DI_NASCITA")
                        Dim sct_luogo_di_nascita As Integer = _context.GetOrdinal("SCT_LUOGO_DI_NASCITA")
                        Dim sct_sesso As Integer = _context.GetOrdinal("SCT_SESSO")
                        Dim sct_Residenza As Integer = _context.GetOrdinal("SCT_RESIDENZA")
                        Dim sct_centro As Integer = _context.GetOrdinal("SCT_CENTRO")
                        Dim sct_ulss_res As Integer = _context.GetOrdinal("SCT_ULSS_RES")
                        Dim sct_ulss_richiedente As Integer = _context.GetOrdinal("SCT_ULSS_RICHIEDENTE")
                        Dim sct_data_referto As Integer = _context.GetOrdinal("SCT_DATA_REFERTO")
                        Dim sct_motivo_esecuzione As Integer = _context.GetOrdinal("SCT_MOTIVO_ESECUZIONE")
                        Dim sct_cellulare As Integer = _context.GetOrdinal("SCT_CELLULARE")
                        Dim sct_cellulare_consenso As Integer = _context.GetOrdinal("SCT_CELLULARE_CONSENSO")
                        Dim sct_primo_esito As Integer = _context.GetOrdinal("SCT_DATA_PRIMO_ESITO")

                        If _context.Read() Then
                            result.IdTest = _context.GetInt64OrDefault(sct_campione_nr)
                            result.Cognome = _context.GetStringOrDefault(sct_cognome)
                            result.Nome = _context.GetStringOrDefault(sct_nome)
                            result.CodiceFiscale = _context.GetStringOrDefault(sct_codice_fiscale)
                            result.DataDiNascita = _context.GetDateTimeOrDefault(sct_data_di_Nascita)
                            result.LuogoDiNascita = _context.GetStringOrDefault(sct_luogo_di_nascita)
                            result.Sesso = _context.GetStringOrDefault(sct_sesso)
                            result.Residenza = _context.GetStringOrDefault(sct_Residenza)
                            result.Centro = _context.GetStringOrDefault(sct_centro)
                            result.UlssRes = _context.GetStringOrDefault(sct_ulss_res)
                            result.UlssRichiedente = _context.GetStringOrDefault(sct_ulss_richiedente)
                            result.DataReferto = _context.GetDateTimeOrDefault(sct_data_referto)
                            result.MotivoEsecuzione = _context.GetStringOrDefault(sct_motivo_esecuzione)
                            result.Cellulare = _context.GetStringOrDefault(sct_cellulare)
                            result.CellulareConsenso = _context.GetStringOrDefault(sct_cellulare_consenso)
                            result.PrimaRilevazione = _context.GetNullableDateTimeOrDefault(sct_primo_esito)
                        End If
                    End Using
                End Using
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return result
        End Function
        Public Function GetTestRapidoByIdGr(idTest As String, Gruppo As String) As TestRapido Implements IRilevazioniCovid19Provider.GetTestRapidoByIdGr
            Dim result As TestRapido = New TestRapido()
            Dim ownConnection As Boolean = False
            Dim query As String = "SELECT SCS_CAMPIONE_NR, SCS_COGNOME, SCS_NOME, SCS_CODICE_FISCALE, SCS_DATA_DI_NASCITA, SCS_LUOGO_DI_NASCITA, SCS_SESSO, SCS_RESIDENZA, SCS_CENTRO, SCS_ULSS_RES, SCS_ULSS_RICHIEDENTE, SCS_DATA_REFERTO, SCS_CELLULARE, SCS_CELLULARE_CONSENSO, SCS_DATA_PRIMO_ESITO " +
                                    "FROM T_ANA_SCARICO_SCREENING " +
                                    "LEFT JOIN T_ANA_LINK_CENTRO_GRUPPOCENTRI ON T_ANA_LINK_CENTRO_GRUPPOCENTRI.LCG_CENTRO = T_ANA_SCARICO_SCREENING.SCS_CENTRO " +
                                    "WHERE SCS_CAMPIONE_NR = :SCS_CAMPIONE_NR AND LCG_GRUPPO = :LCG_GRUPPO"
            Try
                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    cmd.Parameters.AddWithValue("SCS_CAMPIONE_NR", idTest)
                    cmd.Parameters.AddWithValue("LCG_GRUPPO", Gruppo)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()
                        Dim scs_campione_nr As Integer = _context.GetOrdinal("SCS_CAMPIONE_NR")
                        Dim scs_cognome As Integer = _context.GetOrdinal("SCS_COGNOME")
                        Dim scs_nome As Integer = _context.GetOrdinal("SCS_NOME")
                        Dim scs_codice_fiscale As Integer = _context.GetOrdinal("SCS_CODICE_FISCALE")
                        Dim scs_data_di_Nascita As Integer = _context.GetOrdinal("SCS_DATA_DI_NASCITA")
                        Dim scs_luogo_di_nascita As Integer = _context.GetOrdinal("SCS_LUOGO_DI_NASCITA")
                        Dim scs_sesso As Integer = _context.GetOrdinal("SCS_SESSO")
                        Dim scs_Residenza As Integer = _context.GetOrdinal("SCS_RESIDENZA")
                        Dim scs_centro As Integer = _context.GetOrdinal("SCS_CENTRO")
                        Dim scs_ulss_res As Integer = _context.GetOrdinal("SCS_ULSS_RES")
                        Dim scs_ulss_richiedente As Integer = _context.GetOrdinal("SCS_ULSS_RICHIEDENTE")
                        Dim scs_data_referto As Integer = _context.GetOrdinal("SCS_DATA_REFERTO")
                        Dim scs_cellulare As Integer = _context.GetOrdinal("SCS_CELLULARE")
                        Dim scs_cellulare_consenso As Integer = _context.GetOrdinal("SCS_CELLULARE_CONSENSO")
                        Dim scs_primo_esito As Integer = _context.GetOrdinal("SCS_DATA_PRIMO_ESITO")

                        If _context.Read() Then

                            result.IdTest = _context.GetInt64OrDefault(scs_campione_nr)
                            result.Cognome = _context.GetStringOrDefault(scs_cognome)
                            result.Nome = _context.GetStringOrDefault(scs_nome)
                            result.CodiceFiscale = _context.GetStringOrDefault(scs_codice_fiscale)
                            result.DataDiNascita = _context.GetDateTimeOrDefault(scs_data_di_Nascita)
                            result.LuogoDiNascita = _context.GetStringOrDefault(scs_luogo_di_nascita)
                            result.Sesso = _context.GetStringOrDefault(scs_sesso)
                            result.Residenza = _context.GetStringOrDefault(scs_Residenza)
                            result.Centro = _context.GetStringOrDefault(scs_centro)
                            result.UlssRes = _context.GetStringOrDefault(scs_ulss_res)
                            result.UlssRichiedente = _context.GetStringOrDefault(scs_ulss_richiedente)
                            result.DataReferto = _context.GetDateTimeOrDefault(scs_data_referto)
                            result.Cellulare = _context.GetStringOrDefault(scs_cellulare)
                            result.CellulareConsenso = _context.GetStringOrDefault(scs_cellulare_consenso)
                            result.PrimaRilevazione = _context.GetNullableDateTimeOrDefault(scs_primo_esito)

                        End If
                    End Using
                End Using
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return result
        End Function
        Public Function GetTestRapidoTarByIdGr(idTest As String, Gruppo As String) As TestRapido Implements IRilevazioniCovid19Provider.GetTestRapidoTarByIdGr
            Dim result As TestRapido = New TestRapido()
            Dim ownConnection As Boolean = False
            Dim query As String = "SELECT SCT_CAMPIONE_NR, SCT_COGNOME, SCT_NOME, SCT_CODICE_FISCALE, SCT_DATA_DI_NASCITA, SCT_LUOGO_DI_NASCITA, SCT_SESSO, SCT_RESIDENZA, SCT_CENTRO, SCT_ULSS_RES, SCT_ULSS_RICHIEDENTE, SCT_DATA_REFERTO, SCT_MOTIVO_ESECUZIONE, SCT_CELLULARE, SCT_CELLULARE_CONSENSO, SCT_DATA_PRIMO_ESITO " +
                                    "FROM T_ANA_SCARICO_SCREENING_TAR " +
                                    "LEFT JOIN T_ANA_LINK_CENTRO_GRUPPOCENTRI ON T_ANA_LINK_CENTRO_GRUPPOCENTRI.LCG_CENTRO = T_ANA_SCARICO_SCREENING_TAR.SCT_CENTRO " +
                                    "WHERE SCT_CAMPIONE_NR = :SCT_CAMPIONE_NR AND LCG_GRUPPO = :LCG_GRUPPO"
            Try
                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    cmd.Parameters.AddWithValue("SCT_CAMPIONE_NR", Convert.ToInt64(idTest))
                    cmd.Parameters.AddWithValue("LCG_GRUPPO", Gruppo)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()
                        Dim sct_campione_nr As Integer = _context.GetOrdinal("SCT_CAMPIONE_NR")
                        Dim sct_cognome As Integer = _context.GetOrdinal("SCT_COGNOME")
                        Dim sct_nome As Integer = _context.GetOrdinal("SCT_NOME")
                        Dim sct_codice_fiscale As Integer = _context.GetOrdinal("SCT_CODICE_FISCALE")
                        Dim sct_data_di_Nascita As Integer = _context.GetOrdinal("SCT_DATA_DI_NASCITA")
                        Dim sct_luogo_di_nascita As Integer = _context.GetOrdinal("SCT_LUOGO_DI_NASCITA")
                        Dim sct_sesso As Integer = _context.GetOrdinal("SCT_SESSO")
                        Dim sct_Residenza As Integer = _context.GetOrdinal("SCT_RESIDENZA")
                        Dim sct_centro As Integer = _context.GetOrdinal("SCT_CENTRO")
                        Dim sct_ulss_res As Integer = _context.GetOrdinal("SCT_ULSS_RES")
                        Dim sct_ulss_richiedente As Integer = _context.GetOrdinal("SCT_ULSS_RICHIEDENTE")
                        Dim sct_data_referto As Integer = _context.GetOrdinal("SCT_DATA_REFERTO")
                        Dim sct_motivo_esecuzione As Integer = _context.GetOrdinal("SCT_MOTIVO_ESECUZIONE")
                        Dim sct_cellulare As Integer = _context.GetOrdinal("SCT_CELLULARE")
                        Dim sct_cellulare_consenso As Integer = _context.GetOrdinal("SCT_CELLULARE_CONSENSO")
                        Dim sct_primo_esito As Integer = _context.GetOrdinal("SCT_DATA_PRIMO_ESITO")

                        If _context.Read() Then

                            result.IdTest = _context.GetInt64OrDefault(sct_campione_nr)
                            result.Cognome = _context.GetStringOrDefault(sct_cognome)
                            result.Nome = _context.GetStringOrDefault(sct_nome)
                            result.CodiceFiscale = _context.GetStringOrDefault(sct_codice_fiscale)
                            result.DataDiNascita = _context.GetDateTimeOrDefault(sct_data_di_Nascita)
                            result.LuogoDiNascita = _context.GetStringOrDefault(sct_luogo_di_nascita)
                            result.Sesso = _context.GetStringOrDefault(sct_sesso)
                            result.Residenza = _context.GetStringOrDefault(sct_Residenza)
                            result.Centro = _context.GetStringOrDefault(sct_centro)
                            result.UlssRes = _context.GetStringOrDefault(sct_ulss_res)
                            result.UlssRichiedente = _context.GetStringOrDefault(sct_ulss_richiedente)
                            result.DataReferto = _context.GetDateTimeOrDefault(sct_data_referto)
                            result.MotivoEsecuzione = _context.GetStringOrDefault(sct_motivo_esecuzione)
                            result.Cellulare = _context.GetStringOrDefault(sct_cellulare)
                            result.CellulareConsenso = _context.GetStringOrDefault(sct_cellulare_consenso)
                            result.PrimaRilevazione = _context.GetNullableDateTimeOrDefault(sct_primo_esito)
                        End If
                    End Using
                End Using
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return result
        End Function
        Public Function GetTestRapidoByCodiceFiscaleGr(codiceFiscale As String, Gruppo As String) As List(Of TestRapido) Implements IRilevazioniCovid19Provider.GetTestRapidoByCodiceFiscaleGr
            Dim result As List(Of TestRapido) = New List(Of TestRapido)
            Dim ownConnection As Boolean = False
            Dim query As String = "SELECT SCS_CAMPIONE_NR, SCS_COGNOME, SCS_NOME, SCS_CODICE_FISCALE, SCS_DATA_DI_NASCITA, SCS_LUOGO_DI_NASCITA, SCS_SESSO, SCS_RESIDENZA, SCS_DATA_PRELIEVO, SCS_CENTRO, SCS_ULSS_RES, SCS_ULSS_RICHIEDENTE, SCS_DATA_REFERTO, SCS_CELLULARE, SCS_CELLULARE_CONSENSO, SCS_DATA_PRIMO_ESITO " +
                                    "FROM T_ANA_SCARICO_SCREENING " +
                                    "LEFT JOIN T_ANA_LINK_CENTRO_GRUPPOCENTRI ON T_ANA_LINK_CENTRO_GRUPPOCENTRI.LCG_CENTRO = T_ANA_SCARICO_SCREENING.SCS_CENTRO " +
                                    "WHERE SCS_CODICE_FISCALE = :SCS_CODICE_FISCALE AND LCG_GRUPPO = :LCG_GRUPPO"
            Try
                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    cmd.Parameters.AddWithValue("SCS_CODICE_FISCALE", codiceFiscale)
                    cmd.Parameters.AddWithValue("LCG_GRUPPO", Gruppo)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()
                        Dim scs_campione_nr As Integer = _context.GetOrdinal("SCS_CAMPIONE_NR")
                        Dim scs_cognome As Integer = _context.GetOrdinal("SCS_COGNOME")
                        Dim scs_nome As Integer = _context.GetOrdinal("SCS_NOME")
                        Dim scs_codice_fiscale As Integer = _context.GetOrdinal("SCS_CODICE_FISCALE")
                        Dim scs_data_di_Nascita As Integer = _context.GetOrdinal("SCS_DATA_DI_NASCITA")
                        Dim scs_luogo_di_nascita As Integer = _context.GetOrdinal("SCS_LUOGO_DI_NASCITA")
                        Dim scs_sesso As Integer = _context.GetOrdinal("SCS_SESSO")
                        Dim scs_Residenza As Integer = _context.GetOrdinal("SCS_RESIDENZA")
                        Dim scs_DataPrelievo As Integer = _context.GetOrdinal("SCS_DATA_PRELIEVO")
                        Dim scs_centro As Integer = _context.GetOrdinal("SCS_CENTRO")
                        Dim scs_ulss_res As Integer = _context.GetOrdinal("SCS_ULSS_RES")
                        Dim scs_ulss_richiedente As Integer = _context.GetOrdinal("SCS_ULSS_RICHIEDENTE")
                        Dim scs_data_referto As Integer = _context.GetOrdinal("SCS_DATA_REFERTO")
                        Dim scs_cellulare As Integer = _context.GetOrdinal("SCS_CELLULARE")
                        Dim scs_cellulare_consenso As Integer = _context.GetOrdinal("SCS_CELLULARE_CONSENSO")
                        Dim scs_primo_esito As Integer = _context.GetOrdinal("SCS_DATA_PRIMO_ESITO")

                        While _context.Read()
                            Dim test As TestRapido = New TestRapido()
                            test.IdTest = _context.GetInt64OrDefault(scs_campione_nr)
                            test.Cognome = _context.GetStringOrDefault(scs_cognome)
                            test.Nome = _context.GetStringOrDefault(scs_nome)
                            test.CodiceFiscale = _context.GetStringOrDefault(scs_codice_fiscale)
                            test.DataDiNascita = _context.GetDateTimeOrDefault(scs_data_di_Nascita)
                            test.LuogoDiNascita = _context.GetStringOrDefault(scs_luogo_di_nascita)
                            test.Sesso = _context.GetStringOrDefault(scs_sesso)
                            test.Residenza = _context.GetStringOrDefault(scs_Residenza)
                            test.DataPrelievo = _context.GetDateTimeOrDefault(scs_DataPrelievo)
                            test.Centro = _context.GetStringOrDefault(scs_centro)
                            test.UlssRes = _context.GetStringOrDefault(scs_ulss_res)
                            test.UlssRichiedente = _context.GetStringOrDefault(scs_ulss_richiedente)
                            test.DataReferto = _context.GetDateTimeOrDefault(scs_data_referto)
                            test.Cellulare = _context.GetStringOrDefault(scs_cellulare)
                            test.CellulareConsenso = _context.GetStringOrDefault(scs_cellulare_consenso)
                            test.PrimaRilevazione = _context.GetNullableDateTimeOrDefault(scs_primo_esito)

                            result.Add(test)
                        End While
                    End Using
                End Using
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return result
        End Function
        Public Function GetTestRapidoTarByCodiceFiscaleGr(codiceFiscale As String, Gruppo As String) As List(Of TestRapido) Implements IRilevazioniCovid19Provider.GetTestRapidoTarByCodiceFiscaleGr
            Dim result As List(Of TestRapido) = New List(Of TestRapido)
            Dim ownConnection As Boolean = False
            Dim query As String = "SELECT SCT_CAMPIONE_NR, SCT_COGNOME, SCT_NOME, SCT_CODICE_FISCALE, SCT_DATA_DI_NASCITA, SCT_LUOGO_DI_NASCITA, SCT_SESSO, SCT_RESIDENZA, SCT_DATA_PRELIEVO, SCT_CENTRO, SCT_ULSS_RES, SCT_ULSS_RICHIEDENTE, SCT_DATA_REFERTO, SCT_MOTIVO_ESECUZIONE, SCT_CELLULARE, SCT_CELLULARE_CONSENSO, SCT_DATA_PRIMO_ESITO " +
                                    "FROM T_ANA_SCARICO_SCREENING_TAR " +
                                    "LEFT JOIN T_ANA_LINK_CENTRO_GRUPPOCENTRI ON T_ANA_LINK_CENTRO_GRUPPOCENTRI.LCG_CENTRO = T_ANA_SCARICO_SCREENING_TAR.SCT_CENTRO " +
                                    "WHERE SCT_CODICE_FISCALE = :SCT_CODICE_FISCALE AND LCG_GRUPPO = :LCG_GRUPPO"
            Try
                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    cmd.Parameters.AddWithValue("SCT_CODICE_FISCALE", codiceFiscale)
                    cmd.Parameters.AddWithValue("LCG_GRUPPO", Gruppo)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()
                        Dim sct_campione_nr As Integer = _context.GetOrdinal("SCT_CAMPIONE_NR")
                        Dim sct_cognome As Integer = _context.GetOrdinal("SCT_COGNOME")
                        Dim sct_nome As Integer = _context.GetOrdinal("SCT_NOME")
                        Dim sct_codice_fiscale As Integer = _context.GetOrdinal("SCT_CODICE_FISCALE")
                        Dim sct_data_di_Nascita As Integer = _context.GetOrdinal("SCT_DATA_DI_NASCITA")
                        Dim sct_luogo_di_nascita As Integer = _context.GetOrdinal("SCT_LUOGO_DI_NASCITA")
                        Dim sct_sesso As Integer = _context.GetOrdinal("SCT_SESSO")
                        Dim sct_Residenza As Integer = _context.GetOrdinal("SCT_RESIDENZA")
                        Dim sct_DataPrelievo As Integer = _context.GetOrdinal("SCT_DATA_PRELIEVO")
                        Dim sct_centro As Integer = _context.GetOrdinal("SCT_CENTRO")
                        Dim sct_ulss_res As Integer = _context.GetOrdinal("SCT_ULSS_RES")
                        Dim sct_ulss_richiedente As Integer = _context.GetOrdinal("SCT_ULSS_RICHIEDENTE")
                        Dim sct_data_referto As Integer = _context.GetOrdinal("SCT_DATA_REFERTO")
                        Dim sct_motivo_esecuzione As Integer = _context.GetOrdinal("SCT_MOTIVO_ESECUZIONE")
                        Dim sct_cellulare As Integer = _context.GetOrdinal("SCT_CELLULARE")
                        Dim sct_cellulare_consenso As Integer = _context.GetOrdinal("SCT_CELLULARE_CONSENSO")
                        Dim sct_primo_esito As Integer = _context.GetOrdinal("SCT_DATA_PRIMO_ESITO")

                        While _context.Read()
                            Dim test As TestRapido = New TestRapido()
                            test.IdTest = _context.GetInt64OrDefault(sct_campione_nr)
                            test.Cognome = _context.GetStringOrDefault(sct_cognome)
                            test.Nome = _context.GetStringOrDefault(sct_nome)
                            test.CodiceFiscale = _context.GetStringOrDefault(sct_codice_fiscale)
                            test.DataDiNascita = _context.GetDateTimeOrDefault(sct_data_di_Nascita)
                            test.LuogoDiNascita = _context.GetStringOrDefault(sct_luogo_di_nascita)
                            test.Sesso = _context.GetStringOrDefault(sct_sesso)
                            test.Residenza = _context.GetStringOrDefault(sct_Residenza)
                            test.DataPrelievo = _context.GetDateTimeOrDefault(sct_DataPrelievo)
                            test.Centro = _context.GetStringOrDefault(sct_centro)
                            test.UlssRes = _context.GetStringOrDefault(sct_ulss_res)
                            test.UlssRichiedente = _context.GetStringOrDefault(sct_ulss_richiedente)
                            test.DataReferto = _context.GetDateTimeOrDefault(sct_data_referto)
                            test.MotivoEsecuzione = _context.GetStringOrDefault(sct_motivo_esecuzione)
                            test.Cellulare = _context.GetStringOrDefault(sct_cellulare)
                            test.CellulareConsenso = _context.GetStringOrDefault(sct_cellulare_consenso)
                            test.PrimaRilevazione = _context.GetNullableDateTimeOrDefault(sct_primo_esito)

                            result.Add(test)
                        End While
                    End Using
                End Using
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return result
        End Function
        Public Function SalvaDettaglio(dettaglio As DettaglioTestRapido) As ResultSetPost Implements IRilevazioniCovid19Provider.SalvaDettaglio
            Try
                Return DoCommand(Function(cmd)
                                     cmd.UpdateTable("T_ANA_SCARICO_SCREENING",
                                                     New With {
                                                           Key .SCS_DATA_PRELIEVO_TEST = dettaglio.DataPrelievo,
                                                           .SCS_DATA_REFERTO = dettaglio.DataReferto,
                                                           .SCS_TEST_VALIDO = dettaglio.TestValido,
                                                           .SCS_IGG = dettaglio.IGG,
                                                           .SCS_IGM = dettaglio.IGM,
                                                           .SCS_MARCA = dettaglio.Marca,
                                                           .SCS_FOTO = dettaglio.Foto,
                                                           .SCS_UTENTE_REFERTO = dettaglio.IdUtenteRilevazione,
                                                           .SCS_CELLULARE = dettaglio.Cellulare,
                                                           .SCS_CELLULARE_CONSENSO = dettaglio.CellulareConsenso,
                                                           .SCS_DATA_PRIMO_ESITO = dettaglio.PrimaRilevazione.Value
                                                     },
                                                     New With {
                                                           Key .SCS_CAMPIONE_NR = dettaglio.IdTest,
                                                           .SCS_ULSS_RICHIEDENTE = dettaglio.UlssRichiedente
                                                     })
                                     Return New ResultSetPost() With {
                                       .Message = "update eseguito con successo",
                                       .Success = True
                                     }
                                 End Function)

            Catch ex As Exception
                Return New ResultSetPost() With {
                                       .Message = ex.Message,
                                       .Success = False
                                     }
            End Try
        End Function
        Public Function SalvaDettaglioTar(dettaglio As DettaglioAntigeneTestRapido) As ResultSetPost Implements IRilevazioniCovid19Provider.SalvaDettaglioTar
            Try
                Return DoCommand(Function(cmd)
                                     cmd.UpdateTable("T_ANA_SCARICO_SCREENING_TAR",
                                                     New With {
                                                       Key .SCT_DATA_PRELIEVO_TEST = dettaglio.DataPrelievo,
                                                       .SCT_DATA_REFERTO = dettaglio.DataReferto,
                                                       .SCT_TEST_VALIDO = dettaglio.TestValido,
                                                       .SCT_FOTO = dettaglio.Foto,
                                                       .SCT_ESITO_TAR = dettaglio.EsitoTar,
                                                       .SCT_MARCA = dettaglio.Marca,
                                                       .SCT_UTENTE_REFERTO = dettaglio.IdUtenteRilevazione,
                                                       .SCT_CELLULARE = dettaglio.Cellulare,
                                                       .SCT_CELLULARE_CONSENSO = dettaglio.CellulareConsenso,
                                                       .SCT_DATA_PRIMO_ESITO = dettaglio.PrimaRilevazione
                                                     },
                                                     New With {
                                                       Key .SCT_CAMPIONE_NR = dettaglio.IdTest,
                                                       .SCT_ULSS_RICHIEDENTE = dettaglio.UlssRichiedente
                                                     })
                                     Return New ResultSetPost() With {
                                               .Message = "update eseguito con successo",
                                               .Success = True
                                             }
                                 End Function)
            Catch ex As Exception
                Return New ResultSetPost() With {
                                       .Message = ex.Message,
                                       .Success = False
                                     }
            End Try
        End Function
        Sub UpdateUtenteInserimento(idTampone As Long, utenteInserimento As Long) Implements IRilevazioniCovid19Provider.UpdateUtenteInserimento
            Using cmd As OracleCommand = New OracleCommand
                cmd.Connection = Connection
                Dim ownConnection As Boolean = False
                Try
                    ownConnection = ConditionalOpenConnection(cmd)
                    cmd.CommandText = QueryUpdateUtenteInserimentoTampone
                    cmd.Parameters.AddWithValue("PET_UTE_ID_INSERIMENTO", utenteInserimento)
                    cmd.Parameters.AddWithValue("idTampone", idTampone)
                    cmd.ExecuteNonQuery()
                    cmd.Parameters.Clear()

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try
            End Using
        End Sub

#End Region

#Region " Funzionalita per Service App Zero"
        Public Function GetPazientiDaEpisodiConSorveglianzaAttiva() As List(Of AppZeroDatiEpisodio) Implements IRilevazioniCovid19Provider.GetPazientiDaEpisodiConSorveglianzaAttiva
            Dim result As New List(Of AppZeroDatiEpisodio)()

            Dim ownConnection As Boolean = False
            Dim query As String = "SELECT DISTINCT PAZ_CODICE_FISCALE, PES_ID " +
                                            "FROM T_PAZ_EPISODI " +
                                            "INNER JOIN T_PAZ_PAZIENTI ON PES_PAZ_CODICE = PAZ_CODICE " +
                                            "WHERE  PES_SEP_CODICE = 2 AND PAZ_CODICE IS NOT NULL AND PAZ_CODICE_FISCALE IS NOT NULL AND PES_ID IS NOT NULL "
            Try
                Using cmd As OracleCommand = New OracleCommand(query, Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        Dim paz_codice_fiscale As Integer = idr.GetOrdinal("PAZ_CODICE_FISCALE")
                        Dim pes_id As Integer = idr.GetOrdinal("PES_ID")

                        While idr.Read()

                            Dim dati As New AppZeroDatiEpisodio()
                            dati.CodiceFiscale = idr.GetStringOrDefault(paz_codice_fiscale)
                            dati.IdEpisodio = idr.GetInt64OrDefault(pes_id)
                            result.Add(dati)

                        End While
                    End Using
                End Using
            Catch ex As Exception
                SetErrorMsg(ex.Message)
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return result
        End Function

        Public Function SintomiApp() As List(Of SintomiApp) Implements IRilevazioniCovid19Provider.SintomiApp
            Dim result As New List(Of SintomiApp)()
            Dim ownConnection As Boolean = False
            Dim query As String = "SELECT SMP_NOME_SINTOMO_APP, SMP__SINTOMO_SIAVR " +
                                            "FROM T_ANA_SINTOMI_MAPPING "
            Try
                Using cmd As OracleCommand = New OracleCommand(query, Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        Dim SMP_NOME_SINTOMO_APP As Integer = idr.GetOrdinal("SMP_NOME_SINTOMO_APP")
                        Dim SMP__SINTOMO_SIAVR As Integer = idr.GetOrdinal("SMP__SINTOMO_SIAVR")

                        While idr.Read()

                            Dim dati As New SintomiApp()
                            dati.NomeSintomi = idr.GetStringOrDefault(SMP_NOME_SINTOMO_APP)
                            dati.NumeroSintomoSiavr = idr.GetInt64OrDefault(SMP__SINTOMO_SIAVR)
                            result.Add(dati)

                        End While
                    End Using
                End Using
            Catch ex As Exception
                SetErrorMsg(ex.Message)
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return result

        End Function

#End Region
        Public Function PossibiliFocolaiEpisodio(codiceEpisodio As Long) As IEnumerable(Of InfoFocolaio) Implements IRilevazioniCovid19Provider.PossibiliFocolaiEpisodio
            Return DoCommand(Function(cmd)
                                 cmd.CommandText = "SELECT 
                                                        t.PES_ID AS CodiceEpisodio,
                                                        foc.FOC_CODICE AS CodiceFocolaio,
                                                        foc.FOC_DESCRIZIONE AS Descrizione,
                                                        foc.FOC_SCOLASTICO AS Scolastico,
                                                        foc.FOC_PLESSO AS Plesso,
                                                        foc.FOC_ISTITUTO AS Istituto,
                                                        foc.FOC_GIORNI_CHIUSURA AS GiorniChiusura
                                                            FROM T_PAZ_EPISODI e 
                                                            JOIN T_PAZ_EPISODI_CONTATTI c ON c.PEC_PES_ID = e.PES_ID 
                                                            JOIN T_PAZ_EPISODI t ON t.PES_ID = c.PEC_PES_ID_CONTATTO 
                                                            JOIN T_LINK_EPISODI_FOCOLAI lf ON lf.LFE_PES_ID = t.PES_ID 
                                                            JOIN T_ANA_FOCOLAI foc ON foc.FOC_CODICE = lf.LFE_FOC_CODICE 
                                                                WHERE e.PES_ID = ?idEpisodio"
                                 cmd.AddParameter("idEpisodio", codiceEpisodio)
                                 Dim piatta As List(Of FocolaioFlat) = cmd.Fill(Of FocolaioFlat)

                                 Return piatta.GroupBy(Function(flat)
                                                           Return New With {
                                                            Key flat.CodiceFocolaio,
                                                            flat.Descrizione,
                                                            flat.GiorniChiusura,
                                                            flat.Istituto,
                                                            flat.Plesso,
                                                            flat.Scolastico
                                                         }
                                                       End Function).Select(Function(el)
                                                                                Return New InfoFocolaio With {
                                                                                .Scolastico = el.Key.Scolastico,
                                                                                .Plesso = el.Key.Plesso,
                                                                                .CodiceFocolaio = el.Key.CodiceFocolaio,
                                                                                .Descrizione = el.Key.Descrizione,
                                                                                .GiorniChiusura = el.Key.GiorniChiusura,
                                                                                .Istituto = el.Key.Istituto,
                                                                                .Episodi = el.Select(Function(l)
                                                                                                         Return l.CodiceEpisodio
                                                                                                     End Function).ToArray()
                                                                              }
                                                                            End Function).ToArray()
                             End Function)
        End Function

        Public Function FocolaiEpisodio(codiceEpisodio As Long) As IEnumerable(Of InfoFocolaio) Implements IRilevazioniCovid19Provider.FocolaiEpisodio
            Return DoCommand(Function(cmd)
                                 cmd.CommandText = "SELECT 
                                                        foc.FOC_CODICE AS CodiceFocolaio,
                                                        foc.FOC_DESCRIZIONE AS Descrizione,
                                                        foc.FOC_SCOLASTICO AS Scolastico,
                                                        foc.FOC_PLESSO AS Plesso,
                                                        foc.FOC_ISTITUTO AS Istituto,
                                                        foc.FOC_GIORNI_CHIUSURA AS GiorniChiusura
                                                            FROM T_PAZ_EPISODI t
                                                            JOIN T_LINK_EPISODI_FOCOLAI lf ON lf.LFE_PES_ID = t.PES_ID 
                                                            JOIN T_ANA_FOCOLAI foc ON foc.FOC_CODICE = lf.LFE_FOC_CODICE
                                                                WHERE t.PES_ID = ?idEpisodio"
                                 cmd.AddParameter("idEpisodio", codiceEpisodio)

                                 Dim piatti As List(Of InfoFocolaio) = cmd.Fill(Of InfoFocolaio)
                                 Dim parenti As Dictionary(Of Integer, List(Of Long)) = ElencoEpisodiFocolai(piatti.Select(Function(x) x.CodiceFocolaio))

                                 For Each p As InfoFocolaio In piatti
                                     If parenti.ContainsKey(p.CodiceFocolaio) Then
                                         p.Episodi = parenti.Item(p.CodiceFocolaio)
                                     Else
                                         p.Episodi = New List(Of Long)
                                     End If
                                 Next
                                 Return piatti
                             End Function)
        End Function

        Public Function GetInformazioniFocolaio(codiceFocolaio As Integer) As DettagliFocolaio Implements IRilevazioniCovid19Provider.GetInformazioniFocolaio
            Return DoCommand(Function(cmd)
                                 cmd.CommandText = "SELECT 
                                                        l.LFE_FOC_CODICE AS CodiceFocolaio,
                                                        f.FOC_DESCRIZIONE AS Descrizione,
                                                        f.FOC_SCOLASTICO AS Scolastico,
                                                        f.FOC_GIORNI_CHIUSURA AS GiorniChiusura,
                                                        f.FOC_PLESSO AS Plesso,
                                                        f.FOC_ISTITUTO AS Istituto,
                                                        e.PES_ID AS CodiceEpisodio,
                                                        p.PAZ_NOME AS NomePaziente,
                                                        p.PAZ_COGNOME AS CognomePaziente,
                                                        p.PAZ_DATA_NASCITA AS DataDiNascita,
                                                        p.PAZ_CODICE_FISCALE AS CodiceFiscale
                                                            FROM T_LINK_EPISODI_FOCOLAI l
                                                            JOIN T_ANA_FOCOLAI f ON l.LFE_FOC_CODICE = f.FOC_CODICE 
                                                            JOIN T_PAZ_EPISODI e ON e.PES_ID = l.LFE_PES_ID 
                                                            JOIN T_PAZ_PAZIENTI p ON p.PAZ_CODICE = e.PES_PAZ_CODICE 
                                                                where l.LFE_FOC_CODICE = ?codice"
                                 cmd.AddParameter("codice", codiceFocolaio)

                                 Return cmd.Fill(Of DettagliFocolaioFlat).GroupBy(Function(el) New With {
                                                                                        Key el.CodiceFocolaio,
                                                                                        el.Descrizione,
                                                                                        el.Scolastico,
                                                                                        el.Plesso,
                                                                                        el.Istituto,
                                                                                        el.GiorniChiusura
                                                                                      }).Select(Function(el)
                                                                                                    Return New DettagliFocolaio With {
                                                                                                        .GiorniChiusura = el.Key.GiorniChiusura,
                                                                                                        .Istituto = el.Key.Istituto,
                                                                                                        .CodiceFocolaio = el.Key.CodiceFocolaio,
                                                                                                        .Descrizione = el.Key.Descrizione,
                                                                                                        .Plesso = el.Key.Plesso,
                                                                                                        .Scolastico = el.Key.Scolastico,
                                                                                                        .InformazioniEpisodi = el.Select(Function(ep)
                                                                                                                                             Return New DettagliFocolaio.InfoEpisodioFocolaio With {
                                                                                                                                                .CodiceEpisodio = ep.CodiceEpisodio,
                                                                                                                                                .CodiceFiscale = ep.CodiceFiscale,
                                                                                                                                                .CognomePaziente = ep.CognomePaziente,
                                                                                                                                                .DataDiNascita = ep.DataDiNascita,
                                                                                                                                                .NomePaziente = ep.NomePaziente
                                                                                                                                             }
                                                                                                                                         End Function).ToArray()
                                                                                                    }
                                                                                                End Function).FirstOrDefault()
                             End Function)
        End Function

        Public Function GetContattiPositiviLiberi(codiceEpisodio As Long) As IEnumerable(Of ContattoLiberoFocolaio) Implements IRilevazioniCovid19Provider.GetContattiPositiviLiberi
            Return DoCommand(Function(cmd)
                                 cmd.CommandText = "SELECT 
                                                        c.PEC_PES_ID_CONTATTO AS CodiceEpisodio,
                                                        ec.PES_TIPO_CASO,
                                                        p.PAZ_NOME AS NomePaziente,
                                                        p.PAZ_COGNOME AS CognomePaziente,
                                                        p.PAZ_DATA_NASCITA AS DataNascita,
                                                        p.PAZ_CODICE_FISCALE AS CodiceFiscale
                                                        FROM T_PAZ_EPISODI_CONTATTI c
                                                        JOIN T_PAZ_EPISODI eC ON eC.PES_ID = c.PEC_PES_ID_CONTATTO 
                                                        JOIN T_PAZ_PAZIENTI p ON p.PAZ_CODICE = ec.PES_PAZ_CODICE 
                                                        WHERE 
	                                                        c.PEC_PES_ID = ?codice AND eC.PES_TIPO_CASO IS NOT NULL AND upper(ec.PES_TIPO_CASO) = ?tipoCaso
	                                                        AND NOT EXISTS (SELECT 1 FROM T_LINK_EPISODI_FOCOLAI l WHERE l.LFE_PES_ID = ec.PES_ID)"
                                 cmd.AddParameter("codice", codiceEpisodio)
                                 cmd.AddParameter("tipoCaso", "CP")
                                 Return cmd.Fill(Of ContattoLiberoFocolaio)
                             End Function)
        End Function

        Public Sub AbbinaFocolaioEpisodio(codiceEpisodio As Long, codiceFocolaio As Integer) Implements IRilevazioniCovid19Provider.AbbinaFocolaioEpisodio
            DoCommand(Sub(cmd)
                          cmd.InsertInTable("T_LINK_EPISODI_FOCOLAI", New With {
                                                Key .LFE_FOC_CODICE = codiceFocolaio,
                                                .LFE_PES_ID = codiceEpisodio
                                            })
                      End Sub)
        End Sub

        Public Function CreaFocolaio(model As CreaFocolaio) As Long Implements IRilevazioniCovid19Provider.CreaFocolaio
            Return DoCommand(Function(cmd)
                                 Dim codice As Long = cmd.InsertInTable(Of Long)("T_ANA_FOCOLAI", New With {
                                                                           Key .FOC_DESCRIZIONE = model.Descrizione,
                                                                           .FOC_SCOLASTICO = model.Scolastico,
                                                                           .FOC_PLESSO = model.Plesso,
                                                                           .FOC_ISTITUTO = model.Istituto,
                                                                           .FOC_GIORNI_CHIUSURA = model.GiorniChiusura
                                                                        }, "FOC_CODICE")

                                 For Each a As Long In model.Episodi
                                     Me.AbbinaFocolaioEpisodio(a, codice)
                                 Next
                                 Return codice
                             End Function, IsolationLevel.ReadCommitted)
        End Function

        Public Function GetTestataEpisodi(codiceEpisodi As IEnumerable(Of Long)) As IEnumerable(Of TestataEpisodioCovid) Implements IRilevazioniCovid19Provider.GetTestataEpisodi
            If codiceEpisodi Is Nothing OrElse Not codiceEpisodi.Any() Then
                Return New List(Of TestataEpisodioCovid)
            End If
            Return DoCommand(Function(cmd)
                                 cmd.CommandText = String.Format("SELECT 
                                                        PES_ID as CodiceEpisodio,
                                                        paz_codice AS CodicePaziente,
                                                        paz_nome AS NomePaziente,
                                                        paz_cognome AS CognomePaziente,
                                                        PES_TELEFONO AS telefono,
                                                        PES_EMAIL AS email,
                                                        PES_COM_CODICE_ISOLAMENTO AS CodiceComuneIsolamento,
                                                        isolamento.COM_DESCRIZIONE AS DescrizioneComuneIsolamento,
                                                        PES_COM_CODICE AS CodiceComuneEsposizione,
                                                        esposizione.COM_DESCRIZIONE AS DescrizioneComuneEsposizione,
                                                        PES_INDIRIZZO_ISOLAMENTO AS IndirizzoIsolamento,
                                                        PES_INTERNO AS InternoRegione,
                                                        PES_TIPO_CASO AS CodiceTipoCaso,
                                                        tipoCaso.COD_DESCRIZIONE AS DescrizioneTipoCaso,
                                                        PES_ATC_CODICE AS CodiceTipoContatto,
                                                        tipoContatto.ATC_DESCRIZIONE AS DescrizioneTipoContatto,
                                                        PES_ASE_CODICE AS CodiceFonteSegnalatore,
                                                        fonte.ASE_DESCRIZIONE AS DescrizioneFonteSegnalatore,
                                                        PES_SEP_CODICE AS CodiceStato,
                                                        STATI.SEP_DESCRIZIONE AS DescrizioneStato,
                                                        PES_USL_CODICE_RACCOLTA AS CodiceUlssIncaricata,
                                                        PES_DATA_DECESSO_COVID as DataDecessoCovid,
                                                        uslIncarico.USL_DESCRIZIONE AS DescrizioneUlssIncaricata,
                                                        PES_DATA_INIZIO_ISOLAMENTO DataInizioSorveglianza, 
                                                        PES_DATA_FINE_ISOLAMENTO DataFineSorveglianza,
                                                        PES_NOTE as Note
                                                            FROM T_PAZ_EPISODI 
                                                                JOIN T_PAZ_PAZIENTI paz ON paz.PAZ_CODICE = pes_paz_codice
                                                                LEFT JOIN T_ANA_COMUNI isolamento ON isolamento.COM_CODICE = pes_com_codice_isolamento
                                                                LEFT JOIN t_ana_comuni esposizione ON ESPOSIZIONE.COM_CODICE = pes_com_codice
                                                                LEFT JOIN t_ana_codifiche tipoCaso ON pes_tipo_caso = tipoCaso.COD_CODICE AND tipoCaso.COD_CAMPO = 'PES_TIPO_CASO'
                                                                LEFT JOIN T_ANA_TIPO_CONTATTO tipoContatto ON tipoContatto.ATC_CODICE = pes_atc_codice
                                                                LEFT JOIN T_ANA_SEGNALATORE fonte ON fonte.ASE_CODICE = pes_ase_codice
                                                                LEFT JOIN T_ANA_STATI_EPISODIO stati ON stati.SEP_CODICE = PES_SEP_CODICE 
                                                                LEFT JOIN t_ana_usl uslIncarico ON uslIncarico.usl_codice = pes_usl_codice_raccolta
                                                                    WHERE pes_id IN ({0})", cmd.SetParameterIn("C", codiceEpisodi))
                                 Return cmd.Fill(Of TestataEpisodioCovid)
                             End Function)
        End Function

        Public Sub SalvaTestateEpisodi(tmp As List(Of SalvaTestataEpisodioCovid)) Implements IRilevazioniCovid19Provider.SalvaTestateEpisodi
            If Not tmp.Any() Then
                Return
            End If
            If tmp.Count = 1 Then
                DoCommand(Sub(cmd)
                              For Each el As SalvaTestataEpisodioCovid In tmp
                                  cmd.UpdateTable("T_PAZ_EPISODI", New With {
                                        Key .PES_TIPO_CASO = el.CodiceTipoCaso,
                                        .PES_INTERNO = el.InternoRegione,
                                        .PES_COM_CODICE = el.CodiceComuneEsposizione,
                                        .PES_SEP_CODICE = el.CodiceStato,
                                        .PES_DATA_DECESSO_COVID = el.DataDecessoCovid,
                                        .PES_DATA_INIZIO_ISOLAMENTO = el.DataInizioSorveglianza,
                                        .PES_DATA_FINE_ISOLAMENTO = el.DataFineSorveglianza,
                                        .PES_NOTE = el.Note
                            }, New With {Key .PES_ID = el.CodiceEpisodio})
                              Next
                          End Sub, IsolationLevel.ReadCommitted)
            Else
                DoCommand(Sub(cmd)
                              Dim condizioni As New List(Of String)

                              Dim el As SalvaTestataEpisodioCovid = tmp.First()
                              Dim campi As New List(Of String)
                              If Not String.IsNullOrWhiteSpace(el.CodiceTipoCaso) Then
                                  DoCommand(Sub(c)
                                                c.CommandText = String.Format("update T_PAZ_EPISODI set PES_TIPO_CASO = ?tipo where PES_TIPO_CASO <> ?pos and PES_ID In ({0})", c.SetParameterIn("I", tmp.Select(Function(x)
                                                                                                                                                                                                                     Return x.CodiceEpisodio
                                                                                                                                                                                                                 End Function)))
                                                c.AddParameter("tipo", el.CodiceTipoCaso)
                                                c.AddParameter("pos", "CP")
                                                c.ExecuteNonQuery()
                                            End Sub, IsolationLevel.ReadCommitted)
                              End If
                              If el.CodiceStato.HasValue Then
                                  DoCommand(Sub(c)
                                                c.CommandText = String.Format("SELECT PES_ID FROM T_PAZ_EPISODI 
                                                                    LEFT JOIN T_ANA_STATI_EPISODIO tase ON PES_SEP_CODICE = tase.SEP_CODICE 
                                                                    WHERE (SEP_ATTIVO = 'S' or PES_SEP_CODICE is null) and PES_ID IN ({0})", c.SetParameterIn("I", tmp.Select(Function(x)
                                                                                                                                                                                  Return x.CodiceEpisodio
                                                                                                                                                                              End Function)))
                                                Dim codici As List(Of Long) = c.Fill(Of Long)
                                                c.Parameters.Clear()
                                                If codici.Any() Then
                                                    c.CommandText = String.Format("update T_PAZ_EPISODI SET PES_SEP_CODICE = ?stato where PES_ID IN ({0})", c.SetParameterIn("I", codici))
                                                    c.AddParameter("stato", el.CodiceStato.Value)
                                                    c.ExecuteNonQuery()
                                                End If
                                            End Sub, IsolationLevel.ReadCommitted)
                              End If
                              If Not String.IsNullOrWhiteSpace(el.InternoRegione) Then
                                  campi.Add("PES_INTERNO = ?interno")
                                  cmd.AddParameter("interno", el.InternoRegione)
                              End If
                              If Not String.IsNullOrWhiteSpace(el.CodiceComuneEsposizione) Then
                                  campi.Add("PES_COM_CODICE = ?comune")
                                  cmd.AddParameter("comune", el.CodiceComuneEsposizione)
                              End If
                              If el.DataDecessoCovid.HasValue Then
                                  campi.Add("PES_DATA_DECESSO_COVID = ?dataDecesso")
                                  cmd.AddParameter("dataDecesso", el.DataDecessoCovid.Value)
                              End If
                              If el.DataInizioSorveglianza.HasValue Then
                                  campi.Add("PES_DATA_INIZIO_ISOLAMENTO = ?dataInizio")
                                  cmd.AddParameter("dataInizio", el.DataInizioSorveglianza.Value)
                              End If
                              If el.DataFineSorveglianza.HasValue Then
                                  campi.Add("PES_DATA_FINE_ISOLAMENTO = ?dataFine")
                                  cmd.AddParameter("dataFine", el.DataFineSorveglianza.Value)
                              End If
                              If Not String.IsNullOrWhiteSpace(el.Note) Then
                                  campi.Add("PES_NOTE = ?note")
                                  cmd.AddParameter("note", el.Note)
                              End If

                              If campi.Any() Then
                                  condizioni.Add(String.Format("PES_ID IN ({0})", cmd.SetParameterIn("I", tmp.Select(Function(x)
                                                                                                                         Return x.CodiceEpisodio
                                                                                                                     End Function))))

                                  Dim sql As New StringBuilder()
                                  sql.Append("update T_PAZ_EPISODI set ")
                                  sql.Append(String.Join(", ", campi))
                                  sql.Append(String.Format(" WHERE {0}", String.Join(" AND ", condizioni)))
                                  cmd.CommandText = sql.ToString()
                                  cmd.ExecuteNonQuery()
                              End If

                          End Sub, IsolationLevel.ReadCommitted)
            End If
        End Sub


        Private Function ElencoEpisodiFocolai(codiceEpisodio As IEnumerable(Of Integer)) As Dictionary(Of Integer, List(Of Long))
            If IsNothing(codiceEpisodio) OrElse Not codiceEpisodio.Any() Then
                Return New Dictionary(Of Integer, List(Of Long))
            End If
            Return DoCommand(Function(cmd)
                                 Dim result As New Dictionary(Of Integer, List(Of Long))
                                 cmd.CommandText = String.Format("Select LFE_FOC_CODICE, LFE_PES_ID from T_LINK_EPISODI_FOCOLAI where LFE_FOC_CODICE IN ({0})", cmd.SetParameterIn("F", codiceEpisodio))
                                 Using reader As IDataReader = cmd.ExecuteReader()
                                     Dim foc As Integer = reader.GetOrdinal("LFE_FOC_CODICE")
                                     Dim epp As Integer = reader.GetOrdinal("LFE_PES_ID")
                                     While reader.Read()
                                         Dim codiceFocolaio As Integer = reader.GetInt32(foc)
                                         If result.ContainsKey(codiceFocolaio) Then
                                             Dim elenco As List(Of Long) = result.Item(codiceFocolaio)
                                             elenco.Add(reader.GetInt64(epp))
                                         Else
                                             Dim elenco As New List(Of Long)
                                             elenco.Add(reader.GetInt64(epp))
                                             result.Add(codiceFocolaio, elenco)
                                         End If
                                     End While
                                 End Using
                                 Return result
                             End Function)
        End Function

        Public Function SalvaElaborazioniContatti(dati As IEnumerable(Of ImportazioneContatti)) As String Implements IRilevazioniCovid19Provider.SalvaElaborazioniContatti
            Dim codiceGruppo As String = Guid.NewGuid().ToString()
            Dim data As DateTime = DateTime.Now
            DoCommand(Sub(cmd)
                          For Each el As ImportazioneContatti In dati
                              cmd.InsertInTable("T_ELAB_CONTATTI_EPISODI", New With {
                                            Key .ECE_DATA = data,
                                            .ECE_COGNOME = el.Cognome,
                                            .ECE_NOME = el.Nome,
                                            .ECE_COMUNE = el.Comune,
                                            .ECE_SESSO = el.Sesso,
                                            .ECE_CODICE_FISCALE = el.CodiceFiscale,
                                            .ECE_CELLULARE = el.Cellulare,
                                            .ECE_CODICE_MECCANO = el.CodiceMeccanografico,
                                            .ECE_CODICE_SCUOLA = el.CodiceFiscaleScuola,
                                            .ECE_CLASSE = el.Classe,
                                            .ECE_TIPO = el.Tipo,
                                            .ECE_GREZZO = el.Grezzo,
                                            .ECE_UTE_ID = el.CodiceUtente,
                                            .ECE_GRUPPO = codiceGruppo})
                          Next
                      End Sub)
            Return codiceGruppo
        End Function

        Public Function GetPossibiliContatti(codiciFiscali As IEnumerable(Of String)) As IEnumerable(Of InfoPossibileContatto) Implements IRilevazioniCovid19Provider.GetPossibiliContatti
            Return DoCommand(Function(cmd)
                                 cmd.CommandText = String.Format("select PAZ_CODICE as Codice, PAZ_COGNOME Cognome,
                                               PAZ_NOME Nome,
                                               PAZ_CODICE_FISCALE Cf,
                                               PAZ_DATA_NASCITA DataNascita,
                                               PAZ_SESSO Sesso,
                                               COMUNE_NASCITA.COM_DESCRIZIONE ComuneNascita,
                                               COMUNE_RESIDENZA.COM_DESCRIZIONE ComuneResidenza,
                                               PAZ_INDIRIZZO_RESIDENZA IndirizzoResidenza,
                                               COMUNE_DOMICILIO.COM_DESCRIZIONE ComuneDomicilio,
                                               PAZ_INDIRIZZO_DOMICILIO IndirizzoDomicilio,
                                               PAZ_TELEFONO_1 TelefonoUno,
                                               PAZ_TELEFONO_2 TelefonoDue,
                                               PAZ_TELEFONO_3 TelefonoTre,
                                               PAZ_EMAIL Email,
                                               MED_NOME NomeMedico,
                                               MED_COGNOME CognomeMedico,
                                               MED_CODICE_FISCALE CfMedico,
                                               pes_id AS CodiciEpisodi
                                                from T_PAZ_PAZIENTI
                                                  left outer join T_ANA_COMUNI COMUNE_NASCITA on PAZ_COM_CODICE_NASCITA = COMUNE_NASCITA.COM_CODICE
                                                  left outer join T_ANA_COMUNI COMUNE_RESIDENZA on PAZ_COM_CODICE_RESIDENZA = COMUNE_RESIDENZA.COM_CODICE
                                                  left outer join T_ANA_COMUNI COMUNE_DOMICILIO on PAZ_COM_CODICE_DOMICILIO = COMUNE_DOMICILIO.COM_CODICE
                                                  left outer join T_ANA_MEDICI on PAZ_MED_CODICE_BASE = MED_CODICE
                                                  LEFT OUTER JOIN t_paz_episodi ON PES_PAZ_CODICE = paz_codice
                                                    where PAZ_CODICE_FISCALE IN ({0})", cmd.SetParameterIn("P", codiciFiscali.Select(Function(x) x.ToUpper()).Distinct()))
                                 Return cmd.Fill(Of InfoPossibileContatto)
                             End Function)
        End Function

        Public Sub AggiornaStatoSorveglianzaEpisodiDaRicovero(dataRicovero As Date, codiceTipoRicovero As Long, codiciEpisodi() As Long) Implements IRilevazioniCovid19Provider.AggiornaStatoSorveglianzaEpisodiDaRicovero
            Dim giorniDopoDimissione As Integer = 7
            Dim descrizioneDimissione As String() = {"dimissione", "dimesso in isolamento", "dimesso negativizzato", "dimesso non specificato"}
            Dim descrizioneDecesso As String() = {"decesso", "deceduto"}
            DoCommand(Sub(cmd)
                          cmd.CommandText = "select ATR_SEP_CODICE from T_ANA_TIPI_RICOVERO where ATR_CODICE = ?codice"
                          cmd.AddParameter("codice", codiceTipoRicovero)
                          Dim codiceStato As Integer? = cmd.FirstOrDefault(Of Integer?)
                          cmd.CommandText = "select ATR_DESCRIZIONE from T_ANA_TIPI_RICOVERO where ATR_CODICE = ?codice"
                          Dim descrizioneStato As String = cmd.FirstOrDefault(Of String)

                          If codiceStato.HasValue AndAlso codiciEpisodi.Any() Then
                              DoCommand(Sub(c)
                                            c.CommandText = String.Format("update T_PAZ_EPISODI set PES_SEP_CODICE = ?codice where PES_ID in ({0})", c.SetParameterIn("ID", codiciEpisodi))
                                            c.AddParameter("codice", codiceStato)
                                            c.ExecuteNonQuery()
                                        End Sub, IsolationLevel.ReadCommitted)
                              If Not String.IsNullOrWhiteSpace(descrizioneStato) Then
                                  If descrizioneDimissione.Contains(descrizioneStato.ToLower()) Then
                                      ' è disattivato perchè non lo vogliono, imposta la data di fine isolamento se il paziente viene dimesso
                                      'DoCommand(Sub(c)
                                      '              c.CommandText = String.Format("update T_PAZ_EPISODI set PES_DATA_FINE_ISOLAMENTO = ?data where PES_ID in ({0})", c.SetParameterIn("ID", codiciEpisodi))
                                      '              c.AddParameter("data", dataRicovero.AddDays(giorniDopoDimissione).Date)
                                      '              c.ExecuteNonQuery()
                                      '          End Sub, IsolationLevel.ReadCommitted)

                                  ElseIf descrizioneDecesso.Contains(descrizioneStato.ToLower()) Then
                                      DoCommand(Sub(c)
                                                    c.CommandText = String.Format("update T_PAZ_EPISODI set PES_DATA_DECESSO_COVID = ?data where PES_ID in ({0})", c.SetParameterIn("ID", codiciEpisodi))
                                                    c.AddParameter("data", dataRicovero.Date)
                                                    c.ExecuteNonQuery()
                                                End Sub)
                                  End If
                              End If

                          End If
                      End Sub, IsolationLevel.ReadCommitted)
        End Sub

        Public Function GetCodiciLaboratorio(q As String) As IEnumerable(Of CodiceLaboratorioTampone) Implements IRilevazioniCovid19Provider.GetCodiciLaboratorio
            Return DoCommand(Function(cmd)
                                 cmd.CommandText = "select TLM_ID as Codice, TLM_CODICE_LAB as Descrizione from T_ANA_LABORATORI_MICRO"
                                 If Not String.IsNullOrWhiteSpace(q) Then
                                     cmd.CommandText += " where UPPER(TLM_CODICE_LAB) like ?q"
                                     cmd.AddParameter("q", String.Format("{0}%", q.ToUpper()))
                                 End If
                                 Return cmd.Fill(Of CodiceLaboratorioTampone)
                             End Function)
        End Function

        Class DettagliFocolaioFlat
            Public Property CodiceFocolaio As Integer
            Public Property Descrizione As String
            Public Property Scolastico As Boolean
            Public Property Plesso As String
            Public Property Istituto As String
            Public Property GiorniChiusura As Integer?


            Public Property CodiceEpisodio As Long
            Public Property NomePaziente As String
            Public Property CognomePaziente As String
            Public Property DataDiNascita As Date?
            Public Property CodiceFiscale As String
        End Class

        Class FocolaioFlat
            Public Property CodiceEpisodio As Long
            Public Property CodiceFocolaio As Integer
            Public Property Descrizione As String
            Public Property Scolastico As Boolean
            Public Property Plesso As String
            Public Property Istituto As String
            Public Property GiorniChiusura As Integer?
        End Class

        Class FlatDiaria
            Public CodiceDiaria As Long
            Public Note As String
            Public DataRilevazione As Date
            Public CodiceRilevazioneUsl As String
            Public UtenteInserimento As Long
            Public CodiceInserimentoUsl As String
            Public DataInserimento As DateTime
            Public RispostaTelefono As Boolean
            Public UtenteModifica As Long?
            Public DataModifica As DateTime?
            Public Property CodiceEpisodio As Long
            Public CodiceSintomo As Integer?
            Public DescrizioneSintomo As String
        End Class

    End Class

End Namespace
