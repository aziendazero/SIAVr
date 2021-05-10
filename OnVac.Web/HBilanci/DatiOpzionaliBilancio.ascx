<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="DatiOpzionaliBilancio.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.DatiOpzionaliBilancio" %>

<%@ Register TagPrefix="dyp" Assembly="Onit.Web.UI.WebControls.DynamicPanel" Namespace="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>

<dyp:DynamicPanel ID="dypLabelVaccinazioni" runat="server" Width="100%" Height="22px" ScrollBars="None">
    <div class="Sezione"><asp:Label id="lblSezioneVaccinazioni" runat="server" Text="Vaccinazioni" ToolTip=""></asp:Label></div>
</dyp:DynamicPanel>

<dyp:DynamicPanel ID="dypVaccinazioni" runat="server" Width="100%" Height="100px" ScrollBars="Auto">
    <asp:DataList id="dlsVaccinazioni" runat="server" RepeatColumns="3" RepeatDirection="Horizontal" Width="100%">
        <ItemTemplate>
            <table style="table-layout:fixed; width: 100%" border="0" cellpadding="0" cellspacing="0">
                <colgroup>
                    <col style="width: 90%" />
                    <col style="width: 10%" />
                </colgroup>
                <tr>
                    <td>
                        <asp:CheckBox ID="chkVac" runat="server" Text='<%# Eval("Descrizione")%>' CssClass="label_left" />
                    </td>
                    <td>
                        <on_val:OnitJsValidator id="txtDose" runat="server" CssClass="textbox_numerico" Text='<%# Eval("Dose")%>' Width="25px"
						    actionCorrect="False" actionDelete="False" actionFocus="False" actionMessage="True" actionSelect="False"
						    actionUndo="False" autoFormat="False" validationType="Validate_integer" MaxLength="2"
                            PreParams-numDecDigit="0" PreParams-maxValue="99" PreParams-minValue="1"></on_val:OnitJsValidator>

                        <asp:TextBox ID="txtCodVac" runat="server" Text='<%# Eval("Codice")%>' style="display:none" Width="1px" ></asp:TextBox>
                    </td>
                </tr>
            </table>
        </ItemTemplate>
    </asp:DataList>
</dyp:DynamicPanel>

<dyp:DynamicPanel ID="dypViaggio" runat="server" Width="100%" Height="125px" ScrollBars="none">
    <div class="Sezione"><asp:Label id="lblSezioneViaggio" runat="server" Text="Dati Viaggio"></asp:Label></div>
	<table cellspacing="0" cellpadding="2" width="100%" border="0" style="table-layout:fixed;">
        <colgroup>
            <col style="width: 10%" />
            <col style="width: 18%" />
            <col style="width: 10%" />
            <col style="width: 18%" />
            <col  />
        </colgroup>
        <tr>
            <td class="label"><asp:Label ID="LabelDataInizioFollowUp" runat="server" Text="<%$ Resources: Onit.OnAssistnet.OnVac.Web, Anamnesi.DataFollowUpPrevista%>"></asp:Label></td>
            <td class="label_left">
                <on_val:onitdatepick id="dpkFollowUpPrevista" runat="server" Width="130px" CssClass="TextBox_Data" Height="22px" DateBox="True" BorderColor="White"></on_val:onitdatepick>
			<!--	<on_val:onitdatepick id="dpkInizioViaggio" runat="server" Width="130px" CssClass="TextBox_Data" Height="22px" DateBox="True" BorderColor="White"></on_val:onitdatepick></td>-->
            <td class="label"><asp:Label ID="LabelDataFineFollowUp" runat="server" Text="<%$ Resources: Onit.OnAssistnet.OnVac.Web, Anamnesi.DataFollowUpEffettiva%>"></asp:Label></td>
            <td class="label_left">
                <on_val:onitdatepick id="dpkFollowUpEffettiva" runat="server" Width="130px" CssClass="TextBox_Data" Height="22px" DateBox="True" BorderColor="White"></on_val:onitdatepick>
				<!--<on_val:onitdatepick id="dpkFineViaggio" runat="server" Width="130px" CssClass="TextBox_Data" Height="22px" DateBox="True" BorderColor="White"></on_val:onitdatepick></td>
            
            <td class="label" title="Giorni totali di permanenza all'estero (estremi inclusi)">Giorni</td>
            <td title="Giorni totali di permanenza all'estero (estremi inclusi)">                
                <input type="text" id="txtGiorniViaggio" runat="server" readonly="readonly" CssClass="TextBox_Data_Disabilitato" style="width: 100%; background-color:gainsboro" />
            </td>            
            <td class="label">Paese</td>
            <td>
                <on_ofm:onitmodallist id="fmPaeseViaggio" runat="server" Width="80%" Height="22px" CodiceWidth="19%"
                    CampoCodice="CIT_CODICE Codice" CampoDescrizione="CIT_STATO Stato" Tabella="T_ANA_CITTADINANZE" UseCode="True" SetUpperCase="True" 
                    AltriCampi="to_char(CIT_SCADENZA, 'dd/MM/yyyy') Scadenza" Filtro="1=1 ORDER BY Stato, Scadenza desc"
                    LabelWidth="-8px" PosizionamentoFacile="False" Label="" Obbligatorio="False"></on_ofm:onitmodallist></td>-->
            <td></td>
                    
        </tr>
    </table>
    <div style="overflow-y: scroll; height:75px;"> 
    <table cellspacing="0" cellpadding="2" width="100%" border="0" >
        <asp:Repeater ID="rptViaggi" runat="server">
            <HeaderTemplate>
                <table width="100%" cellspacing="0" cellpadding="0" border="0" >
                    <colgroup>
                        <col width="5px" />
                        <col width="15%" />
                        <col width="15%" />
                        <col width="5%" />
                        <col width="65%" />
                        <col width="150px" />
                    </colgroup>
                    <tr class="header">
                        <td><asp:ImageButton src="../../Images/nuovo.gif" runat="server" ID="InsertViaggi" OnCommand="btnInsertViaggio_Click" /></td>
                        <td class="label_left"><asp:Label ID="HeaderDataInizio" runat="server" Text="<%$ Resources: Onit.OnAssistnet.OnVac.Web, ViaggiVisita.HeaderDataInizio %>"></asp:Label></td>
                        <td class="label_left"><asp:Label ID="HeaderDataFine" runat="server" Text="<%$ Resources: Onit.OnAssistnet.OnVac.Web, ViaggiVisita.HeaderDataFine %>"></asp:Label></td>
                        <td class="label_left" title="Giorni totali di permanenza all'estero (estremi inclusi)"><asp:Label ID="HeaderGiorni" runat="server" Text="<%$ Resources: Onit.OnAssistnet.OnVac.Web, ViaggiVisita.HeaderGiorni %>"></asp:Label></td>
                        <td class="label_left"><asp:Label ID="HeaderPaese" runat="server" Text="<%$ Resources: Onit.OnAssistnet.OnVac.Web, ViaggiVisita.HeaderPaese %>"></asp:Label></td>
                        <td> </td>
                    </tr>
                </table>
            </HeaderTemplate>
            <ItemTemplate>
                <table width="100%"  >
                    <colgroup>
                        <col width="5px" />
                        <col width="15%" />
                        <col width="15%" />
                        <col width="5%" />
                        <col width="65%" />
                        <col width="150px" />
                    </colgroup>
                    <tr>
                        <td><asp:ImageButton src="../../Images/elimina.gif" runat="server" ID="DeleteViaggi" CommandName="ClickDelete" CommandArgument='<%# Eval("Id") %>'/></td>
                        <td class="label_left">
                            <on_val:OnitDatePick ID="dpkInizioViaggioRpt" runat="server"  Height="22px" DateBox="True" BorderColor="White" Data='<%# Eval("DataInizioViaggio") %>'></on_val:OnitDatePick>
                        </td>
                        <td class="label_left">
                            <on_val:OnitDatePick ID="dpkFineViaggioRpt" runat="server"  CssClass="TextBox_Data dtp" Height="22px" DateBox="True" BorderColor="White" Data='<%# Eval("DataFineViaggio") %>'></on_val:OnitDatePick>
                        </td>
                        <td title="Giorni totali di permanenza all'estero (estremi inclusi)">
                            <asp:TextBox id="txtGiorniViaggioRpt" runat="server" Enabled="False" cssclass="TextBox_Data_Disabilitato" style="width: 100%; background-color: gainsboro"/>
                        </td>
                        <td>
                            <on_ofm:OnitModalList ID="fmPaeseViaggioRpt" runat="server" Width="80%" Height="22px" CodiceWidth="19%"
                                CampoCodice="CIT_CODICE Codice" CampoDescrizione="CIT_STATO Stato" Tabella="T_ANA_CITTADINANZE" UseCode="True" SetUpperCase="True"
                                AltriCampi="to_char(CIT_SCADENZA, 'dd/MM/yyyy') Scadenza" Filtro="1=1 ORDER BY Stato, Scadenza desc" Codice='<%# Eval("CodicePaese") %>' Descrizione='<%# Eval("DescPaese") %>'
                                LabelWidth="-8px" PosizionamentoFacile="False" Label="" Obbligatorio="False"></on_ofm:OnitModalList>
                        </td>
                        <td><asp:Label runat="server" ID="idViaggioRpt" Text='<%# Eval("Id") %>' Visible="false"/> 
                            <asp:Label runat="server" ID="IdVisitaRpt" Text='<%# Eval("IdVisita") %>' Visible="False"/>
                            <asp:Label runat="server" ID="OperazioneRpt" Text='<%# Eval("Operazione") %>' Visible="False"/> </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:Repeater>
    </table>
        </div>

</dyp:DynamicPanel>

<script type="text/javascript">
    /*
        Calcolo giorni viaggio
    */

    function OnitDataPick_Blur(sender, e) {
        CalcolaGiorniViaggio(sender);
    }

    function OnitDataPick_ClickDay(sender) {
        CalcolaGiorniViaggio(sender);
    }

    function CalcolaGiorniViaggio(sender) {
        
        var idInizio = sender.replace('dpkFineViaggioRpt', 'dpkInizioViaggioRpt');
        var idFine = sender.replace('dpkInizioViaggioRpt', 'dpkFineViaggioRpt');
        var txtGiorni = $("#" + sender.replace('dpkInizioViaggioRpt', 'txtGiorniViaggioRpt').replace('dpkFineViaggioRpt', 'txtGiorniViaggioRpt'))[0];

        var startDate = OnitDataPickGet(idInizio);
        if (startDate == "") {
            txtGiorni.value = "";
            return;
        }

        var endDate = OnitDataPickGet(idFine);
        if (endDate == "") {
            txtGiorni.value = "";
            return;
        }

        var totGiorni = dateDiffDays(startDate, endDate, true);
        if (totGiorni == -1)
            txtGiorni.value = "";
        else
            txtGiorni.value = totGiorni;        

    }


</script>
