<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="FirmaDigitaleArchiviazioneSostitutiva.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.FirmaDigitaleArchiviazioneSostitutiva" %>
<%@ Register Assembly="Onit.Web.UI.WebControls.DynamicPanel" Namespace="Onit.Web.UI.WebControls.DynamicPanel" TagPrefix="dyp" %>

<script type="text/javascript">
    function InizializzaToolBarFirma(t) {
        t.PostBackButton = false;
    }

    function SignatureCallBack(s, e) {
        var lnk = document.getElementById('<% =lnkPostBk.ClientID %>');
        var hd = document.getElementById('<% =txtResult.ClientID%>');
        hd.value = JSON.stringify(e[0]);
        resetWaitScreen(true);
        lnk.click();
    }

    function ToolBarFirmaClick(ToolBar, button, evnt) {
        evnt.needPostBack = true;

        switch (button.Key) {

            case 'btnFirma':
                evnt.needPostBack = false;

                if (confirm("Dopo la firma, il documento non sarà più modificabile.\nProcedere con la firma digitale del documento?")) {
                    showWaitScreen(2000, true);
                    signDocument('<%= Me.UrlServizioFirmaDigitale%>', [<%= Me.IdDocumento %>], <%= Me.IdUtenteCorrente%>, '<%= Me.IdApplicativoCorrente%>', '<%= Me.CodiceAziendaCorrente%>', SignatureCallBack);
                }
                break;
        }
    }
</script>

<div>
	<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="toolbarFirma" runat="server" ItemWidthDefault="100px" CssClass="infratoolbar">
        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		<SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
        <ClientSideEvents InitializeToolbar="InizializzaToolBarFirma" Click="ToolBarFirmaClick"></ClientSideEvents>
		<Items>
            <igtbar:TBarButton Key="btnIndietro" Text="Indietro" DisabledImage="~/Images/indietro_dis.gif" 
                Image="~/Images/indietro.gif" ToolTip="Torna all'elenco delle anamnesi" />

			<igtbar:TBarButton Key="btnFirma" Text="Firma digitale" DisabledImage="../../images/firmaDigitale_dis.png" 
                Image="../../images/firmaDigitale.png" ToolTip="Firma digitalmente il documento">
			</igtbar:TBarButton>
		</Items>
	</igtbar:UltraWebToolbar>
    <asp:HiddenField ID="txtResult" runat="server" />
    <asp:LinkButton ID="lnkPostBk" runat="server" style="display:none"></asp:LinkButton>
</div>
    
<dyp:DynamicPanel ID="dyp1" runat="server" Width="100%" Height="100%" ExpandDirection="vertical" ScrollBars="None">
    <iframe id="iframeAnteprimaAnamnesi" runat="server" style="width:100%; height:100%;"></iframe>
</dyp:DynamicPanel>
<applet id="OnJSign" style="width:1px; height:1px;" archive="<%= ResolveUrl("~/Applet/OnJSignature/1.1.0.0/OnJSignature.jar") %>" code="it.onit.OnJSign.class" width="100" height="100">
</applet>