<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReazAvverse.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_ReazAvverse" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<%@ Register TagPrefix="uc1" TagName="ReazAvverseDetail" Src="../../common/Controls/ReazAvverseDetail.ascx" %>

<%@ Import Namespace="Onit.OnAssistnet.OnVac" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Reazioni Avverse</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript" src='<%= OnVacUtility.GetScriptUrl("scriptReazioniAvverse.js") %>'></script>

    <script type="text/javascript" language="javascript">
    		
		function confermaEliminazione()
		{
			if (!confirm("Eliminare la reazione avversa?"))
				StopPreventDefault(getEvent());
		}
	
		function InizializzaToolBarDetail(t)
		{
			t.PostBackButton=false;
		}
		
		function ToolBarDetailClick(ToolBar,button,evnt)
        {
			evnt.needPostBack=true;
		
			switch(button.Key)
			{
			    case 'btn_Indietro':

					if(!confirm("Le modifiche apportate non saranno salvate. Continuare?")) evnt.needPostBack=false;
					break;

			    case 'btn_Conferma':

			        if (!CheckDettaglioReazione('<%=ReazAvverseDetail.GetDataOraVaccinazioneString()%>', <%= ReazAvverseDetail.IsAltraReazioneObbligatoria().ToString().ToLower() %>)) evnt.needPostBack = false;
			        break;
				
				default:
					evnt.needPostBack=true; 
			}
		}
		
		function showDettaglioVac(img, dgrId) 
        {
			var ret = "0";
			
			// Cambio l'immagine del pulsante
			if (img.src.indexOf('piu.gif')!=-1) {
			    img.src='../../images/meno.gif';
				img.alt='Nascondi vaccinazioni associate';
			} else {
			    img.src='../../images/piu.gif';
				img.alt='Mostra vaccinazioni associate';
			}
			
			// divDettaglio
			var div=document.getElementById(dgrId).parentNode;
			
			if (div==null) return ret;
			
			// Mostro o nascondo il dettaglio delle vaccinazioni
			if (div.style.display=='none') {
				div.style.display='block';
				ret = "1";
			} else {
				div.style.display='none';
				ret = "0"
			}			
			
			return ret;
		}
    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Titolo="Reazioni Avverse" Width="100%" Height="100%" TitleCssClass="Title3">
            
            <asp:MultiView ID="mlvMaster" runat="server">

                <!-- View elenco reazioni avverse -->
                <asp:View ID="pan_Reaz" runat="server">
					        
                    <div class="Title" id="divLayoutTitolo1" >
						<asp:Label id="LayoutTitolo1" runat="server" Width="100%"></asp:Label>
                    </div>

                    <div>
				        <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                            <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                            <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
						    <Items>
							    <igtbar:TBarButton Key="btn_Salva" Text="Salva" DisabledImage="~/Images/salva_dis.gif" Image="~/Images/salva.gif"></igtbar:TBarButton>
							    <igtbar:TBarButton Key="btn_Annulla" Text="Annulla" DisabledImage="~/Images/annulla_dis.gif" Image="~/Images/annulla.gif"></igtbar:TBarButton>
                                <igtbar:TBarButton Key="btnRecuperaStoricoVacc" Text="Recupera" DisabledImage="../../images/recupera_dis.png" Image="../../images/recupera.png" ToolTip="Recupera lo storico vaccinale centralizzato del paziente"></igtbar:TBarButton>
						    </Items>
					    </igtbar:UltraWebToolbar>
                    </div>

                    <div class="vac-sezione">
                        <asp:Label id="LayoutTitolo_sezione" runat="server" Text="ELENCO REAZIONI AVVERSE"></asp:Label>
                    </div>

					<div id="divLegenda" class="legenda-vaccinazioni">
						<span class="legenda-vaccinazioni-scaduta">S</span>
						<span>Associazione Scaduta</span>
                    </div>

                    <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="99%" ScrollBars="Auto" RememberScrollPosition="true">
						<asp:Panel id="pan_VacEs" class="pan_VacEs" runat="server" style="width:100%; height:100%; overflow: auto;">
                            <asp:DataGrid id="dg_Reaz" runat="server" Width="100%" CssClass="dgr" AutoGenerateColumns="False"
                                BorderWidth="0px" BorderColor="Black" CellPadding="1" >
							    <SelectedItemStyle Font-Bold="True" CssClass="Selected"></SelectedItemStyle>
							    <AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
							    <ItemStyle BackColor="#E7E7FF" VerticalAlign="Top"></ItemStyle>
							    <HeaderStyle Font-Bold="True" CssClass="header"></HeaderStyle>
							    <PagerStyle CssClass="Pager" Mode="NumericPages"></PagerStyle>
							    <Columns>
								    <asp:ButtonColumn Text="&lt;div class=&quot;margin_btn&quot;&gt; &lt;img src=&quot;../../images/dettaglio.gif&quot; title=&quot;Dettaglio&quot;&gt; &lt;/div&gt;"
									    CommandName="Select" >
									    <HeaderStyle HorizontalAlign="Center" Width="20px"></HeaderStyle>
									    <ItemStyle HorizontalAlign="Center"></ItemStyle>
								    </asp:ButtonColumn>
								    <asp:ButtonColumn SortExpression="BtnElimina"
                                        Text="&lt;div class=&quot;margin_btn&quot;&gt; &lt;img onclick='confermaEliminazione();' title=&quot;Elimina&quot; src=&quot;../../images/elimina.gif&quot; &gt; &lt;/div&gt;"
									    CommandName="Delete">
									    <ItemStyle HorizontalAlign="Center" Width="20px" VerticalAlign="Top"></ItemStyle>
									    <HeaderStyle Width="20px"></HeaderStyle>
								    </asp:ButtonColumn>
								    <asp:TemplateColumn SortExpression="BtnStampa">
									    <ItemTemplate>
									        <div class="margin_btn">
    										    <asp:ImageButton id="ImageButton1" runat="server" CommandName="Stampa" ImageUrl="~/Images/stampa.gif" Visible='<%# Iif(Container.DataItem.Row.RowState=System.Data.DataRowState.Unchanged ,True,False)%>' />
										    </div>
									    </ItemTemplate>
									    <HeaderStyle Width="20px"></HeaderStyle>
								    </asp:TemplateColumn>
								    <asp:TemplateColumn>
									    <ItemStyle HorizontalAlign="Center" VerticalAlign="Top"></ItemStyle>
									    <ItemTemplate>
										    <div class="margin_btn">
											    <img height="16" id="imgDettaglio" alt="Mostra vaccinazioni associate" src="../../Images/piu.gif"
												    width="16" border="0" style="cursor: hand" onclick='showDettaglioVac(this,"<%# Container.FindControl("dgrDettaglio").ClientId %>");' />
										    </div>
									    </ItemTemplate>
									    <HeaderStyle Width="20px"></HeaderStyle>
								    </asp:TemplateColumn>
								    <asp:BoundColumn DataField="ves_dataora_effettuazione" HeaderText="Data Effettuazione" DataFormatString="{0:dd/MM/yyyy HH:mm}">
									    <HeaderStyle HorizontalAlign="Center" Width="110px"></HeaderStyle>
									    <ItemStyle HorizontalAlign="Center"></ItemStyle>
								    </asp:BoundColumn>
								    <asp:TemplateColumn HeaderText="Associazione">
                                        <HeaderStyle Width="200px"></HeaderStyle>
								        <ItemStyle Width="200px" VerticalAlign="Top" />
									    <ItemTemplate>
									        <asp:HiddenField ID="tb_ves_id" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem")("ves_id") %>' />
										    <asp:Label ID="Label1" Width="100%" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("ass_descrizione") %>' />
                                            
                                            <asp:HiddenField ID="hdUslInserimento" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem")("ves_usl_inserimento") %>' />
										
										    <div id="divDettaglio" style="display: none; margin-top: 20px;">
								                <asp:DataGrid id="dgrDettaglio" runat="server" Width="100%" CssClass="dgr2" AutoGenerateColumns="False">
											        <AlternatingItemStyle CssClass="r1"></AlternatingItemStyle>
											        <ItemStyle CssClass="r2"></ItemStyle>
											        <HeaderStyle CssClass="h1"></HeaderStyle>
											        <Columns>
												        <asp:BoundColumn DataField="vac_descrizione" HeaderText="Descrizione" HeaderStyle-Width="75%"></asp:BoundColumn>
												        <asp:BoundColumn DataField="ves_n_richiamo" HeaderText="Dose" HeaderStyle-Width="25%" HeaderStyle-HorizontalAlign="Center"
													        ItemStyle-HorizontalAlign="Center"></asp:BoundColumn>
												        <asp:BoundColumn Visible="False" DataField="ves_ass_codice" HeaderText="ves_ass_codice"></asp:BoundColumn>
												        <asp:BoundColumn Visible="False" DataField="ves_data_effettuazione" HeaderText="ves_data_effettuazione"></asp:BoundColumn>
											        </Columns>
										        </asp:DataGrid>
									        </div>
									    </ItemTemplate>
								    </asp:TemplateColumn>
								    <%--
								    <asp:TemplateColumn>
									    <HeaderStyle HorizontalAlign="Center" Width="30%"></HeaderStyle>
									    <ItemStyle HorizontalAlign="Center"></ItemStyle>
									    <HeaderTemplate>
										    <TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
											    <TR>
												    <TD align="left" width="70%">Descrizione
												    </TD>
												    <TD align="left" width="30%">Codice</TD>
											    </TR>
										    </TABLE>
									    </HeaderTemplate>
									    <ItemTemplate>
										    <TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
											    <TR>
												    <TD width="70%">
													    <asp:Label id=lb_descVac runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("vac_descrizione") %>'></asp:Label>
												    </TD>
												    <TD width="30%">
													    <asp:Label id=lb_codVac runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("ves_vac_codice") %>'></asp:Label>
												    </TD>
											    </TR>
										    </TABLE>
									    </ItemTemplate>
								    </asp:TemplateColumn>
								    --%>
								    <asp:BoundColumn DataField="ves_ass_n_dose" HeaderText="Dose">
									    <HeaderStyle HorizontalAlign="Center" Width="35px"></HeaderStyle>
									    <ItemStyle HorizontalAlign="Center"></ItemStyle>
								    </asp:BoundColumn>
								    <asp:TemplateColumn HeaderText="Nome commerciale">
								        <HeaderStyle HorizontalAlign="Center"  Width="80px" ></HeaderStyle>
									    <ItemStyle HorizontalAlign="Left"></ItemStyle>
									    <ItemTemplate>
										    <asp:Label id="tb_nome_com" Width="100%" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("noc_descrizione") %>'>
										    </asp:Label>
									    </ItemTemplate>
								    </asp:TemplateColumn>								
								    <asp:TemplateColumn HeaderText="Reazione Avversa">
									    <HeaderStyle HorizontalAlign="Left" Width="120px"></HeaderStyle>
									    <ItemStyle HorizontalAlign="Center"></ItemStyle>
									    <ItemTemplate>
										    <table cellspacing="0" cellpadding="0" width="100%" border="0">
											    <tr>
												    <td width="100%">
													    <asp:Label id="lb_descReaz" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("rea_descrizione") %>'></asp:Label>
												    </td>
											    </tr>
										    </table>
									    </ItemTemplate>
								    </asp:TemplateColumn>
								    <asp:TemplateColumn HeaderText="Reazione 2">
									    <HeaderStyle HorizontalAlign="Left" Width="100px"></HeaderStyle>
									    <ItemStyle HorizontalAlign="Left"></ItemStyle>
									    <ItemTemplate>
										    <asp:Label id="lb_codReaz2" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("rea_descrizione1") %>'>
										    </asp:Label>
									    </ItemTemplate>
								    </asp:TemplateColumn>
								    <asp:TemplateColumn HeaderText="Reazione 3">
									    <HeaderStyle HorizontalAlign="Left" Width="100px"></HeaderStyle>
									    <ItemStyle HorizontalAlign="Left"></ItemStyle>
									    <ItemTemplate>
										    <asp:Label id="lb_codReaz3" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("rea_descrizione2") %>'>
										    </asp:Label>
									    </ItemTemplate>
								    </asp:TemplateColumn>
								    <asp:BoundColumn DataField="vra_data_reazione" HeaderText="Data&lt;br&gt;Reazione" DataFormatString="{0:dd/MM/yyyy}">
									    <HeaderStyle HorizontalAlign="Center" Width="90px"></HeaderStyle>
									    <ItemStyle HorizontalAlign="Center"></ItemStyle>
								    </asp:BoundColumn>
								    <asp:TemplateColumn HeaderText="<%$ Resources: Onit.OnAssistnet.OnVac.Web, DatiVaccinali.AslInserimentoVacc %>" HeaderStyle-HorizontalAlign="Center" SortExpression="usl_inserimento_vra_descr" >
									    <ItemStyle width="100px" HorizontalAlign="Left" />
                                        <HeaderStyle width="100px" HorizontalAlign="Left" ></HeaderStyle>
									    <ItemTemplate>
										    <div align="left">
											    <asp:Label id="txtUslInserimentoReazione" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("usl_inserimento_vra_descr") %>'></asp:Label>
										    </div>
									    </ItemTemplate>
								    </asp:TemplateColumn>
								    <asp:TemplateColumn HeaderText="" SortExpression="FlagVisibilita" >
									    <HeaderStyle HorizontalAlign="Center" Width="20px"></HeaderStyle>
									    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                        <ItemTemplate>
                                            <asp:Image ID="imgFlagVisibilita" runat="server" 
                                                ImageUrl="<%# BindFlagVisibilitaImageUrlValue(Container.DataItem) %>" 
                                                ToolTip="<%# BindFlagVisibilitaToolTipValue(Container.DataItem) %>" />
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
								    <asp:TemplateColumn>
									    <HeaderStyle HorizontalAlign="Left" Width="20px"></HeaderStyle>
									    <ItemStyle HorizontalAlign="Left"></ItemStyle>
									    <ItemTemplate>
										    <asp:Label id="lblScaduta" runat="server" CssClass="legenda-vaccinazioni-scaduta"
                                                ToolTip="Reazione avversa scaduta"
                                                Visible='<%# IIf(Eval("scaduta").ToString() = "S", True, False) %>'>S</asp:Label>
									    </ItemTemplate>
								    </asp:TemplateColumn>
								    <asp:BoundColumn Visible="False" DataField="vra_rea_codice" HeaderText="ReCodice"></asp:BoundColumn>
								    <asp:BoundColumn Visible="False" DataField="vra_re1_codice" HeaderText="Re1Codice"></asp:BoundColumn>
								    <asp:BoundColumn Visible="False" DataField="vra_re2_codice" HeaderText="Re2Codice"></asp:BoundColumn>
                                    <asp:BoundColumn Visible="False" DataField="vra_usl_inserimento" HeaderText="UslInserimentoCodice"></asp:BoundColumn>
							    </Columns>
						    </asp:DataGrid>
						</asp:Panel>
                    </dyp:DynamicPanel>

                    <div class="alert" id="lblWarning1" runat="server"></div>

                </asp:View>

                <!-- View del dettaglio della reazione avversa -->
                <asp:View ID="pan_Det" runat="server">
                    
                    <div class="Title" id="divLayoutTitolo" >
						<asp:Label id="LayoutTitolo2" runat="server"></asp:Label>
                    </div>

                    <div>
				        <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBarDetail" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                            <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                            <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
						    <ClientSideEvents InitializeToolbar="InizializzaToolBarDetail" Click="ToolBarDetailClick"></ClientSideEvents>
                            <Items>
							    <igtbar:TBarButton Key="btn_Conferma" DisabledImage="~/Images/conferma_dis.gif" Text="Conferma" Image="~/Images/conferma.gif"></igtbar:TBarButton>
							    <igtbar:TBarButton Key="btn_Indietro" DisabledImage="~/Images/annullaconf_dis.gif" Text="Indietro" Image="~/Images/annullaconf.gif"></igtbar:TBarButton>
							    <igtbar:TBSeparator />
							    <igtbar:TBarButton Key="btn_Stampa_Mod1" DisabledImage="~/Images/stampa_dis.gif" Text="Stampa" Image="~/Images/stampa.gif"></igtbar:TBarButton>							
						    </Items>
					    </igtbar:UltraWebToolbar>
                    </div>

                    <div class="vac-sezione">DETTAGLIO REAZIONE AVVERSA</div>

                    <uc1:ReazAvverseDetail id="ReazAvverseDetail" runat="server"></uc1:ReazAvverseDetail>

                    <div class="alert" id="lblWarning2" runat="server"></div>

                </asp:View>

            </asp:MultiView>

        </on_lay3:OnitLayout3>

    </form>

    <script type="text/javascript" language="javascript">
		<%response.write(strJS)%>	
    </script>

    <!-- #include file="../../Common/Scripts/AggiornaStatoVaccinazioni.inc" -->
    <!-- #include file="../../Common/Scripts/ControlloDatiValidi.inc" -->

</body>
</html>
