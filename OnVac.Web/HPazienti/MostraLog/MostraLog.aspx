<%@ Page Language="vb" AutoEventWireup="false" Codebehind="MostraLog.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.MostraLog"%>

<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>MostraLog</title>
		
        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

		<style type="text/css">
		    .dgr { width: 100% }
            .dgr2 { border-right: gray 1px solid; border-top: gray 1px solid; border-left: gray 1px solid; width: 70%; border-bottom: gray 1px solid; border-collapse: collapse }
            .dgr TD { font-size: 12px; font-family: Tahoma }
            .dgr2 TD { font-size: 10px }
            .r1 { background-color: whitesmoke }
            .r2 { background-color: #e7e7ff }
            .h1 { font-weight: bold; color: #4a3c8c; border-top-style: none; border-right-style: none; border-left-style: none; background-color: lightsteelblue; border-bottom-style: none }
            .cExp { padding-right: 2px; padding-left: 2px; font-weight: bold; padding-bottom: 2px; color: white; padding-top: 2px; background-color: #4a3c8c }
		</style>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout" runat="server" WindowNoFrames="False" width="100%" height="100%" TitleCssClass="Title3" HeightTitle="90px" Titolo="Log Paziente">

                <div class="Title" id="divLayoutTitolo" style="width: 100%">
				    <asp:Label id="LayoutTitolo" runat="server" CssClass="title"></asp:Label>
                </div>

                <div>
					<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" CssClass="infratoolbar" ItemWidthDefault="80px">
					    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                        <HoverStyle CssClass="infratoolbar_button_hover" ></HoverStyle>
                        <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
						<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
						<Items>
							<igtbar:TBarButton Key="Filtra" Text="Cerca" DisabledImage="~/Images/cerca.gif" Image="~/Images/cerca.gif">
							</igtbar:TBarButton>
							<igtbar:TBarButton Key="Pulisci" Text="Pulisci Filtri" DisabledImage="~/Images/pulisci.gif" Image="~/Images/pulisci.gif">
								<DefaultStyle Width="100px" CssClass="infratoolbar_button_default"></DefaultStyle>
							</igtbar:TBarButton>
						</Items>
					</igtbar:UltraWebToolbar>
                </div>

                <div class="sezione">Filtri di ricerca</div>

				<dyp:DynamicPanel ID="dypScroll1" runat="server" Width="100%" Height="100px" ScrollBars="Auto" RememberScrollPosition="true">
					<table class="datagrid" id="Table1" cellspacing="0" cellpadding="2" width="100%" border="0">
						<tr>
							<td colspan="5">
								<table id="Table4" cellspacing="0" cellpadding="0" width="50%" border="0">
									<tr>
										<td style="font-weight: bold" width="16%">Da data:</td>
										<td style="padding-left: 7px">
											<on_val:OnitDatePick id="txtDaData" runat="server" height="18px" DateBox="True"></on_val:OnitDatePick></td>
										<td style="font-weight: bold" width="16%">A data:</td>
										<td>
											<on_val:OnitDatePick id="txtAdata" runat="server" height="18px" DateBox="True"></on_val:OnitDatePick></td>
									</tr>
								</table>
							</td>
						</tr>
						<tr>
							<td style="font-weight: bold" width="8%">Operazioni:</td>
							<td colspan="3">
								<asp:CheckBoxList id="chkOperazioni" style="table-layout: fixed" runat="server" RepeatColumns="5"
									Width="100%" CssClass="label_left"></asp:CheckBoxList></td>
						</tr>
						<tr>
							<td style="font-weight: bold; vertical-align:top; padding-top:10px">Argomenti:</td>
							<td style="border-top: gainsboro 2px solid" colspan="3">
								<asp:CheckBoxList id="chkArgomenti" style="table-layout: fixed" runat="server" RepeatColumns="3" Width="100%"
									CssClass="label_left"></asp:CheckBoxList></td>
						</tr>
					</table>
                </dyp:DynamicPanel>

                <div>
					<table class="sezione" id="Table3" cellspacing="0" cellpadding="0" width="100%" border="0">
						<tr>
							<td>Legenda:</td>
							<td align="left">
								<p>
                                    <img alt="" src="../../images/op_log.gif" align="absMiddle" />&nbsp;Log generico&nbsp;&nbsp; 
                                    <img alt="" src="../../images/op_eliminazione.gif" align="absMiddle" />&nbsp;Eliminazione&nbsp;&nbsp; 
                                    <img alt="" src="../../images/op_inserimento.gif" align="absMiddle" />&nbsp;Inserimento&nbsp;&nbsp;
									<img alt="" src="../../images/op_modifica.gif" align="absMiddle" />&nbsp;Modifica&nbsp;&nbsp;
									<img alt="" src="../../images/op_eccezione.gif" align="absMiddle" />&nbsp;Eccezione
                                </p>
							</td>
							<td style="border-bottom: 1px solid" align="right">
								<asp:CheckBox id="chkEsteso" runat="server" Text="Modalità estesa" AutoPostBack="True"></asp:CheckBox></td>
						</tr>
					</table>
                </div>

                <dyp:DynamicPanel ID="dypScroll2" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                
					<asp:DataGrid id="dgrArgomento" runat="server" CssClass="dgr" AutoGenerateColumns="False" ShowHeader="False" GridLines="None">
						<AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
						<ItemStyle BackColor="#E7E7FF"></ItemStyle>
						<HeaderStyle Font-Bold="True" ForeColor="White" BackColor="#4A3C8C"></HeaderStyle>
						<Columns>
							<asp:TemplateColumn>
								<ItemTemplate>
									<table id="Table2" style="table-layout: fixed" cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr>
											<td class="cExp" style='width:100%;'>
												<span style='width:20PX;text-align:center'>
                                                    <img id="imgEspandi1" style="cursor: hand" alt="" src="../../images/piu.gif" runat="server" />
                                                </span>
												<asp:Label id="lblArgomento" style="display: none" runat="server" Text='<%# Container.DataItem("LOA_CODICE").ToString %>'>
												</asp:Label>
												<asp:Label id="Label1" runat="server" Text='<%# Container.DataItem("LOA_DESCRIZIONE").ToString %>'>
												</asp:Label>
												<!--
												&nbsp;( Criticità:
												<asp:Label id=Label2 runat="server" Text='<%# Container.DataItem("LOA_CRITICITA").ToString %>'>
												</asp:Label>&nbsp;)
												-->
											</td>
										</tr>
										<tr>
											<td>
												<asp:DataGrid id="dgrTestate" style='display:none;Width:100%' runat="server" AutoGenerateColumns="False"
													ShowHeader="False" GridLines="None">
													<AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
													<ItemStyle BackColor="#E7E7FF" Width="100%"></ItemStyle>
													<HeaderStyle Font-Bold="True" ForeColor="White" BackColor="#4A3C8C"></HeaderStyle>
													<Columns>
														<asp:TemplateColumn>
															<ItemTemplate>
																<table id="Table1" style="TABLE-LAYOUT: fixed" cellspacing="0" cellpadding="0" width="100%" border="0">
																	<tr>
																		<td width="20">
																			<asp:Label id="lblCodice" style="display: none" runat="server" Text='<%# Container.DataItem("LOT_CODICE") %>'>
																			</asp:Label></td>
																		<td width="15">
                                                                            <img id="imgEspandi2" style="cursor: hand" alt="" src="../../images/piu.gif" runat="server" /></td>
																		<td width="20">
																			<asp:Image id="imgOperazione" runat="server" AlternateText='<%# Container.DataItem("LOT_OPERAZIONE").ToString()%>' ImageUrl='<%# "../../images/" &amp; GetImageOperazion(Container.DataItem("LOT_OPERAZIONE"))%>'>
																			</asp:Image>&nbsp;</td>
																		<td width="15%">
																			<asp:Label id="lblDataOperazione" runat="server" Text='<%# Container.DataItem("LOT_DATA_OPERAZIONE") %>'>
																			</asp:Label></td>
																		<td width="20%">
																			<asp:Label id="lblUtente" runat="server" Text='<%# Container.DataItem("UTE_DESCRIZIONE") %>'>
																			</asp:Label></td>
																		<td width="20%">
																			<asp:Label id="lblMaschera" runat="server" Text='<%# Container.DataItem("LOT_MASCHERA") %>'>
																			</asp:Label></td>
																		<td width="20%">
																			<asp:Label id="lblPaziente" runat="server" Text='<%# Container.DataItem("PAZ_COGNOME") &amp; " " &amp; Container.DataItem("PAZ_NOME") %>' Visible="<%# chkEsteso.Checked %>">
																			</asp:Label></td>
																		<td width="20%">
																			<asp:Label id="lblStack" runat="server" Text='<%# Container.DataItem("LOT_STACK") %>' Visible="<%# chkEsteso.Checked %>">
																			</asp:Label></td>
																		<td width="20">
																			<asp:Image id="imgAuto" runat="server" AlternateText='<%# iif(Container.DataItem("LOT_AUTOMATICO")="S","Automatico","Manuale")%>' ImageUrl='<%# iif(Container.DataItem("LOT_AUTOMATICO")="S","../../images/operazione_automatica.gif","../../images/operazione_manuale.gif")%>'>
																			</asp:Image>&nbsp;</td>
																		<td></td>
																	</tr>
																	<tr>
																		<td></td>
																		<td></td>
																		<td colspan="8">
																			<asp:DataList id="dgrRecords" style="display: none;width:100%" runat="server" RepeatColumns="2" ShowHeader="False" ShowFooter="False">
																				<ItemStyle></ItemStyle>
																				<ItemTemplate>
																					<asp:Label id="lblCodice" style="display: none" runat="server" Text='<%# Container.DataItem("LOR_RECORD") %>'>
																					</asp:Label>
																					<asp:Label id="lblTipo" style="display: none" runat="server" Text='<%# Container.DataItem("LOT_OPERAZIONE") %>'>
																					</asp:Label>
																					<asp:DataGrid id="dgrRecord" style="table-layout: fixed; Width:95%" runat="server" CssClass="dgr2" AutoGenerateColumns="False">
																						<AlternatingItemStyle CssClass="r1"></AlternatingItemStyle>
																						<ItemStyle CssClass="r2"></ItemStyle>
																						<HeaderStyle CssClass="h1"></HeaderStyle>
																						<Columns>
																							<asp:TemplateColumn HeaderText="Campo">
																								<ItemTemplate>
																									<asp:Label runat="server" Text='<%# Container.DataItem("Campo") %>'>
																									</asp:Label>
																								</ItemTemplate>
																							</asp:TemplateColumn>
																							<asp:TemplateColumn HeaderText="Valore vecchio">
																								<ItemTemplate>
																									<asp:Label runat="server" Text='<%# Container.DataItem("Valore vecchio") %>'>
																									</asp:Label>
																								</ItemTemplate>
																							</asp:TemplateColumn>
																							<asp:TemplateColumn HeaderText="Valore nuovo">
																								<ItemTemplate>
																									<asp:Label runat="server" Text='<%# Container.DataItem("Valore nuovo") %>'>
																									</asp:Label>
																								</ItemTemplate>
																							</asp:TemplateColumn>
																						</Columns>
																					</asp:DataGrid>
																				</ItemTemplate>
																			</asp:DataList>
                                                                        </td>
																	</tr>
																</table>
															</ItemTemplate>
														</asp:TemplateColumn>
													</Columns>
												</asp:DataGrid>
                                            </td>
										</tr>
									</table>
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
					</asp:DataGrid>

                </dyp:DynamicPanel>

			</on_lay3:onitlayout3>

        </form>

		<script type="text/javascript" language="javascript">
		    function Espandi(dgrId, imgId) {

		        var img = document.getElementById(imgId);
		        var dgr = document.getElementById(dgrId);
		        var stato = img.stato;

		        if (stato == 'True') {
		            if (dgr != null) dgr.style.display = 'none';
		            img.stato = 'False';
		            img.src = '../../images/piu.gif';
		        }
		        else {
		            if (dgr != null) dgr.style.display = 'block';
		            img.stato = 'True';
		            img.src = '../../images/meno.gif';
		        }
		    }
		</script>

	</body>
</html>
