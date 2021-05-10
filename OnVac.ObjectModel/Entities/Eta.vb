Namespace Entities

    Public Class Eta

#Region " Property "

        Private _giorniTotali As Int32
        Public ReadOnly Property GiorniTotali() As Int32
            Get
                Return _giorniTotali
            End Get
        End Property

        Private _giorni As Int32
        Public ReadOnly Property Giorni() As Int32
            Get
                Return _giorni
            End Get
        End Property

        Private _mesi As Int32
        Public ReadOnly Property Mesi() As Int32
            Get
                Return _mesi
            End Get
        End Property

        Private _anni As Int32
        Public ReadOnly Property Anni() As Int32
            Get
                Return _anni
            End Get
        End Property

#End Region

#Region " Costruttori "

        Public Sub New(giorniTotali As Object)

            If Not giorniTotali Is Nothing AndAlso Not giorniTotali Is DBNull.Value AndAlso giorniTotali.ToString() <> "" Then

                _giorniTotali = Convert.ToInt32(giorniTotali)

                _giorni = (_giorniTotali Mod 360) Mod 30
                _mesi = Convert.ToInt32(Fix((_giorniTotali Mod 360) / 30))
                _anni = Convert.ToInt32(Fix(_giorniTotali / 360))

            Else

                _giorniTotali = 0

                _giorni = 0
                _mesi = 0
                _anni = 0

            End If

        End Sub

        Public Sub New(giorni As Object, mesi As Object, anni As Object)

            _giorniTotali = 0

            If Not giorni Is Nothing AndAlso Not giorni Is DBNull.Value AndAlso giorni.ToString() <> "" Then
                _giorni = Convert.ToInt32(giorni)
                _giorniTotali += _giorni
            Else
                _giorni = 0
            End If

            If Not mesi Is Nothing AndAlso Not mesi Is DBNull.Value AndAlso mesi.ToString() <> "" Then
                _mesi = Convert.ToInt32(mesi)
                _giorniTotali += _mesi * 30
            Else
                _mesi = 0
            End If

            If Not anni Is Nothing AndAlso Not anni Is DBNull.Value AndAlso anni.ToString() <> "" Then
                _anni = Convert.ToInt32(anni)
                _giorniTotali += _anni * 360
            Else
                _anni = 0
            End If

        End Sub

#End Region

    End Class

End Namespace