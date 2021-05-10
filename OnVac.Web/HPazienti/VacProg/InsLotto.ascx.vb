Imports Onit.Database.DataAccessManager
Imports System.Collections.Generic


Partial Class InsLotto
    Inherits Common.UserControlFinestraModalePageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Fields "

    Protected WithEvents OnitLayout31 As Onit.Controls.PagesLayout.OnitLayout3
    Protected WithEvents DatiLotto As InsDatiLotto

#End Region

#Region " Events "

    Public Event InsLotti()
    Public Event AnnullaLotti()

    Protected Sub OnInsLotti()
        RaiseEvent InsLotti()
    End Sub

#End Region

#Region " Public Members "

    Public strJS As String

#End Region

#Region " Public Methods "

    Public Overrides Sub LoadModale()
        '--
        Me.DatiLotto.ModaleName = "modInsLotto"
        Me.DatiLotto.Modalita = InsDatiLotto.Mode.Nuovo
        Me.DatiLotto.SetLottoMagazzino(Nothing)
        '--
        ' Imposto un ritardo in maniera che il focus venga impostato DOPO
        ' che la finestra modale è stata visualizzata, altrimenti fa errore
        Me.strJS &= "setTimeout('impostaFocus()',500)" & vbCrLf
        Me.strJS &= "function impostaFocus(){" & DatiLotto.GetJSFocusLotto() & "}" & vbCrLf
        '--
    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub ToolBar_ButtonClicked(ByVal sender As Object, ByVal e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case (e.Button.Key)

            Case "btn_Salva"

                Me.Salva()

        End Select

    End Sub

#End Region

#Region " Private Methods "

    Private Sub Salva()

        Using dam As IDAM = OnVacUtility.OpenDam()

            Try

                dam.BeginTrans()

                Using genericProvider As New DAL.DbGenericProvider(dam)

                    Dim lottoMagazzino As Entities.LottoMagazzino = Me.DatiLotto.GetLottoMagazzino()

                    Dim bizLottiResult As Biz.BizLotti.BizLottiResult = Nothing
                    Dim listTestateLog As New List(Of Testata)()

                    Using bizLotti As New Biz.BizLotti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                        bizLottiResult = bizLotti.InsertLottoMagazzino(lottoMagazzino, True, OnVacUtility.Variabili.CNS.Codice,
                                                                       OnVacUtility.Variabili.CNSMagazzino.Codice, Me.DatiLotto.GetQuantitaIniziale(),
                                                                       (Me.DatiLotto.GetUnitaMisuraDose() = Enumerators.UnitaMisuraLotto.Scatola),
                                                                       False, Me.Settings.ASSOCIA_LOTTI_ETA, listTestateLog)
                    End Using

                    If bizLottiResult.Result = Biz.BizLotti.BizLottiResult.ResultType.Success Then

                        ' Se tutto ok, effettua l'inserimento, scrive il log e fa scattare l'evento.

                        If Not dam Is Nothing AndAlso dam.ExistTra() Then

                            dam.Commit()

                            For Each testata As Testata In listTestateLog
                                LogBox.WriteData(testata)
                            Next

                        End If

                        Me.OnInsLotti()

                    Else

                        ' Altrimenti, anche in caso di warning, l'inserimento non viene effettuato
                        ' e l'utente viene avvertito.

                        If Not dam Is Nothing AndAlso dam.ExistTra() Then dam.Rollback()

                        If bizLottiResult.Result = Biz.BizLotti.BizLottiResult.ResultType.IsActiveLottoWarning Then

                            ' Non ho più il codice del lotto attivo, volendo si potrebbe fare una query per recuperarlo.
                            '        strJS += "alert(""Esiste già un 'Lotto Attivo' [ " + codiceLottoAttivo + " ] per lo stesso 'Nome Commerciale' nello stesso Centro Vaccinale.\nNon è possibile confermare il nuovo lotto."");"
                            Me.strJS += "alert(""Esiste già un 'Lotto Attivo' per lo stesso 'Nome Commerciale' nello stesso Centro Vaccinale.\nNon è possibile confermare il nuovo lotto."");"

                        Else

                            Me.strJS += "alert(""" + bizLottiResult.Message + """);"

                        End If

                    End If

                End Using

            Catch ex As Exception

                If Not dam Is Nothing AndAlso dam.ExistTra() Then dam.Rollback()

            End Try

        End Using

    End Sub

#End Region

End Class
