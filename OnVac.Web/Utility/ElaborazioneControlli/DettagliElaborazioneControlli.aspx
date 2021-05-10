<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DettagliElaborazioneControlli.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.DettagliElaborazioneControlli" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register Assembly="Onit.Web.UI.WebControls.DynamicPanel" Namespace="Onit.Web.UI.WebControls.DynamicPanel" TagPrefix="dyp" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_ocb" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitCombo" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>

<!DOCTYPE html>

<html>
<head>
    <title>Elaborazione Controlli</title>
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <style type="text/css">
        .divFieldSet {
            padding-top: 5px;
            padding-left: 5px;
        }
        .box_left, .box_right, .box_center {
            font-family: Arial;
            font-size: 12px;
            border: 1px solid black ;
            width: 100%;
            padding: 1px;
            background-color: aliceblue;
        }
        .box_left {
            text-align: left;
            padding-left: 3px;
        }
        .box_right {
            text-align: right;
        }
        .box_center {
            text-align: center;
        }
        .vac-sezione {
            padding-top: 2px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Height="100%" Width="100%" TitleCssClass="Title3" Titolo="Dettaglio elaborazione controlli">
            <div class="Title" id="divLayoutTitolo" style="width: 100%">
                <asp:Label id="LayoutTitolo" runat="server" CssClass="Title">Dettaglio elaborazione controlli</asp:Label>
            </div>
            <div>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="tlbElaborazione" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                    <Items>
                        <igtbar:TBarButton Key="btnIndietro" Text="Indietro" DisabledImage="~/Images/indietro_dis.gif" Image="~/Images/indietro.gif"
                    ToolTip="Torna all'elenco dei processi">
                        </igtbar:TBarButton>
                        <igtbar:TBSeparator></igtbar:TBSeparator>
				        <igtbar:TBarButton Key="btnCerca" Text="Cerca" DisabledImage="~/Images/cerca_dis.gif" Image="~/Images/cerca.gif"
                            ToolTip="Effettua la ricerca in base filtri impostati">
				        </igtbar:TBarButton>
                        <igtbar:TBSeparator></igtbar:TBSeparator>
				        <igtbar:TBarButton Key="btnExportExcelCentro" Text="Export Centro " DisabledImage="~/Images/exportExcel.png" Image="~/Images/exportExcel.png" 
                    ToolTip="Esporta i risultati in formato excel per il centro">
                    <DefaultStyle CssClass="infratoolbar_button_default" Width="110px"></DefaultStyle>
				</igtbar:TBarButton>
				        <igtbar:TBarButton Key="btnExportExcelScuola" Text="Export Scuola" DisabledImage="~/Images/exportExcel.png" Image="~/Images/exportExcel.png" 
                    ToolTip="Esporta i risultati in formato excel per la scuola">
                    <DefaultStyle CssClass="infratoolbar_button_default" Width="110px"></DefaultStyle>
				</igtbar:TBarButton>
                    </Items>
                </igtbar:UltraWebToolbar>
            </div>
            <div class="vac-sezione">Dati del processo</div>
             
            <asp:Panel id="pnldati" runat="server" Width="100%" Height="100px" style="padding-top:3px; border-bottom:1px solid #485d96; background-color: whitesmoke">
                <table style="width: 100%" cellpadding="2" cellspacing="0" border="0">
                    <colgroup>
                        <col style="width: 15%" />
                        <col style="width: 15%" />
                        <col style="width: 19%" />
                        <col style="width: 15%" />
                        <col style="width: 19%" />
                        <col style="width: 15%" />
                        <col style="width: 2%" />
                    </colgroup>
                    <tr>
                        <td class="label_right">Id caricamento</td>
                        <td>
                            <div class="box_left" style="height:14px">
                                <asp:Label ID="lblIdCaricamento" runat="server"></asp:Label>
                            </div>
                        </td>
                        <td class="label_right">File caricato</td>
                        <td colspan="3">
                            <div class="box_left" style="height:14px">
                                <asp:Label ID="lblFileCaricamento" runat="server" CssClass="Label"></asp:Label>
                            </div>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td class="label_right">Data controllo</td>
                        <td >
                            <div class="box_left" style="height:14px">
                                <asp:Label ID="lblDataControllo" runat="server" ></asp:Label>
                            </div>
                        </td>
                        <td class="label_right">Utente Controllo</td>
                        <td >
                            <div class="box_left" style="height:14px">
                                <asp:Label ID="lblCodiceUtente" runat="server"></asp:Label>
                            </div>
                             <td class="label_right">Stato controllo</td>
                        <td>
                            <div class="box_left" style="height:14px">
                                <asp:Label ID="lblStatoControllo" runat="server"></asp:Label>
                            </div>
                        </td>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td class="label_right">Record caricati</td>
                        <td>
                            <div class="box_left" style="height:14px">
                                <asp:Label ID="lblcaricati" runat="server"></asp:Label>
                            </div>
                        </td>
                        <td class="label_right">Record controllati</td>
                        <td>
                            <div class="box_left" style="height:14px">
                                <asp:Label ID="lblControllati" runat="server"></asp:Label>
                            </div>
                        </td>
                        <td class="label_right">Record in errore</td>
                        <td>
                            <div class="box_left" style="height:14px">
                                <asp:Label ID="lblErr" runat="server"></asp:Label>
                            </div>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td class="label_right">N. vaccinati/immuni</td>
                        <td>
                            <div class="box_left" style="height:14px">
                                <asp:Label ID="lblvaccinati" runat="server"></asp:Label>
                            </div>
                        </td>
                        <td class="label_right">N. non vaccinati</td>
                        <td>
                            <div class="box_left" style="height:14px">
                                <asp:Label ID="lblNonVaccinati" runat="server"></asp:Label>
                            </div>
                        </td>
                        <td class="label_right">Copertura</td>
                        <td >
                            <div class="box_left" style="height:14px">
                                <asp:Label ID="lblCopertura" runat="server"></asp:Label>
                            </div>
                        </td>
                        <td></td>
                    </tr>
                
                </table>
            </asp:Panel>
            <div style="width:100%; text-align:center; margin:5px;" >
                <fieldset class="vacFieldset" title="Filtri di estrazione">
                    <legend class="Label" style="text-align: left; font-family: Arial; font-size: 12px;">Filtri Controlli</legend>
                    <table width="100%" cellpadding="2" cellspacing="0" border="0">
				        <colgroup>
                            <col width="15%" />
                            <col width="30%" />
                            <col width="7%" />
                            <col width="15%" />
                            <col width="4%" />
                            <col width="27%" />
                            <col width="2%" />
				        </colgroup>
                        <tr>
                            <td class="label_right">Stato</td>
					        <td>
                                <on_ocb:OnitCombo ID="cmbStatoElaborazione" runat="server" IncludeNull="true" CssClass="textbox_stringa" Width="100%">
                                </on_ocb:OnitCombo>
					        </td>
                            <td style="text-align:right">
                                <img src='<%= StatoElabConErrore.UrlIcona%>' alt='<%= StatoElabConErrore.ToolTip%>' title='<%= StatoElabConErrore.ToolTip%>' />
                            </td>
                            <td class="label_left">
                                <asp:Label ID="lblStatoElabConErrore" runat="server" CssClass="label_left"></asp:Label>
                            </td>
                            <td style="text-align:right">
                                <img src='<%= StatoInRegola.UrlIcona%>' alt='<%= StatoInRegola.ToolTip%>' title='<%= StatoInRegola.ToolTip%>' />
                            </td>
                            <td class="label_left">
                                <asp:Label ID="lblStatoInRegola" runat="server" CssClass="label_left"></asp:Label>
                            </td>
                        
                            <td></td>
				        </tr>
                        <tr>
                            <td></td>
                            <td></td>
                            <td style="text-align:right">
                                <img src='<%= StatoParzialmenteInRegola.UrlIcona%>' alt='<%= StatoParzialmenteInRegola.ToolTip%>' title='<%= StatoParzialmenteInRegola.ToolTip%>' />
                            </td>
                            <td class="label_left">
                                <asp:Label ID="lblStatoParzialmenteInRegola" runat="server" CssClass="label_left"></asp:Label>
                            </td>
                            <td style="text-align:right">
                                <img src='<%= StatoNonInRegola.UrlIcona%>' alt='<%= StatoNonInRegola.ToolTip%>' title='<%= StatoNonInRegola.ToolTip%>' />
                            </td>
                            <td class="label_left">
                                <asp:Label ID="lblStatoNonInRegola" runat="server" CssClass="label_left"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                </fieldset>
            </div>      
            <div class="vac-sezione">Risultati della ricerca</div>
            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" ExpandDirection="horizontal">
                 <asp:DataGrid ID="dgrElaborazioneControlli" runat="server" Width="100%" AutoGenerateColumns="False"
                    AllowCustomPaging="true" AllowPaging="true" PageSize="25" AllowSorting="False">
                    <AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
                    <ItemStyle CssClass="Item"></ItemStyle>
                    <PagerStyle CssClass="Pager" Position="TopAndBottom" Mode="NumericPages" />
                    <HeaderStyle CssClass="Header"></HeaderStyle>
                    <SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
                    <Columns>
                        <asp:TemplateColumn>
                            <HeaderStyle Width="2%" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Image ID="imgStatoElaborazione" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Paziente" >
                            <HeaderStyle Width="17%" HorizontalAlign="Left" />
                            <ItemTemplate>
                                <asp:LinkButton id="lnkCognomeNome" runat="server" Text='<%# Eval("Nominativo").ToString()  %>' 
                                    ToolTip="Visualizza dati paziente" OnClick="lnkCognomeNome_Click">
                                </asp:LinkButton>
                            </ItemTemplate>    
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="Sesso" HeaderText="Sesso " >
                            <HeaderStyle Width="8%" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:BoundColumn>
                        <asp:TemplateColumn HeaderText="Data Nascita" >
                            <HeaderStyle Width="12%" HorizontalAlign="Left" />
                            <ItemTemplate>
                                <asp:Label ID="lblDataNascita" CssClass="label_left" runat="server" Text='<%# GetData(Eval("DataDiNascita"), False)%>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="CodiceFiscale" HeaderText="Codice Fiscale">
                            <HeaderStyle Width="15%" HorizontalAlign="Left" />
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="EsitoControllo" HeaderText="Esito Controllo">
                            <HeaderStyle Width="15%" HorizontalAlign="Left" />
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="Errore" HeaderText="Errore">
                            <HeaderStyle Width="13%" HorizontalAlign="Left" />
                        </asp:BoundColumn>
                       <%-- <asp:BoundColumn DataField="Dose" HeaderText="Dose <img id='imgDose' alt='' src='../../images/transparent16.gif' />"
                            SortExpression="Dose">
                            <HeaderStyle Width="7%" HorizontalAlign="Left" />
                        </asp:BoundColumn>                        
                        <asp:TemplateColumn HeaderText="Errore <img id='imgErr' alt='' src='../../images/transparent16.gif' />" SortExpression="Errore">
                            <HeaderStyle Width="9%" HorizontalAlign="Left" />
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkErrore" runat="server" CssClass="label_left" Text='<%# GetErrore(Eval("Errore"))%>' OnClick="lnkErrore_Click" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Gen <img id='imgGen' alt='' src='../../images/transparent16.gif' />" SortExpression="DataGenerazione" >
                            <HeaderStyle Width="6%" HorizontalAlign="Left" />
                            <ItemTemplate>
                                <asp:Image ID="imgGenerazioneFile" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateColumn>--%>

                        <asp:BoundColumn DataField="PazCodice" Visible="false"></asp:BoundColumn>
                        <asp:BoundColumn DataField="CocId" Visible="false"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Stato" Visible="false"></asp:BoundColumn>

                    </Columns>
                </asp:DataGrid>
            </dyp:DynamicPanel>
        </on_lay3:OnitLayout3>
    </form>
</body>
</html>
