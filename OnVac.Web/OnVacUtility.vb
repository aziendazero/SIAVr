Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports Onit.Controls
Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.Biz
Imports Onit.OnAssistnet.OnVac.Common.Utility
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.Shared.NTier.Security


Public Class OnVacUtility

#Region " Classes "

    Public Class DataGridColumnIndexes
        Inherits Collection(Of String)

        Default Public Overloads ReadOnly Property Item(columnName As String) As Int32
            Get
                Return Me.IndexOf(columnName)
            End Get
        End Property

    End Class

#End Region

#Region " Fields / Properties "

    Public Shared Property Variabili() As VariabiliOnVac
        Get
            Return HttpContext.Current.Session("Variabili")
        End Get
        Set(Value As VariabiliOnVac)
            HttpContext.Current.Session("Variabili") = Value
        End Set
    End Property

    'contiene il datatable con tutte le vaccinazioni sostitute [modifica 08/08/2005]
    Public Shared Property dt_VacSostitute() As DataTable
        Get
            Return HttpContext.Current.Session("OnVac_dt_VacSostitute")
        End Get
        Set(Value As DataTable)
            HttpContext.Current.Session("OnVac_dt_VacSostitute") = Value
        End Set
    End Property

    Public Shared ReadOnly Property OnSharePortalIntegrated() As Boolean
        Get
            Dim onSharePortalIntegratedTemp As String = ConfigurationManager.AppSettings("OnSharePortalIntegrated")
            If onSharePortalIntegratedTemp Is Nothing OrElse onSharePortalIntegratedTemp = String.Empty Then
                Return False
            Else
                Return Boolean.Parse(onSharePortalIntegratedTemp)
            End If

        End Get
    End Property

    Private Shared _NumberFormatInfo As Globalization.NumberFormatInfo
    Public Shared ReadOnly Property NumberFormatInfo() As Globalization.NumberFormatInfo
        Get
            If _NumberFormatInfo Is Nothing Then
                '--
                _NumberFormatInfo = New Globalization.NumberFormatInfo()
                '--
                _NumberFormatInfo.CurrencyDecimalSeparator = ","
                _NumberFormatInfo.CurrencyGroupSeparator = "."
                _NumberFormatInfo.CurrencyDecimalDigits = 2
                _NumberFormatInfo.CurrencyPositivePattern = 2
                _NumberFormatInfo.CurrencySymbol = "€"
                '--
            End If
            Return _NumberFormatInfo
        End Get
    End Property

#End Region

#Region " Data Conversion Utility "

    Public Shared Function CutTime(d1 As Object, Optional d2 As Object = Nothing) As String

        If d2 Is Nothing OrElse Not d1 Is DBNull.Value Then

            If d1 Is DBNull.Value Then
                Return String.Empty
            Else
                Return CType(d1, DateTime).ToString("dd/MM/yyyy")
            End If

        Else

            If d2 Is DBNull.Value Then
                Return String.Empty
            Else
                Return CType(d2, DateTime).ToString("dd/MM/yyyy")
            End If

        End If

    End Function

#End Region

#Region " DataTable "

    Public Shared Function DataTableSelect(ByRef sourceDataTable As DataTable, filterExpression As String) As DataTable

        Dim drSelect As DataRow() = sourceDataTable.Select(filterExpression)
        Return DataRowToDataTable(drSelect, sourceDataTable.Clone())

    End Function

    Public Shared Function DataRowToDataTable(arrDataRow As DataRow(), dtTarget As DataTable) As DataTable

        If arrDataRow.Length > 0 Then

            If arrDataRow(0).ItemArray.Length <> dtTarget.Columns.Count Then
                Return dtTarget
            End If

            Dim dr, drCurrent As DataRow
            For Each drCurrent In arrDataRow
                dr = dtTarget.NewRow()
                dr.ItemArray = drCurrent.ItemArray
                dtTarget.Rows.Add(dr)
            Next

        End If

        dtTarget.AcceptChanges()

        Return dtTarget

    End Function

    Public Shared Sub addKey(ByRef dt As DataTable, ParamArray columns() As String)

        Dim keys(columns.Length - 1) As DataColumn

        For i As Int16 = 0 To columns.Length - 1
            keys(i) = dt.Columns(columns(i))
        Next

        dt.PrimaryKey = keys

    End Sub

#End Region

#Region " LeftBar "

    Public Shared Function CalcolaVacInadempienze(settings As Settings.Settings, isGestioneCentrale As Boolean) As Integer

        If IsPazIdEmpty() Then
            Return 0
        End If

        Dim numeroVacInadempienze As Integer

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizPaziente As New BizPaziente(genericProvider, settings, OnVacContext.CreateBizContextInfos(), Nothing)

                numeroVacInadempienze = bizPaziente.CountInadempienzePaziente(OnVacUtility.Variabili.PazId, isGestioneCentrale)

            End Using
        End Using

        Return numeroVacInadempienze

    End Function

    Public Shared Function CalcolaNumeroDocumenti(settings As Settings.Settings, isGestioneCentrale As Boolean) As Integer

        If IsPazIdEmpty() Then
            Return 0
        End If

        Dim numeroDocumenti As Integer

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizPaziente As New BizPaziente(genericProvider, settings, OnVacContext.CreateBizContextInfos(), Nothing)

                numeroDocumenti = bizPaziente.CountDocumentiPaziente(OnVacUtility.Variabili.PazId, isGestioneCentrale)

            End Using
        End Using

        Return numeroDocumenti

    End Function

    Public Shared Function CalcolaVacEscluse(settings As Settings.Settings, isGestioneCentrale As Boolean) As Integer

        If IsPazIdEmpty() Then
            Return 0
        End If

        Dim numeroVacEscluse As Integer

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizVaccinazioniEscluse As New BizVaccinazioniEscluse(genericProvider, settings, OnVacContext.CreateBizContextInfos(), Nothing)

                numeroVacEscluse = bizVaccinazioniEscluse.CountVaccinazioniEsclusePaziente(OnVacUtility.Variabili.PazId, isGestioneCentrale)

            End Using
        End Using

        Return numeroVacEscluse

    End Function

    Public Shared Function CalcolaVisite(settings As Settings.Settings, isGestioneCentrale As Boolean) As Integer

        If IsPazIdEmpty() Then
            Return 0
        End If

        Dim numeroVisite As Integer

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizPaziente As New BizPaziente(genericProvider, settings, OnVacContext.CreateBizContextInfos(), Nothing)

                numeroVisite = bizPaziente.CountVisitePaziente(OnVacUtility.Variabili.PazId, isGestioneCentrale)

            End Using
        End Using

        Return numeroVisite

    End Function

    Public Shared Function CalcolaConvocazioniAppuntamenti(settings As Settings.Settings, isGestioneCentrale As Boolean, queryString As Specialized.NameValueCollection) As String

        If IsPazIdEmpty() Then
            Return String.Empty
        End If

        Dim livelloUtenteCnv As Enumerators.LivelloUtenteConvocazione = GetLivelloUtenteConvocazione(queryString, False)

        Dim numeroConvocazioni As Integer = 0
        Dim numeroAppuntamenti As Integer = 0

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizCnv As New BizConvocazione(genericProvider, settings, OnVacContext.CreateBizContextInfos(), Nothing)

                numeroConvocazioni = bizCnv.CountConvocazioniSenzaAppuntamento(OnVacUtility.Variabili.PazId, OnVacUtility.Variabili.CNS.Codice, livelloUtenteCnv, isGestioneCentrale)

                numeroAppuntamenti = bizCnv.CountConvocazioniConAppuntamento(OnVacUtility.Variabili.PazId, OnVacUtility.Variabili.CNS.Codice, livelloUtenteCnv, isGestioneCentrale)

            End Using
        End Using

        Dim convApp As New Text.StringBuilder()

        If numeroConvocazioni > 0 Then
            convApp.AppendFormat("{0} CNV, ", numeroConvocazioni.ToString())
        End If

        If numeroAppuntamenti > 0 Then
            convApp.AppendFormat("{0} APP, ", numeroAppuntamenti.ToString())
        End If

        If convApp.Length > 0 Then

            convApp.Remove(convApp.Length - 2, 2)

            convApp.Insert(0, "(")
            convApp.Append(")")

        End If

        Return convApp.ToString()

    End Function

    Public Shared Function CalcolaReazioniAvverse(settings As Settings.Settings, isGestioneCentrale As Boolean) As Integer

        If IsPazIdEmpty() Then
            Return 0
        End If

        Dim numeroReazioniAvverse As Integer = 0

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizVaccEseguite As New BizVaccinazioniEseguite(genericProvider, settings, OnVacContext.CreateBizContextInfos())

                numeroReazioniAvverse = bizVaccEseguite.CountReazioniAvverse(OnVacUtility.Variabili.PazId, isGestioneCentrale)

            End Using
        End Using

        Return numeroReazioniAvverse

    End Function

    Public Shared Function CalcolaVacEseguite(settings As Settings.Settings, isGestioneCentrale As Boolean) As Integer

        If IsPazIdEmpty() Then
            Return 0
        End If

        Dim numeroVacEseguite As Integer = 0

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizVaccinazioniEseguite As New BizVaccinazioniEseguite(genericProvider, settings, OnVacContext.CreateBizContextInfos())

                numeroVacEseguite = bizVaccinazioniEseguite.CountVaccinazioniEseguite(OnVacUtility.Variabili.PazId, True, isGestioneCentrale)

            End Using
        End Using

        Return numeroVacEseguite

    End Function

    Public Shared Function CalcolaVacProgrammate(settings As Settings.Settings, isGestioneCentrale As Boolean) As Integer

        If IsPazIdEmpty() Then
            Return 0
        End If

        Dim numeroVaccinazioniProgrammate As Integer = 0

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizVaccinazioniProgrammate As New BizVaccinazioneProg(genericProvider, settings, OnVacContext.CreateBizContextInfos())

                numeroVaccinazioniProgrammate = bizVaccinazioniProgrammate.CountProgrammatePaziente(OnVacUtility.Variabili.PazId, isGestioneCentrale)

            End Using
        End Using

        Return numeroVaccinazioniProgrammate

    End Function

    Public Shared Function CalcolaNumeroBilanci(settings As Settings.Settings, isGestioneCentrale As Boolean) As Integer

        If IsPazIdEmpty() Then
            Return 0
        End If

        Dim numeroBilanci As Integer

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizBilanci As New BizBilancioProgrammato(genericProvider, settings, OnVacContext.CreateBizContextInfos(), Nothing)

                numeroBilanci = bizBilanci.CountBilanciPaziente(OnVacUtility.Variabili.PazId, isGestioneCentrale)

            End Using
        End Using

        Return numeroBilanci

    End Function

    Public Shared Function CalcolaNumeroRifiuti(settings As Settings.Settings, isGestioneCentrale As Boolean) As Integer

        If IsPazIdEmpty() Then
            Return 0
        End If

        Dim numeroRifiuti As Integer = 0

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizPaziente As New BizPaziente(genericProvider, settings, OnVacContext.CreateBizContextInfos(), Nothing)

                numeroRifiuti = bizPaziente.CountRifiutiPaziente(OnVacUtility.Variabili.PazId, isGestioneCentrale)

            End Using
        End Using

        Return numeroRifiuti

    End Function

    Public Shared Function CalcolaNumeroConsulenze(settings As Settings.Settings, isGestioneCentrale As Boolean) As Integer

        If IsPazIdEmpty() Then
            Return 0
        End If

        If isGestioneCentrale Then Return 0

        Dim numeroConsulenze As Integer = 0

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizInterventi As New BizInterventi(genericProvider, settings, OnVacContext.CreateBizContextInfos())

                numeroConsulenze = bizInterventi.CountInterventiPaziente(OnVacUtility.Variabili.PazId)

            End Using
        End Using

        Return numeroConsulenze

    End Function

    Public Shared Function CalcolaNumeroNotePaziente(settings As Settings.Settings, isGestioneCentrale As Boolean) As Integer

        If IsPazIdEmpty() Then
            Return 0
        End If

        If isGestioneCentrale Then Return 0

        Dim numeroNote As Integer = 0

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizPaziente As New BizPaziente(genericProvider, settings, OnVacContext.CreateBizContextInfos(), Nothing)

                numeroNote = bizPaziente.CountNotePaziente(OnVacUtility.Variabili.PazId, False)

            End Using
        End Using

        Return numeroNote

    End Function

    Public Shared Function GetInfoGestionePazientiLeftMenu(settings As Settings.Settings, isGestioneCentrale As Boolean) As String

        If IsPazIdEmpty() Then
            Return String.Empty
        End If

        Dim hasRitardi As Boolean = False
        'Dim hasNote As Boolean = False
        Dim countMantoux As Integer = 0

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizPaziente As New BizPaziente(genericProvider, settings, OnVacContext.CreateBizContextInfos(), Nothing)

                hasRitardi = bizPaziente.HasRitardi(OnVacUtility.Variabili.PazId, isGestioneCentrale)
                'hasNote = bizPaziente.HasNote(OnVacUtility.Variabili.PazId, False, isGestioneCentrale, False)
                countMantoux = bizPaziente.CountMantouxPaziente(OnVacUtility.Variabili.PazId, isGestioneCentrale)

            End Using
        End Using

        If Not hasRitardi AndAlso countMantoux = 0 Then 'If Not hasRitardi AndAlso Not hasNote AndAlso countMantoux = 0 Then
            Return String.Empty
        End If

        Dim info As New Text.StringBuilder()

        info.Append(" (")

        'If hasNote Then
        '    info.Append("Note, ")
        'End If

        If hasRitardi Then
            info.Append("Ritardo, ")
        End If

        If countMantoux > 0 Then
            info.AppendFormat("{0} Mantoux, ", countMantoux.ToString())
        End If

        info.RemoveLast(2)
        info.Append(")")

        Return info.ToString()

    End Function

    Public Shared Function CalcolaNumeroEpisodiCovid(settings As Settings.Settings) As Integer

        If IsPazIdEmpty() Then
            Return 0
        End If

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizPaziente As New BizPaziente(genericProvider, settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Return bizPaziente.CountEpisodiCovid(OnVacUtility.Variabili.PazId)

            End Using
        End Using

    End Function

    Public Shared Function CalcolaNumeroTamponiCovid(settings As Settings.Settings) As Integer

        If IsPazIdEmpty() Then
            Return 0
        End If

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizTamponi As New BizCovid19Tamponi(genericProvider, settings, OnVacContext.CreateBizContextInfos())
                If OnVacUtility.Variabili.PazId <= 0 Then
                    Return 0
                End If

                Return bizTamponi.ContaTamponiPaziente(OnVacUtility.Variabili.PazId)
            End Using
        End Using

    End Function

    Public Shared Function CalcolaNumeroTestSierologiciCovid(settings As Settings.Settings) As Integer

        If IsPazIdEmpty() Then
            Return 0
        End If

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizPaziente As New BizPaziente(genericProvider, settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Return bizPaziente.CountTestSierologiciCovid(OnVacUtility.Variabili.PazId)

            End Using
        End Using

    End Function

    Public Shared Function CalcolaNumeroRicoveriCovid(settings As Settings.Settings) As Integer

        If IsPazIdEmpty() Then
            Return 0
        End If

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizPaziente As New BizPaziente(genericProvider, settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Return bizPaziente.CountRicoveriCovid(OnVacUtility.Variabili.PazId)

            End Using
        End Using

    End Function

#End Region

#Region " Vaccinazioni Sostitute "

    'costruisce il datatable con le vaccinazioni sostitute associate [modifica 05/08/2005]
    Public Shared Sub RecuperaVaccinazioniSostitute()

        OnVacUtility.dt_VacSostitute = New DataTable()

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            OnVacUtility.dt_VacSostitute = genericProvider.AnaVaccinazioni.GetDataTableVaccinazioniSostitute()

        End Using

        If OnVacUtility.dt_VacSostitute.Rows.Count > 0 Then

            OnVacUtility.dt_VacSostitute.PrimaryKey = New DataColumn() {OnVacUtility.dt_VacSostitute.Columns("VAC_CODICE")}

        End If

    End Sub

    'controlla se esiste una vaccinazione sostituta per quella passata come parametro [modifica 05/08/2005]
    Public Shared Sub ControllaVaccinazioneSostituta(ByRef vaccinazione As String, Optional ByRef codVacPrec As String = Nothing, Optional ByRef descVacPrec As String = Nothing, Optional dipendenza As Enumerators.DipendenzaSostituta = Enumerators.DipendenzaSostituta.Destra)

        If OnVacUtility.dt_VacSostitute.Rows.Count = 0 Then Return

        'parametro dipendenza:
        '----> Destra: controlla se dalla vaccinazione è possibile risalire a quella che sostituisce
        '----> Sinistra: indica quale sostituta fa riferimento la sostituita
        Select Case dipendenza

            Case Enumerators.DipendenzaSostituta.Destra

                If (Not dt_VacSostitute.Rows.Find(vaccinazione).Item("VAC_COD_SOSTITUTA") Is Nothing) AndAlso (Not dt_VacSostitute.Rows.Find(vaccinazione).Item("VAC_COD_SOSTITUTA") Is DBNull.Value) Then
                    If Not codVacPrec Is Nothing Then codVacPrec = dt_VacSostitute.Rows.Find(vaccinazione).Item("VAC_CODICE")
                    If Not descVacPrec Is Nothing Then descVacPrec = dt_VacSostitute.Rows.Find(vaccinazione).Item("VAC_DESCRIZIONE")
                    vaccinazione = dt_VacSostitute.Rows.Find(vaccinazione).Item("VAC_COD_SOSTITUTA")
                End If

            Case Enumerators.DipendenzaSostituta.Sinistra

                If dt_VacSostitute.Select("vac_cod_sostituta = '" & vaccinazione & "'").Length > 0 Then
                    vaccinazione = dt_VacSostitute.Select("vac_cod_sostituta = '" & vaccinazione & "'")(0).Item("vac_codice")
                End If

        End Select

    End Sub

#End Region

#Region " PazId "

    Public Shared Function IsPazIdEmpty() As Boolean

        If String.IsNullOrEmpty(OnVacUtility.Variabili.PazId) Then Return True

        If OnVacUtility.Variabili.PazId < 0 Then Return True

        Return False

    End Function

    Public Shared Sub ClearPazId()

        OnVacUtility.Variabili.PazId = "-1"

    End Sub

#End Region

#Region " Paziente "

    Public Shared Sub ImpostaIntestazioniPagina(ByRef onitLayout As Onit.Controls.PagesLayout.OnitLayout3, ByRef labelLayoutTitolo As Label, genericProvider As DbGenericProvider, settings As Settings.Settings, isGestioneCentrale As Boolean)

        ' Aggiunta del consultorio di lavoro nel titolo della maschera
        OnVacUtility.ImpostaCnsLavoro(onitLayout)

        ' Impostazione dati del paziente nella label di intestazione della maschera
        If Not genericProvider Is Nothing Then

            labelLayoutTitolo.Text = OnVacUtility.GetDescrizionePaziente(genericProvider, settings, isGestioneCentrale)

        Else

            Using _genericProvider As New DbGenericProvider(OnVacContext.Connection)

                labelLayoutTitolo.Text = OnVacUtility.GetDescrizionePaziente(_genericProvider, settings, isGestioneCentrale)

            End Using

        End If

    End Sub

    ' Restituisce la stringa da impostare nell'intestazione delle maschere relative al paziente
    Private Shared Function GetDescrizionePaziente(genericProvider As DbGenericProvider, settings As Settings.Settings, isGestioneCentrale As Boolean) As String

        Dim datiAnagraficiPaziente As PazienteDatiAnagrafici = Nothing

        Using bizPaziente As New BizPaziente(genericProvider, settings, OnVacContext.CreateBizContextInfos(), Nothing)

            datiAnagraficiPaziente = bizPaziente.GetDatiAnagraficiPaziente(OnVacUtility.Variabili.PazId, isGestioneCentrale)

        End Using

        If datiAnagraficiPaziente Is Nothing Then Return String.Empty

        Return String.Format("{0} {1} [nat{2} il {3:dd/MM/yyyy} - età {4}]",
                             datiAnagraficiPaziente.Cognome,
                             datiAnagraficiPaziente.Nome,
                             IIf(datiAnagraficiPaziente.Sesso = "F", "a", "o"),
                             datiAnagraficiPaziente.DataNascita,
                             OnVacUtility.ImpostaEtaPazienteConv(datiAnagraficiPaziente.DataNascita, Date.Now))

    End Function

    Public Shared Function GetValoreCampiPaziente(settings As Settings.Settings, isGestioneCentrale As Boolean, ParamArray pazFields() As String) As Hashtable

        Dim ht As Hashtable

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizPaziente As New BizPaziente(genericProvider, settings, OnVacContext.CreateBizContextInfos(), Nothing)

                ht = bizPaziente.GetValoreCampiPaziente(OnVacUtility.Variabili.PazId, isGestioneCentrale, pazFields)

            End Using
        End Using

        Return ht

    End Function

    'calcolo dell'età del paziente alla data di convocazione (modifica 13/01/2005)
    Public Shared Function ImpostaEtaPazienteConv(dataNascita As Date, dataConvocazione As Date) As String

        '---------------------------
        'calcolo differenza di date
        '---------------------------

        'nel caso in cui si sposti la data di nascita prima di quella di convocazione (modifica 25/01/2004)
        If dataNascita = dataConvocazione Then Return "0 giorni"
        If dataNascita > dataConvocazione Then Return "---"

        Dim eta As Eta = PazienteHelper.CalcoloEtaDaData(dataNascita, dataConvocazione)

        ' Costruzione condizionale della stringa di output
        Dim result As New System.Text.StringBuilder

        If eta.Anni <> 0 Then
            result.AppendFormat("{0} ann{1}", eta.Anni, IIf(eta.Anni = 1, "o", "i"))
        End If

        If eta.Mesi <> 0 Then
            If eta.Anni <> 0 Then result.Append(", ")
            result.AppendFormat("{0} mes{1}", eta.Mesi, IIf(eta.Mesi = 1, "e", "i"))
        End If

        If eta.Giorni <> 0 Then
            If eta.Anni <> 0 OrElse eta.Mesi <> 0 Then result.Append(", ")
            result.AppendFormat("{0} giorn{1}", eta.Giorni, IIf(eta.Giorni = 1, "o", "i"))
        End If

        Return result.ToString()

    End Function

    'calcolo dell'età del paziente alla data di convocazione (modifica 13/01/2005)
    Public Shared Function ImpostaEtaPazienteConv(dataConvocazione As Date, isGestioneCentrale As Boolean, settings As Settings.Settings) As String

        Dim dataNascita As Date

        If Not OnVacUtility.IsPazIdEmpty() Then

            Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                Using bizPaziente As New BizPaziente(genericProvider, settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    dataNascita = bizPaziente.GetDataNascitaPaziente(OnVacUtility.Variabili.PazId, isGestioneCentrale)

                End Using
            End Using

        Else

            dataNascita = dataConvocazione

        End If

        Return ImpostaEtaPazienteConv(dataNascita, dataConvocazione)

    End Function

    'ottiene il codice dell'indirizzo come successivo al massimo di quelli già presenti [modifica 01/09/2005]
    Public Shared Function RecuperaCodMaxIndirizzo() As Integer

        Using dam As IDAM = OnVacUtility.OpenDam()

            With dam.QB
                .NewQuery()
                .AddSelectFields("max(ind_codice)")
                .AddTables("t_paz_indirizzi")
            End With

            Dim codMaxIndirizzo As Object = dam.ExecScalar()

            If Not codMaxIndirizzo Is Nothing AndAlso Not codMaxIndirizzo Is DBNull.Value Then
                Return Convert.ToInt32(codMaxIndirizzo) + 1
            End If

        End Using

        Return 1

    End Function

#End Region

#Region " Inadempienze/Esclusioni "

    ' Crea l'inadempienza per la vaccinazione, se è obbligatoria e se il motivo di esclusione lo prevede.
    ' Se il parametro logOperazioneAutomatica è nothing, non effettua nessun log.
    ' Se il parametro logOperazioneAutomatica vale true/false, effettua il log indicando l'operazione come "automatica"/"manuale".
    Public Shared Function CreaInadempienza(codiceMotivoEsclusione As String, codiceVaccinazione As String, codicePaziente As Integer, dataInadempienza As Date, operatore As String, dam As IDAM, logOperazioneAutomatica As Boolean?) As Integer

        Dim numRecordInseriti As Integer = 0

        Dim necessarioCreareInadempienza As Boolean = False

        Using genericProvider As New DbGenericProvider(dam)

            ' Se la vaccinazione è obbligatoria, controllo se il motivo di esclusione è tra quelli che generano inadempienza
            If genericProvider.AnaVaccinazioni.IsVaccinazioneObbligatoria(codiceVaccinazione) Then

                necessarioCreareInadempienza = genericProvider.MotiviEsclusione.MotivoEsclusioneGeneraInadempienza(codiceMotivoEsclusione)

            End If

        End Using

        If necessarioCreareInadempienza Then

            ' Creazione inadempienza
            Dim comunicazioneSindaco As Int16 = Enumerators.StatoInadempienza.ComunicazioneAlSindaco

            With dam.QB
                .NewQuery()
                .AddTables("T_PAZ_INADEMPIENZE")
                .AddInsertField("PIN_PAZ_CODICE", codicePaziente, DataTypes.Numero)
                .AddInsertField("PIN_VAC_CODICE", codiceVaccinazione, DataTypes.Stringa)
                .AddInsertField("PIN_STATO", comunicazioneSindaco, DataTypes.Stringa)
                .AddInsertField("PIN_DATA", dataInadempienza, DataTypes.Data)
                .AddInsertField("PIN_UTE_ID", operatore, DataTypes.Stringa)
            End With

            numRecordInseriti = dam.ExecNonQuery(ExecQueryType.Insert)

            ' Log
            If logOperazioneAutomatica.HasValue Then

                Dim testataLog As New Testata(Log.DataLogStructure.TipiArgomento.INADEMPIENZE, Operazione.Inserimento, logOperazioneAutomatica.Value)

                Dim recordLog As New Record()

                recordLog.Campi.Add(New Campo("PIN_VAC_CODICE", String.Empty, codiceVaccinazione))
                recordLog.Campi.Add(New Campo("PIN_STATO", String.Empty, comunicazioneSindaco.ToString()))
                recordLog.Campi.Add(New Campo("PIN_DATA", String.Empty, dataInadempienza))

                testataLog.Records.Add(recordLog)

                LogBox.WriteData(testataLog)

            End If

        End If

        Return numRecordInseriti

    End Function

    ' Eliminazione inadempienza
    ' Se il parametro logOperazioneAutomatica è nothing, non effettua nessun log.
    ' Se il parametro logOperazioneAutomatica vale true/false, effettua il log indicando l'operazione come "automatica"/"manuale".
    Public Shared Function EliminaInadempienza(codiceVaccinazione As String, codicePaziente As Integer, dam As IDAM, logOperazioneAutomatica As Boolean?) As Integer

        With dam.QB
            .NewQuery()
            .AddTables("T_PAZ_INADEMPIENZE")
            .AddWhereCondition("PIN_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
            .AddWhereCondition("PIN_VAC_CODICE", Comparatori.Uguale, codiceVaccinazione, DataTypes.Stringa)
        End With

        Dim numRecordEliminati As Integer = dam.ExecNonQuery(ExecQueryType.Delete)

        If logOperazioneAutomatica.HasValue And numRecordEliminati > 0 Then

            Dim testataLog As New Testata(Log.DataLogStructure.TipiArgomento.INADEMPIENZE, Operazione.Eliminazione, logOperazioneAutomatica.Value)

            Dim recordLog As New Record()

            recordLog.Campi.Add(New Campo("PIN_VAC_CODICE", codiceVaccinazione, "Eliminato"))

            testataLog.Records.Add(recordLog)

            LogBox.WriteData(testataLog)

        End If

        Return numRecordEliminati

    End Function

    'Public Shared Function IsPazienteTotalmenteInadempiente(dam As IDAM, codicePaziente As String) As Boolean

    '    With dam.QB

    '        .NewQuery()
    '        .AddTables("T_PAZ_INADEMPIENZE")
    '        .AddSelectFields("VAC_CODICE")
    '        .AddWhereCondition("PIN_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Stringa)
    '        .AddWhereCondition("VAC_CODICE", Comparatori.Uguale, "PIN_VAC_CODICE", DataTypes.Replace)

    '        Dim q1 As String = .GetSelect

    '        .NewQuery(False, False)
    '        .AddTables("T_ANA_VACCINAZIONI")
    '        .AddSelectFields("1")
    '        .AddWhereCondition("VAC_OBBLIGATORIA", Comparatori.Uguale, Constants.ObbligatorietaVaccinazione.Obbligatoria, DataTypes.Stringa)
    '        .AddWhereCondition("VAC_COD_SOSTITUTA", Comparatori.Is, "NULL", DataTypes.Replace)
    '        .AddWhereCondition("", Comparatori.NotExist, "(" + q1 + ")", DataTypes.Replace)

    '    End With

    '    Return dam.ExecScalar() Is Nothing

    'End Function

    'nel caso in cui il paziente sia totalmente inadempiente (ovvero inadempiente su tutte le vaccinazioni obbligatorie)
    'mette INADEMPIENTE nello stato vaccinale, in caso contrario lo mette in corso se era inadempiente
    'Sara: ho aggiunto un parametro opzionale che indica se diverso da "" che si vuole loggare e al tempo 
    'stesso indica se e' una operazione automatica o no: la stringa deve valere: "automatica" oppure "manuale" 
    Public Shared Sub ControllaSeTotalmenteInadempiente(codicePaziente As String, dam As IDAM, settings As Settings.Settings)

        Using genericprovider As New DbGenericProvider(dam)

            Dim isTotalmenteInadempiente As Boolean = False

            Dim bizContextInfos As BizContextInfos = OnVacContext.CreateBizContextInfos()

            Using bizPaziente As New BizPaziente(genericprovider, settings, bizContextInfos, Nothing)

                isTotalmenteInadempiente = bizPaziente.IsTotalmenteInadempiente(Convert.ToInt64(codicePaziente))

            End Using

            If isTotalmenteInadempiente Then

                ' Modifica stato vaccinale per inadempienza totale
                Using bizPaziente As New BizPaziente(genericprovider, settings, bizContextInfos, Nothing)

                    bizPaziente.UpdateStatoVaccinalePaziente(Convert.ToInt32(codicePaziente), Enumerators.StatiVaccinali.InadempienteTotale)

                End Using

                'esclude le vaccinazioni non obbligatorie se il paz e' totalmente inademiente e a seconda di un parametro
                If settings.ESCLUDINONOBBLSETI Then

                    If Not String.IsNullOrEmpty(settings.APP_ID_CENTRALE) AndAlso settings.APP_ID_CENTRALE <> OnVacContext.AppId Then
                        Throw New NotImplementedException("settings.CODESCLNONOBBLSETI non supportato se settings.APP_ID_CENTRALE valorizzato")
                    End If

                    With dam.QB

                        .NewQuery()
                        .AddTables("T_VAC_ESCLUSE")
                        .AddSelectFields("1")
                        .AddWhereCondition("VEX_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Stringa)
                        .AddWhereCondition("VEX_VAC_CODICE", Comparatori.Uguale, "SAS_VAC_CODICE", DataTypes.Replace)
                        .OpenParanthesis()
                        .AddWhereCondition("VEX_DATA_SCADENZA", Comparatori.Is, "Null", DataTypes.Replace)
                        .AddWhereCondition("VEX_DATA_SCADENZA", Comparatori.Maggiore, DateTime.Today, DataTypes.Data, "or")
                        .CloseParanthesis()

                        Dim q1 As String = .GetSelect()

                        .NewQuery(False, False)
                        .AddSelectFields("distinct SAS_VAC_CODICE")
                        .AddTables("T_ANA_VACCINAZIONI,T_PAZ_CICLI,T_ANA_TEMPI_SEDUTE,V_ANA_ASS_VACC_SEDUTE")
                        .AddWhereCondition("PAC_CIC_CODICE", Comparatori.Uguale, "TSD_CIC_CODICE", DataTypes.Join)
                        .AddWhereCondition("PAC_CIC_CODICE", Comparatori.Uguale, "SAS_CIC_CODICE", DataTypes.Join)
                        .AddWhereCondition("TSD_N_SEDUTA", Comparatori.Uguale, "SAS_N_SEDUTA", DataTypes.Join)
                        .AddWhereCondition("SAS_VAC_CODICE", Comparatori.Uguale, "VAC_CODICE", DataTypes.Join)
                        .AddWhereCondition("VAC_OBBLIGATORIA", Comparatori.Diverso, "A", DataTypes.Stringa)
                        .AddWhereCondition("PAC_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Stringa)
                        .AddWhereCondition("", Comparatori.NotExist, "(" + q1 + ")", DataTypes.Replace)

                    End With

                    Dim dt As New DataTable()
                    dam.BuildDataTable(dt)

                    ' --- Inserimento Esclusione --- '
                    ' L'esclusione viene creata in data odierna, senza scadenza e con il motivo specificato nel parametro
                    Dim dataVisita As Date = Date.Now.Date
                    Dim codiceVaccinazione As String = String.Empty

                    For h As Integer = 0 To dt.Rows.Count - 1

                        codiceVaccinazione = dt.Rows(h)(0)

                        Using bizVaccinazioniEscluse As New BizVaccinazioniEscluse(genericprovider, settings, bizContextInfos, Nothing)

                            Dim vaccinazioneEsclusa As New VaccinazioneEsclusa()

                            vaccinazioneEsclusa.CodicePaziente = codicePaziente
                            vaccinazioneEsclusa.CodiceVaccinazione = codiceVaccinazione
                            vaccinazioneEsclusa.DataVisita = dataVisita
                            vaccinazioneEsclusa.CodiceMotivoEsclusione = settings.CODESCLNONOBBLSETI

                            Dim vaccinazioneEsclusaEsistenteEliminata As VaccinazioneEsclusa =
                                bizVaccinazioniEscluse.AggiornaVaccinazioneEsclusa(vaccinazioneEsclusa, True, "Controllo Inadempienza Totale")


                            ' TODO [ESCLUSE]:   la vaccinazione esclusa e l'esclusa eliminata dovrebbero essere aggiunte alle rispettive liste.
                            '                   dove viene richiamato questo metodo, deve sempre essere richiamato anche il frullo per aggiornare le escluse, 
                            '                   ma non succede o, se succede, non sono aggiornate le liste di cui sopra.
                            '                   Bisogna portare tutto in un unico metodo del bizEscluse!!!

                        End Using

                    Next

                End If

            Else

                ' Modifica stato vaccinale da inadempiente totale a in corso.
                Using bizPaziente As New BizPaziente(genericprovider, settings, bizContextInfos, Nothing)

                    bizPaziente.UpdateStatoVaccinalePaziente(Convert.ToInt32(codicePaziente), Enumerators.StatiVaccinali.InadempienteTotale, Enumerators.StatiVaccinali.InCorso)

                End Using

                If settings.ESCLUDINONOBBLSETI Then

                    ' TODO [ESCLUSE]: se paz totalmente inadempiente e parametro ESCLUDINONOBBLSETI true => fa la delete secca delle escluse, 
                    '                 senza spostarle nella _ELIMINATE e senza gestire centrale e altre ULSS!

                    With dam.QB
                        .NewQuery()
                        .AddTables("T_VAC_ESCLUSE")
                        .AddWhereCondition("VEX_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                        .AddWhereCondition("VEX_MOE_CODICE", Comparatori.Uguale, settings.CODESCLNONOBBLSETI, DataTypes.Stringa)
                    End With

                    dam.ExecNonQuery(ExecQueryType.Delete)

                End If

            End If
        End Using

    End Sub

#End Region

#Region " Luoghi Esecuzione Vaccinazioni "

    ''' <summary>
    ''' OBSOLETA: Vecchia logica, utilizza un parametro, ora viene usata la tabella T_ANA_LUOGHI_ESECUZIONE_VAC
    ''' </summary>
    ''' <param name="settings"></param>
    ''' <returns></returns>
    Public Shared Function GetDescrizioneLuogo(codiceLuogo As String, ByRef settings As Settings.Settings) As String

        Dim collectionLuoghi As Collection.LuoghiEsecuzioneVacCollection = OnVacUtility.GetLuoghiEsecuzioneVaccinazioni(settings)

        Dim luogo As LuogoEsecuzioneVac = collectionLuoghi.Find(codiceLuogo)

        If luogo Is Nothing Then Return String.Empty

        Return luogo.DescrizioneLuogo

    End Function

    ''' <summary>
    ''' OBSOLETA: Vecchia logica, utilizza un parametro, ora viene usata la tabella T_ANA_LUOGHI_ESECUZIONE_VAC
    ''' </summary>
    ''' <param name="settings"></param>
    ''' <returns></returns>
    Public Shared Function GetLuoghiEsecuzioneVaccinazioni(ByRef settings As Settings.Settings) As Collection.LuoghiEsecuzioneVacCollection

        If settings Is Nothing Then
            Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                settings = New Settings.Settings(OnVacUtility.Variabili.CNS.Codice, genericProvider)
            End Using
        End If

        Dim luoghi As String() = settings.LUOGHI.Split(";")

        Dim collectionLuoghi As New Collection.LuoghiEsecuzioneVacCollection()

        Dim arr As String()
        For i As Integer = 0 To luoghi.Length - 1
            arr = luoghi(i).Split("|")
            collectionLuoghi.Add(New Entities.LuogoEsecuzioneVac(arr(0), arr(1)))
        Next

        Return collectionLuoghi

    End Function

#End Region

#Region " DataBase Utility "

#Region " Dam "

    ''' <summary>
    ''' Restituisce il dam che utilizza la connessione in session
    ''' </summary>
    Public Shared Function OpenDam() As IDAM

        Dim cninfo As ConnectionInfo = OnVacContext.Connection

        Return DAMBuilder.CreateDAM(cninfo.Provider, cninfo.ConnectionString)

    End Function

    ''' <summary>
    ''' Restituisce il dam che utilizza la connessione relativa ad applicativo e azienda specificati.
    ''' La connessione utilizzata dal dam viene recuperata da onit_manager, in base ai due parametri.
    ''' </summary>
    Public Shared Function OpenDam(appId As String, codiceAzienda As String) As IDAM

        Dim app As Onit.Shared.Manager.Apps.App = Onit.Shared.Manager.Apps.App.getInstance(appId, codiceAzienda)

        If app Is Nothing Then
            Throw New OnVacConfigurationException(String.Format("Errore di configurazione. AppId specificato ({0}) non valido o azienda ({1}) non valida.", appId, codiceAzienda))
        End If

        Dim cninfo As Onit.Shared.NTier.Dal.DAAB.DbConnectionInfo = app.getConnectionInfo()

        ' In caso di AppId errato, viene sollevata un'eccezione di configurazione.
        Return DAMBuilder.CreateDAM(cninfo.Provider.ToString(), cninfo.ConnectionString)

    End Function

    ''' <summary>
    ''' Restituisce il dam per accedere ad OnitManager
    ''' </summary>
    Public Shared Function OpenDamToOnitManager() As IDAM

        Dim crypto As New Crypto(Providers.Rijndael)

        Return DAMBuilder.CreateDAM(ConfigurationManager.AppSettings.Get("Provider"), crypto.Decrypt(ConfigurationManager.AppSettings.Get("ManagerConnectString")))

    End Function

    ''' <summary>
    ''' Chiusura connessione e dispose del dam
    ''' </summary>
    Public Shared Sub CloseDam(ByRef DAM As IDAM)

        ' N.B.: DAM.Dispose(True) e DAM.Dispose() sono uguali, eseguono le stesse operazioni:

        ' 1.  DAM.Command.Dispose()
        ' 2.  if (DAM.Connection.State != ConnectionState.Closed) DAM.Connection.Close()
        ' 3.  DAM.Connection.Dispose()

        If Not DAM Is Nothing Then DAM.Dispose(True)

    End Sub

#End Region

#End Region

#Region " Consultorio "

    ' Aggiunta del consultorio di lavoro al titolo della maschera (se non è già presente)
    Public Shared Sub ImpostaCnsLavoro(ByRef onitLayout As Onit.Controls.PagesLayout.OnitLayout3)

        If Not onitLayout.Titolo.ToLower().Contains("- centro vaccinale corrente:") Then

            onitLayout.Titolo += String.Format(" - Centro Vaccinale corrente: <b>{0} ({1})</b>", Variabili.CNS.Descrizione, Variabili.CNS.Codice)

        End If

    End Sub

    ''' <summary>
    ''' Caricamento variabili relative al consultorio corrente
    ''' </summary>
    ''' <param name="codiceConsultorio"></param>
    ''' <param name="azzeraVariabiliDiLavoro"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CaricaVariabiliConsultorio(codiceConsultorio As String, azzeraVariabiliDiLavoro As Boolean) As Boolean

        Dim controlloOk As Boolean = False

        'Carica le variabili per la corrente sessione di onvac
        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            ' --- Gestione postazioni --- '

            ' Imposta il consultorio di lavoro
            OnVacUtility.SetConsultorioCorrente(codiceConsultorio, genericProvider)

            ' Codice del consultorio associato alla postazione (o di default)
            Dim codiceCns As String = String.Empty
            If Not OnVacUtility.Variabili.CNS Is Nothing AndAlso OnVacUtility.Variabili.CNS.Codice <> String.Empty Then
                codiceCns = OnVacUtility.Variabili.CNS.Codice
            End If

            ' --- '
            ' Se la chiave AppTest è true, imposto postazioni a N per forzare l'accesso all'applicativo
            Dim test As Boolean = False

            If Not ConfigurationManager.AppSettings("AppTest") Is Nothing Then
                Try
                    test = Boolean.Parse(ConfigurationManager.AppSettings.Item("AppTest"))
                Catch ex As Exception
                    test = False
                End Try
            End If
            ' --- '

            ' Caricamento parametri. L'oggetto settings viene sovrascritto.
            Dim settings As New Settings.Settings(codiceCns, genericProvider)

            Dim postazioni As Boolean = settings.POSTAZIONI
            If test Then postazioni = False

            If postazioni Then
                ' Se il parametro vale S, deve controllare se c'è un consultorio associato alla postazione, 
                ' altrimenti il controllo restituisce esito false
                controlloOk = (codiceCns <> String.Empty)
            Else
                ' Se il parametro vale N, l'esito del controllo è sempre true. 
                ' In più, se il consultorio non è associato alla postazione, imposta quello di default (definito in un parametro).
                controlloOk = True
                If codiceCns = String.Empty Then
                    OnVacUtility.Variabili.CNS.Codice = settings.CNS_DEFAULT
                    OnVacUtility.Variabili.CNS.Descrizione = String.Format("CENTRO VACCINALE DEFAULT [{0}]", OnVacUtility.Variabili.CNS.Codice)
                End If
            End If

            ' Vado avanti solo se il controllo ha dato esito positivo
            If controlloOk Then

                ' --- Magazzino del consultorio corrente --- '
                OnVacUtility.Variabili.CNSMagazzino = genericProvider.Consultori.GetConsultorioMagazzino(OnVacUtility.Variabili.CNS.Codice)

                ' --- Azzeramento variabili di lavoro --- '
                If azzeraVariabiliDiLavoro Then
                    OnVacUtility.Variabili.DataEsecuzione = DateTime.Now
                    OnVacUtility.Variabili.MedicoResponsabile = New VariabiliOnVac.Medico()
                    OnVacUtility.Variabili.MedicoVaccinante = New VariabiliOnVac.Medico()
                    OnVacUtility.Variabili.MedicoRilevatore = New VariabiliOnVac.Medico()
                End If

            End If

        End Using

        Return controlloOk

    End Function

    ' Imposta il consultorio associato alla postazione oppure quello di default tra i consultori abilitati per l'utente corrente.
    ' Utilizza l'OnVac.Log
    Private Shared Sub SetConsultorioCorrente(codiceConsultorio As String, genericProvider As DbGenericProvider)

        Dim consultorio As Consultorio = Nothing
        Dim pc As Web.UI.CurrentMachineInfo = OnVacContext.CurrentMachine

        Using bizConsultori As New BizConsultori(genericProvider, Nothing, OnVacContext.CreateBizContextInfos(), Nothing)

            Dim command As New BizConsultori.ConsultorioLoginCommand()
            command.CodiceConsultorio = codiceConsultorio
            command.ComputerName = pc.ComputerName
            command.IdUtente = OnVacContext.UserId
            command.MachineGroupID = pc.MachineGroupID
            command.MachineID = pc.MachineID
            command.RemoteIP = pc.RemoteIP

            consultorio = bizConsultori.GetConsultorioLogin(command)

        End Using

        If consultorio Is Nothing Then
            consultorio = New Consultorio()
            consultorio.Codice = String.Empty
            consultorio.Descrizione = String.Empty
        End If

        OnVacUtility.Variabili.CNS = consultorio

        ' Ogni volta che si imposta il consultorio corrente, viene resettato l'ambulatorio di convocazione (che verrà impostato nella maschera delle programmate)
        OnVacUtility.Variabili.AMBConvocazione = Nothing

    End Sub

#End Region

#Region " Report "

    Public Shared Sub StampaNonPresente(page As System.Web.UI.Page, nome As String)

        Dim sb As New System.Text.StringBuilder()
        sb.Append("<script type='text/javascript' language='javascript'>")
        sb.AppendFormat("alert('Stampe non presenti per questa installazione:\n{0}\n');", nome.Replace("'", "\'"))
        sb.Append("</script>")

        page.ClientScript.RegisterStartupScript(page.GetType(), "messaggio", sb.ToString())

    End Sub

    ''' <summary>
    ''' Metodo di stampa del certificato vaccinale per il paziente corrente, con visualizzazione del lotto e del nome commerciale
    ''' </summary>
    ''' <param name="currentPage"></param>
    ''' <param name="settings"></param>
    ''' <remarks></remarks>
    Public Shared Sub StampaCertificatoVaccinaleLotti(currentPage As Page, settings As Settings.Settings)

        StampaCertificatoVaccinale(currentPage, currentPage.Request.RawUrl, settings, "{T_PAZ_PAZIENTI.PAZ_CODICE}=" + OnVacUtility.Variabili.PazId.ToString(), False, True, False)

    End Sub

    ''' <summary>
    ''' Metodo di stampa del certificato vaccinale per il paziente corrente
    ''' </summary>
    ''' <param name="currentPage"></param>
    ''' <param name="settings"></param>
    ''' <param name="stampaNotaValidita"></param>
    ''' <param name="stampaScrittaCertificato"></param>
    ''' <remarks></remarks>
    Public Shared Sub StampaCertificatoVaccinale(currentPage As Page, settings As Settings.Settings, stampaNotaValidita As Boolean, stampaScrittaCertificato As Boolean)

        StampaCertificatoVaccinale(currentPage, currentPage.Request.RawUrl, settings, "{T_PAZ_PAZIENTI.PAZ_CODICE}=" + OnVacUtility.Variabili.PazId.ToString(), stampaNotaValidita, False, stampaScrittaCertificato)

    End Sub

    ''' <summary>
    ''' Metodo di stampa dei certificati vaccinali in base al filtro specificato
    ''' </summary>
    ''' <param name="currentPage"></param>
    ''' <param name="url"></param>
    ''' <param name="settings"></param>
    ''' <param name="reportFilter"></param>
    ''' <param name="stampaNotaValidita"></param>
    ''' <remarks></remarks>
    Public Shared Sub StampaCertificatoVaccinale(currentPage As Page, url As String, settings As Settings.Settings, reportFilter As String, stampaNotaValidita As Boolean)

        StampaCertificatoVaccinale(currentPage, url, settings, reportFilter, stampaNotaValidita, False, False)

    End Sub

    Public Shared Sub StampaCertificatoVaccinaleGiornaliero(currentPage As Page, url As String, settings As Settings.Settings, reportFilter As String, stampaNotaValidita As Boolean, data As String)

        StampaCertificatoVaccinaleGiornaliero(currentPage, url, settings, reportFilter, stampaNotaValidita, False, False, data)

    End Sub

    ''' <summary>
    ''' Stampa del certificato vaccinale del paziente, filtrato per le sole vaccinazioni eseguite specificate nella lista
    ''' </summary>
    ''' <param name="currentPage"></param>
    ''' <param name="settings"></param>
    ''' <param name="stampaNotaValidita"></param>
    ''' <param name="stampaScrittaCertificato"></param>
    ''' <param name="listaCodiciVaccinazioni"></param>
    Public Shared Sub StampaCertificatoVaccinalePazienteSoloVaccinazioniSpecificate(currentPage As Page, settings As Settings.Settings, stampaNotaValidita As Boolean, stampaScrittaCertificato As Boolean, listaCodiciVaccinazioni As List(Of String))

        Dim filtro As New Text.StringBuilder()

        filtro.AppendFormat("{{T_PAZ_PAZIENTI.PAZ_CODICE}} = {0}", OnVacUtility.Variabili.PazId.ToString())

        If Not listaCodiciVaccinazioni.IsNullOrEmpty() Then

            filtro.AppendFormat(" AND {{T_VAC_ESEGUITE.VES_VAC_CODICE}} IN ['{0}']", String.Join("','", listaCodiciVaccinazioni.ToArray()))

        End If

        StampaCertificatoVaccinale(currentPage, currentPage.Request.RawUrl, settings, filtro.ToString(), stampaNotaValidita, False, stampaScrittaCertificato)

    End Sub

    Private Shared Sub StampaCertificatoVaccinale(currentPage As Page, url As String, settings As Settings.Settings, reportFilter As String, stampaNotaValidita As Boolean, stampaLottoNomeCommerciale As Boolean, stampaScrittaCertificato As Boolean)

        Dim consultorio As Cns = Nothing
        Dim reportFolder As String = String.Empty

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            Using bizConsultori As New BizConsultori(genericProvider, settings, OnVacContext.CreateBizContextInfos(), Nothing)

                ' Lettura dati consultorio da stampare nel report
                consultorio = bizConsultori.GetConsultorio(OnVacUtility.Variabili.CNS.Codice)

            End Using

            ' Lettura directory in cui è definito lo schema del report
            Using bizReport As New BizReport(genericProvider, settings, OnVacContext.CreateBizContextInfos())

                reportFolder = bizReport.GetReportFolder(Constants.ReportName.CertificatoVaccinale)

            End Using

        End Using

        Dim rpt As ReportParameter =
            OnVacUtility.GetReportParameterCertificatoVaccinale(consultorio, settings, stampaNotaValidita, stampaLottoNomeCommerciale, stampaScrittaCertificato)

        ' Stampa
        If Not OnVacReport.StampaReport(url, Constants.ReportName.CertificatoVaccinale, reportFilter, rpt, Nothing, Nothing, reportFolder) Then
            OnVacUtility.StampaNonPresente(currentPage, Constants.ReportName.CertificatoVaccinale)
        End If

    End Sub

    Private Shared Sub StampaCertificatoVaccinaleGiornaliero(currentPage As Page, url As String, settings As Settings.Settings, reportFilter As String, stampaNotaValidita As Boolean, stampaLottoNomeCommerciale As Boolean, stampaScrittaCertificato As Boolean, data As String)

        Dim consultorio As Cns = Nothing
        Dim reportFolder As String = String.Empty

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            Using bizConsultori As New BizConsultori(genericProvider, settings, OnVacContext.CreateBizContextInfos(), Nothing)

                ' Lettura dati consultorio da stampare nel report
                consultorio = bizConsultori.GetConsultorio(OnVacUtility.Variabili.CNS.Codice)

            End Using

            ' Lettura directory in cui è definito lo schema del report
            Using bizReport As New BizReport(genericProvider, settings, OnVacContext.CreateBizContextInfos())

                reportFolder = bizReport.GetReportFolder(Constants.ReportName.CertificatoVaccinaleGiornaliero)

            End Using

        End Using

        Dim rpt As ReportParameter = OnVacUtility.GetReportParameterCertificatoVaccinale(consultorio, settings, stampaNotaValidita, stampaLottoNomeCommerciale, stampaScrittaCertificato)

        rpt.AddParameter("dataSel", data)

        ' Stampa
        If Not OnVacReport.StampaReport(url, Constants.ReportName.CertificatoVaccinaleGiornaliero, reportFilter, rpt, Nothing, Nothing, reportFolder) Then
            OnVacUtility.StampaNonPresente(currentPage, Constants.ReportName.CertificatoVaccinaleGiornaliero)
        End If

    End Sub

    ''' <summary>
    ''' Restituisce un oggetto di tipo ReportParameter con i parametri utilizzati da CertificatoVaccinale.rpt
    ''' </summary>
    ''' <param name="consultorio"></param>
    ''' <param name="settings"></param>
    ''' <param name="stampaNotaValidita"></param>
    ''' <param name="stampaLottoNomeCommerciale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetReportParameterCertificatoVaccinale(consultorio As Cns, settings As Settings.Settings, stampaNotaValidita As Boolean, stampaLottoNomeCommerciale As Boolean, stampaScrittaCertificato As Boolean) As ReportParameter

        Dim rpt As New ReportParameter()

        If consultorio Is Nothing Then
            rpt.AddParameter("DescrizioneComune", String.Empty)
            rpt.AddParameter("cnsStampaIndirizzo", String.Empty)
            rpt.AddParameter("cnsStampaCap", String.Empty)
            rpt.AddParameter("cnsStampaComune", String.Empty)
            rpt.AddParameter("cnsStampaTelefono", String.Empty)
        Else
            rpt.AddParameter("DescrizioneComune", consultorio.Comune)
            rpt.AddParameter("cnsStampaIndirizzo", consultorio.Indirizzo)
            rpt.AddParameter("cnsStampaCap", consultorio.Cap)
            rpt.AddParameter("cnsStampaComune", consultorio.Comune)
            rpt.AddParameter("cnsStampaTelefono", consultorio.Telefono)
        End If

        rpt.AddParameter("cnsStampaCodice", OnVacUtility.Variabili.CNS.Codice)

        If stampaNotaValidita Then
            rpt.AddParameter("notaValidita", settings.CERTIFICATO_VACCINALE_NOTA_VALIDITA)
        Else
            rpt.AddParameter("notaValidita", String.Empty)
        End If

        If stampaLottoNomeCommerciale Then
            rpt.AddParameter("StampaLotto", "S")
        Else
            rpt.AddParameter("StampaLotto", "N")
        End If

        Dim scrittaCertificato As String = String.Empty

        If stampaScrittaCertificato Then

            Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                Using bizPaziente As New BizPaziente(genericProvider, settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    scrittaCertificato = bizPaziente.GetEsitoControlloSituazioneVaccinale(Convert.ToInt64(OnVacUtility.Variabili.PazId))

                End Using
            End Using

        End If

        rpt.AddParameter("scrittaCertificato", scrittaCertificato)

        Return rpt

    End Function

#End Region

#Region " Utenti/Gruppi "

    Public Shared Function IsCurrentUserInGroup(idGruppo As String) As Boolean
        '--
        Dim groupId As Int32
        '--
        If String.IsNullOrEmpty(idGruppo) OrElse Not Int32.TryParse(idGruppo, groupId) Then Return False
        '--
        Dim isInGroup As Boolean = False
        '--
        ' Connessione a onit_manager
        Using dam As IDAM = OnVacUtility.OpenDamToOnitManager()
            '--
            Try
                '--
                dam.QB.NewQuery()
                dam.QB.AddTables("t_ana_link_gruppi_utenti")
                dam.QB.AddSelectFields("lgu_gru_id")
                dam.QB.AddWhereCondition("lgu_ute_id", Comparatori.Uguale, OnVacContext.UserId, DataTypes.Numero)
                dam.QB.AddWhereCondition("lgu_gru_id", Comparatori.Uguale, groupId, DataTypes.Numero)
                '--
                Dim obj As Object = dam.ExecScalar()
                '--
                isInGroup = Not obj Is Nothing AndAlso Not obj Is System.DBNull.Value
                '--
            Catch
                '--
                Return False
                '--
            End Try
            '--
        End Using
        '--
        Return isInGroup
        '--
    End Function

	''' <summary>
	''' Restituisce il filtro da impostare nell'OnitModalList che legge gli utenti della v_ana_utenti.
	''' </summary>
	''' <param name="includiUtentiObsoleti"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Shared Function GetFiltroUtenteForOnitModalList(escludiUtentiObsoleti As Boolean) As String

		Dim filtroObsoleti As String = String.Empty
		If escludiUtentiObsoleti Then filtroObsoleti = "AND (UTE_OBSOLETO = 'N' OR UTE_OBSOLETO = 'V' OR UTE_OBSOLETO IS NULL)"

		Return String.Format("UTE_APP_ID = '{0}' {1} ORDER BY UTE_CODICE", Onit.OnAssistnet.OnVac.OnVacContext.AppId, filtroObsoleti)

	End Function


#End Region

#Region " Format "

	''' <summary> 
	''' Restituisce un valore formattato in base al valore di partenza, al tipo e alla formattazione specificati.
	''' Nel caso in cui il dato di partenza sia nullo, viene restituita la stringa vuota
	''' </summary>
	Public Shared Function GetFormattedValue(dataItem As Object, typeName As String, formatString As String) As Object

        Return OnVacUtility.GetFormattedValue(dataItem, typeName, formatString, String.Empty)

    End Function

    ''' <summary> 
    ''' Restituisce un valore formattato in base al valore di partenza, al tipo e alla formattazione specificati.
    ''' Nel caso in cui il dato di partenza sia nullo, viene restituito il valore nullValue specificato.
    ''' </summary> 
    ''' <param name="dataItem">Valore del campo da formattare</param>
    ''' <param name="typeName">Il tipo di dato in stringa. Ad esempio "System.DateTime"</param>
    ''' <param name="formatString">La formattazione da applicare al tipo di dato</param>
    ''' <param name="nullValue">Valore da utilizzare se il campo è nullo</param>
    Public Shared Function GetFormattedValue(dataItem As Object, typeName As String, formatString As String, nullValue As String) As Object

        If nullValue Is Nothing Then nullValue = String.Empty

        If dataItem Is Nothing OrElse dataItem Is DBNull.Value Then Return nullValue

        Dim returnValue As Object = dataItem

        If returnValue.ToString() <> "" Then

            returnValue = Convert.ChangeType(returnValue, Type.GetType(typeName))

            If Not String.IsNullOrEmpty(formatString) Then
                returnValue = returnValue.GetType().InvokeMember("ToString", Reflection.BindingFlags.InvokeMethod, Nothing, returnValue, New Object() {formatString})
            End If

        End If

        Return returnValue

    End Function

    Public Shared Function GetNumberFormatInfo(showCurrencySymbol As Boolean) As System.Globalization.NumberFormatInfo

        Dim _NumberFormatInfo As New System.Globalization.NumberFormatInfo()
        _NumberFormatInfo.CurrencyDecimalSeparator = ","
        _NumberFormatInfo.CurrencyGroupSeparator = "."
        _NumberFormatInfo.CurrencyDecimalDigits = 2
        _NumberFormatInfo.CurrencyPositivePattern = 2

        If showCurrencySymbol Then
            _NumberFormatInfo.CurrencySymbol = "€"
        Else
            _NumberFormatInfo.CurrencySymbol = String.Empty
        End If

        Return _NumberFormatInfo

    End Function

#End Region

#Region " Get Url "

    ''' <summary>
    ''' Restituisce il path della cartella in cui si trovano le icone comuni di OnAssistNet.
    ''' Il percorso di riferimento deve essere impostato nel file di configurazione.
    ''' </summary>
    ''' <param name="iconFileName">Nome file immagine</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetIconUrl(iconFileName As String) As String

        Return GetPercorsoCompleto("~/images/", iconFileName)

    End Function

    ''' <summary>
    ''' Restituisce il path del foglio di stile comune per la Protesica.
    ''' Il percorso di riferimento deve essere impostato nel file di configurazione.
    ''' </summary>
    ''' <param name="cssFileName">Foglio di stile</param>
    ''' <returns></returns>
    ''' <remarks>Il file css deve essere presente nella cartella in cui si trovano i fogli di stile comuni di OnAssistNet</remarks>
    Public Shared Function GetCssUrl(cssFileName As String) As String

        Return "~/" & cssFileName

    End Function

    ''' <summary>
    ''' Restituisce il path del file de script spcificato.
    ''' Il percorso di riferimento deve essere impostato nel file di configurazione.
    ''' </summary>
    ''' <param name="scriptFileName">File di script</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetScriptUrl(scriptFileName As String) As String

        Return VirtualPathUtility.ToAbsolute("~/common/scripts/") & scriptFileName

    End Function

    Private Shared Function GetPercorsoCompleto(path As String, fileName As String) As String

        If String.IsNullOrWhiteSpace(path) Then
            Return fileName
        End If

        If Not path.EndsWith("/") Then
            path += "/"
        End If

        Return VirtualPathUtility.ToAbsolute(path + fileName)

    End Function

#End Region

#Region " Consenso "

    ''' <summary>
    ''' Restituisce lo stato del consenso alla comunicazione relativo al paziente, in base al codice (locale) indicato.
    ''' </summary>
    Public Shared Function GetConsensoComunicazionePaziente(codicePaziente As Integer, settings As Settings.Settings) As Consenso.StatoConsensoPaziente

        ' Lettura ausiliario paziente
        Dim codiceAusiliarioPaziente As String = String.Empty

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizPaziente As New BizPaziente(genericProvider, settings, OnVacContext.CreateBizContextInfos(), Nothing)

                ' Recupero consenso (il caso in cui il codiceAusiliario è String.Empty è già previsto dalla funzione del consenso)
                Return bizPaziente.GetConsensoComunicazionePaziente(codiceAusiliarioPaziente)

            End Using
        End Using

    End Function

    ''' <summary>
    ''' Restituisce lo stato del consenso globale del paziente (senza filtro per id consenso)
    ''' </summary>
    ''' <param name="codiceAusiliarioPaziente"></param>
    ''' <param name="settings"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetConsensoGlobalePaziente(codiceAusiliarioPaziente As String, settings As Settings.Settings) As Consenso.StatoConsensoPaziente

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizPaziente As New BizPaziente(genericProvider, settings, OnVacContext.CreateBizContextInfos(), Nothing)

                ' Recupero consenso (il caso in cui il codiceAusiliario è String.Empty è già previsto dalla funzione del consenso)
                Return bizPaziente.GetConsensoGlobalePaziente(codiceAusiliarioPaziente)

            End Using
        End Using

    End Function

    ''' <summary>
    ''' Restituisce lo stato del consenso globale dell'ultimo paziente selezionato.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetConsensoUltimoPazienteSelezionato(ultimoPazienteSelezionato As Entities.UltimoPazienteSelezionato, settings As Settings.Settings) As Consenso.StatoConsensoPaziente

        If ultimoPazienteSelezionato Is Nothing OrElse
           (String.IsNullOrEmpty(ultimoPazienteSelezionato.CodicePazienteCentrale) AndAlso
            String.IsNullOrEmpty(ultimoPazienteSelezionato.CodicePazienteLocale)) Then

            Return Nothing
        End If

        Dim codiceAusiliarioPaziente As String = ultimoPazienteSelezionato.CodicePazienteCentrale

        If String.IsNullOrEmpty(codiceAusiliarioPaziente) Then

            Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                codiceAusiliarioPaziente = genericProvider.Paziente.GetCodiceAusiliario(ultimoPazienteSelezionato.CodicePazienteLocale)
            End Using

            If String.IsNullOrEmpty(codiceAusiliarioPaziente) Then Return Nothing

        End If

        Return GetConsensoGlobalePaziente(codiceAusiliarioPaziente, settings)

    End Function

    ''' <summary>
    ''' Restituisce il codice dell'azienda in base all'applicativo corrente.
    ''' Utilizza la t_usl_gestite. Se non è impostata nessuna usl, restituisce il codice della usl corrente.
    ''' </summary>
    ''' <param name="settings"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetCodiceAziendaRegistrazione(settings As Settings.Settings) As String

        If String.IsNullOrWhiteSpace(OnVacContext.CodiceUslCorrente) Then Return OnVacContext.Azienda

        Return OnVacContext.CodiceUslCorrente

    End Function

#End Region

#Region " Log "

    ''' <summary>
    ''' Restituisce un oggetto BizLogOptions relativo all'argomento specificato e con tipo di operazione impostato ad "automatico".
    ''' </summary>
    Public Shared Function CreateBizLogOptions(codiceArgomento As String) As BizLogOptions

        Return OnVacUtility.CreateBizLogOptions(codiceArgomento, False)

    End Function

    ''' <summary>
    ''' Restituisce un oggetto BizLogOptions relativo all'argomento e al tipo di operazione specificati.
    ''' </summary>
    Public Shared Function CreateBizLogOptions(codiceArgomento As String, automatico As Boolean) As BizLogOptions

        Return New BizLogOptions(codiceArgomento, automatico)

    End Function

#End Region

#Region " Installazioni "

    ''' <summary>
    ''' Restituisce i dati di installazione relativi alla ulss corrente
    ''' </summary>
    Public Shared Function GetDatiInstallazioneCorrente(settings As Settings.Settings) As Installazione

        Dim installazione As Installazione = Nothing

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizInstallazioni As New BizInstallazioni(genericProvider, settings, OnVacContext.CreateBizContextInfos())

                installazione = bizInstallazioni.GetInstallazioneCorrente()

            End Using
        End Using

        Return installazione

    End Function

#End Region

#Region " Postel "

    ''' <summary>
    ''' Riempie la radiobuttonlist con i valori indicati nel parametro EXPORT_POSTEL_TIPO_AVVISO_VISIBILE.
    ''' Tutti i valori possibili sono elencati in Constants.TipoAvvisoPostel.
    ''' </summary>
    ''' <param name="rdb"></param>
    ''' <remarks></remarks>
    Public Shared Sub BindTipoAvvisoPostel(rdbList As RadioButtonList, settings As Settings.Settings)

        rdbList.ClearSelection()
        rdbList.Items.Clear()

        Dim listTipoAvviso As List(Of String) = settings.EXPORT_POSTEL_TIPO_AVVISO_VISIBILE

        If Not listTipoAvviso Is Nothing AndAlso listTipoAvviso.Count > 0 Then

            If listTipoAvviso.Contains(Constants.TipoAvvisoPostel.Avvisi) Then
                rdbList.Items.Add(New ListItem("Avvisi", Constants.TipoAvvisoPostel.Avvisi))
            End If

            If listTipoAvviso.Contains(Constants.TipoAvvisoPostel.Sollecito) Then
                rdbList.Items.Add(New ListItem("Sollecito", Constants.TipoAvvisoPostel.Sollecito))
            End If

            If listTipoAvviso.Contains(Constants.TipoAvvisoPostel.TerminePerentorio) Then
                rdbList.Items.Add(New ListItem("Termine Perentorio", Constants.TipoAvvisoPostel.TerminePerentorio))
            End If

            Dim listItem As New ListItem("Tutti", String.Empty)
            listItem.Selected = True

            rdbList.Items.Add(listItem)

        End If

    End Sub

    Public Class OpenPostelHandlerCommand
        Public CodiceConsultorio As String
        Public CodiceAmbulatorio As Integer
        Public DataInizioAppuntamenti As DateTime?
        Public DataFineAppuntamenti As DateTime?
        Public FiltroPazientiAvvisati As Enumerators.FiltroAvvisati
        Public CodiceCittadinanza As String
        Public DataInizioNascita As DateTime?
        Public DataFineNascita As DateTime?
        Public TipoAvviso As String
        Public Distretto As String
        Public FiltroAssociazioniDosi As Entities.FiltroComposto
        Public ArgomentoExport As String
        Public CurrentPage As Page
    End Class

    ''' <summary>
    ''' Apertura handler per export postel. Compone la querystring in base ai parametri impostati.
    ''' </summary>
    ''' <param name="command"></param>
    ''' <remarks></remarks>
    Public Shared Sub OpenPostelHandler(command As OpenPostelHandlerCommand)

        Dim filtri As New System.Text.StringBuilder()

        ' Consultorio
        If Not String.IsNullOrEmpty(command.CodiceConsultorio) Then
            filtri.AppendFormat("&cns={0}", command.CodiceConsultorio)
        End If

        ' Ambulatorio
        If command.CodiceAmbulatorio > 0 Then
            filtri.AppendFormat("&amb={0}", command.CodiceAmbulatorio.ToString())
        End If

        ' Periodo appuntamento
        If command.DataInizioAppuntamenti.HasValue AndAlso command.DataInizioAppuntamenti.Value > DateTime.MinValue Then
            filtri.AppendFormat("&da={0}", command.DataInizioAppuntamenti.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture))
        End If

        If command.DataFineAppuntamenti.HasValue AndAlso command.DataFineAppuntamenti.Value > DateTime.MinValue Then
            filtri.AppendFormat("&a={0}", command.DataFineAppuntamenti.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture))
        End If

        ' Filtro avvisati
        If command.FiltroPazientiAvvisati <> Enumerators.FiltroAvvisati.Tutti Then
            filtri.AppendFormat("&avv={0}", DirectCast(command.FiltroPazientiAvvisati, Integer).ToString())
        End If

        ' Cittadinanza
        If Not String.IsNullOrEmpty(command.CodiceCittadinanza) Then
            filtri.AppendFormat("&citt={0}", command.CodiceCittadinanza)
        End If

        ' Intervallo nascita
        If command.DataInizioNascita.HasValue AndAlso command.DataInizioNascita.Value > DateTime.MinValue Then
            filtri.AppendFormat("&daNasc={0}", command.DataInizioNascita.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture))
        End If

        If command.DataFineNascita.HasValue AndAlso command.DataFineNascita.Value > DateTime.MinValue Then
            filtri.AppendFormat("&aNasc={0}", command.DataFineNascita.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture))
        End If
        If Not String.IsNullOrWhiteSpace(command.Distretto) Then
            filtri.AppendFormat("&distr={0}", command.Distretto)
        End If

        ' Tipo avvisi postel
        filtri.AppendFormat("&tipoavviso={0}", command.TipoAvviso)

        ' Associazioni-dosi
        If Not command.FiltroAssociazioniDosi Is Nothing Then

            ' N.B. : la serializzazione avviene correttamente ma la deserializzazione no: non so perchè, mi arrangio...
            'Dim ser As New System.Web.Script.Serialization.JavaScriptSerializer()
            'filtri.AppendFormat("&ass={0}", ser.Serialize(command.FiltroAssociazioniDosi))

            If Not command.FiltroAssociazioniDosi.CodiceValore.IsNullOrEmpty() Then

                Dim ass As IEnumerable(Of String) = command.FiltroAssociazioniDosi.CodiceValore.Select(Function(item) String.Format("{0};{1}", item.Key, item.Value))

                filtri.AppendFormat("&ass={0}", String.Join("|", ass.ToArray()))

            End If

            If Not command.FiltroAssociazioniDosi.Valori.IsNullOrEmpty() Then

                filtri.AppendFormat("&dosi={0}", String.Join("|", command.FiltroAssociazioniDosi.Valori.ToArray()))

            End If

        End If

        ' Open file export handler
        Dim url As String = String.Format("{0}?argomento={1}&appId={2}&codAzienda={3}&userId={4}&ulss={5}{6}",
                                          command.CurrentPage.ResolveClientUrl("~/Common/Handlers/FileExportHandler.ashx"),
                                          command.ArgomentoExport, OnVacContext.AppId, OnVacContext.Azienda, OnVacContext.UserId.ToString(),
                                          OnVacContext.CodiceUslCorrente, filtri.ToString())

        DirectCast(command.CurrentPage, Common.PageBase).RegisterStartupScriptCustom("openFileExportHandler", String.Format("window.open('{0}');", url))

    End Sub

#End Region

#Region " Varie "

    ''' <summary>
    ''' Riempie la radioButtonList con i valori previsti per i Soggetti relativi all'avviso.
    ''' </summary>
    ''' <param name="rdbList"></param>
    ''' <remarks></remarks>
    Public Shared Sub BindTipoSoggettiAvviso(rdbList As RadioButtonList)

        rdbList.ClearSelection()
        rdbList.Items.Clear()

        rdbList.Items.Add(New ListItem("Tutti", DirectCast(Enumerators.FiltroAvvisati.Tutti, Integer).ToString()))
        rdbList.Items.Add(New ListItem("Solo quelli già avvisati", DirectCast(Enumerators.FiltroAvvisati.SoloAvvisati, Integer).ToString()))
        rdbList.Items.Add(New ListItem("Solo quelli non avvisati", DirectCast(Enumerators.FiltroAvvisati.SoloNonAvvisati, Integer).ToString()))

        rdbList.Items(0).Selected = True

    End Sub

    Public Shared Function GetCodeValue(panel As OnitDataPanel.OnitDataPanel, dgr As OnitDataPanel.wzDataGrid) As String

        Return GetCodeValue(panel, dgr, False)

    End Function

    Public Shared Function GetCodeValue(panel As OnitDataPanel.OnitDataPanel, dgr As OnitDataPanel.wzMsDataGrid) As String

        Return GetCodeValue(panel, dgr, False)

    End Function

    Public Shared Function GetCodeValue(panel As OnitDataPanel.OnitDataPanel, dgr As OnitDataPanel.wzDataGrid, removeLastSeparator As Boolean) As String

        Dim value As New System.Text.StringBuilder()

        Dim keys As OnitDataPanel.keyFieldInfoCollection = panel.MainTable.keyFields

        For Each k As OnitDataPanel.keyFieldInfo In keys
            value.AppendFormat("{0}|", dgr.DisplayLayout.ActiveRow.GetCellText(dgr.Columns.FromKey(k.FieldName)))
        Next

        If removeLastSeparator Then
            If value.Length > 0 Then value.Remove(value.Length - 1, 1)
        End If

        Return value.ToString()

    End Function

    Public Shared Function GetCodeValue(panel As OnitDataPanel.OnitDataPanel, dgr As OnitDataPanel.wzMsDataGrid, removeLastSeparator As Boolean) As String

        Dim value As New System.Text.StringBuilder()

        Dim keys As OnitDataPanel.keyFieldInfoCollection = panel.MainTable.keyFields

        For Each k As OnitDataPanel.keyFieldInfo In keys
            value.AppendFormat("{0}|", dgr.SelectedItem.Cells(dgr.getColumnNumberByKey(k.FieldName)).Text)
        Next

        If removeLastSeparator Then
            If value.Length > 0 Then value.Remove(value.Length - 1, 1)
        End If

        Return value.ToString()

    End Function

    'disabilita un controllo passato come parametro [modifica 05/07/2005]
    Public Shared Sub DisabilitaModale(ByRef controllo As Object, enable As Boolean)

        Dim mdl As OnitModalList = DirectCast(controllo, Onit.Controls.OnitModalList)

        mdl.Enabled = (Not enable)
        mdl.CssClass = IIf(enable, "TextBox_Stringa_Disabilitato", "TextBox_Stringa")

        If enable Then
            mdl.Codice = String.Empty
            mdl.Descrizione = String.Empty
        End If

    End Sub

    ''' <summary>
    ''' Restituisce il valore dell'enumerazione corrispondente al livello dell'utente (relativamente a convocazioni/prenotazioni)
    ''' specificato nella querystring passata come parametro.
    ''' </summary>
    Public Shared Function GetLivelloUtenteConvocazione(queryString As System.Collections.Specialized.NameValueCollection, defaultIfUndefined As Boolean) As Enumerators.LivelloUtenteConvocazione

        If queryString Is Nothing OrElse queryString.Count = 0 Then

            ' Querystring non presente
            If defaultIfUndefined Then
                Return Enumerators.LivelloUtenteConvocazione.Default
            Else
                Return Enumerators.LivelloUtenteConvocazione.Undefined
            End If

        End If

        ' Lettura parametro da querystring
        Dim levelCNV As String = queryString.Get("levelCNV")

        If String.IsNullOrEmpty(levelCNV) Then

            ' Parametro non presente
            If defaultIfUndefined Then
                Return Enumerators.LivelloUtenteConvocazione.Default
            Else
                Return Enumerators.LivelloUtenteConvocazione.Undefined
            End If

        End If

        ' Parsing del parametro
        Dim enumValue As Enumerators.LivelloUtenteConvocazione

        If Not [Enum].TryParse(Of Enumerators.LivelloUtenteConvocazione)(levelCNV, enumValue) Then

            ' Parsing non riuscito
            If defaultIfUndefined Then
                Return Enumerators.LivelloUtenteConvocazione.Default
            Else
                Return Enumerators.LivelloUtenteConvocazione.Undefined
            End If

        End If

        ' Controllo valore parametro compreso nell'enumerazione
        Dim arrayEnumValues As Enumerators.LivelloUtenteConvocazione() = [Enum].GetValues(GetType(Enumerators.LivelloUtenteConvocazione))

        If Not arrayEnumValues.Contains(enumValue) Then

            ' Valore parametro non compreso nell'enumerazione
            If defaultIfUndefined Then
                Return Enumerators.LivelloUtenteConvocazione.Default
            Else
                Return Enumerators.LivelloUtenteConvocazione.Undefined
            End If

        End If

        Return enumValue

    End Function

    Shared Function GetJSFocus(htmlInputClientId As String, Optional selectIt As Boolean = True) As String

        Dim js As String = ""

        If selectIt Then js &= "document.getElementById('" & htmlInputClientId & "').select();"
        js &= "document.getElementById('" & htmlInputClientId & "').focus()"

        Return GetJSAddProcToWindowOnLoad(js, htmlInputClientId & "Focus")

    End Function

    Shared Function GetJSAddProcToWindowOnLoad(jsProc As String, key As String) As String

        Return "addProcToEvent(""window.onload"",""" & jsProc & """,""" & key & """)"

    End Function

    ''' <summary>
    ''' Restituisce true se il numero di giorni trascorsi dalla data specificata è inferiore al parametro GIORNI_VARIAZIONE_VACCINAZIONE_ESEGUITA.
    ''' Se il parametro GIORNI_VARIAZIONE_VACCINAZIONE_ESEGUITA è minore o uguale a 0, non effettua controlli e restituisce true.
    ''' Se l'utente corrente appartiene al gruppo specificato in ID_GRUPPO_ADMIN_DATI_VACCINALI, restituisce true senza effettuare nessun controllo.
    ''' </summary>
    ''' <param name="value"></param>
    ''' <param name="settings"></param>
    ''' <returns></returns>
    Public Shared Function CheckGiorniTrascorsiVariazioneDatiVaccinali(value As Object, settings As Settings.Settings) As Boolean

        If settings.GIORNI_VARIAZIONE_VACCINAZIONE_ESEGUITA <= 0 Then Return True

        If OnVacUtility.IsCurrentUserInGroup(settings.ID_GRUPPO_ADMIN_DATI_VACCINALI) Then Return True

        Dim dataDaValutare As DateTime = DateTime.MinValue

        Try
            dataDaValutare = Convert.ToDateTime(value)
        Catch ex As Exception
            dataDaValutare = DateTime.MinValue
        End Try

        If dataDaValutare > DateTime.MinValue Then

            Dim numGiorni As Integer = (Date.Today - dataDaValutare.Date).Days

            If numGiorni > settings.GIORNI_VARIAZIONE_VACCINAZIONE_ESEGUITA Then

                Return False

            End If

        End If

        Return True

    End Function

    Public Shared Function GetReadCommittedTransactionOptions() As Transactions.TransactionOptions

        Dim transactionOptions As New Transactions.TransactionOptions()
        transactionOptions.IsolationLevel = Transactions.IsolationLevel.ReadCommitted

        Return transactionOptions

    End Function

    ''' <summary>
    ''' Imposta nella label il valore della data, se specificato, altrimenti la sbianca.
    ''' </summary>
    ''' <param name="lbl"></param>
    ''' <param name="valoreData"></param>
    ''' <remarks></remarks>
    Public Shared Sub SetLabelCampoDate(lbl As Label, valoreData As DateTime?)
        '--
        lbl.Text = String.Empty
        '--
        If valoreData.HasValue Then lbl.Text = valoreData.Value.ToString("dd/MM/yyyy")
        '--
    End Sub

    Public Shared Function GetStatoImportUrlIcona(value As String, page As Page) As String

        Select Case value
            Case "S"
                Return page.ResolveClientUrl("~/images/import_succes.png")
            Case "F"
                Return page.ResolveClientUrl("~/images/import_fail.png")
            Case "R"
                Return page.ResolveClientUrl("~/images/import_running.png")
            Case "W"
                Return page.ResolveClientUrl("~/images/import_waiting.png")
        End Select

        Return String.Empty

    End Function

    Public Shared Function GetStatoImportToolTip(value As String) As String

        Select Case value
            Case "S"
                Return "Elaborazione terminata con successo"
            Case "F"
                Return "Elaborazione terminata con errori"
            Case "R"
                Return "Elaborazione in corso"
            Case "W"
                Return "In attesa di essere elaborato"
        End Select

        Return String.Empty

    End Function

    ''' <summary>
    ''' Restituisce il filtro per la modale degli operatori. 
    ''' Filtra in base alla qualifica (C per i soli medici, C o D per tutti gli operatori vaccinali), alla validità 
    ''' e all'associazione dell'operatore con il centro corrente.
    ''' </summary>
    ''' <param name="soloMedici"></param>
    ''' <returns></returns>
    Public Shared Function GetModalListFilterOperatori(soloMedici As Boolean, soloValidi As Boolean) As String

        Dim filter As New Text.StringBuilder()

        If soloMedici Then
            filter.Append(" OPE_QUALIFICA = 'C' ")
        Else
            filter.Append(" OPE_QUALIFICA in ('C','D') ")
        End If

        If soloValidi Then
            filter.Append(" AND (OPE_OBSOLETO IS NULL OR OPE_OBSOLETO = 'N') ")
        End If

        filter.AppendFormat(" AND LOC_OPE_CODICE = OPE_CODICE AND LOC_CNS_CODICE = '{0}' ", OnVacUtility.Variabili.CNS.Codice)

        filter.Append(" order by OPE_NOME ")

        Return filter.ToString()

    End Function

#End Region

#Region " Firma digitale "

    ''' <summary>
    ''' Restituisce l'url dell'immagine relativa allo stato della firma digitale e dell'archiviazione sostitutiva
    ''' </summary>
    ''' <param name="valoreCampoFirma"></param>
    ''' <param name="valoreCampoArchiviazione"></param>
    ''' <param name="page"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetFlagFirmaImageUrlValue(valoreCampoFirma As Object, valoreCampoArchiviazione As Object, page As Page) As String

        Dim flagValue As FlagFirmaArchiviazione = OnVacUtility.GetFlagFirmaArchiviazione(valoreCampoFirma, valoreCampoArchiviazione)

        Select Case flagValue
            Case FlagFirmaArchiviazione.SoloFirma
                Return page.ResolveClientUrl("~/Images/firmaDigitale.png")
            Case FlagFirmaArchiviazione.FirmaArchiviazione
                Return page.ResolveClientUrl("~/Images/archiviazione.png")
        End Select

        Return page.ResolveClientUrl("~/Images/transparent16.gif")

    End Function

    ''' <summary>
    ''' Restituisce il tooltip relativo allo stato della firma digitale e dell'archiviazione sostitutiva
    ''' </summary>
    ''' <param name="valoreCampoFirma"></param>
    ''' <param name="valoreCampoArchiviazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetFlagFirmaToolTipValue(valoreCampoFirma As Object, valoreCampoArchiviazione As Object) As String

        Dim flagValue As FlagFirmaArchiviazione = OnVacUtility.GetFlagFirmaArchiviazione(valoreCampoFirma, valoreCampoArchiviazione)

        Select Case flagValue
            Case FlagFirmaArchiviazione.SoloFirma
                Return "Il documento è FIRMATO DIGITALMENTE ma non è in archiviazione sostitutiva"
            Case FlagFirmaArchiviazione.FirmaArchiviazione
                Return "Il documento è FIRMATO DIGITALMENTE ed è in ARCHIVIAZIONE SOSTITUTIVA"
        End Select

        Return "Il documento non è firmato digitalmente"

    End Function

    Private Enum FlagFirmaArchiviazione
        None = 0
        SoloFirma = 1
        FirmaArchiviazione = 2
    End Enum

    Private Shared Function GetFlagFirmaArchiviazione(valoreCampoFirma As Object, valoreCampoArchiviazione As Object) As FlagFirmaArchiviazione

        Dim flagFirma As Boolean = OnVacUtility.GetFlagValue(valoreCampoFirma)
        Dim flagArchiviazione As Boolean = OnVacUtility.GetFlagValue(valoreCampoArchiviazione)

        If flagFirma Then
            If flagArchiviazione Then
                Return FlagFirmaArchiviazione.FirmaArchiviazione
            Else
                Return FlagFirmaArchiviazione.SoloFirma
            End If
        End If

        Return FlagFirmaArchiviazione.None

    End Function

    Private Shared Function GetFlagValue(valoreCampo As Object) As Boolean

        Return Not valoreCampo Is Nothing AndAlso Not valoreCampo Is DBNull.Value AndAlso Not String.IsNullOrEmpty(valoreCampo.ToString())

    End Function

#End Region

#Region " Legenda Vaccinazioni "

    Public Shared Function LegendaVaccinazioniBindToolTip(legendaItem As Enumerators.LegendaVaccinazioniItemType) As String

        Return OnVacUtility.LegendaVaccinazioniBindToolTip(Nothing, legendaItem, False)

    End Function

    Public Shared Function LegendaVaccinazioniBindToolTip(dataItemValue As Object, legendaItem As Enumerators.LegendaVaccinazioniItemType, flagAbilitazioneVaccUslCorrente As Boolean) As String

        Dim tooltip As String = String.Empty

        Select Case legendaItem
            Case Enumerators.LegendaVaccinazioniItemType.ReazioneAvversa
                tooltip = "Reazione avversa"
            Case Enumerators.LegendaVaccinazioniItemType.EseguitaScaduta
                tooltip = "Associazione scaduta"
            Case Enumerators.LegendaVaccinazioniItemType.EseguitaFittizia
                tooltip = "Associazione fittizia"
            Case Enumerators.LegendaVaccinazioniItemType.EsclusioneScaduta
                tooltip = "Esclusione scaduta"
            Case Enumerators.LegendaVaccinazioniItemType.Obbligatoria
                tooltip = "Vaccinazione obbligatoria"
            Case Enumerators.LegendaVaccinazioniItemType.Eseguita
                tooltip = "Vaccinazione eseguita"
            Case Enumerators.LegendaVaccinazioniItemType.Esclusa
                tooltip = "Vaccinazione esclusa"
        End Select

        If legendaItem = Enumerators.LegendaVaccinazioniItemType.ReazioneAvversa Or
           legendaItem = Enumerators.LegendaVaccinazioniItemType.EseguitaScaduta Then

            If flagAbilitazioneVaccUslCorrente AndAlso dataItemValue IsNot Nothing AndAlso dataItemValue IsNot DBNull.Value Then

                tooltip += String.Format("{0}(azienda: {1})", Environment.NewLine, dataItemValue.ToString())

            End If

        End If

        Return tooltip

    End Function

    Public Shared Function LegendaVaccinazioniBindVisibility(dataItemValue As Object, legendaItem As Enumerators.LegendaVaccinazioniItemType) As String

        If dataItemValue IsNot Nothing Then

            Select Case legendaItem

                Case Enumerators.LegendaVaccinazioniItemType.ReazioneAvversa

                    If dataItemValue IsNot DBNull.Value Then
                        Return Boolean.TrueString
                    End If

                Case Enumerators.LegendaVaccinazioniItemType.EseguitaScaduta,
                     Enumerators.LegendaVaccinazioniItemType.EseguitaFittizia,
                     Enumerators.LegendaVaccinazioniItemType.EsclusioneScaduta

                    Dim value As String = dataItemValue.ToString()

                    If value = "S" OrElse value = Boolean.TrueString Then
                        Return Boolean.TrueString
                    End If

                Case Enumerators.LegendaVaccinazioniItemType.Obbligatoria

                    If dataItemValue IsNot DBNull.Value Then

                        If dataItemValue.ToString() = Constants.ObbligatorietaVaccinazione.Obbligatoria Then Return Boolean.TrueString

                    End If

                Case Enumerators.LegendaVaccinazioniItemType.Eseguita

                    If dataItemValue.ToString() = "eseguita" Then
                        Return Boolean.TrueString
                    End If

                Case Enumerators.LegendaVaccinazioniItemType.Esclusa

                    ' Legenda visibile se data scadenza nulla o data scadenza superiore ad oggi
                    If dataItemValue Is DBNull.Value Then Return Boolean.TrueString

                    Dim dataScadenzaEsclusione As DateTime
                    If DateTime.TryParse(dataItemValue, dataScadenzaEsclusione) Then

                        If dataScadenzaEsclusione > DateTime.Now.Date Then Return Boolean.TrueString

                    End If

            End Select

        End If

        Return Boolean.FalseString

    End Function

#End Region

#Region " Dalla portale shared "

    ''' <summary>
    ''' Questa funzione ritorna i parametri globali comuni agli applicativi del portale. L'elenco dei parametri viene passato in input come sequenza di stringhe.
    ''' </summary>
    ''' <param name="Parametri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetGlobalParams(ParamArray Parametri() As String) As ReturnValue

        Dim dam As IDAM = DAMBuilder.CreateDAM(Onit.Shared.NTier.OnitNTierAppRuntime.Settings.OnitManager.Provider.ToString(),
                                               Onit.Shared.NTier.OnitNTierAppRuntime.Settings.OnitManager.ConnectionString)

        Dim dt As New DataTable("t_ana_parametri")

        GetGlobalParams = New ReturnValue()

        dam.QB.AddSelectFields("par_codice", "par_valore")
        dam.QB.AddTables("t_ana_parametri")
        For i As Integer = 0 To Parametri.GetUpperBound(0)
            dam.QB.AddWhereCondition("par_codice", Comparatori.Uguale, Parametri(i), DataTypes.Stringa, "or")
        Next

        Try
            dam.BuildDataTable(dt)
            GetGlobalParams.ErrorMessage = Nothing
            GetGlobalParams.Value = dt
        Catch exc As Exception
            GetGlobalParams.ErrorMessage = exc.Message
            GetGlobalParams.Value = Nothing
        Finally
            dam.Dispose(True)
        End Try

    End Function

#End Region

#Region " FSE "

    Public Class FSEHelper

        'TODO[ITI61]Invio transaction ITI-61 (indicizzazione sul registry regionale)
        Public Shared Function IndicizzaSuRegistry(codPaziente As Long, tipoDocumento As String, funzionalitaLog As String, eventoLog As String, settings As Settings.Settings, codiceOperatore As String) As BizGenericResult

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizPazienti As New BizPaziente(genericProvider, settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    Return bizPazienti.IndicizzaSuRegistry(codPaziente, tipoDocumento, funzionalitaLog, eventoLog, settings, codiceOperatore)

                End Using
            End Using

        End Function

    End Class

#End Region

End Class
