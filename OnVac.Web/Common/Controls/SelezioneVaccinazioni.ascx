<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="SelezioneVaccinazioni.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.SelezioneVaccinazioni" %>

<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>

<table width="100%" border="0" style="table-layout: fixed;">
    <tr>
        <td width="26px" align="right" runat="server" id="tdBtnSelezione">
            <asp:ImageButton ID="btnSelezionaVac" runat="server" OnClick="btnSelezionaVac_Click" AlternateText="Selezione delle vaccinazioni associate" />
        </td>
        <td style="height: 22px; border: navy 1px solid;" valign="top" bgcolor="gainsboro"
            runat="server" id="tdLblVaccinazioni">
            <asp:Label ID="lblVaccinazioni" Style="font-size: 10px; text-transform: uppercase;
                font-style: italic; font-family: Verdana" runat="server" Width="98%" CssClass="TextBox_Stringa"></asp:Label>
        </td>
    </tr>
</table>
<on_ofm:OnitFinestraModale ID="fmVaccinazioni" Title="Vaccinazioni" runat="server"  Width="400px" BackColor="LightGray" UseDefaultTab="True" RenderModalNotVisible="false" NoRenderX="true">
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default">
                    </DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover">
                    </HoverStyle>
                    <SelectedStyle CssClass="infratoolbar_button_selected">
                    </SelectedStyle>
                    <Items>
                        <igtbar:TBarButton Key="btn_Conferma" DisabledImage="~/Images/conferma_dis.gif"
                            Text="Conferma" Image="~/Images/conferma.gif">
                        </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btn_Annulla" DisabledImage="~/Images/annullaconf_dis.gif"
                            Text="Annulla" Image="~/Images/annullaconf.gif">
                        </igtbar:TBarButton>
                    </Items>
                </igtbar:UltraWebToolbar>
            </td>
        </tr>
        <tr>
            <td>
                <div style="width: 100%; height: 400px; overflow: auto">
                    <asp:DataGrid ID="dgrVaccinazioni" runat="server" Width="100%" AutoGenerateColumns="False"
                        CellPadding="1" GridLines="None">
                        <SelectedItemStyle Font-Bold="True" CssClass="selected"></SelectedItemStyle>
                        <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
                        <ItemStyle CssClass="item"></ItemStyle>
                        <HeaderStyle CssClass="header"></HeaderStyle>
                        <Columns>
                            <asp:TemplateColumn>
                                <HeaderStyle Width="20px"></HeaderStyle>
                                <ItemTemplate>
                                    <asp:CheckBox ID="cb" runat="server"></asp:CheckBox>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:BoundColumn DataField="vac_descrizione" HeaderText="Descrizione">
                                <HeaderStyle HorizontalAlign="Center" Width="200px"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Left"></ItemStyle>
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="vac_codice" HeaderText="Codice">
                                <HeaderStyle HorizontalAlign="Center" Width="100px"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Left"></ItemStyle>
                            </asp:BoundColumn>
                            <asp:TemplateColumn>
                                <HeaderStyle HorizontalAlign="Center" Width="30px"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:TemplateColumn>
                            <asp:BoundColumn></asp:BoundColumn>
                        </Columns>
                        <PagerStyle CssClass="pager" Mode="NumericPages"></PagerStyle>
                    </asp:DataGrid>
                </div>
            </td>
        </tr>
    </table>
</on_ofm:OnitFinestraModale>
