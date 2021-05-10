Imports System.Text

Public Class Renderator

    Public Enum Type
        table
    End Enum

    Public Function getError(ByRef ex As Exception) As DataTable

        Dim dt As New DataTable
        Dim dr As DataRow

        dr = dt.NewRow

        dr(0) = ex.Message

        dr.AcceptChanges()
        dt.Rows.Add(dr)

        Return dt

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''<asp:datagrid id="dgrAmbAppuntamento" runat="server" AutoGenerateColumns="False" width="100%"  CellPadding="1" GridLines="None">
    '''<SelectedItemStyle Font-Bold="True" CssClass="selected"></SelectedItemStyle>
    '''<AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
    '''<ItemStyle CssClass="item"></ItemStyle>
    '''<HeaderStyle CssClass="header"></HeaderStyle>
    '''<Columns>
    '''   <asp:ButtonColumn ButtonType="LinkButton" CommandName="Select" Visible="true"></asp:ButtonColumn>
    '''	<asp:BoundColumn DataField="AMB_DESCRIZIONE" HeaderText="Descrizione">
    '''		<HeaderStyle HorizontalAlign="Center" Width="200px"></HeaderStyle>
    '''		<ItemStyle HorizontalAlign="Left"></ItemStyle>
    '''	</asp:BoundColumn>
    '''	<asp:BoundColumn DataField="AMB_CODICE" HeaderText="Codice">
    '''		<HeaderStyle HorizontalAlign="Center" Width="100px"></HeaderStyle>
    '''		<ItemStyle HorizontalAlign="Left"></ItemStyle>
    '''	</asp:BoundColumn>
    '''</Columns>
    '''<PagerStyle CssClass="pager" Mode="NumericPages"></PagerStyle>
    '''</asp:datagrid>
    ''' </summary>
    ''' <param name="DataSource"></param>
    ''' <param name="type"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adesimone]	27/02/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function ExportToHtml(ByVal DataSource As Object, Optional ByVal type As Type = Type.table) As String

        Dim dtGrid As New System.Web.UI.WebControls.DataGrid

        Return ExportToHtml(dtGrid, DataSource, type)

    End Function

    Public Function ExportToHtml(ByVal dtGrid As DataGrid, ByVal DataSource As Object, Optional ByVal type As Type = Type.table) As String

        Dim dt As New DataTable
        Try
            dt = CType(DataSource, DataTable).Copy
        Catch ex As Exception
            dt = getError(ex)
        End Try

        ' dt.TableName = "Nome Tabella"
        Dim SB As New System.Text.StringBuilder
        Dim SW As New System.IO.StringWriter(SB)
        Dim htmlTW As New System.Web.UI.HtmlTextWriter(SW)
        Dim btnColumn As New ButtonColumn

        With dtGrid
            .Width = System.Web.UI.WebControls.Unit.Percentage(100)
            .BorderWidth = System.Web.UI.WebControls.Unit.Point(0)
            .SelectedItemStyle.Font.Bold = True
            .SelectedItemStyle.CssClass = "selected"
            .AlternatingItemStyle.CssClass = "alternating"
            .ItemStyle.CssClass = "item"
            .HeaderStyle.CssClass = "header"
            .PagerStyle.CssClass = "pager"
            .PagerStyle.Mode = PagerMode.NumericPages
            .ShowHeader = False

            btnColumn.ButtonType = ButtonColumnType.LinkButton
            btnColumn.CommandName = "Select"
            btnColumn.Visible = True

            .Columns.Add(btnColumn)

            .DataSource = dt
            .DataBind()
            .RenderControl(htmlTW)

        End With

        Return SB.ToString()

    End Function

    Public Function ExportToHtml(ByVal idr As System.Data.IDataReader, Optional ByVal type As Type = Type.table) As String

        Dim stb As New StringBuilder("")
        Dim j As Integer
        Dim colName As String
        Dim cellVal As String
        Dim toggle As Boolean = False

        stb.Append("<table class='datagrid' >")
        stb.Append("<tr class='header'>")

        For j = 0 To idr.FieldCount - 1
            colName = idr.GetName(j)
            colName = colName.Replace("_", " ")
            stb.Append(String.Format("<td class=""label_left"" style=""FONT-WEIGHT: bold"">{0}</td>", colName))

        Next

        While idr.Read

            If toggle Then
                stb.Append("<tr class='item'>")
            Else
                stb.Append("<tr class='alternating'>")
            End If


            For j = 0 To idr.FieldCount - 1
                cellVal = ""
                If Not idr(j) Is System.DBNull.Value Then
                    Try
                        cellVal = CStr(idr(j))
                    Catch ex As Exception
                        ''' nothing
                    End Try
                End If
                cellVal = cellVal.Replace("_", " ")
                stb.Append(String.Format("<td>{0}</td>", cellVal))
            Next

            stb.Append("</tr>")

            If toggle Then
                toggle = False
            Else
                toggle = True
            End If

        End While

        Return stb.ToString

    End Function
End Class
