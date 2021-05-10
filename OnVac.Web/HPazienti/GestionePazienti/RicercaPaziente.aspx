<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RicercaPaziente.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.RicercaPaziente" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>
<%@ Register TagPrefix="uc1" TagName="OnVacAlias" Src="OnVacAlias.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ConsensoUtente" Src="../../Common/Controls/ConsensoTrattamentoDatiUtente.ascx" %>

<%@ Import Namespace="Onit.OnAssistnet.OnVac" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>Ricerca Pazienti</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/toolbar.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/default/toolbar.default.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript" src="RicercaPaziente.js"></script>
</head>
<body>
    <script type="text/javascript" language="javascript">
        <%= HideLeftFrameIfNeeded() %>
    </script>
    <form id="Form1" method="post" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" TitleCssClass="title3" Titolo="Ricerca pazienti" Width="100%" Height="100%">
            <div>
                <telerik:RadToolBar ID="tlbRicerca" runat="server" Width="100%" Skin="Default" EnableEmbeddedSkins="false"
                    EnableAjaxSkinRendering="false" EnableEmbeddedBaseStylesheet="false"
                    OnButtonClick="tlbRicerca_ButtonClick" OnClientButtonClicking="toolbar_click">
                    <Items>
                        <telerik:RadToolBarButton Value="btnFind" Text="Cerca" ImageUrl="~/Images/cerca.gif"></telerik:RadToolBarButton>
                        <telerik:RadToolBarButton IsSeparator="true"></telerik:RadToolBarButton>
                        <telerik:RadToolBarButton Value="btnSeleziona" Text="Seleziona" ImageUrl="~/Images/conferma.gif"></telerik:RadToolBarButton>
                        <telerik:RadToolBarButton Value="btnPulisci" Text="Pulisci" ImageUrl="~/Images/Pulisci.gif" ToolTip="Resetta i filtri di ricerca"></telerik:RadToolBarButton>
                        <telerik:RadToolBarButton Value="btnNew" Text="Inserisci" ImageUrl="~/Images/nuovo.gif"></telerik:RadToolBarButton>
                        <telerik:RadToolBarButton IsSeparator="true" Text="sepRicercheRapide" Value="sepRicercheRapide"></telerik:RadToolBarButton>
                        <telerik:RadToolBarButton Value="btnUltimoPaz" Text="Ultimo Paziente" ImageUrl="~/Images/paziente.gif" ToolTip="Effettua la ricerca dell'ultimo paziente selezionato"></telerik:RadToolBarButton>
                        <telerik:RadToolBarButton Value="btnUltimaRicerca" Text="Ultima Ricerca" ImageUrl="~/Images/rieseguiRicerca.gif" ToolTip="Riesegue l'ultima ricerca effettuata"></telerik:RadToolBarButton>
                        <telerik:RadToolBarButton Value="btnAlias" Text="Merge" ImageUrl="../../images/alias.gif"></telerik:RadToolBarButton>
                        <telerik:RadToolBarButton IsSeparator="true" Value="sepConsenso"></telerik:RadToolBarButton>
                        <telerik:RadToolBarButton Value="btnConsenso" Text="Consensi" ImageUrl="~/Images/Consensi.gif" ToolTip="Apertura maschera di rilevazione del consenso"></telerik:RadToolBarButton>
                    </Items>
                </telerik:RadToolBar>
            </div>
            <div class="vac-sezione">Criteri di ricerca</div>
            <asp:Panel ID="pnlFiltri" runat="server" Style="padding: 3px 0 5px 0; background-color: whitesmoke;">
                <table id="tableFiltri" onkeyup="AvviaRicerca(event,this)" style="width: 99%;" cellspacing="0" cellpadding="1" border="0">
                    <colgroup>
                        <col width="13%" style="text-align: right" />
                        <col width="35%" />
                        <col width="12%" style="text-align: right" />
                        <col width="8%" />
                        <col width="10%" style="text-align: right" />
                        <col width="7%" />
                        <col width="5%" style="text-align: right" />
                        <col width="10%" />
                    </colgroup>
                    <tr id="trCodicePaziente" runat="server">
                        <td class="Label">
                            <asp:Label ID="lblPazCodice" runat="server">Codice Locale</asp:Label></td>
                        <td>
                            <asp:TextBox ID="txtPazCodice" runat="server" Width="130px"></asp:TextBox>
                        </td>
                        <td class="Label">
                            <asp:Label ID="lblPazCodiceAusiliario" runat="server">Codice Ausiliario</asp:Label></td>
                        <td colspan="5">
                            <asp:TextBox ID="txtPazCodiceAusiliario" runat="server" Width="130px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="Label">
                            <asp:Label ID="Label2" runat="server">Cognome</asp:Label></td>
                        <td>
                            <on_val:OnitJsValidator ID="txtCognome" runat="server" Width="100%" customvalfunction-length="20" AddToBatchValidation="True"
                                actionCorrect="False" actionDelete="False" actionFocus="True" actionMessage="True" actionSelect="True"
                                actionUndo="False" autoFormat="True" validationType="Validate_custom" CustomValFunction="validaCognomeNomeRic">
								<Parameters>
									<on_val:ValidationParam paramType="boolean" paramValue="true" paramOrder="0" paramName="blnUpper"></on_val:ValidationParam>
								</Parameters>
                            </on_val:OnitJsValidator>
                        </td>
                        <td class="Label">
                            <asp:Label ID="Label3" runat="server">Nome</asp:Label></td>
                        <td colspan="5">
                            <on_val:OnitJsValidator ID="txtNome" runat="server" customvalfunction-length="20" Width="100%" AddToBatchValidation="True"
                                actionCorrect="False" actionDelete="False" actionFocus="True" actionMessage="True" actionSelect="True"
                                actionUndo="False" autoFormat="True" validationType="Validate_custom" CustomValFunction="validaCognomeNomeRic">
								<Parameters>
									<on_val:ValidationParam paramType="boolean" paramValue="true" paramOrder="0" paramName="blnUpper"></on_val:ValidationParam>
								</Parameters>
                            </on_val:OnitJsValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="Label">
                            <asp:Label ID="Label6" runat="server">Comune Nascita</asp:Label></td>
                        <td>
                            <on_ofm:OnitModalList ID="fmComuneNascita" runat="server" Width="100%" CssClass="textbox_data" SetUpperCase="True"
                                CampoDescrizione="COM_DESCRIZIONE as DESCRIZIONE" CampoCodice="COM_CODICE as CODICE" Tabella="T_ANA_COMUNI" CodiceWidth="0px"
                                LabelWidth="-8px" PosizionamentoFacile="True" RaiseChangeEvent="False" DataTypeCode="Stringa" UseCode="False"
                                DataTypeDescription="Stringa" Obbligatorio="False" Filtro="1=1 ORDER BY DESCRIZIONE"></on_ofm:OnitModalList>
                        </td>
                        <td class="Label">
                            <asp:Label ID="Label4" runat="server">Sesso</asp:Label></td>
                        <td>
                            <asp:DropDownList ID="ddlSesso" runat="server" Width="100%">
                                <asp:ListItem Selected="True"></asp:ListItem>
                                <asp:ListItem Value="M">M</asp:ListItem>
                                <asp:ListItem Value="F">F</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td class="Label">
                            <asp:Label ID="Label5" runat="server">Data Nascita</asp:Label></td>
                        <td>
                            <on_val:OnitDatePick ID="odpDataNascita" runat="server" Height="22px" Width="120px" CssClass="TextBox_data" BorderColor="White"></on_val:OnitDatePick>
                        </td>
                        <td class="Label">
                            <asp:Label ID="lblAnnoNascita" runat="server">Anno</asp:Label></td>
                        <td>
                            <on_val:OnitJsValidator ID="txtAnnoNascita" runat="server" Width="100%" CssClass="textbox_stringa"
                                actionCorrect="False" actionDelete="False" actionFocus="False" actionMessage="True" actionSelect="True"
                                actionUndo="False" autoFormat="False" validationType="Validate_integer" PreParams-numDecDigit="0" PreParams-maxValue="3999"
                                PreParams-minValue="1800" MaxLength="4"></on_val:OnitJsValidator></td>
                    </tr>
                    <tr>
                        <td class="Label">
                            <asp:Label ID="Label7" runat="server">Codice Fiscale</asp:Label></td>
                        <td>
                            <on_val:OnitJsValidator ID="txtCodFiscale" runat="server" customvalfunction-length="14" Width="100%"
                                AddToBatchValidation="True" actionCorrect="False" actionDelete="False" actionFocus="True" actionMessage="True" actionSelect="True"
                                actionUndo="False" autoFormat="True" validationType="Validate_custom" CustomValFunction="validaSintaxCF"></on_val:OnitJsValidator>
                        </td>
                        <td class="Label">
                            <asp:Label ID="Label8" runat="server">Tessera Sanitaria</asp:Label></td>
                        <td colspan="5">
                            <asp:TextBox ID="txtTesseraSan" runat="server" Width="100%"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="Label">
                            <asp:Label ID="lblComuneResidenza" runat="server">Comune Residenza</asp:Label></td>
                        <td>
                            <on_ofm:OnitModalList ID="fmComuneResidenza" runat="server" Width="100%" CssClass="textbox_data" SetUpperCase="True"
                                CampoDescrizione="COM_DESCRIZIONE as DESCRIZIONE" CampoCodice="COM_CODICE as CODICE" Tabella="T_ANA_COMUNI" CodiceWidth="0px"
                                LabelWidth="-8px" PosizionamentoFacile="True" RaiseChangeEvent="False" UseCode="False" Obbligatorio="False"
                                Filtro="1=1 ORDER BY DESCRIZIONE"></on_ofm:OnitModalList>
                        </td>
                        <td class="Label">
                            <asp:Label ID="lblFiltroCns" runat="server">Centro Vaccinale</asp:Label></td>
                        <td colspan="5">
                            <on_ofm:OnitModalList ID="omlConsultorio" runat="server" Width="75%" CssClass="textbox_data" SetUpperCase="True"
                                CampoDescrizione="CNS_DESCRIZIONE" CampoCodice="CNS_CODICE" Tabella="T_ANA_CONSULTORI" Obbligatorio="False"
                                LabelWidth="-8px" PosizionamentoFacile="True" RaiseChangeEvent="False" UseCode="True" CodiceWidth="80px"></on_ofm:OnitModalList>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <div class="vac-sezione" id="sezioneRisultati">
                <asp:Label ID="lblRisultati" runat="server">Risultati della ricerca</asp:Label>
            </div>
            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                <on_dgr:OnitGrid ID="dgrPazienti" runat="server" Width="100%" PagerVoicesBefore="-1" PagerVoicesAfter="-1"
                    AllowSelection="true" EditMode="None" AutoGenerateColumns="False" sortAscImage="~/Images/ordAZ.gif"
                    sortDescImage="~/Images/ordZA.gif" SelectionOption="clientOnly" DataKeyField="ID">
                    <HeaderStyle CssClass="header"></HeaderStyle>
                    <FooterStyle CssClass="footer"></FooterStyle>
                    <PagerStyle CssClass="pager"></PagerStyle>
                    <ItemStyle CssClass="item"></ItemStyle>
                    <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
                    <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
                    <EditItemStyle CssClass="edit"></EditItemStyle>
                    <Columns>
                        <on_dgr:OnitMultiSelColumn key="chkColumn"></on_dgr:OnitMultiSelColumn>
                        <on_dgr:OnitTemplateColumn key="Fonte" HeaderText="Fonte" Visible="false">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Label ID="lblFonte" runat="server" Text='<%# GetValoreFonte(Eval("Fonte")) %>' CssClass='<%# GetCssClassFonte(Eval("Fonte")) %>'></asp:Label>
                            </ItemTemplate>
                        </on_dgr:OnitTemplateColumn>
                        <on_dgr:OnitTemplateColumn key="StatoConsenso" HeaderText="Cons.">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Image ID="imgStatoConsenso" runat="server" />
                            </ItemTemplate>
                        </on_dgr:OnitTemplateColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="CodiceLocale" HeaderText="Codice" key="Codice"></on_dgr:OnitBoundColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="CodiceCentrale" HeaderText="Cod. Centrale" key="Ausiliario"></on_dgr:OnitBoundColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="CodiceRegionale" HeaderText="Cod. Regionale" key="Regionale"></on_dgr:OnitBoundColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="Cognome" HeaderText="Cognome" key="Cognome"></on_dgr:OnitBoundColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="Nome" HeaderText="Nome" key="Nome"></on_dgr:OnitBoundColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="Sesso" HeaderText="Sesso" key="Sesso"></on_dgr:OnitBoundColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="DataNascita" HeaderText="Data nascita" key="DataNascita" DataFormatString="{0:dd/MM/yyyy}"></on_dgr:OnitBoundColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="ComuneNascita" HeaderText="Comune nascita" key="ComuneNascita"></on_dgr:OnitBoundColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="CodiceFiscale" HeaderText="Codice fiscale" key="Fiscale"></on_dgr:OnitBoundColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="Tessera" HeaderText="Tessera" key="Tessera"></on_dgr:OnitBoundColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="ComuneResidenza" HeaderText="Comune res." key="ComuneResidenza"></on_dgr:OnitBoundColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="IndirizzoResidenza" HeaderText="Indirizzo res." key="IndirizzoResidenza"></on_dgr:OnitBoundColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="CodiceCentroVaccinale" HeaderText="Centro Vaccinale"></on_dgr:OnitBoundColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="StatoAnagrafico" HeaderText="Stato Anag."></on_dgr:OnitBoundColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="CodiceUslAssistenza" HeaderText="ULSS" key="ULSS"></on_dgr:OnitBoundColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="CodiceUslDomicilio" HeaderText="ULLSD" key="ULSSD"></on_dgr:OnitBoundColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="PazTipo" HeaderText="Paz. tipo"></on_dgr:OnitBoundColumn>
                        <on_dgr:OnitTemplateColumn key="Cancellato" HeaderText="Canc" Visible="False">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Label ID="lblCanc" runat="server" Text='<%# IIf(Eval("Cancellato") = "True", "S", "N") %>'></asp:Label>
                            </ItemTemplate>
                        </on_dgr:OnitTemplateColumn>
                        <on_dgr:OnitTemplateColumn key="Vaccinazioni" HeaderText="V">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Image ID="imgEseguite" runat="server"
                                    Visible='<%# (If(Eval("VaccinazioniEseguite"), 0) > 0) %>'
                                    ImageUrl="~/images/flagVacEseguite.png" ToolTip='<%# String.Format(IIf(Eval("VaccinazioniEseguite") <> 1, "Sono presenti {0} vaccinazioni eseguite", "E&#39; presente {0} vaccinazione eseguita"), If(Eval("VaccinazioniEseguite"), 0))  %>' />
                            </ItemTemplate>
                        </on_dgr:OnitTemplateColumn>
                        <on_dgr:OnitTemplateColumn key="Appuntamenti" HeaderText="A">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Image ID="imgAppuntamenti" runat="server"
                                    Visible='<%# (If(Eval("Appuntamenti"), 0) > 0) %>'
                                    ImageUrl="~/images/flagAppuntamenti.png" ToolTip='<%# String.Format(IIf(Eval("Appuntamenti") <> 1, "Sono presenti {0} appuntamenti", "E&#39; presente {0} appuntamento"), If(Eval("Appuntamenti"),0)) %>' />
                            </ItemTemplate>
                        </on_dgr:OnitTemplateColumn>
                        <on_dgr:OnitTemplateColumn key="Escluse" HeaderText="E">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Image ID="imgEscluse" runat="server"
                                    Visible='<%#(If(Eval("VaccinazioniEscluse"), 0) > 0) %>'
                                    ImageUrl="~/images/flagVacEscluse.png" ToolTip='<%# String.Format(IIf(Eval("VaccinazioniEscluse") <> 1, "Sono presenti {0} esclusioni", "E&#39; presente {0} esclusione"), If(Eval("VaccinazioniEscluse"),0)) %>' />
                            </ItemTemplate>
                        </on_dgr:OnitTemplateColumn>
                    </Columns>
                </on_dgr:OnitGrid>
            </dyp:DynamicPanel>
        </on_lay3:OnitLayout3>

        <!-- modale alias -->
        <on_ofm:OnitFinestraModale ID="fmOnVacAlias" Title="Scelta dell'Anagrafica Corretta" runat="server" Width="850px" Height="300px" BackColor="LightGray" NoRenderX="False">
            <uc1:OnVacAlias ID="OnVacAlias1" runat="server"></uc1:OnVacAlias>
        </on_ofm:OnitFinestraModale>

        <!-- modale consenso -->
        <on_ofm:OnitFinestraModale ID="modConsenso" Title="Consenso" runat="server" Width="820px" Height="570px" BackColor="LightGray" ClientEventProcs-OnClose="RefreshFromPopup()">
            <iframe id="frameConsenso" runat="server" style="width: 815px; height: 545px; background-color: white;">
                <div style="text-align: center; font-family: Verdana; font-size: 18px;">
                    Caricamento in corso. Attendere...
                </div>
            </iframe>
        </on_ofm:OnitFinestraModale>
        
        <!-- consenso trattamento dati utente -->
        <on_ofm:OnitFinestraModale ID="fmConsensoUtente" Title="Consenso al trattamento dati per l'utente" runat="server" Width="400px" Height="250px" BackColor="LightGoldenrodYellow">
            <uc1:ConsensoUtente runat="server" id="ucConsensoUtente"></uc1:ConsensoUtente>
        </on_ofm:OnitFinestraModale>

    </form>

</body>
</html>

