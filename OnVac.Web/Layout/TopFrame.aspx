<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TopFrame.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.TopFrame" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>TopFrame</title>
    <link type="text/css" rel="stylesheet" href="<%= ResolveUrl("~/Css/menu/sun2.css") %>" />
    <script type="text/javascript" src="<%= ResolveUrl("~/scripts/onitlayoutnew.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/scripts/menu.js") %>"></script>
    <style type="text/css">
        .datiTOP {
            font-family: Calibri;
            font-size: 13px;
            color: white;
            text-align: right;
            padding-right: 3px;
        }

        .datiTOPUser {
            text-align: right;
            font-style: italic;
        }

        .datiTOPMenu {
            font-size: 15px;
            height: 22px;
        }

        .titoloSiavr {
            font-family: Calibri;
            font-size: 14px;
            color: white;
            text-align: left;
            padding-left: 3px;
        }

        .logoSiavr {
            height: 24px;
            border: 1px solid steelblue;
            margin-left: 2px;
            position: absolute;
            bottom: 6px;
            left: 14px;
        }
    </style>
</head>
<body text="#000000" bgcolor="#ffffff" style="width: 100%; margin: 0px; border: 0px">
    <img id="copri" runat="server" src="~/Images/Menu/clearpixel.gif" style="z-index: 1000; left: 0px; visibility: hidden; width: 100%; position: absolute; top: 0px; height: 100%" alt="" />
    <table id="tblMenu" onmouseover="Menu_MouseOver(tblMenu, tdPin, tblHidMenu, delay, 0)" onmouseout="Menu_MouseOut(tblMenu, tdPin, tblHidMenu, delay, 0)"
        cellspacing="0" cellpadding="0" width="100%" border="0" style="background-color: steelblue; height: 100%">
        <colgroup>
            <col style="width: 110px" />
            <col />
            <col style="width: 40%" />
        </colgroup>
        <tbody>
            <tr>
                <td colspan="2">
                    <div class="titoloSiavr">Anagrafe Vaccinale - Regione Veneto</div>
                </td>
                <td class="datiTOP datiTOPUser">
                    <asp:Label ID="lblUtente" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td rowspan="2">
                    <img alt="Onit Group Srl - SIAVr" src="../Images/logo-no-payoff-contrasto.png" class="logoSiavr" />
                </td>
                <td colspan="2" class="datiTOP datiTOPMenu">
                    <asp:Label ID="lblNomeApp" runat="server"></asp:Label>
                </td>
            </tr>
            <!--
                    <td style="BACKGROUND-IMAGE: url(<%= ResolveUrl("~/Images/Menu/top_bg.gif") %>)" vAlign="middle" align="center" width="13">
					    <img  id="imgLockUser" src='../Images/Menu/lock.gif' style='cursor:hand' onclick="LockUser()" alt="Blocca Utente" runat=server/>
				    </td>
				    <td id="tdPin"  style="background-color:#048; text-align:right;padding:2px 4px 2px 2px">
                        <img id="imgPin" src="../Images/Menu/pinLocked.gif" alt="Sblocca" onclick="Pin_Click(this, '<%= me.ResolveClientUrl("../Images/Menu/") %>', 'pinLocked.gif', 'pinUnlocked.gif')" style="border: 0; cursor: pointer" />
                    </td>
                -->
            <tr>
                <td colspan="2" style="background-color: #3771a1; border-top: 1px solid #048; border-left: 1px solid #048; border-top-left-radius: 10px; padding-left: 5px">
                    <table cellspacing="0" cellpadding="0" border="0">
                        <tbody>
                            <tr>
                                <%=htmlRendered%>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
        </tbody>
    </table>
    <!-- #BeginEditable "index_middle" -->
    <table id="tblHidMenu" title="Visualizza Top Menu"
        onmouseover="Menu_MouseOver(tblMenu, tdPin, tblHidMenu, delay, 0)" onmouseout="Menu_MouseOut(tblMenu, tdPin, tblHidMenu, delay, 0)"
        onclick="HidMenu_MouseClick(tblMenu, tdPin, tblHidMenu, 0)"
        style="cursor: pointer; display: none; width: 100%; border: 1px solid whitesmoke; background-image: url(<%= ResolveUrl("~/Images/Menu/tabHidO.gif") %>); background-repeat: repeat-x;">
        <tr>
            <td valign="top" align="center" style="font-size: 1px">
                <img runat="server" src="~/Images/Menu/showTop.gif" alt="Visualizza Top Menu" />
            </td>
        </tr>
    </table>
    <script type="text/javascript">

        var delay = 1000
        var tblMenu = document.getElementById('tblMenu');
        var tdPin = document.getElementById('tdPin');
        var tblHidMenu = document.getElementById('tblHidMenu');

        function SetTitolo(titolo) {
            document.getElementById("lblNomeApp").innerHTML = titolo;
        }
    </script>
    <%=JsRendered%>
</body>
</html>
