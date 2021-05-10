<%@ Control Language="vb" AutoEventWireup="false" Codebehind="CalVac.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_CalVac" %>

<script type="text/javascript" language="javascript">
    function InitToolbar(t)
    {
	    t.PostBackButton=false;
    }

    function ToolBarCalVacClick(ToolBar, button, evnt)
    {
	    switch (button.Key)	
	    {
	        case 'btnCalVacChiudi':
		        closeFm('modCalendarioVaccinale');
		        evnt.needPostBack=false;
	            break;
	        case 'btnCalVacStampa':
	            closeFm('modCalendarioVaccinale');
		        evnt.needPostBack=true;
	            break;
        }
    }
</script>
<div>
	<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBarCalVac" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
		<DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		<SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
		<ClientSideEvents InitializeToolbar="InitToolbar" Click="ToolBarCalVacClick"></ClientSideEvents>
        <Items>
			<igtbar:TBarButton Key="btnCalVacChiudi" Text="Chiudi" Image="~/Images/Esci.gif"></igtbar:TBarButton>
			<igtbar:TBarButton Key="btnCalVacStampa" Text="Stampa" Image="~/Images/Stampa.gif"></igtbar:TBarButton>
		</Items>
	</igtbar:UltraWebToolbar>
</div>
<div class="title">
    <asp:label id="lb_cic" runat="server"></asp:label>
</div>
<div style="OVERFLOW: auto; HEIGHT: 300px">
	<%response.write(toString())%>
</div>
