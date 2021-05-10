<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="VacProg.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_VacProg" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>

<%@ Register TagPrefix="uc1" TagName="AssLotti" Src="AssLotti.ascx" %>
<%@ Register TagPrefix="uc1" TagName="InsAssociazione" Src="../InsAssociazione.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ScegliAss" Src="ScegliAss.ascx" %>
<%@ Register TagPrefix="uc1" TagName="InsLotto" Src="InsLotto.ascx" %>
<%@ Register TagPrefix="uc1" TagName="InsDatiEsc" Src="InsDatiEsc.ascx" %>
<%@ Register TagPrefix="uc2" TagName="SelezioneAmbulatorio" Src="../../Common/Controls/SelezioneAmbulatorio.ascx" %>

<%@ Import Namespace="Onit.OnAssistnet.OnVac.OnVacUtility" %>
<%@ Import Namespace="Onit.OnAssistnet.OnVac" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Vaccinazioni Programmate</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/toolbar.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/default/toolbar.default.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript" src='<%= OnVacUtility.GetScriptUrl("onvac.common.js") %>'></script>

    <style type="text/css">
        .inputLCB {
            left: -2000px;
            position: absolute;
            top: -2000px;
        }

        .box {
            border: 1px solid navy;
            font-weight: bold;
            font-size: 12px;
        }

        .padding4px {
            padding: 4px;
        }

        .TextBox_TitoloData_Disabilitato {
            border: 1px solid black;
            font-size: 12px;
            color: black;
            font-family: Arial;
            background-color: #dcdcdc;
            text-align: center;
        }

        .TextBox_TitoloStringa_Disabilitato {
            border: 1px solid gray;
            font-size: 12px;
            color: black;
            font-family: Arial;
            background-color: #dcdcdc;
            text-align: left;
        }

        .TextBox_TitoloNumero_Disabilitato {
            border: 1px solid gray;
            font-size: 12px;
            color: black;
            font-family: Arial;
            background-color: #dcdcdc;
            text-align: right;
        }

        .lblRitardo_Nessuno {
            font-size: 13px;
            color: black;
            font-family: Ms Sans Serif;
            background-color: transparent;
            text-align: center;
        }

        .lblRitardo_Sollecito {
            border: 1px solid black;
            font-weight: bold;
            font-size: 15px;
            color: black;
            font-family: Ms Sans Serif;
            background-color: red;
            text-align: center;
        }

        .vacOrangeTable {
            padding-right: 0px;
            padding-left: 0px;
            padding-bottom: 10px;
            padding-top: 10px;
        }

        .vacOrangeTableHeader {
            font-weight: bold;
            font-size: 14px;
            color: #ffffff;
            font-family: Arial,Tahoma,Verdana;
            background-color: #ff7f50;
            text-align: left;
        }

        .tdRitardiBorderTop {
            border-top: solid 1px lightgrey;
        }

        .warningModaleEdit {
            border: navy 2px solid;
            padding: 4px;
            color: blue;
            background-color: #f5f5f5;
            font-family: Verdana;
            font-weight: bold;
            font-size: 12px;
        }

        .vac-note {
            overflow-y: auto;
            font-family: Verdana;
            font-size: 12px;
            width: 100%;
            height: 100%;
        }

        .vac-img-btn {
            position: relative;
            top: 2px;
        }
    </style>
</head>
<body onkeypress="DisableEnter(event)">

    <script type="text/javascript" src="ScriptVacProg.js"></script>

    <script type="text/javascript">

        function DisableEnter(evt) {
            if (evt.keyCode == 13) {
                evt.preventDefault();
                return false;
            }
        }

        // --------- ATTENZIONE!!! --------- \\
        // idxVacProg è definito nello script incluso in precedenza, e contiene i valori degli indici delle colonne di dg_vacProg.
        // Se si modifica il datagrid, allineare i valori degli indici delle colonne.

        function btnOKLogin_Click(evnt) {
            if (!isValidFinestraModale('txtMedicoResponsabile', false)) {
                StopPreventDefault(evnt);
            }
        }

        function GetDateFromString(value) {
            var temp = value.split("/");

            return new Date(temp[2], temp[1] - 1, temp[0], 0, 0, 0);
        }

        // Restituisce true se la data di esecuzione (impostata nel tab "ESECUZIONE") non è precedente rispetto alla data di convocazione
        function controllaDataEsecuzione() {

            var dataCNV = GetDateFromString(document.getElementById('<%=Me.tb_cnv_dataConv.ClientID%>').value);
            var dataEsecuzione = GetDateFromString(document.getElementById('<%=Me.txtDataEs.ClientID%>').value);

            return (dataEsecuzione >= dataCNV);
        }

        function confirmDelete(evt) {
            if (!isInEdit()) {
                if (!confirm("ATTENZIONE !!!\nLa vaccinazione programmata verrà eliminata.\nContinuare ?")) {
                    StopPropagation(evt);
                    StopPreventDefault(evt);
                }
            }
            else {
                StopPropagation(evt);
                StopPreventDefault(evt);
            }
        }

        function confirmEdit(evt) {
            if (isInEdit()) {
                StopPropagation(evt);
                StopPreventDefault(evt);
            }
        }

        function isInEdit() {
            // datagrid vaccinazioni programmate in edit
            if ('<%= Me.dg_vacProg.EditItemIndex.ToString() %>' != '-1') return true;

            // datagrid bilanci programmati in edit
            if ('<%= Me.dg_bilProg.EditItemIndex.ToString() %>' != '-1') return true;

            return false;
        }

        function clearEseMalPag() {
            var ddl = document.getElementById('<%= Me.ddlEseMalPag.ClientID %>');
            if (ddl != null) ddl.selectedIndex = 0;
        }

        function modalitaAccessoClick(tipo) {
            var id = "";
            if (tipo == 'CUP') id = "<%= Me.rb_cup.ClientId %>";
            else if (tipo == 'PS') id = "<%= Me.rb_ps.ClientID%>";
            else if (tipo == 'APP') id = "<%= Me.rb_app.ClientID%>";
            else if (tipo == 'VOL') id = "<%= Me.rb_vol.ClientID%>";

            if (id != "") document.getElementById(id).click();
        }

    </script>

    <form id="Form1" method="post" runat="server">

        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Width="100%" Height="100%" Titolo="Vaccinazioni Programmate" TitleCssClass="Title3">

            <asp:Panel ID="PanelTitolo" runat="server" CssClass="title" Width="100%">
                <asp:Label ID="LayoutTitolo" runat="server"></asp:Label>
            </asp:Panel>

            <div>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBar" runat="server" ItemWidthDefault="70px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                    <Items>
                        <igtbar:TBarButton Key="btn_Cnv" Text="CNV" DisabledImage="~/Images/indietro_dis.gif" Image="~/Images/indietro.gif" ToolTip="Ritorna a elenco convocazioni">
                            <DefaultStyle Width="45px" CssClass="infratoolbar_button_default"></DefaultStyle>
                        </igtbar:TBarButton>
                        <igtbar:TBSeparator></igtbar:TBSeparator>
                        <igtbar:TBarButton Key="btn_Salva" Text="Salva" DisabledImage="~/Images/salva_dis.gif" Image="~/Images/salva.gif">
                        </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btn_Annulla" Text="Annulla" DisabledImage="~/Images/annulla_dis.gif" Image="~/Images/annulla.gif">
                        </igtbar:TBarButton>
                        <igtbar:TBSeparator></igtbar:TBSeparator>
                        <igtbar:TBarButton Key="btn_Esegui" Text="Esegui" DisabledImage="../../images/esegui_dis.gif" Image="../../images/esegui.gif">
                        </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btn_Escludi" Text="Escludi" DisabledImage="../../images/escludi_dis.gif" Image="../../images/escludi.gif">
                        </igtbar:TBarButton>
                        <igtbar:TBSeparator></igtbar:TBSeparator>
                        <igtbar:TBarButton Key="btn_InsAssociazione" Text="Ins.Ass." DisabledImage="~/Images/nuovo_dis.gif" Image="~/Images/nuovo.gif" ToolTip="Aggiunge associazioni alla programmazione corrente">
                            <DefaultStyle Width="70px" CssClass="infratoolbar_button_default"></DefaultStyle>
                        </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btn_AssLotti" Text="Ass.Lotti" DisabledImage="../../images/ricalcola_dis.gif" Image="../../images/ricalcola.gif">
                            <DefaultStyle Width="75px" CssClass="infratoolbar_button_default"></DefaultStyle>
                        </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btn_InsLotto" Text="Ins.Lotto" DisabledImage="~/Images/nuovo_dis.gif" Image="~/Images/nuovo.gif" ToolTip="Inserimento lotto in anagrafe lotti">
                            <DefaultStyle Width="70px" CssClass="infratoolbar_button_default"></DefaultStyle>
                        </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btn_AddBil" Text="Ins.Bil" DisabledImage="~/Images/nuovo_dis.gif" Image="~/Images/nuovo.gif" ToolTip="Aggiunta bilanci alla programmazione corrente">
                        </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btn_VisioneBilanci" Text="Anamnesi" DisabledImage="../../images/bilanci_dis.gif" Image="../../images/bilanci.gif" ToolTip="Visione anamnesi">
                            <DefaultStyle Width="100px" CssClass="infratoolbar_button_default"></DefaultStyle>
                        </igtbar:TBarButton>
                        <igtbar:TBSeparator></igtbar:TBSeparator>
                        <igtbar:TBarButton Key="btn_CertificatoVaccinale" Text="Certif." DisabledImage="../../images/stampa_dis.gif" Image="../../images/stampa.gif" ToolTip="Visualizza l'anteprima di stampa del certificato vaccinale del paziente">
                        </igtbar:TBarButton>
                    </Items>
                </igtbar:UltraWebToolbar>
            </div>

            <asp:Panel ID="LayoutTitolo_cnv" runat="server" Width="100%">
                <asp:Panel ID="PanelTitoloSezione" runat="server" CssClass="vac-sezione" >
                    <asp:Label ID="LayoutTitolo_sezioneCnv" runat="server">DATI CONVOCAZIONE</asp:Label>
                </asp:Panel>
                <table style="height: 160px" width="100%" border="0">
                    <tr style="height: 70%">
                        <td style="padding-right: 2px" width="36%" rowspan="2">
                            <igtab:UltraWebTab BrowserTarget="UpLevel" ID="TabConvocazione" runat="server" Width="100%" BorderStyle="Solid" Height="100%"
                                BorderWidth="2px" BorderColor="#485D96" ThreeDEffect="False">
                                <DefaultTabStyle Height="18px" Font-Size="6pt" Font-Names="Microsoft Sans Serif" ForeColor="Black"
                                    BackColor="#FBF9F9">
                                    <Padding Top="2px"></Padding>
                                </DefaultTabStyle>
                                <RoundedImage RightSideWidth="2" NormalImage="..\..\Images\tab_vacProg.gif" FillStyle="LeftMergedWithCenter"></RoundedImage>
                                <SelectedTabStyle Font-Size="9px" Font-Bold="True">
                                    <Padding Bottom="2px" Left="4px" Right="4px"></Padding>
                                </SelectedTabStyle>
                                <Tabs>
                                    <igtab:Tab Text="CONVOCAZIONE" Key="tabCnvConvocazione">
                                        <ContentTemplate>
                                            <table cellspacing="0" cellpadding="2" width="99%" border="0">
                                                <colgroup style="width: 100%; height: 100%">
                                                    <col width="20%" />
                                                    <col width="40%" />
                                                    <col width="35%" />
                                                    <col width="5%" />
                                                </colgroup>
                                                <tr>
                                                    <td class="label">Centro Vacc.</td>
                                                    <td colspan="3">
                                                        <asp:TextBox ID="tb_cnv_cons" runat="server" Width="100%" CssClass="TextBox_TitoloStringa_Disabilitato"
                                                            ReadOnly="True"></asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <td class="label">Convocazione</td>
                                                    <td>
                                                        <asp:TextBox ID="tb_cnv_dataConv" runat="server" Width="100%" CssClass="TextBox_Data_Disabilitato"
                                                            Font-Bold="True" ReadOnly="True" BorderStyle="Solid" BorderWidth="1px" BorderColor="black"></asp:TextBox></td>
                                                    <td align="right">
                                                        <asp:Label ID="lblInCampagna" runat="server" CssClass="label" Text="Campagna"></asp:Label></td>
                                                    <td>
                                                        <asp:CheckBox ID="chkInCampagna" AutoPostBack="True" runat="server"></asp:CheckBox></td>
                                                </tr>
                                                <tr>
                                                    <td class="label">Età Paziente</td>
                                                    <td colspan="3">
                                                        <asp:TextBox ID="tb_cnv_etaConv" runat="server" Width="100%" CssClass="Textbox_TitoloStringa_Disabilitato"
                                                            ReadOnly="True"></asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <td class="label">Bilancio</td>
                                                    <td>
                                                        <asp:TextBox ID="tb_bilancio" Style="width: 100%" runat="server" CssClass="Textbox_TitoloNumero_Disabilitato"
                                                            ReadOnly="True"></asp:TextBox></td>
                                                    <td colspan="2"></td>
                                                </tr>
                                                <tr>
                                                    <td class="label">Malattia</td>
                                                    <td>
                                                        <asp:TextBox ID="tb_malattia" Style="width: 100%" runat="server" CssClass="TextBox_TitoloNumero_Disabilitato"
                                                            ReadOnly="True"></asp:TextBox></td>
                                                    <td colspan="2"></td>
                                                </tr>
                                            </table>
                                        </ContentTemplate>
                                    </igtab:Tab>
                                    <igtab:Tab Text="ESECUZIONE" Key="tabCnvEsecuzione">
                                        <ContentTemplate>
                                            <table cellspacing="0" cellpadding="2" width="100%" border="0">
                                                <colgroup>
                                                    <col width="20%">
                                                    <col width="75%">
                                                    <col width="5%">
                                                </colgroup>
                                                <tr>
                                                    <td class="label">Responsabile</td>
                                                    <td valign="middle">
                                                        <asp:TextBox ID="txtMedicoReferenteReadOnly" runat="server" Width="100%" CssClass="Textbox_TitoloStringa_Disabilitato"
                                                            ReadOnly="True"></asp:TextBox></td>
                                                    <td rowspan="2" valign="middle">
                                                        <asp:ImageButton ID="imbModificaMedicoReferente" runat="server" ToolTip="Modifica il Medico Responsabile e l'ambulatorio"></asp:ImageButton></td>
                                                </tr>
                                                <tr>
                                                    <td class="label">Ambulatorio</td>
                                                    <td valign="middle">
                                                        <asp:TextBox ID="txtAmbulatorioReadOnly" runat="server" Width="100%" CssClass="Textbox_TitoloStringa_Disabilitato"
                                                            ReadOnly="True"></asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <td class="label" valign="middle">Vaccinatore</td>
                                                    <td valign="middle">
                                                        <asp:TextBox ID="txtVaccinatoreReadOnly" runat="server" Width="100%" CssClass="Textbox_TitoloStringa_Disabilitato"
                                                            ReadOnly="True"></asp:TextBox></td>
                                                    <td valign="middle">
                                                        <asp:ImageButton ID="imbModificaVaccinatore" runat="server" ToolTip="Modifica il Vaccinatore della seduta"></asp:ImageButton></td>
                                                </tr>
                                                <tr>
                                                    <td class="label">Data</td>
                                                    <td valign="middle" align="center">
                                                        <asp:TextBox ID="txtDataEs" runat="server" CssClass="TextBox_TitoloData_Disabilitato" Font-Bold="True"
                                                            Width="100%" ReadOnly="True"></asp:TextBox></td>
                                                    <td rowspan="2" valign="middle">
                                                        <asp:ImageButton ID="imbDataEs" runat="server" ToolTip="Modifica la Data di Esecuzione"></asp:ImageButton></td>
                                                </tr>
                                                <tr>
                                                    <td class="label">Ora</td>
                                                    <td valign="middle">
                                                        <asp:TextBox ID="txtOraEs" runat="server" CssClass="TextBox_TitoloData_Disabilitato" Font-Bold="True"
                                                            Width="100%" ReadOnly="True"></asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td align="right">
                                                        <asp:CheckBox ID="chkInAmbulatorio" CssClass="Label" runat="server" TextAlign="Left" Text="Medico in ambulatorio" AutoPostBack="True"></asp:CheckBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td align="right">
                                                        <asp:CheckBox ID="chkFlagVisibilita" CssClass="Label" runat="server" TextAlign="Left" Text="<%$ Resources: Onit.OnAssistnet.OnVac.Web, DatiVaccinali.ConsensoComunicazione %>" AutoPostBack="false" ToolTip="Consenso alla comunicazione dei dati vaccinali da parte del paziente"></asp:CheckBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </ContentTemplate>
                                    </igtab:Tab>
                                    <igtab:Tab Text="PAGAMENTO" Key="tabCnvPagamento">
                                        <ContentTemplate>
                                            <table cellspacing="0" cellpadding="2" width="100%" border="0">
                                                <colgroup>
                                                    <col width="28%">
                                                    <col width="67%">
                                                    <col width="5%">
                                                </colgroup>
                                                <tr>
                                                    <td class="label">Esenzione</td>
                                                    <td valign="middle">
                                                        <asp:DropDownList ID="ddlEseMalPag" Width="100%" AutoPostBack="true" CssClass="TextBox_Stringa" runat="server" />
                                                    </td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td class="label">Ticket</td>
                                                    <td valign="middle">
                                                        <asp:TextBox ID="txtImpTotPag" runat="server" Width="100%" CssClass="TextBox_Numerico_Disabilitato" Font-Bold="True"
                                                            BorderStyle="Solid" BorderWidth="1px" BorderColor="black" ReadOnly="True"></asp:TextBox>
                                                    </td>
                                                    <td></td>
                                                </tr>
                                            </table>
                                        </ContentTemplate>
                                    </igtab:Tab>
                                </Tabs>
                            </igtab:UltraWebTab>
                        </td>
                        <td style="padding-right: 2px; padding-left: 2px" width="34%" rowspan="2">
                            <igtab:UltraWebTab BrowserTarget="UpLevel" ID="TabAppuntamento" runat="server" Width="100%" BorderStyle="Solid" Height="100%"
                                BorderWidth="2px" BorderColor="#485D96" ThreeDEffect="False">
                                <DefaultTabStyle Height="18px" Font-Size="6pt" Font-Names="Microsoft Sans Serif" ForeColor="Black"
                                    BackColor="#FBF9F9">
                                    <Padding Top="2px"></Padding>
                                </DefaultTabStyle>
                                <RoundedImage RightSideWidth="2" NormalImage="..\..\Images\tab_vacProg.gif" FillStyle="LeftMergedWithCenter"></RoundedImage>
                                <SelectedTabStyle Font-Size="9px" Font-Bold="True">
                                    <Padding Bottom="2px" Left="4px" Right="4px"></Padding>
                                </SelectedTabStyle>
                                <Tabs>
                                    <igtab:Tab Text="APPUNTAMENTO" Key="tabAppuntamento">
                                        <ContentTemplate>
                                            <table cellspacing="0" cellpadding="2" width="100%" border="0">
                                                <colgroup>
                                                    <col width="30%">
                                                    <col width="65%">
                                                    <col width="5%">
                                                </colgroup>
                                                <tr>
                                                    <td class="label">Data App.</td>
                                                    <td>
                                                        <asp:TextBox ID="tb_cnv_app" runat="server" Width="100%" CssClass="Textbox_TitoloStringa_Disabilitato"
                                                            Font-Bold="True" ReadOnly="True"></asp:TextBox></td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td class="label">Ambulatorio</td>
                                                    <td>
                                                        <asp:TextBox ID="tb_cnv_app_amb" runat="server" Width="100%" CssClass="Textbox_TitoloStringa_Disabilitato"
                                                            Font-Bold="True" ReadOnly="True"></asp:TextBox></td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td class="label">Data Invio</td>
                                                    <td>
                                                        <asp:TextBox ID="tb_cnv_dataInv" runat="server" Width="100%" CssClass="Textbox_TitoloStringa_Disabilitato"
                                                            ReadOnly="True"></asp:TextBox></td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td class="label">Durata</td>
                                                    <td>
                                                        <asp:TextBox ID="tb_cnv_durApp" runat="server" Width="40%" CssClass="Textbox_TitoloNumero_Disabilitato"
                                                            ReadOnly="True"></asp:TextBox></td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td class="label">
                                                        <asp:Label ID="lblPrimoAppuntamento" runat="server" CssClass="label">Primo App.</asp:Label></td>
                                                    <td>
                                                        <asp:TextBox ID="txtPrimoAppuntamento" runat="server" Width="100%" CssClass="Textbox_TitoloStringa_Disabilitato"
                                                            ReadOnly="True"></asp:TextBox></td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td class="label">
                                                        <asp:Label ID="lblUtenteAppuntamento" runat="server" CssClass="label">Assegnato da</asp:Label></td>
                                                    <td>
                                                        <asp:TextBox ID="txtUtenteAppuntamento" runat="server" Width="100%" CssClass="Textbox_TitoloStringa_Disabilitato"
                                                            ReadOnly="True"></asp:TextBox></td>
                                                    <td align="center">
                                                        <asp:Image ID="imgTipoAppuntamento" runat="server"></asp:Image></td>
                                                </tr>
                                            </table>
                                            <table id="tbModalitaAccesso" cellspacing="0" cellpadding="0" width="100%" border="0" runat="server">
                                                <colgroup>
                                                    <col width="30%" align="right" />
                                                    <col width="1%" />
                                                    <col width="27%" />
                                                    <col width="1%" />
                                                    <col width="36%" />
                                                    <col width="5%" />
                                                </colgroup>
                                                <tr>
                                                    <td rowspan="2">
                                                        <asp:Label ID="lblModalitaAccesso" Width="100%" class="label" runat="server" Font-Bold="True">Modalità<br />di accesso</asp:Label></td>
                                                    <td title="Prenotazione da CUP" align="right">
                                                        <asp:RadioButton ID="rb_cup" CssClass="label" runat="server" GroupName="ModalitaAccesso"></asp:RadioButton></td>
                                                    <td class="label_left" title="Prenotazione da CUP" onclick="modalitaAccessoClick('CUP')">CUP</td>
                                                    <td title="Accesso da Pronto Soccorso" align="right">
                                                        <asp:RadioButton ID="rb_ps" CssClass="label" runat="server" GroupName="ModalitaAccesso"></asp:RadioButton></td>
                                                    <td class="label_left" title="Accesso da Pronto Soccorso" onclick="modalitaAccessoClick('PS')">P.S.</td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td title="Accesso Volontario" align="right">
                                                        <asp:RadioButton ID="rb_vol" CssClass="label" runat="server" GroupName="ModalitaAccesso"></asp:RadioButton></td>
                                                    <td class="label_left" title="Accesso Volontario" onclick="modalitaAccessoClick('VOL')">Volontario</td>
                                                    <td title="Appuntamento da SIAVr" align="right">
                                                        <asp:RadioButton ID="rb_app" CssClass="label" runat="server" GroupName="ModalitaAccesso"></asp:RadioButton></td>
                                                    <td class="label_left" title="Appuntamento da SIAVr" onclick="modalitaAccessoClick('APP')">App. OnVac</td>
                                                    <td></td>
                                                </tr>
                                            </table>
                                        </ContentTemplate>
                                    </igtab:Tab>
                                </Tabs>
                            </igtab:UltraWebTab>
                        </td>
                        <td style="padding-left: 2px" width="30%">
                            <igtab:UltraWebTab BrowserTarget="UpLevel" ID="TabRitardi" runat="server" Width="100%" BorderStyle="Solid" Height="100%" BorderWidth="2px"
                                BorderColor="#485D96" ThreeDEffect="False">
                                <DefaultTabStyle Height="18px" Font-Size="9px" Font-Names="Microsoft Sans Serif" ForeColor="Black" BackColor="#FBF9F9">
                                    <Padding Bottom="2px" Left="4px" Right="4px"></Padding>
                                </DefaultTabStyle>
                                <RoundedImage RightSideWidth="3" NormalImage="..\..\Images\tab_vacProg.gif" FillStyle="LeftMergedWithCenter"></RoundedImage>
                                <SelectedTabStyle Font-Bold="True"></SelectedTabStyle>
                                <Tabs>
                                    <igtab:Tab Text="RITARDI VACC." Key="tabRitardiVacc">
                                        <ContentTemplate>
                                            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                                <colgroup>
                                                    <col width="35%" valign="middle" />
                                                    <col width="75%" />
                                                </colgroup>
                                                <tr>
                                                    <td valign="middle" class="label_center">
                                                        <asp:Label ID="lblRitardoInt" runat="server"></asp:Label><br />
                                                        <asp:Label ID="lblRitardo" runat="server" Width="20px" Style="margin-top: 5px"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <div style="height: 110px; overflow: auto;">
                                                            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                                                <colgroup>
                                                                    <col width="40%" />
                                                                    <col width="60%" />
                                                                </colgroup>
                                                                <tr valign="top">
                                                                    <td class="label">
                                                                        <asp:Label ID="lblRitardoInt1" runat="server"></asp:Label>
                                                                    </td>
                                                                    <td class="label_center">
                                                                        <asp:Label ID="lblRitardo1" runat="server" Width="100%"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr valign="top" style="padding-top: 2px;">
                                                                    <td id="tdRitardoInt2" runat="server">
                                                                        <asp:Label ID="lblRitardoInt2" runat="server"></asp:Label>
                                                                    </td>
                                                                    <td id="tdRitardo2" runat="server">
                                                                        <asp:Label ID="lblRitardo2" runat="server" Width="100%"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr valign="top" style="padding-top: 2px;">
                                                                    <td id="tdRitardoInt3" runat="server">
                                                                        <asp:Label ID="lblRitardoInt3" runat="server"></asp:Label>
                                                                    </td>
                                                                    <td id="tdRitardo3" runat="server">
                                                                        <asp:Label ID="lblRitardo3" runat="server" Width="100%"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr valign="top" style="padding-top: 2px;">
                                                                    <td id="tdRitardoInt4" runat="server">
                                                                        <asp:Label ID="lblRitardoInt4" runat="server"></asp:Label>
                                                                    </td>
                                                                    <td id="tdRitardo4" runat="server">
                                                                        <asp:Label ID="lblRitardo4" runat="server" Width="100%"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </ContentTemplate>
                                    </igtab:Tab>
                                    <igtab:Tab Text="RITARDI BIL." Key="tabRitardiBilanci">
                                        <ContentTemplate>
                                            <div style="height: 110px; overflow: hidden;">
                                                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td rowspan="7" class="label_center" width="45%">
                                                            <asp:Label ID="lblRitardiSolleciti" runat="server" Width="100%" CssClass="label_center"></asp:Label>&nbsp;</td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </ContentTemplate>
                                    </igtab:Tab>
                                </Tabs>
                            </igtab:UltraWebTab>
                        </td>
                    </tr>
                    <tr style="height: 30%">
                        <td valign="bottom" width="30%">
                            <igtab:UltraWebTab BrowserTarget="UpLevel" ID="TabSospensione" runat="server" Width="100%" BorderStyle="Solid" BorderWidth="2px"
                                BorderColor="#485D96" ThreeDEffect="False">
                                <DefaultTabStyle Height="18px" Font-Size="6pt" Font-Names="Microsoft Sans Serif" ForeColor="Black" BackColor="#FBF9F9">
                                    <Padding Top="2px"></Padding>
                                </DefaultTabStyle>
                                <RoundedImage RightSideWidth="3" NormalImage="..\..\Images\tab_vacProg.gif" FillStyle="LeftMergedWithCenter"></RoundedImage>
                                <SelectedTabStyle Font-Size="9px" Font-Bold="True">
                                    <Padding Bottom="2px" Left="4px" Right="4px"></Padding>
                                </SelectedTabStyle>
                                <Tabs>
                                    <igtab:Tab Text="SOSPENSIONE">
                                        <ContentTemplate>
                                            <table border="0" width="100%">
                                                <tr>
                                                    <td width="25%" class="label">Sospensione
                                                    </td>
                                                    <td width="75%">
                                                        <asp:TextBox ID="txtSospensione" runat="server" CssClass="Textbox_TitoloStringa_Disabilitato"
                                                            Width="100%" ReadOnly="True" ForeColor="Red"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </ContentTemplate>
                                    </igtab:Tab>
                                </Tabs>
                            </igtab:UltraWebTab>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <asp:Panel ID="PanelTitolo_sezione" runat="server" CssClass="vac-sezione">
                <asp:Label ID="LayoutTitolo_sezione" runat="server">ELENCO VACCINAZIONI</asp:Label>
            </asp:Panel>

            <asp:Panel ID="LayoutTitolo_leg" runat="server" Width="100%" CssClass="legenda-vaccinazioni">
                <span class="legenda-vaccinazioni-reazione">R</span>
                <span>Reazione Avversa</span>
                <span class="legenda-vaccinazioni-esclusa">X</span>
                <span>Vaccinazione Esclusa</span>
                <span class="legenda-vaccinazioni-eseguita">E</span>
                <span>Vaccinazione Eseguita</span>
                <span class="legenda-vaccinazioni-obbligatoria">O</span>
                <span>Vaccinazione Obbligatoria</span>
            </asp:Panel>

            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="99%" ScrollBars="Auto" RememberScrollPosition="true">

                <asp:DataGrid ID="dg_vacProg" Style="table-layout: auto;" runat="server" CssClass="DATAGRID" Width="100%" ItemStyle-Height="25px"
                    AllowSorting="True" AutoGenerateColumns="False" CellPadding="1" GridLines="None">
                    <FooterStyle ForeColor="#4A3C8C" BackColor="#B5C7DE" />
                    <SelectedItemStyle CssClass="selected" />
                    <AlternatingItemStyle CssClass="alternating" />
                    <ItemStyle CssClass="item" />
                    <HeaderStyle Font-Bold="True" CssClass="header" />
                    <PagerStyle CssClass="pager" Mode="NumericPages" />
                    <Columns>
                        <asp:TemplateColumn>
                            <HeaderStyle HorizontalAlign="Center" Width="1%" />
                            <ItemStyle HorizontalAlign="Center" />
                            <HeaderTemplate>
                                <input id="cb_All" type="checkbox" onclick="CheckAll('dg_vacProg', this.checked, 0, 0)" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="cb" runat="server"></asp:CheckBox>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="cb_edit" runat="server" Enabled="False"></asp:CheckBox>
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn>
                            <HeaderStyle HorizontalAlign="Center" Width="1%" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:ImageButton ID="btnDelete" runat="server" OnClientClick="confirmDelete(event)"
                                    AlternateText="Elimina Vaccinazione Programmata"
                                    ImageUrl="~/Images/elimina.gif"
                                    CommandName="Delete" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn>
                            <HeaderStyle HorizontalAlign="Center" Width="1%" />
                            <ItemStyle HorizontalAlign="Center" Wrap="false" />
                            <ItemTemplate>
                                <asp:ImageButton ID="btnEdit" runat="server" OnClientClick="confirmEdit(event)"
                                    AlternateText="Modifica Vaccinazione Programmata"
                                    ImageUrl="~/Images/modifica.gif"
                                    CommandName="Edit" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="btnUpdate" runat="server" OnClientClick="controllaConDoseStessa(this,event)"
                                    AlternateText="Conferma Modifiche Vaccinazione Programmata" ToolTip="Conferma Modifiche Vaccinazione Programmata"
                                    ImageUrl="~/Images/conferma.gif"
                                    CommandName="Update" />
                                <asp:ImageButton ID="btnCancel" runat="server" CssClass="vac-img-btn"
                                    AlternateText="Annulla Modifiche Vaccinazione Programmata" ToolTip="Annulla Modifiche Vaccinazione Programmata"
                                    ImageUrl="~/Images/annulla.gif"
                                    CommandName="Cancel" />
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Assoc.&lt;br&gt;Dose">
                            <HeaderStyle HorizontalAlign="Left" Width="11%" />
                            <ItemStyle HorizontalAlign="Left" Font-Size="11px" />
                            <ItemTemplate>
                                <asp:Label ID="lblCodAss" runat="server" Text='<%# DescrizioneAssociazioneDose(Eval("vpr_ass_codice"), Eval("ves_ass_n_dose")) %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="lblCodAss" runat="server" Text='<%# DescrizioneAssociazioneDose(Eval("vpr_ass_codice"), Eval("ves_ass_n_dose")) %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn>
                            <HeaderStyle HorizontalAlign="Center" Width="1%" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" CssClass="legenda-vaccinazioni-obbligatoria" ToolTip="Vaccinazione obbligatoria"
                                    Visible='<%# IsVaccinazioneObbligatoria(Eval("vac_obbligatoria")) %>'>O</asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn>
                            <HeaderStyle Width="1%" />
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" CssClass="legenda-vaccinazioni-reazione" ToolTip="Reazione avversa"
                                    Visible='<%# IIf(Eval("vra_data_reazione") Is DBNull.Value, False, True) %>'>R</asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Vaccinazione">
                            <HeaderStyle HorizontalAlign="Left" Width="14%" />
                            <ItemStyle HorizontalAlign="Left" Font-Size="11px" />
                            <ItemTemplate>
                                <asp:Label ID="lblDescVac" runat="server" Font-Bold="True" Text='<%# DataBinder.Eval(Container, "DataItem")("vac_descrizione") %>'>
                                </asp:Label>
                                <asp:Label ID="lblCodVac" Text='<%# DataBinder.Eval(Container, "DataItem")("vpr_vac_codice") %>' Visible="False" runat="server">
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="lblDescVac" runat="server" Font-Bold="True" Text='<%# DataBinder.Eval(Container, "DataItem")("vac_descrizione") %>'>
                                </asp:Label>
                                <asp:Label ID="lblCodVac" Text='<%# DataBinder.Eval(Container, "DataItem")("vpr_vac_codice") %>' Style="display: none" runat="server">
                                </asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Dose&lt;br&gt;Vacc.">
                            <HeaderStyle HorizontalAlign="Center" Width="5%" />
                            <ItemStyle HorizontalAlign="Center" Font-Size="11px" />
                            <ItemTemplate>
                                <asp:Label ID="lblNumRich" runat="server" Font-Bold="True" Text='<%# DataBinder.Eval(Container, "DataItem")("vpr_n_richiamo") %>'>
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNumRich" TabIndex="2" runat="server" Width="100%" CssClass="TextBox_Numerico_Obbligatorio" Text='<%# DataBinder.Eval(Container, "DataItem")("vpr_n_richiamo") %>' Style="text-align: center" Font-Size="11px">
                                </asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Lotto">
                            <HeaderStyle HorizontalAlign="Left" Width="11%" />
                            <ItemStyle HorizontalAlign="Left" Font-Size="12px" />
                            <ItemTemplate>
                                <asp:Label ID="lblLotto" Text='<%# DataBinder.Eval(Container, "DataItem")("ves_lot_codice") %>' runat="server" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Nome&lt;br&gt;Commerc.">
                            <HeaderStyle HorizontalAlign="Left" Width="13%" />
                            <ItemStyle HorizontalAlign="Left" Font-Size="11px" />
                            <ItemTemplate>
                                <asp:Label ID="lblDescNC" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("noc_descrizione") %>'></asp:Label>
                                <asp:HiddenField ID="hidCodNC" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem")("ves_noc_codice") %>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Via&lt;br&gt;Sommin.">
                            <HeaderStyle HorizontalAlign="Left" Width="9%" />
                            <ItemStyle Font-Size="11px" />
                            <ItemTemplate>
                                <asp:DropDownList ID="ddlVii" runat="server" CssClass="TextBox_Stringa" Font-Size="11px" Width="100%"
                                    AutoPostBack="true" OnSelectedIndexChanged="ddlVii_SelectedIndexChanged">
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Inoculazione">
                            <HeaderStyle HorizontalAlign="Left" Width="9%" />
                            <ItemStyle HorizontalAlign="Left" Font-Size="11px" />
                            <ItemTemplate>
                                <asp:DropDownList ID="ddlSii" runat="server" CssClass="TextBox_Stringa" Font-Size="11px" Width="100%"
                                    AutoPostBack="true" OnSelectedIndexChanged="ddlSii_SelectedIndexChanged">
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn>
                            <HeaderStyle HorizontalAlign="Left" Width="4%" />
                            <ItemStyle HorizontalAlign="Left" Font-Size="11px" />
                            <HeaderTemplate>
                                <div title="Condizione Sanitaria">
                                    Cond.<br />
                                    Sanit.
                                </div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <on_ofm:OnitModalList ID="omlCondSanitaria" runat="server" Width="0%" LabelWidth="-8px" SetUpperCase="True"
                                    PosizionamentoFacile="False" Obbligatorio="False" Font-Size="11px"
                                    OnSetUpFiletr="omlCondSanitaria_SetUpFiletr" OnChange="omlCondSanitaria_Change"
                                    AltriCampi="VCS_PAZ_MAL_CODICE Paz, VCS_COND_SANITARIA_DEFAULT Def"
                                    Tabella="V_CONDIZIONI_SANITARIE" CampoCodice="VCS_MAL_CODICE Codice" CampoDescrizione="VCS_MAL_DESCRIZIONE Descrizione"
                                    RaiseChangeEvent="True" CodiceWidth="99%" UseCode="True" IsDistinct="true" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Cond.&lt;br&gt;Risc.">
                            <HeaderStyle HorizontalAlign="Left" Width="4%" />
                            <ItemStyle HorizontalAlign="Left" Font-Size="11px" />
                            <HeaderTemplate>
                                <div title="Condizione di Rischio">
                                    Cond.<br />
                                    Risc.
                                </div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <on_ofm:OnitModalList ID="omlCondRischio" runat="server" Width="0%" LabelWidth="-8px" SetUpperCase="True"
                                    PosizionamentoFacile="False" Obbligatorio="False" Font-Size="11px"
                                    OnSetUpFiletr="omlCondRischio_SetUpFiletr" OnChange="omlCondRischio_Change"
                                    AltriCampi="VCR_PAZ_RSC_CODICE Paz, VCR_RISCHIO_DEFAULT Def"
                                    Tabella="V_CONDIZIONI_RISCHIO" CampoCodice="VCR_CODICE_RISCHIO Codice" CampoDescrizione="VCR_DESCRIZIONE_RISCHIO Descrizione"
                                    RaiseChangeEvent="True" CodiceWidth="99%" UseCode="True" IsDistinct="true" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn>
                            <HeaderStyle HorizontalAlign="Center" Width="3%" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkPagVac" runat="server" OnClick="lnkPagVac_Click" Text="€" Style="font-weight: bold"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn>
                            <HeaderStyle Width="1%" />
                            <ItemTemplate>
                                <asp:ImageButton ID="btnNoteVac" runat="server" OnClick="btnNoteVac_Click" ToolTip="Note" AlternateText="Note"></asp:ImageButton>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn>
                            <HeaderStyle Width="1%" />
                            <ItemTemplate>
                                <asp:ImageButton ID="btnInfoVac" runat="server" OnClick="btnInfoVac_Click" ImageUrl="~/images/info.png" Style="cursor: auto;" ToolTip="Info" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn>
                            <HeaderStyle Width="1%" />
                            <ItemTemplate>
                                <asp:Label ID="lb_es" runat="server" CssClass="legenda-vaccinazioni-eseguita"
                                    ToolTip='<%# OnVacUtility.LegendaVaccinazioniBindToolTip(Enumerators.LegendaVaccinazioniItemType.Eseguita) %>'
                                    Visible='<%# OnVacUtility.LegendaVaccinazioniBindVisibility(Eval("E"), Enumerators.LegendaVaccinazioniItemType.Eseguita) %>'>E</asp:Label>
                                <asp:Label ID="lb_ex" runat="server" CssClass="legenda-vaccinazioni-esclusa" 
                                    ToolTip='<%# OnVacUtility.LegendaVaccinazioniBindToolTip(Enumerators.LegendaVaccinazioniItemType.Esclusa) %>'
                                    Visible='<%# BindVisibilityScadenza(Eval("E"), Eval("VEX_DATA_SCADENZA")) %>'>X</asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Ciclo" Visible="false">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Left" />
                            <ItemTemplate>
                                <asp:Label ID="lblDescCic" Text='<%# DataBinder.Eval(Container, "DataItem")("cic_descrizione") %>' runat="server" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="lblDescCic" Text='<%# DataBinder.Eval(Container, "DataItem")("cic_descrizione") %>' runat="server" />
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Sed." Visible="false">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Label ID="lblNumSed" Text='<%# DataBinder.Eval(Container, "DataItem")("vpr_n_seduta") %>' runat="server" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="lblNumSed" Text='<%# DataBinder.Eval(Container, "DataItem")("vpr_n_seduta") %>' runat="server" />
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Dose Assoc." Visible="false">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" Font-Size="11px" />
                            <ItemTemplate>
                                <asp:Label ID="lblDoseAss" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("ves_ass_n_dose") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="lblDoseAss" Text='<%# DataBinder.Eval(Container, "DataItem")("ves_ass_n_dose") %>' runat="server" />
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Vaccinatore" Visible="false">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                            <ItemTemplate>
                                <asp:Label ID="lblMedico" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("ope_vac") %>'>
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <on_ofm:OnitModalList ID="omlMedico" TabIndex="5" runat="server" Width="100%" PosizionamentoFacile="False" LabelWidth="-1px" Obbligatorio="False" UseCode="False" SetUpperCase="True" CampoCodice="ope_codice" CampoDescrizione="ope_nome" CodiceWidth="0px" Tabella="t_ana_operatori" Filtro="'true'='true' order by ope_nome" RaiseChangeEvent="False" Descrizione='<%# DataBinder.Eval(Container, "DataItem")("ope_vac") %>' Connection="" Codice='<%# DataBinder.Eval(Container, "DataItem")("ves_med_vaccinante") %>'></on_ofm:OnitModalList>
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>

                <asp:Panel ID="LayoutTitolo_bil" runat="server" CssClass="vac-sezione" Visible="false">
                    <div id="divBil" style="width: 100%; height: 1px; background-color: white"></div>
                    <asp:Label ID="Label5" runat="server">ELENCO BILANCI</asp:Label>
                </asp:Panel>

                <asp:Panel ID="pan_legBil" runat="server" Width="100%" Visible="false">
                    <table class="datagrid" id="tb_legendaBil" cellspacing="0" cellpadding="3" width="100%" border="0">
                        <tr>
                            <td width="1"><span class="box" style="background-color: #ff0000">&nbsp;X&nbsp;</span></td>
                            <td>Bilancio Escluso</td>
                        </tr>
                    </table>
                </asp:Panel>

                <asp:DataGrid ID="dg_bilProg" Style="table-layout: auto" runat="server" CssClass="DATAGRID" Width="100%" Visible="false"
                    AllowSorting="True" AutoGenerateColumns="False" CellPadding="1" GridLines="None">
                    <FooterStyle ForeColor="#4A3C8C" BackColor="#B5C7DE"></FooterStyle>
                    <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
                    <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
                    <ItemStyle CssClass="item"></ItemStyle>
                    <HeaderStyle Font-Bold="True" CssClass="header"></HeaderStyle>
                    <Columns>
                        <asp:TemplateColumn>
                            <HeaderStyle HorizontalAlign="left" Width="20px"></HeaderStyle>
                            <ItemStyle HorizontalAlign="left"></ItemStyle>
                            <ItemTemplate>
                                <asp:ImageButton ID="btnEscBil" AlternateText="Escludi bilancio"
                                    ImageUrl="~/Images/annullaconf.gif"
                                    CommandName="EscludiBilancio"
                                    runat="server" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn>
                            <HeaderStyle HorizontalAlign="left" Width="20px"></HeaderStyle>
                            <ItemStyle HorizontalAlign="left"></ItemStyle>
                            <ItemTemplate>
                                <asp:ImageButton ID="btnDelBil" AlternateText="Elimina bilancio"
                                    ImageUrl="~/Images/elimina.gif"
                                    CommandName="Delete"
                                    runat="server" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn>
                            <HeaderStyle HorizontalAlign="left" Width="40px"></HeaderStyle>
                            <ItemStyle HorizontalAlign="left"></ItemStyle>
                            <ItemTemplate>
                                <asp:ImageButton ID="btnEditBil" AlternateText="Modifica bilancio"
                                    ImageUrl="~/Images/modifica.gif"
                                    CommandName="Edit"
                                    runat="server" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="btnUpdBil" AlternateText="Aggiorna bilancio"
                                    ImageUrl="~/Images/conferma.gif"
                                    CommandName="Update"
                                    runat="server" />
                                <asp:ImageButton ID="btnCancBil" AlternateText="Annulla bilancio"
                                    ImageUrl="~/Images/annulla.gif"
                                    CommandName="Cancel"
                                    runat="server" />
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Malattia">
                            <HeaderStyle HorizontalAlign="center" Width="30%"></HeaderStyle>
                            <ItemStyle HorizontalAlign="center"></ItemStyle>
                            <ItemTemplate>
                                <asp:Label ID="oml_malattiaDis" runat="server" Font-Bold="true" Text='<%# DataBinder.Eval(Container, "DataItem")("mal_descrizione") %>'>
                                </asp:Label>
                                <asp:TextBox ID="oml_codMalattiaDis" runat="server" Width="0px" Height="15px" Text='<%# DataBinder.Eval(Container, "DataItem")("bip_mal_codice") %>' Visible="False">
                                </asp:TextBox>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="cmbBilancio" Width="80%" BorderColor="Transparent"
                                    Enabled="True" CssClass="TextBox_Stringa" runat="server">
                                </asp:DropDownList><br>
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Numero bilancio">
                            <HeaderStyle HorizontalAlign="center" Width="15%"></HeaderStyle>
                            <ItemStyle HorizontalAlign="center"></ItemStyle>
                            <ItemTemplate>
                                <asp:Label ID="cmbBilancioDis" Font-Bold="True" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "bip_bil_numero") %>'>
                                </asp:Label><br />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="cmbBilancioDis_e" Font-Bold="True" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "bip_bil_numero") %>'>
                                </asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Data Invio">
                            <HeaderStyle HorizontalAlign="center" Width="15%"></HeaderStyle>
                            <ItemStyle HorizontalAlign="center"></ItemStyle>
                            <ItemTemplate>
                                <asp:Label ID="Label4" Font-Bold="True" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "bip_data_invio") %>'>
                                </asp:Label><br />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label6" Font-Bold="True" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "bip_data_invio") %>'>
                                </asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="">
                            <HeaderStyle HorizontalAlign="center" Width="40%"></HeaderStyle>
                            <ItemStyle HorizontalAlign="center"></ItemStyle>
                            <ItemTemplate>
                                <asp:Label ID="lbl_stato" runat="server" Visible="False" Text='<%# DataBinder.Eval(Container, "DataItem")("bip_stato") %>'>
                                </asp:Label><br>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn>
                            <HeaderStyle Width="32px"></HeaderStyle>
                            <ItemTemplate>
                                <asp:Label ID="lbl_exBilancio" CssClass="box" Style="background-color: #ff0000;" runat="server" Visible='<%# IIF(DataBinder.Eval(Container, "DataItem")("escluso") is dbNull.value orelse DataBinder.Eval(Container, "DataItem")("escluso")="", False, True)%>'>&nbsp;X&nbsp;</asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="" Visible="True">
                            <ItemTemplate>
                                <asp:Label ID="lbl_id" runat="server" Visible="False" Text='<%# DataBinder.Eval(Container, "DataItem")("ID") %>'>
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>

            </dyp:DynamicPanel>

            <on_ofm:OnitFinestraModale ID="modAssLotto" Title="Associa lotti" runat="server" Width="520px" NoRenderX="false" BackColor="LightGray">
                <uc1:AssLotti ID="uscAssLotti" runat="server"></uc1:AssLotti>
            </on_ofm:OnitFinestraModale>

            <on_ofm:OnitFinestraModale ID="modInsDatiEsc" Title="Inserisci dati esclusione" runat="server" BackColor="LightGray" Width="480px" Height="330px">
                <uc1:InsDatiEsc ID="uscInsDatiEsc" runat="server"></uc1:InsDatiEsc>
            </on_ofm:OnitFinestraModale>

            <on_ofm:OnitFinestraModale ID="modInsLotto" Title="Inserisci lotto" NoRenderX="false" runat="server" Width="650px" BackColor="LightGray">
                <uc1:InsLotto ID="uscInsLotto" runat="server"></uc1:InsLotto>
            </on_ofm:OnitFinestraModale>

            <on_ofm:OnitFinestraModale ID="modInsAssociazione" Title="Inserisci associazione" runat="server" Width="610px" BackColor="LightGray" NoRenderX="True">
                <uc1:InsAssociazione ID="uscInsAssociazione" runat="server"></uc1:InsAssociazione>
            </on_ofm:OnitFinestraModale>

            <on_ofm:OnitFinestraModale ID="modMedLogin" Title="Login per la seduta corrente" runat="server" Width="450px" BackColor="LightGray"
                NoRenderX="false" ZIndexPosition="1" IsAnagrafe="False" RenderModalNotVisible="True">
                <table class="vacOrangeTable" style="width: 100%">
                    <tr>
                        <td>
                            <fieldset class="fldroot" style="padding: 10px">
                                <legend class="label_left" style="margin-bottom: 5px">Medico Responsabile</legend>
                                <on_ofm:OnitModalList ID="txtMedicoResponsabile" runat="server" Width="70%"
                                    PosizionamentoFacile="False" LabelWidth="-1px" Obbligatorio="False" UseCode="True" SetUpperCase="True" CampoCodice="OPE_CODICE Codice"
                                    CampoDescrizione="OPE_NOME Nome" CodiceWidth="29%" Tabella="T_ANA_OPERATORI,T_ANA_LINK_OPER_CONSULTORI" Label="Titolo"></on_ofm:OnitModalList>
                            </fieldset>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <fieldset class="fldroot" style="padding: 10px">
                                <legend class="label_left" style="margin-bottom: 5px">Esecuzione Vaccinazioni</legend>
                                <table width="100%" border="0" cellpadding="2" cellspacing="0">
                                    <colgroup>
                                        <col width="15%" />
                                        <col width="35%" />
                                        <col width="15%" />
                                        <col width="35%" />
                                    </colgroup>
                                    <tr>
                                        <td class="label_left">Vaccinatore</td>
                                        <td class="label_left" colspan="3">
                                            <on_ofm:OnitModalList ID="txtMedicoVaccinante" runat="server" Width="70%"
                                                PosizionamentoFacile="False" LabelWidth="-1px" Obbligatorio="False" UseCode="True" SetUpperCase="True" CampoCodice="OPE_CODICE Codice"
                                                CampoDescrizione="OPE_NOME Nome" CodiceWidth="29%" Tabella="T_ANA_OPERATORI,T_ANA_LINK_OPER_CONSULTORI" Label="Titolo"></on_ofm:OnitModalList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="label_left">Data Esecuzione</td>
                                        <td class="label_left">
                                            <on_val:OnitDatePick ID="odpDataEs" runat="server" Width="120px" CssClass="TextBox_Data" />
                                        </td>
                                        <td class="label">Ora</td>
                                        <td>
                                            <asp:TextBox ID="txtOraEs_MedLogin" Width="40px" CssClass="TextBox_Stringa" onblur="formattaOrario(this)" runat="server" MaxLength="5">
                                            </asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <fieldset class="fldroot" style="padding: 10px">
                                <legend class="label_left" style="margin-bottom: 3px">Ambulatorio</legend>
                                <uc2:SelezioneAmbulatorio ID="uscScegliAmb" runat="server" Tutti="False" SceltaConsultorio="False"></uc2:SelezioneAmbulatorio>
                            </fieldset>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <p align="center">
                                <asp:Button ID="btnOKLogin" runat="server" OnClientClick="btnOKLogin_Click(event);" Width="63px" Text="OK"></asp:Button><br />
                                <asp:Label ID="lblErrorLogin" CssClass="label" runat="server" ForeColor="#ff0000" Visible="False" Text=""></asp:Label>
                            </p>
                        </td>
                    </tr>
                </table>
            </on_ofm:OnitFinestraModale>

            <on_ofm:OnitFinestraModale ID="ofmSceltaAssociazioni" Title="Scelta Associazioni" runat="server" Width="500px" BackColor="LightGray"
                NoRenderX="True" ZIndexPosition="1" IsAnagrafe="False">
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="uwtSceltaAssociazioni" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                    <ClientSideEvents InitializeToolbar="InizializzaToolBar_SceltaAss" Click="ToolBarClick_SceltaAss"></ClientSideEvents>
                    <Items>
                        <igtbar:TBarButton Key="btn_Conferma" Text="Conferma" DisabledImage="~/Images/conferma_dis.gif" Image="~/Images/conferma.gif">
                        </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btn_Annulla" Text="Annulla" DisabledImage="~/Images/annullaconf_dis.gif" Image="~/Images/annullaconf.gif">
                        </igtbar:TBarButton>
                    </Items>
                </igtbar:UltraWebToolbar>
                <asp:Panel ID="cpag" runat="server">Panel</asp:Panel>
            </on_ofm:OnitFinestraModale>

            <on_ofm:OnitFinestraModale ID="modNoteVac" Title="Note Vaccinazione" runat="server" Width="470px" BackColor="LightGray"
                NoRenderX="true" ZIndexPosition="1" IsAnagrafe="False">
                <table cellspacing="0" cellpadding="2" width="100%" border="0">
                    <tr style="height: 100px">
                        <td>
                            <asp:TextBox ID="txtNoteVac" CssClass="vac-note" runat="server" MaxLength="200" TextMode="MultiLine" Rows="5"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Button ID="btnNoteVacOk" Width="75px" runat="server" Text="Ok" OnClick="btnNoteVacOk_Click" Style="cursor: pointer;"></asp:Button>
                            <asp:Button ID="btnNoteVacAnnulla" Width="75px" runat="server" Text="Annulla" OnClick="btnNoteVacAnnulla_Click" Style="cursor: pointer;"></asp:Button>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <div class="warningModaleEdit" style="width: 450px;">
                                ATTENZIONE: le note inserite verranno ripetute per tutte le vaccinazioni relative al nome commerciale. Le note precedenti saranno sovrascritte.
                            </div>
                        </td>
                    </tr>
                </table>
            </on_ofm:OnitFinestraModale>

            <on_ofm:OnitFinestraModale ID="modPagVac" Title="Pagamento Vaccinazione" runat="server" Width="470px" BackColor="LightGray"
                NoRenderX="true" ZIndexPosition="1" IsAnagrafe="False">
                <table cellspacing="0" cellpadding="2" width="100%" border="0" style="margin-top: 5px">
                    <colgroup>
                        <col width="15%" />
                        <col width="85%" />
                    </colgroup>
                    <tr>
                        <td class="label_right">Tipologia</td>
                        <td class="label_left">
                            <asp:DropDownList ID="ddlTipiPagVac" CssClass="Textbox_Stringa" runat="server" Width="100%" AutoPostBack="true"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="label_right">Esenzione</td>
                        <td class="label_left">
                            <asp:DropDownList ID="ddlEseMalPagVac" onchange="clearEseMalPag()" Width="100%" runat="server"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="label_right">Importo&nbsp;€</td>
                        <td class="label_left">
                            <on_val:OnitJsValidator ID="valImpPagVac" runat="server" Width="100%" actionCorrect="False" actionCustom="" actionDelete="False"
                                actionFocus="True" actionMessage="True" actionSelect="True" actionUndo="True" autoFormat="False" CustomValFunction="validaNumero"
                                validationType="Validate_custom" MaxLength="10">
                            <Parameters>
                                <on_val:ValidationParam paramName="numCifreIntere" paramOrder="0" 
                                    paramType="number" paramValue="5" />
                                <on_val:ValidationParam paramName="numCifreDecimali" paramOrder="1" 
                                    paramType="number" paramValue="2" />
                                <on_val:ValidationParam paramName="minValore" paramOrder="2" paramType="number" 
                                    paramValue="0" />
                                <on_val:ValidationParam paramName="maxValore" paramOrder="3" paramType="number" 
                                    paramValue="99999.99" />
                                <on_val:ValidationParam paramName="blnCommaSeparator" paramOrder="4" 
                                    paramType="boolean" paramValue="true" />
                            </Parameters>
                            </on_val:OnitJsValidator>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="2">
                            <asp:Button ID="btnPagVacOk" Width="75px" runat="server" Text="Ok" Style="cursor: auto;" OnClick="btnPagVacOk_Click"></asp:Button>
                            <asp:Button ID="btnPagVacAnnulla" Width="75px" runat="server" Text="Annulla" Style="cursor: auto;" OnClick="btnPagVacAnnulla_Click"></asp:Button>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: center" colspan="2">
                            <div class="warningModaleEdit" style="width: 450px;">
                                ATTENZIONE: premendo Ok, i dati del pagamento inserito verranno ripetuti per tutte le vaccinazioni relative al nome commerciale. I dati precedenti saranno sovrascritti.
                            </div>
                        </td>
                    </tr>
                </table>
            </on_ofm:OnitFinestraModale>

            <on_ofm:OnitFinestraModale ID="modInfoVacc" Title="&nbsp;&lt;img src='../../images/info.png'&gt;&nbsp;Info" runat="server" Width="550px" BackColor="#fbf9f9"
                Height="200px" NoRenderX="false" ZIndexPosition="1" IsAnagrafe="False">
                <table cellpadding="2" cellspacing="2" border="0" width="100%" style="margin-top: 10px;">
                    <colgroup>
                        <col width="16%" style="font-weight: bold; padding: 3px" />
                        <col width="57%" style="padding: 3px" />
                        <col width="16%" style="font-weight: bold; padding: 3px" />
                        <col width="9%" style="padding: 3px" />
                        <col width="2%" />
                    </colgroup>
                    <tr>
                        <td class="label">
                            <asp:Label ID="lblInfoCaptionVacc" runat="server" Text="Vaccinazione"></asp:Label></td>
                        <td class="label_left">
                            <div class="infoDati">
                                <asp:Label ID="lblInfoVacc" runat="server"></asp:Label>
                            </div>
                        </td>
                        <td class="label">
                            <asp:Label ID="lblInfoCaptionDoseVacc" runat="server" Text="Dose vacc."></asp:Label></td>
                        <td class="label_left">
                            <div class="infoDati">
                                <asp:Label ID="lblInfoDoseVacc" runat="server"></asp:Label>
                            </div>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Label ID="lblInfoCaptionAss" runat="server" Text="Associazione"></asp:Label></td>
                        <td class="label_left">
                            <div class="infoDati">
                                <asp:Label ID="lblInfoAss" runat="server"></asp:Label>
                            </div>
                        </td>
                        <td class="label">
                            <asp:Label ID="lblInfoCaptionDoseAss" runat="server" Text="Dose assoc."></asp:Label></td>
                        <td class="label_left">
                            <div class="infoDati">
                                <asp:Label ID="lblInfoDoseAss" runat="server"></asp:Label>
                            </div>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Label ID="lblInfoCaptionCiclo" runat="server" Text="Ciclo"></asp:Label></td>
                        <td class="label_left">
                            <div class="infoDati">
                                <asp:Label ID="lblInfoCiclo" runat="server"></asp:Label>
                            </div>
                        </td>
                        <td class="label">
                            <asp:Label ID="lblInfoCaptionSeduta" runat="server" Text="Seduta"></asp:Label></td>
                        <td class="label_left">
                            <div class="infoDati">
                                <asp:Label ID="lblInfoSeduta" runat="server"></asp:Label>
                            </div>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Label ID="lblInfoCaptionVaccinatore" runat="server" Text="Vaccinatore"></asp:Label></td>
                        <td class="label_left">
                            <div class="infoDati">
                                <asp:Label ID="lblInfoVaccinatore" runat="server"></asp:Label>&nbsp;
                            </div>
                        </td>
                        <td colspan="3"></td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Label ID="lblInfoCaptionResponsabile" runat="server" Text="Responsabile"></asp:Label></td>
                        <td class="label_left">
                            <div class="infoDati">
                                <asp:Label ID="lblInfoResponsabile" runat="server"></asp:Label>
                            </div>
                        </td>
                        <td colspan="3"></td>
                    </tr>
                </table>
            </on_ofm:OnitFinestraModale>

            <asp:TextBox ID="AssociazioniScelteLatoClient" Style="display: none" runat="server"></asp:TextBox>

            <asp:HiddenField ID="hidRowIndex" runat="server" />

        </on_lay3:OnitLayout3>

        <!-- #include file="../../Common/Scripts/AggiornaStatoVaccinazioni.inc" -->

    </form>

    <script type="text/javascript" language="javascript">

        var dataEsecuzionePrecedente = OnitDataPickGet('<%= Me.odpDataEs.ClientID %>');

        busy = '<%= Me.onitlayout31.busy %>';
        statusVaccinale = '<%= Me.StatusVaccinale %>';
        paz_deceduto = '<%= Me.PazienteDeceduto %>';

        doseVacProg = '<%= Me.doseVacProg %>';

        //associa il click al pulsante della modale di errore [modifica 08/07/2005]
        if (document.getElementById('btnChiudiMessaggioErrore') != null)
            document.getElementById('btnChiudiMessaggioErrore').onclick = ChiudiMessaggioErrore;

        function controllaConDoseStessa(obj, evt) {
            if (doseVacProg.toString() != document.getElementById('<%= Me.ClientIDTxtNumRich %>').value) {
                controllaPerDoseStessa(evt, true);
            }
            else {
                controllaPerDoseStessa(evt, false);
            }
        }

        function checkRadioButtonSelected() {
            
            // Se la modalità di accesso è nascosta, non devo farla selezionare
            if ('<%= Settings.GESMODALITAACCESSO %>' == 'False') return true;

            if ((document.getElementById('<%= rb_cup.ClientID%>').checked) == true)
                return true;
            if ((document.getElementById('<%= rb_ps.ClientID %>').checked) == true)
                return true;
            if ((document.getElementById('<%= rb_vol.ClientID %>').checked) == true)
                return true;
            if ((document.getElementById('<%= rb_app.ClientID %>').checked) == true)
                return true;

            return false;
        }

        // Focus sul campo della modale Note, quando presente 
        var txt = document.getElementById('txtNoteVac');
        if (txt != null) txt.focus();

        // Focus sul campo importo della modale Pagamento, quando presente
        txt = document.getElementById('valImpPagVac');
        if (txt != null) txt.focus();

        function OnitDataPick_Blur(id, e) {
            DataPickBlur(id);
        }

        function OnitDataPick_ClickDay(id) {
            DataPickBlur(id);
        }

        function DataPickBlur(id) {
            if (id == '<%= Me.odpDataEs.ClientID %>') {

                // Data inserita dall'utente nel picker, che restituisce sempre una stringa in formato dd/MM/yyyy
                var dataEsecuzione = OnitDataPickGet(id);

                if (dataEsecuzione != "" && dataEsecuzionePrecedente != "" && dataEsecuzione != dataEsecuzionePrecedente) {

                    var dataEsecuzioneSplitted = dataEsecuzione.split("/");

                    var today = new Date();
                    var currentDay = GetFormattedValue(today.getDate());
                    var currentMonth = GetFormattedValue(today.getMonth() + 1);
                    var currentYear = today.getFullYear();

                    if (currentDay == dataEsecuzioneSplitted[0] && currentMonth == dataEsecuzioneSplitted[1] && currentYear == dataEsecuzioneSplitted[2]) {
                        var currentHour = GetFormattedValue(today.getHours());
                        var currentMin = GetFormattedValue(today.getMinutes());
                        document.getElementById('<%= Me.txtOraEs_MedLogin.ClientID %>').value = currentHour + ":" + currentMin;
                    }
                    else {
                        document.getElementById('<%= Me.txtOraEs_MedLogin.ClientID %>').value = "00:00";
                    }
                }
            }
        }

        function GetFormattedValue(value) {
            if (value.toString().length == 1) value = "0" + value;
            return value;
        }
    </script>

</body>
</html>
