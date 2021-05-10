Imports System.Collections.Generic

Public Class MagazzinoUtility

#Region " Toolbar "

    ''' <summary>
    ''' Imposta le immagini (abilitata e disabilitata) della toolbar infragistics specificata, per il pulsante specificato
    ''' </summary>
    Public Shared Sub SetToolbarButtonImages(buttonKey As String, imageName As String, toolbar As Infragistics.WebUI.UltraWebToolbar.UltraWebToolbar)

        Dim OnVacImagesVirtualPath As String = "~/images/"

        toolbar.Items.FromKeyButton(buttonKey).Image = OnVacImagesVirtualPath + imageName

        Dim disabledImageName As String = imageName

        Dim extensionIndex As Integer = disabledImageName.LastIndexOf(".")

        If extensionIndex > -1 Then

            Dim extension As String = disabledImageName.Substring(extensionIndex)

            disabledImageName = disabledImageName.Substring(0, extensionIndex) + "_dis" + extension

            toolbar.Items.FromKeyButton(buttonKey).DisabledImage = OnVacImagesVirtualPath + disabledImageName

        End If

    End Sub

#End Region

#Region " Url "

    ''' <summary>
    ''' Restituisce l'url per il redirect alla locazione indicata, con la querystring completa.
    ''' I parametri vengono codificati dal metodo.
    ''' </summary>
    ''' <param name="location"></param>
    ''' <param name="filtroLottiMagazzino"></param>
    ''' <param name="campoOrdinamento"></param>
    ''' <param name="versoOrdinamento"></param>
    ''' <param name="flagRicerca"></param>
    ''' <param name="codiceLottoSelezionato"></param>
    ''' <param name="currentPageIndex"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetRedirectUrl(ByVal location As String, ByVal filtroLottiMagazzino As Filters.FiltriRicercaLottiMagazzino, ByVal campoOrdinamento As String, ByVal versoOrdinamento As String, ByVal flagRicerca As Boolean, ByVal codiceLottoSelezionato As String, currentPageIndex As Integer) As String

        Dim url As New System.Text.StringBuilder()

        url.AppendFormat("{0}?", location)

        url.AppendFormat("codL={0}", QueryStringParameterEncode(filtroLottiMagazzino.CodiceLotto))
        url.AppendFormat("&descL={0}", QueryStringParameterEncode(filtroLottiMagazzino.DescrizioneLotto))
        url.AppendFormat("&codNC={0}", QueryStringParameterEncode(filtroLottiMagazzino.CodiceNomeCommerciale))
        url.AppendFormat("&descNC={0}", QueryStringParameterEncode(filtroLottiMagazzino.DescrizioneNomeCommerciale))
        url.AppendFormat("&scad={0}", QueryStringParameterEncode(filtroLottiMagazzino.NoLottiScaduti.ToString()))
        url.AppendFormat("&null={0}", QueryStringParameterEncode(filtroLottiMagazzino.NoLottiScortaNulla.ToString()))
        url.AppendFormat("&seq={0}", QueryStringParameterEncode(filtroLottiMagazzino.SoloLottiSequestrati.ToString()))
        url.AppendFormat("&codDis={0}", QueryStringParameterEncode(filtroLottiMagazzino.CodiceDistretto))

        url.AppendFormat("&ord={0}", QueryStringParameterEncode(campoOrdinamento))
        url.AppendFormat("&ver={0}", QueryStringParameterEncode(versoOrdinamento))

        If flagRicerca Then
            url.Append("&cerca=true")
        End If

        If Not String.IsNullOrEmpty(codiceLottoSelezionato) Then
            url.AppendFormat("&codSel={0}", QueryStringParameterEncode(codiceLottoSelezionato))
        End If

        If currentPageIndex >= 0 Then
            url.AppendFormat("&page={0}", currentPageIndex.ToString())
        End If

        Return url.ToString()

    End Function

    ''' <summary>
    ''' Restituisce l'url per il redirect alla locazione indicata, con la querystring completa.
    ''' I campi di ordinamenti, il verso e il flag di ricerca vengono valorizzati in base ai valori già presenti in querystring
    ''' </summary>
    ''' <param name="location"></param>
    ''' <param name="queryString"></param>
    ''' <param name="codiceLottoSelezionato"></param>
    ''' <param name="currentPageIndex"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetRedirectUrl(ByVal location As String, ByVal queryString As System.Collections.Specialized.NameValueCollection, ByVal codiceLottoSelezionato As String) As String

        Dim url As New System.Text.StringBuilder()

        url.AppendFormat("{0}?", location)

        url.AppendFormat("codL={0}", QueryStringParameterEncode(queryString.Get("codL")))
        url.AppendFormat("&descL={0}", QueryStringParameterEncode(queryString.Get("descL")))
        url.AppendFormat("&codNC={0}", QueryStringParameterEncode(queryString.Get("codNC")))
        url.AppendFormat("&descNC={0}", QueryStringParameterEncode(queryString.Get("descNC")))
        url.AppendFormat("&scad={0}", QueryStringParameterEncode(queryString.Get("scad")))
        url.AppendFormat("&null={0}", QueryStringParameterEncode(queryString.Get("null")))
        url.AppendFormat("&seq={0}", QueryStringParameterEncode(queryString.Get("seq")))
        url.AppendFormat("&codDis={0}", QueryStringParameterEncode(queryString.Get("codDis")))

        url.AppendFormat("&ord={0}", QueryStringParameterEncode(queryString.Get("ord")))
        url.AppendFormat("&ver={0}", QueryStringParameterEncode(queryString.Get("ver")))

        url.AppendFormat("&cerca={0}", QueryStringParameterEncode(queryString.Get("cerca")))

        If Not String.IsNullOrEmpty(codiceLottoSelezionato) Then
            url.AppendFormat("&codSel={0}", QueryStringParameterEncode(codiceLottoSelezionato))
        End If

        Dim currentPageIndex As String = QueryStringParameterEncode(queryString.Get("page"))
        If Not String.IsNullOrEmpty(currentPageIndex) Then
            url.AppendFormat("&page={0}", currentPageIndex)
        End If

        Return url.ToString()

    End Function

#End Region

#Region " Querystring "

    ''' <summary>
    ''' Restituisce un oggetto contenente i filtri indicati in querystring
    ''' </summary>
    ''' <param name="queryString"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetFiltersFromQueryString(ByVal queryString As System.Collections.Specialized.NameValueCollection) As Filters.FiltriRicercaLottiMagazzino

        Dim codiceLotto As String = queryString.Get("codL")
        Dim descrizioneLotto As String = queryString.Get("descL")
        Dim codiceNomeCommerciale As String = queryString.Get("codNC")
        Dim descrizioneNomeCommerciale As String = queryString.Get("descNC")

        Dim noLottiScaduti As String = queryString.Get("scad")
        Dim noLottiScortaNulla As String = queryString.Get("null")
        Dim soloLottiSequestrati As String = queryString.Get("seq")
        Dim codDistr As String = queryString.Get("codDis")

        If String.IsNullOrEmpty(codiceLotto) AndAlso String.IsNullOrEmpty(descrizioneLotto) _
           AndAlso String.IsNullOrEmpty(codiceNomeCommerciale) AndAlso String.IsNullOrEmpty(descrizioneNomeCommerciale) _
           AndAlso String.IsNullOrEmpty(noLottiScaduti) AndAlso String.IsNullOrEmpty(noLottiScortaNulla) _
           AndAlso String.IsNullOrEmpty(soloLottiSequestrati) AndAlso String.IsNullOrWhiteSpace(codDistr) Then

            Return Nothing

        End If

        Dim filtroLottiMagazzino As New Filters.FiltriRicercaLottiMagazzino()

        filtroLottiMagazzino.CodiceLotto = codiceLotto
        filtroLottiMagazzino.DescrizioneLotto = descrizioneLotto
        filtroLottiMagazzino.CodiceNomeCommerciale = codiceNomeCommerciale
        filtroLottiMagazzino.DescrizioneNomeCommerciale = descrizioneNomeCommerciale

        If Not String.IsNullOrEmpty(noLottiScaduti) Then
            filtroLottiMagazzino.NoLottiScaduti = Convert.ToBoolean(noLottiScaduti)
        End If

        If Not String.IsNullOrEmpty(noLottiScortaNulla) Then
            filtroLottiMagazzino.NoLottiScortaNulla = Convert.ToBoolean(noLottiScortaNulla)
        End If

        If Not String.IsNullOrEmpty(soloLottiSequestrati) Then
            filtroLottiMagazzino.SoloLottiSequestrati = Convert.ToBoolean(soloLottiSequestrati)
        End If
        filtroLottiMagazzino.CodiceDistretto = codDistr

        Return filtroLottiMagazzino

    End Function

    Private Shared Function QueryStringParameterEncode(parameterValue As String)

        If String.IsNullOrEmpty(parameterValue) Then Return String.Empty

        Return HttpUtility.UrlEncode(parameterValue)

    End Function

#End Region

#Region " Datagrid "

    ''' <summary>
    ''' Restituisce un valore in formato datetime dalla cella indicata del datagrid, 
    ''' oppure nothing se il valore è nullo o non è una data
    ''' </summary>
    ''' <param name="datagridItem"></param>
    ''' <param name="columnIndex"></param>
    Public Shared Function GetDateFromDataGridItem(ByVal datagridItem As DataGridItem, ByVal columnIndex As Integer) As DateTime?

        Dim returnDate As DateTime?

        Dim value As String = datagridItem.Cells(columnIndex).Text

        Try
            returnDate = Convert.ToDateTime(value)
        Catch ex As Exception
            returnDate = Nothing
        End Try

        Return returnDate

    End Function

    ''' <summary>
    ''' Restituisce un valore in formato boolean dalla cella indicata del datagrid, 
    ''' oppure nothing se il valore è nullo o non è un booleano
    ''' </summary>
    ''' <param name="datagridItem"></param>
    ''' <param name="columnIndex"></param>
    Public Shared Function GetBooleanFromDataGridItem(ByVal datagridItem As DataGridItem, ByVal columnIndex As Integer) As Boolean?

        Dim returnValue As Boolean?

        Dim cellValue As String = datagridItem.Cells(columnIndex).Text

        If cellValue Is Nothing Then
            returnValue = Nothing
        ElseIf cellValue = String.Empty Then
            returnValue = False
        Else
            Try
                returnValue = Convert.ToBoolean(cellValue)
            Catch
                returnValue = Nothing
            End Try
        End If

        Return returnValue

    End Function

    ''' <summary>
    ''' Restituisce l'intero presente nella cella indicata del datagrid, 
    ''' oppure 0 se il valore è nullo o non è un numero
    ''' </summary>
    ''' <param name="datagridItem"></param>
    ''' <param name="columnIndex"></param>
    Public Shared Function GetIntegerFromDataGridItem(ByVal datagridItem As DataGridItem, ByVal columnIndex As Integer) As Integer

        Dim returnValue As Integer

        Try
            returnValue = Convert.ToInt32(datagridItem.Cells(columnIndex).Text)
        Catch ex As Exception
            returnValue = 0
        End Try

        Return returnValue

    End Function

#End Region

#Region " Report "

    ''' <summary>
    ''' Controlla l'esistenza del report specificato, in base al nome
    ''' </summary>
    Public Shared Function ExistsReport(nomeReport As String) As Boolean

        If String.IsNullOrEmpty(nomeReport) Then Return False

        Dim exists As Boolean = False

        ' Caricamento report
        Dim rpt As Entities.Report = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, New Settings.Settings(OnVacUtility.Variabili.CNS.Codice, genericProvider), OnVacContext.CreateBizContextInfos())

                exists = bizReport.ExistsReport(nomeReport)

            End Using
        End Using

        Return exists

    End Function

    ''' <summary>
    ''' Restituisce la cartella in cui è presente il report, in base al nome del report e all'installazione corrente
    ''' </summary>
    Public Shared Function GetCartellaReport(ByVal nomeReport As String) As String

        Dim folder As String = String.Empty

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, New Settings.Settings(OnVacUtility.Variabili.CNS.Codice, genericProvider), OnVacContext.CreateBizContextInfos())

                folder = bizReport.GetReportFolder(nomeReport)

            End Using
        End Using

        Return folder

    End Function

    ''' <summary>
    ''' Nomi dei campi utilizzati
    ''' </summary>
    Private Class CampiStampaMagazzino

        Public Const CodiceLotto As String = "CodiceLotto"
        Public Const DescrizioneLotto As String = "DescrizioneLotto"
        Public Const DescrizioneNomeCommerciale As String = "DescrizioneNomeCommerciale"
        Public Const DataPreparazione As String = "DataPreparazione"
        Public Const DataScadenza As String = "DataScadenza"
        Public Const DosiRimaste As String = "DosiRimaste"
        Public Const CodiceConsultorio As String = "CodiceConsultorio"
        Public Const Attivo As String = "Attivo"

    End Class

    ''' <summary>
    ''' Stampa l'elenco dei lotti visualizzato
    ''' </summary>
    Public Shared Sub StampaMagazzinoLotti(listLottiMagazzino As List(Of Entities.LottoMagazzino), listDatiOrdinamento As List(Of Entities.DatiOrdinamento), currentPage As Page, hideFlagLottoAttivo As Boolean, isMagazzinoCentrale As Boolean)

        Dim rpt As New ReportParameter()

        ' Il report prevede che vengano specificati 3 campi di ordinamento
        Dim campoOrdinamento1 As String = String.Empty
        Dim versoOrdinamento1 As Enumerators.VersoOrdinamento = Enumerators.VersoOrdinamento.ASC

        Dim campoOrdinamento2 As String = String.Empty
        Dim versoOrdinamento2 As Enumerators.VersoOrdinamento = Enumerators.VersoOrdinamento.ASC

        Dim campoOrdinamento3 As String = String.Empty
        Dim versoOrdinamento3 As Enumerators.VersoOrdinamento = Enumerators.VersoOrdinamento.ASC

        If Not listDatiOrdinamento Is Nothing AndAlso listDatiOrdinamento.Count > 0 Then

            campoOrdinamento1 = listDatiOrdinamento(0).Campo
            versoOrdinamento1 = listDatiOrdinamento(0).Verso

            If listDatiOrdinamento.Count > 1 Then
                campoOrdinamento2 = listDatiOrdinamento(1).Campo
                versoOrdinamento2 = listDatiOrdinamento(1).Verso
            End If

            If listDatiOrdinamento.Count > 2 Then
                campoOrdinamento3 = listDatiOrdinamento(2).Campo
                versoOrdinamento3 = listDatiOrdinamento(2).Verso
            End If

        End If

        If String.IsNullOrEmpty(campoOrdinamento2) And String.IsNullOrEmpty(campoOrdinamento3) Then
            ' Se il secondo e il terzo campo di ordinamento sono nulli
            campoOrdinamento3 = campoOrdinamento1
            versoOrdinamento3 = versoOrdinamento1

            campoOrdinamento2 = String.Empty
            versoOrdinamento2 = Enumerators.VersoOrdinamento.ASC

            campoOrdinamento1 = CampiStampaMagazzino.CodiceConsultorio
            versoOrdinamento1 = Enumerators.VersoOrdinamento.ASC

        ElseIf String.IsNullOrEmpty(campoOrdinamento3) Then
            ' Se solo il terzo campo di ordinamento è nullo
            campoOrdinamento3 = campoOrdinamento2
            versoOrdinamento3 = versoOrdinamento2

            campoOrdinamento2 = campoOrdinamento1
            versoOrdinamento2 = versoOrdinamento1

            campoOrdinamento1 = CampiStampaMagazzino.CodiceConsultorio
            versoOrdinamento1 = Enumerators.VersoOrdinamento.ASC
        End If

        ' Controllo colonne per cui deve essere invertito il verso
        versoOrdinamento1 = CheckVersoOrdinamentoStampa(campoOrdinamento1, versoOrdinamento1)
        versoOrdinamento2 = CheckVersoOrdinamentoStampa(campoOrdinamento2, versoOrdinamento2)
        versoOrdinamento3 = CheckVersoOrdinamentoStampa(campoOrdinamento3, versoOrdinamento3)

        Dim dtsLotti As System.Data.DataSet = New Lotti()

        Dim rowLotti As DataRow
        Dim rowLottiAggiuntaFittizio As DataRow
        Dim campiOrdinamentoRow As DataRow

        Dim indice As Integer = 0

        For i As Integer = 0 To listLottiMagazzino.Count - 1

            indice = indice + 1

            rowLotti = dtsLotti.Tables("element1").NewRow()

            rowLotti("lot_codice") = listLottiMagazzino(i).CodiceLotto
            rowLotti("lot_descrizione") = listLottiMagazzino(i).DescrizioneLotto
            rowLotti("noc_descrizione") = listLottiMagazzino(i).DescrizioneNomeCommerciale

            If listLottiMagazzino(i).DataPreparazione = Date.MinValue Then
                rowLotti("lot_data_preparazione") = DBNull.Value
            Else
                rowLotti("lot_data_preparazione") = listLottiMagazzino(i).DataPreparazione.ToString("dd/MM/yyyy")
            End If

            rowLotti("lot_data_scadenza") = listLottiMagazzino(i).DataScadenza.ToString("dd/MM/yyyy")

            rowLotti("lcn_dosi_rimaste") = listLottiMagazzino(i).DosiRimaste
            rowLotti("lcn_qta_minima") = listLottiMagazzino(i).QuantitaMinima

            If isMagazzinoCentrale Then
                rowLotti("lcn_attivo") = "N"
                rowLotti("lcn_cns_codice") = String.Empty
            Else
                rowLotti("lcn_attivo") = IIf(listLottiMagazzino(i).Attivo, "S", "N")
                rowLotti("lcn_cns_codice") = listLottiMagazzino(i).CodiceConsultorio
            End If

            rowLotti("lot_obsoleto") = IIf(listLottiMagazzino(i).Obsoleto, "S", "N")

            ' Fittizio
            rowLottiAggiuntaFittizio = dtsLotti.Tables("element1").NewRow()
            rowLottiAggiuntaFittizio.ItemArray = rowLotti.ItemArray
            rowLottiAggiuntaFittizio("Fittizio") = indice

            ' Ordinamento
            campiOrdinamentoRow = dtsLotti.Tables("CampiOrdinamento").NewRow()

            campiOrdinamentoRow("Fittizio") = indice

            campiOrdinamentoRow("Campo1") = GetFormattedValue(listLottiMagazzino(i), campoOrdinamento1)

            If String.IsNullOrEmpty(campoOrdinamento2) Then
                campiOrdinamentoRow("Campo2") = GetFormattedValue(listLottiMagazzino(i), campoOrdinamento1)
            Else
                campiOrdinamentoRow("Campo2") = GetFormattedValue(listLottiMagazzino(i), campoOrdinamento2)
            End If

            If String.IsNullOrEmpty(campoOrdinamento3) Then
                campiOrdinamentoRow("Campo3") = GetFormattedValue(listLottiMagazzino(i), campoOrdinamento1)
            Else
                campiOrdinamentoRow("Campo3") = GetFormattedValue(listLottiMagazzino(i), campoOrdinamento3)
            End If

            dtsLotti.Tables("element1").Rows.Add(rowLottiAggiuntaFittizio)
            dtsLotti.Tables("CampiOrdinamento").Rows.Add(campiOrdinamentoRow)

        Next

        ' Gestione Verso Ordinamento
        rpt.arraySortFields = New ArrayList()

        rpt.arraySortFields.Add(GetSortDirectionFromVersoOrdinamento(versoOrdinamento1))
        rpt.arraySortFields.Add(GetSortDirectionFromVersoOrdinamento(versoOrdinamento2))
        rpt.arraySortFields.Add(GetSortDirectionFromVersoOrdinamento(versoOrdinamento3))

        rpt.set_dataset(dtsLotti)

        ' Parametri
        If isMagazzinoCentrale Then

            rpt.AddParameter("ConsultorioLotti", "Magazzino Centrale")
            rpt.AddParameter("LottiAnnullati", String.Empty)
            rpt.AddParameter("Consultorio", String.Empty)
            rpt.AddParameter("OrdinamentoConsultorio", "N")
            rpt.AddParameter("LottoAttivo", "N")

        Else

            rpt.AddParameter("ConsultorioLotti", OnVacUtility.Variabili.CNS.Descrizione)
            rpt.AddParameter("LottiAnnullati", String.Empty)
            rpt.AddParameter("Consultorio", OnVacUtility.Variabili.CNS.Codice)

            If campoOrdinamento1 = CampiStampaMagazzino.CodiceConsultorio Then
                rpt.AddParameter("OrdinamentoConsultorio", "S")
            Else
                rpt.AddParameter("OrdinamentoConsultorio", "N")
            End If

            ' Se è abilitata la gestione dei lotti con i codici a barre, non deve considerare il lotto attivo
            If hideFlagLottoAttivo Then
                rpt.AddParameter("LottoAttivo", "N")
            Else
                rpt.AddParameter("LottoAttivo", "S")
            End If

        End If

        ' Gestione campi ordinamento
        Dim ordinamento As New ReportParameter.StrutturaOrdinamento()

        If Not String.IsNullOrEmpty(campoOrdinamento1) Then
            ordinamento.AggiungiOrdinamento("campoOrdinamento", _
                                            GetSortFieldFromCampoOrdinamento(campoOrdinamento1), _
                                            GetSortDirectionFromVersoOrdinamento(versoOrdinamento1))
        End If

        If Not String.IsNullOrEmpty(campoOrdinamento2) Then
            ordinamento.AggiungiOrdinamento("campoOrdinamento2", _
                                            GetSortFieldFromCampoOrdinamento(campoOrdinamento2), _
                                            GetSortDirectionFromVersoOrdinamento(versoOrdinamento2))
        Else
            ordinamento.AggiungiOrdinamento("campoOrdinamento2", _
                                            GetSortFieldFromCampoOrdinamento(campoOrdinamento3), _
                                            GetSortDirectionFromVersoOrdinamento(versoOrdinamento3))
        End If

        If Not String.IsNullOrEmpty(campoOrdinamento3) Then
            ordinamento.AggiungiOrdinamento("campoOrdinamento3", _
                                            GetSortFieldFromCampoOrdinamento(campoOrdinamento3), _
                                            GetSortDirectionFromVersoOrdinamento(versoOrdinamento3))
        End If

        If Not OnVacReport.StampaReport(Constants.ReportName.MagazzinoLotti, String.Empty, rpt, , ordinamento, MagazzinoUtility.GetCartellaReport(Constants.ReportName.MagazzinoLotti)) Then

            OnVacUtility.StampaNonPresente(currentPage, Constants.ReportName.MagazzinoLotti)

        End If

    End Sub

    Private Shared Function CheckVersoOrdinamentoStampa(ByVal campoOrdinamento As String, ByVal versoOrdinamento As Enumerators.VersoOrdinamento) As Enumerators.VersoOrdinamento

        ' Campi per cui verrà invertito il verso dell'ordinamento
        If campoOrdinamento = CampiStampaMagazzino.Attivo Or campoOrdinamento = CampiStampaMagazzino.CodiceConsultorio Then

            If versoOrdinamento = Enumerators.VersoOrdinamento.ASC Then
                Return Enumerators.VersoOrdinamento.DESC
            Else
                Return Enumerators.VersoOrdinamento.ASC
            End If

        End If

        Return versoOrdinamento

    End Function

    Private Shared Function GetSortDirectionFromVersoOrdinamento(ByVal versoOrdinamento As Enumerators.VersoOrdinamento) As Web.Report.SortField.SortDirection

        If versoOrdinamento = Enumerators.VersoOrdinamento.ASC Then
            Return SortDirection.Ascending
        End If

        Return SortDirection.Descending

    End Function

    Private Shared Function GetSortFieldFromCampoOrdinamento(ByVal campoOrdinamento As String) As String

        Select Case campoOrdinamento

            Case CampiStampaMagazzino.CodiceLotto
                Return "lot_codice"

            Case CampiStampaMagazzino.DescrizioneLotto
                Return "lot_descrizione"

            Case CampiStampaMagazzino.DescrizioneNomeCommerciale
                Return "noc_descrizione"

            Case CampiStampaMagazzino.DataPreparazione
                Return "lot_data_preparazione"

            Case CampiStampaMagazzino.DataScadenza
                Return "lot_data_scadenza"

            Case CampiStampaMagazzino.DosiRimaste
                Return "dosi_rimaste"

            Case CampiStampaMagazzino.CodiceConsultorio
                Return "lcn_cns_codice"

            Case CampiStampaMagazzino.Attivo
                Return "lcn_attivo"

        End Select

        Return String.Empty

    End Function

    Private Shared Function GetFormattedValue(ByVal lottoMagazzino As Entities.LottoMagazzino, ByVal campoOrdinamento As String) As String

        Select Case campoOrdinamento

            Case CampiStampaMagazzino.DataPreparazione

                If lottoMagazzino.DataPreparazione = Date.MinValue Then
                    Return String.Empty
                Else
                    Return String.Format("{0:yyyyMMdd}", lottoMagazzino.DataPreparazione)
                End If

            Case CampiStampaMagazzino.DataScadenza

                If lottoMagazzino.DataScadenza = Date.MinValue Then
                    Return String.Empty
                Else
                    Return String.Format("{0:yyyyMMdd}", lottoMagazzino.DataScadenza)
                End If

            Case CampiStampaMagazzino.DescrizioneLotto

                Return lottoMagazzino.DescrizioneLotto

            Case CampiStampaMagazzino.CodiceLotto

                Return lottoMagazzino.CodiceLotto

            Case CampiStampaMagazzino.DescrizioneNomeCommerciale

                Return lottoMagazzino.DescrizioneNomeCommerciale

            Case CampiStampaMagazzino.DosiRimaste

                Return lottoMagazzino.DosiRimaste.ToString().PadLeft(8, "0")

            Case CampiStampaMagazzino.CodiceConsultorio

                Return IIf(lottoMagazzino.CodiceConsultorio = OnVacUtility.Variabili.CNS.Codice, "C", String.Empty)

            Case CampiStampaMagazzino.Attivo

                Return IIf(lottoMagazzino.Attivo, "A", String.Empty)

        End Select

        Return String.Empty

    End Function

#End Region

End Class
