Imports System.Collections.Generic


Namespace Entities


    Public Class SedutaCalendario

        Private _id As String
        Public Property ID() As String
            Get
                Return _id
            End Get
            Set(ByVal value As String)
                _id = value
            End Set
        End Property

        Private _descrizione As String
        Public Property Descrizione() As String
            Get
                Return _descrizione
            End Get
            Set(ByVal value As String)
                _descrizione = value
            End Set
        End Property

        Private _seduta As Integer
        Public Property Seduta() As Integer
            Get
                Return _seduta
            End Get
            Set(ByVal value As Integer)
                _seduta = value
            End Set
        End Property

        Private _eta As Integer
        Public Property Eta() As Integer
            Get
                Return _eta
            End Get
            Set(ByVal value As Integer)
                _eta = value
            End Set
        End Property

        Private _intervallo As Integer
        Public Property Intervallo() As Integer
            Get
                Return _intervallo
            End Get
            Set(ByVal value As Integer)
                _intervallo = value
            End Set
        End Property

        Private _intervalloProssima As Integer
        Public Property IntervalloProssima() As Integer
            Get
                Return _intervalloProssima
            End Get
            Set(ByVal value As Integer)
                _intervalloProssima = value
            End Set
        End Property


        Private _durata As Integer
        Public Property Durata() As Integer
            Get
                Return _durata
            End Get
            Set(ByVal value As Integer)
                _durata = value
            End Set
        End Property


        Private _vaccinazioni As List(Of Vaccinazione)
        Public Property Vaccinazioni() As List(Of Vaccinazione)
            Get
                Return _vaccinazioni
            End Get
            Set(ByVal value As List(Of Vaccinazione))
                _vaccinazioni = value
            End Set
        End Property



        Public Class Vaccinazione

            Private _cicloCodice As String
            Public Property CicloCodice() As String
                Get
                    Return _cicloCodice
                End Get
                Set(ByVal value As String)
                    _cicloCodice = value
                End Set
            End Property


            Private _cicloDescrizione As String
            Public Property CicloDescrizione() As String
                Get
                    Return _cicloDescrizione
                End Get
                Set(ByVal value As String)
                    _cicloDescrizione = value
                End Set
            End Property

            Private _cicloSeduta As Integer
            Public Property CicloSeduta() As Integer
                Get
                    Return _cicloSeduta
                End Get
                Set(ByVal value As Integer)
                    _cicloSeduta = value
                End Set
            End Property

            Private _codice As String
            Public Property Codice() As String
                Get
                    Return _codice
                End Get
                Set(ByVal value As String)
                    _codice = value
                End Set
            End Property


            Private _descrizione As String
            Public Property Descrizione() As String
                Get
                    Return _descrizione
                End Get
                Set(ByVal value As String)
                    _descrizione = value
                End Set
            End Property

            Private _dose As Integer
            Public Property Dose() As Integer
                Get
                    Return _dose
                End Get
                Set(ByVal value As Integer)
                    _dose = value
                End Set
            End Property
        End Class

    End Class

End Namespace

