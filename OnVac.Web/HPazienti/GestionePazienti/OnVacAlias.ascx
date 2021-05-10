<%@ Control Language="vb" AutoEventWireup="false" Codebehind="OnVacAlias.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVacAlias" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<script type="text/javascript">
    function InitToolbar(t) {
        t.PostBackButton = false;
    }
		
    function toolbarAliasClick(oToolBar, oItem, oEvent) {

        if (oToolBar.Enabled) {

            oEvent.needPostBack = true;

            switch (oItem.Key) {

                case "btnAnnulla":
                    oEvent.needPostBack = false;
                    closeFm('fmOnVacAlias');
                    break;

                case "btnConferma":

                    // Restituisce l'oggetto grid in base al client id.
                    var oGrid = get_OnitGrid('<%= dgrListaAlias.ClientId %>');

                    if (oGrid != null) {

                        if (oGrid.SelectedIndex < 0) {
                            alert("Impossibile effettuare il merge: nessun paziente selezionato come master.");
                            oEvent.needPostBack = false;
                            return;
                        }
                        else {

                            // Restituisce il valore contenuto nella cella indicata dalla key della riga selezionata
                            var codiceAux = oGrid.getSelText('PAZ_CODICE_AUSILIARIO');

                            if (codiceAux == null) {
                                if (!confirm("Il paziente scelto non ha il codice ausiliario. Si desidera proseguire?")) {
                                    oEvent.needPostBack = false;
                                    return;
                                }
                            }

                            if (!confirm("Attenzione: questa operazione comporta l'eliminazione degli alias. Si desidera proseguire?")) {
                                oEvent.needPostBack = false;
                                return;
                            }
                        }
                    }
                    break;
            }
        }
        return;
    }
</script>

<div>
    <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="toolbarAlias" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
	    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
	    <ClientSideEvents InitializeToolbar="InitToolbar" Click="toolbarAliasClick"></ClientSideEvents>
        <Items>
		    <igtbar:TBarButton Key="btnConferma" Text="Conferma" Image="~/Images/conferma.gif"></igtbar:TBarButton>
		    <igtbar:TBarButton Key="btnAnnulla" Text="Annulla" Image="~/Images/annulla.gif"></igtbar:TBarButton>
	    </Items>
    </igtbar:ultrawebtoolbar>
</div>

<div class="sezione" style="width: 100%;">
	Selezionare il paziente master dalla lista e confermare per effettuare il merge
</div>

<dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true" style="vertical-align:top">
    <on_dgr:OnitGrid ID="dgrListaAlias" runat="server" Width="100%" CssClass="datagrid" AutoGenerateColumns="False" SortedColumns="Matrice IGridColumn[]"
        PagerVoicesBefore="-1" PagerVoicesAfter="-1" SelectionOption="clientOnly">
	    <AlternatingItemStyle CssClass="alternating"/>
	    <ItemStyle CssClass="item"/>
	    <HeaderStyle CssClass="header"/>
        <SelectedItemStyle CssClass="selected" />
	    <Columns>
		    <on_dgr:OnitBoundColumn HeaderText="Cancellato" Key="PAZ_CANCELLATO" DataField="PAZ_CANCELLATO" />
		    <on_dgr:OnitBoundColumn HeaderText="Cod. ausiliario" Key="PAZ_CODICE_AUSILIARIO" DataField="PAZ_CODICE_AUSILIARIO"/>
		    <on_dgr:OnitBoundColumn HeaderText="Codice" Key="PAZ_CODICE" DataField="PAZ_CODICE" />
		    <on_dgr:OnitBoundColumn HeaderText="Cognome" Key="PAZ_COGNOME"  DataField="PAZ_COGNOME"/>
		    <on_dgr:OnitBoundColumn HeaderText="Nome" Key="PAZ_NOME"  DataField="PAZ_NOME"/>
		    <on_dgr:OnitBoundColumn HeaderText="Sesso" Key="PAZ_SESSO" DataField="PAZ_SESSO"/>
		    <on_dgr:OnitBoundColumn HeaderText="Data nascita" Key="PAZ_DATA_NASCITA" DataField="PAZ_DATA_NASCITA" DataFormatString="{0:dd/MM/yyyy}" />
		    <on_dgr:OnitBoundColumn HeaderText="Comune nascita" Key="COMUNE_DI_NASCITA"  DataField="COMUNE_DI_NASCITA"/> 
		    <on_dgr:OnitBoundColumn HeaderText="Codice fiscale" Key="PAZ_CODICE_FISCALE" DataField="PAZ_CODICE_FISCALE"/>
		    <on_dgr:OnitBoundColumn HeaderText="Tessera" Key="PAZ_TESSERA" DataField="PAZ_TESSERA"/>
		    <on_dgr:OnitBoundColumn HeaderText="Comune res." Key="COMUNE_DI_RESIDENZA" DataField="COMUNE_DI_RESIDENZA"/>
		    <on_dgr:OnitBoundColumn HeaderText="Indirizzo res." Key="PAZ_INDIRIZZO_RESIDENZA"  DataField="PAZ_INDIRIZZO_RESIDENZA"/>
	    </Columns>
    </on_dgr:OnitGrid>
</dyp:DynamicPanel>
