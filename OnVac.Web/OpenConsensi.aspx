<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="OpenConsensi.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OpenConsensi" ValidateRequest="False" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
    <head>
        <title>Rilevazione Consensi</title>
        <script type="text/javascript" src="./Common/scripts/onvac.common.js" ></script>
    </head>
    <body style="background-color:aliceblue" >
	    <form id="Form1" method="post" runat="server">

            <div style="text-align:center; font-family:Verdana; font-size:16px;">
                Caricamento in corso. Attendere...
            </div>    
		
            <script type="text/javascript" language="javascript">
		        var urlConsensi = "<%=GetUrlConsensi()%>";
		        var userCrypted = "<%=GetCryptedUser()%>";
		        var paziente = "<%=GetPaziente()%>";
		        var appIdCons = "<%=GetAppIdConsenso()%>";
		        var editMode = "<%=GetFlagEditMode()%>";
                var azi = "<%=GetCodiceAziendaRegistrazione()%>";
                var hide = "<%=GetIdConsensiNonVisibili()%>";

		        if (userCrypted != null || userCrypted != '') {
		            apriRemoteApp(urlConsensi.replace("{0}", encodeUrl(userCrypted)).replace("{1}", paziente).replace("{2}", appIdCons) + editMode + azi + hide);
		        }

		        function apriRemoteApp(strUrl) {
		            if (strUrl != "") {
		                window.location = strUrl;
		            }
		        }
		    </script>
    </form>
  </body>
</html>
