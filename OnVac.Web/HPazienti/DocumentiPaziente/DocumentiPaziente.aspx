<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DocumentiPaziente.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.DocumentiPaziente" %>

<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>

<%@ Import Namespace="Onit.OnAssistnet.OnVac" %>
<%@ Import Namespace="Onit.OnAssistnet.OnVac.OnVacUtility" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Documenti Paziente</title>

    <link href='<%= ResolveClientUrl("~/css/button.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/default/button.default.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <style type="text/css">
        .mt {
            height: 18px;
            width: 40px;
            margin-top: 4px;
            text-decoration: none;
        }
    </style>
    <script type="text/javascript" language="javascript">
        function ToolBarClick(ToolBar,button,evnt)
        {
            evnt.needPostBack=true;
            
            switch(button.Key)
            {
                case "btnSave":
                    
                    var isEditState = <%= (Me.State = DocumentiPaziente.DocumentiState.Edit).ToString().ToLower() %>;
                    if (isEditState) {
				        
                        evnt.needPostBack=false;
                        var desc=document.getElementById("txtDesc").value;
                        var note=document.getElementById("txtNote").value;
				    
                        if (desc=="") {
                            alert("Impossibile eseguire l'operazione.\nLa Descrizione e' obbligatoria !");
                            document.getElementById("txtDesc").focus();
                            return;
                        }
				    
                        if (note.length>1000) {
                            alert("Impossibile eseguire l'operazione.\nLe Note non possono superare i 1000 caratteri !");
                            document.getElementById("txtNote").select();
                            document.getElementById("txtNote").focus();
                            return;
                        }
				    
                        evnt.needPostBack=true;
                    }

                    break;
            }					
        }

        function fup_Change(fup) {
            
        }

        function btnConfermaUpload_Click(sender) {
           
            if (document.getElementById("fup").value == '') {
                alert("Impossibile eseguire l'operazione.\nSelezionare un File o Scansionare un Documento o !");
                return;
            }
        
            var desc = document.getElementById("ddlDettaglioDescrizioneUpload").options[document.getElementById("ddlDettaglioDescrizioneUpload").selectedIndex].text;
            var note = document.getElementById("txtNoteUpload").value;
        
            if (desc == "") {
                alert("Impossibile eseguire l'operazione.\nLa Descrizione e' obbligatoria !");
                document.getElementById("ddlDettaglioDescrizioneUpload").focus();
                return;
            }
      
            if (note.length > 1000) {
                alert("Impossibile eseguire l'operazione.\nLe Note non possono superare i 1000 caratteri !");
                document.getElementById("txtNoteUpload").select();
                document.getElementById("txtNoteUpload").focus();
                return;
            }

            var btnConfermaUpload = document.getElementById('<%= btnConfermaUpload.ClientID()%>');
            btnConfermaUpload.click();
        }

        function btnAnnullaUpload_Click(sender) {
            closeFm('<%= fmUpload.ClientID()%>');
        }


        function btnReplica_Click() {}
    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" HeightTitle="90px" Height="100%" Width="100%" Titolo="Documenti Paziente" TitleCssClass="Title3">
					
			<div class="title" id="divLayoutTitolo_sezione1" style="WIDTH: 100%">
				<asp:label id="LayoutTitolo" runat="server" Width="100%" BorderStyle="None" CssClass="title"></asp:label>
            </div>
			<div>			
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="Toolbar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
					<Items>
						<igtbar:TBarButton Key="btnNew" Text="Nuovo" DisabledImage="~/Images/nuovo_dis.gif" Image="~/Images/nuovo.gif"></igtbar:TBarButton>
						<igtbar:TBarButton Key="btnEdit" Text="Modifica" DisabledImage="~/Images/modifica_dis.gif"
							Image="~/Images/modifica.gif"></igtbar:TBarButton>
						<igtbar:TBarButton Key="btnDelete" Text="Elimina" DisabledImage="~/Images/elimina_dis.gif"
							Image="~/Images/elimina.gif"></igtbar:TBarButton>
						<igtbar:TBSeparator></igtbar:TBSeparator>
						<igtbar:TBarButton Key="btnSave" Enabled="False" Text="Salva" DisabledImage="~/Images/salva_dis.gif"
							Image="~/Images/salva.gif"></igtbar:TBarButton>
						<igtbar:TBarButton Key="btnCancel" Enabled="False" Text="Annulla" DisabledImage="~/Images/annulla_dis.gif"
							Image="~/Images/annulla.gif"></igtbar:TBarButton>
						<igtbar:TBSeparator></igtbar:TBSeparator>
						<igtbar:TBarButton Key="btnDownload" Text="Visualizza" DisabledImage="../../images/download_dis.gif"
							Image="../../images/download.gif"></igtbar:TBarButton>
					</Items>
				</igtbar:UltraWebToolbar>
			</div>
                
			<div class="sezione" style="width: 100%">Filtri sui Documenti</div>
            <div>
				<table style="table-layout: fixed" cellspacing="2" cellpadding="0" width="100%">
					<tr>
						<td class="label" width="80">Descrizione</td>
						<td colspan="3">
							<asp:DropDownList id="ddlFiltroDescrizione" runat="server" Width="512px" AutoPostBack="True" DataTextField="COD_DESCRIZIONE"
								DataValueField="COD_CODICE"></asp:DropDownList></td> 
                    </tr>
				</table>
            </div>

			<div class="sezione" id="divLayoutTitolo_sezione" style="width: 100%">
				<asp:Label id="LayoutTitolo_sezione" runat="server">Elenco Documenti</asp:Label>
            </div>

            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
				<on_dgr:onitgrid id="dgrDocumenti" runat="server" Width="100%" CssClass="DataGrid" SortedColumns="Matrice IGridColumn[]"
					GridLines="None" AutoGenerateColumns="False" sortAscImage="../images/arrow_up_small.gif" sortDescImage="../images/arrow_down_small.gif"
					SelectionOption="rowClick" PagerVoicesAfter="0" PagerVoicesBefore="0" PageSize="1" PagerGoToOption="False">
					<AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
					<ItemStyle CssClass="Item"></ItemStyle>
					<Columns>
						<on_dgr:OnitMultiSelColumn Visible="False" key=""></on_dgr:OnitMultiSelColumn>
						<on_dgr:OnitBoundColumn Visible="False" DataField="PDO_ID" key="PDO_ID" SortDirection="NoSort">
							<HeaderStyle Width="0%"></HeaderStyle>
						</on_dgr:OnitBoundColumn>
						<on_dgr:OnitBoundColumn DataField="PDO_DESCRIZIONE" HeaderText="Descrizione" key="PDO_DESCRIZIONE" SortDirection="NoSort">
							<HeaderStyle HorizontalAlign="Left" Width="20%"></HeaderStyle>
							<ItemStyle HorizontalAlign="Left"></ItemStyle>
						</on_dgr:OnitBoundColumn>
						<on_dgr:OnitBoundColumn DataField="PDO_NOTE" HeaderText="Descrizione dettagliata" key="PDO_NOTE" SortDirection="NoSort">
							<HeaderStyle HorizontalAlign="Left" Width="40%"></HeaderStyle>
							<ItemStyle HorizontalAlign="Left"></ItemStyle>
						</on_dgr:OnitBoundColumn>
						<on_dgr:OnitBoundColumn DataField="PDO_DATA_ARCHIVIAZIONE" HeaderText="Archiviazione" key="PDO_DATA_ARCHIVIAZIONE"
							SortDirection="NoSort">
							<HeaderStyle HorizontalAlign="Left" Width="20%"></HeaderStyle>
							<ItemStyle HorizontalAlign="Left"></ItemStyle>
						</on_dgr:OnitBoundColumn>
						<on_dgr:OnitBoundColumn DataField="USL_INSERIMENTO_PDO_DESCR" HeaderText="<%$ Resources: Onit.OnAssistnet.OnVac.Web, DatiVaccinali.AslInserimentoVacc %>" key="USL_INSERIMENTO_PDO_DESCR"
							SortDirection="NoSort">
							<HeaderStyle HorizontalAlign="Left" Width="10%"></HeaderStyle>
							<ItemStyle HorizontalAlign="Left"></ItemStyle>
						</on_dgr:OnitBoundColumn>
						<on_dgr:OnitBoundColumn Visible="false" DataField="PDO_USL_INSERIMENTO" key="PDO_USL_INSERIMENTO" SortDirection="NoSort">
							<HeaderStyle HorizontalAlign="Left" Width="0%"></HeaderStyle>
						</on_dgr:OnitBoundColumn>
                        <asp:TemplateColumn>
                            <HeaderStyle HorizontalAlign="Center" />
							<ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Image ID="imgFlagVisibilita" runat="server" 
                                    ImageUrl="<%# BindFlagVisibilitaImageUrlValue(Container.DataItem) %>" 
                                    ToolTip="<%# BindFlagVisibilitaToolTipValue(Container.DataItem) %>" />
                                <asp:HiddenField ID="HiddenField_FLAGVISIBILITA" runat="server" Value='<%# Container.DataItem("PDO_FLAG_VISIBILITA") %>'></asp:HiddenField>
                            </ItemTemplate>
                        </asp:TemplateColumn>
					</Columns>
					<HeaderStyle CssClass="Header"></HeaderStyle>
					<SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
					<PagerStyle CssClass="Pager"></PagerStyle>
				</on_dgr:onitgrid>
            </dyp:DynamicPanel>

            <div class="sezione" style="width:100%">Dati Documento</div>
            <div>
                <div class="label" style="text-align:left; padding:5px;"><asp:Label ID="Lbl_StatoDetail" runat="server" Text="Non disponibile" Visible="false"></asp:Label></div>
                <table id="Table_Documento" runat="server" tyle="table-layout: fixed" cellspacing="2" cellpadding="0" width="100%">
					<tr>
						<td class="label" width="80">Descrizione</td>
						<td>
							<asp:DropDownList id="ddlDettaglioDescrizione" runat="server" Width="512px" DataTextField="COD_DESCRIZIONE"
								DataValueField="COD_CODICE"></asp:DropDownList>
                        </td>
                        <td colspan="2" class="label" style="text-align:left;">
                            <asp:CheckBox ID="chkFlagVisibilita" runat="server" Text="Consenso comunicazione" ToolTip="Consenso alla comunicazione dei dati vaccinali da parte del paziente" />
                        </td>
					</tr>
					<tr>
						<td class="label" valign="top">Note</td>
						<td colspan="3">
							<asp:TextBox id="txtNote" runat="server" Width="100%" CssClass="textbox_stringa" TextMode="MultiLine" Rows="3"></asp:TextBox>
                        </td>
					</tr>
				</table>
            </div>

        </on_lay3:OnitLayout3>

        <on_ofm:OnitFinestraModale ID="fmUpload" Title="Upload Documento" runat="server" Width="500px" BackColor="White" NoRenderX="true" RenderModalNotVisible="true">
            <div>
                <div class="sezione" style="width: 100%">Dati Documento</div>
                <table style="table-layout: fixed" cellspacing="2" cellpadding="3" width="100%">
                    <tr>
                        <td class="label" width="80">File</td>
                        <td colspan="3">
                            <asp:FileUpload ID="fup" runat="server" onchange="fup_Change(this)" Width="100%" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label" width="80">Descrizione</td>
                        <td colspan="3">
                            <asp:DropDownList ID="ddlDettaglioDescrizioneUpload" runat="server" Width="100%" DataValueField="COD_CODICE" DataTextField="COD_DESCRIZIONE"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="label" valign="top">Note</td>
                        <td colspan="3">
                            <asp:TextBox ID="txtNoteUpload" runat="server" Width="100%" CssClass="textbox_stringa" Rows="3" TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="label" width="80">&nbsp;</td>
                        <td class="label" colspan="3" style="text-align:left;">
                            <asp:CheckBox ID="chkFlagVisibilita_new" runat="server" Text="Consenso comunicazione" ToolTip="Consenso alla comunicazione dei dati vaccinali da parte del paziente" />
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td align="right">
                            <a href="javascript:void(0)" class="RadButton RadButton_Default rbSkinnedButton" onclick="btnConfermaUpload_Click(this)">
                                <span class="rbPrimaryIcon" style="background-image: url(<%= ResolveUrl("~/Images/conferma.gif") %>);"></span>
                                <span class="rbDecorated rbPrimary">
                                    <span class="mt">Conferma</span>
                                </span>
                            </a>
                            <asp:Button ID="btnConfermaUpload" runat="server" OnClick="btnConfermaUpload_Click" Style="display: none" />
                        </td>
                        <td>
                            <a href="javascript:void(0)" class="RadButton RadButton_Default rbSkinnedButton" onclick="btnAnnullaUpload_Click(this)">
                                <span class="rbPrimaryIcon" style="background-image: url(<%= ResolveUrl("~/Images/annulla.gif") %>);"></span>
                                <span class="rbDecorated rbPrimary">
                                    <span class="mt">Annulla</span>
                                </span>
                            </a>
                        </td>
                        <td>&nbsp;</td>
                    </tr>
                </table>
            </div>
        </on_ofm:OnitFinestraModale>

    </form>

    <!-- #include file="../../Common/Scripts/AggiornaStatoVaccinazioni.inc" -->
    <!-- #include file="../../Common/Scripts/ControlloDatiValidi.inc" -->

</body>
</html>
