<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="OpenMVC.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OpenMVC" ValidateRequest="False" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>Open MVC</title>
    <script type="text/javascript" src="./Common/scripts/onvac.common.js" ></script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <script type="text/javascript" language="javascript">

            var url = "<%=GetUrl()%>";
            var paziente = "<%=GetPaziente()%>";
            var appId = "<%=GetAppId()%>";

            if (url.indexOf("{paz_codice}") != -1) {
                if (paziente != null && paziente != '' && paziente > 0) {
                    //                if (url.length > 0 && url.charAt(url.length - 1) != '/') {
                    //                    url = url + "/";
                    //                }
                    url = url.replace("{paz_codice}", paziente);
                    window.location = encodeUrl(url);
                }
                else {
                    alert('Codice paziente non valorizzato');
                }
            }
            else {
                window.location = encodeUrl(url);
            }
        </script>
    </form>
</body>
</html>

