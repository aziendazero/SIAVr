Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.DAL

Public Class DbContattiScolasticiCovidProvider
    Inherits DbProvider
    Implements IContattiScolasticiCovidProvider

    Public Sub New(ByRef DAM As IDAM)
        MyBase.New(DAM)
    End Sub

    Public Sub SalvaDettaglioImportazione(command As SalvaDettaglioImportazioneCMD) Implements IContattiScolasticiCovidProvider.SalvaDettaglioImportazione
        Dim esiste As Boolean = DoCommand(Function(cmd)
                                              cmd.CommandText = "SELECT (
	                                                                CASE WHEN EXISTS (SELECT 1 AS a FROM T_ELAB_SCUOLA_DATI WHERE ESD_CODICE = ?gruppo) THEN 1 ELSE 0 END 
                                                                ) AS esiste FROM dual"
                                              cmd.AddParameter("gruppo", command.CodiceGruppo)
                                              Return cmd.First(Of Boolean)
                                          End Function)
        DoCommand(Sub(cmd)
                      If esiste Then
                          cmd.UpdateTable("T_ELAB_SCUOLA_DATI", New With {Key .ESD_EMAIL = command.Email,
                                              .ESD_TELEFONO = command.Telefono,
                                              .ESD_DIRIGENTE = command.Dirigente,
                                              .ESD_ULTIMO_GIORNO = command.UltimoGiorno,
                                              .ESD_NOTE = command.Note
                                          },
                                          New With {Key .ESD_CODICE = command.CodiceGruppo})
                      Else
                          cmd.InsertInTable("T_ELAB_SCUOLA_DATI", New With {Key .ESD_CODICE = command.CodiceGruppo,
                                            .ESD_EMAIL = command.Email,
                                            .ESD_TELEFONO = command.Telefono,
                                            .ESD_DIRIGENTE = command.Dirigente,
                                            .ESD_ULTIMO_GIORNO = command.UltimoGiorno,
                                            .ESD_NOTE = command.Note
                                            })
                      End If
                  End Sub)
    End Sub

    Public Sub ModificaClasseElaborazione(codiceElaborazione As Long, classe As String) Implements IContattiScolasticiCovidProvider.ModificaClasseElaborazione
        DoCommand(Sub(cmd)
                      cmd.UpdateTable("T_ELAB_CONTATTI_EPISODI", New With {Key .ECE_CLASSE = classe}, New With {Key .ECE_CODICE = codiceElaborazione})
                  End Sub)
    End Sub

    Public Function CercaInfoContattiScolastici(filtri As FiltriContattiScolastici) As IEnumerable(Of InfoContattoScolastico) Implements IContattiScolasticiCovidProvider.CercaInfoContattiScolastici
        Return DoCommand(Function(cmd)
                             Dim w As New List(Of String)
                             If Not IsNothing(filtri) Then
                                 If filtri.Chiuso Then
                                     w.Add("NOT EXISTS (
											SELECT 1 FROM T_PAZ_EPISODI_CONTATTI c
											JOIN T_PAZ_EPISODI e ON c.PEC_PES_ID = e.PES_ID 
											JOIN T_ANA_STATI_EPISODIO s1 ON s1.SEP_CODICE = e.PES_SEP_CODICE 
											LEFT JOIN T_PAZ_EPISODI ec ON ec.PES_ID = c.PEC_PES_ID_CONTATTO 
											LEFT JOIN T_ANA_STATI_EPISODIO s2 ON s2.SEP_CODICE = ec.PES_SEP_CODICE 
											WHERE c.PEC_ECE_GRUPPO = elab.ECE_GRUPPO AND (s1.SEP_ATTIVO = 'S' OR s2.SEP_ATTIVO = 'S') 
										)")
                                 End If
                                 If Not String.IsNullOrWhiteSpace(filtri.CodiceUlss) Then
                                     w.Add("indice.PES_USL_CODICE_RACCOLTA = ?uls")
                                     cmd.AddParameter("uls", filtri.CodiceUlss)
                                 End If
                                 If Not String.IsNullOrWhiteSpace(filtri.CodiceMeccanograficoScuola) Then
                                     w.Add("ECE_CODICE_MECCANO = ?codiceScuola")
                                     cmd.AddParameter("codiceScuola", filtri.CodiceMeccanograficoScuola)
                                 End If
                                 If Not String.IsNullOrWhiteSpace(filtri.CodiceComuneScuola) Then
                                     w.Add("sc.TAS_COM_CODICE = ?comuneScuola")
                                     cmd.AddParameter("comuneScuola", filtri.CodiceComuneScuola)
                                 End If
                             End If

                             Dim condizioni As String = ""
                             If w.Any() Then
                                 condizioni = "where " + String.Join(" AND ", w)
                             End If

                             cmd.CommandText = String.Format("SELECT 
													(CASE WHEN statoIndice.SEP_ATTIVO = 'S' OR statocontatto.SEP_ATTIVO = 'S' THEN 'S' ELSE 'N' END) AS Attivo,
										 			pazIndice.PAZ_NOME AS NomeIndice,
										 			pazIndice.paz_cognome AS CognomeIndice,
										 			ECE_COMUNE,
													ECE_CODICE_MECCANO as CodiceMeccanografico,
													sc.TAS_DESCRIZIONE AS DescrizioneScuola,
													ECE_GRUPPO as CodiceGruppo,
													elab.ECE_CLASSE as Classe,
													indice.PES_ID AS CodiceEpisodioIndice,
													EPISODIOCONTATTO.PES_ID AS CodiceEpisodioContatto,
													indice.PES_SEP_CODICE AS CodiceStatoEpisodioIndice,
													statoIndice.SEP_DESCRIZIONE AS DescStatoEpisodioIndice,
													statoIndice.SEP_ATTIVO AS AttivoIndice,
													episodioContatto.PES_SEP_CODICE AS CodiceStatoEpisodioContatto,
													STATOCONTATTO.SEP_DESCRIZIONE AS DescStatoEpisodioContatto,
													statocontatto.SEP_ATTIVO AS AttivoContatto,
													(SELECT count(*) + 1 FROM T_PAZ_EPISODI_CONTATTI WHERE PEC_ECE_GRUPPO = ECE_GRUPPO) AS NumeroPersone
													FROM T_ELAB_CONTATTI_EPISODI elab
													JOIN T_PAZ_EPISODI_CONTATTI con ON con.PEC_ECE_GRUPPO = elab.ECE_GRUPPO
													JOIN T_PAZ_EPISODI indice ON con.PEC_PES_ID = indice.PES_ID
													JOIN T_ANA_STATI_EPISODIO statoIndice ON statoIndice.SEP_CODICE = indice.PES_SEP_CODICE 
													LEFT JOIN T_PAZ_EPISODI episodioContatto ON con.PEC_PES_ID_CONTATTO = EPISODIOCONTATTO.PES_ID
													LEFT JOIN T_ANA_STATI_EPISODIO statoContatto ON STATOCONTATTO.SEP_CODICE = EPISODIOCONTATTO.PES_SEP_CODICE
													LEFT JOIN T_ANA_SCUOLE sc ON sc.TAS_CODICE_MECCANO = ece_codice_meccano
													JOIN T_PAZ_PAZIENTI pazIndice ON pazIndice.PAZ_CODICE = indice.PES_PAZ_CODICE 
													{0}
													ORDER BY ECE_DATA DESC, ECE_CODICE DESC", condizioni)
                             Return cmd.Fill(Of InfoContattoScolastico)
                         End Function)
    End Function

    Public Function CercaScuole(q As String) As IEnumerable(Of InfoScuola) Implements IContattiScolasticiCovidProvider.CercaScuole
        Return DoCommand(Function(cmd)
                             cmd.CommandText = "SELECT	TAS_ID as Codice,
														TAS_DESCRIZIONE as Descrizione, 
														TAS_CODICE_MECCANO as CodiceMeccanografico
														FROM T_ANA_SCUOLE"
                             If Not String.IsNullOrWhiteSpace(q) Then
                                 cmd.CommandText += " where UPPER(TAS_DESCRIZIONE) like ?qdesc or UPPER(TAS_CODICE_MECCANO) like ?qmeccano"
                                 cmd.AddParameter("qmeccano", String.Format("{0}%", q.ToUpper()))
                                 cmd.AddParameter("qdesc", String.Format("%{0}%", q.ToUpper()))
                             End If
                             Return cmd.Fill(Of InfoScuola)
                         End Function)
    End Function

    Public Function GetScuola(codiceMeccanografico As String) As InfoScuola Implements IContattiScolasticiCovidProvider.GetScuola
        Return DoCommand(Function(cmd)
                             cmd.CommandText = "SELECT	TAS_ID as Codice,
														TAS_DESCRIZIONE as Descrizione, 
														TAS_CODICE_MECCANO as CodiceMeccanografico
														FROM T_ANA_SCUOLE"
                             If Not String.IsNullOrWhiteSpace(codiceMeccanografico) Then
                                 cmd.CommandText += " where TAS_CODICE_MECCANO = ?q"
                                 cmd.AddParameter("q", codiceMeccanografico)
                             Else
                                 Return Nothing
                             End If
                             Return cmd.FirstOrDefault(Of InfoScuola)
                         End Function)
    End Function

    Public Function DataPositivizzazione(codiceEpisodio As Long) As Date? Implements IContattiScolasticiCovidProvider.DataPositivizzazione
        Return DoCommand(Function(cmd) As Date?
                             cmd.CommandText = "SELECT 
	                                                t.PET_DATA_TAMPONE 
                                                FROM T_PAZ_EPISODI tpe 
                                                JOIN T_PAZ_EPISODI_TAMPONI t ON PET_PES_ID = PES_ID
                                                WHERE PES_ID = ?codice AND t.PET_ESITO = 'P'
                                                UNION ALL (
	                                                SELECT a.ATC_SAMPLING_DATE FROM T_ANA_TAMPONI_COVID a
		                                                WHERE ATC_RESULT = 'POSITIVO' AND ATC_PAZ_CODICE = (SELECT PES_PAZ_CODICE FROM T_PAZ_EPISODI WHERE PES_ID = ?codice)
                                                )"
                             cmd.AddParameter("codice", codiceEpisodio)
                             Dim elementi As List(Of Date) = cmd.Fill(Of Date?).Where(Function(X)
                                                                                          Return X.HasValue
                                                                                      End Function).Select(Function(x)
                                                                                                               Return x.Value
                                                                                                           End Function).ToList()
                             If elementi.Any() Then
                                 Return elementi.Min()
                             Else
                                 Return Nothing
                             End If
                         End Function)
    End Function

    Public Function GetDettaglioImportazione(codiceGruppo As String) As DettaglioImportazione Implements IContattiScolasticiCovidProvider.GetDettaglioImportazione
        Return DoCommand(Function(cmd)
                             cmd.CommandText = "SELECT distinct
                                                    sc.TAS_ID as CodiceScuola,
                                                    sc.TAS_INDIRIZZO as Indirizzo,
                                                    sc.TAS_DESCRIZIONE as NomeScuola,
                                                    nvl(d.ESD_DIRIGENTE, sc.TAS_DIRIGENTE) as Dirigente,
                                                    nvl(d.ESD_TELEFONO, sc.TAS_TELEFONO) as Telefono,
                                                    nvl(d.ESD_EMAIL, sc.TAS_MAIL) as Email,
                                                    d.ESD_NOTE AS Note,
                                                    d.ESD_ULTIMO_GIORNO AS UltimoGiorno
                                                    FROM T_ELAB_CONTATTI_EPISODI imp
                                                    JOIN T_ANA_SCUOLE sc ON sc.TAS_CODICE_MECCANO = imp.ECE_CODICE_MECCANO 
                                                    LEFT JOIN T_ELAB_SCUOLA_DATI d ON d.ESD_CODICE = ECE_GRUPPO
                                                    WHERE ECE_GRUPPO = ?codice"
                             cmd.AddParameter("codice", codiceGruppo)
                             Return cmd.FirstOrDefault(Of DettaglioImportazione)
                         End Function)
    End Function

    Public Function InformazioniContattiEpisodio(codiceImportazione As String) As List(Of InformazioneContattoEsportazione) Implements IContattiScolasticiCovidProvider.InformazioniContattiEpisodio
        Return DoCommand(Function(c)
                             Dim ritorno As List(Of InformazioneContattoEsportazione) = DoCommand(Function(cmd)
                                                                                                      cmd.CommandText = "SELECT 
                                                                                                                            paz.paz_codice as CodicePaziente,
                                                                                                                            paz.PAZ_COGNOME as Cognome,
                                                                                                                            paz.PAZ_NOME as Nome,
                                                                                                                            paz.PAZ_SESSO as Sesso,
                                                                                                                            paz.PAZ_DATA_NASCITA as DataNascita,
                                                                                                                            com.COM_DESCRIZIONE as ComuneResidenza,
                                                                                                                            epp.PES_SEP_CODICE as CodiceStato,
                                                                                                                            sta.SEP_DESCRIZIONE as DescrizioneStato,
                                                                                                                            elab.ECE_TIPO as Tipologia,
                                                                                                                            elab.ECE_CODICE as CodiceElaborazione,
                                                                                                                            elab.ECE_CLASSE as Classe
                                                                                                                            FROM T_PAZ_EPISODI_CONTATTI c
                                                                                                                                JOIN T_PAZ_PAZIENTI paz ON paz.PAZ_CODICE = c.PEC_PAZ_CODICE 
                                                                                                                                LEFT JOIN T_PAZ_EPISODI epp ON c.PEC_PES_ID_CONTATTO = epp.PES_ID 
                                                                                                                                LEFT JOIN T_ANA_STATI_EPISODIO sta ON sta.SEP_CODICE = epp.PES_SEP_CODICE 
                                                                                                                                LEFT JOIN T_ANA_COMUNI com ON com.COM_CODICE = paz.PAZ_COM_CODICE_RESIDENZA 
                                                                                                                                LEFT JOIN T_ELAB_CONTATTI_EPISODI elab ON elab.ECE_CODICE_FISCALE = paz.PAZ_CODICE_FISCALE AND elab.ECE_GRUPPO = c.PEC_ECE_GRUPPO 
                                                                                                                                    WHERE c.PEC_ECE_GRUPPO = ?codice"
                                                                                                      cmd.AddParameter("codice", codiceImportazione)
                                                                                                      Return cmd.Fill(Of InformazioneContattoEsportazione)
                                                                                                  End Function)

                             If ritorno.Any(Function(x) Not x.CodiceStato.HasValue) Then

                                 Dim codiciPaziente As IEnumerable(Of Long) = ritorno.Where(Function(x)
                                                                                                Return Not x.CodiceStato.HasValue
                                                                                            End Function).Select(Function(x) x.CodicePaziente)

                                 Dim integrativi As IEnumerable(Of DatiContattiScolasticiIntegrativi) = GetDatiIntegrativiContattiScolastici(codiciPaziente)

                                 For Each i As DatiContattiScolasticiIntegrativi In integrativi

                                     For Each s As InformazioneContattoEsportazione In ritorno.Where(Function(x) x.CodicePaziente = i.CodicePaziente)
                                         s.CodiceStato = i.CodiceStato
                                         s.DescrizioneStato = i.DescrizioneStato
                                     Next

                                 Next


                             End If



                             If Not ritorno.Any() Then
                                 Return ritorno
                             End If

                             Dim codiceEpisodio As Long = DoCommand(Function(cmd)
                                                                        cmd.CommandText = "SELECT 
                                                                                                c.PEC_PES_ID 
                                                                                                FROM T_PAZ_EPISODI_CONTATTI c
                                                                                                WHERE c.PEC_ECE_GRUPPO = ?gruppo"
                                                                        cmd.AddParameter("gruppo", codiceImportazione)
                                                                        Return cmd.First(Of Long)
                                                                    End Function)

                             Dim dataPositivizzazione As Date? = Me.DataPositivizzazione(codiceEpisodio)
                             If Not dataPositivizzazione.HasValue Then
                                 Return ritorno
                             End If

                             Dim elencoTest As List(Of InformazioniTestScolastici) = DoCommand(Function(cmd)
                                                                                                   cmd.CommandText = String.Format("SELECT 
                                                                                                                        ATC_PAZ_CODICE AS CodicePaziente,
                                                                                                                        ATC_SAMPLING_DATE AS Data,
                                                                                                                        ATC_RESULT AS RisultatoTampone,
                                                                                                                        1 AS IsTampone,
                                                                                                                        '' AS esito,
                                                                                                                        '' AS IGG,
                                                                                                                        '' AS IGM,
                                                                                                                        'Tampone' AS Metodologia
                                                                                                                            FROM T_ANA_TAMPONI_COVID WHERE ATC_PAZ_CODICE in ({0})
                                                                                                                            UNION (
	                                                                                                                            SELECT
	                                                                                                                            s.TSS_PAZ_CODICE,
	                                                                                                                            s.TSS_DATA_PRELIEVO,
	                                                                                                                            '',
	                                                                                                                            0,
	                                                                                                                            s.TSS_ESITO_ANTIGENE,
	                                                                                                                            s.TSS_IGG,
	                                                                                                                            s.TSS_IGM,
	                                                                                                                            s.TSS_METODICA 
	                                                                                                                                FROM T_PAZ_TEST_SIEROLOGICI s
	                                                                                                                                WHERE s.TSS_PAZ_CODICE in ({0})
                                                                                                                            )
	                                                                                                                                union(
	                                                                                                                                	SELECT 
	                                                                                                                                	pes_paz_codice,
	                                                                                                                                	et.PET_DATA_TAMPONE,
	                                                                                                                                	cod.COD_DESCRIZIONE ,
	                                                                                                                                	1,
	                                                                                                                                	'',
	                                                                                                                                	'',
	                                                                                                                                	'',
	                                                                                                                                	'Tampone'
	                                                                                                                                	FROM t_paz_episodi_tamponi et
	                                                                                                                                	JOIN T_PAZ_EPISODI tpe ON tpe.PES_ID = et.PET_PES_ID 
	                                                                                                                                	JOIN T_ANA_CODIFICHE cod ON cod.COD_CAMPO = 'PET_ESITO' AND cod.COD_CODICE = et.PET_ESITO 
	                                                                                                                                	WHERE (et.PET_TIPO_TAMPONE IS NULL OR et.PET_TIPO_TAMPONE = 1) and tpe.PES_PAZ_CODICE IN ({0})
	                                                                                                                                )
                                                                                                                                        union(
	                                                                                                                                	    SELECT 
	                                                                                                                                	    pes_paz_codice,
	                                                                                                                                	    et.PET_DATA_TAMPONE,
	                                                                                                                                	    cod.COD_DESCRIZIONE ,
	                                                                                                                                	    0,
	                                                                                                                                	    '',
	                                                                                                                                	    '',
	                                                                                                                                	    '',
	                                                                                                                                	    'Antigenico'
	                                                                                                                                	    FROM t_paz_episodi_tamponi et
	                                                                                                                                	    JOIN T_PAZ_EPISODI tpe ON tpe.PES_ID = et.PET_PES_ID 
	                                                                                                                                	    JOIN T_ANA_CODIFICHE cod ON cod.COD_CAMPO = 'PET_ESITO' AND cod.COD_CODICE = et.PET_ESITO 
	                                                                                                                                	    WHERE (et.PET_TIPO_TAMPONE IS not NULL and et.PET_TIPO_TAMPONE <> 1) and tpe.PES_PAZ_CODICE IN ({0})
	                                                                                                                                    )", cmd.SetParameterIn("P", ritorno.Select(Function(x) x.CodicePaziente)))
                                                                                                   Return cmd.Fill(Of InformazioniTestScolastici)
                                                                                               End Function)

                             For Each el As InformazioneContattoEsportazione In ritorno

                                 Dim elenco As IEnumerable(Of InformazioniTestScolastici) = elencoTest.Where(Function(x)
                                                                                                                 Return x.CodicePaziente = el.CodicePaziente
                                                                                                             End Function)

                                 el.Ultimo = elenco.Where(Function(x)
                                                              Return x.Data < dataPositivizzazione.Value
                                                          End Function).OrderByDescending(Function(x) x.Data).FirstOrDefault()

                                 el.Iniziale = elenco.Where(Function(x)
                                                                Return x.Data >= dataPositivizzazione.Value AndAlso x.Data <= dataPositivizzazione.Value.AddDays(5)
                                                            End Function).OrderBy(Function(x) x.Data).FirstOrDefault()

                                 el.Finale = elenco.Where(Function(x)
                                                              Return x.Data > dataPositivizzazione.Value.AddDays(5)
                                                          End Function).OrderByDescending(Function(x) x.Data).FirstOrDefault()


                             Next
                             Return ritorno
                         End Function)
    End Function

    Private Function GetDatiIntegrativiContattiScolastici(codiciPazienti As IEnumerable(Of Long)) As IEnumerable(Of DatiContattiScolasticiIntegrativi)
        If Not codiciPazienti.Any() Then
            Return New List(Of DatiContattiScolasticiIntegrativi)
        End If

        Return DoCommand(Function(cmd)
                             cmd.CommandText = String.Format("SELECT 
                                                    PES_SEP_CODICE AS CodiceStato,
                                                    sep_descrizione AS DescrizioneStato,
                                                    sep_attivo AS Attivo,
                                                    tpe.PES_DATA_INIZIO_ISOLAMENTO AS DataInizio,
                                                    PES_PAZ_CODICE AS CodicePaziente 
                                                    FROM T_PAZ_EPISODI tpe
                                                    JOIN T_ANA_STATI_EPISODIO sta ON sta.SEP_CODICE = tpe.PES_SEP_CODICE 
                                                    WHERE PES_SEP_CODICE is not null and PES_PAZ_CODICE IN ({0})", cmd.SetParameterIn("P", codiciPazienti))
                             Return cmd.Fill(Of DatiContattiScolasticiIntegrativi)
                         End Function).GroupBy(Function(x) x.CodicePaziente).Select(Function(g)
                                                                                        Return g.OrderByDescending(Function(x) x.DataInizio).FirstOrDefault()
                                                                                    End Function).Where(Function(x) Not IsNothing(x)).ToList()

    End Function

    Class DatiContattiScolasticiIntegrativi
        Public Property CodiceStato As Integer
        Public Property DescrizioneStato As String
        Public Property Attivo As Boolean
        Public Property DataInizio As Date
        Public Property CodicePaziente As Long
    End Class
End Class
