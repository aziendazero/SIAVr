<%@ Page Language="vb" AutoEventWireup="false" Codebehind="AvvisoMaggiorenni.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.AvvisoMaggiorenni" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="uc1" TagName="SelezioneConsultori" Src="../../Common/Controls/SelezioneConsultori.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>AvvisoMaggiorenni</title>
		
        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

		<script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
        <script type="text/javascript">
		    var oldSel = 'EA';

		    function Index_Changed() {
		        var cmb = document.getElementById('cmbMalCronica');
		        var newSel = getSelectedItem();
				
		        if (newSel != oldSel){
		            if (newSel == 'EBM'){
		                cmb.disabled = false;
		            } else {
		                cmb.disabled = true;
		            }
		            oldSel = newSel;
		        } 
		    }

		    function getSelectedItem(){
		
		        var rbl = document.forms["Form1"].optModalit‡Stampa;
			
		        for (i=0;i<rbl.length;i++) {
		            if (rbl[i].checked) {
		                var checkedVal = rbl[i].value;
		            }
		        }
		        return checkedVal;
		    }
		
		    // inizializzazione della toolbar
		    function InizializzaToolBar(t) {
		        t.PostBackButton = false;
		    }
		
		    //controllo valore dei datepick
		    function ToolBarClick(ToolBar, button, evnt) {
		        evnt.needPostBack = true;
		        switch(button.Key)
		        {
		            case 'btnStampa':
		                if (OnitDataPickGet('odpDataNascitaIniz') == "" || OnitDataPickGet('odpDataNascitaFin') == "")
		                {
		                    alert("Non tutti i campi obbligatori sono impostati. Impossibile stampare il report.");
		                    evnt.needPostBack=false;
		                }
		                break;
		        }
		    }
		</script>
        <style type="text/css">
             .vac-fieldset-height-55 {
                 height: 55px;
             }

            .margin-bottom-5 {
                margin-bottom: 5px;
            }
        </style>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" titolo="Avviso maggiorenni" width="100%" height="100%">
				<div id="PanelTitolo" class="title" runat="server">
					<asp:Label id="LayoutTitolo" runat="server">Avviso Maggiorenni</asp:Label>
                </div>
                <div>
					<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="Toolbar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		                <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
						<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
						<Items>
							<igtbar:TBarButton Key="btnStampa" Text="Stampa" DisabledImage="~/Images/stampa.gif" Image="~/Images/Stampa.gif"></igtbar:TBarButton>
						</Items>
					</igtbar:UltraWebToolbar>
                </div>
				<div class="sezione" runat="server">
					<asp:Label id="LayoutTitolo_sezioneRicerca" runat="server">Filtri di stampa</asp:Label>
                </div>

                <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Centro Vaccinale" class="fldroot vac-fieldset-height-55">
                                <legend class="label margin-bottom-5">Centro Vaccinale</legend>
								<uc1:SelezioneConsultori id="ucSelezioneConsultori" runat="server" Width="100%"  MaxCnsInListaSelezionati="3"></uc1:SelezioneConsultori>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Ultima stampa Avviso" class="fldroot vac-fieldset-height-55">
								<legend class="label">Data ultima stampa avviso</legend>
                                <table style="width:100%">
                                    <colgroup>
                                        <col style="width: 15%" />
                                        <col style="width: 85%" />
                                    </colgroup>
                                    <tr>
                                        <td class="label">Da</td>
                                        <td>
                                            <asp:Label id="lblOldDataDa" Runat="server" CssClass="label_left" style="font-weight: bold"></asp:Label>
                                        </td>
									</tr>
									<tr>
										<td class="label">A</td>
                                        <td>
											<asp:Label id="lblOldDataA" Runat="server" CssClass="label_left" style="font-weight: bold"></asp:Label>
                                        </td>
									</tr>
								</table>
							</fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Comune" class="fldroot vac-fieldset-height-45">
                                <legend class="label margin-bottom-5">Comune di Residenza</legend>
								<on_ofm:onitmodallist id="fmComuneRes" runat="server" Width="70%" SetUpperCase="True" UseCode="True" Tabella="T_ANA_COMUNI"
									CampoDescrizione="COM_DESCRIZIONE" CampoCodice="COM_CODICE" Label="Comune" CodiceWidth="30%" LabelWidth="-8px"
									PosizionamentoFacile="False" RaiseChangeEvent="True"></on_ofm:onitmodallist>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Data nascita" class="fldroot vac-fieldset-height-45">
								<legend class="label">Compimento 18 anni</legend>
								<table style="width:100%">
                                    <tr>
                                        <td class="label">
                                            <asp:Label id="lblDataEffettuazioneIniz" runat="server">Da</asp:Label></td>
										<td>
                                            <on_val:onitdatepick id="odpDataNascitaIniz" runat="server" CssClass="textbox_data_obbligatorio" Width="136px" DateBox="True"></on_val:onitdatepick></td>
										<td class="label">
											<asp:Label id="lblDataEffettuazioneFin" runat="server">A</asp:Label></td>
										<td>
                                            <on_val:onitdatepick id="odpDataNascitaFin" runat="server" CssClass="textbox_data_obbligatorio" Width="136px" DateBox="True"></on_val:onitdatepick></td>
									</tr>
								</table>
							</fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <fieldset id="fldStatoAnag" title="Stato anagrafico" class="fldroot" >
							<legend class="label">Stato anagrafico</legend>
                            <onit:CheckBoxList id="chklStatoAnagrafico" runat="server" CssClass="label_left" RepeatDirection="Horizontal" TextAlign="Right" RepeatColumns="5" Width="100%"></onit:CheckBoxList>
						</fieldset>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset id="fldSoggetti" title="Soggetti" class="fldroot">
								<legend class="label">Soggetti</legend>
                                <asp:RadioButtonList id="rdbFiltroSoggetti" runat="server" CssClass="textbox_stringa" Width="100%">
									<asp:ListItem Value="1" Selected="True">Tutti</asp:ListItem>
									<asp:ListItem Value="2">Solo quelli gi&#224; avvisati</asp:ListItem>
									<asp:ListItem Value="3">Solo quelli non avvisati</asp:ListItem>
								</asp:RadioButtonList>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            
                        </div>
                    </div>
                </dyp:DynamicPanel>

                <dyp:DynamicPanel ID="dypMsg" runat="server" Width="100%" Height="18px">
                    <asp:Label id="Label1" runat="server" Width="100%" CssClass="label_errormsg"></asp:Label>
                </dyp:DynamicPanel>

			</on_lay3:onitlayout3>
		</form>

		<script type="text/javascript">
		    if (<%= (Not IsPostBack).ToString().ToLower() %>)
			    OnitDataPickFocus('odpDataIniz',1,false);
		</script>

	</body>
</html>
