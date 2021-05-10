<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="PrenotazioneAutomatica.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.PrenotazioneAutomatica" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel"  %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>

<%@ Register TagPrefix="uc1" TagName="UscFiltroPrenotazioneSelezioneMultipla" Src="../../Appuntamenti/Gestione Appuntamenti/UscFiltroPrenotazioneSelezioneMultipla.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SelezioneConsultori" Src="../../Common/Controls/SelezioneConsultori.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Utility prenotazione automatica appuntamenti</title>
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript">

        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;

            <%--switch (button.Key) {
                case 'btnCerca':
						
                    if (<%= (String.IsNullOrWhiteSpace(omlConsultorio.Codice)).ToString().ToLower()%> == 'true') 
                    {
                        alert("Selezionare il Centro Vaccinale per proseguire con la ricerca.");
                        evnt.needPostBack=false;
                    }
                    break;
            }--%>
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
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" TitleCssClass="Title3" Titolo="Utility Cancellazione Programmazione Vaccinale">
            <div>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBar" runat="server" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                    <Items>
                        <igtbar:TBarButton Key="btnAvvia" Text="Prenotazione automatica" DisabledImage="~/Images/rotella_dis.gif" Image="~/Images/rotella.gif" />
                    </Items>
                </igtbar:UltraWebToolbar>
            </div>
            <div class="vac-sezione" style="margin: 2px;">
                <asp:Label ID="lblFiltriRicerca" runat="server">Filtri ricerca appuntamenti</asp:Label>
            </div>
            <dyp:DynamicPanel ID="dyScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" style="padding-top:3px;">
                <table width="100%" bgcolor="whitesmoke" cellpadding="2px">
                    <colgroup>
                        <col width="2%" />
                        <col width="12%" />
                        <col width="2%" />
                        <col width="15%" />
                        <col width="2%" />
                        <col width="15%" />
                        <col width="7%" />
                        <col width="11%" />
                        <col width="14%" />
                        <col width="15%" />
                        <col width="2%" />
                    </colgroup>
                    <tr>
                        <td></td>
                        <td class="label" style="padding-left: 5px">Data nascita</td>
                        <td class="label" align="right">Da</td>
                        <td>
                            <on_val:onitdatepick id="dpkDataNascitaDa" runat="server" height="20px" width="120px" cssclass="textbox_data"
                                datebox="True"></on_val:onitdatepick>
                        </td>
                        <td class="label" align="right">A</td>
                        <td>
                            <on_val:onitdatepick id="dpkDataNascitaA" runat="server" height="20px" width="120px" cssclass="textbox_data"
                                datebox="True"></on_val:onitdatepick>
                        </td>
                        <td class="label">Sesso</td>
                        <td>
                            <asp:DropDownList ID="ddlSesso" Width="99%" runat="server">
                                <asp:ListItem Value="" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="M">Maschio</asp:ListItem>
                                <asp:ListItem Value="F">Femmina</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td class="label">Ultima convocazione</td>
                        <td>
                            <on_val:onitdatepick id="dpkDataConvocazione" runat="server" height="20px" width="120px" cssclass="textbox_data_obbligatorio"
                                datebox="True"></on_val:onitdatepick></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td class="label">Centri vaccinali</td>
                        <td class="label" colspan="8">
                            <uc1:SelezioneConsultori id="ucSelezioneConsultori" runat="server" Width="100%" MostraPulsanteSelezione="true" SelezioneMultipla="true" 
                                MostraCnsUtente="false" MostraSoloCnsUslCorrente="true" MostraSoloAperti="true" ></uc1:SelezioneConsultori>
                        </td>                       
                        <td></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td class="label">Distretto</td>
                        <td class="label_left" colspan="8">
                            <on_ofm:onitmodallist id="fmDistretto" runat="server" UseCode="True" Tabella="T_ANA_DISTRETTI" CampoDescrizione="DIS_DESCRIZIONE"
								CampoCodice="DIS_CODICE" Label="Distretto" CodiceWidth="28%" LabelWidth="-8px" PosizionamentoFacile="False" OnSetUpFiletr="fmDistretto_SetUpFiletr"
								Width="70%" RaiseChangeEvent="True" SetUpperCase="True"></on_ofm:onitmodallist>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td></td>
						<td class="label">Associazioni-Dosi</td>
                        <td colspan="8">
                            <table width="100%" border="0" style="table-layout: fixed;">
	                            <tr>
		                            <td width="26px" align="right">
                                        <asp:ImageButton id="btnImgAssociazioniDosi" runat="server" onmouseover="mouse(this,'over');" title="Impostazione filtro associazioni-dosi" style="cursor: pointer" onmouseout="mouse(this,'out');" ImageUrl="~/images/filtro_associazioni.gif" />
                                    </td>
		                            <td style="height: 22px; border: navy 1px solid; padding-left: 5px; background: gainsboro" >
                                        <asp:Label id="lblAssociazioniDosi" style="height: 18px; font-size: 10px; text-transform: uppercase; font-style: italic; font-family: verdana" runat="server" Text=""></asp:Label>
                                    </td>
	                            </tr>
                            </table>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td></td>
						<td class="label">Tipo Comunicazione</td>
                        <td colspan="4">
                            <asp:RadioButtonList ID="rblTipoComunicazione" name="rblTipoComunicazione" runat="server" Width="100%" CssClass="textbox_stringa" RepeatColumns="4">
                                <asp:ListItem Value="AV">Invito</asp:ListItem>
                                <asp:ListItem Value="SL">Sollecito</asp:ListItem>
                                <asp:ListItem Value="TP">Diffida</asp:ListItem>
                                <asp:ListItem Value="" Selected="True">Tutti</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                        <td colspan="4"></td>
                    </tr>
                    <tr>
                        <td colspan="11">
                            <div class="vac-sezione">
                                <asp:Label ID="lblFiltriPrenotazione" runat="server">Filtri prenotazione appuntamenti</asp:Label>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
						<td class="label" style="padding-left: 5px">Prenotazione</td>
                        <td class="label" align="right">Da</td>
                        <td>
                            <on_val:onitdatepick id="dpkPrenotazioneDa" runat="server" height="20px" width="120px" cssclass="textbox_data_obbligatorio"
                                datebox="True"></on_val:onitdatepick>
                        </td>
                        <td class="label" align="right">A</td>
                        <td>
                            <on_val:onitdatepick id="dpkPrenotazioneA" runat="server" height="20px" width="120px" cssclass="textbox_data_obbligatorio"
                                datebox="True"></on_val:onitdatepick>
                        </td>
                        <td class="label" colspan="3">Max prenotazioni al giorno</td>
                        <td>
                            <on_val:OnitJsValidator id="txtNumPazientiGiorno" runat="server" CssClass="textbox_numerico w100p" ReadOnly="False"
								actionCorrect="False" actionDelete="False" actionFocus="True" actionMessage="True" actionSelect="False"
								actionUndo="True" autoFormat="True" validationType="Validate_custom" CustomValFunction-Length="4" CustomValFunction="validaNumero"
								SetOnChange="True" MaxLength="4"></on_val:OnitJsValidator>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td class="label">Centro di prenotazione</td>
                        <td colspan="8">
                            <on_ofm:onitmodallist id="fmConsultorioPrenotazione" runat="server" Width="69%" PosizionamentoFacile="False" LabelWidth="-8px"
								CodiceWidth="30%" CampoCodice="CNS_CODICE" CampoDescrizione="CNS_DESCRIZIONE" Tabella="T_ANA_CONSULTORI, T_ANA_DISTRETTI"
								UseCode="True" Obbligatorio="False" SetUpperCase="True" RaiseChangeEvent="True"></on_ofm:onitmodallist>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td class="label">Data schedulazione</td>
                        <td></td>
                        <td>
                            <on_val:onitdatepick id="dpkDataSchedulazione" runat="server" height="20px" width="120px" cssclass="textbox_data_obbligatorio"
                                datebox="True"></on_val:onitdatepick></td>
                        <td class="label">Ore</td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlOraSchedulazione" CssClass="textbox_numerico_obbligatorio" Width="60px">
                                <asp:ListItem Value="0" Selected="True">00</asp:ListItem>
                                <asp:ListItem Value="1">01</asp:ListItem>
                                <asp:ListItem Value="2">02</asp:ListItem>
                                <asp:ListItem Value="3">03</asp:ListItem>
                                <asp:ListItem Value="4">04</asp:ListItem>
                                <asp:ListItem Value="5">05</asp:ListItem>
                                <asp:ListItem Value="6">06</asp:ListItem>
                                <asp:ListItem Value="7">07</asp:ListItem>
                                <asp:ListItem Value="8">08</asp:ListItem>
                                <asp:ListItem Value="9">09</asp:ListItem>
                                <asp:ListItem Value="10">10</asp:ListItem>
                                <asp:ListItem Value="11">11</asp:ListItem>
                                <asp:ListItem Value="12">12</asp:ListItem>
                                <asp:ListItem Value="13">13</asp:ListItem>
                                <asp:ListItem Value="14">14</asp:ListItem>
                                <asp:ListItem Value="15">15</asp:ListItem>
                                <asp:ListItem Value="16">16</asp:ListItem>
                                <asp:ListItem Value="17">17</asp:ListItem>
                                <asp:ListItem Value="18">18</asp:ListItem>
                                <asp:ListItem Value="19">19</asp:ListItem>
                                <asp:ListItem Value="20">20</asp:ListItem>
                                <asp:ListItem Value="21">21</asp:ListItem>
                                <asp:ListItem Value="22">22</asp:ListItem>
                                <asp:ListItem Value="23">23</asp:ListItem>
                            </asp:DropDownList>
                        </td>                        
                        <td class="label">Minuti</td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlMinutiSchedulazione" CssClass="textbox_numerico_obbligatorio" Width="60px">
                                <asp:ListItem Value="0" Selected="True">00</asp:ListItem>
                                <asp:ListItem Value="5">05</asp:ListItem>
                                <asp:ListItem Value="10">10</asp:ListItem>
                                <asp:ListItem Value="15">15</asp:ListItem>
                                <asp:ListItem Value="20">20</asp:ListItem>
                                <asp:ListItem Value="25">25</asp:ListItem>
                                <asp:ListItem Value="30">30</asp:ListItem>
                                <asp:ListItem Value="35">35</asp:ListItem>
                                <asp:ListItem Value="40">40</asp:ListItem>
                                <asp:ListItem Value="45">45</asp:ListItem>
                                <asp:ListItem Value="50">50</asp:ListItem>
                                <asp:ListItem Value="55">55</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td></td>
                    </tr>
                </table>
            </dyp:DynamicPanel>
        </on_lay3:OnitLayout3>

        <on_ofm:OnitFinestraModale ID="fmFiltroAssociazioniDosi" Title="<div style=&quot;font-family:'Microsoft Sans Serif';text-align:center; font-size: 11pt;padding-bottom:2px&quot;>Seleziona le associazioni e le dosi per cui filtrare</div>"
            runat="server" Width="532px" BackColor="LightGray" RenderModalNotVisible="True" UseDefaultTab="True" NoRenderX="true">
            <table border="0" cellspacing="0" cellpadding="0" width="100%" style="background-color: LightGrey; width: 532px; ">
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
                        <uc1:UscFiltroPrenotazioneSelezioneMultipla ID="UscFiltroAssociazioniDosi" runat="server" Tipo="Associazioni_Dosi" 
                            EscludiObsoleti="true" TipoVisualizzazione="2"></uc1:UscFiltroPrenotazioneSelezioneMultipla>
                    </td>
                    <td></td>
                </tr>
                <tr height="10">
                    <td colspan="5"></td>
                </tr>
                <tr>
                    <td></td>
                    <td align="right">
                        <asp:Button Style="cursor: pointer" ID="btnOk_FiltroAssociazioniDosi" runat="server"
                            Width="100px" Text="OK"></asp:Button>
                    </td>
                    <td></td>
                    <td>
                        <asp:Button Style="cursor: pointer" ID="btnAnnulla_FiltroAssociazioniDosi" runat="server"
                            Width="100px" Text="Annulla"></asp:Button>
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
