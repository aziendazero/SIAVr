Imports Onit.OnAssistnet.OnVac.Filters

Partial Class CambiaConsultorio
    Inherits OnVac.Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(sender As System.Object, e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Properties "

    Private Property OldConsultorio() As String
        Get
            Return ViewState("OnVac_CambiaCns_oldCns")
        End Get
        Set(Value As String)
            ViewState("OnVac_CambiaCns_oldCns") = Value
        End Set
    End Property

    Private Property FiltriMaschera() As FiltriGestioneAppuntamenti
        Get
            Return Session("FiltriGestioneAppuntamenti")
        End Get
        Set(Value As FiltriGestioneAppuntamenti)
            Session("FiltriGestioneAppuntamenti") = Value
        End Set
    End Property

#End Region

#Region " Page Events "

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            txtConsultorio.Codice = OnVacUtility.Variabili.CNS.Codice
            txtConsultorio.Filtro = GetModalListFilter()
            txtConsultorio.RefreshDataBind()

            'slokkiamo il paziente lokkato...
            OnitLayout.lock.EndLock(OnVacUtility.Variabili.PazId)

            OldConsultorio = txtConsultorio.Codice

            EnableToolbarButtons(False)

            lblRisultato.Text = "Selezionare il nuovo centro vaccinale"

        End If

    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btnConferma"

                OnVacUtility.CaricaVariabiliConsultorio(txtConsultorio.Codice, True)

                ' Cancello il flag per la conservazione dei filtri di gestione appuntamenti 
                If Not FiltriMaschera Is Nothing Then
                    FiltriMaschera.SetFiltriMaschera = False
                End If

                lblRisultato.Text = "Cambio di centro vaccinale avvenuto con successo"

                OnitLayout.Busy = False
                EnableToolbarButtons(False)

                OldConsultorio = txtConsultorio.Codice

            Case "btnAnnulla"

                txtConsultorio.Codice = OldConsultorio
                txtConsultorio.RefreshDataBind()

                lblRisultato.Text = "Centro vaccinale ripristinato"

                EnableToolbarButtons(False)
                OnitLayout.Busy = False

        End Select

    End Sub

#End Region

#Region " OnitModalList Events "

    Private Sub txtConsultorio_Change(Sender As Object, e As Onit.Controls.OnitModalList.ModalListaEventArgument) Handles txtConsultorio.Change

        lblRisultato.Text = String.Empty

        If OldConsultorio <> txtConsultorio.Codice Then
            OnitLayout.Busy = True
            EnableToolbarButtons(True)
            lblRisultato.Text = "Confermare per aggiornare i dati del programma"
        Else
            lblRisultato.Text = String.Empty
            OnitLayout.Busy = False
            EnableToolbarButtons(False)
            lblRisultato.Text = "Il centro vaccinale coincide con quello attualmente in uso!!"
        End If

    End Sub

#End Region

#Region " Private "

    Private Sub EnableToolbarButtons(enable As Boolean)
        ToolBar.Items.FromKeyButton("btnConferma").Enabled = enable
        ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = enable
    End Sub

    Private Function GetModalListFilter() As String

        Dim filter As New Text.StringBuilder("cns_data_apertura <= SYSDATE and (cns_data_chiusura > SYSDATE or cns_data_chiusura is null) ")

        If Settings.USER_SOLO_CNS_ABILITATI Then
            filter.AppendFormat("and exists (select 1 from t_ana_link_utenti_consultori t where(cns_codice = t.luc_cns_codice) and t.luc_ute_id = {0}) ", OnVacContext.UserId)
        End If

        filter.Append("order by cns_descrizione")

        Return filter.ToString()

    End Function

#End Region

End Class
