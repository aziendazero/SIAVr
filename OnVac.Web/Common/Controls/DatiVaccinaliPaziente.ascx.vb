Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.Common.Utility



Partial Public Class DatiVaccinaliPaziente
    Inherits System.Web.UI.UserControl

#Region " Events "

    Public Event ErrorMessage(ByVal errorMessage As String)

    Public Sub OnErrorMessage(ByVal errorMessage As String)
        RaiseEvent ErrorMessage(errorMessage)
    End Sub

#End Region

#Region " Enums "

    Private Enum ColumnIndexDatiVaccinali
        paz_codice_old = 0
        ves_data_effettuazione = 1
        ves_ass_codice = 2
        ves_ass_n_dose = 3
        ves_vac_codice = 4
        ves_n_richiamo = 5
        ves_noc_codice = 6
        ves_lot_codice = 7
    End Enum

#End Region

#Region " Properties "

    Private _codicePazienteMaster As Integer
    Public ReadOnly Property CodicePazienteMaster() As Integer
        Get
            Return _codicePazienteMaster
        End Get
    End Property

    Private _codicePazienteAlias As Integer
    Public ReadOnly Property CodicePazienteAlias() As Integer
        Get
            Return _codicePazienteAlias
        End Get
    End Property

    Private _hideDatiVaccinali As Boolean
    Public Property HideDatiVaccinali() As Boolean
        Get
            Return _hideDatiVaccinali
        End Get
        Set(ByVal value As Boolean)
            _hideDatiVaccinali = value
        End Set
    End Property


#End Region

#Region " User Control Events "


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

#End Region

#Region " Public Methods "

    Public Sub CaricaDati(pazCodiceMaster As Integer, pazCodiceAlias As Integer)

        Me._codicePazienteMaster = pazCodiceMaster
        Me._codicePazienteAlias = pazCodiceAlias

        OnitSectionDatiVaccinali.Visible = Not HideDatiVaccinali
        OnitSectionLegendaDatiVaccinali.Visible = Not HideDatiVaccinali

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            ' Dati Paziente
            Dim collPaz As Collection.PazienteCollection = genericProvider.Paziente.GetPazienti(pazCodiceMaster, OnVacContext.CodiceUslCorrente)
            If collPaz Is Nothing OrElse collPaz.Count = 0 Then
                OnErrorMessage("Paziente non presente in locale")
                Return
            End If

            Me.setDatiPaziente(collPaz(0), genericProvider)

            ' Vaccinazioni Eseguite
            dgrVacEseguite.DataSource = genericProvider.VaccinazioniEseguite.GetVaccinazioniEseguitePaziente(pazCodiceMaster)
            dgrVacEseguite.DataBind()

            ' Vaccinazioni Escluse
            dgrVacEscluse.DataSource = genericProvider.VaccinazioniEscluse.GetVaccinazioniEsclusePaziente(pazCodiceMaster)
            dgrVacEscluse.DataBind()

            ' Visite del paziente
            dgrVisite.DataSource = genericProvider.Visite.GetVisitePaziente(pazCodiceMaster)
            dgrVisite.DataBind()

            ' Convocazioni del paziente
            'dgrConvocazioni.DataSource = genericProvider.Convocazione.GetConvocazioniVaccinazioniPaziente(pazCodiceMaster)
            'dgrConvocazioni.DataBind()
            Me.BindConvocazioniPaziente(pazCodiceMaster, genericProvider)

            ' Cicli del paziente
            Me.BindCicliPaziente(pazCodiceMaster, genericProvider)


        End Using

    End Sub

    Public Sub Clear()

        Me.setDatiPaziente(Nothing, Nothing)

        dgrVacEseguite.DataSource = Nothing
        dgrVacEseguite.DataBind()

        dgrVacEscluse.DataSource = Nothing
        dgrVacEscluse.DataBind()

        dgrVisite.DataSource = Nothing
        dgrVisite.DataBind()

        dgrConvocazioni.DataSource = Nothing
        dgrConvocazioni.DataBind()

        lblCicliPaziente.Text = String.Empty

    End Sub

#End Region

#Region " Private Methods "

    Private Sub setDatiPaziente(ByVal paziente As Entities.Paziente, ByVal genericProvider As DAL.DbGenericProvider)

        If Not paziente Is Nothing Then

            lblCognome.Text = paziente.PAZ_COGNOME
            lblNome.Text = paziente.PAZ_NOME

            lblDataNascita.Text = paziente.Data_Nascita.ToString("dd/MM/yyyy")
            lblComuneNascita.Text = Me.SetComunePaziente(paziente.ComuneNascita_Codice, String.Empty, genericProvider)

            lblSesso.Text = paziente.Sesso
            lblStatoAnagrafico.Text = paziente.StatoAnagrafico.ToString()

            If (String.IsNullOrEmpty(paziente.Paz_Cns_Codice)) Then
                lblConsultorio.Text = String.Empty
            Else
                lblConsultorio.Text = String.Format("{0} - {1}", paziente.Paz_Cns_Codice, _
                                                    genericProvider.Consultori.GetCnsDescrizione(paziente.Paz_Cns_Codice))
            End If

            lblComuneResidenza.Text = Me.SetComunePaziente(paziente.ComuneResidenza_Codice, paziente.ComuneResidenza_Descrizione, genericProvider)
            lblComuneDomicilio.Text = Me.SetComunePaziente(paziente.ComuneDomicilio_Codice, paziente.ComuneDomicilio_Descrizione, genericProvider)

            lblCodiceFiscale.Text = paziente.PAZ_CODICE_FISCALE
            lblCittadinanza.Text = paziente.Cittadinanza_Descrizione

        Else
            lblCognome.Text = String.Empty
            lblNome.Text = String.Empty

            lblDataNascita.Text = String.Empty
            lblComuneNascita.Text = String.Empty

            lblSesso.Text = String.Empty
            lblStatoAnagrafico.Text = String.Empty

            lblConsultorio.Text = String.Empty

            lblComuneResidenza.Text = String.Empty
            lblComuneDomicilio.Text = String.Empty

        End If

    End Sub

    Private Function SetComunePaziente(ByVal codiceComune As String, ByVal descrizioneComune As String, ByVal genericProvider As DAL.DbGenericProvider) As String

        If Not String.IsNullOrEmpty(descrizioneComune) Then
            Return descrizioneComune
        End If

        If Not String.IsNullOrEmpty(codiceComune) Then
            Dim comune As Entities.Comune = genericProvider.Comuni.GetComuneByCodice(codiceComune)

            If Not comune Is Nothing Then
                Return comune.Descrizione
            End If
        End If

        Return String.Empty

    End Function

    ' Bind delle convocazioni nel datagrid, con le vaccinazioni e le dosi in un'unica stringa separata dalle virgole. 
    ' Le vaccinazioni con paz_codice_old uguale al codice dell'alias vengono colorati diversamente.
    Private Sub BindConvocazioniPaziente(ByVal pazCodiceMaster As Integer, ByVal genericProvider As DAL.DbGenericProvider)

        Dim listCnvVacc As List(Of Entities.ConvocazioneVaccinazione)
        listCnvVacc = genericProvider.Convocazione.GetConvocazioniVaccinazioniPaziente(pazCodiceMaster)

        Dim listGroupCnvVacc As New List(Of Entities.ConvocazioneVaccinazione)

        Dim element As Entities.ConvocazioneVaccinazione

        For i As Integer = 0 To listCnvVacc.Count - 1

            If ListItem(listCnvVacc(i), listGroupCnvVacc).Count = 0 Then

                element = listCnvVacc(i).Clone()

                element.DescrizioneVaccinazione = Me.SetDescrizioneVaccinazioni(listCnvVacc(i), listCnvVacc)

                listGroupCnvVacc.Add(element)

            End If

        Next

        dgrConvocazioni.DataSource = listGroupCnvVacc
        dgrConvocazioni.DataBind()

    End Sub

    ' Restituisce una lista con i soli oggetti che hanno data cnv, cns, amb e data app in comune con l'elemento specificato.
    Private Function ListItem(ByVal element As Entities.ConvocazioneVaccinazione, ByVal list As List(Of Entities.ConvocazioneVaccinazione)) As List(Of Entities.ConvocazioneVaccinazione)

        Dim listElements As List(Of Entities.ConvocazioneVaccinazione) = _
            (From item As Entities.ConvocazioneVaccinazione In list _
             Where item.DataConvocazione = element.DataConvocazione _
             And item.CodiceConsultorio = element.CodiceConsultorio _
             And item.CodiceAmbulatorio = element.CodiceAmbulatorio _
             And ((Not item.DataAppuntamento.HasValue And Not element.DataAppuntamento.HasValue) _
                  OrElse item.DataAppuntamento = element.DataAppuntamento) _
             Select item).ToList()

        If listElements Is Nothing Then
            listElements = New List(Of Entities.ConvocazioneVaccinazione)()
        End If

        Return listElements

    End Function

    ' Restituisce una stringa con i codici delle vaccinazioni e i rispettivi richiami, 
    ' impostando un colore diverso per le vaccinazioni appartenenti all'alias.
    Private Function SetDescrizioneVaccinazioni(ByVal element As Entities.ConvocazioneVaccinazione, ByVal list As List(Of Entities.ConvocazioneVaccinazione)) As String

        Dim listElements As List(Of Entities.ConvocazioneVaccinazione) = ListItem(element, list)

        If listElements.Count = 0 Then
            Return String.Empty
        End If

        Dim stbElencoVaccinazioni As New System.Text.StringBuilder()

        For i As Integer = 0 To listElements.Count - 1
            If listElements(i).CodicePazienteAlias.HasValue AndAlso listElements(i).CodicePazienteAlias = Me.CodicePazienteAlias Then
                stbElencoVaccinazioni.AppendFormat("<span class='itemAlias' title='Vaccinazione relativa al paziente Alias'>{0}({1})</span>, ", listElements(i).CodiceVaccinazione, listElements(i).NumeroRichiamo.ToString())
            Else
                stbElencoVaccinazioni.AppendFormat("{0}({1}), ", listElements(i).CodiceVaccinazione, listElements(i).NumeroRichiamo.ToString())
            End If
        Next
        If stbElencoVaccinazioni.Length > 0 Then
            stbElencoVaccinazioni.Remove(stbElencoVaccinazioni.Length - 2, 2)
        End If

        Return stbElencoVaccinazioni.ToString()

    End Function

    ' Bind dei cicli del paziente in un'unica stringa separata dalle virgole. 
    ' I cicli con paz_codice_old uguale al codice dell'alias vengono colorati diversamente.
    Private Sub BindCicliPaziente(ByVal pazCodiceMaster As Integer, ByVal genericProvider As DAL.DbGenericProvider)
        Dim listCicliPaziente As List(Of Entities.CicloPaziente) = genericProvider.Cicli.LoadCicliPaziente(pazCodiceMaster)

        Dim stbElencoCicli As New System.Text.StringBuilder()

        For i As Integer = 0 To listCicliPaziente.Count - 1
            If listCicliPaziente(i).CodicePazienteAlias = CodicePazienteAlias Then
                stbElencoCicli.AppendFormat("<span class='itemAlias' title='Ciclo relativo al paziente Alias'>{0}</span>, ", listCicliPaziente(i).CodiceCiclo)
            Else
                stbElencoCicli.AppendFormat("{0}, ", listCicliPaziente(i).CodiceCiclo)
            End If
        Next
        If stbElencoCicli.Length > 0 Then
            stbElencoCicli.Remove(stbElencoCicli.Length - 2, 2)
        End If

        lblCicliPaziente.Text = stbElencoCicli.ToString()
    End Sub

#End Region

#Region " Datagrid Events & Functions "

    Private Sub dgr_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) _
        Handles dgrVacEseguite.ItemDataBound, dgrVacEscluse.ItemDataBound, dgrVisite.ItemDataBound

        Me.SetDatiVaccinaliAlias(e.Item)

    End Sub

    ' Colora le righe che hanno il paz_codice_old uguale al codice dell'alias.
    ' N.B. : Utilizzo la stessa funzione per tutti i datagrid in cui serve, 
    ' mettendo il paz_codice_old sempre nella prima colonna (indice 0)
    Private Sub SetDatiVaccinaliAlias(ByVal item As DataGridItem)

        If item.ItemType = ListItemType.Item OrElse item.ItemType = ListItemType.AlternatingItem Then

            ' Colore riga a seconda che il campo codice_paziente_old contenga il codice dell'alias
            If Me.CodicePazienteAlias > 0 Then

                Dim codAlias As String = item.Cells(ColumnIndexDatiVaccinali.paz_codice_old).Text

                If Not String.IsNullOrEmpty(codAlias) AndAlso codAlias = Me.CodicePazienteAlias.ToString() Then

                    item.CssClass = "aliasItemBackground"

                End If

            End If

        End If

    End Sub

    Protected Function ApplyDateFormat(ByVal objectToBind As Object, ByVal dateOnly As Boolean) As String

        ' objectToBind deve essere una data
        If (objectToBind Is Nothing) Then Return String.Empty

        Dim returnString As String = String.Empty

        Dim dateToBind As DateTime

        Try
            dateToBind = Convert.ToDateTime(objectToBind)
        Catch ex As Exception
            returnString = String.Empty
        End Try

        If dateToBind > DateTime.MinValue Then
            If dateOnly Then
                returnString = String.Format("{0:dd/MM/yyyy}", dateToBind)
            Else
                returnString = String.Format("{0:dd/MM/yyyy HH:mm}", dateToBind)
            End If
        End If

        Return returnString

    End Function

    Protected Function ApplyNumberFormat(ByVal objectToBind As Object) As String

        If (objectToBind Is Nothing) Then Return String.Empty

        Dim returnString As String = String.Empty

        Dim intToBind As Int32

        Try
            intToBind = Convert.ToInt32(objectToBind)
        Catch ex As Exception
            returnString = String.Empty
        End Try

        If intToBind = 0 Then
            returnString = String.Empty
        Else
            returnString = intToBind.ToString()
        End If

        Return returnString

    End Function

#End Region


End Class