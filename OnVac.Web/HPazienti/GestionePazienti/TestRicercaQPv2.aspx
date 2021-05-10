<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TestRicercaQPv2.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.TestRicercaQPv2" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head runat="server">
    <title>Pagina di test per ricerca QPv2</title>
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <style type="text/css">
        body {
            width: 100%;
            margin: 0px;
            padding-left: 10px;
            border-top: 2px solid whitesmoke;
            background-color: steelblue;
            font-family: Calibri;
            font-size: 16px;            
            color: white;
        }

        p {
            font-size: 20px;            
            padding: 20px 0px 10px 0px;
            width: 90%;
        }

        div {
            padding: 20px 0px 0px 20px;
            border: 1px solid #058;
            border-radius: 8px;
            width: 90%;
            height: 50px;
        }

        .btn {
            cursor: pointer;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" TitleCssClass="title3" Titolo="Ricerca pazienti" Width="100%" Height="100%">
            <p>
                TEST PER RICERCA PAZIENTE QPv2
            </p>
            <div>
                <asp:Label runat="server" ID="lblFiltroCF" CssClass="Label" Text="Codice Fiscale"></asp:Label>
                <asp:TextBox runat="server" ID="txtFiltroCF" CssClass="TextBox_Stringa" Width="250px"></asp:TextBox>
                <asp:Button runat="server" ID="btnRicerca" Text="Cerca" CssClass="btn" />
            </div>
        </on_lay3:OnitLayout3>
    </form>
</body>
</html>
