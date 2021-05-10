<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="UnmergePazienti.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.UnmergePazienti" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>

<%@ Register Src="../../Common/Controls/DatiVaccinaliPaziente.ascx" TagName="DatiVaccinaliPaziente" TagPrefix="uc1" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Unmerge Pazienti</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <style type="text/css">
        .toUpper {
            text-transform: uppercase;
        }

        .legendFiltri {
            text-align: left;
            font-family: Arial;
            font-size: 12px;
            font-weight: bold;
            color: Navy;
        }
    </style>

    <script type="text/javascript" language="javascript">
        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;
        }
    </script>

</head>
<body>
    <form id="Form1" method="post" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Height="100%" Width="100%" TitleCssClass="Title3" Titolo="Unmerge Pazienti - Annullamento Alias">
			<div class="title" id="PanelTitolo" runat="server" style="width: 100%;">
			    <asp:Label id="LayoutTitolo" runat="server">&nbsp;Annullamento Pazienti Alias</asp:Label>
			</div>
            <div>
			    <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
		            <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
		            <Items>
		                <igtbar:TBarButton Key="btnCerca" Text="Cerca" DisabledImage="../../images/cerca.gif" Image="../../images/cerca.gif"
		                    ToolTip="Effettua la ricerca in base ai filtri impostati" >
			                <DefaultStyle Width="100px" CssClass="infratoolbar_button_default"></DefaultStyle>
		                </igtbar:TBarButton>
		                <igtbar:TBarButton Key="btnInfoMaster" Text="Visualizza Dati Master" DisabledImage="~/Images/dettaglio_dis.gif" Image="~/Images/dettaglio.gif"
		                    ToolTip="Visualizza i dati anagrafici del master" >
			                <DefaultStyle Width="150px" CssClass="infratoolbar_button_default"></DefaultStyle>
		                </igtbar:TBarButton>
		                <igtbar:TBarButton Key="btnUnmerge" Text="Esegui" DisabledImage="../../images/rotella_dis.gif" Image="../../images/rotella.gif"
		                    ToolTip="Esegue l'operazione di unmerge" >
			                <DefaultStyle Width="100px" CssClass="infratoolbar_button_default"></DefaultStyle>
		                </igtbar:TBarButton>
		                <igtbar:TBSeparator />
		                <igtbar:TBarButton Key="btnPulisci" Text="Pulisci" DisabledImage="../../images/pulisci_dis.gif" Image="../../images/pulisci.gif"
		                    ToolTip="Cancella i filtri e i risultati della ricerca" >
			                <DefaultStyle Width="100px" CssClass="infratoolbar_button_default"></DefaultStyle>
		                </igtbar:TBarButton>
                                
                        <%--  

                        <igtbar:TBarButton Key="btnTestReceiveInserisci" Text="Test Receive Inserisci" Image="../../images/alert.gif" ToolTip="TODO: DA CANCELLARE!!!" >
                            <DefaultStyle Width="150px"></DefaultStyle>
                        </igtbar:TBarButton> 

                        <igtbar:TBarButton Key="btnTestReceiveModifica" Text="Test Receive Modifica" Image="../../images/alert.gif" ToolTip="TODO: DA CANCELLARE!!!" >
                            <DefaultStyle Width="150px"></DefaultStyle>
                        </igtbar:TBarButton>  
                                
                        <igtbar:TBarButton Key="btnTestReceiveMerge" Text="Test Receive Merge" Image="../../images/alias.gif" ToolTip="TODO: DA CANCELLARE!!!" >
                            <DefaultStyle Width="130px"></DefaultStyle>
                        </igtbar:TBarButton> 
                               
                        <igtbar:TBarButton Key="btnTestMerge" Text="Test Merge" Image="../../images/alias.gif" ToolTip="TODO: DA CANCELLARE!!!" >
                            <DefaultStyle Width="100px"></DefaultStyle>
                        </igtbar:TBarButton>  

                        <igtbar:TBarButton Key="btnTestSend" Text="Test Send" Image="../../images/TermPer.gif" ToolTip="TODO: DA CANCELLARE!!!" >
                            <DefaultStyle Width="100px"></DefaultStyle>
                        </igtbar:TBarButton>

                        --%>
                    </Items>
		        </igtbar:UltraWebToolbar>
            </div>
			<div class="vac-sezione" style="margin-top:2px">Filtri di ricerca</div>
            <div>
			    <table width="100%" style="background-color: #F5F5F5" border="0">
	                <tr style="height:2px;">
		                <td></td>
	                </tr>
	                <tr align="center">
		                <td>
			                <fieldset class="vacFieldset" title="Filtri di ricerca">
			                    <legend style="width:175px">
			                        <table cellspacing="0" cellpadding="0" width="100%" border="0">
			                            <tr style="vertical-align:middle;">
			                                <td class="legendFiltri" style="vertical-align:middle; margin-right: 5px;">Filtri di ricerca Alias</td>
                                            <td style="text-align:center">
			                                    <asp:LinkButton ID="btnPulisciFiltriAlias" runat="server" ToolTip="Cancella i filtri di ricerca relativi al paziente alias" >
			                                        <img src="../../images/pulisci.gif" alt="Cancella i filtri di ricerca relativi al paziente alias" />
			                                    </asp:LinkButton>
                                            </td>
                                        </tr>
                                    </table>
                                </legend>
                                <table class="label_left" style="margin-top: 2px" cellspacing="0" cellpadding="2" width="100%" border="0">
					                <colgroup>
						                <col width="12%" />
						                <col width="22%" />
						                <col width="10%" />
						                <col width="22%" />
						                <col width="10%" />
						                <col width="22%" />
						                <col width="2%" />
					                </colgroup>
					                <tr style="height: 3px">
						                <td colspan="7"></td>
					                </tr>
					                <tr>
					                    <td class="Label">Cognome</td>
                                        <td>
					                        <asp:TextBox ID="txtCognomeAlias" runat="server" Width="100%" class="toUpper"></asp:TextBox></td>
                                        <td class="Label">Nome</td>
                                        <td>
					                        <asp:TextBox ID="txtNomeAlias" runat="server" Width="100%" class="toUpper"></asp:TextBox></td>
                                        <td class="Label">Sesso:</td>
                                        <td>
					                        <asp:DropDownList ID="ddlSessoAlias" Runat="server" Width="100%">
					                            <asp:ListItem Selected="True"></asp:ListItem>
                                                <asp:ListItem Value="M">Maschio</asp:ListItem>
                                                <asp:ListItem Value="F">Femmina</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td></td>
					                </tr>
					                <tr>
					                    <td class="Label">Codice Fiscale</td>
                                        <td>
					                        <asp:TextBox ID="txtCodiceFiscaleAlias" runat="server" Width="100%" class="toUpper"></asp:TextBox></td>
                                        <td class="Label">Data di Nascita</td>
                                        <td>
					                        <on_val:onitdatepick ID="dpkDataNascitaAlias" runat="server" CssClass="textbox_data" DateBox="True" 
					                            Height="20px" Width="120px"></on_val:onitdatepick>
						                </td>
						                <td class="Label">Comune di Nascita</td>
                                        <td>
						                    <onitcontrols:OnitModalList id="modComuneNascitaAlias" runat="server" 
						                        Filtro=" 1=1 order by com_descrizione" 
								                Width="100%" CampoDescrizione="COM_DESCRIZIONE" CampoCodice="COM_CODICE" Tabella="T_ANA_COMUNI" 
								                PosizionamentoFacile="False" LabelWidth="-1px" CodiceWidth="0px" Label="Titolo" UseCode="False" 
								                SetUpperCase="True" Obbligatorio="False">
								            </onitcontrols:OnitModalList>
						                </td>
						                <td></td>
			                        </tr>
					                <tr>
						                <td class="label">Centro Vaccinale:</td>
                                        <td>
							                <onitcontrols:OnitModalList id="modConsultorioAlias" runat="server" Filtro="cns_data_apertura <= SYSDATE AND (cns_data_chiusura > SYSDATE OR cns_data_chiusura IS NULL) ORDER BY cns_descrizione"
								                Width="70%" CampoDescrizione="CNS_DESCRIZIONE" CampoCodice="CNS_CODICE" Tabella="T_ANA_CONSULTORI" PosizionamentoFacile="False"
								                LabelWidth="-1px" CodiceWidth="29%" Label="Titolo" UseCode="True" SetUpperCase="True" Obbligatorio="False">
								            </onitcontrols:OnitModalList>
								        </td>
						                <td colspan="5"></td>
						            </tr>
					                <tr style="height: 3px">
						                <td colspan="7"></td>
					                </tr>
			                    </table>
		                    </fieldset>
                        </td>
                    </tr>
                    <tr align="center">
		                <td>
		                    <fieldset class="vacFieldset" title="Filtri di ricerca">
		                        <legend style="width:175px">
		                            <table cellspacing="0" cellpadding="0" width="100%" border="0">
			                            <tr style="vertical-align:middle;">
			                                <td class="legendFiltri" style="vertical-align:middle; margin-right: 5px;">Filtri di ricerca Master</td>
                                            <td style="text-align:center">
			                                    <asp:LinkButton ID="btnPulisciFiltriMaster" runat="server" >
			                                        <img src="../../images/pulisci.gif" alt="Cancella i filtri di ricerca relativi al paziente master" />
			                                    </asp:LinkButton>
			                                </td>
			                            </tr>
			                        </table>
			                    </legend>
			                    <table class="label_left" style="margin-top: 2px" cellspacing="0" cellpadding="2" width="100%" border="0">
				                    <colgroup>
					                    <col width="12%" />
					                    <col width="22%" />
					                    <col width="10%" />
					                    <col width="22%" />
					                    <col width="10%" />
					                    <col width="22%" />
					                    <col width="2%" />
				                    </colgroup>
				                    <tr style="height: 3px">
					                    <td colspan="7"></td>
				                    </tr>
				                    <tr>
				                        <td class="Label">Cognome</td>
                                        <td>
				                            <asp:TextBox ID="txtCognomeMaster" runat="server" Width="100%" class="toUpper"></asp:TextBox></td>
                                        <td class="Label">Nome</td>
                                        <td>
				                            <asp:TextBox ID="txtNomeMaster" runat="server" Width="100%" class="toUpper"></asp:TextBox></td>
                                        <td class="Label">Sesso:</td>
                                        <td>
				                            <asp:DropDownList ID="ddlSessoMaster" Runat="server" Width="100%">
				                                <asp:ListItem Selected="True"></asp:ListItem>
                                                <asp:ListItem Value="M">Maschio</asp:ListItem>
                                                <asp:ListItem Value="F">Femmina</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td></td>
				                    </tr>
				                    <tr>
				                        <td class="Label">Codice Fiscale</td>
                                        <td>
				                            <asp:TextBox ID="txtCodiceFiscaleMaster" runat="server" Width="100%" class="toUpper"></asp:TextBox></td>
                                        <td class="Label">Data di Nascita</td>
                                        <td>
				                            <on_val:onitdatepick ID="dpkDataNascitaMaster" runat="server" CssClass="textbox_data" DateBox="True" 
				                                Height="20px" Width="120px"></on_val:onitdatepick>
					                    </td>
					                    <td class="Label">Comune di Nascita</td>
                                        <td>
					                        <onitcontrols:OnitModalList id="modComuneNascitaMaster" runat="server" 
					                            Filtro=" 1=1 order by com_descrizione" 
							                    Width="100%" CampoDescrizione="COM_DESCRIZIONE" CampoCodice="COM_CODICE" Tabella="T_ANA_COMUNI" 
							                    PosizionamentoFacile="False" LabelWidth="-1px" CodiceWidth="0px" Label="Titolo" UseCode="False" 
							                    SetUpperCase="True" Obbligatorio="False">
							                </onitcontrols:OnitModalList>
					                    </td>							                        
					                    <td></td>
		                            </tr>
				                    <tr style="height: 3px">
					                    <td colspan="7"></td>
				                    </tr>
		                        </table>
	                        </fieldset>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="vac-sezione">
                <asp:Label id="lblRisultati" runat="server">Risultati della ricerca</asp:Label>
            </div>
            
            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">

    			<asp:DataGrid id="dgrAlias" runat="server" Width="100%" AutoGenerateColumns="False" AllowCustomPaging="true" AllowPaging="true" PageSize="25" >
					<AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
					<ItemStyle CssClass="item"></ItemStyle>
					<SelectedItemStyle CssClass="selected"></SelectedItemStyle>
                    <HeaderStyle CssClass="header"></HeaderStyle>
					<PagerStyle CssClass="pager" Position="TopAndBottom" Mode="NumericPages"  />
					<Columns>
					    <on_dgr:SelectorColumn>
					        <ItemStyle HorizontalAlign="Center" Width="2%" />
					    </on_dgr:SelectorColumn>
					    <asp:BoundColumn Visible="False" DataField="paz_codice" HeaderText="H_idPaz"></asp:BoundColumn>
					    <asp:BoundColumn Visible="False" DataField="CodicePazienteMaster" HeaderText="H_idMaster"></asp:BoundColumn>
					    <asp:TemplateColumn HeaderText="Paziente Alias">
					        <HeaderStyle Width="18%" />
					        <ItemTemplate>
					            <asp:Label ID="lblPazienteAlias" runat="server" Text='<%# Eval("paz_cognome") + " " + Eval("paz_nome") %>'></asp:Label></ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="data_nascita" HeaderText="Data Nascita" DataFormatString="{0:dd/MM/yyyy}">
							<HeaderStyle Width="7%"></HeaderStyle>
						</asp:BoundColumn>
						<asp:BoundColumn DataField="ComuneNascita_Descrizione" HeaderText="Comune Nascita">
							<HeaderStyle Width="10%"></HeaderStyle>
						</asp:BoundColumn>
						<asp:BoundColumn DataField="PAZ_CODICE_FISCALE" HeaderText="Cod. Fiscale">
							<HeaderStyle Width="8%"></HeaderStyle>
						</asp:BoundColumn>
						<asp:BoundColumn DataField="Sesso" HeaderText="Sesso">
							<HeaderStyle Width="2%"></HeaderStyle>
						</asp:BoundColumn>
						<asp:BoundColumn DataField="Cittadinanza_Descrizione" HeaderText="Cittadinanza">
							<HeaderStyle Width="10%"></HeaderStyle>
						</asp:BoundColumn>
						<asp:BoundColumn DataField="ComuneResidenza_Descrizione" HeaderText="Residenza">
							<HeaderStyle Width="10%"></HeaderStyle>
						</asp:BoundColumn>
						<asp:TemplateColumn HeaderText="Paziente Master">
						    <HeaderStyle Width="18%"></HeaderStyle>
						    <ItemTemplate>
					            <asp:Label ID="lblPazienteMaster" runat="server" Text='<%# Eval("CognomeMaster") + " " + Eval("NomeMaster") %>'></asp:Label></ItemTemplate></asp:TemplateColumn><asp:BoundColumn DataField="DataNascitaMaster" HeaderText="Data Nascita" DataFormatString="{0:dd/MM/yyyy}">
							<HeaderStyle Width="7%"></HeaderStyle>
						</asp:BoundColumn>
						<asp:BoundColumn DataField="CodiceFiscaleMaster" HeaderText="Cod. Fiscale">
							<HeaderStyle Width="8%"></HeaderStyle>
						</asp:BoundColumn>
                    </Columns>
				</asp:DataGrid>

            </dyp:DynamicPanel>

            <onitcontrols:OnitFinestraModale ID="modInfoMaster" Title="Paziente Master" runat="server" Width="800px" Height="600px" BackColor="LightGray">

                <uc1:DatiVaccinaliPaziente ID="uscDatiVaccinaliMaster" runat="server" />

            </onitcontrols:OnitFinestraModale>

        </on_lay3:OnitLayout3>
    </form>
</body>
</html>
