<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DettaglioLotto.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.DettaglioLotto" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<%@ Register TagPrefix="uc1" TagName="InsDatiLotto" Src="../../Common/Controls/InsDatiLotto.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html xmlns="http://www.w3.org/1999/xhtml" >

<head id="Head1" runat="server">
    <title>Dettaglio Lotto</title>
    
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript" src="../Magazzino.js"></script>
</head>
<body>
    <form id="form1" runat="server">

        <script type="text/javascript">
            function ToolBarClick(ToolBar, button, evnt) {
                evnt.needPostBack = true;

                switch (button.Key) {
                    case 'btnSalva':
                        var errorMessage = ControlloDatiLotto('<%=ucDatiLotto.codLottoClientID%>', '<%=ucDatiLotto.descLottoClientID%>', '<%=ucDatiLotto.codNCClientID%>', '<%=ucDatiLotto.dataPreparazioneClientID%>', '<%=ucDatiLotto.dataScadenzaClientID%>', '<%=ucDatiLotto.dosiScatolaClientID%>', '<%=ucDatiLotto.qtaInizialeClientID%>', '<%=ucDatiLotto.qtaMinimaClientID%>', '<%=ucDatiLotto.fornitoreClientID%>', '<%=ucDatiLotto.noteClientID%>');

                        if (errorMessage != "") {
                            alert("Salvataggio non effettuato.\n" + errorMessage);
                            evnt.needPostBack = false;
                        }
                        else {
                            // Controllo data scadenza
                            if (!ControlloScadenzaLotto('<%=ucDatiLotto.dataScadenzaClientID%>')) {
                                evnt.needPostBack = false;
                            }
                        }
                        break;

                    case 'btnAnnulla':
                        if (!confirm("Attenzione: le modifiche non salvate verranno perse. Continuare?")) {
                            evnt.needPostBack = false;
                        }
                        break;
                    }
            }
        </script>

        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" TitleCssClass="Title3" Height="100%" Width="100%" Titolo="Magazzino Lotti">
            <div>
				<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="110px" CssClass="infratoolbar">
					<DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
					<SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                    <Items>
                        <igtbar:TBarButton Key="btnIndietro" Text="Indietro" >
                        </igtbar:TBarButton>
                        <igtbar:TBSeparator />
                        <igtbar:TBarButton Key="btnModifica" Text="Modifica" DisabledImage="~/Images/modifica_dis.gif" Image="~/Images/modifica.gif">
                        </igtbar:TBarButton>							    
                        <igtbar:TBarButton Key="btnMovimenti" Text="Carico/Scarico" DisabledImage="~/Images/nuovo_dis.gif" Image="~/Images/nuovo.gif">
                        </igtbar:TBarButton>
                        <igtbar:TBSeparator />							    
                        <igtbar:TBarButton Key="btnSalva" Text="Salva" >
                        </igtbar:TBarButton>							    
                        <igtbar:TBarButton Key="btnAnnulla" Text="Annulla" >
                        </igtbar:TBarButton>
                    </Items>						
				</igtbar:UltraWebToolbar>
            </div>
			<div class="vac-sezione">
				<asp:Label id="Label1" runat="server">DATI LOTTO</asp:Label>
			</div>
            <div>
				<uc1:InsDatiLotto id="ucDatiLotto" runat="server"></uc1:InsDatiLotto>
            </div>
			<div class="vac-sezione">
				<asp:Label id="Label2" runat="server">ELENCO MOVIMENTI</asp:Label>
			</div>

            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
				<asp:datagrid id="dgrMovimenti" runat="server" BackColor="White" CssClass="datagrid"
					Width="100%" CellPadding="2" GridLines="None" AutoGenerateColumns="False" 
					AllowPaging="True" AllowCustomPaging="true" >
					<SelectedItemStyle Font-Bold="True" CssClass="Selected"></SelectedItemStyle>
					<AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
					<ItemStyle CssClass="item"></ItemStyle>
					<HeaderStyle Font-Bold="True" CssClass="header"></HeaderStyle>
					<PagerStyle CssClass="pager" Mode="NumericPages" ></PagerStyle>
					<Columns>
						<asp:TemplateColumn HeaderText="Data">
							<HeaderStyle HorizontalAlign="Center" Width="15%"></HeaderStyle>
							<ItemStyle HorizontalAlign="Center"></ItemStyle>
							<ItemTemplate>
								<asp:Label id="lblDataRegistrazione" runat="server" >
								</asp:Label>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:TextBox id="txtDataRegistrazione" runat="server" Width="100%" CssClass="TextBox_Data_Disabilitato" ReadOnly="True">
								</asp:TextBox>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Quantit&#224;">
							<HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
							<ItemStyle HorizontalAlign="Center"></ItemStyle>
							<ItemTemplate>
								<asp:Label id="lblQuantita" runat="server" >
								</asp:Label>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:TextBox id="txtQuantita" runat="server" Width="100%" CssClass="TextBox_Numerico_Obbligatorio"></asp:TextBox>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Movimento">
							<HeaderStyle HorizontalAlign="Left" Width="16%"></HeaderStyle>
							<ItemStyle HorizontalAlign="Left"></ItemStyle>
							<ItemTemplate>
								<asp:Label id="lblTipoMovimento" runat="server" >
								</asp:Label>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:DropDownList id="ddlTipoMovimento" runat="server" CssClass="TextBox_Stringa_Obbligatorio" Width="100%"
									onchange="AbilitaTrasferimento(event,this);">
									<asp:ListItem Selected="True" Value=""></asp:ListItem>
									<asp:ListItem Value="C">CARICO</asp:ListItem>
									<asp:ListItem Value="S">SCARICO</asp:ListItem>
									<asp:ListItem Value="A">TRASFERIMENTO A</asp:ListItem>
								</asp:DropDownList>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Luogo di Trasferimento">
							<HeaderStyle Width="25%" />
							<ItemTemplate>
								<asp:Label id="lblLuogoTrasferimento" runat="server" >
								</asp:Label>
							</ItemTemplate>
							<EditItemTemplate>
								<onitcontrols:onitmodallist id="fmLuogoTrasferimento" runat="server"
									Width="69%" Obbligatorio="True" PosizionamentoFacile="False" LabelWidth="-1px" SetUpperCase="True" IsDistinct="true"
									UseCode="True" Tabella="T_ANA_CONSULTORI,T_ANA_LINK_UTENTI_CONSULTORI" CampoDescrizione="CNS_DESCRIZIONE Descrizione" CampoCodice="CNS_CODICE Codice"
									CodiceWidth="30%" Label="Titolo" Filtro="CNS_CNS_MAGAZZINO IS NULL"></onitcontrols:onitmodallist>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Operatore">
							<HeaderStyle HorizontalAlign="Left" Width="7%"></HeaderStyle>
							<ItemStyle HorizontalAlign="Left"></ItemStyle>
							<ItemTemplate>
								<onitcontrols:onitmodallist id="fmOperatore" runat="server" BackColor="Transparent" BorderStyle="None" Width="98%" RaiseChangeEvent="False" BorderColor="Transparent" CodiceWidth="0px" CampoCodice="ute_id" CampoDescrizione="ute_descrizione" Tabella="v_ana_utenti" UseCode="False" SetUpperCase="True" LabelWidth="-1px" PosizionamentoFacile="False" Obbligatorio="False" LikeMode="Right" Sorting="False" UseAllResultCodeIfEqual="False" UseAllResultDescIfEqual="False" DataTypeCode="Stringa" Paging="False" IsDistinct="False" DataTypeDescription="Stringa" UseTableLayout="False">
								</onitcontrols:onitmodallist>
							</ItemTemplate>
							<EditItemTemplate>
								<onitcontrols:onitmodallist id="fmOperatoreEdit" runat="server" Width="98%" Enabled="False" RaiseChangeEvent="False" CodiceWidth="0px" CampoCodice="ute_id" CampoDescrizione="ute_descrizione" Tabella="v_ana_utenti" UseCode="False" SetUpperCase="True" LabelWidth="-1px" PosizionamentoFacile="False" Obbligatorio="False">
								</onitcontrols:onitmodallist>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn Visible="False" HeaderText="Unit&#224;">
							<HeaderStyle HorizontalAlign="Center" Width="3%"></HeaderStyle>
							<ItemStyle HorizontalAlign="Left"></ItemStyle>
							<EditItemTemplate>
								<div style="width: 80px;">
									<asp:RadioButton id="rbtScatole" tabIndex="20" runat="server" CssClass="label" Text="Scatole" GroupName="unita"></asp:RadioButton>
									<br />
									<asp:RadioButton id="rbtDosi" tabIndex="21" runat="server" CssClass="label" Text="Dosi" GroupName="unita" Checked="True"></asp:RadioButton>
								</div>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Note">
							<HeaderStyle HorizontalAlign="Left" Width="27%"></HeaderStyle>
							<ItemStyle HorizontalAlign="Left"></ItemStyle>
							<ItemTemplate>
								<asp:Label id="lblNote" runat="server" >
								</asp:Label>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:TextBox id="txtNote" runat="server" Height="21px" CssClass="TextBox_Stringa" Width="100%"
									MaxLength="250"></asp:TextBox>
							</EditItemTemplate>
						</asp:TemplateColumn>
								
						<asp:BoundColumn DataField="DataRegistrazione" Visible="False"></asp:BoundColumn>
							    
					</Columns>
				</asp:datagrid>
            </dyp:DynamicPanel>
		</on_lay3:OnitLayout3>
    </form>
    
</body>

</html>
