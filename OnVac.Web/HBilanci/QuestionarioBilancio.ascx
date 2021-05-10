<%@ Control Language="vb" AutoEventWireup="false" Codebehind="QuestionarioBilancio.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.QuestionarioBilancio" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>

<asp:DataList id="dlsQuestionarioCompleto" runat="server" width="100%">
	<ItemTemplate>
		<table id="dlsQuestionarioCompleto_table" style="font-weight: bold; color: white; background-color: black" cellspacing="0" cellpadding="0" width="100%" border="0">
            <tr>
				<td class="header">
					<asp:Label id="lblSezione" runat="server" CssClass="label"></asp:Label>
					<asp:Label id="lblSezioneCodice" runat="server" style="display: none;"></asp:Label>
					<asp:Label id="lblSezioneN" runat="server" style="display: none;"></asp:Label>
				</td>
			</tr>
		</table>
		<asp:DataList id="dlsQuestionarioSezione" runat="server" Width="100%" CssClass="Datagrid" ShowHeader="True">
			<SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
			<HeaderStyle BackColor="#cecece" Height="18px"></HeaderStyle>
			<EditItemStyle CssClass="Edit"></EditItemStyle>
			<AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
			<ItemStyle CssClass="Item"></ItemStyle>
			<HeaderTemplate>
				<table id="dlsQuestionarioSezione_table_header" style="table-layout: fixed; width:100%">
                    <colgroup>
                        <col style="width: 5%" />
                        <col style="width: 60%" />
                        <col style="width: 35%" />
                    </colgroup>
					<tr>
						<td>Codice</td>
						<td>Osservazione</td>
						<td>Risposta</td>
					</tr>
				</table>
			</HeaderTemplate>
			<ItemTemplate>

                <asp:HiddenField id="lblOsservazioneDisabilitata" runat="server" Value="" />
                <asp:HiddenField id="lblOsservazioneN" runat="server" Value='<%# Container.DataItem("OSB_N_OSSERVAZIONE") %>' />

				<table id="Table6" style="table-layout: fixed; width:100%">
                    <colgroup>
                        <col style="width: 5%" />
                        <col style="width: 60%" />
                        <col style="width: 35%" />
                    </colgroup>
					<tr>
						<td>
							<asp:Label id="lblCodice" runat="server" Text='<%# Container.DataItem("OSS_CODICE") %>'>
							</asp:Label></td>
						<td>
							<asp:Label id="lblDescrizione" runat="server" Text='<%# Container.DataItem("OSS_DESCRIZIONE") %>'>
							</asp:Label></td>
						<td>
							<asp:DropDownList id="cmbRisposta" runat="server" Width="100%" DataTextField="RIS_DESCRIZIONE" DataValueField="RIS_CODICE" />
							<asp:TextBox id="txtCondizioni" runat="server" Width="100%" style="display:none" />
							<asp:TextBox id="txtControlloValore" runat="server" style="display:none" Text='<%# Container.DataItem("OSS_CODICE") %>' />
							<asp:TextBox id="txtRisposta" runat="server" Width="100%" MaxLength="1000" TextMode="MultiLine" Rows="2" style="overflow-y:auto" />
                            <telerik:RadComboBox ID="cmbRispostaMultipla" runat="server" CheckBoxes="true" Localization-ItemsCheckedString="Risposte multiple" 
                                            Localization-AllItemsCheckedString="Tutte" Width="100%" DataTextField="RIS_DESCRIZIONE" DataValueField="RIS_CODICE" 
                                             Visible="true" CheckedItemsTexts="DisplayAllInInput" /> 
						</td>
					</tr>
				</table>

                <asp:HiddenField id="hdIdOsservazione" runat="server" Value="" />
                <asp:HiddenField ID="hidObbligo" runat="server" Value=<%# Container.DataItem("OSB_OBBLIGATORIA") %> />
                
			</ItemTemplate>
		</asp:DataList>
	</ItemTemplate>
</asp:DataList>
