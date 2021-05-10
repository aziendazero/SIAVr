<%@ Page Language="vb" AutoEventWireup="false" Codebehind="VacRifiuti.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_VacRifiuti"%>

<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<%@ Register TagPrefix="uc1" TagName="InsRifiuto" Src="InsRifiuto.ascx" %>

<%@ Import namespace="Onit.OnAssistnet.OnVac" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>Rifiuti</title>
		
        <script type="text/javascript" language="javascript">
		
		    function InizializzaToolBar(t)
		    {
			    t.PostBackButton=false;
		    }

		    function ToolBarClick(ToolBar, button, evnt) 
		    {
			    evnt.needPostBack=true;
    		
			    switch(button.Key)
			    {
			        case 'btn_Annulla':
    				
					    if("<%response.write(CStr(onitlayout31.busy).toLower())%>"=="true"){
						    evnt.needPostBack= confirm("Le modifiche effettuate andranno perse. Continuare?");
						    }
					    else
						    evnt.needPostBack=false;
					    break;

	                case 'btn_Salva':
    				
					    if("<%response.write(CStr(onitlayout31.busy).toLower())%>"!="true") 
						    evnt.needPostBack=false;
					    else
					    {
						    if ('<%= msgElimProg %>'!="") 
							    if (!confirm('<%= msgElimProg %>')) evnt.needPostBack=false;
					    }
    					
					    break;
			    }
            }		
		</script>
		
        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
		
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
		
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" TitleCssClass="Title3" HeightTitle="90px" width="100%" height="100%" Titolo="Rifiuti">
				
				<div class="Title" id="divLayoutTitolo" style="width: 100%">
					<asp:Label id="LayoutTitolo" runat="server" CssClass="title"></asp:Label>
				</div>

                <div>
                    <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                        <DefaultStyle CssClass="infratoolbar_button_default">
                        </DefaultStyle>
                        <HoverStyle CssClass="infratoolbar_button_hover">
                        </HoverStyle>
                        <SelectedStyle CssClass="infratoolbar_button_selected">
                        </SelectedStyle>
					    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
					    <Items>
						    <igtbar:TBarButton Key="btn_Salva" Text="Salva" DisabledImage="~/Images/salva_dis.gif" Image="~/Images/salva.gif"></igtbar:TBarButton>
						    <igtbar:TBarButton Key="btn_Annulla" Text="Annulla" DisabledImage="~/Images/annulla_dis.gif"
							    Image="~/Images/annulla.gif"></igtbar:TBarButton>
						    <igtbar:TBarButton Key="btn_Inserisci" Text="Inserisci" DisabledImage="~/Images/nuovo_dis.gif"
							    Image="~/Images/nuovo.gif"></igtbar:TBarButton>
						    <igtbar:TBarButton Key="btn_Stampa" Text="Stampa" DisabledImage="~/Images/stampa_dis.gif"	Image="~/Images/stampa.gif">
						    </igtbar:TBarButton>
					    </Items>
				    </igtbar:UltraWebToolbar>
				</div>

				<div class="sezione" id="divLayoutTitolo_sezione" style="width: 100%">
					<asp:Label id="LayoutTitolo_sezione" runat="server">ELENCO VACCINAZIONI</asp:Label>
                </div>

				<dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
				    
                    <asp:DataGrid id="dg_VacInad" style="table-layout: fixed" runat="server" CssClass="DATAGRID" Width="100%"
						AutoGenerateColumns="False" CellPadding="1" GridLines="None">
						<SelectedItemStyle Font-Bold="True" CssClass="selected"></SelectedItemStyle>
						<AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
						<ItemStyle CssClass="item"></ItemStyle>
						<HeaderStyle CssClass="header"></HeaderStyle>
						<PagerStyle CssClass="pager" Mode="NumericPages"></PagerStyle>
						<Columns>
						
							<asp:ButtonColumn Text="&lt;img  title=&quot;Elimina&quot; src=&quot;../../images/elimina.gif&quot;"
								CommandName="Delete">
								<HeaderStyle HorizontalAlign="Center" Width="16px"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center"></ItemStyle>
							</asp:ButtonColumn>
							
							<asp:EditCommandColumn ButtonType="LinkButton" UpdateText="&lt;img  title=&quot;Conferma&quot; src=&quot;../../images/conferma.gif&quot;&gt;"
								CancelText="&lt;img  title=&quot;Annulla&quot; src=&quot;../../images/annullaconf.gif&quot; &gt;"
								EditText="&lt;img  title=&quot;Modifica&quot; src=&quot;../../images/modifica.gif&quot;  &gt;">
								<HeaderStyle HorizontalAlign="Center" Width="16px"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center"></ItemStyle>
							</asp:EditCommandColumn>							
							
							<asp:TemplateColumn>
								<HeaderStyle HorizontalAlign="Center" Width="15%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center" Width="15%"></ItemStyle>
								<HeaderTemplate>
									<P>Data</P>
								</HeaderTemplate>
								<ItemTemplate>
									<asp:Label id=Label1 runat="server" Text='<%# ctype(DataBinder.Eval(Container, "DataItem")("prf_data_rifiuto"),DATETIME).tostring("dd/MM/yyyy") %>'>
									</asp:Label>
								</ItemTemplate>
								<EditItemTemplate>
									<on_val:OnitDatePick id=dpkData Value='<%# DataBinder.Eval(Container, "DataItem").row("prf_data_rifiuto") %>' runat="server" CssClass="textbox_data" Height="20px" Width="120px" DateBox="True" NoCalendario="True" FormatoData="GeneralDate" Formatta="False" CalendarioPopUp="True" indice="-1" Hidden="False" Focus="False" ControlloTemporale="False">
									</on_val:OnitDatePick>
								</EditItemTemplate>
							</asp:TemplateColumn>
							
							<asp:TemplateColumn>
								<HeaderTemplate>
									<P>Descrizione</P>
								</HeaderTemplate>
								<ItemTemplate>
									<asp:Label id=lbl_codVac runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("prf_vac_codice") %>' Visible="False">
									</asp:Label>
									<asp:Label id=lbl_descVac runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("vac_descrizione") %>'>
									</asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
														
							<asp:TemplateColumn>
							<HeaderStyle HorizontalAlign="Left" Width="8%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Left" Width="8%"></ItemStyle>
								<HeaderTemplate>
									<P>Richiamo</P>
								</HeaderTemplate>
								<ItemTemplate>
									<asp:Label id=lblnrichiamo runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("prf_n_richiamo") %>'>
									</asp:Label>
								</ItemTemplate>
								<EditItemTemplate>
									<on_val:OnitJsValidator Width="30px" id=OnitJsValidator1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("prf_n_richiamo") %>' actionCorrect="True" actionDelete="False" actionFocus="True" actionMessage="True" actionSelect="True" actionUndo="True" autoFormat="False" validationType="Validate_custom" CustomValFunction-Length="12" CustomValFunction="validaNumero">
										<Parameters>
											<on_val:ValidationParam paramValue="2" paramOrder="0" paramType="number" paramName="numCifreIntere"></on_val:ValidationParam>
											<on_val:ValidationParam paramValue="0" paramOrder="1" paramType="number" paramName="numCifreDecimali"></on_val:ValidationParam>
											<on_val:ValidationParam paramValue="1" paramOrder="2" paramType="number" paramName="minValore"></on_val:ValidationParam>
											<on_val:ValidationParam paramValue="null" paramOrder="3" paramType="number" paramName="maxValore"></on_val:ValidationParam>
											<on_val:ValidationParam paramValue="true" paramOrder="4" paramType="boolean" paramName="blnCommaSeparator"></on_val:ValidationParam>
										</Parameters>
									</on_val:OnitJsValidator>
								</EditItemTemplate>
							</asp:TemplateColumn>
																	
							<asp:TemplateColumn>
								<HeaderStyle HorizontalAlign="Left" Width="20%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Left" Width="20%"></ItemStyle>
								<HeaderTemplate>
									Genitore
								</HeaderTemplate>
								<ItemTemplate>
									<asp:Label id="lbl_genitore" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("prf_genitore") %>'>
									</asp:Label>
								</ItemTemplate>
								<EditItemTemplate>
									<asp:TextBox id="tb_genitore" MaxLength="60" runat="server" Width="100%" Text='<%# DataBinder.Eval(Container, "DataItem")("prf_genitore") %>'>
									</asp:TextBox>
								</EditItemTemplate>
							</asp:TemplateColumn>
							
							<asp:TemplateColumn>
								<HeaderStyle HorizontalAlign="Left" Width="20%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Left" Width="20%"></ItemStyle>
								<HeaderTemplate>
									Note
								</HeaderTemplate>
								<ItemTemplate>
									<asp:Label id="lbl_noteRif" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("prf_note_rifiuto") %>'>
									</asp:Label>
								</ItemTemplate>
								<EditItemTemplate>
									<asp:TextBox id="tb_noteRif" runat="server" Width="100%" Text='<%# DataBinder.Eval(Container, "DataItem")("prf_note_rifiuto") %>'>
									</asp:TextBox>
								</EditItemTemplate>
							</asp:TemplateColumn>
														
							<asp:TemplateColumn>
								<HeaderStyle HorizontalAlign="Left" Width="10%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Left" Width="10%"></ItemStyle>
								<HeaderTemplate>
									Utente
								</HeaderTemplate>
								<ItemTemplate>
									<asp:Label id=lblCodUtente runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("ute_descrizione")%>' >
									</asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>							
							
							<asp:TemplateColumn>
								<ItemTemplate>
									<asp:Label id="lb_pazcodice" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("prf_paz_codice") %>' visible="false">
									</asp:Label>
									<asp:Label id="lb_codVac" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("prf_vac_codice") %>' visible="false">
									</asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
					</asp:DataGrid>

                </dyp:DynamicPanel>
					
				<on_ofm:onitfinestramodale id="modInsRifiuto" title="Inserisci rifiuti" runat="server" BackColor="LightGray" Width="350px">
					<uc1:InsRifiuto id="uscInsRifiuto" runat="server"></uc1:InsRifiuto>
				</on_ofm:onitfinestramodale>
					
			</on_lay3:onitlayout3>
			
		</form>
		
		<script type="text/javascript" language="javascript">
		    <%response.write(strJS)%>
		</script>
		
		<!-- #include file="../../Common/Scripts/AggiornaStatoVaccinazioni.inc" -->
		<!-- #include file="../../Common/Scripts/ControlloDatiValidi.inc" -->
		
	</body>
</html>
