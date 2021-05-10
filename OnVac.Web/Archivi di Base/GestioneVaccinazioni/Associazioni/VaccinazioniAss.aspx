<%@ Page Language="vb" AutoEventWireup="false" Codebehind="VaccinazioniAss.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_VaccinazioniAss" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title>VaccinazioniAss</title>
		
        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

		<script type="text/javascript" language="javascript">
		    function InizializzaToolBar(t) {
		        t.PostBackButton = false;
		    }
		
		    function ToolBarClick(ToolBar, button, evnt) {
		        evnt.needPostBack = true;
		        switch (button.Key) {
		            case 'btn_Annulla':
		                if ("<%response.write(onitlayout31.busy)%>" == "True") {
		                    var ret;
		                    ret = confirm("Le modifiche effettuate andranno perse. Continuare?");
		                    if (ret == true) {
		                        window.location.href = 'Associazioni.aspx';
		                        evnt.needPostBack = false;
		                    }
		                }
		                else
		                    window.location.href = 'Associazioni.aspx';
		                evnt.needPostBack = false;
		                break;

		            case 'btn_Salva':
		                if ("<%response.write(onitlayout31.busy)%>" != "True")
		                    evnt.needPostBack = false;
		                break;
		        }
		    }
			
		    function controlla(evt) {
		        el = SourceElement(evt);
		        riga = el.parentNode.parentNode.parentNode;
		        tab = GetElementByTag(riga, 'TABLE', 1, 0, false);
		        cell = tab.rows[riga.rowIndex].cells[1];
		        el = GetElementByTag(cell, 'INPUT', 1, 1, false);
		        if (el.value == "") {
		            alert('I campi Descrizione-Codice sono vuoti.Non è possibile aggiornare la tabella.');
		            el.focus();
		            StopPreventDefault(evt);
		        }
		    }			
		</script>
	</head>
	<body>
	    <form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" height="100%" width="100%">
				<div class="title">
    				<asp:Label id="LayoutTitolo" runat="server" Width="100%" ></asp:Label>
				</div>
                <div>
		            <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
	                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
			            <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
			            <Items>
				            <igtbar:TBarButton Key="btn_Salva" Text="Salva" Image="~/Images/salva.gif"></igtbar:TBarButton>
				            <igtbar:TBarButton Key="btn_Annulla" Text="Annulla" Image="~/Images/annulla.gif"></igtbar:TBarButton>
				            <igtbar:TBarButton Key="btn_Inserisci" Text="Inserisci" Image="~/Images/nuovo.gif"></igtbar:TBarButton>
			            </Items>
		            </igtbar:UltraWebToolbar>
                </div>
		        <div class="sezione">
				    <asp:Label id="LayoutTitolo_sezioneCnv" runat="server" Width="100%">ELENCO VACCINAZIONI</asp:Label>
				</div>
					
                <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto">
				    <asp:Panel id="pan_vac" runat="server" Width="100%">
					    <asp:DataGrid id="dg_vac" runat="server" Width="100%" AutoGenerateColumns="False" CellPadding="1" GridLines="None">
						    <SelectedItemStyle Font-Bold="True" CssClass="Selected"></SelectedItemStyle>
						    <EditItemStyle CssClass="edit"></EditItemStyle>
						    <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
						    <ItemStyle CssClass="item"></ItemStyle>
						    <HeaderStyle Font-Bold="True" CssClass="header"></HeaderStyle>
						    <PagerStyle CssClass="pager" Mode="NumericPages"></PagerStyle>
						    <Columns>
							    <asp:EditCommandColumn ButtonType="LinkButton" UpdateText="&lt;img  title=&quot;Conferma&quot; src=&quot;../../../images/conferma.gif&quot; onclick=&quot;controlla(event)&quot;&gt;" CancelText="&lt;img  title=&quot;Annulla&quot; src=&quot;../../../images/annullaconf.gif&quot; &gt;" EditText="&lt;img  title=&quot;Elimina&quot; src=&quot;../../../images/elimina.gif&quot;  onclick=&quot;if(!confirm('La riga sar&#224; eliminata. Continuare?')){event.returnValue=false}&quot;&gt;">
								    <HeaderStyle Width="1%"></HeaderStyle>
								    <ItemStyle HorizontalAlign="Center"></ItemStyle>
							    </asp:EditCommandColumn>
							    <asp:TemplateColumn>
								    <HeaderStyle HorizontalAlign="Center" Width="49%"></HeaderStyle>
								    <ItemStyle HorizontalAlign="Left"></ItemStyle>
								    <HeaderTemplate>
									    <table cellspacing="0" cellpadding="0" width="100%" border="0">
										    <tr>
											    <td align="center" width="70%">Descrizione</td>
											    <td align="center" width="30%">Codice</td>
										    </tr>
									    </table>
								    </HeaderTemplate>
								    <ItemTemplate>
									    <table cellspacing="0" cellpadding="0" width="100%" border="0">
										    <tr>
											    <td width="70%">
												    <asp:Label id=tb_descVac runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("vac_descrizione") %>'>
												    </asp:Label></td>
											    <td width="30%">
												    <asp:Label id=tb_codVac runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("vac_codice") %>'>
												    </asp:Label></td>
										    </tr>
									    </table>
								    </ItemTemplate>
								    <EditItemTemplate>
									    <on_ofm:onitmodallist id=fm_vac_edit runat="server" Width="70%" RaiseChangeEvent="False" Codice='<%# DataBinder.Eval(Container, "DataItem")("vac_codice") %>' Descrizione='<%# DataBinder.Eval(Container, "DataItem")("vac_descrizione") %>' SetUpperCase="True" Obbligatorio="True" UseCode="True" Tabella="t_ana_vaccinazioni" Connection="" CampoDescrizione="vac_descrizione" CampoCodice="vac_codice" CodiceWidth="30%" LabelWidth="-1px" PosizionamentoFacile="False">
									    </on_ofm:onitmodallist>
								    </EditItemTemplate>
							    </asp:TemplateColumn>
							    <asp:TemplateColumn>
								    <HeaderStyle Width="50%"></HeaderStyle>
							    </asp:TemplateColumn>
						    </Columns>
					    </asp:DataGrid>
				    </asp:Panel>
                </dyp:DynamicPanel>
            </on_lay3:onitlayout3>					
	    </form>
		<%response.write(strJS)%>
	</body>
</html>