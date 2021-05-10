Imports Onit.Controls.OnitDataPanel
Imports Infragistics.WebUI.UltraWebGrid

<Serializable()>
Public Class OnitDataPanelState

    Private sortToRestore As String
    Private currentCheckedValueKeys As ArrayList
    Private currentActiveValueKeys As Hashtable
    Private filter As FilterState
    Private dataNeedLoaded As Boolean

    Public Sub ClearState()
        sortToRestore = Nothing
        currentCheckedValueKeys = Nothing
        currentActiveValueKeys = Nothing
        filter = Nothing
        dataNeedLoaded = False
    End Sub

    Public Sub SaveFilterState(ByRef filter As wzFilter)
        If Not filter Is Nothing Then Me.filter = New FilterState(filter)
    End Sub

    Public Sub SavePanelState(ByRef odp As OnitDataPanel, ByRef dgr As wzDataGrid)
        '--
        Me.currentCheckedValueKeys = New ArrayList()
        '--
        'if dgr.SortingMode=
        Me.sortToRestore = odp.getCurrentSort()
        '--
        If Not odp.getCurrentDataTable Is Nothing AndAlso odp.getCurrentDataTable.Rows.Count > 0 Then
            Me.currentActiveValueKeys = New Hashtable
            Dim currentRow As DataRow = odp.getCurrentDataRow()
            If Not currentRow Is Nothing Then
                For i As Int16 = 0 To odp.MainTable.keyFields.Count - 1
                    Dim fieldNameTemp As String = odp.GetCurrentTableEncoder().getCode(odp.MainTable.TableName, odp.MainTable.keyFields(i).FieldName)
                    Me.currentActiveValueKeys.Add(fieldNameTemp, New Object() {currentRow(fieldNameTemp), odp.MainTable.keyFields(i).FieldType})
                Next
            End If
            dataNeedLoaded = True
        End If
        '--
        For i As Int16 = 0 To dgr.Rows.Count - 1
            If dgr.Rows(i).Cells(0).Value Then
                Dim ht As New Hashtable()
                For j As Int16 = 0 To odp.MainTable.keyFields.Count - 1
                    Dim fieldNameTemp As String = odp.GetCurrentTableEncoder().getCode(odp.MainTable.TableName, odp.MainTable.keyFields(j).FieldName)
                    ht.Add(fieldNameTemp, dgr.Rows(i).Cells.FromKey(fieldNameTemp).Value)
                Next
                Me.currentCheckedValueKeys.Add(ht)
            End If
        Next
        '--
    End Sub


    Public Sub SavePanelState(ByRef odp As OnitDataPanel, ByRef dgr As wzMsDataGrid)
        '--
        Me.currentCheckedValueKeys = New ArrayList()
        '--
        Me.sortToRestore = odp.getCurrentSort()
        '--
        If Not odp.getCurrentDataTable Is Nothing AndAlso odp.getCurrentDataTable.Rows.Count > 0 Then
            Me.currentActiveValueKeys = New Hashtable()
            Dim currentRow As DataRow = odp.getCurrentDataRow()
            If Not currentRow Is Nothing Then
                For i As Int16 = 0 To odp.MainTable.keyFields.Count - 1
                    Dim fieldNameTemp As String = odp.GetCurrentTableEncoder().getCode(odp.MainTable.TableName, odp.MainTable.keyFields(i).FieldName)
                    Me.currentActiveValueKeys.Add(fieldNameTemp, New Object() {currentRow(fieldNameTemp), odp.MainTable.keyFields(i).FieldType})
                Next
            End If
            dataNeedLoaded = True
        End If
        '--
        For i As Int16 = 0 To dgr.Items.Count - 1
            Dim chkCol As Control = dgr.Items(i).FindControl(wzMultiSelColumn.CheckBoxName)
            If chkCol Is Nothing Then
                Exit For
            End If

            If DirectCast(chkCol, CheckBox).Checked Then
                Dim ht As New Hashtable
                For j As Int16 = 0 To odp.MainTable.keyFields.Count - 1
                    Dim fieldNameTemp As String = odp.GetCurrentTableEncoder().getCode(odp.MainTable.TableName, odp.MainTable.keyFields(j).FieldName)
                    ht.Add(fieldNameTemp, dgr.Items(i).Cells(dgr.getColumnNumberByKey(fieldNameTemp)).Text)
                Next
                Me.currentCheckedValueKeys.Add(ht)
            End If
        Next
        '--
    End Sub


    Public Sub LoadState(ByRef odp As OnitDataPanel, ByRef filter As wzFilter, ByRef dgr As wzDataGrid)
        ' ------------------------------------------------------------------ '
        Dim defaultSort As String = odp.defaultSort
        ' ------------------------------------------------------------------ '
        If Not sortToRestore Is Nothing Then odp.defaultSort = sortToRestore
        ' ------------------------------------------------------------------ '
        If Not filter Is Nothing Then
            SetFilter(filter)
            If dataNeedLoaded Then odp.Find()
        Else
            If dataNeedLoaded Then odp.LoadData()
        End If
        ' ------------------------------------------------------------------ '
        If Not Me.currentActiveValueKeys Is Nothing Then
            Dim filterColl As New Onit.Controls.OnitDataPanel.FilterCollection()
            Dim currentActiveValueKeysEnum As IDictionaryEnumerator = Me.currentActiveValueKeys.GetEnumerator()
            While currentActiveValueKeysEnum.MoveNext()
                filterColl.Add(New Onit.Controls.OnitDataPanel.Filter(DirectCast(currentActiveValueKeysEnum.Value, Object())(0), New Onit.Controls.OnitDataPanel.BindingFieldValue(odp.MainTable.Connection, odp.MainTable.TableName, currentActiveValueKeysEnum.Key, "'" & DirectCast(currentActiveValueKeysEnum.Value, Object())(0) & "'"), Onit.Controls.OnitDataPanel.Filter.FilterComparators.Uguale, Onit.Controls.OnitDataPanel.Filter.FilterOperators.And, [Enum].Parse(GetType(Onit.Controls.OnitDataPanel.Filter.FieldTypes), DirectCast(currentActiveValueKeysEnum.Value, Object())(1).ToString), Nothing))
            End While
            If filterColl.Count > 0 Then
                Dim keepCurrentRecord As Boolean = dgr.keepCurrentRecord
                dgr.keepCurrentRecord = True
                odp.applySelectionFilters(filterColl)
                dgr.keepCurrentRecord = keepCurrentRecord
            End If
        End If
        ' ------------------------------------------------------------------ '
        If Not dgr Is Nothing Then
            If Not Me.currentCheckedValueKeys Is Nothing Then
                For j As Int16 = 0 To Me.currentCheckedValueKeys.Count - 1
                    Dim checkedValueKeys As Hashtable = DirectCast(Me.currentCheckedValueKeys(j), Hashtable)
                    For i As Int16 = 0 To dgr.Rows.Count - 1
                        Dim isRowToCheck As Boolean
                        Dim checkedValueKeysEnum As IDictionaryEnumerator = checkedValueKeys.GetEnumerator()
                        While checkedValueKeysEnum.MoveNext()
                            If dgr.Rows(i).Cells.FromKey(checkedValueKeysEnum.Key).Value = checkedValueKeysEnum.Value Then
                                isRowToCheck = True
                            Else
                                isRowToCheck = False
                                Exit While
                            End If
                        End While
                        If isRowToCheck Then
                            dgr.Rows(i).Cells(0).Value = True
                            Exit For
                        End If
                    Next
                Next
            End If
        End If
        ' ------------------------------------------------------------------ '
        If Not sortToRestore Is Nothing Then odp.defaultSort = defaultSort
        ' ------------------------------------------------------------------ '
    End Sub

    Public Sub LoadState(ByRef odp As OnitDataPanel, ByRef filter As wzFilter, ByRef dgr As wzMsDataGrid)
        ' ------------------------------------------------------------------ '
        Dim defaultSort As String = odp.defaultSort
        ' ------------------------------------------------------------------ '
        If Not sortToRestore Is Nothing Then odp.defaultSort = sortToRestore
        ' ------------------------------------------------------------------ '
        If Not filter Is Nothing Then
            SetFilter(filter)
            If dataNeedLoaded Then odp.Find()
        Else
            If dataNeedLoaded Then odp.LoadData()
        End If
        ' ------------------------------------------------------------------ '
        If Not Me.currentActiveValueKeys Is Nothing Then
            Dim filterColl As New Onit.Controls.OnitDataPanel.FilterCollection()
            Dim currentActiveValueKeysEnum As IDictionaryEnumerator = Me.currentActiveValueKeys.GetEnumerator()
            While currentActiveValueKeysEnum.MoveNext
                filterColl.Add(New Onit.Controls.OnitDataPanel.Filter(DirectCast(currentActiveValueKeysEnum.Value, Object())(0), New Onit.Controls.OnitDataPanel.BindingFieldValue(odp.MainTable.Connection, odp.MainTable.TableName, currentActiveValueKeysEnum.Key, "'" & DirectCast(currentActiveValueKeysEnum.Value, Object())(0) & "'"), Onit.Controls.OnitDataPanel.Filter.FilterComparators.Uguale, Onit.Controls.OnitDataPanel.Filter.FilterOperators.And, [Enum].Parse(GetType(Onit.Controls.OnitDataPanel.Filter.FieldTypes), DirectCast(currentActiveValueKeysEnum.Value, Object())(1).ToString), Nothing))
            End While
            If filterColl.Count > 0 Then
                Dim keepCurrentRecord As Boolean = dgr.keepCurrentRecord
                dgr.keepCurrentRecord = True
                odp.applySelectionFilters(filterColl)
                dgr.keepCurrentRecord = keepCurrentRecord
            End If
        End If
        ' ------------------------------------------------------------------ '
        If Not dgr Is Nothing Then
            If Not Me.currentCheckedValueKeys Is Nothing Then
                For j As Int16 = 0 To Me.currentCheckedValueKeys.Count - 1
                    Dim checkedValueKeys As Hashtable = DirectCast(Me.currentCheckedValueKeys(j), Hashtable)
                    For i As Int16 = 0 To dgr.Items.Count - 1
                        Dim isRowToCheck As Boolean
                        Dim checkedValueKeysEnum As IDictionaryEnumerator = checkedValueKeys.GetEnumerator
                        While checkedValueKeysEnum.MoveNext
                            If dgr.Items(i).Cells(dgr.getColumnNumberByKey(checkedValueKeysEnum.Key)).Text = checkedValueKeysEnum.Value Then
                                isRowToCheck = True
                            Else
                                isRowToCheck = False
                                Exit While
                            End If
                        End While
                        If isRowToCheck Then
                            DirectCast(dgr.Items(i).FindControl(wzMultiSelColumn.CheckBoxName), CheckBox).Checked = True
                            Exit For
                        End If
                    Next
                Next
            End If
        End If
        ' ------------------------------------------------------------------ '
        If Not sortToRestore Is Nothing Then odp.defaultSort = defaultSort
        ' ------------------------------------------------------------------ '
    End Sub


    Private Sub SetFilter(ByRef filter As wzFilter)
        If Not Me.filter Is Nothing Then
            filter.SelectedTab = Me.filter.selectedTab
            filter.TabIndex = Me.filter.selectedTab
            If filter.SelectedTab = 0 Then
                Dim tbKeyBase As TextBox = DirectCast(filter.FindControl("WzFilterKeyBase"), TextBox)
                Dim cbAllKeyBase As CheckBox = DirectCast(filter.FindControl("WzFilterFraseIntera"), CheckBox)
                If Not tbKeyBase Is Nothing Then
                    DirectCast(filter.FindControl("WzFilterKeyBase"), TextBox).Text = Me.filter.keyBase
                Else
                    Me.filter.SetCustomFilter(filter)
                End If
                If Not cbAllKeyBase Is Nothing Then DirectCast(filter.FindControl("WzFilterFraseIntera"), CheckBox).Checked = Me.filter.allKeyBase
            Else
                Me.filter.SetCustomFilter(filter)
            End If
        End If
    End Sub

    <Serializable()>
    Private Class FilterState
        Public keyBase As String
        Public allKeyBase As Boolean
        Public selectedTab As Int16
        Public keyCustom As Hashtable

        Public Sub New(ByRef filter As wzFilter)
            selectedTab = filter.SelectedTab
            If selectedTab = 0 Then
                Dim tbKeyBase As TextBox = DirectCast(filter.FindControl("WzFilterKeyBase"), TextBox)
                Dim cbAllKeyBase As CheckBox = DirectCast(filter.FindControl("WzFilterFraseIntera"), CheckBox)
                If Not tbKeyBase Is Nothing Then
                    keyBase = DirectCast(filter.FindControl("WzFilterKeyBase"), TextBox).Text
                Else
                    keyCustom = GetFilter(filter)
                End If
                If Not cbAllKeyBase Is Nothing Then allKeyBase = cbAllKeyBase.Checked
            Else
                keyCustom = GetFilter(filter)
            End If
        End Sub

        Public Function GetFilter(ByRef filter As wzFilter) As Hashtable
            Dim keyCustom = New Hashtable
            Dim c As Control
            Dim controls As ControlCollection = filter.GetSelectedTabReference.ContentPane.Children
            For Each c In controls
                If Not c.GetType.GetInterface("IOnitWebControl") Is Nothing Then
                    If c.GetType() Is GetType(wzOnitDatePick) Then
                        Dim text As String = DirectCast(c, wzOnitDatePick).Text
                        If text <> "" Then keyCustom.Add(c.ID, New Object() {text})
                    End If
                    If c.GetType() Is GetType(wzCheckBox) Then
                        Dim check As Boolean = DirectCast(c, wzCheckBox).Checked
                        If check Then keyCustom.Add(c.ID, New Object() {check})
                    End If
                    If c.GetType() Is GetType(wzDropDownList) Then
                        Dim value As String = DirectCast(c, wzDropDownList).SelectedValue
                        If Not value Is Nothing Then keyCustom.Add(c.ID, New Object() {DirectCast(c, wzDropDownList).SelectedValue})
                    End If
                    If c.GetType() Is GetType(wzRadioButtonList) Then
                        keyCustom.Add(c.ID, New Object() {DirectCast(c, wzDropDownList).SelectedValue})
                    End If
                    If c.GetType() Is GetType(wzTextBox) Then
                        Dim text As String = DirectCast(c, wzTextBox).Text
                        If text <> "" Then keyCustom.Add(c.ID, New Object() {text})
                    End If
                    If c.GetType() Is GetType(wzFinestraModale) Then
                        Dim fm As wzFinestraModale = DirectCast(c, wzFinestraModale)
                        Dim codice As String = fm.Codice
                        If codice <> "" Then
                            Dim valueKey As New ArrayList
                            If fm.CampiChiave = "" Then
                                valueKey.Add(codice)
                            Else
                                Dim keyFields As String() = fm.CampiChiave.Split("|")
                                For i As Int16 = 0 To keyFields.Length - 1
                                    valueKey.Add(fm.ValoriAltriCampi(keyFields(i)))
                                Next
                            End If
                            keyCustom.Add(c.ID, valueKey.ToArray(GetType(Object)))
                        End If
                    End If
                End If
            Next
            Return keyCustom
        End Function

        Public Sub SetCustomFilter(ByRef filter As wzFilter)
            Dim c As Control
            Dim controls As ControlCollection = filter.GetSelectedTabReference.ContentPane.Children
            For Each c In controls
                If Not c.GetType.GetInterface("IOnitWebControl") Is Nothing AndAlso Not Me.keyCustom(c.ID) Is Nothing Then
                    If c.GetType() Is GetType(wzOnitDatePick) Then
                        Dim cTemp As wzOnitDatePick = DirectCast(c, wzOnitDatePick)
                        cTemp.Text = Me.keyCustom(cTemp.ID)(0).ToString
                    End If
                    If c.GetType() Is GetType(wzCheckBox) Then
                        Dim cTemp As wzCheckBox = DirectCast(c, wzCheckBox)
                        cTemp.Checked = Boolean.Parse(Me.keyCustom(cTemp.ID)(0).ToString)
                    End If
                    If c.GetType() Is GetType(wzDropDownList) Then
                        Dim cTemp As wzDropDownList = DirectCast(c, wzDropDownList)
                        cTemp.ClearSelection()
                        cTemp.Items.FindByValue(Me.keyCustom(cTemp.ID)(0).ToString).Selected = True
                    End If
                    If c.GetType() Is GetType(wzRadioButtonList) Then
                        Dim cTemp As wzRadioButtonList = DirectCast(c, wzRadioButtonList)
                        cTemp.ClearSelection()
                        cTemp.Items.FindByValue(Me.keyCustom(cTemp.ID)(0).ToString).Selected = True
                    End If
                    If c.GetType() Is GetType(wzTextBox) Then
                        Dim cTemp As wzTextBox = DirectCast(c, wzTextBox)
                        cTemp.Text = Me.keyCustom(cTemp.ID)(0).ToString
                    End If
                    If c.GetType() Is GetType(wzFinestraModale) Then
                        Dim cTemp As wzFinestraModale = DirectCast(c, wzFinestraModale)
                        If cTemp.CampiChiave = "" Then
                            cTemp.Codice = Me.keyCustom(cTemp.ID)(0).ToString
                        Else
                            Dim keyFields As String() = cTemp.CampiChiave.Split("|")
                            For i As Int16 = 0 To keyFields.Length - 1
                                cTemp.ValoriAltriCampi(keyFields(i)) = Me.keyCustom(cTemp.ID)(i).ToString
                            Next
                        End If
                        cTemp.RefreshDataBind()
                    End If
                End If
            Next
        End Sub

    End Class


End Class