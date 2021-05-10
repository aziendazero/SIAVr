Imports System.Web
Imports System.Collections.Generic
Imports Onit.OnAssistnet.FileExport
Imports Onit.OnAssistnet.Data


Public Class FileExportHandler
    Implements System.Web.IHttpHandler

#Region " Export Parameters "

    Private Class ExportParameters

        Public Property IdApplicazione As String
        Public Property CodiceAzienda As String
        Public Property UserId As Long
        Public Property CodiceArgomento As String
        Public Property CodiceConsultorio As String
        Public Property CodiceAmbulatorio As Integer
        Public Property DataAppuntamentoInizio As DateTime
        Public Property DataAppuntamentoFine As DateTime
        Public Property Avvisati As Enumerators.FiltroAvvisati
        Public Property TipoAvviso As String
        Public Property CodiceCittadinanza As String
        Public Property Distretto As String
        Public Property DataNascitaInizio As DateTime?
        Public Property DataNascitaFine As DateTime?
        Public Property FiltroAssociazioniDosi As Entities.FiltroComposto
        Public Property CodiceUslCorrente As String

        Public Sub New()
            FiltroAssociazioniDosi = New Entities.FiltroComposto()
        End Sub

    End Class

    Private Function GetExportParameters(context As HttpContext) As ExportParameters

        Dim parameters As New ExportParameters()

        ' Lettura parametri da querystring
        parameters.IdApplicazione = context.Request.QueryString.Get("appId")
        parameters.CodiceAzienda = context.Request.QueryString.Get("codAzienda")
        parameters.UserId = Convert.ToInt64(context.Request.QueryString.Get("userId"))
        parameters.CodiceArgomento = context.Request.QueryString.Get("argomento")
        parameters.CodiceConsultorio = context.Request.QueryString.Get("cns")
        parameters.CodiceUslCorrente = context.Request.QueryString.Get("ulss")

        Dim distretto As String = context.Request.QueryString.Get("distr")
        If Not String.IsNullOrWhiteSpace(distretto) Then parameters.Distretto = distretto

        Dim amb As String = context.Request.QueryString.Get("amb")
        If String.IsNullOrEmpty(amb) Then
            parameters.CodiceAmbulatorio = 0
        Else
            parameters.CodiceAmbulatorio = Integer.Parse(amb)
        End If

        ' N.B. : le date di inizio e fine appuntamento sono obbligatorie
        Dim daData As String = context.Request.QueryString.Get("da")
        parameters.DataAppuntamentoInizio = Convert.ToDateTime(daData)

        Dim aData As String = context.Request.QueryString.Get("a")
        parameters.DataAppuntamentoFine = Convert.ToDateTime(aData)

        Dim avvisati As String = context.Request.QueryString.Get("avv")
        If String.IsNullOrEmpty(avvisati) Then
            parameters.Avvisati = Enumerators.FiltroAvvisati.Tutti
        Else
            parameters.Avvisati = DirectCast(Convert.ToInt32(avvisati), Enumerators.FiltroAvvisati)
        End If

        parameters.TipoAvviso = context.Request.QueryString.Get("tipoavviso")
        parameters.CodiceCittadinanza = context.Request.QueryString.Get("citt")

        Dim daNascita As String = context.Request.QueryString.Get("daNasc")
        If Not String.IsNullOrEmpty(daNascita) Then parameters.DataNascitaInizio = Convert.ToDateTime(daNascita)

        Dim aNascita As String = context.Request.QueryString.Get("aNasc")
        If Not String.IsNullOrEmpty(aNascita) Then parameters.DataNascitaFine = Convert.ToDateTime(aNascita)

        ' N.B. : da querystring il valore arriva, ma la deserializzazione non è corretta! Tutti i valori sono vuoti...
        'Dim ass As String = context.Request.QueryString.Get("ass")
        'If Not String.IsNullOrEmpty(ass) Then
        '    Dim des As New System.Web.Script.Serialization.JavaScriptSerializer()
        '    parameters.FiltroAssociazioniDosi = des.Deserialize(Of Entities.FiltroComposto)(ass)
        'End If

        Dim ass As String = context.Request.QueryString.Get("ass")
        If Not String.IsNullOrEmpty(ass) Then

            parameters.FiltroAssociazioniDosi.CodiceValore = New List(Of KeyValuePair(Of String, String))()

            Dim items As String() = ass.Split("|")
            For Each item As String In items

                Dim keyValue As String() = item.Split(";")

                parameters.FiltroAssociazioniDosi.CodiceValore.Add(
                    New KeyValuePair(Of String, String)(keyValue(0), keyValue(1)))

            Next

        End If

        Dim dosi As String = context.Request.QueryString.Get("dosi")
        If Not String.IsNullOrEmpty(dosi) Then

            parameters.FiltroAssociazioniDosi.Valori = New List(Of Integer)()

            Dim items As String() = dosi.Split("|")
            parameters.FiltroAssociazioniDosi.Valori = items.AsEnumerable().Select(Function(item) Convert.ToInt32(item)).ToList()

        End If

        Return parameters

    End Function

#End Region

#Region " IHttpHandler "

    Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest

        ' Parametri da querystring
        Dim parameters As ExportParameters = GetExportParameters(context)

        ' Connessione
        Dim app As Onit.Shared.Manager.Apps.App = Onit.Shared.Manager.Apps.App.getInstance(parameters.IdApplicazione, parameters.CodiceAzienda)

        Dim provider As String = app.DbmsProvider
        Dim connectionString As String = app.getConnectionInfo().ConnectionString

        ' Nome del file che verrà esportato
        Dim nomefileCompleto As String = String.Empty

        ' Risultato dell'export
        Dim result As Onit.OnAssistnet.FileExport.ObjectModel.FileExportResult

        Using fileExportManager As New FileExportManager(provider, connectionString)

            ' Caricamento dati argomento
            Dim argomento As ObjectModel.Argomento = fileExportManager.LoadArgomento(parameters.CodiceArgomento)

            If argomento Is Nothing Then Throw New ApplicationException("Codice argomento per POSTEL non riconosciuto!")

            ' Creazione del nome del file
            Dim fileName As String = String.Empty
            Dim fileExtension As String = String.Empty

            If Not String.IsNullOrEmpty(argomento.NomeFile) Then
                fileName = argomento.NomeFile.Substring(0, argomento.NomeFile.LastIndexOf("."))
                fileExtension = argomento.NomeFile.Substring(argomento.NomeFile.LastIndexOf("."))
            Else
                fileName = "ExportFile_" + argomento.CodiceArgomento
                fileExtension = ".txt"
            End If

            ' Nome del file che verrà esportato
            nomefileCompleto = String.Format("{0}_{1:yyyyMMddHHmmss}{2}", fileName, DateTime.Now, fileExtension)

            ' Creazione struttura filtri per l'export
            Dim filter As ObjectModel.FileExportFilter = Me.CreateFilter(parameters)

            ' Scrittura tracciato
            result = fileExportManager.ScriviTracciato(argomento, filter, context.Response.OutputStream)

        End Using

        If result.Success Then

            ' Download tracciato
            context.Response.ContentType = Constants.MIMEContentType.TEXT_PLAIN
            context.Response.AddHeader("Content-disposition", "attachment;filename=" + nomefileCompleto)

            ' Salvataggio date
            Using dbGenericProviderFactory As New Biz.DbGenericProviderFactory()
                Using genericProvider As DAL.DbGenericProvider = dbGenericProviderFactory.GetDbGenericProvider(parameters.IdApplicazione, parameters.CodiceAzienda)

                    Dim command As New Entities.PazientiAvvisiCommand()
                    command.CodiceConsultorio = parameters.CodiceConsultorio
                    command.CodiceAmbulatorio = parameters.CodiceAmbulatorio.ToString()
                    command.DataInizioAppuntamento = parameters.DataAppuntamentoInizio
                    command.DataFineAppuntamento = parameters.DataAppuntamentoFine
                    command.FiltroPazientiAvvisati = parameters.Avvisati
                    command.TipoStampaAppuntamento = Constants.TipoStampaAppuntamento.Avvisi
                    command.IsPostel = True
                    command.DataInizioNascita = parameters.DataNascitaInizio
                    command.DataFineNascita = parameters.DataNascitaFine
                    command.CodiceCittadinanza = parameters.CodiceCittadinanza
                    command.Distretti = parameters.Distretto
                    command.userId = parameters.UserId

                    Dim bizContextInfos As New Biz.BizContextInfos(parameters.UserId, parameters.CodiceAzienda, parameters.IdApplicazione, parameters.CodiceConsultorio, parameters.CodiceUslCorrente, Nothing)

                    Using bizStampa As New Biz.BizStampaInviti(genericProvider, New Settings.Settings(genericProvider), bizContextInfos)

                        bizStampa.SalvaDateStampa(command)

                    End Using

                End Using
            End Using

        Else

            ' Messaggio di errore all'utente
            context.Response.ContentType = Constants.MIMEContentType.TEXT_HTML

            Dim err As New System.Text.StringBuilder()
            err.Append("<html>")
            err.Append("<script type='text/javascript'>")
            err.Append("function showMessage() {")
            err.AppendFormat("alert('{0}');", HttpUtility.JavaScriptStringEncode(result.Message))
            err.Append("window.close();")
            err.Append("}</script>")
            err.Append("<body onload='showMessage()'>")
            err.Append("</body></html>")

            Dim outByte() As Byte = System.Text.ASCIIEncoding.Default.GetBytes(err.ToString())

            context.Response.OutputStream.Write(outByte, 0, outByte.Length)
            context.Response.OutputStream.Flush()

        End If

    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

#End Region

#Region " Private "

    ' Restituisce un oggetto di tipo filtro per la libreria di export,
    ' contenente i filtri per la query da eseguire e i parametri, con i relativi valori.
    Private Function CreateFilter(parameters As ExportParameters) As ObjectModel.FileExportFilter

        Dim filter As New ObjectModel.FileExportFilter()

        Dim filtroAvvisi As New System.Text.StringBuilder()

        filtroAvvisi.Append("(")
        filtroAvvisi.Append("(cnv_data_appuntamento >= :dataInizio AND cnv_data_appuntamento < :dataFine)")
        filtroAvvisi.Append(" OR ")
        filtroAvvisi.Append("(cnv_data_invio IS NULL AND cnv_data_appuntamento = :dataNulla AND sollecito_seduta_ciclo > num_solleciti AND seduta_ciclo_obbligatoria = :sedutaObbligatoria)")
        filtroAvvisi.Append(")")

        filter.Parameters.Add(New ObjectModel.FileExportParameter("dataInizio", parameters.DataAppuntamentoInizio, GetType(DateTime)))
        filter.Parameters.Add(New ObjectModel.FileExportParameter("dataFine", parameters.DataAppuntamentoFine.AddDays(1), GetType(DateTime)))
        filter.Parameters.Add(New ObjectModel.FileExportParameter("dataNulla", New DateTime(2100, 1, 1), GetType(DateTime)))
        filter.Parameters.Add(New ObjectModel.FileExportParameter("sedutaObbligatoria", "S", GetType(String)))

        ' Avvisati
        Select Case parameters.Avvisati

            Case Enumerators.FiltroAvvisati.SoloAvvisati
                filtroAvvisi.Append(" AND NOT cnv_data_invio IS NULL ")

            Case Enumerators.FiltroAvvisati.SoloNonAvvisati
                filtroAvvisi.Append(" AND cnv_data_invio IS NULL ")

        End Select

        ' Consultorio
        If Not String.IsNullOrEmpty(parameters.CodiceConsultorio) Then
            filtroAvvisi.Append(" AND cnv_cns_codice = :codiceCns ")
            filter.Parameters.Add(New ObjectModel.FileExportParameter("codiceCns", parameters.CodiceConsultorio, GetType(String)))
        End If

        ' Ambulatorio
        If parameters.CodiceAmbulatorio > 0 Then
            filtroAvvisi.Append(" AND cnv_amb_codice = :codiceAmb ")
            filter.Parameters.Add(New ObjectModel.FileExportParameter("codiceAmb", parameters.CodiceAmbulatorio, parameters.CodiceAmbulatorio.GetType()))
        End If
        ' Distretti
        If Not String.IsNullOrWhiteSpace(parameters.Distretto) Then
            filtroAvvisi.Append(String.Format(" AND EXISTS (SELECT 1 FROM T_ANA_CONSULTORI,T_ANA_LINK_UTENTI_CONSULTORI WHERE CNS_CODICE = cnv_cns_codice AND CNS_CODICE=LUC_CNS_CODICE AND CNS_DIS_CODICE = '{0}' AND LUC_UTE_ID = {1})", parameters.Distretto, parameters.UserId))
        Else
            filtroAvvisi.Append(String.Format(" AND EXISTS (SELECT 1 FROM T_ANA_CONSULTORI, T_ANA_LINK_UTENTI_CONSULTORI WHERE CNS_CODICE = cnv_cns_codice AND CNS_CODICE=LUC_CNS_CODICE AND LUC_UTE_ID = {0})", parameters.UserId))
        End If

        ' Tipo avviso
        If Not String.IsNullOrEmpty(parameters.TipoAvviso) Then
            filtroAvvisi.Append(" AND tipo_avviso = :tipoAvviso ")
            filter.Parameters.Add(New ObjectModel.FileExportParameter("tipoAvviso", parameters.TipoAvviso, parameters.TipoAvviso.GetType()))
        End If

        ' Cittadinanza
        If Not String.IsNullOrEmpty(parameters.CodiceCittadinanza) Then
            filtroAvvisi.Append(" AND paz_cit_codice = :paz_cit_codice ")
            filter.Parameters.Add(New ObjectModel.FileExportParameter("paz_cit_codice", parameters.CodiceCittadinanza, GetType(String)))
        End If

        ' Date di nascita
        If parameters.DataNascitaInizio.HasValue Then
            filtroAvvisi.Append(" AND paz_data_nascita >= :dataNascitaInizio ")
            filter.Parameters.Add(New ObjectModel.FileExportParameter("dataNascitaInizio", parameters.DataNascitaInizio.Value, GetType(DateTime)))
        End If

        If parameters.DataNascitaFine.HasValue Then
            filtroAvvisi.Append(" AND paz_data_nascita < :dataNascitaFine ")
            filter.Parameters.Add(New ObjectModel.FileExportParameter("dataNascitaFine", parameters.DataNascitaFine.Value.AddDays(1), GetType(DateTime)))
        End If

        ' Associazioni-dosi
        If parameters.FiltroAssociazioniDosi IsNot Nothing Then

            ' Associazioni
            If Not parameters.FiltroAssociazioniDosi.CodiceValore.IsNullOrEmpty() Then

                filtroAvvisi.Append(" AND ( ")

                For Each pair As KeyValuePair(Of String, String) In parameters.FiltroAssociazioniDosi.CodiceValore

                    filtroAvvisi.Append(String.Format("( vpr_ass_codice = '{0}' ", pair.Key))

                    If Not String.IsNullOrWhiteSpace(pair.Value) Then
                        'Numero dosi dell'associazione
                        filtroAvvisi.Append(String.Format(" AND vpr_n_richiamo IN ({0})", pair.Value))
                    End If

                    filtroAvvisi.Append(") OR ")

                Next

                filtroAvvisi.RemoveLast(3)

                filtroAvvisi.Append(" )")

            End If

            ' Numero dosi
            If Not parameters.FiltroAssociazioniDosi.Valori.IsNullOrEmpty() Then
                filtroAvvisi.Append(String.Format(" AND vpr_n_richiamo IN ({0}) ", parameters.FiltroAssociazioniDosi.Valori.GetStringForQuery()))
            End If

        End If

        filter.QueryFilter = filtroAvvisi.ToString()

        Return filter

    End Function

#End Region

End Class