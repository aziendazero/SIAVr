<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Sedute.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Sedute" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>Sedute</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript" language="javascript">

        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function InizializzaToolBarVisualizzaParametri(t) {
            t.PostBackButton = false;
        }

        function NuovaSed() {
            if (document.getElementById("nascosto").value == "DisNS") {
                alert("Impossibile aggiungere una nuova seduta!");
            }
            else {
                __doPostBack('NuovaSeduta');
            }
        }

        function NuovaVac() {
            if (document.getElementById("nascosto").value == "DisNV") {
                alert("Impossibile aggiungere una nuova vaccinazione!");
            }
            else {
                __doPostBack('NuovaVac');
            }
        }

        function NuovaAss() {
            if (document.getElementById("nascosto").value == "DisNA") {
                alert("Impossibile aggiungere una nuova associazione!");
            }
            else {
                __doPostBack('NuovaAss');
            }
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;

            switch (button.Key) {
                case 'btn_Annulla':

                    if ("<%response.write(onitlayout1.busy)%>" == "True") {
                        evnt.needPostBack = confirm("Le modifiche effettuate andranno perse. Continuare?");
                    }
                    else { evnt.needPostBack = false };
                    break;

                case 'btn_Salva':

                    if ("<%response.write(onitlayout1.busy)%>" == "True") {
                        evnt.needPostBack = true;
                    }
                    else { evnt.needPostBack = false; }
                    break;
            }
        }

        function ToolBarClickVisualizzaParametri(ToolBar, button, evnt) {
            evnt.needPostBack = true;

            switch (button.Key) {
                case 'btnConferma':

                    break;

                case 'btnAnnulla':

                    closeFm('<% = fmVisualizzaParametri.ClientId %>')
                    evnt.needPostBack = false;
                    break;
            }
        }

        function controlla_sed(evt) {
            //alert("controlla!");
            //el=event.srcElement.parentNode.parentNode.parentNode.childNodes[4].firstChild;
            el = SourceElement(evt);
            riga = el.parentNode.parentNode.parentNode;
            tab = GetElementByTag(riga, 'TABLE', 1, 0, false);
            //alert(tab);
            cell = tab.rows[riga.rowIndex].cells[4];
            el = GetElementByTag(cell, 'INPUT', 1, 1, false);

            if (el.value == "") {
                alert("Il campo 'Intervallo' è vuoto. Non è possibile aggiornare la riga.");
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }

            if (isNaN(el.value) || parseInt(el.value) < 0) {
                alert("Il campo 'Intervallo' contiene un dato non valido. Non è possibile aggiornare la riga.");
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }

            if (el.value.length > 5) {
                alert("Il campo 'Intervallo' non può contenere più di 5 caratteri. Non è possibile aggiornare la riga.");
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }

            //el=event.srcElement.parentNode.parentNode.parentNode.childNodes[5].firstChild;
            el = SourceElement(evt);
            riga = el.parentNode.parentNode.parentNode;
            tab = GetElementByTag(riga, 'TABLE', 1, 0, false);
            //alert(tab);
            cell = tab.rows[riga.rowIndex].cells[5];
            el = GetElementByTag(cell, 'INPUT', 1, 1, false);
            //alert("valore: " + el.value);

            if (isNaN(el.value) || parseInt(el.value) < 0) {
                alert("Il campo 'Validità' contiene un dato non valido. Non è possibile aggiornare la riga.");
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }

            if (el.value == "") {
                alert("Il campo 'Validità' è obbligatirio. Non è possibile aggiornare la riga.");
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }

            if (el.value.length > 3) {
                alert("Il campo 'Validità' non può contenere più di 3 caratteri. Non è possibile aggiornare la riga.");
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }

            //el=event.srcElement.parentNode.parentNode.parentNode.childNodes[6].firstChild;
            el = SourceElement(evt);
            riga = el.parentNode.parentNode.parentNode;
            tab = GetElementByTag(riga, 'TABLE', 1, 0, false);
            //alert(tab);
            cell = tab.rows[riga.rowIndex].cells[6];
            el = GetElementByTag(cell, 'INPUT', 1, 1, false);
            //alert("valore: " + el.value);

            if (el.value == "") {
                alert("Il campo 'Durata' è obbligatorio !! Non è possibile aggiornare la riga.");
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }

            if (isNaN(el.value) || parseInt(el.value) < 0) {
                alert("Il campo 'Durata' contiene un dato non valido. Non è possibile aggiornare la riga.");
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }

            if (el.value.length > 2) {
                alert("Il campo 'Durata' non può contenere più di 2 caratteri. Non è possibile aggiornare la riga.");
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }

            var intervalloProssima = 0;

            //el=event.srcElement.parentNode.parentNode.parentNode.childNodes[12].childNodes[0];
            el = SourceElement(evt);
            riga = el.parentNode.parentNode.parentNode;
            tab = GetElementByTag(riga, 'TABLE', 1, 0, false);
            //alert(tab);
            cell = tab.rows[riga.rowIndex].cells[12];
            el = GetElementByTag(cell, 'INPUT', 1, 1, false);
            //alert("valore: " + el.value);

            if (isNaN(el.value) || parseInt(el.value) < 0) {
                alert("Il campo 'Prossima-Giorni' contiene un dato non valido. Non è possibile aggiornare la riga.");
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }
            else {
                if (el.value != "") intervalloProssima += parseInt(el.value)
            }


            //el=event.srcElement.parentNode.parentNode.parentNode.childNodes[11].childNodes[0];
            el = SourceElement(evt);
            riga = el.parentNode.parentNode.parentNode;
            tab = GetElementByTag(riga, 'TABLE', 1, 0, false);
            //alert(tab);
            cell = tab.rows[riga.rowIndex].cells[11];
            el = GetElementByTag(cell, 'INPUT', 1, 1, false);
            //alert("valore: " + el.value);

            if (isNaN(el.value) || parseInt(el.value) < 0) {
                alert("Il campo 'Prossima-Mesi' contiene un dato non valido. Non è possibile aggiornare la riga.");
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }
            else {
                if (el.value != "") intervalloProssima += parseInt(el.value) * 30
            }

            //el=event.srcElement.parentNode.parentNode.parentNode.childNodes[10].childNodes[0];
            el = SourceElement(evt);
            riga = el.parentNode.parentNode.parentNode;
            tab = GetElementByTag(riga, 'TABLE', 1, 0, false);
            //alert(tab);
            cell = tab.rows[riga.rowIndex].cells[10];
            el = GetElementByTag(cell, 'INPUT', 1, 1, false);
            //alert("valore: " + el.value);

            if (isNaN(el.value) || parseInt(el.value) < 0) {
                alert("Il campo 'Prossima-Anni' contiene un dato non valido. Non è possibile aggiornare la riga.");
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }

            if (parseInt(el.value) >= 200) {
                alert("Il dato 'Prossima-Anni' deve essere inferiore a 200. Non è possibile aggiornare la riga.");
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }
            else {
                if (el.value != "") intervalloProssima += parseInt(el.value) * 360
            }

            var totGiorni = 0;

            //el=event.srcElement.parentNode.parentNode.parentNode.childNodes[9].childNodes[0];
            el = SourceElement(evt);
            riga = el.parentNode.parentNode.parentNode;
            tab = GetElementByTag(riga, 'TABLE', 1, 0, false);
            //alert(tab);
            cell = tab.rows[riga.rowIndex].cells[9];
            el = GetElementByTag(cell, 'INPUT', 1, 1, false);
            //alert("valore: " + el.value);

            if (el.value == '') {
                alert("Il campo 'Età-Giorni' è obbligatorio. Non è possibile aggiornare la riga.");
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }

            if (isNaN(el.value) || parseInt(el.value) < 0) {
                alert("Il campo 'Età-Giorni' contiene un dato non valido. Non è possibile aggiornare la riga.");
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }
            if (el.value != "") totGiorni += parseInt(el.value);

            //el=event.srcElement.parentNode.parentNode.parentNode.childNodes[8].childNodes[0];
            el = SourceElement(evt);
            riga = el.parentNode.parentNode.parentNode;
            tab = GetElementByTag(riga, 'TABLE', 1, 0, false);
            //alert(tab);
            cell = tab.rows[riga.rowIndex].cells[8];
            el = GetElementByTag(cell, 'INPUT', 1, 1, false);
            //alert("valore: " + el.value);

            if (el.value == '') {
                alert("Il campo 'Età-Mesi' è obbligatorio. Non è possibile aggiornare la riga.");
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }

            if (isNaN(el.value) || parseInt(el.value) < 0) {
                alert("Il campo 'Età-Mesi' contiene un dato non valido. Non è possibile aggiornare la riga.");
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }

            if (el.value != "") totGiorni += parseInt(el.value) * 30;

            //el=event.srcElement.parentNode.parentNode.parentNode.childNodes[7].childNodes[0];
            el = SourceElement(evt);
            riga = el.parentNode.parentNode.parentNode;
            tab = GetElementByTag(riga, 'TABLE', 1, 0, false);
            //alert(tab);
            cell = tab.rows[riga.rowIndex].cells[7];
            el = GetElementByTag(cell, 'INPUT', 1, 1, false);
            //alert("valore: " + el.value);

            if (el.value == '') {
                alert("Il campo 'Età-Anni' è obbligatorio. Non è possibile aggiornare la riga.");
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }

            if (isNaN(el.value) || parseInt(el.value) < 0) {
                alert("Il campo 'Età-Anni' contiene un dato non valido. Non è possibile aggiornare la riga.");
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }

            if (el.value != "") totGiorni += parseInt(el.value) * 360;

            if (parseInt(el.value) >= 200) {
                alert("Il dato 'Età-Anni' deve essere inferiore a 200. Non è possibile aggiornare la riga.");
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }

            el = SourceElement(evt);
            riga = el.parentNode.parentNode.parentNode;
            tab = GetElementByTag(riga, 'TABLE', 1, 0, false);
            //alert(tab);
            //cell=tab.rows[riga.rowIndex].cells[3];
            //el=GetElementByTag(cell,'#text',1,1,false);
            //alert("valore: " + el.value);
            //alert(totGiorni);
            valueNode = riga.rowIndex;

            if (totGiorni == 0 && valueNode > 1) {
                alert("Solo la prima seduta vaccinale può essere effettuata all'età di 0 giorni. Non è possibile aggiornare la riga.");
                //event.srcElement.parentNode.parentNode.parentNode.childNodes[7].childNodes[0].focus();
                el = SourceElement(evt);
                riga = el.parentNode.parentNode.parentNode;
                tab = GetElementByTag(riga, 'TABLE', 1, 0, false);
                //alert(tab);
                cell = tab.rows[riga.rowIndex].cells[7];
                el = GetElementByTag(cell, 'INPUT', 1, 1, false);
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }

            var etaPrecedente = parseInt(document.getElementById("etaPrec").value);
            if (totGiorni < etaPrecedente) {
                alert("Attenzione: ogni seduta vaccinale deve essere effettuata ad una età superiore alla precedente.");
                el = SourceElement(evt);
                riga = el.parentNode.parentNode.parentNode;
                tab = GetElementByTag(riga, 'TABLE', 1, 0, false);
                //alert(tab);
                cell = tab.rows[riga.rowIndex].cells[7];
                el = GetElementByTag(cell, 'INPUT', 1, 1, false);
                el.focus();
                /*evt.returnValue=false;
		        StopPreventDefault(evt);
		        return false;*/
            }
            etaSuccessiva = parseInt(document.getElementById("etaSuc").value);

            if (totGiorni > etaSuccessiva - intervalloProssima) {
                alert("Attenzione: ogni seduta vaccinale deve essere effettuata ad una età inferiore alla successiva.");
                el = SourceElement(evt);
                riga = el.parentNode.parentNode.parentNode;
                tab = GetElementByTag(riga, 'TABLE', 1, 0, false);
                //alert(tab);
                cell = tab.rows[riga.rowIndex].cells[7];
                el = GetElementByTag(cell, 'INPUT', 1, 1, false);
                el.focus();
                /*evt.returnValue=false;
		        StopPreventDefault(evt);
		        return false;*/
            }

            el = SourceElement(evt);
            riga = el.parentNode.parentNode.parentNode;
            tab = GetElementByTag(riga, 'TABLE', 1, 0, false);
            cell = tab.rows[riga.rowIndex].cells[13];
            el = GetElementByTag(cell, 'INPUT', 1, 1, false);

            if (isNaN(el.value) || (el.value < 0)) {
                alert("Il campo numero solleciti deve contenere un valore numerico maggiore di 0.");
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }

            el = SourceElement(evt);
            riga = el.parentNode.parentNode.parentNode;
            tab = GetElementByTag(riga, 'TABLE', 1, 0, false);
            cell = tab.rows[riga.rowIndex].cells[14];
            el = GetElementByTag(cell, 'INPUT', 1, 1, false);

            if (el.value.length > 240) {
                alert("Il campo 'Note' non può contenere più di 240 caratteri. Non è possibile aggiornare la riga.");
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }
        }

        function controlla_vac(evt) {
            el = SourceElement(evt);
            riga = el.parentNode.parentNode.parentNode;
            tab = GetElementByTag(riga, 'TABLE', 1, 0, false);
            cell = tab.rows[riga.rowIndex].cells[3];
            el = GetElementByTag(cell, 'INPUT', 1, 1, false);

            if (el.value == "") {
                alert("Il campo 'Vaccinazioni' è vuoto. Non è possibile aggiornare la riga.");
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }

            //controllo sull'inserimento del sito di inoculazione
            cell = tab.rows[riga.rowIndex].cells[4];
            el = GetElementByTag(cell, 'INPUT', 1, 1, false);
            var descrizioneSito = el.value;
            var codiceSito = el.nextSibling.value;

            if (((codiceSito == "") && (descrizioneSito != "")) || ((codiceSito != "") && (descrizioneSito == ""))) {
                alert("Il campo 'Sito di Inoculazione' non è corretto. Non è possibile aggiornare la riga.");
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }
        }

        function controlla_ass(evt) {
            el = SourceElement(evt);
            riga = el.parentNode.parentNode.parentNode;
            tab = GetElementByTag(riga, 'TABLE', 1, 0, false);
            cell = tab.rows[riga.rowIndex].cells[4];
            el = GetElementByTag(cell, 'INPUT', 1, 1, false);

            if (el.value == "") {
                alert("Il campo 'Associazioni' è vuoto. Non è possibile aggiornare la riga.");
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }

            //controllo sull'inserimento del sito di inoculazione
            cell = tab.rows[riga.rowIndex].cells[4];
            el = GetElementByTag(cell, 'INPUT', 1, 1, false);
            var descrizioneSito = el.value;
            var codiceSito = el.nextSibling.value;
            if (((codiceSito == "") && (descrizioneSito != "")) || ((codiceSito != "") && (descrizioneSito == ""))) {
                alert("Il campo 'Sito di Inoculazione' non è corretto. Non è possibile aggiornare la riga.");
                el.focus();
                evt.returnValue = false;
                StopPreventDefault(evt);
                return false;
            }
        }

        //controllo sui parametri delle singole vaccinazioni [modifica 28/03/2006]
        function controllaNumero(txt) {
            var valoreTxt = txt.value;

            if (valoreTxt.replace(' ', '') == '') {
                txt.value = '0';
            }
            else {
                if (isNaN(valoreTxt)) {
                    alert('Attenzione: il campo selezionato deve essere di tipo numerico!');
                    txt.focus();
                }
            }
        }

        //valorizzazione del sollecito e di quello raccomandato [modifica 12/04/2006]
        function controllaSollecitoRaccomandato(txt) {
            var valoreTxt = txt.value;

            if (valoreTxt.replace(' ', '') == '') {
                document.getElementById('txtNumSol').readOnly = false;
                document.getElementById('txtNumSol').className = 'textbox_numerico_obbligatorio';
                document.getElementById('txtNumSol').focus();
            }
            else {
                document.getElementById('txtNumSol').readOnly = true;
                document.getElementById('txtNumSol').className = 'textbox_numerico_disabilitato';
                document.getElementById('txtNumSol').value = '';
            }
        }
    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout1" runat="server" Titolo="Sedute" Height="100%" Width="100%">
            <div class="title" id="divLayoutTitolo_sezione1" style="width: 100%">
                <asp:Label ID="LayoutTitolo" runat="server" CssClass="Title" Width="100%"></asp:Label>
            </div>
            <div>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                    <Items>
                        <igtbar:TBarButton Key="btn_Salva" DisabledImage="~/Images/salva.gif" Text="Salva" Image="~/Images/salva.gif"></igtbar:TBarButton>
                        <igtbar:TBarButton Key="btn_Annulla" DisabledImage="~/Images/annulla.gif" Text="Annulla"
                            Image="~/Images/annulla.gif">
                        </igtbar:TBarButton>
                    </Items>
                </igtbar:UltraWebToolbar>
            </div>
            <div class="sezione" id="divLayoutTitolo_sezioneSed" style="width: 100%">
                <asp:Label ID="LayoutTitolo_sezioneSed" runat="server" Width="100%" Visible="true">ELENCO SEDUTE</asp:Label>
            </div>

            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                <asp:Panel ID="pan_sedute" runat="server" Width="100%">
                    <asp:DataGrid ID="dg_sedute" runat="server" CssClass="DATAGRID" Width="100%" AllowPaging="True"
                        PageSize="7" AutoGenerateColumns="False" CellPadding="1" GridLines="None">
                        <SelectedItemStyle Font-Bold="True" CssClass="SELECTED"></SelectedItemStyle>
                        <AlternatingItemStyle CssClass="ALTERNATING"></AlternatingItemStyle>
                        <ItemStyle CssClass="ITEM"></ItemStyle>
                        <HeaderStyle Font-Bold="True" CssClass="HEADER"></HeaderStyle>
                        <Columns>
                            <asp:ButtonColumn Text="&lt;img  title=&quot;Visualizza associazioni e vaccinazioni&quot; src=&quot;../../../images/seleziona.gif&quot;&gt;"
                                HeaderText="&lt;img style=&quot;cursor:hand&quot; title=&quot;Inserisci seduta&quot; src=&quot;../../../images/nuovo.gif&quot; onclick=&quot;NuovaSed()&quot;&gt;"
                                CommandName="Select">
                                <HeaderStyle HorizontalAlign="Center" Width="1%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:ButtonColumn>
                            <asp:ButtonColumn Text="&lt;img  title=&quot;Elimina&quot; src=&quot;../../../images/elimina.gif&quot; onclick=&quot;if(!confirm('La riga sar&#224; eliminata..continuare?')){event.returnValue=false}&quot;&gt;"
                                CommandName="Delete">
                                <HeaderStyle HorizontalAlign="Center" Width="1%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:ButtonColumn>
                            <asp:EditCommandColumn ButtonType="LinkButton" UpdateText="&lt;img  title=&quot;Aggiorna&quot; src=&quot;../../../images/conferma.gif&quot; onClick=&quot;controlla_sed(event)&quot;&gt;"
                                CancelText="&lt;img  title=&quot;Annulla&quot; src=&quot;../../../images/annullaconf.gif&quot;&gt;"
                                EditText="&lt;img  title=&quot;Modifica&quot; src=&quot;../../../images/modifica.gif&quot; &gt;">
                                <HeaderStyle HorizontalAlign="Center" Width="1%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:EditCommandColumn>
                            <asp:TemplateColumn HeaderText="N.Seduta">
                                <HeaderStyle HorizontalAlign="Left" Width="5%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                <ItemTemplate>
                                    <asp:Label ID="lb_index" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("tsd_n_seduta") %>' Font-Bold="True">
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="lb_index_edit" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("tsd_n_seduta") %>' Font-Bold="True">
                                    </asp:Label>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Intervallo &lt;br&gt;&lt;span style=&quot;font-size:12px;font-weight:lighter&quot;&gt;Giorni&lt;/span&gt;">
                                <HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                <ItemTemplate>
                                    <asp:Label ID="tb_int" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("tsd_intervallo") %>'>
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="tb_int_edit" runat="server" Width="45px" Text='<%# DataBinder.Eval(Container, "DataItem")("tsd_intervallo") %>' CssClass="textbox_numerico_obbligatorio">
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Validit&#224;&lt;br&gt;&lt;span style=&quot;font-size:12px;font-weight:lighter&quot;&gt;Giorni&lt;/span&gt;">
                                <HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                <ItemTemplate>
                                    <asp:Label ID="tb_val" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("tsd_validita") %>'>
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="tb_val_edit" runat="server" Width="43px" CssClass="textbox_numerico_obbligatorio" Text='<%# DataBinder.Eval(Container, "DataItem")("tsd_validita") %>'>
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Durata&lt;br&gt;&lt;span style=&quot;font-size:12px;font-weight:lighter&quot;&gt;Minuti&lt;/span&gt;">
                                <HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                <ItemTemplate>
                                    <asp:Label ID="tb_dur" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("tsd_durata_seduta") %>'>
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="tb_dur_edit" runat="server" CssClass="textbox_numerico_obbligatorio" Width="31px" Text='<%# DataBinder.Eval(Container, "DataItem")("tsd_durata_seduta") %>'>
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="&lt;br&gt;&lt;span style=&quot;font-weight:lighter&quot;&gt;Anni&lt;/span&gt;">
                                <HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                <ItemTemplate>
                                    <asp:Label ID="tb_aa" runat="server" CssClass="label" Text='<%# Conv(DataBinder.Eval(Container, "DataItem")("tsd_eta_seduta"),"aa") %>'>
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="tb_aa_edit" runat="server" Style="width: 100%" CssClass="textbox_numerico_obbligatorio" Text='<%# Conv(DataBinder.Eval(Container, "DataItem")("tsd_eta_seduta"),"aa") %>'>
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Et&#224;&lt;br&gt;&lt;span style=&quot;font-weight:lighter&quot;&gt;Mesi&lt;/span&gt;">
                                <HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                <ItemTemplate>
                                    <asp:Label ID="tb_mm" runat="server" CssClass="label" Text='<%# Conv(DataBinder.Eval(Container, "DataItem")("tsd_eta_seduta"),"mm") %>'>
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="tb_mm_edit" runat="server" Style="width: 100%" CssClass="textbox_numerico_obbligatorio" Text='<%# Conv(DataBinder.Eval(Container, "DataItem")("tsd_eta_seduta"),"mm") %>'>
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="&lt;br&gt;&lt;span style=&quot;font-weight:lighter&quot;&gt;Giorni&lt;/span&gt;">
                                <HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                <ItemTemplate>
                                    <asp:Label ID="tb_gg" runat="server" CssClass="label" Text='<%# Conv(DataBinder.Eval(Container, "DataItem")("tsd_eta_seduta"),"gg") %>'>
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="tb_gg_edit" runat="server" Style="width: 100%" CssClass="textbox_numerico_obbligatorio" Text='<%# Conv(DataBinder.Eval(Container, "DataItem")("tsd_eta_seduta"),"gg") %>'>
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="&lt;br&gt;&lt;span style=&quot;font-weight:lighter&quot;&gt;Anni&lt;/span&gt;">
                                <HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                <ItemTemplate>
                                    <asp:Label ID="tb_pros_aa" runat="server" CssClass="label" Text='<%# Conv(DataBinder.Eval(Container, "DataItem")("tsd_intervallo_prossima"),"aa") %>'>
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="tb_pros_aa_edit" runat="server" Style="width: 100%" CssClass="textbox_numerico" Text='<%# Conv(DataBinder.Eval(Container, "DataItem")("tsd_intervallo_prossima"),"aa") %>'>
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Prossima&lt;br&gt;&lt;span style=&quot;font-size:12px;font-weight:lighter&quot;&gt;Mesi&lt;/span&gt;">
                                <HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                <ItemTemplate>
                                    <asp:Label ID="tb_pros_mm" runat="server" CssClass="label" Text='<%# Conv(DataBinder.Eval(Container, "DataItem")("tsd_intervallo_prossima"),"mm") %>'>
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="tb_pros_mm_edit" runat="server" Style="width: 100%" CssClass="textbox_numerico" Text='<%# Conv(DataBinder.Eval(Container, "DataItem")("tsd_intervallo_prossima"),"mm") %>'>
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="&lt;br&gt;&lt;span style=&quot;font-weight:lighter&quot;&gt;Giorni&lt;/span&gt;">
                                <HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                <ItemTemplate>
                                    <asp:Label ID="tb_pros_gg" runat="server" CssClass="label" Text='<%# Conv(DataBinder.Eval(Container, "DataItem")("tsd_intervallo_prossima"),"gg") %>'>
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="tb_pros_gg_edit" runat="server" Style="width: 100%" CssClass="textbox_numerico" Text='<%# Conv(DataBinder.Eval(Container, "DataItem")("tsd_intervallo_prossima"),"gg") %>'>
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn>
                                <HeaderStyle HorizontalAlign="Center" Width="0px"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                <ItemTemplate>
                                    <asp:Label ID="lblNumSollecitiRac" Style="text-align: center" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("tsd_num_solleciti_rac") %>'>
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtNumSollecitiRac" Style="text-align: center" runat="server" CssClass="textbox_stringa" Width="40" Text='<%# DataBinder.Eval(Container, "DataItem")("tsd_num_solleciti_rac") %>'>
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn>
                                <HeaderStyle HorizontalAlign="Center" Width="0px"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                <ItemTemplate>
                                    <asp:Label ID="lblNumSolleciti" Style="text-align: center;" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("tsd_num_solleciti") %>'>
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtNumSolleciti" Style="text-align: center;" runat="server" CssClass="textbox_stringa" Width="40" Text='<%# DataBinder.Eval(Container, "DataItem")("tsd_num_solleciti") %>'>
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn>
                                <HeaderStyle HorizontalAlign="Center" Width="0px"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                <ItemTemplate>
                                    <asp:Label ID="lblGiorniPosticipo" Style="text-align: center;" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("tsd_giorni_posticipo") %>'>
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtGiorniPosticipo" Style="text-align: center;" runat="server" CssClass="textbox_stringa" Width="40" Text='<%# DataBinder.Eval(Container, "DataItem")("tsd_giorni_posticipo") %>'>
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn>
                                <HeaderStyle HorizontalAlign="Center" Width="0px"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                <ItemTemplate>
                                    <asp:Label ID="lblSollecitiNonObbligatori" Style="text-align: center;" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("tsd_num_soll_non_obbl") %>'>
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtSollecitiNonObbligatori" Style="text-align: center;" runat="server" CssClass="textbox_stringa" Width="40" Text='<%# DataBinder.Eval(Container, "DataItem")("tsd_num_soll_non_obbl") %>'>
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn>
                                <HeaderStyle HorizontalAlign="Center" Width="0px"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                <ItemTemplate>
                                    <asp:Label ID="lblPosticipoSeduta" Style="text-align: center;" runat="server" CssClass="label" Text='<%# SelezionaPosticipoSeduta(DataBinder.Eval(Container, "DataItem")("tsd_posticipo_seduta")) %>'>
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="ddlPosticipoSeduta" runat="server">
                                        <asp:ListItem Value="N" Selected="True">NO</asp:ListItem>
                                        <asp:ListItem Value="S">SI</asp:ListItem>
                                    </asp:DropDownList>
                                    <!--<asp:CheckBox id="chkPosticipoSeduta" runat="server" Checked='<%# IIf(DataBinder.Eval(Container, "DataItem")("tsd_posticipo_seduta") = "S", True, False) %>'>
									    </asp:CheckBox>-->
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn>
                                <HeaderStyle HorizontalAlign="Center" Width="3%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                <ItemTemplate>
                                    <asp:Image ID="imgSeduteParametri" Style="text-align: center; cursor: auto;" ToolTip="Visualizza i parametri associati alla Seduta (modificare la riga per accedere ai parametri)." runat="server" ImageUrl='<%# IIf(VisualizzaImmagineParametri(Container),"../../../Images/sedute_parametri_ok.gif", "../../../Images/sedute_parametri_no.gif") %>'></asp:Image>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:ImageButton ID="imbSeduteParametri" Style="text-align: center; cursor: hand;" ToolTip="Visualizza i parametri associati alla Seduta." runat="server" ImageUrl='<%# IIf(VisualizzaImmagineParametri(Container),"../../../Images/sedute_parametri_ok.gif", "../../../Images/sedute_parametri_no.gif") %>' CommandName="VisualizzaParametri" CommandArgument='<%# DataBinder.Eval(Container, "DataItem")("tsd_n_seduta") %>'></asp:ImageButton>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Note">
                                <HeaderStyle HorizontalAlign="Center" Width="19%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                <ItemTemplate>
                                    <asp:Label ID="tb_note" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("tsd_note") %>'>
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="tb_note_edit" runat="server" Style="width: 100%" CssClass="textbox_stringa" Text='<%# DataBinder.Eval(Container, "DataItem")("tsd_note") %>'>
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                        <PagerStyle CssClass="PAGER" Mode="NumericPages"></PagerStyle>
                    </asp:DataGrid>
                    <div class="sezione" id="divLayoutTitolo_sezioneAss" style="width: 100%">
                        <asp:Label ID="sezioneAss" runat="server" Width="100%" Visible="false">ELENCO ASSOCIAZIONI</asp:Label>
                    </div>
                    <asp:DataGrid ID="dg_ass_sed" runat="server" CssClass="datagrid" Width="100%" AllowPaging="True"
                        PageSize="7" AutoGenerateColumns="False" CellPadding="1" GridLines="None">
                        <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
                        <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
                        <ItemStyle CssClass="item"></ItemStyle>
                        <HeaderStyle CssClass="header"></HeaderStyle>
                        <Columns>
                            <asp:ButtonColumn Text="&lt;img  title=&quot;Visualizza vaccinazioni&quot; src=&quot;../../../images/seleziona.gif&quot;&gt;"
                                HeaderText="&lt;img   style=&quot;cursor:hand&quot; title=&quot;Inserisci associazione&quot; src=&quot;../../../images/nuovo.gif&quot; onclick=&quot;NuovaAss()&quot;&gt;"
                                CommandName="Select"></asp:ButtonColumn>
                            <asp:ButtonColumn Text="&lt;img  title=&quot;Elimina&quot; src=&quot;../../../images/elimina.gif&quot; onclick=&quot;if(!confirm('La riga sar&#224; eliminata. Continuare?')){event.returnValue=false}&quot;&gt;"
                                CommandName="Delete"></asp:ButtonColumn>
                            <asp:EditCommandColumn ButtonType="LinkButton" UpdateText="&lt;img  title=&quot;Conferma&quot; src=&quot;../../../images/conferma.gif&quot; onClick=&quot;controlla_ass(event)&quot;&gt;"
                                CancelText="&lt;img  title=&quot;Annulla&quot; src=&quot;../../../images/annullaconf.gif&quot;&gt;"
                                EditText="&lt;img  title=&quot;Modifica&quot; src=&quot;../../../images/modifica.gif&quot;&gt;"></asp:EditCommandColumn>
                            <asp:TemplateColumn HeaderText="Dose">
                                <HeaderStyle HorizontalAlign="Center" Width="8%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("sas_n_richiamo") %>'>
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="tb_n_ric_edit_ass" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("sas_n_richiamo") %>'>
                                    </asp:Label>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn>
                                <HeaderStyle HorizontalAlign="Center" Width="30%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                <HeaderTemplate>
                                    <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                        <tr>
                                            <td style="padding-left: 80px" align="center" colspan="2">Associazione</td>
                                        </tr>
                                        <tr>
                                            <td style="font-weight: lighter" align="center" width="70%">Descrizione</td>
                                            <td style="font-weight: lighter" align="center" width="30%">Codice</td>
                                        </tr>
                                    </table>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                        <tr>
                                            <td width="70%">
                                                <asp:Label ID="tb_descAss" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("ass_descrizione") %>'>
                                                </asp:Label></td>
                                            <td width="30%">
                                                <asp:Label ID="tb_codAss" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("sas_ass_codice") %>'>
                                                </asp:Label></td>
                                        </tr>
                                    </table>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <on_ofm:OnitModalList ID="fm_ass_edit" runat="server" Width="70%" PosizionamentoFacile="False" LabelWidth="-1px" CodiceWidth="30%" UseCode="True" Obbligatorio="True" SetUpperCase="True" CampoCodice="ass_codice" CampoDescrizione="ass_descrizione" Connection="" Tabella="t_ana_associazioni" Descrizione='<%# DataBinder.Eval(Container, "DataItem")("ass_descrizione") %>' Codice='<%# DataBinder.Eval(Container, "DataItem")("sas_ass_codice") %>' RaiseChangeEvent="False"></on_ofm:OnitModalList>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn>
                                <HeaderStyle HorizontalAlign="Center" Width="25%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                <HeaderTemplate>
                                    <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                        <tr>
                                            <td style="padding-left: 80px" align="center" colspan="2">Via di Somministrazione</td>
                                        </tr>
                                        <tr>
                                            <td style="font-weight: lighter" align="center" width="70%">Descrizione</td>
                                            <td style="font-weight: lighter" align="center" width="30%">Codice</td>
                                        </tr>
                                    </table>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                        <tr>
                                            <td width="70%">
                                                <asp:Label ID="tb_descVii_ass" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("vii_descrizione") %>'></asp:Label>
                                            </td>
                                            <td width="30%">
                                                <asp:Label ID="tb_codVii_ass" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("sas_vii_codice") %>'></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <on_ofm:OnitModalList ID="fm_vii_edit_ass" runat="server" Width="70%" Filtro="'true'='true' order by vii_descrizione" PosizionamentoFacile="False" LabelWidth="-1px" CodiceWidth="30%" UseCode="True" Obbligatorio="False" SetUpperCase="True" CampoCodice="vii_codice" CampoDescrizione="vii_descrizione" Connection="" Tabella="t_ana_vie_somministrazione" Descrizione='<%# DataBinder.Eval(Container, "DataItem")("vii_descrizione") %>' Codice='<%# DataBinder.Eval(Container, "DataItem")("sas_vii_codice") %>' RaiseChangeEvent="False"></on_ofm:OnitModalList>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn>
                                <HeaderStyle HorizontalAlign="Center" Width="25%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                <HeaderTemplate>
                                    <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                        <tr>
                                            <td style="padding-left: 80px" align="center" colspan="2">Sito di Inoculazione</td>
                                        </tr>
                                        <tr>
                                            <td style="font-weight: lighter" align="center" width="70%">Descrizione</td>
                                            <td style="font-weight: lighter" align="center" width="30%">Codice</td>
                                        </tr>
                                    </table>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                        <tr>
                                            <td width="70%">
                                                <asp:Label ID="tb_descSii_ass" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("sii_descrizione") %>'></asp:Label>
                                            </td>
                                            <td width="30%">
                                                <asp:Label ID="tb_codSii_ass" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("sas_sii_codice") %>'></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <on_ofm:OnitModalList ID="fm_sii_edit_ass" runat="server" Width="70%" Filtro="'true'='true' order by sii_descrizione" PosizionamentoFacile="False" LabelWidth="-1px" CodiceWidth="30%" UseCode="True" Obbligatorio="False" SetUpperCase="True" CampoCodice="sii_codice" CampoDescrizione="sii_descrizione" Connection="" Tabella="t_ana_siti_inoculazione" Descrizione='<%# DataBinder.Eval(Container, "DataItem")("sii_descrizione") %>' Codice='<%# DataBinder.Eval(Container, "DataItem")("sas_sii_codice") %>' RaiseChangeEvent="False"></on_ofm:OnitModalList>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn>
                                <HeaderStyle Width="10%"></HeaderStyle>
                            </asp:TemplateColumn>
                        </Columns>
                        <PagerStyle CssClass="pager" Mode="NumericPages"></PagerStyle>
                    </asp:DataGrid>
                    <div class="sezione" id="divLayoutTitolo_sezioneVac" style="width: 100%">
                        <asp:Label ID="sezioneVac" runat="server" Width="100%" Visible="false">ELENCO VACCINAZIONI</asp:Label>
                    </div>
                    <asp:DataGrid ID="dg_vac_sed" runat="server" CssClass="DATAGRID" Width="100%" AllowPaging="True"
                        PageSize="7" AutoGenerateColumns="False" CellPadding="1" GridLines="None">
                        <SelectedItemStyle Font-Bold="True" CssClass="Selected"></SelectedItemStyle>
                        <AlternatingItemStyle CssClass="ALTERNATING"></AlternatingItemStyle>
                        <ItemStyle CssClass="ITEM"></ItemStyle>
                        <HeaderStyle Font-Bold="True" CssClass="HEADER"></HeaderStyle>
                        <PagerStyle CssClass="PAGER" Mode="NumericPages"></PagerStyle>
                        <Columns>
                            <asp:ButtonColumn Text="" HeaderText="" CommandName="Select">
                                <ItemStyle HorizontalAlign="Center" Width="2%"></ItemStyle>
                            </asp:ButtonColumn>
                            <asp:ButtonColumn Text="&lt;img  title=&quot;Elimina&quot; src=&quot;../../../images/elimina.gif&quot; onclick=&quot;if(!confirm('La riga sar&#224; eliminata. Continuare?')){event.returnValue=false}&quot;&gt;"
                                HeaderText="" CommandName="Delete">
                                <HeaderStyle HorizontalAlign="Center" Width="1%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:ButtonColumn>
                            <asp:TemplateColumn HeaderText="Dose">
                                <HeaderStyle HorizontalAlign="Center" Width="8%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                <ItemTemplate>
                                    <asp:Label ID="Label3" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("sed_n_richiamo") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="tb_n_ric_edit" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("sed_n_richiamo") %>'></asp:Label>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn>
                                <HeaderStyle HorizontalAlign="Center" Width="40%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                <HeaderTemplate>
                                    <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                        <tr>
                                            <td style="padding-left: 80px" align="center" colspan="2">Vaccinazione</td>
                                        </tr>
                                        <tr>
                                            <td style="font-weight: lighter" align="center" width="70%">Descrizione</td>
                                            <td style="font-weight: lighter" align="center" width="30%">Codice</td>
                                        </tr>
                                    </table>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                        <tr>
                                            <td width="70%">
                                                <asp:Label ID="tb_descVac" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("vac_descrizione") %>'>
                                                </asp:Label></td>
                                            <td width="30%">
                                                <asp:Label ID="tb_codVac" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("sed_vac_codice") %>'>
                                                </asp:Label></td>
                                        </tr>
                                    </table>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <on_ofm:OnitModalList ID="fm_vac_edit" runat="server" Width="70%" AltriCampi="vac_obbligatoria" PosizionamentoFacile="False" LabelWidth="-1px" CodiceWidth="30%" UseCode="True" Obbligatorio="True" SetUpperCase="True" CampoCodice="vac_codice" CampoDescrizione="vac_descrizione" Connection="" Tabella="t_ana_vaccinazioni" Descrizione='<%# DataBinder.Eval(Container, "DataItem")("vac_descrizione") %>' Codice='<%# DataBinder.Eval(Container, "DataItem")("sed_vac_codice") %>' RaiseChangeEvent="False"></on_ofm:OnitModalList>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn>
                                <HeaderStyle HorizontalAlign="Center" Width="40%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                <HeaderTemplate>
                                    <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                        <tr>
                                            <td style="padding-left: 80px" align="center" colspan="2">Sito di Inoculazione</td>
                                        </tr>
                                        <tr>
                                            <td style="font-weight: lighter" align="center" width="70%">Descrizione</td>
                                            <td style="font-weight: lighter" align="center" width="30%">Codice</td>
                                        </tr>
                                    </table>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                        <tr>
                                            <td width="70%">
                                                <asp:Label ID="tb_descSii" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("sii_descrizione") %>'>
                                                </asp:Label></td>
                                            <td width="30%">
                                                <asp:Label ID="tb_codSii" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("sed_sii_codice") %>'>
                                                </asp:Label></td>
                                        </tr>
                                    </table>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <on_ofm:OnitModalList ID="fm_sii_edit" runat="server" Width="70%" Filtro="'true'='true' order by sii_descrizione" PosizionamentoFacile="False" LabelWidth="-1px" CodiceWidth="30%" UseCode="True" Obbligatorio="False" SetUpperCase="True" CampoCodice="sii_codice" CampoDescrizione="sii_descrizione" Connection="" Tabella="t_ana_siti_inoculazione" Descrizione='<%# DataBinder.Eval(Container, "DataItem")("sii_descrizione") %>' Codice='<%# DataBinder.Eval(Container, "DataItem")("sed_sii_codice") %>' RaiseChangeEvent="False"></on_ofm:OnitModalList>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn>
                                <HeaderStyle Width="10%"></HeaderStyle>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                </asp:Panel>
            </dyp:DynamicPanel>

            <asp:TextBox ID="nascosto" Style="visibility: hidden" runat="server" Width="74px"></asp:TextBox>
            <asp:TextBox ID="etaPrec" Style="visibility: hidden" runat="server" Width="71px"></asp:TextBox>
            <asp:TextBox ID="etaSuc" Style="visibility: hidden" runat="server" Width="71px"></asp:TextBox>
            
            <on_ofm:OnitFinestraModale ID="fmVisualizzaParametri" runat="server" Width="500px" BackColor="LightGray">
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="uwtVisualizzaParametri" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                    <ClientSideEvents InitializeToolbar="InizializzaToolBarVisualizzaParametri" Click="ToolBarClickVisualizzaParametri"></ClientSideEvents>
                    <Items>
                        <igtbar:TBarButton Key="btnConferma" DisabledImage="~/Images/conferma_dis.gif" Text="Conferma"
                            Image="~/Images/conferma.gif">
                        </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btnAnnulla" DisabledImage="~/Images/annulla_dis.gif" Text="Annulla"
                            Image="~/Images/annulla.gif">
                        </igtbar:TBarButton>
                    </Items>
                </igtbar:UltraWebToolbar>
                <div id="parametriVacObbligatorie" style="width: 100%; height: 50%">
                    <table height="100%" cellspacing="3" cellpadding="3" width="100%" border="0">
                        <tr>
                            <td class="label" width="28%">Numero Solleciti Globale</td>
                            <td width="7%">
                                <asp:TextBox ID="txtNumSolGlobal" CssClass="textbox_numerico_disabilitato" Width="40px" ReadOnly="True"
                                    runat="server"></asp:TextBox></td>
                            <td class="label" width="28%">Numero Solleciti</td>
                            <td width="7%">
                                <asp:TextBox ID="txtNumSol" CssClass="textbox_numerico_obbligatorio" Width="40px" runat="server"></asp:TextBox></td>
                            <td class="label" width="28%" rowspan="2">Numero Solleciti Raccomandati</td>
                            <td width="6%">
                                <asp:TextBox ID="txtNumSolRac" onblur="controllaSollecitoRaccomandato(this);" CssClass="textbox_numerico_obbligatorio"
                                    Width="40px" runat="server"></asp:TextBox></td>
                        </tr>
                    </table>
                </div>
                <div id="parametriVacNonObbligatorie" style="width: 100%; height: 50%">
                    <table height="100%" cellspacing="3" cellpadding="3" width="100%" border="0">
                        <tr>
                            <td class="label" width="28%">Giorni Posticipo</td>
                            <td width="6%">
                                <asp:TextBox ID="txtGiorniPost" onblur="controllaNumero(this);" CssClass="textbox_numerico_obbligatorio"
                                    Width="40px" runat="server"></asp:TextBox></td>
                            <td class="label" width="28%">Numero Solleciti Non Obbligatori</td>
                            <td width="7%">
                                <asp:TextBox ID="txtNumSolNonObbl" onblur="controllaNumero(this);" CssClass="textbox_numerico_obbligatorio"
                                    Width="40px" runat="server"></asp:TextBox></td>
                            <td class="label" width="28%">Posticipo Seduta</td>
                            <td width="6%">
                                <asp:DropDownList ID="ddlPostSed" runat="server">
                                    <asp:ListItem Value="N" Selected="True">NO</asp:ListItem>
                                    <asp:ListItem Value="S">SI</asp:ListItem>
                                </asp:DropDownList></td>
                        </tr>
                    </table>
                </div>
            </on_ofm:OnitFinestraModale>
        </on_lay3:OnitLayout3>
    </form>
    <% response.write(strJS) %>
    <script type="text/javascript" language="javascript">
        //non deve visualizzare le colonne con i parametri della seduta [modifica 29/03/2006]
        var dgSedute = document.getElementById('dg_sedute');
        for (i = 0; i < dgSedute.rows.length - 1; i++) {
            dgSedute.rows[i].cells[13].style.display = 'none';
            dgSedute.rows[i].cells[14].style.display = 'none';
            dgSedute.rows[i].cells[15].style.display = 'none';
            dgSedute.rows[i].cells[16].style.display = 'none';
            dgSedute.rows[i].cells[17].style.display = 'none';
        }
    </script>
</body>
</html>
