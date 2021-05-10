Imports System.Data
Imports System.Data.OracleClient
Imports System.Linq
Imports System.Text
Imports System.Collections.Generic
Imports System.Text.RegularExpressions

Imports Onit.Database.DataAccessManager

Imports Onit.OnAssistnet.Data
Imports Onit.OnAssistnet.Data.OracleClient
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.Filters
Imports Onit.OnAssistnet.OnVac.Log.DataLogStructure


Namespace DAL

    Public Class DbCoperturaProvider
        Inherits DbProvider
        Implements ICoperturaProvider

#Region " Costruttori "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " ICoperturaProvider "

        Public Function GetTotaleAnagrafico(filtro As FiltriCopertura) As Int32 Implements ICoperturaProvider.GetTotaleAnagrafico

            Dim totaleAnagrafico As Int32 = 0

            Dim cmd As OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try

                cmd = Me.Connection.CreateCommand()

                Dim query As New StringBuilder()
                Dim filtroPazienti As New StringBuilder()

                cmd.Parameters.AddWithValue("data_nascita_inizio", filtro.dataNascitaDa)
                cmd.Parameters.AddWithValue("data_nascita_fine", filtro.dataNascitaA)

                ' --- Filtri sui pazienti

                If Not String.IsNullOrEmpty(filtro.codiceComuneResidenza) Then
                    filtroPazienti.AppendFormat(" and paz_com_codice_residenza = '{0}' ", filtro.codiceComuneResidenza)
                End If

                If filtro.codiceConsultorio.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_cns_codice IN ('{0}') ", filtro.codiceConsultorio.Aggregate(Function(p, g) p & "', '" & g))
                End If

                If Not String.IsNullOrEmpty(filtro.codiceCircoscrizione) Then
                    filtroPazienti.AppendFormat(" and paz_cir_codice = '{0}' ", filtro.codiceCircoscrizione)
                End If

                If filtro.Sesso.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_sesso IN ('{0}') ", filtro.Sesso.Aggregate(Function(p, g) p & "', '" & g))
                End If

                If filtro.StatoAnagrafico.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_stato_anagrafico IN ('{0}') ", filtro.StatoAnagrafico.Aggregate(Function(p, g) p & "', '" & g))
                End If
                ' ---

                query.AppendFormat(OnVac.Queries.Copertura.OracleQueries.selTotaleAnagrafico, filtroPazienti.ToString())

                ' --- Query finale --- '
                cmd.CommandText = query.ToString()

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Dim obj As Object = cmd.ExecuteScalar()
                If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                    totaleAnagrafico = obj
                End If

            Catch ex As Exception

                LogError(ex)

                ex.InternalPreserveStackTrace()
                Throw

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return totaleAnagrafico

        End Function

        Public Function GetTotaleAnagraficoMedico(filtro As FiltriCoperturaMedico) As Int32 Implements ICoperturaProvider.GetTotaleAnagraficoMedico

            Dim totaleAnagrafico As Int32 = 0

            Dim cmd As OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try

                cmd = Me.Connection.CreateCommand()

                Dim query As New StringBuilder()
                Dim filtroPazienti As New StringBuilder()

                cmd.Parameters.AddWithValue("data_nascita_inizio", filtro.dataNascitaDa)
                cmd.Parameters.AddWithValue("data_nascita_fine", filtro.dataNascitaA)

                ' --- Filtri sui pazienti

                If Not String.IsNullOrEmpty(filtro.codiceComuneResidenza) Then
                    filtroPazienti.AppendFormat(" and paz_com_codice_residenza = '{0}' ", filtro.codiceComuneResidenza)
                End If

                If filtro.codiceConsultorio.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_cns_codice IN ('{0}') ", filtro.codiceConsultorio.Aggregate(Function(p, g) p & "', '" & g))
                End If

                If Not String.IsNullOrEmpty(filtro.codiceCircoscrizione) Then
                    filtroPazienti.AppendFormat(" and paz_cir_codice = '{0}' ", filtro.codiceCircoscrizione)
                End If

                If filtro.Sesso.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_sesso IN ('{0}') ", filtro.Sesso.Aggregate(Function(p, g) p & "', '" & g))
                End If

                If filtro.StatoAnagrafico.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_stato_anagrafico IN ('{0}') ", filtro.StatoAnagrafico.Aggregate(Function(p, g) p & "', '" & g))
                End If

                If filtro.tipoMedico.Count > 0 Then
                    filtroPazienti.AppendFormat(" and med_tipo IN ('{0}') ", filtro.tipoMedico.Aggregate(Function(p, g) p & "', '" & g))
                End If

                If Not String.IsNullOrEmpty(filtro.codiceMedico) Then
                    filtroPazienti.AppendFormat(" and med_codice = '{0}' ", filtro.codiceMedico)
                Else
                    filtroPazienti.AppendFormat(" and (med_scadenza is null or med_scadenza >= :med_scadenza) ")
                    cmd.Parameters.AddWithValue("med_scadenza", Date.Today)
                End If

                query.AppendFormat(OnVac.Queries.Copertura.OracleQueries.selTotaleAnagraficoMedico, filtroPazienti.ToString())

                ' --- Query finale --- '
                cmd.CommandText = query.ToString()

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Dim obj As Object = cmd.ExecuteScalar()
                If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                    totaleAnagrafico = obj
                End If

            Catch ex As Exception

                LogError(ex)

                ex.InternalPreserveStackTrace()
                Throw

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return totaleAnagrafico

        End Function

        Public Function GetCoperturaVaccinale(filtro As FiltriCopertura) As dsCoperturaVaccinale Implements ICoperturaProvider.GetCoperturaVaccinale

            Dim dsCoperturaVaccinale As New dsCoperturaVaccinale()

            Dim cmd As OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try

                cmd = Me.Connection.CreateCommand()

                cmd.Parameters.AddWithValue("giorni_vita", filtro.giorniVita)
                cmd.Parameters.AddWithValue("data_nascita_inizio", filtro.dataNascitaDa)
                cmd.Parameters.AddWithValue("data_nascita_fine", filtro.dataNascitaA)
                cmd.Parameters.AddWithValue("dosi", filtro.richiamo)

                ' --- Filtri sui pazienti
                Dim filtroPazienti As New StringBuilder()

                If Not String.IsNullOrEmpty(filtro.codiceComuneResidenza) Then
                    filtroPazienti.AppendFormat(" and paz_com_codice_residenza = '{0}' ", filtro.codiceComuneResidenza)
                End If

                If filtro.codiceConsultorio.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_cns_codice IN ('{0}') ", filtro.codiceConsultorio.Aggregate(Function(p, g) p & "', '" & g))
                End If

                If Not String.IsNullOrEmpty(filtro.codiceCircoscrizione) Then
                    filtroPazienti.AppendFormat(" and paz_cir_codice = '{0}' ", filtro.codiceCircoscrizione)
                End If

                If filtro.Sesso.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_sesso IN ('{0}') ", filtro.Sesso.Aggregate(Function(p, g) p & "', '" & g))
                End If

                If filtro.StatoAnagrafico.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_stato_anagrafico IN ('{0}') ", filtro.StatoAnagrafico.Aggregate(Function(p, g) p & "', '" & g))
                End If
                ' ---

                ' --- Filtri sulle vaccinazioni
                Dim filtroVaccinazioni As New StringBuilder()

                If filtro.dataEffettuazioneDa.HasValue Then
                    filtroVaccinazioni.Append(" and ves_data_effettuazione >= :ves_data_effettuazione_da ")
                    cmd.Parameters.AddWithValue("ves_data_effettuazione_da", filtro.dataEffettuazioneDa.Value)
                End If

                If filtro.dataEffettuazioneDa.HasValue Then
                    filtroVaccinazioni.Append(" and ves_data_effettuazione <= :ves_data_effettuazione_a ")
                    cmd.Parameters.AddWithValue("ves_data_effettuazione_a", filtro.dataEffettuazioneA.Value)
                End If

                ' ---

                Dim query As New StringBuilder()

                query.AppendFormat(OnVac.Queries.Copertura.OracleQueries.selCopertura, filtroPazienti.ToString(), filtroVaccinazioni.ToString())
                query.AppendFormat(" WHERE vac_codice IN ('{0}') ", filtro.codiceVaccinazione.Aggregate(Function(p, g) p & "', '" & g))

                If filtro.tipoVaccinazioni.Count > 0 Then
                    query.AppendFormat(" and vac_obbligatoria IN ('{0}') ", filtro.tipoVaccinazioni.Aggregate(Function(p, g) p & "', '" & g))
                End If

                query.Append(" ORDER BY vac_ordine, vac_codice ")

                ' --- Query finale --- '
                cmd.CommandText = query.ToString()

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using adp As New OracleDataAdapter(cmd)
                    adp.Fill(dsCoperturaVaccinale.CoperturaVaccinale)
                End Using

            Catch ex As Exception

                LogError(ex)

                ex.InternalPreserveStackTrace()
                Throw

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return dsCoperturaVaccinale

        End Function

        Public Function GetCoperturaVaccinaleMedico(filtro As FiltriCoperturaMedico) As dsCoperturaVaccinaleMedico Implements ICoperturaProvider.GetCoperturaVaccinaleMedico

            Dim dsCoperturaVaccinale As New dsCoperturaVaccinaleMedico()
            Dim cmd As OracleCommand = Nothing
            Dim ownConnection As Boolean = False

            Try

                ' --- Filtri sui pazienti
                Dim filtroPazienti As New StringBuilder()

                If Not String.IsNullOrEmpty(filtro.codiceComuneResidenza) Then
                    filtroPazienti.AppendFormat(" and paz_com_codice_residenza = '{0}' ", filtro.codiceComuneResidenza)
                End If

                If filtro.codiceConsultorio.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_cns_codice IN ('{0}') ", filtro.codiceConsultorio.Aggregate(Function(p, g) p & "', '" & g))
                End If

                If Not String.IsNullOrEmpty(filtro.codiceCircoscrizione) Then
                    filtroPazienti.AppendFormat(" and paz_cir_codice = '{0}' ", filtro.codiceCircoscrizione)
                End If

                If filtro.Sesso.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_sesso IN ('{0}') ", filtro.Sesso.Aggregate(Function(p, g) p & "', '" & g))
                End If

                If filtro.StatoAnagrafico.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_stato_anagrafico IN ('{0}') ", filtro.StatoAnagrafico.Aggregate(Function(p, g) p & "', '" & g))
                End If
                ' ---

                For i As Integer = 0 To filtro.Vaccinazioni.Count - 1

                    cmd = Me.Connection.CreateCommand()

                    cmd.Parameters.AddWithValue("giorni_vita", filtro.Vaccinazioni(i).giorniVita)
                    cmd.Parameters.AddWithValue("dosi", filtro.Vaccinazioni(i).richiamo)
                    cmd.Parameters.AddWithValue("data_nascita_inizio", filtro.dataNascitaDa)
                    cmd.Parameters.AddWithValue("data_nascita_fine", filtro.dataNascitaA)


                    ' --- Filtri sulle vaccinazioni
                    Dim filtroVaccinazioni As New StringBuilder()

                    If filtro.dataEffettuazioneDa.HasValue Then
                        filtroVaccinazioni.Append(" and ves_data_effettuazione >= :ves_data_effettuazione_da ")
                        cmd.Parameters.AddWithValue("ves_data_effettuazione_da", filtro.dataEffettuazioneDa.Value)
                    End If

                    If filtro.dataEffettuazioneDa.HasValue Then
                        filtroVaccinazioni.Append(" and ves_data_effettuazione <= :ves_data_effettuazione_a ")
                        cmd.Parameters.AddWithValue("ves_data_effettuazione_a", filtro.dataEffettuazioneA.Value)
                    End If
                    ' ---

                    Dim query As New StringBuilder()

                    query.AppendFormat(OnVac.Queries.Copertura.OracleQueries.selCoperturaMedico, filtroPazienti.ToString(), filtroVaccinazioni.ToString())
                    query.AppendFormat(" WHERE   vac_codice = :vac_codice ")
                    cmd.Parameters.AddWithValue("vac_codice", filtro.Vaccinazioni(i).codice)

                    If Not String.IsNullOrEmpty(filtro.codiceMedico) Then
                        query.AppendFormat(" and t2.med_codice = :med_codice ")
                        cmd.Parameters.AddWithValue("med_codice", filtro.codiceMedico)
                    Else
                        query.AppendFormat(" and (t2.med_scadenza is null or t2.med_scadenza >= :med_scadenza) ")
                        cmd.Parameters.AddWithValue("med_scadenza", Date.Today)
                    End If

                    If filtro.tipoMedico.Count > 0 Then
                        query.AppendFormat(" and med_tipo IN ('{0}') ", filtro.tipoMedico.Aggregate(Function(p, g) p & "', '" & g))
                    End If

                    If filtro.tipoVaccinazioni.Count > 0 Then
                        query.AppendFormat(" and vac_obbligatoria IN ('{0}') ", filtro.tipoVaccinazioni.Aggregate(Function(p, g) p & "', '" & g))
                    End If

                    'query.Append(" ORDER BY   vac_ordine, vac_codice ")

                    ' --- Query finale --- '
                    cmd.CommandText = query.ToString()

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using adp As New OracleDataAdapter(cmd)
                        Dim dstemp As New dsCoperturaVaccinaleMedico()
                        adp.Fill(dstemp.CoperturaVaccinale)
                        dsCoperturaVaccinale.CoperturaVaccinale.Merge(dstemp.CoperturaVaccinale)
                    End Using

                Next


                ' Valorizzazione dei campi calcolati
                For Each row As dsCoperturaVaccinaleMedico.CoperturaVaccinaleRow In dsCoperturaVaccinale.CoperturaVaccinale.Rows

                    row.IMMUNI = row.IMMUNI_MAI_VACCINATI + row.IMMUNI_VACCINATI
                    row.NON_VACCINABILI = row.NON_VACCINABILI_MAI_VACCINATI + row.NON_VACCINABILI_VACCINATI

                    If (row.NUM_PAZIENTI > 0) Then
                        row.PERCENTO_NETTA = (((row.NUM_VACCINATI + row.IMMUNI_MAI_VACCINATI + row.IMMUNI_VACCINATI) * 100) / (row.NUM_PAZIENTI - row.NON_VACCINABILI_MAI_VACCINATI - row.NON_VACCINABILI_VACCINATI)).ToString("0.##") & " %"
                    Else
                        row.PERCENTO_NETTA = "-"
                    End If

                    If (row.NUM_PAZIENTI > 0) Then
                        row.PERCENTO_VACCINATI = ((row.NUM_VACCINATI * 100) / (row.NUM_PAZIENTI)).ToString("0.##") & " %"
                    Else
                        row.PERCENTO_VACCINATI = "-"
                    End If
                    row.NUM_NVAC = row.NUM_PAZIENTI - row.NUM_VACCINATI

                Next

                ' Riempimento del datatable con i filtri di vaccinazione
                For i As Integer = 0 To filtro.Vaccinazioni.Count - 1
                    Dim row As dsCoperturaVaccinaleMedico.FiltriVaccinazioniRow = dsCoperturaVaccinale.FiltriVaccinazioni.NewFiltriVaccinazioniRow()
                    row.DOSE = filtro.Vaccinazioni(i).richiamo
                    row.GIORNI_VITA = filtro.Vaccinazioni(i).giorniVita
                    row.VAC_CODICE = filtro.Vaccinazioni(i).codice
                    row.VAC_DESCRIZIONE = filtro.Vaccinazioni(i).descrizione
                    dsCoperturaVaccinale.FiltriVaccinazioni.AddFiltriVaccinazioniRow(row)
                Next

            Catch ex As Exception

                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw

            Finally

                Me.ConditionalCloseConnection(ownConnection)
                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return dsCoperturaVaccinale

        End Function

        Public Function GetCoperturaVaccinaleCNS(filtro As FiltriCopertura) As dsCoperturaVaccinaleCNS Implements ICoperturaProvider.GetCoperturaVaccinaleCNS

            Dim dsCoperturaVaccinaleCNS As New dsCoperturaVaccinaleCNS()

            Dim cmd As OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try

                cmd = Me.Connection.CreateCommand()

                cmd.Parameters.AddWithValue("giorni_vita", filtro.giorniVita)
                cmd.Parameters.AddWithValue("data_nascita_inizio", filtro.dataNascitaDa)
                cmd.Parameters.AddWithValue("data_nascita_fine", filtro.dataNascitaA)
                cmd.Parameters.AddWithValue("dosi", filtro.richiamo)

                ' --- Filtri sui pazienti
                Dim filtroPazienti As New StringBuilder()

                If Not String.IsNullOrEmpty(filtro.codiceComuneResidenza) Then
                    filtroPazienti.AppendFormat(" and paz_com_codice_residenza = '{0}' ", filtro.codiceComuneResidenza)
                End If

                If filtro.codiceConsultorio.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_cns_codice IN ('{0}') ", filtro.codiceConsultorio.Aggregate(Function(p, g) p & "', '" & g))
                End If

                If Not String.IsNullOrEmpty(filtro.codiceCircoscrizione) Then
                    filtroPazienti.AppendFormat(" and paz_cir_codice = '{0}' ", filtro.codiceCircoscrizione)
                End If

                If filtro.Sesso.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_sesso IN ('{0}') ", filtro.Sesso.Aggregate(Function(p, g) p & "', '" & g))
                End If

                If filtro.StatoAnagrafico.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_stato_anagrafico IN ('{0}') ", filtro.StatoAnagrafico.Aggregate(Function(p, g) p & "', '" & g))
                End If
                ' ---

                ' --- Filtri sulle vaccinazioni
                Dim filtroVaccinazioni As New StringBuilder()

                If filtro.dataEffettuazioneDa.HasValue Then
                    filtroVaccinazioni.Append(" and ves_data_effettuazione >= :ves_data_effettuazione_da ")
                    cmd.Parameters.AddWithValue("ves_data_effettuazione_da", filtro.dataEffettuazioneDa.Value)
                End If

                If filtro.dataEffettuazioneDa.HasValue Then
                    filtroVaccinazioni.Append(" and ves_data_effettuazione <= :ves_data_effettuazione_a ")
                    cmd.Parameters.AddWithValue("ves_data_effettuazione_a", filtro.dataEffettuazioneA.Value)
                End If
                ' ---

                Dim query As New StringBuilder()

                query.AppendFormat(OnVac.Queries.Copertura.OracleQueries.selCoperturaCNS, filtroPazienti.ToString(), filtroVaccinazioni.ToString())
                query.AppendFormat(" WHERE   vac_codice IN ('{0}') ", filtro.codiceVaccinazione.Aggregate(Function(p, g) p & "', '" & g))

                If filtro.tipoVaccinazioni.Count > 0 Then
                    query.AppendFormat(" and vac_obbligatoria IN ('{0}') ", filtro.tipoVaccinazioni.Aggregate(Function(p, g) p & "', '" & g))
                End If
				If filtro.codiceConsultorio.Count > 0 Then
					query.AppendFormat(" and cns_codice IN ('{0}') ", filtro.codiceConsultorio.Aggregate(Function(p, g) p & "', '" & g))
				Else
					query.AppendFormat(" and cns_codice is null ")
				End If

				query.Append(" ORDER BY vac_ordine, vac_codice ")

                ' --- Query finale --- '
                cmd.CommandText = query.ToString()

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using adp As New OracleDataAdapter(cmd)
                    adp.Fill(dsCoperturaVaccinaleCNS.CoperturaVaccinaleCNS)
                End Using

            Catch ex As Exception

                LogError(ex)

                ex.InternalPreserveStackTrace()
                Throw

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return dsCoperturaVaccinaleCNS

        End Function

        Public Function GetMotiviEsclusione(filtro As FiltriCopertura) As dsMotiviEsclusione Implements ICoperturaProvider.GetMotiviEsclusione

            Dim dsMotiviEsclusione As New dsMotiviEsclusione()

            Dim cmd As OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try

                cmd = Me.Connection.CreateCommand()

                cmd.Parameters.AddWithValue("giorni_vita", filtro.giorniVita)
                cmd.Parameters.AddWithValue("data_nascita_inizio", filtro.dataNascitaDa)
                cmd.Parameters.AddWithValue("data_nascita_fine", filtro.dataNascitaA)
                cmd.Parameters.AddWithValue("dosi", filtro.richiamo)

                ' --- Filtri sui pazienti
                Dim filtroPazienti As New StringBuilder()

                If Not String.IsNullOrEmpty(filtro.codiceComuneResidenza) Then
                    filtroPazienti.AppendFormat(" and paz_com_codice_residenza = '{0}' ", filtro.codiceComuneResidenza)
                End If

                If filtro.codiceConsultorio.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_cns_codice IN ('{0}') ", filtro.codiceConsultorio.Aggregate(Function(p, g) p & "', '" & g))
                End If

                If Not String.IsNullOrEmpty(filtro.codiceCircoscrizione) Then
                    filtroPazienti.AppendFormat(" and paz_cir_codice = '{0}' ", filtro.codiceCircoscrizione)
                End If

                If filtro.Sesso.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_sesso IN ('{0}') ", filtro.Sesso.Aggregate(Function(p, g) p & "', '" & g))
                End If

                If filtro.StatoAnagrafico.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_stato_anagrafico IN ('{0}') ", filtro.StatoAnagrafico.Aggregate(Function(p, g) p & "', '" & g))
                End If
                ' ---

                ' --- Filtri sulle vaccinazioni
                Dim filtroVaccinazioni As New StringBuilder()

                If filtro.dataEffettuazioneDa.HasValue Then
                    filtroVaccinazioni.Append(" and ves_data_effettuazione >= :ves_data_effettuazione_da ")
                    cmd.Parameters.AddWithValue("ves_data_effettuazione_da", filtro.dataEffettuazioneDa.Value)
                End If

                If filtro.dataEffettuazioneDa.HasValue Then
                    filtroVaccinazioni.Append(" and ves_data_effettuazione <= :ves_data_effettuazione_a ")
                    cmd.Parameters.AddWithValue("ves_data_effettuazione_a", filtro.dataEffettuazioneA.Value)
                End If
                ' ---

                Dim query As New StringBuilder()

                query.AppendFormat(OnVac.Queries.Copertura.OracleQueries.selMotiviEsclusione, filtroPazienti.ToString(), filtroVaccinazioni.ToString())
                query.AppendFormat(" and vac_codice IN ('{0}') ", filtro.codiceVaccinazione.Aggregate(Function(p, g) p & "', '" & g))

                If filtro.tipoVaccinazioni.Count > 0 Then
                    query.AppendFormat(" and vac_obbligatoria IN ('{0}') ", filtro.tipoVaccinazioni.Aggregate(Function(p, g) p & "', '" & g))
                End If

                ' --- Query finale --- '
                cmd.CommandText = query.ToString()

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using adp As New OracleDataAdapter(cmd)
                    adp.Fill(dsMotiviEsclusione.V_MOTIVI_ESCLUSIONE)
                End Using

            Catch ex As Exception

                LogError(ex)

                ex.InternalPreserveStackTrace()
                Throw

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return dsMotiviEsclusione

        End Function

        Public Function GetElencoNonVaccinati(filtro As FiltriCopertura, codiceUsl As String) As dsNonVaccinati Implements ICoperturaProvider.GetElencoNonVaccinati

            Dim dsNonVaccinati As New dsNonVaccinati()

            Dim cmd As OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try

                cmd = Me.Connection.CreateCommand()

                cmd.Parameters.AddWithValue("giorni_vita", filtro.giorniVita)
                cmd.Parameters.AddWithValue("data_nascita_inizio", filtro.dataNascitaDa)
                cmd.Parameters.AddWithValue("data_nascita_fine", filtro.dataNascitaA)
                cmd.Parameters.AddWithValue("dosi", filtro.richiamo)

                ' --- Filtri sui pazienti
                Dim filtroPazienti As New StringBuilder()

                If Not String.IsNullOrEmpty(filtro.codiceComuneResidenza) Then
                    filtroPazienti.AppendFormat(" and paz_com_codice_residenza = '{0}' ", filtro.codiceComuneResidenza)
                End If

                If filtro.codiceConsultorio.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_cns_codice IN ('{0}') ", filtro.codiceConsultorio.Aggregate(Function(p, g) p & "', '" & g))
                End If

                If Not String.IsNullOrEmpty(filtro.codiceCircoscrizione) Then
                    filtroPazienti.AppendFormat(" and paz_cir_codice = '{0}' ", filtro.codiceCircoscrizione)
                End If

                If filtro.Sesso.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_sesso IN ('{0}') ", filtro.Sesso.Aggregate(Function(p, g) p & "', '" & g))
                End If

                If filtro.StatoAnagrafico.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_stato_anagrafico IN ('{0}') ", filtro.StatoAnagrafico.Aggregate(Function(p, g) p & "', '" & g))
                End If
                ' ---

                ' --- Filtri sulle vaccinazioni
                Dim filtroVaccinazioni As New StringBuilder()

                If filtro.dataEffettuazioneDa.HasValue Then
                    filtroVaccinazioni.Append(" and ves_data_effettuazione >= :ves_data_effettuazione_da ")
                    cmd.Parameters.AddWithValue("ves_data_effettuazione_da", filtro.dataEffettuazioneDa.Value)
                End If

                If filtro.dataEffettuazioneDa.HasValue Then
                    filtroVaccinazioni.Append(" and ves_data_effettuazione <= :ves_data_effettuazione_a ")
                    cmd.Parameters.AddWithValue("ves_data_effettuazione_a", filtro.dataEffettuazioneA.Value)
                End If
                ' ---

                Dim query As New StringBuilder()

                query.AppendFormat(OnVac.Queries.Copertura.OracleQueries.selElencoNonVaccinati, filtroPazienti.ToString(), filtroVaccinazioni.ToString())
                cmd.Parameters.AddWithValue("pno_azi_codice", codiceUsl)
                cmd.Parameters.AddWithValue("codice_note_annotazioni", Constants.CodiceTipoNotaPaziente.Annotazioni)
                cmd.Parameters.AddWithValue("codice_note_solleciti", Constants.CodiceTipoNotaPaziente.Solleciti)
                query.AppendFormat(" and vac_codice IN ('{0}') ", filtro.codiceVaccinazione.Aggregate(Function(p, g) p & "', '" & g))

				If filtro.tipoVaccinazioni.Count > 0 Then
					query.AppendFormat(" and vac_obbligatoria IN ('{0}') ", filtro.tipoVaccinazioni.Aggregate(Function(p, g) p & "', '" & g))
				End If


				' --- Query finale --- '
				cmd.CommandText = query.ToString()

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using adp As New OracleDataAdapter(cmd)
                    adp.Fill(dsNonVaccinati.V_NON_VACCINATI)
                End Using

            Catch ex As Exception

                LogError(ex)

                ex.InternalPreserveStackTrace()
                Throw

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return dsNonVaccinati

        End Function

        Public Function GetElencoVaccinati(filtro As FiltriCopertura, codiceUsl As String) As DsVaccinati Implements ICoperturaProvider.GetElencoVaccinati

            Dim dsVaccinati As New DsVaccinati()

            Dim cmd As OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try

                cmd = Me.Connection.CreateCommand()

                cmd.Parameters.AddWithValue("giorni_vita", filtro.giorniVita)
                cmd.Parameters.AddWithValue("data_nascita_inizio", filtro.dataNascitaDa)
                cmd.Parameters.AddWithValue("data_nascita_fine", filtro.dataNascitaA)
                cmd.Parameters.AddWithValue("dosi", filtro.richiamo)

                ' --- Filtri sui pazienti
                Dim filtroPazienti As New StringBuilder()

                If Not String.IsNullOrEmpty(filtro.codiceComuneResidenza) Then
                    filtroPazienti.AppendFormat(" and paz_com_codice_residenza = '{0}' ", filtro.codiceComuneResidenza)
                End If

                If filtro.codiceConsultorio.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_cns_codice IN ('{0}') ", filtro.codiceConsultorio.Aggregate(Function(p, g) p & "', '" & g))
                End If

                If Not String.IsNullOrEmpty(filtro.codiceCircoscrizione) Then
                    filtroPazienti.AppendFormat(" and paz_cir_codice = '{0}' ", filtro.codiceCircoscrizione)
                End If

                If filtro.Sesso.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_sesso IN ('{0}') ", filtro.Sesso.Aggregate(Function(p, g) p & "', '" & g))
                End If

                If filtro.StatoAnagrafico.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_stato_anagrafico IN ('{0}') ", filtro.StatoAnagrafico.Aggregate(Function(p, g) p & "', '" & g))
                End If
                ' ---

                ' --- Filtri sulle vaccinazioni
                Dim filtroVaccinazioni As New StringBuilder()

                If filtro.dataEffettuazioneDa.HasValue Then
                    filtroVaccinazioni.Append(" and v2.ves_data_effettuazione >= :ves_data_effettuazione_da ")
                    cmd.Parameters.AddWithValue("ves_data_effettuazione_da", filtro.dataEffettuazioneDa.Value)
                End If

                If filtro.dataEffettuazioneDa.HasValue Then
                    filtroVaccinazioni.Append(" and v2.ves_data_effettuazione <= :ves_data_effettuazione_a ")
                    cmd.Parameters.AddWithValue("ves_data_effettuazione_a", filtro.dataEffettuazioneA.Value)
                End If
                ' ---

                Dim query As New StringBuilder()

                query.AppendFormat(OnVac.Queries.Copertura.OracleQueries.selElencoVaccinati, filtroPazienti.ToString(), filtroVaccinazioni.ToString())
                cmd.Parameters.AddWithValue("pno_azi_codice", codiceUsl)
                cmd.Parameters.AddWithValue("codice_note_annotazioni", Constants.CodiceTipoNotaPaziente.Annotazioni)
                query.AppendFormat(" and vac_codice IN ('{0}') ", filtro.codiceVaccinazione.Aggregate(Function(p, g) p & "', '" & g))

                If filtro.tipoVaccinazioni.Count > 0 Then
                    query.AppendFormat(" and vac_obbligatoria IN ('{0}') ", filtro.tipoVaccinazioni.Aggregate(Function(p, g) p & "', '" & g))
                End If

                ' --- Query finale --- '
                cmd.CommandText = query.ToString()

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using adp As New OracleDataAdapter(cmd)
                    adp.Fill(dsVaccinati.ELENCO_VACCINATI)
                End Using

            Catch ex As Exception

                LogError(ex)

                ex.InternalPreserveStackTrace()
                Throw

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return dsVaccinati

        End Function

        Public Function GetNonVaccinatiMedico(filtro As FiltriCoperturaMedico, codiceUsl As String) As dsNonVaccinatiMedico Implements ICoperturaProvider.GetNonVaccinatiMedico

            Dim dsNonVaccinatiMedico As New dsNonVaccinatiMedico()
            Dim cmd As OracleCommand = Nothing
            Dim ownConnection As Boolean = False

            Try
                ' --- Filtri sui pazienti
                Dim filtroPazienti As New StringBuilder()

                If Not String.IsNullOrEmpty(filtro.codiceComuneResidenza) Then
                    filtroPazienti.AppendFormat(" and paz_com_codice_residenza = '{0}' ", filtro.codiceComuneResidenza)
                End If

                If filtro.codiceConsultorio.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_cns_codice IN ('{0}') ", filtro.codiceConsultorio.Aggregate(Function(p, g) p & "', '" & g))
                End If

                If Not String.IsNullOrEmpty(filtro.codiceCircoscrizione) Then
                    filtroPazienti.AppendFormat(" and paz_cir_codice = '{0}' ", filtro.codiceCircoscrizione)
                End If

                If filtro.Sesso.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_sesso IN ('{0}') ", filtro.Sesso.Aggregate(Function(p, g) p & "', '" & g))
                End If

                If filtro.StatoAnagrafico.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_stato_anagrafico IN ('{0}') ", filtro.StatoAnagrafico.Aggregate(Function(p, g) p & "', '" & g))
                End If
                ' ---



                ' Inizio query per il datatable dei medici
                cmd = Me.Connection.CreateCommand()

                cmd.Parameters.AddWithValue("data_nascita_inizio", filtro.dataNascitaDa)
                cmd.Parameters.AddWithValue("data_nascita_fine", filtro.dataNascitaA)

                Dim queryMedici As New StringBuilder()

                queryMedici.AppendFormat(OnVac.Queries.Copertura.OracleQueries.selElencoMediciVaccinazioni, filtroPazienti.ToString())
                queryMedici.AppendFormat(" WHERE   vac_codice IN ('{0}') ", filtro.Vaccinazioni.Select(Function(p) p.codice).Aggregate(Function(p, g) p & "', '" & g))

                If filtro.tipoVaccinazioni.Count > 0 Then
                    queryMedici.AppendFormat(" and vac_obbligatoria IN ('{0}') ", filtro.tipoVaccinazioni.Aggregate(Function(p, g) p & "', '" & g))
                End If

                If Not String.IsNullOrEmpty(filtro.codiceMedico) Then
                    queryMedici.AppendFormat(" and med_codice = :med_codice ")
                    cmd.Parameters.AddWithValue("med_codice", filtro.codiceMedico)
                Else
                    queryMedici.AppendFormat(" and (med_scadenza is null or med_scadenza >= :med_scadenza) ")
                    cmd.Parameters.AddWithValue("med_scadenza", Date.Today)
                End If

                If filtro.tipoMedico.Count > 0 Then
                    queryMedici.AppendFormat(" and med_tipo IN ('{0}') ", filtro.tipoMedico.Aggregate(Function(p, g) p & "', '" & g))
                End If

                cmd.CommandText = queryMedici.ToString()

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using adp As New OracleDataAdapter(cmd)
                    adp.Fill(dsNonVaccinatiMedico.Medici)
                End Using
                ' Fine query per il datatable dei medici



                ' Inizio query per il datatable dei pazienti
                For i As Integer = 0 To filtro.Vaccinazioni.Count - 1

                    cmd = Me.Connection.CreateCommand()

                    cmd.Parameters.AddWithValue("giorni_vita", filtro.Vaccinazioni(i).giorniVita)
                    cmd.Parameters.AddWithValue("dosi", filtro.Vaccinazioni(i).richiamo)
                    cmd.Parameters.AddWithValue("data_nascita_inizio", filtro.dataNascitaDa)
                    cmd.Parameters.AddWithValue("data_nascita_fine", filtro.dataNascitaA)


                    ' --- Filtri sulle vaccinazioni
                    Dim filtroVaccinazioni As New StringBuilder()

                    If filtro.dataEffettuazioneDa.HasValue Then
                        filtroVaccinazioni.Append(" and ves_data_effettuazione >= :ves_data_effettuazione_da ")
                        cmd.Parameters.AddWithValue("ves_data_effettuazione_da", filtro.dataEffettuazioneDa.Value)
                    End If

                    If filtro.dataEffettuazioneDa.HasValue Then
                        filtroVaccinazioni.Append(" and ves_data_effettuazione <= :ves_data_effettuazione_a ")
                        cmd.Parameters.AddWithValue("ves_data_effettuazione_a", filtro.dataEffettuazioneA.Value)
                    End If
                    ' ---

                    Dim query As New StringBuilder()

                    query.AppendFormat(OnVac.Queries.Copertura.OracleQueries.selElencoNonVaccinatiMedico, filtroPazienti.ToString(), filtroVaccinazioni.ToString())
                    cmd.Parameters.AddWithValue("pno_azi_codice", codiceUsl)
                    cmd.Parameters.AddWithValue("codice_note_annotazioni", Constants.CodiceTipoNotaPaziente.Annotazioni)
                    cmd.Parameters.AddWithValue("codice_note_appuntamenti", Constants.CodiceTipoNotaPaziente.Appuntamenti)
                    query.AppendFormat(" AND   vac_codice = :vac_codice ")
                    cmd.Parameters.AddWithValue("vac_codice", filtro.Vaccinazioni(i).codice)

                    If filtro.tipoVaccinazioni.Count > 0 Then
                        query.AppendFormat(" and vac_obbligatoria IN ('{0}') ", filtro.tipoVaccinazioni.Aggregate(Function(p, g) p & "', '" & g))
                    End If

                    If Not String.IsNullOrEmpty(filtro.codiceMedico) Then
                        query.AppendFormat(" and med_codice = :med_codice ")
                        cmd.Parameters.AddWithValue("med_codice", filtro.codiceMedico)
                    Else
                        query.AppendFormat(" and (med_scadenza is null or med_scadenza >= :med_scadenza) ")
                        cmd.Parameters.AddWithValue("med_scadenza", Date.Today)
                    End If

                    If filtro.tipoMedico.Count > 0 Then
                        query.AppendFormat(" and med_tipo IN ('{0}') ", filtro.tipoMedico.Aggregate(Function(p, g) p & "', '" & g))
                    End If

                    cmd.CommandText = query.ToString()

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using adp As New OracleDataAdapter(cmd)
                        Dim dstemp As New dsNonVaccinatiMedico()
                        adp.Fill(dstemp.ElencoNonVaccinati)
                        dsNonVaccinatiMedico.ElencoNonVaccinati.Merge(dstemp.ElencoNonVaccinati)
                    End Using

                Next
                ' Fine query per il datatable dei pazienti


                ' Riempimento del datatable con i filtri di vaccinazione
                For i As Integer = 0 To filtro.Vaccinazioni.Count - 1
                    Dim row As dsNonVaccinatiMedico.FiltriVaccinazioniRow = dsNonVaccinatiMedico.FiltriVaccinazioni.NewFiltriVaccinazioniRow()
                    row.DOSE = filtro.Vaccinazioni(i).richiamo
                    row.GIORNI_VITA = filtro.Vaccinazioni(i).giorniVita
                    row.VAC_CODICE = filtro.Vaccinazioni(i).codice
                    row.VAC_DESCRIZIONE = filtro.Vaccinazioni(i).descrizione
                    dsNonVaccinatiMedico.FiltriVaccinazioni.AddFiltriVaccinazioniRow(row)
                Next

            Catch ex As Exception

                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw

            Finally

                Me.ConditionalCloseConnection(ownConnection)
                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return dsNonVaccinatiMedico

        End Function

        Public Function GetVaccinatiMedico(filtro As FiltriCoperturaMedico, codiceUsl As String) As dsVaccinatiMedico Implements ICoperturaProvider.GetVaccinatiMedico

            Dim dsVaccinatiMedico As New dsVaccinatiMedico()
            Dim cmd As OracleCommand = Nothing
            Dim ownConnection As Boolean = False

            Try
                ' --- Filtri sui pazienti
                Dim filtroPazienti As New StringBuilder()

                If Not String.IsNullOrEmpty(filtro.codiceComuneResidenza) Then
                    filtroPazienti.AppendFormat(" and paz_com_codice_residenza = '{0}' ", filtro.codiceComuneResidenza)
                End If

                If filtro.codiceConsultorio.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_cns_codice IN ('{0}') ", filtro.codiceConsultorio.Aggregate(Function(p, g) p & "', '" & g))
                End If

                If Not String.IsNullOrEmpty(filtro.codiceCircoscrizione) Then
                    filtroPazienti.AppendFormat(" and paz_cir_codice = '{0}' ", filtro.codiceCircoscrizione)
                End If

                If filtro.Sesso.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_sesso IN ('{0}') ", filtro.Sesso.Aggregate(Function(p, g) p & "', '" & g))
                End If

                If filtro.StatoAnagrafico.Count > 0 Then
                    filtroPazienti.AppendFormat(" and paz_stato_anagrafico IN ('{0}') ", filtro.StatoAnagrafico.Aggregate(Function(p, g) p & "', '" & g))
                End If
                ' ---



                ' Inizio query per il datatable dei medici
                cmd = Me.Connection.CreateCommand()

                cmd.Parameters.AddWithValue("data_nascita_inizio", filtro.dataNascitaDa)
                cmd.Parameters.AddWithValue("data_nascita_fine", filtro.dataNascitaA)

                Dim queryMedici As New StringBuilder()

                queryMedici.AppendFormat(OnVac.Queries.Copertura.OracleQueries.selElencoMediciVaccinazioni, filtroPazienti.ToString())
                queryMedici.AppendFormat(" WHERE   vac_codice IN ('{0}') ", filtro.Vaccinazioni.Select(Function(p) p.codice).Aggregate(Function(p, g) p & "', '" & g))

                If filtro.tipoVaccinazioni.Count > 0 Then
                    queryMedici.AppendFormat(" and vac_obbligatoria IN ('{0}') ", filtro.tipoVaccinazioni.Aggregate(Function(p, g) p & "', '" & g))
                End If

                If Not String.IsNullOrEmpty(filtro.codiceMedico) Then
                    queryMedici.AppendFormat(" and med_codice = :med_codice ")
                    cmd.Parameters.AddWithValue("med_codice", filtro.codiceMedico)
                Else
                    queryMedici.AppendFormat(" and (med_scadenza is null or med_scadenza >= :med_scadenza) ")
                    cmd.Parameters.AddWithValue("med_scadenza", Date.Today)
                End If

                If filtro.tipoMedico.Count > 0 Then
                    queryMedici.AppendFormat(" and med_tipo IN ('{0}') ", filtro.tipoMedico.Aggregate(Function(p, g) p & "', '" & g))
                End If

                cmd.CommandText = queryMedici.ToString()

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using adp As New OracleDataAdapter(cmd)
                    adp.Fill(dsVaccinatiMedico.Medici)
                End Using
                ' Fine query per il datatable dei medici



                ' Inizio query per il datatable dei pazienti
                For i As Integer = 0 To filtro.Vaccinazioni.Count - 1

                    cmd = Me.Connection.CreateCommand()

                    cmd.Parameters.AddWithValue("giorni_vita", filtro.Vaccinazioni(i).giorniVita)
                    cmd.Parameters.AddWithValue("dosi", filtro.Vaccinazioni(i).richiamo)
                    cmd.Parameters.AddWithValue("data_nascita_inizio", filtro.dataNascitaDa)
                    cmd.Parameters.AddWithValue("data_nascita_fine", filtro.dataNascitaA)


                    ' --- Filtri sulle vaccinazioni
                    Dim filtroVaccinazioni As New StringBuilder()

                    If filtro.dataEffettuazioneDa.HasValue Then
                        filtroVaccinazioni.Append(" and ves_data_effettuazione >= :ves_data_effettuazione_da ")
                        cmd.Parameters.AddWithValue("ves_data_effettuazione_da", filtro.dataEffettuazioneDa.Value)
                    End If

                    If filtro.dataEffettuazioneDa.HasValue Then
                        filtroVaccinazioni.Append(" and ves_data_effettuazione <= :ves_data_effettuazione_a ")
                        cmd.Parameters.AddWithValue("ves_data_effettuazione_a", filtro.dataEffettuazioneA.Value)
                    End If
                    ' ---

                    Dim query As New StringBuilder()

                    query.AppendFormat(OnVac.Queries.Copertura.OracleQueries.selElencoVaccinatiMedico, filtroPazienti.ToString(), filtroVaccinazioni.ToString())
                    cmd.Parameters.AddWithValue("pno_azi_codice", codiceUsl)
                    cmd.Parameters.AddWithValue("codice_note_annotazioni", Constants.CodiceTipoNotaPaziente.Annotazioni)
                    cmd.Parameters.AddWithValue("codice_note_appuntamenti", Constants.CodiceTipoNotaPaziente.Appuntamenti)
                    query.AppendFormat(" AND   vac_codice = :vac_codice ")
                    cmd.Parameters.AddWithValue("vac_codice", filtro.Vaccinazioni(i).codice)

                    If filtro.tipoVaccinazioni.Count > 0 Then
                        query.AppendFormat(" and vac_obbligatoria IN ('{0}') ", filtro.tipoVaccinazioni.Aggregate(Function(p, g) p & "', '" & g))
                    End If

                    If Not String.IsNullOrEmpty(filtro.codiceMedico) Then
                        query.AppendFormat(" and med_codice = :med_codice ")
                        cmd.Parameters.AddWithValue("med_codice", filtro.codiceMedico)
                    Else
                        query.AppendFormat(" and (med_scadenza is null or med_scadenza >= :med_scadenza) ")
                        cmd.Parameters.AddWithValue("med_scadenza", Date.Today)
                    End If

                    If filtro.tipoMedico.Count > 0 Then
                        query.AppendFormat(" and med_tipo IN ('{0}') ", filtro.tipoMedico.Aggregate(Function(p, g) p & "', '" & g))
                    End If

                    cmd.CommandText = query.ToString()

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using adp As New OracleDataAdapter(cmd)
                        Dim dstemp As New dsVaccinatiMedico()
                        adp.Fill(dstemp.ElencoVaccinati)
                        dsVaccinatiMedico.ElencoVaccinati.Merge(dstemp.ElencoVaccinati)
                    End Using

                Next
                ' Fine query per il datatable dei pazienti


                ' Riempimento del datatable con i filtri di vaccinazione
                For i As Integer = 0 To filtro.Vaccinazioni.Count - 1
                    Dim row As dsVaccinatiMedico.FiltriVaccinazioniRow = dsVaccinatiMedico.FiltriVaccinazioni.NewFiltriVaccinazioniRow()
                    row.DOSE = filtro.Vaccinazioni(i).richiamo
                    row.GIORNI_VITA = filtro.Vaccinazioni(i).giorniVita
                    row.VAC_CODICE = filtro.Vaccinazioni(i).codice
                    row.VAC_DESCRIZIONE = filtro.Vaccinazioni(i).descrizione
                    dsVaccinatiMedico.FiltriVaccinazioni.AddFiltriVaccinazioniRow(row)
                Next

            Catch ex As Exception

                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw

            Finally

                Me.ConditionalCloseConnection(ownConnection)
                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return dsVaccinatiMedico

        End Function

#End Region

    End Class

End Namespace