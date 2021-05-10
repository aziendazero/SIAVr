<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="LogRicezioneACN.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.LogRicezioneACN" ValidateRequest="false" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Monitor Integrazione ACN</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' rel="stylesheet" type="text/css" />

    <style type="text/css">
        .filtroDati {
            margin-top: 5px;
            margin-bottom: 5px;
            width: 99%;
            border-width: 0px;
        }

        .vac-sezione {
            padding-top: 3px;
            padding-bottom: 1px;
        }

        .clickable {
            cursor: pointer;
        }

        .xmlstyle {
            font-family: 'Courier New';
            font-size: 12px;
            overflow: auto;
        }

        .div-dettaglio {
            padding: 2px 10px 5px 10px;
        }
    </style>

    <script type="text/javascript">
        function InizializzaToolBar(t) {
            t.PostBackButton = true;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;

            switch (button.Key) {
                case 'btnRielabora':

                    if (!confirm('ATTENZIONE: verrà rielaborato l\'invio della vaccinazione\i. L\'operazione non e\' annullabile. Continuare?')) {
                        evnt.needPostBack = false;
                    }
					break;
            }
		}
		function ToolBarMainClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;

            switch (button.Key) {
               
				case 'btnRielaboraMassivo':

                    if (!confirm('ATTENZIONE: verrà rielaborato l\'invio della vaccinazione\i. L\'operazione non e\' annullabile. Continuare?')) {
                        evnt.needPostBack = false;
                    }
                    break;
            }
        }


        function ImpostaImmagineOrdinamento(imgId, imgUrl) {
            var img = document.getElementById(imgId);
            if (img != null) {
                img.style.display = 'inline';
                img.src = imgUrl;
            }
        }

        function setVisibility() {

            // Filtri
            var div = document.getElementById('divFiltri');
            var hid = document.getElementById('hidFiltriVisibility');

            if (div != null && hid != null) {
                div.style.display = hid.value;
            }

            resizeAllDynamicPanels();
        }

        function toggleVisibility(idDiv, idHid) {
            var div = document.getElementById(idDiv);
            var hid = document.getElementById(idHid);

            if (div != null && hid != null) {
                if (hid.value == 'block') {
                    hid.value = 'none';
                }
                else {
                    hid.value = 'block';
                }

                div.style.display = hid.value;
            }

            resizeAllDynamicPanels();
        }
    </script>
</head>
<body onload="setVisibility()">
    <form id="form1" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Height="100%" Width="100%" Titolo="Monitor integrazione anagrafica">
            <div class="Title" id="PanelTitolo" runat="server">
                <asp:Label ID="LayoutTitolo" runat="server">Monitor integrazione ACN</asp:Label>
            </div>
            <asp:MultiView ID="mainView" runat="server" ActiveViewIndex="0">
                <asp:View ID="viewRicerca" runat="server">
                    <div>
                        <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBarMain" runat="server" ItemWidthDefault="90px" CssClass="infratoolbar">
                            <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                            <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                            <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarMainClick"></ClientSideEvents>
                            <Items>
                                <igtbar:TBarButton Key="btnCerca" Text="Cerca" DisabledImage="~/Images/cerca_dis.gif" Image="~/Images/cerca.gif"
                                    ToolTip="Effettua la ricerca in base filtri impostati">
                                </igtbar:TBarButton>
                                <igtbar:TBarButton Key="btnPulisci" Text="Pulisci" ToolTip="Resetta i filtri impostati ai valori di default"
                                    DisabledImage="../../images/eraser.png" Image="../../images/eraser.png">
                                </igtbar:TBarButton>
								<igtbar:TBarButton Key="btnRielaboraMassivo" Text="Rielabora" ToolTip="Rielabora i messaggi falliti" Visible="True"
                                    Image="../../images/rotella.png">
                                </igtbar:TBarButton>

                            </Items>
                        </igtbar:UltraWebToolbar>
                    </div>

                    <div class="vac-sezione clickable" onclick="toggleVisibility('divFiltri', 'hidFiltriVisibility');" title="Mostra/nasconde i filtri di ricerca">Filtri di ricerca</div>
                    <div id="divFiltri" style="display: block; width: 100%">
                        <asp:HiddenField ID="hidFiltriVisibility" runat="server" Value="block" />
                        <table style="width: 100%; background-color: whitesmoke; text-align: center;">
                            <tr>
                                <td>
                                    <fieldset class="vacFieldset" title="Filtri sui messaggi">
                                        <legend class="label">Filtri sui messaggi</legend>
                                        <table class="filtroDati">
                                            <colgroup>
                                                <col style="width: 15%" />
                                                <col style="width: 35%" />
                                                <col style="width: 12%" />
                                                <col style="width: 16%" />
                                                <col style="width: 6%" />
                                                <col style="width: 16%" />
                                            </colgroup>
                                            <tr>
												 <td class="label">
                                                    <asp:Label ID="lblId" runat="server" Text="Id"></asp:Label></td>
                                                <td>
                                                    <asp:TextBox ID="txtId" runat="server" CssClass="TextBox_Stringa" Style="width: 100%"></asp:TextBox></td>
                                                <td class="label">
                                                    <asp:Label ID="lblDataDa" runat="server" Text="Ricevuti dal"></asp:Label></td>
                                                <td>
                                                    <on_val:OnitDatePick ID="dpkRicezioneDa" runat="server" Height="20px" Width="120px" CssClass="textbox_data" DateBox="True"></on_val:OnitDatePick>
                                                </td>
                                                <td class="label">
                                                    <asp:Label ID="lblDataA" runat="server" Text="al"></asp:Label></td>
                                                <td>
                                                    <on_val:OnitDatePick ID="dpkRicezioneA" runat="server" Height="20px" Width="120px" CssClass="textbox_data" DateBox="True"></on_val:OnitDatePick>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="label">
                                                    <asp:Label ID="lblOperazione" runat="server" Text="Operazione"></asp:Label></td>
                                                <td>
                                                    <asp:DropDownList ID="ddlOperazione" runat="server" CssClass="TextBox_Stringa" Style="width: 100%"></asp:DropDownList></td>
                                                <td class="label">
                                                    <asp:Label ID="lblRisultato" runat="server" Text="Risultato"></asp:Label></td>
                                                <td colspan="3">
                                                    <asp:DropDownList ID="ddlRisultato" runat="server" CssClass="TextBox_Stringa" Style="width: 100%"></asp:DropDownList></td>
                                            </tr>
                                        </table>
                                    </fieldset>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <fieldset class="vacFieldset" title="Filtri sui pazienti">
                                        <legend class="label">Filtri sui pazienti</legend>
                                        <table class="filtroDati">
                                            <colgroup>
                                                <col style="width: 15%" />
                                                <col style="width: 35%" />
                                                <col style="width: 12%" />
                                                <col style="width: 16%" />
                                                <col style="width: 6%" />
                                                <col style="width: 16%" />
                                            </colgroup>
                                            <tr>
                                                <td class="label">
                                                    <asp:Label ID="lblCodiceCentrale" runat="server" Text="Codice Centrale"></asp:Label></td>
                                                <td>
                                                    <asp:TextBox ID="txtCodiceCentrale" runat="server" CssClass="TextBox_Stringa" Style="width: 100%"></asp:TextBox></td>
                                                <td class="label">
                                                    <asp:Label ID="lblCodiceLocale" runat="server" Text="Codice"></asp:Label></td>
                                                <td colspan="3">
                                                    <asp:TextBox ID="txtCodiceLocale" runat="server" CssClass="TextBox_Stringa" Style="width: 100%"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td class="label">
                                                    <asp:Label ID="lblCodiceFiscale" runat="server" Text="Codice fiscale"></asp:Label></td>
                                                <td>
                                                    <asp:TextBox ID="txtCodiceFiscale" runat="server" CssClass="TextBox_Stringa" Style="width: 100%"></asp:TextBox></td>
                                                <td class="label">
                                                    <asp:Label ID="lblDataNascitaDa" runat="server" Text="Nati da"></asp:Label></td>
                                                <td>
                                                    <on_val:OnitDatePick ID="dpkNascitaDa" runat="server" Height="20px" Width="120px" CssClass="textbox_data" DateBox="True"></on_val:OnitDatePick>
                                                </td>
                                                <td class="label">
                                                    <asp:Label ID="lblDataNascitaA" runat="server" Text="A"></asp:Label></td>
                                                <td>
                                                    <on_val:OnitDatePick ID="dpkNascitaA" runat="server" Height="20px" Width="120px" CssClass="textbox_data" DateBox="True"></on_val:OnitDatePick>
                                                </td>
                                            </tr>
                                        </table>
                                    </fieldset>
                                </td>
                            </tr>
                        </table>
                    </div>

                    <div class="vac-sezione">Elenco messaggi ricevuti</div>

                    <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                        <asp:DataGrid ID="dgrLogNotifiche" runat="server" Width="100%" AutoGenerateColumns="False" AllowCustomPaging="true"
                            AllowPaging="true" PageSize="25" AllowSorting="True">
                            <HeaderStyle CssClass="header"></HeaderStyle>
                            <ItemStyle CssClass="item"></ItemStyle>
                            <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
                            <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
                            <PagerStyle CssClass="pager" Position="TopAndBottom" Mode="NumericPages" />
                            <Columns>
								<asp:TemplateColumn>
								<HeaderStyle Width="1%" HorizontalAlign="Center" />
								<ItemStyle HorizontalAlign="Center" />
									<ItemTemplate>
										<asp:CheckBox ID="chkSelezioneItem" runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
                                <on_dgr:SelectorColumn>
                                    <ItemStyle HorizontalAlign="Center" Width="1%" />
                                </on_dgr:SelectorColumn>
                                <asp:BoundColumn DataField="IdMessaggio" HeaderText="Id <img id='imgIdMsg' alt='' src='../../images/transparent16.gif' />"
                                    SortExpression="IdMessaggio" HeaderStyle-Width="12%"></asp:BoundColumn>
                                <asp:TemplateColumn HeaderText="Operazione <img id='imgOp' alt='' src='../../images/transparent16.gif' />"
                                    SortExpression="Operazione" HeaderStyle-Width="9%">
                                    <ItemTemplate>
                                        <asp:Label ID="lblOperazione" CssClass="label_left" runat="server"
                                            Text='<%# GetStringOperazioneFromEnumValue(Eval("Operazione"))%>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Data Ricezione <img id='imgDataRicez' alt='' src='../../images/transparent16.gif' />"
                                    SortExpression="DataRicezione" HeaderStyle-Width="9%">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDataRicez" CssClass="label_left" runat="server"
                                            Text='<%# ConvertToDateString(Eval("DataRicezione"), True)%>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:BoundColumn DataField="CodiceCentralePaziente" HeaderText="Codice Centrale Paziente <img id='imgMpiPaz' alt='' src='../../images/transparent16.gif' />"
                                    SortExpression="CodiceCentralePaziente" HeaderStyle-Width="12%"></asp:BoundColumn>
                                <asp:BoundColumn DataField="CodiceRegionale" HeaderText="Codice Regionale <img id='imgMpiAlias' alt='' src='../../images/transparent16.gif' />"
                                    SortExpression="CodiceRegionale" HeaderStyle-Width="12%"></asp:BoundColumn>
                                <asp:BoundColumn DataField="CodiceLocalePaziente" HeaderText="Codice Paziente <img id='imgCodPaz' alt='' src='../../images/transparent16.gif' />"
                                    SortExpression="CodiceLocalePaziente" HeaderStyle-Width="9%"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Cognome" HeaderText="Cognome <img id='imgCognome' alt='' src='../../images/transparent16.gif' />"
                                    SortExpression="Cognome" HeaderStyle-Width="12%"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Nome" HeaderText="Nome <img id='imgNome' alt='' src='../../images/transparent16.gif' />"
                                    SortExpression="Nome" HeaderStyle-Width="12%"></asp:BoundColumn>
                                <asp:TemplateColumn HeaderText="Data Nascita <img id='imgDataNas' alt='' src='../../images/transparent16.gif' />"
                                    SortExpression="DataNascita" HeaderStyle-Width="7%">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDataNas" CssClass="label_left" runat="server"
                                            Text='<%# ConvertToDateString(Eval("DataNascita"), False)%>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:BoundColumn DataField="CodiceFiscale" HeaderText="Codice Fiscale <img id='imgNome' alt='' src='../../images/transparent16.gif' />"
                                    SortExpression="CodiceFiscale" HeaderStyle-Width="12%"></asp:BoundColumn>
                                <asp:TemplateColumn HeaderText="Risultato <img id='imgRisultato' alt='' src='../../images/transparent16.gif' />"
                                    SortExpression="RisultatoElaborazione" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Image ID="imgRisultato" runat="server"
                                            ImageUrl='<%# GetUrlIconaRisultato(Eval("RisultatoElaborazione"))%>'
                                            ToolTip='<%# GetDescrizioneRisultato(Eval("RisultatoElaborazione"), Eval("MessaggioElaborazione"))%>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:BoundColumn DataField="IdNotifica" SortExpression="IdNotifica" Visible="false"></asp:BoundColumn>
								<asp:BoundColumn DataField="RisultatoElaborazione" SortExpression="RisultatoElaborazione" Visible="false"></asp:BoundColumn>
                            </Columns>
                        </asp:DataGrid>
                    </dyp:DynamicPanel>
                </asp:View>

                <asp:View ID="viewDettaglio" runat="server">
                    <div>
                        <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolbarDetail" runat="server" ItemWidthDefault="90px" CssClass="infratoolbar">
                            <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                            <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                            <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                            <Items>
                                <igtbar:TBarButton Key="btnIndietro" Text="Indietro" DisabledImage="../../images/prev_dis.gif" Image="../../images/prev.gif"
                                    ToolTip="Torna alla ricerca">
                                </igtbar:TBarButton>
                                <igtbar:TBarButton Key="btnRielabora" Text="Rielabora" ToolTip="Rielabora i messaggi falliti" Visible="false"
                                    Image="../../images/rotella.png">
                                </igtbar:TBarButton>
                            </Items>
                        </igtbar:UltraWebToolbar>
                    </div>

                    <div class="vac-sezione">
                        <asp:Label ID="lblCaptionDettaglio" runat="server"></asp:Label></div>

                    <dyp:DynamicPanel ID="dypScroll2" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true" BackColor="whitesmoke">

                        <div class="div-dettaglio">
                            <table>
                                <tr>
                                    <td class="label_left">Risultato</td>
                                    <td>
                                        <asp:Image ID="imgRisult" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div class="div-dettaglio">
                            <asp:TextBox ID="txtRisult" ReadOnly="true" runat="server" CssClass="TextBox_Stringa" Width="100%" TextMode="MultiLine" Rows="4"></asp:TextBox>
                        </div>

                        <div class="div-dettaglio">
                            <fieldset class="vacFieldset" title="Filtri sui messaggi" style="width: 100%">
                                <legend class="label">Paziente</legend>
                                <table width="100%">
                                    <colgroup>
                                        <col width="18%" />
                                        <col width="32%" />
                                        <col width="18%" />
                                        <col width="32%" />
                                    </colgroup>
                                    <tr>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblCodiceMpi" runat="server" CssClass="label" Text="Codice Centrale"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCodiceMpi" runat="server" CssClass="TextBox_Stringa" ReadOnly="true" Width="100%"></asp:TextBox></td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblCodiceMpiAlias" runat="server" CssClass="label" Text="Codice Regionale"></asp:Label></td>
                                        <td>
                                            <asp:TextBox ID="txtCodiceMpiAlias" runat="server" CssClass="TextBox_Stringa" ReadOnly="true" Width="100%"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblCodicePaziente" runat="server" CssClass="label" Text="Codice Paziente"></asp:Label></td>
                                        <td>
                                            <asp:TextBox ID="txtCodicePaziente" runat="server" CssClass="TextBox_Stringa" ReadOnly="true" Width="100%"></asp:TextBox></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblCognome" runat="server" CssClass="label" Text="Cognome"></asp:Label></td>
                                        <td>
                                            <asp:TextBox ID="txtCognome" runat="server" CssClass="TextBox_Stringa" ReadOnly="true" Width="100%"></asp:TextBox></td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblNome" runat="server" CssClass="label" Text="Nome"></asp:Label></td>
                                        <td>
                                            <asp:TextBox ID="txtNome" runat="server" CssClass="TextBox_Stringa" ReadOnly="true" Width="100%"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblDataNascita" runat="server" CssClass="label" Text="Data di nascita"></asp:Label></td>
                                        <td>
                                            <asp:TextBox ID="txtDataNascita" runat="server" CssClass="TextBox_Stringa" ReadOnly="true" Width="100%"></asp:TextBox></td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblCF" runat="server" CssClass="label" Text="Codice Fiscale"></asp:Label></td>
                                        <td>
                                            <asp:TextBox ID="txtCF" runat="server" CssClass="TextBox_Stringa" ReadOnly="true" Width="100%"></asp:TextBox></td>
                                    </tr>
                                </table>
                            </fieldset>
                        </div>
                        <div class="div-dettaglio">
                            <fieldset class="vacFieldset" title="Filtri sui messaggi" style="width: 100%">
                                <legend class="label">Messaggio</legend>
                                <table width="100%">
                                    <colgroup>
                                        <col width="18%" />
                                        <col width="32%" />
                                        <col width="18%" />
                                        <col width="32%" />
                                    </colgroup>
                                    <tr>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblIdMessaggio" runat="server" CssClass="label">Id Messaggio</asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtIdMessaggio" runat="server" CssClass="TextBox_Stringa" ReadOnly="true" Width="100%"></asp:TextBox>

                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblOperaz" runat="server" CssClass="label" Text="Operazione"></asp:Label></td>
                                        <td>
                                            <asp:TextBox ID="txtOpeaz" ReadOnly="true" runat="server" CssClass="TextBox_Stringa" Width="100%"></asp:TextBox></td>


                                    </tr>
                                    <tr>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblDataRicezione" runat="server" CssClass="label">Data ricezione</asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtDataRicezione" runat="server" CssClass="TextBox_Stringa" ReadOnly="true" Width="100%"></asp:TextBox>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblDataInvio" runat="server" CssClass="label">Data invio risposta</asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtDataInvio" runat="server" CssClass="TextBox_Stringa" ReadOnly="true" Width="100%"></asp:TextBox>

                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblNumeroRicezione" runat="server" CssClass="label">Numero ricezioni</asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtNumeroRicezione" runat="server" CssClass="TextBox_Stringa" ReadOnly="true" Width="100%"></asp:TextBox>
                                        </td>

                                    </tr>
                                    <tr>
                                        <td valign="top" style="text-align: right">
                                            <asp:Label ID="lblMessaggioCompleto" runat="server" CssClass="label">Messaggio completo</asp:Label></td>
                                        <td colspan="3">
                                            <asp:TextBox ID="txtMessaggioCompleto" ReadOnly="true" runat="server" CssClass="TextBox_Stringa xmlstyle"
                                                Height="200px" Width="100%" TextMode="MultiLine"></asp:TextBox>
                                        </td>
                                    </tr>
									<tr>
                                        <td valign="top" style="text-align: right">
                                            <asp:Label ID="lblEntity" runat="server" CssClass="label">Messaggio entity</asp:Label></td>
                                        <td colspan="3">
                                            <asp:TextBox ID="txtEntity" ReadOnly="true" runat="server" CssClass="TextBox_Stringa xmlstyle"
                                                Height="200px" Width="100%" TextMode="MultiLine"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                        </div>
                    </dyp:DynamicPanel>
                </asp:View>
            </asp:MultiView>
        </on_lay3:OnitLayout3>
    </form>
</body>
</html>
