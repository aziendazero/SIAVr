<%@ Page Language="vb" AutoEventWireup="false" Codebehind="parametri.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Parametri" %>

<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title>Parametri</title>

        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

		<script type="text/javascript" language="javascript">
		
		    function InizializzaToolBar(t) {
		        t.PostBackButton = false;
		    }

		    function ToolBarClick(ToolBar, button, evnt) {
		        evnt.needPostBack = true;

		        switch (button.Key) {
		            case 'btn_Annulla':

		                if ("<%response.write(onitlayout31.busy)%>" == "True") {
		                    var ret;
		                    ret = confirm("Le modifiche effettuate andranno perse. Continuare?");
		                    if (ret == true) {
		                        window.location.href = "Consultori.aspx?RicaricaDati=True";
		                        evnt.needPostBack = false;
		                    }
		                }
		                else
		                    window.location.href = "Consultori.aspx?RicaricaDati=True";
		                evnt.needPostBack = false;
		                break;

		            case 'btn_Salva':

		                if ("<%response.write(onitlayout31.busy)%>" != "True") {
		                    evnt.needPostBack = false;
		                }
		                break;
		            }
		    }
			
		    function controlla(evt) {
		        // riga=event.srcElement.parentNode.parentNode.parentNode;
		        el = SourceElement(evt);
		        riga = el.parentNode.parentNode.parentNode;
		        tab = GetElementByTag(riga, 'TABLE', 1, 0, false);
		        //alert(tab);
		        cell = tab.rows[riga.rowIndex].cells[2];
		        el = GetElementByTag(cell, 'INPUT', 1, 1, false);
		        //alert("valore: " + el.value);
		        if ("<%response.write(codConsultorio)%>" != "") {
		            //el=riga.childNodes[2].firstChild.firstChild;
		            if (el.value == "") {
		                alert("I campi 'Descrizione-Codice' sono vuoti.Non è possibile aggiornare la tabella!");
		                el.focus();
		                // event.returnValue=false; (versione precedente)
		                StopPreventDefault(evt);
		                return false;
		            }
		        }
		        if ("<%response.write(codConsultorio)%>" != "") {
		            // el=riga.childNodes[3].firstChild;
		            cell = tab.rows[riga.rowIndex].cells[3];
		            el = GetElementByTag(cell, 'INPUT', 1, 1, false);
		            //alert("prova valore: " + el.value);
		        }
		        else {
		            // el=riga.childNodes[2].firstChild;
		            cell = tab.rows[riga.rowIndex].cells[2];
		            el = GetElementByTag(cell, 'INPUT', 1, 1, false);
		            //alert("prova valore: " + el.value);
		        }
		        //if (el.value == "") {
		        //    alert("Il  campo 'Valore' è vuoto.Non è possibile aggiornare la tabella!");
		            el.focus();
		            // event.returnValue=false; (vesione precedente)
		        //    StopPreventDefault(evt);
		        //    return false;
		        }
		        if (el.value.indexOf("|") > 0) {
		            alert("Il  campo 'valore' non può contenere il carattere '|'. Non è possibile aggiornare la tabella!");
		            el.focus();
		            // event.returnValue=false; (versione precedente)
		            StopPreventDefault(evt);
		            return false;
		        }
		        if (el.value.length > 256) {
		            alert("Il  campo 'valore' non può contenere più di 256 caratteri. Non è possibile aggiornare la tabella!");
		            el.focus();
		            // event.returnValue=false; (versione precedente)
		            StopPreventDefault(evt);
		            return false;
		        }
		    }
	
		    function ritorno(evt) {
		        evt.returnValue = true;
		    }
        </script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" height="100%" width="100%">
				<div class="title" id="divLayoutTitolo_sezione1" style="width: 100%">
					<asp:Label id="LayoutTitolo" runat="server" CssClass="title" Width="100%" BorderStyle="None" ></asp:Label>
                </div>
                <div>
					<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		                <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
						<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
						<Items>
							<igtbar:TBarButton Text="Salva" Key="btn_Salva" DisabledImage="~/Images/salva.gif" Image="~/Images/salva.gif"></igtbar:TBarButton>
							<igtbar:TBarButton Text="Annulla" Key="btn_Annulla" DisabledImage="~/Images/annulla.gif" Image="~/Images/annulla.gif"></igtbar:TBarButton>
						</Items>
					</igtbar:UltraWebToolbar>
                </div>
				<div class="sezione" id="divLayoutTitolo_sezione" style="width: 100%">
					<asp:Label id="LayoutTitolo_sezioneCnv" runat="server" Width="100%">ELENCO PARAMETRI</asp:Label>
                </div>
                <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
					<asp:Panel id="pan_par" runat="server" Width="100%">
						<asp:datagrid id="dg_par" style="table-layout: fixed" runat="server" CssClass="datagrid" Width="100%"
							GridLines="None" CellPadding="1" AutoGenerateColumns="False">
							<SelectedItemStyle Font-Bold="True" CssClass="Selected"></SelectedItemStyle>
							<EditItemStyle Wrap="False" CssClass="edit"></EditItemStyle>
							<AlternatingItemStyle Wrap="False" CssClass="alternating"></AlternatingItemStyle>
							<ItemStyle Wrap="False" CssClass="item"></ItemStyle>
							<HeaderStyle Font-Bold="True" CssClass="header"></HeaderStyle>
							<PagerStyle CssClass="pager" Mode="NumericPages"></PagerStyle>
							<Columns>
								<asp:ButtonColumn Text="&lt;img  title=&quot;Elimina&quot; src=&quot;../../../images/elimina.gif&quot;  onclick=&quot;ritorno(event)&quot;&gt;"
									CommandName="Delete">
									<HeaderStyle HorizontalAlign="Center" Width="20px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
								</asp:ButtonColumn>
								<asp:EditCommandColumn ButtonType="LinkButton" UpdateText="&lt;img  title=&quot;Conferma&quot; src=&quot;../../../images/conferma.gif&quot; onclick=&quot;controlla(event)&quot;&gt;"
									CancelText="&lt;img  title=&quot;Annulla&quot;src=&quot;../../../images/annullaconf.gif&quot;&gt;"
									EditText="&lt;img  title=&quot;Modifica&quot; src=&quot;../../../images/modifica.gif&quot;&gt;">
									<HeaderStyle HorizontalAlign="Center" Width="20px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
								</asp:EditCommandColumn>
								<asp:TemplateColumn>
									<HeaderStyle HorizontalAlign="Center" Width="80%"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
									<HeaderTemplate>
										<table id="Table7" cellspacing="0" cellpadding="0" width="100%" border="0">
											<tr>
												<td align="center" width="70%">Descrizione</td>
												<td align="center" width="30%">Codice</td>
											</tr>
										</table>
									</HeaderTemplate>
									<ItemTemplate>
										<table id="Table6" cellspacing="0" cellpadding="0" width="100%" border="0">
											<tr>
												<td width="70%">
													<asp:Label id="tb_descrizione" title='<%# DataBinder.Eval(Container, "DataItem")("par_descrizione") %>' runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("par_descrizione") %>'>
													</asp:Label></td>
												<td width="30%">
													<asp:Label id="tb_codice" title='<%# DataBinder.Eval(Container, "DataItem")("par_codice") %>' runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("par_codice") %>'>
													</asp:Label></td>
											</tr>
										</table>
									</ItemTemplate>
									<EditItemTemplate>
										<on_ofm:onitmodallist id=fm_par runat="server" Width="70%" UseTableLayout="True" RaiseChangeEvent="False" Codice='<%# DataBinder.Eval(Container, "DataItem")("par_codice") %>' Descrizione='<%# DataBinder.Eval(Container, "DataItem")("par_descrizione") %>' SetUpperCase="True" Obbligatorio="True" UseCode="True" Tabella="t_ana_parametri" Connection="" CampoDescrizione="par_descrizione" CampoCodice="par_codice" CodiceWidth="30%" LabelWidth="-1px" PosizionamentoFacile="False" ToolTip='<%# DataBinder.Eval(Container, "DataItem")("par_descrizione").tostring & vbcrlf & DataBinder.Eval(Container, "DataItem")("par_codice").tostring %>'>
										</on_ofm:onitmodallist>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Valore">
									<HeaderStyle HorizontalAlign="Center" Width="20%"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
									<ItemTemplate>
										<asp:Label id=tb_valore runat="server" title='<%# DataBinder.Eval(Container, "DataItem")("par_valore") %>' Text='<%# DataBinder.Eval(Container, "DataItem")("par_valore") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=tb_valore_edit runat="server" Width="100%" CssClass="textbox_stringa_obbligatorio" Text='<%# DataBinder.Eval(Container, "DataItem")("par_valore") %>' title='<%# DataBinder.Eval(Container, "DataItem")("par_valore") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn Visible="False" DataField="par_centrale" HeaderText="Centrale">
									<HeaderStyle Width="0%"></HeaderStyle>
								</asp:BoundColumn>
							</Columns>
						</asp:datagrid>
						<asp:Label id="lb_warning" runat="server" Width="100%" BackColor="#E7E7FF" BorderStyle="Solid"
							Font-Size="12px" Font-Names="ARIAL" Height="35px" Font-Bold="True" BorderWidth="1px" BorderColor="Black"
							Visible="False"></asp:Label>
					</asp:Panel>
                </dyp:DynamicPanel>
			</on_lay3:onitlayout3>
		</form>
		<% response.write(strJS) %>
	</body>
</html>
