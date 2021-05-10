Imports System.Text
Imports System.Collections.Generic
Imports Onit.OnBatch.WebService.Proxy
Imports Onit.OnAssistnet.OnVac.Entities


Public Class BizElaborazionePaziente
    Inherits BizClass

#Region " Consts "

    Public Const IDProceduraElaborazioneVaccinazioniPazienti As Int32 = 10

#End Region

#Region " Events "

    Public Event ElaborazioniVaccinazioniPazientiAcquisite(sender As Object, e As ElaborazioniVaccinazioniPazientiAcquisiteEventArgs)

#End Region

#Region " Constructors "

    Public Sub New(contextInfos As BizContextInfos, logOptions As BizLogOptions)
        MyBase.New(contextInfos, logOptions)
    End Sub

    Public Sub New(ByRef settings As Onit.OnAssistnet.OnVac.Settings.Settings, uslGestitaAllineaSettingsProvider As BizUslGestitaAllineaSettingsProvider, contextInfos As BizContextInfos, logOptions As BizLogOptions)
        MyBase.New(settings, uslGestitaAllineaSettingsProvider, contextInfos, logOptions)
    End Sub

#End Region

#Region " Methods "

#Region " Public "

    Public Function CountElaborazioniVaccinazionePaziente(filter As IElaborazionePazienteProvider.ElaborazioneVaccinazionePazienteFilter) As Long

        Return Me.GenericProvider.ElaborazionePaziente.CountElaborazioniVaccinazionePaziente(filter)

    End Function

    Public Function CountElaborazioniVaccinazionePazienteSchedulate(idProcessoAcquisizioneSchedulato As Int64) As Long

        Dim elaborazioneVaccinazionePazienteFilter As New IElaborazionePazienteProvider.ElaborazioneVaccinazionePazienteFilter()
        elaborazioneVaccinazionePazienteFilter.IdProcessoAcquisizione = idProcessoAcquisizioneSchedulato

        Return Me.CountElaborazioniVaccinazionePaziente(elaborazioneVaccinazionePazienteFilter)

    End Function

    Public Function GetElaborazioneVaccinazionePaziente(id As Long) As ElaborazioneVaccinazionePaziente

        Dim elaborazioneVaccinazionePazienteFilter As New IElaborazionePazienteProvider.ElaborazioneVaccinazionePazienteFilter()
        elaborazioneVaccinazionePazienteFilter.Id = id

        Return GenericProvider.ElaborazionePaziente.GetElaborazioniVaccinazionePaziente(elaborazioneVaccinazionePazienteFilter, Nothing)(0)

    End Function

    Public Function GetElaborazioniVaccinazionePaziente(filter As IElaborazionePazienteProvider.ElaborazioneVaccinazionePazienteFilter, pagingOptions As Onit.OnAssistnet.Data.PagingOptions) As IEnumerable(Of Entities.ElaborazioneVaccinazionePaziente)

        Return GenericProvider.ElaborazionePaziente.GetElaborazioniVaccinazionePaziente(filter, pagingOptions)

    End Function

    Public Function SchedulaAcquisizioneElaborazioniVaccinazionePaziente() As RisultatoSchedulazioneAcquisizioneElaborazione

        Dim risultatoSchedulazioneAcquisizioneElaborazione As New RisultatoSchedulazioneAcquisizioneElaborazione()

        Dim onBatchProxy As New OnBatchProxy(ContextInfos.IDUtente, ContextInfos.CodiceAzienda, ContextInfos.IDApplicazione)
        onBatchProxy.AddParameter("codiceCentroVaccinale", ContextInfos.CodiceCentroVaccinale)
        onBatchProxy.AddParameter("codiceUslCorrente", ContextInfos.CodiceUsl)

        Dim idProcessoAcquisizione As Int64 = onBatchProxy.CreateProcedure(IDProceduraElaborazioneVaccinazioniPazienti)

        If idProcessoAcquisizione > -1 Then

            risultatoSchedulazioneAcquisizioneElaborazione.AcquisizioneElaborazioneSchedulateCount = GenericProvider.ElaborazionePaziente.UpdateElaborazioniVaccinazionePaziente(idProcessoAcquisizione, Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.AcquisizioneInCorso, Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.DaAcquisire)

            onBatchProxy.StartProcedure(IDProceduraElaborazioneVaccinazioniPazienti, idProcessoAcquisizione)

            risultatoSchedulazioneAcquisizioneElaborazione.Success = True

        End If

        risultatoSchedulazioneAcquisizioneElaborazione.Message = onBatchProxy.Message
        risultatoSchedulazioneAcquisizioneElaborazione.IdProcessoAcquisizioneSchedulato = idProcessoAcquisizione

        Return risultatoSchedulazioneAcquisizioneElaborazione

    End Function

    Public Function AcquisisciElaborazioniVaccinazionePazienteSchedulate(idProcessoAcquisizioneSchedulato As Int64) As RisultatoAcquisizioneElaborazione

        Dim risultatoAcquisizioneElaborazioni As New RisultatoAcquisizioneElaborazione()

        Dim elaborazioniSuccesso As New List(Of ElaborazioneVaccinazionePaziente)()
        Dim elaborazioniFallite As New List(Of ElaborazioneVaccinazionePaziente)()

        Dim elaborazioniVaccinazionePaziente As IEnumerable(Of ElaborazioneVaccinazionePaziente) =
            GenericProvider.ElaborazionePaziente.GetElaborazioniVaccinazionePazienteByIDProcesso(idProcessoAcquisizioneSchedulato)

        Dim vaccinazioniEseguitePazienti As New Dictionary(Of String, DataTable)()

        For Each elaborazioneVaccinazionePaziente As ElaborazioneVaccinazionePaziente In elaborazioniVaccinazionePaziente

            elaborazioneVaccinazionePaziente.DataAcquisizione = DateTime.Now
            elaborazioneVaccinazionePaziente.MessaggioAcquisizione = Nothing

            Try
                Dim vaccinazioniEseguiteAcquisita As VaccinazioneEseguita = Nothing

                IdentificaPaziente(elaborazioneVaccinazionePaziente)

                Dim isConsensoBloccante As Boolean = False

                If elaborazioneVaccinazionePaziente.CodicePazienteAcquisizione.HasValue Then

                    ' Controllo consenso GLOBALE del paziente
                    Using bizPaziente As New BizPaziente(GenericProvider, Settings, ContextInfos, Nothing)
                        isConsensoBloccante = Not bizPaziente.IsVisibilitaConcessaByStatoConsensoGlobale(elaborazioneVaccinazionePaziente.CodicePazienteAcquisizione.Value)
                    End Using

                    If isConsensoBloccante Then

                        ' Consenso globale BLOCCANTE => no acquisizione
                        elaborazioneVaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.ConsensoPazienteBloccante
                        elaborazioneVaccinazionePaziente.MessaggioAcquisizione = "Consenso globale del paziente BLOCCANTE."

                    Else

                        If Not vaccinazioniEseguitePazienti.ContainsKey(elaborazioneVaccinazionePaziente.CodicePazienteAcquisizione) Then
                            vaccinazioniEseguitePazienti(elaborazioneVaccinazionePaziente.CodicePazienteAcquisizione) = GenericProvider.VaccinazioniEseguite.GetVaccinazioniEseguite(elaborazioneVaccinazionePaziente.CodicePazienteAcquisizione)
                        End If

                        AggiornaVaccinazioniEseguite(elaborazioneVaccinazionePaziente, vaccinazioniEseguitePazienti(elaborazioneVaccinazionePaziente.CodicePazienteAcquisizione))

#Region " FSE "
                        'Invio transaction ITI-61 (indicizzazione sul registry regionale)
                        If elaborazioneVaccinazionePaziente.CodicePaziente.HasValue Then
                            Using bizPazienti As New BizPaziente(GenericProvider, Settings, ContextInfos, Nothing)
                                bizPazienti.IndicizzaSuRegistry(
                                    elaborazioneVaccinazionePaziente.CodicePaziente,
                                    Constants.TipoDocumentoFSE.CertificatoVaccinale,
                                    Constants.FunzionalitaNotificaFSE.BatchElaborazioneVaccinazionePaziente,
                                    Constants.EventoNotificaFSE.InserimentoVaccinazione,
                                    Settings,
                                    elaborazioneVaccinazionePaziente.CodiceOperatore)
                            End Using
                        End If
#End Region

                    End If

                End If

                Me.GenericProvider.ElaborazionePaziente.UpdateElaborazioneVaccinazionePaziente(elaborazioneVaccinazionePaziente)

                If Not isConsensoBloccante AndAlso elaborazioneVaccinazionePaziente.CodicePazienteAcquisizione.HasValue Then
                    vaccinazioniEseguitePazienti(elaborazioneVaccinazionePaziente.CodicePazienteAcquisizione).AcceptChanges()
                End If

            Catch ex As Exception

                Dim messagioProcessazioneUpdated As New StringBuilder()

                Me.AddExceptionMessaggioProcessazione(ex, messagioProcessazioneUpdated)

                elaborazioneVaccinazionePaziente.MessaggioAcquisizione = messagioProcessazioneUpdated.ToString()
                elaborazioneVaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.Eccezione

                Me.GenericProvider.ElaborazionePaziente.UpdateElaborazioneVaccinazionePaziente(elaborazioneVaccinazionePaziente)

            Finally

                If elaborazioneVaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.AcquisitaCorrettamente Then
                    elaborazioniSuccesso.Add(elaborazioneVaccinazionePaziente)
                Else
                    elaborazioniFallite.Add(elaborazioneVaccinazionePaziente)
                End If

                RaiseEvent ElaborazioniVaccinazioniPazientiAcquisite(Me, New ElaborazioniVaccinazioniPazientiAcquisiteEventArgs(elaborazioniSuccesso.Count + elaborazioniFallite.Count, elaborazioniFallite.Count, elaborazioneVaccinazionePaziente))

            End Try

        Next

        risultatoAcquisizioneElaborazioni.Successo = elaborazioniSuccesso.AsEnumerable
        risultatoAcquisizioneElaborazioni.Fallite = elaborazioniFallite.AsEnumerable

        Return risultatoAcquisizioneElaborazioni

    End Function

#End Region

#Region " Private "

    Private Sub IdentificaPaziente(elaborazioneVaccinazionePaziente As ElaborazioneVaccinazionePaziente)

        Dim codicePazientiIdentificati As IEnumerable(Of String) = Nothing

        Dim criterioIdentificazionePaziente As String = Nothing

        Dim identificazionePazienteMessageStringBuilder As New System.Text.StringBuilder()

        If elaborazioneVaccinazionePaziente.CodicePaziente.HasValue Then

            If GenericProvider.Paziente.Exists(Convert.ToInt32(elaborazioneVaccinazionePaziente.CodicePaziente.Value)) Then

                codicePazientiIdentificati = New String() {elaborazioneVaccinazionePaziente.CodicePaziente.Value.ToString()}

                criterioIdentificazionePaziente = "Codice Paziente"

            End If

        End If

        If codicePazientiIdentificati Is Nothing OrElse codicePazientiIdentificati.Count = 0 Then

            If Not String.IsNullOrEmpty(elaborazioneVaccinazionePaziente.CodiceRegionalePaziente) Then

                codicePazientiIdentificati = GenericProvider.Paziente.GetCodicePazientiByCodiceRegionale(elaborazioneVaccinazionePaziente.CodiceRegionalePaziente)

                criterioIdentificazionePaziente = "Codice Regionale Paziente"

            End If

        End If

        If codicePazientiIdentificati Is Nothing OrElse codicePazientiIdentificati.Count = 0 Then

            If Not String.IsNullOrEmpty(elaborazioneVaccinazionePaziente.TesseraSanitariaPaziente) Then

                codicePazientiIdentificati = GenericProvider.Paziente.GetCodicePazientiByTessera(elaborazioneVaccinazionePaziente.TesseraSanitariaPaziente)

                criterioIdentificazionePaziente = "Tessera Sanitaria"

            End If

        End If

        If codicePazientiIdentificati Is Nothing OrElse codicePazientiIdentificati.Count = 0 Then

            If Not String.IsNullOrEmpty(elaborazioneVaccinazionePaziente.CodiceFiscalePaziente) Then

                codicePazientiIdentificati = GenericProvider.Paziente.GetCodicePazientiByCodiceFiscale(elaborazioneVaccinazionePaziente.CodiceFiscalePaziente)

                criterioIdentificazionePaziente = "Codice Fiscale"

            End If

        End If

        If codicePazientiIdentificati Is Nothing OrElse codicePazientiIdentificati.Count = 0 Then

            If Not String.IsNullOrEmpty(elaborazioneVaccinazionePaziente.NomePaziente) OrElse
               Not String.IsNullOrEmpty(elaborazioneVaccinazionePaziente.CognomePaziente) OrElse
               Not String.IsNullOrEmpty(elaborazioneVaccinazionePaziente.SessoPaziente) OrElse
               Not String.IsNullOrEmpty(elaborazioneVaccinazionePaziente.DataNascitaPaziente) OrElse
               Not String.IsNullOrEmpty(elaborazioneVaccinazionePaziente.CodiceComuneNascitaPaziente) Then

                codicePazientiIdentificati = GenericProvider.Paziente.GetCodicePazientiByComponentiCodiceFiscale(elaborazioneVaccinazionePaziente.NomePaziente, elaborazioneVaccinazionePaziente.CognomePaziente, elaborazioneVaccinazionePaziente.SessoPaziente, elaborazioneVaccinazionePaziente.DataNascitaPaziente, elaborazioneVaccinazionePaziente.CodiceComuneNascitaPaziente)

                criterioIdentificazionePaziente = "Nome, Cognome, Sesso, Data Nascita, Comune Nascita"

            End If

        End If

        If Not codicePazientiIdentificati Is Nothing AndAlso codicePazientiIdentificati.Count > 0 Then

            identificazionePazienteMessageStringBuilder.AppendFormat("Criterio Identificazione: {0}", criterioIdentificazionePaziente)

            If codicePazientiIdentificati.Count > 1 Then

                elaborazioneVaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.CorrispondenzaPazienteMultipla

                identificazionePazienteMessageStringBuilder.AppendFormat(" [{0}]", String.Join(" / ", codicePazientiIdentificati.ToArray()))

            Else

                elaborazioneVaccinazionePaziente.CodicePazienteAcquisizione = Convert.ToInt64(codicePazientiIdentificati(0))

            End If

        Else

            elaborazioneVaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.NoCorrispondenzaPaziente

        End If

        If identificazionePazienteMessageStringBuilder.Length > 0 Then

            elaborazioneVaccinazionePaziente.MessaggioAcquisizione = String.Format("{0}{1}{2}", elaborazioneVaccinazionePaziente.MessaggioAcquisizione, IIf(String.IsNullOrEmpty(elaborazioneVaccinazionePaziente.MessaggioAcquisizione), String.Empty, Environment.NewLine), identificazionePazienteMessageStringBuilder.ToString())

        End If

    End Sub

    Private Sub AggiornaVaccinazioniEseguite(elaborazioneVaccinazionePaziente As ElaborazioneVaccinazionePaziente, dtEseguite As DataTable)

        Dim hasError As Boolean = False
        Dim numeroRichiamo As Int32
        Dim assDose As Int32 = 1
        Dim vaccinazioneEseguita As DataRow = Nothing

        If dtEseguite Is Nothing Then
            Throw New ApplicationException("Non è possibile che il dt delle eseguite sia nullo!")
        End If

        Dim dataNascita As DateTime = GenericProvider.Paziente.GetDataNascita(elaborazioneVaccinazionePaziente.CodicePazienteAcquisizione.Value)
        If dataNascita <> DateTime.MinValue AndAlso elaborazioneVaccinazionePaziente.DataNascitaPaziente > elaborazioneVaccinazionePaziente.DataEffettuazione Then

            hasError = True
            elaborazioneVaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.Eccezione
            elaborazioneVaccinazionePaziente.MessaggioAcquisizione = String.Format("{0}{1}Data di effettuazione della vaccinazione incompatibile con la data di nascita del paziente: {2}",
                    elaborazioneVaccinazionePaziente.MessaggioAcquisizione,
                    IIf(String.IsNullOrEmpty(elaborazioneVaccinazionePaziente.MessaggioAcquisizione), String.Empty, Environment.NewLine),
                    dataNascita.ToString("d"))

        End If

        Dim dtEseguiteEnumerable As IEnumerable(Of DataRow) = dtEseguite.
            AsEnumerable().
            Where(Function(p) p("ves_vac_codice") = elaborazioneVaccinazionePaziente.CodiceVaccinazione AndAlso p("scaduta") = "N")

        ' Controllo se presente lo stesso codice vaccinazione in stessa data
        If dtEseguiteEnumerable.Any(Function(p) p("ves_data_effettuazione") = elaborazioneVaccinazionePaziente.DataEffettuazione) Then
            elaborazioneVaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.VaccinazioneEsistente
            hasError = True
        End If

        If Not hasError Then

            ' Controllo se la vaccinazione si può inserire in un buco tra i richiami
            Dim vaccinazioneEseguitaDataSuccessiva As DataRow = dtEseguiteEnumerable.
                Where(Function(p) p("ves_data_effettuazione") > elaborazioneVaccinazionePaziente.DataEffettuazione).
                OrderBy(Function(p) p("ves_n_richiamo")).
                FirstOrDefault()

            If vaccinazioneEseguitaDataSuccessiva Is Nothing Then

                ' Non esiste una vaccinazione in data successiva, non è un buco
                Dim numeroRichiamoMax As Object = dtEseguiteEnumerable.Max(Function(pe) pe("ves_n_richiamo"))
                If numeroRichiamoMax Is Nothing Then
                    numeroRichiamo = 1
                Else
                    numeroRichiamo = Convert.ToInt32(numeroRichiamoMax) + 1
                End If

            Else

                Dim numeroRichiamoVaccinazioneEseguitaDataSuccessiva As Int32 = Convert.ToInt32(vaccinazioneEseguitaDataSuccessiva("ves_n_richiamo"))
                Dim vaccinazioneEseguitaDataPrecedente As DataRow = dtEseguiteEnumerable.
                    Where(Function(p) p("ves_data_effettuazione") < elaborazioneVaccinazionePaziente.DataEffettuazione).
                    OrderBy(Function(p) p("ves_n_richiamo")).
                    LastOrDefault()

                If vaccinazioneEseguitaDataPrecedente Is Nothing Then

                    If numeroRichiamoVaccinazioneEseguitaDataSuccessiva > 1 Then
                        numeroRichiamo = 1
                    Else
                        elaborazioneVaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.VaccinazioneEsistenteInDataSuccessiva
                    End If

                Else

                    Dim numeroRichiamoVaccinazioneEseguitaDataPrecedente As Int32 = Convert.ToInt32(vaccinazioneEseguitaDataPrecedente("ves_n_richiamo"))
                    Dim dosiMancanti As Int32 = Math.Abs(numeroRichiamoVaccinazioneEseguitaDataSuccessiva - numeroRichiamoVaccinazioneEseguitaDataPrecedente) - 1

                    If dosiMancanti > 0 Then
                        numeroRichiamo = numeroRichiamoVaccinazioneEseguitaDataPrecedente + 1
                    Else
                        elaborazioneVaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.VaccinazioneEsistenteInDataSuccessiva
                    End If

                End If

                If elaborazioneVaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.VaccinazioneEsistenteInDataSuccessiva Then

                    hasError = True
                    elaborazioneVaccinazionePaziente.MessaggioAcquisizione = String.Format("{0}{1}Vaccinazione Esistente In Data Successiva: {2} [{3}]",
                            elaborazioneVaccinazionePaziente.MessaggioAcquisizione,
                            IIf(String.IsNullOrEmpty(elaborazioneVaccinazionePaziente.MessaggioAcquisizione), String.Empty, Environment.NewLine),
                            DirectCast(vaccinazioneEseguitaDataSuccessiva("ves_data_effettuazione"), DateTime).ToString("d"),
                            numeroRichiamoVaccinazioneEseguitaDataSuccessiva)

                End If

            End If

        End If

        If Not hasError Then

            Dim dt As DataTable = dtEseguite.Clone()
            Dim dv As DataView

            If dtEseguite.Rows.Count > 0 Then

                ' Stessa data e stessa associazione di una precedente
                dv = New DataView(dtEseguite)
                dv.RowFilter = String.Format("ves_ass_codice = '{0}' and ves_vac_codice <> '{1}' and ves_data_effettuazione = {2}",
                                             elaborazioneVaccinazionePaziente.CodiceAssociazione,
                                             elaborazioneVaccinazionePaziente.CodiceVaccinazione,
                                             FormatForDataView(elaborazioneVaccinazionePaziente.DataEffettuazione.Date))

                If dv.Count > 0 Then
                    assDose = dv(0)("ves_ass_n_dose")
                Else

                    ' Genero il datatable con associazione e dose
                    For j As Integer = 0 To dtEseguite.Rows.Count - 1

                        If dtEseguite.Rows(j).RowState <> DataRowState.Deleted Then
                            If dtEseguite.Rows(j)("ves_ass_codice").ToString = elaborazioneVaccinazionePaziente.CodiceAssociazione Then
                                dv = New DataView(dt)
                                dv.RowFilter = String.Format("ves_ass_codice = '{0}' and ves_ass_n_dose = {1}",
                                                             dtEseguite.Rows(j)("ves_ass_codice").ToString(),
                                                             dtEseguite.Rows(j)("ves_ass_n_dose"))

                                If dv.Count = 0 Then

                                    Dim drow As DataRow = dt.NewRow()
                                    drow("ves_ass_codice") = dtEseguite.Rows(j)("ves_ass_codice")
                                    drow("ves_ass_n_dose") = dtEseguite.Rows(j)("ves_ass_n_dose")
                                    drow("ves_data_effettuazione") = dtEseguite.Rows(j)("ves_data_effettuazione")
                                    dt.Rows.Add(drow)

                                End If
                            End If
                        End If

                    Next

                    Dim dtEnumerable As IEnumerable(Of DataRow) = dt.AsEnumerable()

                    ' Controllo se la vaccinazione si può inserire in un buco tra i richiami
                    Dim vaccinazioneEseguitaDataSuccessiva As DataRow = dtEnumerable.
                        Where(Function(p) p("ves_data_effettuazione") > elaborazioneVaccinazionePaziente.DataEffettuazione).
                        OrderBy(Function(p) p("ves_ass_n_dose")).
                        FirstOrDefault()

                    If vaccinazioneEseguitaDataSuccessiva Is Nothing Then

                        ' Non esiste una vaccinazione in data successiva, non è un buco
                        Dim doseMax As Object = dtEnumerable.Max(Function(pe) pe("ves_ass_n_dose"))
                        If doseMax Is Nothing Then
                            assDose = 1
                        Else
                            assDose = Convert.ToInt32(doseMax) + 1
                        End If

                    Else

                        Dim doseDataSuccessiva As Int32 = Convert.ToInt32(vaccinazioneEseguitaDataSuccessiva("ves_ass_n_dose"))
                        Dim vaccinazioneEseguitaDataPrecedente As DataRow = dtEnumerable.
                            Where(Function(p) p("ves_data_effettuazione") < elaborazioneVaccinazionePaziente.DataEffettuazione).
                            OrderBy(Function(p) p("ves_ass_n_dose")).
                            LastOrDefault()

                        If vaccinazioneEseguitaDataPrecedente Is Nothing Then

                            If doseDataSuccessiva > 1 Then
                                assDose = 1
                            Else
                                elaborazioneVaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.VaccinazioneEsistenteInDataSuccessiva
                            End If

                        Else

                            Dim doseDataPrecedente As Int32 = Convert.ToInt32(vaccinazioneEseguitaDataPrecedente("ves_ass_n_dose"))
                            Dim dosiMancanti As Int32 = Math.Abs(doseDataSuccessiva - doseDataPrecedente) - 1

                            If dosiMancanti > 0 Then
                                assDose = doseDataPrecedente + 1
                            Else
                                elaborazioneVaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.VaccinazioneEsistenteInDataSuccessiva
                            End If

                        End If

                        If elaborazioneVaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.VaccinazioneEsistenteInDataSuccessiva Then

                            hasError = True
                            elaborazioneVaccinazionePaziente.MessaggioAcquisizione = String.Format("{0}{1}Associazione Esistente In Data Successiva: {2} [{3}]",
                                elaborazioneVaccinazionePaziente.MessaggioAcquisizione,
                                IIf(String.IsNullOrEmpty(elaborazioneVaccinazionePaziente.MessaggioAcquisizione), String.Empty, Environment.NewLine),
                                DirectCast(vaccinazioneEseguitaDataSuccessiva("ves_data_effettuazione"), DateTime).ToString("d"),
                                doseDataSuccessiva)

                        End If

                    End If

                End If

            End If

        End If

        If Not hasError Then

            vaccinazioneEseguita = dtEseguite.NewRow()

            vaccinazioneEseguita("ves_n_richiamo") = numeroRichiamo
            vaccinazioneEseguita("paz_codice") = elaborazioneVaccinazionePaziente.CodicePazienteAcquisizione
            vaccinazioneEseguita("ves_dataora_effettuazione") = elaborazioneVaccinazionePaziente.DataEffettuazione
            vaccinazioneEseguita("ves_data_effettuazione") = elaborazioneVaccinazionePaziente.DataEffettuazione.Date
            vaccinazioneEseguita("ves_vac_codice") = elaborazioneVaccinazionePaziente.CodiceVaccinazione
            vaccinazioneEseguita("ves_ass_codice") = elaborazioneVaccinazionePaziente.CodiceAssociazione
            vaccinazioneEseguita("ves_ope_codice") = elaborazioneVaccinazionePaziente.CodiceOperatore
            vaccinazioneEseguita("scaduta") = "N"
            vaccinazioneEseguita("ves_cns_registrazione") = ContextInfos.CodiceCentroVaccinale
            vaccinazioneEseguita("ves_stato") = "R"
            vaccinazioneEseguita("ves_ass_n_dose") = assDose

            ' N.B. : il valore di visibilità dei dati, in questo caso, dipende solo dallo stato del consenso alla COMUNICAZIONE e non da quello globale.
            Using bizPaziente As New BizPaziente(GenericProvider, Settings, ContextInfos, Nothing)
                vaccinazioneEseguita("ves_flag_visibilita") =
                    bizPaziente.GetValoreVisibilitaDatiVaccinaliPaziente(elaborazioneVaccinazionePaziente.CodicePazienteAcquisizione, True)
            End Using

            dtEseguite.Rows.Add(vaccinazioneEseguita)

            Dim bizVaccinazioniEseguite As BizVaccinazioniEseguite = Nothing
            Dim bizVaccinazioniProg As BizVaccinazioneProg = Nothing
            Dim bizConvocazione As BizConvocazione = Nothing

            Dim vaccinazioneEseguitaList As New List(Of VaccinazioneEseguita)()
            Dim vaccinazioneEseguitaEliminataList As New List(Of VaccinazioneEseguita)()

            Try
                bizVaccinazioniEseguite = New BizVaccinazioniEseguite(GenericProviderFactory, Settings, ContextInfos, LogOptions)

                bizVaccinazioniEseguite.SalvaNoTransactionScope(elaborazioneVaccinazionePaziente.CodicePazienteAcquisizione, dtEseguite)

                bizVaccinazioniProg = New BizVaccinazioneProg(GenericProviderFactory, Settings, ContextInfos, LogOptions)

                If bizVaccinazioniProg.EliminaVaccinazioneProgrammataByRichiamo(elaborazioneVaccinazionePaziente.CodicePazienteAcquisizione,
                                                                                elaborazioneVaccinazionePaziente.CodiceVaccinazione,
                                                                                numeroRichiamo) Then

                    bizConvocazione = New BizConvocazione(GenericProviderFactory, Settings, ContextInfos, LogOptions)

                    Dim command As New BizConvocazione.EliminaConvocazioneEmptyCommand()
                    command.CodicePaziente = elaborazioneVaccinazionePaziente.CodicePazienteAcquisizione
                    command.DataConvocazione = Nothing
                    command.DataEliminazione = DateTime.Now
                    command.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.Esecuzione
                    command.NoteEliminazione = "Eliminazione convocazione per acquisizione eseguite da ElaborazionePaziente"
                    command.WriteLog = True

                    bizConvocazione.EliminaConvocazioneEmpty(command)

                End If

            Finally

                If Not bizVaccinazioniEseguite Is Nothing Then bizVaccinazioniEseguite.Dispose()
                If Not bizVaccinazioniProg Is Nothing Then bizVaccinazioniProg.Dispose()
                If Not bizConvocazione Is Nothing Then bizConvocazione.Dispose()

            End Try

            elaborazioneVaccinazionePaziente.StatoAcquisizione = Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente.AcquisitaCorrettamente

        End If

    End Sub

    Private Function FormatForDataView(d As DateTime) As String

        Return String.Format(System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat, "#{0}#", d)

    End Function

#Region " Message "

    Private Sub AddExceptionMessaggioProcessazione(exc As Exception, messaggioProcessazione As StringBuilder)

        Dim messaggioException As New StringBuilder()

        Dim excTemp As Exception = exc

        While Not excTemp Is Nothing

            If messaggioException.Length > 0 Then messaggioException.AppendLine()

            messaggioException.AppendLine(excTemp.Message)
            messaggioException.AppendFormat("[{0}]", excTemp.StackTrace)

            excTemp = excTemp.InnerException

        End While

        Me.AddMessaggiooProcessazione("EXCEPTION", messaggioException.ToString(), messaggioProcessazione)

    End Sub

    Private Sub AddMessaggiooProcessazione(title As String, content As String, messaggioProcessazione As StringBuilder)

        If messaggioProcessazione.Length > 0 Then
            messaggioProcessazione.AppendLine()
        End If

        messaggioProcessazione.AppendFormat("{0}:", title)
        messaggioProcessazione.AppendLine()
        messaggioProcessazione.AppendLine(content)

    End Sub

#End Region

#End Region

#End Region

#Region " Types "

    Public Structure RisultatoSchedulazioneAcquisizioneElaborazione
        Public Property Success As Boolean
        Public Property Message As String
        Public Property AcquisizioneElaborazioneSchedulateCount As Int32
        Public Property IdProcessoAcquisizioneSchedulato As Int64
    End Structure

    Public Structure RisultatoAcquisizioneElaborazione
        Public Property Successo As IEnumerable(Of ElaborazioneVaccinazionePaziente)
        Public Property Fallite As IEnumerable(Of ElaborazioneVaccinazionePaziente)
    End Structure

    Public Class ElaborazioniVaccinazioniPazientiAcquisiteEventArgs
        Inherits EventArgs

        Public ReadOnly TotaleElaborazioni As Int32
        Public ReadOnly TotaleElaborazioniFallite As Int32
        Public ReadOnly ElaborazioneVaccinazionePaziente As ElaborazioneVaccinazionePaziente

        Public Sub New(totaleElaborazioni As Int32, totaleElaborazioniErrore As Int32, elaborazioneVaccinazionePaziente As ElaborazioneVaccinazionePaziente)
            Me.TotaleElaborazioni = totaleElaborazioni
            Me.TotaleElaborazioniFallite = totaleElaborazioniErrore
            Me.ElaborazioneVaccinazionePaziente = elaborazioneVaccinazionePaziente
        End Sub

    End Class

#End Region

End Class
