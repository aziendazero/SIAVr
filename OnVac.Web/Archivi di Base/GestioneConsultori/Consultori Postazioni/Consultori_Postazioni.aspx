<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Consultori_Postazioni.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Consultori_Postazioni" %>

<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title>Centri_Postazioni</title>

        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
		
		<script type="text/javascript" language="javascript">
			function InizializzaToolBar(t)
			{
				t.PostBackButton=false;
			}
		
			function ToolBarClick(ToolBar,button,evnt)
			{
				evnt.needPostBack=true;
				switch(button.Key)
				{
					case 'btnAnnulla':
					    if (!confirm("Le modifiche apportate non saranno salvate. Continuare?"))
                    {
						evnt.needPostBack=false;
					} 
					break;
				}
			}
		</script>
         <style type="text/css">
            div#OnitCell4 {
            float:left;
        }

    </style>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout21" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 0px"
				runat="server" Titolo="Associazione Postazioni-Centri" HeightTitle="50px" Height="100%" Width="100%"
				DisableLeft="False" DisableTop="False" ButDefault="-1">
                <div>
						<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                            <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                            <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
							<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
							<Items>
								<igtbar:TBarButton Key="btnSalva" Text="Salva" Image="~/Images/salva.gif"></igtbar:TBarButton>
								<igtbar:TBarButton Key="btnAnnulla" Text="Annulla" Image="~/Images/annulla.gif"></igtbar:TBarButton>
							</Items>
						</igtbar:UltraWebToolbar>
                </div>
                <div Class="Title" style="height:20px">
					<asp:Label id="LayoutTitolo_Titolo" runat="server"  >Associazione Postazione-Sedi Vaccinali</asp:Label>
                </div>
                <dyp:DynamicPanel ID="dypR1" runat="server" Width="100%" Height="50%" ExpandDirection="horizontal">
                    <dyp:DynamicPanel ID="dypLeft" runat="server" Width="50%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                        <ignav:UltraWebTree BrowserTarget="UpLevel" id="tvwGP" runat="server" Width="100%"  Font-Names="Arial" Font-Size="12px"
                            WebTreeTarget="ClassicTree" AutoPostBack="True"></ignav:UltraWebTree>
                    </dyp:DynamicPanel>
                    <dyp:DynamicPanel ID="dypRight" runat="server" Width="50%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                        <asp:Label id="lblInfo" runat="server" Width="100%" Height="100%" CssClass="TextBox_Stringa"></asp:Label>
                    </dyp:DynamicPanel>
                </dyp:DynamicPanel>
                <div>
                    <table id="Table2" cellspacing="1" cellpadding="1" width="100%" border="0">
                        <tr>
		                    <td class="label" width="40%">Centro Vaccinale:</td>
		                    <td width="45%">
			                    <on_ofm:onitmodallist id="txtConsultorio" runat="server" Width="70%" PosizionamentoFacile="False" LabelWidth="-1px"
				                    CodiceWidth="30%" Connection="" UseCode="True" Obbligatorio="False" SetUpperCase="True" CampoCodice="CNS_CODICE"
				                    CampoDescrizione="CNS_DESCRIZIONE" Tabella="T_ANA_CONSULTORI"></on_ofm:onitmodallist></td>
		                    <td width="15%">
			                    <asp:Button id="btnRiassocia" runat="server" Width="100%" Text="Riassocia"></asp:Button></td>
	                    </tr>
	                    <tr height="18">
		                    <td bgcolor="#ffff99" colspan="3">
                                <asp:Label id="lblWarning" runat="server" Width="100%" Height="18px" CssClass="TextBox_Stringa"></asp:Label>
		                    </td>
	                    </tr>
                    </table>
                </div>
			</on_lay3:onitlayout3>
		</form>
	</body>
</html>
