<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Log_bilancio.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.Log_bilancio" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<%@ Register TagPrefix="uc1" TagName="RicercaBilancio" Src="../../../Common/Controls/RicercaBilancio.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>GestioneBilancio</title>		

        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

        <style type="text/css">
            .margin {
                margin-top: 2px;
                margin-bottom: 2px;            
            }
        </style>

		<script type="text/javascript" language="javascript">
		    function InizializzaToolBar(t) {
		        t.PostBackButton = false;
		    }

		    function ToolBarClick(ToolBar, button, evnt) {
		        evnt.needPostBack = true;
		        evnt.needPostBack = true;
		    }
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" Titolo="Gestione Bilancio" width="100%" height="100%" HeightTitle="90px">
                <div>
					<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="130px" CssClass="infratoolbar">
                        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		                <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                        <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
						<Items>
							<igtbar:TBarButton Key="btnBilancio" DisabledImage="../../../images/bilanci_dis.gif" Text="Scegli Bilancio"
								Image="../../../images/bilanci.gif"></igtbar:TBarButton>
							<igtbar:TBarButton Key="Pulisci" Text="Pulisci" DisabledImage="~/Images/pulisci.gif"
								Image="~/Images/pulisci.gif">
                                <DefaultStyle CssClass="infratoolbar_button_default" Width="90px"></DefaultStyle>
							</igtbar:TBarButton>
						</Items>
					</igtbar:UltraWebToolbar>
                </div>
                <div class="Sezione">Filtri</div>
                <div>
					<table id="Table1" cellpadding="2" cellspacing="0" width="100%" border="0">
                        <colgroup>
                            <col width="10%" />
                            <col width="20%" />
                            <col width="10%" />
                            <col width="58%" />
                            <col width="2%" />
                        </colgroup>
						<tr>
							<td class="label_right">Da</td>
                            <td>
                                <on_val:OnitDatePick id="txtDaData" runat="server" DateBox="True" Width="130px"></on_val:OnitDatePick></td>
                            <td class="label_right">A</td>
							<td>
                                <on_val:OnitDatePick id="txtAdata" runat="server" DateBox="True" Width="130px"></on_val:OnitDatePick></td>
							<td></td>
						</tr>
						<tr>
                            <td class="label_right">N. Bilancio</td>
							<td>
                                <asp:TextBox id="txtNumero" runat="server" Width="130px" CssClass="TextBox_Numerico_Disabilitato" ReadOnly="true"></asp:TextBox>
							</td>
							<td class="label_right">Malattia</td>
                            <td>
                                <asp:TextBox id="txtMalattia" runat="server" Width="100%" CssClass="TextBox_Stringa_Disabilitato" ReadOnly="true"></asp:TextBox> 
                            </td>
                            <td></td>
						</tr>
					</table>
                </div>
                
                <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
							
                    <div class="sezione margin">
                        <asp:Label id="lblSezione" runat="server">Associazioni Bilanci-Osservazioni</asp:Label>
                    </div>
                    
                    <div>
					    <table class="header" id="Table12" cellspacing="0" cellpadding="0" width="100%" border="0">
						    <tr class="header" style="height:30px">
							    <td width="40">
								    <asp:Label id="Operazione_oss" runat="server">Op.</asp:Label></td>
							    <td width="195">
								    <asp:Label id="Data_oss" runat="server">Data</asp:Label></td>
							    <td width="130">
								    <asp:Label id="Ute_oss" runat="server">Utente</asp:Label></td>
							    <td width="5">
								    <asp:Label id="Codice_oss" runat="server">Oss Codice</asp:Label></td>
							    <td width="5">
								    <asp:Label id="Descrizione_oss" runat="server">Oss Descrizione</asp:Label></td>
							    <td width="0">
								    <asp:Label id="Label9" style="display: none" runat="server">vuoto</asp:Label></td>
						    </tr>
					    </table>
                    </div>

					<asp:DataGrid id="dgr_log_bilancio_oss" runat="server" ShowHeader="False" Width="100%" CssClass="DataGrid" AutoGenerateColumns="False">
                        <HeaderStyle CssClass="Header"></HeaderStyle>
                        <ItemStyle CssClass="Item"></ItemStyle>
                        <AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
                        <SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
                        <EditItemStyle CssClass="Edit"></EditItemStyle>
						<Columns>
							<asp:TemplateColumn>
								<ItemTemplate>
									<table id="Table_log_bil_oss" style="table-layout: fixed" cellspacing="0" cellpadding="0"
										width="100%" border="0">
										<tr style="font-family: Arial, Helvetica; font-size: 12px;">
											<td width="50">
												<asp:Image id="imgOperazione" runat="server" 
                                                    ToolTip='<%# iif(Container.DataItem("TLM_OPERAZIONE") = Onit.OnAssistnet.OnVac.Constants.OperazioneLogOsservazioniBilancio.Inserimento, "Inserimento", "Eliminazione")%>' 
                                                    ImageUrl='<%# "../../../images/" + IIf(Container.DataItem("TLM_OPERAZIONE") = Onit.OnAssistnet.OnVac.Constants.OperazioneLogOsservazioniBilancio.Inserimento, "op_inserimento.gif", "op_eliminazione.gif").ToString() %>'>
												</asp:Image>
											</td>
											<td width="200">
												<asp:Label id="lblDataOperazione" runat="server" Text='<%# Container.DataItem("TLM_DATA") %>'>
												</asp:Label></td>
											<td width="150">
												<asp:Label id="lblUtente" runat="server" Text='<%# Container.DataItem("UTE_DESCRIZIONE") %>'>
												</asp:Label></td>
											<td width="30">
												<asp:Label id="Label6" runat="server" Text='<%# Container.DataItem("TLM_OSS_CODICE") %>'>
												</asp:Label></td>
											<td width="400">
												<asp:Label id="lblStack" runat="server" Text='<%# Container.DataItem("OSS_DESCRIZIONE") %>'>
												</asp:Label></td>
											<td width="0">
												<asp:Label id="lblCodice" style="display: none" runat="server" Text='<%# Container.DataItem("TLM_OPERAZIONE") %>'>
												</asp:Label>
											</td>
										</tr>
									</table>
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
					</asp:DataGrid>

                    <div class="sezione margin">
                        <asp:Label id="Label1" runat="server">Associazioni Osservazioni-Risposte</asp:Label>
					</div>

                    <div>
					    <table class="header" id="testata_ris" cellspacing="0" cellpadding="0" width="100%" border="0">
						    <tr class="header" style="height:30px">
							    <td width="40">
								    <asp:Label id="Label8" runat="server">Op.</asp:Label></td>
							    <td width="195">
								    <asp:Label id="Label10" runat="server">Data</asp:Label></td>
							    <td width="145">
								    <asp:Label id="Label11" runat="server">Utente</asp:Label></td>
							    <td width="390">
								    <asp:Label id="Label12" runat="server">Domanda</asp:Label></td>
							    <td width="5">
								    <asp:Label id="Label13" runat="server">Risposta</asp:Label></td>
							    <td width="0">
								    <asp:Label id="Label14" style="display: none" runat="server">vuoto</asp:Label></td>
						    </tr>
					    </table>
                    </div>

					<asp:DataGrid id="dgr_log_oss_risp" runat="server" ShowHeader="False" Width="100%" CssClass="DataGrid" AutoGenerateColumns="False">
                        <HeaderStyle CssClass="Header"></HeaderStyle>
                        <ItemStyle CssClass="Item"></ItemStyle>
                        <AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
                        <SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
                        <EditItemStyle CssClass="Edit"></EditItemStyle>
						<Columns>
							<asp:TemplateColumn>
								<ItemTemplate>
									<table id="Table_log_oss_risp" style="table-layout: fixed" cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr style="font-family: Arial, Helvetica; font-size: 12px;">
											<td width="50">
												<asp:Image id="Image1" runat="server" 
                                                    ToolTip='<%# IIf(Container.DataItem("TLM_OPERAZIONE") = Onit.OnAssistnet.OnVac.Constants.OperazioneLogOsservazioniBilancio.Inserimento, "Inserimento", "Eliminazione")%>' 
                                                    ImageUrl='<%# "../../../images/" + IIf(Container.DataItem("TLM_OPERAZIONE") = Onit.OnAssistnet.OnVac.Constants.OperazioneLogOsservazioniBilancio.Inserimento, "op_inserimento.gif", "op_eliminazione.gif").ToString()%>'>
												</asp:Image>&nbsp;
											</td>
											<td width="200">
												<asp:Label id="Label3" runat="server" Text='<%# Container.DataItem("TLM_DATA") %>'>
												</asp:Label></td>
											<td width="150">
												<asp:Label id="Label4" runat="server" Text='<%# Container.DataItem("UTE_DESCRIZIONE") %>'>
												</asp:Label></td>
											<td width="400">
												<asp:Label id="Label5" runat="server" Text='<%# Container.DataItem("OSS_DESCRIZIONE") %>'>
												</asp:Label></td>
											<td width="280">
												<asp:Label id="Label7" runat="server" Text='<%# Container.DataItem("RIS_DESCRIZIONE") %>'>
												</asp:Label></td>
											<td width="0">
												<asp:Label id="Label2" style="display: none" runat="server" Text='<%# Container.DataItem("TLM_OPERAZIONE") %>'>
												</asp:Label>
											</td>
										</tr>
									</table>
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
					</asp:DataGrid>
                </dyp:DynamicPanel>
                
                <on_ofm:onitfinestramodale id="modRicBil" title="Ricerca bilancio" runat="server" width="617px" BackColor="LightGray">
								
                    <uc1:RicercaBilancio id="uscRicBil" filtraPazCodice="False" runat="server" IncludiNessunaMalattia="true"></uc1:RicercaBilancio>

				</on_ofm:onitfinestramodale>

			</on_lay3:onitlayout3>
		</form>
	</body>
</html>
