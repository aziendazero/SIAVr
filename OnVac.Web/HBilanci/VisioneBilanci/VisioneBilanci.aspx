<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="VisioneBilanci.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.VisioneBilanci" %>

<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_ocb" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitCombo" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<%@ Register TagPrefix="uc2" TagName="QuestionarioBilancio" Src="../QuestionarioBilancio.ascx" %>
<%@ Register TagPrefix="uc1" TagName="FirmaDigitale" Src="../../Common/Controls/FirmaDigitaleArchiviazioneSostitutiva.ascx" %>
<%@ Register TagPrefix="uc3" TagName="FirmaDigitaleInfo" Src="../../Common/Controls/FirmaDigitaleInfo.ascx" %>
<%@ Register TagPrefix="uc4" TagName="DatiOpzionaliBilancio" Src="../DatiOpzionaliBilancio.ascx" %>

<%@ Import Namespace="Onit.OnAssistnet.OnVac" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Visione Anamnesi</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <!-- #include file="../../Common/Scripts/AggiornaStatoVaccinazioni.inc" -->

    <script type="text/javascript" src='<%= OnVacUtility.GetScriptUrl("onvac.common.js")%>'></script>
    <script type="text/javascript" language="javascript">
	
        function InizializzaToolBar(t)
        {
            t.PostBackButton=false;
        }
		
        function ToolBarClick(ToolBar,button,evnt)
        {
            evnt.needPostBack=true;
		
            switch(button.Key)
            {
                case 'btnAvvisoBilancio':
						
                    if (<%= dlsBilanci.Items.Count %>==0) 
                    {
                        alert("Il paziente non ha alcun bilancio associato: impossibile effettuare la stampa!");
                        evnt.needPostBack=false;
                    }
                    else
                    {
                        if (<%= dlsBilanci.SelectedIndex%><0)
                        {
                            alert("Selezionare un bilancio per effettuare la stampa!");
                            evnt.needPostBack=false;
                        }
                    }
                    break;
					
                case 'btnStampaBilancio':
						
                    if (<%= dlsBilanci.Items.Count %>==0) 
                    {
                        alert("Il paziente non ha alcun bilancio associato: impossibile effettuare la stampa!");
                        evnt.needPostBack=false;
                    }
                    else
                    {
                        if (<%= dlsBilanci.SelectedIndex%><0)
                        {
                            alert("Selezionare un bilancio per effettuare la stampa!");
                            evnt.needPostBack=false;
                        }
                    }
                    break;
                case 'btnFollowUp':
						
                    if (<%= dlsBilanci.Items.Count %>==0) 
                        {
                            alert("Il paziente non ha alcun bilancio associato: impossibile creare follow up!");
                            evnt.needPostBack=false;
                        }
                        else
                        {
                            if (<%= dlsBilanci.SelectedIndex%><0)
                            {
                                alert("Selezionare un bilancio per creare un follow up!");
                                evnt.needPostBack=false;
                            }
                        }
                        break;
					
                    case 'btnElimina':

                        var msg = "Si desidera eliminare l'anamnesi selezionata?";

                        if ('<%= GetFlagFirmaDigitaleVisitaSelezionata().ToString().ToLower()%>' == 'true')
                    {
                        msg = "L'anamnesi selezionata è stata firmata digitalmente. Eliminarla comunque?";
                    }
						
                    if (!confirm(msg))
                        evnt.needPostBack = false;
                    break;	
                        
                case 'btnModificaConsenso':

                    if (!confirm("Attenzione: si è sicuri di voler concedere/revocare il consenso per l'elemento selezionato?"))
                        evnt.needPostBack=false;
                    break;
            }
        }
		
        //controlla che il valore sia numerico [modifica 05/04/2005]
        function ControllaValore(oggetto, tipo, interi, decimali)
        {
            var valore = oggetto.value;
            if (valore != '')
            {
                valore = valore.replace(',','.');
				
                if (isNaN(valore))
                {
                    AssegnaValoreFormattato(oggetto,valore);
                    alert('Attenzione: il valore inserito non è numerico!');
                    oggetto.focus();
                    return;
                }
				
                valore = valore.replace('.',',');
                if (valore.search(',') != -1)
                {
                    AssegnaValoreFormattato(oggetto,valore);
                    //controllo che il formato interi/decimali venga rispettato per il campo [modifica 13/07/2005]
                    if ((interi != null) && (decimali != null))
                    {
                        var valoreIntero = valore.split(',')[0];
                        var valoreDecimale = valore.split(',')[1];
                        if ((valoreIntero.length > interi) || (valoreDecimale.length > decimali))
                        {
                            MessaggioErrore(interi,decimali);
                            oggetto.focus();
                        }
                    }
                    //Maurizio 23-05-05
                    return;
                }
                else
                {
                    if (valore.length > interi)
                    {
                        MessaggioErrore(interi,decimali);
                        oggetto.focus();
                    }	
                }
            }
        }
			
        //genera il messaggio con il formato del numero relativo [modifica 13/07/2005]
        function MessaggioErrore(interi, decimali)
        {
            var formatoIntero = '';
            var formatoDecimale = '';
            for (i=0;i<interi;i++)
            {
                formatoIntero += '0';
            }
            for (i=0;i<decimali;i++)
            {
                formatoDecimale += '0';
            }
            alert('Attenzione: il formato del numero deve essere [' + formatoIntero + ',' + formatoDecimale + '].');
        }
		
        //assegna il valore corretto al campo selezionato [modifica 05/04/2005]
        function AssegnaValoreFormattato(oggetto, valore)
        {
            valore = valore.replace(',','.');
            oggetto.value = valore;
        }
    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
        <on_lay3:OnitLayout3 ID="OnitLayout31" Titolo="Visione Anamnesi" runat="server" Width="100%" Height="100%">

            <asp:MultiView ID="multiViewMain" runat="server" ActiveViewIndex="0">

                <asp:View ID="viewDati" runat="server">

                    <div style="width: 100%" id="divLayoutTitolo" class="Title">
                        <asp:Label ID="LayoutTitolo" runat="server" BorderStyle="None" Width="100%" CssClass="title"></asp:Label>
                    </div>

                    <div>
                        <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBar" runat="server" ItemWidthDefault="75px" CssClass="infratoolbar">
                            <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                            <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                            <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                            <Items>
                                <igtbar:TBarButton Key="btnIndietro" Text="Indietro" DisabledImage="~/Images/indietro_dis.gif"
                                    Image="~/Images/indietro.gif" ToolTip="Ritorna alla vaccinazione programmata">
                                </igtbar:TBarButton>
                                <igtbar:TBarButton Key="btnModifica" Text="Modifica" DisabledImage="~/Images/modifica_dis.gif"
                                    Image="~/Images/modifica.gif">
                                </igtbar:TBarButton>
                                <igtbar:TBarButton Key="btnSalva" Text="Salva" DisabledImage="~/Images/salva_dis.gif"
                                    Image="~/Images/salva.gif">
                                </igtbar:TBarButton>
                                <igtbar:TBarButton Key="btnAnnulla" Text="Annulla" DisabledImage="../../images/annulla_dis.gif"
                                    Image="../../images/annulla.gif">
                                </igtbar:TBarButton>
                                <igtbar:TBarButton Key="btnElimina" Text="Elimina" DisabledImage="~/Images/AnnullaConf_dis.gif"
                                    Image="~/Images/AnnullaConf.gif">
                                </igtbar:TBarButton>
                                <igtbar:TBSeparator></igtbar:TBSeparator>
                                <igtbar:TBarButton Key="btnModificaConsenso" Text="Concedi/Revoca Consenso" DisabledImage="../../images/modificaConsenso_dis.png"
                                    Image="../../images/modificaConsenso.png" ToolTip="Imposta il consenso alla comunicazione dei dati a CONCESSO/NEGATO per l'elemento selezionato">
                                    <DefaultStyle Width="180px" CssClass="infratoolbar_button_default"></DefaultStyle>
                                </igtbar:TBarButton>
                                <igtbar:TBarButton Key="btnStampa" Text="Stampa" DisabledImage="~/Images/stampa_dis.gif"
                                    Image="~/Images/stampa.gif" ToolTip="Stampa l'anamnesi selezionata">
                                </igtbar:TBarButton>
                                <igtbar:TBSeparator Key="sepFirma"></igtbar:TBSeparator>
                                <igtbar:TBarButton Key="btnFirma" Text="Firma digitale" DisabledImage="../../images/firmaDigitale_dis.png"
                                    Image="../../images/firmaDigitale.png" ToolTip="Visualizza l'anteprima del documento da firmare digitalmente">
                                    <DefaultStyle Width="100px" CssClass="infratoolbar_button_default"></DefaultStyle>
                                </igtbar:TBarButton>
                                <igtbar:TBarButton Key="btnFollowUp" Text="Follow Up" DisabledImage="../../images/document-edit-dis.png"
                                    Image="../../images/document-edit.png" ToolTip="Crea un Follow up">
                                    <DefaultStyle Width="100px" CssClass="infratoolbar_button_default"></DefaultStyle>
                                </igtbar:TBarButton>
                                <igtbar:TBSeparator></igtbar:TBSeparator>
                                <igtbar:TBarButton Key="btnRecuperaStoricoVacc" Text="Recupera" DisabledImage="../../images/recupera_dis.png"
                                    Image="../../images/recupera.png" ToolTip="Recupera lo storico vaccinale centralizzato del paziente">
                                    <DefaultStyle Width="90px" CssClass="infratoolbar_button_default"></DefaultStyle>
                                </igtbar:TBarButton>
                            </Items>
                        </igtbar:UltraWebToolbar>
                        <!--<igtbar:TBSeparator></igtbar:TBSeparator>
							    <igtbar:TBarButton Key="btnStampaBilancio" DisabledImage="~/Images/stampa.gif" Text="Stampa Bilancio"
								    Image="~/Images/stampa.gif">
								    <DefaultStyle Width="120px" CssClass="infratoolbar_button_default"></DefaultStyle>
							    </igtbar:TBarButton>
							    <igtbar:TBarButton Key="btnAvvisoBilancio" DisabledImage="~/Images/stampa.gif" Text="Avviso Bilancio"
								    Image="~/Images/stampa.gif">
								    <DefaultStyle Width="120px" CssClass="infratoolbar_button_default"></DefaultStyle>
							    </igtbar:TBarButton>-->
                    </div>
                    <div class="sezione">
                        <asp:Label ID="LayoutTitolo_sezione" runat="server" Text="BILANCI DISPONIBILI PER IL PAZIENTE"></asp:Label>
                    </div>

                    <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="180px" ScrollBars="Auto" RememberScrollPosition="true">

                        <asp:DataList ID="dlsBilanci" runat="server" Width="100%" CssClass="DataGrid">
                            <SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
                            <HeaderStyle CssClass="Header"></HeaderStyle>
                            <EditItemStyle CssClass="Edit"></EditItemStyle>
                            <AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
                            <FooterStyle CssClass="Footer"></FooterStyle>
                            <ItemStyle CssClass="Item"></ItemStyle>
                            <HeaderTemplate>
                                <table style="table-layout: fixed" width="100%">
                                    <tr>
                                        <td width="2%"></td>
                                        <td width="2%" title="Numero bilancio">N.</td>
                                        <td style="display: none">Cod. Mal.</td>
                                        <td width="13%">Descrizione</td>
                                        <td width="13%">Malattia</td>
                                        <td width="11%">Età</td>
                                        <td width="11%">Data Visita</td>
                                        <td width="12%">Medico</td>
                                        <td width="9%">Firmato da</td>
                                        <td width="12%">Operatore</td>
                                        <td width="10%" id="tdHeaderUslInserimento" runat="server">
                                            <asp:Label ID="lblHeaderUslInserimento" runat="server" Text="<%$ Resources: Onit.OnAssistnet.OnVac.Web, DatiVaccinali.AslInserimentoVacc %>"></asp:Label>
                                        </td>
                                        <td style="display: none">Cod. Usl Ins.</td>
                                        <td width="3%" id="tdHeaderFlagVisibilita" runat="server" title="Consenso alla comunicazione dei dati vaccinali da parte del paziente"></td>
                                        <td style="display: none">Valore flag visibilita</td>
                                        <td width="2%" id="tdHeaderFlagFirma" runat="server" title="Firma digitale e archiviazione sostitutiva documento"></td>
                                    </tr>
                                </table>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <table id="Table5" style="table-layout: fixed" width="100%">
                                    <tr>
                                        <td width="2%" valign="middle">
                                            <asp:ImageButton ID="imgSeleziona" runat="server" CommandName="Seleziona" ImageUrl='<%# Me.ResolveClientUrl("~/images/sel_app.png")%>'></asp:ImageButton>
                                            <asp:HiddenField ID="txtIdVisita" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem")("vis_id") %>' />
                                            <asp:HiddenField ID="hdCodicePaziente" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem")("vis_paz_codice") %>' />
                                            <asp:HiddenField ID="HiddenIdFollowUp" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem")("VIS_VIS_ID_FOLLOW_UP") %>' />
                                        </td>
                                        <td align="center" width="2%">
                                            <asp:Label ID="lblNumero" runat="server" Width="100%" Text='<%# Container.DataItem("VIS_N_BILANCIO") %>'>
                                            </asp:Label></td>
                                        <td style="display: none">
                                            <asp:Label ID="lblMalattia" runat="server" Width="100%" Text='<%# Container.DataItem("VIS_MAL_CODICE") %>'>
                                            </asp:Label></td>
                                        <td width="13%">
                                            <asp:Label ID="lblBilDescrizione" runat="server" Width="100%" Text='<%# Container.DataItem("BIL_DESCRIZIONE") %>'>
                                            </asp:Label></td>
                                        <td width="13%">
                                            <asp:Label ID="lblDescrizioneMalattia" runat="server" Width="100%" Text='<%# Container.DataItem("MAL_DESCRIZIONE") %>'>
                                            </asp:Label>
                                            <asp:HiddenField ID="HiddenSoloFollowUp" runat="server" Value='<%# Container.DataItem("MAL_SOLO_FOLLOW_UP")%>' />
                                            <asp:HiddenField ID="HiddenMalCodFollowUp" runat="server" Value='<%# Container.DataItem("MAL_MAL_CODICE_FOLLOW_UP")%>' />
                                        </td>
                                        <td width="11%">
                                            <asp:Label ID="lblEta" runat="server" Width="100%" Text='<%# GetStringEta(Container.DataItem, "BIL_ETA_MINIMA") %>'>
                                            </asp:Label></td>
                                        <td width="11%">
                                            <on_val:OnitDatePick ID="dtpDataBilancio" runat="server" Width="100%" Text='<%# Container.DataItem("VIS_DATA_VISITA") %>' ControlloTemporale="False" Focus="False" Hidden="False" indice="-1" CalendarioPopUp="True" Formatta="False" FormatoData="GeneralDate" NoCalendario="False" DateBox="True"></on_val:OnitDatePick>
                                        </td>
                                        <td width="12%">
                                            <on_ofm:OnitModalList ID="omlMedicoBilancio" runat="server" Width="100%" SetUpperCase="True"
                                                Obbligatorio="True" UseCode="True" Tabella="T_ANA_OPERATORI,T_ANA_LINK_OPER_CONSULTORI" CampoDescrizione="OPE_NOME" CampoCodice="OPE_CODICE"
                                                CodiceWidth="0px" LabelWidth="-1px" PosizionamentoFacile="False" Codice='<%# DataBinder.Eval(Container, "DataItem")("VIS_OPE_CODICE")%>'
                                                Descrizione='<%# DataBinder.Eval(Container, "DataItem")("ope_nome") %>'></on_ofm:OnitModalList>
                                        </td>
                                        <td width="10%">
                                            <asp:TextBox ID="txtFirmaBil" runat="server" Width="100%" CssClass="TextBox_Stringa" Text='<%# Container.DataItem("VIS_FIRMA") %>'></asp:TextBox>
                                        </td>
                                        <td width="11%">
                                            <on_ofm:OnitModalList ID="omlRilevatore" runat="server" Width="100%" SetUpperCase="True"
                                                Obbligatorio="True" UseCode="True" Tabella="T_ANA_OPERATORI,T_ANA_LINK_OPER_CONSULTORI" CampoDescrizione="OPE_NOME" CampoCodice="OPE_CODICE"
                                                CodiceWidth="0px" LabelWidth="-1px" PosizionamentoFacile="False" Codice='<%# DataBinder.Eval(Container, "DataItem")("VIS_OPE_CODICE_RILEVATORE")%>'
                                                Descrizione='<%# DataBinder.Eval(Container, "DataItem")("DESCRIZIONE_RILEVATORE")%>'></on_ofm:OnitModalList>
                                        </td>
                                        <td width="10%" id="tdItemUslInserimento" runat="server">
                                            <asp:Label ID="lblDescrizioneUslInserimento" runat="server" Width="100%" Text='<%# Container.DataItem("USL_INSERIMENTO_VIS_DESCR") %>'></asp:Label>
                                        </td>
                                        <td style="display: none">
                                            <asp:Label ID="lblCodiceUslInserimento" runat="server" Width="100%" Text='<%# Container.DataItem("VIS_USL_INSERIMENTO") %>'></asp:Label>
                                        </td>
                                        <td width="3%" id="tdItemFlagVisibilita" runat="server" align="center">
                                            <asp:Image ID="imgFlagVisibilita" runat="server"
                                                ImageUrl="<%# BindFlagVisibilitaImageUrlValue(Container.DataItem) %>"
                                                ToolTip="<%# BindFlagVisibilitaToolTipValue(Container.DataItem) %>" />
                                            <asp:HiddenField ID="HiddenField_FLAGVISIBILITA" runat="server" Value='<%# Container.DataItem("VIS_FLAG_VISIBILITA") %>'></asp:HiddenField>
                                        </td>
                                        <td style="display: none">
                                            <asp:Label ID="lblFlagVisibilita" runat="server" Text='<%# Container.DataItem("VIS_FLAG_VISIBILITA") %>'></asp:Label>
                                        </td>
                                        <td width="2%" id="tdItemFlagFirma" runat="server" align="center">
                                            <asp:ImageButton ID="btnFlagFirma" runat="server" Visible="false" CommandName="InfoArchiviazione"
                                                ImageUrl="<%# BindFlagFirmaImageUrlValue(Container.DataItem)%>"
                                                ToolTip="<%# BindFlagFirmaToolTipValue(Container.DataItem)%>" />
                                            <asp:HiddenField ID="hidUteFirma" runat="server" Value='<%# Container.DataItem("VIS_UTE_ID_FIRMA")%>' />
                                            <asp:HiddenField ID="hidUteArchiviazione" runat="server" Value='<%# Container.DataItem("VIS_UTE_ID_ARCHIVIAZIONE")%>' />
                                            <asp:HiddenField ID="hidIdDocumento" runat="server" Value='<%# Container.DataItem("VIS_DOC_ID_DOCUMENTO")%>' />
                                        </td>
                                        <td width="3%" id="tdItemNote" runat="server" align="center">
                                            <asp:ImageButton ID="imgNote" runat="server" CommandName="InfoNote"
                                                ImageUrl="<%# BindNoteImageUrlValue(Container.DataItem) %>"
                                                ToolTip="<%# BindNoteToolTipValue(Container.DataItem) %>" Visible="<%# BindNoteVisible(Container.DataItem) %>" />
                                        </td>
                                        <td width="2%" id="tdFollowUp" runat="server" align="center">
                                            <asp:ImageButton ID="btnFollowupRow" runat="server" CommandName="GoFollowUp"
                                                ImageUrl="<%# BindFlagFollowUpImageUrlValue(Container.DataItem)%>"
                                                ToolTip="<%# BindFlagFollowUPToolTipValue(Container.DataItem)%>" Visible="<%# BindFollowUpVisible(Container.DataItem) %>" />
                                            <asp:HiddenField ID="hidFollowup" runat="server" Value='<%# Container.DataItem("VIS_VIS_ID_FOLLOW_UP")%>' />
                                            <asp:HiddenField ID="hidFollowUpDataPrevisto" runat="server" Value='<%# Container.DataItem("VIS_DATA_FOLLOW_UP_PREVISTO")%>' />
                                            <asp:HiddenField ID="hidFollowUPDataEffettivo" runat="server" Value='<%# Container.DataItem("VIS_DATA_FOLLOW_UP_EFFETTIVO")%>' />
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </asp:DataList>
                    </dyp:DynamicPanel>

                    <uc4:DatiOpzionaliBilancio ID="ucDatiOpzionaliBilancio" runat="server" Visible="true" UseUpperCaseCaption="true" width="100%"></uc4:DatiOpzionaliBilancio>

                    <div class="Sezione">QUESTIONARIO COMPILATO</div>

                    <div class="label" style="text-align: left; padding: 5px;">
                        <asp:Label ID="Lbl_StatoDetail" runat="server" Text="Non disponibile" Visible="false"></asp:Label>
                    </div>

                    <dyp:DynamicPanel ID="DynamicPanel2" runat="server" Width="100%" Height="100%" ScrollBars="Auto">
                        <div id="divPercentili" runat="server">
                            <table id="table_questionario_compilato" border="0" cellspacing="3" cellpadding="3" width="100%">
                                <colgroup>
                                    <col width="17%" align="right" />
                                    <col width="17%" align="left" />
                                    <col width="17%" align="right" />
                                    <col width="16%" align="left" />
                                    <col width="17%" align="right" />
                                    <col width="16%" align="left" />
                                </colgroup>
                                <tr>
                                    <td class="label">
                                        <asp:Label ID="lblPeso" runat="server" Width="100%" CssClass="label_right">Peso (kg)</asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPeso" runat="server" Width="60px" CssClass="TextBox_Numerico"></asp:TextBox>
                                    </td>
                                    <td class="label">
                                        <asp:Label ID="lblAltezza" runat="server" Width="100%" CssClass="label_right">Altezza (cm)</asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtAltezza" runat="server" Width="60px" CssClass="TextBox_Numerico"></asp:TextBox>
                                    </td>
                                    <td class="label">
                                        <asp:Label ID="lblCranio" runat="server" Width="100%" CssClass="label_right">Circonferenza cranica (cm)</asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCranio" runat="server" Width="60px" CssClass="TextBox_Numerico"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label">
                                        <asp:Label ID="lblPercentilePeso" runat="server" Width="100%" CssClass="label_right">Percentile</asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPercentilePeso" runat="server" Width="60px" CssClass="TextBox_Numerico_Disabilitato" ReadOnly="True"></asp:TextBox>
                                    </td>
                                    <td class="label">
                                        <asp:Label ID="lblPercentileAltezza" runat="server" Width="100%" CssClass="label_right">Percentile</asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPercentileAltezza" runat="server" Width="60px" CssClass="TextBox_Numerico_Disabilitato" ReadOnly="True"></asp:TextBox>
                                    </td>
                                    <td class="label">
                                        <asp:Label ID="lblPercentileCranio" runat="server" Width="100%" CssClass="label_right">Percentile</asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPercentileCranio" runat="server" Width="60px" CssClass="TextBox_Numerico_Disabilitato" ReadOnly="True"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div>
                            <uc2:QuestionarioBilancio ID="Questionario" runat="server" Enabled="false" Visible="true" />
                        </div>
                    </dyp:DynamicPanel>

                </asp:View>

                <asp:View ID="viewFirma" runat="server">
                    <dyp:DynamicPanel ID="dypFirma" runat="server" Width="100%" Height="100%" ExpandDirection="vertical" ScrollBars="None">
                        <div style="width: 100%" id="divLayoutTitoloViewFirma" class="Title">
                            <asp:Label ID="LayoutTitoloViewFirma" runat="server" BorderStyle="None" Width="100%" CssClass="title"></asp:Label>
                        </div>
                        <uc1:FirmaDigitale ID="ucFirma" runat="server" />
                    </dyp:DynamicPanel>
                </asp:View>

            </asp:MultiView>

            <on_ofm:OnitFinestraModale ID="fmInfoArchiviazione" Title="Info Archiviazione" runat="server" Width="500px" Height="160px" BackColor="WhiteSmoke">
                <uc3:FirmaDigitaleInfo ID="ucInfoFirma" runat="server" />
            </on_ofm:OnitFinestraModale>

            <on_ofm:OnitFinestraModale ID="fmInfoNote" Title="Note Visita" runat="server" Width="500px" Height="160px" BackColor="WhiteSmoke">
                <div style="margin: 5px;">
                    <asp:TextBox ID="tbNote" runat="server" Text="Note" Width="490px" Height="120px" TextMode="MultiLine" CssClass="TextBox_Stringa" ReadOnly="true"></asp:TextBox>
                </div>
            </on_ofm:OnitFinestraModale>
        </on_lay3:OnitLayout3>
    </form>

    <script type="text/javascript" language="javascript">
		
        if (<%= Me.txtPeso.Visible.ToString().ToLower() %>) 
            document.getElementById("txtPeso").onblur = function() {ControllaValore(this, 'peso', 3, 2);};
        if (<%= Me.txtAltezza.Visible.ToString().ToLower() %>) 
            document.getElementById("txtAltezza").onblur = function() {ControllaValore(this, 'altezza', 3, 1);};
        if (<%= Me.txtCranio.Visible.ToString().ToLower() %>) 
            document.getElementById("txtCranio").onblur = function() {ControllaValore(this, 'cranio', 2, 1);};
		
    </script>

    <script type="text/javascript">
        controllaStato();
    </script>

</body>
</html>
