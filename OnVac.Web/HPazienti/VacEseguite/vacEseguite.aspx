<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="vacEseguite.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_VacEseguite" %>

<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators"  %>
<%@ Register TagPrefix="on_ocb" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitCombo" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>

<%@ Register TagPrefix="uc1" TagName="ReazAvverseDetail" Src="../../common/Controls/ReazAvverseDetail.ascx" %>
<%@ Register TagPrefix="uc2" TagName="VacEs" Src="../../common/Controls/ElencoVaccinazioniEseguite.ascx" %>
<%@ Register TagPrefix="uc3" TagName="SelezioneAmbulatorio" Src="../../common/Controls/SelezioneAmbulatorio.ascx" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="Onit.OnAssistnet.OnVac" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Eseguite</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/toolbar.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/default/toolbar.default.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript" src='<%= OnVacUtility.GetScriptUrl("onvac.common.js") %>'></script>
    <script type="text/javascript" src='<%= OnVacUtility.GetScriptUrl("scriptReazioniAvverse.js") %>'></script>
    <script type="text/javascript">

        function controlla()
        {
        }

        function InizializzaToolBarDetail(t)
        {
	        t.PostBackButton=false;
        }
		
        function ToolBarDetailClick(ToolBar,button,evnt)
        {
            evnt.needPostBack = true;
	
            switch (button.Key)
            {
                case 'btn_Indietro':

                    if (!confirm("Le modifiche apportate non saranno salvate. Continuare?")) evnt.needPostBack = false;
                    break;


                case 'btn_Conferma':

                    if (!CheckDettaglioReazione('<%=ReazAvverseDetail.GetDataOraVaccinazioneString()%>', <%= ReazAvverseDetail.IsAltraReazioneObbligatoria().ToString().ToLower() %>)) evnt.needPostBack = false;

                    break;	
			
		        default:
		            evnt.needPostBack = true;
		            break; 
	        }
        }

        function OnClientButtonClicking(sender, args) 
        {
            if (!e) var e = window.event;

            var button = args.get_item();
            switch (button.get_value())	
	        {

		        case 'btn_Annulla':
			        if("<%response.write(onitlayout31.busy)%>"=="True")
                    {
                        if (!confirm("Le modifiche effettuate andranno perse. Continuare?")) args.set_cancel(true);
                    }

			        break;
			
		        case 'btn_Salva':
			        if("<%response.write(onitlayout31.busy)%>"!="True")
				        args.set_cancel(true);
			        break;
		
		        case 'btnScaduta':
			        if (!confirm('Attenzione: si è sicuri di voler far scadere le associazioni selezionate?'))
				        args.set_cancel(true);
			        break;
			
		        case 'btnShowVacc':
			        args.set_cancel(true);
			        document.getElementById('hiddenStatusDetailVac').value = showDgrDetail();
			        break;

                case 'btnRevocaConsenso':
			        if (!confirm('Attenzione: si è sicuri di voler revocare il consenso per le associazioni selezionate?'))
				        args.set_cancel(true);
                    break;
	        }
        }

		function showDgrDetail(show) 
        {
			ret='0';
			var el = document.getElementsByName('divDettaglio');
			var img = document.getElementsByName('imgDettaglio');
			if (el!=null)
			{
				// Se il parametro è null, controllo il primo div: se è nascosto li mostro tutti, e viceversa.
				if (typeof(show) == "undefined" || show == null) {	
					if (el[0].style.display=='none') show=true;
					else show=false;
				}
			
				for (var i=0;i<el.length;i++)
					if (show) 
					{
						el[i].style.display='block';
						img[i].src='../../images/meno.gif';
						img[i].alt='Nascondi vaccinazioni associate';
					}
					else 
					{
						el[i].style.display='none';
						img[i].src='../../images/piu.gif';
						img[i].alt='Mostra vaccinazioni associate';
					}
				
				if (show) ret='1';
			}
			return ret;
		}
		
		function showDettaglioVac(img, dgrId) 
        {
			var ret = "0";
			
			// Cambio l'immagine del pulsante
			if (img.src.indexOf('piu.gif')!=-1) {
			    img.src='../../images/meno.gif';
				img.alt='Nascondi vaccinazioni associate';
			} else {
			    img.src='../../images/piu.gif';
				img.alt='Mostra vaccinazioni associate';
			}
			
			// divDettaglio
			var div=document.getElementById(dgrId).parentNode;
			
			if (div==null) return ret;
			
			// Mostro o nascondo il dettaglio delle vaccinazioni
			if (div.style.display=='none') {
				div.style.display='block';
				ret = "1";
			} else {
				div.style.display='none';
				ret = "0"
			}
			
			
			return ret;
		}

    </script>

    <style type="text/css">
        td.hsort a
        {
            color:white;
            font-weight:bold;
        }

        td.hsort a[disabled="true"], td.hsort a[disabled="disabled"]
        {
            text-shadow: 1px 1px #FFF, 0px 0px;
            color: #A0A0A0;
            text-decoration:none;
        }

        td.hsort img
        {
            height: 10px;
            width: 10px;
            margin-left:2px
        }
        

    </style>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
        
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" TitleCssClass="Title3" Height="100%" Titolo="Vaccinazioni Eseguite">

            <!-- #include file="../../Common/Scripts/AggiornaStatoVaccinazioni.inc" -->
		    <!-- #include file="../../Common/Scripts/ControlloDatiValidi.inc" -->

            <asp:MultiView ID="mlvMaster" runat="server" >
				    
                <!-- View elenco vaccinazioni eseguite -->
				<asp:View ID="Eseguite" runat="server">
                    
					<div class="title" id="divTitolo">
						<asp:Label id="LayoutTitolo1" runat="server" Width="100%"></asp:Label>
					</div>
                        <div>
                            <telerik:RadToolBar ID="ToolBar" runat="server" Width="100%" Skin="Default" EnableEmbeddedSkins="false" EnableAjaxSkinRendering="false" 
                                EnableEmbeddedBaseStylesheet="false" OnButtonClick="Toolbar_ButtonClick" OnClientButtonClicking="OnClientButtonClicking">
                                <Items>
                                    <telerik:RadToolBarButton runat="server" Text="Salva" Value="btn_Salva" ImageUrl="~/Images/salva.gif" DisabledImageUrl="~/Images/salva_dis.gif"></telerik:RadToolBarButton>
                                    <telerik:RadToolBarButton runat="server" Text="Annulla" Value="btn_Annulla" ImageUrl="~/Images/annulla.gif" DisabledImageUrl="~/Images/annulla_dis.gif"></telerik:RadToolBarButton>
                                    <telerik:RadToolBarButton runat="server" Text="Vista Ridotta" Value="btnChangeView" ImageUrl="~/Images/dettaglio.gif" DisabledImageUrl="~/Images/dettaglio_dis.gif"></telerik:RadToolBarButton>
                                    <telerik:RadToolBarDropDown Text="Scadenza" runat="server" ToolTip="Le vaccinazioni relative alle associazioni selezionate verranno fatte scadere o ripristinate"
                                        DropDownWidth="140px" ImageUrl="../../images/flagStatoScaduta.png" DisabledImageUrl="../../images/flagStatoScaduta_dis.png" >
                                        <Buttons>
                                            <telerik:RadToolBarButton runat="server" Text="Fai Scadere" Value="btnScaduta" ImageUrl="../../images/scade_ok.gif"
                                                DisabledImageUrl="../../images/scade_ok.gif" ></telerik:RadToolBarButton>
                                            <telerik:RadToolBarButton runat="server" Text="Ripristina" Value="btnRipristinaScaduta" ImageUrl="../../images/scade_no.gif"
                                                DisabledImageUrl="../../images/scade_no.gif" ></telerik:RadToolBarButton>                                            
                                        </Buttons>
                                    </telerik:RadToolBarDropDown>
                                    <telerik:RadToolBarButton runat="server" Text="Mostra/Nascondi Vacc" Value="btnShowVacc" ImageUrl="~/Images/piu.gif" DisabledImageUrl="~/Images/piu_dis.gif"></telerik:RadToolBarButton>
                                    <telerik:RadToolBarDropDown Text="Consenso" runat="server" ToolTip="Concede/Nega il consenso alla comunicazione dei dati vaccinali"
                                        DropDownWidth="180px" ImageUrl="../../images/modificaConsenso.png" DisabledImageUrl="../../images/modificaConsenso_dis.png" >
                                        <Buttons>
                                            <telerik:RadToolBarButton runat="server" Text="Concedi Consenso" Value="btnConcediConsenso" ImageUrl="../../images/consensoPositivo.png"
                                                ToolTip="Imposta il consenso alla comunicazione dei dati a CONCESSO per le vaccinazioni selezionate"></telerik:RadToolBarButton>
                                            <telerik:RadToolBarButton runat="server" Text="Revoca Consenso" Value="btnRevocaConsenso" ImageUrl="../../images/consensoNegativo.png"
                                                ToolTip="Imposta il consenso alla comunicazione dei dati a NEGATO per le vaccinazioni selezionate"></telerik:RadToolBarButton>
                                        </Buttons>
                                    </telerik:RadToolBarDropDown>
                                    <telerik:RadToolBarButton runat="server" Text="Modifica Vaccinazioni" Value="btnModificaVaccinazioni" ImageUrl="../../images/modificaDose.gif" DisabledImageUrl="../../images/modificaDose_dis.gif" ></telerik:RadToolBarButton>
                                    <telerik:RadToolBarButton runat="server" Text="Recupera" Value="btnRecuperaStoricoVacc" ImageUrl="../../images/recupera.png" DisabledImageUrl="../../images/recupera_dis.png" ToolTip="Recupera lo storico vaccinale centralizzato del paziente" ></telerik:RadToolBarButton>
                                    <telerik:RadToolBarButton runat="server" Text="Certif. Vaccinale" Value="btnStampaCertificatoVacc" ImageUrl="../../images/stampa.gif" DisabledImageUrl="../../images/stampa_dis.gif" ToolTip="Stampa il certificato vaccinale del paziente" ></telerik:RadToolBarButton>
									<telerik:RadToolBarButton runat="server" Text="Certif. Vacc. Giornaliero" Value="btnStampaCertificatoVaccGior" ImageUrl="../../images/stampa.gif" DisabledImageUrl="../../images/stampa_dis.gif" ToolTip="Stampa il certificato vaccinale del paziente del giorno selezionato" ></telerik:RadToolBarButton>
                                </Items>
                            </telerik:RadToolBar>
                        </div>
					    <%--<div class="vac-sezione">
                            <asp:Label id="LayoutTitolo_sezione" runat="server" Text="ELENCO VACCINAZIONI ESEGUITE"></asp:Label>
                        </div>--%>
					    <div id="divLegenda" class="legenda-vaccinazioni">
						    <span class="legenda-vaccinazioni-reazione">R</span>
						    <span>Reazione avversa associata</span>
						    <span class="legenda-vaccinazioni-scaduta">S</span>
						    <span>Associazione Scaduta</span>
                            <span class="legenda-vaccinazioni-fittizia">F</span>
						    <span>Associazione Fittizia</span>
                            <asp:CheckBox id="chkFilterAntiInfluenzali" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="chkFilterAntiInfluenzali_CheckedChanged" 
                                Text="<%$ Resources: Onit.OnAssistnet.OnVac.Web, VaccinazioniEseguite.IncludiAntiInfluenzali %>" 
                                ToolTip="<%$ Resources: Onit.OnAssistnet.OnVac.Web, VaccinazioniEseguite.IncludiAntiInfluenzaliTooltip %>"
                                TextAlign="right" CssClass="legenda-vaccinazioni-checkbox" />
					    </div>

                    <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="99%" ScrollBars="Auto" RememberScrollPosition="true">
                        <asp:Panel id="pan_VacEs" CssClass="pan_VacEs" runat="server" style="width:100%; height:100%; overflow: auto;">
						    <asp:DataGrid id="dgrAssEseguite" runat="server" CssClass="dgr" AutoGenerateColumns="False" BorderWidth="0px" BorderColor="Black"	
                                AllowSorting="True"  CellPadding="1" ShowHeader="True" style="" >
							    <SelectedItemStyle Font-Bold="True" CssClass="Selected"></SelectedItemStyle>
							    <AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
							    <ItemStyle BackColor="#E7E7FF" VerticalAlign="Top"></ItemStyle>
							    <PagerStyle CssClass="Pager" Mode="NumericPages"></PagerStyle>
                                <HeaderStyle CssClass="header"></HeaderStyle>
							    <Columns>
								    <asp:TemplateColumn SortExpression="CheckboxSelection">
									    <ItemStyle HorizontalAlign="Center" Width="25px" ></ItemStyle>
                                        <HeaderStyle Width="25px" />
									    <ItemTemplate>
                                            <div style="text-align:center; padding-top:3px">
											    <asp:checkBox id="cb" runat="server" class="margin_btn" ></asp:checkBox>
                                            </div>
									    </ItemTemplate>
								    </asp:TemplateColumn>
                                    <asp:TemplateColumn>
                                        <ItemStyle HorizontalAlign="Center" Width="25px"  />
                                        <HeaderStyle Width="25px" />
                                        <ItemTemplate>
                                            <asp:ImageButton CssClass="margin_btn" ID="btnDettaglioReazione" runat="server" CommandName="Select" ImageUrl="~/Images/avvertimento.gif" ToolTip="Inserisci/visualizza la reazione avversa" />
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
								    <asp:ButtonColumn Text="&lt;div class=&quot;margin_btn&quot;&gt; &lt;img onclick='confermaEliminazione();' title=&quot;Elimina&quot; src=&quot;../../images/elimina.gif&quot; &gt; &lt;/div&gt;"
									    CommandName="Delete" SortExpression="BtnElimina">
									    <ItemStyle HorizontalAlign="Center" Width="25px" ></ItemStyle>
                                        <HeaderStyle Width="25px" />
								    </asp:ButtonColumn>
								    <asp:EditCommandColumn SortExpression="BtnEdit" ButtonType="LinkButton" 
                                        UpdateText="&lt;div class=&quot;margin_btn&quot;&gt; &lt;img  title=&quot;Aggiorna&quot; src=&quot;../../images/conferma.gif&quot; onclick=&quot;controlla()&quot;&gt; &lt;/div&gt;"
									    CancelText="&lt;img  title=&quot;Annulla&quot; src=&quot;../../images/annullaconf.gif&quot;&gt;"
									    EditText="&lt;div class=&quot;margin_btn&quot;&gt; &lt;img  title=&quot;Modifica&quot; src=&quot;../../images/modifica.gif&quot; &gt; &lt;/div&gt;">
									    <ItemStyle HorizontalAlign="Center" Width="25px" ></ItemStyle>
                                        <HeaderStyle Width="25px" />
								    </asp:EditCommandColumn>
								    <asp:TemplateColumn SortExpression="BtnStampaReazione">
									    <ItemStyle HorizontalAlign="Center" Width="25px" ></ItemStyle>
                                        <HeaderStyle Width="25px" />
									    <ItemTemplate>
										    <div class="margin_btn">
											    <asp:ImageButton onmouseup="CheckSaving();" id="imgStampa" runat="server" CommandName="Stampa" ImageUrl="../../images/stampa.gif" Visible='<%# Container.DataItem("vra_data_reazione").ToString<>"" %>' AlternateText="Stampa il modulo delle reazioni avverse" />
										    </div>
									    </ItemTemplate>
								    </asp:TemplateColumn>
								    <asp:TemplateColumn>
									    <ItemStyle HorizontalAlign="Center" Width="25px" ></ItemStyle>
                                        <HeaderStyle Width="25px" />
									    <ItemTemplate>
										    <div class="margin_btn">
											    <img height="16" id="imgDettaglio" name="imgDettaglio" alt="Mostra vaccinazioni associate" src="../../Images/piu.gif"
												    width="16" border="0" style="cursor:hand" onclick='showDettaglioVac(this,"<%# Container.FindControl("dgrDettaglio").ClientId %>");' />
										    </div>
									    </ItemTemplate>
								    </asp:TemplateColumn>
								    <asp:TemplateColumn SortExpression="ves_data_effettuazione">
								        <ItemStyle Width="120px" HorizontalAlign="Center"></ItemStyle>
                                        <HeaderStyle Width="120px"  HorizontalAlign="Center" CssClass="hsort" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerData" runat="server"   CommandArgument="ves_data_effettuazione"
							                CommandName="Sort"  >Data</asp:LinkButton><asp:image id="img_headerData" ImageUrl='<%# GetImageHeader("ves_data_effettuazione")%>' 	runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
									        <asp:HiddenField ID="tb_ves_id" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem")("ves_id") %>' />
                                            <asp:HiddenField ID="hdVesUslInserimento" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem")("ves_usl_inserimento") %>' />
										    <asp:Label id="tb_data_eff" runat="server" Width="97px" Text='<%# CTYPE(DataBinder.Eval(Container, "DataItem")("ves_data_effettuazione"),DATETIME).tostring("dd/MM/yyyy") %>' Visible="false"></asp:Label>
										    <asp:Label id="tb_ora_eff" runat="server" Width="100%" Text='<%# CType(DataBinder.Eval(Container, "DataItem")("ves_dataora_effettuazione"), DateTime).ToString("dd/MM/yyyy HH:mm") %>'></asp:Label>
									    </ItemTemplate>
									    <EditItemTemplate>
									        <asp:Label id="tb_dataora_eff_edit_label" runat="server" Text='<%# CType(DataBinder.Eval(Container, "DataItem")("ves_dataora_effettuazione"), DateTime).ToString("dd/MM/yyyy HH:mm") %>'></asp:Label>
										    <table style="width:100%">
											    <tr>		
												    <td width="70%" align="right">
													    <on_val:onitdatepick id="tb_data_eff_edit" tabIndex="10" runat="server" Height="22px" Width="100%" indice="-1" DateBox="True" target="tb_data_eff_edit" Text='<%# CTYPE(DataBinder.Eval(Container, "DataItem")("ves_dataora_effettuazione"),DATETIME).tostring("dd/MM/yyyy") %>' Focus="False" FormatoData="GeneralDate" ControlloTemporale="False" Hidden="False" Formatta="False" NoCalendario="True" CalendarioPopUp="False"></on_val:onitdatepick>
												    </td>
												    <td width="30%" align="left">
													    <asp:TextBox id="tb_ora_eff_edit" tabIndex="20" style="border:inset; width:40px ; border-width:1px; height:24px;" CssClass="TextBox_Stringa" Text='<%# CTYPE(DataBinder.Eval(Container, "DataItem")("ves_dataora_effettuazione"),DATETIME).tostring("HH:mm",System.Globalization.CultureInfo.InvariantCulture) %>' onblur="formattaOrario(this)"  Runat="server" ></asp:TextBox>
												    </td>
											    </tr>
										    </table>
									    </EditItemTemplate>
								    </asp:TemplateColumn>
								    <asp:TemplateColumn SortExpression="ass_descrizione">
                                        <HeaderStyle Width="250px" CssClass="hsort" />
								        <ItemStyle Width="250px" VerticalAlign="Top" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerAssoc" runat="server"  CommandArgument="ass_descrizione"
							                CommandName="Sort">Associazione</asp:LinkButton><asp:image id="img_headerAssoc" ImageUrl='<%# GetImageHeader("ass_descrizione")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <asp:Label Width="180px" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("ass_descrizione") %>'>
										    </asp:Label>										
										    <div id="divDettaglio" name="divDettaglio" style="display: none; margin-top: 20px">
								                <asp:DataGrid id="dgrDettaglio" runat="server" Width="100%" CssClass="dgr2" AutoGenerateColumns="False">
											        <AlternatingItemStyle CssClass="r1"></AlternatingItemStyle>
											        <ItemStyle CssClass="r2"></ItemStyle>
											        <HeaderStyle CssClass="h1"></HeaderStyle>
											        <Columns>
                                                            <asp:TemplateColumn HeaderText="Vaccinazione [dose]">
                                                                <HeaderStyle Width="60%" />
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblVacDesc" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("vac_descrizione")%>'></asp:Label>
                                                                    <asp:Label ID="lblVacDose" runat="server" Text='<%# "[" + DataBinder.Eval(Container, "DataItem")("ves_n_richiamo").ToString() + "]"%>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateColumn>
                                                            <asp:TemplateColumn>
                                                                <HeaderStyle HorizontalAlign="Center" Width="20%" />
                                                                <ItemStyle HorizontalAlign="Center" />
                                                                <HeaderTemplate>
                                                                    <div title='Condizione sanitaria'>Cond.<br/>Sanit.</div>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblCondSanitaria" runat="server"
                                                                        Text='<%# DataBinder.Eval(Container, "DataItem")("ves_mal_codice_cond_sanitaria")%>'
                                                                        ToolTip='<%# DataBinder.Eval(Container, "DataItem")("descrizione_condizione_sanitaria")%>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateColumn>
                                                            <asp:TemplateColumn>
                                                                <HeaderStyle HorizontalAlign="Center" Width="20%" />
                                                                <ItemStyle HorizontalAlign="Center" />
                                                                <HeaderTemplate>
                                                                    <div title='Condizione di rischio'>Cond.<br/>Rischio</div>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblCondRischio" runat="server" 
                                                                        Text='<%# DataBinder.Eval(Container, "DataItem")("ves_rsc_codice")%>'
                                                                        ToolTip='<%# DataBinder.Eval(Container, "DataItem")("descrizione_condizione_rischio")%>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateColumn>
												            <asp:BoundColumn Visible="false" DataField="vac_descrizione" HeaderText="vac_descrizione"></asp:BoundColumn>
												            <asp:BoundColumn Visible="false" DataField="ves_n_richiamo" HeaderText="ves_n_richiamo"></asp:BoundColumn>
												            <asp:BoundColumn Visible="False" DataField="ves_ass_codice" HeaderText="ves_ass_codice"></asp:BoundColumn>
												            <asp:BoundColumn Visible="False" DataField="ves_data_effettuazione" HeaderText="ves_data_effettuazione"></asp:BoundColumn>
                                                            <asp:TemplateColumn>
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdIdVaccinazione" runat="server" />
                                                                </ItemTemplate>
                                                            </asp:TemplateColumn>
											            </Columns>
										        </asp:DataGrid>
									        </div>									
									    </ItemTemplate>
								    </asp:TemplateColumn>
								    <asp:TemplateColumn SortExpression="ass_codice" Visible="true">
                                        <HeaderStyle Width="70px" HorizontalAlign="Center" CssClass="hsort" />
								        <ItemStyle Width="70px" HorizontalAlign="Center" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerAssCod" runat="server"  CommandArgument="ass_codice"	
						                    CommandName="Sort" >Cod. Ass</asp:LinkButton><asp:image id="img_headerAssCod"  ImageUrl='<%# GetImageHeader("ass_codice")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <asp:Label id="tb_ass_codice" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("ass_codice") %>'></asp:Label>
									    </ItemTemplate>
								    </asp:TemplateColumn>
								    <asp:TemplateColumn Visible="True" SortExpression="ves_ass_n_dose">
                                        <HeaderStyle Width="45px" CssClass="hsort" />
								        <ItemStyle Width="45px" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerDose" runat="server" CommandArgument="ves_ass_n_dose"
							                CommandName="Sort">Dose</asp:LinkButton><asp:image id="img_headerDose"  ImageUrl='<%# GetImageHeader("ves_ass_n_dose")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <div align="center">
											    <asp:Label Width="100%" id="tb_ass_n_dose" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("ves_ass_n_dose") %>' />
										        <asp:HiddenField id="tb_n_rich" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem")("ves_n_richiamo") %>' />
										    </div>
									    </ItemTemplate>
									    <EditItemTemplate>
										    <div align="center">
										        <asp:HiddenField id="hdfAssDose" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem")("ves_ass_n_dose") %>' />
                                                <asp:Label Width="100%" id="lblDoseAss" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("ves_ass_n_dose") %>'
                                                    Visible='<%# IIF( Not DataBinder.Eval(Container, "DataItem")("ass_codice") Is System.DBNull.Value AndAlso DataBinder.Eval(Container, "DataItem")("scaduta").ToString() = "N", False, True) %>'  />
											    <asp:TextBox  Width="100%" id="tb_ass_n_dose_edit" runat="server" MaxLength="3"
											        Visible='<%# IIF( Not DataBinder.Eval(Container, "DataItem")("ass_codice") Is System.DBNull.Value AndAlso DataBinder.Eval(Container, "DataItem")("scaduta").ToString() = "N", True, False) %>' 
											        Text='<%# DataBinder.Eval(Container, "DataItem")("ves_ass_n_dose") %>' style="text-align:right;"   />
										    </div>
                                        </EditItemTemplate>
								    </asp:TemplateColumn>																	
								    <asp:TemplateColumn SortExpression="ves_lot_codice">
                                        <HeaderStyle Width="120px"  HorizontalAlign="Center" CssClass="hsort" />
									    <ItemStyle Width="120px" HorizontalAlign="Center" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerLotto" runat="server"  CommandArgument="ves_lot_codice"
							                CommandName="Sort" >Lotto</asp:LinkButton><asp:image id="img_headerLotto"  ImageUrl='<%# GetImageHeader("ves_lot_codice")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <asp:Label id="tb_lotto" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("ves_lot_codice") %>'>
										    </asp:Label>
									    </ItemTemplate>
								    </asp:TemplateColumn>
								    <asp:TemplateColumn SortExpression="noc_descrizione">
                                        <HeaderStyle Width="90px"  CssClass="hsort" HorizontalAlign="Left" />
									    <ItemStyle Width="90px" HorizontalAlign="Left"/>
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerNomeComm" runat="server"  CommandArgument="noc_descrizione"
							                CommandName="Sort" >Nome Commerciale</asp:LinkButton><asp:image id="img_headerNomeComm"  ImageUrl='<%# GetImageHeader("noc_descrizione")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <asp:Label id="tb_nome_com" Width="100%" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("noc_descrizione") %>'>
										    </asp:Label>
									    </ItemTemplate>
								    </asp:TemplateColumn>
								    <asp:TemplateColumn SortExpression="ope_nome">
                                        <HeaderStyle Width="200px"  HorizontalAlign="Left" CssClass="hsort" />
									    <ItemStyle Width="200px" HorizontalAlign="Left" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerMedResp" runat="server"  CommandArgument="ope_nome"
							                CommandName="Sort" >Medico Responsabile</asp:LinkButton><asp:image id="img_headerMedResp"  ImageUrl='<%# GetImageHeader("ope_nome")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <asp:Label id="tb_medico" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("ope_nome") %>'>
										    </asp:Label>
									    </ItemTemplate>
									    <EditItemTemplate>
										    <on_ofm:onitmodallist id="fm_medico_edit" tabIndex="30" runat="server" Width="100%" UseTableLayout="False" DataTypeDescription="Stringa" IsDistinct="False" Paging="False" DataTypeCode="Stringa" UseAllResultDescIfEqual="False" UseAllResultCodeIfEqual="False" Sorting="False" PosizionamentoFacile="False" LabelWidth="-1px" CodiceWidth="0%" Obbligatorio="False" SetUpperCase="True" UseCode="False" CampoCodice="ope_codice" CampoDescrizione="ope_nome" Tabella="t_ana_operatori" Filtro="'true'='true' order by ope_nome" Codice='<%# DataBinder.Eval(Container, "DataItem")("ves_ope_codice") %>' Descrizione='<%# DataBinder.Eval(Container, "DataItem")("ope_nome") %>' RaiseChangeEvent="False" LikeMode="All">
										    </on_ofm:onitmodallist>
									    </EditItemTemplate>
								    </asp:TemplateColumn>
								    <asp:TemplateColumn SortExpression="ves_ope_in_ambulatorio">
                                        <HeaderStyle Width="70px" HorizontalAlign="Center"  CssClass="hsort"/>
								        <ItemStyle Width="70px" HorizontalAlign="Center" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerInAmb" runat="server"  CommandArgument="ves_ope_in_ambulatorio"
							                CommandName="Sort">In Amb</asp:LinkButton><asp:image id="img_headerInAmb"  ImageUrl='<%# GetImageHeader("ves_ope_in_ambulatorio")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <%# RisolviInAmbulatorio(Container.DataItem("ves_ope_in_ambulatorio").toString()) %>
									    </ItemTemplate>
									    <EditItemTemplate>
										    <on_ocb:onitcombo id="ocOpeInAmb" tabIndex="40" runat="server" Width="100%" IncludeNull="False" BoundText='<%# DataBinder.Eval(Container, "DataItem")("ves_ope_in_ambulatorio") %>'>
											    <asp:ListItem Value="N">NO</asp:ListItem>
											    <asp:ListItem Value="S">SI</asp:ListItem>
										    </on_ocb:onitcombo>
									    </EditItemTemplate>
								    </asp:TemplateColumn>
								    <asp:TemplateColumn SortExpression="ves_med_vaccinante">
                                        <HeaderStyle Width="110px"  CssClass="hsort"/>
								        <ItemStyle width="110px" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerVaccinante" runat="server"  CommandArgument="ves_med_vaccinante"
							                CommandName="Sort" >Vaccinatore</asp:LinkButton><asp:image id="img_headerVaccinante"  ImageUrl='<%# GetImageHeader("ves_med_vaccinante")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <asp:Label  id="tb_medico_vaccinante" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("ope_nome1") %>'>
										    </asp:Label>
									    </ItemTemplate>
									    <EditItemTemplate>
										    <on_ofm:onitmodallist id="fm_medico_vaccinante_edit" tabIndex="50" runat="server" Width="100%" UseTableLayout="False" DataTypeDescription="Stringa" IsDistinct="False" Paging="False" DataTypeCode="Stringa" UseAllResultDescIfEqual="False" UseAllResultCodeIfEqual="False" Sorting="False" PosizionamentoFacile="False" LabelWidth="-1px" CodiceWidth="0px" Obbligatorio="False" SetUpperCase="True" UseCode="False" CampoCodice="ope_codice" CampoDescrizione="ope_nome" Tabella="t_ana_operatori" Filtro="'true'='true' order by ope_nome" Codice='<%# DataBinder.Eval(Container, "DataItem")("ves_med_vaccinante") %>' Descrizione='<%# DataBinder.Eval(Container, "DataItem")("ope_nome1") %>' RaiseChangeEvent="False" LikeMode="All">
										    </on_ofm:onitmodallist>
									    </EditItemTemplate>
								    </asp:TemplateColumn>
								    <asp:TemplateColumn SortExpression="ves_luogo">
                                        <HeaderStyle Width="200px" CssClass="hsort" />
								        <ItemStyle width="200px" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerLuogo" runat="server"  CommandArgument="ves_luogo"
							                CommandName="Sort" >Luogo</asp:LinkButton><asp:image id="img_headerLuogo"  ImageUrl='<%# GetImageHeader("ves_luogo")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <%# RisolviLuogo(Container.DataItem("ves_luogo").toString()) %>
									    </ItemTemplate>
									    <EditItemTemplate>
										    <on_ocb:onitcombo id="cmbLuogo" tabIndex="60" runat="server" Width="100%" IncludeNull="False" BoundText='<%# DataBinder.Eval(Container, "DataItem")("ves_luogo") %>'
												DataTextField="Descrizione" DataValueField="Codice">
											    <asp:ListItem Value=""></asp:ListItem>
										    </on_ocb:onitcombo>
									    </EditItemTemplate>
								    </asp:TemplateColumn>
								    <asp:TemplateColumn SortExpression="cns_descrizione" >
                                        <HeaderStyle Width="150px" HorizontalAlign="Left" CssClass="hsort" />
									    <ItemStyle width="150px" HorizontalAlign="Left" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerConsultorio" runat="server" CommandArgument="cns_descrizione"
							                CommandName="Sort" >CentroVacc/Ambulatorio</asp:LinkButton><asp:image id="img_headerConsultorio"  ImageUrl='<%# GetImageHeader("cns_descrizione")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <div align="center">
											    <asp:Label id="tb_cons" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("cns_descrizione") %>'></asp:Label><br/>
										        <asp:Label id="tb_amb" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("amb_descrizione") %>'></asp:Label>
										    </div>
									    </ItemTemplate>
									    <EditItemTemplate>
										    <div align="center">
											    <uc3:SelezioneAmbulatorio id="uscScegliAmb" tabIndex="61" runat="server" Tutti="False" MostraCodici="false" MostraLabel="false" />
										    </div>
									    </EditItemTemplate>
								    </asp:TemplateColumn>
								    <asp:TemplateColumn SortExpression="usl_inserimento_ves_descr" >
                                        <HeaderStyle Width="100px"  HorizontalAlign="Left" CssClass="hsort"/>
									    <ItemStyle width="100px" HorizontalAlign="Left" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerUslInserimentoVes" runat="server" CommandArgument="usl_inserimento_ves_descr"
							                CommandName="Sort" Text="<%$ Resources: Onit.OnAssistnet.OnVac.Web, DatiVaccinali.AslInserimentoVacc %>"></asp:LinkButton><asp:image id="img_headerUslInserimentoVes"  ImageUrl='<%# GetImageHeader("usl_inserimento_ves_descr")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <div align="center">
											    <asp:Label id="tb_usl_ves" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("usl_inserimento_ves_descr") %>'></asp:Label>
										    </div>
									    </ItemTemplate>
								    </asp:TemplateColumn>
								    <asp:TemplateColumn SortExpression="vii_descrizione">
                                        <HeaderStyle Width="120px"  HorizontalAlign="Left"  CssClass="hsort"/>
									    <ItemStyle width="120px" HorizontalAlign="Left" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerViaSomm" runat="server"  CommandArgument="vii_descrizione"
							                CommandName="Sort" >Via di somministrazione</asp:LinkButton><asp:image id="img_headerViaSomm"  ImageUrl='<%# GetImageHeader("vii_descrizione")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <asp:Label id="tb_vii" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("vii_descrizione") %>'></asp:Label>
									    </ItemTemplate>
									    <EditItemTemplate>
                                            <asp:Label id="tb_vii_edit_label" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("vii_descrizione") %>'></asp:Label>
										    <on_ofm:onitmodallist id="fm_vii_edit" tabIndex="70" runat="server" Width="100%" LabelWidth="-1px" PosizionamentoFacile="False" Obbligatorio="False" SetUpperCase="True" UseCode="False" CodiceWidth="0%" CampoCodice="vii_codice" CampoDescrizione="vii_descrizione" Connection="" Tabella="t_ana_vie_somministrazione" Filtro="'true'='true' order by vii_descrizione" Codice='<%# DataBinder.Eval(Container, "DataItem")("ves_vii_codice") %>' Descrizione='<%# DataBinder.Eval(Container, "DataItem")("vii_descrizione") %>' RaiseChangeEvent="False"></on_ofm:onitmodallist>
									    </EditItemTemplate>
								    </asp:TemplateColumn>										
								    <asp:TemplateColumn SortExpression="sii_descrizione">
                                        <HeaderStyle Width="100px" HorizontalAlign="Left"  CssClass="hsort" />
									    <ItemStyle width="100px" HorizontalAlign="Left" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerSitoInoc" runat="server" CommandArgument="sii_descrizione"
							                CommandName="Sort" >Sito di inoculazione</asp:LinkButton><asp:image id="img_headerSitoInoc"  ImageUrl='<%# GetImageHeader("sii_descrizione")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <asp:Label id="tb_sii" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("sii_descrizione") %>'>
										    </asp:Label>
									    </ItemTemplate>
									    <EditItemTemplate>
                                            <asp:Label id="tb_sii_edit_label" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("sii_descrizione") %>'></asp:Label>
										    <on_ofm:onitmodallist id="fm_sii_edit" tabIndex="80" runat="server" Width="100%" LabelWidth="-1px" PosizionamentoFacile="False" Obbligatorio="False" SetUpperCase="True" UseCode="False" CodiceWidth="0%" CampoCodice="sii_codice" CampoDescrizione="sii_descrizione" Connection="" Tabella="t_ana_siti_inoculazione" Filtro="'true'='true' order by sii_descrizione" Codice='<%# DataBinder.Eval(Container, "DataItem")("ves_sii_codice") %>' Descrizione='<%# DataBinder.Eval(Container, "DataItem")("sii_descrizione") %>' RaiseChangeEvent="False"></on_ofm:onitmodallist>
									    </EditItemTemplate>
								    </asp:TemplateColumn>																	
								    <asp:TemplateColumn SortExpression="ute_descrizione">
                                        <HeaderStyle Width="200px"  CssClass="hsort" />
								        <ItemStyle width="200px"  />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerUtente" runat="server" CommandArgument="ute_descrizione"
							                CommandName="Sort">Utente</asp:LinkButton><asp:image id="img_headerUtente"  ImageUrl='<%# GetImageHeader("ute_descrizione")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("ute_descrizione") %>' Width=100%>
										    </asp:Label>
									    </ItemTemplate>
								    </asp:TemplateColumn>																	
								    <asp:TemplateColumn SortExpression="com_descrizione">
                                        <HeaderStyle Width="100px"  CssClass="hsort" />
								        <ItemStyle width="100px" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerComune" runat="server" CommandArgument="com_descrizione"
							                CommandName="Sort" >Comune o stato</asp:LinkButton><asp:image id="img_headerComune"  ImageUrl='<%# GetImageHeader("com_descrizione")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("com_descrizione") %>'>
										    </asp:Label>
									    </ItemTemplate>
									    <EditItemTemplate>
										    <on_ofm:onitmodallist id="fmComuneOStato" tabIndex="90" runat="server" Width="100%" LabelWidth="-1px" PosizionamentoFacile="False" Obbligatorio="False" SetUpperCase="True" UseCode="False" CodiceWidth="0%" CampoCodice="com_codice" CampoDescrizione="com_descrizione" Tabella="t_ana_comuni" Filtro="" Codice='<%# DataBinder.Eval(Container, "DataItem")("ves_comune_o_stato") %>' Descrizione='<%# DataBinder.Eval(Container, "DataItem")("com_descrizione") %>' RaiseChangeEvent="False">
										    </on_ofm:onitmodallist>
									    </EditItemTemplate>
								    </asp:TemplateColumn>																	
								    <asp:TemplateColumn SortExpression="ves_esito">
                                        <HeaderStyle Width="100px" HorizontalAlign="Center"  CssClass="hsort" />
								        <ItemStyle width="100px" HorizontalAlign="Center" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerEsito" runat="server" CommandArgument="ves_esito"
							                CommandName="Sort" >Esito</asp:LinkButton><asp:image id="img_headerEsito"  ImageUrl='<%# GetImageHeader("ves_esito")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("ves_esito") %>'>
										    </asp:Label>
									    </ItemTemplate>
								    </asp:TemplateColumn>																	
								    <asp:TemplateColumn SortExpression="ves_note">
                                        <HeaderStyle Width="200px" HorizontalAlign="Left"  CssClass="hsort"/>
									    <ItemStyle width="200px" HorizontalAlign="Left" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerNote" runat="server" CommandArgument="ves_note"
							                CommandName="Sort" >Note</asp:LinkButton><asp:image id="img_headerNote"  ImageUrl='<%# GetImageHeader("ves_note")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("ves_note") %>'>
										    </asp:Label>
									    </ItemTemplate>
									    <EditItemTemplate>
										    <asp:TextBox id="txtNoteVac" TextMode="MultiLine" MaxLength="200" Rows="3"  style="overflow-y:auto;" 
										        CssClass="TextBox_Stringa" Text='<%# DataBinder.Eval(Container, "DataItem")("ves_note") %>' Runat="server" 
										        tabIndex="91" width="100%" />
									    </EditItemTemplate>
								    </asp:TemplateColumn>																	
								    <asp:TemplateColumn SortExpression="ves_in_campagna">
                                        <HeaderStyle Width="100px" HorizontalAlign="Center"  CssClass="hsort"/>
								        <ItemStyle width="100px" HorizontalAlign="Center" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerInCampagna" runat="server"  CommandArgument="ves_in_campagna"
							                CommandName="Sort" >In Campagna</asp:LinkButton><asp:image id="img_headerInCampagna"  ImageUrl='<%# GetImageHeader("ves_in_campagna")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <asp:Label runat="server" Text='<%# IIF(DataBinder.Eval(Container, "DataItem")("ves_in_campagna") is dbnull.value,"N",DataBinder.Eval(Container, "DataItem")("ves_in_campagna")) %>'>
										    </asp:Label>
									    </ItemTemplate>
								    </asp:TemplateColumn>																									
								    <asp:TemplateColumn SortExpression="ves_cnv_data">
                                        <HeaderStyle Width="100px"  HorizontalAlign="Center"  CssClass="hsort"/>
								        <ItemStyle width="100px" HorizontalAlign="Center" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerDataConv" runat="server"  CommandArgument="ves_cnv_data"
							                CommandName="Sort" >Data Conv.</asp:LinkButton><asp:image id="img_headerDataConv"  ImageUrl='<%# GetImageHeader("ves_cnv_data")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <asp:Label runat="server" Text='<%# FormattaData(container.dataItem("ves_cnv_data").tostring()) %>' >
										    </asp:Label>
									    </ItemTemplate>
								    </asp:TemplateColumn>
								    <asp:TemplateColumn SortExpression="ves_cnv_data_primo_app">
                                        <HeaderStyle Width="100px" HorizontalAlign="Center"  CssClass="hsort"/>
								        <ItemStyle width="100px" HorizontalAlign="Center" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerDataConvPrimoApp" runat="server" CommandArgument="ves_cnv_data_primo_app"
							                CommandName="Sort" >Primo App.</asp:LinkButton><asp:image id="img_headerDataConvPrimoApp"  ImageUrl='<%# GetImageHeader("ves_cnv_data_primo_app")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <asp:Label runat="server" Text='<%# FormattaData(container.dataItem("ves_cnv_data_primo_app").tostring()) %>' >
										    </asp:Label>
									    </ItemTemplate>
								    </asp:TemplateColumn>								
								    <asp:TemplateColumn SortExpression="mal_descrizione">
                                        <HeaderStyle Width="200px" HorizontalAlign="Left" CssClass="hsort" />
								        <ItemStyle width="200px" HorizontalAlign="Left" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerMalattia" runat="server" CommandArgument="mal_descrizione"
							                CommandName="Sort">Malattia</asp:LinkButton><asp:image id="img_headerMalattia"  ImageUrl='<%# GetImageHeader("mal_descrizione")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <asp:Label  runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("mal_descrizione") %>'>
										    </asp:Label>
									    </ItemTemplate>
								    </asp:TemplateColumn>		
								    <asp:TemplateColumn SortExpression="ves_codice_esenzione">
                                        <HeaderStyle Width="100px" HorizontalAlign="Left" CssClass="hsort" />
								        <ItemStyle width="100px" HorizontalAlign="Left" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerCodEse" runat="server" CommandArgument="ves_codice_esenzione"
							                CommandName="Sort">Cod. Esenzione</asp:LinkButton><asp:image id="img_headerCodEse"  ImageUrl='<%# GetImageHeader("ves_codice_esenzione")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <asp:Label  runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("ves_codice_esenzione") %>'>
										    </asp:Label>
									    </ItemTemplate>
								    </asp:TemplateColumn>	
								    <asp:TemplateColumn SortExpression="ves_importo">
                                        <HeaderStyle Width="100px" HorizontalAlign="Right"  CssClass="hsort"/>
								        <ItemStyle width="100px" HorizontalAlign="Right" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerImporto" runat="server" CommandArgument="ves_importo"
							                CommandName="Sort" >Importo</asp:LinkButton><asp:image id="img_headerImporto"  ImageUrl='<%# GetImageHeader("ves_importo")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <asp:Label runat="server" Text='<%# FormattaImporto(directcast(Container.DataItem, datarowview))%>' />
										    &nbsp;
									    </ItemTemplate>
								    </asp:TemplateColumn>
                                    <asp:TemplateColumn SortExpression="ves_flag_visibilita" >
                                        <HeaderStyle Width="20px" HorizontalAlign="Center" />
                                        <ItemStyle width="20px" HorizontalAlign="Center" />
                                        <HeaderTemplate>
                                            <span id="tdHeaderFlagVisibilita" runat="server" title="Consenso alla comunicazione dei dati vaccinali da parte del paziente" 
                                                style="font-family:Arial;font-size:12px;font-weight:bold;color:White">&nbsp;</span>
                                        </HeaderTemplate>
									    <ItemTemplate>
                                            <asp:Image ID="imgFlagVisibilita" runat="server" 
                                                ImageUrl="<%# BindFlagVisibilitaImageUrlValue(Container.DataItem) %>" 
                                                ToolTip="<%# BindFlagVisibilitaToolTipValue(Container.DataItem) %>" />
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
								    <asp:TemplateColumn >
                                        <HeaderStyle Width="18px" HorizontalAlign="Center" />
								        <ItemStyle Width="18px" HorizontalAlign="Center" />
                                        <HeaderTemplate>
                                            <span title="Reazione avversa" style="width:18px" >&nbsp;</span>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <asp:Label runat="server" CssClass="legenda-vaccinazioni-reazione" 
                                                title='<%# IIf(Not FlagAbilitazioneVaccUslCorrente OrElse DataBinder.Eval(Container, "DataItem")("vra_usl_inserimento") Is DBNull.Value,
                                                                           "Reazione avversa presente",
                                                                           "Reazione avversa presente" + Environment.NewLine + "(azienda: " + DataBinder.Eval(Container, "DataItem")("vra_usl_inserimento").ToString() + ")") %>' 
                                                Visible='<%# IIF(DataBinder.Eval(Container, "DataItem")("vra_data_reazione") is dbnull.value,false,true) %>'>R</asp:Label>
									    </ItemTemplate>
								    </asp:TemplateColumn>
								    <asp:TemplateColumn>
                                        <HeaderStyle Width="18px" HorizontalAlign="Center" />
								        <ItemStyle Width="18px" HorizontalAlign="Center" />
                                        <HeaderTemplate>
                                            <span style="width:18px" title="Associazione scaduta" >&nbsp;</span>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <asp:Label runat="server" CssClass="legenda-vaccinazioni-scaduta"
                                                title='<%# IIf(Not FlagAbilitazioneVaccUslCorrente OrElse DataBinder.Eval(Container, "DataItem")("ves_usl_scadenza") Is DBNull.Value,
                                                                            "Associazione scaduta",
                                                                            "Associazione scaduta" + Environment.NewLine + "(azienda: " + DataBinder.Eval(Container, "DataItem")("ves_usl_scadenza").ToString() + ")") %>' 
                                                Visible='<%# IIF(DataBinder.Eval(Container, "DataItem")("scaduta").tostring()="S",true,false) %>'>S</asp:Label>
									    </ItemTemplate>
								    </asp:TemplateColumn>
								    <asp:TemplateColumn>
                                        <HeaderStyle Width="18px" HorizontalAlign="Center" />
								        <ItemStyle Width="18px" HorizontalAlign="Center" />
                                        <HeaderTemplate>
                                            <span style="width:18px" title="Associazione fittizia" >&nbsp;</span>
                                        </HeaderTemplate>
									    <ItemTemplate>
                                            <asp:Label runat="server" CssClass="legenda-vaccinazioni-fittizia"
                                                title="Associazione fittizia"
                                                Visible='<%# IIF(DataBinder.Eval(Container, "DataItem")("ves_flag_fittizia").tostring()="S",true,false) %>'>F</asp:Label>
									    </ItemTemplate>
								    </asp:TemplateColumn>	
								    <asp:TemplateColumn SortExpression="ass_anti_influenzale" Visible="false">
                                        <HeaderStyle Width="100px" HorizontalAlign="Right"  CssClass="hsort"/>
								        <ItemStyle width="100px" HorizontalAlign="Right" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerAntiInfluenzale" runat="server" CommandArgument="ass_anti_influenzale"
							                CommandName="Sort" >Anti Influenzale</asp:LinkButton><asp:image id="img_headerAntiInfluenzale"  ImageUrl='<%# GetImageHeader("ass_anti_influenzale")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <asp:Label  runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("ass_anti_influenzale") %>'>
										    </asp:Label>
									    </ItemTemplate>
								    </asp:TemplateColumn>
                                    <asp:TemplateColumn SortExpression="ves_lot_data_scadenza">
                                        <HeaderStyle Width="100px"  HorizontalAlign="Center"  CssClass="hsort"/>
								        <ItemStyle width="100px" HorizontalAlign="Center" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerDataScadenzaLotto" runat="server"  CommandArgument="ves_lot_data_scadenza"
							                CommandName="Sort" >Data Scadenza Lotto</asp:LinkButton><asp:image id="img_headerDataScadenzaLotto"  ImageUrl='<%# GetImageHeader("ves_lot_data_scadenza")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <asp:Label runat="server" Text='<%# FormattaData(Container.DataItem("ves_lot_data_scadenza").ToString()) %>' >
										    </asp:Label>
									    </ItemTemplate>
                                        <EditItemTemplate>
										    <table style="width:100%">
											    <tr>		
												    <td width="70%" align="right">
													    <on_val:onitdatepick id="tb_data_scadenza_lotto_edit" tabIndex="10" runat="server" Height="22px" Width="100%" indice="-1" DateBox="True" target="tb_data_scadenza_lotto_edit" Text='<%#If(DataBinder.Eval(Container, "DataItem")("ves_lot_data_scadenza") Is DBNull.Value, String.Empty, CType(DataBinder.Eval(Container, "DataItem")("ves_lot_data_scadenza"), DateTime).ToString("dd/MM/yyyy")) %>' Focus="False" FormatoData="GeneralDate" ControlloTemporale="False" Hidden="False" Formatta="False" NoCalendario="True" CalendarioPopUp="False"></on_val:onitdatepick>
												    </td>												 
											    </tr>
										    </table>
									    </EditItemTemplate>
								    </asp:TemplateColumn>
									<%--NUOVI--%>
									<%--TIPO EROGATORE--%>
									<asp:TemplateColumn SortExpression="ves_tipo_erogatore">
                                        <HeaderStyle Width="200px" CssClass="hsort" />
								        <ItemStyle width="200px" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerTipoErogatore" runat="server"  CommandArgument="ves_tipo_erogatore"
							                CommandName="Sort" >TipoErogatore</asp:LinkButton><asp:image id="img_headerTipoErogatore"  ImageUrl='<%# GetImageHeader("ves_tipo_erogatore")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <%# GetDescrizioneTipoErogatore(Container.DataItem("ves_tipo_erogatore").ToString()) %>
									    </ItemTemplate>
									    <EditItemTemplate>
										    <on_ocb:onitcombo id="cmbTipoErogatore" tabIndex="60" runat="server" Width="100%" IncludeNull="False" BoundText='<%# DataBinder.Eval(Container, "DataItem")("ves_tipo_erogatore") %>' DataTextField="Descrizione" DataValueField="Codice">
											    <asp:ListItem Value=""></asp:ListItem>
										    </on_ocb:onitcombo>
									    </EditItemTemplate>
								    </asp:TemplateColumn>

									<%--CODICE STRUTTURA--%>
									<asp:TemplateColumn SortExpression="ves_codice_struttura">
                                        <HeaderStyle Width="60px"  HorizontalAlign="Left"  CssClass="hsort"/>
									    <ItemStyle width="60px" HorizontalAlign="Left" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerCodiceStruttura" runat="server"  CommandArgument="ves_codice_struttura"
							                CommandName="Sort" >Struttura</asp:LinkButton><asp:image id="img_headerCodiceStruttura"  ImageUrl='<%# GetImageHeader("ves_codice_struttura")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <asp:Label id="lblCodiceStruttura" runat="server" Text='<%# Container.DataItem("ves_codice_struttura").ToString() %>'></asp:Label>
									    </ItemTemplate>
									    <EditItemTemplate>
										    <asp:TextBox id="txtCodiceStruttura"  CssClass="TextBox_Stringa_Disabilitato" Text='<%# DataBinder.Eval(Container, "DataItem")("ves_codice_struttura") %>' 
												Runat="server" Enabled="false" Width="100%" />									    
									    </EditItemTemplate>
								    </asp:TemplateColumn>

									<%--TIPO PAGAMENTO--%>
									 <asp:TemplateColumn SortExpression="ves_tpa_guid_tipi_pagamento">
                                        <HeaderStyle Width="200px" CssClass="hsort" />
								        <ItemStyle width="200px" />
                                        <HeaderTemplate>
                                            <asp:LinkButton id="lnkBtn_headerTipoPagamento" runat="server"  CommandArgument="ves_tpa_guid_tipi_pagamento"
							                CommandName="Sort" >Tipo Pagamento</asp:LinkButton><asp:image id="img_headerTipoPagamento"  ImageUrl='<%# GetImageHeader("ves_tpa_guid_tipi_pagamento")%>' runat="server"/>
                                        </HeaderTemplate>
									    <ItemTemplate>
										    <%# RisolviTipoPagamento(Container.DataItem("ves_tpa_guid_tipi_pagamento")) %>
									    </ItemTemplate>
									    <EditItemTemplate>
										    <on_ocb:onitcombo id="cmbTipoPagamento" tabIndex="60" runat="server" Width="100%" IncludeNull="false" DataTextField="Value" DataValueField="Key" BoundText='<%# DataBinder.Eval(Container, "DataItem")("ves_tpa_guid_tipi_pagamento") %>'>
											    <asp:ListItem Value=""></asp:ListItem>
										    </on_ocb:onitcombo>
									    </EditItemTemplate>
								    </asp:TemplateColumn>

									<%--CODICE SOMMINISTRAZIONE NON GESTITO--%>

						<%--			•	VES_TPA_GUID_TIPI_PAGAMENTO			RAW (16)				=> FATTO
•	VES_MAL_CODICE_COND_SANITARIA		VARCHAR2 (8 Byte)		=> SI modificabile, sotto la vaccinazione
•	VES_RSC_CODICE						VARCHAR2 (8 Byte)		=> SI modificabile, sotto la vaccinazione
•	VES_LOT_DATA_SCADENZA				DATE					=> SI Editabile
•	VES_ANTIGENE						VARCHAR2 (2 Byte)		=> NO
•	VES_TIPO_EROGATORE					VARCHAR2 (2 Byte)		=> SI Editabile (fatto)
•	VES_CODICE_STRUTTURA				VARCHAR2 (8 Byte)		=> SI
•	VES_USL_CODICE_SOMMINISTRAZIONE		VARCHAR2 (8 Byte)		=> NO


ATTENZIONE A VES_LUOGO--%>

                                    <asp:BoundColumn Visible="False" DataField="ves_data_effettuazione"></asp:BoundColumn>
								    <asp:BoundColumn Visible="False" DataField="ves_ass_codice"></asp:BoundColumn>
                                    <asp:BoundColumn Visible="False" DataField="ves_data_registrazione"></asp:BoundColumn>
                                    <asp:BoundColumn Visible="False" DataField="ves_usl_inserimento"></asp:BoundColumn>
                                    <asp:BoundColumn Visible="False" DataField="ves_usl_scadenza"></asp:BoundColumn>
                                    <asp:BoundColumn Visible="False" DataField="vra_usl_inserimento"></asp:BoundColumn>
                                    <asp:BoundColumn Visible="False" DataField="vra_data_compilazione"></asp:BoundColumn>
                                    <asp:BoundColumn Visible="False" DataField="ves_usl_scadenza"></asp:BoundColumn>
                                    <asp:BoundColumn Visible="False" DataField="ves_ute_id_scadenza"></asp:BoundColumn>
                                    <asp:BoundColumn Visible="False" DataField="ves_data_scadenza"></asp:BoundColumn>
                                    <asp:BoundColumn Visible="False" DataField="ves_ute_id_ripristino"></asp:BoundColumn>
                                    <asp:BoundColumn Visible="False" DataField="ves_data_ripristino"></asp:BoundColumn>                                    
                                </Columns>
						    </asp:DataGrid>
					    </asp:Panel>
				    </dyp:DynamicPanel>
			
                    <div class="alert" id="lblWarning1" runat="server">&nbsp;</div>

                </asp:View>
                    
                <!-- View dettaglio reazione avversa -->
				<asp:View ID="ReazioneAvversa" runat="server">

					<div class="Title" id="divLayoutTitolo" >
						<asp:Label id="LayoutTitolo2" runat="server" CssClass="title" BorderStyle="None"></asp:Label>
                    </div>
                    <div>
                        <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBarDetail" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                            <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                            <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
	                        <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
						    <ClientSideEvents InitializeToolbar="InizializzaToolBarDetail" Click="ToolBarDetailClick"></ClientSideEvents>
                            <Items>
							    <igtbar:TBarButton Key="btn_Conferma" DisabledImage="~/Images/conferma_dis.gif" Text="Conferma" Image="~/Images/conferma.gif"></igtbar:TBarButton>
							    <igtbar:TBarButton Key="btn_Indietro" DisabledImage="~/Images/annullaconf_dis.gif" Text="Indietro" Image="~/Images/annullaconf.gif"></igtbar:TBarButton>
						    </Items>
                        </igtbar:UltraWebToolbar>
                    </div>

					<dyp:DynamicPanel ID="DynamicPanel1" runat="server" Width="100%" Height="100%">
						<uc1:ReazAvverseDetail id="ReazAvverseDetail" runat="server"></uc1:ReazAvverseDetail>
                        <div class="alert" id="lblWarning2" runat="server">&nbsp;</div>
                    </dyp:DynamicPanel>

                </asp:View>

            </asp:MultiView>

			<input id="hiddenStatusDetailVac" type="hidden" value="0" runat="server"/>

            <on_ofm:OnitFinestraModale ID="modElencoVaccinazioniEseguite" Title="Modifica vaccinazioni" runat="server" Width="850px" BackColor="LightGray" NoRenderX="false">
                <uc2:VacEs ID="ucElencoVaccinazioniEseguite" runat="server"></uc2:VacEs>
            </on_ofm:OnitFinestraModale>

        </on_lay3:OnitLayout3>
    
        <script type="text/javascript">
		
            // --------- Per gestire l'intestazione delle colonne --------- //
            if (document.getElementById('header_VacEseguite') != null)
                document.getElementById('header_VacEseguite').style.width = document.getElementById('dgrAssEseguite').style.width;
	
		    function CheckSaving()
		    {
			    if (<%= (OnitLayout31.Busy).ToString().ToLower()%>)
				    alert('I dati non sono stati salvati. E\' possibile che le modifiche non siano visibili. Si consiglia di salvare prima di eseguire la stampa.')
		    }
		
		    function confermaEliminazione()
		    {
			    if (!confirm("Si desidera eliminare l'associazione selezionata?"))
				    StopPreventDefault(getEvent());
		    }
		
        </script>
    
    </form>
</body>
</html>
