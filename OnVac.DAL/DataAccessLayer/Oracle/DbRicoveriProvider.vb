Imports System.Collections.Generic
Imports System.Data.OracleClient
Imports Onit.Database.DataAccessManager

Namespace DAL.Oracle
    Public Class DbRicoveriProvider
        Inherits DbProvider
        Implements IDbRicoveriProvider

        Public Sub New(ByRef mad As IDAM)
            MyBase.New(mad)
        End Sub

        Public Function SalvaRicovero(command As SalvaRicovero, codiceUtente As Long) As RicoveroPaziente Implements IDbRicoveriProvider.SalvaRicovero
            Return DoCommand(Function(cmd)
                                 If String.IsNullOrWhiteSpace(command.CodiceGruppo) Then
                                     cmd.CommandText = "select S_T_RICOVERI_GRUPPO.nextval from dual"
                                     command.CodiceGruppo = cmd.First(Of Long).ToString()
                                 End If


                                 EliminaEventiNonNecessari(command.CodiceGruppo, command.Episodi.Where(Function(x)
                                                                                                           Return x.Codice.HasValue
                                                                                                       End Function).Select(Function(x)
                                                                                                                                Return x.Codice.Value
                                                                                                                            End Function), codiceUtente)

                                 For Each ev As SalvaRicovero.EpisodioRicovero In command.Episodi
                                     If ev.Codice.HasValue Then
                                         cmd.UpdateTable("T_RICOVERI_PAZIENTE", New With {
                                            .RIP_ATR_CODICE = ev.CodiceTipoEvento,
                                            .RIP_DATA_INGRESSO = ev.DataIngresso,
                                            .RIP_DATA_USCITA = ev.DataUscita,
                                            .RIP_STC = ev.CodiceStatoClinico,
                                            .RIP_HSP_ID = ev.CodiceStruttura,
                                            .RIP_UOP = ev.CodiceUnitaOperativa,
                                            .RIP_ARE_CODICE = ev.CodiceArea,
                                            .RIP_UTE_ID_MODIFICA = codiceUtente,
                                            .RIP_DATA_MODIFICA = DateTime.Now
                                         }, New With {.RIP_ID = ev.Codice})
                                     Else
                                         ev.Codice = cmd.InsertInTable(Of Long)("T_RICOVERI_PAZIENTE", New With {
                                             Key .RIP_CODICE_GRUPPO = command.CodiceGruppo,
                                            .RIP_PAZ_CODICE = command.CodicePaziente,
                                            .RIP_ATR_CODICE = ev.CodiceTipoEvento,
                                            .RIP_DATA_INGRESSO = ev.DataIngresso,
                                            .RIP_DATA_USCITA = ev.DataUscita,
                                            .RIP_STC = ev.CodiceStatoClinico,
                                            .RIP_HSP_ID = ev.CodiceStruttura,
                                            .RIP_UOP = ev.CodiceUnitaOperativa,
                                            .RIP_ARE_CODICE = ev.CodiceArea,
                                            .RIP_UTE_ID_INSERIMENTO = codiceUtente,
                                            .RIP_DATA_INSERIMENTO = DateTime.Now
                                         }, "RIP_ID")

                                     End If

                                 Next

                                 Return _getRicovero(command.CodiceGruppo)

                             End Function, IsolationLevel.ReadCommitted)
        End Function

        Public Sub EliminaRicovero(codiceGruppo As String, codiceUtente As Long) Implements IDbRicoveriProvider.EliminaRicovero
            EliminaEventiNonNecessari(codiceGruppo, Nothing, codiceUtente)
        End Sub

        Private Sub EliminaEventiNonNecessari(codiceGruppo As String, codiciEventi As IEnumerable(Of Long), codiceUtente As Long)
            DoCommand(Sub(cmd)
                          If codiciEventi Is Nothing OrElse Not codiciEventi.Any() Then
                              cmd.UpdateTable("T_RICOVERI_PAZIENTE", New With {
                                                Key .RIP_DATA_ELIMINAZIONE = DateTime.Now,
                                                .RIP_UTE_ID_ELIMINAZIONE = codiceUtente
                                              },
                                              New With {
                                                Key .RIP_CODICE_GRUPPO = codiceGruppo
                                              })
                              Return
                          End If

                          cmd.CommandText = "SELECT RIP_ID FROM T_RICOVERI_PAZIENTE WHERE RIP_CODICE_GRUPPO = ?COD"
                          cmd.AddParameter("COD", codiceGruppo)
                          Dim codiciEsistenti As List(Of Long) = cmd.Fill(Of Long)


                          cmd.Parameters.Clear()

                          Dim daEliminare As List(Of Long) = codiciEsistenti.Where(Function(x)
                                                                                       Return Not codiciEventi.Contains(x)
                                                                                   End Function).ToList()

                          If daEliminare.Any() Then
                              cmd.CommandText = String.Format("update T_RICOVERI_PAZIENTE set RIP_DATA_ELIMINAZIONE = ?data, RIP_UTE_ID_ELIMINAZIONE = ?ute where RIP_ID IN ({0})", cmd.SetParameterIn("D", daEliminare))
                              cmd.AddParameter("data", DateTime.Now)
                              cmd.AddParameter("ute", codiceUtente)
                              cmd.ExecuteNonQuery()
                          End If


                      End Sub)
        End Sub

        Public Function GetRicoveriPaziente(codicePaziente As Long) As IEnumerable(Of RicoveroPaziente) Implements IDbRicoveriProvider.GetRicoveriPaziente
            Return DoCommand(Function(cmd)
                                 cmd.CommandText = "SELECT 
	                                    RIP_ID ,
	                                    RIP_CODICE_GRUPPO ,
	                                    RIP_PAZ_CODICE ,
	                                    RIP_ATR_CODICE ,
	                                    atr_descrizione,
                                        atr_chiuso,
                                        atr_aperto,
	                                    RIP_DATA_INGRESSO ,
	                                    RIP_DATA_USCITA ,
	                                    RIP_STC ,
	                                    stati.COD_DESCRIZIONE AS RIP_STC_DESCRIZIONE,
	                                    RIP_HSP_ID ,
                                        HSP_DESCRIZIONE,
	                                    RIP_UOP ,
	                                    unita.COD_DESCRIZIONE AS RIP_UOP_DESCRIZIONE,
	                                    RIP_ARE_CODICE ,
	                                    are_descrizione,
                                        RIP_REPARTO_DESCRIZIONE
                                    FROM T_RICOVERI_PAZIENTE trp 
                                    LEFT JOIN T_ANA_TIPI_RICOVERO tatr ON trp.RIP_ATR_CODICE = ATR_CODICE 
                                    LEFT JOIN t_ana_codifiche stati ON rip_stc = stati.COD_CODICE AND stati.COD_CAMPO  = 'RIP_STC'
                                    LEFT JOIN t_ana_codifiche unita ON RIP_UOP = unita.COD_CODICE AND unita.COD_CAMPO = 'RIP_UOP'
                                    LEFT JOIN T_ANA_REPARTI tar ON rip_are_codice = ARE_CODICE 
                                    LEFT JOIN T_ANA_CODICI_HSP ON HSP_ID = rip_hsp_id
                                    where RIP_PAZ_CODICE = ?paz and RIP_DATA_ELIMINAZIONE is null"
                                 cmd.AddParameter("paz", codicePaziente)
                                 Return _readRicovero(cmd).ToList()
                             End Function)
        End Function

        Private Function _getRicovero(idGruppo As String) As RicoveroPaziente
            Return DoCommand(Function(Command)
                                 Command.CommandText = "SELECT 
	                                    RIP_ID ,
	                                    RIP_CODICE_GRUPPO ,
	                                    RIP_PAZ_CODICE ,
	                                    RIP_ATR_CODICE ,
	                                    atr_descrizione,
	                                    RIP_DATA_INGRESSO ,
	                                    RIP_DATA_USCITA ,
	                                    RIP_STC ,
	                                    stati.COD_DESCRIZIONE AS RIP_STC_DESCRIZIONE,
	                                    RIP_HSP_ID ,
                                        HSP_DESCRIZIONE,
	                                    RIP_UOP ,
	                                    unita.COD_DESCRIZIONE AS RIP_UOP_DESCRIZIONE,
	                                    RIP_ARE_CODICE ,
	                                    are_descrizione,
                                        RIP_REPARTO_DESCRIZIONE
                                    FROM T_RICOVERI_PAZIENTE trp 
                                    LEFT JOIN T_ANA_TIPI_RICOVERO tatr ON trp.RIP_ATR_CODICE = ATR_CODICE 
                                    LEFT JOIN t_ana_codifiche stati ON rip_stc = stati.COD_CODICE AND stati.COD_CAMPO  = 'RIP_STC'
                                    LEFT JOIN t_ana_codifiche unita ON RIP_UOP = unita.COD_CODICE AND unita.COD_CAMPO = 'RIP_UOP'
                                    LEFT JOIN T_ANA_REPARTI tar ON rip_are_codice = ARE_CODICE 
                                    LEFT JOIN T_ANA_CODICI_HSP ON HSP_ID = rip_hsp_id
                                    where RIP_CODICE_GRUPPO = ?cod and RIP_DATA_ELIMINAZIONE is null"
                                 Command.AddParameter("cod", idGruppo)

                                 Return _readRicovero(Command).FirstOrDefault(Function(x)
                                                                                  Return x.CodiceGruppo = idGruppo
                                                                              End Function)

                             End Function)
        End Function

        Private Function _readRicovero(Command As IDbCommand) As List(Of RicoveroPaziente)
            Return Command.Fill(Of RicoveroFlat).GroupBy(Function(x)
                                                             Return New With {Key x.CodiceGruppo, x.CodicePaziente}
                                                         End Function).Select(Function(x)
                                                                                  Return New RicoveroPaziente With {
                                                                                       .CodiceGruppo = x.Key.CodiceGruppo,
                                                                                       .CodicePaziente = x.Key.CodicePaziente,
                                                                                       .Episodi = x.Select(Function(f)
                                                                                                               Return DirectCast(f, RicoveroPaziente.EpisodioRicovero)
                                                                                                           End Function).ToList()
                                                                                  }
                                                                              End Function).ToList()
        End Function
        Private Function _readRicovero_Old(reader As IDataReader) As List(Of RicoveroPaziente)
            Dim ritorno As New List(Of RicoveroPaziente)

            Dim RIP_ID As Integer = reader.GetOrdinal("RIP_ID")
            Dim RIP_CODICE_GRUPPO As Integer = reader.GetOrdinal("RIP_CODICE_GRUPPO")
            Dim RIP_PAZ_CODICE As Integer = reader.GetOrdinal("RIP_PAZ_CODICE")
            Dim RIP_ATR_CODICE As Integer = reader.GetOrdinal("RIP_ATR_CODICE")
            Dim ATR_DESCRIZIONE As Integer = reader.GetOrdinal("ATR_DESCRIZIONE")
            Dim DATA_INGRESSO As Integer = reader.GetOrdinal("RIP_DATA_INGRESSO")
            Dim DATA_USCITA As Integer = reader.GetOrdinal("RIP_DATA_USCITA")
            Dim RIP_STC As Integer = reader.GetOrdinal("RIP_STC")
            Dim RIP_STC_DESCRIZIONE As Integer = reader.GetOrdinal("RIP_STC_DESCRIZIONE")
            Dim RIP_HSP_ID As Integer = reader.GetOrdinal("RIP_HSP_ID")
            Dim RIP_UOP As Integer = reader.GetOrdinal("RIP_UOP")
            Dim RIP_UOP_DESCRIZIONE As Integer = reader.GetOrdinal("RIP_UOP_DESCRIZIONE")
            Dim RIP_ARE_CODICE As Integer = reader.GetOrdinal("RIP_ARE_CODICE")
            Dim ARE_DESCRIZIONE As Integer = reader.GetOrdinal("ARE_DESCRIZIONE")
            Dim HSP_DESCRIZIONE As Integer = reader.GetOrdinal("HSP_DESCRIZIONE")

            While reader.Read()
                Dim gruppo As String = reader.GetString(RIP_CODICE_GRUPPO)
                Dim el As RicoveroPaziente = ritorno.FirstOrDefault(Function(x)
                                                                        Return x.CodiceGruppo.Equals(gruppo)
                                                                    End Function)
                If el Is Nothing Then
                    el = New RicoveroPaziente() With {
                        .CodiceGruppo = gruppo,
                        .CodicePaziente = reader.GetInt64(RIP_PAZ_CODICE),
                        .Episodi = New List(Of RicoveroPaziente.EpisodioRicovero)
                    }
                    ritorno.Add(el)
                End If

                Dim nuovo As New RicoveroPaziente.EpisodioRicovero With {
                    .Codice = reader.GetInt64(RIP_ID),
                    .CodiceArea = reader.GetNullableInt32OrDefault(RIP_ARE_CODICE),
                    .CodiceStatoClinico = reader.GetString(RIP_STC),
                    .CodiceStruttura = reader.GetString(RIP_HSP_ID),
                    .CodiceTipoEvento = reader.GetInt32(RIP_ATR_CODICE),
                    .CodiceUnitaOperativa = reader.GetStringOrDefault(RIP_UOP),
                    .DataIngresso = reader.GetDateTime(DATA_INGRESSO),
                    .DataUscita = reader.GetNullableDateTimeOrDefault(DATA_USCITA),
                    .DescrizioneArea = reader.GetStringOrDefault(ARE_DESCRIZIONE),
                    .DescrizioneStatoClinico = reader.GetStringOrDefault(RIP_STC_DESCRIZIONE),
                    .DescrizioneTipoEvento = reader.GetStringOrDefault(ATR_DESCRIZIONE),
                    .DescrizioneUnitaOperativa = reader.GetStringOrDefault(RIP_UOP_DESCRIZIONE),
                    .DescrizioneStruttura = reader.GetStringOrDefault(HSP_DESCRIZIONE)
                }
                el.Episodi.Add(nuovo)

            End While

            Return ritorno
        End Function

        Public Function CercaRicoveri(filtri As FiltriRicoveri) As IEnumerable(Of TestataRicovero) Implements IDbRicoveriProvider.CercaRicoveri
            Return DoCommand(Function(cmd)
                                 ''il primo where è una distinct by codice gruppo non toccare
                                 ''divertiti gl hf
                                 cmd.CommandText = "SELECT 
	                                                r.RIP_ID,
	                                                (SELECT min(r2.RIP_DATA_INGRESSO) FROM T_RICOVERI_PAZIENTE r2 WHERE r2.RIP_CODICE_GRUPPO = r.RIP_CODICE_GRUPPO) AS DataInizio,
	                                                r.RIP_CODICE_GRUPPO as CodiceGruppoRicovero,
	                                                r.RIP_PAZ_CODICE as CodicePaziente,
	                                                paz.PAZ_NOME as NomePaziente,
	                                                paz.PAZ_COGNOME as CognomePaziente,
	                                                paz.PAZ_DATA_NASCITA as DataDiNascita,
	                                                paz.PAZ_SESSO as Sesso,
	                                                r.RIP_STC as CodiceStatoClinico,
	                                                stc.COD_DESCRIZIONE as DescrizioneStatoClinico,
	                                                r.RIP_HSP_ID as CodiceStruttura,
	                                                r.RIP_ARE_CODICE as CodiceReparto,
	                                                nvl(rep.ARE_DESCRIZIONE, r.RIP_REPARTO_DESCRIZIONE) AS DescrizioneReparto,
	                                                h.HSP_CODICE_ASL,
	                                                h.HSP_DESCRIZIONE AS DescrizioneStruttura,
	                                                tr.ATR_CHIUSO AS Chiuso,
                                                    tr.ATR_CODICE as CodiceStatoRicovero,
                                                    tr.ATR_DESCRIZIONE as DescrizioneStatoRicovero
                                                FROM T_RICOVERI_PAZIENTE r
                                                JOIN T_PAZ_PAZIENTI paz ON paz.PAZ_CODICE = r.RIP_PAZ_CODICE
                                                JOIN T_ANA_CODICI_HSP h ON h.HSP_ID = r.RIP_HSP_ID 
                                                LEFT JOIN T_ANA_CODIFICHE stc ON stc.COD_CAMPO = 'RIP_STC' AND stc.COD_CODICE = r.RIP_STC 
                                                LEFT JOIN T_ANA_REPARTI rep ON rep.ARE_CODICE = r.RIP_ARE_CODICE 
                                                LEFT JOIN T_ANA_TIPI_RICOVERO tr ON tr.ATR_CODICE = r.RIP_ATR_CODICE
                                                WHERE EXISTS (
	                                                SELECT rip_id FROM (
		                                                SELECT  
			                                                r.RIP_CODICE_GRUPPO,
			                                                FIRST_VALUE(r.RIP_ID) OVER ( PARTITION BY r.RIP_CODICE_GRUPPO ORDER BY POS desc, RIP_DATA_INGRESSO desc, RIP_DATA_INSERIMENTO DESC, rip_id DESC, rip_codice_gruppo ) AS RIP_ID
			                                                FROM T_RICOVERI_PAZIENTE r			
			                                                JOIN (
				                                                SELECT
					                                                ATR_CODICE,
					                                                (
						                                                CASE (concat(NVL(a2.ATR_APERTO, 'N'), nvl(a2.ATR_CHIUSO, 'N'))) 
							                                                WHEN 'SN' THEN 0 
							                                                WHEN 'NS' THEN 2 
							                                                ELSE 1
							                                                end
					                                                ) AS POS
				                                                FROM 
				                                                T_ANA_TIPI_RICOVERO a2
				                                                ) a ON r.RIP_ATR_CODICE = a.ATR_CODICE 
			                                                WHERE RIP_DATA_ELIMINAZIONE IS NULL 	
			                                                ORDER BY POS desc, RIP_DATA_INGRESSO DESC, rip_codice_gruppo
		                                                ) t
		                                                WHERE t.rip_id = r.RIP_ID
	                                                )"
                                 Dim f As New List(Of String)
                                 If Not IsNothing(filtri) Then
                                     If filtri.SoloAperto Then
                                         f.Add("(ATR_CHIUSO is null or ATR_CHIUSO = 'N')")
                                     ElseIf filtri.StatoRicovero.HasValue AndAlso filtri.StatoRicovero.Value > 0 Then
                                         f.Add("ATR_CODICE = ?atr")
                                         cmd.AddParameter("atr", filtri.StatoRicovero.Value)
                                     End If
                                     If filtri.SoloChiusi Then
                                         f.Add("ATR_CHIUSO is not null and ATR_CHIUSO = 'S'")
                                     End If
                                     If Not String.IsNullOrWhiteSpace(filtri.CodiceUsl) Then
                                         f.Add("HSP_CODICE_ASL = ?usl")
                                         cmd.AddParameter("usl", filtri.CodiceUsl)
                                     End If
                                     If filtri.CodiceReparto.HasValue Then
                                         f.Add("RIP_ARE_CODICE = ?rep")
                                         cmd.AddParameter("rep", filtri.CodiceReparto.Value)
                                     End If
                                     If Not String.IsNullOrWhiteSpace(filtri.CodiceStatoClinico) Then
                                         f.Add("RIP_STC = ?stc")
                                         cmd.AddParameter("stc", filtri.CodiceStatoClinico)
                                     End If
                                     If Not String.IsNullOrWhiteSpace(filtri.CodiceStruttura) Then
                                         f.Add("RIP_HSP_ID = ?hsp")
                                         cmd.AddParameter("hsp", filtri.CodiceStruttura)
                                     End If
                                     If filtri.CodicePaziente.HasValue AndAlso filtri.CodicePaziente.Value > 0 Then
                                         f.Add("RIP_PAZ_CODICE = ?paz")
                                         cmd.AddParameter("paz", filtri.CodicePaziente.Value)
                                     End If
                                 End If

                                 If f.Any() Then
                                     cmd.CommandText += String.Format(" AND {0}", String.Join(" AND ", f))
                                 End If

                                 Dim take As Integer? = Nothing
                                 Dim skip As Integer = 0

                                 If Not IsNothing(filtri) Then
                                     If filtri.Skip.HasValue AndAlso filtri.Skip.Value > 0 Then
                                         skip = filtri.Skip.Value
                                     End If
                                     If filtri.Take.HasValue AndAlso filtri.Take.Value > 0 Then
                                         take = filtri.Take
                                     End If
                                 End If

                                 cmd.SkipTakeQuery(String.Format("{0} ORDER BY DATAINIZIO desc", cmd.CommandText), skip, take)

                                 Return cmd.Fill(Of TestataRicovero)
                             End Function)
        End Function

        Class RicoveroFlat
            Inherits RicoveroPaziente.EpisodioRicovero

            <DbColumnName("RIP_CODICE_GRUPPO")>
            Public Property CodiceGruppo As String
            <DbColumnName("RIP_PAZ_CODICE")>
            Public Property CodicePaziente As Long
        End Class

    End Class
End Namespace

