<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ExportPostel.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.ExportPostel" %>

<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<%@ Register TagPrefix="uc2" TagName="SelezioneAmbulatorio" Src="../../Common/Controls/SelezioneAmbulatorio.ascx" %>
<%@ Register TagPrefix="uc1" TagName="UscFiltroPrenotazioneSelezioneMultipla" Src="../../Appuntamenti/Gestione Appuntamenti/UscFiltroPrenotazioneSelezioneMultipla.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Export Postel</title>
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <style type="text/css">
        .elemento_sinistro {
            width: 60px; 
            padding-left: 10px; 
            padding-top: 5px; 
        }

        .elemento_destro,
        .elemento_destro_bold {
            padding-top: 5px;
            text-align: left;
        }

        .elemento_destro_bold {
            font-weight: bold;
        }

        .fieldset_height_70 {
            height: 70px;
        }

        .fieldset_height_90 {
            height: 90px;
        }
    </style>

    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
    <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("onvac.common.js")%>'></script>
    <script type="text/javascript">

        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;

            switch (button.Key) {
                case 'btnExport':
                    if (!CheckDatiObbligatori()) {
                        alert("Non tutti i campi obbligatori sono impostati. Impossibile esportare il tracciato!");
                        evnt.needPostBack = false;
                    }
                    break;
            }
        }

        // Restituisce true se le date di inizio e fine periodo sono entrambe valorizzate.
        function CheckDatiObbligatori() {
            return (OnitDataPickGet('dpkDataAppInizio') != "" && OnitDataPickGet('dpkDataAppFine') != "");
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Height="100%" Width="100%" Titolo="Export Tracciato Postel">
		
        <div class="title" id="PanelTitolo" runat="server">
            <asp:Label ID="LayoutTitolo" runat="server">&nbsp;Export Postel</asp:Label>
        </div>
        <div>
            <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="Toolbar" runat="server" ItemWidthDefault="150px" CssClass="infratoolbar">
                <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                <Items>
                    <igtbar:TBarButton Key="btnExport" Text="Genera File Postel" Image="~/images/elaboraMail.png"  >
                    </igtbar:TBarButton>
                    <igtbar:TBarButton Key="btnPulisci" Text="Pulisci filtri" Image="~/images/eraser.png" >
                        <DefaultStyle CssClass="infratoolbar_button_default" Width="90px"></DefaultStyle>
                    </igtbar:TBarButton>
                </Items>
            </igtbar:UltraWebToolbar>

        </div>
        <div class="sezione" id="PanelFiltri" runat="server">
            <asp:Label ID="lblSezioneFiltri" runat="server">Filtri di stampa</asp:Label>
        </div>
            
        <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
         
            <div class="vac-riga">
                <div class="vac-colonna-sinistra">
                    <fieldset title="Centro Vaccinale" class="fldroot fieldset_height_70">
                        <legend class="label">Centro Vaccinale</legend>
                        <uc2:SelezioneAmbulatorio ID="uscScegliAmb" runat="server" Tutti="True" MostraPulsanteClean="true" />
                    </fieldset>                    
                </div>
                <div class="vac-colonna-destra">
                    <fieldset title="Ultima Stampa Avviso" class="fldroot fieldset_height_70">
                        <legend class="label">Dati ultima stampa Avviso</legend>
                        <table>
                            <colgroup>
                                <col width="60px" />
                                <col />
                            </colgroup>
                            <tr>
                                <td>
                                    <asp:Label ID="lblCaptionAvvDa" Text="Da data:" CssClass="label_left" runat="server"></asp:Label></td>
                                <td>
                                    <asp:Label ID="lblUltimoAvvisoDataDa" Style="font-weight: bold" CssClass="label_left" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblCaptionAvvA" Text="A data:" CssClass="label_left" runat="server"></asp:Label></td>
                                <td>
                                    <asp:Label ID="lblUltimoAvvisoDataA" style="font-weight: bold" CssClass="label_left" runat="server"></asp:Label></td>
                            </tr>
                        </table>
                    </fieldset>
                </div>
            </div>
            
            <div class="vac-riga">
                <div class="vac-colonna-sinistra">
                    <fieldset title="Periodo di Appuntamento" class="fldroot fieldset_height_90" >
                        <legend class="label">Periodo Appuntamento</legend>
                        <table>
                            <colgroup>
                                <col width="60px" />
                                <col />
                            </colgroup>
                            <tr>
                                <td class="label_left">Da data:</td>
                                <td>
                                    <on_val:OnitDatePick ID="dpkDataAppInizio" runat="server" Width="136px"
                                        CssClass="textbox_data_obbligatorio" DateBox="True"></on_val:OnitDatePick>
                                </td>
                            </tr>
                            <tr>
                                <td class="label_left">A data:</td>
                                <td>
                                    <on_val:OnitDatePick ID="dpkDataAppFine" runat="server" Width="136px" 
                                        CssClass="textbox_data_obbligatorio" DateBox="True"></on_val:OnitDatePick>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </div>
                <div class="vac-colonna-destra">
                    <fieldset title="Soggetti" class="fldroot fieldset_height_90" >
                        <legend class="label">Soggetti</legend>
                        <asp:RadioButtonList ID="rdbFiltroSoggetti" runat="server" CssClass="textbox_stringa">
                        </asp:RadioButtonList>
                    </fieldset>
                </div>
            </div>
            
            <div class="vac-riga">
                <div class="vac-colonna-sinistra">
                    <fieldset title="Intervallo di Nascita" class="fldroot fieldset_height_70" >
                        <legend class="label">Intervallo Nascita</legend>
                        <table>
                            <colgroup>
                                <col width="60px"/>
                                <col />
                            </colgroup>
                            <tr>
                                <td class="label_left">Da data:</td>
                                <td>
                                    <on_val:OnitDatePick ID="dpkNascitaInizio" runat="server" Width="136px"
                                        CssClass="textbox_data" DateBox="True"></on_val:OnitDatePick>
                                </td>
                            </tr>
                            <tr>
                                <td class="label_left">A data:</td>
                                <td>
                                    <on_val:OnitDatePick ID="dpkNascitaFine" runat="server" Width="136px" 
                                        CssClass="textbox_data" DateBox="True"></on_val:OnitDatePick>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </div>
                <div class="vac-colonna-destra">
                    <fieldset title="Cittadinanza" class="fldroot fieldset_height_70" >
                        <legend class="label">Cittadinanza</legend>
                        <on_ofm:OnitModalList id="fmCittadinanza" runat="server" UseCode="True" SetUpperCase="True" Width="70%" CodiceWidth="30%"  style="margin-top: 12px;"
                            RaiseChangeEvent="False" CampoCodice="CIT_CODICE Codice" CampoDescrizione="CIT_STATO Stato" Tabella="T_ANA_CITTADINANZE" 
                            Obbligatorio="False" PosizionamentoFacile="False" LabelWidth="-8px"  Filtro="CIT_SCADENZA IS NULL ORDER BY Stato"></on_ofm:OnitModalList>
                    </fieldset>
                </div>
            </div>
            
            <div class="vac-riga">
                <div class="vac-colonna-sinistra">
                    <fieldset title="Tipo Avviso Postel" class="fldroot vac-fieldset-height-45" >
                        <legend class="label">Tipo Avviso Postel</legend>
                        <!-- 
							I value della radiobuttonlist sono usati per filtrare sulla
                            V_AVVISI_POSTEL.TIPO_AVVISO
						-->
                        <asp:RadioButtonList ID="rblExportPostel" name="rblExportPostel" runat="server" CssClass="textbox_stringa" RepeatColumns="4">
                        </asp:RadioButtonList>
                    </fieldset>
                </div>
                <div class="vac-colonna-destra">
                    <fieldset title="Associazioni-Dosi" class="fldroot vac-fieldset-height-45">
                        <legend class="label">Associazioni-Dosi</legend>
                        <table id="tblAssociazioniDosi" width="100%" style="margin: 0;">
                            <tr>
                                <td width="26px" align="right">
                                    <asp:ImageButton ID="btnImgAssociazioniDosi" runat="server" onmouseover="mouseRollOver(this,'over');"
                                        title="Impostazione filtro associazioni-dosi" onmouseout="mouseRollOver(this,'out');"
                                        ImageUrl="../../images/filtro_associazioni.gif" />
                                </td>
                                <td style="height: 22px; border: navy 1px solid; padding-left: 5px; background: gainsboro">
                                    <asp:Label ID="lblAssociazioniDosi" Style="height: 18px; font-size: 10px; text-transform: uppercase; font-style: italic; font-family: verdana" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </div>
            </div>
            <div class="vac-riga">
                 <div class="vac-colonna-sinistra">
                      <fieldset title="Tipo Avviso Postel" class="fldroot vac-fieldset-height-45" >
                        <legend class="label">Distretti</legend>
                          <asp:DropDownList ID="ddlDistretti" runat="server" CssClass="textbox_stringa" Width="100%">
                          </asp:DropDownList>
                      </fieldset>
                 </div>
                <div class="vac-colonna-destra">

                </div>
            </div>

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
                    <uc1:UscFiltroPrenotazioneSelezioneMultipla ID="UscFiltroAssociazioniDosi" runat="server" Tipo="Associazioni_Dosi"></uc1:UscFiltroPrenotazioneSelezioneMultipla>
                </td>
                <td></td>
            </tr>
            <tr height="10px">
                <td colspan="5"></td>
            </tr>
            <tr>
                <td></td>
                <td align="right">
                    <asp:Button Style="cursor: pointer" ID="btnOk_FiltroAssociazioniDosi" runat="server" Width="100px" Text="OK"></asp:Button>
                </td>
                <td></td>
                <td>
                    <asp:Button Style="cursor: pointer" ID="btnAnnulla_FiltroAssociazioniDosi" runat="server" Width="100px" Text="Annulla"></asp:Button>
                </td>
                <td></td>
            </tr>
            <tr height="10px">
                <td colspan="5"></td>
            </tr>
        </table>
    </on_ofm:OnitFinestraModale>

    <script type="text/javascript">
        if (<%= (Not IsPostBack).ToString().ToLower() %>)
            OnitDataPickFocus('dpkDataAppInizio', 1, false);
    </script>
    </form>
</body>
</html>
