<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RicercaLotti.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.RicercaLotti" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<%@ Register TagPrefix="ucFiltri" TagName="FiltriRicercaMagazzino" Src="../FiltriRicercaMagazzino.ascx" %>
<%@ Register TagPrefix="ucEtaAttivazione" TagName="CampiEtaAttivazione" Src="../../Common/Controls/CampiEtaAttivazione.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>Ricerca lotti magazzino</title>
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <script type="text/javascript" src="../Magazzino.js"></script>
    <script type="text/javascript">
        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;

            switch (button.Key) {
                case 'btnStampa':
                    // controllo se l'elenco contiene almeno un lotto da stampare
                    if ("<%= dgrLotti.items.count %>" == "0") {
                        alert("Ricercare almeno un lotto per effettuare la stampa!");
                        evnt.needPostBack = false;
                    }
                    break;
            }
        }
    </script>
    <style type="text/css">
        .saveAlertLabel {
            font-family: Verdana;
            font-size: 12px;
            font-weight: bold;
            color: Navy;
        }

        .saveInfoLabel, 
        .saveInfoLabelRight {
            font-family: Verdana;
            font-size: 11px;
            font-weight: normal;
            color: Navy;
        }

        .saveInfoLabelRight {
            text-align: right;
        }

        .saveButton {
            width: 80px;
            cursor: pointer;
            padding: 2px 2px 2px 2px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" TitleCssClass="Title3" Height="100%" Width="100%" Titolo="Magazzino Centro Vaccinale">

			<div class="title" id="PanelTitolo" runat="server">
				<asp:Label id="LayoutTitolo" runat="server"></asp:Label>
			</div>
            <div>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
					<DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
					<SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
					<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                    <Items>
                        <igtbar:TBarButton Key="btnCerca" Text="Cerca" >
                        </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btnInserisci" Text="Inserisci" DisabledImage="~/Images/nuovo_dis.gif" Image="~/Images/nuovo.gif">
                            <DefaultStyle CssClass="infratoolbar_button_default" Width="90px"></DefaultStyle>
                        </igtbar:TBarButton>
                        <igtbar:TBSeparator />
                        <igtbar:TBarButton Key="btnPulisci" Text="Pulisci" DisabledImage="~/Images/pulisci_dis.gif" Image="~/Images/pulisci.gif">
                        </igtbar:TBarButton>
                        <igtbar:TBSeparator />
                        <igtbar:TBarButton Key="btnStampa" Text="Stampa" >
                        </igtbar:TBarButton>
                    </Items>
				</igtbar:UltraWebToolbar>
            </div>
			<div class="vac-sezione">
				<asp:Label id="LayoutTitolo_sezioneRicerca" runat="server">FILTRI DI RICERCA</asp:Label>
			</div>
			<div>
				<ucFiltri:FiltriRicercaMagazzino ID="ucFiltriRicerca" ShowFiltroDistretti="false" runat="server" />
			</div>
			<div class="vac-sezione">
				<asp:Label id="LayoutTitolo_sezione" runat="server">ELENCO LOTTI</asp:Label>
			</div>
			<div id="divLegenda" class="legenda-vaccinazioni">
				<span id="lblAttivo" runat="server">
					<span class="legenda-magazzino-lotto-attivo">A</span><span>Lotto attivo</span>
				</span> 
				<span class="legenda-magazzino-scorta-nulla">O</span><span>Scorta nulla</span>
				<span class="legenda-magazzino-scorta-insufficiente">I</span><span>Scorta insufficiente</span>
				<span class="legenda-magazzino-lotto-centro">C</span><span>Lotto Centro Vaccinale</span>
			</div>
            
            <dyp:DynamicPanel ID="dypScroll1" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">

                <asp:datagrid id="dgrLotti" runat="server" CssClass="datagrid" Width="100%" CellPadding="2" AutoGenerateColumns="False" AllowSorting="True" GridLines="Both">
					<SelectedItemStyle Font-Bold="True" Wrap="False" CssClass="selected"></SelectedItemStyle>
					<EditItemStyle Wrap="False"></EditItemStyle>
					<AlternatingItemStyle Wrap="False" CssClass="alternating"></AlternatingItemStyle>
					<ItemStyle Wrap="False" CssClass="item"></ItemStyle>
					<HeaderStyle Font-Bold="True" CssClass="header"></HeaderStyle>
					<PagerStyle CssClass="pager" Mode="NumericPages"></PagerStyle>
					<Columns>
						<asp:ButtonColumn CommandName="Stampa" Text="&lt;img title=&quot;Stampa&quot; src=&quot;../../images/stampa.gif&quot;&gt;" >
							<HeaderStyle Width="1%"></HeaderStyle>
						</asp:ButtonColumn>
						<asp:ButtonColumn CommandName="AttDisLotto" Text="&lt;img title=&quot;Attiva/Disattiva Lotto&quot; src=&quot;../../images/star.png&quot;&gt;" >
							<HeaderStyle Width="1%"></HeaderStyle>
						</asp:ButtonColumn>
						<asp:ButtonColumn CommandName="Select" Text="&lt;img title=&quot;Visualizza dati e movimenti del lotto&quot; src=&quot;../../images/dettaglio.gif&quot;&gt;" >
							<HeaderStyle Width="1%"></HeaderStyle>
						</asp:ButtonColumn>
						<asp:BoundColumn DataField="CodiceLotto" SortExpression="CodiceLotto" ReadOnly="True" HeaderText="Cod. Lotto&lt;img id=&quot;cl_up&quot; src=&quot;../../images/arrow_up_small.gif&quot;&gt;&lt;img id=&quot;cl_down&quot; src=&quot;../../images/arrow_down_small.gif&quot;&gt;">
							<HeaderStyle HorizontalAlign="Left" Width="12%"></HeaderStyle>
							<ItemStyle HorizontalAlign="Left"></ItemStyle>
						</asp:BoundColumn>
						<asp:BoundColumn DataField="DescrizioneLotto" SortExpression="DescrizioneLotto" ReadOnly="True" HeaderText="Desc. Lotto&lt;img id=&quot;dl_up&quot; src=&quot;../../images/arrow_up_small.gif&quot;&gt;&lt;img id=&quot;dl_down&quot; src=&quot;../../images/arrow_down_small.gif&quot;&gt;">
							<HeaderStyle HorizontalAlign="Left" Width="12%"></HeaderStyle>
							<ItemStyle HorizontalAlign="Left"></ItemStyle>
						</asp:BoundColumn>
						<asp:BoundColumn DataField="DescrizioneNomeCommerciale" SortExpression="DescrizioneNomeCommerciale" ReadOnly="True" HeaderText="Nome Commerciale&lt;img id=&quot;nc_up&quot; src=&quot;../../images/arrow_up_small.gif&quot;&gt;&lt;img id=&quot;nc_down&quot; src=&quot;../../images/arrow_down_small.gif&quot;&gt;">
							<HeaderStyle HorizontalAlign="Left" Width="29%"></HeaderStyle>
							<ItemStyle HorizontalAlign="Left"></ItemStyle>
						</asp:BoundColumn>
						<asp:TemplateColumn SortExpression="DataPreparazione" HeaderText="Data Preparazione&lt;img id=&quot;dp_up&quot; src=&quot;../../images/arrow_up_small.gif&quot;&gt;&lt;img id=&quot;dp_down&quot; src=&quot;../../images/arrow_down_small.gif&quot;&gt;" >
							<HeaderStyle HorizontalAlign="Center" Width="12%"></HeaderStyle>
							<ItemStyle HorizontalAlign="Center"></ItemStyle>
							<ItemTemplate>
								<asp:Label id="lblDataPreparazione" runat="server" ></asp:Label>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn SortExpression="DataScadenza" HeaderText="Data Scadenza&lt;img id=&quot;ds_up&quot; src=&quot;../../images/arrow_up_small.gif&quot;&gt;&lt;img id=&quot;ds_down&quot; src=&quot;../../images/arrow_down_small.gif&quot;&gt;" >
							<HeaderStyle HorizontalAlign="Center" Width="12%"></HeaderStyle>
							<ItemStyle HorizontalAlign="Center"></ItemStyle>
							<ItemTemplate>
								<asp:Label id="lblDataScadenza" runat="server" ></asp:Label>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:BoundColumn DataField="DosiRimaste" SortExpression="DosiRimaste" ReadOnly="True" HeaderText="Dosi&lt;img id=&quot;dr_up&quot; src=&quot;../../images/arrow_up_small.gif&quot;&gt;&lt;img id=&quot;dr_down&quot; src=&quot;../../images/arrow_down_small.gif&quot;&gt;">
							<HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
							<ItemStyle HorizontalAlign="Right"></ItemStyle>
						</asp:BoundColumn>
						<asp:TemplateColumn>
							<HeaderStyle Width="2%"></HeaderStyle>
							<HeaderTemplate>
								&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
							</HeaderTemplate>
							<ItemTemplate>
								<asp:Label id="lb_null" runat="server" CssClass="legenda-magazzino-scorta-nulla" Visible="False">0</asp:Label>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn>
							<HeaderStyle Width="2%"></HeaderStyle>
                            <HeaderTemplate>
								&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
							</HeaderTemplate>
							<ItemTemplate>
								<asp:Label id="lb_alert" runat="server" cssclass="legenda-magazzino-scorta-insufficiente" Visible="False">I</asp:Label>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn SortExpression="CodiceConsultorio" HeaderText="&lt;span class=&quot;legenda-magazzino-lotto-centro&quot;&gt;&#160;C&#160;&lt;/span&gt;&lt;img id=&quot;cn_up&quot; src=&quot;../../images/arrow_up_small.gif&quot;&gt;&lt;img id=&quot;cn_down&quot; src=&quot;../../images/arrow_down_small.gif&quot;&gt;">
							<HeaderStyle Width="2%"></HeaderStyle>
							<ItemTemplate>
                                <asp:Label id="lb_cns" runat="server" cssclass="legenda-magazzino-lotto-centro" Visible="False">&nbsp;C&nbsp;</asp:Label>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn SortExpression="Attivo" HeaderText="&lt;span class=&quot;legenda-magazzino-lotto-attivo&quot;&gt;&#160;A&#160;&lt;/span&gt;&lt;img id=&quot;at_up&quot; src=&quot;../../images/arrow_up_small.gif&quot;&gt;&lt;img id=&quot;at_down&quot; src=&quot;../../images/arrow_down_small.gif&quot;&gt;">
							<HeaderStyle Width="2%"></HeaderStyle>
							<ItemTemplate>
                                <asp:Label id="lb_att" runat="server" cssclass="legenda-magazzino-lotto-attivo" Visible="False">&nbsp;A&nbsp;</asp:Label>
							</ItemTemplate>
						</asp:TemplateColumn>

						<asp:BoundColumn DataField="DataPreparazione" Visible="False"></asp:BoundColumn>
						<asp:BoundColumn DataField="DataScadenza" Visible="False"></asp:BoundColumn>
						<asp:BoundColumn DataField="CodiceConsultorio" Visible="False"></asp:BoundColumn>
						<asp:BoundColumn DataField="Attivo" Visible="False"></asp:BoundColumn>
						<asp:BoundColumn DataField="CodiceNomeCommerciale" Visible="False"></asp:BoundColumn>
						<asp:BoundColumn DataField="QuantitaMinima" Visible="False"></asp:BoundColumn>
						<asp:BoundColumn DataField="Obsoleto" Visible="False"></asp:BoundColumn>
							    
					</Columns>
				</asp:datagrid>

            </dyp:DynamicPanel>

            <onitcontrols:OnitFinestraModale ID="modAttivazioneLotto" Title="Attivazione Lotto" runat="server" Width="400px" Height="250px" 
                BackColor="LightGray" NoRenderX="true" RenderModalNotVisible="false" ClientEventProcs-OnShow="setFocusIfNedeed()">
                <table border="0" cellpadding="2" cellspacing="0" width="100%">
                    <colgroup>
                        <col width="40%" />
                        <col width="20%" />
                        <col width="40%" />
                    </colgroup>
                    <tr>
                        <td colspan="3" style="text-align:center; padding:15px;">
                            <div style="border:1px solid navy; background-color:#f5f5f5;  margin:auto;">
                                <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                    <colgroup>
                                        <col width="25%" />
                                        <col width="75%" />
                                    </colgroup>
                                    <tr>
                                        <td style="vertical-align:middle; padding-top:5px; text-align:center;" rowspan="3">
                                            <img src="../../Images/disk_blue_48.gif" alt="Salvataggio" style="text-align:center; " />
                                        </td>
                                        <td style="vertical-align:middle; padding-top:5px;">
                                            <asp:Label ID="lblWarningOperazione" runat="server" CssClass="saveAlertLabel"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="vertical-align:top;">
                                            <asp:Label ID="lblCodiceLotto" runat="server" CssClass="saveInfoLabel" Width="100%" ></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="vertical-align:top;">
                                            <asp:Label ID="lblDescrizioneLotto" runat="server" CssClass="saveInfoLabel" Width="100%" ></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="saveInfoLabelRight">Eta minima</td>
                                        <td>
                                            <ucEtaAttivazione:CampiEtaAttivazione ID="ucEtaMinAttivazione" runat="server" LabelCssClass="saveInfoLabel" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="saveInfoLabelRight">Eta massima</td>
                                        <td>
                                            <ucEtaAttivazione:CampiEtaAttivazione ID="ucEtaMaxAttivazione" runat="server" LabelCssClass="saveInfoLabel" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" class="saveAlertLabel" style="text-align:center; vertical-align:middle; padding-top:10px; padding-bottom:10px">
                                            L'operazione non potrà essere annullata:<br \>continuare con il salvataggio su database?
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:Button ID="btnSalvaAttivazioneLotto" runat="server" Text="Salva" CssClass="saveButton" />
                        </td>
                        <td></td>
                        <td>
                            <asp:Button ID="btnAnnullaAttivazioneLotto" runat="server" Text="Annulla" CssClass="saveButton" />
                        </td>
                    </tr>
                    <tr style="height:10px">
                        <td colspan="3"></td>
                    </tr>
                </table>
                <asp:TextBox ID="txtCodiceLottoDaAttivare" runat="server" style="display:none"></asp:TextBox>
                <asp:TextBox ID="txtFlagAttivazioneLotto" runat="server" style="display:none"></asp:TextBox>
            </onitcontrols:OnitFinestraModale>

        </on_lay3:OnitLayout3>
        <script type="text/javascript">
            function setFocusIfNedeed() {
                var idTxtEtaMinAnni = '<%= Me.ucEtaMinAttivazione.GetClientIdCampoAnno()%>';
                if (document.getElementById(idTxtEtaMinAnni) != null) document.getElementById(idTxtEtaMinAnni).focus();
            }
        </script>
    </form>
</body>
</html>
