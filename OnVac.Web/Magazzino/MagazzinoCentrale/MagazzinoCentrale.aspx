<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="MagazzinoCentrale.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.MagazzinoCentrale" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<%@ Register TagPrefix="ucFiltri" TagName="FiltriRicercaMagazzino" Src="../FiltriRicercaMagazzino.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Ricerca lotti magazzino centrale</title> 

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

        function FocusCampi() {
            var txt;

            txt = document.getElementById('txtQtaMovimento')
            if (txt != null) {
                txt.focus();
                return;
            }

            txt = document.getElementById('txtDosiLottoConsultorio')
            if (txt != null) {
                txt.focus();
                return;
            }
        }

        function checkModaleMovimento(evt) {
            return checkModale('<%= fmMagazzinoDestinazioneMovimento.ClientId %>', evt);
        }

        function checkModaleLottoConsultorio(evt) {
            return checkModale('<%= fmMagazzinoLottoConsultorio.ClientId %>', evt);
        }

        function checkModale(idModale, evt) {
            if (!isValidFinestraModale(idModale, false)) {
                // Funzione presente nello script utility.js già incluso dai controlli
                StopPreventDefault(evt);
                return false;
            }
            return true;
        }
    </script>
    <style type ="text/css">
        .dgrLottiItem {
            margin-top: 2px;
            vertical-align: top;
            margin-bottom: 2px;
        }

        .datagridDettaglio {
            border: 1px solid navy;
            font-family: Arial;
            font-size: 12px;
            margin-top: 5px;
            margin-bottom: 5px;
        }

        tr.headerDettaglio td, tr.headerDettaglio a {
            font-weight: bold;
            color: navy;
            background-color: #B0C4DE;
            height: 20px;
            padding-top: 2px;
        }

        tr.itemDettaglio td {
            background-color: #F5F5F5;
            height: 22px;
        }

        tr.alternatingDettaglio td {
            background-color: #e7e7ff;
            height: 22px;
        }

        .styleCursorButton {
            cursor: pointer;
        }

    </style>
</head>
<body onload="FocusCampi()">
    <form id="form1" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" TitleCssClass="Title3" Height="100%" Width="100%" Titolo="Magazzino Centrale">
			<div class="title" id="PanelTitolo" runat="server">
				<asp:Label id="LayoutTitolo" runat="server" Text="Magazzino Centrale"></asp:Label>
			</div>
            <div>
				<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="90px" CssClass="infratoolbar">
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
				<ucFiltri:FiltriRicercaMagazzino ID="ucFiltriRicerca" ShowFiltroDistretti="true" runat="server" />
			</div>
			<div class="vac-sezione">
				<asp:Label id="LayoutTitolo_sezione" runat="server">ELENCO LOTTI</asp:Label>
			</div>
			<div id="divLegenda" class="legenda-vaccinazioni">
                <span class="legenda-magazzino-scorta-nulla">O</span><span>Scorta nulla</span>
				<span class="legenda-magazzino-scorta-insufficiente">I</span><span>Scorta insufficiente</span>
			</div>

            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                       
                <asp:datagrid id="dgrLotti" runat="server" Width="100%" CellPadding="2" AutoGenerateColumns="False" 
                    AllowCustomPaging="true" AllowPaging="true" PageSize="25" AllowSorting="True" GridLines="Both">
					<SelectedItemStyle Font-Bold="True" Wrap="False" CssClass="selected"></SelectedItemStyle>
					<EditItemStyle Wrap="False"></EditItemStyle>
					<AlternatingItemStyle Wrap="False" CssClass="alternating dgrLottiItem"></AlternatingItemStyle>
					<ItemStyle Wrap="False" CssClass="item dgrLottiItem"></ItemStyle>
					<HeaderStyle Font-Bold="True" CssClass="header"></HeaderStyle>
                    <PagerStyle CssClass="pager" Position="TopAndBottom" Mode="NumericPages"  />
					<Columns>
                        <asp:TemplateColumn>
                            <HeaderStyle Width="1%"></HeaderStyle>
                            <ItemStyle VerticalAlign="Top"></ItemStyle>
							<ItemTemplate>
                                <asp:ImageButton ID="btnDettaglioDosi" runat="server" ImageUrl="~/Images/piu.gif" ToolTip="Mostra i dettagli delle dosi per magazzino" CommandName="DettaglioDosi" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
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
						<asp:TemplateColumn SortExpression="DescrizioneNomeCommerciale" HeaderText="Nome Commerciale&lt;img id=&quot;nc_up&quot; src=&quot;../../images/arrow_up_small.gif&quot;&gt;&lt;img id=&quot;nc_down&quot; src=&quot;../../images/arrow_down_small.gif&quot;&gt;">
                            <HeaderStyle HorizontalAlign="Left" Width="41%"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Top"></ItemStyle>
							<ItemTemplate>
								<asp:Label ID="lblDescrizioneNomeCommerciale" runat="server" ></asp:Label>
                                <asp:Panel ID="panelDettaglio" runat="server" >
                                    <asp:DataGrid id="dgrDettaglio" runat="server" Width="100%" CssClass="datagridDettaglio"
                                        AutoGenerateColumns="False" AllowSorting="True" GridLines="Both" >
										<AlternatingItemStyle CssClass="alternatingDettaglio"></AlternatingItemStyle>
										<ItemStyle CssClass="itemDettaglio"></ItemStyle>
										<HeaderStyle CssClass="headerDettaglio"></HeaderStyle>
										<Columns>
                                            <asp:TemplateColumn>
                                                <HeaderStyle Width="3%"></HeaderStyle>
                                                <HeaderTemplate>
                                                    <asp:ImageButton ID="btnNuovoLottoConsultorio" runat="server" ImageUrl="../../images/nuovoLottoCns.gif" ToolTip="Caricamento del lotto in un magazzino" CommandName="InsertLottoConsultorio"  />
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="btnAddMovimento" runat="server" ImageUrl="../../images/registrazionevaccinazioni.gif" ToolTip="Movimenti del lotto nel magazzino" CommandName="InsertMovimento" />
                                                </ItemTemplate>
                                            </asp:TemplateColumn>

                                            <asp:TemplateColumn SortExpression="DescrizioneConsultorio">
                                                <HeaderStyle Width="53%"></HeaderStyle>
                                                <HeaderTemplate>
                                                    <asp:LinkButton ID="lblDescrizioneConsultorioDettaglioHeader" runat="server" Text="Magazzino" 
                                                                    CommandName="SortDettaglio" CommandArgument="DescrizioneConsultorio"></asp:LinkButton>
                                                    <asp:Image ID="mag_up" runat="server" ImageUrl="../../images/arrow_blue_up.png" />
                                                    <asp:Image ID="mag_down" runat="server" ImageUrl="../../images/arrow_blue_down.png" />
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDescrizioneConsultorioDettaglioItem" runat="server" ></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>

                                            <asp:TemplateColumn SortExpression="DosiRimaste">
                                                <HeaderStyle Width="15%" HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" />
                                                <HeaderTemplate>
                                                    <asp:LinkButton ID="lblDosiRimasteDettaglioHeader" runat="server" Text="Dosi" 
                                                                    CommandName="SortDettaglio" CommandArgument="DosiRimaste"></asp:LinkButton>
                                                    <asp:Image ID="dosi_up" runat="server" ImageUrl="../../images/arrow_blue_up.png" />
                                                    <asp:Image ID="dosi_down" runat="server" ImageUrl="../../images/arrow_blue_down.png" />
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDosiRimasteDettaglioItem" runat="server" ></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
                                                    
                                            <asp:TemplateColumn SortExpression="QuantitaMinima">
                                                <HeaderStyle Width="15%" HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" />
                                                <HeaderTemplate>
                                                    <asp:LinkButton ID="lblQuantitaMinimaDettaglioHeader" runat="server" Text="Soglia" 
                                                                    CommandName="SortDettaglio" CommandArgument="QuantitaMinima"></asp:LinkButton>
                                                    <asp:Image ID="qmin_up" runat="server" ImageUrl="../../images/arrow_blue_up.png" />
                                                    <asp:Image ID="qmin_down" runat="server" ImageUrl="../../images/arrow_blue_down.png" />
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblQuantitaMinimaDettaglioItem" runat="server" ></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>


							                <asp:TemplateColumn SortExpression="Attivo" >
								                <HeaderStyle Width="15%" HorizontalAlign="Center" />
								                <ItemStyle HorizontalAlign="Center" />
                                                <HeaderTemplate>
                                                    <asp:LinkButton ID="lblLottoAttivoDettaglioHeader" runat="server" Text="Attivo" 
                                                                    CommandName="SortDettaglio" CommandArgument="Attivo"></asp:LinkButton>
                                                    <asp:Image ID="att_up" runat="server" ImageUrl="../../images/arrow_blue_up.png" />
                                                    <asp:Image ID="att_down" runat="server" ImageUrl="../../images/arrow_blue_down.png" />
                                                </HeaderTemplate>
								                <ItemTemplate>
									                <asp:Label id="lblLottoAttivoDettaglioItem" runat="server" ></asp:Label>
								                </ItemTemplate>
							                </asp:TemplateColumn>
                                                
											<asp:BoundColumn DataField="CodiceConsultorio" Visible="False" ></asp:BoundColumn>
											<asp:BoundColumn DataField="CodiceLotto" Visible="False" ></asp:BoundColumn>
                                            <asp:BoundColumn DataField="DescrizioneLotto" Visible="False" ></asp:BoundColumn>

										</Columns>
									</asp:DataGrid>
								</asp:Panel>
							</ItemTemplate>
						</asp:TemplateColumn>
                        <asp:BoundColumn DataField="DosiRimaste" SortExpression="DosiRimaste" ReadOnly="True" HeaderText="Dosi&lt;img id=&quot;dr_up&quot; src=&quot;../../images/arrow_up_small.gif&quot;&gt;&lt;img id=&quot;dr_down&quot; src=&quot;../../images/arrow_down_small.gif&quot;&gt;">
							<HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
							<ItemStyle HorizontalAlign="Center"></ItemStyle>
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

						<asp:BoundColumn DataField="DataPreparazione" Visible="False"></asp:BoundColumn>
                        <asp:BoundColumn DataField="DataScadenza" Visible="False"></asp:BoundColumn>
						<asp:BoundColumn DataField="CodiceNomeCommerciale" Visible="False"></asp:BoundColumn>
						<asp:BoundColumn DataField="QuantitaMinima" Visible="False"></asp:BoundColumn>
						<asp:BoundColumn DataField="Obsoleto" Visible="False"></asp:BoundColumn>
							    
                    </Columns>
                </asp:datagrid>
            </dyp:DynamicPanel>

            <onitcontrols:OnitFinestraModale ID="modMovimentoLotto" Title="Inserimento movimento" runat="server"
                Width="500px" BackColor="LightGray" NoRenderX="true">
                
                <table cellpadding="2" cellspacing="0" width="100%" style="border:1px solid navy; background: WhiteSmoke; margin-top:3px;">
                    <colgroup>
                        <col width="2%" />
                        <col width="96%" />
                        <col width="2%" />
                    </colgroup>
                    <tr>
                        <td></td>
                        <td style="font-weight:bold">
                            <asp:Label ID="lblMagazzinoMovimento" runat="server" CssClass="TextBox_Stringa"></asp:Label>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td style="font-weight:bold">
                            <asp:Label ID="lblLottoMovimento" runat="server" CssClass="TextBox_Stringa"></asp:Label>
                        </td>
                        <td></td>
                    </tr>
                </table>

                <table cellpadding="2" cellspacing="0" width="100%" border="0">
                    <colgroup>
                        <col width="2%" />
                        <col width="15%" />
                        <col width="30%" />
                        <col width="4%" />
                        <col width="9%" />
                        <col width="1%" />
                        <col width="15%" />
                        <col width="22%" />
                        <col width="2%" />
                    </colgroup>
                    <tr style="height:10px">
                        <td colspan="9"></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td class="Label">
                            <asp:Label ID="lblQtaMovimento" runat="server"  Text="Quantità" ></asp:Label>
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="txtQtaMovimento" runat="server" CssClass="TextBox_Numerico_Obbligatorio" Width="100%" MaxLength="8"></asp:TextBox>
                        </td>
                        <td colspan="4"></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td class="Label">
                            <asp:Label ID="Label3" runat="server"  Text="Movimento" ></asp:Label>
                        </td>
                        <td colspan="3">
                            <asp:DropDownList id="ddlTipoMovimento" runat="server" CssClass="TextBox_Stringa_Obbligatorio" Width="100%" AutoPostBack="true">
								<asp:ListItem Selected="True" Value=""></asp:ListItem>
								<asp:ListItem Value="C">CARICO</asp:ListItem>
								<asp:ListItem Value="S">SCARICO</asp:ListItem>
								<asp:ListItem Value="A">TRASFERIMENTO A</asp:ListItem>
							</asp:DropDownList>
                        </td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td  class="Label">
                            <asp:Label ID="Label5" runat="server" Text="Destinazione" ></asp:Label>
                        </td>
                        <td colspan="6">
                            <onitcontrols:onitmodallist id="fmMagazzinoDestinazioneMovimento" runat="server" enabled="false"
                                Width="69%" Obbligatorio="True" PosizionamentoFacile="False" LabelWidth="-1px" SetUpperCase="True" IsDistinct="true"
                                UseCode="True" Tabella="T_ANA_CONSULTORI,T_ANA_LINK_UTENTI_CONSULTORI" CampoDescrizione="CNS_DESCRIZIONE Descrizione" CampoCodice="CNS_CODICE Codice"
								CodiceWidth="30%" Label="Titolo"></onitcontrols:onitmodallist>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td valign="top" class="Label">
                            <asp:Label ID="Label6" runat="server"  Text="Note" ></asp:Label>
                        </td>
                        <td colspan="6">
                            <asp:TextBox id="txtNoteMovimento" runat="server" CssClass="TextBox_Stringa" Width="100%" 
                                         TextMode="MultiLine" Rows="3" MaxLength="250" style="overflow-y:auto; resize:none;"></asp:TextBox>
                        </td>
                        <td></td>
                    </tr>
                    <tr style="height:10px">
                        <td colspan="9">
                            <asp:HiddenField ID="txtCodiceLottoMovimento" runat="server" />
                            <asp:HiddenField ID="txtDescrizioneLottoMovimento" runat="server" />
                            <asp:HiddenField ID="txtCodiceConsultorioMovimento" runat="server" />
                            <asp:HiddenField ID="txtDescrizioneConsultorioMovimento" runat="server" />
                        </td>
                    </tr>
                </table>

                <table cellpadding="2" cellspacing="0" width="100%" border="0" style="margin-bottom:10px;">
                    <colgroup>
                        <col width="45%" />
                        <col width="10%" />
                        <col width="45%" />
                    </colgroup>
                    <tr>
                        <td align="right" >
                            <asp:Button ID="btnSalvaMovimento" runat="server" Text="Salva" Width="80px" CssClass="styleCursorButton" ToolTip="Inserimento del movimento per il lotto nel centro vaccinale selezionato" OnClientClick="checkModaleMovimento(event)" />
                        </td>
                        <td></td>
                        <td>
                            <asp:Button ID="btnChiudiModaleMovimento" runat="server" Text="Annulla" Width="80px" CssClass="styleCursorButton" ToolTip="Chiude la pop-up senza inserire il movimento specificato" />
                        </td>
                    </tr>
                </table>

            </onitcontrols:OnitFinestraModale>

            <onitcontrols:OnitFinestraModale ID="modInserimentoLottoConsultorio" Title="Inserimento lotto nel centro vaccinale" runat="server"
                Width="500px" Height="200px" BackColor="LightGray" NoRenderX="true">

                <table cellpadding="2" cellspacing="0" width="100%" style="border:1px solid navy; background: WhiteSmoke; margin-top:3px;">
                    <colgroup>
                        <col width="2%" />
                        <col width="96%" />
                        <col width="2%" />
                    </colgroup>
                    <tr style="padding-top:10px; padding-bottom:10px">
                        <td></td>
                        <td class="Label_left" style="font-weight:bold">
                            <asp:Label ID="lblLottoConsultorio" runat="server" CssClass="TextBox_Stringa"></asp:Label>
                        </td>
                        <td></td>
                    </tr>
                </table>

                <table cellpadding="2" cellspacing="0" width="100%" border="0" style="margin-top:10px">
                    <colgroup>
                        <col width="2%" />
                        <col width="20%" />
                        <col width="28%" />
                        <col width="20%" />
                        <col width="28%" />
                        <col width="2%" />
                    </colgroup>
                    <tr>
                        <td></td>
                        <td class="label" >
                            <asp:Label ID="lblDosiLottoConsultorio" runat="server" Text="Numero dosi"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDosiLottoConsultorio" runat="server" MaxLength="8" CssClass="TextBox_Numerico_Obbligatorio" Width="100%"></asp:TextBox>
                        </td>
                        <td class="label">
                            <asp:Label ID="lblQtaMinimaLottoConsultorio" runat="server" Text="Soglia minima"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtQtaMinimaLottoConsultorio" runat="server" MaxLength="8" CssClass="TextBox_Numerico" Width="100%"></asp:TextBox>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td class="label">
                            <asp:Label ID="lblMagazzinoLottoConsultorio" runat="server" Text="Magazzino"></asp:Label>
                        </td>
                        <td colspan="3">
                            <onitcontrols:onitmodallist id="fmMagazzinoLottoConsultorio" runat="server"
                                Width="69%" Obbligatorio="True" PosizionamentoFacile="False" LabelWidth="-1px" SetUpperCase="True"
                                UseCode="True" Tabella="T_ANA_CONSULTORI" CampoDescrizione="CNS_DESCRIZIONE Descrizione" CampoCodice="CNS_CODICE Codice"
								CodiceWidth="30%" Label="Titolo"></onitcontrols:onitmodallist>                            
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <asp:HiddenField ID="txtCodiceLottoConsultorio" runat="server" />
                            <asp:HiddenField ID="txtDescrizioneLottoConsultorio" runat="server" />
                        </td>
                    </tr>
                </table>

                <table cellpadding="2" cellspacing="0" width="100%" border="0" style="margin-top: 20px;margin-bottom: 10px;">
                    <colgroup>
                        <col width="45%" />
                        <col width="10%" />
                        <col width="45%" />
                    </colgroup>
                    <tr>
                        <td align="right" >
                            <asp:Button ID="btnSalvaLottoConsultorio" runat="server" Text="Salva" CssClass="styleCursorButton" Width="80px" ToolTip="Inserimento del lotto nel centro vaccinale selezionato" OnClientClick="checkModaleLottoConsultorio(event)" />
                        </td>
                        <td></td>
                        <td>
                            <asp:Button ID="btnAnnullaLottoConsultorio" runat="server" Text="Annulla" CssClass="styleCursorButton" Width="80px" ToolTip="Chiude la pop-up senza inserire il lotto specificato nel centro vaccinale" />
                        </td>
                    </tr>
                </table>

            </onitcontrols:OnitFinestraModale>

        </on_lay3:OnitLayout3>
    </form>
</body>
</html>
