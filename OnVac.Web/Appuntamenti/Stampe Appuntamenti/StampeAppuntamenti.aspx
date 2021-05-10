<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="StampeAppuntamenti.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.StampeAppuntamenti" %>

<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators"  %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<%@ Register TagPrefix="uc2" TagName="SelezioneAmbulatorio" Src="../../Common/Controls/SelezioneAmbulatorio.ascx" %>
<%@ Register TagPrefix="uc1" TagName="UscFiltroPrenotazioneSelezioneMultipla" Src="../Gestione Appuntamenti/UscFiltroPrenotazioneSelezioneMultipla.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Stampe Appuntamenti</title>
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    
    <style type="text/css">
        .postel_abilitato
        {
            font-family: Arial;
            font-size: 12px;
            color: Black;
        }
        
        .postel_disabilitato
        {
            font-family: Arial;
            font-size: 12px;
            color: Gray;
        }

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

        .fieldset_height_80 {
            height: 80px;
        }

        .fieldset_height_95 {
            height: 95px;
        }

        .fieldset_height_115 {
            height: 115px;
        }
    </style>
    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
    <script type="text/javascript">

        var oldSel = 'EA';

        function Index_Changed() {
            var newSel = getSelectedItem();

            if (newSel != oldSel) {
                // Combo malattia cronica abilitata solo per report Elenco Bilanci Malattia
                setEnabledComboMalattiaCronica(newSel);

                // Checkbox export postel abilitato solo per report Avvisi
                setEnabledCheckboxExportPostel(newSel);

                oldSel = newSel;
            }
        }

        function setEnabledComboMalattiaCronica(tipoReportSelezionato) {
            var cmb = document.getElementById('cmbMalCronica');

            cmb.disabled = (tipoReportSelezionato != 'EBM');
        }

        // In caso di selezione della Stampa Avvisi, 
        // abilito il checkbox di scelta dell'export del tracciato postel
        function setEnabledCheckboxExportPostel(tipoReportSelezionato) {
            var chk = document.getElementById('chkExportPostel');

            if (chk != null) {
                var rbl = document.getElementById('rblExportPostel');
                var td = document.getElementById('tdExportPostel');

                if (tipoReportSelezionato == 'A') {
                    chk.disabled = false;
                    rbl.disabled = false;
                    td.className = 'postel_abilitato';
                }
                else {
                    chk.checked = false;
                    chk.disabled = true;
                    rbl.disabled = true;
                    td.className = 'postel_disabilitato';
                }


                setRadioButtonPostelSelected();
            }
        }

        function setRadioButtonPostelSelected() {
            var items = document.getElementsByName('rblExportPostel');

            if (!document.getElementById('rblExportPostel').disabled) 
            {
                for (i = 0; i < items.length; i++) 
                {
                    if (items[i].value != null && items[i].checked) 
                    {
                        return;
                    }
                }
            }

            for (i = 0; i < items.length; i++) 
            {
                if (items[i].value != null && items[i].value == '') 
                {
                    items[i].checked = true;
                }
            }
        }

        function getSelectedItem() {
            var rbl = document.forms["Form1"].optModalitàStampa;

            for (i = 0; i < rbl.length; i++) {
                if (rbl[i].checked) {
                    var checkedVal = rbl[i].value;
                }
            }
            return checkedVal;
        }

        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;

            switch (button.Key) {
                case 'btnStampa':
                    if (!checkDatiObbligatori()) {
                        var msg;

                        var chk = document.getElementById('chkExportPostel');

                        if (chk != null && chk.checked && getSelectedItem() == 'A') {
                            msg = "Impossibile esportare il tracciato!";
                        }
                        else {
                            msg = "Impossibile stampare il report!";
                        }

                        alert("Non tutti i campi obbligatori sono impostati. " + msg);

                        evnt.needPostBack = false;
                    }
                    break;
            }
        }

        // Restituisce true se le date di inizio e fine periodo sono entrambe valorizzate.
        function checkDatiObbligatori() {
            return (OnitDataPickGet('odpDataIniz') != "" && OnitDataPickGet('odpDataFin') != "");
        }

        function onLoadPage() {
            var tipoReportSelezionato = getSelectedItem();

            // Abilita/disabilita la combo malattia cronica
            setEnabledComboMalattiaCronica(tipoReportSelezionato);

            // Abilita/disabilita il checkbox export postel
            setEnabledCheckboxExportPostel(tipoReportSelezionato);
        }
		
    </script>
</head>
<body onload="onLoadPage()">
    <form id="Form1" method="post" runat="server">
    <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Height="100%" Width="100%" Titolo="Stampe appuntamenti">
		
        <div class="title" id="PanelTitolo" runat="server">
            <asp:Label ID="LayoutTitolo" runat="server"> Stampe appuntamenti </asp:Label>
        </div>
        <div>
            <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="Toolbar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                <DefaultStyle CssClass="infratoolbar_button_default">
                </DefaultStyle>
                <HoverStyle CssClass="infratoolbar_button_hover">
                </HoverStyle>
                <SelectedStyle CssClass="infratoolbar_button_selected">
                </SelectedStyle>
                <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                <Items>
                    <igtbar:TBarButton Key="btnStampa" Text="Stampa" DisabledImage="~/Images/stampa.gif"
                        Image="~/Images/Stampa.gif">
                    </igtbar:TBarButton>
                    <igtbar:TBarButton Key="btnStampaUltimoAvviso" Text="Ultimi Avvisi" DisabledImage="~/Images/stampa.gif"
                        Image="~/Images/Stampa.gif" Visible="false">
                        <DefaultStyle Width="110px" CssClass="infratoolbar_button_default">
                        </DefaultStyle>
                    </igtbar:TBarButton>
                    <igtbar:TBarButton Key="btnStampaUltimoBilancio" Text="Ultimi Bilanci" DisabledImage="~/Images/stampa.gif"
                        Image="~/Images/Stampa.gif" Visible="false">
                        <DefaultStyle Width="110px" CssClass="infratoolbar_button_default">
                        </DefaultStyle>
                    </igtbar:TBarButton>
                </Items>
            </igtbar:UltraWebToolbar>
        </div>
        <div class="sezione" id="Panel23" runat="server">
            <asp:Label ID="LayoutTitolo_sezioneRicerca" runat="server">Filtri di stampa</asp:Label>
        </div>

        <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">

            <div class="vac-riga">
                <div class="vac-colonna-sinistra">
                    <fieldset id="fldCnv" title="Centro Vaccinale" class="fldroot fieldset_height_70">
                        <legend class="label">Centro Vaccinale</legend>
                        <uc2:SelezioneAmbulatorio ID="uscScegliAmb" runat="server" Tutti="True" />
                    </fieldset>
                </div>
                <div class="vac-colonna-destra">
                    <fieldset id="fldOldPrint" title="Ultima stampa Avviso" class="fldroot fieldset_height_70" >
                        <legend class="label">Dati ultima stampa Avviso</legend>
                        <table>
                            <tr>
                                <td class="label_left elemento_sinistro">Da data:</td>
                                <td class="label elemento_destro_bold">
                                    <asp:Label ID="lblOldDataDa" Style="font-weight: bold" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="label_left elemento_sinistro">A data:</td>
                                <td class="label elemento_destro_bold">
                                    <asp:Label ID="lblOldDataA" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </div>
            </div>
            
            <div class="vac-riga">
                <div class="vac-colonna-sinistra">
                    <fieldset id="fldPeriodo" title="Appuntamento" class="fldroot fieldset_height_80" >
                        <legend class="label">Periodo</legend>
                        <table>
                            <tr>
                                <td class="label_left elemento_sinistro">Da data:</td>
                                <td class="elemento_destro">
                                    <on_val:OnitDatePick ID="odpDataIniz" runat="server" Height="20px" Width="136px"
                                        CssClass="textbox_data_obbligatorio" DateBox="True"></on_val:OnitDatePick>
                                </td>
                            </tr>
                            <tr>
                                <td class="label_left elemento_sinistro">A data:</td>
                                <td class="elemento_destro">
                                    <on_val:OnitDatePick ID="odpDataFin" runat="server" Height="20px" Width="136px" 
                                        CssClass="textbox_data_obbligatorio" DateBox="True"></on_val:OnitDatePick>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </div>
                <div class="vac-colonna-destra">
                    <fieldset id="fldOldPrintBil" title="Ultima stampa Bilancio" class="fldroot fieldset_height_80" >
                        <legend class="label">Dati ultima stampa Bilancio</legend>
                        <table>
                            <tr>
                                <td class="label_left elemento_sinistro">Da data:</td>
                                <td class="label elemento_destro_bold">
                                    <asp:Label ID="lblOldDataDaBil" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="label_left elemento_sinistro">A data:</td>
                                <td class="label elemento_destro_bold">
                                    <asp:Label ID="lblOldDataABil" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </div>
            </div>
            
            <div class="vac-riga">
                <div class="vac-colonna-sinistra">
                    <fieldset id="fldMalCronica" title="Elenco malattie croniche" class="fldroot fieldset_height_95" style="vertical-align: middle;">
                        <legend class="label">Elenco malattie croniche</legend>
                        <asp:DropDownList ID="cmbMalCronica" runat="server" Width="100%" style="margin-top: 25px;"
                            DataValueField="MAL_CODICE" DataTextField="MAL_DESCRIZIONE"></asp:DropDownList>
                    </fieldset>
                </div>
                <div class="vac-colonna-destra">
                    <fieldset id="fldNoteAvviso" title="Note Avviso" class="fldroot fieldset_height_95" >
                        <legend class="label">Note Avviso</legend>
                            <asp:TextBox ID="txtNoteAvviso" runat="server" TextMode="multiline" style="overflow-y: auto; margin-top: 2px;"
                                CssClass="textbox_stringa" Width="100%" Rows="4" MaxLength="4000"></asp:TextBox>
                    </fieldset>
                </div>
            </div>
            
            <div class="vac-riga">
                <div class="vac-colonna-sinistra">
                    <fieldset id="fldModalita" title="Modalità di stampa" class="fldroot fieldset_height_115">
                        <legend class="label">Modalità di stampa</legend>
                        <table id="tblModalita" cellspacing="0" cellpadding="0" width="100%" border="0">
                            <tr>
                                <td colspan="2">
                                    <!-- 
										Questa radiobutton list viene popolata lato server, 
										solo con gli elementi (tra quelli elencati) il cui
										report corrispondente è presente per l'installazione corrente.
									-->
                                    <asp:RadioButtonList ID="optModalitàStampa" onclick="Index_Changed()" runat="server" width="100%" CssClass="textbox_stringa" RepeatColumns="3">
                                        <asp:ListItem Value="A">Avvisi</asp:ListItem>
                                        <asp:ListItem Value="EA" Selected="True">Elenco Avvisi</asp:ListItem>
                                        <asp:ListItem Value="ETA">Etichette Avvisi</asp:ListItem>
                                        <asp:ListItem Value="B">Bilanci</asp:ListItem>
                                        <asp:ListItem Value="EB">Elenco Bilanci</asp:ListItem>
                                        <asp:ListItem Value="CA">Avvisi Campagna Adulti</asp:ListItem>
                                        <asp:ListItem Value="BM">Bilanci per Malattia cronica</asp:ListItem>
                                        <asp:ListItem Value="EBM">Elenco Bilanci per Malattia cronica</asp:ListItem>
                                        <asp:ListItem Value="EAA">Etichette assisititi</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr style="height: 25px; vertical-align: middle">
                                <td id="tdExportPostel" style="padding-left: 4px">
                                    <!-- 
									    La visibilità di questo checkbox è gestita lato server
									    in base al parametro per l'export del tracciato postel
									-->
                                    <asp:CheckBox ID="chkExportPostel" runat="server" Text="Esporta dati avvisi per tracciato POSTEL" />
                                    <!-- 
									    I value della radiobuttonlist sono usati per filtrare sulla
                                        V_AVVISI_POSTEL.TIPO_AVVISO
									-->
                                    <asp:RadioButtonList ID="rblExportPostel" name="rblExportPostel" runat="server" CssClass="textbox_stringa" RepeatColumns="4">
                                        <asp:ListItem Value="AV">Avvisi</asp:ListItem>
                                        <asp:ListItem Value="SL">Sollecito</asp:ListItem>
                                        <asp:ListItem Value="TP">Termine Perentorio</asp:ListItem>
                                        <asp:ListItem Value="" Selected="True">Tutti</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </div>
                <div class="vac-colonna-destra">
                    <fieldset id="fldSoggetti" title="Soggetti" class="fldroot fieldset_height_115">
                        <legend class="label">Soggetti</legend>
                        <table id="tblSoggetti" cellspacing="0" cellpadding="0" width="100%" border="0">
                            <tr>
                                <td>
                                    <asp:RadioButtonList ID="rdbFiltroSoggetti" runat="server" CssClass="textbox_stringa">
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </div>
            </div>
            <div class="vac-riga">
                <div class="vac-colonna-sinistra">
                    <fieldset title="Associazioni-Dosi" class="fldroot vac-fieldset-height-45">
                        <legend class="label">Associazioni-Dosi</legend>
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 5%; text-align: right">
                                    <asp:ImageButton ID="btnImgAssociazioniDosi" runat="server" onmouseover="mouseRollOver(this,'over');"
                                        title="Impostazione filtro associazioni-dosi" onmouseout="mouseRollOver(this,'out');"
                                        ImageUrl="../../images/filtro_associazioni.gif" />
                                </td>
                                <td style="height: 22px; border: navy 1px solid; padding-left: 5px; background: gainsboro; width:95%">
                                    <asp:Label ID="lblAssociazioniDosi" Style="height: 18px; font-size: 10px; text-transform: uppercase; font-style: italic; font-family: verdana" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </div>
                <div class="vac-colonna-destra">

                </div>
            </div>

            <table width="100%" border="0">
                <tr>
                    <td valign="bottom">
                        <asp:Label ID="Label1" runat="server" Width="100%" CssClass="label_errormsg"></asp:Label>
                    </td>
                </tr>
            </table>

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

    <script type="text/javascript" >
        
        if (<%= (Not IsPostBack).ToString().ToLower() %>)
            OnitDataPickFocus('odpDataIniz',1,false);
    </script>

    </form>
</body>
</html>
