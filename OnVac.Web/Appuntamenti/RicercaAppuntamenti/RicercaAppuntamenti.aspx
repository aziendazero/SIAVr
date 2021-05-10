<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RicercaAppuntamenti.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_RicercaAppuntamenti" %>

<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel"  Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<%@ Register TagPrefix="uc2" TagName="SelezioneAmbulatorio" Src="../../common/Controls/SelezioneAmbulatorio.ascx" %>
<%@ Register TagPrefix="uc1" TagName="StoricoAppuntamenti" Src="../../Common/Controls/ElencoStoricoAppuntamenti.ascx" %>

<%@ Import Namespace="Onit.OnAssistnet.OnVac" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Ricerca Appuntamenti</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <style type="text/css">
        .headerListDataOra {
            background-color: #485d96;
            font-family: Arial,Tahoma,Verdana;
            color: white;
            font-size: 12px;
            font-weight: bold;
        }

        .corniceDati {
            border: 2px solid;
            margin: 3px 3px 0px 3px;
        }

        .corniceDatiLast {
            border: 2px solid;
            margin: 3px 3px 3px 3px;
        }

        .vac-sezione {
            margin: 3px 3px 0px 3px;
        }

        .btn-calendar {
            background-image: url('../../images/calendario.gif');
            background-repeat: no-repeat;
            background-position-x: center;
            background-position-y: center;
            cursor: pointer;
            width: 30px;
            margin-right: 3px;
        }
                
        .btn-calendar-disabled {
            background-image: url('../../images/calendario_dis.gif');
            background-repeat: no-repeat;
            background-position-x: center;
            background-position-y: center;
            cursor: default;
            width: 30px;
            margin-right: 3px;
        }

        .Label_ErrorMsg {
            margin-left: 5px;
        }

        .Festa {
            background-color: #c7c7c7;
        }

        .Indisponibile {
            background-color: #c7c7c7;
        }
    </style>

    <script type="text/javascript" src='<%= OnVacUtility.GetScriptUrl("onvac.common.js")%>'></script>
    <script type="text/javascript">

        var PreviousStartDate = "";

        function InizializzaToolBar(t)
        {
            t.PostBackButton=false;
        }
			
        function ToolBarClick(ToolBar,button,evnt)
        {
            evnt.needPostBack=true;
			
            switch(button.Key)
            {
                case 'btnConferma':
                    if (<% = cancellaDataInvio.ToString().ToLower() %>) {
						if (!confirm('Attenzione: salvando la nuova data di appuntamento verrà cancellata la precedente data di invio. Continuare?')) {
					        evnt.needPostBack = false;
					    }
                    }
                    break;
            }
        }
			
        function ChiudiError() {
            closeFm('<%= fmError.clientid() %>');
		}

		function OnitDataPick_Blur(id, e) {
			SetEndDate(id);
		}

		function OnitDataPick_ClickDay(id) {
			SetEndDate(id);
		}

		function SetEndDate(id) {
			if (id == "odpDispFine") return;

			var startDate = OnitDataPickGet("odpDispInizio");

			if (startDate != "" && startDate != PreviousStartDate)
			{
			    var endDate = addDays(startDate, <%= Me.GiorniIntervalloDateDisponibili %>); 
                OnitDataPickSet("odpDispFine", endDate);
                PreviousStartDate = startDate;
            }
        }
    </script>

</head>
<body style="margin: 0px">
    <form id="Form1" method="post" runat="server" style="margin: 0px">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" TitleCssClass="Title3" Titolo="Ricerca appuntamenti">
            <div class="title">
                <asp:Label ID="LayoutTitolo" runat="server" Width="955"></asp:Label>
            </div>
            <div>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                    <Items>
                        <igtbar:TBarButton Key="btnConferma" Text="Salva" Enabled="false" DisabledImage="~/Images/salva_dis.gif"
                            Image="~/Images/salva.gif">
                        </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btnAnnulla" Text="Annulla" Enabled="false" DisabledImage="~/Images/annulla_dis.gif"
                            Image="~/Images/annulla.gif">
                        </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btnAnnullaPrenotazione" Text="Elimina prenotazione" DisabledImage="~/Images/elimina_dis.gif"
                            Image="~/Images/elimina.gif" Enabled="false">
                            <DefaultStyle Width="150px" CssClass="infratoolbar_button_default"></DefaultStyle>
                        </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btnStampaAppuntamento" Text="Appuntamento" DisabledImage="~/Images/stampa_dis.gif"
                            Image="~/Images/Stampa.gif">
                            <DefaultStyle Width="110px" CssClass="infratoolbar_button_default"></DefaultStyle>
                        </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btnBilancioBianco" Text="Bilancio in Bianco" DisabledImage="~/Images/stampa_dis.gif"
                            Image="~/Images/Stampa.gif">
                            <DefaultStyle Width="130px" CssClass="infratoolbar_button_default"></DefaultStyle>
                        </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btnEliminaSoloBilancio" Text="Elimina Solo Bilancio" DisabledImage="~/Images/elimina_dis.gif"
                            Image="~/Images/elimina.gif" Enabled="false">
                            <DefaultStyle Width="150px" CssClass="infratoolbar_button_default"></DefaultStyle>
                        </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btnStoricoAppuntamento" Text="Storico Appuntamento" DisabledImage="~/Images/calendario_dis.gif"
                            Image="~/Images/calendario.gif" Enabled="false">
                            <DefaultStyle Width="160px" CssClass="infratoolbar_button_default"></DefaultStyle>
                        </igtbar:TBarButton>
                    </Items>
                </igtbar:UltraWebToolbar>

            </div>
            <dyp:DynamicPanel ID="dypR1" runat="server" Width="100%" Height="50%" ExpandDirection="horizontal">
                <dyp:DynamicPanel ID="dypVr11" runat="server" Width="50%" Height="100%">
                    <div class="vac-sezione" id="divLayoutPrenot" style="">
                        <asp:Label ID="LayoutPrenot" runat="server">Prenotazioni</asp:Label>
                    </div>
                    <dyp:DynamicPanel ID="dypElencoDate" runat="server" Height="100%" ScrollBars="Auto" CssClass="corniceDati" RememberScrollPosition="true">
                        <table id="TableHeader3" cellspacing="0" cellpadding="0" width="100%" border="0">
                            <tr class="headerListDataOra">
                                <td width="5%"></td>
                                <td width="53%">Data</td>
                                <td width="20%">Ora</td>
                                <td width="22%">CV</td>
                            </tr>
                            <tr>
                                <td colspan="4">
                                    <asp:DataList ID="dltDateAppuntamenti" runat="server" Width="100%" CssClass="DataGrid">
                                        <HeaderStyle CssClass="Header"></HeaderStyle>
                                        <SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
                                        <AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
                                        <ItemStyle CssClass="Item"></ItemStyle>
                                        <ItemTemplate>
                                            <table id="Table9" style="font-size: 10px;" cellspacing="1" cellpadding="1" width="100%" height="100%" border="0">
                                                <tr>
                                                    <td width="5%" align="center">
                                                        <asp:ImageButton ID="ImageButton1" runat="server" CommandName="SelezionaData" ImageUrl='<%# Me.ResolveClientUrl("~/images/sel.gif") %>'></asp:ImageButton>
                                                    </td>
                                                    <td width="53%">
                                                        <asp:Label ID="lblData" runat="server" Text='<%# OnVacUtility.GetFormattedValue(Container.DataItem("CNV_DATA_APPUNTAMENTO"), "System.DateTime", "d", "(Data appuntamento non impostata)") %>'>
                                                        </asp:Label>
                                                    </td>
                                                    <td width="20%">
                                                        <asp:Label ID="lblOra" runat="server" Text='<%# OnVacUtility.GetFormattedValue(Container.DataItem("CNV_DATA_APPUNTAMENTO"), "System.DateTime", "t") %>'>
                                                        </asp:Label>
                                                    </td>
                                                    <td width="22%">
                                                        <asp:Label ID="lblCnsCnvSelezionata" runat="server" Text='<%# Container.DataItem("CNV_CNS_CODICE") %>'>
                                                        </asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </ItemTemplate>
                                    </asp:DataList>
                                </td>
                            </tr>
                        </table>
                    </dyp:DynamicPanel>
                    <div class="vac-sezione" id="divCnsAppuntamentoTitolo" style="">
                        <asp:Label ID="lblCnsAppuntamentoDiv" runat="server">Centro Vaccinale di Appuntamento</asp:Label>
                    </div>
                    <div id="divCnsAppuntamento" runat="server" class="corniceDati">

                        <table width="90%">
                            <colgroup>
                                <col width="50%" />
                                <col width="50%" />
                            </colgroup>
                            <tr>
                                <td>
                                    <dyp:DynamicPanel runat="server" Height="100%" Width="100%" Style="text-align: left;">
                                        <asp:CheckBox ID="chkShowPrenotazioni" runat="server" AutoPostBack="true" Text="Mostra anche le prenotazioni" CssClass="label_left" ToolTip="Mostra in elenco anche i posti già prenotati" />
                                    </dyp:DynamicPanel>
                                </td>
                                <td>
                                    <dyp:DynamicPanel runat="server" Height="100%" Width="100%" Style="text-align: left;">
                                        <asp:CheckBox ID="chkMantieniCnsAppuntamento" runat="server" AutoPostBack="true" Text="Mantieni CV di convocazione" CssClass="label_right" TextAlign="Right" ToolTip="Mantiene il centro vaccinale di appuntamento uguale a quello impostato nella convocazione selezionata" />
                                    </dyp:DynamicPanel>
                                </td>
                            </tr>
                        </table>

                        <div style="margin: 0 3px">
                            <!-- Selezione dell'ambulatorio -->
                            <uc2:SelezioneAmbulatorio ID="uscScegliAmb" runat="server" Tutti="True" />
                        </div>
                    </div>
                </dyp:DynamicPanel>
                <dyp:DynamicPanel ID="dypVr12" runat="server" Width="50%" Height="100%">
                    <div class="vac-sezione" id="divLayoutInfo" style="">
                        <asp:Label ID="LayoutInfo" runat="server">Informazioni</asp:Label>
                    </div>
                    <dyp:DynamicPanel ID="dypInfo" runat="server" Height="100%" CssClass="corniceDati" Style="">
                        <div>
                            <table id="Table1" style="table-layout: fixed" cellspacing="1" cellpadding="1" width="100%" border="0">
                                <colgroup>
                                    <col width="40%" />
                                    <col width="60%" />
                                </colgroup>
                                <tr>
                                    <td>
                                        <div class="TextBox_Stringa">Convocazione</div>
                                    </td>
                                    <td class="Label_left">
                                        <asp:Label ID="lblConvocazione" runat="server" Text='<%# OnVacUtility.GetFormattedValue(p_dtaDateAppuntamenti.Rows(dltDateAppuntamenti.selectedIndex)("CNV_DATA"), "System.DateTime", "f") %>' Font-Bold="True">
                                        </asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="top">
                                        <div class="TextBox_Stringa">Centro Vacc. e Ambulatorio</div>
                                    </td>
                                    <td valign="top" class="Label_left">
                                        <asp:Label ID="lblCnsAmb" runat="server" Font-Bold="True" Style="vertical-align: top">
                                        </asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div class="TextBox_Stringa">Primo appuntamento</div>
                                    </td>
                                    <td class="Label_left">
                                        <asp:Label ID="lblPrimoAppuntamento" runat="server" Text='<%# p_dtaDateAppuntamenti.Rows(dltDateAppuntamenti.SelectedIndex)("CNV_PRIMO_APPUNTAMENTO") %>' Font-Bold="True">
                                        </asp:Label></td>
                                </tr>
                                <tr>
                                    <td>
                                        <div class="TextBox_Stringa">Durata</div>
                                    </td>
                                    <td class="Label_left">
                                        <asp:Label ID="lblDurata" runat="server" Text='<%# p_dtaDateAppuntamenti.Rows(dltDateAppuntamenti.selectedIndex)("CNV_DURATA_APPUNTAMENTO") %>' Font-Bold="True">
                                        </asp:Label></td>
                                </tr>
                                <tr>
                                    <td>
                                        <div class="TextBox_Stringa">Medico</div>
                                    </td>
                                    <td class="Label_left">
                                        <asp:Label ID="lblMedico" runat="server" Text='<%# p_dtaDateAppuntamenti.Rows(dltDateAppuntamenti.selectedIndex)("OPE_NOME") %>' Font-Bold="True">
                                        </asp:Label></td>
                                </tr>
                                <tr valign="top">
                                    <td class="label_left">
                                        <asp:Label ID="lblRitardoInt" runat="server">Ritardo/i: </asp:Label>
                                        <asp:Label ID="lblRitardo" runat="server" Font-Bold="True"></asp:Label>
                                    </td>
                                    <td class="Label_left">
                                        <asp:Label ID="lblSollecito" runat="server" Font-Bold="True">
                                        </asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label_left">
                                        <asp:Label ID="lblBilancioInt" runat="server">Bilancio</asp:Label></td>
                                    <td class="Label_left">
                                        <asp:Label ID="lblBilancio" runat="server" Text='<%# Me.GetDescrizioneBilancioMalattia(p_dtaDateAppuntamenti.Rows(dltDateAppuntamenti.SelectedIndex)("CNV_DATA")) %>' Font-Bold="True">
                                        </asp:Label>
                                    </td>
                                </tr>
                                 <tr>
                                    <td class="label_left">
                                        <asp:Label ID="lblIdAppuntamentoInt" runat="server">Id Appuntamento</asp:Label>
                                    </td>
                                    <td class="label_left">
                                        <asp:Label ID="lblIdAppuntamento" runat="server" Text='<%# p_dtaDateAppuntamenti.Rows(dltDateAppuntamenti.SelectedIndex)("CNV_ID_CONVOCAZIONE") %>' Font-Bold="True"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <dyp:DynamicPanel ID="dypElencoVac" Height="100%" Style="overflow: auto;" runat="server">
                            <table id="TableHeader2" style="font-size: 10px" cellspacing="0" cellpadding="0" width="100%" border="0">
                                <tr class="Header">
                                    <td align="left" width="10%">Vaccinazione</td>
                                    <td align="center" width="60%"></td>
                                    <td align="left" width="30%">Dose</td>
                                </tr>
                            </table>
                            <asp:DataList ID="dltVaccinazioni" runat="server" Width="100%" CssClass="DataGrid">
                                <SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
                                <AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
                                <ItemStyle CssClass="Item"></ItemStyle>
                                <HeaderStyle CssClass="Header"></HeaderStyle>
                                <ItemTemplate>
                                    <table id="Table17" style="font-size: 10px" cellspacing="1" cellpadding="1" width="100%"
                                        border="0">
                                        <tr>
                                            <td width="10%">
                                                <asp:Label ID="Label1" runat="server" Text='<%# Container.DataItem("VPR_VAC_CODICE") %>'>
                                                </asp:Label></td>
                                            <td width="60%">
                                                <asp:Label ID="Label2" runat="server" Text='<%# Container.DataItem("VAC_DESCRIZIONE") %>'>
                                                </asp:Label></td>
                                            <td width="30%">
                                                <asp:Label ID="Label5" runat="server" Text='<%# Container.DataItem("VPR_N_RICHIAMO") %>'>
                                                </asp:Label></td>
                                        </tr>
                                    </table>
                                </ItemTemplate>
                            </asp:DataList>
                        </dyp:DynamicPanel>
                    </dyp:DynamicPanel>
                </dyp:DynamicPanel>
            </dyp:DynamicPanel>
            <dyp:DynamicPanel ID="dypR2" runat="server" Width="100%" Height="50%" ExpandDirection="horizontal" Style="">
                <dyp:DynamicPanel ID="dypV21" runat="server" Width="50%" Height="100%">
                    <div class="vac-sezione" id="divLayoutDisp">Disponibilità</div>
                    <div style="overflow: hidden">
                        <fieldset id="fldDisponibilità" title="Periodo" class="fldroot_clone" style="">
                            <legend class="label">Periodo</legend>
                            <table id="Table7" width="100%" border="0" class="TextBox_Stringa">
                                <tr>
                                    <td width="8%" style="text-align: center; vertical-align: middle;">Dal</td>
                                    <td>
                                        <on_val:OnitDatePick ID="odpDispInizio" runat="server" Width="100%" CssClass="textbox_data_obbligatorio" NoCalendario="False"
                                            DateBox="True"></on_val:OnitDatePick>
                                    </td>
                                    <td width="8%" style="text-align: center; vertical-align: middle;">Al</td>
                                    <td>
                                        <on_val:OnitDatePick ID="odpDispFine" runat="server" Width="100%" CssClass="textbox_data_obbligatorio" NoCalendario="False"
                                            DateBox="True"></on_val:OnitDatePick>
                                    </td>
                                    <td valign="middle" align="center" width="14%">
                                        <asp:Button ID="btnCercaData" runat="server" Width="65px" Height="24px" Text="Cerca" style="cursor: pointer"></asp:Button></td>
                                </tr>
                            </table>
                        </fieldset>
                    </div>
                    <dyp:DynamicPanel ID="dypDisp" runat="server" Height="100%" CssClass="corniceDatiLast" Style="">
                        <dyp:DynamicPanel runat="server" ExpandDirection="horizontal" Height="30px" Style="">

                            <dyp:DynamicPanel runat="server" Height="100%" Width="130px" Style="padding-top: 5px">
                                <span class="label_left" style="margin-left: 2px;" title="Totale appuntamenti prenotati">Appuntamenti:&nbsp;</span>
                                <asp:Label ID="lblTotAppuntamenti" runat="server"
                                    Style="font-weight: bold; font-size: 12px; font-family: Arial,Tahoma,Verdana; text-align: center; vertical-align: middle;"></asp:Label>
                            </dyp:DynamicPanel>

                            <dyp:DynamicPanel runat="server" Height="100%" Width="50%" Style="text-align: right; padding-top: 5px">
                                <asp:ImageButton ID="btnFirst" onmouseover="rollover(this,'over')" onmouseout="rollover(this,'out')"
                                    runat="server" ToolTip="Carica la prima data disponibile" Visible="true" ImageUrl="../../images/first_dis.gif"
                                    Enabled="false"></asp:ImageButton>

                                <asp:ImageButton ID="btnPrev" onmouseover="rollover(this,'over')" onmouseout="rollover(this,'out')"
                                    runat="server" ToolTip="Carica la data disponibile precedente" Visible="true" ImageUrl="../../images/prev_dis.gif"
                                    Enabled="false"></asp:ImageButton>
                            </dyp:DynamicPanel>
                            <dyp:DynamicPanel runat="server" Height="100%" Width="130px" Style="text-align: center; padding-top: 6px">
                                <asp:Label ID="lblCurrentDay" Style="font-weight: bold; font-size: 12px; font-family: Arial,Tahoma,Verdana; text-align: center; vertical-align: middle;"
                                    runat="server"></asp:Label>
                            </dyp:DynamicPanel>
                            <dyp:DynamicPanel runat="server" Height="100%" Width="50%" Style="text-align: left; padding-top: 5px">
                                <asp:ImageButton ID="btnNext" onmouseover="rollover(this,'over')" onmouseout="rollover(this,'out')"
                                    runat="server" ToolTip="Carica la data disponibile successiva" Visible="true" ImageUrl="../../images/next_dis.gif"
                                    Enabled="false"></asp:ImageButton>
                                <asp:ImageButton ID="btnLast" onmouseover="rollover(this,'over')" onmouseout="rollover(this,'out')"
                                    runat="server" ToolTip="Carica l'ultima data disponibile" Visible="true" ImageUrl="../../images/last_dis.gif"
                                    Enabled="false"></asp:ImageButton>
                            </dyp:DynamicPanel>

                            <dyp:DynamicPanel runat="server" Height="100%" Width="40px" Style="padding-top: 3px; text-align:right">
                                <asp:Button ID="btnCalendario" runat="server" class="btn-calendar" ToolTip="Apri calendario" />
                            </dyp:DynamicPanel>

                        </dyp:DynamicPanel>
                        <dyp:DynamicPanel ID="dypDispLs" runat="server" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                            <table id="TableHeader" style="font-size: 10px; width: 100%; table-layout: fixed" cellspacing="0" cellpadding="0" border="0">
                                <tr class="headerListDataOra">
                                    <td style="width: 20px"></td>
                                    <td style="width: 20%; text-align: center">Ora</td>
                                    <td style="width: 30%">Ambulatorio</td>
                                    <td style="width: 50%">Appuntamento</td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <asp:DataList ID="dltOrariDisponibili" runat="server" Width="100%" CssClass="DataGrid">
                                            <SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
                                            <AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
                                            <ItemStyle CssClass="Item"></ItemStyle>
                                            <HeaderStyle CssClass="Header"></HeaderStyle>
                                            <ItemTemplate>
                                                <table id="Table14" style="font-size: 10px; width; 100%; table-layout: fixed" cellspacing="1" cellpadding="1" border="0">
                                                    <tr>
                                                        <td style="width: 20px">
                                                            <asp:ImageButton ID="ImageButton2" runat="server" ImageAlign="AbsMiddle" Width="16px" ImageUrl='<%# GetImageUrlDisponibilitaOrario(Container)%>'
                                                                CommandName="Seleziona" Enabled='<%# AbilitaImageDisponibilitaOrario(Container)%>' AlternateText="" />
                                                            <asp:HiddenField ID="hfCodAmb" Value='<%# Container.DataItem("AmbCodice") %>' runat="server" />
                                                        </td>
                                                        <td style="width: 20%; text-align: center">
                                                            <asp:Label ID="Label8" runat="server" Text='<%# OnVacUtility.GetFormattedValue(Container.DataItem("Ora"), "System.DateTime", "HH.mm") %>'>
                                                            </asp:Label>
                                                        </td>
                                                        <td style="width: 30%">
                                                            <asp:Label ID="Label10" runat="server" Text='<%# Container.DataItem("Amb") %>'>
                                                            </asp:Label>
                                                        </td>
                                                        <td style="width: 50%">
                                                            <asp:Label ID="Label3" runat="server" Text='<%# Container.DataItem("Vaccinazioni") %>'>
                                                            </asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ItemTemplate>
                                        </asp:DataList>
                                    </td>
                                </tr>
                            </table>
                        </dyp:DynamicPanel>
                    </dyp:DynamicPanel>
                </dyp:DynamicPanel>

                <dyp:DynamicPanel ID="DynamicPanel2" runat="server" Width="50%" Height="100%" ExpandDirection="vertical">
                    <dyp:DynamicPanel ID="DynamicPanel1" runat="server" Width="100%">
                        <div class="vac-sezione" id="divLayoutAppunt" style="">Fissa appuntamento</div>
                        <div style="overflow: hidden">
                            <fieldset id="fldDataApp" title="Dati appuntamento" class="fldroot_clone">
                                <legend class="label">Dati appuntamento</legend>
                                <table id="Table3" width="100%" border="0">
                                    <tr>
                                        <td style="width: 10%" align="center" class="Label">
                                            <asp:Label ID="Label6" runat="server">Data: </asp:Label></td>
                                        <td style="width: 30%" valign="middle" align="left">
                                            <on_val:OnitDatePick ID="pickNuovaData" runat="server" Width="100%" CssClass="textbox_data_disabilitato"
                                                NoCalendario="False" DateBox="True"></on_val:OnitDatePick>
                                        </td>
                                        <td style="width: 10%" valign="middle" align="center" class="Label">
                                            <asp:Label ID="Label7" runat="server">Ora:</asp:Label></td>
                                        <td style="width: 10%" valign="middle" align="left">
                                            <asp:TextBox ID="textNuovaOra" runat="server" Width="100%" CssClass="textbox_data_disabilitato" MaxLength="5"></asp:TextBox></td>
                                        <td style="width: 10%" valign="middle" align="center" class="Label">
                                            <asp:Label ID="Label4" runat="server">Durata:</asp:Label></td>
                                        <td style="width: 5%" valign="middle" align="left">
                                            <on_val:OnitJsValidator ID="textNuovaDurata" runat="server" Width="25px" CssClass="textbox_data_disabilitato"
                                                actionCorrect="False" actionDelete="False" actionFocus="False" actionMessage="True" actionSelect="True"
                                                actionUndo="False" autoFormat="False" validationType="Validate_integer" PreParams-numDecDigit="0" PreParams-maxValue="59"
                                                PreParams-minValue="0" MaxLength="2"></on_val:OnitJsValidator></td>
                                        <td style="width: 20%" valign="middle" align="center">
                                            <asp:Button ID="btnAssegna" runat="server" Width="100%" Height="24px" Text="Assegna" style="cursor: pointer"></asp:Button></td>
                                    </tr>
                                </table>
                            </fieldset>
                        </div>
                    </dyp:DynamicPanel>
                    <dyp:DynamicPanel ID="dypV22" runat="server" Width="100%" Height="100%" ExpandDirection="horizontal">
                        <dyp:DynamicPanel ID="dypCalendario" runat="server" Width="200px" Height="100%">
                            <asp:Calendar ID="Calendario" runat="server" UseAccessibleHeader="false" CssClass="DataGrid" Height="90%">
                                <DayStyle CssClass="Alternating"></DayStyle>
                                <NextPrevStyle ForeColor="White"></NextPrevStyle>
                                <DayHeaderStyle CssClass="Header"></DayHeaderStyle>
                                <SelectedDayStyle BorderWidth="1px" ForeColor="Black" BorderStyle="Solid" BorderColor="Blue" BackColor="#FFFFC0"></SelectedDayStyle>
                                <TitleStyle ForeColor="White" CssClass="Header" BackColor="Navy"></TitleStyle>
                                <WeekendDayStyle CssClass="Alternating" BackColor="Lavender"></WeekendDayStyle>
                                <OtherMonthDayStyle ForeColor="Silver"></OtherMonthDayStyle>
                            </asp:Calendar>
                        </dyp:DynamicPanel>
                        <dyp:DynamicPanel ID="dypMessNote" runat="server" Width="100%" Height="100%">
                            <div class="vac-sezione" id="divWarning">Area messaggi</div>
                            <dyp:DynamicPanel ID="dypMsg" runat="server" Height="50%" ScrollBars="None" CssClass="corniceDati" Style="">
                                <asp:Label ID="lblWarning" runat="server" Width="100%" CssClass="Label_ErrorMsg" Height="100%"></asp:Label>
                            </dyp:DynamicPanel>
                            <div class="vac-sezione" id="divNoteAvvisi" runat="server">Note Avvisi</div>
                            <dyp:DynamicPanel ID="dypAvvisi" runat="server" Height="50%" CssClass="corniceDatiLast" Style="overflow: hidden;">
                                <asp:TextBox ID="txtNoteAvvisi" Style="width:98%; height:100%; border:0; overflow-y:auto;" 
                                    CssClass="textbox_stringa" runat="server" TextMode="MultiLine" MaxLength="4000"></asp:TextBox>                                
                            </dyp:DynamicPanel>
                        </dyp:DynamicPanel>
                    </dyp:DynamicPanel>
                </dyp:DynamicPanel>
            </dyp:DynamicPanel>

        </on_lay3:OnitLayout3>

        <on_ofm:OnitFinestraModale ID="fmMessaggioOrariApertura" Title="Orari di Apertura del Centro Vaccinale" runat="server"
            Width="450px" BackColor="LightGray" NoRenderX="True">
            <table width="100%" border="0">
                <tr height="40">
                    <td valign="middle">
                        <asp:Label ID="lblOrariApertura" runat="server" Width="430px" CssClass="label_center" Height="40px"
                            Font-Bold="True">Attenzione: gli Orari di Apertura del Centro Vaccinale non sono impostati: è necessario effettuare questa operazione per eseguire la Ricerca!</asp:Label></td>
                </tr>
                <tr height="30">
                    <td valign="middle">
                        <div align="center">
                            <asp:Button ID="btnOrariApertura" runat="server" Text="Chiudi"></asp:Button>
                        </div>
                    </td>
                </tr>
            </table>
        </on_ofm:OnitFinestraModale>

        <on_ofm:OnitFinestraModale ID="fmStampaBilancioBianco" Title="Stampa Bilancio in Bianco" runat="server" Width="450px" BackColor="LightGray" NoRenderX="True">
            <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="uwtBilancioBianco" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                <Items>
                    <igtbar:TBarButton Key="btnChiudi" Text="Chiudi" DisabledImage="~/Images/esci_dis.gif" Image="~/Images/esci.gif"></igtbar:TBarButton>
                    <igtbar:TBarButton Key="btnStampa" Text="Stampa" DisabledImage="~/Images/stampa_dis.gif"
                        Image="~/Images/stampa.gif">
                    </igtbar:TBarButton>
                </Items>
            </igtbar:UltraWebToolbar>
            <table height="80" width="100%" border="0">
                <tr>
                    <td valign="middle" align="right" width="30%" height="100%">
                        <asp:Label ID="lblNumBilancio" runat="server" Width="100px" CssClass="label_left">Numero Bilancio</asp:Label>
                    </td>
                    <td valign="middle" align="left" width="70%" height="100%">
                        <asp:DropDownList ID="ddlBilancio" runat="server" Width="90%" CssClass="textbox_stringa_obbligatorio"
                            DataValueField="BIL_NUMERO" DataTextField="BIL_DESCRIZIONE">
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </on_ofm:OnitFinestraModale>

        <asp:TextBox ID="txtStatoBtnAssegna" Style="display: none" runat="server" CssClass="Textbox_Stringa"></asp:TextBox>

        <on_ofm:OnitFinestraModale ID="fmError" Title="Attenzione" runat="server" Width="400px" BackColor="Gainsboro"
            NoRenderX="False" RenderModalNotVisible="True" ZIndexPosition="10000">
            <table id="Table6" cellspacing="0" cellpadding="0" border="0">
                <tr>
                    <td valign="bottom" align="center">
                        <table id="Table10" height="100%" cellspacing="0" cellpadding="0" width="100%" border="0">
                            <tr>
                                <td align="center" width="20%" height="60%">
                                    <br>
                                    <img id="Img1" src="../../Images/warning.gif" runat="server" alt="" />
                                </td>
                                <td align="center" width="80%" height="60%">
                                    <asp:Label ID="lblErrore" runat="server" Width="100%" CssClass="TextBox_Stringa" Font-Bold="True"></asp:Label></td>
                            </tr>
                            <tr>
                                <td colspan="2">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="center" width="20%" colspan="2" height="40%">
                                    <asp:Button ID="btnFmError" runat="server" Width="96" Height="24" Text="OK" style="cursor: pointer"></asp:Button></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </on_ofm:OnitFinestraModale>

        <on_ofm:OnitFinestraModale ID="fmMotivoNote" Title="Motivo Modifica Appuntamento" runat="server" NoRenderX="True" Width="480px" Height="240px"
            BackColor="LightGray" RenderModalNotVisible="True" >
            <table border="0" cellspacing="3" cellpadding="0" width="100%">
                <colgroup>
                    <col width="12%" />
                    <col width="33%" />
                    <col width="10%" />
                    <col width="40%" />
                    <col width="5%" />
                </colgroup>
                <tr>
                    <td colspan="5"></td>
                </tr>
                <tr>
                    <td class="label_right">Motivo</td>
                    <td colspan="4">
                        <asp:DropDownList ID="ddlMotivoModifica" runat="server" CssClass="TextBox_Stringa_Obbligatorio" Width="100%"></asp:DropDownList>
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td class="label_right">Note</td>
                    <td colspan="4">
                        <asp:TextBox ID="txtNoteModifica" runat="server" TextMode="MultiLine" MaxLength="100" 
                            Rows="7" Width="100%" CssClass="TextBox_Stringa"></asp:TextBox>
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td colspan="5">&nbsp;</td>
                </tr>
                <tr>
                    <td></td>
                    <td align="right">
                        <asp:Button id="btnMotivoModifica_Ok" runat="server" Width="80px" Text="Ok" style="cursor: pointer"></asp:Button>
                    </td>
                    <td></td>
                    <td colspan="2" align="left">
                        <asp:Button id="btnMotivoModifica_Annulla" runat="server" Width="80px" Text="Annulla" style="cursor: pointer"></asp:Button>
                    </td>
                    <td></td>
                </tr>
            </table>
        </on_ofm:OnitFinestraModale>

        <on_ofm:OnitFinestraModale ID="fmStoricoAppuntamenti" Title="Storico Appuntamento" runat="server" NoRenderX="False" Width="830px" Height="580px"
            BackColor="LightGray" RenderModalNotVisible="True" >
            
            <uc1:StoricoAppuntamenti id="ucStoricoAppuntamenti" runat="server" Visualizzazione="Ridotta"></uc1:StoricoAppuntamenti>

        </on_ofm:OnitFinestraModale>

        <script type="text/javascript" language="javascript">

            function rollover(obj, stato)
            {
                if (obj.disabled==false)
                    if (stato=='over')
                        obj.src=obj.src.split(".gif")[0]+ "_over.gif";
                    else
                        if (stato=='out')
                            obj.src=obj.src.split("_over.gif")[0]+ ".gif";
            }
			
            document.getElementById('textNuovaOra').onblur=function () {formattaOrario();};

            function formattaOrario()
            {
                document.getElementById('textNuovaOra').value=document.getElementById('textNuovaOra').value.replace(':','.');
            }

            //associa il click al pulsante della modale di errore [modifica 13/07/2005]
            if (document.getElementById('Button1')	!= null) 
                document.getElementById('Button1').onclick = ChiudiError;

            // Aggiorna la data impostata nel dataPicker di inizio periodo
            PreviousStartDate = OnitDataPickGet("odpDispInizio");
                   
            //function  resizeNote(s,e){
            //    var txtNote = document.getElementById("txtNoteAvvisi");
            //    if(txtNote!=null){
            //        txtNote.style.height=e.height-4;
            //        txtNote.style.width=e.width-8;
            //    }
            //} 
        </script>

    </form>

    <!-- #include file="../../Common/Scripts/AggiornaStatoVaccinazioni.inc" -->

</body>
</html>
