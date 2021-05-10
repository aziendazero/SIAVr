<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="UtentiConsultori.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.UtentiConsultori" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<%@ Register Src="../../../Common/Controls/FiltroRicercaImmediata.ascx" TagName="FiltroRicercaImmediata" TagPrefix="uc1" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Associazione Utenti - Centri Vaccinali</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    
    <style type="text/css">
        .filtri-ricerca {
            margin: 2px 0px 0px 2px; 
            background-color: whitesmoke; 
            border: 1px solid navy; 
            padding: 2px;
            height: 21px;
        }
    </style>
    <script type="text/javascript" src="<%= Me.ResolveClientUrl("~/common/scripts/onvac.common.js") %>"></script>

    <script type="text/javascript" language="javascript">

        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }
        
        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;
        }

        function flagDefaultChanged(clientIdChkConsultorio, rowSelectedIndex) {
            var hdIndexFlagDefault = document.getElementById('hdIndexFlagDefault');
            if (hdIndexFlagDefault != null) hdIndexFlagDefault.value = rowSelectedIndex;

            var chkConsultorio = document.getElementById(clientIdChkConsultorio);
            if (chkConsultorio != null) chkConsultorio.checked = true;

            return true;
        }

        function setChkAllVisibility() {
            var chkSelDeselAll = document.getElementById("chkSelDeselAll");

            if (<%= (Me.StatoPagina = PageStatus.Edit).ToString().ToLower() %>) {
                chkSelDeselAll.disabled = false;
            } else {
                chkSelDeselAll.disabled = true;
            }
        }
    </script>
</head>
<body onload="setChkAllVisibility()">
    <form id="form1" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Height="100%" Width="100%" TitleCssClass="Title3" Titolo="Associazione Utenti - Centri Vaccinali" >
			<div class="title" id="PanelTitolo" runat="server" style="width: 100%;">
			    <asp:Label id="LayoutTitolo" runat="server">&nbsp;Associazione Utenti - Centri Vaccinali</asp:Label>
			</div>
            <div>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
		            <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
		            <Items>
		                <igtbar:TBarButton Key="btnModifica" Text="Modifica" DisabledImage="../../../images/modifica_dis.gif" Image="../../../images/modifica.gif">
		                </igtbar:TBarButton>
                        <igtbar:TBSeparator />
						<igtbar:TBarButton Key="btnSalva" Text="Salva" DisabledImage="../../../images/salva_dis.gif" Image="../../../images/salva.gif">
						</igtbar:TBarButton>
						<igtbar:TBarButton Key="btnAnnulla" Text="Annulla" DisabledImage="../../../images/annulla_dis.gif" Image="../../../images/annulla.gif">
						</igtbar:TBarButton>
                    </Items>
                </igtbar:UltraWebToolbar>
            </div>

            <dyp:DynamicPanel ID="dypRiga" runat="server" Width="100%" Height="50%" ExpandDirection="horizontal">
                
                <dyp:DynamicPanel ID="dypLeft" runat="server" Width="50%" Height="100%">
                    <div class="vac-sezione" style="margin-top: 2px; margin-left: 2px;">UTENTI</div>
                    <div class="filtri-ricerca">
                        <uc1:FiltroRicercaImmediata id="ucFiltroRicercaUtenti" runat="server"></uc1:FiltroRicercaImmediata>
                    </div>
                    <dyp:DynamicPanel ID="dypScrollLeft" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                        <div style="width:100%; margin-left: 2px;">
                            <asp:DataGrid id="dgrUtenti" runat="server" Width="100%" AutoGenerateColumns="False" 
                                AllowCustomPaging="true" AllowPaging="true" PageSize="25">
					            <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
					            <ItemStyle CssClass="item"></ItemStyle>
					            <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
                                <HeaderStyle CssClass="header"></HeaderStyle>
					            <PagerStyle CssClass="pager" Position="TopAndBottom" Mode="NumericPages" />
					            <Columns>
					                <on_dgr:SelectorColumn>
					                    <ItemStyle HorizontalAlign="Center" Width="2%" />
					                </on_dgr:SelectorColumn>
					                <asp:BoundColumn Visible="False" DataField="Id" HeaderText="idUtente"></asp:BoundColumn>
                                    <asp:BoundColumn Visible="True" DataField="Codice" HeaderText="Codice" HeaderStyle-Width="20%"></asp:BoundColumn>
                                    <asp:BoundColumn Visible="True" DataField="Descrizione" HeaderText="Descrizione" HeaderStyle-Width="30%"></asp:BoundColumn>
                                    <asp:BoundColumn Visible="True" DataField="Cognome" HeaderText="Cognome" HeaderStyle-Width="25%"></asp:BoundColumn>
                                    <asp:BoundColumn Visible="True" DataField="Nome" HeaderText="Nome" HeaderStyle-Width="25%"></asp:BoundColumn>
                                </Columns>
                            </asp:DataGrid>                                    
                        </div>
                    </dyp:DynamicPanel>
                </dyp:DynamicPanel>

                <dyp:DynamicPanel ID="dypRight" runat="server" Width="50%" Height="100%">
                    <div class="vac-sezione" style="margin-top: 2px; margin-left: 2px; margin-right: 2px">CENTRI VACCINALI</div>
                    <div class="filtri-ricerca">
                        <table width="100%" border="0" cellpadding="0" cellspacing="0">
                            <colgroup>
                                <col width="10%" />
                                <col width="35%" />
                                <col width="10%" />
                                <col width="38%" />
                                <col width="7%" />
                            </colgroup>
                            <tr>
                                <td style="text-align: right; padding-right:1px;">
                                    <asp:Label runat="server" ID="lblUls" CssClass="Label" Text="Ulss"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlUls" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlUls_SelectedIndexChanged" Width="100%"></asp:DropDownList>
                                </td>
                                <td style="text-align: right; padding-right:1px;">
                                    <asp:Label runat="server" ID="lbDistretto" Text="Distretto" CssClass="Label"></asp:Label>
                                </td>
                                <td colspan="2">
                                    <asp:DropDownList ID="ddlDistretto" runat="server" Width="100%" AutoPostBack="true" OnSelectedIndexChanged="ddlDistretto_SelectedIndexChanged"></asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <dyp:DynamicPanel ID="dypScrollRight" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                        <div style="margin-left: 2px; width:100%; margin-right: 2px; margin-top: 1px;">
                            <asp:HiddenField ID="hdIndexFlagDefault" runat="server" />
                            <asp:DataGrid id="dgrConsultori" runat="server" Width="100%" AutoGenerateColumns="False" >
					            <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
					            <ItemStyle CssClass="item"></ItemStyle>
					            <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
                                <HeaderStyle CssClass="header"></HeaderStyle>
					            <PagerStyle CssClass="pager" Position="TopAndBottom" Mode="NumericPages" />
					            <Columns>
                                    <asp:TemplateColumn>
                                        <HeaderStyle Width="2%" HorizontalAlign="Center" />
                                        <HeaderTemplate>
                                            <input type="checkbox" id="chkSelDeselAll" onclick="selezionaTutti(this, 'dgrConsultori', 0)" />
                                        </HeaderTemplate>
                                        <ItemStyle Width="2%" HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkConsultorio" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:BoundColumn Visible="True" DataField="DescrizioneConsultorio" HeaderText="Descrizione" HeaderStyle-Width="45%" ></asp:BoundColumn>
                                    <asp:BoundColumn Visible="True" DataField="CodiceConsultorio" HeaderText="Codice" HeaderStyle-Width="33%" ></asp:BoundColumn>
                                    <asp:TemplateColumn HeaderText="Default">
                                        <HeaderStyle Width="20%" HorizontalAlign="Center" />
                                        <ItemStyle Width="20%" HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <asp:Image ID="imgDefault" runat="server" ImageUrl="../../../images/success.png" />
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:BoundColumn Visible="False" DataField="ConsultorioDefault" ></asp:BoundColumn>
                                </Columns>
                            </asp:DataGrid>
                        </div>
                    </dyp:DynamicPanel>
                </dyp:DynamicPanel>
            </dyp:DynamicPanel>
        </on_lay3:OnitLayout3>
    </form>
</body>
</html>
