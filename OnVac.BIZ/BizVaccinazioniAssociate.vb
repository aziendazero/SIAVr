Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities

Public Class BizVaccinazioniAssociate
    Inherits BizClass

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos)
        MyBase.New(genericprovider, settings, contextInfos, Nothing)
    End Sub

#End Region

#Region " Public "

    Public Function GetVaccinazioniAssociabili() As List(Of VaccinazioneAssociabile)

        Return Me.GenericProvider.VaccinazioniAssociate.GetVaccinazioniAssociabili()

    End Function

    Public Function GetCodiciVaccinazioniAssociateAMalattia(codiceMalattia As String) As List(Of String)

        Return Me.GenericProvider.VaccinazioniAssociate.GetCodiciVaccinazioniAssociateAMalattia(codiceMalattia)

    End Function


    Public Sub SalvaVaccinazioniAssociateAMalattia(codiceMalattia As String, codiciVacAss As List(Of String))

        Dim ownTransaction As Boolean = False

        Try

            If Me.GenericProvider.Transaction Is Nothing Then
                Me.GenericProvider.BeginTransaction()
                ownTransaction = True
            End If

            'Elimino le link con le vaccinazioni
            Me.GenericProvider.VaccinazioniAssociate.DeleteVaccinazioniAssociateAMalattia(codiceMalattia)

            'Aggiungo le link aggiornate con le vaccinazioni
            Me.GenericProvider.VaccinazioniAssociate.InsertVaccinazioniAssociateAMalattia(codiceMalattia, codiciVacAss)

            If ownTransaction Then
                Me.GenericProvider.Commit()
            End If

        Catch ex As Exception

            If ownTransaction Then
                Me.GenericProvider.Rollback()
            End If

            ex.InternalPreserveStackTrace()
            Throw

        End Try

    End Sub

    Public Function GetVaccinazioniAssociateACategoriaRischio(codiceCatRischio As String) As List(Of String)

        Return Me.GenericProvider.VaccinazioniAssociate.GetCodiciVaccinazioniAssociateACategoriaRischio(codiceCatRischio)

    End Function

    Public Sub SalvaVaccinazioniAssociateACategoriaRischio(codiceCatRis As String, codiciVacAss As List(Of String))

        Dim ownTransaction As Boolean = False

        Try

            If Me.GenericProvider.Transaction Is Nothing Then
                Me.GenericProvider.BeginTransaction()
                ownTransaction = True
            End If
            Me.GenericProvider.VaccinazioniAssociate.DeleteVaccinazioniAssociateACategoriaRischio(codiceCatRis)
            Me.GenericProvider.VaccinazioniAssociate.InsertVaccinazioniAssociateACategoriaRischio(codiceCatRis, codiciVacAss)

            If ownTransaction Then
                Me.GenericProvider.Commit()
            End If

        Catch ex As Exception

            If ownTransaction Then
                Me.GenericProvider.Rollback()
            End If

            ex.InternalPreserveStackTrace()
            Throw

        End Try

    End Sub

#End Region

End Class
