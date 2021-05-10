<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Control Language="vb" AutoEventWireup="false" Codebehind="Anagrafica.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.Common.Controls.Anagrafica" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>

<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>

<table  style="width: 100%; height: 22px; table-layout: fixed; " cellspacing="0" cellpadding="0"">
    <colgroup>
        <col width="95%" />
        <col width="5%" />
       
    </colgroup>
	<tr>
		<td runat="server"  id="tdAnagrafica" BgColor="White"  style="padding-left: 5px; vertical-align: central;border: 1px inset currentColor; border-image: none; " >
			
            <asp:Label id="lblAnagrafe" style="font-size: 12px; text-transform: uppercase; font-style: italic; font-family: Verdana"
				runat="server"  CssClass="TextBox_Stringa" ></asp:Label>     
		</td>
         <td  align="center">
             <table style="width: 100%; height: 22px;" cellspacing="0" cellpadding="0">
                 <tr>
                     <td>
                         <asp:Button ID="btnAggiungi2"  runat="server"  Text="..."  BorderStyle="NotSet" style="cursor:pointer" OnClientClick="clickmodale(event,this);"  />
                     </td>
                     
                     <td>
                         <asp:ImageButton id="btnCancalla" runat="server" title="Cancella" style="cursor: pointer; " ImageUrl="../../images/pulisci.gif" ToolTip="Cancella" />
                     </td>
                 </tr>

             </table>
		</td>
	</tr>
    <tr>
         <asp:Label id="lblCodice" style="font-size: 10px; text-transform: uppercase; font-style: italic; font-family: Verdana"
				runat="server" Width="10%" CssClass="TextBox_Stringa" Visible="false" ></asp:Label>
    </tr>
</table>
<script language="javascript" type='text/javascript'>
function SetFocus()
{
    // safety check, make sure its a post 1999 browser
    if (!document.getElementById)
    {
        return;
    }

    var txtMyInputBoxElement = document.getElementById(txtFiltro);
    alert(txtMyInputBoxElement);
    if (txtMyInputBoxElement != null)
    {
        alert(txtMyInputBoxElement);
        txtMyInputBoxElement.focus();
    }
}
SetFocus();
function clickmodale()
    {
    var filtro = document.getElementById(txtFiltro);
                alert(filtro);
                if (filtro != null) filtro.focus();
                evt.preventDefault();
                StopPreventDefault(evt);
                return false;
           
            }
</script>
<on_ofm:OnitFinestraModale id="fmAnagrafici" title="<div style=&quot;font-family:'Microsoft Sans Serif';text-align:center; font-size: 11pt;padding-bottom:2px&quot;>Anagrafica</div>"
	runat="server" width="600px" Height="400px" BackColor="LightGray" UseDefaultTab="True" RenderModalNotVisible="True" NoRenderX="True"  >
    
    <table style="border-width:0px; width:100%;"  >
        <colgroup>
            <col style="width:1%" />
            <col style="width:98%" />
            <col style="width:1%" />
        </colgroup>
        
        <tr align="center">
		                <td colspan="3">
			                
                                <table class="label_left" style="margin-top: 2px" cellspacing="0" cellpadding="2" width="100%" border="0">
					                <colgroup>
						                <col width="12%" />
						                <col  />
                                        </colgroup>
                                    <tr>
                                        <td class="Label">Filtro</td>
                                        <td>
					                        <asp:TextBox ID="txtFiltro" runat="server" Width="80%" class="toUpper" style="text-transform:uppercase"></asp:TextBox>
                                            <asp:LinkButton ID="cerca" runat="server" ToolTip="Filtra anagrafica" >
			                                        <img src="../../images/cerca.gif" alt="cerca i filtri" />
			                                    </asp:LinkButton>
                                        </td>
                                    </tr>
                                    </table>
                            </td>
            </tr>
       
        <tr style="height:200px">
            <td></td>
            <td>
                <div style="overflow-y:auto; width:100%;height:298px">
                <asp:DataGrid id="dgrRicerca1" runat="server" Width="100%" AutoGenerateColumns="False" AllowCustomPaging="False" AllowPaging="False"  Visible="true">
					<AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
					<ItemStyle CssClass="item"></ItemStyle>
					<SelectedItemStyle CssClass="selected"></SelectedItemStyle>
                    <HeaderStyle CssClass="header"></HeaderStyle>
					<Columns>
					    <on_dgr:SelectorColumn>
					        <ItemStyle HorizontalAlign="Center" Width="2%" />
					    </on_dgr:SelectorColumn>
					    <asp:BoundColumn Visible="true" DataField="Codice" HeaderText="Codice"></asp:BoundColumn>
					    <asp:BoundColumn Visible="true" DataField="Descrizione" HeaderText="Descrizione"></asp:BoundColumn>
                        <asp:BoundColumn Visible="false" DataField="CodiceEsterno" HeaderText="CodiceEsterno"></asp:BoundColumn>
                        <asp:BoundColumn Visible="false" DataField="Obsoleto" HeaderText="Obsoleto"></asp:BoundColumn>
                        </Columns>
        </asp:DataGrid>
                   </div>
            </td>
            <td></td>
        </tr>
    </table>
	<div style="text-align: center">
		<asp:Button id="btnConfermaAnagrafica" OnClick="btnConfermaAnagrafica_Click" Text="OK" runat="server" Width="100px"></asp:Button>
		<asp:Button id="btnAnnullaAnagrafica" OnClick="btnAnnullaAnagrafica_Click" Text="Annulla" runat="server" Width="100px"></asp:Button>
    </div>
</on_ofm:OnitFinestraModale>


