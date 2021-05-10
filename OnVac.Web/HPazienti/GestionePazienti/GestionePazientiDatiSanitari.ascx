<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="GestionePazientiDatiSanitari.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.GestionePazientiDatiSanitari" %>

<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="uc1" TagName="OnVacSceltaCicli" Src="OnVacSceltaCicli.ascx" %>

<%@ Import Namespace="Onit.OnAssistnet.OnVac" %>

<script type="text/javascript" >

function ClickNuovaDiagnosi() {
	document.getElementById('<%= btnChkNuovaDiagnosi.ClientID %>').click()
}

function AllineaModale() {

    var idModale = '<%= ClientIdModaleMalattia() %>';

    if (idModale == null || idModale == '') return true;

    if (!isValidFinestraModale(idModale, false)) { 
		alert("Il campo 'Malattia' non era aggiornato.\nRiprovare.");
		return false;
	}

    return true;
}


</script>
<asp:Button Style="display: none" ID="btnChkNuovaDiagnosi" runat="server"></asp:Button>
<table border="0" cellspacing="0" cellpadding="2" width="100%">
    <colgroup>
        <col width="15%" />
        <col width="85%" />
    </colgroup>
    <tr>
        <td class="label">
            <asp:Label ID="lblCicli" runat="server">Cicli</asp:Label>
        </td>
        <td>
            <asp:DataGrid ID="dgrCicli" runat="server" Width="100%" PageSize="1000" AutoGenerateColumns="False"
                CssClass="DataGrid">
                <SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
                <AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
                <ItemStyle CssClass="Item"></ItemStyle>
                <HeaderStyle CssClass="Header"></HeaderStyle>
                <Columns>
                    <asp:TemplateColumn>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="Delete">
                                <img runat="server" src='~/Images/elimina.gif' onclick="if (!confirm('Si desidera eliminare la riga?')) return false" alt="Elimina">
                            </asp:LinkButton>
                        </ItemTemplate>
                        <HeaderTemplate>
                            <img runat="server" src="~/Images/nuovo.gif" style="cursor: hand" onclick="__doPostBack('Nuovo','dgrCicli');" alt="Associa ciclo">
                        </HeaderTemplate>
                        <HeaderStyle Width="10px"></HeaderStyle>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Ciclo">
                        <HeaderStyle Width="95%"></HeaderStyle>
                        <HeaderTemplate>
                            &nbsp;Ciclo
                        </HeaderTemplate>
                        <ItemTemplate>
                            <on_ofm:OnitModalList ID="Finestramodale1" runat="server" Width="70%" BackColor="Transparent"
                                LabelWidth="-8px" PosizionamentoFacile="False" Obbligatorio="False" Tabella="T_ANA_CICLI"
                                CampoDescrizione="CIC_DESCRIZIONE Descrizione" CampoCodice="CIC_CODICE Codice"
                                RaiseChangeEvent="False" CodiceWidth="30%" UseCode="True" SetUpperCase="False"
                                Label="Ciclo" UseTableLayout="True" Enabled="False" Connection="" Descrizione='<%# DataBinder.Eval(Container.DataItem, "CIC_DESCRIZIONE") %>'
                                BorderWidth="0px" Codice='<%# DataBinder.Eval(Container.DataItem, "PAC_CIC_CODICE") %>'>
                            </on_ofm:OnitModalList>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <div style="position: relative">
                                <table id="Table8" cellspacing="0" cellpadding="0" width="100%" border="0">
                                    <tr>
                                        <td width="0px">
                                            <asp:ImageButton ID="Imagebutton3" runat="server" ImageUrl="~/Images/conferma.gif"
                                                CommandName="Conferma"></asp:ImageButton>
                                        </td>
                                        <td width="100%">
                                            <on_ofm:OnitModalList ID="txtCicliEdit" runat="server" Width="70%" LabelWidth="-8px"
                                                PosizionamentoFacile="False" Obbligatorio="False" Tabella="T_ANA_CICLI" CampoDescrizione="CIC_DESCRIZIONE Descrizione"
                                                CampoCodice="CIC_CODICE Codice" RaiseChangeEvent="False" CodiceWidth="30%" UseCode="True"
                                                SetUpperCase="True" Filtro="1=1 ORDER BY Descrizione" Label="Ciclo" UseTableLayout="True">
                                            </on_ofm:OnitModalList>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
                <PagerStyle CssClass="Pager" Mode="NumericPages"></PagerStyle>
            </asp:DataGrid>
        </td>
    </tr>
    <tr>
        <td class="label">
            <asp:Label ID="lblMantoux" runat="server">Mantoux</asp:Label>
        </td>
        <td>
            <asp:DataGrid ID="dgrMantoux" runat="server" Width="100%" PageSize="1000" AutoGenerateColumns="False"
                CssClass="DataGrid">
                <SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
                <AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
                <ItemStyle CssClass="Item"></ItemStyle>
                <HeaderStyle CssClass="Header"></HeaderStyle>
                <Columns>
                    <asp:TemplateColumn>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="Delete">
                                <img runat="server" language=javascript src='~/Images/elimina.gif' onclick="if (!confirm('Si desidera eliminare la riga?')) return false" alt="Elimina">
                            </asp:LinkButton>
                        </ItemTemplate>
                        <HeaderTemplate>
                            <img runat="server" src="~/Images/nuovo.gif" style="cursor: hand" onclick="__doPostBack('Nuovo','dgrMantoux')" alt="Inserisci Mantoux">
                        </HeaderTemplate>
                        <HeaderStyle Width="10px"></HeaderStyle>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="Edit">
                                <img runat="server" src='~/Images/modifica.gif' alt="Modifica">
                            </asp:LinkButton>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:LinkButton runat="server" CommandName="Update">
                                <img runat="server" src='~/Images/conferma.gif' alt="Conferma">
                            </asp:LinkButton>
                            <asp:LinkButton runat="server" CommandName="Cancel">
                                <img runat="server" src='~/Images/annulla.gif' alt="Annulla">
                            </asp:LinkButton>
                        </EditItemTemplate>
                        <HeaderStyle Width="10px"></HeaderStyle>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Data">
                        <HeaderStyle Width="15%"></HeaderStyle>
                        <HeaderTemplate>
                            &nbsp;Data
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="txtMantouxDataDis" runat="server" Width="100%" Text='<%# RitornaValore(0,"MAN_DATA",Container.DataItem) %>'>
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <div style="position: relative">
                                <on_val:OnitDatePick ID="txtMantouxData" runat="server" Height="20px" target="txtMantouxData"
                                    BorderColor="White" Text='<%# RitornaValore(0,"MAN_DATA",Container.DataItem) %>'
                                    CssClass="TextBox_Data_Obbligatorio" FormatoData="GeneralDate" Focus="False"
                                    NoCalendario="True" Formatta="False" Hidden="False" ControlloTemporale="False"
                                    DateBox="True" indice="-1" CalendarioPopUp="True"></on_val:OnitDatePick>
                            </div>
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Eseguita da">
                        <HeaderStyle Width="23%"></HeaderStyle>
                        <ItemTemplate>
                            <asp:Label ID="txtMantouxDescrizioneDis" runat="server" Width="100%" Text='<%# DataBinder.Eval(Container.DataItem, "MAN_DESCRIZIONE") %>'>
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtMantouxDescrizione" runat="server" Width="100%" Text='<%# DataBinder.Eval(Container.DataItem, "MAN_DESCRIZIONE") %>'>
                            </asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="mm">
                        <HeaderStyle Width="10%"></HeaderStyle>
                        <ItemTemplate>
                            <asp:Label ID="txtMantouxNumDis" runat="server" Width="100%" Text='<%# DataBinder.Eval(Container.DataItem, "MAN_MM") %>'>
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtMantouxNum" runat="server" Width="100%" MaxLength="5" Text='<%# DataBinder.Eval(Container.DataItem, "MAN_MM") %>'>
                            </asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Medico">
                        <HeaderStyle Width="15%"></HeaderStyle>
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Width="100%" Text='<%# DataBinder.Eval(Container.DataItem, "OPE_NOME") %>'>
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <on_ofm:OnitModalList ID="txtMantouxMedico" runat="server" Width="100%" LabelWidth="-1px"
                                PosizionamentoFacile="False" Obbligatorio="False" Tabella="T_ANA_OPERATORI" CampoDescrizione="OPE_NOME Nome"
                                CampoCodice="OPE_CODICE Codice" CodiceWidth="0px" UseCode="True" SetUpperCase="True"
                                Filtro="OPE_QUALIFICA in ('C','D') AND (OPE_OBSOLETO='N' OR OPE_OBSOLETO IS NULL) order by Nome"
                                Label="Titolo" UseTableLayout="True" Codice='<%# DataBinder.Eval(Container.DataItem, "MAN_OPE_CODICE") %>'
                                Descrizione='<%# DataBinder.Eval(Container.DataItem, "OPE_NOME") %>'></on_ofm:OnitModalList>
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Eseguita S&#236;/No">
                        <HeaderStyle Width="15%"></HeaderStyle>
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkSiNoDis" runat="server" Enabled="False" Checked='<%# BindBooleanValue(Eval("MAN_SINO")) %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox ID="chkSiNo" runat="server" Checked='<%# BindBooleanValue(Eval("MAN_SINO")) %>' />
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Positiva S&#236;/No">
                        <HeaderStyle Width="15%"></HeaderStyle>
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkPositivaSiNoDis" runat="server" Enabled="False" Checked='<%# BindBooleanValue(Eval("MAN_POSITIVA")) %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox ID="chkPositivaSiNo" runat="server" Checked='<%# BindBooleanValue(Eval("MAN_POSITIVA")) %>' />
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="<%$ Resources: Onit.OnAssistnet.OnVac.Web, DatiPaziente.MantouxDataInvio%>">
                        <HeaderStyle Width="12%"></HeaderStyle>
                        <ItemTemplate>
                            <asp:Label ID="txtMantouxDataInvioDis" runat="server" Width="100%" Text='<%# RitornaValore(0,"MAN_DATA_INVIO",Container.DataItem) %>'>
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <div style="position: relative">
                                <on_val:OnitDatePick ID="txtMantouxDataInvio" runat="server" Height="20px" target="txtMantouxDataInvio"
                                    CalendarioPopUp="True" indice="-1" DateBox="True" ControlloTemporale="False"
                                    Hidden="False" Formatta="False" NoCalendario="True" Focus="False" FormatoData="GeneralDate"
                                    Text='<%# RitornaValore(0,"MAN_DATA_INVIO",Container.DataItem) %>' BorderColor="White"
                                    CssClass="TextBox_Data"></on_val:OnitDatePick>
                            </div>
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
                <PagerStyle CssClass="Pager" Mode="NumericPages"></PagerStyle>
            </asp:DataGrid>
        </td>
    </tr>
    <tr>
        <td class="label">
            <asp:Label ID="lblMalattie" runat="server" Text="<%$ Resources: Onit.OnAssistnet.OnVac.Web, DatiPaziente.LabelMalattie %>" />
        </td>
        <td>
            <asp:DataGrid ID="dgrMalattie" runat="server" Width="100%" PageSize="1000" AutoGenerateColumns="False" CssClass="DataGrid">
                <SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
                <AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
                <ItemStyle CssClass="Item"></ItemStyle>
                <HeaderStyle CssClass="Header"></HeaderStyle>
                <Columns>
                    <asp:TemplateColumn>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="Delete">
                                <img runat="server" id="imgEliminaMalattia" src='~/Images/elimina.gif' onclick="if (!confirm('Eliminando la malattia verranno eliminati gli eventuali bilanci in programmazione.\nSi desidera procedere?')) return false" alt="Elimina">
                            </asp:LinkButton>
                        </ItemTemplate>
                        <HeaderTemplate>
                            <img runat="server" src="~/Images/nuovo.gif" style="cursor: pointer" onclick="__doPostBack('Nuovo','dgrMalattie')" alt="Inserisci malattia cronica">
                        </HeaderTemplate>
                        <HeaderStyle Width="10px"></HeaderStyle>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="Edit">
                                <img runat="server" id="imgModificaMalattia" src='~/Images/modifica.gif' alt="Modifica" >
                            </asp:LinkButton>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:LinkButton runat="server" CommandName="Update">
                                <img runat="server" src='~/Images/conferma.gif' onclick='if (!AllineaModale()) {return false}' alt="Conferma">
                            </asp:LinkButton>
                            <asp:LinkButton runat="server" CommandName="Cancel">
                                <img runat="server" src='~/Images/annulla.gif' alt="Annulla">
                            </asp:LinkButton>
                        </EditItemTemplate>
                        <HeaderStyle Width="10px"></HeaderStyle>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn>
                        <HeaderTemplate>
                            <asp:Label ID="lblHeaderMalattie" runat="server" Text= '<%$ Resources: Onit.OnAssistnet.OnVac.Web, DatiPaziente.HeaderMalattie %>' />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <on_ofm:OnitModalList ID="txtMalattieDis" runat="server" Width="80%" BackColor="Transparent"
                                LabelWidth="-8px" PosizionamentoFacile="False" Obbligatorio="False" Tabella="T_ANA_MALATTIE"
                                CampoDescrizione="MAL_DESCRIZIONE Descrizione" CampoCodice="MAL_CODICE Codice"
                                RaiseChangeEvent="False" CodiceWidth="20%" UseCode="True" SetUpperCase="False"
                                Label="Ciclo" Enabled="False" Connection="" UseTableLayout="True" Codice='<%# DataBinder.Eval(Container.DataItem, "PMA_MAL_CODICE") %>'
                                BorderWidth="0px" Descrizione='<%# DataBinder.Eval(Container.DataItem, "MAL_DESCRIZIONE") %>'>
                            </on_ofm:OnitModalList>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <div style="position: relative">
                                <on_ofm:OnitModalList ID="txtMalattia" runat="server" Width="70%" LabelWidth="-8px"
                                    PosizionamentoFacile="False" Obbligatorio="True" Tabella="V_ANA_MALATTIE" CampoDescrizione="MAL_DESCRIZIONE Descrizione"
                                    CampoCodice="MAL_CODICE Codice" RaiseChangeEvent="True" CodiceWidth="30%" UseCode="True" IsDistinct="true"
                                    SetUpperCase="True" Filtro='<%# RecuperaFiltroModaleMalattia %>' Label="Ciclo"
                                    Connection="" UseTableLayout="True" Codice='<%# DataBinder.Eval(Container.DataItem, "PMA_MAL_CODICE") %>'
                                    Descrizione='<%# DataBinder.Eval(Container.DataItem, "MAL_DESCRIZIONE") %>'>
                                </on_ofm:OnitModalList>
                            </div>
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn >
                        <HeaderStyle Width="5%"></HeaderStyle>
                        <HeaderTemplate>
                            <asp:Label ID="lblFollowUp" runat="server" Text="Follow up" ToolTip="Se selezionato verr&#224; gestito l'iter dei bilanci per quella malattia" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox runat="server" ID="chkFollowUpDis" Enabled="False" Checked='<%# BindBooleanValue(Eval("PMA_FOLLOW_UP")) %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox runat="server" ID="chkFollowUp" Enabled="True" Checked='<%# BindBooleanValue(Eval("PMA_FOLLOW_UP")) %>' />
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Nuova diagnosi">
                        <HeaderStyle Width="5%"></HeaderStyle>
                        <ItemTemplate>
                            <asp:CheckBox runat="server" ID="chkNuovaDiagnosiDis" Enabled="False" Checked='<%# BindBooleanValue(Eval("PMA_NUOVA_DIAGNOSI")) %>' /><br>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox runat="server" ID="chkNuovaDiagnosi" Enabled="True" onclick="ClickNuovaDiagnosi()"
                                AutoPostBack="False" Checked='<%# BindBooleanValue(Eval("PMA_NUOVA_DIAGNOSI")) %>' /><br>
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn >
                        <HeaderStyle Width="15%"></HeaderStyle>
                        <HeaderTemplate>
                            <asp:Label ID="lblDataDiagnosiDis" runat="server" Text="Data Diagnosi" ToolTip="Data in cui &#232; avvenuta la prima diagnosi" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <on_val:OnitDatePick ID="txtDataDiagnosiDis" runat="server" Height="20px" target="txtDataDiagnosi"
                                BorderColor="White" Enabled="False" Text='<%# RitornaValore(0,"PMA_DATA_DIAGNOSI",Container.DataItem) %>'
                                CssClass="TextBox_Data_Disabilitato" FormatoData="GeneralDate" Focus="False"
                                NoCalendario="False" Formatta="False" Hidden="False" ControlloTemporale="False"
                                DateBox="True" indice="-1" CalendarioPopUp="True"></on_val:OnitDatePick>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <on_val:OnitDatePick ID="txtDataDiagnosi" runat="server" Height="20px" target="txtDataDiagnosi"
                                BorderColor="White" Text='<%# RitornaValore(0,"PMA_DATA_DIAGNOSI",Container.DataItem) %>'
                                CssClass="TextBox_Data" FormatoData="GeneralDate" Focus="False" NoCalendario="False"
                                Formatta="False" Hidden="False" ControlloTemporale="False" DateBox="True" indice="-1"
                                CalendarioPopUp="True"></on_val:OnitDatePick>
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn>
                        <HeaderStyle Width="15%"></HeaderStyle>
                        <HeaderTemplate>
                            <asp:Label ID="lblDataUltimaVisitaDis" runat="server" Text="Data ultima visita" ToolTip="Data in cui &#232; avvenuta l'ultima visita in caso di vecchia diagnosi" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <on_val:OnitDatePick ID="txtDataUltimaVisitaDis" runat="server" Height="20px" target="txtDataUltimaVisita"
                                BorderColor="White" Enabled="False" Text='<%# RitornaValore(0,"PMA_DATA_ULTIMA_VISITA",Container.DataItem) %>'
                                CssClass="TextBox_Data_Disabilitato" FormatoData="GeneralDate" Focus="False"
                                NoCalendario="False" Formatta="False" Hidden="False" ControlloTemporale="False"
                                DateBox="True" indice="-1" CalendarioPopUp="True"></on_val:OnitDatePick>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <on_val:OnitDatePick ID="txtDataUltimaVisita" runat="server" Height="20px" target="txtDataUltimaVisita"
                                BorderColor="White" Text='<%# RitornaValore(0,"PMA_DATA_ULTIMA_VISITA",Container.DataItem) %>'
                                CssClass="TextBox_Data" FormatoData="GeneralDate" Focus="False" NoCalendario="False"
                                Formatta="False" Hidden="False" ControlloTemporale="False" DateBox="True" indice="-1"
                                CalendarioPopUp="True"></on_val:OnitDatePick>
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="N&#176; bilancio di partenza">
                        <HeaderStyle Width="8%"></HeaderStyle>
                        <ItemTemplate>
                            <asp:TextBox ID="cmbBilancioPartenzaDis" Width="100%" BorderColor="Transparent" Enabled="True"
                                CssClass="TextBox_Stringa_Disabilitato" ReadOnly="True" Text='<%# DataBinder.Eval(Container.DataItem, "PMA_N_BILANCIO_PARTENZA") %>'
                                runat="server">
                            </asp:TextBox><br />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="cmbBilancioPartenza" Width="100%" Style="position: center"
                                BorderColor="Transparent" Enabled="True" CssClass="TextBox_Stringa" ReadOnly="False"
                                runat="server" DataValueField="BIL_NUMERO" DataTextField="BIL_NUMERO">
                            </asp:DropDownList>
                            <br />
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn>
                        <HeaderStyle Width="2%"></HeaderStyle>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="imgFrecciaSu"></asp:LinkButton><br>
                            <asp:LinkButton runat="server" ID="imgFrecciaGiu"></asp:LinkButton>
                        </ItemTemplate>
                        <EditItemTemplate>
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn>
                        <HeaderStyle Width="5%"></HeaderStyle>
                        <HeaderTemplate>
                            <asp:Label ID="lblNMalattiaDis" runat="server" Text="Gravit&#224" ToolTip="&#200; necessario specificare un numero intero che indica la gravit&#224; associata alla malattia; la priorit&#224; della gravit&#224; &#232; segnalata dal numero pi&#249; basso." />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:TextBox ID="txtNMalattiaDis" runat="server" Width="100%" CssClass="Disabilitatxt"
                                Text='<%# DataBinder.Eval(Container.DataItem, "PMA_N_MALATTIA") %>' BackColor="Transparent"
                                BorderWidth="0px" ReadOnly="True">
                            </asp:TextBox>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtNMalattia" runat="server" Width="100%" CssClass="textboxDgr_noEdit"
                                Style="text-align: left" ReadOnly="True" Text='<%# DataBinder.Eval(Container.DataItem, "PMA_N_MALATTIA") %>'>
                            </asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
                <PagerStyle CssClass="Pager" Mode="NumericPages"></PagerStyle>
            </asp:DataGrid>
        </td>
    </tr>
    <tr height="10">
        <td colspan="2">
        </td>
    </tr>
</table>

<on_ofm:OnitFinestraModale ID="fmSceltaCicli" Title="Scelta dei cicli" runat="server"
     Height="248px" Width="400px" BackColor="LightGray" NoRenderX="True">
    <uc1:OnVacSceltaCicli ID="OnVacSceltaCicli" runat="server"></uc1:OnVacSceltaCicli>
</on_ofm:OnitFinestraModale>
