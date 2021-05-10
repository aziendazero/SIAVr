Public Class FirmaDigitaleInfo
    Inherits OnVac.Common.UserControlPageBase

#Region " UserControl "

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

    End Sub

#End Region

#Region " Public "

    ''' <summary>
    ''' Restituisce le informazioni su firma/archiviazione relative alla visita specificata.
    ''' </summary>
    ''' <param name="idVisita"></param>
    ''' <param name="codiceAziendaInserimento"></param>
    ''' <remarks></remarks>
    Public Sub SetInfoFirmaDigitaleArchiviazioneSostitutiva(idVisita As Long, codiceAziendaInserimento As String)

        Dim info As Biz.BizFirmaDigitale.GetInfoFirmaArchiviazioneVisitaResult = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizFirma As New Biz.BizFirmaDigitale(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                info = bizFirma.GetInfoFirmaArchiviazioneVisita(idVisita, codiceAziendaInserimento)

            End Using
        End Using

        ' Descrizione stato (uguale al tooltip)
        Me.lblDescrizioneStato.Text =
            OnVacUtility.GetFlagFirmaToolTipValue(info.InfoFirmaArchiviazioneVisita.UtenteFirma, info.InfoFirmaArchiviazioneVisita.UtenteArchiviazione)

        ' Info di firma e archiviazione
        If String.IsNullOrWhiteSpace(info.InfoFirmaArchiviazioneVisita.UtenteFirma) Then
            Me.lblUtenteFirma.Text = "&nbsp;"
        Else
            Me.lblUtenteFirma.Text = info.InfoFirmaArchiviazioneVisita.UtenteFirma
        End If

        If info.InfoFirmaArchiviazioneVisita.DataFirma.HasValue Then
            Me.lblDataFirma.Text = info.InfoFirmaArchiviazioneVisita.DataFirma.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
        Else
            Me.lblDataFirma.Text = "&nbsp;"
        End If

        If String.IsNullOrWhiteSpace(info.InfoFirmaArchiviazioneVisita.UtenteArchiviazione) Then
            Me.lblUtenteArchiviazione.Text = "&nbsp;"
        Else
            Me.lblUtenteArchiviazione.Text = info.InfoFirmaArchiviazioneVisita.UtenteArchiviazione
        End If

        If info.InfoFirmaArchiviazioneVisita.DataArchiviazione.HasValue Then
            Me.lblDataArchiviazione.Text = info.InfoFirmaArchiviazioneVisita.DataArchiviazione.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
        Else
            Me.lblDataArchiviazione.Text = "&nbsp;"
        End If

        If String.IsNullOrWhiteSpace(info.TokenArchiviazione) Then
            Me.lblTokenArchiviazione.Text = "&nbsp;"
        Else
            Me.lblTokenArchiviazione.Text = info.TokenArchiviazione
        End If

    End Sub

#End Region

End Class