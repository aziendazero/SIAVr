<%@ Page Language="vb" AutoEventWireup="false" Codebehind="RichiediCampo.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.RichiediCampo" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
	<HEAD>
		<title>RichiediCampo</title>
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<asp:TextBox id="txtCampo" style="Z-INDEX: 101; LEFT: 32px; POSITION: absolute; TOP: 104px" runat="server" Width="312px"></asp:TextBox>
			<asp:Label id="lblMessaggio" style="Z-INDEX: 102; LEFT: 40px; POSITION: absolute; TOP: 24px" runat="server" Width="296px" Height="80px" CssClass="labelnotaligned"></asp:Label>
			<asp:Button id="btnOK" style="Z-INDEX: 103; LEFT: 64px; POSITION: absolute; TOP: 152px" runat="server" Width="112px" Text="OK"></asp:Button>
			<asp:Button id="btnAnnulla" style="Z-INDEX: 104; LEFT: 200px; POSITION: absolute; TOP: 152px" runat="server" Width="112px" Text="Annulla"></asp:Button>
		</form>
		<script language="vbscript">
			document.all("lblMessaggio").innertext=window.dialogArguments
			
			sub btnOK_onclick()
				window.returnValue=document.all("txtCampo").value
				window.close 
				window.event.returnValue=False
			end sub
			
			sub btnAnnulla_onclick()
				window.close 
				window.event.returnValue=False				
			end sub
		</script>
	</body>
</HTML>  
