<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ConfigurazioneControlli.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.ConfigurazioneControlli" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register Assembly="Onit.Web.UI.WebControls.DynamicPanel" Namespace="Onit.Web.UI.WebControls.DynamicPanel" TagPrefix="dyp" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="uc1" TagName="MotivoEsc" Src="../../../Common/Controls/MotivoEsc.ascx" %>
<%@ Register TagPrefix="uc1" TagName="UscFiltroPrenotazioneSelezioneMultipla" Src="../../../Appuntamenti/Gestione Appuntamenti/UscFiltroPrenotazioneSelezioneMultipla.ascx" %>
<%@ Register TagPrefix="uc1" TagName="CampiEtaAttivazione" Src="../../../Common/Controls/CampiEtaAttivazione.ascx" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Configurazione controlli</title>
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <script type="text/javascript">
        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;

            switch (button.Key) {
                case 'btnSalva':
                    if (!confirm('Salvare le modifiche effettuate?')) evnt.needPostBack = false;
                    break;
                case 'btnAnnulla':
                    if (!confirm('Annullare le modifiche effettuate?')) evnt.needPostBack = false;
                    break;
                case 'btnElimina':
                    if (!confirm("ATTENZIONE: l'eliminazione e' definitiva e non potra' essere annullata. Eliminare la riga selezionata?")) evnt.needPostBack = false;
                    break;
            }

            return;
        }
    </script>
    <style type="text/css">
        .vac-sezione {
            padding: 2px 0 0 5px;
            margin: 1px 0 1px 0;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Height="100%" Width="100%" TitleCssClass="Title3" Titolo="Configurazione controlli">
            <div>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
				    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
				    <Items>
                        <igtbar:TBarButton Key="btnNuovo" Text="Nuovo" DisabledImage="~/Images/nuovo_dis.gif" Image="~/Images/nuovo.gif" ></igtbar:TBarButton>
					    <igtbar:TBarButton Key="btnModifica" Text="Modifica" DisabledImage="~/Images/modifica_dis.gif" Image="~/Images/modifica.gif" ></igtbar:TBarButton>
                        <igtbar:TBarButton Key="btnElimina" Text="Elimina" DisabledImage="~/Images/elimina_dis.gif" Image="~/Images/elimina.gif" ></igtbar:TBarButton>
					    <igtbar:TBSeparator></igtbar:TBSeparator>
					    <igtbar:TBarButton Key="btnSalva" Text="Salva" DisabledImage="~/Images/salva_dis.gif" Image="~/Images/salva.gif"></igtbar:TBarButton>
					    <igtbar:TBarButton Key="btnAnnulla" Text="Annulla" DisabledImage="~/Images/annulla_dis.gif" Image="~/Images/annulla.gif"></igtbar:TBarButton>
				    </Items>
			    </igtbar:UltraWebToolbar>    
            </div>
            <div class="vac-sezione">
                <asp:Label id="lblSezioneConfigurazioni" runat="server">Configurazioni</asp:Label>
            </div>
            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                <asp:datagrid id="dgrConfigurazioneCertificati" runat="server" Width="100%" AutoGenerateColumns="False" CellPadding="1" GridLines="None" >
		            <SelectedItemStyle Font-Bold="True" CssClass="selected"></SelectedItemStyle>
		            <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
		            <ItemStyle CssClass="item"></ItemStyle>
		            <HeaderStyle CssClass="header"></HeaderStyle>
		            <PagerStyle CssClass="pager" Mode="NumericPages"></PagerStyle>
		            <Columns>
                        <asp:ButtonColumn Text="&lt;img src='../../../images/seleziona.gif' title='seleziona' /&gt;" CommandName="Select">
                            <ItemStyle Width="2%" HorizontalAlign="Center" />
                        </asp:ButtonColumn>
                        <asp:BoundColumn DataField="Id" HeaderText="ID" Visible="false"></asp:BoundColumn>
                        <asp:BoundColumn DataField="DataNascitaDa" HeaderText="Nascita Da" Visible="True" DataFormatString="{0:dd/MM/yyyy}" >
                            <HeaderStyle HorizontalAlign="Left" Width="12%" ></HeaderStyle>
			            </asp:BoundColumn>
                        <asp:BoundColumn DataField="DataNascitaA" HeaderText="Nascita A" Visible="True" DataFormatString="{0:dd/MM/yyyy}" >
                            <HeaderStyle HorizontalAlign="Left" Width="12%" ></HeaderStyle>
			            </asp:BoundColumn>									
                        <asp:BoundColumn DataField="PeriodoDa" HeaderText="Eta Inizio">
                            <HeaderStyle HorizontalAlign="Left" Width="18%" ></HeaderStyle>
			            </asp:BoundColumn>
                        <asp:BoundColumn DataField="PeriodoA" HeaderText="Eta Fine">
                            <HeaderStyle HorizontalAlign="Left" Width="18%" ></HeaderStyle>
			            </asp:BoundColumn>
                        <asp:BoundColumn DataField="listCodVacciniDosi" HeaderText="Vaccinazioni - N. Dosi">
                            <HeaderStyle HorizontalAlign="Left" Width="28%" ></HeaderStyle>
			            </asp:BoundColumn>
                        <asp:BoundColumn DataField="SessoCod" HeaderText="Sesso">
                            <HeaderStyle HorizontalAlign="Left" Width="10%" ></HeaderStyle>
			            </asp:BoundColumn>
		            </Columns>
	            </asp:datagrid>
            </dyp:DynamicPanel>
            <div class="vac-Sezione">
                <asp:Label id="lblSezioneDettaglio" runat="server">Dettaglio</asp:Label>
            </div>
            <div>
                <table class="dgr">
                    <colgroup>
                        <col width="16%" />
                        <col width="14%"/>
                        <col width="10%" />
                        <col width="14%"/>
                        <col width="12%" />
                        <col width="34%">
                    </colgroup>
                    <tr>
                        <td class="Label">Data di nascita da</td>
                        <td colspan="3">
                            <on_val:OnitDatePick ID="odpDataNascitaDa" runat="server" Height="20px" CssClass="TextBox_Data" Width="130px" DateBox="True"></on_val:OnitDatePick>
                        </td>
                        <td class="label">Data di nascita a</td>
                        <td>
                            <on_val:OnitDatePick ID="odpDataNascitaA" runat="server" Height="20px" CssClass="TextBox_Data" Width="130px" DateBox="True"></on_val:OnitDatePick>
                        </td>                            
                    </tr>
                    <tr>
                        <td class="Label">Età Inizio</td>
                        <td colspan="3">
                            <uc1:CampiEtaAttivazione ID="ucEtaInizio" runat="server" Obbligatorio="true" TextBoxObbigatorioCssClass="TextBox_Numerico_Obbligatorio"
                                LabelCssClass="label_left" TextBoxCssClass="TextBox_Numerico_Obbligatorio" TextBoxDisabledCssClass="TextBox_Numerico_Disabilitato"  />
                        </td>
                        <td class="Label">Età Fine</td>
                        <td>
                            <uc1:CampiEtaAttivazione ID="ucEtaFine" runat="server" Obbligatorio="true" TextBoxObbigatorioCssClass="TextBox_Numerico_Obbligatorio"
                                LabelCssClass="label_left" TextBoxCssClass="TextBox_Numerico_Obbligatorio" TextBoxDisabledCssClass="TextBox_Numerico_Disabilitato" />
                        </td>
                    </tr>                       
                    <tr>
                        <td class="label">Sesso</td>
                        <td>
                            <asp:DropDownList ID="ddlSesso" runat="server" CssClass="TextBox_Stringa" Width="100%">
                                <asp:ListItem Text="ENTRAMBI" Value="E"></asp:ListItem>
                                <asp:ListItem Text="MASCHIO" Value="M"></asp:ListItem>
                                <asp:ListItem Text="FEMMINA" Value="F"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td class="label">Data Controllo</td>
                        <td>
                            <on_val:OnitDatePick ID="odpDataControllo" runat="server" Height="20px" CssClass="TextBox_Stringa" Width="120px" DateBox="True"></on_val:OnitDatePick>
                        </td>
                        <td class="label">Immunità</td>
                        <td colspan="3">
                            <table class="filtroCnv">
                                <tr>
                                    <td style="height: 22px; border: navy 1px solid; padding-left: 5px; background: gainsboro; width: 100%;">
                                        <asp:Label ID="lblMotiviImmunita" Style="height: 18px; font-size: 10px; text-transform: uppercase; font-style: italic; font-family: verdana" runat="server" Text=""></asp:Label>
                                    </td>
                                    <td width="26px" align="right">
                                        <asp:Button ID="buttonMotiviImmunita" runat="server" Text="Motivi Immunita" ToolTip="Motivi Immunita" Enabled="false" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td class="label">Vaccinazioni - Dosi</td>
                        <td colspan="3">
                            <table class="filtroCnv">
                                <tr>
                                    <td width="26px" align="right">
                                        <asp:ImageButton ID="btnImgVaccinazioniDosi" runat="server" onmouseover="mouse(this,'over');"
                                            title="Impostazione filtro vaccinazioni-dosi" onmouseout="mouse(this,'out');"
                                            ImageUrl="../../../images/filtro_vaccinazioni_dis.gif" Enabled="false" OnClick="btnImgVaccinazioniDosi_Click" />
                                    </td>
                                    <td style="height: 22px; border: navy 1px solid; padding-left: 5px; background: gainsboro; width: 100%;">
                                        <asp:Label ID="lblVaccinazioniDosi" Style="height: 18px; font-size: 10px; text-transform: uppercase; font-style: italic; font-family: verdana" runat="server" Text=""></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td class="label">Esonero</td>
                        <td colspan="3">
                            <table class="filtroCnv">
                                <tr>
                                    <td style="height: 22px; border: navy 1px solid; padding-left: 5px; background: gainsboro; width: 100%;">
                                        <asp:Label ID="lblMotiviEsonero" Style="height: 18px; font-size: 10px; text-transform: uppercase; font-style: italic; font-family: verdana" runat="server" Text=""></asp:Label>
                                    </td>
                                    <td width="26px" align="right">
                                        <asp:Button ID="buttonMotiviEsonero" runat="server" Text="Motivi Esonero " ToolTip="Motivi Esonero" Enabled="false" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td class="label" valign="top">Testo positivo</td>
                        <td colspan="3">
                            <asp:TextBox ID="txtTestoPositivo" runat="server" CssClass="TextBox_Stringa" MaxLength="500" TextMode="MultiLine" Rows="3" Width="100%"></asp:TextBox>
                        </td>
                        <td class="label" valign="top">Testo negativo</td>
                        <td>
                            <asp:TextBox ID="txtTestoNegativo" runat="server" CssClass="TextBox_Stringa" MaxLength="500" TextMode="MultiLine" Rows="3" Width="100%"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="label" valign="top"> Controlla Appuntamenti</td>
                        <td valign="top"><asp:CheckBox runat="server" ID="cbAppuntamenti" CssClass="TextBox_Stringa" AutoPostBack="true" /> </td>
                        <td class="label" valign="top">Tipo Controllo</td>
                        <td valign="top">
                            <asp:DropDownList ID="ddlTipoControllo" runat="server" CssClass="TextBox_Stringa" Width="100%">
                                <asp:ListItem Text="PARZIALE" Value="1"></asp:ListItem>
                                <asp:ListItem Text="POSITIVO" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td class="label" valign="top">Testo parziale</td>
                        <td>
                            <asp:TextBox ID="txtTestoEsoneri" runat="server" CssClass="TextBox_Stringa" MaxLength="500" TextMode="MultiLine" Rows="3" Width="100%"></asp:TextBox>
                        </td>

                    </tr>
                </table>
            </div>
        </on_lay3:OnitLayout3>
        
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
                        <uc1:UscFiltroPrenotazioneSelezioneMultipla ID="UscFiltroPrenotazioneSelezioneMultipla1" runat="server" Tipo="2" ControlloDosi="2" TipoVifsualizzazione="2" ></uc1:UscFiltroPrenotazioneSelezioneMultipla>
                    </td>
                    <td></td>
                </tr>
                <tr height="10">
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
                <tr height="10">
                    <td colspan="5"></td>
                </tr>
            </table>
        </on_ofm:OnitFinestraModale>
    
        <on_ofm:OnitFinestraModale id="modInsMotivoImmunita" title="Inserisci Esclusione" runat="server" width="618px"  BackColor="LightGray" NoRenderX="True">
	        <uc1:MotivoEsc id="InsMotivoImmunita" runat="server"></uc1:MotivoEsc>
        </on_ofm:OnitFinestraModale>
        <on_ofm:OnitFinestraModale id="modInsMotivoEsonero" title="Inserisci Esclusione" runat="server" width="618px"  BackColor="LightGray" NoRenderX="True">
            <uc1:MotivoEsc id="InsMotivoEsonero" runat="server"></uc1:MotivoEsc>
        </on_ofm:OnitFinestraModale>
    </form>
</body>
</html>
