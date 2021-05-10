<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CancellazioneConvocazioni.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.CancellazioneConvocazioni" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Assembly="Onit.Web.UI.WebControls.DynamicPanel" Namespace="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="uc1" TagName="UscFiltroPrenotazioneSelezioneMultipla" Src="../../Appuntamenti/Gestione Appuntamenti/UscFiltroPrenotazioneSelezioneMultipla.ascx" %>
<%@ Register TagPrefix="uc2" TagName="StatiAnagrafici" Src="../../Common/Controls/StatiAnagrafici.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Utility Cancellazione Programmazione</title>
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <style type="text/css">
        .infratoolbar_button_default,
        .infratoolbar_button_hover,
        .infratoolbar_button_selected {
            padding-left: 10px;
            padding-right: 10px;
        }

        .fldnode {
            padding-bottom: 5px;
        }

        .filtroCnv {
            width: 100%;
            border: 0;
            table-layout: fixed;
        }
    </style>

    <script type="text/javascript">

        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;

            switch (button.Key) {
                case 'btnCerca':
						
                    if (<%= (String.IsNullOrWhiteSpace(omlConsultorio.Codice)).ToString().ToLower()%> == 'true') 
                    {
                        alert("Selezionare il Centro Vaccinale per proseguire con la ricerca.");
                        evnt.needPostBack=false;
                    }
                    break;
            }
        }

        function mouse(obj, tipo) {
            if (obj.src.indexOf('_dis') == -1) {
                var idx_file = obj.src.lastIndexOf('/') + 1;
                var new_path = obj.src.substr(0, idx_file);
                var new_file = obj.src.substr(idx_file);
                var idx_ext = new_file.indexOf('.');
                var new_ext = new_file.substr(idx_ext);
                new_file = new_file.substr(0, idx_ext);

                if (tipo == 'over') {
                    obj.src = new_path + new_file + '_hov' + new_ext;
                } else {
                    obj.src = new_path + new_file.substr(0, new_file.indexOf('_hov')) + new_ext;
                }
            }
            return true;
        }

        function SelezionaTutti(chkValue) {
            __doPostBack('selectAll', chkValue);
        }

        function ImpostaImmagineOrdinamento(imgId, imgUrl) {
            var img = document.getElementById(imgId);
            if (img != null) {
                img.style.display = 'inline';
                img.src = imgUrl;
            }
        }

        function AbilitaFiltriConvocazioni() {
            //Recupero i radio button
            var rbCicliSedute = document.getElementById('<%= rbCicliSedute.ClientID%>');
            var rbAssociazioniDosi = document.getElementById('<%= rbAssociazioniDosi.ClientID%>');
            var rbVaccinazioniDosi = document.getElementById('<%= rbVaccinazioniDosi.ClientID%>');

            //Recupero i filtri convocazioni
            var btnImgCicliSedute = document.getElementById('<%= btnImgCicliSedute.ClientID%>');
            var btnImgAssociazioniDosi = document.getElementById('<%= btnImgAssociazioniDosi.ClientID%>');
            var btnImgVaccinazioniDosi = document.getElementById('<%= btnImgVaccinazioniDosi.ClientID%>');
            var lblCicliSedute = document.getElementById('<%= lblCicliSedute.ClientID%>');
            var lblAssociazioniDosi = document.getElementById('<%= lblAssociazioniDosi.ClientID%>');
            var lblVaccinazioniDosi = document.getElementById('<%= lblVaccinazioniDosi.ClientID%>');

            //Impostazione abilitazioni
            if (rbCicliSedute.checked) {
                btnImgCicliSedute.disabled = false;
                btnImgAssociazioniDosi.disabled = true;
                btnImgVaccinazioniDosi.disabled = true;

                btnImgCicliSedute.style.cursor = "pointer"
                btnImgAssociazioniDosi.style.cursor = "default"
                btnImgVaccinazioniDosi.style.cursor = "default"

                btnImgCicliSedute.src = "../../images/filtro_cicli.gif"
                btnImgAssociazioniDosi.src = "../../images/filtro_associazioni_dis.gif"
                btnImgVaccinazioniDosi.src = "../../images/filtro_vaccinazioni_dis.gif"
                
                //Ripulisco le label
                lblAssociazioniDosi.innerText  = ''
                lblVaccinazioniDosi.innerText  = ''
            }
            else if (rbAssociazioniDosi.checked) {
                btnImgCicliSedute.disabled = true;
                btnImgAssociazioniDosi.disabled = false;
                btnImgVaccinazioniDosi.disabled = true;

                btnImgCicliSedute.style.cursor = "default"
                btnImgAssociazioniDosi.style.cursor = "pointer"
                btnImgVaccinazioniDosi.style.cursor = "default"

                btnImgCicliSedute.src = "../../images/filtro_cicli_dis.gif"
                btnImgAssociazioniDosi.src = "../../images/filtro_associazioni.gif"
                btnImgVaccinazioniDosi.src = "../../images/filtro_vaccinazioni_dis.gif"

                //Ripulisco le label
                lblCicliSedute.innerText  = ''
                lblVaccinazioniDosi.innerText  = ''
            }
            else if (rbVaccinazioniDosi.checked) {
                btnImgCicliSedute.disabled = true;
                btnImgAssociazioniDosi.disabled = true;
                btnImgVaccinazioniDosi.disabled = false;

                btnImgCicliSedute.style.cursor = "default"
                btnImgAssociazioniDosi.style.cursor = "default"
                btnImgVaccinazioniDosi.style.cursor = "pointer"

                btnImgCicliSedute.src = "../../images/filtro_cicli_dis.gif"
                btnImgAssociazioniDosi.src = "../../images/filtro_associazioni_dis.gif"
                btnImgVaccinazioniDosi.src = "../../images/filtro_vaccinazioni.gif"
                
                //Ripulisco le label
                lblCicliSedute.innerText  = ''
                lblAssociazioniDosi.innerText  = ''
            }
            else {
                btnImgCicliSedute.disabled = true;
                btnImgAssociazioniDosi.disabled = true;
                btnImgVaccinazioniDosi.disabled = true;

                btnImgCicliSedute.style.cursor = "default"
                btnImgAssociazioniDosi.style.cursor = "default"
                btnImgVaccinazioniDosi.style.cursor = "default"

                btnImgCicliSedute.src = "../../images/filtro_cicli_dis.gif"
                btnImgAssociazioniDosi.src = "../../images/filtro_associazioni_dis.gif"
                btnImgVaccinazioniDosi.src = "../../images/filtro_vaccinazioni_dis.gif"

                //Ripulisco le label
                lblCicliSedute.innerText  = ''
                lblAssociazioniDosi.innerText  = ''
                lblVaccinazioniDosi.innerText  = ''
            }
        }

    </script>
</head>

<body onload="AbilitaFiltriConvocazioni()">
    <form id="form1" runat="server">
        <asp:HiddenField ID="hidMaxCnvDaElab" runat="server" Value="1000" />
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" TitleCssClass="Title3" Titolo="Utility Cancellazione Programmazione Vaccinale">
            <div>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBar" runat="server" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                    <Items>
                        <igtbar:TBarButton Key="btnCerca" Text="Cerca" DisabledImage="~/Images/cerca_dis.gif" Image="~/Images/cerca.gif" />
                        <igtbar:TBSeparator />
                        <igtbar:TBarButton Key="btnElimina" Text="Elimina Programmazione" DisabledImage="~/Images/genera_dis.gif" Image="~/Images/genera.gif" />
                        <igtbar:TBSeparator />
                        <%--<igtbar:TBarButton Key="btnStampa" Text="Stampa" DisabledImage="~/Images/stampa_dis.gif" Image="~/Images/stampa.gif" />
                        <igtbar:TBSeparator />--%>
                        <igtbar:TBarButton Key="btnPulisci" Text="Pulisci" ToolTip="Resetta i filtri impostati ai valori di default"
                            DisabledImage="../../images/eraser.png" Image="../../images/eraser.png">
                        </igtbar:TBarButton>
                    </Items>
                </igtbar:UltraWebToolbar>
            </div>
            <div>
                <table width="100%" bgcolor="whitesmoke">
                    <tr height="2">
                        <td></td>
                    </tr>
                    <tr>
                        <td>
                            <fieldset class="vacFieldset" title="Filtri Pazienti">
                                <legend class="label">Filtri Pazienti</legend>
                                <table class="label_left" style="margin-top: 2px" cellspacing="0" cellpadding="2" width="100%" border="0">
                                    <colgroup>
                                        <col width="2%" />
                                        <col width="12%" />
                                        <col width="2%" />
                                        <col width="15%" />
                                        <col width="2%" />
                                        <col width="15%" />
                                        <col width="10%" />
                                        <col width="37%" />
                                        <col width="2%" />
                                    </colgroup>
                                    <tr height="5">
                                        <td colspan="9"></td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td class="label_left" style="padding-left:5px">Data nascita:</td>
                                        <td class="label" align="right">Da</td>
                                        <td>
                                            <on_val:OnitDatePick ID="dpkDataNascitaDa" runat="server" Height="20px" Width="120px" CssClass="textbox_data"
                                                DateBox="True"></on_val:OnitDatePick>
                                        </td>
                                        <td class="label" align="right">A</td>
                                        <td>
                                            <on_val:OnitDatePick ID="dpkDataNascitaA" runat="server" Height="20px" Width="120px" CssClass="textbox_data"
                                                DateBox="True"></on_val:OnitDatePick>
                                        </td>
                                        <td class="label">Sesso:</td>
                                        <td>
                                            <asp:DropDownList ID="ddlSesso" Width="99%" runat="server">
                                                <asp:ListItem Selected="True"></asp:ListItem>
                                                <asp:ListItem Value="M">Maschio</asp:ListItem>
                                                <asp:ListItem Value="F">Femmina</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td class="label_left" style="padding-left:5px">Categoria rischio:</td>
                                        <td colspan="4">
                                            <onitcontrols:OnitModalList ID="omlCategorieRischio" runat="server" Width="70%" CampoDescrizione="RSC_DESCRIZIONE"
                                                CampoCodice="RSC_CODICE" Tabella="T_ANA_RISCHIO" PosizionamentoFacile="False" LabelWidth="-1px" CodiceWidth="29%"
                                                Label="Titolo" SetUpperCase="True"></onitcontrols:OnitModalList>
                                        </td>
                                        <td class="label">Malattia:</td>
                                        <td>
                                            <onitcontrols:OnitModalList ID="omlMalattia" runat="server" Width="70%" CampoDescrizione="MAL_DESCRIZIONE" CampoCodice="MAL_CODICE"
                                                Tabella="T_ANA_MALATTIE" PosizionamentoFacile="False" LabelWidth="-1px" CodiceWidth="29%" Label="Titolo"
                                                SetUpperCase="True"></onitcontrols:OnitModalList>
                                        </td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td colspan="7">
                                            <uc2:StatiAnagrafici ID="staStatiAnagrafici" runat="server" />
                                        </td>
                                        <td></td>
                                    </tr>
                                    <tr height="10">
                                        <td colspan="9"></td>
                                    </tr>
                                </table>
                            </fieldset>
                        </td>
                    </tr>
                    <tr height="2">
                        <td></td>
                    </tr>
                    <tr>
                        <td>
                            <fieldset class="vacFieldset" id="fldFiltriCnv" title="Filtri Convocazioni">
                                <legend class="label">Filtri Convocazioni</legend>
                                <table width="100%">
                                    <colgroup>
                                        <col width="2%" />
                                        <col width="38%" />
                                        <col width="56%" />
                                        <col width="2%" />
                                    </colgroup>
                                    <tr>
                                        <td></td>
                                        <td>
                                            <table id="TableFiltriDataCnv" width="100%" border="0">
                                                <tr>
                                                    <td></td>
                                                    <td class="label_left">CV:</td>
                                                    <td>
                                                        <onitcontrols:OnitModalList ID="omlConsultorio" runat="server" Filtro="cns_data_apertura <= SYSDATE AND (cns_data_chiusura > SYSDATE OR cns_data_chiusura IS NULL) ORDER BY cns_descrizione"
                                                            Width="70%" CampoDescrizione="CNS_DESCRIZIONE" CampoCodice="CNS_CODICE" Tabella="T_ANA_CONSULTORI" PosizionamentoFacile="False"
                                                            LabelWidth="-1px" CodiceWidth="29%" Label="Titolo" UseCode="True" SetUpperCase="True" Obbligatorio="True"></onitcontrols:OnitModalList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="1%"></td>
                                                    <td colspan="2">
                                                        <fieldset id="fldCnv" title="Data Convocazione" class="fldnode">
                                                            <legend class="label">Data Convocazione</legend>
                                                            <table id="Table_nascita" width="100%" border="0">
                                                                <colgroup>
                                                                    <col width="30%" />
                                                                    <col width="70%" />
                                                                </colgroup>
                                                                <tr>
                                                                    <td class="label">Da :</td>
                                                                    <td align="left">
                                                                        <on_val:OnitDatePick ID="dpkDataCnvDa" runat="server" Height="20px" Width="120px" CssClass="textbox_data" DateBox="True" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="label">A :</td>
                                                                    <td align="left">
                                                                        <on_val:OnitDatePick ID="dpkDataCnvA" runat="server" Height="20px" Width="120px" CssClass="textbox_data" DateBox="True" />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </fieldset>
                                                    </td>
                                                    <td width="1%"></td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td>
                                            <table cellspacing="0" cellpadding="0">
                                                <colgroup>
                                                    <col width="2%" />
                                                    <col width="3%" />
                                                    <col width="25%" />
                                                    <col width="70%" />
                                                </colgroup>
                                                <tr>
                                                    <td></td>
                                                    <td>
                                                        <asp:RadioButton ID="rbCicliSedute" CssClass="label" Font-Bold="True" runat="server" GroupName="filtro" onclick="AbilitaFiltriConvocazioni()" />
                                                    </td>
                                                    <td class="label_left">Cicli-Sedute:</td>
                                                    <td>
                                                        <table class="filtroCnv">
                                                            <tr>
                                                                <td width="26px" align="right">
                                                                    <asp:ImageButton ID="btnImgCicliSedute" runat="server" onmouseover="mouse(this,'over');"
                                                                        title="Impostazione filtro cicli-sedute" onmouseout="mouse(this,'out');"
                                                                        ImageUrl="../../images/filtro_cicli_dis.gif" Enabled="false" />
                                                                </td>
                                                                <td style="height: 22px; border: navy 1px solid; padding-left: 5px; background: gainsboro">
                                                                    <asp:Label ID="lblCicliSedute" Style="height: 18px; font-size: 10px; text-transform: uppercase; font-style: italic; font-family: verdana" runat="server" Text=""></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td>
                                                        <asp:RadioButton ID="rbAssociazioniDosi" CssClass="label" Font-Bold="True" runat="server" GroupName="filtro" onclick="AbilitaFiltriConvocazioni()" />
                                                    </td>
                                                    <td class="label_left">Associazioni-Dosi:</td>
                                                    <td>
                                                        <table class="filtroCnv">
                                                            <tr>
                                                                <td width="26px" align="right">
                                                                    <asp:ImageButton ID="btnImgAssociazioniDosi" runat="server" onmouseover="mouse(this,'over');"
                                                                        title="Impostazione filtro associazioni-dosi" onmouseout="mouse(this,'out');"
                                                                        ImageUrl="../../images/filtro_associazioni_dis.gif" Enabled="false" />
                                                                </td>
                                                                <td style="height: 22px; border: navy 1px solid; padding-left: 5px; background: gainsboro">
                                                                    <asp:Label ID="lblAssociazioniDosi" Style="height: 18px; font-size: 10px; text-transform: uppercase; font-style: italic; font-family: verdana" runat="server" Text=""></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td>
                                                        <asp:RadioButton ID="rbVaccinazioniDosi" CssClass="label" Font-Bold="True" runat="server" GroupName="filtro" onclick="AbilitaFiltriConvocazioni()" />
                                                    </td>
                                                    <td class="label_left">Vaccinazioni-Dosi:</td>
                                                    <td>
                                                        <table class="filtroCnv">
                                                            <tr>
                                                                <td width="26px" align="right">
                                                                    <asp:ImageButton ID="btnImgVaccinazioniDosi" runat="server" onmouseover="mouse(this,'over');"
                                                                        title="Impostazione filtro vaccinazioni-dosi" onmouseout="mouse(this,'out');"
                                                                        ImageUrl="../../images/filtro_vaccinazioni_dis.gif" Enabled="false" />
                                                                </td>
                                                                <td style="height: 22px; border: navy 1px solid; padding-left: 5px; background: gainsboro">
                                                                    <asp:Label ID="lblVaccinazioniDosi" Style="height: 18px; font-size: 10px; text-transform: uppercase; font-style: italic; font-family: verdana" runat="server" Text=""></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td>
                                                        <asp:RadioButton ID="rbNessunFiltro" CssClass="label" Font-Bold="True" runat="server" Checked="true" GroupName="filtro" onclick="AbilitaFiltriConvocazioni()" />
                                                    </td>
                                                    <td class="label_left">Nessun filtro.</td>
                                                    <td></td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td></td>
                                    </tr>
                                </table>
                            </fieldset>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <fieldset class="vacFieldset" id="fldOpDaEffettuare" title="Operazioni da Effettuare">
                                <legend class="label">Operazioni da Effettuare</legend>
                                <table style="margin-top: 2px" cellspacing="0" cellpadding="2" width="100%" border="0">
                                    <colgroup>
                                        <col width="2%" />
                                        <col width="38%" />
                                        <col width="34%" />
                                        <col width="24%" />
                                        <col width="2%" />
                                    </colgroup>
                                    <tr>
                                        <td></td>
                                        <td align="left">
                                            <asp:CheckBox ID="chkCnvConAppuntamento" runat="server" Height="23px" CssClass="TextBox_Stringa" Text="Cancella anche le convocazioni con appuntamento"
                                                TextAlign="Right"></asp:CheckBox>
                                        </td>
                                        <td align="left">
                                            <asp:CheckBox ID="chkCnvConSollecito" runat="server" Height="23px" CssClass="TextBox_Stringa" Text="Cancella anche le convocazioni con sollecito"
                                                TextAlign="Right"></asp:CheckBox>
                                        </td>
                                        <td align="left">
                                            <asp:CheckBox ID="chkCancellaInteraCnv" runat="server" Height="23px" CssClass="TextBox_Stringa" Text="Cancella intera convocazione"
                                                TextAlign="Right"></asp:CheckBox>
                                        </td>
                                        <td></td>
                                    </tr>
                                </table>
                            </fieldset>
                        </td>
                    </tr>
                    <tr height="2">
                        <td></td>
                    </tr>
                </table>
                <div class="vac-sezione" style="margin: 2px">
                    <asp:Label ID="lblCnvTrovate" runat="server">Convocazioni trovate</asp:Label>
                </div>
            </div>
            <dyp:DynamicPanel ID="dypConvocazioni" runat="server" Width="100%" Height="100%" ScrollBars="Auto">
                <asp:DataGrid ID="dgrConvocazioni" runat="server" Width="100%" AutoGenerateColumns="False"
                    AllowCustomPaging="true" AllowPaging="true" PageSize="25" AllowSorting="True">
                    <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
                    <ItemStyle CssClass="item"></ItemStyle>
                    <PagerStyle CssClass="pager" Position="TopAndBottom" Mode="NumericPages" />
                    <HeaderStyle CssClass="header"></HeaderStyle>
                    <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
                    <Columns>
                        <asp:TemplateColumn>
                            <HeaderStyle Width="1%" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <HeaderTemplate>
                                <input type="checkbox" id="chkSelezioneHeader" onclick="SelezionaTutti(this.checked);" title="Seleziona tutti" runat="server" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSelezioneItem" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderStyle-Width="0%">
                            <ItemTemplate>
                                <asp:HiddenField ID="hidIdPaziente" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "IdPaziente")%>' />
                                <asp:HiddenField ID="hidData" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "Data")%>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="PazienteCognome" HeaderText="Cognome <img id='imgCog' alt='' src='../../images/transparent16.gif' />"
                            SortExpression="PazienteCognome">
                            <HeaderStyle Width="15%" HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="PazienteNome" HeaderText="Nome <img id='imgNom' alt='' src='../../images/transparent16.gif' />"
                            SortExpression="PazienteNome">
                            <HeaderStyle Width="15%" HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundColumn>
                        <asp:TemplateColumn HeaderText="Data Nascita <img id='imgNas' alt='' src='../../images/transparent16.gif' />"
                            HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="12%" SortExpression="PazienteDataNascita" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <asp:Label ID="lblDataNascita" CssClass="label_left" runat="server"
                                    Text='<%# CDate(DataBinder.Eval(Container.DataItem, "PazienteDataNascita")).Date.ToString("dd/MM/yyyy")%>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Convocazione <img id='imgCnv' alt='' src='../../images/transparent16.gif' />"
                            HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="13%" SortExpression="Data" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <asp:Label ID="lblDataCnv" CssClass="label_left" runat="server"
                                    Text='<%# CDate(DataBinder.Eval(Container.DataItem, "Data")).Date.ToString("dd/MM/yyyy")%>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="Vaccinazioni" HeaderText="Vaccinazioni <img id='imgVac' alt='' src='../../images/transparent16.gif' />"
                            SortExpression="Vaccinazioni">
                            <HeaderStyle Width="31%" HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundColumn>
                        <asp:TemplateColumn HeaderText="Data App. <img id='imgDApp' alt='' src='../../images/transparent16.gif' />"
                            HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="13%" SortExpression="DataAppuntamento" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <asp:Label ID="lblDataApp" CssClass="label_left" runat="server"
                                    Text='<%# ConvertToDateString(DataBinder.Eval(Container.DataItem, "DataAppuntamento"))%>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>

            </dyp:DynamicPanel>

        </on_lay3:OnitLayout3>

        <!-- Modale filtro associazioni dosi -->
        <on_ofm:OnitFinestraModale ID="fmFiltroAssociazioniDosi" Title="<div style=&quot;font-family:'Microsoft Sans Serif';text-align:center; font-size: 11pt;padding-bottom:2px&quot;>Seleziona le associazioni e le dosi per cui filtrare</div>"
            runat="server" Width="532px" BackColor="LightGray" RenderModalNotVisible="True" UseDefaultTab="True">
            <table border="0" cellspacing="0" cellpadding="0" width="100%" style="background-color: LightGrey; width: 532px;">
                <colgroup>
                    <col width="1%" />
                    <col width="45%" />
                    <col width="8%" />
                    <col width="45%" />
                    <col width="1%" />
                </colgroup>
                <tr>
                    <td></td>
                    <td colspan="3">
                        <uc1:UscFiltroPrenotazioneSelezioneMultipla ID="UscFiltroAssociazioniDosi" runat="server" Tipo="0"></uc1:UscFiltroPrenotazioneSelezioneMultipla>
                    </td>
                    <td></td>
                </tr>
                <tr height="10">
                    <td colspan="5"></td>
                </tr>
                <tr>
                    <td></td>
                    <td align="right">
                        <asp:Button Style="cursor: hand" ID="btnOk_FiltroAssociazioniDosi" runat="server" Width="100px" Text="OK"></asp:Button>
                    </td>
                    <td></td>
                    <td>
                        <asp:Button Style="cursor: hand" ID="btnAnnulla_FiltroAssociazioniDosi" runat="server" Width="100px" Text="Annulla"></asp:Button>
                    </td>
                    <td></td>
                </tr>
                <tr height="10">
                    <td colspan="5"></td>
                </tr>
            </table>
        </on_ofm:OnitFinestraModale>


        <!-- Modale filtro cicli sedute -->
        <on_ofm:OnitFinestraModale ID="fmFiltroCicliSedute" Title="<div style=&quot;font-family:'Microsoft Sans Serif';text-align:center; font-size: 11pt;padding-bottom:2px&quot;>Seleziona i cicli e le sedute per cui filtrare</div>"
            runat="server" Width="532px" BackColor="LightGray" RenderModalNotVisible="True" UseDefaultTab="True">
            <table border="0" cellspacing="0" cellpadding="0" width="100%" style="background-color: LightGrey; width: 532px; height: 60px">
                <colgroup>
                    <col width="1%" />
                    <col width="45%" />
                    <col width="8%" />
                    <col width="45%" />
                    <col width="1%" />
                </colgroup>
                <tr>
                    <td>&nbsp;</td>
                    <td colspan="3">
                        <uc1:UscFiltroPrenotazioneSelezioneMultipla ID="UscFiltroCicliSedute" runat="server" Tipo="1"></uc1:UscFiltroPrenotazioneSelezioneMultipla>
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr height="10">
                    <td colspan="5">&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td align="right">
                        <asp:Button Style="cursor: hand" ID="btnOk_FiltroCicliSedute" runat="server" Width="100px" Text="OK"></asp:Button>
                    </td>
                    <td>&nbsp;</td>
                    <td>
                        <asp:Button Style="cursor: hand" ID="btnAnnulla_FiltroCicliSedute" runat="server" Width="100px" Text="Annulla"></asp:Button>
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr height="10">
                    <td colspan="5">&nbsp;</td>
                </tr>
            </table>
        </on_ofm:OnitFinestraModale>

        <!-- Modale filtro vaccinazioni dosi -->
        <on_ofm:OnitFinestraModale ID="fmFiltroVaccinazioniDosi" Title="<div style=&quot;font-family:'Microsoft Sans Serif';text-align:center; font-size: 11pt;padding-bottom:2px&quot;>Seleziona le vaccinazioni e le dosi per cui filtrare</div>"
            runat="server" Width="532px" BackColor="LightGray" RenderModalNotVisible="True" UseDefaultTab="True">
            <table border="0" cellspacing="0" cellpadding="0" width="100%" style="background-color: LightGrey; width: 532px;">
                <colgroup>
                    <col width="1%" />
                    <col width="45%" />
                    <col width="8%" />
                    <col width="45%" />
                    <col width="1%" />
                </colgroup>
                <tr>
                    <td></td>
                    <td colspan="3">
                        <uc1:UscFiltroPrenotazioneSelezioneMultipla ID="UscFiltroVaccinazioniDosi" runat="server" Tipo="2"></uc1:UscFiltroPrenotazioneSelezioneMultipla>
                    </td>
                    <td></td>
                </tr>
                <tr height="10">
                    <td colspan="5"></td>
                </tr>
                <tr>
                    <td></td>
                    <td align="right">
                        <asp:Button Style="cursor: hand" ID="btnOk_FiltroVaccinazioniDosi" runat="server" Width="100px" Text="OK"></asp:Button>
                    </td>
                    <td></td>
                    <td>
                        <asp:Button Style="cursor: hand" ID="btnAnnulla_FiltroVaccinazioniDosi" runat="server" Width="100px" Text="Annulla"></asp:Button>
                    </td>
                    <td></td>
                </tr>
                <tr height="10">
                    <td colspan="5"></td>
                </tr>
            </table>
        </on_ofm:OnitFinestraModale>

    </form>
</body>
</html>
