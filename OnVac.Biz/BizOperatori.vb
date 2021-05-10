Imports System.Collections.Generic

Imports Onit.OnAssistnet.Data
Imports Onit.OnAssistnet.OnVac.DAL


Public Class BizOperatori
	Inherits BizClass

#Region " Constructors "

	Public Sub New(genericprovider As DbGenericProvider, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos)

		MyBase.New(genericprovider, settings, contextInfos, Nothing)

	End Sub

#End Region

#Region " Public "

    ''' <summary>
    ''' Restituisce gli operatori del centro vaccinale corrente, in base alla descrizione specificata
    ''' </summary>
    ''' <param name="Descrizione"></param>
    ''' <returns></returns>
    Public Function GetListOperatori(Descrizione As String) As List(Of Entities.Operatore)

        Return GenericProvider.Operatori.GetListOperatori(Descrizione, ContextInfos.CodiceCentroVaccinale)

    End Function

    ''' <summary>
    ''' Restituisce la lista dei consultori a cui l'operatore è abilitato
    ''' </summary>
    ''' <param name="codiceOpe"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetListConsultoriOperatori(codiceOpe As String) As List(Of Entities.ConsultorioOperatore)

        Return GenericProvider.Operatori.GetListConsultoriOperatori(codiceOpe)

    End Function

	''' <summary>
	''' Salvataggio abilitazioni consultori per l'operatore specificato. 
	''' </summary>
	''' <param name="codiceOp"></param>
	''' <param name="listConsultoriOpe"></param>
	''' <remarks></remarks>
	Public Sub SaveConsultoriOperatore(codiceOp As String, listConsultoriOpe As List(Of Entities.ConsultorioOperatore))

		Try
			Me.GenericProvider.BeginTransaction()

			' Eliminazione abilitazioni precedenti
			Me.GenericProvider.Operatori.DeleteConsultoriOperatore(codiceOp)

			If Not listConsultoriOpe Is Nothing AndAlso listConsultoriOpe.Count > 0 Then



				' Inserimento abilitazioni correnti
				For Each consultorioOpItem As Entities.ConsultorioOperatore In listConsultoriOpe
					Me.GenericProvider.Operatori.InsertConsultorioOperatore(consultorioOpItem)
				Next

			End If

			Me.GenericProvider.Commit()

		Catch ex As Exception

			Me.GenericProvider.Rollback()
			ex.InternalPreserveStackTrace()
			Throw

		End Try

	End Sub



	''' <summary>
	''' Inserimento di un'associazione operatore-consultorio
	''' </summary>
	''' <param name="codiceOpe"></param>
	''' <param name="codiceConsultorio"></param>
	''' <remarks></remarks>
	Public Function InserConsultoriotOperatore(codiceOpe As String, codiceConsultorio As String) As BizGenericResult

		Dim result As New BizGenericResult()
		result.Success = True
		result.Message = String.Empty

		' Se l'operatore non ha consultori associati 
		Dim isDefault As Boolean = True

		Dim listConsultoriOperatore As List(Of Entities.ConsultorioOperatore) = GetListConsultoriOperatori(codiceOpe)

		If Not listConsultoriOperatore.IsNullOrEmpty() Then

			If listConsultoriOperatore.Any(Function(c) c.CodiceConsultorio = codiceConsultorio) Then
				result.Success = False
				result.Message = "Operatore già associato al centro vaccinale"
				Return result
			End If



		End If

		Dim consultorioOperatore As New Entities.ConsultorioOperatore()
		consultorioOperatore.CodiceConsultorio = codiceConsultorio
		consultorioOperatore.CodiceOperatore = codiceOpe


		Me.GenericProvider.Operatori.InsertConsultorioOperatore(consultorioOperatore)

		Return result

	End Function

	''' <summary>
	''' Elimina l'associazione tra il consultorio e l'operatore specificati
	''' </summary>
	''' <param name="codiceOpe"></param>
	''' <param name="codiceConsultorio"></param>
	''' <remarks></remarks>
	Public Sub DeleteConsultorioOperatore(codiceOpe As String, codiceConsultorio As String)

		Me.GenericProvider.Operatori.DeleteConsultorioOperatore(codiceConsultorio, codiceOpe)

	End Sub


    ''' <summary>
    ''' Aggiorna l'id ACN dell'operatore
    ''' </summary>
    ''' <param name="codiceOperatore"></param>
    ''' <param name="idACN"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateIdACNOperatore(codiceOperatore As String, idACN As String) As Integer

        Return Me.GenericProvider.Operatori.UpdateIdACNOperatore(codiceOperatore, idACN)

    End Function

    Public Function GetOperatoreById(codice As String) As Entities.Operatore
        Return GenericProvider.Operatori.GetOperatoreById(codice)
    End Function

    Public Function GetOperatoriByIdRSATipoOpe(idRSA As String, TipoOperatore As String, Nome As String, uslCodice As String) As List(Of Entities.Operatore)
        Dim ope As New List(Of Entities.Operatore)

        ope = GenericProvider.Operatori.GetOperatoriByIdRSATipoOpe(idRSA, TipoOperatore, Nome)
        If ope.Count() > 0 Then
            Return ope
        Else
            ope = GenericProvider.Operatori.GetOperatoriByIdRSATipoOpeDISTINCT(TipoOperatore, Nome, idRSA)
        End If
        Return ope
    End Function
#End Region

End Class
