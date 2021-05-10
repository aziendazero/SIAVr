Imports System.Collections.Generic

Namespace Entities

    <Serializable()>
    Public Class ColonneAlias

#Region " Properties "

        Public Property Tabella() As String

        Public Property CampoCodicePaziente() As String

        Public Property CampoCodicePazienteOld() As String

        Public Property CampiIndice() As List(Of String)

        Public Property Campi() As List(Of String)

        Public Property Ordine() As Integer

        Public Property CampoOrdinamento() As String

#End Region

#Region " Constructor "

        Public Sub New()
        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Restituisce la stringa risultato della concatenazione di tutti i campi indice, separati dalla virgola
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetStringCampiIndice() As String

            Return String.Join(",", Me.CampiIndice.ToArray())

        End Function

        ''' <summary>
        ''' Imposta la proprietà CampiIndice come lista composta dalle stringhe specificate, splittate in base alla ","
        ''' </summary>
        ''' <param name="campiIndice"></param>
        ''' <remarks></remarks>
        Public Sub SetCampiIndice(campiIndice As String)

            Me.CampiIndice = New List(Of String)()

            If Not String.IsNullOrEmpty(campiIndice) Then

                Dim arrayCampi As String() = campiIndice.Split(","c)

                For i As Integer = 0 To arrayCampi.Length - 1
                    Me.CampiIndice.Add(arrayCampi(i))
                Next

            End If

        End Sub

        ''' <summary>
        ''' Restituisce la stringa risultato della concatenazione di tutti i campi, separati dalla virgola
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetStringCampi() As String

            Return String.Join(",", Me.Campi.ToArray())

        End Function

        ''' <summary>
        ''' Imposta la proprietà Campi come lista composta dalle stringhe specificate, splittate in base alla ","
        ''' </summary>
        ''' <param name="campi"></param>
        ''' <remarks></remarks>
        Public Sub SetCampi(campi As String)

            Me.Campi = New List(Of String)()

            If Not String.IsNullOrEmpty(campi) Then

                Dim arrayCampi As String() = campi.Split(","c)

                For i As Integer = 0 To arrayCampi.Length - 1
                    Me.Campi.Add(arrayCampi(i))
                Next

            End If

        End Sub

#End Region

    End Class

End Namespace
