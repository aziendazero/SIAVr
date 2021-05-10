<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="VacEscluse.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_VacEscluse" %>

<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<%@ Register TagPrefix="uc1" TagName="InsVacEsc" Src="InsVacEsc.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ModDataVacEsc" Src="ModDataVacEsc.ascx" %>
<%@ Register TagPrefix="uc1" TagName="RinnovaEsc" Src="RinnovaEsc.ascx" %>
<%@ Register TagPrefix="uc1" TagName="StoricoRinnovi" Src="StoricoRinnovi.ascx" %>

<%@ Import Namespace="Onit.OnAssistnet.OnVac" %>
<%@ Import Namespace="Onit.OnAssistnet.OnVac.OnVacUtility" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Vaccinazioni Escluse</title>
    
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/toolbar.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/default/toolbar.default.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript">
        var gridIndex = new function () {
            this.ChkSeleziona = 0;
            this.BtnElimina = 1;
            this.BtnModificaConferma = 2;
            this.DettaglioRinnovi = 3;
            this.Vaccinazione = 4;
            this.Dose = 5;
            this.DataVisita = 6;
            this.Motivo = 7;
            this.Medico = 8;
            this.DataScadenza = 9;
            this.Note = 10;
            this.DescrizioneUslInserimento = 11;	 // visible = false
            this.FlagVisibilita = 12;
            this.FlagScaduta = 13;
            this.Rinnovo = 14;
            this.CodiceUslInserimento = 15;
        }

        function ToolBar_OnClientButtonClicking(sender, args) {
            if (!e) var e = window.event;

            var button = args.get_item();

            switch (button.get_value()) {
                case 'btn_Annulla':

                    if ("<%response.Write(OnitLayout31.Busy)%>" == "True") {
                        if (!confirm("Le modifiche effettuate andranno perse. Continuare?")) args.set_cancel(true);
                    }
                    else {
                        args.set_cancel(true);
                    }
                    break;

                case 'btn_Salva':

                    if ("<%response.Write(OnitLayout31.Busy)%>" != "True")
                        args.set_cancel(true);
                    break;
            }
        }

        function controlla(evt) {
            ev = SourceElement(evt);
            riga = ev.parentNode.parentNode.parentNode;
            tab = GetElementByTag(riga, 'TABLE', 1, 0, false);

            // Dose
            cell = tab.rows[riga.rowIndex].cells[gridIndex.Dose];
            elDesc = GetElementByTag(cell, 'INPUT', 1, 1, false);

            if (elDesc.value == "") {
                alert("Il campo 'Dose' è obbligatorio. Non è possibile aggiornare la riga.");
                evt.returnValue = false;
                StopPreventDefault(evt);
                elDesc.focus();
                return false;
            }

            // Motivo
            cell = tab.rows[riga.rowIndex].cells[gridIndex.Motivo];
            elDesc = GetElementByTag(cell, 'INPUT', 1, 1, false);

            if (elDesc.value == "") {
                alert("Il motivo di esclusione è obbligatorio. Non è possibile aggiornare la riga.");
                evt.returnValue = false;
                StopPreventDefault(evt);
                elDesc.focus();
                return false;
            }

            if ((elDesc.value != '') && (elDesc.nextSibling.value == '')) {
                alert("È necessario valorizzare correttamente il motivo di esclusione. Non è possibile aggiornare la riga.");
                evt.returnValue = false;
                StopPreventDefault(evt);
                elDesc.nextSibling.focus();
                return false;
            }

            // Data visita
            if (OnitDatePick.tb_data_visita_edit[0].Get() == "") {
                alert("Il campo 'Data Visita' è obbligatorio. Non è possibile aggiornare la riga.");
                OnitDatePick.tb_data_visita_edit[0].Focus(1, false);
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }

            dVis = new Array();
            dVis = OnitDatePick.tb_data_visita_edit[0].Get().split("/");

            if (new Date(dVis[2], dVis[1] - 1, dVis[0], 0, 0, 0) > new Date()) {
                alert("La data della visita non può essere futura. Non è possibile aggiornare la riga.");
                OnitDatePick.tb_data_visita_edit[0].Focus(1, true);
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }

            // Data scadenza
            dArr = new Array();
            dArr = OnitDatePick.tb_data_scadenza_edit[0].Get().split("/");

            if (new Date(dArr[2], dArr[1] - 1, dArr[0]) <= new Date(dVis[2], dVis[1] - 1, dVis[0], 0, 0, 0)) {
                alert("La data di scadenza non può essere inferiore alla visita. Non è possibile aggiornare la riga.");
                OnitDatePick.tb_data_scadenza_edit[0].Focus(1, true);
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }
        }

    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
    <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" TitleCssClass="Title3" Titolo="Vaccinazioni Escluse" Width="100%" Height="100%" HeightTitle="90px">
		<div class="title" id="divLayoutTitolo_sezione1" style="width: 100%">
			<asp:label id="LayoutTitolo" runat="server" Width="100%" BorderStyle="None" CssClass="title"></asp:label>
        </div>
        <div>
            <telerik:RadToolBar ID="Toolbar" runat="server" Width="100%" Skin="Default" EnableEmbeddedSkins="false" EnableAjaxSkinRendering="false" 
                EnableEmbeddedBaseStylesheet="false" OnClientButtonClicking="ToolBar_OnClientButtonClicking" OnButtonClick="Toolbar_ButtonClick">
                <Items>
                    <telerik:RadToolBarButton runat="server" Text="Salva" Value="btn_Salva" ImageUrl="~/Images/salva.gif" DisabledImageUrl="~/Images/salva_dis.gif" ToolTip="Salva le modifiche"></telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Text="Annulla" Value="btn_Annulla" ImageUrl="~/Images/annulla.gif" DisabledImageUrl="~/Images/annulla_dis.gif" ToolTip="Annulla le modifiche"></telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Text="Inserisci" Value="btn_Inserisci" ImageUrl="~/Images/nuovo.gif" DisabledImageUrl="~/Images/nuovo_dis.gif" ToolTip="Inserisce un'esclusione per il paziente"></telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Text="Imposta Scadenza" Value="btn_Modifica_Data_Scadenza" ImageUrl="~/Images/esclusioni_scadenza.png" DisabledImageUrl="~/Images/esclusioni_scadenza_dis.png" ToolTip="Modifica le date di scadenza delle vaccinazioni selezionate"></telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Text="Rinnova Esclusione" Value="btn_Rinnova_Esclusione" ImageUrl="~/Images/refresh.png" DisabledImageUrl="~/Images/refresh_dis.png" ToolTip="Rinnova Esclusione Scaduta"></telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Text="Recupera" Value="btnRecuperaStoricoVacc" ImageUrl="../../images/recupera.png" DisabledImageUrl="../../images/recupera_dis.png" ToolTip="Recupera lo storico vaccinale centralizzato del paziente"></telerik:RadToolBarButton>
                </Items>
            </telerik:RadToolBar>
        </div>
		<div class="vac-sezione">
			<asp:Label id="LayoutTitolo_sezione" runat="server" Text="ELENCO VACCINAZIONI ESCLUSE"></asp:Label>
        </div>
        <div id="divLegenda" class="legenda-vaccinazioni">
            <span class="legenda-vaccinazioni-esclusioneScaduta">S</span>
            <span>Esclusione scaduta</span>
            <span class="legenda-vaccinazioni-esclusioneRinnovata">R</span>
            <span>Esclusione rinnovata</span>
        </div>
        
        <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
			<asp:DataGrid id="dg_vacEx" style="padding: 0px;" runat="server" Width="100%" CssClass="datagrid" AutoGenerateColumns="False" CellPadding="1" GridLines="None">
				<SelectedItemStyle Font-Bold="True" CssClass="selected" VerticalAlign="Top"></SelectedItemStyle>
				<AlternatingItemStyle CssClass="alternating" VerticalAlign="Top"></AlternatingItemStyle>
				<ItemStyle CssClass="item" VerticalAlign="Top"></ItemStyle>
				<HeaderStyle CssClass="header"></HeaderStyle>
				<FooterStyle ForeColor="#4A3C8C" BackColor="#B5C7DE"></FooterStyle>
				<PagerStyle CssClass="pager" Mode="NumericPages"></PagerStyle>
				<Columns>
                     <asp:TemplateColumn>
                        <HeaderStyle Width="1%"/>
                        <ItemTemplate>
                            <asp:CheckBox id="chkSelezione" runat="server"/>
                        </ItemTemplate>
                    </asp:TemplateColumn>          
					<asp:ButtonColumn Text="&lt;img title=&quot;Elimina&quot; src=&quot;../../images/elimina.gif&quot;&gt;" CommandName="Delete">
						<HeaderStyle HorizontalAlign="Center" Width="1%"></HeaderStyle>
						<ItemStyle HorizontalAlign="Center"></ItemStyle>
					</asp:ButtonColumn>
					<asp:EditCommandColumn ButtonType="LinkButton" 
                        UpdateText="&lt;img  title=&quot;Conferma&quot; src=&quot;../../images/conferma.gif&quot; onclick='controlla(event)' &gt;"
						CancelText="&lt;img  title=&quot;Annulla&quot; src=&quot;../../images/annullaconf.gif&quot; &gt;"
						EditText="&lt;img  title=&quot;Modifica&quot; src=&quot;../../images/modifica.gif&quot;  &gt;">
						<HeaderStyle HorizontalAlign="Center" Width="1%"></HeaderStyle>
						<ItemStyle HorizontalAlign="Center"></ItemStyle>
					</asp:EditCommandColumn>
                    <asp:TemplateColumn>
						<HeaderStyle HorizontalAlign="Center" Width="1%"></HeaderStyle>
						<ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:ImageButton ID="btnDettaglioStoricoRinnovi" runat="server" ImageUrl="../../images/dettaglio.gif" CommandName="DettaglioStorico" ToolTip="Visualizza storico esclusioni rinnovate" />
                        </ItemTemplate>
                    </asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Vaccinazione">
						<HeaderStyle HorizontalAlign="Left" Width="15%"></HeaderStyle>
						<ItemStyle HorizontalAlign="Left"></ItemStyle>
						<ItemTemplate>
							<asp:Label id="tb_descVac" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("vac_descrizione") %>'></asp:Label>
                            &nbsp;-&nbsp;
                            <asp:Label id="tb_codVac" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("vex_vac_codice") %>'></asp:Label>
						</ItemTemplate>
					</asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Dose">
                        <HeaderStyle HorizontalAlign="Center" Width="6%" />
						<ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:Label ID="lblDose" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("vex_dose") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <on_val:OnitJsValidator id="txtDose" runat="server" CssClass="textbox_numerico_obbligatorio" Width="100%"
                                Text='<%# DataBinder.Eval(Container, "DataItem")("vex_dose") %>'
					            actionCorrect="False" actionDelete="False" actionFocus="True" actionMessage="True" actionSelect="False"
					            actionUndo="True" autoFormat="True" validationType="Validate_custom" CustomValFunction-Length="2" CustomValFunction="validaNumero"
					            SetOnChange="True" MaxLength="2"></on_val:OnitJsValidator>
                        </EditItemTemplate>
                    </asp:TemplateColumn>
					<asp:TemplateColumn HeaderText=" Data&lt;br&gt; Visita">
						<HeaderStyle HorizontalAlign="Center" Width="11%"></HeaderStyle>
						<ItemStyle HorizontalAlign="Center"></ItemStyle>
						<ItemTemplate>
							<asp:Label id="tb_data_visita" runat="server" Text='<%# CutTime(DataBinder.Eval(Container, "DataItem")("vex_data_visita")) %>'>
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<on_val:onitdatepick id="tb_data_visita_edit" runat="server" Width="100px" Height="22px" CssClass="TextBox_Data_Obbligatorio" Text='<%# CutTime(DataBinder.Eval(Container, "DataItem")("vex_data_visita")) %>' FormatoData="GeneralDate" Focus="False" DateBox="True" CalendarioPopUp="False" NoCalendario="True" Formatta="False" indice="-1" Hidden="False" ControlloTemporale="False" target="tb_data_visita_edit">
							</on_val:onitdatepick>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Motivazione">
						<HeaderStyle HorizontalAlign="Left" Width="15%"></HeaderStyle>
						<ItemStyle HorizontalAlign="Left"></ItemStyle>
						<ItemTemplate>
							<asp:Label id="tb_motivo" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("moe_descrizione") %>'>
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<on_ofm:onitmodallist id="fm_motivo_edit" runat="server" Width="100%" LabelWidth="-1px" PosizionamentoFacile="False" Obbligatorio="True" Codice='<%# DataBinder.Eval(Container, "DataItem")("vex_moe_codice") %>' Descrizione='<%# DataBinder.Eval(Container, "DataItem")("moe_descrizione") %>' RaiseChangeEvent="true" OnChange="fmMotivo_Change" SetUpperCase="True" UseCode="False" Tabella="t_ana_motivi_esclusione" Connection="" CampoDescrizione="moe_descrizione" CampoCodice="moe_codice" CodiceWidth="0%" Filtro="'true'='true' order by moe_descrizione">
							</on_ofm:onitmodallist>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Medico">
						<HeaderStyle HorizontalAlign="Left" Width="15%"></HeaderStyle>
						<ItemStyle HorizontalAlign="Left"></ItemStyle>
						<ItemTemplate>
							<asp:Label id="tb_medico" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("ope_nome") %>'>
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<on_ofm:onitmodallist id="fm_medico_edit" runat="server" Width="100%" Filtro="nvl(ope_obsoleto,'N')='N' order by ope_nome" CodiceWidth="0%" CampoCodice="ope_codice" CampoDescrizione="ope_nome" Connection="" Tabella="t_ana_operatori" UseCode="False" SetUpperCase="True" RaiseChangeEvent="False" Descrizione='<%# DataBinder.Eval(Container, "DataItem")("ope_nome") %>' Codice='<%# DataBinder.Eval(Container, "DataItem")("vex_ope_codice") %>' Obbligatorio="False" PosizionamentoFacile="False" LabelWidth="-1px">
							</on_ofm:onitmodallist>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Data&lt;br&gt; Scadenza">
						<HeaderStyle HorizontalAlign="Center" Width="11%"></HeaderStyle>
						<ItemStyle HorizontalAlign="Center"></ItemStyle>
						<ItemTemplate>
							<asp:Label id="tb_data_scadenza" runat="server" Text='<%# IIF(DataBinder.Eval(Container,  "DataItem")("vex_data_scadenza") is dbnull.value,"", CutTime(DataBinder.Eval(Container,  "DataItem")("vex_data_scadenza"))) %>'>
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<on_val:onitdatepick id="tb_data_scadenza_edit" runat="server" Width="103px" Height="22px" CssClass="TextBox_Data" Text='<%# IIF(DataBinder.Eval(Container,  "DataItem")("vex_data_scadenza") is dbnull.value,"", CutTime(DataBinder.Eval(Container,  "DataItem")("vex_data_scadenza"))) %>' FormatoData="GeneralDate" Focus="False" DateBox="True" CalendarioPopUp="True" NoCalendario="True" Formatta="False" indice="-1" Hidden="False" ControlloTemporale="False" target="tb_data_scadenza_edit">
							</on_val:onitdatepick>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Note">
						<HeaderStyle Width="12%" />
						<ItemTemplate>
							<asp:Label ID="lblNote" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("vex_note")%>'></asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox id="txtNote" TextMode="MultiLine" MaxLength="1000" Rows="5" style="overflow-y:auto; width:100%" 
                                CssClass="TextBox_Stringa" Text='<%# DataBinder.Eval(Container, "DataItem")("vex_note")%>' Runat="server"></asp:TextBox>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn SortExpression="usl_inserimento_vex_descr" HeaderText="<%$ Resources: Onit.OnAssistnet.OnVac.Web, DatiVaccinali.AslInserimentoVacc %>" >
                        <HeaderStyle HorizontalAlign="Left" />
						<ItemStyle width="8%" HorizontalAlign="Left" />
						<ItemTemplate>
							<asp:Label id="tb_usl_vex" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("usl_inserimento_vex_descr") %>'></asp:Label>
                            <asp:HiddenField ID="hdCodiceUslInserimento" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem")("vex_usl_inserimento") %>' />
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="" SortExpression="vex_flag_visibilita" >
						<HeaderStyle HorizontalAlign="Center" Width="1%"></HeaderStyle>
						<ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:Image ID="imgFlagVisibilita" runat="server" 
                                ImageUrl="<%# BindFlagVisibilitaImageUrlValue(Container.DataItem) %>" 
                                ToolTip="<%# BindFlagVisibilitaToolTipValue(Container.DataItem) %>" />
                        </ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle Width="1%"></HeaderStyle>
                        <ItemTemplate>
                            <asp:Label ID="Label14" runat="server" CssClass="legenda-vaccinazioni-esclusioneScaduta"
                                title='<%# OnVacUtility.LegendaVaccinazioniBindToolTip(Enumerators.LegendaVaccinazioniItemType.EsclusioneScaduta)%>'
                                Visible='<%# OnVacUtility.LegendaVaccinazioniBindVisibility(Eval("s"), Enumerators.LegendaVaccinazioniItemType.EsclusioneScaduta)%>'>S</asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:Label ID="Label25" runat="server" CssClass="legenda-vaccinazioni-esclusioneScaduta"
                                title='<%# OnVacUtility.LegendaVaccinazioniBindToolTip(Enumerators.LegendaVaccinazioniItemType.EsclusioneScaduta)%>'
                                Visible='<%# OnVacUtility.LegendaVaccinazioniBindVisibility(Eval("s"), Enumerators.LegendaVaccinazioniItemType.EsclusioneScaduta)%>'>S</asp:Label>
                        </EditItemTemplate>
					</asp:TemplateColumn>
                    <asp:TemplateColumn>
						<HeaderStyle Width="1%"></HeaderStyle>
						<ItemTemplate>
							<asp:Label id="lblRinnovo" runat="server" CssClass="legenda-vaccinazioni-esclusioneRinnovata" ToolTip="Esclusione rinnovata" >R</asp:Label>
						</ItemTemplate>
					</asp:TemplateColumn>
<%--                    <asp:ButtonColumn Text="&lt;img title=&quot;R&quot; src=&quot;../../images/dettaglio.gif&quot;&gt;" ItemStyle-CssClass="legenda_rinnovata" CommandName="DettaglioStorico" ButtonType="LinkButton">
						<HeaderStyle HorizontalAlign="Center" Width="1%"></HeaderStyle>
						<ItemStyle HorizontalAlign="Center"></ItemStyle>
					</asp:ButtonColumn>--%>

<%--                    <asp:TemplateColumn>
						<HeaderStyle HorizontalAlign="Left" Width="1%"></HeaderStyle>
						<ItemStyle HorizontalAlign="Left"></ItemStyle>
						<ItemTemplate>
							<asp:ImageButton id="imgRinnovo" runat="server" ImageUrl="~/Images/R.png" OnClick="imgRinnovo_Click"/>
						</ItemTemplate>
					</asp:TemplateColumn>--%>
                    <asp:BoundColumn Visible="false" DataField="vex_usl_inserimento" ></asp:BoundColumn>
                    <asp:BoundColumn Visible="false" DataField="vex_id_acn" ></asp:BoundColumn>
				</Columns>
			</asp:DataGrid>
        </dyp:DynamicPanel>

    </on_lay3:OnitLayout3>

    <on_ofm:OnitFinestraModale ID="modInserimentoEsclusioni" Title="Inserisci Esclusione" runat="server" Width="800px" Height="600px" BackColor="LightGray">
        <uc1:InsVacEsc ID="InsVacEscluse" runat="server" />
    </on_ofm:OnitFinestraModale>

    <on_ofm:OnitFinestraModale ID="modModificaDataScadenzaMultipla" Title="Modifica data scadenza" runat="server" Width="600px" Height="200px" BackColor="LightGray" NoRenderX="true">
            <uc1:ModDataVacEsc ID="ModDataVacEsc" runat="server" />
    </on_ofm:OnitFinestraModale>

    <on_ofm:OnitFinestraModale ID="modRinnovaEsclusione" Title="Rinnova Esclusione" runat="server" Width="800px" Height="550px" BackColor="LightGray" NoRenderX="true">
            <uc1:RinnovaEsc ID="RinnovaEsclusione" runat="server" />
    </on_ofm:OnitFinestraModale>

    <on_ofm:OnitFinestraModale ID="modStoricoRinnovi" Title="Storico Rinnovi" runat="server" Width="800px" Height="250px" BackColor="LightGray" NoRenderX="true">
        <uc1:StoricoRinnovi ID="StoricoRinnovi" runat="server" SoloRinnovati="true" />
    </on_ofm:OnitFinestraModale>

    </form>
    <script type="text/javascript" language="javascript">
    	<%response.write(strJS)%>
    </script>
    <!-- #include file="../../Common/Scripts/AggiornaStatoVaccinazioni.inc" -->
    <!-- #include file="../../Common/Scripts/ControlloDatiValidi.inc" -->
</body>
</html>
