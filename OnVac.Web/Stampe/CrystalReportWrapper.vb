Imports System.Data

Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared

Imports CrystalDecisions.ReportAppServer.ClientDoc
Imports CrystalDecisions.ReportAppServer.Controllers
Imports CrystalDecisions.ReportAppServer.ReportDefModel
Imports CrystalDecisions.ReportAppServer.CommonControls
Imports CrystalDecisions.ReportAppServer.CommLayer
Imports CrystalDecisions.ReportAppServer.CommonObjectModel
Imports CrystalDecisions.ReportAppServer.ObjectFactory
Imports CrystalDecisions.ReportAppServer.DataSetConversion
Imports CrystalDecisions.ReportAppServer.DataDefModel
Imports System.Collections.Generic
Imports System.Reflection
Imports Onit.Shared.NTier.Security


Public Class CrystalReportWrapper

    Private parameters As New Hashtable

    Public Class SortField
        Public Enum SortDirection
            Ascending
            Descending
        End Enum
        Public TableName As String
        Public FieldName As String
        Public Direction As SortField.SortDirection
    End Class

    Dim _ReportDocument As CrystalDecisions.CrystalReports.Engine.ReportDocument
    Public Property ReportDocument() As CrystalDecisions.CrystalReports.Engine.ReportDocument
        Get
            Return _ReportDocument
        End Get
        Set(ByVal Value As CrystalDecisions.CrystalReports.Engine.ReportDocument)
            _ReportDocument = Value
        End Set
    End Property

    Dim _DataSource As System.Data.DataSet
    Public Property DataSource() As System.Data.DataSet
        Get
            Return _DataSource
        End Get
        Set(ByVal Value As System.Data.DataSet)
            _DataSource = Value
        End Set
    End Property


    Public Sub SetSubReportDataSource(ByVal name As String, ByVal dst As System.Data.DataSet)
        For Each t As CrystalDecisions.CrystalReports.Engine.Table In _ReportDocument.OpenSubreport(name).Database.Tables
            'applico le logon info solo se si tratta di un database sql o se la versione è la 9
            If t.LogOnInfo.ConnectionInfo.GetType().GetProperty("Attributes") Is Nothing OrElse Not isSqlDb(t.LogOnInfo) Then
                t.SetDataSource(dst)
            End If
        Next
    End Sub

    Public Property SelectionFormula() As String
        Get
            Return _ReportDocument.RecordSelectionFormula
        End Get
        Set(ByVal Value As String)
            _ReportDocument.RecordSelectionFormula = Value
        End Set
    End Property

    'indica i tipi di formato  dei documenti da esportare
    Public Enum docFormat
        NoFormat = 0
        CrystalReport = 1
        RichText = 2
        WordForWindows = 3
        Excel = 4
        PortableDocFormat = 5
        HTML32 = 6
        HTML40 = 7
    End Enum

    Public Sub OpenSubReport(ByVal name As String)
        Me.ReportDocument.OpenSubreport(name)
    End Sub


    Public Function GetSortFields()
        '--
        Dim sortFieldsTemp As New ArrayList
        Dim sortFieldTemp As SortField
        '--
        For Each sortField As CrystalDecisions.CrystalReports.Engine.SortField In _ReportDocument.DataDefinition.SortFields
            '--
            Dim fieldTemp As CrystalDecisions.CrystalReports.Engine.DatabaseFieldDefinition = DirectCast(sortField.Field, CrystalDecisions.CrystalReports.Engine.DatabaseFieldDefinition)
            '--
            sortFieldTemp = New SortField
            sortFieldTemp.FieldName = fieldTemp.Name
            sortFieldTemp.TableName = fieldTemp.TableName
            sortFieldTemp.Direction = IIf(sortField.SortDirection = SortDirection.AscendingOrder, CrystalReportWrapper.SortField.SortDirection.Ascending, CrystalReportWrapper.SortField.SortDirection.Descending)
            '--
            sortFieldsTemp.Add(sortFieldTemp)
            '--
        Next
        '--
        Return sortFieldsTemp.ToArray(GetType(SortField))
        '--
    End Function


    Public Sub SetSortFields(ByVal sortFields As SortField())
        '--
        Dim found As Boolean
        Dim fieldTemp As SortField
        '--
        For i As Short = 0 To _ReportDocument.DataDefinition.SortFields.Count - 1
            '--
            found = False
            '--
            If i <= sortFields.Length - 1 Then
                fieldTemp = sortFields(i)
            Else
                fieldTemp = sortFields(sortFields.Length - 1)
            End If
            '--
            For Each reportTable As CrystalDecisions.CrystalReports.Engine.Table In _ReportDocument.Database.Tables
                If fieldTemp.TableName Is Nothing OrElse fieldTemp.TableName = String.Empty OrElse reportTable.Name = fieldTemp.TableName Then
                    '--
                    For Each reportField As DatabaseFieldDefinition In reportTable.Fields
                        If reportField.Name = fieldTemp.FieldName Then
                            _ReportDocument.DataDefinition.SortFields(i).Field = reportField
                            _ReportDocument.DataDefinition.SortFields(i).SortDirection = IIf(fieldTemp.Direction = SortField.SortDirection.Ascending, CrystalDecisions.[Shared].SortDirection.AscendingOrder, CrystalDecisions.[Shared].SortDirection.DescendingOrder)
                            Exit For
                        End If
                    Next
                    '--
                    If found Then Exit For
                    '--
                End If
            Next
            '--
        Next
        '--
    End Sub


    Public Sub SetDirectionFormulaSortFields(arrayDirezioni As ArrayList)
        '--
        For i As Short = 0 To arrayDirezioni.Count - 1
            Select Case DirectCast(arrayDirezioni(i), SortField.SortDirection)
                Case SortField.SortDirection.Ascending
                    _ReportDocument.DataDefinition.SortFields(i).SortDirection = SortDirection.AscendingOrder
                Case SortField.SortDirection.Descending
                    _ReportDocument.DataDefinition.SortFields(i).SortDirection = SortDirection.DescendingOrder
            End Select
        Next
    End Sub


    ' Imposta il valore della formula che restituisce il campo per cui il report deve essere ordinato.
    ' N.B. : la formula deve essere compresa tra i campi di ordinamento del report!!!
    '
    ' rpt_formula_name:     nome della formula (il nome semplice, non nel formato di crystal {@nome_formula}).
    '
    ' rpt_field_name:       il campo per cui si vuole ordinare, che sarà restituito dalla formula (anche questo deve essere
    '                       il nome semplice, non nel formato di crystal report {nome_campo}). 
    '                       Il campo può essere passato al metodo con o senza il nome della tabella (es: t_ana_pazienti.paz_cognome 
    '                       o semplicemente paz_cognome). Nel secondo caso, associa la prima tabella del report che ha un campo 
    '                       con lo stesso nome (bruttino, ma non ho trovato di meglio).
    '
    ' rpt_field_order:      Onit.[Shared].Utility.OnitReport.Report.SortField.SortDirection.Ascending,
    '                       Onit.[Shared].Utility.OnitReport.Report.SortField.SortDirection.Descending,
    '                       Nothing (se non si vuole specificare nessun ordine).
    '
    ' Se tutto ok, restituisce true.
    ' Restituisce false se la formula non è presente nel report o se non ha impostato la formula (perchè non ha trovato la tabella 
    ' o il campo in crystal report). 
    Public Function SetSortFieldFormula(ByVal rpt_formula_name As String, ByVal rpt_field_name As String, ByVal rpt_field_order As SortField.SortDirection) As Boolean
        '--
        ' Controllo che esista la formula specificata
        Dim found As Boolean = False
        For Each rptFormula As FormulaFieldDefinition In _ReportDocument.DataDefinition.FormulaFields
            If rptFormula.Name = rpt_formula_name Then
                found = True
            End If
        Next
        If Not found Then Return False
        '--
        ' Tolgo eventuali parentesi graffe (non si sa mai)
        rpt_field_name = rpt_field_name.Replace("{", "").Replace("}", "")
        '--
        Dim table_name As String = String.Empty
        Dim field_name As String = String.Empty
        found = False
        '
        If rpt_field_name.IndexOf(".") <> -1 Then
            ' E' stato specificato il nome della tabella
            Dim s() As String = rpt_field_name.Split(".")
            table_name = s(0)
            field_name = s(1)
            '--
            ' Controllo che tabella e campo siano nel report
            For Each reportTable As CrystalDecisions.CrystalReports.Engine.Table In _ReportDocument.Database.Tables
                If reportTable.Name = table_name Then
                    For Each reportField As FieldDefinition In reportTable.Fields
                        If reportField.Name = field_name Then
                            found = True
                            Exit For
                        End If
                    Next
                    If found Then Exit For
                End If
            Next
        Else
            ' Non è stato specificato il nome della tabella
            field_name = rpt_field_name
            '--
            ' Ricerca della tabella
            For Each reportTable As CrystalDecisions.CrystalReports.Engine.Table In _ReportDocument.Database.Tables
                For Each reportField As FieldDefinition In reportTable.Fields
                    If reportField.Name = field_name Then
                        table_name = reportTable.Name
                        found = True
                        Exit For
                    End If
                Next
                If found Then Exit For
            Next
        End If
        '--
        ' Controllo che il campo appartenga ad una tabella
        If Not found Then Return False
        '--
        ' Imposto la formula
        _ReportDocument.DataDefinition.FormulaFields(rpt_formula_name).Text = String.Format("{{{0}.{1}}}", table_name, field_name)
        '--
        ' Imposto l'ordine (se specificato)
        If Not IsNothing(rpt_field_order) Then
            For Each sort_field As CrystalDecisions.CrystalReports.Engine.SortField In _ReportDocument.DataDefinition.SortFields
                If sort_field.Field.Name = rpt_formula_name Then
                    sort_field.SortDirection = IIf(rpt_field_order = SortField.SortDirection.Ascending, SortDirection.AscendingOrder, SortDirection.DescendingOrder)
                End If
            Next
        End If
        '--
        Return True
        '--
    End Function


    Private Function convertFormat(ByVal f As docFormat) As CrystalDecisions.Shared.ExportFormatType
        Dim fRet As CrystalDecisions.Shared.ExportFormatType
        Select Case f
            Case docFormat.CrystalReport
                fRet = CrystalDecisions.Shared.ExportFormatType.CrystalReport
            Case docFormat.Excel
                fRet = CrystalDecisions.Shared.ExportFormatType.Excel
            Case docFormat.HTML32
                fRet = CrystalDecisions.Shared.ExportFormatType.HTML32
            Case docFormat.HTML40
                fRet = CrystalDecisions.Shared.ExportFormatType.HTML40
            Case docFormat.NoFormat
                fRet = CrystalDecisions.Shared.ExportFormatType.NoFormat
            Case docFormat.PortableDocFormat
                fRet = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat
            Case docFormat.RichText
                fRet = CrystalDecisions.Shared.ExportFormatType.RichText
            Case docFormat.WordForWindows
                fRet = CrystalDecisions.Shared.ExportFormatType.WordForWindows

        End Select
        Return fRet
    End Function

#Region "constructors"

    Sub New(ByVal reportPath As String, ByVal cnString As String, ByVal crypted As Boolean)

        _ReportDocument = New CrystalDecisions.CrystalReports.Engine.ReportDocument
        _ReportDocument.Load(reportPath, OpenReportMethod.OpenReportByDefault)

        SetDbConnection(cnString, crypted, False)

    End Sub

#End Region

    'permette di impostare la stringa di connessione qualora non sia 
    '   stato fatto chiamando il costruttore
    'PARAMETRI:
    '   cnString: stringa di connessione
    '   crypted: indica se la stringa di connessione è criptata
    '   alignTablesToDb: riallinea le tabelle al database specificato nella stringa di connessione. utile se
    '       i reports sono lanciati su un database diverso da quello in cui sono stati creati
    Public Function SetDbConnection(ByVal cnString As String, ByVal crypted As Boolean, ByVal alignTablesToDb As Boolean) As Boolean
        Dim t As CrystalDecisions.CrystalReports.Engine.Table
        Dim cnInfo As New CrystalDecisions.Shared.ConnectionInfo
        Dim strDecoded As String
        Dim parsedString() As String
        Dim i As Int16
        Dim strDbTable, strNewLocation As String

        If crypted Then
            Dim cr As New Crypto(Providers.Rijndael)
            strDecoded = cr.Decrypt(cnString)
        Else
            strDecoded = cnString
        End If
        parsedString = strDecoded.Split(";")
        For i = 0 To parsedString.GetUpperBound(0)
            Dim parsedValue() As String = parsedString(i).Split("=")
            If parsedValue.GetUpperBound(0) > 0 Then
                Select Case parsedValue(0).ToUpper
                    Case "DATA SOURCE"
                        cnInfo.ServerName = parsedValue(1)
                    Case "PASSWORD"
                        cnInfo.Password = parsedValue(1)
                    Case "DATABASE"
                        cnInfo.DatabaseName = parsedValue(1)
                    Case "USER ID"
                        cnInfo.UserID = parsedValue(1)
                    Case "INITIAL CATALOG"
                        cnInfo.DatabaseName = parsedValue(1)
                End Select
            End If
        Next i

        'nel caso non sia specificato il nome del database, uso il nome dell'utente
        If cnInfo.DatabaseName <> "" Then
            strDbTable = cnInfo.DatabaseName.Trim
        Else 'oracle
            strDbTable = cnInfo.UserID
        End If


        'Dim dbc As CrystalDecisions.ReportAppServer.Controllers.DatabaseController = _ReportDocument.ReportClientDocument.DatabaseController
        'Dim connections As CrystalDecisions.ReportAppServer.DataDefModel.ConnectionInfos = dbc.GetConnectionInfos(Nothing)
        'For Each o As CrystalDecisions.ReportAppServer.DataDefModel.ConnectionInfo In connections
        '    Dim conInfo As CrystalDecisions.ReportAppServer.DataDefModel.ISCRConnectionInfo = o.Clone(True)

        '    Dim QEProperties As CrystalDecisions.ReportAppServer.DataDefModel.PropertyBag = conInfo.Attributes
        '    QEProperties("QE_DatabaseName") = cnInfo.DatabaseName
        '    QEProperties("QE_ServerDescription") = cnInfo.ServerName

        '    conInfo.UserName = cnInfo.UserID
        '    conInfo.Password = cnInfo.Password

        '    dbc.ReplaceConnection(o, conInfo, Nothing, CrystalDecisions.ReportAppServer.DataDefModel.CrDBOptionsEnum.crDBOptionIgnoreCurrentTableQualifiers)
        'Next

        If Not IsNothing(_ReportDocument) Then
            For Each t In _ReportDocument.Database.Tables
                Dim logOnInfoTemp As TableLogOnInfo = t.LogOnInfo
                'applico le logon info solo se si tratta di un database sql o se la versione è la 9
                If logOnInfoTemp.ConnectionInfo.GetType().GetProperty("Attributes") Is Nothing OrElse isSqlDb(logOnInfoTemp) Then

                    logOnInfoTemp.ConnectionInfo = cnInfo

                    ApplyPatch(cnInfo)

                    t.ApplyLogOnInfo(logOnInfoTemp)

                    If alignTablesToDb Then
                        strNewLocation = (strDbTable + "." + t.Name).ToUpper
                        'siccome l'operazione è onerosa la cambio solo se è diversa
                        If strNewLocation <> t.Location.ToUpper Then
                            t.Location = strNewLocation
                        End If
                    End If

                End If

            Next t
        Else
            Return False
        End If

        Return True

    End Function

    Private Sub ApplyPatch(cnInfo As CrystalDecisions.Shared.ConnectionInfo)
        Dim field As FieldInfo = cnInfo.GetType().GetField("m_IsLogonPropertiesModified", BindingFlags.NonPublic Or BindingFlags.Instance)
        field.SetValue(cnInfo, True)
    End Sub

    Public Function SetDbConnectionToSubReports(ByVal cnString As String, ByVal crypted As Boolean, ByVal alignTablesToDb As Boolean) As Boolean
        Dim t As CrystalDecisions.CrystalReports.Engine.Table
        Dim cnInfo As New CrystalDecisions.Shared.ConnectionInfo
        Dim strDecoded As String
        Dim parsedString() As String
        Dim i As Int16
        Dim strDbTable, strNewLocation As String

        If crypted Then
            Dim cr As New Crypto(Providers.Rijndael)
            strDecoded = cr.Decrypt(cnString)
        Else
            strDecoded = cnString
        End If
        parsedString = strDecoded.Split(";")
        For i = 0 To parsedString.GetUpperBound(0)
            Dim parsedValue() As String = parsedString(i).Split("=")
            If parsedValue.GetUpperBound(0) > 0 Then
                Select Case parsedValue(0).ToUpper
                    Case "DATA SOURCE"
                        cnInfo.ServerName = parsedValue(1)
                    Case "PASSWORD"
                        cnInfo.Password = parsedValue(1)
                    Case "DATABASE"
                        cnInfo.DatabaseName = parsedValue(1)
                    Case "USER ID"
                        cnInfo.UserID = parsedValue(1)
                    Case "INITIAL CATALOG"
                        cnInfo.DatabaseName = parsedValue(1)
                End Select
            End If
        Next i

        'nel caso non sia specificato il nome del database, uso il nome dell'utente
        If cnInfo.DatabaseName <> "" Then
            strDbTable = cnInfo.DatabaseName.Trim
        Else 'oracle
            strDbTable = cnInfo.UserID
        End If

        If Not IsNothing(_ReportDocument) Then

            For i = 0 To _ReportDocument.Subreports.Count - 1


                For Each t In _ReportDocument.Subreports(i).Database.Tables
                    Dim logOnInfoTemp As TableLogOnInfo = t.LogOnInfo
                    'applico le logon info solo se si tratta di un database sql o se la versione è la 9
                    If logOnInfoTemp.ConnectionInfo.GetType().GetProperty("Attributes") Is Nothing OrElse isSqlDb(logOnInfoTemp) Then

                        logOnInfoTemp.ConnectionInfo = cnInfo

                        ApplyPatch(cnInfo)

                        t.ApplyLogOnInfo(logOnInfoTemp)

                        If alignTablesToDb Then
                            strNewLocation = (strDbTable + "." + t.Name).ToUpper
                            'siccome l'operazione è onerosa la cambio solo se è diversa
                            If strNewLocation <> t.Location.ToUpper Then
                                t.Location = strNewLocation
                            End If
                        End If

                    End If

                Next t
            Next i

        Else
            Return False
        End If

        Return True
    End Function



    Private Function isSqlDb(ByVal logInfo As CrystalDecisions.Shared.TableLogOnInfo) As Boolean
        Dim blnRet As Boolean
        Dim attr As Object
        Dim valObj As Object
        Dim cninfo As CrystalDecisions.Shared.ConnectionInfo
        blnRet = True
        cninfo = logInfo.ConnectionInfo
        attr = CType(cninfo, Object).Attributes
        If Not attr Is Nothing Then
            valObj = attr.Collection.Lookup("QE_SQLDB")
            If Not valObj Is Nothing AndAlso TypeOf valObj Is Boolean Then
                blnRet = CType(valObj, Boolean)
            End If
        End If
        Return blnRet
    End Function

    Public Function Export(ByVal exp As docFormat, ByVal strUtente As String, ByVal dirTemp As String, Optional ByVal formatOption As Object = Nothing) As String

        Dim filename As String
        Dim rndName As String
        Dim rptClientDoc As CrystalDecisions.ReportAppServer.ClientDoc.ISCDReportClientDocument = New CrystalDecisions.ReportAppServer.ClientDoc.ReportClientDocument()

        rptClientDoc = _ReportDocument.ReportClientDocument

        Try
            Dim rndGen As New System.Random
            rndName = rndGen.Next(99999999).ToString.PadLeft(8)
            filename = String.Format("{0}.{1}_{2}{3}", strUtente, CStr(Format(Now, "yyyyMMddhhmmss")), rndName, GetExtension(exp))
            Dim exportOpts As New CrystalDecisions.Shared.ExportOptions
            Dim diskOpts = New DiskFileDestinationOptions
            exportOpts = _ReportDocument.ExportOptions
            exportOpts.ExportFormatType = convertFormat(exp)
            exportOpts.ExportDestinationType = ExportDestinationType.DiskFile
            diskOpts.DiskFileName = dirTemp & "\" & filename
            exportOpts.DestinationOptions = diskOpts

            If Not formatOption Is Nothing Then
                exportOpts.FormatOptions = formatOption
            End If

            If Not _DataSource Is Nothing Then
                For Each t As CrystalDecisions.CrystalReports.Engine.Table In _ReportDocument.Database.Tables
                    'applico le logon info solo se si tratta di un database sql o se la versione è la 9
                    If t.LogOnInfo.ConnectionInfo.GetType().GetProperty("Attributes") Is Nothing OrElse Not isSqlDb(t.LogOnInfo) Then
                        t.SetDataSource(_DataSource)
                    End If
                Next
            End If

            Dim paramsEnum As IDictionaryEnumerator = parameters.GetEnumerator
            While paramsEnum.MoveNext
                Dim paramField = Nothing

                Try
                    paramField = _ReportDocument.DataDefinition.ParameterFields.Item(paramsEnum.Key)
                Catch ex As Exception
                    Throw New Exception(String.Format("Valorization error for parameter '{0}' in {1}", paramsEnum.Key, _ReportDocument.FileName), ex)
                End Try

                Dim discreteParam As New ParameterDiscreteValue
                Dim defaultvalues As ParameterValues
                Dim currentValues As ParameterValues
                discreteParam.Value = paramsEnum.Value
                defaultvalues = paramField.DefaultValues
                currentValues = paramField.currentValues
                defaultvalues.Add(CType(discreteParam, Object))
                currentValues.Add(CType(discreteParam, Object))
                paramField.ApplyDefaultValues(defaultvalues)
                paramField.applycurrentvalues(defaultvalues)
            End While

            _ReportDocument.Export()
        Catch exc As Exception
            Throw
        Finally
            _ReportDocument.Close()
        End Try
        Return filename
    End Function

    Private Function GetExtension(ByVal exp As ExportFormatType) As String
        Dim ext As String
        Select Case exp
            Case ExportFormatType.CrystalReport
                ext = ".rpt"
            Case ExportFormatType.Excel
                ext = ".xls"
            Case ExportFormatType.HTML32 Or ExportFormatType.HTML40
                ext = ".htm"
            Case ExportFormatType.PortableDocFormat
                ext = ".pdf"
            Case ExportFormatType.RichText
                ext = ".rtf"
            Case ExportFormatType.WordForWindows
                ext = ".doc"
            Case Else
                ext = ".xxx"
        End Select

        Return ext
    End Function

    Public Sub SetParameter(ByVal paramName As String, ByVal value As Object)
        If Not parameters.Contains(paramName) Then
            parameters.Add(paramName, value)
        Else
            parameters(paramName) = value
        End If
    End Sub

    Public Function getExportStream(ByVal exportType As docFormat) As IO.MemoryStream
        Dim s As IO.MemoryStream

        Try

            'impostazione di eventuale dataset
            If Not _DataSource Is Nothing Then
                For Each t As CrystalDecisions.CrystalReports.Engine.Table In _ReportDocument.Database.Tables
                    'applico le logon info solo se si tratta di un database sql o se la versione è la 9
                    If t.LogOnInfo.ConnectionInfo.GetType().GetProperty("Attributes") Is Nothing OrElse Not isSqlDb(t.LogOnInfo) Then
                        t.SetDataSource(_DataSource)
                    End If
                Next
            End If

            'impostazione dei parametri
            Dim paramsEnum As IDictionaryEnumerator = parameters.GetEnumerator
            While paramsEnum.MoveNext
                Dim paramField = _ReportDocument.DataDefinition.ParameterFields.Item(paramsEnum.Key)
                Dim discreteParam As New ParameterDiscreteValue
                Dim defaultvalues As ParameterValues
                Dim currentValues As ParameterValues
                discreteParam.Value = paramsEnum.Value
                defaultvalues = paramField.DefaultValues
                currentValues = paramField.currentValues
                defaultvalues.Add(CType(discreteParam, Object))
                currentValues.Add(CType(discreteParam, Object))
                paramField.ApplyDefaultValues(defaultvalues)
                paramField.applycurrentvalues(defaultvalues)
            End While

            'esportazione del report
            Dim exp As CrystalDecisions.Shared.ExportOptions = New CrystalDecisions.Shared.ExportOptions
            exp.ExportFormatType = convertFormat(exportType)
            Dim req As ExportRequestContext = New ExportRequestContext
            req.ExportInfo = exp
            s = CType(_ReportDocument.FormatEngine.ExportToStream(req), IO.MemoryStream)

        Catch ex As Exception
            Throw
        Finally
            'chiusura del documento
            _ReportDocument.Close()
        End Try


        Return s
    End Function

End Class