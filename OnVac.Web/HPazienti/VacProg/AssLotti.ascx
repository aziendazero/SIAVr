<%@ Control Language="vb" AutoEventWireup="false" Codebehind="AssLotti.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_AssLotti" %>

<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>

<%@ Register TagPrefix="uc1" TagName="AttivazioneLotto" Src="../../Common/Controls/AttivazioneLotto.ascx" %>

<script type="text/javascript">
    var dg_lotti='<%= Me.dg_lotti.ClientId %>';
	
    function InizializzaToolBar_insAss(t) {
        t.PostBackButton = false;
    }
	
    function ToolBarClick_insAss(ToolBar, button, evnt) {
        evnt.needPostBack = true;
        switch (button.Key) {
            case 'btn_Annulla':
                closeFm('<%= ModaleName %>');
                //riabilitazione di left e top frame [modifica 25/07/2005]
                OnitLayoutStatoMenu(false);
                evnt.needPostBack = false;
                break;
            default:
                evnt.needPostBack = true;
        }
    }

    //chiude la modale con il messaggio di errore [modifica 08/07/2005]
    function ChiudiMessaggioErrore() {
        closeFm('<%= fmMessaggioErrore.clientid() %>');
        SetFocusTbCodLottoLCB()
    }

    function clickPassword(evt) {
        if (evt.keyCode == 13) {
            var btn = document.getElementById('<%= btnOKPassword.ClientID %>');
            if (btn != null) {
                btn.click();
            }
            else {
                evt.preventDefault();
                return false;
            }
        }
    }
</script>

<table height="300" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr height="50">
		<td colspan="2">
            <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
	            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
				<ClientSideEvents InitializeToolbar="InizializzaToolBar_insAss" Click="ToolBarClick_insAss"></ClientSideEvents>
				<Items>
					<igtbar:TBarButton Key="btn_Conferma" DisabledImage="~/Images/conferma_dis.gif" Text="Conferma"
						Image="~/Images/conferma.gif"></igtbar:TBarButton>
				</Items>
			</igtbar:UltraWebToolbar>
            <asp:panel id="PanelLayoutTitolo_sezione" runat="server" CssClass="Sezione" Style="height:28px">
				<table class="label_left" width="100%" border="0">
					<tr>
						<td class="label_left">
							<asp:Label id="LayoutTitolo_sezione" runat="server">ELENCO LOTTI</asp:Label></td>
						<td style="text-align:right">
							<asp:checkbox id="chkLottiFuoriEta" runat="server" Text="Mostra anche lotti fuori età" CssClass="label"
								AutoPostBack="True"></asp:checkbox></td>
					</tr>
				</table>
			</asp:panel>
        </td>
	</tr>
	<tr>
		<td colspan="2" style="vertical-align: middle"> 
			<div class="label_center" id="divPassword" style="border: navy 1px solid; display: none; margin: 2px; background-color: lightyellow; text-align: center;"
				runat="server" onkeypress="clickPassword(event)">
				<p class="label_center" style="font-weight: bold; color: red">
                    L'opzione selezionata è disponibile solo dopo aver inserito nuovamente la password.</p>
				<p class="label_center">Password:
					<asp:textbox id="txtPassword" runat="server" TextMode="Password"></asp:textbox></p>
				<p class="label_center">
                    <asp:button id="btnOKPassword" runat="server" Text="OK" Width="100px" style="cursor:pointer"></asp:button>
					<asp:button id="btnAnullaPassword" runat="server" Text="Annulla" Width="100px" style="margin-left:30px; cursor:pointer"></asp:button>
                </p>
			</div>
		</td>
	</tr>
	<tr>
		<td colspan="2"></td>
	</tr>
	<tr>
		<td valign="middle" align="center" height="0">
            <asp:label id="lblCodLottoLCB" runat="server" CssClass="label">Lotto Inserito</asp:label>&nbsp;
			<asp:textbox id="tbCodLottoLCB" onkeydown="checkKey(event);window.clearInterval(intID);intID=window.setInterval('DoBlurTbCodLottoLCB()',100)" 
                runat="server"></asp:textbox>&nbsp;
			<asp:checkbox id="chkPenna" runat="server" CssClass="label_left" Text="Penna?" Checked="True"></asp:checkbox>
        </td>
		<td></td>
	</tr>
	<tr height="250">
		<td>
			<div style="overflow: auto; width: 100%; height: 250px">
                <asp:datagrid id="dg_lotti" runat="server" GridLines="None" CellPadding="1" AutoGenerateColumns="False" Width="100%">
					<SelectedItemStyle Font-Bold="True" CssClass="selected"></SelectedItemStyle>
					<AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
					<ItemStyle CssClass="item"></ItemStyle>
					<HeaderStyle CssClass="header"></HeaderStyle>
                    <PagerStyle CssClass="pager" Mode="NumericPages"></PagerStyle>
					<Columns>
						<asp:TemplateColumn>
							<HeaderStyle HorizontalAlign="Center" Width="4%"></HeaderStyle>
							<ItemStyle HorizontalAlign="Center"></ItemStyle>
							<ItemTemplate>
								<asp:CheckBox id="cb_sel" runat="server"></asp:CheckBox>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Descrizione">
							<HeaderStyle HorizontalAlign="Left" Width="25%"></HeaderStyle>
							<ItemStyle HorizontalAlign="Left"></ItemStyle>
							<ItemTemplate>
								<asp:Label id="lb_descLotto" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("descLotto") %>'>
								</asp:Label>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Codice">
							<HeaderStyle HorizontalAlign="Left" Width="25%"></HeaderStyle>
							<ItemStyle HorizontalAlign="Left"></ItemStyle>
							<ItemTemplate>
								<asp:Label id="lb_codLotto" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("codLotto") %>'>
								</asp:Label>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Scadenza">
							<HeaderStyle HorizontalAlign="Center" Width="15%"></HeaderStyle>
							<ItemStyle HorizontalAlign="Center"></ItemStyle>
							<ItemTemplate>
								<asp:Label id="lb_scadenza" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("scadenza").ToShortDateString()  %>'>
								</asp:Label>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Nome Commerciale">
							<HeaderStyle HorizontalAlign="Left" Width="26%"></HeaderStyle>
							<ItemStyle HorizontalAlign="Left"></ItemStyle>
							<ItemTemplate>
								<asp:Label id="lb_NC" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("descNC") %>'>
								</asp:Label>
							</ItemTemplate>
						</asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Attivo" SortExpression="LottoAttivo">
                            <HeaderStyle HorizontalAlign="Center" Width="4%"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            <ItemTemplate>
                                <uc1:AttivazioneLotto id="ucAttivaLotto" runat="server" ToolTip="Lotto non attivo, click per attivarlo" MostraStatoAttivazioneLotto="true"
                                    OnAnnullaAttivazioneLotto="ucAttivaLotto_AnnullaAttivazioneLotto" 
                                    OnOpenModaleAttivazioneLotto="ucAttivaLotto_OpenModaleAttivazioneLotto"
                                    OnSalvaAttivazioneLotto="ucAttivaLotto_SalvaAttivazioneLotto" 
                                    OnShowMessage="ucAttivaLotto_ShowMessage" />
                                <asp:HiddenField id="hdCodiceNomeCommerciale" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem")("codNC") %>'></asp:HiddenField>
                                <asp:HiddenField id="hdLottoAttivo" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem")("attivo") %>'></asp:HiddenField>
                                <asp:HiddenField id="hdEtaMinAttivazione" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem")("lcn_eta_min_attivazione")%>'></asp:HiddenField>
                                <asp:HiddenField id="hdEtaMaxAttivazione" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem")("lcn_eta_max_attivazione") %>'></asp:HiddenField>
                            </ItemTemplate>
                        </asp:TemplateColumn>
					</Columns>
				</asp:datagrid>
            </div>
		</td>
	</tr>
</table>

<on_ofm:onitfinestramodale id="fmMessaggioErrore" title="Errore inserimento lotto" BackColor="Silver" runat="server" 
	RenderModalNotVisible="True" ZIndexPosition="10000" NoRenderX="False" width="400px" >
	<table cellspacing="0" cellpadding="0" border="0">
		<tr>
			<td valign="bottom" align="center" width="432" height="90">
				<table height="100%" cellspacing="0" cellpadding="0" width="100%" border="0">
					<tr>
						<td valign="middle" align="center" width="20%" height="60%">
                            <img id="imgErrore" src="../../Images/errore.gif" runat="server" alt="Errore" />
						</td>
						<td valign="middle" align="center" width="80%" height="60%">
							<asp:Label id="lblErrore" runat="server" Width="272px" Text="Attenzione: il codice lotto inserito non è tra quelli elencati!"
								Font-Names="Tahoma" Font-Size="10pt" Font-Bold="True">Attenzione: il codice lotto inserito non è tra quelli elencati!</asp:Label>
                        </td>
					</tr>
					<tr>
						<td valign="middle" align="center" width="20%" colspan="2" height="40%">
                            <input id="btnChiudiMessaggioErrore" style="width: 96px; height: 24px" type="button" value="Ok" />
						</td>
						<td valign="middle" align="center" width="80%" height="40%"></td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
</on_ofm:onitfinestramodale>

<script type="text/javascript" src="ScriptAssLotti.js"></script>
<script type="text/javascript">
	messaggioErrore='<%= fmMessaggioErrore.clientid() %>';
	modalName='<%= ModaleName%>';
	tbCodLotto='<%= tbCodLottoLCB.clientId%>';
	chkPenna='<%= chkPenna.ClientId%>';
</script>
<script type="text/javascript">
<%response.write(strJS)%>
</script>
