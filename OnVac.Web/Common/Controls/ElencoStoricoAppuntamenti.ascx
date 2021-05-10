<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ElencoStoricoAppuntamenti.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.ElencoStoricoAppuntamenti" %>

<%@ Register TagPrefix="dyp" Assembly="Onit.Web.UI.WebControls.DynamicPanel" Namespace="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>

<script type="text/javascript">
    function ImpostaImmagineOrdinamento(imgId, imgUrl) {
        var img = document.getElementById(imgId);
        if (img != null) {
            img.style.display = 'inline';
            img.src = imgUrl;
        }
    }
</script>

<style type="text/css">
    .message {
        width: 100%;
        text-align: center;
        font-weight: bold;
        margin-top: 20px;
    }

    .info-storico {
        font-family: Calibri;
        font-size: 14px;
    }

        .info-storico div {
            background-color: steelblue;
            color: white;
            margin: 12px 1px 5px 1px;
            padding: 2px 0px 2px 3px;
            border: 1px solid navy;
            font-weight: bold;
        }

        .info-storico span {
            color: navy;
            padding-left: 5px;
        }

        .info-storico textarea {
            color: navy;
            padding-left: 5px;
            border-width: 0px;
            background-color: transparent; 
            font-family: Calibri;
            font-size: 14px;
            overflow-y: auto;
        }
</style>

    <dyp:DynamicPanel ID="dypStoricoAppuntamenti" runat="server" Width="100%" Height="100%" ScrollBars="Auto">
        
        <asp:Label ID="lblMessage" runat="server" CssClass="TextBox_Stringa message" Visible="false" ></asp:Label>

        <asp:DataGrid ID="dgrStoricoAppuntamenti" runat="server" Width="100%" AutoGenerateColumns="False"
            AllowCustomPaging="true" AllowPaging="true" PageSize="25" AllowSorting="True">
            <AlternatingItemStyle CssClass="alternating" Font-Size="10px"></AlternatingItemStyle>
            <ItemStyle CssClass="item" Font-Size="10px"></ItemStyle>
            <PagerStyle CssClass="pager" Position="TopAndBottom" Mode="NumericPages" />
            <HeaderStyle CssClass="header" Font-Size="10px"></HeaderStyle>
            <SelectedItemStyle CssClass="selected" Font-Size="10px"></SelectedItemStyle>
            <Columns>
                <asp:BoundColumn DataField="Id" HeaderText="Id <img id='imgId' alt='' src='../../images/transparent16.gif' />"
                    SortExpression="Id" Visible="false">
                    <HeaderStyle Width="2%" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundColumn>
                <asp:TemplateColumn HeaderText="Appuntamento <img id='imgApp' alt='' src='../../images/transparent16.gif' />"
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="10%" SortExpression="DataAppuntamento" ItemStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                        <asp:Label ID="lblDataAppuntamento" CssClass="label_left" runat="server"
                            Text='<%# ConvertToDateString(DataBinder.Eval(Container.DataItem, "DataAppuntamento"), True)%>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:BoundColumn DataField="Vaccinazioni" HeaderText="Vaccinazioni <img id='imgVac' alt='' src='../../images/transparent16.gif' />"
                    SortExpression="Vaccinazioni">
                    <HeaderStyle Width="9%" HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundColumn>
                <asp:TemplateColumn HeaderText="Centro - Ambulatorio <img id='imgAmb' alt='' src='../../images/transparent16.gif' />"
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="16%" SortExpression="Ambulatorio" ItemStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                        <asp:Label ID="lblAmbulatorio" CssClass="label_left" runat="server"
                            Text='<%# GetCodiceDescConsultorio(DataBinder.Eval(Container.DataItem, "CodiceConsultorio"), DataBinder.Eval(Container.DataItem, "DescrizioneAmbulatorio"))%>' />
                    </ItemTemplate>
                </asp:TemplateColumn>                      
                <asp:TemplateColumn HeaderText="Eliminato il <img id='imgDel' alt='' src='../../images/transparent16.gif' />"
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="8%" SortExpression="DataEliminazioneAppuntamento" ItemStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                        <asp:Label ID="lblDataEliminazioneAppuntamento" CssClass="label_left" runat="server"
                            Text='<%# ConvertToDateString(DataBinder.Eval(Container.DataItem, "DataEliminazioneAppuntamento"), True)%>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:BoundColumn DataField="DescrizioneMotivoEliminazioneAppuntamento" HeaderText="Motivo <img id='imgMot' alt='' src='../../images/transparent16.gif' />"
                    SortExpression="MotivoEliminazioneAppuntamento">
                    <HeaderStyle Width="6%" HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundColumn>
                <asp:TemplateColumn HeaderText="Note <img id='imgNotAvv' alt='' src='../../images/transparent16.gif' />"
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="15%" SortExpression="NoteAvvisi" ItemStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                        <asp:Label ID="lblNoteAvvisiAppuntamento" CssClass="label_left" runat="server"
                            Text='<%# GetNoteAvvisiAppuntamento(DataBinder.Eval(Container.DataItem, "NoteAvvisi"), DataBinder.Eval(Container.DataItem, "NoteModificaAppuntamento"))%>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Cnv <img id='imgCnv' alt='' src='../../images/transparent16.gif' />"
                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="6%" SortExpression="DataConvocazione" ItemStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                        <asp:Label ID="lblDataConvocazione" CssClass="label_left" runat="server"
                            Text='<%# ConvertToDateString(DataBinder.Eval(Container.DataItem, "DataConvocazione"), False)%>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Info" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="2%" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:ImageButton ID="btnInfo" runat="server" ImageUrl="~/images/info.png" CommandName="Info" />
                            <%--ToolTip='<%# DataBinder.Eval(Container.DataItem, "Note")%>' --%>
                    </ItemTemplate>
                </asp:TemplateColumn>

                <asp:BoundColumn DataField="CodiceUtenteRegistrazioneAppuntamento" Visible="false"></asp:BoundColumn>
                <asp:BoundColumn DataField="DataRegistrazioneAppuntamento" Visible="false"></asp:BoundColumn>
                <asp:BoundColumn DataField="DataInvio" Visible="false"></asp:BoundColumn>
                <asp:BoundColumn DataField="CodiceUtenteEliminazioneAppuntamento" Visible="false"></asp:BoundColumn>
                <asp:BoundColumn DataField="Note" Visible="false"></asp:BoundColumn>               

            </Columns>
        </asp:DataGrid>

    </dyp:DynamicPanel>

    <on_ofm:OnitFinestraModale ID="fmInfo" Title="&nbspAltre informazioni" runat="server" NoRenderX="False" Width="580px" Height="400px" BackColor="GhostWhite" RenderModalNotVisible="True" >
        <div class="info-storico">
            <div>Data Prenotazione</div>
            <asp:Label ID="lblDataRegistrazioneAppuntamentoInfo" runat="server"></asp:Label>
            <div>Utente Prenotazione</div>
            <asp:Label ID="lblUtenteRegistrazioneAppuntamentoInfo" runat="server"></asp:Label>
            <div>Data Stampa</div>
            <asp:Label ID="lblDataInvioAppuntamentoInfo" runat="server"></asp:Label>
            <div>Utente Eliminazione Appuntamento</div>
            <asp:Label ID="lblUtenteEliminazioneAppuntamentoInfo" runat="server"></asp:Label>
            <div>Note</div>
            <asp:TextBox id="txtNoteInfo" runat="server" ReadOnly="true" Width="100%" TextMode="MultiLine" Rows="5"></asp:TextBox>
        </div>
    </on_ofm:OnitFinestraModale>


