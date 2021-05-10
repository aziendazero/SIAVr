<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="OnVacMain.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVacMain" %>

<%@ Import Namespace="Onit.OnAssistnet.OnVac" %>
<!DOCTYPE  HTML PUBLIC "-//W3C//DTD HTML 4.01 Frameset//EN" "http://www.w3.org/TR/html4/frameset.dtd">
<html>
<head>
    <title>
        <% Response.Write(Me.AppTitle)%>
    </title>
    <meta http-equiv="X-UA-Compatible" content="IE=Edge" />
    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Utility.js") %>"></script>
    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
    <script type="text/javascript">

        $(document).ready(function () {
            // A Ravenna rimaneva bloccato il pulsante maximize e non si potevano modificare le dimensioni della finestra.
            // Con queste due istruzioni mette la finestra iniziale a tutto schermo, non è bello, ma così non c'è bisogno
            // di rilasciare il manager nuovo. Alle altre installazioni non dà nessun fastidio.
            var  debug = <%= Me.IsDebug %>;
            if (debug) { 
                window.resizeTo(1024, 768);
            }
            else
            {   
                window.moveTo(0, 0);
                window.resizeTo(screen.availWidth, screen.availHeight);
            }
        });

    </script>
</head>
<frameset id="FsRow" rows="57,*" border="0" framespacing="0" frameborder="0" onbeforeunload="AreYouSure()" onload="SetFirst();">
    <frame name="TopFrame" src='./layout/TopFrame.aspx' noresize scrolling="no" />
    <frameset cols="110,*" border="0" onbeforeunload="AreYouSure()" onload="SetFirst();"
        id="FsCol">
        <frame name="LeftFrame" src="about:blank" noresize scrolling="no" />
        <frame name="MainFrame" src="OnVacWait.aspx" scrolling="no" />
    </frameset>
</frameset>
</html>
