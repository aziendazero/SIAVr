Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.Common

Partial Class OnVacAlias
    Inherits UserControlPageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As Object

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Types "

    Private Class AliasKeys

        Public PAZ_CODICE As Int64
        Public PAZ_CODICE_AUSILIARIO As String
        Public PAZ_CANCELLATO As Boolean

    End Class

#End Region

#Region " Events "

    Public Event ConfermaAlias(sender As Object, e As OnVacAliasArgs)
    Public Event AliasNonEseguibile(sender As Object, e As AliasNonEseguibileArgs)

#End Region

#Region " Public "

    Public Sub ImpostaDati(dt As DataTable)

        Dim msgAliasNonEseguibile As String = String.Empty
        Dim showAliases As Boolean = True

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then

            msgAliasNonEseguibile = "Impossibile eseguire l'alias: nessun paziente selezionato!"
            showAliases = False

        ElseIf dt.Rows.Count = 1 Then

            msgAliasNonEseguibile = "Impossibile eseguire l'alias: un solo paziente selezionato!"
            showAliases = False

        Else

            ' Se, tra i pazienti selezionati, almeno uno non ha il codice locale, non posso eseguire l'alias.
            For i As Integer = 0 To dt.Rows.Count - 1

                If dt.Rows(i)("PAZ_CODICE") Is Nothing OrElse dt.Rows(i)("PAZ_CODICE") Is DBNull.Value Then

                    ' Il paziente locale deve essere agganciato al centrale dal paz_codice_ausiliario. 
                    ' Se il paziente locale è un fantasma e TIPOANAG = 2, non ritorna dalla ricerca paziente con 
                    msgAliasNonEseguibile = "Impossibile effettuare l'alias: tutte le posizioni anagrafiche selezionate devono essere presenti in locale!"
                    showAliases = False

                    Exit For

                End If

            Next

        End If

        If showAliases Then

            dgrListaAlias.DataSource = dt

        Else

            dgrListaAlias.DataSource = Nothing
            RaiseEvent AliasNonEseguibile(Me, New AliasNonEseguibileArgs() With {
              .ChiudiControllo = True,
              .Messaggio = msgAliasNonEseguibile})

        End If

        dgrListaAlias.DataBind()

    End Sub

#End Region

#Region " Private "

    Private Sub toolbarAlias_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles toolbarAlias.ButtonClicked

        Select Case be.Button.Key

            Case "btnConferma"

                Conferma()

            Case "btnAnnulla"

                Annulla()

        End Select

    End Sub

    Private Sub Conferma()

        If dgrListaAlias.SelectedItem Is Nothing Then

            RaiseEvent AliasNonEseguibile(Me, New AliasNonEseguibileArgs() With {
                .ChiudiControllo = False,
                .Messaggio = "Selezionare un paziente"})
            Return

        End If

        Dim e As New OnVacAliasArgs()
        Dim lst As New List(Of AliasKeys)()

        ' Determino il codice del master e degli alias
        For i As Integer = 0 To dgrListaAlias.Items.Count - 1

            Dim row As DataGridItem = dgrListaAlias.Items(i)
            Dim ixPazCodice As Integer = dgrListaAlias.getColumnNumberByKey("PAZ_CODICE")
            Dim ixPazCodiceAusiliario As Integer = dgrListaAlias.getColumnNumberByKey("PAZ_CODICE_AUSILIARIO")
            Dim ixPazCancellato As Integer = dgrListaAlias.getColumnNumberByKey("PAZ_CANCELLATO")


            Dim pazCodice As Int64
            If Not Int64.TryParse(HttpUtility.HtmlDecode(row.Cells(ixPazCodice).Text).Trim(), pazCodice) Then
                pazCodice = -1
            End If

            Dim pazCodiceAusiliario As String = HttpUtility.HtmlDecode(row.Cells(ixPazCodiceAusiliario).Text).Trim()
            Dim pazCancellato As Boolean = (HttpUtility.HtmlDecode(row.Cells(ixPazCancellato).Text).Trim() = "S")


            If dgrListaAlias.SelectedItem Is row Then
                '--
                ' Riga relativa al master
                '--
                ' Se il master risulta cancellato mi fermo
                If pazCancellato Then
                    RaiseEvent AliasNonEseguibile(Me, New AliasNonEseguibileArgs() With {
                          .ChiudiControllo = False,
                          .Messaggio = "Impossibile effettuare l'alias: il paziente master risulta cancellato"})
                    Return
                End If
                '--
                e.PazMasterId = pazCodice
                '--
            Else
                '--
                ' Riga relativa all'alias
                '--
                Dim item As New AliasKeys() With {
                    .PAZ_CODICE = pazCodice,
                    .PAZ_CODICE_AUSILIARIO = pazCodiceAusiliario,
                    .PAZ_CANCELLATO = pazCancellato
                }
                '--
                lst.Add(item)
                '--
            End If

        Next

        If Settings.CHECK_DATI_ALIAS_PER_MERGE Then

            ' Controllo della presenza del codice ausiliario sui pazienti alias non cancellati
            Dim lstAliasCheck As List(Of AliasKeys) = lst.Where(Function(p) Not String.IsNullOrWhiteSpace(p.PAZ_CODICE_AUSILIARIO) AndAlso Not p.PAZ_CANCELLATO).ToList()

            If Not lstAliasCheck.IsNullOrEmpty() Then

                Dim msgAliasNonEseguibile As String = String.Empty

                If lstAliasCheck.Count = 1 Then
                    msgAliasNonEseguibile = String.Format("Impossibile effettuare l'alias: il paziente con codice {0} ha anche un codice ausiliario associato", lstAliasCheck(0).PAZ_CODICE)
                Else
                    Dim strCodici As String = lstAliasCheck.Select(Function(p) p.PAZ_CODICE.ToString()).Aggregate(Function(p, g) p & ", " & g)
                    msgAliasNonEseguibile = String.Format("Impossibile effettuare l'alias: i pazienti con codice {0} hanno anche un codice ausiliario associato", strCodici)
                End If

                RaiseEvent AliasNonEseguibile(Me, New AliasNonEseguibileArgs() With {
                    .ChiudiControllo = False,
                    .Messaggio = msgAliasNonEseguibile})
                Return

            End If

        End If

        e.pazAliasIDs = lst.Select(Function(p) p.PAZ_CODICE).ToArray()
        e.Conferma = True

        RaiseEvent ConfermaAlias(Me, e)

    End Sub

    Private Sub Annulla()

        Dim e As New OnVacAliasArgs()
        e.PazMasterId = -1
        e.Conferma = False

        RaiseEvent ConfermaAlias(Me, e)

    End Sub

#End Region

End Class

#Region " Types "

Public Class OnVacAliasArgs
    Inherits System.EventArgs

    Public PazMasterId As Int64
    Public Conferma As Boolean
    Public pazAliasIDs() As Int64 = {}

End Class

Public Class AliasNonEseguibileArgs
    Inherits System.EventArgs

    Public Messaggio As String
    Public ChiudiControllo As Boolean

End Class

#End Region
