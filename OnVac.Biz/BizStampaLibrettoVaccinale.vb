Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL


Public Class BizStampaLibrettoVaccinale
    Inherits BizClass

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos)
        MyBase.New(genericprovider, settings, contextInfos, Nothing)
    End Sub

    Public Sub New(genericprovider As DbGenericProvider, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)
        MyBase.New(genericprovider, settings, contextInfos, logOptions)
    End Sub

#End Region

#Region " Types "

    Public Class AssociazioneLibretto

        Public DescrizioneAssociazione As String
        Public Posizione As Integer
        Public RigheStampa As Integer

        Public Sub New()
        End Sub

        Public Sub New(descrizioneAssociazione As String, righeStampa As Integer)
            Me.DescrizioneAssociazione = descrizioneAssociazione
            Me.RigheStampa = righeStampa
        End Sub

    End Class

    'Private Enum TipoVaccinazioniLibretto
    '    ANTIPOLIO = 0
    '    ANTIDIFTOTETANO_PERTOSSE = 1
    '    ANTIDIFTOTETANO = 2
    '    ANTIEPATITE_B = 3
    '    HAEMOPHILUS_INFLUENTIAE_B = 4
    '    ANTIPNEUMOCOCCO = 5
    '    ANTIMENINGOCOCCO = 6
    '    ANTIMORBILLO = 7
    '    ANTIROSOLIA = 8
    '    ANTIPAROTITE = 9
    '    ANTIVARICELLA = 10
    '    ALTRE = 11
    'End Enum

    Private Class PosizioneVaccinazioneLibretto
        Public Const ANTIPOLIO As Integer = 0
        Public Const ANTIDIFTOTETANO_PERTOSSE As Integer = 1
        Public Const ANTIDIFTOTETANO As Integer = 2
        Public Const ANTIEPATITE_B As Integer = 3
        Public Const HAEMOPHILUS_INFLUENTIAE_B As Integer = 4
        Public Const ANTIPNEUMOCOCCO As Integer = 5
        Public Const ANTIMENINGOCOCCO As Integer = 6
        Public Const ANTIMORBILLO As Integer = 7
        Public Const ANTIROSOLIA As Integer = 8
        Public Const ANTIPAROTITE As Integer = 9
        Public Const ANTIVARICELLA As Integer = 10
        Public Const ALTRE As Integer = 11
    End Class

    Private Class CodiceUnicoVaccinazione
        '--
        Public Const POL As String = "POL"
        '--
        Public Const DIF As String = "DIF"
        Public Const TET As String = "TET"
        Public Const PER As String = "PER"
        '--
        Public Const HB As String = "HB"
        '--
        Public Const HIB As String = "HIB"
        '--
        Public Const PNC As String = "PNC"
        Public Const PNC23 As String = "PNC"
        '--
        Public Const MNC As String = "MNC"
        Public Const MNCA As String = "MNC"
        Public Const ACYW135 As String = "MNC"
        '--
        Public Const MOR As String = "MOR"
        '--
        Public Const ROS As String = "ROS"
        '--
        Public Const PAR As String = "PAR"
        '--
        Public Const VAR As String = "VAR"
        '--
    End Class

    Public Class DatiLibrettoVaccinalePazienteResult

        Public ReadOnly DstLibrettoVaccinale As LibrettoVaccinale
        Public ReadOnly AssociazioniLibretto As List(Of AssociazioneLibretto)

        Public Sub New(dstLibrettoVaccinale As LibrettoVaccinale, associazioniLibretto As List(Of AssociazioneLibretto))
            Me.DstLibrettoVaccinale = dstLibrettoVaccinale
            Me.AssociazioniLibretto = associazioniLibretto
        End Sub

    End Class

#End Region

#Region " Public Methods "

    Public Function GetDataSetLibrettoVaccinale(codicePaziente As Integer) As DSLibrettoVaccinale

        Dim vaccinatore As String = Me.Settings.LIBRETTO_VAC_MEDICO

        Return Me.GenericProvider.StampaLibrettoVaccinale.GetDataSetLibrettoVaccinale(codicePaziente, vaccinatore)

    End Function

    Public Function GetDatiLibrettoVaccinalePazienteSingolaPagina(codicePaziente As Integer) As DatiLibrettoVaccinalePazienteResult

        Dim associazioniLibretto As New List(Of AssociazioneLibretto)()

        associazioniLibretto.Insert(PosizioneVaccinazioneLibretto.ANTIPOLIO, New AssociazioneLibretto("ANTIPOLIO (Eipv - Opv)", 8))
        associazioniLibretto.Insert(PosizioneVaccinazioneLibretto.ANTIDIFTOTETANO_PERTOSSE, New AssociazioneLibretto("ANTIDIFTOTETANO PERTOSSE (DTPa)", 6))
        associazioniLibretto.Insert(PosizioneVaccinazioneLibretto.ANTIDIFTOTETANO, New AssociazioneLibretto("ANTIDIFTOTETANO (DT)", 6))
        associazioniLibretto.Insert(PosizioneVaccinazioneLibretto.ANTIEPATITE_B, New AssociazioneLibretto("ANTIEPATITE B (HBV)", 4))
        associazioniLibretto.Insert(PosizioneVaccinazioneLibretto.HAEMOPHILUS_INFLUENTIAE_B, New AssociazioneLibretto("HAEMOPHILUS INFLUENTIAE B (HIB)", 4))
        associazioniLibretto.Insert(PosizioneVaccinazioneLibretto.ANTIPNEUMOCOCCO, New AssociazioneLibretto("ANTIPNEUMOCOCCO (PNC)", 4))
        associazioniLibretto.Insert(PosizioneVaccinazioneLibretto.ANTIMENINGOCOCCO, New AssociazioneLibretto("ANTIMENINGOCOCCO (MNC)", 4))
        associazioniLibretto.Insert(PosizioneVaccinazioneLibretto.ANTIMORBILLO, New AssociazioneLibretto("ANTIMORBILLO / MEASLES", 3))
        associazioniLibretto.Insert(PosizioneVaccinazioneLibretto.ANTIROSOLIA, New AssociazioneLibretto("ANTIROSOLIA / GERMAN MEASLES", 3))
        associazioniLibretto.Insert(PosizioneVaccinazioneLibretto.ANTIPAROTITE, New AssociazioneLibretto("ANTIPAROTITE / MUMPS", 3))
        associazioniLibretto.Insert(PosizioneVaccinazioneLibretto.ANTIVARICELLA, New AssociazioneLibretto("ANTIVARICELLA / CHICKEN POX", 2))
        associazioniLibretto.Insert(PosizioneVaccinazioneLibretto.ALTRE, New AssociazioneLibretto("ALTRE / OTHERS", 20))

        ' Dati paziente
        Dim paziente As Entities.Paziente

        Using bizPaz As Biz.BizPaziente = New Biz.BizPaziente(Me.GenericProvider, Me.Settings, Me.ContextInfos, Me.LogOptions)
            paziente = bizPaz.CercaPaziente(codicePaziente)
        End Using

        Dim dstLibrettoVaccinale As New LibrettoVaccinale()

        Dim rowPazienti As DataRow = dstLibrettoVaccinale.Tables("PAZIENTI").NewRow()

        rowPazienti("paz_codice") = codicePaziente
        rowPazienti("paz_cognome") = paziente.PAZ_COGNOME
        rowPazienti("paz_nome") = paziente.PAZ_NOME
        rowPazienti("paz_data_nascita") = paziente.Data_Nascita

        dstLibrettoVaccinale.Tables("PAZIENTI").Rows.Add(rowPazienti)

        Dim dtVacPaziente As DataTable = Me.GenericProvider.StampaLibrettoVaccinale.GetDtVaccinazioniPaziente(codicePaziente)

        Dim dwVacPaziente As DataView = dtVacPaziente.DefaultView
        dwVacPaziente.Sort = "VAC_COD_UNICO, VES_N_RICHIAMO"

        Dim count As Integer = 0
        Dim rowVaccinazioni As DataRow

        While 0 < dwVacPaziente.Count

            rowVaccinazioni = dstLibrettoVaccinale.Tables("VACCINAZIONI").NewRow()
            rowVaccinazioni("paz_codice") = codicePaziente
            rowVaccinazioni("vac_dose") = dwVacPaziente(0)("VES_N_RICHIAMO")
            rowVaccinazioni("vac_data") = dwVacPaziente(0)("VES_DATA_EFFETTUAZIONE")
            rowVaccinazioni("vac_nome") = dwVacPaziente(0)("VAC_DESCRIZIONE")
            rowVaccinazioni("vac_associazione") = dwVacPaziente(0)("ASS_STAMPA") & ""
            rowVaccinazioni("vac_fittizia") = dwVacPaziente(0)("VES_FLAG_FITTIZIA") & ""

            If ((dwVacPaziente.Item(0)("VAC_COD_UNICO")).GetType()).ToString() <> "System.DBNull" Then

                Select Case dwVacPaziente.Item(0)("VAC_COD_UNICO").ToString()

                    Case CodiceUnicoVaccinazione.POL
                        rowVaccinazioni("vac_tipo") = associazioniLibretto(PosizioneVaccinazioneLibretto.ANTIPOLIO).DescrizioneAssociazione

                    Case CodiceUnicoVaccinazione.DIF, CodiceUnicoVaccinazione.TET, CodiceUnicoVaccinazione.PER

                        dwVacPaziente.RowFilter = "VAC_COD_UNICO IN " &
                                                  String.Format("('{0}','{1}','{2}')", CodiceUnicoVaccinazione.DIF, CodiceUnicoVaccinazione.TET, CodiceUnicoVaccinazione.PER) &
                                                  " AND VES_N_RICHIAMO = " & dwVacPaziente(0)("VES_N_RICHIAMO") &
                                                  " AND VES_DATA_EFFETTUAZIONE = '" & dwVacPaziente(0)("VES_DATA_EFFETTUAZIONE") & "'"

                        If dwVacPaziente.Count = 3 Then
                            rowVaccinazioni("vac_tipo") = associazioniLibretto(PosizioneVaccinazioneLibretto.ANTIDIFTOTETANO_PERTOSSE).DescrizioneAssociazione
                            For count = dwVacPaziente.Count - 1 To 0 Step -1
                                dwVacPaziente(count).Delete()
                            Next
                            Exit Select
                        Else
                            dwVacPaziente.RowFilter = String.Empty
                        End If

                        dwVacPaziente.RowFilter = "VAC_COD_UNICO IN " &
                                                  String.Format("('{0}','{1}')", CodiceUnicoVaccinazione.DIF, CodiceUnicoVaccinazione.TET) &
                                                  " AND VES_N_RICHIAMO = " & dwVacPaziente(0)("VES_N_RICHIAMO") &
                                                  " AND VES_DATA_EFFETTUAZIONE = '" & dwVacPaziente(0)("VES_DATA_EFFETTUAZIONE") & "'"

                        If dwVacPaziente.Count = 2 Then
                            rowVaccinazioni("vac_tipo") = associazioniLibretto(PosizioneVaccinazioneLibretto.ANTIDIFTOTETANO).DescrizioneAssociazione
                            For count = dwVacPaziente.Count - 1 To 0 Step -1
                                dwVacPaziente(count).Delete()
                            Next
                            Exit Select
                        Else
                            rowVaccinazioni("vac_tipo") = associazioniLibretto(PosizioneVaccinazioneLibretto.ALTRE).DescrizioneAssociazione
                            dwVacPaziente.RowFilter = String.Empty
                        End If

                    Case CodiceUnicoVaccinazione.HB

                        rowVaccinazioni("vac_tipo") = associazioniLibretto(PosizioneVaccinazioneLibretto.ANTIEPATITE_B).DescrizioneAssociazione

                    Case CodiceUnicoVaccinazione.HIB

                        rowVaccinazioni("vac_tipo") = associazioniLibretto(PosizioneVaccinazioneLibretto.HAEMOPHILUS_INFLUENTIAE_B).DescrizioneAssociazione

                    Case CodiceUnicoVaccinazione.PNC,
                         CodiceUnicoVaccinazione.PNC23

                        rowVaccinazioni("vac_tipo") = associazioniLibretto(PosizioneVaccinazioneLibretto.ANTIPNEUMOCOCCO).DescrizioneAssociazione

                    Case CodiceUnicoVaccinazione.MNC,
                         CodiceUnicoVaccinazione.MNCA,
                         CodiceUnicoVaccinazione.ACYW135

                        rowVaccinazioni("vac_tipo") = associazioniLibretto(PosizioneVaccinazioneLibretto.ANTIMENINGOCOCCO).DescrizioneAssociazione

                    Case CodiceUnicoVaccinazione.MOR

                        rowVaccinazioni("vac_tipo") = associazioniLibretto(PosizioneVaccinazioneLibretto.ANTIMORBILLO).DescrizioneAssociazione

                    Case CodiceUnicoVaccinazione.ROS

                        rowVaccinazioni("vac_tipo") = associazioniLibretto(PosizioneVaccinazioneLibretto.ANTIROSOLIA).DescrizioneAssociazione

                    Case CodiceUnicoVaccinazione.PAR

                        rowVaccinazioni("vac_tipo") = associazioniLibretto(PosizioneVaccinazioneLibretto.ANTIPAROTITE).DescrizioneAssociazione

                    Case CodiceUnicoVaccinazione.VAR

                        rowVaccinazioni("vac_tipo") = associazioniLibretto(PosizioneVaccinazioneLibretto.ANTIVARICELLA).DescrizioneAssociazione

                    Case Else

                        rowVaccinazioni("vac_tipo") = associazioniLibretto(PosizioneVaccinazioneLibretto.ALTRE).DescrizioneAssociazione

                End Select

            Else
                rowVaccinazioni("vac_tipo") = associazioniLibretto(PosizioneVaccinazioneLibretto.ALTRE).DescrizioneAssociazione
            End If

            dstLibrettoVaccinale.Tables("VACCINAZIONI").Rows.Add(rowVaccinazioni)

            If dwVacPaziente.Count > 0 Then dwVacPaziente(0).Delete()
            dwVacPaziente.RowFilter = String.Empty

        End While

        'riempio le righe rimanenti con la linea per passare il tutto ai sottoreport relativi
        For count = 0 To associazioniLibretto.Count - 1

            Dim filtroVacTipo As String = String.Format("vac_tipo = '{0}'", associazioniLibretto(count).DescrizioneAssociazione)

            For contaRighe As Integer = dstLibrettoVaccinale.Tables("VACCINAZIONI").Select(filtroVacTipo).Length To associazioniLibretto(count).RigheStampa - 1

                rowVaccinazioni = dstLibrettoVaccinale.Tables("VACCINAZIONI").NewRow()

                rowVaccinazioni("paz_codice") = codicePaziente
                rowVaccinazioni("vac_dose") = 0
                rowVaccinazioni("vac_data") = #1/1/1900#
                rowVaccinazioni("vac_associazione") = ""
                rowVaccinazioni("vac_nome") = ""
                rowVaccinazioni("vac_tipo") = associazioniLibretto(count).DescrizioneAssociazione
                rowVaccinazioni("vac_fittizia") = "N"

                dstLibrettoVaccinale.Tables("VACCINAZIONI").Rows.Add(rowVaccinazioni)

            Next

        Next

        Return New DatiLibrettoVaccinalePazienteResult(dstLibrettoVaccinale, associazioniLibretto)

    End Function

#End Region

End Class
