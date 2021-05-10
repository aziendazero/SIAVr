Namespace Entities

    Public Class ElaborazioneVaccinazionePaziente

        Private _Id As Long
        Public Property Id() As Long
            Get
                Return _Id
            End Get
            Set(ByVal value As Long)
                _Id = value
            End Set
        End Property


        Private _CodicePaziente As Long?
        Public Property CodicePaziente() As Long?
            Get
                Return _CodicePaziente
            End Get
            Set(ByVal value As Long?)
                _CodicePaziente = value
            End Set
        End Property

        Private _CodiceRegionalePaziente As String
        Public Property CodiceRegionalePaziente() As String
            Get
                Return _CodiceRegionalePaziente
            End Get
            Set(ByVal value As String)
                _CodiceRegionalePaziente = value
            End Set
        End Property

        Private _TesseraSanitariaPaziente As String
        Public Property TesseraSanitariaPaziente() As String
            Get
                Return _TesseraSanitariaPaziente
            End Get
            Set(ByVal value As String)
                _TesseraSanitariaPaziente = value
            End Set
        End Property

        Private _CodiceFiscalePaziente As String
        Public Property CodiceFiscalePaziente() As String
            Get
                Return _CodiceFiscalePaziente
            End Get
            Set(ByVal value As String)
                _CodiceFiscalePaziente = value
            End Set
        End Property

        Private _NomePaziente As String
        Public Property NomePaziente() As String
            Get
                Return _NomePaziente
            End Get
            Set(ByVal value As String)
                _NomePaziente = value
            End Set
        End Property

        Private _CognomePaziente As String
        Public Property CognomePaziente() As String
            Get
                Return _CognomePaziente
            End Get
            Set(ByVal value As String)
                _CognomePaziente = value
            End Set
        End Property

        Private _DataNascitaPaziente As DateTime?
        Public Property DataNascitaPaziente() As DateTime?
            Get
                Return _DataNascitaPaziente
            End Get
            Set(ByVal value As DateTime?)
                _DataNascitaPaziente = value
            End Set
        End Property

        Private _CodiceComuneNascitaPaziente As String
        Public Property CodiceComuneNascitaPaziente() As String
            Get
                Return _CodiceComuneNascitaPaziente
            End Get
            Set(ByVal value As String)
                _CodiceComuneNascitaPaziente = value
            End Set
        End Property

        Private _DescrizioneComuneNascitaPaziente As String
        Public Property DescrizioneComuneNascitaPaziente() As String
            Get
                Return _DescrizioneComuneNascitaPaziente
            End Get
            Set(ByVal value As String)
                _DescrizioneComuneNascitaPaziente = value
            End Set
        End Property

        Private _SessoPaziente As String
        Public Property SessoPaziente() As String
            Get
                Return _SessoPaziente
            End Get
            Set(ByVal value As String)
                _SessoPaziente = value
            End Set
        End Property

        Private _CodiceVaccinazione As String
        Public Property CodiceVaccinazione() As String
            Get
                Return _CodiceVaccinazione
            End Get
            Set(ByVal value As String)
                _CodiceVaccinazione = value
            End Set
        End Property

        Private _DescrizioneVaccinazione As String
        Public Property DescrizioneVaccinazione() As String
            Get
                Return _DescrizioneVaccinazione
            End Get
            Set(ByVal value As String)
                _DescrizioneVaccinazione = value
            End Set
        End Property

        Private _CodiceAssociazione As String
        Public Property CodiceAssociazione() As String
            Get
                Return _CodiceAssociazione
            End Get
            Set(ByVal value As String)
                _CodiceAssociazione = value
            End Set
        End Property

        Private _DescrizioneAssociazione As String
        Public Property DescrizioneAssociazione() As String
            Get
                Return _DescrizioneAssociazione
            End Get
            Set(ByVal value As String)
                _DescrizioneAssociazione = value
            End Set
        End Property

        Private _DataEffettuazione As DateTime
        Public Property DataEffettuazione() As DateTime
            Get
                Return _DataEffettuazione
            End Get
            Set(ByVal value As DateTime)
                _DataEffettuazione = value
            End Set
        End Property

        Private _CodiceOperatore As String
        Public Property CodiceOperatore() As String
            Get
                Return _CodiceOperatore
            End Get
            Set(ByVal value As String)
                _CodiceOperatore = value
            End Set
        End Property

        Private _NomeOperatore As String
        Public Property NomeOperatore() As String
            Get
                Return _NomeOperatore
            End Get
            Set(ByVal value As String)
                _NomeOperatore = value
            End Set
        End Property

        Private _StatoAcquisizione As Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente
        Public Property StatoAcquisizione() As Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente
            Get
                Return _StatoAcquisizione
            End Get
            Set(ByVal value As Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente)
                _StatoAcquisizione = value
            End Set
        End Property

        Private _MessaggioAcquisizione As String
        Public Property MessaggioAcquisizione() As String
            Get
                Return _MessaggioAcquisizione
            End Get
            Set(ByVal value As String)
                _MessaggioAcquisizione = value
            End Set
        End Property

        Private _DataAcquisizione As DateTime?
        Public Property DataAcquisizione() As DateTime?
            Get
                Return _DataAcquisizione
            End Get
            Set(ByVal value As DateTime?)
                _DataAcquisizione = value
            End Set
        End Property

        Private _IdProcessoAcquisizione As Long?
        Public Property IdProcessoAcquisizione() As Long?
            Get
                Return _IdProcessoAcquisizione
            End Get
            Set(ByVal value As Long?)
                _IdProcessoAcquisizione = value
            End Set
        End Property

        Private _CodicePazienteAcquisizione As Long?
        Public Property CodicePazienteAcquisizione() As Long?
            Get
                Return _CodicePazienteAcquisizione
            End Get
            Set(ByVal value As Long?)
                _CodicePazienteAcquisizione = value
            End Set
        End Property


        'Private _GuidProcessoCaricamento As Guid
        'Public Property GuidProcessoCaricamento() As Guid
        '    Get
        '        Return _GuidProcessoCaricamento
        '    End Get
        '    Set(ByVal value As Guid)
        '        _GuidProcessoCaricamento = value
        '    End Set
        'End Property

    End Class

End Namespace