Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager


Namespace DAL.Oracle

    Public Class DbPazientiVaccinazioniProvider
        Inherits DbProvider
        Implements IPazientiVaccinazioniProvider

#Region " Constructors "

        Public Sub New(ByRef DAM As IDAM)
            MyBase.New(DAM)
        End Sub

#End Region

#Region " Public "

        ''' <summary>
        ''' Restituisce un hashtable con i codici e le descrizioni delle vaccinazioni specificate
        ''' </summary>
        ''' <param name="listCodiciVaccinazioni">Lista di codici delle vaccinazioni</param>
        ''' <remarks>
        ''' </remarks>
        Public Function LoadDescrizioneVaccinazioni(listCodiciVaccinazioni As List(Of String)) As System.Collections.Hashtable Implements IPazientiVaccinazioniProvider.LoadDescrizioneVaccinazioni

            RefurbishDT()   ' pulisce il datatable della classe base, anche se poi non lo usa

            Dim hashVac As New Hashtable()

            If listCodiciVaccinazioni Is Nothing OrElse listCodiciVaccinazioni.Count = 0 Then
                Return hashVac
            End If

            With _DAM.QB

                .NewQuery()

                .AddSelectFields("vac_codice, vac_descrizione")
                .AddTables("t_ana_vaccinazioni")

                ' Creazione parametri per filtro di IN
                Dim codiciVaccinazioni As New System.Text.StringBuilder()

                For Each codiceVac As String In listCodiciVaccinazioni
                    codiciVaccinazioni.AppendFormat("{0},", .AddCustomParam(codiceVac))
                Next

                codiciVaccinazioni.Remove(codiciVaccinazioni.Length - 1, 1)

                .AddWhereCondition("vac_codice", Comparatori.In, codiciVaccinazioni.ToString(), DataTypes.Replace)

            End With

            Try

                Using dr As IDataReader = _DAM.BuildDataReader()

                    If Not dr Is Nothing Then

                        Dim pos_vac_cod As Integer = dr.GetOrdinal("vac_codice")
                        Dim pos_vac_descr As Integer = dr.GetOrdinal("vac_descrizione")

                        While dr.Read()
                            hashVac.Add(dr(pos_vac_cod).ToString(), dr(pos_vac_descr).ToString())
                        End While

                    End If

                End Using

            Catch ex As Exception

                Me.SetErrorMsg("Errore durante il recupero delle vaccinazioni specificate")
                Me.LogError(ex, "DbPazientiVaccinazioniProvider.LoadDescrizioneVaccinazioni: errore lettura db per recupero dati vaccinazioni.")

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return hashVac

        End Function

#End Region

    End Class

End Namespace

