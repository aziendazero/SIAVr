<%@ Page Language="vb" AutoEventWireup="false" Codebehind="AnalisiReport.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.AnalisiReport"%>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="cc1" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitTable" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title>Analsi Report</title>
        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
		
        <style type="text/css">
            .dgr { width: 100% }
	        .dgr2 { border-right: gray 1px solid; border-top: gray 1px solid; border-left: gray 1px solid; width: 70%; border-bottom: gray 1px solid; border-collapse: collapse }
	        .dgr TD { font-size: 12px; font-family: Tahoma }
	        .dgr2 TD { font-size: 10px }
	        .r1 { BACKGROUND-COLOR: whitesmoke }
	        .r2 { BACKGROUND-COLOR: #e7e7ff }
	        .h1 { FONT-WEIGHT: bold; COLOR: #4a3c8c; BORDER-TOP-STYLE: none; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BACKGROUND-COLOR: lightsteelblue; BORDER-BOTTOM-STYLE: none }
	        .cExp { PADDING-RIGHT: 2px; PADDING-LEFT: 2px; FONT-WEIGHT: bold; PADDING-BOTTOM: 2px; COLOR: white; PADDING-TOP: 2px; BACKGROUND-COLOR: #4a3c8c }
	        .tdInt { FONT-WEIGHT: bold; COLOR: #4a3c8c; BACKGROUND-COLOR: lightsteelblue }
		</style>

		<script type="text/javascript" language="javascript">
		function InizializzaToolBar(t)
		{
			t.PostBackButton=false;
		}		
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout" runat="server" WindowNoFrames="False" width="70%" height="100%">
                <cc1:onittable id="OnitTable1" runat="server" width="100%" height="100%">

				    <cc1:OnitSection id="OnitSection1" runat="server" width="100%" TypeHeight="Content">
					    <cc1:OnitCell id="OnitCell1" runat="server" height="100%" width="100%" TypeScroll="Hidden">
						    <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="tlbOperazioniGruppo" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                                <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                                <HoverStyle CssClass="infratoolbar_button_hover" ></HoverStyle>
				                <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
							    <Items>
								    <igtbar:TBarButton Key="btnOpGrpCerca" Text="Cerca" DisabledImage="../../images/cerca_dis.gif" Image="../../images/cerca.gif">
								    </igtbar:TBarButton>
								    <igtbar:TBSeparator></igtbar:TBSeparator>
								    <igtbar:TBarButton Key="btnOpGrpStampa" Text="Stampa" DisabledImage="../../images/stampa_dis.gif" Image="../../images/stampa.gif">
								    </igtbar:TBarButton>
							    </Items>
						    </igtbar:UltraWebToolbar>
						    <div class="sezione">Filtri di ricerca</div>
					    </cc1:OnitCell>
				    </cc1:OnitSection>

				    <cc1:OnitSection id="Onitsection3" runat="server" height="60px" width="100%">
					    <cc1:OnitCell id="Onitcell3" runat="server" height="100%" width="100%" TypeScroll="Auto">
						    <table cellspacing="2" cellpadding="2">
							    <tr>
								    <td>
									    <asp:Label class="textbox_stringa" id="Label1" runat="server" Text="Installazione"></asp:Label></td>
								    <td>
									    <asp:DropDownList id="ddlInstallazione" runat="server"></asp:DropDownList></td>
							    </tr>
						    </table>
						    <table cellspacing="0" cellpadding="0" width="100%" border="0">
							    <tr class="header">
								    <td width="5%">&nbsp;</td>
								    <td width="30%">Nome</td>
								    <td width="30%">Installazione</td>
								    <td width="15%">Cartella</td>
								    <td width="20%">Dataset</td>
							    </tr>
						    </table>
					    </cc1:OnitCell>
				    </cc1:OnitSection>

				    <cc1:OnitSection id="Onitsection4" runat="server" height="100%" width="100%" TypeHeight="Calculate">
					    <cc1:OnitCell id="Onitcell4" runat="server" height="100%" width="100%" TypeScroll="Auto">
						    <asp:DataGrid id="dgrReport" runat="server" Width="100%" GridLines="None" ShowHeader="False" CssClass="dgr" AutoGenerateColumns="False">
							    <AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
							    <ItemStyle BackColor="#E7E7FF"></ItemStyle>
							    <HeaderStyle Font-Bold="True" ForeColor="White" BackColor="#4A3C8C"></HeaderStyle>
							    <Columns>
								    <asp:TemplateColumn>
									    <ItemTemplate>
										    <table id="TableInnerRecord" style="table-layout: fixed" cellspacing="0" cellpadding="0" width="100%" border="0">
											    <tr>
												    <td width="10">
													    <asp:Label id="lblCodice" style="display: none" runat="server" Text='<%# Container.DataItem("RPT_NOME") %>'>
													    </asp:Label></td>
												    <td width="20">
													    <asp:ImageButton id="ImageButton1" style="cursor: hand" ImageUrl="../../images/sel.gif" OnClick='ImageButton_Click' 
                                                            CommandName='<%# Container.DataItem("RPT_NOME") %>' CommandArgument='<%# Container.DataItem("RPT_CARTELLA") %>' 
                                                            runat="server"></asp:ImageButton></td>
												    <td width="10"></td>
												    <td width="30%">
													    <asp:Label id=lblNomeReport runat="server" Text='<%# Container.DataItem("RPT_NOME") %>'>
													    </asp:Label></td>
												    <td width="30%">
													    <asp:Label id="lblInstallazione" runat="server" Text='<%# Container.DataItem("RPT_INSTALLAZIONE") %>'>
													    </asp:Label></td>
												    <td width="15%">
													    <asp:Label id="lblCartella" runat="server" Text='<%# Container.DataItem("RPT_CARTELLA") %>'>
													    </asp:Label></td>
												    <td width="25%">
													    <asp:Label id=lblDataSet runat="server" Text='<%# Container.DataItem("RPT_DATASET") %>'>
													    </asp:Label></td>
											    </tr>
										    </table>
									    </ItemTemplate>
								    </asp:TemplateColumn>
							    </Columns>
						    </asp:DataGrid>
					    </cc1:OnitCell>
				    </cc1:OnitSection>

				    <cc1:OnitSection id="Onitsection5" runat="server" width="100%" TypeHeight="Content">
					    <cc1:OnitCell id="Onitcell5" runat="server" height="100%" width="100%" TypeScroll="Hidden">
						    <div class="sezione">Dettaglio operazione
							    <asp:CheckBox id="cbStandard" runat="server" Text="Standard"></asp:CheckBox>
							    <asp:CheckBox id="cbIntestazione" runat="server" Text="Intest. personalizzata"></asp:CheckBox>
						    </div>
						    <asp:Label id="lblSelected" runat="server" Text="" Width="100%" CssClass="textbox_stringa"></asp:Label>
						    <table class="datagrid" width="100%">
							    <colgroup>
								    <col width="10%" />
								    <col width="80%" />
								    <col width="10%" />
							    </colgroup>
							    <tr class="item">
								    <td style="height: 28px">
									    <asp:Label id="Label2" runat="server" Text="Descrizione"></asp:Label></td>
								    <td style="height: 28px">
									    <asp:TextBox id="txtDescrizione" runat="server" width="100%" TextMode="MultiLine"></asp:TextBox></td>
								    <td style="height: 27px" vAlign="middle" colspan="2">
									    <asp:ImageButton id="ImgSaveDetail" style="cursor: hand" onclick="ImgSaveDetail_Click" runat="server"
										    CommandArgument="" CommandName="" ImageUrl="../../images/salva.gif"></asp:ImageButton></td>
							    </tr>
							    <tr>
								    <td>
									    <asp:Label id="Label3" runat="server" Text="Utilizzato da"></asp:Label></td>
								    <td>
									    <asp:TextBox id="txtUtilizzato" runat="server" width="100%" TextMode="MultiLine"></asp:TextBox></td>
								    <td></td>
							    </tr>
							    <tr class="item">
								    <td>
									    <asp:Label id="Label4" runat="server" Text="Maschera"></asp:Label></td>
								    <td>
									    <asp:TextBox id="txtMaschera" runat="server" width="100%" TextMode="MultiLine"></asp:TextBox></td>
								    <td></td>
							    </tr>
							    <tr >
								    <td>
									    <asp:Label id="Label5" runat="server" Text="Intestazione"></asp:Label></td>
								    <td>
									    <asp:TextBox id="txtIntestazione" runat="server" width="100%" MaxLength="100"></asp:TextBox></td>
								    <td></td>
							    </tr>
							    <tr class="item">
								    <td>
									    <asp:Label id="Label6" runat="server" Text="Piede"></asp:Label></td>
								    <td>
									    <asp:TextBox id="txtPiede" runat="server" width="100%" MaxLength="100"></asp:TextBox></td>
								    <td></td>
							    </tr>
						    </table>
						    <asp:DataGrid id="dgrDettaglio" runat="server" Width="100%" GridLines="None" ShowHeader="False"
							    CssClass="dgr" AutoGenerateColumns="True">
							    <AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
							    <ItemStyle BackColor="#E7E7FF"></ItemStyle>
							    <HeaderStyle Font-Bold="True" ForeColor="White" BackColor="#4A3C8C"></HeaderStyle>
						    </asp:DataGrid>
					    </cc1:OnitCell>
				    </cc1:OnitSection>

			    </cc1:onittable>
            </on_lay3:onitlayout3>
		</form>
	</body>
</html>
