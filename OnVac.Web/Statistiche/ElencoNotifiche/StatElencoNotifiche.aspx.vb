Imports Onit.Database.DataAccessManager
Imports System.Collections.Generic

Partial Class StatElencoNotifiche
    Inherits OnVac.Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub
    Protected WithEvents lblConsultorio As System.Web.UI.WebControls.Label
    Protected WithEvents lblStatoAnagrafico As System.Web.UI.WebControls.Label
    Protected WithEvents Label2 As System.Web.UI.WebControls.Label
    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Page Events "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then
            PanelTitolo.InnerText = OnVacUtility.Variabili.CNS.Descrizione

            'impostazione dello stato anagrafico
            CaricaStatiAnagrafici()

			ucSelezioneConsultori.MostraSoloAperti = False
			ucSelezioneConsultori.ImpostaCnsCorrente = True
			ucSelezioneConsultori.LoadGetCodici()

			ShowPrintButtons()
        End If

        'per la stampa generata dal pulsante invio
        Select Case Request.Form("__EVENTTARGET")

            Case "Stampa"

                If Toolbar.Items.FromKeyButton("btnStampa").Visible Then
                    Select Case Request.Form("__EVENTARGUMENT")
                        Case "ImpostaStatoS"
                            Stampa(True)
                        Case "ImpostaStatoN"
                            Stampa(False)
                    End Select
                End If

        End Select

    End Sub

#End Region

#Region " Private Methods "

    Private Sub CaricaStatiAnagrafici()

        Dim dt As DataTable = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizStatiAnagrafici As New Biz.BizStatiAnagrafici(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                dt = bizStatiAnagrafici.LeggiStatiAnagrafici()

            End Using
        End Using

        Me.chklStatoAnagrafico.DataValueField = "SAN_CODICE"
        Me.chklStatoAnagrafico.DataTextField = "SAN_DESCRIZIONE"
        Me.chklStatoAnagrafico.DataSource = dt
        Me.chklStatoAnagrafico.DataBind()

    End Sub

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()
        listPrintButtons.Add(New PrintButton(Constants.ReportName.ElencoNotifiche, "btnStampa"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Toolbar)

    End Sub

    Private Sub Stampa(impostaCasoConcluso As Boolean)

        Dim control As Boolean = False

        Dim filtroReport As String = String.Empty
        Dim rpt As New ReportParameter()

		' Filtro sul consultorio
		Dim lista As List(Of String) = ucSelezioneConsultori.GetConsultoriSelezionati
		If lista.Count > 0 Then

			filtroReport = String.Format(" ({{T_PAZ_PAZIENTI.PAZ_CNS_CODICE}} in ['{0}'])", lista.Aggregate(Function(p, g) p & "', '" & g))
			control = True
			rpt.AddParameter("Consultorio", ucSelezioneConsultori.GetDescrizioneConsultoriSelezionati)



		Else
			Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
				Using bizCns As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)
					lista = bizCns.GetListCodiceConsultoriUtente(OnVacContext.UserId)
					If lista.Count > 0 Then
						filtroReport = String.Format(" ({{T_PAZ_PAZIENTI.PAZ_CNS_CODICE}} in ['{0}'])", lista.Aggregate(Function(p, g) p & "', '" & g))
					End If

				End Using
			End Using
			control = True

			rpt.AddParameter("Consultorio", "TUTTI")
        End If

        ' Filtro sullo stato anagrafico
        Dim listStatiAnagraficiSelezionati As New List(Of String)()

        If Not Me.chklStatoAnagrafico.SelectedItem Is Nothing Then

            If control Then filtroReport += " AND ("

            Dim stbFiltroStatiAnagrafici As New System.Text.StringBuilder()
            Dim stbDescrizioneStatiAnagraficiSelezionati As New System.Text.StringBuilder()

            For i As Integer = 0 To Me.chklStatoAnagrafico.Items.Count - 1
                If Me.chklStatoAnagrafico.Items(i).Selected Then
                    listStatiAnagraficiSelezionati.Add(chklStatoAnagrafico.Items(i).Value)
                    stbFiltroStatiAnagrafici.AppendFormat(" ({{T_PAZ_PAZIENTI.PAZ_STATO_ANAGRAFICO}} = '{0}') OR", Me.chklStatoAnagrafico.Items(i).Value)
                    stbDescrizioneStatiAnagraficiSelezionati.AppendFormat("{0} ", Me.chklStatoAnagrafico.Items(i).Text)
                End If
            Next

            rpt.AddParameter("StatoAnagrafico", stbDescrizioneStatiAnagraficiSelezionati.ToString())

            If stbFiltroStatiAnagrafici.Length > 0 Then
                stbFiltroStatiAnagrafici.Remove(stbFiltroStatiAnagrafici.Length - 3, 3)
            End If

            ' Aggiunta del filtro sullo stato anagrafico al filtro totale
            filtroReport += stbFiltroStatiAnagrafici.ToString()

            If control Then filtroReport += ")"
            control = True

        Else
            rpt.AddParameter("StatoAnagrafico", "TUTTI")
        End If

        ' filtro sullo status vaccinale. N.B. : la ddlStatusVaccinale è nascosta -> il filtro non viene mai applicato!
        If Me.ddlStatusVaccinale.SelectedItem.Value <> String.Empty Then
            If control Then filtroReport += " AND"
            filtroReport += String.Format(" {{T_PAZ_PAZIENTI.PAZ_STATO}} = '{0}'", Me.ddlStatusVaccinale.SelectedItem.Value)
            control = True
            rpt.AddParameter("StatusVaccinale", Me.ddlStatusVaccinale.SelectedItem.Text)
        Else
            rpt.AddParameter("StatusVaccinale", "TUTTI")
        End If

        'filtro sullo stato dell'inadempienza (la stampa deve considerare solo le inadempienze in stato COMUNICAZIONE AL SINDACO)
        If control Then filtroReport += " AND"
        Dim comunicazioneSindaco As Int16 = Enumerators.StatoInadempienza.ComunicazioneAlSindaco
        filtroReport += String.Format(" {{T_PAZ_INADEMPIENZE.PIN_STATO}} = '{0}'", comunicazioneSindaco.ToString())

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Try
                genericProvider.BeginTransaction()

                Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                    If Not OnVacReport.StampaReport(Constants.ReportName.ElencoNotifiche, filtroReport, Nothing, Nothing, Nothing, bizReport.GetReportFolder(Constants.ReportName.ElencoNotifiche)) Then
                        OnVacUtility.StampaNonPresente(Page, Constants.ReportName.ElencoNotifiche)
                    Else
                        ' Aggiorna la data di stampa della CS, il campo pin_stampato = 'S' e l'utente che l'ha stampato
                        ' Se l'utente lo ha confermato, imposta anche lo stato dei pazienti stampati a CASO CONCLUSO
                        For Each sCodice As String In lista
                            genericProvider.Paziente.UpdateInadempienze(sCodice, listStatiAnagraficiSelezionati, OnVacContext.UserId, impostaCasoConcluso)
                        Next

                    End If

                End Using

                genericProvider.Commit()

            Catch ex As Exception

                genericProvider.Rollback()

                ex.InternalPreserveStackTrace()
                Throw

            End Try

        End Using

    End Sub

#End Region

#Region " Public Methods "

    ' Restituisce la descrizione dell'inadempienza in base al codice specificato
    Public Function RecuperaStato(codStato As Enumerators.StatoInadempienza) As String

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizCodifiche As New Biz.BizCodifiche(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Return bizCodifiche.GetDescrizioneCodifica("PIN_STATO", codStato)

            End Using

        End Using

    End Function

#End Region

End Class
