<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="InterventiPaziente.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.InterventiPaziente" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register Assembly="Onit.Web.UI.WebControls.DynamicPanel" Namespace="Onit.Web.UI.WebControls.DynamicPanel" TagPrefix="dyp" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>

<%@ Import namespace="Onit.OnAssistnet.OnVac.OnVacUtility" %>
<%@ Import namespace="Onit.OnAssistnet.OnVac" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Interventi Paziente</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <style type="text/css">
        .ckeditor {
            font-family: Arial,Tahoma,Verdana;
            font-size: 12px;
        }
        .item,
        .Alternating {
            vertical-align: top;
            margin-top: 5px;
        }
    </style>
    <script type="text/javascript" language="javascript">
        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;
        }

        function ImpostaImmagineOrdinamento(imgId, imgUrl) {
            var img = document.getElementById(imgId);
            if (img != null) {
                img.style.display = 'inline';
                img.src = imgUrl;
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Height="100%" Width="100%" TitleCssClass="Title3" Titolo="Anagrafe Interventi">
            <div class="title" id="divLayoutTitolo_sezione1" style="width: 100%">
                <asp:Label ID="LayoutTitolo" runat="server" Width="100%" BorderStyle="None" CssClass="title"></asp:Label>
            </div>
            <div>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="tlbInterventi" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                    <Items>
                        <igtbar:TBarButton Key="btnNuovo" Text="Nuovo" DisabledImage="~/Images/nuovo_dis.gif" Image="~/Images/nuovo.gif">
                        </igtbar:TBarButton>
                    </Items>
                </igtbar:UltraWebToolbar>
            </div>
            <div class="sezione">
                Elenco interventi
            </div>
            <dyp:DynamicPanel ID="dyp2" runat="server" Width="100%" Height="100%" ScrollBars="Auto" ExpandDirection="horizontal">
                <asp:DataGrid ID="dgrInterventi" runat="server" CssClass="datagrid" Width="100%" ShowFooter="false"
                    AllowSorting="True" AutoGenerateColumns="False" CellPadding="1" GridLines="None" DataKeyField="Codice">
                    <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
                    <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
                    <ItemStyle CssClass="item"></ItemStyle>
                    <HeaderStyle Font-Bold="True" CssClass="header"></HeaderStyle>
                    <Columns>
                        <asp:BoundColumn Visible="False" DataField="Codice" HeaderText="H_idIntPaz"></asp:BoundColumn>
                        <asp:TemplateColumn HeaderStyle-Width="2%">
                            <ItemStyle HorizontalAlign="Center" Width="2%" />
                            <ItemTemplate>
                                <asp:ImageButton ID="btnEditGrid" runat="server" CommandName="EditRowInterventi" ImageUrl="~/Images/modifica.gif" ToolTip="Modifica" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="btnConfermaGrid" runat="server" CommandName="ConfirmRowInterventi" ImageUrl="~/Images/conferma.gif" ToolTip="Conferma" />
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderStyle-Width="2%">
                            <ItemStyle HorizontalAlign="Left" Width="2%" />
                            <ItemTemplate>
                                <asp:ImageButton ID="btnDeleteGrid" runat="server" CommandName="DeleteRowInterventi" ImageUrl="~/Images/elimina.gif" ToolTip="Elimina" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="btnAnnullaGrid" runat="server" CommandName="CancelRowInterventi" ImageUrl="~/Images/annullaConf.gif" ToolTip="Annulla" />
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Intervento <img id='imgInt' alt='' src='../../images/transparent16.gif' />"
                            SortExpression="Intervento">
                            <HeaderStyle Width="25%" />
                            <ItemStyle Width="25%" />
                            <ItemTemplate>
                                <asp:Label ID="lblIntervento" CssClass="label_left" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Intervento")%>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="lblCodiceIntervento" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CodiceIntervento")%>' Visible="false" />
                                <asp:DropDownList ID="ddlIntervento" CssClass="textbox_stringa_obbligatorio" runat="server" Width="100%" AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlIntervento_SelectedIndexChanged" />
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Data <img id='imgDat' alt='' src='../../images/transparent16.gif' />" 
                            SortExpression="Data" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="12%">
                            <ItemStyle Width="12%" HorizontalAlign="Left" />
                            <ItemTemplate>
                                <asp:Label ID="lblData" CssClass="label_left" runat="server"
                                    Text='<%# IIf(String.IsNullOrWhiteSpace(DataBinder.Eval(Container.DataItem, "Data")), String.Empty,
                                        CDate(DataBinder.Eval(Container.DataItem, "Data")).Date.ToString("dd/MM/yyyy")) %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <on_val:OnitDatePick ID="odpData" runat="server" DateBox="True" CssClass="textbox_data_obbligatorio"
                                    Text='<%# IIf(String.IsNullOrWhiteSpace(DataBinder.Eval(Container.DataItem, "Data")), String.Empty,
                                        CDate(DataBinder.Eval(Container.DataItem, "Data")).Date.ToString("dd/MM/yyyy")) %>' />
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Tipologia <img id='imgTip' alt='' src='../../images/transparent16.gif' />" 
                            SortExpression="Tipologia" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="12%">
                            <ItemStyle HorizontalAlign="Left" Width="12%" />
                            <ItemTemplate>
                                <asp:Label ID="lblTipologia" CssClass="label_left" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Tipologia")%>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Dur. <img id='imgDur' alt='' src='../../images/transparent16.gif' />" 
                            SortExpression="Durata" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="10%">
                            <ItemStyle Width="10%" HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Label ID="lblDurata" CssClass="label_center" style="margin-right:8px" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Durata") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <on_val:OnitJsValidator ID="txtDurata" runat="server" CssClass="textbox_stringa_obbligatorio" style="text-align: right"
                                    actionCorrect="False" actionDelete="False" actionFocus="True" actionMessage="True" actionSelect="False" Width="35px"
                                    actionUndo="True" autoFormat="True" validationType="Validate_custom" CustomValFunction-Length="2" CustomValFunction="validaNumero"
                                    SetOnChange="True" MaxLength="5" Text='<%# DataBinder.Eval(Container.DataItem, "Durata") %>'></on_val:OnitJsValidator>
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Operatore <img id='imgOp' alt='' src='../../images/transparent16.gif' />" 
                            SortExpression="Operatore" >
                            <HeaderStyle Width="17%" />
                            <ItemStyle Width="17%" />
                            <ItemTemplate>
                                <asp:Label ID="lblOperatore" CssClass="label_left" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Operatore")%>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="lblCodiceOperatore" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CodiceOperatore")%>' Visible="false" />
                                <on_ofm:OnitModalList ID="omlOperatore" runat="server" Width="70%" Filtro="1=1 order by OPE_NOME" OnSetUpFiletr="omlOperatore_SetUpFiletr"
                                    PosizionamentoFacile="False" LabelWidth="-1px" Obbligatorio="True" UseCode="True" SetUpperCase="True" CampoCodice="OPE_CODICE Codice"
                                    CampoDescrizione="OPE_NOME Nome" CodiceWidth="29%" Tabella="T_ANA_OPERATORI" />
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderStyle-Width="1%">
                            <ItemStyle Width="1%" />
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkShowAllOp" runat="server" Checked="False" ToolTip="Mostra tutti gli operatori" />
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Note <img id='imgNot' alt='' src='../../images/transparent16.gif' />" 
                            SortExpression="Note" >
                            <HeaderStyle Width="23%" />
                            <ItemStyle Width="23%" />
                            <ItemTemplate>
                                <asp:Label ID="lblNote" CssClass="label_left" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Note")%>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNote" runat="server" CssClass="ckeditor" TextMode="MultiLine" Rows="3"
                                    MaxLength="1000" Width="98%" Text='<%# DataBinder.Eval(Container.DataItem, "Note")%>' />
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
            </dyp:DynamicPanel>
        </on_lay3:OnitLayout3>
		
        <!-- #include file="../../Common/Scripts/AggiornaStatoVaccinazioni.inc" -->

    </form>
</body>
</html>
