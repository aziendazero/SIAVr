<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="StarterOnVacMVC.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.StarterOnVacMVC" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
    <meta http-equiv="X-UA-Compatible" content="IE=Edge" />
</head>
<body onload='submitForm()'>
    <form id="form1" method="post" action="<%=GetMainWebFolder()%>OnVac050.WebMVC/StarterEnvironment">
        <div style="text-align:center; font-family:Verdana; font-size:16px;">
            Caricamento in corso...
        </div>
        <input type='hidden' id="url" name='url' value="<%=GetUrl()%>" />
        <input type='hidden' id="appIdCen" name='appIdCen' value="<%=GetAppIdCentrale()%>" />
        <input type='hidden' id='appIdLoc' name='appIdLoc' value="<%=GetAppIdLocale()%>" />
        <input type='hidden' id='codAzi' name='codAzi' value="<%=GetCodiceAzienda()%>" />
        <input type='hidden' id='codCNS' name='codCNS' value="<%=GetCodiceConsultorio()%>" />
        <input type='hidden' id='userId' name='userId' value="<%=GetUserId()%>" />
        <input type='hidden' id="codPaz" name='codPaz' value="<%=GetCodiceLocalePaziente()%>" />
        <input type='hidden' id="ulss" name='ulss' value="<%=GetCodiceUslCorrente()%>" />
        <input type="hidden" id="menuDisPaziente" name="menuDisPaziente" value="<%=GetMenuDis_HPazienti()%>" />
        <input type="hidden" id="menuAppId" name="menuAppId" value="<%=GetAppIdMenu()%>" />
    </form>
    <script type="text/javascript">
        function submitForm()
        {
            document.getElementById('form1').submit();
        }
    </script>
</body>
</html>
