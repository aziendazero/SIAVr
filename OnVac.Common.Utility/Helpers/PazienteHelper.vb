Imports Onit.OnAssistnet.OnVac.Entities

Public Class PazienteHelper


    '<summary> Calcola l'età in giorni, considerando l'anno da 360 giorni </summary>
    Public Shared Function CalcoloEta(ByVal dataNascita As Date) As Eta

        Return CalcoloEtaDaData(dataNascita, Date.Now)

        ' Vecchio metodo impreciso. 
        'Dim r As Integer
        'Dim eta_giorni As Integer = 0

        'eta_giorni = Math.DivRem(Date.Now.Subtract(dataNascita).Days, 365, r) * 360
        'eta_giorni += Math.Floor((r / 365) * 360)

        'Return New Eta(eta_giorni)

    End Function

    'calcolo dell'età del paziente alla data di convocazione (modifica 13/01/2005)
    Public Shared Function CalcoloEtaDaData(dataNascita As Date, dataCalcolo As Date) As Eta

        Dim anniTot As Integer = 0
        Dim mesiTot As Integer = 0
        Dim giorniTot As Integer = 0

        Dim dataAppoggio As Date = dataNascita
        Dim dataTmp As Date = dataAppoggio

        'nel caso in cui si sposti la data di nascita prima di quella di convocazione (modifica 25/01/2004)
        If dataNascita = dataCalcolo Then Return New Eta(0)
        If dataNascita > dataCalcolo Then Return New Eta(0)

        While dataNascita < dataCalcolo
            If dataNascita.Year = dataCalcolo.Year And dataNascita.Month = dataCalcolo.Month Then Exit While
            dataNascita = dataNascita.AddMonths(1)
            mesiTot += 1
        End While

        If dataAppoggio.Day <= dataCalcolo.Day Then
            giorniTot = dataCalcolo.Day - dataAppoggio.Day
        Else
            mesiTot -= 1
            dataNascita = dataNascita.AddMonths(-1)
            giorniTot = (Date.DaysInMonth(dataAppoggio.Year, dataAppoggio.Month) - dataAppoggio.Day) + dataCalcolo.Day
        End If

        If mesiTot >= 12 Then
            anniTot = Math.DivRem(mesiTot, 12, New Integer)
            mesiTot = mesiTot - (anniTot * 12)
        End If

        Return New Eta(giorniTot, mesiTot, anniTot)

    End Function

End Class
