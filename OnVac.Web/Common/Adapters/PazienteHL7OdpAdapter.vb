Imports Onit.Controls.OnitDataPanel
Imports Onit.OnAssistnet.OnVac.Biz

Namespace Common.Adapters

    Public Class PazienteHL7OdpAdapter
        Inherits Onit.Controls.OnitDataPanel.OnitServiceBaseAdapter

#Region " Constructors "

        Public Sub New()
            MyBase.New()
            MyBase.ResourceName = "Onit.OnAssistnet.OnVac.InfoAdapter.resources"
            MyBase.schemaEntryName = "Paziente_HL7_Schema"
        End Sub

#End Region

#Region " Constants "

        Private Const ERR_MESSAGE_PARAM As String = "strErrMsg"

#End Region

#Region " Private "

        Private _filterValorized As Integer = 0

#End Region

#Region "getting schema"

        Public Overrides Function GetColumnsFromTable(strTableName As String) As System.Data.DataTable
            Return MyBase.GetColumnsFromTable(strTableName)
        End Function

#End Region

#Region " Load "

        'questa funzione restituisce un dataset compatibile con il dtResult presente nel pannello
        Public Overrides Function LoadData(filters As FilterCollection, ByRef dt As DataTable, ByRef dtEncoder As FieldsEncoder) As OnitDataPanelError

            Dim strErrMsg As String = ""
            Dim dstPazienti As New System.Data.DataSet()
            Dim odpError As New OnitDataPanelError(True, "OnitServicePaziente.LoadData", "caricamento eseguito con successo")

            Dim codice As String

            'controllo la presenza di eventuali filtri null
            If filters.containAlwaysFalseFilter(False) Then
                dstPazienti = Me.getEmptyDataset()
            Else
                Using centrale As New BizPazienteCentrale(OnVacContext.CreateBizContextInfos(), Nothing)

                    _filterValorized = 0
                    GetValueFromFilter(filters, "")
                    codice = GetValueFromFilter(filters, "PAZ_CODICE")

                    If _filterValorized > 0 Then

                        Dim allOk As Boolean = True

                        If String.IsNullOrEmpty(codice) Then
                            '--
                            ' RICERCA IN CENTRALE IN BASE AI FILTRI IMPOSTATI
                            Dim annoNascita As Integer? = Nothing

                            If Not filters Is Nothing AndAlso filters.Count > 0 Then
                                For Each filter As Onit.Controls.OnitDataPanel.Filter In filters
                                    If filter.Field = Constants.CommonConstants.ANNO_NASCITA Then
                                        annoNascita = Convert.ToInt32(filter.Value)
                                    End If
                                Next
                            End If

                            allOk = centrale.RicercaInCentrale(String.Empty,
                                                               GetValueFromFilter(filters, "PAZ_COGNOME"),
                                                               GetValueFromFilter(filters, "PAZ_NOME"),
                                                               GetValueFromFilter(filters, "PAZ_CODICE_FISCALE"),
                                                               GetValueFromFilter(filters, "PAZ_TESSERA"),
                                                               GetValueFromFilter(filters, "PAZ_DATA_NASCITA"),
                                                               GetValueFromFilter(filters, "PAZ_COM_CODICE_NASCITA"),
                                                               GetValueFromFilter(filters, "PAZ_SESSO"),
                                                               GetValueFromFilter(filters, "PAZ_COM_CODICE_RESIDENZA"),
                                                               GetValueFromFilter(filters, "PAZ_INDIRIZZO_RESIDENZA"),
                                                               GetValueFromFilter(filters, "PAZ_CAP_RESIDENZA"),
                                                               GetValueFromFilter(filters, "PAZ_COM_CODICE_DOMICILIO"),
                                                               GetValueFromFilter(filters, "PAZ_INDIRIZZO_DOMICILIO"),
                                                               GetValueFromFilter(filters, "PAZ_CAP_DOMICILIO"),
                                                               GetValueFromFilter(filters, "PAZ_USL_CODICE_RESIDENZA"),
                                                               GetValueFromFilter(filters, "PAZ_CIT_CODICE"),
                                                               GetValueFromFilter(filters, "PAZ_TELEFONO_1"),
                                                               GetValueFromFilter(filters, "PAZ_TELEFONO_2"),
                                                               GetValueFromFilter(filters, "PAZ_TELEFONO_3"),
                                                               GetValueFromFilter(filters, "PAZ_MED_CODICE_BASE"),
                                                               GetValueFromFilter(filters, "PAZ_NOTE"),
                                                               Nothing,
                                                               GetValueFromFilter(filters, "PAZ_DATA_INSERIMENTO"),
                                                               GetValueFromFilter(filters, "PAZ_DATA_DECESSO"),
                                                               GetValueFromFilter(filters, "PAZ_DATA_CESSAZIONE_ASS"),
                                                               annoNascita,
                                                               strErrMsg,
                                                               dstPazienti)
                        Else
                            '--
                            ' RICERCA IN CENTRALE IN BASE AL CODICE DEL PAZIENTE
                            allOk = centrale.RicercaInCentrale(codice, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", Nothing, strErrMsg, dstPazienti)
                            '--
                        End If

                        If Not allOk Then
                            odpError.generateError(strErrMsg)
                        End If

                    Else

                        'la ricerca non è stata effettuata
                        dstPazienti = Me.getEmptyDataset()

                    End If

                End Using

            End If

            If odpError.ok Then
                'se tutto ok, elaboro la tabella
                Me.convertDatatable(dstPazienti.Tables(0), dt, dtEncoder)
                dstPazienti.Tables(0).Dispose()
                dstPazienti.Dispose()
            End If

            'aggiorno eventuali parametri di uscita del web method
            If Me.params.Contains(ERR_MESSAGE_PARAM) Then
                Me.params.Item(ERR_MESSAGE_PARAM) = strErrMsg
            End If

            Return odpError

        End Function

        Private Function GetValueFromFilter(filters As FilterCollection, fieldName As String)

            Dim f0 As Filter
            Dim out As String = Nothing

            For Each f0 In filters 'primo livello

                If String.Compare(f0.connectionName, Me.ConnectionName, True) = 0 Or Me.ConnectionName = "" Then
                    'controllo se il filtro è in and oppure se ce ne è solo uno in or
                    If f0.Operator = Filter.FilterOperators.And Or filters.Count = 1 Then
                        If Not f0.hasSubfilters() Then
                            If String.Compare(f0.TableName, "t_paz_pazienti_centrale", True) = 0 And _
                               (String.Compare(f0.Field, fieldName, True) = 0 Or fieldName = "") Then
                                'cerco il campo nell'elenco degli argomenti del servizio
                                'il campo del filtro è un argomento del servizio, valorizzo il parametro col valore di default
                                If f0.Comparator = Filter.FilterComparators.Like Then
                                    out = RemovePercent(f0.getValue().ToString)
                                Else
                                    out = f0.getValue()
                                End If
                                _filterValorized += 1
                                Exit For
                            End If
                        Else
                            'se ha sottofiltri, passo di ricosione
                            out = GetValueFromFilter(f0.SubFilters, fieldName)
                        End If

                    ElseIf f0.Operator = Filter.FilterOperators.Or Or filters.Count > 1 Then
                        'caso di più filtri or prendo solo il primo
                        If Not f0.hasSubfilters() Then
                            If String.Compare(f0.TableName, "t_paz_pazienti_centrale", True) = 0 And _
                               (String.Compare(f0.Field, fieldName, True) = 0 Or fieldName = "") Then
                                'cerco il campo nell'elenco degli argomenti del servizio
                                'il campo del filtro è un argomento del servizio, valorizzo il parametro col valore di default
                                If f0.Comparator = Filter.FilterComparators.Like Then
                                    out = RemovePercent(f0.getValue().ToString)
                                Else
                                    out = f0.getValue()
                                End If
                                _filterValorized += 1
                                Exit For
                            End If
                        End If

                        Exit For

                    End If

                End If
            Next

            Return out

        End Function

        Private Function RemovePercent(value As String) As String
            Dim re As System.Text.RegularExpressions.Regex = New System.Text.RegularExpressions.Regex("^%|%$")
            Return re.Replace(value, "")
        End Function

#End Region

#Region " Save "

        ''' Il metodo SaveRow non è più supportato. Usare la classe del biz BizPazienteCentrale per effettuare il salvataggio dei dati
        ''' in anagrafe centrale
        'Public Overrides Function SaveRow(drRow As System.Data.DataRow, dtEncoder As Onit.Controls.OnitDataPanel.FieldsEncoder, maintable As Onit.Controls.OnitDataPanel.MainTableInfo) As Onit.Controls.OnitDataPanel.OnitDataPanelError

        '    Dim odpError As OnitDataPanelError
        '    Dim strAddetto As String = Nothing
        '    Dim strEnte As String = Nothing
        '    Dim strErrMessage As String = Nothing

        '    Dim centrale As New PazienteCentraleWrapper()

        '    'parametri:
        '    If Me.params.Contains("strEnte") Then
        '        strEnte = Me.params("strEnte")
        '    End If
        '    If Me.params.Contains("strAddetto") Then
        '        strAddetto = Me.params("strAddetto")
        '    End If

        '    odpError = centrale.SaveRow(drRow, dtEncoder, maintable, Me.ConnectionName, strEnte, strAddetto, strErrMessage)

        '    'output dell'errore sul parametro
        '    If Me.params.Contains("strErrMessage") Then
        '        Me.params("strErrMessage") = strErrMessage
        '    End If

        '    Return odpError

        'End Function

#End Region

    End Class

End Namespace
