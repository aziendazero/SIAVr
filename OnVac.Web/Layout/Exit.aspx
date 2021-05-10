<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Exit.aspx.vb" Inherits="Onit.OnAssistnet.OnVac._Exit"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title>Exit</title>

		<style type="text/css">
		    .exitMsg
		    {
		        font-size: 12px;
		        font-family: verdana;
		        font-weight: bold;
		        color: blue;
		        position: absolute; 
		        left: 16px; 
		        top: 16px;
		    }
		    
		    .exitBar
		    {
		        display: inline; 
		        left: 16px; 
		        width: 0.81%; 
		        position: absolute; 
		        top: 56px; 
		        height: 19px; 
		        background-color: #00aaff;
		        /*  filter:progid:DXImageTransform.Microsoft.Gradient(endColorstr='#000080', startColorstr='#00aaff', gradientType='1');  */
		    }
		</style>
	</head>
	<body style="overflow:hidden; background-color:#f5f5f5;" >
		<form id="Form1" method="post" runat="server" >
			<asp:Label id="Label1" CssClass="exitMsg" runat="server">E' stata avviata la procedura di pulizia dell'applicazione.<br />Attendere qualche secondo.</asp:Label>
			<div id="Barra" class="exitBar" ></div>
		</form>
		<script type="text/javascript" language="javascript">
			var c,t;
			c=2;
			t=window.setInterval("Conta()",50);
			function Conta()
			{
				c=c+1;
				valore=((c-2)*5).toString() + "%";
				document.getElementById("Barra").style.width=valore;
				if (c==20)
				{
					window.clearInterval(t);
					window.close();
				}
			}
		</script>
	</body>
</html>
