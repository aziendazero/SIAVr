<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Bilanci-Osservazioni.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Bilanci_Osservazioni" %>

<%@ Register TagPrefix="dyp" Assembly="Onit.Web.UI.WebControls.DynamicPanel" Namespace="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>

<%@ Import namespace="Onit.OnAssistnet.OnVac" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Bilanci Osservazioni</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <style type="text/css">
        .Abilitato
        {
            border-right: black 1px solid;
            border-top: black 1px solid;
            font-weight: bold;
            border-left: black 1px solid;
            width: 15px;
            border-bottom: black 1px solid;
            text-align: center;
        }
        .Disabilitato
        {
            font-size: 15px;
            color: white;
            text-align: center;
        }

        .margin {
            margin: 2px;
        }
    </style>

    <script type="text/javascript">

        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;

            switch (button.Key) {

                case 'btnIndietro':
                    evnt.needPostBack = confirm("Eventuali modifiche effettuate dopo il salvataggio verranno perse. Continuare?");
                    break;
            }
        }

        function rollover(btn, stato) {

            if (btn.disabled == false) {
                if (stato == 'over') {
                    btn.src = btn.src.split(".png")[0] + "_over.png";
                }
                else
                    if (stato == 'out') {
                        btn.src = btn.src.split("_over.png")[0] + ".png";
                    }
            }
        }

    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server" >
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Height="100%" Width="100%" Titolo="Bilanci - Osservazioni" TitleCssClass="Title3" >

            <div class="title" id="titolo" runat="server"></div>

		    <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		        <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
			    <Items>
				    <igtbar:TBarButton Key="btnAggiungi" Text="Aggiungi" Image="../../../images/list_add.png" tooltip="Aggiunge le osservazioni selezionate alle osservazioni associate al bilancio"></igtbar:TBarButton>
				    <igtbar:TBarButton Key="btnElimina" Text="Elimina" Image="../../../images/list_remove.png" tooltip="Rimuove le osservazioni selezionate alle osservazioni associate al bilancio" ></igtbar:TBarButton>
				    <igtbar:TBSeparator></igtbar:TBSeparator>
                    <igtbar:TBarButton Key="btnSalva" Text="Salva" Image="~/Images/salva.gif"></igtbar:TBarButton>
				    <igtbar:TBarButton Key="btnAnnulla" Text="Annulla" Image="~/Images/annulla.gif"></igtbar:TBarButton>
				    <igtbar:TBSeparator></igtbar:TBSeparator>
				    <igtbar:TBarButton Key="btnIndietro" Text="Indietro" Image="../../../images/prev.gif" DisabledImage="../../../images/prev_dis.gif"></igtbar:TBarButton>
			    </Items>
		    </igtbar:UltraWebToolbar>

            <dyp:DynamicPanel ID="DynamicPanel1" runat="server" Width="100%" Height="28px" ScrollBars="None" ExpandDirection="horizontal">
            
                <dyp:DynamicPanel ID="DynamicPanel2" runat="server" Width="45%" ScrollBars="None" >
                    <div class="vac-sezione margin">Osservazioni disponibili</div>
                </dyp:DynamicPanel>
            
                <dyp:DynamicPanel ID="DynamicPanel3" runat="server" Width="55%" ScrollBars="None" >
                    <div class="vac-sezione margin">Osservazioni associate</div>
                </dyp:DynamicPanel>

            </dyp:DynamicPanel>             

            <dyp:DynamicPanel ID="DynamicPanel4" runat="server" Width="100%" Height="100%" ScrollBars="None" ExpandDirection="horizontal">

                <dyp:DynamicPanel ID="DynamicPanel5" runat="server" Width="45%" Height="100%" ScrollBars="Auto" CssClass="margin">
                    <asp:DataGrid ID="dgrOsservazioni" runat="server" Width="100%" CssClass="DataGrid" AutoGenerateColumns="False">
                        <HeaderStyle CssClass="Header"></HeaderStyle>
                        <ItemStyle CssClass="Item"></ItemStyle>
                        <AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
                        <SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
                        <EditItemStyle CssClass="Edit"></EditItemStyle>
                        <Columns>
                            <asp:TemplateColumn>
                                <HeaderStyle Width="5%" HorizontalAlign="Center"></HeaderStyle>
                                <HeaderTemplate>
                                    <asp:CheckBox id="chkSelezioneHeader" runat="server" onclick="CheckAll('dgrOsservazioni', this.checked, 0, 0);" 
                                        ToolTip="Seleziona tutte le osservazioni della colonna"></asp:CheckBox>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox id="chkSelezione" runat="server"></asp:CheckBox>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:BoundColumn DataField="CodiceOsservazione" HeaderText="Codice">
                                <HeaderStyle Width="25%" />
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="DescrizioneOsservazione" HeaderText="Descrizione">
                                <HeaderStyle Width="70%" />
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="TipoRisposta" Visible="false"></asp:BoundColumn>
                        </Columns>
                    </asp:DataGrid>
                </dyp:DynamicPanel>

                <dyp:DynamicPanel ID="DynamicPanel6" runat="server" Width="55%" Height="100%" ScrollBars="Auto" CssClass="margin">
                    <asp:DataGrid ID="dgrOsservazioniAssociate" runat="server" Width="100%" CssClass="DataGrid" AutoGenerateColumns="False">
                        <HeaderStyle CssClass="Header"></HeaderStyle>
                        <ItemStyle CssClass="Item"></ItemStyle>
                        <AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
                        <SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
                        <EditItemStyle CssClass="Edit"></EditItemStyle>
                        <Columns>
                            <asp:TemplateColumn>
                                <HeaderStyle Width="2%" HorizontalAlign="Center" />
                                <ItemStyle Width="2%" HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:ImageButton ID="btnUp" runat="server" ImageUrl="../../../images/arrow_blue12_up.png" CommandName="MoveUp" ToolTip="Sposta sopra"
                                        onmouseover="rollover(this, 'over');" onmouseout="rollover(this, 'out');" />
                                    <asp:ImageButton ID="btnDown" runat="server" ImageUrl="../../../images/arrow_blue12_down.png" CommandName="MoveDown" ToolTip="Sposta sotto"
                                        onmouseover="rollover(this, 'over');" onmouseout="rollover(this, 'out');" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn>
                                <HeaderStyle Width="4%" HorizontalAlign="Center" />
                                <ItemStyle Width="4%" HorizontalAlign="Center" />
                                <HeaderTemplate>
									<asp:CheckBox onclick="CheckAll('dgrOsservazioniAssociate', this.checked, 1, 0);" id="chkSelezioneAssociateHeader" runat="server" 
                                        ToolTip="Seleziona tutte le osservazioni della colonna"></asp:CheckBox>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox id="chkSelezioneAssociata" runat="server" ToolTip="Seleziona l'osservazione da eliminare dal gruppo delle osservazioni associate al bilancio"></asp:CheckBox></td>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:BoundColumn DataField="CodiceOsservazione" HeaderText="Codice">
                                <HeaderStyle Width="15%" />
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="DescrizioneOsservazione" HeaderText="Descrizione">
                                <HeaderStyle Width="40%" />
                            </asp:BoundColumn>
                            <asp:TemplateColumn HeaderText="Sezione">
                                <HeaderStyle Width="30%" />
                                <ItemTemplate>
                                    <asp:DropDownList id="cmbSezione" runat="server" Width="100%"></asp:DropDownList>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Obbl.">
                                <HeaderStyle Width="6%" HorizontalAlign="Center" />
                                <ItemStyle Width="6%" HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:CheckBox id="chkObbligatorio" runat="server" tooltip="Indica se l'osservazione è obbligatoria" Checked='<%# Eval("IsObbligatoria")%>'></asp:CheckBox></td>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn>
                                <HeaderStyle Width="3%" HorizontalAlign="Center" />
                                <ItemStyle Width="3%" HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:ImageButton id="btnCondizioni" runat="server" CommandName="SetCondizioni" ImageUrl="../../../images/ricalcola.gif"
                                        ToolTip="Imposta condizioni" style="display: block"></asp:ImageButton>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:BoundColumn DataField="TipoRisposta" Visible="false"></asp:BoundColumn>
                        </Columns>
                    </asp:DataGrid>
                </dyp:DynamicPanel>

            </dyp:DynamicPanel>

		    <onitcontrols:OnitFinestraModale id="fmCondizioni" title="Condizioni" runat="server" width="656px" BackColor="#E0E0E0">

			    <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="tlbCondizioni" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
				    <Items>
					    <igtbar:TBarButton Key="btnAggiungi" Text="Aggiungi" Image="~/Images/nuovo.gif"></igtbar:TBarButton>
					    <igtbar:TBarButton Key="btnElimina" Text="Elimina" Image="~/Images/elimina.gif"></igtbar:TBarButton>
					    <igtbar:TBarButton Key="btnAnnulla" Text="Chiudi" Image="~/Images/annulla.gif"></igtbar:TBarButton>
				    </Items>
			    </igtbar:UltraWebToolbar>

			    <div class="sezione">Condizioni esistenti</div>
			    <div style="height:200px;overflow:auto;">

			    <asp:DataList id="dlsCondizioni" runat="server" Width="100%" CssClass="datagrid">
				    <HeaderStyle CssClass="header"></HeaderStyle>
				    <ItemStyle CssClass="item"></ItemStyle>
				    <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
				    <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
				    <HeaderTemplate>
					    <table cellspacing="0" cellpadding="0" width="100%" border="0" style="table-layout:fixed">
						    <tr>
							    <td style="width:25px"></td>
							    <td style="width:40%">Risposta</td>
							    <td style="width:40%">Osservazione collegata</td>
							    <td style="width:20%; text-align:center;">Condizioni</td>
						    </tr>
					    </table>
				    </HeaderTemplate>
				    <ItemTemplate>
					    <table style="table-layout: fixed" cellspacing="0" cellpadding="0" width="100%" border="0">
						    <tr>
							    <td style="width: 25px">
								    <asp:CheckBox id="chkSelezione" runat="server"></asp:CheckBox></td>
							    <td style="width: 40%"><%# Container.DataItem("RIS_DESCRIZIONE")%></td>
							    <td style="width: 40%"><%# Container.DataItem("OSS_DESCRIZIONE")%></td>
							    <td style="width: 20%; text-align: center">
                                    <%# IIf(Container.DataItem("LRO_FLAG_VISIBILE").ToString() = "S", "<span class='Abilitato' style='background-color:gainsboro'>D</span>", "<span class='Disabilitato'>&nbsp;</span>")%>
                                    &nbsp;
                                    <%# IIf(Container.DataItem("LRO_FLAG_DEFAULT").ToString() = "S", "<span class='Abilitato'>D</span>", "<span class='Disabilitato'>&nbsp;</span>")%>
                                    &nbsp;
                                    <%# iif(Container.DataItem("LRO_FLAG_COLLEGATA").ToString()="S","<span class='Abilitato' style='background-color:rgb(0,150,0)'>C</span>","<span class='Disabilitato'>&nbsp;</span>")%>
							    </td>
						    </tr>
					    </table>
				    </ItemTemplate>
			    </asp:DataList>
			    </div>
			    <div class="sezione">Nuova condizione</div>
			    <table class="datagrid" id="Table1" cellspacing="0" cellpadding="0" width="100%" border="0">
				    <tr>
					    <td style="width: 40%">Alla risposta:</td>
					    <td>
						    <asp:DropDownList id="cmbRisposta" runat="server" Width="100%"></asp:DropDownList></td>
				    </tr>
				    <tr>
					    <td style="height: 17px">Osservazione collegata:</td>
					    <td style="height: 17px">
						    <asp:DropDownList id="cmbOsservazione" runat="server" Width="100%"></asp:DropDownList></td>
				    </tr>
				    <tr>
					    <td>Azione:</td>
					    <td>
						    <asp:CheckBox id="chkDisabilita" runat="server" Text="Disabilita"></asp:CheckBox><br/>
						    <asp:CheckBox id="chkCollegata" runat="server" Text="Collegata"></asp:CheckBox><br/>
						    <asp:CheckBox id="chkImpostaDefault" style="DISPLAY: none" runat="server" Text="Imposta un default"></asp:CheckBox></td>
				    </tr>
				    <tr id="default" style="display: none">
					    <td>Risposta di default</td>
					    <td>
						    <asp:DropDownList id="cmbRispostaDefault" runat="server" Width="100%"></asp:DropDownList></td>
				    </tr>
			    </table>
		    </onitcontrols:OnitFinestraModale>

        </on_lay3:OnitLayout3>

    </form>

    <script type="text/javascript">
        
        function mostraDefault() {
            if (document.getElementById('chkImpostaDefault').checked)
                document.getElementById('default').style.display = 'inline';
            else
                document.getElementById('default').style.display = 'none';
        }
		
		if (document.getElementById('chkImpostaDefault') != null) {
		    mostraDefault();
		    document.getElementById('chkImpostaDefault').onclick = mostraDefault;
		}

    </script>

</body>
</html>
