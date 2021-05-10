<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="LogEventi.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.LogEventi" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<%@ Register TagPrefix="uc2" TagName="StatiAnagrafici" Src="../../Common/Controls/StatiAnagrafici.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Log Eventi</title>
    
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <style type="text/css">
        .dettaglioLog {
            text-align: right;
            font-family: Verdana;
            font-size: 12px;
            width: 100%;
        }

        .labelLog {
            text-align: left;
            font-family: Verdana;
            font-size: 12px;
            width: 100%;
            border: 1px solid black;
            background-color: whitesmoke;
            padding: 2px;
        }
    </style>

    <script type="text/javascript">
        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;

            switch (button.Key) {
                case 'btnStampaElencoLog':
                    // controllo se l'elenco contiene almeno un dato da stampare
                    if ("<%= Me.dgrLog.Items.Count %>" == "0") {
                        alert("Stampa non effettuata: nessun dato da stampare.");
                        evnt.needPostBack = false;
                    }
                    break;
            }
        }

        function toggleVisibilitaFiltri(img, divId) {
            var div = document.getElementById(divId);

            if (div != null) {
                var showDettaglio = (img.src.indexOf('piu.gif') != -1);

                if (showDettaglio) {
                    img.src = '../../images/meno.gif';
                    img.alt = 'Nasconde i filtri di ricerca';
                    div.style.display = 'block';
                }
                else {
                    img.src = '../../images/piu.gif';
                    img.alt = 'Mostra i filtri di ricerca';
                    div.style.display = 'none';
                }
            }
        }
    </script> 

</head>
<body>
    <form id="form1" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Height="100%" Width="100%" TitleCssClass="Title3" Titolo="Log Eventi">
            <div class="title" id="PanelTitolo" runat="server" style="width: 100%;">
			    <asp:Label id="LayoutTitolo" runat="server">&nbsp;Log Eventi</asp:Label>
		    </div>
            <div>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
		            <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
		            <Items>
		                <igtbar:TBarButton Key="btnCerca" Text="Cerca" DisabledImage="../../images/cerca.gif" Image="../../images/cerca.gif"
		                    ToolTip="Effettua la ricerca in base ai filtri impostati" ></igtbar:TBarButton>
                        <igtbar:TBarButton Key="btnPulisci" Text="Pulisci" DisabledImage="../../images/pulisci_dis.gif" Image="../../images/pulisci.gif"
		                    ToolTip="Cancella i filtri e i risultati della ricerca" ></igtbar:TBarButton>
		                <igtbar:TBSeparator />
		                <igtbar:TBarButton Key="btnStampaElencoLog" Text="Stampa" DisabledImage="../../images/stampa_dis.gif" Image="../../images/stampa.gif"
		                    ToolTip="Stampa i risultati della ricerca" ></igtbar:TBarButton>		                        
                    </Items>
		        </igtbar:UltraWebToolbar>
            </div>
            <div>
                <table width="100%" style="background-color: #F5F5F5" border="0">
	                <tr style="height:5px;">
		                <td></td>
	                </tr>                            
	                <tr>
                        <td>
			                <fieldset class="vacFieldset" title="Filtri di ricerca" style="padding-bottom:5px;">
			                    <legend style="width:200px; text-align: left; font-family: Arial; font-size: 12px;">
                                    <img alt="Nasconde i filtri di ricerca" src="../../images/meno.gif" 
                                        onclick="toggleVisibilitaFiltri(this, 'divFiltriPaziente')" style="cursor: pointer; margin-right:3px;" />
                                    Filtri di ricerca per paziente
                                </legend>
                                <div id="divFiltriPaziente" style="display:block;">
                                    <table width="100%" cellpadding="2" cellspacing="0" border="0">
                                        <colgroup>
                                            <col width="13%" />
                                            <col width="3%" />
                                            <col width="15%" />
                                            <col width="3%" />
                                            <col width="15%" />
                                            <col width="13%" />
                                            <col width="36%" />
                                            <col width="2%" />
                                        </colgroup>
                                            
                                        <tr style="height:10px">
                                            <td colspan="8"  class="label"></td>
                                        </tr>
                                        <tr>
                                            <td class="label">Cognome</td>
                                            <td></td>
                                            <td colspan="3">
                                                <asp:TextBox ID="txtCognome" runat="server" CssClass="textbox_stringa" Width="100%" style="text-transform:uppercase" ></asp:TextBox>
                                            </td>
                                            <td class="label">Nome</td>
                                            <td>
                                                <asp:TextBox ID="txtNome" runat="server" CssClass="textbox_stringa" Width="100%" style="text-transform:uppercase" ></asp:TextBox>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td class="label">Data Nascita</td>
                                            <td class="label">Da</td>
                                            <td>
                                                <on_val:onitdatepick ID="dpkDataNascitaDa" runat="server" CssClass="textbox_data" DateBox="True" Height="20px" Width="120px"></on_val:onitdatepick>
                                            </td>
                                            <td class="label">A</td>
                                            <td>
                                                <on_val:onitdatepick ID="dpkDataNascitaA" runat="server" CssClass="textbox_data" DateBox="True" Height="20px" Width="120px"></on_val:onitdatepick>
                                            </td>
                                            <td class="label">Codice Fiscale</td>
                                            <td>
                                                <asp:TextBox ID="txtCodiceFiscale" runat="server" CssClass="textbox_stringa" Width="100%" ></asp:TextBox>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td class="label">Centro Vaccinale</td>
                                            <td></td>
                                            <td colspan="3">
									            <onitcontrols:OnitModalList id="fmConsultorio" runat="server" Filtro="cns_data_apertura <= SYSDATE AND (cns_data_chiusura > SYSDATE OR cns_data_chiusura IS NULL) ORDER BY cns_descrizione"
										            Width="70%" CampoDescrizione="CNS_DESCRIZIONE" CampoCodice="CNS_CODICE" Tabella="T_ANA_CONSULTORI" PosizionamentoFacile="False"
										            LabelWidth="-1px" CodiceWidth="29%" Label="Titolo" UseCode="True" SetUpperCase="True" Obbligatorio="False"></onitcontrols:OnitModalList>
                                            </td>
                                            <td class="label">Stato Anagrafico</td>
                                            <td>
                                                <uc2:StatiAnagrafici id="ucStatiAnagrafici" runat="server" ShowLabel="false"></uc2:StatiAnagrafici>
                                            </td>
                                            <td></td>
                                        </tr>
                                    </table>
                                </div>
                            </fieldset>
                        </td>
                    </tr>
                </table>
            </div>
            <div>
                <table width="100%" style="background-color: #F5F5F5" border="0">
	                <tr style="height:3px;">
		                <td></td>
	                </tr>
	                <tr>
		                <td>
			                <fieldset class="vacFieldset" title="Filtri di ricerca" style="padding-bottom:5px;" >
			                    <legend style="width:200px; text-align:left; font-family: Arial; font-size: 12px;">
                                    <img alt="Nasconde i filtri di ricerca" src="../../images/meno.gif" 
                                        onclick="toggleVisibilitaFiltri(this, 'divFiltriOperazione')" style="cursor: pointer; margin-right:3px;" />
                                    Filtri di ricerca per operazione
                                </legend>
                                <div id="divFiltriOperazione" style="display:block;">
                                    <table width="100%" cellpadding="2" cellspacing="0" border="0">
                                        <colgroup>
                                            <col width="13%" />
                                            <col width="3%" />
                                            <col width="15%" />
                                            <col width="3%" />
                                            <col width="15%" />
                                            <col width="13%" />
                                            <col width="36%" />
                                            <col width="2%" />
                                        </colgroup>

                                        <tr style="height:10px">
                                            <td colspan="8"></td>
                                        </tr>
                                        <tr>
                                            <td class="label">Data Operazione</td>
                                            <td class="label">Da</td>
                                            <td>
                                                <on_val:onitdatepick ID="dpkDataOperazioneDa" runat="server" CssClass="textbox_data" DateBox="True" Height="20px" Width="120px"></on_val:onitdatepick>
                                            </td>
                                            <td class="label">A</td>
                                            <td>
                                                <on_val:onitdatepick ID="dpkDataOperazioneA" runat="server" CssClass="textbox_data" DateBox="True" Height="20px" Width="120px"></on_val:onitdatepick>
                                            </td>
                                            <td class="label">Tipo Operazione</td>
                                            <td>
                                                <asp:DropDownList ID="ddlTipoOperazione" runat="server" Width="100%" CssClass="textbox_stringa"
                                                    DataTextField="Value" DataValueField="Key"></asp:DropDownList>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td class="label">Argomento</td>
                                            <td></td>
                                            <td colspan="3">
                                                <asp:DropDownList ID="ddlArgomento" runat="server" Width="100%" CssClass="textbox_stringa"
                                                    DataTextField="Descrizione" DataValueField="Codice"></asp:DropDownList>
                                            </td>
                                            <td class="label">Risultato</td>
                                            <td>
                                                <table width="100%" border="0" cellpadding="3" cellspacing="0" class="label_left">
                                                    <colgroup>
                                                        <col width="25%" />
                                                        <col width="25%" />
                                                        <col width="25%" />
                                                        <col width="25%" />
                                                    </colgroup>
                                                    <tr>
                                                        <td>
                                                            <asp:RadioButton ID="rdbRisultatoSuccesso" runat="server" GroupName="risultato" Text="Successo" /></td>
                                                        <td>
                                                            <asp:RadioButton ID="rdbRisultatoWarning" runat="server" GroupName="risultato" Text="Warning" /></td>
                                                        <td>
                                                            <asp:RadioButton ID="rdbRisultatoErrore" runat="server" GroupName="risultato" Text="Errore" /></td>
                                                        <td>
                                                            <asp:RadioButton ID="rdbRisultatoTutti" runat="server" GroupName="risultato" Text="Tutti" Checked="true" /></td>
                                                    </tr>
                                                </table>                                                    
                                            </td>
                                            <td></td>
                                        </tr>
                                    </table>
                                </div>
                            </fieldset>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="vac-sezione">
                <asp:Label id="lblRisultati" runat="server">Risultati della ricerca</asp:Label>
            </div>

            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">

                <asp:DataGrid id="dgrLog" runat="server" Width="100%" AutoGenerateColumns="False" AllowCustomPaging="true" AllowPaging="true" PageSize="25" >
					<AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
					<ItemStyle CssClass="item"></ItemStyle>
					<PagerStyle CssClass="pager" Position="TopAndBottom" Mode="NumericPages" />
                    <HeaderStyle CssClass="header"></HeaderStyle>
					<SelectedItemStyle CssClass="selected"></SelectedItemStyle>
					<Columns>
					    <asp:TemplateColumn Visible="false">
					        <ItemTemplate>
					            <asp:Label ID="lblCodicePaziente" runat="server" Text='<%# Eval("Paziente.paz_codice") %>' ></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
					    <asp:TemplateColumn Visible="false">
					        <ItemTemplate>
					            <asp:Label ID="lblIdLog" runat="server" Text='<%# Eval("Id") %>' ></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn>
                            <HeaderStyle Width="2%" />
                            <ItemStyle HorizontalAlign="Center" />
					        <ItemTemplate>
					            <asp:ImageButton ID="imgRedirectDatiPaziente" runat="server" CommandName="DatiPaziente" 
                                    ImageUrl="~/Images/utente.gif" ToolTip="Visualizza dati paziente" ></asp:ImageButton>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn>
					        <HeaderStyle Width="2%" />
					        <ItemTemplate>
					            <asp:ImageButton ID="imgDettaglioLog" runat="server" CommandName="ShowDettaglioLog"
                                    ImageUrl="~/Images/dettaglio.gif" ToolTip="Dettaglio del log" ></asp:ImageButton>
                            </ItemTemplate>
                        </asp:TemplateColumn>
					    <asp:TemplateColumn HeaderText="Cognome e Nome Paziente">
					        <HeaderStyle Width="18%" />
					        <ItemTemplate>
					            <asp:Label ID="lblCognomeNomePaziente" runat="server" Text='<%# Eval("Paziente.paz_cognome") + " " + Eval("Paziente.paz_nome") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
					    <asp:TemplateColumn HeaderText="Data Nascita" >
					        <HeaderStyle Width="9%" />
					        <ItemTemplate>
					            <asp:Label ID="lblDataNascitaPaziente" runat="server" Text='<%# String.Format("{0:dd/MM/yyyy}", Eval("Paziente.Data_Nascita")) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
					    <asp:TemplateColumn HeaderText="Stato Anagrafico" >
					        <HeaderStyle Width="10%" />
					        <ItemTemplate>
					            <asp:Label ID="lblStatoAnagraficoPaziente" runat="server" Text='<%# Eval("Paziente.StatoAnagrafico").ToString().Replace("_", " ") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
					    <asp:TemplateColumn HeaderText="C.V. Paz." >
					        <HeaderStyle Width="12%" />
					        <ItemTemplate>
					            <asp:Label ID="lblConsultorioPaziente" runat="server" Text='<%# Eval("Paziente.paz_cns_codice") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
						<asp:BoundColumn DataField="DataOperazione" HeaderText="Data Operazione" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}">
							<HeaderStyle Width="10%"></HeaderStyle>
						</asp:BoundColumn>
						<asp:BoundColumn DataField="Operazione" HeaderText="Operazione" >
							<HeaderStyle Width="9%"></HeaderStyle>
						</asp:BoundColumn>
                        <asp:TemplateColumn HeaderText="Argomento" >
					        <HeaderStyle Width="11%" />
					        <ItemTemplate>
					            <asp:Label ID="lblDescrizioneArgomento" runat="server" Text='<%# Eval("Argomento.Descrizione") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
						<asp:BoundColumn DataField="Note" HeaderText="Note">
							<HeaderStyle Width="15%"></HeaderStyle>
						</asp:BoundColumn>
                        <asp:TemplateColumn >
					        <HeaderStyle Width="2%" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <HeaderTemplate>
                                <asp:Label ID="lblHeaderRisultato" runat="server" Text="Ris." ToolTip="Risultato" ></asp:Label>
                            </HeaderTemplate>
					        <ItemTemplate>
                                <asp:Image ID="imgRisultato" runat="server" ToolTip='<%# Eval("Stato") %>'></asp:Image>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
				</asp:DataGrid>
            </dyp:DynamicPanel>
            
            <onitcontrols:OnitFinestraModale id="fmDettaglioLog" title="<div style=&quot;font-family:'Microsoft Sans Serif'; font-size: 11pt;padding-bottom:2px&quot;>&nbsp;Dettaglio Log</div>"
				runat="server" width="426px" BackColor="LightGray"  NoRenderX="False" RenderModalNotVisible="True">
                <div style="text-align:center; padding-top:5px;" >
			        <fieldset class="vacFieldset" style="width:400px;">
			            <legend style="width:75px; text-align: center; font-family:Verdana; font-size:12px">Azienda</legend>                            
                        <table cellpadding="2" cellspacing="0" class="dettaglioLog" style="margin-top:5px; margin-bottom:10px;width:100%" >
                            <colgroup>
                                <col width="25%" />
                                <col width="72%" />
                                <col width="3%" />
                            </colgroup>
                            <tr>
                                <td>Codice</td>
                                <td>
                                    <div class="labelLog">
                                        <asp:Label ID="lblDettaglioCodiceAzienda" runat="server" ></asp:Label>&nbsp;
                                    </div>
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td>Descrizione</td>
                                <td>
                                    <div class="labelLog">
                                        <asp:Label ID="lblDettaglioDescrizioneAzienda" runat="server"  ></asp:Label>&nbsp;
                                    </div>
                                </td>
                                <td></td>
                            </tr>
                        </table>
                    </fieldset>
                </div>
                <div style="text-align:center; padding-top:5px;" >
			        <fieldset class="vacFieldset" style="width:400px;">
			            <legend style="width:75px; text-align: center; font-family:Verdana; font-size:12px">Utente</legend>                            
                        <table width="100%" cellpadding="2" cellspacing="0" class="dettaglioLog" style="margin-top:5px; margin-bottom:10px;" >
                            <colgroup>
                                <col width="25%" />
                                <col width="72%" />
                                <col width="3%" />
                            </colgroup>                                                   
                            <tr>
                                <td>Codice</td>
                                <td>
                                    <div class="labelLog">
                                        <asp:Label ID="lblDettaglioCodiceUtente" runat="server"  ></asp:Label>&nbsp;
                                    </div>
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td>Descrizione</td>
                                <td>
                                    <div class="labelLog">
                                        <asp:Label ID="lblDettaglioDescrizioneUtente" runat="server" ></asp:Label>&nbsp;
                                    </div>
                                </td>
                                <td></td>
                            </tr>
                        </table>
                    </fieldset>
                </div>
            </onitcontrols:OnitFinestraModale>

        </on_lay3:OnitLayout3>
    </form>
</body>
</html>
