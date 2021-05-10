<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="StoricoRinnovi.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.StoricoRinnovi" %>

<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>

<link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
<link href="../../css/button.css" type="text/css" rel="stylesheet" />
<link href="../../css/default/button.default.css" type="text/css" rel="stylesheet" />

<script type="text/javascript" src="<%= Page.ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
<script type="text/javascript">

    function ImpostaImmagineOrdinamento(imgId, imgUrl) {
        var img = document.getElementById(imgId);
        if (img != null) {
            img.style.display = 'inline';
            img.src = imgUrl;
        }
    }
   
    var ModaleName = '<%= ModaleName %>';

    function InizializzaToolBar_InsVacEsc(t) {
        t.PostBackButton = false;
    }

    function ToolBarClick_InsVacEsc(toolBar, button, evnt) {
        evnt.needPostBack = true;
        switch (button.Key) {
            case 'btn_Chiudi':                
                evnt.needPostBack = true;
                break;
        }
    }

</script>

 <dyp:DynamicPanel ID="dypStoricoAppuntamenti" runat="server" Width="100%" Height="100%" >
        
        <asp:Label ID="lblMessage" runat="server" CssClass="TextBox_Stringa message" Visible="false" ></asp:Label>
            <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                <ClientSideEvents InitializeToolbar="InizializzaToolBar_InsVacEsc" Click="ToolBarClick_InsVacEsc"></ClientSideEvents>
                <Items>
                    <igtbar:TBarButton Key="btn_Chiudi" DisabledImage="~/Images/logoff.gif"
                        Text="Chiudi" Image="~/Images/logoff.gif" />
                </Items>
            </igtbar:UltraWebToolbar>

            <div id="divLegenda" class="legenda-vaccinazioni" runat="server">
                <span class="legenda-vaccinazioni-esclusioneRinnovata">R</span>
                <span>Esclusione rinnovata</span>
            </div>

            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                  <asp:DataGrid ID="dg_VacEscStorico" runat="server" CssClass="datagrid" Width="100%" GridLines="None"
                    CellPadding="3" AutoGenerateColumns="False" AllowSorting="True">
                    <HeaderStyle Font-Bold="True" CssClass="header"></HeaderStyle>
                    <PagerStyle CssClass="pager" Mode="NumericPages"></PagerStyle>
                    <SelectedItemStyle Font-Bold="True" CssClass="selected"></SelectedItemStyle>
                    <EditItemStyle CssClass="edit"></EditItemStyle>
                    <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
                    <ItemStyle CssClass="item"></ItemStyle>
                    <Columns>
                        <asp:TemplateColumn>
						    <HeaderStyle Width="1%"></HeaderStyle>
						    <ItemTemplate>
							    <asp:Label id="lblRinnovo" runat="server" CssClass="legenda-vaccinazioni-esclusioneRinnovata" 
                                    Visible='<%# IsVisibleFlagRinnovo(Eval("StatoEliminazione")) %>'
                                    ToolTip="Esclusione rinnovata" >R</asp:Label>
						    </ItemTemplate>
					    </asp:TemplateColumn>
                         <asp:TemplateColumn>
                             <HeaderStyle Width="0%"/>
                            <ItemTemplate>
                                <asp:HiddenField id="IdHidden" runat ="server" Value='<%# Eval("Id") %>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn>
                            <ItemStyle Width="0%" />
                            <ItemTemplate>
                                <asp:HiddenField id="CodiceVaccinazioneHidden" runat ="server" Value='<%# Eval("CodiceVaccinazione") %>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Vaccinazione <img id='imgVac' alt='' src='../../images/transparent16.gif' />" SortExpression="DescrizioneVaccinazione">
                            <HeaderStyle HorizontalAlign="left" Width="13%"></HeaderStyle>
                            <ItemTemplate>
                                <asp:Label ID="DescrizioneVaccinazione" runat="server" CssClass="label"  Text='<%# String.Format("{0} ({1})", Eval("DescrizioneVaccinazione"), Eval("CodiceVaccinazione"))%>'>
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>                                              
                        <asp:TemplateColumn HeaderText="Data Visita <img id='imgDataVis' alt='' src='../../images/transparent16.gif' />"
                            HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="13%" SortExpression="DataVisita" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <asp:Label ID="lblDataVisita" CssClass="label_left" runat="server"
                                    Text='<%# ConvertToDateString(DataBinder.Eval(Container.DataItem, "DataVisita"), False)%>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Scadenza <img id='imgDataScad' alt='' src='../../images/transparent16.gif' />"
                            HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="13%" SortExpression="DataScadenza" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <asp:Label ID="lblDataScadenza" CssClass="label_left" runat="server"
                                    Text='<%# ConvertToDateString(DataBinder.Eval(Container.DataItem, "DataScadenza"), False)%>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>  
                        <asp:TemplateColumn HeaderText="Data Registrazione <img id='imgDataReg' alt='' src='../../images/transparent16.gif' />"
                            HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="13%" SortExpression="DataRegistrazione" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <asp:Label ID="lblDataRegistrazione" CssClass="label_left" runat="server"
                                    Text='<%# ConvertToDateString(DataBinder.Eval(Container.DataItem, "DataRegistrazione"), True)%>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>                            
                        <asp:BoundColumn DataField="DescrizioneOperatore" HeaderText="Operatore <img id='imgOpe' alt='' src='../../images/transparent16.gif' />" SortExpression="DescrizioneOperatore">
							<HeaderStyle Width="20%"></HeaderStyle>
						</asp:BoundColumn>
                        <asp:BoundColumn DataField="UtenteRegistrazione" HeaderText="Registrazione" Visible="false">
							<HeaderStyle Width="0%"></HeaderStyle>
						</asp:BoundColumn>
                        <asp:BoundColumn DataField="UtenteModifica" HeaderText="Modifica" Visible="false">
							<HeaderStyle Width="0%"></HeaderStyle>
						</asp:BoundColumn>
                         <asp:TemplateColumn HeaderText="Data Eliminazione <img id='imgDataEli' alt='' src='../../images/transparent16.gif' />"
                            HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="13%" SortExpression="DataEliminazione" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <asp:Label ID="lblDataEliminazione" CssClass="label_left" runat="server"
                                    Text='<%# ConvertToDateString(DataBinder.Eval(Container.DataItem, "DataEliminazione"), True)%>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="StatoEliminazione" HeaderText="Stato" Visible="false">
							<HeaderStyle Width="1%"></HeaderStyle>
						</asp:BoundColumn>
                        <asp:BoundColumn DataField="UtenteEliminazione" HeaderText="Utente Eliminazione <img id='imgUteEli' alt='' src='../../images/transparent16.gif' />" SortExpression="UtenteEliminazione">
							<HeaderStyle Width="20%"></HeaderStyle>
						</asp:BoundColumn>
                    </Columns>
                </asp:DataGrid>
        </dyp:DynamicPanel>
     </dyp:DynamicPanel>
