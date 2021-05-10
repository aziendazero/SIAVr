<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="NoteAnagPaziente.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.NoteAnagPaziente" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register Assembly="Onit.Web.UI.WebControls.DynamicPanel" Namespace="Onit.Web.UI.WebControls.DynamicPanel" TagPrefix="dyp" %>

<%@ Import Namespace="Onit.OnAssistnet.OnVac" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Note Paziente</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <script type='text/javascript' src='<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Utility.js") %>'></script>
    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
   
    <style type="text/css">
        .blocco_dati {
            background-color: #e7e7ff;
        }
    </style>

    <script type="text/javascript" language="javascript">
        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;
            switch (button.Key) {

                case 'btnAnnulla':
                    if (confirm("Sicuro di annullare le modifiche?") == false) {
                        evnt.needPostBack = false;
                        return;
                    }
                    break;
                case 'btnSalva':
                    // controllo lunghezza note
                    if (!ControllaLunghezzaNote()) {
                        evnt.needPostBack = false;
                        return;
                    }
                    break;
            }
        }


        function ControllaLunghezzaNoteAlert(label, length, maxlength) {
            alert("Attenzione: il campo '" + label + "' eccede la lunghezza massima di " + maxlength + " caratteri!");
        }

        // controlla se la lunghezza delle note eccede quella del campo su DB
        function ControllaLunghezzaNote() {
            var lunghezza_max = 4000;
            var esito = true;
            $(".mynote").each(function() {
                var riganota = $(this);
                var lblnota = riganota.find('.lblnota');
                var txtnota = riganota.find('.txtnota');
                if(txtnota[0].disabled == false && txtnota[0].value.length > lunghezza_max) {
                    ControllaLunghezzaNoteAlert(lblnota[0].innerHTML, txtnota[0].value.length, lunghezza_max);
                    esito = false;
                }
            });
            return esito;
        }

    </script>

</head>
<body>
    <form id="form1" runat="server">

        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Height="100%" Width="100%" TitleCssClass="Title3" Titolo="Note Paziente">
            <div class="title" id="divLayoutTitolo_sezione1" style="width: 99%">
                <asp:Label ID="LayoutTitolo" runat="server" Width="100%" BorderStyle="None" CssClass="title"></asp:Label>
            </div>
            <div style="width: 99%">
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolbarNote" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                    <Items>
                        <igtbar:TBarButton Key="btnSalva" Text="Salva" DisabledImage="~/Images/salva_dis.gif" Image="~/Images/salva.gif">
                        </igtbar:TBarButton>
                        <igtbar:TBSeparator></igtbar:TBSeparator>
                        <igtbar:TBarButton Key="btnAnnulla" Text="Annulla" DisabledImage="~/Images/annulla_dis.gif" Image="~/Images/annulla.gif">
                        </igtbar:TBarButton>
                    </Items>
                </igtbar:UltraWebToolbar>
            </div>
            <dyp:DynamicPanel ID="dyp2" runat="server" Width="100%" Height="100%" ScrollBars="Auto" ExpandDirection="horizontal">
                <table style="table-layout:fixed; width:99%; border-collapse:collapse;" class="DataGrid">
                    <tr class="header">
                        <td class="label" width="15%" style="text-align:left;">
                            Tipo Nota
                        </td>
                        <td class="label" width="85%" style="text-align:left;">
                            Testo Nota
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Repeater ID="rptNote" runat="server">
                                <ItemTemplate>
                                    <table id="tblNote" cellpadding="3" cellspacing="0" style="table-layout:fixed;width:100%;" runat="server" class="mynote">
                                        <tr class="Item">
                                            <td class="label" width="15%" style="text-align:left;">
                                                <asp:Label ID="lblDescrTipoNote" runat="server" CssClass="lblnota"><%# Eval("DescrizioneNota")%></asp:Label>
                                                <asp:HiddenField ID="hdfIdNota" runat="server" Value='<%# Eval("IdNota") %>'/>
                                                <asp:HiddenField ID="hdfCodiceNota" runat="server" Value='<%# Eval("CodiceNota") %>'/>
                                            </td>
                                            <td width="85%">
                                                <asp:TextBox id="txtTestoNote" style="OVERFLOW: auto" runat="server" Width="100%" CssClass="textbox_stringa txtnota" 
                                                    MaxLength="4000" TextMode="MultiLine" Rows="5" Text='<%# Bind("TestoNota") %>' Enabled='<%# IIf(Eval("FlagNotaModificabile"), True, False) %>'></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <table id="tblNote" cellpadding="3" cellspacing="0" style="table-layout:fixed;width:100%;" runat="server" class="mynote">
                                        <tr class="alternating">
                                            <td class="label" width="15%" style="text-align:left;">
                                                <asp:Label ID="lblDescrTipoNote" runat="server" CssClass="lblnota"><%# Eval("DescrizioneNota")%></asp:Label>
                                                <asp:HiddenField ID="hdfIdNota" runat="server" Value='<%# Eval("IdNota") %>'/>
                                                <asp:HiddenField ID="hdfCodiceNota" runat="server" Value='<%# Eval("CodiceNota") %>'/>
                                            </td>
                                            <td width="85%">
                                                <asp:TextBox id="txtTestoNote" style="overflow-y:auto" runat="server" Width="100%" CssClass="textbox_stringa txtnota" 
                                                    MaxLength="4000" TextMode="MultiLine" Rows="4" Text='<%# Bind("TestoNota") %>' Enabled='<%# IIf(Eval("FlagNotaModificabile"), True, False) %>'></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </AlternatingItemTemplate>
                            </asp:Repeater>
                        </td>
                    </tr>
                </table>
            </dyp:DynamicPanel>
        </on_lay3:OnitLayout3>
    </form>


    <!-- #include file="../../Common/Scripts/AggiornaStatoVaccinazioni.inc" -->


</body>
</html>
