Imports Onit.Database.DataAccessManager

Namespace DAL.Oracle

    Public Class DbElencoBilanciProvider
        Implements IElencoBilanciProvider
      
        Private _DAM As IDAM
        Private _RET As Object
        Dim _dt As DataTable

#Region " Costruttori"

        Public Sub New(ByRef DAM As IDAM)

            _DAM = DAM

        End Sub

#End Region

#Region " Select "

        Public Function fillDtElencoBilanci(ByRef dt As DataTable, codCns As String, codAmb As Integer, strDataInizio As String, strDataFine As String, filtroPazAvvisati As OnVac.Enumerators.FiltroAvvisati, malattia As String, codiceUsl As String) As Boolean Implements IElencoBilanciProvider.fillDtElencoBilanci

            If dt Is Nothing Then
                Exit Function
            End If

            'SELECT   paz_codice, paz_cognome, paz_nome, paz_data_nascita,paz_libero_1,
            '      res.com_descrizione AS res_com_descrizione,
            '      dom.com_descrizione AS dom_com_descrizione,
            '      cnv_data_appuntamento, bip_bil_numero,
            '      mal_descrizione , cns_descrizione,med_descrizione,
            '      (select max(ves_data_effettuazione) from t_vac_eseguite
            '      where ves_paz_codice = paz_codice )as data_ultima_vaccinazione
            ' FROM t_cnv_convocazioni,
            '      t_bil_programmati,                
            '      t_paz_pazienti,
            '      t_ana_comuni res,
            '      t_ana_comuni dom,
            '      t_ana_consultori,
            '      t_paz_malattie,
            '      t_ana_malattie,
            '      t_ana_medici
            'WHERE paz_codice = cnv_paz_codice
            '  AND paz_com_codice_domicilio = dom.com_codice(+)
            '  AND paz_com_codice_residenza = res.com_codice(+)
            '  AND paz_cns_codice = cns_codice
            '  AND bip_paz_codice = cnv_paz_codice                          
            '  AND bip_cnv_data = cnv_data                                   
            '  and paz_codice = pma_paz_codice
            '  and bip_mal_codice = pma_mal_codice
            '  and bip_mal_codice = mal_codice
            '  AND bip_bil_numero IS NOT NULL
            '  AND bip_mal_codice <> '0'
            '  AND paz_med_codice_base = med_codice(+)

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("pno_testo_note")
                .AddTables("t_ana_tipo_note, t_paz_note")
                .AddWhereCondition("tno_codice", Comparatori.Uguale, "pno_tno_codice", DataTypes.Join)
                .AddWhereCondition("pno_paz_codice", Comparatori.Uguale, "paz_codice", DataTypes.Join)
                .OpenParanthesis()
                .AddWhereCondition("pno_azi_codice", Comparatori.Uguale, codiceUsl, DataTypes.Stringa)
                .AddWhereCondition("pno_azi_codice", Comparatori.In, String.Format("select dis_codice from t_ana_distretti where dis_usl_codice = '{0}'", codiceUsl), DataTypes.Replace, "OR")
                .CloseParanthesis()
                .AddWhereCondition("pno_tno_codice", Comparatori.Uguale, Constants.CodiceTipoNotaPaziente.Appuntamenti, DataTypes.Stringa)
            End With
            Dim queryNotaLibero1 As String = _DAM.QB.GetSelect()


            With _DAM.QB
                .NewQuery(False, False)
                .AddSelectFields("paz_codice", "paz_cognome", "paz_nome", "paz_data_nascita")
                .AddSelectFields("(" + queryNotaLibero1 + ") paz_libero_1")
                .AddSelectFields("res.com_descrizione as res_com_descrizione", "dom.com_descrizione as dom_com_descrizione",
                                 "cnv_data_appuntamento", "bip_bil_numero", "mal_descrizione", "cns_descrizione",
                                 "med_descrizione", "(select max(ves_data_effettuazione) from t_vac_eseguite where ves_paz_codice = paz_codice)as data_ultima_vaccinazione")

                .AddTables("t_cnv_convocazioni", "t_bil_programmati", "t_paz_pazienti", "t_ana_comuni res", "t_ana_comuni dom", "t_ana_consultori", "t_paz_malattie", "t_ana_malattie", "t_ana_medici")

                .AddWhereCondition("paz_codice", Comparatori.Uguale, "cnv_paz_codice", DataTypes.Join)
                .AddWhereCondition("paz_com_codice_domicilio", Comparatori.Uguale, "dom.com_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("paz_com_codice_residenza", Comparatori.Uguale, "res.com_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("paz_cns_codice", Comparatori.Uguale, "cns_codice", DataTypes.Join)
                .AddWhereCondition("bip_paz_codice", Comparatori.Uguale, "cnv_paz_codice", DataTypes.Join)
                .AddWhereCondition("bip_cnv_data", Comparatori.Uguale, "cnv_data", DataTypes.Join)
                .AddWhereCondition("paz_codice", Comparatori.Uguale, "pma_paz_codice", DataTypes.Join)
                .AddWhereCondition("bip_mal_codice", Comparatori.Uguale, "pma_mal_codice", DataTypes.Join)
                .AddWhereCondition("bip_mal_codice", Comparatori.Uguale, "mal_codice", DataTypes.Join)
                .AddWhereCondition("bip_bil_numero", Comparatori.[IsNot], "null", DataTypes.Numero)
                .AddWhereCondition("bip_mal_codice", Comparatori.Diverso, "0", DataTypes.Stringa)
                .AddWhereCondition("paz_med_codice_base", Comparatori.Uguale, "med_codice", DataTypes.OutJoinLeft)

                ''' Optional filters

                If codCns <> "" Then
                    .AddWhereCondition("CNV_CNS_CODICE", Comparatori.Uguale, codCns, DataTypes.Stringa)
                End If
                If Not codAmb = Nothing AndAlso codAmb <> 0 Then
                    .AddWhereCondition("CNV_AMB_CODICE", Comparatori.Uguale, codAmb, DataTypes.Numero)
                End If

                .AddWhereCondition("cnv_data_appuntamento", Comparatori.MaggioreUguale, strDataInizio + " 0.00", DataTypes.DataOra)
                .AddWhereCondition("cnv_data_appuntamento", Comparatori.MinoreUguale, strDataFine + " 23.59", DataTypes.DataOra)
                If malattia <> "" Then
                    .AddWhereCondition("bip_mal_codice", Comparatori.Uguale, malattia, DataTypes.Stringa)
                End If
                Select Case filtroPazAvvisati
                    Case OnVac.Enumerators.FiltroAvvisati.SoloAvvisati
                        ' Solo pazienti che hanno la data di invio valorizzata
                        .AddWhereCondition("cnv_data_invio", Comparatori.[IsNot], "NULL", DataTypes.Replace)
                    Case OnVac.Enumerators.FiltroAvvisati.SoloNonAvvisati
                        ' Solo pazienti che hanno la data di invio non valorizzata
                        .AddWhereCondition("cnv_data_invio", Comparatori.Is, "NULL", DataTypes.Replace)
                    Case OnVac.Enumerators.FiltroAvvisati.Tutti
                        ' Tutti i pazienti, quindi nessun filtro
                End Select
            End With

            Try

                _DAM.BuildDataTable(dt)

            Catch ex As Exception
                dt = Nothing
                Return False
            End Try

            Return True

        End Function

#End Region

    End Class

End Namespace