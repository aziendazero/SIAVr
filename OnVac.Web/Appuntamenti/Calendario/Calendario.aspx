<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Calendario.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Calendario" %>

<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<%@ Register TagPrefix="uc2" TagName="SelezioneAmbulatorio" Src="../../Common/Controls/SelezioneAmbulatorio.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ConsensoUtente" Src="../../Common/Controls/ConsensoTrattamentoDatiUtente.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Calendario</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <style type="text/css">
        .filtri-calendario {
            background-color: whitesmoke;
        }
    </style>

    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
    <script type="text/javascript" language="javascript">

        $(document).ready(function () {

            // Imposta l'altezza uguale nei fieldset di ricerca
            try {
                var max = 0;
                $(".fldroot").each(function () {
                    max = Math.max($(this).height(), max);
                }).height(max);

            } catch (e) {
                // 
            }
        });

        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function isValid_ScegliAmbulatorio(obj) {
			    /*
				    var uscSce = document.getElementById('<%= uscScegliAmb.ClientId %>');
            if (uscSce.validaTutti()) return true;
            else return false;

            alert(uscSce);

            if (uscSce.value != 0) return true;
            else return false;
			    */

            return true;
        }

        //controllo valore dei datepick
        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;

            switch (button.Key) {
                case 'btnCerca':
                    if ((!isValid_ScegliAmbulatorio('<%= uscScegliAmb.ClientId %>')) || OnitDataPickGet("txtData") == '') {
                        alert("Impostare tutti i filtri di ricerca!");
                        evnt.needPostBack = false;
                    }
                    break;
            }
        }

        function popitup(url) {
            newwindow = window.open(url, 'Consenso', 'top=0,left=0,width=750,height=550,menubar=0,resizable=1,scrollbars=1');
            if (window.focus) { newwindow.focus() }
            return false;
        }

        function RefreshFromPopup() {
            __doPostBack("RefreshFromPopup", "");
        }

    </script>

</head>
<body>
    <form id="Form1" method="post" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" TitleCssClass="Title3" Titolo="Appuntamenti del Giorno" Width="100%" Height="100%">
            <div class="title" id="PanelTitolo" runat="server">
                <asp:Label ID="LayoutTitolo" runat="server"> Appuntamenti del giorno </asp:Label>
            </div>
            <div>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBar" runat="server" CssClass="infratoolbar" ItemWidthDefault="90px">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                    <Items>
                        <igtbar:TBarButton Key="btnCerca" Text="Cerca" DisabledImage="~/Images/cerca.gif" Image="~/Images/cerca.gif">
                        </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btnPulisci" Text="Pulisci" DisabledImage="~/Images/pulisci_dis.gif" Image="~/Images/pulisci.gif">
                        </igtbar:TBarButton>
                        <igtbar:TBSeparator />
                        <igtbar:TBarButton Key="btnStampa" Text="Stampa appuntamenti" DisabledImage="~/Images/stampa.gif" Image="~/Images/stampa.gif">
                            <DefaultStyle Width="180px" CssClass="infratoolbar_button_default"></DefaultStyle>
                        </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btnStampaEtichette" Text="Stampa etichette assistiti" DisabledImage="~/Images/stampa.gif" Image="~/Images/stampa.gif">
                            <DefaultStyle Width="180px" CssClass="infratoolbar_button_default"></DefaultStyle>
                        </igtbar:TBarButton>
                    </Items>
                </igtbar:UltraWebToolbar>
            </div>
            <div class="vac-sezione" id="Panel23" runat="server">
                <asp:Label ID="LayoutTitolo_sezioneRicerca" runat="server">Filtri</asp:Label>
            </div>
            <div class="filtri-calendario">
                <table id="Table1" style="width: 100%;" cellspacing="1" cellpadding="1">
                    <tr>
                        <td>
                            <fieldset class="fldroot" id="fldConsultorio" title="Centro Vaccinale" style="height: 100%; padding: 0px">
                                <legend class="label_left">Centro Vaccinale</legend>
                                <uc2:SelezioneAmbulatorio ID="uscScegliAmb" runat="server" Tutti="True" />
                            </fieldset>
                        </td>
                        <td width="2px"></td>
                        <td width="120px">
                            <fieldset class="fldroot" id="fldData" title="Data" style="height: 100%; padding: 0px">
                                <legend class="label_left">Data</legend>
                                <table id="tblData" height="60%" cellspacing="0" cellpadding="3" width="100%" border="0">
                                    <tr>
                                        <td valign="middle" align="center">
                                            <on_val:OnitDatePick ID="txtData" runat="server" Height="20px" CssClass="textbox_stringa_obbligatorio"
                                                DateBox="True" Width="110px"></on_val:OnitDatePick>
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                        </td>
                        <td width="2px"></td>
                        <td width="140px">
                            <fieldset class="fldroot" id="fldLegenda" title="Data" style="height: 100%; padding: 0px">
                                <legend class="label_left">Legenda</legend>
                                <table class="DataGrid" cellspacing="0" cellpadding="2" width="100%">
                                    <tr>
                                        <td><span class="legenda-appuntamenti-E">E</span></td>
                                        <td>Immigrato extracomunitario</td>
                                    </tr>
                                    <tr>
                                        <td><span class="legenda-appuntamenti-I">I</span></td>
                                        <td>Immigrato non extr. prima volta</td>
                                    </tr>
                                    <tr>
                                        <td><span class="legenda-appuntamenti-C">C</span></td>
                                        <td>Cronico</td>
                                    </tr>
                                </table>
                            </fieldset>
                        </td>
                        <td width="2px"></td>
                        <td runat="server" id="cellLegendaConsenso" width="220px">
                            <fieldset class="fldroot" id="fldLegendaConsenso" title="Consenso" style="width: 250px; height: 100%; padding: 0px">
                                <legend class="label_left">Consenso</legend>
                                <asp:Repeater ID="rtrConsenso" runat="server">
                                    <HeaderTemplate>
                                        <table border="0" cellpadding="1" cellspacing="0">
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td class="label" width="25px">
                                                <img src='<%# ResolveClientUrl(Eval("Url")) %>' title='<%# Eval("Descrizione") %>' alt="" />
                                            </td>
                                            <td class="label_left">
                                                <%# Eval("Descrizione") %>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </fieldset>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="vac-sezione" id="Div13" runat="server">
                <asp:Label ID="Label2" runat="server">Risultati ricerca</asp:Label>
            </div>

            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">

                <asp:DataList ID="dlsAppuntamenti" runat="server" CssClass="DataGrid" Width="100%">
                    <SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
                    <HeaderTemplate>
                        <table width="100%">
                            <tr>
                                <td width="2%">&nbsp;</td>
                                <td width="2%" id="headerConsensi" runat="server" align="center">Cons</td>
                                <td width="3%">
                                    <asp:LinkButton ID="lnkSortOra" runat="server" Text="Ora" CommandName="SortOra" ToolTip="Ordina l'elenco degli appuntamenti per l'ora di appuntamento"></asp:LinkButton></td>
                                <td width="15%">
                                    <asp:LinkButton ID="LinkSortAmb" runat="server" Text="Amb" CommandName="SortAmb" ToolTip="Ordina l'elenco degli appuntamenti per l'ambulatorio di appuntamento"></asp:LinkButton></td>
                                <td width="25%">
                                    <asp:LinkButton ID="lnkSortCognomeNome" runat="server" Text="Cognome e nome" CommandName="SortCognomeNome"
                                        ToolTip="Ordina l'elenco degli appuntamenti per cognome e nome"></asp:LinkButton></td>
                                <td width="10%">
                                    <asp:LinkButton ID="lnkSortDataNascita" runat="server" Text="Data di nascita" CommandName="SortDataNascita"
                                        ToolTip="Ordina l'elenco degli appuntamenti per la data di nascita"></asp:LinkButton></td>
                                <td width="25%">
                                    <asp:LinkButton ID="lnkVaccinazioni" runat="server" Text="Vaccinazioni" CommandName="SortVaccinazioni"
                                        ToolTip="Ordina l'elenco degli appuntamenti in base alle vaccinazioni"></asp:LinkButton></td>
                                <td width="4%">
                                    <asp:LinkButton ID="lnkDose" runat="server" Text="Dose" CommandName="SortDosi" ToolTip="Ordina l'elenco degli appuntamenti in base alla dose"></asp:LinkButton></td>
                                <td width="4%">
                                    <asp:LinkButton ID="lnkDurata" runat="server" Text="Durata" CommandName="SortDurata" ToolTip="Ordina l'elenco degli appuntamenti in base alla durata"></asp:LinkButton></td>
                                 <td width="4%">
                                    <asp:LinkButton ID="lnkIdApp" runat="server" Text="Id App" CommandName="SortIdApp" ToolTip="Ordina l'elenco degli appuntamenti per Id Appuntamento"></asp:LinkButton></td>
                                <td width="2%">&nbsp;</td>
                                <td width="2%">&nbsp;</td>
                                <td width="2%">&nbsp;</td>
                            </tr>
                        </table>
                    </HeaderTemplate>
                    <EditItemStyle CssClass="Edit"></EditItemStyle>
                    <AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
                    <ItemStyle CssClass="Item"></ItemStyle>
                    <ItemTemplate>
                        <table width="100%">
                            <tr>
                                <td style="width: 2%">
                                    <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl='<%# Me.ResolveClientUrl("~/images/sel.gif") %>' CommandName="Seleziona" ToolTip="Mostra vaccinazioni programmate"></asp:ImageButton>
                                </td>
                                <td style="width: 2%" runat="server" id="itemConsenso" align="center">
                                    <asp:ImageButton ID="imgBtnConsenso" runat="server" CommandName="ApriRilevazioneConsenso"
                                        ToolTip="Stato Consenso. Click per aprire il programma di rilevazione del consenso." />
                                </td>
                                <td style="width: 3%">
                                    <asp:Label ID="Label1" runat="server" Text='<%# Convert.ToDateTime(Container.DataItem("ORA")).ToString("HH.mm") %>'>
                                    </asp:Label></td>
                                <td style="width: 15%" align="left">
                                    <asp:Label ID="lblAmb" runat="server" Text='<%# Container.DataItem("AMB_DESCRIZIONE") %>'>
                                    </asp:Label></td>
                                <asp:Label ID="lblCodAmb" Style="display: none" runat="server" Text='<%# Container.DataItem("CNV_AMB_CODICE") %>'>
                                </asp:Label></td>
									<td style="width: 25%">
                                        <asp:LinkButton ID="lblCognomeNome" runat="server" Text='<%# Container.DataItem("NOME") %>' CommandName="Nome" ToolTip="Visualizza dati paziente">
                                        </asp:LinkButton>
                                        <asp:Label ID="lblCodicePaz" Style="display: none" runat="server" Text='<%# Container.DataItem("PAZ_CODICE") %>' />
                                        <asp:Label ID="lblCodiceAusiliario" Style="display: none" runat="server" Text='<%# Container.DataItem("PAZ_CODICE_AUSILIARIO") %>' />
                                    </td>
                                <td style="width: 10%">
                                    <asp:Label ID="lblDataNascita" runat="server" Text='<%# Convert.ToDateTime(Container.DataItem("PAZ_DATA_NASCITA")).ToString("dd/MM/yyyy") %>'>
                                    </asp:Label></td>
                                <td style="width: 25%">
                                    <asp:Label ID="lblCicli" runat="server" Text='<%# Container.DataItem("VACCINAZIONI") %>'>
                                    </asp:Label></td>
                                <td style="width: 4%">
                                    <asp:Label ID="lblSeduta" runat="server" Text='<%# Container.DataItem("DOSE") %>'>
                                    </asp:Label></td>
                                <td style="width: 4%">
                                    <asp:Label ID="lblDurata" runat="server" Text='<%# Container.DataItem("CNV_DURATA_APPUNTAMENTO") %>'>
                                    </asp:Label></td>
                                <td style="width: 4%">
                                    <asp:Label ID="lblIdApp" runat="server" Text='<%# Container.DataItem("CNV_ID_CONVOCAZIONE") %>'>
                                    </asp:Label></td>
                                <td style="width: 2%">
                                    <asp:Label ID="lblEmigrato" runat="server" CssClass="legenda-appuntamenti-E" Text='<%# Eval("TIPO_EXTRACOMUNITARI") %>'
                                        Visible='<%# Eval("TIPO_EXTRACOMUNITARI") IsNot DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(Eval("TIPO_EXTRACOMUNITARI").ToString()) %>'>
                                    </asp:Label></td>
                                <td style="width: 2%">
                                    <asp:Label ID="lblImmigrato" runat="server" CssClass="legenda-appuntamenti-I" Text='<%# Eval("TIPO_IMMI_NON_EXTRA_PRIMA") %>'
                                        Visible='<%# Eval("TIPO_IMMI_NON_EXTRA_PRIMA") IsNot DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(Eval("TIPO_IMMI_NON_EXTRA_PRIMA").ToString()) %>'>
                                    </asp:Label></td>
                                <td style="width: 2%">
                                    <asp:Label ID="lblCronico" runat="server" CssClass="legenda-appuntamenti-C" Text='<%# Eval("CRONICO") %>'
                                        Visible='<%# Eval("CRONICO") IsNot DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(Eval("CRONICO").ToString()) %>'>
                                    </asp:Label></td>
                            </tr>
                        </table>
                    </ItemTemplate>
                    <HeaderStyle CssClass="Header"></HeaderStyle>
                </asp:DataList>

                <!--
                        <iframe id="modal" src="../../../../includes/formdettaglio/FinestraModaleDettaglio.aspx?cn=&SQL=SELECT CNS_DESCRIZIONE, CNS_CODICE FROM T_ANA_CONSULTORI" style="background-color:white;BORDER-RIGHT:2px outset;BORDER-TOP:2px outset;LEFT:50px;OVERFLOW:AUTO;BORDER-LEFT:2px outset;BORDER-BOTTOM:2px outset;POSITION:absolute;TOP:50px;width:400px;height:500px">
                        </iframe>
                        <div id="modal">
                        Immetti il nome e la password <input tye=text>
                        <div>
                    -->
            </dyp:DynamicPanel>
        </on_lay3:OnitLayout3>

        <!-- modale consenso -->
        <on_ofm:OnitFinestraModale ID="modConsenso" Title="Consenso" runat="server" Width="800px" Height="600px" BackColor="LightGray" ClientEventProcs-OnClose="RefreshFromPopup()">
            <iframe id="frameConsenso" runat="server" class="frameConsensoStyle">
                <div>
                    Caricamento in corso. Attendere...
                </div>
            </iframe>
        </on_ofm:OnitFinestraModale>

        <!-- consenso trattamento dati utente -->
        <on_ofm:OnitFinestraModale ID="fmConsensoUtente" Title="Consenso al trattamento dati per l'utente" runat="server" Width="400px" Height="250px" BackColor="LightGoldenrodYellow">
            <uc1:ConsensoUtente runat="server" id="ucConsensoUtente"></uc1:ConsensoUtente>
        </on_ofm:OnitFinestraModale>

    </form>

    <script type="text/javascript" language="javascript">
        //<!--
        //createModalDialog('Prova.htm',400,500);
        //readParameter('');
        //closeModalDialog();
        //-->

        function OnitDataPick_ClickDay(id) {
            if (id == 'txtData' && (isValid_ScegliAmbulatorio('<%= uscScegliAmb.ClientId %>'))) {
                __doPostBack('Ricerca', '');
            }
        }
    </script>

</body>

</html>
