<%@ Page Language="vb" AutoEventWireup="false" Codebehind="VacEseguitePS.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.VacEseguitePS" %>

<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_ocb" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitCombo" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>

<%@ Register TagPrefix="uc1" TagName="ReazAvverseDetail" Src="../common/Controls/ReazAvverseDetail.ascx" %>

<%@ Import namespace="Onit.OnAssistnet.OnVac" %>
<%@ Import Namespace="Onit.OnAssistnet.OnVac.OnVacUtility" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>Vac Eseguite PS</title>		
		<style type="text/css">
        .legenda_scaduta
        {
            border: #000080 1px solid; 
            font-size: 12px; 
            background-color: Red;
        }
    </style>
		<script type="text/javascript" language="javascript">
			<%= ShowLeftFrameIfNeeded() %>
		</script>
		
        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" Titolo="Vaccinazioni Eseguite" height="100%" width="100%" HeightTitle="90px" TitleCssClass="Title3">
				<div class="title" id="divTitolo">
					<asp:Label id="LayoutTitolo" runat="server" Width="100%"></asp:Label>
                </div>
                <div>
					<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		                <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                        <ClientSideEvents InitializeToolbar="" Click="passa_val_al_server"></ClientSideEvents>
						<Items>
							<igtbar:TBarButton Key="btn_Aggiorna" DisabledImage="~/Images/aggiorna_dis.gif" Text="Aggiorna"
								Image="~/Images/aggiorna.gif"></igtbar:TBarButton>
							<igtbar:TBarButton Key="btn_Indietro" DisabledImage="~/Images/indietro_dis.gif" Text="Indietro"
								Image="~/Images/indietro.gif"></igtbar:TBarButton>
                            <igtbar:TBSeparator></igtbar:TBSeparator>
                            <igtbar:TBarButton Key="btn_Esclusione" Text="Esclusioni" DisabledImage="../images/vaccinazioniescluse18.gif"
                                Image="../images/vaccinazioniescluse18.gif" ToolTip="Visualizza l'elenco delle esclusioni del paziente">
                                <DefaultStyle Width="100px" CssClass="infratoolbar_button_default" ></DefaultStyle>
                            </igtbar:TBarButton>
						    <igtbar:TBarButton Key="btn_CertificatoVaccinale" Text="Certificato Vaccinale" DisabledImage="../images/vaccinazioniescluse.gif"
                                Image="../images/stampa.gif"  ToolTip="Visualizza l'anteprima di stampa del certificato vaccinale del paziente">
                                <DefaultStyle Width="150px" CssClass="infratoolbar_button_default"></DefaultStyle>
                            </igtbar:TBarButton>
						</Items>
					</igtbar:UltraWebToolbar>
                </div>
                <div class="sezione" id="LayoutTitolo_sezione" runat="server">ELENCO VACCINAZIONI</div>
				<div id="divLegenda" style="padding: 5px; border-bottom: #485d96 1px solid; background-color: whitesmoke;">
                    <table>
                        <tr>
                            <td>
                                <img id="legRea" src="../images/reazioniavverse.gif" alt="Reazione avversa" />
                            </td>
                            <td>
                                <asp:Label id="Label19" runat="server" CssClass="label">Reazione avversa associata&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</asp:Label>
                            </td>
                            <td>
                                <asp:Label id="Label12" style="border: 1px solid navy; font-size: 12px; background-color: whitesmoke;"
                                    runat="server" Font-Names="Arial" Font-Bold="True">&nbsp;S&nbsp;</asp:Label>
                            </td>
                            <td>
                                <asp:Label id="Label13" runat="server" CssClass="label">Vaccinazione Scaduta</asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>

                <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                            
                    <asp:Panel id="pan_VacEs" runat="server">
					    <asp:DataGrid id="dg_vacEseguite" runat="server" Width="100%" AutoGenerateColumns="False" CellPadding="1" GridLines="None" AllowSorting="True" >
						    <SelectedItemStyle Font-Bold="True" CssClass="Selected" Height="25px"></SelectedItemStyle>
						    <AlternatingItemStyle CssClass="Alternating" Height="25px"></AlternatingItemStyle>
						    <ItemStyle CssClass="Item" Height="25px"></ItemStyle>
						    <HeaderStyle Font-Bold="True" CssClass="Header"></HeaderStyle>
						    <Columns>
							    <asp:TemplateColumn>
								    <HeaderStyle Width="2%"></HeaderStyle>
								    <ItemTemplate>
									    <asp:ImageButton id="ImageButton1" runat="server" Visible='<%# not DataBinder.Eval(Container, "DataItem")("vra_data_reazione") is DBNull.Value %>' ImageUrl="../images/reazioniavverse.gif" CommandName="ReazioneCmd">
									    </asp:ImageButton>
								    </ItemTemplate>
							    </asp:TemplateColumn>
							    <asp:TemplateColumn SortExpression="vac_descrizione" HeaderText="Vaccinazione &lt;IMG src='../images/arrow_up_small.gif' id='ordDesc'&gt;">
								    <HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
								    <ItemStyle HorizontalAlign="Left"></ItemStyle>
								    <ItemTemplate>
									    <asp:Label id="tb_descVac" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("vac_descrizione") %>'>
									    </asp:Label>
								    </ItemTemplate>
							    </asp:TemplateColumn>
							    <asp:TemplateColumn SortExpression="ves_vac_codice">
								    <HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
								    <ItemStyle HorizontalAlign="Left"></ItemStyle>
								    <ItemTemplate>
									    <asp:Label id="tb_codVac" style="display: none" runat="server" Width="0px" Text='<%# DataBinder.Eval(Container, "DataItem")("ves_vac_codice") %>'>
									    </asp:Label>
								    </ItemTemplate>
							    </asp:TemplateColumn>
							    <asp:TemplateColumn SortExpression="ves_n_richiamo" HeaderText="Dose &lt;IMG src='../images/arrow_up_small.gif' id='ordDosi'&gt;">
								    <HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
								    <ItemStyle HorizontalAlign="Center"></ItemStyle>
								    <ItemTemplate>
									    <asp:Label id="tb_n_rich" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("ves_n_richiamo") %>'>
									    </asp:Label>
								    </ItemTemplate>
							    </asp:TemplateColumn>
							    <asp:TemplateColumn SortExpression="ves_data_effettuazione" HeaderText="Data &lt;IMG src='../images/arrow_up_small.gif' id='ordData'&gt;">
								    <HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
								    <ItemStyle HorizontalAlign="Center"></ItemStyle>
								    <ItemTemplate>
									    <asp:Label id="tb_data_eff" runat="server"  Text='<%# CTYPE(DataBinder.Eval(Container, "DataItem")("ves_data_effettuazione"),DATETIME).tostring("dd/MM/yyyy") %>'>
									    </asp:Label>
								    </ItemTemplate>
							    </asp:TemplateColumn>
							    <asp:TemplateColumn SortExpression="noc_descrizione" HeaderText="Nome &lt;BR&gt;Commerciale &lt;IMG src='../images/arrow_up_small.gif' id='ordNC'&gt;">
								    <HeaderStyle HorizontalAlign="Center" Width="13%"></HeaderStyle>
								    <ItemStyle HorizontalAlign="Left"></ItemStyle>
								    <ItemTemplate>
									    <asp:Label id="tb_nome_com" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("noc_descrizione") %>'>
									    </asp:Label>
								    </ItemTemplate>
							    </asp:TemplateColumn>
							    <asp:TemplateColumn SortExpression="ass_descrizione" HeaderText="Associazione &lt;IMG src='../images/arrow_up_small.gif' id='ordAss'&gt;">
								    <HeaderStyle Width="6%"></HeaderStyle>
								    <ItemTemplate>
									    <asp:Label id="Label2" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("ass_descrizione") %>'>
									    </asp:Label>
								    </ItemTemplate>
							    </asp:TemplateColumn>
							    <asp:TemplateColumn SortExpression="ope_nome" HeaderText="Medico &lt;IMG src='../images/arrow_up_small.gif' id='ordOp'&gt;">
								    <HeaderStyle HorizontalAlign="Center" Width="13%"></HeaderStyle>
								    <ItemStyle HorizontalAlign="Left"></ItemStyle>
								    <ItemTemplate>
									    <asp:Label id="tb_medico" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("ope_nome")%>'>
									    </asp:Label>
								    </ItemTemplate>
							    </asp:TemplateColumn>
							    <asp:TemplateColumn HeaderText="Luogo">
								    <HeaderStyle Width="10%"></HeaderStyle>
								    <ItemTemplate>
									    <asp:Label id="lblLuogo" runat="server"  Text='<%# GetDescrizioneLuogo(DataBinder.Eval(Container, "DataItem")("ves_luogo").ToString) %>'>
									    </asp:Label>
								    </ItemTemplate>
							    </asp:TemplateColumn>
							    <asp:TemplateColumn SortExpression="cns_descrizione" HeaderText="Centro &lt;BR&gt;Vaccinale &lt;IMG src='../images/arrow_up_small.gif' id='ordCNS'&gt;">
								    <HeaderStyle HorizontalAlign="Center" Width="16%"></HeaderStyle>
								    <ItemStyle HorizontalAlign="Left"></ItemStyle>
								    <ItemTemplate>
									    <asp:Label id="tb_cons" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("cns_descrizione") %>'>
									    </asp:Label>
								    </ItemTemplate>
							    </asp:TemplateColumn>
							    <asp:TemplateColumn SortExpression="sii_descrizione" HeaderText="Sito di &lt;BR&gt;Inoculazione &lt;IMG src='../images/arrow_up_small.gif' id='ordSII'&gt;">
								    <HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
								    <ItemStyle HorizontalAlign="Left"></ItemStyle>
								    <ItemTemplate>
									    <asp:Label id="tb_sii" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("sii_descrizione") %>'>
									    </asp:Label>
								    </ItemTemplate>
							    </asp:TemplateColumn>
							    <asp:TemplateColumn>
								    <HeaderStyle HorizontalAlign="Center" Width="2%"></HeaderStyle>
								    <ItemTemplate>
									    <asp:Label id="Label11" runat="server" Font-Names="Arial" style="BORDER-RIGHT: navy 1px solid; BORDER-TOP: navy 1px solid; FONT-SIZE: 12px; BORDER-LEFT: navy 1px solid; BORDER-BOTTOM: navy 1px solid; BACKGROUND-COLOR: whitesmoke; font-weigth: bold" Font-Bold="True" Visible='<%# IIF(DataBinder.Eval(Container, "DataItem")("scaduta").tostring="S",true,false) %>'>&nbsp;S&nbsp;</asp:Label>
								    </ItemTemplate>
							    </asp:TemplateColumn>
						    </Columns>
						    <PagerStyle CssClass="Pager" Mode="NumericPages"></PagerStyle>
					    </asp:DataGrid>
				    </asp:Panel>

                    <asp:Panel id="pan_ReazAvv" runat="server">
                        <uc1:ReazAvverseDetail id="ReazAvverseDetail" runat="server" readonly="true"></uc1:ReazAvverseDetail>								
				    </asp:Panel>

                </dyp:DynamicPanel>                            

			</on_lay3:onitlayout3>

            <on_ofm:OnitFinestraModale ID="modEsclusioni" Title="Elenco vaccinazioni escluse del paziente" runat="server" Width="800px" Height="600px" BackColor="LightGray">
                <div class="sezione"style="width: 100%">
			<asp:Label id="Label1" runat="server" Text="ELENCO VACCINAZIONI ESCLUSE"></asp:Label>
        </div>
		<div id="divLegenda" style="width:100%; height:21px; padding-left: 2px">
			<asp:Label id="Label4" class="legenda_scaduta" runat="server" Font-Names="Arial" Font-Bold="True">&nbsp;S&nbsp;</asp:Label>
			<asp:Label id="Label15" runat="server" CssClass="label"  Font-Size="12px" Font-Names="Arial">Esclusione Scaduta</asp:Label>
        </div>
          <dyp:DynamicPanel ID="DynamicPanel1" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
			<asp:DataGrid id="dg_vacEx" style="padding: 0px;" runat="server" Width="100%" CssClass="datagrid" AutoGenerateColumns="False" CellPadding="1" GridLines="None">
				<SelectedItemStyle Font-Bold="True" CssClass="selected"></SelectedItemStyle>
				<AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
				<ItemStyle CssClass="item"></ItemStyle>
				<HeaderStyle CssClass="header"></HeaderStyle>
				<FooterStyle ForeColor="#4A3C8C" BackColor="#B5C7DE"></FooterStyle>
				<PagerStyle CssClass="pager" Mode="NumericPages"></PagerStyle>
				<Columns>
					<asp:ButtonColumn Text="&lt;img title=&quot;Elimina&quot; src=&quot;../images/elimina.gif&quot;&gt;" CommandName="Delete" Visible="false">
						<HeaderStyle HorizontalAlign="Center" Width="1%"></HeaderStyle>
						<ItemStyle HorizontalAlign="Center"></ItemStyle>
					</asp:ButtonColumn>
					<asp:EditCommandColumn ButtonType="LinkButton" 
                        UpdateText="&lt;img  title=&quot;Conferma&quot; src=&quot;../images/conferma.gif&quot; onclick='controlla(event)' &gt;"
						CancelText="&lt;img  title=&quot;Annulla&quot; src=&quot;../images/annullaconf.gif&quot; &gt;"
						EditText="&lt;img  title=&quot;Modifica&quot; src=&quot;../images/modifica.gif&quot;  &gt;" Visible="False">
						<HeaderStyle HorizontalAlign="Center" Width="1%"></HeaderStyle>
						<ItemStyle HorizontalAlign="Center"></ItemStyle>
					</asp:EditCommandColumn>
                        
					<asp:TemplateColumn HeaderText="Vaccinazione">
						<HeaderStyle HorizontalAlign="Left" Width="25%"></HeaderStyle>
						<ItemStyle HorizontalAlign="Left"></ItemStyle>
						<ItemTemplate>
							<asp:Label id="tb_descVac" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("vac_descrizione") %>'></asp:Label>
                            &nbsp;-&nbsp;
                            <asp:Label id="tb_codVac" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("vex_vac_codice") %>'></asp:Label>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText=" Data&lt;br&gt; Visita">
						<HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
						<ItemStyle HorizontalAlign="Center"></ItemStyle>
						<ItemTemplate>
							<asp:Label id="tb_data_visita" runat="server" Text='<%# CutTime(DataBinder.Eval(Container, "DataItem")("vex_data_visita")) %>'>
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<on_val:onitdatepick id="tb_data_visita_edit" runat="server" Width="100px" Height="22px" CssClass="TextBox_Data_Obbligatorio" Text='<%# CutTime(DataBinder.Eval(Container, "DataItem")("vex_data_visita")) %>' FormatoData="GeneralDate" Focus="False" DateBox="True" CalendarioPopUp="False" NoCalendario="True" Formatta="False" indice="-1" Hidden="False" ControlloTemporale="False" target="tb_data_visita_edit">
							</on_val:onitdatepick>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Motivazione">
						<HeaderStyle HorizontalAlign="Left" Width="20%"></HeaderStyle>
						<ItemStyle HorizontalAlign="Left"></ItemStyle>
						<ItemTemplate>
							<asp:Label id="tb_motivo" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("moe_descrizione") %>'>
							</asp:Label>
							<asp:HiddenField id="hdCodMotivo" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem")("vex_moe_codice") %>' />
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Medico">
						<HeaderStyle HorizontalAlign="Left" Width="19%"></HeaderStyle>
						<ItemStyle HorizontalAlign="Left"></ItemStyle>
						<ItemTemplate>
							<asp:Label id="tb_medico" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("ope_nome") %>'>
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<on_ofm:onitmodallist id="fm_medico_edit" runat="server" Width="100%" Filtro="'true'='true' order by ope_nome" CodiceWidth="0%" CampoCodice="ope_codice" CampoDescrizione="ope_nome" Connection="" Tabella="t_ana_operatori" UseCode="False" SetUpperCase="True" RaiseChangeEvent="False" Descrizione='<%# DataBinder.Eval(Container, "DataItem")("ope_nome") %>' Codice='<%# DataBinder.Eval(Container, "DataItem")("vex_ope_codice") %>' Obbligatorio="False" PosizionamentoFacile="False" LabelWidth="-1px">
							</on_ofm:onitmodallist>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Data&lt;br&gt; Scadenza">
						<HeaderStyle HorizontalAlign="Center" Width="11%"></HeaderStyle>
						<ItemStyle HorizontalAlign="Center"></ItemStyle>
						<ItemTemplate>
							<asp:Label id="tb_data_scadenza" runat="server" Text='<%# IIF(DataBinder.Eval(Container,  "DataItem")("vex_data_scadenza") is dbnull.value,"", CutTime(DataBinder.Eval(Container,  "DataItem")("vex_data_scadenza"))) %>'>
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<on_val:onitdatepick id="tb_data_scadenza_edit" runat="server" Width="103px" Height="22px" CssClass="TextBox_Data" Text='<%# IIF(DataBinder.Eval(Container,  "DataItem")("vex_data_scadenza") is dbnull.value,"", CutTime(DataBinder.Eval(Container,  "DataItem")("vex_data_scadenza"))) %>' FormatoData="GeneralDate" Focus="False" DateBox="True" CalendarioPopUp="True" NoCalendario="True" Formatta="False" indice="-1" Hidden="False" ControlloTemporale="False" target="tb_data_scadenza_edit">
							</on_val:onitdatepick>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn SortExpression="usl_inserimento_vex_descr" HeaderText="<%$ Resources: Onit.OnAssistnet.OnVac.Web, DatiVaccinali.AslInserimentoVacc %>" Visible="false">
                        <HeaderStyle HorizontalAlign="Left" />
						<ItemStyle width="10%" HorizontalAlign="Left" />
						<ItemTemplate>
							<asp:Label id="tb_usl_vex" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("usl_inserimento_vex_descr") %>'></asp:Label>
                            <asp:HiddenField ID="hdCodiceUslInserimento" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem")("vex_usl_inserimento") %>' />
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="" SortExpression="vex_flag_visibilita" Visible="False">
						<HeaderStyle HorizontalAlign="Center" Width="1%"></HeaderStyle>
						<ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:Image ID="imgFlagVisibilita" runat="server" 
                                ImageUrl="<%# BindFlagVisibilitaImageUrlValue(Container.DataItem) %>" 
                                ToolTip="<%# BindFlagVisibilitaToolTipValue(Container.DataItem) %>" />
                        </ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle Width="1%"></HeaderStyle>
						<ItemTemplate>
							<asp:Label id="Label14" runat="server" class="legenda_scaduta" Font-Names="Arial" Font-Bold="True" Visible='<%# DataBinder.Eval(Container, "DataItem")("s") %>'>&nbsp;S&nbsp;</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:Label id="Label25" runat="server" class="legenda_scaduta" Font-Names="Arial" Font-Bold="True" Visible='<%# DataBinder.Eval(Container, "DataItem")("s") %>'>&nbsp;S&nbsp;</asp:Label>
						</EditItemTemplate>
					</asp:TemplateColumn>
                    <asp:BoundColumn Visible="false" DataField="vex_usl_inserimento" ></asp:BoundColumn>
				</Columns>
			</asp:DataGrid>
        </dyp:DynamicPanel>
    </on_ofm:OnitFinestraModale>
			<script type="text/javascript">

				if (document.getElementById('ordDesc') != null) document.getElementById('ordDesc').style.display='none';
				if (document.getElementById('ordDosi') != null) document.getElementById('ordDosi').style.display='none';
				if (document.getElementById('ordOp') != null) document.getElementById('ordOp').style.display='none';
				if (document.getElementById('ordSII') != null) document.getElementById('ordSII').style.display='none';
				if (document.getElementById('ordCNS') != null) document.getElementById('ordCNS').style.display='none';
				if (document.getElementById('ordNC') != null) document.getElementById('ordNC').style.display='none';
				if (document.getElementById('ordLotto') != null) document.getElementById('ordLotto').style.display='none';
				if (document.getElementById('ordData') != null) document.getElementById('ordData').style.display='none';
				if (document.getElementById('ordAss') != null) document.getElementById('ordAss').style.display='none';
				if (document.getElementById('ordUtente') != null) document.getElementById('ordUtente').style.display='none';
				
				<%response.write(strJS)%>
			</script>
			
		</form>
	</body>
</HTML>
