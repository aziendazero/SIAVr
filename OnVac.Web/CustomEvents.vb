Imports System.Web.UI

Public MustInherit Class CustomEvent
    Inherits Control
    Implements IPostBackEventHandler

    Protected Event EventoScatenato(ByVal eventName As String, ByVal eventObj As Object)

    Public Sub New(ByVal id As String, ByVal p As Control)
        Me.ID = id
        If Not p Is Nothing Then
            p.Controls.Add(Me)
        End If
    End Sub

    Sub InsertInPage(ByVal c As Control)
        c.Controls.Add(Me)
    End Sub

    Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent
        Dim evt As Object
        Dim pars() As String = eventArgument.Split("|")
        Dim val(0) As Object
        Dim prop As Type
        Dim i As Int16
        evt = Activator.CreateInstance(Type.GetType(pars(1), True, False))
        For i = 2 To pars.GetUpperBound(0)
            prop = evt.GetType.GetField(pars(i).Split(":")(0)).FieldType
            val(0) = Convert.ChangeType(pars(i).Split(":")(1), prop)
            evt.GetType.InvokeMember(pars(i).Split(":")(0), Reflection.BindingFlags.SetField, Nothing, evt, val)
        Next
        RaiseEvent EventoScatenato(pars(0), evt)
    End Sub

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        writer.Write("<SPAN id=" & Me.UniqueID & " name=" & Me.UniqueID & " runat=server></SPAN>")
    End Sub

    Private Sub EventiModConv_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
        Page.RegisterRequiresRaiseEvent(Me)
    End Sub
End Class

Public Class CustomEventUtility
    Public Const START_SCRIPT = "<script language='javascript'>" & vbCrLf & "<!--" & vbCrLf
    Public Const END_SCRIPT = vbCrLf & "//-->" & vbCrLf & "</script>"

    Public Shared Function GetRefEvent(ByVal pag As Page, ByVal evtobj As CustomEvent, ByVal struct As Object, ByVal eventName As String) As String
        Dim pval As Reflection.FieldInfo() = struct.GetType.GetFields
        Dim p As Reflection.FieldInfo
        Dim str As String
        str = eventName & "|" & struct.GetType.FullName & "|"

        Dim stb As New System.Text.StringBuilder
        For Each p In pval
            'str &= p.Name & ":" & p.GetValue(struct) & "|"
            stb.AppendFormat("{0}:{1}|", p.Name, p.GetValue(struct))
        Next
        str &= stb.ToString

        Return pag.GetPostBackClientEvent(evtobj, Microsoft.VisualBasic.Mid(str, 1, str.Length - 1))
    End Function

    Public Shared Sub HandleEventsToMessage(ByVal p As Page, ByVal eventName As String, ByVal strScript As String)
        Dim strs As String
        strs = START_SCRIPT & vbCrLf & strScript & END_SCRIPT
        p.RegisterStartupScript(eventName, strs)
    End Sub
End Class