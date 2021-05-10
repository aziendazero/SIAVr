<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ConflittiVisite.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.ConflittiVisite" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<%@ Register TagPrefix="uc2" TagName="FiltriRicercaConflitti" Src="../../Common/Controls/FiltriRicercaConflitti.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Conflitti Visite</title>
    
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/DettaglioConflitti.css") %>' type="text/css" rel="stylesheet" />
    
    <script type="text/javascript"> window.imagesPath = "<%= ResolveUrl("~/Images/") %>"; </script>
    <script type="text/javascript" src="<%= Me.ResolveClientUrl("~/common/scripts/conflitti.js") %>"></script>

</head>
<body>
    <form id="form1" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Height="100%" Width="100%" TitleCssClass="Title3" Titolo="Risoluzione Conflitti Visite">
                       
            <div class="title" id="PanelTitolo" runat="server" style="width: 100%;">
			    <asp:Label id="LayoutTitolo" runat="server">&nbsp;Risoluzione Conflitti Visite</asp:Label>
		    </div>
            <div>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
		            <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
		            <Items>
                        <igtbar:TBarButton Key="btnCerca" Text="Cerca" DisabledImage="../../images/cerca_dis.gif" Image="../../images/cerca.gif"
		                    ToolTip="Effettua la ricerca in base ai filtri impostati" >
                        </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btnPulisci" Text="Pulisci" DisabledImage="../../images/pulisci_dis.gif" Image="../../images/pulisci.gif"
		                    ToolTip="Cancella i filtri e i risultati della ricerca" >
		                </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btnEdit" Text="Modifica" DisabledImage="../../images/modifica_dis.gif" Image="../../images/modifica.gif"
		                    ToolTip="Modifica" ></igtbar:TBarButton>
		                <igtbar:TBSeparator />
		                <igtbar:TBarButton Key="btnRisolviConflitti" Text="Risolvi Conflitti" DisabledImage="../../images/risolviConflitti_dis.png" Image="../../images/risolviConflitti.png"
		                    ToolTip="Risolve i conflitti selezionati" >
			                <DefaultStyle Width="110px" CssClass="infratoolbar_button_default"></DefaultStyle>
		                </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btnAnnulla" Text="Annulla" DisabledImage="../../images/annulla_dis.gif" Image="../../images/annulla.gif"
		                    ToolTip="Annulla le modifiche effettuate" >
		                </igtbar:TBarButton>
                    </Items>
		        </igtbar:UltraWebToolbar>
            </div>
            
            <uc2:FiltriRicercaConflitti id="ucFiltriRicercaConflitti" runat="server"></uc2:FiltriRicercaConflitti>

            <div class="vac-sezione">
                <asp:Label id="lblRisultati" runat="server">Risultati della ricerca</asp:Label>
            </div>

            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
    			        
                <asp:DataGrid id="dgrConflitti" runat="server" Width="100%" AutoGenerateColumns="False" AllowCustomPaging="true" AllowPaging="true" PageSize="25" >
					<AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
					<ItemStyle CssClass="item"></ItemStyle>
					<PagerStyle CssClass="pager" Position="TopAndBottom" Mode="NumericPages" />
                    <HeaderStyle CssClass="header"></HeaderStyle>
					<SelectedItemStyle CssClass="selected"></SelectedItemStyle>
					<Columns>
                        <asp:TemplateColumn Visible="false">
                            <HeaderStyle Width="20px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <HeaderTemplate>
                                <input id="chkSelezionaTutti" type="checkbox" onclick="selezionaTutti(this, <%= DgrConflittoColumnIndex.ChkSelezione %>);" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSelezione" runat="server" AutoPostBack="false" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                                
					    <asp:TemplateColumn Visible="false">
					        <ItemTemplate>
					            <asp:Label ID="lblCodicePazienteCentrale" runat="server" Text='<%# Eval("CodicePazienteCentrale") %>' ></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
					    <asp:TemplateColumn Visible="false">
					        <ItemTemplate>
					            <asp:Label ID="lblIdVisitaCentrale" runat="server" Text='<%# Eval("IdVisitaCentrale") %>' ></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>

                        <asp:TemplateColumn HeaderText="Cognome e Nome Paziente">
					        <HeaderStyle Width="18%" />
					        <ItemTemplate>
					            <asp:Label ID="lblCognomeNomePaziente" runat="server" Text='<%# Eval("Cognome") + " " + Eval("Nome") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
					    <asp:TemplateColumn HeaderText="Data Nascita" >
					        <HeaderStyle Width="12%" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
					        <ItemTemplate>
					            <asp:Label ID="lblDataNascitaPaziente" runat="server" Text='<%# BindDateField(Eval("DataNascita")) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn>
					        <HeaderStyle Width="16px" HorizontalAlign="Right" />
							<ItemStyle HorizontalAlign="Right" ></ItemStyle>
                            <HeaderTemplate>
								<div class="margin_btn">
                                    <img id="imgTuttiDettagli" alt="Mostra dettagli di tutti i conflitti" runat="server" src="~/Images/piu.gif"
                                        height="16" width="16" border="0" style="cursor:hand" onclick='showAllDettagliVac(this);' />
								</div>
                            </HeaderTemplate>
							<ItemTemplate>
								<div class="margin_btn">
                                    <img id="imgDettaglio" name="imgDettaglio" alt="Mostra dettagli conflitto" src="../../Images/piu.gif"
										height="16" width="16" border="0" style="cursor:hand" onclick='showDettaglioVac(this,"<%# Container.FindControl("dgrDettaglio").ClientId %>");' />
                                </div>
							</ItemTemplate>
						</asp:TemplateColumn>

                        <asp:TemplateColumn HeaderText="Visite in conflitto" >
					        <HeaderStyle Width="68%" />
					        <ItemTemplate>
                                <div id="divDettaglio" name="divDettaglio" style="display: none; margin-top: 2px; margin-bottom: 2px;">
                                    <asp:HiddenField ID="hdIndexFlagVisibilita" runat="server" />
								    <asp:DataGrid id="dgrDettaglio" runat="server" Width="100%" CssClass="datagridDettaglio" AutoGenerateColumns="False" >
										<AlternatingItemStyle CssClass="alternatingDettaglio"></AlternatingItemStyle>
										<ItemStyle CssClass="itemDettaglio"></ItemStyle>
										<HeaderStyle CssClass="headerDettaglio"></HeaderStyle>
										<Columns>
					                        <asp:TemplateColumn Visible="false">
					                            <ItemTemplate>
					                                <asp:Label ID="lblIdVisitaCentrale" runat="server" Text='<%# Eval("Id") %>' ></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
					                        <asp:TemplateColumn Visible="false">
					                            <ItemTemplate>
					                                <asp:Label ID="lblIdVisitaLocale" runat="server" Text='<%# Eval("IdVisita") %>' ></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
					                        <asp:TemplateColumn Visible="false">
					                            <ItemTemplate>
					                                <asp:Label ID="lblCodicePazienteLocale" runat="server" Text='<%# Eval("CodicePaziente") %>' ></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>

                                            <asp:TemplateColumn HeaderText="">
                                                <HeaderStyle Width="10%" HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                <HeaderTemplate>
                                                    <asp:Label ID="lblHeaderFlagConsenso" runat="server" Width="100%"
                                                        Text="<%$ Resources: Onit.OnAssistnet.OnVac.Web, DatiVaccinali.ConsensoComunicazione %>" 
                                                        ToolTip="Consenso alla comunicazione dei dati vaccinali da parte del paziente" ></asp:Label>
                                                </HeaderTemplate>
					                            <ItemTemplate>
                                                    <asp:HiddenField ID="hdFlagVisibilita" runat="server" />
                                                    <asp:Image ID="imgFlagVisibilita" runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
					                        <asp:TemplateColumn HeaderText="Data Visita">
                                                <HeaderStyle Width="15%" HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" />
					                            <ItemTemplate>
					                                <asp:Label ID="lblDataVisita" runat="server" Text='<%# BindDateField(Eval("DataVisita")) %>' ></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
					                        <asp:TemplateColumn HeaderText="Malattia">
                                                <HeaderStyle Width="15%" />
					                            <ItemTemplate>
					                                <asp:Label ID="lblDescrizioneMalattia" runat="server" Text='<%# Eval("DescrizioneMalattia") %>' ></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
					                        <asp:TemplateColumn HeaderText="Num. Bil.">
                                                <HeaderStyle Width="15%" />
					                            <ItemTemplate>
					                                <asp:Label ID="lblNumeroBilancio" runat="server" Text='<%# Eval("NumeroBilancio") %>' ></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
					                        <asp:TemplateColumn HeaderText="Bilancio">
                                                <HeaderStyle Width="15%" />
					                            <ItemTemplate>
					                                <asp:Label ID="lblDescrizioneBilancio" runat="server" Text='<%# Eval("DescrizioneBilancio") %>' ></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
					                        <asp:TemplateColumn HeaderText="Fine Sospensione">
                                                <HeaderStyle Width="15%" HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" />
					                            <ItemTemplate>
					                                <asp:Label ID="lblDataFineSospensione" runat="server" Text='<%# BindDateField(Eval("DataFineSospensione")) %>' ></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn HeaderText="Azienda">
                                                <HeaderStyle Width="10%" />
					                            <ItemTemplate>
					                                <asp:Label ID="lblCodiceUslVaccinazioneEseguita" runat="server" Text='<%# Eval("CodiceUslVisita") %>' ></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn HeaderText="Stato" Visible="false" >
                                                <HeaderStyle Width="5%" HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" />
                                                <ItemTemplate>
                                                    <asp:Image ID="imgTipoVisitaCentrale" runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
										</Columns>
									</asp:DataGrid>                                        
                                </div>
                            </ItemTemplate>
                        </asp:TemplateColumn>
						<asp:TemplateColumn>
					        <HeaderStyle Width="1%" />
                            <ItemTemplate>&nbsp;</ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
				</asp:DataGrid>

            </dyp:DynamicPanel>

        </on_lay3:OnitLayout3>
    </form>
</body>
</html>

