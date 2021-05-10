Imports Infragistics.WebUI.UltraWebToolbar
Imports Onit.Controls


Public Class OnitDataPanelUtility

#Region " Private "

    Private _btnFind As String
    Private _btnNew As String
    Private _btnCopy As String
    Private _btnEdit As String
    Private _btnDelete As String
    Private _btnSave As String
    Private _btnCancel As String
    Private _btnOther As String
    Private _imgFind As String
    Private _imgNew As String
    Private _imgCopy As String
    Private _imgEdit As String
    Private _imgDelete As String
    Private _imgSave As String
    Private _imgCancel As String
    Private _imgFolder As String

    Private _Toolbar As UltraWebToolbar

    Private _odpMaster As Onit.Controls.OnitDataPanel.OnitDataPanel
    Private _odpDetail As Onit.Controls.OnitDataPanel.OnitDataPanel

    Private _wzDataGrid As Onit.Controls.OnitDataPanel.wzDataGrid

    Private _wzRicBase As TextBox

#End Region

#Region " Properties "

#Region " Controls "

    Public Property Toolbar() As UltraWebToolbar
        Get
            Return _Toolbar
        End Get
        Set(ByVal Value As UltraWebToolbar)
            _Toolbar = Value
        End Set
    End Property

    Public Property WZDataGrid() As Onit.Controls.OnitDataPanel.wzDataGrid
        Get
            Return _wzDataGrid
        End Get
        Set(ByVal Value As Onit.Controls.OnitDataPanel.wzDataGrid)
            _wzDataGrid = Value
        End Set
    End Property

    Private _wzMsDataGrid As Onit.Controls.OnitDataPanel.wzMsDataGrid
    Public Property WZMsDataGrid() As Onit.Controls.OnitDataPanel.wzMsDataGrid
        Get
            Return _wzMsDataGrid
        End Get
        Set(ByVal Value As Onit.Controls.OnitDataPanel.wzMsDataGrid)
            _wzMsDataGrid = Value
        End Set
    End Property

    Public Property WZRicBase() As TextBox
        Get
            Return _wzRicBase
        End Get
        Set(ByVal Value As TextBox)
            _wzRicBase = Value
        End Set
    End Property

#End Region

#Region " DataPanels "

    Public Property MasterDataPanel() As Onit.Controls.OnitDataPanel.OnitDataPanel
        Get
            Return _odpMaster
        End Get
        Set(ByVal Value As Onit.Controls.OnitDataPanel.OnitDataPanel)
            _odpMaster = Value
        End Set
    End Property

    Public Property DetailDataPanel() As Onit.Controls.OnitDataPanel.OnitDataPanel
        Get
            Return _odpDetail
        End Get
        Set(ByVal Value As Onit.Controls.OnitDataPanel.OnitDataPanel)
            _odpDetail = Value
        End Set
    End Property

#End Region

#Region " Buttons "

    Public Property FindButtonName() As String
        Get
            Return _btnFind
        End Get
        Set(ByVal Value As String)
            _btnFind = Value
        End Set
    End Property

    Public Property NewButtonName() As String
        Get
            Return _btnNew
        End Get
        Set(ByVal Value As String)
            _btnNew = Value
        End Set
    End Property

    Public Property CopyButtonName() As String
        Get
            Return _btnCopy
        End Get
        Set(ByVal Value As String)
            _btnCopy = Value
        End Set
    End Property

    Public Property EditButtonName() As String
        Get
            Return _btnEdit
        End Get
        Set(ByVal Value As String)
            _btnEdit = Value
        End Set
    End Property

    Public Property DeleteButtonName() As String
        Get
            Return _btnDelete
        End Get
        Set(ByVal Value As String)
            _btnDelete = Value
        End Set
    End Property

    Public Property SaveButtonName() As String
        Get
            Return _btnSave
        End Get
        Set(ByVal Value As String)
            _btnSave = Value
        End Set
    End Property

    Public Property CancelButtonName() As String
        Get
            Return _btnCancel
        End Get
        Set(ByVal Value As String)
            _btnCancel = Value
        End Set
    End Property

    Public Property OtherButtonsNames() As String
        Get
            Return _btnOther
        End Get
        Set(ByVal Value As String)
            _btnOther = Value
        End Set
    End Property

#End Region

#Region " Images "

    Public Property FindImage() As String
        Get
            Return _imgFind
        End Get
        Set(ByVal Value As String)
            _imgFind = Value
        End Set
    End Property

    Public Property NewImage() As String
        Get
            Return _imgNew
        End Get
        Set(ByVal Value As String)
            _imgNew = Value
        End Set
    End Property

    Public Property CopyImage() As String
        Get
            Return _imgCopy
        End Get
        Set(ByVal Value As String)
            _imgCopy = Value
        End Set
    End Property

    Public Property EditImage() As String
        Get
            Return _imgEdit
        End Get
        Set(ByVal Value As String)
            _imgEdit = Value
        End Set
    End Property

    Public Property DeleteImage() As String
        Get
            Return _imgDelete
        End Get
        Set(ByVal Value As String)
            _imgDelete = Value
        End Set
    End Property

    Public Property SaveImage() As String
        Get
            Return _imgSave
        End Get
        Set(ByVal Value As String)
            _imgSave = Value
        End Set
    End Property

    Public Property CancelImage() As String
        Get
            Return _imgCancel
        End Get
        Set(ByVal Value As String)
            _imgCancel = Value
        End Set
    End Property

    Public Property ImageFolder() As String
        Get
            Return _imgFolder
        End Get
        Set(ByVal Value As String)
            _imgFolder = Value
        End Set
    End Property

#End Region

#End Region

#Region " Constructors "

    Sub New(toolBar As UltraWebToolbar)
        _Toolbar = toolBar
        _btnFind = "btnFind"
        _btnNew = "btnNew"
        _btnCopy = "btnCopy"
        _btnEdit = "btnEdit"
        _btnDelete = "btnDelete"
        _btnSave = "btnSave"
        _btnCancel = "btnCancel"
        _imgFind = VirtualPathUtility.ToAbsolute("~/images/cerca.gif")
        _imgNew = VirtualPathUtility.ToAbsolute("~/images/nuovo.gif")
        _imgCopy = VirtualPathUtility.ToAbsolute("~/images/duplica.gif")
        _imgEdit = VirtualPathUtility.ToAbsolute("~/images/modifica.gif")
        _imgDelete = VirtualPathUtility.ToAbsolute("~/images/annullaConf.gif")
        _imgSave = VirtualPathUtility.ToAbsolute("~/images/salva.gif")
        _imgCancel = VirtualPathUtility.ToAbsolute("~/images/annulla.gif")
    End Sub

#End Region

#Region " Methods "

    Public Sub InitToolbar()

        Dim cancelButton As TBarButton = _Toolbar.Items.FromKeyButton(_btnCancel)
        Dim saveButton As TBarButton = _Toolbar.Items.FromKeyButton(_btnSave)

        If Not cancelButton Is Nothing Then cancelButton.Enabled = False
        If Not saveButton Is Nothing Then saveButton.Enabled = False

    End Sub

    Public Function CheckToolBarState(operation As OnitDataPanel.OnitDataPanel.CurrentOperationTypes) As Boolean

        Dim cancelButton As TBarButton = _Toolbar.Items.FromKeyButton(_btnCancel)
        Dim copyButton As TBarButton = _Toolbar.Items.FromKeyButton(_btnCopy)
        Dim deleteButton As TBarButton = _Toolbar.Items.FromKeyButton(_btnDelete)
        Dim editButton As TBarButton = _Toolbar.Items.FromKeyButton(_btnEdit)
        Dim findButton As TBarButton = _Toolbar.Items.FromKeyButton(_btnFind)
        Dim newRecordButton As TBarButton = _Toolbar.Items.FromKeyButton(_btnNew)
        Dim saveButton As TBarButton = _Toolbar.Items.FromKeyButton(_btnSave)

        Dim buttonNames() As String = Nothing
        Dim otherButtons As TBarButton() = Nothing

        If Not _btnOther Is Nothing Then

            buttonNames = _btnOther.Split(";")
            otherButtons = Array.CreateInstance(GetType(TBarButton), buttonNames.Length)

            For i As Int16 = 0 To buttonNames.GetUpperBound(0)
                otherButtons(i) = DirectCast(_Toolbar.Items.FromKey(buttonNames(i)), TBarButton)
            Next

        End If

        Dim menuBusy As Boolean

        Select Case operation

            Case OnitDataPanel.OnitDataPanel.CurrentOperationTypes.CancelRecord

                _wzRicBase.ReadOnly = False
                _wzRicBase.CssClass = "textbox_stringa w100p"

                If Not cancelButton Is Nothing Then cancelButton.Enabled = False
                If Not saveButton Is Nothing Then saveButton.Enabled = False
                If Not findButton Is Nothing Then findButton.Enabled = True
                If Not deleteButton Is Nothing Then deleteButton.Enabled = True
                If Not copyButton Is Nothing Then copyButton.Enabled = True
                If Not findButton Is Nothing Then findButton.Enabled = True
                If Not newRecordButton Is Nothing Then newRecordButton.Enabled = True
                If Not editButton Is Nothing Then editButton.Enabled = True

                SetStatusOtherButtons(otherButtons, True)
                menuBusy = False

            Case OnitDataPanel.OnitDataPanel.CurrentOperationTypes.SaveRecord

                _wzRicBase.ReadOnly = False
                _wzRicBase.CssClass = "textbox_stringa w100p"

                If Not cancelButton Is Nothing Then cancelButton.Enabled = False
                If Not saveButton Is Nothing Then saveButton.Enabled = False
                If Not findButton Is Nothing Then findButton.Enabled = True
                If Not deleteButton Is Nothing Then deleteButton.Enabled = True
                If Not copyButton Is Nothing Then copyButton.Enabled = True
                If Not findButton Is Nothing Then findButton.Enabled = True
                If Not newRecordButton Is Nothing Then newRecordButton.Enabled = True
                If Not editButton Is Nothing Then editButton.Enabled = True

                SetStatusOtherButtons(otherButtons, True)
                menuBusy = False

            Case OnitDataPanel.OnitDataPanel.CurrentOperationTypes.Find

                If Not deleteButton Is Nothing Then deleteButton.Enabled = _odpMaster.CurrentRecord > -1
                If Not editButton Is Nothing Then editButton.Enabled = _odpMaster.CurrentRecord > -1

                SetStatusOtherButtons(otherButtons, _odpMaster.CurrentRecord > -1)
                menuBusy = False

            Case OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord

                _wzRicBase.ReadOnly = True
                _wzRicBase.CssClass = "textbox_stringa_disabilitato w100p"

                If Not cancelButton Is Nothing Then cancelButton.Enabled = True
                If Not saveButton Is Nothing Then saveButton.Enabled = True
                If Not findButton Is Nothing Then findButton.Enabled = False
                If Not deleteButton Is Nothing Then deleteButton.Enabled = False
                If Not copyButton Is Nothing Then copyButton.Enabled = False
                If Not findButton Is Nothing Then findButton.Enabled = False
                If Not newRecordButton Is Nothing Then newRecordButton.Enabled = False
                If Not editButton Is Nothing Then editButton.Enabled = False

                SetStatusOtherButtons(otherButtons, False)
                menuBusy = True

            Case OnitDataPanel.OnitDataPanel.CurrentOperationTypes.DeleteRecord

                _wzRicBase.ReadOnly = True
                _wzRicBase.CssClass = "textbox_stringa_disabilitato w100p"

                If Not cancelButton Is Nothing Then cancelButton.Enabled = True
                If Not saveButton Is Nothing Then saveButton.Enabled = True
                If Not findButton Is Nothing Then findButton.Enabled = False
                If Not deleteButton Is Nothing Then deleteButton.Enabled = False
                If Not copyButton Is Nothing Then copyButton.Enabled = False
                If Not findButton Is Nothing Then findButton.Enabled = False
                If Not newRecordButton Is Nothing Then newRecordButton.Enabled = False
                If Not editButton Is Nothing Then editButton.Enabled = False

                SetStatusOtherButtons(otherButtons, False)
                menuBusy = True

            Case OnitDataPanel.OnitDataPanel.CurrentOperationTypes.EditRecord

                _wzRicBase.ReadOnly = True
                _wzRicBase.CssClass = "textbox_stringa_disabilitato w100p"

                If Not cancelButton Is Nothing Then cancelButton.Enabled = True
                If Not saveButton Is Nothing Then saveButton.Enabled = True
                If Not findButton Is Nothing Then findButton.Enabled = False
                If Not deleteButton Is Nothing Then deleteButton.Enabled = False
                If Not copyButton Is Nothing Then copyButton.Enabled = False
                If Not findButton Is Nothing Then findButton.Enabled = False
                If Not newRecordButton Is Nothing Then newRecordButton.Enabled = False
                If Not editButton Is Nothing Then editButton.Enabled = False

                SetStatusOtherButtons(otherButtons, False)
                menuBusy = True

            Case OnitDataPanel.OnitDataPanel.CurrentOperationTypes.DuplicateRecord

                _wzRicBase.ReadOnly = True
                _wzRicBase.CssClass = "textbox_stringa_disabilitato w100p"

                If Not cancelButton Is Nothing Then cancelButton.Enabled = True
                If Not saveButton Is Nothing Then saveButton.Enabled = True
                If Not findButton Is Nothing Then findButton.Enabled = False
                If Not deleteButton Is Nothing Then deleteButton.Enabled = False
                If Not copyButton Is Nothing Then copyButton.Enabled = False
                If Not findButton Is Nothing Then findButton.Enabled = False
                If Not newRecordButton Is Nothing Then newRecordButton.Enabled = False
                If Not editButton Is Nothing Then editButton.Enabled = False

                SetStatusOtherButtons(otherButtons, False)
                menuBusy = True

        End Select

        If _odpMaster.CurrentRecord = -1 Then
            If Not deleteButton Is Nothing Then deleteButton.Enabled = False
            If Not editButton Is Nothing Then editButton.Enabled = False
            SetStatusOtherButtons(otherButtons, True)
        End If

        Return menuBusy

    End Function

    Public Sub SetStatusOtherButtons(toolBarButton As TBarButton(), status As Boolean)

        If Not toolBarButton Is Nothing Then

            For i As Int16 = 0 To toolBarButton.GetUpperBound(0)
                toolBarButton(i).Enabled = status
            Next

        End If

    End Sub

    Public Sub SetToolbarButtonImages()

        Dim cancelButton As TBarButton = _Toolbar.Items.FromKeyButton(_btnCancel)
        Dim copyButton As TBarButton = _Toolbar.Items.FromKeyButton(_btnCopy)
        Dim deleteButton As TBarButton = _Toolbar.Items.FromKeyButton(_btnDelete)
        Dim editButton As TBarButton = _Toolbar.Items.FromKeyButton(_btnEdit)
        Dim findButton As TBarButton = _Toolbar.Items.FromKeyButton(_btnFind)
        Dim newRecordButton As TBarButton = _Toolbar.Items.FromKeyButton(_btnNew)
        Dim saveButton As TBarButton = _Toolbar.Items.FromKeyButton(_btnSave)

        If Not cancelButton Is Nothing Then cancelButton.Image = _imgCancel
        If Not copyButton Is Nothing Then copyButton.Image = _imgCopy
        If Not deleteButton Is Nothing Then deleteButton.Image = _imgDelete
        If Not editButton Is Nothing Then editButton.Image = _imgEdit
        If Not findButton Is Nothing Then findButton.Image = _imgFind
        If Not newRecordButton Is Nothing Then newRecordButton.Image = _imgNew
        If Not saveButton Is Nothing Then saveButton.Image = _imgSave

    End Sub

    Public Sub ManagingToolbar(buttonKey As String)

        Select Case buttonKey

            Case _btnFind

                _odpMaster.Find()

            Case _btnDelete
                Dim e As New BeforeOperationEventArgs()

                RaiseEvent BeforeDelete(Me, e)

                If Not e.Cancel Then

                    If Not WZDataGrid Is Nothing Then
                        WZDataGrid.needSelPostBack = OnitDataPanel.wzDataGrid.needPosts.NotSet
                    ElseIf Not WZMsDataGrid Is Nothing Then
                        WZMsDataGrid.AllowSelection = True
                    End If

                    _odpMaster.DeleteRecord()

                End If

            Case _btnSave

                Dim e As New BeforeOperationEventArgs()

                RaiseEvent BeforeSave(Me, e)

                If Not e.Cancel Then

                    If Not WZDataGrid Is Nothing Then
                        WZDataGrid.needSelPostBack = OnitDataPanel.wzDataGrid.needPosts.NotSet
                    ElseIf Not WZMsDataGrid Is Nothing Then
                        WZMsDataGrid.AllowSelection = True
                    End If

                    If _odpDetail Is Nothing OrElse _odpMaster.CurrentOperation = OnitDataPanel.OnitDataPanel.CurrentOperationTypes.DeleteRecord Then
                        _odpMaster.SaveData()
                    Else
                        _odpDetail.SaveData()
                    End If

                End If

            Case _btnCancel

                If Not WZDataGrid Is Nothing Then
                    WZDataGrid.needSelPostBack = OnitDataPanel.wzDataGrid.needPosts.NotSet
                ElseIf Not WZMsDataGrid Is Nothing Then
                    WZMsDataGrid.AllowSelection = True
                End If

                If _odpDetail Is Nothing OrElse _odpMaster.CurrentOperation = OnitDataPanel.OnitDataPanel.CurrentOperationTypes.DeleteRecord Then
                    _odpMaster.CancelData()
                Else
                    _odpDetail.CancelData()
                End If

            Case _btnEdit

                Dim e As New BeforeOperationEventArgs()

                RaiseEvent BeforeEdit(Me, e)

                If Not e.Cancel Then

                    If Not WZDataGrid Is Nothing Then
                        WZDataGrid.needSelPostBack = OnitDataPanel.wzDataGrid.needPosts.lock
                    ElseIf Not WZMsDataGrid Is Nothing Then
                        WZMsDataGrid.AllowSelection = False
                    End If

                    If Not _odpDetail Is Nothing Then
                        _odpDetail.EditRecord()
                    Else
                        _odpMaster.EditRecord()
                    End If

                End If

            Case _btnNew

                    Dim e As New BeforeOperationEventArgs()

                    RaiseEvent BeforeNew(Me, e)

                    If Not e.Cancel Then

                    If Not WZDataGrid Is Nothing Then
                        WZDataGrid.needSelPostBack = OnitDataPanel.wzDataGrid.needPosts.lock
                    ElseIf Not WZMsDataGrid Is Nothing Then
                        WZMsDataGrid.AllowSelection = False
                    End If

                        If Not _odpDetail Is Nothing Then
                            _odpDetail.NewRecord(False)
                        Else
                            _odpMaster.NewRecord(False)
                        End If

                    End If

            Case _btnCopy

                    Dim e As New BeforeOperationEventArgs()

                    RaiseEvent BeforeCopy(Me, e)

                    If Not e.Cancel Then

                        If Not _odpDetail Is Nothing Then
                            _odpDetail.NewRecord(True)
                        Else
                            _odpMaster.NewRecord(True)
                        End If

                    End If

        End Select
    End Sub

    Public Sub Find(keyFieldName As String)

        Dim dtResult As DataTable
        Dim pkey As String = (Me.MasterDataPanel.getCurrentDataRow())(keyFieldName).ToString()

        Me.MasterDataPanel.Find()

        ' Ripristino della riga selezionata precedente al find
        dtResult = Me.MasterDataPanel.getCurrentDataTable()
        If Not dtResult Is Nothing Then
            For i As Integer = 0 To dtResult.Rows.Count - 1
                If dtResult.Rows(i)(keyFieldName) = pkey Then
                    Me.MasterDataPanel.MoveRecord(Onit.Controls.OnitDataPanel.OnitDataPanel.RecordMoveTo.NextRecord, i)
                    Exit For
                End If
            Next
        End If

    End Sub

#End Region

#Region " Events "

    Public Class BeforeOperationEventArgs
        Inherits EventArgs

        Public Property Cancel As Boolean

    End Class

    Public Event BeforeNew(sender As Object, beforeNewEventArgs As BeforeOperationEventArgs)

    Public Event BeforeEdit(sender As Object, beforeEditEventArgs As BeforeOperationEventArgs)

    Public Event BeforeCopy(sender As Object, beforeCopyEventArgs As BeforeOperationEventArgs)

    Public Event BeforeSave(sender As Object, beforeSaveEventArgs As BeforeOperationEventArgs)

    Public Event BeforeDelete(sender As Object, beforeDeleteEventArgs As BeforeOperationEventArgs)

#End Region

End Class
