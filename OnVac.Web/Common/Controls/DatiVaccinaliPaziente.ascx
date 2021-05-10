<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="DatiVaccinaliPaziente.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.DatiVaccinaliPaziente" %>

<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>
<%@ Register TagPrefix="cc1" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitTable" %>

<style type="text/css">
    .border {
        border: solid 1px navy;
        padding: 1px;
        width: 100%;
    }

    .bold {
        font-weight: bold;
    }

    .margin3 {
        margin-top: 3px;
        margin-bottom: 1px;
        margin-left: 1px;
        margin-right: 1px;
    }

    .margin7 {
        margin-top: 7px;
        margin-bottom: 1px;
        margin-left: 1px;
        margin-right: 1px;
    }

    tr.aliasItemBackground td, TD.aliasItemBackground TABLE, TD.aliasItemBackground {
        background-color: #FBE070; /* #FFC11F;    #9EEE9E; */
        font-family: Arial;
        font-size: 12px;
        border: 0px;
    }

    tr.legendaColoreAlias td, TD.legendaColoreAlias TABLE, TD.legendaColoreAlias {
        background-color: #FBE070; /* #FFC11F;     #9EEE9E; */
        border: solid 1px navy;
        padding: 2px;
    }

    .legendaDescrizioneAlias {
        font-family: Arial;
        font-size: 12px;
        font-style: italic;
    }

    .itemAlias {
        color: #E9AA10; /* #E56000;     #7FB780; */
        font-weight: bold;
    }

    .divCicliPaziente {
        background-color: #F5F5F5; /* whitesmoke */
        font-family: Arial;
        font-size: 12px;
    }
</style>

<cc1:OnitTable ID="OnitTable1" runat="server" Width="100%">
    <cc1:OnitSection id="OnitSectionDatiAnagrafici" runat="server" width="100%" TypeHeight="Content" Height="100px">
        <cc1:OnitCell id="OnitCellDatiAnagrafici" runat="server" width="100%" height="100%" TypeScroll="Hidden" BackColor="#E0E0E0">
            <div class="vac-sezione margin3">
                Dati Paziente
            </div>
            <table width="100%" cellpadding="2" cellspacing="0" border="0">
                <colgroup>
                    <col style="width:20%" />
                    <col style="width:30%" />
                    <col style="width:20%" />
                    <col style="width:30%" />
                </colgroup>    
                <tr>
                    <td class="label bold">Cognome</td>
                    <td><asp:Label ID="lblCognome" CssClass="textbox_stringa border" runat="server"></asp:Label></td>
                    <td class="label bold">Nome</td>
                    <td><asp:Label ID="lblNome" CssClass="textbox_stringa border" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td class="label bold">Data nascita</td>
                    <td><asp:Label ID="lblDataNascita" CssClass="textbox_stringa border" runat="server"></asp:Label></td>                
                    <td class="label bold">Comune nascita</td>
                    <td><asp:Label ID="lblComuneNascita" CssClass="textbox_stringa border" runat="server"></asp:Label></td>                
                </tr>
                <tr>
                    <td class="label bold">Codice Fiscale</td>
                    <td><asp:Label ID="lblCodiceFiscale" CssClass="textbox_stringa border" runat="server"></asp:Label></td>
                    <td class="label bold">Cittadinanza</td>
                    <td><asp:Label ID="lblCittadinanza" CssClass="textbox_stringa border" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td class="label bold">Sesso</td>
                    <td><asp:Label ID="lblSesso" CssClass="textbox_stringa border" runat="server"></asp:Label></td>
                    <td class="label bold">Stato Anagrafico</td>
                    <td><asp:Label ID="lblStatoAnagrafico" CssClass="textbox_stringa border" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td class="label bold">Residenza</td>
                    <td><asp:Label ID="lblComuneResidenza" CssClass="textbox_stringa border" runat="server"></asp:Label></td>
                    <td class="label bold">Domicilio</td>
                    <td><asp:Label ID="lblComuneDomicilio" CssClass="textbox_stringa border" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td class="label bold">Centro Vaccinale</td>
                    <td colspan="3"><asp:Label ID="lblConsultorio" CssClass="textbox_stringa border" runat="server"></asp:Label></td>
                </tr>
            </table>
        </cc1:OnitCell>
    </cc1:OnitSection>
    <cc1:OnitSection  id="OnitSectionLegendaDatiVaccinali" runat="server" width="100%" TypeHeight="Content" >
        <cc1:OnitCell id="OnitCellLegendaDatiVaccinali" runat="server" width="100%" height="100%" TypeScroll="Hidden">
            <table border="0" cellpadding="2" cellspacing="0" width="100%" style="border-top: solid 1px navy; border-bottom: solid 1px navy; background-color:#E0E0E0">
                <colgroup>
                    <col style="width:5%; text-align:center" />
                    <col style="width:95%;" />
                </colgroup>
                <tr>
                    <td>
                        <table border="0" cellpadding="0" cellspacing="5" width="100%" >
                            <tr style="text-align:center">
                                <td class="legendaColoreAlias">&nbsp;
                                </td>
                            </tr>
                        </table>  
                    </td>                          
                    <td class="legendaDescrizioneAlias"> Dati del master che verranno spostati sull'alias dopo l'esecuzione dell'unmerge</td>
                </tr>
            </table>
        </cc1:OnitCell>
    </cc1:OnitSection>
    <cc1:OnitSection id="OnitSectionDatiVaccinali" runat="server" width="100%" TypeHeight="Content">
        <cc1:OnitCell id="OnitCellDatiVaccinali" runat="server" width="100%" height="100%" TypeScroll="Hidden">
            <div style="height:395px; overflow:auto;">
            <div class="vac-sezione margin3">
                Vaccinazioni Eseguite
            </div>
            <asp:DataGrid id="dgrVacEseguite" runat="server" Width="100%" AutoGenerateColumns="False"
	            AllowCustomPaging="false" AllowPaging="false" PageSize="25" >
	            <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
	            <ItemStyle CssClass="item"></ItemStyle>
	            <PagerStyle CssClass="pager" Position="TopAndBottom" Mode="NumericPages"  />
	            <Columns>
	                <asp:BoundColumn DataField="ves_paz_codice_old" Visible="false"></asp:BoundColumn>
	                <asp:BoundColumn DataField="ves_data_effettuazione" HeaderText="Data" >
			            <HeaderStyle Width="15%"></HeaderStyle>
		            </asp:BoundColumn>
	                <asp:BoundColumn DataField="ves_ass_codice" HeaderText="Associazione" >
			            <HeaderStyle Width="15%"></HeaderStyle>
		            </asp:BoundColumn>
	                <asp:BoundColumn DataField="ves_ass_n_dose" HeaderText="Dose Ass." >
			            <HeaderStyle Width="10%"></HeaderStyle>
		            </asp:BoundColumn>
		            <asp:BoundColumn DataField="ves_vac_codice" HeaderText="Vaccinazione" >
			            <HeaderStyle Width="15%"></HeaderStyle>
		            </asp:BoundColumn>
	                <asp:BoundColumn DataField="ves_n_richiamo" HeaderText="Dose Vac." >
			            <HeaderStyle Width="10%"></HeaderStyle>
		            </asp:BoundColumn>
	                <asp:BoundColumn DataField="ves_noc_codice" HeaderText="Nome Commerciale" >
			            <HeaderStyle Width="20%"></HeaderStyle>
		            </asp:BoundColumn>
	                <asp:BoundColumn DataField="ves_lot_codice" HeaderText="Lotto" >
			            <HeaderStyle Width="15%"></HeaderStyle>
		            </asp:BoundColumn>
                </Columns>
                <HeaderStyle CssClass="header"></HeaderStyle>
	            <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
            </asp:DataGrid>
            <div class="vac-sezione margin7">
                Vaccinazioni Escluse
            </div>
            <asp:DataGrid id="dgrVacEscluse" runat="server" Width="100%" AutoGenerateColumns="False"
	            AllowCustomPaging="false" AllowPaging="false" PageSize="25" >
	            <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
	            <ItemStyle CssClass="item"></ItemStyle>
	            <PagerStyle CssClass="pager" Position="TopAndBottom" Mode="NumericPages"  />
	            <Columns>
	                <asp:BoundColumn DataField="CodicePazientePrecedente" Visible="false"></asp:BoundColumn>
		            <asp:TemplateColumn HeaderText="Visita">
		                <HeaderStyle Width="20%"></HeaderStyle>
		                <ItemTemplate>
		                    <asp:Label ID="lblDataVisita" runat="server" Text='<%# ApplyDateFormat(Eval("DataVisita"), True) %>'></asp:Label>
		                </ItemTemplate>
		            </asp:TemplateColumn>
	                <asp:BoundColumn DataField="CodiceVaccinazione" HeaderText="Vaccinazione" >
			            <HeaderStyle Width="20%"></HeaderStyle>
		            </asp:BoundColumn>
	                <asp:BoundColumn DataField="DescrizioneMotivoEsclusione" HeaderText="Motivo Esclusione" >
			            <HeaderStyle Width="40%"></HeaderStyle>
		            </asp:BoundColumn>
		            <asp:TemplateColumn HeaderText="Scadenza">
		                <HeaderStyle Width="20%"></HeaderStyle>
		                <ItemTemplate>
		                    <asp:Label ID="lblDataScadenza" runat="server" Text='<%# ApplyDateFormat(Eval("DataScadenza"), True) %>'></asp:Label>
		                </ItemTemplate>
		            </asp:TemplateColumn>
                </Columns>
                <HeaderStyle CssClass="header"></HeaderStyle>
	            <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
            </asp:DataGrid>
            <div class="vac-sezione margin7">
                Visite
            </div>
            <asp:DataGrid id="dgrVisite" runat="server" Width="100%" AutoGenerateColumns="False"
	            AllowCustomPaging="false" AllowPaging="false" PageSize="25" >
	            <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
	            <ItemStyle CssClass="item"></ItemStyle>
	            <PagerStyle CssClass="pager" Position="TopAndBottom" Mode="NumericPages"  />
	            <Columns>
	                <asp:BoundColumn DataField="CodicePazienteAlias" Visible="false"></asp:BoundColumn>
		            <asp:TemplateColumn HeaderText="Visita">
		                <HeaderStyle Width="15%"></HeaderStyle>
		                <ItemTemplate>
		                    <asp:Label ID="lblDataVisita" runat="server" Text='<%# ApplyDateFormat(Eval("DataVisita"), True) %>'></asp:Label>
		                </ItemTemplate>
		            </asp:TemplateColumn>
	                <asp:BoundColumn DataField="MotivoSospensioneDescrizione" HeaderText="Motivo Sospensione" >
			            <HeaderStyle Width="35%"></HeaderStyle>
		            </asp:BoundColumn>
		            <asp:TemplateColumn HeaderText="Fine Sospensione">
		                <HeaderStyle Width="15%"></HeaderStyle>
		                <ItemTemplate>
		                    <asp:Label ID="lblDataFineSospensione" runat="server" Text='<%# ApplyDateFormat(Eval("DataFineSospensione"), True) %>'></asp:Label>
		                </ItemTemplate>
		            </asp:TemplateColumn>
	                <asp:BoundColumn DataField="MalattiaDescrizione" HeaderText="Malattia" >
			            <HeaderStyle Width="25%"></HeaderStyle>
		            </asp:BoundColumn>
		            <asp:TemplateColumn HeaderText="Bilancio">
		                <HeaderStyle Width="10%" HorizontalAlign="Center"></HeaderStyle>
		                <ItemStyle HorizontalAlign="Center" />
		                <ItemTemplate>
		                    <asp:Label runat="server" ID="lblNumBilancio" Text='<%# ApplyNumberFormat(Eval("BilancioNumero")) %>'></asp:Label>
		                </ItemTemplate>
		            </asp:TemplateColumn>
                </Columns>
                <HeaderStyle CssClass="header"></HeaderStyle>
	            <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
            </asp:DataGrid>
            <div class="vac-sezione margin7">
                Convocazioni del paziente
            </div>
            <asp:DataGrid id="dgrConvocazioni" runat="server" Width="100%" AutoGenerateColumns="False"
	            AllowCustomPaging="false" AllowPaging="false" PageSize="25" >
	            <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
	            <ItemStyle CssClass="item"></ItemStyle>
	            <PagerStyle CssClass="pager" Position="TopAndBottom" Mode="NumericPages"  />
	            <Columns>
	                <asp:BoundColumn DataField="CodicePazienteAlias" Visible="false"></asp:BoundColumn>
		            <asp:TemplateColumn HeaderText="Convocazione">
		                <HeaderStyle Width="12%"></HeaderStyle>
		                <ItemTemplate>
		                    <asp:Label ID="lblDataCnv" runat="server" Text='<%# ApplyDateFormat(Eval("DataConvocazione"), True) %>'></asp:Label>
		                </ItemTemplate>
		            </asp:TemplateColumn>
	                <asp:BoundColumn DataField="DescrizioneConsultorio" HeaderText="Centro Vaccinale" >
			            <HeaderStyle Width="22%"></HeaderStyle>
		            </asp:BoundColumn>
		            <asp:TemplateColumn HeaderText="Appuntamento">
		                <HeaderStyle Width="16%"></HeaderStyle>
		                <ItemTemplate>
		                    <asp:Label ID="lblDataAppuntamento" runat="server" Text='<%# ApplyDateFormat(Eval("DataAppuntamento"), False) %>'></asp:Label>
		                </ItemTemplate>
		            </asp:TemplateColumn>
	                <asp:BoundColumn DataField="DescrizioneAmbulatorio" HeaderText="Ambulatorio" >
			            <HeaderStyle Width="15%"></HeaderStyle>
		            </asp:BoundColumn>
		            <asp:TemplateColumn HeaderText="Vaccinazioni">
		                <HeaderStyle Width="35%"></HeaderStyle>
		                <ItemTemplate>
		                    <asp:Label runat="server" ID="lblVaccinazioni" Text='<%# Eval("DescrizioneVaccinazione") %>'></asp:Label>
		                </ItemTemplate>
		            </asp:TemplateColumn>
                </Columns>
                <HeaderStyle CssClass="header"></HeaderStyle>
	            <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
            </asp:DataGrid>            
            <div class="vac-sezione margin7">
                Cicli del paziente
            </div>
            <div class="divCicliPaziente" style="padding-top:5px; padding-bottom:10px; padding-left:2px; vertical-align:middle;">
                <asp:Label runat="server" ID="lblCicliPaziente" Width="100%"></asp:Label>
            </div>
          </div>
        </cc1:OnitCell>
    </cc1:OnitSection>
</cc1:OnitTable>