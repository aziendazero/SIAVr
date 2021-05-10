<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="LeftFrame.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.LeftFrame" %>

<%@ Import Namespace="Onit.OnAssistnet.OnVac.OnVacUtility" %>
<%@ Import Namespace="Onit.OnAssistnet.OnVac" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title></title>
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <script type="text/javascript" src="<%= ResolveUrl("~/scripts/menu.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/scripts/onitlayout.js") %>"></script>
    <style type="text/css">
        .leftBody {
            background-color: steelblue;
            margin: 0px;
            overflow: auto;
        }

        .leftMenu {
            border-style: solid;
            border-color: steelblue;
            border-width: 0px 1px 0px 2px;
            background-color: steelblue;
            /* BorderWidth="1px" BorderColor="steelblue" BorderStyle="Solid"  */
        }
    </style>
</head>
<body onload="BodyOnLoad();">
    <form id="Form1" method="post" runat="server" style="overflow: auto">
        <asp:Label ID="voci" Style="display: none" runat="server" Width="1px" Height="9px"></asp:Label>
        <div id="divMenu" onmouseover="Menu_MouseOver(divMenu, divPin, tblHidMenu, delay, 1)"
            onmouseout="Menu_MouseOut(divMenu, divPin, tblHidMenu, delay, 1)">
            <iglbar:UltraWebListbar BrowserTarget="UpLevel" ID="UltraWebListbar" runat="server" Width="100%" BarWidth="100%" Height="100%" CssClass="leftMenu">
                <DefaultGroupStyle CssClass="InfraListBar_GroupStyle"></DefaultGroupStyle>
                <DefaultItemStyle CssClass="InfraListBar_ItemStyle"></DefaultItemStyle>
                <DefaultItemHoverStyle CssClass="InfraListBar_ItemHoverStyle"></DefaultItemHoverStyle>
                <DefaultItemSelectedStyle CssClass="InfraListBar_ItemStyle"></DefaultItemSelectedStyle>
                <DefaultGroupButtonStyle CssClass="InfraListBar_GroupButtonStyle"></DefaultGroupButtonStyle>
                <DefaultGroupButtonSelectedStyle CssClass="InfraListBar_GroupButtonSelectedStyle">
                </DefaultGroupButtonSelectedStyle>
                <DefaultGroupButtonHoverStyle CssClass="InfraListBar_GroupButtonHoverStyle">
                </DefaultGroupButtonHoverStyle>
                <ClientSideEvents AfterItemSelected="listaBar_AfterItemSelected"></ClientSideEvents>
            </iglbar:UltraWebListbar>
            <div id="divPin" style="position: absolute; top: -2px; left: 75px; width: 20%">
                <img id="imgPin" src="../Images/Menu/pinLocked.gif" alt="Sblocca" onclick="Pin_Click(this, '<%= me.ResolveClientUrl("../Images/Menu/") %>', 'pinLocked.gif', 'pinUnlocked.gif')"
                    style="cursor: pointer; display: block; float: right; margin: 10px 9px" />
            </div>
        </div>
        <table id="tblHidMenu" title="Visualizza Left Menu" onmouseover="Menu_MouseOver(divMenu, divPin, tblHidMenu, delay, 1)"
            onmouseout="Menu_MouseOut(divMenu, divPin, tblHidMenu, delay, 1)" onclick="HidMenu_MouseClick(divMenu, divPin, tblHidMenu, 1)"
            style="cursor: pointer; display: none; width: 100%; height: 100%; border: 1px solid whitesmoke; background-image: url(<%= ResolveUrl("~/Images/Menu/tabHidV.gif") %>); background-repeat: repeat-y;">
            <tr>
                <td>
                    <img style="border: 0; cursor: pointer;" runat="server" src="~/Images/Menu/showLeft.gif"
                        alt="Visualizza Left Menu" />
                </td>
            </tr>
        </table>
        <!-- #include file="../Common/Scripts/AggiornaStatoVaccinazioni.inc" -->
    </form>
    <script type="text/javascript">
        var delay = 1000
        var divMenu = document.getElementById('divMenu');
        var divPin = document.getElementById('divPin');
        var tblHidMenu = document.getElementById('tblHidMenu');

        //var main = top.window.frames["MainFrame"];
        //if (main && AggiornaLeftTitles) {
        //    main.addEventListener('load', function () {
        //        AggiornaLeftTitles();
        //    })
        //}
    </script>
</body>


</html>
