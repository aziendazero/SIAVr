Namespace Entities

    <Serializable()>
    Public Class VaccinazioneEseguita

#Region " Properties "

        Private _cnv_data As Date
        Public Property cnv_data() As Date
            Get
                Return _cnv_data
            End Get
            Set(Value As Date)
                _cnv_data = Value
            End Set
        End Property

        Private _ves_cnv_data_primo_app As Date?
        Public Property ves_cnv_data_primo_app() As Date?
            Get
                Return _ves_cnv_data_primo_app
            End Get
            Set(Value As Date?)
                _ves_cnv_data_primo_app = Value
            End Set
        End Property

        Private _paz_codice As Integer
        Public Property paz_codice() As Integer
            Get
                Return _paz_codice
            End Get
            Set(Value As Integer)
                _paz_codice = Value
            End Set
        End Property

        Private _ves_paz_codice_old As Integer
        Public Property ves_paz_codice_old() As Integer
            Get
                Return _ves_paz_codice_old
            End Get
            Set(Value As Integer)
                _ves_paz_codice_old = Value
            End Set
        End Property

        Private _ves_id As Long?
        Public Property ves_id() As Long?
            Get
                Return _ves_id
            End Get
            Set(Value As Long?)
                _ves_id = Value
            End Set
        End Property

        Private _ves_n_richiamo As Integer
        Public Property ves_n_richiamo() As Integer
            Get
                Return _ves_n_richiamo
            End Get
            Set(Value As Integer)
                _ves_n_richiamo = Value
            End Set
        End Property

        Private _ves_n_seduta As Integer?
        Public Property ves_n_seduta() As Integer?
            Get
                Return _ves_n_seduta
            End Get
            Set(Value As Integer?)
                _ves_n_seduta = Value
            End Set
        End Property

        Private _ves_vac_codice As String
        Public Property ves_vac_codice() As String
            Get
                Return _ves_vac_codice
            End Get
            Set(Value As String)
                _ves_vac_codice = Value
            End Set
        End Property

        Private _vac_descrizione As String
        Public Property vac_descrizione() As String
            Get
                Return _vac_descrizione
            End Get
            Set(Value As String)
                _vac_descrizione = Value
            End Set
        End Property

        Private _ves_ass_prog As String
        Public Property ves_ass_prog() As String
            Get
                Return _ves_ass_prog
            End Get
            Set(Value As String)
                _ves_ass_prog = Value
            End Set
        End Property

        Private _ves_ass_codice As String
        Public Property ves_ass_codice() As String
            Get
                Return _ves_ass_codice
            End Get
            Set(Value As String)
                _ves_ass_codice = Value
            End Set
        End Property

        Private _ves_ass_n_dose As Int32
        Public Property ves_ass_n_dose() As Int32
            Get
                Return _ves_ass_n_dose
            End Get
            Set(Value As Int32)
                _ves_ass_n_dose = Value
            End Set
        End Property

        Private _ass_descrizione As String
        Public Property ass_descrizione() As String
            Get
                Return _ass_descrizione
            End Get
            Set(Value As String)
                _ass_descrizione = Value
            End Set
        End Property

        Private _ves_noc_codice As String
        Public Property ves_noc_codice() As String
            Get
                Return _ves_noc_codice
            End Get
            Set(Value As String)
                _ves_noc_codice = Value
            End Set
        End Property

        Private _noc_descrizione As String
        Public Property noc_descrizione() As String
            Get
                Return _noc_descrizione
            End Get
            Set(Value As String)
                _noc_descrizione = Value
            End Set
        End Property

        Private _ves_sii_codice As String
        Public Property ves_sii_codice() As String
            Get
                Return _ves_sii_codice
            End Get
            Set(Value As String)
                _ves_sii_codice = Value
            End Set
        End Property

        Private _sii_descrizione As String
        Public Property sii_descrizione() As String
            Get
                Return _sii_descrizione
            End Get
            Set(Value As String)
                _sii_descrizione = Value
            End Set
        End Property

        Private _ves_vii_codice As String
        Public Property ves_vii_codice() As String
            Get
                Return _ves_vii_codice
            End Get
            Set(Value As String)
                _ves_vii_codice = Value
            End Set
        End Property

        Private _ves_lot_codice As String
        Public Property ves_lot_codice() As String
            Get
                Return _ves_lot_codice
            End Get
            Set(Value As String)
                _ves_lot_codice = Value
            End Set
        End Property

        Private _lot_data_scadenza As Date
        Public Property lot_data_scadenza() As Date
            Get
                Return _lot_data_scadenza
            End Get
            Set(Value As Date)
                _lot_data_scadenza = Value
            End Set
        End Property

        Private _ves_data_registrazione As DateTime?
        Public Property ves_data_registrazione() As DateTime?
            Get
                Return _ves_data_registrazione
            End Get
            Set(Value As DateTime?)
                _ves_data_registrazione = Value
            End Set
        End Property

        Private _ves_cns_registrazione As String
        Public Property ves_cns_registrazione() As String
            Get
                Return _ves_cns_registrazione
            End Get
            Set(Value As String)
                _ves_cns_registrazione = Value
            End Set
        End Property

        Private _ves_data_ultima_variazione As DateTime?
        Public Property ves_data_ultima_variazione() As DateTime?
            Get
                Return _ves_data_ultima_variazione
            End Get
            Set(Value As DateTime?)
                _ves_data_ultima_variazione = Value
            End Set
        End Property

        Private _ves_ute_id_ultima_variazione As Int64?
        Public Property ves_ute_id_ultima_variazione() As Int64?
            Get
                Return _ves_ute_id_ultima_variazione
            End Get
            Set(Value As Int64?)
                _ves_ute_id_ultima_variazione = Value
            End Set
        End Property

        Private _ves_ute_id As Int64?
        Public Property ves_ute_id() As Int64?
            Get
                Return _ves_ute_id
            End Get
            Set(Value As Int64?)
                _ves_ute_id = Value
            End Set
        End Property

        Private _ves_comune_o_stato As String
        Public Property ves_comune_o_stato() As String
            Get
                Return _ves_comune_o_stato
            End Get
            Set(Value As String)
                _ves_comune_o_stato = Value
            End Set
        End Property

        Public ReadOnly Property ves_data_effettuazione() As String
            Get
                If ves_dataora_effettuazione = DateTime.MinValue Then
                    Return String.Empty
                End If
                Return ves_dataora_effettuazione.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
            End Get
        End Property

        Public ReadOnly Property ves_ora_effettuazione() As String
            Get
                If ves_dataora_effettuazione = DateTime.MinValue Then
                    Return String.Empty
                End If
                Return ves_dataora_effettuazione.ToString("HH:mm", System.Globalization.CultureInfo.InvariantCulture)
            End Get
        End Property

        Private _reazioneAvversa As ReazioneAvversa
        Public Property ReazioneAvversa() As ReazioneAvversa
            Get
                Return _reazioneAvversa
            End Get
            Set(Value As ReazioneAvversa)
                _reazioneAvversa = Value
            End Set
        End Property

        Private ves_dataora_effettuazione_ As Date
        Public Property ves_dataora_effettuazione() As Date
            Get
                Return ves_dataora_effettuazione_
            End Get
            Set(Value As Date)
                ves_dataora_effettuazione_ = Value
            End Set
        End Property

        Private ves_amb_codice_ As Integer
        Public Property ves_amb_codice() As Integer
            Get
                Return ves_amb_codice_
            End Get
            Set(Value As Integer)
                ves_amb_codice_ = Value
            End Set
        End Property

        Private ves_cns_codice_ As String
        Public Property ves_cns_codice() As String
            Get
                Return ves_cns_codice_
            End Get
            Set(Value As String)
                ves_cns_codice_ = Value
            End Set
        End Property

        Private _ves_mal_codice_malattia As String
        Public Property ves_mal_codice_malattia() As String
            Get
                Return _ves_mal_codice_malattia
            End Get
            Set(Value As String)
                _ves_mal_codice_malattia = Value
            End Set
        End Property

        Private ves_ope_codice_ As String
        Public Property ves_ope_codice() As String
            Get
                Return ves_ope_codice_
            End Get
            Set(Value As String)
                ves_ope_codice_ = Value
            End Set
        End Property

        Private ves_med_vaccinante_codice_ As String
        Public Property ves_med_vaccinante_codice() As String
            Get
                Return ves_med_vaccinante_codice_
            End Get
            Set(Value As String)
                ves_med_vaccinante_codice_ = Value
            End Set
        End Property

        Private ves_luogo_ As String
        Public Property ves_luogo() As String
            Get
                Return ves_luogo_
            End Get
            Set(Value As String)
                ves_luogo_ = Value
            End Set
        End Property

        Private _ves_stato As String
        Public Property ves_stato() As String
            Get
                Return _ves_stato
            End Get
            Set(Value As String)
                _ves_stato = Value
            End Set
        End Property

        Private _ves_cic_codice As String
        Public Property ves_cic_codice() As String
            Get
                Return _ves_cic_codice
            End Get
            Set(Value As String)
                _ves_cic_codice = Value
            End Set
        End Property

        Private _ves_importo As Decimal?
        Public Property ves_importo() As Decimal?
            Get
                Return _ves_importo
            End Get
            Set(Value As Decimal?)
                _ves_importo = Value
            End Set
        End Property

        Private _ves_codice_esenzione As String
        Public Property ves_codice_esenzione() As String
            Get
                Return _ves_codice_esenzione
            End Get
            Set(Value As String)
                _ves_codice_esenzione = Value
            End Set
        End Property

        Private _ves_accesso As String
        Public Property ves_accesso() As String
            Get
                Return _ves_accesso
            End Get
            Set(Value As String)
                _ves_accesso = Value
            End Set
        End Property

        Private _ves_esito As String
        Public Property ves_esito() As String
            Get
                Return _ves_esito
            End Get
            Set(Value As String)
                _ves_esito = Value
            End Set
        End Property

        Private _ves_ope_in_ambulatorio As String
        Public Property ves_ope_in_ambulatorio() As String
            Get
                Return _ves_ope_in_ambulatorio
            End Get
            Set(Value As String)
                _ves_ope_in_ambulatorio = Value
            End Set
        End Property

        Private _ves_in_campagna As String
        Public Property ves_in_campagna() As String
            Get
                Return _ves_in_campagna
            End Get
            Set(Value As String)
                _ves_in_campagna = Value
            End Set
        End Property

        Private _ves_note As String
        Public Property ves_note() As String
            Get
                Return _ves_note
            End Get
            Set(Value As String)
                _ves_note = Value
            End Set
        End Property

        Private _ves_flag_fittizia As String
        Public Property ves_flag_fittizia() As String
            Get
                Return _ves_flag_fittizia
            End Get
            Set(Value As String)
                _ves_flag_fittizia = Value
            End Set
        End Property

        Private _ves_usl_inserimento As String
        Public Property ves_usl_inserimento() As String
            Get
                Return _ves_usl_inserimento
            End Get
            Set(value As String)
                _ves_usl_inserimento = value
            End Set
        End Property

        Private _ves_usl_scadenza As String
        Public Property ves_usl_scadenza() As String
            Get
                Return _ves_usl_scadenza
            End Get
            Set(value As String)
                _ves_usl_scadenza = value
            End Set
        End Property

        Private _ves_data_scadenza As DateTime?
        Public Property ves_data_scadenza() As DateTime?
            Get
                Return _ves_data_scadenza
            End Get
            Set(value As DateTime?)
                _ves_data_scadenza = value
            End Set
        End Property

        Private _ves_ute_id_scadenza As Long?
        Public Property ves_ute_id_scadenza() As Long?
            Get
                Return _ves_ute_id_scadenza
            End Get
            Set(value As Long?)
                _ves_ute_id_scadenza = value
            End Set
        End Property

        Private _ves_data_ripristino As DateTime
        Public Property ves_data_ripristino() As DateTime
            Get
                Return _ves_data_ripristino
            End Get
            Set(value As DateTime)
                _ves_data_ripristino = value
            End Set
        End Property

        Private _ves_ute_id_ripristino As Long?
        Public Property ves_ute_id_ripristino() As Long?
            Get
                Return _ves_ute_id_ripristino
            End Get
            Set(value As Long?)
                _ves_ute_id_ripristino = value
            End Set
        End Property

        Private _ves_flag_visibilita_vac_centrale As String
        Public Property ves_flag_visibilita_vac_centrale() As String
            Get
                Return _ves_flag_visibilita_vac_centrale
            End Get
            Set(value As String)
                _ves_flag_visibilita_vac_centrale = value
            End Set
        End Property

        Private _ves_note_acquisizione_vac_centrale As String
        Public Property ves_note_acquisizione_vac_centrale() As String
            Get
                Return _ves_note_acquisizione_vac_centrale
            End Get
            Set(value As String)
                _ves_note_acquisizione_vac_centrale = value
            End Set
        End Property

        Public Property DataEliminazione() As DateTime?

        Public Property IdUtenteEliminazione() As Int64?

        Public Property CodiceOrigineEtnica() As String

        Public Property ves_id_acn As String

        Public Property ves_provenienza As String

        Public Property ves_mal_codice_cond_sanitaria As String

		Public Property ves_rsc_codice As String

		Public Property ves_tpa_guid_tipi_pagamento As Guid
        Public Property ves_lot_data_scadenza As DateTime?
        Public Property ves_antigene As String
        Public Property ves_tipo_erogatore As String
        Public Property ves_codice_struttura As String
		Public Property ves_usl_cod_somministrazione As String
		Public Property ves_tipo_associazione_acn As String

#Region "Proprieta in JOIN"

		' Non usare queste proprietà perchè non tutte 
		' le query che ritornano l'oggetto le valorizzano
		' Sono rimaste solo perchè è impossibile trovare 
		' i riferimenti delle proprietà nel markup.

		Private cns_descrizione_ As String
        <Obsolete>
        Public Property cns_descrizione() As String
            Get
                Return cns_descrizione_
            End Get
            Set(Value As String)
                cns_descrizione_ = Value
            End Set
        End Property

        Private amb_descrizione_ As String
        <Obsolete>
        Public Property amb_descrizione() As String
            Get
                Return amb_descrizione_
            End Get
            Set(Value As String)
                amb_descrizione_ = Value
            End Set
        End Property

        Private ope_nome_ As String
        <Obsolete>
        Public Property ope_nome() As String
            Get
                Return ope_nome_
            End Get
            Set(Value As String)
                ope_nome_ = Value
            End Set
        End Property

        Private ves_med_vaccinante_nome_ As String
        <Obsolete>
        Public Property ves_med_vaccinante_nome() As String
            Get
                Return ves_med_vaccinante_nome_
            End Get
            Set(Value As String)
                ves_med_vaccinante_nome_ = Value
            End Set
        End Property

        Private _vii_descrizione As String
        <Obsolete>
        Public Property vii_descrizione() As String
            Get
                Return _vii_descrizione
            End Get
            Set(Value As String)
                _vii_descrizione = Value
            End Set
        End Property

#End Region

#End Region

#Region " Costruttori "

        Public Sub New()

            Me.ReazioneAvversa = New ReazioneAvversa()

        End Sub

#End Region

    End Class


End Namespace
